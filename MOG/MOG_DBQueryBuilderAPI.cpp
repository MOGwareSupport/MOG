#include "stdafx.h"
#include "MOG_Database.h"
#include "MOG_ControllerProject.h"
#include "MOG_ControllerSystem.h"
#include "MOG_DBQueryBuilderAPI.h"

//////////////////////////////////////////////////////////////////////////////////////////////////////////////
//BEGINNING OF GENERAL PURPOSE QUERY BUILDER FUNCTIONS
//////////////////////////////////////////////////////////////////////////////////////////////////////////////

//This function builds the select portion for properties.
String *MOG_DBQueryBuilderAPI::BuildQueryString_AddPropertiesToSELECT(ArrayList *properties, bool leadingComma)
{
	String *addSelects = "";

	if(properties)
	{
		if(leadingComma )
		{
			addSelects = S",";
		}
		int i;
		//Iterate through all the properties and add the property to the Select statment of the query.
		for(i = 0; i < ((properties->Count-1)); i++)
		{
			MOG_Property *theProp = __try_cast<MOG_Property *>(properties->Item[i]);
			addSelects = String::Concat(addSelects, S"[Property",theProp->mPropertyKey,S"].[Value] AS [", theProp->mPropertyKey,S"],");
		}
		MOG_Property *theProp = __try_cast<MOG_Property *>(properties->Item[i]);
		addSelects = String::Concat(addSelects, S"[Property",theProp->mPropertyKey,S"].[Value] AS [", theProp->mPropertyKey,S"]");
	}
	return addSelects;
}

//This function builds the  FROM part based on the properties that need to be returned.
String * MOG_DBQueryBuilderAPI::BuildQueryString_AddPropertyToFROM(String *section, ArrayList *properties, String *joinColumn)
{		
	String *addFroms = "";
	if(properties)
	{
		int i;
		//iterate through the properties list and add each property to a FROM statement.
		for(i = 0; i < properties->Count ; i++)
		{
			MOG_Property *theProp = __try_cast<MOG_Property *>(properties->Item[i]);
			addFroms = String::Concat(addFroms, 
																		S" LEFT OUTER JOIN (SELECT * FROM ", 
																		MOG_ControllerSystem::GetDB()->GetAssetPropertiesTable(),
																		S" WHERE (Section = '",
																		MOG_DBAPI::FixSQLParameterString(section),
																		S"') AND ([Property] = '",
																		MOG_DBAPI::FixSQLParameterString(theProp->mKey),
																		S"')) AS [Property",
																		theProp->mPropertyKey,
																		S"] ON (",
																		joinColumn,
																		S" =[Property",
																		theProp->mPropertyKey,
																		S"].AssetVersionID)"
																		);	
		}
	}
			return addFroms;
}

//This function is used to add a constraint based on value to a Query
String *MOG_DBQueryBuilderAPI::AddPropertyValueConstraint(MOG_Property *property, String *value)
{
	return String::Concat(S"(",property->mPropertyKey, S" = ", value,S")");
}

//This function builds the  FROM part based on the classification properties that need to be returned.
String * MOG_DBQueryBuilderAPI::BuildQueryString_AddClassificationPropertyToFROM(String *section, ArrayList *properties, String *joinColumn)
{		
	String *addFroms = "";
	if(properties)
	{
		int i;
		//iterate through the properties list and add each property to a FROM statement.
		for(i = 0; i < properties->Count ; i++)
		{
			MOG_Property *theProp = __try_cast<MOG_Property *>(properties->Item[i]);
			addFroms = String::Concat(addFroms, 
																		S" LEFT OUTER JOIN (SELECT * FROM ", 
																		MOG_ControllerSystem::GetDB()->GetAssetClassificationsPropertiesTable(),
																		S" WHERE (Section = '",
																		MOG_DBAPI::FixSQLParameterString(section),
																		S"') AND ([Property] = '",
																		MOG_DBAPI::FixSQLParameterString(theProp->mKey),
																		S"')) AS [Property",
																		theProp->mPropertyKey,
																		S"] ON (",
																		joinColumn,
																		S" =[Property",
																		theProp->mPropertyKey,
																		S"].AssetClassificationBranchLinkID)"
																		);	
		}
	}
	return addFroms;
}

//This function takes a set of column names from a sql query and concats them to be the full file name;
String *MOG_DBQueryBuilderAPI::BuildQueryString_AddFullFileNameToSELECT(String *fullTreeNameColumn, String *platformKeyColumn, String *assetLabelColumn)
{
	String *stringToReturn = String::Concat( fullTreeNameColumn ,
											S"+'{'+", 
											platformKeyColumn,
											S"+'}'+",
											assetLabelColumn
											);
	return stringToReturn;
}

//Build a query that filters out values based on a property explicitly being set.
String *MOG_DBQueryBuilderAPI::FilteredQuery(String *selectString, ArrayList *mogPropertiesToFilterOn, bool excludeValue)
{
	//Take the query that is passed in and use it as a sub query
	String *filteredQuery = String::Concat(S"SELECT FS.* FROM(", selectString, S") AS FS ");

	//for each property in the exclude list add a WHERE clause which filters in or out based on the property's value.
	if(mogPropertiesToFilterOn != NULL && mogPropertiesToFilterOn->Count > 0)
	{
		//add the where part to the sub query for the first item.
		filteredQuery = String::Concat(filteredQuery, S" WHERE ");

		String* conditionString = "";

		//Do the same as above for all subsequent values except add the AND clause instead of the where clause.
		for (int i = 0; i < mogPropertiesToFilterOn->Count; i++)
		{
			MOG_Property* tempProperty = __try_cast <MOG_Property*> (mogPropertiesToFilterOn->Item[i]);

			String* value = tempProperty->mValue;
			String* comparison = (excludeValue) ? S"<>" : S"=";

			// Check if there was a wild card specified?
			if (value->Contains(S"*"))
			{
				// Change the comparison to be 'LIKE' and the '*' to be '%'
				comparison = S"LIKE";
				value = value->Replace(S"*", S"%");
			}

			filteredQuery = String::Concat(filteredQuery, S"((", tempProperty->mPropertyKey, S" ", comparison, S" '", MOG_DBAPI::FixSQLParameterString(value), S"') OR (", tempProperty->mPropertyKey, S" IS NULL ))");
			if (i > 0)
			{
				filteredQuery = String::Concat(filteredQuery, S" AND ");
			}
		}
	}
	return filteredQuery;
}

