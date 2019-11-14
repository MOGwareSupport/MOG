///////////////////////////////////////////////
// MOG_BridgeContainer.cpp
// 
// Starting from here, to add a new function to the MOG_Bridge.cpp
//  for use by external functions:
//	1) Add the global variable or static function you 
//		would like to use.  Program as normal Managed C++
//	2) Add a DllExport function in MOG_Bridge.h and MOG_Bridge.cpp
//		so it is available for external functions.
//		Program as unmanged C/C++.
//
//	Note:  Remember that to use anything other than an 
//			unmanaged type in MOG_Bridge, you *must*
//			use the __gc compiler option, which explicitly
//			indicates that you are using a pointer
//			to a managed type.
//
///////////////////////////////////////////////

#using <mscorlib.dll>

#include "MOG_BridgeContainer.h"
#include "MOG_BridgeAPI.h"

using namespace System::Collections;
using namespace System;

using namespace MOG;
using namespace MOG::BRIDGE;
using namespace MOG::CONTROLLER::CONTROLLERSYSTEM;
using namespace MOG::REPORT;

//************************
//
//
//
MOG_BridgeContainer::MOG_BridgeContainer(MOG_Bridge *pBridge)
{
	mBridge = pBridge;
	bIsConnected = false;
	bIsLoggedIn = false;
}

// Created to register our Container for this pBridge outside of the MainInit thread...
int MOG_BridgeContainer::RegisterContainer( MOG_Bridge *pBridge )
{
	MOG_BridgeContainer *pContainer = new MOG_BridgeContainer(pBridge);
	mContainers->Add( pContainer );

	// Return zero-index for MOG_Bridge to reference this container
	return (mContainers->Count - 1);
}

// 'Registers' a new MOG_Bridge by assigning it to the 
void MOG_BridgeContainer::RegisterBridge(MOG_Bridge *pBridge)
{
	// Assign globals, for pBase and pBridge.
	//mBaseGlobal = pBase;
	//mBridgeGlobal = pBridge;
	MOG_BridgeContainer *pContainer = GetContainer( pBridge->GetHandle() );
}

void MOG_BridgeContainer::DestroyBridge( int baseHandle )
{
	mContainers->Remove( mContainers->get_Item( baseHandle ) );
}

bool MOG_BridgeContainer::InitializeEditor(String *name)
{
	mBridgeName = name;

	if (MOG_Main::Init_Editor(S"", mBridgeName))
	{
		bIsConnected = true;
		SetCallbacks();

		// Attempt to obtain the Login info from the MOG Server
		if (MOG_ControllerSystem::RequireNetworkIDInitialization())
		{
			bIsLoggedIn = true;
		}
		
		return true;
	}

	return false;
}


// Adapted from MOGMainForm.cs 3970 (2005-07-15)
bool MOG_BridgeContainer::MogProcess(void)
{
	bool bSuccessful = true;

	// If we are not connected, always return false.
	if( !bIsConnected )
		return false;

	try
	{
		bSuccessful = MOG_Main::Process();
	}
	catch(Exception *ex)
	{
		bSuccessful = false;
		MOG_Report::ReportSilent("MogProcess", ex->Message, ex->StackTrace);
	}

	return bSuccessful;
}

bool MOG_BridgeContainer::Shutdown( void )
{
	try
	{
		MOG_Main::Shutdown();
	}
	catch(Exception *e)
	{
		e->ToString();
		return false;
	}
	return true;
}

MOG_BridgeContainer *MOG_BridgeContainer::GetContainer(int iBaseHandle)
{
	return __try_cast<MOG_BridgeContainer*>( mContainers->get_Item(iBaseHandle) );
}

