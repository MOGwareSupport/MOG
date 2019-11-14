using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections;
using System.IO;

using MOG;
using MOG.INI;
using MOG.TIME;
using MOG.REPORT;
using MOG.ASSETINFO;
using MOG.FILENAME;
using MOG.DOSUTILS;
 
using MOG_Client.Forms;
using MOG_Client.Client_Utilities;
using MOG_Client.Client_Mog_Utilities.AssetOptions;

namespace MOG_Client.Client_Mog_Utilities
{

	/// <summary>
	/// Summary description for guiPlatformSync.
	/// </summary>
	public class guiPlatformSinc
	{
		private BASE mMog;
		private MogMainForm mainForm;
		
		private string mSyncRoot;
		private string mTargetConsole;
		private string mSourcePath;
		private string mUserName;
		private bool mUseDefaultUser;
		private bool mForce;
		private bool mConsoleCopy;
		private bool mGetFileCount;
		private bool mRunAfterSync;
		private bool mShowSummary;
		private bool mFileMapCreate;
		private int mTotalFilesToCopy, mFileNumber;

		private MOG_Ini mPlatformSync, mTargetTimestamps, mSummary, mPendingCopy;
		private CallbackDialogForm mProgress;
		
		public string mUserSyncFile;
		public string mProjectSyncFile;

		public guiPlatformSinc(string targetConsole, string sourcePath, MogMainForm main, string userName, bool useDefaultUser, bool force, bool runAfterSync, bool showSummary)
		{
			mTargetConsole = targetConsole;
			mSourcePath = sourcePath.ToLower();
			mMog = main.gMog;
			mUserName = userName;
			mUseDefaultUser = useDefaultUser;
			mForce = force;
			mRunAfterSync = runAfterSync;
			mShowSummary = showSummary;

			mUserSyncFile = "None";
			mProjectSyncFile = "Platform." + mMog.GetActivePlatform().mPlatformName + ".sync";
			mConsoleCopy = true;
			mFileMapCreate = false;
			
			mainForm = main;			
		}

		private void RunXbox()
		{
			string output = "";
			string xboxExe = "";
			string Autoplay = "";
			MOG_Ini buttonDefaults = null;

			if (mainForm.gMog.IsProject())
			{
				// Get the project defaults
				string projectDefaultButtonsFile = mainForm.gMog.GetProject().GetProjectToolsPath() + "\\" + mainForm.gMog.GetProject().GetProjectName() + ".Client.Buttons.Default.info";
				if (DosUtils.FileExist(projectDefaultButtonsFile))
				{
					buttonDefaults = new MOG_Ini(projectDefaultButtonsFile);
				}
			}

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
				xboxExe = string.Concat("xE:\\", mMog.GetProject().GetProjectName(), ".", mainForm.gMog.GetUser().GetUserName(), "\\default.xbe", " ", Autoplay);
			}

			// Get the tool listed on the startup page
			string command = "";
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
				command = string.Concat(command.Substring(0, command.IndexOf("[")), mMog.GetActivePlatform().mPlatformTargetPath, command.Substring(command.IndexOf("]")+1));
			}