String *MOG_DBQueryBuilderAPI::CreateSqlDataFileLocation(String *databaseName, String *dataFilePath)
{
	String *dataFile = CreateSqlFileLocation	(databaseName, dataFilePath);
	return String::Concat(dataFile, S".mdf");
}

String *MOG_DBQueryBuilderAPI::CreateSqlLogFileLocation(String *databaseName, String *logFilePath)
{
	String *logFile = CreateSqlFileLocation	(databaseName, logFilePath);
	return  String::Concat(logFile, S".ldf");
}

String *MOG_DBQueryBuilderAPI::CreateSqlFileLocation(String *databaseName, String *dataFilePath)
{
	String *dataFile = "";
	if(dataFilePath && dataFilePath->Length > 0 
	  && databaseName && databaseName->Length > 0)
	{
		if(dataFilePath->Trim()->EndsWith(S"\\"))
		{
			dataFile = dataFilePath;
		}
		else
		{
			dataFile = String::Concat(dataFilePath, S"\\");
		}
		dataFile = String::Concat(dataFile, databaseName, S"_", MOG_Time::GetVersionTimestamp());
	}
	return dataFile;
}


///////////////////////////////////////////////////////////////////////////////////////////////////////////////
//END OF GENERAL PURPOSE QUERY BUILDER FUNCTIONS
///////////////////////////////////////////////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////
//BEGINNING OF REPORT QUERIES
//////////////////////////////////////////////////////////

//This function returns a query which can be used to generate a list of assets for a specific Sync target within a given branch.  This only returns active assets.
//rootSyncTargetPath is the syncTargetPath to return assets for.
//platformName  The specific platform to return assets for.
//propertiesToInclude  An array list of properties to include in the list.
String *MOG_DBQueryBuilderAPI::MOG_ReportQueries::GenerateReport_GetActiveAssetsBySyncTarget(String *rootSyncTargetPath, String *platformName, ArrayList *propertiesToInclude)
{
	//Buid the report query.
	String *reportQuery = "";
	reportQuery = String::Concat(S"SELECT DISTINCT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ",
																	BuildQueryString_AddPropertiesToSELECT(propertiesToInclude, true),
																	S" FROM ",
																	MOG_ControllerSystem::GetDB()->GetBranchesTable(),
																	S" AS Branches INNER JOIN (SELECT DISTINCT * FROM ",
																	MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), 
																	S")AS BranchLinks ON Branches.ID = BranchLinks.BranchID 	INNER JOIN ",
																	MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), 
																	S" AS AssetVersions ON BranchLinks.AssetVersionID = AssetVersions.ID INNER JOIN ",
																	MOG_ControllerSystem::GetDB()->GetAssetNamesTable(),
																	S" AS AssetNames ON AssetVersions.AssetNameID = AssetNames.ID INNER JOIN ",
																	MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(),
																	S" AS AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID  INNER JOIN ",
																	MOG_ControllerSystem::GetDB()->GetSyncTargetFileMapTable(),
																	S" AS SyncTargetFileMap on SyncTargetFileMap.AssetVersionID = AssetVersions.ID ",
																	BuildQueryString_AddPropertyToFROM(PROPTEXT_Properties,propertiesToInclude, S"AssetVersions.ID"),
																	S" WHERE (Branches.BranchName = '",MOG_DBAPI::FixSQLParameterString(MOG_ControllerProject::GetBranchName()),
																	S"') AND  (AssetNames.RemovedDate = '') AND (SyncTargetFileMap.SyncTargetFile LIKE '"
																	, MOG_DBAPI::FixSQLParameterString(rootSyncTargetPath), S"%')"
																	);
	//return the built report query.
	return reportQuery;
}

//This function only returns active packages.
//This function returns a query necessary for building the all Active Packages functionality.
String *MOG_DBQueryBuilderAPI::MOG_ReportQueries::GenerateReport_GetActivePackagesByClassification(String *rootClassification, ArrayList *propertiesToInclude)
{
	String *reportQuery = "";
	reportQuery = String::Concat(S"SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ",
																	BuildQueryString_AddPropertiesToSELECT(propertiesToInclude, true),
																	S" FROM ",
																	MOG_ControllerSystem::GetDB()->GetBranchesTable(),
																	S" AS Branches INNER JOIN (SELECT DISTINCT * FROM ",
																	MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), 
																	S")AS BranchLinks ON Branches.ID = BranchLinks.BranchID 	INNER JOIN ",
																	MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), 
																	S" AS AssetVersions ON BranchLinks.AssetVersionID = AssetVersions.ID INNER JOIN ",
																	MOG_ControllerSystem::GetDB()->GetAssetNamesTable(),
																	S" AS AssetNames ON AssetVersions.AssetNameID = AssetNames.ID INNER JOIN ",
																	MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(),
																	S" AS AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID ",
																	BuildQueryString_AddPropertyToFROM(PROPTEXT_Properties,propertiesToInclude, S"AssetVersions.ID"),
																	S" WHERE (Branches.BranchName = '",MOG_DBAPI::FixSQLParameterString(MOG_ControllerProject::GetBranchName()),
																	S"') AND (AssetClassifications.FullTreeName LIKE '",MOG_DBAPI::FixSQLParameterString(rootClassification),
																	S"%')  AND  (AssetNames.RemovedDate = '')"
																	);
	return reportQuery;
}

