using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.Threading;

using MOG;
using MOG.INI;
using MOG.TIME;
using MOG.USER;
using MOG.COMMAND;
using MOG.DATABASE;
using MOG.FILENAME;
using MOG.REPORT;
using MOG.PROMPT;
using MOG_Client;
using MOG.DOSUTILS;
using MOG.PROJECT;
using MOG.PLATFORM;
using MOG_Client.Client_Mog_Utilities;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;

using EV.Windows.Forms;
using MOG_ControlsLibrary;
using MOG_ControlsLibrary.Utils;
using MOG_ControlsLibrary.Common.MOG_ContextMenu;
using MOG_Client.Client_Gui.AssetManager_Helper;
using MOG_ControlsLibrary.MogUtils_Settings;

namespace MOG_Client.Client_Gui
{
	/// <summary>
	/// Summary description for guiConnectionManager.
	/// </summary>
	public class guiConnectionManager
	{
		MogMainForm mainForm;
		public ArrayList mListViewSort_Manager;
		public MogControl_AssetContextMenu mConnectionsMenu, mCommandsMenu, mPackageMenu, mPostMenu, mLateResolverMenu;
		public guiAssetManagerGroups mGroups;

		public enum MOGImagesImages {SERVER, CLIENT, SLAVE, EDITOR, COMMANDLINE};
		public enum ConnectionsColumns {MACHINE, IP, ID, TYPE, INFO, FULLNAME};
		public enum CommandsColumns {COMMAND, ASSET, PLATFORM, SLAVE, LABEL, MACHINE, IP, NETWORKID, FULLNAME, COMMANDID};

		public guiConnectionManager(MogMainForm main)
		{
			mainForm = main;

			mListViewSort_Manager = new ArrayList();

			// Initialize the context menus
			mConnectionsMenu = new MogControl_AssetContextMenu("MACHINE, IP, NETWORK, TYPE, INFO, FULLNAME", mainForm.ConnectionsListView);
			mCommandsMenu = new MogControl_AssetContextMenu("COMMAND, ASSET, PLATFORM, SLAVE, LABEL, MACHINE, IP, NETWORKID, FULLNAME, COMMANDID", mainForm.ConnectionManagerCommandsListView);
			mPackageMenu = new MogControl_AssetContextMenu("NAME, CLASSIFICATION, DATE, TARGET PACKAGE, USER, FULLNAME, COMMANDID, LABEL, VERSION", mainForm.ConnectionManagerMergingListView);
			mPostMenu = new MogControl_AssetContextMenu("NAME, CLASSIFICATION, DATE, OWNER, FULLNAME, COMMANDID, LABEL, VERSION", mainForm.ConnectionManagerPostingListView);
			mLateResolverMenu = new MogControl_AssetContextMenu("BROKEN PACKAGE, CLASSIFICATION, DATE, DEPENDANT PACKAGE, OWNER, FULLNAME, COMMANDID, LABEL, VERSION", mainForm.ConnectionManagerLateResolversListView);

			mainForm.ConnectionsListView.ContextMenuStrip = mConnectionsMenu.InitializeContextMenu("{CONNECTIONS}");
			mainForm.ConnectionManagerCommandsListView.ContextMenuStrip = mCommandsMenu.InitializeContextMenu("{COMMANDS}");
			mainForm.ConnectionManagerMergingListView.ContextMenuStrip = mPackageMenu.InitializeContextMenu("{PACKAGECOMMANDS}");
			mainForm.ConnectionManagerPostingListView.ContextMenuStrip = mPostMenu.InitializeContextMenu("{POSTCOMMANDS}");
			mainForm.ConnectionManagerLateResolversListView.ContextMenuStrip = mLateResolverMenu.InitializeContextMenu("{LATERESOLVERCOMMANDS}");

			mainForm.ConnectionsListView.ShowGroups = mainForm.showGroupsToolStripMenuItem.Checked;
			mGroups = new guiAssetManagerGroups();
			mGroups.Add(mainForm.ConnectionsListView, "Type");			
		}

