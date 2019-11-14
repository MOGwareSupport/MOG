//--------------------------------------------------------------------------------
//	MOG_DBSyncedDataAPI.cpp
//	
//	
//--------------------------------------------------------------------------------
#include "stdafx.h"

#include "MOG_ControllerProject.h"
#include "MOG_ControllerSystem.h"
#include "MOG_DBAssetAPI.h"
#include "MOG_DBInboxAPI.h"

#include "MOG_DBSyncedDataAPI.h"
#include "MOG_DBQueryBuilderAPI.h"
#include "MOG_StringUtils.h"



//initilizer that just makes a default object but does not populate.
MOG_LocalSyncInfo::MOG_LocalSyncInfo(MOG_DBSyncedLocationInfo*  syncedLocation)
{
	InitilizeLocalSyncInfo(syncedLocation->mComputerName, syncedLocation->mProjectName, syncedLocation->mPlatformName, syncedLocation->mWorkingDirectory, syncedLocation->mBranchName, syncedLocation->mUserName, S"", S"");
	PopulateLocalSyncInfo();
}


MOG_LocalSyncInfo::MOG_LocalSyncInfo(String* computerName, String* projectName, String* platformName, String* workingDirectory, String* branchName, String* userName)
{
	InitilizeLocalSyncInfo(computerName, projectName, platformName, workingDirectory, branchName, userName, S"", S"");
	PopulateLocalSyncInfo();
}


MOG_LocalSyncInfo::MOG_LocalSyncInfo(String* computerName, String* projectName, String* platformName, String* workingDirectory, String* branchName, String* userName, String* classification, String* syncLocation)
{
	InitilizeLocalSyncInfo(computerName, projectName, platformName, workingDirectory, branchName, userName, classification, syncLocation);
	PopulateLocalSyncInfo();
}


