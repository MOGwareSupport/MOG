using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

using MOG_Client.Client_Utilities;
using MOG_Client.Client_Mog_Utilities;

using MOG;
using MOG.INI;
using MOG.DOSUTILS;
using MOG.REPORT;
using MOG.TIME;

namespace MOG_Client.Client_Gui.AssetManager_Helper
{
	/// <summary>
	/// Summary description for guiAssetManagerProjectTools.
	/// </summary>
	public class guiAssetManagerProjectTools
	{
		public BASE mMog;													// Pointer to global MOG
		public MogMainForm mainForm;										// Pointer to the main form
		private Thread mPlatformSyncThread;									// Thread for performing a platform sync to a console

		// Custom buttons
		public guiAssetManagerCustomButtons	mButtons;
		
		// Sync options
		private string mTargetXbox;
		public string mUserSyncFile;
		public string mProjectSyncFile;
		
		public guiAssetManagerProjectTools(BASE mog, MogMainForm main)
		{
			mMog = mog;
			mainForm = main;

			mButtons = new guiAssetManagerCustomButtons(mainForm);
		}

		public void Initialize()
		{
			InitializeXboxList();

			// HACK Advent only code
			// Initialize the tool buttons
			mButtons.AddButtonHandle("pc", mainForm.AssetManagerLocalDataPc0Button);
			mButtons.AddButtonHandle("pc", mainForm.AssetManagerLocalDataPc1Button);
			mButtons.AddButtonHandle("pc", mainForm.AssetManagerLocalDataPc2Button);
			mButtons.AddButtonHandle("pc", mainForm.AssetManagerLocalDataPc3Button);
			mButtons.AddButtonHandle("pc", mainForm.AssetManagerLocalDataPc4Button);
			mButtons.AddButtonHandle("pc", mainForm.AssetManagerLocalDataPc5Button);
			mButtons.AddButtonHandle("pc", mainForm.AssetManagerLocalDataPc6Button);
			mButtons.AddButtonHandle("pc", mainForm.AssetManagerLocalDataPc7Button);
			mButtons.AddButtonHandle("pc", mainForm.AssetManagerLocalDataPc8Button);

			mButtons.AddButtonHandle("xbox", mainForm.AssetManagerLocalDataXbox0Button);
			mButtons.AddButtonHandle("xbox", mainForm.AssetManagerLocalDataXbox1Button);
			mButtons.AddButtonHandle("xbox", mainForm.AssetManagerLocalDataXbox2Button);
			mButtons.AddButtonHandle("xbox", mainForm.AssetManagerLocalDataXbox3Button);
			mButtons.AddButtonHandle("xbox", mainForm.AssetManagerLocalDataXbox4Button);
			mButtons.AddButtonHandle("xbox", mainForm.AssetManagerLocalDataXbox5Button);
			mButtons.AddButtonHandle("xbox", mainForm.AssetManagerLocalDataXbox6Button);
			mButtons.AddButtonHandle("xbox", mainForm.AssetManagerLocalDataXbox7Button);
			mButtons.AddButtonHandle("xbox", mainForm.AssetManagerLocalDataXbox8Button);

			mButtons.Load();
		}

		public void LoadSyncFilePrefs()
		{
			string prefsSection = mMog.GetProject().GetProjectName() + "." + mMog.GetActivePlatform().mPlatformName;				
			mUserSyncFile = guiUserPrefs.Load(prefsSection, "UserSyncFile");
			mProjectSyncFile = guiUserPrefs.Load(prefsSection, "ProjectSyncFile");	
		}

		#region XboxTools
		public string GetTargetXbox()
		{
			return mTargetXbox;
		}