//This function builds a query wich returns a list of assets contained in a package.
String *MOG_DBQueryBuilderAPI::MOG_ReportQueries::GenerateReport_GetAssetsContainedInPackage(String *packageName, int packageVersion, ArrayList *propertiesToInclude)
{
	String *reportQuery = "";
	reportQuery = String::Concat(S"SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ",
																	BuildQueryString_AddPropertiesToSELECT(propertiesToInclude,true),
																	S" FROM ",
																	MOG_ControllerSystem::GetDB()->GetBranchesTable(),
																	S" AS Branches INNER JOIN (SELECT DISTINCT * FROM ",
																	MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), 
																	S")AS BranchLinks ON Branches.ID = BranchLinks.BranchID 	INNER JOIN ",
																	MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), 
																	S" AS AssetVersions ON BranchLinks.AssetVersionID = AssetVersions.ID INNER JOIN ",
																	MOG_ControllerSystem::GetDB()->GetAssetNamesTable(),
																	S" AS AssetNames ON AssetVersions.AssetNameID = AssetNames.ID INNER JOIN ",
																	MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(),
																	S" AS AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID INNER JOIN ",
																	MOG_ControllerSystem::GetDB()->GetPackageLinksTable(),
																	S" AS PackageLinks ON PackageLinks.PackageGroupNameID = PackageGroupNames.ID INNER JOIN ",
																	MOG_ControllerSystem::GetDB()->GetPackageGroupNamesTable(),
																	S"AS PackageGroupNames ON PackageLinks.PackageGroupNameID = PackageGroupNames.ID ",
																	BuildQueryString_AddPropertyToFROM(PROPTEXT_Properties,propertiesToInclude, S"AssetVersions.ID"),
																	S" WHERE (PackageGroupNames.PackageVersionID = ",Convert::ToString(packageVersion),
																	S") AND ( PackageGroupNames.PackageGroupName = '",MOG_DBAPI::FixSQLParameterString(packageName), S"')"
																	);
	return reportQuery;
}

//This function builds a query which gets all assets by classification.
//rootClassification is the classification the asset must contain in order to be valid for this report.
//propertiesToInclude  an array list of mog properties which are to be included in the report.
//activeOnly  based on the value of this switch you get either all assests or only the active assets.
String *MOG_DBQueryBuilderAPI::MOG_ReportQueries::GenerateReport_GetAssetsByClassification(String *rootClassification, ArrayList *propertiesToInclude, bool activeOnly)
{
	String *reportQuery = "";
	reportQuery = String::Concat(S"SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ",
																	BuildQueryString_AddPropertiesToSELECT(propertiesToInclude,true),
																	S" FROM ",
																	MOG_ControllerSystem::GetDB()->GetBranchesTable(),
																	S" AS Branches INNER JOIN (SELECT DISTINCT * FROM ",
																	MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), 
																	S")AS BranchLinks ON Branches.ID = BranchLinks.BranchID 	INNER JOIN ",
																	MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), 
																	S" AS AssetVersions ON BranchLinks.AssetVersionID = AssetVersions.ID INNER JOIN ",
																	MOG_ControllerSystem::GetDB()->GetAssetNamesTable(),
																	S" AS AssetNames ON AssetVersions.AssetNameID = AssetNames.ID INNER JOIN ",
																	MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(),
																	S" AS AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID ",
																	BuildQueryString_AddPropertyToFROM(PROPTEXT_Properties,propertiesToInclude, S"AssetVersions.ID"),
																	S" WHERE (Branches.BranchName = '",MOG_DBAPI::FixSQLParameterString(MOG_ControllerProject::GetBranchName()),
																	S"') AND (AssetClassifications.FullTreeName LIKE '",MOG_DBAPI::FixSQLParameterString(rootClassification),S"%')"
																	);
	if(activeOnly)
	{
		reportQuery = String::Concat(reportQuery, S" AND  (AssetNames.RemovedDate = '') ");
	}
	return reportQuery;
}

//This function builds a query for getting all archived assests based on the passed classification.
String *MOG_DBQueryBuilderAPI::MOG_ReportQueries::GenerateReport_GetArchiveAssetsByClassification(String *rootClassification, ArrayList *propertiesToInclude)
{
	String *reportQuery = "";
	reportQuery = String::Concat(S"SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ",
																	BuildQueryString_AddPropertiesToSELECT(propertiesToInclude, true),
																	S" FROM 	(SELECT AV.* FROM ",
																	MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), 
																	S" AS AV INNER JOIN (SELECT Max(ID) as ID, AssetNameID FROM " ,
																	MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), 
																	S" GROUP BY AssetNameID) AS MostRecentVersion	ON(AV.ID = MostRecentVersion.ID)) AS AssetVersions INNER JOIN ",
																	MOG_ControllerSystem::GetDB()->GetAssetNamesTable(),
																	S" AS AssetNames ON AssetVersions.AssetNameID = AssetNames.ID INNER JOIN ",
																	MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(),
																	S" AS AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID ",
																	BuildQueryString_AddPropertyToFROM(PROPTEXT_Properties,propertiesToInclude, S"AssetVersions.ID"),
																	S" WHERE (AssetClassifications.FullTreeName LIKE '",MOG_DBAPI::FixSQLParameterString(rootClassification),S"%')"
																	);
	return reportQuery;
}

//This function builds a query for Getting all revisions of an asset.
String *MOG_DBQueryBuilderAPI::MOG_ReportQueries::GenerateReport_GetAllRevisionsOfAsset(MOG_Filename *assetFilename, ArrayList *propertiesToInclude)
{
	String *reportQuery = "";
	reportQuery = String::Concat(S"SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ",
																	BuildQueryString_AddPropertiesToSELECT(propertiesToInclude, true),
																	S" FROM ",
																	MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), 
																	S" AS AssetVersions  INNER JOIN ",
																	MOG_ControllerSystem::GetDB()->GetAssetNamesTable(),
																	S" AS AssetNames ON AssetVersions.AssetNameID = AssetNames.ID INNER JOIN ",
																	MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(),
																	S" AS AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID ",
																	BuildQueryString_AddPropertyToFROM(PROPTEXT_Properties,propertiesToInclude, S"AssetVersions.ID"),
																	S" WHERE (AssetNames.AssetLabel ='",MOG_DBAPI::FixSQLParameterString(assetFilename->GetAssetLabel()),
																	S"') AND (AssetNames.AssetPlatformKey = '",MOG_DBAPI::FixSQLParameterString(assetFilename->GetAssetPlatform()),
																	S"') AND (AssetClassifications.FullTreeName = '",MOG_DBAPI::FixSQLParameterString(assetFilename->GetAssetClassification()),
																	S"') "
																	);
return reportQuery;
}

