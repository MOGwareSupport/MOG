//--------------------------------------------------------------------------------
//	MOG_SystemUtilities.h
//	
//	
//--------------------------------------------------------------------------------

#ifndef __MOG_SYSTEMUTILITIES_H__
#define __MOG_SYSTEMUTILITIES_H__


namespace MOG
{
public __gc class BASE;
}


namespace MOG {
namespace SYSTEMUTILITIES {


public __gc class MOG_SystemUtilities {

public:
	// New System Utilities
	static ArrayList *GetAutomatedTestNames(void);
	static bool AutomatedTesting_Drone(String *testName, int testDuration_Seconds, String *projectName, String *testImportFile, bool bCopyFileLocal, int startingExtensionIndex);
	static void AutomatedTesting_Drone_ImportProcessBless_Thread();
	static void AutomatedTesting_Drone_ImportProcessBless_Worker(Object* sender, DoWorkEventArgs* e);
	static void AutomatedTesting_Drone_CopyNetworkFiles_Thread();
	static void AutomatedTesting_Drone_CopyNetworkFiles_Worker(Object* sender, DoWorkEventArgs* e);
	static void AutomatedTesting_Drone_LibraryCheckOutIn_Thread();
	static void AutomatedTesting_Drone_LibraryCheckOutIn_Worker(Object* sender, DoWorkEventArgs* e);
	static bool AutomatedTesting_UserInbox(int testDuration_Seconds, int testBlessPercent, int testProcessPercent);
	static void AutomatedTesting_UserInbox_Thread();
	static void AutomatedTesting_UserInbox_Worker(Object* sender, DoWorkEventArgs* e);
	static void CheckAssets_VerifyBlessedFile(MOG_Filename *assetFilename, String **sourceCopyFile, String **targetCopyFile);


	static bool FixGameDataTable_MissingSlashes();

	static ArrayList* AdminToolsReportUnreferencedRevisions(String *classification);
	static bool CleanInvalidItems();
	static ArrayList* RepairRevisionMetadata(String *classification);
	static ArrayList* FindNewerUnpostedRevisions(String *classification);
	static ArrayList* FindAssetWithMultiplePackageAssignments(String *classification);
	static ArrayList* FindCollidingPackageAssignments(String *classification);
	static ArrayList* FindInvalidPackageAssignments(String *classification);
	static bool UpdateProjectFor304XDrop();
	
	// Old System Utilities - Most like need to be yanked!!!
	static bool CheckUserOutBox_FindStaleAssets(String *projectPath, String *userName, String *assetPattern);
	static ArrayList *RepairAssetDirectories(ArrayList *assets, bool bAutoRepair);
	static ArrayList *BuildBlessedAssetsList(String *blessedPath);
	static bool CheckCurrentBlessedAssets_FindMissingVersions(String *projectPath, String *assetPattern);
	static bool SyncProjectFrom(String *sourceMogPath, String *sourceProjectPath, String *branchName, String *assetPattern);
	static bool RebuildPackageDatabaseLists(String *projectName, String *branchName);
	static ArrayList *CheckBlessedAssets_VerifyPackageAssetIntegrity(String *packageName);
	static ArrayList *CheckBlessedAssets_VerifyPackageIntegrity(String *packageName);
	static ArrayList *ScanDataBaseForOutOfSyncAssets(String *branchName);
	static ArrayList *CheckAssets_FindCollidingAssets(ArrayList *assets);
	static ArrayList *CheckAssets_BuildPlatformSyncTargetFileList(ArrayList *assets, String *platform);
	static bool TestLocks(String *lockName);
	static bool PopulateDataBaseTables(String *projectName, String *branchName);
	static bool DatabaseVersion6_UpdatePropertiesIniFile(MOG_Filename *assetToUpdate);

private:
	static void AdminToolsReportUnreferencedRevsions_Worker(Object* sender, DoWorkEventArgs* e);
	static void CleanInvalidItems_Worker(Object* sender, DoWorkEventArgs* e);
	static void RepairRevisionMetadata_Worker(Object* sender, DoWorkEventArgs* e);
	static void FindNewerUnpostedRevisions_Worker(Object* sender, DoWorkEventArgs* e);
	static void FindAssetWithMultiplePackageAssignments_Worker(Object* sender, DoWorkEventArgs* e);
	static void FindCollidingPackageAssignments_Worker(Object* sender, DoWorkEventArgs* e);
	static void FindInvalidPackageAssignments_Worker(Object* sender, DoWorkEventArgs* e);
	static void UpdateProjectFor304XDrop_Worker(Object* sender, DoWorkEventArgs* e);

private:
	static int mAutomatedTesting_Duration;
	static String *mAutomatedTesting_TestImportFile;
	static int mAutomatedTesting_TestBlessPercent;
	static int mAutomatedTesting_TestProcessPercent;
	static int mAutomatedTesting_AssetIndexNumber = 0;
};


}
}

using namespace MOG::SYSTEMUTILITIES;

#endif //__MOG_SYSTEMUTILITIES_H__
