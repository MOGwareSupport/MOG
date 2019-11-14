//--------------------------------------------------------------------------------
//	MOG_ControllerInbox.cpp
//	
//	
//--------------------------------------------------------------------------------

#include "StdAfx.h"

#include "MOG_Define.h"
#include "MOG_Tokens.h"
#include "MOG_Time.h"
#include "MOG_ControllerProject.h"
#include "MOG_ControllerAsset.h"
#include "MOG_ControllerSystem.h"
#include "MOG_ControllerRepository.h"
#include "MOG_ControllerPackage.h"
#include "MOG_DBAssetAPI.h"
#include "MOG_DBPackageAPI.h"
#include "MOG_DosUtils.h"
#include "MOG_DBInboxAPI.h"
#include "MOG_Progress.h"
#include "MOG_CommandFactory.h"

#include "MOG_ControllerInbox.h"

using namespace MOG_CoreControls;

bool MOG_ControllerInbox::RipAsset(MOG_Filename *assetFilename)
{
	bool bProcessed = false;
	bool bFailed = false;

	// Never do anything if we are not online?
	MOG_NetworkClient* pClient = dynamic_cast<MOG_NetworkClient*>(MOG_ControllerSystem::GetCommandManager()->GetNetwork());
	if (pClient && !pClient->Connected())
	{
		String *message = String::Concat( S"MOG cannot process this command because the connection to the MOG Server appears to be down.\n",
										  S"ASSET: ", assetFilename->GetAssetLabel(), S" was not processed.");
		MOG_Prompt::PromptMessage(S"Process", message, "", MOG_ALERT_LEVEL::ERROR);
		return false;
	}

	// Make sure this is a legitmate asset?
	if (assetFilename->GetFilenameType() == MOG_FILENAME_TYPE::MOG_FILENAME_Asset)
	{
		MOG_ControllerAsset *asset = MOG_ControllerAsset::OpenAsset(assetFilename);
		if (asset)
		{
			MOG_Command *ripCommand = NULL;
			MOG_Properties *pProperties = asset->GetProperties();

			// Make sure this is not a NativeAsset
			if (!pProperties->NativeDataType)
			{
				// Check if this asset now needs to be moved to the drafts?
				if (assetFilename->IsInbox())
				{
					// Move the asset and indicate it is now waiting to be ripped
					if (MoveToBox(asset, S"Drafts", MOG_AssetStatusType::Waiting))
					{
						// Fixup our local assetFilename to follow the asset
						assetFilename = asset->GetAssetFilename();
					}
				}

				// Update the asset's ripped time
				pProperties->RippedTime = MOG_Time::GetVersionTimestamp();
				// Set creator to the local login user
				pProperties->Creator = assetFilename->GetUserName();
				pProperties->Owner = assetFilename->GetUserName();

				// Make sure the asset contain a 'Files.Imported' directory
				String *filesDirectory = MOG_ControllerAsset::GetAssetImportedDirectory(pProperties);
				MOG_ASSERT_THROW(DosUtils::DirectoryExistFast(filesDirectory), MOG_Exception::MOG_EXCEPTION_MissingFile, "Asset is missing the 'Files.Imported' directory");

				// Indicate a Waiting status
				MOG_ControllerInbox::UpdateInboxView(asset, MOG_AssetStatusType::Waiting);

				// Make sure we haven't failed?
				if (!bFailed)
				{
					// Clean the asset of any previous rip information and files
					asset->CleanAsset();

					// Check if this asset is a NativeDataType?
					if (pProperties->NativeDataType)
					{
						// Record all the files and return if any assets failed
						if (MOG_ControllerAsset::RecordAllAssetFiles(asset))
						{
							// Set the asset sizes for each platform of this asset
							MOG_ControllerAsset::SetAssetSizes(asset);

							bProcessed = true;
						}
					}
					else
					{
						// Obtain a unique jobLabel for this rip
						String *jobLabel = String::Concat(S"Rip.", MOG_ControllerSystem::GetComputerName(), S".", MOG_Time::GetVersionTimestamp());
						// Start the distributed rip for this Asset by sending our request to the server
						ripCommand = MOG_CommandFactory::Setup_AssetRipRequest(jobLabel, asset->GetAssetFilename(), S"");

						// Check if the user has specified mUseOnlyMySlaves?
						if (MOG_ControllerSystem::GetCommandManager()->mUseOnlyMySlaves)
						{
							// Specify us as the only ValidSlaves
							ripCommand->SetValidSlaves(MOG_ControllerSystem::GetComputerName());
						}
					}
				}
			}
			else
			{
				// Nothing needs to be done for this asset so at least mark it as processed
				MOG_ControllerInbox::UpdateInboxView(asset, MOG_AssetStatusType::Processed);
			}

			// Always make sure to close the asset when we are finished
			asset->Close();

			// Check if we constructed a rip command?
			if (ripCommand)
			{
				// We can't process the rip command until after we close the asset or else we may have a sharing violation with a slave
				bProcessed = MOG_ControllerSystem::GetCommandManager()->CommandProcess(ripCommand);
				// Start the job
				MOG_ControllerProject::StartJob(ripCommand->GetJobLabel());
			}
		}
	}

	return bProcessed;
}


bool MOG_ControllerInbox::BlessAsset(MOG_Filename *assetFilename, String *comment, bool maintainLock, BackgroundWorker* worker)
{
	bool bBlessed = false;

	// Create a unique jobLabel for this bless
	String *jobLabel = String::Concat(S"Bless.", MOG_ControllerSystem::GetComputerName(), S".", MOG_Time::GetVersionTimestamp());
	// Call our normal bless API
	if (BlessAsset(assetFilename, comment, maintainLock, jobLabel, worker))
	{
		// We need to start this job since we were the ones that created the JobLabel
		if (MOG_ControllerProject::StartJob(jobLabel))
		{
			bBlessed = true;
		}
	}

	return bBlessed;
}


