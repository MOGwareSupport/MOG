//--------------------------------------------------------------------------------
//	MOG_ControllerPackageRebuildNetwork.h
//	
//	
//--------------------------------------------------------------------------------

#ifndef __MOG_CONTROLLERPACKAGEREBUILDNETWORK_H__
#define __MOG_CONTROLLERPACKAGEREBUILDNETWORK_H__

#include "MOG_ControllerPackageMergeNetwork.h"


namespace MOG {
namespace CONTROLLER {
namespace CONTROLLERPACKAGE {


public __gc class MOG_ControllerPackageRebuildNetwork : public MOG_ControllerPackageMergeNetwork
{
public:
	PackageList *DoPackageEvent(MOG_Filename *packageFilename, String *jobLabel, String *branchName, String *workspaceDirectory, String *platformName, String *validSlaves, BackgroundWorker* worker);

protected:
	bool RejectFailedAsset(MOG_Filename *blessedAssetFilename, String *comment, BackgroundWorker* worker);
};

}
}
}


#endif	// __MOG_CONTROLLERPACKAGEREBUILDNETWORK_H__

