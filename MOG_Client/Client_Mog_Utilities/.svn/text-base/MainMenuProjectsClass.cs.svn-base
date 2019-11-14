using System;
using System.Collections;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.IO;

using MOG_Client;
using MOG_Client.Forms;
using MOG_Client.Client_Gui;
using MOG_Client.Client_Mog_Utilities.AssetOptions;
using MOG_Client.Client_Mog_Utilities;
using MOG_ControlsLibrary;
using MOG_ControlsLibrary.Common.MOG_ContextMenu;
using MOG_ControlsLibrary.Utils;

using MOG;
using MOG.PROJECT;
using MOG.INI;
using MOG.DOSUTILS;
using MOG.REPORT;
using MOG.PROMPT;
using MOG.DATABASE;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;


namespace MOG_Client.Client_Mog_Utilities
{
	/// <summary>
	/// Summary description for MainMenuViewClass.
	/// </summary>
	public class MainMenuProjectsClass
	{
		public static MogMainForm mainForm;

		static public void MOGGlobalProjectsInit(bool force)
		{
			// Add all valid login projects
			if (mainForm.projectsToolStripMenuItem.DropDownItems.Count == 0 || force)
			{
				mainForm.projectsToolStripMenuItem.DropDownItems.Clear();

				// Reload the system ini
				MOG_ControllerSystem.GetSystem().Load(MOG_ControllerSystem.GetSystem().GetConfigFilename());

				foreach (string project in MOG_ControllerSystem.GetSystem().GetProjectNames())
				{
					ToolStripMenuItem Item = new ToolStripMenuItem(project);
					Item.Click += new System.EventHandler(MainMenuProjectsClass.MOGGlobalProjects_Click);
					mainForm.projectsToolStripMenuItem.DropDownItems.Add(Item);
				}

				mainForm.projectsToolStripMenuItem.Enabled = true;
			}
		}

		static public void MOGGlobalBranchesInit(bool force)
		{
			// Add all valid login projects
			if (mainForm.branchesToolStripMenuItem.DropDownItems.Count == 0 || force)
			{
				mainForm.branchesToolStripMenuItem.DropDownItems.Clear();

				ArrayList Branches = MOG_DBProjectAPI.GetActiveBranchNames();

				if (Branches != null)
				{
					foreach (MOG_DBBranchInfo branch in Branches)
					{
						// Only show branches, not TAGS
						if (!branch.mTag)
						{
							ToolStripMenuItem Item = new ToolStripMenuItem(branch.mBranchName);
							Item.Click += new System.EventHandler(MainMenuProjectsClass.MOGGlobalBranches_Click);

							// Check if this is our currently selected branch
							if (string.Compare(branch.mBranchName, MOG_ControllerProject.GetBranchName(), true) == 0)
							{
								Item.Checked = true;
							}

							// Set light version control
							MogUtil_VersionInfo.SetLightVersionControl(Item);

							mainForm.branchesToolStripMenuItem.DropDownItems.Add(Item);
						}
					}
				}

				mainForm.branchesToolStripMenuItem.Enabled = true;                
			}
		}

      	static private void MOGGlobalBranches_Click(object sender, System.EventArgs e)
		{
			string branch = ((ToolStripMenuItem)sender).Text;
			
			// Check if we already have an Active Branch?
			if (MOG_ControllerProject.IsBranch())
			{
				// Check if they just specified the already active Branch?
				if (string.Compare(MOG_ControllerProject.GetBranchName(), branch, true) == 0)
				{
					// No need to login into the already active project
					return;
				}
			}

			// Login into the specified Branch
			if (guiProject.SetLoginBranch(branch))
			{
				((ToolStripMenuItem)sender).Checked = true;				
			}
			else
			{
				guiProject.UpdateGuiBranch(MOG_ControllerProject.GetBranchName());
			}
		}

		static private void MOGGlobalBranchCreate_Click(object sender, System.EventArgs e)
		{
			string branch = ((ToolStripMenuItem)sender).Text;
			
			CreateBranchForm newBranch = new CreateBranchForm();
			newBranch.BranchSourceTextBox.Text = MOG_ControllerProject.GetBranchName();
			
			if (newBranch.ShowDialog() == DialogResult.OK)
			{
				if (MOG_ControllerProject.BranchCreate(MOG_ControllerProject.GetBranchName(), newBranch.BranchNameTextBox.Text))
				{
					// Rebuild the branch menu
					MainMenuProjectsClass.MOGGlobalBranchesInit(true);
					MOG_Prompt.PromptMessage("Create Branch", "New branch successfully created.\n" + 
											 "BRANCH: " + newBranch.BranchNameTextBox.Text);
				}
			}
		}

		// Enable the mainForm's MOGGlobalFileSQLConnectionMenuItem (SQL Connection MenuItem) if userName is in the Admin group, otherwise disable it
		static public void MOGGlobalSetSQLConnectionMenuItemEnabled(string userName)
		{
			MOG_Privileges privileges = new MOG_Privileges();
			if (privileges.ValidateUser(userName)  &&  privileges.GetUserGroup(userName).ToLower() == "admin")
			{
				mainForm.testSQLConnectionToolStripMenuItem.Enabled = true;
			}
			else
			{
				mainForm.testSQLConnectionToolStripMenuItem.Enabled = false;
			}
		}

