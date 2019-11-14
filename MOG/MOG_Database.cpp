//--------------------------------------------------------------------------------
//	MOG_Database.cpp
//	
//	
//--------------------------------------------------------------------------------
#include "stdafx.h"

#include "MOG_Main.h"
#include "MOG_Report.h"
#include "MOG_Time.h"
#include "MOG_DBInboxAPI.h"
#include "MOG_DosUtils.h"
#include "MOG_Ini.h"

#include "MOG_Database.h"
#include "MOG_ControllerSystem.h"
#include "MOG_ControllerRepository.h"
#include "MOG_DBAssetAPI.h"
#include "MOG_SystemUtilities.h"
#include "MOG_DBQueryBuilderAPI.h"

using namespace MOG_CoreControls;
using namespace System::Collections::Generic;

MOG_Database::MOG_Database()
{
	// Get the default connection string
	mConnectionString = S"";
	mDBCache = new MOG_DBCache();
}


String *MOG_Database::GetDefaultConnectionString()
{
	String *connectionString = "";

	String *filename = MOG_Main::FindLocalConfigFile();
	if (filename && filename->Length)
	{
		MOG_Ini *configFile = new MOG_Ini(filename);
		if (configFile)
		{
			if (configFile->KeyExist("SQL", "ConnectionString"))
			{
				connectionString = configFile->GetString("SQL", "ConnectionString");
			}

			configFile->CloseNoSave();
		}
	}

	return connectionString;
}


bool MOG_Database::Initialize(String *connectionString)
{
	// Check if we need to supply the connection string?
	if (!connectionString || !connectionString->Length)
	{
		// Get the connection string
		connectionString = GetConnectionString();
		// Check if we are still missing a ConnectionString?
		if (!connectionString || !connectionString->Length)
		{
			// Try the default connection string?
			connectionString = GetDefaultConnectionString();
			// Check if we are still missing a ConnectionString?
			if (!connectionString || !connectionString->Length)
			{
				return false;
			}
		}
	}

	// Establish this as our new connection string
	SetDefaultConnectionString(connectionString);

	// Attempt to open a connection to make sure we will work?
	SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(connectionString);
	mIsConnected = true;

	return mIsConnected;
}


bool MOG_Database::Connect(String *projectName, String *branchName)
{
	// Make sure we have a valid project and branch specified?
	if (projectName->Length &&
		branchName->Length)
	{
		// Create all the project specific table names
		mAssetClassificationsTable				= String::Concat(S"[", projectName, S".AssetClassifications]");
		mAssetClassificationsBranchLinksTable	= String::Concat(S"[", projectName, S".AssetClassificationsBranchLinks]");
		mAssetClassificationsPropertiesTable	= String::Concat(S"[", projectName, S".AssetClassificationsProperties]");
		mAssetNamesTable						= String::Concat(S"[", projectName, S".AssetNames]");
		mAssetPropertiesTable					= String::Concat(S"[", projectName, S".AssetProperties]");
		mAssetVersionsTable						= String::Concat(S"[", projectName, S".AssetVersions]");
		mBranchesTable							= String::Concat(S"[", projectName, S".Branches]");
		mBranchLinksTable						= String::Concat(S"[", projectName, S".BranchLinks]");
		mDepartmentsTable						= String::Concat(S"[", projectName, S".Departments]");
		mSyncTargetFileMapTable					= String::Concat(S"[", projectName, S".SyncTargetFileMap]");
		mInboxAssetsTable						= String::Concat(S"[", projectName, S".InboxAssets]");
		mInboxAssetsPropertiesTable				= String::Concat(S"[", projectName, S".InboxAssetsProperties]");
		mPackageCommandsTable					= String::Concat(S"[", projectName, S".PackageCommands]");
		mPackageGroupNamesTable					= String::Concat(S"[", projectName, S".PackageGroupNames]");
		mPackageLinksTable						= String::Concat(S"[", projectName, S".PackageLinks]");
		mPlatformsTable							= String::Concat(S"[", projectName, S".Platforms]");
		mPlatformsBranchLinksTable				= String::Concat(S"[", projectName, S".PlatformsBranchLinks]");
		mPostCommandsTable						= String::Concat(S"[", projectName, S".PostCommands]");
		mSourceFileMapTable						= String::Concat(S"[", projectName, S".SourceFileMap]");
		mSyncedDataLocationsTable				= String::Concat(S"[", projectName, S".SyncedDataLocations]");
		mSyncedDataLinksTable					= String::Concat(S"[", projectName, S".SyncedDataLinks]");
		mTaskAssetLinksTable					= String::Concat(S"[", projectName, S".TaskAssetLinks]");
		mTaskLinksTable							= String::Concat(S"[", projectName, S".TaskLinks]");
		mTasksTable								= String::Concat(S"[", projectName, S".Tasks]");
		mUsersTable								= String::Concat(S"[", projectName, S".Users]");

		// Flush all caches
		mDBCache->FlushDBCache();
		return true;
	}

	return false;
}

bool MOG_Database::VerifyDatabase(String *connectionString, String *databaseName)
{
	bool bExists = false;
	SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(connectionString);
	try
	{
		String *command = String::Concat("SELECT name FROM master.dbo.sysdatabases WHERE (name = '", databaseName,"')");
		SqlDataReader *myReader = MOG_DBAPI::GetNewDataReader(command,myConnection);
		if( myReader->Read() )
		{
			bExists = true;
		}

		myReader->Close();
		
		if( !bExists )
		{
			
			String *createDatabase = String::Concat(S"CREATE DATABASE ", databaseName);
			MOG_DBAPI::RunNonQuery(createDatabase, connectionString);
			bExists = true;
		}
	}
	catch(Exception *e)
	{
		MOG_Prompt::PromptMessage("Could not verify SQL database!", e->Message, e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
	}
	__finally
	{
		myConnection->Close();
	}

	return bExists;
}

int MOG_Database::GetDBVersion()
{
	String *tblCmd;
	if(MOG_DBAPI::TableExists("MOG.DBVersion"))
	{
		tblCmd = "SELECT DBVersion FROM [MOG.DBVersion]";
		
		return MOG_DBAPI::GetNewSecularValueAsInt(tblCmd,mConnectionString);	
	}

	return 0;
}

bool MOG_Database::UpdateDBVersion()
{
	String *tblCmd;
	if(MOG_DBAPI::TableExists("MOG.DBVersion"))
	{
		tblCmd = String::Concat(S"UPDATE [MOG.DBVersion] SET DBVersion = ", Convert::ToString(sCurrentDatabaseVersion)) ;
		MOG_DBAPI::RunNonQuery(tblCmd, mConnectionString);
	}

	return true;
}

bool MOG_Database::VerifySystemTables(int databaseVersion)
{
	String *tblCmd;
	if( !MOG_DBAPI::TableExists("MOG.DBVersion") )
	{
		tblCmd = S"CREATE TABLE dbo.[MOG.DBVersion] ( DBVersion int NOT NULL )";
		if(!MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString))
		{
			return false;
		}
		tblCmd = S"INSERT INTO [MOG.DBVersion] (DBVersion) values(0)";
		if(!MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString))
		{
			return false;
		}
	}

	if(MOG_DBAPI::TableExists("MOG.Commands"))
	{
		if(MOG_DBAPI::ColumnExists("MOG.Commands","CheckPendingCommands") || MOG_DBAPI::ColumnExists("MOG.Commands","CheckPendingCommands")||MOG_DBAPI::ColumnExists("MOG.Commands","GameDataPath") )
		{
			tblCmd = S"DROP TABLE [MOG.Commands]";
			if(!MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString))
			{
				return false;
			}			
		}	
	}

	// global MOG tables
	if( !MOG_DBAPI::TableExists("MOG.Commands") )
	{
		tblCmd = MOG_DBQueryBuilderAPI::MOG_TableManagmentQueries::MOG_SystemTablesQueries::Query_CreateCommandsTable(S"MOG.Commands");
			//MOG_DBQueryBuilderAPI::MOG_TableManagmentQueries::MOG_SystemTablesQueries::Query_CreateCommandsTable(S"MOG.Commands");
			
			/* "CREATE TABLE dbo.[MOG.Commands] ( \
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
					Options					varChar(260) NOT NULL, \
					AssetFilename				varChar(260) NOT NULL );";*/

			if(!MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString))
			{
				return false;
			}
	}

	// Create the events table if it does not exist
	if( !MOG_DBAPI::TableExists("MOG.Events") )
	{
		tblCmd = MOG_DBQueryBuilderAPI::MOG_TableManagmentQueries::MOG_SystemTablesQueries::Query_CreateEventsTable(S"MOG.Events");
			/*"CREATE TABLE dbo.[MOG.Events] ( \
					ID				int IDENTITY (1,1) NOT NULL, \
					Type			varChar(50) NOT NULL, \
					TimeStamp		varChar(15) NOT NULL, \
					Title			varChar(100) NOT NULL, \
					StackTrace		varChar(1000) NOT NULL, \
					Description	varChar(1000) NOT NULL, \
					EventID		varChar(50) NOT NULL, \
					UserName		varChar(50) NOT NULL, \
					ComputerName	varChar(50) NOT NULL, \
					ProjectName	varChar(50) NOT NULL, \
					BranchName		varChar(50) NOT NULL, \
					RepeatCount	int DEFAULT 1 NOT NULL );";*/

			if(!MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString))
			{
				return false;
			}
	}

	return true;
}

