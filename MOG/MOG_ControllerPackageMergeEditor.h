//--------------------------------------------------------------------------------
//	MOG_ControllerPackageMergeEditor.h
//	
//	
//--------------------------------------------------------------------------------

#ifndef __MOG_CONTROLLERPACKAGEMERGEEDITOR_H__
#define __MOG_CONTROLLERPACKAGEMERGEEDITOR_H__

#include "MOG_ControllerPackageMergeLocal.h"


namespace MOG {
namespace CONTROLLER {
namespace CONTROLLERPACKAGE {


public __gc class MOG_ControllerPackageMergeEditor : public MOG_ControllerPackageMergeLocal
{
public:
	MOG_ControllerPackageMergeEditor();

protected:
	// Package Merge Process Routines
	PackageList *ExecutePackageMergeTasks(PackageList *packageMergeTasks);
	bool DetectLocalAssetsToMerge();
	PackageList* DetectAffectedPackageFiles(MOG_PackageMerge_PackageFileInfo* packageFileInfo);
};

}
}
}


#endif	// __MOG_CONTROLLERPACKAGEMERGEEDITOR_H__

