//--------------------------------------------------------------------------------
//	MOG_DBEventAPI.cpp
//	
//	
//--------------------------------------------------------------------------------
#include "stdafx.h"

#include "MOG_Report.h"
#include "MOG_Time.h"
#include "MOG_ControllerSystem.h"
#include "MOG_ControllerProject.h"
#include "MOG_DBEventAPI.h"


ArrayList *MOG_DBEventAPI::GetEvents(String *minTimeStamp, String *maxTimeStamp, String *projectName, String *branchName)
{
	String *selectString = S"SELECT * FROM [MOG.Events]";

	if( minTimeStamp != NULL || maxTimeStamp != NULL || projectName != NULL || branchName != NULL )
	{
		String *and = S"";

		selectString = String::Concat(selectString, S" WHERE ");

		if( minTimeStamp != NULL )
		{
			selectString = String::Concat(selectString, and, S"(TimeStamp >= '", minTimeStamp, S"')");
			and = S" AND ";
		}

		if( maxTimeStamp != NULL )
		{
			selectString = String::Concat(selectString, and, S"(TimeStamp <= '", maxTimeStamp, S"')");
			and = S" AND ";
		}

		if( projectName != NULL )
		{
			selectString = String::Concat(selectString, and, S"(ProjectName = '", MOG_DBAPI::FixSQLParameterString(projectName), S"')");
			and = S" AND ";
		}

		if( branchName != NULL )
		{
			selectString = String::Concat(selectString, and, S"(BranchName = '", MOG_DBAPI::FixSQLParameterString(branchName), S"')");
			and = S" AND ";
		}
	}

	// Add the ORDER BY
	selectString = String::Concat(selectString, S"ORDER BY [MOG.Events].Timestamp");

	return QueryEvents(selectString);
}

bool MOG_DBEventAPI::AddEvent(String *type, String *timeStamp, String *title, String *description, String *stackTrace, String *eventID, String *userName, String *computerName, bool allowDuplicates)
{
	MOG_DBEventInfo *eventInfo = new MOG_DBEventInfo();

	// Abort if the events table does not extist
	if (MOG_DBAPI::TableExists("MOG.Events") == false)
	{
		return false;
	}

	if( timeStamp == NULL )
	{
		timeStamp = MOG_Time::GetVersionTimestamp();
	}

	eventInfo->mType = type;
	eventInfo->mTimeStamp = timeStamp;
	eventInfo->mTitle = title;
	eventInfo->mDescription = description;
	eventInfo->mStackTrace = stackTrace;
	eventInfo->mEventID = eventID;
	eventInfo->mUserName = userName;
	eventInfo->mComputerName = computerName;
	eventInfo->mProjectName = MOG_ControllerProject::GetProjectName();
	eventInfo->mBranchName = MOG_ControllerProject::GetBranchName();

	return (SQLCreateEvent(eventInfo, allowDuplicates) != NULL);
}

bool MOG_DBEventAPI::RemoveEvent(MOG_DBEventInfo *eventInfo)
{
	return SQLRemoveEvent(eventInfo);
}

bool MOG_DBEventAPI::RemoveEvents(ArrayList *events)
{
	if( events == NULL ||
		events->Count == 0)
	{
		return false;
	}

	StringBuilder* deleteCmd = new StringBuilder(S"DELETE FROM [MOG.Events] WHERE ID IN (");

	for( int i = 0; i < events->Count; i++ )
	{
		MOG_DBEventInfo *event = NULL;

		event = dynamic_cast<MOG_DBEventInfo *>(events->Item[i]);
		if (event)
		{
			if (i > 0)
			{
				deleteCmd->Append(S",");
			}

			deleteCmd->Append(Convert::ToString(event->mID));
		}
	}

	deleteCmd->Append(S")");

	return MOG_DBAPI::RunNonQuery(deleteCmd->ToString(), MOG_ControllerSystem::GetDB()->GetConnectionString());
}


MOG_DBEventInfo *MOG_DBEventAPI::QueryEvent(String *selectString)
{
	SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());
	MOG_DBEventInfo *eventInfo = NULL;
	SqlDataReader *myReader = MOG_DBAPI::GetNewDataReader(selectString, myConnection);
	try
	{
		while(myReader->Read())
		{
			eventInfo = SQLReadEvent(myReader);
			break;
		}
	}
	catch(Exception *e)
	{
		String *message = String::Concat(	S"Could not get records from SQL database!\n",
											S"MOG SERVER: ", MOG_ControllerSystem::GetServerComputerName());
		MOG_Report::ReportMessage(message, e->Message, e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
		return NULL;
	}
		__finally
	{	//Clean Up
		myReader->Close();
		myConnection->Close();
	}

	return eventInfo;
}

