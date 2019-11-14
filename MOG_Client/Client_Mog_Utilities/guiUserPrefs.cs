using System;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;

using MOG;
using MOG.INI;
using MOG.DOSUTILS;
using MOG.PROJECT;
using MOG.PLATFORM;
using MOG_Client.Client_Mog_Utilities;
using MOG_Client.Client_Gui;
using MOG_Client.Forms;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.CONTROLLER.CONTROLLERSYNCDATA;
using MOG.REPORT;
using MOG.DATABASE;

using MOG_ControlsLibrary.Common.MogControl_RepositoryTreeViews;
using MOG_ControlsLibrary.Common.MogControl_LibraryListView;
using MOG_ControlsLibrary.Forms;
using MOG_ControlsLibrary.MogUtils_Settings;
using MOG_Client.Client_Gui.AssetManager_Helper;

namespace MOG_Client.Client_Utilities
{
	/// <summary>
	/// Summary description for UserPrefs.
	/// </summary>
	public class guiUserPrefs
	{
		public const string PackageManagementForm_Text = "PackageManagementForm";
		public MogMainForm mainForm;

		public guiUserPrefs(String PrefsLocation, MogMainForm main)
		{
			MogUtils_Settings.CreateSettings(PrefsLocation);
			mainForm = main;
		}

		#region Utilities
		
		public bool Save()
		{
			return MogUtils_Settings.Settings.Save();
		}
		#endregion
		#region Loaders
		#region Tab page pref loading functions
		private void Load_Global()
		{
			// Global Stuff
			if (MogUtils_Settings.SettingExist("GuiLayout", "Gui", "ActiveTab"))
			{
				int activeTab = MogUtils_Settings.LoadIntSetting("GuiLayout", "Gui", "ActiveTab");
				if (activeTab >= mainForm.MOGTabControl.TabPages.Count)
				{
					activeTab = 0;
				}
				mainForm.MOGTabControl.SelectedIndex = activeTab;
				mainForm.MOG_TabControl_InitializeTabPage(mainForm.MOGTabControl.SelectedTab.Name);
			}
			else
			{
				mainForm.MOGTabControl.SelectedIndex = 0;
				mainForm.MOG_TabControl_InitializeTabPage(mainForm.MOGTabControl.SelectedTab.Name);
			}

			// Load sound theme
			if (MogUtils_Settings.SettingExist("GuiLayout", "Gui", "SoundTheme")) mainForm.mSoundScheme = MogUtils_Settings.LoadSetting("GuiLayout", "Gui", "SoundTheme");
			if (MogUtils_Settings.SettingExist("GuiDynamicOptions", "Gui", "Sounds"))
			{
				mainForm.mSoundManager.Enabled = MogUtils_Settings.LoadBoolSetting("GuiDynamicOptions", "Gui", "Sounds", true);
				mainForm.soundEnableToolStripMenuItem.Checked = mainForm.mSoundManager.Enabled;
			}

			mainForm.minimizeToSystemTrayToolStripMenuItem.Checked = MogUtils_Settings.LoadBoolSetting("Client", "MinimiseToSystemTray", false);
            mainForm.alwaysDisplayAdvancedGetLatestToolStripMenuItem.Checked = MogUtils_Settings.LoadBoolSetting("Client", "AlwaysDisplayAdvancedGetLatest", false);
            
// JohnRen - For some unknown reason, MOG crashes when windowState=Maxiimzed && minimizeToSystemTrayToolStripMenuItem=true
//			 Both Kier and I have given up on this one so it is simply easier to just not save this GUI setting
//			// Global Stuff: Set the windows state last (for some reason, LoadProjectData() sets it to Normal)
//			if (MogUtils_Settings.SettingExist("GuiLayout", "Gui", "windowState"))
//			{
//				mainForm.WindowState = (FormWindowState)MogUtils_Settings.LoadIntSetting("GuiLayout", "Gui", "windowState");
//				
//				// We never want to reload a window in the minimized state.  Override to normal
//				if (mainForm.WindowState == FormWindowState.Minimized)
//				{
//					mainForm.WindowState = FormWindowState.Normal;
//				}
//			}
		}

		private void Load_Workspace()
		{
			//
			// Workspace
			//
			mainForm.AssetManagerAutoProcessCheckBox.Checked = MogUtils_Settings.LoadBoolSetting("GuiLayout", "Workspace", "AutoProcess", true);
			mainForm.AssetManagerAutoImportCheckBox.Checked = MogUtils_Settings.LoadBoolSetting("GuiLayout", "Workspace", "AutoImport", true);
			mainForm.AssetManagerAutoUpdateLocalCheckBox.Checked = MogUtils_Settings.LoadBoolSetting("GuiLayout", "Workspace", "AutoUpdate", true);
			mainForm.AssetManagerAutoPackageCheckBox.Checked = MogUtils_Settings.LoadBoolSetting("GuiLayout", "Workspace", "AutoPackage", true);
			mainForm.AssetManagerPackageButton.Enabled = MogUtils_Settings.LoadBoolSetting("GuiLayout", "Workspace", "PackageMerge", true);

			// Asset Manager Inbox Columns
			if (MogUtils_Settings.SettingExist("WorkspaceDrafts")) MogUtils_Settings.LoadListView("WorkspaceDrafts", mainForm.AssetManagerDraftsListView);
			if (MogUtils_Settings.SettingExist("WorkspaceInbox")) MogUtils_Settings.LoadListView("WorkspaceInbox", mainForm.AssetManagerInboxListView);
			if (MogUtils_Settings.SettingExist("WorkspaceOutbox")) MogUtils_Settings.LoadListView("WorkspaceOutbox", mainForm.AssetManagerSentListView);
			if (MogUtils_Settings.SettingExist("WorkspaceTrash")) MogUtils_Settings.LoadListView("WorkspaceTrash", mainForm.AssetManagerTrashListView);
			
			// Populate the columns menu
			//foreach (ColumnHeader header in mainForm.AssetManagerInboxAssetListView.Columns)
			//{
			//    MenuItem menuItem = new MenuItem(header.Text, new System.EventHandler(mainForm.MOGAssetInboxMenuItem_Click));
			//    if (header.Width > 0)
			//    {
			//        menuItem.Checked = true;
			//    }
			//    else
			//    {
			//        menuItem.Checked = false;
			//    }
			//    mainForm.AssetInboxColumnContextMenu.MenuItems.Add(menuItem);
			//}
		}

