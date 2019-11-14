//--------------------------------------------------------------------------------
//	MOG_DBPackage.cpp
//	
//	
//--------------------------------------------------------------------------------
#include "stdafx.h"


#include "MOG_ControllerSystem.h"
#include "MOG_ControllerProject.h"
#include "MOG_ControllerPackage.h"
#include "MOG_DBPackageCommandAPI.h"

MOG_DBPackageCommandInfo *MOG_DBPackageCommandAPI::SchedulePackageCommand(String *assetName, String *assetVersion, String *jobLabel, String *branchName, String *platform, String *createdBy, String *blessedBy, String *packageName, String *packageGroups, String *packageObjects, MOG_PACKAGECOMMAND_TYPE packageCommandType)
{
	return SQLCreatePackageCommand(assetName, assetVersion, jobLabel, branchName, platform, createdBy, blessedBy, packageName, packageGroups, packageObjects, packageCommandType);
}

MOG_DBPackageCommandInfo *MOG_DBPackageCommandAPI::SchedulePackageLateResolver( String *packageName, String *linkedPackageName)
{
	return SchedulePackageCommand(packageName, S"", S"LateResolver", MOG_ControllerProject::GetBranchName(), MOG_ControllerPackage::GetPackagePlatform(packageName), S"", S"", linkedPackageName, S"", S"", MOG_PACKAGECOMMAND_TYPE::MOG_PackageCommand_LateResolver);
}

ArrayList *MOG_DBPackageCommandAPI::GetScheduledPackageCommands(String *branchName)
{
	String *selectString;
	
	// Get related package commands for this jobLabel, BranchName and platform
	selectString = String::Concat(
		S"SELECT * FROM ", MOG_ControllerSystem::GetDB()->GetPackageCommandsTable(),
		S" WHERE BranchName='", MOG_DBAPI::FixSQLParameterString(branchName),
			S"' ORDER BY ID" );

	return QueryPackageCommands(selectString);
}


ArrayList *MOG_DBPackageCommandAPI::GetScheduledPackageCommands(String *branchName, String *platform)
{
	String *selectString;
	
	// Get related package commands for this jobLabel, BranchName and platform
	selectString = String::Concat(
		S"SELECT * FROM ", MOG_ControllerSystem::GetDB()->GetPackageCommandsTable(),
		S" WHERE BranchName='", MOG_DBAPI::FixSQLParameterString(branchName),
			S"' AND Platform='", MOG_DBAPI::FixSQLParameterString(platform),
			S"' ORDER BY ID" );

	return QueryPackageCommands(selectString);
}


ArrayList *MOG_DBPackageCommandAPI::GetScheduledPackageCommands(String *jobLabel, String *branchName, String *platform)
{
	String *selectString;
	
	// Get related package commands for this jobLabel, BranchName and platform
	selectString = String::Concat(
		S"SELECT * FROM ", MOG_ControllerSystem::GetDB()->GetPackageCommandsTable(),
		S" WHERE Label='", MOG_DBAPI::FixSQLParameterString(jobLabel),
			S"' AND BranchName='", MOG_DBAPI::FixSQLParameterString(branchName),
			S"' AND Platform='", MOG_DBAPI::FixSQLParameterString(platform),
			S"' ORDER BY ID" );

	return QueryPackageCommands(selectString);
}


ArrayList *MOG_DBPackageCommandAPI::GetScheduledPackageCommands(String *packageName, String *jobLabel, String *branchName, String *platform)
{
	String *selectString;

	// Get related package commands for this jobLabel, BranchName, platform and PackageName
	selectString = String::Concat(
		S"SELECT * FROM ", MOG_ControllerSystem::GetDB()->GetPackageCommandsTable(),
		S" WHERE Label='", MOG_DBAPI::FixSQLParameterString(jobLabel), 
			S"' AND BranchName='", MOG_DBAPI::FixSQLParameterString(branchName), 
			S"' AND Platform='", MOG_DBAPI::FixSQLParameterString(platform), 
			S"' AND PackageName='", MOG_DBAPI::FixSQLParameterString(packageName),
			S"' ORDER BY ID" );

	return QueryPackageCommands(selectString);
}

