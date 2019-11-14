//--------------------------------------------------------------------------------
//	MOG_CommandClient.h
//	
//	
//--------------------------------------------------------------------------------

#ifndef __MOG_COMMANDCLIENT_H__
#define __MOG_COMMANDCLIENT_H__


#include "MOG_CommandManager.h"
#include "MOG_NetworkClient.h"
#include "MOG_LockTracker.h"


namespace MOG {
namespace COMMAND {
namespace CLIENT {

public __gc class MOG_CommandClient : public MOG_CommandManager
{
public:
	MOG_CommandClient();

	virtual bool RegisterWithServer(void);

	virtual bool CommandExecute(MOG_Command *pCommand);

	virtual bool NotifyServerOfActiveViews(String *tabName, String *userName, String *platformName, String *workspaceDirectory);

	ArrayList* GetLocks()	{	return mLockTracker->GetLocks();	};

protected:
	// Thread related member functions
	virtual void Thread_Process_LocalPackageMerge(Object *object);

	// Contain all the persistent locks that have been sent from the server
	MOG_LockTracker *mLockTracker;

	// Support Routines	
	bool AddDeletePackageCommand(String *packageFilename, String *platform, String *packages[]);

	// Command Routines
	virtual bool Command_ShutdownClient(MOG_Command *pCommand);
	virtual bool Command_ShutdownSlave(MOG_Command *pCommand);
	virtual bool Command_RegisterEditor(MOG_Command *pCommand);
	virtual bool Command_ShutdownEditor(MOG_Command *pCommand);
	virtual bool Command_ConnectionKill(MOG_Command *pCommand);

	virtual bool Command_LoginProject(MOG_Command *pCommand);
	virtual bool Command_LoginUser(MOG_Command *pCommand);

	virtual bool Command_AssetRipRequest(MOG_Command *pCommand);

	virtual bool Command_Bless(MOG_Command *pCommand);

	virtual bool Command_LocalPackageMerge(MOG_Command *pCommand);
	virtual bool Command_LocalPackageRebuild(MOG_Command *pCommand);

	virtual bool Command_NetworkBroadcast(MOG_Command *pCommand);

	virtual bool Command_InstantMessage(MOG_Command *pCommand);

	virtual bool Command_NotifyActiveCommand(MOG_Command *pCommand);
	virtual bool Command_NotifyActiveLock(MOG_Command *pCommand);
	virtual bool Command_NotifyActiveConnection(MOG_Command *pCommand);

	virtual bool Command_AutomatedTesting(MOG_Command *pCommand);

	virtual bool Command_Complete(MOG_Command *pCommand);
};

}
}
}

using namespace MOG::COMMAND::CLIENT;

#endif	// __MOG_COMMANDCLIENT_H__

