//--------------------------------------------------------------------------------
//	MOG_ControllerSystem.cpp
//	
//	
//--------------------------------------------------------------------------------

#include "StdAfx.h"

#include "MOG_Define.h"
#include "MOG_Tokens.h"
#include "MOG_DosUtils.h"
#include "MOG_CommandClient.h"
#include "MOG_CommandSlave.h"
#include "MOG_CommandEditor.h"
#include "MOG_CommandCommandLine.h"
#include "MOG_ControllerProject.h"
#include "MOG_ControllerRepository.h"
#include "MOG_DBEventAPI.h"
#include "MOG_Prompt.h"
#include "MOG_Progress.h"
#include "MOG_CommandFactory.h"

#include "MOG_ControllerSystem.h"

using namespace MOG_CoreControls;
using namespace System::IO;
using namespace System::ComponentModel;
using namespace System::Collections::Generic;
using namespace System::Net;

bool MOG_ControllerSystem::Shutdown()
{
	//Let the prompt thread catch up and think about if it is visible or not.
	Thread::Sleep(100);

	//Don't shut down the application until the prompt dialog is closed.
	while (MOG_Prompt::IsDialogVisible())
	{
		Thread::Sleep(100);
	}

	// Make sure we have a command manager initialized?
	if (mCommandManager)
	{
		// Make sure we shutdown our command manager
		mCommandManager->Shutdown();

		// Make sure we shutdown any active processes
		DosUtils::Shutdown();
	}

	return true;
}

bool MOG_ControllerSystem::InitializeSystem(String *configFilename, MOG_CommandManager *commandManager )
{
	// Initialize our Database
	MOG_ControllerSystem::InitializeDatabase(S"", S"", S"");

	// Create a new system
	mSystem = new MOG_System();
	// Load the system
	if (mSystem->Load(configFilename))
	{
		mCommandManager = commandManager;

// JohnRen - TEMP CODE - This can be removed anytime as this was only needed as we transitioned to a longer timestamp
		//Update Date Columns as Necessary
		MOG_ControllerSystem::GetDB()->AlterAllDateColumns();
		
		int dbVersion = MOG_ControllerSystem::GetDB()->GetDBVersion();

		//We need to add this to turn off silent prompt mode while we update tables.
		bool defaultMode = MOG_Prompt::IsMode(MOG_PROMPT_MODE_TYPE::MOG_PROMPT_SILENT);
		bool updateVersion = true;
		if(defaultMode)
		{
			MOG_Prompt::ClearMode(MOG_PROMPT_MODE_TYPE::MOG_PROMPT_SILENT);
		}

		// Verify the system tables
		if(!MOG_ControllerSystem::GetDB()->UpdateSystemTables(dbVersion, false))
		{
			MOG_Main::Shutdown();
			updateVersion = false;
			return false;
		}

		// Verify and Update all Project Tables
		for (int i = 0; i < mSystem->GetProjectNames()->Count; i++)
		{
			if(updateVersion)
			{
				String *projectName = __try_cast<String*>(mSystem->GetProjectNames()->Item[i]);
				if(!MOG_ControllerSystem::GetDB()->UpdateTables(projectName, dbVersion, true))
				{
					MOG_Main::Shutdown();
					updateVersion = false;
					return false;
				}
			}
		}

		//This is to reset the silent mode once we are done with table updates.
		if(defaultMode)
		{
			MOG_Prompt::SetMode(MOG_PROMPT_MODE_TYPE::MOG_PROMPT_SILENT);
		}

		//We need to make sure we didn't cancel table update before reving database version.
		//We may not need this after shutdown is working appropriately.
		if(updateVersion)
		{
			MOG_ControllerSystem::GetDB()->UpdateDBVersion();
		}

		// Create a command manager		
		if (commandManager->Initialize())
		{
			// Get the computer's information from the network
			MOG_Network* pNetwork = commandManager->GetNetwork();
			if (pNetwork)
			{
				mComputerName = pNetwork->GetComputerName();
				mComputerIP = pNetwork->GetIP();
				mComputerMacAddress = pNetwork->GetMacAddress();
			}

			return true;
		}
	}
	return false;
}


bool MOG_ControllerSystem::InitializeClient(String *configFilename)
{
	// Initialize the client mode?
	mMode = MOG_SystemMode_Client;

	// Create a new system
	mSystem = new MOG_System();
	
	// Load the system
	if (mSystem->Load(configFilename))
	{
		// Create a command manager
		mCommandManager = new MOG_CommandClient();
		// Initialize the command manager?
		if (mCommandManager->Initialize())
		{
			// Initialize the network variables
			InitializeNetworkInfo();

			// Wait for our NetworkID
			if (RequireNetworkIDInitialization())
			{
				// Initialize our Database
				if (MOG_ControllerSystem::InitializeDatabase(S"", S"", S""))
				{
					return true;
				}
			}
		}
	}

	return false;
}


bool MOG_ControllerSystem::InitializeSlave(String *configFilename)
{
	// Initialize the client mode?
	mMode = MOG_SystemMode_Slave;

	// Initialize our Database
	MOG_ControllerSystem::InitializeDatabase(S"", S"", S"");
	// This component can't properly manage the inbox cache because it doesn't receive the view update commands
	MOG_ControllerSystem::GetDB()->GetDBCache()->GetInboxAssetCache()->SetEnabled(false);

	// Create a new system
	mSystem = new MOG_System();

	// Load the system
	if (mSystem->Load(configFilename))
	{
		// Create a command manager
		mCommandManager = new MOG_CommandSlave();
		// Initialize the command manager?
		if (mCommandManager->Initialize())
		{
			// Initialize the network variables
			InitializeNetworkInfo();
			return true;
		}
	}
	return false;
}


bool MOG_ControllerSystem::InitializeEditor(String *configFilename)
{
	// Initialize the client mode?
	mMode = MOG_SystemMode_Editor;

	// Initialize our Database
	MOG_ControllerSystem::InitializeDatabase(S"", S"", S"");
	// This component can't properly manage the inbox cache because it doesn't receive the view update commands
	MOG_ControllerSystem::GetDB()->GetDBCache()->GetInboxAssetCache()->SetEnabled(false);
	
	// Create a new system
	mSystem = new MOG_System();

	// Load the system
	if (mSystem->Load(configFilename))
	{
		// Create a command manager
		mCommandManager = new MOG_CommandEditor();
		// Initialize the command manager?
		if (mCommandManager->Initialize())
		{
			// Initialize the network variables
			InitializeNetworkInfo();
			return true;
		}
	}
	return false;
}