bool MOG_Database::UpdateSystemTables(int databaseVersion, bool supressMessage)
{
	String *tblCmd;

	// Check if the database version is out-of-date?
	if (databaseVersion < sCurrentDatabaseVersion)
	{
		// Check if we need to supress this message?
		// Also supress this message if the database version has never been defined (new install)
		if (supressMessage == false &&
			databaseVersion > 0)
		{
			if(MOG_Prompt::PromptResponse(S"Updating Database Tables", "If you have not recently backed up your database it is recommended that you do so now. \nWould you like to proceed with the database update?","",MOGPromptButtons::YesNo) == MOG::PROMPT::No)
			{
				return false;
			}
		}
		switch (databaseVersion)
		{
			case 0:
			case 1:
				if( MOG_DBAPI::TableExists("MOG.Events") )
				{
					tblCmd = "DROP TABLE [MOG.Events]";
					MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString);
				}
			case 2:
			default:
				break;
		}
	}

	// Always verify system tables!
	VerifySystemTables(databaseVersion);

	return true;
}

bool MOG_Database::UpdateTables(String *projectName, int databaseVersion, bool supressMessage)
{
	String *tblCmd;
	if(databaseVersion < sCurrentDatabaseVersion)
	{
		// Check if we need to supress this message?
		// Also supress this message if the database version has never been defined (new install)
		if (supressMessage == false &&
			databaseVersion > 0)
		{
			if(MOG_Prompt::PromptResponse(S"Updating Database Tables", "If you have not recently backed up your database it is recommended that you do so now. \nWould you like to proceed with the database update?","",MOGPromptButtons::YesNo) == MOG::PROMPT::No)
			{
				return false;
			}
		}
		switch (databaseVersion)
		{
		case 0:
		case 1:
		case 2:
		case 3:
		case 4:
		case 5:
		case 6:
		case 7:
			if (MOG_DBAPI::TableExists(String::Concat(projectName, S".Branches")) && !MOG_DBAPI::ColumnExists(String::Concat(projectName, S".Branches"), "Tag"))
			{
				// Add a new column to the branch table
				tblCmd = String::Concat(S"ALTER TABLE [", String::Concat(projectName, S".Branches"), S"] ADD Tag BIT NOT NULL DEFAULT 0");
				MOG_DBAPI::RunNonQuery(tblCmd, mConnectionString);
			}
		case 8:
			if (MOG_DBAPI::TableExists(String::Concat(projectName, S".PlatformsBranchLinks")))
			{
				tblCmd = String::Concat(S"DROP TABLE [", projectName, S".PlatformsBranchLinks]");
				MOG_DBAPI::RunNonQuery(tblCmd, mConnectionString);
			}
			if (!MOG_DBAPI::TableExists(String::Concat(projectName, S".PlatformsBranchLinks")))
			{
				// Create a new table
				tblCmd = String::Concat(S"CREATE TABLE dbo.[", projectName, S".PlatformsBranchLinks] (\
										PlatformNameID	int NOT NULL, \
										BranchID		int NOT NULL, \
										PRIMARY KEY (PlatformNameID, BranchID) );");
				if (MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString))
				{
					// Initialize the databse connection information for this project
					Connect(projectName, "Current");

					// Get the project's existing branches
					ArrayList* existingBranches = MOG_DBProjectAPI::GetAllBranchNames(projectName);
					if (existingBranches)
					{
						// Get all of the known platforms
						String *selectString = String::Concat(S"SELECT * FROM dbo.[", projectName, S".Platforms]");
						ArrayList *platformNames = MOG_DBAPI::QueryStrings(selectString, "PlatformName");

						// Get all of the current branches
						for (int b = 0; b < existingBranches->Count; b++)
						{
							MOG_DBBranchInfo* pBranchInfo = __try_cast<MOG_DBBranchInfo*>(existingBranches->Item[b]);
							if (pBranchInfo)
							{
								// Automatically populate this new table with all of the platforms in this project
								for (int p = 0; p < platformNames->Count; p++)
								{
									String* pPlatformName = __try_cast<String*>(platformNames->Item[p]);

									// Add this platform to the PlatformsBranchLinks table
									MOG_DBProjectAPI::AddPlatformToBranch(pPlatformName, pBranchInfo->mBranchName);
								}
							}
						}
					}
				}
			}

		default:
			break;
		}

		VerifyTables(projectName, databaseVersion);
	}
	return true;
}

void MOG_Database::UpdateAssetProperties_Worker(Object* sender, DoWorkEventArgs* e)
{
	BackgroundWorker* worker = dynamic_cast<BackgroundWorker*>(sender);
	List<Object*>* args = dynamic_cast<List<Object*>*>(e->Argument);
	String* projectName = dynamic_cast<String*>(args->Item[0]);
	ArrayList* listOfAssetVersions = dynamic_cast<ArrayList*>(args->Item[1]);

	//itterate through all the asset versions
	for (int assetIndex = 0; assetIndex < listOfAssetVersions->Count; assetIndex++)
	{
		if (worker != NULL)
		{
			//update the progress 
			worker->ReportProgress(assetIndex * 100 / listOfAssetVersions->Count);
		}

		//get a MOG_Filename for the asset we want to update.
		MOG_Filename *currentAsset = dynamic_cast <MOG_Filename*>(listOfAssetVersions->Item[assetIndex]);
		MOG_Filename *blessedAsset = MOG_ControllerRepository::GetAssetBlessedVersionPath(currentAsset, currentAsset->GetVersionTimeStamp());
		
		// We have to do this the ugly way because we have not yet logged into the project
		String *projectPath = String::Concat(MOG_ControllerSystem::GetSystemProjectsPath(), S"\\", projectName);
		MOG_Filename *tokenizedFilename = new MOG_Filename(blessedAsset->GetEncodedFilename()->Replace(S"{ProjectPath}", projectPath));

		//Actually update the INI file 
		MOG_SystemUtilities::DatabaseVersion6_UpdatePropertiesIniFile(tokenizedFilename);
	}
}

bool MOG_Database::RenameTable(String *currentTableName, String *newTableName)
{
	String * renameCommand = String::Concat(S"EXEC SP_RENAME  '", currentTableName, S"',  '", newTableName, S"'");
	return MOG_DBAPI::RunNonQuery(renameCommand, mConnectionString);
}

bool MOG_Database::RenameColumn(String *tableName, String *currentColumnName, String *newColumnName)
{
	String * renameCommand = String::Concat(S"EXEC SP_RENAME  '", tableName,S".",currentColumnName, S"',  '", newColumnName, S"', 'COLUMN'");
	return MOG_DBAPI::RunNonQuery(renameCommand, mConnectionString);
}

bool MOG_Database::VerifyTables(String *projectName)
{
	return MOG_Database::VerifyTables(projectName, sCurrentDatabaseVersion);
}

