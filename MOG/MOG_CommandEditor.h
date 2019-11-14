//--------------------------------------------------------------------------------
//	MOG_CommandEditor.h
//	
//	
//--------------------------------------------------------------------------------

#ifndef __MOG_COMMANDEDITOR_H__
#define __MOG_COMMANDEDITOR_H__


#include "MOG_CommandManager.h"
#include "MOG_NetworkClient.h"
#include "MOG_LockTracker.h"


namespace MOG {
namespace COMMAND {
namespace EDITOR {

public __gc class MOG_CommandEditor : public MOG_CommandManager
{
public:
	MOG_CommandEditor();

	// Support Routines
	virtual bool RegisterWithServer(void);

	virtual bool CommandExecute(MOG_Command *pCommand);

protected:
	// Contain all the persistent locks that have been sent from the server
	MOG_LockTracker *mLockTracker;

	MOG_Command *mRegisteredClient;

	// Command Routines
	virtual bool Command_RegisterClient(MOG_Command *pCommand);
	virtual bool Command_ShutdownClient(MOG_Command *pCommand);

	virtual bool Command_LoginProject(MOG_Command *pCommand);
	virtual bool Command_LoginUser(MOG_Command *pCommand);

	virtual bool Command_LocalPackageMerge(MOG_Command *pCommand);

	virtual bool Command_Complete(MOG_Command *pCommand);
};

}
}
}

using namespace MOG::COMMAND::EDITOR;

#endif	// __MOG_COMMANDCOMMANDLINE_H__

