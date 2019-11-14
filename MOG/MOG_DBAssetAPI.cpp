
//--------------------------------------------------------------------------------
//	MOG_Database.cpp
//	
//	
//--------------------------------------------------------------------------------
#include "stdafx.h"

#include "MOG_Report.h"
#include "MOG_Time.h"
#include "MOG_ControllerProject.h"
#include "MOG_ControllerPackage.h"
#include "MOG_DosUtils.h"
#include "MOG_ControllerAsset.h"
#include "MOG_ControllerSystem.h"
#include "MOG_DBProjectAPI.h"
#include "MOG_DBPostCommandAPI.h"
#include "MOG_DBPackageCommandAPI.h"
#include "MOG_DBReports.h"
#include "MOG_DBInboxAPI.h"
#include "MOG_Tokens.h"
#include "MOG_ControllerRepository.h"

#include "MOG_DBAssetAPI.h"

using namespace System::ComponentModel;
using namespace MOG_CoreControls;


bool MOG_DBAssetAPI::AddAssetVersionsToBranch(ArrayList *listOfPostInfo)
{
	bool bSuccess = false;

	//verify that we actually have a list of post commands that need to be executed.
	if (listOfPostInfo)
	{
		String* message = String::Concat(S"Please wait while the assets are being posted.\n", S"This may take a few minutes.");

		ProgressDialog* progress = new ProgressDialog(S"Adding Asset to project", message, new DoWorkEventHandler(NULL, &AddAssetVersionsToBranch_Worker), listOfPostInfo, true);
		// Set Hidden based on the Prompt mode to prevent slaves from showing this progress dialog
		progress->Hidden = MOG_Prompt::IsMode(MOG_PROMPT_SILENT);
		if (progress->ShowDialog() == DialogResult::OK)
		{
			bSuccess = *dynamic_cast<__box bool*>(progress->WorkerResult);
		}
	}

	return bSuccess;
}

void MOG_DBAssetAPI::AddAssetVersionsToBranch_Worker(Object* sender, DoWorkEventArgs* e)
{
	BackgroundWorker* worker = dynamic_cast<BackgroundWorker*>(sender);
	ArrayList* listOfPostInfo = dynamic_cast<ArrayList*>(e->Argument);

	//create a string to hold the update transaction.
	String *bulkTransaction = NULL;
	//create an array list to hold all the commands we are going to execute in the transaction.
	ArrayList *setOfQueryStatements = new ArrayList();

	MOG_DBGenericCache *previousVersionCache = new MOG_DBGenericCache(MOG_DBQueryBuilderAPI::MOG_DBCacheQueries::PopulateTemporaryCache_PreviousVersion(), S"CACHEID", S"CACHENAME",S"CACHELIST" , false, false);

	//Get the Branch ID
	int branchID = MOG_DBProjectAPI::GetBranchIDByName(MOG_ControllerProject::GetBranchName());

	// This is a fiarly arbatrary number to determine if we pre cache or not.  as we get more info this may be tweeked
	//In testing we esimate that at about 2500 assets there is a good chance we would benefit from precacheing.
	//however if we have only a few assets to post we don't want to take the time to initilize a precache which can take 2 to 3 min on average.
	//this command initiates the precache if we are posting more than 2500 assets.
	if (listOfPostInfo->Count > 2500)
	{
		// Initialize caches
		//AssetNameID
		//AssetVersionID
		//previousVersionID - Branch Links Table
		MOG_ControllerSystem::GetDB()->GetDBCache()->GetAssetNameCache()->PopulateCacheFromSQL(MOG_DBQueryBuilderAPI::MOG_DBCacheQueries::PopulateCache_AssetNameCache(),S"CACHEID", S"CACHENAME");
		previousVersionCache->PopulateCacheFromSQL();
		MOG_ControllerSystem::GetDB()->GetDBCache()->GetAssetVersionCache()->PopulateCacheFromSQL(MOG_DBQueryBuilderAPI::MOG_DBCacheQueries::PopulateCache_AssetVersionCache(), S"CACHEID", S"CACHENAME", S"CACHELIST");
	}

	//itterate through the list of Post Info Objects.
	for( int currentMogFileNameIndex = 0; currentMogFileNameIndex < listOfPostInfo->Count ; currentMogFileNameIndex++ )
	{
		//Cast the current array element to a PostInfo object.
		MOG_DBPostInfo *currentPostInfo    = dynamic_cast<MOG_DBPostInfo*> (listOfPostInfo->Item[currentMogFileNameIndex]);
		//Convert to a MogFile name so we can reuse the code we already have.
		MOG_Filename   *currentMogFileName = new MOG_Filename(currentPostInfo->mAssetName);

		if (worker != NULL)
		{
			worker->ReportProgress(currentMogFileNameIndex * 100 / listOfPostInfo->Count);
		}

		int assetNameID = GetAssetNameID(currentMogFileName);
		if( assetNameID != 0)
		{
			// Make sure we have an AssetRevision?
			int assetVersionID = GetAssetVersionID(currentMogFileName, currentPostInfo->mAssetVersion);
			if( assetVersionID != 0 )
			{
				int previousVersionID = 0;
				if(previousVersionCache->IsPreCached())
				{
					previousVersionID = previousVersionCache->GetIDFromName(Convert::ToString(assetNameID), Convert::ToString(branchID));
				}
				else
				{
					//create a variable which will hold the select command to find out if we already have a branch Link.
					String *selectCmd;

					// check if there is already a branch link for this asset
					selectCmd = String::Concat( S"SELECT AssetVersionID ",
												S"FROM (SELECT DISTINCT * FROM ", MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), S") AS BranchLinks  INNER JOIN ", MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" ON ",
												S"BranchLinks.AssetVersionID = ", MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S".ID ",
												S"WHERE BranchLinks.BranchID=", branchID, S" AND AssetNameID=", assetNameID );
					//Look for a previous version 
					previousVersionID = MOG_DBAPI::QueryInt(selectCmd, "AssetVersionID");
				}

				//If there is already a branch Link update it.
				if( previousVersionID != 0 )
				{
					setOfQueryStatements->Add( MOG_DBAssetAPI::GetAssetVersionsToBranch_UpdateSQL(currentMogFileName, previousVersionID, assetVersionID));
				}
				//if not insert a new one.
				else
				{
					setOfQueryStatements->Add( MOG_DBAssetAPI::GetAssetVersionsToBranch_InsertSQL(currentMogFileName, assetVersionID ) );
				}
			}
		}
	}

	// Execute the entire transaction
	bool bSuccess = MOG_DBAPI::ExecuteTransaction(setOfQueryStatements);

	e->Result = __box(bSuccess);
}

bool MOG_DBAssetAPI::AddAssetVersionToBranch(MOG_Filename *assetFilename, String *versionTimeStamp)
{
	// Get the assetVersionID
	int assetVersionID = MOG_DBAssetAPI::GetAssetVersionID(assetFilename, versionTimeStamp);
	if (assetVersionID != 0)
	{
		// Proceed to add this asset to the database
		String *addCmd = MOG_DBAssetAPI::GetAssetVersionsToBranch_InsertSQL(assetFilename, assetVersionID);
		return MOG_DBAPI::RunNonQuery(addCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
	}

	return false;
}

//This creates a update statment for a particular assest if a branch link already exists.
String *MOG_DBAssetAPI::GetAssetVersionsToBranch_UpdateSQL(MOG_Filename *blessedAssetFilename, int oldVersionID, int newVersionID)
{
	String *updateCmd = String::Concat(S"UPDATE ", MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), S" SET");
	// Variables
	updateCmd = String::Concat(updateCmd, S" AssetVersionID='", Convert::ToString(newVersionID), S"' ");
	// Where clause
	updateCmd = String::Concat(updateCmd, S" WHERE AssetVersionID=", Convert::ToString(oldVersionID), S" AND BranchID=", MOG_DBProjectAPI::GetBranchIDByName(MOG_ControllerProject::GetBranchName()));
	return updateCmd;
}

//This creates a insert statment for an asset without a branch Link.
String *MOG_DBAssetAPI::GetAssetVersionsToBranch_InsertSQL(MOG_Filename *blessedAssetFileName, int assetVersionID)
{
	// doesn't exist...create it
	String *insertCmd = String::Concat(S"INSERT INTO ", MOG_ControllerSystem::GetDB()->GetBranchLinksTable(),
							S" (BranchID, AssetVersionID) VALUES ",
							S" (",MOG_DBProjectAPI::GetBranchIDByName(MOG_ControllerProject::GetBranchName()) , S", " , assetVersionID, S")" );
	return insertCmd;
}

int MOG_DBAssetAPI::CreateAssetName(MOG_Filename *assetFilename)
{
	return CreateAssetName(assetFilename, MOG_ControllerProject::GetUserName_DefaultAdmin(), MOG_Time::GetVersionTimestamp());
}

int MOG_DBAssetAPI::CreateAssetName(MOG_Filename *assetFilename, String* createdBy)
{
	return CreateAssetName(assetFilename, createdBy, MOG_Time::GetVersionTimestamp());
}

int MOG_DBAssetAPI::CreateAssetName(MOG_Filename *assetFilename, String* createdBy, String *createdDate)
{
	// Check if we have no createdBy?
	if (createdBy->Length == 0)
	{
		// Make sure we have something valid
		createdBy = MOG_ControllerProject::GetUserName_DefaultAdmin();
	}

	// Make sure we are missing an AssetNames record?
	int assetNameID = GetAssetNameID(assetFilename);
	if( assetNameID == 0 )
	{
		int classificationID = GetClassificationID(assetFilename->GetAssetClassification());
		if( classificationID == 0 )
		{
			// ensure the classification exists
			CreateClassification(assetFilename->GetAssetClassification(), createdBy, createdDate);

			// try to get ID again
			classificationID = GetClassificationID(assetFilename->GetAssetClassification());
		}

		// Make sure we have a valid classification?
		if (classificationID != 0)
		{
			int createdByID = MOG_DBProjectAPI::GetUserID(createdBy);

			// Make sure the platform matches the one defined in the project for any case sensitive tools
			String *platformName = assetFilename->GetAssetPlatform();
			if (String::Compare(platformName, S"All", true) == 0)
			{
				platformName = S"All";
			}
			else
			{
				// Obtain the platformInfo for this platform from the project
				MOG_Platform *platformInfo = MOG_ControllerProject::GetProject()->GetPlatform(platformName);
				if (platformInfo)
				{
					// Replace the platformName with the one from the project's platformInfo
					platformName = platformInfo->mPlatformName;
				}
				else
				{
					// This might be a good place for an error...but why limit the user
				}
			}

			// Add AssetNames record
			String *insertCmd = String::Concat(S"INSERT INTO ", MOG_ControllerSystem::GetDB()->GetAssetNamesTable(),
											   S" ( AssetClassificationID, AssetPlatformKey, AssetLabel, CreatedBy, CreatedDate, RemovedDate, RemovedBy) VALUES ",
											   S" (",__box(classificationID),S",'",MOG_DBAPI::FixSQLParameterString(platformName), S"','", MOG_DBAPI::FixSQLParameterString(assetFilename->GetAssetLabel()) , S"',",__box(createdByID), S", '", createdDate, S"', '', 0)" );

			MOG_DBAPI::RunNonQuery(insertCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());

			assetNameID = GetAssetNameID(assetFilename);
		}
        else
        {
			// Warn the user about this failed classification
			String *message = String::Concat(S"Unable to create this AssetName becasue we failed to create its new classification.\n",
											 S"Asset: ", assetFilename->GetAssetFullName());
			MOG_Report::ReportMessage(S"CreateAssetName", message, "", MOG_ALERT_LEVEL::ERROR);
        }
	}

	return assetNameID;
}


int MOG_DBAssetAPI::CreateAssetRevision(int assetNameID, String *version, int createdByID, String *createdDate)
{
	int assetVersionID = 0;

	String *selectCmd = String::Concat( S"SELECT ID ",
								S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" ",
								S"WHERE AssetNameID=", assetNameID, S" AND Version='", MOG_DBAPI::FixSQLParameterString(version), S"'" );

	// Make sure this revision doesn't already exist?
	assetVersionID = MOG_DBAPI::QueryInt(selectCmd, S"ID");
	if( assetVersionID == 0 )
	{
		// add AssetVersion record
		String *insertCmd = String::Concat(S"INSERT INTO ", MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(),
								   S" (AssetNameID, Version, CreatedDate, CreatedBy, BlessedDate, BlessedBy) VALUES ",
								   S" (",__box(assetNameID) , S", '", MOG_DBAPI::FixSQLParameterString(version), S"', '",createdDate , S"', ",__box(createdByID) , S", '', 0)" );

			MOG_DBAPI::RunNonQuery(insertCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());

		assetVersionID = MOG_DBAPI::QueryInt(selectCmd, S"ID");
	}

	return assetVersionID;
}

//This replaces one AssetNameID for another one in the AssetVersions table
bool MOG_DBAssetAPI::UpdateRevisionsWithReplacementAssetNameID(int oldAssetNameID, int newAssetNameID)
{
	String *updateCmd = String::Concat(	S" UPDATE ", MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), 
										S" SET AssetNameID='", Convert::ToString(newAssetNameID), S"' "
										S" WHERE AssetNameID=", Convert::ToString(oldAssetNameID));
	return MOG_DBAPI::RunNonQuery(updateCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
}

MOG_Filename *MOG_DBAssetAPI::QueryAsset(String *selectString)
{
	SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());
	MOG_Filename *asset = NULL;

	SqlDataReader *myReader = NULL;
	try
	{
		myReader = MOG_DBAPI::GetNewDataReader(selectString, myConnection);
		while(myReader->Read())
		{
			asset = SQLReadAsset(myReader);
			break;
		}
	}
	catch(Exception *e)
	{
		String *message = String::Concat(	S"Could not get records from SQL database!\n",
											S"MOG SERVER: ", MOG_ControllerSystem::GetServerComputerName());
		MOG_Report::ReportMessage(message, e->Message, e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
		return NULL;
	}
	__finally
	{
		if( myReader != NULL )
		{
			myReader->Close();
		}
		myConnection->Close();
	}
	return asset;
}

ArrayList *MOG_DBAssetAPI::QueryAssets(String *selectString)
{
	SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());
	ArrayList *assets = new ArrayList();

	SqlDataReader *myReader = NULL;
	try
	{
		//Get the reader from the passed select String
		myReader = MOG_DBAPI::GetNewDataReader(selectString, myConnection);
		while(myReader->Read())
		{
			assets->Add(MOG_DBAssetAPI::SQLReadAsset(myReader));
		}
	}
	catch(Exception *e)
	{
		String *message = String::Concat(	S"Could not get records from SQL database!\n",
											S"MOG SERVER: ", MOG_ControllerSystem::GetServerComputerName());
		MOG_Report::ReportMessage(message, e->Message, e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
		return NULL;
	}
	__finally
	{
		if( myReader != NULL )
		{
			myReader->Close();
		}
		myConnection->Close();
	}
	return assets;
}

MOG_Filename *MOG_DBAssetAPI::SQLReadAsset(SqlDataReader *myReader)
{
	//NOTE: for this function to work properly the reader must be using a properly formatted SELECT statement.
	// That means the first 3 elements must be FullTreeName, AssetPlatformKey, and AssetLabel, in that order.
	//We could use SafeStringRead so the order doesn't matter, but it is so slow, it is better just to arrange the elements in the proper order.

	StringBuilder* assetFullname = new StringBuilder(260);
	assetFullname->Append(myReader->GetString(0)->Trim()); //"FullTreeName"
	assetFullname->Append(S"{");
	assetFullname->Append(myReader->GetString(1)->Trim()); //"AssetPlatformKey"
	assetFullname->Append(S"}");
	assetFullname->Append(myReader->GetString(2)->Trim()); //"AssetLabel"

	String *version = "";

	// Check if there might be more to read
	if (myReader->FieldCount > 3)
	{
		try
		{
			// Set our version, if available
			version = myReader->GetString(3)->Trim(); //"Version"
		}
		catch( IndexOutOfRangeException *e )
		{
			e->ToString();
			// If version was not available, set it to empty string
			version = S"";
		}
	}

	return new MOG_Filename(assetFullname->ToString(), version);
}

