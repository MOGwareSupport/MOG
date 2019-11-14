using System;
using System.IO;
using System.Threading;
using System.Collections;
using System.Windows.Forms;

using MOG;
using MOG.INI;
using MOG.REPORT;
using MOG.PROMPT;
using MOG.DOSUTILS;
using MOG.COMMAND;
using MOG.FILENAME;
using MOG.PROJECT;
using MOG.PROPERTIES;
using MOG.PLATFORM;
using MOG.DATABASE;
using MOG.COMMAND.COMMANDLINE;
using MOG.CONTROLLER;
using MOG.CONTROLLER.CONTROLLERASSET;
using MOG.CONTROLLER.CONTROLLERSYNCDATA;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.CONTROLLER.CONTROLLERINBOX;
using MOG.CONTROLLER.CONTROLLERREPOSITORY;
using MOG.CONTROLLER.CONTROLLERLIBRARY;
using MOG.ENVIRONMENT_VARS;

using MOG_ControlsLibrary.Forms.Wizards;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MOG_ControlsLibrary.Forms;
using MOG_ControlsLibrary.MogUtils_Settings;
using MOG.CONTROLLER.CONTROLLERRIPPER;
using MOG_ControlsLibrary;
using System.Diagnostics;
using MOG.CONTROLLER.CONTROLLERPACKAGE;
using MOG_CoreControls;

