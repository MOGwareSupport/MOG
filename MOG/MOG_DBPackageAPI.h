//--------------------------------------------------------------------------------
//	MOG_DBPackageAPI.h
//	
//	
//--------------------------------------------------------------------------------
#ifndef __MOG_DBPACKAGEAPI_H__
#define __MOG_DBPACKAGEAPI_H__


#using <mscorlib.dll>
#using <system.dll>
#using <System.Data.dll>

#include "stdlib.h"
#include "MOG_Define.h"
#include "MOG_Filename.h"
#include "MOG_DBAssetAPI.h"
#include "MOG_Filename.h"


using namespace System;
using namespace System::Data;
using namespace System::Data::SqlClient;



namespace MOG {
namespace DATABASE {

public __gc struct MOG_DBPackageInfo
{
	MOG_Filename *mPackageFilename;
	String *mPackageGroup;
};

public __gc class MOG_DBPackageAPI
{
private:
	static int GetPackageGroupID(int packageVersionID, String *packageGroupName);

	static MOG_DBPackageInfo *SQLReadPackageInfo(SqlDataReader *myReader);
	static ArrayList *QueryPackageGroups(String *selectString);

public:
	// Package Association
	static bool AddPackageLink(MOG_Filename *newBlessedPackageFilename, MOG_Filename *pBlessedAssetFilename, String *packageGroupName);
	static bool RemovePackageLink(MOG_Filename *newBlessedPackageFilename, MOG_Filename *pBlessedAssetFilename, String *packageGroupName);

	static ArrayList *GetAllAssetsInPackage(MOG_Filename *packageName, String *packageVersion);
	static ArrayList *GetAllAssetsInPackageGroup(MOG_Filename *packageName, String *packageVersion, String *packageGroupName);
	static ArrayList *GetAllActivePackagesByClassification(String *classification);

	static ArrayList *GetPackageGroupsForAsset(MOG_Filename *assetName);

	// Package File Management
	static bool AddPackage(MOG_Filename *packageName, String *createdBy);
	static bool RemovePackage(MOG_Filename *packageName, String *removedBy);
	static bool PopulateNewPackageVersion(MOG_Filename *oldPackageFilename, MOG_Filename *newBlessedPackageFilename, ArrayList *packageCommands);

	// Package Group Management
	static bool AddPackageGroupName(MOG_Filename *packageName, String *packageVersion, String *packageGroupName, String *createdBy);
	static bool RemovePackageGroupName(MOG_Filename *packageName, String *packageVersion, String *packageGroupName, String *removedBy);
	static ArrayList *GetAllPackageGroups(MOG_Filename *packageName, String *packageVersion);
	static ArrayList *GetAllActivePackageGroups(MOG_Filename *packageName, String *packageVersion);
};


}
}

using namespace MOG;
using namespace MOG::DATABASE;

#endif


