//--------------------------------------------------------------------------------
//	MOG_User.h
//	
//	
//--------------------------------------------------------------------------------

#ifndef __MOG_USER_H__
#define __MOG_USER_H__

#include "MOG_Filename.h"
#include "MOG_INI.h"
#include "MOG_CommandManager.h"


namespace MOG {
namespace USER {


public __gc class MOG_UserDepartment
{
public:
	MOG_UserDepartment(String* department) : mDepartmentName(department) {}

	String *mDepartmentName;
};


public __gc class MOG_User : public ArrayList {
public:
	MOG_User(void);

	// Access Functions
	void SetUserID(int value) { mUserID = value; };
	int GetUserID(void) { return mUserID; };

	void SetUserName(String *value)	{ mUserName = value; };
	String *GetUserName(void) { return mUserName; };

//	void SetUserDepartmentID(String *value) { mDepartmentID = value; };
//	String *GetUserDepartmentID(void) { return mDepartmentID; };
	void SetUserDepartment(String *userDepartment) { mDepartment = userDepartment; };
	String *GetUserDepartment(void) { return mDepartment; };

//	void SetBlessTargetID(int value) { mBlessTargetID = value; };
//	int GetBlessTargetID(void) { return mBlessTargetID; };
	void SetBlessTarget(String *blessTarget) { mBlessTarget = blessTarget; };
	String *GetBlessTarget(void) { return mBlessTarget; };

	void SetUserEmailAddress(String *userEmail) { mEmailAddress = userEmail; };
	String *GetUserEmailAddress(void) { return mEmailAddress; };

	void SetCreatedDate(String *value) { mCreatedDate = value; };
	String *GetCreatedDate(void) { return mCreatedDate; };

//	void SetCreatedByID(int value) { mCreatedByID = value; };
//	int GetCreatedByID(void) { return mCreatedByID; };
	void SetCreatedBy(String *value) { mCreatedBy = value; };
	String *GetCreatedBy(void) { return mCreatedBy; };

	void SetRemovedDate(String *value) { mRemovedDate = value; };
	String *GetRemovedDate(void) { return mRemovedDate; };

//	void SetRemovedByID(int value) { mRemovedByID = value; };
//	int GetRemovedByID(void) { return mRemovedByID; };
	void SetRemovedBy(String *value) { mRemovedBy = value; };
	String *GetRemovedBy(void) { return mRemovedBy; };

	String *GetUserToolsPath(void);
	String *GetUserPath(void);



private:
	int mUserID;
	String *mUserName;
	String *mEmailAddress;
	String *mBlessTarget;
	String *mDepartment;
	String *mCreatedDate;
	String *mCreatedBy;
	String *mRemovedDate;
	String *mRemovedBy;
};


}
}

using namespace MOG::USER;

#endif	// __MOG_USER_H__
