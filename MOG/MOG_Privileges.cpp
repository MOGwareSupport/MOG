#include "StdAfx.h"
#include "MOG_Privileges.h"
#include "MOG_ControllerProject.h"

#using <mscorlib.dll>

using namespace System::IO;
using namespace System::Collections;
using namespace MOG::CONTROLLER::CONTROLLERPROJECT;

MOG_Privileges::MOG_Privileges(void)
{
	// Set all our globally used strings
	mUserSection = (__box(MOG_Privileges::MainSections::Users))->ToString();
	mGroupSection = (__box(MOG_Privileges::MainSections::Groups))->ToString();
	mUserDefaultSection = (__box(MOG_Privileges::MainSections::DefaultUser))->ToString();
	mAdminSection = (__box(MOG_Privileges::MainSections::Admin))->ToString();
	mLeadSection = (__box(MOG_Privileges::MainSections::Lead))->ToString();

	MOG_Project* project = MOG_ControllerProject::GetProject();
	if (project)
	{
		String *filename = String::Concat(project->GetProjectToolsPath(), S"\\Configs\\Privileges.info");

		// TEMP CODE!!!
		// This was only needed while we transitioned from the incorrectly spelled filename
		// This can be changed to remove the old 'Priviledges.Info' file after 1-2 public drops
		if (true)
		{
			// Check if the new name is missing?
			if (!DosUtils::FileExistFast(filename))
			{
				// Make sure the old name exists?
				String *oldname = String::Concat(project->GetProjectToolsPath(), S"\\Configs\\Priviledges.info");
				if (DosUtils::FileExistFast(filename))
				{
					// Copy the old name forward to the correct spelling
					DosUtils::FileCopyFast(oldname, filename, true);
				}
			}
		}

		mPrivilegesIni = new MOG_Ini( filename );
	}
	else
	{
		mPrivilegesIni = new MOG_Ini();
	}

	ValidatePrivilegesIni();
}


MOG_Privileges::~MOG_Privileges(void)
{
	mPrivilegesIni = NULL;
}


// Returns empty ArrayList if [Users] section does not exist
SortedList *MOG_Privileges::GetUsers(void)
{
	SortedList *userList = new SortedList();

	// Check to see that [Users] exists
	if( mPrivilegesIni->SectionExist( mUserSection ) )
	{
		int userCount = mPrivilegesIni->CountKeys( mUserSection );

		// Foreach user in [Users]...
		for( int i = 0; i < userCount; ++i )
		{
			// Add user's name to userList
			userList->Add( mPrivilegesIni->GetKeyNameByIndexSLOW( mUserSection, i ), mPrivilegesIni->GetKeyByIndexSLOW( mUserSection, i ) );
		}
	}

	// Return our ArrayList
	return userList;
}


// Returns empty array if there are no groups
ArrayList *MOG_Privileges::GetGroups(void)
{
	ArrayList *groupList = new ArrayList();

	// Check to see that [Group] exists
	if( mPrivilegesIni->SectionExist( mGroupSection ) )
	{
		int groupsCount = mPrivilegesIni->CountKeys( mGroupSection );

		// Foreach group in [Group]
		for( int i = 0; i < groupsCount; ++i )
		{
			// Add group key to groupList
			groupList->Add( mPrivilegesIni->GetKeyNameByIndexSLOW( mGroupSection, i ) );
		}
	}

	// Return arraylist of groups
	return groupList;
}

void MOG_Privileges::ValidatePrivilegesIni( void )
{
	// If we have no sections or either of the mainSections do not exist, fill them...
	if( mPrivilegesIni->CountSections() < 1 || 
		!mPrivilegesIni->SectionExist( mUserSection ) || 
		!mPrivilegesIni->SectionExist( mGroupSection ) )
	{
		PopulateMainSections();
	}
	// Else, if we have a users section, validate it
	else if( mPrivilegesIni->SectionExist( mUserSection ) )
	{
		// This is equivalent to ValidateUsersSection(), which does not exist
		PopulateUsersSection();
	} // end if-else on sections and keys
} // end method

