//--------------------------------------------------------------------------------
//	MOG_Database.cpp
//	
//	
//--------------------------------------------------------------------------------
#include "stdafx.h"

#include "MOG_Report.h"
#include "MOG_Time.h"
#include "MOG_ControllerSystem.h"
#include "MOG_ControllerProject.h"
#include "MOG_ControllerPackage.h"
#include "MOG_DBSyncedDataAPI.h"
#include "MOG_DBInboxAPI.h"
#include "MOG_DBAssetAPI.h"

#include "MOG_DBProjectAPI.h"


ArrayList *MOG_DBProjectAPI::GetPlatformNames()
{
	if (MOG_DBAPI::TableExists(MOG_ControllerSystem::GetDB()->GetPlatformsTable()))
	{
		String *selectString = String::Concat(S"SELECT * FROM ", MOG_ControllerSystem::GetDB()->GetPlatformsTable());
		String *ordinal = "PlatformName";

		return MOG_DBAPI::QueryStrings(selectString, ordinal);
	}
	return new ArrayList();
}


ArrayList *MOG_DBProjectAPI::GetPlatformsInBranch(String *branchName)
{
	ArrayList *platforms = new ArrayList();

	int branchID = MOG_DBProjectAPI::GetBranchIDByName(branchName);
	if (branchID)
	{
		if (MOG_DBAPI::TableExists(MOG_ControllerSystem::GetDB()->GetPlatformsBranchLinksTable()))
		{
			String *selectString = String::Concat(	S"SELECT PlatformName ",
													S" FROM ", MOG_ControllerSystem::GetDB()->GetPlatformsBranchLinksTable(), S" INNER JOIN ",
													MOG_ControllerSystem::GetDB()->GetPlatformsTable(), S" ON ", MOG_ControllerSystem::GetDB()->GetPlatformsBranchLinksTable(), S".PlatformNameID = ", MOG_ControllerSystem::GetDB()->GetPlatformsTable(), S".ID",
													S" WHERE (BranchID = ", branchID.ToString(), S")");
			String *ordinal = "PlatformName";
			platforms = MOG_DBAPI::QueryStrings(selectString, ordinal);
		}
	}

	return platforms;
}


ArrayList *MOG_DBProjectAPI::GetDepartments()
{
	String *selectString = String::Concat(S"SELECT * FROM ", MOG_ControllerSystem::GetDB()->GetDepartmentsTable());
	String *ordinal = "DepartmentName";

	return MOG_DBAPI::QueryStrings(selectString, ordinal);
}


String *MOG_DBProjectAPI::GetDepartmentNameByID(int id)
{
	String *selectString = String::Concat(S"SELECT * FROM ", MOG_ControllerSystem::GetDB()->GetDepartmentsTable(), S" WHERE id='",Convert::ToString(id), S"'");
	String *ordinal = "DepartmentName";

	return MOG_DBAPI::QueryString(selectString, ordinal);
}

String *MOG_DBProjectAPI::GetDepartmentNameByUserName(String *userName)
{
	String *selectString = String::Concat(	S"SELECT DepartmentName",
											S" FROM ", MOG_ControllerSystem::GetDB()->GetUsersTable(), S" INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetDepartmentsTable(), S" ON ", MOG_ControllerSystem::GetDB()->GetUsersTable(), S".DepartmentID = ", MOG_ControllerSystem::GetDB()->GetDepartmentsTable(), S".ID",
											S" WHERE (UserName = '", MOG_DBAPI::FixSQLParameterString(userName), S"')");

	String *ordinal = "DepartmentName";

	return MOG_DBAPI::QueryString(selectString, ordinal);
}


int MOG_DBProjectAPI::GetDepartmentID(String *name)
{
	int retDepartmentID = MOG_ControllerSystem::GetDB()->GetDBCache()->GetDepartmentCache()->GetIDFromName(name);
	if(retDepartmentID == 0)
	{
		String *selectString = String::Concat(  S"SELECT * FROM ", MOG_ControllerSystem::GetDB()->GetDepartmentsTable(),
												S" WHERE DepartmentName='",MOG_DBAPI::FixSQLParameterString(name), S"'");
		String *ordinal = "ID";

		retDepartmentID = MOG_DBAPI::QueryInt(selectString, ordinal);
		if(retDepartmentID > 0)
		{
			MOG_ControllerSystem::GetDB()->GetDBCache()->GetDepartmentCache()->AddSetToCache(retDepartmentID, name);
		}
	}
	return retDepartmentID;
}

ArrayList *MOG_DBProjectAPI::GetDepartmentUsers(String *departmentName)
{
	int departmentID = GetDepartmentID(departmentName);
	String *selectString = String::Concat(	S"SELECT * FROM ", MOG_ControllerSystem::GetDB()->GetUsersTable(),
											S" WHERE DepartmentID='",Convert::ToString(departmentID), S"'");
	String *ordinal = "UserName";

	return MOG_DBAPI::QueryStrings(selectString, ordinal);
}


ArrayList *MOG_DBProjectAPI::GetActiveUserNames()
{
//McCoy - Exclude the removed users
	String *selectString = String::Concat(  S"SELECT * FROM ", MOG_ControllerSystem::GetDB()->GetUsersTable(),
											S"WHERE (RemovedDate = '')" );
	String *ordinal = "UserName";

	return MOG_DBAPI::QueryStrings(selectString, ordinal);
}


ArrayList *MOG_DBProjectAPI::GetAllUsers()
{
	String *selectString = String::Concat(S"SELECT * FROM ", MOG_ControllerSystem::GetDB()->GetUsersTable());
	String *ordinal = "UserName";

	return MOG_DBAPI::QueryStrings(selectString, ordinal);
}


String *MOG_DBProjectAPI::GetUserNameByID(int id)
{
	String *selectString = String::Concat(S"SELECT * FROM ", MOG_ControllerSystem::GetDB()->GetUsersTable(), S" WHERE ID='",Convert::ToString(id), S"'");
	String *ordinal = "UserName";

	return MOG_DBAPI::QueryString(selectString, ordinal);
}

bool MOG_DBProjectAPI::IsActiveUser(int userID)
{
	String *queryString = String::Concat(S"SELECT RemovedBy FROM ", MOG_ControllerSystem::GetDB()->GetUsersTable(), S" WHERE ID = ", Convert::ToString(userID));
	if(MOG_DBAPI::GetNewSecularValueAsInt(queryString, MOG_ControllerSystem::GetDB()->GetConnectionString()) != 0)
	{
		return false;
	}
	queryString = String::Concat(S"SELECT RemovedDate FROM ", MOG_ControllerSystem::GetDB()->GetUsersTable(), S" WHERE ID = ", Convert::ToString(userID));
	String *removedDate = MOG_DBAPI::GetNewSecularValueAsString(queryString, MOG_ControllerSystem::GetDB()->GetConnectionString());
	if(removedDate->Length > 0)
	{
		return false;
	}
	return true;
}

bool MOG_DBProjectAPI::ReactivateUser(int userID, int createdBy, String *createdDate)
{
	String *updateString = String::Concat(S"UPDATE ", MOG_ControllerSystem::GetDB()->GetUsersTable(), S" SET RemovedDate = '', RemovedBy = 0, CreatedDate = '", createdDate, S"', CreatedBy = ",Convert::ToString(createdBy) ,S"  WHERE ID = ", Convert::ToString(userID));
	return MOG_DBAPI::RunNonQuery(updateString, MOG_ControllerSystem::GetDB()->GetConnectionString());
}

int MOG_DBProjectAPI::GetUserID(String *name)
{
	//Try to get it from cache
	int cachedUserID = MOG_ControllerSystem::GetDB()->GetDBCache()->GetUserCache()->GetUserIDByName(name);
	//Do it the old way if we don't get it in cache
	if(cachedUserID == 0)
	{
		String *selectString = String::Concat(S"SELECT * FROM ", MOG_ControllerSystem::GetDB()->GetUsersTable(), S" WHERE UserName='",MOG_DBAPI::FixSQLParameterString(name), S"'");
		String *ordinal = "ID";

		cachedUserID = MOG_DBAPI::QueryInt(selectString, ordinal);
	}

	return cachedUserID;
}


MOG_DBBranchInfo::MOG_DBBranchInfo()
{
	mID = 0;
	mBranchName = "";
	mCreatedDate = "";
	mCreatedBy = "";
	mRemovedDate = "";
	mRemovedBy = "";
	mTag = false;
}

