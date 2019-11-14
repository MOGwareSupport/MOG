//--------------------------------------------------------------------------------
//	MOG_DBInboxAPI.h
//	
//	
//--------------------------------------------------------------------------------
#ifndef __MOG_DBINBOXAPI_H__
#define __MOG_DBINBOXAPI_H__


#using <mscorlib.dll>
#using <system.dll>
#using <System.Data.dll>

#include "stdlib.h"
#include "MOG_Define.h"
#include "MOG_Filename.h"
#include "MOG_Time.h"
#include "MOG_PropertiesIni.h"
#include "MOG_DBProjectAPI.h"


using namespace System;
using namespace System::Data;
using namespace System::Data::SqlClient;



namespace MOG {
namespace DATABASE {

public __gc struct MOG_DBInboxProperties
{
	MOG_Filename *mAsset;
	ArrayList *mProperties;
};

public __gc class MOG_DBInboxAPI
{
public:
	static bool InboxAddAsset(MOG_Filename *assetFilename);
	static bool InboxMoveAsset(MOG_Filename *sourceAssetFilename, MOG_Filename *destinationAssetFilename);
	static bool InboxRemoveAsset(MOG_Filename *assetFilename);
	static bool InboxRemoveAllAssets(MOG_Filename *boxFilename);
	static bool InboxRemoveAllAssets(String *boxName, String *userName, String *groupPath, String *fullNameFilter);
	static String *InboxGetAssetName(int inboxID);
	static int GetInboxAssetID(MOG_Filename *assetFilename);
	static ArrayList *InboxGetAssetListFromProperty(String *boxName, String *assetLabel, MOG_Property *property);
	static ArrayList *InboxGetAssetListFromImportedFilename(String *userName, String *filename);
	static ArrayList *InboxGetAssetListFromImportedDirectory(String *userName, String *dirname);
	static ArrayList *InboxGetAssetList();
	static ArrayList *InboxGetAssetList(MOG_Filename *boxFilename);
	static ArrayList *InboxGetAssetList(String *boxName, String *userName, String *groupPath, String *fullNameFilter);
	static ArrayList *InboxGetAssetListWithProperties(String *boxName, String *userName, String *groupPath, String *fullNameFilter);
	static ArrayList *InboxGetLocalAssetList(String *workspaceDirectory);
	static ArrayList *InboxGetLocalAssetList(String *workspaceDirectory, MOG_Filename *assetFilename);

	// inbox asset properties
	static bool UpdateInboxAssetProperty(MOG_Filename *assetFilename, MOG_Property *property);
	static bool UpdateInboxAssetProperties(MOG_Filename *assetFilename, ArrayList *properties);
	static bool RemoveInboxAssetProperty(MOG_Filename *assetFilename, String *propertySection, String *propertyName);
	static bool RemoveAllInboxAssetProperties(MOG_Filename *assetFilename);
	static bool RemoveAllInboxAssetProperties(MOG_Filename *assetFilename, String *propertySection);
	static String *GetInboxAssetProperty(MOG_Filename *assetFilename, String *propertySection, String *propertyName);
	static ArrayList *GetAllInboxAssetProperties(MOG_Filename *assetFilename);
	static ArrayList *GetAllInboxAssetProperties(MOG_Filename *assetFilename, String *propertySection);
	
	static bool UpdateCache(MOG_Filename *addFilename, MOG_Filename *delFilename);

private:
	static String *GetUserName(MOG_Filename *assetFilename);
	static String *GetBoxName(MOG_Filename *assetFilename);
	static MOG_Filename *SQLReadInboxAsset(SqlDataReader *myReader);
	static ArrayList* QueryInboxAssets(String* selectString);
	static bool SQLCreateInboxAssetProperty(MOG_Filename *assetFilename, String *propertySection, String *propertyName, String *propertyValue);

	static ArrayList* QueryInboxAssetsWithProperties(String* selectString);
};

}
}

using namespace MOG::DATABASE;
using namespace MOG;

#endif