//This function builds a query for Getting all revisions of an asset.
String *MOG_DBQueryBuilderAPI::MOG_ReportQueries::GenerateReport_GetAllRevisionsOfAssets(String *rootClassification, ArrayList *propertiesToInclude)
{
	String *reportQuery = "";
	reportQuery = String::Concat(S"SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ",
																	BuildQueryString_AddPropertiesToSELECT(propertiesToInclude, true),
																	S" FROM ",
																	MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), 
																	S" AS AssetVersions  INNER JOIN ",
																	MOG_ControllerSystem::GetDB()->GetAssetNamesTable(),
																	S" AS AssetNames ON AssetVersions.AssetNameID = AssetNames.ID INNER JOIN ",
																	MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(),
																	S" AS AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID ",
																	BuildQueryString_AddPropertyToFROM(PROPTEXT_Properties,propertiesToInclude, S"AssetVersions.ID"),
																	S" WHERE (AssetClassifications.FullTreeName LIKE '",MOG_DBAPI::FixSQLParameterString(rootClassification),S"%')"
																	);
return reportQuery;
}

//This function is used to build a set of assets in a package
String *MOG_DBQueryBuilderAPI::MOG_ReportQueries::GenerateReport_PackageAssets(MOG_Filename *packageName, ArrayList *propertiesToInclude, String *platform)
{
	int packageAssetVersionIDToUse = MOG_DBAssetAPI::GetAssetVersionID(packageName, packageName->GetVersionTimeStamp());
	String *reportQuery = String::Concat(S"SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ",
																	BuildQueryString_AddPropertiesToSELECT(propertiesToInclude, true),
																	S" FROM ",
																	MOG_ControllerSystem::GetDB()->GetBranchesTable(),
																	S" AS Branches INNER JOIN (SELECT DISTINCT * FROM ",
																	MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), 
																	S")AS BranchLinks ON Branches.ID = BranchLinks.BranchID 	INNER JOIN ",
																	MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), 
																	S" AS AssetVersions ON BranchLinks.AssetVersionID = AssetVersions.ID INNER JOIN ",
																	MOG_ControllerSystem::GetDB()->GetAssetNamesTable(),
																	S" AS AssetNames ON AssetVersions.AssetNameID = AssetNames.ID INNER JOIN ",
																	MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(),
																	S" AS AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID INNER JOIN ",
																	MOG_ControllerSystem::GetDB()->GetPackageLinksTable(),
																	S" AS PackageLinks ON PackageLinks.AssetVersionID = AssetVersions.ID INNER JOIN ",
																	MOG_ControllerSystem::GetDB()->GetPackageGroupNamesTable(),
																	S" AS PackageGroupNames ON PackageGroupNames.ID = PackageLinks.PackageGroupNameID ",
																	BuildQueryString_AddPropertyToFROM(PROPTEXT_Properties,propertiesToInclude, S"AssetVersions.ID"),
																	S" WHERE (PackageGroupNames.PackageVersionID = ",Convert::ToString(packageAssetVersionIDToUse), S") ");
	return reportQuery;
}

//This function returns a query necessary for building the all Active Packages functionality.
String *MOG_DBQueryBuilderAPI::MOG_ReportQueries::SqlStringForAllAssetsWithPropertiesWherePackageIsTrueOrNull(String *rootClassification, ArrayList *propertiesToInclude)
{
	String *reportQuery = "";
	reportQuery = String::Concat(S"SELECT FV.* FROM (SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ",
																	BuildQueryString_AddPropertiesToSELECT(propertiesToInclude, true),
																	S" FROM ",
																	MOG_ControllerSystem::GetDB()->GetBranchesTable(),
																	S" AS Branches INNER JOIN (SELECT DISTINCT * FROM ",
																	MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), 
																	S")AS BranchLinks ON Branches.ID = BranchLinks.BranchID 	INNER JOIN ",
																	MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), 
																	S" AS AssetVersions ON BranchLinks.AssetVersionID = AssetVersions.ID INNER JOIN ",
																	MOG_ControllerSystem::GetDB()->GetAssetNamesTable(),
																	S" AS AssetNames ON AssetVersions.AssetNameID = AssetNames.ID INNER JOIN ",
																	MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(),
																	S" AS AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID ",
																	BuildQueryString_AddPropertyToFROM(PROPTEXT_Properties,propertiesToInclude, S"AssetVersions.ID"),
																	S" WHERE (Branches.BranchName = '",MOG_DBAPI::FixSQLParameterString(MOG_ControllerProject::GetBranchName()),
																	S"') AND (AssetClassifications.FullTreeName LIKE '",MOG_DBAPI::FixSQLParameterString(rootClassification),
																	S"%')  AND  (AssetNames.RemovedDate = '')) AS FV WHERE ((PACKAGE <> 'FALSE') OR PACKAGE IS NULL)"
																	);
	return reportQuery;
}
//////////////////////////////////////////////////
//END OF REPORT QUERIES
////////////////////////////////////////////////////

////////////////////////////////////////////////////////////
//BEGINING OF TREEVIEW QUERIES
////////////////////////////////////////////////////////////


String * MOG_DBQueryBuilderAPI::MOG_TreeviewQueries::QueryToGetClassificationsUnderRootClassification(String * rootNode, MOG_Property *propertyToFilterOn)
{
	//we need to create an array list and add the property to filter on to the array list so it can be passed to the BuildQueryString_AddPropertiesToSELECT and BuildQueryString_AddClassificationPropertyToFROM functions.
	ArrayList *propertiesToInclude = new ArrayList();
	propertiesToInclude->Add(propertyToFilterOn);

	//Build the appropriate select string 
	String *selectString = String::Concat(S"SELECT FullTreeName ", 
																			BuildQueryString_AddPropertiesToSELECT(propertiesToInclude, true),
																			S" FROM ",
																			MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), 
																			S" AS AssetClassifications INNER JOIN ", 
																			MOG_ControllerSystem::GetDB()->GetAssetClassificationsBranchLinksTable(), 
																			S" AS BranchLinks ON (AssetClassifications.ID = BranchLinks.ClassificationID) ",
																			BuildQueryString_AddClassificationPropertyToFROM(propertyToFilterOn->mSection, propertiesToInclude, 
																			S"BranchLinks.ID"), S" WHERE FullTreeName LIKE'",MOG_DBAPI::FixSQLParameterString(rootNode),S"%'");

	return  FilteredQuery(selectString, propertiesToInclude, true);
}

