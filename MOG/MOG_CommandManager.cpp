//--------------------------------------------------------------------------------
//	MOG_CommandManager.cpp
//	
//	
//--------------------------------------------------------------------------------

#include "StdAfx.h"

#include "MOG_Define.h"
#include "MOG_StringUtils.h"
#include "MOG_Main.h"
#include "MOG_ControllerAsset.h"
#include "MOG_DosUtils.h"
#include "MOG_Ini.h"
#include "MOG_Tokens.h"
#include "MOG_ControllerProject.h"
#include "MOG_ControllerSystem.h"
#include "MOG_ControllerSyncData.h"
#include "MOG_Progress.h"
#include "MOG_DBInboxAPI.h"
#include "MOG_DBAssetAPI.h"
#include "MOG_CommandFactory.h"

#include "MOG_CommandManager.h"


using namespace System;
using namespace System::Threading;
using namespace System::Diagnostics;
using namespace System::Collections::Generic;



MOG_CommandManager::MOG_CommandManager(void)
{
	mNetwork = NULL;

	mCallbacks = new MOG_Callbacks();

	mCommands = new ArrayList();

	mMaxLogLength = 1024 * 10;
	mOutputLog = "";

	mCommandLineHideWindow = true;
}


MOG_CommandManager::~MOG_CommandManager(void)
{
}


bool MOG_CommandManager::Shutdown()
{
	MOG_NetworkClient *pClient = dynamic_cast<MOG_NetworkClient*>(GetNetwork());
	if (pClient)
	{
		pClient->CloseConnectionToServer();
	}

	return true;
}


bool MOG_CommandManager::AbortThread(void)
{
	try
	{

	// Check if we have a thread actively running?
	if (gThread)
	{
		// Get a local handle now because we need to clear the slave's global handle
		Thread *thread = gThread;

		// Always re-initialize the thread variables
		InitializeThreadVariables(NULL, true);

		// Finally, abort the thread using our local variable
		//thread->Abort();
	}

	}
	catch (...)
	{
	}

	return true;
}


void MOG_CommandManager::InitializeThreadVariables(MOG_Command *pCommand, bool hideCommandWindow)
{
	// Initialize our thread related globals
	gThread = NULL;

	// Initialize the gCommand...Make sure to clone pCommand
	gCommand = NULL;
	if (pCommand)
	{
		gCommand = new MOG_Command();
		gCommand->Clone(pCommand);
	}

	gCommandLineHideWindow = hideCommandWindow;
}


bool MOG_CommandManager::Initialize()
{
	if (MOG_ControllerSystem::GetSystem()->LoadServerInformation())
	{
		MOG_NetworkClient* pClient = dynamic_cast<MOG_NetworkClient*>(mNetwork);
		if (pClient)
		{
			// Attempt the reconnect?
			if (pClient->Initialize())
			{
				// Initialize the network variables
				MOG_ControllerSystem::InitializeNetworkInfo();

				// Indicate that we successfully initialized the system
				return true;
			}					
		}				
	}
	return false;
}


void MOG_CommandManager::Process(void)
{
	//Send all pending packets
	if (mNetwork)
	{
		if (mNetwork->SendPendingPackets() == -1)
		{
			// Inform the command manager that the connection was just lost
			MOG_Command *connectionLost = MOG_CommandFactory::Setup_ConnectionLost();
			CommandProcess(connectionLost);
		}
	}

	// Check if we have any commands?
	if (mCommands && mCommands->Count)
	{
		// Loop through all our commands...don't always increment our counter
		for (int c = 0; c < mCommands->Count; /* c++ */)
		{
			// Attempt to process this command
			MOG_Command *pCommand = __try_cast<MOG_Command*>(mCommands->Item[c]);
			if (pCommand && CommandProcess(pCommand))
			{
				// Remove this processed command from our array
				mCommands->RemoveAt(c);
				// Don't increment our counter because the Remove will collapse our array wround us
				continue;
			}

			// Increment over this unprocessed command
			c++;
		}
	}
}


bool MOG_CommandManager::AppendMessageToOutputLog(String *message)
{
	// Append the commandLog to the mOutputLog
	mOutputLog = String::Concat(message, mOutputLog);
	if (mOutputLog->Length > GetMaxLogLength())
	{
		mOutputLog = mOutputLog->Substring(0, GetMaxLogLength());
	}

	return true;
}


