//--------------------------------------------------------------------------------
//	MOG_ControllerPackage.h
//	
//	
//--------------------------------------------------------------------------------

#ifndef __MOG_CONTROLLERPACKAGEMERGE_H__
#define __MOG_CONTROLLERPACKAGEMERGE_H__

#include "MOG_Filename.h"
#include "MOG_ControllerAsset.h"


namespace MOG {
namespace CONTROLLER {
namespace CONTROLLERPACKAGE {

public __gc struct MOG_PackageMerge_PackageFileInfo
{
	MOG_PackageMerge_PackageFileInfo(String* packageFile, String* workspaceDirectory, String* platformName, String* jobLabel);
	MOG_PackageMerge_PackageFileInfo(MOG_Filename *packageAssetFilename, String* workspaceDirectory, String* platformName, String *jobLabel);

	void Clear();
	void Initialize(String* workspaceDirectory, String* platformName);
	void Tokenize(String* packageWorkspaceDirectory, String* platformName);

	MOG_Filename *mPackageAssetFilename;
	MOG_Filename *mBlessedPackageFilename;

	MOG_Properties *mPackageProperties;

	String *mRelativePackageFile;
	String *mPackageFile;
	String *mJobLabel;

	MOG_PackageStyle mPackageStyle;
	bool mAutoPackage;
	bool mHideWindow;
	bool mSyncPackageWorkspaceDirectory;
	bool mCleanupPackageWorkspaceDirectory;

	// Fully expanded and tokenized package properties
	String *mTaskFileTool;
	String *mPackagePreMergeEvent;
	String *mPackagePostMergeEvent;
	String *mInputPackageTaskFile;
	String *mOutputPackageTaskFile;
	String *mPackageWorkspaceDirectory;
	String *mPackageDataDirectory;
	String *mPackageCommand_DeletePackageFile;
	String *mPackageCommand_ResolveLateResolvers;
	String *mPackageCommand_Add;
	String *mPackageCommand_Remove;
};

public __gc class PackageList : public ArrayList
{
public:
	void								Add(PackageList* packageFiles);
	void								Add(MOG_PackageMerge_PackageFileInfo* packageFile);
	void								AddTask(MOG_PackageMerge_PackageFileInfo* packageFile);
	MOG_PackageMerge_PackageFileInfo*	FindByFileName(String* packageName);
	MOG_PackageMerge_PackageFileInfo*	FindByAssetName(String* assetName);
	void								GenerateUniquePackageTasks(PackageList *packageFiles);
};


public __gc class MOG_ControllerPackageMerge
{
protected:
	PackageList*	mPackageFiles;
	PackageList*	mPackageMergeTasks;
	String*			mWorkspaceDirectory;
	String*			mPlatformName;
	DateTime		mStartTime;

protected:
	MOG_ControllerPackageMerge();

	// Package Merge Process Routines
	virtual PackageList* VerifyWorkingDirectories(PackageList *packageFiles);
	virtual MOG_PackageMerge_PackageFileInfo* VerifyWorkingDirectory(MOG_PackageMerge_PackageFileInfo* packageFile, bool bWarnUser);
	virtual bool CleanWorkingDirectories(PackageList *packageFiles);
	virtual bool CleanWorkingDirectories_TaskRelatedFiles(MOG_PackageMerge_PackageFileInfo *packageFileInfo);
	virtual bool CleanWorkingDirectories_EntireWorkspace(MOG_PackageMerge_PackageFileInfo *packageFileInfo);
	virtual bool PrepareWorkingDirectories(PackageList *packageFiles);
	virtual bool ExecutePreMergeEvents(PackageList *packageFiles);
	virtual PackageList *PreparePackageMergeTasks(PackageList *packageMergeTasks);
	virtual PackageList *ExecutePackageMergeTasks(PackageList *packageMergeTasks);
	virtual PackageList *DetectAffectedPackageFiles(MOG_PackageMerge_PackageFileInfo* packageFileInfo);
	virtual PackageList *GetUpdatedPackageFilesFromOutputTaskFile(MOG_PackageMerge_PackageFileInfo* packageFileInfo);
	virtual bool ExecutePostMergeEvents(PackageList *packageFiles);
	virtual bool ParseOutputTaskFile(PackageList *packageMergeTasks);
	virtual bool ParseOutputTaskFile_RecordLateResolvers(MOG_PackageMerge_PackageFileInfo* packageFileInfo);
	virtual bool ParseOutputTaskFile_ReportErrors(MOG_PackageMerge_PackageFileInfo* packageFileInfo);

	// Execute Package Commands
	virtual PackageList *ExecutePackageCommands_DeletePackageFile(MOG_PackageMerge_PackageFileInfo* packageFileInfo);
	virtual PackageList *ExecutePackageCommands_DeletePackageFile(PackageList *packageFiles);
	virtual PackageList *ExecutePackageCommands_LateResolver(MOG_PackageMerge_PackageFileInfo* packageFileInfo);
	virtual PackageList *ExecutePackageCommands_LateResolver(PackageList *packageFiles);
	virtual PackageList *ExecutePackageCommands_AddAsset(MOG_ControllerAsset *asset);
	virtual PackageList *ExecutePackageCommands_AddAsset(MOG_ControllerAsset *asset, MOG_PackageMerge_PackageFileInfo* packageFileInfo);
	virtual PackageList *ExecutePackageCommands_AddAsset(MOG_ControllerAsset *asset, ArrayList *packageAssignmentProperties);
	virtual PackageList *ExecutePackageCommands_RemoveAsset(MOG_ControllerAsset *asset);
	virtual PackageList *ExecutePackageCommands_RemoveAsset(MOG_ControllerAsset *asset, MOG_PackageMerge_PackageFileInfo* packageFileInfo);
	virtual PackageList *ExecutePackageCommands_RemoveAsset(MOG_ControllerAsset *asset, ArrayList *packageAssignmentProperties);
	virtual bool ExecutePackageCommands(MOG_ControllerAsset *asset, String *assetCommandFormat, MOG_Property *packageAssignmentProperty, MOG_PackageMerge_PackageFileInfo *packageFileInfo);
	virtual bool ExecutePackageCommand(MOG_PackageMerge_PackageFileInfo *packageFileInfo, String *assetCommand);

	// Package Tool Routines
	bool ExecutePackageTool(MOG_PackageMerge_PackageFileInfo *packageFileInfo, String* packageTool);
	virtual ArrayList *GetPackageToolEnvironment(String *relativeToolPath, MOG_PackageMerge_PackageFileInfo* packageFileInfo);

	// PostPackageMerge/OutputTaskFile Support Functions
	ArrayList* GetErrorsFromPackageCommandsFile(MOG_PackageMerge_PackageFileInfo* packageFileInfo, bool bReport);
	ArrayList* GetWarningsFromPackageCommandsFile(MOG_PackageMerge_PackageFileInfo* packageFileInfo, bool bReport);
	ArrayList* GetEventsFromPackageCommandsFile(MOG_PackageMerge_PackageFileInfo* packageFileInfo, bool bReport, String *section, MOG_ALERT_LEVEL alertLevel);

	// Misc Utility Routines
	PackageList *GetPackageFilesFromPackageAssignmentProperties(ArrayList* packageAssignments, MOG_Properties *properties, String *jobLabel);
	ArrayList *AutoDetectPackageLogFiles(String* packageWorkspaceDirectory, String* packageFilePath);
	PackageList *GetAssociatedPackageList(MOG_Filename *assetFilename, MOG_Properties *properties, String* jobLabel);

	MOG_Filename *LocateAsset_LocalWorkspaceOnly(MOG_Filename *assetName);
	MOG_Filename *LocateAsset_RepositoryOnly(MOG_Filename *assetName);
	MOG_Filename *LocateAsset_LocalWorkspaceAndRepository(MOG_Filename *assetName);
};

}
}
}


using namespace MOG::CONTROLLER::CONTROLLERPACKAGE;
using MOG::CONTROLLER::CONTROLLERPACKAGE::MOG_PackageMerge_PackageFileInfo;


#endif	// __MOG_CONTROLLERPACKAGEMERGE_H__