bool MOG_DBAssetAPI::SQLWriteAsset(MOG_Filename *asset, String *version)
{
	return false;	// JTM-FIXME do we need this?
	String *updateCmd = "UPDATE Assets SET";

	MOG_Time *time = new MOG_Time();

	// Variables
	updateCmd = String::Concat(updateCmd, S" AssetName='", MOG_DBAPI::FixSQLParameterString(asset->GetAssetName()), S"',");
	updateCmd = String::Concat(updateCmd, S" VersionTimeStamp='", MOG_DBAPI::FixSQLParameterString(version), S"',");
	updateCmd = String::Concat(updateCmd, S" AssetClass='", MOG_DBAPI::FixSQLParameterString(asset->GetAssetClassification()), S"',");
	updateCmd = String::Concat(updateCmd, S" AssetPlatformKey='", MOG_DBAPI::FixSQLParameterString(asset->GetAssetPlatform()), S"'");
	updateCmd = String::Concat(updateCmd, S" AssetLabel='", MOG_DBAPI::FixSQLParameterString(asset->GetAssetLabel()), S"',");

	// Where clause
	updateCmd = String::Concat(updateCmd, S" WHERE AssetName='", MOG_DBAPI::FixSQLParameterString(asset->GetAssetName()), S"'");
	
	MOG_DBAPI::RunNonQuery(updateCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
}

bool MOG_DBAssetAPI::SQLCreateAssetProperty(MOG_Filename *assetName, String *version, String *section, String *property, String *value)
{
	//bool created = false;
	int assetVersionID = GetAssetVersionID(assetName, version);
	String *createCmd = NULL;

	// does the asset version exist?
	if( assetVersionID != 0 )
	{
		String *selectCmd = String::Concat( S"SELECT Value ",
											S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetPropertiesTable(),
											S"WHERE (AssetVersionID = ", Convert::ToString(assetVersionID), S") AND ",
												S"(Section = '", MOG_DBAPI::FixSQLParameterString(section), S"') AND ",
												S"(Property = '", MOG_DBAPI::FixSQLParameterString(property), S"')" );

		// does it already exist?
		if( MOG_DBAPI::QueryExists(selectCmd) )
		{
			// just update
			createCmd = String::Concat( S"UPDATE ", MOG_ControllerSystem::GetDB()->GetAssetPropertiesTable(), S" SET ",
										S"Value='", MOG_DBAPI::FixSQLParameterString(value), S"' ",
										S"WHERE (AssetVersionID = ", Convert::ToString(assetVersionID), S") AND ",
											S"(Section = '", MOG_DBAPI::FixSQLParameterString(section), S"') AND ",
											S"(Property = '", MOG_DBAPI::FixSQLParameterString(property), S"')" );

		}
		else
		{
			// add a new record
			createCmd = String::Concat(S"INSERT INTO ", MOG_ControllerSystem::GetDB()->GetAssetPropertiesTable(),
									S" ( AssetVersionID, Section, Property, Value ) VALUES ",
									S" (",__box(assetVersionID),S",'",MOG_DBAPI::FixSQLParameterString(section) ,S"', '",MOG_DBAPI::FixSQLParameterString(property) ,S"', '",MOG_DBAPI::FixSQLParameterString(value) ,S"')" );
		}
			return MOG_DBAPI::RunNonQuery(createCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
	}
	return false;
}

bool MOG_DBAssetAPI::SQLCreateAssetClassificationProperty(String *classification, String *branch, String *section, String *property, String *value)
{
	//bool created = false;
	String *createCmd = NULL;

	// Check if we should assume the branch?
	if (!branch || !branch->Length)
	{
		branch = MOG_ControllerProject::GetBranchName();
	}

	int classificationBranchID = GetClassificationIDForBranch(classification, branch);

	// does the classification exist for branch?
	if( classificationBranchID != 0 )
	{
		String *selectCmd = String::Concat( S"SELECT Value ",
											S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsPropertiesTable(),
											S"WHERE (AssetClassificationBranchLinkID = ", Convert::ToString(classificationBranchID), S") AND ",
												S"(Section = '", MOG_DBAPI::FixSQLParameterString(section), S"') AND ",
												S"(Property = '", MOG_DBAPI::FixSQLParameterString(property), S"')" );

		// does it already exist?
		if( MOG_DBAPI::QueryExists(selectCmd) )
		{
			// just update
			createCmd = String::Concat( S"UPDATE ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsPropertiesTable(), S" SET ",
										S"Value='", MOG_DBAPI::FixSQLParameterString(value), S"' ",
										S"WHERE (AssetClassificationBranchLinkID = ", Convert::ToString(classificationBranchID), S") AND ",
											S"(Section = '", MOG_DBAPI::FixSQLParameterString(section), S"') AND ",
											S"(Property = '", MOG_DBAPI::FixSQLParameterString(property), S"')" );
		}
		else
		{
			// add a new record
			createCmd = String::Concat(S"INSERT INTO ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsPropertiesTable(),
									S" ( AssetClassificationBranchLinkID, Section, [Property], [Value] ) VALUES ",
									S" (",__box(classificationBranchID) ,S",'",MOG_DBAPI::FixSQLParameterString(section) ,S"', '",MOG_DBAPI::FixSQLParameterString(property) ,S"', '",MOG_DBAPI::FixSQLParameterString(value),S"')" );

			//return MOG_DBAPI::RunNonQuery(createCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
		}
		return MOG_DBAPI::RunNonQuery(createCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
	}
	return false;
}

MOG_Property *MOG_DBAssetAPI::SQLReadProperty(SqlDataReader *myReader)
{
	MOG_Property *prop = new MOG_Property();

	prop->SetSection(myReader->GetString(myReader->GetOrdinal(S"Section"))->Trim());
	prop->SetKey(myReader->GetString(myReader->GetOrdinal(S"Property"))->Trim());
	prop->SetValue(myReader->GetString(myReader->GetOrdinal(S"Value"))->Trim());
		
	return prop;
}

ArrayList *MOG_DBAssetAPI::QueryProperties(String *selectString)
{
	SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());
	ArrayList *props = new ArrayList();

	SqlDataReader *myReader = NULL;

	try
	{
		myReader = MOG_DBAPI::GetNewDataReader(selectString, myConnection);
	
		while(myReader->Read())
		{
			props->Add(SQLReadProperty(myReader));
		}
	}
	catch(Exception *e)
	{
		String *message = String::Concat(	S"Could not get records from SQL database!\n",
											S"MOG SERVER: ", MOG_ControllerSystem::GetServerComputerName());
		MOG_Report::ReportMessage(message, e->Message, e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
		return NULL;
	}
	__finally
	{
		if( myReader != NULL )
		{
			myReader->Close();
		}
		myConnection->Close();
	}

	return props;
}

MOG_Property *MOG_DBAssetAPI::QueryProperty(String *selectString)
{
	SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());
	MOG_Property *prop = NULL;

	SqlDataReader *myReader = NULL;

	try
	{
		myReader = MOG_DBAPI::GetNewDataReader(selectString, myConnection);
		while(myReader->Read())
		{
			prop = SQLReadProperty(myReader);
			break;
		}
	}
	catch(Exception *e)
	{
		String *message = String::Concat(	S"Could not get records from SQL database!\n",
											S"MOG SERVER: ", MOG_ControllerSystem::GetServerComputerName());
		MOG_Report::ReportMessage(message, e->Message, e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
		return NULL;
	}
	__finally
	{
		if( myReader != NULL )
		{
			myReader->Close();
		}
		myConnection->Close();
	}

	return prop;
}

void MOG_DBAssetAPI::GetAllClassificationChildrenIDs(String *parentID, ArrayList *children)
{
	if( children == NULL )
	{
		return;
	}

	String *selectCmd = String::Concat( S"SELECT AssetClassifications.ID ",
										S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ",
										S"WHERE (AssetClassifications.ParentID = ", parentID, S")" );

	ArrayList *childResults = MOG_DBAPI::QueryStrings(selectCmd, S"ID");
	for( int i = 0; i < childResults->Count; i++ )
	{
		GetAllClassificationChildrenIDs(__try_cast<String*>(childResults->Item[i]), children);
		children->Add(childResults->Item[i]);
	}
}

void MOG_DBAssetAPI::GetAllClassificationChildrenIDsInBranch(String *parentID, String *branchID, ArrayList *children)
{
	if( children == NULL )
	{
		return;
	}

	String *selectCmd = String::Concat( S"SELECT AssetClassifications.ID ",
										S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetAssetClassificationsBranchLinksTable(), S" AssetClassificationsBranchLinks ON ",
											S"AssetClassifications.ID = AssetClassificationsBranchLinks.ClassificationID ",
										S"WHERE (AssetClassificationsBranchLinks.BranchID = ", branchID, S") AND ",
											S"(AssetClassifications.ParentID = ", parentID, S")" );

	ArrayList *childResults = MOG_DBAPI::QueryStrings(selectCmd, S"ID");
	for( int i = 0; i < childResults->Count; i++ )
	{
		GetAllClassificationChildrenIDsInBranch(__try_cast<String*>(childResults->Item[i]), branchID, children);
		children->Add(childResults->Item[i]);
	}
}

// Returns an ArrayList of MOG_Filenames
ArrayList *MOG_DBAssetAPI::GetAllAssetsByClassification(String *classification)
{
	//use the Query builder to create the query for this function.
	String *selectString = MOG_DBQueryBuilderAPI::MOG_AssetQueries::Query_AllAssetsForBranchByClassification(classification);

	return QueryAssets(selectString);
}

ArrayList *MOG_DBAssetAPI::GetAllCurrentAssetsByClassification(String *classification, bool bExcludeLibrary)
{
	return GetAllCurrentAssetsByClassification(classification, bExcludeLibrary, MOG_ControllerProject::GetBranchName());
}

ArrayList *MOG_DBAssetAPI::GetAllCurrentAssetsByClassification(String *classification, bool bExcludeLibrary, String *branchName)
{
	//use the Query Builder to create the query for this function.
	String *selectString = MOG_DBQueryBuilderAPI::MOG_AssetQueries::Query_AllCurrentAssetsForBranchByClassificationTree(classification, bExcludeLibrary, branchName);
	return QueryAssets(selectString);
}

ArrayList *MOG_DBAssetAPI::GetAllAssetsInClassificationTree(String *classification)
{
	String *selectString = String::Concat( S"SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ",
										   S"FROM (SELECT DISTINCT * FROM ", MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), S") AS BranchLinks INNER JOIN ",
												MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions ON BranchLinks.AssetVersionID = AssetVersions.ID INNER JOIN ",
												MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches ON BranchLinks.BranchID = Branches.ID INNER JOIN ",
												MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames ON AssetVersions.AssetNameID = AssetNames.ID INNER JOIN ",
												MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID ",
										   S"WHERE (Branches.BranchName = '", MOG_DBAPI::FixSQLParameterString(MOG_ControllerProject::GetBranchName()), S"') AND ",
												S"(AssetClassifications.FullTreeName LIKE '", MOG_DBAPI::FixSQLParameterString(classification), S"%') ORDER BY AssetNames.AssetLabel" );

	return QueryAssets(selectString);
}

int MOG_DBAssetAPI::GetDependantAssetCount(String *rootClassification)
{
	String *selectString = String::Concat( S"SELECT COUNT(AssetClassifications.ID) AS AssetCount ",
										   S"FROM ", MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), S" AS BranchLinks INNER JOIN ",
												MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" AS Branches ON BranchLinks.BranchID = Branches.ID INNER JOIN ",
												MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AS AssetClassifications INNER JOIN ",
												MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AS AssetNames ON AssetClassifications.ID = AssetNames.AssetClassificationID INNER JOIN ",
												MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AS AssetVersions ON AssetNames.ID = AssetVersions.AssetNameID ON ",
												S"BranchLinks.AssetVersionID = AssetVersions.ID ",
										   S"WHERE (Branches.BranchName = '", MOG_DBAPI::FixSQLParameterString(MOG_ControllerProject::GetBranchName()), S"') AND ",
												S"(AssetClassifications.FullTreeName = '", MOG_DBAPI::FixSQLParameterString(rootClassification), S"' OR AssetClassifications.FullTreeName LIKE '", MOG_DBAPI::FixSQLParameterString(rootClassification), S"~%')" );

	return MOG_DBAPI::QueryInt(selectString, S"AssetCount");
}

ArrayList *MOG_DBAssetAPI::GetAllAssetsByClassification(String *classification, MOG_Property *property)
{

	String *selectString = String::Concat(
		S"SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ",
		S"FROM (SELECT DISTINCT * FROM ", MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), S") AS BranchLinks INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions ON BranchLinks.AssetVersionID = AssetVersions.ID INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches ON BranchLinks.BranchID = Branches.ID INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames ON AssetVersions.AssetNameID = AssetNames.ID INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetPropertiesTable(), S" AssetProperties ON AssetVersions.AssetNameID = AssetProperties.AssetVersionID ",
		S"WHERE (Branches.BranchName = '", MOG_DBAPI::FixSQLParameterString(MOG_ControllerProject::GetBranchName()), S"') AND ",
			S"(AssetClassifications.FullTreeName = '", MOG_DBAPI::FixSQLParameterString(classification), S"') AND ",
			S"(AssetProperties.Section = '", MOG_DBAPI::FixSQLParameterString(property->mSection), S"') AND ",
			S"(AssetProperties.Property = '", MOG_DBAPI::FixSQLParameterString(property->mKey), S"') AND ",
			S"(AssetProperties.Value = '", MOG_DBAPI::FixSQLParameterString(property->mPropertyValue), S"') AND ",
			S"(AssetNames.RemovedDate = '')" );

	return QueryAssets(selectString);
}

ArrayList *MOG_DBAssetAPI::GetAllAssetsByClassificationAndProperties(String *classification, ArrayList *properties, bool bIncludeSubClassifications)
{
	ArrayList *finalAssetList = new ArrayList();
	ArrayList *propertyAssetLists = new ArrayList();
	
	String *branchName = MOG_ControllerProject::GetBranchName();
	String *classificationSQL;
	if( bIncludeSubClassifications )
	{
		classificationSQL = String::Concat( S"LIKE '", MOG_DBAPI::FixSQLParameterString(classification), S"%'" );
	}
	else
	{
		classificationSQL = String::Concat( S"= '", MOG_DBAPI::FixSQLParameterString(classification), S"'" );
	}

	// generate an asset list for each property
	for( int i = 0; i < properties->Count; i++ )
	{
		Hashtable *classificationsHash = new Hashtable();
		Hashtable *assetHash = new Hashtable();

		// get the current property
		MOG_Property *property = __try_cast<MOG_Property*>(properties->Item[i]);

		// first - get all the assets that have set the property directly
		String *selectString = String::Concat(
			S"SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ",
			S"FROM (SELECT DISTINCT * FROM ", MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), S") AS BranchLinks INNER JOIN ",
				MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions ON BranchLinks.AssetVersionID = AssetVersions.ID INNER JOIN ",
				MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches ON BranchLinks.BranchID = Branches.ID INNER JOIN ",
				MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames ON AssetVersions.AssetNameID = AssetNames.ID INNER JOIN ",
				MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID INNER JOIN ",
				MOG_ControllerSystem::GetDB()->GetAssetPropertiesTable(), S" AssetProperties ON AssetVersions.ID = AssetProperties.AssetVersionID ",
			S"WHERE (Branches.BranchName = '", MOG_DBAPI::FixSQLParameterString(branchName), S"') AND ",
				S"(AssetClassifications.FullTreeName ", classificationSQL, S") AND ");

		// Check what parts of the property should be checked?
		if (property->mSection->Length)
		{
			String* value = MOG_DBAssetAPI::PrepareValueForSQLComparison(property->mSection);
			String* comparison = GetProperSQLComparisonString(value, false);
			selectString = String::Concat(	selectString,	S"(AssetProperties.Section ", comparison, S" '", MOG_DBAPI::FixSQLParameterString(value), S"') AND ");
		}
		if (property->mKey->Length)
		{
			String* value = MOG_DBAssetAPI::PrepareValueForSQLComparison(property->mKey);
			String* comparison = GetProperSQLComparisonString(value, false);
			selectString = String::Concat(	selectString,	S"(AssetProperties.Property ", comparison, S" '", MOG_DBAPI::FixSQLParameterString(value), S"') AND ");
		}
		if (property->mValue->Length)
		{
			String* value = MOG_DBAssetAPI::PrepareValueForSQLComparison(property->mValue);
			String* comparison = GetProperSQLComparisonString(value, false);
			selectString = String::Concat(	selectString,	S"(AssetProperties.Value ", comparison, S" '", MOG_DBAPI::FixSQLParameterString(value), S"') AND ");
		}

		selectString = String::Concat(	selectString,	S"(AssetNames.RemovedDate = '')" );

		ArrayList *propAssets = QueryAssets(selectString);

		// Add all matching assets to hashtable
		for(int z = 0; z < propAssets->Count; z++)
		{
			if (!assetHash->Contains( propAssets->Item[z] ))
			{
				assetHash->Add(propAssets->Item[z], NULL);
			}
		}

		// second - get all classifications that match the property
		ArrayList *resultClass = GetAllActiveClassifications( property );
		for( int z = 0; z < resultClass->Count; z++ )
		{
			// if the Value is set then that means a matching classification
			classificationsHash->Add( resultClass->Item[z], resultClass->Item[z] );
		}

		// third - get all classifications that dont match the property
		selectString = String::Concat(
			S"SELECT AssetClassifications.FullTreeName ",
			S"FROM ", MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches INNER JOIN ",
				MOG_ControllerSystem::GetDB()->GetAssetClassificationsBranchLinksTable(), S" ACBL ON Branches.ID = ACBL.BranchID INNER JOIN ",
				MOG_ControllerSystem::GetDB()->GetAssetClassificationsPropertiesTable(), S" ACP ON ACBL.ID = ACP.AssetClassificationBranchLinkID INNER JOIN ",
				MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON ACBL.ClassificationID = AssetClassifications.ID ",
			S"WHERE (Branches.BranchName = '", MOG_DBAPI::FixSQLParameterString(branchName), S"') AND ",
				S"(ACP.Section = '", MOG_DBAPI::FixSQLParameterString(property->mSection), S"') AND ",
				S"(ACP.Property = '", MOG_DBAPI::FixSQLParameterString(property->mKey), S"') AND ",
				S"(ACP.Value <> '", MOG_DBAPI::FixSQLParameterString(property->mPropertyValue), S"')" );
		
		resultClass = MOG_DBAPI::QueryStrings( selectString, S"FullTreeName" );
		for( int z = 0; z < resultClass->Count; z++ )
		{
			if (classificationsHash->Contains(resultClass->Item[z]) == false)
			{
				// if the Value is NULL then that means an unmatching classification
				classificationsHash->Add( resultClass->Item[z], NULL );
			}
		}

		// fourth - get all assets that dont have that property at all
// JohnRen - Dang, this is so freaking slow!!!
// Why do all this in the query when we can build this list faster on our side using propAssets that was obtained above
//		selectString = String::Concat(
//			S"SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ",
//			S"FROM (SELECT DISTINCT * FROM ", MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), S") AS BranchLinks INNER JOIN ",
//				MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions ON BranchLinks.AssetVersionID = AssetVersions.ID INNER JOIN ",
//				MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches ON BranchLinks.BranchID = Branches.ID INNER JOIN ",
//				MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames ON AssetVersions.AssetNameID = AssetNames.ID INNER JOIN ",
//				MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID LEFT OUTER JOIN ",
//				S"(SELECT * ",
//					S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetPropertiesTable(), S" prop ",
//					S"WHERE prop.Section = '", MOG_DBAPI::FixSQLParameterString(property->mSection), S"' AND prop.Property = '", MOG_DBAPI::FixSQLParameterString(property->mKey), S"') Property ON AssetVersions.ID = Property.AssetVersionID ",
//			S"WHERE (Branches.BranchName = '", MOG_DBAPI::FixSQLParameterString(branchName), S"') AND ",
//				S"(AssetClassifications.FullTreeName ", classificationSQL, S") AND ",
//				S"(Property.Value IS NULL) AND ",
//				S"(AssetNames.RemovedDate = '') ",
//			S"ORDER BY AssetClassifications.FullTreeName" );
//
//		ArrayList *unsetAssets = QueryAssets(selectString);
		// Get all of the current assets in the project
		HybridDictionary* allAssets = MOG_LocalSyncInfo::ConvertFromArrayListToHybridDictionary(MOG_DBAssetAPI::GetAllAssets(classification, true));
		// Exclude the assets in propAssets
		for(int a = 0; a < propAssets->Count; a++)
		{
			MOG_Filename *assetFilename = __try_cast<MOG_Filename*>(propAssets->Item[a]);
		}
		ArrayList *unsetAssets = MOG_LocalSyncInfo::ConvertFromHybridDictionaryToArrayList(allAssets);

		// loop over assets and find those that should be added to the property list
		for( int z = 0; z < unsetAssets->Count; z++ )
		{
			MOG_Filename *assetFilename = __try_cast<MOG_Filename*>(unsetAssets->Item[z]);
			// if the asset inherits the property then add it to the hash
			if( DoesClassificationMatch( assetFilename->GetAssetClassification(), classificationsHash ) )
			{
				if (!assetHash->Contains( assetFilename ))
				{
					assetHash->Add(assetFilename, NULL);
				}
				else
				{
					String *message = String::Concat(S"Two or more duplicate asset files are named the same in your project!\n\n", 
							S"ASSETNAME: ", assetFilename->GetAssetFullName(), S"\n",
							S"\nRemove one from the project trees to fix this conflict! Until this is fixed this asset will not be shown in your asset tree.");

					MOG_Report::ReportMessage("Directory Set", message, Environment::StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
				}
			}
		}

		// add hash to property asset list array
		propertyAssetLists->Add(assetHash);
	}

	// find intersection of propertyArrayLists
	if( propertyAssetLists->Count > 0 )
	{
		Hashtable *hashTables[] = new Hashtable*[propertyAssetLists->Count];
		for( int z = 0; z < propertyAssetLists->Count; z++ )
		{
			hashTables[z] = __try_cast<Hashtable*>(propertyAssetLists->Item[z]);
		}

		// loop over first table and cross check with all other tables
		// a good optimization would be to pick the smallest table to loop over
		IDictionaryEnumerator *enumerator = hashTables[0]->GetEnumerator();

		while( enumerator->MoveNext() )
		{
			bool good = true;

			for( int z = 1; z < propertyAssetLists->Count && good; z++ )
			{
				if( !hashTables[z]->Contains(enumerator->Key) )
				{
					// if any of the other tables dont contain the key then it doesnt belong in final list
					good = false;
				}
			}

			if( good )
			{
				finalAssetList->Add(enumerator->Key);
			}
		}
	}

	return finalAssetList;
}

ArrayList *MOG_DBAssetAPI::GetAllArchivedAssetsByClassification(String *classification)
{
//	String *selectString = String::Concat(
//		S"SELECT * ",
//		S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" INNER JOIN ",
//			MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" ON ", MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S".AssetClassificationID = ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S".ID ",
//			S"WHERE (", MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(),S".FullTreeName = '", MOG_DBAPI::FixSQLParameterString(classification), S"')" );

//SELECT     [SimplePackaging.AssetNames].AssetPlatformKey, [SimplePackaging.AssetNames].AssetLabel, [SimplePackaging.AssetClassifications].FullTreeName, 
//                      [SimplePackaging.AssetVersions].Version
//FROM         [SimplePackaging.AssetNames] INNER JOIN
//                      [SimplePackaging.AssetClassifications] ON 
//                      [SimplePackaging.AssetNames].AssetClassificationID = [SimplePackaging.AssetClassifications].ID INNER JOIN
//                      [SimplePackaging.AssetVersions] ON [SimplePackaging.AssetNames].ID = [SimplePackaging.AssetVersions].AssetNameID
//WHERE     ([SimplePackaging.AssetClassifications].FullTreeName = 'SimplePackaging~Packages')	String *selectString = String::Concat(
	String *selectString = String::Concat(
		S"SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ",
		S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions ON AssetNames.ID = AssetVersions.AssetNameID ",
		S"WHERE (AssetClassifications.FullTreeName = '", MOG_DBAPI::FixSQLParameterString(classification), S"')" );

	return QueryAssets(selectString);
}