		public void Initialize()
		{
			mainForm.LockManagerLocksListView.Items.Clear();
			mainForm.LockManagerPendingListView.Items.Clear();
			mainForm.ConnectionManagerMergingListView.SmallImageList = MogUtil_AssetIcons.Images;
			mainForm.ConnectionManagerPostingListView.SmallImageList = MogUtil_AssetIcons.Images;
			mainForm.ConnectionManagerLateResolversListView.SmallImageList = MogUtil_AssetIcons.Images;

			// Assets
			mListViewSort_Manager.Add(new ListViewSortManager(mainForm.ConnectionManagerMergingListView, new Type[] {
																														typeof(ListViewTextCaseInsensitiveSort),
																														typeof(ListViewTextCaseInsensitiveSort),
																														typeof(ListViewDateSort),
																														typeof(ListViewTextCaseInsensitiveSort),
																														typeof(ListViewTextCaseInsensitiveSort),
																														typeof(ListViewTextCaseInsensitiveSort),
																														typeof(ListViewTextCaseInsensitiveSort)
																													}));
			
			mListViewSort_Manager.Add(new ListViewSortManager(mainForm.ConnectionManagerPostingListView, new Type[] {
																														typeof(ListViewTextCaseInsensitiveSort),
																														typeof(ListViewTextCaseInsensitiveSort),
																														typeof(ListViewDateSort),
																														typeof(ListViewTextCaseInsensitiveSort)
																													}));
			// Assets
			mListViewSort_Manager.Add(new ListViewSortManager(mainForm.ConnectionManagerLateResolversListView, new Type[] {
																														typeof(ListViewTextCaseInsensitiveSort),
																														typeof(ListViewTextCaseInsensitiveSort),
																														typeof(ListViewDateSort),
																														typeof(ListViewTextCaseInsensitiveSort),
																														typeof(ListViewTextCaseInsensitiveSort),
																														typeof(ListViewTextCaseInsensitiveSort),
																														typeof(ListViewTextCaseInsensitiveSort)
																													}));
		
// JohnRen - Removed because this was fireing again in MOG_TabControl_SelectedIndexChanged()
//			Refresh();
		}

		public void Refresh()
		{
			try
			{
				if(mainForm.ConnectionsListView.Visible)
					mainForm.ConnectionsListView.Items.Clear();

				if(mainForm.ConnectionManagerCommandsListView.Visible)
					mainForm.ConnectionManagerCommandsListView.Items.Clear();

				// Make sure we add the server first
				ListViewItem serverItem = new ListViewItem();
				serverItem.Text = MOG_ControllerSystem.GetServerComputerName();
				serverItem.SubItems.Add(MOG_ControllerSystem.GetServerComputerIP());
				serverItem.SubItems.Add("1");
				serverItem.SubItems.Add("Server");
				serverItem.SubItems.Add("N/A");
				serverItem.SubItems.Add("1");
				serverItem.ForeColor = Color.OrangeRed;
				serverItem.ImageIndex = (int)MOGImagesImages.SERVER;
				mainForm.ConnectionsListView.Items.Add(serverItem);

				mGroups.UpdateGroupItem(mainForm.ConnectionsListView, serverItem, "Type");

				MOG_ControllerSystem.RequestActiveConnections();
				MOG_ControllerSystem.RequestActiveCommands();
			}
			catch(Exception e)
			{
				MOG_Prompt.PromptMessage("Refresh Connection View", e.Message, e.StackTrace, MOG_ALERT_LEVEL.CRITICAL);
				return;
			}

			RefreshMerging();
			RefreshPosting();
			RefreshLateResolvers();
		}

		private ListViewItem LocateItem(int column, string searchString, ListView view)
		{
			foreach (ListViewItem item in view.Items)
			{
				if (item != null && item.SubItems != null)
				{
					if (column >= 0 && column < item.SubItems.Count)
					{
						if (string.Compare(item.SubItems[column].Text, searchString, true) == 0)
						{
							return item;
						}
					}
				}
			}

			return null;
		}