		private void Load_Projects()
		{
			//
			// Projects
			//
			//if (MogUtils_Settings.SettingExist("GuiLayout", "ProjectManager", "SelectedRepositoryView")) mainForm.mpra.ProjectManagerRepositoryViewComboBox.SelectedIndex = MogUtils_Settings.LoadIntSetting("GuiLayout", "ProjectManager", "SelectedRepositoryView");
			//if (MogUtils_Settings.SettingExist("GuiLayout", "ProjectManager", "AutoUpdateTrees")) mainForm.ProjectManagerAutoUpdateTreesCheckBox.Checked =MogUtils_Settings.LoadBoolSetting("GuiLayout", "ProjectManager", "AutoUpdateTrees", false);
		}

		private void Load_Connections()
		{
			//
			// Connections
			//			

			mainForm.ConnectionsBottomTabControl.SelectedIndex = MogUtils_Settings.LoadIntSetting("ConnectionManager", "ConnectionsBottomTabControlSelectedIndex", 0);
		}

		private void Load_Library()
		{
			// Library Columns
			//if (MogUtils_Settings.SettingExist("GuiLayout", "LibraryColumns", "ColumnOrder")) PutColumnOrderString(mainForm.LibraryExplorer., MogUtils_Settings.LoadSetting("GuiLayout", "WorkspaceInboxColumns", "name"));
			MogUtils_Settings.LoadListView("LibraryListView", mainForm.LibraryExplorer.LibraryListView.LibraryListView);			
		}

		#endregion

		public void LoadStaticForm_LayoutPrefs()
		{
			try
			{
				// Load global gui
				Load_Global();

				// Load the tab prefs
				Load_Workspace();
				Load_Projects();
				Load_Connections();
				Load_Library();
			}
			// Catch any regular C# exception
			catch (Exception ex)
			{
				MOG_Report.ReportSilent("LoadStaticForm_LayoutPrefs", ex.Message, ex.StackTrace);
			}
		}