bool MOG_ControllerSystem::InitializeCommandLine(String *configFilename)
{
	// Initialize the client mode?
	mMode = MOG_SystemMode_CommandLine;
	bSuppressMessageBox=true;

	// Initialize our Database
	MOG_ControllerSystem::InitializeDatabase(S"", S"", S"");
	// This component can't properly manage the inbox cache because it doesn't receive the view update commands
	MOG_ControllerSystem::GetDB()->GetDBCache()->GetInboxAssetCache()->SetEnabled(false);

	// Create a new system
	mSystem = new MOG_System();

	// Load the system
	if (mSystem->Load(configFilename))
	{
		// Create a command manager
		mCommandManager = new MOG_CommandCommandLine();
		// Initialize the command manager?
		if (mCommandManager->Initialize())
		{
			// Initialize the network variables
			InitializeNetworkInfo();
			return true;
		}
	}
	return false;
}


bool MOG_ControllerSystem::InitializeDatabase(String *connectionString, String *projectName, String *branchName)
{
	// Initialize a new MOG_Database?
	if (!mDB)
	{
		mDB = new MOG_Database();
	}

	// Attempt to initialize the database?
	if (mDB->Initialize(connectionString))
	{
		// Check if we need to supply the project info?
		if (!projectName || !projectName->Length)
		{
			projectName = MOG_ControllerProject::GetProjectName();
		}

		// Check if we need to supply the branch info?
		if (!branchName || !branchName->Length)
		{
			branchName = MOG_ControllerProject::GetBranchName();
		}

		// Attempt to connect to the project?
		if (mDB->Connect(projectName, branchName))
		{
		}

		return true;
	}

	return false;
}


// Initialize the network variables
void MOG_ControllerSystem::InitializeNetworkInfo()
{
	// Get the computer's information from the network
	mComputerName = mCommandManager->GetNetwork()->GetComputerName();
	mComputerIP = mCommandManager->GetNetwork()->GetIP();
}

bool MOG_ControllerSystem::RequireProjectUserInitialization()
{
	return RequireInitializationCredentials(true, true, false);
}

// Verify's Project and User are valid or wait for server confirmation
bool MOG_ControllerSystem::RequireInitializationCredentials(bool bRequireProject, bool bRequireUser, bool bRequirePlatform)
{
	//Attempt to obtain initialization credentials
	if (!ObtainInitializationCredentials(bRequireProject, bRequireUser, bRequirePlatform))
	{
		// Make sure we were able to establish our Project and User?
		MOG_ASSERT_THROW(MOG_ControllerProject::IsProject() && MOG_ControllerProject::IsUser(), MOG_Exception::MOG_EXCEPTION_NotInitialized, "MOG - Failed to obtain the client's Project/User from the server.");
	}

	return true;
}

bool MOG_ControllerSystem::ObtainInitializationCredentials(bool bRequireProject, bool bRequireUser, bool bRequirePlatform)
{
	List<bool>* args = new List<bool>();
	args->Add(bRequireProject);
	args->Add(bRequireUser);
	args->Add(bRequirePlatform);
	
	ProgressDialog* progress = new ProgressDialog(S"Obtaining Initialization Credentials", S"Please wait while MOG obtains initialization credentials from the server", new DoWorkEventHandler(NULL, &ObtainInitializationCredentials_Worker), args, false);
	if (progress->ShowDialog() == DialogResult::OK)
	{
		//Check specified requirements
		if (bRequireProject && !MOG_ControllerProject::IsProject())
		{
			return false;
		}
		if (bRequireUser && !MOG_ControllerProject::IsUser())
		{
			return false;
		}
		if (bRequirePlatform && MOG_ControllerProject::GetCurrentSyncDataController() == NULL)
		{
			return false;
		}

		return true;
	}

	return false;
}

void MOG_ControllerSystem::ObtainInitializationCredentials_Worker(Object* sender, DoWorkEventArgs* e)
{
	BackgroundWorker* worker = dynamic_cast<BackgroundWorker*>(sender);
	List<bool>* args = dynamic_cast<List<bool>*>(e->Argument);
	bool bRequireProject = args->Item[0];
	bool bRequireUser = args->Item[1];
	bool bRequirePlatform = args->Item[2];

	// We need to wait a little while till the server can tell us our client's project and user
	int sleepTime = 10;

	for (int s = 0; s < 5000 && !worker->CancellationPending; s += sleepTime)
	{
		// Tick our process loop
		MOG_Main::Process();

		// Check to see if we heard back from the server?
		bool bWait = false;
		if (bRequireProject && !MOG_ControllerProject::IsProject())
		{
			bWait = true;
		}
		if (bRequireUser &&	!MOG_ControllerProject::IsUser())
		{
			bWait = true;
		}
		if (bRequirePlatform &&	MOG_ControllerProject::GetCurrentSyncDataController() == NULL)
		{
			bWait = true;
		}
		// Can we stop waiting?
		if (!bWait)
		{
			//MOG_Report::LogComment(String::Concat(S"Received Project:", MOG_ControllerProject::GetProjectName(), S"   User:", MOG_ControllerProject::GetUser()->GetUserName()));
			break;
		}

		// Fall asleep for a a bit while we are waiting to hear back from the server
		Thread::Sleep(sleepTime);
	}
}

// Verify's Project and User are valid or wait for server confirmation
bool MOG_ControllerSystem::RequireNetworkIDInitialization()
{
	// We need to wait a little while till the server can tell us our client's project and user
	int sleepTime = 10;

	// Make sure we have a valid CommandManager?
	if (GetCommandManager())
	{
		// Establish our new NetworkID
		MOG_NetworkClient *pClient = dynamic_cast<MOG_NetworkClient*>(GetCommandManager()->GetNetwork());
		if (pClient)
		{
			// If we already have a NetworkID we can skip this
			if (!pClient->GetID())
			{
				// Indicate that we need to obtain this information from the Server
//				MOG_Report::LogComment(S"No NetworkID");
//				MOG_Report::LogComment(S"Waiting for Server to send ConnectionNew command");

				for (int s = 0; s < 2000; s += sleepTime)
				{
					// Tick our process loop
					MOG_Main::Process();

					// Check to see if we heard back from the server?
					if (pClient->GetID())
					{
//						MOG_Report::LogComment(String::Concat(S"Received NetworkID:", Convert::ToString(pClient->GetID())));
						return true;
					}

					// Fall asleep for a a bit while we are waiting to hear back from the server
					System::Threading::Thread::Sleep(sleepTime);
				}
			}
		}

		// Make sure we were able to establish our Project and User?
		MOG_ASSERT_THROW(pClient->GetID(), MOG_Exception::MOG_EXCEPTION_NotInitialized, "MOG - Failed to obtain the ConnectionNew command from the server");
	}

	// Make sure we were able to establish our Project and User?
	MOG_ASSERT_THROW(GetCommandManager(), MOG_Exception::MOG_EXCEPTION_NotInitialized, "MOG - Failed to initialize our CommandManager");

	return false;
}


