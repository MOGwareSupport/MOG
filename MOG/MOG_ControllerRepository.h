//--------------------------------------------------------------------------------
//	MOG_ControllerRepository.h
//	
//	
//--------------------------------------------------------------------------------

#ifndef __MOG_CONTROLLERREPOSITORY_H__
#define __MOG_CONTROLLERREPOSITORY_H__

#include "MOG_Main.h"
#include "MOG_INI.h"
#include "MOG_Filename.h"
#include "MOG_System.h"
#include "MOG_Time.h"
#include "MOG_ControllerRepository.h"
#include "MOG_ControllerAsset.h"


namespace MOG {
namespace CONTROLLER {
namespace CONTROLLERREPOSITORY {


public __gc class MOG_ControllerRepository
{
protected:
	static String *MOG_ControllerRepository::VerifyTimestamp(String *timestamp);
	static bool AddUniqueFilename(ArrayList *array, String *revision);

public:
	// Bless
	static bool AddAssetRevisionInfo(MOG_ControllerAsset *blessedAsset, bool bPurgeOldProperties);

	// Blessed
	static String *GetRepositoryPath();
	static MOG_Filename *GetAssetBlessedPath(MOG_Filename *assetFilename);
	static MOG_Filename *GetAssetBlessedVersionPath(MOG_Filename *assetFilename, String *timestamp);
	static ArrayList *GetBlessedRevisions(MOG_Filename *assetFilename, bool bCheckDatabase, bool bCheckDisk);

	// Archived
	static String *GetArchivePath();
	static MOG_Filename *GetAssetArchivedPath(MOG_Filename *assetFilename);
	static MOG_Filename *GetAssetArchivedVersionPath(MOG_Filename *assetFilename, String *timestamp);
	static ArrayList *GetArchivedRevisions(MOG_Filename *assetFilename, bool bCheckDatabase, bool bCheckDisk);
};

}
}
}

using namespace MOG::CONTROLLER::CONTROLLERREPOSITORY;

#endif	// __MOG_CONTROLLERREPOSITORY_H__




