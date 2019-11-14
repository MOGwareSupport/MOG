using System;
using System.Windows.Forms;
using System.Collections;
using System.Diagnostics;
using System.IO;

using MOG.DOSUTILS;
using MOG.INI;
using MOG.REPORT;

using MOG_Client.Forms;
using MOG_Client.Client_Utilities;
using MOG_Client.Client_Mog_Utilities;

namespace MOG_Client.Client_Gui.AssetManager_Helper
{
	class guiPlatformButtonHandles
	{
		public string mPlatform;
		public ArrayList mButtonHandles;			// Array of button handles

		public guiPlatformButtonHandles(string platform)
		{
			mPlatform = platform;
			mButtonHandles = new ArrayList();
		}

		public void Add(Button button)
		{
			mButtonHandles.Add(button);
		}
	}

	/// <summary>
	/// Custom buttons are a series of buttons created in advance that the user can configure either on a project level
	/// or a user level.  The project buttons are stored in [ProjectName].Client.Buttons.Default.info
	/// </summary>
	public class guiAssetManagerCustomButtons
	{
		private MogMainForm mainForm;				// Pointer to the main form
		private ArrayList PlatformButtonHandles;	// Container of button arrays
		
		public guiAssetManagerCustomButtons(MogMainForm main)
		{
			mainForm = main;
			PlatformButtonHandles = new ArrayList();		
		}

		/// <summary>
		/// Main entry point for creating a custom button correlation to a platform and a Button control
		/// </summary>
		/// <param name="platform"></param>
		/// <param name="button"></param>
		public void AddButtonHandle(string platform, Button button)
		{
			foreach (guiPlatformButtonHandles platformButton in PlatformButtonHandles)
			{
				// Is this our platform that we want to add a button handle to?
				if (string.Compare(platform, platformButton.mPlatform, true) == 0)
				{
					platformButton.Add(button);
					return;
				}
			}

			// Looks like we did not find it.  Now create a new handler
			guiPlatformButtonHandles buttonHandle = new guiPlatformButtonHandles(platform);
			buttonHandle.Add(button);
			PlatformButtonHandles.Add(buttonHandle);
			return;
		}

		#region Load / Save routines
		/// <summary>
		/// Load the project defined custom buttons list
		/// </summary>
		public void Load()
		{
			if (mainForm.gMog.IsProject())
			{
				// Get the project defaults
				string projectDefaultButtonsFile = mainForm.gMog.GetProject().GetProjectToolsPath() + "\\" + mainForm.gMog.GetProject().GetProjectName() + ".Client.Buttons.Default.info";
				if (DosUtils.FileExist(projectDefaultButtonsFile))
				{
					MOG_Ini defaults = new MOG_Ini(projectDefaultButtonsFile);			
					LoadButtons(defaults);
				}

				// Load the custom button configs
				LoadButtons(guiUserPrefs.ini);
			}
		}

