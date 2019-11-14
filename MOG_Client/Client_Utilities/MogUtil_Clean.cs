
using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Windows.Forms;

using MOG_CoreControls;

using MOG;
using MOG.CONTROLLER.CONTROLLERLIBRARY;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERSYNCDATA;
using MOG.DATABASE;
using MOG.DOSUTILS;
using MOG.FILENAME;
using MOG.PROMPT;
using MOG.PROPERTIES;

using MOG_Client.Client_Mog_Utilities.AssetOptions;

namespace MOG_Client.Utilities
{
	class MogUtil_WorkspaceCleaner
	{
		HybridDictionary mKnownSyncFiles = new HybridDictionary(true);
		ArrayList mKnownSyncFiles_ArrayList = new ArrayList();

		public bool Clean(string startingPath)
		{
			ProgressDialog scanFilesProgress = new ProgressDialog("Scanning local data", "Please wait while MOG scans the local directories...", LocateNonMogAssets_Worker, startingPath, false);
			if (scanFilesProgress.ShowDialog(MogMainForm.MainApp) == DialogResult.OK)
			{
				ArrayList rogueFiles = scanFilesProgress.WorkerResult as ArrayList;
				if (rogueFiles.Count > 0)
				{
					if (guiConfirmDialog.MessageBoxDialog("Remove non-Mog files", "Are you sure you want to remove these files?", startingPath, rogueFiles, null, null, MessageBoxButtons.YesNo) == DialogResult.Yes)
					{
						ProgressDialog progress = new ProgressDialog("Cleaning local data", "Please wait while MOG cleans the local directories...", Clean_Worker, guiConfirmDialog.SelectedItems, true);
						progress.ShowDialog(MogMainForm.MainApp);
						return true;
					}
				}
				else
				{
					MOG_Prompt.PromptMessage("Clean", "No files needed to be cleaned.");
				}
			}

			return false;
		}

		public bool Remove(string startingPath)
		{
			ProgressDialog scanFilesProgress = new ProgressDialog("Scanning local data", "Please wait while MOG scans the local directories...", LocateNonMogAssets_Worker, startingPath, false);
			if (scanFilesProgress.ShowDialog(MogMainForm.MainApp) == DialogResult.OK)
			{
				// Initialize our list of removeFiles to mKnownSyncFiles which was populated with LocateNonMogAssets_Worker
				ArrayList removeFiles = new ArrayList(mKnownSyncFiles.Values);

				ArrayList rogueFiles = scanFilesProgress.WorkerResult as ArrayList;
				if (rogueFiles.Count > 0)
				{
					if (guiConfirmDialog.MessageBoxDialog("Remove non-Mog files", "Are you sure you want to remove these files?", startingPath, rogueFiles, null, null, MessageBoxButtons.YesNo) == DialogResult.Yes)
					{
						// Insert the files that the user has flagged for removal
						removeFiles.InsertRange(0, guiConfirmDialog.SelectedItems);
					}
				}

				// Check if we have something to remove?
				if (removeFiles.Count > 0)
				{
					ProgressDialog progress = new ProgressDialog("Removing local data", "Please wait while MOG removes these files.", Clean_Worker, removeFiles, true);
					progress.ShowDialog(MogMainForm.MainApp);
					return true;
				}
			}

			return false;
		}

		public ArrayList DetectNonMOGFiles(string startingPath)
		{
			ArrayList files = new ArrayList();

			ProgressDialog scanFilesProgress = new ProgressDialog("Detecting Files", "Please wait while MOG scans the local directories...", LocateNonMogAssets_Worker, startingPath, false);
			if (scanFilesProgress.ShowDialog(MogMainForm.MainApp) == DialogResult.OK)
			{
				files = scanFilesProgress.WorkerResult as ArrayList;
			}

			return files;
		}

		private void Clean_Worker(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			ArrayList selectedItems = e.Argument as ArrayList;
			string workspaceDirectory = MOG_ControllerProject.GetWorkspaceDirectory() + "\\";

			for (int i = 0; i < selectedItems.Count && !worker.CancellationPending; i++)
			{
				string file = selectedItems[i] as string;
				string relativeFile = file;
				if (file.StartsWith(workspaceDirectory, StringComparison.CurrentCultureIgnoreCase))
				{
					relativeFile = file.Substring(workspaceDirectory.Length);
				}

				string message = "Deleting:\n" +
								 "     " + Path.GetDirectoryName(relativeFile) + "\n" +
								 "     " + Path.GetFileName(relativeFile);
				worker.ReportProgress(i * 100 / selectedItems.Count, message);

				// Check if this file really exists?
				if (DosUtils.FileExistFast(file))
				{
					if (!DosUtils.Recycle(file))
					{
						// Error
						MOG_Prompt.PromptMessage("Delete File", "Could not delete:\n" + file);
					}
					else
					{
						if (!DosUtils.DirectoryDeleteEmptyParentsFast(Path.GetDirectoryName(file), true))
						{
							if (DosUtils.GetLastError() != null && DosUtils.GetLastError().Length > 0)
							{
								// Error
								MOG_Prompt.PromptMessage("Delete directory", "Could not delete directory:\n" + Path.GetDirectoryName(file));
							}
						}
					}
				}
			}
		}

