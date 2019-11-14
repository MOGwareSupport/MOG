//--------------------------------------------------------------------------------
//	MOG_CommandEditor.cpp
//	
//	
//--------------------------------------------------------------------------------

#include "StdAfx.h"

#include "MOG_Define.h"
#include "MOG_Main.h"
#include "MOG_StringUtils.h"
#include "MOG_CommandEditor.h"
#include "MOG_DosUtils.h"
#include "MOG_Ini.h"
#include "MOG_ControllerPackageMergeEditor.h"
#include "MOG_ControllerProject.h"
#include "MOG_ControllerSystem.h"
#include "MOG_CommandFactory.h"



MOG_CommandEditor::MOG_CommandEditor()
{
	mNetwork = new MOG_NetworkClient;
	mLockTracker = new MOG_LockTracker(this, false);
	mRegisteredClient = NULL;
}


bool MOG_CommandEditor::RegisterWithServer(void)
{
	// Autodetect our editor's login information
	String *path = MOG_Main::GetAssociatedWorkspacePath();
	if (path->Length)
	{
		// Automatically detect the root of this workspace
		String* workspaceDirectory = MOG_ControllerSyncData::DetectWorkspaceRoot(path);
		if (workspaceDirectory->Length)
		{
			// Check this workspaceDirectory for our 'MOG' directory
			String *syncInfoFilename = String::Concat(workspaceDirectory, "\\MOG\\Sync.Info");
			if (DosUtils::FileExistFast(syncInfoFilename))
			{
				// Load our workspace info
				MOG_Ini *syncInfo = new MOG_Ini(syncInfoFilename);
				if (syncInfo->SectionExist("Workspace"))
				{
					// Make sure we have everything we need to login
					if (syncInfo->KeyExist("Workspace", "Project") &&
						syncInfo->KeyExist("Workspace", "Branch") &&
						syncInfo->KeyExist("Workspace", "User") &&
						syncInfo->KeyExist("Workspace", "Platform"))
					{
						String *project = syncInfo->GetString("Workspace", "Project");
						String *branch = syncInfo->GetString("Workspace", "Branch");
						String *user = syncInfo->GetString("Workspace", "User");
						String *platform = syncInfo->GetString("Workspace", "Platform");

						// Login to the project
						MOG_ControllerProject::LoginProject(project, branch);
						MOG_ControllerProject::LoginUser(user);
						// Set the workspace
						MOG_ControllerSyncData *syncData = new MOG_ControllerSyncData(workspaceDirectory, project, branch, platform, user);
						MOG_ControllerProject::SetCurrentSyncDataController(syncData);
						// Register us with the Server
						MOG_Command *command = MOG_CommandFactory::Setup_RegisterEditor(MOG_Main::GetName(), workspaceDirectory);
						SendToServer(command);
					}
				}
			}
		}
	}

	return true;
}


bool MOG_CommandEditor::CommandExecute(MOG_Command *pCommand)
{
	bool processed = false;

	switch (pCommand->GetCommandType())
	{
		case MOG_COMMAND_RegisterClient:
			processed = Command_RegisterClient(pCommand);
			break;
		case MOG_COMMAND_ShutdownClient:
			processed = Command_ShutdownClient(pCommand);
			break;
		case MOG_COMMAND_LoginProject:
			processed = Command_LoginProject(pCommand);
			break;
		case MOG_COMMAND_LoginUser:
			processed = Command_LoginUser(pCommand);
			break;
		case MOG_COMMAND_LocalPackageMerge:
			processed = Command_LocalPackageMerge(pCommand);
			break;
		case MOG_COMMAND_LockReadRequest:
		case MOG_COMMAND_LockReadRelease:
		case MOG_COMMAND_LockWriteRequest:
		case MOG_COMMAND_LockWriteRelease:
		case MOG_COMMAND_LockWriteQuery:
		case MOG_COMMAND_LockReadQuery:
		case MOG_COMMAND_LockPersistentNotify:
			processed = mLockTracker->CommandExecute(pCommand);
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


bool MOG_CommandEditor::Command_RegisterClient(MOG_Command *pCommand)
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


bool MOG_CommandEditor::Command_ShutdownClient(MOG_Command *pCommand)
{
	//We just lost our client
	MOG_ControllerProject::Logout();
	
	// Always eat the command
	return true;
}


bool MOG_CommandEditor::Command_LoginProject(MOG_Command *pCommand)
{
	// Always eat this command...We don't care because the server tracks this from the client
	return true;
}


bool MOG_CommandEditor::Command_LoginUser(MOG_Command *pCommand)
{
	// Always eat this command...We don't care because the server tracks this from the client
	return true;
}


bool MOG_CommandEditor::Command_LocalPackageMerge(MOG_Command *pCommand)
{
	// Check if we are missing the Database?
	if (!MOG_ControllerSystem::GetDB())
	{
		// Never process this command w/o the database!
		return false;
	}

	try
	{
		// If callback exist, call it
		if (mCallbacks && mCallbacks->mCommandCallback)
		{
			MOG_ControllerPackageMergeEditor *editorPackageMerge = new MOG_ControllerPackageMergeEditor();
			editorPackageMerge->DoPackageEvent(pCommand->GetWorkingDirectory(), pCommand->GetPlatform(), NULL);
		}
		else
		{
			// Indicate the Editor has a major problem!
			String *message = String::Concat(	S"MOG is missing the required Editor callback routines in order to execute this package merge request.\n",
												S"Contact your MOG System Administrator.");
			MOG_Report::ReportMessage(S"Editor/Package Merge", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
		}
	}
	catch(Exception *e)
	{
		// Report the exception
		MOG_Report::ReportMessage(S"Editor/Package Merge", e->Message, e->StackTrace, MOG_ALERT_LEVEL::CRITICAL);
	}
	__finally
	{
		// Send the MOG_COMMAND_PackageComplete command
		MOG_Command *packageComplete = MOG_CommandFactory::Setup_Complete(pCommand, true);
		SendToServer(packageComplete);
	}

	return true;
}


bool MOG_CommandEditor::Command_Complete(MOG_Command *pCommand)
{
	// Make sure to call our base class
	MOG_CommandManager::Command_Complete(pCommand);

	// Always eat this command
	return true;
}


