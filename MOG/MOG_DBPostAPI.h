//--------------------------------------------------------------------------------
//	MOG_DBPostAPI.h
//	
//	
//--------------------------------------------------------------------------------
#ifndef __MOG_DBPOSTAPI_H__
#define __MOG_DBPOSTAPI_H__


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
	String *mVersion;
	String *mGroupID;
	String *mBranchName;
	String *mCreatedBy;
	String *mBlessedBy;
};


public __gc class MOG_DBPostAPI : public MOG_DBAPI
{
public:
	MOG_DBPostInfo *SchedulePost(String *assetName, String *version, String *groupID, String *branchName, String *creatorName, String *blesserName);
	ArrayList *GetScheduledPosts(String *groupID, String *branchName);
	bool RemovePost(MOG_DBPostInfo *post);
	bool RemovePosts(ArrayList *posts);

private:
	MOG_DBPostInfo *SQLCreatePost(String *assetName, String *version, String *groupID, String *branchName, String *creatorName, String *blesserName);
	MOG_DBPostInfo *QueryPost(String *selectString);
	ArrayList *QueryPosts(String *selectString);
	MOG_DBPostInfo *SQLReadPost(SqlDataReader *myReader);
	bool SQLWritePost(String *assetName, String *version, String *groupID, String *branchName, String *creatorName, String *blesserName);
};

}
}

using namespace MOG;
using namespace MOG::DATABASE;

#endif


