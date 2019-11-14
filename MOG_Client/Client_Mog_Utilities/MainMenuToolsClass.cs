using System;
using System.Collections;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.IO;

using MOG_Client;
using MOG_Client.Forms;
using MOG_Client.Client_Mog_Utilities.AssetOptions;
using MOG_ControlsLibrary.Utils;
using MOG_ControlsLibrary.Common.MogForm_AutomatedTesting;

using MOG;
using MOG.PROJECT;
using MOG.PROPERTIES;
using MOG.PLATFORM;
using MOG.INI;
using MOG.DOSUTILS;
using MOG.TIME;
using MOG.REPORT;
using MOG.PROMPT;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.ENVIRONMENT_VARS;
using MOG.SYSTEMUTILITIES;

using MOG_Client.Client_Utilities;
using MOG_Client.Client_Gui;
using MOG_ControlsLibrary.MogUtils_Settings;
using MOG_ControlsLibrary.Common.MogControl_PrivilegesForm;

namespace MOG_Client.Client_Mog_Utilities
{
	/// <summary>
	/// Summary description for MainMenuToolsClass.
	/// </summary>
	public class MainMenuToolsClass
	{
		public MainMenuToolsClass()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		static public void PopupVerificationForRights(MogMainForm mainForm)
		{
			MOG_Privileges privilidges = new MOG_Privileges();
			if (privilidges.GetUserPrivilege( MOG_ControllerProject.GetUser().GetUserName(), MOG_PRIVILEGE.AccessAdminTools))
			{
				mainForm.serverManagerToolStripMenuItem.Enabled = true;
				mainForm.eventViewerToolStripMenuItem.Enabled = true;				
			}
			else
			{
				mainForm.serverManagerToolStripMenuItem.Enabled = false;
				mainForm.eventViewerToolStripMenuItem.Enabled = false;				
			}
		}

		static public void MOGGlobalToolsRequestBuildForm(MogMainForm mainForm)
		{
			RequestBuildForm build = new RequestBuildForm();
			if (build.ShowDialog() == DialogResult.OK)
			{
				// Launch the request
				foreach (string command in build.BuildRequests.Items)
				{
					LaunchBuild(mainForm, command, build.mOptions);
				}				
			}
		}

		static private void LaunchBuild(MogMainForm mainForm, string command, MOG_Ini options)
		{
			// Extract the Build's Options
			string BuildPlatform = "";
			string BuildType = "";

			// Split up the Build's Options
			string []commands = command.Trim().Split(" ".ToCharArray());
			if (command.Length == 0 || commands.Length < 1)
			{
				MOG_Prompt.PromptMessage("Request build", "Bad or missing command (" + command + ") in request build!", Environment.StackTrace);
				return;
			}

			BuildType = commands[0];

			if (commands.Length > 1)
			{
				BuildPlatform = commands[1];
			}


			// Get the list of available slaves to use for builds
			// Get the highest slaves
			string validSlaves = "";
			if (options.SectionExist("Builds.ValidSlaves") && 
				options.KeyExist("Builds.ValidSlaves", "Builds"))
			{
				validSlaves = options.GetString("Builds.ValidSlaves", "Builds");
			}

			string buildTypeSection = "Builds." + BuildType;

			// Locate the buildTool within the Build.Options.Info file
			string buildTool = "";
			// Determine what the executable name is
			if (options.SectionExist(buildTypeSection))
			{
				// Check for a buildTypeSection level validSlaves definition
				if (options.SectionExist("Builds.ValidSlaves") && options.KeyExist("Builds.ValidSlaves", buildTypeSection))
				{
					validSlaves = options.GetString("Builds.ValidSlaves", buildTypeSection);
				}

				// Check if a specific BuildPlatform was specified? and
				// Check if we have keys in our buildTypeSection?
				if (BuildPlatform.Length > 0 &&
					options.CountKeys(buildTypeSection) > 0)
				{
					// Obtain the build tool for this build type and BuildPlatform
					buildTool = options.GetString(buildTypeSection, BuildPlatform);
					if (buildTool.Length > 0)
					{
						// Check for a BuildPlatform level validSlaves definition
						if (options.SectionExist("Builds.ValidSlaves") && options.KeyExist("Builds.ValidSlaves", buildTypeSection + "." + BuildPlatform))
						{
							validSlaves = options.GetString("Builds.ValidSlaves", buildTypeSection + "." + BuildPlatform);
						}
					}
				}
				else if (options.KeyExist("Builds", BuildType))
				{
					buildTool = options.GetString("Builds", BuildType);
				}
				else
				{
				}
			}

			// Make sure we have a valid build tool?
			if (buildTool.Length > 0)
			{
				// Locate the buildTool in the tools directories
				string buildToolRelativePath = Path.GetDirectoryName(buildTool);
				buildTool = MOG_ControllerSystem.LocateTool(buildToolRelativePath, buildTool);
				if (buildTool.Length > 0)
				{
					//:: -----------------------------------------------------------------------
					//:: MOG ENVIRONMENT VARIABLES
					//:: -----------------------------------------------------------------------
					string EnvVariables =	MOG_EnvironmentVars.CreateToolsPathEnvironmentVariableString(buildToolRelativePath) + "," +
											MOG_EnvironmentVars.GetBuildProjectName() + "=" + MOG_ControllerProject.GetProjectName() + "," +
											MOG_EnvironmentVars.GetBuildProjectBranchName() + "=" + MOG_ControllerProject.GetBranchName() + "," +
											MOG_EnvironmentVars.GetBuildType() + "=" + BuildType + "," +
											MOG_EnvironmentVars.GetBuildPlatformName() + "=" + BuildPlatform + "," +
											MOG_EnvironmentVars.GetBuildTool() + "=" + buildTool + "," +
											MOG_EnvironmentVars.GetBuildToolPath() + "=" + System.IO.Path.GetDirectoryName(buildTool) + "," +
											MOG_EnvironmentVars.GetBuildRequestedBy() + "=" + MOG_ControllerProject.GetUser().GetUserName();

					// Fire off the build
					if (MOG_ControllerProject.Build(buildTool, validSlaves, MOG_ControllerProject.GetUser().GetUserName(), EnvVariables))
					{
						string message = "Build Request Successfull.\n" + 
										 "You will be notified when it is completed.";
						MessageBox.Show(message, "Refresh Build", MessageBoxButtons.OK);
					}
				}
				else
				{
					string message = "MOG was unable to locate the specified build tool.\n" + 
									 "BUILD TOOL: " + buildTool + "\n\n" +
									 "Your MOG Administrator may need to internalize this tool before it will work.";
					MOG_Prompt.PromptMessage("Request Build", message);
				}
			}
			else
			{
				string message = "No build tool was located for the selected build type.\n" +
								 "BUILD TYPE: " + buildTypeSection + "\n\n" +
								 "Your MOG Administrator may need to edit the 'Build.Options.Info' file to configure this build type.";
				MOG_Prompt.PromptMessage("Request Build", message);
			}
		}