bool MOG_Database::VerifyTables(String *projectName, int databaseVersion)
{
	bool bVerified = true;
	String *tblCmd;
	if( projectName && projectName->Length )
	{
		if( !MOG_DBAPI::TableExists(String::Concat(projectName, S".AssetClassifications")) )
		{
			tblCmd = String::Concat(S"CREATE TABLE dbo.[", projectName, S".AssetClassifications] (\
									ID						int IDENTITY (1,1) NOT NULL, \
									ClassificationName		varChar(1024) NOT NULL, \
									ParentID				int NOT NULL, \
									FullTreeName			varChar(1024), \
									CreatedDate				varChar(15) NOT NULL, \
									CreatedBy				int NOT NULL, \
									PRIMARY KEY (ClassificationName, ParentID));");

			if(!MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString))
			{
				return false;
			}
		}

		if( !MOG_DBAPI::TableExists(String::Concat(projectName, S".AssetClassificationsBranchLinks")) )
		{
			tblCmd = String::Concat(S"CREATE TABLE dbo.[", projectName, S".AssetClassificationsBranchLinks] (\
									ID						int IDENTITY (1,1) NOT NULL, \
									ClassificationID		int NOT NULL, \
									BranchID				int NOT NULL, \
									PRIMARY KEY (ClassificationID, BranchID));" );

			if(!MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString))
			{
				return false;
			}
		}

		if( !MOG_DBAPI::TableExists(String::Concat(projectName, S".AssetClassificationsProperties")) )
		{
			tblCmd = String::Concat(S"CREATE TABLE dbo.[", projectName, S".AssetClassificationsProperties] (\
									AssetClassificationBranchLinkID	int NOT NULL, \
									Section							varChar(260) NOT NULL, \
									Property						varChar(260) NOT NULL, \
									Value							varChar(1024) NOT NULL, \
									PRIMARY KEY (AssetClassificationBranchLinkID, Section, Property));");

			if(!MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString))
			{
				return false;
			}
		}

		if( !MOG_DBAPI::TableExists(String::Concat(projectName, S".AssetNames")) )
		{
			tblCmd = String::Concat(S"CREATE TABLE dbo.[", projectName, S".AssetNames] (\
									ID						int IDENTITY (1,1) NOT NULL, \
									AssetClassificationID	int NOT NULL, \
									AssetPlatformKey		varChar(50) NOT NULL, \
									AssetLabel				varChar(256) NOT NULL, \
									CreatedDate				varChar(15) NOT NULL, \
									CreatedBy				int NOT NULL, \
									RemovedDate				varChar(15) NOT NULL, \
									RemovedBy				int NOT NULL, \
									PRIMARY KEY (AssetClassificationID, AssetPlatformKey, AssetLabel) );");

			if(!MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString))
			{
				return false;
			}
		}

		if( !MOG_DBAPI::TableExists(String::Concat(projectName, S".AssetProperties")) )
		{
			tblCmd = String::Concat(S"CREATE TABLE dbo.[", projectName, S".AssetProperties] (\
									AssetVersionID	int NOT NULL, \
									Section			varChar(260) NOT NULL, \
									Property		varChar(260) NOT NULL, \
									Value			varChar(1024) NOT NULL, \
									PRIMARY KEY (AssetVersionID, Section, Property));");

	
			MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString);
		}

		if( !MOG_DBAPI::TableExists(String::Concat(projectName, S".AssetVersions")) )
		{
			tblCmd = String::Concat(S"CREATE TABLE dbo.[", projectName, S".AssetVersions] (\
									ID			int IDENTITY (1,1) NOT NULL, \
									AssetNameID int NOT NULL, \
									Version		varChar(15) NOT NULL, \
									CreatedDate varChar(15) NOT NULL, \
									CreatedBy	int NOT NULL, \
									BlessedDate varChar(15) NOT NULL, \
									BlessedBy	int NOT NULL, \
									PRIMARY KEY (ID) );");

			if(!MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString))
			{
				return false;
			}
		}

		if( !MOG_DBAPI::TableExists(String::Concat(projectName, S".Branches")) )
		{
			tblCmd = String::Concat(S"CREATE TABLE dbo.[", projectName, S".Branches] (\
									ID			int IDENTITY (1,1) NOT NULL, \
									BranchName	varChar(50) NOT NULL, \
									CreatedDate varChar(15) NOT NULL, \
									CreatedBy	int NOT NULL, \
									RemovedDate varChar(15) NOT NULL, \
									RemovedBy	int NOT NULL, \
									Tag			bit NOT NULL DEFAULT 0, \
									PRIMARY KEY (ID) );");

			if(!MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString))
			{
				return false;
			}
		}

		if( !MOG_DBAPI::TableExists(String::Concat(projectName, S".BranchLinks")) )
		{
			tblCmd = String::Concat(S"CREATE TABLE dbo.[", projectName, S".BranchLinks] (\
									BranchID		int NOT NULL, \
									AssetVersionID	int NOT NULL, \
									PRIMARY KEY (BranchID, AssetVersionID) );");

			if(!MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString))
			{
				return false;
			}
		}

		if( !MOG_DBAPI::TableExists(String::Concat(projectName, S".Departments")) )
		{
			tblCmd = String::Concat(S"CREATE TABLE dbo.[", projectName, S".Departments] (\
									ID				int IDENTITY (1,1) NOT NULL, \
									DepartmentName	varChar(50) NOT NULL, \
									PRIMARY KEY (ID) );");

			if(!MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString))
			{
				return false;
			}
		}

		if( !MOG_DBAPI::TableExists(String::Concat(projectName, S".SyncTargetFileMap")) )
		{
			tblCmd = String::Concat(S"CREATE TABLE dbo.[", projectName, S".SyncTargetFileMap] (\
									SyncTargetFile	varChar(260) NOT NULL, \
									AssetVersionID	int NOT NULL, \
									PlatformID		int NOT NULL, \
									PRIMARY KEY (SyncTargetFile, AssetVersionID, PlatformID) );");

			if(!MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString))
			{
				return false;
			}
		}

		if( !MOG_DBAPI::TableExists(String::Concat(projectName, S".InboxAssets")) )
		{
			tblCmd = String::Concat(S"CREATE TABLE dbo.[", projectName, S".InboxAssets] (\
									ID					int IDENTITY (1,1) NOT NULL, \
									UserID				int NOT NULL, \
									Box					varChar(30) NOT NULL, \
									GroupPath			varChar(1024) NOT NULL, \
									AssetFullName		varChar(1024) NOT NULL, \
									PRIMARY KEY (UserID, Box, GroupPath, AssetFullName));");

			if(!MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString))
			{
				return false;
			}
		}

		if( !MOG_DBAPI::TableExists(String::Concat(projectName, S".InboxAssetsProperties")) )
		{
			tblCmd = String::Concat(S"CREATE TABLE dbo.[", projectName, S".InboxAssetsProperties] (\
									InboxAssetID	int NOT NULL, \
									Section			varChar(260) NOT NULL, \
									Property		varChar(260) NOT NULL, \
									Value			varChar(1024) NOT NULL, \
									PRIMARY KEY (InboxAssetID, Section, Property));");

			if(!MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString))
			{
				return false;
			}
		}

		if( !MOG_DBAPI::TableExists(String::Concat(projectName, S".PackageCommands")) )
		{
			tblCmd = String::Concat(S"CREATE TABLE dbo.[", projectName, S".PackageCommands] (\
									ID						int IDENTITY (1,1) NOT NULL, \
									AssetName				varChar(1024) NOT NULL, \
									AssetVersionTimestamp	varChar(15) NOT NULL, \
									Label					varChar(50) NOT NULL, \
									BranchName				varChar(50) NOT NULL, \
									Platform				varChar(50) NOT NULL, \
									CreatedBy				varChar(30) NOT NULL, \
									BlessedBy				varChar(30) NOT NULL, \
									PackageName				varChar(1024) NOT NULL, \
									PackageGroups			varChar(1024) NOT NULL, \
									PackageObjects			varChar(1024) NOT NULL, \
									PackageCommandType		int NOT NULL, \
									PRIMARY KEY (ID) );");


			if(!MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString))
			{
				return false;
			}
		}

		if( !MOG_DBAPI::TableExists(String::Concat(projectName, S".PackageGroupNames")) )
		{
			tblCmd = String::Concat(S"CREATE TABLE dbo.[", projectName, S".PackageGroupNames] (\
									ID					int IDENTITY (1,1) NOT NULL, \
									PackageVersionID	int NOT NULL, \
									PackageGroupName	varChar(1024) NOT NULL, \
									CreatedDate			varChar(15) NOT NULL, \
									CreatedBy			int NOT NULL, \
									RemovedDate			varChar(15) NOT NULL, \
									RemovedBy			int NOT NULL, \
									PRIMARY KEY (ID) );");

			if(!MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString))
			{
				return false;
			}
		}

		if( !MOG_DBAPI::TableExists(String::Concat(projectName, S".PackageLinks")) )
		{
			tblCmd = String::Concat(S"CREATE TABLE dbo.[", projectName, S".PackageLinks] (\
									AssetVersionID		int NOT NULL, \
									PackageGroupNameID	int NOT NULL, \
									PRIMARY KEY (AssetVersionID, PackageGroupNameID) );");

			if(!MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString))
			{
				return false;
			}
		}

		if( !MOG_DBAPI::TableExists(String::Concat(projectName, S".Platforms")) )
		{
			tblCmd = String::Concat(S"CREATE TABLE dbo.[", projectName, S".Platforms] (\
									ID				int IDENTITY (1,1) NOT NULL, \
									PlatformName	varChar(50) NOT NULL, \
									PlatformKey		varChar(50) NOT NULL, \
									PRIMARY KEY (ID) );");

			if(!MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString))
			{
				return false;
			}
		}

		if( !MOG_DBAPI::TableExists(String::Concat(projectName, S".PlatformsBranchLinks")) )
		{
			tblCmd = String::Concat(S"CREATE TABLE dbo.[", projectName, S".PlatformsBranchLinks] (\
									PlatformNameID	int NOT NULL, \
									BranchID		int NOT NULL, \
									PRIMARY KEY (PlatformNameID, BranchID) );");

			if(!MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString))
			{
				return false;
			}
		}

		if( !MOG_DBAPI::TableExists(String::Concat(projectName, S".PostCommands")) )
		{
			tblCmd = String::Concat(S"CREATE TABLE dbo.[", projectName, S".PostCommands] (\
									ID						int IDENTITY (1,1) NOT NULL, \
									AssetName				varChar(1024) NOT NULL, \
									AssetVersionTimestamp	varChar(15) NOT NULL, \
									Label					varChar(50) NOT NULL, \
									BranchName				varChar(50) NOT NULL, \
									CreatedBy				varChar(30) NOT NULL, \
									BlessedBy				varChar(30) NOT NULL, \
									PRIMARY KEY (ID) );");

			if(!MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString))
			{
				return false;
			}
		}

		if( !MOG_DBAPI::TableExists(String::Concat(projectName, S".SourceFileMap")) )
		{
			tblCmd = String::Concat(S"CREATE TABLE dbo.[", projectName, S".SourceFileMap] (\
									SourceFilename	varChar(260) NOT NULL, \
									SourcePath		varChar(260) NOT NULL, \
									AssetID			int NOT NULL, \
									PRIMARY KEY (SourceFilename, SourcePath, AssetID) );");

			if(!MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString))
			{
				return false;
			}
		}

		if( !MOG_DBAPI::TableExists(String::Concat(projectName, S".SyncedDataLocations")) )
		{
			tblCmd = String::Concat(S"CREATE TABLE dbo.[", projectName, S".SyncedDataLocations] (\
									ID						int IDENTITY (1,1) NOT NULL, \
									ComputerName			varChar(50) NOT NULL, \
									ProjectName				varChar(50) NOT NULL, \
									PlatformID				int NOT NULL, \
									WorkingDirectory		varChar(260) NOT NULL, \
									BranchID				int NOT NULL, \
									TabText					varChar(260),\
									UserID					int NOT NULL,\
									PRIMARY KEY (ComputerName, ProjectName, PlatformID, WorkingDirectory, UserID) );");

			if(!MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString))
			{
				return false;
			}
		}
		if( !MOG_DBAPI::TableExists(String::Concat(projectName, S".SyncedDataLinks")) )
		{
			tblCmd = String::Concat(S"CREATE TABLE dbo.[", projectName, S".SyncedDataLinks] (\
									SyncedDataLocationID	int NOT NULL, \
									AssetVersionID			int NOT NULL, \
									PRIMARY KEY (SyncedDataLocationID, AssetVersionID) );");

			if(!MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString))
			{
				return false;
			}
		}

		if( !MOG_DBAPI::TableExists(String::Concat(projectName, S".TaskAssetLinks")) )
		{
			tblCmd = String::Concat(S"CREATE TABLE dbo.[", projectName, S".TaskAssetLinks] (\
									ID				int IDENTITY (1,1) NOT NULL, \
									TaskID			int NOT NULL, \
									AssetFullName	varChar(1024) NOT NULL, \
									AssetID			int NOT NULL, \
									PRIMARY KEY (ID) );");

			if(!MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString))
			{
				return false;
			}
		}

		if( !MOG_DBAPI::TableExists(String::Concat(projectName, S".TaskLinks")) )
		{
			tblCmd = String::Concat(S"CREATE TABLE dbo.[", projectName, S".TaskLinks] (\
									ID			int IDENTITY (1,1) NOT NULL, \
									ParentID	int NOT NULL, \
									ChildID		int NOT NULL, \
									PRIMARY KEY (ID) );");

			if(!MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString))
			{
				return false;
			}
		}

		if( !MOG_DBAPI::TableExists(String::Concat(projectName, S".Tasks")) )
		{
			tblCmd = String::Concat(S"CREATE TABLE dbo.[", projectName, S".Tasks] (\
									ID					int IDENTITY (1,1) NOT NULL, \
									TaskName			varChar(50) NOT NULL, \
									BranchID			int NOT NULL, \
									DepartmentID		int NOT NULL, \
									Priority			varChar(10) NOT NULL, \
									CreatorID			int NOT NULL, \
									AssignedToID		int NOT NULL, \
									Status				varChar(10) NOT NULL, \
									Comment				varChar(1024) NOT NULL, \
									PercentComplete		int NOT NULL, \
									CreatedDate			varChar(15) NOT NULL, \
									DueDate				varChar(15) NOT NULL, \
									CompletedDate		varChar(15) NOT NULL, \
									PRIMARY KEY (ID) );");

			if(!MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString))
			{
				return false;
			}
		}

		if( !MOG_DBAPI::TableExists(String::Concat(projectName, S".Users")) )
		{
			tblCmd = String::Concat(S"CREATE TABLE dbo.[", projectName, S".Users] (\
									ID				int IDENTITY (1,1) NOT NULL, \
									UserName		varChar(30) NOT NULL, \
									EmailAddress	varChar(50) NOT NULL, \
									BlessTarget		int NOT NULL, \
									DepartmentID	int NOT NULL, \
									CreatedDate		varChar(15) NOT NULL, \
									CreatedBy		int NOT NULL, \
									RemovedDate		varChar(15) NOT NULL, \
									RemovedBy		int NOT NULL, \
									PRIMARY KEY (ID) );");

			if(!MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString))
			{
				return false;
			}
		}
	}
	return bVerified;
}


