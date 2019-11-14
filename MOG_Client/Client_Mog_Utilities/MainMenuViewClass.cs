using System;
using System.Collections;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.IO;

using MOG_ControlsLibrary;

using MOG_Client;

using MOG;
using MOG.PROJECT;
using MOG.INI;
using MOG.DOSUTILS;
using MOG.REPORT;
using MOG.CONTROLLER.CONTROLLERINBOX;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG_Client.Client_Gui.AssetManager_Helper;
using MOG_ControlsLibrary.Common;


namespace MOG_Client.Client_Mog_Utilities
{
	/// <summary>
	/// Summary description for MainMenuViewClass.
	/// </summary>
	public class MainMenuViewClass
	{
		static public bool bChangeLocalBranchRightPanelWidth;
		static public int LocalBranchRightPanelSavedWidth;

		static private System.Windows.Forms.Timer connectionRefreshTimer = null;

		public MainMenuViewClass()
		{
		}

		static public void MOGGlobalViewRemoteMachineInit(MogMainForm mainForm)
		{
			// Reset our list to a virgin state
			for (int x = mainForm.remoteMachinesToolStripMenuItem.DropDownItems.Count; x > 1; x--)
			{
				ToolStripItem item = mainForm.remoteMachinesToolStripMenuItem.DropDownItems[x - 1];
				if (string.Compare(item.Text, "&Add...", true) != 0)
				{
					mainForm.remoteMachinesToolStripMenuItem.DropDownItems.RemoveAt(x - 1);
				}
			}

			ArrayList machineNames = MOG_ControllerSystem.LocateTools("RemoteConnections", "*.RDP");
			machineNames.Sort();

			// Add a separator
			ToolStripSeparator separator = new ToolStripSeparator();
			mainForm.remoteMachinesToolStripMenuItem.DropDownItems.Add(separator);

			// Add each machine name
			foreach (string machine in machineNames)
			{
				ToolStripMenuItem Item = new ToolStripMenuItem(Path.GetFileNameWithoutExtension(machine));
				Item.Click += new System.EventHandler(MainMenuViewClass.MOGGlobalViewRemoteMachine_Click);
				mainForm.remoteMachinesToolStripMenuItem.DropDownItems.Add(Item);
			}
		}

		static public void MOGGlobalViewRemoteMachineAdd(MogMainForm mainForm)
		{
			// Ask for a new rdp
			mainForm.MOGOpenFileDialog.Filter = "Remote connections | *.rdp";
			mainForm.MOGOpenFileDialog.Multiselect = true;

			if (mainForm.MOGOpenFileDialog.ShowDialog(mainForm) == DialogResult.OK)
			{
				foreach (string remoteConnection in mainForm.MOGOpenFileDialog.FileNames)
				{
					DosUtils.FileCopy(remoteConnection, MOG_ControllerProject.GetUser().GetUserToolsPath() + "\\RemoteConnections\\" + Path.GetFileName(remoteConnection));
				}

				// Rebuild the menuItems array of these newly added rdp's
				MOGGlobalViewRemoteMachineInit(mainForm);
			}			
		}

		static private void MOGGlobalViewRemoteMachine_Click(object sender, System.EventArgs e)
		{
			ToolStripItem menu = sender as ToolStripItem;
			string MachineName = menu.Text;
			
			string output = "";

			// Get the tool listed on the startup page
			string command = MOG_ControllerSystem.LocateTool("RemoteConnections", MachineName + ".Rdp");
			if (command == null ||
				command.Length == 0)
			{
				MessageBox.Show(string.Concat("The target Rdp (",MachineName,") does not exist in any of our valid tools directories!", "Warning!", MessageBoxButtons.OK));
				return;
			}
						
			guiCommandLine.ShellSpawn(command, "", ProcessWindowStyle.Normal, ref output);
		}
		static public void MOGGlobalViewRemoteMachine(MogMainForm mainForm, string MachineName)
		{
			string output = "";
			
			// Get the tool listed on the startup page
			string command = MOG_ControllerSystem.LocateTool("RemoteConnections", MachineName + ".Rdp");
			if (command == null ||
				command.Length == 0)
			{
				MessageBox.Show(string.Concat("The target Rdp (",MachineName,") does not exist in any of our valid tools directories!", "Warning!", MessageBoxButtons.OK));
				return;
			}
						
			guiCommandLine.ShellSpawn(command, "", ProcessWindowStyle.Normal, ref output);			
		}

