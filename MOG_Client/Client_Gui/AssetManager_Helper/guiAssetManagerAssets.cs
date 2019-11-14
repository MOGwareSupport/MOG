using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.IO;
using System.ComponentModel;

using MOG;
using MOG.INI;
using MOG.USER;
using MOG.DOSUTILS;
using MOG.FILENAME;
using MOG.COMMAND;
using MOG.PROPERTIES;
using MOG.REPORT;
using MOG.PROMPT;
using MOG.CONTROLLER;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.CONTROLLER.CONTROLLERINBOX;
using MOG.CONTROLLER.CONTROLLERASSET;
using MOG.ASSET_STATUS;
using MOG.DATABASE;
using MOG.TIME;
using MOG_Client.Client_Gui;

using MOG_Client.Client_Utilities;
using MOG_Client.Client_Mog_Utilities;
using MOG_Client.Client_Mog_Utilities.AssetOptions;

using MOG_ControlsLibrary;
using MOG_ControlsLibrary.Utils;
using MOG_ControlsLibrary.Common;

namespace MOG_Client.Client_Gui.AssetManager_Helper
{
	/// <summary>
	/// Summary description for Assets.
	/// </summary>
	public class guiAssetManagerAssets
	{
		guiAssetManager mAssetManager;
		BackgroundWorker mWorkerInbox = new BackgroundWorker();
		BackgroundWorker mWorkerDrafts = new BackgroundWorker();
		BackgroundWorker mWorkerOutbox = new BackgroundWorker();

		private enum BOX { INBOX, OUTBOX };

		public guiAssetManagerAssets(guiAssetManager assetManager)
		{
			mAssetManager = assetManager;

			// Initialize all our work handlers
			mWorkerInbox.DoWork += InboxRefresh_DoWork;
			mWorkerInbox.RunWorkerCompleted += InboxRefreshCompleted;
			mWorkerInbox.ProgressChanged += Mailbox_ProgressUpdate;
			mWorkerInbox.WorkerReportsProgress = true;

			mWorkerDrafts.DoWork += InboxRefresh_DoWork;
			mWorkerDrafts.RunWorkerCompleted += DraftsRefreshCompleted;
			mWorkerDrafts.ProgressChanged += Mailbox_ProgressUpdate;
			mWorkerDrafts.WorkerReportsProgress = true;

			mWorkerOutbox.DoWork += OutboxRefresh_DoWork;
			mWorkerOutbox.RunWorkerCompleted += OutboxRefreshCompleted;
			mWorkerOutbox.ProgressChanged += Mailbox_ProgressUpdate;
			mWorkerOutbox.WorkerReportsProgress = true;

			mAssetManager.mainForm.AssetManagerInboxListView.SelectedIndexChanged += AssetboxListView_SelectedIndexChanged;
			mAssetManager.mainForm.AssetManagerDraftsListView.SelectedIndexChanged += AssetboxListView_SelectedIndexChanged;
			mAssetManager.mainForm.AssetManagerSentListView.SelectedIndexChanged += AssetboxListView_SelectedIndexChanged;
			mAssetManager.mainForm.AssetManagerTrashListView.SelectedIndexChanged += AssetboxListView_SelectedIndexChanged;
		}

		void AssetboxListView_SelectedIndexChanged(object sender, EventArgs e)
		{
			ListView view = sender as ListView;
			if (view != null && view.SelectedItems != null && view.SelectedItems.Count > 0)
			{
				MogMainForm main = view.GetContainerControl() as MogMainForm;
				if (main != null)
				{
					guiStartup.StatusBarUpdate(main, "Selected items:", "ASSETS: " + view.SelectedItems.Count.ToString());
				}
			}
			else
			{
				MogMainForm main = view.GetContainerControl() as MogMainForm;
				if (main != null)
				{
					guiStartup.StatusBarClear(main);
				}
			}
		}

		public void RefreshInbox()
		{
			if (!mWorkerInbox.IsBusy)
			{
				// Only do this if we have an active project and user
				if (MOG_ControllerProject.GetProject() != null &&
					MOG_ControllerProject.GetActiveUser() != null)
				{
					mAssetManager.mainForm.Cursor = Cursors.WaitCursor;
					mWorkerInbox.RunWorkerAsync("InBox");
				}
			}
		}