		private void InitializeXboxList()
		{
			if (mMog.IsUser())
			{
				// Get the project ini
				MOG_Ini configFile = new MOG_Ini(mMog.GetProject().GetProjectConfigFilename());

				// Clear our target combo box
				mainForm.AssetManagerLocalDataXboxTargetComboBox.Items.Clear();

				// Add each target to the combo box
				if (configFile.SectionExist("Xboxes"))
				{
					for (int x = 0; x < configFile.CountKeys("Xboxes"); x++)
					{
						mainForm.AssetManagerLocalDataXboxTargetComboBox.Items.Add(configFile.GetKeyNameByIndex("Xboxes", x));
					}
				}

				// Check for a user defined console list
				string userPath = mMog.GetUser().GetUserToolsPath();
				string consoleIniFile = string.Concat(userPath, "\\consoles.ini");

				if (DosUtils.FileExist(consoleIniFile))
				{
					MOG_Ini userConsoleIni = new MOG_Ini(consoleIniFile);

					// Add each target to the combo box
					if (userConsoleIni.SectionExist("Xboxes"))
					{
						for (int x = 0; x < userConsoleIni.CountKeys("Xboxes"); x++)
						{
							mainForm.AssetManagerLocalDataXboxTargetComboBox.Items.Add(userConsoleIni.GetKeyNameByIndex("Xboxes", x));
						}
					}
				}
			}
		}

		public void TargetXboxSet(string machineName)
		{
			mTargetXbox = machineName;
			mainForm.AssetManagerLocalDataXboxTargetComboBox.Text = machineName;
		}

		public void TargetXboxReset()
		{
			string output = "";
			string sourceDir = mainForm.mAssetManager.GetTargetPath();
			MOG_Ini buttonDefaults = null;

			if (mMog.IsProject())
			{
				// Get the project defaults
				string projectDefaultButtonsFile = mMog.GetProject().GetProjectToolsPath() + "\\" + mMog.GetProject().GetProjectName() + ".Client.Buttons.Default.info";
				if (DosUtils.FileExist(projectDefaultButtonsFile))
				{
					buttonDefaults = new MOG_Ini(projectDefaultButtonsFile);			
				}
			}

			// Get the tool listed on the startup page
			string command = "";
			if (buttonDefaults != null)
			{
				if (buttonDefaults.SectionExist(mMog.GetProject().GetProjectName() + ".Buttons"))
				{
					if (buttonDefaults.KeyExist(mMog.GetProject().GetProjectName() + ".Buttons", "Reboot"))
					{																													
						command = buttonDefaults.GetString(mMog.GetProject().GetProjectName() + ".Buttons", "Reboot");
					}
				}
			}

			if (command.IndexOf("[ProjectPath]") != -1)
			{
				command = string.Concat(command.Substring(0, command.IndexOf("[")), mainForm.mAssetManager.GetTargetPath(), command.Substring(command.IndexOf("]")+1));
			}

			// Make sure the tool we need exits
			if (DosUtils.FileExist(command))
			{
				if (guiCommandLine.ShellExecute(command, string.Concat("/x ", mTargetXbox, " ", sourceDir), ProcessWindowStyle.Hidden, ref output) != 0)
				{
					MOG_REPORT.ShowMessageBox("XBox Tools", string.Concat(output), MessageBoxButtons.OK);
				}
			}
			else
			{
				MOG_REPORT.ShowMessageBox("XBox Tools", string.Concat("This tool is missing, have you updated to the latest version?"), MessageBoxButtons.OK);
			}
		}

