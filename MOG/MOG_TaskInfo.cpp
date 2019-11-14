
//--------------------------------------------------------------------------------
//	MOG_TaskInfo.cpp
//	
//	
//--------------------------------------------------------------------------------
#include "stdafx.h"

#include "MOG_TaskInfo.h"
#include "MOG_Report.h"
#include "MOG_Time.h"
#include "MOG_ControllerProject.h"
#include "MOG_DBTaskAPI.h"

MOG_TaskInfo::MOG_TaskInfo(MOG_TaskInfo *parentTask)
{
	//mLink_ID = baseTask->getl;
	//mChild_ID;
	//mParent_ID;

	mName = String::Concat(S"Child:", parentTask->GetName());
	mComment = String::Concat(S"Parent Comment:", parentTask->GetComment());
	mPercentComplete = 0;

	mStatus = parentTask->GetStatus();
	mBranch = parentTask->GetBranch();
	mPriority = parentTask->GetPriority();
	mAssignedTo = parentTask->GetAssignedTo();
	mParentID = parentTask->GetParentID();
	mParent = parentTask->GetParent();

	mCreator =  parentTask->GetCreator();
	mDepartment = parentTask->GetDepartment();
	
	mHasAsset = false;
	mCreationDate = MOG_Time::GetVersionTimestamp();
	mCompletionDate = parentTask->GetCompletionDate();
	mImportanceRating = parentTask->GetImportanceRating();
}

MOG_TaskInfo::MOG_TaskInfo()
{
	mName = "";
	mComment = "";
	mPercentComplete = 0;
	mImportanceRating = 1;

	mStatus = "Unassigned";
	mBranch = "Current";
	mCreator = "Admin";
	mPriority = "Medium";
	mDepartment = "";
	mAssignedTo = "";
	mParent = "";

	mHasAsset = false;

	mCreationDate = MOG_Time::GetVersionTimestamp();
	mCompletionDate = MOG_Time::GetVersionTimestamp();
}

MOG_TaskInfo *MOG_TaskInfo::Clone()
{
	MOG_TaskInfo *copy = new MOG_TaskInfo();

	copy->mName = GetName();
	copy->mComment = GetComment();
	copy->mPercentComplete = GetPercentComplete();

	copy->mStatus = GetStatus();
	copy->mBranch = GetBranch();
	copy->mPriority = GetPriority();
	copy->mAssignedTo = GetAssignedTo();
	copy->mParentID = GetParentID();
	copy->mParent = GetParent();

	copy->mCreator = GetCreator();
	copy->mDepartment = GetDepartment();
	
	copy->mHasAsset = GetHasAsset();
	copy->mCreationDate = GetCreationDate();
	copy->mCompletionDate = GetCompletionDate();
	copy->mDueDate = GetDueDate();
	copy->mImportanceRating = GetImportanceRating();

	return copy;
}

bool MOG_TaskInfo::AddAsset(MOG_Filename *assetName)
{
	return MOG_DBTaskAPI::AddTaskAssetLink(this->mTaskID, assetName);
}

String *MOG_TaskInfo::GetChangesAsComment(MOG_TaskInfo *original)
{
	String *comment = "";
	String *editor = "Unknown";

	// Get out editors name
	if (MOG_ControllerProject::IsUser())
	{
		editor = MOG_ControllerProject::GetUser()->GetUserName();
	}

	// Get the edit time
	MOG_Time *current = new MOG_Time();
	String *editTime = current->FormatString("");

	if (String::Compare(GetName(), original->GetName(), true) != 0)
	{
		comment = String::Concat(editor, S"[", editTime, S"] Changed name from ", original->GetName(), S" to ", GetName());
	}

	if (String::Compare(GetComment(), original->GetComment(), true) != 0)
	{
		comment = String::Concat(comment, S"\n", editor, S"[", editTime, S"] Changed comment from ", original->GetComment(), S" to ", GetComment());
	}
	
	if (GetPercentComplete() != original->GetPercentComplete())
	{
		comment = String::Concat(comment, S"\n", editor, S"[", editTime, S"] Changed percent complete from ", original->GetPercentComplete(), S" to ", GetPercentComplete());
	}

	if (String::Compare(GetStatus(), original->GetStatus(), true) != 0)
	{
		comment = String::Concat(comment, S"\n", editor, S"[", editTime, S"] Changed status from ", original->GetStatus(), S" to ", GetStatus());
	}
	
	if (String::Compare(GetBranch(), original->GetBranch(), true) != 0)
	{
		comment = String::Concat(comment, S"\n", editor, S"[", editTime, S"] Changed branch from ", original->GetBranch(), S" to ", GetBranch());
	}
	
	if (String::Compare(GetPriority(), original->GetPriority(), true) != 0)
	{
		comment = String::Concat(comment, S"\n", editor, S"[", editTime, S"] Changed priority from ", original->GetPriority(), S" to ", GetPriority());
	}
		
	if (String::Compare(GetAssignedTo(), original->GetAssignedTo(), true) != 0)
	{
		comment = String::Concat(comment, S"\n", editor, S"[", editTime, S"] Changed Assigned To from ", original->GetAssignedTo(), S" to ", GetAssignedTo());
	}
	
	if (String::Compare(GetParent(), original->GetParent(), true) != 0)
	{
		comment = String::Concat(comment, S"\n", editor, S"[", editTime, S"] Changed Parent from ", original->GetParent(), S" to ", GetParent());
	}

	if (String::Compare(GetCreator(), original->GetCreator(), true) != 0)
	{
		comment = String::Concat(comment, S"\n", editor, S"[", editTime, S"] Changed creator from ", original->GetCreator(), S" to ", GetCreator());
	}
	
	if (String::Compare(GetDepartment(), original->GetDepartment(), true) != 0)
	{
		comment = String::Concat(comment, S"\n", editor, S"[", editTime, S"] Changed department from ", original->GetDepartment(), S" to ", GetDepartment());
	}
	
	if (String::Compare(GetCompletionDate(), original->GetCompletionDate(), true) != 0)
	{
		comment = String::Concat(comment, S"\n", editor, S"[", editTime, S"] Changed completion date from ", original->GetCompletionDate(), S" to ", GetCompletionDate());
	}

	if (String::Compare(GetDueDate(), original->GetDueDate(), true) != 0)
	{
		comment = String::Concat(comment, S"\n", editor, S"[", editTime, S"] Changed due date from ", original->GetDueDate(), S" to ", GetDueDate());
	}

	if (GetImportanceRating() != original->GetImportanceRating())
	{
		comment = String::Concat(comment, S"\n", editor, S"[", editTime, S"] Changed importance rating from ", original->GetImportanceRating(), S" to ", GetImportanceRating());
	}

	return comment;
}