		static public void MOGGlobalToolsRequestBuild(MogMainForm mainForm, MenuItem MenuPlatform)
		{
			// Build default values
			string targetProject = MOG_ControllerProject.GetProject().GetProjectCvsName().ToLower();
			string targetDirectory = MOG_ControllerProject.GetProject().GetProjectBuildDirectory();
			string sourceDirectory = MOG_ControllerProject.GetProject().GetProjectToolsPath();
			
			// Get the build type
			string parentMenu = MenuPlatform.Parent.ToString().ToLower();
			// Parse the string to get to the name "Advent.Gold" then move our index to the length of the word "advent" + 1 for the "."
			string BuildType = parentMenu.Substring(parentMenu.LastIndexOf(targetProject) + targetProject.Length + 1);

			// Get the list of available slaves to use for builds
			string validSlaves = "";
			if (MOG_ControllerProject.GetProject().GetConfigFile().SectionExist("BuildMachines"))
			{
				for ( int i = 0; i < MOG_ControllerProject.GetProject().GetConfigFile().CountKeys("BuildMachines"); i++)
				{
					if ( validSlaves.Length == 0)
					{
						validSlaves = MOG_ControllerProject.GetProject().GetConfigFile().GetKeyNameByIndexSLOW("BuildMachines", i);
					}
					else
					{
						validSlaves = string.Concat(validSlaves, "," , MOG_ControllerProject.GetProject().GetConfigFile().GetKeyNameByIndexSLOW("BuildMachines", i));
					}
				}				
			}
			
			string platform;
			// Determine BuildPlatform
			if (string.Compare(MenuPlatform.Text, "All", true) == 0)
			{
				ArrayList platformList = MOG_ControllerProject.GetProject().GetPlatforms();
				for (int p = 0; p < platformList.Count; p++)
				{
					MOG_Platform MogPlatform = (MOG_Platform)platformList[p];
					platform = MogPlatform.mPlatformName;

					// Create build name
					string buildName = string.Concat(MOG_ControllerProject.GetProjectName(), ".", "Build.", targetProject, ".", BuildType, ".AdventGameCode.", platform);

					// Fire the refresh build
					//					if (MOG_ControllerProject.Build(buildName, sourceDirectory, targetDirectory, validSlaves, false, MOG_ControllerProject::GetActiveUser().GetUserName(), ""))
					//					{
					//						MessageBox.Show(string.Concat("Server has recieved your build request for a (", BuildPlatform, ") build.  You will be notified when it completes."), "Refresh Build", MOGPromptButtons.OK);
					//					}
				}
			}
			else
			{
				platform = MenuPlatform.Text;

				// Create build name
				string buildName = string.Concat(MOG_ControllerProject.GetProjectName(), ".", "Build.", targetProject, ".", BuildType, ".AdventGameCode.", platform);

				// Fire the refresh build
				//				if (MOG_ControllerProject.Build(buildName, sourceDirectory, targetDirectory, validSlaves, false, MOG_ControllerProject::GetActiveUser().GetUserName(), ""))
				//				{
				//					MessageBox.Show("Server has recieved your build request.  You will be notified when it completes.", "Refresh Build", MOGPromptButtons.OK);
				//				}
			}			
		}

