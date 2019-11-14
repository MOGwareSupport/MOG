
//--------------------------------------------------------------------------------
//	MOG_DBInboxAPI.cpp
//	
//	
//--------------------------------------------------------------------------------
#include "stdafx.h"


#include "MOG_ControllerSystem.h"
#include "MOG_ControllerProject.h"
#include "MOG_DBAssetAPI.h"

#include "MOG_DBInboxAPI.h"



String *MOG_DBInboxAPI::GetUserName(MOG_Filename *assetFilename)
{
	// Attempt to use the user from the the assetFilename?
	String *userName = assetFilename->GetUserName();
	if (!userName->Length)
	{
		// At this point, let's default to the user that is logged in
		if (MOG_ControllerProject::IsUser())
		{
			userName = MOG_ControllerProject::GetUser()->GetUserName();
		}
	}

	return userName;
}


String *MOG_DBInboxAPI::GetBoxName(MOG_Filename *assetFilename)
{
	// Attempt to use the boxName from the assetFilename?
	String *boxName = assetFilename->GetBoxName();
	if (!boxName->Length)
	{
		// At this point determin if this could be a local directory?
		if (assetFilename->IsLocal())
		{
			// Force the boxName to be 'Local.{ComputerName}'
			boxName = String::Concat(S"Local.", MOG_ControllerSystem::GetComputerName());
		}
	}

	return boxName;
}