/////////////////////////////////////////////////////////
//End of TREEVIEW Queries
//////////////////////////////////////////////////////////


////////////////////////////////////////////////////////////////////////////////////////
//END of ASSET MANAGEMENT QUERIES
//////////////////////////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////////////////////////
//START OF CACHE BUILDER QUERIES
/////////////////////////////////////////////////////////////////////////////////////
String *MOG_DBQueryBuilderAPI::MOG_DBCacheQueries::PopulateCache_AssetNameCache()
{
		return String::Concat(	S"SELECT AssetNames.ID AS CACHEID, ", BuildQueryString_AddFullFileNameToSELECT(S"AssetClassifications.FullTreeName",S"AssetNames.AssetPlatformKey", S"AssetNames.AssetLabel" ) ,S" AS CACHENAME ",
								S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AS AssetNames INNER JOIN ",
								MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AS AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID ");
}

String *MOG_DBQueryBuilderAPI::MOG_DBCacheQueries::PopulateCache_AssetVersionCache()
{
	return  String::Concat(	S"SELECT AssetVersions.ID AS CACHEID,  ", BuildQueryString_AddFullFileNameToSELECT(S"AssetClassifications.FullTreeName",S"AssetNames.AssetPlatformKey", S"AssetNames.AssetLabel"  ) ,S" AS CACHELIST, AssetVersions.Version AS  CACHENAME ",
										S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AS AssetNames INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AS AssetVersions ON AssetNames.ID = AssetVersions.AssetNameID INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AS AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID ");
}

String *MOG_DBQueryBuilderAPI::MOG_DBCacheQueries::PopulateTemporaryCache_PreviousVersion()
{
	return String::Concat( S"SELECT BranchLinks.AssetVersionID AS CACHEID, BranchLinks.BranchID AS CACHELIST, AVT.AssetNameID AS CACHENAME ",
							S"FROM (SELECT DISTINCT * FROM ", MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), S") AS BranchLinks  INNER JOIN ", MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AS AVT ON ",
							S"BranchLinks.AssetVersionID = AVT.ID ");
}

String *MOG_DBQueryBuilderAPI::MOG_DBCacheQueries::PopulateCache_ClassificationCache()
{
	return  String::Concat(	S"SELECT AssetClassifications.ID AS CACHEID, AssetClassifications.FullTreeName AS CACHENAME ",
										S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AS AssetClassifications ");
}


//////////////////////////////////////////////////////////////////////////////////////
//END OF CACHE BUILDER QUERIES
/////////////////////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////////////////////////
//START OF SYNC TARGET Queries
/////////////////////////////////////////////////////////////////////////////////////

String *MOG_DBQueryBuilderAPI::MOG_SyncDataQueries::Query_AllCurrentAssetsInMachineUserWorkspace(String *computerName, String *projectName, String *platformName, String *workingDirectory, String *userName, int syncedLocationID)
{
	return String::Concat(S"SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ",
						S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames INNER JOIN ",
						MOG_ControllerSystem::GetDB()->GetSyncedDataLinksTable(), S" SyncedDataLinks INNER JOIN ",
						MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions ON SyncedDataLinks.AssetVersionID = AssetVersions.ID ON AssetNames.ID = AssetVersions.AssetNameID INNER JOIN ",
						MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID ",
						S"WHERE (SyncedDataLinks.SyncedDataLocationID = ", Convert::ToString(syncedLocationID), S") AND ",
						S"(AssetClassifications.FullTreeName NOT LIKE '", MOG_DBAPI::FixSQLParameterString(MOG_ControllerProject::GetProjectName()), S"~Library%')" );
}

String *MOG_DBQueryBuilderAPI::MOG_SyncDataQueries::Query_AllCurrentAssetsInMachineUserWorkspace_ByClassification(String *computerName, String *projectName, String *platformName, String *workingDirectory, String *userName, String *classification, int syncedLocationID)
{
	return String::Concat(	S"SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ",
							S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames INNER JOIN ",
							MOG_ControllerSystem::GetDB()->GetSyncedDataLinksTable(), S" SyncedDataLinks INNER JOIN ",
							MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions ON SyncedDataLinks.AssetVersionID = AssetVersions.ID ON AssetNames.ID = AssetVersions.AssetNameID INNER JOIN ",
							MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID ",
							S"WHERE (SyncedDataLinks.SyncedDataLocationID = ", Convert::ToString(syncedLocationID), S") AND ",
							S"(AssetClassifications.FullTreeName LIKE '", MOG_DBAPI::FixSQLParameterString(classification), S"%') AND ",
							S"(AssetClassifications.FullTreeName NOT LIKE '", MOG_DBAPI::FixSQLParameterString(MOG_ControllerProject::GetProjectName()), S"~Library%')" );
}

String *MOG_DBQueryBuilderAPI::MOG_SyncDataQueries::Query_AllCurrentAssetsInMachineUserWorkspace_BySyncLocation(String *computerName, String *projectName, String *platformName, String *workingDirectory, String *userName, String *syncLocation, int syncedLocationID)
{
	return String::Concat(	S"SELECT DISTINCT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ",
							S"FROM ", MOG_ControllerSystem::GetDB()->GetSyncedDataLinksTable(), S" SDL INNER JOIN ",
							MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames INNER JOIN ",
							MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions ON AssetNames.ID = AssetVersions.AssetNameID INNER JOIN ",
							MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID ON ",
							S"SDL.AssetVersionID = AssetVersions.ID INNER JOIN ",
							MOG_ControllerSystem::GetDB()->GetPlatformsTable(), S" Platforms INNER JOIN ",
							MOG_ControllerSystem::GetDB()->GetSyncTargetFileMapTable(), S"SyncTargetFileMap ON Platforms.ID = SyncTargetFileMap.PlatformID ON ",
							S"SDL.AssetVersionID = SyncTargetFileMap.AssetVersionID ",
							S"WHERE (SyncTargetFileMap.SyncTargetFile LIKE '", MOG_DBAPI::FixSQLParameterString(syncLocation), S"%') AND ",
							S"(Platforms.PlatformName = '", MOG_DBAPI::FixSQLParameterString(platformName), S"') AND ",
							S"(SDL.SyncedDataLocationID = ", Convert::ToString(syncedLocationID), S") AND ",
							S"(AssetClassifications.FullTreeName NOT LIKE '", MOG_DBAPI::FixSQLParameterString(MOG_ControllerProject::GetProjectName()), S"~Library%')" );
}

////////////////////////////////////////////////////////////////////////////////////////////
//End of Sync Target Queries
///////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////
//Beginning of the Database Managment Functions
/////////////////////////////////////////////////////////////////////////////////////////
String *MOG_DBQueryBuilderAPI::MOG_DatabaseManagementQueries::Query_CreateMogDatabase(String * newDatabaseName, String *dataFilePath, String *logFilePath)
{
	return String::Concat( S"CREATE DATABASE ", newDatabaseName ,S" ON PRIMARY ( NAME = ", newDatabaseName,
	S"_MDF, FILENAME = '", CreateSqlDataFileLocation (newDatabaseName, dataFilePath) ,S"', SIZE = 50MB, MAXSIZE = UNLIMITED, FILEGROWTH = 50MB  )",
	S" LOG ON ( NAME = ", newDatabaseName, S"_LDF, FILENAME = '", CreateSqlLogFileLocation(newDatabaseName, dataFilePath) ,S"', SIZE = 10MB, MAXSIZE = UNLIMITED, FILEGROWTH = 10MB )");
}
String *MOG_DBQueryBuilderAPI::MOG_DatabaseManagementQueries::Query_GetDefaultDataFilePath(String * databaseName)
{
	return String::Concat(S"SELECT physical_name FROM ", databaseName, S".sys.database_files WHERE type_desc = 'ROWS'");
}
String *MOG_DBQueryBuilderAPI::MOG_DatabaseManagementQueries::Query_GetDefaultLogFilePath(String * databaseName)
{
	return String::Concat(S"SELECT physical_name FROM ", databaseName, S".sys.database_files WHERE type_desc = 'LOG'");
}


///////////////////////////////////////////////////////////////////////////////////////////
//End of the Database Managment Functions
////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////
//Beginning of Table Management Queries
////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////
//Beginning of SystemTable Management Functions
//////////////////////////////////////////////////////////////////////////////////////////////////
String * MOG_DBQueryBuilderAPI::MOG_TableManagmentQueries::MOG_SystemTablesQueries::Query_CreateCommandsTable(String *tableName)
{
	return String::Concat(S"CREATE TABLE dbo.[",tableName  ,"] ( \
							ID							int IDENTITY (1,1) NOT NULL, \
							CommandID					int NOT NULL, \
							CommandType				int NOT NULL, \
							CommandTimeStamp			varChar(15) NOT NULL, \
							Blocking					bit NOT NULL, \
							Completed					bit NOT NULL, \
							RemoveDuplicateCommands	bit NOT NULL, \
							PersistantLock				bit NOT NULL, \
							PreserveCommand			bit NOT NULL, \
							NetworkID					int NOT NULL, \
							ComputerIP					varChar(15) NOT NULL, \
							ComputerName				varChar(50) NOT NULL, \
							ProjectName				varChar(50) NOT NULL, \
							Branch						varChar(50) NOT NULL, \
							Label						varChar(50) NOT NULL, \
							Tab						varChar(50) NOT NULL, \
							UserName					varChar(50) NOT NULL, \
							Platform					varChar(50) NOT NULL, \
							ValidSlaves				varChar(50) NOT NULL, \
							WorkingDirectory			varChar(260) NOT NULL, \
							Source						varChar(260) NOT NULL, \
							Destination				varChar(260) NOT NULL, \
							Description				text NOT NULL, \
							Version					varChar(15) NOT NULL, \
							Options					varChar(1024) NOT NULL, \
							AssetFilename				varChar(1024) NOT NULL );");
}
String * MOG_DBQueryBuilderAPI::MOG_TableManagmentQueries::MOG_SystemTablesQueries::Query_CreateDBVersionTable(String *tableName)
{
	return "";
}
String * MOG_DBQueryBuilderAPI::MOG_TableManagmentQueries::MOG_SystemTablesQueries::Query_CreateEventsTable(String *tableName)
{
	return String::Concat("CREATE TABLE dbo.[",tableName ,S"] ( \
					ID				int IDENTITY (1,1) NOT NULL, \
					Type			varChar(50) NOT NULL, \
					TimeStamp		varChar(15) NOT NULL, \
					Title			varChar(100) NOT NULL, \
					StackTrace		varChar(1024) NOT NULL, \
					Description	varChar(1024) NOT NULL, \
					EventID		varChar(50) NOT NULL, \
					UserName		varChar(50) NOT NULL, \
					ComputerName	varChar(50) NOT NULL, \
					ProjectName	varChar(50) NOT NULL, \
					BranchName		varChar(50) NOT NULL, \
					RepeatCount	int DEFAULT 1 NOT NULL );");
}
/////////////////////////////////////////////////////////////////////////////////////////////////
//End of SystemTable Management Functions
/////////////////////////////////////////////////////////////////////////////////////////////////

String *MOG_DBQueryBuilderAPI::MOG_TableManagmentQueries::Query_CopyTable(String *sourceTable, String *destinationTable)
{
	return String::Concat(	S"SELECT * FROM ", sourceTable, S" INTO ", destinationTable  );
}
////////////////////////////////////////////////////////////////////////////////////////////
//End of Table Management Queries
////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////
//beginning of Asset Queries
///////////////////////////////////////////////////////////////////////////////////////////////

//This is the query to retireve a list of all the assets that need to be reversioned when the SyncTarget changes.
String *MOG_DBQueryBuilderAPI::MOG_AssetQueries::Query_AssetsWithPropertyValue(String *rootClassification, MOG_Property *propertyToFilterOn)
{
	//we need to create an array list and add the property to filter on to the array list so it can be passed to the BuildQueryString_AddPropertiesToSELECT and BuildQueryString_AddClassificationPropertyToFROM functions.
	ArrayList *propertiesToInclude = new ArrayList();
	propertiesToInclude->Add(propertyToFilterOn);

	//Build the appropriate select string 
	String *selectString = MOG_DBQueryBuilderAPI::MOG_ReportQueries::GenerateReport_GetAssetsByClassification(rootClassification, propertiesToInclude, true);

	return  FilteredQuery(selectString, propertiesToInclude,false);
}

String *MOG_DBQueryBuilderAPI::MOG_AssetQueries::Query_AssetsWithProperties_Filtered(String *selectString, ArrayList *mogPropertiesToFilterOn, bool excludeValue)
{
	return FilteredQuery(selectString, mogPropertiesToFilterOn, excludeValue);
}

String *MOG_DBQueryBuilderAPI::MOG_AssetQueries::Query_AllAssetsAllRevisionsForProject(String *projectName)
{
		return String::Concat(	S"SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ",
										S" FROM [", projectName, S".AssetVersions] AS AssetVersions  INNER JOIN [",
											projectName, S".AssetNames] AS AssetNames ON AssetVersions.AssetNameID = AssetNames.ID INNER JOIN [",
											projectName, S".AssetClassifications] AS AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID ");
}

String *MOG_DBQueryBuilderAPI::MOG_AssetQueries::Query_AllCurrentAssetsForBranch()
{
	return String::Concat(	S"SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ",
							S"FROM (SELECT DISTINCT * FROM ", MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), S") AS BranchLinks INNER JOIN ",
							MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions ON BranchLinks.AssetVersionID = AssetVersions.ID INNER JOIN ",
							MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches ON BranchLinks.BranchID = Branches.ID INNER JOIN ",
							MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames ON AssetVersions.AssetNameID = AssetNames.ID INNER JOIN ",
							MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID ",
							S"WHERE (Branches.BranchName = '", MOG_DBAPI::FixSQLParameterString(MOG_ControllerProject::GetBranchName()), S"') AND " 
							S"(AssetNames.RemovedDate = '')" );
}

String *MOG_DBQueryBuilderAPI::MOG_AssetQueries::Query_AllAssetsForBranch()
{
	return Query_AllAssetsForBranch(false);
}

String *MOG_DBQueryBuilderAPI::MOG_AssetQueries::Query_AllAssetsForBranch(bool bExcludeLibrary)
{
	return Query_AllAssetsForBranch(bExcludeLibrary, MOG_ControllerProject::GetBranchName());
}

String *MOG_DBQueryBuilderAPI::MOG_AssetQueries::Query_AllAssetsForBranch(bool bExcludeLibrary, String *branchName)
{
	String *excludeLibraryString = String::Concat(S" AND (AssetClassifications.FullTreeName NOT LIKE '", MOG_DBAPI::FixSQLParameterString(MOG_ControllerProject::GetProjectName()), S"~Library%') ");

	return String::Concat(	S"SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ",
							S"FROM (SELECT DISTINCT * FROM ", MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), S") AS BranchLinks INNER JOIN ",
							MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions ON BranchLinks.AssetVersionID = AssetVersions.ID INNER JOIN ",
							MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches ON BranchLinks.BranchID = Branches.ID INNER JOIN ",
							MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames ON AssetVersions.AssetNameID = AssetNames.ID INNER JOIN ",
							MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID ",
							S"WHERE (Branches.BranchName = '", MOG_DBAPI::FixSQLParameterString(branchName), S"') ",
						   (bExcludeLibrary) ? excludeLibraryString : "");
}

