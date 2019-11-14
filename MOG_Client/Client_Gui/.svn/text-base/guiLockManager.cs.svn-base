using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;

//using MOG_Server.Server_Gui.guiConfigurationsHelpers;
using MOG_Client.Client_Mog_Utilities.AssetOptions;

using MOG;
using MOG.INI;
using MOG.FILENAME;
using MOG.DATABASE;
using MOG.TIME;
using MOG.COMMAND;
using MOG.CONTROLLER.CONTROLLERSYSTEM;

using MOG_ControlsLibrary.Utils;
using MOG_ControlsLibrary.Common.MOG_ContextMenu;

using EV.Windows.Forms;
using MOG_Client.Client_Gui.AssetManager_Helper;
using MOG_ControlsLibrary.MogUtils_Settings;
using MOG.COMMAND.CLIENT;

namespace MOG_Client.Client_Gui
{
	/// <summary>
	/// Summary description for guiLocks.
	/// </summary>
	public class guiLockManager
	{
		private MogMainForm mainForm;
		private bool mResetLockList;
		public guiAssetManagerGroups mGroups;

		public ArrayList mListViewSort_Manager;
		public MogControl_AssetContextMenu mLocksContextMenu;

		public guiLockManager(MogMainForm main)
		{
			mainForm = main;

			// Initialize context menus
			mLocksContextMenu = new MogControl_AssetContextMenu("LABEL, CLASSIFICATION, USER, DESCRIPTION, MACHINE, IP, ID, TIME, FULLNAME, TYPE", mainForm.LockManagerLocksListView);
			mainForm.LockManagerLocksListView.ContextMenuStrip = mLocksContextMenu.InitializeContextMenu("{LockManager}");

			mListViewSort_Manager = new ArrayList();

			mGroups = new guiAssetManagerGroups();
			mGroups.Add(mainForm.LockManagerLocksListView, "User");
			mainForm.LockManagerLocksListView.ShowGroups = mainForm.showGroupsToolStripMenuItem.Checked;

			mainForm.LockManagerToggleFilterToolStripButton.Checked = MogUtils_Settings.LoadBoolSetting("LockManager", "Filtered", false);
			mainForm.LockManagerFilterToolStripTextBox.Text = MogUtils_Settings.LoadSetting_default("LockManager", "FilterString", "");			
		}

		public void Initialize()
		{			
			mainForm.LockManagerLocksListView.Items.Clear();
			mainForm.LockManagerPendingListView.Items.Clear();
			mainForm.LockManagerLocksListView.SmallImageList = MogUtil_AssetIcons.Images;
			mainForm.LockManagerPendingListView.SmallImageList = MogUtil_AssetIcons.Images;

			// Locks
			mListViewSort_Manager.Add(new ListViewSortManager(mainForm.LockManagerLocksListView, new Type[] {
																												typeof(ListViewTextCaseInsensitiveSort),
																												typeof(ListViewTextCaseInsensitiveSort),
																												typeof(ListViewTextCaseInsensitiveSort),
																												typeof(ListViewTextCaseInsensitiveSort),
																												typeof(ListViewTextCaseInsensitiveSort),
																												typeof(ListViewTextCaseInsensitiveSort),
																												typeof(ListViewInt32Sort),
																												typeof(ListViewDateSort),
																												typeof(ListViewTextCaseInsensitiveSort),
																												typeof(ListViewTextCaseInsensitiveSort),
																												typeof(ListViewTextCaseInsensitiveSort),
																											}));
			// RequestLocks
			mListViewSort_Manager.Add(new ListViewSortManager(mainForm.LockManagerPendingListView, new Type[] {
																												typeof(ListViewTextCaseInsensitiveSort),
																												typeof(ListViewTextCaseInsensitiveSort),
																												typeof(ListViewTextCaseInsensitiveSort),
																												typeof(ListViewTextCaseInsensitiveSort),
																												typeof(ListViewTextCaseInsensitiveSort),
																												typeof(ListViewTextCaseInsensitiveSort),
																												typeof(ListViewInt32Sort),
																												typeof(ListViewDateSort),
																												typeof(ListViewTextCaseInsensitiveSort),
																												typeof(ListViewTextCaseInsensitiveSort),
																												typeof(ListViewTextCaseInsensitiveSort),
																											}));

			// Populate our list of locks
			MOG_CommandClient client = MOG_ControllerSystem.GetCommandManager() as MOG_CommandClient;
			if (client != null)
			{
				// Check if we already have the locks from the server?
				ArrayList locks = client.GetLocks();
				if (locks.Count > 0)
				{
					ArrayList lockItems = new ArrayList();

					// Create the new lock items for each lock
					foreach (MOG_Command lockCommand in locks)
					{
						ListViewItem newLockItem = InitNewLockItem(lockCommand);
						if (newLockItem != null)
						{
							lockItems.Add(newLockItem);
						}
					}

					// Add them all at once because it is much faster
					AddLockItems(lockItems);
				}
				else
				{
					// Looks like we need to request the locks from the server (this is much slower)
					MOG_ControllerSystem.RequestActiveLocks();
				}
			}
		}