		private ListViewItem LocateCommandItem(MOG_Command command)
		{
			foreach (ListViewItem item in mainForm.ConnectionManagerCommandsListView.Items)
			{
				if (item != null && item.SubItems != null)
				{
					MOG_Command existingCommand = item.Tag as MOG_Command;
					if (existingCommand != null)
					{
						if (existingCommand.GetCommandType() == command.GetCommandType() &&
							string.Compare(existingCommand.GetAssetFilename().GetAssetFullName(), command.GetAssetFilename().GetAssetFullName(), true) == 0 &&
							existingCommand.GetPlatform() == command.GetPlatform() &&
							existingCommand.GetJobLabel() == command.GetJobLabel())
						{
							return item;
						}
					}
				}
			}

			return null;
		}

		public void UpdateConnections(MOG_Command command)
		{
			if (mainForm.ConnectionsListView != null)
			{
				MOG_Command action = command.GetCommand();
				if (action != null)
				{
					// Check if this item already exist in our list?
					ListViewItem item = LocateItem((int)ConnectionsColumns.ID, action.GetNetworkID().ToString(), mainForm.ConnectionsListView);
					if (item == null)
					{
						// Create a new item
						item = new ListViewItem();
						item.Text = action.GetComputerName();
						item.SubItems.Add(action.GetComputerIP());
						item.SubItems.Add(action.GetNetworkID().ToString());
						item.SubItems.Add(action.GetDescription());
						item.SubItems.Add("N/A");
						item.SubItems.Add(action.GetNetworkID().ToString());
						// Add the new item to our list
						mainForm.ConnectionsListView.Items.Add(item);
						mGroups.UpdateGroupItem(mainForm.ConnectionsListView, item, "Type");
					}

					// Make sure we have a valid item
					if (item != null)
					{
						// Update the item's information
						item.SubItems[(int)ConnectionsColumns.INFO].Text = GetClientStatusDescription(action);
					}

					// Identify the desired icon & colors
					switch(action.GetCommandType())
					{
						case MOG_COMMAND_TYPE.MOG_COMMAND_RegisterEditor:
							item.ImageIndex = (int)MOGImagesImages.EDITOR;
							item.ForeColor = Color.SlateGray;
							break;
						case MOG_COMMAND_TYPE.MOG_COMMAND_RegisterSlave:
							item.ImageIndex = (int)MOGImagesImages.SLAVE;
							item.ForeColor = Color.BlueViolet;
							break;
						case MOG_COMMAND_TYPE.MOG_COMMAND_RegisterClient:
						case MOG_COMMAND_TYPE.MOG_COMMAND_ActiveViews:
							// Check what kind of client?
							if (action.GetDescription().Contains("Client"))
							{
								item.ImageIndex = (int)MOGImagesImages.CLIENT;
								item.ForeColor = Color.DarkBlue;
							}
							else if (action.GetDescription().Contains("Server Manager"))
							{
								item.ImageIndex = (int)MOGImagesImages.SERVER;
								item.ForeColor = Color.Firebrick;
							}
							else
							{
								item.ImageIndex = (int)MOGImagesImages.CLIENT;
								item.ForeColor = Color.SteelBlue;
							}
							break;
						case MOG_COMMAND_TYPE.MOG_COMMAND_RegisterCommandLine:
							item.ImageIndex = (int)MOGImagesImages.COMMANDLINE;
							item.ForeColor = Color.Black;
							break;
						case MOG_COMMAND_TYPE.MOG_COMMAND_ShutdownSlave:
						case MOG_COMMAND_TYPE.MOG_COMMAND_ShutdownClient:
						case MOG_COMMAND_TYPE.MOG_COMMAND_ShutdownEditor:
						case MOG_COMMAND_TYPE.MOG_COMMAND_ShutdownCommandLine:
						case MOG_COMMAND_TYPE.MOG_COMMAND_ConnectionKill:
						case MOG_COMMAND_TYPE.MOG_COMMAND_ConnectionLost:
							item.Remove();
							break;
					}
				}
			}
		}
		