bool MOG_LocalSyncInfo::PopulateLocalSyncInfo()
{
	// Precache our lists
	MOG_ControllerSystem::GetDB()->GetDBCache()->GetAssetNameCache()->PopulateCacheFromSQL(MOG_DBQueryBuilderAPI::MOG_DBCacheQueries::PopulateCache_AssetNameCache(),S"CACHEID", S"CACHENAME");
	MOG_ControllerSystem::GetDB()->GetDBCache()->GetAssetVersionCache()->PopulateCacheFromSQL(MOG_DBQueryBuilderAPI::MOG_DBCacheQueries::PopulateCache_AssetVersionCache(), S"CACHEID", S"CACHENAME", S"CACHELIST");
	MOG_ControllerSystem::GetDB()->GetDBCache()->GetClassificationCache()->PopulateCacheFromSQL(MOG_DBQueryBuilderAPI::MOG_DBCacheQueries::PopulateCache_ClassificationCache(),S"CACHEID", S"CACHENAME");

	//Get the remove list to process on.
	if (mClassificationFilter && mClassificationFilter->Length)
	{
		mPreviouslySyncedAssetList = ConvertFromArrayListToHybridDictionary(MOG_DBSyncedDataAPI::GetCurrentSyncedAssetsViaClassification(mSyncedLocationInfo->mComputerName, mSyncedLocationInfo->mProjectName, mSyncedLocationInfo->mPlatformName, mSyncedLocationInfo->mWorkingDirectory,mSyncedLocationInfo->mUserName, mClassificationFilter));
	}
	else if(mSyncLocationFilter && mSyncLocationFilter->Length)
	{
		mPreviouslySyncedAssetList = ConvertFromArrayListToHybridDictionary(MOG_DBSyncedDataAPI::GetCurrentSyncedAssetsViaSyncLocation(mSyncedLocationInfo->mComputerName, mSyncedLocationInfo->mProjectName, mSyncedLocationInfo->mPlatformName, mSyncedLocationInfo->mWorkingDirectory,mSyncedLocationInfo->mUserName, mSyncLocationFilter));
	}
	else
	{
		mPreviouslySyncedAssetList = ConvertFromArrayListToHybridDictionary(MOG_DBSyncedDataAPI::GetCurrentSyncedAssets(mSyncedLocationInfo->mComputerName, mSyncedLocationInfo->mProjectName, mSyncedLocationInfo->mPlatformName, mSyncedLocationInfo->mWorkingDirectory, mSyncedLocationInfo->mUserName));
	}

	//Get the Current Assets to sync to.
	// Check if there was a specific sync filter was specified?
	if (mSyncLocationFilter && mSyncLocationFilter->Length)
	{
		mPotentialAssetsToBeSyncedList = ConvertFromArrayListToHybridDictionary(MOG_DBAssetAPI::GetAllCurrentAssetsBySyncLocation(mSyncLocationFilter, mSyncedLocationInfo->mPlatformName, true, mSyncedLocationInfo->mBranchName));
	}
	// Check if we want all of the assets in the project?
	else if (mClassificationFilter->Length == 0 ||
			 String::Compare(mClassificationFilter, MOG_ControllerProject::GetProjectName(), true) == 0)
	{
		mPotentialAssetsToBeSyncedList = ConvertFromArrayListToHybridDictionary(MOG_DBAssetAPI::GetAllAssets(true, mSyncedLocationInfo->mBranchName));
	}
	else if (mClassificationFilter && mClassificationFilter->Length)
	{
		mPotentialAssetsToBeSyncedList = ConvertFromArrayListToHybridDictionary(MOG_DBAssetAPI::GetAllCurrentAssetsByClassification(mClassificationFilter, true, mSyncedLocationInfo->mBranchName));
	}

	// Iterate through the mPotentialAssetsToBeSyncedList looking for any irrelevant assets
	mIrrelevantAssets = new HybridDictionary();
	IDictionaryEnumerator* myEnumerator = mPotentialAssetsToBeSyncedList->GetEnumerator();
	while ( myEnumerator->MoveNext() )
	{
		MOG_Filename* potentialAssetToBeSynced = __try_cast <MOG_Filename*>(myEnumerator->Value);

		// Check if this is a platform specific asset?
		if (potentialAssetToBeSynced->IsPlatformSpecific())
		{
			// Make sure this asset matches our platform?
			if (String::Compare(potentialAssetToBeSynced->GetAssetPlatform(), this->mSyncedLocationInfo->mPlatformName, true) == 0)
			{
				// Check if a non platform specific asset exists
				MOG_Filename* testAssetFilename = MOG_Filename::CreateAssetName(potentialAssetToBeSynced->GetAssetClassification(), S"All", potentialAssetToBeSynced->GetAssetLabel());
				MOG_Filename* genericAssetFilename = __try_cast <MOG_Filename*>(this->mPotentialAssetsToBeSyncedList->Item[testAssetFilename->GetAssetFullName()]);
				if (genericAssetFilename)
				{
					// Add the non platform specific asset as an irrelavant asset
					mIrrelevantAssets->Item[genericAssetFilename->GetAssetFullName()] = genericAssetFilename;
				}
			}
			else
			{
				// Add the non matching platform specific asset as an irrelavant asset
				mIrrelevantAssets->Item[potentialAssetToBeSynced->GetAssetFullName()] = potentialAssetToBeSynced;
			}
		}
	}
	// Remove the mIrrelevantAssets
	myEnumerator = mIrrelevantAssets->GetEnumerator();
	while ( myEnumerator->MoveNext() )
	{
		MOG_Filename* irrelevantAsset = __try_cast <MOG_Filename*>(myEnumerator->Value);
		mPotentialAssetsToBeSyncedList->Remove(irrelevantAsset->GetAssetFullName());
	}

	// Iterate through the mPreviouslySyncedAssetList looking for what can be removed
	myEnumerator = mPreviouslySyncedAssetList->GetEnumerator();
	while ( myEnumerator->MoveNext() )
	{
		MOG_Filename* previouslySyncedAsset = __try_cast <MOG_Filename*>(myEnumerator->Value);

		// Check if this asset also exists in the mPotentialAssetsToBeSyncedList
		if (!mPotentialAssetsToBeSyncedList->Contains(previouslySyncedAsset->GetAssetFullName()))
		{
			// Add previouslySyncedAsset to the list of assets being removed
			mRemoveList->Item[previouslySyncedAsset->GetAssetFullName()] = previouslySyncedAsset;
		}
	}

	// Iterate through the mPotentialAssetsToBeSyncedList looking for what needs to be synced
	myEnumerator = mPotentialAssetsToBeSyncedList->GetEnumerator();
	while ( myEnumerator->MoveNext() )
	{
		MOG_Filename* potentialAssetToBeSynced = __try_cast <MOG_Filename*>(myEnumerator->Value);

		// Check if we have previously synced this asset?
		MOG_Filename* previouslySyncedAsset = __try_cast <MOG_Filename*>(mPreviouslySyncedAssetList->Item[potentialAssetToBeSynced->GetAssetFullName()]);
		if (previouslySyncedAsset)
		{
			// Check if the versions are different
			if (String::Compare(previouslySyncedAsset->GetVersionTimeStamp(), potentialAssetToBeSynced->GetVersionTimeStamp(), true) != 0)
			{
				// Create a new MOG_DBUpdatePair
				MOG_DBUpdatePair*  aNewUpdatePair = new MOG_DBUpdatePair();
				aNewUpdatePair->mNewVersion = potentialAssetToBeSynced;
				aNewUpdatePair->mOldVersion = previouslySyncedAsset;
				// Add asset to the list of assets being updated
				mUpdateList->Item[previouslySyncedAsset->GetAssetFullName()] = aNewUpdatePair;
			}
		}
		else
		{
			// Add previouslySyncedAsset to the list of assets being added
			mAddList->Item[potentialAssetToBeSynced->GetAssetFullName()] = potentialAssetToBeSynced;
		}
	}

	// Obtain the list of assets in the local updated tray
	String* filter = String::Concat(mSyncedLocationInfo->mWorkingDirectory, S"\\*");
	ArrayList *localAssets = MOG_DBInboxAPI::InboxGetAssetList("Local", MOG_ControllerProject::GetUserName_DefaultAdmin(), S"", filter);
	mLocalUpdateTrayList = ConvertFromArrayListToHybridDictionary(localAssets);

	// Wait to initialize this until someone asks for it
	mNonSyncingAssets = NULL;

	return true;
}


