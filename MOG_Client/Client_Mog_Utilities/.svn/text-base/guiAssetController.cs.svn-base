using System;
using System.Windows.Forms;
using System.Collections;
using Microsoft.Win32;
using System.IO;
using System.Threading;
using System.Diagnostics;

using MOG;
using MOG.INI;
using MOG.PROJECT;
using MOG.PLATFORM;
using MOG.DATABASE;
using MOG.PROPERTIES;
using MOG.CONTROLLER;
using MOG.CONTROLLER.CONTROLLERSYNCDATA;
using MOG.CONTROLLER.CONTROLLERASSET;
using MOG.CONTROLLER.CONTROLLERREPOSITORY;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERPACKAGE;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.CONTROLLER.CONTROLLERINBOX;
using MOG.FILENAME;
using MOG.DOSUTILS;
using MOG.REPORT;
using MOG.TIME;
using MOG.PROMPT;
using MOG.PROGRESS;

using MOG_Client.Client_Utilities;
using MOG_Client.Forms;

using MOG_ControlsLibrary.Utils;
using MOG_ControlsLibrary.Forms.Wizards;
using System.Collections.Generic;
using System.ComponentModel;
using MOG_CoreControls;
using MOG_ControlsLibrary.MogUtils_Settings;
using MOG.ASSET_STATUS;
using System.Collections.Specialized;
using MOG.CONTROLLER.CONTROLLERLIBRARY;
using MOG_Client.Client_Gui.AssetManager_Helper;

namespace MOG_Client.Client_Mog_Utilities
{
	/// <summary>
	/// Summary description for MOG_ControllerAsset.
	/// </summary>
	public class guiAssetController
	{
		// Used for tracking the assets needing to be updated that were automatically triggered
		private static Queue<MOG_Filename> mAutoUpdateQueue = null;
		// Second queue for keeping track of the user initiated assets seperately because they should never be imported
		private static Queue<MOG_Filename> mUserUpdateQueue = null;
		private static object mUpdateQueueLock = new object();

		public enum AssetDirectories {IMPORTED, PROCESSED, SOURCE};

		public static List<string> mNewAssetNames = new List<string>();
		public static List<ArrayList> mNewAssetProperties = new List<ArrayList>();
		public static List<ImportFile> mInvalidAssetNames = new List<ImportFile>();

		static public string GetAssetSize(string asset)
		{
			string size = "n/a";

			try
			{
				// Make sure we have a valid workspace?
				if (MOG_ControllerProject.GetCurrentSyncDataController() != null)
				{
					// Attempt to obtain the size of this asset for the active platform
					MOG_Properties pProperties = new MOG_Properties(new MOG_Filename(asset));
					pProperties.SetScope(MOG_ControllerProject.GetCurrentSyncDataController().GetPlatformName());
					size = FormatSize(pProperties.Size);
				}
			}
			catch
			{
			}

			return size;
		}

		static public string FormatSize(string size)
		{
			if (size.Length > 0)
			{
				return FormatSize(Convert.ToInt64(size));
			}
			else
			{
				return "";
			}
		}

		static public string FormatSize(long size)
		{
			string length;
			if (size == 0)
			{
				length = "";
			}
			else if (size > 1024000)
			{
				// Do MegaByte conversion
				length = Convert.ToString((size/10240) + 1);
				length = string.Concat(length.Substring(0, length.Length - 2), ".", length.Substring(length.Length - 2), " MB");
			}
			else
			{
				// Do KiloByte conversion
				length = string.Concat(Convert.ToString((size/1024) + 1), " KB");
			}
			return length;
		}

		public bool GroupAsset(string groupName, string assetFullName)
		{
			bool rc = false;

			return rc;
		}

