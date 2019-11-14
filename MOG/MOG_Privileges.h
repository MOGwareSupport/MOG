//--------------------------------------------------------------------------------
//	MOG_Privileges.h
//	
//	Container for all __value enums to be used in C# for privileges
//	Wrapper for privileges MOG_Ini with accessor functions
//	Container for all properties having to do with MOG_Privileges
//--------------------------------------------------------------------------------
#ifndef __MOG_PRIVILEGES_H__
#define __MOG_PRIVILEGES_H__

#include "stdafx.h"
#include "MOG_Ini.h"
#include "MOG_Main.h"
#include "MOG_Project.h"


using namespace System;
using namespace System::ComponentModel;

using namespace MOG;
using namespace MOG::INI;
using namespace MOG::PROJECT;



namespace MOG
{
public __gc class BASE;
}

namespace MOG
{
		// Privileges that are possible to assign
		public __value enum MOG_PRIVILEGE
		{
			// Inbox related:
			BlessAssetFromWithinOwnInbox,
			BlessOutofdateAssets,
			ChangeAssetProperties,
			ExploreAssetDirectory,
			ViewOtherUsersInbox,
			RenameLocal,
			PerformAllInboxOpps,
			RenameBlessedAsset,
			// Inbox related to other users:
			BlessAssetFromWithinOtherInbox,

			// Local Data (Toolbox) related:
			ConfigureUserCustomTools,
			ConfigureDepartmentCustomTools,
			ConfigureProjectCustomTools,
			ConfigureUpdateFilterPromotions,

			// Tasks related:
			VoidTaskFromProject,
			DeleteTaskFromProject,
			DeleteOtherUsersTask,
			EditOtherUsersTask,

			// Project related:
			AddNewUser,
			CreatePropertyMenu,
			CreatePackage,
			AddPackageGroup,
			PackageManagement,
			AddClassification,
			DeleteClassification,
			RebuildPackage,
			AccessAdminTools,
			ChangeRevisionOfAsset,
			RemoveAssetFromProject,
			CreateBranch,
			RequestBuild,
			IgnoreSyncAsReadOnly,

			// MOG Admin related:
			KillLock,
			LaunchSlaveOnRemoteComputer,
			KillConnection,
			RetaskCommand,
			ConfigureProjectSettings,
			TrashCollection,

			// Always true:
			All,

			// Always false:
			None,

			// Add new values above this line
			Count,
		};