		public string GetClientStatusDescription(MOG_Command command)
		{
			// Describe the current command of this connection
			string description = "";


			// Check if this was a registered slave reporting?
			switch (command.GetCommandType())
			{
				case MOG_COMMAND_TYPE.MOG_COMMAND_RegisterSlave:
					description = "Idle";

					// Get the contained command this slave is working on
					MOG_Command slaveTask = command.GetCommand();
					if (slaveTask != null)
					{
						// Check if we have a ProjectName specified?
						if (slaveTask.GetProject().Length > 0)
						{
							// Add on our ProjectName
							description = "PROJECT: " + slaveTask.GetProject();
						}

						// Check if there is an encoded platform in this command?
						if (slaveTask.GetPlatform().Length > 0)
						{
							// Get the slave's active platform
							description += "     PLATFORM: " + slaveTask.GetPlatform();
						}

						// Check if we have a UserName specified?
						if (slaveTask.GetUserName().Length > 0)
						{
							// Add on our ProjectName
							description += "     USER: " + slaveTask.GetUserName();
						}

						// Get the slave's active command
						description += "     COMMAND: " + slaveTask.ToString();

						// Make sure we are in the same project
						if (string.Compare(slaveTask.GetProject(), MOG_ControllerProject.GetProjectName(), true) == 0)
						{
							// Check if we have an asset specified?
							if (slaveTask.GetAssetFilename().GetAssetName().Length > 0)
							{
								// Build the slave's description
								description += "     ASSET: " + slaveTask.GetAssetFilename().GetAssetName();
							}
						}
					}
					break;

				default:
					// Check if we have a ProjectName specified?
					if (command.GetProject().Length > 0)
					{
						// Add on our ProjectName
						description += "PROJECT: " + command.GetProject();
					}
					// Check if we have a ProjectName specified?
					if (command.GetBranch().Length > 0)
					{
						// Add on our ProjectName
						description += "     BRANCH: " + command.GetBranch();
					}
					// Check if we have a UserName specified?
					if (command.GetUserName().Length > 0)
					{
						// Add on our ProjectName
						description += "     USER: " + command.GetUserName();
					}
					// Check if we have a Tab specified?
					if (command.GetTab().Length > 0)
					{
						description += "     TAB: " + command.GetTab();
					}
					// Check if we have a Platform specified?
					if (command.GetPlatform().Length > 0)
					{
						description += "     PLATFORM: " + command.GetPlatform();
					}
					// Check if we have a Tab specified?
					if (command.GetUserName().Length > 0)
					{
						description += "     INBOX: " + command.GetUserName();
					}
					break;
			}

			return description;
		}