		public static bool RemoveBlessed(MOG_Filename asset, string jobLabel)
		{
			if (MOG_ControllerProject.RemoveAssetFromProject(asset, "No longer needed", jobLabel, false))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public static bool RemoveBlessedGroup(MOG_Filename asset, string groups, bool ShowConfirmation)
		{
			if (MOG_ControllerPackage.RemoveGroup(asset, groups))
			{
				if (ShowConfirmation)
				{
					string groupString = (groups.Length > 0) ? (" GROUP: " + groups) : "";

					// Display our message
					MOG_Prompt.PromptResponse("Asset Remove", asset.GetAssetName() + groupString + " has been successfully removed from the current version info file.  To be completely removed from the game you must rebuild its package.", MOGPromptButtons.OK);
				}
				return true;
			}
			else
			{
				return false;
			}
		}

        public static bool ImportSeparately(string[] sourceFullNames, bool looseFileMatching)
        {
            bool rc = false;

            // Import all previously imported assets
			ImportPrevious(sourceFullNames, looseFileMatching);
			if (mInvalidAssetNames.Count > 0)
            {
                DialogResult result = DialogResult.Ignore;

                // Has the user told us to only start with the MOG asset Imorter
                string StartInAdvancedMode = MogUtils_Settings.LoadSetting("ImportWizard", "StartupMode");
                if (StartInAdvancedMode != "Advanced")
                {
                    // Try running the import wizard first
					rc = ImportWithWizard(sourceFullNames, out result);
                }                    

                // Did the user descide to use the MOG Asset Importer
                if (result == DialogResult.Ignore)
                {
					rc = ImportWithMOGAssetImporter();
                }
            }

            return rc;
        }

		private static bool ImportWithMOGAssetImporter()
        {
			guiAssetImportCheck check = new guiAssetImportCheck();
			if (check.CheckImportAssetNames(mInvalidAssetNames, ref mNewAssetNames, ref mNewAssetProperties))
			{
				ProgressDialog progress = new ProgressDialog("Importing Assets", "Please wait while assets are imported...", ImportAsset_Worker, null, true);
				progress.ShowDialog(MogMainForm.MainApp);
			}

            return true;
        }

		private static void ImportAsset_Worker(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;

			// Loop through each filename to be imported, and import them
			for (int i = 0; i < mInvalidAssetNames.Count && !worker.CancellationPending; i++)
			{
				// A new name that is null is a canceled import
				string newAssetName = mNewAssetNames[i];

				string message = "Importing:\n" +
								 "     " + newAssetName;
				worker.ReportProgress(i * 100 / mInvalidAssetNames.Count, message);

				if (!String.IsNullOrEmpty(newAssetName))
				{
					// Create pure import source files array
					ArrayList importList = new ArrayList();
					importList.Add(mInvalidAssetNames[i].mImportFilename);

					Debug.Write(newAssetName, "\nImport");
					// Import this asset with its properties
					MOG_ControllerAsset.CreateAsset(newAssetName, "", importList, null, mNewAssetProperties[i], false, false);
				}
			}
		}

		private static bool ImportWithWizard(string[] sourceFullNames, out DialogResult result)
        {
            ImportAssetWizard wizard;

			List<ImportFile> remove = new List<ImportFile>();

			try
			{
				// Loop through each filename to be imported, and import them
				for (int f = 0; f < mInvalidAssetNames.Count; f++)
				{
					// Launch the wizard
					wizard = new ImportAssetWizard();

					// Set the wizard startup variables
					wizard.ImportSourceFilename = mInvalidAssetNames[f].mImportFilename;
					wizard.ImportPotentialMatches = mInvalidAssetNames[f].mPotentialFileMatches;
					wizard.ImportHasMultiples = mInvalidAssetNames.Count > 1;

					// Show the form
					wizard.ShowDialog(MogMainForm.MainApp);
					result = wizard.DialogResult;

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
							if (MOG_ControllerAsset.CreateAsset(wizard.ImportEndMogTextBox.Text, "", importList, null, wizard.ImportPropertyArray, false, false) != null)
							{
								remove.Add(mInvalidAssetNames[f]);
							}

							// Has the user elected to import all the remaining files with the same settings?
							if (wizard.ImportHasMultiples && wizard.ImportMultipleApplyToAll)
							{
								List<ImportFile> remainingItems = new List<ImportFile>(mInvalidAssetNames);
								remainingItems.RemoveRange(0, f);

								List<Object> args = new List<object>();
								args.Add(remainingItems);
								args.Add(wizard.ImportShowExtension);
								args.Add(wizard.ImportFinalMOGFilename.GetAssetClassification());
								args.Add(wizard.ImportFinalMOGFilename.GetAssetPlatform());
								args.Add(wizard.ImportPropertyArray);

								ProgressDialog creationProgress = new ProgressDialog("Importing Asset", "Please wait while the remaining items are imported.", ImportRemainingItems_Worker, args, true);
								creationProgress.ShowDialog(MogMainForm.MainApp);

								// Now break out of this entire loop as we have now just imported all the remaining files to be imported
								break;
							}
						}
						catch (Exception e)
						{
							MOG_Report.ReportMessage("Import", "Could not import the file:\n" + wizard.ImportEndMogTextBox.Text + "\n\nError Message:\n" + e.Message, e.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.ERROR);
							return false;
						}
					}
					else if (result == DialogResult.Cancel)
					{
						result = DialogResult.Cancel;
						return true;
					}
					else if (result == DialogResult.Ignore)
					{
						return true;
					}
				}
			}
			finally
			{
				foreach (ImportFile rem in remove)
				{
					mInvalidAssetNames.Remove(rem);
				}
			}