bool MOG_Database::CreateClusteredIndex(String *projectName, String *tableName, String *columnName)
{
	return CreateClusteredIndex(projectName, tableName, columnName, true);
}

bool MOG_Database::CreateClusteredIndex(String *projectName, String *tableName, String *columnName, bool bUnique)
{
	String *indexName = String::Concat(columnName, S"_ClusteredIndex");
	String *projectTable = String::Concat(S"dbo.[", projectName, S".", tableName, S"]");
	String *unique = (bUnique == true) ? "UNIQUE" : "";

	// Execute the command
	String *tblCmd = String::Concat(S"CREATE UNIQUE CLUSTERED INDEX ", indexName, S" ON ", projectTable, S" ( ", columnName, S") WITH DROP_EXISTING ");
	return MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString);
}


bool MOG_Database::CreateNonClusteredIndex(String *projectName, String *tableName, String *columnName)
{
	String *indexName = String::Concat(columnName, S"_NonClusteredIndex");
	String *projectTable = String::Concat(S"dbo.[", projectName, S".", tableName, S"]");

	// Execute the command
	String *tblCmd = String::Concat(S"CREATE NONCLUSTERED INDEX ", indexName, S" ON ", projectTable, S" ( ", columnName, S") WITH DROP_EXISTING ");
	return MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString);
}


bool MOG_Database::CreateProjectIndexes(String *projectName)
{
	// Create Indexes
	CreateNonClusteredIndex(projectName, "AssetClassifications", "ID");
	CreateNonClusteredIndex(projectName, "AssetClassifications", "ClassificationName");
	CreateNonClusteredIndex(projectName, "AssetClassifications", "ParentID");
	CreateNonClusteredIndex(projectName, "AssetClassifications", "FullTreeName");

	// Create Indexes
	CreateNonClusteredIndex(projectName, "AssetClassificationsBranchLinks", "ID");
	CreateNonClusteredIndex(projectName, "AssetClassificationsBranchLinks", "ClassificationID");
	CreateNonClusteredIndex(projectName, "AssetClassificationsBranchLinks", "BranchID");

	// Create Indexes
	CreateNonClusteredIndex(projectName, "AssetClassificationsProperties", "AssetClassificationBranchLinkID");
	CreateNonClusteredIndex(projectName, "AssetClassificationsProperties", "Section");
	CreateNonClusteredIndex(projectName, "AssetClassificationsProperties", "Property");
	CreateNonClusteredIndex(projectName, "AssetClassificationsProperties", "Value");

	// Create Indexes
	CreateClusteredIndex(projectName, "AssetNames", "ID");
	CreateNonClusteredIndex(projectName, "AssetNames", "AssetClassificationID");
	CreateNonClusteredIndex(projectName, "AssetNames", "AssetPlatformKey");
	CreateNonClusteredIndex(projectName, "AssetNames", "AssetLabel");

	// Create Indexes
	CreateNonClusteredIndex(projectName, "AssetProperties", "AssetVersionID");
	CreateNonClusteredIndex(projectName, "AssetProperties", "Section");
	CreateNonClusteredIndex(projectName, "AssetProperties", "Property");
	CreateNonClusteredIndex(projectName, "AssetProperties", "Value");

	// Create Indexes
	CreateClusteredIndex(projectName, "AssetVersions", "ID");
	CreateNonClusteredIndex(projectName, "AssetVersions", "AssetNameID");
	CreateNonClusteredIndex(projectName, "AssetVersions", "Version");

	// Create Indexes
	CreateClusteredIndex(projectName, "Branches", "ID");
	CreateNonClusteredIndex(projectName, "Branches", "BranchName");

	// Create Indexes
	CreateNonClusteredIndex(projectName, "BranchLinks", "BranchID");
	CreateNonClusteredIndex(projectName, "BranchLinks", "AssetVersionID");

	// Create Indexes
	CreateClusteredIndex(projectName, "Departments", "ID");
	CreateNonClusteredIndex(projectName, "Departments", "DepartmentName");

	// Create Indexes
	CreateNonClusteredIndex(projectName, "SyncTargetFileMap", "SyncTargetFile");
	CreateNonClusteredIndex(projectName, "SyncTargetFileMap", "AssetVersionID");
	CreateNonClusteredIndex(projectName, "SyncTargetFileMap", "PlatformID");

	// Create Indexes
	CreateNonClusteredIndex(projectName, "InboxAssets", "ID");
	CreateNonClusteredIndex(projectName, "InboxAssets", "UserID");
	CreateNonClusteredIndex(projectName, "InboxAssets", "Box");
	CreateNonClusteredIndex(projectName, "InboxAssets", "AssetFullName");

	// Create Indexes
	CreateNonClusteredIndex(projectName, "InboxAssetsProperties", "InboxAssetID");
	CreateNonClusteredIndex(projectName, "InboxAssetsProperties", "Section");
	CreateNonClusteredIndex(projectName, "InboxAssetsProperties", "Property");
	CreateNonClusteredIndex(projectName, "InboxAssetsProperties", "Value");

	// Create Indexes
	CreateNonClusteredIndex(projectName, "PackageLinks", "AssetVersionID");
	CreateNonClusteredIndex(projectName, "PackageLinks", "PackageGroupNameID");

	// Create Indexes
	CreateClusteredIndex(projectName, "Platforms", "ID");
	CreateNonClusteredIndex(projectName, "Platforms", "PlatformName");
	CreateNonClusteredIndex(projectName, "Platforms", "PlatformKey");

	// Create Indexes
	CreateNonClusteredIndex(projectName, "SourceFileMap", "SourceFilename");
	CreateNonClusteredIndex(projectName, "SourceFileMap", "SourcePath");
	CreateNonClusteredIndex(projectName, "SourceFileMap", "AssetID");

	// Create Indexes
	CreateNonClusteredIndex(projectName, "SyncedDataLocations", "ID");

	// Create Indexes
	CreateNonClusteredIndex(projectName, "SyncedDataLinks", "SyncedDataLocationID");
	CreateNonClusteredIndex(projectName, "SyncedDataLinks", "AssetVersionID");

	// Create Indexes
	CreateClusteredIndex(projectName, "Users", "ID");
	CreateNonClusteredIndex(projectName, "Users", "UserName");

	return true;
}

