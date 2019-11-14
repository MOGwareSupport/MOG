//--------------------------------------------------------------------------------
//	MOG_ControllerPackageMerge.cpp
//	
//	
//--------------------------------------------------------------------------------

#include "StdAfx.h"

#include "MOG_Define.h"
#include "MOG_DosUtils.h"
#include "MOG_ControllerProject.h"
#include "MOG_ControllerRepository.h"
#include "MOG_ControllerPackage.h"
#include "MOG_ControllerSystem.h"
#include "MOG_DBAssetAPI.h"
#include "MOG_Tokens.h"
#include "MOG_EnvironmentVars.h"
#include "MOG_DBInboxAPI.h"

#include "MOG_ControllerPackageMerge.h"


MOG_PackageMerge_PackageFileInfo::MOG_PackageMerge_PackageFileInfo(String* packageFile, String* workspaceDirectory, String* platformName, String* jobLabel)
{
	Clear();

	// Make sure we have a realive package file
	String *relativePackageFile = DosUtils::PathMakeRelativePath(workspaceDirectory, packageFile);

	// Determine our PackageAssetFilename using relativePackageFile
	ArrayList* assets = MOG_ControllerProject::MapFilenameToAssetName(mRelativePackageFile, platformName);
	if (assets && assets->Count)
	{
		// Always assume the first one is the correct match
		mPackageAssetFilename = __try_cast<MOG_Filename *>(assets->Item[0]);
		MOG_ControllerProject::MapFilenameToAssetName_WarnAboutAmbiguousMatches(mRelativePackageFile, assets);
	}
	else
	{
		// Make sure to inform the Server about this error
		String *message = String::Concat(	S"MOG was unable to match the PackageFile to a valid AssetName.\n",
											S"PACKAGEFILE: ", packageFile, S"\n",
											S"PLATFORM: ", platformName, S"\n",
											S"Please make sure a valid asset exists for this PackageFile.");
		MOG_Report::ReportMessage(S"Packaging Error", message, S"", MOG_ALERT_LEVEL::ERROR);
	}
	
	mJobLabel = jobLabel;

	Initialize(workspaceDirectory, platformName);
}

MOG_PackageMerge_PackageFileInfo::MOG_PackageMerge_PackageFileInfo(MOG_Filename *packageAssetFilename, String* workspaceDirectory, String* platformName, String *jobLabel)
{
	Clear();

	mPackageAssetFilename = packageAssetFilename;

	mJobLabel = jobLabel;
	
	Initialize(workspaceDirectory, platformName);
}

void MOG_PackageMerge_PackageFileInfo::Clear()
{
	mPackageAssetFilename = new MOG_Filename(S"");
	mBlessedPackageFilename = new MOG_Filename(S"");
	mPackageProperties = NULL;
	mRelativePackageFile = S"";
	mPackageFile = S"";
	mJobLabel = S"";
	MOG_PackageStyle mPackageStyle = MOG_PackageStyle::Disabled;
	mAutoPackage = false;
	mHideWindow = true;
	mSyncPackageWorkspaceDirectory = false;
	mCleanupPackageWorkspaceDirectory = false;
	mTaskFileTool = S"";
	mPackagePreMergeEvent = S"";
	mPackagePostMergeEvent = S"";
	mInputPackageTaskFile = S"";
	mOutputPackageTaskFile = S"";
	mPackageWorkspaceDirectory = S"";
	mPackageDataDirectory = S"";
	mPackageCommand_DeletePackageFile = S"";
	mPackageCommand_ResolveLateResolvers = S"";
	mPackageCommand_Add = S"";
	mPackageCommand_Remove = S"";
}

void MOG_PackageMerge_PackageFileInfo::Initialize(String* workspaceDirectory, String* platformName)
{
	// Make sure we have a valid mPackageAssetFilename?
	if (mPackageAssetFilename->GetFilenameType() == MOG_FILENAME_TYPE::MOG_FILENAME_Asset)
	{
		// Make sure we have obtained an active revision for this package
		String *revision = MOG_ControllerPackage::GetLatestPackageRevision_IncludingUnpostedRevisions(mPackageAssetFilename, mJobLabel);
		if (revision->Length)
		{
			mBlessedPackageFilename = MOG_ControllerRepository::GetAssetBlessedVersionPath(mPackageAssetFilename, revision);
			mPackageProperties = new MOG_Properties(mBlessedPackageFilename);
			mPackageProperties->SetScope(mPackageAssetFilename->GetAssetPlatform());
			// This should already be relative but there was an older bug where this returned a full path and ended up crashing later because relative would be appended like "c:\\path\\c:\path\\"
			mRelativePackageFile = DosUtils::PathMakeRelativePath(workspaceDirectory, MOG_ControllerPackage::MapPackageAssetNameToFile(mBlessedPackageFilename));

			mPackageStyle = mPackageProperties->PackageStyle;
			mAutoPackage = mPackageProperties->AutoPackage;
			mHideWindow = (mPackageProperties->ShowPackageCommandWindow) ? false : MOG_ControllerSystem::GetCommandManager()->mCommandLineHideWindow;
			mSyncPackageWorkspaceDirectory = mPackageProperties->SyncPackageWorkspaceDirectory;
			mCleanupPackageWorkspaceDirectory = mPackageProperties->CleanupPackageWorkspaceDirectory;

			Tokenize(workspaceDirectory, platformName);
		}
	}
}

void MOG_PackageMerge_PackageFileInfo::Tokenize(String* workspaceDirectory, String* platformName)
{
	// Tokenize the PackageWorkspaceDirectory
	mPackageWorkspaceDirectory = MOG_Tokens::GetFormattedString(mPackageProperties->PackageWorkspaceDirectory, mBlessedPackageFilename, this, workspaceDirectory, platformName);
	if (mPackageWorkspaceDirectory->Length == 0)
	{
		// Looks like the wasn't anything defined so fall back and use the one specified
		mPackageWorkspaceDirectory = workspaceDirectory;
	}

	// Tokenize the PackageDataDirectory
	mPackageDataDirectory = MOG_Tokens::GetFormattedString(mPackageProperties->PackageDataDirectory, mBlessedPackageFilename, this, workspaceDirectory, platformName);

	// Build our mPackageFile using PackageWorkspaceDirectory
	if (mPackageWorkspaceDirectory->Length)
	{
		if (!mPackageFile->String::StartsWith( mPackageWorkspaceDirectory ))
		{
			mPackageFile = String::Concat(mPackageWorkspaceDirectory, S"\\", mRelativePackageFile);
		}
	}
	else
	{
		mPackageFile = mRelativePackageFile;
	}

	// Tokenize the task files
	mInputPackageTaskFile = MOG_Tokens::GetFormattedString(String::Concat(mPackageWorkspaceDirectory, S"\\", mPackageProperties->InputPackageTaskFile), mBlessedPackageFilename, this, workspaceDirectory, platformName);
	mOutputPackageTaskFile = MOG_Tokens::GetFormattedString(String::Concat(mPackageWorkspaceDirectory, S"\\", mPackageProperties->OutputPackageTaskFile), mBlessedPackageFilename, this, workspaceDirectory, platformName);

	// Tokenize the package commands
	mPackageCommand_DeletePackageFile = MOG_Tokens::GetFormattedString(mPackageProperties->PackageCommand_DeletePackageFile, mBlessedPackageFilename, this, workspaceDirectory, platformName);
	mPackageCommand_ResolveLateResolvers = MOG_Tokens::GetFormattedString(mPackageProperties->PackageCommand_ResolveLateResolvers, mBlessedPackageFilename, this, workspaceDirectory, platformName);
	// We can't tokenize these commands because they are asset based and need to be expanded when used
	mPackageCommand_Add = mPackageProperties->PackageCommand_Add;
	mPackageCommand_Remove = mPackageProperties->PackageCommand_Remove;

	// Tokenize the package tools
	mTaskFileTool = MOG_Tokens::GetFormattedString(mPackageProperties->TaskFileTool, mBlessedPackageFilename, this, workspaceDirectory, platformName);
	mPackagePreMergeEvent = MOG_Tokens::GetFormattedString(mPackageProperties->PackagePreMergeEvent, mBlessedPackageFilename, this, workspaceDirectory, platformName);
	mPackagePostMergeEvent = MOG_Tokens::GetFormattedString(mPackageProperties->PackagePostMergeEvent, mBlessedPackageFilename, this, workspaceDirectory, platformName);
}


