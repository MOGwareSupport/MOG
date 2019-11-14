//--------------------------------------------------------------------------------
//	MOG_ControllerPackageMergeNetwork.h
//	
//	
//--------------------------------------------------------------------------------

#ifndef __MOG_CONTROLLERPACKAGEMERGENETWORK_H__
#define __MOG_CONTROLLERPACKAGEMERGENETWORK_H__

#include "MOG_ControllerPackageMerge.h"
#include "MOG_DBPackageCommandAPI.h"


namespace MOG {
namespace CONTROLLER {
namespace CONTROLLERPACKAGE {


public __gc class MOG_ControllerPackageMergeNetwork : public MOG_ControllerPackageMerge
{
public:
	MOG_ControllerPackageMergeNetwork();
	MOG_ControllerPackageMergeNetwork(bool bRejectAssets);

	PackageList* DoPackageEvent(MOG_Filename* packageFilename, String* jobLabel, String* branchName, String* workspaceDirectory, String* platformName, String* validSlaves, BackgroundWorker* worker);

	// Schedule Pending Network Package Commands
	static PackageList* ScheduleDeletePackageCommand(MOG_PackageMerge_PackageFileInfo* packageFileInfo, String* jobLabel);
	static PackageList* ScheduleAssetPackageCommands(MOG_ControllerAsset* asset, MOG_PACKAGECOMMAND_TYPE commandType, String* jobLabel, String* validSlaves, bool rejectAssets);
	static PackageList* ScheduleAssetPackageCommands(MOG_ControllerAsset* asset, MOG_PACKAGECOMMAND_TYPE commandType, String* jobLabel, String* validSlaves, MOG_PackageMerge_PackageFileInfo* specificPackageFileInfo, bool rejectAssets);
	static bool SchedulePlatformSpecificAssetAddition(MOG_ControllerAsset* platformSpecificAsset, String* jobLabel, String* validSlaves);
	static bool SchedulePlatformSpecificAssetRemoval(MOG_ControllerAsset* platformSpecificAsset, String* jobLabel, String* validSlaves);
	// Late Resolvers
	static bool AddLateResolvers(String* PackageName, String* LinkedPackage);
	static bool WasPackageFileScheduledForDeletion(MOG_Filename *packageFilename, ArrayList* pPackageCommands);

protected:
	bool mRejectAssets;
	String* mJobLabel;
	String* mValidSlaves;

	PackageList* PerformNetworkPackageMerge(ArrayList* pPackageCommands, BackgroundWorker* worker);
	PackageList* UpdatePackageRelationships(ArrayList* pPackageCommands, BackgroundWorker* worker);

	// Package Merge Process Routines
	bool PrepareWorkingDirectories(PackageList* packageFiles);
	ArrayList* GetPackageToolEnvironment(String* relativeToolPath, MOG_PackageMerge_PackageFileInfo* packageFileInfo);
	PackageList* PreparePackageMergeTasks(PackageList* packageMergeTasks);
	bool ParseOutputTaskFile_RecordLateResolvers(MOG_PackageMerge_PackageFileInfo* packageFileInfo);
	PackageList* ProcessAffectedPackages(PackageList* affectedPackageFiles, ArrayList* pPackageCommands, BackgroundWorker* worker);
	HybridDictionary* GetAssetsToRejectForFailedPackage(MOG_PackageMerge_PackageFileInfo* pFailedPackageFileInfo, ArrayList* pPackageCommands);

	// Network Process Routines
	PackageList* ExecuteNetworkPackagingCommands(ArrayList* pPackageCommands);
	PackageList* ExecuteNetworkPackageMergeCommand(MOG_PackageMerge_PackageFileInfo* packageFileInfo, MOG_DBPackageCommandInfo* packageCommand);
	PackageList* GetPackageFilesFromPackageCommands(ArrayList* pPackageCommands);
	bool BlessAffectedPackageFile(MOG_PackageMerge_PackageFileInfo* packageFileInfo, ArrayList* pPackageCommands);
	void RemoveAffectedPackageCommands(MOG_PackageMerge_PackageFileInfo* packageFileInfo, ArrayList* pPackageCommands);
	ArrayList* GetScheduledNetworkPackageCommands(MOG_Filename* packageAssetFullName, String* jobLabel, String* branchName);
	ArrayList* GetAssociatedLateResolvers(ArrayList* pPackageCommands, String* branchName);
	bool RejectFailedAssets(ArrayList* pPackageCommands, BackgroundWorker* worker);
	virtual bool MOG_ControllerPackageMergeNetwork::RejectFailedAsset(MOG_Filename* blessedAssetFilename, String* comment, BackgroundWorker* worker);
	static bool ScheduleGenericAssetPackageCommands(MOG_ControllerAsset* platformSpecificAsset, MOG_PACKAGECOMMAND_TYPE packageCommandType, String* jobLabel, String* validSlaves);
};

}
}
}


#endif	// __MOG_CONTROLLERPACKAGEMERGENETWORK_H__

