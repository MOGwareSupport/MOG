//--------------------------------------------------------------------------------
//	MOG_System.h
//	
//	
//--------------------------------------------------------------------------------

#ifndef __MOG_System_H__
#define __MOG_System_H__

#include "MOG_Define.h"
#include "MOG_Project.h"
#include "MOG_Ini.h"
#include "MOG_Filename.h"

namespace MOG {
namespace SYSTEM {

public __gc class MOG_System {

private:
	String *mRepositoryPath;

	String *mExternalAssetRepositoryPath;
	String *mExternalAssetArchivePath;

	String *mConfigFilename;
	MOG_Ini *mConfigFile;

	ArrayList *mProjectNames;

	String* mServerName;
	String* mServerIP;
	String* mServerMacAddress;
	int mServerPort;
	int mServerMajorVersion;
	int mServerMinorVersion;

public:
	MOG_System(void);

	bool Load()									{ return Load(mConfigFilename); }
	bool Load(String *pConfigFilename);
	
	bool LoadServerInformation();

	String *LocateRipper(String *type, MOG_Filename *assetFullFilename);

	// Project Specific Functions
	ArrayList *GetProjectNames() { return mProjectNames; }
	MOG_Project *GetProject(String *projectName);

	MOG_Project *ProjectCreate(String *projectName);
	bool ProjectRemove(String *projectName);

	// Access Functions
	String *GetSystemRepositoryPath()			{ return mRepositoryPath; }
	String *GetSystemToolsPath(void)			{ return String::Concat(mRepositoryPath, S"\\Tools"); }
	String *GetSystemProjectsPath(void)			{ return String::Concat(mRepositoryPath, S"\\Projects"); }
	String *GetSystemDeletedProjectsPath(void)	{ return String::Concat(mRepositoryPath, S"\\Trash"); }
	String *GetSystemDemoProjectsPath(void)		{ return String::Concat(mRepositoryPath, S"\\Demos"); }
	String *GetExternalRepositoryPath()			{ return mExternalAssetRepositoryPath; }
	String *GetExternalArchivePath()			{ return mExternalAssetArchivePath; }
	String *GetConfigFilename(void)				{ return mConfigFilename; }
	MOG_Ini *GetConfigFile(void)				{ return mConfigFile; }

	__property String* get_ServerName()			{ return mServerName; }
	__property String* get_ServerIP()			{ return mServerIP; }
	__property String* get_ServerMacAddress()	{ return mServerMacAddress; }
	__property int get_ServerPort()				{ return mServerPort; }
	__property int get_ServerMajorVersion()		{ return mServerMajorVersion; }
	__property int get_ServerMinorVersion()		{ return mServerMinorVersion; }

	__property void set_ServerName(String *value)		{ mServerName = value; }
	__property void set_ServerIP(String *value)			{ mServerIP = value; }
	__property void set_ServerMacAddress(String *value)	{ mServerMacAddress = value; }
	__property void set_ServerPort(int value)			{ mServerPort = value; }
	__property void set_ServerMajorVersion(int value)	{ mServerMajorVersion = value; }
	__property void set_ServerMinorVersion(int value)	{ mServerMinorVersion = value; }
};

}
}

using namespace MOG::SYSTEM;

#endif	// __MOG_System_H__
