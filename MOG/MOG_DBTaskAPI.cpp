//--------------------------------------------------------------------------------
//	MOG_DBTasks.cpp
//	
//	
//--------------------------------------------------------------------------------
#include "stdafx.h"


#include "MOG_Report.h"
#include "MOG_Time.h"
#include "MOG_ControllerSystem.h"
#include "MOG_DBAPI.h"
#include "MOG_DBProjectAPI.h"
#include "MOG_DBAssetAPI.h"

#include "MOG_DBTaskAPI.h"



MOG_TaskInfo *MOG_DBTaskAPI::QueryTask(String *selectString)
{
	SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());
	MOG_TaskInfo *task = NULL;

	try
	{
		SqlDataReader *myReader = MOG_DBAPI::GetNewDataReader(selectString, myConnection);
		while(myReader->Read())
		{
			task = SQLReadTask(myReader);
			break;
		}
		myReader->Close();
		
	}
	catch(Exception *e)
	{
		String *message = String::Concat(	S"Could not get records from SQL database!\n",
											S"MOG SERVER: ", MOG_ControllerSystem::GetServerComputerName());
		MOG_Report::ReportMessage(message, e->Message, e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
		return NULL;
	}
	__finally
	{
		myConnection->Close();
	}

	return task;
}


ArrayList *MOG_DBTaskAPI::QueryTasks(String *selectString)
{
	SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());
	ArrayList *tasks = NULL;

	try
	{
		tasks = new ArrayList();

		SqlDataReader *myReader = MOG_DBAPI::GetNewDataReader(selectString, myConnection);
		while(myReader->Read())
		{
			tasks->Add(SQLReadTask(myReader));
		}
		myReader->Close();
	}
	catch(Exception *e)
	{
		String *message = String::Concat(	S"Could not get records from SQL database!\n",
											S"MOG SERVER: ", MOG_ControllerSystem::GetServerComputerName());
		MOG_Report::ReportMessage(message, e->Message, e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
		return NULL;
	}
	__finally
	{
		myConnection->Close();
	}

	return tasks;
}


MOG_TaskInfo *MOG_DBTaskAPI::SQLReadTask(SqlDataReader *myReader)
{
	MOG_TaskInfo *task = new MOG_TaskInfo();

	task->SetTaskID(myReader->GetInt32(myReader->GetOrdinal(S"ID")));
	task->SetName(myReader->GetString(myReader->GetOrdinal(S"TaskName"))->Trim());
	task->SetComment(myReader->GetString(myReader->GetOrdinal(S"Comment")));
	task->SetPercentComplete(myReader->GetInt32(myReader->GetOrdinal(S"PercentComplete")));

	task->SetBranch(MOG_DBAPI::SafeStringRead(myReader , S"BranchName"));
	
	task->SetDepartment(MOG_DBAPI::SafeStringRead(myReader, S"DepartmentName"));
	task->SetPriority(MOG_DBAPI::SafeStringRead(myReader, S"Priority")->Trim());
	task->SetCreator(MOG_DBAPI::SafeStringRead(myReader, S"Creator"));

	task->SetAssignedTo(MOG_DBAPI::SafeStringRead(myReader , S"AssignedTo"));
	
	task->SetStatus(myReader->GetString(myReader->GetOrdinal(S"Status"))->Trim());

	task->SetHasAsset(String::Compare(MOG_DBAPI::SafeStringRead(myReader , S"HasAsset"), S"true") == 0);
	task->SetParent(MOG_DBAPI::SafeStringRead(myReader , S"Parent"));
	task->SetParentID(MOG_DBAPI::SafeIntRead(myReader , S"ParentID"));

	task->SetCreationDate(MOG_DBAPI::SafeStringRead(myReader, S"CreatedDate"));
	task->SetCompletionDate(MOG_DBAPI::SafeStringRead(myReader, S"CompletedDate"));
	task->SetDueDate(MOG_DBAPI::SafeStringRead(myReader, S"DueDate"));

	return task;
}