bool MOG_ControllerInbox::BlessAsset(MOG_Filename *assetFilename, String *comment, bool maintainLock, String *jobLabel, BackgroundWorker* worker)
{
	bool bBlessed = false;
	bool bFailed = false;
	bool bAutoStartJob = false;

	// Never do anything if we are not online?
	MOG_NetworkClient* pClient = dynamic_cast<MOG_NetworkClient*>(MOG_ControllerSystem::GetCommandManager()->GetNetwork());
	if (pClient && !pClient->Connected())
	{
		String *message = String::Concat( S"MOG cannot process this command because the connection to the MOG Server appears to be down.\n",
										  S"ASSET: ", assetFilename->GetAssetLabel(), S" was not blessed.");
		MOG_Prompt::PromptMessage(S"Bless", message, "", MOG_ALERT_LEVEL::ERROR);
		return false;
	}

	// Check if no j	obLabel was specified?
	if (jobLabel == NULL || jobLabel->Length == 0)
	{
		// Obtain a unique jobLabel for this bless
		jobLabel = String::Concat(S"Bless.", MOG_ControllerSystem::GetComputerName(), S".", MOG_Time::GetVersionTimestamp());
		// If we have to create a default jobLabel then we need to start it
		bAutoStartJob = true;
	}

	// Make sure this is a legitmate asset that has been processed?
	if (assetFilename->GetFilenameType() == MOG_FILENAME_TYPE::MOG_FILENAME_Asset)
	{
		// Proceed to open the specified asset
		MOG_ControllerAsset *asset = MOG_ControllerAsset::OpenAsset(assetFilename);
		if (asset)
		{
			try
			{
				// Check if this is an unblessable asset?
				if (asset->GetProperties()->UnBlessable)
				{
					String *message = String::Concat( S"This asset has been marked as an unblessable asset and cannot be blessed.\n",
													  S"ASSET: ", assetFilename->GetAssetLabel(), S"\n",
													  S"Assets can be marked unblessable if copied (duplicated) to another user's inbox.  Only the original asset remains blessable." );
					MOG_Prompt::PromptMessage(S"Bless", message, "", MOG_ALERT_LEVEL::ERROR);
					return false;
				}
				// Make sure this asset doesn't still need to be processed?
				if (asset->GetProperties()->IsUnprocessed)
				{
					String *message = String::Concat( S"Assets must be processed before thay can be blessed.\n",
													  S"ASSET: ", assetFilename->GetAssetLabel());
					MOG_Prompt::PromptMessage(S"Bless", message, "", MOG_ALERT_LEVEL::ERROR);
					return false;
				}

				bool bFailed = false;

				// We need to declare the command here so we can make sure to properly close the asset before sending the blessCommand
				MOG_Command *blessCommand = NULL;

				// Retain the maintain lock property
				asset->GetProperties()->MaintainLock = maintainLock;

				// Check if we need to establish a BlessedTime for this asset?
				if (asset->GetProperties()->BlessedTime->Length == 0)
				{
					// Obtain the BlessedTime from the jobLabel's timestamp
					asset->GetProperties()->BlessedTime = ObtainTimestampFromJobLabel(jobLabel);
				}

				// Get the proper bless target for this asset
				String *blessTarget = GetAssetBlessTarget(assetFilename, asset->GetProperties());
				if (blessTarget->Length)
				{
					// Send the asset onto the specified bless target's inbox
					if (MoveAssetToUserInbox(asset, blessTarget, comment, MOG_AssetStatusType::Blessed, worker))
					{
						bBlessed = true;
					}
					else
					{
						bFailed = true;
					}
				}
				else
				{
					// Check if this Asset is outside of the MOG Repository?
					if (!assetFilename->IsWithinRepository())
					{
						// Make sure this asset's classification exists
						if (MOG_ControllerProject::GetProject()->ClassificationAdd(asset->GetAssetFilename()->GetAssetClassification()))
						{
							// Get the target for this asset in the MOG Repository using the BlessedRepositoryRevision
							MOG_Filename *target = new MOG_Filename(MOG_ControllerRepository::GetAssetBlessedVersionPath(asset->GetAssetFilename(), asset->GetProperties()->BlessedTime));

							// Make sure this is a valid Asset?
							// Make sure that we are blessing to this Project's master repository?
							// Make sure that the target is really going into the 'Assets' directory?
							if (target->GetFilenameType() == MOG_FILENAME_Asset &&
								String::Compare(target->GetProjectPath(), MOG_ControllerProject::GetProject()->GetProjectPath(), true) == 0 &&
								target->IsWithinRepository())
							{
								// Move the asset in with the new version to MasterData
								if (MOG_ControllerInbox::Move(asset, target, MOG_AssetStatusType::Blessing, true))
								{
									// Post the comment to this asset
									asset->PostComment(comment, true);

									// Add this asset to the database now that it exists in the MOG Repository
									if (MOG_DBAssetAPI::AddAsset(asset->GetAssetFilename(), asset->GetProperties()->Creator, asset->GetAssetFilename()->GetVersionTimeStamp()))
									{
										// Add the asset's revision information
										if (MOG_ControllerRepository::AddAssetRevisionInfo(asset, true))
										{
											// Check if the user specified mUseOnlyMySlaves?
											String *validSlaves = S"";
											if (MOG_ControllerSystem::GetCommandManager()->mUseOnlyMySlaves)
											{
												// Specify us as the only ValidSlaves
												validSlaves = MOG_ControllerSystem::GetComputerName();
											}

											// Create the blessCommand to be sent to the server
											blessCommand = MOG_CommandFactory::Setup_Bless(asset->GetAssetFilename(), jobLabel, validSlaves);

											// Check if this asset is a package?
											if (asset->GetProperties()->IsPackage)
											{
												// PackageMerge places packages directly into the repository so this code will never be hit unless
												// this package was manually blessed from a user's Inbox.  We need to preserve the package's groups by
												// populating the new package revision information from the current revision of this package
												MOG_DBPackageAPI::PopulateNewPackageVersion(MOG_ControllerProject::GetAssetCurrentBlessedVersionPath(asset->GetAssetFilename()), asset->GetAssetFilename(), NULL);
											}
										}
										else
										{
											// Inform the Server about this
											String *message = String::Concat(	S"The System failed to add new revision of '", assetFilename->GetAssetLabel(), S"' in the MOG Repository.\n",
																				S"The Asset was not blessed correctly.");
											MOG_Report::ReportMessage(S"Bless Failed", message, "", MOG_ALERT_LEVEL::ERROR);
										}
									}
								}
								else
								{
									// Inform the Server about this
									String *message = String::Concat(	S"The System was unable to revision ", assetFilename->GetAssetLabel(), S" in the MOG Repository\n",
																		S"The Asset was not blessed correctly.");
									MOG_Report::ReportMessage(S"Bless Failed", message, "", MOG_ALERT_LEVEL::ERROR);
									bFailed = true;
								}
							}
							else
							{
								// Inform the Server about this
								String *message = String::Concat(	assetFilename->GetAssetLabel(), S" Failed the Bless Path Checks.\n",
																	S"The Asset was not blessed correctly.");
								MOG_Report::ReportMessage(S"Bless Failed", message, "", MOG_ALERT_LEVEL::ERROR);
								bFailed = true;
							}
						}
						else
						{
							// Inform the Server about this
							String *message = String::Concat(	S"Failed to create the new classification.\n",
																S"CLASSIFICATION: ", assetFilename->GetAssetClassification(), S"\n\n",
																S"The Asset was not blessed correctly.");
							MOG_Report::ReportMessage(S"Bless Failed", message, "", MOG_ALERT_LEVEL::ERROR);
							bFailed = true;
						}
					}
					else
					{
						// Inform the Server about this
						String *message = String::Concat(	S"The specified asset has already been blessed into the MOG Repository.\n",
															S"ASSET: ", assetFilename->GetAssetLabel());
						MOG_Report::ReportMessage(S"Bless Failed", message, "", MOG_ALERT_LEVEL::ERROR);
					}
				}

				if (!bFailed)
				{
					// Check if we constructed a bless command?
					if (blessCommand)
					{
						// We must process this command after closing the asset or else we may have a sharing violation with a slave
						bBlessed = MOG_ControllerSystem::GetCommandManager()->CommandProcess(blessCommand);

						// Check if we need to start the job?
						if (bAutoStartJob)
						{
							// Start the job
							MOG_ControllerProject::StartJob(jobLabel);
						}
					}
				}
			} // end try around MOG_AssetController
			catch(Exception *ex)
			{
				// Throw our exception up to whowever might want it
				String *message = String::Concat(	S"A serious exception occurred when trying to add the new asset revision in the MOG Repository.\n",
													S"ASSET: ", assetFilename->GetAssetLabel(), S"\n",
													S"The Asset was not blessed correctly.");
				MOG_Report::ReportMessage(S"Bless", ex->Message, ex->StackTrace, MOG_ALERT_LEVEL::CRITICAL);
			}
			__finally
			{
				// Always make sure to close the asset when we are finished
				asset->Close();
			}
		}
		else
		{
			// Inform the Server about this
			String *message = String::Concat(	S"The System was unable to open the specified asset.\n",
												S"ASSET: ", assetFilename->GetAssetLabel(), S"\n",
												S"The Asset was not blessed correctly.");
			MOG_Prompt::PromptMessage(S"Bless", message, "", MOG_ALERT_LEVEL::ERROR);
		}
	}

	return bBlessed;
}


// Send asset to a specific user
String *MOG_ControllerInbox::GetAssetBlessTarget(MOG_Filename *assetFilename, MOG_Properties *properties)
{
	String *blessTarget = S"";
	String *userBlessTarget = MOG_ControllerProject::GetActiveUser()->GetBlessTarget();
	String *classificationBlessTarget = S"";

	// Check if there was a propeerties specified?
	if (properties)
	{
		// Obtain the classification-level bless target
		classificationBlessTarget = properties->BlessTarget;
	}

	// Check if we have a valid assetFilename?
	if (assetFilename)
	{
		// Make sure this asset is located in the inbox?
		if (assetFilename->IsWithinInboxes())
		{
			// Check if the userBlessTarget is valid? and
			// Make sure we are not their bless target? and
			// Make sure the asset isn't already sitting in the bless target's directory?
			if (MOG_ControllerProject::IsValidUser(userBlessTarget) &&
				String::Compare(userBlessTarget, MOG_ControllerProject::GetUserName(), true) != 0 &&
				String::Compare(userBlessTarget, assetFilename->GetUserName(), true) != 0)
			{
				blessTarget = userBlessTarget;
			}
			// Check if the userBlessTarget is valid? and
			// Make sure we are not their bless target? and
			// Make sure the asset isn't already sitting in the bless target's directory?
			else if (MOG_ControllerProject::IsValidUser(classificationBlessTarget) &&
					 String::Compare(classificationBlessTarget, MOG_ControllerProject::GetUserName(), true) != 0 &&
					 String::Compare(classificationBlessTarget, assetFilename->GetUserName(), true) != 0)
			{
				blessTarget = classificationBlessTarget;
			}
		}
	}
	else
	{
		// Worst case, use the user's bless target
		blessTarget = userBlessTarget;
	}

	return blessTarget;
}


// Send asset to a specific user
bool MOG_ControllerInbox::MoveAssetToUserInbox(MOG_Filename *assetFilename, String *user, String *comment, MOG_AssetStatusType status, BackgroundWorker* worker)
{
	bool bMoved = false;

	// Make sure we have a valid user specified?
	if (user->Length)
	{
		// Open this old asset
		MOG_ControllerAsset *asset = MOG_ControllerAsset::OpenAsset(assetFilename);
		if (asset)
		{
			bMoved = MoveAssetToUserInbox(asset, user, comment, status, worker);

			// Always make sure to close the asset when we are finished
			asset->Close();
		}
	}

	return bMoved;
}


// Send asset to a specific user
bool MOG_ControllerInbox::MoveAssetToUserInbox(MOG_ControllerAsset *asset, String *user, String *comment, MOG_AssetStatusType status, BackgroundWorker* worker)
{
	bool bMoved = false;
	bool bFailed = false;

	if (asset)
	{
		// Construct the destinationFilename to the specified user's Inbox
		MOG_Filename *destinationFilename = new MOG_Filename(asset->GetAssetFilename()->GetAssetEncodedInboxPath(user, S"Inbox"));
		if (String::Compare(asset->GetAssetFilename()->GetEncodedFilename(), destinationFilename->GetEncodedFilename(), true) == 0)
		{
			//This is the same file, no need to copy it.  In fact it will give us problems if we try to copy it 
			//because we remove colliding assets first, and if we do that we won't have the source file anymore.
			bMoved = true;
		}
		else
		{
			// Resolve any potentially colliding assets
			if (ResolveCollidingAssets(asset, destinationFilename, worker))
			{
				// Make sure we haven't failed?
				if (!bFailed)
				{
					// Move Asset onto the destinationFilename and leave a link behind
					if (Move(asset, destinationFilename, status, true))
					{
						// Format and post the comment to this asset
   						asset->PostComment(comment, false);
						bMoved = true;
					}
					else
					{
// JohnRen - This is already implied because Move() will have already reported a file sharing violation
//						String *message = String::Concat( S"MOG was unable to move the asset due to a potential file sharing violation.\n",
//															S"ASSET: ", assetFilename->GetAssetLabel(), S"\n",
//															S"\n",
//															S"Close any open files that are associated with this asset.");
//						MOG_Prompt::PromptMessage(S"MoveAssetToUserInbox", message, "", MOG_ALERT_LEVEL::ERROR);
						bFailed = true;
					}
				}
			}
		}
	}

	return bMoved;
}


// Send a copy of an asset to a specific user
bool MOG_ControllerInbox::CopyAssetToUserInbox(MOG_Filename *assetFilename, String *user, String *comment, MOG_AssetStatusType status, BackgroundWorker* worker)
{
	bool bMoved = false;

	// Make sure we have a valid user specified?
	if (user->Length)
	{
		// Open this old asset
		MOG_ControllerAsset *asset = MOG_ControllerAsset::OpenAsset(assetFilename);
		if (asset)
		{
			// Call our other counterpart
			bMoved = CopyAssetToUserInbox(asset, user, comment, status, worker);

			// Always make sure to close the asset when we are finished
			asset->Close();
		}
	}

	return bMoved;
}