bool MOG_ControllerSystem::Process(void)
{
	// Make sure we have a valid CommandManager?
	if (mCommandManager)
	{
		try
		{
			// ReadConnections will receive all pending commands from all active connections
			mCommandManager->ReadConnections();
			// Process this cycle
			mCommandManager->Process();
		}
		catch(ThreadAbortException *e)
		{
			// Eat this error, since really, all that's happening is MOG is being Shutdown
			e->ToString();
			return false;
		}
		catch(Exception *e)
		{
			// Make sure to inform the Server about this exception
			MOG_Report::ReportMessage(S"Process", e->Message, e->StackTrace, MOG_ALERT_LEVEL::CRITICAL);
			System::Diagnostics::Debug::WriteLine( String::Concat(e->Message, S"\n", e->StackTrace ) );
			return false;
		}
	}
	return true;
}

String *MOG_ControllerSystem::GetSystemMode()
{
	switch(mMode)
	{
	case MOG_SystemMode_Server:
		return "MOG_Server";
		break;    
	case MOG_SystemMode_Client:
		return "MOG_Client";
		break;    
	case MOG_SystemMode_Slave:
		return "MOG_Slave";
		break;    
	case MOG_SystemMode_Editor:
		return "MOG_Editor";
		break;    
	case MOG_SystemMode_CommandLine:
		return "MOG_CommandLine";
		break;    
	}

	return "Unknown";
}


void MOG_ControllerSystem::SetSystemMode(MOG_SystemMode mode)
{
	mMode = mode;

	if (mode == MOG_SystemMode_Server)
	{
		mSystem->ServerName = mComputerName;
		mSystem->ServerIP = mComputerIP;
		mSystem->ServerMacAddress = mComputerMacAddress;

		// Specify we are the new server
		mSystem->GetConfigFile()->PutString(S"Network", S"ServerName", mComputerName);
		mSystem->GetConfigFile()->PutString(S"Network", S"ServerIP", mComputerIP);
		mSystem->GetConfigFile()->PutString(S"Network", S"ServerMacAddress", mComputerMacAddress);
		mSystem->GetConfigFile()->PutValue(S"Network", S"MajorVersion", MOG_MAJOR_VERSION);
		mSystem->GetConfigFile()->PutValue(S"Network", S"MinorVersion", MOG_MINOR_VERSION);
		mSystem->GetConfigFile()->Save();
	}
}

MOG_System *MOG_ControllerSystem::GetSystem(void)
{
	MOG_ASSERT_THROW(mSystem, MOG_Exception::MOG_EXCEPTION_NotInitialized, "MOG - mSystem hasn't been specified");
	return mSystem;
}


MOG_CommandManager *MOG_ControllerSystem::GetCommandManager(void)
{
	MOG_ASSERT_THROW(mCommandManager, MOG_Exception::MOG_EXCEPTION_NotInitialized, "MOG - mCommandManager failed to initialize.  Check startup settings in 'MOG.ini'.");
	return mCommandManager;
}


bool MOG_ControllerSystem::RequestActiveCommands(void)
{
	MOG_Command *command = MOG_CommandFactory::Setup_RequestActiveCommands();
	return mCommandManager->SendToServer(command);
}


bool MOG_ControllerSystem::RequestActiveLocks(void)
{
	MOG_Command *command = MOG_CommandFactory::Setup_RequestActiveLocks();
	return mCommandManager->SendToServer(command);
}


bool MOG_ControllerSystem::RequestActiveConnections(void)
{
	MOG_Command *command = MOG_CommandFactory::Setup_RequestActiveConnections();
	return mCommandManager->SendToServer(command);
}


