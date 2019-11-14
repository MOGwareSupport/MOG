//--------------------------------------------------------------------------------
//	MOG_CommandFactory.h
//	
//	
//--------------------------------------------------------------------------------
#ifndef __MOG_CommandFactory_H__
#define __MOG_CommandFactory_H__


#include "MOG_Define.h"
#include "MOG_Ini.h"
#include "MOG_Command.h"
#include "MOG_Properties.h"



namespace MOG 
{
namespace COMMAND {

public __gc class MOG_CommandFactory
{
public:

	static MOG_Command *Setup_None(void);

	static MOG_Command *Setup_ConnectionNew(int networkID, String *connectionString, String *disabledFeatureList);
	static MOG_Command * Setup_ConnectionLost(void);
	static MOG_Command * Setup_ConnectionKill(int networkID);

	static MOG_Command *Setup_RegisterClient(String *name, String *executablePath);
	static MOG_Command *Setup_ShutdownClient(void);
	static MOG_Command *Setup_ShutdownClient(MOG_Command *pRegisteredCommand);
	static MOG_Command *Setup_RegisterSlave(String *name, String *executablePath);
	static MOG_Command *Setup_ShutdownSlave(void);
	static MOG_Command *Setup_ShutdownSlave(MOG_Command *pRegisteredCommand);
	static MOG_Command *Setup_ShutdownSlave(int networkID);
	static MOG_Command *Setup_ShutdownSlave(String *computerName);
	static MOG_Command *Setup_RegisterEditor(String *name, String *executablePath);
	static MOG_Command *Setup_ShutdownEditor(void);
	static MOG_Command *Setup_ShutdownEditor(MOG_Command *pRegisteredCommand);
	static MOG_Command *Setup_RegisterCommandLine(String *name, String *executablePath);
	static MOG_Command *Setup_ShutdownCommandLine(void);
	static MOG_Command *Setup_ShutdownCommandLine(MOG_Command *pRegisteredCommand);

	static MOG_Command *Setup_SQLConnection(String *connectionString);
	static MOG_Command *Setup_MOGRepository(String *reposiotryPath);

	static MOG_Command *Setup_License();
	static MOG_Command *Setup_LoginProject(String *loginProject, String *loginBranch);
	static MOG_Command *Setup_LoginUser(String *loginUser);

	static MOG_Command *Setup_ActiveViews(String *tabName, String *userName, String *platformName, String *workspaceDirectory);
	static MOG_Command *Setup_ViewUpdate(String *source, String *target, MOG_AssetStatusType status);
	static MOG_Command *Setup_ViewUpdate(String *source, String *target, String *status, MOG_Properties *properties);

	static MOG_Command *Setup_RefreshApplication();
	static MOG_Command *Setup_RefreshTools();
	static MOG_Command *Setup_RefreshProject(String *projectName);

	static MOG_Command *Setup_AssetRipRequest(String *jobLabel, MOG_Filename *assetFullFilename, String *validSlaves);
	static MOG_Command *Setup_AssetProcessed(int originatingNetworkID, String *originatingUserName, String *jobLabel, MOG_Filename *assetFullFilename, String *validSlaves);
	static MOG_Command *Setup_SlaveTask(int originatingNetworkID, String *originatingUserName, String *jobLabel, MOG_Filename *assetFilename, String *platformName, String *workingDirectory, String *ripper, ArrayList *environment, String *showWindow, String *source, String *destination, String *description, String *validSlaves);

	static MOG_Command *Setup_ReinstanceAssetRevision(MOG_Filename* newAssetRevisionFilename, MOG_Filename* oldAssetRevisionFilename, String* jobLabel, ArrayList* propertiesToRemove, ArrayList* propertiesToAdd, String* options);
	static MOG_Command *Setup_ReinstanceAssetRevision(MOG_Filename* newAssetRevisionFilename, MOG_Filename* oldAssetRevisionFilename, String* jobLabel, ArrayList* propertiesToRemove, ArrayList* propertiesToAdd, String* options, MOG_Filename* targetPackageFilename);

	static MOG_Command* Setup_Bless(MOG_Filename* assetFilename, String* jobLabel, String* validSlaves);
	static MOG_Command* Setup_Bless(MOG_Filename* assetFilename, String* jobLabel, String* validSlaves, bool skipPackaging, bool skipArchiving);
	static MOG_Command* Setup_Bless(MOG_Filename* assetFilename, String* jobLabel, String* validSlaves, bool skipPackaging, bool skipArchiving, MOG_Filename* targetPackageFilename);

	static MOG_Command *Setup_RemoveAssetFromProject(MOG_Filename *assetFullFilename, String *comment, String *jobLabel, bool skipUnpackaging);

	static MOG_Command *Setup_NetworkPackageMerge(String *platform, String *jobLabel, String *validSlaves, bool rejectAssets);
	static MOG_Command *Setup_NetworkPackageMerge(String *packageName, String *platform, String *jobLabel, String *validSlaves, bool rejectAssets);
	static MOG_Command *Setup_LocalPackageMerge(String *workingDirectory, String *platformName);
	static MOG_Command *Setup_EditorPackageMergeTask(String *workingDirectory, String *platformName);
	static MOG_Command *Setup_NetworkPackageRebuild(String *packageName, String *platform, String *jobLabel);
	static MOG_Command *Setup_LocalPackageRebuild(String *packageName, String *platform, String *jobLabel);

	static MOG_Command *Setup_Post(String *jobLabel);
	static MOG_Command *Setup_Post(String *jobLabel, bool skipArchiving);
	static MOG_Command *Setup_Post(MOG_Filename *assetFilename, String *jobLabel);

	static MOG_Command *Setup_Archive(String *assetName);
	static MOG_Command *Setup_ScheduleArchive(MOG_Filename *assetName, String *jobLabel);
	static MOG_Command *Setup_CleanTrash(String *userName, String *timeStamp);

	static MOG_Command *Setup_LockCopy(String *source, String *dest, String *description);
	static MOG_Command *Setup_LockMove(String *source, String *dest, String *description);
	static MOG_Command *Setup_LockReadRequest(String *lockName, String *description);
	static MOG_Command *Setup_LockWriteRequest(String *lockName, String *description);
	static MOG_Command *Setup_LockReadRelease(String *lockName);
	static MOG_Command *Setup_LockWriteRelease(String *lockName);
	static MOG_Command *Setup_LockWriteQuery(String *lockName);
	static MOG_Command *Setup_LockReadQuery(String *lockName);
	static MOG_Command *Setup_LockPersistentNotify(MOG_Command *lock, String* options);
	static MOG_Command *Setup_LockWatch(MOG_Command *lock);

	static MOG_Command *Setup_GetEditorInfo(String* workspaceDirectory);
	static MOG_Command *Setup_NetworkBroadcast(String *users, String *message);
	static MOG_Command *Setup_NetworkBroadcast_EntireProject(String *project, String *message);

	static MOG_Command *Setup_BuildFull(String *assetBuildName, String *source, String *target, String *validSlaves, bool force);
	static MOG_Command *Setup_Build(String *executable, String *validSlaves, String *user, String *EnvVariables);

	static MOG_Command *Setup_NewBranch(String *newBranchName, String *baseBranchName);

	static MOG_Command *Setup_InstantMessage(String *handle, String *senderUserName, String *projectName, String *invitedUsers, String *message);

	static MOG_Command *Setup_Complete(MOG_Command *pCommand, bool bWasCommandCompleted);
	static MOG_Command *Setup_Postpone(MOG_Command *pCommand, String *description);
	static MOG_Command *Setup_Failed(MOG_Command *pCommand, String *description);

	static MOG_Command *Setup_NotifySystemAlert(String *title, String *message, String *stackTrace, bool bNotifyUser);
	static MOG_Command *Setup_NotifySystemError(String *title, String *message, String *stackTrace, bool bNotifyUser);
	static MOG_Command *Setup_NotifySystemException(String *title, String *message, String *stackTrace, bool bNotifyUser);

	static MOG_Command *Setup_RequestActiveCommands(void);
	static MOG_Command *Setup_NotifyActiveCommand(MOG_Command *pCommand);

	static MOG_Command *Setup_RequestActiveLocks(void);
	static MOG_Command *Setup_NotifyActiveLock(MOG_Command *pCommand);

	static MOG_Command *Setup_RequestActiveConnections(void);
	static MOG_Command *Setup_NotifyActiveConnection(MOG_Command *pCommand);

	static MOG_Command *Setup_AutomatedTesting(String *testName, String *projectName, String *testImportFile, int testDuration_Seconds, bool bCopyFileLocal, int startingExtensionIndex);

	static MOG_Command *Setup_RetaskCommand(int slaveNetworkID);
	static MOG_Command *Setup_KillCommand(UInt32 commandID);

	static MOG_Command *Setup_LaunchSlave(int clientNetworkID);

	static MOG_Command *Setup_StartJob(String *jobLabel);
};
}
}
using namespace MOG;

#endif	// __MOG_CommandFactory_H__
