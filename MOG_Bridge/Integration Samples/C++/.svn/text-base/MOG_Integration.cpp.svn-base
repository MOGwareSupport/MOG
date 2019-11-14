// MOG_Integration
// Copyright (c) 2006 MOGware, Inc.
//
//

#include "MOG_Integration.h"

#if MOG_INTEGRATION

HINSTANCE	MOG_Integration::mMOGHandle = 0;
MOG_BridgeAPI *MOG_Integration::mMOGBridge = NULL;
MOGStatus MOG_Integration::mStatus = MOGStatus_Offline;
MOG_Integration::EventHandler MOG_Integration::mUserEventHandler = NULL;
void* MOG_Integration::mUserEventParameter = NULL;


// ********************************************************************************************************************************
void MOG_Integration::SetEventHandler(MOG_Integration::EventHandler handler, void* parameter)
{
	mUserEventHandler = handler;
	mUserEventParameter = parameter;
}

// Connect to the Mog Bridge.
bool MOG_Integration::ConnectToMOG()
{
	// Make sure we are disconnected from the server
	DisconnectFromMOG(MOGStatus_Offline);

	// Get a handle to the DLL module.
	if (!mMOGHandle)
	{
		char dir[MAX_PATH];
		char dllPath[MAX_PATH];
		::GetCurrentDirectory(MAX_PATH, dir);

		sprintf(dllPath, "%s%s", dir, "\\MOG_Bridge.dll");
		mMOGHandle = LoadLibrary( dllPath );

		if (!mMOGHandle)
		{
			//There was no MOG DLL in the current directory.  Try loading it from the MOG_PATH
			char* MOG_PATH = getenv("MOG_PATH");
			if (MOG_PATH)
			{
				sprintf(dllPath, "%s%s", MOG_PATH, "\\MOG_Bridge.dll");
				mMOGHandle = LoadLibrary(dllPath);
			}
		}
	}

	// Proceed with connection attempt
    if (mMOGHandle != NULL)
    {
		// Get the address of MOGPROC_GetMOGBridge
		FARPROC lpProc = GetProcAddress(mMOGHandle, "GetMOGBridge");
        if (lpProc != NULL)
        {
			// Obtain the pointer to the MOGBridge
			MogBridgeReturner GetBridge = (MogBridgeReturner)(lpProc);
			mMOGBridge = GetBridge();
			if (mMOGBridge)
			{
				// Register the MOG command handler.
				mMOGBridge->RegisterCommandHandler(HandleCommand);
				// Register the MOG event handler.
				mMOGBridge->RegisterEventHandler(HandleEvent);

				// Assume a NoClient status and rely on the callback being called during initialization to say otherwise
				mStatus = MOGStatus_NoClient;

				// Initialize the MOGBridge as an Editor
				if (mMOGBridge->InitializeEditor("Editor", false))
				{
					// Ensure we are compatible
					if (mMOGBridge->CheckAPIVersion(MOG_API_VERSION))
					{
					}
					else
					{
						// Handled in the menu refresh so the status can be updated initally
						// should be compatible enough to generate the commands and events to notify problem
						mStatus = MOGStatus_IncorrectAPIVersion;
					}
				}
				else
				{
					// Set OfflineNoServer because we failed to connect to the server
					DisconnectFromMOG(MOGStatus_OfflineNoServer);
				}
			}
			else
			{
				// Failed to obtain a valid MOGBridge object
			}
        }
		else
		{
			// Failed to locate 'GetMOGBridge' proc within the loaded MOG.dll
		}
    }
	else
	{
		// Failed to load the MOG.dll
	}

	return false;
}


bool MOG_Integration::ProcessTick()
{
	if (mMOGBridge)
	{
		return mMOGBridge->ProcessTick();
	}

	return false;
}


bool MOG_Integration::DisconnectFromMOG(MOGStatus disconnectedStatus)
{
	// Retain the indicated purpose behind this disconnection
	mStatus = MOGStatus_NoClient;

	// Dump our MOGBridge pointer
	mMOGBridge = NULL;

	return true;
}


bool MOG_Integration::HandleCommand(struct MOGAPI_BaseInfo *pCommandType)
{
	// Check if there was a callback description included in the event
	if (pCommandType->CallbackDescription)
	{
		if (stricmp(pCommandType->CallbackDescription, "LocalPackageMerge") == 0)
		{
			// Check if this Command is the PackageMergeType
			if (pCommandType->CallbackType == MOGAPI_PackageMergeType)
			{
				// Cast this Command to our more expanded PackageMergeCommandType
				MOGAPI_PackageMergeInfo *pPackageMergeCommandType = (MOGAPI_PackageMergeInfo *)pCommandType;

				// Perform the PackageMerge within this application
				// return true;
			}
		}
	}

	return false;
}