ArrayList *MOG_DBAssetAPI::GetAllArchivedAssetsByClassificationAndProperties(String *classification, ArrayList *properties, bool bIncludeSubClassifications)
{
	ArrayList *finalAssetList = new ArrayList();
	ArrayList *propertyAssetLists = new ArrayList();
	
	String *classificationSQL;
	if( bIncludeSubClassifications )
	{
		classificationSQL = String::Concat( S"LIKE '", MOG_DBAPI::FixSQLParameterString(classification), S"%'" );
	}
	else
	{
		classificationSQL = String::Concat( S"= '", MOG_DBAPI::FixSQLParameterString(classification), S"'" );
	}

	// generate an asset list for each property
	for( int i = 0; i < properties->Count; i++ )
	{
		Hashtable *classificationsHash = new Hashtable();
		Hashtable *assetHash = new Hashtable();

		// get the current property
		MOG_Property *property = __try_cast<MOG_Property*>(properties->Item[i]);

		// first - get all the assets that have set the property directly
		String *selectString = String::Concat(
			S"SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ",
			S"FROM (SELECT DISTINCT * FROM ", MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S") AssetVersions INNER JOIN ",
				MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames ON AssetVersions.AssetNameID = AssetNames.ID INNER JOIN ",
				MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID INNER JOIN ",
				MOG_ControllerSystem::GetDB()->GetAssetPropertiesTable(), S" AssetProperties ON AssetVersions.ID = AssetProperties.AssetVersionID ",
			S"WHERE (AssetClassifications.FullTreeName ", classificationSQL, S") AND ");

		// Check what parts of the property should be checked?
		if (property->mSection->Length)
		{
			String* value = MOG_DBAssetAPI::PrepareValueForSQLComparison(property->mSection);
			String* comparison = GetProperSQLComparisonString(value, false);
			selectString = String::Concat(	selectString,	S"(AssetProperties.Section ", comparison, S" '", MOG_DBAPI::FixSQLParameterString(value), S"') AND ");
		}
		if (property->mKey->Length)
		{
			String* value = MOG_DBAssetAPI::PrepareValueForSQLComparison(property->mKey);
			String* comparison = GetProperSQLComparisonString(value, false);
			selectString = String::Concat(	selectString,	S"(AssetProperties.Property ", comparison, S" '", MOG_DBAPI::FixSQLParameterString(value), S"') AND ");
		}
		if (property->mValue->Length)
		{
			String* value = MOG_DBAssetAPI::PrepareValueForSQLComparison(property->mValue);
			String* comparison = GetProperSQLComparisonString(value, false);
			selectString = String::Concat(	selectString,	S"(AssetProperties.Value ", comparison, S" '", MOG_DBAPI::FixSQLParameterString(value), S"') AND ");
		}

		selectString = String::Concat(	selectString,	S"(AssetNames.RemovedDate = '')" );

		ArrayList *propAssets = QueryAssets(selectString);

		// Add all matching assets to hashtable
		for(int z = 0; z < propAssets->Count; z++)
		{
			if (!assetHash->Contains( propAssets->Item[z] ))
			{
				assetHash->Add(propAssets->Item[z], NULL);
			}
		}

		// second - get all classifications that match the property
		ArrayList *resultClass = GetAllActiveClassifications( property );
		for( int z = 0; z < resultClass->Count; z++ )
		{
			// if the Value is set then that means a matching classification
			classificationsHash->Add( resultClass->Item[z], resultClass->Item[z] );
		}

		// third - get all classifications that dont match the property
		selectString = String::Concat(
			S"SELECT AssetClassifications.FullTreeName ",
			S"FROM ", MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches INNER JOIN ",
				MOG_ControllerSystem::GetDB()->GetAssetClassificationsBranchLinksTable(), S" ACBL ON Branches.ID = ACBL.BranchID INNER JOIN ",
				MOG_ControllerSystem::GetDB()->GetAssetClassificationsPropertiesTable(), S" ACP ON ACBL.ID = ACP.AssetClassificationBranchLinkID INNER JOIN ",
				MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON ACBL.ClassificationID = AssetClassifications.ID ",
			S"WHERE (ACP.Section = '", MOG_DBAPI::FixSQLParameterString(property->mSection), S"') AND ",
				S"(ACP.Property = '", MOG_DBAPI::FixSQLParameterString(property->mKey), S"') AND ",
				S"(ACP.Value <> '", MOG_DBAPI::FixSQLParameterString(property->mPropertyValue), S"')" );
		
		resultClass = MOG_DBAPI::QueryStrings( selectString, S"FullTreeName" );
		for( int z = 0; z < resultClass->Count; z++ )
		{
			if (classificationsHash->Contains(resultClass->Item[z]) == false)
			{
				// if the Value is NULL then that means an unmatching classification
				classificationsHash->Add( resultClass->Item[z], NULL );
			}
		}

		// fourth - get all assets that dont have that property at all
// JohnRen - Dang, this is so freaking slow!!!
// Why do all this in the query when we can build this list faster on our side using propAssets that was obtained above
//		selectString = String::Concat(
//			S"SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ",
//			S"FROM (SELECT DISTINCT * FROM ", MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), S") AS BranchLinks INNER JOIN ",
//				MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions ON BranchLinks.AssetVersionID = AssetVersions.ID INNER JOIN ",
//				MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches ON BranchLinks.BranchID = Branches.ID INNER JOIN ",
//				MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames ON AssetVersions.AssetNameID = AssetNames.ID INNER JOIN ",
//				MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID LEFT OUTER JOIN ",
//				S"(SELECT * ",
//					S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetPropertiesTable(), S" prop ",
//					S"WHERE prop.Section = '", MOG_DBAPI::FixSQLParameterString(property->mSection), S"' AND prop.Property = '", MOG_DBAPI::FixSQLParameterString(property->mKey), S"') Property ON AssetVersions.ID = Property.AssetVersionID ",
//			S"WHERE (Branches.BranchName = '", MOG_DBAPI::FixSQLParameterString(branchName), S"') AND ",
//				S"(AssetClassifications.FullTreeName ", classificationSQL, S") AND ",
//				S"(Property.Value IS NULL) AND ",
//				S"(AssetNames.RemovedDate = '') ",
//			S"ORDER BY AssetClassifications.FullTreeName" );
//
//		ArrayList *unsetAssets = QueryAssets(selectString);
		// Get all of the current assets in the project
		HybridDictionary* allAssets = MOG_LocalSyncInfo::ConvertFromArrayListToHybridDictionary(MOG_DBAssetAPI::GetAllAssets(classification, true));
		// Exclude the assets in propAssets
		for(int a = 0; a < propAssets->Count; a++)
		{
			MOG_Filename *assetFilename = __try_cast<MOG_Filename*>(propAssets->Item[a]);
		}
		ArrayList *unsetAssets = MOG_LocalSyncInfo::ConvertFromHybridDictionaryToArrayList(allAssets);

		// loop over assets and find those that should be added to the property list
		for( int z = 0; z < unsetAssets->Count; z++ )
		{
			MOG_Filename *assetFilename = __try_cast<MOG_Filename*>(unsetAssets->Item[z]);
			// if the asset inherits the property then add it to the hash
			if( DoesClassificationMatch( assetFilename->GetAssetClassification(), classificationsHash ) )
			{
				if (!assetHash->Contains( assetFilename ))
				{
					assetHash->Add(assetFilename, NULL);
				}
				else
				{
					String *message = String::Concat(S"Two or more duplicate asset files are named the same in your project!\n\n", 
							S"ASSETNAME: ", assetFilename->GetAssetFullName(), S"\n",
							S"\nRemove one from the project trees to fix this conflict! Until this is fixed this asset will not be shown in your asset tree.");

					MOG_Report::ReportMessage("Directory Set", message, Environment::StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
				}
			}
		}

		// add hash to property asset list array
		propertyAssetLists->Add(assetHash);
	}

	// find intersection of propertyArrayLists
	if( propertyAssetLists->Count > 0 )
	{
		Hashtable *hashTables[] = new Hashtable*[propertyAssetLists->Count];
		for( int z = 0; z < propertyAssetLists->Count; z++ )
		{
			hashTables[z] = __try_cast<Hashtable*>(propertyAssetLists->Item[z]);
		}

		// loop over first table and cross check with all other tables
		// a good optimization would be to pick the smallest table to loop over
		IDictionaryEnumerator *enumerator = hashTables[0]->GetEnumerator();

		while( enumerator->MoveNext() )
		{
			bool good = true;

			for( int z = 1; z < propertyAssetLists->Count && good; z++ )
			{
				if( !hashTables[z]->Contains(enumerator->Key) )
				{
					// if any of the other tables dont contain the key then it doesnt belong in final list
					good = false;
				}
			}

			if( good )
			{
				finalAssetList->Add(enumerator->Key);
			}
		}
	}

	return finalAssetList;
}

ArrayList *MOG_DBAssetAPI::GetAllArchivedAssetsByParentClassification(String *classification)
{
	String *selectString = String::Concat(
		S"SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ",
		S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions ON AssetNames.ID = AssetVersions.AssetNameID ",
		S"WHERE (AssetClassifications.FullTreeName LIKE '", MOG_DBAPI::FixSQLParameterString(classification), S"%')" );

	return QueryAssets(selectString);
}

ArrayList *MOG_DBAssetAPI::GetAllAssetsByParentClassification(String *classification)
{
	String *selectString = String::Concat(
		S"SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ",
		S"FROM ", MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches INNER JOIN (SELECT DISTINCT * FROM ",
			MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), S") AS BranchLinks ON Branches.ID = BranchLinks.BranchID INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions ON BranchLinks.AssetVersionID = AssetVersions.ID INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames ON AssetVersions.AssetNameID = AssetNames.ID INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID INNER JOIN ",
			S"(SELECT ID ",
				S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AC ",
				S"WHERE (FullTreeName LIKE '", MOG_DBAPI::FixSQLParameterString(classification), S"%')) ACT ON AssetNames.AssetClassificationID = ACT.ID ",
		S"WHERE (AssetNames.RemovedDate = '') AND (Branches.BranchName = '", MOG_DBAPI::FixSQLParameterString(MOG_ControllerProject::GetBranchName()), S"')" );

	return QueryAssets(selectString);
}

ArrayList *MOG_DBAssetAPI::GetAllAssetsByParentClassificationWithProperties(String *classification, ArrayList *properties)
{
	String *selectCmd = S"SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ";

	// add all the prop values we want
	for(int i = 0; i < properties->Count; i++)
	{
		String *num = Convert::ToString(i);
		selectCmd = String::Concat( selectCmd, S", Prop", num, S".Value AS PropValue", num, S" ");
	}

	// add FROM clause
	selectCmd = String::Concat( selectCmd,
		S"FROM ", MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches INNER JOIN (SELECT DISTINCT * FROM ",
			MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), S") AS BranchLinks ON Branches.ID = BranchLinks.BranchID INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions ON BranchLinks.AssetVersionID = AssetVersions.ID INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames ON AssetVersions.AssetNameID = AssetNames.ID INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID INNER JOIN ",
			S"(SELECT ID ",
				S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AC ",
				S"WHERE (FullTreeName LIKE '", MOG_DBAPI::FixSQLParameterString(classification), S"%')) ACT ON AssetNames.AssetClassificationID = ACT.ID " );

	// add all the prop tables
	for(int i = 0; i < properties->Count; i++)
	{
		String *num = Convert::ToString(i);
		MOG_Property *theProp = __try_cast<MOG_Property *>(properties->Item[i]);

		selectCmd = String::Concat( selectCmd, 
			S"LEFT OUTER JOIN ",
				S"(SELECT * ",
					S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetPropertiesTable(), S" prop ",
					S"WHERE (prop.Section = '", MOG_DBAPI::FixSQLParameterString(theProp->mSection), S"') AND ",
					S"(prop.Property = '", MOG_DBAPI::FixSQLParameterString(theProp->mKey), S"')) Prop", num, S" ON AssetVersions.ID = Prop", num, S".AssetVersionID " );
	}

	// add WHERE clause
	selectCmd  = String::Concat(selectCmd, S"WHERE (AssetNames.RemovedDate = '') AND (Branches.BranchName = '", MOG_DBAPI::FixSQLParameterString(MOG_ControllerProject::GetBranchName()), S"')" );

	return QueryAssetsWithProperties(selectCmd, properties);
}

ArrayList *MOG_DBAssetAPI::GetAllAssetsByParentClassificationWithProperties_ThatInheritProperty(String *classification, MOG_Property *property)
{
	ArrayList *properties = new ArrayList();
	properties->Add(property);
	return GetAssetsWithProperties(MOG_DBQueryBuilderAPI::MOG_AssetQueries::Query_AssetsWithPropertyValue(classification, property), properties);
}

ArrayList *MOG_DBAssetAPI::GetAllArchivedAssetsByParentClassificationWithProperties(String *classification, ArrayList *properties)
{
	String *selectCmd = S"SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ";

	// add all the prop values we want
	for(int i = 0; i < properties->Count; i++)
	{
		String *num = Convert::ToString(i);
		selectCmd = String::Concat( selectCmd, S", Prop", num, S".Value AS PropValue", num, S" ");
	}

	// add FROM clause
	selectCmd = String::Concat( selectCmd,
		S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions ON AssetNames.ID = AssetVersions.AssetNameID INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID ");

	// add all the prop tables
	for(int i = 0; i < properties->Count; i++)
	{
		String *num = Convert::ToString(i);
		MOG_Property *theProp = __try_cast<MOG_Property *>(properties->Item[i]);

		selectCmd = String::Concat( selectCmd, 
			S"LEFT OUTER JOIN ",
				S"(SELECT * ",
					S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetPropertiesTable(), S" prop ",
					S"WHERE (prop.Section = '", MOG_DBAPI::FixSQLParameterString(theProp->mSection), S"') AND ",
					S"(prop.Property = '", MOG_DBAPI::FixSQLParameterString(theProp->mKey), S"')) Prop", num, S" ON AssetVersions.ID = Prop", num, S".AssetVersionID " );
	}

	// add WHERE clause
	selectCmd  = String::Concat(selectCmd, S"WHERE (AssetClassifications.FullTreeName LIKE '", MOG_DBAPI::FixSQLParameterString(classification), S"%')" );

	return QueryAssetsWithProperties(selectCmd, properties);
}

ArrayList *MOG_DBAssetAPI::GetAllArchivedAssetVersionsByAssetNameWithProperties(String *assetName, ArrayList *properties)
{
	String *selectCmd = S"SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ";

	// add all the prop values we want
	for(int i = 0; i < properties->Count; i++)
	{
		String *num = Convert::ToString(i);
		selectCmd = String::Concat( selectCmd, S", Prop", num, S".Value AS PropValue", num, S" ");
	}

	// add FROM clause
	selectCmd = String::Concat( selectCmd,
		S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions ON AssetNames.ID = AssetVersions.AssetNameID INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID " );

	// add all the prop tables
	for(int i = 0; i < properties->Count; i++)
	{
		String *num = Convert::ToString(i);
		MOG_Property *theProp = __try_cast<MOG_Property *>(properties->Item[i]);

		selectCmd = String::Concat( selectCmd, 
			S"LEFT OUTER JOIN ",
				S"(SELECT * ",
					S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetPropertiesTable(), S" prop ",
					S"WHERE (prop.Section = '", MOG_DBAPI::FixSQLParameterString(theProp->mSection), S"') AND ",
					S"(prop.Property = '", MOG_DBAPI::FixSQLParameterString(theProp->mKey), S"')) Prop", num, S" ON AssetVersions.ID = Prop", num, S".AssetVersionID " );
	}

	// add WHERE clause
	selectCmd  = String::Concat(selectCmd, S"WHERE (AssetNames.AssetLabel = '", MOG_DBAPI::FixSQLParameterString(assetName), S"')" );

	return QueryAssetsWithProperties(selectCmd, properties);
}

ArrayList *MOG_DBAssetAPI::GetAllAssetsByLabel(String *label)
{
	String *selectString = String::Concat( S"SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ",
										   S"FROM (SELECT DISTINCT * FROM ", MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), S") AS BranchLinks INNER JOIN ",
												MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions ON BranchLinks.AssetVersionID = AssetVersions.ID INNER JOIN ",
												MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches ON BranchLinks.BranchID = Branches.ID INNER JOIN ",
												MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames ON AssetVersions.AssetNameID = AssetNames.ID INNER JOIN ",
												MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID ",
										   S"WHERE (Branches.BranchName = '", MOG_DBAPI::FixSQLParameterString(MOG_ControllerProject::GetBranchName()), S"') AND ",
										   S"(AssetNames.AssetLabel = '", MOG_DBAPI::FixSQLParameterString(label), S"')" );

	return QueryAssets(selectString);
}


ArrayList *MOG_DBAssetAPI::GetAllAssetsLikeLabel(String *label)
{
	// Check if this label doesn't contains a '*'?
	if (!label->Contains("*"))
	{
		// Add a default '*' at the end of this label
		label = String::Concat(label, S"*");
	}

	label = MOG_DBAssetAPI::PrepareValueForSQLComparison(label);
	String* comparison = GetProperSQLComparisonString(label, false);

	String *selectString = String::Concat( S"SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ",
										   S"FROM (SELECT DISTINCT * FROM ", MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), S") AS BranchLinks INNER JOIN ",
												MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions ON BranchLinks.AssetVersionID = AssetVersions.ID INNER JOIN ",
												MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches ON BranchLinks.BranchID = Branches.ID INNER JOIN ",
												MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames ON AssetVersions.AssetNameID = AssetNames.ID INNER JOIN ",
												MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID ",
										   S"WHERE (Branches.BranchName = '", MOG_DBAPI::FixSQLParameterString(MOG_ControllerProject::GetBranchName()), S"') AND ",
										   S"(AssetNames.AssetLabel ", comparison, S" '", MOG_DBAPI::FixSQLParameterString(label), S"')" );

	return QueryAssets(selectString);
}


ArrayList *MOG_DBAssetAPI::GetAllAssetsByPlatform(String *platform)
{
	String *selectString = String::Concat( S"SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ",
										   S"FROM (SELECT DISTINCT * FROM ", MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), S") AS BranchLinks INNER JOIN ",
												MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions ON BranchLinks.AssetVersionID = AssetVersions.ID INNER JOIN ",
												MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches ON BranchLinks.BranchID = Branches.ID INNER JOIN ",
												MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames ON AssetVersions.AssetNameID = AssetNames.ID INNER JOIN ",
												MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID ",
										   S"WHERE (Branches.BranchName = '", MOG_DBAPI::FixSQLParameterString(MOG_ControllerProject::GetBranchName()), S"') AND ",
												S"(AssetNames.AssetPlatformKey = '", MOG_DBAPI::FixSQLParameterString(platform), S"')" );

	return QueryAssets(selectString);
}

ArrayList *MOG_DBAssetAPI::GetAllAssetsByProperty(MOG_Property *property)
{
	return GetAllAssetsByProperty("", property);
}

ArrayList *MOG_DBAssetAPI::GetAllAssetsByProperty(String *classification, MOG_Property *property)
{
	return GetAllAssetsByProperty(classification, property, false);
}

ArrayList *MOG_DBAssetAPI::GetAllAssetsByProperty(String *classification, MOG_Property *property, bool bUseNotEqualComparison)
{
	String *selectString = String::Concat(
		S"SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ",
		S"FROM (SELECT DISTINCT * FROM ", MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), S") AS BranchLinks INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions ON BranchLinks.AssetVersionID = AssetVersions.ID INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches ON BranchLinks.BranchID = Branches.ID INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames ON AssetVersions.AssetNameID = AssetNames.ID INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetPropertiesTable(), S" AssetProperties ON AssetVersions.ID = AssetProperties.AssetVersionID ",
		S"WHERE (Branches.BranchName = '", MOG_DBAPI::FixSQLParameterString(MOG_ControllerProject::GetBranchName()), S"') AND ",
			S"(AssetNames.RemovedDate = '')" );

	// Check if we have a classification specified?
	if (classification &&
		classification->Length)
	{
		selectString = String::Concat(selectString, S" AND (AssetClassifications.FullTreeName = '", MOG_DBAPI::FixSQLParameterString(classification), S"') ");
	}

	// Check if we have a property specified?
	if (property)
	{
		if (property->mSection->Length)
		{
			String* value = MOG_DBAssetAPI::PrepareValueForSQLComparison(property->mSection);
			String* comparison = GetProperSQLComparisonString(value, false);
			selectString = String::Concat(selectString, S" AND (AssetProperties.Section ", comparison, S" '", MOG_DBAPI::FixSQLParameterString(value), S"') ");
		}
		if (property->mKey->Length)
		{
			String* value = MOG_DBAssetAPI::PrepareValueForSQLComparison(property->mKey);
			String* comparison = GetProperSQLComparisonString(value, false);
			selectString = String::Concat(selectString, S" AND (AssetProperties.Property ", comparison, S" '", MOG_DBAPI::FixSQLParameterString(value), S"') ");
		}
		if (property->mPropertyValue->Length)
		{
			String* value = MOG_DBAssetAPI::PrepareValueForSQLComparison(property->mPropertyValue);
			String* comparison = GetProperSQLComparisonString(value, bUseNotEqualComparison);
			selectString = String::Concat(selectString, S" AND (AssetProperties.Value ", comparison, S" '", MOG_DBAPI::FixSQLParameterString(value), S"') ");
		}
	}

	return QueryAssets(selectString);
}

ArrayList *MOG_DBAssetAPI::GetAllArchivedAssetsByProperty(MOG_Property *property)
{
	String *selectString = String::Concat(
		S"SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ",
		S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames ON AssetVersions.AssetNameID = AssetNames.ID INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetPropertiesTable(), S" AssetProperties ON AssetVersions.ID = AssetProperties.AssetVersionID ",
		S"WHERE (AssetProperties.Section = '", MOG_DBAPI::FixSQLParameterString(property->mSection), S"') AND ",
			S"(AssetProperties.Property = '", MOG_DBAPI::FixSQLParameterString(property->mKey), S"') AND ",
			S"(AssetProperties.Value = '", MOG_DBAPI::FixSQLParameterString(property->mPropertyValue), S"')" );

	return QueryAssets(selectString);
}

ArrayList *MOG_DBAssetAPI::GetAllArchivedAssets() 
{
	String *selectString = String::Concat(
		S"SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ",
		S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames ON AssetVersions.AssetNameID = AssetNames.ID INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID ");

	return QueryAssets(selectString);
}

ArrayList *MOG_DBAssetAPI::GetAllArchivedAssetsByPlatform(String *platform)
{
	String *selectString = String::Concat(
		S"SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ",
		S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames ON AssetVersions.AssetNameID = AssetNames.ID INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID ",
	    S"WHERE (AssetNames.AssetPlatformKey = '", MOG_DBAPI::FixSQLParameterString(platform), S"')" );

	return QueryAssets(selectString);
}

ArrayList *MOG_DBAssetAPI::GetAllAssetsBySyncLocation(String *syncLocation, String *platform)
{
	//use the query build to create the query for this function.
	String *selectString = MOG_DBQueryBuilderAPI::MOG_AssetQueries::Query_AllAssetsForBranchBySyncLocation(syncLocation, platform);
	ArrayList *assets = QueryAssets(selectString);

	// Resolve any platform specific asset collisions that may be contained within the list of assets
	return MOG_ControllerProject::ResolveAssetNameListForPlatformSpecificAssets(assets, platform);
}

ArrayList *MOG_DBAssetAPI::GetAllCurrentAssetsBySyncLocation(String *syncLocation, String *platform, bool bExcludeLibrary)
{
	return GetAllCurrentAssetsBySyncLocation(syncLocation, platform, bExcludeLibrary, MOG_ControllerProject::GetBranchName());
}

ArrayList *MOG_DBAssetAPI::GetAllCurrentAssetsBySyncLocation(String *syncLocation, String *platform, bool bExcludeLibrary, String *branchName)
{
	//Use the Query builder to create the query for this function.
	String *selectString = MOG_DBQueryBuilderAPI::MOG_AssetQueries::Query_AllCurrentAssetsForBranchBySyncLocation(syncLocation, platform, bExcludeLibrary, branchName);
	ArrayList *assets = QueryAssets(selectString);

	// Resolve any platform specific asset collisions that may be contained within the list of assets
	return MOG_ControllerProject::ResolveAssetNameListForPlatformSpecificAssets(assets, platform);
}

ArrayList *MOG_DBAssetAPI::GetAllAssetsBySyncLocationWithProperties(String *syncLocation, String *platform, ArrayList *properties)
{
	String *selectCmd = S"SELECT DISTINCT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ";

	// add all the prop values we want
	for(int i = 0; i < properties->Count; i++)
	{
		String *num = Convert::ToString(i);
		selectCmd = String::Concat( selectCmd, S", Prop", num, S".Value AS PropValue", num, S" ");
	}

	// add FROM clause
	selectCmd = String::Concat( selectCmd,
		S"FROM ", MOG_ControllerSystem::GetDB()->GetPlatformsTable(), S" Platforms INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetSyncTargetFileMapTable(), S" SyncTargetFileMap ON Platforms.ID = SyncTargetFileMap.PlatformID INNER JOIN (SELECT DISTINCT * FROM ",
			MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), S") AS BranchLinks INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches ON BranchLinks.BranchID = Branches.ID INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions ON AssetNames.ID = AssetVersions.AssetNameID INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID ON ",
				S"BranchLinks.AssetVersionID = AssetVersions.ID ON SyncTargetFileMap.AssetVersionID = BranchLinks.AssetVersionID " );

	// add all the prop tables
	for(int i = 0; i < properties->Count; i++)
	{
		String *num = Convert::ToString(i);
		MOG_Property *theProp = __try_cast<MOG_Property *>(properties->Item[i]);

		selectCmd = String::Concat( selectCmd, 
			S"LEFT OUTER JOIN ",
				S"(SELECT * ",
					S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetPropertiesTable(), S" prop ",
					S"WHERE (prop.Section = '", MOG_DBAPI::FixSQLParameterString(theProp->mSection), S"') AND ",
					S"(prop.Property = '", MOG_DBAPI::FixSQLParameterString(theProp->mKey), S"')) Prop", num, S" ON AssetVersions.ID = Prop", num, S".AssetVersionID " );
	}

	// add WHERE clause
	selectCmd  = String::Concat(selectCmd,
		S"WHERE (SyncTargetFileMap.SyncTargetFile LIKE '", MOG_DBAPI::FixSQLParameterString(syncLocation), S"%') AND ",
			S"(Branches.BranchName = '", MOG_DBAPI::FixSQLParameterString(MOG_ControllerProject::GetBranchName()), S"') AND ",
			S"(Platforms.PlatformName = '", MOG_DBAPI::FixSQLParameterString(platform), S"')" );

	return QueryAssetsWithProperties(selectCmd, properties);
}

ArrayList *MOG_DBAssetAPI::GetAllAssetsByVersion(String *version)
{
	String *selectCmd = String::Concat( S"SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ",
										S"FROM (SELECT DISTINCT * FROM ", MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), S") AS BranchLinks INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions ON BranchLinks.AssetVersionID = AssetVersions.ID INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches ON BranchLinks.BranchID = Branches.ID INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames ON AssetVersions.AssetNameID = AssetNames.ID INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID ",
										S"WHERE (Branches.BranchName = '", MOG_DBAPI::FixSQLParameterString(MOG_ControllerProject::GetBranchName()), S"') AND ",
											S"(AssetVersions.Version > '", MOG_DBAPI::FixSQLParameterString(version), S"')" );

	return QueryAssets(selectCmd);
}

