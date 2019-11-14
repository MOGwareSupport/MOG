//--------------------------------------------------------------------------------
//	MOG_TaskInfo.h
//	
//	
//--------------------------------------------------------------------------------
#ifndef __MOG_TASKINFO_H__
#define __MOG_TASKINFO_H__


#using <mscorlib.dll>
#using <system.dll>
#using <System.Data.dll>

#include "stdlib.h"
#include "MOG_Filename.h"

using namespace System;
using namespace System::Data;
using namespace System::Data::SqlClient;


namespace MOG {

public __gc class MOG_TaskInfo
{
private:
	int mTaskID;

	String *mName;

	String *mBranch;
	int mBranchID;
	String *mDepartment;
	int mDepartmentID;
	String *mPriority;
	String *mCreator;
	int mCreatorID;
	String *mAssignedTo;
	int mAssignedToID;
	String *mStatus;
	String *mCreationDate;
	String *mDueDate;
	String *mCompletionDate;
	int mPercentComplete;
	String *mComment;
	bool mHasAsset;
	int mImportanceRating;
	int mParentID;
	String *mParent;

public:
	MOG_TaskInfo(MOG_TaskInfo *parentTask);
	MOG_TaskInfo();

	MOG_TaskInfo *Clone();
	String *GetChangesAsComment(MOG_TaskInfo *original);

	int GetTaskID() { return mTaskID; }
	void SetTaskID(int id) { mTaskID = id; }

	int GetImportanceRating() { return mImportanceRating; }
	void SetImportanceRating(int rating) { mImportanceRating = rating; }
	void IncrementImportanceRating(int rating) { mImportanceRating += rating; }

	String *GetName(void) { return mName; }
	void SetName(String *name) { mName = name; }

	String *GetBranch(void) { return mBranch; }
	void SetBranch(String *name) { mBranch = name; }
	int GetBranchID(void) { return mBranchID; }
	void SetBranchID(int id) { mBranchID = id; }

	String *GetDepartment(void) { return mDepartment; }
	void SetDepartment(String *name) { mDepartment = name; }
	int GetDepartmentID(void) { return mDepartmentID; }
	void SetDepartmentID(int id) { mDepartmentID = id; }

	String *GetPriority(void) { return mPriority; }
	void SetPriority(String *priority) { mPriority = priority; }

	String *GetCreator(void) { return mCreator; }
	void SetCreator(String *name) { mCreator = name; }
	int GetCreatorID(void) { return mCreatorID; }
	void SetCreatorID(int id) { mCreatorID = id; }

	String *GetAssignedTo(void) { return mAssignedTo; }
	void SetAssignedTo(String *name) { mAssignedTo = name; }
	int GetAssignedToID(void) { return mAssignedToID; }
	void SetAssignedToID(int id) { mAssignedToID = id; }

	String *GetStatus(void) { return mStatus; }
	void SetStatus(String *status) { mStatus = status; }

	String *GetCreationDate(void) { return mCreationDate; }
	void SetCreationDate(String *date) { mCreationDate = date; }

	String *GetDueDate(void) { return mDueDate; }
	void SetDueDate(String *date) { mDueDate = date; }

	String *GetCompletionDate(void) { return mCompletionDate; }
	void SetCompletionDate(String *date) { mCompletionDate = date; }

	int GetPercentComplete(void) { return mPercentComplete; }
	void SetPercentComplete(int percent) { mPercentComplete = percent; }

	String *GetComment(void) { return mComment; }
	void SetComment(String *comment) { mComment = comment; }

	bool GetHasAsset(void) { return mHasAsset; }
	void SetHasAsset(bool hasAsset) { mHasAsset = hasAsset; }
	bool AddAsset(MOG_Filename *assetName);

	int GetParentID(void) { return mParentID; }
	void SetParentID(int id) { mParentID = id; }

	String *GetParent(void) { return mParent; }
	void SetParent(String *name) { mParent = name; }

};

}

using namespace MOG;

#endif