            result = DialogResult.OK;
            return true;
        }

		private static void ImportRemainingItems_Worker(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			List<Object> args = e.Argument as List<Object>;

			List<ImportFile> remainingItems = args[0] as List<ImportFile>;
			bool useExtension = (bool)args[1];
			string classification = args[2] as string;
			string platform = args[3] as string;
			ArrayList propertyArray = args[4] as ArrayList;

			int itemCount = 0;
			foreach (ImportFile remainingItem in remainingItems)
			{
				// Create the MOG asset name
				string assetLabel = useExtension ? DosUtils.PathGetFileName(remainingItem.mImportFilename) : DosUtils.PathGetFileNameWithoutExtension(remainingItem.mImportFilename);
				MOG_Filename multiFile = MOG_Filename.CreateAssetName(classification, platform, assetLabel);

				// Create our import file list
				ArrayList multiInFiles = new ArrayList();
				multiInFiles.Add(remainingItem.mImportFilename);

				string message = "Importing:\n" +
								 "     " + classification + "\n" +
								 "     " + Path.GetFileName(remainingItem.mImportFilename);
				worker.ReportProgress(itemCount++ * 100 / remainingItems.Count, message);

				// Import the asset
				MOG_ControllerAsset.CreateAsset(multiFile, "", multiInFiles, null, propertyArray, false, false);

				// Check if the user canceled things?
				if (worker.CancellationPending)
				{
					break;
				}
			}
		}

		public static bool ImportAsOne(string[] sourceFilenames, bool looseFileMatching)
		{
			if (sourceFilenames.Length > 0)
			{
				ArrayList propertiesList = new ArrayList();

				List<object> args = new List<object>();
				args.Add(looseFileMatching);
				args.Add(sourceFilenames);

				ProgressDialog progress = new ProgressDialog("Importing asset as single asset", "Please wait while MOG imports the asset.", ImportAsOne_Worker, args, true);
				if (progress.ShowDialog(MogMainForm.MainApp) == DialogResult.OK)
				{
					ArrayList assetNames = progress.WorkerResult as ArrayList;
					string theOneName = "";

					// Check if we failed to find a single match?
					if (assetNames.Count == 1)
					{
						MOG_Filename assetName = assetNames[0] as MOG_Filename;
						theOneName = assetName.GetAssetFullName();
					}
					else
					{
						//None of these files appear to be in the repository already, so we're going to have to do things the hard way
						guiAssetImportCheck check = new guiAssetImportCheck();
						check.CheckImportAssetName(sourceFilenames[0], ref theOneName, ref propertiesList, assetNames);
					}

					// Make sure we found theOneName
					if (!string.IsNullOrEmpty(theOneName))
					{
						args = new List<object>();
						args.Add(theOneName);
						args.Add(sourceFilenames);
						args.Add(propertiesList);

						ProgressDialog creationProgress = new ProgressDialog("Creating Asset", "IMPORTING ASSET: " + theOneName + " with " + sourceFilenames.Length + " files", CreateSingleAsset_Worker, args, false);
						creationProgress.ShowDialog(MogMainForm.MainApp);

						return true;
					}
				}
			}
            
			return false;
		}

