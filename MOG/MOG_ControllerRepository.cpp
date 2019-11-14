//--------------------------------------------------------------------------------
//	MOG_ControllerRepository.cpp
//	
//	
//--------------------------------------------------------------------------------

#include "StdAfx.h"

#include "MOG_Define.h"
#include "MOG_Tokens.h"
#include "MOG_ControllerProject.h"
#include "MOG_ControllerSystem.h"
#include "MOG_DosUtils.h"
#include "MOG_DBAssetAPI.h"

#include "MOG_ControllerRepository.h"



bool MOG_ControllerRepository::AddAssetRevisionInfo(MOG_ControllerAsset *blessedAsset, bool bPurgeOldProperties)
{
	// Make sure we have a valid asset?
	if (blessedAsset)
	{
		// Get the Asset's Properties?
		MOG_Properties *pProperties = blessedAsset->GetProperties();
		if (pProperties)
		{
			// Add all the associated files of this asset to the database?
			MOG_DBAssetAPI::AddAssetRelatedFiles(blessedAsset);

			// Check if we should purge any old properties of this asset?
			if (bPurgeOldProperties)
			{
				// Make sure we always remove the old properties just to be safe
				MOG_DBAssetAPI::RemoveAllAssetVersionProperties(blessedAsset->GetAssetFilename(), blessedAsset->GetAssetFilename()->GetVersionTimeStamp());
			}

			// Add all the properties of this blessed asset to the database
			if (MOG_DBAssetAPI::AddAssetVersionProperties(blessedAsset->GetAssetFilename(), blessedAsset->GetAssetFilename()->GetVersionTimeStamp(), pProperties->GetNonInheritedProperties()))
			{
				// Indicate we were able to complete the process
				return true;
			}
		}
	}

	return false;
}


String *MOG_ControllerRepository::GetRepositoryPath()
{
	MOG_System* pSystem = MOG_ControllerSystem::GetSystem();
	if (pSystem)
	{
		// Check if we have an external asset repository specified?
		String *format = pSystem->GetExternalRepositoryPath();
		if (format->Length == 0)
		{
			// Use the default Repository located off of the the ProjectPath in the 'Assets' directory
			format = String::Concat(TOKEN_ProjectPath, S"\\Assets");
		}

		// Format any tokens located within the format string
		return MOG_Tokens::GetFormattedString(format);
	}

	return S"System not initialized";
}


MOG_Filename *MOG_ControllerRepository::GetAssetBlessedPath(MOG_Filename *assetFilename)
{
	// Get the base of the system's repository
	String *repositoryLocation = GetRepositoryPath();
	// Make sure to tokenize this string because it contains tokens
	String *format = String::Concat(repositoryLocation, S"\\", assetFilename->GetAssetEncodedClassification(), S"\\", assetFilename->GetAssetEncodedName());
	// Get the asset's location within the repository
	String *assetLocation = MOG_Tokens::GetFormattedString(format, assetFilename);
	// Return a MOG_Filename because this points to an asset
	return new MOG_Filename(assetLocation);
}


MOG_Filename *MOG_ControllerRepository::GetAssetBlessedVersionPath(MOG_Filename *assetFilename, String *timestamp)
{
	// Get verified time stamp
	timestamp = VerifyTimestamp(timestamp);
	// Get the base directory of this asset within the repository
	MOG_Filename *assetLocation = GetAssetBlessedPath(assetFilename);
	// Add on the revision container
	assetLocation->SetFilename(String::Concat(assetLocation->GetEncodedFilename(), S"\\R.", timestamp));
	// Return a MOG_Filename because this points to an asset
	return assetLocation;
}


ArrayList *MOG_ControllerRepository::GetBlessedRevisions(MOG_Filename *assetFilename, bool bCheckDatabase, bool bCheckDisk)
{
	ArrayList *revisions = new ArrayList();

	// Did they indicate we check the database?
	if (bCheckDatabase)
	{
		revisions = MOG_DBAssetAPI::GetAllAssetRevisions(assetFilename);
	}

	// Did they indicate we check the disk?
	if (bCheckDisk)
	{
		DirectoryInfo *dirs[] = DosUtils::DirectoryGetList(GetAssetBlessedPath(assetFilename)->GetEncodedFilename(), S"*.*");
		if (dirs != NULL)
		{
			for (int r = 0; r < dirs->Count; r++)
			{
				// Add each found revision
				MOG_Filename *revisionFilename = new MOG_Filename(dirs[r]->FullName);
				String *revision = revisionFilename->GetVersionTimeStamp();
				AddUniqueFilename(revisions, revision);
			}
		}
	}

	return revisions;
}


