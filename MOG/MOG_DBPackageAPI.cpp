//--------------------------------------------------------------------------------
//	MOG_DBPackage.cpp
//	
//	
//--------------------------------------------------------------------------------
#include "stdafx.h"


#include "MOG_Time.h"
#include "MOG_ControllerSystem.h"
#include "MOG_ControllerPackage.h"
#include "MOG_ControllerProject.h"
#include "MOG_DBAPI.h"
#include "MOG_DBPackageCommandAPI.h"

#include "MOG_DBPackageAPI.h"




int MOG_DBPackageAPI::GetPackageGroupID(int packageVersionID, String *packageGroupName)
{
	int retPackageGroupID = MOG_ControllerSystem::GetDB()->GetDBCache()->GetPackageGroupCache()->GetIDFromName(Convert::ToString(packageVersionID), packageGroupName);
	if(retPackageGroupID == 0)
	{
	// root case
	if( packageGroupName == NULL )
		packageGroupName = S"";

	String *selectCmd = String::Concat( S"SELECT ID ",
										S"FROM ", MOG_ControllerSystem::GetDB()->GetPackageGroupNamesTable(), S" PackageGroupNames ",
										S"WHERE (PackageVersionID = ", Convert::ToString(packageVersionID), S") AND ",
											S"(PackageGroupName = '", MOG_DBAPI::FixSQLParameterString(packageGroupName), S"')" );
		
	retPackageGroupID = MOG_DBAPI::QueryInt( selectCmd, "ID");
	if(retPackageGroupID > 0)
	{
		MOG_ControllerSystem::GetDB()->GetDBCache()->GetPackageGroupCache()->AddSetToCache(retPackageGroupID,Convert::ToString(packageVersionID), packageGroupName);
	}

	}
	return retPackageGroupID;
}


MOG_DBPackageInfo *MOG_DBPackageAPI::SQLReadPackageInfo(SqlDataReader *myReader)
{
	// expects the following columns: PackageGroupName FullTreeName AssetPlatformKey Version AssetLabel
	MOG_DBPackageInfo *packageInfo = new MOG_DBPackageInfo();
	
	String *packageFullname = String::Concat( MOG_DBAPI::SafeStringRead(myReader,S"FullTreeName")->Trim(),
											  S"{", MOG_DBAPI::SafeStringRead(myReader,S"AssetPlatformKey")->Trim(), S"}",
											  MOG_DBAPI::SafeStringRead(myReader,S"AssetLabel")->Trim(),
											  S"\\R.", MOG_DBAPI::SafeStringRead(myReader,S"Version")->Trim());

	packageInfo->mPackageFilename = new MOG_Filename(packageFullname);
	packageInfo->mPackageGroup = MOG_DBAPI::SafeStringRead(myReader,S"PackageGroupName")->Trim();
	
	return packageInfo;
}