// Create all our base sections
void MOG_Privileges::PopulateMainSections( void )
{
	// Populate our Users section first
	PopulateUsersSection();

	// Determine whether or not we have a Groups section
	bool missingGroupSection = false;
	ArrayList *groupsList = new ArrayList();
	if( mPrivilegesIni->SectionExist( mGroupSection ) )
	{
		int groupsCount = mPrivilegesIni->CountKeys( mGroupSection );
		for( int i = 0; i < groupsCount; ++i )
		{
			groupsList->Add( mPrivilegesIni->GetKeyByIndexSLOW( mGroupSection, i) );
		}
	}
	else
	{
		// Make sure we build it
		missingGroupSection = true;
	}

	// If we do not have an Admin section, indicate it
	bool missingGroupAdmin = false;
	if( !mPrivilegesIni->SectionExist( mAdminSection ) )
	{
		// Make sure we build it
		missingGroupAdmin = true;
	}
	// If we do not have a Lead section, indicate it
	bool missingGroupLead = false;
	if( !mPrivilegesIni->SectionExist( mLeadSection ) )
	{
		// Make sure we build it
		missingGroupLead = true;
	}
	// If we do not have a Users section, indicate it
	bool missingGroupDefaultUser = false;
	if( !mPrivilegesIni->SectionExist( mUserDefaultSection ) )
	{
		// Make sure we build it
		missingGroupDefaultUser = true;
	}

	// If we are missing any of the above, open the ini file and add it
	if ( missingGroupSection || 
		 missingGroupAdmin || 
		 missingGroupLead || 
		 missingGroupDefaultUser)
	{
		// Open the file so we have exclusive rights
		MOG_Ini *temp = new MOG_Ini();
		if (temp->Open(mPrivilegesIni->GetFilename(), FileShare::Write))
		{
			if( missingGroupSection )
			{
				temp->PutString( mGroupSection, mAdminSection, S"" );
				temp->PutString( mGroupSection, mUserDefaultSection, S"" );
				temp->PutString( mGroupSection, mLeadSection, S"" );
			}
			if( missingGroupAdmin )
			{
				PopulateGroupPrivileges( temp, mAdminSection );
			}
			if( missingGroupLead )
			{
				PopulateGroupPrivileges( temp, mLeadSection );
			}
			if( missingGroupDefaultUser )
			{
				PopulateGroupPrivileges( temp, mUserDefaultSection );
			}

			// Release our exclusive access
			temp->Close();
		}

		// Reload our ram file
		mPrivilegesIni->Load();
	}
} // end method