ArrayList *MOG_DBPackageCommandAPI::GetScheduledLateResolverCommands(String *packageName, String *branchName)
{
	String *selectString;

	// Get related package commands for this BranchName, PackageName
	selectString = String::Concat(
		S"SELECT * FROM ", MOG_ControllerSystem::GetDB()->GetPackageCommandsTable(),
		S" WHERE Label='", S"LateResolver", 
			S"' AND BranchName='", MOG_DBAPI::FixSQLParameterString(branchName), 
			S"' AND (PackageName='", MOG_DBAPI::FixSQLParameterString(packageName), S"' OR AssetName='", MOG_DBAPI::FixSQLParameterString(packageName), S"')",
			S" ORDER BY ID" );

	return QueryPackageCommands(selectString);
}


ArrayList *MOG_DBPackageCommandAPI::GetScheduledLateResolverCommands(String *packageName, String *branchName, String *platform)
{
	String *selectString;

	// Get related package commands for this jobLabel, BranchName, platform and PackageName
	selectString = String::Concat(
		S"SELECT * FROM ", MOG_ControllerSystem::GetDB()->GetPackageCommandsTable(),
		S" WHERE Label='", S"LateResolver", 
			S"' AND BranchName='", MOG_DBAPI::FixSQLParameterString(branchName), 
			S"' AND Platform='", MOG_DBAPI::FixSQLParameterString(platform), 
			S"' AND (PackageName='", MOG_DBAPI::FixSQLParameterString(packageName), S"' OR AssetName='", MOG_DBAPI::FixSQLParameterString(packageName), S"')",
			S" ORDER BY ID" );

	return QueryPackageCommands(selectString);
}


bool MOG_DBPackageCommandAPI::RemovePackageCommand(MOG_DBPackageCommandInfo *packageCommand)
{
	// Make sure we have a valid package specified?
	if (packageCommand)
	{
		return RemovePackageCommand(packageCommand->mID);
	}

	return false;
}

bool MOG_DBPackageCommandAPI::RemovePackageCommand(MOG_Filename *blessedAssetFilename)
{
	return RemovePackageCommand(blessedAssetFilename, S"", S"", "");
}

bool MOG_DBPackageCommandAPI::RemovePackageCommand(MOG_Filename *blessedAssetFilename, String *branchName)
{
	return RemovePackageCommand(blessedAssetFilename, S"", branchName, "");
}

