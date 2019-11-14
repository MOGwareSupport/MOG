using System;
using System.ComponentModel;
using MOG;

namespace MOG_ControlsLibrary.Common.MogControl_PriviledgesChangeForm
{
	/// <summary>
	/// Summary description for Mog_PriviledgesWrapper.
	/// </summary>
	[Category( "MOG-related priviledges" ), Description( "Customize MOG priviledges." )]
	public class Mog_PriviledgesWrapper
	{
		private string mCurrentGroupName;
		private MOG_Priviledges mPriviledges;

		[Category( "MOG ToolBox" ), Description("Allow user to to create Custom Controls in "
		+"MOG's Workspace Toolbox for the entire Project (only recommended for Admin group).")]
		public bool AddCustomControlToProject
		{
			get
			{
				if( mCurrentGroupName != null )
				{
					return mPriviledges.GetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.ConfigureProjectCustomTools );
				}
				return true;
			}
			set
			{
				if( mCurrentGroupName != null )
				{
					mPriviledges.SetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.ConfigureProjectCustomTools,
						value );
				}
			}
		}
		
		[Category( "MOG ToolBox" ), Description("Allow user to create Custom Controls in "
		+"MOG's Workspace Toolbox for the entire department.")]
		public bool AddCustomControlToDepartment
		{
			get
			{
				if( mCurrentGroupName != null )
				{
					return mPriviledges.GetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.ConfigureDepartmentCustomTools );
				}
				return true;
			}
			set
			{
				if( mCurrentGroupName != null )
				{
					mPriviledges.SetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.ConfigureDepartmentCustomTools,
						value );
				}
			}
		}

		[Category( "Inbox" ), Description("Allow user to bless Asset from own Inbox.")]
		public bool BlessAssetFromWithinOwnInbox
		{
			get
			{
				if( mCurrentGroupName != null )
				{
					return mPriviledges.GetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.BlessAssetFromWithinOwnInbox );
				}
				return true;
			}
			set
			{
				if( mCurrentGroupName != null )
				{
					mPriviledges.SetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.BlessAssetFromWithinOwnInbox,
						value );
				}
			}
		}

		[Category( "Inbox" ), Description("Allow user to change properties of an Asset.")]
		public bool ChangeAssetProperties
		{
			get
			{
				if( mCurrentGroupName != null )
				{
					return mPriviledges.GetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.ChangeAssetProperties );
				}
				return true;
			}
			set
			{
				if( mCurrentGroupName != null )
				{
					mPriviledges.SetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.ChangeAssetProperties,
						value );
				}
			}
		}

		[Category( "Inbox" ), Description("Allow user to explore Asset directory.")]
		public bool ExploreAssetDirectory 
		{
			get
			{
				if( mCurrentGroupName != null )
				{
					return mPriviledges.GetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.ExploreAssetDirectory );
				}
				return true;
			}
			set
			{
				if( mCurrentGroupName != null )
				{
					mPriviledges.SetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.ExploreAssetDirectory,
						value );
				}
			}
		}
	
		[Category( "Inbox" ), Description("Allow user to view other user's Inbox.")]
		public bool ViewOtherUsersInbox 
		{
			get
			{
				if( mCurrentGroupName != null )
				{
					return mPriviledges.GetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.ViewOtherUsersInbox );
				}
				return true;
			}
			set
			{
				if( mCurrentGroupName != null )
				{
					mPriviledges.SetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.ViewOtherUsersInbox,
						value );
				}
			}
		}

		[Category( "Inbox" ), Description("Allow user to rename local Assets.")]
		public bool RenameLocal 
		{
			get
			{
				if( mCurrentGroupName != null )
				{
					return mPriviledges.GetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.RenameLocal );
				}
				return true;
			}
			set
			{
				if( mCurrentGroupName != null )
				{
					mPriviledges.SetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.RenameLocal,
						value );
				}
			}
		}

		[Category( "Inbox" ), Description("Allow user to perform all Inbox operations.")]
		public bool PerformAllInboxOpps 
		{
			get
			{
				if( mCurrentGroupName != null )
				{
					return mPriviledges.GetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.PerformAllInboxOpps );
				}
				return true;
			}
			set
			{
				if( mCurrentGroupName != null )
				{
					mPriviledges.SetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.PerformAllInboxOpps,
						value );
				}
			}
		}

		[Category( "Inbox" ), Description("Allow user to bless an Asset from another user's inbox.")]
		public bool BlessAssetFromWithinOtherInbox 
		{
			get
			{
				if( mCurrentGroupName != null )
				{
					return mPriviledges.GetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.BlessAssetFromWithinOtherInbox );
				}
				return true;
			}
			set
			{
				if( mCurrentGroupName != null )
				{
					mPriviledges.SetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.BlessAssetFromWithinOtherInbox,
						value );
				}
			}
		}

		[Category( "Custom Tools" ), Description("Allow user to configure MOG Custom Tools (lower-right corner of Workspace tab).")]
		public bool ConfigureUserCustomTools 
		{
			get
			{
				if( mCurrentGroupName != null )
				{
					return mPriviledges.GetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.ConfigureUserCustomTools );
				}
				return true;
			}
			set
			{
				if( mCurrentGroupName != null )
				{
					mPriviledges.SetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.ConfigureUserCustomTools,
						value );
				}
			}
		}

		[Category( "Custom Tools" ), Description("Allow user to configure MOG Custom Tools for a given department (e.g. 'Animators').")]
		public bool ConfigureDepartmentCustomTools 
		{
			get
			{
				if( mCurrentGroupName != null )
				{
					return mPriviledges.GetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.ConfigureDepartmentCustomTools );
				}
				return true;
			}
			set
			{
				if( mCurrentGroupName != null )
				{
					mPriviledges.SetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.ConfigureDepartmentCustomTools,
						value );
				}
			}
		}

		[Category( "Custom Tools" ), Description("Allow user to configure MOG Custom Tools for the entire project.")]
		public bool ConfigureProjectCustomTools 
		{
			get
			{
				if( mCurrentGroupName != null )
				{
					return mPriviledges.GetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.ConfigureProjectCustomTools );
				}
				return true;
			}
			set
			{
				if( mCurrentGroupName != null )
				{
					mPriviledges.SetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.ConfigureProjectCustomTools,
						value );
				}
			}
		}
		
		[Category( "Tasks" ), Description("Allow user to void his/her own tasks from a project.")]
		public bool VoidTaskFromProject 
		{
			get
			{
				if( mCurrentGroupName != null )
				{
					return mPriviledges.GetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.VoidTaskFromProject );
				}
				return true;
			}
			set
			{
				if( mCurrentGroupName != null )
				{
					mPriviledges.SetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.VoidTaskFromProject,
						value );
				}
			}
		}
        
		[Category( "Tasks" ), Description("Allow user to delete his/her own task(s) from a project.")]
		public bool DeleteTaskFromProject 
		{
			get
			{
				if( mCurrentGroupName != null )
				{
					return mPriviledges.GetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.DeleteTaskFromProject );
				}
				return true;
			}
			set
			{
				if( mCurrentGroupName != null )
				{
					mPriviledges.SetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.DeleteTaskFromProject,
						value );
				}
			}
		}

		[Category( "Tasks" ), Description("Allow user to delete other user's tasks.")]
		public bool DeleteOtherUsersTask 
		{
			get
			{
				if( mCurrentGroupName != null )
				{
					return mPriviledges.GetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.DeleteOtherUsersTask );
				}
				return true;
			}
			set
			{
				if( mCurrentGroupName != null )
				{
					mPriviledges.SetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.DeleteOtherUsersTask,
						value );
				}
			}
		}

		[Category( "Project" ), Description("Allow user to add another user.")]
		public bool AddNewUser 
		{
			get
			{
				if( mCurrentGroupName != null )
				{
					return mPriviledges.GetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.AddNewUser );
				}
				return true;
			}
			set
			{
				if( mCurrentGroupName != null )
				{
					mPriviledges.SetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.AddNewUser,
						value );
				}
			}
		}

		[Category( "Project" ), Description("Allow user to add a new Asset type.")]
		public bool AddAssetType 
		{
			get
			{
				if( mCurrentGroupName != null )
				{
					return mPriviledges.GetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.AddAssetType );
				}
				return true;
			}
			set
			{
				if( mCurrentGroupName != null )
				{
					mPriviledges.SetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.AddAssetType,
						value );
				}
			}
		}

		[Category( "Project" ), Description("Allow user to create a new Package.")]
		public bool CreatePackage 
		{
			get
			{
				if( mCurrentGroupName != null )
				{
					return mPriviledges.GetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.CreatePackage );
				}
				return true;
			}
			set
			{
				if( mCurrentGroupName != null )
				{
					mPriviledges.SetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.CreatePackage,
						value );
				}
			}
		}

		[Category( "Project" ), Description("Allow user to add a new Package group.")]
		public bool AddPackageGroup 
		{
			get
			{
				if( mCurrentGroupName != null )
				{
					return mPriviledges.GetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.AddPackageGroup );
				}
				return true;
			}
			set
			{
				if( mCurrentGroupName != null )
				{
					mPriviledges.SetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.AddPackageGroup,
						value );
				}
			}
		}

		[Category( "Project" ), Description("Allow user to add a new Classification.")]
		public bool AddClassification 
		{
			get
			{
				if( mCurrentGroupName != null )
				{
					return mPriviledges.GetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.AddClassification );
				}
				return true;
			}
			set
			{
				if( mCurrentGroupName != null )
				{
					mPriviledges.SetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.AddClassification,
						value );
				}
			}
		}

		[Category( "Project" ), Description("Allow user to delete an existing Classification (which will place it in Workspace's Archival Tree).")]
		public bool DeleteClassification 
		{
			get
			{
				if( mCurrentGroupName != null )
				{
					return mPriviledges.GetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.DeleteClassification );
				}
				return true;
			}
			set
			{
				if( mCurrentGroupName != null )
				{
					mPriviledges.SetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.DeleteClassification,
						value );
				}
			}
		}

		[Category( "Project" ), Description("Allow user to rebuild a package.")]
		public bool RebuildPackage 
		{
			get
			{
				if( mCurrentGroupName != null )
				{
					return mPriviledges.GetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.RebuildPackage );
				}
				return true;
			}
			set
			{
				if( mCurrentGroupName != null )
				{
					mPriviledges.SetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.RebuildPackage,
						value );
				}
			}
		}

		[Category( "Project" ), Description("Allow user to use Admin tools.")]
		public bool AccessAdminTools 
		{
			get
			{
				if( mCurrentGroupName != null )
				{
					return mPriviledges.GetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.AccessAdminTools );
				}
				return true;
			}
			set
			{
				if( mCurrentGroupName != null )
				{
					mPriviledges.SetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.AccessAdminTools,
						value );
				}
			}
		}

		[Category( "Project" ), Description("Allow user to change blessed revision of an Asset.")]
		public bool ChangeRevisionOfAsset 
		{
			get
			{
				if( mCurrentGroupName != null )
				{
					return mPriviledges.GetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.ChangeRevisionOfAsset );
				}
				return true;
			}
			set
			{
				if( mCurrentGroupName != null )
				{
					mPriviledges.SetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.ChangeRevisionOfAsset,
						value );
				}
			}
		}

		[Category( "Project" ), Description("Allow user to remove an Asset from a project.")]
		public bool RemoveAssetFromProject 
		{
			get
			{
				if( mCurrentGroupName != null )
				{
					return mPriviledges.GetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.RemoveAssetFromProject );
				}
				return true;
			}
			set
			{
				if( mCurrentGroupName != null )
				{
					mPriviledges.SetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.RemoveAssetFromProject,
						value );
				}
			}
		}

		[Category( "Project" ), Description("Allow user to create a new Branch for a project.")]
		public bool CreateBranch 
		{
			get
			{
				if( mCurrentGroupName != null )
				{
					return mPriviledges.GetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.CreateBranch );
				}
				return true;
			}
			set
			{
				if( mCurrentGroupName != null )
				{
					mPriviledges.SetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.CreateBranch,
						value );
				}
			}
		}

		[Category( "Project" ), Description("Allow user to request a new Build.")]
		public bool RequestBuild 
		{
			get
			{
				if( mCurrentGroupName != null )
				{
					return mPriviledges.GetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.RequestBuild );
				}
				return true;
			}
			set
			{
				if( mCurrentGroupName != null )
				{
					mPriviledges.SetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.RequestBuild,
						value );
				}
			}
		}

		[Category( "Admin" ), Description("Allow user to kill a lock on an Asset.")]
		public bool KillLock 
		{
			get
			{
				if( mCurrentGroupName != null )
				{
					return mPriviledges.GetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.KillLock );
				}
				return true;
			}
			set
			{
				if( mCurrentGroupName != null )
				{
					mPriviledges.SetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.KillLock,
						value );
				}
			}
		}

		[Category( "Admin" ), Description("Allow user to launch a Slave on a remote computer.")]
		public bool LaunchSlaveOnRemoteComputer 
		{
			get
			{
				if( mCurrentGroupName != null )
				{
					return mPriviledges.GetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.LaunchSlaveOnRemoteComputer );
				}
				return true;
			}
			set
			{
				if( mCurrentGroupName != null )
				{
					mPriviledges.SetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.LaunchSlaveOnRemoteComputer,
						value );
				}
			}
		}

		[Category( "Admin" ), Description("Allow user to kill a connection to MOG.")]
		public bool KillConnection 
		{
			get
			{
				if( mCurrentGroupName != null )
				{
					return mPriviledges.GetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.KillConnection );
				}
				return true;
			}
			set
			{
				if( mCurrentGroupName != null )
				{
					mPriviledges.SetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.KillConnection,
						value );
				}
			}
		}

		[Category( "Admin" ), Description("Allow user to retask a MOG Command.")]
		public bool RetaskCommand 
		{
			get
			{
				if( mCurrentGroupName != null )
				{
					return mPriviledges.GetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.RetaskCommand );
				}
				return true;
			}
			set
			{
				if( mCurrentGroupName != null )
				{
					mPriviledges.SetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.RetaskCommand,
						value );
				}
			}
		}

		[Category( "Admin" ), Description("Allow user to configure Project-wide settings.")]
		public bool ConfigureProjectSettings 
		{
			get
			{
				if( mCurrentGroupName != null )
				{
					return mPriviledges.GetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.ConfigureProjectSettings );
				}
				return true;
			}
			set
			{
				if( mCurrentGroupName != null )
				{
					mPriviledges.SetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.ConfigureProjectSettings,
						value );
				}
			}
		}

		[Category( "Admin" ), Description("Allow user to order trash collection.")]
		public bool TrashCollection 
		{
			get
			{
				if( mCurrentGroupName != null )
				{
					return mPriviledges.GetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.TrashCollection );
				}
				return true;
			}
			set
			{
				if( mCurrentGroupName != null )
				{
					mPriviledges.SetUserOrGroupPriviledge( mCurrentGroupName, 
						MOG_Priviledges.Priviledges.TrashCollection,
						value );
				}
			}
		}

		public Mog_PriviledgesWrapper( MOG_Priviledges priviledges )
		{
			this.mPriviledges = priviledges;
			this.mCurrentGroupName = null;
		}

		/// <summary>
		/// Assigns the groupName to a user or group's name
		/// </summary>
		/// <param name="groupOrUserName"></param>
		public void InitializeWrapperToGroup( string groupOrUserName )
		{
			this.mCurrentGroupName = groupOrUserName;
		}
	}
}