bool MOG_DBProjectAPI::CreateNewBranch(String *oldBranch, String *newBranch)
{
	bool success = false;

	if( oldBranch == NULL || oldBranch->Length == 0 ||
		newBranch == NULL || newBranch->Length == 0)
	{
		return success;
	}

	int oldBranchID = MOG_DBProjectAPI::GetBranchIDByName(oldBranch);
	int newBranchID = MOG_DBProjectAPI::GetBranchIDByName(newBranch);

	if( oldBranchID == 0 || newBranchID == 0 )
	{
		return success;
	}
	ArrayList *transactionCommands = new ArrayList();

	transactionCommands->Add(String::Concat(S"INSERT INTO ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsBranchLinksTable(),
												S" (ClassificationID, BranchID) ",
											S"SELECT ClassificationID, ", Convert::ToString(newBranchID), S" AS BranchID ",
											S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsBranchLinksTable(), S" ",
											S"WHERE (BranchID = ", Convert::ToString(oldBranchID), S")" ));

	transactionCommands->Add(String::Concat(S"INSERT INTO ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsPropertiesTable(),
												S" (AssetClassificationBranchLinkID, Section, Property, Value) ",
											S"SELECT NewBranchLinks.ID, ACP.Section, ACP.Property, ACP.Value ",
											S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsBranchLinksTable(), S" OldBranchLinks INNER JOIN ",
												MOG_ControllerSystem::GetDB()->GetAssetClassificationsPropertiesTable(), S" ACP ON OldBranchLinks.ID = ACP.AssetClassificationBranchLinkID INNER JOIN ",
												MOG_ControllerSystem::GetDB()->GetAssetClassificationsBranchLinksTable(), S" NewBranchLinks ON OldBranchLinks.ClassificationID = NewBranchLinks.ClassificationID ",
											S"WHERE (NewBranchLinks.BranchID = ", Convert::ToString(newBranchID), S") AND ",
												S"(OldBranchLinks.BranchID = ", Convert::ToString(oldBranchID), S")" ));

	transactionCommands->Add(String::Concat(S"INSERT INTO ", MOG_ControllerSystem::GetDB()->GetBranchLinksTable(),
												S" (AssetVersionID, BranchID) ",
											S"SELECT AssetVersionID, ", Convert::ToString(newBranchID), S" AS BranchID ",
											S"FROM ", MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), S" ",
											S"WHERE (BranchID = ", Convert::ToString(oldBranchID), S")" ));
	
	transactionCommands->Add(String::Concat(S"INSERT INTO ", MOG_ControllerSystem::GetDB()->GetPlatformsBranchLinksTable(),
												S" (PlatformNameID, BranchID) ",
											S"SELECT PlatformNameID, ", Convert::ToString(newBranchID), S" AS BranchID ",
											S"FROM ", MOG_ControllerSystem::GetDB()->GetPlatformsBranchLinksTable(), S" ",
											S"WHERE (BranchID = ", Convert::ToString(oldBranchID), S")" ));
	
	// Execute the entire transaction
	return MOG_DBAPI::ExecuteTransaction(transactionCommands);
}

bool MOG_DBProjectAPI::CreateNewBranch(MOG_ControllerSyncData* workspace, String *newBranch)
{
	bool success = false;

	if (workspace == NULL ||
		newBranch == NULL || newBranch->Length == 0)
	{
		return success;
	}

	// Get the LastUpdated time of this workspace
	String* lastUpdated = workspace->GetLastUpdatedTime();
	if (lastUpdated->Length > 0)
	{
		if (MOG_DBProjectAPI::CreateNewBranch(workspace->GetBranchName(), newBranch))
		{
			HybridDictionary* corrections = new HybridDictionary(true);
			String *skippedAssets = S"";

			// Compare all the currentAssets in the branch to check for any needed corrections based on the workspace's LastUpdated time
			ArrayList* currentAssets = MOG_DBAssetAPI::GetAllAssets(false, newBranch);
			HybridDictionary* currentAssetsLookup = MOG_LocalSyncInfo::ConvertFromArrayListToHybridDictionary(currentAssets);
			for (int a = 0; a < currentAssets->Count; a++)
			{
				MOG_Filename* assetFilename = dynamic_cast<MOG_Filename*>(currentAssets->Item[a]);
				if (String::Compare(assetFilename->GetVersionTimeStamp(), lastUpdated, true) > 0)
				{
					corrections->Item[assetFilename->GetAssetFullName()] = assetFilename;
				}
			}

			// Double check all the syncedAssets with the currentAssets of this branch to further check for any needed corrections
			ArrayList* syncedAssets = MOG_DBSyncedDataAPI::GetCurrentSyncedAssets(	MOG_ControllerSystem::GetComputerName(), workspace->GetProjectName(), workspace->GetPlatformName(), workspace->GetSyncDirectory(), workspace->GetUserName());
			for (int a = 0; a < syncedAssets->Count; a++)
			{
				MOG_Filename* syncedFilename = dynamic_cast<MOG_Filename*>(syncedAssets->Item[a]);
				MOG_Filename* currentFilename = dynamic_cast<MOG_Filename*>(currentAssetsLookup->Item[syncedFilename->GetAssetFullName()]);

				if (currentFilename == NULL ||
					String::Compare(currentFilename->GetVersionTimeStamp(), syncedFilename->GetVersionTimeStamp(), true) != 0)
				{
					corrections->Item[syncedFilename->GetAssetFullName()] = syncedFilename;
				}
			}

			// Scan the inbox checking for any additional corrections
			ArrayList *localAssets = MOG_DBInboxAPI::InboxGetLocalAssetList(workspace->GetSyncDirectory());
			for (int a = 0; a < localAssets->Count; a++)
			{
				MOG_DBInboxProperties *inboxProp = __try_cast<MOG_DBInboxProperties*>(localAssets->Item[a]);
				MOG_Properties *properties = new MOG_Properties(inboxProp->mProperties);

				// Check if this asset has a BlessedTime?
				if (properties->BlessedTime->Length)
				{
					// Check if this is a packaged asset?
					if (properties->IsPackagedAsset)
					{
						// Walk through all of our packages and make sure they are also revisioned
						ArrayList *packages = properties->GetPackages();
						if (packages && packages->Count)
						{
							// Walk through all the listed packages
							for (int packageIndex = 0; packageIndex < packages->Count; packageIndex++)
							{
								MOG_Property *packageAssignmentProperty = __try_cast<MOG_Property*>(packages->Item[packageIndex]);

								// Get the packageName in this package assignment
								String *packageName = MOG_ControllerPackage::GetPackageName(packageAssignmentProperty->mPropertyKey);
								// Add this package using this BlessedTime to the list of corrections
								MOG_Filename *packageAssetFilename = new MOG_Filename(packageName, properties->BlessedTime);
								corrections->Item[packageAssetFilename->GetAssetFullName()] = packageAssetFilename;
							}
						}
					}

					// Add this asset using this BlessedTime to the list of corrections
					MOG_Filename *assetFilename = new MOG_Filename(inboxProp->mAsset->GetAssetFullName(), properties->BlessedTime);
					corrections->Item[assetFilename->GetAssetFullName()] = assetFilename;
				}
				else
				{
					// Append this asset to the list of errors we want to inform the user about
					skippedAssets = String::Concat(skippedAssets, S"UNBLESSED: ", inboxProp->mAsset->GetAssetFullName(), S"\n");
				}
			}

			// Scan the list of corrections and account for any platform-specific assets
			// Get a new list so we can continue to add to our corrections list
			ArrayList *tempList = MOG_LocalSyncInfo::ConvertFromHybridDictionaryToArrayList(corrections);
			for (int a = 0; a < tempList->Count; a++)
			{
				MOG_Filename* assetFilename = dynamic_cast<MOG_Filename*>(tempList->Item[a]);
				MOG_Filename* genericAssetFilename = NULL;

				// Check if this is platform specific?
				if (assetFilename->IsPlatformSpecific())
				{
					// Create the platfrom generic asset name
					genericAssetFilename = MOG_Filename::CreateAssetName(assetFilename->GetAssetClassification(), S"All", assetFilename->GetAssetLabel());
					// Now recreate it so it will include the desired timestamp
					genericAssetFilename = new MOG_Filename(genericAssetFilename->GetAssetFullName(), assetFilename->GetVersionTimeStamp());
				}
				else
				{
					// Use this platform generic asset name as is
					genericAssetFilename = assetFilename;
				}

				// Check if this generic asset exists in the project?
				if (MOG_ControllerProject::DoesAssetExists(genericAssetFilename))
				{
					// Add the platform generic asset to our newcorrections
					corrections->Item[genericAssetFilename->GetAssetFullName()] = genericAssetFilename;
				}

				// Now get all of the applicable platform-specific names for this asset
				String* platforms[] = MOG_ControllerProject::GetAllApplicablePlaformsForAsset(genericAssetFilename->GetAssetFullName(), false);
				for (int p = 0; p < platforms->Count; p++)
				{
					// Create a platform-specific name for this asset
					MOG_Filename* platformAssetFilename = MOG_Filename::CreateAssetName(assetFilename->GetAssetClassification(), platforms[p], assetFilename->GetAssetLabel());
					// Now recreate it so it will include the desired timestamp
					platformAssetFilename = new MOG_Filename(platformAssetFilename->GetAssetFullName(), assetFilename->GetVersionTimeStamp());
					// Check if this platform-specific asset exists in the project?
					if (MOG_ControllerProject::DoesAssetExists(platformAssetFilename))
					{
						// Add the platform generic asset to our newcorrections
						corrections->Item[platformAssetFilename->GetAssetFullName()] = platformAssetFilename;
					}
				}
			}


			// Begin a database transaction
			ArrayList *transactionCommands = new ArrayList();
			int newBranchID = MOG_DBProjectAPI::GetBranchIDByName(newBranch);


			// Fixup all the needed corrections to this new branch
			IDictionaryEnumerator* correctionEnumerator = corrections->GetEnumerator();
			while ( correctionEnumerator->MoveNext() )
			{
				MOG_Filename* correctionFilename = dynamic_cast<MOG_Filename*>(correctionEnumerator->Value);
				MOG_Filename* currentFilename = dynamic_cast<MOG_Filename*>(currentAssetsLookup->Item[correctionFilename->GetAssetFullName()]);

				// Schedule the current in this branch for removal
				if (currentFilename != NULL)
				{
					// Obtain the assetVersionID for the current asset needing correction
					int currentAssetVersionID = MOG_DBAssetAPI::GetAssetVersionID(currentFilename, currentFilename->GetVersionTimeStamp());
					if (currentAssetVersionID)
					{
						// Remove the synced asset from the branch links
						transactionCommands->Add(String::Concat(S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), S" ",
																S"WHERE (BranchID = ", Convert::ToString(newBranchID), S") AND ",
																S"      (AssetVersionID = ", Convert::ToString(currentAssetVersionID), S") "));
					}
				}

				// Get all the revisions for this asset
				ArrayList* revisions = MOG_DBAssetAPI::GetAllAssetRevisions(correctionFilename);
				revisions->Sort();
				// Scan for the bext matching revision for the identified timestamp
				String* newTimestamp = "";
				for (int r = 0; r < revisions->Count; r++)
				{
					String* thisTimestamp = dynamic_cast<String*>(revisions->Item[r]);
					if (String::Compare(thisTimestamp, correctionFilename->GetVersionTimeStamp(), true) <= 0)
					{
						newTimestamp = thisTimestamp;
					}
				}
				// Check if we have a newTimestamp
				if (newTimestamp->Length > 0)
				{
					// Obtain the assetVersionID for the new asset needing correction
					int newAssetVersionID = MOG_DBAssetAPI::GetAssetVersionID(correctionFilename, correctionFilename->GetVersionTimeStamp());
					if (newAssetVersionID)
					{
						// Update the branch links with the local asset's version
						transactionCommands->Add(String::Concat(S"INSERT INTO ", MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), S" ",
																S" (AssetVersionID, BranchID) VALUES ",
																S"( ", Convert::ToString(newAssetVersionID), S", ", Convert::ToString(newBranchID), S" )"));
					}
				}
				else
				{
					// Append this asset to the list of errors we want to inform the user about
					skippedAssets = String::Concat(skippedAssets, S"UNPOSTED: ", correctionFilename->GetAssetFullName(), S"\n");
				}
			}

			// Execute the entire transaction
			if (MOG_DBAPI::ExecuteTransaction(transactionCommands))
			{
				// Check if we skipped any assets?
				if (skippedAssets->Length)
				{
					// Inform the user
					String* message = String::Concat(S"The following workspace assets were skipped and not included in the Branch/Tag because they haven't been blessed or posted.\n",
													 skippedAssets);
					MOG_Prompt::PromptMessage("Branch/Tag Warning", message);
				}

				return true;
			}
		}
		else
		{
			// We failed to create the new branch/tag
		}
	}
	else
	{
		// Inform the user
		String* message = String::Concat(S"The specified workspace hasn't ever been synced so there is nothing to tag.");
		MOG_Prompt::PromptMessage("Branch/Tag Failed", message);
	}

	return false;
}