			// Make sure the tool we need exits
			if (DosUtils.FileExist(command))
			{
				if (guiCommandLine.ShellExecute(command, string.Concat("/x ", mTargetConsole, " ", xboxExe), ProcessWindowStyle.Hidden, ref output) != 0)
				{
					MOG_REPORT.ShowMessageBox("XBox Tools", string.Concat(output), MessageBoxButtons.OK);
				}
			}
			else
			{
				MOG_REPORT.ShowMessageBox("XBox Tools", string.Concat("This tool is missing, have you updated to the latest version?"), MessageBoxButtons.OK);
			}
		}

		public void TargetConsoleRemoveSync()
		{
			// Build a list of exactly what should be on the xbox
			mGetFileCount = true;
			mTotalFilesToCopy = 0;
			bool success = true;

			string CopyFileMap = mSourcePath + "\\MOG\\platformSincMap." + mMog.GetActivePlatform().mPlatformName + ".info";
			if (DosUtils.FileExist(CopyFileMap))
			{
				if (!DosUtils.FileDelete(CopyFileMap))
				{
					return;
				}
			}

			// Create the new map
			mPendingCopy = new MOG_Ini(CopyFileMap);

			InitializeFileMap();
			mFileMapCreate = true;

			// Initialize our progress dialog
			mProgress = new CallbackDialogForm();
			string message = "Preforming platform remove data Sync: \n" + 
				"   Project Sync file map:" + mProjectSyncFile + "\n" +
				"   User Sync file map:" + mUserSyncFile + "\n";
			
			mProgress.DialogInitialize("(" + mTargetConsole + ")Platform Remove Data Sync", message, "");
			Application.DoEvents();
						
			for (int i = 0; i < mPlatformSync.CountKeys("FileMap"); i ++)
			{
				string sourcePath = FormatString(mPlatformSync.GetKeyNameByIndex("FileMap", i).ToLower());
				string targetPath = sourcePath.Replace(mSourcePath, mSyncRoot);
				string filePattern = mPlatformSync.GetKeyByIndex("FileMap", i).ToLower();
				success = SyncDirectories(sourcePath, filePattern, mSyncRoot);

				mProgress.DialogUpdate((i * 100) / mPlatformSync.CountKeys("FileMap"), sourcePath + "\n" + targetPath);
				Application.DoEvents();

				// Create the needed directory on the xbox
				string newDirName = RemapDirectoryString(targetPath);
				if (mConsoleCopy)
				{
					if (!XboxUtils.FileExist(newDirName))
					{
						mPendingCopy.PutSectionString("CREATE_DIR", newDirName);
						mPendingCopy.Save();
					}
				}
				else
				{
					if (!DosUtils.FileExist(newDirName))
					{
						mPendingCopy.PutSectionString("CREATE_DIR", newDirName);
						mPendingCopy.Save();
					}
				}
			}

			if (mConsoleCopy)
			{
				// Logon to correct xbox
				XboxUtils.SetXboxName(mTargetConsole, false);
			}

			mProgress.DialogInitialize("Sync Remove", "Scanning console for non-needed assets", "");

			ArrayList deletableAssets = new ArrayList();
			// Verify the xbox for each of these files
			for (int j = 0; j < mPlatformSync.CountKeys("FileMap"); j++)
			{
				string sourcePath = FormatString(mPlatformSync.GetKeyNameByIndex("FileMap", j).ToLower());
				string targetPath = sourcePath.Replace(mSourcePath, mSyncRoot);
				string walkFiles = XboxUtils.GetFiles(targetPath + "\\");
				
				mProgress.DialogUpdate((j * 100) / mPlatformSync.CountKeys("FileMap"), sourcePath + "\n" + targetPath);
				Application.DoEvents();

				if (walkFiles.Length != 0 && walkFiles.IndexOf(",") != -1)
				{
					string []files = walkFiles.Split(",".ToCharArray());
					for (int k = 0; k < files.Length; k++)
					{
						string targetFile = files[k];

						if (Path.GetExtension(targetFile) != "")
						{
							// Check to see if this file exists in the map
							if (!mPendingCopy.KeyExist("MAP", targetFile))
							{
								// Get the list of deletable assets
								deletableAssets.Add(targetFile);
							}
						}
					}
				}
			}
			
			mProgress.DialogKill();
			mProgress = null;

			if (deletableAssets.Count != 0)
			{
				if (guiConfirmDialog.MessageBoxDialog("Console Remove Sync", "Is is OK to Delete the Following Assets?", deletableAssets, MessageBoxButtons.OKCancel) == DialogResult.OK)
				{
					// Delete the assets in the list from off the xbox
					foreach (string str in guiConfirmDialog.SelectedItems)
					{
						string []parts = str.Split(",;".ToCharArray());
						if (parts != null && parts.Length >=2)
						{
							string assetDeleteName = parts[0];
							XboxUtils.FileDelete(assetDeleteName);						
						}
					}
				}
			}
			else
			{
				MOG_REPORT.ShowMessageBox("Sync Remove", "This target does not have any files that need to be deleted", MessageBoxButtons.OK);
			}
		}

		public void TargetConsoleSync()
		{
			bool success = true;
			string summaryFile = "";

			try
			{
				// Initialize our progress dialog
				mProgress = new CallbackDialogForm();
				
				// Initialize our summery files for reporting what was copied
				summaryFile = InitializeSummaryMap();

				// Initialize our file map to tell us what patterns to copy
				InitializeFileMap();

				// Initialize a timestamp map to help us not copy over the same stuff every time
				InitializeTimeStampMap();
			
				if (mConsoleCopy)
				{
					// Logon to correct xbox
					XboxUtils.SetXboxName(mTargetConsole, false);
				}
							
				// Create initial console directory
				InitializeTargetDirectory();

				// Get the total number of files to be copied
				success = InitializeFileCounts();
			
				string message = "Preforming platform data Sync: \n" + 
					"   Project Sync file map:" + mProjectSyncFile + "\n" +
					"   User Sync file map:" + mUserSyncFile + "\n";
			
				mProgress.DialogInitialize("(" + mTargetConsole + ")Platform Data Sync", message, "Cancel");
				Application.DoEvents();

				// Walk the 'FileMap' and compare to our local game directory and sync to the Xbox
				if (success)
				{
					mGetFileCount = false;
					mFileNumber = 0;
					mProgress.CallbackDialogFilesProgressBar.Maximum = mTotalFilesToCopy + 2;
				
					// Walk the map file directories
					for (int i = 0; i < mPlatformSync.CountKeys("FileMap"); i ++)
					{
						string sourcePath = FormatString(mPlatformSync.GetKeyNameByIndex("FileMap", i).ToLower());
						string targetPath = "";
						string filePattern = mPlatformSync.GetKeyByIndex("FileMap", i).ToLower();

						if (RemapString(sourcePath, ref targetPath))
						{						
							success = SyncDirectories(sourcePath, filePattern, mSyncRoot);

							// Check if the user has canceled
							if (!success)
							{
								break;
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				MOG_REPORT.ShowMessageBox("Console Sync", e.Message.ToString(), MessageBoxButtons.OK);
			}

			// Clean up our progress dialog
			if (mProgress != null)
			{
				mProgress.DialogKill();
				mProgress.CallbackDialogFilesProgressBar.Maximum = 100;
				mProgress = null;
			}

			// Close and save the timestamps file
			if (mTargetTimestamps != null)
			{
				mTargetTimestamps.Save();
				mTargetTimestamps.Close();
			}

			// Close the  platform.sinc.info
			if (mPlatformSync != null)
			{
				mPlatformSync.CloseNoSave();
			}

			// Close the summary file
			if (mSummary != null)
			{
				mSummary.Save();
			}
			
			// Check if we are supposed to launch the game after the sync
			if (mRunAfterSync)
			{
				RunXbox();
			}

			// Show the update summary form
			if (mShowSummary)
			{
				UpdateBuildSummaryForm summary = new UpdateBuildSummaryForm(summaryFile);
				summary.ShowDialog();
			}
		}

		#region Sync methods
		private bool SyncDirectories(string fullFilename, string filePattern, string targetString)
		{
			// Copy Files

			// Make sure our target directory exists.
			CheckDirectory:
				if (!DosUtils.DirectoryExist(fullFilename))
				{
					//Error
					switch (MOG_REPORT.ShowMessageBox("Platform Sync", "Attempted to sync (" + fullFilename + ") but could not find it! \n What should we do?", MessageBoxButtons.AbortRetryIgnore))
					{
						case DialogResult.Ignore:
							break;
						case DialogResult.Retry:
							goto CheckDirectory;
						case DialogResult.Abort:
							return false;
					}				
				}
			
			// Walk all the directories
			DirectoryInfo[] dirs = DosUtils.DirectoryGetList(fullFilename, "*.*");
			if (dirs != null && dirs.Length > 0)
			{
				foreach (DirectoryInfo dir in dirs)
				{
					string targetDirectory = dir.FullName.ToLower().Replace(mSourcePath, mSyncRoot);

					// Copy the files in this directory
					if (!SyncDirectories(dir.FullName, filePattern, targetDirectory))
					{
						return false;
					}					
				}

				// Now do the files in this directory
				return SyncFiles(fullFilename, filePattern);
			}
			else
			{
				return SyncFiles(fullFilename, filePattern);
			}
		}

		private bool SyncFiles(string fullFilename, string filePattern)
		{
			FileInfo[] files = DosUtils.FileGetList(fullFilename, filePattern);
			if (files != null)
			{
				foreach (FileInfo file in files)
				{
					// Replace the source dir of this file with the target dir and call the copy routine
					string targetFile = file.FullName.ToLower().Replace(mSourcePath, mSyncRoot);

					if (!CheckExclusions(file.Name, file.DirectoryName))
					{
						if (mGetFileCount)
						{
							if (!SyncFile(file.FullName, targetFile))
							{
								return false;
							}
						}
						else
						{
							if (!SyncFile(file.FullName, targetFile))
							{
								return false;
							}
						
							// Check if the user cancels
							Application.DoEvents();
							if (mProgress.DialogProcess())
							{
								return false;
							}
						}					
					}
				}				
			}

			return true;
		}

		private bool SyncFile(string fullFilename, string targetFile)
		{
			bool rc = false;

			// Check for special rules for this asset in the ReMap section
			if (!RemapString(fullFilename, ref targetFile))
			{
				return false;
			}

			// Get the file
			FileInfo file = new FileInfo(fullFilename);

			bool fileExists = false;
			if (mConsoleCopy && !mFileMapCreate)
			{
				fileExists = XboxUtils.FileExist(targetFile);
			}
			else if (!mConsoleCopy && !mFileMapCreate)
			{
				fileExists = DosUtils.FileExist(targetFile);
			}
			else
			{
				fileExists = false;
			}

			if (fileExists && !mGetFileCount && mConsoleCopy)
			{
				// Check to see if the sizes of the source and target are the same
				uint targetSize = XboxUtils.GetFileSize(targetFile);
				uint sourceSize = (uint)file.Length;
				if (targetSize != sourceSize)
				{
					fileExists = false;
				}
			}

			// Check if the target file exists
			// If asset is in our timestamp file, it means we have already copied it once.  We need to check if there is a timestamp difference
			// But if the asset does not even exist on the xbox we should force a copy
			if ( fileExists &&
				(mTargetTimestamps.SectionExist("ASSETS") && mTargetTimestamps.KeyExist("ASSETS", file.FullName)) 
				)
			{
				// Get timestamp of target and source						
				// Copy if they are not the same
				if (string.Compare(file.LastWriteTime.ToFileTime().ToString(), mTargetTimestamps.GetString("ASSETS", file.FullName)) != 0)
				{
					if (mGetFileCount)
					{
						mTotalFilesToCopy++;
						mPendingCopy.PutString("COPY_FILE", file.FullName, targetFile);
						mPendingCopy.Save();
					}
					else
					{
						mProgress.DialogUpdate( mFileNumber++ , string.Concat("Source: ", file.FullName, "\nTarget: ", targetFile));
						Application.DoEvents();
						if (mProgress.DialogProcess())
						{
							return false;
						}

						if (mConsoleCopy)
						{
							rc = XboxUtils.FileCopyVerify(file.FullName, targetFile, true);
						}
						else
						{
							rc = DosUtils.FileCopy(file.FullName, targetFile, true);
						}

						if (!rc)
						{
							mSummary.PutString("File.CopyError", file.FullName, targetFile);
							mSummary.Save();

							// Check to see if the reason we failed is because we ran out of space
//							UInt64 freeSpace = XboxUtils.GetDiskFreeSpace(mSyncRoot);
//							if (freeSpace < (ulong)file.Length)
//							{
//								MOG_REPORT.ShowErrorMessageBox("Platform Sync: Out of space!", "There is insufficient space on " + mSyncRoot + " to copy " + file.FullName);
//							}
						}
						else
						{
							mSummary.PutString("File.Copied", file.FullName, targetFile);
							mSummary.Save();
						
							mTargetTimestamps.PutString("ASSETS", file.FullName, file.LastWriteTime.ToFileTime().ToString());
							mTargetTimestamps.Save();
						}
					}
				}
				else
				{
					if (!mGetFileCount)
					{
						// This file is up to date
						mProgress.DialogUpdate( mFileNumber , string.Concat("FILE UP TO DATE!  Skipping...\nSource: ", file.FullName));
						Application.DoEvents();
						// Check if the user cancels
						if (mProgress.DialogProcess())
						{
							return false;
						}
					}
				}
			}
			else
			{
				if (mGetFileCount)
				{
					mTotalFilesToCopy++;
					if (mFileMapCreate)
					{
						// When making a file map, put the target first then the source
						mPendingCopy.PutString("MAP", targetFile, file.FullName);
					}
					else
					{
						// When making a pending file, put the source first then target
						mPendingCopy.PutString("COPY_FILE", file.FullName, targetFile);
					}
					//mPendingCopy.PutString("COPY_FILE", file.FullName, targetFile);
					mPendingCopy.Save();
				}
				else
				{
					mProgress.DialogUpdate( mFileNumber++, string.Concat("Source: ", file.FullName, "\nTarget: ", targetFile));
					Application.DoEvents();
					// Check if the user cancels
					if (mProgress.DialogProcess())
					{
						return false;
					}

					CreateTargetDirectoryPath(targetFile.Substring(0, targetFile.LastIndexOf("\\")));

					// Sync the file to the platform
					try 
					{
						if (mConsoleCopy)rc = XboxUtils.FileCopyVerify(file.FullName, targetFile, true);
						else rc = DosUtils.FileCopy(file.FullName, targetFile, true);
						//rc = XboxUtils.FileCopyVerify(file.FullName, targetFile, true);
					}
					catch(DllNotFoundException e)
					{
						MOG_REPORT.ShowMessageBox("DLL ERROR", e.ToString(), MessageBoxButtons.OK);
						return false;
					}
					catch(EntryPointNotFoundException e)
					{
						MOG_REPORT.ShowMessageBox("DLL ERROR", e.ToString(), MessageBoxButtons.OK);
						return false;
					}
					if (!rc)
					{
						// Error
						mSummary.PutString("File.CopyError", file.FullName, targetFile);
						mSummary.Save();

						// Check to see if the reason we failed is because we ran out of space
						//UInt64 freeSpace = XboxUtils.GetDiskFreeSpace(mSyncRoot);
						//if (freeSpace < (ulong)file.Length)
						//{
						//	MOG_REPORT.ShowErrorMessageBox("Platform Sync: Out of space!", "There is insufficient space on " + mSyncRoot + " to copy " + file.FullName);
						//}

					}
					else
					{

						mSummary.PutString("File.Copied", file.FullName, targetFile);
						mSummary.Save();

						mTargetTimestamps.PutString("ASSETS", file.FullName, file.LastWriteTime.ToFileTime().ToString());
						mTargetTimestamps.Save();
					}
				}
			}
		
			return true;
		}

		#endregion
		#region Initializers
		private bool InitializeTimeStampMap()
		{
			// Get our target root
			string platformTimeStampsFile = string.Concat(mSourcePath, "\\MOG\\platform.", mMog.GetActivePlatform().mPlatformName, ".", Path.GetFileNameWithoutExtension(mTargetConsole), ".timestamps.info");

			// Is this a force
			if (mForce)
			{
				if (DosUtils.FileExist(platformTimeStampsFile))
				{
					DosUtils.FileDelete(platformTimeStampsFile);
				}

				if(mConsoleCopy)
				{
					// Delete the target directory on the xbox
					if (!XboxUtils.DirectoryDelete(mSyncRoot, true))
					{
						// Error
						//MOG_REPORT.ShowMessageBox("Xbox Delete Error", string.Concat(mSyncRoot, " could not be deleted!"), MessageBoxButtons.OK);
						throw new Exception(mSyncRoot + " could not be deleted!");
					}
				}
				else
				{
					if (!DosUtils.DirectoryDelete(mSyncRoot))
					{
						//Error
						//MOG_REPORT.ShowMessageBox("Delete Error", string.Concat(mSyncRoot, " could not be deleted!"), MessageBoxButtons.OK);
						throw new Exception(mSyncRoot + " could not be deleted!");						
					}
				}
			}
			// Open the timestamp ini
			mTargetTimestamps = new MOG_Ini(platformTimeStampsFile);

			return true;
		}

		private string InitializeSummaryMap()
		{
			// Summary filename
			string summaryFile = string.Concat(mSourcePath, "\\MOG\\platformSincSummary.", mMog.GetActivePlatform().mPlatformName, ".info");

			// Clear out the summary file if it exists
			if (DosUtils.FileExist(summaryFile))
			{
				if (!DosUtils.FileDelete(summaryFile))
				{
					throw(new Exception("Could not delete summaryFile:" + summaryFile));
				}
			}
			
			string summaryPendingFile = summaryFile + "." + Path.GetFileNameWithoutExtension(mTargetConsole) + ".pending";
			if (DosUtils.FileExist(summaryPendingFile))
			{
				if (!DosUtils.FileDelete(summaryPendingFile))
				{
					throw(new Exception("Could not delete summaryPendingFile:" + summaryPendingFile));
				}
			}

			mSummary = new MOG_Ini(summaryFile);
			mPendingCopy = new MOG_Ini(summaryPendingFile);

			return summaryFile;
		}

		private bool InitializeFileMap()
		{
			mPlatformSync = new MOG_Ini();

			// Get the platformSinc.info file
			string platformSyncFile = mMog.GetProject().GetProjectToolsPath() + "\\" + mProjectSyncFile;
			if (DosUtils.Exist(platformSyncFile))
			{
				// Open the global sinc file to determine what to sinc
				mPlatformSync.Open(platformSyncFile, FileShare.Read);
				mPlatformSync.SetFilename(mMog.GetUser().GetUserToolsPath() + "\\platformSinc." + mMog.GetActivePlatform().mPlatformName + ".Merge.info", false);
			}

			// Check if the user has a custom sync file
			string userSyncFile = mMog.GetUser().GetUserToolsPath() + "\\" + mUserSyncFile;
			if (DosUtils.FileExist(userSyncFile))
			{
				// Should we force ourselves to use only the user sync file?
				if (string.Compare(mProjectSyncFile, "none", true) == 0)
				{
					mPlatformSync.CloseNoSave();
					mPlatformSync.Open(userSyncFile, FileShare.Read);
				}
				else
				{
					// Lets merge the two then
					mPlatformSync.PutFile(userSyncFile);
				}
			}

			// Make sure we got 'a' Map file loaded
			if (mPlatformSync.GetFilename().Length > 0)
			{
				// Is this a local sync
				if (Path.IsPathRooted(mTargetConsole))
				{
					string root = "";
					// Get our local directory root path
					// Get our console root path
					if (mUseDefaultUser)
					{
						root = FormatString(mPlatformSync.GetString(mMog.GetActivePlatform().mPlatformName, "SpecialRoot").ToLower());
					}
					else
					{
						root = FormatString(mPlatformSync.GetString(mMog.GetActivePlatform().mPlatformName, "Root").ToLower());
					}

					// Fix up the pc console name
					mSyncRoot = mTargetConsole + "\\" + Path.GetFileNameWithoutExtension(root);
					mConsoleCopy = false;
				}
				else
				{
					// Get our console root path
					if (mUseDefaultUser)
					{
						mSyncRoot = FormatString(mPlatformSync.GetString(mMog.GetActivePlatform().mPlatformName, "SpecialRoot").ToLower());
					}
					else
					{
						mSyncRoot = FormatString(mPlatformSync.GetString(mMog.GetActivePlatform().mPlatformName, "Root").ToLower());
					}
				}
			}
			else
			{
				throw(new Exception("Valid platform sync file never properly loaded!"));
			}

			return true;
		}

		private bool InitializeTargetDirectory()
		{
			if (mConsoleCopy)
			{
				if(!XboxUtils.FileExist(mSyncRoot))
				{
					if (!XboxUtils.DirectoryCreateVerify(mSyncRoot, true))
					{
						// Error
						//MOG_REPORT.ShowMessageBox("Xbox Dir Error", string.Concat(mSyncRoot, " could not be created!"), MessageBoxButtons.OK);
						throw new Exception(mSyncRoot + " could not be created!");
					}
				}
			}
			else
			{
				// Create the initial directory on the pc
				if(!DosUtils.Exist(mSyncRoot))
				{
					try
					{
						Directory.CreateDirectory(mSyncRoot);
					}
					catch(Exception e)
					{
						// Error
						MOG_REPORT.ShowMessageBox("Dir Error", string.Concat(mSyncRoot, " could not be created!", "\n", e.ToString()), MessageBoxButtons.OK);
						throw new Exception(mSyncRoot + " could not be created!", e);
						//return false;
					}
				}
			}

			return true;
		}
		private bool InitializeFileCounts()
		{
			bool success = true;
			mGetFileCount = true;
			mTotalFilesToCopy = 0;

			mProgress.DialogInitialize("(" + mTargetConsole + ")Platform Data Sync", "Calculating total files to be copied:\nThis may take a few seconds...", "");
			
			for (int i = 0; i < mPlatformSync.CountKeys("FileMap"); i ++)
			{
				string sourcePath = FormatString(mPlatformSync.GetKeyNameByIndex("FileMap", i).ToLower());
				string targetPath = sourcePath.Replace(mSourcePath, mSyncRoot);
				string filePattern = mPlatformSync.GetKeyByIndex("FileMap", i).ToLower();
				success = SyncDirectories(sourcePath, filePattern, mSyncRoot);

				mProgress.DialogUpdate((i * 100) / mPlatformSync.CountKeys("FileMap"), "Scanning:\n" + targetPath);
				Application.DoEvents();
				
				// Create the needed directory on the xbox
				string newDirName = RemapDirectoryString(targetPath);
				if (mConsoleCopy)
				{
					if (!XboxUtils.FileExist(newDirName))
					{
						mPendingCopy.PutSectionString("CREATE_DIR", newDirName);
						mPendingCopy.Save();
					}
					else
					{
						mPendingCopy.PutSectionString("DIR_EXISTS", newDirName);
						mPendingCopy.Save();
					}
				}
				else
				{
					if (!DosUtils.FileExist(newDirName))
					{
						mPendingCopy.PutSectionString("CREATE_DIR", newDirName);
						mPendingCopy.Save();
					}
				}

				// Check if the user has canceled
				if (!success)
				{
					throw new Exception("User canceled the opperation");
				}
			}

			return true;
		}
		#endregion
		#region Utility methods
		private bool CreateTargetDirectoryPath(string targetPath)
		{
			// Check for a sub-dir
			int index = targetPath.LastIndexOf("\\");
			if (index != -1 && targetPath[index-1] != ':')
			{
				string RootDir = targetPath.Substring(0, index);
				if (!CreateTargetDirectoryPath(RootDir))
				{
					return false;
				}
			}
	
			if (mConsoleCopy)
			{
				// Check if it already exists
				if (!XboxUtils.FileExist(targetPath))
				{
					//
					if (!XboxUtils.DirectoryCreateVerify(targetPath, false))
					{
						// Error
						MOG_REPORT.ShowMessageBox("Create Target Directory Path", string.Concat(RemapDirectoryString(targetPath), " could not be created!"), MessageBoxButtons.OK);
						return false;
					}
					
					XboxUtils.DM_FILE_ATTRIBUTES att = new XboxUtils.DM_FILE_ATTRIBUTES();
					XboxUtils.GetFileAttributes(targetPath, ref att);
					if (!(att.SizeHigh != 0 && att.SizeLow != 0))
					{
						// Error
						MOG_REPORT.ShowMessageBox("Create Target Directory Path", string.Concat(RemapDirectoryString(targetPath), " could not be created!"), MessageBoxButtons.OK);
						return false;
					}
				}
			}
			else
			{
				// Check if it already exists
				if (!DosUtils.FileExist(targetPath))
				{
					//
					try
					{
						Directory.CreateDirectory(targetPath);
					}
					catch(Exception e)
					{
						// Error
						MOG_REPORT.ShowMessageBox("Create Directory Error", string.Concat(RemapDirectoryString(targetPath), " could not be created!", "\n", e.ToString()), MessageBoxButtons.OK);
						return false;
					}						
				}
			}

			return true;
		}

		private bool RemapString(string source, ref string targetFile)
		{
			string remapName;

			source = source.ToLower();

			// Default remap
			targetFile = source.Replace(mSourcePath, mSyncRoot);
			
			// Check for special rules for this asset in the ReMap section
			if (mPlatformSync.SectionExist("ReMap"))
			{
				for (int x = 0; x < mPlatformSync.CountKeys("ReMap"); x++)
				{
					remapName = FormatString(mPlatformSync.GetKeyNameByIndex("ReMap", x).ToLower());

					//Strip the files of their roots
					//string remapNameNoRoot = remapName.Replace(mSourcePath, "");
					//string sourceNoRoot = source.Replace(mSourcePath, "");

					//if (StringUtils.StringCompare(remapNameNoRoot, sourceNoRoot))
					if (StringUtils.StringCompare(remapName, source))
					{
						string wildString = "";

						// Check for wildcards
						if (remapName.IndexOf("*") != -1)
						{
							// Make sure we only have one *
							if (remapName.IndexOf("*", remapName.IndexOf("*")+1) != -1)
							{
								// Error!
								MOG_REPORT.ShowMessageBox(mPlatformSync.GetFilename(), string.Concat("There are more than one '*' in string(", remapName, ")!  This is not allowed!"), MessageBoxButtons.OK);
								return false;
							}
							else
							{
								// If we have a wildcard, replace all target * with the text from the source *
								wildString = FindWildcardString(source, remapName);
							}
						}

						targetFile = mPlatformSync.GetKeyByIndex("Remap", x).ToLower();
						targetFile = FormatString(targetFile);

						// replace * if we had a wildCard
						if (wildString.Length != 0)
						{
							targetFile = targetFile.Replace("*", wildString);
						}

						break;
					}
				}
			}

			return true;
		}

		private string RemapDirectoryString(string source)
		{
			source = source.ToLower();

			// Check for special rules for this asset in the ReMap section
			if (mPlatformSync.SectionExist("ReMap"))
			{
				for (int x = 0; x < mPlatformSync.CountKeys("ReMap"); x++)
				{
					string remapName = FormatString(mPlatformSync.GetKeyNameByIndex("ReMap", x).ToLower());

					//Strip the files of their roots
					string remapNameNoRoot = remapName.Replace(mSourcePath, "");
					string sourceNoRoot = source.Replace(mSyncRoot, "");

					// Remove the extension if there is one
					if (remapNameNoRoot.IndexOf(".") > -1 && remapNameNoRoot.IndexOf(".") + 4 == remapNameNoRoot.Length)
					{
						remapNameNoRoot = remapNameNoRoot.Substring(0, remapNameNoRoot.LastIndexOf("\\"));
					}

					if (string.Compare(remapNameNoRoot, sourceNoRoot) == 0)
					{
						string targetFile = mPlatformSync.GetKeyByIndex("Remap", x).ToLower();
						targetFile = FormatString(targetFile);
						string targetFileNoRoot = targetFile.Replace(mSyncRoot, "");

						// Remove the extension if there is one
						if (targetFileNoRoot.IndexOf(".") > -1 && targetFileNoRoot.IndexOf(".") + 4 == targetFileNoRoot.Length)
						{
							targetFileNoRoot = targetFileNoRoot.Substring(0, targetFileNoRoot.LastIndexOf("\\"));
						}

						targetFile = source.Replace(sourceNoRoot, targetFileNoRoot);
						
						return targetFile;
					}

				}
			}

			return source;
		}

		private string FormatString(string format)
		{
			format = format.ToLower();

			// Replace out any string options {}
			if (format.IndexOf("{projectroot}") != -1)
			{
				format = format.Replace("{projectroot}", mSourcePath);
			}

			// Replace out any string options {}
			if (format.IndexOf("{projectname}") != -1)
			{
				format = format.Replace("{projectname}", mMog.GetProject().GetProjectName());
			}

			// Replace out any string options {}
			if (format.IndexOf("{loginusername}") != -1)
			{
				format = format.Replace("{loginusername}", mUserName);
			}

			// Replace out any string options {}
			if (format.IndexOf("{platformroot}") != -1)
			{
				format = format.Replace("{platformroot}", mSyncRoot);
			}

			return format.ToLower();
		}

		private bool CheckExclusions(string file, string path)
		{
			// Check if this file is in the exclusion list
			if (mPlatformSync.SectionExist("EXCLUSION"))
			{
				for (int j = 0; j < mPlatformSync.CountKeys("EXCLUSION"); j++)
				{
					string exclusionString = mPlatformSync.GetKeyNameByIndex("EXCLUSION", j).ToLower();

					exclusionString = FormatString(exclusionString);
					//Separate exclusion in to path and filename
					string exclusionPath, exclusionFile;
					if (exclusionString.LastIndexOf("\\") != -1)
					{
						exclusionPath = exclusionString.Substring(0, exclusionString.LastIndexOf("\\"));
						exclusionFile = exclusionString.Substring(exclusionString.LastIndexOf("\\")+1);
					}
					else
					{
						exclusionPath = "*";
						exclusionFile = exclusionString;
					}
					
					// Compare paths for a match
					//if (StringUtils.StringCompare(exclusionPath, path))
					if (path.ToLower().IndexOf(exclusionPath.ToLower()) != -1)
					{
						// We will skip all files that match this exclusion
						if (StringUtils.StringCompare(exclusionFile, file))
						{
							return true;
						}
					}
				}
			}

			return false;
		}

		private string FindWildcardString(string source, string targetSource)
		{
			string result = "";
			bool inWildcard = false;
			int c=0;

			CharEnumerator ch = targetSource.ToLower().GetEnumerator();

			// Itterate through the target string looking for the *
			while(ch.MoveNext())
			{
				// When we find it, set our state to be the 'inWildcard' state
				if (ch.Current == '*')
				{
					inWildcard = true;
				}
				else
				{
					if (inWildcard)
					{
						// Now, loop through all the chars of the source till we find a character that is no longer = to the target
						while(ch.Current != source[c])
						{
							// Append each character found to find out what would go inplace of the *
							// I.e. Animations\xbox_adventinterface.ukx
							//      Animations\xbox_*.ukx
							//      = adventinterface
							result = string.Concat(result, source[c++]);
							if (c > source.Length)
							{
								// Error!
								MOG_REPORT.ShowMessageBox("guiPlatformSync::FindWildcardString(string, string)", string.Concat("Source (", source, ") ran out of characters looking for match from (", targetSource, ")!"), MessageBoxButtons.OK);
								return "";
							}
						}
						return result.ToLower();
					}
					else
					{
						c++;
					}
				}
			}
			return result.ToLower();
		}
		#endregion
	}
}
