//--------------------------------------------------------------------------------
//	MOG_CommandCommandLine.h
//	
//	
//--------------------------------------------------------------------------------

#ifndef __MOG_COMMANDCOMMANDLINE_H__
#define __MOG_COMMANDCOMMANDLINE_H__


#include "MOG_CommandManager.h"
#include "MOG_NetworkClient.h"


namespace MOG {
namespace COMMAND {
namespace COMMANDLINE {

public __gc class MOG_CommandCommandLine : public MOG_CommandManager
{
public:
	MOG_CommandCommandLine();

	// Support Routines
	virtual bool RegisterWithServer(void);
	virtual bool CommandExecute(MOG_Command *pCommand);

protected:
	MOG_Command *mRegisteredClient;

	// Command Routines
	virtual bool Command_RegisterClient(MOG_Command *pCommand);
	virtual bool Command_ShutdownCommandLine(MOG_Command *pCommand);

	virtual bool Command_LoginProject(MOG_Command *pCommand);
	virtual bool Command_LoginUser(MOG_Command *pCommand);

	virtual bool Command_AssetRipRequest(MOG_Command *pCommand);

	virtual bool Command_Bless(MOG_Command *pCommand);

	virtual bool Command_LockReadRequest(MOG_Command *pCommand);
	virtual bool Command_LockWriteRequest(MOG_Command *pCommand);

	virtual bool Command_Complete(MOG_Command *pCommand);
};

}
}
}

using namespace MOG::COMMAND::COMMANDLINE;

#endif	// __MOG_COMMANDCOMMANDLINE_H__