		public void TargetXboxRun()
		{
			string output = "";
			string xboxExe = "";
			string Autoplay = "";
			MOG_Ini buttonDefaults = null;

			if (mMog.IsProject())
			{
				// Get the project defaults
				string projectDefaultButtonsFile = mMog.GetProject().GetProjectToolsPath() + "\\" + mMog.GetProject().GetProjectName() + ".Client.Buttons.Default.info";
				if (DosUtils.FileExist(projectDefaultButtonsFile))
				{
					buttonDefaults = new MOG_Ini(projectDefaultButtonsFile);			
				}
			}

			// Do we auto synch?
			if (mainForm.AssetManagerLocalDataXboxAutoSynchCheckBox.Checked)
			{
				// reset xbox
				TargetXboxReset();
				// Synch the xbox with the local drive first, then run the game
				TargetXboxSynch(false, true);
			}
			else
			{
				// Do we run autoPlay?
				if (mainForm.AssetManagerLocalDataXboxAutoplayCheckBox.Checked)
				{
					Autoplay = string.Concat(mainForm.AssetManagerLocalDataXboxUserMapComboBox.Text, "?Game=EonEngine.EonGameInfo -NoMenu");
				}

				// Use 'default' as user, if the Default checkbox is checked.  This is used for the programmers who debug their code using the default.xbe
				if (mainForm.AssetManagerLocalDataXboxDefaultCheckBox.Checked)
				{
					xboxExe = string.Concat("xE:\\", mMog.GetProject().GetProjectName(), "\\default.xbe", " ", Autoplay);
				}
				else
				{
					xboxExe = string.Concat("xE:\\", mMog.GetProject().GetProjectName(), ".", mMog.GetUser().GetUserName(), "\\default.xbe", " ", Autoplay);
				}

				// Check to see if there is a project defined tool for this
				string RunCommand = "";
				if (buttonDefaults != null)
				{
					if (buttonDefaults.SectionExist(mMog.GetProject().GetProjectName() + ".Buttons"))
					{
						if (buttonDefaults.KeyExist(mMog.GetProject().GetProjectName() + ".Buttons", "Run"))
						{																													
							RunCommand = buttonDefaults.GetString(mMog.GetProject().GetProjectName() + ".Buttons", "Run");
						}
					}
				}

				// Get the tool listed on the startup page
				string command = RunCommand;
				if (command.IndexOf("[ProjectPath]") != -1)
				{
					command = string.Concat(command.Substring(0, command.IndexOf("[")), mainForm.mAssetManager.GetTargetPath(), command.Substring(command.IndexOf("]")+1));
				}

				// Make sure the tool we need exits
				if (DosUtils.FileExist(command))
				{
					if (guiCommandLine.ShellExecute(command, string.Concat("/x ", mTargetXbox, " ", xboxExe), ProcessWindowStyle.Hidden, ref output) != 0)
					{
						MOG_REPORT.ShowMessageBox("XBox Tools", string.Concat(output), MessageBoxButtons.OK);
					}
				}
				else
				{
					MOG_REPORT.ShowMessageBox("XBox Tools", string.Concat("This tool is missing, have you updated to the latest version?"), MessageBoxButtons.OK);
				}
			}
		}

		public void TargetXboxCapture()
		{
			string output = "";
			MOG_Ini buttonDefaults = null;

			if (mMog.IsProject())
			{
				// Get the project defaults
				string projectDefaultButtonsFile = mMog.GetProject().GetProjectToolsPath() + "\\" + mMog.GetProject().GetProjectName() + ".Client.Buttons.Default.info";
				if (DosUtils.FileExist(projectDefaultButtonsFile))
				{
					buttonDefaults = new MOG_Ini(projectDefaultButtonsFile);			
				}
			}

			// Get the tool listed on the startup page
			string command = "";
			if (buttonDefaults != null)
			{
				if (buttonDefaults.SectionExist(mMog.GetProject().GetProjectName() + ".Buttons"))
				{
					if (buttonDefaults.KeyExist(mMog.GetProject().GetProjectName() + ".Buttons", "Capture"))
					{																													
						command = buttonDefaults.GetString(mMog.GetProject().GetProjectName() + ".Buttons", "Capture");
					}
				}
			}

			if (command.IndexOf("[ProjectPath]") != -1)
			{
				command = string.Concat(command.Substring(0, command.IndexOf("[")), mainForm.mAssetManager.GetTargetPath(), command.Substring(command.IndexOf("]")+1));
			}

			// Create a screenshot name
			string path = "";
			if (buttonDefaults != null)
			{
				if (buttonDefaults.SectionExist(mMog.GetProject().GetProjectName() + ".Buttons"))
				{
					if (buttonDefaults.KeyExist(mMog.GetProject().GetProjectName() + ".Buttons", "Run"))
					{																													
						path = buttonDefaults.GetString(mMog.GetProject().GetProjectName() + ".Buttons", "Run");
					}
				}
			}

			string screenShotName = string.Concat(path, "\\", mMog.GetProject().GetProjectName(), new MOG_Time().FormatString("{month.2}{day.2}{year.2}{hour.2}{minute.2}{second.2}"), ".bmp" );

			// Make sure the tool we need exits
			if (DosUtils.FileExist(command))
			{
				if (mTargetXbox != null && mTargetXbox.Length != 0)
				{
					if (guiCommandLine.ShellExecute(command, string.Concat("/x ", mTargetXbox, " \"", screenShotName, "\""), ProcessWindowStyle.Hidden, ref output) != 0)
					{
						MOG_REPORT.ShowMessageBox("XBox Tools", string.Concat("Screen capture was not successful. This is probably due to a disconnected Xbox or an Xbox that was not currently running an application"), MessageBoxButtons.OK);
					}
				}
				else
				{
					MOG_REPORT.ShowMessageBox("XBox Tools", string.Concat("Invalid target Xbox defined!"), MessageBoxButtons.OK);
				}
			}
			else
			{
				MOG_REPORT.ShowMessageBox("XBox Tools", string.Concat("This tool is missing, have you updated to the latest version?"), MessageBoxButtons.OK);
			}
		}

