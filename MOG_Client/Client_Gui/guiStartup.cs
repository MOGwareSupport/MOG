using System;
using System.Collections;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Threading;

using MOG_ControlsLibrary;

using MOG;
using MOG.PROJECT;
using MOG.PLATFORM;
using MOG.USER;
using MOG.REPORT;
using MOG.PROMPT;
using MOG.DOSUTILS;
using MOG.DATABASE;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;

using MOG_Client.Client_Mog_Utilities;
using System.Collections.Generic;

namespace MOG_Client.Client_Gui
{
	/// <summary>
	/// Summary description for Startup.
	/// </summary>
	public class guiStartup
	{
		private MogMainForm mainForm;
		
		public guiStartup(MogMainForm main)
		{
			mainForm = main;

			try
			{
				mainForm.MOGWelcomeWebBrowser.Navigate("http://www.mogware.com/Startup/Client");
			}
			catch
			{
			}

			if (!MOG_ControllerSystem.GetOffline())
			{
				InitializeMog_Projects(main);
			}
		}

        public void InitializeMog_Projects(MogMainForm main)
		{
			// Initialize projects menu
			MainMenuProjectsClass.mainForm = mainForm;
			MainMenuProjectsClass.MOGGlobalProjectsInit(true);
			guiProject.mainForm = mainForm;
		}

		static public List<string> GetProjects()
		{
			List<string> projects = new List<string>();

			// Make sure the system has the latest project info
			MOG_ControllerSystem.GetSystem().Load();

			foreach (string project in MOG_ControllerSystem.GetSystem().GetProjectNames())
			{					
				projects.Add(project);
			}

			return projects;
		}

		static public List<string> GetPlatforms(string project)
		{
			List<string> platformNames = new List<string>();

			MOG_Project projectPtr;
			if (project.Length != 0)
			{
				projectPtr = MOG_ControllerSystem.GetSystem().GetProject(project);
			}
			else
			{
				projectPtr = MOG_ControllerProject.GetProject();
			}

			// Make sure we have an active project
			if (projectPtr != null)
			{
				ArrayList platforms = MOG_ControllerProject.GetProject().GetPlatforms();

				// Add a comboBox item for every platform
				foreach (MOG_Platform platform in platforms)
				{
					platformNames.Add(platform.mPlatformName);
				}
			}

			return platformNames;
		}

		internal static List<string> GetDepartments(string projectname)
		{
			List<string> departments = new List<string>();

			MOG_Project project = MOG_ControllerSystem.GetSystem().GetProject(projectname);
			if (project != null)
			{
				foreach (MOG_User user in project.GetUsers())
				{
					if (departments.Contains(user.GetUserDepartment()) == false)
					{
						departments.Add(user.GetUserDepartment());
					}					
				}
			}

			return departments;
		}

		static public List<string> GetUsers(string projectname, string department)
		{
			List<string> users = new List<string>();

			MOG_Project project = MOG_ControllerSystem.GetSystem().GetProject(projectname);
			if (project != null)
			{
				foreach (MOG_User user in project.GetUsers())
				{
					// Is this user within the same department as the one specified
					if (string.Compare(user.GetUserDepartment(), department, true) == 0)
					{
						users.Add(user.GetUserName());
					}
				}
			}

			return users;
		}

		static public List<string> GetBranches(string projectname)
		{
			List<string> branches = new List<string>();

			ArrayList branchNames = MOG_DBProjectAPI.GetAllBranchNames(projectname);
			if (branches != null)
			{
				foreach (MOG_DBBranchInfo branchName in branchNames)
				{
					// Ignore tags
					if (!branchName.mTag)
					{
						branches.Add(branchName.mBranchName);
					}
				}
			}

			return branches;
		}

		public static void StatusBarUpdate(MogMainForm mainForm, string modeText, string message)
		{
            mainForm.MainFormStatusBarUpdate(modeText, message);
		}