		/// <summary>
		/// Try to rebuild all of our user boxes based on what's on the HDD
		/// </summary>
		/// <param name="mainForm"></param>
		static public void MOGGlobalViewRebuildInbox(MogMainForm mainForm)
		{
			if (mainForm.mAssetManager != null)
			{
				// Initialize our variables...
				string boxDirectory = mainForm.mAssetManager.mInboxAssetsDirectory;
				string userPath = MOG_ControllerProject.GetActiveUser().GetUserPath();
				// If we have a proper boxDirectory, add a "\\" to it
				if( boxDirectory.Length > 0)
				{
					boxDirectory += "\\";
				}

				// Rebuild our contents.info and the database based on what's in the HDD
				UpdateBox(userPath, "Inbox", boxDirectory, false);
				UpdateBox(userPath, "Drafts", boxDirectory, false);
				UpdateBox(userPath, "Outbox", boxDirectory, false);
				UpdateBox(userPath, "Trash", boxDirectory, true);

				// Now rebuild local window
				if (mainForm.mAssetManager.mLocal != null && MOG_ControllerProject.GetCurrentSyncDataController() != null)
				{
					MOG_ControllerInbox.RebuildInboxView(String.Concat(MOG_ControllerProject.GetCurrentSyncDataController().GetSyncDirectory(), "\\MOG\\UpdatedTray"), false);
					mainForm.mAssetManager.mLocal.RefreshWindow();
				}
			}			
		}

		/// <summary>
		/// UpdateBox to reflect what's on the HDD
		/// </summary>
		/// <param name="userPath"></param>
		/// <param name="boxName"></param>
		/// <param name="boxDirectory"></param>
		/// <param name="sendUpdateCommand"></param>
		static private void UpdateBox(string userPath, string boxName, string boxDirectory, bool sendUpdateCommand)
		{
			string targetContentsFile = string.Concat(userPath, "\\", boxName, "\\", boxDirectory);
			DirectoryInfo dir = new DirectoryInfo(targetContentsFile);
			if (dir.Exists)
			{
				MOG_ControllerInbox.RebuildInboxView(targetContentsFile, sendUpdateCommand);
			}
		}

		static public void connectionRefreshTimer_Tick(object sender, EventArgs args)
		{
			connectionRefreshTimer = null;
		}

		static public void MOGGlobalViewRefresh(MogMainForm mainForm, string tabName)
		{
			switch (tabName)
			{
				case "MainTabStartupTabPage":
					mainForm.MOGWelcomeWebBrowser.Refresh();
					break;
				case "MainTabProjectManagerTabPage":
					mainForm.mProjectManager.BuildRepositoryTrees(true);
					//mainForm.ProjectManagerTaskWindow.InitializeForProject();
					break;
				case "MainTabAssetManagerTabPage":
					mainForm.RefreshClientAssetWindow();
					mainForm.mAssetManager.mLocal.RefreshWindow();
					//mainForm.AssetManagerTaskWindow.InitializeForUser();
					break;
				case "MainTabConnectionManagerTabPage":
					if (connectionRefreshTimer == null)//mainForm.AllowConnectionsRefresh)
					{
						mainForm.mConnectionManager.Refresh();
						connectionRefreshTimer = new System.Windows.Forms.Timer();
						connectionRefreshTimer.Interval = 1000;
						connectionRefreshTimer.Tick += new EventHandler(connectionRefreshTimer_Tick);
						connectionRefreshTimer.Start();
					}
					break;
				case "MainTabLockManagerTabPage":
					if (mainForm.mLockManager != null)
					{
						mainForm.mLockManager.Initialize();
					}
					break;
				case "MainTabLibraryTabPage":
					if (mainForm.mLibraryManager != null)
					{
						mainForm.mLibraryManager.Refresh();
					}

					break;
				default:
					MOG_Report.ReportSilent("Error In MOG_Client.Client_Mog_Utilities.MainMenuViewClass.MOGGlobalViewRefresh",
						"Error finding tab page to refresh: \r\n\r\n"
						+ mainForm.MOGTabControl.SelectedTab.Name, Environment.StackTrace);
					break;
			}
		}