		public void TargetXboxDeepSynch()
		{
			string userName = mMog.GetUser().GetUserName();
			bool useDefault = false;

			// Use 'default' as user, if the Default checkbox is checked.  This is used for the programmers who debug their code using the default.xbe
			if (mainForm.AssetManagerLocalDataXboxDefaultCheckBox.Checked)
			{
				userName = "default";
				useDefault = true;
			}
			
			guiPlatformSinc sinc = new guiPlatformSinc(mTargetXbox, mainForm.mAssetManager.GetTargetPath(), mainForm, userName, useDefault, false, false, true);
			sinc.mProjectSyncFile = mProjectSyncFile;
			sinc.mUserSyncFile = mUserSyncFile;
			sinc.TargetConsoleRemoveSync();
		}

		public void TargetXboxSynch(bool force, bool runAfterComplete)
		{
			string userName = mMog.GetUser().GetUserName();
			bool useDefault = false;

			// Use 'default' as user, if the Default checkbox is checked.  This is used for the programmers who debug their code using the default.xbe
			if (mainForm.AssetManagerLocalDataXboxDefaultCheckBox.Checked)
			{
				userName = "default";
				useDefault = true;
			}

			// No reset for PC targets
			if (mTargetXbox.IndexOf(":") == -1)
			{
				TargetXboxReset();
			}
			
			guiPlatformSinc sinc = new guiPlatformSinc(mTargetXbox, mainForm.mAssetManager.GetTargetPath(), mainForm, userName, useDefault, force, runAfterComplete, true);
			sinc.mProjectSyncFile = mProjectSyncFile;
			sinc.mUserSyncFile = mUserSyncFile;
			
			if (mPlatformSyncThread == null)
			{
				mPlatformSyncThread = new Thread(new ThreadStart(sinc.TargetConsoleSync));
			}
			else
			{
				mPlatformSyncThread.Abort();
				mPlatformSyncThread = null;
				mPlatformSyncThread = new Thread(new ThreadStart(sinc.TargetConsoleSync));
			}

			mPlatformSyncThread.Start();
		}

		public void TargetXboxMakeIso()
		{
			// Optimize all the data
			string output = "";
			string sourceDir = mainForm.mAssetManager.GetTargetPath();

			// Get the tool listed on the startup page
			string command = "[ProjectPath]\\Mog\\Tools\\Xbox\\Optimize\\Optimize.bat";
			if (command.IndexOf("[ProjectPath]") != -1)
			{
				command = string.Concat(command.Substring(0, command.IndexOf("[")), mainForm.mAssetManager.GetTargetPath(), command.Substring(command.IndexOf("]")+1));
			}

			// Make sure the tool we need exits
			if (DosUtils.FileExist(command))
			{
				if (guiCommandLine.ShellExecute(command, mainForm.mAssetManager.GetTargetPath() + "\\System", ProcessWindowStyle.Normal, ref output) != 0)
				{					
					MOG_REPORT.ShowMessageBox("XBox Tools", string.Concat(output), MessageBoxButtons.OK);
				}

				// Sync to the target xbox or folder
				TargetXboxSynch(false, false);
			}
			else
			{
				MOG_REPORT.ShowMessageBox("XBox Tools", string.Concat("This tool is missing, have you updated to the latest version?"), MessageBoxButtons.OK);
			}			
		}

