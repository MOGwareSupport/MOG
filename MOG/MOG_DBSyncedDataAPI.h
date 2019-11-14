//--------------------------------------------------------------------------------
//	MOG_DBSyncedDataAPI.h
//	
//	
//--------------------------------------------------------------------------------
#ifndef __MOG_DBSYNCEDDATAAPI_H__
#define __MOG_DBSYNCEDDATAAPI_H__


#using <mscorlib.dll>
#using <system.dll>
#using <System.Data.dll>

#include "stdlib.h"
#include "MOG_Define.h"

using namespace System;
using namespace System::Data;
using namespace System::Data::SqlClient;


namespace MOG {
namespace DATABASE {

public __gc class MOG_DBUpdatePair : public System::IComparable
{
public:
	MOG_Filename* mOldVersion;
	MOG_Filename* mNewVersion;

	/// Implementation for IComparable .NET System interface (for ArrayList::Sort() )
	int CompareTo(Object *obj)
	{
		// If this is a MOG_Filename*, compare it as such
		if( obj->GetType() == __typeof(MOG_DBUpdatePair) ) 
		{
			MOG_DBUpdatePair *compareTo = __try_cast<MOG_DBUpdatePair*>(obj);
			return this->mNewVersion->GetAssetFullName()->CompareTo( compareTo->mNewVersion->GetAssetFullName() );
		}

		// Else throw
		throw new ArgumentException(S"Object being compared is not a MOG_DBUpdatePair");
	}
};

public __gc struct MOG_DBSyncedLocationInfo
{
	int mID;
	String* mComputerName;
	String* mProjectName;
	String* mPlatformName;
	String* mWorkingDirectory;
	String* mBranchName;
	String* mTabName;
	String* mUserName;
};

//New object for creating SyncLists.  This is to eliminate the necessity to make complex calls to the database.
public __gc class MOG_LocalSyncInfo
{
public:
	MOG_LocalSyncInfo(MOG_DBSyncedLocationInfo*  syncedLocation);
	MOG_LocalSyncInfo(String* computerName, String* projectName, String* platformName, String* workingDirectory, String* branchName, String* userName);
	MOG_LocalSyncInfo(String* computerName, String* projectName, String* platformName, String* workingDirectory, String* branchName, String* userName, String* classification, String* syncLocation);

	MOG_DBSyncedLocationInfo* GetSyncedLocationInfo()			{ return mSyncedLocationInfo; };
	ArrayList* GetPreviouslySyncedAssetList()					{ return ConvertFromHybridDictionaryToArrayList(mPreviouslySyncedAssetList); };
	ArrayList* GetPotentialAssetsToBeSyncedList()				{ return ConvertFromHybridDictionaryToArrayList(mPotentialAssetsToBeSyncedList); };
	ArrayList* GetRemoveAssetList()								{ return ConvertFromHybridDictionaryToArrayList(mRemoveList); };
	ArrayList* GetAddAssetList()								{ return ConvertFromHybridDictionaryToArrayList(mAddList); };
	ArrayList* GetUpdateAssetList()								{ return ConvertFromHybridDictionaryToArrayList(mUpdateList); };
	ArrayList* GetLocalUpdateTrayList()							{ return ConvertFromHybridDictionaryToArrayList(mLocalUpdateTrayList); };

	bool IsNonSyncingAsset(MOG_Filename* assetFilename);
	bool IsAssetIncluded(MOG_Filename* assetFilename, String* exclusionList, String* inclusionList);
	bool IsAssetInLocalUpdatedTray(MOG_Filename* assetFilename);
	MOG_Filename* GetAssetInLocalUpdatedTray(MOG_Filename* assetFilename);

	static HybridDictionary* ConvertFromArrayListToHybridDictionary(ArrayList* arrayList);
	static ArrayList* ConvertFromHybridDictionaryToArrayList(HybridDictionary* hybridDictionary);

private:
	MOG_DBSyncedLocationInfo* mSyncedLocationInfo;
	String* mClassificationFilter;
	String* mSyncLocationFilter;

	HybridDictionary* mPreviouslySyncedAssetList;
	HybridDictionary* mPotentialAssetsToBeSyncedList;
	HybridDictionary* mIrrelevantAssets;
	HybridDictionary* mNonSyncingAssets;
//	HybridDictionary* mLibraryAssets;

	HybridDictionary* mRemoveList;
	HybridDictionary* mAddList;
	HybridDictionary* mUpdateList;

	HybridDictionary* mLocalUpdateTrayList;

	void InitilizeLocalSyncInfo(String* computerName, String* projectName, String* platformName, String* workingDirectory, String* branchName, String* userName, String* classificationFilter, String* syncLocationFilter);
	bool PopulateLocalSyncInfo();
};

public __gc class MOG_DBSyncedDataAPI
{
public:
	static int GetSyncedLocationID(String* workspaceDirectory);
	static int GetSyncedLocationID(String* computerName, String* projectName, String* platformName, String* workingDirectory, String* userName);

	static bool AddSyncedLocation(String* computerName, String* projectName, String* platformName, String* workingDirectory, String* branchName, String* tabText, String* userName);
	static bool RemoveSyncedLocation(String* computerName, String* projectName, String* platformName, String* workingDirectory, String* userName);

	static MOG_DBSyncedLocationInfo* GetSyncedLocationByID(int workspaceID);
	static ArrayList* GetAllSyncedLocations(String* computerName, String* workingDirectory, String* platformName, String* userName);
	static bool DoesSyncedLocationExist(String* computerName, String* projectName, String* platformName, String* workingDirectory, String* userName);

	static bool AddSyncedLocationLink(MOG_Filename* blessedAssetFilename, String* computerName, String* projectName, String* platformName, String* workingDirectory, String* userName);
	static bool RemoveSyncedLocationLink(MOG_Filename* blessedAssetFilename, String* computerName, String* projectName, String* platformName, String* workingDirectory, String* userName);

	static ArrayList* GetCurrentSyncedAssets(String* computerName, String* projectName, String* platformName, String* workingDirectory, String* userName);
	static ArrayList* GetCurrentSyncedAssetsViaClassification(String* computerName, String* projectName, String* platformName, String* workingDirectory, String* userName, String* classification);
	static ArrayList* GetCurrentSyncedAssetsViaSyncLocation(String* computerName, String* projectName, String* platformName, String* workingDirectory, String* userName, String* syncLocation);

	static String* GetLocalAssetVersion(MOG_Filename* assetName, String *workspaceDirectory, String *workspacePlatformName);
	static String* GetLocalAssetVersion(String* computerName, String* projectName, String* platformName, String* workingDirectory, String* userName, MOG_Filename* assetName);
	static String* GetLocalAssetVersion(int syncedLocationID, MOG_Filename* assetName);
	
	static bool RenameWorkspaceTab(String* computerName, String* projectName, String* platformName, String* workingDirectory, String* userName, String* newTabNam);
	static bool SwitchWorkspaceBranch(String* computerName, String* projectName, String* platformName, String* workingDirectory, String* userName, String *newBranchName);
	
protected:
	static MOG_DBSyncedLocationInfo* SQLReadSyncedLocation(SqlDataReader* myReader);
	static MOG_DBSyncedLocationInfo* QuerySyncedLocation(String* selectString);
	static ArrayList* QuerySyncedLocations(String* selectString);
};


}
}

using namespace MOG;
using namespace MOG::DATABASE;

#endif