bool MOG_LocalSyncInfo::IsNonSyncingAsset(MOG_Filename* assetFilename)
{
	// This takes really really really long to initialize so we are going to wait to do this until someone asks for it.
	if (mNonSyncingAssets == NULL)
	{
		// Obtain all assets with the 'SyncFile=False' property
		ArrayList* noSyncProperties = new ArrayList;
		MOG_Property* noSyncProperty = MOG_PropertyFactory::MOG_Sync_OptionsProperties::New_SyncFiles(false);
		noSyncProperties->Add(noSyncProperty);
		ArrayList* nonSyncingAssets = MOG_DBAssetAPI::GetAllAssetsByClassificationAndProperties("", noSyncProperties, true);
		// Convert the ArrayList
		mNonSyncingAssets = ConvertFromArrayListToHybridDictionary(nonSyncingAssets);
	}

	// Make sure we have a valid assetFIlename?
	if (assetFilename)
	{
		// Scan mNonSyncingAssets for matching assets
		if (mNonSyncingAssets->Contains(assetFilename->GetAssetFullName()))
		{
			return true;
		}

//		// Scan mLibraryAssets for matching assets
//		if (mLibraryAssets->Contains(assetFilename->GetAssetFullName()))
//		{
//			return true;
//		}
	}

	return false;
}


bool MOG_LocalSyncInfo::IsAssetIncluded(MOG_Filename* assetFilename, String* exclusionList, String* inclusionList)
{
	return !StringUtils::IsFiltered(assetFilename->GetFullFilename(), exclusionList, inclusionList);
}


bool MOG_LocalSyncInfo::IsAssetInLocalUpdatedTray(MOG_Filename* assetFilename)
{
	// Scan mLocalUpdateTrayList for matching assets
	if (mLocalUpdateTrayList->Contains(assetFilename->GetAssetFullName()))
	{
		return true;
	}

	return false;
}


MOG_Filename* MOG_LocalSyncInfo::GetAssetInLocalUpdatedTray(MOG_Filename* assetFilename)
{
	// Scan mLocalUpdateTrayList for matching assets
	if (mLocalUpdateTrayList->Contains(assetFilename->GetAssetFullName()))
	{
		return dynamic_cast<MOG_Filename*>(mLocalUpdateTrayList->Item[assetFilename->GetAssetFullName()]);
	}

	return NULL;
}


void MOG_LocalSyncInfo::InitilizeLocalSyncInfo(String* computerName, String* projectName, String* platformName, String* workingDirectory, String* branchName, String* userName, String* classificationFilter, String* syncLocationFilter)
{
	try
	{
		mSyncedLocationInfo = new MOG_DBSyncedLocationInfo();
		mSyncedLocationInfo->mComputerName = computerName;
		mSyncedLocationInfo->mProjectName = projectName;
		mSyncedLocationInfo->mPlatformName = platformName;
		mSyncedLocationInfo->mWorkingDirectory = workingDirectory;
		mSyncedLocationInfo->mBranchName = branchName;
		mSyncedLocationInfo->mUserName = userName;

		mClassificationFilter = classificationFilter;
		mSyncLocationFilter = syncLocationFilter;

		mAddList = new HybridDictionary(true);
		mRemoveList = new HybridDictionary(true);
		mUpdateList = new HybridDictionary(true);
	}
	catch(Exception* e)
	{
		if(e)
		{
			String* errorStack = e->StackTrace;
		}
	}
}


