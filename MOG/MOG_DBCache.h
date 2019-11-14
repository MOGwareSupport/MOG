#ifndef __MOG_DBCACHE_H__
#define __MOG_DBCACHE_H__

#using <mscorlib.dll>
#using <system.dll>
#using <System.Data.dll>

#include "stdlib.h"
#include "MOG_Define.h"
#include "MOG_User.h"


using namespace System;
using namespace System::Data;
using namespace System::Data::SqlClient;
using namespace System::Collections;
using namespace System::Collections::Specialized;



namespace MOG {
namespace DATABASE {



public __gc class MOG_DBGenericCache
{
public:
	MOG_DBGenericCache(String *populationQuery, String *idColumn, String *nameColumn, String *listColumn, bool bPreCacheNow, bool bAutoRebuildCacheForMissingSets);

	void SetEnabled(bool enabled) { mEnabled = enabled; };
	bool IsEnabled() { return mEnabled; };

	HybridDictionary *GetWorkingNameCache(String *cacheListName);
	HybridDictionary *GetWorkingIDCache(String *cacheListName);

	bool AddSetToCache(int setID, String *setName);
	bool AddSetToCache(int SetID, String *setName, String *cacheListName);

	bool RemoveSetFromCacheByID(int setID);
	bool RemoveSetFromCacheByID(int setID, String *cacheListName);
	bool RemoveSetFromCacheByName(String *setName);
	bool RemoveSetFromCacheByName(String *setName, String *cacheListName);

	bool UpdateSetByName(String *oldSetName, String *newSetName);
	bool UpdateSetByName(String *oldSetName, String *newSetName, String *cacheListName);
	bool ClearCache();

	int GetIDFromName(String *setName);
	int GetIDFromName(String *setName, String *cacheListName);
	String *GetNameFromID(int setID);
	String *GetNameFromID(int setID, String *cacheListName);

	bool IsPreCached(void) { return mIsPreCached; };
	
	bool PopulateCacheFromSQL();
	bool PopulateCacheFromSQL(String *populationQuery, String *idColumn, String *nameColumn);
	bool PopulateCacheFromSQL(String *populationQuery, String *idColumn, String *nameColumn, String *cacheNameColumn);


private:
	bool mEnabled;
	String *mPopulationQuery;
	String *mIdColumn;
	String *mNameColumn;
	String *mListColumn;
	HybridDictionary *mNameCache;
	HybridDictionary *mIDCache;
	bool mIsPreCached;
};



public __gc class MOG_DBUsersCache
{

public:
	MOG_DBUsersCache();

	int GetUserIDByName(String *userName);
	int GetActiveUserID(void);
	int GetCurrentUserID(void);

};



public __gc class MOG_DBCache
{

public:
	MOG_DBCache(void);
	void FlushDBCache(void);

	MOG_DBUsersCache *GetUserCache(void);
	MOG_DBGenericCache *GetDepartmentCache(void);
	MOG_DBGenericCache *GetBranchCache(void);
	MOG_DBGenericCache *GetClassificationCache(void);
	MOG_DBGenericCache *GetClassificationBranchLinkCache(void);
	MOG_DBGenericCache *GetAssetVersionCache(void);
	MOG_DBGenericCache *GetAssetNameCache(void);
	MOG_DBGenericCache *GetInboxAssetCache(void);
	MOG_DBGenericCache *GetSyncedDataLocationsCache(void);
	MOG_DBGenericCache *GetPackageGroupCache(void);

private:
	MOG_DBUsersCache *mUserCache;
	MOG_DBGenericCache *mDepartmentCache;
	MOG_DBGenericCache *mBranchCache;
	MOG_DBGenericCache *mClassificationCache;
	MOG_DBGenericCache *mClassificationBranchLinkCache;
	MOG_DBGenericCache *mAssetVersionCache;
	MOG_DBGenericCache *mAssetNameCache;
	MOG_DBGenericCache *mInboxAssetCache;
	MOG_DBGenericCache *mSyncedDataLocationsCache;
	MOG_DBGenericCache *mPackageGroupCache;

	void Initialize(void);
};


}
}

using namespace MOG;
using namespace MOG::DATABASE;

#endif



