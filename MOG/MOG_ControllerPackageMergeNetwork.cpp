//--------------------------------------------------------------------------------
//	MOG_ControllerPackageMergeNetwork.cpp
//	
//	
//--------------------------------------------------------------------------------

#include "StdAfx.h"

#include "MOG_Define.h"
#include "MOG_DosUtils.h"
#include "MOG_ControllerSystem.h"
#include "MOG_ControllerProject.h"
#include "MOG_ControllerRepository.h"
#include "MOG_ControllerInbox.h"
#include "MOG_ControllerPackage.h"
#include "MOG_DBInboxAPI.h"
#include "MOG_DBSyncedDataAPI.h"
#include "MOG_DBPackageAPI.h"
#include "MOG_DBPostCommandAPI.h"
#include "MOG_DBPackageCommandAPI.h"
#include "MOG_EnvironmentVars.h"
#include "MOG_CommandFactory.h"

#include "MOG_ControllerPackageMergeNetwork.h"


MOG_ControllerPackageMergeNetwork::MOG_ControllerPackageMergeNetwork()
{
	mRejectAssets = false;
}

MOG_ControllerPackageMergeNetwork::MOG_ControllerPackageMergeNetwork(bool bRejectAssets)
{
	mRejectAssets = bRejectAssets;
}