void PackageList::Add(PackageList *packageFiles)
{
	// Scan the list to make sure it isn't already listed?
	for (int i = 0; i < packageFiles->Count; i++)
	{
		MOG_PackageMerge_PackageFileInfo *packageFileInfo = __try_cast<MOG_PackageMerge_PackageFileInfo *>(packageFiles->Item[i]);
		Add(packageFileInfo);
	}
}


void PackageList::Add(MOG_PackageMerge_PackageFileInfo *packageFile)
{
	// Scan the list to make sure it isn't already listed?
	for (int i = 0; i < Count; i++)
	{
		MOG_PackageMerge_PackageFileInfo *testPackageFileInfo = __try_cast<MOG_PackageMerge_PackageFileInfo *>(Item[i]);
		// Check if this matches?
		if (String::Compare(testPackageFileInfo->mPackageAssetFilename->GetAssetFullName(), packageFile->mPackageAssetFilename->GetAssetFullName(), true) == 0)
		{
			// Bail out now because this package appears to be already listed
			return;
		}
	}

	// Add this new PackageFileInfo
	ArrayList::Add(packageFile);
}

void PackageList::AddTask(MOG_PackageMerge_PackageFileInfo *packageTask)
{
	// Scan the list to make sure it isn't already listed?
	for (int i = 0; i < Count; i++)
	{
		MOG_PackageMerge_PackageFileInfo *test = __try_cast<MOG_PackageMerge_PackageFileInfo *>(Item[i]);
		// Check if this matches?
		if (String::Compare(packageTask->mPackageWorkspaceDirectory, test->mPackageWorkspaceDirectory, true) == 0 &&
			String::Compare(packageTask->mTaskFileTool, test->mTaskFileTool, true) == 0 &&
			String::Compare(packageTask->mPackagePreMergeEvent, test->mPackagePreMergeEvent, true) == 0 &&
			String::Compare(packageTask->mPackagePostMergeEvent, test->mPackagePostMergeEvent, true) == 0 &&
			String::Compare(packageTask->mInputPackageTaskFile, test->mInputPackageTaskFile, true) == 0 &&
			String::Compare(packageTask->mOutputPackageTaskFile, test->mOutputPackageTaskFile, true) == 0)
		{
			// Bail out now because this package task appears to be already listed
			return;
		}
	}

	// Add this new PackageFileInfo
	ArrayList::Add(packageTask);
}

MOG_PackageMerge_PackageFileInfo* PackageList::FindByFileName(String* packageName)
{
	// Scan the list for an info with a name that matches whatever they gave us
	for (int i = 0; i < Count; i++)
	{
		MOG_PackageMerge_PackageFileInfo *test = __try_cast<MOG_PackageMerge_PackageFileInfo *>(Item[i]);
		// Check if this matches?
		if (String::Compare(test->mPackageFile, packageName, true) == 0 ||
			String::Compare(test->mRelativePackageFile, packageName, true) == 0)
		{
			// We found it
			return test;
		}
	}

	return NULL;
}

MOG_PackageMerge_PackageFileInfo* PackageList::FindByAssetName(String* packageAssetName)
{
	// Scan the list for an info with a name that matches whatever they gave us
	for (int i = 0; i < Count; i++)
	{
		MOG_PackageMerge_PackageFileInfo *testPackageFileInfo = __try_cast<MOG_PackageMerge_PackageFileInfo *>(Item[i]);
		// Check if this matches?
		if (String::Compare(testPackageFileInfo->mPackageAssetFilename->GetAssetFullName(), packageAssetName, true) == 0)
		{
			// Found it!
			return testPackageFileInfo;
		}
	}

	return NULL;
}

void PackageList::GenerateUniquePackageTasks(PackageList *packageFiles)
{
	if (packageFiles)
	{
		// Scan the list to make sure it isn't already listed?
		for (int i = 0; i < packageFiles->Count; i++)
		{
			MOG_PackageMerge_PackageFileInfo *packageFileInfo = __try_cast<MOG_PackageMerge_PackageFileInfo *>(packageFiles->Item[i]);

			// Check if the asset's properties indicate it uses a task file?
			if (packageFileInfo->mPackageStyle == MOG_PackageStyle::TaskFile)
			{
				//If there's no package working directory, we have no business doing any work on this package
				if (packageFileInfo->mPackageWorkspaceDirectory->Length)
				{
					// Add the new task
					AddTask(packageFileInfo);
				}
			}
		}
	}
}

MOG_ControllerPackageMerge::MOG_ControllerPackageMerge()
{
	// Track when we were constructed for later comparison with modified package times
	mStartTime = DateTime::Now;
}

PackageList* MOG_ControllerPackageMerge::VerifyWorkingDirectories(PackageList* packageFiles)
{
	PackageList* packagesWithValidWorkingDirectories = new PackageList;

	//Go through the list of packages and make sure they all have valid working directories
	for (int i = 0; i < packageFiles->Count; i++)
	{
		MOG_PackageMerge_PackageFileInfo* packageFile = __try_cast<MOG_PackageMerge_PackageFileInfo*>(packageFiles->Item[i]);
		MOG_PackageMerge_PackageFileInfo* validatedPackageFile = VerifyWorkingDirectory(packageFile, true);
		if (validatedPackageFile)
		{
			//It passed the test, so add it to the list of verified packages
			packagesWithValidWorkingDirectories->Add(validatedPackageFile);
		}
	}

	//return the list of packages that actually have valid working directories
	return packagesWithValidWorkingDirectories;
}

MOG_PackageMerge_PackageFileInfo* MOG_ControllerPackageMerge::VerifyWorkingDirectory(MOG_PackageMerge_PackageFileInfo* packageFile, bool bWarnUser)
{
	if (packageFile)
	{
		// Check if we are missing a PackageWorkspaceDirectory?
		if (packageFile->mPackageWorkspaceDirectory->Length == 0)
		{
			// Use the workspace directory supplied to us
			packageFile->Tokenize(mWorkspaceDirectory, mPlatformName);
		}

		// Make sure we have a valid PackageWorkspaceDirectory
		if (packageFile->mPackageWorkspaceDirectory->Length > 0)
		{
			return packageFile;
		}
		else
		{
			if (bWarnUser)
			{
				String* message = String::Concat(	S"Package is missing a valid PackageWorkspaceDirectory.\n",
													S"PACKAGE: ", packageFile->mPackageAssetFilename->GetAssetFullName(), S"\n",
													S"MOG cannot perform a package merge without a valid PackageWorkspaceDirectory.");
				MOG_Report::ReportMessage(S"Package Merge Error", message, S"", MOG_ALERT_LEVEL::ERROR);
			}
		}
	}

	return NULL;
}


bool MOG_ControllerPackageMerge::CleanWorkingDirectories(PackageList *packageFiles)
{
	bool bFailed = false;

	// Prepare for the upcomming package merge
	for (int p = 0; p < packageFiles->Count; p++)
	{
		// Get the package file info
		MOG_PackageMerge_PackageFileInfo *packageFileInfo = __try_cast<MOG_PackageMerge_PackageFileInfo*>(packageFiles->Item[p]);
		//If there's no package working directory, we have no business doing any work on this package
		if (packageFileInfo->mPackageWorkspaceDirectory->Length)
		{
			// Check if we have been asked to cleanup the package working directories?
			if (packageFileInfo->mCleanupPackageWorkspaceDirectory)
			{
				// Clean any TaskFile related items
				CleanWorkingDirectories_EntireWorkspace(packageFileInfo);
			}
			else
			{
				// Clean any TaskFile related items
				CleanWorkingDirectories_TaskRelatedFiles(packageFileInfo);
			}
		}
	}

	if (!bFailed)
	{
		return true;
	}
	return false;
}