// Send a copy of an asset to a specific user
bool MOG_ControllerInbox::CopyAssetToUserInbox(MOG_ControllerAsset *asset, String *user, String *comment, MOG_AssetStatusType status, BackgroundWorker* worker)
{
	bool bMoved = false;

	// Make sure we have a valid user specified?
	if (user->Length)
	{
		String *box = S"Inbox";
		// Check if this user is sending it to themself?
		if (String::Compare(user, MOG_ControllerProject::GetUserName_DefaultAdmin(), true) == 0)
		{
			// Send the asset to the user's own Drafts because they are sending this asset to themself
			box = S"Drafts";
		}

		// Construct the destinationFilename to the specified user's Inbox
		MOG_Filename *destinationFilename = new MOG_Filename(asset->GetAssetFilename()->GetAssetEncodedInboxPath(user, box));
		if (String::Compare(asset->GetAssetFilename()->GetEncodedFilename(), destinationFilename->GetEncodedFilename(), true) == 0)
		{
			//This is the same file, no need to copy it.  In fact it will give us problems if we try to copy it 
			//because we remove colliding assets first, and if we do that we won't have the source file anymore.
			bMoved = true;
		}
		else
		{
			// Resolve any potentially colliding assets
			if (ResolveCollidingAssets(asset, destinationFilename, worker))
			{
				// Format and post the comment to this asset
       			asset->PostComment(comment, false);
				// Move Asset onto the new destinationFilename and leave a link behind
				if (Copy(asset, destinationFilename, status, worker))
				{
					bMoved = true;
				}
				else
				{
// JohnRen - This is already implied because Copy() will have already reported a file sharing violation
//					String *message = String::Concat( S"MOG was unable to copy the asset due to a potential file sharing violation.\n",
//														S"ASSET: ", asset->GetAssetFilename()->GetAssetLabel(), S"\n",
//														S"\n",
//														S"Close any open files that are associated with this asset.");
//					MOG_Prompt::PromptMessage(S"CopyAssetToUserInbox", message, "", MOG_ALERT_LEVEL::ERROR);
				}
			}
		}
	}

	return bMoved;
}


// Rename the asset and set its status to Unprocessed
bool MOG_ControllerInbox::Rename(MOG_Filename *assetFilename, String *newAssetName, bool bRenameFiles)
{
	bool bFailed = false;

	// Retain the old name for later with a new MOG_Filename NOT pointing to assetFilename
	MOG_Filename *oldAssetFilename = new MOG_Filename(assetFilename->GetEncodedFilename());

	// Make sure newAssetName is a valid asset name?
	MOG_Filename *newAssetFilename = new MOG_Filename(newAssetName);
	if (newAssetFilename->GetFilenameType() == MOG_FILENAME_TYPE::MOG_FILENAME_Asset)
	{
		// Check for invalid characters in the new name
		if (MOG_ControllerSystem::InvalidMOGCharactersCheck(newAssetFilename->GetAssetFullName(), true))
		{
			return false;
		}

		// Open this asset
		MOG_ControllerAsset *asset = MOG_ControllerAsset::OpenAsset(assetFilename);
		if (asset)
		{
			// Create the appropriate destination for this asset
			String *destination = "";
			MOG_AssetStatusType state = MOG_AssetStatusType::None;
			// Assume the user specified in assetFilename should be used in the new name as well
			destination = newAssetFilename->GetAssetEncodedInboxPath(assetFilename->GetUserName(), S"Drafts");
			if (asset->GetProperties()->NativeDataType)
			{
				state = MOG_AssetStatusType::Processed;
			}
			else
			{
				state = MOG_AssetStatusType::Unprocessed;
			}

			// Attempt to move the asset to the new name
			if (Move(asset, new MOG_Filename(destination), state))
			{
				// Update the asset's created time
				asset->GetProperties()->CreatedTime = MOG_Time::GetVersionTimestamp();

				// Check if we want to rename files?
				if (bRenameFiles)
				{
					// Get the Asset's Files
					ArrayList *importFiles = DosUtils::FileGetRecursiveList(MOG_ControllerAsset::GetAssetImportedDirectory(asset->GetProperties()), S"*.*");
					if (importFiles != NULL)
					{
						// Check each filename
						for (int f = 0; f < importFiles->Count; f++)
						{
							String *importFile = dynamic_cast<String *>(importFiles->Item[f]);

							if (!Rename_FileRename(oldAssetFilename, asset->GetAssetFilename(), importFile))
							{
								bFailed = true;
							}
						}
					}
				}
			}

			// Record Asset files
			MOG_ControllerAsset::RecordAllAssetFiles(asset);

			// Always make sure to close the asset when we are finished
			asset->Close();

			if (!bFailed)
			{
				// Rename Succeeded
				return true;
			}
		}
	}
	else
	{
		String *message = String::Concat(	S"The new name was not recognized as a valid Asset name.\n",
											S"ASSET: ", newAssetFilename->GetAssetLabel(), S"\n",
											S"Asset was not renamed.");
		MOG_Prompt::PromptMessage(S"Rename Asset Failed", message, "", MOG_ALERT_LEVEL::ERROR);
	}

	// Rename failed
	return false;
}

// Rename() helper:  Returns true if successful
bool MOG_ControllerInbox::Rename_FileRename(MOG_Filename *oldFilename, MOG_Filename *newFilename, String *file)
{
	bool bFailed = false;

	// Check if this file exactly matches the oldFilename?
	String *fileNoExtension = DosUtils::PathGetFileNameWithoutExtension(file);
	String *newName = S"";

	// Check if this filename matches the asset's full name?
	if (String::Compare(fileNoExtension, Path::GetFileNameWithoutExtension(oldFilename->GetAssetFullName()), true) == 0)
	{
		newName = newFilename->GetAssetFullName();
	}
	// Check if this filename matches the asset's name?
	else if (String::Compare(fileNoExtension, Path::GetFileNameWithoutExtension(oldFilename->GetAssetName()), true) == 0)
	{
		newName = newFilename->GetAssetName();
	}
	// Check if this filename matches the asset jobLabel?
	else if (String::Compare(fileNoExtension, oldFilename->GetAssetLabelNoExtension(), true) == 0)
	{
		newName = newFilename->GetAssetLabelNoExtension();
	}

	// Check if we found a newName?
	if (newName->Length)
	{		
		// Ensure the newName matches the newFilename's extension
		newName = String::Concat(newName, S".", DosUtils::PathGetExtension(file));

		// Rename the file?
		if (!DosUtils::FileRenameFast(file, String::Concat(DosUtils::PathGetDirectoryPath(file), S"\\", newName), true))
		{
			bFailed = true;
		}	
	}

	// Check if we failed?
	if (!bFailed)
	{
		return true;
	}
	return false;
}


bool MOG_ControllerInbox::Reject(MOG_Filename *assetFilename, String *comment, BackgroundWorker* worker)
{
	return Reject(assetFilename, comment, MOG_AssetStatusType::Rejected, worker);
}

bool MOG_ControllerInbox::Reject(MOG_Filename *assetFilename, String *comment, MOG_AssetStatusType state, BackgroundWorker* worker)
{
	bool bRejected = false;

	// Make sure we have a valid comment?
	if (comment->Length)
	{
		// Open this asset
		MOG_ControllerAsset *asset = MOG_ControllerAsset::OpenAsset(assetFilename);
		if (asset)
		{
			// Reject the asset
			if (Reject(asset, comment, state, worker))
			{
				bRejected = true;
			}

			// Always make sure we close the asset when we are finished
			asset->Close();
		}
	}

	return bRejected;
}


bool MOG_ControllerInbox::Reject(MOG_ControllerAsset *asset, String *comment, BackgroundWorker* worker)
{
	return Reject(asset, comment, MOG_AssetStatusType::Rejected, worker);
}

bool MOG_ControllerInbox::Reject(MOG_ControllerAsset *asset, String *comment, MOG_AssetStatusType state, BackgroundWorker* worker)
{
	bool bRejected = false;

	// Make sure we have a valid asset?
	if (asset)
	{
		// Make sure we have a valid comment?
		if (comment->Length)
		{
			// Get the creator of this asset?
			String *creator = asset->GetProperties()->Creator;
			if (creator->Length &&
				MOG_ControllerProject::IsValidUser(creator))
			{
				// Establish the destination back to the creator's inbox
				MOG_Filename *destinationFilename = new MOG_Filename(asset->GetAssetFilename()->GetAssetEncodedInboxPath(creator, S"Drafts"));

				// Post the comment to this asset
   				asset->PostComment(comment, false);

				// Check if this is an inbox asset?
				if (asset->GetAssetFilename()->IsWithinInboxes())
				{
					// Send this asset back to the creator
					if (Move(asset, destinationFilename, state))
					{
						bRejected = true;
					}
				}
				// Check if this is a repository asset?
				else if (asset->GetAssetFilename()->IsWithinRepository())
				{
					// Attempt to remove this revision from the repository (it will fail if this version is in use by anything)
					if (MOG_DBAssetAPI::RemoveAssetVersion(asset->GetAssetFilename(), asset->GetAssetFilename()->GetVersionTimeStamp()))
					{
						// Since we were able to remove this revision, send this asset back to it's creator
						if (Move(asset, destinationFilename, state))
						{
							bRejected = true;
						}
					}
					else
					{
						// We can't move revisions that are in use so copy this asset back to it's creator's inbox instead
						if (Copy(asset, destinationFilename, state, worker))
						{
							bRejected = true;

							// KLUDGE - This may not be the best place for this, but it is the most logical spot.
							// Newly blessed assets that might fail the network package merge get rejected back to the owner.
							// However, sometimes the asset's version can get posted early which cause us to have to copy it instead of moving it.
							// This at least will clear the stale links of an asset in this situation.
							// This really won't hurt blessed assets because this was already performed during its post.
							// Remove Inbox Links for this asset
							MOG_ControllerInbox::RemoveAllAssetLinkFiles(asset);
						}
					}
				}
			}
			else
			{
				String *message = String::Concat(	S"Asset was not Rejected because its creator is an invalid user.\n",
													S"\tASSET: ", asset->GetAssetFilename()->GetAssetLabel(), S"\n",
													S"\tCREATOR: ", creator);
				MOG_Prompt::PromptMessage(S"Reject", message, "", MOG_ALERT_LEVEL::ERROR);
			}
		}
	}

	return bRejected;
}