void MOG_Privileges::PopulateUsersSection( void )
{
	// Initialize ArrayLists
	ArrayList *usersToAdd = new ArrayList();
	ArrayList *usersList = new ArrayList();
	ArrayList *usersIniList = new ArrayList();

	if (MOG_ControllerProject::GetProject())
	{
		usersList = MOG_ControllerProject::GetProject()->GetUsers();
	}

	// Populate usersIniList
	// If we have a Users section
	if( mPrivilegesIni->SectionExist( mUserSection ) )
	{
		// Count of user's section
		int usersCount = mPrivilegesIni->CountKeys( mUserSection );

		// If we have any users...
		if( usersCount > 0 )
		{
			// For each user we know about in the ini...
			for( int j = 0; j < usersCount; ++j )
			{
				// Add the string of the user's name to usersIniList
				String *userFromIni = mPrivilegesIni->GetKeyNameByIndexSLOW( mUserSection, j );
				usersIniList->Add( userFromIni );
			}
		}
	}

	// If we have anything in the ini...
	if( usersIniList->Count > 0 )
	{
		// Foreach user in the project
		for( int i = 0; i < usersList->Count; ++i )
		{
			// Initialize userListName
			String *userListName = (__try_cast<MOG_User*>( usersList->get_Item(i) ))->GetUserName();

			// If this user does not have an entry...
			if( !usersIniList->Contains( userListName )  )
			{
				// Add user to list of users that need to be added
				usersToAdd->Add( userListName );
			}
		} // end for on users
	}
	// Else, we have nothing in the ini, so add all the uses in usersList
	else
	{
		for( int i = 0; i < usersList->Count; ++i )
		{
			usersToAdd->Add( (__try_cast<MOG_User*>( usersList->get_Item(i) ))->GetUserName() );
		}
	}

	// If we found users that need to be added to the INI...
	if( usersToAdd->Count > 0 )
	{
		// Open the file so we have exclusive rights
		MOG_Ini *temp = new MOG_Ini();
		if (temp->Open(mPrivilegesIni->GetFilename(), FileShare::Write))
		{
			// Foreach String* in usersToAdd...
			for( int i = 0; i < usersToAdd->Count; ++i)
			{
				String *userName = __try_cast<String*>( usersToAdd->get_Item(i) );
				// If we are Admin, indicate such
				if( String::Compare( userName, mAdminSection, true ) ==  0 )
				{
					temp->PutString( mUserSection, userName, mAdminSection );
				}
				// Else, we are average-joe user
				else
				{
					temp->PutString( mUserSection, userName, mUserDefaultSection );
				}
			}

			// Release our lock by closing the file
			temp->Close();
		}

		// Reload our ram file
		mPrivilegesIni->Load();
	} // end if on adding users
}

// Goes through each privilege and adds it into the Ini under *section
void MOG_Privileges::PopulateGroupPrivileges( MOG_Ini *file, String *groupSectionName )
{
	int privilegeCount = (int)MOG_PRIVILEGE::Count;
	// Foreach privilege...
	for( int i = 0; i < privilegeCount; ++i )
	{
		// Get a string for the privilege we will be filling in
		String *privilegeStr = (__box( ( MOG_PRIVILEGE)i ) )->ToString();
		// If our String* is S"None", write false...
		if( String::Compare( privilegeStr, (__box( MOG_PRIVILEGE::None ) )->ToString(), true ) == 0 )
		{
            file->PutBool( groupSectionName, privilegeStr, false );
		}
		// Write the privilege as true, unless we are a group OTHER than Admin or Lead
		else
		{
			// If this is NOT our Lead or Admin group
			if( String::Compare( groupSectionName, mAdminSection, true ) != 0 
				&& String::Compare( groupSectionName, mLeadSection, true ) != 0 )
			{
				// Set our privileges by switch
				switch( ((MOG_PRIVILEGE)i) )
				{
					// Privileges below this line are set to FALSE
					case MOG_PRIVILEGE::PerformAllInboxOpps:
					case MOG_PRIVILEGE::AddClassification:
					case MOG_PRIVILEGE::DeleteClassification:
					case MOG_PRIVILEGE::BlessAssetFromWithinOtherInbox:
						// Only for privileges above this line are we defaulting to false
						file->PutBool( groupSectionName, privilegeStr, false );
						break;
					default:
						file->PutBool( groupSectionName, privilegeStr, true );
				}
			}
// TODO: glk: Add Lead group section with some restrictions...
			// Else, we have our Admin or Lead, section, so give them TRUE all
			else
			{
				file->PutBool( groupSectionName, privilegeStr, true );
			}
		}
	}
}

// Add a user, given the section to use as the group
bool MOG_Privileges::AddUser( String *userName, String *groupSectionName )
{
	// Open the file so we have exclusive rights
	MOG_Ini *temp = new MOG_Ini();
	if (temp->Open(mPrivilegesIni->GetFilename(), FileShare::Write))
	{
		// If we have no groupName or it's length is zero, assume DefaultUser
		if( !groupSectionName || !groupSectionName->Length )
		{
			groupSectionName = this->mUserDefaultSection;
		}

		temp->PutString( this->mUserSection, userName, groupSectionName );

		// Release our lock by closing the file
		temp->Close();
	}

	// Reload our ram file
	mPrivilegesIni->Load();
	return true;
}

