//--------------------------------------------------------------------------------
//	MOG_ControllerAsset.h
//	
//	
//--------------------------------------------------------------------------------

#ifndef __MOG_CONTROLLERASSET_H__
#define __MOG_CONTROLLERASSET_H__

#include "MOG_Filename.h"
#include "MOG_Ini.h"
#include "MOG_Time.h"
#include "MOG_Properties.h"



namespace MOG {
namespace CONTROLLER {
namespace CONTROLLERASSET {


typedef enum {
	AssetFiles_CopyIfMissing,
	AssetFiles_Add,
	AssetFiles_Remove,
	AssetFiles_CheckNotModified,
} AssetFilesFlag;



public __gc class MOG_ControllerAsset
{
public:
	MOG_ControllerAsset();
	~MOG_ControllerAsset(void);

	// Creation
	static MOG_Filename *CreateAsset(String *sourceFullName, String *targetName, bool move);
	static MOG_Filename *CreateAsset(String *assetName, String *assetRootPath, ArrayList *importItems, ArrayList *logFiles, ArrayList *properties, bool bMoveImportFiles, bool bMoveLogFiles);
	static MOG_Filename *CreateAsset(MOG_Filename *assetFilename, String *assetRootPath, ArrayList *importItems, ArrayList *logFiles, ArrayList *properties, bool bMoveImportFiles, bool bMoveLogFiles);
	static MOG_Filename *CreateAsset(String *assetName, String *assetRootPath, ArrayList *importItems, ArrayList *logFiles, ArrayList *properties, bool bMoveImportFiles, bool bMoveLogFiles, bool bAutoDetectMissingImportFiles);
	static MOG_Filename *CreateAsset(MOG_Filename *assetFilename, String *assetRootPath, ArrayList *importItems, ArrayList *logFiles, ArrayList *properties, bool bMoveImportFiles, bool bMoveLogFiles, bool bAutoDetectMissingImportFiles);
	// Reimport
	static bool ReimportAssetUsingLocalFiles(MOG_Filename *inboxAssetFilename, String* localWorkspaceDirectory);

	// Open/Close
	static MOG_ControllerAsset *OpenAsset(MOG_Filename *assetFilename)		{ return OpenAsset(assetFilename->GetEncodedFilename()); }
	static MOG_ControllerAsset *OpenAsset(MOG_Filename *assetFilename, MOG_Properties *pSubstituteProperties);
	static MOG_ControllerAsset *OpenAsset(String *assetFullFilename);
	bool Close(void);

	// Properties
	MOG_Properties *GetProperties(void);
	MOG_Properties *GetProperties(String *platformName);
	void SetSubtituteProperties(MOG_Properties *pSubstituteProperties)		{ mSubstituteProperties = pSubstituteProperties; }
	void SetPendingViewUpdate(MOG_Command *view)							{ mPendingViewUpdate = view; }

	// Comment
	bool PostComment(String *fullComment, bool bUseRevisionHeader);

	// Accessor Functions
	MOG_Filename *GetAssetFilename()										{ return mAssetFilename; }

	// Asset Support functions
	bool CleanAsset();
	static String *GetAssetImportedFilesSection(MOG_Properties *pProperties)													{ return GetAssetPlatformFilesSection(pProperties, S"", false); }
	static String *GetAssetPlatformFilesSection(MOG_Properties *pProperties, String *platformName, bool bValidateSection);
	static String *GetAssetImportedFiles(MOG_Properties *pProperties)[]															{ return GetAssetPlatformFiles(pProperties, S"", false); }
	static String *GetAssetPlatformFiles(MOG_Properties *pProperties, String *platformName, bool bValidateSection)[];
	static String *GetAssetImportedDirectory(MOG_Properties *pProperties)														{ return GetAssetFilesDirectory(pProperties, S"", false); }
	static String *GetAssetProcessedDirectory(MOG_Properties *pProperties, String *platformName)								{ return GetAssetFilesDirectory(pProperties, platformName, true); }
	static String *GetAssetFilesDirectory(MOG_Properties *pProperties, String *platformName, bool bValidateSection);
	String *GetFileList(String* platformName, bool bAddSyncTargetPath)[];
	String *GetFileList(String* platformName, String* workspace)[];
	bool IsProcessed();

	String *GetAssetPropertiesFilename();
	static String *GetAssetPropertiesFilename(MOG_Filename *assetFilename);
	String *GetAssetCommentsFilename();
	static String *GetAssetCommentsFilename(MOG_Filename *assetFilename);
	static String *GetCommonDirectoryPath(String *rootDirectoryPath, ArrayList *files);
	static String *SetDefaultGroup(MOG_ControllerAsset *asset);

	// Asset Validation Routines
	static bool ValidateAsset_IsUniqueSyncTarget(MOG_Properties* pProperties);
	static bool ValidateAsset_IsUniquePackageAssignment(MOG_Properties* pProperties);
	static ArrayList* ValidateAsset_GetCollidingSyncTargets(MOG_Properties *pProperties);
	static ArrayList* ValidateAsset_GetCollidingPackageAssignments(MOG_Properties *pProperties);
	static bool ValidateAsset_HasPackageAssignment(MOG_Properties *pProperties);
	static bool ValidateAsset_AllAssignedPackagesExist(MOG_Properties* pProperties);
	static ArrayList* ValidateAsset_GetMissingPackages(MOG_Properties* pProperties);
	static bool ValidateAsset_PlatformApplicable(MOG_Properties *pProperties, String* platformName);
	static bool ValidateAsset_IsAssetOutofdate(MOG_Filename *assetFilename);
	static bool ValidateAsset_IsAssetOutofdate(MOG_Filename *assetFilename, MOG_Properties *properties);

	// Database Utility Functions
	static ArrayList *GetAssetsByClassification( String *classification );
	static ArrayList *GetAssetsByClassification( String *classification, MOG_Property *propertyToFilterBy );
	static ArrayList *GetArchivedAssetsByClassification( String *classification );

	static bool RecordAllAssetFiles(MOG_ControllerAsset *asset);
	static bool RecordAllAssetFiles(MOG_Filename *assetFilename);
	static bool SetAssetSizes(MOG_ControllerAsset *asset);
	static bool SetAssetSizes(MOG_Filename *assetFilename);


protected:
	bool FetchFromPreviousAsset(ArrayList *importItems);

	void Initialize(void);

	String *FormatUserComment(String *comment);
	String *FormatRevisionComment(String *comment);

	bool FreeAssetLock(void);

private:
	static HybridDictionary *gAssetCache = new HybridDictionary();
	bool mOpened;
	String *mOpenedStackTrace;

	MOG_Filename *mAssetFilename;
	MOG_Properties *mProperties;
	MOG_Properties *mSubstituteProperties;
	MOG_Command *mPendingViewUpdate;

	bool Open(String *assetFullFilename);
	bool Open(MOG_Filename *assetFullFilename)							{ return Open(assetFullFilename->GetEncodedFilename()); }

	static String *GetCommonDirectoryPath_IdenticalStartingCharacterCount(String *strOne, String *strTwo);

	static bool CreateAsset_IsAssetRootPathValid(String *assetRootPath, ArrayList *importItems);
	static ArrayList *CreateAsset_ResolveImportItems(MOG_ControllerAsset *asset, ArrayList *importItems);
	static String *CreateAsset_BuildAssetPath(MOG_Filename *assetFilename);
	static bool CreateAsset_ResolveAlreadyExistingAsset(MOG_Filename *assetFilename);
	static bool CreateAsset_VerifyClassificationFilterCheck(MOG_ControllerAsset *asset, ArrayList *importFiles);
	static bool CreateAsset_SetImportProperties(MOG_ControllerAsset *asset);
	static bool CreateAsset_ProcessLogFiles(MOG_ControllerAsset *asset, ArrayList *logFiles, bool bMoveLogFiles);
	static bool CreateAsset_DetectMissingImportFiles(MOG_ControllerAsset *asset, ArrayList *importFiles);
	static bool CreateAsset_ProcessImportFiles(MOG_ControllerAsset *asset, ArrayList *importFiles, bool bMoveImportFiles);
	static bool CreateAsset_AutoPopulatePlaformSpecificFiles(MOG_ControllerAsset *asset, ArrayList *importFiles);
	static String *CreateAsset_SetFinalStatus(bool bFailed, MOG_ControllerAsset *asset);
	static bool CreateAsset_StampRepositoryAsset(MOG_ControllerAsset *asset);
};

}
}
}

using namespace MOG::CONTROLLER::CONTROLLERASSET;

#endif	// __MOG_CONTROLLERASSET_H__




