//--------------------------------------------------------------------------------
//	MOG_AssetAPI.h
//	
//	
//--------------------------------------------------------------------------------
#ifndef __MOG_DBASSETAPI_H__
#define __MOG_DBASSETAPI_H__


#using <mscorlib.dll>
#using <system.dll>
#using <System.Data.dll>

#include "stdlib.h"
#include "MOG_Property.h"
#include "MOG_Filename.h"
#include "MOG_DBProjectAPI.h"


using namespace System;
using namespace System::Data;
using namespace System::Data::SqlClient;


// Circular reference
namespace MOG {
namespace CONTROLLER {
namespace CONTROLLERASSET {
	public __gc class MOG_ControllerAsset;
}
}
}


namespace MOG {
namespace DATABASE {

//public __gc struct MOG_DBProperty
//{
//	String *mSection;
//	String *mProperty;
//	String *mValue;
//};

public __gc struct MOG_ClassificationProperties
{
	String *mClassification;
	ArrayList *mProperties;
};

public __gc struct MOG_DBAssetProperties
{
	MOG_Filename *mAsset;
	ArrayList *mProperties;
};

public __gc struct MOG_DBSourceFile
{
	String *mSourceFile;
	String *mSourcePath;
};

public __gc struct MOG_DBSyncTargetInfo
{
private:
    String *mFilenameOnly;
    String *mPath;
    void InitializeFilenameAndPath();
public:
    String *mSyncTargetFile;
    String *mVersion;
    String *mAssetLabel;
    String *mAssetPlatform;
    String *mAssetClassification;
    __property String *get_FilenameOnly();
    __property String *get_Path();
    MOG_DBSyncTargetInfo( String *syncTargetFile, String *version, String *assetLabel, String *assetPlatform, String *assetClassification );
};

public __gc class MOG_DBAssetAPI
{
public:
	// Asset
	static bool AddAsset(MOG_Filename *blessedAssetFilename, String *createdBy, String *createdDate);
	static bool AddAssetVersionsToBranch(ArrayList *listOfPostInfo);
	static bool AddAssetVersionToBranch(MOG_Filename *assetFilename, String *versionTimeStamp);
	static bool AddAssetRelatedFiles(MOG::CONTROLLER::CONTROLLERASSET::MOG_ControllerAsset *pAssetController);
	static bool AddAssetRelatedFilesForPlatform(MOG_Filename* assetFilename, MOG_Properties* assetProperties, String *platformName);
	static bool RemoveAssetFromBranch(MOG_Filename *pAsset, String *branch);
	static ArrayList *GetAllAssets();
	static ArrayList *GetAllAssets(bool bExcludeLibrary);
	static ArrayList *GetAllAssets(bool bExcludeLibrary, String *branchName);
	static ArrayList *GetAllAssets(String* classification, bool bExcludeLibrary);
	static ArrayList *GetAllAssetsAllRevisionsForProject(String *projectName);
	static ArrayList *GetAssetNames();
	static String *GetAssetVersion(MOG_Filename *pAsset);
	static ArrayList *GetAllAssetRevisions(MOG_Filename *pAsset);
	static ArrayList *GetAllUnusedAssetRevisions(MOG_Filename *pAsset);
	static ArrayList *GetAllAssetsByVersion(String *version);

	static ArrayList *GetAllAssetsByClassification(String *classification);
	static ArrayList *GetAllAssetsInClassificationTree(String *classification);
	static int GetDependantAssetCount(String *rootClassification);
	static ArrayList *GetAllAssetsByClassification(String *classification, MOG_Property *property);
	static ArrayList *GetAllCurrentAssetsByClassification(String *classification, bool bExcludeLibrary);
	static ArrayList *GetAllCurrentAssetsByClassification(String *classification, bool bExcludeLibrary, String *branchName);
	static ArrayList *GetAllAssetsByClassificationAndProperties(String *classification, ArrayList *properties, bool bIncludeSubClassifications);
	static ArrayList *GetAllArchivedAssets();
	static ArrayList *GetAllArchivedAssetsByClassification(String *classification);
	static ArrayList *GetAllArchivedAssetsByClassificationAndProperties(String *classification, ArrayList *properties, bool bIncludeSubClassifications);
	static ArrayList *GetAllArchivedAssetsByParentClassification(String *classification);
	static ArrayList *GetAllArchivedAssetsByPlatform(String *platform);
	static ArrayList *GetAllAssetsByParentClassification(String *classification);
	static ArrayList *GetAllAssetsByParentClassificationWithProperties(String *classification, ArrayList *properties);
	static ArrayList *GetAllAssetsByParentClassificationWithProperties_ThatInheritProperty(String *classification, MOG_Property *property);
	static ArrayList *GetAllArchivedAssetsByParentClassificationWithProperties(String *classification, ArrayList *properties);
	static ArrayList *GetAllArchivedAssetVersionsByAssetNameWithProperties(String *assetName, ArrayList *properties);
	static ArrayList *GetAllAssetsByLabel(String *label);
	static ArrayList *GetAllAssetsLikeLabel(String *label);
	static ArrayList *GetAllAssetsByPlatform(String *platform);
	static ArrayList *GetAllAssetsByProperty(MOG_Property *property);
	static ArrayList *GetAllAssetsByProperty(String *classification, MOG_Property *property);
	static ArrayList *GetAllAssetsByProperty(String *classification, MOG_Property *property, bool bUseNotEqualComparison);
	static ArrayList *GetAllArchivedAssetsByProperty(MOG_Property *property);
	static ArrayList *GetAllAssetsBySyncLocation(String *syncLocation, String *platform);
	static ArrayList *GetAllCurrentAssetsBySyncLocation(String *syncLocation, String *platform, bool bExcludeLibrary);
	static ArrayList *GetAllCurrentAssetsBySyncLocation(String *syncLocation, String *platform, bool bExcludeLibrary, String *branchName);
	static ArrayList *GetAllAssetsBySyncLocationWithProperties(String *syncLocation, String *platform, ArrayList *properties);
	static ArrayList *GetAssetsWithProperties(String* selectString, ArrayList * properties);
	static ArrayList *GetAssetsWithPropertiesFilterByPropertyValue(String* selectString, ArrayList * properties, String* rootNode, ArrayList *mogPropertiesToFilterOn, bool excludeValue);
	static bool DoesAssetVersionExist(MOG_Filename *assetName, String *version);
	static ArrayList *GetAssetRevisionReferences(MOG_Filename *assetName, String *version, bool bGetAllReferences);
	static ArrayList *GetAssetRevisionReferencesInBranches(MOG_Filename *assetName, String *version);
	static ArrayList *GetAssetRevisionReferencesInPackages(MOG_Filename *assetName, String *version);
	static ArrayList *GetAssetRevisionReferencesInPendingPostCommands(MOG_Filename *assetName, String *version);
	static ArrayList *GetAssetRevisionReferencesInPendingPackageCommands(MOG_Filename *assetName, String *version);
	static ArrayList *GetAssetRevisionReferencesInWorkspaces(MOG_Filename *assetName, String *version);
	static ArrayList *GetBranchReferencesForAsset(int assetVersionID);
	static ArrayList *GetPackageReferencesForAsset(int assetVersionID);
	static ArrayList *GetPendingPostCommandsForAsset(MOG_Filename *assetName, String *revision);
	static ArrayList *GetPendingPackageCommandsForAsset(MOG_Filename *assetName, String *revision);
	static ArrayList *GetLocalWorkspaceReferencesForAsset(int assetVersionID);
	static bool RemoveAssetName(int assetNameID);
	static bool RemoveAssetName(MOG_Filename *assetFilename);
	static bool RemoveAssetVersion(MOG_Filename *assetName, String *version);
	static int GetCurrentAssetVersionID(MOG_Filename *assetName);
	static MOG_Filename *GetAssetName(int nameID);
	static ArrayList *GetAssetNameIDs(MOG_Filename *pAsset);
	static int GetAssetNameID(MOG_Filename *pAsset);

	static int GetPlatformID(String *platform);

	static bool RemoveAllAssets();

	// Classifications
	static bool CreateClassification(String *classification, String *createdBy, String *createdDate);
	static bool CreateClassification(String *classification, String *createdBy, String *createdDate, bool bAddToActiveBranch);
	static bool RemoveClassification(String *classification);
	static bool RemoveClassification(String *classification, String *branchName);
	static bool RenameClassificationName(String* oldClassification, String* newClassification);
	static bool RenameAdamClassificationName(String* oldAdamClassificationName, String* newAdamClassificationName);
	static int GetClassificationID(String *classification);
	static String *GetClassificationFullTreeNameByID(int classificationID);
	static bool DoesClassificationExistInAnyBranch(String *classification);
	static int GetClassificationIDForBranch(String *classification);
	static int GetClassificationIDForBranch(String *classification, String *branch);
	static int AddSubClassification(int parentID, String *name, String *fullTreeName, String *createdBy, String *createdDate, bool bAddToActiveBranch);
	static ArrayList *GetClassificationChildren(String *classificationParent);
	static ArrayList *GetClassificationChildren(String *classificationParent, String *branchName);
	static ArrayList *GetClassificationChildren(String *classificationParent, String *branchName, ArrayList *properties);
	static ArrayList *GetClassificationChildren(String *classificationParent, String *branchName, ArrayList *properties, bool bUseNotEqualComparison);
	static ArrayList *GetArchivedClassificationChildren(String *classificationParent);
	static ArrayList *GetAllClassifications();
	static ArrayList *GetAllActiveClassifications();
	static ArrayList *GetAllActiveClassifications(MOG_Property *property);
	static ArrayList *GetAllActiveClassifications(MOG_Property *property, bool bUseNotEqualComparison);
	static ArrayList *GetAllActiveClassifications(String *rootClassification, MOG_Property *property, bool bUseNotEqualComparison);
	static ArrayList *GetAllActiveClassificationsByRootClassification(String *rootClassification);

	// Classifications Properties
	static bool AddAssetClassificationProperty(String *classification, String *branch, String *section, String *property, String *value);
	static bool AddAssetClassificationProperties(String *classification, String *branch, ArrayList *properties);
	static bool RemoveAssetClassificationProperty(String *classification, String *branch, String *section, String *property);
	static bool RemoveAllAssetClassificationProperties(String *classification, String *branch);
	static bool RemoveAllAssetClassificationProperties(String *classification, String *branch, String *section);
	static String *GetAssetClassificationProperty(String *classification, String *branch, String *section, String *property);
	static ArrayList *GetAllAssetClassificationProperties(String *classification, String *branch);
	static ArrayList *GetAllAssetClassificationProperties(String *classification, String *branch, String *section);
	static ArrayList *GetAllDerivedAssetClassificationProperties(String *classification, String *branch);

	// SyncTargetFileMap
	static bool AddSyncTargetFileLink(String *syncTargetFile, MOG_Filename *assetName, String *platform);
	static bool AddSyncTargetFileLinks(ArrayList *syncTargetFiles, MOG_Filename *assetName, String *platform);
	static bool DoesSyncTargetFileLinkExistForAsset(MOG_Filename *assetName, String* platformName);
	static bool RemoveAllSyncTargetFileLinksForAsset(MOG_Filename *assetName);
	static ArrayList *GetSyncTargetFileAssetLinks(String *syncTargetFile, String *platformName);
	static ArrayList *GetSyncTargetFileAssetLinksFileNameOnly(String *syncTargetFile, String *platformName);
	static ArrayList *GetSyncTargetFileLinks(MOG_Filename *assetName, String *platformName);
	static ArrayList *GetAllProjectSyncTargetFilesForPlatform(String *platformName);
	static ArrayList *GetAllProjectSyncTargetInfosForPlatform( String *platformName );
	static ArrayList *GetAllProjectSyncTargetInfosForPlatform( String *platformName, String *exclusion );
	static ArrayList *GetAllProjectSyncTargetInfosForPlatformExcludeLibrary( String *platformName );
	static ArrayList *GetAllProjectSyncTargetFilesForDirectory(String *gameDataRoot, String *path, String *platformName);

	// Properties
	static bool AddAssetVersionProperty(MOG_Filename *assetName, String *assetVersion, String *section, String *property, String *value);
	static bool AddAssetVersionProperties(MOG_Filename *assetName, String *assetVersion, ArrayList *properties);
	static bool RemoveAssetVersionProperty(MOG_Filename *assetName, String *assetVersion, String *section, String *property);
	static bool RemoveAllAssetVersionProperties(MOG_Filename *assetName, String *assetVersion);
	static bool RemoveAllAssetVersionProperties(MOG_Filename *assetName, String *assetVersion, String *section);
	static String *GetAssetVersionProperty(MOG_Filename *assetName, String *assetVersion, String *section, String *property);
	static ArrayList *GetAllAssetVersionProperties(MOG_Filename *assetName, String *assetVersion);
	static ArrayList *GetAllAssetVersionProperties(MOG_Filename *assetName, String *assetVersion, String *section);

protected:
    static String *GetAssetVersionsToBranch_UpdateSQL(MOG_Filename *blessedAssetFilename, int oldVersionID, int newVersionID);
	static String *GetAssetVersionsToBranch_InsertSQL(MOG_Filename *blessedAssetFileName, int assetVersionID);
//McCoy	Fix all these functions to utilize the ProjectName and BranchName
	static bool SQLWriteAsset(MOG_Filename *asset, String *version);
//	static MOG_Filename *SQLReadAsset(SqlDataReader *myReader);
//	static ArrayList *QueryAssets(String *selectString);
//	static int GetAssetVersionID(MOG_Filename *assetName, String *version);
	static MOG_Filename *QueryAsset(String *selectString);
//	static int CreateAssetRevision(int assetNameID, String *version, int createdByID, String *createdDate);

	static bool SQLCreateAssetProperty(MOG_Filename *assetName, String *version, String *section, String *property, String *value);
	static bool SQLCreateAssetClassificationProperty(String *classification, String *branch, String *section, String *property, String *value);
	static MOG_Property *SQLReadProperty(SqlDataReader *myReader);
//	static ArrayList *QueryProperties(String *selectString);
	static MOG_Property *QueryProperty(String *selectString);

	static void GetAllClassificationChildrenIDs(String *parentID, ArrayList *children);
	static void GetAllClassificationChildrenIDsInBranch(String *parentID, String *branchID, ArrayList *children);

	static ArrayList *QueryDerivedAssetClassificationProperties(String *selectString, ArrayList *classifications);

	static MOG_DBAssetProperties *SQLReadAssetWithProperties(SqlDataReader *myReader, ArrayList *properties);
	static MOG_DBAssetProperties *SQLReadAssetWithProperties_UseFriendlyPropertyNames(SqlDataReader *myReader, ArrayList *properties);	
	static MOG_DBAssetProperties *SQLReadAssetWithPropertiesWithInheritanceFilterByProperyValue(SqlDataReader *myReader, ArrayList *properties, ArrayList *mogPropertiesWithValue, bool excludeValue, String *rootClassfication, ArrayList *listOfClassificationsWithProperyObjects );
	static ArrayList *QueryAssetsWithProperties(String* selectString, ArrayList *properties);

	static MOG_DBSourceFile *SQLReadFullSourceFile(SqlDataReader *myReader);
	static ArrayList *QueryFullSourceFileLinks(String *selectString);

	static bool DoesClassificationMatch(String *classification, Hashtable* classHash);

	static ArrayList *RemoveDuplicateAssetNamesFromList(ArrayList *assets);
	static String* PrepareValueForSQLComparison(String* value);
	static String* GetProperSQLComparisonString(String* value, bool bUseNotEqualComparison);

// JohnRen - Managed C doesn't support friends!
// We still need these functions exposed to the other Database classes...Grrr!
// for the mean time, lets expose only these functions
public:
	static MOG_Filename *SQLReadAsset(SqlDataReader *myReader);
	static ArrayList *QueryAssets(String *selectString);
	static int GetAssetVersionID(MOG_Filename *assetName, String *version);
	static ArrayList *QueryProperties(String *selectString);
	static int CreateAssetName(MOG_Filename *asset);
	static int CreateAssetName(MOG_Filename *asset, String* createdBy);
	static int CreateAssetName(MOG_Filename *asset, String* createdBy, String *createdDate);
	static int CreateAssetRevision(int assetNameID, String *version, int createdByID, String *createdDate);
	static bool UpdateRevisionsWithReplacementAssetNameID(int oldAssetNameID, int newAssetNameID);

private:
	static void AddAssetVersionsToBranch_Worker(Object* sender, DoWorkEventArgs* e);
};


}
}

using namespace MOG;
using namespace MOG::DATABASE;

#endif