// Add a user, given a MainSection
bool MOG_Privileges::AddUser( String *userName, MOG_Privileges::MainSections mainGroupSection )
{
	return AddUser( userName, (__box(mainGroupSection))->ToString() );
}

// Remove a user
bool MOG_Privileges::RemoveUser( String *userName )
{
	// Open the file so we have exclusive rights
	MOG_Ini *temp = new MOG_Ini();
	if (temp->Open(mPrivilegesIni->GetFilename(), FileShare::Write))
	{
		if( temp->SectionExist( this->mUserSection ) &&
			temp->KeyExist( mUserSection, userName ) )
		{
			temp->RemoveString( mUserSection, userName );
		}

		// Release our lock by closing the file
		temp->Close();
	}

	// Reload our ram file
	mPrivilegesIni->Load();
	return true;
}

bool MOG_Privileges::AddGroup( String *groupName )
{
	// If group already exists, or we are missing our group section,
	//  return false.
	if( mPrivilegesIni->SectionExist( groupName ) ||
		!mPrivilegesIni->SectionExist( mGroupSection ))
	{
		return false;
	}

	// Open the file so we have exclusive rights
	MOG_Ini *temp = new MOG_Ini();
	if (temp->Open(mPrivilegesIni->GetFilename(), FileShare::Write))
	{
		// Add our group to our Groups section
		temp->PutString( mGroupSection, groupName, S"" );
		// Add a section for our group
		temp->PutSection( groupName );
		PopulateGroupPrivileges( temp, groupName );

		// Release our lock by closing the file
		temp->Close();
	}

	// Reload our ram file
	mPrivilegesIni->Load();
	return true;
}

bool MOG_Privileges::RemoveGroup( String *groupName )
{
	// If group exists, remove it, otherwise, return false
	if( this->mPrivilegesIni->SectionExist( groupName ) )
	{
		// If this is one of our mainsection groups, don't remove it
		if( String::Compare( groupName, mUserDefaultSection, true ) == 0
			|| String::Compare( groupName, mLeadSection, true ) == 0
			|| String::Compare( groupName, mAdminSection, true ) == 0 )
		{
			return false;
		}

		// Open the file so we have exclusive rights
		MOG_Ini *temp = new MOG_Ini();
		if (temp->Open(mPrivilegesIni->GetFilename(), FileShare::Write))
		{
			// If we have any users with this groupName, change them to defaultUser
			int count = temp->CountKeys( mUserSection );
			if( count > 0 )
			{
				// Foreach user, add that user to our defaultUser group
				for( int i = 0; i < count; ++i )
				{
					// Get our user's name
					String *userKey = temp->GetKeyNameByIndexSLOW( mUserSection, i );
					// Get the groupName of our user
					String *userGroup = temp->GetKeyByIndexSLOW( mUserSection, i );
					// If we have found a user with this groupName as his/her group, change...
					if( String::Compare( userGroup, groupName, true ) == 0 )
					{
						temp->PutString( mUserSection, userKey, mUserDefaultSection );
					}
				}
			}

			// Go ahead and remove our section
			temp->RemoveSection( groupName );
			// Also, remove it from our Group section
			if( temp->KeyExist( mGroupSection, groupName ) )
			{
				temp->RemoveString( mGroupSection, groupName );
			}

			// Release our lock by closing the file
			temp->Close();
		}

		// Reload our ram file
		mPrivilegesIni->Load();
		return true;
	}

	// Else, we should return false
	return false;
}

