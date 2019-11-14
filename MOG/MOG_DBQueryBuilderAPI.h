//--------------------------------------------------------------------------------
//	MOG_DBAPI.h
//	
//	
//--------------------------------------------------------------------------------
#ifndef __MOG_DBQUERYBUILDERAPI_H__
#define __MOG_DBQUERYBUILDERAPI_H__


#using <mscorlib.dll>
#using <system.dll>
#using <System.Data.dll>

#include "stdlib.h"
#include "MOG_Define.h"
#include "MOG_DBPackageAPI.h"


using namespace System;
using namespace System::Data;
using namespace System::Data::SqlClient;
using namespace System::Collections;
using namespace System::Threading;

namespace MOG {
namespace DATABASE {

public __gc class MOG_DBQueryBuilderAPI
{
public:
	//Class containing Report Queries
	__gc class MOG_ReportQueries
	{
	public:
		//This function returns a select query to retrieve  list of all active assets based on the passed SyncTarget. (Used in the Sync Target View)
		static String *GenerateReport_GetActiveAssetsBySyncTarget(String *rootSyncTargetPath, String *platformName, ArrayList *propertiesToInclude);
		//This function returns a select query to retrieve a list of all active packages based by the classification which was passed. (Used in the Package View)
		static String *GenerateReport_GetActivePackagesByClassification(String *rootClassification, ArrayList *propertiesToInclude);
		//This function returns a select query to retrieve a list of the assests contained in a given package. (Used in the Package View on a specific Package)
		static String *GenerateReport_GetAssetsContainedInPackage(String *packageName, int packageVersion, ArrayList *propertiesToInclude);
		//This function returns a select query to retrieve a list of the most current assets (active or not) (Used in the archive view)
		static String *GenerateReport_GetAssetsByClassification(String *rootClassification, ArrayList *propertiesToInclude, bool activeOnly);

		static String *GenerateReport_GetArchiveAssetsByClassification(String *rootClassification, ArrayList *propertiesToInclude);
		//A function which returns a select query to retrieve all the archived assests at an asset level in the Archive View
		static String *GenerateReport_GetAllRevisionsOfAsset(MOG_Filename *assetFilename, ArrayList *propertiesToInclude);
		static String *MOG_ReportQueries::GenerateReport_GetAllRevisionsOfAssets(String *rootClassification, ArrayList *propertiesToInclude);
		
		static String *GenerateReport_PackageAssets(MOG_Filename *packageName, ArrayList *propertiesToInclude, String *platform);
		static String *SqlStringForAllAssetsWithPropertiesWherePackageIsTrueOrNull(String *rootClassification, ArrayList *propertiesToInclude);
	};

	//class containing TreeviewQueries
	__gc class MOG_TreeviewQueries
	{
	public:
		static String *QueryToGetClassificationsUnderRootClassification(String * rootNode, MOG_Property *propertyToFilterOn);
	};
	//class containing cache queries
	__gc class MOG_DBCacheQueries
	{
	public:
		static  String *PopulateCache_AssetVersionCache();
		static String *PopulateCache_AssetNameCache();
		static String *PopulateTemporaryCache_PreviousVersion();
		static String *MOG_DBCacheQueries::PopulateCache_ClassificationCache();
	};
	__gc class MOG_SyncDataQueries
	{
	public:
		static String *Query_AllCurrentAssetsInMachineUserWorkspace(String *computerName, String *projectName, String *platformName, String *workingDirectory, String *userName, int syncedLocationID);
		static String *Query_AllCurrentAssetsInMachineUserWorkspace_ByClassification(String *computerName, String *projectName, String *platformName, String *workingDirectory, String *userName, String *classification, int syncedLocationID);
		static String *Query_AllCurrentAssetsInMachineUserWorkspace_BySyncLocation(String *computerName, String *projectName, String *platformName, String *workingDirectory, String *userName, String *syncLocation, int syncedLocationID);
	};
	__gc class MOG_AssetQueries
	{
	public:
		static String *Query_AllCurrentAssetsForBranch();
		static String *Query_AllAssetsForBranch();
		static String *Query_AllAssetsForBranch(bool bExcludeLibrary);
		static String *Query_AllAssetsForBranch(bool bExcludeLibrary, String *branchName);		
		static String *Query_AllAssetsForBranchByClassification(String *classification);
		static String *Query_AllCurrentAssetsForBranchByClassificationTree(String *classification);
		static String *Query_AllCurrentAssetsForBranchByClassificationTree(String *classification, bool bExcludeLibrary);
		static String *Query_AllCurrentAssetsForBranchByClassificationTree(String *classification, bool bExcludeLibrary, String *branchName);
		static String *Query_AllAssetsForBranchBySyncLocation(String *syncLocation, String *platform);
		static String *Query_AllCurrentAssetsForBranchBySyncLocation(String *syncLocation, String *platform);
		static String *Query_AllCurrentAssetsForBranchBySyncLocation(String *syncLocation, String *platform, bool bExcludeLibrary);
		static String *Query_AllCurrentAssetsForBranchBySyncLocation(String *syncLocation, String *platform, bool bExcludeLibrary, String *branchName);
		static String *Query_AssetsWithPropertyValue(String *rootClassification, MOG_Property *propertyToFilterOn);
		static String *Query_AssetsWithProperties_Filtered(String *selectString, ArrayList *mogPropertiesToFilterOn, bool excludeValue);
		static String *Query_AllAssetsAllRevisionsForProject(String *projectName);
		static String *Query_AllCurrentAssetsWithPropertySet(MOG_Property *requiredProperty);
	};
	__gc class MOG_DatabaseManagementQueries
	{
	public:
		static String * Query_GetDefaultDataFilePath(String * databaseName);
		static String * Query_GetDefaultLogFilePath(String * databaseName);
		static String * Query_CreateMogDatabase(String * newDatabaseName, String *dataFilePath, String *logFilePath);
	};
	__gc class MOG_TableManagmentQueries
	{
	public:
		__gc class MOG_SystemTablesQueries
		{
		public:
			static String * Query_CreateCommandsTable(String *tableName);
			static String * Query_CreateDBVersionTable(String *tableName);
			static String * Query_CreateEventsTable(String *tableName);
		};
		__gc class MOG_ProjectTables
		{
		
		};
		static String *Query_CopyTable(String *sourceTable, String *destinationTable);
	};
	__gc class MOG_PropertyQueries
	{
	public:
	};
private:

	//This is a helper function that builds the select part of a Sql Query based on properties.
	static String *BuildQueryString_AddPropertiesToSELECT(ArrayList *properties, bool leadingComma);
	//This is a helper function that builds the FROM part of a Sql Query based on the properties we want to return. 
	static String *BuildQueryString_AddPropertyToFROM(String *section, ArrayList *properties, String *joinColumn);
	//This is a helper function which adds a WHERE constraint for a given property to a Sql Query
	static String *AddPropertyValueConstraint(MOG_Property *property, String *value);

	//This is a helper function that builds the FROM part of a Sql Query based on classifications properties we want to return. 
	static String *BuildQueryString_AddClassificationPropertyToFROM(String *section, ArrayList *properties, String *joinColumn);

	static String *BuildQueryString_AddFullFileNameToSELECT(String *fullTreeNameColumn, String *platformKeyColumn, String *assetLabelColumn);

	static String *FilteredQuery(String *selectString, ArrayList *mogPropertiesToFilterOn, bool excludeValue);

	static String *CreateSqlFileLocation(String *databaseName, String *dataFilePath);
	static String *CreateSqlLogFileLocation(String *databaseName, String *logFilePath);
	static String *CreateSqlDataFileLocation(String *databaseName, String *dataFilePath);
};
}
}

#endif