		public void RefreshDrafts()
		{
			if (!mWorkerDrafts.IsBusy)
			{
				// Only do this if we have an active project and user
				if (MOG_ControllerProject.GetProject() != null &&
					MOG_ControllerProject.GetActiveUser() != null)
				{
					mAssetManager.mainForm.Cursor = Cursors.WaitCursor;
					mWorkerDrafts.RunWorkerAsync("Drafts");
				}
			}
		}

		public void RefreshOutbox()
		{
			if (!mWorkerOutbox.IsBusy)
			{
				// Only do this if we have an active project and user
				if (MOG_ControllerProject.GetProject() != null &&
					MOG_ControllerProject.GetActiveUser() != null)
				{
					mAssetManager.mainForm.Cursor = Cursors.WaitCursor;
					mWorkerOutbox.RunWorkerAsync("Outbox");
				}
			}
		}

		private void InboxRefresh_DoWork(object sender, DoWorkEventArgs e)
		{
			// Get the files in the inbox\contents.info 
			e.Result = RefreshUsersBox(sender as BackgroundWorker, e.Argument as string, mAssetManager.mInboxAssetsDirectory);
		}

		private void OutboxRefresh_DoWork(object sender, DoWorkEventArgs e)
		{
			// Get the files in the inbox\contents.info 
			e.Result = RefreshUsersBox(sender as BackgroundWorker, e.Argument as string, mAssetManager.mOutboxAssetsDirectory);
		}

		private void Mailbox_ProgressUpdate(object sender, ProgressChangedEventArgs e)
		{
			if (e.UserState is string)
			{
				guiStartup.StatusBarUpdate(mAssetManager.mainForm, "Building", e.UserState as string);
			}
		}

		private void InboxRefreshCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			lock (mAssetManager)
			{
				mAssetManager.mInboxSortManager.SortEnabled = false;
				mAssetManager.mainForm.AssetManagerInboxListView.Items.Clear();
				mAssetManager.mainForm.AssetManagerInboxListView.Items.AddRange(e.Result as ListViewItem[]);
				mAssetManager.mInboxSortManager.SortEnabled = true;
				mAssetManager.mainForm.Cursor = Cursors.Default;

				// Make sure all the tabs have updated text
				mAssetManager.UpdateAssetManagerInboxTabText(true);

				mAssetManager.mGroups.UpdateGroups(mAssetManager.mainForm.AssetManagerInboxListView);

				guiStartup.StatusBarClear(mAssetManager.mainForm);
			}
		}

