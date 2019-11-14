//--------------------------------------------------------------------------------
//	MOG_ControllerLibrary.cpp
//	
//	
//--------------------------------------------------------------------------------

#include "StdAfx.h"

#include "MOG_Define.h"
#include "MOG_ControllerAsset.h"
#include "MOG_ControllerRepository.h"
#include "MOG_ControllerSystem.h"
#include "MOG_ControllerProject.h"
#include "MOG_DBPostCommandAPI.h"
#include "MOG_Prompt.h"
#include "MOG_Tokens.h"
#include "FilePattern.h"

#include "MOG_ControllerLibrary.h"

using namespace System::Collections::Generic;
using namespace MOG_CoreControls;

String *MOG_ControllerLibrary::GetWorkingDirectory()
{
	return mWorkingDirectory;
}

void MOG_ControllerLibrary::SetWorkingDirectory(String *workingDirectory)
{
	mWorkingDirectory = workingDirectory;
}

bool MOG_ControllerLibrary::SetReadOnly(MOG_Filename *filename, bool readOnly)
{
	// Build the targetFile for this library asset
	String *targetFile = ConstructLocalFilenameFromAssetName(filename);
	// Determin the proper attribute
	FileAttributes attribute = FileAttributes::Normal;
	if (readOnly)
	{
		attribute = FileAttributes::ReadOnly;
	}
	// set the targetFile's attribute
	return DosUtils::SetAttributes(targetFile, attribute);
}

bool MOG_ControllerLibrary::GetLatest(ArrayList *assetFilenames)
{
	bool bSuccess = false;

	ProgressDialog* progress = new ProgressDialog(S"GetLatest", S"", new DoWorkEventHandler(NULL, &GetLatest_Worker), assetFilenames, true);
	if (progress->ShowDialog() == DialogResult::OK)
	{
		bSuccess = *dynamic_cast<__box bool*>(progress->WorkerResult);
	}

	return bSuccess;
}

void MOG_ControllerLibrary::GetLatest_Worker(Object* sender, DoWorkEventArgs* e)
{
	BackgroundWorker* worker = dynamic_cast<BackgroundWorker*>(sender);
	ArrayList* assetFilenames = dynamic_cast<ArrayList*>(e->Argument);

	e->Result = __box(true);
	
	__try
	{
		// Initialize our GetLatest to ensure things are all setup correctly
		// Reset our YesToAll and NoToAll variables
		mGetLatest_OverwriteFile->Reset();
		// FileHamster integration	
		MOG_FileHamsterIntegration::Initialize();

		// Perform the GetLatest on all of the specified asset
		for (int a = 0; a < assetFilenames->Count && !worker->CancellationPending; a++)
		{
			MOG_Filename *assetFilename = dynamic_cast<MOG_Filename *>(assetFilenames->Item[a]);

			String *message = String::Concat(	S"Syncing:\n",
												S"     ", assetFilename->GetAssetClassification(), S"\n",
												S"     ", assetFilename->GetAssetLabel());
			worker->ReportProgress(a * 100 / assetFilenames->Count, message);

			if (!GetLatest(assetFilename, worker))
			{
				// Something failed and the user aborted
				e->Result = __box(false);
				break;
			}
		}
	}
	__finally
	{
		// Indicate we have finished the GetLatest
		// FileHamster integration	
		MOG_FileHamsterIntegration::FileHamsterUnPause();
	}
}

bool MOG_ControllerLibrary::GetLatest(MOG_Filename *assetFilename, BackgroundWorker* worker)
{
	// GetLatest to latest revision of this asset
	return GetLatestDataForLibraryAsset(assetFilename, true, worker);
}


bool MOG_ControllerLibrary::Unsync(MOG_Filename *assetFilename)
{
	// Make sure we clean up after this asset just in case it had already been synced
	String *localFile = ConstructLocalFilenameFromAssetName(assetFilename);
	return DosUtils::DeleteFast(localFile);
}


bool MOG_ControllerLibrary::CheckOut(ArrayList *assetFilenames, String *comment)
{
	bool bSuccess = false;

	List<Object*>* args = new List<Object*>();
	args->Add(assetFilenames);
	args->Add(comment);

	ProgressDialog* progress = new ProgressDialog(S"Checking out files", S"Please wait...", new DoWorkEventHandler(NULL, &CheckOut_Worker), args, true);
	if (progress->ShowDialog() == DialogResult::OK)
	{
		bSuccess = *dynamic_cast<__box bool*>(progress->WorkerResult);
	}
	
	return bSuccess;
}