MOG_DBBranchInfo *MOG_DBProjectAPI::SQLCreateBranch(String *branchName, String *createdBy, String *createdDate, bool tag)
{
	String *removedDate = "";
	String *removedBy = "";
	
	if( createdDate == NULL )
	{
		createdDate = MOG_Time::GetVersionTimestamp();
	}

	// Obtain the correct userID
	int userID = GetUserID(createdBy);
	if (userID == 0)
	{
		// Assume the 'Admin' user in the event we are about the fail.
		userID = 1;
	}

	// Build an SQL Insert statement string for all the input-form
	String *insertCmd = String::Concat( S"INSERT INTO ", MOG_ControllerSystem::GetDB()->GetBranchesTable(),
										S" (BranchName, CreatedDate, CreatedBy, RemovedDate, RemovedBy, Tag) VALUES",
										S" ('",MOG_DBAPI::FixSQLParameterString(branchName) , S"', '", createdDate , S"', ", __box(userID), S",'", removedDate, S"',",__box(0), S",'", tag.ToString(), S"')" );

	MOG_DBAPI::RunNonQuery(insertCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());

	// Create a MOG_DBBranchInfo to return back
	MOG_DBBranchInfo *branchInfo = new MOG_DBBranchInfo();
	branchInfo->mBranchName = branchName;
	branchInfo->mCreatedDate = createdDate;
	branchInfo->mCreatedBy = createdBy;
	branchInfo->mRemovedDate = removedDate;
	branchInfo->mRemovedBy = removedBy;
	branchInfo->mTag = tag;

	return branchInfo;
}


MOG_DBBranchInfo *MOG_DBProjectAPI::QueryBranch(String *selectString)
{
	SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());
	MOG_DBBranchInfo *branchInfo = NULL;

	try
	{
		SqlDataReader *myReader = MOG_DBAPI::GetNewDataReader(selectString, myConnection);
		while(myReader->Read())
		{
			branchInfo = SQLReadBranch(myReader);
			break;
		}
		myReader->Close();
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
		myConnection->Close();
	}

	return branchInfo;
}


ArrayList *MOG_DBProjectAPI::QueryBranches(String *selectString)
{
	SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());
	ArrayList *branches = NULL;

	try
	{
		branches = new ArrayList();

		SqlDataReader *myReader = MOG_DBAPI::GetNewDataReader(selectString, myConnection);
		while(myReader->Read())
		{
			branches->Add(SQLReadBranch(myReader));
		}
		myReader->Close();
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
		myConnection->Close();
	}

	return branches;
}


MOG_DBBranchInfo *MOG_DBProjectAPI::SQLReadBranch(SqlDataReader *myReader)
{
	MOG_DBBranchInfo *branchInfo = new MOG_DBBranchInfo();

	branchInfo->mID = myReader->GetInt32(myReader->GetOrdinal(S"ID"));
	branchInfo->mBranchName = myReader->GetString(myReader->GetOrdinal(S"BranchName"))->Trim();
	branchInfo->mCreatedDate = myReader->GetString(myReader->GetOrdinal(S"CreatedDate"))->Trim();
	
	if (!myReader->IsDBNull(myReader->GetOrdinal(S"CreatedBy")))
	{
		branchInfo->mCreatedBy = myReader->GetString(myReader->GetOrdinal(S"CreatedBy"))->Trim();
	}

	branchInfo->mRemovedDate = myReader->GetString(myReader->GetOrdinal(S"RemovedDate"))->Trim();
	
	if (!myReader->IsDBNull(myReader->GetOrdinal(S"RemovedBy")))
	{
		branchInfo->mRemovedBy = myReader->GetString(myReader->GetOrdinal(S"RemovedBy"))->Trim();
	}

	if (!myReader->IsDBNull(myReader->GetOrdinal(S"Tag")))
	{
		branchInfo->mTag = myReader->GetBoolean(myReader->GetOrdinal(S"Tag"));
	}

	return branchInfo;
}


