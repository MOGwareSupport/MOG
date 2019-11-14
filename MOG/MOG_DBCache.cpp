#include "stdafx.h"
#include "MOG_Database.h"
#include "MOG_ControllerProject.h"
#include "MOG_ControllerSystem.h"
#include "MOG_DBCache.h"
#include "MOG_DBAPI.h"



//MOG GenericCache
MOG_DBGenericCache::MOG_DBGenericCache(String *populationQuery, String *idColumn, String *nameColumn, String *listColumn, bool cacheNow, bool bAutoRebuildCacheForMissingSets )
{
	mEnabled = true;

	//Set all the Query information on how to Query this Cache
	mPopulationQuery = populationQuery;
	mIdColumn = idColumn;
	mNameColumn = nameColumn;
	mListColumn = listColumn;
	mNameCache = new HybridDictionary(true);
	mIDCache = new HybridDictionary(true);
	//If we want to cache immideatly do so.
	if(cacheNow)
	{
		if(populationQuery && populationQuery->Length && 
		   mIdColumn && mIdColumn->Length &&
		   mNameColumn && mNameColumn->Length)
		{
			PopulateCacheFromSQL();
			mIsPreCached = true;
		}
	}
}

//This function adds an item to a single cache set.
HybridDictionary *MOG_DBGenericCache::GetWorkingNameCache(String *cacheListName)
{
	HybridDictionary *workingNameCache = mNameCache;

	// Check if we are a complex cache
	if (cacheListName != NULL && cacheListName->Length > 0)
	{
		// Check if this sub cache exists
		if (workingNameCache->Contains(cacheListName))
		{
			// Get the sub cache
			workingNameCache = __try_cast<HybridDictionary *>(mNameCache->Item[cacheListName]);
		}
		else
		{
			// Create a new sub cache
			workingNameCache = new HybridDictionary(true);
			mNameCache->Item[cacheListName] = workingNameCache;
		}
	}

	return workingNameCache;
}

//This function adds an item to a single cache set.
HybridDictionary *MOG_DBGenericCache::GetWorkingIDCache(String *cacheListName)
{
	HybridDictionary *workingIDCache = mIDCache;

	// Check if we are a complex cache
	if (cacheListName != NULL && cacheListName->Length > 0)
	{
		// Check if this sub cache exists
		if (workingIDCache->Contains(cacheListName))
		{
			// Get the sub cache
			workingIDCache = __try_cast<HybridDictionary *>(mIDCache->Item[cacheListName]);
		}
		else
		{
			// Create a new sub cache
			workingIDCache = new HybridDictionary(true);
			mIDCache->Item[cacheListName] = workingIDCache;
		}
	}

	return workingIDCache;
}

//This function adds an item to a single cache set.
bool MOG_DBGenericCache::AddSetToCache(int setID, String *setName)
{
	return AddSetToCache(setID, setName, NULL);
}

//adds an item to a complex cache
bool MOG_DBGenericCache::AddSetToCache(int setID, String *setName, String *cacheListName)
{
	// Make sure our cache is enabled?
	if (mEnabled)
	{
		// Make sure valid info was specified
		if (setID != 0 && 
			setName != NULL)
		{
			HybridDictionary *workingNameCache = GetWorkingNameCache(cacheListName);
			HybridDictionary *workingIDCache = GetWorkingIDCache(cacheListName);

			// Add/update the new cache item
			workingNameCache->Item[setName] = __box(setID);
			workingIDCache->Item[setID.ToString()] = setName;

			return true;
		}
	}

	return false;
}

bool MOG_DBGenericCache::RemoveSetFromCacheByID(int setID)
{
	//Remove with a null cache list
	return RemoveSetFromCacheByID(setID, NULL);
}

bool MOG_DBGenericCache::RemoveSetFromCacheByID(int setID, String *cacheListName)
{
	// Make sure valid info was specified
	if (setID != 0)
	{
		HybridDictionary *workingNameCache = GetWorkingNameCache(cacheListName);
		HybridDictionary *workingIDCache = GetWorkingIDCache(cacheListName);

		// Check if this ID is cached
		if (workingIDCache->Contains(setID.ToString()))
		{
			// Get the setName
			String *setName = Convert::ToString(workingIDCache->Item[setID.ToString()]);

			// Remove the cached items
			workingNameCache->Remove(setName);
			workingIDCache->Remove(setID.ToString());
		}

		return true;
	}

	return false;
}