bool MOG_DBInboxAPI::InboxAddAsset(MOG_Filename *assetFilename)
{
	bool bAdded = true;
	
	String *insertCmd;

	// Identify if this asset is already listed?
	int inboxAssetID = GetInboxAssetID(assetFilename);
	if( inboxAssetID == 0 )
	{
		// Identify this user's ID?
		int userID = MOG_DBProjectAPI::GetUserID(GetUserName(assetFilename));
		if( userID != 0 )
		{
//? MOG_DBInboxAPI::InboxAddAsset - This code can throw anytime another mog client beats us to the punch when adding the inbox asset ID of this asset.  Can't we find an SQL query that will only insert it if it isn't already there?
			insertCmd = String::Concat(S"INSERT INTO ", MOG_ControllerSystem::GetDB()->GetInboxAssetsTable(),
									S" ( UserID, Box, GroupPath, AssetFullName ) VALUES ",
									S" (",__box(userID), S",'",MOG_DBAPI::FixSQLParameterString(GetBoxName(assetFilename)) , S"', '",MOG_DBAPI::FixSQLParameterString(assetFilename->GetGroupTree()) , S"', '",MOG_DBAPI::FixSQLParameterString(assetFilename->GetRelativePathWithinInbox()), S"')" );

			return MOG_DBAPI::RunNonQuery(insertCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
		}
	}

	return bAdded;
}

bool MOG_DBInboxAPI::InboxMoveAsset(MOG_Filename *sourceAssetFilename, MOG_Filename *destinationAssetFilename)
{
	bool bMoved = true;
	if( sourceAssetFilename == NULL || destinationAssetFilename == NULL )
	{
		return false;
	}

	// Check if they are the same?
	if( String::Compare(sourceAssetFilename->GetFullFilename(), destinationAssetFilename->GetFullFilename(), true) == 0 )
	{
		// No move needed because the source and destination are identical
		return bMoved;
	}
	int sourceAssetID = GetInboxAssetID(sourceAssetFilename);
	int destinationInboxAssetID = GetInboxAssetID(destinationAssetFilename);

	if( destinationInboxAssetID != 0 )
	{
		// if our destination already exists then remove it first
		InboxRemoveAsset(destinationAssetFilename);
	}

	String *updateCmd;

	int sourceUserID = MOG_DBProjectAPI::GetUserID(GetUserName(sourceAssetFilename));
	int destinationUserID = MOG_DBProjectAPI::GetUserID(GetUserName(destinationAssetFilename));

	if( sourceUserID != 0 && destinationUserID != 0 )
	{
		updateCmd = String::Concat(	S"UPDATE ", MOG_ControllerSystem::GetDB()->GetInboxAssetsTable(), S" SET",
									S" UserID = '", destinationUserID, S"', ", 
									S" Box = '", MOG_DBAPI::FixSQLParameterString(GetBoxName(destinationAssetFilename)), S"', ", 
									S" GroupPath = '", MOG_DBAPI::FixSQLParameterString(destinationAssetFilename->GetGroupTree()), S"', ",
									S" AssetFullName = '", MOG_DBAPI::FixSQLParameterString(destinationAssetFilename->GetRelativePathWithinInbox()), S"'",
									S" WHERE (UserID = '", sourceUserID, S"') AND ",
									S"(Box = '", MOG_DBAPI::FixSQLParameterString(GetBoxName(sourceAssetFilename)),S"') AND ",
									S"(GroupPath = '", MOG_DBAPI::FixSQLParameterString(sourceAssetFilename->GetGroupTree()), S"') AND ",
									S"(AssetFullName = '", MOG_DBAPI::FixSQLParameterString(sourceAssetFilename->GetRelativePathWithinInbox()), S"')" );

		if(MOG_DBAPI::RunNonQuery(updateCmd, MOG_ControllerSystem::GetDB()->GetConnectionString()))
		{
			MOG_ControllerSystem::GetDB()->GetDBCache()->GetInboxAssetCache()->UpdateSetByName(String::Concat(sourceAssetFilename->GetUserName(), S"#", sourceAssetFilename->GetRelativePathWithinInbox()), String::Concat(destinationAssetFilename->GetUserName(), S"#", destinationAssetFilename->GetRelativePathWithinInbox()));
			return true;
		}
	}

	return false;
}

bool MOG_DBInboxAPI::InboxRemoveAsset(MOG_Filename *assetFilename)
{
	bool success = true;

	if( assetFilename == NULL )
	{
		return false;
	}

	// clean up any properties
	if( !RemoveAllInboxAssetProperties(assetFilename) )
	{
		return false;
	}

	int userID = MOG_DBProjectAPI::GetUserID(GetUserName(assetFilename));

	// remove inbox asset
	String *queryText = String::Concat(S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetInboxAssetsTable(),
										S" WHERE ",
										S"     (UserID=", Convert::ToString(userID), S") ",
										S" AND (Box='", MOG_DBAPI::FixSQLParameterString(GetBoxName(assetFilename)), S"') ",
										S" AND (GroupPath='", MOG_DBAPI::FixSQLParameterString(assetFilename->GetGroupTree()), S"') ",
										S" AND (AssetFullName='", MOG_DBAPI::FixSQLParameterString(assetFilename->GetRelativePathWithinInbox()), S"')" );
		
	if( MOG_DBAPI::RunNonQuery(queryText, MOG_ControllerSystem::GetDB()->GetConnectionString()))
	{
		success = true;
		int inboxAssetID = GetInboxAssetID(assetFilename);
		if( inboxAssetID )
		{
			MOG_ControllerSystem::GetDB()->GetDBCache()->GetInboxAssetCache()->RemoveSetFromCacheByID(inboxAssetID);
		}
	}
	return success;
}

bool MOG_DBInboxAPI::InboxRemoveAllAssets(String *boxName, String *userName, String *groupPath, String *filter)
{
	ArrayList *allAssets = InboxGetAssetList(boxName, userName, groupPath, filter);
	if (allAssets)
	{
		for( int i = 0; i < allAssets->Count; i++ )
		{
			if( !InboxRemoveAsset(__try_cast<MOG_Filename*>(allAssets->Item[i])) )
			{
				return false;
			}
		}
		return true;
	}

	return false;
}

bool MOG_DBInboxAPI::InboxRemoveAllAssets(MOG_Filename *boxFilename)
{
	ArrayList *allAssets = InboxGetAssetList(boxFilename);
	if (allAssets)
	{
		for( int i = 0; i < allAssets->Count; i++ )
		{
			if( !InboxRemoveAsset(__try_cast<MOG_Filename*>(allAssets->Item[i])) )
			{
				return false;
			}
		}
		return true;
	}

	return false;
}

String *MOG_DBInboxAPI::InboxGetAssetName(int inboxID)
{
	String *selectCmd = String::Concat( S"SELECT InboxAssets.AssetFullName ",
										S"FROM ", MOG_ControllerSystem::GetDB()->GetInboxAssetsTable(), S" InboxAssets ",
										S"WHERE (InboxAssets.ID = ", inboxID.ToString(), S")");

	return MOG_DBAPI::QueryString(selectCmd, "AssetFullName");
}

ArrayList *MOG_DBInboxAPI::InboxGetAssetListFromProperty(String *boxName, String *assetLabel, MOG_Property *property)
{
	String* userName = MOG_ControllerProject::GetUserName_DefaultAdmin();

	String *selectCmd = String::Concat( S"SELECT InboxAssets.AssetFullName ",
										S"FROM ", MOG_ControllerSystem::GetDB()->GetInboxAssetsTable(), S" InboxAssets INNER JOIN ",
												  MOG_ControllerSystem::GetDB()->GetInboxAssetsPropertiesTable(), S" InboxAssetsProperties ON InboxAssets.ID = InboxAssetsProperties.InboxAssetID INNER JOIN ",
												  MOG_ControllerSystem::GetDB()->GetUsersTable(), S" Users ON InboxAssets.UserID = Users.ID ",
										S"WHERE (InboxAssets.AssetFullName LIKE '%}", assetLabel, S"%') AND ",
												S"(Users.UserName = '", MOG_DBAPI::FixSQLParameterString(userName), S"') AND ",
												S"(InboxAssets.Box = '", boxName, S"') ");

	if (property->mSection->Contains("*"))
	{
		selectCmd = String::Concat(selectCmd, S" AND (InboxAssetsProperties.Section LIKE '", MOG_DBAPI::FixSQLParameterString(property->mSection->Replace("*", "%")), S"')");
	}
	else
	{
		selectCmd = String::Concat(selectCmd, S" AND (InboxAssetsProperties.Section = '", MOG_DBAPI::FixSQLParameterString(property->mSection), S"')");
	}
	if (property->mKey->Contains("*"))
	{
		selectCmd = String::Concat(selectCmd, S" AND (InboxAssetsProperties.Property LIKE '", MOG_DBAPI::FixSQLParameterString(property->mKey->Replace("*", "%")), S"')");
	}
	else
	{
		selectCmd = String::Concat(selectCmd, S" AND (InboxAssetsProperties.Property = '", MOG_DBAPI::FixSQLParameterString(property->mKey), S"')");
	}
	// Ignore the keyword 'None' - This keyword is used in package assignments and will never match the database
	if (String::Compare(property->mPropertyValue, S"None", true) != 0)
	{
		if (property->mPropertyValue->Contains("*"))
		{
			selectCmd = String::Concat(selectCmd, S" AND (InboxAssetsProperties.Value LIKE '", MOG_DBAPI::FixSQLParameterString(property->mPropertyValue->Replace("*", "%")), S"')");
		}
		else
		{
			selectCmd = String::Concat(selectCmd, S" AND (InboxAssetsProperties.Value = '", MOG_DBAPI::FixSQLParameterString(property->mPropertyValue), S"')");
		}
	}

	return MOG_DBAPI::QueryStrings(selectCmd, "AssetFullName");
}

ArrayList *MOG_DBInboxAPI::InboxGetAssetListFromImportedFilename(String *userName, String *filename)
{
	String *selectCmd = String::Concat( S"SELECT InboxAssets.AssetFullName ",
										S"FROM ", MOG_ControllerSystem::GetDB()->GetInboxAssetsTable(), S" InboxAssets INNER JOIN ",
												  MOG_ControllerSystem::GetDB()->GetInboxAssetsPropertiesTable(), S" InboxAssetsProperties ON InboxAssets.ID = InboxAssetsProperties.InboxAssetID INNER JOIN ",
												  MOG_ControllerSystem::GetDB()->GetUsersTable(), S" Users ON InboxAssets.UserID = Users.ID ",
										S"WHERE (InboxAssetsProperties.Section = 'Files.Imported') AND ",
												S"((InboxAssetsProperties.Property = '", MOG_DBAPI::FixSQLParameterString(filename), S"') OR (InboxAssetsProperties.Property LIKE '%\\", MOG_DBAPI::FixSQLParameterString(filename), S"')) AND ",
												S"(Users.UserName = '", MOG_DBAPI::FixSQLParameterString(userName), S"') AND ",
												S"(InboxAssets.Box <> 'Trash')");

	return MOG_DBAPI::QueryStrings(selectCmd, "AssetFullName");
}

ArrayList *MOG_DBInboxAPI::InboxGetAssetListFromImportedDirectory(String *userName, String *dirname)
{
	String *selectCmd = String::Concat( S"SELECT InboxAssets.AssetFullName ",
										S"FROM ", MOG_ControllerSystem::GetDB()->GetInboxAssetsTable(), S" InboxAssets INNER JOIN ",
												  MOG_ControllerSystem::GetDB()->GetInboxAssetsPropertiesTable(), S" InboxAssetsProperties ON InboxAssets.ID = InboxAssetsProperties.InboxAssetID INNER JOIN ",
												  MOG_ControllerSystem::GetDB()->GetUsersTable(), S" Users ON InboxAssets.UserID = Users.ID ",
										S"WHERE (InboxAssetsProperties.Section = 'Files.Imported') AND ",
												S"(InboxAssetsProperties.Property LIKE '", MOG_DBAPI::FixSQLParameterString(dirname), S"%\\') AND ",
												S"(Users.UserName = '", MOG_DBAPI::FixSQLParameterString(userName), S"') AND ",
												S"(InboxAssets.Box <> 'Trash')");

	return MOG_DBAPI::QueryStrings(selectCmd, "AssetFullName");
}

ArrayList *MOG_DBInboxAPI::InboxGetAssetList()
{
	return InboxGetAssetList("", "", "", NULL);
}

ArrayList *MOG_DBInboxAPI::InboxGetAssetList(MOG_Filename *boxFilename)
{
	String *boxName = GetBoxName(boxFilename);
	String *userName = GetUserName(boxFilename);
	String *groupPath = boxFilename->GetGroupTree();
	
	return InboxGetAssetList(boxName, userName, groupPath, NULL);
}

ArrayList *MOG_DBInboxAPI::InboxGetAssetList(String *boxName, String *userName, String *groupPath, String *filter)
{
	String *selectString = String::Concat(
		S"SELECT Users.UserName, InboxAssets.Box, InboxAssets.GroupPath, InboxAssets.AssetFullName ",
		S"FROM ", MOG_ControllerSystem::GetDB()->GetInboxAssetsTable(), S" InboxAssets INNER JOIN ",
			MOG_ControllerSystem::GetDB()->GetUsersTable(), S" Users ON InboxAssets.UserID = Users.ID ");

	if (userName != NULL && userName->Length > 0)
	{
		selectString = String::Concat(selectString, S"WHERE (Users.UserName = '", MOG_DBAPI::FixSQLParameterString(userName), S"')" );
	}

	if( boxName != NULL && boxName->Length > 0 )
	{
		// If we are local, append the machine name to the query
		if (String::Compare(boxName, "local", true) == 0)
		{
			boxName = String::Concat(S"Local.", MOG_ControllerSystem::GetComputerName());
		}

		// Make sure we have a where clause
		if (selectString->IndexOf("WHERE") == -1)
		{
			selectString = String::Concat(selectString, S"WHERE " );
		}
		else
		{
			selectString = String::Concat(selectString, S" AND " );
		}

		selectString = String::Concat( selectString, S"(InboxAssets.Box = '", MOG_DBAPI::FixSQLParameterString(boxName) , S"')" );
	}

	if( groupPath != NULL && groupPath->Length > 0 )
	{
		// Make sure we have a where clause
		if (selectString->IndexOf("WHERE") == -1)
		{
			selectString = String::Concat(selectString, S"WHERE " );
		}
		else
		{
			selectString = String::Concat(selectString, S" AND " );
		}

		selectString = String::Concat( selectString, S"(InboxAssets.GroupPath = '", MOG_DBAPI::FixSQLParameterString(groupPath), S"')" );
	}
	
	if( filter != NULL && filter->Length > 0 )
	{
		// Make sure we have a where clause
		if (selectString->IndexOf("WHERE") == -1)
		{
			selectString = String::Concat(selectString, S"WHERE " );
		}
		else
		{
			selectString = String::Concat(selectString, S" AND " );
		}

		if (filter->Contains(S"*"))
		{
			// Fixup the filter's wildcards
			filter = filter->Replace(S"*", "%");
			selectString = String::Concat( selectString, S"(InboxAssets.AssetFullName LIKE '", MOG_DBAPI::FixSQLParameterString(filter), S"')" );
		}
		else
		{
			selectString = String::Concat( selectString, S"(InboxAssets.AssetFullName = '", MOG_DBAPI::FixSQLParameterString(filter), S"')" );
		}
	}

	return QueryInboxAssets(selectString);
}

ArrayList *MOG_DBInboxAPI::InboxGetAssetListWithProperties(String *boxName, String *userName, String *groupPath, String *filter)
{
	String *selectCmd = String::Concat( S"SELECT Users.UserName, InboxAssets.Box, InboxAssets.GroupPath, InboxAssets.AssetFullName, ", MOG_ControllerSystem::GetDB()->GetInboxAssetsPropertiesTable(), S".* ",
										S"FROM ", MOG_ControllerSystem::GetDB()->GetInboxAssetsTable(), S" AS InboxAssets INNER JOIN ",
												MOG_ControllerSystem::GetDB()->GetUsersTable(), S" AS Users ON InboxAssets.UserID = Users.ID INNER JOIN ", 
												MOG_ControllerSystem::GetDB()->GetInboxAssetsPropertiesTable(), S" ON InboxAssets.ID = ", MOG_ControllerSystem::GetDB()->GetInboxAssetsPropertiesTable(), S".InboxAssetID " );

	// add Users WHERE clause
	bool haveUserName = (userName != NULL && userName->Length > 0);
	bool haveBoxName = (boxName != NULL && boxName->Length > 0);
	bool haveGroupPath = (groupPath != NULL && groupPath->Length > 0);
	bool haveFilter = (filter != NULL && filter->Length > 0);

	if( haveUserName || haveBoxName || haveGroupPath || haveFilter )
	{
		selectCmd  = String::Concat(selectCmd, S"WHERE " );
	}

	if( haveUserName )
	{
		selectCmd = String::Concat( selectCmd, S"(Users.UserName = '", MOG_DBAPI::FixSQLParameterString(userName) , S"')" );

		if( haveBoxName || haveGroupPath || haveFilter )
		{
			selectCmd = String::Concat( selectCmd, S" AND " );
		}
	}
	if( haveBoxName )
	{
		// If we are local, append the machine name to the query
		if (String::Compare(boxName, "local", true) == 0)
		{
			boxName = String::Concat(S"Local.", MOG_ControllerSystem::GetComputerName());
		}

		// Check if this box is trash?
		if (String::Compare(boxName, S"Trash", true) == 0)
		{
			selectCmd = String::Concat( selectCmd, S"(InboxAssets.Box = '", MOG_DBAPI::FixSQLParameterString(boxName) , S"')" );
		}
		else
		{
			// We can safely ignore the 'Trash' box...it saves us a lot of time since the trash is usually contains many old assets
			selectCmd = String::Concat( selectCmd, S"(InboxAssets.Box = '", MOG_DBAPI::FixSQLParameterString(boxName) , S"') AND (InboxAssets.Box <> 'Trash')" );
		}

		if( haveGroupPath || haveFilter )
		{
			selectCmd = String::Concat( selectCmd, S" AND " );
		}
	}
	if( haveGroupPath )
	{
		selectCmd = String::Concat( selectCmd, S"(InboxAssets.GroupPath = '", MOG_DBAPI::FixSQLParameterString(groupPath), S"')" );

		if( haveFilter )
		{
			selectCmd = String::Concat( selectCmd, S" AND " );
		}
	}
	if( haveFilter )
	{
		if (filter->Contains(S"*"))
		{
			// Fixup the filter's wildcards
			filter = filter->Replace(S"*", "%");
			selectCmd = String::Concat( selectCmd, S"(InboxAssets.AssetFullName LIKE '", MOG_DBAPI::FixSQLParameterString(filter), S"')" );
		}
		else
		{
			selectCmd = String::Concat( selectCmd, S"(InboxAssets.AssetFullName = '", MOG_DBAPI::FixSQLParameterString(filter), S"')" );
		}
	}

	// Order the list by InboxAsset.ID for faster parsing
	selectCmd = String::Concat( selectCmd, S"ORDER BY InboxAssets.ID" );

	return QueryInboxAssetsWithProperties(selectCmd);
}

ArrayList *MOG_DBInboxAPI::InboxGetLocalAssetList(String *workspaceDirectory)
{
	String *boxName = String::Concat(S"Local.", MOG_ControllerSystem::GetComputerName());
	String *userName = MOG_ControllerProject::GetUserName_DefaultAdmin();
	String *filter = String::Concat(workspaceDirectory, S"\\*");
	return InboxGetAssetListWithProperties(boxName, userName, S"", filter);
}

ArrayList *MOG_DBInboxAPI::InboxGetLocalAssetList(String *workspaceDirectory, MOG_Filename *assetFilename)
{
	String *boxName = String::Concat(S"Local.", MOG_ControllerSystem::GetComputerName());
	String *userName = MOG_ControllerProject::GetUserName_DefaultAdmin();
	String *filter = String::Concat(workspaceDirectory, S"\\MOG\\UpdatedTray\\", assetFilename->GetAssetFullName());
	return InboxGetAssetListWithProperties(boxName, userName, S"", filter);
}

bool MOG_DBInboxAPI::UpdateInboxAssetProperty(MOG_Filename *assetFilename, MOG_Property *property)
{
	return SQLCreateInboxAssetProperty(assetFilename, property->mSection, property->mKey, property->mPropertyValue);
}

bool MOG_DBInboxAPI::UpdateInboxAssetProperties(MOG_Filename* assetFilename, ArrayList* properties)
{
	bool success = false;

	// make sure we have a valid inboxAssetID?
	int inboxAssetID = GetInboxAssetID(assetFilename);
	if( inboxAssetID)
	{
		ArrayList* transactionCommands = new ArrayList();

		// Add command to clear out this asset's previous props
		String* deleteCmd = String::Concat( S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetInboxAssetsPropertiesTable(),
											S" WHERE (InboxAssetID = ", Convert::ToString(inboxAssetID), S")" );
		transactionCommands->Add(deleteCmd);

		// For each property, add it to our command
		for( int i = 0; i < properties->Count; i++ )
		{
			MOG_Property *prop = dynamic_cast<MOG_Property*>(properties->Item[i]);
			
			String* createCmd = String::Concat(	S"INSERT INTO ", MOG_ControllerSystem::GetDB()->GetInboxAssetsPropertiesTable(),
												S" ( InboxAssetID, Section, Property, Value ) VALUES ",
												S" (",__box(inboxAssetID), S",'", MOG_DBAPI::FixSQLParameterString(prop->mSection), S"', '", MOG_DBAPI::FixSQLParameterString(prop->mKey), S"','",MOG_DBAPI::FixSQLParameterString(prop->mValue) ,S"')" );
			transactionCommands->Add(createCmd);
		}

		// Execute the entire transaction
		success = MOG_DBAPI::ExecuteTransaction(transactionCommands);
	}

	return success;
}

bool MOG_DBInboxAPI::RemoveInboxAssetProperty(MOG_Filename *assetFilename, String *propertySection, String *propertyName)
{
	int inboxAssetID = GetInboxAssetID(assetFilename);

	String *deleteCmd = String::Concat( S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetInboxAssetsPropertiesTable(),
										S" WHERE (InboxAssetID = ", Convert::ToString(inboxAssetID), S") AND ",
											S"(Section = '", MOG_DBAPI::FixSQLParameterString(propertySection), S"') AND ",
											S"(Property = '", MOG_DBAPI::FixSQLParameterString(propertyName), S"')" );
	
	return MOG_DBAPI::RunNonQuery(deleteCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
}
bool MOG_DBInboxAPI::RemoveAllInboxAssetProperties(MOG_Filename *assetFilename)
{
	int inboxAssetID = GetInboxAssetID(assetFilename);

	String *deleteCmd = String::Concat( S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetInboxAssetsPropertiesTable(),
										S" WHERE (InboxAssetID = ", Convert::ToString(inboxAssetID), S")" );
	
	return MOG_DBAPI::RunNonQuery(deleteCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
}
bool MOG_DBInboxAPI::RemoveAllInboxAssetProperties(MOG_Filename *assetFilename, String *propertySection)
{
	int inboxAssetID = GetInboxAssetID(assetFilename);

	String *deleteCmd = String::Concat( S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetInboxAssetsPropertiesTable(),
										S" WHERE (InboxAssetID = ", Convert::ToString(inboxAssetID), S") AND ",
											S"(Section = '", MOG_DBAPI::FixSQLParameterString(propertySection), S"')" );
	
	return MOG_DBAPI::RunNonQuery(deleteCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
}
String *MOG_DBInboxAPI::GetInboxAssetProperty(MOG_Filename *assetFilename, String *propertySection, String *propertyName)
{
	int inboxAssetID = GetInboxAssetID(assetFilename);

	String *selectCmd = String::Concat( S"SELECT Section, Property, Value ",
										S"FROM ", MOG_ControllerSystem::GetDB()->GetInboxAssetsPropertiesTable(),
										S"WHERE (InboxAssetID = ", Convert::ToString(inboxAssetID), S") AND ",
											S"(Section = '", MOG_DBAPI::FixSQLParameterString(propertySection), S"') AND ",
											S"(Property = '", MOG_DBAPI::FixSQLParameterString(propertyName), S"')" );

	return MOG_DBAPI::QueryString(selectCmd, "Value");
}
ArrayList *MOG_DBInboxAPI::GetAllInboxAssetProperties(MOG_Filename *assetFilename)
{
	int inboxAssetID = GetInboxAssetID(assetFilename);

	String *selectCmd = String::Concat( S"SELECT Section, Property, Value ",
										S"FROM ", MOG_ControllerSystem::GetDB()->GetInboxAssetsPropertiesTable(),
										S"WHERE (InboxAssetID = ", Convert::ToString(inboxAssetID), S")" );

	return MOG_DBAssetAPI::QueryProperties(selectCmd);
}
ArrayList *MOG_DBInboxAPI::GetAllInboxAssetProperties(MOG_Filename *assetFilename, String *propertySection)
{
	int inboxAssetID = GetInboxAssetID(assetFilename);

	String *selectCmd = String::Concat( S"SELECT Section, Property, Value ",
										S"FROM ", MOG_ControllerSystem::GetDB()->GetInboxAssetsPropertiesTable(),
										S"WHERE (InboxAssetID = ", Convert::ToString(inboxAssetID), S") AND ",
											S"(Section = '", MOG_DBAPI::FixSQLParameterString(propertySection), S"')" );

	return MOG_DBAssetAPI::QueryProperties(selectCmd);
}

bool MOG_DBInboxAPI::UpdateCache(MOG_Filename *addFilename, MOG_Filename *delFilename)
{
	// Check if we have both an add and delete
	if(addFilename && delFilename)
	{
		String * newCacheName = String::Concat(addFilename->GetUserName(), S"#", addFilename->GetRelativePathWithinInbox());
		String * oldCacheName = String::Concat(delFilename->GetUserName(), S"#", delFilename->GetRelativePathWithinInbox());

		// Check if the name changed?
		if (String::Compare(newCacheName, oldCacheName, true) != 0)
		{
			return MOG_ControllerSystem::GetDB()->GetDBCache()->GetInboxAssetCache()->UpdateSetByName(oldCacheName, newCacheName);
		}
	}
	// Check if whe have only ad delete value (we won't hit this if we have both see above ^^)
	else if(delFilename)
	{
		String *delCacheName = String::Concat(delFilename->GetUserName(), S"#", delFilename->GetRelativePathWithinInbox());
		return MOG_ControllerSystem::GetDB()->GetDBCache()->GetInboxAssetCache()->RemoveSetFromCacheByName(delCacheName);
	}

	return false;
}

MOG_Filename *MOG_DBInboxAPI::SQLReadInboxAsset(SqlDataReader *myReader)
{
	// required columns: UserName, Box, GroupPath, AssetFullName

	String *userName = MOG_DBAPI::SafeStringRead(myReader,S"UserName")->Trim();
	String *box = MOG_DBAPI::SafeStringRead(myReader,S"Box")->Trim();

	String *userPath = S"";

	// Get user path if its not the local box
	if( box->ToLower()->IndexOf("local.") == -1 )
	{
		// Ask the project for this user
		MOG_User* user = MOG_ControllerProject::GetProject()->GetUser(userName);
		if (user)
		{
			userPath = user->GetUserPath();
		}
		else
		{
			// Worst case, let's at least preserve the user name
			userPath = String::Concat(S"Users\\", userName);
		}
	}

	MOG_Filename *asset = new MOG_Filename( String::Concat(userPath, MOG_DBAPI::SafeStringRead(myReader,S"AssetFullName")->Trim()) );
	
	return asset;
}

ArrayList* MOG_DBInboxAPI::QueryInboxAssets(String* selectString)
{
	SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());
	ArrayList *assets = new ArrayList();

	SqlDataReader *myReader = NULL;
	try
	{
		myReader = MOG_DBAPI::GetNewDataReader(selectString,myConnection);;
		while(myReader->Read())
		{
			assets->Add(SQLReadInboxAsset(myReader));
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

int MOG_DBInboxAPI::GetInboxAssetID(MOG_Filename *assetFilename)
{
	if( assetFilename == NULL )
	{
		return 0;
	}
	int retInboxAssetID = MOG_ControllerSystem::GetDB()->GetDBCache()->GetInboxAssetCache()->GetIDFromName(String::Concat(assetFilename->GetUserName(), S"#", assetFilename->GetRelativePathWithinInbox()));
	if(retInboxAssetID == 0)
	{
		String *selectString = String::Concat(
				S"SELECT InboxAssets.ID ",
				S"FROM ", MOG_ControllerSystem::GetDB()->GetInboxAssetsTable(), S" InboxAssets INNER JOIN ",
					MOG_ControllerSystem::GetDB()->GetUsersTable(), S" Users ON InboxAssets.UserID = Users.ID ",
				S"WHERE (Users.UserName = '", MOG_DBAPI::FixSQLParameterString(GetUserName(assetFilename)), S"') AND ",
					S"(InboxAssets.Box = '", MOG_DBAPI::FixSQLParameterString(GetBoxName(assetFilename)), S"') AND ",
					S"(InboxAssets.GroupPath = '", MOG_DBAPI::FixSQLParameterString(assetFilename->GetGroupTree()), S"') AND ",
					S"(InboxAssets.AssetFullName = '", MOG_DBAPI::FixSQLParameterString(assetFilename->GetRelativePathWithinInbox()), S"')" );


		retInboxAssetID = MOG_DBAPI::QueryInt(selectString, S"ID");
		if (retInboxAssetID > 0)
		{
			MOG_ControllerSystem::GetDB()->GetDBCache()->GetInboxAssetCache()->AddSetToCache(retInboxAssetID, String::Concat(assetFilename->GetUserName(), S"#", assetFilename->GetRelativePathWithinInbox()));
		}
	}
	return retInboxAssetID;
}

bool MOG_DBInboxAPI::SQLCreateInboxAssetProperty(MOG_Filename *assetFilename, String *propertySection, String *propertyName, String *propertyValue)
{
	bool created = false;
	int inboxAssetID = GetInboxAssetID(assetFilename);

	// does the asset version exist?
	if( inboxAssetID != 0 )
	{
		String *selectCmd = String::Concat( S"SELECT Value ",
											S"FROM ", MOG_ControllerSystem::GetDB()->GetInboxAssetsPropertiesTable(),
											S"WHERE (InboxAssetID = ", Convert::ToString(inboxAssetID), S") AND ",
												S"(Section = '", MOG_DBAPI::FixSQLParameterString(propertySection), S"') AND ",
												S"(Property = '", MOG_DBAPI::FixSQLParameterString(propertyName), S"')" );
		String *createCmd;

		// does it already exist?
		if( MOG_DBAPI::QueryExists(selectCmd) )
		{
			// just update
			createCmd = String::Concat( S"UPDATE ", MOG_ControllerSystem::GetDB()->GetInboxAssetsPropertiesTable(), S" SET ",
										S"Value='", MOG_DBAPI::FixSQLParameterString(propertyValue), S"' ",
										S"WHERE (InboxAssetID = ", Convert::ToString(inboxAssetID), S") AND ",
											S"(Section = '", MOG_DBAPI::FixSQLParameterString(propertySection), S"') AND ",
											S"(Property = '", MOG_DBAPI::FixSQLParameterString(propertyName), S"')" );
		}
		else
		{
			// add a new record
			createCmd = String::Concat(S"INSERT INTO ", MOG_ControllerSystem::GetDB()->GetInboxAssetsPropertiesTable(),
									S" ( InboxAssetID, Section, Property, Value ) VALUES ",
									S" (",__box(inboxAssetID), S",'", MOG_DBAPI::FixSQLParameterString(propertySection), S"', '", MOG_DBAPI::FixSQLParameterString(propertyName), S"','",MOG_DBAPI::FixSQLParameterString(propertyValue) ,S"')" );
		}

		return MOG_DBAPI::RunNonQuery(createCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
	}

	return created;
}

ArrayList* MOG_DBInboxAPI::QueryInboxAssetsWithProperties(String* selectString)
{
	SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());
	ArrayList *assets = new ArrayList();

	SqlDataReader *myReader = NULL;
	try
	{
		MOG_DBInboxProperties *inboxItem = NULL;
		int lastID = 0;

		myReader = MOG_DBAPI::GetNewDataReader(selectString, myConnection);
		while(myReader->Read())
		{
			// Read only the InboxAssetID
			int thisID = MOG_DBAPI::SafeIntRead(myReader, S"InboxAssetID");
			// Check if this the ID is changing?
			if (thisID != lastID)
			{
				// Obtain the full inboxAssetFilename of this new asset
				MOG_Filename *inboxAssetFilename = SQLReadInboxAsset(myReader);

				// Create a new inboxItem for this next item
				inboxItem = new MOG_DBInboxProperties();
				inboxItem->mAsset = inboxAssetFilename;
				inboxItem->mProperties = new ArrayList();
				assets->Add(inboxItem);

				// Retain thisID for the next round
				lastID = thisID;
			}

			// Read the property
			MOG_Property *inboxProperty = new MOG_Property();
			inboxProperty->SetSection(myReader->GetString(myReader->GetOrdinal(S"Section"))->Trim());
			inboxProperty->SetKey(myReader->GetString(myReader->GetOrdinal(S"Property"))->Trim());
			inboxProperty->SetValue(myReader->GetString(myReader->GetOrdinal(S"Value"))->Trim());

			// Add this property to our inboxItem
			inboxItem->mProperties->Add(inboxProperty);
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