bool MOG_DBProjectAPI::SQLWriteBranch(String *branchName, String *createdDate, String *createdBy, bool bTag)
{
	String *removedDate = "";
	String *removedBy = "";
	String *updateCmd = String::Concat( S"UPDATE ", MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" SET" );

	// Variables
	updateCmd = String::Concat(updateCmd, S" BranchName='", MOG_DBAPI::FixSQLParameterString(branchName), S"',");
	updateCmd = String::Concat(updateCmd, S" CreatedDate='", createdDate, S"',");
	updateCmd = String::Concat(updateCmd, S" CreatedBy='", MOG_DBAPI::FixSQLParameterString(createdBy), S"',");
	updateCmd = String::Concat(updateCmd, S" RemovedDate='", removedDate, S"',");
	updateCmd = String::Concat(updateCmd, S" RemovedBy='", MOG_DBAPI::FixSQLParameterString(removedBy), S"',");
	updateCmd = String::Concat(updateCmd, S" Tag='", bTag.ToString());

	// Where clause
	updateCmd = String::Concat(updateCmd, S" WHERE BranchName='", MOG_DBAPI::FixSQLParameterString(branchName), S"'");

	MOG_DBAPI::RunNonQuery(updateCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());

	return true;
}

bool MOG_DBProjectAPI::SQLUpdateBranch(String *branchName, MOG_DBBranchInfo * newBranch)
{
	if( newBranch == NULL )
		return false;

	String *updateCmd = String::Concat( S"UPDATE ", MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" SET" );

	// Variables
	updateCmd = String::Concat(updateCmd, S" BranchName='", MOG_DBAPI::FixSQLParameterString(newBranch->mBranchName), S"',");
	updateCmd = String::Concat(updateCmd, S" CreatedDate='", newBranch->mCreatedDate, S"',");
	updateCmd = String::Concat(updateCmd, S" CreatedBy=", Convert::ToString(GetUserID(newBranch->mCreatedBy)), S",");
	updateCmd = String::Concat(updateCmd, S" RemovedDate='", newBranch->mRemovedDate, S"',");
	updateCmd = String::Concat(updateCmd, S" RemovedBy=", Convert::ToString(GetUserID(newBranch->mRemovedBy)), S", ");
	updateCmd = String::Concat(updateCmd, S" Tag='", newBranch->mTag.ToString(), S"'");

	// Where clause
	updateCmd = String::Concat(updateCmd, S" WHERE BranchName='", MOG_DBAPI::FixSQLParameterString(branchName), S"'");

	return MOG_DBAPI::RunNonQuery(updateCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
}

String *MOG_DBProjectAPI::GetBranchNameByID(int id)
{
	String *branchName = "";

	// First check our cache?
	String *cachedValue = MOG_ControllerSystem::GetDB()->GetDBCache()->GetBranchCache()->GetNameFromID(id);
	if(cachedValue == NULL)
	{
		String *selectString = String::Concat( S"SELECT * FROM ", MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" WHERE id='", Convert::ToString(id), S"'" );
		String *ordinal = "BranchName";

		branchName = MOG_DBAPI::QueryString(selectString, ordinal);
		MOG_ControllerSystem::GetDB()->GetDBCache()->GetAssetNameCache()->AddSetToCache(id, branchName);
	}

	return branchName;
}


int MOG_DBProjectAPI::GetBranchIDByName(String *name)
{
	int retBranchNameID = MOG_ControllerSystem::GetDB()->GetDBCache()->GetBranchCache()->GetIDFromName(name);
	if(retBranchNameID == 0)
	{
		String *selectString = String::Concat( S"SELECT * FROM ", MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" WHERE BranchName='", MOG_DBAPI::FixSQLParameterString(name), S"'" );
		String *ordinal = "ID";

		retBranchNameID = MOG_DBAPI::QueryInt(selectString, ordinal);
		if(retBranchNameID > 0)
		{
			MOG_ControllerSystem::GetDB()->GetDBCache()->GetBranchCache()->AddSetToCache(retBranchNameID, name);
		}
	}
	return retBranchNameID;
}

ArrayList *MOG_DBProjectAPI::GetAllUnreferencedAssetVersions(String *assetName)
{
	String *selectString = String::Concat(
		S"SELECT AssetVersions.Version ",
		S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames INNER JOIN ",
				MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions ON AssetNames.ID = AssetVersions.AssetNameID LEFT OUTER JOIN (SELECT DISTINCT * FROM ",
				MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), S") AS BranchLinks ON AssetVersions.ID = BranchLinks.AssetVersionID ",
		S"WHERE (AssetNames.AssetName = '", MOG_DBAPI::FixSQLParameterString(assetName), S"') AND (BranchLinks.AssetVersionID IS NULL) ",
		S"ORDER BY AssetVersions.Version DESC" );

	String *ordinal = "Version";

	return MOG_DBAPI::QueryStrings(selectString, ordinal);
}

ArrayList *MOG_DBProjectAPI::GetActiveBranchNames()
{
	String *selectString = String::Concat(
		S"SELECT ", MOG_ControllerSystem::GetDB()->GetBranchesTable(), S".ID, ", MOG_ControllerSystem::GetDB()->GetBranchesTable(), S".BranchName, ", MOG_ControllerSystem::GetDB()->GetBranchesTable(), S".CreatedDate, Users1.UserName As CreatedBy, ",
					MOG_ControllerSystem::GetDB()->GetBranchesTable(), S".RemovedDate, Users2.UserName AS RemovedBy, ",
					MOG_ControllerSystem::GetDB()->GetBranchesTable(), S".Tag ",
		S"FROM ", MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" LEFT OUTER JOIN ",
				MOG_ControllerSystem::GetDB()->GetUsersTable(), S" Users1 ON ", MOG_ControllerSystem::GetDB()->GetBranchesTable(), S".CreatedBy = Users1.ID LEFT OUTER JOIN ",
				MOG_ControllerSystem::GetDB()->GetUsersTable(), S" Users2 ON ", MOG_ControllerSystem::GetDB()->GetBranchesTable(), S".RemovedBy = Users2.ID ",
		S"WHERE (", MOG_ControllerSystem::GetDB()->GetBranchesTable(), S".RemovedDate = '')" );

	return QueryBranches(selectString);
}

ArrayList *MOG_DBProjectAPI::GetAllTagNames()
{
	return MOG_DBProjectAPI::GetAllBranchNames(true);
}

ArrayList *MOG_DBProjectAPI::GetAllBranchNames()
{
	return MOG_DBProjectAPI::GetAllBranchNames(false);
}

ArrayList *MOG_DBProjectAPI::GetAllBranchNames(bool bTags)
{
	String *selectString = String::Concat(
		S"SELECT ", MOG_ControllerSystem::GetDB()->GetBranchesTable(), S".ID, ", MOG_ControllerSystem::GetDB()->GetBranchesTable(), S".BranchName, ", MOG_ControllerSystem::GetDB()->GetBranchesTable(), S".CreatedDate, Users1.UserName As CreatedBy, ",
					MOG_ControllerSystem::GetDB()->GetBranchesTable(), S".RemovedDate, Users2.UserName AS RemovedBy, ",
					MOG_ControllerSystem::GetDB()->GetBranchesTable(), S".Tag ",
		S"FROM ", MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" LEFT OUTER JOIN ",
				MOG_ControllerSystem::GetDB()->GetUsersTable(), S" Users1 ON ", MOG_ControllerSystem::GetDB()->GetBranchesTable(), S".CreatedBy = Users1.ID LEFT OUTER JOIN ",
				MOG_ControllerSystem::GetDB()->GetUsersTable(), S" Users2 ON ", MOG_ControllerSystem::GetDB()->GetBranchesTable(), S".RemovedBy = Users2.ID ");

	return QueryBranches(selectString);
}

// This is a special function that can obtain the branches of a project w/o being logged into the project
ArrayList *MOG_DBProjectAPI::GetAllBranchNames(String *projectName)
{
	// Use the project passed in because we need to be able to access this project's tables directly
	String *BranchesTable = String::Concat(S"[", projectName, S".Branches]");
	String *UsersTable = String::Concat(S"[", projectName, S".Users]");

	String *selectString = String::Concat(
		S"SELECT ", BranchesTable, S".ID, ", BranchesTable, S".BranchName, ", BranchesTable, S".CreatedDate, Users1.UserName As CreatedBy, ",
					BranchesTable, S".RemovedDate, Users2.UserName AS RemovedBy, ",
					BranchesTable, S".Tag ",
		S"FROM ", BranchesTable, S" LEFT OUTER JOIN ",
				UsersTable, S" Users1 ON ", BranchesTable, S".CreatedBy = Users1.ID LEFT OUTER JOIN ",
				UsersTable, S" Users2 ON ", BranchesTable, S".RemovedBy = Users2.ID ");

	return QueryBranches(selectString);
}

MOG_DBBranchInfo *MOG_DBProjectAPI::GetBranch(String *branchName)
{
	String *selectString = String::Concat(
		S"SELECT ", MOG_ControllerSystem::GetDB()->GetBranchesTable(), S".ID, ", MOG_ControllerSystem::GetDB()->GetBranchesTable(), S".BranchName, ", MOG_ControllerSystem::GetDB()->GetBranchesTable(), S".CreatedDate, Users1.UserName As CreatedBy, ",
					MOG_ControllerSystem::GetDB()->GetBranchesTable(), S".RemovedDate, Users2.UserName AS RemovedBy, ",
					MOG_ControllerSystem::GetDB()->GetBranchesTable(), S".Tag ",
		S"FROM ", MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" LEFT OUTER JOIN ",
				MOG_ControllerSystem::GetDB()->GetUsersTable(), S" Users1 ON ", MOG_ControllerSystem::GetDB()->GetBranchesTable(), S".CreatedBy = Users1.ID LEFT OUTER JOIN ",
				MOG_ControllerSystem::GetDB()->GetUsersTable(), S" Users2 ON ", MOG_ControllerSystem::GetDB()->GetBranchesTable(), S".RemovedBy = Users2.ID ",
		S"WHERE (BranchName = '", MOG_DBAPI::FixSQLParameterString(branchName), S"')" );

	return QueryBranch(selectString);
}


bool MOG_DBProjectAPI::AddBranch(String *branchName, String* createdBy, String *createdDate)
{
	return AddBranch(branchName, createdBy, createdDate, false);
}

bool MOG_DBProjectAPI::AddBranch(String *branchName, String* createdBy, String *createdDate, bool tag)
{
	// Check if this branch already exists?
	int branchID = GetBranchIDByName(branchName);
	if( branchID )
	{
		// Don't bother to add one that is already there!
		return false;
	}

	return (SQLCreateBranch(branchName, createdBy, createdDate, tag) != NULL);
}

bool MOG_DBProjectAPI::RemoveBranch(String *branchName, String* removedBy)
{
	// Never allow them to EVER do anything to 'Current'
	if (String::Compare(S"Current", branchName, true) == 0)
	{
		return false;
	}

	// Check if this branch already exists
	MOG_DBBranchInfo *branchInfo = GetBranch(branchName);
	if( branchInfo != NULL )
	{
		branchInfo->mRemovedBy = removedBy;
		branchInfo->mRemovedDate = MOG_Time::GetVersionTimestamp();
		
		if (SQLUpdateBranch(branchName, branchInfo))
		{
			return true;
		}
	}

	// Don't bother to remove one that is not already there!
	return false;
}

bool MOG_DBProjectAPI::RenameBranch(String *oldBranchName, String *newBranchName)
{
	// Never allow them to EVER do anything to 'Current'
	if (String::Compare(S"Current", oldBranchName, true) == 0 ||
		String::Compare(S"Current", newBranchName, true) == 0)
	{
		return false;
	}

	// JTM - TODO: check if newBranchName already exists? to avoid dups
	MOG_DBBranchInfo *branchInfo = GetBranch(oldBranchName);
	if( branchInfo != NULL )
	{
		branchInfo->mBranchName = newBranchName;

		if (SQLUpdateBranch(oldBranchName, branchInfo))
		{
			return true;
		}
	}

	return false;
}

bool MOG_DBProjectAPI::PurgeBranch(String *branchName)
{
	// Never allow them to EVER do anything to 'Current'
	if (String::Compare(S"Current", branchName, true) == 0)
	{
		return false;
	}

	if (branchName->Length)
	{
		// Get the branchID
		int branchID = GetBranchIDByName(branchName);
		if (branchID)
		{
			ArrayList *transactionCommands = new ArrayList();

			// Remove all the SyncedDataLinks
			transactionCommands->Add(String::Concat(S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetSyncedDataLinksTable(), S" ",
													S"WHERE     (SyncedDataLocationID IN ",
																S"(SELECT   ", MOG_ControllerSystem::GetDB()->GetSyncedDataLocationsTable(), S".ID ",
																S"FROM      ", MOG_ControllerSystem::GetDB()->GetSyncedDataLocationsTable(), S" INNER JOIN ",
																MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" ON ", MOG_ControllerSystem::GetDB()->GetSyncedDataLocationsTable(), S".BranchID = ", MOG_ControllerSystem::GetDB()->GetBranchesTable(), S".ID ",
																S"WHERE      (", MOG_ControllerSystem::GetDB()->GetBranchesTable(), S".ID = ", branchID, S"))) "));
			// Remove the SyncedDataLocations
			transactionCommands->Add(String::Concat(S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetSyncedDataLocationsTable(), S" ",
													S"WHERE (BranchID = ", Convert::ToString(branchID), S")" ));

			// Remove all of the classification properties for this branch
			transactionCommands->Add(String::Concat(S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsPropertiesTable(), S" ",
													S"WHERE     (AssetClassificationBranchLinkID IN ",
																S"(SELECT   ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsBranchLinksTable(), S".ID ",
																S"FROM      ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsBranchLinksTable(), S" INNER JOIN ",
																MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" ON ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsBranchLinksTable(), S".BranchID = ", MOG_ControllerSystem::GetDB()->GetBranchesTable(), S".ID ",
																S"WHERE      (", MOG_ControllerSystem::GetDB()->GetBranchesTable(), S".ID = ", branchID, S"))) "));
			// Remove all of the classifications for this branch
			transactionCommands->Add(String::Concat(S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsBranchLinksTable(), S" ",
													S"WHERE (BranchID = ", Convert::ToString(branchID), S")" ));
			// Remove all of the branchlinks for this branch
			transactionCommands->Add(String::Concat(S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), S" ",
													S"WHERE (BranchID = ", Convert::ToString(branchID), S")" ));
			// Remove all of the PlatformsBranchLinks for this branch
			transactionCommands->Add(String::Concat(S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetPlatformsBranchLinksTable(), S" ",
													S"WHERE (BranchID = ", Convert::ToString(branchID), S")" ));
			// Remove the branch
			transactionCommands->Add(String::Concat(S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetBranchesTable(), S" ",
													S"WHERE (ID = ", branchID, S")" ));

			// Execute the entire transaction
			if (MOG_DBAPI::ExecuteTransaction(transactionCommands))
			{
				// Flush all branch related caches
				MOG_ControllerSystem::GetDB()->GetDBCache()->GetBranchCache()->ClearCache();
				MOG_ControllerSystem::GetDB()->GetDBCache()->GetClassificationBranchLinkCache()->ClearCache();
				MOG_ControllerSystem::GetDB()->GetDBCache()->GetClassificationCache()->ClearCache();
				MOG_ControllerSystem::GetDB()->GetDBCache()->GetSyncedDataLocationsCache()->ClearCache();

// TODO -  This would be a good time to inform the server so it can force all users to log out of this branch

				return true;
			}
		}
	}

	return false;
}

