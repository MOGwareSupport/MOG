//--------------------------------------------------------------------------------
//	MOG_ControllerPackageMergeLocal.cpp
//	
//	
//--------------------------------------------------------------------------------

#include "StdAfx.h"

#include "MOG_Define.h"
#include "MOG_DosUtils.h"
#include "MOG_ControllerPackage.h"
#include "MOG_ControllerSystem.h"
#include "MOG_ControllerProject.h"
#include "MOG_ControllerRepository.h"
#include "MOG_ControllerInbox.h"
#include "MOG_DBInboxAPI.h"
#include "MOG_DBSyncedDataAPI.h"
#include "MOG_EnvironmentVars.h"

#include "MOG_ControllerPackageMergeLocal.h"

MOG_ControllerPackageMergeLocal::MOG_ControllerPackageMergeLocal()
{
	mWorkspaceDirectory = MOG_ControllerProject::GetWorkspaceDirectory();
	mPlatformName = S"All";
	mPackageMergeTasks = new PackageList;
	mPackagesToRemove = new ArrayList;
	mAssetsToAdd = new ArrayList();
	mAssetsToRemove = new ArrayList();
	mWarnUnmodifiedPackages = true;
}

PackageList *MOG_ControllerPackageMergeLocal::DoPackageEvent(String *workspaceDirectory, String *platformName, BackgroundWorker* worker)
{
	mWorkspaceDirectory = workspaceDirectory;
	mPlatformName = platformName;
	PackageList* affectedPackageFiles = new PackageList;

	// Obtain the list of local assets
	if (DetectLocalAssetsToMerge())
	{
		// Get the list of packages referenced in this list of assets
		mPackageFiles = new PackageList;
		mPackageFiles->Add(GetPackageInfoListFromAssets(mAssetsToAdd));
		mPackageFiles->Add(GetPackageInfoListFromAssets(mAssetsToRemove));
		if (mPackageFiles->Count)
		{
			// Perform the package merge
			affectedPackageFiles = PerformLocalPackageMerge(worker);
		}
		else
		{
			// Report an error back to the user that no package assignments were detected
			String *message = String::Concat(	S"No actual package assignments were detected in the pending assets.\n",
												S"Please confirm that the pending assets have valid package assignments.");
			MOG_Report::ReportMessage(S"Local Package", message, Environment::StackTrace, MOG_ALERT_LEVEL::ALERT);

			// Set the 'PackageError' status on all assets
			MarkedFailedAssets(mAssetsToAdd);
			MarkedFailedAssets(mAssetsToRemove);
		}
	}

	return affectedPackageFiles;
}


PackageList *MOG_ControllerPackageMergeLocal::PerformLocalPackageMerge(BackgroundWorker* worker)
{
	PackageList* newlyAffectedPackageFiles = new PackageList();
	PackageList* affectedPackageFiles = new PackageList();
	bool bFailed = false;

	// Make sure all the working directories are valid
	mPackageFiles = VerifyWorkingDirectories(mPackageFiles);
	// First clean the working directories
	CleanWorkingDirectories(mPackageFiles);
	// Prepare the working directories
	PrepareWorkingDirectories(mPackageFiles);

	// Execute any pre package merge commands
	ExecutePreMergeEvents(mPackageFiles);

	// Execute the package merge commands for each asset tracking the affected packages
	affectedPackageFiles->Add(ExecuteLocalPackagingCommands(mAssetsToRemove));
	affectedPackageFiles = OptimizeAffectedPackageFilesPendingRemoval(affectedPackageFiles);
	affectedPackageFiles->Add(ExecuteLocalPackagingCommands(mAssetsToAdd));

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
	}

	// Execute any post package merge commands
	ExecutePostMergeEvents(affectedPackageFiles);

	// Process affected packages
	PackageList *processedPackageFiles = ProcessAffectedPackages(affectedPackageFiles, worker);

	// Update Inbox status and delete packaged assets that were removed
	FinalizeLocalAssets(mAssetsToRemove);
	FinalizeLocalAssets(mAssetsToAdd);

	// Clean the working directories
	CleanWorkingDirectories(affectedPackageFiles);

	// perform nested packaging for packaged packages
	newlyAffectedPackageFiles = PackagePackagedPackages(processedPackageFiles, worker);
	affectedPackageFiles->Add(newlyAffectedPackageFiles);

	return affectedPackageFiles;
}