bool MOG_DBGenericCache::RemoveSetFromCacheByName(String * setName)
{
	//This calls remove set by name with a null cacheListName.
	return RemoveSetFromCacheByName(setName, NULL);
}

bool MOG_DBGenericCache::RemoveSetFromCacheByName(String *setName, String *cacheListName)
{
	// Make sure valid info was specified
	if (setName != NULL)
	{
		HybridDictionary* workingNameCache = GetWorkingNameCache(cacheListName);
		HybridDictionary* workingIDCache = GetWorkingIDCache(cacheListName);

		// Check if this Name is cached
		if (workingNameCache->Contains(setName))
		{
			try
			{
				// Get the setID
				int setID = Convert::ToInt32(workingNameCache->Item[setName]);

				// Remove the cached items
				workingNameCache->Remove(setName);
				workingIDCache->Remove(setID.ToString());
			}
			catch(...)
			{
			}
		}

		return true;
	}

	return false;
}

//This function just generically clears this specific cache.
bool MOG_DBGenericCache::ClearCache()
{
	mIsPreCached = false;
	mNameCache = new HybridDictionary(true);
	mIDCache = new HybridDictionary(true);
	return true;
}

//This function takes an old name and a new name and replaces the instance of the first name with the second value. (simple cache)
bool MOG_DBGenericCache::UpdateSetByName(String *oldSetName, String *newSetName)
{
	return UpdateSetByName(oldSetName, newSetName, NULL);
}

//This is the cache update by name mecanics and the call for complex caches.
bool MOG_DBGenericCache::UpdateSetByName(String *oldSetName, String *newSetName, String *cacheListName)
{
	// Make sure valid info was specified
	if (oldSetName != NULL && 
		newSetName != NULL)
	{
		HybridDictionary *workingNameCache = GetWorkingNameCache(cacheListName);

		// Make sure the oldSetName exists
		if (workingNameCache->Contains(oldSetName))
		{
			try
			{
				// Get the old value
				int setID = Convert::ToInt32(workingNameCache->Item[oldSetName]);
				if (setID)
				{
					HybridDictionary *workingIDCache = GetWorkingNameCache(cacheListName);

					// Remove the cached items
					workingNameCache->Remove(oldSetName);
					workingIDCache->Remove(setID.ToString());
					// Add/update the new cache items
					workingNameCache->Item[newSetName] = __box(setID);
					workingIDCache->Item[setID.ToString()] = newSetName;
					return true;
				}
			}
			catch(...)
			{
			}
		}
	}

	return false;
}

//Function to Search cache for name and return the ID
int MOG_DBGenericCache::GetIDFromName(String *setName)
{
	return GetIDFromName(setName, NULL);
}

int MOG_DBGenericCache::GetIDFromName(String *setName, String *cacheListName)
{
	int setID = 0;

	// Make sure our cache is enabled?
	if (mEnabled)
	{
		// Make sure valid info was specified
		if (setName != NULL)
		{
			HybridDictionary *workingNameCache = GetWorkingNameCache(cacheListName);

			// Make sure the setName exists
			if (workingNameCache->Contains(setName))
			{
				try
				{
					// Get the value
					setID = Convert::ToInt32(workingNameCache->Item[setName]);
				}
				catch(...)
				{
				}
			}
		}
	}

	return setID;
}

//Code to search for a Item By ID and return the String name value.
String *MOG_DBGenericCache::GetNameFromID(int setID)
{
	return MOG_DBGenericCache::GetNameFromID(setID, NULL);
}

String *MOG_DBGenericCache::GetNameFromID(int setID, String *cacheListName)
{
	String *setName = NULL;

	// Make sure our cache is enabled?
	if (mEnabled)
	{
		// Make sure valid info was specified
		if (setID)
		{
			HybridDictionary *workingIDCache = GetWorkingIDCache(cacheListName);

			// Make sure the setID exists
			if (workingIDCache->Contains(setID.ToString()))
			{
				// Get the value
				setName = Convert::ToString(workingIDCache->Item[setID.ToString()]);
			}
		}
	}

	return setName;
}