bool MOG_DBPackageCommandAPI::RemovePackageCommand(MOG_Filename *blessedAssetFilename, String *jobLabel, String *branchName, String *platformName)
{
	String *deleteCmd = String::Concat(	S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetPackageCommandsTable(), 
										S" WHERE ");
	String *whereCmd = "";
	if (blessedAssetFilename)
	{
		if (blessedAssetFilename->GetAssetFullName()->Length)
		{
			if (whereCmd->Length) whereCmd = String::Concat(whereCmd, S" AND ");
			whereCmd = String::Concat(whereCmd, S" AssetName='", MOG_DBAPI::FixSQLParameterString(blessedAssetFilename->GetAssetFullName()), S"'");
		}
		if (blessedAssetFilename->GetVersionTimeStamp()->Length)
		{
			if (whereCmd->Length) whereCmd = String::Concat(whereCmd, S" AND ");
			whereCmd = String::Concat(whereCmd, S" AssetVersionTimestamp='", blessedAssetFilename->GetVersionTimeStamp(), S"'");
		}
	}
	if (jobLabel->Length)
	{
		if (whereCmd->Length) whereCmd = String::Concat(whereCmd, S" AND ");
		whereCmd = String::Concat(whereCmd, S" Label='", MOG_DBAPI::FixSQLParameterString(jobLabel), S"'");
	}
	if (branchName->Length)
	{
		if (whereCmd->Length) whereCmd = String::Concat(whereCmd, S" AND ");
		whereCmd = String::Concat(whereCmd, S" BranchName='", MOG_DBAPI::FixSQLParameterString(branchName), S"'");
	}
	if (platformName->Length)
	{
		if (whereCmd->Length) whereCmd = String::Concat(whereCmd, S" AND ");
		whereCmd = String::Concat(whereCmd, S" Platform='", MOG_DBAPI::FixSQLParameterString(platformName), S"'");
	}
	deleteCmd = String::Concat(deleteCmd, whereCmd);
		
	return MOG_DBAPI::RunNonQuery(deleteCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
}


bool MOG_DBPackageCommandAPI::RemovePackageCommand(UInt32 commandId)
{
	String *deleteCmd = String::Concat(S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetPackageCommandsTable(), S" WHERE id='", commandId.ToString(), S"'");

	return MOG_DBAPI::RunNonQuery(deleteCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
}


bool MOG_DBPackageCommandAPI::RemovePackageCommands(ArrayList *packageCommands)
{
	bool bFailed = false;

	// Make sure we have something to do?
	if (packageCommands && packageCommands->Count)
	{
		// Walk through the specified packages
		for (int p = 0; p < packageCommands->Count; p++)
		{
			MOG_DBPackageCommandInfo *packageCommand = __try_cast<MOG_DBPackageCommandInfo *>(packageCommands->Item[p]);
			if (!RemovePackageCommand(packageCommand))
			{
				bFailed = true;
			}
		}
	}

	// Check if we ever failed?
	if (!bFailed)
	{
		return true;
	}

	return false;
}


bool MOG_DBPackageCommandAPI::RemoveStalePackageCommands(ArrayList *recentlyRemovedCommands, String *branchName)
{
	bool bFailed = false;

	// Make sure we have something to do?
	if (recentlyRemovedCommands && recentlyRemovedCommands->Count)
	{
		// Get all the remaining post commands for this branch
		ArrayList *remainingPackageCommands = MOG_DBPackageCommandAPI::GetScheduledPackageCommands(branchName);
		if (remainingPackageCommands && remainingPackageCommands->Count)
		{
			// Walk through the specified posts
			for (int p = 0; p < recentlyRemovedCommands->Count; p++)
			{
				MOG_DBPackageCommandInfo *recentlyRemovedCommand = __try_cast<MOG_DBPackageCommandInfo *>(recentlyRemovedCommands->Item[p]);

				// Walk through the remaining posts
				for (int r = 0; r < remainingPackageCommands->Count; r++)
				{
					MOG_DBPackageCommandInfo *remainingPackageCommand = __try_cast<MOG_DBPackageCommandInfo *>(remainingPackageCommands->Item[r]);

					// Check if this remaining post matches? and
					// Check if this package command is older?
					// Check if this package command was from a different JobLabel?
					if (String::Compare(remainingPackageCommand->mAssetName, recentlyRemovedCommand->mAssetName, true) == 0 &&
						remainingPackageCommand->mID < recentlyRemovedCommand->mID &&
						String::Compare(remainingPackageCommand->mLabel, recentlyRemovedCommand->mLabel, true) != 0)
					{
						// Remove this older post command because a newer version was just posted
						if (!RemovePackageCommand(remainingPackageCommand))
						{
							bFailed = true;
						}
					}
				}
			}
		}
	}

	// Check if we ever failed?
	if (!bFailed)
	{
		return true;
	}

	return false;
}


MOG_DBPackageCommandInfo *MOG_DBPackageCommandAPI::SQLCreatePackageCommand(String *assetName, String *assetVersion, String *jobLabel, String *branchName, String *platform, String *createdBy, String *blessedBy, String *packageName, String *packageGroups, String *packageObjects, MOG_PACKAGECOMMAND_TYPE packageCommandType)
{
	// check if it exists already
	String *selectCmd = String::Concat(
		S"SELECT ID FROM ", MOG_ControllerSystem::GetDB()->GetPackageCommandsTable(),
		S" WHERE (AssetName = '", MOG_DBAPI::FixSQLParameterString(assetName), S"') AND ",
			S"(AssetVersionTimestamp = '", MOG_DBAPI::FixSQLParameterString(assetVersion), S"') AND ",
			S"(Label = '", MOG_DBAPI::FixSQLParameterString(jobLabel), S"') AND ",
			S"(BranchName = '", MOG_DBAPI::FixSQLParameterString(branchName), S"') AND ",
			S"(Platform = '", MOG_DBAPI::FixSQLParameterString(platform), S"') AND ",
			S"(CreatedBy = '", MOG_DBAPI::FixSQLParameterString(createdBy), S"') AND ",
			S"(BlessedBy = '", MOG_DBAPI::FixSQLParameterString(blessedBy), S"') AND ",
			S"(PackageName = '", MOG_DBAPI::FixSQLParameterString(packageName), S"') AND "
			S"(PackageGroups = '", MOG_DBAPI::FixSQLParameterString(packageGroups), S"') AND ",
			S"(PackageObjects = '", MOG_DBAPI::FixSQLParameterString(packageObjects), S"') AND ",
			S"(PackageCommandType = ", Convert::ToString((int)packageCommandType), S")" );

	unsigned int packageCommandID = MOG_DBAPI::QueryInt(selectCmd, S"ID");

	// dont add duplicates
	if( packageCommandID == 0 )
	{
		// Build an SQL Insert statement string for all the input-form
		String *insertCmd = String::Concat(S"INSERT INTO ", MOG_ControllerSystem::GetDB()->GetPackageCommandsTable(),
			S" ( AssetName, AssetVersionTimestamp, Label, BranchName, Platform, CreatedBy, BlessedBy, PackageName, PackageGroups, PackageObjects, PackageCommandType) VALUES ",
			S" ('",MOG_DBAPI::FixSQLParameterString(assetName) , S"','",MOG_DBAPI::FixSQLParameterString(assetVersion), S"', '",MOG_DBAPI::FixSQLParameterString(jobLabel) , S"', '", MOG_DBAPI::FixSQLParameterString(branchName), S"', '", MOG_DBAPI::FixSQLParameterString(platform), S"', '", MOG_DBAPI::FixSQLParameterString(createdBy) , S"', '", MOG_DBAPI::FixSQLParameterString(blessedBy) , S"', '", MOG_DBAPI::FixSQLParameterString(packageName) , S"', '",MOG_DBAPI::FixSQLParameterString(packageGroups), S"', '", MOG_DBAPI::FixSQLParameterString(packageObjects), S"',",__box((int)packageCommandType), S")");

		MOG_DBAPI::RunNonQuery(insertCmd, MOG_ControllerSystem::GetDB()->GetConnectionString()); 
	}

	// Create a MOG_DBPackageCommandInfo to return back
	MOG_DBPackageCommandInfo *packageInfo = new MOG_DBPackageCommandInfo;
	packageInfo->mID = packageCommandID;
	packageInfo->mAssetName = assetName;
	packageInfo->mAssetVersion = assetVersion;
	packageInfo->mLabel = jobLabel;
	packageInfo->mBranchName = branchName;
	packageInfo->mPlatform = platform;
	packageInfo->mCreatedBy = createdBy;
	packageInfo->mBlessedBy = blessedBy;
	packageInfo->mPackageName = packageName;
	packageInfo->mPackageGroups = packageGroups;
	packageInfo->mPackageObjects = packageObjects;
	packageInfo->mPackageCommandType = packageCommandType;

	return packageInfo;
}


MOG_DBPackageCommandInfo *MOG_DBPackageCommandAPI::QueryPackageCommand(String *selectString)
{
	SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());

	MOG_DBPackageCommandInfo *packageInfo = NULL;

	try
	{
		SqlDataReader *myReader = MOG_DBAPI::GetNewDataReader(selectString, myConnection);
		while(myReader->Read())
		{
			packageInfo = SQLReadPackageCommand(myReader);
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

	return packageInfo;
}


ArrayList *MOG_DBPackageCommandAPI::QueryPackageCommands(String *selectString)
{
	SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());
	ArrayList *packages = NULL;

	try
	{
		packages = new ArrayList();

		SqlDataReader *myReader = MOG_DBAPI::GetNewDataReader(selectString, myConnection);
		while(myReader->Read())
		{
			packages->Add(SQLReadPackageCommand(myReader));
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

	return packages;
}


MOG_DBPackageCommandInfo *MOG_DBPackageCommandAPI::SQLReadPackageCommand(SqlDataReader *myReader)
{
	MOG_DBPackageCommandInfo *packageInfo = new MOG_DBPackageCommandInfo;

	packageInfo->mID = myReader->GetInt32(myReader->GetOrdinal(S"ID"));
	packageInfo->mAssetName = myReader->GetString(myReader->GetOrdinal(S"AssetName"))->Trim();
	packageInfo->mAssetVersion = myReader->GetString(myReader->GetOrdinal(S"AssetVersionTimestamp"))->Trim();
	packageInfo->mLabel = myReader->GetString(myReader->GetOrdinal(S"Label"))->Trim();
	packageInfo->mBranchName = myReader->GetString(myReader->GetOrdinal(S"BranchName"))->Trim();
	packageInfo->mPlatform = myReader->GetString(myReader->GetOrdinal(S"Platform"))->Trim();
	packageInfo->mCreatedBy = myReader->GetString(myReader->GetOrdinal(S"CreatedBy"))->Trim();
	packageInfo->mBlessedBy = myReader->GetString(myReader->GetOrdinal(S"BlessedBy"))->Trim();
	packageInfo->mPackageName = myReader->GetString(myReader->GetOrdinal(S"PackageName"))->Trim();
	packageInfo->mPackageGroups = myReader->GetString(myReader->GetOrdinal(S"PackageGroups"))->Trim();
	packageInfo->mPackageObjects = myReader->GetString(myReader->GetOrdinal(S"PackageObjects"))->Trim();
	packageInfo->mPackageCommandType = (MOG_PACKAGECOMMAND_TYPE)myReader->GetInt32(myReader->GetOrdinal(S"PackageCommandType"));

	return packageInfo;
}


bool MOG_DBPackageCommandAPI::SQLWritePackageCommand(String *assetName, String *assetVersion, String *jobLabel, String *branchName, String *platform, String *createdBy, String *blessedBy, String *packageName, String *packageGroups, String *packageObjects, MOG_PACKAGECOMMAND_TYPE packageCommandType)
{
	String *updateCmd = String::Concat(S"UPDATE ", MOG_ControllerSystem::GetDB()->GetPackageCommandsTable(), S" SET");

	// Variables
	updateCmd = String::Concat(updateCmd, S" AssetName='", MOG_DBAPI::FixSQLParameterString(assetName), S"',");
	updateCmd = String::Concat(updateCmd, S" AssetVersionTimestamp='", MOG_DBAPI::FixSQLParameterString(assetVersion), S"',");
	updateCmd = String::Concat(updateCmd, S" Label='", MOG_DBAPI::FixSQLParameterString(jobLabel), S"',");
	updateCmd = String::Concat(updateCmd, S" BranchName='", MOG_DBAPI::FixSQLParameterString(branchName), S"',");
	updateCmd = String::Concat(updateCmd, S" Platform='", MOG_DBAPI::FixSQLParameterString(platform), S"',");
	updateCmd = String::Concat(updateCmd, S" CreatedBy='", MOG_DBAPI::FixSQLParameterString(createdBy), S"',");
	updateCmd = String::Concat(updateCmd, S" BlessedBy='", MOG_DBAPI::FixSQLParameterString(blessedBy), S"',");
	updateCmd = String::Concat(updateCmd, S" PackageName='", MOG_DBAPI::FixSQLParameterString(packageName), S"',");
	updateCmd = String::Concat(updateCmd, S" PackageGroups='", MOG_DBAPI::FixSQLParameterString(packageGroups), S"',");
	updateCmd = String::Concat(updateCmd, S" PackageObjects='", MOG_DBAPI::FixSQLParameterString(packageObjects), S"',");
	updateCmd = String::Concat(updateCmd, S" PackageCommandType=", Convert::ToString((int)(packageCommandType)), S" ");
	
	// Where clause
	updateCmd = String::Concat(updateCmd, S" WHERE AssetName='", MOG_DBAPI::FixSQLParameterString(assetName), S"' and BranchName='", MOG_DBAPI::FixSQLParameterString(branchName), S"' and Platform='", MOG_DBAPI::FixSQLParameterString(platform), S"'");

	return MOG_DBAPI::RunNonQuery(updateCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
}


