//--------------------------------------------------------------------------------
//	MOG_ControllerPackage.h
//	
//	
//--------------------------------------------------------------------------------

#ifndef __MOG_CONTROLLERPACKAGE_H__
#define __MOG_CONTROLLERPACKAGE_H__

#include "MOG_Main.h"
#include "MOG_INI.h"
#include "MOG_Filename.h"
#include "MOG_ControllerAsset.h"
#include "MOG_Command.h"


namespace MOG {
namespace CONTROLLER {
namespace CONTROLLERPACKAGE {

public __gc class MOG_ControllerPackage
{
public:
	static bool RebuildPackage(MOG_Filename *assetFilename, String *jobLabel);

	// Package Parsing Functions
	static String* GetPackageName(String* AssignedPackage);
	static String* GetPackagePlatform(String* AssignedPackage);
	static String* GetPackageGroupObjectPath(String* AssignedPackage);
	static String* GetPackageGroups(String* AssignedPackage);
	static String* GetPackageGroupParent(String* groups);
	static String* GetPackageGroupLevels(String* AssignedPackage)[];
	static String* GetPackageObjects(String* AssignedPackage);
	static String* GetPackageObjectLevels(String* AssignedPackage)[];
	static String* AddPackageObjectIdentifiers(String* packageObjects);
	static String* StripPackageObjectIdentifiers(String* packageObjects);
	
	static String* CombinePackageInternalPath(String* groups, String* objects);
	static String* CombinePackageGroups(String* group1, String* group2);
	static String* CombinePackageObjects(String* object1, String* object2);
	static String* CombinePackageAssignment(String* packageName, String* packageGroups, String* packageObjects);

	static bool RemoveGroup(MOG_Filename* assetFilename, String* packageGroup);

	// Package Utility Functions
	static String* MapPackageAssetNameToFile(MOG_Filename* packageName);
	static ArrayList* GetAssociatedAssetsForPackage(MOG_Filename* packageFilename);
	static ArrayList* GetAssetsInPackage( MOG_Filename* blessedPackageFilename );
	static ArrayList* GetPackageSubGroups( MOG_Filename* packageName, String* packageVersion );
	static ArrayList* GetAssetsInPackageGroup( MOG_Filename* packageName, String* packageVersion, String* packageGroupName);
	static String* GetLatestPackageRevision_IncludingUnpostedRevisions(MOG_Filename* packageAssetFilename, String* jobLabel);
};

}
}
}

using namespace MOG::CONTROLLER::CONTROLLERPACKAGE;

#endif	// __MOG_CONTROLLERPACKAGE_H__




