//--------------------------------------------------------------------------------
//	MOG_DBCommands.cpp
//	
//	
//--------------------------------------------------------------------------------
#include "stdafx.h"


#include "MOG_ControllerSystem.h"

#include "MOG_DBCommandAPI.h"



// ********************************
// Command Sql code
MOG_Command *MOG_DBCommandAPI::QueryCommand(String *selectString)
{
	SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());
	MOG_Command *command = NULL;
	
	SqlDataReader *myReader = MOG_DBAPI::GetNewDataReader(selectString, myConnection);
	try
	{

		while(myReader->Read())
		{
			command = SQLReadCommand(myReader);
			break;
		}
		myReader->Close();
		myConnection->Close();
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
	return command;
}


ArrayList *MOG_DBCommandAPI::QueryCommands(String *selectString)
{
	SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());
	ArrayList *commands = NULL;
	commands = new ArrayList();
	SqlDataReader *myReader = MOG_DBAPI::GetNewDataReader(selectString, myConnection);
	try
	{
		while(myReader->Read())
		{
			commands->Add(SQLReadCommand(myReader));
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
	return commands;
}


MOG_Command *MOG_DBCommandAPI::SQLReadCommand(SqlDataReader *myReader)
{
	MOG_Command *command = new MOG_Command();

	command->SetCommandType(MOG_COMMAND_TYPE(myReader->GetInt32(myReader->GetOrdinal(S"CommandType"))));
	
	command->SetCommandID(myReader->GetInt32(myReader->GetOrdinal(S"CommandID")));
	command->SetCommandTimeStamp(myReader->GetString(myReader->GetOrdinal(S"CommandTimeStamp"))->Trim());
	
	command->SetBlocking(myReader->GetBoolean(myReader->GetOrdinal(S"Blocking")));
	command->SetCompleted(myReader->GetBoolean(myReader->GetOrdinal(S"Completed")));
	command->SetRemoveDuplicateCommands(myReader->GetBoolean(myReader->GetOrdinal(S"RemoveDuplicateCommands")));

	command->SetPersistentLock(myReader->GetBoolean(myReader->GetOrdinal(S"PersistantLock")));
	command->SetPreserveCommand(myReader->GetBoolean(myReader->GetOrdinal(S"PreserveCommand")));
	
	command->SetNetworkID(myReader->GetInt32(myReader->GetOrdinal(S"NetworkID")));
	
	command->SetComputerIP(myReader->GetString(myReader->GetOrdinal(S"ComputerIP"))->Trim());
	command->SetComputerName(myReader->GetString(myReader->GetOrdinal(S"ComputerName"))->Trim());
	command->SetProject(myReader->GetString(myReader->GetOrdinal(S"ProjectName"))->Trim());
	command->SetBranch(myReader->GetString(myReader->GetOrdinal(S"Branch"))->Trim());
	command->SetJobLabel(myReader->GetString(myReader->GetOrdinal(S"Label"))->Trim());
	command->SetTab(myReader->GetString(myReader->GetOrdinal(S"Tab"))->Trim());
	command->SetUserName(myReader->GetString(myReader->GetOrdinal(S"UserName"))->Trim());
	command->SetPlatform(myReader->GetString(myReader->GetOrdinal(S"Platform"))->Trim());
	command->SetValidSlaves(myReader->GetString(myReader->GetOrdinal(S"ValidSlaves"))->Trim());
	
	command->SetWorkingDirectory(myReader->GetString(myReader->GetOrdinal(S"WorkingDirectory"))->Trim());
	command->SetSource(myReader->GetString(myReader->GetOrdinal(S"Source"))->Trim());
	command->SetDestination(myReader->GetString(myReader->GetOrdinal(S"Destination"))->Trim());
	command->SetDescription(myReader->GetString(myReader->GetOrdinal(S"Description"))->Trim());
	command->SetVersion(myReader->GetString(myReader->GetOrdinal(S"Version"))->Trim());

	command->SetOptions(myReader->GetString(myReader->GetOrdinal(S"Options"))->Trim());
	command->SetAssetFilename(myReader->GetString(myReader->GetOrdinal(S"AssetFilename"))->Trim());

	return command;
}


bool MOG_DBCommandAPI::SQLWriteCommand(MOG_Command *command)
{
	String *updateCmd = "UPDATE MOG_Commands SET";

	// Variables
	updateCmd = String::Concat(updateCmd, S" CommandType='", Convert::ToInt32(command->GetCommandType()).ToString(), S"',");
	updateCmd = String::Concat(updateCmd, S" CommandID='", command->GetCommandID().ToString(), S"',");
	updateCmd = String::Concat(updateCmd, S" CommandTimeStamp='", command->GetCommandTimeStamp(), S"',");
	updateCmd = String::Concat(updateCmd, S" Blocking='", Convert::ToString(Convert::ToInt32(command->IsBlocking())), S"',");
	updateCmd = String::Concat(updateCmd, S" Completed='", Convert::ToString(Convert::ToInt32(command->IsCompleted())), S"',");
	updateCmd = String::Concat(updateCmd, S" RemoveDuplicateCommands='", Convert::ToString(Convert::ToInt32(command->IsRemoveDuplicateCommands())), S"',");
	updateCmd = String::Concat(updateCmd, S" PersistentLock='", Convert::ToString(Convert::ToInt32(command->IsPersistentLock())), S"',");
	updateCmd = String::Concat(updateCmd, S" PreserveCommand='", Convert::ToString(Convert::ToInt32(command->IsPreserveCommand())), S"',");
	updateCmd = String::Concat(updateCmd, S" NetworkID='", command->GetNetworkID().ToString(), S"',");
	updateCmd = String::Concat(updateCmd, S" ComputerIP='", command->GetComputerIP(), S"',");
	updateCmd = String::Concat(updateCmd, S" ComputerName='", MOG_DBAPI::FixSQLParameterString(command->GetComputerName()), S"',");
	updateCmd = String::Concat(updateCmd, S" ProjectName='", MOG_DBAPI::FixSQLParameterString(command->GetProject()), S"',");
	updateCmd = String::Concat(updateCmd, S" Branch='", MOG_DBAPI::FixSQLParameterString(command->GetBranch()), S"',");
	updateCmd = String::Concat(updateCmd, S" Label='", MOG_DBAPI::FixSQLParameterString(command->GetJobLabel()), S"',");
	updateCmd = String::Concat(updateCmd, S" Tab='", MOG_DBAPI::FixSQLParameterString(command->GetTab()), S"',");
	updateCmd = String::Concat(updateCmd, S" UserName='", MOG_DBAPI::FixSQLParameterString(command->GetUserName()), S"',");
	updateCmd = String::Concat(updateCmd, S" Platform='", MOG_DBAPI::FixSQLParameterString(command->GetPlatform()), S"'");
	updateCmd = String::Concat(updateCmd, S" validSlaves='", MOG_DBAPI::FixSQLParameterString(command->GetValidSlaves()), S"'");
	updateCmd = String::Concat(updateCmd, S" WorkingDirectory='", MOG_DBAPI::FixSQLParameterString(command->GetWorkingDirectory()), S"'");
	updateCmd = String::Concat(updateCmd, S" Source='", MOG_DBAPI::FixSQLParameterString(command->GetSource()), S"'");
	updateCmd = String::Concat(updateCmd, S" Destination='", MOG_DBAPI::FixSQLParameterString(command->GetDestination()), S"'");
	updateCmd = String::Concat(updateCmd, S" Description='", MOG_DBAPI::FixSQLParameterString(command->GetDescription()), S"'");
	updateCmd = String::Concat(updateCmd, S" Version='", MOG_DBAPI::FixSQLParameterString(command->GetVersion()), S"'");
	updateCmd = String::Concat(updateCmd, S" Options='", MOG_DBAPI::FixSQLParameterString(command->GetOptions()), S"'");
	updateCmd = String::Concat(updateCmd, S" AssetFilename='", MOG_DBAPI::FixSQLParameterString(command->GetAssetFilename()->GetOriginalFilename()), S"'");

	// Where clause
	updateCmd = String::Concat(updateCmd, S" WHERE id='", command->GetCommandID().ToString(), S"'");
	return MOG_DBAPI::RunNonQuery(updateCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
}


MOG_Command *MOG_DBCommandAPI::SQLCreateCommand(MOG_Command *command)
{
	// Make sure to sync all the possible IDs in this task
//	SQLSyncTaskIDs(taskInfo);

	// Build a SQL Insert statement string for all the input-form
	String *insertCmd = String::Concat(S"INSERT INTO [Mog.Commands] ",
		S"( CommandType, CommandID, CommandTimeStamp, Blocking, Completed, RemoveDuplicateCommands, PersistantLock, PreserveCommand, NetworkID, ComputerIP, ComputerName, ProjectName, Branch, Label, Tab, UserName, Platform, ValidSlaves, WorkingDirectory, Source, Destination, Description, Version, Options, AssetFilename) VALUES ", S"("
		, __box(Convert::ToInt32(command->GetCommandType())),S","
		, __box(command->GetCommandID()),S",'"
		,command->GetCommandTimeStamp(),S"' ,"
		,(int)(command->IsBlocking()), S","
		,(int)(command->IsCompleted()),S","
		,(int)(command->IsRemoveDuplicateCommands()),S","
		,(int)(command->IsPersistentLock()), S","
		,(int)(command->IsPreserveCommand()),S","
		,__box(command->GetNetworkID()), S",'"
		,command->GetComputerIP(), S"', '"
		,MOG_DBAPI::FixSQLParameterString(command->GetComputerName()),S"','"
		,MOG_DBAPI::FixSQLParameterString(command->GetProject()), S"','"
		,MOG_DBAPI::FixSQLParameterString(command->GetBranch()) ,S"','"
		,MOG_DBAPI::FixSQLParameterString(command->GetJobLabel()), S"','"
		,MOG_DBAPI::FixSQLParameterString(command->GetTab()) , S"','"
		,MOG_DBAPI::FixSQLParameterString(command->GetUserName()), S"','"
		,MOG_DBAPI::FixSQLParameterString(command->GetPlatform()), S"','"
		,MOG_DBAPI::FixSQLParameterString(command->GetValidSlaves()), S"','"
		,MOG_DBAPI::FixSQLParameterString(command->GetWorkingDirectory()), S"','"
		,MOG_DBAPI::FixSQLParameterString(command->GetSource()), S"','"
		,MOG_DBAPI::FixSQLParameterString(command->GetDestination()), S"','"
		,MOG_DBAPI::FixSQLParameterString(command->GetDescription()), S"','"
		,MOG_DBAPI::FixSQLParameterString(command->GetVersion()), S"','"
		,MOG_DBAPI::FixSQLParameterString(command->GetOptions()), S"','"
		,MOG_DBAPI::FixSQLParameterString(command->GetAssetFilename()->GetOriginalFilename())
		, S"')");

		MOG_DBAPI::RunNonQuery(insertCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
	return command;
}


bool MOG_DBCommandAPI::AddLock(MOG_Command *pCommand)
{
	SQLCreateCommand(pCommand);
	return true;
}


bool MOG_DBCommandAPI::RemoveLock(MOG_Command *pCommand)
{
	// Early out for non valid lock
	if (!pCommand ||
		!pCommand->GetCommandID())
	{
		return false;
	}

	String *deleteCmd = String::Concat(S"DELETE FROM [Mog.Commands] WHERE CommandID='", pCommand->GetCommandID().ToString(), S"'");
	return MOG_DBAPI::RunNonQuery(deleteCmd, MOG_ControllerSystem::GetDB()->GetConnectionString()); 
}


bool MOG_DBCommandAPI::RemoveAllLocks()
{
	String *deleteCmd = String::Concat( S"DELETE FROM [Mog.Commands] WHERE ",
										S" (CommandType='", Convert::ToInt32(MOG_COMMAND_TYPE::MOG_COMMAND_LockReadRequest).ToString(), S"') OR ",
										S" (CommandType='", Convert::ToInt32(MOG_COMMAND_TYPE::MOG_COMMAND_LockWriteRequest).ToString(), S"')");
	
	return MOG_DBAPI::RunNonQuery(deleteCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
}


ArrayList *MOG_DBCommandAPI::GetAllLocks()
{
	String *selectString = String::Concat(  S" SELECT       *",
											S" FROM         [Mog.Commands]");

	return QueryCommands(selectString);
}