bool MOG_BridgeContainer::CommandCallbackWrapper(MOG_Command *pCommand)
{
	// Return value
	MOGAPI_BaseInfo *pBaseInfo = NULL;
	// Values wrapped by pBaseInfo to be cast back to their type by user
	MOGAPI_PackageMergeInfo *pPMInfo;
	MOG_COMMAND_TYPE commandType = pCommand->GetCommandType();
	String *descriptionStr = (__box(commandType))->ToString();
	descriptionStr = descriptionStr->Replace( S"MOG_", S"" );
	char* description = MOG_Bridge::AllocNativeString( descriptionStr );
	char* destination = MOG_Bridge::AllocNativeString( pCommand->GetDestination() );
	char* source = MOG_Bridge::AllocNativeString( pCommand->GetSource() );

	if (commandType == MOG_COMMAND_EditorPackageMergeTask)
	{
		pBaseInfo = new MOGAPI_PackageMergeInfo;
		pBaseInfo->CallbackType = MOGAPI_PackageMergeType;
		pPMInfo = ((MOGAPI_PackageMergeInfo*)pBaseInfo);
		pPMInfo->DestinationFile = destination;
		pPMInfo->SourceFile = source;
	}
	else
	{
		pBaseInfo = new MOGAPI_BaseInfo;
		pBaseInfo->CallbackType = MOGAPI_BaseType;
	}

	// Always set ProjectName, BranchName, and CommandDescription
	pBaseInfo->CallbackDescription = description;

	try
	{
		return mBridge->HandleCommand( pBaseInfo );
	}
	catch(Exception *ex)
	{
		// Inform the user we crashed in their application
		String *message = String::Concat(	S"The 3rd party application has thrown an unhandled exception during the Command Callback Event.\n",
											S"Exception Message: ", ex->Message, S"\n");
		MOG_Report::ReportMessage("3rd Party Appplication Error", message, ex->StackTrace, MOG::PROMPT::MOG_ALERT_LEVEL::CRITICAL);
	}
	__finally
	{
		MOG_Bridge::FreeNativeString(description);
		MOG_Bridge::FreeNativeString(destination);
		MOG_Bridge::FreeNativeString(source);
	}

	return false;
}

void MOG_BridgeContainer::SetCallbacks( void )
{
	MOG_Callbacks *callbacks = new MOG_Callbacks();
	callbacks->mPreEventCallback = new MOG_CallbackCommandEvent(this, &MOG_BridgeContainer::PreEventCallbackWrapper);
	callbacks->mEventCallback = new MOG_CallbackCommandEvent(this, &MOG_BridgeContainer::EventCallbackWrapper);
	callbacks->mCommandCallback = new MOG_CallbackCommandProcess(this, &MOG_BridgeContainer::CommandCallbackWrapper);
	MOG_ControllerSystem::GetCommandManager()->SetCallbacks(callbacks);
}

void MOG_BridgeContainer::PreEventCallbackWrapper(MOG_Command *pCommand)
{
	// Set description to the Command Type used for MOG_Command
	String *commandEnumStr = __box( pCommand->GetCommandType() )->ToString();
	commandEnumStr = commandEnumStr->Replace(S"MOG_COMMAND_", S"");
	char* commandType = MOG_Bridge::AllocNativeString( commandEnumStr );

	switch (pCommand->GetCommandType())
	{
	case MOG_COMMAND_LocalPackageMerge:
		break;
	}

	MOG_Bridge::FreeNativeString(commandType);
}