		public int FindColumn(string name)
		{
			int x = 0;
			foreach (ColumnHeader column in mainForm.LockManagerLocksListView.Columns)
			{
				if (String.Compare(column.Text, name, true) == 0)
				{
					return x;
				}

				x++;
			}

			return 0;
		}
		
		public void SetResetLocks(bool state)
		{
			mResetLockList = state;
		}

		private ListViewItem InitNewLockItem(MOG_Command command)
		{
			if (command != null)
			{
				if ((command.GetCommandType() == MOG_COMMAND_TYPE.MOG_COMMAND_LockWriteRequest) || (command.GetCommandType() == MOG_COMMAND_TYPE.MOG_COMMAND_LockReadRequest))
				{
					ListViewItem item = new ListViewItem();
					MOG_Time time = new MOG_Time();
					time.SetTimeStamp(command.GetCommandTimeStamp());

					// Gather appropriate lock info from the command's MOG_Filename
					string label = "";
					string classification = "";

					// Check if this is an asset?
					if (command.GetAssetFilename().GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
					{
						// Get the label and classifciation of the asset
						label = command.GetAssetFilename().GetAssetLabel();
						classification = command.GetAssetFilename().GetAssetClassification();

						// Obtain the proper icon
						item.ImageIndex = MogUtil_AssetIcons.GetAssetIconIndex(command.GetAssetFilename().GetOriginalFilename());
					}
					// Check if this is a classification-level by seeing if it ends with a '*'?
					else if (command.GetAssetFilename().GetOriginalFilename().EndsWith("*"))
					{
						// Do our best to illustrate a classification-level lock
						label = "*";
						classification = command.GetAssetFilename().GetOriginalFilename().Trim("*".ToCharArray());

						// Obtain the proper icon
						item.ImageIndex = MogUtil_AssetIcons.GetClassIconIndex(classification);
					}

					//LOCK_COLUMNS {LABEL, CLASSIFICATION, USER, DESCRIPTION, MACHINE, IP, ID, TIME, FULLNAME, TYPE};
					item.Text = label;
					item.SubItems.Add(classification);
					item.SubItems.Add(command.GetUserName());
					item.SubItems.Add(command.GetDescription());
					item.SubItems.Add(command.GetComputerName());
					item.SubItems.Add(command.GetComputerIP());
					item.SubItems.Add(Convert.ToString(command.GetNetworkID()));
					item.SubItems.Add(time.FormatString(""));
					item.SubItems.Add(command.GetAssetFilename().GetOriginalFilename());

					switch (command.GetCommandType())
					{
						case MOG_COMMAND_TYPE.MOG_COMMAND_LockReadRequest:
							item.SubItems.Add("Read Lock");
							item.ForeColor = Color.Green;
							break;
						case MOG_COMMAND_TYPE.MOG_COMMAND_LockWriteRequest:
							item.SubItems.Add("Write Lock");
							item.ForeColor = Color.Red;
							break;
					}

					return item;
				}
			}
			return null;
		}

		private ListViewItem LocateLockItem(MOG_Command command)
		{
			ListViewItem commandItem = InitNewLockItem(command);

			if (commandItem != null)
			{
				foreach (ListViewItem item in mainForm.LockManagerLocksListView.Items)
				{
					if (item.SubItems.Count == commandItem.SubItems.Count)
					{
						bool match = true;
						for (int i = 0; i < item.SubItems.Count; i++)
						{
							if (String.Compare(commandItem.SubItems[i].Text, item.SubItems[i].Text, true) != 0)
							{
								//We have encountered at least one item where the text doesn't match the command, so this is not a match
								match = false;
								break;
							}
						}

						if (match)
						{
							//We went through all the subitems in this item and it totally matches the command, so return it
							return item;
						}
					}
				}
			}

			return null;
		}

		private ListViewItem LocateListViewItem(ListView view, string fullName)
		{
			foreach (ListViewItem item in view.Items)
			{
				string assetName = item.SubItems[FindColumn("Fullname")].Text;

				if (string.Compare(fullName, assetName, true) == 0)
				{
					return item;
				}
			}
			return null;
		}

		public void KillLock(string name, string type, string user, string cpu)
		{
			MOG_ControllerSystem.LockKill(name, type, user, cpu);
		}

		public void RefreshLockWindows(MOG_Command command)
		{
			MOG_Command lockCommand = command.GetCommand();
			//if (string.Compare(mainForm.SMOG_Main_TabControl.SelectedTab.Text, "Locks", true) == 0)
			if (lockCommand != null)
			{
				// If the command completed then it is a successful lock, if not then it is a request
				if (lockCommand.IsCompleted())
				{
					switch(lockCommand.GetCommandType())
					{
					case MOG_COMMAND_TYPE.MOG_COMMAND_LockReadRequest:
					case MOG_COMMAND_TYPE.MOG_COMMAND_LockWriteRequest:
						// Check if this is a lock that was waiting and now has succedded
						ListViewItem oldReleaseLock = LocateListViewItem(mainForm.LockManagerPendingListView, lockCommand.GetAssetFilename().GetOriginalFilename());
						if (oldReleaseLock != null)
						{
							oldReleaseLock.Remove();
						}

						//Put a new item in the list, but only if it doesn't exist in there already
						if (LocateLockItem(lockCommand) == null)
						{
							// Create the new lock item
							ListViewItem newLockItem = InitNewLockItem(lockCommand);
							AddLockItem(newLockItem);
						}

						break;
					case MOG_COMMAND_TYPE.MOG_COMMAND_LockWriteRelease:
					case MOG_COMMAND_TYPE.MOG_COMMAND_LockReadRelease:
						ListViewItem releaseLock = LocateListViewItem(mainForm.LockManagerLocksListView, lockCommand.GetAssetFilename().GetOriginalFilename());
						if (releaseLock != null)
						{
							releaseLock.Remove();
						}
						break;
					}
				}
				else
				{
					if (LocateListViewItem(mainForm.LockManagerPendingListView, lockCommand.GetAssetFilename().GetOriginalFilename()) == null)
					{
						ListViewItem item = InitNewLockItem(lockCommand);

						if (item != null)
						{
							mainForm.LockManagerPendingListView.Items.Add(item);
						}
					}
				}
			}
		}

		private void AddLockItems(ArrayList lockItems)
		{
			// Begin the update so this can happen fast withut updating the GUI!
			mainForm.LockManagerLocksListView.Cursor = Cursors.WaitCursor;
			mainForm.LockManagerLocksListView.BeginUpdate();

			// Add all of the locks
			foreach(ListViewItem newLockItem in lockItems)
			{
				AddLockItem(newLockItem);
			}

			// End the update
			mainForm.LockManagerLocksListView.EndUpdate();
			mainForm.LockManagerLocksListView.Cursor = Cursors.Default;
		}

		private void AddLockItem(ListViewItem newLockItem)
		{
			// Are we filtered
			if (mainForm.LockManagerToggleFilterToolStripButton.Checked)
			{
				// Does the text in the selected column match some part of the filter string?
				string assetName = GetItemName(newLockItem);
				if (assetName.ToUpper().Contains(mainForm.LockManagerFilterToolStripTextBox.Text.ToUpper()))
				{
					// If so, then its ok to add it
					mGroups.UpdateGroupItem(mainForm.LockManagerLocksListView, newLockItem, "User");
					mainForm.LockManagerLocksListView.Items.Add(newLockItem);
				}
			}
			else
			{
				// Non - filtered.  Anything goes
				mGroups.UpdateGroupItem(mainForm.LockManagerLocksListView, newLockItem, "User");
				mainForm.LockManagerLocksListView.Items.Add(newLockItem);
			}
		}

		private string GetItemName(ListViewItem item)
		{
			string name = item.Text;
			for (int i = 0; i < item.SubItems.Count; i++)
			{
				string subName = item.SubItems[i].Text;
				name += " " + subName;
			}

			return name;
		}

		internal void ToggleFilter(bool enabled)
		{
			// Refresh our list
			Initialize();

			// Save our settings
			MogUtils_Settings.SaveSetting("LockManager", "Filtered", enabled.ToString());
		}

		internal void AdjustFilter(string filterString)
		{
			// Are we currently filtering?
			if (mainForm.LockManagerToggleFilterToolStripButton.Checked)
			{
				// Then refresh our list
				Initialize();
			}

			// Save our settings
			MogUtils_Settings.SaveSetting("LockManager", "FilterString", filterString);			
		}
	}
}
