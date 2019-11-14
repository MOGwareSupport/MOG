//--------------------------------------------------------------------------------
//	MOG_ControllerDemoProject.h
//	
//	
//--------------------------------------------------------------------------------

#ifndef __MOG_CONTROLLERDEMOPROJECT_H__
#define __MOG_CONTROLLERDEMOPROJECT_H__

#include "MOG_Main.h"
#include "MOG_INI.h"
#include "MOG_Filename.h"
#include "MOG_System.h"
#include "MOG_Project.h"
#include "MOG_Time.h"
#include "MOG_Privileges.h"
#include "MOG_ControllerAsset.h"
#include "MOG_ControllerSyncData.h"

namespace MOG {
namespace CONTROLLER {
namespace CONTROLLERDEMOPROJECT {


public __gc class MOG_ControllerDemoProject
{
public:
	static ArrayList *GetAllDemoProjectNames();

	static bool ImportDemoProject(String *projectName, BackgroundWorker* worker);
	static bool DemoizeProject(String *projectName, BackgroundWorker* worker);

	static bool AddDemoProject(String *importItem, BackgroundWorker* worker);
	static bool RemoveDemoProject(String *projectName, BackgroundWorker* worker);

	static bool ContainsDemoProject(String *demoProjectPath);
	static bool IsValidDemoProject(String *demoProjectPath);

protected:
	static bool ResolveRemovalOfAlreadyExistingDemoProject(String *projectName, BackgroundWorker* worker);

	static bool IsValidDemoProjectDirectory(String *directory);
	static bool IsValidDemoProjectCompressedFile(String *filename);
};

}
}
}

using namespace MOG::CONTROLLER::CONTROLLERDEMOPROJECT;

#endif	// __MOG_CONTROLLERDEMOPROJECT_H__




