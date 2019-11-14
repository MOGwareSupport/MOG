//--------------------------------------------------------------------------------
//	MOG_ControllerProject.h
//	
//	
//--------------------------------------------------------------------------------

#ifndef __MOG_CONTROLLERPROJECT_H__
#define __MOG_CONTROLLERPROJECT_H__

#include "MOG_Main.h"
#include "MOG_INI.h"
#include "MOG_Filename.h"
#include "MOG_System.h"
#include "MOG_Project.h"
#include "MOG_Time.h"
#include "MOG_Privileges.h"
#include "MOG_ControllerSystem.h"
#include "MOG_ControllerAsset.h"
#include "MOG_ControllerSyncData.h"

namespace MOG {
namespace CONTROLLER {
namespace CONTROLLERPROJECT {


public __gc class MOG_ControllerProject
{
public:
	// Project
	static bool ProjectExists(String* projectName);
	static MOG_Project *LoginProject(String *projectName, String *branchName);
	static MOG_Project *LoginProjectIfNecessary(String *projectName, String *branchName);
	static void Logout();
	static MOG_Project *GetProject(void)					{ return mProject; };
	static String *GetProjectPath(void)						{ return (mProject) ? mProject->GetProjectPath() : S""; };
	static String *GetProjectName(void)						{ return (mProject) ? mProject->GetProjectName() : S""; };
	static String *GetBranchName(void)						{ return mBranchName; };
	static bool IsProject(void)								{ return (mProject) ? true : false; };
	static bool IsBranch(void)								{ return (IsProject() && mBranchName->Length) ? true : false; };
	static MOG_Privileges *GetPrivileges( void )			{ return mPrivileges; };
	static void RefreshPrivileges( void );

	// Project Branch
	static bool TagCreate(String *sourceTagName, String *newTagName);
	static bool BranchCreate(String *sourceBranchName, String *newBranchName);
	static bool BranchCreate(String *sourceBranchName, String *newBranchName, bool bIsTag);
	static bool TagCreate(MOG_ControllerSyncData* workspace, String *newTagName);
	static bool BranchCreate(MOG_ControllerSyncData* workspace, String *newBranchName);
	static bool BranchCreate(MOG_ControllerSyncData* workspace, String *newBranchName, bool bIsTag);
	static bool BranchRename(String *oldBranchName, String *newBranchName);
	static bool BranchRemove(String *branchName);
	static bool BranchPurge(String *branchName);

	// Project Department
	static String* GetDepartment()									{ return mUser ? mUser->GetUserDepartment() : S""; }

	// Project User
	static bool IsValidUser(String *userName);
	static MOG_User *LoginUser(String *userName);
	static MOG_User *GetUser(void)									{ return mUser; };
	static String *GetUserPath(void)								{ return (mUser) ? mUser->GetUserPath() : S""; };
	static String *GetUserPath(String *userName)					{ return (mProject && mProject->GetUser(userName)) ? mProject->GetUser(userName)->GetUserPath() : S""; };
	static String *GetUserName(void)								{ return (mUser) ? mUser->GetUserName() : S""; };
	static String *GetUserName_DefaultAdmin(void)					{ return (mUser) ? mUser->GetUserName() : S"Admin"; };
	static bool IsUser(void)										{ return (mUser) ? true : false; };

	// Computer
	static String *LoginComputer(String *computerName);
	static String *GetComputerName()								{ return (mComputerName) ? mComputerName : MOG_ControllerSystem::GetComputerName(); };

	// Project Workspace
	static bool SetActiveTabName(String *activeTabName);
	static String *GetActiveTabName(void)							{ return mActiveTabName; };
	static bool SetActiveUserName(String *activeUserName);
	static String *GetActiveUserName(void)							{ return (mActiveUser) ? mActiveUser->GetUserName() : S""; };
	static MOG_User *GetActiveUser()								{ return mActiveUser; };

	static MOG_ControllerSyncData *GetCurrentSyncDataController()	{ return mCurrentSyncDataController; };
	static bool SetCurrentSyncDataController(MOG_ControllerSyncData *controller);
	static String *GetWorkspaceDirectory(void)						{ return (mCurrentSyncDataController) ? mCurrentSyncDataController->GetSyncDirectory() : S""; };
	static String *GetPlatformName(void)							{ return (mCurrentSyncDataController) ? mCurrentSyncDataController->GetPlatformName() : S"All"; };

	// Platforms
	static bool IsValidPlatform(String *platformName);
	static String* GetAllPlatformsString();

	// Classifications
	static bool IsValidClassification(String *classificationFullName);
	static MOG_Properties *GetClassificationProperties(String *classification);
	

	// Asset Functions
	static bool ValidateAssetFilename(MOG_Filename *assetFilename);
	static bool MakeAssetCurrentVersion(MOG_Filename *assetFilename, String *comment, String *jobLabel);
	static bool MakeAssetCurrentVersion(ArrayList *blessedAssetFilenames, String *comment);
	static bool RemoveAssetFromProject(MOG_Filename *assetFilename, String *comment, bool skipUnpackaging);
	static bool RemoveAssetFromProject(MOG_Filename *assetFilename, String *comment, String *jobLabel, bool skipUnpackaging);
	static bool DoesAssetExists(MOG_Filename *assetFilename);
	static bool DoesPlatformSpecificAssetExists(MOG_Filename *assetFilename, String *platformName);
	static MOG_Filename* GetPlatformSpecificAsset(MOG_Filename *assetFilename, String *platformName);
	static String *GetAllApplicablePlaformsForAsset(String *assetName, bool bExcludeOverriddenPlatforms)[];
	static bool IsPlatformValidForAsset(MOG_Filename *assetFilename, String *platformName);
	static bool AddAssetForPosting(MOG_Filename *pendingAsset, String *jobLabel)		{ return AddAssetForPosting(pendingAsset, jobLabel, S"", S""); };
	static bool AddAssetForPosting(MOG_Filename *pendingAsset, String *jobLabel, String *owner, String *creator);
	static bool PostAssets(String *projectName, String *branchName, String *jobLabel);
	static bool PostAssets(String *projectName, String *branchName, String *jobLabel, bool skipArchiving);
	static MOG_Filename *GetAssetCurrentBlessedPath( MOG_Filename *assetFilename );
	static MOG_Filename *GetAssetCurrentBlessedVersionPath( MOG_Filename *assetFilename );
	static String *GetAssetCurrentVersionTimeStamp( MOG_Filename *assetFilename );
	static ArrayList *GetPackagesForAsset( MOG_Filename *assetFilename );
	static ArrayList *MapFilenameToAssetName_FilterLibraryAssets(ArrayList *assetNames);
	static ArrayList *MapFilenameToAssetName_FilterAssetsWithExtensions(ArrayList *assetNames);
	static ArrayList *MapAssetNameToFilename(MOG_Filename *assetFilename, String *platformName);
	static MOG_Filename *MapFilenameToLibraryAssetName(String *filename, String *platformName);
	static ArrayList *MapFilenameToAssetName(String *filename, String *platformName, String* workspaceDirectory);
	static ArrayList *MapFilenameToAssetName(String *filename, String *platformName);
	static ArrayList *MapFilenamesToAssetNames(ArrayList *pFilenames, String *platformName, BackgroundWorker* worker);
	static bool MapFilenameToAssetName_WarnAboutAmbiguousMatches(String *filename, ArrayList *assets);
	static ArrayList *MapPackageObjectToAssetNames(String* assetLabel, MOG_Property* relationshipProperty);
	static ArrayList *MapPackageObjectToAssetNamesInUserInbox(String* assetLabel, MOG_Property* relationshipProperty);
	static ArrayList *MapPackageObjectToContainedAssetNames(String* assetLabel, MOG_Property* relationshipProperty, String* LOD);
	static ArrayList *MapPackageObjectToContainedAssetNamesInUserInbox(String* assetLabel, MOG_Property* relationshipProperty, String* LOD);
	static String* GenerateLODName(String* AssetLabel, String* LOD);
	static String* DetectLODTagFromAssetLabel(String* AssetLabel);
	static String* StripLODTagFromAssetLabel(String* AssetLabel);
	static String* DetectLODFromAssetLabel(String* AssetLabel);
	static String* DetectLODFromLODTag(String* LODTag);
	
	static ArrayList *GetAllAssetsBySyncLocation(String *syncLocation, String *platform);
	static ArrayList *ResolveAssetNameListForPlatformSpecificAssets(ArrayList *assets, String* platformName);

	// Package Functions
	static MOG_Filename *CreatePackage(MOG_Filename *packageFilename, String *syncTargetPath);
	static MOG_Filename *CreatePackage(MOG_Filename *packageFilename, String *syncTargetPath, String* jobLabel);

	// Network Jobs
	static bool StartJob(String *jobLabel);
	static bool RestartJob(MOG_Filename *assetFilename, String *revisionTimestamp, String *jobLabel);

	// Lock Functions
	static bool PersistentLock_Release(String *assetName);
	static MOG_Command *PersistentLock_Request(String *assetName, String *description);
	static MOG_Command *PersistentLock_Request(String *assetName, String *description, String *lockOwner);
	static String* GetLockOwner(String *assetName);
	static bool IsLocked(String *assetName);
	static bool IsLockedByMe(String *assetName);
	static MOG_Command *PersistentLock_Query(String *assetName);
	static bool QueryWriteLock(String *lockName);
	static bool QueryReadLock(String *lockName);

	// Build Functions
	static bool BuildFull(String *assetBuildName, String *source, String *target, String *validSlaves, bool force);
	static bool Build(String *executable, String *validSlaves, String *user, String *EnvVariables);

	// Archive Functions
	static bool Archive(void);

protected:
	static MOG_Project *mProject = NULL;
	static String *mBranchName = S"";
	static MOG_User *mUser = NULL;
	static String *mComputerName = NULL;
	static MOG_Privileges *mPrivileges = NULL;
	static String *mActiveTabName = S"";
	static MOG_User *mActiveUser = NULL;
	static MOG_ControllerSyncData *mCurrentSyncDataController = NULL;

	static ArrayList *MapFilenameToAssetName_InsertAssetsByProperty(MOG_Property* property);

	static void PostAssets_Worker(Object* sender, DoWorkEventArgs* e);
};

}
}
}

using namespace MOG::CONTROLLER::CONTROLLERPROJECT;

#endif	// __MOG_CONTROLLERPROJECT_H__