bool MOG_Privileges::GroupHasUsers( String *groupName )
{
	// If we have this section...
	if( this->mPrivilegesIni->SectionExist( groupName ) )
	{
		int count = this->mPrivilegesIni->CountKeys( mUserSection );
		if( count > 0 )
		{
			// Foreach user, see if this groupName is assigned
			for( int i = 0; i < count; ++i )
			{
				String *userGroup = this->mPrivilegesIni->GetKeyByIndexSLOW( mUserSection, i );

				// If we've found a user attached to our groupName, return true...
				if( String::Compare( userGroup, groupName, true ) == 0 )
				{
					return true;
				}
			}
		}
	}

	// By default, return false
	return false; 
}
bool MOG_Privileges::ValidateUser( String *userName )
{
	if( !this->mPrivilegesIni->KeyExist( mUserSection, userName ) )
	{
		this->AddUser( userName, "" );
	}
	return true;
}

String *MOG_Privileges::GetUserGroup( String *userName )
{
	String *retString = S"";
	if( this->mPrivilegesIni->KeyExist( mUserSection, userName ) )
	{
		retString = mPrivilegesIni->GetString( mUserSection, userName );
	}
	return retString;
}

bool MOG_Privileges::SetUserGroup( String *userName, String *groupName )
{
	this->RemoveUser( userName );
	this->AddUser( userName, groupName );
	return true;
}


// Returns blank ArrayList if privileges for user not listed
//	or user name not found
bool MOG_Privileges::GetUserPrivilege( String *userName, MOG_PRIVILEGE privilege )
{
	return GetUserOrGroupPrivilege( userName, privilege );
}

bool MOG_Privileges::UserHasCustomPrivileges( String *userName )
{
	return mPrivilegesIni->SectionExist(userName);
}

void MOG_Privileges::UserRemoveCustomPrivileges( String *userName )
{
	mPrivilegesIni->RemoveSection(userName);
	mPrivilegesIni->Save();
}


bool MOG_Privileges::GetUserOrGroupPrivilege( String *userOrGroupName, MOG_PRIVILEGE privilege )
{
	String *groupName = S"";
	String *privilegeStr = (__box(privilege))->ToString();

	// Return true for all (i.e. we want all users to have this right, no matter what)
	if( privilege == MOG_PRIVILEGE::All )
		return true;
	// Return false if we want no user to be able to so something
	if( privilege == MOG_PRIVILEGE::None )
		return false;

	// Check that [Users] exists with group name inside it...
	if( mPrivilegesIni->KeyExist( mUserSection, userOrGroupName ) )
	{
		groupName = mPrivilegesIni->GetString( mUserSection, userOrGroupName );
	}
	// glk: Else we might want to put something for a default privilege group here...

	// If we have this privilege under the user section (an override)...
	if( mPrivilegesIni->KeyExist( userOrGroupName, privilegeStr ) )
	{
		return mPrivilegesIni->GetBool( userOrGroupName, privilegeStr );
	}
	// Else, if we only have this privilege under the group section...
	else if( mPrivilegesIni->KeyExist( groupName, privilegeStr) )
	{
		return mPrivilegesIni->GetBool( groupName, privilegeStr );
	}
	// Else, we have an error, return false.
	else
		return false;
}

bool MOG_Privileges::SetUserOrGroupPrivilege( String *userOrGroupName, MOG_PRIVILEGE privilege, bool value )
{
	// If we have a user, return the value of SetUserPrivilege...
	if( this->UsersList->Contains( userOrGroupName ) )
	{
		return SetUserPrivilege( userOrGroupName, privilege, value );
	}
	// Else, we are going to set the group privilege, overriding the user-specific privilege
	else
	{
		return SetGroupPrivilege( userOrGroupName, privilege, value, true );
	}
}

// Returns true if we were able to set the privilege.
bool MOG_Privileges::SetUserPrivilege( String *userName, MOG_PRIVILEGE privilege, bool value )
{
	// If this is the Admin, do not allow any change
	if( String::Compare( userName, mAdminSection, true ) == 0 && !value )
	{
		return false;
	}

	String *privilegeStr = (__box(privilege))->ToString();

	// Open the file so we have exclusive rights
	MOG_Ini *temp = new MOG_Ini();
	if (temp->Open(mPrivilegesIni->GetFilename(), FileShare::Write))
	{
		// If the section doesn't exist, this adds the section, the key, and the value
		//  otherwise, it simply adds the key and value.
		temp->PutBool( userName, privilegeStr, value );

		// Release our lock by closing the file
		temp->Close();
	}

	// Reload our ram file
	mPrivilegesIni->Load();
	return true;
}