bool MOG_Database::SingleDatabasePort(String *oldDatabaseConnectionString, String *oldProjectName, String *newDatabaseConnectionString)
{
//	String * tblCmd = "";
//	
//
//	SELECT * INTO TestProject2.dbo.AssetClassifications FROM MOGMAIN.dbo.[DEMSHOOTER.AssetClassifications] 
//SELECT * INTO TestProject2.dbo.AssetClassificationsBranchLinks FROM MOGMAIN.dbo.[DEMSHOOTER.AssetClassificationsBranchLinks] 
//SELECT * INTO TestProject2.dbo.AssetNames FROM MOGMAIN.dbo.[DEMSHOOTER.AssetNames] 
//SELECT * INTO TestProject2.dbo.AssetProperties FROM MOGMAIN.dbo.[DEMSHOOTER.AssetProperties] 
//SELECT * INTO TestProject2.dbo.AssetVersions FROM MOGMAIN.dbo.[DEMSHOOTER.AssetVersions] 
//SELECT * INTO TestProject2.dbo.Branches FROM MOGMAIN.dbo.[DEMSHOOTER.Branches] 
//SELECT * INTO TestProject2.dbo.BranchLinks FROM MOGMAIN.dbo.[DEMSHOOTER.BranchLinks] 
//SELECT * INTO TestProject2.dbo.Departments FROM MOGMAIN.dbo.[DEMSHOOTER.Departments] 
//SELECT * INTO TestProject2.dbo.InboxAssets FROM MOGMAIN.dbo.[DEMSHOOTER.InboxAssets] 
//SELECT * INTO TestProject2.dbo.InboxAssetsProperties FROM MOGMAIN.dbo.[DEMSHOOTER.InboxAssetsProperties] 
//SELECT * INTO TestProject2.dbo.PackageCommands FROM MOGMAIN.dbo.[DEMSHOOTER.PackageCommands] 
//SELECT * INTO TestProject2.dbo.PackageGroupNames FROM MOGMAIN.dbo.[DEMSHOOTER.PackageGroupNames] 
//SELECT * INTO TestProject2.dbo.PackageLinks FROM MOGMAIN.dbo.[DEMSHOOTER.PackageLinks] 
//SELECT * INTO TestProject2.dbo.Platforms FROM MOGMAIN.dbo.[DEMSHOOTER.Platforms] 
//SELECT * INTO TestProject2.dbo.PostCommands FROM MOGMAIN.dbo.[DEMSHOOTER.PostCommands] 
//SELECT * INTO TestProject2.dbo.SourceFileMap FROM MOGMAIN.dbo.[DEMSHOOTER.SourceFileMap] 
//SELECT * INTO TestProject2.dbo.SyncedDataLinks FROM MOGMAIN.dbo.[DEMSHOOTER.SyncedDataLinks] 
//SELECT * INTO TestProject2.dbo.SyncedDataLocations FROM MOGMAIN.dbo.[DEMSHOOTER.SyncedDataLocations] 
//SELECT * INTO TestProject2.dbo.SyncTargetFileMap FROM MOGMAIN.dbo.[DEMSHOOTER.SyncTargetFileMap] 
//SELECT * INTO TestProject2.dbo.TaskAssetLinks FROM MOGMAIN.dbo.[DEMSHOOTER.TaskAssetLinks] 
//SELECT * INTO TestProject2.dbo.TaskLinks FROM MOGMAIN.dbo.[DEMSHOOTER.TaskLinks] 
//SELECT * INTO TestProject2.dbo.Tasks FROM MOGMAIN.dbo.[DEMSHOOTER.Tasks] 
//if(MOG_DBAPI::TableExists(String::Concat(projectName, S".Users")) )
//{
//SELECT * INTO TestProject2.dbo.Users FROM MOGMAIN.dbo.[DEMSHOOTER.Users] 
//}
	return true;
}
bool MOG_Database::DeleteMOGTables()
{
	bool bDeleted = true;

	String *tblCmd;

	// global MOG tables
	if( MOG_DBAPI::TableExists("MOG.Commands") )
	{
		tblCmd = S"DROP TABLE [MOG.Commands]";

		MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString);
	}

	if( MOG_DBAPI::TableExists("MOG.Events") )
	{
		tblCmd = S"DROP TABLE [MOG.Events]";

		MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString);
	}

	return bDeleted;
}