bool MOG_ControllerPackageMergeLocal::DetectLocalAssetsToMerge()
{
	String *workspaceDirectory = mWorkspaceDirectory;

	ArrayList *localAssets = MOG_DBInboxAPI::InboxGetLocalAssetList(workspaceDirectory);
	if (localAssets)
	{
		// Build our text strings for the various status types
		String* textPackaging = MOG_AssetStatus::GetText(MOG_AssetStatusType::Packaging);
		String* textRepackaging = MOG_AssetStatus::GetText(MOG_AssetStatusType::Repackaging);
		String* textUnpackaging = MOG_AssetStatus::GetText(MOG_AssetStatusType::Unpackaging);

		// Walk the list of local assets
		for (int a = 0; a < localAssets->Count; a++)
		{
			MOG_DBInboxProperties *inboxProp = __try_cast<MOG_DBInboxProperties*>(localAssets->Item[a]);
			MOG_Properties *properties = new MOG_Properties(inboxProp->mProperties);
			MOG_Filename *assetFilename = inboxProp->mAsset;

			// Check for any assets needing to be added?
			if (String::Compare(properties->Status, textPackaging, true) == 0 ||
				String::Compare(properties->Status, textRepackaging, true) == 0)
			{
				// Retain this asset for packaging
				mAssetsToAdd->Add(assetFilename);
			}
			// Check for any assets needing to be removed?
			else if (String::Compare(properties->Status, textUnpackaging, true) == 0)
			{
				// Retain this asset for packaging
				mAssetsToRemove->Add(assetFilename);

				// Keep track of all the packages we are removing
				if (properties->IsPackage)
				{
					mPackagesToRemove->Add(assetFilename);
				}
			}
		}
	}

	return (mAssetsToAdd->Count || mAssetsToRemove->Count);
}


PackageList *MOG_ControllerPackageMergeLocal::GetPackageInfoListFromAsset(MOG_Filename *assetFilename)
{
	PackageList *assignedPackageFiles = new PackageList();

	// Obtain the properties the fast way w/o having to open the asset
	MOG_Properties *assetProperties = new MOG_Properties(assetFilename);
	assetProperties->SetScope(mPlatformName);
	assignedPackageFiles->AddRange(GetPackageFilesFromPackageAssignmentProperties(assetProperties->GetApplicablePackages(), assetProperties, S""));

	// Check if this asset has something sitting in the remove tray?
	MOG_Filename* removeTrayAssetFilename = MOG_Filename::GetLocalRemoveTrayFilename(mWorkspaceDirectory, assetFilename);
	if (DosUtils::DirectoryExistFast(removeTrayAssetFilename->GetEncodedFilename()))
	{
		// As painful as it is, we need to open the asset to get it's properties directly from the asset because 
		// this is sitting in the remove tray and the database doesn't know anything about this asset
		MOG_ControllerAsset *asset = MOG_ControllerAsset::OpenAsset(removeTrayAssetFilename);
		if (asset)
		{
			try
			{
				MOG_Properties* properties = asset->GetProperties(mPlatformName);
				assignedPackageFiles->AddRange(GetPackageFilesFromPackageAssignmentProperties(properties->GetApplicablePackages(), properties, S""));
			}
			__finally
			{
				asset->Close();
			}
		}
	}

	return assignedPackageFiles;
}


PackageList *MOG_ControllerPackageMergeLocal::GetPackageInfoListFromAssets(ArrayList* assets)
{
	// Build the complete list of packages referenced in this list of assets
	PackageList *packageFiles = new PackageList();

	if (assets)
	{
		for (int a = 0; a < assets->Count; a++)
		{
			MOG_Filename *assetFilename = __try_cast<MOG_Filename*>(assets->Item[a]);

			packageFiles->Add(GetPackageInfoListFromAsset(assetFilename));
		}
	}

	return packageFiles;
}


bool MOG_ControllerPackageMergeLocal::CleanWorkingDirectories_EntireWorkspace(MOG_PackageMerge_PackageFileInfo *packageFileInfo)
{
	// Get the current local workspace
	if (mWorkspaceDirectory->Length)
	{
		// Make sure this is not the same as our local workspace directory?
		if (!packageFileInfo->mPackageWorkspaceDirectory->ToLower()->StartsWith(mWorkspaceDirectory->ToLower()))
		{
			// Allow our parent to do its stuff
			return MOG_ControllerPackageMerge::CleanWorkingDirectories_EntireWorkspace(packageFileInfo);
		}
	}

	return true;
}