		public void LoadStatic_ProjectPrefs()
		{
			if (MogUtils_Settings.SettingExist("MOG", "Project"))
			{
				string projectName = "";

				projectName = MogUtils_Settings.LoadSetting("MOG", "Project");
				if (guiProject.SetLoginProject(projectName, ""))
				{
					if (MogUtils_Settings.SettingExist("MOG", "LoginUser"))
					{
						guiUser user = new guiUser(mainForm);
						user.SetLoginUser(MogUtils_Settings.LoadSetting("MOG", "LoginUser"));
					}

					if (MogUtils_Settings.SettingExist("MOG", "ActiveUser"))
					{
						guiUser user = new guiUser(mainForm);
						user.SetActiveUser(MogUtils_Settings.LoadSetting("MOG", "ActiveUser"));
					}

					// Make sure we have properly logged in?
					if (MOG_ControllerProject.IsProject() &&
						MOG_ControllerProject.IsUser())
					{
						// Set the last workspace the user had loaded as the currently active workspace
						string activeWorkspace = MogUtils_Settings.LoadSetting(MOG_ControllerProject.GetProjectName(), MOG_ControllerProject.GetUserName(), "ActiveWorkspace");
						if (activeWorkspace.Length != 0)
						{
							ArrayList SyncedLocations = MOG.DATABASE.MOG_DBSyncedDataAPI.GetAllSyncedLocations(MOG_ControllerSystem.GetComputerName(), activeWorkspace, "", MOG_ControllerProject.GetUserName());
							if (SyncedLocations != null)
							{
								foreach (MOG_DBSyncedLocationInfo location in SyncedLocations)
								{
									if (string.Compare(activeWorkspace, location.mWorkingDirectory, true) == 0)
									{
										MOG_ControllerSyncData sync = WorkspaceManager.AddNewWorkspace(location);
										if (sync != null)
										{
											MOG_ControllerProject.SetCurrentSyncDataController(sync);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		static public bool LoadDynamic_LayoutPrefs(string key, Form form)
		{
			switch (key)
			{
				case "AssetManager":
					#region AssetManager
					MogMainForm myMainForm = form as MogMainForm;
					if (myMainForm != null)
					{
						int draftSortCol = 0;
						int inboxSortCol = 0;
						int outboxSortCol = 0;
						int trashSortCol = 0;
						SortOrder draftSort = SortOrder.Descending;
						SortOrder inboxSort = SortOrder.Descending;
						SortOrder outboxSort = SortOrder.Descending;
						SortOrder trashSort = SortOrder.Descending;

						if (MogUtils_Settings.SettingExist("MOGLayout", "Workspace", "updateBuildType")) myMainForm.mAssetManager.mDefaultUpdateBuildType = MogUtils_Settings.LoadSetting("MOGLayout", "Workspace", "updateBuildType");
						if (MogUtils_Settings.SettingExist("MOGLayout", "Workspace", "updateBuildHidePlatforms")) myMainForm.mAssetManager.mHidePlatforms = MogUtils_Settings.LoadBoolSetting("MOGLayout", "Workspace", "updateBuildHidePlatforms", true);

						if (MogUtils_Settings.SettingExist("MOGLayout", "Workspace", "DraftsSortColumn")) draftSortCol = MogUtils_Settings.LoadIntSetting("MOGLayout", "Workspace", "DraftsSortColumn");
						if (MogUtils_Settings.SettingExist("MOGLayout", "Workspace", "DraftsSort")) draftSort = MogUtils_Settings.LoadSortOrderSetting("MOGLayout", "Workspace", "DraftsSort");
						if (MogUtils_Settings.SettingExist("MOGLayout", "Workspace", "InboxSortColumn")) inboxSortCol = MogUtils_Settings.LoadIntSetting("MOGLayout", "Workspace", "InboxSortColumn");
						if (MogUtils_Settings.SettingExist("MOGLayout", "Workspace", "InboxSort")) inboxSort = MogUtils_Settings.LoadSortOrderSetting("MOGLayout", "Workspace", "InboxSort");
						if (MogUtils_Settings.SettingExist("MOGLayout", "Workspace", "OutboxSortColumn")) outboxSortCol = MogUtils_Settings.LoadIntSetting("MOGLayout", "Workspace", "OutboxSortColumn");
						if (MogUtils_Settings.SettingExist("MOGLayout", "Workspace", "OutboxSort")) outboxSort = MogUtils_Settings.LoadSortOrderSetting("MOGLayout", "Workspace", "OutboxSort");
						if (MogUtils_Settings.SettingExist("MOGLayout", "Workspace", "TrashSortColumn")) trashSortCol = MogUtils_Settings.LoadIntSetting("MOGLayout", "Workspace", "TrashSortColumn");
						if (MogUtils_Settings.SettingExist("MOGLayout", "Workspace", "TrashSort")) trashSort = MogUtils_Settings.LoadSortOrderSetting("MOGLayout", "Workspace", "TrashSort");

						// Groups
						if (MogUtils_Settings.SettingExist("GuiDynamicOptions", "Groups", "DraftsBoxGroup")) myMainForm.mAssetManager.mGroups.SetGroup(myMainForm.AssetManagerDraftsListView, MogUtils_Settings.LoadSetting("GuiDynamicOptions", "Groups", "DraftsBoxGroup"));
						if (MogUtils_Settings.SettingExist("GuiDynamicOptions", "Groups", "InBoxGroup")) myMainForm.mAssetManager.mGroups.SetGroup(myMainForm.AssetManagerInboxListView, MogUtils_Settings.LoadSetting("GuiDynamicOptions", "Groups", "InBoxGroup"));
						if (MogUtils_Settings.SettingExist("GuiDynamicOptions", "Groups", "SentBoxGroup")) myMainForm.mAssetManager.mGroups.SetGroup(myMainForm.AssetManagerSentListView, MogUtils_Settings.LoadSetting("GuiDynamicOptions", "Groups", "SentBoxGroup"));
						if (MogUtils_Settings.SettingExist("GuiDynamicOptions", "Groups", "TrashBoxGroup")) myMainForm.mAssetManager.mGroups.SetGroup(myMainForm.AssetManagerTrashListView, MogUtils_Settings.LoadSetting("GuiDynamicOptions", "Groups", "TrashBoxGroup"));
						
						myMainForm.showGroupsToolStripMenuItem.Checked = MogUtils_Settings.LoadBoolSetting("GuiLayout", "Groups", "Enabled", true);
						myMainForm.mAssetManager.mGroups.Enable(myMainForm.showGroupsToolStripMenuItem.Checked);
									
						MogUtils_Settings.LoadListView("WorkspaceDrafts", myMainForm.AssetManagerDraftsListView);
						MogUtils_Settings.LoadListView("WorkspaceInbox", myMainForm.AssetManagerInboxListView);
						MogUtils_Settings.LoadListView("WorkspaceOutbox", myMainForm.AssetManagerSentListView);
						MogUtils_Settings.LoadListView("WorkspaceTrash", myMainForm.AssetManagerTrashListView);

						myMainForm.mAssetManager.mToDo.Load(MogUtils_Settings.Settings, "MOGLayout", "Workspace", "ToDo");
						myMainForm.mAssetManager.mLocalExplorer.Load(MogUtils_Settings.Settings, "MOGLayout", "Workspace", "LocalExplorer");
						myMainForm.mAssetManager.mTools.Load(MogUtils_Settings.Settings, "MOGLayout", "Workspace", "LocalTools");

						if (draftSortCol >= 0)
							myMainForm.mAssetManager.mDraftsSortManager.Sort(draftSortCol, draftSort);
						if (inboxSortCol >= 0)
							myMainForm.mAssetManager.mInboxSortManager.Sort(inboxSortCol, inboxSort);
						if (outboxSortCol >= 0)
							myMainForm.mAssetManager.mOutboxSortManager.Sort(outboxSortCol, outboxSort);
						if (trashSortCol >= 0)
							myMainForm.mAssetManager.mTrashSortManager.Sort(trashSortCol, trashSort);
					}
					#endregion AssetManager
					break;
				case "NewAssetList":
					#region NewListForm / NewAssetsList
					NewListForm myForm = form as NewListForm;
					if (myForm != null)
					{
						if (MogUtils_Settings.SettingExist("MOGLayout", "AssetList", "NoRange")) myForm.ListTimeNoRangeRadioButton.Checked = MogUtils_Settings.LoadBoolSetting("MOGLayout", "AssetList", "NoRange", true);
						if (MogUtils_Settings.SettingExist("MOGLayout", "AssetList", "SpecifyRange")) myForm.ListDateRangeRadioButton.Checked = MogUtils_Settings.LoadBoolSetting("MOGLayout", "AssetList", "SpecifyRange", false);
						if (MogUtils_Settings.SettingExist("MOGLayout", "AssetList", "SpecifyTimeRange")) myForm.ListTimeRangeRadioButton.Checked = MogUtils_Settings.LoadBoolSetting("MOGLayout", "AssetList", "SpecifyTimeRange", false);
						if (MogUtils_Settings.SettingExist("MOGLayout", "AssetList", "TimeRange")) myForm.ListHoursTextBox.Text = MogUtils_Settings.LoadSetting("MOGLayout", "AssetList", "TimeRange");
						if (MogUtils_Settings.SettingExist("MOGLayout", "AssetList", "Start")) myForm.ListStartDateTimePicker.Value = MogUtils_Settings.LoadDateTimeSetting("MOGLayout", "AssetList", "Start");
						if (MogUtils_Settings.SettingExist("MOGLayout", "AssetList", "End")) myForm.ListEndDateTimePicker.Value = MogUtils_Settings.LoadDateTimeSetting("MOGLayout", "AssetList", "End");
						if (MogUtils_Settings.SettingExist("MOGLayout", "AssetList", "FilterEnable")) myForm.ListFilterCheckBox.Checked = MogUtils_Settings.LoadBoolSetting("MOGLayout", "AssetList", "FilterEnable", false);
						if (MogUtils_Settings.SettingExist("MOGLayout", "AssetList", "Filter")) myForm.ListFilterTextBox.Text = MogUtils_Settings.LoadSetting("MOGLayout", "AssetList", "Filter");
					}
					#endregion NewListForm
					break;
				case "ReportForm":
					#region ReportForm
					Report myLogForm = form as Report;
					if (myLogForm != null)
					{
						if (MogUtils_Settings.SettingExist("MOGLayout", "CustomToolReports", "Width")) myLogForm.Width = MogUtils_Settings.LoadIntSetting("MOGLayout", "CustomToolReports", "Width");
						if (MogUtils_Settings.SettingExist("MOGLayout", "CustomToolReports", "Height")) myLogForm.Height = MogUtils_Settings.LoadIntSetting("MOGLayout", "CustomToolReports", "Height");
						if (MogUtils_Settings.SettingExist("MOGLayout", "CustomToolReports", "top")) myLogForm.Top = MogUtils_Settings.LoadIntSetting("MOGLayout", "CustomToolReports", "top");
						if (MogUtils_Settings.SettingExist("MOGLayout", "CustomToolReports", "left")) myLogForm.Left = MogUtils_Settings.LoadIntSetting("MOGLayout", "CustomToolReports", "left");
					}
					#endregion ReportForm
					break;
				case "AssetProperties":
					#region AssetPropertiesForm
					AssetPropertiesForm myPropertiesForm = form as AssetPropertiesForm;
					if (myPropertiesForm != null)
					{
						if (MogUtils_Settings.SettingExist("MOGLayout", "AssetProperties", "SimplePropertiesMode"))
							myPropertiesForm.ViewPropertiesInSimpleMode = MogUtils_Settings.LoadBoolSetting("MOGLayout", "AssetProperties", "SimplePropertiesMode", true);
						if (MogUtils_Settings.SettingExist("MOGLayout", "AssetProperties", "Width")) myPropertiesForm.Width = MogUtils_Settings.LoadIntSetting("MOGLayout", "AssetProperties", "Width");
						if (MogUtils_Settings.SettingExist("MOGLayout", "AssetProperties", "Height")) myPropertiesForm.Height = MogUtils_Settings.LoadIntSetting("MOGLayout", "AssetProperties", "Height");
						//if (MogUtils_Settings.SettingExist("MOGLayout", "AssetProperties", "top")) myPropertiesForm.Top = MogUtils_Settings.LoadIntSetting("MOGLayout", "AssetProperties", "top"));
						//if (MogUtils_Settings.SettingExist("MOGLayout", "AssetProperties", "left")) myPropertiesForm.Left = MogUtils_Settings.LoadIntSetting("MOGLayout", "AssetProperties", "left"));
						if (MogUtils_Settings.SettingExist("MOGLayout", "AssetProperties", "ActiveTab"))
						{
							int tabIndex = MogUtils_Settings.LoadIntSetting("MOGLayout", "AssetProperties", "ActiveTab");
							if (tabIndex < myPropertiesForm.PropertiesTabControl.TabPages.Count)
							{
								myPropertiesForm.PropertiesTabControl.SelectedIndex = tabIndex;
							}
						}
					}
					#endregion AssetPropertiesForm
					break;
				case "AssetImporter":
					#region AssetImporter
					ImportAssetTreeForm myImportForm = form as ImportAssetTreeForm;
					if (myImportForm != null)
					{
						if (MogUtils_Settings.SettingExist("MOGLayout", "AssetImporter", "Width")) myImportForm.Width = MogUtils_Settings.LoadIntSetting("MOGLayout", "AssetImporter", "Width");
						if (MogUtils_Settings.SettingExist("MOGLayout", "AssetImporter", "Height")) myImportForm.Height = MogUtils_Settings.LoadIntSetting("MOGLayout", "AssetImporter", "Height");
						if (MogUtils_Settings.SettingExist("MOGLayout", "AssetImporter", "Project") && string.Compare(MogUtils_Settings.LoadSetting("MOGLayout", "AssetImporter", "Project"), MOG_ControllerProject.GetProjectName(), true) == 0)
						{
							// Store our project name so we can store these settings specific to this project
							string projectName = MOG_ControllerProject.GetProjectName();
							if (MogUtils_Settings.SettingExist("MOGLayout", "AssetImporter", projectName + "_Advanced")) myImportForm.mAdvancedOpened = MogUtils_Settings.LoadBoolSetting("MOGLayout", "AssetImporter", projectName + "_Advanced", false);
						}
					}
					#endregion AssetImporter
					break;
				case "CheckoutComments:Checkout":
					#region CheckoutComments
					AddCommentForm myCheckoutComments = form as AddCommentForm;
					if (myCheckoutComments != null)
					{
						if (MogUtils_Settings.SettingExist("MOGLayout", "CheckoutComments", "Edit")) myCheckoutComments.Edit = MogUtils_Settings.LoadBoolSetting("MOGLayout", "CheckoutComments", "Edit", false);
						if (MogUtils_Settings.SettingExist("MOGLayout", "CheckoutComments", "SavedComment")) myCheckoutComments.CommentsRichTextBox.Text = MogUtils_Settings.LoadSetting("MOGLayout", "CheckoutComments", "SavedComment");
					}
					#endregion
					break;
				case "CheckoutComments:Checkin":
					#region CheckinComments
					AddCommentForm myCheckinComments = form as AddCommentForm;
					if (myCheckinComments != null)
					{
						if (MogUtils_Settings.SettingExist("MOGLayout", "CheckoutComments", "Edit")) myCheckinComments.Edit = MogUtils_Settings.LoadBoolSetting("MOGLayout", "CheckoutComments", "Edit", false);
						if (MogUtils_Settings.SettingExist("MOGLayout", "CheckinComments", "SavedComment")) myCheckinComments.CommentsRichTextBox.Text = MogUtils_Settings.LoadSetting("MOGLayout", "CheckinComments", "SavedComment");
					}
					#endregion
					break;
				case "CheckoutComments:Lock":
					#region LockComments
					AddCommentForm myLockComments = form as AddCommentForm;
					if (myLockComments != null)
					{
						if (MogUtils_Settings.SettingExist("MOGLayout", "CheckoutComments", "Edit")) myLockComments.Edit = MogUtils_Settings.LoadBoolSetting("MOGLayout", "CheckoutComments", "Edit", false);
						if (MogUtils_Settings.SettingExist("MOGLayout", "LockComments", "SavedComment")) myLockComments.CommentsRichTextBox.Text = MogUtils_Settings.LoadSetting("MOGLayout", "LockComments", "SavedComment");
					}
					#endregion
					break;
				case PackageManagementForm_Text:
					#region PackageManagementForm
					PackageManagementForm myPackageForm = form as PackageManagementForm;
					if (myPackageForm != null)
					{
						string projectSpecificSection = PackageManagementForm_Text + "_" + MOG_ControllerProject.GetProjectName();
						if (MogUtils_Settings.SettingExist("MOGLayout", projectSpecificSection, "SelectedClassification"))
						{
							myPackageForm.SelectedClassification = MogUtils_Settings.LoadSetting("MOGLayout", projectSpecificSection, "SelectedClassification");
						}
					}
					#endregion
					break;
				case "LibraryManager":
					#region LibraryManager
					MogMainForm myLibMainForm = form as MogMainForm;
					if (myLibMainForm != null)
					{
						string LastSelectedFolder = MogUtils_Settings.LoadSetting("LibraryListView", "InitialTree", "Start");
						TreeNode lastSelectedNode = myLibMainForm.LibraryExplorer.LibraryListView.LibraryExplorer.LibraryTreeView.DrillToNodePath(LastSelectedFolder);
						myLibMainForm.LibraryExplorer.LibraryListView.LibraryExplorer.LibraryTreeView.SelectedNode = lastSelectedNode;

						string autoImport = MogUtils_Settings.LoadSetting("LibraryListView", "AutoImport");
						if (autoImport.Length > 0) myLibMainForm.LibraryExplorer.AutoImport = Convert.ToBoolean(autoImport);
					}
					#endregion
					break;
				case "BlessInfo":
					#region BlessInfoForm
					BlessInfoForm mBlessInfoForm = form as BlessInfoForm;
					if (mBlessInfoForm != null)
					{
						mBlessInfoForm.Width = MogUtils_Settings.LoadIntSetting("MOGLayout", "BlessInfo", "Width", mBlessInfoForm.Width);
						mBlessInfoForm.Height = MogUtils_Settings.LoadIntSetting("MOGLayout", "BlessInfo", "Height", mBlessInfoForm.Height);
						mBlessInfoForm.Top = MogUtils_Settings.LoadIntSetting("MOGLayout", "BlessInfo", "top", mBlessInfoForm.Top);
						mBlessInfoForm.Left = MogUtils_Settings.LoadIntSetting("MOGLayout", "BlessInfo", "left", mBlessInfoForm.Left);
						mBlessInfoForm.BlessSplitter.SplitPosition = MogUtils_Settings.LoadIntSetting("MOGLayout", "BlessInfo", "splitter", mBlessInfoForm.BlessSplitter.SplitPosition);

						MogUtils_Settings.LoadListView("BlessInfoBlessFilesCheckedListView", mBlessInfoForm.BlessInfoBlessFilesCheckedListView);
					}
					#endregion BlessInfoForm
					break;
			}

			return true;
		}
		#endregion
		#region Savers

		#region Page prefs saving functions
		private void Save_Global()
		{
			MogUtils_Settings.SaveSetting("GuiLayout", "Gui", "ActiveTab", Convert.ToString(mainForm.MOGTabControl.SelectedIndex));
			// If this is a maximized window, save the width and height minus a few pixels
			//if( mainForm.WindowState == FormWindowState.Maximized )
			//{
			//    MogUtils_Settings.SaveSetting("GuiLayout", "Gui", "Width", Convert.ToString(mainForm.Width - 8));
			//    MogUtils_Settings.SaveSetting("GuiLayout", "Gui", "Height", Convert.ToString(mainForm.Height - 8));
			//}
			//// Else save normally...
			//else
			//{
			//    MogUtils_Settings.SaveSetting("GuiLayout", "Gui", "Width", Convert.ToString(mainForm.Width));
			//    MogUtils_Settings.SaveSetting("GuiLayout", "Gui", "Height", Convert.ToString(mainForm.Height));
			//}

			//MogUtils_Settings.SaveSetting("GuiLayout", "Gui", "top", Convert.ToString(mainForm.Top));
			//MogUtils_Settings.SaveSetting("GuiLayout", "Gui", "left", Convert.ToString(mainForm.Left));

//			// For some unknown reason, MOG crashes when windowState=Maxiimzed && minimizeToSystemTrayToolStripMenuItem=true
//			// Both Kier and I have given up on this one so it is simply easier to just not save this GUI setting
//			MogUtils_Settings.SaveSetting("GuiLayout", "Gui", "windowState", Convert.ToString(((int)mainForm.WindowState)));
			MogUtils_Settings.SaveSetting("GuiLayout", "Gui", "SoundTheme", mainForm.mSoundScheme);
			MogUtils_Settings.SaveSetting("GuiDynamicOptions", "Gui", "Sounds", mainForm.soundEnableToolStripMenuItem.Checked.ToString());

			MogUtils_Settings.SaveSetting("Client", "MinimiseToSystemTray", mainForm.minimizeToSystemTrayToolStripMenuItem.Checked.ToString());
			 
            //
			// Tools
			//
		}

		private void Save_Workspace()
		{
			//
			// WorkSpace
			//
			if (mainForm != null)
			{
				MogUtils_Settings.SaveSetting("GuiLayout", "Workspace", "ActiveInboxTab", Convert.ToString(mainForm.AssetManagerInboxTabControl.SelectedIndex));
				MogUtils_Settings.SaveSetting("GuiLayout", "Workspace", "ActiveBox", Convert.ToString(mainForm.AssetManagerInboxTabControl.SelectedIndex));
				MogUtils_Settings.SaveSetting("GuiLayout", "Workspace", "AutoProcess", Convert.ToString(mainForm.AssetManagerAutoProcessCheckBox.Checked));
				MogUtils_Settings.SaveSetting("GuiLayout", "Workspace", "AutoImport", Convert.ToString(mainForm.AssetManagerAutoImportCheckBox.Checked));
				MogUtils_Settings.SaveSetting("GuiLayout", "Workspace", "AutoUpdate", Convert.ToString(mainForm.AssetManagerAutoUpdateLocalCheckBox.Checked));
				MogUtils_Settings.SaveSetting("GuiLayout", "Workspace", "AutoPackage", Convert.ToString(mainForm.AssetManagerAutoPackageCheckBox.Checked));
				MogUtils_Settings.SaveSetting("GuiLayout", "Workspace", "PackageMerge", Convert.ToString(mainForm.AssetManagerPackageButton.Enabled));

				if (mainForm.CustomToolsBox != null)
				{
					MogUtils_Settings.SaveSetting("GuiLayout", "Workspace", "ToolBoxWidth", mainForm.CustomToolsBox.Width.ToString());
				}

				MogUtils_Settings.SaveSetting("GuiLayout", "Groups", "Enabled", mainForm.showGroupsToolStripMenuItem.Checked.ToString());

				if (mainForm.mAssetManager != null)
				{
					if (mainForm.mAssetManager.mGroups != null)
					{
						MogUtils_Settings.SaveSetting("GuiDynamicOptions", "Groups", "DraftsBoxGroup", mainForm.mAssetManager.mGroups.GetGroup(mainForm.AssetManagerDraftsListView));
						MogUtils_Settings.SaveSetting("GuiDynamicOptions", "Groups", "InBoxGroup", mainForm.mAssetManager.mGroups.GetGroup(mainForm.AssetManagerInboxListView));
						MogUtils_Settings.SaveSetting("GuiDynamicOptions", "Groups", "SentBoxGroup", mainForm.mAssetManager.mGroups.GetGroup(mainForm.AssetManagerSentListView));
						MogUtils_Settings.SaveSetting("GuiDynamicOptions", "Groups", "TrashBoxGroup", mainForm.mAssetManager.mGroups.GetGroup(mainForm.AssetManagerTrashListView));
					}
				}
			}
		}


		private void Save_Project()
		{
			//
			// Project Manager
			//
			//MogUtils_Settings.SaveSetting("GuiLayout", "ProjectManager", "SelectedRepositoryView", Convert.ToString(mainForm.ProjectManagerRepositoryViewComboBox.SelectedIndex));
			//MogUtils_Settings.SaveSetting("GuiLayout", "ProjectManager", "AutoUpdateTrees", mainForm.ProjectManagerAutoUpdateTreesCheckBox.Checked.ToString());

		}

		private void Save_Library()
		{
			//
			// Library Manager
			//
			if (mainForm.LibraryExplorer.LibraryListView.LibraryExplorer != null &&
				mainForm.LibraryExplorer.LibraryListView.LibraryExplorer.LibraryTreeView != null)
			{
				MogUtils_Settings.SaveSetting("LibraryListView", "InitialTree", "Start", mainForm.LibraryExplorer.LibraryTreeView.LastNodePath);
				MogUtils_Settings.SaveSetting("LibraryListView", "AutoImport", mainForm.LibraryExplorer.AutoImport.ToString());
			}
		}

		private void Save_Connections()
		{
			//
			// Connection Manager
			//
			MogUtils_Settings.SaveSetting("ConnectionManager", "ConnectionsBottomTabControlSelectedIndex", mainForm.ConnectionsBottomTabControl.SelectedIndex.ToString());
		}

		#endregion

		public void SaveStaticForm_LayoutPrefs()
		{
			Save_Global();

			Save_Workspace();

			Save_Project();

			Save_Library();

			Save_Connections();
		}

		static public void SaveStatic_ProjectPrefs()
		{
			if (MOG_ControllerProject.GetProject() != null) MogUtils_Settings.SaveSetting("MOG", "Project", MOG_ControllerProject.GetProjectName());
			if (MOG_ControllerProject.GetActiveUser() != null) MogUtils_Settings.SaveSetting("MOG", "ActiveUser", MOG_ControllerProject.GetActiveUser().GetUserName());
			if (MOG_ControllerProject.GetUser() != null) MogUtils_Settings.SaveSetting("MOG", "LoginUser", MOG_ControllerProject.GetUser().GetUserName());

			MogUtils_Settings.Settings.Save();
		}

		static public bool SaveDynamic_LayoutPrefs(string key, Form form)
		{
			try
			{
				switch (key)
				{
				case "AssetManager":
					#region AssetManager
					MogMainForm myMainForm = form as MogMainForm;
					if (myMainForm != null)
					{
						MogUtils_Settings.SaveListView("WorkspaceInbox", myMainForm.AssetManagerInboxListView);
						MogUtils_Settings.SaveListView("WorkspaceDrafts", myMainForm.AssetManagerDraftsListView);
						MogUtils_Settings.SaveListView("WorkspaceOutbox", myMainForm.AssetManagerSentListView);
						MogUtils_Settings.SaveListView("WorkspaceTrash", myMainForm.AssetManagerTrashListView);

						if (myMainForm.mAssetManager != null)
						{
							MogUtils_Settings.SaveSetting("MOGLayout", "Workspace", "updateBuildType", myMainForm.mAssetManager.mDefaultUpdateBuildType);

							MogUtils_Settings.SaveSetting("MOGLayout", "Workspace", "updateBuildHidePlatforms", myMainForm.mAssetManager.mHidePlatforms.ToString());

							if (myMainForm.mAssetManager.mDraftsSortManager != null)
							{
								MogUtils_Settings.SaveSetting("MOGLayout", "Workspace", "DraftsSort", myMainForm.mAssetManager.mDraftsSortManager.SortOrder.ToString());
								MogUtils_Settings.SaveSetting("MOGLayout", "Workspace", "DraftsSortColumn", myMainForm.mAssetManager.mDraftsSortManager.Column.ToString());
							}
							if (myMainForm.mAssetManager.mInboxSortManager != null)
							{
								MogUtils_Settings.SaveSetting("MOGLayout", "Workspace", "InboxSort", myMainForm.mAssetManager.mInboxSortManager.SortOrder.ToString());
								MogUtils_Settings.SaveSetting("MOGLayout", "Workspace", "InboxSortColumn", myMainForm.mAssetManager.mInboxSortManager.Column.ToString());
							}
							if (myMainForm.mAssetManager.mOutboxSortManager != null)
							{
								MogUtils_Settings.SaveSetting("MOGLayout", "Workspace", "OutboxSort", myMainForm.mAssetManager.mOutboxSortManager.SortOrder.ToString());
								MogUtils_Settings.SaveSetting("MOGLayout", "Workspace", "OutboxSortColumn", myMainForm.mAssetManager.mOutboxSortManager.Column.ToString());
							}
							if (myMainForm.mAssetManager.mTrashSortManager != null)
							{
								MogUtils_Settings.SaveSetting("MOGLayout", "Workspace", "TrashSort", myMainForm.mAssetManager.mTrashSortManager.SortOrder.ToString());
								MogUtils_Settings.SaveSetting("MOGLayout", "Workspace", "TrashSortColumn", myMainForm.mAssetManager.mTrashSortManager.Column.ToString());
							}

							if (myMainForm.mAssetManager.mToDo != null)
							{
								myMainForm.mAssetManager.mToDo.Save(MogUtils_Settings.Settings, "MOGLayout", "Workspace", "ToDo");
							}
							if (myMainForm.mAssetManager.mLocalExplorer != null)
							{
								myMainForm.mAssetManager.mLocalExplorer.Save(MogUtils_Settings.Settings, "MOGLayout", "Workspace", "LocalExplorer");
							}
							if (myMainForm.mAssetManager.mTools != null)
							{
								myMainForm.mAssetManager.mTools.Save(MogUtils_Settings.Settings, "MOGLayout", "Workspace", "LocalTools");
							}
						}
					}
					#endregion
					break;
				case "LibraryManager":
					#region LibraryManager
					MogMainForm myLibMainForm = form as MogMainForm;
					if (myLibMainForm != null)
					{
						MogUtils_Settings.SaveListView("LibraryListView", myLibMainForm.LibraryExplorer.LibraryListView.LibraryListView);
						if (myLibMainForm.LibraryExplorer.LibraryTreeView.SelectedNode != null)
						{
							MogUtils_Settings.SaveSetting("LibraryListView", "InitialTree", "Start", myLibMainForm.LibraryExplorer.LibraryTreeView.SelectedNode.FullPath);
						}
					}
					#endregion
					break;
				case "NewAssetList":
					#region NewAssetList
					NewListForm myForm = form as NewListForm;
					if (myForm != null)
					{
						MogUtils_Settings.SaveSetting("MOGLayout", "AssetList", "NoRange", myForm.ListTimeNoRangeRadioButton.Checked.ToString());
						MogUtils_Settings.SaveSetting("MOGLayout", "AssetList", "SpecifyRange", myForm.ListDateRangeRadioButton.Checked.ToString());
						MogUtils_Settings.SaveSetting("MOGLayout", "AssetList", "Start", myForm.ListStartDateTimePicker.Value.ToString());
						MogUtils_Settings.SaveSetting("MOGLayout", "AssetList", "SpecifyTimeRange", myForm.ListTimeRangeRadioButton.Checked.ToString());
						MogUtils_Settings.SaveSetting("MOGLayout", "AssetList", "TimeRange", myForm.ListHoursTextBox.Text);
						MogUtils_Settings.SaveSetting("MOGLayout", "AssetList", "End", myForm.ListEndDateTimePicker.Value.ToString());
						MogUtils_Settings.SaveSetting("MOGLayout", "AssetList", "FilterEnable", myForm.ListFilterCheckBox.Checked.ToString());
						MogUtils_Settings.SaveSetting("MOGLayout", "AssetList", "Filter", myForm.ListFilterTextBox.Text);
					}
					#endregion
					break;
				case "ReportForm":
					#region ReportForm
					Report myLogForm = form as Report;
					if (myLogForm != null)
					{
						MogUtils_Settings.SaveSetting("MOGLayout", "CustomToolReports", "Width", Convert.ToString(myLogForm.Width));
						MogUtils_Settings.SaveSetting("MOGLayout", "CustomToolReports", "Height", Convert.ToString(myLogForm.Height));
						MogUtils_Settings.SaveSetting("MOGLayout", "CustomToolReports", "top", Convert.ToString(myLogForm.Top));
						MogUtils_Settings.SaveSetting("MOGLayout", "CustomToolReports", "left", Convert.ToString(myLogForm.Left));
					}
					#endregion
					break;
				case "AssetProperties":
					#region AssetProperties
					AssetPropertiesForm myPropertiesForm = form as AssetPropertiesForm;
					if (myPropertiesForm != null)
					{
						MogUtils_Settings.SaveSetting("MOGLayout", "AssetProperties", "Width", Convert.ToString(myPropertiesForm.Width));
						MogUtils_Settings.SaveSetting("MOGLayout", "AssetProperties", "Height", Convert.ToString(myPropertiesForm.Height));
						MogUtils_Settings.SaveSetting("MOGLayout", "AssetProperties", "top", Convert.ToString(myPropertiesForm.Top));
						MogUtils_Settings.SaveSetting("MOGLayout", "AssetProperties", "left", Convert.ToString(myPropertiesForm.Left));
						MogUtils_Settings.SaveSetting("MOGLayout", "AssetProperties", "ActiveTab", Convert.ToString(myPropertiesForm.PropertiesTabControl.SelectedIndex));

						MogUtils_Settings.SaveSetting("MOGLayout", "AssetProperties", "SimplePropertiesMode", Convert.ToString(myPropertiesForm.ViewPropertiesInSimpleMode));
					}
					#endregion
					break;
				case "AssetImporter":
					#region AssetImporter
					ImportAssetTreeForm myImportForm = form as ImportAssetTreeForm;
					if (myImportForm != null)
					{
						MogUtils_Settings.SaveSetting("MOGLayout", "AssetImporter", "Width", Convert.ToString(myImportForm.Width));
						MogUtils_Settings.SaveSetting("MOGLayout", "AssetImporter", "Height", Convert.ToString(myImportForm.Height));
						MogUtils_Settings.SaveSetting("MOGLayout", "AssetImporter", "top", Convert.ToString(myImportForm.Top));
						MogUtils_Settings.SaveSetting("MOGLayout", "AssetImporter", "left", Convert.ToString(myImportForm.Left));
						// Save our project-specific settings
						string projectName = MOG_ControllerProject.GetProjectName();
						if (projectName != null)
						{
							MogUtils_Settings.SaveSetting("MOGLayout", "AssetImporter", "Project", projectName);
							MogUtils_Settings.SaveSetting("MOGLayout", "AssetImporter", projectName + "_Advanced", Convert.ToString(myImportForm.mAdvancedOpened));
						}
					}
					#endregion
					break;
				case "CheckoutComments:Checkout":
					#region CheckoutComments
					AddCommentForm myCheckoutComments = form as AddCommentForm;
					if (myCheckoutComments != null)
					{
						MogUtils_Settings.SaveSetting("MOGLayout", "CheckoutComments", "Edit", Convert.ToString(myCheckoutComments.Edit));
						MogUtils_Settings.SaveSetting("MOGLayout", "CheckoutComments", "SavedComment", myCheckoutComments.CommentsRichTextBox.Text);
					}
					#endregion
					break;
				case "CheckoutComments:Checkin":
					#region CheckinComments
					AddCommentForm myCheckinComments = form as AddCommentForm;
					if (myCheckinComments != null)
					{
						MogUtils_Settings.SaveSetting("MOGLayout", "CheckoutComments", "Edit", Convert.ToString(myCheckinComments.Edit));
						MogUtils_Settings.SaveSetting("MOGLayout", "CheckinComments", "SavedComment", myCheckinComments.CommentsRichTextBox.Text);
					}
					#endregion
					break;
				case "CheckoutComments:Lock":
					#region LockComments
					AddCommentForm myLockComments = form as AddCommentForm;
					if (myLockComments != null)
					{
						MogUtils_Settings.SaveSetting("MOGLayout", "CheckoutComments", "Edit", Convert.ToString(myLockComments.Edit));
						MogUtils_Settings.SaveSetting("MOGLayout", "LockComments", "SavedComment", myLockComments.CommentsRichTextBox.Text);
					}
					#endregion
					break;
				case PackageManagementForm_Text:
					#region PackageManagementForm
					// Save out the last selected classification in the PackageManagementForm
					PackageManagementForm myPackageForm = form as PackageManagementForm;
					if (myPackageForm != null)
					{
						string projectSpecificSection = PackageManagementForm_Text + "_" + MOG_ControllerProject.GetProjectName();
						MogUtils_Settings.SaveSetting("MOGLayout", projectSpecificSection, "SelectedClassification", myPackageForm.SelectedClassification);
					}
					#endregion
					break;
				case "BlessInfo":
					#region BlessInfoForm
					BlessInfoForm mBlessInfoForm = form as BlessInfoForm;
					if (mBlessInfoForm != null)
					{
						MogUtils_Settings.SaveSetting("MOGLayout", "BlessInfo", "Width", mBlessInfoForm.Width.ToString());
						MogUtils_Settings.SaveSetting("MOGLayout", "BlessInfo", "Height", mBlessInfoForm.Height.ToString());
						MogUtils_Settings.SaveSetting("MOGLayout", "BlessInfo", "top", mBlessInfoForm.Top.ToString());
						MogUtils_Settings.SaveSetting("MOGLayout", "BlessInfo", "left", mBlessInfoForm.Left.ToString());
						MogUtils_Settings.SaveSetting("MOGLayout", "BlessInfo", "splitter", mBlessInfoForm.BlessSplitter.SplitPosition.ToString());

						MogUtils_Settings.SaveListView("BlessInfoBlessFilesCheckedListView", mBlessInfoForm.BlessInfoBlessFilesCheckedListView);
					}
					#endregion BlessInfoForm
					break;
				}

				MogUtils_Settings.Settings.Save();
			}
			catch
			{
			}
			return true;
		}
		#endregion

		internal static string LoadPref(string section, string key)
		{
			return MogUtils_Settings.LoadSetting(section, key);
		}

		internal static string LoadPref(string section, string property, string key)
		{
			return MogUtils_Settings.LoadSetting(section, property, key);
		}
	}
}