// Delete the asset with full path
bool MOG_ControllerInbox::Delete(MOG_Filename *assetFilename)
{
	bool bDeleted = false;

	if (assetFilename && !assetFilename->IsWithinRepository())
	{
		// Check if this is a link?
		if (assetFilename->IsLink())
		{
			// Delete the Link file
			if (RemoveAssetLinkFile(assetFilename))
			{
				bDeleted = true;
			}

			return bDeleted;
		}

		// Make sure this is a valid asset or group?
		if (assetFilename->GetFilenameType() == MOG_FILENAME_TYPE::MOG_FILENAME_Asset ||
			assetFilename->GetFilenameType() == MOG_FILENAME_TYPE::MOG_FILENAME_Group)
		{
			// Open the asset
			MOG_ControllerAsset *asset = MOG_ControllerAsset::OpenAsset(assetFilename);
			if (asset)
			{
				// Call the normal API
				if (Delete(asset))
				{
					bDeleted = true;
				}

				// Make sure to always close the asset controller
				asset->Close();
			}
			// It must be a bogus asset
			else
			{
				// Always remove it regardless because it makes for a better user experience
				UpdateInboxView(MOG_AssetStatusType::Deleted, assetFilename, NULL);
				bDeleted = true;
			}
		}
	}

	return bDeleted;
}


// Delete the asset with full path
bool MOG_ControllerInbox::Delete(MOG_ControllerAsset *asset)
{
	bool bDeleted = false;

	if (asset && !asset->GetAssetFilename()->IsWithinRepository())
	{
		bool bDoDelete = true;
		bool bMoveToTrash = false;
		bool bIsTrash = asset->GetAssetFilename()->IsTrash();

		// Make sure this asset isn't already in the trash?
		if (!bIsTrash)
		{
			// Make sure this asset is within the inboxes?
			if (asset->GetAssetFilename()->IsWithinInboxes())
			{
				bDoDelete = false;
				bMoveToTrash = true;
			}
		}

		// Check if we should move it to the trash?
		if (bMoveToTrash)
		{
			// Move this asset to the User's trash
			if (MoveToTrash(asset))
			{
				bDeleted = true;
			}
		}
		// Check if we should delete it?
		else if (bDoDelete)
		{
			String *deleteDirectory = asset->GetAssetFilename()->GetEncodedFilename();
			String *assetName = asset->GetAssetFilename()->GetAssetName();

			// Delete the specified links to this asset throughout the entire project
			RemoveAllAssetLinkFiles(asset);

			// Close the asset prior to us deleting it
			asset->Close();

			// Attempt to delete the asset because this is already in 'Trash'
// Removed because this was really really slow!  Besides, why would we want to recycle an entire asset container?
//			if (DosUtils::Recycle(deleteDirectory))
			if (DosUtils::DirectoryDeleteFast(deleteDirectory))
			{
				// Check if this asset is being deleted from the trash?
				if (bIsTrash)
				{
					// Get rid of any empty folders that might be left behind as a result of this deletion from the trash
					DosUtils::DirectoryDeleteEmptyParentsFast(DosUtils::PathGetDirectoryPath(deleteDirectory), false);
				}

				// Update the link from the inbox views
				UpdateInboxView(MOG_AssetStatusType::Deleted, new MOG_Filename(deleteDirectory), NULL);
				bDeleted = true;
			}
			else
			{
				String *message = String::Concat( S"MOG was unable to delete the asset due to a potential file sharing violation.\n",
													S"ASSET: ", assetName, S"\n",
													S"\n",
													S"Close any open files that are associated with this asset.");
				MOG_Prompt::PromptMessage(S"Delete", message, "", MOG_ALERT_LEVEL::ERROR);
			}
		}
	}

	return bDeleted;
}


// Remove Package Assignment
bool MOG_ControllerInbox::RemovePackageAssignment(MOG_Filename *assetFilename, String *packageName, String *packageGroups, String *packageObjects)
{
	// Combine the package assignment
	MOG_Property *relationshipProperty = MOG_PropertyFactory::MOG_Relationships::New_PackageAssignment(packageName, packageGroups, packageObjects);
	// Use our other API
	return RemoveRelationshipProperty(assetFilename, relationshipProperty);
}


bool MOG_ControllerInbox::RemoveRelationshipProperty(MOG_Filename *assetFilename, MOG_Property *relationshipProperty)
{
	bool bRemoved = false;
	bool bDeleted = false;

	// Open this asset
	MOG_ControllerAsset *asset = MOG_ControllerAsset::OpenAsset(assetFilename);
	if (asset)
	{
		try
		{
			// Call our other API
			bRemoved = RemoveRelationshipProperty(asset, relationshipProperty);

			// Check if this is a packaged asset?
			if (asset->GetProperties()->IsPackagedAsset)
			{
				// Check if we have no more package assignments?
				if (asset->GetProperties()->GetPackages()->Count == 0)
				{
					// Automatically delete the asset from the user's inbox
					if (Delete(asset))
					{
// JohnRen - Moved this logic to the Client in the callback events so it can effect all of the active local workspaces
//						// Check if this asset exists in their local updated tray?
//						MOG_Filename* localAssetFilename = MOG_Filename::GetLocalUpdatedTrayFilename(MOG_ControllerProject::GetWorkspaceDirectory(), assetFilename);
//						if (DosUtils::DirectoryExistFast(localAssetFilename->GetEncodedFilename()))
//						{
//							// Automattically delete it from the user's local updated tray?
//							Delete(localAssetFilename);
//						}
					}
					else
					{
						// Inform the user we failed to remove the asset from their inbox
					}
				}
			}
		}
		__finally
		{
			// Always make sure to close the asset when we are finished
			asset->Close();
		}
	}

	return bRemoved;
}


bool MOG_ControllerInbox::RemoveRelationshipProperty(MOG_ControllerAsset *asset, MOG_Property *relationshipProperty)
{
	bool bRemoved = false;

	// Make sure we have a valid asset controller? and
	// Make sure we have s valid relationshipProperty?
	if (asset &&
		relationshipProperty)
	{
		// Remove the relationship property
		bRemoved = asset->GetProperties()->RemoveRelationship(relationshipProperty);
	}

	return bRemoved;
}


ArrayList* MOG_ControllerInbox::GetAssetList(String* box)
{
	ArrayList *localAssets = NULL;
	String *filter = "";
	String *boxName = box;

	// Check if this is the 'Local' box?
	if (String::Compare(box, S"Local", true) == 0)
	{
		// Append '.ComputerName' onto the boxName
		boxName = String::Concat(box, S".", MOG_ControllerSystem::GetComputerName());
		// Set the filter for the local workspace
		filter = String::Concat(MOG_ControllerProject::GetWorkspaceDirectory(), S"\\*");
	}
	else
	{
		// Set the filter for the specified box
		filter = String::Concat(S"\\", boxName, S"\\*");
	}

	localAssets = MOG_DBInboxAPI::InboxGetAssetList(boxName, MOG_ControllerProject::GetUserName(), S"", filter);

	return localAssets;
}

String *MOG_ControllerInbox::GetAssetTrashPath(MOG_Filename *assetFilename, String *timestamp)
{
	String *path = S"";

	// Make sure we have a valid assetFilename?
	if (assetFilename)
	{
		// Make sure we are logged into a project?
		MOG_Project *pProjectInfo = MOG_ControllerProject::GetProject();
		if (pProjectInfo)
		{
			String *seeds = S"";
			String *format = S"";

			// Check for an invalid timestamp?
			if (!timestamp || !timestamp->Length)
			{
				// Construct a new timestamp
				timestamp = MOG_Time::GetVersionTimestamp();
			}

			// Obtain the user's inbox path from the asset
			String *userPath = assetFilename->GetUserPath();
			if (userPath->Length == 0)
			{
				// Worst case, lets use our loggedin user's inbox path
				userPath = MOG_ControllerProject::GetUserPath();
			}

			// Build the format string for the 'Trash' box
			path = String::Concat(userPath, S"\\Trash\\", assetFilename->GetAssetOriginalFullName(), S"\\R.", timestamp);
		}
	}

	return path;
}


// Delete the asset with full path
bool MOG_ControllerInbox::MoveToTrash(MOG_Filename *assetFilename)
{
	bool bMoved = false;

//?	MOG_Controller::MoveToTrash - What about locally updated assets?  Should we try to support a trash there as well?
	// Make sure the asset is within a User's box?
	if (assetFilename->GetUserPath()->Length)
	{
		// Open this old asset
		MOG_ControllerAsset *asset = MOG_ControllerAsset::OpenAsset(assetFilename);
		if (asset)
		{
			// Call the normal API
			bMoved = MoveToTrash(asset);

			// Always close the asset to make sure any locks are released
			asset->Close();
		}
	}

	return bMoved;
}


// Delete the asset with full path
bool MOG_ControllerInbox::MoveToTrash(MOG_ControllerAsset *asset)
{
//?	MOG_Controller::MoveToTrash - What about locally updated assets?  Should we try to support a trash there as well?
	// Make sure the asset is within a User's box?
	if (asset->GetAssetFilename()->GetUserPath()->Length)
	{
		MOG_Filename *trashTargetFilename = new MOG_Filename(MOG_ControllerInbox::GetAssetTrashPath(asset->GetAssetFilename(), asset->GetProperties()->CreatedTime));

		// Move the asset into the 'Trash'
		if (Move(asset, trashTargetFilename, MOG_AssetStatusType::Deleted))
		{
			// Indicate the asset was moved to the 'Trash'
			return true;
		}
		else
		{
			String *message = String::Concat( S"MOG was unable to move the asset to the trash due to a potential file sharing violation.\n",
												S"ASSET: ", asset->GetAssetFilename()->GetAssetLabel(), S"\n",
												S"\n",
												S"Close any open files that are associated with this asset.");
			MOG_Prompt::PromptMessage(S"MoveToTrash", message, "", MOG_ALERT_LEVEL::ERROR);
		}
	}

	return false;
}



// Delete the asset with full path
bool MOG_ControllerInbox::RestoreAsset(MOG_Filename *assetFilename)
{
	bool bRestored = false;

	// Make sure the asset is in the trash bin?
	if (assetFilename->IsTrash())
	{
		// Open this old asset
		MOG_ControllerAsset *asset = MOG_ControllerAsset::OpenAsset(assetFilename);
		if (asset)
		{
			// Call the normal API
			bRestored = RestoreAsset(asset);

			// Always close the asset to make sure any locks are released
			asset->Close();
		}
	}

	return bRestored;
}