bool MOG_ControllerSystem::ChangeSQLConnection(String *SQLConnectionString)
{
	if (mCommandManager)
	{
		MOG_Command *command = MOG_CommandFactory::Setup_SQLConnection(SQLConnectionString);
		return mCommandManager->SendToServer(command);
	}
	else
	{
		MOG_Report::ReportMessage(S"ChangeSQLConnection Error", S"MOG cannot change the SQL Connection because the Command Manager has not been initialized.", Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
		return false;
	}
}


bool MOG_ControllerSystem::ChangeRepository(String *newRepositoryPath)
{
	if (mCommandManager)
	{
		MOG_Command *command = MOG_CommandFactory::Setup_MOGRepository(newRepositoryPath);
		return mCommandManager->SendToServer(command);
	}
	else
	{
		MOG_Report::ReportMessage(S"ChangeRepository Error", S"MOG cannot change the Repository because the Command Manager has not been initialized.", Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
		return false;
	}
}


bool MOG_ControllerSystem::ConnectionKill(int networkID)
{
	MOG_Command *pCommand = MOG_CommandFactory::Setup_ConnectionKill(networkID);
	return mCommandManager->SendToServer(pCommand);
}


bool MOG_ControllerSystem::LockKill(MOG_Command *pLockCommand)
{
	// Make sure we have a valid pLockCommand?
	if (pLockCommand)
	{
		MOG_Command *pLockInfo = pLockCommand;

		// Chack if this is a lock Query?
		if (pLockCommand->GetCommandType() == MOG_COMMAND_TYPE::MOG_COMMAND_LockReadQuery ||
			pLockCommand->GetCommandType() == MOG_COMMAND_TYPE::MOG_COMMAND_LockWriteQuery)
		{
			// Make sure we contain a command?
			if (pLockInfo->GetCommand())
			{
				// Use the contained command within this LockQuery
				pLockInfo = pLockInfo->GetCommand();
			}
		}

		// Kill the lock
		return LockKill(pLockInfo->GetAssetFilename()->GetOriginalFilename(),
						pLockInfo->ToString(),
						pLockInfo->GetUserName(),
						pLockInfo->GetComputerName());
	}

	return false;
}

bool MOG_ControllerSystem::LockKill(String *lockName, String *commandType, String *userName, String *computerName)
{
	MOG_Command *command;
	bool bRemoved = false;

	// Make sure that the lock type matches before we setup for the release
	if (commandType->ToLower()->IndexOf(S"Read"->ToLower()) != -1)
	{
		command = MOG_CommandFactory::Setup_LockReadRelease(lockName);
		command->SetUserName(userName);
		command->SetComputerName(computerName);
		bRemoved = mCommandManager->CommandProcess(command);
	}
	else if (commandType->ToLower()->IndexOf(S"Write"->ToLower()) != -1)
	{
		command = MOG_CommandFactory::Setup_LockWriteRelease(lockName);
		command->SetUserName(userName);
		command->SetComputerName(computerName);
		bRemoved = mCommandManager->CommandProcess(command);
	}

	// Check if the lock was successfully removed?
	if (bRemoved)
	{
		// Check if this lock was a different user's lock?
		if (String::Compare(userName, MOG_ControllerProject::GetUserName_DefaultAdmin(), true) != 0)
		{
			// Inform the user we just killed their lock!
			String *message = String::Concat(S"'", MOG_ControllerProject::GetUserName_DefaultAdmin(), S"' just killed one of your locks.\n",
											 S"LOCK: ", lockName, S"\n\n",
											 S"It is recommended you contact them if this presents a problem.");
			NetworkBroadcast(userName, message);
		}
	}

	return bRemoved;
}


MOG_Command *MOG_ControllerSystem::GetEditorInfo(String* workspaceDirectory)
{
	// Send the GetEditorInfo command to the server.
	MOG_Command *command = MOG_CommandFactory::Setup_GetEditorInfo(workspaceDirectory);
	if (mCommandManager->SendToServerBlocking(command))
	{
		return command->GetCommand();
	}

	return NULL;
}


bool MOG_ControllerSystem::CheckEditorRunning(String* workspaceDirectory)
{
	// Check if we have an Editor running?
	MOG_Command *pEditor = GetEditorInfo(workspaceDirectory);
	if (pEditor)
	{
		return true;
	}

	return false;
}


bool MOG_ControllerSystem::CheckEditorBusy(String* workspaceDirectory)
{
	// Check if we have an Editor running?
	MOG_Command *pEditor = GetEditorInfo(workspaceDirectory);
	if (pEditor)
	{
		// Check if the Editor is currently assigned a command?
		if (pEditor->GetCommand())
		{
			return true;
		}
	}

	return false;
}


bool MOG_ControllerSystem::WaitWhileEditorRunning(String* workspaceDirectory)
{
	// Inform the user that they shouldn't sync while running the Editor
	String *message = String::Concat(S"You have an editor running in this workspace that needs to be closed before MOG can perform a GetLatest.");
	
	ProgressDialog* progress = new ProgressDialog(S"Please close associated Editor", message, new DoWorkEventHandler(NULL, &WaitWhileEditorRunning_Worker), workspaceDirectory, false);
	if (progress->ShowDialog() == DialogResult::OK)
	{
		return true;
	}

	return false;
}

void MOG_ControllerSystem::WaitWhileEditorRunning_Worker(Object* sender, DoWorkEventArgs* e)
{
	BackgroundWorker* worker = dynamic_cast<BackgroundWorker*>(sender);
	String* workspaceDirectory = dynamic_cast<String*>(e->Argument);

	// Check if the user is currently running their Editor?
	while (MOG_ControllerSystem::CheckEditorRunning(workspaceDirectory) && !worker->CancellationPending)
	{
		Thread::Sleep(1000);
	}
}

bool MOG_ControllerSystem::ShutdownSlave(void)
{
	return ShutdownSlave(MOG_ControllerSystem::GetComputerName());
}


bool MOG_ControllerSystem::ShutdownSlave(int networkID)
{
	// Send the ShutdownSlave command to the server.
	MOG_Command *command = MOG_CommandFactory::Setup_ShutdownSlave(networkID);
	return mCommandManager->SendToServer(command);
}


bool MOG_ControllerSystem::ShutdownSlave(String *computerName)
{
	// Send the ShutdownSlave command to the server.
	MOG_Command *command = MOG_CommandFactory::Setup_ShutdownSlave(computerName);
	return mCommandManager->SendToServer(command);
}


bool MOG_ControllerSystem::NetworkBroadcast(String *users, String *message)
{
	// Send the NetworkBroadcast command to the server.
	MOG_Command *command = MOG_CommandFactory::Setup_NetworkBroadcast(users, message);
	return mCommandManager->SendToServer(command);
}


bool MOG_ControllerSystem::NetworkBroadcast_EntireProject(String *project, String *message)
{
	// Send the NetworkBroadcast command to the server.
	MOG_Command *command = MOG_CommandFactory::Setup_NetworkBroadcast_EntireProject(project, message);
	return mCommandManager->SendToServer(command);
}


bool MOG_ControllerSystem::InstantMessage(String *handle, String *invitedUsers, String *message)
{
	// Always eat this command
	return true;
}

String *MOG_ControllerSystem::ResolveToolRelativePath(String *fullPath)
{
	String *relativePath = fullPath;

	// First, check the system tools
	if (fullPath->ToLower()->StartsWith(MOG_ControllerSystem::GetSystem()->GetSystemToolsPath()->ToLower()))
	{
		// project scope
		relativePath = fullPath->Substring(MOG_ControllerSystem::GetSystem()->GetSystemToolsPath()->Length);
	}
	else
	{
		// Are we logged in to a project?
		MOG_Project *pProj = MOG_ControllerProject::GetProject();
		if (pProj != NULL)
		{
			// Check project tools path
			if (fullPath->ToLower()->StartsWith(pProj->GetProjectToolsPath()->ToLower()))
			{
				// project scope
				relativePath = fullPath->Substring(pProj->GetProjectToolsPath()->Length);
			}
			else
			{
				// If they're logged in as a user, see if its in the user's tools path
				if (MOG_ControllerProject::IsUser())
				{
					if (fullPath->ToLower()->StartsWith(MOG_ControllerProject::GetUser()->GetUserToolsPath()->ToLower()))
					{
						// project scope
						relativePath = fullPath->Substring(MOG_ControllerProject::GetUser()->GetUserToolsPath()->Length);
					}
				}
			}
		}
	}

	// Trim off any potentially hazardous characters and return
	String* delimStr = S"\\ ";
	Char delimiter[] = delimStr->ToCharArray();
	return relativePath->Trim(delimiter);
}

bool MOG_ControllerSystem::ToolExist(String *relativeFilename)
{
	// Make sure we have a valid relativeFilename?
	if (relativeFilename && relativeFilename->Length)
	{
		// This path should always be relative and never contain a drive letter or unc path
		if (relativeFilename->IndexOf(S":") != -1 ||
			relativeFilename->IndexOf(S"\\\\") != -1)
		{
			return DosUtils::FileExistFast(relativeFilename);
		}

		// Break the relative path up so we can call LocateTool
		String *relDir = DosUtils::PathGetDirectoryPath(relativeFilename);
		String *filename = DosUtils::PathGetFileName(relativeFilename);
		String *tool = LocateTool(relDir, filename);
		if (tool && tool->Length)
		{
			return true;
		}
	}

	return false;
}

String *MOG_ControllerSystem::LocateTool(String *relativeFilename)
{
	if (relativeFilename && relativeFilename->Length)
	{
		String *relDir = DosUtils::PathGetDirectoryPath(relativeFilename);
		String *filename = DosUtils::PathGetFileName(relativeFilename);
		return LocateTool(relDir, filename);
	}

	return relativeFilename;
}

String *MOG_ControllerSystem::LocateTool(String *relativeToolsPath, String *filename)
{
	return LocateTool(relativeToolsPath, filename, NULL);
}

String *MOG_ControllerSystem::LocateTool(String *relativeToolsPath, String *filename, MOG_Filename *assetDirectory)
{
	String *tool;
	String *toolsDirectory = S"Tools";

	try
	{
		// If the filename passed in is already fully quantified, return it
		if (DosUtils::PathIsDriveRooted(filename))
		{
			if (DosUtils::ExistFast(filename))
			{
				return filename;
			}
			else
			{
				return "";
			}
		}

		if (DosUtils::PathIsDriveRooted(relativeToolsPath))
		{
			String *temp = String::Concat(relativeToolsPath, "\\", filename);
			if (DosUtils::ExistFast(temp))
			{
				return temp;
			}
			else
			{
				return "";
			}
		}

		// Check if we have anything specified in relativeToolsPath?
		if (relativeToolsPath->Length)
		{
			// Make sure relative path is really relative and does not contain a drive letter
			String *root = DosUtils::PathGetRootPath(relativeToolsPath);
			if (root->Length)
			{
				//Take the drive off the relative path
				relativeToolsPath = relativeToolsPath->Substring(root->Length);
			}
		}

		// Check for invalid values?
		if (relativeToolsPath->Length == 0 && filename->Length == 0)
		{
			return S"";
		}

		// Trim off any spaces just to be on the safe side.
		String* delimStr = S" \\";
		Char delimiter[] = delimStr->ToCharArray();
		relativeToolsPath = relativeToolsPath->Trim(delimiter);
		filename = filename->Trim(delimiter);
		relativeToolsPath = relativeToolsPath->Trim(delimiter);

		// Check if a specific tool type was specified?
		if (relativeToolsPath->Length)
		{
			// Extend the tool's sub directory
			toolsDirectory = String::Concat(toolsDirectory, S"\\", relativeToolsPath);
		}
		// We support filenames as well as just directories
		if (filename && filename->Length)
		{
			toolsDirectory = String::Concat(toolsDirectory, S"\\", filename);
		}

		// Check if an assetFilename was specified?
		if (assetDirectory)
		{
			// Look for the tool within the Asset
			tool = String::Concat(assetDirectory->GetEncodedFilename(), S"\\", toolsDirectory);
			// Did we find the tool at this level?
			if (DosUtils::Exist(tool))
			{
				return tool;
			}
		}

		// Make sure that the user has been specified
		if (MOG_ControllerProject::IsUser())
		{
			// Look for the filename in the user's tools
			tool = String::Concat(MOG_ControllerProject::GetUser()->GetUserPath(), S"\\", toolsDirectory);
			// Did we find the tool at this level?
			if (DosUtils::Exist(tool))
			{
				return tool;
			}
		}

		// Make sure that the project has been specified
		if (MOG_ControllerProject::IsProject())
		{
			// Look for the filename in the project's tools
			tool = String::Concat(MOG_ControllerProject::GetProject()->GetProjectPath(), S"\\", toolsDirectory);
			// Did we find the tool at this level?
			if (DosUtils::Exist(tool))
			{
				return tool;
			}
		}

		// Make sure that the system has been specified
		if (MOG_ControllerSystem::GetSystem())
		{
			// Look for the filename in the system's tools
			tool = String::Concat(MOG_ControllerSystem::GetSystem()->GetSystemRepositoryPath(), S"\\", toolsDirectory);
			// Did we find the tool at this level?
			if (DosUtils::Exist(tool))
			{
				return tool;
			}
		}

		// Check if we had a relativeToolsPath specified?
		if (relativeToolsPath->Length)
		{
			// Try it again in the root of tools just in case it might exist there
			tool = LocateTool(S"", filename, assetDirectory);
			// Check if we found the tool this time?
			if (tool->Length)
			{
				return tool;
			}
		}

		// Check each of the listed paths listed in the Path environment variable
		String *path = DosUtils::GetSystemEnvironmentVariable(S"Path");
		if (path->Length)
		{
			// Remove any contained quotes within the path because it messes up DosUtils
			path = path->Replace(S"\"", "");
			// Split the path variable up into all its parts
			String *parts[] = path->Split(S",;"->ToCharArray());
			if (parts != NULL)
			{
				String *relativeFilename = String::Concat(relativeToolsPath, S"\\", filename)->Trim(S" \\"->ToCharArray());

				// Check each individual part to see if this tool can be located?
				for (int i = 0; i < parts->Count; i++)
				{
					// Check if the filename exists within this path?
					String *part = parts[i]->Trim(S"\\ "->ToCharArray());
					tool = String::Concat(part, S"\\", relativeFilename);
					if (DosUtils::Exist(tool))
					{
						return tool;
					}
				}
			}
		}

		// Attempt to tokenize any other environment variables
//?	MOG_ControllerSystem::LocateTool - Include any potential %Variable% environment variables they may have included

		// Check if this is a mog component?
		tool = String::Concat(MOG_Main::GetExecutablePath(), S"\\", filename);
		// Did we find the tool at this level?
		if (DosUtils::Exist(tool))
		{
			return tool;
		}
	}
	catch (...)
	{
	}

	// Indicate we never found the tool
	tool = "";
	return tool;
}


ArrayList *MOG_ControllerSystem::LocateTools(String *relativeFilePattern)
{
	if (relativeFilePattern && relativeFilePattern->Length)
	{
		String *relDir = DosUtils::PathGetDirectoryPath(relativeFilePattern);
		String *filePattern = DosUtils::PathGetFileName(relativeFilePattern);
		return LocateTools(relDir, filePattern);
	}

	return NULL;
}

ArrayList *MOG_ControllerSystem::LocateTools(String *relativeToolsPath, String *filePattern)
{
	ArrayList *tools = new ArrayList();
	String *toolsDirectory = S"Tools";

	// Trim off any spaces just to be on the safe side.
	String* delimStr = S" ";
	Char delimiter[] = delimStr->ToCharArray();
	relativeToolsPath = relativeToolsPath->Trim(delimiter);
	filePattern = filePattern->Trim(delimiter);

	// Check if a specific tool type was specified?
	if (relativeToolsPath->Length)
	{
		// Extend the tool's sub directory
		toolsDirectory = String::Concat(toolsDirectory, S"\\", relativeToolsPath);
	}

	// Make sure that the user has been specified
	if (MOG_ControllerProject::IsUser())
	{
		// Look for the filePattern in the user's tools
		String *tempPath = String::Concat(MOG_ControllerProject::GetUser()->GetUserPath(), S"\\", toolsDirectory);
		// Did we find any matching tools at this level?
		FileInfo *files[] = DosUtils::FileGetList(tempPath, filePattern);
		if (files != NULL)
		{
			// Add each one to our list of tools
			for (int i = 0; i < files->Count; i++)
			{
				tools->Add(files[i]->FullName);
			}
		}
	}

	// Make sure that the project has been specified
	if (MOG_ControllerProject::IsProject())
	{
		// Look for the filePattern in the project's tools
		String *tempPath = String::Concat(MOG_ControllerProject::GetProject()->GetProjectPath(), S"\\", toolsDirectory);
		// Did we find any matching tools at this level?
		FileInfo *files[] = DosUtils::FileGetList(tempPath, filePattern);
		if (files != NULL)
		{
			// Add each one to our list of tools
			for (int i = 0; i < files->Count; i++)
			{
				tools->Add(files[i]->FullName);
			}
		}
	}

	// Make sure that the system has been specified
	if (MOG_ControllerSystem::GetSystem())
	{
		// Look for the filePattern in the system's tools
		String *tempPath = String::Concat(MOG_ControllerSystem::GetSystem()->GetSystemRepositoryPath(), S"\\", toolsDirectory);
		// Did we find any matching tools at this level?
		FileInfo *files[] = DosUtils::FileGetList(tempPath, filePattern);
		if (files != NULL)
		{
			// Add each one to our list of tools
			for (int i = 0; i < files->Count; i++)
			{
				tools->Add(files[i]->FullName);
			}
		}
	}

	// Check if we had a relativeToolsPath specified?
	if (relativeToolsPath->Length)
	{
		// Try it again in the root of tools just in case it might exist there
		ArrayList *rootTools = LocateTools(S"", filePattern);
		if (rootTools != NULL)
		{
			tools->AddRange(rootTools);
		}
	}

	// Indicate we never found the tool
	return tools;
}


String *MOG_ControllerSystem::LocateInstallItem(String *item)
{
//?	MOG_ControllerSystem::LocateInstallItem - We should store these install path assumptions in an ini file instead of hard coding them

	// First, check the dev directory location?
	String *devItem = String::Concat(S"C:\\EInstall\\", item);
	if (DosUtils::Exist(devItem))
	{
		return devItem;
	}

	// Second, check in the MOG Repository?
	String *repositoryItem = String::Concat(MOG_ControllerSystem::GetSystemRepositoryPath(), S"\\", item);
	if (DosUtils::Exist(repositoryItem))
	{
		return repositoryItem;
	}

	// Check the install directory location?
	String *installItem = String::Concat(MOG_Main::GetExecutablePath_StripCurrentDirectory(), S"\\setup\\", item);
	if (DosUtils::Exist(installItem))
	{
		return installItem;
	}

	return "";
}


String *MOG_ControllerSystem::LocateCriticalInstallItem(String *item)
{
	String *found = LocateInstallItem(item);
	// Always throw when we couldn't locate our needed file
	MOG_ASSERT_THROW(found->Length, MOG_Exception::MOG_EXCEPTION_MissingFile, String::Concat(S"MOG ERROR - Could not locate required install file: ", item));
	return found;
}


String* MOG_ControllerSystem::InternalizeTool(String* tool, String *relativeToolsPath)
{
	// Check for the hard coded reserved words?
	if (String::Compare(tool, S"") == 0  ||  
		String::Compare(tool, S"Inherit", true) == 0  ||  
		String::Compare(tool, S"Inherited", true) == 0  ||  
		String::Compare(tool, S"None", true) == 0  ||  
		String::Compare(tool, S"Nothing", true) == 0  ||  
		String::Compare(tool, S"Default", true) == 0  ||  
		String::Compare(tool, S"Copy", true) == 0)
	{
		return tool;
	}

	// Break up the specified tool
	String *sourceFile = DosUtils::FileStripArguments(tool)->Trim(S"\""->ToCharArray());
	String *toolFilename = DosUtils::PathGetFileName(sourceFile);
	String *arguments = DosUtils::FileGetArguments(tool);

	// Check if the tool describes a full path by checking if it exists
	if (File::Exists(sourceFile))
	{
		// Attempt to strip this tool's path down
		String *strippedTool = StripInternalizedToolPath(sourceFile);
		// Check if we failed to strip anything off?
		if (strippedTool->Length == sourceFile->Length)
		{
			// This means the tool exists outside of the repository
			// Ask the user if this tool can be internalized?
			String *message = String::Concat(	S"The System has detected this to be an external tool.\n",
												S"TOOL: ", sourceFile, S"\n",
												S"Internalizing tools allows Clients and Slaves easy access without it needing to be installed on each machine.  Internalization is not recommended for applications that require additional files or components.\n",
												S"Can this tool be internalized?");
			if (MOG_Prompt::PromptResponse(S"Internalize Tool", message, MOGPromptButtons::YesNo) == MOGPromptResult::Yes)
			{
				// Get the current Project?
				MOG_Project* proj = MOG_ControllerProject::GetProject();
				if (proj != NULL)
				{
					String *projectRepositoryFile = S"";
					String *internalizedFile = S"";

					// Copy this tool into the project's tools
					// Check if we have a relativeToolsPath specified?
					if (relativeToolsPath->Length)
					{
						// Append on the relativeToolsPath
						projectRepositoryFile = String::Concat(proj->GetProjectToolsPath(), S"\\", relativeToolsPath, S"\\", toolFilename);
						internalizedFile = String::Concat(relativeToolsPath, S"\\", toolFilename);
					}
					else
					{
						projectRepositoryFile = String::Concat(proj->GetProjectToolsPath(), S"\\", toolFilename);
						internalizedFile = toolFilename;
					}
					// Attempt to copy this tool into our project's tools
					if (DosUtils::FileCopyFast(sourceFile, projectRepositoryFile, true))
					{
						// Specify our new relative internalized file
						sourceFile = internalizedFile;
					}
					else
					{
						// Inform the user we failed to internalize this tool
						String *message = String::Concat(	S"The System encountered an error when attempting to internalize the specified tool.\n",
															S"TOOL: ", sourceFile);
						MOG_Prompt::PromptResponse(S"Tool Copy Failed", message);
						// Return the original tool to preserve their desired setting
						return tool;
					}
				}
			}
		}
		else
		{
			// Specify our new relative stripped path
			sourceFile = strippedTool;
		}

		// Return our new potentially modified sourceFile and it's arguments
		return String::Concat(sourceFile, S" ", arguments)->Trim();
	}
	else
	{
		// Allow LocateTool to attempt to locate the tool?
		String *locatedTool = LocateTool(sourceFile);
		if (locatedTool->Length == 0)
		{
			String *message = String::Concat(	S"The specified tool could not be located and may not function properly.\n",
												S"TOOL: ", sourceFile);
			MOG_Prompt::PromptMessage(S"Tool Warning", message);
		}
	}

	// Return the original tool to preserve their desired setting
	return tool;
}


String* MOG_ControllerSystem::StripInternalizedToolPath(String* tool)
{
	// First we need to check if this tool includes the full repository path?
	if (tool->ToLower()->StartsWith( MOG_ControllerSystem::GetSystem()->GetSystemRepositoryPath()->ToLower() ))
	{
		String *testPath = S"";

		// Test for the system tools path?
		testPath = String::Concat(MOG_ControllerSystem::GetSystem()->GetSystemToolsPath(), S"\\")->ToLower();
		if (tool->ToLower()->StartsWith( testPath ))
		{
			// Strip off the repository portion and make this a relative path
			return tool->Substring( testPath->Length );
		}

		// Get the current Project?
		MOG_Project* proj = MOG_ControllerProject::GetProject();
		if (proj != NULL)
		{
			// Test for the project tools path?
			testPath = String::Concat(proj->GetProjectToolsPath(), S"\\")->ToLower();
			if (tool->ToLower()->StartsWith( testPath ))
			{
				// Strip off the repository portion and make this a relative path
				return tool->Substring( testPath->Length );
			}
		}

		// Test for the user tools path?
		if (MOG_ControllerProject::IsUser())
		{
			testPath = String::Concat(MOG_ControllerProject::GetUser()->GetUserPath(), S"\\Tools\\")->ToLower();
			if (tool->ToLower()->StartsWith( testPath ))
			{
				// Strip off the repository portion and make this a relative path
				return tool->Substring( testPath->Length );
			}
		}
	}

	// Return what we received
	return tool;
}


bool MOG_ControllerSystem::RetaskAssignedSlaveCommand(int slaveNetworkID)
{
	MOG_Command *command = MOG_CommandFactory::Setup_RetaskCommand(slaveNetworkID);
	return mCommandManager->SendToServer(command);
}


bool MOG_ControllerSystem::KillCommandFromServer(UInt32 commandID)
{
	MOG_Command *command = MOG_CommandFactory::Setup_KillCommand(commandID);
	return mCommandManager->SendToServer(command);
}


bool MOG_ControllerSystem::LaunchSlave(int clientNetworkID)
{
	MOG_Command *command = MOG_CommandFactory::Setup_LaunchSlave(clientNetworkID);
	return mCommandManager->SendToServer(command);
}


bool MOG_ControllerSystem::RefreshApplication()
{
	MOG_Command *command = MOG_CommandFactory::Setup_RefreshApplication();
	return mCommandManager->SendToServer(command);
}


bool MOG_ControllerSystem::RefreshTools()
{
	MOG_Command *command = MOG_CommandFactory::Setup_RefreshTools();
	return mCommandManager->SendToServer(command);
}


bool MOG_ControllerSystem::RefreshProject(String *projectName)
{
	MOG_Command *command = MOG_CommandFactory::Setup_RefreshProject(projectName);
	return mCommandManager->SendToServer(command);
}


int MOG_ControllerSystem::GetNetworkID()
{
	if (MOG_ControllerSystem::GetCommandManager()->GetNetwork())
	{
		return MOG_ControllerSystem::GetCommandManager()->GetNetwork()->GetID();
	}

	return 0;
}


bool MOG_ControllerSystem::NotifySystemAlert(String *title, String *message, String *stackTrace, bool bNotifyUser)
{
	// Make sure we have initialized our CommandManager?
	if (mCommandManager)
	{
		// Setup the system notify
		MOG_Command *notify = MOG_CommandFactory::Setup_NotifySystemAlert(title, message, stackTrace, bNotifyUser);

		// get user name
		String *userName = MOG_ControllerProject::GetUserName_DefaultAdmin();

		// log in database
		MOG_DBEventAPI::AddEvent( S"Alert", notify->GetCommandTimeStamp(), title, message, stackTrace, S"-1", userName, GetComputerName(), false );

		// Send the command to the Server
		if (mCommandManager->SendToServer(notify))
		{
			return true;
		}
		else
		{
			// Since we were unable to send this to the server, we should at least process it locally so the user is aware of the problem
			return mCommandManager->CommandProcess(notify);
		}
	}

	return false;
}


bool MOG_ControllerSystem::NotifySystemError(String *title, String *message, String *stackTrace, bool bNotifyUser)
{
	// Make sure we have initialized our CommandManager?
	if (mCommandManager)
	{
		// Setup the system notify
		MOG_Command *notify = MOG_CommandFactory::Setup_NotifySystemError(title, message, stackTrace, bNotifyUser);

		// get user name
		String *userName = MOG_ControllerProject::GetUserName_DefaultAdmin();

		// log in database
		MOG_DBEventAPI::AddEvent( S"Error", notify->GetCommandTimeStamp(), title, message, stackTrace, S"-1", userName, GetComputerName(), false );

		// Send the command to the Server
		if (mCommandManager->SendToServer(notify))
		{
			return true;
		}
		else
		{
			// Since we were unable to send this to the server, we should at least process it locally so the user is aware of the problem
			return mCommandManager->CommandProcess(notify);
		}
	}

	return false;
}


bool MOG_ControllerSystem::NotifySystemException(String *title, String *message, String *stackTrace, bool bNotifyUser)
{
	// Make sure we have initialized our CommandManager?
	if (mCommandManager)
	{
		bool bNotifyUser = false;
		// Check if we are a debug build?
		if (MOG_Main::GetExecutablePath()->ToLower()->EndsWith(S"\\Debug"->ToLower()))
		{
			// Always notify the programmer of their exceptions
			bNotifyUser = true;
		}

		// Setup the system notify
		MOG_Command *notify = MOG_CommandFactory::Setup_NotifySystemException(title, message, stackTrace, bNotifyUser);

		// get user name
		String *userName = MOG_ControllerProject::GetUserName_DefaultAdmin();

		// log in database
		MOG_DBEventAPI::AddEvent( S"Exception", notify->GetCommandTimeStamp(), title, message, stackTrace, S"-1", userName, GetComputerName(), false );

		// Log to our Visual Studio output window
		System::Diagnostics::Debug::WriteLine( message );

		// Send the command to the Server
		if (mCommandManager->SendToServer(notify))
		{
			return true;
		}
		else
		{
			// Since we were unable to send this to the server, we should at least process it locally so the user is aware of the problem
			return mCommandManager->CommandProcess(notify);
		}
	}

	return false;
}

bool MOG_ControllerSystem::FileCopyEx(String* srcFilename, String* dstFilename, bool overwrite, bool clearReadOnly, BackgroundWorker* worker)
{
	bool bFailed = false;

	try
	{
		FileInfo *srcInfo = new FileInfo(srcFilename);
		FileInfo* dstInfo = new FileInfo(dstFilename);

		// Check if the files are the same?  (Must Use ToFileTime() because sometimes LastWriteTime comparisons can fail)
		if (dstInfo->LastWriteTime.ToFileTime() == srcInfo->LastWriteTime.ToFileTime() &&
			dstInfo->Length == srcInfo->Length)
		{
			// Just early out...no need to copy something that didn't change
			return true;
		}

		// Make sure the dstFile's directory exists
		if (!dstInfo->Directory->Exists)
		{
			dstInfo->Directory->Create();
		}
		// Check if the dstFile exists?
		else if (dstInfo->Exists)
		{
			// Check if the dstFile is ReadOnly?
			if (dstInfo->IsReadOnly)
			{
				// This rarely happens but if it ever does, we want to make sure we can remove this file
				dstInfo->Attributes = FileAttributes::Normal;
			}
		}

		if (worker != NULL)
		{
			worker->ReportProgress(0, new PushProgressbarCommand());
		}

		FileStream* readStream = new FileStream(srcFilename, FileMode::Open, FileAccess::Read, FileShare::Read);
		FileStream* writeStream = new FileStream(dstFilename, FileMode::Create, FileAccess::Write, FileShare::None);

		BinaryReader* reader = new BinaryReader(readStream);
		BinaryWriter* writer = new BinaryWriter(writeStream);

		Byte buffer[] = new Byte[64 * 1024];
		int bytesRead = 0;
		Int64 totalBytesRead = 0;

		while ((bytesRead = reader->Read(buffer, 0, 64 * 1024)) > 0)
		{
			totalBytesRead += bytesRead;
			
			if (worker != NULL)
			{
				worker->ReportProgress((int)(totalBytesRead * 100 / readStream->Length), NULL);
			}
			
			writer->Write(buffer, 0, bytesRead);
		}

		writer->Close();
		reader->Close();

		if (worker != NULL)
		{
			worker->ReportProgress(0, new PopProgressbarCommand());
		}

		// Make sure we transfer over the srcFilename's important fileInfo
		dstInfo->CreationTime = srcInfo->CreationTime;
		dstInfo->LastWriteTime = srcInfo->LastWriteTime;
		dstInfo->LastAccessTime = srcInfo->LastAccessTime;
		dstInfo->Attributes = (clearReadOnly) ? FileAttributes::Normal : srcInfo->Attributes;
	}
	catch(...)
	{
		bFailed = true;
	}

	if (!bFailed)
	{
		return true;
	}
	return false;
}

bool MOG_ControllerSystem::DirectoryCopyEx(String* srcPath, String* dstPath, String* label, BackgroundWorker* worker)
{
	bool success = true;
	
	if (worker != NULL)
	{
		worker->ReportProgress(0, new PushProgressbarCommand(ProgressBarStyle::Marquee));
	}
	// Get the full list of contained files
	ArrayList *files = DosUtils::FileGetRecursiveRelativeList(srcPath, S"*.*");
	if (worker != NULL)
	{
		worker->ReportProgress(0, new PopProgressbarCommand());
	}

	if (worker != NULL)
	{
		// Show the progress of this directory copy
		worker->ReportProgress(0, new PushProgressbarCommand());
	}

	String *dstLastFullPath = "";

	//Copy all the files
	for (int i = 0; i < files->Count; i++)
	{
		String *srcRelativeFile = dynamic_cast<String *>(files->Item[i]);
		String *srcRelativePath = Path::GetDirectoryName(srcRelativeFile);
		String *srcFullPath = Path::Combine(srcPath, srcRelativePath);
		String *dstFullPath = Path::Combine(dstPath, srcRelativePath);
		String *srcFile = Path::Combine(srcPath, srcRelativeFile);
		String *dstFile = Path::Combine(dstPath, srcRelativeFile);

		// Check if the directory paths just changed?
		if (String::Compare(dstLastFullPath, dstFullPath, true) != 0)
		{
			if (!DosUtils::DirectoryExistFast(dstFullPath))
			{
				DosUtils::DirectoryCreate(dstFullPath);
			}

			dstLastFullPath = dstFullPath;
		}

		if (worker != NULL)
		{
			// Inform the user about what file we are copying
			worker->ReportProgress(i * 100 / files->Count, NULL);
		}

		if (!FileCopyEx(srcFile, dstFile, true, false, worker))
		{
			success = false;
			break;
		}

		if (worker != NULL)
		{
			// Check if the useer canceled?
			if (worker->CancellationPending)
			{
				success = false;
				break;
			}
		}
	}

	if (worker != NULL)
	{
		worker->ReportProgress(0, new PopProgressbarCommand());
	}

	return success;
}


String *MOG_ControllerSystem::TokenizeMessageString(String *message)
{
	try
	{
		if (message != NULL)
		{
		  // Scan for the following system paths and replace them with a token
		  message = TokenizeMessageString(message, MOG_ControllerRepository::GetRepositoryPath(), TOKEN_Repository);
		  message = TokenizeMessageString(message, MOG_ControllerRepository::GetArchivePath(), TOKEN_Archive);
		  if (MOG_ControllerProject::GetProject())
		  {
			  message = TokenizeMessageString(message, MOG_ControllerProject::GetProject()->GetProjectUsersPath(), TOKEN_Users);
		  }
		  if (MOG_ControllerProject::GetProject())
		  {
			  message = TokenizeMessageString(message, MOG_ControllerProject::GetProject()->GetProjectPath(), TOKEN_ProjectPath);
		  }
		}
	}
	catch(...)
	{		
	}

	// Return back the tokenized message
	return message;
}


String *MOG_ControllerSystem::TokenizeMessageString(String *message, String *targetString, String *token)
{
	// Make sure we have a valid message?
	if (message)
	{
		// Strip out the Inbox Path
		while(true)
		{
			// Make sure we have something identified as a target string?
			if (targetString->Length)
			{
				// Find the position of a RepositoryPath
				int pos = message->ToLower()->IndexOf(targetString->ToLower());
				if (pos != -1)
				{
					// Rebuild the message using TOKEN_SystemRepositoryPath
					message = String::Concat(message->Substring(0, pos), token, message->Substring(pos + targetString->Length));
					// Try another attempt in case there another instance
					continue;
				}
			}

			break;
		}
	}

	// Return the tokeinzed message
	return message;
}

bool MOG_ControllerSystem::InvalidMOGCharactersCheck(String *examine, bool bWarn)
{
	// Check for invalid Windows characters
	return InvalidCharactersCheck(examine, GetInvalidMOGCharacters(), bWarn);
}

bool MOG_ControllerSystem::InvalidWindowsPathCharactersCheck(String *examine, bool bWarn)
{
	// Check for invalid Windows characters
	return InvalidCharactersCheck(examine, GetInvalidWindowsPathCharacters(), bWarn);
}

bool MOG_ControllerSystem::InvalidWindowsFilenameCharactersCheck(String *examine, bool bWarn)
{
	// Check for invalid Windows characters
	return InvalidCharactersCheck(examine, GetInvalidWindowsFilenameCharacters(), bWarn);
}

bool MOG_ControllerSystem::InvalidCharactersCheck(String *examine, String *invalidChars, bool bWarn)
{
	// Check for invalid characters
	if (examine->IndexOfAny(invalidChars->ToCharArray()) != -1)
	{
		if (bWarn)
		{
			// Inform them that it can't be changed because it contains invalid characters
			String *message = String::Concat(	S"The specified value contained invalid characters.\n",
												S"INVALID CHARACTERS: ", invalidChars, S"\n\n",
												S"The value was not set.");
			MOG_Prompt::PromptResponse(S"Invalid Characters Found", message, NULL, MOGPromptButtons::OK, MOG_ALERT_LEVEL::ALERT);
		}
		return true;
	}

	return false;
}


String *MOG_ControllerSystem::ReplaceInvalidCharacters(String *examine, String *invalidChars, String *replacementString)
{
	// Check if any specific invalidChars were specified?
	if (invalidChars->Length == 0)
	{
		// Default to the MOG invalid characters
		invalidChars = GetInvalidMOGCharacters();
	}

	// Check if there are any invlaid characters?
	if (examine->IndexOfAny(invalidChars->ToCharArray()) != -1)
	{
		// Split the string up around each invalidChar
		String *parts[] = examine->Split(invalidChars->ToCharArray());
		if( parts->Length > 1 )
		{
			// Rejoin the strings using the replacement string
			return String::Join(replacementString, parts);
		}
	}
	return examine;
}