		public void UpdateCommands(MOG_Command command)
		{
			ListViewItem item;
			MOG_Command action = command.GetCommand();
			if (action != null)
			{
				bool bAdd = false;
				bool bRemove = false;

				switch (action.GetCommandType())
				{
					case MOG_COMMAND_TYPE.MOG_COMMAND_None:
					case MOG_COMMAND_TYPE.MOG_COMMAND_ConnectionKill:
					case MOG_COMMAND_TYPE.MOG_COMMAND_ConnectionLost:
					case MOG_COMMAND_TYPE.MOG_COMMAND_ConnectionNew:
					case MOG_COMMAND_TYPE.MOG_COMMAND_InstantMessage:
					case MOG_COMMAND_TYPE.MOG_COMMAND_LaunchSlave:
					case MOG_COMMAND_TYPE.MOG_COMMAND_LockCopy:
					case MOG_COMMAND_TYPE.MOG_COMMAND_LockMove:
					case MOG_COMMAND_TYPE.MOG_COMMAND_LockReadRelease:
					case MOG_COMMAND_TYPE.MOG_COMMAND_LockReadRequest:
					case MOG_COMMAND_TYPE.MOG_COMMAND_LockWriteRelease:
					case MOG_COMMAND_TYPE.MOG_COMMAND_LockWriteRequest:
					case MOG_COMMAND_TYPE.MOG_COMMAND_LoginProject:
					case MOG_COMMAND_TYPE.MOG_COMMAND_LoginUser:
					case MOG_COMMAND_TYPE.MOG_COMMAND_NetworkBroadcast:
					case MOG_COMMAND_TYPE.MOG_COMMAND_NewBranch:
					case MOG_COMMAND_TYPE.MOG_COMMAND_NotifyActiveCommand:
					case MOG_COMMAND_TYPE.MOG_COMMAND_NotifyActiveConnection:
					case MOG_COMMAND_TYPE.MOG_COMMAND_NotifyActiveLock:
					case MOG_COMMAND_TYPE.MOG_COMMAND_NotifySystemAlert:
					case MOG_COMMAND_TYPE.MOG_COMMAND_NotifySystemError:
					case MOG_COMMAND_TYPE.MOG_COMMAND_NotifySystemException:
					case MOG_COMMAND_TYPE.MOG_COMMAND_RegisterClient:
					case MOG_COMMAND_TYPE.MOG_COMMAND_RegisterCommandLine:
					case MOG_COMMAND_TYPE.MOG_COMMAND_RegisterEditor:
					case MOG_COMMAND_TYPE.MOG_COMMAND_RegisterSlave:
					case MOG_COMMAND_TYPE.MOG_COMMAND_RequestActiveCommands:
					case MOG_COMMAND_TYPE.MOG_COMMAND_RequestActiveConnections:
					case MOG_COMMAND_TYPE.MOG_COMMAND_ShutdownClient:
					case MOG_COMMAND_TYPE.MOG_COMMAND_ShutdownCommandLine:
					case MOG_COMMAND_TYPE.MOG_COMMAND_ShutdownEditor:
					case MOG_COMMAND_TYPE.MOG_COMMAND_ShutdownSlave:
					case MOG_COMMAND_TYPE.MOG_COMMAND_RefreshApplication:
					case MOG_COMMAND_TYPE.MOG_COMMAND_RefreshTools:
					case MOG_COMMAND_TYPE.MOG_COMMAND_RefreshProject:
					case MOG_COMMAND_TYPE.MOG_COMMAND_ViewConnection:
					case MOG_COMMAND_TYPE.MOG_COMMAND_ViewLock:
					case MOG_COMMAND_TYPE.MOG_COMMAND_ActiveViews:
					case MOG_COMMAND_TYPE.MOG_COMMAND_ViewUpdate:
						// Eat these commands, we don't need to show them in this window
						break;

					case MOG_COMMAND_TYPE.MOG_COMMAND_Complete:
						// Drill one more level into this Complete command
						action = action.GetCommand();
						bRemove = true;
						break;

					// All other commands can simply be added
					default:
						bAdd = true;
						break;
				}

				// Check if we are removing the command?
				if (bRemove)
				{
					// Strip out any matching commands
					do
					{
						// Find it using specific information
//						item = LocateItem((int)CommandsColumns.COMMANDID, action.GetCommandID().ToString(), mainForm.ConnectionManagerCommandsListView);
						item = LocateCommandItem(action);
						if (item != null)
						{
							item.Remove();
						}
					} while(item != null);
				}

				// Check if we are adding the command?
				if (bAdd)
				{
					// Find it using a generic approach
					item = LocateCommandItem(action);
					if (item != null)
					{
						// Check if this could replace an existing command?
						if (action.IsRemoveDuplicateCommands())
						{
							// Remove the duplicate
							item.Remove();
							item = null;
						}
					}

					// Check if the item already exists
					if (item == null)
					{
						//It doesn't already exist, so let's make a new one
						item = new ListViewItem();
						MOG_Time time = new MOG_Time();
						time.SetTimeStamp(command.GetCommandTimeStamp());
						string assetFullName = "PROJECT: " + action.GetProject() + "     ASSET: " + action.GetAssetFilename().GetAssetOriginalFullName();
						if (string.Compare(action.GetProject(), MOG_ControllerProject.GetProjectName(), true) == 0)
						{
							assetFullName = action.GetAssetFilename().GetAssetFullName();
						}

						item.Text = action.ToString();
						item.SubItems.Add(assetFullName);
						item.SubItems.Add(action.GetPlatform());
						item.SubItems.Add(action.IsCompleted() ? "Working" : "");
						item.SubItems.Add(action.GetJobLabel());
						item.SubItems.Add(action.GetComputerName().ToString());
						item.SubItems.Add(action.GetComputerIP().ToString());
						item.SubItems.Add(action.GetNetworkID().ToString());
						item.SubItems.Add(action.GetCommandID().ToString());
						item.Tag = action;

						item.ImageIndex = GetImageIndex(action.GetCommandType());

						mainForm.ConnectionManagerCommandsListView.Items.Add(item);
					}
				}
			}
		}

