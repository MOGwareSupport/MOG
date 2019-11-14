//--------------------------------------------------------------------------------
//	MOG_ControllerSyncData.cpp
//	
//	
//--------------------------------------------------------------------------------

#include "StdAfx.h"

#include "MOG_Define.h"
#include "MOG_DosUtils.h"
#include "MOG_ControllerAsset.h"
#include "MOG_ControllerRepository.h"
#include "MOG_ControllerProject.h"
#include "MOG_ControllerSystem.h"
#include "MOG_ControllerPackage.h"
#include "MOG_ControllerPackageMergeLocal.h"
#include "MOG_ControllerInbox.h"
#include "MOG_ControllerLibrary.h"
#include "MOG_Tokens.h"
#include "MOG_DBSyncedDataAPI.h"
#include "MOG_DBAssetAPI.h"
#include "MOG_DBInboxAPI.h"
#include "MOG_Progress.h"
#include "MOG_CommandFactory.h"

#include "MOG_ControllerSyncData.h"
#include "MOG_StringUtils.h"

using namespace System::Collections::Generic;
using namespace MOG_CoreControls;

MOG_ControllerSyncData::MOG_ControllerSyncData(String *syncDirectory, String *projectName, String *branchName, String *platformName, String *userName)
{
	// Retain our own member variables within our controller
	mSyncDirectory = syncDirectory;

	// Check if a project name was specified
	mProjectName = projectName;
	if (!mProjectName->Length)
	{
		// Default to the current project
		mProjectName = MOG_ControllerProject::GetProjectName();
	}

	// Check if a branch name was specified
	mBranchName = branchName;
	if (!mBranchName->Length)
	{
		// Default to the current project
		mBranchName = MOG_ControllerProject::GetBranchName();
	}

	// Check if a platform name was specified
	mPlatformName = platformName;
	if (!mPlatformName->Length)
	{
		// Default to the current project
		mPlatformName = MOG_ControllerProject::GetPlatformName();
	}

	// Check for the specified user name?
	mUserName = userName;
	if(!mUserName->Length)
	{
		mUserName = MOG_ControllerProject::GetUserName_DefaultAdmin();
	}

	// Set default name
	// KLK - New default tab name per walkers request
	mName = String::Concat(syncDirectory);

	// create a sync log
	mSyncLog = new MOG_Ini();

	mLocalAssets = NULL;

	InitializePackageState();
}

MOG_ControllerSyncData::MOG_ControllerSyncData(String *name, String *syncDirectory, String *projectName, String *branchName, String *platformName, String *userName)
{
	// Retain our own member variables within our controller
	mSyncDirectory = syncDirectory;

	// Check if a project name was specified
	mProjectName = projectName;
	if (!mProjectName->Length)
	{
		mProjectName = MOG_ControllerProject::GetProjectName();
	}

	// Check if a branch name was specified
	mBranchName = branchName;
	if (!mBranchName->Length)
	{
		// Default to the current project
		mBranchName = MOG_ControllerProject::GetBranchName();
	}

	// Check if a platform name was specified
	mPlatformName = platformName;
	if (!mPlatformName->Length)
	{
		// Default to the current project
		mPlatformName = MOG_ControllerProject::GetPlatformName();
	}

	// Check for the specified user name?
	mUserName = userName;
	if(!mUserName->Length)
	{
		mUserName = MOG_ControllerProject::GetUserName_DefaultAdmin();
	}

	// Set default name
	mName  = name;

	// create a sync log
	mSyncLog = new MOG_Ini();

	mLocalAssets = NULL;

	InitializePackageState();
}


MOG_ControllerSyncData::~MOG_ControllerSyncData(void)
{
//	Close();
}

void MOG_ControllerSyncData::InitializePackageState()
{
	// Clear our IsPackaging state
	mIsPackaging = false;
	// Clear our list of assets being packaged
	mAssetNamesGettingPackagedList = new ArrayList();

	// Check if we have nothing to do?
	if (GetLocalPackagingStatus() == PackageState_NothingToDo)
	{
		// Reset our YesToAll and NoToAll flags
		mUnpackageLocalAsset->Reset();
	}
}

bool MOG_ControllerSyncData::Rename(String* newTabName)
{
	if (MOG_DBSyncedDataAPI::RenameWorkspaceTab(MOG_ControllerSystem::GetComputerName(), GetProjectName(), GetPlatformName(), GetSyncDirectory(), MOG_ControllerProject::GetUserName(), newTabName))
	{
		mName = newTabName;
		return true;
	}

	return false;
}

bool MOG_ControllerSyncData::ValidateLocalWorkspace(void)
{
	if (mSyncDirectory->Length)
	{
		DirectoryInfo *info = new DirectoryInfo(String::Concat(mSyncDirectory, S"\\MOG"));
		if (info)
		{
			// Check if we need to create the MOG directory?
			if (!info->Exists)
			{
				info->Create();
			}

			// Always make sure we hide this directory
			info->Attributes = FileAttributes::Hidden;
			return true;
		}
	}

	return false;
}

// Import the asset to the local workspace
bool MOG_ControllerSyncData::AddAssetToLocalUpdatedTray(MOG_ControllerAsset* asset)
{
	return AddAssetToLocalUpdatedTray(asset, NULL);
}

// Import the asset to the local workspace
bool MOG_ControllerSyncData::AddAssetToLocalUpdatedTray(MOG_ControllerAsset* asset, BackgroundWorker* worker)
{
	bool bFailed = false;
	
	// Make sure we have a valid name
	if (asset)
	{
		MOG_Filename* assetFilename = asset->GetAssetFilename();

		// FileHamster Integration...
		// Make sure we ignore everything in the hidden MOG directory
		MOG_FileHamsterIntegration::FileHamsterPause(String::Concat(mSyncDirectory, S"\\MOG\\Sync.Info"));

		// Check for platform specific versus non platform specific collisions?
		if (assetFilename->IsPlatformSpecific())
		{
			MOG_Filename *genericAssetFilename = MOG_Filename::CreateAssetName(assetFilename->GetAssetClassification(), S"All", assetFilename->GetAssetLabel());
			MOG_Filename *updatedAssetFilename = MOG_Filename::GetLocalUpdatedTrayFilename(mSyncDirectory, genericAssetFilename);
			// Check if the generic asset already exists in the local workspace?
			if (DosUtils::DirectoryExistFast(updatedAssetFilename->GetEncodedFilename()))
			{
				// Remove the generic asset because the platform specific one is on its way
				MOG_ControllerSyncData::RemoveAssetFromLocalWorkspace(updatedAssetFilename, worker);
			}
		}

		// Make sure the asset is processed?
		if (asset->IsProcessed())
		{
			// Always assume an imported status
			MOG_AssetStatusType finalStatus = MOG_AssetStatusType::Imported;
//			// Check if this is a packaged asset?
//			if (asset->GetProperties()->IsPackagedAsset)
//			{
//				finalStatus = MOG_AssetStatusType::Packaged;
//			}

			// Copy the asset into the local UpdatedAssets directory
			MOG_Filename *updatedAssetFilename = MOG_Filename::GetLocalUpdatedTrayFilename(mSyncDirectory, assetFilename);
			if (MOG_ControllerInbox::Copy(asset, updatedAssetFilename, finalStatus, worker))
			{
				// The asset was successfully updated
			}
			else
			{
				bFailed = true;
			}
		}
		else
		{
			bFailed = true;
		}
	}
	else
	{
		bFailed = true;
	}

	// Check if we failed?
	if (!bFailed)
	{
		return true;
	}
	return false;
}


// Copy the asset's files to the local project's directory
bool MOG_ControllerSyncData::CanAddAssetToLocalWorkspace(MOG_ControllerAsset* asset, bool bInformUser)
{
	// Make sure we have a valid asset?
	if (!asset)
	{
		return false;
	}

	MOG_Filename* assetFilename = asset->GetAssetFilename();
	MOG_Properties* pProperties = asset->GetProperties(mPlatformName);

	//Only add this asset to the local workspace if it is "All" or matches the current platform
	if (String::Compare(assetFilename->GetAssetPlatform(), S"All", true) != 0 &&
		String::Compare(assetFilename->GetAssetPlatform(), mPlatformName, true) != 0)
	{
		// Check if we have been instructed to inform the user about why we failed?
		if (bInformUser)
		{
			// Warn the user about the platform specific asset taking precedence
			String* message = String::Concat(	S"This asset cannot be updated to the local workspace because it is a platform-specific asset that doesn't match the local workspace's platform.\n",
												S"ASSET: ", assetFilename->GetAssetFullName());
			MOG_Prompt::PromptMessage("Local Update Failed", message, S"", MOG_ALERT_LEVEL::ALERT);
		}
		return false;
	}

	// Check for platform specific versus non platform specific collisions?
	if (!assetFilename->IsPlatformSpecific())
	{
		MOG_Filename *platformSpecificAssetFilename = MOG_Filename::CreateAssetName(assetFilename->GetAssetClassification(), mPlatformName, assetFilename->GetAssetLabel());
		MOG_Filename *updatedAssetFilename = MOG_Filename::GetLocalUpdatedTrayFilename(mSyncDirectory, platformSpecificAssetFilename);
		// Check if the generic asset already exists in the local workspace? or
		// Check if the project has a platform specific version of this asset?
		if (DosUtils::DirectoryExistFast(updatedAssetFilename->GetEncodedFilename()) ||
			MOG_ControllerProject::DoesPlatformSpecificAssetExists(assetFilename, mPlatformName))
		{
			// Check if we have been instructed to inform the user about why we failed?
			if (bInformUser)
			{
				// Warn the user about the platform specific asset taking precedence
				String* message = String::Concat(	S"This asset cannot be updated to the local workspace because the project has a platform-specific version of this asset.\n",
													S"ASSET: ", assetFilename->GetAssetFullName(), S"\n\n",
													S"Please use the platform-specific version of this asset.");
				MOG_Report::ReportMessage("Local Update Failed", message, S"", MOG_ALERT_LEVEL::ALERT);
			}
			return false;
		}
	}

	// Check if this asset hasn't been processed?
	if (!asset->IsProcessed())
	{
		// Check if we have been instructed to inform the user about why we failed?
		if (bInformUser)
		{
			// Inform the user that only processed assets will be allowed into the local workspace
			String* message = String::Concat(	S"This is an unprocessed asset.\n",
												S"ASSET: ", assetFilename->GetAssetFullName(), S"\n\n",
												S"Only processed assets can be updated in the local workspace.");
			MOG_Report::ReportMessage("Update Failed", message, S"", MOG_ALERT_LEVEL::ERROR);
		}
		return false;
	}

	// Make sure this asset will either be synced or packaged before we do anymore?
	if (!pProperties->IsPackagedAsset && 
		!pProperties->SyncFiles)
	{
		// Check if we have been instructed to inform the user about why we failed?
		if (bInformUser)
		{
			// Inform the user why it was not copied to the local workspace
			String* message = String::Concat(	S"The asset was not updated in the workspace because there wasn't anything needing to be done with its files.\n",
												S"Please check the asset's properties for either 'SyncFiles=True' or 'PackagedAsset=True'.\n"
												S"ASSET: ", assetFilename->GetAssetFullName());
			MOG_Prompt::PromptMessage("Sync Error", message, S"", MOG_ALERT_LEVEL::ERROR);
		}
		return false;
	}

	// Check if this asset requires packaging?
	if (pProperties->IsPackagedAsset)
	{
		// Make sure we have valid package assignments
		if (!MOG_ControllerAsset::ValidateAsset_HasPackageAssignment(pProperties))
		{
			// Check if we have been instructed to inform the user about why we failed?
			if (bInformUser)
			{
				// Indicate we are missing a package assignment?
				String *message = String::Concat(	S"This asset indicates it is a packaged asset yet it has no package assignments so it cannot be updated to the local workspace.\n",
													S"ASSET: ", assetFilename->GetAssetLabel(), S"\n\n", 
													S"Package assignments are made from the package management dialog.");
				MOG_Prompt::PromptMessage(S"Package Assignment", message, "", MOG_ALERT_LEVEL::ERROR);
			}
			return false;
		}

		// Make sure we have valid package assignments
		if (!MOG_ControllerAsset::ValidateAsset_AllAssignedPackagesExist(pProperties))
		{
			// Check if we have been instructed to inform the user about why we failed?
			if (bInformUser)
			{
				// Fail because this package doesn't exist in the project
				String *message = String::Concat(	S"This asset contains a package assignment to a nonexistent package.\n",
													S"ASSET: ", assetFilename->GetAssetLabel(), S"\n\n", 
//													S"PACKAGE: ", packageFilename->GetAssetLabel(), S"\n\n", 
													S"Either bless the new package, create the new package or remove this package assignment.");
				MOG_Report::ReportMessage("Local Update Failed", message, S"", MOG_ALERT_LEVEL::ERROR);
			}
			return false;
		}

		// Make sure we have valid package assignments
		if (!MOG_ControllerAsset::ValidateAsset_PlatformApplicable(pProperties, mPlatformName))
		{
			// Check if we have been instructed to inform the user about why we failed?
			if (bInformUser)
			{
				// Indicate we are missing a package assignment?
				String *message = String::Concat(	S"This asset has no package assignment that is applicable for this platform.\n",
													S"ASSET: ", assetFilename->GetAssetLabel(), S"\n", 
													S"PLATFORM: ", mPlatformName, S"\n\n", 
													S"Check package assignments in the package management dialog.");
				MOG_Prompt::PromptMessage(S"Package Assignment", message, "", MOG_ALERT_LEVEL::ERROR);
			}
			return false;
		}
	}

	return true;
}