//This function is used to pre populate a simple cache from SQL.
bool  MOG_DBGenericCache::PopulateCacheFromSQL(String *populationQuery, String *idColumn, String *nameColumn)
{
	mPopulationQuery = populationQuery;
	mIdColumn = idColumn;
	mNameColumn = nameColumn;
	return PopulateCacheFromSQL();
}

//  This fuction is used to prepopulate a complex cache.
// We need to make sure we are passing all valid values.  it will work to set up a non complex cache but there are other calls that are more approprate do do so.
bool MOG_DBGenericCache::PopulateCacheFromSQL(String *populationQuery, String *idColumn, String *nameColumn, String *cacheNameColumn)
{
	mPopulationQuery = populationQuery;
	mIdColumn = idColumn;
	mNameColumn = nameColumn;
	mListColumn = cacheNameColumn;
	return PopulateCacheFromSQL();
}

//Populate The cache from SQL
//The cache must know how to precache it's own data at this point.
//This function will diffrentiate between simple and complex cache
bool MOG_DBGenericCache::PopulateCacheFromSQL(void)
{
	//Check to see if we have the values which are necessary to precache.  We need to have the precache query as well as a minimum of 2 columns
	//need the column containing the cached object's ID
	//need the column containing the cached object's Name
	if (!mPopulationQuery || !mPopulationQuery->Length || 
		!mIdColumn || !mIdColumn->Length ||
		!mNameColumn || !mNameColumn->Length)
	{
		//if we don't have the minimum stuff don't precache.
		return false;
		mIsPreCached = false;
	}
	//create a connection and command
	SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());

	SqlDataReader *myReader;
	try
	{
		//Populate the Cache with data from the Query
		myReader = MOG_DBAPI::GetNewDataReader(mPopulationQuery, myConnection);
		//SqlDataReader *myReader = myCommand->ExecuteReader();
		//once we have a list of values to populate the cache from.
		while(myReader->Read())
		{
			//Get the id tied to name
			int setID = myReader->GetInt32(myReader->GetOrdinal(mIdColumn));
			String *setName = myReader->GetValue(myReader->GetOrdinal(mNameColumn))->ToString();

			//If this is a simple cache add it to the cache.
			if (mListColumn == NULL || mListColumn->Length == 0)
			{
				mNameCache->Item[setName] = __box(setID);
			}
			else
			{
				String *cacheListName = myReader->GetValue(myReader->GetOrdinal(mListColumn))->ToString();
				HybridDictionary *workingNameCache = GetWorkingNameCache(cacheListName);
				HybridDictionary *workingIDCache = GetWorkingIDCache(cacheListName);
				workingNameCache->Item[setName] = __box(setID);
				workingIDCache->Item[setID.ToString()] = setName;
			}
		}
	}
	catch(Exception *e)
	{
		String *message = String::Concat(	S"Could not get records from SQL database!\n",
											S"MOG SERVER: ", MOG_ControllerSystem::GetServerComputerName());
		MOG_Report::ReportMessage(message, e->Message, e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
		mIsPreCached = false;
		return false;
	}
	__finally
	{	//Clean Up
		myReader->Close();
		myConnection->Close();
	}

	mIsPreCached = true;
	return true;
}


//MOG DBCache Class
MOG_DBCache::MOG_DBCache()
{
	Initialize();
}

void MOG_DBCache::Initialize(void)
{
	// Initialize all caches...
	if (mUserCache == NULL)
	{
		mUserCache = new MOG_DBUsersCache();
	}
	if (mDepartmentCache == NULL)
	{
		mDepartmentCache = new MOG_DBGenericCache("","","","",false,false);
	}
	if (mBranchCache == NULL)
	{
		mBranchCache = new MOG_DBGenericCache("","","","",false,false);
	}
	if (mClassificationCache == NULL)
	{
		mClassificationCache = new MOG_DBGenericCache("","","","",false,false);
	}
	if (mClassificationBranchLinkCache == NULL)
	{
		mClassificationBranchLinkCache = new MOG_DBGenericCache("","","","",false,false);
	}
	if (mAssetVersionCache == NULL)
	{
		mAssetVersionCache = new MOG_DBGenericCache("","","","",false,false);
	}
	if (mAssetNameCache == NULL)
	{
		mAssetNameCache = new MOG_DBGenericCache("","","","",false,false);
	}
	if (mSyncedDataLocationsCache == NULL)
	{
		mSyncedDataLocationsCache = new MOG_DBGenericCache("","","","",false,false);
	}
	if (mInboxAssetCache == NULL)
	{
		mInboxAssetCache = new MOG_DBGenericCache("","","","",false,false);
	}
	if (mPackageGroupCache == NULL)
	{
		mPackageGroupCache = new MOG_DBGenericCache("","","","",false,false);
	}
}