String *MOG_ControllerRepository::GetArchivePath()
{
	MOG_System* pSystem = MOG_ControllerSystem::GetSystem();
	if (pSystem)
	{
		// Check if we have an external asset archive specified?
		String *format = pSystem->GetExternalArchivePath();
		if (format->Length == 0)
		{
			// Use the default Archive located off of the the ProjectPath in the 'Archive' directory
			format = String::Concat(TOKEN_ProjectPath, S"\\Archive");
		}

		// Format any tokens located within the format string
		return MOG_Tokens::GetFormattedString(format);
	}

	return S"System not initialized";
}


MOG_Filename *MOG_ControllerRepository::GetAssetArchivedPath(MOG_Filename *assetFilename)
{
	// Get the base of the system's Archive
	String *archiveLocation = GetArchivePath();
	// Make sure to tokenize this string because it contains tokens
	String *format = String::Concat(archiveLocation, S"\\", assetFilename->GetAssetEncodedClassification(), S"\\", assetFilename->GetAssetEncodedName());
	// Get the asset's location within the archive
	String *assetLocation = MOG_Tokens::GetFormattedString(format, assetFilename);
	// Return a MOG_Filename because this points to an asset
	return new MOG_Filename(assetLocation);
}


MOG_Filename *MOG_ControllerRepository::GetAssetArchivedVersionPath(MOG_Filename *assetFilename, String *timestamp)
{
	// Get verified time stamp
	timestamp = VerifyTimestamp(timestamp);
	// Get the base directory of this asset within the Archive
	MOG_Filename *assetLocation = GetAssetArchivedPath(assetFilename);
	// Add on the revision container
	assetLocation->SetFilename(String::Concat(assetLocation->GetEncodedFilename(), S"\\R.", timestamp));
	// Return a MOG_Filename because this points to an asset
	return assetLocation;
}


ArrayList *MOG_ControllerRepository::GetArchivedRevisions(MOG_Filename *assetFilename, bool bCheckDatabase, bool bCheckDisk)
{
	ArrayList *revisions = new ArrayList();

//	// Did they indicate we check the database?
//	if (bCheckDatabase)
//	{
//		revisions = MOG_DBAssetAPI::GetAllAssetRevisions(assetFilename);
//	}

	// Did they indicate we check the disk?
	if (bCheckDisk)
	{
		DirectoryInfo *dirs[] = DosUtils::DirectoryGetList(GetAssetArchivedPath(assetFilename)->GetEncodedFilename(), S"*.*");
		if (dirs != NULL)
		{
			for (int r = 0; r < dirs->Count; r++)
			{
				// Add each found revision
				MOG_Filename *revisionFilename = new MOG_Filename(dirs[r]->FullName);
				String *revision = revisionFilename->GetVersionTimeStamp();
				AddUniqueFilename(revisions, revision);
			}
		}
	}

	return revisions;
}


String *MOG_ControllerRepository::VerifyTimestamp(String *timestamp)
{
	// Check for an invalid timestamp?
	if (!timestamp || !timestamp->Length)
	{
		// Construct a new timestamp
		timestamp = MOG_Time::GetVersionTimestamp();
	}

	return timestamp;
}


bool MOG_ControllerRepository::AddUniqueFilename(ArrayList *array, String *revision)
{
	bool bFound = false;

	// Scan the array looking for duplicates?
	for (int f = 0; f < array->Count; f++)
	{
		String *tempFilename = __try_cast<String *>(array->Item[f]);
		if (String::Compare(tempFilename, revision, true) == 0)
		{
			bFound = true;
		}
	}

	// Make sure we never found a match?
	if (!bFound)
	{
		// Add this filename to the array
		array->Add(revision);
		return true;
	}

	return false;
}