MOG_DBPlatformInfo *MOG_DBProjectAPI::GetPlatform(String *platformName)
{
	String *selectString = String::Concat(
		S"SELECT * FROM ", MOG_ControllerSystem::GetDB()->GetPlatformsTable(), S" ",
		S"WHERE (Platform = '", MOG_DBAPI::FixSQLParameterString(platformName), S"')" );

	return QueryPlatform(selectString);
}

bool MOG_DBProjectAPI::AddPlatformName(String *platformName)
{
	return SQLCreatePlatformName(platformName);
}

bool MOG_DBProjectAPI::AddPlatformToBranch(String *platformName, String *branchName)
{
	return SQLCreatePlatformInBranch(platformName, branchName);
}

bool MOG_DBProjectAPI::RemovePlatformFromBranch(String *platformName, String* branchName)
{
	MOG_DBPlatformInfo *platformInfo = new MOG_DBPlatformInfo();
	platformInfo->mPlatformName = platformName;

	// Remove the actual platform name
	return SQLRemovePlatformFromBranch(platformInfo, branchName);
}

bool MOG_DBProjectAPI::RenamePlatform(String *oldPlatformName, String *newPlatformName)
{
	// JTM - TODO: check if newPlatformName already exists? to avoid dups
	MOG_DBPlatformInfo *platformInfo = GetPlatform(oldPlatformName);
	if( platformInfo != NULL )
	{
		platformInfo->mPlatformName = newPlatformName;

		return SQLUpdatePlatform(oldPlatformName, platformInfo);		
	}

	return false;
}