bool MOG_DBSyncedDataAPI::AddSyncedLocation(String* computerName, String* projectName, String* platformName, String* workingDirectory, String* branchName, String* tabText, String* userName)
{
	bool bAdded = false;

	// Make sure we have a valid directory?
	if (workingDirectory->Length)
	{
		if( branchName == NULL || branchName->Length == 0 )
		{
			branchName = MOG_ControllerProject::GetBranchName();
		}

		int syncedLocationID = GetSyncedLocationID(computerName, projectName, platformName, workingDirectory, userName);
		if( syncedLocationID == 0 )
		{
			int platformID = MOG_DBProjectAPI::GetPlatformNameID(platformName);
			int branchID = MOG_DBProjectAPI::GetBranchIDByName(branchName);
			int userID = MOG_DBProjectAPI::GetUserID(userName);

			if( platformID != 0 && branchID != 0 )
			{
				String* insertCmd = String::Concat(	S"INSERT INTO ", MOG_ControllerSystem::GetDB()->GetSyncedDataLocationsTable(),
													S" ( ComputerName, ProjectName, PlatformID, WorkingDirectory, BranchID, TabText, UserID ) VALUES ",
													S" ('", MOG_DBAPI::FixSQLParameterString(computerName), S"', '", MOG_DBAPI::FixSQLParameterString(projectName), S"', ",__box(platformID), S", '", MOG_DBAPI::FixSQLParameterString(workingDirectory) , S"', ", __box(branchID), S", '", MOG_DBAPI::TruncateIfNecessary(MOG_DBAPI::FixSQLParameterString(tabText), 265, false) , S"', ", __box(userID) , S")" );

				return MOG_DBAPI::RunNonQuery(insertCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
			}
		}
	}

	return bAdded;
}

bool MOG_DBSyncedDataAPI::RemoveSyncedLocation(String* computerName, String* projectName, String* platformName, String* workingDirectory, String* userName)
{
	bool success = false;

	int syncedLocationID = GetSyncedLocationID(computerName, projectName, platformName, workingDirectory, userName);
	if( syncedLocationID == 0 )
	{
		return success;
	}
	ArrayList* transactionList = new ArrayList();

	transactionList->Add(String::Concat(S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetSyncedDataLocationsTable(),
											S" WHERE (ID=", Convert::ToString(syncedLocationID), S")" ));

	// remove all links associated with location ID
	transactionList->Add(String::Concat(S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetSyncedDataLinksTable(),
											S" WHERE (SyncedDataLocationID=", Convert::ToString(syncedLocationID), S")" ));

	success = MOG_DBAPI::ExecuteTransaction(transactionList);
	if(success)
	{
		MOG_ControllerSystem::GetDB()->GetDBCache()->GetSyncedDataLocationsCache()->RemoveSetFromCacheByID(syncedLocationID);
	}
	return success;
}

MOG_DBSyncedLocationInfo* MOG_DBSyncedDataAPI::GetSyncedLocationByID(int workspaceID)
{
	String* selectString = String::Concat(
		S" SELECT SDL.ID, SDL.ComputerName, SDL.ProjectName, SDL.WorkingDirectory, Platforms.PlatformName, Branches.BranchName, SDL.TabText, Users.UserName ",
		S" FROM ", MOG_ControllerSystem::GetDB()->GetSyncedDataLocationsTable(), S" SDL INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetPlatformsTable(), S" Platforms ON SDL.PlatformID = Platforms.ID INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches ON SDL.BranchID = Branches.ID INNER JOIN ", MOG_ControllerSystem::GetDB()->GetUsersTable(), S" Users ON Users.ID = SDL.UserID ",
		S" WHERE (SDL.ID = ", workspaceID.ToString(), S") " );

	return QuerySyncedLocation(selectString);
}

ArrayList* MOG_DBSyncedDataAPI::GetAllSyncedLocations(String* computerName, String* workingDirectory, String* platformName, String* userName)
{
	String* selectString = String::Concat(
		S"SELECT SDL.ID, SDL.ComputerName, SDL.ProjectName, SDL.WorkingDirectory, Platforms.PlatformName, Branches.BranchName, SDL.TabText, Users.UserName ",
		S"FROM ", MOG_ControllerSystem::GetDB()->GetSyncedDataLocationsTable(), S" SDL INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetPlatformsTable(), S" Platforms ON SDL.PlatformID = Platforms.ID INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches ON SDL.BranchID = Branches.ID INNER JOIN ", MOG_ControllerSystem::GetDB()->GetUsersTable(), S" Users ON Users.ID = SDL.UserID "  );

	// Build the Where part of the select statment
	String *whereClause = "";
	// Check to see if you add the ComputerName part of the select WHERE statement.
	if (computerName != NULL && computerName->Length)
	{
		if (whereClause->Length)
		{
			whereClause = String::Concat(whereClause, S" AND " );
		}
		whereClause = String::Concat(whereClause, S"(SDL.ComputerName = '", MOG_DBAPI::FixSQLParameterString(computerName), S"')" );
	}
	if (workingDirectory != NULL && workingDirectory->Length)
	{
		if (whereClause->Length)
		{
			whereClause = String::Concat(whereClause, S" AND " );
		}
		whereClause = String::Concat(whereClause, S"(SDL.WorkingDirectory = '", MOG_DBAPI::FixSQLParameterString(workingDirectory), S"')" );
	}
	if (platformName != NULL && platformName->Length)
	{
		if (whereClause->Length)
		{
			whereClause = String::Concat(whereClause, S" AND " );
		}
		int platformID = MOG_DBAssetAPI::GetPlatformID(platformName);
		whereClause = String::Concat(whereClause, S" (SDL.PlatformID = ", platformID.ToString(), S")");
	}
	if (userName != NULL && userName->Length)
	{
		if (whereClause->Length)
		{
			whereClause = String::Concat(whereClause, S" AND " );
		}
		whereClause = String::Concat(whereClause, S" (Users.UserName = '", MOG_DBAPI::FixSQLParameterString(userName), S"')");
	}
	if (whereClause->Length)
	{
		selectString = String::Concat(selectString, S" WHERE ", whereClause);
	}

	return QuerySyncedLocations(selectString);
}

bool MOG_DBSyncedDataAPI::DoesSyncedLocationExist(String* computerName, String* projectName, String* platformName, String* workingDirectory, String* userName)
{
	String* selectString = String::Concat(
		S"SELECT SDL.ID, SDL.ComputerName, SDL.ProjectName, SDL.WorkingDirectory, Platforms.PlatformName, Branches.BranchName, Users.UserName, SDL.TabText ",
		S"FROM ", MOG_ControllerSystem::GetDB()->GetSyncedDataLocationsTable(), S" SDL INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetPlatformsTable(), S" Platforms ON SDL.PlatformID = Platforms.ID INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches ON SDL.BranchID = Branches.ID INNER JOIN ", MOG_ControllerSystem::GetDB()->GetUsersTable(), S" Users ON Users.ID = SDL.UserID ");

	bool haveComputerName = (computerName != NULL && computerName->Length > 0);
	bool haveProjectName = (projectName != NULL && projectName->Length > 0);
	bool havePlatformName = (platformName != NULL && platformName->Length > 0);
	bool haveWorkingDirectory = (workingDirectory != NULL && workingDirectory->Length > 0);
	bool haveUserName = (userName != NULL && userName->Length > 0);

	if( haveComputerName || haveProjectName || havePlatformName || haveWorkingDirectory || haveUserName)
	{
		selectString = String::Concat( selectString, S"WHERE " );

		if( haveComputerName )
		{
			selectString = String::Concat( selectString, S"(SDL.ComputerName = '", MOG_DBAPI::FixSQLParameterString(computerName), S"')" );
		
			if( haveProjectName || havePlatformName || haveWorkingDirectory || haveUserName )
			{
				selectString = String::Concat( selectString, S" AND " );
			}
		}

		if( haveProjectName )
		{
			selectString = String::Concat( selectString, S"(SDL.ProjectName = '", MOG_DBAPI::FixSQLParameterString(projectName), S"')" );
		
			if( havePlatformName || haveWorkingDirectory || haveUserName )
			{
				selectString = String::Concat( selectString, S" AND " );
			}
		}

		if( havePlatformName )
		{
			selectString = String::Concat( selectString, S"(Platforms.PlatformName = '", MOG_DBAPI::FixSQLParameterString(platformName), S"')" );
		
			if( haveWorkingDirectory || haveUserName )
			{
				selectString = String::Concat( selectString, S" AND " );
			}
		}

		if( haveWorkingDirectory )
		{
			selectString = String::Concat( selectString, S"(SDL.WorkingDirectory = '", MOG_DBAPI::FixSQLParameterString(workingDirectory), S"')" );

			if( haveUserName )
			{
				selectString = String::Concat( selectString, S" AND " );
			}
		}

		if( haveUserName )
		{
			selectString = String::Concat( selectString, S"(Users.UserName = '", MOG_DBAPI::FixSQLParameterString(userName), S"')" );
		}
	}

	return (QuerySyncedLocations(selectString)->Count > 0);
}

bool MOG_DBSyncedDataAPI::AddSyncedLocationLink(MOG_Filename* blessedAssetFilename, String* computerName, String* projectName, String* platformName, String* workingDirectory, String* userName)
{
	int syncedLocationID = GetSyncedLocationID(computerName, projectName, platformName, workingDirectory, userName);
	int assetNameID = MOG_DBAssetAPI::GetAssetNameID(blessedAssetFilename);
	int assetVersionID = MOG_DBAssetAPI::GetAssetVersionID(blessedAssetFilename, NULL);

	String* insUpdCmd;

	if( assetNameID == 0 )
	{
		// error invalid asset name
		return false;
	}

	if( assetVersionID == 0 )
	{
		// error invalid asset version
		return false;
	}

	if( syncedLocationID == 0 )
	{
		// error invalid sync location
		return false;
	}
	String* selectCmd = String::Concat(
		S"SELECT SyncedDataLinks.AssetVersionID ",
		S"FROM ", MOG_ControllerSystem::GetDB()->GetSyncedDataLinksTable(), S" SyncedDataLinks INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions ON SyncedDataLinks.AssetVersionID = AssetVersions.ID ",
		S"WHERE (SyncedDataLinks.SyncedDataLocationID = ", Convert::ToString(syncedLocationID), S") AND ",
			S"(AssetVersions.AssetNameID = ", Convert::ToString(assetNameID), S")" );

	int existingAssetVersionID = MOG_DBAPI::QueryInt(selectCmd, S"AssetVersionID");

	if( existingAssetVersionID == 0 )
	{
		insUpdCmd = String::Concat(S"INSERT INTO ", MOG_ControllerSystem::GetDB()->GetSyncedDataLinksTable(),
								S" ( AssetVersionID, SyncedDataLocationID ) VALUES ",
								S" (", __box(assetVersionID), S",", __box(syncedLocationID), S" )" );
	}
	else
	{
		insUpdCmd = String::Concat(
			S"UPDATE ", MOG_ControllerSystem::GetDB()->GetSyncedDataLinksTable(),
			S" SET AssetVersionID = ", Convert::ToString(assetVersionID),
			S" WHERE (AssetVersionID = ", Convert::ToString(existingAssetVersionID), S") AND ",
				S"(SyncedDataLocationID = ", Convert::ToString(syncedLocationID), S")" );
	}

	return MOG_DBAPI::RunNonQuery(insUpdCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
	
}

bool MOG_DBSyncedDataAPI::RemoveSyncedLocationLink(MOG_Filename* blessedAssetFilename, String* computerName, String* projectName, String* platformName, String* workingDirectory, String* userName)
{
	int syncedLocationID = GetSyncedLocationID(computerName, projectName, platformName, workingDirectory, userName);
	int assetNameID = MOG_DBAssetAPI::GetAssetNameID(blessedAssetFilename);

	if( assetNameID == 0 )
	{
		// error invalid asset name
		return false;
	}

	if( syncedLocationID == 0 )
	{
		// error invalid sync location
		return false;
	}
	String* selectCmd = String::Concat(
		S"SELECT SyncedDataLinks.AssetVersionID ",
		S"FROM ", MOG_ControllerSystem::GetDB()->GetSyncedDataLinksTable(), S" SyncedDataLinks INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions ON SyncedDataLinks.AssetVersionID = AssetVersions.ID ",
		S"WHERE (SyncedDataLinks.SyncedDataLocationID = ", Convert::ToString(syncedLocationID), S") AND ",
			S"(AssetVersions.AssetNameID = ", Convert::ToString(assetNameID), S")" );

	int existingAssetVersionID = MOG_DBAPI::QueryInt(selectCmd, S"AssetVersionID");

	String* deleteCmd = String::Concat(
		S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetSyncedDataLinksTable(), S" ",
		S"WHERE (SyncedDataLocationID = ", Convert::ToString(syncedLocationID), S") AND ",
			S"(AssetVersionID = ", Convert::ToString(existingAssetVersionID), S")" );

	return MOG_DBAPI::RunNonQuery(deleteCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
}

ArrayList* MOG_DBSyncedDataAPI::GetCurrentSyncedAssets(String* computerName, String* projectName, String* platformName, String* workingDirectory, String* userName)
{
	ArrayList* assets = new ArrayList();
	int syncedLocationID = GetSyncedLocationID(computerName, projectName, platformName, workingDirectory, userName);

	if( syncedLocationID == 0 )
	{
		return assets;
	}

	//use the query builder to create the query string for this function.
	String* selectString = MOG_DBQueryBuilderAPI::MOG_SyncDataQueries::Query_AllCurrentAssetsInMachineUserWorkspace(computerName, projectName, platformName, workingDirectory, userName, syncedLocationID);
	assets = MOG_DBAssetAPI::QueryAssets(selectString);
	return assets;
}

ArrayList* MOG_DBSyncedDataAPI::GetCurrentSyncedAssetsViaClassification(String* computerName, String* projectName, String* platformName, String* workingDirectory, String* userName, String* classification)
{
	ArrayList* assets = new ArrayList();
	int syncedLocationID = GetSyncedLocationID(computerName, projectName, platformName, workingDirectory, userName);

	if( syncedLocationID == 0 )
	{
		return assets;
	}

	//use the query builder to create the query string for this function.
	String* selectString = MOG_DBQueryBuilderAPI::MOG_SyncDataQueries::Query_AllCurrentAssetsInMachineUserWorkspace_ByClassification(computerName, projectName, platformName, workingDirectory, userName, classification, syncedLocationID);

	assets = MOG_DBAssetAPI::QueryAssets(selectString);
	return assets;
}

ArrayList* MOG_DBSyncedDataAPI::GetCurrentSyncedAssetsViaSyncLocation(String* computerName, String* projectName, String* platformName, String* workingDirectory, String* userName, String* syncLocation)
{
	ArrayList* assets = new ArrayList();
	int syncedLocationID = GetSyncedLocationID(computerName, projectName, platformName, workingDirectory, userName);

	if( syncedLocationID == 0 )
	{
		return assets;
	}

	//use the query builder to create the query string for this function.
	String* selectString = MOG_DBQueryBuilderAPI::MOG_SyncDataQueries::Query_AllCurrentAssetsInMachineUserWorkspace_BySyncLocation(computerName, projectName, platformName, workingDirectory, userName, syncLocation, syncedLocationID);

	assets = MOG_DBAssetAPI::QueryAssets(selectString);
	return assets;
}

String* MOG_DBSyncedDataAPI::GetLocalAssetVersion(MOG_Filename* assetName, String *workspaceDirectory, String *workspacePlatformName)
{
	return GetLocalAssetVersion(MOG_ControllerSystem::GetComputerName(), 
								MOG_ControllerProject::GetProjectName(), 
								workspacePlatformName, 
								workspaceDirectory, 
								MOG_ControllerProject::GetUserName(), 
								assetName);
}

String* MOG_DBSyncedDataAPI::GetLocalAssetVersion(String* computerName, String* projectName, String* platformName, String* workingDirectory, String* userName, MOG_Filename* assetName)
{
	int syncedLocationID = GetSyncedLocationID(computerName, projectName, platformName, workingDirectory, userName);

	return MOG_DBSyncedDataAPI::GetLocalAssetVersion(syncedLocationID, assetName);
}

String* MOG_DBSyncedDataAPI::GetLocalAssetVersion(int syncedLocationID, MOG_Filename* assetName)
{
	if( syncedLocationID == 0 || assetName == NULL )
	{
		return S"";
	}

	String* selectString = String::Concat(
		S"SELECT AssetVersions.Version ",
		S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetSyncedDataLinksTable(), S" SyncedDataLinks INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions ON SyncedDataLinks.AssetVersionID = AssetVersions.ID ON ",
				S"AssetNames.ID = AssetVersions.AssetNameID INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID ",
		S"WHERE (SyncedDataLinks.SyncedDataLocationID = ", Convert::ToString(syncedLocationID), S") AND ",
			S"(AssetClassifications.FullTreeName = '", MOG_DBAPI::FixSQLParameterString(assetName->GetAssetClassification()), S"') AND ",
			S"(AssetNames.AssetPlatformKey = '", MOG_DBAPI::FixSQLParameterString(assetName->GetAssetPlatform()), S"') AND ",
			S"(AssetNames.AssetLabel = '", MOG_DBAPI::FixSQLParameterString(assetName->GetAssetLabel()), S"')" );

	return MOG_DBAPI::QueryString(selectString, S"Version");
}

//JD This function returns the SQL ID value
int MOG_DBSyncedDataAPI::GetSyncedLocationID(String* workspaceDirectory)
{
	return GetSyncedLocationID(	MOG_ControllerSystem::GetComputerName(), 
								MOG_ControllerProject::GetProjectName(), 
								"", 
								workspaceDirectory, 
								"");
}
int MOG_DBSyncedDataAPI::GetSyncedLocationID(String* computerName, String* projectName, String* platformName, String* workingDirectory, String* userName)
{
	String* workspaceCacheKey = String::Concat(computerName, S"#", projectName, S"#", platformName, S"#", workingDirectory, S"#", userName);
	int retSyncLocationID = MOG_ControllerSystem::GetDB()->GetDBCache()->GetSyncedDataLocationsCache()->GetIDFromName(workspaceCacheKey);
	if(retSyncLocationID == 0)
	{
		String* selectCmd = String::Concat(
			S"SELECT SyncedDataLocations.ID ",
			S"  FROM ", MOG_ControllerSystem::GetDB()->GetPlatformsTable(), S" Platforms INNER JOIN ",
				MOG_ControllerSystem::GetDB()->GetSyncedDataLocationsTable(), S" SyncedDataLocations ON Platforms.ID = SyncedDataLocations.PlatformID INNER JOIN ", MOG_ControllerSystem::GetDB()->GetUsersTable() , S" Users ON Users.ID = SyncedDataLocations.UserID ",
			S"  WHERE (SyncedDataLocations.ComputerName = '", MOG_DBAPI::FixSQLParameterString(computerName), S"') AND ",
					S"(SyncedDataLocations.ProjectName = '", MOG_DBAPI::FixSQLParameterString(projectName), S"') AND ",
					S"(SyncedDataLocations.WorkingDirectory = '", MOG_DBAPI::FixSQLParameterString(workingDirectory), S"') ");
		// Check if a specific platform was specified?
		if (userName->Length)
		{
			selectCmd = String::Concat(selectCmd, S" AND (Users.UserName = '", MOG_DBAPI::FixSQLParameterString(userName) , S"')" );
		}
		// Check if a specific platform was specified?
		if (platformName->Length)
		{
			selectCmd = String::Concat(selectCmd, S" AND (Platforms.PlatformName = '", MOG_DBAPI::FixSQLParameterString(platformName), S"') ");
		}
		
		retSyncLocationID = MOG_DBAPI::QueryInt(selectCmd, S"ID");
		if(retSyncLocationID > 0)
		{
			MOG_ControllerSystem::GetDB()->GetDBCache()->GetSyncedDataLocationsCache()->AddSetToCache(retSyncLocationID, workspaceCacheKey);
		}
	}
	return retSyncLocationID;
}

MOG_DBSyncedLocationInfo* MOG_DBSyncedDataAPI::SQLReadSyncedLocation(SqlDataReader* myReader)
{
	MOG_DBSyncedLocationInfo* location = new MOG_DBSyncedLocationInfo();
	
	location->mID = MOG_DBAPI::SafeIntRead(myReader,S"ID");
	location->mComputerName = MOG_DBAPI::SafeStringRead(myReader,S"ComputerName")->Trim();
	location->mProjectName = MOG_DBAPI::SafeStringRead(myReader,S"ProjectName")->Trim();
	location->mPlatformName = MOG_DBAPI::SafeStringRead(myReader,S"PlatformName")->Trim();
	location->mWorkingDirectory = MOG_DBAPI::SafeStringRead(myReader,S"WorkingDirectory")->Trim();
	location->mUserName = MOG_DBAPI::SafeStringRead(myReader,S"UserName")->Trim();
	location->mBranchName = MOG_DBAPI::SafeStringRead(myReader,S"BranchName")->Trim();
	location->mTabName = MOG_DBAPI::SafeStringRead(myReader, S"TabText")->Trim();

	return location;
}

//JD This is the function used to actually query the SyncedLocations using the passed query string.
MOG_DBSyncedLocationInfo* MOG_DBSyncedDataAPI::QuerySyncedLocation(String* selectString)
{
	SqlConnection* myConnection = MOG_DBAPI::GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());
	MOG_DBSyncedLocationInfo* location = NULL;

	SqlDataReader* myReader = NULL;
	try
	{
		myReader = MOG_DBAPI::GetNewDataReader(selectString, myConnection);
		myReader->Read();
		location = SQLReadSyncedLocation(myReader);
	}
	catch(Exception* e)
	{
		String* message = String::Concat(	S"Could not get records from SQL database!\n",
											S"MOG SERVER: ", MOG_ControllerSystem::GetServerComputerName());
		MOG_Report::ReportMessage(message, e->Message, e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
		return NULL;
	}
	__finally
	{
		if( myReader != NULL )
		{
			myReader->Close();
		}
			myConnection->Close();
	}

	return location;
}

ArrayList* MOG_DBSyncedDataAPI::QuerySyncedLocations(String* selectString)
{
	SqlConnection* myConnection = MOG_DBAPI::GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());
	ArrayList* syncedLocations = new ArrayList();

	SqlDataReader* myReader = NULL;
	try
	{
		myReader = MOG_DBAPI::GetNewDataReader(selectString, myConnection);
		while(myReader->Read())
		{
			syncedLocations->Add(SQLReadSyncedLocation(myReader));
		}
	}
	catch(Exception* e)
	{
		String* message = String::Concat(	S"Could not get records from SQL database!\n",
											S"MOG SERVER: ", MOG_ControllerSystem::GetServerComputerName());
		MOG_Report::ReportMessage(message, e->Message, e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
		return NULL;
	}
	__finally
	{
		if( myReader != NULL )
		{
			myReader->Close();
		}
			myConnection->Close();
	}
	return syncedLocations;
}

bool MOG_DBSyncedDataAPI::RenameWorkspaceTab(String* computerName, String* projectName, String* platformName, String* workingDirectory, String* userName, String* newTabName)
{
	String* updateQuery = String::Concat(S"UPDATE ",MOG_ControllerSystem::GetDB()->GetSyncedDataLocationsTable(),S" SET TabText = '", MOG_DBAPI::FixSQLParameterString(newTabName), S"' WHERE ID =", Convert::ToString(GetSyncedLocationID(computerName, projectName, platformName, workingDirectory, userName)));
	return MOG_DBAPI::RunNonQuery(updateQuery,MOG_ControllerSystem::GetDB()->GetConnectionString());
}


bool MOG_DBSyncedDataAPI::SwitchWorkspaceBranch(String* computerName, String* projectName, String* platformName, String* workingDirectory, String* userName, String *newBranchName)
{
	bool bSwitched = false;

	// Get the newBranchID
	int newBranchID = MOG_DBProjectAPI::GetBranchIDByName(newBranchName);
	if (newBranchID)
	{
		String* updateQuery = String::Concat(S"UPDATE ",MOG_ControllerSystem::GetDB()->GetSyncedDataLocationsTable(),S" SET BranchID = ", newBranchID.ToString(), S" WHERE ID =", Convert::ToString(GetSyncedLocationID(computerName, projectName, platformName, workingDirectory, userName)));
		bSwitched = MOG_DBAPI::RunNonQuery(updateQuery,MOG_ControllerSystem::GetDB()->GetConnectionString());
	}

	return bSwitched;
}


HybridDictionary* MOG_LocalSyncInfo::ConvertFromArrayListToHybridDictionary(ArrayList* arrayList)
{
	HybridDictionary* hybridDictionary = NULL;

	// Make sure we have a valid list
	if (arrayList != NULL)
	{
		// Create a new HybridDictionary list
		hybridDictionary = new HybridDictionary(true);

		// Loop through the specified arrayList adding each item
		for (int i = 0; i < arrayList->Count; i++)
		{
			// Check if we have found a matching asset?
			MOG_Filename* tempFilename = __try_cast <MOG_Filename*>(arrayList->Item[i]);
			hybridDictionary->Item[tempFilename->GetAssetFullName()] = tempFilename;
		}
	}

	return hybridDictionary;
}

ArrayList* MOG_LocalSyncInfo::ConvertFromHybridDictionaryToArrayList(HybridDictionary* hybridDictionary)
{
	ArrayList* arrayList = NULL;

	// Make sure we have a valid list
	if (hybridDictionary != NULL)
	{
		// Check if we had more than one item?
		arrayList = new ArrayList(hybridDictionary->Values);
		if (arrayList->Count > 1)
		{
			// Only sort this sucka if it was filled with something we want to sort
			if (dynamic_cast<System::IComparable*>(arrayList->Item[0]))
			{
				// Make sure we sort our arraylist
				arrayList->Sort();
			}
		}
	}

	// Sort the array
	arrayList->Sort();

	return arrayList;
}

