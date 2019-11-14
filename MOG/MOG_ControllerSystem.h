//--------------------------------------------------------------------------------
//	MOG_ControllerSystem.h
//	
//	
//--------------------------------------------------------------------------------

#ifndef __MOG_CONTROLLERSYSTEM_H__
#define __MOG_CONTROLLERSYSTEM_H__

#include "MOG_Main.h"
#include "MOG_INI.h"
#include "MOG_Filename.h"
#include "MOG_System.h"
#include "MOG_Time.h"


namespace MOG {
namespace CONTROLLER {
namespace CONTROLLERSYSTEM {

public __value enum MOG_SystemMode {
	MOG_SystemMode_None,
	MOG_SystemMode_Server,
	MOG_SystemMode_Client,
	MOG_SystemMode_Slave,
	MOG_SystemMode_Editor,
	MOG_SystemMode_CommandLine,
};


public __gc class MOG_ControllerSystem
{
public:
	static void GoOffline() { mOffline = true; };
	static void GoOnline() { mOffline = false; };
	static bool GetOffline() { return mOffline; };

	__property static int get_MogMajorVersion() { return MOG_MAJOR_VERSION; }
	__property static int get_MogMinorVersion() { return MOG_MINOR_VERSION; }
	static String* GetMogSystemRelativeConfig() {return MOG_SYSTEM_RELATIVECONFIG; }

	// System Initialize
	static bool Shutdown();
	static bool InitializeSystem(String *configFilename, MOG_CommandManager *commandManager );
	static bool InitializeClient(String *configFilename);
	static bool InitializeSlave(String *configFilename);
	static bool InitializeEditor(String *configFilename);
	static bool InitializeCommandLine(String *configFilename);

	static bool InitializeDatabase(String *connectionString, String *projectName, String *branchName);
	static void InitializeNetworkInfo();
	static bool RequireProjectUserInitialization();
	static bool RequireNetworkIDInitialization();
	static bool RequireInitializationCredentials(bool bRequireProject, bool bRequireUser, bool bRequirePlatform);
	static bool ObtainInitializationCredentials(bool bRequireProject, bool bRequireUser, bool bRequirePlatform);

	// System Tick
	static bool Process(void);

	// System Functions
	static bool IsServerMode()		{ return (mMode == MOG_SystemMode_Server); };
	static bool IsClientMode()		{ return (mMode == MOG_SystemMode_Client); };
	static bool IsSlaveMode()		{ return (mMode == MOG_SystemMode_Slave); };
	static bool IsEditorMode()		{ return (mMode == MOG_SystemMode_Editor); };
	static bool IsCommandLineMode()	{ return (mMode == MOG_SystemMode_CommandLine); };	
	static String *GetSystemMode();
	static void	SetSystemMode(MOG_SystemMode mode);
	static MOG_System *GetSystem(void);
	static String *GetSystemRepositoryPath() { if (mSystem) return mSystem->GetSystemRepositoryPath(); else return "";}
	static String *GetSystemDemoProjectsPath() { if (mSystem) return mSystem->GetSystemDemoProjectsPath(); else return "";}
	static String *GetSystemProjectsPath() { if (mSystem) return mSystem->GetSystemProjectsPath(); else return "";}
	static String *GetSystemDeletedProjectsPath() { if (mSystem) return mSystem->GetSystemDeletedProjectsPath(); else return "";}
	static bool IsCommandManager(void)	{	return (mCommandManager != NULL); };
	static MOG_CommandManager *GetCommandManager(void);
	static MOG_Database *GetDB(void)	{	return mDB;	};

	static String *GetComputerName(void)		{ return mComputerName; }
	static String *GetComputerIP(void)			{ return mComputerIP; }
	static String *GetComputerMacAddress(void)	{ return mComputerMacAddress; }
	static int GetNetworkID();
	static String *GetServerComputerName(void)		{ if (mSystem) return mSystem->ServerName; else return ""; }
	static String *GetServerComputerIP(void)		{ if (mSystem) return mSystem->ServerIP; else return ""; }
	static String *GetServerComputerMacAddress(void){ if (mSystem) return mSystem->ServerMacAddress; else return ""; }
	
	static DateTime GetLastServerShutdownTime(void) { return mLastServerShutdownTime; }
	static void SetLastServerShutdownTime(DateTime time) { mLastServerShutdownTime = time; }

	static bool isSuppressMessageBox()		{	return bSuppressMessageBox;	};

	static bool RequestActiveCommands(void);
	static bool RequestActiveLocks(void);
	static bool RequestActiveConnections(void);

	static bool ChangeSQLConnection(String *SQLConnectionString);
	static bool ChangeRepository(String *newRepositoryPath);

	static bool ConnectionKill(int networkID);
	static bool LockKill(MOG_Command *pLockCommand);
	static bool LockKill(String *lockName, String *commandType, String *userName, String *computerName);

	static bool ToolExist(String *relativeFilename);
	static String *ResolveToolRelativePath(String *fullPath);
	static String *LocateTool(String *relativeFilename);
	static String *LocateTool(String *relativeToolsPath, String *filename);
	static String *LocateTool(String *relativeToolsPath, String *filename, MOG_Filename *assetDirectory);
	static ArrayList *LocateTools(String *relativeFilePattern);
	static ArrayList *LocateTools(String *relativeToolsPath, String *filePattern);
	static String *LocateInstallItem(String *item);
	static String *LocateCriticalInstallItem(String *item);
	static String* MOG_ControllerSystem::InternalizeTool(String* tool, String *relativeToolsPath);
	static String* MOG_ControllerSystem::StripInternalizedToolPath(String* tool);

	// Editor Utility API
	static MOG_Command *GetEditorInfo(String* workspaceDirectory);
	static bool CheckEditorRunning(String* workspaceDirectory);
	static bool CheckEditorBusy(String* workspaceDirectory);
	static bool WaitWhileEditorRunning(String* workspaceDirectory);

	// Slave API
	static bool ShutdownSlave(void);
	static bool ShutdownSlave(int networkID);
	static bool ShutdownSlave(String *computerName);

	// Network API
	static bool NetworkBroadcast(String *users, String *message);
	static bool NetworkBroadcast_EntireProject(String *Project, String *message);
	static bool InstantMessage(String *handle, String *invitedUsers, String *message);

	// Server Commands
	static bool RetaskAssignedSlaveCommand(int slaveNetworkID);
	static bool KillCommandFromServer(UInt32 commandID);
	static bool LaunchSlave(int clientNetworkID);

	// Refresh Commands
	static bool RefreshApplication();
	static bool RefreshTools();
	static bool RefreshProject(String *projectName);

	// System Notifications
	static bool NotifySystemAlert(String *title, String *message, String *stackTrace, bool bNotifyUser);
	static bool NotifySystemError(String *title, String *message, String *stackTrace, bool bNotifyUser);
	static bool NotifySystemException(String *title, String *message, String *stackTrace, bool bNotifyUser);

	// Copy a file with a dialog
	static bool FileCopyEx(String *srcFilename, String *dstFilename, bool overwrite, bool clearReadOnly, BackgroundWorker* worker);
	
	static bool DirectoryCopyEx(String *srcPath, String *dstPath, BackgroundWorker* worker)	{return DirectoryCopyEx(srcPath, dstPath, S"", worker);}
	static bool DirectoryCopyEx(String *srcPath, String *dstPath, String *label, BackgroundWorker* worker);

	static String *TokenizeMessageString(String *message);
	static String *TokenizeMessageString(String *message, String *targetString, String *token);

	// Invalid Characters
	static String *GetInvalidWindowsPathCharacters()							{ return INVALID_WINDOWSPATH_CHARACTERS; };
	static String *GetInvalidWindowsFilenameCharacters()						{ return INVALID_WINDOWSFILENAME_CHARACTERS; };
	static String *GetInvalidMOGCharacters()									{ return INVALID_MOG_CHARACTERS; };
	static bool InvalidMOGCharactersCheck(String *examine, bool bWarn);
	static bool InvalidWindowsPathCharactersCheck(String *examine, bool bWarn);
	static bool InvalidWindowsFilenameCharactersCheck(String *examine, bool bWarn);
	static bool InvalidCharactersCheck(String *examine, String *invalidChars, bool bWarn);
	static String *ReplaceInvalidCharacters(String *examine)					{ return ReplaceInvalidCharacters(examine, S"", S""); };
	static String *ReplaceInvalidCharacters(String *examine, String *invalidChars, String *replacementString);

protected:
	static void ObtainInitializationCredentials_Worker(Object* sender, DoWorkEventArgs* e);
	static void WaitWhileEditorRunning_Worker(Object* sender, DoWorkEventArgs* e);

protected:
	static MOG_System *mSystem = NULL;
	static MOG_CommandManager *mCommandManager = NULL;
	static MOG_Database *mDB = NULL;

	static MOG_SystemMode mMode;

	static String *mComputerName = S"";
	static String *mComputerIP = S"";
	static String *mComputerMacAddress = S"";

	static DateTime mLastServerShutdownTime = DateTime::Now;

	static bool bSuppressMessageBox = false;
	static bool mOffline;
};

}
}
}

using namespace MOG::CONTROLLER::CONTROLLERSYSTEM;

#endif	// __MOG_CONTROLLERSYSTEM_H__