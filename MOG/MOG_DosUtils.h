//--------------------------------------------------------------------------------
//	MOG_DosUtils.hpp
//	
//	
//--------------------------------------------------------------------------------
#pragma once

#include "MOG_Define.h"
#include "MOG_CommandManager.h"


using namespace System::Runtime::InteropServices;
using namespace System::Diagnostics;
using System::Text::StringBuilder;
using System::DateTime;


namespace MOG {
namespace DOSUTILS {


public __gc class DosUtils
{
private:
	String *sDrive;
	String *tDrive;
	String *sPath;
	String *tPath;
	String *sFullName;
	String *tFullName;
	String *sName;
	String *tName;
	String *sDirectoryName;
	String *tDirectoryName;
	String *sFilename;
	String *tFilename;
	bool sIsDirectory;
	bool tIsDirectory;
	bool sContainsWildCard;
	bool tContainsWildCard;

	DirectoryInfo *sPathInfo;
	DirectoryInfo *tPathInfo;

	DirectoryInfo *sDirInfo;
	DirectoryInfo *tDirInfo;
	FileInfo *sFileInfo;
	FileInfo *tFileInfo;

	static String *mLastKnownError;

	static Process *gProcess;
	static StringBuilder* mProcessOutputLog;

public:
	bool Initialize(String *s, String *t);
	void Dispose();
	static void Shutdown();

	static String *GetLastError()		{	return mLastKnownError; };

	// Utility routines
	DirectoryInfo *Get_sPathInfo(void)	{	if (!sPathInfo) sPathInfo = new DirectoryInfo(sPath);  return sPathInfo;	};
	DirectoryInfo *Get_tPathInfo(void)	{	if (!tPathInfo) tPathInfo = new DirectoryInfo(tPath);  return tPathInfo;	};

	DirectoryInfo *Get_sDirInfo(void)	{	if (!sDirInfo) sDirInfo = new DirectoryInfo(sFullName);  return sDirInfo;	};
	DirectoryInfo *Get_tDirInfo(void)	{	if (!tDirInfo) tDirInfo = new DirectoryInfo(tFullName);  return tDirInfo;	};
	FileInfo *Get_sFileInfo(void)		{	if (!sFileInfo) sFileInfo = new FileInfo(sFullName);  return sFileInfo;	};
	FileInfo *Get_tFileInfo(void)		{	if (!tFileInfo) tFileInfo = new FileInfo(tFullName);  return tFileInfo;	};

	static bool SetAttributes(String *filename, FileAttributes attributes);
	static bool SetAttributesRecursive(String *directoryFullName, String *filePattern, FileAttributes attributes);

	static bool FileTouch(String *filename);

	// *********************************************************
	// Generic routines...either files or directories
	static bool Exist(String *filename);
	static bool ExistFast(String *filename);
	static bool Copy(String *source, String *target) { return Copy(source, target, true); };
	static bool Copy(String *source, String *target, bool overwrite);
	static bool CopyFast(String *source, String *target, bool overwrite);
	static bool Move(String *source, String *target) { return Move(source, target, true); };
	static bool Move(String *source, String *target, bool overwrite);
	static bool MoveFast(String *source, String *target, bool overwrite);
		// Rename folder or file assuming user wants to overwrite
	static bool Rename(String *source, String *target) { return Rename(source, target, true); };
	static bool Rename(String *source, String *target, bool overwrite);
	static bool RenameFast(String *source, String *target, bool overwrite);
	static bool Delete(String *filename);
	static bool DeleteFast(String *filename);
	static bool Recycle(String *path);

	static int DirectoryDepth(String *directory);
	static String *DirectoryGetAtDepth(String *directory, int depth);

	// *********************************************************
	// File routines
	static bool FileExist(String *filename);
	static bool FileExistFast(String *filename);
	static bool FileMissingModifiedFast(String *sourceFilename, String *targetFilename);
	static String* FileStripArguments(String *filename);
	static String* FileGetArguments(String *filename);
	static String* DosUtils::SplitArguments(String *argumentsString)[];
	static long long FileGetSize(String *filename);
	static bool FileCopy(String *source, String *target) { return FileCopy(source, target, true); };
	static bool FileCopy(String *source, String *target, bool overwrite);
	static bool FileCopyFast(String *source, String *target, bool overwrite);
	static bool FileCopyModified(String *source, String *target);
	static bool FileMove(String *source, String *target) { return FileMove(source, target, true, false); };
	static bool FileMove(String *source, String *target, bool overwrite, bool includeDirectoriesWithWildCards);
	static bool FileMoveFast(String *source, String *target, bool overwrite);
	static bool FileRename(String *source, String *target) { return FileRename(source, target, true); };
	static bool FileRename(String *source, String *target, bool overwrite);
	static bool FileRenameFast(String *source, String *target, bool overwrite);
	static bool FileDelete(String *filename);
	static bool FileDeleteFast(String *filename);

	static bool AppendTextToSlkFile(String *filename, String *text);
	static bool FileCloseSlk(String *filename);
	static bool AppendTextToFile(String *filename, String *text);
	static bool FileWrite(String *filename, String *text);
	static String* FileRead(String *filename);

	// *********************************************************
	// Path routines
	static String *PathGetFileName(String *filename);
	static String *PathGetRootPath(String *filename);
	static String *PathGetDirectoryPath(String *filename);
	static String *DosUtils::PathEnsureFullPath(String *path, String *defaultRootPath);
	static String *PathGetExtension(String *filename);
	static String *PathGetFileNameWithoutExtension(String *filename);
	static String *CheckForInvalidCharactersInPath(String *filename);
	static String *PathGetDrive(String *filename);
	static bool	PathIsDriveRooted(String *filename);
	static bool	PathIsWithinPath(String* parentPath, String* childPath);
	static String *PathMakeRelativePath(String* rootPath, String* fullPath);

	// *********************************************************
	// Directory routines
	static bool DirectoryExist(String *directory);
	static bool DirectoryExistFast(String *directory);
	static long long DirectoryGetSize(String *directory);
	static bool DirectoryCopy(String *source, String *target) { return DirectoryCopy(source, target, true); };
	static bool DirectoryCopy(String *source, String *target, bool overwrite);
	static bool DirectoryCopyFast(String *source, String *target, bool overwrite);
	static bool DirectoryCopyModified(String *source, String *target);
	static bool DirectoryMove(String *source, String *target) { return DirectoryMove(source, target, true); };
	static bool DirectoryMove(String *source, String *target, bool overwrite);
	static bool DirectoryMoveFast(String *source, String *target, bool overwrite);
	static bool DirectoryRename(String *source, String *target) { return DirectoryRename(source, target, true); };
	static bool DirectoryRename(String *source, String *target, bool overwrite);
	static bool DirectoryRenameFast(String *source, String *target, bool overwrite);
	static bool DirectoryCreate(String *directory) { return DirectoryCreate(directory, true); };
	static bool DirectoryCreate(String *directory, bool overwrite);
	static bool DirectoryDelete(String *directory);
	static bool DirectoryDeleteFast(String *directory);
	static bool DirectoryDeleteFast(String *directory, bool force);
	static bool DirectoryDeleteEmptyParents(String *path, String *fileExclusions);
	static bool DirectoryDeleteEmptyParentsFast(String *path, bool recurse);

	static String *CurrentDirectory();


	// *********************************************************
	// File list routines
	static FileInfo *FileGetInfo(String *filename);
	static DirectoryInfo *DirectoryGetInfo(String *filename);
	static FileInfo *FileGetList(String *directoryFullName, String *filePattern)[];
	static ArrayList *FileGetRecursiveList(String *directoryFullName, String *filePattern);
	static ArrayList *FileGetRecursiveRelativeList(String *directoryFullName, String *filePattern);
	static DirectoryInfo *DirectoryGetList(String *directoryFullName, String *filePattern)[];


	// *********************************************************
	// Environment variables
	static String *GetSystemEnvironmentVariable(String *EnvVarName);
	static ArrayList *GetSystemEnvironmentVariables();
	
	static void EnvironmentList_AddVariable(ArrayList *environment, String *variableStatement);
	static void EnvironmentList_AddVariable(ArrayList *environment, String *variable, String *value);
	static void EnvironmentList_AddVariables(ArrayList *environment, String *variableStatements[]);
	static void EnvironmentList_AddFile(ArrayList *environment, String *infoFile);
	static String* EnvironmentList_GetVariable(ArrayList* environment, String* variable);

	// *********************************************************
	// Dos command routines
	static int SpawnDosCommand(String *directory, String *command, String *args, ArrayList *environment, String **OutputLog, bool hiddenWindow);
	static bool SpawnCommand(String *command, String *args);

private:
	static int SilentSharingViolationUpdate(String* filename, int violationStage, DateTime startTime, String* message, String* stackTrace);
	static void OnOutputDataReceived(Object* sender, DataReceivedEventArgs* e);
	static void OnErrorDataReceived(Object* sender, DataReceivedEventArgs* e);
};

}
}


using namespace MOG::DOSUTILS;
