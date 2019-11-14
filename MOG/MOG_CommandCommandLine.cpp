//--------------------------------------------------------------------------------
//	MOG_CommandCommandLine.cpp
//	
//	
//--------------------------------------------------------------------------------

#include "StdAfx.h"

#include "MOG_Define.h"
#include "MOG_Main.h"
#include "MOG_StringUtils.h"
#include "MOG_DosUtils.h"
#include "MOG_Ini.h"
#include "MOG_ControllerProject.h"
#include "MOG_CommandFactory.h"

#include "MOG_CommandCommandLine.h"


MOG_CommandCommandLine::MOG_CommandCommandLine()
{
	mNetwork = new MOG_NetworkClient;
}


bool MOG_CommandCommandLine::RegisterWithServer(void)
{
	MOG_Command *command = MOG_CommandFactory::Setup_RegisterCommandLine(MOG_Main::GetName(), MOG_Main::GetAssociatedWorkspacePath());
	SendToServer(command);

	// Check if we are logged into a project?
	if (MOG_ControllerProject::IsProject())
	{
		// Inform the server what project we have logged in to
		command = MOG_CommandFactory::Setup_LoginProject(MOG_ControllerProject::GetProjectName(), MOG_ControllerProject::GetBranchName());
		SendToServer(command);
	}
	// Check if we have logged in as a user?
	if (MOG_ControllerProject::IsUser())
	{
		// Inform the server about who we are logged in as
		command = MOG_CommandFactory::Setup_LoginUser(MOG_ControllerProject::GetUser()->GetUserName());
		SendToServer(command);
	}

	return true;
}


bool MOG_CommandCommandLine::CommandExecute(MOG_Command *pCommand)
{
	bool processed = false;

	switch (pCommand->GetCommandType())
	{
		case MOG_COMMAND_RegisterClient:
			processed = Command_RegisterClient(pCommand);
			break;
		case MOG_COMMAND_ShutdownCommandLine:
			processed = Command_ShutdownCommandLine(pCommand);
			break;
		case MOG_COMMAND_LoginProject:
			processed = Command_LoginProject(pCommand);
			break;
		case MOG_COMMAND_LoginUser:
			processed = Command_LoginUser(pCommand);
			break;
		case MOG_COMMAND_ActiveViews:
			processed = Command_ActiveViews(pCommand);
			break;
		case MOG_COMMAND_AssetRipRequest:
			processed = Command_AssetRipRequest(pCommand);
			break;
		case MOG_COMMAND_Bless:
			processed = Command_Bless(pCommand);
			break;
		case MOG_COMMAND_LockReadRequest:
			processed = Command_LockReadRequest(pCommand);
			break;
		case MOG_COMMAND_LockWriteRequest:
			processed = Command_LockWriteRequest(pCommand);
			break;
		case MOG_COMMAND_Complete:
			processed = Command_Complete(pCommand);
			break;

		default:
			// Allow the parent a chance to process this command
			processed = MOG_CommandManager::CommandExecute(pCommand);
			break;
	}

	return processed;
}


bool MOG_CommandCommandLine::Command_RegisterClient(MOG_Command *pCommand)
{
	// Keep this information around so we know about the client
	mRegisteredClient = pCommand;

	// Make sure that this RegisterClient command knows what project to log into?
	if (mRegisteredClient->GetProject()->Length)
	{
		// Login into the right project and user
		MOG_ControllerProject::LoginProject(mRegisteredClient->GetProject(), mRegisteredClient->GetBranch());
	}
	// Make sure that this RegisterClient command knows what user to log into?
	if (mRegisteredClient->GetUserName()->Length)
	{
		MOG_ControllerProject::LoginUser(mRegisteredClient->GetUserName());
	}

	// Always eat the command
	return true;
}


bool MOG_CommandCommandLine::Command_ShutdownCommandLine(MOG_Command *pCommand)
{
	MOG_Main::Shutdown();

	// Always eat the command
	return true;
}


bool MOG_CommandCommandLine::Command_LoginProject(MOG_Command *pCommand)
{
	// Always eat this command...We don't care if the server to tracks this
	return true;
}


bool MOG_CommandCommandLine::Command_LoginUser(MOG_Command *pCommand)
{
	// Always eat this command...We don't care if the server to tracks this
	return true;
}


bool MOG_CommandCommandLine::Command_AssetRipRequest(MOG_Command *pCommand)
{
	// Send this command to the server
	return SendToServer(pCommand);
}


bool MOG_CommandCommandLine::Command_Bless(MOG_Command *pCommand)
{
	// Send this command to the server
	return SendToServer(pCommand);
}


bool MOG_CommandCommandLine::Command_LockReadRequest(MOG_Command *pCommand)
{
	return LockWaitDialog(pCommand);
}


bool MOG_CommandCommandLine::Command_LockWriteRequest(MOG_Command *pCommand)
{
	return LockWaitDialog(pCommand);
}


bool MOG_CommandCommandLine::Command_Complete(MOG_Command *pCommand)
{
	// Make sure to call our base class
	MOG_CommandManager::Command_Complete(pCommand);

	// Make sure this contains an encapsulated command?
	if (pCommand->GetCommand())
	{
		// Determin the type of encapsulated command
		switch(pCommand->GetCommand()->GetCommandType())
		{
			case MOG_COMMAND_AssetProcessed:
				// Tell the command line tool to close
				MOG_Main::Shutdown();
		}
	}

	// Always eat this command...
	return true;
}