		public static void StatusBarClear(MogMainForm mainForm)
		{
            StatusBarUpdate(mainForm, "", "");
		}

		/// <summary>
		/// Set the connection status in the status bar
		/// </summary>
		/// <param name="connected"></param>
		public static void ConnectionStatus(MogMainForm mainForm, bool connected)
		{
			bool problemLoadingIcon = false;
			if (connected)
			{
				if( mainForm.StatusBarImageList.Images.Count > 0 )
				{
					Bitmap home = new Bitmap(mainForm.StatusBarImageList.Images[0]);			
					mainForm.MOGStatusBarConnectionStatusBarPanel.Icon = System.Drawing.Icon.FromHandle(home.GetHicon());					
				}
				else
				{
					problemLoadingIcon = true;
				}
				mainForm.MOGStatusBarConnectionStatusBarPanel.Text = "Connected";

				mainForm.MOGStatusBarConnectionStatusBarPanel.ToolTipText = mainForm.RefreshConnectionToolText();
				MOG_ControllerSystem.GoOnline();
			}
			else
			{
				if( mainForm.StatusBarImageList.Images.Count > 1 )
				{
					Bitmap home = new Bitmap(mainForm.StatusBarImageList.Images[1]);			
					mainForm.MOGStatusBarConnectionStatusBarPanel.Icon = System.Drawing.Icon.FromHandle(home.GetHicon());					
				}
				else
				{
					problemLoadingIcon = true;
				}
				mainForm.MOGStatusBarConnectionStatusBarPanel.Text = "Disconnected";
				mainForm.MOGStatusBarConnectionStatusBarPanel.ToolTipText = string.Concat("Could not connect to ", MOG_ControllerSystem.GetServerComputerName(), " IP:", MOG_ControllerSystem.GetServerComputerIP());
				MOG_ControllerSystem.GoOffline();
			}

			if( problemLoadingIcon )
			{
				MOG_Prompt.PromptMessage( "Project Error", "Programmer: Please rebuild MOG_Client to have all the appropriate *.resx "
					+ "\r\nfiles correctly included in this project.");
			}
		}

		public static bool SetOffline(MogMainForm mainForm, string offline)
		{
			switch (offline.ToLower())
			{
				case "true":
				case "1":
					return guiStartup.SetOffline(mainForm, true);
//					break;
				case "false":
				case "0":
					return guiStartup.SetOffline(mainForm, false);
//					break;				
			}

			return false;
		}
		public static bool SetOffline(MogMainForm mainForm, bool offline)
		{
			if (offline)
			{
				// TODO KIER How do we go offline now?

//				string targetPath = string.Concat(MOG_ControllerProject.GetGameData().GetGameDataPath(), "\\MOG\\Offline");
//				if (MOG_ControllerProject.GetProject().Offline(targetPath))
//				{
//					// Disconnect from the server
//					MOG_ControllerSystem.GoOffline();
//
//					if (MOG_ControllerSystem.GetSystem().Load(MOG_Main.BuildDefaultConfigFile(targetPath, "", "")))
//					{
//						// Reload the project
//						MOG_ControllerProject.LoginProject(MOG_ControllerProject.GetProjectName(), MOG_ControllerProject.GetBranchName());
//
//						// Set our gui
//						guiStartup.ConnectionStatus(mainForm, false);
//					}
//				}
			}
			else
			{
				// Connect to the server
				MOG_ControllerSystem.GoOnline();

				if (MOG_ControllerSystem.GetSystem().Load(MOG_Main.BuildDefaultConfigFile()))
				{
					// Reload the project
					MOG_ControllerProject.LoginProject(MOG_ControllerProject.GetProjectName(), MOG_ControllerProject.GetBranchName());

					// Login user
					MOG_ControllerProject.LoginUser(MOG_ControllerProject.GetUser().GetUserName());

					// Set our gui
					guiStartup.ConnectionStatus(mainForm, true);
				}
			}

			return true;
		}		
	}
}