String *MOG_DBQueryBuilderAPI::MOG_AssetQueries::Query_AllAssetsForBranchByClassification(String *classification)
{
	return  String::Concat( S"SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ",
						   S"FROM (SELECT DISTINCT * FROM ", MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), S") AS BranchLinks INNER JOIN ",
						   MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions ON BranchLinks.AssetVersionID = AssetVersions.ID INNER JOIN ",
						   MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches ON BranchLinks.BranchID = Branches.ID INNER JOIN ",
						   MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames ON AssetVersions.AssetNameID = AssetNames.ID INNER JOIN ",
						   MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID ",
						   S"WHERE (Branches.BranchName = '", MOG_DBAPI::FixSQLParameterString(MOG_ControllerProject::GetBranchName()), S"') AND ",
						   S"(AssetClassifications.FullTreeName = '", MOG_DBAPI::FixSQLParameterString(classification), S"') ORDER BY AssetNames.AssetLabel" );
}

String *MOG_DBQueryBuilderAPI::MOG_AssetQueries::Query_AllCurrentAssetsForBranchByClassificationTree(String *classification)
{
	return Query_AllCurrentAssetsForBranchByClassificationTree(classification, false);
}

String *MOG_DBQueryBuilderAPI::MOG_AssetQueries::Query_AllCurrentAssetsForBranchByClassificationTree(String *classification, bool bExcludeLibrary)
{
	return Query_AllCurrentAssetsForBranchByClassificationTree(classification, bExcludeLibrary, MOG_ControllerProject::GetBranchName());
}

