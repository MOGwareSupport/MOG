// MOG_Integration
// Copyright (c) 2006 MOGware, Inc.
//
//


#ifndef __MOGINTEGRATION_H__
#define __MOGINTEGRATION_H__

#define MOG_INTEGRATION 1

#if MOG_INTEGRATION

#include <stdio.h> 
#include <windows.h> 

#include "MOG_BridgeAPI.h"


enum MOGStatus
{
	MOGStatus_Offline,
	MOGStatus_OfflineNoServer,
	MOGStatus_LoggingIn,
	MOGStatus_LoggedIn,
	MOGStatus_NoClient,
	MOGStatus_NoServer,
	MOGStatus_IncorrectAPIVersion,
};


class MOG_Integration
{
public:
	typedef void (*EventHandler)(void* parameter, char* filename);

	// Initialization
	static void SetEventHandler(EventHandler handler, void* parameter);
	static bool ConnectToMOG();

	// Process
	static bool ProcessTick();

	// Shutdown
	static bool DisconnectFromMOG(MOGStatus disconnectedStatus);

	// Accessor Functions
	static MOG_BridgeAPI *GetMOGBridge()					{ return mMOGBridge; };

	// MOG Status
	static MOGStatus GetMOGStatus()							{ return mStatus; };
	static void SetMOGStatus(MOGStatus status)				{ mStatus = status; };
	static char *GetMOGStatusString(char *buffer);

	// Utility Functions
	static char *GetProjectName(char *buffer);
	static char *GetBranchName(char *buffer);
	static char *GetUserName(char *buffer);

private:
	// Member Variables
	static HINSTANCE		mMOGHandle;
	static MOG_BridgeAPI*	mMOGBridge;
	static MOGStatus		mStatus;

	static EventHandler		mUserEventHandler;
	static void*			mUserEventParameter;

	// Default Callback Handlers
	static bool __cdecl HandleCommand(struct MOGAPI_BaseInfo *pCommandType);
	static void __cdecl HandleEvent(struct MOGAPI_BaseInfo *pEventType);
};

#endif

#endif // __MOGINTEGRATION_H__

