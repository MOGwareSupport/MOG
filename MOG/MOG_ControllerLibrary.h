//--------------------------------------------------------------------------------
//	MOG_ControllerLibrary.h
//	
//	
//---------------------------------------------------------------------------------

#ifndef __MOG_CONTROLLERLIBRARY_H__
#define __MOG_CONTROLLERLIBRARY_H__

#include "MOG_Main.h"
#include "MOG_ControllerSyncData.h"
#include "MOG_AllinatorPrompt.h"


namespace MOG {
namespace CONTROLLER {
namespace CONTROLLERLIBRARY {


public __gc class MOG_ControllerLibrary
{
public:
	// Directory info
	static String *GetWorkingDirectory();
	static void SetWorkingDirectory(String *workingDirectory);

	static bool	SetReadOnly(MOG_Filename *filename, bool readOnly);

	// Check In/Out Functions
	static bool GetLatest(ArrayList *assetFilenames);
	static bool GetLatest(MOG_Filename *assetFilename, BackgroundWorker* worker);
	static bool Unsync(MOG_Filename *assetFilename);
	static bool CheckOut(ArrayList *assetFilenames, String *comment);
	static bool CheckOut(MOG_Filename *assetFilename, String *comment);
	static bool CheckOut(MOG_Filename *assetFilename, String *comment, BackgroundWorker* worker);
	static bool CheckIn(ArrayList *assetFilenames, String *comment, bool bMaintainLock, String *jobLabel);
	static bool CheckIn(MOG_Filename *assetFilename, String *comment, bool bMaintainLock, String *jobLabel);
	static bool UndoCheckOut(ArrayList *assetFilenames);
	static bool UndoCheckOut(MOG_Filename *assetFilename);
	static bool UndoCheckOut(MOG_Filename *assetFilename, BackgroundWorker* worker);
	static bool AddDirectory(String *directory, String *classification);
	static bool AddFile(String *filename, String *classification);
	static bool AddFiles(ArrayList *filenames);
	static bool AddFiles(ArrayList *filenames, String *classification);
	static String* EnsureClassificationIsWithinLibrary(String *classification);
	static bool IsPathWithinLibrary(String *path);
	static String *ConstructLibraryClassificationFromPath(String *path);
	static String *ConstructPathFromLibraryClassification(String *classification);
	static MOG_Filename *ConstructAssetNameFromLibraryFile(String *filename);
	static String *ConstructLocalFilenameFromAssetName(MOG_Filename *assetFilename);
	static String *ConstructBlessedFilenameFromAssetName(MOG_Filename *assetFilename);

protected:
	static bool AddFile(String *filename, String *classification, String *jobLabel);
	static bool GetLatestDataForLibraryAsset(MOG_Filename *assetFilename, bool bSkipLockedAssets, BackgroundWorker* worker);
	static bool GetLatestAssetFiles_CanOverwriteFile(String *localFile, String *originalFile);	

	static void GetLatest_Worker(Object* sender, DoWorkEventArgs* e);
	static void CheckOut_Worker(Object* sender, DoWorkEventArgs* e);
	static void CheckIn_Worker(Object* sender, DoWorkEventArgs* e);
	static void UndoCheckOut_Worker(Object* sender, DoWorkEventArgs* e);
	static void AddDirectory_Worker(Object* sender, DoWorkEventArgs* e);
	static void AddFiles_Worker(Object* sender, DoWorkEventArgs* e);

protected:
	static String* mWorkingDirectory = S"";
	static MOG_AllinatorPrompt* mGetLatest_OverwriteFile = new MOG_AllinatorPrompt();
};

}
}
}

using namespace MOG::CONTROLLER::CONTROLLERLIBRARY;

#endif	// __MOG_CONTROLLERLIBRARY_H__
