using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Collections.Specialized;
using MOG.CONTROLLER.CONTROLLERSYNCDATA;
using MOG.FILENAME;
using System.ComponentModel;
using MOG.DATABASE;
using MOG.PROMPT;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERASSET;
using MOG.CONTROLLER.CONTROLLERINBOX;
using MOG.ASSET_STATUS;
using MOG.COMMAND;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.DOSUTILS;

namespace MOG_Client.Client_Gui.AssetManager_Helper
{
	static public class WorkspaceManager
	{
		static HybridDictionary mWorkspaces = new HybridDictionary();


		static public MOG_ControllerSyncData AddNewWorkspace(MOG_DBSyncedLocationInfo workspace)
		{
			return AddNewWorkspace(workspace.mTabName, workspace.mWorkingDirectory, workspace.mProjectName, workspace.mBranchName, workspace.mPlatformName, workspace.mUserName);
		}

		static public MOG_ControllerSyncData AddNewWorkspace(string tabName, string workspaceDirectory, string projectName, string  branchName, string platformName, string userName)
		{
			// Make sure we are not already tracking this workspace?
			MOG_ControllerSyncData workspace = GetWorkspace(workspaceDirectory);
			if (workspace == null)
			{
				// Create a new workspace;
				workspace = new MOG_ControllerSyncData(tabName, workspaceDirectory, projectName, branchName, platformName, userName);
				// Add the workspace
				mWorkspaces[workspace.GetSyncDirectory()] = workspace;
			}

			return workspace;
		}

		static public MOG_ControllerSyncData GetWorkspace(string workspaceDirectory)
		{
			// Add the workspace
			return (MOG_ControllerSyncData)mWorkspaces[workspaceDirectory];
		}

		static public bool RemoveWorkspace(MOG_ControllerSyncData workspace)
		{
			return RemoveWorkspace(workspace.GetSyncDirectory());
		}

		static public bool RemoveWorkspace(string workspaceDirectory)
		{
			// Check if we are tracking this workspace?
			if (mWorkspaces.Contains(workspaceDirectory))
			{
				// Remove this workspace
				mWorkspaces.Remove(workspaceDirectory);
				return true;
			}
			return false;
		}

		static public bool RenameWorkspace()
		{
			return false;
		}

		static public bool RegisterEditor(string workspaceDirectory)
		{
			// Get this workspace given the specified workspaceDirectory
			MOG_ControllerSyncData workspace = GetWorkspace(workspaceDirectory);
			if (workspace != null)
			{
				// Check if we were in the middle of a package merge?
				if (workspace.GetLocalPackagingStatus() == MOG_ControllerSyncData.PackageState.PackageState_Busy)
				{
					// Close the merge progress dialog
					workspace.EndPackaging();
				}

				return true;
			}

			return false;
		}

		static public bool ShutdownEditor(string workspaceDirectory)
		{
			// Get this workspace given the specified workspaceDirectory
			MOG_ControllerSyncData workspace = GetWorkspace(workspaceDirectory);
			if (workspace != null)
			{
				// Check if we were in the middle of a package merge?
				if (workspace.GetLocalPackagingStatus() == MOG_ControllerSyncData.PackageState.PackageState_Busy)
				{
					// Close the merge progress dialog
					workspace.EndPackaging();
				}

				return true;
			}

			return false;
		}

		static public void AutoPackage(bool value)
		{
			// Walk through all of our workspaces
			foreach (MOG_ControllerSyncData workspace in mWorkspaces.Values)
			{
				// Check if this is the current workspace?  or
				// Check if this workspace is active?
				if (workspace == MOG_ControllerProject.GetCurrentSyncDataController() ||
					workspace.IsAlwaysActive())
				{
					workspace.AutoPackage();
				}
			}
		}

		static public void SuspendPackaging(bool value)
		{
			// Walk through all of our workspaces
			foreach (MOG_ControllerSyncData workspace in mWorkspaces.Values)
			{
				// Check if this is the current workspace?  or
				// Check if this workspace is active?
				if (workspace == MOG_ControllerProject.GetCurrentSyncDataController() ||
					workspace.IsAlwaysActive())
				{
					workspace.SuspendPackaging(value);
				}
			}
		}

		static public bool EndPackaging(string workspaceDirectory)
		{
			// Make sure we are not already tracking this workspace?
			MOG_ControllerSyncData workspace = GetWorkspace(workspaceDirectory);
			if (workspace != null)
			{
				// Inform this workspace we are finished packaging
				workspace.EndPackaging();

				// Kick start another auto package merge check just in case anything new came in while we were waiting for this package to finish
				workspace.AutoPackage();
				return true;
			}

			return false;
		}

		static public void ModifyLocalWorkspace_Begin()
		{
			// Walk through all of our workspaces
			foreach (MOG_ControllerSyncData workspace in mWorkspaces.Values)
			{
				// Check if this is the current workspace?  or
				// Check if this workspace is active?
				if (workspace == MOG_ControllerProject.GetCurrentSyncDataController() ||
					workspace.IsAlwaysActive())
				{
					workspace.ModifyLocalWorkspace_Begin();
				}
			}
		}