bool MOG_DBAssetAPI::RemoveAssetFromBranch(MOG_Filename *pAsset, String *branch)
{
	// Early out if the asset is not valid
	if( pAsset == NULL )
	{
		return false;
	}

	// use current branch if none specified
	if( branch == NULL || branch->Length == 0 )
	{
		branch = MOG_ControllerProject::GetBranchName();
	}

	int branchID = MOG_DBProjectAPI::GetBranchIDByName(branch);
	int versionID = MOG_DBAssetAPI::GetAssetVersionID(pAsset, NULL);

	String *deleteCmd = String::Concat( S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetBranchLinksTable(),
										S" WHERE (BranchID=", Convert::ToString(branchID), S") AND ",
											S"(AssetVersionID=", Convert::ToString(versionID), S")" );
	
	return MOG_DBAPI::RunNonQuery(deleteCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
}

bool MOG_DBAssetAPI::AddAsset(MOG_Filename *blessedAssetFilename, String *createdBy, String *createdDate)
{
	if( createdDate == NULL || createdDate->Length == 0)
	{
		createdDate = MOG_Time::GetVersionTimestamp();
	}

	// Make sure we have an AssetNames record
	int assetNameID = CreateAssetName(blessedAssetFilename, createdBy, createdDate);
	if( assetNameID != 0)
	{
		int createdByID = MOG_DBProjectAPI::GetUserID(createdBy);

		// Create AssetRevision
		int assetVersionID = CreateAssetRevision(assetNameID, blessedAssetFilename->GetVersionTimeStamp(), createdByID, createdDate);
		if( assetVersionID != 0 )
		{
			return true;
		}
	}

	return true;
}


String *MOG_DBAssetAPI::GetAssetVersion(MOG_Filename *pAsset)
{
//?	MOG_DBAssetAPI::GetAssetVersion - This should use the database cache
	String *selectCmd = String::Concat(	S"SELECT AssetVersions.Version ",
										S"FROM (SELECT DISTINCT * FROM ", MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), S") AS BranchLinks INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions ON BranchLinks.AssetVersionID = AssetVersions.ID INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches ON BranchLinks.BranchID = Branches.ID INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames ON AssetVersions.AssetNameID = AssetNames.ID INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID ",
										S"WHERE (Branches.BranchName = '", MOG_DBAPI::FixSQLParameterString(MOG_ControllerProject::GetBranchName()), S"') AND ",
											S"(AssetClassifications.FullTreeName = '", MOG_DBAPI::FixSQLParameterString(pAsset->GetAssetClassification()), S"') AND ",
											S"(AssetNames.AssetLabel = '", MOG_DBAPI::FixSQLParameterString(pAsset->GetAssetLabel()), S"') AND ",
											S"(AssetNames.AssetPlatformKey = '", MOG_DBAPI::FixSQLParameterString(pAsset->GetAssetPlatform()), S"')" );

	return MOG_DBAPI::QueryString(selectCmd, S"Version");
}

ArrayList *MOG_DBAssetAPI::GetAllAssetRevisions(MOG_Filename *pAsset)
{
//?	MOG_DBAssetAPI::GetAllAssetRevisions - This should use the database cache
	String *selectCmd = String::Concat( S"SELECT AssetVersions.Version ",
										S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames ON AssetVersions.AssetNameID = AssetNames.ID INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID ",
										S"WHERE (AssetClassifications.FullTreeName = '", MOG_DBAPI::FixSQLParameterString(pAsset->GetAssetClassification()), S"') AND ",
										S"(AssetNames.AssetLabel = '", MOG_DBAPI::FixSQLParameterString(pAsset->GetAssetLabel()), S"') AND ",
											S"(AssetNames.AssetPlatformKey = '", MOG_DBAPI::FixSQLParameterString(pAsset->GetAssetPlatform()), S"') ",
										S"ORDER BY AssetVersions.Version DESC" );

	return MOG_DBAPI::QueryStrings(selectCmd, S"Version");
}

ArrayList *MOG_DBAssetAPI::GetAllUnusedAssetRevisions(MOG_Filename *pAsset)
{
	String *selectCmd = String::Concat(
		S"SELECT AssetVersions.Version ",
		S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames ON AssetVersions.AssetNameID = AssetNames.ID INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID ",
		S"WHERE (AssetClassifications.FullTreeName = '", MOG_DBAPI::FixSQLParameterString(pAsset->GetAssetClassification()), S"') AND ",
		S"(AssetNames.AssetLabel = '", MOG_DBAPI::FixSQLParameterString(pAsset->GetAssetLabel()), S"') AND ",
			S"(AssetNames.AssetPlatformKey = '", MOG_DBAPI::FixSQLParameterString(pAsset->GetAssetPlatform()), S"') AND ",
			S"(AssetVersions.ID NOT IN ",
				S"(SELECT DISTINCT BranchLinks.AssetVersionID ",
					S"FROM ", MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), S" BranchLinks)) AND ",
			S"(AssetVersions.ID NOT IN ",
				S"(SELECT DISTINCT PackageLinks.AssetVersionID ",
					S"FROM ", MOG_ControllerSystem::GetDB()->GetPackageLinksTable(), S" PackageLinks)) AND ",
			S"(AssetVersions.ID NOT IN ",
				S"(SELECT DISTINCT SDL.AssetVersionID ",
					S"FROM ", MOG_ControllerSystem::GetDB()->GetSyncedDataLinksTable(), S" SDL)) ",
		S"ORDER BY AssetVersions.Version DESC" );

	ArrayList *unusedRevisions = MOG_DBAPI::QueryStrings(selectCmd, S"Version");


	// Make sure to exclude any versions that might be listed in the PostCommands
	ArrayList *postCommands = MOG_DBPostCommandAPI::GetScheduledPosts(MOG_ControllerProject::GetBranchName());
	if (postCommands)
	{
		// Cycle through all the pending commands
		for (int i = 0; i < postCommands->Count; i++)
		{
			// Check if this pending command matches our asset name?
			MOG_DBPostInfo *pendingCommand = __try_cast<MOG_DBPostInfo *>(postCommands->Item[i]);
			if (String::Compare(pendingCommand->mAssetName, pAsset->GetAssetFullName(), true) == 0)
			{
				// Scan our unusedRevisions looking for a match
				// Don't automatically increment incase the array collapses around us on a remove
				for (int r = 0; r < unusedRevisions->Count; /*r++*/)
				{
					// Check if this revision is listed in our pending commands?
					String *revision = __try_cast<String*>(unusedRevisions->Item[r]);
					if (String::Compare(revision, pendingCommand->mAssetVersion, true) == 0)
					{
						// Remove this revision because it is still pending a post
						unusedRevisions->RemoveAt(r);
						continue;
					}
					// Increment our index now that we know we didn't remove anything
					r++;
				}
			}
		}
	}


	// Make sure to exclude any versions that might be listed in the PackageCommands
	ArrayList *packageCommands = MOG_DBPackageCommandAPI::GetScheduledPackageCommands(MOG_ControllerProject::GetBranchName());
	if (packageCommands)
	{
		// Cycle through all the pending commands
		for (int i = 0; i < packageCommands->Count; i++)
		{
			// Check if this post matches our asset name?
			MOG_DBPackageCommandInfo *pendingCommand = __try_cast<MOG_DBPackageCommandInfo *>(packageCommands->Item[i]);
			if (String::Compare(pendingCommand->mAssetName, pAsset->GetAssetFullName(), true) == 0)
			{
				// Scan our unusedRevisions looking for a match
				// Don't automatically increment incase the array collapses around us on a remove
				for (int r = 0; r < unusedRevisions->Count; /*r++*/)
				{
					// Check if this revision is listed in our pending commands?
					String *revision = __try_cast<String*>(unusedRevisions->Item[r]);
					if (String::Compare(revision, pendingCommand->mAssetVersion, true) == 0)
					{
						// Remove this revision because it is still pending a post
						unusedRevisions->RemoveAt(r);
						continue;
					}
					// Increment our index now that we know we didn't remove anything
					r++;
				}
			}
		}
	}


	return unusedRevisions;
}

ArrayList *MOG_DBAssetAPI::GetAllAssets()
{
	return GetAllAssets(false);
}

ArrayList *MOG_DBAssetAPI::GetAllAssets(bool bExcludeLibrary)
{
	//use the query builder to create the query for this function.
	String *selectCmd = MOG_DBQueryBuilderAPI::MOG_AssetQueries::Query_AllAssetsForBranch(bExcludeLibrary);
	return QueryAssets(selectCmd);
}

ArrayList *MOG_DBAssetAPI::GetAllAssets(bool bExcludeLibrary, String *branchName)
{
	//use the query builder to create the query for this function.
	String *selectCmd = MOG_DBQueryBuilderAPI::MOG_AssetQueries::Query_AllAssetsForBranch(bExcludeLibrary, branchName);
	return QueryAssets(selectCmd);
}

ArrayList *MOG_DBAssetAPI::GetAllAssets(String* classification, bool bExcludeLibrary)
{
	String *selectCmd = MOG_DBQueryBuilderAPI::MOG_AssetQueries::Query_AllCurrentAssetsForBranchByClassificationTree(classification, true);
	return QueryAssets(selectCmd);
}

ArrayList *MOG_DBAssetAPI::GetAssetNames()
{
	String *selectString = String::Concat(
		S"SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel ",
		S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AS AssetNames INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AS AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID ");

	return QueryAssets(selectString);
}

ArrayList *MOG_DBAssetAPI::GetAllAssetsAllRevisionsForProject(String *projectName)
{
	if (MOG_DBAPI::TableExists(String::Concat(projectName,S".AssetNames")) && 
		MOG_DBAPI::TableExists(String::Concat(projectName,S".AssetVersions")) && 
		MOG_DBAPI::TableExists(String::Concat(projectName,S".AssetClassifications")))
	{
		String *selectCmd = MOG_DBQueryBuilderAPI::MOG_AssetQueries::Query_AllAssetsAllRevisionsForProject(projectName);
		return QueryAssets(selectCmd);
	}
	return new ArrayList();
}

bool MOG_DBAssetAPI::DoesAssetVersionExist(MOG_Filename *assetName, String *version)
{
	return (GetAssetVersionID(assetName, version) != 0);
}

ArrayList *MOG_DBAssetAPI::GetAssetRevisionReferences(MOG_Filename *assetName, String *version, bool bGetAllReferences)
{
	ArrayList *references = new ArrayList();

	references->AddRange(GetAssetRevisionReferencesInBranches(assetName, version));
	if (!bGetAllReferences && references->Count > 0)
	{
		// Looks like we can bail early!
		return references;
	}

	references->AddRange(GetAssetRevisionReferencesInPackages(assetName, version));
	if (!bGetAllReferences && references->Count > 0)
	{
		// Looks like we can bail early!
		return references;
	}

	references->AddRange(GetAssetRevisionReferencesInPendingPostCommands(assetName, version));
	if (!bGetAllReferences && references->Count > 0)
	{
		// Looks like we can bail early!
		return references;
	}

	references->AddRange(GetAssetRevisionReferencesInPendingPackageCommands(assetName, version));
	if (!bGetAllReferences && references->Count > 0)
	{
		// Looks like we can bail early!
		return references;
	}

// JohnRen - Removed because this ended up being too much of a pain when trying to clean up a project
//	references->AddRange(GetAssetRevisionReferencesInWorkspaces(assetName, version));

	return references;
}

ArrayList *MOG_DBAssetAPI::GetAssetRevisionReferencesInBranches(MOG_Filename *assetName, String *version)
{
	ArrayList *references = new ArrayList();

	// Get all branch references for this asset
	int assetVersionID = GetAssetVersionID(assetName, version);
	ArrayList *branches = MOG_DBAssetAPI::GetBranchReferencesForAsset(assetVersionID);
	for( int i = 0; i < branches->Count; i++ )
	{
		String *branchName = __try_cast<String*>(branches->Item[i]);
		references->Add(String::Concat(S"BRANCH: ", branchName));
	}

	return references;
}

ArrayList *MOG_DBAssetAPI::GetAssetRevisionReferencesInPackages(MOG_Filename *assetName, String *version)
{
	ArrayList *references = new ArrayList();

	// Get all package references for this asset
	int assetVersionID = GetAssetVersionID(assetName, version);
	ArrayList *packages = MOG_DBAssetAPI::GetPackageReferencesForAsset(assetVersionID);
	for( int i = 0; i < packages->Count; i++ )
	{
		MOG_Filename *packageName = __try_cast<MOG_Filename*>(packages->Item[i]);
		references->Add(String::Concat(S"PACKAGE: ", packageName->GetFullFilename(), S"   REVISION: ", packageName->GetVersionTimeStamp()));
	}

	return references;
}

ArrayList *MOG_DBAssetAPI::GetAssetRevisionReferencesInPendingPostCommands(MOG_Filename *assetName, String *version)
{
	ArrayList *references = new ArrayList();

	// Get all PendingPostCommands for asset
	ArrayList *pendingPostCommands = MOG_DBAssetAPI::GetPendingPostCommandsForAsset(assetName, version);
	if (pendingPostCommands->Count)
	{
		references->Add(S"PENDING POST COMMAND: ");
	}

	return references;
}

ArrayList *MOG_DBAssetAPI::GetAssetRevisionReferencesInPendingPackageCommands(MOG_Filename *assetName, String *version)
{
	ArrayList *references = new ArrayList();

	// Get all PendingPostCommands for asset
	ArrayList *pendingPackageCommands = MOG_DBAssetAPI::GetPendingPackageCommandsForAsset(assetName, version);
	if (pendingPackageCommands->Count)
	{
		references->Add(S"PENDING PACKAGE COMMAND: ");
	}

	return references;
}

ArrayList *MOG_DBAssetAPI::GetAssetRevisionReferencesInWorkspaces(MOG_Filename *assetName, String *version)
{
	ArrayList *references = new ArrayList();

	int assetVersionID = GetAssetVersionID(assetName, version);
	ArrayList *computers = GetLocalWorkspaceReferencesForAsset(assetVersionID);
	for( int i = 0; i < computers->Count; i++ )
	{
		String *computerName = __try_cast<String*>(computers->Item[i]);

		references->Add(String::Concat(S"SYNCED WORKSPACE: ", computerName));
	}

	return references;
}

ArrayList *MOG_DBAssetAPI::GetBranchReferencesForAsset(int assetVersionID)
{
	// check branches
	String *selectCmd = String::Concat(
		S"SELECT Branches.BranchName ",
		S"FROM (SELECT DISTINCT * FROM ", MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), S") AS BranchLinks INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches ON BranchLinks.BranchID = Branches.ID ",
		S"WHERE (BranchLinks.AssetVersionID = ", Convert::ToString(assetVersionID), S")" );

	return MOG_DBAPI::QueryStrings(selectCmd, S"BranchName");
}

ArrayList *MOG_DBAssetAPI::GetPackageReferencesForAsset(int assetVersionID)
{
	String *selectCmd = String::Concat(
		S"SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ",
		S"FROM ", MOG_ControllerSystem::GetDB()->GetPackageLinksTable(), S" PackageLinks INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetPackageGroupNamesTable(), S" PackageGroupNames ON PackageLinks.PackageGroupNameID = PackageGroupNames.ID INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions ON PackageGroupNames.PackageVersionID = AssetVersions.ID INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames ON AssetVersions.AssetNameID = AssetNames.ID INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID ",
		S"WHERE (PackageLinks.AssetVersionID = ", Convert::ToString(assetVersionID), S")" );

	return QueryAssets(selectCmd);
}

ArrayList *MOG_DBAssetAPI::GetPendingPostCommandsForAsset(MOG_Filename *assetName, String *revision)
{
	ArrayList *results = new ArrayList();

	// Make sure to check the PostCommands
	ArrayList *postCommands = MOG_DBPostCommandAPI::GetScheduledPosts(MOG_ControllerProject::GetBranchName());
	if (postCommands)
	{
		// Cycle through all the pending commands
		for (int i = 0; i < postCommands->Count; i++)
		{
			// Check if this asset's revision is in our pending command?
			MOG_DBPostInfo *pendingCommand = __try_cast<MOG_DBPostInfo *>(postCommands->Item[i]);
			if (String::Compare(pendingCommand->mAssetName, assetName->GetAssetFullName(), true) == 0)
			{
				// Check if a specific revision was specified?
				if (revision->Length)
				{
					// CHeckif this command is for this revision?
					if (String::Compare(revision, pendingCommand->mAssetVersion, true) == 0)
					{
						results->Add(pendingCommand);
					}
				}
				else
				{
					results->Add(pendingCommand);
				}
			}
		}
	}

	return results;
}

ArrayList *MOG_DBAssetAPI::GetPendingPackageCommandsForAsset(MOG_Filename *assetName, String *revision)
{
	ArrayList *results = new ArrayList();

	// Make sure to check the PackageCommands
	ArrayList *packageCommands = MOG_DBPackageCommandAPI::GetScheduledPackageCommands(MOG_ControllerProject::GetBranchName());
	if (packageCommands)
	{
		// Cycle through all the pending commands
		for (int i = 0; i < packageCommands->Count; i++)
		{
			// Check if this asset's revision is in our pending command?
			MOG_DBPackageCommandInfo *pendingCommand = __try_cast<MOG_DBPackageCommandInfo *>(packageCommands->Item[i]);
			if (String::Compare(pendingCommand->mAssetName, assetName->GetAssetFullName(), true) == 0)
			{
				// Check if a specific revision was specified?
				if (revision->Length)
				{
					// CHeckif this command is for this revision?
					if (String::Compare(revision, pendingCommand->mAssetVersion, true) == 0)
					{
						results->Add(pendingCommand);
					}
				}
				else
				{
					results->Add(pendingCommand);
				}
			}
		}
	}

	return results;
}

ArrayList *MOG_DBAssetAPI::GetLocalWorkspaceReferencesForAsset(int assetVersionID)
{
	// check synced data
	String *selectCmd = String::Concat(
		S"SELECT Locations.ComputerName ",
		S"FROM ", MOG_ControllerSystem::GetDB()->GetSyncedDataLinksTable(), S" SDL INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetSyncedDataLocationsTable(), S" Locations ON SDL.SyncedDataLocationID = Locations.ID ",
		S"WHERE (SDL.AssetVersionID = ", Convert::ToString(assetVersionID), S")" );

	return MOG_DBAPI::QueryStrings(selectCmd, S"ComputerName");
}

bool MOG_DBAssetAPI::RemoveAssetName(int assetNameID)
{
	if(assetNameID != 0)
	{
		String *deleteCmd = String::Concat(	S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" ",
											S"WHERE (ID = ", assetNameID.ToString(), S")");
		if (MOG_DBAPI::RunNonQuery(deleteCmd, MOG_ControllerSystem::GetDB()->GetConnectionString()))
		{
			// Make sure we also remove this from our cache
			MOG_ControllerSystem::GetDB()->GetDBCache()->GetAssetNameCache()->RemoveSetFromCacheByID(assetNameID);
			return true;
		}

		return false;
	}

	return true;
}

bool MOG_DBAssetAPI::RemoveAssetName(MOG_Filename *assetFilename)
{
	int assetNameID = GetAssetNameID(assetFilename);
	if(assetNameID != 0)
	{
		return MOG_DBAssetAPI::RemoveAssetName(assetNameID);
	}

	return true;
}

bool MOG_DBAssetAPI::RemoveAssetVersion(MOG_Filename *assetName, String *version)
{
	bool bRemoved = false;

	// Check to make sure the asset isn't being referenced by anything
	if( GetAssetRevisionReferences(assetName, version, false)->Count == 0 )
	{
		// Make sure we have a valid asset version
		int assetVersionID = GetAssetVersionID(assetName, version);
		if( assetVersionID != 0 )
		{
			ArrayList *transactionItems = new ArrayList();

			// Remove all properties
			transactionItems->Add( String::Concat(S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetAssetPropertiesTable(),
													S" WHERE (AssetVersionID = ", Convert::ToString(assetVersionID), S")" ));

			// Remove all game data files
			transactionItems->Add( String::Concat(S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetSyncTargetFileMapTable(),
													S" WHERE (AssetVersionID = ", Convert::ToString(assetVersionID), S")" ));

			// Remove all package links
			transactionItems->Add( String::Concat(S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetPackageLinksTable(),
													S" WHERE (PackageGroupNameID IN ",
														S"(SELECT ID ",
															S"FROM ", MOG_ControllerSystem::GetDB()->GetPackageGroupNamesTable(), S" PGN ",
															S"WHERE (PackageVersionID = ", Convert::ToString(assetVersionID), S")))" ));
			// Remove all package groups
			transactionItems->Add( String::Concat(S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetPackageGroupNamesTable(),
													S" WHERE (PackageVersionID = ", Convert::ToString(assetVersionID), S")" ));

			// Remove asset version record
			transactionItems->Add( String::Concat(S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(),
													S" WHERE (ID = ", Convert::ToString(assetVersionID), S")" ));

			// Execute the entire transaction
			if (MOG_DBAPI::ExecuteTransaction(transactionItems))
			{
				// Indicate this was properly removed
				bRemoved = true;
			}
		}
		else
		{
			// This revision is already gone!
			bRemoved = true;
		}
	}

	return bRemoved;
}