		private static void ImportAsOne_Worker(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			List<object> args = e.Argument as List<object>;
			bool looseFileMatching = (bool)args[0];
			string[] sourceFilenames = (string[])args[1];
			HybridDictionary exactAssetNames = new HybridDictionary();
			HybridDictionary looseAssetNames = new HybridDictionary();

			for (int i = 0; i < sourceFilenames.Length && !worker.CancellationPending; i++)
			{
				string filename = sourceFilenames[i];
				ArrayList assetNames = new ArrayList();

				string message = "Mapping:\n" +
								 "     " + Path.GetDirectoryName(filename) + "\n" +
								 "     " + Path.GetFileName(filename);
				worker.ReportProgress(i * 100 / sourceFilenames.Length, message);

				if (DosUtils.DirectoryExistFast(filename))
				{
					// Obtain the list of contained files
					ArrayList containedFiles = DosUtils.FileGetRecursiveList(filename, "*.*");
					if (containedFiles != null)
					{
						// Map these filenames to all the possible assetnames
						assetNames = MOG_ControllerProject.MapFilenamesToAssetNames(containedFiles, "", worker);
					}
				}
				else
				{
					// Map this filename to all possible assetnames
					assetNames = MOG_ControllerProject.MapFilenameToAssetName(filename, "", "");
				}

				// Check if we found some assetNames?
				if (assetNames != null)
				{
					// Check if we are allowing loose matches? and
					// Check if this is a possible loose match?
					if (looseFileMatching &&
						assetNames.Count == 2)
					{
						// Check if this is a blank?
						MOG_Filename assetName = assetNames[1] as MOG_Filename;
						if (assetName.GetFullFilename().Length == 0)
						{
							// Turn this into an exact match
							assetNames.RemoveAt(1);
						}
					}

					// Check if we found an exact match?
					if (assetNames.Count == 1)
					{
						// Add this to our list of exactAssetNames if we haven't already found it
						MOG_Filename assetName = assetNames[0] as MOG_Filename;
						if (!exactAssetNames.Contains(assetName.GetAssetFullName()))
						{
							exactAssetNames[assetName.GetAssetFullName()] = assetName;
						}
					}
					else
					{
						// Add all of the assetNames
						foreach (MOG_Filename assetName in assetNames)
						{
							// Check if this is a blank?
							if (assetName.GetFullFilename().Length == 0)
							{
								continue;
							}

							// Add this to our list of looseAssetNames if we haven't already found it
							if (!looseAssetNames.Contains(assetName.GetAssetFullName()))
							{
								looseAssetNames[assetName.GetAssetFullName()] = assetName;
							}
						}
					}
				}
			}

			// Check if we have any exact matches?
			if (exactAssetNames.Count > 0)
			{
				e.Result = new ArrayList(exactAssetNames.Values);
			}
			else
			{
				// Check if are allowing loose matching?
				if (looseFileMatching)
				{
					e.Result = new ArrayList(looseAssetNames.Values);
				}
				else
				{
					// Add back on the empty entry if it is needed
					if (looseAssetNames.Count == 1)
					{
						looseAssetNames[""] = new MOG_Filename("");
					}

					// Return our list of foundAssets
					e.Result = new ArrayList(looseAssetNames.Values);
				}
			}
		}

		private static void CreateSingleAsset_Worker(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			List<object> args = e.Argument as List<object>;
			string theOneName = args[0] as string;
			string[] sourceFilenames = args[1] as string[];
			ArrayList propertiesList = args[2] as ArrayList;
			
			//use the name of the asset we found, and import all of the assets as that
			ArrayList importFiles = new ArrayList(sourceFilenames);

			MOG_ControllerAsset.CreateAsset(theOneName, "", importFiles, null, propertiesList, false, false);
		}

		/// <summary>
		/// Check if this asset has been imported into mog before.  
		/// If it has get its asset information and import it with those settings
		/// </summary>
		private static void ImportPrevious(string[] sourceFullNames, bool looseFileMatching)
		{
			if (MogMainForm.MainApp != null)
			{
				List<object> args = new List<object>();
				args.Add(looseFileMatching);
				args.Add(sourceFullNames);

				ProgressDialog progress = new ProgressDialog("Importing previous asset", "Please wait while MOG imports the asset...", ImportPrevious_Worker, args, true);
				progress.ShowDialog(MogMainForm.MainApp);
			}
			else
			{
				// Check if asset has been previously imported into the system
				foreach (string sourceFullName in sourceFullNames)
				{
					// Reject these assets back to parent
					ImportFile invalidName = new ImportFile(sourceFullName);
					mInvalidAssetNames.Add(invalidName);
				}
			}
		}

