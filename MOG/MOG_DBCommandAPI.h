//--------------------------------------------------------------------------------
//	MOG_DBCommandAPI.h
//	
//	
//--------------------------------------------------------------------------------
#ifndef __MOG_DBCOMMANDAPI_H__
#define __MOG_DBCOMMANDAPI_H__


#using <mscorlib.dll>
#using <system.dll>
#using <System.Data.dll>

#include "stdlib.h"
#include "MOG_Define.h"
#include "MOG_Command.h"
#include "MOG_DBAPI.h"


using namespace System;
using namespace System::Data;
using namespace System::Data::SqlClient;



namespace MOG {
namespace DATABASE {

public __gc class MOG_DBCommandAPI
{
private:
	static MOG_Command *QueryCommand(String *selectString);
	static ArrayList *QueryCommands(String *selectString);

	static MOG_Command *SQLReadCommand(SqlDataReader *myReader);
	static bool SQLWriteCommand(MOG_Command *command);
	static MOG_Command *SQLCreateCommand(MOG_Command *command);

public:
	// Lock Related API
	static bool AddLock(MOG_Command *pCommand);
	static bool RemoveLock(MOG_Command *pCommand);
	static bool RemoveAllLocks();
	static ArrayList *GetAllLocks();
};


}
}

using namespace MOG;
using namespace MOG::DATABASE;

#endif