MOG_DBDepartmentInfo *MOG_DBProjectAPI::GetDepartment(String *departmentName)
{
	String *selectString = String::Concat(
		S"SELECT * FROM ", MOG_ControllerSystem::GetDB()->GetDepartmentsTable(), S" ",
		S"WHERE (DepartmentName = '", MOG_DBAPI::FixSQLParameterString(departmentName), S"')" );

	return QueryDepartment(selectString);
}

bool MOG_DBProjectAPI::AddDepartment(String *departmentName)
{
	// Check if this data already exists
	int departmentID = GetDepartmentID(departmentName);
	if( departmentID )
	{
		// Don't bother to add one that is already there!
		return false;
	}

	MOG_DBDepartmentInfo *departmentInfo = new MOG_DBDepartmentInfo();

	departmentInfo->mDepartmentName = departmentName;

	return (SQLCreateDepartment(departmentInfo) != NULL);
}

bool MOG_DBProjectAPI::RemoveDepartment(String *departmentName)
{
	MOG_DBDepartmentInfo *departmentInfo = new MOG_DBDepartmentInfo();

	departmentInfo->mDepartmentName = departmentName;

	return SQLRemoveDepartment(departmentInfo);
}

bool MOG_DBProjectAPI::RenameDepartment(String *oldDepartmentName, String *newDepartmentName)
{
	// JTM - TODO: check if newDepartmentName already exists? to avoid dups
	MOG_DBDepartmentInfo *departmentInfo = GetDepartment(oldDepartmentName);
	if( departmentInfo != NULL )
	{
		departmentInfo->mDepartmentName = newDepartmentName;

		return SQLUpdateDepartment(oldDepartmentName, departmentInfo);		
	}

	return false;
}

MOG_User *MOG_DBProjectAPI::GetUser(String *userName)
{
	String *selectString = String::Concat(
		S"SELECT ", MOG_ControllerSystem::GetDB()->GetUsersTable(), S".ID, ", MOG_ControllerSystem::GetDB()->GetUsersTable(), S".UserName, ", MOG_ControllerSystem::GetDB()->GetUsersTable(), S".EmailAddress, ",
					MOG_ControllerSystem::GetDB()->GetDepartmentsTable(), S".DepartmentName, Users3.UserName As BlessTarget, ",
					MOG_ControllerSystem::GetDB()->GetUsersTable(), S".CreatedDate, Users1.UserName As CreatedBy, ",
					MOG_ControllerSystem::GetDB()->GetUsersTable(), S".RemovedDate, Users2.UserName AS RemovedBy ",
		S"FROM ", MOG_ControllerSystem::GetDB()->GetUsersTable(), S" LEFT OUTER JOIN ",
				MOG_ControllerSystem::GetDB()->GetUsersTable(), S" Users1 ON ", MOG_ControllerSystem::GetDB()->GetUsersTable(), S".CreatedBy = Users1.ID LEFT OUTER JOIN ",
				MOG_ControllerSystem::GetDB()->GetUsersTable(), S" Users2 ON ", MOG_ControllerSystem::GetDB()->GetUsersTable(), S".RemovedBy = Users2.ID LEFT OUTER JOIN ",
				MOG_ControllerSystem::GetDB()->GetUsersTable(), S" Users3 ON ", MOG_ControllerSystem::GetDB()->GetUsersTable(), S".BlessTarget = Users3.ID LEFT OUTER JOIN ",
				MOG_ControllerSystem::GetDB()->GetDepartmentsTable(), S" ON ", MOG_ControllerSystem::GetDB()->GetUsersTable(), S".DepartmentID = ", MOG_ControllerSystem::GetDB()->GetDepartmentsTable(), S".ID ",
		S"WHERE (", MOG_ControllerSystem::GetDB()->GetUsersTable(), S".UserName = '", MOG_DBAPI::FixSQLParameterString(userName), S"')" );

	return QueryUser(selectString);
}

ArrayList *MOG_DBProjectAPI::GetUsers()
{
	String *selectString = String::Concat(
		S"SELECT ", MOG_ControllerSystem::GetDB()->GetUsersTable(), S".ID, ", MOG_ControllerSystem::GetDB()->GetUsersTable(), S".UserName, ", MOG_ControllerSystem::GetDB()->GetUsersTable(), S".EmailAddress, ",
					MOG_ControllerSystem::GetDB()->GetDepartmentsTable(), S".DepartmentName, Users3.UserName As BlessTarget, ",
					MOG_ControllerSystem::GetDB()->GetUsersTable(), S".CreatedDate, Users1.UserName As CreatedBy, ",
					MOG_ControllerSystem::GetDB()->GetUsersTable(), S".RemovedDate, Users2.UserName AS RemovedBy ",
		S"FROM ", MOG_ControllerSystem::GetDB()->GetUsersTable(), S" LEFT OUTER JOIN ",
				MOG_ControllerSystem::GetDB()->GetUsersTable(), S" Users1 ON ", MOG_ControllerSystem::GetDB()->GetUsersTable(), S".CreatedBy = Users1.ID LEFT OUTER JOIN ",
				MOG_ControllerSystem::GetDB()->GetUsersTable(), S" Users2 ON ", MOG_ControllerSystem::GetDB()->GetUsersTable(), S".RemovedBy = Users2.ID LEFT OUTER JOIN ",
				MOG_ControllerSystem::GetDB()->GetUsersTable(), S" Users3 ON ", MOG_ControllerSystem::GetDB()->GetUsersTable(), S".BlessTarget = Users3.ID LEFT OUTER JOIN ",
				MOG_ControllerSystem::GetDB()->GetDepartmentsTable(), S" ON ", MOG_ControllerSystem::GetDB()->GetUsersTable(), S".DepartmentID = ", MOG_ControllerSystem::GetDB()->GetDepartmentsTable(), S".ID ");

	return QueryUsers(selectString);
}

ArrayList *MOG_DBProjectAPI::GetActiveUsers()
{
	String *selectString = String::Concat(
		S"SELECT ", MOG_ControllerSystem::GetDB()->GetUsersTable(), S".ID, ", MOG_ControllerSystem::GetDB()->GetUsersTable(), S".UserName, ", MOG_ControllerSystem::GetDB()->GetUsersTable(), S".EmailAddress, ",
					MOG_ControllerSystem::GetDB()->GetDepartmentsTable(), S".DepartmentName, Users3.UserName As BlessTarget, ",
					MOG_ControllerSystem::GetDB()->GetUsersTable(), S".CreatedDate, Users1.UserName As CreatedBy, ",
					MOG_ControllerSystem::GetDB()->GetUsersTable(), S".RemovedDate, Users2.UserName AS RemovedBy ",
		S"FROM ", MOG_ControllerSystem::GetDB()->GetUsersTable(), S" LEFT OUTER JOIN ",
				MOG_ControllerSystem::GetDB()->GetUsersTable(), S" Users1 ON ", MOG_ControllerSystem::GetDB()->GetUsersTable(), S".CreatedBy = Users1.ID LEFT OUTER JOIN ",
				MOG_ControllerSystem::GetDB()->GetUsersTable(), S" Users2 ON ", MOG_ControllerSystem::GetDB()->GetUsersTable(), S".RemovedBy = Users2.ID LEFT OUTER JOIN ",
				MOG_ControllerSystem::GetDB()->GetUsersTable(), S" Users3 ON ", MOG_ControllerSystem::GetDB()->GetUsersTable(), S".BlessTarget = Users3.ID LEFT OUTER JOIN ",
				MOG_ControllerSystem::GetDB()->GetDepartmentsTable(), S" ON ", MOG_ControllerSystem::GetDB()->GetUsersTable(), S".DepartmentID = ", MOG_ControllerSystem::GetDB()->GetDepartmentsTable(), S".ID ",
		S"WHERE (", MOG_ControllerSystem::GetDB()->GetUsersTable(), S".RemovedDate = '')" );

	return QueryUsers(selectString);
}