		private void DraftsRefreshCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			lock (mAssetManager)
			{
				mAssetManager.mDraftsSortManager.SortEnabled = false;
				mAssetManager.mainForm.AssetManagerDraftsListView.Items.Clear();
				mAssetManager.mainForm.AssetManagerDraftsListView.Items.AddRange(e.Result as ListViewItem[]);
				mAssetManager.mDraftsSortManager.SortEnabled = true;
				mAssetManager.mainForm.Cursor = Cursors.Default;

				// Make sure all the tabs have updated text
				mAssetManager.UpdateAssetManagerDraftsTabText(true);

				mAssetManager.mGroups.UpdateGroups(mAssetManager.mainForm.AssetManagerDraftsListView);

				guiStartup.StatusBarClear(mAssetManager.mainForm);
			}
		}

		private void OutboxRefreshCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			lock (mAssetManager)
			{
				mAssetManager.mOutboxSortManager.SortEnabled = false;
				mAssetManager.mainForm.AssetManagerSentListView.Items.Clear();
				mAssetManager.mainForm.AssetManagerSentListView.Items.AddRange(e.Result as ListViewItem[]);
				mAssetManager.mOutboxSortManager.SortEnabled = true;
				mAssetManager.mainForm.Cursor = Cursors.Default;

				// Make sure all the tabs have updated text
				mAssetManager.UpdateAssetManagerOutboxTabText(true);

				mAssetManager.mGroups.UpdateGroups(mAssetManager.mainForm.AssetManagerSentListView);
				
				guiStartup.StatusBarClear(mAssetManager.mainForm);
			}
		}

		public ListViewItem[] RefreshUsersBox(BackgroundWorker worker, string desiredBox, string contentsFileDirectory)
		{
			MOG_Time now = new MOG_Time();

			ArrayList assets = MOG_DBInboxAPI.InboxGetAssetListWithProperties(desiredBox, MOG_ControllerProject.GetActiveUser().GetUserName(), "", "");
			ListViewItem[] items = new ListViewItem[assets.Count];

			int x = 0;
			foreach (MOG_DBInboxProperties asset in assets)
			{
				// Create the properties for this asset
				MOG_Properties pProperties = new MOG_Properties(asset.mProperties);

				string message = "Refreshing:\n" +
								 "     " + asset.mAsset.GetAssetClassification() + "\n" +
								 "     " + asset.mAsset.GetAssetName();
				worker.ReportProgress(x, message);

				// Update the ListViewItem
				ListViewItem item = guiAssetManager.NewListViewItem(asset.mAsset, pProperties.Status, pProperties);
				items[x++] = item;
			}

			return items;
		}

		public void RefreshLockStatus(MOG_Command LockUpdate)
		{
			Color textColor = Color.Black;

			if (LockUpdate != null)
			{
				ListView currentView = mAssetManager.mainForm.AssetManagerInboxTabControl.SelectedTab.Controls[0] as ListView;

				if (currentView == null)
					return;

				int index = mAssetManager.ListViewItemFindItem(LockUpdate.GetAssetFilename().GetAssetLabel(), currentView);
				if (index != -1)
				{
					currentView.Items[index].ImageIndex = MogUtil_AssetIcons.GetAssetIconIndex(LockUpdate.GetAssetFilename().GetOriginalFilename());
				}
			}
		}


		/// <summary>
		/// Refresh an assets status from a ViewUpdate Command
		/// </summary>
		public void RefreshBox(MOG_Filename add, MOG_Filename del, MOG_Command command)
		{
			lock (mAssetManager)
			{
				ListView currentViewAdd = mAssetManager.IsolateListView(add.GetBoxName(), add.GetFilenameType(), add.GetUserName());
				ListView currentViewDel = mAssetManager.IsolateListView(del.GetBoxName(), del.GetFilenameType(), del.GetUserName());

				// Don't continue if we don't have a valid add and del box
				if ((currentViewAdd == null) && (currentViewDel == null))
					return;

				// Begin the update
				if (currentViewAdd != null)
				{
					currentViewAdd.BeginUpdate();
				}
				if (currentViewDel != null)
				{
					currentViewDel.BeginUpdate();
				}

				string status = command.GetDescription();

				mAssetManager.mainForm.mSoundManager.PlayStatusSound("AssetEvents", status);

				try
				{
					// Obtain the properties from the command
					MOG_Properties pProperties = null;
					string[] commandProps = command.GetVeriables().Split("$".ToCharArray());
					if (commandProps != null)
					{
						ArrayList realProps = new ArrayList();

						// Convert the commandProps into a list of real Properties
						for (int i = 0; i < commandProps.Length; i++)
						{
							// Confirm this appears to be a valid prop?
							if (commandProps[i].StartsWith("["))
							{
								MOG_Property commandProp = new MOG_Property(commandProps[i]);

								// Make sure the property was initialized correctly?
								if (commandProp.mSection.Length > 0 &&
									commandProp.mKey.Length > 0)
								{
									// Add this realProp
									realProps.Add(commandProp);
								}
							}
						}

						// Construct the pProperties from the realProps
						pProperties = new MOG_Properties(realProps);

						// If we have a valid gameDataController, set our platform scope
						if (MOG_ControllerProject.GetCurrentSyncDataController() != null)
						{
							pProperties.SetScope(MOG_ControllerProject.GetCurrentSyncDataController().GetPlatformName());
						}
					}

					// Check if we are just updating an item in the same list?
					if (currentViewAdd == currentViewDel)
					{
						// Check to see if this item already exists?
						int targetIndex = mAssetManager.ListViewItemFindFullItem(del, currentViewDel);
						int sourceIndex = mAssetManager.ListViewItemFindFullItem(add, currentViewAdd);
						if (sourceIndex != -1)
						{
							// Update the existing item
							ListViewItem item = currentViewAdd.Items[sourceIndex];
							guiAssetManager.UpdateListViewItem(item, add, status, pProperties);

							// If there was also a target index, we need to delete it because the source became the target.
							if (targetIndex != -1 &&
								targetIndex != sourceIndex)
							{
								ListViewItem removeItem = currentViewAdd.Items[targetIndex];

								// Make sure to clear our stateImage index which will clear the checked state before we attempt to remove the node or we will throw
								removeItem.StateImageIndex = 0;

								currentViewAdd.Items.Remove(removeItem);
							}
						}
						// Special case when we are dealing with a renamed file
						else if (targetIndex != -1 && sourceIndex == -1)
						{
							// Get the old item
							ListViewItem item = currentViewDel.Items[targetIndex];

							// Update it to the new renamed item
							guiAssetManager.UpdateListViewItem(item, add, status, pProperties);
						}
						else
						{
							// Add a new item
							CreateListViewItem(currentViewAdd, add, status, pProperties);
							// Make sure we always keep our tab's text up-to-date
							mAssetManager.UpdateAssetManagerTabText(add);
						}
					}
					else
					{
						// Looks like we may need to do both the add and the remove
						// Check if we have the currentViewDel?
						if (currentViewDel != null)
						{
							int index = mAssetManager.ListViewItemFindFullItem(del, currentViewDel);
							if (index != -1)
							{
								ListViewItem item = currentViewDel.Items[index];

								// Make sure to clear our stateImage index which will clear the checked state before we attempt to remove the node or we will throw
								item.StateImageIndex = 0;

								// Remove the item from the list
								currentViewDel.Items.Remove(item);

								// Make sure we always keep our tab's text up-to-date
								mAssetManager.UpdateAssetManagerTabText(del);
							}
						}

						// Check if we have the currentViewAdd?
						if (currentViewAdd != null)
						{
							// Check to see if this item already exists?
							int sourceIndex = mAssetManager.ListViewItemFindFullItem(add, currentViewAdd);
							if (sourceIndex != -1)
							{
								// Update the existing item
								ListViewItem item = currentViewAdd.Items[sourceIndex];
								guiAssetManager.UpdateListViewItem(item, add, status, pProperties);
							}
							else
							{
								// Add a new item
								CreateListViewItem(currentViewAdd, add, status, pProperties);
								// Make sure we always keep our tab's text up-to-date
								mAssetManager.UpdateAssetManagerTabText(add);
							}
						}
					}
				}
				catch (Exception ex)
				{
					MOG_Report.ReportSilent("RefreshBox", ex.Message, ex.StackTrace);
				}

				// End the update
				if (currentViewAdd != null)
				{
					currentViewAdd.EndUpdate();
				}
				if (currentViewDel != null)
				{
					currentViewDel.EndUpdate();
				}
			}
		}

		/// <summary>
		/// Add a new asset to this listView
		/// </summary>
		public ListViewItem CreateListViewItem(ListView thisListView, MOG_Filename add, string status, MOG_Properties properties)
		{
			// Update the ListViewItem
			ListViewItem item = guiAssetManager.NewListViewItem(add, status, properties);

			// Add the item to the list view
			thisListView.Items.Add(item);

			return item;
		}

		public void AssetImportFromShell(string[] files, bool separate, bool looseFileMatching)
		{
			// If we are not the login user, switch us
			if (string.Compare(MOG_ControllerProject.GetActiveUser().GetUserName(), MOG_ControllerProject.GetUser().GetUserName(), true) != 0)
			{
				guiUser user = new guiUser(mAssetManager.mainForm);
				mAssetManager.mainForm.AssetManagerActiveUserComboBox.Text = MOG_ControllerProject.GetUser().GetUserName();
				user.SetActiveUser(MOG_ControllerProject.GetUser().GetUserName());
				mAssetManager.RefreshActiveWindow();
			}

			if (separate)
			{
				guiAssetController.ImportSeparately(files, looseFileMatching);
			}
			else
			{
				guiAssetController.ImportAsOne(files, looseFileMatching);
			}
		}

		public void DoubleClick()
		{
			string doubleClickCommand = guiUserPrefs.LoadPref("ClientPrefs", "ContextMenu", "DoubleClick");
			switch (doubleClickCommand)
			{
			case MOG_ControlsLibrary.Common.MOG_ContextMenu.MogControl_AssetContextMenu.Explore_MenuItemText:
				AssetExplore();
				break;
			case MOG_ControlsLibrary.Common.MOG_ContextMenu.MogControl_AssetContextMenu.ShowComments_MenuItemText:
				AssetComments();
				break;
			case MOG_ControlsLibrary.Common.MOG_ContextMenu.MogControl_AssetContextMenu.PackageManagement_MenuItemText:
				AssetPackageManagement();
				break;
			case MOG_ControlsLibrary.Common.MOG_ContextMenu.MogControl_AssetContextMenu.View_MenuItemText:
				AssetView();
				break;
			case MOG_ControlsLibrary.Common.MOG_ContextMenu.MogControl_AssetContextMenu.Properties_MenuItemText:
				AssetProperties();
				break;
			default:
				AssetView();
				break;
			}
		}

		public void AssetView()
		{
			TabPage tab = mAssetManager.mainForm.AssetManagerInboxTabControl.SelectedTab;
			if (tab != null)
			{
				ListView view = tab.Controls[0] as ListView;
				if (view != null)
				{
					if (view.SelectedItems.Count != 0)
					{
						foreach (ListViewItem item in view.SelectedItems)
						{
							string viewTarget = guiUserPrefs.LoadPref("ClientPrefs", "ContextMenu", "ViewTarget");
							guiAssetController.AssetDirectories viewType = guiAssetController.AssetDirectories.PROCESSED;
							switch (viewTarget)
							{
								case MOG_ControlsLibrary.Common.MOG_ContextMenu.MogControl_AssetContextMenu.ViewSource_MenuItemText:
									viewType = guiAssetController.AssetDirectories.SOURCE;
									break;
								case MOG_ControlsLibrary.Common.MOG_ContextMenu.MogControl_AssetContextMenu.ViewProcessed_MenuItemText:
									viewType = guiAssetController.AssetDirectories.PROCESSED;
									break;
								case MOG_ControlsLibrary.Common.MOG_ContextMenu.MogControl_AssetContextMenu.ViewImported_MenuItemText:
									viewType = guiAssetController.AssetDirectories.IMPORTED;
									break;
							}

							string assetName = item.SubItems[(int)guiAssetManager.AssetBoxColumns.FULLNAME].Text;
							MOG_Filename filename = new MOG_Filename(assetName.ToLower());

							MOG_Properties pProperties = new MOG_Properties(filename);
							string viewer = pProperties.AssetViewer;

							guiAssetController controller = new guiAssetController();
							controller.View(filename, viewType, viewer);
						}
					}
				}
			}
		}

		public void AssetComments()
		{
			TabPage tab = mAssetManager.mainForm.AssetManagerInboxTabControl.SelectedTab;
			if (tab != null)
			{
				ListView view = tab.Controls[0] as ListView;
				if (view != null)
				{
					if (view.SelectedItems.Count != 0)
					{
						foreach (ListViewItem item in view.SelectedItems)
						{
							AssetPropertiesForm properties = new AssetPropertiesForm();
							properties.Initialize(item.SubItems[(int)guiAssetManager.AssetBoxColumns.FULLNAME].Text);
							properties.Show(mAssetManager.mainForm);
							properties.SelectTab("PropertiesCommentsTabPage");
						}
					}
				}
			}
		}

		public void AssetExplore()
		{
			TabPage tab = mAssetManager.mainForm.AssetManagerInboxTabControl.SelectedTab;
			if (tab != null)
			{
				ListView view = tab.Controls[0] as ListView;
				if (view != null)
				{
					if (view.SelectedItems.Count != 0)
					{
						foreach (ListViewItem item in view.SelectedItems)
						{
							string realDirectory = item.SubItems[(int)guiAssetManager.AssetBoxColumns.FULLNAME].Text;
							guiCommandLine.ShellExecute(realDirectory);
						}
					}
				}
			}
		}

		public void AssetPackageManagement()
		{
			TabPage tab = mAssetManager.mainForm.AssetManagerInboxTabControl.SelectedTab;
			if (tab != null)
			{
				ListView view = tab.Controls[0] as ListView;
				if (view != null)
				{
					if (view.SelectedItems.Count != 0)
					{
						ArrayList fixedItems = new ArrayList();
						foreach (ListViewItem item in view.SelectedItems)
						{
							string realDirectory = item.SubItems[(int)guiAssetManager.AssetBoxColumns.FULLNAME].Text;
							fixedItems.Add(new MOG_Filename(realDirectory));
						}

						MOG_Client.Forms.PackageManagementForm form = new MOG_Client.Forms.PackageManagementForm(fixedItems, false);
						form.ShowDialog();
					}
				}
			}
		}

		public void AssetProperties()
		{
			TabPage tab = mAssetManager.mainForm.AssetManagerInboxTabControl.SelectedTab;
			if (tab != null)
			{
				ListView view = tab.Controls[0] as ListView;
				if (view != null)
				{
					if (view.SelectedItems.Count != 0)
					{
						foreach (ListViewItem item in view.SelectedItems)
						{
							AssetPropertiesForm properties = new AssetPropertiesForm();
							properties.Initialize(item.SubItems[(int)guiAssetManager.AssetBoxColumns.FULLNAME].Text);
							properties.Show(mAssetManager.mainForm);
						}
					}
				}
			}
		}
	}
}
