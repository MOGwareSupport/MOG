//--------------------------------------------------------------------------------
//	MOG.h
//	
//	
//--------------------------------------------------------------------------------

#ifndef __MOG_H__
#define __MOG_H__

#include "MOG_Define.h"


namespace MOG
{

using namespace System::ComponentModel;

public __gc class MOG_Main
{
private:
	static bool mProcessMutex_Locked = false;
	static bool mShutdownInProgress = false;
	static bool mShutdown = false;
	static String *mName = S"";
	static String *mAssociatedWorkspacePath = S"";
	static String *mDisabledFeatureList = S"";

public:
	MOG_Main(void);
	~MOG_Main(void);

	static bool Init_Client(String *configFilename, String *name);
	static bool Init_Slave(String *configFilename, String *name);
	static bool Init_Editor(String *configFilename, String *name);
	static bool Init_CommandLine(String *configFilename, String *name);

	static bool Process(void);
	static void LockProcessMutexFlag(bool state)					{ mProcessMutex_Locked = state; };
	static bool IsProcessMutexLocked()								{ return mProcessMutex_Locked; };

	static void Shutdown();
	static bool IsShutdownInProgress(void)							{ return mShutdownInProgress; };
	static bool IsShutdown(void)									{ return mShutdown; };
	static void NotifyUserOfPendingShutdown(String *message);

	static bool CheckInitPrecautions(String *configFilename);	
	static String *GetName()										{ return mName; };
	static void SetName(String *name)								{ mName = name; };
	static String *GetExecutablePath();
	static String *GetExecutablePath_StripCurrentDirectory();
	static String *GetAssociatedWorkspacePath();
	static void SetAssociatedWorkspacePath(String *path)			{ mAssociatedWorkspacePath = path->TrimEnd(S"\\"->ToCharArray()); };
	static String *GetDefaultRepositoryPath(String *configFile);
	static String *GetDefaultSystemConfigFile(String *configFile);
	static String *GetDefaultSystemConfigFilenameDefine()			{ return MOG_SYSTEM_CONFIGFILENAME; };
	static String *GetDefaultSystemRelativeConfigFileDefine()		{ return MOG_SYSTEM_RELATIVECONFIG; };
	static String *GetDefaultSystemConfigFileDefine()				{ return MOG_SYSTEM_CONFIG; };
	static String *FindLocalConfigFile();
	static String *FindInstalledConfigFile();
	static String *BuildDefaultConfigFile();

	static void SetDisabledFeatureList(String *disabledFeatureList)	{ mDisabledFeatureList = disabledFeatureList; };
	static String *GetDisabledFeatureList()							{ return mDisabledFeatureList; };
	static bool IsLicensed();
	static bool IsUnlicensed()										{ return !IsLicensed(); };
	static bool IsFeatureDisabled(String *feature);
	static bool IsFeatureEnabled(String *feature)					{ return !IsFeatureDisabled(feature); };

private:
	static void NotifyUserOfPendingShutdown_Worker(Object* sender, DoWorkEventArgs* e);
};

}

using namespace MOG;

#endif	// __MOG_H__