		private void LoadButtons(MOG_Ini config)
		{
			foreach (guiPlatformButtonHandles platformButton in PlatformButtonHandles)
			{
				string projectName = mainForm.gMog.GetProject().GetProjectName();
				string platformButtons = projectName + "." + platformButton.mPlatform + ".Buttons";
				
				if (config.SectionExist(platformButtons))
				{
					// Lets attempt to set up each button handle found in the handles array
					for (int k = 0; k < platformButton.mButtonHandles.Count; k++)
					{
						string buttonKey = "Button" + k.ToString();
						if (config.KeyExist(platformButtons, buttonKey))
						{
							string buttonName = "";
							string buttonTool = "";

							// Split the value of this key by the :
							string []parts = config.GetString(platformButtons, buttonKey).Split("@".ToCharArray());
							if (parts.Length >=2)
							{
								buttonName = parts[0];
								buttonTool = parts[1];
							}
							else if (parts.Length == 1)
							{
								buttonName = parts[0];
							}
						
							// Now assign the name and tool if it was defined
							if (buttonName.Length != 0 && buttonTool.Length != 0)
							{
								((Button)platformButton.mButtonHandles[k]).Text = buttonName;
								((Button)platformButton.mButtonHandles[k]).Tag = buttonTool;
								((Button)platformButton.mButtonHandles[k]).Visible = true;
							}
								// Buttons with a name but no tool specified are to be set to inVisible
							else if (buttonName.Length != 0)
							{
								((Button)platformButton.mButtonHandles[k]).Text = buttonName;
								((Button)platformButton.mButtonHandles[k]).Tag = "";
								((Button)platformButton.mButtonHandles[k]).Visible = false;
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Save any custom user defined buttons that have been altered or added to the custom buttons array
		/// </summary>
		public void Save()
		{
			MOG_Ini defaults = null;
			string projectName = mainForm.gMog.GetProject().GetProjectName();
			
			string projectDefaultButtonsFile = mainForm.gMog.GetProject().GetProjectToolsPath() + "\\" + mainForm.gMog.GetProject().GetProjectName() + ".Client.Buttons.Default.info";
			if (DosUtils.FileExist(projectDefaultButtonsFile))
			{
				defaults = new MOG_Ini(projectDefaultButtonsFile);
			}
			
			foreach (guiPlatformButtonHandles platformButton in PlatformButtonHandles)
			{
				int count = 0;
				string platformButtons = projectName + "." + platformButton.mPlatform + ".Buttons";

				foreach (Button button in platformButton.mButtonHandles)
				{
					string buttonString = button.Text + "@" + (string)button.Tag;

					// Does this button exist in the defaults and is it different?
					if (!CheckButtonMatch(buttonString, platformButton.mPlatform, defaults))
					{
						// Save this in our local prefs
						guiUserPrefs.Save(platformButtons, "Button" + count, buttonString);
					}
					else
					{
						// If we match, make sure we clear any referance in the users pref file
						guiUserPrefs.ini.RemoveString(platformButtons, "Button" + count);
					}

					count++;
				}
			}
		}

		/// <summary>
		/// Does this button exist in the project defined defaults and is it different?
		/// </summary>
		/// <param name="str"></param>
		/// <param name="platform"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		private bool CheckButtonMatch(string str, string platform, MOG_Ini config)
		{
			string projectName = mainForm.gMog.GetProject().GetProjectName();
			string platformButtons = projectName + "." + platform + ".Buttons";

			if (config.SectionExist(platformButtons))
			{
				// Lets attempt to set up each button handle found in the handles array
				for (int k = 0; k < config.CountKeys(platformButtons); k++)
				{
					string buttonKey = "Button" + k.ToString();
					if (config.KeyExist(platformButtons, buttonKey))
					{
						// Get the command string
						string buttonDefault = config.GetString(platformButtons, buttonKey);

						// Now assign the name and tool if it was defined
						if (string.Compare(buttonDefault, str, true) == 0)
						{
							return true;
						}
					}				
				}
			}

			return false;
		}

		#endregion
		#region Button custom action context menu handles
		public void Add()
		{
			// Find the first non-visible button in the valid buttons array and make it visible
			foreach (guiPlatformButtonHandles platformButton in PlatformButtonHandles)
			{
				if (string.Compare(platformButton.mPlatform, mainForm.gMog.GetActivePlatform().mPlatformName, true) == 0)
				{
					foreach (Button button in platformButton.mButtonHandles)
					{
						if (button.Visible == false)
						{
							button.Visible = true;
							Save();
							return;
						}
					}
				}
			}

			MOG_REPORT.ShowMessageBox("Add Custom tool button", "There are no more buttons available", MessageBoxButtons.OK);
		}

		public void Remove(Button removeButton)
		{
			// Find the first non-visible button in the valid buttons array and make it visible
			foreach (guiPlatformButtonHandles platformButton in PlatformButtonHandles)
			{
				if (string.Compare(platformButton.mPlatform, mainForm.gMog.GetActivePlatform().mPlatformName, true) == 0)
				{
					foreach (Button button in platformButton.mButtonHandles)
					{
						if (button == removeButton)
						{
							button.Text = "n/a";
							button.Tag = null;
							button.Visible = false;
							Save();
							return;
						}						
					}
				}
			}

			MOG_REPORT.ShowMessageBox("Remove Custom tool button", "We were able to remove this button", MessageBoxButtons.OK);
		}

		/// <summary>
		/// Reset the button that was clicked on to the projects default state
		/// </summary>
		/// <param name="resetButton"></param>
		public void Reset(Button resetButton)
		{			
			// Get the project defaults
			string projectDefaultButtonsFile = mainForm.gMog.GetProject().GetProjectToolsPath() + "\\" + mainForm.gMog.GetProject().GetProjectName() + ".Client.Buttons.Default.info";
			if (DosUtils.FileExist(projectDefaultButtonsFile))
			{
				MOG_Ini defaults = new MOG_Ini(projectDefaultButtonsFile);

				string projectName = mainForm.gMog.GetProject().GetProjectName();
				string platformButtons = projectName + "." + mainForm.gMog.GetActivePlatform().mPlatformName + ".Buttons";

				string buttonKey = "Button" + GetButtonPrefsString(resetButton);
								
				if (defaults.KeyExist(platformButtons, buttonKey))
				{
					string buttonName = "";
					string buttonTool = "";

					// Split the value of this key by the @ sign
					string []parts = defaults.GetString(platformButtons, buttonKey).Split("@".ToCharArray());
					if (parts.Length >=2)
					{
						buttonName = parts[0];
						buttonTool = parts[1];
					}
					else if (parts.Length == 1)
					{
						buttonName = parts[0];
					}
						
					// Now assign the name and tool if it was defined
					if (buttonName.Length != 0)
					{
						resetButton.Text = buttonName;
						resetButton.Tag = buttonTool;
						Save();
					}
				}
			}
		}

		/// <summary>
		/// Lauch the desired command associated with this button
		/// </summary>
		/// <param name="button"></param>
		public void Click(Button button)
		{
			// Early out if there is no command specified
			if (button.Tag == null)
			{
				return;
			}

			string commandSource = ((string)button.Tag).ToLower();
			string output = "";
			string command = "";
			string arguments = "";

			// Make sure this is not a system command
			if (string.Compare(commandSource, "system", true) == 0)
			{
				// Determine if this command is within our system defined keywords
				switch(button.Text.ToLower())
				{
					case "sync":
						// Launch Sync code
						mainForm.mAssetManager.mTools.TargetXboxSynch(false, false);
						break;
					case "run":
						if (string.Compare(mainForm.gMog.GetActivePlatform().mPlatformName, "xbox", true) == 0)
						{
							mainForm.mAssetManager.mTools.TargetXboxRun();
						}
						else
						{
							MOG_REPORT.ShowMessageBox("Launch Tool", "System command for button:" + button.Text + " not found!", MessageBoxButtons.OK);
							return;
						}
						break;
					case "sync remove":
						mainForm.mAssetManager.mTools.TargetXboxDeepSynch();
						break;			
					case "screenshot":
						// Launch Sync code
						mainForm.mAssetManager.mTools.TargetXboxCapture();
						break;
					case "make iso":
						mainForm.mAssetManager.mTools.TargetXboxMakeIso();
						break;
					case "full make iso":
						mainForm.mAssetManager.BuildMerge(true);
						mainForm.mAssetManager.mTools.TargetXboxMakeIso();
						break;
					case "make linear iso":
						mainForm.mAssetManager.BuildMerge(true);
						mainForm.mAssetManager.mTools.TargetXboxMakeLinearLoadIso();
						break;
					default:
						MOG_REPORT.ShowMessageBox("Launch Tool", "System command for button:" + button.Text + " not found!", MessageBoxButtons.OK);
						return;
				}
			}
			else
			{
				// Break the command into two strings.  One for the exe and the other for arguments
				CreateCommandAndArguments(commandSource, ref command, ref arguments);
				
				// Make sure the exe exists
				if (DosUtils.FileExist(command))
				{
					// Spawn the app or tool
					if (guiCommandLine.ShellSpawn(command, arguments, ProcessWindowStyle.Normal, ref output) != 0)
					{
						// Report an error if the tool did not return 0
//						MOG_REPORT.ShowMessageBox("Launch Exe", string.Concat(output), MessageBoxButtons.OK);
					}
				}
				else
				{
					MOG_REPORT.ShowMessageBox("Launch Exe", string.Concat("File: ", command, ", does not exist"), MessageBoxButtons.OK);
				}
			}
		}

		/// <summary>
		/// Launch the edit button dialog for defining the name and target of a custom button
		/// </summary>
		/// <param name="menu"></param>
		public void EditButton(ContextMenu menu)
		{
			// Determine which button was clicked on
			string buttonName = menu.SourceControl.Text;

			// Setup the edit form
			EditCustomButtonForm editButtonForm = new EditCustomButtonForm();
			editButtonForm.EditButtonNameTextBox.Text = buttonName;
			editButtonForm.EditToolExeTextBox.Text = menu.SourceControl.Tag != null ? (string)menu.SourceControl.Tag : "n/a";

			// Launch the form
			if (editButtonForm.ShowDialog() == DialogResult.OK)
			{
				// If ok, set the arrays
				menu.SourceControl.Text = editButtonForm.EditButtonNameTextBox.Text;
				menu.SourceControl.Tag = editButtonForm.EditToolExeTextBox.Text;
				Save();
			}
		}

		#endregion
		#region Command parsing routines

		/// <summary>
		/// Parse the selected string to locate the runnable executable and return it
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		private string GetCommandName(string source)
		{
			// Get the index of the last \ before the XXXXXX.XXX
			int DirSeparator = 0;
			int ExtSeparator = 0;
			for (int i = 0; i < source.Length; i++)
			{				
				if( source[i] == '\\' )
				{
					DirSeparator = i;
				}

				if ( source[i] == '.' )
				{
					// Check to see if this . has a 3 letter extension
					if ( i + 3 <= source.Length)
					{
						// Check if there is also a space after the extension for an argument
						if (i + 4 <= source.Length && source.Length >  i + 4 && source[i + 4] == ' ')
						{
							ExtSeparator = i;

							// This is it!
							i = source.Length;
							break;
						}
					}
				}
			}

			int last = DirSeparator;
			
			// Check to see if the multiple directory spacers and that that index is less than the overall length of the command source
			if (last != -1 && source.Length > last)
			{
				// Get a substring of all thats left in the command
				string temp = source.Substring(last);

				// Now lets look for an extension followed by a space.  If found, we have some arguments that need to be removed
				int extensionIndex = temp.IndexOf(".");
				if ( extensionIndex != -1 )
				{
					int spaceIndex = temp.IndexOf(" ", extensionIndex);
					if (spaceIndex != -1 && extensionIndex + 4 == spaceIndex)
					{
						return source.Substring(0, last + spaceIndex);
					}
					else
					{
						// Nope, no arguments
						return source;
					}
				}
				else
				{
					// Nope, no arguments
					return source;
				}
			}
			else
			{
				// If that last seperator was the last one, return the command up to that point
				if (last != -1)
				{
					return source.Substring(0, last);
				}
			}

			return "";
		}

		/// <summary>
		/// Seperate the command from the arguments within the source string, replace any tokens with real data and return them
		/// </summary>
		/// <param name="source"></param>
		/// <param name="command"></param>
		/// <param name="arguments"></param>
		private void CreateCommandAndArguments(string source, ref string command, ref string arguments)
		{
			command = GetCommandName(source);
			arguments = command.Length == source.Length ? "" : source.Substring(command.Length + 1);

			// Launch the command specified by the tag
			string token = "[projectpath]";
			if (command.IndexOf(token) != -1)
			{
				command = string.Concat(command.Substring(0, command.IndexOf("[")), mainForm.gMog.GetActivePlatform().mPlatformTargetPath, command.Substring(command.IndexOf("]")+1));
			}

			// Launch the command specified by the tag
			token = "[projectpath]";
			if (arguments.IndexOf("[projectpath]") != -1)
			{
				arguments = arguments.Replace("[projectpath]", mainForm.gMog.GetActivePlatform().mPlatformTargetPath);
			}

			// Launch the command specified by the tag
			token = "[projectsystempath]";
			if (arguments.IndexOf("[projectsystempath]") != -1)
			{
				arguments = arguments.Replace("[projectsystempath]", mainForm.gMog.GetProject().GetProjectPath());
			}

			// Launch the command specified by the tag
			token = "[projectsystemtoolspath]";
			if (arguments.IndexOf("[projectsystemtoolspath]") != -1)
			{
				arguments = arguments.Replace("[projectsystemtoolspath]", mainForm.gMog.GetProject().GetProjectToolsPath());
			}

			// Launch the command specified by the tag
			token = "[projectsystempath]";
			if (command.IndexOf("[projectsystempath]") != -1)
			{
				command = command.Replace("[projectsystempath]", mainForm.gMog.GetProject().GetProjectPath());
			}

			// Launch the command specified by the tag
			token = "[projectsystemtoolspath]";
			if (command.IndexOf("[projectsystemtoolspath]") != -1)
			{
				command = command.Replace("[projectsystemtoolspath]", mainForm.gMog.GetProject().GetProjectToolsPath());
			}

			// HACK special case user map arg
			token = "[usermap]";
			if (arguments.IndexOf(token) != -1)
			{
				string map = "";
				if (string.Compare(mainForm.gMog.GetActivePlatform().mPlatformName, "xbox", true) ==0)
				{
					map = (mainForm.AssetManagerLocalDataXboxAutoplayCheckBox.Checked) ? mainForm.AssetManagerLocalDataXboxUserMapComboBox.Text : "";
				}
				else if (string.Compare(mainForm.gMog.GetActivePlatform().mPlatformName, "pc", true) ==0)
				{
					map = (mainForm.AssetManagerLocalDataPcAutoplayCheckBox.Checked) ? mainForm.AssetManagerLocalDataPcUserMapComboBox.Text : "";
				}				
				arguments = arguments.Replace(token, map);
			}

			// HACK special case user map arg
			token = "[targetxbox]";
			if (arguments.IndexOf(token) != -1)
			{
				string target = mainForm.mAssetManager.mTools.GetTargetXbox();
				arguments = arguments.Replace(token, target);
			}

			// HACK special case user map arg
			token = "[gamedirectory]";
			if (arguments.IndexOf(token) != -1)
			{
				string target = mainForm.gMog.GetActivePlatform().mPlatformTargetPath;
				arguments = arguments.Replace(token, target);
			}
		}

		#endregion
		#region Utility functions
		/// <summary>
		/// Get the button name of the selected button and return its button index
		/// All buttons are currently named like {button0, button1, button2, ...)
		/// </summary>
		/// <param name="button"></param>
		/// <returns></returns>
		private string GetButtonPrefsString(Button button)
		{
			// Determine if this button is one of the predefined configurable buttons by searching for an index at the end of its name
			int start = button.Name.IndexOfAny("123456789".ToCharArray());
			if (start != -1)
			{
				// Yep, this is a customizable button
				// Get its index
				string index = button.Name.Substring(start, 1);
				if (index != null && index.Length != 0)
				{
					return index;
				}
			}

			return "";
		}
		#endregion
	}
}