bool MOG_ControllerPackageMerge::CleanWorkingDirectories_TaskRelatedFiles(MOG_PackageMerge_PackageFileInfo *packageFileInfo)
{
	bool bFailed = false;

	//If there's no package working directory, we have no business doing any work on this package
	if (packageFileInfo->mPackageWorkspaceDirectory->Length)
	{
		// Check if there was a task file?
		if (packageFileInfo->mInputPackageTaskFile->Length)
		{
			// Check if an old InputPackageTaskFile file exists?
			if (DosUtils::FileExistFast(packageFileInfo->mInputPackageTaskFile))
			{
				// Delete the old InputPackageTaskFile
				if (!DosUtils::FileDeleteFast(packageFileInfo->mInputPackageTaskFile))
				{
					bFailed = true;
				}
			}
		}

		// Check if there was a task file?
		if (packageFileInfo->mOutputPackageTaskFile->Length)
		{
			// Check if there is an OutputPackageTaskFile file?
			if (DosUtils::FileExistFast(packageFileInfo->mOutputPackageTaskFile))
			{
				// Delete the OutputPackageTaskFile
				if (!DosUtils::FileDeleteFast(packageFileInfo->mOutputPackageTaskFile))
				{
					bFailed = true;
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


bool MOG_ControllerPackageMerge::CleanWorkingDirectories_EntireWorkspace(MOG_PackageMerge_PackageFileInfo *packageFileInfo)
{
	// Check if there is an OutputPackageTaskFile file?
	if (DosUtils::DirectoryExistFast(packageFileInfo->mPackageWorkspaceDirectory))
	{
		// Delete the PackageWorkspaceDirectory
		if (!DosUtils::DirectoryDeleteFast(packageFileInfo->mPackageWorkspaceDirectory))
		{
			String *message = String::Concat(	S"System failed to cleanup the PackageWorkspaceDirectory.\n",
												S"DIRECTORY: ", packageFileInfo->mPackageWorkspaceDirectory, S"\n",
												S"This may be caused by a possible file sharing violation.");
			MOG_Report::ReportMessage(S"Local Package", message, S"", MOG_ALERT_LEVEL::ERROR);
			return false;
		}
	}

	return true;
}


bool MOG_ControllerPackageMerge::PrepareWorkingDirectories(PackageList *packageFiles)
{
	return true;
}


bool MOG_ControllerPackageMerge::ExecutePreMergeEvents(PackageList *packageFiles)
{
	bool bFailed = false;

	// Loop through all the packageFiles and execute their prepackage events
	for (int p = 0; p < packageFiles->Count; p++)
	{
		// Get the package file info
		MOG_PackageMerge_PackageFileInfo *packageFileInfo = __try_cast<MOG_PackageMerge_PackageFileInfo*>(packageFiles->Item[p]);
		//If there's no package working directory, we have no business doing any work on this package
		if (packageFileInfo->mPackageWorkspaceDirectory->Length)
		{
			// Execute the PackageBegin command for this package
			if (packageFileInfo->mPackagePreMergeEvent->Length)
			{
				if (!MOG_ControllerPackageMerge::ExecutePackageTool(packageFileInfo, packageFileInfo->mPackagePreMergeEvent))
				{
					bFailed = true;
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


PackageList *MOG_ControllerPackageMerge::PreparePackageMergeTasks(PackageList *packageMergeTasks)
{
	return new PackageList();
}


PackageList *MOG_ControllerPackageMerge::ExecutePackageMergeTasks(PackageList *packageMergeTasks)
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
					// Execute the packageTool
					if (MOG_ControllerPackageMerge::ExecutePackageTool(packageFileInfo, packageFileInfo->mTaskFileTool))
					{
						PackageList *newlyAffectedPackages = DetectAffectedPackageFiles(packageFileInfo);
						affectedPackageFiles->Add(newlyAffectedPackages);
					}
				}
				else
				{
					String *message = String::Concat(	S"System did not locate the necessary InputPackageTaskFile '", packageFileInfo->mInputPackageTaskFile, S"'\n",
														S"TaskFile package merges always require an InputPackageTaskFile.");
					MOG_Report::ReportMessage(S"Bless/Package", message, S"", MOG_ALERT_LEVEL::ERROR);
					bFailed = true;
				}
			}
		}
	}

	return affectedPackageFiles;
}


PackageList* MOG_ControllerPackageMerge::DetectAffectedPackageFiles(MOG_PackageMerge_PackageFileInfo* packageFileInfo)
{
	PackageList* modifiedFiles = new PackageList;

	// Attempt to get our list of updated packages from the package output file?
	PackageList *updatedPackageFiles = GetUpdatedPackageFilesFromOutputTaskFile(packageFileInfo);
	if (updatedPackageFiles->Count)
	{
		// Scan the packages listed in the Updated section
		for (int p = 0; p < updatedPackageFiles->Count; p++)
		{
			bool bModified = false;

			// Get the details of the updated package
			MOG_PackageMerge_PackageFileInfo *updatedPackageFileInfo = __try_cast<MOG_PackageMerge_PackageFileInfo *>(updatedPackageFiles->Item[p]);

			// Make sure this package file exists
			if (DosUtils::FileExistFast(updatedPackageFileInfo->mPackageFile))
			{
				// Validate this package as being modified
				FileInfo *fileInfo = DosUtils::FileGetInfo(updatedPackageFileInfo->mPackageFile);
				if (DateTime::Compare(fileInfo->LastWriteTime, mStartTime) > 0)
				{
					//This package has been modified
					modifiedFiles->Add(updatedPackageFileInfo);
					bModified = true;
				}
				else
				{
					// Warn the user about this package has not being modified
					String *message = String::Concat(	S"The package file that should have been modified during the package merge was not changed.\n",
														S"PACKAGEFILE: ", updatedPackageFileInfo->mRelativePackageFile, S"\n",
														S"PLATFORM: ", mPlatformName, S"\n",
														S"No new package was generated.\n",
														S"Confirm the seriousness of this error with your MOG Administrator.");
					MOG_Report::ReportMessage(S"Bless/Package", message, S"", MOG_ALERT_LEVEL::ERROR);
				}
			}
			else
			{
				// Check if we are missing an extension?
				if (DosUtils::PathGetExtension(updatedPackageFileInfo->mPackageFile)->Length == 0)
				{
					String *message = String::Concat(	S"The package file that should have been modified during the package merge was not found.\n",
														S"It is suspicious that this package file does not contain an extension.\n",
														S"PACKAGEFILE: ", updatedPackageFileInfo->mRelativePackageFile, S"\n",
														S"PLATFORM: ", mPlatformName, S"\n",
														S"No new package was generated.\n",
														S"Confirm the seriousness of this error with your MOG Administrator.");
					MOG_Report::ReportMessage(S"Bless/Package", message, S"", MOG_ALERT_LEVEL::ERROR);
				}
				else
				{
					String *message = String::Concat(	S"The package file that should have been modified during the package merge was not found.\n",
														S"PACKAGEFILE: ", updatedPackageFileInfo->mRelativePackageFile, S"\n",
														S"PLATFORM: ", mPlatformName, S"\n",
														S"No new package was generated.\n",
														S"Confirm the seriousness of this error with your MOG Administrator.");
					MOG_Report::ReportMessage(S"Bless/Package", message, S"", MOG_ALERT_LEVEL::ERROR);
				}
			}
		}
	}

	return modifiedFiles;
}


PackageList *MOG_ControllerPackageMerge::GetUpdatedPackageFilesFromOutputTaskFile(MOG_PackageMerge_PackageFileInfo *packageFileInfo)
{
	PackageList *updatedPackages = new PackageList();

	// Check if we have a 'PackagePosting.Info'?
	if (DosUtils::FileExistFast(packageFileInfo->mOutputPackageTaskFile))
	{
		// Open the Package.Posting.Info file
		MOG_Ini *packageCommandFile = new MOG_Ini(packageFileInfo->mOutputPackageTaskFile);
		// Build the list of updated packages from the package output file
		if (packageCommandFile->SectionExist(S"UpdatedPackages"))
		{
			// Add these packages to the blessed MasterData
			// Scan the packages listed in the Updated section
			int numPackages = packageCommandFile->CountKeys(S"UpdatedPackages");
			for (int p = 0; p < numPackages; p++)
			{
				// Add this package to our list of updated packages
				String *updatedPackageFile = packageCommandFile->GetKeyNameByIndexSLOW(S"UpdatedPackages", p);

				MOG_PackageMerge_PackageFileInfo* tempPackageFileInfo = mPackageFiles->FindByFileName(updatedPackageFile);
				if (!tempPackageFileInfo)
				{
					//Create a corresponding package file info for the updated package file
					MOG_PackageMerge_PackageFileInfo* tempPackageFileInfo = new MOG_PackageMerge_PackageFileInfo(updatedPackageFile, packageFileInfo->mPackageWorkspaceDirectory, packageFileInfo->mPackageAssetFilename->GetAssetPlatform(), packageFileInfo->mJobLabel);
					// Make sure we obtained a valid PackageFileInfo by checking the mPackageProperties?
					if (tempPackageFileInfo->mPackageProperties)
					{
						// Make sure we have a valid PackageFileInfo?
						if (tempPackageFileInfo->mPackageAssetFilename->GetFilenameType() == MOG_FILENAME_TYPE::MOG_FILENAME_Asset)
						{
							tempPackageFileInfo = VerifyWorkingDirectory(tempPackageFileInfo, false);
							if (tempPackageFileInfo)
							{
								updatedPackages->Add(tempPackageFileInfo);
							}
						}
					}
				}
			}
		}

		// Close the package output file
		packageCommandFile->Close();
		packageCommandFile = NULL;
	}

	if (updatedPackages->Count == 0)
	{
		// we didn't get anything from the output file, so at the very least use the package that was passed in
		updatedPackages->Add(packageFileInfo);
	}

	return updatedPackages;
}


bool MOG_ControllerPackageMerge::ExecutePostMergeEvents(PackageList *packageFiles)
{
	bool bFailed = false;

	// Loop through all the packageFiles and execute their postpackage events
	for (int p = 0; p < packageFiles->Count; p++)
	{
		// Get the package file info
		MOG_PackageMerge_PackageFileInfo *packageFileInfo = __try_cast<MOG_PackageMerge_PackageFileInfo*>(packageFiles->Item[p]);
		//If there's no package working directory, we have no business doing any work on this package
		if (packageFileInfo->mPackageWorkspaceDirectory->Length)
		{
			// Execute the PackageEnd command for this package
			if (packageFileInfo->mPackagePostMergeEvent->Length)
			{
				if (!MOG_ControllerPackageMerge::ExecutePackageTool(packageFileInfo, packageFileInfo->mPackagePostMergeEvent))
				{
					bFailed = true;
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


bool MOG_ControllerPackageMerge::ParseOutputTaskFile(PackageList *packageMergeTasks)
{
	bool bFailed = false;

	// Loop through all the packageTasks and check for any reported errors
	for (int p = 0; p < packageMergeTasks->Count; p++)
	{
		// Get the package file info
		MOG_PackageMerge_PackageFileInfo *packageFileInfo = __try_cast<MOG_PackageMerge_PackageFileInfo*>(packageMergeTasks->Item[p]);

		// Record LateResolvers
		if (!ParseOutputTaskFile_RecordLateResolvers(packageFileInfo))
		{
			bFailed = true;
		}

		// Report Errors
		if (!ParseOutputTaskFile_ReportErrors(packageFileInfo))
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


bool MOG_ControllerPackageMerge::ParseOutputTaskFile_RecordLateResolvers(MOG_PackageMerge_PackageFileInfo* packageFileInfo)
{
	return true;
}

bool MOG_ControllerPackageMerge::ParseOutputTaskFile_ReportErrors(MOG_PackageMerge_PackageFileInfo* packageFileInfo)
{
	return true;
}


ArrayList* MOG_ControllerPackageMerge::GetErrorsFromPackageCommandsFile(MOG_PackageMerge_PackageFileInfo* packageFileInfo, bool bReport)
{
	return GetEventsFromPackageCommandsFile(packageFileInfo, bReport, S"ERRORS", MOG_ALERT_LEVEL::ERROR);
}

ArrayList* MOG_ControllerPackageMerge::GetWarningsFromPackageCommandsFile(MOG_PackageMerge_PackageFileInfo* packageFileInfo, bool bReport)
{
	return GetEventsFromPackageCommandsFile(packageFileInfo, bReport, S"ALERTS", MOG_ALERT_LEVEL::ALERT);
}

ArrayList* MOG_ControllerPackageMerge::GetEventsFromPackageCommandsFile(MOG_PackageMerge_PackageFileInfo* packageFileInfo, bool bReport, String *section, MOG_ALERT_LEVEL alertLevel)
{
	ArrayList *assetFiles = new ArrayList();

	// Check if we have a 'PackageCommands.Done.Info'?
	if (DosUtils::FileExistFast(packageFileInfo->mOutputPackageTaskFile))
	{
		// Open the PackageCommands.Done.Info file
		MOG_Ini *packageCommandFile = new MOG_Ini(packageFileInfo->mOutputPackageTaskFile);

		// Build the list of package errors from the package output file
		if (packageCommandFile->SectionExist(section))
		{
			// Add these packages to the list of errors
			// Scan the packages listed in the MOG_ERRORS section
			int numErrors = packageCommandFile->CountKeys(section);
			for (int e = 0; e < numErrors; e++)
			{
				// Add this error to our list of errors
				String *assetName = packageCommandFile->GetKeyNameByIndexSLOW(section, e);
				String *assetError = packageCommandFile->GetKeyByIndexSLOW(section, e);
				assetFiles->Add(assetName);

				// Report the error?
				if (bReport)
				{
					String *errorMessage = String::Concat(	S"Asset: '", assetName, S"'\n",
															assetError);
					MOG_Report::ReportMessage(S"PackageMergeEvent", errorMessage, "", alertLevel);
				}
			}
		}

		// Close the package output file
		packageCommandFile->Close();
		packageCommandFile = NULL;
	}

	return assetFiles;
}


ArrayList *MOG_ControllerPackageMerge::GetPackageToolEnvironment(String *relativeToolPath, MOG_PackageMerge_PackageFileInfo* packageFileInfo)
{
	ArrayList *environment = new ArrayList;

	// Make sure to include our relative tool path directory in our environment
	DosUtils::EnvironmentList_AddVariable(environment, MOG_EnvironmentVars::CreateToolsPathEnvironmentVariableString(relativeToolPath, packageFileInfo->mBlessedPackageFilename));

	// Construct the desired environment
	DosUtils::EnvironmentList_AddVariable(environment, MOG_EnvVar_ProjectName, MOG_ControllerProject::GetProjectName());
	DosUtils::EnvironmentList_AddVariable(environment, MOG_EnvVar_BranchName, MOG_ControllerProject::GetBranchName());
	DosUtils::EnvironmentList_AddVariable(environment, MOG_EnvVar_UserName, MOG_ControllerProject::GetUser()->GetUserName());
	DosUtils::EnvironmentList_AddVariable(environment, MOG_EnvVar_PlatformName, mPlatformName);

	DosUtils::EnvironmentList_AddVariable(environment, MOG_EnvVar_WorkingDirectory, packageFileInfo->mPackageWorkspaceDirectory);
	DosUtils::EnvironmentList_AddVariable(environment, MOG_EnvVar_PackageWorkspaceDirectory, packageFileInfo->mPackageWorkspaceDirectory);
	DosUtils::EnvironmentList_AddVariable(environment, MOG_EnvVar_PackageDataDirectory, packageFileInfo->mPackageDataDirectory);
	DosUtils::EnvironmentList_AddVariable(environment, MOG_EnvVar_PackageInputFile, packageFileInfo->mInputPackageTaskFile);
	DosUtils::EnvironmentList_AddVariable(environment, MOG_EnvVar_PackageOutputFile, packageFileInfo->mOutputPackageTaskFile);

	// Check if there were any properties specified?
	ArrayList *properties = packageFileInfo->mPackageProperties->GetApplicableProperties();
	if (properties != NULL)
	{
		// Add each specified property to our environment
		for (int p = 0; p < properties->Count; p++)
		{
			MOG_Property *property = __try_cast<MOG_Property *>(properties->Item[p]);
			if (property->mPropertyKey->Length)
			{
				String *tokenizedValue = MOG_Tokens::GetFormattedString(property->mPropertyValue, NULL, packageFileInfo, mWorkspaceDirectory, mPlatformName);
				DosUtils::EnvironmentList_AddVariable(environment, property->mPropertyKey, tokenizedValue);
			}
		}
	}

	return environment;
}


bool MOG_ControllerPackageMerge::ExecutePackageTool(MOG_PackageMerge_PackageFileInfo *packageFileInfo, String* packageTool)
{
	bool bFailed = false;

	if (packageFileInfo->mPackageStyle == MOG_PackageStyle::TaskFile)
	{
		// Make sure a InputPackageTaskFile exists?
		if (!DosUtils::FileExistFast(packageFileInfo->mInputPackageTaskFile))
		{
			// Make sure to inform the Server about this
			String *message = String::Concat(	S"System did not locate the necessary InputPackageTaskFile.\n",
												S"FILE: ", packageFileInfo->mInputPackageTaskFile, S"\n",
												S"TaskFile package merges always require an InputPackageTaskFile.");
			MOG_Report::ReportMessage(S"Packaging Error", message, S"", MOG_ALERT_LEVEL::ERROR);
			return false;
		}
	}

	//Make sure there is a valid tool specified
	if (packageTool && packageTool->Length)
	{
		// Break up the specified tool command
		String *tool = DosUtils::FileStripArguments(packageTool);
		String *arguments = DosUtils::FileGetArguments(packageTool);
		if (tool->Length)
		{
			// Check for reserved tools?
			if (String::Compare(tool, S"Del", true) == 0 ||
				String::Compare(tool, S"Delete", true) == 0 ||
				String::Compare(tool, S"Remove", true) == 0)
			{
				String *argumentList[] = DosUtils::SplitArguments(arguments);
				if (argumentList->Count == 1)
				{
					String *arg1 = DosUtils::PathEnsureFullPath(argumentList[0], packageFileInfo->mPackageWorkspaceDirectory);
					if (!DosUtils::FileDeleteFast(arg1))
					{
						// Make sure to inform the Server about this
						String *message = String::Concat(	S"MOG experienced an error while trying to execute the 'Delete' command.\n",
															S"The file may not have been properly removed from the package.");
						MOG_Report::ReportMessage(S"ExecutePackageTool", message, S"", MOG_ALERT_LEVEL::ALERT);
					}
				}
				else
				{
					// Make sure to inform the Server about this
					String *message = String::Concat(	S"Incorrect number of arguments for the specified internal command.\n",
														S"The file was not processed correctly.");
					MOG_Report::ReportMessage(S"ExecutePackageTool", message, S"", MOG_ALERT_LEVEL::ALERT);
				}
			}
			else if (String::Compare(tool, S"Copy", true) == 0 )
			{
				String *argumentList[] = DosUtils::SplitArguments(arguments);
				if (argumentList->Count == 2)
				{
					String *arg1 = DosUtils::PathEnsureFullPath(argumentList[0], packageFileInfo->mPackageWorkspaceDirectory);
					String *arg2 = DosUtils::PathEnsureFullPath(argumentList[1], packageFileInfo->mPackageWorkspaceDirectory);
					if (!DosUtils::FileCopyFast(arg1, arg2, true))
					{
						// Make sure to inform the Server about this
						String *message = String::Concat(	S"MOG experienced an error while trying to execute the 'Copy' command.\n",
															S"The file may not have been properly added to the package.");
						MOG_Report::ReportMessage(S"ExecutePackageTool", message, S"", MOG_ALERT_LEVEL::ALERT);
					}
				}
				else
				{
					// Make sure to inform the Server about this
					String *message = String::Concat(	S"Incorrect number of arguments for the specified internal command.\n",
														S"The file was not processed correctly.");
					MOG_Report::ReportMessage(S"ExecutePackageTool", message, S"", MOG_ALERT_LEVEL::ALERT);
				}
			}
			// Attempt to execute the tool
			else
			{
				String *relativeToolPath = DosUtils::PathGetDirectoryPath(tool);

				// Locate the tool within all the system's tools directories
				String *packageToolPath = MOG_ControllerSystem::LocateTool(relativeToolPath, tool);
				if (packageToolPath->Length)
				{
					ArrayList* environment = GetPackageToolEnvironment(relativeToolPath, packageFileInfo);
					String *outputLog = S"";

					// Launch the process in a seperate window
					int errorCode = DosUtils::SpawnDosCommand(packageFileInfo->mPackageWorkspaceDirectory, packageToolPath, arguments, environment, &outputLog, packageFileInfo->mHideWindow);
					if (errorCode == 0)
					{
						// Append the log onto our slave's log
						MOG_ControllerSystem::GetCommandManager()->AppendMessageToOutputLog(outputLog);
					}
					else
					{
						// Make sure to inform the Server about this
						String *message = String::Concat(	S"The '", tool, S"' PackageTool returned ErrorCode ", errorCode.ToString(), S"\n",
															S"Confirm this seriousness of this error with your MOG Administrator.");
						MOG_Report::ReportMessage(S"ExecutePackageTool", message, S"", MOG_ALERT_LEVEL::ALERT);
					}
				}
				else
				{
					// Make sure to inform the Server about this
					String *message = String::Concat(	S"The System failed to locate the '", tool, S"' PackageTool.\n",
														S"Confirm this tool exists.");
					MOG_Report::ReportMessage(S"ExecutePackageTool", message, S"", MOG_ALERT_LEVEL::ERROR);
					bFailed = true;
				}
			}
		}
	}
	else
	{
		// Make sure to inform the Server about this
		String *message = String::Concat(	S"This project does not appear to be setup to perform packaging because there is no defined PackageTool.\n");
		MOG_Report::ReportMessage(S"Packaging Failed", message, S"", MOG_ALERT_LEVEL::ALERT);
		bFailed = true;
	}

	if (!bFailed)
	{
		// Check if the InputPackageTaskFile still exists?
		if (DosUtils::FileExistFast(packageFileInfo->mInputPackageTaskFile))
		{
			// Delete the old InputPackageTaskFile
			DosUtils::FileDeleteFast(packageFileInfo->mInputPackageTaskFile);
		}
		
		return true;
	}

	return false;
}


PackageList *MOG_ControllerPackageMerge::ExecutePackageCommands_DeletePackageFile(MOG_PackageMerge_PackageFileInfo *packageFileInfo)
{
	PackageList *tempPackageFiles = new PackageList();
	tempPackageFiles->Add(packageFileInfo);
	return ExecutePackageCommands_DeletePackageFile(tempPackageFiles);
}


PackageList *MOG_ControllerPackageMerge::ExecutePackageCommands_DeletePackageFile(PackageList *packageFiles)
{
	PackageList *affectedPackages = new PackageList();

	// Walk through all the listed packages
	for (int p = 0; p < packageFiles->Count; p++)
	{
		MOG_PackageMerge_PackageFileInfo *packageFileInfo = __try_cast<MOG_PackageMerge_PackageFileInfo*>(packageFiles->Item[p]);

		// Check if there is already a package file in the local workspace?
		if (DosUtils::FileExistFast(packageFileInfo->mPackageFile))
		{
			// Perform the default action and simply delete the package file from the designated workspace so we start fresh
			if (!DosUtils::FileDeleteFast(packageFileInfo->mPackageFile))
			{
				String *message = String::Concat(	S"System failed to remove the existing package file before performing the package rebuild.\n",
													S"PACKAGEFILE: ", packageFileInfo->mPackageFile, S"\n",
													S"PLATFORM: ", mPlatformName, S"\n",
													S"This rebuilt package may contain undesired items.");
				MOG_Report::ReportMessage(S"Failed to delete existing package file before rebuilding", message, S"", MOG_ALERT_LEVEL::ERROR);
			}
		}

		// Make sure we have a tokenized package delete command
		if (packageFileInfo->mPackageCommand_DeletePackageFile->Length)
		{
			// Execute this package command
			if (!ExecutePackageCommand(packageFileInfo, packageFileInfo->mPackageCommand_DeletePackageFile))
			{
				String *message = String::Concat(	S"System failed to execute the PackageCommand_DeletePackage command before performing the package rebuild.\n",
													S"PACKAGEFILE: ", packageFileInfo->mPackageFile, S"\n",
													S"PLATFORM: ", mPlatformName, S"\n",
													S"Contact your MOG Administrator to verify the seriousness of this error.");
				MOG_Report::ReportMessage(S"Failed to execute PackageCommand_DeletePackage", message, S"", MOG_ALERT_LEVEL::ERROR);
			}
		}

		// Check if this initial revision contains a base package file?
		// Now check if this package was created in MOG or imported to MOG by checking this package's initial revision?
		ArrayList* packageRevisions = MOG_DBAssetAPI::GetAllAssetRevisions(packageFileInfo->mPackageAssetFilename);
		if (packageRevisions &&
			packageRevisions->Count)
		{
			// Get the initial revision of this package
			String* initialRevision = dynamic_cast<String*>(packageRevisions->Item[packageRevisions->Count - 1]);
			MOG_Filename* initialPackageFilename = MOG_ControllerRepository::GetAssetBlessedVersionPath(packageFileInfo->mPackageAssetFilename, initialRevision);

			// Use the imported assets of this blessed package asset as our source
			MOG_Properties* initialPackageProperties = new MOG_Properties(initialPackageFilename);
			String *processedDirectory = MOG_ControllerAsset::GetAssetProcessedDirectory(initialPackageProperties, mPlatformName);
			String *basePackageFile = String::Concat(processedDirectory, S"\\", DosUtils::PathGetFileName(packageFileInfo->mRelativePackageFile));
			// Check if the source file exists?
			if (DosUtils::FileExistFast(basePackageFile))
			{
				// Copy over the initial base revision of this package
				if (!DosUtils::FileCopyFast(basePackageFile, packageFileInfo->mPackageFile, true))
				{
					String *message = String::Concat(	S"System failed to seed the PackageWorkspaceDirectory with the initial revision of the PackageFile.\n",
														S"PACKAGEFILE: ", basePackageFile, S"\n",
														S"PLATFORM: ", mPlatformName, S"\n",
														S"This package will only be rebuilt using the contained assets that MOG knows about.");
					MOG_Report::ReportMessage(S"Failed to seed packagefile during rebuild package", message, S"", MOG_ALERT_LEVEL::ERROR);
				}
				// Make sure we clear the ReadOnly attribute because this package came directly out of the repository
				DosUtils::SetAttributes(packageFileInfo->mPackageFile, FileAttributes::Normal);
			}
		}

		// Add this package to the affected packages
		affectedPackages->Add(packageFileInfo);
	}

	return affectedPackages;
}


PackageList *MOG_ControllerPackageMerge::ExecutePackageCommands_LateResolver(MOG_PackageMerge_PackageFileInfo *packageFileInfo)
{
	PackageList *tempPackageFiles = new PackageList();
	tempPackageFiles->Add(packageFileInfo);
	return ExecutePackageCommands_LateResolver(tempPackageFiles);
}


PackageList *MOG_ControllerPackageMerge::ExecutePackageCommands_LateResolver(PackageList *packageFiles)
{
	PackageList *affectedPackages = new PackageList();

	// Walk through all the listed packages
	for (int p = 0; p < packageFiles->Count; p++)
	{
		MOG_PackageMerge_PackageFileInfo *packageFileInfo = __try_cast<MOG_PackageMerge_PackageFileInfo*>(packageFiles->Item[p]);

		// Make sure we have a tokenized late resolver command
		if (packageFileInfo->mPackageCommand_ResolveLateResolvers->Length)
		{
			// Execute this package command
			if (ExecutePackageCommand(packageFileInfo, packageFileInfo->mPackageCommand_ResolveLateResolvers))
			{
				affectedPackages->Add(packageFileInfo);
			}
		}
		else
		{
			// Make sure to inform the Server about this
			String *message = String::Concat(	S"The system appears to be missing the PackageCommand_LateResolver command for ", packageFileInfo->mPackageAssetFilename->GetAssetFullName(), S"\n",
												S"Confirm this property is correct.");
			MOG_Report::ReportMessage(S"ExecutePackageCommands_LateResolver", message, S"", MOG_ALERT_LEVEL::ERROR);
		}
	}

	return affectedPackages;
}


PackageList *MOG_ControllerPackageMerge::ExecutePackageCommands_AddAsset(MOG_ControllerAsset *asset)
{
	// Default to use the Asset's currently assigned packages
	ArrayList *packageAssignmentProperties = asset->GetProperties(mPlatformName)->GetApplicablePackages();
	return ExecutePackageCommands_AddAsset(asset, packageAssignmentProperties);
}


PackageList *MOG_ControllerPackageMerge::ExecutePackageCommands_AddAsset(MOG_ControllerAsset *asset, MOG_PackageMerge_PackageFileInfo *packageFileInfo)
{
	// Get all of the asset's assigned packages
	ArrayList *packageAssignmentProperties = asset->GetProperties(mPlatformName)->GetApplicablePackages(packageFileInfo->mPackageAssetFilename->GetAssetFullName());
	return ExecutePackageCommands_AddAsset(asset, packageAssignmentProperties);
}


PackageList *MOG_ControllerPackageMerge::ExecutePackageCommands_AddAsset(MOG_ControllerAsset *asset, ArrayList *packageAssignmentProperties)
{
	PackageList *affectedPackages = new PackageList();

	// Walk through all the listed packageAssignmentProperties
	for (int p = 0; p < packageAssignmentProperties->Count; p++)
	{
		MOG_Property *packageAssignmentProperty = __try_cast<MOG_Property*>(packageAssignmentProperties->Item[p]);

		// Find the PackageFileInfo for this package
		// Check if this package is an '{All}' platform?
		MOG_Filename* packageAssetFilename = new MOG_Filename(MOG_ControllerPackage::GetPackageName(packageAssignmentProperty->mPropertyKey));
		if (!packageAssetFilename->IsPlatformSpecific())
		{
			// Check if there is a platform specific version of this package?
			MOG_Filename* platformSpecificPackageAssetFilename = MOG_ControllerProject::GetPlatformSpecificAsset(packageAssetFilename, mPlatformName);
			if (platformSpecificPackageAssetFilename)
			{
				// Substitute this platform specific package in place of the generic one
				packageAssetFilename = platformSpecificPackageAssetFilename;
			}
		}

		// Get the preinitialized PackageFileInfo for this package
		MOG_PackageMerge_PackageFileInfo *packageFileInfo = mPackageFiles->FindByAssetName(packageAssetFilename->GetAssetFullName());
		if (packageFileInfo)
		{
			// Make sure this package is valid?
			if (MOG_ControllerProject::DoesAssetExists(packageFileInfo->mPackageAssetFilename))
			{
			    // Ask the asset if it has a package command?
			    String *assetCommandFormat = asset->GetProperties(mPlatformName)->PackageCommand_Add;
			    if (assetCommandFormat->Length == 0)
			    {
				    assetCommandFormat = packageFileInfo->mPackageCommand_Add;
			    }

			    // Check if we have actually obtained a package command?
			    if (assetCommandFormat->Length)
			    {
				    // Execute the asset's package commands for this package
				    if (ExecutePackageCommands(asset, assetCommandFormat, packageAssignmentProperty, packageFileInfo))
				    {
					    // Add this package as an affected package file
					    affectedPackages->Add(packageFileInfo);
				    }
			    }
			    else
			    {
				    // Check if this asset's files were synced?
				    if (asset->GetProperties()->SyncFiles)
				    {
					    // Add this package as an affected package file
					    affectedPackages->Add(packageFileInfo);
				    }
				    else
				    {
					    // Make sure to inform the Server about a missing package command
					    String *message = String::Concat(	S"The asset was not successfully packaged because 'PackageCommand_Add' was not defined.\n",
														    S"ASSET: ", asset->GetAssetFilename()->GetAssetFullName(), S"\n",
														    S"PACKAGE: ", packageAssetFilename->GetAssetFullName());
					    MOG_Report::ReportMessage(S"ExecutePackageCommands_AddAsset", message, S"", MOG_ALERT_LEVEL::ERROR);
				    }
			    }
		    }
			else
			{
				String *message = String::Concat(	S"The asset was not successfully packaged because the package does not exist in the project.\n",
													S"ASSET: ", asset->GetAssetFilename()->GetAssetFullName(), S"\n",
													S"PACKAGE: ", packageAssetFilename->GetAssetFullName());
				MOG_Report::ReportMessage(S"ExecutePackageCommands_AddAsset", message, S"", MOG_ALERT_LEVEL::ERROR);
			}
		}
	}

	return affectedPackages;
}


PackageList *MOG_ControllerPackageMerge::ExecutePackageCommands_RemoveAsset(MOG_ControllerAsset *asset)
{
	// Default to use the Asset's currently assigned packages
	ArrayList *packageAssignmentProperties = asset->GetProperties(mPlatformName)->GetApplicablePackages();
	return ExecutePackageCommands_RemoveAsset(asset, packageAssignmentProperties);
}


PackageList *MOG_ControllerPackageMerge::ExecutePackageCommands_RemoveAsset(MOG_ControllerAsset *asset, MOG_PackageMerge_PackageFileInfo *packageFileInfo)
{
	ArrayList *packageAssignmentProperties = asset->GetProperties(mPlatformName)->GetApplicablePackages(packageFileInfo->mPackageAssetFilename->GetAssetFullName());
	return ExecutePackageCommands_RemoveAsset(asset, packageAssignmentProperties);
}


PackageList *MOG_ControllerPackageMerge::ExecutePackageCommands_RemoveAsset(MOG_ControllerAsset *asset, ArrayList *packageAssignmentProperties)
{
	PackageList *affectedPackages = new PackageList();

	// Walk through all the listed packageAssignmentProperties
	for (int p = 0; p < packageAssignmentProperties->Count; p++)
	{
		MOG_Property *packageAssignmentProperty = __try_cast<MOG_Property*>(packageAssignmentProperties->Item[p]);

		// Find the PackageFileInfo for this package
		// Check if this package is an '{All}' platform?
		MOG_Filename* packageAssetFilename = new MOG_Filename(MOG_ControllerPackage::GetPackageName(packageAssignmentProperty->mPropertyKey));
		if (!packageAssetFilename->IsPlatformSpecific())
		{
			// Check if there is a platform specific version of this package?
			MOG_Filename* platformSpecificPackageAssetFilename = MOG_ControllerProject::GetPlatformSpecificAsset(packageAssetFilename, mPlatformName);
			if (platformSpecificPackageAssetFilename)
			{
				// Substitute this platform specific package in place of the generic one
				packageAssetFilename = platformSpecificPackageAssetFilename;
			}
		}

		// Get the preinitialized PackageFileInfo for this package
		MOG_PackageMerge_PackageFileInfo *packageFileInfo = mPackageFiles->FindByAssetName(packageAssetFilename->GetAssetFullName());
		if (packageFileInfo)
		{
			// Make sure this package is valid?
			if (MOG_ControllerProject::DoesAssetExists(packageFileInfo->mPackageAssetFilename))
			{
			    // Ask the asset if it has a package command?
			    String *assetCommandFormat = asset->GetProperties(mPlatformName)->PackageCommand_Remove;
			    if (assetCommandFormat->Length == 0)
			    {
				    assetCommandFormat = packageFileInfo->mPackageCommand_Remove;
			    }
    
			    // Check if we have actually obtained a package command?
			    if (assetCommandFormat->Length)
			    {
				    // Execute the asset's package commands for this package
				    if (ExecutePackageCommands(asset, assetCommandFormat, packageAssignmentProperty, packageFileInfo))
				    {
					    // Add this package as an affected package file
					    affectedPackages->Add(packageFileInfo);
				    }
			    }
		    }
	    }
	}

	return affectedPackages;
}


bool MOG_ControllerPackageMerge::ExecutePackageCommands(MOG_ControllerAsset *asset, String *assetCommandFormat, MOG_Property *packageAssignmentProperty, MOG_PackageMerge_PackageFileInfo *packageFileInfo)
{
	// Make sure we have a valid packageFileInfo?
	if (packageFileInfo == NULL)
	{
		return false;
	}

	// Make sure this package has a valid workspace?
	if (packageFileInfo->mPackageWorkspaceDirectory->Length == 0)
	{
		return false;
	}

	// Gather the required package information
	// Check if this package is an '{All}' platform?
	MOG_Filename* packageAssetFilename = new MOG_Filename(MOG_ControllerPackage::GetPackageName(packageAssignmentProperty->mPropertyKey));
	if (!packageAssetFilename->IsPlatformSpecific())
	{
		// Check if there is a platform specific version of this package?
		MOG_Filename* platformSpecificPackageAssetFilename = MOG_ControllerProject::GetPlatformSpecificAsset(packageAssetFilename, mPlatformName);
		if (platformSpecificPackageAssetFilename)
		{
			// Substitute this platform specific package in place of the generic one
			packageAssetFilename = platformSpecificPackageAssetFilename;
		}
	}
	// Obtain the package's platform
	String *packagePlatform = packageAssetFilename->GetAssetPlatform();
	// Make sure this package's platform is valid for this asset?
	if (MOG_ControllerProject::IsPlatformValidForAsset(asset->GetAssetFilename(), packagePlatform))
	{
		// Check if the PackageFile's directory is missing?
		String *packageFilePath = DosUtils::PathGetDirectoryPath(packageFileInfo->mPackageFile);
		if (!DosUtils::DirectoryExistFast(packageFilePath))
		{
			// Always build the directory where the PackageFile will be created to make it easier on the packaging tools
			if (!DosUtils::DirectoryCreate(packageFilePath))
			{
				// Make sure to inform the Server about this error
				String *message = String::Concat(	S"The system was unable to create directory ", packageFilePath, S"\n",
													S"The PackageTools will not be able function properly without write access to this location.");
				MOG_Report::ReportMessage(S"CreatePackageCommands", message, S"", MOG_ALERT_LEVEL::ERROR);
				return false;
			}
		}

		// Make sure we obtain valid properties for this asset
		MOG_Properties *pProperties = asset->GetProperties(packagePlatform);
		if (pProperties)
		{
			// Process all the files within the Asset's Files directory
			String *filesPath = MOG_ControllerAsset::GetAssetProcessedDirectory(pProperties, packagePlatform);
			ArrayList *files = NULL;
			// Check the asset's package command scope?
			switch (pProperties->PackageCommand_Propagation)
			{
				case MOG_PackageCommandPropagation::PerRecursiveFile:
					// Get all the files in the asset (including subdirectories)
					files = DosUtils::FileGetRecursiveList(filesPath, S"*.*");
					break;
				case MOG_PackageCommandPropagation::PerRootFile:
					// Get all the files in the root of the asset
					files = new ArrayList(DosUtils::FileGetList(filesPath, S"*.*"));
					break;
				case MOG_PackageCommandPropagation::PerAsset:
					// Create a simple array with just our asset
					files = new ArrayList();
					files->Add(filesPath);
					break;
			}

			// Make sure wew have files to package?
			if (files != NULL &&
				files->Count)
			{
				bool bPackaged = false;

				// Process each file within the asset
				for (int f = 0; f < files->Count; f++)
				{
					// Format the packageCommand
					String *file = dynamic_cast<String *>(files->Item[f]);
					MOG_Filename *tempFilename = new MOG_Filename(file);
					String *packageCommand = MOG_Tokens::GetFormattedString(assetCommandFormat, tempFilename, packageFileInfo, mWorkspaceDirectory, mPlatformName, asset->GetProperties(), packageAssignmentProperty->mPropertyKey);
					if (packageCommand->Length)
					{
						// Execute this package command
						if (!ExecutePackageCommand(packageFileInfo, packageCommand))
						{
							return false;
						}
					}
					else
					{
						// Make sure to inform the Server about this error
						String *message = String::Concat(	S"The tokenization of the specified package command failed.\n",
															S"COMMAND: ", assetCommandFormat, S"\n",
															S"PACKAGE: ", packageAssetFilename->GetAssetFullName(), S"\n"
															S"ASSET: ", asset->GetAssetFilename()->GetAssetFullName());
						MOG_Report::ReportMessage(S"ExecutePackageCommands", message, S"", MOG_ALERT_LEVEL::ERROR);
						return false;
					}
				}
			}
			else
			{
				// It is possible for newly created packages to be missing files so ignore this error if this is a package
				if (!asset->GetProperties()->IsPackage)
				{
					// Make sure to inform the Server about this error
					String *message = String::Concat(	S"Asset appears to be missing files.\n",
														S"ASSET: ", asset->GetAssetFilename()->GetAssetFullName(), S"\n",
														S"Confirm this asset contains valid files for the '", packagePlatform, S"' platform.");
					MOG_Report::ReportMessage(S"ExecutePackageCommands", message, S"", MOG_ALERT_LEVEL::ERROR);
					return false;
				}
			}

			return true;
		}
	}

	return false;
}


bool MOG_ControllerPackageMerge::ExecutePackageCommand(MOG_PackageMerge_PackageFileInfo *packageFileInfo, String *packageCommand)
{
	bool bCompleted = false;

	// Check the package style?
	switch (packageFileInfo->mPackageStyle)
	{
		case MOG_PackageStyle::Disabled:
			break;
		case MOG_PackageStyle::Simple:
			{
				if (ExecutePackageTool(packageFileInfo, packageCommand))
				{
					// Indicate this was successfully packaged
					bCompleted = true;
				}
			}
			break;
		case MOG_PackageStyle::TaskFile:
			{
				// Open this platform's Package File
				MOG_Ini *packageCommandFile = new MOG_Ini();
				if (packageCommandFile->Open(packageFileInfo->mInputPackageTaskFile, FileShare::ReadWrite))
				{
					// Add the asset's package command to the PackageCommands file
					packageCommandFile->PutArrayItem("Commands", packageCommand, S"");

					// Close the file
					packageCommandFile->Close();
					packageCommandFile = NULL;

					// Indicate this was successfully packaged
					bCompleted = true;
				}
				else
				{
					// Make sure to inform the Server about this error
					String *message = String::Concat(	S"The System was unable to open the InputPackageTaskFile.\n",
														S"FILE: ", packageFileInfo->mInputPackageTaskFile, S"\n",
														S"Confirm access rights to this location");
					MOG_Report::ReportMessage(S"CreatePackageCommands", message, S"", MOG_ALERT_LEVEL::ERROR);
				}
			}
			break;
	}

	return bCompleted;
}


ArrayList* MOG_ControllerPackageMerge::AutoDetectPackageLogFiles(String* packageWorkspaceDirectory, String* packageFilePath)
{
	//String *logFilename = String::Concat(packageFileInfo->mPackageWorkspaceDirectory, S"\\Rip.", platformName, S".log");
	//logFiles->Add(logFilename);
	
	return new ArrayList;
}


PackageList *MOG_ControllerPackageMerge::GetPackageFilesFromPackageAssignmentProperties(ArrayList* packageAssignments, MOG_Properties *properties, String *jobLabel)
{
	PackageList* packageFiles = new PackageList();

	// Loop the the specified properties
	for (int p = 0; p < packageAssignments->Count; p++)
	{
		MOG_Property *packageAssignment = __try_cast<MOG_Property*>(packageAssignments->Item[p]);

		// Get the name of this package from the property
		// Check if this package is an '{All}' platform?
		MOG_Filename* packageAssetFilename = new MOG_Filename(MOG_ControllerPackage::GetPackageName(packageAssignment->mPropertyKey));
		if (String::Compare(packageAssetFilename->GetAssetPlatform(), S"All", true) == 0)
		{
			// Check if there is a platform specific version of this package?
			MOG_Filename* platformSpecificPackageAssetFilename = MOG_ControllerProject::GetPlatformSpecificAsset(packageAssetFilename, mPlatformName);
			if (platformSpecificPackageAssetFilename)
			{
				// Substitute this platform specific package in place of the generic one
				packageAssetFilename = platformSpecificPackageAssetFilename;
			}
			else
			{
				// Looks like we are missing this platform specific package
				// Check if this asset is ripped for divergent platforms?
				if (properties->AssetRipper->Length &&
					properties->DivergentPlatformDataType)
				{
					// We should warn the user about this platform divergent asset not being able to be packaged
					String *message = String::Concat(	S"The asset being packaged has been ripped with platform-specific data yet the project doesn't yet have a platform-specific version of this package.\n",
														S"ASSET: ", properties->GetAssetFilename()->GetAssetFullName(), S"\n",
														S"PLATFORM: ", mPlatformName, S"\n",
														S"PACKAGE: ", packageAssetFilename->GetAssetFullName(), S"\n",
														S"A platform-specific package needs to be created in order to package the platform-specific contents of this asset.");
					MOG_Report::ReportMessage(S"Platform-specific Packaging", message, "", MOG_ALERT_LEVEL::ERROR);
				}
			}
		}

		// Check if we have already generated this package's PackageFileInfo?
		MOG_PackageMerge_PackageFileInfo *packageFileInfo = packageFiles->FindByAssetName(packageAssetFilename->GetAssetFullName());
		if (!packageFileInfo)
		{
			packageFileInfo = new MOG_PackageMerge_PackageFileInfo(packageAssetFilename, mWorkspaceDirectory, mPlatformName, jobLabel);
			// Make sure we obtained a valid PackageFileInfo by checking the mPackageProperties?
			if (packageFileInfo->mPackageProperties)
			{
				packageFileInfo = VerifyWorkingDirectory(packageFileInfo, false);
				if (packageFileInfo)
				{
					packageFiles->Add(packageFileInfo);
				}
			}
		}
	}

	return packageFiles;
}


PackageList *MOG_ControllerPackageMerge::GetAssociatedPackageList(MOG_Filename *assetFilename, MOG_Properties *properties, String* jobLabel)
{
	ArrayList *packageAssignments = new ArrayList();
	
	// Check if this is a package?
	if (properties->IsPackage)
	{
		// Add ourself as needing to be rebuilt
		packageAssignments->Add(MOG_PropertyFactory::MOG_Relationships::New_PackageAssignment(MOG_ControllerPackage::GetPackageName(assetFilename->GetAssetFullName()), S"", S""));
	}
	// Check if this is a packaged asset?
	if (properties->IsPackagedAsset)
	{
		// Get all the assigned packages of this asset as our list of package assignment properties
		packageAssignments = properties->GetApplicablePackages();
	}
	
	// Convert the package assignment properties and return a PackageFileInfo array
	return GetPackageFilesFromPackageAssignmentProperties(packageAssignments, properties, jobLabel);
}


MOG_Filename *MOG_ControllerPackageMerge::LocateAsset_LocalWorkspaceOnly(MOG_Filename *assetName)
{
	MOG_Filename *assetFilename = NULL;

	// Check if this file is listed in the local workspace?
	// Get the locally updated asset list
	String *boxName = String::Concat(S"Local", S".", MOG_ControllerSystem::GetComputerName());
	String *filter = String::Concat(mWorkspaceDirectory, S"\\*");
	ArrayList *localAssets = MOG_DBInboxAPI::InboxGetAssetList(boxName, MOG_ControllerProject::GetUserName(), S"", filter);
	if (localAssets)
	{
		// Scan the list of local workspace assets
		for (int a = 0; a < localAssets->Count; a++)
		{
			// Check if this asset matches?
			MOG_Filename *testFilename = __try_cast<MOG_Filename*>(localAssets->Item[a]);
			if (String::Compare(testFilename->GetAssetFullName(), assetName->GetAssetFullName(), true) == 0)
			{
				// Indicate this asset was found to be listed in the local assets
				assetFilename = testFilename;
				break;
			}
		}
	}

	return assetFilename;
}


MOG_Filename *MOG_ControllerPackageMerge::LocateAsset_RepositoryOnly(MOG_Filename *assetName)
{
	MOG_Filename *assetFilename = NULL;

	// Check if we are still missing an assetFilename?
	if (assetFilename == NULL)
	{
		// Get the version for this asset as specified in the sync tables
		String *revision = MOG_DBSyncedDataAPI::GetLocalAssetVersion(MOG_ControllerSystem::GetComputerName(), MOG_ControllerProject::GetProjectName(), mPlatformName, mWorkspaceDirectory, MOG_ControllerProject::GetUserName_DefaultAdmin(), assetName);
		if (revision->Length)
		{
			// Build the blessedAssetFilename
			assetFilename = new MOG_Filename(MOG_ControllerRepository::GetAssetBlessedVersionPath(assetName, revision));
		}
	}

	return assetFilename;
}


MOG_Filename *MOG_ControllerPackageMerge::LocateAsset_LocalWorkspaceAndRepository(MOG_Filename *assetName)
{
	MOG_Filename *assetFilename = NULL;

	assetFilename = LocateAsset_LocalWorkspaceOnly(assetName);
	if (!assetFilename)
	{
		assetFilename = LocateAsset_RepositoryOnly(assetName);
	}

	return assetFilename;
}

