using System;
using System.Windows.Forms;
using System.Collections;

using MOG;
using MOG.USER;
using MOG.CONTROLLER.CONTROLLERPROJECT;

using MOG_Client.Client_Gui;
using MOG_Client.Client_Utilities;


namespace MOG_Client.Client_Mog_Utilities
{
	/// <summary>
	/// Summary description for User.
	/// </summary>
	public class guiUser
	{
		MogMainForm mainForm;

		public guiUser(MogMainForm main)
		{
			mainForm = main;
		}
		
		public bool SetActiveUser(string name)
		{
			// Make sure the user specified is not a label
			if (name.IndexOf("[") != -1)
				return false;

			// Set our active user
			if (MOG_ControllerProject.SetActiveUserName(name))
			{
				// If we have an asset manager, update it
				if (mainForm.mAssetManager != null)
				{
					// Set the gui to reflect changes
					mainForm.mAssetManager.SetActiveUser(name);

					// Make sure this user actually exists in the project?
					MOG_User user = MOG_ControllerProject.GetUser();
					if (user != null)
					{
						return true;
					}
				}
			}

			return false;
		}

		public bool SetLoginUser(string userName)
		{
			string ActiveUser, LoginUser;

			// Make sure the user specified is not a label
			if (userName.IndexOf("[") != -1)
				return false;

			if(	MOG_ControllerProject.LoginUser(userName) != null)
			{
				// Update tab pages
				if (mainForm.mAssetManager != null)
				{
					ActiveUser = mainForm.mAssetManager.GetActiveUser();
					LoginUser = MOG_ControllerProject.GetUser().GetUserName();

					if (MOG_ControllerProject.GetActiveUser() != null)
					{
						mainForm.mAssetManager.SetActiveUser(MOG_ControllerProject.GetActiveUser().GetUserName());
						ActiveUser = MOG_ControllerProject.GetActiveUser().GetUserName();
					}
				}
				else
				{
					ActiveUser = "None";
					LoginUser = "None";
				}

				// Update tab pages
				if (mainForm.mProjectManager != null)
				{
					mainForm.mProjectManager.Initialize();
				}

				if (mainForm.mAssetManager != null)
				{
					mainForm.mAssetManager.LatentInitialize();

					// Only load branches if we have already booted MOG
					if (mainForm.mAssetManager.mLoadBranches == false)
					{
						mainForm.mAssetManager.mLocal.LoadUserLocalBranches();
					}

					// Initialize the Tasks Window
					//mainForm.AssetManagerTaskWindow.InitializeForUser();
				}

				if (mainForm.mLibraryManager != null)
				{
					mainForm.mLibraryManager.Initialize();
				}

                // Enable the logout button
				mainForm.AssetManagerLogoutButton.Enabled = true;

				// Also set the default active user
				SetActiveUser(userName);

				// Set the status bar
				mainForm.MOGStatusBarUserBarPanel.Text = userName;

				// Save our prefs file
				guiUserPrefs.SaveStatic_ProjectPrefs();
				
				return true;
			}

			mainForm.AssetManagerLogoutButton.Enabled = false;
			return false;
		}

		static public void BuildUserMenu(MenuItem parent)
		{
			// Initialize the Explorer context menu
			parent.MenuItems.Clear();
			parent.MenuItems.Add("User");
			parent.MenuItems.Add("-");

			if ( MOG_ControllerProject.IsProject() )
			{
				// Add all the users to the AssignTo subMenu
				ArrayList users = MOG_ControllerProject.GetProject().GetUsers();
				for (int u = 0; u < users.Count; u++)
				{
					MOG_User user = (MOG_User)users[u];
					MenuItem Item = new MenuItem(user.GetUserName());

					//if (e != null) Item.Click += new System.EventHandler(e);

					parent.MenuItems.Add(Item);
				}

			}
		}
	}
}