		static public void ModifyLocalWorkspace_Complete()
		{
			// Walk through all of our workspaces
			foreach (MOG_ControllerSyncData workspace in mWorkspaces.Values)
			{
				// Check if this is the current workspace?  or
				// Check if this workspace is active?
				if (workspace == MOG_ControllerProject.GetCurrentSyncDataController() ||
					workspace.IsAlwaysActive())
				{
					workspace.ModifyLocalWorkspace_Complete();
				}
			}
		}

		static public bool AddAssetToWorkspaces(MOG_Filename assetFilename, bool userInitiated, BackgroundWorker worker)
		{
			bool bFailed = false;

			// Open the asset now to save time later so it doesn't need to be opened for each workspace
			MOG_ControllerAsset asset = MOG_ControllerAsset.OpenAsset(assetFilename);
			if (asset != null)
			{
				try
				{
					// Walk through all of our workspaces
					foreach (MOG_ControllerSyncData workspace in mWorkspaces.Values)
					{
						bool bAddAsset = false;

						// Check if this workspace is active?
						if (workspace.IsAlwaysActive())
						{
							bAddAsset = true;
						}
						// Imported asset has special logic
						else if (asset.GetProperties().Status == MOG_AssetStatus.GetText(MOG_AssetStatusType.Imported))
						{
							// Check if this asset originated from within this workspace?  (special case added for SmartBomb so editor will always update the workspace of an object sent from the editor)
							if (DosUtils.PathIsWithinPath(workspace.GetSyncDirectory(), asset.GetProperties().SourcePath))
							{
								bAddAsset = true;
							}
							// Check if the user actually initiated this event?
							else if (userInitiated &&
									 workspace == MOG_ControllerProject.GetCurrentSyncDataController())
							{
								bAddAsset = true;
							}
						}
						// All other assets should simply go into the current
						else if (workspace == MOG_ControllerProject.GetCurrentSyncDataController())
						{
							bAddAsset = true;
						}

						// Should this asset be added?
						if (bAddAsset)
						{
							// Decide if the user wants to be notified about the asset's update
							bool bInformUser = userInitiated;
							// Check if no worker was specified?  or
							// Check if this isn't the active workspace?
							if (worker == null ||
								workspace != MOG_ControllerProject.GetCurrentSyncDataController())
							{
								// Don't bother the user about any problems
								bInformUser = false;
							}

							// Check if we can add this asset to the local workspace?
							if (workspace.CanAddAssetToLocalWorkspace(asset, bInformUser))
							{
								// Check if this asset comming from an inbox?   and
								// Check if this asset comming from our inbox?   and
								// Check if this asset's current state is 'Imported'  and
								// Check if this asset originated from this workspace?
								// Finally, Make sure this wasn't user initiated?
								if (assetFilename.IsWithinInboxes() &&
									string.Compare(assetFilename.GetUserName(), MOG_ControllerProject.GetUserName(), true) == 0 &&
									string.Compare(asset.GetProperties().Status, MOG_AssetStatus.GetText(MOG_AssetStatusType.Imported), true) == 0 &&
									MOG_Filename.IsWithinPath(workspace.GetSyncDirectory(), asset.GetProperties().SourcePath) &&
									!userInitiated)
								{
									// Looks like we can proceed to import
									if (!workspace.AddAssetToLocalUpdatedTray(asset, worker))
									{
										bFailed = true;
									}

									// Continue on to the next asset
									continue;
								}

								// Proceed to add the asset to this workspace
								if (!workspace.AddAssetToLocalWorkspace(asset, worker))
								{
									bFailed = true;
								}
							}
						}
					}
				}
				finally
				{
					asset.Close();
				}
			}
			else
			{
				bFailed = true;
			}

			if (!bFailed)
			{
				return true;
			}
			return false;
		}

		static public bool RemoveAssetFromWorkspaces(MOG_Filename assetFilename, BackgroundWorker worker)
		{
			return RemoveAssetFromWorkspaces(assetFilename, false, worker);
		}

		static public bool RemoveAssetFromWorkspaces(MOG_Filename assetFilename, bool bRespectAutoUpdate, BackgroundWorker worker)
		{
			bool bFailed = false;

			// Walk through all of our workspaces
			foreach (MOG_ControllerSyncData workspace in mWorkspaces.Values)
			{
				// Check if this is the current workspace?  or
				// Check if this workspace is active?
				if (workspace == MOG_ControllerProject.GetCurrentSyncDataController() ||
					workspace.IsAlwaysActive())
				{
					// Check if we have been instructed to respect the auto update flag?
					if (bRespectAutoUpdate)
					{
						// Check if this workspace has the auto update enabled?
						if (!workspace.IsAutoUpdateEnabled())
						{
							// Skip this workspace as it doesn't have the auto update enabled
							continue;
						}
					}

					// Proceed to add the asset to this workspace
					MOG_Filename workspaceAsset = MOG_Filename.GetLocalUpdatedTrayFilename(workspace.GetSyncDirectory(), assetFilename);
					if (!workspace.RemoveAssetFromLocalWorkspace(workspaceAsset, worker))
					{
						bFailed = true;
					}
				}
			}

			if (!bFailed)
			{
				return true;
			}
			return false;
		}

