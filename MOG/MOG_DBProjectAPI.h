//--------------------------------------------------------------------------------
//	MOG_DBProjectAPI.h
//	
//	
//--------------------------------------------------------------------------------
#ifndef __MOG_DBPROJECTAPI_H__
#define __MOG_DBPROJECTAPI_H__


#using <mscorlib.dll>
#using <system.dll>
#using <System.Data.dll>

#include "stdlib.h"
#include "MOG_Define.h"
#include "MOG_User.h"
#include "MOG_DBAPI.h"
#include "MOG_ControllerSyncData.h"


using namespace System;
using namespace System::Data;
using namespace System::Data::SqlClient;



namespace MOG {
namespace DATABASE {

public __gc class MOG_DBBranchInfo
{
public:
	MOG_DBBranchInfo();

	int mID;
	String *mBranchName;
	String *mCreatedDate;
	String *mCreatedBy;
	String *mRemovedDate;
	String *mRemovedBy;
	bool mTag;
};

public __gc struct MOG_DBDepartmentInfo
{
	int mID;
	String *mDepartmentName;
};

public __gc struct MOG_DBPlatformInfo
{
	int mID;
	String *mPlatformName;
	String *mPlatformKey;
};


public __gc class MOG_DBProjectAPI
{
public:
	static String *GetDepartmentNameByUserName(String *userName);
	static ArrayList *GetActiveUserNames();
	static ArrayList *GetAllUsers();
	static ArrayList *GetPlatformNames();
	static ArrayList *GetPlatformsInBranch(String *branchName);
	static ArrayList *GetDepartments();
	static ArrayList *GetDepartmentUsers(String *departmentName);

	// Branch Related
	static ArrayList *GetAllUnreferencedAssetVersions(String *assetName);
	static ArrayList *GetActiveBranchNames();
	static ArrayList *GetAllTagNames();
	static ArrayList *GetAllBranchNames();
	static ArrayList *GetAllBranchNames(bool bTags);
	static ArrayList *GetAllBranchNames(String *projectName);
	static MOG_DBBranchInfo *GetBranch(String *branchName);
	static bool CreateNewBranch(String *oldBranch, String *newBranch);
	static bool CreateNewBranch(MOG_ControllerSyncData* workspace, String *newBranch);
	static bool AddBranch(String *branchName, String* createdBy, String *createdDate);
	static bool AddBranch(String *branchName, String* createdBy, String *createdDate, bool tag);
	static bool RemoveBranch(String *branchName, String* removedBy);
	static bool RenameBranch(String *oldBranchName, String *newBranchName);
	static bool PurgeBranch(String *branchName);

	// Platform Related
	static MOG_DBPlatformInfo *GetPlatform(String *platformName);
	static bool AddPlatformName(String *platformName);
	static bool AddPlatformToBranch(String *platformName, String *branchName);
	static bool RemovePlatformFromBranch(String *platformName, String* branchName);
	static bool RenamePlatform(String *oldPlatformName, String *newPlatformName);

	// Department Related
	static MOG_DBDepartmentInfo *GetDepartment(String *departmentName);
	static bool AddDepartment(String *departmentName);
	static bool RemoveDepartment(String *departmentName);
	static bool RenameDepartment(String *oldDepartmentName, String *newDepartmentName);

	// User Related
	static MOG_User *GetUser(String *userName);
	static ArrayList *GetUsers();
	static ArrayList *GetActiveUsers();
	static bool AddUser(String *userName, String *emailAddress, String *blessTarget, String *department, String *createdBy, String *createdDate);
	static bool UpdateUser(String *userName, String *emailAddress, String *blessTarget, String *department);
	static bool RemoveUser(String *userName, String *removedBy);
	static bool RenameUser(String *oldUserName, String *newUserName);

protected:
	// Branch Related
	static MOG_DBBranchInfo *SQLCreateBranch(String *branchName, String *createdBy, String *createdDate, bool tag);
	static MOG_DBBranchInfo *QueryBranch(String *selectString);
	static ArrayList *QueryBranches(String *selectString);
	static MOG_DBBranchInfo *SQLReadBranch(SqlDataReader *myReader);
	static bool SQLWriteBranch(String *branchName, String *createdDate, String *createdBy, bool tag);
	static bool SQLUpdateBranch(String *branchName, MOG_DBBranchInfo * newBranch);
//	static int GetBranchIDByName(String *name);
	static String *GetBranchNameByID(int id);

	// Platform Related
	static MOG_DBPlatformInfo *QueryPlatform(String *selectString);
	static ArrayList *QueryPlatforms(String *selectString);
	static bool SQLCreatePlatformName(String* platformName);
	static bool SQLCreatePlatformInBranch(String *platformName, String* branchName);
	static MOG_DBPlatformInfo *SQLReadPlatform(SqlDataReader *myReader);
	static bool SQLUpdatePlatform(String *platformName, MOG_DBPlatformInfo *newPlatform);
	static bool SQLRemovePlatformFromBranch(MOG_DBPlatformInfo *platformInfo, String* branchName);
	static String *GetPlatformByID(int id);
//	static int GetPlatformID(String *name);

	// Department Related
	static MOG_DBDepartmentInfo *QueryDepartment(String *selectString);
	static ArrayList *QueryDepartments(String *selectString);
	static MOG_DBDepartmentInfo *SQLCreateDepartment(MOG_DBDepartmentInfo *departmentInfo);
	static MOG_DBDepartmentInfo *SQLReadDepartment(SqlDataReader *myReader);
	static bool SQLUpdateDepartment(String *departmentName, MOG_DBDepartmentInfo *newDepartment);
	static bool SQLRemoveDepartment(MOG_DBDepartmentInfo *departmentInfo);
	static String *GetDepartmentNameByID(int id);
//	static int GetDepartmentID(String *name);

	// User Related
	static MOG_User *QueryUser(String *selectString);
	static ArrayList *QueryUsers(String *selectString);
	static MOG_User *SQLCreateUser(MOG_User *userInfo);
	static MOG_User *SQLReadUser(SqlDataReader *myReader);
	static bool SQLUpdateUser(String *userName, MOG_User *newUser);
	static bool SQLRemoveUser(MOG_User *userInfo, String *removedBy);
	static String *GetUserNameByID(int id);
	static bool IsActiveUser(int userID);
	static bool ReactivateUser(int userID, int createdBy, String *createdDate);
//	static int GetUserID(String *name);

// JohnRen - Managed C doesn't support friends!
// We still need these functions exposed to the other Database classes...Grrr!
// for the mean time, lets expose only these functions
public:
	static int GetDepartmentID(String *name);
	static int GetBranchIDByName(String *name);
	static int GetUserID(String *name);
	static int GetPlatformNameID(String *name);
};


}
}

using namespace MOG;
using namespace MOG::DATABASE;

#endif


