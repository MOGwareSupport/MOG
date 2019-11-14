//--------------------------------------------------------------------------------
//	MOG_DBPostCommandAPI.h
//	
//	
//--------------------------------------------------------------------------------
#ifndef __MOG_DBPOSTCOMMANDAPI_H__
#define __MOG_DBPOSTCOMMANDAPI_H__


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


public __gc struct MOG_DBPostInfo
{
	int mID;
	String *mAssetName;
	String *mAssetVersion;
	String *mLabel;
	String *mBranchName;
	String *mCreatedBy;
	String *mBlessedBy;
};


public __gc class MOG_DBPostCommandAPI
{
public:
	static MOG_DBPostInfo *SchedulePost(String *assetName, String *assetVersion, String *jobLabel, String *branchName, String *creatorName, String *blesserName);
	static ArrayList *GetScheduledPosts(String *branchName);
	static ArrayList *GetScheduledPosts(String *jobLabel, String *branchName);
	static ArrayList *GetScheduledPosts(String *assetName, String *jobLabel, String *branchName);
	static bool RemovePost(MOG_DBPostInfo *post);
	static bool RemovePost(MOG_Filename *blessedAssetFilename);
	static bool RemovePost(MOG_Filename *blessedAssetFilename, String *branchName);
	static bool RemovePost(MOG_Filename *blessedAssetFilename, String *jobLabel, String *branchName);
	static bool RemovePost(UInt32 commandId);
	static bool RemovePosts(ArrayList *posts);
	static bool RemoveStalePostCommands(ArrayList *recentlyRemovedCommands, String *branchName);

private:
	static MOG_DBPostInfo *SQLCreatePost(String *assetName, String *assetVersion, String *jobLabel, String *branchName, String *creatorName, String *blesserName);
	static MOG_DBPostInfo *QueryPost(String *selectString);
	static ArrayList *QueryPosts(String *selectString);
	static MOG_DBPostInfo *SQLReadPost(SqlDataReader *myReader);
	static bool SQLWritePost(String *assetName, String *assetVersion, String *jobLabel, String *branchName, String *creatorName, String *blesserName);

	static void RemovePosts_Worker(Object* sender, DoWorkEventArgs* e);
};

}
}

using namespace MOG;
using namespace MOG::DATABASE;

#endif


