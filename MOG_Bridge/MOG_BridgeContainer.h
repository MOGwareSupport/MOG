//--------------------------------------------------------------------------------
//	MOG_BridgeContainer.h
//	
//	This class manages any __gc pointers that MOG_Bridge cannot keep
//		inside itself.  Also, it handles the instance wrappers for
//		MOG Process and event and command callbacks.
//
//	NOTE: Other than the above, all functionality should remain in MOG_Bridge.cpp
//--------------------------------------------------------------------------------
#pragma once

#ifndef __MOG_BRIDGECONTAINER_H__
#define __MOG_BRIDGECONTAINER_H__

#include "MOG_Bridge.h"  

using namespace System::Collections;
using namespace System::Threading;


namespace MOG
{
	namespace BRIDGE
	{
																	// Defaults:
		#define EVENT_CALLBACK_DESCRIPTION_ConnectionEvents			"Connection Changed"
		#define EVENT_CALLBACK_DESCRIPTION_RegistrationEvents		"Registration Changed"
																	// Custom Populated:
		#define EVENT_CALLBACK_DESCRIPTION_LoginProject				"Logged in to Project"
		#define EVENT_CALLBACK_DESCRIPTION_LoginUser				"Logged in User"
																	// Defaults:
		#define EVENT_CALLBACK_DESCRIPTION_ViewEvents				"View Changed"
		#define EVENT_CALLBACK_DESCRIPTION_ToolsEvents				"Tools Changed"
		#define EVENT_CALLBACK_DESCRIPTION_RipEvents				"Rip Event Occured"
		#define EVENT_CALLBACK_DESCRIPTION_BlessEvents				"Bless Event Occured"
		#define EVENT_CALLBACK_DESCRIPTION_RemoveEvents				"Remove Event Occured"
																	// Custom Populated:
		#define EVENT_CALLBACK_DESCRIPTION_PackageMerge				"Package Merge"
																	// Defaults:
		#define EVENT_CALLBACK_DESCRIPTION_PackageEvents			"Other Package Event Occured (not merge)"
		#define EVENT_CALLBACK_DESCRIPTION_PostEvents				"Post Event Occured"
		#define EVENT_CALLBACK_DESCRIPTION_CleanupEvents			"Cleanup Event Occured"
		#define EVENT_CALLBACK_DESCRIPTION_LockEvents				"Lock Event Occured"
		#define EVENT_CALLBACK_DESCRIPTION_MergeEvents				"Merge Event Occured"
		#define EVENT_CALLBACK_DESCRIPTION_MessageEvents			"Message Event Occured"
		#define EVENT_CALLBACK_DESCRIPTION_BuildEvents				"Build Event Occured"
		#define EVENT_CALLBACK_DESCRIPTION_CommandContainersEvents	"Command Container Event Occured"
		#define EVENT_CALLBACK_DESCRIPTION_CriticalEventEvents		"Critical Exception Occured!"
		#define EVENT_CALLBACK_DESCRIPTION_RequestsEvents			"Request Event Occured"

		// Managed class that exposes static methods to MOG_Bridge, 
		//	and acts as a container for __gc members that are unavailable to MOG_Bridge
		public __gc class MOG_BridgeContainer
		{
		private:
			// Static container for all MOG_BridgeContainers to be used by MOG_Bridge
			static ArrayList *mContainers = new ArrayList();


			// Instance members
			MOG_Bridge *mBridge;
			String *mBridgeName;
			Thread *mMogProcess;
			// Threads for initializing specific types of MOGs
			Thread *mInitEditorThread, *mInitClientThread, *mInitCommandLineThread;
			unsigned int mEventCallbackFlags;
			bool bIsConnected;	//have we connected to the MOG Server?
			bool bIsLoggedIn;	//are we logged in?
			
			// Sets callbacks for this instance
			void SetCallbacks( void );
		public:

			MOG_BridgeContainer( MOG_Bridge *pBridge );

			// Thread methods for initialization
			bool InitializeEditor( String *name );

			// Callback Wrappers
			bool CommandCallbackWrapper( MOG_Command *pCommand );
			void PreEventCallbackWrapper( MOG_Command *pCommand );
			void EventCallbackWrapper( MOG_Command *pCommand );
			
			// Function for mMogProcess threads
			bool MogProcess(void);

			// Destructor
			bool Shutdown( void );

			int GetEventFlags( void );
			void SetEventFlags( unsigned int iFlags );

			// Other
			bool IsConnected(void);
			bool IsLoggedIn(void);

			//
			// STATIC MEMBER FUNCTIONS
			//
			// Initializes __gc components of one MOG_Bridge, returns and int, which is an 
			//	index into mBridges
			static int RegisterContainer( MOG_Bridge *pBridge );
			static void RegisterBridge(MOG_Bridge *pBridge);
			static void DestroyBridge( int baseHandle );

			// Returns MOG_BridgeContainer for MOG_Bridge to user
			static MOG_BridgeContainer *GetContainer(int iBaseHandle);

		};
	}
}

#endif __MOG_BRIDGECONTAINER_H__