bool MOG_DBTaskAPI::SQLWriteTask(MOG_TaskInfo *taskInfo)
{
	String *updateCmd = String::Concat(S"UPDATE ", MOG_ControllerSystem::GetDB()->GetTasksTable(), S" SET");

	// Confirm all IDs match
	SQLSyncTaskIDs(taskInfo);

	// Variables
	updateCmd = String::Concat(updateCmd, S" TaskName='", MOG_DBAPI::FixSQLParameterString(taskInfo->GetName()), S"',");
	updateCmd = String::Concat(updateCmd, S" BranchID='", taskInfo->GetBranchID().ToString(), S"',");
	updateCmd = String::Concat(updateCmd, S" DepartmentID='", taskInfo->GetDepartmentID().ToString(), S"',");
	updateCmd = String::Concat(updateCmd, S" Priority='", MOG_DBAPI::FixSQLParameterString(taskInfo->GetPriority()), S"',");
	updateCmd = String::Concat(updateCmd, S" CreatorID='", taskInfo->GetCreatorID().ToString(), S"',");
	updateCmd = String::Concat(updateCmd, S" AssignedToID='", taskInfo->GetAssignedToID().ToString(), S"',");
	updateCmd = String::Concat(updateCmd, S" Status='", MOG_DBAPI::FixSQLParameterString(taskInfo->GetStatus()), S"',");
	updateCmd = String::Concat(updateCmd, S" Comment='", MOG_DBAPI::FixSQLParameterString(taskInfo->GetComment()), S"',");
	updateCmd = String::Concat(updateCmd, S" PercentComplete='", taskInfo->GetPercentComplete().ToString(), S"',");
	updateCmd = String::Concat(updateCmd, S" CreatedDate='", taskInfo->GetCreationDate(), S"',");
	updateCmd = String::Concat(updateCmd, S" CompletedDate='", taskInfo->GetCompletionDate(), S"',");
	updateCmd = String::Concat(updateCmd, S" DueDate='", taskInfo->GetDueDate(), S"'");

	// Where clause
	updateCmd = String::Concat(updateCmd, S" WHERE (id='", taskInfo->GetTaskID().ToString(), S"')");
	
	return MOG_DBAPI::RunNonQuery(updateCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
}


MOG_TaskInfo *MOG_DBTaskAPI::SQLCreateTask(MOG_TaskInfo *taskInfo)
{
	// Make sure to sync all the possible IDs in this task
	SQLSyncTaskIDs(taskInfo);

	// Build a SQL Insert statement string for all the input-form
	String *insertCmd = String::Concat(S"INSERT INTO ", MOG_ControllerSystem::GetDB()->GetTasksTable(),
		S" (TaskName, DepartmentID, AssignedToID, Comment, PercentComplete, CreatorID, CreatedDate, DueDate, CompletedDate, Status, Priority, BranchID) VALUES ",
		S"('",MOG_DBAPI::FixSQLParameterString(taskInfo->GetName()), S"', ",__box(taskInfo->GetDepartmentID()), S",",__box(taskInfo->GetAssignedToID()), S",'",MOG_DBAPI::FixSQLParameterString(taskInfo->GetComment()), S"',",__box(taskInfo->GetPercentComplete()), S",",__box(taskInfo->GetCreatorID()), S",'",taskInfo->GetCreationDate(), S"', '", taskInfo->GetDueDate(), S"','",taskInfo->GetCompletionDate(), S"','",MOG_DBAPI::FixSQLParameterString(taskInfo->GetStatus()), S"', '",MOG_DBAPI::FixSQLParameterString(taskInfo->GetPriority()), S"',",__box(taskInfo->GetBranchID()), S")");

		MOG_DBAPI::RunNonQuery(insertCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());

	taskInfo->SetTaskID(GetTaskID(taskInfo->GetName(), taskInfo->GetCreationDate()));

	return taskInfo;
}


bool MOG_DBTaskAPI::SQLSyncTaskIDs(MOG_TaskInfo *taskInfo)
{
	taskInfo->SetBranchID(MOG_DBProjectAPI::GetBranchIDByName(taskInfo->GetBranch()));
	taskInfo->SetDepartmentID(MOG_DBProjectAPI::GetDepartmentID(taskInfo->GetDepartment()));
	taskInfo->SetCreatorID(MOG_DBProjectAPI::GetUserID(taskInfo->GetCreator()));
	taskInfo->SetAssignedToID(MOG_DBProjectAPI::GetUserID(taskInfo->GetAssignedTo()));

	return true;
}


ArrayList *MOG_DBTaskAPI::GetPriorites()
{
	String *selectString = String::Concat(S"SELECT * from Priority");
	String *ordinal = "name";

	return MOG_DBAPI::QueryStrings(selectString, ordinal);
}


ArrayList *MOG_DBTaskAPI::GetDepartmentTasks(String *departmentName)
{
	String *selectString = String::Concat(
		S"SELECT Tasks.ID, Tasks.TaskName, Departments.DepartmentName, User1.UserName AS AssignedTo, Tasks.Comment, Tasks.PercentComplete, ",
			S"Users2.UserName AS Creator, Tasks.CreatedDate, Tasks.DueDate, Tasks.CompletedDate, Tasks.Status, Tasks.Priority, Branches.BranchName, ",
			S"CASE WHEN TAL.TaskID IS NULL THEN 'false' ELSE 'true' END AS HasAsset ",
		S"FROM ", MOG_ControllerSystem::GetDB()->GetTasksTable(), S" Tasks LEFT OUTER JOIN ",
			MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches ON Tasks.BranchID = Branches.ID LEFT OUTER JOIN ",
			MOG_ControllerSystem::GetDB()->GetDepartmentsTable(), S" Departments ON Tasks.DepartmentID = Departments.ID LEFT OUTER JOIN ",
			MOG_ControllerSystem::GetDB()->GetUsersTable(), S" Users2 ON Tasks.CreatorID = Users2.ID LEFT OUTER JOIN ",
			MOG_ControllerSystem::GetDB()->GetUsersTable(), S" User1 ON Tasks.AssignedToID = User1.ID LEFT OUTER JOIN ",
			S"(SELECT DISTINCT TaskID ",
				S"FROM ", MOG_ControllerSystem::GetDB()->GetTaskAssetLinksTable(), S") TAL ON Tasks.ID = TAL.TaskID ",
		S"WHERE (Tasks.AssignedToID = 0) AND ",
			S"(Departments.DepartmentName = '", MOG_DBAPI::FixSQLParameterString(departmentName) ,S"')" );

	return QueryTasks(selectString);
}


String *MOG_DBTaskAPI::GetTaskResponsibleParty(int id)
{
	String *selectString = String::Concat(S"SELECT * from Users WHERE id='",Convert::ToString(id), S"'");
	String *ordinal = "name";

	return MOG_DBAPI::QueryString(selectString, ordinal);
}


int MOG_DBTaskAPI::GetTaskResponsiblePartyID(String *name)
{
	String *selectString = String::Concat(S"SELECT * from Users WHERE name='",MOG_DBAPI::FixSQLParameterString(name), S"'");
	String *ordinal = "id";

	return MOG_DBAPI::QueryInt(selectString, ordinal);
}

String *MOG_DBTaskAPI::GetTaskProject(int id)
{
	String *selectString = String::Concat(S"SELECT * from Projects WHERE id='",Convert::ToString(id), S"'");
	String *ordinal = "name";

	return MOG_DBAPI::QueryString(selectString, ordinal);
}


int MOG_DBTaskAPI::SetParentTaskID(MOG_TaskInfo *taskInfo, int parentTaskID)
{
	// Build a SQL Insert statement string for all the input-form
	String *insertCmd = String::Concat(S"INSERT INTO ",
		MOG_ControllerSystem::GetDB()->GetTaskLinksTable(), S" (ParentID, ChildID) VALUES (", __box(parentTaskID), S", ",__box(taskInfo->GetTaskID()), S")");

	return MOG_DBAPI::RunNonQuery(insertCmd,MOG_ControllerSystem::GetDB()->GetConnectionString());
}

ArrayList *MOG_DBTaskAPI::GetUserTasks(String *userName)
{
	String *selectString = String::Concat(
		S"SELECT Tasks.ID, Tasks.TaskName, Departments.DepartmentName, User1.UserName AS AssignedTo, Tasks.Comment, Tasks.PercentComplete, "
			S"Users2.UserName AS Creator, Tasks.CreatedDate, Tasks.DueDate, Tasks.CompletedDate, Tasks.Status, Tasks.Priority, Branches.BranchName, ",
			S"ParentTasks.TaskName AS Parent, ParentTasks.ID AS ParentID, CASE WHEN TAL.TaskID IS NULL THEN 'false' ELSE 'true' END AS HasAsset ",
		S"FROM ", MOG_ControllerSystem::GetDB()->GetTasksTable(), S" Tasks LEFT OUTER JOIN ",
			MOG_ControllerSystem::GetDB()->GetTasksTable(), S" ParentTasks INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetTaskLinksTable(), S" TaskLinks ON ParentTasks.ID = TaskLinks.ParentID INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetUsersTable(), S" ParentUser ON ParentTasks.AssignedToID = ParentUser.ID ON Tasks.ID = TaskLinks.ChildID LEFT OUTER JOIN ",
			MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches ON Tasks.BranchID = Branches.ID LEFT OUTER JOIN ",
			MOG_ControllerSystem::GetDB()->GetDepartmentsTable(), S" Departments ON Tasks.DepartmentID = Departments.ID LEFT OUTER JOIN ",
			MOG_ControllerSystem::GetDB()->GetUsersTable(), S" Users2 ON Tasks.CreatorID = Users2.ID LEFT OUTER JOIN ",
			MOG_ControllerSystem::GetDB()->GetUsersTable(), S" User1 ON Tasks.AssignedToID = User1.ID LEFT OUTER JOIN ",
			S"(SELECT DISTINCT TaskID ",
				S"FROM ", MOG_ControllerSystem::GetDB()->GetTaskAssetLinksTable(), S") TAL ON Tasks.ID = TAL.TaskID ",
		S"WHERE (User1.UserName = '", MOG_DBAPI::FixSQLParameterString(userName), S"') AND (TaskLinks.ChildID IS NULL) OR ",
			S"(User1.UserName = '", MOG_DBAPI::FixSQLParameterString(userName), S"') AND (ParentUser.UserName <> '", MOG_DBAPI::FixSQLParameterString(userName), S"')" );

	return QueryTasks(selectString);
}

ArrayList *MOG_DBTaskAPI::GetAllUserTasks(String *userName)
{
	String *selectString = String::Concat(
		S"SELECT Tasks.ID, Tasks.TaskName, Departments.DepartmentName, User1.UserName AS AssignedTo, Tasks.Comment, Tasks.PercentComplete, ",
			S"Users2.UserName AS Creator, Tasks.CreatedDate, Tasks.DueDate, Tasks.CompletedDate, Tasks.Status, Tasks.Priority, Branches.BranchName, ",
			S"TasksParent.TaskName AS Parent, TasksParent.ID AS ParentID, CASE WHEN TAL.TaskID IS NULL THEN 'false' ELSE 'true' END AS HasAsset ",
		S"FROM ", MOG_ControllerSystem::GetDB()->GetTasksTable(), S" Tasks LEFT OUTER JOIN ",
			MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches ON Tasks.BranchID = Branches.ID LEFT OUTER JOIN ",
			MOG_ControllerSystem::GetDB()->GetTasksTable(), S" TasksParent INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetTaskLinksTable(), S" TaskLinks ON TasksParent.ID = TaskLinks.ParentID ON Tasks.ID = TaskLinks.ChildID LEFT OUTER JOIN ",
			MOG_ControllerSystem::GetDB()->GetDepartmentsTable(), S" Departments ON Tasks.DepartmentID = Departments.ID LEFT OUTER JOIN ",
			MOG_ControllerSystem::GetDB()->GetUsersTable(), S" Users2 ON Tasks.CreatorID = Users2.ID LEFT OUTER JOIN ",
			MOG_ControllerSystem::GetDB()->GetUsersTable(), S" User1 ON Tasks.AssignedToID = User1.ID LEFT OUTER JOIN ",
			S"(SELECT DISTINCT TaskID ",
				S"FROM ", MOG_ControllerSystem::GetDB()->GetTaskAssetLinksTable(), S") TAL ON Tasks.ID = TAL.TaskID ",
		S"WHERE (User1.UserName = '", MOG_DBAPI::FixSQLParameterString(userName), S"')" );

	return QueryTasks(selectString);
}

MOG_TaskInfo *MOG_DBTaskAPI::GetTask(int taskID)
{
	String *selectString = String::Concat(  
		S"SELECT Tasks.ID, Tasks.TaskName, Departments.DepartmentName, User1.UserName AS AssignedTo, Tasks.Comment, Tasks.PercentComplete, ",
			S"Users2.UserName AS Creator, Tasks.CreatedDate, Tasks.DueDate, Tasks.CompletedDate, Tasks.Status, Tasks.Priority, Branches.BranchName, ",
			S"TasksParent.TaskName AS Parent, TasksParent.ID AS ParentID, CASE WHEN TAL.TaskID IS NULL THEN 'false' ELSE 'true' END AS HasAsset ",
		S"FROM ", MOG_ControllerSystem::GetDB()->GetTasksTable(), S" Tasks LEFT OUTER JOIN ",
			MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches ON Tasks.BranchID = Branches.ID LEFT OUTER JOIN ",
			MOG_ControllerSystem::GetDB()->GetTasksTable(), S" TasksParent INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetTaskLinksTable(), S" TaskLinks ON TasksParent.ID = TaskLinks.ParentID ON Tasks.ID = TaskLinks.ChildID LEFT OUTER JOIN ",
			MOG_ControllerSystem::GetDB()->GetDepartmentsTable(), S" Departments ON Tasks.DepartmentID = Departments.ID LEFT OUTER JOIN ",
			MOG_ControllerSystem::GetDB()->GetUsersTable(), S" Users2 ON Tasks.CreatorID = Users2.ID LEFT OUTER JOIN ",
			MOG_ControllerSystem::GetDB()->GetUsersTable(), S" User1 ON Tasks.AssignedToID = User1.ID LEFT OUTER JOIN ",
			S"(SELECT DISTINCT TaskID ",
				S"FROM ", MOG_ControllerSystem::GetDB()->GetTaskAssetLinksTable(), S") TAL ON Tasks.ID = TAL.TaskID ",
		S"WHERE (Tasks.ID = ", Convert::ToString(taskID), S")" );

	return QueryTask(selectString);
}

int MOG_DBTaskAPI::GetTaskID(String *taskName, String *createdDate)
{
	String *selectString = String::Concat(  S"SELECT    ID",
											S" FROM     ", MOG_ControllerSystem::GetDB()->GetTasksTable(),
											S" WHERE    (TaskName = '", MOG_DBAPI::FixSQLParameterString(taskName), S"') AND (CreatedDate = '", createdDate, S"')");
	String *ordinal = "ID";

	return MOG_DBAPI::QueryInt(selectString, ordinal);
}


ArrayList *MOG_DBTaskAPI::GetChildrenTasks(int taskID)
{
	String *selectString = String::Concat(
		S"SELECT Tasks.ID, Tasks.TaskName, Departments.DepartmentName, User1.UserName AS AssignedTo, Tasks.Comment, Tasks.PercentComplete, ",
			S"Users2.UserName AS Creator, Tasks.CreatedDate, Tasks.DueDate, Tasks.CompletedDate, Tasks.Status, Tasks.Priority, Branches.BranchName, ",
			S"TasksParent.TaskName AS Parent, TasksParent.ID AS ParentID, CASE WHEN TAL.TaskID IS NULL THEN 'false' ELSE 'true' END AS HasAsset ",
		S"FROM ", MOG_ControllerSystem::GetDB()->GetTasksTable(), S" TasksParent INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetTaskLinksTable(), S" TaskLinks ON TasksParent.ID = TaskLinks.ParentID RIGHT OUTER JOIN ",
			MOG_ControllerSystem::GetDB()->GetTasksTable(), S" Tasks LEFT OUTER JOIN ",
			MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches ON Tasks.BranchID = Branches.ID ON TaskLinks.ChildID = Tasks.ID LEFT OUTER JOIN ",
			MOG_ControllerSystem::GetDB()->GetDepartmentsTable(), S" Departments ON Tasks.DepartmentID = Departments.ID LEFT OUTER JOIN ",
			MOG_ControllerSystem::GetDB()->GetUsersTable(), S" Users2 ON Tasks.CreatorID = Users2.ID LEFT OUTER JOIN ",
			MOG_ControllerSystem::GetDB()->GetUsersTable(), S" User1 ON Tasks.AssignedToID = User1.ID LEFT OUTER JOIN ",
			S"(SELECT DISTINCT TaskID ",
				S"FROM ", MOG_ControllerSystem::GetDB()->GetTaskAssetLinksTable(), S") TAL ON Tasks.ID = TAL.TaskID ",
		S"WHERE (TaskLinks.ParentID = ", Convert::ToString(taskID), S")" );

	return QueryTasks(selectString);
}


MOG_TaskInfo *MOG_DBTaskAPI::CreateTask(MOG_TaskInfo *taskInfo)
{
	return SQLCreateTask(taskInfo);
}


bool MOG_DBTaskAPI::UpdateTask(MOG_TaskInfo *taskInfo)
{
	return SQLWriteTask(taskInfo);
}

bool MOG_DBTaskAPI::RemoveTask(int taskID)
{
	ArrayList *listOfCommands = new ArrayList();

	listOfCommands->Add(String::Concat( S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetTasksTable(),
											 S" WHERE (ID = ", Convert::ToString(taskID), S")"));

	listOfCommands->Add(String::Concat( S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetTaskLinksTable(),
											 S" WHERE (ParentID = ", Convert::ToString(taskID), S")"));
	
	listOfCommands->Add(String::Concat( S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetTaskLinksTable(),
											 S" WHERE (ChildID = ", Convert::ToString(taskID), S")"));
	
	listOfCommands->Add(String::Concat( S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetTaskAssetLinksTable(),
											 S" WHERE (TaskID = ", Convert::ToString(taskID), S")"));
	
	return MOG_DBAPI::ExecuteTransaction(listOfCommands);
}

bool MOG_DBTaskAPI::AddTaskAssetLink(int task_ID, MOG_Filename *assetName)
{
	String *selectCmd;
	String *insertCmd;
	bool added = false;

	int assetNameID = MOG_DBAssetAPI::GetAssetNameID(assetName);
	String *assetFullName = S"";

	// if it doesnt exist in the asset name table just store the full name
	if( assetNameID == 0 )
	{
		assetFullName = assetName->GetAssetFullName();
	}

	// make sure it doesnt exist
	selectCmd = String::Concat( S"SELECT ID ",
								S"FROM ", MOG_ControllerSystem::GetDB()->GetTaskAssetLinksTable(), S" ",
								S"WHERE TaskID=", Convert::ToString(task_ID),
								S" AND AssetID=", Convert::ToString(assetNameID),
								S" AND AssetFullName='", MOG_DBAPI::FixSQLParameterString(assetFullName), S"'" );

	if( !MOG_DBAPI::QueryExists(selectCmd) )
	{
		// add task asset link record
		insertCmd = String::Concat(S"INSERT INTO ", MOG_ControllerSystem::GetDB()->GetTaskAssetLinksTable(),
								   S" ( TaskID, AssetFullName, AssetID ) VALUES ",
								   S" (",__box(task_ID), S", '", MOG_DBAPI::FixSQLParameterString(assetFullName), S"', ",__box(assetNameID), S")" );

		return MOG_DBAPI::RunNonQuery(insertCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
	}

	return added;
}

bool MOG_DBTaskAPI::RemoveTaskAssetLink(int task_ID, MOG_Filename *assetName)
{
	String *selectCmd;
	String *deleteCmd;
	bool removed = false;

	int assetNameID = MOG_DBAssetAPI::GetAssetNameID(assetName);
	String *assetFullName = S"";

	// if it doesnt exist in the asset name table just store the full name
	if( assetNameID == 0 )
	{
		assetFullName = assetName->GetAssetFullName();
	}

	// make sure it doesnt exist
	selectCmd = String::Concat( S"SELECT ID ",
								S"FROM ", MOG_ControllerSystem::GetDB()->GetTaskAssetLinksTable(), S" ",
								S"WHERE TaskID=", Convert::ToString(task_ID),
								S" AND AssetID=", Convert::ToString(assetNameID),
								S" AND AssetFullName='", MOG_DBAPI::FixSQLParameterString(assetFullName), S"'" );

	int taskAssetLinksID = MOG_DBAPI::QueryInt(selectCmd, S"ID");
	if( taskAssetLinksID != 0 )
	{
		// remove task Asset link record
		deleteCmd = String::Concat( S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetTaskAssetLinksTable(),
									S" WHERE ID = ", Convert::ToString(taskAssetLinksID) );

		return MOG_DBAPI::RunNonQuery(deleteCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
	}
	return removed;
}

ArrayList *MOG_DBTaskAPI::GetTaskAssetLinks(int task_ID)
{
	String *selectString = String::Concat(
		S"SELECT TaskAssetLinks.AssetID, TaskAssetLinks.AssetFullName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, ",
			S"AssetClassifications.FullTreeName, '' AS Version ",
		S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames ON AssetClassifications.ID = AssetNames.AssetClassificationID RIGHT OUTER JOIN ",
			MOG_ControllerSystem::GetDB()->GetTaskAssetLinksTable(), S" TaskAssetLinks ON AssetNames.ID = TaskAssetLinks.AssetID ",
		S"WHERE (TaskAssetLinks.TaskID = ", Convert::ToString(task_ID), S")" );

	return QueryTaskAssets(selectString);
}

ArrayList *MOG_DBTaskAPI::QueryTaskAssets(String *selectString)
{
	SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());
	ArrayList *taskAssets = new ArrayList();

	SqlDataReader *myReader = NULL;
	try
	{
		myReader = MOG_DBAPI::GetNewDataReader(selectString, myConnection);
		while(myReader->Read())
		{
			int assetID = MOG_DBAPI::SafeIntRead(myReader, S"AssetID");
			MOG_Filename *newFilename = NULL;
			if( assetID == 0 )
			{
				newFilename = new MOG_Filename(MOG_DBAPI::SafeStringRead(myReader, S"AssetFullName"));
			}
			else
			{
				newFilename = MOG_DBAssetAPI::SQLReadAsset(myReader);
			}
			taskAssets->Add(newFilename);
		}
	}
	catch(Exception *e)
	{
		String *message = String::Concat(	S"Could not get records from SQL database!\n",
											S"MOG SERVER: ", MOG_ControllerSystem::GetServerComputerName());
		MOG_Report::ReportMessage(message, e->Message, e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
	}
	__finally
	{
		if( myReader != NULL )
		{
			myReader->Close();
		}
		myConnection->Close();
	}

	return taskAssets;
}