		public void UpdateConnectionStatus()
		{
			int clientCount = 0;
			int editorCount = 0;
			int slaveCount = 0;
			
			if (mainForm.ConnectionsListView.SelectedItems.Count > 0)
			{
				foreach (ListViewItem item in mainForm.ConnectionsListView.SelectedItems)
				{
					if(item.SubItems[(int)ConnectionsColumns.TYPE].Text.ToLower() == "client")
					{
						clientCount++;
					}
					if(item.SubItems[(int)ConnectionsColumns.TYPE].Text.ToLower() == "slave")
					{
						slaveCount++;
					}
					if(item.SubItems[(int)ConnectionsColumns.TYPE].Text.ToLower() == "editor")
					{
						editorCount++;
					}
				}

				guiStartup.StatusBarUpdate(mainForm, "Selected items:", "CLIENTS: " + clientCount.ToString() + " EDITORS: " + editorCount.ToString() + " SLAVES: " + slaveCount.ToString());
			}
			else
			{
				guiStartup.StatusBarClear(mainForm);
			}
		}

		public void UpdateCommandStatus()
		{
//			int commandCount = 0;
			
			if (mainForm.ConnectionManagerCommandsListView.SelectedItems.Count > 0)
			{
//				foreach (ListViewItem item in mainForm.ConnectionManagerCommandsListView.SelectedItems)
//				{					
//					if(item.SubItems[(int)CommandsColumns.COMMAND].Text.Length > 0)
//					{
//						commandCount++;
//					}
//				}

				guiStartup.StatusBarUpdate(mainForm, "Selected items:", "COMMANDS: " + mainForm.ConnectionManagerCommandsListView.SelectedItems.Count.ToString());
			}
			else
			{
				guiStartup.StatusBarClear(mainForm);
			}
		}

		internal void UpdateMergeStatus()
		{
			if (mainForm.ConnectionManagerMergingListView.SelectedItems.Count > 0)
			{
				guiStartup.StatusBarUpdate(mainForm, "Selected items:", "ASSETS: " + mainForm.ConnectionManagerMergingListView.SelectedItems.Count.ToString());
			}
			else
			{
				guiStartup.StatusBarClear(mainForm);
			}
		}

		internal void UpdatePostStatus()
		{
			if (mainForm.ConnectionManagerPostingListView.SelectedItems.Count > 0)
			{
				guiStartup.StatusBarUpdate(mainForm, "Selected items:", "POSTS: " + mainForm.ConnectionManagerPostingListView.SelectedItems.Count.ToString());
			}
			else
			{
				guiStartup.StatusBarClear(mainForm);
			}
		}

		internal void UpdateLateResolverStatus()
		{
			if (mainForm.ConnectionManagerLateResolversListView.SelectedItems.Count > 0)
			{
				guiStartup.StatusBarUpdate(mainForm, "Selected items:", "LATERESOLVERS: " + mainForm.ConnectionManagerLateResolversListView.SelectedItems.Count.ToString());
			}
			else
			{
				guiStartup.StatusBarClear(mainForm);
			}
		}