bool MOG_Database::UpdatePropertiesTableProperties(String *tableName)
{

	try
	{
		//change from {Asset Info} to {Asset Stats}
		UpdatePropertySection(tableName, S"{AssetInfo}Creator",										S"{Asset Stats}Creator");
		UpdatePropertySection(tableName, S"{AssetInfo}Owner",										S"{Asset Stats}Owner");
		UpdatePropertySection(tableName, S"{AssetInfo}SourceMachine",								S"{Asset Stats}SourceMachine");
		UpdatePropertySection(tableName, S"{AssetInfo}SourcePath",									S"{Asset Stats}SourcePath");
		UpdatePropertySection(tableName, S"{AssetInfo}CreatedTime",									S"{Asset Stats}CreatedTime");
		UpdatePropertySection(tableName, S"{AssetInfo}ModifiedTime",								S"{Asset Stats}ModifiedTime");
		UpdatePropertySection(tableName, S"{AssetInfo}Status",										S"{Asset Stats}Status");
		UpdatePropertySection(tableName, S"{AssetInfo}Size",										S"{Asset Stats}Size");

		//change from {MOGProperty} to {Asset Info}
		UpdatePropertySection(tableName, S"{MOGProperty}Description",								S"{Asset Info}Description");
		UpdatePropertySection(tableName, S"{MOGProperty}AssetIcon",									S"{Asset Info}AssetIcon");
		UpdatePropertySection(tableName, S"{MOGProperty}AssetViewer",								S"{Asset Info}AssetViewer");
		UpdatePropertySection(tableName, S"{MOGProperty}PropertyMenu",								S"{Asset Info}PropertyMenu");
		UpdatePropertySection(tableName, S"{MOGProperty}PackagedAsset",								S"{Asset Info}IsPackagedAsset");		//Change from PackagedAsset to IsPackagedAsset
		UpdatePropertySection(tableName, S"{MOGProperty}Package",									S"{Asset Info}IsPackage");				//Changed from Package to IsPackage
		UpdatePropertySection(tableName, S"{MOGProperty}Build",										S"{Asset Info}IsBuild");				//Changed from Build to IsBuild

		//Changed from {MOGProperty} to {Asset Options}
		UpdatePropertySection(tableName, S"{MOGProperty}SyncTargetPath",							S"{Asset Options}SyncTargetPath");
		UpdatePropertySection(tableName, S"{MOGProperty}ValidSlaves",								S"{Asset Options}ValidSlaves");
		UpdatePropertySection(tableName, S"{MOGProperty}ShowRipCommandWindow",						S"{Asset Options}ShowRipCommandWindow");
		UpdatePropertySection(tableName, S"{MOGProperty}MaintainLock",								S"{Asset Options}MaintainLock");
		UpdatePropertySection(tableName, S"{MOGProperty}UnReferencedRevisionHistory",				S"{Asset Options}UnReferencedRevisionHistory");

		//Changed from {MOGProperty} to {Classification Info}
		UpdatePropertySection(tableName, S"{MOGProperty}Library",									S"{Classification Info}IsLibrary");		//Changed from Library to IsLibrary
		UpdatePropertySection(tableName, S"{MOGProperty}ClassIcon",									S"{Classification Info}ClassIcon");
		UpdatePropertySection(tableName, S"{MOGProperty}Classification",							S"{Classification Info}Classification");

		//Changed from {MOGProperty} to {Sync Options}
		//UpdatePropertySection(tableName, S"{MOGProperty}SyncRootNodeLabel",						S"{Sync Options}SyncRootNodeLabel");  
		UpdatePropertySection(tableName, S"{MOGProperty}SyncRootNode",								S"{Sync Options}SyncLabel");  

		//Changed from {MOGProperty} to {Rip Options}
		UpdatePropertySection(tableName, S"{MOGProperty}AssetRipTasker",							S"{Rip Options}AssetRipTasker");
		UpdatePropertySection(tableName, S"{MOGProperty}AssetRipper",								S"{Rip Options}AssetRipper");
		UpdatePropertySection(tableName, S"{MOGProperty}DivergentPlatformDataType",					S"{Rip Options}DivergentPlatformDataType");
		UpdatePropertySection(tableName, S"{MOGProperty}UseTempRipDir",								S"{Rip Options}UseTempRipDir");
		UpdatePropertySection(tableName, S"{MOGProperty}UseLocalTempRipDir",						S"{Rip Options}UseLocalTempRipDir");
		UpdatePropertySection(tableName, S"{MOGProperty}CopyFilesIntoTempRipDir",					S"{Rip Options}CopyFilesIntoTempRipDir");
		UpdatePropertySection(tableName, S"{MOGProperty}AutoDetectRippedFiles",						S"{Rip Options}AutoDetectRippedFiles");

		//Changed from {MOGProperty} to {Package Options}
		UpdatePropertySection(tableName, S"{MOGProperty}PackageStyle",								S"{Package Options}PackageStyle");
		UpdatePropertySection(tableName, S"{MOGProperty}PackageWorkingDirectory",					S"{Package Options}PackageWorkingDirectory");
		UpdatePropertySection(tableName, S"{MOGProperty}CleanupPackageWorkingDirectory",			S"{Package Options}CleanupPackageWorkingDirectory");
		UpdatePropertySection(tableName, S"{MOGProperty}AutoPackage",								S"{Package Options}AutoPackage");
		UpdatePropertySection(tableName, S"{MOGProperty}ClusterPackaging",							S"{Package Options}ClusterPackaging");
		UpdatePropertySection(tableName, S"{MOGProperty}PackagePreMergeEvent",						S"{Package Options}PackagePreMergeEvent");
		UpdatePropertySection(tableName, S"{MOGProperty}PackagePostMergeEvent",						S"{Package Options}PackagePostMergeEvent");
		UpdatePropertySection(tableName, S"{MOGProperty}InputPackageTaskFile",						S"{Package Options}InputPackageTaskFile");
		UpdatePropertySection(tableName, S"{MOGProperty}OutputPackageTaskFile",						S"{Package Options}OutputPackageTaskFile");
		UpdatePropertySection(tableName, S"{MOGProperty}PackageTool",								S"{Package Options}TaskFileTool");							//Changed from PackageTool to TaskFileTool
		UpdatePropertySection(tableName, S"{MOGProperty}PackageCommand_DeletePackage",				S"{Package Options}PackageCommand_DeletePackageFile");		//Changed from PackageCommand_DeletePackage to PackageCommand_DeletePackageFile
		UpdatePropertySection(tableName, S"{MOGProperty}PackageCommand_ResolveLateResolvers",		S"{Package Options}PackageCommand_ResolveLateResolvers");		

		//Changed from {MOGProperty} to {Package Commands}
		UpdatePropertySection(tableName, S"{MOGProperty}PackageCommand_AddAsset",					S"{Packaging Commands}PackageCommand_AddFile");				//Changed from PackageCommand_AddAsset to PackageCommand_AddFile
		UpdatePropertySection(tableName, S"{MOGProperty}PackageCommand_RemoveAsset",				S"{Packaging Commands}PackageCommand_RemoveFile");			//Changed From PackageCommand_RemoveAsset to PackageCommand_RemoveFile

		//Changed from {MOGProperty to {Build Options}
		UpdatePropertySection(tableName, S"{MOGProperty}BuildTool",									S"{Build Options}BuildTool");
		UpdatePropertySection(tableName, S"{MOGProperty}BuildWorkingDirectory",						S"{Build Options}BuildWorkingDirectory");

		//Remove properties no longer used
		RemovePropertySection(tableName, S"{MOGProperty}NativeDataType");
		RemovePropertySection(tableName, S"{MOGProperty}GameDataAsset");
	}
	catch(Exception *e)
	{
		if(e)
		{
			return false;
		}
	}
	return true;

}

//This function is used to updated the PropertySection for the various properties.
bool  MOG_Database::UpdatePropertySection(String *tableName, String *oldProperty, String *newProperty)
{
	String *updateQuery = String::Concat( S"UPDATE [", tableName, S"] SET [Property] = '", newProperty, S"' WHERE ([Property] = '", oldProperty, S"')");
	return MOG_DBAPI::RunNonQuery(updateQuery, mConnectionString);
}

//This function removes depricated PropertySections
bool MOG_Database::RemovePropertySection(String *tableName, String *propertyToRemove)
{
	String *updateQuery = String::Concat( S"DELETE FROM [", tableName, S"] WHERE [Property] = '", propertyToRemove, S"'");
	return MOG_DBAPI::RunNonQuery(updateQuery, mConnectionString);
}

bool MOG_Database::DeleteProjectTables(String *projectName)
{
	bool bDeleted = true;

	String *tblCmd;

	if( projectName && projectName->Length )
	{
		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".AssetClassifications")) )
		{
			tblCmd = String::Concat(S"DROP TABLE [", projectName, S".AssetClassifications]");
			MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString);
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".AssetClassificationsBranchLinks")) )
		{
			tblCmd = String::Concat(S"DROP TABLE [", projectName, S".AssetClassificationsBranchLinks]");
			MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString);
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".AssetClassificationsProperties")) )
		{
			tblCmd = String::Concat(S"DROP TABLE [", projectName, S".AssetClassificationsProperties]");
			MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString);
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".AssetNames")) )
		{
			tblCmd = String::Concat(S"DROP TABLE [", projectName, S".AssetNames]");
			MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString);
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".AssetProperties")) )
		{
			tblCmd = String::Concat(S"DROP TABLE [", projectName, S".AssetProperties]");
			MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString);
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".AssetVersions")) )
		{
			tblCmd = String::Concat(S"DROP TABLE [", projectName, S".AssetVersions]");

			MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString);
		}
		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".Branches")) )
		{
			tblCmd = String::Concat(S"DROP TABLE [", projectName, S".Branches]");

			MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString);
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".BranchLinks")) )
		{
			tblCmd = String::Concat(S"DROP TABLE [", projectName, S".BranchLinks]");

			MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString);
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".Departments")) )
		{
			tblCmd = String::Concat(S"DROP TABLE [", projectName, S".Departments]");

			MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString);
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".SyncTargetFileMap")) )
		{
			tblCmd = String::Concat(S"DROP TABLE [", projectName, S".SyncTargetFileMap]");

			MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString);
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".InboxAssets")) )
		{
			tblCmd = String::Concat(S"DROP TABLE [", projectName, S".InboxAssets]");

			MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString);
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".InboxAssetsProperties")) )
		{
			tblCmd = String::Concat(S"DROP TABLE [", projectName, S".InboxAssetsProperties]");

			MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString);
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".PackageCommands")) )
		{
			tblCmd = String::Concat(S"DROP TABLE [", projectName, S".PackageCommands]");

			MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString);
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".PackageGroupNames")) )
		{
			tblCmd = String::Concat(S"DROP TABLE [", projectName, S".PackageGroupNames]");

			MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString);
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".PackageLinks")) )
		{
			tblCmd = String::Concat(S"DROP TABLE [", projectName, S".PackageLinks]");

			MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString);
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".Platforms")) )
		{
			tblCmd = String::Concat(S"DROP TABLE [", projectName, S".Platforms]");

			MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString);
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".PlatformsBranchLinks")) )
		{
			tblCmd = String::Concat(S"DROP TABLE [", projectName, S".PlatformsBranchLinks]");

			MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString);
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".PostCommands")) )
		{
			tblCmd = String::Concat(S"DROP TABLE [", projectName, S".PostCommands]");

			MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString);
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".SourceFileMap")) )
		{
			tblCmd = String::Concat(S"DROP TABLE [", projectName, S".SourceFileMap]");

			MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString);
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".SyncedDataLocations")) )
		{
			tblCmd = String::Concat(S"DROP TABLE [", projectName, S".SyncedDataLocations]");
			MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString);
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".SyncedDataLinks")) )
		{
			tblCmd = String::Concat(S"DROP TABLE [", projectName, S".SyncedDataLinks]");

			MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString);;
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".TaskAssetLinks")) )
		{
			tblCmd = String::Concat(S"DROP TABLE [", projectName, S".TaskAssetLinks]");

			MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString);
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".TaskLinks")) )
		{
			tblCmd = String::Concat(S"DROP TABLE [", projectName, S".TaskLinks]");
			MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString);
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".Tasks")) )
		{
			tblCmd = String::Concat(S"DROP TABLE [", projectName, S".Tasks]");
			MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString);
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".Users")) )
		{
			tblCmd = String::Concat(S"DROP TABLE [", projectName, S".Users]");

			MOG_DBAPI::RunNonQuery(tblCmd,mConnectionString);
		}
	}
	return bDeleted;
}