void MOG_DBCache::FlushDBCache(void)
{
	// Flush all caches...
// JohnRen - We still don't have a ClearCache for MOG_DBUsersCache so just reallocate a new one...
//	mUserCache->ClearCache();
	mUserCache = new MOG_DBUsersCache();
	mDepartmentCache->ClearCache();
	mBranchCache->ClearCache();
	mClassificationCache->ClearCache();
	mClassificationBranchLinkCache->ClearCache();
	mAssetVersionCache->ClearCache();
	mAssetNameCache->ClearCache();
	mSyncedDataLocationsCache->ClearCache();
	mInboxAssetCache->ClearCache();
	mPackageGroupCache->ClearCache();
}

MOG_DBUsersCache *MOG_DBCache::GetUserCache(void)
{
	return mUserCache;
}

MOG_DBGenericCache *MOG_DBCache::GetAssetNameCache()
{
	return mAssetNameCache;
}

MOG_DBGenericCache *MOG_DBCache::GetAssetVersionCache()
{
	return mAssetVersionCache;
}

MOG_DBGenericCache *MOG_DBCache::GetBranchCache()
{
	return mBranchCache;
}

MOG_DBGenericCache *MOG_DBCache::GetClassificationCache()
{
	return mClassificationCache;
}

MOG_DBGenericCache *MOG_DBCache::GetClassificationBranchLinkCache()
{
	return mClassificationBranchLinkCache;
}

MOG_DBGenericCache *MOG_DBCache::GetDepartmentCache()
{
	return mDepartmentCache;
}

MOG_DBGenericCache *MOG_DBCache::GetInboxAssetCache(void)
{
	return mInboxAssetCache;
}
MOG_DBGenericCache *MOG_DBCache::GetSyncedDataLocationsCache(void)
{
	return mSyncedDataLocationsCache;
}
MOG_DBGenericCache *MOG_DBCache::GetPackageGroupCache(void)
{
	return mPackageGroupCache;
}




//User Cache Class
MOG_DBUsersCache::MOG_DBUsersCache()
{
	
}

int MOG_DBUsersCache::GetUserIDByName(String *userName)
{
	MOG_Project *pProject = MOG_ControllerProject::GetProject();
	if (pProject)
	{
		MOG_User *pUser = pProject->GetUser(userName);
		if (pUser != NULL)
		{
			return pUser->GetUserID();
		}
	}

	//This is bad but we don't have anything else to give them.
	return 0;
}

int MOG_DBUsersCache::GetActiveUserID(void)
{
	// Check to make sure this is a User
	MOG_User *activeUser = MOG_ControllerProject::GetProject()->GetUser(MOG_ControllerProject::GetActiveUserName());
	if(activeUser != NULL)
	{
		return activeUser->GetUserID();
	}

	//This is bad but we don't have anything else to give them.
	return 0;
}

int MOG_DBUsersCache::GetCurrentUserID(void)
{
	//Check to make sure this is a User
	if(MOG_ControllerProject::IsUser())
	{
		return MOG_ControllerProject::GetUser()->GetUserID();
	}

	//This is bad but we don't have anything else to give them.
	return 0;
}

//Under MOG_DBCache have a sub class for MOG_DBUserCache
//Constructor goes and gets all the users form the db and populates an ArrayList with all the users from the Database.
//We will also have a pointer to both the Current User and the Active User so we can short circut the search on the most common searches.
//create a method that returns a pointer to a user structure (even if this is only private this is useful to have for future.
//Need a function that goes to the database and gets all the user information then populates a user object for each item in the database and then adds it to the array list.