int MOG_DBAssetAPI::GetAssetVersionID(MOG_Filename *assetName, String *version)
{
	String *ver;

	// get correct version
	if( version == NULL )
	{
		ver = assetName->GetVersionTimeStamp();
	}
	else
	{
		ver = version;
	}

	int retAssetVersionID = MOG_ControllerSystem::GetDB()->GetDBCache()->GetAssetVersionCache()->GetIDFromName(ver, assetName->GetAssetFullName());
	if(retAssetVersionID == 0)
	{
		String *selectCmd = String::Concat(	S"SELECT AssetVersions.ID ",
											S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames INNER JOIN ",
												MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions ON AssetNames.ID = AssetVersions.AssetNameID INNER JOIN ",
												MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID ",
											S"WHERE (AssetVersions.Version = '", MOG_DBAPI::FixSQLParameterString(ver), S"') AND ",
												S"(AssetClassifications.FullTreeName = '", MOG_DBAPI::FixSQLParameterString(assetName->GetAssetClassification()), S"') AND ",
												S"(AssetNames.AssetPlatformKey = '", MOG_DBAPI::FixSQLParameterString(assetName->GetAssetPlatform()), S"') AND ",
												S"(AssetNames.AssetLabel = '", MOG_DBAPI::FixSQLParameterString(assetName->GetAssetLabel()), S"')" );

		retAssetVersionID = MOG_DBAPI::QueryInt(selectCmd, S"ID");
		if(retAssetVersionID > 0)
		{
			MOG_ControllerSystem::GetDB()->GetDBCache()->GetAssetVersionCache()->AddSetToCache(retAssetVersionID, ver, assetName->GetAssetFullName());
		}
	}
	return retAssetVersionID;
}

int MOG_DBAssetAPI::GetCurrentAssetVersionID(MOG_Filename *assetName)
{
	String *selectCmd = String::Concat(	S"SELECT AssetVersions.ID ",
										S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions ON AssetNames.ID = AssetVersions.AssetNameID INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID INNER JOIN (SELECT DISTINCT * FROM ",
											MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), S") AS BranchLinks ON AssetVersions.ID = BranchLinks.AssetVersionID INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches ON BranchLinks.BranchID = Branches.ID ",
										S"WHERE (Branches.BranchName = '", MOG_DBAPI::FixSQLParameterString(MOG_ControllerProject::GetBranchName()), S"') AND ",
											S"(AssetClassifications.FullTreeName = '", MOG_DBAPI::FixSQLParameterString(assetName->GetAssetClassification()), S"') AND ",
											S"(AssetNames.AssetPlatformKey = '", MOG_DBAPI::FixSQLParameterString(assetName->GetAssetPlatform()), S"') AND ",
											S"(AssetNames.AssetLabel = '", MOG_DBAPI::FixSQLParameterString(assetName->GetAssetLabel()), S"')" );

	return MOG_DBAPI::QueryInt(selectCmd, S"ID");
}

MOG_Filename *MOG_DBAssetAPI::GetAssetName(int nameID)
{
	MOG_Filename *assetFilename = NULL;

	// First check our cache?
	String *cachedValue = MOG_ControllerSystem::GetDB()->GetDBCache()->GetAssetNameCache()->GetNameFromID(nameID);
	if(cachedValue == NULL)
	{
		// Build our select string
		String *selectCmd = String::Concat( S"SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel ",
											S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames INNER JOIN ",
												MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID ",
											S"WHERE (AssetNames.ID = ", nameID, S")" );

		// Perform our query for a single asset
		assetFilename = QueryAsset(selectCmd);
		if(assetFilename)
		{
			// Save the value in our cache
			cachedValue = assetFilename->GetAssetFullName();
			MOG_ControllerSystem::GetDB()->GetDBCache()->GetAssetNameCache()->AddSetToCache(nameID, cachedValue);
		}
	}
	else
	{
		// Create a new MOG_Filename from the cached value
		assetFilename = new MOG_Filename(cachedValue);
	}

	return assetFilename;
}

ArrayList *MOG_DBAssetAPI::GetAssetNameIDs(MOG_Filename *pAsset)
{
	if( pAsset == NULL )
	{
		return 0;
	}

	String *selectString = String::Concat(	S"SELECT AssetNames.ID ",
											S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames INNER JOIN ",
												MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID ",
											S"WHERE (AssetClassifications.FullTreeName = '", MOG_DBAPI::FixSQLParameterString(pAsset->GetAssetClassification()), S"') AND ",
												S"(AssetNames.AssetPlatformKey = '", MOG_DBAPI::FixSQLParameterString(pAsset->GetAssetPlatform()), S"') AND ",
												S"(AssetNames.AssetLabel = '", MOG_DBAPI::FixSQLParameterString(pAsset->GetAssetLabel()), S"')",
											S"ORDER BY ID");

	// Query our int array for this AssetNameID
	return MOG_DBAPI::QueryIntArray(selectString, S"ID");
}

int MOG_DBAssetAPI::GetAssetNameID(MOG_Filename *pAsset)
{
	int assetNameID = MOG_ControllerSystem::GetDB()->GetDBCache()->GetAssetNameCache()->GetIDFromName(pAsset->GetAssetFullName());
	if(assetNameID == 0)
	{
		// Query our int array for this AssetNameID
		ArrayList *values = GetAssetNameIDs(pAsset);
		// Check if we received more than one back?
		if (values->Count > 1)
		{
			// Warn the user about this double AssetNameID
			String *message = String::Concat(S"This asset has more than one AssetNameID in the database which can cause unexpected problems when working with this asset.\n",
											 S"Asset: ", pAsset->GetAssetFullName(), S"\n\n",
											 S"Using the Admin Tools can properly clean up this issue.\n");
			MOG_Report::ReportMessage(S"AssetNameID", message, "", MOG_ALERT_LEVEL::ERROR);
		}

		assetNameID = 0;
		// Obtain our AssetNameID from the array of values
		if (values->Count > 0)
		{
			// Always take the last on in the array as that one is the most recently added AssetNameID
			assetNameID = *dynamic_cast<__box int*>(values->Item[values->Count - 1]);
			// Stick this newly obtained ID into our cache
			MOG_ControllerSystem::GetDB()->GetDBCache()->GetAssetNameCache()->AddSetToCache(assetNameID, pAsset->GetAssetFullName());
		}
	}

	return assetNameID;
}

int MOG_DBAssetAPI::GetPlatformID(String *platform)
{
	String *selectCmd = String::Concat( S"SELECT ID FROM ", MOG_ControllerSystem::GetDB()->GetPlatformsTable(),
										S" WHERE (PlatformName='", MOG_DBAPI::FixSQLParameterString(platform), S"')" );

	return MOG_DBAPI::QueryInt(selectCmd, "ID");
}