void MOG_Database::Test(void)
{
}

bool MOG_Database::ProjectTablesExist(String *projectName)
{
	SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(mConnectionString); //new SqlConnection(mConnectionString);

	if( projectName && projectName->Length )
	{
		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".AssetClassifications")) )
		{
			return true;
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".AssetClassificationsBranchLinks")) )
		{
			return true;
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".AssetClassificationsProperties")) )
		{
			return true;
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".AssetNames")) )
		{
			return true;
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".AssetProperties")) )
		{
			return true;
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".AssetVersions")) )
		{
			return true;
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".Branches")) )
		{
			return true;
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".BranchLinks")) )
		{
			return true;
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".Departments")) )
		{
			return true;
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".SyncTargetFileMap")) )
		{
			return true;
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".InboxAssets")) )
		{
			return true;
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".InboxAssetsProperties")) )
		{
			return true;
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".PackageCommands")) )
		{
			return true;
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".PackageGroupNames")) )
		{
			return true;
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".PackageLinks")) )
		{
			return true;
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".Platforms")) )
		{
			return true;
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".PostCommands")) )
		{
			return true;
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".SourceFileMap")) )
		{
			return true;
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".SyncedDataLocations")) )
		{
			return true;
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".SyncedDataLinks")) )
		{
			return true;
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".TaskAssetLinks")) )
		{
			return true;
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".TaskLinks")) )
		{
			return true;
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".Tasks")) )
		{
			return true;
		}

		if( MOG_DBAPI::TableExists(String::Concat(projectName, S".Users")) )
		{
			return true;
		}
	}
	myConnection->Close();
	return false;
}
ArrayList *MOG_Database::GetAllTableNames(void)
{

		ArrayList *tablesList = new ArrayList();
		SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(mConnectionString);

		String *getTablesCommand = S"SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES";
		SqlDataReader *listOfTables = MOG_DBAPI::GetNewDataReader(getTablesCommand, myConnection);
	try
	{
		while(listOfTables->Read())
		{
			tablesList->Add(listOfTables->GetString(0));
		}
	}
	catch(Exception *e)
	{
		MOG_Prompt::PromptMessage("Could not verify SQL database!", e->Message, e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);				
	}
	__finally
	{
		myConnection->Close();
	}
	return tablesList;
}

bool MOG_Database::SetDefaultConnectionString(String *connectionString)
{
	// Set the secified connectionString
	mConnectionString = connectionString;

	try
	{
		// Save the new default connection string in the local config file for future reference
		String *filename = MOG_Main::FindLocalConfigFile();
		if (filename && filename->Length)
		{
			// Special fix for Windows7 - Make sure we are running from within this path before we dare write to the 'mog.ini' file
			if (DosUtils::PathIsWithinPath(Application::StartupPath, filename))
			{
				MOG_Ini *config = new MOG_Ini();

				// Open this local config file for writing
				if (config->Open(filename, FileShare::ReadWrite))
				{
					try
					{
						// Save the specified connection string
						config->PutString(S"SQL", S"ConnectionString", connectionString);
						config->Close();
						config = NULL;
					}
					catch (...)
					{
						// Wow, something bad happened...Make sure we release this file handle
						config->CloseNoSave();
					}
				}
			}
		}
	}
	catch (...)
	{
	}

	return true;
}

// JohnRen - TEMP CODE - This can be removed anytime as this was only needed as we transitioned to a longer timestamp
bool MOG_Database::AlterAllDateColumns()
{
	SqlConnection *connectionToAlterColumns = MOG_DBAPI::GetOpenSqlConnection(mConnectionString);
	
	SqlConnection *secondaryConnectionToAlterColumns = MOG_DBAPI::GetOpenSqlConnection(mConnectionString);

	String *verifyAlter = S"SELECT COUNT(*) AS NumQualifyingColumns from INFORMATION_SCHEMA.COLUMNS WHERE CHARACTER_MAXIMUM_LENGTH = 12 AND TABLE_NAME IN (SELECT TABLE_NAME from INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE <> 'VIEW')";
	if(MOG_DBAPI::GetNewSecularValueAsInt(verifyAlter, mConnectionString) != 0)
	{
		String *getOneOffColumnsToAlter = S"SELECT TABLE_NAME, COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS WHERE CHARACTER_MAXIMUM_LENGTH = 12 AND COLUMN_NAME = 'Label'";
		SqlDataReader *oneOffColumnsToAlter = MOG_DBAPI::GetNewDataReader(getOneOffColumnsToAlter, connectionToAlterColumns);
		while(oneOffColumnsToAlter->Read())
		{
			String *oneOffAlterCommand = String::Concat(S"ALTER TABLE [", oneOffColumnsToAlter->GetString(0), S"] ALTER COLUMN ", oneOffColumnsToAlter->GetString(1), S" varchar(50) NOT NULL");
			MOG_DBAPI::RunNonQuery(oneOffAlterCommand, mConnectionString);
		}
		//Get all the columns in the System with a Length Of 12
		try
		{
		String *getColumnsToAlter = S"SELECT TABLE_NAME, COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS WHERE CHARACTER_MAXIMUM_LENGTH = 12 AND TABLE_NAME IN (SELECT TABLE_NAME from INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE <> 'VIEW')";
		SqlDataReader *columnsToAlter = MOG_DBAPI::GetNewDataReader(getColumnsToAlter,secondaryConnectionToAlterColumns);
		//SqlDataReader *columnsToAlter = getColumnsToAlter->ExecuteReader();

			while(columnsToAlter->Read())
			{
				String *alterCommands = String::Concat(S"ALTER TABLE [", columnsToAlter->GetString(0), S"] ALTER COLUMN ", columnsToAlter->GetString(1), S" varchar(15) NOT NULL");
				MOG_DBAPI::RunNonQuery(alterCommands, mConnectionString);
			}
		}
		catch(Exception *e)
		{
			MOG_Prompt::PromptMessage("Could not verify SQL database!", e->Message, e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);				
		}
		__finally
		{
			connectionToAlterColumns->Close();
			secondaryConnectionToAlterColumns->Close();
		}
	}
	return true;
}

bool MOG_Database::CopyTable(String *sourceTable, String *destinationTable)
{
	return false;
}


bool MOG_Database::ParseConnectionString(String *dbConnectionString, String **outMachineName, String **outCatalog, String **outSecurity, String **outUserID, String **outUserPassword)
{
//	// Example Connection String
//	dbConnectionString = S"Packet size=4096;integrated security=SSPI;data source=KIERWORK\MOGEXPRESS;persist security info=False;initial catalog=MOGtest;Packet size=4096;integrated security=SSPI;data source=NEMESIS;persist security info=False;initial catalog=mog16";

	// Validate the dbConnectionString contains the bare minimum information?
	if (dbConnectionString->ToLower()->IndexOf(S"Data Source="->ToLower()) != -1  &&
		dbConnectionString->ToLower()->IndexOf(S"Initial Catalog="->ToLower()) != -1)
	{
		// Split up the connection string
		String *parts[] = dbConnectionString->Split(S";"->ToCharArray());

		for (int i = 0; i < parts->Count; i++)
		{
			String *part = parts[i]->ToLower()->Trim();

			if (part->StartsWith(S"Data Source="->ToLower()))
			{
				*outMachineName = part->Substring(part->IndexOf("=") + 1);
			}
			if (part->StartsWith(S"Initial Catalog="->ToLower()))
			{
				*outCatalog = part->Substring(part->IndexOf("=") + 1);
			}
			if (part->StartsWith(S"Integrated Security="->ToLower()))
			{
				*outSecurity = part->Substring(part->IndexOf("=") + 1);
			}
			if (part->StartsWith(S"User ID="->ToLower()))
			{
				*outUserID = part->Substring(part->IndexOf("=") + 1);
			}
			if (part->StartsWith(S"Password="->ToLower()))
			{
				*outUserPassword = part->Substring(part->IndexOf("=") + 1);
			}
		}

		return true;
	}

	return false;
}


