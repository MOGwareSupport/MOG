//--------------------------------------------------------------------------------
//	MOG_ControllerInbox.h
//	
//	
//--------------------------------------------------------------------------------

#ifndef __MOG_CONTROLLERINBOX_H__
#define __MOG_CONTROLLERINBOX_H__

#include "MOG_Main.h"
#include "MOG_Filename.h"
#include "MOG_ControllerAsset.h"
#include "MOG_AssetStatus.h"

#include "MOG_ControllerInbox.h"



namespace MOG {
namespace CONTROLLER {
namespace CONTROLLERINBOX {


public __gc class MOG_ControllerInbox
{
protected:
	// Helper for Rename()
	static bool Rename_FileRename(MOG_Filename *oldFilename, MOG_Filename *newFilename, String *file);

public:
	// Asset Commands
	static bool RipAsset(MOG_Filename *assetFilename);
	static bool BlessAsset(MOG_Filename *assetFilename, String *comment, bool maintainLock, BackgroundWorker* worker);
	static bool BlessAsset(MOG_Filename *assetFilename, String *comment, bool maintainLock, String *jobLabel, BackgroundWorker* worker);
	static String *GetAssetBlessTarget(MOG_Filename *assetFilename, MOG_Properties *properties);
	static bool MoveAssetToUserInbox(MOG_Filename *assetFilename, String *user, String *comment, MOG_AssetStatusType status, BackgroundWorker* worker);
	static bool MoveAssetToUserInbox(MOG_ControllerAsset *asset, String *user, String *comment, MOG_AssetStatusType status, BackgroundWorker* worker);
	static bool CopyAssetToUserInbox(MOG_Filename *assetFilename, String *user, String *comment, MOG_AssetStatusType status, BackgroundWorker* worker);
	static bool CopyAssetToUserInbox(MOG_ControllerAsset *asset, String *user, String *comment, MOG_AssetStatusType status, BackgroundWorker* worker);
	static bool Rename(MOG_Filename *assetFilename, String *newAssetName, bool bRenameFiles);
	static bool Reject(MOG_Filename *assetFilename, String *comment, BackgroundWorker* worker);
	static bool Reject(MOG_Filename *assetFilename, String *comment, MOG_AssetStatusType state, BackgroundWorker* worker);
	static bool Reject(MOG_ControllerAsset *asset, String *comment, BackgroundWorker* worker);
	static bool Reject(MOG_ControllerAsset *asset, String *comment, MOG_AssetStatusType state, BackgroundWorker* worker);
	static bool Delete(MOG_Filename *assetFilename);
	static bool Delete(MOG_ControllerAsset *asset);
	static bool RemovePackageAssignment(MOG_Filename *assetFilename, String *packageName, String *packageGroups, String *packageObjects);
	static bool RemoveRelationshipProperty(MOG_Filename *assetFilename, MOG_Property *relationshipProperty);
	static bool RemoveRelationshipProperty(MOG_ControllerAsset *asset, MOG_Property *relationshipProperty);

	static ArrayList*	GetAssetList(String* box);

	// Asset Links
	static bool CreateAssetLinkFile(MOG_ControllerAsset *asset);
	static bool UpdateAssetLinkFiles(MOG_ControllerAsset *asset);
	static bool RemoveAssetLinkFile(MOG_Filename *linkFilename);
	static bool RemoveAllAssetLinkFiles(MOG_Filename *assetFilename);
	static bool RemoveAllAssetLinkFiles(MOG_ControllerAsset *asset);

	// Asset Navigation
	static bool MoveToBox(MOG_Filename *assetFilename, String *targetBox, MOG_AssetStatusType status);
	static bool MoveToBox(MOG_ControllerAsset *asset, String *targetBox, MOG_AssetStatusType status);
	static bool Move(MOG_ControllerAsset *asset, MOG_Filename *targetPath, MOG_AssetStatusType status);
	static bool Move(MOG_ControllerAsset *asset, MOG_Filename *targetPath, MOG_AssetStatusType status, bool leaveLink);
	static bool CopyToBox(MOG_Filename *assetFilename, String *targetBox, MOG_AssetStatusType status, BackgroundWorker* worker);
	static bool CopyToBox(MOG_ControllerAsset *asset, String *targetBox, MOG_AssetStatusType status, BackgroundWorker* worker);
	static bool Copy(MOG_ControllerAsset *asset, MOG_Filename *target, MOG_AssetStatusType status, BackgroundWorker* worker);
	static MOG_ControllerAsset* Copy_ReturnNewAsset(MOG_ControllerAsset *asset, MOG_Filename *targetAssetFilename, MOG_AssetStatusType status, BackgroundWorker* worker);
	static bool ResolveCollidingAssets(MOG_ControllerAsset *asset, MOG_Filename *destinationFilename, BackgroundWorker* worker);

	// Trash
	static String *GetAssetTrashPath(MOG_Filename *assetFilename, String *timestamp);
	static bool MoveToTrash(MOG_Filename *assetFilename);
	static bool MoveToTrash(MOG_ControllerAsset *asset);
	static bool RestoreAsset(MOG_Filename *assetFilename);
	static bool RestoreAsset(MOG_ControllerAsset *asset);

	// Inbox Properties
	static void UpdateInboxView(String *status, MOG_Filename *sourceFilename, MOG_Filename *targetFilename);
	static void UpdateInboxView(MOG_AssetStatusType status, MOG_Filename *sourceFilename, MOG_Filename *targetFilename);
	static void UpdateInboxView(MOG_AssetStatusType status, MOG_Filename *sourceFilename, MOG_Filename *targetFilename, MOG_Properties *properties);
	static void UpdateInboxView(MOG_ControllerAsset *asset, String *status);
	static void UpdateInboxView(MOG_ControllerAsset *asset, MOG_AssetStatusType status);
	static void UpdateInboxView(MOG_ControllerAsset *asset, MOG_AssetStatusType status, MOG_Filename *sourceFilename, MOG_Filename *targetFilename);
	static void UpdateInboxView(MOG_ControllerAsset *asset, String *status, MOG_Filename *sourceFilename, MOG_Filename *targetFilename, MOG_Properties *properties);
	static bool RebuildInboxView(String *path, bool bSendViewUpdateEvent);
	static bool UpdateAssetProperties(MOG_ControllerAsset *asset);
	static bool UpdateLinkProperties(MOG_Filename *linkFilename);

	//Utilities
	static MOG_Filename *LocateBestMatchingAsset(MOG_Filename *assetFilename);
	static String *GetInboxPath(String *boxName);
	static String *GetInboxPath(String *userName, String *boxName);
	static String *GetInboxPath(String *userName, String *boxName, String *groupPath);
	static String *ObtainTimestampFromJobLabel(String *jobLabel);
	static ArrayList* GetInboxConflictsForAsset(MOG_Filename* assetFilename, String* creator);

protected:
	static void RebuildInbox_Worker(Object* sender, DoWorkEventArgs* e);
};

}
}
}

using namespace MOG::CONTROLLER::CONTROLLERINBOX;

#endif	// __MOG_CONTROLLERINBOX_H__




