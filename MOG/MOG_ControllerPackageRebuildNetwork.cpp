//--------------------------------------------------------------------------------
//	MOG_ControllerPackageMergeNetwork.cpp
//	
//	
//--------------------------------------------------------------------------------

#include "StdAfx.h"

#include "MOG_Define.h"
#include "MOG_ControllerProject.h"
#include "MOG_DBPackageAPI.h"
#include "MOG_ControllerRepository.h"
#include "MOG_DBPostCommandAPI.h"
#include "MOG_ControllerInbox.h"
#include "MOG_ControllerPackage.h"

#include "MOG_ControllerPackageRebuildNetwork.h"


PackageList *MOG_ControllerPackageRebuildNetwork::DoPackageEvent(MOG_Filename *assetFilename, String *jobLabel, String *branchName, String *workspaceDirectory, String *platformName, String *validSlaves, BackgroundWorker* worker)
{
	PackageList* affectedPackageFiles = new PackageList;

	mWorkspaceDirectory = workspaceDirectory;
	mPlatformName = platformName;
	mJobLabel = jobLabel;
	mValidSlaves = validSlaves;
	mPackageMergeTasks = new PackageList();

	// Create the needed package commands for this rebuild
	// Get the current revision of this item needing to be rebuilt
	String *revision = MOG_ControllerProject::GetAssetCurrentVersionTimeStamp(assetFilename);
	// Make sure we have a valid revision to rebuild?
	if (revision->Length)
	{
		// Get the properties for this package
		MOG_Properties *pProperties = new MOG_Properties(assetFilename);
		pProperties->SetScope(mPlatformName);

		// Get the list of packages that need to be rebuilt
		ArrayList *packages = MOG_ControllerPackageMerge::GetAssociatedPackageList(assetFilename, pProperties, mJobLabel);
		if (packages->Count)
		{
			// Loop through all the specified packages
			for (int p = 0; p < packages->Count; p++)
			{
				// Get the PackageFileInfo
				MOG_PackageMerge_PackageFileInfo *packageFileInfo = __try_cast<MOG_PackageMerge_PackageFileInfo*>(packages->Item[p]);

				// Get the specified valid slaves for this package from the command?
				String *validSlaves = mValidSlaves;
				if (!validSlaves->Length)
				{
					// Ask the package if they have validSlaves requirement?
					validSlaves = packageFileInfo->mPackageProperties->ValidSlaves;
				}

				// Make sure this package's platform is still valid for the project?
				if (MOG_ControllerProject::IsValidPlatform(packageFileInfo->mPackageAssetFilename->GetAssetPlatform()))
				{
					// Make sure this package also has a current revision?
					revision = MOG_ControllerProject::GetAssetCurrentVersionTimeStamp(packageFileInfo->mPackageAssetFilename);
					if (revision->Length)
					{
						// Schedule the specified package to be deleted
						MOG_ControllerPackageMergeNetwork::ScheduleDeletePackageCommand(packageFileInfo, mJobLabel);

						// Check for any current assets in the project that contain a package relationship...
						// This is the better way to get the contained assets in the event this platform-specific package was created much later
						// Get any assets with a relationship to this file
						ArrayList *containedAssets = MOG_ControllerPackage::GetAssociatedAssetsForPackage(packageFileInfo->mPackageAssetFilename);

						// Scan the contained assets
						for (int a = 0; a < containedAssets->Count; a++)
						{
							MOG_Filename *assetFilename = __try_cast<MOG_Filename*>(containedAssets->Item[a]);

							// Open this asset
							MOG_Filename *blessedAssetFilename = MOG_ControllerRepository::GetAssetBlessedVersionPath(assetFilename, assetFilename->GetVersionTimeStamp());
							MOG_ControllerAsset *asset = MOG_ControllerAsset::OpenAsset(blessedAssetFilename);
							if (asset)
							{
								MOG_ControllerPackageMergeNetwork::ScheduleAssetPackageCommands(asset,
																								MOG_PACKAGECOMMAND_TYPE::MOG_PackageCommand_AddAsset, 
																								mJobLabel,
																								mValidSlaves,
																								packageFileInfo,
																								false);
								// Close the asset controller as soon as we are finished with it
								asset->Close();
							}
							else
							{
								// Make sure to inform the Server about this
								String *message = String::Concat(	S"The System was unable to open ", blessedAssetFilename->GetFullFilename(), S"\n",
																	S"Confirm this asset exists in the MOG Repository.  The rebuilt package will be missing this asset.");
								MOG_Report::ReportMessage(S"RebuildPackage Failed", message, S"", MOG_ALERT_LEVEL::ERROR);
							}
						}
					}
					else
					{
						String *message = String::Concat(S"System was unable to locate a current revision for '", packageFileInfo->mPackageAssetFilename->GetAssetFullName(), S"'");
						MOG_Prompt::PromptMessage(S"RebuildPackage Failed", message, S"", MOG_ALERT_LEVEL::ERROR);
					}
				}
				else
				{
					String *message = String::Concat(S"Package platform is not longer valid for this project and cannot be rebuilt.\n",
													 S"PACKAGE: ", packageFileInfo->mPackageAssetFilename->GetAssetFullName());
					MOG_Prompt::PromptMessage(S"RebuildPackage Failed", message, S"", MOG_ALERT_LEVEL::ERROR);
				}
			}
		}
	}
	else
	{
		String *message = String::Concat(S"System was unable to locate a current revision for '", assetFilename, S"'");
		MOG_Prompt::PromptMessage(S"RebuildPackage Failed", message, S"", MOG_ALERT_LEVEL::ERROR);
	}

	return affectedPackageFiles;
}


bool MOG_ControllerPackageRebuildNetwork::RejectFailedAsset(MOG_Filename *blessedAssetFilename, String *comment, BackgroundWorker* worker)
{
	// We don't want to reject any assets on a failed network package rebuild
	// Just return acting like we rejected the asset
	return true;
}