// Delete the asset with full path
bool MOG_ControllerInbox::RestoreAsset(MOG_ControllerAsset *asset)
{
	// Make sure the asset is within a User's box?
	if (asset->GetAssetFilename()->IsTrash())
	{
		// Create the appropriate destination for this asset
		String *destinationPath = "";
		MOG_AssetStatusType state = MOG_AssetStatusType::None;
		// Assume the user specified in the asset->GetAssetFilename() should be used in the restored location as well
		destinationPath = asset->GetAssetFilename()->GetAssetEncodedInboxPath(asset->GetAssetFilename()->GetUserName(), S"Drafts");
		if (asset->GetProperties()->NativeDataType)
		{
			state = MOG_AssetStatusType::Processed;
		}
		else
		{
			state = MOG_AssetStatusType::Unprocessed;
		}

		// Move the asset back to the user's UnProcessed
		MOG_Filename *restoredFilename = new MOG_Filename(destinationPath);
		if (Move(asset, restoredFilename, state))
		{
			// Post comment to this asset
			String *comment = S"This Asset was restored from the user's trash.";
       		if (asset->PostComment(comment, false))
			{
			}

			// Indicate the asset was restored
			return true;
		}
	}

	return false;
}



bool MOG_ControllerInbox::CreateAssetLinkFile(MOG_ControllerAsset *asset)
{
	MOG_Filename *assetFilename = asset->GetAssetFilename();
	bool bCreated = false;

	// Make sure this asset is within a user's inbox?
	if (assetFilename->GetFilenameType() == MOG_FILENAME_TYPE::MOG_FILENAME_Asset &&
		assetFilename->IsWithinInboxes())
	{
		// Add the system recognizable '(Link)' tag before the asset's full name and drop it in our 'OutBox'
		MOG_Filename *linkFilename = new MOG_Filename(asset->GetAssetFilename()->GetUserName(), S"OutBox", asset->GetAssetFilename()->GetGroupTree(), String::Concat(S"(Link)", assetFilename->GetAssetOriginalFullName()));

		// Create the new Link file using the asset's properties file
		MOG_PropertiesIni *newLinkFile = new MOG_PropertiesIni();
		newLinkFile->PutString("InboxLink", "Target", linkFilename->GetEncodedFilename());
		newLinkFile->PutFile(asset->GetProperties()->GetNonInheritedPropertiesIni());
		// Save the new Link file into the current directory
		if (newLinkFile->Save(linkFilename->GetEncodedFilename()))
		{
			// Add this link file to the Asset's LINK section
			asset->GetProperties()->GetNonInheritedPropertiesIni()->PutSectionString("InboxLinks", linkFilename->GetEncodedFilename());

			// Update all the link's properties
			UpdateLinkProperties(linkFilename);

			// Add the new link to our OutBox view
			UpdateInboxView(asset->GetProperties()->Status, NULL, linkFilename);

			bCreated = true;
		}

		// Make sure to always close the file when we are finished
		newLinkFile->Close();
	}

	return bCreated;
}


bool MOG_ControllerInbox::UpdateAssetLinkFiles(MOG_ControllerAsset *asset)
{
    // Check if any link files exist?
	if (asset->GetProperties()->GetNonInheritedPropertiesIni()->SectionExist("InboxLinks"))
	{
		// Cycle through all the recorded link files...Don't auto increment linkIndex in case we remove a dead link
		for (int linkIndex = 0; linkIndex < asset->GetProperties()->GetNonInheritedPropertiesIni()->CountKeys("InboxLinks"); /*linkIndex++*/ )
		{
			// Get the location of this link file
			MOG_Filename *linkFilename = new MOG_Filename(asset->GetProperties()->GetNonInheritedPropertiesIni()->GetKeyNameByIndexSLOW("InboxLinks", linkIndex));
			// Make sure that this Link file exists
			if (DosUtils::FileExistFast(linkFilename->GetEncodedFilename()))
			{
				// Update this link file with the asset's information
				MOG_Ini *linkFile = new MOG_Ini();
				linkFile->PutString("InboxLink", "Target", asset->GetAssetFilename()->GetEncodedFilename());
				linkFile->PutFile(asset->GetProperties()->GetNonInheritedPropertiesIni());
				linkFile->Save(linkFilename->GetEncodedFilename());
				linkFile->Close();

				// Update all the link's properties
				UpdateLinkProperties(linkFilename);

				// Update the link in the inbox views
				UpdateInboxView(asset->GetProperties()->Status, linkFilename, linkFilename);

				// Only increment link on valid link files
				linkIndex++;
			}
			else
			{
				// Remove this dead link
				// No need to increment linkIndex here because the file will collapse around us when we remove the dead link
				asset->GetProperties()->GetNonInheritedPropertiesIni()->RemoveString(S"InboxLinks", linkFilename->GetEncodedFilename());
			}
		}
	}

	return true;
}


bool MOG_ControllerInbox::RemoveAssetLinkFile(MOG_Filename *linkFilename)
{
	bool bFailed = false;

	// Check if this is a link?
	if (linkFilename->IsLink())
	{
		// Check if the link file exists?
		if (DosUtils::FileExistFast(linkFilename->GetEncodedFilename()))
		{
			// Delete the link file?
			if (!DosUtils::FileDelete(linkFilename->GetEncodedFilename()))
			{
				bFailed = true;
			}
		}

		if (!bFailed)
		{
			// Remove the link from the inbox views
			UpdateInboxView(MOG_AssetStatusType::None, linkFilename, NULL);
			return true;
		}
	}

	return false;
}


bool MOG_ControllerInbox::RemoveAllAssetLinkFiles(MOG_Filename *assetFilename)
{
	bool bRemoved = false;

	// Open this old asset
	MOG_ControllerAsset *asset = MOG_ControllerAsset::OpenAsset(assetFilename);
	if (asset)
	{
		// Call the normal API
		bRemoved = RemoveAllAssetLinkFiles(asset);

		// Always close the asset to make sure any locks are released
		asset->Close();
	}

	return bRemoved;
}


bool MOG_ControllerInbox::RemoveAllAssetLinkFiles(MOG_ControllerAsset *asset)
{
	bool bFailed = false;

    // Check if any link files exist?
	if (asset->GetProperties()->GetNonInheritedPropertiesIni()->SectionExist("InboxLinks"))
	{
		// Cycle through all the recorded link files of this asset...Don't automatically increment the linkIndex
		for (int linkIndex = 0; linkIndex < asset->GetProperties()->GetNonInheritedPropertiesIni()->CountKeys("InboxLinks"); /*linkIndex++*/)
		{
			// Get the location of this link file
			String *link = asset->GetProperties()->GetNonInheritedPropertiesIni()->GetKeyNameByIndexSLOW("InboxLinks", linkIndex);
			MOG_Filename *linkFilename = new MOG_Filename(link);

			// Attempt to delete this link file
			if (RemoveAssetLinkFile(linkFilename))
			{
				// Remove this string from the 'InboxLinks' section
				asset->GetProperties()->GetNonInheritedPropertiesIni()->RemoveString(S"InboxLinks", link);
				// We don't need to increment linkIndex because this link was just removed and the section will collapse around us
				continue;
			}

			// Looks like we failed to remove a link
			bFailed = true;
			// Manually increment the linkIndex to skip this one
			linkIndex++;
		}
	}

	// Check if we failed?
	if (!bFailed)
	{
		return true;
	}
	return false;
}


// Move the asset to the specified targetBox
bool MOG_ControllerInbox::MoveToBox(MOG_Filename *assetFilename, String *targetBox, MOG_AssetStatusType status)
{
	bool bMoved = false;

	// Make sure this isn't a linked asset?
	if (!assetFilename->IsLink())
	{
		// Make sure this is a legitmate asset?
		if (assetFilename->GetFilenameType() == MOG_FILENAME_TYPE::MOG_FILENAME_Asset)
		{
			MOG_ControllerAsset *asset = MOG_ControllerAsset::OpenAsset(assetFilename);
			if (asset)
			{
				bMoved = MoveToBox(asset, targetBox, status);

				// Always make sure to close the asset controller when we are finished
				asset->Close();
			}
		}
	}

	return bMoved;
}


// Move the asset to the specified targetBox
bool MOG_ControllerInbox::MoveToBox(MOG_ControllerAsset *asset, String *targetBox, MOG_AssetStatusType status)
{
	// Make sure a valid box was specified?
	if (targetBox->Length)
	{
		// Create the destination to the specified box
		MOG_Filename *destination = new MOG_Filename(asset->GetAssetFilename()->GetAssetEncodedInboxPath(asset->GetAssetFilename()->GetUserName(), targetBox));

		// Check if this is an inbox asset?
		if (asset->GetAssetFilename()->IsInbox())
		{
			// We can only move inbox assets
			return Move(asset, destination, status);
		}
	}

	return false;
}


// Move the asset to the specified target
bool MOG_ControllerInbox::Move(MOG_ControllerAsset *asset, MOG_Filename *targetAssetFilename, MOG_AssetStatusType status)
{
	return Move(asset, targetAssetFilename, status, false);
}