bool MOG_DBProjectAPI::AddUser(String *userName, String *emailAddress, String *blessTarget, String *department, String *createdBy, String *createdDate)
{
	// Check if this data already exists
	int userID = GetUserID(userName);
	if( userID )
	{
		if(!IsActiveUser( userID ))
		{
			if(MOG_Prompt::PromptResponse("Reactivating User", "The user you are trying to create existed previously in MOG. \nDo you want to replace the previous user with this new user.", MOGPromptButtons::YesNo) == MOGPromptResult::Yes)
			{
				if(ReactivateUser(userID, GetUserID(createdBy), createdDate))
				{
					return UpdateUser(userName, emailAddress, blessTarget,department);
				}
			}
		}
		return false;
	}

	MOG_User *userInfo = new MOG_User();

	userInfo->SetUserName(userName);
	userInfo->SetUserEmailAddress(emailAddress);
	userInfo->SetBlessTarget(blessTarget);
	userInfo->SetUserDepartment(department);
	userInfo->SetCreatedDate(createdDate == NULL ? MOG_Time::GetVersionTimestamp() : createdDate);
	userInfo->SetCreatedBy(createdBy);
	userInfo->SetRemovedDate(S"");
	userInfo->SetRemovedBy(0);

	return (SQLCreateUser(userInfo) != NULL);
}

bool MOG_DBProjectAPI::UpdateUser(String *userName, String *emailAddress, String *blessTarget, String *department)
{
	// Check if this data already exists
	MOG_User *userInfo = GetUser(userName);
	if( userInfo )
	{
		userInfo->SetUserName(userName);
		userInfo->SetUserEmailAddress(emailAddress);
		userInfo->SetBlessTarget(blessTarget);
		userInfo->SetUserDepartment(department);

		return SQLUpdateUser(userName, userInfo);
	}

	return false;
}

bool MOG_DBProjectAPI::RemoveUser(String *userName, String *removedBy)
{
	// Check if this user already exists
	MOG_User *userInfo = GetUser(userName);
	if( userInfo != NULL )
	{
		userInfo->SetRemovedBy(removedBy);
		userInfo->SetRemovedDate(MOG_Time::GetVersionTimestamp());
		
		return SQLUpdateUser(userName, userInfo);
	}

	// Don't bother to remove one that is not already there!
	return false;
}

bool MOG_DBProjectAPI::RenameUser(String *oldUserName, String *newUserName)
{
	// JTM - TODO: check if newUserName already exists? to avoid dups
	MOG_User *userInfo = GetUser(oldUserName);
	if( userInfo != NULL )
	{
		userInfo->SetUserName(newUserName);

		return SQLUpdateUser(oldUserName, userInfo);		
	}

	return false;
}

MOG_DBPlatformInfo *MOG_DBProjectAPI::QueryPlatform(String *selectString)
{
	SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());
	MOG_DBPlatformInfo *platformInfo = NULL;

	try
	{
		SqlDataReader *myReader = MOG_DBAPI::GetNewDataReader(selectString, myConnection);
		while(myReader->Read())
		{
			platformInfo = SQLReadPlatform(myReader);
			break;
		}
		myReader->Close();	
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
		myConnection->Close();
	}

	return platformInfo;
}

ArrayList *MOG_DBProjectAPI::QueryPlatforms(String *selectString)
{
	SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());
	ArrayList *platforms = NULL;
	try
	{
		platforms = new ArrayList();

		SqlDataReader *myReader = MOG_DBAPI::GetNewDataReader(selectString, myConnection);
		while(myReader->Read())
		{
			platforms->Add(SQLReadPlatform(myReader));
		}
		myReader->Close();
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
		myConnection->Close();
	}

	return platforms;
}

bool MOG_DBProjectAPI::SQLCreatePlatformName(String *platformName)
{
	bool bAdded = false;

	// Make sure this is actually a new platform name
	int platformNameID = GetPlatformNameID(platformName);
	if (platformNameID == 0)
	{
		// Build an SQL Insert statement string for all the input-form
		String *insertCmd = String::Concat( S"INSERT INTO ", MOG_ControllerSystem::GetDB()->GetPlatformsTable(),
											S" ( PlatformName, PlatformKey) VALUES",
											S" ('",MOG_DBAPI::FixSQLParameterString(platformName), S"', '", MOG_DBAPI::FixSQLParameterString(platformName) , S"')" );
		bAdded = MOG_DBAPI::RunNonQuery(insertCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
	}

	return bAdded;
}

bool MOG_DBProjectAPI::SQLCreatePlatformInBranch(String *platformName, String* branchName)
{
	bool bAdded = false;

	// Get the branchID
	int branchID = MOG_DBProjectAPI::GetBranchIDByName(branchName);
	if (branchID)
	{
		// Always make sure this platformName exists
		AddPlatformName(platformName);
		int platformNameID = GetPlatformNameID(platformName);
		if (platformNameID)
		{
			// Insert a branch link for this platform
			String *insertCmd = String::Concat( S"INSERT INTO ", MOG_ControllerSystem::GetDB()->GetPlatformsBranchLinksTable(),
												S" ( PlatformNameID, BranchID) VALUES",
												S" (",platformNameID.ToString(), S", ", branchID.ToString() , S")" );
			bAdded = MOG_DBAPI::RunNonQuery(insertCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
		}
	}

	return bAdded;
}

MOG_DBPlatformInfo *MOG_DBProjectAPI::SQLReadPlatform(SqlDataReader *myReader)
{
	MOG_DBPlatformInfo *platformInfo = new MOG_DBPlatformInfo;

	platformInfo->mID = myReader->GetInt32(myReader->GetOrdinal(S"ID"));
	platformInfo->mPlatformName = myReader->GetString(myReader->GetOrdinal(S"PlatformName"))->Trim();
	platformInfo->mPlatformKey = myReader->GetString(myReader->GetOrdinal(S"PlatformKey"))->Trim();

	return platformInfo;
}

bool MOG_DBProjectAPI::SQLUpdatePlatform(String *platformName, MOG_DBPlatformInfo *newPlatform)
{
	if( newPlatform == NULL )
		return false;

	String *updateCmd = String::Concat( S"UPDATE ", MOG_ControllerSystem::GetDB()->GetPlatformsTable(), S" SET" );

	// Variables
	updateCmd = String::Concat(updateCmd, S" PlatformName='", MOG_DBAPI::FixSQLParameterString(newPlatform->mPlatformName), S"',");
	updateCmd = String::Concat(updateCmd, S" PlatformKey='", MOG_DBAPI::FixSQLParameterString(newPlatform->mPlatformKey), S"' ");

	// Where clause
	updateCmd = String::Concat(updateCmd, S" WHERE PlatformName='", MOG_DBAPI::FixSQLParameterString(platformName), S"'");

	MOG_DBAPI::RunNonQuery(updateCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());

//?	MOG_DBProjectAPI::SQLUpdatePlatform - This is just a bad idea! What about already existing assets in the system?  The AssetNames table, package assignments, relationships, all don't use the platformID...This should result in a much more in-depth and intrusive examination of all assets!

	return true;
}

bool MOG_DBProjectAPI::SQLRemovePlatformFromBranch(MOG_DBPlatformInfo *platformInfo, String* branchName)
{
	bool bRemoved = true;

	if (platformInfo != NULL)
	{
		// Get the branchID
		int branchID = MOG_DBProjectAPI::GetBranchIDByName(branchName);
		if (branchID)
		{
			// return early if invalid struct or already exists
			int platformNameID = GetPlatformNameID(platformInfo->mPlatformName);
			if (platformNameID != 0)
			{
				// Remove the platform from the PlatformsTable
				String *deleteCmd = String::Concat(S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetPlatformsBranchLinksTable(), 
					S" WHERE (PlatformNameID=", platformNameID.ToString(), S") AND (BranchID=", branchID.ToString(), S") ");
				bRemoved = MOG_DBAPI::RunNonQuery(deleteCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
			}
			else
			{
				// Looks like the platform is already gone
				bRemoved = true;
			}
		}
	}

	return bRemoved;
}

String *MOG_DBProjectAPI::GetPlatformByID(int id)
{
	String *selectString = String::Concat(S"SELECT * FROM ", MOG_ControllerSystem::GetDB()->GetPlatformsTable(), S" WHERE ID='", Convert::ToString(id), S"'");
	String *ordinal = "PlatformName";

	return MOG_DBAPI::QueryString(selectString, ordinal);
}

int MOG_DBProjectAPI::GetPlatformNameID(String *name)
{
	String *selectString = String::Concat(S"SELECT * FROM ", MOG_ControllerSystem::GetDB()->GetPlatformsTable(), S" WHERE PlatformName='", MOG_DBAPI::FixSQLParameterString(name), S"'");
	String *ordinal = "ID";

	return MOG_DBAPI::QueryInt(selectString, ordinal);
}

MOG_DBDepartmentInfo *MOG_DBProjectAPI::QueryDepartment(String *selectString)
{
	SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());
	MOG_DBDepartmentInfo *departmentInfo = NULL;

	try
	{
		SqlDataReader *myReader = MOG_DBAPI::GetNewDataReader(selectString, myConnection);
	
		while(myReader->Read())
		{
			departmentInfo = SQLReadDepartment(myReader);
			break;
		}
		myReader->Close();
	
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
		myConnection->Close();
	}

	return departmentInfo;
}

ArrayList *MOG_DBProjectAPI::QueryDepartments(String *selectString)
{
	SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());
	ArrayList *departments = NULL;

	try
	{
		departments = new ArrayList();

		SqlDataReader *myReader = MOG_DBAPI::GetNewDataReader(selectString, myConnection);
		while(myReader->Read())
		{
			departments->Add(SQLReadDepartment(myReader));
		}
		myReader->Close();
		
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
		myConnection->Close();
	}

	return departments;
}

MOG_DBDepartmentInfo *MOG_DBProjectAPI::SQLCreateDepartment(MOG_DBDepartmentInfo *departmentInfo)
{
	// return early if invalid struct or already exists
	if( departmentInfo == NULL || GetDepartmentID(departmentInfo->mDepartmentName) != 0 )
	{
		return NULL;
	}

	// Build an SQL Insert statement string for all the input-form
	String *insertCmd = String::Concat( S"INSERT INTO ", MOG_ControllerSystem::GetDB()->GetDepartmentsTable(),
										S" ( DepartmentName) VALUES",
										S" ('", MOG_DBAPI::FixSQLParameterString(departmentInfo->mDepartmentName), S"')" );

	MOG_DBAPI::RunNonQuery(insertCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());

	return departmentInfo;
}

MOG_DBDepartmentInfo *MOG_DBProjectAPI::SQLReadDepartment(SqlDataReader *myReader)
{
	MOG_DBDepartmentInfo *departmentInfo = new MOG_DBDepartmentInfo;

	departmentInfo->mID = myReader->GetInt32(myReader->GetOrdinal(S"ID"));
	departmentInfo->mDepartmentName = myReader->GetString(myReader->GetOrdinal(S"DepartmentName"))->Trim();

	return departmentInfo;
}

bool MOG_DBProjectAPI::SQLUpdateDepartment(String *departmentName, MOG_DBDepartmentInfo *newDepartment)
{
	if( newDepartment == NULL )
		return false;

	String *updateCmd = String::Concat( S"UPDATE ", MOG_ControllerSystem::GetDB()->GetDepartmentsTable(), S" SET" );

	// Variables
	updateCmd = String::Concat(updateCmd, S" DepartmentName='", MOG_DBAPI::FixSQLParameterString(newDepartment->mDepartmentName), S"' ");

	// Where clause
	updateCmd = String::Concat(updateCmd, S" WHERE DepartmentName='", MOG_DBAPI::FixSQLParameterString(departmentName), S"'");

	MOG_DBAPI::RunNonQuery(updateCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());

	return true;
}

bool MOG_DBProjectAPI::SQLRemoveDepartment(MOG_DBDepartmentInfo *departmentInfo)
{
	if (departmentInfo)
	{
		String* deleteCmd = String::Concat(S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetDepartmentsTable(), S" WHERE DepartmentName='", MOG_DBAPI::FixSQLParameterString(departmentInfo->mDepartmentName), S"'");
	
		return MOG_DBAPI::RunNonQuery(deleteCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
	}

	return false;
}

MOG_User *MOG_DBProjectAPI::QueryUser(String *selectString)
{
	SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());
	MOG_User *userInfo = NULL;

	try
	{
		SqlDataReader *myReader = MOG_DBAPI::GetNewDataReader(selectString, myConnection);
		while(myReader->Read())
		{
			userInfo = SQLReadUser(myReader);
			break;
		}
		myReader->Close();
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
		myConnection->Close();
	}

	return userInfo;
}

ArrayList *MOG_DBProjectAPI::QueryUsers(String *selectString)
{
	SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());
	ArrayList *users = new ArrayList();

	try
	{
		SqlDataReader *myReader = MOG_DBAPI::GetNewDataReader(selectString, myConnection);

		while(myReader->Read())
		{
			users->Add(SQLReadUser(myReader));
		}
		myReader->Close();
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
		myConnection->Close();
	}

	return users;
}