		private void LocateNonMogAssets_Worker(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			string path = e.Argument as string;

			// Determin our workspaceDirectory and platformName
			string workspaceDirectory = "";
			string relativePath = "";
			string platformName = "";
			// Check if this is a library path?
			if (MOG_ControllerLibrary.GetWorkingDirectory().Length > 0 &&
				path.StartsWith(MOG_ControllerLibrary.GetWorkingDirectory(), StringComparison.CurrentCultureIgnoreCase))
			{
				// Determin proper settings for the library
				workspaceDirectory = MOG_ControllerLibrary.GetWorkingDirectory();
				relativePath = path.Substring(workspaceDirectory.Length).Trim("\\".ToCharArray());
				platformName = "All";

				// Get the list of known MOG files within the workspace
				string classification = MOG_ControllerLibrary.ConstructLibraryClassificationFromPath(path);
				ArrayList containedAssets = MOG_DBAssetAPI.GetAllAssetsByParentClassification(classification);
				foreach (MOG_Filename thisAsset in containedAssets)
				{
					string thisPath = MOG_ControllerLibrary.ConstructPathFromLibraryClassification(thisAsset.GetAssetClassification());
					string thisFilename = Path.Combine(thisPath, thisAsset.GetAssetLabel());
					mKnownSyncFiles.Add(thisFilename, thisFilename);
					mKnownSyncFiles_ArrayList.Add(thisFilename);
				}
			}
			// Check if this is a workspace path?
			else
			{
				workspaceDirectory = MOG_ControllerSyncData.DetectWorkspaceRoot(path);
				if (workspaceDirectory.Length > 0)
				{
					relativePath = path.Substring(workspaceDirectory.Length).Trim("\\".ToCharArray());
					MOG_ControllerSyncData sync = MOG_ControllerProject.GetCurrentSyncDataController();
					if (sync != null)
					{
						platformName = sync.GetPlatformName();

						// Get the list of known MOG files within the workspace
						ArrayList allFiles = MOG_DBAssetAPI.GetAllProjectSyncTargetFilesForPlatform(platformName);
						foreach (string thisFile in allFiles)
						{
							// Trim thisFile just in case it has an extra '\' at the beginning (Needed for BioWare's formulaic sync target booboo)
							string tempFile = thisFile.Trim("\\".ToCharArray());
							// Check if this relative directory matches?
							if (tempFile.StartsWith(relativePath, StringComparison.CurrentCultureIgnoreCase))
							{
								string filename = Path.Combine(workspaceDirectory, tempFile);
								mKnownSyncFiles[filename] = filename;
								mKnownSyncFiles_ArrayList.Add(filename);
							}
						}
					}
				}
			}

			// Continue as long as we got at least one asset from the database
			ArrayList rogueFiles = new ArrayList();
			LocateNonMogAssetsScanFiles(worker, path, rogueFiles, mKnownSyncFiles, path, null);
			e.Result = rogueFiles;
		}

		/// <summary>
		/// Scans the local branches data to locate all binaries that do not have a counterpart in the project database
		/// </summary>
		private bool LocateNonMogAssetsScanFiles(BackgroundWorker worker, string path, ArrayList rogueFiles, HybridDictionary MergeableAssets, string targetProjectPath, MOG_Properties classificationProperties)
		{
			// Check if this is a library path?
			if (MOG_ControllerLibrary.GetWorkingDirectory().Length > 0 &&
				path.StartsWith(MOG_ControllerLibrary.GetWorkingDirectory(), StringComparison.CurrentCultureIgnoreCase))
			{
				// Build us our library classification for this library path
				string classification = MOG_ControllerLibrary.ConstructLibraryClassificationFromPath(path);
				// Check if this library classification exists?
				if (MOG_ControllerProject.IsValidClassification(classification))
				{
					// Obtain the properties for this library classification
					classificationProperties = new MOG_Properties(classification);
				}
			}

			// Get all the files withing this directory
			FileInfo []files = DosUtils.FileGetList(path, "*.*");
			if (files != null)
			{
				// For each file, check to see if we have a DB link for that filename
				foreach(FileInfo file in files)
				{
					if (worker.CancellationPending)
					{
						return false;
					}

					try
					{
						// Check to see if we have a DB link for that filename						
						if (MergeableAssets.Contains(file.FullName) == false)
						{
							// Check if we have a classificationProperties?
							if (classificationProperties != null)
							{
								// Make sure this does not violate the classification filters
								// Check the classification's inclusion filter.
								if (classificationProperties.FilterInclusions.Length > 0)
								{
									FilePattern inclusions = new FilePattern(classificationProperties.FilterInclusions);
									if (inclusions.IsFilePatternMatch(file.FullName) == false)
									{
										// Skip this file as it is not included
										continue;
									}
								}
								// Check the classification's exclusion filter.
								if (classificationProperties.FilterExclusions.Length > 0)
								{
									FilePattern exclusions = new FilePattern(classificationProperties.FilterExclusions);
									if (exclusions.IsFilePatternMatch(file.FullName) == true)
									{
										// Skip this file as it is excluded
										continue;
									}
								}
							}

							// Make sure it is not hidden
							if (Convert.ToBoolean(file.Attributes & FileAttributes.Hidden) == false)
							{
								// If no match was found, add to our list of rogue assets
								rogueFiles.Add(file.FullName);
							}
						}
					}
					catch
					{
						// Needed just in case we have some files larger than 260 in length
					}
				}
			}

			// Now check all these subDirs
			DirectoryInfo []dirs = DosUtils.DirectoryGetList(path, "*.*");
			if (dirs != null)
			{
				foreach(DirectoryInfo dir in dirs)
				{
					// Make sure it is not hidden
					if (Convert.ToBoolean(dir.Attributes & FileAttributes.Hidden) == false)
					{
						// Scan their respective files
						if (!LocateNonMogAssetsScanFiles(worker, dir.FullName, rogueFiles, MergeableAssets, targetProjectPath, classificationProperties))
						{
							return false;
						}
					}
				}
			}

			return true;
		}
	}
}
