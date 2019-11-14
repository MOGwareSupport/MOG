//--------------------------------------------------------------------------------
//	MOG_LockTracker.h
//	
//	
//--------------------------------------------------------------------------------

#ifndef __MOG_LockTracker_H__
#define __MOG_LockTracker_H__

#include "MOG_CommandManager.h"


using namespace System::Collections::Specialized;



namespace MOG {
namespace COMMAND {
namespace TOOLS {

public __gc class MOG_LockTracker
{
public:
	MOG_LockTracker(MOG_CommandManager* commandManager, bool useLockWaitDialog);

	ArrayList* GetLocks()		{	return new ArrayList(mPersistentLocks->Values);	};

	virtual bool CommandExecute(MOG_Command *pCommand);

protected:
	//Always keep a handle to the command manager that owns us
	MOG_CommandManager* mCommandManager;

	// Contain all the persistent locks that have been sent from the server
	HybridDictionary *mPersistentLocks;
	bool mUseLockWaitDialog;

	// Command Routines
	virtual bool Command_LockCopy(MOG_Command *pCommand);
	virtual bool Command_LockMove(MOG_Command *pCommand);

	virtual bool Command_LockRequest(MOG_Command *pCommand);
	virtual bool Command_LockRelease(MOG_Command *pCommand);
	virtual bool Command_LockQuery(MOG_Command *pCommand);

	virtual bool Command_LockPersistentNotify(MOG_Command *pCommand);


	// Utility Functions
	virtual bool PersistentLockQuery(MOG_Command *pCommand);
	virtual bool UpdateLocks(MOG_Command *pCommand);
};

}
}
}

using namespace MOG::COMMAND::TOOLS;

#endif	// __MOG_LockTracker_H__