		// Enable the mainForm's MOGGlobalToolsConfigureProjectMenuItem (configure project) if userName is in the Admin group, otherwise disable it
		static public void MOGGlobalSetToolsConfigureProjectMenuItemEnabled(string userName)
		{
			MOG_Privileges privileges = new MOG_Privileges();
			if (privileges.ValidateUser(userName)  &&  privileges.GetUserGroup(userName).ToLower() == "admin")
			{
				mainForm.configureProjectToolStripMenuItem.Enabled = true;
			}
			else
			{
				mainForm.configureProjectToolStripMenuItem.Enabled = false;
			}
		}

		// Enable the mainForm's MOGGlobalFileMOGRepositoryMenuItem (set MOG repository) if userName is in the Admin group, otherwise disable it
		static public void MOGGlobalSetFileMOGRepositoryMenuItemEnabled(string userName)
		{
			MOG_Privileges privileges = new MOG_Privileges();
			if (privileges.ValidateUser(userName)  &&  privileges.GetUserGroup(userName).ToLower() == "admin")
			{
				mainForm.setMOGRepositoryToolStripMenuItem.Enabled = true;
			}
			else
			{
				mainForm.setMOGRepositoryToolStripMenuItem.Enabled = false;
			}
		}
		

		static private void MOGGlobalProjects_Click(object sender, System.EventArgs e)
		{
			string project = ((ToolStripMenuItem)sender).Text;
			
			MOGGlobalLaunchProjectLogin(project, false);
		}

		public delegate void MOGProjectRefresh();

		static private void ProjectRefresh()
		{
			string projectName = MOG_ControllerProject.GetProjectName();
			string branchName = MOG_ControllerProject.GetBranchName();
			string userName = MOG_ControllerProject.GetUserName();

			// Refresh all my user list controls
			if (guiProject.SetLoginProject(projectName, branchName))
			{
				// Set the login user
				guiUser guiUsers = new guiUser(mainForm);

				if (!guiUsers.SetLoginUser(userName))
				{
					// Error
					MOG_Prompt.PromptResponse("Refresh Project", "Unable to refresh user!  Try re-logging in to your project");
				}
			}
			else
			{
				// Error
				MOG_Prompt.PromptResponse("Refresh Project", "No local workspace or login to project failed!, Try re-logging in to your project");
			}

			// Refresh all my project lists
		}

		static public void MOGGlobalRefreshProject(MogMainForm main)
		{
			MOGProjectRefresh del = new MOGProjectRefresh(ProjectRefresh);

			main.Invoke(del);
		}

		static public void MOGGlobalLaunchProjectLogin(string projectName, bool forceLogin)
		{
			// Launch the login dialog
			LoginForm login = new LoginForm(projectName);
			mainForm.Enabled = false;

			Login:
				// Show the dialog
				if (login.ShowDialog(mainForm) == DialogResult.OK)
				{	
					try
					{
						// Login to the specified Project
						if (guiProject.SetLoginProject(login.LoginProjectsComboBox.Text, login.LoginBranchesComboBox.Text))
						{

							// Set the login user
							guiUser guiUsers = new guiUser(mainForm);
							if ((string.Compare(login.LoginUsersComboBox.Text, "Choose Valid User", true) == 0) || (login.LoginUsersComboBox.Text.Length == 0) )
							{
								MessageBox.Show("A valid user must be selected!", "Missing User");
								goto Login;
							}

							if (guiUsers.SetLoginUser(login.LoginUsersComboBox.Text))
							{
								mainForm.Enabled = true;

								// Disable the Change SQL Server menu item if the logged in user is not an administrator
								MOGGlobalSetSQLConnectionMenuItemEnabled( login.LoginUsersComboBox.Text );
								// Disable the Configure Project menu item if the logged in user is not an administrator
								MOGGlobalSetToolsConfigureProjectMenuItemEnabled( login.LoginUsersComboBox.Text );
								// Disable the Set MOG Repository menu item if the logged in user is not an administrator
								MOGGlobalSetFileMOGRepositoryMenuItemEnabled( login.LoginUsersComboBox.Text );
							}
							else
							{
								MessageBox.Show("A valid user must be selected!", "Login Error");
								goto Login;
							}
						}
						else
						{
							MessageBox.Show("A valid project and branch must be selected!", "Login Error");
							goto Login;
						}
					}
					catch(Exception e)
					{
						MOG_Report.ReportMessage("Login Project", e.Message, e.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.ERROR);
						goto Login;
					}
				}
				else if (login.DialogResult == DialogResult.Cancel && forceLogin)
				{
					if (MessageBox.Show(mainForm, "Do you wish to exit MOG?", "Exit?", MessageBoxButtons.YesNo) == DialogResult.Yes)
					{
						mainForm.Close();
						mainForm.Shutdown();
					}
					else
					{
						goto Login;
					}
				}
				else
				{
					if (!MOG_ControllerProject.IsProject() || !MOG_ControllerProject.IsUser())
					{
						MessageBox.Show("A valid project and user must be selected!", "Missing Project or User");
						goto Login;
					}

					mainForm.Enabled = true;
				}

				// Always initialize our Database before leaving because the dialog loads projects that will leave us in a dirty state
				MOG_ControllerSystem.InitializeDatabase("", MOG_ControllerProject.GetProjectName(), MOG_ControllerProject.GetBranchName());
			}
	}
}
