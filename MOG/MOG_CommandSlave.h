//--------------------------------------------------------------------------------
//	MOG_CommandSlave.h
//	
//	
//--------------------------------------------------------------------------------

#ifndef __MOG_COMMANDSLAVE_H__
#define __MOG_COMMANDSLAVE_H__


#include "MOG_CommandManager.h"
#include "MOG_NetworkClient.h"
#include "MOG_Ini.h"
#include "MOG_ControllerAsset.h"
#include "MOG_DBPackageCommandAPI.h"
#include "MOG_ControllerPackage.h"

using namespace System::Threading;



namespace MOG {
namespace COMMAND {
namespace SLAVE {



public __gc class MOG_CommandSlave : public MOG_CommandManager
{
public:
	MOG_CommandSlave();
	bool Shutdown(void);

	// Utility Routines
	virtual bool RegisterWithServer(void);

	virtual bool CommandExecute(MOG_Command *pCommand);

	// Spawn dos command utility routines
	bool GetCommandLineHideWindow() { return mCommandLineHideWindow; };
	void SetCommandLineHideWindow(bool val) { mCommandLineHideWindow = val; };

protected:
	// Thread related member functions
	static void Thread_Process_AssetRipRequest(void);
	static void Thread_Process_SlaveTask(void);
	static void Thread_Process_ReinstanceAssetRevision(void);
	static void Thread_Process_RemoveAssetFromProject(void);
	static void Thread_Process_Bless(void);
	static void Thread_Process_NetworkPackageMerge(void);
	static void Thread_Process_PackageRebuild(void);
	static void Thread_Process_Post(void);
	static void Thread_Process_Build(void);

	// Slave Output Log
	static String *FormatSlaveLogCommandComment(MOG_Command *pCommand, String *comment);

	// Command Routines
	virtual bool Command_ShutdownSlave(MOG_Command *pCommand);
	virtual bool Command_ConnectionKill(MOG_Command *pCommand);

	virtual bool Command_LoginProject(MOG_Command *pCommand);
	virtual bool Command_LoginUser(MOG_Command *pCommand);

	virtual bool Command_AssetRipRequest(MOG_Command *pCommand);
	virtual bool Command_AssetProcessed(MOG_Command *pCommand);

	virtual bool Command_SlaveTask(MOG_Command *pCommand);
	virtual bool Command_ReinstanceAssetRevision(MOG_Command *pCommand);
	virtual bool Command_Bless(MOG_Command *pCommand);
	virtual bool Command_RemoveAssetFromProject(MOG_Command *pCommand);
	virtual bool Command_NetworkPackageMerge(MOG_Command *pCommand);
	virtual bool Command_NetworkPackageRebuild(MOG_Command *pCommand);
	virtual bool Command_Post(MOG_Command *pCommand);
	virtual bool Command_Archive(MOG_Command *pCommand);
	virtual bool Command_ScheduleArchive(MOG_Command *pCommand);

	virtual bool Command_LockReadRequest(MOG_Command *pCommand);
	virtual bool Command_LockWriteRequest(MOG_Command *pCommand);
	virtual bool Command_LockReadRelease(MOG_Command *pCommand);
	virtual bool Command_LockWriteRelease(MOG_Command *pCommand);

	virtual bool Command_BuildFull(MOG_Command *pCommand);
	virtual bool Command_Build(MOG_Command *pCommand);

private:
	static bool RemoveAssetFromProject_RemoveAssetFromBranch(MOG_Filename *assetFilename, String *jobLabel);
	static bool RemoveAssetFromProject_RemoveAssetRevision(MOG_Filename *assetFilename, String *removeRevision, bool bReportErrors);
	static bool RemoveAssetFromProject_RemoveAllAssetRevisions(MOG_Filename *assetFilename);
	static bool RemoveAssetFromProject_RemoveAssetName(MOG_Filename *assetFilename);
	static bool RemoveAssetFromProject_RestampAnyCollidingAssets(MOG_ControllerAsset* asset, String* jobLabel, String* validSlaves);
};



}
}
}

using namespace MOG::COMMAND::SLAVE;

#endif	// __MOG_COMMANDSLAVE_H__

