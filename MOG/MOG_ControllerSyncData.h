//--------------------------------------------------------------------------------
//	MOG_ControllerSyncData.h
//	
//	
//--------------------------------------------------------------------------------

#ifndef __MOG_CONTROLLERSYNCDATA_H__
#define __MOG_CONTROLLERSYNCDATA_H__

#include "MOG_Main.h"
#include "MOG_INI.h"
#include "MOG_Filename.h"
#include "MOG_ControllerAsset.h"
#include "MOG_DBSyncedDataAPI.h"
#include "MOG_AllinatorPrompt.h"
#include "MOG_FileHamsterIntegration.h"


namespace MOG {
namespace CONTROLLER {
namespace CONTROLLERSYNCDATA {

using namespace MOG_CoreControls;

public __gc class MOG_ControllerSyncData
{
public:
	__value enum PackageState
	{
		PackageState_NothingToDo,	//Nothing doing, nothing to be done
		PackageState_Error,			//Error
		PackageState_Pending,		//Nothing doing, something to be done
		PackageState_Busy,			//Doing stuff
	};

	MOG_ControllerSyncData(String *syncDirectory, String *projectName, String *branchName, String *platformName, String *userName);
	MOG_ControllerSyncData(String *name, String *syncDirectory, String *projectName, String *branchName, String *platformName, String *userName);
	~MOG_ControllerSyncData(void);
	void InitializePackageState();
	PackageState GetLocalPackagingStatus();
	PackageState GetLocalPackagingStatus(MOG_Filename *assetFilename);
	bool Rename(String* newTabName);
	bool ValidateLocalWorkspace(void);

	void SetSyncDirectory(String* syncdir)		{mSyncDirectory = syncdir;}
	void SetProjectName(String *project)		{mProjectName = project;}
	void SetBranchName(String *branch)			{mBranchName = branch;}
	void SetPlatformName(String *platform)		{mPlatformName = platform;}
	void SetName(String *name)					{mName = name;}
	void SuspendPackaging(bool suspend);
	void EnableAutoPackaging(bool autoPackage);
	void EnableAutoUpdating(bool autoUpdate);

	// Sync Asset Functions
	bool CanAddAssetToLocalWorkspace(MOG_ControllerAsset* asset, bool bInformUser);
	bool AddAssetToLocalUpdatedTray(MOG_ControllerAsset* asset);
	bool AddAssetToLocalUpdatedTray(MOG_ControllerAsset* asset, BackgroundWorker* worker);
	bool AddAssetToLocalWorkspace(MOG_ControllerAsset* asset);
	bool AddAssetToLocalWorkspace(MOG_ControllerAsset* asset, BackgroundWorker* worker);
	bool RemoveAssetFromLocalWorkspace(MOG_Filename *assetFilename, BackgroundWorker* worker);
	bool RepackageAssetInLocalWorkspace(MOG_Filename *assetFilename);
	bool SetLocalFileAttributes(MOG_Filename* assetFilename, FileAttributes attribute);
	bool SetLocalFileAttributes(MOG_Properties* properties, FileAttributes attribute);

	// Sync Workspace Functions
	bool SyncRepositoryData(String *classification, String *exclusionList, String *inclusionList, bool bCheckMissingModifiedFiles, MOG_LocalSyncInfo *syncInfo);
	bool SyncDataFromSyncLocation(String *syncLocation, String *exclusionList, String *inclusionList, bool bCheckMissingModifiedFiles, MOG_LocalSyncInfo *syncInfo);

	bool UnsyncRepositoryData(String *classification, String *exclusionList);
	bool UnsyncDataFromSyncLocation(String *syncLocation, String *exclusionList);

	static bool ExecuteLocalPackageMerge(MOG_ControllerSyncData* sync);
	void AutoPackage();
	void BuildPackage();

	static ArrayList *GetAllSyncDataFileStructsForPlatform( String *platformName );
	static ArrayList *GetSyncDataFileAssetByFileName( String *syncdataFileName, String *platformName  );

	bool ProcessAssetFiles(MOG_Properties *oldProperties, MOG_Properties *newProperties, BackgroundWorker* worker);
	bool LocalFileVerification(MOG_Properties *properties);
	bool SyncAssetFiles_HasBeenModified(String *localFile, String *originalFile);
	bool SyncAssetFiles_CanOverwriteFile(String *localFile, String *originalFile, bool showLocalModifiedWarning);

	String*	GetSyncDirectory()				{return mSyncDirectory;}
	String*	GetProjectName()				{return mProjectName;}
	String*	GetBranchName()					{return mBranchName;}
	String*	GetPlatformName()				{return mPlatformName;}
	String*	GetName()						{return mName;}
	String*	GetUserName()					{return mUserName;}
	void	SetAlwaysActive(bool value)		{mAlwaysActive = value;}
	bool	IsAlwaysActive()				{return mAlwaysActive;}
	bool	IsPackagingSuspended()			{return mPackagingSuspended;}
	bool	IsAutoPackageEnabled()			{return mAutoPackageEnabled;}
	bool	IsAutoUpdateEnabled()			{return mAutoUpdateEnabled;}
	bool	IsEqual(MOG_ControllerSyncData *right);
	
	MOG_Ini *GetSyncLog() { return mSyncLog; }
	void ClearSyncLog() { if( mSyncLog ) mSyncLog->Empty(); }

	void BeginPackaging(ArrayList* assetNames)	{mIsPackaging = true;	mAssetNamesGettingPackagedList = assetNames; }
	void EndPackaging()							{mIsPackaging = false;	mAssetNamesGettingPackagedList = new ArrayList(); }

	bool ModifyLocalWorkspace_Begin();
	bool ModifyLocalWorkspace_Complete();
	static String* DetectWorkspaceRoot(String* path);

	String* MOG_ControllerSyncData::GetLastUpdatedTime();

protected:
	bool StartSyncProcess(String *classification, String *syncLocation, String *exclusionList, String *inclusionList, bool bCheckMissingModifiedFiles, MOG_LocalSyncInfo *syncInfo);
	void SyncRepositoryData_Worker(Object* sender, DoWorkEventArgs* e);
	
	// Check missing files
	bool SyncRepositoryData_CheckForMissingModifiedFiles(BackgroundWorker* worker, String *exclusionList, String *inclusionList, MOG_LocalSyncInfo *syncInfo);

	bool RemoveLocalFile(String *fileToRemove, String *originalFile, bool showLocalModifiedWarning);
	bool CopyLocalFile(String *fileToCopy, String *destinationFile, bool bClearReadOnlyFlag, BackgroundWorker* worker);
	bool RestampLocalAssets(BackgroundWorker* worker, bool bPerformFullRestamp);

	bool IsAssetListedInLocalWorkspace(MOG_Filename *assetFilename);

	// Utility Functions
	String *GetSyncDataDirectoryPath(String *relativeSyncDataFile);
	String *GetSourceDirectoryPath(MOG_ControllerAsset *asset, String *relativeSyncDataFile);

	ArrayList *GetAssetSyncRelativeFileList(MOG_Properties *assetProperties);

	void UnsyncRepositoryData_Worker(Object* sender, DoWorkEventArgs* e);
	void UnsyncDataFromSyncLocation_Worker(Object* sender, DoWorkEventArgs* e);

	bool RestampLocalSyncInfoFile();

	String *mSyncDirectory;
	
	String *mName;
	String *mProjectName;
	String *mBranchName;
	String *mPlatformName;
	String *mUserName;

	MOG_Ini *mSyncLog;

	static MOG_AllinatorPrompt *mGetLatest_OverwriteFile = new MOG_AllinatorPrompt();
	static MOG_AllinatorPrompt *mUnpackageLocalAsset = new MOG_AllinatorPrompt();
	static MOG_AllinatorPrompt *mRemoveOutofdateVerifiedLocalAsset = new MOG_AllinatorPrompt();

	ArrayList *mLocalAssets;

	bool mAlwaysActive;
	bool mPackagingSuspended;
	bool mAutoPackageEnabled;
	bool mAutoUpdateEnabled;

	bool mIsPackaging;
	ArrayList* mAssetNamesGettingPackagedList;
};

}
}
}

using namespace MOG::CONTROLLER::CONTROLLERSYNCDATA;

#endif	// __MOG_CONTROLLERSYNCDATA_H__