		private static void ImportPrevious_Worker(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			List<object> args = e.Argument as List<object>;
			bool looseMatching = (bool)args[0];
			string[] sourceFullNames = (args[1]) as string[];

			//string[] sourceFullNames = e.Argument as string[];

			mInvalidAssetNames.Clear();
			mNewAssetNames.Clear();
			mNewAssetProperties.Clear();

			// Check if asset has been previously imported into the system
			for (int i = 0; i < sourceFullNames.Length && !worker.CancellationPending; i++)
			{
				string sourceFullName = sourceFullNames[i];
				ArrayList previousSourceFiles = new ArrayList();

				string message = "Importing:\n" +
								 "     " + Path.GetDirectoryName(sourceFullName) + "\n" +
								 "     " + Path.GetFileName(sourceFullName);
				worker.ReportProgress(i * 100 / sourceFullNames.Length, message);
				
				// Check if this is a directory?
				if (DosUtils.DirectoryExistFast(sourceFullName))
				{
					// Obtain the list of contained files
					ArrayList containedFiles = DosUtils.FileGetRecursiveList(sourceFullName, "*.*");
					if (containedFiles != null)
					{
						// Map these filenames to all the possible assetnames
						previousSourceFiles = MOG_ControllerProject.MapFilenamesToAssetNames(containedFiles, MOG_ControllerProject.GetPlatformName(), null);
					}
				}
				else
				{
					// Map this filename to all possible assetnames
					previousSourceFiles = MOG_ControllerProject.MapFilenameToAssetName(sourceFullName, MOG_ControllerProject.GetPlatformName(), MOG_ControllerProject.GetWorkspaceDirectory());
				}

				// Are we loose matching?
				if (looseMatching)
				{
					// Did we get back only 2 files
					if (previousSourceFiles.Count == 2)
					{
						// Is the second one a blank?
						MOG_Filename file = previousSourceFiles[1] as MOG_Filename;
						if (file.GetFullFilename().Length == 0)
						{
							// Then remove it!
							previousSourceFiles.RemoveAt(1);
						}
					}
				}

				if (previousSourceFiles.Count == 1)
				{
					MOG_Filename previousFile = previousSourceFiles[0] as MOG_Filename;

					if (MogMainForm.MainApp.AssetManagerAutoImportCheckBox.Checked)
					{
						// Create the correct controller
						MOG_ControllerAsset.CreateAsset(sourceFullName, previousFile.GetEncodedFilename(), false);
					}
					else
					{
						// Create a new invalid name
						ImportFile invalidName = new ImportFile(sourceFullName);

						// Add all possible matches to this name
						foreach (MOG_Filename potentialMatch in previousSourceFiles)
						{
							// Make sure we have a valid match?
							if (potentialMatch != null &&
								potentialMatch.GetOriginalFilename().Length > 0)
							{
								invalidName.mPotentialFileMatches.Add(potentialMatch);
							}
						}

						// Add to our invalidNames array
						mInvalidAssetNames.Add(invalidName);
					}
				}
				else
				{
					// Create a new invalid name
					ImportFile invalidName = new ImportFile(sourceFullName);

					// Add all possible matches to this name
					foreach (MOG_Filename potentialMatch in previousSourceFiles)
					{
						// Make sure we have a valid match?
						if (potentialMatch != null &&
							potentialMatch.GetOriginalFilename().Length > 0)
						{
							invalidName.mPotentialFileMatches.Add(potentialMatch);
						}
					}

					// Add to our invalidNames array
					mInvalidAssetNames.Add(invalidName);
				}
			}
		}

		private string GetProjectRelationalFileName(string fullFilename)
		{
			if (MOG_ControllerProject.GetCurrentSyncDataController() != null)
			{
				// We want to remove everything in the path up to the project root
				string temp = fullFilename.ToLower();

				// Remove our gameDataPath
				temp = temp.Replace(MOG_ControllerProject.GetCurrentSyncDataController().GetSyncDirectory().ToLower(), "");

				// Remove any starting '\'
				temp = temp.TrimStart("\\".ToCharArray());
				
				// Get the start index for this subString in our source
				int startOfRemoved = fullFilename.ToLower().IndexOf(temp);
				
				// If we found it, remove all these characters from the souce and return
				if (startOfRemoved != -1)
				{
					return fullFilename.Substring(startOfRemoved);
				}
			}

			return "";
		}
	
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sourceFiles"></param>
		/// <returns></returns>
		public bool Rename(ArrayList sourceFiles)
		{
			// Get the new desired name
			RenameAssetForm rename = new RenameAssetForm(sourceFiles);

			if (rename.ShowDialog(MogMainForm.MainApp) == DialogResult.OK)
			{
				// Return the result of our rename algorithm, as executed in the rename form
				return rename.bRenameSuccessful;
			}

			return false;
		}