ArrayList *MOG_DBEventAPI::QueryEvents(String *selectString)
{
	SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());
	ArrayList *events = NULL;

	events = new ArrayList();

	SqlDataReader *myReader = MOG_DBAPI::GetNewDataReader(selectString, myConnection);

	try
	{
		while(myReader->Read())
		{
			events->Add(SQLReadEvent(myReader));
		}
	}
	catch(Exception *e)
	{
		String *message = String::Concat(	S"Could not get records from SQL database!\n",
											S"MOG SERVER: ", MOG_ControllerSystem::GetServerComputerName());
		MOG_Report::ReportMessage(message, e->Message, e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
		return NULL;
	}
		__finally
	{	//Clean Up
		myReader->Close();
		myConnection->Close();
	}

	return events;
}

MOG_DBEventInfo *MOG_DBEventAPI::SQLCreateEvent(MOG_DBEventInfo *eventInfo, bool allowDuplicates)
{
	// return early if invalid struct
	if( eventInfo == NULL )
	{
		return NULL;
	}

	bool insertNew = true;
	if( !allowDuplicates )
	{
		String *replaceVal = "'";
		String *description = eventInfo->mDescription->Replace(replaceVal, "\"");

		// check if a record already exists
		String *selectString = String::Concat(
			S"SELECT ID FROM [MOG.Events] ",
			S"WHERE (Type = '", MOG_DBAPI::TruncateIfNecessary(eventInfo->mType, 50, false), S"') AND ",
				S"(Description LIKE '", MOG_DBAPI::FixSQLParameterString(MOG_DBAPI::TruncateIfNecessary(description, 1000, false)), S"') AND ",
				S"(Title = '", MOG_DBAPI::FixSQLParameterString(MOG_DBAPI::TruncateIfNecessary(eventInfo->mTitle, 100, false)), S"') AND ",
				S"(StackTrace = '", MOG_DBAPI::FixSQLParameterString(MOG_DBAPI::TruncateIfNecessary(eventInfo->mStackTrace, 1000, false)), S"') AND ",
				S"(UserName = '", MOG_DBAPI::FixSQLParameterString(eventInfo->mUserName), S"') AND ",
				S"(ComputerName = '", MOG_DBAPI::FixSQLParameterString(eventInfo->mComputerName), S"') AND ",
				S"(ProjectName = '", MOG_DBAPI::FixSQLParameterString(eventInfo->mProjectName), S"') AND ",
				S"(BranchName = '", MOG_DBAPI::FixSQLParameterString(eventInfo->mBranchName), S"')" );

		if( MOG_DBAPI::QueryExists(selectString) )
		{
			insertNew = false;
		}
	}

	if( insertNew )
	{
		// Build an SQL Insert statement string for all the input-form
		String *insertCmd = String::Concat( S"INSERT INTO [MOG.Events] "
											, S" ( Type, TimeStamp, Title, Description, StackTrace, EventID, UserName, ComputerName, ProjectName, BranchName ) VALUES"
											, S" ('"
											, MOG_DBAPI::TruncateIfNecessary(eventInfo->mType, 50, false)
											, S"','"
											, eventInfo->mTimeStamp 
											, S"','"
											, MOG_DBAPI::FixSQLParameterString(MOG_DBAPI::TruncateIfNecessary(eventInfo->mTitle,100,false)) 
											, S"', '"
											, MOG_DBAPI::FixSQLParameterString(MOG_DBAPI::TruncateIfNecessary(eventInfo->mDescription,1000, false))
											, S"', '"
											, MOG_DBAPI::FixSQLParameterString(MOG_DBAPI::TruncateIfNecessary(eventInfo->mStackTrace,1000,false))
											, S"', '"
											, eventInfo->mEventID
											, S"', '"
											, MOG_DBAPI::FixSQLParameterString(eventInfo->mUserName)
											, S"', '"
											, MOG_DBAPI::FixSQLParameterString(eventInfo->mComputerName)
											, S"', '"
											, MOG_DBAPI::FixSQLParameterString(eventInfo->mProjectName)
											, S"', '"
											, MOG_DBAPI::FixSQLParameterString(eventInfo->mBranchName)
											, S"')" );

		MOG_DBAPI::RunNonQuery(insertCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
	}
	else
	{
		// update
		String *updateCmd = String::Concat(
			S"UPDATE [MOG.Events] ",
			S"SET RepeatCount = RepeatCount + 1, ",
			S"TimeStamp = '", eventInfo->mTimeStamp, S"' ",
			S"WHERE (Type = '", MOG_DBAPI::TruncateIfNecessary(eventInfo->mType, 50, false), S"') AND ",
				S"(Description LIKE '", MOG_DBAPI::FixSQLParameterString(MOG_DBAPI::TruncateIfNecessary(eventInfo->mDescription, 1000, false)), S"') AND ",
				S"(EventID = '", eventInfo->mEventID, S"') AND ",
				S"(Title = '", MOG_DBAPI::FixSQLParameterString(MOG_DBAPI::TruncateIfNecessary(eventInfo->mTitle,100,false)), S"') AND ",
				S"(StackTrace = '", MOG_DBAPI::FixSQLParameterString(MOG_DBAPI::TruncateIfNecessary(eventInfo->mStackTrace, 1000, false)), S"') AND ",
				S"(UserName = '", MOG_DBAPI::FixSQLParameterString(eventInfo->mUserName), S"') AND ",
				S"(ComputerName = '", MOG_DBAPI::FixSQLParameterString(eventInfo->mComputerName), S"') AND ",
				S"(ProjectName = '", MOG_DBAPI::FixSQLParameterString(eventInfo->mProjectName), S"') AND ",
				S"(BranchName = '", MOG_DBAPI::FixSQLParameterString(eventInfo->mBranchName), S"')" );

		MOG_DBAPI::RunNonQuery(updateCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
	}

	// Create a MOG_DBEventInfo to return back
	return eventInfo;
}

MOG_DBEventInfo *MOG_DBEventAPI::SQLReadEvent(SqlDataReader *myReader)
{
	MOG_DBEventInfo *eventInfo = new MOG_DBEventInfo();

	eventInfo->mID = myReader->GetInt32(myReader->GetOrdinal(S"ID"));
	eventInfo->mType = myReader->GetString(myReader->GetOrdinal(S"Type"))->Trim();
	eventInfo->mTimeStamp = myReader->GetString(myReader->GetOrdinal(S"TimeStamp"))->Trim();
	eventInfo->mTitle = myReader->GetString(myReader->GetOrdinal(S"Title"))->Trim();
	eventInfo->mDescription = myReader->GetString(myReader->GetOrdinal(S"Description"))->Trim();
	eventInfo->mStackTrace = myReader->GetString(myReader->GetOrdinal(S"StackTrace"))->Trim();
	eventInfo->mEventID = myReader->GetString(myReader->GetOrdinal(S"EventID"))->Trim();
	eventInfo->mUserName = myReader->GetString(myReader->GetOrdinal(S"UserName"))->Trim();
	eventInfo->mComputerName = myReader->GetString(myReader->GetOrdinal(S"ComputerName"))->Trim();
	eventInfo->mProjectName = myReader->GetString(myReader->GetOrdinal(S"ProjectName"))->Trim();
	eventInfo->mBranchName = myReader->GetString(myReader->GetOrdinal(S"BranchName"))->Trim();
	eventInfo->mRepeatCount = myReader->GetInt32(myReader->GetOrdinal(S"RepeatCount"));

	return eventInfo;
}

bool MOG_DBEventAPI::SQLUpdateEvent(MOG_DBEventInfo *eventInfo)
{
	if( eventInfo == NULL )
		return false;

	String *updateCmd = S"UPDATE [MOG.Events] SET";

	// Variables
	updateCmd = String::Concat(updateCmd, S" Type='", MOG_DBAPI::TruncateIfNecessary(eventInfo->mType, 50, false), S"',");
	updateCmd = String::Concat(updateCmd, S" TimeStamp='", eventInfo->mTimeStamp, S"',");
	updateCmd = String::Concat(updateCmd, S" Title='", MOG_DBAPI::FixSQLParameterString(MOG_DBAPI::TruncateIfNecessary(eventInfo->mTitle, 100, false)), S"',");
	updateCmd = String::Concat(updateCmd, S" Description='", MOG_DBAPI::FixSQLParameterString(MOG_DBAPI::TruncateIfNecessary(eventInfo->mDescription, 1000, false)), S"',");
	updateCmd = String::Concat(updateCmd, S" StackTrace='", MOG_DBAPI::FixSQLParameterString(MOG_DBAPI::TruncateIfNecessary(eventInfo->mStackTrace, 1000, false)), S"',");
	updateCmd = String::Concat(updateCmd, S" EventID='", eventInfo->mEventID, S"',");
	updateCmd = String::Concat(updateCmd, S" UserName='", MOG_DBAPI::FixSQLParameterString(eventInfo->mUserName), S"',");
	updateCmd = String::Concat(updateCmd, S" ComputerName='", MOG_DBAPI::FixSQLParameterString(eventInfo->mComputerName), S"',");
	updateCmd = String::Concat(updateCmd, S" ProjectName='", MOG_DBAPI::FixSQLParameterString(eventInfo->mProjectName), S"',");
	updateCmd = String::Concat(updateCmd, S" BranchName='", MOG_DBAPI::FixSQLParameterString(eventInfo->mBranchName), S"', ");
	updateCmd = String::Concat(updateCmd, S" RepeatCount= ", Convert::ToString(eventInfo->mRepeatCount), S" ");

	// Where clause
	updateCmd = String::Concat(updateCmd, S" WHERE ID=", Convert::ToString(eventInfo->mID));

	return MOG_DBAPI::RunNonQuery(updateCmd, MOG_ControllerSystem::GetDB()->GetConnectionString()); 
}

bool MOG_DBEventAPI::SQLRemoveEvent(MOG_DBEventInfo *eventInfo)
{
	// return early if invalid struct or already exists
	if( eventInfo == NULL )
	{
		return false;
	}

	String *deleteCmd = String::Concat(S"DELETE FROM [MOG.Events] WHERE ID=", Convert::ToString(eventInfo->mID));
	
	return MOG_DBAPI::RunNonQuery(deleteCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
}