namespace MOG_CommandLine
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>import
	class CommandLine
	{
		private string mConfigFilename;
		private string mAssetClassification;
		private string mOptionalFile;
		private ArrayList mArgs;
		enum CommandType { GOODCOMMAND, BADCOMMAND, BADHELPCOMMAND, HELPCOMMAND, NOCOMMAND };
		private CommandType mFoundCommand;
		private MOG_Command command;
		private bool mCompleted, mQuiet, mRequireLogin = false, mDrill = false;
		private bool mPause = false;
		private ArrayList mProperties;
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static int Main(string[] args)
		{
			//Create a new CommandLine object.
			CommandLine cl = new CommandLine();
			
			try
			{
				MogUtils_Settings.CreateSettings(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "CommandLinePrefs.ini"));
				//Create a variable to determine if the system can init.
				bool rc;
				//set the default found command to be a good command
				cl.mFoundCommand = CommandType.GOODCOMMAND;
				//Check to see if InitMogSystem is successful. (initilize mArgs mConfigFileName and mAssetClassification)
				rc = cl.InitMogSystem(args);
				//If we fail to init End.
				if (!rc)
				{
					return 1;
				}

				//If Not see if we can parse the command Line Text.
				rc = cl.ParseCommandLine();
				if (rc==false)
				{
					return 1;
				}
			}
			catch
			{
			}
			finally
			{
				// Check if we initialized our command manager?
				if (MOG_ControllerSystem.IsCommandManager())
				{
					// Gracefully inform the server we are shutting down
					MOG_Command shutdown = MOG_CommandFactory.Setup_ShutdownCommandLine();
					MOG_ControllerSystem.GetCommandManager().SendToServer(shutdown);

					// Keep looping until we have gracefully shutdown
					DateTime start = new DateTime();
					TimeSpan duration = new TimeSpan();
					while (duration.Seconds < 3)
					{
						// Listening for the server to properly shut us down
						MOG_Main.Process();

						// Check if we have already shutdown?
						if (MOG_Main.IsShutdown())
						{
							break;
						}

						// Make sure to gracefully wait during this process loop
						Thread.Sleep(10);
						duration = DateTime.Now - start;
					}
				}

				// Make sure we always properly shutdown
				MOG_Main.Shutdown();

				if (cl.mPause)
				{
					Console.ForegroundColor = ConsoleColor.Yellow;
					Console.WriteLine("Press any key to exit...");
					Console.ReadKey(true);
				}
			}

			return 0;
		}
		/// <summary>
		/// Used to initilize mArgs mConfigFileName and mAssetClassification
		/// </summary>
		/// <param name="args">a string array (of args passed from the command line)</param>
		/// <returns>True if everything inits properly False if not.</returns>
		public bool InitMogSystem(string[] args)
		{
			// Default into SLAVE mode
			mConfigFilename = "";
			mAssetClassification = "";

			// Initialize our commandline args
			mArgs = new ArrayList();
			//Move the args in the passed list in to the mArgs Array List.
			foreach(string str in args)
			{
				mArgs.Add(str);
			}

			// Parse command line options
			if (args.Length > 0)
			{	
				foreach(String arg in args)
				{
					//JD If the arg contains the text mog= then set the mConfigFilename = to the apropriate Text for the config file.
					if (arg.IndexOf("mog=") != -1)
					{
						mConfigFilename = arg.Substring(arg.IndexOf("=")+1);
					}
				}

			}
			//JD If you make it here everything was successful.
			return true;
		}
		/// <summary>
		/// Used to check to see if a specific Parameter was passed to the arg list.
		/// </summary>
		/// <param name="list">List of Arguments to Check.</param>
		/// <param name="option">Parameter to Look For</param>
		/// <returns>The Index from the Array list which contains the specified parameter</returns>
		private int CheckOption( ArrayList list, string option)
		{
			//create and initilize return value.
			int index = 0;
			foreach( string arg in list)
			{
				//convert the option to lower case.
				string LowerOption = option.ToLower();
				//check the current string to see if the text for the option is contained in the 
				if( string.Compare(arg.ToLower(), LowerOption, true) == 0)
				{
					//Verify that the index is not an empty string.
					if (((string)list[index]).Length > 0)
					{
						//If the option is found in the current arg return the index of the arg.
						return index;
					}
				}
				else
				{
					//If the item is not found incrament the index.
					index++;
				}
			}
			// return -1 if the value is not found in the arg list.
			return -1;
		}

		private void RemoveOption( ArrayList list, string option)
		{
			int index = 0;
			foreach( string arg in list)
			{
				string LowerOption = option.ToLower();
				if( string.Compare(arg.ToLower(), LowerOption, true) == 0)
				{
					if (((string)list[index]).Length > 0)
					{
						list.RemoveAt(index);
						return;
					}
				}
				else
				{
					index++;
				}
			}
		}

		private bool CheckOut(MOG_Filename assetFullName, string comment)
		{
			bool bComplete = false;

			try
			{
				// Wait until we have finished establishing our connection with the server
				MOG_ControllerSystem.RequireInitializationCredentials(true, true, true);

				MOG_ControllerLibrary.SetWorkingDirectory(@"C:\MOG_Library");

				bComplete = MOG_ControllerLibrary.CheckOut(assetFullName, comment);
			}
			catch (Exception e)
			{
				if (e.Message != "User canceled")
				{
					MOG_Report.ReportMessage("CheckOut", e.Message, e.StackTrace, MOG_ALERT_LEVEL.CRITICAL);
				}
			}

			return bComplete;
		}

		private bool CheckIn(MOG_Filename assetFullName, string comment)
		{
			bool bComplete = false;

			try
			{
				// Wait until we have finished establishing our connection with the server
				MOG_ControllerSystem.RequireInitializationCredentials(true, true, true);

				MOG_ControllerLibrary.SetWorkingDirectory(@"C:\MOG_Library");

				bComplete = MOG_ControllerLibrary.CheckIn(assetFullName, comment, false, "");
			}
			catch (Exception e)
			{
				if (e.Message != "User canceled")
				{
					MOG_Report.ReportMessage("CheckIn", e.Message, e.StackTrace, MOG_ALERT_LEVEL.CRITICAL);
				}
			}

			return bComplete;
		}
		
		private bool Import(MOG_Filename assetFullName, string importFile, bool bMove)
        {
            bool bComplete = false;
            string assetRootPath = "";
            ArrayList importFiles = null;
            ArrayList logFiles = null;
            DialogResult result = DialogResult.Ignore;

			try
            {
                // Wait until we have finished establishing our connection with the server
                MOG_ControllerSystem.RequireInitializationCredentials(true, true, false);

				// Check if there are any special properties being sent in with this asset?
				if (mProperties.Count > 0)
				{
					// Fix up the specified properties with syntactically correct values
					mProperties = MOG_ControlsLibrary.Utils.PropertyHelper.RepairPropertyList(mProperties);
					if (mProperties == null || mProperties.Count == 0)
					{
						return false;
					}
				}

                // Just attempt to import the single specified file
                importFiles = new ArrayList();
				if (importFile.IndexOf("*") != -1 && importFile.IndexOf("|") == -1)
                {
					// Check if this file is missing a drive root?
					if (!Path.IsPathRooted(importFile))
					{
						// Stick on the current directory in front of what the user specified
						importFile = string.Concat(DosUtils.CurrentDirectory(), "\\", importFile);
					}
					// Build the list of files
					BuildImportFileList(importFiles, importFile, mDrill);
					// Establish the assetRootPath
					assetRootPath = Path.GetDirectoryName(importFile);
                }
                else if (importFile.IndexOf("|") != -1)
                {
                    // Milti file import
                    string[] parts = importFile.Split("|".ToCharArray());
                    if (parts != null)
                    {
                        foreach (string file in parts)
                        {
							// Check if this file is missing a drive root?
							string tempFile = file;
							if (!Path.IsPathRooted(tempFile))
							{
								// Stick on the current directory in front of what the user specified
								tempFile = string.Concat(DosUtils.CurrentDirectory(), "\\", tempFile);
							}

							BuildImportFileList(importFiles, tempFile, mDrill);
                        }
                    }
                    // Detect the scope of this asset's directory structure for the importFiles
                    assetRootPath = MOG_ControllerAsset.GetCommonDirectoryPath("", importFiles);
                }
                else
                {
					// Check if this file is missing a drive root?
					if (!Path.IsPathRooted(importFile))
					{
						// Stick on the current directory in front of what the user specified
						importFile = string.Concat(DosUtils.CurrentDirectory(), "\\", importFile);
					}

					// Single file import
                    importFiles.Add(importFile);
                }

                // Check if we found any files to import?
                if (importFiles != null && importFiles.Count > 0)
                {
					// Make sure we have the adam object on the filename now so projects don't have to include the name of their project in all of their commandline scripts
		            MOG_Filename proposedAssetName = MOG_Filename.AppendAdamObjectNameOnAssetName(assetFullName);
					ArrayList detectedAssetNames = new ArrayList();
					MOG_Filename detectedAssetName = null;

					// Establish the asset's platform
                    string platformName = assetFullName.GetAssetPlatform();
                    if (platformName.Length == 0)
                        platformName = "All";

                    // Check if this isn't a valid asset name?
                    if (proposedAssetName.GetFilenameType() != MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
                    {
                        // Attempt to detect this asset name?
						detectedAssetNames = MOG_ControllerProject.MapFilenamesToAssetNames(importFiles, platformName, null);
                        if (detectedAssetNames != null && detectedAssetNames.Count == 1)
                        {
                            // Use the newly detected name
                            detectedAssetName = (MOG_Filename)detectedAssetNames[0];							
                        }
					}
					else
					{
						// Locate the best matching asset for the specified assetFullName
						MOG_Filename inboxFilename = MOG_ControllerInbox.LocateBestMatchingAsset(proposedAssetName);
						if (inboxFilename != null)
						{
							// Use this This must be a new asset so just take what we have been given
							detectedAssetName = inboxFilename;
						}
					}

					// Check if we are missing a dedected asset name?
					if (detectedAssetName == null)
					{
                        if (mQuiet)
                        {
                            string classification = assetFullName.GetAssetClassification();
                            if (classification.Length == 0)
                                classification = MOG_ControllerProject.GetProjectName();

                            string assetLabel = assetFullName.GetAssetLabel();
                            if (assetLabel.Length == 0)
                                assetLabel = assetFullName.GetFilename();
															
                            detectedAssetName = MOG_Filename.CreateAssetName(classification, platformName, assetLabel);
                        }
                        else
                        {
                            // Prepare for import
                            string projectName = MOG_ControllerProject.GetProjectName();
                            string branchName = MOG_ControllerProject.GetBranchName();
                            DialogResult rc = DialogResult.Cancel;

							// Has the user told us to only start with the MOG asset Imorter
							string StartupMode = MogUtils_Settings.LoadSetting("ImportWizard", "StartupMode");
							if (StartupMode != "Advanced" && importFiles.Count == 1)
                            {
								// Try running the import wizard first
                                bComplete = ImportWithWizard(importFiles, detectedAssetNames, proposedAssetName, out result);

                                // If the asset imported and the user did not opt to use the MOG asset importer
                                if (bComplete == true && result != DialogResult.Ignore)
                                {
                                    // Return done!
                                    return bComplete;
                                }
                            }

							// Show user import form
							ImportAssetTreeForm importForm = new ImportAssetTreeForm(new ImportFile(proposedAssetName.GetAssetLabel()));
							importForm.Seed_AssetClassification = proposedAssetName.GetAssetClassification();
							importForm.Seed_AssetPlatform = proposedAssetName.GetAssetPlatform();
							importForm.Seed_AssetLabel = proposedAssetName.GetAssetLabel();
							importForm.Seed_AssetProperties = mProperties;

                            try
                            {
								importForm.StartPosition = FormStartPosition.CenterScreen;
                                rc = importForm.ShowDialog();
                            }
                            catch (Exception ex)
                            {
                                // Since CommandLine is silent, but we're already showing a dialog, show the user what went wrong
                                MOG_Prompt.PromptMessage("Error with Import Dialog", ex.Message, ex.StackTrace, MOG_ALERT_LEVEL.ERROR);
                                throw ex;
                            }
                            
                            // Check dialog results
                            // This means we canceled this one asset
                            if (rc == DialogResult.Cancel)
                            {
                                throw new Exception("User canceled");
                            }

                            // Construct the new name based on the dialog fields
                            string newName = importForm.GetFixedAssetName();

                            mProperties.AddRange(importForm.MOGPropertyArray);
                            detectedAssetName = new MOG_Filename(newName);
                        }
                    }

					// Check if we have a detected asset name for immediate import?
					if (detectedAssetName != null &&
						detectedAssetName.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
					{
						// Proceed to import the specified file(s)
						if (MOG_ControllerAsset.CreateAsset(detectedAssetName.GetAssetFullName(), assetRootPath, importFiles, logFiles, mProperties, bMove, bMove) != null)
						{
							bComplete = true;
						}
					}
                }
            }

            catch (Exception e)
            {
				if (e.Message != "User canceled")
				{
					MOG_Report.ReportMessage("Import", e.Message, e.StackTrace, MOG_ALERT_LEVEL.CRITICAL);
				}
            }

            return bComplete;
        }

		private void BuildImportFileList(ArrayList importFiles, string file, bool drillSubDirectories)
		{
			string fullFile = file;

			// Check if there is a drive specified in the targetFile?
			if ((fullFile.IndexOf(":\\") == -1) && (fullFile.IndexOf(":/") == -1))
			{
				// Stick on the current directory in front of what the user specified
				fullFile = string.Concat(DosUtils.CurrentDirectory(), "\\", file);
			}

			// Check for wildcards
			if (fullFile.IndexOf("*") != -1)
			{
				AddFile(importFiles, DosUtils.PathGetDirectoryPath(fullFile), DosUtils.PathGetFileName(fullFile), drillSubDirectories);
			}			
			else
			{
				string displayFile = fullFile;
				if (fullFile.StartsWith(DosUtils.CurrentDirectory(), StringComparison.CurrentCultureIgnoreCase))
				{
					// Collapse this into a relative file because it will look better
					displayFile = fullFile.Substring(DosUtils.CurrentDirectory().Length + 1);
				}
				Console.WriteLine("   - " + displayFile);
				importFiles.Add(fullFile);
			}
		}

		private void AddFile(ArrayList importFiles, string fullPath, string pattern, bool drillSubDirectories)
		{
			FileInfo[] files = DosUtils.FileGetList(fullPath, pattern);
			if (files != null && files.Length > 0)
			{
				foreach (FileInfo f in files)
				{
					Console.WriteLine("   - " + f.FullName.Substring(fullPath.Length+1));
					importFiles.Add(f.FullName);
				}
			}

			if (drillSubDirectories)
			{
				DirectoryInfo[] dirs = DosUtils.DirectoryGetList(fullPath, "*.*");
				if (dirs != null && dirs.Length > 0)
				{
					foreach (DirectoryInfo dir in dirs)
					{
						AddFile(importFiles, dir.FullName, pattern, drillSubDirectories);
					}
				}
			}
		}

        private bool ImportWithWizard(ArrayList sourceFullNames, ArrayList detectedAssetNames, MOG_Filename proposedAssetName, out DialogResult result)
        {
			ArrayList inValidAssetNames = new ArrayList();
            ImportAssetWizard wizard;

            foreach (string sourceFullName in sourceFullNames)
            {
                // Create a new invalid name
                ImportFile invalidName = new ImportFile(sourceFullName);

                // Add all possible matches to this name
                foreach (MOG_Filename potentialMatch in detectedAssetNames)
                {
                    invalidName.mPotentialFileMatches.Add(potentialMatch);
                }

                // Add to our invalidNames array
                inValidAssetNames.Add(invalidName);
            }


            // Loop through each filename to be imported, and import them
            for (int f = 0; f < inValidAssetNames.Count; f++)
            {
                // Launch the wizard
                wizard = new ImportAssetWizard();

                // Set the wizard startup variables
                wizard.ImportSourceFilename = ((ImportFile)inValidAssetNames[f]).mImportFilename;
                wizard.ImportPotentialMatches = ((ImportFile)inValidAssetNames[f]).mPotentialFileMatches;
				wizard.Seed_AssetClassification = proposedAssetName.GetAssetClassification();
				wizard.Seed_AssetPlatform = proposedAssetName.GetAssetPlatform();
				wizard.Seed_AssetLabel = proposedAssetName.GetAssetLabel();
				wizard.Seed_AssetProperties = mProperties;

				// Show the form
				wizard.StartPosition = FormStartPosition.CenterScreen;
                result = wizard.ShowDialog(null);

                // Did the user complete the wizard
                if (result == DialogResult.OK)
                {
                    // Create pur import source files array
                    ArrayList importList = new ArrayList();
                    importList.Add(wizard.ImportEndSourceTextBox.Text);

					// Import it according to the wizard settings
                    try
                    {
						// Import this asset with its properties
                        MOG_ControllerAsset.CreateAsset(wizard.ImportEndMogTextBox.Text, "", importList, null, wizard.ImportPropertyArray, false, false);
                    }
                    catch (Exception e)
                    {
                        MOG_Report.ReportMessage("Import", "Could not import the file:\n" + wizard.ImportEndMogTextBox.Text + "\n\nError Message:\n" + e.Message, e.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.ERROR);
                        return false;
                    }
                }
                else if (result == DialogResult.Ignore)
                {
                    return true;
                }
            }

            result = DialogResult.OK;
            return true;
        }

		private bool Rip(string assetName)
		{
			bool complete = false;

			Console.WriteLine("Waiting to obtain login information from Client.");
			// Wait until we have finished establishing our connection with the server
			MOG_ControllerSystem.RequireProjectUserInitialization();
			Console.WriteLine("Login information has been obtained.");

			MOG_Filename assetFilename = new MOG_Filename(assetName);
			string inboxPath = assetFilename.GetAssetEncodedInboxPath("Drafts");
			MOG_Filename inboxAssetFilename = new MOG_Filename(inboxPath);
			bool bUseLocalProcessing = true;
			bool bWaitForCompletion = false;

			// Check if we should rip this locally?
			if (bUseLocalProcessing)
			{
				// Inform the user we are using local processing only
				Console.WriteLine(string.Concat("Ripping asset using local processing."));

				// Perform the ripper test
				MOG_ControllerRipper ripper = new MOG_ControllerRipper(inboxAssetFilename);
				complete = ripper.LocalRipper();
			}
			else
			{
				// Inform the user we are using the network slaves
				Console.WriteLine(string.Concat("Ripping asset using network slave processing."));

				// Initiate the rip request
				if (MOG_ControllerInbox.RipAsset(inboxAssetFilename))
				{
					// Check if we need to wait for the rip to complete?
					if (bWaitForCompletion)
					{
						// Wait for this asset to complete
						Console.WriteLine(string.Concat("Waiting for rip process to complete..."));
					}

					complete = true;
				}
			}

			if (complete)
			{
				Console.WriteLine(string.Concat("Rip complete!"));
			}

			return complete;
		}

		private bool SlaveTask(string ripper, string source, string slaveTaskDescription, string validSlaves)
		{
			bool bFailed = false;

			// Gather the login information from the RipTasker environment
			string projectName = DosUtils.GetSystemEnvironmentVariable(MOG_EnvironmentVars.GetProjectName());
			string branchName = DosUtils.GetSystemEnvironmentVariable(MOG_EnvironmentVars.GetBranchName());
			string userName = DosUtils.GetSystemEnvironmentVariable(MOG_EnvironmentVars.GetUserName());
			MOG_ControllerProject.LoginProject(projectName, branchName);
			MOG_ControllerProject.LoginUser(userName);

			// Obtain the remaining SlaveTask variables from the RipTasker environment
			int originatingNetworkID = MOG_ControllerSystem.GetNetworkID();
			string originatingUserName = DosUtils.GetSystemEnvironmentVariable(MOG_EnvironmentVars.GetUserName());
			string jobLabel = DosUtils.GetSystemEnvironmentVariable(MOG_EnvironmentVars.GetCommandLabel());
			string assetFullName = DosUtils.GetSystemEnvironmentVariable(MOG_EnvironmentVars.GetAsset());
			string platformName = DosUtils.GetSystemEnvironmentVariable(MOG_EnvironmentVars.GetPlatformName());
			string workingDirectory = DosUtils.GetSystemEnvironmentVariable(MOG_EnvironmentVars.GetWorkingDirectory());
			string destination = DosUtils.GetSystemEnvironmentVariable(MOG_EnvironmentVars.GetRippedFilesDestination());
			ArrayList environment = DosUtils.GetSystemEnvironmentVariables();

			// Make sure this has all the required information?
			if (originatingNetworkID == 0)
			{
				Console.WriteLine("Unable to obtain a valid NetworkID from the MOG Server.");
				bFailed = true;
			}
			if (originatingUserName == null || originatingUserName.Length == 0)
			{
				Console.WriteLine("Missing '" + MOG_EnvironmentVars.GetUserName() + "' environment variable.");
				bFailed = true;
			}
			if (jobLabel == null || jobLabel.Length == 0)
			{
				Console.WriteLine("Missing '" + MOG_EnvironmentVars.GetCommandLabel() + "' environment variable.");
				bFailed = true;
			}
			if (assetFullName == null || assetFullName.Length == 0)
			{
				Console.WriteLine("Missing '" + MOG_EnvironmentVars.GetAsset() + "' environment variable.");
				bFailed = true;
			}
			if (platformName == null || platformName.Length == 0)
			{
				Console.WriteLine("Missing '" + MOG_EnvironmentVars.GetPlatformName() + "' environment variable.");
				bFailed = true;
			}
			if (workingDirectory == null || workingDirectory.Length == 0)
			{
				Console.WriteLine("Missing '" + MOG_EnvironmentVars.GetWorkingDirectory() + "' environment variable.");
				bFailed = true;
			}
			if (destination == null || destination.Length == 0)
			{
				Console.WriteLine("Missing '" + MOG_EnvironmentVars.GetRippedFilesDestination() + "' environment variable.");
				bFailed = true;
			}
			if (environment == null || environment.Count == 0)
			{
				Console.WriteLine("Failed to obtain any environment variables.");
				bFailed = true;
			}

			if (!bFailed)
			{
				// Send our new default slave task command to the server
				MOG_Command newSlaveTask = MOG.COMMAND.MOG_CommandFactory.Setup_SlaveTask(	originatingNetworkID,
																							originatingUserName,
																							jobLabel,
																							new MOG_Filename(assetFullName),
																							platformName, 
																							workingDirectory,
																							ripper,
																							environment,
																							"",
																							source,
																							destination,
																							slaveTaskDescription,
																							validSlaves);
				return MOG_ControllerSystem.GetCommandManager().SendToServer(newSlaveTask);
			}
			else
			{
				// Inform the user that this command only works within the RipTasker environment
				ExtraError(string.Format("ERROR - 'SlaveTask' should only be used within an active RipTasker environment."));
			}

			return false;
		}

		private bool Bless(string assetName, string comment)
		{
			bool complete = false;

			Console.WriteLine("Waiting to obtain login information from Client.");
			// Wait until we have finished establishing our connection with the server
			MOG_ControllerSystem.RequireProjectUserInitialization();
			Console.WriteLine("Login information has been obtained.");

			MOG_Filename assetFilename = new MOG_Filename(assetName);
			// Make sure we have the adam object on the filename now so projects don't have to include the name of their project in all of their commandline scripts
            MOG_Filename proposedAssetName = MOG_Filename.AppendAdamObjectNameOnAssetName(assetFilename);
			string inboxPath = proposedAssetName.GetAssetEncodedInboxPath("Drafts");
			MOG_Filename inboxAssetFilename = new MOG_Filename(inboxPath);
			bool bWaitForCompletion = false;

			// Inform the user we are using the network slaves
			Console.WriteLine(string.Concat("Blessing asset."));

			// Initiate the rip request
			if (MOG_ControllerInbox.BlessAsset(inboxAssetFilename, comment, false, null))
			{
				// Check if we need to wait for the rip to complete?
				if (bWaitForCompletion)
				{
					// Wait for this asset to complete
					Console.WriteLine(string.Concat("Waiting for bless to be posted..."));
				}

				complete = true;
			}

			if (complete)
			{
				Console.WriteLine(string.Concat("Bless complete!"));
			}

			return complete;
		}

		private string VerifyProperFilename(string str)
		{
			return str.Replace("/", "\\");
		}
		/// <summary>
		/// Used to parse the values out of the commandline args	
		/// </summary>
		/// <returns>true if we successfully parse the args and have all the args we need.</returns>
		public bool ParseCommandLine()
		{
			bool SyncMissing = false;
			mCompleted = false;
			// Get the Index of the -Class argument
			int index = CheckOption(mArgs, "-Class");
			//If it is not -1 (meaning it is missing)
			if (index != -1)
			{
				//And if the number of items in the mArgs array is at least large enough to refrence the index returned.
				if (mArgs.Count >= index + 1)
				{
					//assign the mAssetClassification to the text value assigned to the -class arg.
					mAssetClassification = (string)mArgs[index + 1];
					//remove class from the Args List.
					RemoveOption(mArgs, "-class");
					RemoveOption(mArgs, mAssetClassification);
				}
			}
			//Look for the optional file Arg and assign the variable as appropriate.
			index = CheckOption(mArgs, "-file");
			if (index != -1)
			{
				if (mArgs.Count >= index + 1)
				{
					mOptionalFile = (string)mArgs[index + 1];
					RemoveOption(mArgs, "-file");
					RemoveOption(mArgs, mOptionalFile);
				}
			}

			index = CheckOption(mArgs, "-pause");
			if (index != -1)
			{
				mPause = true;
				RemoveOption(mArgs, "-pause");
			}

			mQuiet = false;

			//This is basically the same as the CheckOption function but just verifies that a switch is thrown on the command line.
			index = 0;
			foreach( string arg in mArgs)
			{
				if( string.Compare(arg.ToLower(), "-q", true) == 0 )
				{
					mQuiet = true;

					// Force the progress dialog to stay hidden
					ProgressDialog.Hidden = true;

					mArgs.RemoveAt(index);
					break;
				}
				else
				{
					index++;
				}
			}

			// Check if we want to drill on import using a wildcard
			index = 0;
			foreach (string arg in mArgs)
			{
				if (string.Compare(arg.ToLower(), "-s", true) == 0)
				{
					mDrill = true;
					mArgs.RemoveAt(index);
					break;
				}
				else
				{
					index++;
				}
			}

			//check for missing sync param
			index = 0;
			foreach( string arg in mArgs)
			{
				if( string.Compare(arg.ToLower(), "-m", true) == 0 )
				{
					SyncMissing = true;
					mArgs.RemoveAt(index);
					break;
				}
				else
				{
					index++;
				}
			}


			//find what command we are using
			if(mArgs.Count > 0)
			{
				switch(((string)mArgs[0]).ToLower())
				{
				case "checkout":		mCompleted = CallCheckOut(); break;
				case "checkin":			mCompleted = CallCheckIn(); break;
				case "lock":			mCompleted = CallLock(); break;
				case "unlock":			mCompleted = CallUnlock(); break;
				case "import":			mCompleted = CallImport(); break;
				case "importcopy":		mCompleted = CallImportCopy(); break;
				case "importmove":		mCompleted = CallImportMove(); break;
				case "rip":				mCompleted = CallRip(); break;
				case "slavetask":		mCompleted = CallSlaveTask(); break;
				case "bless":			mCompleted = CallBless(); break;
				case "sync":			mCompleted = CallSync(SyncMissing); break;
				case "message":			mCompleted = CallMessage(); break;
				case "systemalert":		mCompleted = CallSystemAlert(); break;
				case "systemerror":		mCompleted = CallSystemError(); break;
				case "databaseimport":	mCompleted = CallDatabaseImport(); break;
				case "databaseexport":	mCompleted = CallDatabaseExport(); break;
				case "help": CallHelp(); break;	
				default:
					mFoundCommand = CommandType.BADCOMMAND;
					CallHelp();					
					return false;									
				}
			}
			else
			{
				mFoundCommand = CommandType.NOCOMMAND;
				CallHelp();
				return false;
			}

			// Indicate the status of the command
			if (!mCompleted)
			{
				if (!mQuiet)
				{					
					Console.WriteLine("MOG_CommandLine ERROR - The command was not processed...");			
				}
			}

			return mCompleted;
		}

		public void ParseImplicitParams()
		{
			int index;
			String Project, User, Branch = "";

			index = CheckOption(mArgs, "-b");
			if (index != -1)
			{
				if (mArgs.Count >= index + 1)
				{
					Branch = (string)mArgs[index + 1];
					RemoveOption(mArgs, "-b");
					RemoveOption(mArgs, Branch);
				}
			}

			index = CheckOption(mArgs, "-p");
			if(index != -1)
			{
				if (mArgs.Count >= index + 1)
				{
					Project = (string)mArgs[index + 1];
					RemoveOption(mArgs, "-p");
					RemoveOption(mArgs, Project);

					MOG_ControllerProject.LoginProject(Project, Branch);
				}
			}

			index = CheckOption(mArgs, "-u");
			if (index != -1)
			{
				if (mArgs.Count >= index + 1)
				{
					User = (string)mArgs[index + 1];
					RemoveOption(mArgs, "-u");
					RemoveOption(mArgs, User);

					MOG_ControllerProject.LoginUser(User);
				}
			}

			index = CheckOption(mArgs, "-Login");
			if (index != -1)
			{
				mRequireLogin = true;
				RemoveOption(mArgs, "-Login");
			}
		}

		public bool ParseProperties()
		{
			int InitalIndex = 0;
			int PropsFound = 0;
			bool Found = false;

			mProperties = new ArrayList();
			mProperties.Clear();

			foreach (string arg in mArgs)
			{
				if (string.Compare(arg.ToLower(), "-t", true) == 0)
				{
					Found = true;
					break;
				}
				else
				{
					InitalIndex++;
				}
			}

			if (Found)
			{
				// Check for properties with split()
				for (int i = InitalIndex + 1; i < mArgs.Count; i++)
				{
					string propertyString = (string)mArgs[i];

					// Create a property with our desired defaults
					MOG_Property property = new MOG_Property("UserProperties", "Custom", "UserKey", "");

					// Check if this was a system shortcut?
					if (propertyString.StartsWith("$"))
					{
						// Split this into the key and value
						string[] parts = propertyString.Split("=".ToCharArray(), 2);
						if (parts.Length == 2)
						{
							// Remove the shortcut character from the beginning of this key
							string key = parts[0].Remove(0, 1);
							string value = parts[1];

							// Populate the remaining portion of the property based on the shortcut
							switch (key.ToLower())
							{
								case "sourceapp":
									property = MOG_PropertyFactory.MOG_Relationships.New_AssetSourceApplication(value);
									break;
								case "sourcefile":
									property = MOG_PropertyFactory.MOG_Relationships.New_AssetSourceFile(value);
									break;
								case "package":
									string package = MOG_ControllerPackage.GetPackageName(value);
									string packageGroup = MOG_ControllerPackage.GetPackageGroups(value);
									string packageObject = MOG_ControllerPackage.GetPackageObjects(value);
									property = MOG_PropertyFactory.MOG_Relationships.New_PackageAssignment(package, packageGroup, packageObject);
									break;
								case "synctargetpath":
									property = MOG_PropertyFactory.MOG_Sync_OptionsProperties.New_SyncTargetPath(value);
									break;
								default:
									// TODO - It would be nice if we could ask MOG for the Section and PropertySection of any hardcoded properties so they would all be included in the shortcut API
									//									property.mSection = MOG_Property.LookupSectionForKey(key);
									//									property.mPropertySection = MOG_Property.LookupPropertySectionForKey(key);
									property.mPropertyKey = key;
									property.mPropertyValue = value;
									break;
							}
						}
						else
						{
							// ERROR
						}
					}
					else
					{
						// Have the Property parse this propertyString
						property.ParsePropertyString(propertyString);
					}

					// Add the new property
					mProperties.Add(property);						

					PropsFound++;
				}


				//remove arguments and -t
				mArgs.RemoveRange(InitalIndex, PropsFound + 1);
			}

			return true;
		}

		private bool InitCommand()
		{
			try
			{
				// Attempt to initialize the CommandLin?
				if (MOG_Main.Init_CommandLine(mConfigFilename, "CommandLine"))
				{
					// Make sure we obtain a valid NetworkID before continuing
					if (MOG_ControllerSystem.RequireNetworkIDInitialization())
					{
					}
					else
					{
						string message = "Mog Initialization Error:\n" + 
							"CommandLine was unable to initialize.";
						// This is a great place to kill the application because there is already another server running
						Console.WriteLine(message);
						return false;
					}
				}
				else
				{
					string message = "Mog Initialization Error:\n" + 
						"CommandLine was unable to initialize.";
					// This is a great place to kill the application because there is already another server running
					Console.WriteLine(message);
					return false;
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(	"Mog Exception Error:\n" + 
					e.Message + "\n" + 
					e.StackTrace);
				return false;
			}

			command = new MOG_Command();
			return true;
		}
		
		private bool CallCheckOut()
		{
			if (InitCommand())
			{
				ParseImplicitParams();

				if (ParseProperties())
				{
					if (mArgs.Count == 2 || mArgs.Count == 3)
					{
						// Import the asset
						string assetFullName = VerifyProperFilename((string)mArgs[1]);
						string comment = "";
						if (mArgs.Count == 3)
						{
							comment = (string)mArgs[2];
						}

						return CheckOut(new MOG_Filename(assetFullName), comment);
					}
					else
					{
						ExtraError("ERROR - Wrong number of parameters for CheckOut command ");
						if (!mQuiet)
						{
							HelpCheckOut();
						}
					}
				}
			}

			return false;
		}

		private bool CallCheckIn()
		{
			if (InitCommand())
			{
				ParseImplicitParams();

				if (mArgs.Count == 2 || mArgs.Count == 3)
				{
					// Import the asset
					string assetFullName = VerifyProperFilename((string)mArgs[1]);
					string comment = "";
					if (mArgs.Count == 3)
					{
						comment = (string)mArgs[2];
					}

					return CheckIn(new MOG_Filename(assetFullName), comment);
				}
				else
				{
					ExtraError("ERROR - Wrong number of parameters for CheckIn command ");
					if (!mQuiet)
					{
						HelpCheckIn();
					}
				}
			}

			return false;
		}
		
		private bool CallLock()
		{
			if (!mQuiet)
			{
				Console.WriteLine(" Lock Command not implimented yet! ");
			}
			
			return false;
		}

		private bool CallUnlock()
		{
			if(!mQuiet)
			{
				Console.WriteLine(" UnLock Command not implimented yet! ");
			}

			return false;
		}

		private bool CallImport()
		{
			if (InitCommand())
			{
				ParseImplicitParams();

				if (ParseProperties())
				{
					if (mArgs.Count == 3)
					{
						// Import the asset
						string assetFullName = VerifyProperFilename((string)mArgs[1]);
						string importFile = VerifyProperFilename((string)mArgs[2]);

						return Import(new MOG_Filename(assetFullName), importFile, false);
					}
					else if (mArgs.Count > 3)
					{
						// Import the asset list
						string assetFullName = VerifyProperFilename((string)mArgs[1]);
						string importFile = "";
						for (int i = 2; i < mArgs.Count; i++)
						{
							if (importFile.Length == 0)
							{
								importFile = VerifyProperFilename((string)mArgs[i]);
							}
							else
							{
								importFile = importFile + "|" + VerifyProperFilename((string)mArgs[i]);
							}
						}

						return Import(new MOG_Filename(assetFullName), importFile, false);
					}
					else
					{
						ExtraError("ERROR - Wrong number of parameters for Import command ");
						if (!mQuiet)
						{
							HelpImport();
						}
					}
				}
			}

			return false;
		}

		private bool CallImportCopy()
		{
			if (InitCommand())
			{
				ParseImplicitParams();

				if (ParseProperties())
				{
					//Any different than Import?
					if (mArgs.Count == 3)
					{
						// Import the asset
						string assetFullName = VerifyProperFilename((string)mArgs[1]);
						string importFile = VerifyProperFilename((string)mArgs[2]);

						return Import(new MOG_Filename(assetFullName), importFile, false);
					}
					else if (mArgs.Count > 3)
					{
						// Import the asset list
						string assetFullName = VerifyProperFilename((string)mArgs[1]);
						string importFile = "";
						for (int i = 2; i < mArgs.Count; i++)
						{
							if (importFile.Length == 0)
							{
								importFile = VerifyProperFilename((string)mArgs[i]);
							}
							else
							{
								importFile = "|" + VerifyProperFilename((string)mArgs[i]);
							}
						}

						return Import(new MOG_Filename(assetFullName), importFile, false);
					}
					else
					{
						ExtraError("ERROR - Wrong number of parameters for ImportCopy command ");
						if (!mQuiet)
						{
							HelpImportCopy();
						}
					}
				}
			}

			return false;
		}

		private bool CallImportMove()
		{
			if (InitCommand())
			{
				ParseImplicitParams();

				if (ParseProperties())
				{
					if (mArgs.Count == 3)
					{
						// Import the asset
						string assetFullName = VerifyProperFilename((string)mArgs[1]);
						string importFile = VerifyProperFilename((string)mArgs[2]);

						return Import(new MOG_Filename(assetFullName), importFile, true);
					}
					else
					{
						ExtraError("ERROR - Wrong number of parameters for ImportMove command ");
						if (!mQuiet)
						{
							HelpImportMove();
						}
					}
				}
			}

			return false;
		}
		
		private bool CallRip()
		{
			if (InitCommand())
			{
				ParseImplicitParams();

				if (mArgs.Count == 2)
				{
					// Import the asset
					return Rip(VerifyProperFilename((string)mArgs[1]));
				}
				else
				{
					ExtraError("ERROR - Wrong number of parameters for Rip command ");
					if (!mQuiet)
					{
						HelpRip();
					}
				}
			}

			return false;
		}

		private bool CallSlaveTask()
		{
			if (InitCommand())
			{
				ParseImplicitParams();

				if (mArgs.Count >= 3)
				{
					// Obtain the arguments
					string ripper = (string)mArgs[1];
					string source = (string)mArgs[2];
					string slaveTaskDescription = "";
					string validSlaves = "";

					if (mArgs.Count >= 4)
					{
						slaveTaskDescription = (string)mArgs[3];
					}

					if (mArgs.Count >= 5)
					{
						validSlaves = (string)mArgs[4];
					}

					// Generate the SlaveTask
					return SlaveTask(ripper, source, slaveTaskDescription, validSlaves);
				}
				else
				{
					ExtraError("ERROR - Wrong number of parameters for SlaveTask command ");
					if (!mQuiet)
					{
						HelpSlaveTask();
					}
				}
			}

			return false;
		}

		private bool CallBless()
		{
			if (InitCommand())
			{
				ParseImplicitParams();

				// Bless
				if (mArgs.Count == 3)
				{
					// Import the asset
					return Bless(VerifyProperFilename((string)mArgs[1]), (string)mArgs[2]);
				}
				else
				{
					ExtraError("ERROR - Wrong number of parameters for Bless command ");
					if (!mQuiet)
					{
						HelpBless();
					}
				}
			}

			return false;
		}

		private bool CallSync(bool SyncMissing)
		{
			if (InitCommand())
			{
				ParseImplicitParams();

				if (mArgs.Count >= 4)
				{
					// Login to the specified Project
					MOG_Project project = MOG_ControllerProject.GetProject();
					if (project != null)
					{
						string projectName = project.GetProjectName();
						string branchName = MOG_ControllerProject.GetBranchName();
						string userName = MOG_ControllerProject.GetUserName_DefaultAdmin();
						string platform = (string)mArgs[1];
						string workingDirectory = (string)mArgs[2];
						string syncClassification = (string)mArgs[3];
						string exclusions = (mArgs.Count >= 5) ? (string)mArgs[4] : "";
						string inclusions = (mArgs.Count >= 6) ? (string)mArgs[5] : "";
						if (syncClassification.EndsWith(".sync"))
						{
							string filename = MOG_ControllerSystem.LocateTool(syncClassification);
							if (!String.IsNullOrEmpty(filename))
							{
								SyncFilter filter = new SyncFilter();
								if (filter.Load(filename))
								{
									exclusions = filter.GetExclusionString();
									inclusions = filter.GetInclusionString();
								}

								syncClassification = projectName;
							}
						}
						
						// Get platform
						MOG_Platform activePlatform = project.GetPlatform(platform);
						if (activePlatform == null)
						{
							string platforms = "";
							string[] existingPlatforms = project.GetPlatformNames();
							foreach (string platformName in existingPlatforms)
							{
								platforms += " " + platformName;
							}

							MOG_Report.ReportMessage("MOG_Commandline: Sync",
														platform + " is not a valid platform\n" +
														"EXISTING PLATFORMS: " + platforms,
														Environment.StackTrace, MOG_ALERT_LEVEL.ERROR);
						}
						else
						{
							if (!mQuiet)
							{
								Console.WriteLine(string.Format("====== Sync====== \n  Project: {0}\n  Branch: {1}\n  Platform: {2}\n  Target Directory:{3}\n  Sync Classification or SyncFile: {4}\n  Exclusions: {5}\n  Inclusions: {6}\n  Sync MissingFiles: {7}",
									projectName, branchName, platform, workingDirectory, syncClassification, exclusions, inclusions, SyncMissing.ToString()));
							}
															
							// Initialize the System
							MOG_ControllerSyncData sync = new MOG_ControllerSyncData(workingDirectory, projectName, branchName, platform, userName);
							MOG_ControllerProject.SetCurrentSyncDataController(sync);

							return sync.SyncRepositoryData(syncClassification, exclusions, inclusions, SyncMissing, null);
						}
					}
				}
				else
				{
					ExtraError("ERROR - Wrong number of parameters for Sync command ");
					if (!mQuiet)
					{
						HelpSync();
					}
				}
			}

			return false;
		}

		private bool CallMessage()
		{
			if (InitCommand())
			{
				// NetworkBroadcast
				if (mArgs.Count >= 3)
				{
					string project = (string)mArgs[1];
					string users = (string)mArgs[2];
					string message = "";

					// Login to the specified Project
					MOG_ControllerProject.LoginProject(project, "");

					if (mArgs.Count >= 4)
					{
						message = (string)mArgs[3];
					}
					else if (mOptionalFile != null && mOptionalFile.Length > 0)
					{
						if (File.Exists(mOptionalFile))
						{
							try
							{
								StreamReader sr = new StreamReader(mOptionalFile);
								message = sr.ReadToEnd();
								sr.Close();
							}
							catch (Exception e)
							{
								if (!mQuiet)
								{
									Console.WriteLine("File (" + mOptionalFile + ") could not be read because of error:\n" + e.ToString() + "\nCannot send message!");
								}
								return false;
							}
						}
						else
						{
							if (!mQuiet)
							{
								Console.WriteLine("File (" + mOptionalFile + ") does not exist.  Cannot send message!");
							}
							return false;
						}
					}

					if (MOG_ControllerSystem.NetworkBroadcast(users, message))
					{
						if (!mQuiet)
						{
							Console.WriteLine("Message has been sent to " + users);
						}
						return true;
					}
				}
				else
				{
					ExtraError("ERROR - Wrong number of parameters for Message command ");
					if (!mQuiet)
					{
						HelpMessage();
					}
				}
			}

			return false;
		}

		private bool CallSystemAlert()
		{
			if (InitCommand())
			{
				ParseImplicitParams();

				if (mArgs.Count == 2)
				{
					if (mRequireLogin)
					{
						Console.WriteLine("Waiting to obtain login information from Client.");
						// Wait until we have finished establishing our connection with the server
						MOG_ControllerSystem.RequireProjectUserInitialization();
						Console.WriteLine("Login information has been obtained.");
					}

					return MOG_ControllerSystem.NotifySystemAlert("MOG_CommandLine", (string)mArgs[1], null, true);
				}
				else
				{
					ExtraError("ERROR - Wrong number of parameters for SystemAlert command ");
					if (!mQuiet)
					{
						HelpSystemAlert();
					}
				}
			}

			return false;
		}

		private bool CallSystemError()
		{
			if (InitCommand())
			{
				ParseImplicitParams();

				if (mArgs.Count == 2)
				{
					if (mRequireLogin)
					{
						Console.WriteLine("Waiting to obtain login information from Client.");
						// Wait until we have finished establishing our connection with the server
						MOG_ControllerSystem.RequireProjectUserInitialization();
						Console.WriteLine("Login information has been obtained.");
					}

					return MOG_ControllerSystem.NotifySystemError("MOG_CommandLine", (string)mArgs[1], null, true);
				}
				else
				{
					ExtraError("ERROR - Wrong number of parameters for SystemError command ");
					if (!mQuiet)
					{
						HelpSystemError();
					}
				}
			}

			return false;
		}

		private bool CallDatabaseImport()
		{
			if (InitCommand())
			{
				if (mArgs.Count == 2 || mArgs.Count == 3)
				{
					string projectName = "";
					string location = "";

					// Get the name of the specified project
					projectName = mArgs[1] as string;
					// Get the location
					if (mArgs.Count == 3)
					{
						// Use what the user specified
						location = mArgs[2] as string;
					}
					else
					{
						// Default to the project location in the repository
						location = Path.Combine(MOG_ControllerSystem.GetSystemProjectsPath(), projectName);
					}

					// Inform the user we are starting
					Console.WriteLine("**Importing database tables...");
					Console.WriteLine("	PROJECT: " + projectName);
					Console.WriteLine("	 SOURCE: " + location);

					// Erase all the tables so we know there is no stale data
					MOG_ControllerSystem.GetDB().DeleteProjectTables(projectName);
					// Create new empty tables so we will have a blank slate
					MOG_ControllerSystem.GetDB().VerifyTables(projectName);
					// Proceed to import the specified tables
					return MOG_Database.ImportProjectTables(projectName, location);
				}
				else
				{
					ExtraError("ERROR - Wrong number of parameters for CallDatabaseImport command ");
					if (!mQuiet)
					{
						HelpDatabaseImport();
					}
				}
			}

			return false;
		}

		private bool CallDatabaseExport()
		{
			if (InitCommand())
			{
				if (mArgs.Count == 2 || mArgs.Count == 3)
				{
					string projectName = "";
					string location = "";

					// Get the name of the specified project
					projectName = mArgs[1] as string;
					// Get the location
					if (mArgs.Count == 3)
					{
						// Use what the user specified
						location = mArgs[2] as string;
					}
					else
					{
						// Default to the project location in the repository
						location = Path.Combine(MOG_ControllerSystem.GetSystemProjectsPath(), projectName);
					}

					// Inform the user we are starting
					Console.WriteLine("**Exporting database tables...");
					Console.WriteLine("	PROJECT: " + projectName);
					Console.WriteLine("	 TARGET: " + location);

					return MOG_Database.ExportProjectTables(projectName, location);
				}
				else
				{
					ExtraError("ERROR - Wrong number of parameters for CallDatabaseExport command ");
					if (!mQuiet)
					{
						HelpDatabaseExport();
					}
				}
			}

			return false;
		}

		private void CallHelp()
		{
			if(mFoundCommand == CommandType.GOODCOMMAND)
			{
				if(mArgs.Count > 1)
				{
					switch(((string)mArgs[1]).ToLower())
					{
						case "lock":			HelpLock();					break;
						case "unlock":			HelpUnlock();				break;
						case "import":			HelpImport();				break;
						case "importcopy":		HelpImportCopy();			break;
						case "importmove":		HelpImportMove();			break;
						case "rip":				HelpRip();					break;
						case "slavetask":		HelpSlaveTask();			break;
						case "bless":			HelpBless();				break;
						case "sync":	        HelpSync();			        break;
						case "message":			HelpMessage();				break;
						case "systemalert":		HelpSystemAlert();			break;
						case "systemerror":		HelpSystemError();			break;
						case "databaseimport":	HelpDatabaseImport();		break;
						case "databaseexport":	HelpDatabaseExport();		break;
						default:
							mFoundCommand = CommandType.BADHELPCOMMAND;
							break;
					}
				}
				else
				{
					mFoundCommand = CommandType.HELPCOMMAND;
				}

			}

			// Setup our version number
			FileVersionInfo file = FileVersionInfo.GetVersionInfo(Application.ExecutablePath);
			string version = "";
			if (file != null)
			{
				version = file.FileVersion;
			}
			
			if(mFoundCommand != CommandType.GOODCOMMAND)
			{
				Console.WriteLine("TransMOGrifier - MOG - Version " + version);
				Console.WriteLine("Copyright 2002-2007 MOGware, Inc.  All Rights Reserved");
				Console.WriteLine("");
				//Console.WriteLine("MOG_CommandLine Help...");
				//Console.WriteLine("");
				//Console.WriteLine("");
				Console.WriteLine("For more information about a specific command type ");
				Console.WriteLine("MOG_CommandLine Help [Command]");
				Console.WriteLine("");
				Console.WriteLine("Usage:");
				Console.WriteLine("MOG_CommandLine <Command> <required args> {implicit args} [optional args] ");
				Console.WriteLine("");
				Console.WriteLine("  Commands and Usage: ");
				Console.WriteLine("    Help [Command]");
				Console.WriteLine("    CheckOut <AssetFullName> [Comment]");
				Console.WriteLine("    CheckIn <AssetFullName> [Comment]");
				Console.WriteLine("    Lock <LockName> [Description]");
				Console.WriteLine("    Unlock <LockName>");
				Console.WriteLine("    Import <AssetFullName> <importFilename> {p|u||s} [-t Properties]");
				Console.WriteLine("    ImportCopy <AssetFullName> <importFilename> {p|u|s} [-t Properties]");
				Console.WriteLine("    ImportMove <AssetFullName> <importFilename> {p|u|s} [-t Properties]");
				Console.WriteLine("    Rip <FullAssetName> {p|b|u}");
				Console.WriteLine("    SlaveTask <RipTool> <Source> <description> [ValidSlaves]");
				Console.WriteLine("    Bless <FullAssetName> <Comment> {p|b|u}");
				Console.WriteLine("    Sync -p <ProjectName> -b <Branch> -u <User> <Plat> <WorkDir> (<SyncFilter.sync> or (<Classification> [exclusions] [inclusions])) [-m]");
				Console.WriteLine("    Message <LoginProject> <User> <Message>");
				Console.WriteLine("    SystemAlert <Message> {Login}");
				Console.WriteLine("    SystemError <Message> {Login}");
				Console.WriteLine("    DatabaseImport <ProjectName> [Location]");
				Console.WriteLine("    DatabaseExport <ProjectName> [Location]");
				Console.WriteLine("");
				ImplicitArgs();
				Console.WriteLine("");
				HelpProperty();
				Console.WriteLine("");
				Console.WriteLine("  Global Options:");
				Console.WriteLine("    -Q                 - Quiet output from commandline  ");
			}

			if(mFoundCommand == CommandType.BADCOMMAND)
			{
				ExtraError(String.Format("Unknown command: {0}", mArgs[0]));
			}

			if(mFoundCommand == CommandType.BADHELPCOMMAND)
			{
				ExtraError(String.Format("Unknown help command: Help {0}", mArgs[1]));
			}
		}

		public void HelpCheckOut()
		{
			Console.WriteLine(" CheckOut Help:");
			Console.WriteLine(" CheckOut <AssetFullName> [Comment]");
		}
		public void HelpCheckIn()
		{
			Console.WriteLine(" CheckIn Help:");
			Console.WriteLine(" CheckIn <AssetFullName> [Comment]");
		}
		public void HelpLock()
		{
			Console.WriteLine(" This is the Help for Lock ");
		}
		public void HelpUnlock()
		{
			Console.WriteLine(" This is the Help for Unlock ");
		}
		public void ImplicitArgs()
		{
			Console.WriteLine();
			Console.WriteLine("  Implicit Arguments: ");
			Console.WriteLine("    Values automatically obtained from the MOG Client running on the machine.");
			Console.WriteLine("    If explict values are specified, those values will be used instead.");
			Console.WriteLine("");
			Console.WriteLine("    -p <LoginProject> - Project <LoginProject> will be used");
			Console.WriteLine("    -b <LoginBranch>  - Branch <LoginBranch> of project will be used");
			Console.WriteLine("    -u <LoginUser>    - User <LoginUser> will be used");
			Console.WriteLine("    -s                - Search subdirectories when importing with wildcards");
			Console.WriteLine("    -login            - Obtains login infromation from the client");
			Console.WriteLine();
		}
		public void HelpProperty()
		{
			Console.WriteLine();
			Console.WriteLine("  Import Property:");
			Console.WriteLine("    -t specifies one or more Import Properties.");
			Console.WriteLine("    Any MOG Property can be explicitly defined and saved with the asset.");
			Console.WriteLine("    Custom project specific properties can also be specified.");
			Console.WriteLine();
			Console.WriteLine("    Property Format:");
			Console.WriteLine("    [Section]{PropertyType}PropertyName=Value");
			Console.WriteLine();
			Console.WriteLine("    Property Examples:");
			Console.WriteLine("    Compression");
			Console.WriteLine("    Compression=dxt5");
			Console.WriteLine("    {RipOptions}Compression=dxt5");
			Console.WriteLine("    [Properties]{RipOptions}Compression=dxt5");
			Console.WriteLine("    [Relationships]{Packages}Packages\\Human\\Vehicles=");
			Console.WriteLine("    [Relationships]{SourceFile}Library\\Human\\Vehicles\\Truck.max");
			Console.WriteLine();
			Console.WriteLine("    Property templates:");
			Console.WriteLine("    $SourceFile=Library\\Human\\Vehicles\\Truck.max");
			Console.WriteLine("    $SourceApp=C:\\Program Files\\Autodesk\\3DMax 9.0\\3DSMax.exe");
			Console.WriteLine();
		}
		public void HelpImport()
		{
			Console.WriteLine(" Import Help:");
			Console.WriteLine("");
			Console.WriteLine(" Importing single assets:");
			Console.WriteLine(" Import <AssetFullName> <ImportFilename> {p|u} [-t Properties]");
			Console.WriteLine("");
			Console.WriteLine(" Importing complex assets:");
			Console.WriteLine(" Import <AssetFullName> <ImportFilename> <ImportFilename> <ImportFilename> {p|u} [-t Properties]");
			Console.WriteLine(" Import <AssetFullName> <ImportPath>\\*.* {p|u|s} [-t Properties]");
			Console.WriteLine(" Import <AssetFullName> <ImportPath>\\*.bmp <ImportFilename> <ImportFilename> {p|u|s} [-t Properties]");
			ImplicitArgs();
			HelpProperty();
		}
		public void HelpImportCopy()
		{
			Console.WriteLine(" ImportCopy Help: ");
			Console.WriteLine("");
			Console.WriteLine(" Importing single assets:");
			Console.WriteLine(" ImportCopy <AssetFullName> <ImportFilename> {p|u} [-t Properties]");
			Console.WriteLine("");
			Console.WriteLine(" Importing complex assets:");
			Console.WriteLine(" ImportCopy <AssetFullName> <ImportFilename> <ImportFilename> <ImportFilename> {p|u} [-t Properties]");
			Console.WriteLine(" ImportCopy <AssetFullName> <ImportPath>\\*.* {p|u|s} [-t Properties]");
			Console.WriteLine(" ImportCopy <AssetFullName> <ImportPath>\\*.bmp <ImportFilename> <ImportFilename> {p|u|s} [-t Properties]");
			ImplicitArgs();
			HelpProperty();
		}
		public void HelpImportMove()
		{
			Console.WriteLine(" ImportMove Help:");
			Console.WriteLine("");
			Console.WriteLine(" Importing single assets:");
			Console.WriteLine(" ImportMove <AssetFullName> <ImportFilename> {p|u} [-t Properties]");
			Console.WriteLine("");
			Console.WriteLine(" Importing complex assets:");
			Console.WriteLine(" ImportMove <AssetFullName> <ImportFilename> <ImportFilename> <ImportFilename> {p|u} [-t Properties]");
			Console.WriteLine(" ImportMove <AssetFullName> <ImportPath>\\*.* {p|u|s} [-t Properties]");
			Console.WriteLine(" ImportMove <AssetFullName> <ImportPath>\\*.bmp <ImportFilename> <ImportFilename> {p|u|s} [-t Properties]");
			ImplicitArgs();
			HelpProperty();
		}
		public void HelpRip()
		{
			Console.WriteLine(" This is the Help for Rip ");
			Console.WriteLine(" Rip <AssetFullName> {-p <ProjectName> -u <UserName> | -Login}");
			Console.WriteLine();
			Console.WriteLine(" Example Usage:");
			Console.WriteLine(" Rip \"MyProject~Game Files~Build{PC}Game.exe\"");
		}
		public void HelpSlaveTask()
		{
			Console.WriteLine();
			Console.WriteLine(" This is the Help for SlaveTask ");
			Console.WriteLine(" SlaveTask <RipTool> <Source> <description> [ValidSlaves]");
			Console.WriteLine();
			Console.WriteLine(" SlaveTasks are used to construct custom lists of individual tasks for ");
			Console.WriteLine(" ripping Assets.  Each individual SlaveTask can be tasked out to its own ");
			Console.WriteLine(" Slave.  This allows MOG to solicit help from all available machines on the ");
			Console.WriteLine(" Network to speed up the ripping process.");
			Console.WriteLine();
			Console.WriteLine(" SlaveTask should only be used within a RipTasker environment.");
		}
		public void HelpBless()
		{
			Console.WriteLine(" This is the Help for Bless ");
			Console.WriteLine(" Bless <AssetFullName> \"Comment goes here.\" {-p <ProjectName> -u <UserName> | -Login}");
			Console.WriteLine();
			Console.WriteLine(" Example Usage:");
			Console.WriteLine(" Bless \"MyProject~Game Files~Build{PC}Game.exe\" \"Latest tested build.\"");
		}
		public void HelpSync()
		{
			Console.WriteLine();
			Console.WriteLine(" This is the Help for Sync ");
			Console.WriteLine(" Sync -p <ProjectName> -b <Branch> -u <User> <Platform> <WorkingDirectory> ");
			Console.WriteLine("      (<Classification> [exclusions] [inclusions])|<SyncFilter.sync> [-m]");
			Console.WriteLine();
			Console.WriteLine(" Optional Arguments:");
			Console.WriteLine(" exclusions are used to skip undesired assets from the sync.");
			Console.WriteLine(" -m is used to check for missing/modified files during the sync.");
			Console.WriteLine();
			Console.WriteLine(" Example Usage:");
			Console.WriteLine(" Sync -p MyProj -b Current PC C:\\ProjDir GameFiles -m Cinematics,Music");
			Console.WriteLine();
			Console.WriteLine(" This command will sync the PC files for the current branch of MyProj");
			Console.WriteLine(" into 'C:\\ProjDir' starting at the root sync node of GameFiles");
			Console.WriteLine(" Skipping Cinematics and Music assets, checking for missing/modified files");
			Console.WriteLine();
			Console.WriteLine(" Sync -p MyProj -b Current -u Admin PC C:\\ProjDir TestFilter.sync");
			Console.WriteLine();
			Console.WriteLine(" This command will sync the PC files for the current branch of MyProj");
			Console.WriteLine(" into 'C:\\ProjDir' starting at the project's root classification.");
			Console.WriteLine(" It will also load the classification exclusion and inclusion lists");
			Console.WriteLine(" from TestFilter.sync.");
			Console.WriteLine();
			Console.WriteLine(" Sync -p MyProj -b Current -u Admin PC C:\\ProjDir RootClassification RootClassification RootClassification~Data{All}MyAsset.txt");
			Console.WriteLine();
			Console.WriteLine(" This command will sync the PC files of a single asset into 'C:\\ProjDir'");
			Console.WriteLine(" This sets the exclusions to 'RootClassification'");
			Console.WriteLine(" This sets the inclusions to 'RootClassification~Data{All}MyAsset.txt'");
			Console.WriteLine(" Note: Multiple exclusions/inclusions can be comma delimited");
			Console.WriteLine();
		}
		public void HelpMessage()
		{
			Console.WriteLine(" This is the Help for Message ");
		}
		public void HelpSystemAlert()
		{
			Console.WriteLine(" This is the Help for SystemAlert ");
			Console.WriteLine(" SystemAlert \"Message goes here\" {-p <ProjectName> -u <UserName> | -Login}");
			Console.WriteLine();
			ImplicitArgs();
			Console.WriteLine();
			Console.WriteLine(" Example Usage:");
			Console.WriteLine(" SystemAlert \"Could not perform rip die to missing file...\" -p MyProject -u Jim");
			Console.WriteLine(" SystemAlert \"Could not perform rip die to missing file...\" -login");
			Console.WriteLine();
			Console.WriteLine(" Example 1 logs the message and notifies user Jim.");
			Console.WriteLine(" Example 2 logs the message and notifies the user obtained from the client");
			Console.WriteLine();
		}
		public void HelpSystemError()
		{
			Console.WriteLine(" This is the Help for SystemError ");
			Console.WriteLine(" SystemError \"Message goes here\" {-p <ProjectName> -u <UserName> | -Login}");
			Console.WriteLine();
			ImplicitArgs();
			Console.WriteLine();
			Console.WriteLine(" Example Usage:");
			Console.WriteLine(" SystemAlert \"Could not perform rip die to missing file...\" -p MyProject -u Jim");
			Console.WriteLine(" SystemAlert \"Could not perform rip die to missing file...\" -login");
			Console.WriteLine();
			Console.WriteLine(" Example 1 logs the message and notifies user Jim.");
			Console.WriteLine(" Example 2 logs the message and notifies the user obtained from the client");
			Console.WriteLine();
		}
		public void HelpDatabaseImport()
		{
			Console.WriteLine(" DatabaseImport <ProjectName> [Location]");
			Console.WriteLine();
			Console.WriteLine(" Example Usage:");
			Console.WriteLine(" DatabaseImport MyProject");
			Console.WriteLine(" DatabaseImport MyProject C:\\subfolder");
			Console.WriteLine();
		}
		public void HelpDatabaseExport()
		{
			Console.WriteLine(" DatabaseExport <ProjectName> [Location]");
			Console.WriteLine();
			Console.WriteLine(" Example Usage:");
			Console.WriteLine(" DatabaseExport MyProject");
			Console.WriteLine(" DatabaseExport MyProject C:\\subfolder");
			Console.WriteLine();
		}
		public void ExtraError(string str)
		{
			if(!mQuiet)
			{
				Console.WriteLine("");
				Console.WriteLine("------------------------------------------------------------");
				Console.WriteLine("{0}", str);
				Console.WriteLine("------------------------------------------------------------");
			}
		}

	}
}