		public bool Remove(MOG_Filename filename, BackgroundWorker worker)
		{
			Debug.Write(filename.GetOriginalFilename(), "\nClick_Delete");
			MOG_ControllerInbox.Delete(filename);

			// Check if this asset was within our own inbox?
			if (string.Compare(filename.GetUserName(), MOG_ControllerProject.GetUserName_DefaultAdmin(), true) == 0)
			{
				// Check if this was one of our drafts or inbox assets?
				if (filename.IsDrafts() || filename.IsInbox())
				{
					// Add this asset to all of our active workspaces
					if (WorkspaceManager.RemoveAssetFromWorkspaces(filename, true, worker))
					{
					}
				}
			}

			return true;
		}

		public static void BlessAssets_Worker(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			List<object> parameters = e.Argument as List<object>;
			List<MOG_Filename> filenames = parameters[0] as List<MOG_Filename>;
			string comment = parameters[1] as string;
			bool maintainLock = (bool)parameters[2];

			bool bUserAltered = false;
			string loginUser = MOG_ControllerProject.GetUser().GetUserName();
			string activeUser = MOG_ControllerProject.GetActiveUser().GetUserName();

			// Make sure the inbox that we are in matches the logged in user
			if (string.Compare(MOG_ControllerProject.GetUser().GetUserName(), MOG_ControllerProject.GetActiveUser().GetUserName(), true) != 0 )
			{
				// Login as this user so that his bless targets will be respected during this bless!
				MOG_ControllerProject.LoginUser(MOG_ControllerProject.GetActiveUser().GetUserName());
				bUserAltered = true;
			}

			// Obtain a unique bless jobLabel
			string timestamp = MOG_Time.GetVersionTimestamp();
			string jobLabel = "Bless." + MOG_ControllerSystem.GetComputerName() + "." + timestamp;

			// Changed to a for-loop to facilitate the loop breakout box on bless failure below
			for (int assetIndex = 0; assetIndex < filenames.Count; assetIndex++)
			{
				MOG_Filename asset = filenames[assetIndex] as MOG_Filename;
				if (asset != null)
				{
					string message = "Blessing:\n" +
									 "     " + asset.GetAssetClassification() + "\n" +
									 "     " + asset.GetAssetName();
					worker.ReportProgress(assetIndex * 100 / filenames.Count, message);

					// Try to bless each asset and report if there is a failure
					try
					{
						if (MOG_ControllerInbox.BlessAsset(asset, comment, maintainLock, jobLabel, worker))
						{
							WorkspaceManager.MarkLocalAssetBlessed(asset, timestamp);
						}
						else
						{
							// If there are more assets to bless, ask the user how to proceed
							if (assetIndex < filenames.Count - 1)
							{
								MOGPromptResult result = MOG_Prompt.PromptResponse("Bless Error", "An error has occurred while blessing " + asset.GetAssetFullName() + "\nWould you like to continue blessing assets?", MOGPromptButtons.YesNo);
								if (result == MOGPromptResult.Yes)
								{
									// continue with the next asset
									continue;
								}
								else if (result == MOGPromptResult.No)
								{
									// bail
									return;
								}
							}
						}
					}
					catch (Exception ex)
					{
						// Send this Exception back to the server
						MOG_Report.ReportMessage("Bless", ex.Message, ex.StackTrace, MOG_ALERT_LEVEL.CRITICAL);

						// Check if we are logged in an anyone?
						if (MOG_ControllerProject.IsUser())
						{
							// Send a notification to the ofending user
							MOG_Report.ReportMessage("Bless", ex.Message, ex.StackTrace, MOG_ALERT_LEVEL.CRITICAL);
						}
					}
				}
			}

			// Start the job
			MOG_ControllerProject.StartJob(jobLabel);

			// Restore user if changed
			if (bUserAltered)
			{
				MOG_ControllerProject.LoginUser(loginUser);
				MOG_ControllerProject.SetActiveUserName(activeUser);
			}
		}

		public static void ProcessAssets(object parameter)
		{
			List<string> assetNames = parameter as List<string>;
			foreach (string fullName in assetNames)
			{
				Process(fullName);
			}
		}

		public static bool Process(string fullName)
		{
			try
			{
				MOG_ControllerInbox.RipAsset(new MOG_Filename(fullName));
			}
			catch(Exception e)
			{
				MOG_Report.ReportMessage("Process", fullName + "\n\n" + e.Message, e.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.ERROR);
				return false;
			}

			return true;
		}