String *MOG_DBQueryBuilderAPI::MOG_AssetQueries::Query_AllCurrentAssetsForBranchByClassificationTree(String *classification, bool bExcludeLibrary, String *branchName)
{
	String *excludeLibraryString = String::Concat(S"(AssetClassifications.FullTreeName NOT LIKE '", MOG_DBAPI::FixSQLParameterString(MOG_ControllerProject::GetProjectName()), S"~Library%') AND ");

	return  String::Concat( S"SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ",
						   S"FROM (SELECT DISTINCT * FROM ", MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), S") AS BranchLinks INNER JOIN ",
						   MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions ON BranchLinks.AssetVersionID = AssetVersions.ID INNER JOIN ",
						   MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches ON BranchLinks.BranchID = Branches.ID INNER JOIN ",
						   MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames ON AssetVersions.AssetNameID = AssetNames.ID INNER JOIN ",
						   MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID ",
						   S"WHERE (Branches.BranchName = '", MOG_DBAPI::FixSQLParameterString(branchName), S"') AND ",
						   (bExcludeLibrary) ? excludeLibraryString : "",
						   S"(AssetClassifications.FullTreeName LIKE '", MOG_DBAPI::FixSQLParameterString(classification), S"%') AND (AssetNames.RemovedDate = '') ORDER BY AssetNames.AssetLabel" );
}
String *MOG_DBQueryBuilderAPI::MOG_AssetQueries::Query_AllAssetsForBranchBySyncLocation(String *syncLocation, String *platform)
{
	String * selectString = String::Concat(S"SELECT DISTINCT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ",
										S"FROM ", MOG_ControllerSystem::GetDB()->GetPlatformsTable(), S" Platforms INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetSyncTargetFileMapTable(), S" SyncTargetFileMap ON Platforms.ID = SyncTargetFileMap.PlatformID INNER JOIN (SELECT DISTINCT * FROM ",
											MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), S") AS BranchLinks INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches ON BranchLinks.BranchID = Branches.ID INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions ON AssetNames.ID = AssetVersions.AssetNameID INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID ON "
											S"BranchLinks.AssetVersionID = AssetVersions.ID ON SyncTargetFileMap.AssetVersionID = BranchLinks.AssetVersionID ",
											S"WHERE (SyncTargetFileMap.SyncTargetFile LIKE '", MOG_DBAPI::FixSQLParameterString(syncLocation), S"%') AND ",
											S"(Branches.BranchName = '", MOG_DBAPI::FixSQLParameterString(MOG_ControllerProject::GetBranchName()), S"')" );
		// Check if they specified a platform?
		if (platform && platform->Length)
		{
			selectString = String::Concat(selectString, S" AND (Platforms.PlatformName = '", MOG_DBAPI::FixSQLParameterString(platform), S"')" );
		}

	return selectString;
}

