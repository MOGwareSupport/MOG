//--------------------------------------------------------------------------------
//	MOG_ControllerPackageMergeLocal.h
//	
//	
//--------------------------------------------------------------------------------

#ifndef __MOG_CONTROLLERPACKAGEMERGELOCAL_H__
#define __MOG_CONTROLLERPACKAGEMERGELOCAL_H__

#include "MOG_ControllerPackageMerge.h"


namespace MOG {
namespace CONTROLLER {
namespace CONTROLLERPACKAGE {


public __gc class MOG_ControllerPackageMergeLocal : public MOG_ControllerPackageMerge
{
public:
	MOG_ControllerPackageMergeLocal();

	PackageList *DoPackageEvent(String *workspaceDirectory, String *platformName, BackgroundWorker* worker);

protected:
	ArrayList*		mAssetsToAdd;
	ArrayList*		mAssetsToRemove;
	ArrayList*		mPackagesToRemove;
	bool			mWarnUnmodifiedPackages;

	PackageList *PerformLocalPackageMerge(BackgroundWorker* worker);

	// Package Merge Process Routines
	virtual bool CleanWorkingDirectories_EntireWorkspace(MOG_PackageMerge_PackageFileInfo *packageFileInfo);
	virtual bool PrepareWorkingDirectories(PackageList *packageFiles);
	virtual PackageList *PreparePackageMergeTasks(PackageList *packageMergeTasks);
	virtual bool ParseOutputTaskFile_ReportErrors(MOG_PackageMerge_PackageFileInfo* packageFileInfo);
	virtual PackageList *ProcessAffectedPackages(PackageList *affectedPackageFiles, BackgroundWorker* worker);
	void MarkedFailedAssets(ArrayList *assets);
	void MarkedFailedAssets(PackageList* failedPackages, ArrayList *assets);

	// Local Process Routines
	ArrayList *GetPackageToolEnvironment(String *relativeToolPath, MOG_PackageMerge_PackageFileInfo* packageFileInfo);
	PackageList *ExecuteLocalPackagingCommands(ArrayList *assetsToMerge);
	PackageList *PackagePackagedPackages(PackageList *processedPackageFiles, BackgroundWorker* worker);
	PackageList *ExecuteLocalPackageMergeCommand(MOG_Filename *assetFilename);

	PackageList *OptimizeAffectedPackageFilesPendingRemoval(PackageList* affectedPackageFiles);

	virtual bool DetectLocalAssetsToMerge();
	PackageList *GetPackageInfoListFromAsset(MOG_Filename *assetFilename);
	PackageList *GetPackageInfoListFromAssets(ArrayList *assets);
	void FinalizeLocalAssets(ArrayList* assetsToMerge);
	bool FinalizeLocalAsset(MOG_ControllerAsset* asset);
};

}
}
}


#endif	// __MOG_CONTROLLERPACKAGEMERGELOCAL_H__