		public void TargetXboxMakeLinearLoadIso()
		{
			// Optimize all the data
			string output = "";
			string sourceDir = mainForm.mAssetManager.GetTargetPath();

			string userName = mMog.GetUser().GetUserName();
			bool useDefault = false;

			CallbackDialogForm bigProgress = new CallbackDialogForm();
			bigProgress.DialogInitialize("Make Linear Load Build", "Creating linear maps...\nThis could take many hours", "Cancel");

			// Use 'default' as user, if the Default checkbox is checked.  This is used for the programmers who debug their code using the default.xbe
			if (mainForm.AssetManagerLocalDataXboxDefaultCheckBox.Checked)
			{
				userName = "default";
				useDefault = true;
			}

			// Check for user cancel
			Application.DoEvents();
			if (bigProgress.DialogProcess())
			{
				bigProgress.DialogKill();
				return;
			}

			// Get the tool listed on the startup page
			string command = "[ProjectPath]\\Mog\\Tools\\Xbox\\Optimize\\Optimize.bat";
			if (command.IndexOf("[ProjectPath]") != -1)
			{
				command = string.Concat(command.Substring(0, command.IndexOf("[")), mainForm.mAssetManager.GetTargetPath(), command.Substring(command.IndexOf("]")+1));
			}

			bigProgress.DialogUpdate(10, "Optimizing the newly merged data");

			// Make sure the tool we need exits
			if (DosUtils.FileExist(command))
			{
				// Check for user cancel
				Application.DoEvents();
				if (bigProgress.DialogProcess())
				{
					bigProgress.DialogKill();
					return;
				}

				if (guiCommandLine.ShellExecute(command, mainForm.mAssetManager.GetTargetPath() + "\\System", ProcessWindowStyle.Normal, ref output) != 0)
				{					
					MOG_REPORT.ShowMessageBox("XBox Tools", string.Concat(output), MessageBoxButtons.OK);
				}

				// Check for user cancel
				Application.DoEvents();
				if (bigProgress.DialogProcess())
				{
					bigProgress.DialogKill();
					return;
				}

				bigProgress.DialogUpdate(20, "Syncing to Build Xbox for linear load tool execution...");

				// Sync to the Linear Build xbox
				string buildXbox = "LinearBuild";
				
				// Logon to correct xbox
				XboxUtils.SetXboxName(buildXbox, false);

				guiPlatformSinc sinc = new guiPlatformSinc(buildXbox, mainForm.mAssetManager.GetTargetPath(), mainForm, userName, useDefault, false, false, false);
				sinc.mProjectSyncFile = "xbox.opt.ls.sync";
				sinc.mUserSyncFile = "";
				sinc.TargetConsoleSync();

				// Check for user cancel
				Application.DoEvents();
				if (bigProgress.DialogProcess())
				{
					bigProgress.DialogKill();
					return;
				}

				// Run the linear load tool
				string xboxExe = "xE:\\" + mMog.GetProject().GetProjectName() + "\\defaultls.xbe";

				bigProgress.DialogUpdate(30, "Preparing for linear load tool execution...");
				
				// Kill the Linear load complete file in the root before we call the tool
				if (XboxUtils.FileExist("xQ:\\LoadComplete.sys"))
				{
					XboxUtils.FileDelete("xQ:\\LoadComplete.sys");
				}

				// Check for user cancel
				Application.DoEvents();
				if (bigProgress.DialogProcess())
				{
					bigProgress.DialogKill();
					return;
				}


				// Kill all previous linear files
				string LinearLoadFiles = XboxUtils.GetFiles("xE:\\" + mMog.GetProject().GetProjectName() + "\\System\\");
				if (LinearLoadFiles.Length != 0 && LinearLoadFiles.IndexOf(",") != -1)
				{
					CallbackDialogForm progress1 = new CallbackDialogForm();
					progress1.DialogInitialize("Make Linear Load Build", "Deleting previous linear load maps...", "Cancel");
				
					string []files = LinearLoadFiles.Split(",".ToCharArray());
					foreach (string file in files)
					{
						if (string.Compare(Path.GetExtension(file), ".lin", true) == 0)
						{
							progress1.DialogUpdate(0, file);
							XboxUtils.FileDelete(file);

							// Check for user cancel
							Application.DoEvents();
							if (bigProgress.DialogProcess())
							{
								progress1.DialogKill();
								bigProgress.DialogKill();
								return;
							}

							if (progress1.DialogProcess())
							{
								progress1.DialogKill();
								return;
							}
						}
					}

					progress1.DialogKill();
				}

				// Check for user cancel
				Application.DoEvents();
				if (bigProgress.DialogProcess())
				{
					bigProgress.DialogKill();
					return;
				}

				MOG_Ini buttonDefaults = null;
				if (mMog.IsProject())
				{
					// Get the project defaults
					string projectDefaultButtonsFile = mMog.GetProject().GetProjectToolsPath() + "\\" + mMog.GetProject().GetProjectName() + ".Client.Buttons.Default.info";
					if (DosUtils.FileExist(projectDefaultButtonsFile))
					{
						buttonDefaults = new MOG_Ini(projectDefaultButtonsFile);			
					}
				}

				// Get the tool listed on the startup page
				if (buttonDefaults != null)
				{
					if (buttonDefaults.SectionExist(mMog.GetProject().GetProjectName() + ".Buttons"))
					{
						if (buttonDefaults.KeyExist(mMog.GetProject().GetProjectName() + ".Buttons", "Run"))
						{																													
							command = buttonDefaults.GetString(mMog.GetProject().GetProjectName() + ".Buttons", "Run");
						}
					}
				}

				if (command.IndexOf("[ProjectPath]") != -1)
				{
					command = string.Concat(command.Substring(0, command.IndexOf("[")), mainForm.mAssetManager.GetTargetPath(), command.Substring(command.IndexOf("]")+1));
				}

				// Check for user cancel
				Application.DoEvents();
				if (bigProgress.DialogProcess())
				{
					bigProgress.DialogKill();
					return;
				}

				bigProgress.DialogUpdate(50, "Starting the linear load creation tool...");
				
				// Make sure the tool we need exits
				if (DosUtils.FileExist(command))
				{
					if (guiCommandLine.ShellExecute(command, string.Concat("/x ", buildXbox, " ", xboxExe), ProcessWindowStyle.Hidden, ref output) != 0)
					{
						MOG_REPORT.ShowMessageBox("XBox Tools", string.Concat(output), MessageBoxButtons.OK);
					}

					// Wait till the LinearLoad.sys file exists before continuing...
					while(XboxUtils.FileExist("Q:\\LoadComplete.sys") == false)
					{
						Application.DoEvents();
						if (bigProgress.DialogProcess())
						{
							bigProgress.DialogKill();
							return;
						}

						Thread.Sleep(500);
					}
				
				}
				else
				{
					MOG_REPORT.ShowMessageBox("XBox Tools", string.Concat("This tool is missing, have you updated to the latest version?"), MessageBoxButtons.OK);
				}

				// Kill the Linear load complete file once we are done
				if (XboxUtils.FileExist("xQ:\\LoadComplete.sys"))
				{
					XboxUtils.FileDelete("xQ:\\LoadComplete.sys");
				}

				// Check for user cancel
				Application.DoEvents();
				if (bigProgress.DialogProcess())
				{
					bigProgress.DialogKill();
					return;
				}

				bigProgress.DialogUpdate(80, "Retrieving linear load maps from Build Xbox...");
				
				// Copy off the data
				LinearLoadFiles = XboxUtils.GetFiles("xE:\\" + mMog.GetProject().GetProjectName() + "\\System\\");
				
				if (LinearLoadFiles.Length != 0 && LinearLoadFiles.IndexOf(",") != -1)
				{
					CallbackDialogForm progress = new CallbackDialogForm();
					progress.DialogInitialize("Make Linear Load Build", "Copying completed linear load maps...", "Cancel");
				
					string []files = LinearLoadFiles.Split(",".ToCharArray());
					foreach (string file in files)
					{
						if (string.Compare(Path.GetExtension(file), ".lin", true) == 0)
						{
							string target = mMog.GetActivePlatform().mPlatformTargetPath + "\\xboxdata\\maps\\" + Path.GetFileName(file);

							progress.DialogUpdate(0, target);

							// Check for user cancel
							Application.DoEvents();
							if (bigProgress.DialogProcess())
							{
								progress.DialogKill();
								bigProgress.DialogKill();
								return;
							}

							if (progress.DialogProcess())
							{
								progress.DialogKill();
								break;
							}

							if (!XboxUtils.FileGet(file, target, false))
							{
								MOG_REPORT.ShowMessageBox("Make Linear Load Build", "Error copying file(" + file + ") from build xbox!", MessageBoxButtons.OK);
							}
						}
					}

					progress.DialogKill();
				}
				else
				{
					MOG_REPORT.ShowMessageBox("Make Linear Load Build", "There were no Linear Load files found on the Xbox Build machine(" + buildXbox + ")", MessageBoxButtons.OK);
					return;
				}
				

				// Check for user cancel
				Application.DoEvents();
				if (bigProgress.DialogProcess())
				{
					bigProgress.DialogKill();
					return;
				}

				bigProgress.DialogUpdate(90, "Syncing to target Xbox...");
				
				// Sync
				sinc = new guiPlatformSinc(mTargetXbox, mainForm.mAssetManager.GetTargetPath(), mainForm, userName, useDefault, false, false, false);
				sinc.mProjectSyncFile = "xbox.opt.ll.sync";
				sinc.mUserSyncFile = "";
				sinc.TargetConsoleSync();
			}
			else
			{
				MOG_REPORT.ShowMessageBox("XBox Tools", string.Concat("This tool is missing, have you updated to the latest version?"), MessageBoxButtons.OK);
			}

			bigProgress.DialogUpdate(100, "Done!");				
			bigProgress.DialogKill();
		}