		private int GetImageIndex(MOG_COMMAND_TYPE type)
		{
			switch (type)
			{
			// Eat these commands, we don't need to show them in this window
			case MOG_COMMAND_TYPE.MOG_COMMAND_Archive:
				return 0;
			case MOG_COMMAND_TYPE.MOG_COMMAND_AssetRipRequest:
			case MOG_COMMAND_TYPE.MOG_COMMAND_AssetProcessed:
				return 2;
			case MOG_COMMAND_TYPE.MOG_COMMAND_ReinstanceAssetRevision:
			case MOG_COMMAND_TYPE.MOG_COMMAND_Bless:
				return 3;
			case MOG_COMMAND_TYPE.MOG_COMMAND_BuildFull:
				//case MOG_COMMAND_TYPE.MOG_COMMAND_Build:
				return 4;
			case MOG_COMMAND_TYPE.MOG_COMMAND_SlaveTask:
				return 5;
			case MOG_COMMAND_TYPE.MOG_COMMAND_Post:
				return 6;
			
			default:
				return 1;
			}
		}

		private void CreatePackageItem(MOG_DBPackageCommandInfo packageCommand)
		{
			MOG_Filename assetName = new MOG_Filename(packageCommand.mAssetName);
			if (assetName.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
			{
				ListViewItem item = new ListViewItem();

				// "NAME, CLASSIFICATION, DATE, TARGET PACKAGE, USER, FULLNAME, COMMANDID"

				item.Text = assetName.GetAssetLabel();
				//item.SubItems.Add(assetName.GetAssetLabel()); 
				item.SubItems.Add(assetName.GetAssetClassification()); // Class
				item.SubItems.Add(MogUtils_StringVersion.VersionToString(packageCommand.mAssetVersion));
				item.SubItems.Add(packageCommand.mPackageName);
				item.SubItems.Add((packageCommand.mBlessedBy.Length != 0) ? packageCommand.mBlessedBy : packageCommand.mCreatedBy);
				item.SubItems.Add(packageCommand.mAssetName);
				item.SubItems.Add(packageCommand.mID.ToString());
				item.SubItems.Add(packageCommand.mLabel);
				item.SubItems.Add(packageCommand.mAssetVersion);

				item.ImageIndex = MogUtil_AssetIcons.GetAssetIconIndex(packageCommand.mAssetName, null, false);

				mainForm.ConnectionManagerMergingListView.Items.Add(item);
			}
		}

		
		private void CreatePostItem(MOG_DBPostInfo post)
		{
			MOG_Filename assetName = new MOG_Filename(post.mAssetName);
			if (assetName.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
			{
				ListViewItem item = new ListViewItem();
			
				// "NAME, CLASSIFICATION, DATE, OWNER, FULLNAME, COMMANDID"

				item.Text = assetName.GetAssetLabel();
				//item.SubItems.Add(assetName.GetAssetLabel());
				item.SubItems.Add(assetName.GetAssetClassification());
				item.SubItems.Add(MogUtils_StringVersion.VersionToString(post.mAssetVersion));
				item.SubItems.Add(post.mBlessedBy);
				item.SubItems.Add(post.mAssetName);
				item.SubItems.Add(post.mID.ToString());
				item.SubItems.Add(post.mLabel);
				item.SubItems.Add(post.mAssetVersion);
				item.ImageIndex = MogUtil_AssetIcons.GetAssetIconIndex(post.mAssetName, null, false);

				mainForm.ConnectionManagerPostingListView.Items.Add(item);			
			}
		}

		private void CreateLateResolverItem(MOG_DBPackageCommandInfo packageCommand)
		{
			MOG_Filename assetName = new MOG_Filename(packageCommand.mAssetName);
			if (assetName.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
			{
				ListViewItem item = new ListViewItem();

				// "NAME, CLASSIFICATION, DATE, TARGET PACKAGE, OWNER, FULLNAME, COMMANDID, LABEL, VERSION"

				item.Text = assetName.GetAssetLabel();
				item.SubItems.Add(assetName.GetAssetClassification()); // Class
				item.SubItems.Add(MogUtils_StringVersion.VersionToString(packageCommand.mAssetVersion));
				item.SubItems.Add(packageCommand.mPackageName);
				item.SubItems.Add((packageCommand.mBlessedBy.Length != 0) ? packageCommand.mBlessedBy : packageCommand.mCreatedBy);
				item.SubItems.Add(packageCommand.mAssetName);
				item.SubItems.Add(packageCommand.mID.ToString());
				item.SubItems.Add(packageCommand.mLabel);
				item.SubItems.Add(packageCommand.mAssetVersion);

				item.ImageIndex = MogUtil_AssetIcons.GetAssetIconIndex(packageCommand.mAssetName, null, false);

				mainForm.ConnectionManagerLateResolversListView.Items.Add(item);
			}
		}

		
		public void RefreshMerging()
		{
			if (mainForm != null && mainForm.ConnectionManagerMergingListView != null && mainForm.ConnectionManagerMergingListView.Items != null)
			{
				mainForm.ConnectionManagerMergingListView.Items.Clear();

				if (MOG_ControllerProject.GetProject() != null)
				{
					ArrayList packageCommands = MOG.DATABASE.MOG_DBPackageCommandAPI.GetScheduledPackageCommands(MOG_ControllerProject.GetBranchName());

					mainForm.ConnectionManagerMergingListView.BeginUpdate();
					foreach (MOG_DBPackageCommandInfo packageCommand in packageCommands)
					{
						if (packageCommand.mPackageCommandType != MOG_PACKAGECOMMAND_TYPE.MOG_PackageCommand_LateResolver)
						{
							CreatePackageItem(packageCommand);
						}
					}
					mainForm.ConnectionManagerMergingListView.EndUpdate();
				}
			}
		}

		

		public void RefreshPosting()
		{
			if (mainForm != null && mainForm.ConnectionManagerPostingListView != null && mainForm.ConnectionManagerPostingListView.Items != null)
			{
				mainForm.ConnectionManagerPostingListView.Items.Clear();

				if (MOG_ControllerProject.GetProject() != null)
				{
					ArrayList posts = MOG.DATABASE.MOG_DBPostCommandAPI.GetScheduledPosts(MOG_ControllerProject.GetBranchName());

					mainForm.ConnectionManagerPostingListView.BeginUpdate();
					IComparer oldSorter = mainForm.ConnectionManagerPostingListView.ListViewItemSorter;
					mainForm.ConnectionManagerPostingListView.ListViewItemSorter = null;
					foreach (MOG_DBPostInfo post in posts)
					{
						CreatePostItem(post);
					}

					mainForm.ConnectionManagerPostingListView.ListViewItemSorter = oldSorter;
					mainForm.ConnectionManagerPostingListView.EndUpdate();
				}
			}
		}

		public void RefreshLateResolvers()
		{
			if (mainForm != null && mainForm.ConnectionManagerLateResolversListView != null && mainForm.ConnectionManagerLateResolversListView.Items != null)
			{
				mainForm.ConnectionManagerLateResolversListView.Items.Clear();

				if (MOG_ControllerProject.GetProject() != null)
				{
					ArrayList packageCommands = MOG.DATABASE.MOG_DBPackageCommandAPI.GetScheduledPackageCommands(MOG_ControllerProject.GetBranchName());

					mainForm.ConnectionManagerLateResolversListView.BeginUpdate();
					foreach (MOG_DBPackageCommandInfo packageCommand in packageCommands)
					{
						if (packageCommand.mPackageCommandType == MOG_PACKAGECOMMAND_TYPE.MOG_PackageCommand_LateResolver)
						{
							CreateLateResolverItem(packageCommand);
						}
					}
					mainForm.ConnectionManagerLateResolversListView.EndUpdate();
				}
			}
		}
	}
}