// Move the asset to the specified target
bool MOG_ControllerInbox::Move(MOG_ControllerAsset *asset, MOG_Filename *targetAssetFilename, MOG_AssetStatusType status, bool leaveLink)
{
	bool bMoved = false;

	// We must retain a copy of our sourceFilename when we update the inbox views
	MOG_Filename *sourceFilename = new MOG_Filename(asset->GetAssetFilename());

	// Check if the source and destination are the same?
	if (String::Compare(sourceFilename->GetEncodedFilename(), targetAssetFilename->GetEncodedFilename(), true) == 0)
	{
		// Return true because we really can't do anything about this
		return true;
	}

	// Make sure this isn't a Link?
	if (!asset->GetAssetFilename()->IsLink())
	{
		// Check if this is an asset?
		if (asset->GetAssetFilename()->GetFilenameType() == MOG_FILENAME_TYPE::MOG_FILENAME_Asset)
		{
			// Check if there is an asset in our way?
			if (DosUtils::DirectoryExistFast(targetAssetFilename->GetEncodedFilename()))
			{
				// Check if this is within the inboxes?
				if (targetAssetFilename->IsWithinInboxes())
				{
					// Move the old asset out of our way and into the trash
					if (!MoveToTrash(targetAssetFilename))
					{
// JohnRen - This is already implied because Move() will have already reported a file sharing violation
//						String *message = String::Concat( S"MOG was unable to move the already existing asset to the trash due to a potential file sharing violation.\n",
//															S"ASSET: ", asset->GetAssetFilename()->GetAssetLabel(), S"\n",
//															S"\n",
//															S"Close any open files that are associated with this asset.");
//						MOG_Prompt::PromptMessage(S"Move", message, "", MOG_ALERT_LEVEL::ERROR);
						return false;
					}
				}
				else if (targetAssetFilename->IsLocal())
				{
					// Delete it because we only do trash in the inboxes
					if (!Delete(targetAssetFilename))
					{
// JohnRen - This is already implied because Delete() will have already reported a file sharing violation
//						String *message = String::Concat( S"MOG was unable to delete the already existing asset due to a potential file sharing violation.\n",
//															S"ASSET: ", asset->GetAssetFilename()->GetAssetLabel(), S"\n",
//															S"\n",
//															S"Close any open files that are associated with this asset.");
//						MOG_Prompt::PromptMessage(S"Move", message, "", MOG_ALERT_LEVEL::ERROR);
						return false;
					}
				}
			}
		}

		// Leave a link behind?
		if (leaveLink)
		{
			// Make sure we are within the inboxes?
			if (asset->GetAssetFilename()->IsWithinInboxes())
			{
				// Create Link at the current location
				CreateAssetLinkFile(asset);
			}
		}

		// Move the Asset directory
		if (DosUtils::DirectoryMoveFast(asset->GetAssetFilename()->GetEncodedFilename(), targetAssetFilename->GetEncodedFilename(), true))
		{
			// Check if the user didn't specify to maintain their lock?
			if (asset->GetProperties()->MaintainLock == false)
			{
				// Check if both owners are valid and that the owner is changing?
				if (asset->GetAssetFilename()->GetUserName()->Length &&
					targetAssetFilename->GetUserName() &&
					String::Compare(asset->GetAssetFilename()->GetUserName(), targetAssetFilename->GetUserName(), true) != 0)
				{
					// Check if there is an existing lock?
					MOG_Command *lockInfo = MOG_ControllerProject::PersistentLock_Query(asset->GetAssetFilename()->GetAssetFullName());
					if (lockInfo)
					{
						// Make sure we successfully returned a lock?
						if (lockInfo->IsCompleted() &&
							lockInfo->GetCommand())
						{
							// Only transfer the lock along if the current owner is the active lock owner
							if (String::Compare(asset->GetAssetFilename()->GetUserName(), lockInfo->GetCommand()->GetUserName(), true) == 0)
							{
								// Release the old lock on the asset
								MOG_ControllerProject::PersistentLock_Release(asset->GetAssetFilename()->GetAssetFullName());
								// Build a nice lock history using the old lock's description
								String *lockComment = lockInfo->GetCommand()->GetDescription();
								if (lockComment->Length)
								{
									// Add a ';' seperator
									lockComment = String::Concat(lockComment, S"; ");
								}
								lockComment = String::Concat(lockComment, S"Sent from ", asset->GetAssetFilename()->GetUserName());
								// Obtain a new lock on the asset as the new owner
								MOG_ControllerProject::PersistentLock_Request(asset->GetAssetFilename()->GetAssetFullName(), lockComment, targetAssetFilename->GetUserName());
							}
						}
					}
				}
			}

			// Update asset's filename
			asset->GetAssetFilename()->SetFilename(targetAssetFilename->GetEncodedFilename());
			// Update our MOG_Properties with the new location of our asset
			asset->GetProperties()->UpdateAssetFilename(asset->GetAssetFilename(), asset->GetAssetPropertiesFilename());

			// Update the Inbox views
			UpdateInboxView(asset, status, sourceFilename, targetAssetFilename);

			// Update any links
			UpdateAssetLinkFiles(asset);

			bMoved = true;
		}
		else
		{
			String *message = String::Concat( S"MOG was unable to access the asset due to a potential file sharing violation.\n",
												S"ASSET: ", asset->GetAssetFilename()->GetAssetLabel(), S"\n",
												S"\n",
												S"Close any open files that are associated with this asset.");
			MOG_Prompt::PromptMessage(S"Move", message, "", MOG_ALERT_LEVEL::ERROR);
		}
	}

	return bMoved;
}


// Copy the asset to the specified targetBox
bool MOG_ControllerInbox::CopyToBox(MOG_Filename *assetFilename, String *targetBox, MOG_AssetStatusType status, BackgroundWorker* worker)
{
	bool bCopied = false;

	// Make sure this isn't a linked asset?
	if (!assetFilename->IsLink())
	{
		// Make sure this is a legitmate asset?
		if (assetFilename->GetFilenameType() == MOG_FILENAME_TYPE::MOG_FILENAME_Asset)
		{
			MOG_ControllerAsset *asset = MOG_ControllerAsset::OpenAsset(assetFilename);
			if (asset)
			{
				bCopied = CopyToBox(asset, targetBox, status, worker);

				// Always make sure to close the asset controller when we are finished
				asset->Close();
			}
		}
	}

	return bCopied;
}


// Copy the asset to the specified targetBox
bool MOG_ControllerInbox::CopyToBox(MOG_ControllerAsset *asset, String *targetBox, MOG_AssetStatusType status, BackgroundWorker* worker)
{
	// Make sure this isn't a Link?
	if (!asset->GetAssetFilename()->IsLink())
	{
		// Make sure a valid box was specified?
		if (targetBox->Length)
		{
			// Create the destination to the specified box
			MOG_Filename *destination = new MOG_Filename(asset->GetAssetFilename()->GetAssetEncodedInboxPath(MOG_ControllerProject::GetUserName_DefaultAdmin(), targetBox));

			return Copy(asset, destination, status, worker);
		}
	}

	return false;
}

// Copy the asset to the specified target - This was created for compatibility with existing APIs
bool MOG_ControllerInbox::Copy(MOG_ControllerAsset *asset, MOG_Filename *targetAssetFilename, MOG_AssetStatusType status, BackgroundWorker* worker)
{
	// Check if the source and destination are the same?
	if (String::Compare(asset->GetAssetFilename()->GetEncodedFilename(), targetAssetFilename->GetEncodedFilename(), true) == 0)
	{
		// Return true because we really can't do anything about this
		return true;
	}

	// Copy the asset
	MOG_ControllerAsset* newAsset = Copy_ReturnNewAsset(asset, targetAssetFilename, status, worker);
	if (newAsset)
	{
		// Close the returned newAsset
		newAsset->Close();
		return true;
	}

	return false;
}

// Copy the asset to the specified target and return the newAsset
MOG_ControllerAsset* MOG_ControllerInbox::Copy_ReturnNewAsset(MOG_ControllerAsset *asset, MOG_Filename *targetAssetFilename, MOG_AssetStatusType status, BackgroundWorker* worker)
{
	MOG_ControllerAsset *copiedAsset = NULL;

	// Check if the source and destination are the same?
	if (String::Compare(asset->GetAssetFilename()->GetEncodedFilename(), targetAssetFilename->GetEncodedFilename(), true) == 0)
	{
		// Return the asset because it is the asset
		return asset;
	}

	// Make sure this isn't a Link?
	if (!asset->GetAssetFilename()->IsLink())
	{
//?	MOG_ControllerInbox::Copy - Someday we need to perform trash management on the local updated asset as well
		// Check if this is an asset?
		// Check if we are copying it to a location within the inboxes?
		if (asset->GetAssetFilename()->GetFilenameType() == MOG_FILENAME_TYPE::MOG_FILENAME_Asset)
		{
			// Check if there is an asset in our way?
			if (DosUtils::DirectoryExistFast(targetAssetFilename->GetEncodedFilename()))
			{
				if (targetAssetFilename->IsWithinInboxes())
				{
					// Move the old asset out of our way and into the trash
					if (!MoveToTrash(targetAssetFilename))
					{
						return NULL;
					}
				}
				else if (targetAssetFilename->IsLocal())
				{
					// Delete it because we only do trash in the inboxes
					if (!Delete(targetAssetFilename))
					{
						return NULL;
					}
				}
			}
		}

//		// Get lock on the new destination
//		MOG_Command *command = new MOG_Command;
//		command->Setup_LockWriteRequest(targetAssetFilename->GetFullFilename, "MOG_Controller::Copy - Lock Destination");
//		if (MOG_ControllerSystem::GetCommandManager()->CommandProcess(command))
		{
			// Update inboxes to see that it is being copied
			UpdateInboxView(MOG_AssetStatusType::Copying, NULL, targetAssetFilename, asset->GetProperties());
			
			// Copy the Asset directory
			//We don't have the dialogId of the active dialog, so we will pass an invalid id to prevent a new one from popping up
			if (MOG_ControllerSystem::DirectoryCopyEx(asset->GetAssetFilename()->GetEncodedFilename(), targetAssetFilename->GetEncodedFilename(), worker))
			{
				// Open the new copied asset
				copiedAsset = MOG_ControllerAsset::OpenAsset(targetAssetFilename);
				if (copiedAsset)
				{
					// Check if this asset is headed towards the user's mailbox?
					if (targetAssetFilename->IsWithinInboxes())
					{
						// The target user always become the creator of the asset when it enters the inboxes
						copiedAsset->GetProperties()->Creator = targetAssetFilename->GetUserName();
						// Set a default yet somewhat informative LastComment
						copiedAsset->GetProperties()->LastComment = String::Concat(S"Copied to ", MOG_ControllerProject::GetUserName_DefaultAdmin(), S" on ", MOG_Time::FormatTimestamp(MOG_Time::GetVersionTimestamp(), S""));

						// Check if this asset has a BlessedTime?
						if (copiedAsset->GetProperties()->BlessedTime)
						{
							// Indicate this originating asset as the previous revision for this inbox asset
							copiedAsset->GetProperties()->PreviousRevision = copiedAsset->GetProperties()->BlessedTime;
							// Clear out the asset's BlessedTime as no inbox asset that gets copied should ever have a BlessedTime
							copiedAsset->GetProperties()->BlessedTime = "";
						}

						// Check if this asset just came from the repository?  or
						// Check if this asset is comming from the local updated tray?
						if (asset->GetAssetFilename()->IsWithinRepository() ||
							asset->GetAssetFilename()->IsLocal())
						{
							// Reset the comments log on this asset by deleteing the comments log file that just got copied along with this asset
							DosUtils::DeleteFast(copiedAsset->GetAssetCommentsFilename());
						}
						// Check if this asset was just copied from an inbox to an inbox?
						else if (asset->GetAssetFilename()->IsWithinInboxes() &&
								 copiedAsset->GetAssetFilename()->IsWithinInboxes())
						{
							// Make this asset UnBlessable because it is a copy of the real asset and should never go anywhere
							copiedAsset->GetProperties()->UnBlessable = true;
							status = MOG_AssetStatusType::Unblessable;
						}
					}

					// Check for valid 'InboxLinks' section?
					if (copiedAsset->GetProperties()->GetNonInheritedPropertiesIni()->SectionExist("InboxLinks"))
					{
						// Empty the 'InboxLinks' section because this is a copy and should start clean w/o any links of the original asset
						copiedAsset->GetProperties()->GetNonInheritedPropertiesIni()->EmptySection("InboxLinks");
					}

					// Update the Inbox views for this newly created asset
					UpdateInboxView(copiedAsset, status, NULL, targetAssetFilename);
				}
			}
			else
			{
				String *message = String::Concat( S"MOG was unable to access the asset due to a potential file sharing violation.\n",
													S"ASSET: ", asset->GetAssetFilename()->GetAssetLabel(), S"\n",
													S"\n",
													S"Close any open files that are associated with this asset.");
				MOG_Prompt::PromptMessage(S"Copy", message, "", MOG_ALERT_LEVEL::ERROR);
			}

//			// Release the lock we just obtained on the target to avoid nested locks
//			command->Setup_LockWriteRelease(targetAssetFilename->GetFullFilename);
//			MOG_ControllerSystem::GetCommandManager()->CommandProcess(command);
		}
	}

	return copiedAsset;
}


