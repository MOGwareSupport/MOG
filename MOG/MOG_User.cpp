//--------------------------------------------------------------------------------
//	MOG_User.cpp
//	
//	
//--------------------------------------------------------------------------------
#include "StdAfx.h"

#include "MOG_Define.h"
#include "MOG_ControllerProject.h"

#include "MOG_User.h"


MOG_User::MOG_User(void)
{
	mUserID = 0;
	mUserName = "";
	mDepartment = "";
	mBlessTarget = "MasterData";
	mEmailAddress = "none";
	mCreatedDate = "";
	mCreatedBy = "";
	mRemovedDate = "";
	mRemovedBy = "";
}


String *MOG_User::GetUserToolsPath()
{
	if (MOG_ControllerProject::GetProject())
	{
		return String::Concat(MOG_ControllerProject::GetProject()->GetProjectUsersPath(), S"\\", GetUserName(), S"\\Tools");
	}

	MOG_Report::ReportMessage(S"GetUserToolPath Error", S"You are not currently logged into a project.  The UserToolsPath is unavailable.", Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);

	return S"";
}


String *MOG_User::GetUserPath()
{
	if (MOG_ControllerProject::GetProject())
	{
		return String::Concat(MOG_ControllerProject::GetProject()->GetProjectUsersPath(), S"\\", GetUserName());
	}

	MOG_Report::ReportMessage(S"GetUserPath Error", S"You are not currently logged into a project.  The UserPath is unavailable.", Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);

	return S"";
}