bool MOG_ControllerPackageMergeLocal::PrepareWorkingDirectories(PackageList *packageFiles)
{
	String *workspaceDirectory = mWorkspaceDirectory;
	bool bFailed = false;

	// Prepare for the upcomming package merge
	for (int p = 0; p < packageFiles->Count; p++)
	{
		// Get the package file info
		MOG_PackageMerge_PackageFileInfo *packageFileInfo = __try_cast<MOG_PackageMerge_PackageFileInfo*>(packageFiles->Item[p]);
		//If there's no package working directory, we have no business doing any work on this package
		if (packageFileInfo->mPackageWorkspaceDirectory->Length)
		{
			// Check if the specified package working directory of this package is different than our workspace?
			if (String::Compare(workspaceDirectory, packageFileInfo->mPackageWorkspaceDirectory, true) != 0)
			{
				// Copy the local workspace files into their working directories
				String *sourcePackageFile = String::Concat(workspaceDirectory, S"\\", packageFileInfo->mRelativePackageFile);
				// Check if we are missing this file in our local workspace?
				if (!DosUtils::FileExistFast(sourcePackageFile))
				{
					String *processedDirectory = MOG_ControllerAsset::GetAssetProcessedDirectory(packageFileInfo->mPackageProperties, mPlatformName);
					sourcePackageFile = String::Concat(processedDirectory, S"\\", DosUtils::PathGetDirectoryPath(packageFileInfo->mRelativePackageFile));
				}
				// Check if the source file exists?
				if (DosUtils::FileExistFast(sourcePackageFile))
				{
					String *targetPackageFile = packageFileInfo->mPackageFile;
					if (!DosUtils::FileCopyFast(sourcePackageFile, targetPackageFile, true))
					{
						String *message = String::Concat(	S"System failed to prepare the PackageWorkspaceDirectory because it couldn't copy the PackageFile from the current workspace.\n",
															S"PACKAGEFILE: ", sourcePackageFile, S"\n",
															S"PLATFORM: ", mPlatformName, S"\n",
															S"This may be caused by a possible file sharing violation.");
						MOG_Report::ReportMessage(S"Local Package", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
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


PackageList *MOG_ControllerPackageMergeLocal::OptimizeAffectedPackageFilesPendingRemoval(PackageList* affectedPackageFiles)
{
	// Walk through all the listed affectedPackageFiles looking for packages scheduled for removal
	// We can't auto increment the counter becasue we are dynamically removing items from the array
	for (int p = 0; p < affectedPackageFiles->Count; /*p++*/)
	{
		MOG_PackageMerge_PackageFileInfo *affectedPackageFile = __try_cast<MOG_PackageMerge_PackageFileInfo*>(affectedPackageFiles->Item[p]);

		// Scan mPackagesToRemove
		bool bRemovedPackage = false;
		for (int i = 0; i < mPackagesToRemove->Count; i++)
		{
			MOG_Filename* packageName = __try_cast<MOG_Filename*>(mPackagesToRemove->Item[i]);
			if (packageName)
			{
				if (String::Compare(packageName->GetAssetFullName(), affectedPackageFile->mPackageAssetFilename->GetAssetFullName(), true) == 0)
				{
					// Remove the package from the list of affectedPackageFiles
					affectedPackageFiles->RemoveAt(p);
					bRemovedPackage = true;
					break;
				}
			}
		}

		// Check if we didn't remove the package?
		if (!bRemovedPackage)
		{
			//increment our counter here after we've determined we aren't removing it from the array
			p++;
		}
	}

	return affectedPackageFiles;
}

PackageList *MOG_ControllerPackageMergeLocal::ExecuteLocalPackagingCommands(ArrayList* assetsToMerge)
{
	PackageList *affectedPackageFiles = new PackageList();
	bool bFailed = false;

	// Loop through all the local assets looking for 'Unpackaged' assets
	for (int a = 0; a < assetsToMerge->Count; a++)
	{
		MOG_Filename *assetFilename = __try_cast<MOG_Filename*>(assetsToMerge->Item[a]);

		//Attempt to execute the package commands for this asset and track the affected packages
		PackageList *newlyAffectedPackages = ExecuteLocalPackageMergeCommand(assetFilename);
		affectedPackageFiles->Add(newlyAffectedPackages);
	}

	return affectedPackageFiles;
}

PackageList *MOG_ControllerPackageMergeLocal::PreparePackageMergeTasks(PackageList *packageMergeTasks)
{
	PackageList *preparedPackageTasks = new PackageList();

	// Loop through each task?
	for (int t = 0; t < packageMergeTasks->Count; t++)
	{
		MOG_PackageMerge_PackageFileInfo *packageFileInfo = __try_cast<MOG_PackageMerge_PackageFileInfo*>(packageMergeTasks->Item[t]);

		// Check the PackageStyle?
		if (packageFileInfo->mPackageStyle == MOG_PackageStyle::TaskFile)
		{
			// Make sure our InputPackageTaskFile exists?
			if (DosUtils::FileExistFast(packageFileInfo->mInputPackageTaskFile))
			{
				// Always make sure we indicate this is a Server initiated package merge
				MOG_Ini *packageCommandFile = new MOG_Ini();
				if (packageCommandFile->Open(packageFileInfo->mInputPackageTaskFile, FileShare::ReadWrite))
				{
					// Always make sure we stamp the following settings in the InputPackageTaskFile
					packageCommandFile->PutString(S"Options", S"PackageType", S"Local");
					packageCommandFile->PutString(S"Options", S"Platform", mPlatformName);
					packageCommandFile->Close();

					// Indicate this task was prepared
					preparedPackageTasks->Add(packageFileInfo);
				}
			}
			else
			{
				String *message = String::Concat(	S"System did not locate the necessary InputPackageTaskFile '", packageFileInfo->mInputPackageTaskFile, S"'\n",
													S"TaskFile package merges always require an InputPackageTaskFile.");
				MOG_Report::ReportMessage(S"Bless/Package", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
			}
		}
	}

	return preparedPackageTasks;
}


bool MOG_ControllerPackageMergeLocal::ParseOutputTaskFile_ReportErrors(MOG_PackageMerge_PackageFileInfo* packageFileInfo)
{
	bool bFailed = false;

	// Make sure there is an output file?
	String *outputTaskFilename = packageFileInfo->mOutputPackageTaskFile;;
	if (DosUtils::FileExistFast(outputTaskFilename))
	{
		// Report any package merge errors
		ArrayList *assetFiles = GetErrorsFromPackageCommandsFile(packageFileInfo, true);
		// Scan all the specified assets
		for (int a = 0; a < assetFiles->Count; a++)
		{
			String *assetFile = __try_cast<String *>(assetFiles->Item[a]);
			ArrayList *assetNames = MOG_ControllerProject::MapFilenameToAssetName(assetFile, mPlatformName, packageFileInfo->mPackageWorkspaceDirectory);
			if (assetNames && assetNames->Count)
			{
				// Always use the first asset name
				MOG_Filename *matchedAssetName = __try_cast<MOG_Filename*>(assetNames->Item[0]);
				MOG_ControllerProject::MapFilenameToAssetName_WarnAboutAmbiguousMatches(assetFile, assetNames);

				MOG_Filename *assetFilename = LocateAsset_LocalWorkspaceOnly(matchedAssetName);
				if (assetFilename)
				{
					// Check if this asset is local?
					if (assetFilename->IsLocal())
					{
						// Open the specified asset?
						MOG_ControllerAsset *asset = MOG_ControllerAsset::OpenAsset(assetFilename);
						if (asset)
						{
							// Set the appropriate failed state
							MOG_ControllerInbox::UpdateInboxView(asset, MOG_AssetStatusType::PackageError);
							asset->Close();
						}
					}
				}
			}
			else
			{
				// Warn the user a bad assetfile was specified in the done output file
				String *message = String::Concat(	S"System was unable to match '", assetFile, S"' listed in the package merge output file.\n",
													S"MOG suspects the wrong information was reported by the packaging tool.  Please Contact your MOG Administrator.");
				MOG_Prompt::PromptMessage(S"PackageMergeEvent", message, Environment::StackTrace, MOG_ALERT_LEVEL::ALERT);
				bFailed = true;
			}
		}
	}

	if (!bFailed)
	{
		return true;
	}
	return false;
}


PackageList *MOG_ControllerPackageMergeLocal::ProcessAffectedPackages(PackageList *affectedPackageFiles, BackgroundWorker* worker)
{
	PackageList *packagedPackages = new PackageList();
	String *workspaceDirectory = mWorkspaceDirectory;
	PackageList *failedPackages = new PackageList();
	bool bFailed = false;

	// Clean up after the package merge
	for (int p = 0; p < affectedPackageFiles->Count; p++)
	{
		// Get the package file info
		MOG_PackageMerge_PackageFileInfo *packageFileInfo = __try_cast<MOG_PackageMerge_PackageFileInfo*>(affectedPackageFiles->Item[p]);
		//If there's no package working directory, we have no business doing any work on this package
		if (packageFileInfo->mPackageWorkspaceDirectory->Length)
		{
			// Check for disabled packages?
			if (packageFileInfo->mPackageStyle == MOG_PackageStyle::Disabled)
			{
				// Make sure to inform the user about this disabled package
				String *message = String::Concat(	S"The System just encountered an active package assignment to a disabled package.\n",
													S"PACKAGEFILE: ", packageFileInfo->mPackageAssetFilename->GetAssetFullName(), S"\n",
													S"PLATFORM: ", mPlatformName, S"\n",
													S"This is a system warning indicating the package was not modified because it's 'PackageStyle' is disabled.");
				MOG_Report::ReportMessage(S"PackageMerge Warning", message, Environment::StackTrace, MOG_ALERT_LEVEL::ALERT);

				// Add this package to our list of failed packages
				failedPackages->Add(packageFileInfo);

				continue;
			}

			// Move the new PackageFile from the PackageWorkspaceDirectory back into the local workspace
			String *sourcePackageFile = packageFileInfo->mPackageFile;
			String *targetPackageFile = String::Concat(workspaceDirectory, S"\\", packageFileInfo->mRelativePackageFile);
			// Make sure the packageFile exists?
			if (DosUtils::FileExistFast(sourcePackageFile))
			{
				// Validate this package as being modified
				FileInfo *fileInfo = DosUtils::FileGetInfo(sourcePackageFile);
				if (DateTime::Compare(fileInfo->LastWriteTime, mStartTime) > 0)
				{
					// Check if the specified package working directory of this package is different than our workspace?
					if (String::Compare(workspaceDirectory, packageFileInfo->mPackageWorkspaceDirectory, true) != 0)
					{
						if (!DosUtils::FileMoveFast(sourcePackageFile, targetPackageFile, true))
						{
							String *message = String::Concat(	S"System failed to move the new PackageFile from the PackageWorkspaceDirectory back into the current workspace.\n",
																S"PACKAGEFILE: ", sourcePackageFile, S"\n",
																S"PLATFORM: ", mPlatformName, S"\n",
																S"This may be caused by a possible file sharing violation.");
							MOG_Report::ReportMessage(S"Local Package", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);

							// Add this package to our list of failed packages
							failedPackages->Add(packageFileInfo);

							bFailed = true;
						}
					}

					// Make sure we haven't failed?
					if (!bFailed)
					{
						// Check if this package is a packaged package?
						if (packageFileInfo->mPackageProperties->IsPackagedAsset)
						{
							// Create an instance of this package in our local workspace's updated assets tray
							MOG_Filename *updatedAssetFilename = MOG_Filename::GetLocalUpdatedTrayFilename(workspaceDirectory, packageFileInfo->mPackageAssetFilename);
							// Setup the needed information for the creation of this new package asset
							ArrayList *importFiles = new ArrayList();
//?	MOG_ControllerPackageMergeLocal::ProcessAffectedPackages - This would be faster if we didn't specify the import file.  We need to fixup MOG_ControllerAsset so we don't need to copy the package file again when creating a local workspace asset.
							importFiles->Add(targetPackageFile);
							ArrayList *logFiles = AutoDetectPackageLogFiles(packageFileInfo->mPackageWorkspaceDirectory, DosUtils::PathGetDirectoryPath(packageFileInfo->mPackageFile));

							// Always build this list of properties
							ArrayList *properties = new ArrayList();
							properties->Add(MOG_PropertyFactory::MOG_Asset_InfoProperties::New_IsPackage(true));
							properties->Add(MOG_PropertyFactory::MOG_Asset_StatsProperties::New_CreatedTime(MOG_Time::GetVersionTimestamp()));

							// Create the new package asset in the local workspace
							if (MOG_ControllerAsset::CreateAsset(updatedAssetFilename, S"", importFiles, logFiles, properties, false, false))
							{
								packagedPackages->Add(packageFileInfo);
							}
						}
					}
				}
				else
				{
					//We don't save the package in editor mode, so check before warning
					if (mWarnUnmodifiedPackages)
					{
						// Warn the user about this package has not being modified
						String *message = String::Concat(	S"The System detected a package that should have been merged yet it appears the package was not modified during the package merge command.\n",
															S"PACKAGEFILE: ", sourcePackageFile, S"\n",
															S"PLATFORM: ", mPlatformName, S"\n",
															S"The System suspects the PackageTools failed.  Confirm the seriousness of this error with your MOG Administrator.");
						MOG_Report::ReportMessage(S"Bless/Package", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);

						// Add this package to our list of failed packages
						failedPackages->Add(packageFileInfo);
	
						bFailed = true;
					}
				}
			}
			else
			{
				//We don't save the package in editor mode, so check before warning
				if (mWarnUnmodifiedPackages)
				{
					String *message = String::Concat(	S"System failed to locate the new PackageFile in the PackageWorkspaceDirectory.\n",
														S"PACKAGEFILE: ", sourcePackageFile, S"\n",
														S"PLATFORM: ", mPlatformName, S"\n",
														S"The System suspects the PackageTools failed.  Confirm the seriousness of this error with your MOG Administrator.");
					MOG_Report::ReportMessage(S"Local Package", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);

					// Add this package to our list of failed packages
					failedPackages->Add(packageFileInfo);

					bFailed = true;
				}
			}
		}
	}

	// Check if we had any failed packages?
	if (failedPackages->Count)
	{
		// Set the 'PackageError' status on all assets associated with our failed packages
		MarkedFailedAssets(failedPackages, mAssetsToAdd);
	}

	return packagedPackages;
}


void MOG_ControllerPackageMergeLocal::MarkedFailedAssets(ArrayList *assets)
{
	// Loop through all the specified assets
	for (int a = 0; a < assets->Count; a++)
	{
		MOG_Filename *assetFilename = __try_cast<MOG_Filename*>(assets->Item[a]);

		// Open the specified asset
		MOG_ControllerAsset *asset = MOG_ControllerAsset::OpenAsset(assetFilename);
		if (asset)
		{
			// Change the asset's status to 'PackageError'
			MOG_ControllerInbox::UpdateInboxView(asset, MOG_AssetStatusType::PackageError);
			asset->Close();
		}
	}
}


void MOG_ControllerPackageMergeLocal::MarkedFailedAssets(PackageList* failedPackages, ArrayList *assets)
{
	// Loop through all the specified assets
	for (int a = 0; a < assets->Count; a++)
	{
		MOG_Filename *assetFilename = __try_cast<MOG_Filename*>(assets->Item[a]);

		// Get the packages associated with this asset
		PackageList *associatedPackages = GetPackageInfoListFromAsset(assetFilename);

		// Loop through our list of failed packages
		for (int p = 0; p < failedPackages->Count; p++)
		{
			MOG_PackageMerge_PackageFileInfo *failedPackageInfo = __try_cast<MOG_PackageMerge_PackageFileInfo*>(failedPackages->Item[p]);

			// Check if this failedPackage is associated with this asset?
			if (associatedPackages->FindByFileName(failedPackageInfo->mPackageFile) != NULL)
			{
				// Open the specified asset
				MOG_ControllerAsset *asset = MOG_ControllerAsset::OpenAsset(assetFilename);
				if (asset)
				{
					// Change the asset's status to 'PackageError'
					MOG_ControllerInbox::UpdateInboxView(asset, MOG_AssetStatusType::PackageError);
					asset->Close();
				}
			}
		}
	}
}


ArrayList *MOG_ControllerPackageMergeLocal::GetPackageToolEnvironment(String *relativeToolPath, MOG_PackageMerge_PackageFileInfo* packageFileInfo)
{
	// Call our parent
	ArrayList *environment = MOG_ControllerPackageMerge::GetPackageToolEnvironment(relativeToolPath, packageFileInfo);

	// Add in our PackageMergeType
	DosUtils::EnvironmentList_AddVariable(environment, MOG_EnvVar_PackageMergeType, S"Local");

	return environment;
}


PackageList *MOG_ControllerPackageMergeLocal::PackagePackagedPackages(PackageList *processedPackageFiles, BackgroundWorker* worker)
{
	ArrayList *packagedPackageFiles = new ArrayList();
	PackageList *affectedPackageFiles = new PackageList();

	// Prepare for the upcomming package merge
	for (int p = 0; p < processedPackageFiles->Count; p++)
	{
		// Get the package file info
		MOG_PackageMerge_PackageFileInfo *packageFileInfo = __try_cast<MOG_PackageMerge_PackageFileInfo*>(processedPackageFiles->Item[p]);

		MOG_Filename *localPackageAssetFilename = LocateAsset_LocalWorkspaceOnly(packageFileInfo->mPackageAssetFilename);
		if (localPackageAssetFilename)
		{
			// Open the specified local asset?
			MOG_ControllerAsset *asset = MOG_ControllerAsset::OpenAsset(localPackageAssetFilename);
			if (asset)
			{
				// Check if this asset is a packacked asset?
				if (asset->GetProperties()->IsPackagedAsset)
				{
					// Mark this asset's status as 'Unpackaged' so it will be packaged on our next recursion
					MOG_ControllerInbox::UpdateInboxView(asset, MOG_AssetStatusType::Unpackaged);

					// Add this packaged package to our list
					packagedPackageFiles->Add(localPackageAssetFilename);
				}
				asset->Close();
			}
		}
	}

	// Check if we had any packaged packages?
	if (packagedPackageFiles->Count)
	{
		// Recursively call ourself so these packaged packages will be packaged
		MOG_ControllerPackageMergeLocal* pSubsequentLocalMerge = new MOG_ControllerPackageMergeLocal();
		affectedPackageFiles = pSubsequentLocalMerge->DoPackageEvent(mWorkspaceDirectory, mPlatformName, worker);
	}

	return affectedPackageFiles;
}


PackageList *MOG_ControllerPackageMergeLocal::ExecuteLocalPackageMergeCommand(MOG_Filename *assetFilename)
{
	PackageList *affectedPackageFiles = new PackageList();
	bool bFailed = false;

	// Open the specified asset
	MOG_ControllerAsset *asset = MOG_ControllerAsset::OpenAsset(assetFilename);
	if (asset)
	{
		try
		{
			MOG_Properties *assetProperties = asset->GetProperties(mPlatformName);

			// Check if this asset needs to be unpackaged and removed?
			if (String::Compare(assetProperties->Status, MOG_AssetStatus::GetText(MOG_AssetStatusType::Unpackaging), true) == 0)
			{
				MOG_ControllerAsset *restoreAsset = NULL;

				// Check if this asset was packaged?
				if (assetProperties->IsPackagedAsset)
				{
					// Remove the old binary files from the packages
					PackageList *newlyAffectedPackages = MOG_ControllerPackageMerge::ExecutePackageCommands_RemoveAsset(asset);
					affectedPackageFiles->Add(newlyAffectedPackages);
				}

				// Check if the asset already existed in the LocalBranch?
				String *revision = MOG_DBSyncedDataAPI::GetLocalAssetVersion(assetFilename, mWorkspaceDirectory, mPlatformName);
				if (revision->Length)
				{
					// Build the blessedAssetFilename
					MOG_Filename *blessedAssetFilename = new MOG_Filename(MOG_ControllerRepository::GetAssetBlessedVersionPath(assetFilename, revision));
					// Open the oldAssetRevision from the repository
					restoreAsset = MOG_ControllerAsset::OpenAsset(blessedAssetFilename);
					if (restoreAsset)
					{
						// Get the properties for the addAsset
						MOG_Properties *restoreAssetProperties = restoreAsset->GetProperties(mPlatformName);
						// Check if this asset should be packaged?
						if (restoreAssetProperties->IsPackagedAsset)
						{
							// Add the new binary files to the packages
							PackageList *newlyAffectedPackages = MOG_ControllerPackageMerge::ExecutePackageCommands_AddAsset(restoreAsset);
							if (newlyAffectedPackages->Count)
							{
								affectedPackageFiles->Add(newlyAffectedPackages);
							}
						}

						// Close the restore asset
						restoreAsset->Close();
					}
				}
			}
			else if (String::Compare(assetProperties->Status, MOG_AssetStatus::GetText(MOG_AssetStatusType::Packaging), true) == 0 ||
					 String::Compare(assetProperties->Status, MOG_AssetStatus::GetText(MOG_AssetStatusType::Repackaging), true) == 0)
			{
				MOG_Filename* removeAssetFilename = NULL;
				MOG_ControllerAsset* removeAsset = NULL;

				// Check if this asset is sitting in the remove tray?
				MOG_Filename* removeTrayAssetFilename = MOG_Filename::GetLocalRemoveTrayFilename(mWorkspaceDirectory, assetFilename);
				if (DosUtils::DirectoryExistFast(removeTrayAssetFilename->GetEncodedFilename()))
				{
					removeAssetFilename = removeTrayAssetFilename;
				}
				else
				{
					// We should only consider removing the previously blessed asset when we are not repackaging
					if (String::Compare(assetProperties->Status, MOG_AssetStatus::GetText(MOG_AssetStatusType::Repackaging), true) != 0)
					{
						// Check if the asset already existed in the LocalBranch?
						String *revision = MOG_DBSyncedDataAPI::GetLocalAssetVersion(assetFilename, mWorkspaceDirectory, mPlatformName);
						if (revision->Length)
						{
							// Build the blessedAssetFilename
							removeAssetFilename = new MOG_Filename(MOG_ControllerRepository::GetAssetBlessedVersionPath(assetFilename, revision));
						}
					}
				}

				// Check if we found an asset needing to be removed?
				if (removeAssetFilename)
				{
					// Open the oldAssetRevision from the repository
					removeAsset = MOG_ControllerAsset::OpenAsset(removeAssetFilename);
					if (removeAsset)
					{
						try
						{
							// Add/Remove asset files from packages
							// Get the properties for the removeAsset
							MOG_Properties *removeAssetProperties = removeAsset->GetProperties(mPlatformName);
							// Check if this asset was packaged?
							if (removeAssetProperties->IsPackagedAsset)
							{
								// Remove the old binary files from the packages
								PackageList *newlyAffectedPackages = MOG_ControllerPackageMerge::ExecutePackageCommands_RemoveAsset(removeAsset);
								if (newlyAffectedPackages->Count)
								{
									affectedPackageFiles->Add(newlyAffectedPackages);
								}
							}
						}
						__finally
						{
							removeAsset->Close();
						}
					}
				}

				// Check if this asset should be packaged?
				if (assetProperties->IsPackagedAsset)
				{
					// Add the new binary files to the packages
					PackageList *newlyAffectedPackages = MOG_ControllerPackageMerge::ExecutePackageCommands_AddAsset(asset);
					if (newlyAffectedPackages->Count)
					{
						affectedPackageFiles->Add(newlyAffectedPackages);
					}
					else
					{
						// Change the asset's status to 'PackageError'
						MOG_ControllerInbox::UpdateInboxView(asset, MOG_AssetStatusType::PackageError);
						bFailed = true;
					}
				}
			}
		}
		__finally
		{
			// Close the asset
			asset->Close();
			asset = NULL;
		}
	}

	return affectedPackageFiles;
}


void MOG_ControllerPackageMergeLocal::FinalizeLocalAssets(ArrayList* assets)
{
	if (assets)
	{
		// Loop through all the local assets
		for (int a = 0; a < assets->Count; a++)
		{
			MOG_Filename *assetFilename = __try_cast<MOG_Filename*>(assets->Item[a]);
			MOG_ControllerAsset* asset = MOG_ControllerAsset::OpenAsset(assetFilename);
			if (asset)
			{
				FinalizeLocalAsset(asset);

				asset->Close();
			}
		}
	}
}

bool MOG_ControllerPackageMergeLocal::FinalizeLocalAsset(MOG_ControllerAsset* asset)
{
	if (asset)
	{
		MOG_Properties *assetProperties = asset->GetProperties();
		if (assetProperties)
		{
			if (String::Compare(assetProperties->Status, MOG_AssetStatus::GetText(MOG_AssetStatusType::Packaging), true) == 0 ||
				String::Compare(assetProperties->Status, MOG_AssetStatus::GetText(MOG_AssetStatusType::Repackaging), true) == 0)
			{
				MOG_ControllerInbox::UpdateInboxView(asset, MOG_AssetStatusType::Packaged);

				// Check if this asset had anything sitting in the local remove tray?
				MOG_Filename* removeTrayAssetFilename = MOG_Filename::GetLocalRemoveTrayFilename(mWorkspaceDirectory, asset->GetAssetFilename());
				if (DosUtils::DirectoryExistFast(removeTrayAssetFilename->GetEncodedFilename()))
				{
					// Delete the asset being removed
					if (!DosUtils::DirectoryDeleteFast(removeTrayAssetFilename->GetEncodedFilename()))
					{
						return false;
					}
				}
			}
			else if (String::Compare(assetProperties->Status, MOG_AssetStatus::GetText(MOG_AssetStatusType::Unpackaging), true) == 0)
			{
				// Delete the asset being removed
				if (!MOG_ControllerInbox::Delete(asset))
				{
					return false;
				}
			}
		}
	}

	return true;
}

