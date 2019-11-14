//--------------------------------------------------------------------------------
//	MOG_Database.h
//	
//	
//--------------------------------------------------------------------------------
#ifndef __MOG_DATABASE_H__
#define __MOG_DATABASE_H__

#using <mscorlib.dll>
#using <system.dll>
#using <System.Data.dll>

#include "stdlib.h"
#include "MOG_Define.h"
#include "MOG_DBCache.h"


using namespace System;
using namespace System::Data;
using namespace System::Data::SqlClient;
using namespace System::Collections;
using namespace System::ComponentModel;


namespace MOG {
namespace DATABASE {

#define MOG_DB_ComputerNameSize		50
#define MOG_DB_FilenamePathSize		265

public __gc class MOG_Database
{
private:
	// Connection Information
	bool mIsConnected;
	float mConnectedTime;
	float mLastConnectionAttemptTime;
	String *mConnectionString;
	//This is where we store all Database Cache
	MOG_DBCache *mDBCache;

	// Project specific table names
	String *mAssetClassificationsTable;
	String *mAssetClassificationsBranchLinksTable;
	String *mAssetClassificationsPropertiesTable;
	String *mAssetNamesTable;
	String *mAssetPropertiesTable;
	String *mAssetVersionsTable;
	String *mBranchesTable;
	String *mBranchLinksTable;
	String *mDepartmentsTable;
	String *mSyncTargetFileMapTable;
	String *mInboxAssetsTable;
	String *mInboxAssetsPropertiesTable;
	String *mPackageCommandsTable;
	String *mPackageGroupNamesTable;
	String *mPackageLinksTable;
	String *mPlatformsTable;
	String *mPlatformsBranchLinksTable;
	String *mPostCommandsTable;
	String *mSourceFileMapTable;
	String *mSyncedDataLocationsTable;
	String *mSyncedDataLinksTable;
	String *mTaskAssetLinksTable;
	String *mTaskLinksTable;
	String *mTasksTable;
	String *mUsersTable;

	// Database Functions
	String *GetDefaultConnectionString();
	bool SetDefaultConnectionString(String *connectionString);

public:
	MOG_Database(void);

    //----------->SET DATABASE VERSION HERE AND ONLY HERE<---------------------
	static const int sCurrentDatabaseVersion = 9;
    
	bool Initialize(String *connectionString);
	bool Connect(String *projectName, String *branchName);

	// Access Functions
	String *GetConnectionString()			{	return mConnectionString;	};

	MOG_DBCache *GetDBCache()				{   return mDBCache;   };

	String *GetAssetClassificationsTable()				{	return mAssetClassificationsTable;	};
	String *GetAssetClassificationsBranchLinksTable()	{	return mAssetClassificationsBranchLinksTable;	};
	String *GetAssetClassificationsPropertiesTable()	{	return mAssetClassificationsPropertiesTable;	};
	String *GetAssetNamesTable()						{	return mAssetNamesTable;	};
	String *GetAssetPropertiesTable()					{	return mAssetPropertiesTable;	};
	String *GetAssetVersionsTable()						{	return mAssetVersionsTable;	};
	String *GetBranchesTable()							{	return mBranchesTable;	};
	String *GetBranchLinksTable()						{	return mBranchLinksTable;	};
	String *GetDepartmentsTable()						{	return mDepartmentsTable;	};
	String *GetSyncTargetFileMapTable()					{	return mSyncTargetFileMapTable;	};
	String *GetInboxAssetsTable()						{	return mInboxAssetsTable;	};
	String *GetInboxAssetsPropertiesTable()				{	return mInboxAssetsPropertiesTable;	};
	String *GetPackageCommandsTable()					{	return mPackageCommandsTable;	};
	String *GetPackageGroupNamesTable()					{	return mPackageGroupNamesTable;	};
	String *GetPackageLinksTable()						{	return mPackageLinksTable;	};
	String *GetPlatformsTable()							{	return mPlatformsTable;	};
	String *GetPlatformsBranchLinksTable()				{	return mPlatformsBranchLinksTable;	};
	String *GetPostCommandsTable()						{	return mPostCommandsTable;	};
	String *GetSourceFileMapTable()						{	return mSourceFileMapTable;	};
	String *GetSyncedDataLocationsTable()				{	return mSyncedDataLocationsTable;	};
	String *GetSyncedDataLinksTable()					{	return mSyncedDataLinksTable;	};
	String *GetTaskAssetLinksTable()					{	return mTaskAssetLinksTable;	};
	String *GetTaskLinksTable()							{	return mTaskLinksTable;	};
	String *GetTasksTable()								{	return mTasksTable;	};
	String *GetUsersTable()								{	return mUsersTable;	};

	
	static bool VerifyDatabase(String *connectionString, String *databaseName);
	int GetDBVersion();
	bool UpdateDBVersion();
	bool VerifySystemTables(int databaseVersion);
	bool VerifyTables(String *projectName);
	bool VerifyTables(String *projectName, int databaseVersion);
	bool CreateClusteredIndex(String *projectName, String *tableName, String *columnName);
	bool CreateClusteredIndex(String *projectName, String *tableName, String *columnName, bool bUnique);
	bool CreateNonClusteredIndex(String *projectName, String *tableName, String *columnName);
	bool CreateProjectIndexes(String *projectName);
	bool DeleteMOGTables();
	bool DeleteProjectTables(String *projectName);
	bool ProjectTablesExist(String *projectName);
	ArrayList *GetAllTableNames(void);
	bool AlterAllDateColumns();
	bool UpdateTables(String *projectName, int databaseVersion, bool supressMessage);
	bool UpdateSystemTables(int databaseVersion, bool supressMessage);
	bool RenameTable(String *currentTableName, String *newTableName);
	bool RenameColumn(String *tableName, String *currentColumnName, String *newColumnName);
	bool CopyTable(String *sourceTable, String *destinationTable);
	bool UpdatePropertiesTableProperties(String *tableName);
	bool UpdatePropertySection(String *tableName, String *oldProperty, String *newProperty);
	bool RemovePropertySection(String *tableName, String *propertyToRemove);
	bool SingleDatabasePort(String *oldDatabaseConnectionString, String *oldProjectName, String *newDatabaseConnectionString);

	static bool ParseConnectionString(String *dbConnectionString, String **outMachineName, String **outCatalog, String **outSecurity, String **outUserID, String **outUserPassword);

	static bool ExportProjectTables(String *projectName, String *projectLocation);
	static bool ImportProjectTables(String *projectName, String *projectLocation);

	static bool MySqlBulkCopy(DataTable *dataTable, String *tableName);

private:
	static bool ExportProjectTable(String *outputDirectory, String *tableName);
	static bool ImportProjectTable(String *inputDirectory, String *tableName, String *fileName);

	void UpdateAssetProperties_Worker(Object* sender, DoWorkEventArgs* e);

	void Test(void);
};
}
}

using namespace MOG;
using namespace MOG::DATABASE;

#endif

