//--------------------------------------------------------------------------------
//	MOG_DBPackageCommandAPI.h
//	
//	
//--------------------------------------------------------------------------------
#ifndef __MOG_DBPACKAGECOMMANDAPI_H__
#define __MOG_DBPACKAGECOMMANDAPI_H__


#using <mscorlib.dll>
#using <system.dll>
#using <System.Data.dll>

#include "stdlib.h"
#include "MOG_Define.h"
#include "MOG_DBAPI.h"


using namespace System;
using namespace System::Data;
using namespace System::Data::SqlClient;



namespace MOG {
namespace DATABASE {

public __value enum MOG_PACKAGECOMMAND_TYPE
{
	MOG_PackageCommand_DeletePackage,
	MOG_PackageCommand_AddAsset,
	MOG_PackageCommand_RemoveAsset,
	MOG_PackageCommand_LateResolver,
};

public __gc struct MOG_DBPackageCommandInfo
{
	int mID;
	String *mAssetName;
	String *mAssetVersion;
	String *mLabel;
	String *mBranchName;
	String *mPlatform;
	String *mCreatedBy;
	String *mBlessedBy;
	String *mPackageName;
	String *mPackageGroups;
	String *mPackageObjects;
	MOG_PACKAGECOMMAND_TYPE mPackageCommandType;
};


public __gc class MOG_DBPackageCommandAPI
{
public:
	static MOG_DBPackageCommandInfo *SchedulePackageCommand(String *assetName, String *assetVersion, String *jobLabel, String *branchName, String *platform, String *creatorName, String *blesserName, String *packageName, String *packageGroups, String *packageObjects, MOG_PACKAGECOMMAND_TYPE packageCommandType);
	static MOG_DBPackageCommandInfo *SchedulePackageLateResolver( String *packageName, String *linkedPackageName);
	static ArrayList *GetScheduledPackageCommands(String *branchName);
	static ArrayList *GetScheduledPackageCommands(String *branchName, String *platform);
	static ArrayList *GetScheduledPackageCommands(String *jobLabel, String *branchName, String *platform);
	static ArrayList *GetScheduledPackageCommands(String *packageName, String *jobLabel, String *branchName, String *platform);
	static ArrayList *GetScheduledLateResolverCommands(String *packageName, String *branchName);
	static ArrayList *GetScheduledLateResolverCommands(String *packageName, String *branchName, String *platform);
	static bool RemovePackageCommand(MOG_DBPackageCommandInfo *packageCommand);
	static bool RemovePackageCommand(MOG_Filename *blessedAssetFilename);
	static bool RemovePackageCommand(MOG_Filename *blessedAssetFilename, String *branchName);
	static bool RemovePackageCommand(MOG_Filename *blessedAssetFilename, String *jobLabel, String *branchName, String *platformName);
	static bool RemovePackageCommand(unsigned int commandId);
	static bool RemovePackageCommands(ArrayList *packageCommands);
	static bool RemoveStalePackageCommands(ArrayList *recentlyRemovedCommands, String *branchName);

private:
	static MOG_DBPackageCommandInfo *SQLCreatePackageCommand(String *assetName, String *assetVersion, String *jobLabel, String *branchName, String *platform, String *creatorName, String *blesserName, String *packageName, String *packageGroups, String *packageObjects, MOG_PACKAGECOMMAND_TYPE packageCommandType);
	static MOG_DBPackageCommandInfo *QueryPackageCommand(String *selectString);
	static ArrayList *QueryPackageCommands(String *selectString);
	static MOG_DBPackageCommandInfo *SQLReadPackageCommand(SqlDataReader *myReader);
	static bool SQLWritePackageCommand(String *assetName, String *assetVersion, String *jobLabel, String *branchName, String *platform, String *creatorName, String *blesserName, String *packageName, String *packageGroups, String *packageObjects, MOG_PACKAGECOMMAND_TYPE packageCommandType);
};

}
}

using namespace MOG;
using namespace MOG::DATABASE;

#endif