		public static void UpdateLocal(string name, bool bUserInitiated)
		{
			List<MOG_Filename> filenames = new List<MOG_Filename>();
			filenames.Add(new MOG_Filename(name));

			UpdateLocal(filenames, bUserInitiated);
		}

		public static void UpdateLocal(List<string> names, bool bUserInitiated)
		{
			List<MOG_Filename> filenames = new List<MOG_Filename>();
			foreach (string name in names)
			{
				filenames.Add(new MOG_Filename(name));
			}

			UpdateLocal(filenames, bUserInitiated);
		}

		public static void UpdateLocal(List<MOG_Filename> filenames, bool bUserInitiated)
		{
			bool bStartNewQueue = false;

			lock (mUpdateQueueLock)
			{
				if (mAutoUpdateQueue == null)
				{
					mAutoUpdateQueue = new Queue<MOG_Filename>();
					mUserUpdateQueue = new Queue<MOG_Filename>();

					//Nobody else knows about this queue because we created it
					//We need to make a progressdialog for it because nobody else will
					bStartNewQueue = true;
				}

				lock (mAutoUpdateQueue)
				{
					// Check if this was user initiated?
					if (bUserInitiated)
					{
						//Add the items to the queue
						foreach (MOG_Filename filename in filenames)
						{
							mUserUpdateQueue.Enqueue(filename);
						}
					}
					else
					{
						//Add the items to the queue
						foreach (MOG_Filename filename in filenames)
						{
							mAutoUpdateQueue.Enqueue(filename);
						}
					}
				}
			}

			if (bStartNewQueue)
			{
                // Check if we should inform the user about the update process...did they initiate this action?
                if (bUserInitiated)
                {
                    // Create a new delayed progress dialog
                    ProgressDialog progress = new DelayedProgressDialog("Updating local data", "Please wait while the local data is updated with this asset.", UpdateLocalProgress_Worker, null, true);
                    progress.ShowDialog();
                }
                else
                {
                    // Just do it in a background thread.
                    BackgroundWorker worker = new BackgroundWorker();
                    worker.DoWork += UpdateLocalProgress_Worker;
                    worker.WorkerReportsProgress = true;
                    worker.WorkerSupportsCancellation = true;
                    worker.RunWorkerAsync(null);
                }
			}
		}

		private static void UpdateLocalProgress_Worker(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;

			int autoCount = 0;
			int userCount = 0;

			// Suspend the auto packaging
			WorkspaceManager.SuspendPackaging(true);

			// Begin this GetLatest
			WorkspaceManager.ModifyLocalWorkspace_Begin();

			while (true)
			{
				MOG_Filename filename = new MOG_Filename();
				bool userInitiated = false;

				lock (mAutoUpdateQueue)
				{
					// See if there are any auto update items needing to be processed?
					if (mAutoUpdateQueue.Count > 0)
					{
						filename = mAutoUpdateQueue.Dequeue();
						autoCount++;
					}
					// See if there are any user initiated items needing to be processed?
					else if (mUserUpdateQueue.Count > 0)
					{
						filename = mUserUpdateQueue.Dequeue();
						userCount++;
						userInitiated = true;
					}
				}

				// Recalulate total within this loop because it can be changing outside of us in another thread...
				int total = (mAutoUpdateQueue.Count + autoCount) + (mUserUpdateQueue.Count + userCount);
				string message = "Updating:\n" +
								 "     " + filename.GetAssetClassification() + "\n" +
								 "     " + filename.GetAssetName();
				worker.ReportProgress((autoCount + userCount) * 100 / total, message);

				// Only allow updates of valid assets
				if (filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset ||
					filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Group)
				{
					// Add this asset to all of our active workspaces
					WorkspaceManager.AddAssetToWorkspaces(filename, userInitiated, worker);
				}

				lock (mUpdateQueueLock)
				{
					if (mAutoUpdateQueue.Count == 0 &&
						mUserUpdateQueue.Count == 0)
					{
						mAutoUpdateQueue = null;
						mUserUpdateQueue = null;
						break;
					}

					// Check if the user canceled things?
					if (worker.CancellationPending)
					{
						mAutoUpdateQueue = null;
						mUserUpdateQueue = null;
						break;
					}
				}
			}

			// Restore the auto packaging
			WorkspaceManager.SuspendPackaging(false);

			// End this GetLatest
			WorkspaceManager.ModifyLocalWorkspace_Complete();
		}
		public bool RemoveLocal(string fullName, MOG_ControllerSyncData controller, BackgroundWorker worker)
		{
			MOG_Filename filename = new MOG_Filename(fullName);

			// Add this asset to all of our active workspaces
			if (WorkspaceManager.RemoveAssetFromWorkspaces(filename, worker))
			{
				return true;
			}

			return false;
		}