PackageList* MOG_ControllerPackageMergeNetwork::DoPackageEvent(MOG_Filename* packageFilename, String* jobLabel, String* branchName, String* workspaceDirectory, String* platformName, String* validSlaves, BackgroundWorker* worker)
{
	PackageList* affectedPackageFiles = new PackageList;

	mWorkspaceDirectory = workspaceDirectory;
	mPlatformName = platformName;
	mJobLabel = jobLabel;
	mValidSlaves = validSlaves;
	mPackageMergeTasks = new PackageList();

	// Get the scheduled package commands for this NetworkPackageMerge command
	ArrayList* pPackageCommands = GetScheduledNetworkPackageCommands(packageFilename, mJobLabel, branchName);
	if (pPackageCommands)
	{
		// Check if we actually got something to do?
		if (pPackageCommands->Count)
		{
			// Get the list of packages contained within the list of package commands
			PackageList* possiblePackageFiles = GetPackageFilesFromPackageCommands(pPackageCommands);
			if (possiblePackageFiles->Count)
			{
				PackageList* executeNetworkMergeList = new PackageList();
				PackageList* skipNetworkMergeList = new PackageList();

				// Determine what needs to be done with each package file?
				for (int p = 0; p < possiblePackageFiles->Count; p++)
				{
					// Get the package file info
					MOG_PackageMerge_PackageFileInfo* packageFileInfo = __try_cast<MOG_PackageMerge_PackageFileInfo*>(possiblePackageFiles->Item[p]);

					// Check if this package file requires a network package merge?
					if (packageFileInfo->mPackageProperties->ExecuteNetworkPackageMerge)
					{
						executeNetworkMergeList->Add(packageFileInfo);
					}
					else
					{
						skipNetworkMergeList->Add(packageFileInfo);
					}
				}

				// Check if we should perform a network package merge event?
				if (executeNetworkMergeList->Count > 0)
				{
					// Execute the normal network package merge events
					mPackageFiles = executeNetworkMergeList;
					affectedPackageFiles->AddRange(PerformNetworkPackageMerge(pPackageCommands, worker));
					if (affectedPackageFiles->Count == 0)
					{
						String* message = String::Concat(	S"System did not locate any updated packages during a network package merge.\n",
															S"There should always be an updated package as a result of a package merge command.");
						MOG_Report::ReportMessage(S"Bless/Package", message, Environment::StackTrace, MOG_ALERT_LEVEL::ALERT);
					}
				}

				// Check if we should perform a network package merge event?
				if (skipNetworkMergeList->Count > 0)
				{
					// Update the package relationships with virtual versions of the packages
					mPackageFiles = skipNetworkMergeList;
					affectedPackageFiles->AddRange(UpdatePackageRelationships(pPackageCommands, worker));
				}
			}
		}
	}
	else
	{
		// Make sure to inform the Server about this
		String* message = String::Concat(	S"Slave was unable to receive the package commands\n",
											S"The Database could be down.");
		MOG_Report::ReportMessage(S"PerformNetworkPackageMerge", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
	}

	return affectedPackageFiles;
}


PackageList* MOG_ControllerPackageMergeNetwork::PerformNetworkPackageMerge(ArrayList* pPackageCommands, BackgroundWorker* worker)
{
	PackageList* newlyAffectedPackageFiles = new PackageList();
	PackageList* affectedPackageFiles = new PackageList();
	bool bFailed = false;

	// Make sure all the PackageWorkspaceDirectories are valid
	mPackageFiles = VerifyWorkingDirectories(mPackageFiles);
	// First clean the working directories
	CleanWorkingDirectories(mPackageFiles);
	// Prepare the working directories
	PrepareWorkingDirectories(mPackageFiles);

	// Execute any pre package merge commands
	ExecutePreMergeEvents(mPackageFiles);

	// Execute the package merge commands for each asset tracking the affected packages
	newlyAffectedPackageFiles = ExecuteNetworkPackagingCommands(pPackageCommands);
	affectedPackageFiles->Add(newlyAffectedPackageFiles);

	// Execute each unique package merge task based from the affected packages
	mPackageMergeTasks->GenerateUniquePackageTasks(affectedPackageFiles);
	if (mPackageMergeTasks->Count)
	{
		// Execute each unique package merge task tracking the affected packages
		PreparePackageMergeTasks(mPackageMergeTasks);
		newlyAffectedPackageFiles = ExecutePackageMergeTasks(mPackageMergeTasks);
		affectedPackageFiles->Add(newlyAffectedPackageFiles);

		// Report any errors listed in the output task files
		ParseOutputTaskFile(mPackageMergeTasks);

		// Reject any failed assets back to the user
		RejectFailedAssets(pPackageCommands, worker);
	}

	// Execute any post package merge commands
	ExecutePostMergeEvents(affectedPackageFiles);

	// Process affected packages
	PackageList* processedPackageFiles = ProcessAffectedPackages(affectedPackageFiles, pPackageCommands, worker);

	// Clean the working directories
	CleanWorkingDirectories(affectedPackageFiles);

	return affectedPackageFiles;
}


PackageList* MOG_ControllerPackageMergeNetwork::UpdatePackageRelationships(ArrayList* pPackageCommands, BackgroundWorker* worker)
{
	PackageList* affectedPackageFiles = new PackageList();

	// Update all of the effected packages' relationships
	for (int p = 0; p < mPackageFiles->Count; p++)
	{
		// Get the package file info
		MOG_PackageMerge_PackageFileInfo* packageFileInfo = __try_cast<MOG_PackageMerge_PackageFileInfo*>(mPackageFiles->Item[p]);

		// Use the timestamp of the job label
		String* timeStamp = MOG_ControllerInbox::ObtainTimestampFromJobLabel(mJobLabel);
		MOG_Filename* newBlessedPackageAssetFilename = new MOG_Filename(MOG_ControllerRepository::GetAssetBlessedVersionPath(packageFileInfo->mPackageAssetFilename, timeStamp));

		// Recreate the package using the new revision timestamp
		MOG_Filename *newPackageFilename = MOG_ControllerProject::CreatePackage(newBlessedPackageAssetFilename, packageFileInfo->mPackageProperties->SyncTargetPath, mJobLabel);

		MOG_Filename *previousPackageRevision = packageFileInfo->mBlessedPackageFilename;

		// Make sure this package wasn't intentially deleted?
		if (WasPackageFileScheduledForDeletion(packageFileInfo->mBlessedPackageFilename, pPackageCommands))
		{
			// Don't transfer anything from the previous package revision because this must have been a package rebuild and the commands should contain everything we want!
			previousPackageRevision = NULL;
		}

		// Populate the new package revision information from the current revision of this package
		MOG_DBPackageAPI::PopulateNewPackageVersion(previousPackageRevision, newBlessedPackageAssetFilename, pPackageCommands);

		// Remove the affected asset's package commands
		RemoveAffectedPackageCommands(packageFileInfo, pPackageCommands);

		affectedPackageFiles->Add(packageFileInfo);
	}

	return affectedPackageFiles;
}


PackageList* MOG_ControllerPackageMergeNetwork::ScheduleDeletePackageCommand(MOG_PackageMerge_PackageFileInfo* packageFileInfo, String* jobLabel)
{
	PackageList* affectedPackages = new PackageList();

	// Get this user's name
	String* userName = MOG_ControllerProject::GetUserName_DefaultAdmin();

	// Add the PackageCommand to the database
	if (MOG_DBPackageCommandAPI::SchedulePackageCommand("",
														"",
														jobLabel,
														MOG_ControllerProject::GetBranchName(),
														packageFileInfo->mPackageAssetFilename->GetAssetPlatform(),
														userName,
														userName,
														packageFileInfo->mPackageAssetFilename->GetAssetFullName(),
														"",
														"",
														MOG_PACKAGECOMMAND_TYPE::MOG_PackageCommand_DeletePackage))
	{
		// Add the specified package to the affected packages
		affectedPackages->Add(packageFileInfo);
	}

	return affectedPackages;
}


PackageList* MOG_ControllerPackageMergeNetwork::ScheduleAssetPackageCommands(MOG_ControllerAsset* asset, MOG_PACKAGECOMMAND_TYPE commandType, String* jobLabel, String* validSlaves, bool rejectAssets)
{
	return ScheduleAssetPackageCommands(asset, commandType, jobLabel, validSlaves, NULL, rejectAssets);
}

PackageList* MOG_ControllerPackageMergeNetwork::ScheduleAssetPackageCommands(MOG_ControllerAsset* asset, MOG_PACKAGECOMMAND_TYPE commandType, String* jobLabel, String* validSlaves, MOG_PackageMerge_PackageFileInfo* specificPackageFileInfo, bool rejectAssets)
{
	PackageList* affectedPackages = new PackageList();

	// Make sure we have a valid asset?
	if (asset)
	{
		// Get the asset's properties
		MOG_Properties* assetProperties = asset->GetProperties();
		// Does this asset need to have it's files packaged?
		if (assetProperties->IsPackagedAsset)
		{
			// Get info from the asset
			String* assetName = asset->GetAssetFilename()->GetAssetFullName();
			String* assetVersion = asset->GetAssetFilename()->GetVersionTimeStamp();
			String* assetPlatform = asset->GetAssetFilename()->GetAssetPlatform();

			// Make sure this asset's platform is still valid in the project?
			if (MOG_ControllerProject::IsValidPlatform(assetPlatform))
			{
				ArrayList* packageAssignments = asset->GetProperties()->GetPackages();

				// Walk through all the listed packages
				for (int packageIndex = 0; packageIndex < packageAssignments->Count; packageIndex++)
				{
					MOG_Property* packageAssignmentProperty = __try_cast<MOG_Property*>(packageAssignments->Item[packageIndex]);

					// Build the package related tokens
					MOG_Filename* packageName = new MOG_Filename(MOG_ControllerPackage::GetPackageName(packageAssignmentProperty->mPropertyKey));
					String* packageGroups = MOG_ControllerPackage::GetPackageGroups(packageAssignmentProperty->mPropertyKey);
					String* packageObjects = MOG_ControllerPackage::GetPackageObjects(packageAssignmentProperty->mPropertyKey);

					// Check if there was a specific package specified?
					if (specificPackageFileInfo)
					{
						bool bValidPackageAssignment = false;

						// Check if this package assignment matches the specific package specified?
						if (String::Compare(packageName->GetAssetClassification(), specificPackageFileInfo->mPackageAssetFilename->GetAssetClassification(), true) == 0 && 
							String::Compare(packageName->GetAssetLabel(), specificPackageFileInfo->mPackageAssetFilename->GetAssetLabel(), true) == 0)
						{
							// Check if this package assignment is platform-specific?  and
							if (packageName->IsPlatformSpecific())
							{
								// Make sure the platform match!
								if (String::Compare(packageName->GetAssetPlatform(), specificPackageFileInfo->mPackageAssetFilename->GetAssetPlatform(), true) == 0)
								{
									// We always want to package matching platform-specific package assignments
									bValidPackageAssignment = true;
								}
							}
							else
							{
								// We always want to package 'All' package assignments
								bValidPackageAssignment = true;
							}
						}

						if (!bValidPackageAssignment)
						{
							// Skip this package assignment as it appears to not be valid for this specified package
							continue;
						}
					}

					// Add this packageName to our list of packageNames
					ArrayList* packageNames = new ArrayList();
					// Check if packagePlatform is the 'All'?
					if (String::Compare(packageName->GetAssetPlatform(), S"All", true) == 0)
					{
						bool bMissingPlatformSpecificPackage = false;

						// Check if there was a specific package specified?
						if (specificPackageFileInfo)
						{
							// Add the specificly specified package as the only package needing to be processed
							packageNames->Add(specificPackageFileInfo->mPackageAssetFilename);
						}
						else
						{
							// Check if any platform specific packages exist within the project?
							String* projectPlatformNames[] = MOG_ControllerProject::GetProject()->GetPlatformNames();
							for (int p = 0; p < projectPlatformNames->Count; p++)
							{
								// Get the platform specific asset for this platform?
								MOG_Filename* platformSpecificAssetFilename = MOG_ControllerProject::GetPlatformSpecificAsset(packageName, projectPlatformNames[p]);
								if (platformSpecificAssetFilename)
								{
									// Add this platform specific package to our list of packageNames
									packageNames->Add(platformSpecificAssetFilename);
								}
								else
								{
									// Flag that we were missing a platform specific package...meaning we should still package the all
									bMissingPlatformSpecificPackage = true;

									// Looks like we are missing this platform specific package
									// Check if this asset is ripped for divergent platforms?
									if (assetProperties->AssetRipper->Length &&
										assetProperties->DivergentPlatformDataType)
									{
										// We should warn the user about this platform divergent asset not being able to be packaged
										String* message = String::Concat(	S"The asset being packaged has been ripped with platform-specific data yet the project doesn't yet have a platform-specific version of this package.\n",
																			S"ASSET: ", asset->GetAssetFilename()->GetAssetFullName(), S"\n",
																			S"PLATFORM: ", projectPlatformNames[p], S"\n",
																			S"PACKAGE: ", packageName->GetAssetFullName(), S"\n",
																			S"A platform-specific package needs to be created in order to package the platform-specific contents of this asset.");
										MOG_Report::ReportMessage(S"Platform-specific Packaging", message, "", MOG_ALERT_LEVEL::ERROR);
									}
								}
							}
						}

						// Check if we were missing a platform specific package?
						if (bMissingPlatformSpecificPackage)
						{
							// Being that there is a missing platform-specific package...looks like we need to package this 'All'
							packageNames->Add(packageName);
						}
					}
					else
					{
						// Always add the package if it isn't an all
						packageNames->Add(packageName);
					}

					// Process each packageFilename
					for (int p = 0; p < packageNames->Count; p++)
					{
						packageName = dynamic_cast<MOG_Filename*>(packageNames->Item[p]);

						// Make sure this package's platform is valid for this asset?
						if (MOG_ControllerProject::IsPlatformValidForAsset(asset->GetAssetFilename(), packageName->GetAssetPlatform()))
						{
							// Make sure this Package exists in our project?
							if (MOG_ControllerProject::DoesAssetExists(packageName))
							{
								// Get the PackageFileInfo
								MOG_PackageMerge_PackageFileInfo* packageFileInfo = new MOG_PackageMerge_PackageFileInfo(packageName, S"", S"", jobLabel);
								// Make sure we obtained a valid PackageFileInfo by checking the mPackageProperties?
								if (packageFileInfo->mPackageProperties)
								{
									// Check for disabled packages?
									if (packageFileInfo->mPackageStyle == MOG_PackageStyle::Disabled)
									{
										// Make sure to inform the user about this disabled package
										String* message = String::Concat(	S"The System just encountered an active package assignment to a disabled package.\n",
																			S"PACKAGEFILE: ", packageFileInfo->mPackageAssetFilename->GetAssetFullName(), S"\n",
																			S"PLATFORM: ", packageFileInfo->mPackageAssetFilename->GetAssetPlatform(), S"\n",
																			S"This is a system warning indicating the package was not modified because it's 'PackageStyle' is disabled.");
										MOG_Report::ReportMessage(S"PackageMerge Warning", message, Environment::StackTrace, MOG_ALERT_LEVEL::ALERT);
										continue;
									}

									// Add the PackageCommand to the database
									if (MOG_DBPackageCommandAPI::SchedulePackageCommand(assetName,
																						assetVersion,
																						jobLabel,
																						MOG_ControllerProject::GetBranchName(),
																						packageName->GetAssetPlatform(),
																						assetProperties->Creator,
																						assetProperties->Owner,
																						packageName->GetAssetFullName(),
																						packageGroups,
																						packageObjects,
																						commandType))
									{
										// Add the specified package to our list of affected packages
										affectedPackages->Add(packageFileInfo);

										// Check the AutoPackage property for this package?
										if (packageFileInfo->mPackageProperties->AutoPackage)
										{
											MOG_Command* command = NULL;

											// Check if no validslave were specified in the API?
											if (validSlaves->Length == 0)
											{
												// At the very least, ask the package if they have a preference
												validSlaves = packageFileInfo->mPackageProperties->ValidSlaves;
											}

											// Check if this network package merge can be clustered?
											if (packageFileInfo->mPackageProperties->ClusterPackaging)
											{
												// Perform a more generic single non-package specific network package merge
												command  = MOG_CommandFactory::Setup_NetworkPackageMerge(packageName->GetAssetPlatform(), jobLabel, validSlaves, rejectAssets);
											}
											else
											{
												// Perform a package specific network package merge
												command = MOG_CommandFactory::Setup_NetworkPackageMerge(packageName->GetAssetFullName(), packageName->GetAssetPlatform(), jobLabel, validSlaves, rejectAssets);
											}
											// Send the network package merge command to the server
											MOG_ControllerSystem::GetCommandManager()->SendToServer(command);
										}
										else
										{
											// Only bother the user with an alert when the asset is being added?
											if (commandType == MOG_PACKAGECOMMAND_TYPE::MOG_PackageCommand_AddAsset)
											{
												// Make sure to inform the Server about this
												String* message = String::Concat(	S"The asset has been scheduled for packaging but no PackageMerge Command was initiated because the asset is not set to be auto packaged.\n",
																					S"ASSET: ", asset->GetAssetFilename()->GetAssetFullName(), S"\n",
																					S"PACKAGE: ", packageName->GetAssetFullName());
												MOG_Report::ReportMessage(S"ScheduleAssetPackageCommands", message, Environment::StackTrace, MOG_ALERT_LEVEL::ALERT);
											}
										}
									}
									else
									{
										String* message = String::Concat(	S"MOG was unable to schedule the asset's package command in the database.\n",
																			S"ASSET: ", asset->GetAssetFilename()->GetAssetFullName(), S"\n",
																			S"PACKAGE: ", packageName->GetAssetFullName());
										MOG_Report::ReportMessage(S"ScheduleAssetPackageCommands", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
									}
								}
							}
							else
							{
								// Only bother the user with an alert when the asset is being added?
								if (commandType == MOG_PACKAGECOMMAND_TYPE::MOG_PackageCommand_AddAsset)
								{
									String* message = String::Concat(	S"The asset contains a package assignment to a missing package.\n",
																		S"ASSET: ", asset->GetAssetFilename()->GetAssetFullName(), S"\n",
																		S"PACKAGE: ", packageName->GetAssetFullName(), S"\n\n",
																		S"It is recommended that this package assignment be removed.");
									MOG_Report::ReportMessage(S"ScheduleAssetPackageCommands", message, Environment::StackTrace, MOG_ALERT_LEVEL::ALERT);
								}
							}
						}
					}
				}
			}
		}
	}

	return affectedPackages;
}


bool MOG_ControllerPackageMergeNetwork::AddLateResolvers(String* PackageName, String* LinkedPackage)
{
	// Check if the PackageName begins with the local workspaceDirectory?
	String* testWorkspaceDirectory = String::Concat(MOG_ControllerProject::GetWorkspaceDirectory(), S"\\");
	if (PackageName->StartsWith(testWorkspaceDirectory, StringComparison::CurrentCultureIgnoreCase))
	{
		// Strip off the local workspace from the specified package file
		PackageName = PackageName->Substring(testWorkspaceDirectory->Length);
	}
	// Check if the LinkedPackage begins with the local workspaceDirectory?
	if (LinkedPackage->StartsWith(testWorkspaceDirectory, StringComparison::CurrentCultureIgnoreCase))
	{
		// Strip off the local workspace from the specified package file
		LinkedPackage = LinkedPackage->Substring(testWorkspaceDirectory->Length);
	}

	// proceed to schedule LateResolver
	if (MOG_DBPackageCommandAPI::SchedulePackageLateResolver(PackageName, LinkedPackage))
	{
		return true;
	}
	else
	{
		return false;
	}
}


bool MOG_ControllerPackageMergeNetwork::PrepareWorkingDirectories(PackageList* packageFiles)
{
	bool bFailed = false;

	// Check if any of the upcomming package tasks require the directory to be synced
	for (int p = 0; p < packageFiles->Count; p++)
	{
		// Get the task info
		MOG_PackageMerge_PackageFileInfo* packageFileInfo = __try_cast<MOG_PackageMerge_PackageFileInfo*>(packageFiles->Item[p]);
		//If there's no package working directory, we have no business doing any work on this package
		if (packageFileInfo->mSyncPackageWorkspaceDirectory)
		{
			// Perform the desired sync on this task's directory
			// We normally don't like calling GetCurrentSyncDataController() because the client can support multiple active workspaces...But this is safe because the network package merge only deal swith a single workspace!
			MOG_ControllerSyncData* syncData = MOG_ControllerProject::GetCurrentSyncDataController();
			if (!syncData->SyncRepositoryData(syncData->GetProjectName(), S"", S"", true, NULL))
			{
				bFailed = true;
			}

			// No reason to do it more than once
			break;
		}

		// If there's no package working directory, we have no business doing any work on this package
		if (packageFileInfo->mPackageWorkspaceDirectory->Length)
		{
			// Make sure this package is valid?
			if (MOG_ControllerProject::DoesAssetExists(packageFileInfo->mPackageAssetFilename))
			{
				String* processedDirectory = MOG_ControllerAsset::GetAssetProcessedDirectory(packageFileInfo->mPackageProperties, mPlatformName);
			    // Use the imported assets of this blessed package asset as our source
			    String* sourcePackageFile = String::Concat(processedDirectory, S"\\", DosUtils::PathGetFileName(packageFileInfo->mRelativePackageFile));
			    String* targetPackageFile = packageFileInfo->mPackageFile;

			    // Check if the source file exists?
			    if (DosUtils::FileExistFast(sourcePackageFile))
			    {
				    if (!DosUtils::FileCopyFast(sourcePackageFile, targetPackageFile, true))
				    {
					    String* message = String::Concat(	S"System failed to seed the PackageWorkspaceDirectory with the latest version of the PackageFile.\n",
														    S"PACKAGEFILE: ", sourcePackageFile, S"\n",
															S"PLATFORM: ", mPlatformName, S"\n",
														    S"This may be caused by a possible file sharing violation.");
					    MOG_Report::ReportMessage(S"Local Package", message, S"", MOG_ALERT_LEVEL::ERROR);
					    bFailed = true;
				    }
				    // Make sure we clear the ReadOnly attribute because this package came directly out of the repository
				    DosUtils::SetAttributes(targetPackageFile, FileAttributes::Normal);
			    }
			    else
			    {
				    // Check if this directory is missing?
				    if (!DosUtils::DirectoryExistFast(DosUtils::PathGetDirectoryPath(targetPackageFile)))
				    {
					    // Always build the directory to where this PackageFile will be created to make it easier on the packaging tools
					    if (!DosUtils::DirectoryCreate(DosUtils::PathGetDirectoryPath(targetPackageFile)))
					    {
						    String* message = String::Concat(	S"The system was unable to create directory ", DosUtils::PathGetDirectoryPath(targetPackageFile), S"\n",
															    S"The PackageTools will not be able function properly without write access to this location.");
						    MOG_Report::ReportMessage(S"Bless/Package", message, S"", MOG_ALERT_LEVEL::ALERT);
					    }
				    }

					// Check if this missing package file is platform specific?
					if (packageFileInfo->mPackageAssetFilename->IsPlatformSpecific())
					{
						// Get all of the assets that have an association with this package file
						ArrayList* containedAssets = MOG_ControllerPackage::GetAssociatedAssetsForPackage(packageFileInfo->mPackageAssetFilename);
						for (int a = 0; a < containedAssets->Count; a++)
						{
							MOG_Filename* assetFilename = __try_cast<MOG_Filename*>(containedAssets->Item[a]);

							// Open this asset
							MOG_Filename* blessedAssetFilename = MOG_ControllerRepository::GetAssetBlessedVersionPath(assetFilename, assetFilename->GetVersionTimeStamp());
							MOG_ControllerAsset* asset = MOG_ControllerAsset::OpenAsset(blessedAssetFilename);
							if (asset)
							{
								ScheduleAssetPackageCommands(asset, MOG_PACKAGECOMMAND_TYPE::MOG_PackageCommand_AddAsset, mJobLabel, mValidSlaves, packageFileInfo, false);
								// Close the asset controller as soon as we are finished with it
								asset->Close();
							}
							else
							{
								// Make sure to inform the Server about this
								String* message = String::Concat(	S"The System was unable to open the respository asset.\n",
																	S"ASSET: ", blessedAssetFilename->GetFullFilename(), S"\n",
																	S"Confirm this asset exists in the MOG Repository.  The package will be missing this asset.");
								MOG_Report::ReportMessage(S"Repository asset appears to be missing", message, S"", MOG_ALERT_LEVEL::ERROR);
							}
						}
					}
			    }
		    }
	    }
	}

	// Make sure we didn't already fail?
	if (!bFailed)
	{
		return true;
	}

	return false;
}


ArrayList* MOG_ControllerPackageMergeNetwork::GetPackageToolEnvironment(String* relativeToolPath, MOG_PackageMerge_PackageFileInfo* packageFileInfo)
{
	// Call our parent
	ArrayList* environment = MOG_ControllerPackageMerge::GetPackageToolEnvironment(relativeToolPath, packageFileInfo);

	// Add in our PackageMergeType
	DosUtils::EnvironmentList_AddVariable(environment, MOG_EnvVar_PackageMergeType, S"Server");

	return environment;
}


PackageList* MOG_ControllerPackageMergeNetwork::ExecuteNetworkPackagingCommands(ArrayList* pPackageCommands)
{
	PackageList* affectedPackageFiles = new PackageList();

	// Loop through all package files and execute their asset's package commands
	for (int p = 0; p < mPackageFiles->Count; p++)
	{
		// Get the package file info
		MOG_PackageMerge_PackageFileInfo* packageFileInfo = __try_cast<MOG_PackageMerge_PackageFileInfo*>(mPackageFiles->Item[p]);

		//Go through all the package commands and execute the ones that apply this package
		for (int i = 0; i < pPackageCommands->Count; i++)
		{
			MOG_DBPackageCommandInfo* pPackageCommand = __try_cast<MOG_DBPackageCommandInfo*>(pPackageCommands->Item[i]);
			
			//Only execute the package commands for assets that are associated with the current package
			if (String::Compare(pPackageCommand->mPackageName, packageFileInfo->mPackageAssetFilename->GetAssetFullName(), true) == 0)
			{
				//Attempt to execute the package commands for this asset and track the affected packages
				PackageList* newlyAffectedPackages = ExecuteNetworkPackageMergeCommand(packageFileInfo, pPackageCommand);
				affectedPackageFiles->Add(newlyAffectedPackages);
			}
		}
	}

	return affectedPackageFiles;
}


PackageList* MOG_ControllerPackageMergeNetwork::PreparePackageMergeTasks(PackageList* packageMergeTasks)
{
	PackageList* preparedPackageTasks = new PackageList();

	// Loop through each task?
	for (int t = 0; t < packageMergeTasks->Count; t++)
	{
		MOG_PackageMerge_PackageFileInfo* packageFileInfo = __try_cast<MOG_PackageMerge_PackageFileInfo*>(packageMergeTasks->Item[t]);

		// Check the PackageStyle?
		if (packageFileInfo->mPackageStyle == MOG_PackageStyle::TaskFile)
		{
			// Make sure our InputPackageTaskFile exists?
			if (DosUtils::FileExistFast(packageFileInfo->mInputPackageTaskFile))
			{
				// Always make sure we indicate this is a Server initiated package merge
				MOG_Ini* packageCommandFile = new MOG_Ini();
				if (packageCommandFile->Open(packageFileInfo->mInputPackageTaskFile, FileShare::ReadWrite))
				{
					// Always make sure we stamp the following settings in the InputPackageTaskFile
					packageCommandFile->PutString(S"Options", S"PackageType", S"Server");
					packageCommandFile->PutString(S"Options", S"Platform", mPlatformName);
					packageCommandFile->Close();

					// Indicate this task was prepared
					preparedPackageTasks->Add(packageFileInfo);
				}
			}
			else
			{
				String* message = String::Concat(	S"System did not locate the necessary InputPackageTaskFile '", packageFileInfo->mInputPackageTaskFile, S"'\n",
													S"TaskFile package merges always require an InputPackageTaskFile.");
				MOG_Report::ReportMessage(S"Bless/Package", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
			}
		}
	}

	return preparedPackageTasks;
}


PackageList* MOG_ControllerPackageMergeNetwork::ProcessAffectedPackages(PackageList* affectedPackageFiles, ArrayList* pPackageCommands, BackgroundWorker* worker)
{
	PackageList* processedPackageFiles = new PackageList();
	bool bFailed = false;

	// Attempt to process each affected package
	for (int p = 0; p < affectedPackageFiles->Count; p++)
	{
		// Get the package file info
		MOG_PackageMerge_PackageFileInfo* packageFileInfo = __try_cast<MOG_PackageMerge_PackageFileInfo*>(affectedPackageFiles->Item[p]);
		//If there's no package working directory, we have no business doing any work on this package
		if (packageFileInfo->mPackageWorkspaceDirectory->Length)
		{
			// Check for disabled packages?
			if (packageFileInfo->mPackageStyle == MOG_PackageStyle::Disabled)
			{
				// Make sure to inform the user about this disabled package
				String* message = String::Concat(	S"The System just encountered an active package assignment to a disabled package.\n",
													S"PACKAGEFILE: ", packageFileInfo->mPackageAssetFilename->GetAssetFullName(), S"\n",
													S"PLATFORM: ", mPlatformName, S"\n",
													S"This is a system warning indicating the package was not modified because it's 'PackageStyle' is disabled.");
				MOG_Report::ReportMessage(S"PackageMerge Warning", message, Environment::StackTrace, MOG_ALERT_LEVEL::ALERT);
				continue;
			}

			// Remove the affected asset's package commands
			RemoveAffectedPackageCommands(packageFileInfo, pPackageCommands);

			// Bless this affected package file
			if (BlessAffectedPackageFile(packageFileInfo, pPackageCommands))
			{
				processedPackageFiles->Add(packageFileInfo);
			}
			else
			{
				// Get list of assets to reject from package commands
				HybridDictionary* assetsToReject = GetAssetsToRejectForFailedPackage(packageFileInfo, pPackageCommands);
				// Enumerate through the post commands
				IDictionaryEnumerator* enumerator = assetsToReject->GetEnumerator();
				while(enumerator->MoveNext())
				{
					MOG_Filename* assetToReject = __try_cast<MOG_Filename*>(enumerator->Value);

					// Make sure to remove this rejected asset from the pending package commands
					MOG_DBPackageCommandAPI::RemovePackageCommand(assetToReject, mJobLabel, MOG_ControllerProject::GetBranchName(), mPlatformName);

					// Check if we have no more pending package commands?
					if (MOG_DBAssetAPI::GetAssetRevisionReferencesInPendingPackageCommands(assetToReject, assetToReject->GetVersionTimeStamp())->Count == 0)
					{
						// Make sure to remove this rejected asset from the upcomming post
						MOG_DBPostCommandAPI::RemovePost(assetToReject, mJobLabel, MOG_ControllerProject::GetBranchName());
					}

					// Check if this package merge has been instructed to reject problem assets
					if (mRejectAssets)
					{
						// Reject the failed asset back to it's creator
						String* comment = String::Concat(	S"Rejected because this asset's package was not properly modified during the network package merge.\n",
															S"PACKAGEFILE: ", packageFileInfo->mPackageAssetFilename->GetAssetFullName(), S"\n");
						if (!RejectFailedAsset(assetToReject, comment, worker))
						{
							// bFailed = true;
						}
					}
				}
			}
		}
	}

	return processedPackageFiles;
}


HybridDictionary* MOG_ControllerPackageMergeNetwork::GetAssetsToRejectForFailedPackage(MOG_PackageMerge_PackageFileInfo* pFailedPackageFileInfo, ArrayList* pPackageCommands)
{
	HybridDictionary* assetsToReject = new HybridDictionary();

	// Scan the list of package commands and reject all the assets associated with adding to this package
	for (int c = 0; c < pPackageCommands->Count; c++)
	{
		MOG_DBPackageCommandInfo* packageCommand = __try_cast<MOG_DBPackageCommandInfo* >(pPackageCommands->Item[c]);

		// Check if this packageCommand is associated with this package?
		if (String::Compare(packageCommand->mPackageName, pFailedPackageFileInfo->mPackageAssetFilename->GetAssetFullName(), true) == 0)
		{
			// Create a temp filename for easier parsing
			MOG_Filename* tempFilename = new MOG_Filename(packageCommand->mAssetName);
			// Get the blessedAssetFilename location of this asset using the temp filename
			MOG_Filename* blessedAssetFilename = MOG_ControllerRepository::GetAssetBlessedVersionPath(tempFilename, packageCommand->mAssetVersion);

			// Add the assetToReject using the asset name as the key...
			// This will essentially replace the earlier version of the asset if more than is listed in the package commands
			// We only care about the last instance of each asset
			assetsToReject->Item[blessedAssetFilename->GetAssetFullName()] = blessedAssetFilename;
		}
	}

	return assetsToReject;
}


PackageList* MOG_ControllerPackageMergeNetwork::GetPackageFilesFromPackageCommands(ArrayList* pPackageCommands)
{
	PackageList* packageFileInfoList = new PackageList();

	// Scan the list of package commands and generate out own list of updated packages
	for (int c = 0; c < pPackageCommands->Count; c++)
	{
		MOG_DBPackageCommandInfo* packageCommand = __try_cast<MOG_DBPackageCommandInfo* >(pPackageCommands->Item[c]);

		switch (packageCommand->mPackageCommandType)
		{
			case MOG_PackageCommand_LateResolver:
			{
				// Either find an already existing PackageFileInfo or create a new one
				MOG_PackageMerge_PackageFileInfo* packageFileInfo = packageFileInfoList->FindByAssetName(packageCommand->mAssetName);
				if (!packageFileInfo)
				{
					packageFileInfo = new MOG_PackageMerge_PackageFileInfo(new MOG_Filename(packageCommand->mAssetName), mWorkspaceDirectory, mPlatformName, packageCommand->mLabel);
					// Make sure we obtained a valid PackageFileInfo by checking the mPackageProperties?
					if (packageFileInfo->mPackageProperties)
					{
						packageFileInfo = VerifyWorkingDirectory(packageFileInfo, false);
						if (packageFileInfo)
						{
							packageFileInfoList->Add(packageFileInfo);
						}
					}
				}
			}
			// Fall through
			default:
			{
				// Either find an already existing PackageFileInfo or create a new one
				MOG_PackageMerge_PackageFileInfo* packageFileInfo = packageFileInfoList->FindByAssetName(packageCommand->mPackageName);
				if (!packageFileInfo)
				{
					packageFileInfo = new MOG_PackageMerge_PackageFileInfo(new MOG_Filename(packageCommand->mPackageName), mWorkspaceDirectory, mPlatformName, packageCommand->mLabel);
					// Make sure we obtained a valid PackageFileInfo by checking the mPackageProperties?
					if (packageFileInfo->mPackageProperties)
					{
						packageFileInfo = VerifyWorkingDirectory(packageFileInfo, false);
						if (packageFileInfo)
						{
							packageFileInfoList->Add(packageFileInfo);
						}
					}
				}
			}
			break;
		}
	}

	return packageFileInfoList;
}


bool MOG_ControllerPackageMergeNetwork::ParseOutputTaskFile_RecordLateResolvers(MOG_PackageMerge_PackageFileInfo* packageFileInfo)
{
	// Make sure there is an output file?
	String* outputTaskFilename = packageFileInfo->mOutputPackageTaskFile;;
	if (DosUtils::FileExistFast(outputTaskFilename))
	{
		// Open the Package.Posting.Info file
		MOG_Ini* outputTaskFile = new MOG_Ini(outputTaskFilename);
		// Build the list of updated packages from the package output file
		if (outputTaskFile->SectionExist(S"LateResolvers"))
		{
			// Scan the listed LateResolvers
			int numLateResolvers = outputTaskFile->CountKeys(S"LateResolvers");
			for (int r = 0; r < numLateResolvers; r++)
			{
				String* lateResolver = outputTaskFile->GetKeyNameByIndexSLOW(S"LateResolvers", r);

				// Breakup the tokens
				String* delimStr = S"{}";
				Char delimiter[] = delimStr->ToCharArray();
				String* parts[] = lateResolver->Split(delimiter);

				// Parse the tokens
				String* delimStr2 = S"\" ";
				Char delimiter2[] = delimStr2->ToCharArray();
				for (int p = 0; p < parts->Count; p += 2)
				{
					// Make sure we have enough parts to parse this token?
					if (p + 1 < parts->Count)
					{
						String* token = parts[p]->Trim(delimiter2);
						String* assetFile = parts[p+1]->Trim(delimiter2);
						// Attempt to map the AssetFile to an AssetName
						ArrayList* assets = MOG_ControllerProject::MapFilenameToAssetName(assetFile, mPlatformName, packageFileInfo->mPackageWorkspaceDirectory);
						if (assets)
						{
							// Create lateresolvers
							for (int a = 0; a < assets->Count; a++)
							{
								MOG_Filename* assetFilename = __try_cast<MOG_Filename* >(assets->Item[a]);

								String* packageAssetName = S"";
								String* packageObjectAssetName = S"";
								String* linkedPackageAssetName =  S"";
								String* linkedPackageObjectAssetName =  S"";

								// Check for the tokens?
								if (String::Compare(token, S"PackageFile", true) == 0)
								{
									packageAssetName = assetFilename->GetAssetFullName();
								}
								else if (String::Compare(token, S"PackageObject", true) == 0)
								{
									packageObjectAssetName = assetFilename->GetAssetFullName();
								}
								else if (String::Compare(token, S"LinkedPackageFile", true) == 0)
								{
									linkedPackageAssetName = assetFilename->GetAssetFullName();
								}
								else if (String::Compare(token, S"LinkedPackageObject", true) == 0)
								{
									linkedPackageObjectAssetName = assetFilename->GetAssetFullName();
								}

								// Add the LateResolver?
								if (packageAssetName->Length &&
									linkedPackageAssetName->Length)
								{
									AddLateResolvers(packageAssetName, linkedPackageAssetName);
								}
							}
						}
// JohnRen - This might be more meaningful in the future but right now, it is pointless
//						else
//						{
//							// Make sure to inform the Server about this error
//							String* message = String::Concat(	S"MOG was unable to locate an asset for '", assetFile, S"'.\n"
//																S"The LateResolver for this asset will not be able to be automatically resolved.");
//							MOG_Report::ReportMessage(S"Late Resolvers", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
//						}
					}
				}
			}
		}

		// Close the package output file
		outputTaskFile->Close();
		outputTaskFile = NULL;
	}

	return true;
}


PackageList* MOG_ControllerPackageMergeNetwork::ExecuteNetworkPackageMergeCommand(MOG_PackageMerge_PackageFileInfo* packageFileInfo, MOG_DBPackageCommandInfo* packageCommand)
{
	PackageList* affectedPackages = NULL;

	// Construct our assigned package property
	ArrayList* packageAssignments = new ArrayList;
	packageAssignments->Add(MOG_PropertyFactory::MOG_Relationships::New_PackageAssignment(packageCommand->mPackageName, packageCommand->mPackageGroups, packageCommand->mPackageObjects));
	
	// Create the package command for this asset for the specified package
	// Check what kind of command we have?
	switch(packageCommand->mPackageCommandType)
	{
	case MOG_PACKAGECOMMAND_TYPE::MOG_PackageCommand_AddAsset:
		{
			MOG_ControllerAsset* asset = NULL;

			try
			{
				// Get the path to the specified blessed asset in the MOG repository
				MOG_Filename* blessedAssetFilename = MOG_ControllerRepository::GetAssetBlessedVersionPath(new MOG_Filename(packageCommand->mAssetName), packageCommand->mAssetVersion);
				// Open the specified asset
				asset = MOG_ControllerAsset::OpenAsset(blessedAssetFilename);
				if (asset)
				{
					affectedPackages = ExecutePackageCommands_AddAsset(asset, packageAssignments);
				}
				else
				{
					// Make sure to inform the Server about this
					String* message = String::Concat(	S"The System was unable to open ", blessedAssetFilename->GetAssetFullName(), S"\n",
														S"Confirm this asset exists in the MOG Repository");
					MOG_Report::ReportMessage(S"ExecuteNetworkPackageMergeCommand", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
				}
			}
			catch(Exception* e)
			{
				MOG_Report::ReportMessage("Critical Exception!", e->Message, e->StackTrace, PROMPT::MOG_ALERT_LEVEL::CRITICAL);
			}
			__finally
			{
				// Always close the asset controller as soon as we are finished with it
				asset->Close();
			}
		}
		break;
	case MOG_PACKAGECOMMAND_TYPE::MOG_PackageCommand_RemoveAsset:
		{
			MOG_ControllerAsset* asset = NULL;

			try
			{
				// Get the path to the specified blessed asset in the MOG repository
				MOG_Filename* blessedAssetFilename = MOG_ControllerRepository::GetAssetBlessedVersionPath(new MOG_Filename(packageCommand->mAssetName), packageCommand->mAssetVersion);
				// Open the specified asset
				asset = MOG_ControllerAsset::OpenAsset(blessedAssetFilename);
				if (asset)
				{
					affectedPackages = ExecutePackageCommands_RemoveAsset(asset, packageAssignments);
				}
				else
				{
					// Make sure to inform the Server about this
					String* message = String::Concat(	S"The System was unable to open ", blessedAssetFilename->GetAssetFullName(), S"\n",
														S"Confirm this asset exists in the MOG Repository");
					MOG_Report::ReportMessage(S"ExecuteNetworkPackageMergeCommand", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
				}
			}
			catch(Exception* e)
			{
				MOG_Report::ReportMessage("Critical Exception!", e->Message, e->StackTrace, PROMPT::MOG_ALERT_LEVEL::CRITICAL);
			}
			__finally
			{
				// Always close the asset controller as soon as we are finished with it
				asset->Close();
			}
		}
		break;
	case MOG_PACKAGECOMMAND_TYPE::MOG_PackageCommand_DeletePackage:
		affectedPackages = ExecutePackageCommands_DeletePackageFile(packageFileInfo);
		break;
	case MOG_PACKAGECOMMAND_TYPE::MOG_PackageCommand_LateResolver:
		affectedPackages = ExecutePackageCommands_LateResolver(packageFileInfo);
		break;
	}

	return affectedPackages;
}


bool MOG_ControllerPackageMergeNetwork::WasPackageFileScheduledForDeletion(MOG_Filename *packageFilename, ArrayList* pPackageCommands)
{
	bool bFound = false;

	// Scan the package commands looking for any delete commands for this
	for (int j = 0; j < pPackageCommands->Count; j++)
	{
		MOG_DBPackageCommandInfo* pCommand = __try_cast<MOG_DBPackageCommandInfo*>(pPackageCommands->Item[j]);

		if (pCommand->mPackageCommandType == MOG_PACKAGECOMMAND_TYPE::MOG_PackageCommand_DeletePackage)
		{
			if (String::Compare(pCommand->mPackageName, packageFilename->GetAssetFullName(), true) == 0)
			{
				bFound = true;
				break;
			}
		}
	}

	return bFound;
}


bool MOG_ControllerPackageMergeNetwork::BlessAffectedPackageFile(MOG_PackageMerge_PackageFileInfo* packageFileInfo, ArrayList* pPackageCommands)
{
	bool bBlessed = false;

	// Make sure the package file exists
	if (DosUtils::FileExistFast(packageFileInfo->mPackageFile))
	{
		// Check if this package was actually changed before we revision it
		FileInfo* fileInfo = DosUtils::FileGetInfo(packageFileInfo->mPackageFile);
		if (DateTime::Compare(fileInfo->LastWriteTime, mStartTime) > 0)
		{
//			// Get the blessed repository name for this new package
//			String* timeStamp = MOG_Time::GetVersionTimestamp(fileInfo->LastWriteTime);
			// Use the timestamp of the job label
			String* timeStamp = MOG_ControllerInbox::ObtainTimestampFromJobLabel(mJobLabel);
			MOG_Filename* newBlessedPackageAssetFilename = new MOG_Filename(MOG_ControllerRepository::GetAssetBlessedVersionPath(packageFileInfo->mPackageAssetFilename, timeStamp));

			// Setup the needed information for the creation of this new package asset
			ArrayList* importFiles = new ArrayList();
			importFiles->Add(packageFileInfo->mPackageFile);
			ArrayList* logFiles = AutoDetectPackageLogFiles(packageFileInfo->mPackageWorkspaceDirectory, DosUtils::PathGetDirectoryPath(packageFileInfo->mPackageFile));

			// Make sure we always stamp this as a known Package
			ArrayList* properties = new ArrayList();
			properties->Add(MOG_PropertyFactory::MOG_Asset_InfoProperties::New_IsPackage(true));

			// Create the new package asset in the MOG Repository
			if (MOG_ControllerAsset::CreateAsset(newBlessedPackageAssetFilename, S"", importFiles, logFiles, properties, false, false))
			{
				MOG_Filename *previousPackageRevision = packageFileInfo->mBlessedPackageFilename;

				// Make sure this package wasn't intentially deleted?
				if (WasPackageFileScheduledForDeletion(packageFileInfo->mBlessedPackageFilename, pPackageCommands))
				{
					// Don't transfer anything from the previous package revision because this must have been a package rebuild and the commands should contain everything we want!
					previousPackageRevision = NULL;
				}
				// Populate the new package revision information from the current revision of this package
				MOG_DBPackageAPI::PopulateNewPackageVersion(previousPackageRevision, newBlessedPackageAssetFilename, pPackageCommands);

				// Setup the bless command for this new package and send it to the server so it can trigger subsequent package merges
				MOG_Command* bless = MOG_CommandFactory::Setup_Bless(newBlessedPackageAssetFilename, mJobLabel, mValidSlaves);
				MOG_ControllerSystem::GetCommandManager()->SendToServer(bless);

				bBlessed = true;
			}
			else
			{
				String* message = String::Concat(	S"The system was unable to create a new revision of this package in the MOG Repository.\n",
													S"PACKAGEFILE: ", packageFileInfo->mRelativePackageFile, S"\n",
													S"PLATFORM: ", mPlatformName, S"\n");
				MOG_Report::ReportMessage(S"Bless/Package", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
			}
		}
	}

	return bBlessed;
}


void MOG_ControllerPackageMergeNetwork::RemoveAffectedPackageCommands(MOG_PackageMerge_PackageFileInfo* packageFileInfo, ArrayList* pPackageCommands)
{
	ArrayList* pRemoveCommands = new ArrayList;
	
	if (packageFileInfo &&
		pPackageCommands && pPackageCommands->Count)
	{
		MOG_Filename* assetName = packageFileInfo->mPackageAssetFilename;
		
		//Go through the list of commands and see if we can find the command that matches this affected package
		for (int j = 0; j < pPackageCommands->Count; j++)
		{
			bool bMatch = false;
			MOG_DBPackageCommandInfo* pCommand = __try_cast<MOG_DBPackageCommandInfo*>(pPackageCommands->Item[j]);

			//LateResolvers can match in 2 places, so check the command type
			switch (pCommand->mPackageCommandType)
			{
			case MOG_PackageCommand_LateResolver:
				if (String::Compare(assetName->GetAssetFullName(), pCommand->mAssetName) == 0)
				{
					bMatch = true;
				}
				break;
			default:
				if (String::Compare(assetName->GetAssetFullName(), pCommand->mPackageName) == 0)
				{
					bMatch = true;
				}
				break;
			}

			if (bMatch)
			{
				//Found it, add it to the list and move on to the next affected file
				pRemoveCommands->Add(pCommand);
			}
		}

		//Send our compiled list of commands to the database for removal
		if (pRemoveCommands && pRemoveCommands->Count)
		{
			if (MOG_DBPackageCommandAPI::RemovePackageCommands(pRemoveCommands))
			{
				MOG_DBPackageCommandAPI::RemoveStalePackageCommands(pRemoveCommands, MOG_ControllerProject::GetBranchName());
			}
			else
			{
				String* message = String::Concat(	S"The system was unable to remove the list of pending package commands\n",
													S"The Database may be down.");
				MOG_Report::ReportMessage(S"Bless/Package", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
			}
		}
	}
}


ArrayList* MOG_ControllerPackageMergeNetwork::GetScheduledNetworkPackageCommands(MOG_Filename* packageAssetFullName, String* jobLabel, String* branchName)
{
	// Get all the assets pending package
	ArrayList* packageCommands = NULL;

	// Check if this there was a specific package specified?
	if (packageAssetFullName->GetFullFilename()->Length)
	{
		// Get all the scheduled package commands only for this specified package
		packageCommands = MOG_DBPackageCommandAPI::GetScheduledPackageCommands(packageAssetFullName->GetFullFilename(), jobLabel, branchName, mPlatformName);
	}
	else
	{
		// Get all scheduled package commands for all packages
		packageCommands = MOG_DBPackageCommandAPI::GetScheduledPackageCommands(jobLabel, branchName, mPlatformName);
	}

	if (packageCommands)
	{
		//Add any associated late resolvers to the list of necessary packages
		ArrayList* lateResolvers = GetAssociatedLateResolvers(packageCommands, branchName);
		if (lateResolvers)
		{
			for (int i = 0; i < lateResolvers->Count; i++)
			{
				packageCommands->Add(lateResolvers->Item[i]);
			}
		}
	}

	return packageCommands;
}

ArrayList* MOG_ControllerPackageMergeNetwork::GetAssociatedLateResolvers(ArrayList* pPackageCommands, String* branchName)
{
	ArrayList* associatedCommands = new ArrayList;

	//Grab all the late resolvers in the database and see if any of them have relationships to the incoming package commands
	ArrayList* lateResolvers = MOG_DBPackageCommandAPI::GetScheduledPackageCommands(S"LateResolver", branchName, mPlatformName);
	if (lateResolvers)
	{
		for (int i = 0; i < lateResolvers->Count; i++)
		{
			MOG_DBPackageCommandInfo* lateCommand = __try_cast<MOG_DBPackageCommandInfo*>(lateResolvers->Item[i]);

			// Make sure this is a MOG_PackageCommand_LateResolver?  It better be because or else our SQL is wrong
			if (lateCommand->mPackageCommandType == MOG_PACKAGECOMMAND_TYPE::MOG_PackageCommand_LateResolver)
			{
				// Check if this command references any of the packages in the current job list
				for (int j = 0; j < pPackageCommands->Count; j++)
				{
					MOG_DBPackageCommandInfo* jobCommand = __try_cast<MOG_DBPackageCommandInfo*>(pPackageCommands->Item[j]);

					if (String::Compare(lateCommand->mPackageName, jobCommand->mPackageName, true) == 0)
					{
						//This late resolver is referenced in the incoming package commands, so add it to the list
						associatedCommands->Add(lateCommand);
						break; //to check the next late resolver
					}
				}
			}
		}
	}

	return associatedCommands;
}


bool MOG_ControllerPackageMergeNetwork::RejectFailedAssets(ArrayList* pPackageCommands, BackgroundWorker* worker)
{
	bool bFailed = false;

	// Loop through all the packageTasks and check for any reported errors
	for (int t = 0; t < mPackageMergeTasks->Count; t++)
	{
		// Get the package file info
		MOG_PackageMerge_PackageFileInfo* packageFileInfo = __try_cast<MOG_PackageMerge_PackageFileInfo*>(mPackageMergeTasks->Item[t]);

		// Make sure there is an output file?
		String* outputTaskFilename = packageFileInfo->mOutputPackageTaskFile;
		if (DosUtils::FileExistFast(outputTaskFilename))
		{
			// Report any package merge errors
			ArrayList* assetFiles = GetErrorsFromPackageCommandsFile(packageFileInfo, true);

			// Scan all the specified assets
			for (int a = 0; a < assetFiles->Count; a++)
			{
				String* assetFile = __try_cast<String* >(assetFiles->Item[a]);

				// Create a temp filename for easier parsing
				MOG_Filename* tempFilename = new MOG_Filename(assetFile);
				// Get the blessedAssetFilename location of this asset using the temp filename
				MOG_Filename* blessedAssetFilename = MOG_ControllerRepository::GetAssetBlessedVersionPath(tempFilename, tempFilename->GetVersionTimeStamp());

				// Make sure to remove this rejected asset from the pending package commands
				MOG_DBPackageCommandAPI::RemovePackageCommand(blessedAssetFilename, mJobLabel, MOG_ControllerProject::GetBranchName(), mPlatformName);

				// Check if we have no more pending package commands?
				if (MOG_DBAssetAPI::GetAssetRevisionReferencesInPendingPackageCommands(blessedAssetFilename, blessedAssetFilename->GetVersionTimeStamp())->Count == 0)
				{
					// Make sure to remove this rejected asset from the upcomming post
					MOG_DBPostCommandAPI::RemovePost(blessedAssetFilename, mJobLabel, MOG_ControllerProject::GetBranchName());
				}

				// Check if this package merge has been instructed to reject problem assets
				if (mRejectAssets)
				{
					// Reject the failed asset back to it's creator
					String* comment = S"Asset was rejected due to a network package merge failure.";
					if (!RejectFailedAsset(blessedAssetFilename, comment, worker))
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

bool MOG_ControllerPackageMergeNetwork::RejectFailedAsset(MOG_Filename* blessedAssetFilename, String* comment, BackgroundWorker* worker)
{
	bool bFailed = false;

	// Check if this blessed asset's revision was generated in conjunction with our current job's timestamp?
	if (String::Compare(blessedAssetFilename->GetVersionTimeStamp(), MOG_ControllerInbox::ObtainTimestampFromJobLabel(mJobLabel), true) == 0)
	{
		// Reject the failed asset back to it's creator
		if (!MOG_ControllerInbox::Reject(blessedAssetFilename, comment, MOG_AssetStatusType::PackageError, worker))
		{
			bFailed = true;
		}
	}

	if (!bFailed)
	{
		return true;
	}
	return false;
}

bool MOG_ControllerPackageMergeNetwork::SchedulePlatformSpecificAssetAddition(MOG_ControllerAsset* platformSpecificAsset, String* jobLabel, String* validSlaves)
{
	return MOG_ControllerPackageMergeNetwork::ScheduleGenericAssetPackageCommands(platformSpecificAsset, MOG_PACKAGECOMMAND_TYPE::MOG_PackageCommand_RemoveAsset, jobLabel, validSlaves);
}

bool MOG_ControllerPackageMergeNetwork::SchedulePlatformSpecificAssetRemoval(MOG_ControllerAsset* platformSpecificAsset, String* jobLabel, String* validSlaves)
{
	return MOG_ControllerPackageMergeNetwork::ScheduleGenericAssetPackageCommands(platformSpecificAsset, MOG_PACKAGECOMMAND_TYPE::MOG_PackageCommand_AddAsset, jobLabel, validSlaves);
}

bool MOG_ControllerPackageMergeNetwork::ScheduleGenericAssetPackageCommands(MOG_ControllerAsset* platformSpecificAsset, MOG_PACKAGECOMMAND_TYPE packageCommandType, String* jobLabel, String* validSlaves)
{
	MOG_Filename* platformSpecificAssetFilename = platformSpecificAsset->GetAssetFilename();

	// Make sure this asset is platform specific?
	if (platformSpecificAssetFilename->IsPlatformSpecific())
	{
		// Create generic version of this platform-specific asset
		MOG_Filename* genericAssetFilename = MOG_Filename::CreateAssetName(platformSpecificAssetFilename->GetAssetClassification(), S"All", platformSpecificAssetFilename->GetAssetLabel());
		// Check if this generic asset exists in the project?
		if (MOG_ControllerProject::DoesAssetExists(genericAssetFilename))
		{
			// Open the generic version of this asset
			MOG_Filename* blessedGenericAssetFilename = MOG_ControllerProject::GetAssetCurrentBlessedVersionPath(genericAssetFilename);
			MOG_ControllerAsset* genericAsset = MOG_ControllerAsset::OpenAsset(blessedGenericAssetFilename);
			if (genericAsset)
			{
				// Get the generic asset's properties
				MOG_Properties* genericProperties = genericAsset->GetProperties();
				// Does this asset need to have it's files packaged?
				if (genericProperties->IsPackagedAsset)
				{
					ArrayList* packageAssignments = genericProperties->GetPackages();
					// Walk through all the assigned packages
					for (int packageIndex = 0; packageIndex < packageAssignments->Count; packageIndex++)
					{
						MOG_Property* packageAssignmentProperty = __try_cast<MOG_Property*>(packageAssignments->Item[packageIndex]);
						MOG_Filename* packageName = new MOG_Filename(MOG_ControllerPackage::GetPackageName(packageAssignmentProperty->mPropertyKey));
						bool bScheduleCommand = false;

						// Check if this package assignment is platform specific?  and
						// Check if this package assignment's platform matches this platform-specific asset's platform?
						if (packageName->IsPlatformSpecific() &&
							String::Compare(packageName->GetAssetPlatform(), platformSpecificAssetFilename->GetAssetPlatform(), true) == 0)
						{
							// Indicate we should schedule this package command because it matched this platform
							bScheduleCommand = true;
						}
						else
						{
							// Check if a platform-specific package exists for this generic package assignment?
							if (MOG_ControllerProject::DoesPlatformSpecificAssetExists(packageName, platformSpecificAssetFilename->GetAssetPlatform()))
							{
								// Generate the platform-specific package name
								packageName = MOG_Filename::CreateAssetName(packageName->GetAssetClassification(), platformSpecificAssetFilename->GetAssetPlatform(), packageName->GetAssetLabel());
								// Indicate we should schedule the package command for this platform-specific package
								bScheduleCommand = true;
							}
						}

						// Check if we determined whether or not to schedule a package command for this package?
						if (bScheduleCommand)
						{
							// Make sure this Package exists in our project?
							if (MOG_ControllerProject::DoesAssetExists(packageName))
							{
								// Get the PackageFileInfo
								MOG_PackageMerge_PackageFileInfo* packageFileInfo = new MOG_PackageMerge_PackageFileInfo(packageName, S"", S"", jobLabel);
								// Make sure we obtained a valid PackageFileInfo by checking the mPackageProperties?
								if (packageFileInfo->mPackageProperties)
								{
									// Schedule this asset for removal from this package because the platform-specific version is comming in its place
									MOG_ControllerPackageMergeNetwork::ScheduleAssetPackageCommands(genericAsset, packageCommandType, jobLabel, validSlaves, packageFileInfo, false);
								}
							}
						}
					}
				}

				// Make sure we close the asset controller
				genericAsset->Close();
			}
		}
	}

	return true;
}