		public void TargetXboxAdd()
		{
			// Get the user console ini
			string userPath = mMog.GetUser().GetUserToolsPath();
			string consoleIniFile = string.Concat(userPath, "\\consoles.ini");

			MOG_Ini consoleIni = new MOG_Ini(consoleIniFile);
			consoleIni.PutSectionString("Xboxes", mTargetXbox);

			consoleIni.Save();
			consoleIni.Close();

			// Reset our xbox list
			InitializeXboxList();
		}

		public void TargetXboxLogView()
		{
			string output = "";
			MOG_Ini buttonDefaults = null;

			if (mMog.IsProject())
			{
				// Get the project defaults
				string projectDefaultButtonsFile = mMog.GetProject().GetProjectToolsPath() + "\\" + mMog.GetProject().GetProjectName() + ".Client.Buttons.Default.info";
				if (DosUtils.FileExist(projectDefaultButtonsFile))
				{
					buttonDefaults = new MOG_Ini(projectDefaultButtonsFile);			
				}
			}


			// Get the tool listed on the startup page
			string command = "";
			if (buttonDefaults != null)
			{
				if (buttonDefaults.SectionExist(mMog.GetProject().GetProjectName() + ".Buttons"))
				{
					if (buttonDefaults.KeyExist(mMog.GetProject().GetProjectName() + ".Buttons", "LogView"))
					{																													
						command = buttonDefaults.GetString(mMog.GetProject().GetProjectName() + ".Buttons", "LogView");
					}
				}
			}

			if (command.IndexOf("[ProjectPath]") != -1)
			{
				command = string.Concat(command.Substring(0, command.IndexOf("[")), mainForm.mAssetManager.GetTargetPath(), command.Substring(command.IndexOf("]")+1));
			}

			// Make sure the tool we need exits
			if (DosUtils.FileExist(command))
			{
				guiCommandLine.ShellSpawn(command, "", ProcessWindowStyle.Normal, ref output);				
			}
			else
			{
				MOG_REPORT.ShowMessageBox("XBox Tools", string.Concat("This tool is missing, have you updated to the latest version?"), MessageBoxButtons.OK);
			}
		}