MOG_User *MOG_DBProjectAPI::SQLCreateUser(MOG_User *userInfo)
{
	// return early if invalid struct or already exists
	if( userInfo == NULL || GetUserID(userInfo->GetUserName()) != 0 )
	{
		return NULL;
	}

	// Build an SQL Insert statement string for all the input-form
	String *insertCmd = String::Concat( S"INSERT INTO ", MOG_ControllerSystem::GetDB()->GetUsersTable(),
										S" ( UserName, EmailAddress, BlessTarget, DepartmentID, CreatedDate, CreatedBy, RemovedDate, RemovedBy) VALUES",
										S" ('", MOG_DBAPI::FixSQLParameterString(userInfo->GetUserName()), S"', '", MOG_DBAPI::FixSQLParameterString(userInfo->GetUserEmailAddress()) , S"', ", __box(GetUserID(userInfo->GetBlessTarget())), S",",__box(GetDepartmentID(userInfo->GetUserDepartment())), S", '", userInfo->GetCreatedDate(), S"' ,", __box(GetUserID(userInfo->GetCreatedBy())), S",",S"''", S",",__box(0), S")" );

	MOG_DBAPI::RunNonQuery(insertCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
	return userInfo;
}

MOG_User *MOG_DBProjectAPI::SQLReadUser(SqlDataReader *myReader)
{
	MOG_User *userInfo = new MOG_User;

	userInfo->SetUserID(myReader->GetInt32(myReader->GetOrdinal(S"ID")));
	userInfo->SetUserName(MOG_DBAPI::SafeStringRead(myReader, S"UserName"));
	userInfo->SetUserEmailAddress(MOG_DBAPI::SafeStringRead(myReader, S"EmailAddress"));
	userInfo->SetBlessTarget(MOG_DBAPI::SafeStringRead(myReader, S"BlessTarget"));
	userInfo->SetUserDepartment(MOG_DBAPI::SafeStringRead(myReader, S"DepartmentName"));
	userInfo->SetCreatedDate(MOG_DBAPI::SafeStringRead(myReader, S"CreatedDate"));
	userInfo->SetCreatedBy(MOG_DBAPI::SafeStringRead(myReader, S"CreatedBy"));
	userInfo->SetRemovedDate(MOG_DBAPI::SafeStringRead(myReader, S"RemovedDate"));
	userInfo->SetRemovedBy(MOG_DBAPI::SafeStringRead(myReader, S"RemovedBy"));

	// Default BlessTarget to MasterData
	if (userInfo->GetBlessTarget()->Length == 0)
	{
		userInfo->SetBlessTarget(S"MasterData");
	}

	return userInfo;
}

bool MOG_DBProjectAPI::SQLUpdateUser(String *userName, MOG_User *newUser)
{
	if( newUser == NULL )
		return false;

	String *updateCmd = String::Concat( S"UPDATE ", MOG_ControllerSystem::GetDB()->GetUsersTable(), S" SET" );

	// Variables
	updateCmd = String::Concat(updateCmd, S" UserName='", MOG_DBAPI::FixSQLParameterString(newUser->GetUserName()), S"',");
	updateCmd = String::Concat(updateCmd, S" EmailAddress='", MOG_DBAPI::FixSQLParameterString(newUser->GetUserEmailAddress()), S"',");
	updateCmd = String::Concat(updateCmd, S" BlessTarget='", Convert::ToString(GetUserID(newUser->GetBlessTarget())), S"',");
	updateCmd = String::Concat(updateCmd, S" DepartmentID='", Convert::ToString(GetDepartmentID(newUser->GetUserDepartment())), S"',");
	updateCmd = String::Concat(updateCmd, S" CreatedDate='", newUser->GetCreatedDate(), S"',");
	updateCmd = String::Concat(updateCmd, S" CreatedBy='", Convert::ToString(GetUserID(newUser->GetCreatedBy())), S"',");
	updateCmd = String::Concat(updateCmd, S" RemovedDate='", newUser->GetRemovedDate(), S"',");
	updateCmd = String::Concat(updateCmd, S" RemovedBy='", Convert::ToString(GetUserID(newUser->GetRemovedBy())), S"' ");

	// Where clause
	updateCmd = String::Concat(updateCmd, S" WHERE UserName='", MOG_DBAPI::FixSQLParameterString(userName), S"'");

	return MOG_DBAPI::RunNonQuery(updateCmd, MOG_ControllerSystem::GetDB()->GetConnectionString()); 

}

bool MOG_DBProjectAPI::SQLRemoveUser(MOG_User *userInfo, String *removedBy)
{
	return false;
}