	public __gc class MOG_Privileges
	{
	private: 
		MOG_Ini *mPrivilegesIni;
		String *mUserSection;
		String *mGroupSection;
		String *mUserDefaultSection;
		String *mAdminSection;
		String *mLeadSection;
		String *mItem;

		// Validation/Creation of MOG_Privileges INI file.
		void ValidatePrivilegesIni( void );
		void PopulateUsersSection( void );
		void PopulateMainSections( void );
		void PopulateGroupPrivileges( MOG_Ini *file, String *section );

		// Get list of groups in mPrivilegesIni
		ArrayList *GetGroups( void );
		// Get list of users in mPrivilegesIni
		SortedList *GetUsers( void );
		

	public:
		// Note: `__value` enables other .NET languages to access this enum as 
		//	part of the System::Enum class.

		// MainSections that should ALWAYS BE IN A Privileges ini
        __value enum MainSections
		{
			// The only sections that MUST exist to have a valid MOG_Privileges MOG_Ini file.
			Users,
			Groups,
			Admin,
			Lead,
			DefaultUser,

			// Add new values above this line
			Count,
		};
		
		// Initialize MOG_Privileges
		MOG_Privileges( void );
		~MOG_Privileges( void );

		
		// Allow user to get Ini outside of the wrapper
		[Browsable(false),  CategoryAttribute( "File" )]
		__property MOG_Ini *get_PrivilegesIni()	{	return this->mPrivilegesIni;	}
			// TODO: AddUser(), RemoveUser()
		[Browsable(false), CategoryAttribute( "Lists" ), DescriptionAttribute( "Displays a list of users." )]
		__property SortedList *get_UsersList()		{	return GetUsers();	}
			// TODO: AddGroup(), RemoveGroup()
		[Browsable( false ), CategoryAttribute( "Lists" ), DescriptionAttribute( "Displays a list of our available groups.  \
			To add a Group, enter a Group name in the text box to the lower left, then click 'Add', while  \
			the 'Groups' tree node is selected.")]
		__property ArrayList *get_GroupsList()		{	return GetGroups();	}

        // Add a user to a group
		bool AddUser( String *userName, String *groupSectionName );
		// Add a user to a group by the section name
		bool AddUser( String *userName, MOG_Privileges::MainSections mainGroupSection );
		// Remove a user
		bool RemoveUser( String *userName );
		// Make sure that the user exists
		bool ValidateUser( String *userName );

		// Add a group
		bool AddGroup( String *groupName );
		// Remove a group
		bool RemoveGroup( String *groupName );
		bool GroupHasUsers( String *groupName );

		bool UserHasCustomPrivileges( String *userName );
		void UserRemoveCustomPrivileges( String *userName );

		String *GetUserGroup( String *userName );
		bool SetUserGroup( String *userName, String *groupName );

		// Return value of user's privilege.  False is default return value.
		bool GetUserPrivilege( String *userName, MOG_PRIVILEGE privilege );
		// Set value of user privilege.  Returns false if unsuccessful.
		bool SetUserPrivilege( String *userName, MOG_PRIVILEGE privilege, bool value );

		bool GetUserOrGroupPrivilege( String *userOrGroupName, MOG_PRIVILEGE privilege );
		bool SetUserOrGroupPrivilege( String *userOrGroupName, MOG_PRIVILEGE privilege, bool value );

		// Return value of group privilege.  False is default return value.
		bool GetGroupPrivilege( String *groupName, MOG_PRIVILEGE privilege );
		// Return a SortedList of group privileges
		SortedList *GetGroupPrivileges( String *groupName );
		// Set value of privilege for group.
		bool SetGroupPrivilege( String *groupName, MOG_PRIVILEGE privilege, 
			bool value, bool overrideUsersPrivilege);
		// Same as above, except assumes false for overrideUsersPrivilege
		bool SetGroupPrivilege( String *groupName, MOG_PRIVILEGE privilege, bool value);

	//*****************
	// Related to displaying MOG_Privileges object inside the PropertyGrid Windows Control
	//*****************
	private:
		String *mCurrentGroupName;

	public:
		void SetCurrentGroupForProperties( String *groupName )
		{
			mCurrentGroupName = groupName;
		}


		[Category( "MOG ToolBox" ), Description("Allow user to to create Custom Controls in \
			MOG's Workspace Toolbox for the entire Project (only recommended for Admin group).")]
		__property Boolean get_AddCustomControlToProject()
		{
			if( mCurrentGroupName )
			{
				return GetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::ConfigureProjectCustomTools );
			}
			return true;
		}
		__property void set_AddCustomControlToProject( Boolean value )
		{
			if( mCurrentGroupName )
			{
				SetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::ConfigureProjectCustomTools,
					value );
			}
		}


        [Category( "MOG ToolBox" ), Description("Allow user to create Custom Controls in \
			MOG's Workspace Toolbox for the entire department.")]
		__property Boolean get_AddCustomControlToDepartment()
		{
			if( mCurrentGroupName )
			{
				return GetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::ConfigureDepartmentCustomTools );
			}
			return true;
		}
		__property void set_AddCustomControlToDepartment( Boolean value )
		{
			if( mCurrentGroupName )
			{
				SetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::ConfigureDepartmentCustomTools,
					value );
			}
		}


		[Category( "Inbox" ), Description("Allow user to bless Asset from own Inbox.")]
		__property Boolean get_BlessAssetFromWithinOwnInbox()
		{
			if( mCurrentGroupName )
			{
				return GetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::BlessAssetFromWithinOwnInbox );
			}
			return true;
		}
		__property void set_BlessAssetFromWithinOwnInbox( Boolean value )
		{
			if( mCurrentGroupName )
			{
				SetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::BlessAssetFromWithinOwnInbox,
					value );
			}
		}

		[Category( "Inbox" ), Description("Allow user to bless out-of-date Assets without having to GetLatest.")]
		__property Boolean get_BlessOutofdateAssets()
		{
			if( mCurrentGroupName )
			{
				return GetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::BlessOutofdateAssets );
			}
			return true;
		}
		__property void set_BlessOutofdateAssets( Boolean value )
		{
			if( mCurrentGroupName )
			{
				SetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::BlessOutofdateAssets,
					value );
			}
		}

		[Category( "Inbox" ), Description("Allow user to change properties of an Asset.")]
		__property Boolean get_ChangeAssetProperties()
		{
			if( mCurrentGroupName )
			{
				return GetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::ChangeAssetProperties );
			}
			return true;
		}
		__property void set_ChangeAssetProperties( Boolean value )
		{
			if( mCurrentGroupName )
			{
				SetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::ChangeAssetProperties,
					value );
			}
		}

		[Category( "Inbox" ), Description("Allow user to explore Asset directory.")]
		__property Boolean get_ExploreAssetDirectory()
		{
			if( mCurrentGroupName )
			{
				return GetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::ExploreAssetDirectory );
			}
			return true;
		}
		__property void set_ExploreAssetDirectory( Boolean value )
		{
			if( mCurrentGroupName )
			{
				SetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::ExploreAssetDirectory,
					value );
			}
		}
	
		[Category( "Inbox" ), Description("Allow user to view other user's Inbox.")]
		__property Boolean get_ViewOtherUsersInbox()
		{
			if( mCurrentGroupName )
			{
				return GetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::ViewOtherUsersInbox );
			}
			return true;
		}
		__property void set_ViewOtherUsersInbox( Boolean value )
		{
			if( mCurrentGroupName )
			{
				SetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::ViewOtherUsersInbox,
					value );
			}
		}

		[Category( "Inbox" ), Description("Allow user to rename local Assets.")]
		__property Boolean get_RenameLocal()
		{
			if( mCurrentGroupName )
			{
				return GetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::RenameLocal );
			}
			return true;
		}
		__property void set_RenameLocal( Boolean value )
		{
			if( mCurrentGroupName )
			{
				SetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::RenameLocal,
					value );
			}
		}

		[Category( "Inbox" ), Description("Allow user to perform all Inbox operations.")]
		__property Boolean get_PerformAllInboxOpps()
		{
			if( mCurrentGroupName )
			{
				return GetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::PerformAllInboxOpps );
			}
			return true;
		}
		__property void set_PerformAllInboxOpps( Boolean value )
		{
			if( mCurrentGroupName )
			{
				SetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::PerformAllInboxOpps,
					value );
			}
		}

		[Category( "Project" ), Description( "Allow user to change the name of an asset that has been blessed into the project.")]
		__property Boolean get_RenameBlessedAsset()
		{
			if( mCurrentGroupName )
			{
				return GetUserOrGroupPrivilege( mCurrentGroupName, MOG_PRIVILEGE::RenameBlessedAsset );
			}
			return true;
		}
		__property void set_RenameBlessedAsset( Boolean value )
		{
			if( mCurrentGroupName )
			{
				SetUserOrGroupPrivilege( mCurrentGroupName, MOG_PRIVILEGE::RenameBlessedAsset, value );
			}
		}

		[Category( "Inbox" ), Description("Allow user to bless an Asset from another user's inbox.")]
		__property Boolean get_BlessAssetFromWithinOtherInbox() 
		{
			if( mCurrentGroupName )
			{
				return GetUserOrGroupPrivilege( mCurrentGroupName, MOG_PRIVILEGE::BlessAssetFromWithinOtherInbox );
			}
			return true;
		}
		__property void set_BlessAssetFromWithinOtherInbox( Boolean value )
		{
			if( mCurrentGroupName )
			{
				SetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::BlessAssetFromWithinOtherInbox,
					value );
			}
		}

		[Category( "Custom Tools" ), Description("Allow user to configure MOG Custom Tools (lower-right corner of Workspace tab).")]
		__property Boolean get_ConfigureUserCustomTools() 
		{
			if( mCurrentGroupName )
			{
				return GetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::ConfigureUserCustomTools );
			}
			return true;
		}
		__property void set_ConfigureUserCustomTools( Boolean value )
		{
			if( mCurrentGroupName )
			{
				SetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::ConfigureUserCustomTools,
					value );
			}
		}

		[Category( "Custom Tools" ), Description("Allow user to configure MOG Custom Tools for a given department (e.g. 'Animators').")]
		__property Boolean get_ConfigureDepartmentCustomTools() 
		{
			if( mCurrentGroupName )
			{
				return GetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::ConfigureDepartmentCustomTools );
			}
			return true;
		}
		__property void set_ConfigureDepartmentCustomTools( Boolean value )
		{
			if( mCurrentGroupName )
			{
				SetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::ConfigureDepartmentCustomTools,
					value );
			}
		}

		[Category( "Custom Tools" ), Description("Allow user to configure MOG Custom Tools for the entire project.")]
		__property Boolean get_ConfigureProjectCustomTools() 
		{
			if( mCurrentGroupName )
			{
				return GetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::ConfigureProjectCustomTools );
			}
			return true;
		}
		__property void set_ConfigureProjectCustomTools( Boolean value )
		{
			if( mCurrentGroupName )
			{
				SetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::ConfigureProjectCustomTools,
					value );
			}
		}

		[Category( "Project" ), Description("Allow user to promote or demote sync filters.")]
		__property Boolean get_ConfigureUpdateFilterPromotions() 
		{
			if( mCurrentGroupName )
			{
				return GetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::ConfigureUpdateFilterPromotions );
			}
			return true;
		}
		__property void set_ConfigureUpdateFilterPromotions( Boolean value )
		{
			if( mCurrentGroupName )
			{
				SetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::ConfigureUpdateFilterPromotions,
					value );
			}
		}
		
		[Category( "Tasks" ), Browsable(false), Description("Allow user to void his/her own tasks from a project.")]
		__property Boolean get_VoidTaskFromProject()
		{
			if( mCurrentGroupName )
			{
				return GetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::VoidTaskFromProject );
			}
			return true;
		}
		__property void set_VoidTaskFromProject( Boolean value )
		{
			if( mCurrentGroupName )
			{
				SetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::VoidTaskFromProject,
					value );
			}
		}
        
		[Category( "Tasks" ), Browsable(false), Description("Allow user to delete his/her own task(s) from a project.")]
		__property Boolean get_DeleteTaskFromProject()
		{
			if( mCurrentGroupName )
			{
				return GetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::DeleteTaskFromProject );
			}
			return true;
		}
		__property void set_DeleteTaskFromProject( Boolean value )
		{
			if( mCurrentGroupName )
			{
				SetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::DeleteTaskFromProject,
					value );
			}
		}

		[Category( "Tasks" ), Browsable(false), Description("Allow user to delete other user's tasks.")]
		__property Boolean get_DeleteOtherUsersTask() 
		{
			if( mCurrentGroupName )
			{
				return GetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::DeleteOtherUsersTask );
			}
			return true;
		}
		__property void set_DeleteOtherUsersTask( Boolean value )
		{
			if( mCurrentGroupName )
			{
				SetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::DeleteOtherUsersTask,
					value );
			}
		}

		[Category( "Project" ), Description("Allow user to add another user.")]
		__property Boolean get_AddNewUser() 
		{
			if( mCurrentGroupName )
			{
				return GetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::AddNewUser );
			}
			return true;
		}
		__property void set_AddNewUser( Boolean value )
		{
			if( mCurrentGroupName )
			{
				SetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::AddNewUser,
					value );
			}
		}

		[Category( "Project" ), Description("Allow user to access and add new property menu's.")]
		__property Boolean get_CreatePropertyMenu() 
		{
			if( mCurrentGroupName )
			{
				return GetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::CreatePropertyMenu );
			}
			return true;
		}
		__property void set_CreatePropertyMenu( Boolean value )
		{
			if( mCurrentGroupName )
			{
				SetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::CreatePropertyMenu,
					value );
			}
		}

		[Category( "Project" ), Description("Allow user to create a new Package.")]
		__property Boolean get_CreatePackage() 
		{
			if( mCurrentGroupName )
			{
				return GetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::CreatePackage );
			}
			return true;
		}
		__property void set_CreatePackage( Boolean value )
		{
			if( mCurrentGroupName )
			{
				SetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::CreatePackage,
					value );
			}
		}

		[Category( "Project" ), Description("Allow user to add a new Package group.")]
		__property Boolean get_AddPackageGroup() 
		{
			if( mCurrentGroupName )
			{
				return GetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::AddPackageGroup );
			}
			return true;
		}
		__property void set_AddPackageGroup( Boolean value )
		{
			if( mCurrentGroupName )
			{
				SetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::AddPackageGroup,
					value );
			}
		}

		[Category( "Project" ), Description("Allow user to access package management.")]
		__property Boolean get_PackageManagement()
		{
			if( mCurrentGroupName )
			{
				return GetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::PackageManagement );
			}
			return true;
		}
		__property void set_PackageManagement( Boolean value )
		{
			if( mCurrentGroupName )
			{
				SetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::PackageManagement,
					value );
			}
		}

		[Category( "Project" ), Description("Allow user to add a new Classification.")]
		__property Boolean get_AddClassification() 
		{
			if( mCurrentGroupName )
			{
				return GetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::AddClassification );
			}
			return true;
		}
		__property void set_AddClassification( Boolean value )
		{
			if( mCurrentGroupName )
			{
				SetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::AddClassification,
					value );
			}
		}

		[Category( "Project" ), Description("Allow user to delete an existing Classification (which will place it in Workspace's Archival Tree).")]
		__property Boolean get_DeleteClassification() 
		{
			if( mCurrentGroupName )
			{
				return GetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::DeleteClassification );
			}
			return true;
		}
		__property void set_DeleteClassification( Boolean value )
		{
			if( mCurrentGroupName )
			{
				SetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::DeleteClassification,
					value );
			}
		}

		[Category( "Project" ), Description("Allow user to rebuild a package.")]
		__property Boolean get_RebuildPackage() 
		{
			if( mCurrentGroupName )
			{
				return GetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::RebuildPackage );
			}
			return true;
		}
		__property void set_RebuildPackage( Boolean value )
		{
			if( mCurrentGroupName )
			{
				SetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::RebuildPackage,
					value );
			}
		}

		[Category( "Project" ), Description("Allow user to use Admin tools.")]
		__property Boolean get_AccessAdminTools() 
		{
			if( mCurrentGroupName )
			{
				return GetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::AccessAdminTools );
			}
			return true;
		}
		__property void set_AccessAdminTools( Boolean value )
		{
			if( mCurrentGroupName )
			{
				SetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::AccessAdminTools,
					value );
			}
		}

		[Category( "Project" ), Description("Allow user to change blessed revision of an Asset.")]
		__property Boolean get_ChangeRevisionOfAsset() 
		{
			if( mCurrentGroupName )
			{
				return GetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::ChangeRevisionOfAsset );
			}
			return true;
		}
		__property void set_ChangeRevisionOfAsset( Boolean value )
		{
			if( mCurrentGroupName )
			{
				SetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::ChangeRevisionOfAsset,
					value );
			}
		}

		[Category( "Project" ), Description("Allow user to remove an Asset from a project.")]
		__property Boolean get_RemoveAssetFromProject()
		{
			if( mCurrentGroupName )
			{
				return GetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::RemoveAssetFromProject );
			}
			return true;
		}
		__property void set_RemoveAssetFromProject( Boolean value )
		{
			if( mCurrentGroupName )
			{
				SetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::RemoveAssetFromProject,
					value );
			}
		}

		[Category( "Project" ), Description("Allow user to create a new Branch for a project.")]
		__property Boolean get_CreateBranch() 
		{
			if( mCurrentGroupName )
			{
				return GetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::CreateBranch );
			}
			return true;
		}
		__property void set_CreateBranch( Boolean value )
		{
			if( mCurrentGroupName )
			{
				SetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::CreateBranch,
					value );
			}
		}

		[Category( "Project" ), Description("Allow user to request a new Build.")]
		__property Boolean get_RequestBuild() 
		{
			if( mCurrentGroupName )
			{
				return GetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::RequestBuild );
			}
			return true;
		}
		__property void set_RequestBuild( Boolean value )
		{
			if( mCurrentGroupName )
			{
				SetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::RequestBuild,
					value );
			}
		}

		[Category( "Admin" ), Description("Allow user to kill a lock on an Asset.")]
		__property Boolean get_KillLock() 
		{
			if( mCurrentGroupName )
			{
				return GetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::KillLock );
			}
			return true;
		}
		__property void set_KillLock( Boolean value )
		{
			if( mCurrentGroupName )
			{
				SetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::KillLock,
					value );
			}
		}

		[Category( "Admin" ), Description("Allow user to launch a Slave on a remote computer.")]
		__property Boolean get_LaunchSlaveOnRemoteComputer()
		{
			if( mCurrentGroupName )
			{
				return GetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::LaunchSlaveOnRemoteComputer );
			}
			return true;
		}
		__property void set_LaunchSlaveOnRemoteComputer( Boolean value )
		{
			if( mCurrentGroupName )
			{
				SetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::LaunchSlaveOnRemoteComputer,
					value );
			}
		}

		[Category( "Admin" ), Description("Allow user to kill a connection to MOG.")]
		__property Boolean get_KillConnection() 
		{
			if( mCurrentGroupName )
			{
				return GetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::KillConnection );
			}
			return true;
		}
		__property void set_KillConnection( Boolean value )
		{
			if( mCurrentGroupName )
			{
				SetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::KillConnection,
					value );
			}
		}

		[Category( "Admin" ), Description("Allow user to retask a MOG Command.")]
		__property Boolean get_RetaskCommand()
		{
			if( mCurrentGroupName )
			{
				return GetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::RetaskCommand );
			}
			return true;
		}
		__property void set_RetaskCommand( Boolean value )
		{
			if( mCurrentGroupName )
			{
				SetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::RetaskCommand,
					value );
			}
		}

		[Category( "Admin" ), Description("Allow user to configure Project-wide settings.")]
		__property Boolean get_ConfigureProjectSettings() 
		{
			if( mCurrentGroupName )
			{
				return GetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::ConfigureProjectSettings );
			}
			return true;
		}
		__property void set_ConfigureProjectSettings( Boolean value )
		{
			if( mCurrentGroupName )
			{
				SetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::ConfigureProjectSettings,
					value );
			}
		}

		[Category( "Admin" ), Description("Allow user to order trash collection.")]
		__property Boolean get_TrashCollection() 
		{
			if( mCurrentGroupName )
			{
				return GetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::TrashCollection );
			}
			return true;
		}
		__property void set_TrashCollection( Boolean value )
		{
			if( mCurrentGroupName )
			{
				SetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::TrashCollection,
					value );
			}
		}

		[Category( "Project" ), Description("Ignores the 'SyncAsReadOnly' inheritable property so local workspace files are never synced ReadOnly.")]
		__property Boolean get_IgnoreSyncAsReadOnly() 
		{
			if( mCurrentGroupName )
			{
				return GetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::IgnoreSyncAsReadOnly );
			}
			return true;
		}
		__property void set_IgnoreSyncAsReadOnly( Boolean value )
		{
			if( mCurrentGroupName )
			{
				SetUserOrGroupPrivilege( mCurrentGroupName, 
					MOG_PRIVILEGE::IgnoreSyncAsReadOnly,
					value );
			}
		}
		//*******************  End of Properties
	};
} // end ns MOG

using namespace MOG;
#endif //__MOG_PRIVILEGES_H__