// Resolve colliding assets
bool MOG_ControllerInbox::ResolveCollidingAssets(MOG_ControllerAsset *asset, MOG_Filename *destinationFilename, BackgroundWorker* worker)
{
	bool bResolved = true;

	// Only perform this logic on assets in inboxes
	if (asset->GetAssetFilename()->IsWithinInboxes() &&
		destinationFilename->IsWithinInboxes())
	{
		// Check to see if destinationFilename exists?
		if (DosUtils::DirectoryExistFast(destinationFilename->GetEncodedFilename()))
		{
			// Indicate that there was an asset colision
			bResolved = false;

			// Open the asset that is in our way
			MOG_ControllerAsset *collidingAsset = MOG_ControllerAsset::OpenAsset(destinationFilename);
			if (collidingAsset)
			{
				// Check if our asset is the same?
				if (String::Compare(asset->GetProperties()->CreatedTime, collidingAsset->GetProperties()->CreatedTime, true) == 0)
				{
					// Send the already existing asset to the trash in favor of the new asset
					if (MoveToTrash(collidingAsset))
					{
						// Indicate that the asset colision was resolved
						bResolved = true;
					}
				}
				// Check if our asset is older?
				else if (String::Compare(asset->GetProperties()->CreatedTime, collidingAsset->GetProperties()->CreatedTime, true) < 0)
				{
					// Create an automated comment
					String *message = String::Concat(S"A newer revision of this asset already exists in ", collidingAsset->GetProperties()->Owner, S"'s Inbox.\n",
													 S"ASSET: ", collidingAsset->GetAssetFilename()->GetAssetLabel(), S"\n", 
													 S"This newer revision was modified or imported by ", collidingAsset->GetProperties()->Creator, S" on ", collidingAsset->GetProperties()->CreatedTime, S".\n",
													 S"Your older revision may be missing important changes found in the newer revision.\n");
					// Reject our asset back to it's creator
					if (Reject(asset, message, worker))
					{
						// Indicate that the asset colision was resolved
						bResolved = true;
					}
				}
				// Assume our asset is newer...
				else
				{
					String *message = "";

					// Create an automated comment
					// Is the current user the same as the creator of the asset getting rejected?
					if (String::Compare(asset->GetProperties()->Creator, collidingAsset->GetProperties()->Creator, true) == 0)
					{
						// Send the older version of this asset to the trash
						if (MoveToTrash(collidingAsset))
						{
							// Indicate that the asset colision was resolved
							bResolved = true;
						}
					}
					// Reject this asset back to it's creator
					else
					{
						// Create a more informed message because there were 2 different users involved
						message = String::Concat(S"While ", collidingAsset->GetProperties()->Owner, S" was responsible for this asset, ", asset->GetProperties()->Creator, S" sent him a newer revision of the same asset.\n",
												 S"ASSET: ", collidingAsset->GetAssetFilename()->GetAssetLabel(), S"\n", 
												 S"Since your revision was older, MOG can only assume that your changes were also included in the latest revision.\n",
												 S"It is recomended that you contact ", asset->GetProperties()->Creator, S" to confirm his revision included your changes.");

						// Reject this older asset back to it's creator
						if (Reject(collidingAsset, message, worker))
						{
							// Indicate that the asset colision was resolved
							bResolved = true;
						}
					}
				}
				collidingAsset->Close();
			}
		}
	}

	return bResolved;
}


bool MOG_ControllerInbox::RebuildInboxView(String *path, bool bSendViewUpdateEvent)
{
	MOG_Command *command = new MOG_Command;
	bool done = false;

	// Initialize the dialog
	String *boxName = DosUtils::PathGetFileName(path->Trim(S"\\ "->ToCharArray()));
	String* message = String::Concat(S"Please wait while '", boxName, S"' is rebuilt.");

	// Clear out the database
	MOG_DBInboxAPI::InboxRemoveAllAssets(new MOG_Filename(path));

	ProgressDialog* progress = new ProgressDialog("Rebuilding Inbox", message, new DoWorkEventHandler(NULL, &RebuildInbox_Worker), path, true);
	if (progress->ShowDialog() == DialogResult::OK)
	{
		done = true;
	}

	// Check if we should send server ViewUpdate event?
	if (bSendViewUpdateEvent)
	{
		MOG_Filename *box = new MOG_Filename(path);
		command = MOG_CommandFactory::Setup_ViewUpdate(path, path, MOG_AssetStatusType::Rebuilt);
		MOG_ControllerSystem::GetCommandManager()->SendToServer(command);
	}

	return done;
}

void MOG_ControllerInbox::RebuildInbox_Worker(Object* sender, DoWorkEventArgs* e)
{
	BackgroundWorker* worker = dynamic_cast<BackgroundWorker*>(sender);
	String* path = dynamic_cast<String*>(e->Argument);
	
 //JohnRen - Trash doesn't rebuild correctly??? - This code should have fixed it but I don't have time to debug it
	// Check if this is the trash box?
	if (path->EndsWith("\\Trash\\", StringComparison::CurrentCultureIgnoreCase))
	{
		// Get the list of asset located within this directory
		DirectoryInfo *assets[] = DosUtils::DirectoryGetList(path, S"*.*");
		if (assets != NULL)
		{
			// Add each asset into the Contents.Info file
			for (int a = 0; a < assets->Count; a++)
			{
				RebuildInbox_Worker(sender, new DoWorkEventArgs(assets[a]->FullName));
			}
		}

		return;
	}
	
	// Get the list of asset located within this directory
	DirectoryInfo *assets[] = DosUtils::DirectoryGetList(path, S"*.*");
	if (assets != NULL)
	{
		// Add each asset into the Contents.Info file
		for (int a = 0; a < assets->Count; a++)
		{
			MOG_Filename *assetFilename = new MOG_Filename(assets[a]->FullName);

			// Check if this is a group?
			if (assetFilename->GetFilenameType() == MOG_FILENAME_TYPE::MOG_FILENAME_Group)
			{
//? MOG_Main::RebuildContentsFile() - Check for groups and recursively call RebuildContentsFile()
//				// Recursively call RebuildView() for each group
//				RebuildInboxView(assetFilename->GetEncodedFilename(), bSendViewUpdateEvent);
			}

			// Check if this is an asset?
			if (assetFilename->GetFilenameType() == MOG_FILENAME_TYPE::MOG_FILENAME_Asset)
			{
				String *message = String::Concat(S"Adding:\n",
												 S"     ", assetFilename->GetAssetClassification(), S"\n",
												 S"     ", assetFilename->GetAssetName());

				if (worker != NULL)
				{
					worker->ReportProgress(a * 100 / assets->Count, message);
				}

				// Open the asset up so that we can get the information out of it
				MOG_ControllerAsset *asset = MOG_ControllerAsset::OpenAsset(assetFilename);
				if (asset)
				{
					// Update all the asset's properties
					UpdateAssetProperties(asset);

					// Always make sure we close the asset when we are finished
					asset->Close();
				}
			}
		}
	}

	// Get the list of links located within this directory
	FileInfo *links[] = DosUtils::FileGetList(path, S"*.*");
	if (links != NULL)
	{
		// Add each link into the Contents.Info file
		for (int a = 0; a < links->Count; a++)
		{
			MOG_Filename *linkFilename = new MOG_Filename(links[a]->FullName);

			// Check if this is a link?
			if (linkFilename->GetFilenameType() == MOG_FILENAME_TYPE::MOG_FILENAME_Link)
			{
				String *message = String::Concat(S"Rebuilding:\n",
												 S"     ", linkFilename->GetAssetClassification(), S"\n",
												 S"     ", linkFilename->GetAssetName());

				if (worker != NULL)
				{
					worker->ReportProgress(a * 100 / links->Count, message);
				}

				// Update all the link's properties
				UpdateLinkProperties(linkFilename);
			}
		}
	}
}


bool MOG_ControllerInbox::UpdateAssetProperties(MOG_ControllerAsset *asset)
{
	// Make sure the asset is added
	if (MOG_DBInboxAPI::InboxAddAsset(asset->GetAssetFilename()))
	{
		// Add all of it's propeerties
		return MOG_DBInboxAPI::UpdateInboxAssetProperties(asset->GetAssetFilename(), asset->GetProperties()->GetPropertyList());
	}

	return false;
}


bool MOG_ControllerInbox::UpdateLinkProperties(MOG_Filename *linkFilename)
{
	// Open the linkFile?
	MOG_PropertiesIni *linkFile = new MOG_PropertiesIni(linkFilename->GetEncodedFilename());
	if (linkFile)
	{
		// Add this asset
		if (MOG_DBInboxAPI::InboxAddAsset(linkFilename))
		{
			// Get the link's properties by using its own file
			MOG_Properties *pProperties = MOG_Properties::OpenFileProperties(linkFile);
			// Add all the link's properties to the database
			return MOG_DBInboxAPI::UpdateInboxAssetProperties(linkFilename, pProperties->GetPropertyList());
		}
	}

	return true;
}