void MOG_ControllerLibrary::CheckOut_Worker(Object* sender, DoWorkEventArgs* e)
{
	BackgroundWorker* worker = dynamic_cast<BackgroundWorker*>(sender);
	List<Object*>* args = dynamic_cast<List<Object*>*>(e->Argument);
	ArrayList* assetFilenames = dynamic_cast<ArrayList*>(args->Item[0]);
	String* comment = dynamic_cast<String*>(args->Item[1]);

	e->Result = __box(true);

	try
	{
		// Initialize our GetLatest to ensure things are all setup correctly
		// Reset our YesToAll and NoToAll variables
		mGetLatest_OverwriteFile->Reset();
		// FileHamster integration	
		MOG_FileHamsterIntegration::Initialize();

		// Check Out each asset?
		for (int a = 0; a < assetFilenames->Count; a++)
		{
			MOG_Filename *assetFilename = __try_cast<MOG_Filename *>(assetFilenames->Item[a]);
			bool bRetry = false;

			String *message = String::Concat(	S"Checkout:\n",
												S"     ", assetFilename->GetAssetClassification(), S"\n",
												S"     ", assetFilename->GetAssetLabel());

			if (worker != NULL)
			{
				worker->ReportProgress(a * 100 / assetFilenames->Count, message);
			}

			// Do this in a loop so we can ask the user if they want to try it again
			do
			{
				// Proceed to check out this asset
				if (!CheckOut(assetFilename, comment, worker))
				{
					//If we get here the checkout failed, so ask the user what to do
					String *title = S"CheckOut Error";
					// Inform the user we failed to match an asset
					String *message = String::Concat(	S"FILE: ", assetFilename->GetAssetFullName(), S"\n\n",
														S"MOG failed to check out this asset.");

					MOGPromptResult rc = MOG_Prompt::PromptResponse(title, message, Environment::StackTrace, MOGPromptButtons::AbortRetryIgnore);
					
					switch (rc)
					{
					case MOGPromptResult::Abort:	//Don't retry, stop checking things out
						e->Result = __box(false);
						return;
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
	}
	__finally
	{
		// Indicate we have finished the GetLatest
		// FileHamster integration	
		MOG_FileHamsterIntegration::FileHamsterUnPause();
	}
}

bool MOG_ControllerLibrary::CheckOut(MOG_Filename *assetFilename, String *comment)
{
	ArrayList* assetFilenames = new ArrayList();
	assetFilenames->Add(assetFilename);
	
	return CheckOut(assetFilenames, comment);
}

bool MOG_ControllerLibrary::CheckOut(MOG_Filename *assetFilename, String *comment, BackgroundWorker* worker)
{
	String *message = NULL;
	bool bRetry = false;
	bool bLocked = false;

	// Obtain the required lock on the specified asset
	MOG_Command *lockRequest = MOG_ControllerProject::PersistentLock_Request(assetFilename->GetAssetFullName(), comment);
	if (lockRequest->IsCompleted())
	{
		// Indicate we have the lock
		bLocked = true;
	}
	else
	{
		// See if we already have it checked out
		MOG_Command *pLockQuery = MOG_ControllerProject::PersistentLock_Query(assetFilename->GetAssetFullName());
		if (pLockQuery->IsCompleted() &&
			pLockQuery->GetCommand())
		{
			if (String::Compare(pLockQuery->GetCommand()->GetUserName(), MOG_ControllerProject::GetUser()->GetUserName(), true) == 0)
			{
				// We already had the lock
				bLocked = true;
				return true;
			}
			else
			{
				// Indicate that this asset is already locked
				message = String::Concat(	S"File is already locked by '", pLockQuery->GetCommand()->GetUserName(), S"'.\n",
											S"FILE: ", assetFilename->GetAssetFullName(), S"\n\n",
											S"The file was not checked out.\n");
			}
		}
		else
		{
			// Inform the user we failed to match an asset
			message = String::Concat(	S"Failed to obtain the associated lock.\n",
										S"FILE: ", assetFilename->GetAssetFullName(), S"\n\n",
										S"The file was not checked out.\n");
		}
	}
	
	// Make sure we obtained the lock?
	if (bLocked)
	{
		// GetLatest to latest revision of this asset
		if (GetLatestDataForLibraryAsset(assetFilename, false, worker))
		{
			//Checkout was successful
			SetReadOnly(assetFilename, false);
			return true;
		}
		else
		{
			// Release the PersistentLock?
			if (!MOG_ControllerProject::PersistentLock_Release(assetFilename->GetAssetFullName()))
			{
				// Inform the user we failed to match an asset
				message = String::Concat(	S"Failed to release the associated lock.\n",
											S"FILE: ", assetFilename->GetAssetFullName(), S"\n\n",
											S"The file remained locked.\n");
			}
		}
	}

	// Check if we created a message?
	if (message)
	{
		MOG_Prompt::PromptMessage(S"Check Out Failed", message);
	}

	return false;
}


bool MOG_ControllerLibrary::CheckIn(ArrayList *assetFilenames, String *comment, bool bMaintainLock, String *jobLabel)
{
	// Make sure we have a valid jobLabel specified?
	if (jobLabel == NULL || jobLabel->Length == 0)
	{
		// Obtain a unique jobLabel for this post
		jobLabel = String::Concat(S"CheckIn.", MOG_ControllerSystem::GetComputerName(), S".", MOG_Time::GetVersionTimestamp());
	}

	List<Object*>* args = new List<Object*>();
	args->Add(assetFilenames);
	args->Add(comment);
	args->Add(__box(bMaintainLock));
	args->Add(jobLabel);

	bool bSuccess = false;
	
	ProgressDialog* progress = new ProgressDialog(S"Checking in files", S"Please wait...", new DoWorkEventHandler(NULL, &CheckIn_Worker), args, true);
	if (progress->ShowDialog() == DialogResult::OK)
	{
		bSuccess = *dynamic_cast<__box bool*>(progress->WorkerResult);

		if (bSuccess)
		{
			//Post all the assets
			MOG_ControllerProject::PostAssets(MOG_ControllerProject::GetProjectName(), S"Current", jobLabel);
		}
	}

	return bSuccess;
}

void MOG_ControllerLibrary::CheckIn_Worker(Object* sender, DoWorkEventArgs* e)
{
	BackgroundWorker* worker = dynamic_cast<BackgroundWorker*>(sender);
	List<Object*>* args = dynamic_cast<List<Object*>*>(e->Argument);
	ArrayList* assetFilenames = dynamic_cast<ArrayList*>(args->Item[0]);
	String* comment = dynamic_cast<String*>(args->Item[1]);
	bool bMaintainLock = *dynamic_cast<__box bool*>(args->Item[2]);
	String* jobLabel = dynamic_cast<String*>(args->Item[3]);
	e->Result = __box(true);

	// Check In each asset?
	for (int a = 0; a < assetFilenames->Count; a++)
	{
		MOG_Filename *assetFilename = dynamic_cast<MOG_Filename *>(assetFilenames->Item[a]);

		String *message = String::Concat(	S"Checkin:\n",
											S"     ", assetFilename->GetAssetClassification(), S"\n",
											S"     ", assetFilename->GetAssetLabel());

		if (worker != NULL)
		{
			worker->ReportProgress(a * 100 / assetFilenames->Count, message);
		}

		if (!CheckIn(assetFilename, comment, bMaintainLock, jobLabel))
		{
			//one of the checkins failed and the user chose to abort
			e->Result = __box(false);
			break;
		}
	}
}


bool MOG_ControllerLibrary::CheckIn(MOG_Filename *assetFilename, String *comment, bool bMaintainLock, String *jobLabel)
{
	bool bRetry = false;

	do
	{
		String *message = "";

		// Build the importFile for this library asset
		String *targetFile = ConstructLocalFilenameFromAssetName(assetFilename);
		
		String *timestamp = "";
		FileInfo *file = new FileInfo(targetFile);
		if (file != NULL && file->Exists)
		{
			timestamp = MOG_Time::GetVersionTimestamp(file->LastWriteTime);
		}

		// Obtain a new revision for this asset out in the MOG Repository
		MOG_Filename *blessedAssetFilename = MOG_ControllerRepository::GetAssetBlessedVersionPath(assetFilename, timestamp);

		// Build our importFiles list for CreateAsset()
		ArrayList *importFiles = new ArrayList();
		importFiles->Add(targetFile);
		ArrayList *logFiles = NULL;
		String *assetDirectoryPath = "";
		// Build our list of properties
		ArrayList* properties = new ArrayList();
		MOG_Property* propMaintainLock = MOG_PropertyFactory::MOG_Asset_OptionsProperties::New_MaintainLock(bMaintainLock);
		properties->Add(propMaintainLock);
		MOG_Property* propLastComment = MOG_PropertyFactory::MOG_Asset_StatsProperties::New_LastComment(comment);
		properties->Add(propLastComment);
		// Proceed to import the specified file(s)
		MOG_Filename *importedAssetFilename = MOG_ControllerAsset::CreateAsset(blessedAssetFilename, assetDirectoryPath, importFiles, logFiles, properties, false, false);
		if (importedAssetFilename)
		{
			String *creator = MOG_ControllerProject::GetUserName_DefaultAdmin();
			String *owner = MOG_ControllerProject::GetUserName_DefaultAdmin();

			MOG_ControllerAsset *asset = NULL;
			
			// Open the asset
			asset = MOG_ControllerAsset::OpenAsset(importedAssetFilename);
			if (asset)
			{
				// Save the comments
				asset->PostComment(comment, true);
				asset->Close();
			}

			// Proceed to add the asset for posting
			if (MOG_ControllerProject::AddAssetForPosting(importedAssetFilename, jobLabel, owner, creator))
			{
				// Check if the user is not intending to retain the lock on this asset?
				if (!bMaintainLock)
				{
					// Return the asset to read only
					SetReadOnly(assetFilename, true);
				}
				return true;
			}
			else
			{
				// Inform the user we failed to match an asset
				message = String::Concat( S"Failed.\n",
											S"ASSET: ", assetFilename->GetAssetFullName(), S"\n",
											S"MOG was unable to bless the associated asset from the user's Inbox.\n");
			}
		}
		else
		{
			// Inform the user we failed to match an asset
			message = String::Concat( S"Failed.\n",
										S"ASSET: ", assetFilename->GetAssetFullName(), S"\n",
										S"MOG was unable to create the associated asset in the user's Inbox.\n");
		}
			
		//If we get here the checkin failed, so ask the user what to do
		String *title = S"CheckIn Error";
		if (message->Length == 0)
		{
			message = String::Concat(	S"FILE: ", assetFilename->GetAssetFullName(), S"\n",
										S"MOG failed to check this asset in.");
		}

		// Prompt the user for their immediate response
		MOGPromptResult rc = MOG_Prompt::PromptResponse(title, message, Environment::StackTrace, MOGPromptButtons::AbortRetryIgnore);
		switch (rc)
		{
			case MOGPromptResult::OK:
			case MOGPromptResult::Abort:	//Don't retry, stop checking things in
				return false;
				break;
			case MOGPromptResult::Retry:	//Retry the current asset
				bRetry = true;
				break;
			case MOGPromptResult::Ignore:	//Don't retry the current asset, but continue checking in the other assets
				bRetry = false;
				break;
		}
	}while (bRetry);

	//Checkin failed, but user chose to ignore
	return true;
}

bool MOG_ControllerLibrary::UndoCheckOut(ArrayList *assetFilenames)
{
	bool bSuccess = false;

	ProgressDialog* progress = new ProgressDialog(S"Undoing previous checkout", S"", new DoWorkEventHandler(NULL, &UndoCheckOut_Worker), assetFilenames, true);
	if (progress->ShowDialog() == DialogResult::OK)
	{
		bSuccess = *dynamic_cast<__box bool*>(progress->WorkerResult);
	}

	return bSuccess;
}

void MOG_ControllerLibrary::UndoCheckOut_Worker(Object* sender, DoWorkEventArgs* e)
{
	BackgroundWorker* worker = dynamic_cast<BackgroundWorker*>(sender);
	ArrayList* assetFilenames = dynamic_cast<ArrayList*>(e->Argument);

	e->Result = __box(true);

	__try
	{
		// Initialize our GetLatest to ensure things are all setup correctly
		// Reset our YesToAll and NoToAll variables
		mGetLatest_OverwriteFile->Reset();
		// FileHamster integration
		MOG_FileHamsterIntegration::Initialize();

		// Undo the Checkout for each asset?
		for (int a = 0; a < assetFilenames->Count && !worker->CancellationPending; a++)
		{
			MOG_Filename *assetFilename = __try_cast<MOG_Filename *>(assetFilenames->Item[a]);

			String *message = String::Concat(	S"Undo Checkout:\n",
												S"     ", assetFilename->GetAssetClassification(), S"\n",
												S"     ", assetFilename->GetAssetLabel());
			worker->ReportProgress(a * 100 / assetFilenames->Count, message);

			if (!UndoCheckOut(assetFilename, worker))
			{
				//something failed and the user aborted
				e->Result = __box(false);
				break;
			}
		}
	}
	__finally
	{
		// Indicate we have finished the GetLatest
		// FileHamster integration	
		MOG_FileHamsterIntegration::FileHamsterUnPause();
	}
}

bool MOG_ControllerLibrary::UndoCheckOut(MOG_Filename *assetFilename)
{
	ArrayList* assetFilenames = new ArrayList();
	assetFilenames->Add(assetFilename);

	return UndoCheckOut(assetFilenames);
}

bool MOG_ControllerLibrary::UndoCheckOut(MOG_Filename *assetFilename, BackgroundWorker* worker)
{
	// GetLatest to latest revision of this asset
	if (GetLatestDataForLibraryAsset(assetFilename, false, worker))
	{
		// Release the PersistentLock?
		if (MOG_ControllerProject::PersistentLock_Release(assetFilename->GetAssetFullName()))
		{
			// Return the asset to read only
			SetReadOnly(assetFilename, true);
			return true;
		}
	}

	return false;
}


String *MOG_ControllerLibrary::ConstructLocalFilenameFromAssetName(MOG_Filename *assetFilename)
{
	String *targetFile = "";

	// Make sure we have a valid asset filename specified?
	if (assetFilename &&
		assetFilename->GetAssetClassification()->Length &&
		assetFilename->GetAssetLabel()->Length)
	{
		// Construct the targetFile
		String *targetPath = ConstructPathFromLibraryClassification(assetFilename->GetAssetClassification());
		targetFile = Path::Combine(targetPath, assetFilename->GetAssetLabel());
	}

	return targetFile;
}


MOG_Filename *MOG_ControllerLibrary::ConstructAssetNameFromLibraryFile(String *filename)
{
	MOG_Filename *assetFilename = NULL;

	// Check if this filename is already within the library?
	String *path = DosUtils::PathGetDirectoryPath(filename);
	if (IsPathWithinLibrary(path))
	{
		// Assume the filename's path can dictate the classification
		String *classification = ConstructLibraryClassificationFromPath(Path::GetDirectoryName(filename));
		// Build the blessed asset's Filename
		String *assetLabel = DosUtils::PathGetFileName(filename);
		assetFilename = MOG_Filename::CreateAssetName(classification, "All", assetLabel);
	}

	return assetFilename;
}


String *MOG_ControllerLibrary::ConstructBlessedFilenameFromAssetName(MOG_Filename *assetFilename)
{
	// Build a path from the asset's classification
	String* classifiedDirectory = assetFilename->GetAssetClassification()->Replace(S"~", S"\\");

	// Check if the assetFilename already contains a revision?
	String *revision = assetFilename->GetVersionTimeStamp();
	if (revision->Length == 0)
	{
		// Use the slower way and request it from the database
		revision = MOG_ControllerProject::GetAssetCurrentVersionTimeStamp(assetFilename);
	}

	// Build a full path to the blessed att in the repository
	MOG_Filename *blessedAsset = new MOG_Filename(MOG_ControllerRepository::GetAssetBlessedVersionPath(assetFilename, revision));
	// Combine everything so we have our complete filename
	return String::Concat(blessedAsset->GetOriginalFilename(), S"\\Files.Imported\\", blessedAsset->GetAssetLabel());
}


bool MOG_ControllerLibrary::GetLatestDataForLibraryAsset(MOG_Filename *assetFilename, bool bSkipLockedAssets, BackgroundWorker* worker)
{
	bool bRetry = false;
	bool bFailed = false;
	bool bWarnUser = false;
	bool bDeleteFile = false;
	bool bGetLatestFile = false;

	// Check if we ahould skip locked assets?
	if (bSkipLockedAssets)
	{
		// We only want to copy assets which are not locked by us because we will already have a locally modified version
		bool bIsLockedByMe = MOG_ControllerProject::IsLockedByMe(assetFilename->GetAssetFullName());
		if (bIsLockedByMe)
		{
			// Never bother to resync any assets already locked by me
			return true;
		}
	}

	// Establish the source and target paths
	String *srcFile = ConstructBlessedFilenameFromAssetName(assetFilename);
	String *dstFile = ConstructLocalFilenameFromAssetName(assetFilename);
	if (dstFile->Length)
	{
		// FileHamster Integration...
		// Make sure we know about this directory so it can be paused if needed
		MOG_FileHamsterIntegration::FileHamsterPause(dstFile);

		// Get the info of the srcFile
		FileInfo *srcFileInfo = new FileInfo(srcFile);
		// Make sure this srcFile exists
		if (srcFileInfo->Exists)
		{
			// Get the info of the dstFile
			FileInfo *dstFileInfo = new FileInfo(dstFile);
			// Check if the dstFile exists
			if (dstFileInfo->Exists)
			{
				// Check if the files are different?  (Must Use ToFileTime() because sometimes LastWriteTime comparisons can fail)
				if (srcFileInfo->LastWriteTime.ToFileTime() != dstFileInfo->LastWriteTime.ToFileTime())
				{
					// Check if the dstFile is read only?
					if (dstFileInfo->IsReadOnly)
					{
						// Automatically delete ReadOnly files because the user would have not modified it
						bDeleteFile = true;
					}
					else
					{
						// We better warn the user just in case
						bWarnUser = true;
					}
				}
			}
			else
			{
				bGetLatestFile = true;
			}

			// Check if we need to warn the user?
			if (bWarnUser)
			{
				// Ask the user about overwriting their local file
				if (GetLatestAssetFiles_CanOverwriteFile(dstFile, srcFile))
				{
					// Indicate that we can proceed to overwrite their file
					bDeleteFile = true;
				}
			}

			do
			{
				// Check if we need to delete the existing file?
				if (bDeleteFile)
				{
					try
					{
						// Proceed to delete the file
						// Check if this file has the read only flag?
						if (dstFileInfo->IsReadOnly)
						{
							// Clear the local file's attributes
							dstFileInfo->Attributes = FileAttributes::Normal;
						}

						// Delete the local file
						dstFileInfo->Delete();
						// Now that the file has been deleted we can clear this flag in case we later retry the loop
						bDeleteFile = false;

						// We always need to GetLatest the file if we ever delete the file
						bGetLatestFile = true;
					}
					catch(...)
					{
						bFailed = true;
					}
				}

				// Check if we need to GetLatest the file
				if (bGetLatestFile)
				{
					// Looks like we need to copy the file after all but respect whether we have it locked
					MOG_Filename *assetFilename = ConstructAssetNameFromLibraryFile(dstFile);
					bool clearReadOnly = MOG_ControllerProject::IsLockedByMe(assetFilename->GetAssetFullName());
					if (!MOG_ControllerSystem::FileCopyEx(srcFile, dstFile, true, clearReadOnly, worker))
					{
						bFailed = true;
					}
				}

				// Check if we failed?
				if (bFailed)
				{
					String *title = S"File Error";
					String *message = String::Concat(	S"Failed to get latest version of file due to a potential file sharing violation.\n"
														S"FILE: ", assetFilename->GetAssetFullName(), S"\n\n",
														S"Please make this file isn't in use by another application.");

					MOGPromptResult rc = MOG_Prompt::PromptResponse(title, message, Environment::StackTrace, MOGPromptButtons::AbortRetryIgnore);
					switch (rc)
					{
						case MOGPromptResult::OK:
						case MOGPromptResult::Abort:	//Don't retry, stop checking things in
							return false;
							break;
						case MOGPromptResult::Retry:	//Retry the current asset
							bRetry = true;
							break;
						case MOGPromptResult::Ignore:	//Don't retry the current asset, but continue checking in the other assets
							bRetry = false;
							bFailed = false;
							break;
					}
				}
			}while (bRetry);
		}
	}

	// Check if we failed?
	if (!bFailed)
	{
		return true;
	}
	return false;
}

bool MOG_ControllerLibrary::GetLatestAssetFiles_CanOverwriteFile(String *localFile, String *originalFile)
{
	bool bCanOverwrite = true;

	// Get the time information for both files
	FileInfo *originalInfo = new FileInfo(originalFile);
	FileInfo *localInfo = new FileInfo(localFile);

	// Make sure both files exist
	if (originalInfo->Exists && localInfo->Exists)
	{
		// Has the local file been modified?  (Must Use ToFileTime() because sometimes LastWriteTime comparisons can fail)
		if (originalInfo->LastWriteTime.ToFileTime() != localInfo->LastWriteTime.ToFileTime())
		{
			String* message = String::Concat(	S"Your local file has been modified.\n",
												S"          ", Path::GetDirectoryName(localFile), S"\\\n",
												S"          ", Path::GetFileName(localFile), S"\n\n",
												S"Do you want to continue and overwrite your changes?");
			if (mGetLatest_OverwriteFile->PromptResponse("Local File Modified", message) == MOGPromptResult::No)
			{
				// Indicate that we can't overwrite the modified file
				bCanOverwrite = false;
			}
		}
	}

	return bCanOverwrite;
}


bool MOG_ControllerLibrary::AddDirectory(String *directory, String *classification)
{
	// Create a unique jobLabel
	String *jobLabel = String::Concat(S"LibraryAdd.", MOG_ControllerSystem::GetComputerName(), S".", MOG_Time::GetVersionTimestamp());
	
	// setup progress dialog
	List<Object*>* args = new List<Object*>();
	args->Add(directory);
	args->Add(classification);
	args->Add(jobLabel);

	bool bSuccess = false;

	ProgressDialog* progress = new ProgressDialog(S"Importing Assets", S"Please wait while your assets are imported...", new DoWorkEventHandler(NULL, &AddDirectory_Worker), args, false);
	if (progress->ShowDialog() == DialogResult::OK)
	{
		bSuccess = *dynamic_cast<__box bool*>(progress->WorkerResult);

		// Check if we need to post anything?
		if (bSuccess)
		{
			// Post assets
			MOG_ControllerProject::PostAssets(MOG_ControllerProject::GetProjectName(), MOG_ControllerProject::GetBranchName(), jobLabel, true);
		}
	}

	return bSuccess;
}

void MOG_ControllerLibrary::AddDirectory_Worker(Object* sender, DoWorkEventArgs* e)
{
	BackgroundWorker* worker = dynamic_cast<BackgroundWorker*>(sender);
	List<Object*>* args = dynamic_cast<List<Object*>*>(e->Argument);
	String* directory = dynamic_cast<String*>(args->Item[0]);
	String* classification = dynamic_cast<String*>(args->Item[1]);
	String* jobLabel = dynamic_cast<String*>(args->Item[2]);

	// Obtain the list of files within this directory
	ArrayList *files = DosUtils::FileGetRecursiveList(directory, S"*.*");
	if (files)
	{
		// Obtain the specified classification's properties
		MOG_Properties *properties = new MOG_Properties(classification);

		// Add each file
		for (int i = 0; i < files->Count; i++)
		{
			String *thisFilename = __try_cast<String*>(files->Item[i]);
			String *DirName = Path::GetFileName(directory);
			String *relativeFilename = thisFilename->Substring(directory->Length)->Trim(S"\\"->ToCharArray());
			String *relativePath = Path::Combine(DirName, DosUtils::PathGetDirectoryPath(relativeFilename));
			String *thisClassification = classification;
			// Check if we have a valid classification specified?
			if (thisClassification->Length)
			{
				// Append on the relative portion of the file's path
				thisClassification = MOG_Filename::AppendOnClassification(thisClassification, relativePath);
			}

			// Update the dialog
			String *message = String::Concat(	S"Add:\n",
												S"     ", Path::GetDirectoryName(relativeFilename), S"\n",
												S"     ", Path::GetFileName(relativeFilename));
			worker->ReportProgress(i * 100 / files->Count, message);

			// Make sure this does not violate the classification filters
			// Check the classification's inclusion filter.
			if (properties->FilterInclusions->Length)
			{
				FilePattern *inclusions = new FilePattern(properties->FilterInclusions);
				if (inclusions->IsFilePatternMatch(thisFilename) == false)
				{
					// Skip this file as it is not included
					continue;
				}
			}
			// Check the classification's exclusion filter.
			if (properties->FilterExclusions->Length)
			{
				FilePattern *exclusions = new FilePattern(properties->FilterExclusions);
				if (exclusions->IsFilePatternMatch(thisFilename) == true)
				{
					// Skip this file as it is excluded
					continue;
				}
			}

			// Add this file
			if (AddFile(thisFilename, thisClassification, jobLabel))
			{
				e->Result = __box(true);
			}
			else
			{
				e->Result = __box(false);
			}

			// Check for cancel?
			if (worker->CancellationPending)
			{
				// Being that we were canceled, remove all the pending post commands associated with this job
				MOG_DBPostCommandAPI::RemovePost(NULL, jobLabel, S"");
				break;
			}
		}
	}
}


bool MOG_ControllerLibrary::AddFile(String *filename, String *classification)
{
	// Create an array
	ArrayList *filenames = new ArrayList();
	filenames->Add(filename);

	// Add the array
	return AddFiles(filenames, classification);
}


bool MOG_ControllerLibrary::AddFiles(ArrayList *filenames)
{
	return AddFiles(filenames, S"");
}

bool MOG_ControllerLibrary::AddFiles(ArrayList *filenames, String *classification)
{
	bool bSuccess = false;
	
	if (filenames->Count)
	{
		// Create a unique jobLabel
		String *jobLabel = String::Concat(S"LibraryAdd.", MOG_ControllerSystem::GetComputerName(), S".", MOG_Time::GetVersionTimestamp());
		
		// setup progress dialog
		List<Object*>* args = new List<Object*>();
		args->Add(filenames);
		args->Add(classification);
		args->Add(jobLabel);

		ProgressDialog* progress = new ProgressDialog(S"Importing Assets", S"Please wait while your assets are imported...", new DoWorkEventHandler(NULL, &AddFiles_Worker), args, true);
		if (progress->ShowDialog() == DialogResult::OK)
		{
			bSuccess = *dynamic_cast<__box bool*>(progress->WorkerResult);
		
			// Check if we need to post anything?
			if (bSuccess)
			{
				// Post assets
				MOG_ControllerProject::PostAssets(MOG_ControllerProject::GetProjectName(), MOG_ControllerProject::GetBranchName(), jobLabel, true);
			}
		}
	}

	return bSuccess;
}

void MOG_ControllerLibrary::AddFiles_Worker(Object* sender, DoWorkEventArgs* e)
{
	BackgroundWorker* worker = dynamic_cast<BackgroundWorker*>(sender);
	List<Object*>* args = dynamic_cast<List<Object*>*>(e->Argument);
	ArrayList* filenames = dynamic_cast<ArrayList*>(args->Item[0]);
	String* classification = dynamic_cast<String*>(args->Item[1]);
	String* jobLabel = dynamic_cast<String*>(args->Item[2]);

	// Add each file
	for (int i = 0; i < filenames->Count; i++)
	{
		String *filename = __try_cast<String*>(filenames->Item[i]);

		// Update the dialog
		String *message = String::Concat(	S"Add:\n",
											S"     ", Path::GetDirectoryName(filename), S"\n",
											S"     ", Path::GetFileName(filename));
		worker->ReportProgress(i * 100 / filenames->Count, message);

		// Add this file
		if (AddFile(filename, classification, jobLabel))
		{
			e->Result = __box(true);
		}
		else
		{
			e->Result = __box(false);
		}

		// Check for cancel?
		if (worker->CancellationPending)
		{
			// Being that we were canceled, remove all the pending post commands associated with this job
			MOG_DBPostCommandAPI::RemovePost(NULL, jobLabel, S"");
			break;
		}
	}
}

bool MOG_ControllerLibrary::AddFile(String *filename, String *classification, String *jobLabel)
{
	try
	{
		// Make sure this file exists
		FileInfo* fileInfo = new FileInfo(filename);
		if (fileInfo &&
			fileInfo->Exists)
		{
			// Check if we should create a classification on our own?
			if (classification->Length == 0)
			{
				// Assume the filename's path can dictate the classification
				classification = ConstructLibraryClassificationFromPath(Path::GetDirectoryName(filename));
			}

			// Build the blessed asset's Filename
			String *assetLabel = DosUtils::PathGetFileName(filename);
			MOG_Filename *assetFilename = MOG_Filename::CreateAssetName(classification, "All", assetLabel);
			// Figure out where the locally synced copy of this file will be
			String *localSyncedFilename = ConstructLocalFilenameFromAssetName(assetFilename);

			// Make sure we have a valid classification?
			if (classification->Length &&
				MOG_Filename::IsClassificationValidForProject(classification, MOG_ControllerProject::GetProjectName()))
			{
				// First, make sure this classification exists in the project
				if (MOG_ControllerProject::GetProject()->ClassificationAdd(classification))
				{
					String *timestamp = "";
					FileInfo *file = new FileInfo(filename);
					if (file != NULL && file->Exists)
					{
						timestamp = MOG_Time::GetVersionTimestamp(file->LastWriteTime);
					}
					MOG_Filename *repositoryName = MOG_ControllerRepository::GetAssetBlessedVersionPath(assetFilename, timestamp);

					// Generate a list of files to be imported
					ArrayList *fileList = new ArrayList();
					fileList->Add(filename);

					// Construct the property list
					ArrayList *props = new ArrayList();
					props->Add(MOG_PropertyFactory::MOG_Classification_InfoProperties::New_IsLibrary(true));

					// Determin if we can add this file
					bool bAddFile = false;
					bool bLockedByMe = false;
					// Get any lock information for this file
					MOG_Command *pLockQuery = MOG_ControllerProject::PersistentLock_Query(repositoryName->GetAssetFullName());
					if (pLockQuery->IsCompleted() &&
						pLockQuery->GetCommand())
					{
						// Looks like there was a lock found...Check if it is someone else?
						if (String::Compare(pLockQuery->GetCommand()->GetUserName(), MOG_ControllerProject::GetUser()->GetUserName(), true) == 0)
						{
							// Proceed to add the file if we are the lock owner
							bAddFile = true;
							bLockedByMe = true;
						}
					}
					else
					{
						// Always add the file if there isn't a lock
						bAddFile = true;
					}

					// Did we determine this file can be added?
					if (bAddFile)
					{
						// create the asset
						MOG_Filename *createdAssetFilename = MOG_ControllerAsset::CreateAsset(repositoryName, "", fileList, NULL, props, false, false);
						if (createdAssetFilename)
						{
							// add for posting later on
							MOG_ControllerProject::AddAssetForPosting(createdAssetFilename, jobLabel);

// JohnRen - It is impossible for them to actually have this checked out since they are just adding it and ReadOnly attribute shouldn't be determined by classification-level locks
//							// Check if this isn't locked by me?
//							if (!bLockedByMe)
//							{
								// Make sure we mark this file read only because it now needs to be checked out
								DosUtils::SetAttributes(filename, FileAttributes::ReadOnly);
//							}

							return true;
						}
					}
					else
					{
						// Inform the user this is locked
						String* message = String::Concat(	S"There is a lock by '", pLockQuery->GetCommand()->GetUserName(), S"' preventing this file from being added.\n",
															S"FILE: ", repositoryName->GetAssetFullName(), S"\n\n",
															S"The file was not added to the library.\n");
						MOG_Prompt::PromptMessage(S"Add Failed", message);
					}
				}
			}
			else
			{
				// Inform the user we had a classification discrepency
				String* message = String::Concat(S"The system has detected an improper library classification was generated for this file.\n",
												 S"FILE: ", filename, S"\n");
				MOG_Prompt::PromptMessage(S"Improper Library Classification", message);
			}
		}
		else
		{
			// Inform the user that the specified file doesn't exist
			String *message = String::Concat(	S"The specified file does not exist.\n",
												S"FILENAME: ", filename);
			MOG_Prompt::PromptMessage(S"Library File Add Failed", message);
		}
	}
	catch (Exception *e)
	{
		// Inform the user that the specified file doesn't exist
		MOG_Report::ReportMessage(S"Library File Add Failed", e->Message, e->StackTrace, MOG_ALERT_LEVEL::CRITICAL);
	}

	return false;
}


String* MOG_ControllerLibrary::EnsureClassificationIsWithinLibrary(String *classification)
{
	// Check if this classification is not within the library?
	String *libraryRootClassification = MOG_Filename::GetProjectLibraryClassificationString();
	if (!classification->StartsWith(libraryRootClassification, StringComparison::CurrentCultureIgnoreCase))
	{
		// They want a valid library classification so lets give them the root
		classification = libraryRootClassification;
	}

	return classification;
}


bool MOG_ControllerLibrary::IsPathWithinLibrary(String *path)
{
	bool bIsWithinLibrary = false;

	// Make certain this path is within our project's library path?
	String *projectLibraryPath = ConstructPathFromLibraryClassification(MOG_Filename::GetProjectLibraryClassificationString());
	if (DosUtils::PathIsWithinPath(projectLibraryPath, path))
	{
		bIsWithinLibrary = true;
	}

	return bIsWithinLibrary;
}

String *MOG_ControllerLibrary::ConstructLibraryClassificationFromPath(String *path)
{
	String *classification = S"";

	// Check if this filename is already within the library?
	if (IsPathWithinLibrary(path))
	{
		// Make our assumption about the library's classification based on the filename
		String *relativePath = path->Substring(GetWorkingDirectory()->Length)->Trim(S"\\"->ToCharArray());
		classification = relativePath->Replace("\\", "~");
		// Make sure this classification contains the Adam object or else stray classifications can be generated!
		classification = MOG_Filename::AppendAdamObjectNameOnClassification(classification);
	}

	return classification;
}

String *MOG_ControllerLibrary::ConstructPathFromLibraryClassification(String *classification)
{
	String *path = S"";

	if (MOG_Filename::IsLibraryClassification(classification))
	{
		path = Path::Combine(GetWorkingDirectory(), classification->Replace(S"~", S"\\"));
	}

	return path;
}