		public void View(MOG_Filename filename, AssetDirectories AssetDirectoryType, string viewer)
		{
			viewer = viewer.ToLower();

			// Onlcy create viewer processes for real viewers, not none
            if (viewer.Length > 0)
            {
                try
                {
                    viewer = MOG_Tokens.GetFormattedString(viewer, MOG_Tokens.GetProjectTokenSeeds(MOG_ControllerProject.GetProject()));

					// Does this viewer have an extension
					if (Path.GetExtension(viewer).Length == 0)
					{
						// Try tacking on an exe just for good measure
						viewer += ".exe";
					}

					// Check to see if we can find the tool with the path provided
					if (!DosUtils.Exist(viewer))
					{
						try
						{
							string locatedViewer = MOG_ControllerSystem.LocateTool(Path.GetDirectoryName(viewer), Path.GetFileName(viewer));
							if (!String.IsNullOrEmpty(locatedViewer))
							{
								viewer = locatedViewer;
							}
						}
						catch (Exception e)
						{
							MOG_Report.ReportMessage("Located Viewer", e.Message, e.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
						}
					}
                }
                catch (Exception e2)
                {
                    MOG_Report.ReportMessage("Located Viewer", e2.Message, e2.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
                }

                // Get the binary files for this asset
                ArrayList binaryFiles = LocateAssetBinary(filename, AssetDirectoryType);

                if (binaryFiles.Count > 5)
                {
					if (MOG_Prompt.PromptResponse("Asset View", "There are (" + binaryFiles.Count.ToString() + ") files to be viewed in this asset.\n\nShould we continue and launch one viewer per file?", MOGPromptButtons.OKCancel) == MOGPromptResult.Cancel)
                    {
                        return;
                    }
                }

                // Make sure viewer exists
                foreach (string binary in binaryFiles)
                {
                    AssetView assetViewer = new AssetView();
                    assetViewer.Asset = filename;

                    // If we have a viewer, we need to put quotes around our binary so that its arguments line up
                    if (viewer.Length > 0)
                    {
                        assetViewer.Binary = "\"" + binary + "\"";
                        assetViewer.Viewer = viewer;
                        //}
                        //else
                        //{
                        //    // If we don't have a viewer, we will be launching the biniary its self.
                        //    // Therefore, set the binary without quotes
                        //    assetViewer.Binary = binary;
                        //    assetViewer.Viewer = "";
                    }

                    Thread viewerThread = new Thread(new ThreadStart(assetViewer.ShellSpawnWithLock));
                    viewerThread.Start();
                }
            }
            else
            {
                MOG_Report.ReportMessage("View", "No viewer defined for this asset!", "", MOG_ALERT_LEVEL.ALERT);
            }
		}

		public ArrayList LocateAssetBinary(MOG_Filename filename, AssetDirectories AssetDirectoryType)
		{
			string targetDir = "";
			ArrayList binaryFiles = new ArrayList();
			
			MOG_Properties assetProperties = new MOG_Properties(filename);

			switch(AssetDirectoryType)
			{
				case AssetDirectories.IMPORTED:
					targetDir = MOG_ControllerAsset.GetAssetImportedDirectory(assetProperties);
					break;
				case AssetDirectories.PROCESSED:
					string platformName = "";
					// If we have a valid gameDataController?
					if (MOG_ControllerProject.GetCurrentSyncDataController() != null)
					{
						platformName = MOG_ControllerProject.GetCurrentSyncDataController().GetPlatformName();
					}
					targetDir = MOG_ControllerAsset.GetAssetProcessedDirectory(assetProperties, platformName);
					break;
			}				
			if (targetDir.Length != 0 && DosUtils.Exist(targetDir))
			{					
				FileInfo []files = DosUtils.FileGetList(targetDir, "*.*");
				foreach (FileInfo file in files)
				{
					binaryFiles.Add(file.FullName);					
				}
			}
			else
			{
				MOG_Prompt.PromptMessage("Asset View", "Asset (" + targetDir + ") does not exist or is a zero length file! Cannot View.", Environment.StackTrace);
			}

			return binaryFiles;
		}
	}
}