bool MOG_Database::ExportProjectTables(String *projectName, String *projectLocation)
{
	String *outputDirectory = String::Concat(projectLocation, S"\\Database");
	String *projectHeader = String::Concat(projectName, S".");
	bool bFailed = false;

	// Create the database directory
	if (DosUtils::DirectoryCreate(outputDirectory, true))
	{
		// Get all of the database table names
		ArrayList *tableNames = MOG_ControllerSystem::GetDB()->GetAllTableNames();
		if (tableNames)
		{
			bool bDumpedTable = false;

			// Dump all of the tables
			for (int t = 0; t < tableNames->Count; t++)
			{
				String *tableName = __try_cast<String *>(tableNames->Item[t]);

				// Check if this table is related to the specified project?
				if (tableName->StartsWith(projectHeader, StringComparison::CurrentCultureIgnoreCase))
				{
					// Exclude these tables...
					if (// Tasks are disabled
						tableName->EndsWith(S".Tasks", StringComparison::CurrentCultureIgnoreCase) ||
						tableName->EndsWith(S".TaskAssetLinks", StringComparison::CurrentCultureIgnoreCase) ||
						tableName->EndsWith(S".TaskLinks", StringComparison::CurrentCultureIgnoreCase) ||
						// SyncedDataLinks are not needed because they will simply be restored on the next GetLatest
						tableName->EndsWith(S".SyncedDataLinks", StringComparison::CurrentCultureIgnoreCase) )
					{
						continue;
					}

					// Dump the table's data
					if (ExportProjectTable(outputDirectory, tableName))
					{
						bDumpedTable = true;
					}
					else
					{
						bFailed = true;
					}
				}
			}

			// Check if we failed to ever dump a table?
			if (!bDumpedTable)
			{
				bFailed = true;
			}
		}
	}
	else
	{
	}

	if (!bFailed)
	{
		return true;
	}
	return false;
}


bool MOG_Database::ExportProjectTable(String *outputDirectory, String *tableName)
{
	ArrayList *environment = new ArrayList();
	String *output = "";
	bool bFailed = false;

	String *dataFilename = String::Concat(tableName, S".data");
	String *formatFilename = String::Concat(tableName, S".format");

	try
	{
		// Obtain the table data from the database
		String *wrappedTableName = String::Concat(S"[", tableName, S"]");
		String *command = String::Concat("SELECT * FROM ", wrappedTableName, S";");
		SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());
		SqlCommand *result = new SqlCommand(command, myConnection);

		// Fill the dataTable with the database data
		SqlDataAdapter *a = new SqlDataAdapter(result);
		DataTable *d = new DataTable(wrappedTableName);
		a->Fill(d);

		// Write out the XML files for the table
		d->WriteXmlSchema(String::Concat(outputDirectory, S"\\", formatFilename));
		d->WriteXml(String::Concat(outputDirectory, S"\\", dataFilename));
	}
	catch(Exception *e)
	{
		MOG_Report::ReportMessage(S"Export Database Failed", e->Message, e->StackTrace, MOG_ALERT_LEVEL::ERROR);
		bFailed = true;
	}
	__finally
	{
	}

	if (!bFailed)
	{
		return true;
	}
	return false;
}


bool MOG_Database::ImportProjectTables(String *projectName, String *projectLocation)
{
	String *projectDatabasePath = String::Concat(projectLocation, S"\\Database");
	bool bFailed = false;

	// Get all of the contained database table files
	FileInfo *databaseTableNames[] = DosUtils::FileGetList(projectDatabasePath, S"*.data");
	if (databaseTableNames && databaseTableNames->Length)
	{
		// Import each database table file
		for (int i = 0; i < databaseTableNames->Length; i++)
		{
			String *fileName = DosUtils::PathGetFileNameWithoutExtension(databaseTableNames[i]->Name);
			String *oldProjectHeader = fileName->Substring(0, fileName->IndexOf(S".") + 1);
			String *oldTableName = fileName;
			String *baseTableName = oldTableName->Substring(oldProjectHeader->Length);
			String *newTableName = String::Concat(projectName, S".", baseTableName);
			ImportProjectTable(databaseTableNames[i]->DirectoryName, newTableName, fileName);
		}
	}
	else
	{
		String *message = String::Concat(	S"Unable to locate the database tables in the specified project.\n\n",
											S"This project appears to be corrupt.");
		MOG_Prompt::PromptMessage(S"Import Project's Database Failed", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
		bFailed = true;
	}

	if (!bFailed)
	{
		return true;
	}
	return false;
}


bool MOG_Database::ImportProjectTable(String *inputDirectory, String *tableName, String *fileName)
{
	ArrayList *environment = new ArrayList();
	String *output = "";
	bool bFailed = false;

	String *dataFilename = String::Concat(fileName, S".data");
	String *formatFilename = String::Concat(fileName, S".format");

	try
	{
		String *connectionString = MOG_ControllerSystem::GetDB()->GetConnectionString();
		String *wrappedTableName = String::Concat(S"[", fileName, S"]");

		DataTable *dataTable = new DataTable(wrappedTableName);
		dataTable->ReadXmlSchema(String::Concat(inputDirectory, S"\\", formatFilename));
		dataTable->ReadXml(String::Concat(inputDirectory, S"\\", dataFilename));

		// Push the loaded table up to the database
		if (!MySqlBulkCopy(dataTable, tableName))
		{
			bFailed = true;
		}
	}
	catch (Exception *e)
	{
		MOG_Report::ReportMessage(S"Import Database Failed", e->Message, e->StackTrace, MOG_ALERT_LEVEL::ERROR);
		bFailed = true;
	}

	if (!bFailed)
	{
		return true;
	}
	return false;
}


bool MOG_Database::MySqlBulkCopy(DataTable *dataTable, String *tableName)
{
	bool bFailed = false;

	try
	{
		ArrayList *transactionCommands = new ArrayList();

		bool bHasIdentity = false;

		// Build the column names
		String *columnNames = S"";
		for (int c = 0; c < dataTable->Columns->Count; c++)
		{
			DataColumn *col = __try_cast <DataColumn*> (dataTable->Columns->Item[c]);

			// Check if we need to add a comma delimiter?
			if (columnNames->Length)
			{
				columnNames = String::Concat(columnNames, S", ");
			}
			// Add the new column name
			columnNames = String::Concat(columnNames, col->ColumnName);

			// Check if we are still looking for an identity?
			if (bHasIdentity == false)
			{
				// Check if the column is an identity
				String *select = String::Concat(S"SELECT COLUMNPROPERTY( OBJECT_ID('", dataTable->TableName, S"'),'", col->ColumnName, S"','ISIDENTITY') AS Result");
				int result = MOG_DBAPI::QueryInt(select, S"Result");
				if (result)
				{
					bHasIdentity = true;
				}
			}
		}

		// Rename the table being imported to the specified tableName
		String *wrappedTableName = String::Concat(S"[", tableName, S"]");

		// Activate the IDENTITY_INSERT property?
		if (bHasIdentity)
		{
			transactionCommands->Add(String::Concat(S"SET IDENTITY_INSERT ", wrappedTableName, S" ON"));
		}

		// Process each row in the dataTable
		for (int r = 0; r < dataTable->Rows->Count; r++)
		{
			DataRow *row = __try_cast <DataRow*> (dataTable->Rows->Item[r]);

			// Build the column values
			String *columnValues = S"";
			for (int i = 0; i < row->ItemArray->Count; i++)
			{
				// Check if we need to add a comma delimiter?
				if (columnValues->Length)
				{
					columnValues = String::Concat(columnValues, S", ");
				}
				// Add the new column name
				String *type = row->Item[i]->GetType()->ToString();
				if (String::Compare(type, S"System.String", true) == 0 ||
					String::Compare(type, S"System.Boolean", true) == 0)
				{
					// Add the string with single quotes
					columnValues = String::Concat(columnValues, String::Concat(S"'", MOG_DBAPI::FixSQLParameterString(row->Item[i]->ToString()), S"'"));
				}
				else
				{
					// Add the value
					columnValues = String::Concat(columnValues, row->Item[i]->ToString());
				}
			}

			// Add into the transaction list
			transactionCommands->Add(String::Concat(S"INSERT INTO ", wrappedTableName,
													S" (", columnNames, S") VALUES (", columnValues, S")"));
		}

		// Restore the IDENTITY_INSERT property?
		if (bHasIdentity)
		{
			transactionCommands->Add(String::Concat(S"SET IDENTITY_INSERT ", wrappedTableName, S" OFF"));
		}

		// Execute the entire transaction
		MOG_DBAPI::ExecuteTransaction(transactionCommands);
	}
	catch (Exception *e)
	{
		MOG_Report::ReportMessage(S"Import Database Failed", e->Message, e->StackTrace, MOG_ALERT_LEVEL::ERROR);
		bFailed = true;
	}

	if (!bFailed)
	{
		return true;
	}
	return false;
}

