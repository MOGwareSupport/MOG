//--------------------------------------------------------------------------------
//	MOG_CommandManager.h
//	
//	
//--------------------------------------------------------------------------------

#ifndef __MOG_COMMANDMANAGER_H__
#define __MOG_COMMANDMANAGER_H__

#include "MOG_Define.h"
#include "MOG_Filename.h"
#include "MOG_Command.h"
#include "MOG_NetworkServer.h"
#include "MOG_NetworkClient.h"
#include "MOG_Ini.h"

using namespace System::ComponentModel;

namespace MOG {
namespace COMMAND {

public __delegate void MOG_CallbackCommandEvent(MOG_Command *command);
public __delegate bool MOG_CallbackCommandProcess(MOG_Command *command);

public __gc class MOG_Callbacks
{
public:
	MOG_CallbackCommandEvent		*mPreEventCallback;
	MOG_CallbackCommandEvent		*mEventCallback;
	MOG_CallbackCommandProcess		*mCommandCallback;

	MOG_Callbacks()
	{
		mPreEventCallback	= NULL;
		mEventCallback		= NULL;
		mCommandCallback	= NULL;
	}
};


public __gc class MOG_CommandManager
{
public:
	MOG_CommandManager(void);
	~MOG_CommandManager(void);

	virtual bool Shutdown();
	virtual bool Initialize();

	// Utility Routines
	MOG_Network* GetNetwork(void)					{	return mNetwork;	}

	void SetCallbacks(MOG_Callbacks *callbacks)		{	mCallbacks = callbacks;	}

	virtual void Process(void);
	virtual bool ReadConnections(void);

	ArrayList *GetCommandsList()					{ return new ArrayList(mCommands); };
	virtual bool AddCommand(MOG_Command *pCommand);
	virtual bool CommandProcess(MOG_Command *pCommand);
	virtual bool CommandExecute(MOG_Command *pCommand);
	virtual bool SendToServer(MOG_Command *pCommand);
	virtual bool SendToServerBlocking(MOG_Command *pCommand);
	virtual bool LockWaitDialog(MOG_Command *pCommand);

	virtual bool RegisterWithServer(void);
	virtual bool GetDatabaseLocks();

	virtual bool NotifyServerOfActiveViews(String *tabName, String *userName, String *platformName, String *workspaceDirectory);

// TODO In the future we should add these into a user prefs module
	// Spawn Dos command varibles
	bool			mCommandLineHideWindow;
	bool			mUseOnlyMySlaves;

	MOG_Callbacks*	GetCallbacks(void)					{ return mCallbacks; }

	// LogFile Functions
	int				GetMaxLogLength()					{ return mMaxLogLength; }
	void			SetMaxLogLength(int len)			{ mMaxLogLength = len; }
	void			SetOutputLog(String *log)			{ mOutputLog = log; }
	bool			AppendMessageToOutputLog(String *message);
	String*			GetOutputLog()						{ return mOutputLog; }

	// Client message box stuff
	ArrayList*		staticButtons;

protected:
	// Thread related variables
	static Thread *gThread;
	static MOG_Command *gCommand;
	static bool gCommandLineHideWindow;
	// Thread related member functions
	static void InitializeThreadVariables(MOG_Command *pCommand, bool hideCommandWindow);
	static bool AbortThread(void);

	MOG_Network*	mNetwork;

	MOG_Callbacks*	mCallbacks;

	// Contains all the commands pending processing
	ArrayList *mCommands;

	// LogFile Variables
	String*	mOutputLog;
	int		mMaxLogLength;

	// Command Routines
	virtual bool Command_None(MOG_Command *pCommand);

	virtual bool Command_ConnectionNew(MOG_Command *pCommand);
	virtual bool Command_ConnectionLost(MOG_Command *pCommand);
	virtual bool Command_ConnectionKill(MOG_Command *pCommand);

	virtual bool Command_SQLConnection(MOG_Command *pCommand);
	virtual bool Command_MOGRepository(MOG_Command *pCommand);

	virtual bool Command_ActiveViews(MOG_Command *pCommand);
	virtual bool Command_ViewUpdate(MOG_Command *pCommand);

	virtual bool Command_RefreshApplication(MOG_Command *pCommand);
	virtual bool Command_RefreshTools(MOG_Command *pCommand);
	virtual bool Command_RefreshProject(MOG_Command *pCommand);

	virtual bool Command_LockCopy(MOG_Command *pCommand);
	virtual bool Command_LockMove(MOG_Command *pCommand);
	virtual bool Command_LockReadRequest(MOG_Command *pCommand);
	virtual bool Command_LockWriteRequest(MOG_Command *pCommand);
	virtual bool Command_LockReadRelease(MOG_Command *pCommand);
	virtual bool Command_LockWriteRelease(MOG_Command *pCommand);
	virtual bool Command_LockQuery(MOG_Command *pCommand);

	virtual bool Command_Complete(MOG_Command *pCommand);
	virtual bool Command_Postpone(MOG_Command *pCommand);
	virtual bool Command_Failed(MOG_Command *pCommand);
	virtual bool Command_NotifySystemAlert(MOG_Command *pCommand);
	virtual bool Command_NotifySystemError(MOG_Command *pCommand);
	virtual bool Command_NotifySystemException(MOG_Command *pCommand);
	
	virtual bool Command_LaunchSlave(MOG_Command *pCommand);

	void LockWaitDialog_Worker(Object* sender, DoWorkEventArgs* e);
};

}//COMMAND
}//MOG

using namespace MOG::COMMAND;

#endif	// __MOG_COMMANDMANAGER_H__