		#endregion
		#region PC tools
		public void PcLaunchDeep(string optionalTarget)
		{
			string output = "";
			// Get the tool listed on the startup page
			string command = "";//;mParent.mainForm.StartupDeepTextBox.Text;
			if (command.IndexOf("[ProjectPath]") != -1)
			{
				command = string.Concat(command.Substring(0, command.IndexOf("[")), mainForm.mAssetManager.GetTargetPath(), command.Substring(command.IndexOf("]")+1));
			}

			string target = optionalTarget;

			// Make sure the tool we need exits
			if (DosUtils.FileExist(command))
			{
				if (optionalTarget.Length != 0)
				{
					guiCommandLine.ShellSpawn(command, target, ProcessWindowStyle.Normal, ref output);
				}
				else
				{
					guiCommandLine.ShellSpawn(command);			
				}
			}
			else
			{
				MOG_REPORT.ShowMessageBox("PC Tools", string.Concat("This tool is missing, have you updated to the latest version?"), MessageBoxButtons.OK);
			}
		}

		public void SaveUserMap(string mapName)
		{
			mainForm.mUserPrefs.SaveProjectData();
			switch(mMog.GetActivePlatform().mPlatformName.ToLower())
			{
				case "xbox":
					if (!mainForm.AssetManagerLocalDataXboxUserMapComboBox.Items.Contains((object)mapName))
					{
						mainForm.AssetManagerLocalDataXboxUserMapComboBox.Items.Add(mapName);
					}
					break;
				case "pc":
					if (!mainForm.AssetManagerLocalDataPcUserMapComboBox.Items.Contains((object)mapName))
					{
						mainForm.AssetManagerLocalDataPcUserMapComboBox.Items.Add(mapName);
					}
					break;
			}
		}