// Return value of group privilege.  False is default return value.
bool MOG_Privileges::GetGroupPrivilege( String *groupName, MOG_PRIVILEGE privilege )
{
	// Return true for all (i.e. we want all users to have this right, no matter what)
	if( privilege == MOG_PRIVILEGE::All )
		return true;
	// Return false if we want no user to be able to so something
	if( privilege == MOG_PRIVILEGE::None )
		return false;

	String *privilegeStr = (__box(privilege))->ToString();
	if( mPrivilegesIni->KeyExist( groupName, privilegeStr ) )
	{
		return mPrivilegesIni->GetBool( groupName, privilegeStr );
	}
	return false;
}

// Return a SortedList of all the privileges for a group.
// Returns empty set if group not found
SortedList *MOG_Privileges::GetGroupPrivileges( String *groupName )
{
	SortedList *privileges = new SortedList();

	if( this->mPrivilegesIni->SectionExist( groupName ) )
	{
		int count = mPrivilegesIni->CountKeys( groupName );
		for( int i = 0; i < count; ++i )
		{
			String *privilege = mPrivilegesIni->GetKeyNameByIndexSLOW( groupName, i );
			String *value = mPrivilegesIni->GetKeyByIndexSLOW( groupName, i );
			privileges->Add( privilege, value );
		}
	}

	return privileges;
}

bool MOG_Privileges::SetGroupPrivilege( String *groupName, MOG_PRIVILEGE privilege, bool value)
{
	return SetGroupPrivilege( groupName, privilege, value, false );
}

// Set value of privilege for group.
bool MOG_Privileges::SetGroupPrivilege( String *groupName, MOG_PRIVILEGE privilege, bool value, bool overrideUsersPrivilege )
{
	// If this is any section other than our Admin Section, we will allow the change
	if( String::Compare( groupName, mAdminSection, true ) != 0 )
	{
		String *privilegeStr = (__box(privilege))->ToString();
		String *userSection = (__box(MOG_Privileges::MainSections::Users))->ToString();

		// Open the file so we have exclusive rights
		MOG_Ini *temp = new MOG_Ini();
		if (temp->Open(mPrivilegesIni->GetFilename(), FileShare::Write))
		{
			// If we need to overwrite user values and we have a valid [Users] section...
			if( overrideUsersPrivilege && temp->SectionExist( userSection ) )
			{
				ArrayList *userList = new ArrayList();

				int usersCount = temp->CountKeys( userSection );
				// Foreach user in [Users]
				for( int i = 0; i < usersCount; ++i )
				{
					// Get groupname from current [Users] key
					String *currentGroupName = temp->GetKeyByIndexSLOW( userSection, i );

					// If currentGroupName is same as groupName, ignoring case...
					if( String::Compare( groupName, currentGroupName, true ) == 0 )
					{
						// Add user to userList
						userList->Add( temp->GetKeyNameByIndexSLOW( userSection, i ) );
					}
				}

				// Foreach user section found with given groupName
				for( int i = 0; i < userList->Count; ++i)
				{
					// Get name from userList
					String *currentUserName = __try_cast<String*>( userList->get_Item(i) );
					// If this user has an override of that privilege, delete the override
					if( temp->KeyExist( currentUserName, privilegeStr ) )
					{
						// Clean up any clutter in the ini file
						temp->RemoveString( currentUserName, privilegeStr );
					}
				}
			}

			// Set privilege to value
			temp->PutBool( groupName, privilegeStr, value );

			// Release our lock by closing the file
			temp->Close();
		}

		// Reload our ram file
		mPrivilegesIni->Load();
		return true;
	}

	return false;
}