bool MOG_CommandManager::CommandProcess(MOG_Command *pCommand)
{
	bool processed = false;

	if (pCommand)
	{
		try
		{
			// If callback exist, call it
			if (mCallbacks && mCallbacks->mPreEventCallback)
			{
				mCallbacks->mPreEventCallback->Invoke(pCommand);
			}

			// Attempt to execute this command
			processed = CommandExecute(pCommand);
			if (processed)
			{
				// Indicate that this command has been completed
				pCommand->SetCompleted(processed);

				// If callback exist, call it
				if (mCallbacks && mCallbacks->mEventCallback)
				{
					mCallbacks->mEventCallback->Invoke(pCommand);
				}
			}
		}
		catch(Exception *e)
		{
			// Send this Exception back to the server
			MOG_Report::ReportMessage(S"MOG Command", e->Message, e->StackTrace, MOG_ALERT_LEVEL::CRITICAL);
			// Eat this command or else it will just keep throwing forever
			processed = true;
		}
	}

	return processed;
}


bool MOG_CommandManager::CommandExecute(MOG_Command *pCommand)
{
	bool processed = false;

	if (pCommand)
	{
		switch (pCommand->GetCommandType())
		{
		case MOG_COMMAND_None:
			processed = Command_None(pCommand);
			break;
		case MOG_COMMAND_ConnectionNew:
			processed = Command_ConnectionNew(pCommand);
			break;
		case MOG_COMMAND_ConnectionLost:
			processed = Command_ConnectionLost(pCommand);
			break;
		case MOG_COMMAND_ConnectionKill:
			processed = Command_ConnectionKill(pCommand);
			break;
		case MOG_COMMAND_SQLConnection:
			processed = Command_SQLConnection(pCommand);
			break;
		case MOG_COMMAND_MOGRepository:
			processed = Command_MOGRepository(pCommand);
			break;
		case MOG_COMMAND_ActiveViews:
			processed = Command_ActiveViews(pCommand);
			break;
		case MOG_COMMAND_ViewUpdate:
			processed = Command_ViewUpdate(pCommand);
			break;
		case MOG_COMMAND_RefreshApplication:
			processed = Command_RefreshApplication(pCommand);
			break;
		case MOG_COMMAND_RefreshTools:
			processed = Command_RefreshTools(pCommand);
			break;
		case MOG_COMMAND_RefreshProject:
			processed = Command_RefreshProject(pCommand);
			break;
		case MOG_COMMAND_LockCopy:
			processed = Command_LockCopy(pCommand);
			break;
		case MOG_COMMAND_LockMove:
			processed = Command_LockMove(pCommand);
			break;
		case MOG_COMMAND_LockReadRequest:
			processed = Command_LockReadRequest(pCommand);
			break;
		case MOG_COMMAND_LockReadRelease:
			processed = Command_LockReadRelease(pCommand);
			break;
		case MOG_COMMAND_LockWriteRequest:
			processed = Command_LockWriteRequest(pCommand);
			break;
		case MOG_COMMAND_LockWriteRelease:
			processed = Command_LockWriteRelease(pCommand);
			break;
		case MOG_COMMAND_LockWriteQuery:
		case MOG_COMMAND_LockReadQuery:
			processed = Command_LockQuery(pCommand);
			break;
		case MOG_COMMAND_NetworkBroadcast:
			// Always eat these commands
			processed = true;
			break;
		case MOG_COMMAND_Complete:
			processed = Command_Complete(pCommand);
			break;
		case MOG_COMMAND_Postpone:
			processed = Command_Postpone(pCommand);
			break;
		case MOG_COMMAND_Failed:
			processed = Command_Failed(pCommand);
			break;
		case MOG_COMMAND_NotifySystemAlert:
			processed = Command_NotifySystemAlert(pCommand);
			break;
		case MOG_COMMAND_NotifySystemError:
			processed = Command_NotifySystemError(pCommand);
			break;
		case MOG_COMMAND_NotifySystemException:
			processed = Command_NotifySystemException(pCommand);
			break;
		case MOG_COMMAND_LaunchSlave:
			processed = Command_LaunchSlave(pCommand);
			break;
		default:
			String *message = String::Concat(S"MOG_CommandManager - ", pCommand->ToString(), S" command not implemented");
			MOG_Report::ReportSilent(S"MOG Command", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
			// Always eat the bad command
			processed = true;
			break;
		}
	}

	return processed;
}


bool MOG_CommandManager::AddCommand(MOG_Command *pCommand)
{
	// Make sure this command is valid
	if(pCommand)
	{
		mCommands->Add(pCommand);
		return true;
	}

	return false;
}


bool MOG_CommandManager::SendToServer(MOG_Command *pCommand)
{
	MOG_ASSERT_THROW(!MOG_ControllerSystem::IsServerMode(), MOG_Exception::MOG_EXCEPTION_IllegalOperation, "MOG - CommandManager - Server should never call SendToServer()");

	// Check our connection to the server
	MOG_NetworkClient* pClient = dynamic_cast<MOG_NetworkClient*>(mNetwork);
	if (pClient)
	{
		if (!pClient->Connected())
		{
			// Determin the seriousness of not having a server for this command
			switch(pCommand->GetCommandType())
			{
				case MOG_COMMAND_RegisterClient:
				case MOG_COMMAND_RegisterEditor:
				case MOG_COMMAND_RegisterCommandLine:
				case MOG_COMMAND_AssetRipRequest:
				case MOG_COMMAND_AssetProcessed:
				case MOG_COMMAND_Bless:
				case MOG_COMMAND_RemoveAssetFromProject:
				case MOG_COMMAND_NetworkPackageMerge:
				case MOG_COMMAND_LocalPackageMerge:
				case MOG_COMMAND_NetworkPackageRebuild:
				case MOG_COMMAND_LocalPackageRebuild:
				case MOG_COMMAND_LockCopy:
				case MOG_COMMAND_LockMove:
				case MOG_COMMAND_LockReadRequest:
				case MOG_COMMAND_LockReadRelease:
				case MOG_COMMAND_LockWriteRequest:
				case MOG_COMMAND_LockWriteRelease:
				case MOG_COMMAND_BuildFull:
				case MOG_COMMAND_Build:
					MOG_Prompt::PromptMessage("Network Error", String::Concat(S"No server present to process the '", pCommand->ToString(), "' command\n", pCommand->GetAssetFilename()->GetFullFilename()), S"", PROMPT::MOG_ALERT_LEVEL::ALERT);
					break;
				default:
					break;
			}
			return false;
		}

		// Make sure we have a valid NetworkID?
		if (!pClient->GetID())
		{
			// Never attempt to send a command when we have no NetworkID!
			return false;
		}

		// We always need to populate certain information within the command...
		pCommand->SetSystemDefaultCommandSettings();

		// Serialize the NetworkPacket
		NetworkPacket *packet = pCommand->Serialize();

		// Send to the Server
		return pClient->SendToServer(packet);
	}

	return false;
}


bool MOG_CommandManager::SendToServerBlocking(MOG_Command *pCommand)
{
	MOG_ASSERT_THROW(!MOG_ControllerSystem::IsServerMode(), MOG_Exception::MOG_EXCEPTION_IllegalOperation, "MOG - CommandManager - Server should never call SendToServerBlocking()");
	Thread *currentThread = Thread::CurrentThread;
	MOG_Time *time = new MOG_Time();

	// We always need to populate certain information within the command...
	pCommand->SetSystemDefaultCommandSettings();
	// Force the command's NetworkID to ours so we are garenteed to hear back from the server
	pCommand->SetNetworkID(MOG_ControllerSystem::GetNetworkID());

	Monitor::Enter(this);
	__try
	{
		// Specify this is a blocking command and that we will wait for the reply
		pCommand->SetBlocking(true);
		// Send to the Server Blocking
		if (SendToServer(pCommand))
		{
			bool blockedReply = false;
			DateTime blockedReplyStartTime = DateTime::Now;

			// Keep waiting until we receive the reply from our blocked command
			while (!blockedReply)
			{
				// Read the packets from the network
				MOG_NetworkClient* pClient = dynamic_cast<MOG_NetworkClient*>(mNetwork);
				if (pClient)
				{
					//Send all pending packages to the server while we wait for the reply
					if (pClient->SendPendingPackets() == -1)
					{
						// Inform the command manager that the connection was just lost
						MOG_Command *connectionLost = MOG_CommandFactory::Setup_ConnectionLost();
						CommandProcess(connectionLost);
						break;
					}

					//Read packets and check for a blocking reply
					ArrayList *packets = pClient->ReadPackets();
					if (packets)
					{
						// Process each packet
						for (int p = 0; p < packets->Count; p++)
						{
							NetworkPacket *packet = __try_cast<NetworkPacket *>(packets->Item[p]);
							MOG_Command *command;

							// Deserialize the packet
							command = MOG_Command::Deserialize(packet);
							// Watch for the reply from our blocked request
							if (command->IsBlocking())
							{
								blockedReply = true;
								// Change pCommand to reference the reply from our blocked command
								pCommand->Clone(command);
							}
							// Add the other non-blocking commands for later processing
							else
							{
								AddCommand(command);
							}
						}
					}
					else
					{
						// Inform the command manager that the connection was just lost
						MOG_Command *connectionLost = MOG_CommandFactory::Setup_ConnectionLost();
						CommandProcess(connectionLost);
						break;
					}
				}

// JohnRen - This makes it really hard when we debug!!!
//				// Update the time
//				time->UpdateTime();
//				// Check if we have been waiting for more than 1 second?
//				if (time->GetTimeStampInt() - blockedReplyStartTime > 10000)
//				{
//					// We shouldn't keep waiting forever
//					break;
//				}

				// Gracefully wait for the next loop
				currentThread->Sleep(1);
			}
		}
	}
	__finally
	{
		Monitor::Exit(this);
	}

	// Return whether or not the blocked command was completed
	return pCommand->IsCompleted();
}


bool MOG_CommandManager::RegisterWithServer(void)
{
	String *message = S"MOG_CommandManager - 'RegisterWithServer' command not implemented";
	MOG_Report::ReportMessage(S"GetDatabaseLocks", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
	return true;
}

bool MOG_CommandManager::GetDatabaseLocks(void)
{
	String *message = S"MOG_CommandManager - 'GetDatabaseLocks' command not implemented";
	MOG_Report::ReportMessage(S"GetDatabaseLocks", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
	return false;
}

bool MOG_CommandManager::NotifyServerOfActiveViews(String *tabName, String *userName, String *platformName, String *workspaceDirectory)
{
	return false;
}

bool MOG_CommandManager::LockWaitDialog(MOG_Command *pCommand)
{
	// Make sure we are connected
	MOG_NetworkClient* pClient = dynamic_cast<MOG_NetworkClient*>(mNetwork);
	if (pClient && !pClient->Connected())
	{
		MOG_Prompt::PromptMessage("Network Error", String::Concat(S"No server present to process the '", pCommand->ToString(), "' command\n", pCommand->GetAssetFilename()->GetFullFilename()), Environment::StackTrace, PROMPT::MOG_ALERT_LEVEL::ALERT);
		return false;
	}

	// We will try the lock once before we start to loop with the dialog
	// Send the command to the server blocking
	bool locked = SendToServerBlocking(pCommand);
	if (!locked)
	{
		// Initialize the dialog
		MOG_Command* currentLock = pCommand->GetCommand();
		String* assetName = pCommand->GetAssetFilename()->GetAssetFullName();
		String* text = String::Concat(	S"ASSET: ", assetName, S"\n",
										S"USER: ", currentLock->GetUserName(), S"\n",
										S"MACHINE: ", currentLock->GetComputerName(), S"\n",
										S"COMMENT: ", currentLock->GetDescription());

		ProgressDialog* progress = new ProgressDialog(S"Waiting for lock", text, new DoWorkEventHandler(this, &MOG_CommandManager::LockWaitDialog_Worker), pCommand, false);
		if (progress->ShowDialog() == DialogResult::Cancel)
		{
			return false;
		}
	}

	return true;
}

void MOG_CommandManager::LockWaitDialog_Worker(Object* sender, DoWorkEventArgs* e)
{
	BackgroundWorker* worker = dynamic_cast<BackgroundWorker*>(sender);
	MOG_Command* command = dynamic_cast<MOG_Command*>(e->Argument);
	
	// Wait for the Lock command to be processed
	while (!worker->CancellationPending)
	{
		// Send the command to the server blocking
		bool locked = SendToServerBlocking(command);
		if (locked)
		{
			break;
		}

		Thread::Sleep(500);
	}
}

bool MOG_CommandManager::ReadConnections(void)
{
	ArrayList *packets;
	MOG_NetworkClient* pClient = dynamic_cast<MOG_NetworkClient*>(mNetwork);	
	if (pClient)
	{
		// Check our connection to the server
		if (!pClient->Connected())
		{
			static int timerAutoReconnect = 0;
			static bool skipAutoReconnect = false;

			// Wait a few seconds before automatically trying to reconnect to the server
			if (timerAutoReconnect++ < 100)
			{
				return false;
			}

			// Reset the timerAutoReconnect
			timerAutoReconnect = 0;

			// Try to connect to the server agian
			Initialize();
			// Did we fail to connect to the server?
			if (!pClient->Connected())
			{
				return false;
			}
		}


		Monitor::Enter(this);
		__try
		{
			// Read the packets from the network
			packets = pClient->ReadPackets();
			if (packets)
			{
				// Process each packet
				for (int p = 0; p < packets->Count; p++)
				{
					// Deserialize the packet
					MOG_Command *command = MOG_Command::Deserialize(__try_cast<NetworkPacket *>(packets->Item[p]));

					// Add the command to the CommandManager
					AddCommand(command);
				}

				return true;
			}
			else
			{
				// Inform the command manager that the connection was just lost
				MOG_Command *connectionLost = MOG_CommandFactory::Setup_ConnectionLost();
				CommandProcess(connectionLost);
			}
		}
		__finally
		{
			Monitor::Exit(this);
		}
	}

	return false;
}

bool MOG_CommandManager::Command_None(MOG_Command *pCommand)
{
	// Always eat this command
	return true;
}


bool MOG_CommandManager::Command_ConnectionNew(MOG_Command *pCommand)
{
	// Establish our new NetworkID
	MOG_NetworkClient* pClient = dynamic_cast<MOG_NetworkClient*>(mNetwork);
	if (pClient)
	{
		pClient->SetID(pCommand->GetNetworkID());

		// Initialize our Database
		MOG_ControllerSystem::InitializeDatabase(pCommand->GetDescription(), S"", S"");

		// Record the disabled feature list indicated in the ConnectionNew command
		MOG_Main::SetDisabledFeatureList(pCommand->GetVersion());

		// We just established our new connection to the server
		// Send over a series of commands to the server
		RegisterWithServer();
	}

	// Always eat this command
	return true;
}


bool MOG_CommandManager::Command_ConnectionLost(MOG_Command *pCommand)
{
	// Always eat this command
	return true;
}


bool MOG_CommandManager::Command_ConnectionKill(MOG_Command *pCommand)
{
	// We have just been killed...Shut us down
	MOG_Main::Shutdown();

	// Always eat the command
	return true;
}


bool MOG_CommandManager::Command_SQLConnection(MOG_Command *pCommand)
{
	// Initialize our Database
	MOG_ControllerSystem::InitializeDatabase(pCommand->GetDescription(), S"", S"");

	// Always eat the command
	return true;
}


bool MOG_CommandManager::Command_MOGRepository(MOG_Command *pCommand)
{
	// Save new Repository info to our local ini
	String *localConfigFilename = String::Concat(MOG_Main::GetExecutablePath(), S"\\MOG.ini");
	MOG_Ini *localConfigFile = new MOG_Ini();
	if (localConfigFile->Open(localConfigFilename, FileShare::ReadWrite))
	{
		localConfigFile->PutString("MOG", "SystemRepositoryPath", pCommand->GetWorkingDirectory());
		localConfigFile->Close();

		// Fixup our loader's ini as well...if it exists
		String *loaderFilename = String::Concat(pCommand->GetWorkingDirectory(), S"\\..\\Loader.ini");
		if (DosUtils::FileExistFast(loaderFilename))
		{
			// Fixup the SystemRepositoryPath in this ini
			MOG_Ini *loaderFile = new MOG_Ini();
			if (loaderFile->Open(loaderFilename, FileShare::ReadWrite))
			{
				loaderFile->PutString("MOG", "SystemRepositoryPath", pCommand->GetWorkingDirectory());
				loaderFile->Close();
			}
		}

		// Refresh our system and project
		String *projectName = MOG_ControllerProject::GetProjectName();
		String *branchName = MOG_ControllerProject::GetBranchName();
		String *userName = MOG_ControllerProject::GetUserName();
		if (projectName->Length)
		{
			// Simply login in again and it will refresh everything
			MOG_ControllerProject::LoginProject(projectName, branchName);
			MOG_ControllerProject::LoginUser(userName);
		}
	}

	// Always eat the command
	return true;
}


bool MOG_CommandManager::Command_ActiveViews(MOG_Command *pCommand)
{
	// Setup our active items
	if (pCommand->GetTab()->Length)
	{
		MOG_ControllerProject::SetActiveTabName(pCommand->GetTab());
	}

	if (pCommand->GetUserName()->Length)
	{
		MOG_ControllerProject::SetActiveUserName(pCommand->GetUserName());
	}

	if (pCommand->GetWorkingDirectory()->Length)
	{
		MOG_ControllerSyncData *syncData = new MOG_ControllerSyncData(S"Unspecified", pCommand->GetWorkingDirectory(), MOG_ControllerProject::GetProjectName(), pCommand->GetBranch(), pCommand->GetPlatform(), MOG_ControllerProject::GetUserName());
		MOG_ControllerProject::SetCurrentSyncDataController(syncData);
	}

	// Always eat this command...We don't care because the server tracks this from the client
	return true;
}


bool MOG_CommandManager::Command_ViewUpdate(MOG_Command *pCommand)
{
	//set up the add and delete filenames so we can make sure to pass null if we we don't have a value.
	MOG_Filename *addFilename = NULL;
	MOG_Filename *delFilename = NULL;

	// Load the MOG_Filenames for the source and destination of this command
	if(pCommand->GetDestination()->Length > 0)
	{
		//make a new mog filename if we have a value.
		addFilename = new MOG_Filename(pCommand->GetDestination());
	}
	if(pCommand->GetSource()->Length > 0)
	{
		//make a new mog filename if we have a value.
		delFilename = new MOG_Filename(pCommand->GetSource());
	}

	// Update our cache so it won't fall out of sync
	MOG_DBInboxAPI::UpdateCache(addFilename, delFilename);

	// Always eat the command
	return true;
}


bool MOG_CommandManager::Command_RefreshApplication(MOG_Command *pCommand)
{
	// Always eat this command
	return true;
}


bool MOG_CommandManager::Command_RefreshTools(MOG_Command *pCommand)
{
	// Always eat this command
	return true;
}


bool MOG_CommandManager::Command_RefreshProject(MOG_Command *pCommand)
{
	// Check if this RefreshProject is related to our project?
	if (String::Compare(pCommand->GetProject(), MOG_ControllerProject::GetProjectName(), true) == 0)
	{
		// Gather our active information
		String *userName = MOG_ControllerProject::GetUserName();
		String *projectName = MOG_ControllerProject::GetProjectName();
		String *branchName = MOG_ControllerProject::GetBranchName();

		// Login will refresh the project information
		MOG_ControllerProject::LoginProject(projectName, branchName);
		MOG_ControllerProject::LoginUser(userName);
	}

	// Always eat this command
	return true;
}


bool MOG_CommandManager::Command_LockCopy(MOG_Command *pCommand)
{
	MOG_Command *command = MOG_CommandFactory::Setup_LockReadRequest(pCommand->GetSource(), String::Concat("MOG_CommandManager::Command_LockCopy(Source) - ", pCommand->GetDescription()));
	bool completed = false;

	// Get the ReadLock on the source
	if (Command_LockReadRequest(command))
	{
		// Get the WriteLock on the target
		command = MOG_CommandFactory::Setup_LockWriteRequest(pCommand->GetDestination(), String::Concat("MOG_CommandManager::Command_LockCopy(Target) - ", pCommand->GetDescription()));
		if (Command_LockWriteRequest(command))
		{
			// Copy the asset tree
			DosUtils::Copy(pCommand->GetSource(), pCommand->GetDestination(), false);

			// Release the destination lock
			command = MOG_CommandFactory::Setup_LockWriteRelease(pCommand->GetDestination());
			Command_LockWriteRelease(command);

			// Indicate we successfully processed the command
			completed = true;
		}
		// Release the source lock
		command = MOG_CommandFactory::Setup_LockReadRelease(pCommand->GetSource());
		Command_LockReadRelease(command);
	}

	// Indicate the status of the command
	if (completed)
	{
		return true;
	}

	return false;
}


bool MOG_CommandManager::Command_LockMove(MOG_Command *pCommand)
{
	MOG_Command *command = MOG_CommandFactory::Setup_LockWriteRequest(pCommand->GetSource(), String::Concat(S"MOG_CommandManager::Command_LockMove(Source) - ", pCommand->GetDescription()));
	bool completed = false;

	// Get the WriteLock on the source
	if (Command_LockWriteRequest(command))
	{
		// Get the WriteLock on the target
		command = MOG_CommandFactory::Setup_LockWriteRequest(pCommand->GetDestination(), String::Concat("MOG_CommandManager::Command_LockMove(Target) - ", pCommand->GetDescription()));
		if (Command_LockWriteRequest(command))
		{
			// Move the asset
			DosUtils::Move(pCommand->GetSource(), pCommand->GetDestination());

			// Release the destination lock
			command = MOG_CommandFactory::Setup_LockWriteRelease(pCommand->GetDestination());
			Command_LockWriteRelease(command);

			// Indicate we successfully processed the command
			completed = true;
		}
		// Release the source lock
		command = MOG_CommandFactory::Setup_LockWriteRelease(pCommand->GetSource());
		Command_LockWriteRelease(command);
	}

	// Indicate the status of the command
	if (completed)
	{
		return true;
	}

	return false;
}


bool MOG_CommandManager::Command_LockReadRequest(MOG_Command *pCommand)
{
	// Send the command to the server
	return SendToServerBlocking(pCommand);
}


bool MOG_CommandManager::Command_LockReadRelease(MOG_Command *pCommand)
{
	// Send the command to the server
	return SendToServerBlocking(pCommand);
}


bool MOG_CommandManager::Command_LockWriteRequest(MOG_Command *pCommand)
{
	// Send the command to the server
	return SendToServerBlocking(pCommand);
}


bool MOG_CommandManager::Command_LockWriteRelease(MOG_Command *pCommand)
{
	// Send the command to the server
	return SendToServerBlocking(pCommand);
}


bool MOG_CommandManager::Command_LockQuery(MOG_Command *pCommand)
{
	// We will try the lock once before we start to loop with the dialog
	// Send the command to the server blocking
	return SendToServerBlocking(pCommand);	
}


bool MOG_CommandManager::Command_Complete(MOG_Command *pCommand)
{
	// Check the subcommand
	MOG_Command* pSubcommand = pCommand->GetCommand();
	if (pSubcommand)
	{
		// Check if this is a subcommand that we care about?
		switch (pSubcommand->GetCommandType())
		{
			case MOG_COMMAND_RemoveAssetFromProject:
				// Just to be on the safe side, this is a good event to cause us to flush our cache for anything related to this asset
				MOG_ControllerSystem::GetDB()->GetDBCache()->GetAssetNameCache()->RemoveSetFromCacheByName(pSubcommand->GetAssetFilename()->GetAssetFullName());
				break;
		}
	}

	// Always eat this command
	return true;
}


bool MOG_CommandManager::Command_Postpone(MOG_Command *pCommand)
{
	// Re-add this command for later processing...

	// Always eat this command
	return true;
}


bool MOG_CommandManager::Command_Failed(MOG_Command *pCommand)
{
	// Always eat this command
	return true;
}


bool MOG_CommandManager::Command_NotifySystemAlert(MOG_Command *pCommand)
{
	// Always eat this command
	return true;
}


bool MOG_CommandManager::Command_NotifySystemError(MOG_Command *pCommand)
{
	// Always eat this command
	return true;
}


bool MOG_CommandManager::Command_NotifySystemException(MOG_Command *pCommand)
{
	// Always eat this command
	return true;
}

bool MOG_CommandManager::Command_LaunchSlave(MOG_Command *pCommand)
{
	// Get the correct running directory
	String *slaveCommand = String::Concat(MOG_Main::GetExecutablePath(), S"\\MOG_Slave.exe");

	// Attempt to spawn the Slave?
	if (DosUtils::FileExistFast(slaveCommand))
	{
		if (DosUtils::SpawnCommand(slaveCommand, ""))
		{
		}
	}

	// Always eat this command.
	return true;
}
