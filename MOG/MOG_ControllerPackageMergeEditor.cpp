//--------------------------------------------------------------------------------
//	MOG_ControllerPackageMergeEditor.cpp
//	
//	
//--------------------------------------------------------------------------------

#include "StdAfx.h"

#include "MOG_Define.h"
#include "MOG_CommandFactory.h"
#include "MOG_ControllerSystem.h"

#include "MOG_ControllerPackageMergeEditor.h"

MOG_ControllerPackageMergeEditor::MOG_ControllerPackageMergeEditor()
{
	mWarnUnmodifiedPackages = false;
}

PackageList *MOG_ControllerPackageMergeEditor::ExecutePackageMergeTasks(PackageList *packageMergeTasks)
{
	PackageList *affectedPackageFiles = new PackageList();
	bool bFailed = false;

	// Check if we had any package merge tasks?
	if (packageMergeTasks->Count)
	{
		for (int t = 0; t < packageMergeTasks->Count; t++)
		{
			MOG_PackageMerge_PackageFileInfo *packageFileInfo = __try_cast<MOG_PackageMerge_PackageFileInfo*>(packageMergeTasks->Item[t]);

			// Check the PackageStyle?
			if (packageFileInfo->mPackageStyle == MOG_PackageStyle::TaskFile)
			{
				// Make sure our InputPackageTaskFile exists?
				if (DosUtils::FileExistFast(packageFileInfo->mInputPackageTaskFile))
				{
					// Make sure we have a valid command manager?
					if (MOG_ControllerSystem::IsCommandManager())
					{
						// Get the callbacks?
						MOG_Callbacks *callbacks = MOG_ControllerSystem::GetCommandManager()->GetCallbacks();
						if (callbacks && callbacks->mCommandCallback)
						{
							// Process the command in the Editor
							MOG_Command *editorPackageMergeTask = MOG_CommandFactory::Setup_EditorPackageMergeTask(packageFileInfo->mInputPackageTaskFile, packageFileInfo->mOutputPackageTaskFile);
							if (callbacks->mCommandCallback->Invoke(editorPackageMergeTask))
							{
								PackageList *newlyAffectedPackages = DetectAffectedPackageFiles(packageFileInfo);
								affectedPackageFiles->Add(newlyAffectedPackages);
							}
							else
							{
								// Indicate the Editor has a major problem!
								String *message = String::Concat(	S"The Editor returned an error during its callback when attempting to perform this package merge request.\n",
																	S"Contact your MOG System Administrator.");
								MOG_Report::ReportMessage(S"Editor/Package Merge", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
							}
						}
						else
						{
							// Indicate the Editor has a major problem!
							String *message = String::Concat(	S"MOG is missing the required Editor callback routines in order to execute this package merge request.\n",
																S"Contact your MOG System Administrator.");
							MOG_Report::ReportMessage(S"Editor/Package Merge", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
						}
					}
				}
				else
				{
					String *message = String::Concat(	S"System did not locate the necessary InputPackageTaskFile '", packageFileInfo->mInputPackageTaskFile, S"'\n",
														S"TaskFile package merges always require an InputPackageTaskFile.");
					MOG_Report::ReportMessage(S"Bless/Package", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
					bFailed = true;
				}
			}
		}
	}

	return affectedPackageFiles;
}

bool MOG_ControllerPackageMergeEditor::DetectLocalAssetsToMerge()
{
	//Our cache might be completely out of date in the editor at this point, so clear it completely
	MOG_ControllerSystem::GetDB()->GetDBCache()->GetInboxAssetCache()->ClearCache();

	//Call the real function to get the list of assets
	return MOG_ControllerPackageMergeLocal::DetectLocalAssetsToMerge();
}

PackageList* MOG_ControllerPackageMergeEditor::DetectAffectedPackageFiles(MOG_PackageMerge_PackageFileInfo* packageFileInfo)
{
	PackageList* modifiedFiles = new PackageList;

	// Attempt to get our list of updated packages from the package output file?
	PackageList *updatedPackageFiles = GetUpdatedPackageFilesFromOutputTaskFile(packageFileInfo);
	if (updatedPackageFiles->Count)
	{
		// Scan the packages listed in the Updated section
		for (int p = 0; p < updatedPackageFiles->Count; p++)
		{
			// Get the details of the updated package
			MOG_PackageMerge_PackageFileInfo *updatedPackageFileInfo = __try_cast<MOG_PackageMerge_PackageFileInfo *>(updatedPackageFiles->Item[p]);

			//This package has been modified
			modifiedFiles->Add(updatedPackageFileInfo);
		}
	}

	return modifiedFiles;
}