void MOG_ControllerInbox::UpdateInboxView(String *status, MOG_Filename *sourceFilename, MOG_Filename *targetFilename)
{
	UpdateInboxView(NULL, status, sourceFilename, targetFilename, NULL);
}

void MOG_ControllerInbox::UpdateInboxView(MOG_AssetStatusType status, MOG_Filename *sourceFilename, MOG_Filename *targetFilename)
{
	UpdateInboxView(NULL, MOG_AssetStatus::GetText(status), sourceFilename, targetFilename, NULL);
}

void MOG_ControllerInbox::UpdateInboxView(MOG_AssetStatusType status, MOG_Filename *sourceFilename, MOG_Filename *targetFilename, MOG_Properties *properties)
{
	UpdateInboxView(NULL, MOG_AssetStatus::GetText(status), sourceFilename, targetFilename, properties);
}

void MOG_ControllerInbox::UpdateInboxView(MOG_ControllerAsset *asset, String *status)
{
	UpdateInboxView(asset, status, asset->GetAssetFilename(), asset->GetAssetFilename(), asset->GetProperties());
}

void MOG_ControllerInbox::UpdateInboxView(MOG_ControllerAsset *asset, MOG_AssetStatusType status)
{
	UpdateInboxView(asset, MOG_AssetStatus::GetText(status), asset->GetAssetFilename(), asset->GetAssetFilename(), asset->GetProperties());
}

void MOG_ControllerInbox::UpdateInboxView(MOG_ControllerAsset *asset, MOG_AssetStatusType status, MOG_Filename *sourceFilename, MOG_Filename *targetFilename)
{
	UpdateInboxView(asset, MOG_AssetStatus::GetText(status), sourceFilename, targetFilename, asset->GetProperties());
}

void MOG_ControllerInbox::UpdateInboxView(MOG_ControllerAsset *asset, String *status, MOG_Filename *sourceFilename, MOG_Filename *targetFilename, MOG_Properties *properties)
{
	String *viewSource = S"";
	String *viewTarget = S"";

	// Check if we have a valid asset?
	if (asset)
	{
		// Check if this status is actually being changed?
		if (String::Compare(asset->GetProperties()->Status, status, true) != 0)
		{
			// Update the asset's status
			asset->GetProperties()->Status = status;
		}

		// Check if there is a targetFilename?
		if (targetFilename)
		{
			// Check if there is an owner specified?
			if (targetFilename->GetUserName()->Length)
			{
				// Always update the Owner of this Asset
				asset->GetProperties()->Owner = targetFilename->GetUserName();
			}
		}
	}

	// Check if there is a sourceFilename?
	if (sourceFilename && 
		(sourceFilename->IsWithinInboxes() || sourceFilename->IsLocal()))
	{
		// Always remove the sourceFilename
		viewSource = sourceFilename->GetEncodedFilename();
	}
	// Check if there is a targetFilename?
	if (targetFilename && 
		(targetFilename->IsWithinInboxes() || targetFilename->IsLocal()))
	{
		// Always add the targetFilename
		viewTarget = targetFilename->GetEncodedFilename();
	}

	// If we have a viewSource and a viewTarget, Fixup the inbox database?
	if (viewSource->Length && viewTarget->Length)
	{
		// Make sure the source and target are differnt
		if (String::Compare(viewSource, viewTarget, true) != 0)
		{
			// Move the asset within the database
			MOG_DBInboxAPI::InboxMoveAsset(sourceFilename, targetFilename);
		}
	}
	else if (viewSource->Length)
	{
		// Remove the asset from the database
		MOG_DBInboxAPI::InboxRemoveAsset(sourceFilename);
	}
	else if (viewTarget->Length)
	{
		// Populate the Inbox database
		MOG_DBInboxAPI::InboxAddAsset(targetFilename);

		// Check if we have a valid asset?
		if (asset)
		{
			// Always trigger the properties to push themselves to the database because we are a new target
			asset->GetProperties()->GetNonInheritedPropertiesIni()->HasChanged(true);
		}
	}


	// Create the ViewUpdate command using any specified properties
	MOG_Command *view = MOG_CommandFactory::Setup_ViewUpdate(viewSource, viewTarget, status, properties);
	// Check if there was an asset specified?
	if (asset)
	{
		// Inform the asset about this pending ViewUpdate command for later submission to the server
		asset->SetPendingViewUpdate(view);
	}
	else
	{
		// Send the ViewUpdate command to the server now
		MOG_ControllerSystem::GetCommandManager()->SendToServer(view);
	}
}


MOG_Filename *MOG_ControllerInbox::LocateBestMatchingAsset(MOG_Filename *assetFilename)
{
	MOG_Filename *bestAssetFilename = NULL;

	// Locate all instances of this asset in our inbox
	String *filter = String::Concat(S"*\\", assetFilename->GetAssetFullName());
	ArrayList *assets = MOG_DBInboxAPI::InboxGetAssetList(S"", MOG_ControllerProject::GetUserName_DefaultAdmin(), "", filter);
	if (assets)
	{
		int bestPriority = 0;

		// Check these assets for a match?
		for (int a = 0; a < assets->Count; a++)
		{
			MOG_Filename *testAssetFilename = __try_cast<MOG_Filename *>(assets->Item[a]);

			int priority = 0;

			// Ignore these boxes
			if (testAssetFilename->IsInbox() ||
				testAssetFilename->IsOutBox() ||
				testAssetFilename->IsTrash())
			{
				continue;
			}

			// Weight this asset's priority based on it's box
			if (String::Compare(testAssetFilename->GetBoxName(), S"Drafts", true) == 0)
			{
				priority = 9;
			}
// JohnRen - Respecting the inbox can get tricky...there are a few gotchas
//			else if (String::Compare(testAssetFilename->GetBoxName(), S"Inbox", true) == 0)
//			{
//				priority = 3;
//			}
			else
			{
				// Check if we have a current workspace directory?
				String *workspaceDirectory = MOG_ControllerProject::GetWorkspaceDirectory();
				if (workspaceDirectory->Length)
				{
					// Check if this asset is within our current workspace?
					if (testAssetFilename->GetPath()->ToLower()->StartsWith(workspaceDirectory->ToLower()))
					{
						priority = 2;
					}
				}
			}

			if (priority > bestPriority)
			{
				// Save this as our new priority
				bestPriority = priority;
				// Indicate this as our source PropertiesFile
				bestAssetFilename = testAssetFilename;
			}
		}
	}

	return bestAssetFilename;
}


String *MOG_ControllerInbox::GetInboxPath(String *boxName)
{
	return GetInboxPath(MOG_ControllerProject::GetUserName_DefaultAdmin(), boxName, S"");
}


String *MOG_ControllerInbox::GetInboxPath(String *userName, String *boxName)
{
	return GetInboxPath(userName, boxName, S"");
}


String *MOG_ControllerInbox::GetInboxPath(String *userName, String *boxName, String *groupPath)
{
	String *path = S"";

	// Check if we are missing a user name?
	if (boxName->Length == 0)
	{
		// Default to the drafts folder
		boxName = S"Drafts";
	}

	// Make sure this is a valid box name?
	if (String::Compare(boxName, S"Drafts", true) == 0 ||
		String::Compare(boxName, S"Inbox", true) == 0 ||
		String::Compare(boxName, S"Outbox", true) == 0 ||
		String::Compare(boxName, S"Trash", true) == 0)
	{
		// Check if we are missing a user name?
		if (userName->Length == 0)
		{
			// Obtain the logged in user
			userName = MOG_ControllerProject::GetUserName_DefaultAdmin();
		}

		// Ask the project to resolve the user's path
		String *userPath = MOG_ControllerProject::GetUserPath(userName);

		path = String::Concat(userPath, S"\\", boxName);

		// Check if this asset is within a group?
		if (groupPath->Length)
		{
			path = String::Concat(path, S"\\", groupPath);
		}
	}

	return path;
}


String *MOG_ControllerInbox::ObtainTimestampFromJobLabel(String *jobLabel)
{
	return jobLabel->Substring(jobLabel->Length - 15);
}

ArrayList* MOG_ControllerInbox::GetInboxConflictsForAsset(MOG_Filename* assetFilename, String* creator)
{
	ArrayList* conflicts = new ArrayList();

	// Locate all instances of this asset throughout all inboxes
	String *filter = String::Concat(S"*\\", assetFilename->GetAssetFullName());
	ArrayList *assets = MOG_DBInboxAPI::InboxGetAssetListWithProperties(S"", "", "", filter);
	if (assets)
	{
		// Check these assets for a match?
		for (int a = 0; a < assets->Count; a++)
		{
			MOG_DBInboxProperties* inboxItem = __try_cast<MOG_DBInboxProperties*>(assets->Item[a]);
			MOG_Filename* inboxFilename = inboxItem->mAsset;

			// Ignore anything outside of the inboxes
			if (!inboxFilename->IsWithinInboxes())
			{
				// Ignore this asset because it isn't an inbox asset
				continue;
			}

			// Ingore anything in the Trash and Sent
			if (inboxFilename->IsOutBox() ||
				inboxFilename->IsTrash())
			{
				// We can ignore Trash and Sent
				continue;
			}

			// Ignore anything in our own inbox
			if (String::Compare(inboxFilename->GetUserName(), MOG_ControllerProject::GetUserName(), true) == 0)
			{
				// Never warn us about our own assets
				continue;
			}

			// Get the properties of this asset
			MOG_Properties *inboxProperties = new MOG_Properties(inboxItem->mProperties);

			// Check if this is an unblessable asset?
			if (inboxProperties->UnBlessable)
			{
				// We can ignore this asset because it will never be blessed
				continue;
			}

			// Check if this is by the same creator as out asset?
			if (String::Compare(inboxProperties->Creator, creator, true) == 0)
			{
				// We can ignore this asset because it was created by the same user
				continue;
			}

			// Looks like this is a legitimate conflict!
			String* conflict = String::Concat(inboxFilename->GetBoxName(), S": ", inboxProperties->Owner, S"     CreatedBy: ", inboxProperties->Creator, S"     ", MOG_Time::FormatTimestamp(inboxProperties->CreatedTime, S""));
			conflicts->Add(conflict);
		}
	}

	return conflicts;
}