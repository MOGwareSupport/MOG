//--------------------------------------------------------------------------------
//	MOG_FileHamsterIntegration.cpp
//	
//	
//--------------------------------------------------------------------------------

#include "StdAfx.h"

#include "MOG_DosUtils.h"

#include "MOG_FileHamsterIntegration.h"



bool MOG_FileHamsterIntegration::Initialize()
{
	// Check if we still need to initialize?
	if (mGetLatestFolders == NULL)
	{
		// Check if we have FileHamster running
		if (Process::GetProcessesByName("FileHamster")->Count != 0 || Process::GetProcessesByName("FileHamster.vshost")->Count != 0)
		{
			// Initialize mGetLatestFolders indicating we want to track the folders
			mGetLatestFolders = new HybridDictionary();
		}
	}

	return (mGetLatestFolders != NULL);
}


void MOG_FileHamsterIntegration::FileHamsterPause(String *fullFilename)
{
	// Make sure we are tracking the GetLatest folders?
	if (mGetLatestFolders != NULL)
	{
		String *directory = DosUtils::PathGetDirectoryPath(fullFilename);
		if (directory->Length)
		{
			// Check if this is a new directory that we don't know about?
			if (mGetLatestFolders->Contains(directory) == false)
			{
				String *ignoreFilename = Path::Combine(directory, "FH.Ignore");

				// Add this directory so we will know we have already been here
				mGetLatestFolders->Add(directory, ignoreFilename);

				if (DosUtils::DirectoryExistFast(directory) == false)
				{
					DosUtils::DirectoryCreate(directory);
				}

				// Create the FileHamster pause command
				DosUtils::AppendTextToFile(ignoreFilename, "");
			}
		}
	}
}


void MOG_FileHamsterIntegration::FileHamsterUnPause()
{
	// Check if we were tracking the folders?
	if (mGetLatestFolders != NULL)
	{
		// Enumerate through the GetLatest folders
		IDictionaryEnumerator *enumerator = mGetLatestFolders->GetEnumerator();
		while(enumerator->MoveNext())
		{
			String *ignoreFilename = __try_cast<String*>(enumerator->Value);

			if (DosUtils::FileExistFast(ignoreFilename))
			{
				// Looks like FileHamster never caught this command file we left behind
				// Let's go ahead and delete our old file so we don't pollute the directories
				DosUtils::DeleteFast(ignoreFilename);
			}
		}

		// Reset us back to NULL (requiring MOG_FileHamsterIntegration::Initialize() to recalled again)
		mGetLatestFolders = NULL;
	}
}