ArrayList *MOG_DBPackageAPI::QueryPackageGroups(String *selectString)
{
	SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());
	ArrayList *groups = NULL;

	SqlDataReader *myReader = NULL;
	try
	{
		groups = new ArrayList();

		myReader = MOG_DBAPI::GetNewDataReader(selectString, myConnection);
		while(myReader->Read())
		{
			groups->Add(SQLReadPackageInfo(myReader));
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
	
	
	return groups;
}

bool MOG_DBPackageAPI::AddPackageLink(MOG_Filename *newBlessedPackageFilename, MOG_Filename *pBlessedAssetFilename, String *packageGroupName)
{
	int packageVersionID = MOG_DBAssetAPI::GetAssetVersionID(newBlessedPackageFilename, newBlessedPackageFilename->GetVersionTimeStamp());
	int AssetNameID = MOG_DBAssetAPI::GetAssetNameID(pBlessedAssetFilename);
	int AssetVersionID = MOG_DBAssetAPI::GetAssetVersionID(pBlessedAssetFilename, NULL);
	int PackageGroupNameID = GetPackageGroupID(packageVersionID, packageGroupName);

	if( packageVersionID == 0 )
	{
		// error invalid package version
		return false;
	}

	if( AssetVersionID == 0 )
	{
		// error invalid asset version
		return false;
	}

	if( PackageGroupNameID == 0 )
	{
		// Auto generate the specified packageGroupName
		AddPackageGroupName(newBlessedPackageFilename, newBlessedPackageFilename->GetVersionTimeStamp(), packageGroupName, S"Admin");
		PackageGroupNameID = GetPackageGroupID(packageVersionID, packageGroupName);

		// Confirm that it was added successfully?
		if( PackageGroupNameID == 0 )
		{
			// error invalid package group name
			return false;
		}
	}
	// see if there is a record for any version of this asset
	String *selectCmd = String::Concat(
		S"SELECT PackageLinks.AssetVersionID ",
		S"FROM ", MOG_ControllerSystem::GetDB()->GetPackageLinksTable(), S" PackageLinks INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions ON PackageLinks.AssetVersionID = AssetVersions.ID ",
		S"WHERE (PackageLinks.PackageGroupNameID = ", Convert::ToString(PackageGroupNameID), S") AND ",
			S"(AssetVersions.AssetNameID = ", Convert::ToString(AssetNameID), S")" );

	int existingAssetVersionID = MOG_DBAPI::QueryInt(selectCmd, S"AssetVersionID");

	String *insUpdtCmd;

	// if it doesnt exist add it
	if( existingAssetVersionID == 0 )
	{
		
		insUpdtCmd = String::Concat(S"INSERT INTO ", MOG_ControllerSystem::GetDB()->GetPackageLinksTable(),
								S" ( AssetVersionID, PackageGroupNameID ) VALUES ",
								S" (",__box(AssetVersionID), S",",__box(PackageGroupNameID) ,S" )" );
	}
	else
	{
		// it does exist...update to new version
		//String *updateCmd;
		insUpdtCmd = String::Concat(
			S"UPDATE ", MOG_ControllerSystem::GetDB()->GetPackageLinksTable(),
			S" SET AssetVersionID = ", Convert::ToString(AssetVersionID),
			S" WHERE (AssetVersionID = ", Convert::ToString(existingAssetVersionID), S") AND ",
				S"(PackageGroupNameID = ", Convert::ToString(PackageGroupNameID), S")" );
	}
	return MOG_DBAPI::RunNonQuery(insUpdtCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
}

bool MOG_DBPackageAPI::RemovePackageLink(MOG_Filename *newBlessedPackageFilename, MOG_Filename *pBlessedAssetFilename, String *packageGroupName)
{
	int packageVersionID = MOG_DBAssetAPI::GetAssetVersionID(newBlessedPackageFilename, newBlessedPackageFilename->GetVersionTimeStamp());
	int AssetVersionID = MOG_DBAssetAPI::GetAssetVersionID(pBlessedAssetFilename, NULL);
	int PackageGroupNameID = GetPackageGroupID(packageVersionID, packageGroupName);

	if( packageVersionID == 0 )
	{
		// error invalid package version
		return false;
	}

	if( AssetVersionID == 0 )
	{
		// error invalid asset version
		return false;
	}

	if( PackageGroupNameID == 0 )
	{
		// error invalid package group name
		return false;
	}

	String *deleteCmd = String::Concat(
		S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetPackageLinksTable(), S" ",
		S"WHERE (PackageGroupNameID = ", Convert::ToString(PackageGroupNameID), S") AND ",
			S"(AssetVersionID = ", Convert::ToString(AssetVersionID), S")" );

		return MOG_DBAPI::RunNonQuery(deleteCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
}

ArrayList *MOG_DBPackageAPI::GetAllAssetsInPackage(MOG_Filename *packageName, String *packageVersion)
{
	int packageVersionID = MOG_DBAssetAPI::GetAssetVersionID(packageName, packageVersion);
	String *selectCmd = String::Concat( S"SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ",
										S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetPackageLinksTable(), S" PackageLinks INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetPackageGroupNamesTable(), S" PackageGroupNames ON PackageLinks.PackageGroupNameID = PackageGroupNames.ID ON ",
											S"AssetVersions.ID = PackageLinks.AssetVersionID INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames ON AssetVersions.AssetNameID = AssetNames.ID INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID ",
										S"WHERE (PackageGroupNames.PackageVersionID = ", Convert::ToString(packageVersionID), S") ",
										S"ORDER BY AssetNames.AssetLabel" );

	return MOG_DBAssetAPI::QueryAssets(selectCmd);
}

ArrayList *MOG_DBPackageAPI::GetPackageGroupsForAsset(MOG_Filename *assetName)
{
	int branchID = MOG_DBProjectAPI::GetBranchIDByName(MOG_ControllerProject::GetBranchName());
	int assetVersionID = MOG_DBAssetAPI::GetAssetVersionID(assetName, assetName->GetVersionTimeStamp());

	String *selectCmd = String::Concat( S"SELECT AssetClassifications.FullTreeName, PackageNames.AssetPlatformKey, PackageNames.AssetLabel, PackageVersions.Version, PackageGroupNames.PackageGroupName ",
										S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetPackageLinksTable(), S" PackageLinks INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetPackageGroupNamesTable(), S" PackageGroupNames ON PackageLinks.PackageGroupNameID = PackageGroupNames.ID ON ",
											S"AssetVersions.ID = PackageLinks.AssetVersionID INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" PackageVersions ON PackageGroupNames.PackageVersionID = PackageVersions.ID INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" PackageNames ON PackageVersions.AssetNameID = PackageNames.ID INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON PackageNames.AssetClassificationID = AssetClassifications.ID INNER JOIN (SELECT DISTINCT * FROM ",
											MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), S") AS PackageBranchLinks ON PackageVersions.ID = PackageBranchLinks.AssetVersionID ",
										S"WHERE (PackageBranchLinks.BranchID = ", Convert::ToString(branchID), S") AND ",
											S"(PackageLinks.AssetVersionID = ", Convert::ToString(assetVersionID), S") ",
										S"ORDER BY PackageGroupNames.PackageGroupName" );
		
	return QueryPackageGroups(selectCmd);
}

ArrayList *MOG_DBPackageAPI::GetAllAssetsInPackageGroup(MOG_Filename *packageName, String *packageVersion, String *packageGroupName)
{
	int packageVersionID = MOG_DBAssetAPI::GetAssetVersionID(packageName, packageVersion);
	String *selectCmd = String::Concat( S"SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetVersions.Version ",
										S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetVersionsTable(), S" AssetVersions INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetPackageLinksTable(), S" PackageLinks INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetPackageGroupNamesTable(), S" PackageGroupNames ON PackageLinks.PackageGroupNameID = PackageGroupNames.ID ON ",
											S"AssetVersions.ID = PackageLinks.AssetVersionID INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames ON AssetVersions.AssetNameID = AssetNames.ID INNER JOIN ",
											MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ON AssetNames.AssetClassificationID = AssetClassifications.ID ",
										S"WHERE (PackageGroupNames.PackageGroupName = '", MOG_DBAPI::FixSQLParameterString(packageGroupName), S"') AND ",
											S"(PackageGroupNames.PackageVersionID = ", Convert::ToString(packageVersionID), S") ",
										S"ORDER BY AssetNames.AssetLabel" );
					  
	return MOG_DBAssetAPI::QueryAssets(selectCmd);
}

bool MOG_DBPackageAPI::AddPackage(MOG_Filename *packageName, String *createdBy)
{
	String *insertCmd;
	String *createdDate = MOG_Time::GetVersionTimestamp();

	int createdByID = MOG_DBProjectAPI::GetUserID(createdBy);

	// make sure we have an AssetNames record
	int assetNameID = MOG_DBAssetAPI::GetAssetNameID(packageName);
	if( assetNameID == 0 )
	{
		// add AssetNames record
		int classificationID = MOG_DBAssetAPI::GetClassificationID(packageName->GetAssetClassification());

		if( classificationID == 0 )
		{
			// ensure the classification exists
			MOG_DBAssetAPI::CreateClassification(packageName->GetAssetClassification(), createdBy, createdDate);

			// try to get ID again
			classificationID = MOG_DBAssetAPI::GetClassificationID(packageName->GetAssetClassification());

			// if still 0 we have a serious problem...database down?
			if( classificationID == 0 )
			{
				// JTM-TODO error...throw something here
			}
		}

		insertCmd = String::Concat(S"INSERT INTO ", MOG_ControllerSystem::GetDB()->GetAssetNamesTable(),
								   S" ( AssetClassificationID, AssetPlatformKey, AssetLabel, CreatedBy, CreatedDate, RemovedDate, RemovedBy) VALUES ",
								   S" (",__box(classificationID) , S",'", MOG_DBAPI::FixSQLParameterString(packageName->GetAssetPlatform()), S"','", MOG_DBAPI::FixSQLParameterString(packageName->GetAssetLabel()), S"',", __box(createdByID), S",'", createdDate, S"', '', 0)" );

		return MOG_DBAPI::RunNonQuery(insertCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
	}

	return true;
}

bool MOG_DBPackageAPI::RemovePackage(MOG_Filename *packageName, String *removedBy)
{
	int removedByID = MOG_DBProjectAPI::GetUserID(removedBy);

	int assetNameID = MOG_DBAssetAPI::GetAssetNameID(packageName);

	String *updateCmd = String::Concat(S"UPDATE ", MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" SET");

	// Variables
	updateCmd = String::Concat(updateCmd, S" RemovedDate='", MOG_Time::GetVersionTimestamp(), S"',");
	updateCmd = String::Concat(updateCmd, S" RemovedBy='", Convert::ToString(removedByID), S"',");

	// Where clause
	updateCmd = String::Concat(updateCmd, S" WHERE ID=", Convert::ToString(assetNameID));
	
	return MOG_DBAPI::RunNonQuery(updateCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
}

bool MOG_DBPackageAPI::AddPackageGroupName(MOG_Filename *packageName, String *packageVersion, String *packageGroupName, String *createdBy)
{
	bool bAdded = false;
	String *selectCmd;
	String *insertCmd;

	// Get the invalid MOG characters (except '/' which is a valid package object delimiter)
	String *invalidChars = MOG_ControllerSystem::GetInvalidMOGCharacters()->Replace(S"/", S"");
	// Validate the specified name 
	if (MOG_ControllerSystem::InvalidCharactersCheck(packageGroupName, invalidChars, true))
	{
		return false;
	}

	int packageVersionID = MOG_DBAssetAPI::GetAssetVersionID(packageName, packageVersion);
	if( packageVersionID == 0 )
	{
		// invalid package version
		return bAdded;
	}

	selectCmd = String::Concat( S"SELECT ID FROM ", MOG_ControllerSystem::GetDB()->GetPackageGroupNamesTable(),
								S"WHERE (PackageGroupName='", MOG_DBAPI::FixSQLParameterString(packageGroupName), S"') AND ",
								S"(PackageVersionID = ", Convert::ToString(packageVersionID) ,S")" );
	int id = MOG_DBAPI::QueryInt(selectCmd, S"ID");
	if( id == 0 )
	{
		// Make sure our parent group exists?
		String *parent = MOG_ControllerPackage::GetPackageGroupParent(packageGroupName);
		if (parent->Length)
		{
			// Recursively call ourself to make sure this exists?
			AddPackageGroupName(packageName, packageVersion, parent, createdBy);
		}

		// record doesnt exist...create it
		insertCmd = String::Concat(S"INSERT INTO ", MOG_ControllerSystem::GetDB()->GetPackageGroupNamesTable(),
								   S" ( PackageVersionID, PackageGroupName, CreatedDate, CreatedBy, RemovedDate, RemovedBy ) VALUES ",
								   S" (", __box(packageVersionID), S", '", MOG_DBAPI::FixSQLParameterString(packageGroupName), S"', '", MOG_Time::GetVersionTimestamp(), S"', ", __box(MOG_DBProjectAPI::GetUserID(createdBy)), S", ''		 , 0)" );

			return MOG_DBAPI::RunNonQuery(insertCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
	}
	else
	{
		// update record and make sure removed info is cleared
		String *updateCmd = String::Concat(S"UPDATE ", MOG_ControllerSystem::GetDB()->GetPackageGroupNamesTable(), S" SET");

		// Variables
		updateCmd = String::Concat(updateCmd, S" RemovedDate='',");
		updateCmd = String::Concat(updateCmd, S" RemovedBy=0");

		// Where clause
		updateCmd = String::Concat(updateCmd, S" WHERE ID=", Convert::ToString(id) );

		return MOG_DBAPI::RunNonQuery(updateCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
	}

	return bAdded;
}

bool MOG_DBPackageAPI::RemovePackageGroupName(MOG_Filename *packageName, String *packageVersion, String *packageGroupName, String *removedBy)
{
	bool bRemoved = false;
	String *selectCmd;

	int packageVersionID = MOG_DBAssetAPI::GetAssetVersionID(packageName, packageVersion);

	if( packageVersionID == 0 )
	{
		// invalid package version
		return bRemoved;
	}

	selectCmd = String::Concat( S"SELECT ID FROM ", MOG_ControllerSystem::GetDB()->GetPackageGroupNamesTable(),
								S"WHERE (PackageGroupName='", MOG_DBAPI::FixSQLParameterString(packageGroupName), S"') AND ",
								S"(PackageVersionID = ", Convert::ToString(packageVersionID) ,S")" );

	int groupID = MOG_DBAPI::QueryInt( selectCmd, "ID" );

	if( groupID != 0 )
	{
		String *updateCmd = String::Concat(S"UPDATE ", MOG_ControllerSystem::GetDB()->GetPackageGroupNamesTable(), S" SET");

		// Variables
		updateCmd = String::Concat(updateCmd, S" RemovedDate='", MOG_Time::GetVersionTimestamp(), S"',");
		updateCmd = String::Concat(updateCmd, S" RemovedBy=", Convert::ToString(MOG_DBProjectAPI::GetUserID(removedBy)));

		// Where clause
		updateCmd = String::Concat(updateCmd, S" WHERE ID=", Convert::ToString(groupID) );;

		return MOG_DBAPI::RunNonQuery(updateCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
	}
	return bRemoved;
}

ArrayList *MOG_DBPackageAPI::GetAllPackageGroups(MOG_Filename *packageName, String *packageVersion)
{
	int packageVersionID = MOG_DBAssetAPI::GetAssetVersionID(packageName, packageVersion);
	String *selectCmd = String::Concat( S"SELECT PackageGroupName ",
										S"FROM ", MOG_ControllerSystem::GetDB()->GetPackageGroupNamesTable(), S" PackageGroupNames ",
										S"WHERE (PackageVersionID = ", Convert::ToString(packageVersionID), S") ",
										S"ORDER BY PackageGroupName" );

	return MOG_DBAPI::QueryStrings(selectCmd, S"PackageGroupName");
}


ArrayList *MOG_DBPackageAPI::GetAllActivePackageGroups(MOG_Filename *packageName, String *packageVersion)
{
	int packageVersionID = MOG_DBAssetAPI::GetAssetVersionID(packageName, packageVersion);
	String *selectCmd = String::Concat( S"SELECT PackageGroupName ",
										S"FROM ", MOG_ControllerSystem::GetDB()->GetPackageGroupNamesTable(), S" PackageGroupNames ",
										S"WHERE (PackageVersionID = ", Convert::ToString(packageVersionID), S") AND (RemovedDate = '') ",
										S"ORDER BY PackageGroupName" );

	return MOG_DBAPI::QueryStrings(selectCmd, "PackageGroupName");
}

ArrayList *MOG_DBPackageAPI::GetAllActivePackagesByClassification(String *classification)
{
	String *selectCmd = String::Concat( S"SELECT AssetClassifications.FullTreeName, AssetNames.AssetPlatformKey, AssetNames.AssetLabel, AssetNames.CreatedDate AS Version ",
										S"FROM ", MOG_ControllerSystem::GetDB()->GetAssetNamesTable(), S" AssetNames INNER JOIN ", MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), S" AssetClassifications ",
										S" ON AssetNames.AssetClassificationID = AssetClassifications.ID "
										S"WHERE (AssetClassifications.FullTreeName = '", MOG_DBAPI::FixSQLParameterString(classification), S"') AND (AssetNames.RemovedDate = '')" );

	return MOG_DBAssetAPI::QueryAssets(selectCmd);
}


bool MOG_DBPackageAPI::PopulateNewPackageVersion(MOG_Filename *currentPackageFilename, MOG_Filename *newBlessedPackageFilename, ArrayList *packageCommands)
{
	bool bFailed = false;

	int createdByID = MOG_DBProjectAPI::GetUserID(S"Admin");
	String *createdDate = MOG_Time::GetVersionTimestamp();

	int newPackageNameID = MOG_DBAssetAPI::GetAssetNameID(newBlessedPackageFilename);
	int newPackageVersionID = MOG_DBAssetAPI::CreateAssetRevision(newPackageNameID, newBlessedPackageFilename->GetVersionTimeStamp(), createdByID, createdDate);

	// Check if there was a previous package revision specified?
	if (currentPackageFilename &&
		currentPackageFilename->GetAssetFullName()->Length)
	{
		int currentPackageVersionID = MOG_DBAssetAPI::GetAssetVersionID(currentPackageFilename, currentPackageFilename->GetVersionTimeStamp());

		// Make sure we valid IDs?
		if (currentPackageVersionID && newPackageVersionID)
		{
			// Check if existing package links need to be transferred to the new package?
			if (currentPackageVersionID != newPackageVersionID)
			{
				ArrayList *transactionList = new ArrayList();

				// Transfer the package groups up to the new package
				transactionList->Add(String::Concat(S"INSERT INTO ", MOG_ControllerSystem::GetDB()->GetPackageGroupNamesTable(),
															S" (PackageVersionID, PackageGroupName, CreatedDate, CreatedBy, RemovedDate, RemovedBy) ",
														S"SELECT ",Convert::ToString(newPackageVersionID), S" AS PackageVersionID, PackageGroupName, CreatedDate, CreatedBy, '' AS RemovedDate, 0 AS RemovedBy ",
														S"FROM ", MOG_ControllerSystem::GetDB()->GetPackageGroupNamesTable(), S" ",
														S"WHERE (RemovedDate = '') AND (PackageVersionID = ",Convert::ToString(currentPackageVersionID), S")" ));
				// Transfer the package links up to the new package
				transactionList->Add(String::Concat(S"INSERT INTO ", MOG_ControllerSystem::GetDB()->GetPackageLinksTable(),
															S" (PackageGroupNameID, AssetVersionID) ",
														S"SELECT NewPGN.ID, PackageLinks.AssetVersionID ",
														S"FROM ", MOG_ControllerSystem::GetDB()->GetPackageGroupNamesTable(), S" PGN INNER JOIN ",
															MOG_ControllerSystem::GetDB()->GetPackageLinksTable(), S" PackageLinks ON PGN.ID = PackageLinks.PackageGroupNameID INNER JOIN ",
															MOG_ControllerSystem::GetDB()->GetPackageGroupNamesTable(), S" NewPGN ON PGN.PackageGroupName = NewPGN.PackageGroupName ",
														S"WHERE (PGN.PackageVersionID = ",Convert::ToString(currentPackageVersionID), S") AND ",
															S"(NewPGN.PackageVersionID = ",Convert::ToString(newPackageVersionID), S")" ));
				// Execute the entire transaction
				if (!MOG_DBAPI::ExecuteTransaction(transactionList))
				{
					bFailed = true;
				}
			}
		}
	}

	if( !bFailed )
	{
		// Stamp new package links based on the package commands we just processed
		// Make sure we have a valid array?
		if (packageCommands)
		{
			// fix up assets based on package commands
			for( int i = 0; i < packageCommands->Count; i++ )
			{
				MOG_DBPackageCommandInfo *packageCommandInfo = __try_cast<MOG_DBPackageCommandInfo*>(packageCommands->Item[i]);

				// Make sure this command is related to the specified package?
				if (String::Compare(newBlessedPackageFilename->GetAssetFullName(), packageCommandInfo->mPackageName, true) == 0)
				{
					String *fullPackageName = MOG_ControllerPackage::CombinePackageAssignment(packageCommandInfo->mPackageName, packageCommandInfo->mPackageGroups, packageCommandInfo->mPackageObjects);
					String *packageGroupName = MOG_ControllerPackage::GetPackageGroupObjectPath(fullPackageName);
					MOG_Filename *assetFilename = new MOG_Filename(packageCommandInfo->mAssetName, packageCommandInfo->mAssetVersion);

					switch( packageCommandInfo->mPackageCommandType )
					{
						case MOG_PACKAGECOMMAND_TYPE::MOG_PackageCommand_AddAsset:
							AddPackageLink(newBlessedPackageFilename, assetFilename, packageGroupName);
							break;
						case MOG_PACKAGECOMMAND_TYPE::MOG_PackageCommand_RemoveAsset:
							RemovePackageLink(newBlessedPackageFilename, assetFilename, packageGroupName);
							break;
					}
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


