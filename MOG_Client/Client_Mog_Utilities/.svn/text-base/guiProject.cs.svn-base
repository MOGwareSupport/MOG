using System;
using System.Collections;
using System.Windows.Forms;

using MOG;
using MOG.REPORT;
using MOG.PROMPT;
using MOG.PLATFORM;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;

//using MOG_Client.Client_Mog_Utilities.AssetOptions;
using MOG_Client.Client_Gui;
using MOG_ControlsLibrary.Utils;
using MOG_Client.Client_Utilities;
using System.Collections.Generic;
using MOG_Client.Client_Gui.AssetManager_Helper;



namespace MOG_Client.Client_Mog_Utilities
{
	/// <summary>
	/// Summary description for Project.
	/// </summary>
	public class guiProject
	{
		public static MogMainForm mainForm;

		static public bool SetLoginProject(string name, string branch)
		{
			// Initialize users on the Asset Manager page
			if (mainForm.mAssetManager != null )
			{
				mainForm.mAssetManager.DeInitialize();
			}

			if (MOG_ControllerProject.ProjectExists(name))
			{
				if (MOG_ControllerProject.LoginProject(name, branch) != null)
				{
					// Initialize branches
					MainMenuProjectsClass.MOGGlobalBranchesInit(true);
					UpdateGuiBranch(MOG_ControllerProject.GetBranchName());

					// Initialize the project icons (clear them first)
					MogUtil_AssetIcons.ClassIconsClear();
					MogUtil_AssetIcons.ClassIconInitialize();

					// Initialize users on the Asset Manager page
					if (mainForm.mAssetManager != null)
					{
						//mainForm.mAssetManager.DeInitialize();
						mainForm.mAssetManager.Initialize();

						List<string> departments = guiStartup.GetDepartments(name);
						mainForm.AssetManagerActiveUserDepartmentsComboBox.Items.Clear();
						mainForm.AssetManagerActiveUserDepartmentsComboBox.Items.AddRange(departments.ToArray());
					}

					// Load web tabs
					if (mainForm.mWebTabManager != null)
					{
						mainForm.mWebTabManager.LoadTabs();
					}

					// Update the project related gui stuff
					UpdateGuiProject(name);

					// Check if we passed in a valid branch
					if (branch.Length != 0)
					{
						UpdateGuiBranch(MOG_ControllerProject.GetBranchName());
					}

					// Save our prefs file
					guiUserPrefs.SaveStatic_ProjectPrefs();

					return true;
				}
			}

			return false;
		}

		static public void UpdateGuiBranch(string branch)
		{
			// Set the check mark in the menu
			foreach (ToolStripItem item in mainForm.branchesToolStripMenuItem.DropDownItems)
			{
				ToolStripMenuItem menuItem = item as ToolStripMenuItem;
				if (menuItem != null)
				{
					if (string.Compare(menuItem.Text, branch, true) == 0)
					{
						menuItem.Checked = true;
					}
					else
					{
						menuItem.Checked = false;
					}
				}
			}

			// Set the status bar
			mainForm.MOGStatusBarBranchStatusBarPanel.Text = branch;
		}

		static public void UpdateGuiProject(string project)
		{
			// Set the check mark in the menu
			foreach (ToolStripItem item in mainForm.projectsToolStripMenuItem.DropDownItems)
			{
				ToolStripMenuItem menuItem = item as ToolStripMenuItem;
				if (menuItem != null)
				{
					if (string.Compare(menuItem.Text, project, true) == 0)
					{
						menuItem.Checked = true;
					}
					else
					{
						menuItem.Checked = false;
					}
				}
			}

			// Set the status bar
			mainForm.MOGStatusBarProjectStatusBarPanel.Text = project;
		}

		static public bool SetLoginBranch(string branch)
		{
			if (string.Compare(MOG_ControllerProject.GetBranchName(), branch, true) != 0)
			{
				if( MOG_ControllerProject.LoginProject(MOG_ControllerProject.GetProjectName(), branch) != null && string.Compare(MOG_ControllerProject.GetBranchName(), branch, true) == 0)
				{
					MessageBox.Show("You have been switched to a different branch of the project.\n" + 
									"Please keep this in mind as you bless assets, browse project trees or configure project settings.\n\n" + 
									"     BRANCH: " + branch + "\n",
									"Changing Project Branch", MessageBoxButtons.OK);
					UpdateGuiBranch(MOG_ControllerProject.GetBranchName());

					// Check if we have a current workspace?
					if (MOG_ControllerProject.GetCurrentSyncDataController() != null)
					{
						// Check if this branch is different than our workspace's branch?
						if (string.Compare(branch, MOG_ControllerProject.GetCurrentSyncDataController().GetBranchName(), true) != 0)
						{
							// Switch the associated branch of the current workspace
							WorkspaceManager.SwitchCurrentWorkspaceBranch(branch);
						}
					}

					// Update the project manager if it has been initialized
					if(mainForm.mProjectManager != null)
					{
						mainForm.mProjectManager.BuildRepositoryTrees( true );
					}
		
					return true;
				}			
				MOG_Prompt.PromptMessage("Login Branch", "Could not login to selected branch!\nProject:" + MOG_ControllerProject.GetProjectName() + "\nBranch:" + branch, Environment.StackTrace);				
				return false;
			}

			return true;
		}
	}
}