		public string GetUserMap()
		{
			switch(mMog.GetActivePlatform().mPlatformName.ToLower())
			{
				case "xbox":
					return mainForm.AssetManagerLocalDataXboxUserMapComboBox.Text;
				case "pc":
					return mainForm.AssetManagerLocalDataPcUserMapComboBox.Text;
			}

			return "";
		}

		public void BrowseUserMap(ComboBox target)
		{
			// Setup the FileDialog box
			mainForm.MOGOpenFileDialog.Reset();
			mainForm.MOGOpenFileDialog.DefaultExt = ".adv";
			mainForm.MOGOpenFileDialog.Filter = "Advent Maps (*.adv)|*.adv";
			mainForm.MOGOpenFileDialog.Multiselect = false;
			mainForm.MOGOpenFileDialog.Title = "Select map";
			//mParent.mainForm.MOGOpenFileDialog.RestoreDirectory = true;

			string startingMap = GetUserMap();
			string startingMapDir;
			if (startingMap.LastIndexOf("\\") != -1)
			{
				startingMapDir = startingMap.Substring(0, startingMap.LastIndexOf("\\"));
			}
			else
			{
				startingMapDir = "";
			}

			// Set the folderBrowser to the current target path
			string start = string.Concat(mainForm.mAssetManager.GetTargetPath(), "\\Maps\\", startingMapDir);
			if (start.Length != 0)
			{				
				mainForm.MOGOpenFileDialog.InitialDirectory = start;
			}

			// show the window
			if (mainForm.MOGOpenFileDialog.ShowDialog(mainForm) == DialogResult.OK)
			{
				string newmap = mainForm.MOGOpenFileDialog.FileName.ToLower();
				string sourceDir = mainForm.mAssetManager.GetTargetPath().ToLower();
				target.Text = newmap.Replace(string.Concat(sourceDir, "\\maps\\"), "");
			}		
		}

		#endregion
	}
}
