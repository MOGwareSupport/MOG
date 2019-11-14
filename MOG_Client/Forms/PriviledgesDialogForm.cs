using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using MOG;
using MOG.CONTROLLER;
using MOG.INI;

namespace MOG_Client.Forms
{
	/// <summary>
	/// Summary description for PriviledgesForm.
	/// </summary>
	public class PriviledgesForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TreeView PriviledgesTreeView;
		private MOG_Ini mPriviledgesIni;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public PriviledgesForm( BASE mMog )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			mPriviledgesIni = mMog.GetPriviledgesIni();

			if( mPriviledgesIni != null && mPriviledgesIni.CountSections() > 0 )
			{
				PopulatePermissions();
			}
			else
			{
				PriviledgesTreeView.Nodes.Clear();
				PriviledgesTreeView.Nodes.Add( new TreeNode("No permissions set for this project") );
			}	
		}

		private void PopulatePermissions()
		{
			// Clear the file info tree
			PriviledgesTreeView.Nodes.Clear();
			PriviledgesTreeView.Sorted = true;

			string groupSection = MOG_Priviledges.MainSections.Group.ToString();
			SortedList groupNodeList = new SortedList();

			if( mPriviledgesIni.SectionExist( groupSection ) )
			{
				int groupsCount = mPriviledgesIni.CountKeys( groupSection );
				for( int i = 0; i < groupsCount; ++i)
				{
					string groupName = mPriviledgesIni.GetKeyNameByIndex( groupSection, i );
					TreeNode groupNode = new TreeNode( groupName );
					groupNodeList.Add( groupName, groupNode );
					PriviledgesTreeView.Nodes.Add( groupNode );
				}
			}

			string userSection = MOG_Priviledges.MainSections.Users.ToString();

			// If [Users] exists...
			if( mPriviledgesIni.SectionExist( userSection ) )
			{
				int usersCount = mPriviledgesIni.CountKeys( userSection );
				string[] users = new string[usersCount];
				string[] groups = new string[usersCount];

				// Foreach user...
				for(int i = 0; i < usersCount; ++i)
				{
					// Get user's name and group
					string userName = mPriviledgesIni.GetKeyNameByIndex( userSection, i );
					string userGroup = mPriviledgesIni.GetKeyByIndex( userSection, i );

					// Create our use's node
					TreeNode userNode = new TreeNode( userName );

					SortedList availablePermissions = new SortedList();

					// If the userGroup exists...
					if( mPriviledgesIni.SectionExist( userGroup ) )
					{
						int permissionsCount = mPriviledgesIni.CountKeys( userGroup );
						string key, value;

						// Foreach permission in userGroup...
						for( int j = 0; j < permissionsCount; ++j)
						{
							key = mPriviledgesIni.GetKeyNameByIndex( userGroup, j );
							value = mPriviledgesIni.GetKeyByIndex( userGroup, j );

							// Populate the permissions sorted list
							availablePermissions.Add( key, value );
						} // end for on permissions

						if( mPriviledgesIni.SectionExist( userName ) )
						{
							int userPermissionsCount = mPriviledgesIni.CountKeys( userName );

							// Foreach userPermission...
							for( int k = 0; k < userPermissionsCount; ++k)
							{
								key = mPriviledgesIni.GetKeyNameByIndex( userName, k );
								value = mPriviledgesIni.GetKeyByIndex( userName, k );

								string currentValue = (string)availablePermissions.GetByIndex( availablePermissions.IndexOfKey( key ) );

								if( availablePermissions.ContainsKey( key ) 
									&& currentValue != value )
								{
									string newPermissionEntry = key + "=" + value;
									TreeNode overrideNode = new TreeNode( newPermissionEntry );
									overrideNode.ForeColor = Color.Red;
									userNode.Nodes.Add( overrideNode );
									availablePermissions.Remove( key );
								}
							}
						}
					}
				
					// Foreach permission for this user...
					foreach( DictionaryEntry userPermission in availablePermissions )
					{
						string permissionEntry = userPermission.Key.ToString() + "=" + userPermission.Value.ToString();
						TreeNode permissionNode = new TreeNode( permissionEntry );
						if( userPermission.Value.ToString().ToLower() == "true" )
						{
							permissionNode.Checked = true;
						}
						else
						{
							permissionNode.Checked = false;
						}
						userNode.Nodes.Add( permissionNode );
					}

					TreeNode groupNode = (TreeNode)groupNodeList.GetByIndex( groupNodeList.IndexOfKey( userGroup ) );
					groupNode.Nodes.Add( userNode );
				} // end for on users				
			} // end if on [Users]
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.PriviledgesTreeView = new System.Windows.Forms.TreeView();
			this.SuspendLayout();
			// 
			// PriviledgesTreeView
			// 
			this.PriviledgesTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PriviledgesTreeView.ImageIndex = -1;
			this.PriviledgesTreeView.Location = new System.Drawing.Point(0, 0);
			this.PriviledgesTreeView.Name = "PriviledgesTreeView";
			this.PriviledgesTreeView.SelectedImageIndex = -1;
			this.PriviledgesTreeView.Size = new System.Drawing.Size(292, 273);
			this.PriviledgesTreeView.TabIndex = 0;
			// 
			// PriviledgesForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Controls.Add(this.PriviledgesTreeView);
			this.Name = "PriviledgesForm";
			this.Text = "Set Priviledges...";
			this.ResumeLayout(false);

		}
		#endregion
	}
}