		static public void MOGGlobalViewRefresh(MogMainForm mainForm)
		{
			MOGGlobalViewRefresh(mainForm, mainForm.MOGTabControl.SelectedTab.Name);
		}

		static public void MOGGlobalViewSelectAll(MogMainForm mainForm)
		{
			ListView activeView = null;

			if (mainForm.ActiveControl is ListView)
			{
				activeView = (ListView)mainForm.ActiveControl;
			}
			else if (mainForm.ActiveControl is MogControl_LocalWorkspaceTab)
			{
				MogControl_LocalWorkspaceTab tab = mainForm.ActiveControl as MogControl_LocalWorkspaceTab;

				activeView = tab.WorkSpaceListView;
			}
			else if (mainForm.ActiveControl is MogControl_LibraryExplorer)
			{
				MogControl_LibraryExplorer library = mainForm.ActiveControl as MogControl_LibraryExplorer;
				activeView = library.LibraryListView.LibraryListView;
			}

			if (activeView != null)
			{
				foreach (ListViewItem listViewItem in activeView.Items)
				{
					listViewItem.Selected = true;
				}
			}
		}

		static public void MOGGlobalViewInvertAll(MogMainForm mainForm)
		{
			ListView activeView = null;

			if (mainForm.ActiveControl is ListView)
			{
				activeView = (ListView)mainForm.ActiveControl;				
			}
			else if (mainForm.ActiveControl is MogControl_LocalWorkspaceTab)
			{
				MogControl_LocalWorkspaceTab tab = mainForm.ActiveControl as MogControl_LocalWorkspaceTab;

				activeView = tab.WorkSpaceListView;
			}
			else if (mainForm.ActiveControl is MogControl_LibraryExplorer)
			{
				MogControl_LibraryExplorer library = mainForm.ActiveControl as MogControl_LibraryExplorer;
				activeView = library.LibraryListView.LibraryListView;
			}

			if (activeView != null)
			{
				foreach (ListViewItem listViewItem in activeView.Items)
				{
					listViewItem.Selected = !listViewItem.Selected;
				}				
			}
		}

		static public void MOGGlobalViewMyWorkspace(MogMainForm mainForm)
		{
			mainForm.AssetManagerLocalDataExplorerSplitter.SplitPosition = mainForm.mAssetManager.mLocalExplorer.ToggleWidth();
		}

		static public void MOGGlobalViewMyToolbox(MogMainForm mainForm)
		{
			// Allow LocalBranchRightPanel to change its width
			MainMenuViewClass.bChangeLocalBranchRightPanelWidth = true;
			// Store our toggled width...
			int width = mainForm.mAssetManager.mTools.ToggleWidth();
			// Change our Splitter width (which will immediately turn back to 125 for some reason)
			mainForm.AssetManagerLocalDataSplitter.SplitPosition = width;
			// Save our toggle width
			MainMenuViewClass.LocalBranchRightPanelSavedWidth = width;
			// Tell our LocalBranchRightPanel not to update its width to a different width
			MainMenuViewClass.bChangeLocalBranchRightPanelWidth = false;
		}

		internal static void LoadVisibleTab(MogMainForm main, ToolStripMenuItem toolStripMenuItem, bool state)
		{
			switch (state)
			{
				case true:
					ShowTabPage(main, toolStripMenuItem);
					break;
				case false:
					HideTabPage(main, toolStripMenuItem);
					break;
			}
		}

		internal static void ToggleVisibleTab(MogMainForm main, ToolStripMenuItem toolStripMenuItem)
		{
			switch (toolStripMenuItem.Checked)
			{
				case true:
					ShowTabPage(main, toolStripMenuItem);
					break;
				case false:
					HideTabPage(main, toolStripMenuItem);
					break;
			}
		}

		// Holder for hidden pages
		private static TabControl mHiddenPages = new TabControl();