void MOG_Integration::HandleEvent(struct MOGAPI_BaseInfo *pEventType)
{
	// Check if there was a callback description included in the event
	if (pEventType->CallbackDescription)
	{
		if (stricmp(pEventType->CallbackDescription, "LoginProject") == 0)
		{
			// 'LoginProject' informs us about the project and branch
			MOG_Integration::SetMOGStatus(MOGStatus_LoggedIn);

			// Check if this Event is the LoginProjectType
			if (pEventType->CallbackType == MOGAPI_LoginProjectType)
			{
				// Cast this Event to our more expanded ViewEventType
				MOGAPI_LoginProjectInfo *pLoginProjectEventType = (MOGAPI_LoginProjectInfo *)pEventType;

				// Obtain any needed project information
			}
		}
		if (stricmp(pEventType->CallbackDescription, "LoginUser") == 0)
		{
			// 'LoginUser' informs us about who just logged in
			MOG_Integration::SetMOGStatus(MOGStatus_LoggedIn);

			// Check if this Event is the LoginUserType
			if (pEventType->CallbackType == MOGAPI_LoginUserType)
			{
				// Cast this Event to our more expanded LoginUserEventType
				MOGAPI_LoginUserInfo *pLoginUserEventType = (MOGAPI_LoginUserInfo *)pEventType;

				// Obtain any needed user information
			}
		}
		else if (stricmp(pEventType->CallbackDescription, "ShutdownClient") == 0)
		{
			// 'ShutdownClient' means we no longer have an associated MOG Client
			// Check if we still have a valid MOGBridge?
			if (GetMOGBridge())
			{
				// 'ShutdownClient' means we just lost our associate client
				MOG_Integration::SetMOGStatus(MOGStatus_NoClient);
			}
			else
			{
				// Go offline because we are completely missing our MOGBridge
				MOG_Integration::SetMOGStatus(MOGStatus_Offline);
			}
		}
		else if (stricmp(pEventType->CallbackDescription, "ConnectionLost") == 0)
		{
			// 'ConnectionLost' means we just lost our connection with the server
			MOG_Integration::SetMOGStatus(MOGStatus_NoServer);
		}
		else if (stricmp(pEventType->CallbackDescription, "ConnectionNew") == 0)
		{
			// 'ConnectionNew' means we just connected to the server and are awaiting associated client information
			MOG_Integration::SetMOGStatus(MOGStatus_NoClient);
		}
		else if (stricmp(pEventType->CallbackDescription, "ViewUpdate") == 0)
		{
			// 'ViewUpdate' means an assets was just added or removed from the local workspace
			// Check if this Event is the ViewType
			if (pEventType->CallbackType == MOGAPI_ViewType)
			{
				// Cast this Event to our more expanded ViewEventType
				MOGAPI_ViewInfo *pViewEventType = (MOGAPI_ViewInfo *)pEventType;

				if (stricmp(pViewEventType->Description, "Copied") == 0 ||
					stricmp(pViewEventType->Description, "Deleted") == 0)
				{
					// A new file has been copied, so let's find out more about it 
					// and update the game with the new file.

					// Attempt to locate the asset's name within the source or the destination
					char assetBuffer[MAX_MOG_CharArrayLength] = {0};
					if (strlen(pViewEventType->Destination))
					{
						strcpy(assetBuffer, pViewEventType->Destination);
					}
					else if (strlen(pViewEventType->Source))
					{
						strcpy(assetBuffer, pViewEventType->Source);
					}

					// Check if we have a valid AssetName?
					if (strlen(assetBuffer))
					{
						// Obtain the asset's synced file
						char filename[MAX_MOG_CharArrayLength] = {0};
						MOG_BridgeAPI* pBridge = GetMOGBridge();
						if (pBridge)
						{
							MOGBridgeHandle handle = pBridge->AssetSyncFiles_OpenObject(assetBuffer);
							if (handle != 0)
							{
								int count = pBridge->AssetSyncFiles_GetFileCount(handle);
								for (int i = 0; i < count; i++)
								{
									pBridge->AssetSyncFiles_GetFilename(handle, i, filename);
									if (mUserEventHandler)
									{
										mUserEventHandler(mUserEventParameter, filename);
									}
								}

								pBridge->AssetSyncFiles_CloseObject(handle);
							}
						}
					}
				}
			}
		}
	}
}


char *MOG_Integration::GetMOGStatusString(char *buffer)
{
	// Determin the appropriate message
	switch (MOG_Integration::GetMOGStatus())
	{
	case MOGStatus_Offline:
		buffer = strcpy(buffer, "Offline");
		break;
	case MOGStatus_OfflineNoServer:
		buffer = strcpy(buffer, "Offline - No Server");
		break;
	case MOGStatus_LoggingIn:
		buffer = strcpy(buffer, "Logging In");
		break;
	case MOGStatus_LoggedIn:
		buffer = strcpy(buffer, "Connected");
		break;
	case MOGStatus_NoClient:
		buffer = strcpy(buffer, "No Client");
		break;
	case MOGStatus_NoServer:
		buffer = strcpy(buffer, "No Server");
		break;
	case MOGStatus_IncorrectAPIVersion:
		buffer = strcpy(buffer, "IncorrectAPIVersion");
		break;
	}

	return buffer;
}


char *MOG_Integration::GetProjectName(char *buffer)
{
	if (buffer)
	{
		if (mMOGBridge)
		{
			if (!mMOGBridge->GetProjectName(buffer))
			{
				strcpy(buffer, "None");
			}
		}
	}

	return buffer;
}


char *MOG_Integration::GetBranchName(char *buffer)
{
	if (buffer)
	{
		if (mMOGBridge)
		{
			if (!mMOGBridge->GetBranchName(buffer))
			{
				strcpy(buffer, "None");
			}
		}
	}

	return buffer;
}


char *MOG_Integration::GetUserName(char *buffer)
{
	if (buffer)
	{
		if (mMOGBridge)
		{
			if (!mMOGBridge->GetUserName(buffer))
			{
				strcpy(buffer, "None");
			}
		}
	}

	return buffer;
}


#endif