// Copy the asset's files to the local project's directory with no worker
bool MOG_ControllerSyncData::AddAssetToLocalWorkspace(MOG_ControllerAsset* asset)
{
	return AddAssetToLocalWorkspace(asset, NULL);
}

// Copy the asset's files to the local project's directory
bool MOG_ControllerSyncData::AddAssetToLocalWorkspace(MOG_ControllerAsset* asset, BackgroundWorker* worker)
{
	bool bFailed = false;

	// Make sure we have a valid name
	if (asset)
	{
		MOG_Filename* assetFilename = asset->GetAssetFilename();

		// FileHamster Integration...
		// Make sure we ignore everything in the hidden MOG directory
		MOG_FileHamsterIntegration::FileHamsterPause(String::Concat(mSyncDirectory, S"\\MOG\\Sync.Info"));

		// Check for platform specific versus non platform specific collisions?
		if (assetFilename->IsPlatformSpecific())
		{
			MOG_Filename *genericAssetFilename = MOG_Filename::CreateAssetName(assetFilename->GetAssetClassification(), S"All", assetFilename->GetAssetLabel());
			MOG_Filename *updatedAssetFilename = MOG_Filename::GetLocalUpdatedTrayFilename(mSyncDirectory, genericAssetFilename);
			// Check if the generic asset already exists in the local workspace?
			if (DosUtils::DirectoryExistFast(updatedAssetFilename->GetEncodedFilename()))
			{
				// Remove the generic asset because the platform specific one is on its way
				MOG_ControllerSyncData::RemoveAssetFromLocalWorkspace(updatedAssetFilename, worker);
			}
		}

		// Get the properties of the new asset for the given platform
		MOG_Properties* pProperties = asset->GetProperties(mPlatformName);

		// Make sure srcAsset is processed?
		if (asset->IsProcessed())
		{
			bool bTryAutoPackage = false;

			// Make sure this asset will either be synced or packaged before we do anymore?
			if (pProperties->IsPackagedAsset || 
				pProperties->SyncFiles)
			{
				MOG_Filename *oldAssetFilename = NULL;
				MOG_ControllerAsset* oldAsset = NULL;
				MOG_Properties* oldAssetProperties = NULL;
				MOG_AssetStatusType oldAssetFinalState = MOG_AssetStatusType::None;

				try
				{
					// Check if this asset already exists in the updated tray?
					MOG_Filename* localAssetFilename = MOG_Filename::GetLocalUpdatedTrayFilename(mSyncDirectory, assetFilename);
					if (DosUtils::DirectoryExistFast(localAssetFilename->GetEncodedFilename()))
					{
						ArrayList *localAssets = MOG_DBInboxAPI::InboxGetLocalAssetList(mSyncDirectory, assetFilename);
						if (localAssets &&
							localAssets->Count > 0)
						{
							// Build our text strings for the various status types
							String* textPackaging = MOG_AssetStatus::GetText(MOG_AssetStatusType::Packaging);
							String* textRepackaging = MOG_AssetStatus::GetText(MOG_AssetStatusType::Repackaging);
							String* textUnpackaging = MOG_AssetStatus::GetText(MOG_AssetStatusType::Unpackaging);
							String* textUnpackaged = MOG_AssetStatus::GetText(MOG_AssetStatusType::Unpackaged);

							// Create a real MOG_Properties from the MOG_DBInboxProperties
							MOG_DBInboxProperties *inboxProp = __try_cast<MOG_DBInboxProperties*>(localAssets->Item[0]);
							MOG_Properties *properties = new MOG_Properties(inboxProp->mProperties);

							// Check if this asset is currently involved in a package event?
							if (String::Compare(properties->Status, textPackaging, true) == 0 ||
								String::Compare(properties->Status, textRepackaging, true) == 0 ||
								String::Compare(properties->Status, textUnpackaging, true) == 0)
							{
								// Check if we currently have a package merge in progress?
								if (mIsPackaging)
								{
									// Inform the user that only processed assets will be allowed into the local workspace
									String* message = String::Concat(	S"This asset could not be updated to the local workspace because a previous version of this asset is in the process of being packaged.\n",
																		S"ASSET: ", localAssetFilename->GetAssetFullName(), S"\n\n",
																		S"Please wait until the active package merge is finished then try again.");
									MOG_Prompt::PromptMessage("Local Update Failed", message, S"", MOG_ALERT_LEVEL::ALERT);
									bFailed = true;
									return false;
								}
							}
							// Check if this old asset is anything other than 'Unpackaged'?
							if (String::Compare(properties->Status, textUnpackaged, true) != 0)
							{
								// Move this asset over to the removeTray so it can be preserved for unpackaging
								MOG_Filename *removeAssetFilename = MOG_Filename::GetLocalRemoveTrayFilename(mSyncDirectory, localAssetFilename);
								if (DosUtils::DirectoryMoveFast(localAssetFilename->GetEncodedFilename(), removeAssetFilename->GetEncodedFilename(), true))
								{
									oldAssetFilename = removeAssetFilename;
								}
								else
								{
									bFailed = true;
									return false;
								}
							}
						}
					}
					else
					{
						// Check if the asset already existed in the LocalBranch?
						String *revision = MOG_DBSyncedDataAPI::GetLocalAssetVersion(MOG_ControllerSystem::GetComputerName(), mProjectName, mPlatformName, mSyncDirectory, mUserName, assetFilename);
						if (revision->Length)
						{
							// Build the oldAssetFilename to the blessedAssetFilename
							oldAssetFilename = new MOG_Filename(MOG_ControllerRepository::GetAssetBlessedVersionPath(assetFilename, revision));
						}
					}

					// Check if we found an oldAssetFilename?
					if (oldAssetFilename)
					{
						// Open the old asset that is already in the local updated tray
						oldAsset = MOG_ControllerAsset::OpenAsset(oldAssetFilename);
						if (oldAsset)
						{
							oldAssetProperties = oldAsset->GetProperties(mPlatformName);

							// Check if this is a local asset?
							if (oldAssetFilename->IsLocal())
							{
								// Determin the final state for the old local asset?
								oldAssetFinalState = (oldAssetProperties->IsPackagedAsset) ? MOG_AssetStatusType::Unpackage : MOG_AssetStatusType::Copied;
								oldAssetProperties->Status = MOG_AssetStatus::GetText(oldAssetFinalState);
							}

							// Check if this asset requires packaging?
							if (oldAssetProperties->IsPackagedAsset)
							{
								bTryAutoPackage = true;
							}
						}
					}

					// Figure out what our final state should be now before we copy srcAsset
					MOG_AssetStatusType newAssetFinalState = MOG_AssetStatusType::Copied;
					// Check if this is a packaged asset?
					if (pProperties->IsPackagedAsset)
					{
						// Check if the old asset was already in our local tray?
						if (oldAssetFilename &&
							oldAssetFilename->IsLocal())
						{
							// Indicate we are repackaging this asset
							newAssetFinalState = MOG_AssetStatusType::Repackage;
						}
						else
						{
							// Indicate this needs to be packaged
							newAssetFinalState = MOG_AssetStatusType::Unpackaged;
						}
					}

					// Copy the asset into the local UpdatedAssets directory leaving it in a 'Copying' state
					MOG_ControllerAsset* newAsset = MOG_ControllerInbox::Copy_ReturnNewAsset(asset, localAssetFilename, MOG_AssetStatusType::Copying, worker);
					if (newAsset)
					{
						try
						{
							MOG_Properties* newAssetProperties = newAsset->GetProperties(mPlatformName);

							// Process the files accounting for some files not needing to be synced
							if (!ProcessAssetFiles(oldAssetProperties, newAssetProperties, worker))
							{
								bFailed = true;
							}

							// Check if this asset requires packaging?
							if (newAssetProperties->IsPackagedAsset)
							{
								bTryAutoPackage = true;
							}
						}
						__finally
						{
							// Set newAsset's final status and close
							MOG_ControllerInbox::UpdateInboxView(newAsset, newAssetFinalState);
							newAsset->Close();
						}
					}
					else
					{
						bFailed = true;
					}
				}
				__finally
				{
					// Free the lock on this asset now and delete its directory to make way for the asset being added
					if (oldAsset)
					{
						bool bDeleteLocalAsset = false;

						// Check if this old asset is local? and
						// Make sure this old asset doesn't need to be unpackaged?
						if (oldAssetFilename->IsLocal() &&
							oldAssetFinalState != MOG_AssetStatusType::Unpackage)
						{
							bDeleteLocalAsset = true;
						}

						// Close the oldAsset controller
						oldAsset->Close();

						// Make sure we haven't failed?
						if (!bFailed)
						{
							// Check if we should delete the oldAsset's directory now that it is closed?
							if (bDeleteLocalAsset)
							{
								DosUtils::DirectoryDeleteFast(oldAssetFilename->GetEncodedFilename());
							}
						}
					}
				}

				// Check if we determine we should attempt to start the auto package?
				if (bTryAutoPackage)
				{
					// If autopackaging is enabled, package anything that is pending in our local workspace
					AutoPackage();
				}
			}
			else
			{
				bFailed = true;
			}
		}
		else
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


bool MOG_ControllerSyncData::RemoveAssetFromLocalWorkspace(MOG_Filename *assetFilename, BackgroundWorker* worker)
{
	MOG_ControllerAsset *oldAsset = NULL;
	bool bFailed = false;
	bool bDeleteLocalAsset = true;
	bool bTryAutoPackage = false;

	// Make sure we have a valid name
	if (assetFilename && assetFilename->GetAssetFullName()->Length)
	{
		// FileHamster Integration...
		// Make sure we ignore everything in the hidden MOG directory
		MOG_FileHamsterIntegration::FileHamsterPause(String::Concat(mSyncDirectory, S"\\MOG\\Sync.Info"));

		// Open this asset
		oldAsset = MOG_ControllerAsset::OpenAsset(assetFilename);
		if (oldAsset)
		{
			// Get the properties for the removeAsset
			MOG_Properties *oldAssetProperties = oldAsset->GetProperties(mPlatformName);
			MOG_Properties *newAssetProperties = NULL;

			// Check if the asset already existed in the LocalBranch?
			String *revision = MOG_DBSyncedDataAPI::GetLocalAssetVersion(MOG_ControllerSystem::GetComputerName(), mProjectName, mPlatformName, mSyncDirectory, mUserName, assetFilename);
			if (revision->Length)
			{
				// Build the blessedAssetFilename
				MOG_Filename *blessedAssetFilename = new MOG_Filename(MOG_ControllerRepository::GetAssetBlessedVersionPath(assetFilename, revision));
				newAssetProperties = new MOG_Properties(blessedAssetFilename);
				newAssetProperties->SetScope(mPlatformName);
			}

			// Check if this incomming asset performs the out-of-date verification?
			if (oldAssetProperties->OutofdateVerification)
			{
			    // Only continue with a potential warning message if we are the creator of this local asset?
			    if (String::Compare(oldAssetProperties->Creator, MOG_ControllerProject::GetUserName(), true) == 0)
			    {
				    bool bReportNotPostedWarning = false;
    
				    // Get the current revision of this asset in the repository
				    String *currentRepositoryRevision = MOG_ControllerProject::GetAssetCurrentVersionTimeStamp(assetFilename);
    
				    // Check if this asset's BlessedTime is greater than the master's current revision?
				    if (String::Compare(oldAssetProperties->BlessedTime, currentRepositoryRevision, true) > 0)
				    {
					    // Make sure this revision doesn't exist in the repository yet
					    if (!MOG_DBAssetAPI::GetAssetVersionID(assetFilename, oldAssetProperties->BlessedTime))
					    {
						    bReportNotPostedWarning = true;
					    }
				    }
				    if (String::Compare(oldAssetProperties->PreviousRevision, currentRepositoryRevision, true) > 0)
				    {
					    // Make sure this revision doesn't exist in the repository yet
					    if (!MOG_DBAssetAPI::GetAssetVersionID(assetFilename, oldAssetProperties->PreviousRevision))
					    {
						    bReportNotPostedWarning = true;
					    }
				    }
				    // Check if we decide to warn the user?
				    if (bReportNotPostedWarning)
				    {
					    // Make sure this 
					    // Looks like we need to warn the user about them previously blessing this asset but it still hasn't posted yet.
					    String* message = String::Concat(	S"Your previously blessed revision of this asset has not yet posted!  Reverting back to the originally synced revision will prevent you from continuing to work with this asset until your blessed revision is posted.\n",
														    S"ASSET: ", assetFilename->GetAssetFullName(), S"\n\n",
														    S"Are you sure you want to revert this asset in your local workspace?");
					    if (mRemoveOutofdateVerifiedLocalAsset->PromptResponse("Are you sure you want to revert this asset?", message) == MOGPromptResult::No)
					    {
						    // Indicate that we should not delete this asset
						    bDeleteLocalAsset = false;
					    }
				    }
				    else
				    {
					    // Now check if this asset is up-to-date if they leave it alone?
					    if (String::Compare(oldAssetProperties->BlessedTime, currentRepositoryRevision, true) == 0)
					    {
						    // Get the local workspace's synced revision for this asset
							String *localSyncedRevision = MOG_DBSyncedDataAPI::GetLocalAssetVersion(assetFilename, mSyncDirectory, mPlatformName);
						    // Check if the local synced revision is different than the master repository's revision?
					        if (String::Compare(localSyncedRevision, currentRepositoryRevision, true) != 0)
					        {
						        // Ask the user what MOG should do here?
							    String* message = String::Concat(	S"Reverting this asset to the last GetLatest will cause your local file to be reverted and require another GetLatest before you can work with this asset again.\n",
																    S"ASSET: ", assetFilename->GetAssetFullName(), S"\n\n",
																    S"Are you sure you want to revert this asset in your local workspace?");
							    if (mRemoveOutofdateVerifiedLocalAsset->PromptResponse("Are you sure you want to revert this asset?", message) == MOGPromptResult::No)
						        {
							        // Indicate that we should not delete this asset
							        bDeleteLocalAsset = false;
						        }
				            }
			            }
			        }
			    }
			}

			// Check if wwe still want to remove the local asset?
			if (bDeleteLocalAsset)
			{
				// Process the files accounting for some files not needing to be synced
				if (ProcessAssetFiles(oldAssetProperties, newAssetProperties, worker))
				{
					// Check if this asset was packaged?
					if (oldAssetProperties->IsPackagedAsset)
					{
						if (oldAssetProperties->GetApplicablePackages()->Count)
						{
							bool bUnPackageLocalAsset = true;

							// Check if this has resulted in a previous PackageError?
							if (String::Compare(oldAssetProperties->Status, MOG_AssetStatus::GetText(MOG_AssetStatusType::PackageError), false) == 0)
							{
								// Ask the user what MOG should do here?
								String* message = String::Concat(	S"This asset previously encountered a package error.\n",
																	S"ASSET: ", assetFilename->GetAssetFullName(), S"\n\n",
																	S"Would you like to skip the unpackage event and simply delete it now?\n");
								if (MOG_Prompt::PromptResponse("Local Asset Removal", message, MOGPromptButtons::YesNo) == MOGPromptResult::Yes)
								{
									// Skip the unpackage and make sure we delete the asset
									bUnPackageLocalAsset = false;
									bDeleteLocalAsset = true;
								}
							}
							// Check if this asset was just barely added and doesn't need to be unpackaged?
							else if (String::Compare(oldAssetProperties->Status, MOG_AssetStatus::GetText(MOG_AssetStatusType::Unpackaged), false) == 0)
							{
								// Skip the unpackage and make sure we delete the asset
								bUnPackageLocalAsset = false;
								bDeleteLocalAsset = true;
							}
							else
							{
								// Check if there isn't an old revision being restored in the place of this local asset?
								if (!newAssetProperties)
								{
									// Ask the user what MOG should do here?
									String* message = String::Concat(	S"This asset has never been previously blessed.\n",
																		S"ASSET: ", assetFilename->GetAssetFullName(), S"\n\n",
																		S"Would you like this asset removed from the package(s)?\n");
									if (mUnpackageLocalAsset->PromptResponse("Local Asset Removal", message) == MOGPromptResult::Yes)
									{
										// Indicate that should unpackage this asset
										bUnPackageLocalAsset = true;
									}
									else
									{
										// Indicate we can simply remove this w/o unpackaging it
										bUnPackageLocalAsset = false;
									}
								}
							}

							// Check if we determined to unpackage this asset?
							if (bUnPackageLocalAsset)
							{
								// Change the asset's status to 'Unpackage'
								MOG_ControllerInbox::UpdateInboxView(oldAsset, MOG_AssetStatusType::Unpackage);
								bDeleteLocalAsset = false;
								bTryAutoPackage = true;
							}
						}
					}
				}
			}

			// Check if we determined whether or not to delete the local asset?
			if (bDeleteLocalAsset)
			{
				// Make sure we didn't fail?
				if (!bFailed)
				{
					// Delete the asset being removed
					if (!MOG_ControllerInbox::Delete(oldAsset))
					{
						bFailed = true;
					}
				}
			}

			oldAsset->Close();

			// Check if we determined whether we should try to auto package?
			if (bTryAutoPackage)
			{
				// If autopackaging is enabled, package anything that is pending in our local workspace
				AutoPackage();
			}
		}
		else
		{
			// Didnt get asset controller, remove from database anyway so it won't be duplicated in the list
			MOG_ControllerInbox::UpdateInboxView(MOG_AssetStatusType::Deleted, assetFilename, NULL );
		}
	}

	// Check if we failed?
	if (!bFailed)
	{
		return true;
	}

	return false;
}


bool MOG_ControllerSyncData::RepackageAssetInLocalWorkspace(MOG_Filename *assetFilename)
{
	MOG_ControllerAsset *asset = NULL;
	bool bFailed = false;

	// Make sure we have a valid local filename?
	if (assetFilename && 
		assetFilename->IsLocal())
	{
		// FileHamster Integration...
		// Make sure we ignore everything in the hidden MOG directory
		MOG_FileHamsterIntegration::FileHamsterPause(String::Concat(mSyncDirectory, S"\\MOG\\Sync.Info"));

		// Open this asset
		asset = MOG_ControllerAsset::OpenAsset(assetFilename);
		if (asset)
		{
			// Get the properties for the Asset
			MOG_Properties *pProperties = asset->GetProperties(mPlatformName);
			// Check if this asset was packaged?
			if (pProperties->IsPackagedAsset)
			{
				String* textUnpackaged = MOG_AssetStatus::GetText(MOG_AssetStatusType::Unpackaged);

				// Check if this asset is anything other than unpackaged?
				if (String::Compare(pProperties->Status, textUnpackaged, true) != 0)
				{
					// Change the asset's status to 'Repackaged'
					MOG_ControllerInbox::UpdateInboxView(asset, MOG_AssetStatusType::Repackage);
				}
				AutoPackage();
			}

			asset->Close();
		}
	}

	// Check if we failed?
	if (!bFailed)
	{
		return true;
	}

	return false;
}


bool MOG_ControllerSyncData::SetLocalFileAttributes(MOG_Filename* assetFilename, FileAttributes attribute)
{
	// Locate the best matching asset
	MOG_Filename *bestAssetFilename = MOG_ControllerInbox::LocateBestMatchingAsset(assetFilename);
	if (!bestAssetFilename)
	{
		//Let's just use what was passed in because there is nothing better.
		bestAssetFilename = assetFilename;
	}

	return SetLocalFileAttributes(new MOG_Properties(bestAssetFilename), attribute);
}

bool MOG_ControllerSyncData::SetLocalFileAttributes(MOG_Properties* properties, FileAttributes attribute)
{
	bool bFailed = false;

	// Make sure we have a valid set of properties?
	if (properties)
	{
		// Build the list of files associated with the asset
		MOG_Filename *assetFilename = properties->GetAssetFilename();

		// Check if this asset has it's files synced?
		if (properties->SyncFiles)
		{
			String *sourcePath = MOG_ControllerAsset::GetAssetProcessedDirectory(properties, mPlatformName);
			String *relativePath = MOG_Tokens::GetFormattedString(properties->SyncTargetPath, assetFilename, properties->GetApplicableProperties())->Trim(S"\\"->ToCharArray());

			// Obtain the list of files from the asset
			ArrayList *files = GetAssetSyncRelativeFileList(properties);
			if (files && files->Count)
			{
				for(int i = 0; i < files->Count; i++)
				{
					String* assetFile = __try_cast<String*>(files->Item[i]);
					String* originalFile = Path::Combine(sourcePath, assetFile);
					String* relativeFile = Path::Combine(relativePath, assetFile);
					String* localFile = Path::Combine(mSyncDirectory, relativeFile);

					if (!DosUtils::SetAttributes(localFile, attribute))
					{
						bFailed = true;
					}
				}
			}
		}
	}

	if (!bFailed)
	{
		return true;
	}
	return false;
}


bool MOG_ControllerSyncData::ProcessAssetFiles(MOG_Properties *oldProperties, MOG_Properties *newProperties, BackgroundWorker* worker)
{
	MOG_Filename* assetFilename = NULL;
	MOG_Filename* oldAssetFilename = NULL;
	MOG_Filename* newAssetFilename = NULL;
	HybridDictionary* oldFilesToRemove = new HybridDictionary();
	HybridDictionary* newFilesToCopy = new HybridDictionary();
	HybridDictionary* newFilesAlreadyCopied = new HybridDictionary();
	HybridDictionary* allFiles = new HybridDictionary();
	bool bClearReadOnlyFlag = true;

	// Make sure we have something to do?
	if (oldProperties || newProperties)
	{
		// Check if we received something for the old asset?
		if (oldProperties)
		{
			// Build the list of files associated with the old asset
			oldAssetFilename = oldProperties->GetAssetFilename();
			// Check if this asset has it's files synced?
			if (oldProperties->SyncFiles)
			{
				String *oldSourcePath = MOG_ControllerAsset::GetAssetProcessedDirectory(oldProperties, mPlatformName);
				String *oldRelativePath = MOG_Tokens::GetFormattedString(oldProperties->SyncTargetPath, oldAssetFilename, oldProperties->GetApplicableProperties())->Trim(S"\\"->ToCharArray());

				// Obtain the list of files from the asset
//				ArrayList *files = GetAssetSyncRelativeFileList(oldProperties);
				String *assetFiles[] = MOG_ControllerAsset::GetAssetPlatformFiles(oldProperties, mPlatformName, false);
				if (assetFiles)
				{
					for(int i = 0; i < assetFiles->Count; i++)
					{
						String* oldAssetFile = assetFiles[i];
						String* oldSourceFile = Path::Combine(oldSourcePath, oldAssetFile);
						String* oldRelativeFile = Path::Combine(oldRelativePath, oldAssetFile);
						String* oldTargetFile = Path::Combine(mSyncDirectory, oldRelativeFile);

						oldFilesToRemove->Item[oldTargetFile] = oldSourceFile;
						allFiles->Item[oldTargetFile] = oldTargetFile;
					}
				}
			}
		}

		// Check if we received something for the new asset?
		if (newProperties)
		{
			// Build the list of files associated with the new asset
			newAssetFilename = newProperties->GetAssetFilename();
			// Check if this asset has it's files synced?
			if (newProperties->SyncFiles)
			{
				String *newSourcePath = MOG_ControllerAsset::GetAssetProcessedDirectory(newProperties, mPlatformName);
				String *newRelativePath = MOG_Tokens::GetFormattedString(newProperties->SyncTargetPath, newAssetFilename, newProperties->GetApplicableProperties())->Trim(S"\\"->ToCharArray());

				// Obtain the list of files from the asset
//				ArrayList *files = GetAssetSyncRelativeFileList(newProperties);
				String *assetFiles[] = MOG_ControllerAsset::GetAssetPlatformFiles(newProperties, mPlatformName, false);
				if (assetFiles)
				{
					for(int i = 0; i < assetFiles->Count; i++)
					{
						String* newAssetFile = assetFiles[i];
						String* newSourceFile = Path::Combine(newSourcePath, newAssetFile);
						String* newRelativeFile = Path::Combine(newRelativePath, newAssetFile);
						String* newTargetFile = Path::Combine(mSyncDirectory, newRelativeFile);

						newFilesToCopy->Item[newTargetFile] = newSourceFile;
						allFiles->Item[newTargetFile] = newTargetFile;
					}
				}
			}

			// Check if this asset wants to be synced as read only?
			if (newProperties->SyncAsReadOnly)
			{
				// Make sure the asset isn't locked by me before we decide to leave it ReadOnly
				if (MOG_ControllerProject::IsLockedByMe(newAssetFilename->GetAssetFullName()))
				{
					// Clear the ReadOnly because we have this asset locked
					bClearReadOnlyFlag = true;
				}
				else
				{
					// Check the user's privilege
					MOG_Privileges* privileges = MOG_ControllerProject::GetPrivileges();
					if (!privileges->GetUserPrivilege(MOG_ControllerProject::GetUserName(), MOG_PRIVILEGE::IgnoreSyncAsReadOnly))
					{
						// Leave the file ReadOnly
						bClearReadOnlyFlag = false;
					}
				}
			}
		}

		// Scan the list of allFiles and see which ones can be reduced?
		IDictionaryEnumerator* allFileEnumerator = allFiles->GetEnumerator();
		while ( allFileEnumerator->MoveNext() )
		{
			String* targetFile = dynamic_cast<String*>(allFileEnumerator->Value);

			bool bCheckFileTimestamps = true;
			FileInfo* targetFileInfo = NULL;
			FileInfo* newSourceFileInfo = NULL;

			// Check if this relativeFile exists in both lists?
			String *oldSourceFile = dynamic_cast<String*>(oldFilesToRemove->Item[targetFile]);
			if (oldSourceFile)
			{
				// Obtain FileInfo for targetFile and newSourceFile
				targetFileInfo = new FileInfo(targetFile);
				// Check if the local target file is missing?
				if (!targetFileInfo->Exists)
				{
					// The local target file doesn't exist so let's remove it from the oldFilesToRemove now
					oldFilesToRemove->Remove(targetFile);

					// Indicate we shouldn't bother to check the file's timestamps
					bCheckFileTimestamps = false;
				}
			}
			else
			{
				// Indicate we shouldn't bother to check the file's timestamps
				bCheckFileTimestamps = false;
			}

			String *newSourceFile = dynamic_cast<String*>(newFilesToCopy->Item[targetFile]);
			if (newSourceFile)
			{
				// Obtain FileInfo for targetFile and newSourceFile
				newSourceFileInfo = new FileInfo(newSourceFile);
				// Check if the source file is missing?
				if (!newSourceFileInfo->Exists)
				{
					// Wow, the asset is missing a file and this should be reported
					// Remove this missing file
					newFilesToCopy->Remove(targetFile);
					String* newRelativeFile = targetFile->Substring(mSyncDirectory->Length)->Trim(S"\\"->ToCharArray());
					mSyncLog->PutString( S"Files.Missing", newRelativeFile, newAssetFilename->GetAssetFullName() );
					mSyncLog->PutString( S"Assets.Error", newAssetFilename->GetAssetFullName(), S"" );

					// Indicate we shouldn't bother to check the file's timestamps
					bCheckFileTimestamps = false;
				}
			}
			else
			{
				// Indicate we shouldn't bother to check the file's timestamps
				bCheckFileTimestamps = false;
			}

			// Check if should check the file's timestamps
			if (bCheckFileTimestamps)
			{
				// Check if the targetFile is already up-to-date with the newSourceFile?
				if (newSourceFileInfo->LastWriteTime.ToFileTime() == targetFileInfo->LastWriteTime.ToFileTime())
				{
					// No need to remove a file that is already up-to-date
					oldFilesToRemove->Remove(targetFile);
					// Move this up-to-date file to newFilesAlreadyCopied
					newFilesToCopy->Remove(targetFile);
					newFilesAlreadyCopied->Item[targetFile] = newSourceFileInfo;

					// Check if the targetFile's attributes needs to be changed?
					if (bClearReadOnlyFlag == targetFileInfo->IsReadOnly)
					{
						if (bClearReadOnlyFlag)
						{
							targetFileInfo->Attributes = FileAttributes::Normal;
						}
						else
						{
							targetFileInfo->Attributes = FileAttributes::ReadOnly;
						}
					}
				}
			}
		}

		// Determine the best action name for this event
		String* action = "Copied";
		if (newAssetFilename && oldAssetFilename)
		{
			assetFilename = newAssetFilename;

			// Check if the newAssetFilename's revision is older?
			if (String::Compare(newAssetFilename->GetVersionTimeStamp(), oldAssetFilename->GetVersionTimeStamp(), true) < 0)
			{
				// Use 'Reverted' anytime the new revision is older
				action = S"Reverted";
			}
			// Check if the oldAssetFilenam and newAssetFilename are the same asset?  or
			// Check if this is an asset being removed from the local tray and being restored to the blessed revision?
			else if (String::Compare(newAssetFilename->GetEncodedFilename(), oldAssetFilename->GetEncodedFilename(), true) == 0)
			{
				// Check if these identical assets are in the local updated tray?
				if (newAssetFilename->IsLocal())
				{
					// Use 'Restamped' to indicate locally updated files were restmaped over the last sync
					action = S"Restamped";
				}
				else
				{
					// Use 'Restored' to indicate the files are being returned to their desired state
					action = S"Restored";
				}
			}
			else if (oldAssetFilename->IsLocal() && newAssetFilename->IsWithinRepository())
			{
				// Use 'Restored' to indicate the files are being returned to their desired state
				action = S"Restored";
			}
		}
		else if (oldAssetFilename)
		{
			assetFilename = oldAssetFilename;

			// Use 'Deleted' when all we have is an oldAssetFilename
			action = S"Removed";
		}
		else if (newAssetFilename)
		{
			assetFilename = newAssetFilename;

			// Use 'New' when all we have is a newAssetFilename
			action = S"New";
		}
		// Record this asset in our log
		mSyncLog->PutString( String::Concat(S"Assets.", action), assetFilename->GetAssetFullName(), S"" );

		// Proceed to remove the remaining oldFilesToRemove
		IDictionaryEnumerator* oldFileEnumerator = oldFilesToRemove->GetEnumerator();
		while ( oldFileEnumerator->MoveNext() )
		{
			String* oldTargetFile = dynamic_cast<String*>(oldFileEnumerator->Key);
			String* oldSourceFile = dynamic_cast<String*>(oldFileEnumerator->Value);
			String* oldRelativeFile = oldTargetFile->Substring(mSyncDirectory->Length)->Trim(S"\\"->ToCharArray());

			// FileHamster Integration...
			// Make sure we know about this directory so it can be paused if needed
			MOG_FileHamsterIntegration::FileHamsterPause(oldTargetFile);

			// Attempt to remove this file
			bool bRetry = false;
			do
			{
				try
				{
					// Proceed to remove the oldTargetFile comparing it to the oldSourceFile
					if (RemoveLocalFile(oldTargetFile, oldSourceFile, oldProperties->ShowLocalModifiedWarning))
					{
						// Check if this file is really being removed by checking if it isn't in the newFilesToCopy list?
						if (!newFilesToCopy->Contains(oldTargetFile))
						{
							mSyncLog->PutString( S"Files.Removed", oldRelativeFile, oldAssetFilename->GetAssetFullName() );
						}
					}
					else
					{
						mSyncLog->PutString( S"Files.Skipped", oldRelativeFile, oldAssetFilename->GetAssetFullName() );
						mSyncLog->PutString( S"Assets.Skipped", oldAssetFilename->GetAssetFullName(), S"" );
						return false;
					}
				}
				catch(Exception *e)
				{
					mSyncLog->PutString( S"Files.RemoveError", oldRelativeFile, e->Message );
					mSyncLog->PutString( S"Assets.RemoveError", oldAssetFilename->GetAssetFullName(), e->Message );

					String *title = S"Sync File Error";
					// Inform the user we failed to match an asset
					String *message = String::Concat(	S"Could not access old file:\n",
														S"     ", Path::GetDirectoryName(oldRelativeFile), S"\\ \n",
														S"     ", Path::GetFileName(oldRelativeFile), S"\n\n",
														S"Make sure this file isn't being used by another process.");
					MOGPromptResult rc = MOG_Prompt::PromptResponse(title, message, Environment::StackTrace, MOGPromptButtons::AbortRetryIgnore);
					switch (rc)
					{
						case MOGPromptResult::Abort:	//Don't retry, stop checking things out
							return false;
							break;
						case MOGPromptResult::Retry:	//Retry the current asset
							bRetry = true;
							break;
						case MOGPromptResult::Ignore:	//Don't retry the current asset, but continue checking out the other assets
							bRetry = false;
							break;
					}
				}
			} while (bRetry);
		}

		// Proceed to copy the remaining newFilesToCopy
		IDictionaryEnumerator* newFileEnumerator = newFilesToCopy->GetEnumerator();
		while ( newFileEnumerator->MoveNext() )
		{
			String* newTargetFile = dynamic_cast<String*>(newFileEnumerator->Key);
			String* newSourceFile = dynamic_cast<String*>(newFileEnumerator->Value);
			String* newRelativeFile = newTargetFile->Substring(mSyncDirectory->Length)->Trim(S"\\"->ToCharArray());

			// FileHamster Integration...
			// Make sure we know about this directory so it can be paused if needed
			MOG_FileHamsterIntegration::FileHamsterPause(newTargetFile);

			// Attempt to remove this file
			bool bRetry = false;
			do
			{
				try
				{
					// Proceed to remove the newTargetFile comparing it to the newSourceFile
					if (CopyLocalFile(newSourceFile, newTargetFile, bClearReadOnlyFlag, worker))
					{
						mSyncLog->PutString( String::Concat(S"Files.", action), newRelativeFile, newAssetFilename->GetAssetFullName() );
					}
					else
					{
						mSyncLog->PutString( S"Files.Skipped", newRelativeFile, newAssetFilename->GetAssetFullName() );
						mSyncLog->PutString( S"Assets.Skipped", newAssetFilename->GetAssetFullName(), S"" );
						return false;
					}
				}
				catch(Exception *e)
				{
					mSyncLog->PutString( S"Files.RemoveError", newRelativeFile, e->Message );
					mSyncLog->PutString( S"Assets.RemoveError", newAssetFilename->GetAssetFullName(), e->Message );

					String *title = S"Sync File Error";
					// Inform the user we failed to match an asset
					String *message = String::Concat(	S"Could not remove new file:\n",
														S"     ", Path::GetDirectoryName(newRelativeFile), S"\\ \n",
														S"     ", Path::GetFileName(newRelativeFile), S"\n\n",
														S"Make sure this file isn't being used by another process.");
					MOGPromptResult rc = MOG_Prompt::PromptResponse(title, message, Environment::StackTrace, MOGPromptButtons::AbortRetryIgnore);
					switch (rc)
					{
						case MOGPromptResult::Abort:	//Don't retry, stop checking things out
							return false;
							break;
						case MOGPromptResult::Retry:	//Retry the current asset
							bRetry = true;
							break;
						case MOGPromptResult::Ignore:	//Don't retry the current asset, but continue checking out the other assets
							bRetry = false;
							break;
					}
				}
			} while (bRetry);
		}

		// Inform the user about newFilesAlreadyCopied that were already up-to-date
		action = S"Up-to-date";
		IDictionaryEnumerator* newFileCurrentEnumerator = newFilesAlreadyCopied->GetEnumerator();
		while ( newFileCurrentEnumerator->MoveNext() )
		{
			String* newTargetFile = dynamic_cast<String*>(newFileCurrentEnumerator->Key);
			String* newRelativeFile = newTargetFile->Substring(mSyncDirectory->Length)->Trim(S"\\"->ToCharArray());

			// Make sure this file wasn't already listed in another section?
			if (!mSyncLog->KeyExist( S"Files.New", newRelativeFile) &&
				!mSyncLog->KeyExist( S"Files.Copied", newRelativeFile) &&
				!mSyncLog->KeyExist( S"Files.Reverted", newRelativeFile) &&
				!mSyncLog->KeyExist( S"Files.Restamped", newRelativeFile) &&
				!mSyncLog->KeyExist( S"Files.Restored", newRelativeFile) )
			{
				// Looks like we can inform the user about this asset that was already up-to-date
				mSyncLog->PutString( String::Concat(S"Files.", action), newRelativeFile, newAssetFilename->GetAssetFullName() );
			}
		}
	}

	return true;
}


bool MOG_ControllerSyncData::LocalFileVerification(MOG_Properties *properties)
{
	// Make sure we have a valid set of properties?
	if (properties)
	{
		// Build the list of files associated with the asset
		MOG_Filename *assetFilename = properties->GetAssetFilename();
		// Check if this asset has it's files synced?
		if (properties->SyncFiles)
		{
			String *sourcePath = MOG_ControllerAsset::GetAssetProcessedDirectory(properties, mPlatformName);
			String *relativePath = MOG_Tokens::GetFormattedString(properties->SyncTargetPath, assetFilename, properties->GetApplicableProperties())->Trim(S"\\"->ToCharArray());

			// Obtain the list of files from the asset
			ArrayList *files = GetAssetSyncRelativeFileList(properties);
			if (files && files->Count)
			{
				for(int i = 0; i < files->Count; i++)
				{
					String* assetFile = __try_cast<String*>(files->Item[i]);
					String* originalFile = Path::Combine(sourcePath, assetFile);
					String* relativeFile = Path::Combine(relativePath, assetFile);
					String* localFile = Path::Combine(mSyncDirectory, relativeFile);

					// Check if these files have been modified
					if (SyncAssetFiles_HasBeenModified(localFile, originalFile))
					{
						// Inform the user these file are not in sync
						return false;
					}
				}
			}
		}
	}

	return true;
}


bool MOG_ControllerSyncData::SyncAssetFiles_HasBeenModified(String *localFile, String *originalFile)
{
	bool bHasBeenModified = false;

	// Get the time information for both files
	FileInfo *originalInfo = new FileInfo(originalFile);
	FileInfo *localInfo = new FileInfo(localFile);

	// Make sure both files exist
	if (originalInfo->Exists && localInfo->Exists)
	{
		// Has the local file been modified?  (Must Use ToFileTime() because sometimes LastWriteTime comparisons can fail)
		if (originalInfo->LastWriteTime.ToFileTime() != localInfo->LastWriteTime.ToFileTime())
		{
			bHasBeenModified = true;
		}
	}

	return bHasBeenModified;
}


bool MOG_ControllerSyncData::SyncAssetFiles_CanOverwriteFile(String *targetFile, String *originalFile, bool showLocalModifiedWarning)
{
	bool bCanOverwrite = true;

	// Check if we should prompt the user for verification?
	if (MOG_ControllerSystem::isSuppressMessageBox() == false &&
		showLocalModifiedWarning == true)
	{
		String *relativeFile = targetFile;
		String *testPath = String::Concat(mSyncDirectory, S"\\");
		if (targetFile->StartsWith(testPath))
		{
			relativeFile = targetFile->Substring(testPath->Length);
		}

		String* message = String::Concat(	S"Your local file has been modified.\n",
											S"          ", Path::GetDirectoryName(relativeFile), S"\\\n",
											S"          ", Path::GetFileName(relativeFile), S"\n\n",
											S"Do you want to continue and overwrite your changes?");
		MOGPromptResult shouldOverwrite = mGetLatest_OverwriteFile->PromptResponse("Local File Modified", message);
		if (shouldOverwrite == MOGPromptResult::Yes ||
			shouldOverwrite == MOGPromptResult::OK)
		{
			MOG_Filename* assetFilename = new MOG_Filename(originalFile);
			if (MOG_ControllerProject::IsLockedByMe(assetFilename->GetAssetFullName()))
			{
				// Prompt the user one more time about this locked file
				String* message = String::Concat(	S"WARNING - This local modified file is locked and may be important!.\n",
													S"          ", Path::GetDirectoryName(relativeFile), S"\\\n",
													S"          ", Path::GetFileName(relativeFile), S"\n\n",
													S"Are you REALLY sure you want to continue and overwrite your changes?");
				// The ask user for permission to overwrite the file?
				if (MOG_Prompt::PromptResponse("Local File Modified", message, MOGPromptButtons::YesNo) == MOGPromptResult::No)
				{
					bCanOverwrite = false;
				}
			}
		}
		else
		{
			// Indicate that we can't overwrite the modified file
			bCanOverwrite = false;
		}
	}

	return bCanOverwrite;
}


bool MOG_ControllerSyncData::IsEqual(MOG_ControllerSyncData *right)
{
	if (right)
	{
		// If this is the same SyncData handle
		return ((String::Compare(GetSyncDirectory(), right->GetSyncDirectory(), true) == 0 &&
				String::Compare(GetPlatformName(), right->GetPlatformName(), true) == 0 &&
				String::Compare(GetProjectName(), right->GetProjectName(), true) == 0 &&
				String::Compare(GetBranchName(), right->GetBranchName(), true) == 0 ) == true);	
	}

	return false;
}


String *MOG_ControllerSyncData::GetSyncDataDirectoryPath(String *relativeSyncDataFile)
{
	String *completeSyncDataFile = S"";

	// Check if the relativeSyncDataFile already starts with a '\\'?
	if (relativeSyncDataFile->StartsWith("\\"))
	{
		completeSyncDataFile = String::Concat(mSyncDirectory, relativeSyncDataFile);
	}
	else
	{
		completeSyncDataFile = String::Concat(mSyncDirectory, S"\\", relativeSyncDataFile);
	}

	return completeSyncDataFile;
}


String *MOG_ControllerSyncData::GetSourceDirectoryPath(MOG_ControllerAsset *asset, String *relativeFile)
{
	// Check if there is a SyncTargetPath specified?
	String *syncTargetPath = MOG_Tokens::GetFormattedString(asset->GetProperties(mPlatformName)->SyncTargetPath, asset->GetAssetFilename(), asset->GetProperties(mPlatformName)->GetApplicableProperties())->Trim(S"\\"->ToCharArray());
	if (syncTargetPath->Length)
	{
		// Extract the local project's sync directory plus the '\' from the relativeSyncDataFile
		relativeFile = relativeFile->Substring(syncTargetPath->Length + 1);
	}

	return String::Concat(MOG_ControllerAsset::GetAssetProcessedDirectory(asset->GetProperties(), mPlatformName), S"\\", relativeFile);
}

bool MOG_ControllerSyncData::SyncRepositoryData(String *classification, String *exclusionList, String *inclusionList, bool bCheckMissingModifiedFiles, MOG_LocalSyncInfo *syncInfo)
{
	bool bResult = false;

	if (MOG_ControllerProject::IsValidClassification(classification))
	{
		// Perform the actual sync
		bResult = StartSyncProcess(classification, "", exclusionList, inclusionList, bCheckMissingModifiedFiles, syncInfo);
	}
	else
	{
		MOG_Report::ReportMessage(	"Sync Error", 
									String::Concat("The classification '", classification,"' does not exist"),
									Environment::StackTrace, MOG_ALERT_LEVEL::ERROR); 
	}

	return bResult;
}

bool MOG_ControllerSyncData::SyncDataFromSyncLocation(String *syncLocation, String *exclusionList, String *inclusionList, bool bCheckMissingModifiedFiles, MOG_LocalSyncInfo *syncInfo)
{
	// Remove directory root from Sync location if its in there
	if (syncLocation->StartsWith(mSyncDirectory, true, Globalization::CultureInfo::CurrentCulture))
	{
		// Remove our sync root
		syncLocation = syncLocation->Substring(mSyncDirectory->Length)->TrimStart(S"\\"->ToCharArray());

		return StartSyncProcess("", syncLocation, exclusionList, inclusionList, bCheckMissingModifiedFiles, syncInfo);
	}

	return false;
}

bool MOG_ControllerSyncData::StartSyncProcess(String *classification, String *syncLocation, String *exclusionList, String *inclusionList, bool bCheckMissingModifiedFiles, MOG_LocalSyncInfo *syncInfo)
{
	// Wait while the Editor is running
	if (MOG_ControllerSystem::WaitWhileEditorRunning(mSyncDirectory))
	{
		// Build out list of arguments
		List<Object*>* args = new List<Object*>();
		args->Add(syncInfo);
		args->Add(classification);
		args->Add(syncLocation);
		args->Add(exclusionList);
		args->Add(inclusionList);
		args->Add(__box(bCheckMissingModifiedFiles));

		ProgressDialog* progress = new DelayedProgressDialog("Getting latest data", "Please wait while MOG performs a GetLatest.", new DoWorkEventHandler(this, &MOG_ControllerSyncData::SyncRepositoryData_Worker), args, true);
		
		// KLK - We think that setting this window to hidden is causing some serious delays,
		// so as a compromise we just set the window to minimized
		bool console = MOG_Prompt::IsMode(MOG_PROMPT_CONSOLE);
		if (console)
		{
			// Set Hidden based on the Prompt mode to prevent slaves from showing this progress dialog
			progress->Hidden = MOG_Prompt::IsMode(MOG_PROMPT_SILENT);
//			progress->WindowState = FormWindowState::Minimized;
		}

		progress->ShowDialog();
		if (progress->DialogResult != DialogResult::Cancel)
		{
			return true;
		}
	}

	return false;
}

void MOG_ControllerSyncData::SyncRepositoryData_Worker(Object* sender, DoWorkEventArgs* e)
{
	BackgroundWorker* worker = dynamic_cast<BackgroundWorker*>(sender);
	
	List<Object*>* args = dynamic_cast<List<Object*>*>(e->Argument);
	MOG_LocalSyncInfo* syncInfo = dynamic_cast<MOG_LocalSyncInfo*>(args->Item[0]);
	String* classification = dynamic_cast<String*>(args->Item[1]);
	String* syncLocation = dynamic_cast<String*>(args->Item[2]);
	String* exclusionList = dynamic_cast<String*>(args->Item[3]);
	String* inclusionList = dynamic_cast<String*>(args->Item[4]);
	bool bCheckMissingModifiedFiles = *dynamic_cast<__box bool*>(args->Item[5]);
	
	String *computerName = MOG_ControllerSystem::GetComputerName();

	// Begin this GetLatest
	ModifyLocalWorkspace_Begin();
	
	String *syncInfoFilename = String::Concat(mSyncDirectory, S"\\MOG\\Sync.Info");

	// FileHamster Integration...
	// Make sure we ignore everything in the hidden MOG directory
	MOG_FileHamsterIntegration::FileHamsterPause(syncInfoFilename);

	// Check if we are missing the Sync.Info file?
	if (!DosUtils::FileExistFast(syncInfoFilename))
	{
		// Remove this sync location from the Database so it will be fully updated this time
		if (MOG_DBSyncedDataAPI::RemoveSyncedLocation(computerName, mProjectName, mPlatformName, mSyncDirectory, mUserName))
		{
		}
	}

	// Always make sure that this mSyncDirectory is a recognized sync location
	if (MOG_DBSyncedDataAPI::AddSyncedLocation(computerName, mProjectName, mPlatformName, mSyncDirectory, S"", mName, mUserName))
	{
	}

	// Always validate the workspace
	ValidateLocalWorkspace();

	// Create our filterProperties
	ArrayList *filterProperties = new ArrayList();

	//Build a MOG_LocalSyncInfo object so we can get the list of items to add, remove and update?
	if (!syncInfo)
	{
		syncInfo = new MOG_LocalSyncInfo(computerName, mProjectName, mPlatformName, mSyncDirectory, mBranchName, mUserName, classification, syncLocation);
	}
	// As an optimization, some initialization queries were moved out of the constructor...calling this here will trigger them to be completed
	syncInfo->IsNonSyncingAsset(NULL);

	// Figure out how many assets in total we plan to work with
	int totalAssetCount  =	syncInfo->GetRemoveAssetList()->Count + 
							syncInfo->GetUpdateAssetList()->Count + 
							syncInfo->GetAddAssetList()->Count;
	int assetCount = 0;

	//Assign the remove list to the array list of items to be removed
	ArrayList *assetsThatNeedToBeRemoved = syncInfo->GetRemoveAssetList();
	for (int i = 0; i < assetsThatNeedToBeRemoved->Count; i++)
	{
		MOG_Filename *removeMe = __try_cast<MOG_Filename*>(assetsThatNeedToBeRemoved->Item[i]);
		bool removeSuccess = false;

		// Report progress
		String *message = String::Concat(	S"Removing:\n",
											S"     ", removeMe->GetAssetClassification(), S"\n",
											S"     ", removeMe->GetAssetLabel());
		worker->ReportProgress((assetCount++ * 100) / totalAssetCount, message);

		// Make sure we haven't been canceled
		if (!worker->CancellationPending)
		{
			// Check if this asset is not included?
			if(!syncInfo->IsAssetIncluded(removeMe, exclusionList, inclusionList))
			{
				// Excluded items should be left the same so they will be listed again the next time
				removeSuccess = false;
			}
			// Check if this asset is listed in the local data?
			else if (IsAssetListedInLocalWorkspace(removeMe))
			{
				// Skipping this local workspace asset will save us from having to restamp files for no reason
				// However, act like we did copy it so that the proper link will be there when the local asset gets removed
				mSyncLog->PutString( S"Assets.Omitted", removeMe->GetAssetFullName(), removeMe->GetVersionTimeStamp() );
				removeSuccess = true;
			}
			else
			{
				// Check if this is a non-syncing asset?
				if (syncInfo->IsNonSyncingAsset(removeMe))
				{
					// Update the log
					mSyncLog->PutString( S"Assets.Removed", removeMe->GetAssetFullName(), S"" );
					removeSuccess = true;
				}
				else
				{
					// Check if this asset being removed already exists in our list of local assets?
					MOG_Filename *assetFilename = syncInfo->GetAssetInLocalUpdatedTray(removeMe);
					if (!assetFilename)
					{
						// Resort to using the blessed version of this asset
						assetFilename = new MOG_Filename(MOG_ControllerRepository::GetAssetBlessedVersionPath(removeMe, removeMe->GetVersionTimeStamp()));
					}
					MOG_Properties *assetProperties = new MOG_Properties(assetFilename);
					assetProperties->SetScope(mPlatformName);
					removeSuccess = true;

					// Double check the asset to make sure we actually want to sync files?
					// This can happen when a nonsyncing asset has been removed and therefore not included in the regular IsNonSyncingAsset check
					if (assetProperties->SyncFiles == true)
					{
						if (ProcessAssetFiles(assetProperties, NULL, worker))
						{
							removeSuccess = true;
						}
					}
					else
					{
						// Add this nonsyncing asset to the 'Assets.Deleted' section
						mSyncLog->PutString( S"Assets.Removed", removeMe->GetAssetFullName(), S"" );
						removeSuccess = true;
					}
				}
			}
		}
		else
		{
			mSyncLog->PutString( S"Assets.Canceled", removeMe->GetAssetFullName(), S"" );
		}

		if( removeSuccess )
		{
			MOG_DBSyncedDataAPI::RemoveSyncedLocationLink(removeMe, computerName, mProjectName, mPlatformName, mSyncDirectory, mUserName);
		}
	}

	//assign the newly created syncInfo Update list to the list of assets to be updated.
	ArrayList *assetsWithNewRevisions = syncInfo->GetUpdateAssetList();
	// remove old versions
	for( int i = 0; i < assetsWithNewRevisions->Count; i++ )
	{
		MOG_DBUpdatePair *pair = __try_cast<MOG_DBUpdatePair*>(assetsWithNewRevisions->Item[i]);
		MOG_Filename *removeMe = pair->mOldVersion;
		MOG_Filename *copyMe = pair->mNewVersion;
		bool removeSuccess = false;
		bool copySuccess = true;

		// Show the name of the asset and the progress
		String *message = String::Concat(	S"Syncing:\n",
											S"     ", copyMe->GetAssetClassification(), S"\n",
											S"     ", copyMe->GetAssetLabel());
		worker->ReportProgress((assetCount++ * 100) / totalAssetCount, message);

		// Make sure we haven't been canceled
		if (!worker->CancellationPending)
		{
			// Check if this asset is not included?
			if(!syncInfo->IsAssetIncluded(removeMe, exclusionList, inclusionList))
			{
				// Excluded items should be left the same so they will be listed again the next time
				removeSuccess = false;
				copySuccess = false;
			}
			// Check if this asset is listed in the local data?
			else if (IsAssetListedInLocalWorkspace(copyMe))
			{
				// Skipping this local workspace asset will save us from having to restamp files for no reason
				// However, act like we did copy it so that the proper link will be there when the local asset gets removed
				mSyncLog->PutString( S"Assets.Omitted", copyMe->GetAssetFullName(), copyMe->GetVersionTimeStamp() );
				removeSuccess = true;
				copySuccess = true;
			}
			else
			{
				MOG_Properties *oldAssetProperties = NULL;
				MOG_Properties *newAssetProperties = NULL;

				// Check if this is a non-syncing asset?
				if (syncInfo->IsNonSyncingAsset(removeMe))
				{
					removeSuccess = true;
				}
				else
				{
					// Check if this asset being removed already exists in our list of local assets?
					MOG_Filename *oldAssetFilename = syncInfo->GetAssetInLocalUpdatedTray(removeMe);
					if (!oldAssetFilename)
					{
						// Resort to using the blessed version of this asset
						oldAssetFilename = new MOG_Filename(MOG_ControllerRepository::GetAssetBlessedVersionPath(removeMe, removeMe->GetVersionTimeStamp()));
					}
					oldAssetProperties = new MOG_Properties(oldAssetFilename);
					oldAssetProperties->SetScope(mPlatformName);
				}

				// Check if this is a non-syncing asset?
				if (syncInfo->IsNonSyncingAsset(copyMe))
				{
					// Update the log
					mSyncLog->PutString( S"Assets.Copied", copyMe->GetAssetFullName(), S"" );
					copySuccess = true;
				}
				else
				{
					MOG_Filename *blessedAsset = new MOG_Filename(MOG_ControllerRepository::GetAssetBlessedVersionPath(copyMe, copyMe->GetVersionTimeStamp()));
					newAssetProperties = new MOG_Properties(blessedAsset);
					newAssetProperties->SetScope(mPlatformName);
				}

				// Check if it should be removed or copied?
				if (removeSuccess || copySuccess)
				{
					// Process the files accounting for some files not needing to be synced
					if (!ProcessAssetFiles(oldAssetProperties, newAssetProperties, worker))
					{
						removeSuccess = false;
						copySuccess = false;
					}
				}
			}

			if( removeSuccess )
			{
				MOG_DBSyncedDataAPI::RemoveSyncedLocationLink(removeMe, computerName, mProjectName, mPlatformName, mSyncDirectory, mUserName);
			}

			if( copySuccess )
			{
				MOG_DBSyncedDataAPI::AddSyncedLocationLink(copyMe, computerName, mProjectName, mPlatformName, mSyncDirectory, mUserName);
			}
		}
		else
		{
			mSyncLog->PutString( S"Assets.Canceled", copyMe->GetAssetFullName(), S"" );
		}
	}

	ArrayList *newAssetsThatNeedCopying = syncInfo->GetAddAssetList();
	for( int i = 0; i < newAssetsThatNeedCopying->Count; i++ )
	{
		// Open the blessed revision of this asset
		MOG_Filename *copyMe = __try_cast<MOG_Filename*>(newAssetsThatNeedCopying->Item[i]);
		bool copySuccess = false;

		// Show the name of the asset and the progress
		String *message = String::Concat(	S"Syncing:\n",
											S"     ", copyMe->GetAssetClassification(), S"\n",
											S"     ", copyMe->GetAssetLabel());
		worker->ReportProgress((assetCount++ * 100) / totalAssetCount, message);

		// Make sure we haven't been canceled
		if (!worker->CancellationPending)
		{
			// Check if this asset is not included?
			if(!syncInfo->IsAssetIncluded(copyMe, exclusionList, inclusionList))
			{
				// Excluded items should be left the same so they will be listed again the next time
				copySuccess = false;
			}
			// Check if this asset is listed in the local data?
			else if (IsAssetListedInLocalWorkspace(copyMe))
			{
				// Skipping this local workspace asset will save us from having to restamp files for no reason
				// However, act like we did copy it so that the proper link will be there when the local asset gets removed
				mSyncLog->PutString( S"Assets.Omitted", copyMe->GetAssetFullName(), copyMe->GetVersionTimeStamp() );
				copySuccess = true;
			}
			else
			{
				// Check if this is a non-syncing asset?
				if (syncInfo->IsNonSyncingAsset(copyMe))
				{
					// Update the log
					mSyncLog->PutString( S"Assets.New", copyMe->GetAssetFullName(), S"" );
					copySuccess = true;
				}
				else
				{
					MOG_Filename *blessedAsset = new MOG_Filename(MOG_ControllerRepository::GetAssetBlessedVersionPath(copyMe, copyMe->GetVersionTimeStamp()));
					MOG_Properties *assetProperties = new MOG_Properties(blessedAsset);
					assetProperties->SetScope(mPlatformName);

					// Attempt to copy this asset's files?
					if (ProcessAssetFiles(NULL, assetProperties, worker))
					{
						copySuccess = true;
					}
				}
			}

			if (copySuccess)
			{
				MOG_DBSyncedDataAPI::AddSyncedLocationLink(copyMe, computerName, mProjectName, mPlatformName, mSyncDirectory, mUserName);
			}
		}
		else
		{
			mSyncLog->PutString( S"Assets.Canceled", copyMe->GetAssetFullName(), S"" );
		}
	}
	
	// Make sure we haven't already canceled?
	if (!worker->CancellationPending)
	{
		// Check if we need to restamp missing files?
		if (bCheckMissingModifiedFiles)
		{
			SyncRepositoryData_CheckForMissingModifiedFiles(worker, exclusionList, inclusionList, syncInfo);
		}
	}

	// Make sure we haven't already canceled?
	if (!worker->CancellationPending)
	{
		// Only bother to do a full restamping if the user is wanting us to check modified/missing files
		bool bPerformFullRestamp = bCheckMissingModifiedFiles;
		// Restamp all the local assets
		RestampLocalAssets(worker, bPerformFullRestamp);
	}

	// End this GetLatest
	ModifyLocalWorkspace_Complete();
}

bool MOG_ControllerSyncData::SyncRepositoryData_CheckForMissingModifiedFiles(BackgroundWorker* worker, String *exclusionList, String *inclusionList, MOG_LocalSyncInfo *syncInfo)
{
	String *computerName = MOG_ControllerSystem::GetComputerName();

	// Force the local asset list to be rebuilt
	mLocalAssets = NULL;

	// loop over current synced data and look for missing files
	ArrayList *currentAssets = syncInfo->GetPotentialAssetsToBeSyncedList();
	for( int i = 0; i < currentAssets->Count; i++ )
	{
		// Open the blessed revision of this asset
		MOG_Filename *copyMe = __try_cast<MOG_Filename*>(currentAssets->Item[i]);

		// Show the name of the asset and the progress
		String *message = String::Concat(	S"Restamping:\n",
											S"     ", copyMe->GetAssetClassification(), S"\n",
											S"     ", copyMe->GetAssetLabel());
		worker->ReportProgress((i * 100) / currentAssets->Count, message);

		// Make sure we haven't been canceled
		if (!worker->CancellationPending)
		{
			// Check if this asset is not included?
			if(!syncInfo->IsAssetIncluded(copyMe, exclusionList, inclusionList))
			{
				continue;
			}
			// Check if this is a non-syncing asset?
			if (syncInfo->IsNonSyncingAsset(copyMe))
			{
				continue;
			}
			// Check if this asset is listed in the local data?
			if (IsAssetListedInLocalWorkspace(copyMe))
			{
				// Skipping this local workspace asset will save us from having to restamp files for no reason
				continue;
			}

			MOG_Filename *blessedAsset = new MOG_Filename(MOG_ControllerRepository::GetAssetBlessedVersionPath(copyMe, copyMe->GetVersionTimeStamp()));
			MOG_Properties *assetProperties = new MOG_Properties(blessedAsset);
			assetProperties->SetScope(mPlatformName);

			// Attempt to copy this asset's files?
			ProcessAssetFiles(assetProperties, assetProperties, worker);
		}
		else
		{
			mSyncLog->PutString( S"Assets.Canceled", copyMe->GetAssetFullName(), S"" );
		}
	}

	return true;
}

bool MOG_ControllerSyncData::UnsyncRepositoryData(String *classification, String *exclusionList)
{
	List<String*>* args = new List<String*>();
	args->Add(classification);
	args->Add(exclusionList);
	
	// Initialize the dialog
	ProgressDialog* progress = new ProgressDialog(S"Removing local data", S"Please wait while MOG removes the local data.", new DoWorkEventHandler(this, &MOG_ControllerSyncData::UnsyncRepositoryData_Worker), args, true);
	if (progress->ShowDialog() == DialogResult::OK)
	{
		return true;
	}

	return false;
}

void MOG_ControllerSyncData::UnsyncRepositoryData_Worker(Object* sender, DoWorkEventArgs* e)
{
	BackgroundWorker* worker = dynamic_cast<BackgroundWorker*>(sender);
	List<String*>* args = dynamic_cast<List<String*>*>(e->Argument);
	String* classification = args->Item[0];
	String* exclusionList = args->Item[1];
	
	String *computerName = MOG_ControllerSystem::GetComputerName();

	// Begin this GetLatest
	ModifyLocalWorkspace_Begin();

	String *syncInfoFilename = String::Concat(mSyncDirectory, S"\\MOG\\Sync.Info");

	// FileHamster Integration...
	// Make sure we ignore everything in the hidden MOG directory
	MOG_FileHamsterIntegration::FileHamsterPause(syncInfoFilename);

	// Check if we are missing the Sync.Info file?
	if (!DosUtils::FileExistFast(syncInfoFilename))
	{
		// Remove this sync location from the Database so it will be fully updated this time
		if (MOG_DBSyncedDataAPI::RemoveSyncedLocation(computerName, mProjectName, mPlatformName, mSyncDirectory, mUserName))
		{
		}
	}

	// Always make sure that this mSyncDirectory is a recognized sync location
	if (MOG_DBSyncedDataAPI::AddSyncedLocation(computerName, mProjectName, mPlatformName, mSyncDirectory, S"", mName, mUserName))
	{
	}

	// Always validate the workspace
	ValidateLocalWorkspace();

	// Create our filterProperties
	ArrayList *filterProperties = new ArrayList();

	// Loop through all the assets needing to be unsynced
	ArrayList *assetsThatNeedToBeRemoved = MOG_DBSyncedDataAPI::GetCurrentSyncedAssetsViaClassification(computerName, mProjectName, mPlatformName, mSyncDirectory, mUserName, classification);
	for( int i = 0; i < assetsThatNeedToBeRemoved->Count; i++ )
	{
		MOG_Filename *removeMe = __try_cast<MOG_Filename*>(assetsThatNeedToBeRemoved->Item[i]);

		// Show the name of the asset and the progress
		String *message = String::Concat(	S"Unsyncing:\n",
											S"     ", removeMe->GetAssetClassification(), S"\n",
											S"     ", removeMe->GetAssetLabel());
		worker->ReportProgress((i * 100) / assetsThatNeedToBeRemoved->Count, message);

		// Make sure we haven't been canceled
		if (!worker->CancellationPending)
		{
			MOG_Filename *blessedAsset = new MOG_Filename(MOG_ControllerRepository::GetAssetBlessedVersionPath(removeMe, removeMe->GetVersionTimeStamp()));
			MOG_Properties *assetProperties = new MOG_Properties(blessedAsset);
			assetProperties->SetScope(mPlatformName);

			// Remove this asset's files
			if (ProcessAssetFiles(assetProperties, NULL, worker))
			{
				MOG_DBSyncedDataAPI::RemoveSyncedLocationLink(removeMe, computerName, mProjectName, mPlatformName, mSyncDirectory, mUserName);
			}
		}
		else
		{
			mSyncLog->PutString( S"Assets.Canceled", removeMe->GetAssetFullName(), S"" );
		}
	}

	// End this GetLatest
	ModifyLocalWorkspace_Complete();
}

bool MOG_ControllerSyncData::UnsyncDataFromSyncLocation(String *syncLocation, String *exclusionList)
{
	List<String*>* args = new List<String*>();
	args->Add(syncLocation);
	args->Add(exclusionList);
	
	// Initialize the dialog
	ProgressDialog* progress = new ProgressDialog(S"Removing local data", S"Please wait while MOG removes the local data.", new DoWorkEventHandler(this, &MOG_ControllerSyncData::UnsyncDataFromSyncLocation_Worker), args, true);
	if (progress->ShowDialog() == DialogResult::OK)
	{
		return true;
	}

	return false;
}

void MOG_ControllerSyncData::UnsyncDataFromSyncLocation_Worker(Object* sender, DoWorkEventArgs* e)
{
	BackgroundWorker* worker = dynamic_cast<BackgroundWorker*>(sender);
	List<String*>* args = dynamic_cast<List<String*>*>(e->Argument);
	String* syncLocation = args->Item[0];
	String* exclusionList = args->Item[1];
	
	String *computerName = MOG_ControllerSystem::GetComputerName();

	// Begin this GetLatest
	ModifyLocalWorkspace_Begin();

	// Check if we are missing the Sync.Info file?
	String *syncInfoFilename = String::Concat(mSyncDirectory, S"\\MOG\\Sync.Info");
	if (!DosUtils::FileExistFast(syncInfoFilename))
	{
		// Remove this sync location from the Database so it will be fully updated this time
		if (MOG_DBSyncedDataAPI::RemoveSyncedLocation(computerName, mProjectName, mPlatformName, mSyncDirectory, mUserName))
		{
		}
	}

	// Always make sure that this mSyncDirectory is a recognized sync location
	if (MOG_DBSyncedDataAPI::AddSyncedLocation(computerName, mProjectName, mPlatformName, mSyncDirectory, S"", mName, mUserName))
	{
	}

	// Always validate the workspace
	ValidateLocalWorkspace();

	// Create our filterProperties
	ArrayList *filterProperties = new ArrayList();

	// Loop through all the assets needing to be unsynced
	ArrayList *assetsThatNeedToBeRemoved = MOG_DBSyncedDataAPI::GetCurrentSyncedAssetsViaSyncLocation(computerName, mProjectName, mPlatformName, mSyncDirectory, mUserName, syncLocation);
	for( int i = 0; i < assetsThatNeedToBeRemoved->Count; i++ )
	{
		MOG_Filename *removeMe = __try_cast<MOG_Filename*>(assetsThatNeedToBeRemoved->Item[i]);

		// Show the name of the asset and the progress
		String *message = String::Concat(	S"Unsyncing:\n",
											S"     ", removeMe->GetAssetClassification(), S"\n",
											S"     ", removeMe->GetAssetLabel());
		worker->ReportProgress((i * 100) / assetsThatNeedToBeRemoved->Count, message);

		// Make sure we haven't been canceled
		if (!worker->CancellationPending)
		{
			MOG_Filename *blessedAsset = new MOG_Filename(MOG_ControllerRepository::GetAssetBlessedVersionPath(removeMe, removeMe->GetVersionTimeStamp()));
			MOG_Properties *assetProperties = new MOG_Properties(blessedAsset);
			assetProperties->SetScope(mPlatformName);

			// Remove this asset's files
			if (ProcessAssetFiles(assetProperties, NULL, worker))
			{
				MOG_DBSyncedDataAPI::RemoveSyncedLocationLink(removeMe, computerName, mProjectName, mPlatformName, mSyncDirectory, mUserName);
			}
		}
		else
		{
			mSyncLog->PutString( S"Assets.Canceled", removeMe->GetAssetFullName(), S"" );
		}
	}

	// End this GetLatest
	ModifyLocalWorkspace_Complete();
}

ArrayList *MOG_ControllerSyncData::GetAllSyncDataFileStructsForPlatform( String *platformName )
{
    return MOG::DATABASE::MOG_DBAssetAPI::GetAllProjectSyncTargetInfosForPlatformExcludeLibrary( platformName );
}

ArrayList *MOG_ControllerSyncData::GetSyncDataFileAssetByFileName( String *syncdataFileName, String *platformName )
{
	return MOG::DATABASE::MOG_DBAssetAPI::GetSyncTargetFileAssetLinksFileNameOnly( syncdataFileName, platformName );
}


bool MOG_ControllerSyncData::RemoveLocalFile(String *fileToRemove, String *originalFile, bool showLocalModifiedWarning)
{
	bool bRemoved = true;

	// See if the local file has been modified
	if (SyncAssetFiles_HasBeenModified(fileToRemove, originalFile))
	{
		if (SyncAssetFiles_CanOverwriteFile(fileToRemove, originalFile, showLocalModifiedWarning) )
		{
			// Move the user's modified file to the recycle bin just in case they want to recover it
			if (!DosUtils::Recycle(fileToRemove))
			{
				bRemoved = false;
			}
		}
		else
		{
			bRemoved = false;
		}
	}
	else
	{
		// Do the fast delete
		if (!DosUtils::FileDelete(fileToRemove))
		{
			bRemoved = false;
		}
	}

	return bRemoved;
}

bool MOG_ControllerSyncData::CopyLocalFile(String *fileToCopy, String *destinationFile, bool bClearReadOnlyFlag, BackgroundWorker* worker)
{
	bool bCopied = false;

	// Proceed to copy this asset's file
	if (MOG_ControllerSystem::FileCopyEx(fileToCopy, destinationFile, true, bClearReadOnlyFlag, worker))
	{
		bCopied = true;
	}

	return bCopied;
}


bool MOG_ControllerSyncData::RestampLocalAssets(BackgroundWorker* worker, bool bPerformFullRestamp)
{
	// Check if we are logged in as a user?
	if (MOG_ControllerProject::IsUser())
	{
		// Scan all of the inbox assets
		ArrayList *localAssets = MOG_DBInboxAPI::InboxGetAssetListWithProperties(S"Local", MOG_ControllerProject::GetUserName_DefaultAdmin(), "", "");

		// Scan the local assets
		ArrayList *assets = new ArrayList();
		assets->AddRange(localAssets);
		for( int i = 0; i < assets->Count; i++ )
		{
			MOG_DBInboxProperties* item = __try_cast<MOG_DBInboxProperties*>( assets->Item[i] );
			MOG_Filename* assetFilename = item->mAsset;

			bool bValidAsset = false;

			if (assetFilename->IsLocal())
			{
				// Check if this local asset is within this workspace?
				String *localWorkspaceDirectory = String::Concat(mSyncDirectory, S"\\");
				if (assetFilename->GetPath()->StartsWith(localWorkspaceDirectory, StringComparison::CurrentCultureIgnoreCase))
				{
					bValidAsset = true;
				}
			}

			// Check if we determined this is a valid asset for restamping?
			if (bValidAsset)
			{
				// Show the name of the asset and the progress
				String *message = String::Concat(	S"Restamping:\n",
													S"     ", assetFilename->GetAssetClassification(), S"\n",
													S"     ", assetFilename->GetAssetLabel());
				worker->ReportProgress((i * 100) / assets->Count, message);

				// Make sure we haven't been canceled
				if (!worker->CancellationPending)
				{
					// Create the properties for this asset
					MOG_Properties *pProperties = new MOG_Properties(item->mProperties);

					// Check if this asset was omitted during the sync?
					if (mSyncLog->KeyExist(S"Assets.Omitted", assetFilename->GetAssetFullName()))
					{
						// Check if this omitted asset's revision is newer than our local asset?
						String* omittedAssetRevision = mSyncLog->GetString(S"Assets.Omitted", assetFilename->GetAssetFullName());
						if (String::Compare(omittedAssetRevision, pProperties->PreviousRevision, true) >= 0 ||
							String::Compare(omittedAssetRevision, pProperties->BlessedTime, true) >= 0)
						{
							// Proceed to perform the update we previously omitted to ensure we have the latest files
							MOG_Filename *omittedAssetFilename = MOG_ControllerRepository::GetAssetBlessedVersionPath(assetFilename, omittedAssetRevision);
							MOG_Properties *omittedProperties = new MOG_Properties(omittedAssetFilename);
							// ProcessAssetFiles() requires a properties associated with an asset and pProperties isn't because it came to us from the database
							MOG_Properties *localProperties = new MOG_Properties(assetFilename);
							if (ProcessAssetFiles(localProperties, omittedProperties, worker))
							{
								// Automatically delete this asset because it is out-of-date
								if (MOG_ControllerInbox::Delete(assetFilename))
								{
									// Record this asset as being cleaned for subsequent loops
									mSyncLog->PutString( S"Assets.Cleaned", assetFilename->GetAssetFullName(), S"" );
									continue;
								}
							}
						}
					}

					// Check if this asset is in the user's local updated tray?
					if (assetFilename->IsLocal())
					{
						// Check if this asset should be packaged?
						if (pProperties->IsPackagedAsset)
						{
							//We only need to repackage the asset if the packages it is assigned to were affected by the sync
							bool bAssetNeedsPackaging = false;
							
							//Go through all the packages this asset is assigned to and see if they were affected
							ArrayList* packages = pProperties->GetPackages();
							for (int i = 0; i < packages->Count; i++)
							{
								MOG_Property* packageProperty = __try_cast<MOG_Property*>(packages->Item[i]);
								if (packageProperty)
								{
									// Get the name of this package w/o any of the package group/object descriptors
									String* packageName = MOG_ControllerPackage::GetPackageName(packageProperty->mPropertyKey);
									// Check if this is an 'All' package assignment by checking if it is not platform specific?
									MOG_Filename* packageFilename = new MOG_Filename(packageName);
									if (!packageFilename->IsPlatformSpecific())
									{
										// Check if a platform specific version of this package exists in the project for this platform?
										MOG_Filename* platformSpecificPackageFilename = MOG_ControllerProject::GetPlatformSpecificAsset(packageFilename, mPlatformName);
										if (platformSpecificPackageFilename)
										{
											// Substitute this platform-specific package for the packageName
											packageName = platformSpecificPackageFilename->GetAssetFullName();
										}
									}
									// Was this assigned package changed during the sync?
									if (mSyncLog->KeyExist(S"Assets.Removed", packageName) ||
										mSyncLog->KeyExist(S"Assets.Copied", packageName) ||
										mSyncLog->KeyExist(S"Assets.New", packageName) ||
										mSyncLog->KeyExist(S"Assets.Restored", packageName) ||
										mSyncLog->KeyExist(S"Assets.Reverted", packageName))
									{
										// Open the local updated asset
										MOG_ControllerAsset *asset = MOG_ControllerAsset::OpenAsset(assetFilename);
										if (asset)
										{
											// Mark this asset's status as 'Repackage' so it will be packaged again
											MOG_ControllerInbox::UpdateInboxView(asset, MOG_AssetStatusType::Unpackaged);
											asset->Close();
										}

										// No need to keep checking once we have determined it is going to get repackaged
										break;
									}
								}
							}
						}
						else
						{
							// Check if we are performing a full restamp? types of assets on a normal GetLatest
							if (bPerformFullRestamp)
							{
								// Restamp the asset's files
								MOG_Properties *restampProperties = new MOG_Properties(assetFilename);
								if (!ProcessAssetFiles(restampProperties, restampProperties, worker))
								{
									// Inform the user we failed to restamp the locally updated asset
								}
							}
						}
					}
				}
				else
				{
					mSyncLog->PutString( S"Assets.Canceled", assetFilename->GetAssetFullName(), S"" );
				}
			}
		}
	}

	// Check if there is a pending package merge request?
	AutoPackage();

	return true;
}


ArrayList *MOG_ControllerSyncData::GetAssetSyncRelativeFileList(MOG_Properties *pProperties)
{
	// no platform specified, get the source files
	String *filesDirctory = MOG_ControllerAsset::GetAssetProcessedDirectory(pProperties, mPlatformName);
	return DosUtils::FileGetRecursiveRelativeList(filesDirctory, S"");
}


bool MOG_ControllerSyncData::IsAssetListedInLocalWorkspace(MOG_Filename *assetFilename)
{
	// Check if we need to load our list of local assets?
	if (mLocalAssets == NULL)
	{
		String *boxName = String::Concat(S"Local", S".", MOG_ControllerSystem::GetComputerName());
		String *filter = String::Concat(mSyncDirectory, S"\\*");
		mLocalAssets = MOG_DBInboxAPI::InboxGetAssetList(boxName, MOG_ControllerProject::GetUserName(), S"", filter);
	}

	// Check if we have a valid array?
	if (mLocalAssets)
	{
		// Scan the list of local workspace assets
		for (int a = 0; a < mLocalAssets->Count; a++)
		{
			// Check if this asset matches?
			MOG_Filename *testFilename = __try_cast<MOG_Filename*>(mLocalAssets->Item[a]);
			if (String::Compare(testFilename->GetAssetFullName(), assetFilename->GetAssetFullName(), true) == 0)
			{
				// Indicate this asset was found to be listed in the local assets
				return true;
			}
		}
	}

	return false;
}

MOG_ControllerSyncData::PackageState MOG_ControllerSyncData::GetLocalPackagingStatus()
{
	return MOG_ControllerSyncData::GetLocalPackagingStatus(NULL);
}

MOG_ControllerSyncData::PackageState MOG_ControllerSyncData::GetLocalPackagingStatus(MOG_Filename *assetFilename)
{
	PackageState localPackagingStatus = PackageState_NothingToDo;

	// Check if we have the package merge progress dialog active?
	if (mIsPackaging)
	{
		// Always assume we are busy when we have our dialog up
		localPackagingStatus = PackageState_Busy;

		// Check if a specific asset was specified?
		if (assetFilename)
		{
			// Check if this asset is listed in the assets currently being packaged?
			if (mAssetNamesGettingPackagedList &&
				!mAssetNamesGettingPackagedList->Contains(assetFilename->GetAssetFullName()))
			{
				// The specified asset is not envolved in the current merge
				localPackagingStatus = PackageState_NothingToDo;
			}
		}
	}
	else
	{
		ArrayList *localAssets = NULL;

		// Check if there was a specific asset specified?
		if (assetFilename)
		{
			localAssets = MOG_DBInboxAPI::InboxGetLocalAssetList(mSyncDirectory, assetFilename);
		}
		else
		{
			localAssets = MOG_DBInboxAPI::InboxGetLocalAssetList(mSyncDirectory);
		}

		if (localAssets)
		{
			// Build our text strings for the various status types
			String* textUnpackaged = MOG_AssetStatus::GetText(MOG_AssetStatusType::Unpackaged);
			String* textPackaging = MOG_AssetStatus::GetText(MOG_AssetStatusType::Packaging);
			String* textRepackage = MOG_AssetStatus::GetText(MOG_AssetStatusType::Repackage);
			String* textRepackaging = MOG_AssetStatus::GetText(MOG_AssetStatusType::Repackaging);
			String* textUnpackage = MOG_AssetStatus::GetText(MOG_AssetStatusType::Unpackage);
			String* textUnpackaging = MOG_AssetStatus::GetText(MOG_AssetStatusType::Unpackaging);
			String* textPackageError = MOG_AssetStatus::GetText(MOG_AssetStatusType::PackageError);

			// go through the list and check the asset statuses to see if any of them need packaging
			for (int a = 0; a < localAssets->Count; a++)
			{
				MOG_DBInboxProperties *inboxProp = __try_cast<MOG_DBInboxProperties*>(localAssets->Item[a]);

				// Check if a specific asset was specified?
				if (assetFilename)
				{
					// Check if this inbox asset matches?
					if (String::Compare(inboxProp->mAsset->GetAssetFullName(), assetFilename->GetAssetFullName(), true) != 0)
					{
						// Skip this asset
						continue;
					}
				}

				MOG_Properties *properties = new MOG_Properties(inboxProp->mProperties);

				if (String::Compare(properties->Status, textPackageError, true) == 0)
				{
					// Check if this is a higher priority than our surrent status?
					if (localPackagingStatus < PackageState_Error)
					{
						localPackagingStatus = PackageState_Error;
					}
				}
				else if (String::Compare(properties->Status, textUnpackaged, true) == 0 ||
						 String::Compare(properties->Status, textRepackage, true) == 0 ||
						 String::Compare(properties->Status, textUnpackage, true) == 0)
				{
					// Check if this is a higher priority than our surrent status?
					if (localPackagingStatus < PackageState_Pending)
					{
						localPackagingStatus = PackageState_Pending;
					}
				}
				else if (String::Compare(properties->Status, textPackaging, true) == 0 ||
						 String::Compare(properties->Status, textRepackaging, true) == 0 ||
						 String::Compare(properties->Status, textUnpackaging, true) == 0)
				{
					// Check if this is a higher priority than our surrent status?
					if (localPackagingStatus < PackageState_Busy)
					{
						// Check if we have the package merge progress dialog active?
						if (mIsPackaging)
						{
							localPackagingStatus = PackageState_Busy;
						}
						else
						{
							localPackagingStatus = PackageState_Pending;
						}

						// We can stop early on this one
						break;
					}
				}
			}
		}
	}

	return localPackagingStatus;
}


bool MOG_ControllerSyncData::ExecuteLocalPackageMerge(MOG_ControllerSyncData* sync)
{
	if (sync)
	{
		// Update the status for all the assets needing to be packaged...
		ArrayList *localAssets = MOG_DBInboxAPI::InboxGetLocalAssetList(sync->GetSyncDirectory());
		ArrayList *assetList = new ArrayList();
		if (localAssets)
		{
			// Build our text strings for the various status types
			String* textPackageError = MOG_AssetStatus::GetText(MOG_AssetStatusType::PackageError);
			String* textUnpackaged = MOG_AssetStatus::GetText(MOG_AssetStatusType::Unpackaged);
			String* textRepackage = MOG_AssetStatus::GetText(MOG_AssetStatusType::Repackage);
			String* textUnpackage = MOG_AssetStatus::GetText(MOG_AssetStatusType::Unpackage);

			// go through the list and check the asset status to see if any of them need packaging
			for (int a = 0; a < localAssets->Count; a++)
			{
				MOG_DBInboxProperties *inboxProp = __try_cast<MOG_DBInboxProperties*>(localAssets->Item[a]);
				MOG_Properties *properties = new MOG_Properties(inboxProp->mProperties);
				MOG_Filename *assetFilename = inboxProp->mAsset;

				if (String::Compare(properties->Status, textPackageError, true) == 0 ||
					String::Compare(properties->Status, textUnpackaged, true) == 0)
				{
					// Open this asset and change its status indicating it is involved in this package merge
					MOG_ControllerAsset *asset = MOG_ControllerAsset::OpenAsset(assetFilename);
					if (asset)
					{
						// Change the asset's status to 'Packaging'
						MOG_ControllerInbox::UpdateInboxView(asset, MOG_AssetStatusType::Packaging);
						asset->Close();
						// Track this asset
						assetList->Add(assetFilename->GetAssetFullName());
					}
				}
				if (String::Compare(properties->Status, textRepackage, true) == 0)
				{
					// Open this asset and change its status indicating it is involved in this package merge
					MOG_ControllerAsset *asset = MOG_ControllerAsset::OpenAsset(assetFilename);
					if (asset)
					{
						// Change the asset's status to 'Repackaging'
						MOG_ControllerInbox::UpdateInboxView(asset, MOG_AssetStatusType::Repackaging);
						asset->Close();
						// Track this asset
						assetList->Add(assetFilename->GetAssetFullName());
					}
				}
				else if (String::Compare(properties->Status, textUnpackage, true) == 0)
				{
					// Open this asset and change its status indicating it is involved in this package merge
					MOG_ControllerAsset *asset = MOG_ControllerAsset::OpenAsset(assetFilename);
					if (asset)
					{
						// Change the asset's status to 'Packaging'
						MOG_ControllerInbox::UpdateInboxView(asset, MOG_AssetStatusType::Unpackaging);
						asset->Close();
						// Track this asset
						assetList->Add(assetFilename->GetAssetFullName());
					}
				}
			}
		}

		// Open the package merge progress dialog
		sync->BeginPackaging(assetList);
	}

	// Send the request to the server so it can be forwarded on to our Editor
	MOG_Command *command = MOG_CommandFactory::Setup_LocalPackageMerge(sync->mSyncDirectory, sync->mPlatformName);
	return MOG_ControllerSystem::GetCommandManager()->SendToServer(command);
}

void MOG_ControllerSyncData::BuildPackage()
{
	try
	{
		// Set window to be silent
		MOG_ControllerSystem::GetCommandManager()->mCommandLineHideWindow = true;

		// Make sure we are not already busy?
		// Make sure our Editor isn't already busy?
		if (GetLocalPackagingStatus() != PackageState_Busy)
		{
			// Start the process of packaging
			MOG_ControllerSyncData::ExecuteLocalPackageMerge(this);
		}
	}
	catch (Exception* e)
	{
		MOG_Prompt::PromptMessage("Package", String::Concat("Could not perform package due to error:\n\n", e->Message), e->StackTrace);
	}
}

void MOG_ControllerSyncData::SuspendPackaging(bool suspend)
{
	mPackagingSuspended = suspend;

	if (!suspend)
	{
		// We're coming back from the dead!  Let's package anything that got updated while we were suspended
		AutoPackage();
	}
}

void MOG_ControllerSyncData::EnableAutoPackaging(bool autoPackage)
{
	mAutoPackageEnabled = autoPackage;
}

void MOG_ControllerSyncData::EnableAutoUpdating(bool autoUpdate)
{
	mAutoUpdateEnabled = autoUpdate;
}

void MOG_ControllerSyncData::AutoPackage()
{
	// Check if should continue with this auto package request?
	if (IsAutoPackageEnabled() && !mPackagingSuspended)
	{
		if (GetLocalPackagingStatus() == PackageState_Pending)
		{
			BuildPackage();
		}
	}
}

bool MOG_ControllerSyncData::ModifyLocalWorkspace_Begin()
{
	// Create a new sync log
	mSyncLog = new MOG_Ini();

	// Make sure we recache our list of local assets
	mLocalAssets = NULL;

	// Reset our YesToAll/NoToAll
	mGetLatest_OverwriteFile->Reset();
	mRemoveOutofdateVerifiedLocalAsset->Reset();

	// Initialize the FileHamster integration
	MOG_FileHamsterIntegration::Initialize();

	// Activate the properties cache
	MOG_Properties::ActivatePropertiesCache(true);

	return true;
}

bool MOG_ControllerSyncData::ModifyLocalWorkspace_Complete()
{
	// Restamp the local Sync.Info file
	RestampLocalSyncInfoFile();

	// Reset our YesToAll/NoToAll
	mGetLatest_OverwriteFile->Reset();
	mRemoveOutofdateVerifiedLocalAsset->Reset();

	// FileHamster integration
	MOG_FileHamsterIntegration::FileHamsterUnPause();

	// Deactivate the properties cache
	MOG_Properties::ActivatePropertiesCache(false);

	return true;
}


bool MOG_ControllerSyncData::RestampLocalSyncInfoFile()
{
	String *syncInfoFilename = String::Concat(mSyncDirectory, S"\\MOG\\Sync.Info");

	// Update the Sync.Info file
	MOG_Ini *syncFile = new MOG_Ini();
	if (syncFile->Load(syncInfoFilename))
	{
		syncFile->PutString("Workspace", "Project", MOG_ControllerProject::GetProjectName());
		syncFile->PutString("Workspace", "Branch", MOG_ControllerProject::GetBranchName());
		syncFile->PutString("Workspace", "User", MOG_ControllerProject::GetUserName());
		syncFile->PutString("Workspace", "Platform", mPlatformName);
		syncFile->PutString("Workspace", "LastUpdatedBy", MOG_ControllerProject::GetUserName());
		syncFile->PutString("Workspace", "LastUpdatedFrom", MOG_ControllerSystem::GetComputerName());
		syncFile->PutString("Workspace", "LastUpdated", MOG_Time::GetVersionTimestamp());
		syncFile->Close();

		return true;
	}

	return false;
}


String* MOG_ControllerSyncData::GetLastUpdatedTime()
{
	String *syncInfoFilename = String::Concat(mSyncDirectory, S"\\MOG\\Sync.Info");

	// Update the Sync.Info file
	MOG_Ini *syncFile = new MOG_Ini(syncInfoFilename);
	// Get the workspace's LastUpdated time
	if (syncFile->KeyExist("Workspace", "LastUpdated"))
	{
		return syncFile->GetString("Workspace", "LastUpdated");
	}

	return "";
}


String* MOG_ControllerSyncData::DetectWorkspaceRoot(String* path)
{
	// Keep trying until we have nothing left to check
	while (path->Length)
	{
		// Check this path for our 'MOG' directory
		String *syncInfoFilename = String::Concat(path, "\\MOG\\Sync.Info");
		if (!DosUtils::FileExistFast(syncInfoFilename))
		{
			// Step back up a directory and try this again
			int pos = path->LastIndexOf(S"\\");
			if (pos != -1)
			{
				// Move up one directory
				path = path->Substring(0, pos);
				continue;
			}
			else
			{
				// Looks like we are done
				path = "";
				continue;
			}
		}

		// If we ever hit here we are finished
		break;
	}

	return path;
}