void MOG_BridgeContainer::EventCallbackWrapper(MOG_Command *pCommand)
{
	bool doEventCallback = false;
	// Value given to HandleEvent()
	MOGAPI_BaseInfo *pBaseInfo = NULL;

	String *commandEnumStr = __box( pCommand->GetCommandType() )->ToString();
	commandEnumStr = commandEnumStr->Replace(S"MOG_COMMAND_", S"");
	char* commandType = MOG_Bridge::AllocNativeString( commandEnumStr );
	char* description = MOG_Bridge::AllocNativeString( pCommand->GetDescription() );
	char* username = MOG_Bridge::AllocNativeString( pCommand->GetUserName() );
	char* tab = MOG_Bridge::AllocNativeString( pCommand->GetTab() );
	char* platform = MOG_Bridge::AllocNativeString( pCommand->GetPlatform() );
	char* workingDirectory = MOG_Bridge::AllocNativeString( pCommand->GetWorkingDirectory() );
	char* source = MOG_Bridge::AllocNativeString( pCommand->GetSource() );
	char* destination = MOG_Bridge::AllocNativeString( pCommand->GetDestination() );
	char* project = MOG_Bridge::AllocNativeString( pCommand->GetProject() );
	char* branch = MOG_Bridge::AllocNativeString( pCommand->GetBranch() );

	switch (pCommand->GetCommandType())
	{
		// Check for EVENT_CALLBACK_FLAG_ConnectionEvents
		case MOG_COMMAND_ConnectionNew:
		case MOG_COMMAND_ConnectionLost:
		case MOG_COMMAND_ConnectionKill:
		{
			if (mEventCallbackFlags & EVENT_CALLBACK_FLAG_ConnectionEvents)
			{
				doEventCallback = true;
				//description = EVENT_CALLBACK_DESCRIPTION_ConnectionEvents;
			}
			break;
		}

		//
		// Check for EVENT_CALLBACK_FLAG_RegistrationEvents
		case MOG_COMMAND_RegisterClient:
		case MOG_COMMAND_ShutdownClient:
		case MOG_COMMAND_RegisterSlave:
		case MOG_COMMAND_ShutdownSlave:
		case MOG_COMMAND_RegisterEditor:
		case MOG_COMMAND_ShutdownEditor:
		case MOG_COMMAND_RegisterCommandLine:
		case MOG_COMMAND_ShutdownCommandLine:
		{
			if(mEventCallbackFlags & EVENT_CALLBACK_FLAG_RegistrationEvents)
			{
				doEventCallback = true;
				//description = EVENT_CALLBACK_DESCRIPTION_RegistrationEvents;
			}
			break;
		}
		
		//
		// Check for EVENT_CALLBACK_FLAG_LoginEvents
		case MOG_COMMAND_LoginProject:
		{
			if(mEventCallbackFlags & EVENT_CALLBACK_FLAG_LoginEvents)
			{
				doEventCallback = true;

				pBaseInfo = new MOGAPI_LoginProjectInfo;
				pBaseInfo->CallbackType = MOGAPI_LoginProjectType;

				// Initialize the project's icons (clear them first)
				MOG_ControlsLibrary::Utils::MogUtil_AssetIcons::ClassIconsClear();
				MOG_ControlsLibrary::Utils::MogUtil_AssetIcons::ClassIconInitialize();
			}
			break;
		}
		case MOG_COMMAND_LoginUser:
		{
			if(mEventCallbackFlags & EVENT_CALLBACK_FLAG_LoginEvents)
			{
				doEventCallback = true;

				pBaseInfo = new MOGAPI_LoginUserInfo;
				MOGAPI_LoginUserInfo *pLUInfo = ((MOGAPI_LoginUserInfo*)pBaseInfo);
				pLUInfo->CallbackType = MOGAPI_LoginUserType;
				pLUInfo->UserName = username;
			}
			break;
		}

		//
		// Check for EVENT_CALLBACK_FLAG_ViewEvents
		case MOG_COMMAND_ActiveViews:
		case MOG_COMMAND_ViewUpdate:
		{
			if(mEventCallbackFlags & EVENT_CALLBACK_FLAG_ViewEvents)
			{
				doEventCallback = true;

				pBaseInfo = new MOGAPI_ViewInfo;
				MOGAPI_ViewInfo *pVInfo = ((MOGAPI_ViewInfo*)pBaseInfo);
				pVInfo->CallbackType = MOGAPI_ViewType;
				pVInfo->Tab = tab;
				pVInfo->UserName = username;
				pVInfo->Platform = platform;
				pVInfo->WorkingDirectory = workingDirectory;
				pVInfo->Source = source;
				pVInfo->Destination = destination;
				pVInfo->Description = description;
				pVInfo->SourceFile = source;
				pVInfo->DestinationFile = destination;
			}
			break;
		}

		//
		// Check for EVENT_CALLBACK_FLAG_RefreshEvents
		case MOG_COMMAND_RefreshApplication:
		case MOG_COMMAND_RefreshTools:
		case MOG_COMMAND_RefreshProject:
		{
			if(mEventCallbackFlags & EVENT_CALLBACK_FLAG_RefreshEvents)
			{
				doEventCallback = true;
			}
			break;
		}

		//
		// Check for EVENT_CALLBACK_FLAG_RipEvents
		case MOG_COMMAND_AssetRipRequest:
		case MOG_COMMAND_AssetProcessed:
		case MOG_COMMAND_SlaveTask:
		{
			if(mEventCallbackFlags & EVENT_CALLBACK_FLAG_RipEvents)
			{
				doEventCallback = true;
			}
			break;
		}

		//
		// Check for EVENT_CALLBACK_FLAG_BlessEvents
		case MOG_COMMAND_ReinstanceAssetRevision:
		case MOG_COMMAND_Bless:
		{
			if(mEventCallbackFlags & EVENT_CALLBACK_FLAG_BlessEvents)
			{
				doEventCallback = true;
			}
			break;
		}

		//
		// Check for EVENT_CALLBACK_FLAG_RemoveEvents
		case MOG_COMMAND_RemoveAssetFromProject:
		{
			if(mEventCallbackFlags & EVENT_CALLBACK_FLAG_RemoveEvents)
			{
				doEventCallback = true;
			}
			break;
		}

		//
		// Check for EVENT_CALLBACK_FLAG_PackageEvents
		case MOG_COMMAND_LocalPackageMerge:
		{
			if(mEventCallbackFlags & EVENT_CALLBACK_FLAG_PackageEvents)
			{
				doEventCallback = true;

				pBaseInfo = new MOGAPI_PackageMergeInfo;
				MOGAPI_PackageMergeInfo *pPMInfo = ((MOGAPI_PackageMergeInfo*)pBaseInfo);
				pPMInfo->CallbackType = MOGAPI_PackageMergeType;
				pPMInfo->SourceFile = source;
				pPMInfo->DestinationFile = destination;
			}
			break;
		}
		//
		// Check for EVENT_CALLBACK_FLAG_PostEvents
		case MOG_COMMAND_Post:
		{
			if(mEventCallbackFlags & EVENT_CALLBACK_FLAG_PostEvents)
			{
				doEventCallback = true;
			}
			break;
		}

		//
		// Check for EVENT_CALLBACK_FLAG_CleanupEvents
		case MOG_COMMAND_Archive:
		case MOG_COMMAND_ScheduleArchive:
		case MOG_COMMAND_CleanTrash:
		{
			if(mEventCallbackFlags & EVENT_CALLBACK_FLAG_CleanupEvents)
			{
				doEventCallback = true;
			}
			break;
		}

		//
		// Check for EVENT_CALLBACK_FLAG_LockEvents
		case MOG_COMMAND_LockCopy:
		case MOG_COMMAND_LockMove:
		case MOG_COMMAND_LockReadRequest:
		case MOG_COMMAND_LockReadRelease:
		case MOG_COMMAND_LockWriteRequest:
		case MOG_COMMAND_LockWriteRelease:
		case MOG_COMMAND_LockReadQuery:
		case MOG_COMMAND_LockWriteQuery:
		case MOG_COMMAND_LockRequestRelease:
		case MOG_COMMAND_LockPersistentNotify:
		case MOG_COMMAND_LockWatch:
		{
			if(mEventCallbackFlags & EVENT_CALLBACK_FLAG_LockEvents)
			{
				doEventCallback = true;
			}
			break;
		}

		//
		// Check for EVENT_CALLBACK_FLAG_MessageEvents
		case MOG_COMMAND_NetworkBroadcast:
		case MOG_COMMAND_InstantMessage:
		{
			if(mEventCallbackFlags & EVENT_CALLBACK_FLAG_MessageEvents)
			{
				doEventCallback = true;
			}
			break;
		}

		//
		// Check for EVENT_CALLBACK_FLAG_BuildEvents
		case MOG_COMMAND_BuildFull:
		case MOG_COMMAND_Build:
		case MOG_COMMAND_NewBranch:
		{
			if(mEventCallbackFlags & EVENT_CALLBACK_FLAG_BuildEvents)
			{
				doEventCallback = true;
			}
			break;
		}

		//
		// Check for EVENT_CALLBACK_FLAG_CommandContainersEvents
		case MOG_COMMAND_Complete:
		case MOG_COMMAND_Postpone:
		case MOG_COMMAND_Failed:
		{
			if(mEventCallbackFlags & EVENT_CALLBACK_FLAG_CommandContainersEvents)
			{
				doEventCallback = true;
			}
			break;
		}

		//
		// Check for EVENT_CALLBACK_FLAG_CriticalEventEvents
		case MOG_COMMAND_NotifySystemAlert:
		case MOG_COMMAND_NotifySystemError:
		case MOG_COMMAND_NotifySystemException:
		{
			if(mEventCallbackFlags & EVENT_CALLBACK_FLAG_CriticalEventEvents)
			{
				doEventCallback = true;
			}
			break;
		}

		//
		// Check for EVENT_CALLBACK_FLAG_RequestsEvents
		case MOG_COMMAND_RequestActiveCommands:
		case MOG_COMMAND_NotifyActiveCommand:
		case MOG_COMMAND_RequestActiveLocks:
		case MOG_COMMAND_NotifyActiveLock:
		case MOG_COMMAND_RequestActiveConnections:
		case MOG_COMMAND_NotifyActiveConnection:
		{
			if(mEventCallbackFlags & EVENT_CALLBACK_FLAG_RequestsEvents)
			{
				doEventCallback = true;
			}
			break;
		}
	}

	// Check if we decided to perform the callback?
	if (doEventCallback)
	{
		// If pBaseInfo has not been initialized, initialize it
		if(!pBaseInfo)
		{
			pBaseInfo = new MOGAPI_BaseInfo;
			pBaseInfo->CallbackType = MOGAPI_BaseType;
		}
		
		if (pBaseInfo && pCommand)
		{
			// Always set ProjectName, BranchName, and CommandDescription
			pBaseInfo->ProjectName = project;
			pBaseInfo->BranchName = branch;
			pBaseInfo->CallbackDescription = commandType;

			try
			{
				mBridge->HandleEvent( pBaseInfo );
			}
			catch(Exception *ex)
			{
				// Inform the user we crashed in their application
				String *message = String::Concat(	S"The 3rd party application has thrown an unhandled exception during the Event Callback Event.\n",
													S"Exception Message: ", ex->Message, S"\n");
				MOG_Report::ReportMessage("3rd Party Appplication Error", message, ex->StackTrace, MOG::PROMPT::MOG_ALERT_LEVEL::CRITICAL);
			}
		}
	}

	MOG_Bridge::FreeNativeString(commandType);
	MOG_Bridge::FreeNativeString(description);
	MOG_Bridge::FreeNativeString(username);
	MOG_Bridge::FreeNativeString(tab);
	MOG_Bridge::FreeNativeString(platform);
	MOG_Bridge::FreeNativeString(workingDirectory);
	MOG_Bridge::FreeNativeString(source);
	MOG_Bridge::FreeNativeString(destination);
	MOG_Bridge::FreeNativeString(project);
	MOG_Bridge::FreeNativeString(branch);
}

int MOG_BridgeContainer::GetEventFlags( void )
{
	return mEventCallbackFlags;
}

void MOG_BridgeContainer::SetEventFlags( unsigned int iFlags )
{
	mEventCallbackFlags = iFlags;
}

bool MOG_BridgeContainer::IsConnected( )
{
	return bIsConnected;
}

bool MOG_BridgeContainer::IsLoggedIn()
{
	return bIsLoggedIn;
}