String *MOG_DBQueryBuilderAPI::MOG_AssetQueries::Query_AllCurrentAssetsForBranchBySyncLocation(String *syncLocation, String *platform)
{
	return Query_AllCurrentAssetsForBranchBySyncLocation(syncLocation, platform, false);
}

String *MOG_DBQueryBuilderAPI::MOG_AssetQueries::Query_AllCurrentAssetsForBranchBySyncLocation(String *syncLocation, String *platform, bool bExcludeLibrary)
{
	return Query_AllCurrentAssetsForBranchBySyncLocation(syncLocation, platform, false, MOG_ControllerProject::GetBranchName());
}

String *MOG_DBQueryBuilderAPI::MOG_AssetQueries::Query_AllCurrentAssetsForBranchBySyncLocation(String *syncLocation, String *platform, bool bExcludeLibrary, String *branchName)
{
	String *excludeLibraryString = String::Concat(S"(AssetClassifications.FullTreeName NOT LIKE '", MOG_DBAPI::FixSQLParameterString(MOG_ControllerProject::GetProjectName()), S"~Library%') AND ");

	String * selectString = String::Concat(S"SELECT DISTINCT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ",
										S"FROM ", MOG_ControllerSystem::GetDB()->GetPlatformsTable(), S" Platforms INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetSyncTargetFileMapTable(), S" SyncTargetFileMap ON Platforms.ID = SyncTargetFileMap.PlatformID INNER JOIN (SELECT DISTINCT * FROM ",
											MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), S") AS BranchLinks INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches ON BranchLinks.BranchID = Branches.ID INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions ON AssetNames.ID = AssetVersions.AssetNameID INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID ON "
											S"BranchLinks.AssetVersionID = AssetVersions.ID ON SyncTargetFileMap.AssetVersionID = BranchLinks.AssetVersionID ",
											S"WHERE (SyncTargetFileMap.SyncTargetFile LIKE '", MOG_DBAPI::FixSQLParameterString(syncLocation), S"%') AND ",
										   (bExcludeLibrary) ? excludeLibraryString : "",
											S"(Branches.BranchName = '", MOG_DBAPI::FixSQLParameterString(branchName), S"') AND"
											S"(AssetNames.RemovedDate = '')");
		// Check if they specified a platform?
		if (platform && platform->Length)
		{
			selectString = String::Concat(selectString, S" AND (Platforms.PlatformName = '", MOG_DBAPI::FixSQLParameterString(platform), S"')" );
		}

	return selectString;
}
String *MOG_DBQueryBuilderAPI::MOG_AssetQueries::Query_AllCurrentAssetsWithPropertySet(MOG_Property * requiredProperty)
{
	String * selectString = String::Concat(	S"SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ",
							S"FROM (SELECT DISTINCT * FROM ", MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), S") AS BranchLinks INNER JOIN ",
							MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions ON BranchLinks.AssetVersionID = AssetVersions.ID INNER JOIN ",
							MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches ON BranchLinks.BranchID = Branches.ID INNER JOIN ",
							MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames ON AssetVersions.AssetNameID = AssetNames.ID INNER JOIN ",
							MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID INNER JOIN ",
							MOG_ControllerSystem::GetDB()->GetAssetPropertiesTable(), S" AS AssetProperties ON AssetProperties.AssetVersionID = AssetVersions.ID ",
							S"WHERE (Branches.BranchName = '", MOG_DBAPI::FixSQLParameterString(MOG_ControllerProject::GetBranchName()), S"')" );
	
	//if we have a section add the section to the property filter
	if(requiredProperty->mSection->Length > 0)
	{
		selectString = String::Concat(selectString, S" AND (AssetProperties.Section = '", MOG_DBAPI::FixSQLParameterString(requiredProperty->mSection), S"')");
	}
	if(requiredProperty->mPropertySection->Length > 0)
	{
		selectString = String::Concat(selectString, S" AND (AssetProperties.Property LIKE '%", MOG_DBAPI::FixSQLParameterString(requiredProperty->mPropertySection), S"%')");
	}
	if(requiredProperty->mPropertyKey->Length > 0)
	{
		selectString = String::Concat(selectString, S" AND (AssetProperties.Property LIKE '%", MOG_DBAPI::FixSQLParameterString(requiredProperty->mPropertyKey), S"%')");
	}
	if(requiredProperty->mValue->Length > 0)
	{
		selectString = String::Concat(selectString, S" AND (AssetProperties.Value LIKE '%", MOG_DBAPI::FixSQLParameterString(requiredProperty->mValue), S"%')");
	}

	return selectString;
}
//////////////////////////////////////////////////////////////////////////////////////////
//End of Asset Queries
/////////////////////////////////////////////////////////////////////////////////////////