bool MOG_DBAssetAPI::RemoveAllAssets()
{
	// JTM-FIXME until i find out what this is supposed to do
	return false;

	String *deleteCmd = String::Concat(S"DELETE from Assets");
	
	return MOG_DBAPI::RunNonQuery(deleteCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
}

bool MOG_DBAssetAPI::AddAssetRelatedFiles(MOG_ControllerAsset *pAssetController)
{
	bool bFailed = false;

	// Make sure this asset has been blessed into the main repository?
	if (pAssetController->GetAssetFilename()->IsBlessed())
	{
		// Get all the applicable platforms for this asset
		String *platformNames[] = MOG_ControllerProject::GetAllApplicablePlaformsForAsset(pAssetController->GetAssetFilename()->GetAssetFullName(), false);
		for (int platformIndex = 0; platformIndex < platformNames->Count; platformIndex++)
		{
			String *platformName = platformNames[platformIndex];
			if (!MOG_DBAssetAPI::AddAssetRelatedFilesForPlatform(pAssetController->GetAssetFilename(), pAssetController->GetProperties(), platformName))
			{
				bFailed = true;
			}
		}
	}
	else
	{
		// Only blessed asset can be added
		bFailed = true;
	}

	if (!bFailed)
	{
		return true;
	}
	return false;
}


bool MOG_DBAssetAPI::AddAssetRelatedFilesForPlatform(MOG_Filename* assetFilename, MOG_Properties* assetProperties, String *platformName)
{
	bool bFailed = false;

	// Make sure we have a valid platform?
	if (MOG_ControllerProject::IsValidPlatform(platformName))
	{
		assetProperties->SetScope(platformName);

		// Check if the asset syncs files for this platform?
		if (assetProperties->SyncFiles)
		{
			// Get the list of files for this platform
			String *files[] = MOG_ControllerAsset::GetAssetPlatformFiles(assetProperties, platformName, false);
			if (files == NULL || files->Count == 0)
			{
				// Since there are no files listed in this asset?  At the very least, we should create a default game data file to fill this void.
				// This is important because this could be a brand new package and we want MOG to be able to recognize its file in the future.
				String *fakeFile = assetFilename->GetAssetLabel();
				// Check if this is a package?
				if (assetProperties->IsPackage)
				{
					// The properties scope needs to match the asset's platform
					assetProperties->SetScope(assetFilename->GetAssetPlatform());
					// Check if this package is missing an extension?
					if (DosUtils::PathGetExtension(fakeFile)->Length == 0)
					{
						// Check if we have a DefaultPackageFileExtension defined?
						if (assetProperties->DefaultPackageFileExtension->Length > 0)
						{
							// Apply the DefaultPackageFileExtension on this fakeFile
							fakeFile = String::Concat(fakeFile, S".", assetProperties->DefaultPackageFileExtension);
						}
					}
					// Restore the properties scope to the platform
					assetProperties->SetScope(platformName);
				}
				// Create a fake file list
				files = new String*[1];
				files[0] = fakeFile;
			}

			// Loop through all the contained files
//			ArrayList *relativeSyncFiles = new ArrayList();
			for (int fileIndex = 0; fileIndex < files->Count; fileIndex++)
			{
				String *fileName = files[fileIndex];

				// Build the relative formatted game data path for this file
				String *syncTargetPath = MOG_Tokens::GetFormattedString(assetProperties->SyncTargetPath, assetFilename, assetProperties->GetApplicableProperties());
				String *formattedGameDataPath = Path::Combine(syncTargetPath, fileName);

// JohnRen - Removed because we should do these one at a time for now...I know this is slower but doing them all together can occassionally 
// cause SQL errors...If the network package merge triggered multiple merge events (possible with lots of assets being blessed) the the links 
// are already there for this version of the package causing the problem...we need to be more surgical and add one at a time.
//				relativeSyncFiles->Add(formattedGameDataPath);
				if (!AddSyncTargetFileLink(formattedGameDataPath, assetFilename, platformName))
				{
					bFailed = true;
				}
			}

//			// It is much faster to add all the platform-specific sync files in one single call to AddSyncTargetFileLinks()
//			if (!AddSyncTargetFileLinks(relativeSyncFiles, assetFilename, platformName))
//			{
//				bFailed = true;
//			}
		}
	}
	else
	{
		bFailed = true;
	}

	if (!bFailed)
	{
		return true;
	}
	return false;
}


bool MOG_DBAssetAPI::CreateClassification(String *classification, String *createdBy, String *createdDate)
{
	return CreateClassification(classification, createdBy, createdDate, true);
}

bool MOG_DBAssetAPI::CreateClassification(String *classification, String *createdBy, String *createdDate, bool bAddToActiveBranch)
{
	if( classification == NULL )
	{
		return false;
	}

	String* delimStr = S"~";
	Char delimiter[] = delimStr->ToCharArray();
	String* parts[] = classification->Split(delimiter);
	String* fullTree = S"";

	int parentID = 0;
	for( int i = 0; i < parts->Count; i++ )
	{
		fullTree = String::Concat( fullTree, parts[i] );
		parentID = AddSubClassification(parentID, parts[i], fullTree, createdBy, createdDate, bAddToActiveBranch);
		fullTree = String::Concat( fullTree, S"~" );

		// if we failed to create/find any part of the tree...just break out
		if( parentID == 0 )
		{
			break;
		}
	}

	return (parentID != 0);
}

bool MOG_DBAssetAPI::RemoveClassification(String *classification)
{
	return RemoveClassification(classification, NULL);
}

bool MOG_DBAssetAPI::RemoveClassification(String *classification, String *branchName)
{
	bool removed = false;

	if( classification == NULL )
	{
		return removed;
	}

	if( branchName == NULL || branchName->Length == 0)
	{
		branchName = MOG_ControllerProject::GetBranchName();
	}

	// Get the classification's name ID
	int classificationID = GetClassificationID(classification);
	if( classificationID != 0 )
	{
		// Get our branch's ID in string form
		String *branchID = Convert::ToString(MOG_DBProjectAPI::GetBranchIDByName(branchName));

		// Check if this classification is active in our branch?
		if (MOG_DBAssetAPI::GetClassificationIDForBranch(classification, branchName))
		{
			// Get all of the children classification name IDs for this branch including our own
			ArrayList *allIDs = new ArrayList();
			allIDs->Add(Convert::ToString(classificationID));
			GetAllClassificationChildrenIDsInBranch(Convert::ToString(classificationID), branchID, allIDs);

			// see if any assets are referencing the nodes we are going to remove
			for( int i = 0; i < allIDs->Count; i++ )
			{
				String *selectCmd;
				selectCmd = String::Concat( S"SELECT AssetVersions.ID ",
											S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions INNER JOIN (SELECT DISTINCT * FROM ",
												MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), S") AS BranchLinks ON AssetVersions.ID = BranchLinks.AssetVersionID INNER JOIN ",
												MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames ON AssetVersions.AssetNameID = AssetNames.ID "
											S"WHERE (AssetNames.AssetClassificationID = ", __try_cast<String*>(allIDs->Item[i]),S") AND ",
												S"(BranchLinks.BranchID = ", branchID, S")" );

				MOG_ASSERT_THROW(!MOG_DBAPI::QueryExists(selectCmd), MOG_Exception::MOG_EXCEPTION_OutstandingDependency, "Can't remove classification, dependent data exists.");
			}

			// Remove the classifications from the active branch
			ArrayList *transactionCommands = new ArrayList();
			for( int i = 0; i < allIDs->Count; i++ )
			{
				transactionCommands->Add(String::Concat(S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsBranchLinksTable(),
														S" WHERE (ClassificationID = ", __try_cast<String*>(allIDs->Item[i]), S") AND ",
															S"(BranchID = ", branchID, S")" ));
			}
			// Execute the entire transaction
			removed = MOG_DBAPI::ExecuteTransaction(transactionCommands);
			if (!removed)
			{
			}
		}
		// Check if this classification is active in any branch?
		else if (!MOG_DBAssetAPI::DoesClassificationExistInAnyBranch(classification))
		{
// Yikes - these will never be able to be removed because we lost our BranchLinksID for the associated BranchNameID
//			// Clean up any properties that were associated with these IDs
//			for( int i = 0; i < allIDs->Count; i++ )
//			{
//				String *selectCmd;
//				selectCmd = String::Concat( S"SELECT AssetClassifications.FullTreeName ",
//											S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications INNER JOIN ",											MOG_ControllerSystem::GetDB()->GetAssetClassificationsBranchLinksTable(), S" AssetClassificationsBranchLinks ON ",
//												S"AssetClassifications.ID = AssetClassificationsBranchLinks.ClassificationID ",
//											S"WHERE (AssetClassificationsBranchLinks.ClassificationID = ", __try_cast<String*>(allIDs->Item[i]), S") AND ",
//												S"(AssetClassificationsBranchLinks.BranchID = ", branchID,S")" );
//
//				RemoveAllAssetClassificationProperties(MOG_DBAPI::QueryString(selectCmd, S"FullTreeName"), branchID);
//			}

			// Get all of the children classification name IDs for this branch including our own
			ArrayList *allIDs = new ArrayList();
			allIDs->Add(Convert::ToString(classificationID));
			GetAllClassificationChildrenIDs(Convert::ToString(classificationID), allIDs);

			// Completely remove any empty classifications from the database
			ArrayList *transactionCommands = new ArrayList();
			for( int i = 0; i < allIDs->Count; i++ )
			{
				String *classificationID = __try_cast<String*>(allIDs->Item[i]);

				// Check if this classificationID is no longer referenced by any branches?
				ArrayList *branchLinks = GetAllArchivedAssetsByClassification(MOG_DBAssetAPI::GetClassificationFullTreeNameByID(Convert::ToInt32(classificationID)));
				if (branchLinks->Count > 0)
				{
					MOG_ASSERT_THROW(!branchLinks->Count, MOG_Exception::MOG_EXCEPTION_OutstandingDependency, "Can't remove classification, dependent data exists.");
				}
				// Schedule this classification for removal from the database
				transactionCommands->Add(String::Concat(S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(),
														S" WHERE (ID = ", classificationID, S")" ));
			}
			// Execute the entire transaction
			removed = MOG_DBAPI::ExecuteTransaction(transactionCommands);
			if (!removed)
			{
			}
		}
		else
		{
			MOG_ASSERT_THROW(false, MOG_Exception::MOG_EXCEPTION_OutstandingDependency, "Can't remove classification because it exists in another branch.");
		}

		// Flush the classificationID cache
		MOG_ControllerSystem::GetDB()->GetDBCache()->GetClassificationBranchLinkCache()->ClearCache();
	}

	return removed;
}


bool MOG_DBAssetAPI::RenameClassificationName(String* oldClassification, String* newClassification)
{
	// Get the classification's name ID
	int classificationID = GetClassificationID(oldClassification);
	if( classificationID != 0 )
	{
		String* newParts[] = MOG_Filename::SplitClassificationString(newClassification);
		String* newName = newParts[newParts->Count -1];
		String* updateCmd = String::Concat(	S" UPDATE ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), 
											S" SET ClassificationName='", newName, S"', "
											S"     FullTreeName='", newClassification, S"' "
											S" WHERE ID=", Convert::ToString(classificationID));
		if (MOG_DBAPI::RunNonQuery(updateCmd, MOG_ControllerSystem::GetDB()->GetConnectionString()))
		{
			// Make sure we flush the cache for this classification
			MOG_ControllerSystem::GetDB()->GetDBCache()->GetClassificationCache()->RemoveSetFromCacheByID(classificationID);
			return true;
		}
	}

	return false;
}


bool MOG_DBAssetAPI::RenameAdamClassificationName(String* oldAdamClassificationName, String* newAdamClassificationName)
{
	bool bFailed = false;

	String *oldAdamHeader = String::Concat(oldAdamClassificationName, S"~");
	String *newAdamHeader = String::Concat(newAdamClassificationName, S"~");

	// Get all the classifications in the project
	ArrayList *classifications = GetAllClassifications();

	// Rename each classification in the project
	for(int c = 0; c < classifications->Count; c++)
	{
		// Rename the classification's adam object's name for this classification
		String *oldClassification = dynamic_cast<String*>(classifications->Item[c]);

		// Check if this oldClassification equals the oldAdamClassificationName?
		// Check if this oldClassification begins with the oldAdamHeader?
		if (String::Compare(oldClassification, oldAdamClassificationName, true) == 0 ||
			oldClassification->StartsWith(oldAdamHeader, StringComparison::CurrentCultureIgnoreCase))
		{
			// Build the new classification name
			String *newClassification = String::Concat(newAdamClassificationName, oldClassification->Substring(oldAdamClassificationName->Length));
			// Rename the classification name in the database
			if (!RenameClassificationName(oldClassification, newClassification))
			{
				bFailed = true;
			}
		}
	}

	// Repair all of the inbox assets because the project's adam object just got changed
	// This is needed because the inbox database stores the raw names in text form
	// Get all of the inbox assets
	ArrayList* assets = MOG_DBInboxAPI::InboxGetAssetList();
	assets->Sort();
	// Fix each asset
	for (int i = 0; i < assets->Count; i++)
	{
		MOG_Filename* oldAssetFilename = dynamic_cast<MOG_Filename*>(assets->Item[i]);

		// Check this asset's adam object to see if it is the old project?
		String* thisAdamObject = MOG_Filename::GetClassificationAdamObjectName(oldAssetFilename->GetAssetClassification());
		if (String::Compare(thisAdamObject, oldAdamClassificationName, true) == 0)
		{
			// Check if this asset is withing the inboxes?
			if (oldAssetFilename->IsWithinInboxes())
			{
				// Rebuild a newAssetFilename
				String* newAssetName = String::Concat(newAdamClassificationName, oldAssetFilename->GetAssetFullName()->Substring(oldAdamClassificationName->Length));
				MOG_Filename* newAssetFilename = new MOG_Filename(oldAssetFilename->GetUserName(), oldAssetFilename->GetBoxName(), oldAssetFilename->GetGroupTree(), newAssetName);

				if (!MOG_DBInboxAPI::InboxMoveAsset(oldAssetFilename, newAssetFilename))
				{
					bFailed = true;
				}
			}
			else
			{
				// Check if this is a local asset??
				if (oldAssetFilename->IsLocal())
				{
					// Remove this local asset from the inbox assets because we dumped all synced data locations when duplicating the project
					// KLUDGE ALERT: This code might be problematic because we don't know which user or machine this local file is from so 
					// we make a fairly significant assumption here and simply remove the record from the database by the assetFilename alone which
					// means we could inevertantly remove another record for another user on a different computer but since we want to 
					// remove all of them eventually anyway, it didn't seem to scary
					String *queryText = String::Concat(S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetInboxAssetsTable(),
														S" WHERE ",
														S"     (AssetFullName='", MOG_DBAPI::FixSQLParameterString(oldAssetFilename->GetRelativePathWithinInbox()), S"') " );
					MOG_DBAPI::RunNonQuery(queryText, MOG_ControllerSystem::GetDB()->GetConnectionString());
				}
			}
		}
	}

	if (!bFailed)
	{
		return true;
	}
	return false;
}


int MOG_DBAssetAPI::GetClassificationID(String *classification)
{
	int retClassificationID = MOG_ControllerSystem::GetDB()->GetDBCache()->GetClassificationCache()->GetIDFromName(classification);
	if(retClassificationID == 0)
	{
		String *selectCmd;
		selectCmd = String::Concat( S"SELECT ID ",
									S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" ",
									S"WHERE (FullTreeName = '", MOG_DBAPI::FixSQLParameterString(classification), S"')" );

		retClassificationID = MOG_DBAPI::QueryInt(selectCmd, S"ID");
		if(retClassificationID > 0)
		{
			MOG_ControllerSystem::GetDB()->GetDBCache()->GetClassificationCache()->AddSetToCache(retClassificationID, classification);
		}
	}
	return retClassificationID;
}

String *MOG_DBAssetAPI::GetClassificationFullTreeNameByID(int classificationID)
{
	String *retClassificationFullTreeName = MOG_ControllerSystem::GetDB()->GetDBCache()->GetClassificationCache()->GetNameFromID(classificationID);
	if(!retClassificationFullTreeName)
	{
		String *selectCmd;
		selectCmd = String::Concat( S"SELECT FullTreeName ",
									S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" ",
									S"WHERE (ID = ", classificationID.ToString(), S")" );

		retClassificationFullTreeName = MOG_DBAPI::QueryString(selectCmd, S"FullTreeName");
		if(retClassificationFullTreeName->Length)
		{
			MOG_ControllerSystem::GetDB()->GetDBCache()->GetClassificationCache()->AddSetToCache(classificationID, retClassificationFullTreeName);
		}
	}
	return retClassificationFullTreeName;
}

bool MOG_DBAssetAPI::DoesClassificationExistInAnyBranch(String *classification)
{
	// Get the classificationID
	int classificationID = GetClassificationID(classification);
	if( classificationID)
	{
		// Check if this is ever referenced in the branch links table?
		String *selectCmd = String::Concat( S"SELECT AssetClassificationsBranchLinks.ID ",
											S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsBranchLinksTable(), S" AssetClassificationsBranchLinks ",
											S"WHERE (AssetClassificationsBranchLinks.ClassificationID = '", classificationID.ToString(), S"') " );
		if(MOG_DBAPI::QueryInt(selectCmd, S"ID"))
		{
			return true;
		}
	}

	return false;
}

int MOG_DBAssetAPI::GetClassificationIDForBranch(String *classification)
{
	return GetClassificationIDForBranch(classification, NULL);
}

int MOG_DBAssetAPI::GetClassificationIDForBranch(String *classification, String *branchName)
{
	if( branchName == NULL || branchName->Length == 0)
	{
		branchName = MOG_ControllerProject::GetBranchName();
	}

	int retBranchID = MOG_ControllerSystem::GetDB()->GetDBCache()->GetClassificationBranchLinkCache()->GetIDFromName(classification, branchName);
	if(retBranchID == 0)
	{
		if( branchName != NULL && branchName->Length > 0)
		{
			String *selectCmd;
			selectCmd = String::Concat( S"SELECT AssetClassificationsBranchLinks.ID ",
										S"FROM ", MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches INNER JOIN ",
												MOG_ControllerSystem::GetDB()->GetAssetClassificationsBranchLinksTable(), S" AssetClassificationsBranchLinks ON ",
												S"Branches.ID = AssetClassificationsBranchLinks.BranchID INNER JOIN ",
												MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetClassificationsBranchLinks.ClassificationID = AssetClassifications.ID ",
										S"WHERE (AssetClassifications.FullTreeName = '", MOG_DBAPI::FixSQLParameterString(classification), S"') AND (Branches.BranchName = '", MOG_DBAPI::FixSQLParameterString(branchName), S"')" );

			retBranchID = MOG_DBAPI::QueryInt(selectCmd, S"ID");
			if(retBranchID > 0)
			{
				MOG_ControllerSystem::GetDB()->GetDBCache()->GetClassificationBranchLinkCache()->AddSetToCache(retBranchID, classification, branchName);
			}
		}
		else
		{
			retBranchID = -1;
		}
	}

	return retBranchID;
}

int MOG_DBAssetAPI::AddSubClassification(int parentID, String *name, String *fullTreeName, String *createdBy, String *createdDate, bool bAddToActiveBranch)
{
	int id = 0;

	String *selectCmd;
	String *insertCmd;

	selectCmd = String::Concat( S"SELECT ID ",
								S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" ",
								S"WHERE (ClassificationName = '", MOG_DBAPI::FixSQLParameterString(name), S"') AND ",
									S"(ParentID = ", Convert::ToString(parentID), S")" );

	id = MOG_DBAPI::QueryInt(selectCmd, S"ID");

	// if id is zero then the entry doesnt exist...add it
	if( id == 0 )
	{
// JohnRen - I was tempted to add this logic once before because this code could add a classification whose adam object doesn't match with the project but decided to wait until we track down another bug in the future...
//		String* thisAdamObject = MOG_Filename::GetClassificationAdamObjectName(name);
//		if (String::Compare(thisAdamObject, MOG_ControllerProject::GetProjectName(), true) == 0)
//		{
			int createdByID = MOG_DBProjectAPI::GetUserID(createdBy);
			if( createdDate == NULL )
			{
				createdDate = MOG_Time::GetVersionTimestamp();
			}

			insertCmd = String::Concat(S"INSERT INTO ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(),
									   S" ( ClassificationName, ParentID, FullTreeName, CreatedBy, CreatedDate ) VALUES ",
									   S" ('",MOG_DBAPI::FixSQLParameterString(name),S"',",__box(parentID),S",'",MOG_DBAPI::FixSQLParameterString(fullTreeName), S"',",__box(createdByID), S", '",createdDate , S"' )" );
			
			MOG_DBAPI::RunNonQuery(insertCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());

			id = MOG_DBAPI::QueryInt(selectCmd, S"ID");
//		}
//		else
//		{
//			String *message = String::Concat(	S"This classification doesn't start with the project's matching adam object!\n",
//												S"CLASSIFICATION: ", name);
//			MOG_Report::ReportMessage(S"Bogus Classification Being Added", message, Environment::StackTrace, PROMPT::MOG_ALERT_LEVEL::CRITICAL);
//		}
	}

	if( id != 0 )
	{
		// Check if we need to add this to the active branch?
		if (bAddToActiveBranch)
		{
			int branchID = MOG_DBProjectAPI::GetBranchIDByName(MOG_ControllerProject::GetBranchName());

			selectCmd = String::Concat( S"SELECT ID ",
										S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsBranchLinksTable(), S" ",
										S"WHERE (ClassificationID = ", Convert::ToString(id), S") AND ",
											S"(BranchID = ", Convert::ToString(branchID), S")" );

			int branchLinkID = MOG_DBAPI::QueryInt(selectCmd, S"ID");
			if( branchLinkID == 0 )
			{
				insertCmd = String::Concat(S"INSERT INTO ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsBranchLinksTable(),
										S" ( ClassificationID, BranchID ) VALUES ",
										S" (",__box(id) ,S",",__box(branchID),S" )" );

				MOG_DBAPI::RunNonQuery(insertCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
			}
		}
	}

	// return the id of this classification
	return id;
}

ArrayList *MOG_DBAssetAPI::GetClassificationChildren(String *classificationParent)
{
	return GetClassificationChildren(classificationParent, NULL);
}

ArrayList *MOG_DBAssetAPI::GetClassificationChildren(String *classificationParent, String *branchName)
{
	int parentID = 0;
	if( classificationParent != NULL && classificationParent->Length > 0 )
	{
		parentID = GetClassificationID(classificationParent);

		// if we failed looking up the sub tree...and this is not supposed to be the root case...return null
		if( parentID == 0 )
		{
			return NULL;
		}
	}

	if( branchName == NULL || branchName->Length == 0)
	{
		branchName = MOG_ControllerProject::GetBranchName();
	}

	String *selectCmd = String::Concat( S"SELECT AssetClassifications.ClassificationName ",
										S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetAssetClassificationsBranchLinksTable(), S" AssetClassificationsBranchLinks ON ",
											S"AssetClassifications.ID = AssetClassificationsBranchLinks.ClassificationID INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches ON AssetClassificationsBranchLinks.BranchID = Branches.ID ",
										S"WHERE (AssetClassifications.ParentID = ", Convert::ToString(parentID), S") AND ",
											S"(Branches.BranchName = '", MOG_DBAPI::FixSQLParameterString(branchName), S"')" );

	return MOG_DBAPI::QueryStrings(selectCmd, S"ClassificationName");
}

ArrayList *MOG_DBAssetAPI::GetClassificationChildren(String *classificationParent, String *branchName, ArrayList *properties)
{
	return GetClassificationChildren(classificationParent, branchName, properties, false);
}

ArrayList *MOG_DBAssetAPI::GetClassificationChildren(String *classificationParent, String *branchName, ArrayList *properties, bool bUseNotEqualComparison)
{
	int parentID = 0;
	if( classificationParent != NULL && classificationParent->Length > 0 )
	{
		parentID = GetClassificationID(classificationParent);

		// if we failed looking up the sub tree...and this is not supposed to be the root case...return null
		if( parentID == 0 )
		{
			return NULL;
		}
	}

	if( branchName == NULL || branchName->Length == 0)
	{
		branchName = MOG_ControllerProject::GetBranchName();
	}
	String *selectCmd = String::Concat(
		S"SELECT AssetClassifications.ClassificationName ",
		S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetClassificationsBranchLinksTable(), S" ACBL ON AssetClassifications.ID = ACBL.ClassificationID INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches ON ACBL.BranchID = Branches.ID " );

	// add in as many property tables as we need
	for(int i = 0; i < properties->Count; i++)
	{
		String *num = Convert::ToString(i);

		selectCmd = String::Concat( selectCmd, 
			S"INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetClassificationsPropertiesTable(),
			S" Prop", num, S" ON ACBL.ID = Prop", num, S".AssetClassificationBranchLinkID " );
	}

	// add WHERE clause
	selectCmd = String::Concat( selectCmd, 
		S"WHERE (Branches.BranchName = '", MOG_DBAPI::FixSQLParameterString(branchName), S"') AND ",
			S"(AssetClassifications.ParentID = ", Convert::ToString(parentID), S") " );
	
	// tack on any property WHERE stuff
	for(int i = 0; i < properties->Count; i++)
	{
		String *num = Convert::ToString(i);
		MOG_Property *theProp = __try_cast<MOG_Property *>(properties->Item[i]);

		String* value = MOG_DBAssetAPI::PrepareValueForSQLComparison(theProp->mPropertyValue);
		String* comparison = GetProperSQLComparisonString(value, bUseNotEqualComparison);

		selectCmd = String::Concat( selectCmd, 
			S"AND (Prop", num, S".Section = '", MOG_DBAPI::FixSQLParameterString(theProp->mSection), S"') ",
			S"AND (Prop", num, S".Property = '", MOG_DBAPI::FixSQLParameterString(theProp->mKey), S"') ",
			S"AND (Prop", num, S".Value ", comparison, S" '", MOG_DBAPI::FixSQLParameterString(value), S"') " );
	}

	return MOG_DBAPI::QueryStrings(selectCmd, S"ClassificationName");
}

ArrayList *MOG_DBAssetAPI::GetArchivedClassificationChildren(String *classificationParent)
{
	int parentID = 0;
	if( classificationParent != NULL && classificationParent->Length > 0 )
	{
		parentID = GetClassificationID(classificationParent);

		// if we failed looking up the sub tree...and this is not supposed to be the root case...return null
		if( parentID == 0 )
		{
			return NULL;
		}
	}
	String *selectCmd = String::Concat(
		S"SELECT ClassificationName ",
		S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ",
		S"WHERE (ParentID = ", Convert::ToString(parentID), S")" );

	return MOG_DBAPI::QueryStrings(selectCmd, S"ClassificationName");
}

ArrayList *MOG_DBAssetAPI::GetAllClassifications()
{
	String *selectCmd = String::Concat(
		S"SELECT FullTreeName ",
		S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ",
		S"ORDER BY FullTreeName" );

	return MOG_DBAPI::QueryStrings(selectCmd, S"FullTreeName");
}

ArrayList *MOG_DBAssetAPI::GetAllActiveClassifications()
{
	String *selectCmd = String::Concat(
		S"SELECT AssetClassifications.FullTreeName ",
		S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetClassificationsBranchLinksTable(), S" ACBL ON AssetClassifications.ID = ACBL.ClassificationID INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches ON ACBL.BranchID = Branches.ID ",
		S"WHERE (Branches.BranchName = '", MOG_DBAPI::FixSQLParameterString(MOG_ControllerProject::GetBranchName()), S"') ",
		S"ORDER BY AssetClassifications.FullTreeName" );

	return MOG_DBAPI::QueryStrings(selectCmd, S"FullTreeName");
}

ArrayList *MOG_DBAssetAPI::GetAllActiveClassifications( MOG_Property *property)
{
	return GetAllActiveClassifications(property, false);
}

ArrayList *MOG_DBAssetAPI::GetAllActiveClassifications( MOG_Property *property, bool bUseNotEqualComparison)
{
	String *branchName = MOG_ControllerProject::GetBranchName();
	String *selectString = String::Concat(
		S"SELECT AssetClassifications.FullTreeName ",
		S"FROM ", MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetClassificationsBranchLinksTable(), S" ACBL ON Branches.ID = ACBL.BranchID INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetClassificationsPropertiesTable(), S" ACP ON ACBL.ID = ACP.AssetClassificationBranchLinkID INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON ACBL.ClassificationID = AssetClassifications.ID ",
		S"WHERE (Branches.BranchName = '", MOG_DBAPI::FixSQLParameterString(branchName), S"')");

	// Make sure we have a valid property?
	if (property)
	{
		// Check what parts of the property should be checked?
		if (property->mSection->Length)
		{
			String* value = MOG_DBAssetAPI::PrepareValueForSQLComparison(property->mSection);
			String* comparison = GetProperSQLComparisonString(value, false);
			selectString = String::Concat(	selectString,	S" AND (ACP.Section ", comparison, S" '", MOG_DBAPI::FixSQLParameterString(value), S"')");
		}
		if (property->mKey->Length)
		{
			String* value = MOG_DBAssetAPI::PrepareValueForSQLComparison(property->mKey);
			String* comparison = GetProperSQLComparisonString(value, false);
			selectString = String::Concat(	selectString,	S" AND (ACP.Property ", comparison, S" '", MOG_DBAPI::FixSQLParameterString(value), S"')");
		}
		if (property->mValue->Length)
		{
			String* value = MOG_DBAssetAPI::PrepareValueForSQLComparison(property->mValue);
			String* comparison = GetProperSQLComparisonString(value, bUseNotEqualComparison);
			selectString = String::Concat(	selectString,	S" AND (ACP.Value ", comparison, S" '", MOG_DBAPI::FixSQLParameterString(value), S"')");
		}
	}
	return MOG_DBAPI::QueryStrings( selectString, S"FullTreeName" );
}

ArrayList *MOG_DBAssetAPI::GetAllActiveClassifications( String *rootClassification, MOG_Property *property, bool bUseNotEqualComparison )
{
	String *branchName = MOG_ControllerProject::GetBranchName();
	String *selectString = String::Concat(
		S"SELECT AssetClassifications.FullTreeName ",
		S"FROM ", MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetClassificationsBranchLinksTable(), S" AS ACBL ON Branches.ID = ACBL.BranchID INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetClassificationsPropertiesTable(), S" AS  ACP ON ACBL.ID = ACP.AssetClassificationBranchLinkID INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AS AssetClassifications ON ACBL.ClassificationID = AssetClassifications.ID ",
		S"WHERE (Branches.BranchName = '", MOG_DBAPI::FixSQLParameterString(branchName), S"') AND (AssetClassifications.FullTreeName LIKE '",MOG_DBAPI::FixSQLParameterString(rootClassification),S"%') ");

	// Make sure we have a valid property?
	if (property)
	{
		// Check what parts of the property should be checked?
		if (property->mSection->Length)
		{
			String* value = MOG_DBAssetAPI::PrepareValueForSQLComparison(property->mSection);
			String* comparison = GetProperSQLComparisonString(value, false);
			selectString = String::Concat(selectString, S"AND (ACP.Section ", comparison, S" '", MOG_DBAPI::FixSQLParameterString(value), S"') ");
		}
		if (property->mKey->Length)
		{
			String* value = MOG_DBAssetAPI::PrepareValueForSQLComparison(property->mKey);
			String* comparison = GetProperSQLComparisonString(value, false);
			selectString = String::Concat(selectString, S"AND (ACP.Property ", comparison, S" '", MOG_DBAPI::FixSQLParameterString(value), S"') ");
		}
		if (property->mValue->Length)
		{
			String* value = MOG_DBAssetAPI::PrepareValueForSQLComparison(property->mValue);
			String* comparison = GetProperSQLComparisonString(value, bUseNotEqualComparison);
			selectString = String::Concat(	selectString,	S" AND (ACP.Value ", comparison, S" '", MOG_DBAPI::FixSQLParameterString(value), S"')");
		}
	}

	return MOG_DBAPI::QueryStrings( selectString, S"FullTreeName" );
}

ArrayList *MOG_DBAssetAPI::GetAllActiveClassificationsByRootClassification(String *rootClassification)
{
	ArrayList *classifications = new ArrayList();

	// Check to make sure this is a valid classification?
	int classificationID = GetClassificationIDForBranch(rootClassification);
	if (classificationID)
	{
		// Add this root classification so it will be included in the list
		classifications->Add(rootClassification);

		// Build a select string to obtain all the active children classifications of the specified root
		String *selectCmd = String::Concat(
				S"SELECT AssetClassifications.FullTreeName ",
				S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications INNER JOIN ",
					MOG_ControllerSystem::GetDB()->GetAssetClassificationsBranchLinksTable(), S" ACBL ON AssetClassifications.ID = ACBL.ClassificationID INNER JOIN ",
					MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches ON ACBL.BranchID = Branches.ID ",
				S"WHERE (Branches.BranchName = '", MOG_DBAPI::FixSQLParameterString(MOG_ControllerProject::GetBranchName()), S"') AND (AssetClassifications.FullTreeName LIKE '",MOG_DBAPI::FixSQLParameterString(rootClassification), S"~%') ",
				S"ORDER BY AssetClassifications.FullTreeName" );
		ArrayList *childrenClassifications = MOG_DBAPI::QueryStrings(selectCmd, S"FullTreeName");

		// Add the children classifications to the list
		classifications->AddRange(childrenClassifications);
	}

	return classifications;
}

bool MOG_DBAssetAPI::AddAssetClassificationProperty(String *classification, String *branch, String *section, String *property, String *value)
{
	return SQLCreateAssetClassificationProperty(classification, branch, section, property, value);
}

bool MOG_DBAssetAPI::AddAssetClassificationProperties(String *classification, String *branch, ArrayList *properties)
{
	bool bSuccess = true;
	// Foreach MOG_Property in the ArrayList, add it to our DB
	for (int i = 0; i < properties->Count; ++i)
	{
		MOG_Property *property = __try_cast<MOG_Property*>(properties->get_Item(i));
		// Add our ClassificationProperty, checking if we failed...
		if(!AddAssetClassificationProperty(classification, branch, property->mSection, property->mKey, property->mValue))
		{
			bSuccess = false;
		}
	}

	// Return the value of whether or not we failed (false if we failed)
	return bSuccess;
}

bool MOG_DBAssetAPI::RemoveAssetClassificationProperty(String *classification, String *branch, String *section, String *property)
{
	int classificationBranchID = GetClassificationIDForBranch(classification, branch);

	String *deleteCmd = String::Concat( S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsPropertiesTable(),
										S" WHERE (AssetClassificationBranchLinkID = ", Convert::ToString(classificationBranchID), S") AND ",
											S"(Section = '", MOG_DBAPI::FixSQLParameterString(section), S"') AND ",
											S"(Property = '", MOG_DBAPI::FixSQLParameterString(property), S"')" );
	
	return MOG_DBAPI::RunNonQuery(deleteCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
}

bool MOG_DBAssetAPI::RemoveAllAssetClassificationProperties(String *classification, String *branch)
{
	int classificationBranchID = GetClassificationIDForBranch(classification, branch);

	String *deleteCmd = String::Concat( S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsPropertiesTable(),
										S" WHERE (AssetClassificationBranchLinkID = ", Convert::ToString(classificationBranchID), S")" );
	
	return MOG_DBAPI::RunNonQuery(deleteCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
}

bool MOG_DBAssetAPI::RemoveAllAssetClassificationProperties(String *classification, String *branch, String *section)
{
	int classificationBranchID = GetClassificationIDForBranch(classification, branch);

	String *deleteCmd = String::Concat( S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsPropertiesTable(),
										S" WHERE (AssetClassificationBranchLinkID = ", Convert::ToString(classificationBranchID), S") AND ",
											S"(Section = '", MOG_DBAPI::FixSQLParameterString(section), S"')" );
	
	return MOG_DBAPI::RunNonQuery(deleteCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
}

String *MOG_DBAssetAPI::GetAssetClassificationProperty(String *classification, String *branch, String *section, String *property)
{
	int classificationBranchID = GetClassificationIDForBranch(classification, branch);

	String *selectCmd = String::Concat( S"SELECT Value ",
										S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsPropertiesTable(),
										S"WHERE (AssetClassificationBranchLinkID = ", Convert::ToString(classificationBranchID), S") AND ",
											S"(Section = '", MOG_DBAPI::FixSQLParameterString(section), S"') AND ",
											S"(Property = '", MOG_DBAPI::FixSQLParameterString(property), S"')" );

	return MOG_DBAPI::QueryString(selectCmd, "Value");
}

ArrayList *MOG_DBAssetAPI::GetAllAssetClassificationProperties(String *classification, String *branch)
{
	int classificationBranchID = GetClassificationIDForBranch(classification, branch);
	if(classificationBranchID > -1)
	{
		String *selectCmd = String::Concat( S"SELECT Section, Property, Value ",
											S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsPropertiesTable(),
											S"WHERE (AssetClassificationBranchLinkID = ", Convert::ToString(classificationBranchID), S")" );

		return QueryProperties(selectCmd);
	}
	return NULL;
}

ArrayList *MOG_DBAssetAPI::GetAllAssetClassificationProperties(String *classification, String *branch, String *section)
{
	int classificationBranchID = GetClassificationIDForBranch(classification, branch);

	String *selectCmd = String::Concat( S"SELECT Section, Property, Value ",
										S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsPropertiesTable(),
										S"WHERE (AssetClassificationBranchLinkID = ", Convert::ToString(classificationBranchID), S") AND ",
											S"(Section = '", MOG_DBAPI::FixSQLParameterString(section), S"')" );

	return QueryProperties(selectCmd);
}

ArrayList *MOG_DBAssetAPI::GetAllDerivedAssetClassificationProperties(String *classification, String *branch)
{
	// get correct branch
	if( branch == NULL || branch->Length == 0)
	{
		branch = MOG_ControllerProject::GetBranchName();
	}
	if(MOG_ControllerSystem::GetDB() && 
		MOG_ControllerSystem::GetDB()->GetAssetClassificationsBranchLinksTable() && 
		MOG_ControllerSystem::GetDB()->GetAssetClassificationsPropertiesTable() && 
		MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable() && 
		MOG_ControllerSystem::GetDB()->GetBranchesTable())
	{
	String *selectCmd = String::Concat(
		S"SELECT AC.FullTreeName, ACP.Section, ACP.Property, ACP.Value ",
		S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsBranchLinksTable(), S" ACBL INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetClassificationsPropertiesTable(), S" ACP ON ACBL.ID = ACP.AssetClassificationBranchLinkID INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AC ON ACBL.ClassificationID = AC.ID INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches ON ACBL.BranchID = Branches.ID ",
		S"WHERE (Branches.BranchName = '", MOG_DBAPI::FixSQLParameterString(branch), S"') " );
	
	// add all classifications
	ArrayList *classifications = new ArrayList();
	String* lineage[] = MOG_Filename::SplitClassificationString(classification);
	if( lineage != NULL && lineage->Count > 0 )
	{
		String *parentName = S"";

		// Iterate through each of the strings in our split lineage array
		for (int w = 0; w < lineage->Count; w++)
		{			
			// Get a parent name
			if (parentName->Length == 0)
			{
				parentName = String::Concat(parentName, lineage[w]);

				selectCmd = String::Concat( selectCmd, S"AND ( (AC.FullTreeName = '", MOG_DBAPI::FixSQLParameterString(parentName), S"') " );
			}
			else
			{
				// Re-create the full name to the class
				parentName = String::Concat(parentName, "~", lineage[w]);

				selectCmd = String::Concat( selectCmd, S"OR (AC.FullTreeName = '", MOG_DBAPI::FixSQLParameterString(parentName), S"') " );
			}

			MOG_ClassificationProperties *classProp = new MOG_ClassificationProperties();
			classProp->mClassification = parentName;
			classProp->mProperties = new ArrayList();
			classifications->Add(classProp);
		}
		selectCmd = String::Concat( selectCmd, S")" );
	}

	return QueryDerivedAssetClassificationProperties(selectCmd, classifications);
	}
	return NULL;
}

bool MOG_DBAssetAPI::AddSyncTargetFileLink(String *syncTargetFile, MOG_Filename *assetName, String *platform)
{
	bool bAdded = false;
	String *selectCmd;
	String *insertCmd;

	int assetVersionID = 0;

	// Check if the specified asset includes a revision?
	String *version = assetName->GetVersionTimeStamp();
	if (version->Length)
	{
		assetVersionID = GetAssetVersionID(assetName, version);
	}
	else
	{
		// At least attempt to current revision of this asset for our branch
		assetVersionID = GetCurrentAssetVersionID(assetName);
	}

	int platformID = GetPlatformID(platform);

	selectCmd = String::Concat( S"SELECT AssetVersionID ",
								S"FROM ", MOG_ControllerSystem::GetDB()->GetSyncTargetFileMapTable(), S" SyncTargetFileMap ",
								S"WHERE (SyncTargetFile = '", MOG_DBAPI::FixSQLParameterString(syncTargetFile), S"') AND ",
										S"(AssetVersionID = '", assetVersionID, S"') AND ",
										S"(PlatformID = '", platformID, S"')" );

	if( !MOG_DBAPI::QueryExists(selectCmd) )
	{
		// Add this new link
		insertCmd = String::Concat(S"INSERT INTO ", MOG_ControllerSystem::GetDB()->GetSyncTargetFileMapTable(),
								   S" ( SyncTargetFile, AssetVersionID, PlatformID ) VALUES ",
								   S" ('",MOG_DBAPI::FixSQLParameterString(syncTargetFile) ,S"',",__box(assetVersionID) ,S",",__box(platformID),S")" );
		bAdded = MOG_DBAPI::RunNonQuery(insertCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
	}
	else
	{
		// Looks like it is already there
		bAdded = true;
	}

	return bAdded;
}

bool MOG_DBAssetAPI::AddSyncTargetFileLinks(ArrayList *syncTargetFiles, MOG_Filename *assetName, String *platform)
{
	bool success = false;

	int assetVersionID = GetCurrentAssetVersionID(assetName);
	int platformID = GetPlatformID(platform);

	// make sure we have a valid asset version
	if( assetVersionID == 0 )
	{
		return success;
	}
	String *insertCmd = String::Concat(
		S"INSERT INTO ", MOG_ControllerSystem::GetDB()->GetSyncTargetFileMapTable(), S"(AssetVersionID, PlatformID, SyncTargetFile) ");
		
	for( int i = 0; i < syncTargetFiles->Count; i++ )
	{
		String *syncTargetFile = __try_cast<String*>(syncTargetFiles->Item[i]);
		if( i > 0 )
		{
			insertCmd = String::Concat(insertCmd, S" UNION ALL ");
		}
		insertCmd = String::Concat(insertCmd, S"SELECT ", Convert::ToString(assetVersionID), S", ",
					Convert::ToString(platformID), S", '", 
					MOG_DBAPI::FixSQLParameterString(syncTargetFile), S"' ");
	};
	return MOG_DBAPI::RunNonQuery(insertCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
}

bool MOG_DBAssetAPI::DoesSyncTargetFileLinkExistForAsset(MOG_Filename *assetName, String* platformName)
{
	int assetVersionID = GetAssetVersionID(assetName, assetName->GetVersionTimeStamp());
	int platformID = GetPlatformID(platformName);

	String *checkCmd = String::Concat( S"SELECT AssetVersionID FROM ", MOG_ControllerSystem::GetDB()->GetSyncTargetFileMapTable(),
										S" WHERE (AssetVersionID=", Convert::ToString(assetVersionID), S") AND ",
										S"       (PlatformID=", Convert::ToString(platformID), S") " );
	return MOG_DBAPI::QueryExists(checkCmd);
}

bool MOG_DBAssetAPI::RemoveAllSyncTargetFileLinksForAsset(MOG_Filename *assetName)
{
	int assetVersionID = GetAssetVersionID(assetName, assetName->GetVersionTimeStamp());

	if( assetVersionID == 0 )
	{
		return false;
	}

	String *deleteCmd = String::Concat( S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetSyncTargetFileMapTable(),
										S" WHERE (AssetVersionID=", Convert::ToString(assetVersionID), S")" );
	
	return	MOG_DBAPI::RunNonQuery(deleteCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
}

ArrayList *MOG_DBAssetAPI::GetSyncTargetFileAssetLinks(String *syncTargetFile, String *platformName)
{
	String *selectCmd = String::Concat( S"SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ",
										S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions ON AssetNames.ID = AssetVersions.AssetNameID INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID INNER JOIN (SELECT DISTINCT * FROM ",
											MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), S") AS BranchLinks ON AssetVersions.ID = BranchLinks.AssetVersionID INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches ON BranchLinks.BranchID = Branches.ID INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetSyncTargetFileMapTable(), S" SyncTargetFileMap ON AssetVersions.ID = SyncTargetFileMap.AssetVersionID INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetPlatformsTable(), S" Platforms ON SyncTargetFileMap.PlatformID = Platforms.ID ",
										S"WHERE (Branches.BranchName = '", MOG_DBAPI::FixSQLParameterString(MOG_ControllerProject::GetBranchName()), S"') AND ",
											S"(SyncTargetFileMap.SyncTargetFile = '", MOG_DBAPI::FixSQLParameterString(syncTargetFile), S"') ");
	// Check if there was a platform specified?
	if (platformName->Length)
	{
		selectCmd = String::Concat(selectCmd, S" AND (Platforms.PlatformName = '", MOG_DBAPI::FixSQLParameterString(platformName), S"')" );
	}

	ArrayList *assets = QueryAssets(selectCmd);

	// Remove any duplicates assets returned in this list...This happens for {All} assets that have links for all platforms
	assets = RemoveDuplicateAssetNamesFromList(assets);

	// Resolve any platform specific asset collisions that may be contained within the list of assets
	return MOG_ControllerProject::ResolveAssetNameListForPlatformSpecificAssets(assets, platformName);
}

ArrayList *MOG_DBAssetAPI::GetSyncTargetFileAssetLinksFileNameOnly(String *syncTargetFile, String *platformName)
{
	String *selectCmd = String::Concat( S"SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ",
										S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions ON AssetNames.ID = AssetVersions.AssetNameID INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID INNER JOIN (SELECT DISTINCT * FROM ",
											MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), S") AS BranchLinks ON AssetVersions.ID = BranchLinks.AssetVersionID INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches ON BranchLinks.BranchID = Branches.ID INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetSyncTargetFileMapTable(), S" SyncTargetFileMap ON AssetVersions.ID = SyncTargetFileMap.AssetVersionID INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetPlatformsTable(), S" Platforms ON SyncTargetFileMap.PlatformID = Platforms.ID ",
										S"WHERE (Branches.BranchName = '", MOG_DBAPI::FixSQLParameterString(MOG_ControllerProject::GetBranchName()), S"') AND ",
											S"(SyncTargetFileMap.SyncTargetFile LIKE '%\\", MOG_DBAPI::FixSQLParameterString(syncTargetFile), S"') ");
	// Check if there was a platform specified?
	if (platformName->Length)
	{
		selectCmd = String::Concat(selectCmd, S" AND (Platforms.PlatformName = '", MOG_DBAPI::FixSQLParameterString(platformName), S"')" );
	}

	ArrayList *assets = QueryAssets(selectCmd);

	// Remove any duplicates assets returned in this list...This happens for {All} assets that have links for all platforms
	assets = RemoveDuplicateAssetNamesFromList(assets);

	// Resolve any platform specific asset collisions that may be contained within the list of assets
	return MOG_ControllerProject::ResolveAssetNameListForPlatformSpecificAssets(assets, platformName);
}

ArrayList *MOG_DBAssetAPI::GetSyncTargetFileLinks(MOG_Filename *assetName, String *platformName)
{
	String *selectCmd = String::Concat( S"SELECT SyncTargetFileMap.SyncTargetFile ",
								S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames INNER JOIN ",
									MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions ON AssetNames.ID = AssetVersions.AssetNameID INNER JOIN ",
									MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID INNER JOIN (SELECT DISTINCT * FROM ",
									MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), S") AS BranchLinks ON AssetVersions.ID = BranchLinks.AssetVersionID INNER JOIN ",
									MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches ON BranchLinks.BranchID = Branches.ID INNER JOIN ",
									MOG_ControllerSystem::GetDB()->GetSyncTargetFileMapTable(), S" SyncTargetFileMap ON AssetVersions.ID = SyncTargetFileMap.AssetVersionID INNER JOIN ",
									MOG_ControllerSystem::GetDB()->GetPlatformsTable(), S" Platforms ON SyncTargetFileMap.PlatformID = Platforms.ID ",
								S"WHERE (AssetClassifications.FullTreeName = '", MOG_DBAPI::FixSQLParameterString(assetName->GetAssetClassification()), S"') AND ",
									S"(AssetNames.AssetPlatformKey = '", MOG_DBAPI::FixSQLParameterString(assetName->GetAssetPlatform()), S"') AND ",
									S"(AssetNames.AssetLabel = '", MOG_DBAPI::FixSQLParameterString(assetName->GetAssetLabel()), S"') AND ",
									S"(Branches.BranchName = '", MOG_DBAPI::FixSQLParameterString(MOG_ControllerProject::GetBranchName()), S"') ");
	// Check if there was a platform specified?
	if (platformName->Length)
	{
		selectCmd = String::Concat(selectCmd, S" AND (Platforms.PlatformName = '", MOG_DBAPI::FixSQLParameterString(platformName), S"')" );
	}

	return MOG_DBAPI::QueryStrings(selectCmd, S"SyncTargetFile");
}

ArrayList *MOG_DBAssetAPI::GetAllProjectSyncTargetFilesForPlatform(String *platformName)
{
	ArrayList *syncTargetFiles = new ArrayList();
	ArrayList *syncInfos = GetAllProjectSyncTargetInfosForPlatformExcludeLibrary(platformName);
	for (int i = 0; i < syncInfos->Count; i++)
	{
        MOG_DBSyncTargetInfo *syncInfo = __try_cast<MOG_DBSyncTargetInfo *>(syncInfos->Item[i]);
		syncTargetFiles->Add(syncInfo->mSyncTargetFile);
	}
	return syncTargetFiles;
}

ArrayList *MOG_DBAssetAPI::GetAllProjectSyncTargetInfosForPlatform( String *platformName )
{
	return GetAllProjectSyncTargetInfosForPlatform(platformName, "");
}

ArrayList *MOG_DBAssetAPI::GetAllProjectSyncTargetInfosForPlatformExcludeLibrary( String *platformName )
{
	String *excludeLibrary = String::Concat(MOG_ControllerProject::GetProjectName(), S"\\Library");
	return GetAllProjectSyncTargetInfosForPlatform(platformName, excludeLibrary);
}

ArrayList *MOG_DBAssetAPI::GetAllProjectSyncTargetInfosForPlatform( String *platformName, String *exclusion )
{
	String *selectString = String::Concat( S"SELECT     TOP 100 PERCENT SyncTargetFileMap.SyncTargetFile, ",
		S"AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, ",
        S"AssetVersions.Version ",
		S"FROM ", MOG_ControllerSystem::GetDB()->GetSyncTargetFileMapTable(), S" SyncTargetFileMap INNER JOIN ",
		MOG_ControllerSystem::GetDB()->GetPlatformsTable(), S" Platforms ON SyncTargetFileMap.PlatformID = Platforms.ID INNER JOIN ",
		MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions ON SyncTargetFileMap.AssetVersionID = AssetVersions.ID INNER JOIN (SELECT DISTINCT * FROM ",
		MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), S") AS BranchLinks ON AssetVersions.ID = BranchLinks.AssetVersionID INNER JOIN ",
		MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" Branches ON BranchLinks.BranchID = Branches.ID INNER JOIN ",
		MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S"AssetNames ON AssetVersions.AssetNameID = AssetNames.ID INNER JOIN ",
		MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID ",
		S"WHERE (Platforms.PlatformName = '", MOG_DBAPI::FixSQLParameterString(platformName), S"') AND (Branches.BranchName = '", MOG_DBAPI::FixSQLParameterString(MOG_ControllerProject::GetBranchName()), S"') ",
		S"ORDER BY SyncTargetFileMap.SyncTargetFile");

	String *ordinals[] = new String *[5];
		ordinals[0] = S"SyncTargetFile";
		ordinals[1] = S"AssetLabel";
		ordinals[2] = S"FullTreeName";
		ordinals[3] = S"AssetPlatformKey";
        ordinals[4] = S"Version";

	ArrayList *initialList = MOG_DBAPI::QueryStringsArray(selectString, ordinals );
	
	// Scan this initial list and build a platform-specific list (case insenitive)
    HybridDictionary *skipList = new HybridDictionary(true);
    for( int i = 0; i < initialList->Count; ++i )
    {
        // Get back our ordinals
        String *returnOrdinals[] = __try_cast<String *[]>(initialList->Item[i]);

		String *assetLabel = returnOrdinals[1];
		String *assetClassification = returnOrdinals[2];
		String *assetPlatform = returnOrdinals[3];

		// Check if this platform matches
		if (String::Compare(assetPlatform, platformName, true) == 0)
		{
			// Add the generic asset counterpart to our skipList because the platform-specific asset will override the generic asset
			MOG_Filename *genericAsset = MOG_Filename::CreateAssetName(assetClassification, S"All", assetLabel);
			skipList->Item[genericAsset->GetAssetFullName()] = returnOrdinals;
		}
	}

    ArrayList *returnList = new ArrayList( initialList->Count );
    for( int i = 0; i < initialList->Count; ++i )
    {
        // Get back our ordinals
        String *returnOrdinals[] = __try_cast<String *[]>(initialList->Item[i]);

        String *syncTargetFile = returnOrdinals[0];
        String *assetLabel = returnOrdinals[1];
        String *assetClassification = returnOrdinals[2];
        String *assetPlatform = returnOrdinals[3];
        String *version = returnOrdinals[4];

		// Check if we have an exclusion?
		if (exclusion->Length)
		{
			// Check if this matches the exclusion?
			if (syncTargetFile->StartsWith(exclusion, StringComparison::CurrentCultureIgnoreCase))
			{
				// Skip this syncTargetFile
				continue;
			}
		}

		// Check if this asset is listed in the skipList?
		MOG_Filename *assetFilename = MOG_Filename::CreateAssetName(assetClassification, assetPlatform, assetLabel);
		if (skipList->Contains(assetFilename->GetAssetFullName()))
		{
			continue;
		}

        // Create and add our MOG_DBSyncTargetInfo to our returnList
        MOG_DBSyncTargetInfo *info = new MOG_DBSyncTargetInfo( syncTargetFile, version, assetLabel, assetPlatform, assetClassification );
        returnList->Add( info );
    }

    return returnList;
}

ArrayList *MOG_DBAssetAPI::GetAllProjectSyncTargetFilesForDirectory(String *gameDataRoot, String *path, String *platformName)
{
	String *targetPath = path->ToLower()->Replace(gameDataRoot->ToLower(), "");

	String *selectCmd = String::Concat( S"SELECT SyncTargetFileMap.SyncTargetFile FROM ", 
		MOG_ControllerSystem::GetDB()->GetSyncTargetFileMapTable(), 
		S" SyncTargetFileMap INNER JOIN ",
		MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), 
		S" AS AV on  (AV.ID = SyncTargetFileMap.AssetVersionID) INNER JOIN ",
		MOG_ControllerSystem::GetDB()->GetAssetNamesTable(),
		S" as AN on (AN.ID = AV.AssetNameID) INNER JOIN (SELECT DISTINCT * FROM ",
		MOG_ControllerSystem::GetDB()->GetBranchLinksTable(),
		S") AS BranchLinks ON AV.ID = BranchLinks.AssetVersionID INNER JOIN ",
		MOG_ControllerSystem::GetDB()->GetBranchesTable(),
		S" Branches ON BranchLinks.BranchID = Branches.ID ",
		S" WHERE (Branches.BranchName = '",
		MOG_DBAPI::FixSQLParameterString(MOG_ControllerProject::GetBranchName()),
		S"') AND (AN.AssetPlatformKey = '",
		MOG_DBAPI::FixSQLParameterString(platformName), 
		S"') ", 
		S" UNION ",
		S" SELECT SyncTargetFileMap.SyncTargetFile FROM ",
		MOG_ControllerSystem::GetDB()->GetSyncTargetFileMapTable(),
		S" SyncTargetFileMap INNER JOIN ",
		MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(),
		S" AS AV on  (AV.ID = SyncTargetFileMap.AssetVersionID) INNER JOIN ",
		MOG_ControllerSystem::GetDB()->GetAssetNamesTable(),
		S" AS AN ON (AN.ID = AV.AssetNameID) INNER JOIN (SELECT DISTINCT * FROM ",
		MOG_ControllerSystem::GetDB()->GetBranchLinksTable(),
		S") AS BranchLinks ON AV.ID = BranchLinks.AssetVersionID	INNER JOIN ",
		MOG_ControllerSystem::GetDB()->GetBranchesTable(),
		S" Branches ON BranchLinks.BranchID = Branches.ID INNER JOIN ",
		MOG_ControllerSystem::GetDB()->GetPlatformsTable(),
		S" Platforms ON SyncTargetFileMap.PlatformID = Platforms.ID ",
		S" WHERE	(Platforms.PlatformName = '",MOG_DBAPI::FixSQLParameterString(platformName), S"') AND (Branches.BranchName = '", MOG_DBAPI::FixSQLParameterString(MOG_ControllerProject::GetBranchName()), S"') AND (AN.AssetPlatformKey = 'ALL')",
		S" AND ('(' + CAST(AssetClassificationID as varchar(10))+ ')' + AssetLabel) NOT IN ",
		S" (SELECT ('(' + CAST(AN.AssetClassificationID as varchar(10))+')' + AN.AssetLabel) FROM ",
		MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(),
		S" AS AV INNER JOIN ",
		MOG_ControllerSystem::GetDB()->GetAssetNamesTable(),
		S" AS AN ON (AN.ID = AV.AssetNameID) INNER JOIN (SELECT DISTINCT * FROM ",
		MOG_ControllerSystem::GetDB()->GetBranchLinksTable(),
		S") AS BranchLinks ON AV.ID = BranchLinks.AssetVersionID INNER JOIN ",
		MOG_ControllerSystem::GetDB()->GetBranchesTable(),
		S" Branches ON BranchLinks.BranchID = Branches.ID ",
		S" WHERE (Branches.BranchName = '", MOG_DBAPI::FixSQLParameterString(MOG_ControllerProject::GetBranchName()), S"') AND (AN.AssetPlatformKey = '",MOG_DBAPI::FixSQLParameterString(platformName) , S"'))");

	return MOG_DBAPI::QueryStrings(selectCmd, S"SyncTargetFile");
}

bool MOG_DBAssetAPI::AddAssetVersionProperty(MOG_Filename *assetName, String *assetVersion, String *section, String *property, String *value)
{
	return SQLCreateAssetProperty(assetName, assetVersion, section, property, value);
}

bool MOG_DBAssetAPI::AddAssetVersionProperties(MOG_Filename *assetName, String *assetVersion, ArrayList *properties)
{
	bool success = false;

	int assetVersionID = GetAssetVersionID(assetName, assetVersion);

	// make sure we have a valid asset version
	if( assetVersionID == 0 )
	{
		return success;
	}
		// insert all properties
		String *insertCmd = String::Concat(
			S"INSERT INTO ", MOG_ControllerSystem::GetDB()->GetAssetPropertiesTable(), S"(AssetVersionID, Section, Property, Value) ");
		
		for( int i = 0; i < properties->Count; i++ )
		{
			MOG_Property *prop = __try_cast<MOG_Property*>(properties->Item[i]);
			
			if( i > 0 )
			{
				insertCmd = String::Concat(insertCmd, S" UNION ALL ");
			}

			insertCmd = String::Concat(insertCmd, S"SELECT ", Convert::ToString(assetVersionID), S", '",
													MOG_DBAPI::FixSQLParameterString(prop->mSection), S"', '", 
													MOG_DBAPI::FixSQLParameterString(prop->mKey), S"', '", 
													MOG_DBAPI::FixSQLParameterString(prop->mPropertyValue), S"' ");
		}
		success = MOG_DBAPI::RunNonQuery(insertCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());

	return success;
}

bool MOG_DBAssetAPI::RemoveAssetVersionProperty(MOG_Filename *assetName, String *assetVersion, String *section, String *property)
{
	int assetVersionID = GetAssetVersionID(assetName, assetVersion);

	String *deleteCmd = String::Concat( S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetAssetPropertiesTable(),
										S" WHERE (AssetVersionID = ", Convert::ToString(assetVersionID), S") AND ",
											S"(Section = '", MOG_DBAPI::FixSQLParameterString(section), S"') AND ",
											S"(Property = '", MOG_DBAPI::FixSQLParameterString(property), S"')" );
	
	MOG_DBAPI::RunNonQuery(deleteCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());

	return true;
}

bool MOG_DBAssetAPI::RemoveAllAssetVersionProperties(MOG_Filename *assetName, String *assetVersion)
{
	int assetVersionID = GetAssetVersionID(assetName, assetVersion);

	String *deleteCmd = String::Concat( S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetAssetPropertiesTable(),
										S" WHERE (AssetVersionID = ", Convert::ToString(assetVersionID), S")" );
	
	return MOG_DBAPI::RunNonQuery(deleteCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
}

bool MOG_DBAssetAPI::RemoveAllAssetVersionProperties(MOG_Filename *assetName, String *assetVersion, String *section)
{
	int assetVersionID = GetAssetVersionID(assetName, assetVersion);

	String *deleteCmd = String::Concat( S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetAssetPropertiesTable(),
										S" WHERE (AssetVersionID = ", Convert::ToString(assetVersionID), S")");
	if (section->Length)
	{
		deleteCmd = String::Concat(deleteCmd, S" AND (Section = '", MOG_DBAPI::FixSQLParameterString(section), S"')" );
	}

	return MOG_DBAPI::RunNonQuery(deleteCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
}

String *MOG_DBAssetAPI::GetAssetVersionProperty(MOG_Filename *assetName, String *assetVersion, String *section, String *property)
{
	int assetVersionID = GetAssetVersionID(assetName, assetVersion);

	String *selectCmd = String::Concat( S"SELECT Value ",
										S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetPropertiesTable(),
										S"WHERE (AssetVersionID = ", Convert::ToString(assetVersionID), S") AND ",
											S"(Section = '", MOG_DBAPI::FixSQLParameterString(section), S"') AND ",
											S"(Property = '", MOG_DBAPI::FixSQLParameterString(property), S"')" );

	return MOG_DBAPI::QueryString(selectCmd, "Value");
}

ArrayList *MOG_DBAssetAPI::GetAllAssetVersionProperties(MOG_Filename *assetName, String *assetVersion)
{
	int assetVersionID = GetAssetVersionID(assetName, assetVersion);

	String *selectCmd = String::Concat( S"SELECT Section, Property, Value ",
										S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetPropertiesTable(),
										S"WHERE (AssetVersionID = ", Convert::ToString(assetVersionID), S")" );

	return QueryProperties(selectCmd);
}

ArrayList *MOG_DBAssetAPI::GetAllAssetVersionProperties(MOG_Filename *assetName, String *assetVersion, String *section)
{
	int assetVersionID = GetAssetVersionID(assetName, assetVersion);

	String *selectCmd = String::Concat( S"SELECT Section, Property, Value ",
										S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetPropertiesTable(),
										S"WHERE (AssetVersionID = ", Convert::ToString(assetVersionID), S") AND ",
											S"(Section = '", MOG_DBAPI::FixSQLParameterString(section), S"')" );

	return QueryProperties(selectCmd);
}

//This is the standard way to build an array list of MOG_DBAssetProperties to return to the report interface.
ArrayList *MOG_DBAssetAPI::GetAssetsWithProperties(String* selectString, ArrayList * properties)
{
	SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());
	ArrayList *assets = new ArrayList();
		
	SqlDataReader *myReader = NULL;
	try
	{
		myReader = MOG_DBAPI::GetNewDataReader(selectString, myConnection);
		while(myReader->Read())
		{
			assets->Add(SQLReadAssetWithProperties_UseFriendlyPropertyNames(myReader, properties));
		}
	}
	catch(Exception *e)
	{
		String *message = String::Concat(	S"Could not get records from SQL database!\n",
											S"MOG SERVER: ", MOG_ControllerSystem::GetServerComputerName());
		MOG_Report::ReportMessage(message, e->Message, e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
		return NULL;
	}
	__finally
	{
		if( myReader != NULL )
		{
			myReader->Close();
		}
		myConnection->Close();
	}

	return assets;
}

//This is the special way of genrating an array list of MOG_DBAssetProperties to return to the report interface tha allows us to exclude based on a property value.
ArrayList *MOG_DBAssetAPI::GetAssetsWithPropertiesFilterByPropertyValue(String* selectString, ArrayList * properties, String* rootNode, ArrayList *mogPropertiesToFilterOn, bool excludeValue)
{
	ArrayList *listOfClassificationsWithProperyObjects = new ArrayList();
	for(int i = 0; i < mogPropertiesToFilterOn->Count; i++)
	{
		MOG_Property *tp = __try_cast<MOG_Property*> (mogPropertiesToFilterOn->Item[i]);
		listOfClassificationsWithProperyObjects->Add(new MOG_DBClassificationsWithPropery(tp, rootNode));
	}
	//Build a filtered query that filters out all assets which have the value of a given property explicity set.  (This query returns everyting which is either null or has a value other than the exclude value)
	String *filteredQuery = MOG_DBQueryBuilderAPI::MOG_AssetQueries::Query_AssetsWithProperties_Filtered(selectString, mogPropertiesToFilterOn, excludeValue);
	//the rest of this is the same as the other GetAssetsWithProperties except we call the function to exclude items based on the property value.
	SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());
	ArrayList *assets = new ArrayList();

	SqlDataReader *myReader = NULL;
	try
	{
		myReader = MOG_DBAPI::GetNewDataReader(filteredQuery, myConnection);
		while(myReader->Read())
		{
			MOG_DBAssetProperties *apToAdd =  SQLReadAssetWithPropertiesWithInheritanceFilterByProperyValue(myReader, properties, mogPropertiesToFilterOn, excludeValue, rootNode, listOfClassificationsWithProperyObjects );
			//Only add items to the list if it is actually returned.  Null value indicates and excluded value.			
			if(apToAdd != NULL)
			{
				assets->Add(apToAdd);
			}
		}
	}
	catch(Exception *e)
	{
		String *message = String::Concat(	S"Could not get records from SQL database!\n",
											S"MOG SERVER: ", MOG_ControllerSystem::GetServerComputerName());
		MOG_Report::ReportMessage(message, e->Message, e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
		return NULL;
	}
	__finally
	{
		if( myReader != NULL )
		{
			myReader->Close();
		}
		myConnection->Close();
	}

	return assets;
}

ArrayList *MOG_DBAssetAPI::QueryDerivedAssetClassificationProperties(String *selectString, ArrayList *classifications)
{
	SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());

	SqlDataReader *myReader = NULL;
	try
	{
		myReader = MOG_DBAPI::GetNewDataReader(selectString, myConnection);
		
		while(myReader->Read())
		{
			String *className = MOG_DBAPI::SafeStringRead(myReader,S"FullTreeName")->Trim();
			String* lineage[] = MOG_Filename::SplitClassificationString(className);
			if( lineage != NULL && lineage->Count > 0 )
			{
				__try_cast<MOG_ClassificationProperties*>(classifications->Item[lineage->Count-1])->mProperties->Add(SQLReadProperty(myReader));
			}
		}
	}
	catch(Exception *e)
	{
		String *message = String::Concat(	S"Could not get records from SQL database!\n",
											S"MOG SERVER: ", MOG_ControllerSystem::GetServerComputerName());
		MOG_Report::ReportMessage(message, e->Message, e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
		return NULL;
	}
	__finally
	{
		if( myReader != NULL )
		{
			myReader->Close();
		}
		myConnection->Close();
	}

	//}
	return classifications;
}

MOG_DBAssetProperties *MOG_DBAssetAPI::SQLReadAssetWithProperties(SqlDataReader *myReader, ArrayList *properties)
{
	MOG_DBAssetProperties *assetProp = new MOG_DBAssetProperties();

	assetProp->mAsset = SQLReadAsset(myReader);
	assetProp->mProperties = new ArrayList();
	if(properties)
	{
		for( int i = 0; i < properties->Count; i++ )
		{
			String *columnName = String::Concat(S"PropValue", Convert::ToString(i));
			String *value = MOG_DBAPI::SafeStringRead(myReader,columnName)->Trim();
			if( value->Length > 0 )
			{
				// add property
				MOG_Property *theProp = __try_cast<MOG_Property *>(properties->Item[i])->Clone();
				theProp->SetValue(value);
				assetProp->mProperties->Add(theProp);
			}
		}
	}

	return assetProp;
}

//This is the standard way to build a MOG_DBAssetProperties where no exclusions are necesary.
MOG_DBAssetProperties *MOG_DBAssetAPI::SQLReadAssetWithProperties_UseFriendlyPropertyNames(SqlDataReader *myReader, ArrayList *properties)
{
	MOG_DBAssetProperties *assetProp = new MOG_DBAssetProperties();

	assetProp->mAsset = MOG_DBAssetAPI::SQLReadAsset(myReader);
	assetProp->mProperties = new ArrayList();

	for( int i = 0; i < properties->Count; i++ )
	{
		MOG_Property *theProp = __try_cast<MOG_Property *>(properties->Item[i]);
		String *columnName = theProp->mPropertyKey;
		String *value = MOG_DBAPI::SafeStringRead(myReader,columnName)->Trim();	
		if( value->Length > 0 )
		{
			// add property
			MOG_Property *theProp = __try_cast<MOG_Property *>(properties->Item[i])->Clone();
			theProp->SetValue(value);
			assetProp->mProperties->Add(theProp);
		}
	}

	return assetProp;
}

//This function builds a given MOG_DBAssetProperties.  It ensures that a given property has a value other than null.
//This function is also used to exclude assets that have a given property value by returning null if the item contains a property value to be excluded.
//myReader  this is a reader to use which is pointing at the appropriate row for the AssetProperties object.
//properties is a list of properties which need to be returned in the properties portion of the MOG_DBAssetProperties object.
//mogPropertiesWithValue is a list of properties with an attached value.  These properties are used to filter out inappropriate MOG_DBAssetProperties objects based on the propery value.
//excludeValue determines if the mogPropertiesWithValue are excluded or are the only values included when running this fucntion.
//rootClassfication  This is the rootClassification used to base this report on.
//listOfClassificationsWithProperyObjects  this is a ArrayList of MOG_DBClassificationsWithPropery objects used to dig through for the property values.
MOG_DBAssetProperties *MOG_DBAssetAPI::SQLReadAssetWithPropertiesWithInheritanceFilterByProperyValue(SqlDataReader *myReader, ArrayList *properties, ArrayList *mogPropertiesWithValue, bool excludeValue, String *rootClassfication, ArrayList *listOfClassificationsWithProperyObjects )
{
	//Create a new MOG_DBAssetProperties to return.
	MOG_DBAssetProperties *assetProp = new MOG_DBAssetProperties();
	//Attach the asset to the MOG_DBAssetProperties object.
	assetProp->mAsset = SQLReadAsset(myReader);
	//Initilize the array list to make sure we have the necessary arraylist to add to.
	assetProp->mProperties = new ArrayList();

	//Itterate through all the properties that need to be added to the mProperties object.
	for( int i = 0; i < properties->Count; i++ )
	{
		//Cast the property from the array list properties in to a real MOG_Property
		MOG_Property *theProp = __try_cast<MOG_Property *>(properties->Item[i]);
		//Get the columnName from the property
		String *columnName = theProp->mPropertyKey;
		//get the value of the property.
		String *value = MOG_DBAPI::SafeStringRead(myReader,columnName)->Trim();
		//Check to see if we have and actual value.
		if(value == NULL || value->Length == 0)
		{
			//if we don't have an actual value   Itterate through the list of properties to filter on.
			for(int ii = 0; ii < mogPropertiesWithValue->Count; ii++)
			{
				//Get the property to exclude on
				MOG_Property *exinProperty = __try_cast <MOG_Property *>(mogPropertiesWithValue->Item[ii]);
				//if the Property we are reading from is the same as one of the properties to exclude on.
				if(theProp->mPropertyKey == exinProperty->mPropertyKey)
				{
					//Get a MOG_DBClassificationsWithPropery to search with from the list of MOG_DBClassificationsWithPropery objects passed in via listOfClassificationsWithProperyObjects
					MOG_DBClassificationsWithPropery *searcherToUse = NULL;
					for(int iii = 0; iii < listOfClassificationsWithProperyObjects->Count; iii++)
					{
						//Cast the object to a MOG_DBClassificationsWithPropery
						searcherToUse = __try_cast <MOG_DBClassificationsWithPropery*> (listOfClassificationsWithProperyObjects->Item[iii]);
						//Check to see if this is the right searcher to use
						if(searcherToUse->mProperyName == theProp->mPropertyKey)
						{
							//Once we find the right searcher get the actual property value.
							value = searcherToUse->GetPropertyValueForClosestClassification(assetProp->mAsset->GetAssetClassification());
							//If we are trying to exclude this value.
							if(excludeValue)
							{
								//And the value passed via the exinProperty and the value returned from the searcher match don't return a valid MOG_DBAssetProperties
								if(String::Compare(value, exinProperty->mValue, true) == 0)
								{
									return NULL;
								}
							}
							//if we are including on this asset send back null if the values don't match.
							else
							{
								if(String::Compare(value, exinProperty->mValue, true) != 0)
								{
									return NULL;
								}
							}
						}
					}
				}
			}
		}
		//If we get past exclusion set all necessary values for the property and add it to the property list.
		if( value->Length > 0 )
		{
			// add property
			MOG_Property *theProp = __try_cast<MOG_Property *>(properties->Item[i])->Clone();
			theProp->SetValue(value);
			assetProp->mProperties->Add(theProp);
		}
	}
	//If we get past exclusion on all properties send back the MOG_DBAssetProperties we have created.
	return assetProp;
}

ArrayList *MOG_DBAssetAPI::QueryAssetsWithProperties(String* selectString, ArrayList *properties)
{
	SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());
	ArrayList *assets = new ArrayList();

		SqlDataReader *myReader = NULL;
		try
		{
			myReader = MOG_DBAPI::GetNewDataReader(selectString, myConnection);
			while(myReader->Read())
			{
				assets->Add(SQLReadAssetWithProperties(myReader, properties));
			}
		}
		catch(Exception *e)
		{
			String *message = String::Concat(	S"Could not get records from SQL database!\n",
												S"MOG SERVER: ", MOG_ControllerSystem::GetServerComputerName());
			MOG_Report::ReportMessage(message, e->Message, e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
			return NULL;
		}
		__finally
		{
			if( myReader != NULL )
			{
				myReader->Close();
			}
			myConnection->Close();
		}

	return assets;
}

MOG_DBSourceFile *MOG_DBAssetAPI::SQLReadFullSourceFile(SqlDataReader *myReader)
{
	MOG_DBSourceFile * sourceFile = new MOG_DBSourceFile();

	sourceFile->mSourceFile = MOG_DBAPI::SafeStringRead(myReader,S"SourceFilename")->Trim();
	sourceFile->mSourcePath = MOG_DBAPI::SafeStringRead(myReader,S"SourcePath")->Trim();

	return sourceFile;
}

ArrayList *MOG_DBAssetAPI::QueryFullSourceFileLinks(String *selectString)
{
	SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());
	ArrayList *sourceFiles = new ArrayList();

	SqlDataReader *myReader = NULL;
	try
	{
		myReader = MOG_DBAPI::GetNewDataReader(selectString, myConnection);
		while(myReader->Read())
		{
			sourceFiles->Add(SQLReadFullSourceFile(myReader));
		}
	}
	catch(Exception *e)
	{
		String *message = String::Concat(	S"Could not get records from SQL database!\n",
											S"MOG SERVER: ", MOG_ControllerSystem::GetServerComputerName());
		MOG_Report::ReportMessage(message, e->Message, e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
		return NULL;
	}
	__finally
	{
		if( myReader != NULL )
		{
			myReader->Close();
		}
		myConnection->Close();
	}

	return sourceFiles;
}

bool MOG_DBAssetAPI::DoesClassificationMatch(String *classification, Hashtable* classHash)
{
	// this function is used to determine if a classification is "matching" according to the classHash

	// first see if that classification is already in the hash
	if( classHash->Contains(classification) )
	{
		// return the cached results
		return (classHash->Item[classification] != NULL);
	}
	else
	{
		// find this classifications match...filling in the hash along the way
		bool matches = false;

		String* delimStr = S"~";
		Char delimiter[] = delimStr->ToCharArray();
		String* parts[] = classification->Split(delimiter);
		String* fullTree = S"";

		for( int i = 0; i < parts->Count; i++ )
		{
			fullTree = String::Concat( fullTree, parts[i] );

			if( classHash->Contains(fullTree) )
			{
				// set matches to existing data
				matches = (classHash->Item[fullTree] != NULL);
			}
			else
			{
				// add in the info for this classification
				classHash->Add(fullTree, matches ? fullTree : NULL);
			}

			fullTree = String::Concat( fullTree, S"~" );
		}

		return matches;
	}
}


ArrayList *MOG_DBAssetAPI::RemoveDuplicateAssetNamesFromList(ArrayList *assets)
{
	ArrayList *tempList = new ArrayList();
	if (assets)
	{
		tempList->AddRange(assets);
	}

	// Scan all the assets in the list...
	for(int i = 0; i < tempList->Count - 1; i++)
	{
		MOG_Filename *assetFilename = __try_cast<MOG_Filename*>(tempList->Item[i]);

		// Scan forward to see if this asset is duplicated?...Don't automatically increment because the delete will collapse the array around us
		for(int d = i + 1; d < tempList->Count; /* d++ */)
		{
			MOG_Filename *dupFilename = __try_cast<MOG_Filename*>(tempList->Item[d]);

			// Check if dupFilename is a duplicate of assetFilename?
			if (String::Compare(dupFilename->GetAssetFullName(), assetFilename->GetAssetFullName(), true) == 0)
			{
				// Remove this duplicate
				tempList->RemoveAt(d);
			}
			else
			{
				d++;
			}
		}
	}

	return tempList;
}


String* MOG_DBAssetAPI::PrepareValueForSQLComparison(String* value)
{
	// Replace any '*' wildcards with the SQL '%' wildcard
	return value->Replace("*", "%");
}

String* MOG_DBAssetAPI::GetProperSQLComparisonString(String* value, bool bUseNotEqualComparison)
{
	String* comparison = "=";

	// Check if this value contains '*' or '%'
	if (value->Contains(S"%"))
	{
		if (bUseNotEqualComparison)
		{
			comparison = S"NOT LIKE";
		}
		else
		{
			comparison = S"LIKE";
		}
	}
	else
	{
		if (bUseNotEqualComparison)
		{
			comparison = S"<>";
		}
		else
		{
			comparison = S"=";
		}
	}

	return comparison;
}

//*****************************
MOG_DBSyncTargetInfo::MOG_DBSyncTargetInfo( String *syncTargetFile, String *version, String *assetLabel, String *assetPlatform, String *assetClassification )
{
    mSyncTargetFile = syncTargetFile;
    mVersion = version;
    mAssetLabel = assetLabel;
    mAssetPlatform = assetPlatform;
    mAssetClassification = assetClassification;
    this->mFilenameOnly = NULL;
    this->mPath = NULL;
    InitializeFilenameAndPath();
}

void MOG_DBSyncTargetInfo::InitializeFilenameAndPath()
{
    // If we have valid information
    if( mSyncTargetFile && mSyncTargetFile->Length > 0 )
    {
        // Get the index of our last backslash
        int lastIndexOfBackSlash = mSyncTargetFile->LastIndexOf(S"\\");
        // If we found a last backslash, we will Substring up to that backslash to get our "gamedataDirectory"
        String *gamedataDirectory = (lastIndexOfBackSlash > -1 ) ? mSyncTargetFile->Substring(0, lastIndexOfBackSlash) : S"";
        // Replace what we recieved from our gamedataDirectory+"\\" with empty String
        this->mFilenameOnly = mSyncTargetFile->Replace( String::Concat( gamedataDirectory, S"\\" ), S"");

        // Trim any initial backslash from gamedataDirectory
        gamedataDirectory = gamedataDirectory->Trim( (S"\\")->ToCharArray() );

		// If we have a directory, add a separator and replace any backslashes with separators
		if( gamedataDirectory->Length > 0 )
		{
            this->mPath = gamedataDirectory->Replace( "\\", "~" );
		}
        else
        {
            this->mPath = S"";
        }
    }
    else
    {
        this->mFilenameOnly = S"";
        this->mPath = S"";
    }

}

String *MOG_DBSyncTargetInfo::get_FilenameOnly()
{
    return mFilenameOnly;
}

String *MOG_DBSyncTargetInfo::get_Path()
{
    return mPath;
}
//*****************************