		static public bool RepackageAssetInWorkspaces(MOG_Filename assetFilename, BackgroundWorker worker)
		{
			bool bFailed = false;

			// Walk through all of our workspaces
			foreach (MOG_ControllerSyncData workspace in mWorkspaces.Values)
			{
				// Check if this is the current workspace?  or
				// Check if this workspace is active?
				if (workspace == MOG_ControllerProject.GetCurrentSyncDataController() ||
					workspace.IsAlwaysActive())
				{
					// Proceed to add the asset to this workspace
					MOG_Filename workspaceAsset = MOG_Filename.GetLocalUpdatedTrayFilename(workspace.GetSyncDirectory(), assetFilename);
					if (!workspace.RepackageAssetInLocalWorkspace(workspaceAsset))
					{
						bFailed = true;
					}
				}
			}

			if (!bFailed)
			{
				return true;
			}
			return false;
		}

		public static bool RebuildNetworkPackage(MOG_Filename packageFilename, string jobLabel)
		{
			// Make sure this represents the currently blessed revision of this package
			if (!packageFilename.IsWithinRepository())
			{
				// Obtain the current revision for this specified package
				packageFilename = MOG_ControllerProject.GetAssetCurrentBlessedVersionPath(packageFilename);
			}

			// Now we should be ensured a package located within the repository
			if (packageFilename.IsWithinRepository())
			{
				// Send our Rebuild package command to the server
				MOG_Command rebuildPackageCommand = MOG_CommandFactory.Setup_NetworkPackageRebuild(packageFilename.GetEncodedFilename(), packageFilename.GetAssetPlatform(), jobLabel);
				return MOG_ControllerSystem.GetCommandManager().SendToServer(rebuildPackageCommand);
			}

			return false;
		}

		public static bool RebuildLocalPackage(MOG_Filename packageFilename)
		{
			if (packageFilename.IsLocal())
			{
				// Walk through all of our workspaces
				foreach (MOG_ControllerSyncData workspace in mWorkspaces.Values)
				{
					// Check if this is the current workspace?  or
					// Check if this workspace is active?
					if (workspace == MOG_ControllerProject.GetCurrentSyncDataController() ||
						workspace.IsAlwaysActive())
					{
// We need to know how to handle a local package rebuild differently than a network package rebuild
//						MOG_Command rebuildPackageCommand = MOG_CommandFactory.Setup_PackageRebuild(packageFilename.GetEncodedFilename(), workspace.GetPlatformName(), "");
//						// Send our package merge command to the server
//						return MOG_ControllerSystem.GetCommandManager().SendToServer(rebuildPackageCommand);
					}
				}
			}

			return false;
		}

		public static void MarkLocalAssetBlessed(MOG_Filename assetFilename, string blessedTimestamp)
		{
			// Walk through all of our workspaces
			foreach (MOG_ControllerSyncData workspace in mWorkspaces.Values)
			{
				// Check if this is the current workspace?  or
				// Check if this workspace is active?
				if (workspace == MOG_ControllerProject.GetCurrentSyncDataController() ||
					workspace.IsAlwaysActive())
				{
					// Build the expected path to this asset in our local workspace
					MOG_Filename workspaceAsset = MOG_Filename.GetLocalUpdatedTrayFilename(workspace.GetSyncDirectory(), assetFilename);
					// Open the local asset
					MOG_ControllerAsset localAsset = MOG_ControllerAsset.OpenAsset(workspaceAsset);
					if (localAsset != null)
					{
						// Stamp this locally update asset with the newly blessed revision timestamp
						localAsset.GetProperties().BlessedTime = blessedTimestamp;

						// Change the state of this asset to blessed
						MOG_ControllerInbox.UpdateInboxView(localAsset, MOG_AssetStatusType.Blessed);
						localAsset.Close();
					}
				}
			}
		}

		public static void SwitchCurrentWorkspaceBranch(string newBranchName)
		{
			// Check if this branch is changing?
			if (string.Compare(newBranchName, MOG_ControllerProject.GetCurrentSyncDataController().GetBranchName(), true) != 0)
			{
				// Push the new branch down into the current workspace
				if (MOG_DBSyncedDataAPI.SwitchWorkspaceBranch(  MOG_ControllerSystem.GetComputerName(),
																MOG_ControllerProject.GetCurrentSyncDataController().GetProjectName(),
																MOG_ControllerProject.GetCurrentSyncDataController().GetPlatformName(),
																MOG_ControllerProject.GetCurrentSyncDataController().GetSyncDirectory(),
																MOG_ControllerProject.GetCurrentSyncDataController().GetUserName(),
																newBranchName))
				{
					// Update our loaded workspace
					MOG_ControllerProject.GetCurrentSyncDataController().SetBranchName(newBranchName);
				}
			}
		}
	}
}