		static public void MOGGlobalToolsLoadReport(MogMainForm mainForm)
		{
			string ReportDir = MOG_ControllerProject.GetUser().GetUserPath() + "\\Reports";
			
			ListForm report = new ListForm("Load custom user report");
			report.ListListView.SmallImageList = MogUtil_AssetIcons.Images;

			mainForm.MOGOpenFileDialog.Filter = "Mog Reports | *.rep";
			mainForm.MOGOpenFileDialog.InitialDirectory = ReportDir;
			if (mainForm.MOGOpenFileDialog.ShowDialog(mainForm) == DialogResult.OK)
			{
				report.Show(mainForm);
				report.LoadReportList(mainForm.MOGOpenFileDialog.FileName);
			}
		}

		static public void MOGGlobalToolsPostBuild(MogMainForm mainForm)
		{
			if(mainForm.mAssetManager != null)
			{
				mainForm.mAssetManager.BuildPost();
			}
		}


		static public void MOGGlobalToolsPermisions(MogMainForm mainForm)
		{
			// Encapsulate everything in a try-catch
			try
			{
				if( MOG_ControllerProject.GetPrivileges() != null )
				{
					MogControl_PrivilegesForm privilegesForm = new MogControl_PrivilegesForm( MOG_ControllerProject.GetPrivileges() );
					privilegesForm.StartPosition = FormStartPosition.CenterParent;
					DialogResult result = privilegesForm.ShowDialog(mainForm);
					result.ToString();
				}
				else
				{
					MOG_Prompt.PromptMessage( "Permissions Error!", "Unable to open Permissions Form.  " 
						+ "Please make sure a valid project is selected.\r\n\r\nYou may try clicking Projects |"
						+ " (The Current Project) to resolve this error, and/or close and re-open MOG.", Environment.StackTrace );
				}
			}
				// Catch any .NET-explainable exceptions.
			catch( Exception ex )
			{
				MOG_Report.ReportMessage( "Error in Privileges Change Form!", ex.Message, 
					ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.ERROR );
			}
		}

		static public void MOGGlobalToolsSavePrefs(MogMainForm mainForm)
		{
			// Show wait cursor while we save prefs
			mainForm.Cursor = Cursors.WaitCursor;

			// Save user prefs
			mainForm.mUserPrefs.SaveStaticForm_LayoutPrefs();
			guiUserPrefs.SaveDynamic_LayoutPrefs("AssetManager", mainForm);			
			guiUserPrefs.SaveStatic_ProjectPrefs();	
			mainForm.mUserPrefs.Save();

			// Give the user 1/10th of a second to see that we saved
			Thread.Sleep(100);

			// Change back to normal cursor
			mainForm.Cursor = Cursors.Default;
		}

		static public void MOGGlobalToolsLaunchEventViewer(MogMainForm mainForm)
		{
			string output = "";
			string command = MOG.MOG_Main.GetExecutablePath() + "\\MOG_EventViewer.exe";
			guiCommandLine.ShellSpawn(command, "", ProcessWindowStyle.Normal, ref output);
		}

		static public void MOGGlobalToolsLaunchServerManager(MogMainForm mainForm)
		{
			string output = "";
			string command = MOG.MOG_Main.GetExecutablePath() + "\\MOG_ServerManager.exe";
			guiCommandLine.ShellSpawn(command, "", ProcessWindowStyle.Normal, ref output);
		}

		static public void MOGGlobalToolsSoundEnable(MogMainForm mainForm, bool enabled)
		{
			mainForm.mSoundManager.Enabled = enabled;
			MogUtils_Settings.SaveSetting("Gui", "Sounds", mainForm.mSoundManager.Enabled.ToString());
			MogUtils_Settings.Save();			
		}

		internal static void SetMogLibraryMode(MogMainForm mogMainForm)
		{
			mogMainForm.requestBuildToolStripMenuItem.Visible = false;
			mogMainForm.toolStripSeparator5.Visible = false;
		}
	}
}