		internal static void HideTabPage(MogMainForm main, ToolStripMenuItem removeItem)
		{
			TabControl tabControl = main.MOGTabControl;
			TabPage current = null;

			// Find the active visible page
			foreach (TabPage page in tabControl.TabPages)
			{
				if (string.Compare(page.Text, removeItem.Text, true) == 0)
				{
					current = page;
					break;
				}
			}

			// Do we have one?
			if (current != null)
			{
				// Add it to the hidden container
				mHiddenPages.TabPages.Add(current);

				// Save its current tab position that is saved in the tag of the tabPage
				removeItem.Tag = Convert.ToInt32(current.Tag);

				// Also set the tab name as the menuItem name so that we can look up the tab name by key using the menuItem name
				removeItem.Name = current.Name;

				// Remove the page
				tabControl.TabPages.Remove(current);
				tabControl.Refresh();

				// Hide our associated view menu item
				main.MOG_TabControl_HideViewTabMenuItem(current, false);
			}

			// Ok, we are hidden
			removeItem.Checked = false;
		}

		internal static void ShowTabPage(MogMainForm main, ToolStripMenuItem showItem)
		{
			TabControl tabControl = main.MOGTabControl;

			// Try and get the hidden page from our temp container by key
			TabPage page = mHiddenPages.TabPages[showItem.Name];
			if (page != null)
			{
				// Get the tab index of the hidden page
				int index = (int)showItem.Tag;

				// Do we currently have ANY pages?
				if (tabControl.TabPages.Count > 0)
				{
					// Is the index for this page larger than our current number of pages?
					if (index >= tabControl.TabPages.Count)
					{
						// Get the last visible pages prefered tab position
						int lastIndex = 0;
						int x = 1;
						string tag = tabControl.TabPages[tabControl.TabPages.Count - 1].Tag as string;

						while (true)
						{
							try
							{
								// Is this a web tab?
								lastIndex = Convert.ToInt32(tag);
								break;
							}
							catch
							{
								x++;
								if (tabControl.TabPages.Count >= tabControl.TabPages.Count - x && tabControl.TabPages.Count - x > 0)
								{
									// Then try the next tab back
									tag = tabControl.TabPages[tabControl.TabPages.Count - x].Tag as string;
								}
								else
								{
									// We are out of tabs, so lets put it at the end
									lastIndex = index + 1;
									break;
								}
							}
						}

						// Is our current page position want to be after the last tabs position?
						if (index > lastIndex)
						{
							// Then add it
							tabControl.TabPages.Add(page);
						}
						else
						{
							// Then instert it
							tabControl.TabPages.Insert(tabControl.TabPages.Count - 1, page);
						}

					}
					else
					{
						// Then insert it
						tabControl.TabPages.Insert(index, page);
					}
				}
				else
				{
					// Then add it
					tabControl.TabPages.Add(page);

					// Single pages need to be reInitialized
					main.InitializeMainTabPage(page);
				}

				// Now remove this newly added page from our temp hidden container
				mHiddenPages.TabPages.Remove(page);

				// Ok we should be checked
				showItem.Checked = true;

				// Hide our associated view menu item
				main.MOG_TabControl_HideViewTabMenuItem(page, true);
			}
		}


		internal static void SetMogLibraryMode(MogMainForm mogMainForm)
		{
			mogMainForm.VisibleStartToolStripMenuItem.Visible = false;
			mogMainForm.VisibleWorkspaceToolStripMenuItem.Visible = false;
			mogMainForm.VisibleConnectionsToolStripMenuItem.Visible = false;
			
			mogMainForm.myToolboxToolStripMenuItem.Visible = false;
			mogMainForm.myLocalExplorerToolStripMenuItem.Visible = false;
			mogMainForm.toolStripSeparator9.Visible = false;
			mogMainForm.rebuildInboxToolStripMenuItem.Visible = false;
			mogMainForm.remoteMachinesToolStripMenuItem.Visible = false;
			mogMainForm.toolStripMenuItem2.Visible = false;
			mogMainForm.toolStripSeparator10.Visible = false;
			mogMainForm.showGroupsToolStripMenuItem.Visible = false;
		}
	}
}
