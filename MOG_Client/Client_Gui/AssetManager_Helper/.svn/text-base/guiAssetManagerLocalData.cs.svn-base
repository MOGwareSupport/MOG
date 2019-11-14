using System;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Collections;
using System.IO;
using Microsoft.VisualBasic;

using EV.Windows.Forms;
using MOG_Client.Forms;

using MOG;
using MOG.INI;
using MOG.TIME;
using MOG.REPORT;
using MOG.PROMPT;
using MOG.FILENAME;
using MOG.PLATFORM;
using MOG.PROPERTIES;
using MOG.DOSUTILS;
using MOG.DATABASE;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERLIBRARY;
using MOG.CONTROLLER.CONTROLLERASSET;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.CONTROLLER.CONTROLLERSYNCDATA;
using MOG.ASSET_STATUS;

using MOG_Client.Client_Mog_Utilities;
using MOG_Client.Client_Mog_Utilities.AssetOptions;
using MOG_Client.Client_Utilities;

using MOG_ControlsLibrary;
using MOG_ControlsLibrary.Common.MogControl_RepositoryTreeViews;
using MOG_ControlsLibrary.Utils;
using MOG_ControlsLibrary.Common.MOG_ContextMenu;
using MOG_ControlsLibrary.Common.MogControl_LocalBranchTreeView;

// Module for getting system icons for the explorer view
using Etier.IconHelper;
using MOG_CoreControls;
using MOG_ControlsLibrary.MogUtils_Settings;
using MOG_Client.Utilities;
using MOG.COMMAND;


namespace MOG_Client.Client_Gui.AssetManager_Helper
{
	/// <summary>
	/// Summary description for guiAssetManagerLocalData.
	/// </summary>
	public class guiAssetManagerLocalData
	{
		private guiAssetManager mParent;
		private bool mCreatingWorkspaceTabs = false;
		public ListView mPlatformListView;
		public ImageList mPlatformImages;
		public ArrayList mListViewSort_Manager;

		private MogControl_AssetContextMenu mAssetMenu;

		private const string LocalBranch_NewWorkspace		= "New Workspace";
		private const string LocalBranch_DeleteWorkspace	= "Remove Workspace";
		private const string LocalBranch_GetLatest			= "GetLatest";
		private const string LocalBranch_CleanWorkspace		= "Clean";
		private const string LocalBranch_Refresh			= "Refresh";
		private const string LocalBranch_Explore			= "Explore";
		private const string LocalBranch_GenerateReport		= "Generate Report";

		private enum LocalMenuItems 
		{
			GetLatest,
			Explore,
			Clean,
			Sep1,
			Refresh,
			Sep3,
			CreateWorkspace,
			RemoveWorkspace
		};

		public bool LocalBranchImageShow
		{
			get 
			{
				return mParent.mainForm.SkinMyWorkspacePictureBox.Visible;
			}

			set
			{
				if (value)
				{
					// Show picture
					mParent.mainForm.AssetManagerLocalDataTabControl.Visible = false;
					mParent.mainForm.AssetManagerLocalDataTabControl.Dock = DockStyle.None;
				}
				else
				{
					mParent.mainForm.AssetManagerLocalDataTabControl.Visible = true;
					mParent.mainForm.AssetManagerLocalDataTabControl.Dock = DockStyle.Fill;
				}
			}

		}

		#region Initializers
				/// <summary>
		/// Base constructor of the local data manager
		/// </summary>
		/// <param name="Parent"></param>
		public guiAssetManagerLocalData(guiAssetManager Parent)
		{
			// These are the default settings for the local data window
			mParent = Parent;
			mAssetMenu  = new MogControl_AssetContextMenu("NAME, CLASS, TARGETPATH, DATE, SIZE, STATE, PLATFORM, CREATOR, RESPPARTY, OPTIONS, FULLNAME, BOX, GROUP}");
			mListViewSort_Manager = new ArrayList();
		}
	
		/// <summary>
		/// InitializePlatforms
		/// This routine creates a tab page to the platforms tab
		/// control for each platform listed in the project.ini
		/// </summary>
		public void InitializePotentialLocalBranches()
		{
			// Check for active project
			if (!MOG_ControllerProject.IsProject())
			{
				return;
			}

			// ************************************** Context menu ************************************************
//			mParent.mainForm.LocalBranchExploreCreateMenuItem.MenuItems.Clear();

			// Create our contextMenu
			ContextMenu LocalDataMenu = new ContextMenu();
			MenuItem LocalBranchExploreCreateNewBranchMenuItem = new MenuItem(LocalBranch_NewWorkspace, new EventHandler(LocalBranchExploreCreateNewMenuItem_Click));
			MenuItem LocalBranchExploreRemoveMenuItem = new MenuItem(LocalBranch_DeleteWorkspace, new EventHandler(LocalBranchExploreRemoveMenuItem_Click), Shortcut.Del);
			  
			MenuItem LocalBranchExploreSyncMenuItem = new MenuItem(LocalBranch_GetLatest, new EventHandler(LocalBranchExploreSyncMenuItem_Click), Shortcut.CtrlS);
			MenuItem LocalBranchExploreExploreMenuItem = new MenuItem(LocalBranch_Explore, new EventHandler(LocalBranchExploreExploreMenuItem_Click), Shortcut.CtrlE);
			MenuItem LocalBranchExploreCleanMenuItem = new MenuItem(LocalBranch_CleanWorkspace, new EventHandler(LocalBranchExploreCleanMenuItem_Click), Shortcut.CtrlC);
//			MenuItem LocalBranchExploreRemoveMenuItem = new MenuItem(LocalBranch_DeleteWorkspace, new EventHandler(LocalBranchExploreRemoveMenuItem_Click), Shortcut.Del);
			MenuItem LocalBranchRefreshTabMenuItem = new MenuItem(LocalBranch_Refresh, new EventHandler(LocalBranchRefreshMenuItem_Click), Shortcut.F5);
			MenuItem LocalBranchGenerateReportMenuItem = new MenuItem(LocalBranch_GenerateReport, new EventHandler(LocalBranchGenerateReportMenuItem_Click), Shortcut.CtrlG);


			LocalDataMenu.MenuItems.Add(LocalBranchExploreSyncMenuItem);
			LocalDataMenu.MenuItems.Add(LocalBranchExploreExploreMenuItem);
//			LocalDataMenu.MenuItems.Add(LocalBranchExploreRemoveMenuItem);
			LocalDataMenu.MenuItems.Add(LocalBranchExploreCleanMenuItem);
//			LocalDataMenu.MenuItems.Add("-");
//			LocalDataMenu.MenuItems.Add(LocalBranchGenerateReportMenuItem);
			LocalDataMenu.MenuItems.Add("-");
			LocalDataMenu.MenuItems.Add(LocalBranchRefreshTabMenuItem);
			LocalDataMenu.MenuItems.Add("-");
			LocalDataMenu.MenuItems.Add(LocalBranchExploreCreateNewBranchMenuItem);
			LocalDataMenu.MenuItems.Add(LocalBranchExploreRemoveMenuItem);

            // Set light version control            
			MogUtil_VersionInfo.SetLightVersionControl(mParent.mainForm.LocalBranchAddMenuItem);

			// Set our context menu
			mParent.mainForm.LocalBranchMogControl_GameDataDestinationTreeView.MogContextMenu = LocalDataMenu;
			mParent.mainForm.LocalBranchMogControl_GameDataDestinationTreeView.MogContextMenu.Popup += new EventHandler(LocalBranchExploreContextMenu_Popup);
		}

		#endregion

		#region Refresh local updated window
		public void RefreshWindow()
		{
			// Skip this if we don't have an active project
			if (MOG_ControllerProject.GetProject() == null || 
				MOG_ControllerProject.GetActiveUser() == null ||
				MOG_ControllerProject.GetCurrentSyncDataController() == null )
			{
				return;
			}

			// Make sure we have a current window to refresh
			if (mPlatformListView == null)
			{
				return;
			}

			RefreshWindowToolbar();

			mPlatformListView.BeginUpdate();			

			// Clear the desired listBox
			mPlatformListView.Items.Clear();

			// Populate our local updated window with what the user has updated to
			RefreshLocalInbox();

			mPlatformListView.EndUpdate();
		}

		public void RefreshWindowToolbar()
		{
			// Get a handle to the tab page
			TabPage activeLocalBranch = mParent.mainForm.AssetManagerLocalDataTabControl.SelectedTab;
			if (activeLocalBranch != null)
			{
				MogControl_LocalWorkspaceTab workspace = (MogControl_LocalWorkspaceTab)LocateControl(activeLocalBranch, "MogControl_LocalWorkspaceTab");
				if (workspace != null)
				{
					workspace.RefreshWorkspaceToolbar();
				}
			}
		}

		public void RefreshLockStatus(MOG_Command LockUpdate)
		{
			Color textColor = Color.Black;

			if (LockUpdate != null && LockUpdate.GetCommand() != null)
			{
				MOG_Filename assetName = LockUpdate.GetCommand().GetAssetFilename();
				ListView currentView = mPlatformListView;

				if (currentView == null)
					return;

				int index = mParent.ListViewItemFindItem(assetName.GetAssetLabel(), currentView);
				if (index != -1)
				{
					currentView.Items[index].ImageIndex = MogUtil_AssetIcons.GetAssetIconIndex(assetName.GetOriginalFilename());
				}
			}
		}

		/// <summary>
		/// Ask the database for all files that have been manually updated for this user and his platform and target directory
		/// </summary>
		/// <param name="textColor"></param>
		private void RefreshLocalInbox()
		{
			string filter = MOG_ControllerProject.GetCurrentSyncDataController().GetSyncDirectory() + "\\*";
			ArrayList assets = MOG_DBInboxAPI.InboxGetAssetListWithProperties("Local", MOG_ControllerProject.GetCurrentSyncDataController().GetUserName(), null, filter);

			// If the .info file exists, open it
			if (assets!= null && assets.Count > 0)
			{
				mPlatformListView.BeginUpdate();

				foreach (MOG_DBInboxProperties asset in assets)
				{
					MOG_Properties property = new MOG_Properties(asset.mProperties);

					// Add item to list view
					ListViewItem node = guiAssetManager.NewListViewItem(asset.mAsset, property.Status, property);
					mPlatformListView.Items.Add(node);
				}

				mPlatformListView.EndUpdate();
			}
		}
		#endregion

		#region Local Branch related functions
		#region Local branch creation / loading
		/// <summary>
		/// Create a local branch tab in the local data window
		/// </summary>
		public bool CreateLocalBranch(MOG_ControllerSyncData gameDataHandle)
		{
            // Check if there is a colliding workspace at this location?
			if (MOG_DBSyncedDataAPI.DoesSyncedLocationExist(MOG_ControllerSystem.GetComputerName(), gameDataHandle.GetProjectName(), gameDataHandle.GetPlatformName(), gameDataHandle.GetSyncDirectory(), "") == true)
			{
				// Get the full details of the collising workspace?
				ArrayList locations = MOG_DBSyncedDataAPI.GetAllSyncedLocations(MOG_ControllerSystem.GetComputerName(), gameDataHandle.GetSyncDirectory(), "", "");
				if (locations != null && locations.Count > 0)
				{
					MOG_DBSyncedLocationInfo syncLocation = locations[0] as MOG_DBSyncedLocationInfo;

					// Check if this is a different user?
					if (string.Compare(MOG_ControllerProject.GetUserName_DefaultAdmin(), syncLocation.mUserName, true) != 0)
					{
						// Check to make sure this user is still an active user in the project?
						if (MOG_ControllerProject.GetProject().GetUser(syncLocation.mUserName) != null)
						{
							// Inform the user this workspace is colliding with this other user
							MOG_Prompt.PromptResponse("Cannot create a new workspace where another already exists!", "Colliding Workspace:\n" +
								"\n        COMPUTER:  " + syncLocation.mComputerName +
								"\n        DIRECTORY: " + syncLocation.mWorkingDirectory +
								"\n        PROJECT:   " + syncLocation.mProjectName +
								"\n        BRANCH:    " + syncLocation.mBranchName +
								"\n        USER:      " + syncLocation.mUserName,
								MOGPromptButtons.OK);

							return false;
						}
					}
				}
			}

			// Create the tab
			CreateLocalTab(gameDataHandle);

			// Inform the Database of our new local branch
			MOG_DBSyncedDataAPI.AddSyncedLocation(MOG_ControllerSystem.GetComputerName(), gameDataHandle.GetProjectName() , gameDataHandle.GetPlatformName(), gameDataHandle.GetSyncDirectory(), gameDataHandle.GetBranchName(), gameDataHandle.GetName(),gameDataHandle.GetUserName());
			return gameDataHandle.ValidateLocalWorkspace();
		}

		/// <summary>
		/// Search all the tabs of our localBranches tabView looking to see if there is already a tab of this name
		/// </summary>
		/// <param name="tabName"></param>
		/// <returns></returns>
		private bool LocalTabNameExist(string tabName)
		{
			foreach (TabPage page in mParent.mainForm.AssetManagerLocalDataTabControl.TabPages)
			{
				if (string.Compare(tabName, page.Text, true) == 0)
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Create a local branch tab in the local data window
		/// </summary>
		public void CreateLocalTab(MOG_ControllerSyncData gameDataHandle)
		{
			TabPage page = new TabPage();

			page.Name = gameDataHandle.GetBranchName() + "." + gameDataHandle.GetPlatformName();

			// Make sure to get a unique name
			string name = gameDataHandle.GetName();
			int count = 2;
			while (LocalTabNameExist(gameDataHandle.GetName()) == false)
			{
				gameDataHandle.SetName(name + count.ToString());
				count++;
			}

			page.Text = gameDataHandle.GetName();
			
			page.Tag = gameDataHandle;
			page.ToolTipText = gameDataHandle.GetSyncDirectory();
			
			string iconFilename = MOG_ControllerSystem.LocateTool("Images\\Platforms", gameDataHandle.GetPlatformName() + ".bmp");
			if (DosUtils.FileExist(iconFilename))
			{
				// Get the image
				Image myImage = new Bitmap(iconFilename);

				// Add the image and the type to the arrayLists
				mPlatformImages.Images.Add(myImage);

				page.ImageIndex = mPlatformImages.Images.Count - 1;
			}
			else
			{
				page.ImageIndex = 0;
			}

			// Create a new local workspace control
			MogControl_LocalWorkspaceTab workSpace = new MogControl_LocalWorkspaceTab(mParent.mainForm, gameDataHandle);
// JohnRen - Removed because this would trigger the local workspace to package merge on client startup regardless of AutoPackage state.
//			workSpace.RefreshWorkspaceToolbar();
			workSpace.Dock = DockStyle.Fill;

			// Add the view
			page.Controls.Add(workSpace);
			workSpace.BringToFront();

			// Add the Page
			mParent.mainForm.AssetManagerLocalDataTabControl.TabPages.Add(page);
			mParent.mainForm.AssetManagerLocalDataTabControl.SelectedTab = page;

			mParent.mGroups.Add(workSpace.WorkSpaceListView, "Group");

			LocalBranchImageShow = false;
		}

		/// <summary>
		/// Load all the local branches this machine knows about
		/// </summary>
		public void InitialiseLocalView()
		{
			// Initialize the project platforms image list for the local branches tab
			mPlatformImages = new ImageList();
			mParent.mainForm.AssetManagerLocalDataTabControl.ImageList = mPlatformImages;
			UnLoadUserLocalBranches();

			// Initialize our local version explorer window
			// KLK - We now initialize the local explorer tree view when the splitter control is opening
			// mParent.mainForm.LocalBranchMogControl_GameDataDestinationTreeView.InitializeProjectsOnly();
		}

		public void LoadUserLocalBranches()
		{
			ArrayList localBranches = LoadGameDataControllers();
			
			if (localBranches != null && localBranches.Count > 0)
			{
				mCreatingWorkspaceTabs = true;
				foreach (MOG_ControllerSyncData branch in localBranches)
				{
					// Only load local branches for this machine that are of the same projects as the current logged in project?
					if (string.Compare(branch.GetProjectName(), MOG_ControllerProject.GetProjectName(), true) == 0)
					{
						// Create the tab
						CreateLocalTab(branch);

						LocalBranchImageShow = false;
					}
				}

				mCreatingWorkspaceTabs = false;

				if (MOG_ControllerProject.GetCurrentSyncDataController() != null)
				{
					SetActiveLocalBranchTab(MOG_ControllerProject.GetCurrentSyncDataController());
				}
				else if (localBranches.Count > 0)
				{
					MOG_ControllerSyncData defaultBranch = localBranches[0] as MOG_ControllerSyncData;
					SetActiveLocalBranchTab(defaultBranch);
				}

				UpdateActiveLocalBranch();
			}
			else
			{
				// Make sure this project actually has a platform...
				if (MOG_ControllerProject.GetProject().GetPlatforms().Count > 0)
				{
					// Show picture
					LocalBranchImageShow = true;

					CreateNewWorkspaceWizard wizard = new CreateNewWorkspaceWizard();
					wizard.ShowDialog(mParent.mainForm);
					if (wizard.DialogResult == DialogResult.OK)
					{
						//Get the current user.
						string userName = MOG_ControllerProject.GetUserName_DefaultAdmin();

						// Add this new workspace
						MOG_ControllerSyncData workspace = WorkspaceManager.AddNewWorkspace(wizard.NewWorkspaceDirectory, wizard.NewWorkspaceDirectory, MOG_ControllerProject.GetProjectName(), wizard.NewWorkspaceBranch, wizard.NewWorkspacePlatform, userName);
						if (workspace != null)
						{
							// Create the new local branch
							if (this.CreateLocalBranch(workspace))
							{
								// KLK - We now initialize the local explorer tree view when the splitter control is opening
								// mParent.mainForm.LocalBranchMogControl_GameDataDestinationTreeView.InitializeProjectsOnly();
								this.UpdateActiveLocalBranch();

								// Should we perform a mergeVersion?
								if (wizard.NewWorkspaceMergeCheckBox.Checked)
								{
									mParent.BuildMerge(true, false);
								}
							}
						}
					}
				}
			}
		}

		private ArrayList LoadGameDataControllers()
		{
			ArrayList gameDataControllers = new ArrayList();
			ArrayList SyncedLocations = MOG.DATABASE.MOG_DBSyncedDataAPI.GetAllSyncedLocations(MOG_ControllerSystem.GetComputerName(), "", "", MOG_ControllerProject.GetUserName_DefaultAdmin());

			foreach (MOG_DBSyncedLocationInfo syncedLocation in SyncedLocations)
			{
				// Create a new workspace controller for this location
				MOG_ControllerSyncData workspace = WorkspaceManager.AddNewWorkspace(syncedLocation.mTabName, syncedLocation.mWorkingDirectory, syncedLocation.mProjectName, syncedLocation.mBranchName, syncedLocation.mPlatformName, MOG_ControllerProject.GetUserName_DefaultAdmin());
				if (workspace != null)
				{
					gameDataControllers.Add(workspace);
				}
			}

			return gameDataControllers;
		}

		/// <summary>
		/// When logging into a new project, the system will deInitialize the Assetmanager to remove conrtols that should be changed with the new project
		/// </summary>
		public void UnLoadUserLocalBranches()
		{
			// Clear current pages
			mCreatingWorkspaceTabs = true;
			mParent.mainForm.AssetManagerLocalDataTabControl.TabPages.Clear();
			mCreatingWorkspaceTabs = false;
			LocalBranchImageShow = true;

			mParent.mainForm.AssetManagerMergeVersionButton.Enabled = false;
		}

		#endregion

		/// <summary>
		/// Pass in a gameData handle and we will search all local branch tabs for a match.  Once found, we will make that
		/// page the active selected tab
		/// </summary>
		/// <param name="targetGameData"></param>
		/// <returns></returns>
		public void SetActiveLocalBranchTab(MOG_ControllerSyncData targetGameData)
		{
			try
			{
				// Make sure this tab isn't already the active one
				MOG_ControllerSyncData current = (MOG_ControllerSyncData)mParent.mainForm.AssetManagerLocalDataTabControl.SelectedTab.Tag;
				if (!current.IsEqual(targetGameData))
				{																															   
					foreach (TabPage page in mParent.mainForm.AssetManagerLocalDataTabControl.TabPages)
					{
						// Get the handle to this tabs gameData controller
						MOG_ControllerSyncData gameDataHandle = (MOG_ControllerSyncData)page.Tag;
				
						// If this is the same gameData handle
						if (gameDataHandle.IsEqual(targetGameData))
						{
                            // Set it as the current active tab
							mParent.mainForm.AssetManagerLocalDataTabControl.SelectedTab = page;
						}
					}
				}
			}
			catch
			{
			}
		}

		internal TabPage GetActiveLocalTab()
		{
			// Get the current platform list view
			if (mParent.mainForm.AssetManagerLocalDataTabControl.SelectedTab != null && mCreatingWorkspaceTabs == false)
			{
				// Get a handle to the tab page
				return mParent.mainForm.AssetManagerLocalDataTabControl.SelectedTab;				
			}

			return null;
		}

		public ListView GetLocalWorkSpaceListView()
		{
			// Get the current platform list view
			if (mParent.mainForm.AssetManagerLocalDataTabControl.SelectedTab != null && mCreatingWorkspaceTabs == false)
			{
				// Get a handle to the tab page
				TabPage activeLocalBranch = mParent.mainForm.AssetManagerLocalDataTabControl.SelectedTab;

				// get a handle to the listView on that page
				MogControl_LocalWorkspaceTab workspace = (MogControl_LocalWorkspaceTab)LocateControl(activeLocalBranch, "MogControl_LocalWorkspaceTab");
				return workspace.WorkSpaceListView;
			}

			return null;
		}

		/// <summary>
		/// Set all of our internal pointers when local branch tabs are selected
		/// </summary>
		/// <returns></returns>
		public bool UpdateActiveLocalBranch()
		{			
			// Get the current platform list view
			if (mParent.mainForm.AssetManagerLocalDataTabControl.SelectedTab != null && mCreatingWorkspaceTabs == false)
			{
				// Get a handle to the tab page
				TabPage activeLocalBranch = mParent.mainForm.AssetManagerLocalDataTabControl.SelectedTab;

				// get a handle to the listView on that page
				MogControl_LocalWorkspaceTab workspace = (MogControl_LocalWorkspaceTab)LocateControl(activeLocalBranch, "MogControl_LocalWorkspaceTab");
				mPlatformListView = workspace.WorkSpaceListView;

				// Get the handle to this tabs gameData controller
				MOG_ControllerSyncData gameDataHandle = (MOG_ControllerSyncData)activeLocalBranch.Tag;
				
				
				// Set our current tab name
				mParent.mainForm.AssetManagerLocalDataTabControl.Tag = activeLocalBranch.Text;
				
				// Set our projects gameData controller
				MOG_ControllerProject.SetCurrentSyncDataController(gameDataHandle);
				MogUtils_Settings.SaveSetting(gameDataHandle.GetProjectName(), gameDataHandle.GetUserName(), "ActiveWorkspace", gameDataHandle.GetSyncDirectory());

				// Login to the new branch
				if (guiProject.SetLoginBranch(gameDataHandle.GetBranchName()))
				{
					guiProject.UpdateGuiBranch(MOG_ControllerProject.GetBranchName());
				}

				// Set our target edit box
				mParent.mainForm.LocalWorkspaceSyncTargetTextBox.Text = gameDataHandle.GetSyncDirectory();

				// Refresh our window if it is currently empty
				RefreshWindow();

				// Update the local Explorer window to show this selected workspace
				if (!gameDataHandle.IsEqual(mParent.mainForm.LocalBranchMogControl_GameDataDestinationTreeView.MOGCurrentGameDataNode))
				{
					// Highlight our selected branch in the LocalBranch explorer window
					mParent.mainForm.LocalBranchMogControl_GameDataDestinationTreeView.MOGHighLightGameDataTarget(gameDataHandle.GetSyncDirectory());
				}
				

				mParent.mainForm.CustomToolsBox.Initialize( gameDataHandle.GetPlatformName() );
				mParent.mainForm.MOGStatusBarPlatformBarPanel.Text = gameDataHandle.GetPlatformName();
				mParent.mainForm.mAssetManager.UpdateAssetManagerLocalTabText(true);

				// Update our global buttons
				mParent.mainForm.AssetManagerMergeVersionButton.Enabled = true;
				mParent.mainForm.AssetManagerAutoPackageCheckBox.Checked = gameDataHandle.IsAutoPackageEnabled();
				mParent.mainForm.AssetManagerAutoUpdateLocalCheckBox.Checked = gameDataHandle.IsAutoUpdateEnabled();
							
				// Set our context menu point to our ListView
				mAssetMenu.SetView = mPlatformListView;

				return true;			
			}
			else
			{
				mParent.mainForm.CustomToolsBox.Clear( );	// DVC
			}

			return false;
		}

		internal void UpdateActiveLocalBranchAutoPackage()
		{
			// Get the current platform list view
			if (mParent.mainForm.AssetManagerLocalDataTabControl.SelectedTab != null && mCreatingWorkspaceTabs == false)
			{
				// Get a handle to the tab page
				TabPage activeLocalBranch = mParent.mainForm.AssetManagerLocalDataTabControl.SelectedTab;

				// get a handle to the listView on that page
				MogControl_LocalWorkspaceTab workspace =
					(MogControl_LocalWorkspaceTab) LocateControl(activeLocalBranch, "MogControl_LocalWorkspaceTab");

				workspace.WorkspaceAutoPackage = mParent.mainForm.AssetManagerAutoPackageCheckBox.Checked;
			}
		}

		internal void UpdateActiveLocalBranchAutoUpdate()
		{
			// Get the current platform list view
			if (mParent.mainForm.AssetManagerLocalDataTabControl.SelectedTab != null && mCreatingWorkspaceTabs == false)
			{
				// Get a handle to the tab page
				TabPage activeLocalBranch = mParent.mainForm.AssetManagerLocalDataTabControl.SelectedTab;

				// get a handle to the listView on that page
				MogControl_LocalWorkspaceTab workspace =
					(MogControl_LocalWorkspaceTab)LocateControl(activeLocalBranch, "MogControl_LocalWorkspaceTab");

				workspace.AutoUpdate = mParent.mainForm.AssetManagerAutoUpdateLocalCheckBox.Checked;
			}
		}

		/// <summary>
		/// Search all the controls passed in to find one that matches the string name provided
		/// </summary>
		/// <param name="source"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public object LocateControl(Control source, string name)
		{
			foreach (Control target in source.Controls)
			{
				if (string.Compare(target.Name, name, true) == 0)
				{
					return target;
				}
			}

			return null;
		}


		// ****************************************************************************
		#region Context menu click handles
		/// <summary>
		/// Verify each menuItem for applicability to the current selected treeNode
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LocalBranchExploreContextMenu_Popup(object sender, System.EventArgs e)
		{
			try
			{
				ContextMenu myMenu = (ContextMenu)sender;
				// Always disable these
				myMenu.MenuItems[(int)LocalMenuItems.GetLatest].Enabled = false;
				myMenu.MenuItems[(int)LocalMenuItems.RemoveWorkspace].Enabled = false;
				myMenu.MenuItems[(int)LocalMenuItems.Clean].Enabled = false;
				myMenu.MenuItems[(int)LocalMenuItems.Explore].Enabled = false;

				// Check if this is the top tree node?
				if (mParent.mainForm.LocalBranchMogControl_GameDataDestinationTreeView.MOGSelectedNode != null &&
					mParent.mainForm.LocalBranchMogControl_GameDataDestinationTreeView.MOGSelectedNode.Parent == null)
				{
					myMenu.MenuItems[(int)LocalMenuItems.CreateWorkspace].Enabled = true;
				}

				// Check to see if the selected node is a mog target
				string dir = mParent.mainForm.LocalBranchMogControl_GameDataDestinationTreeView.MOGGameDataTarget;
				// Check if this node has an associate location?
				if (dir.Length > 0)
				{
					// Assume we can explore anything we have a valid dir
					myMenu.MenuItems[(int)LocalMenuItems.Explore].Enabled = true;
				}

				// Update the menuItem names
				myMenu.MenuItems[(int)LocalMenuItems.GetLatest].Text = LocalBranch_GetLatest + " <" + dir + ">";

				// Check if we know anything about the current node being part of a workspace?
				if (mParent.mainForm.LocalBranchMogControl_GameDataDestinationTreeView.MOGCurrentGameDataNode != null)
				{
					string computerName = MOG_ControllerSystem.GetComputerName();
					string projectName = mParent.mainForm.LocalBranchMogControl_GameDataDestinationTreeView.MOGCurrentGameDataNode.GetProjectName();
					string platformName = mParent.mainForm.LocalBranchMogControl_GameDataDestinationTreeView.MOGCurrentGameDataNode.GetPlatformName();
					string workspaceDirectory = mParent.mainForm.LocalBranchMogControl_GameDataDestinationTreeView.MOGCurrentGameDataNode.GetSyncDirectory();
					string userName = mParent.mainForm.LocalBranchMogControl_GameDataDestinationTreeView.MOGCurrentGameDataNode.GetUserName();

					// If it is... Disable creation and enable updating and cleaning
					myMenu.MenuItems[(int)LocalMenuItems.CreateWorkspace].Enabled = false;
					myMenu.MenuItems[(int)LocalMenuItems.GetLatest].Enabled = true;
					myMenu.MenuItems[(int)LocalMenuItems.Clean].Enabled = true;

					// Check if this is the root of the project?
					if (dir.Equals(workspaceDirectory, StringComparison.CurrentCultureIgnoreCase))
					{
						// enable the remove workspace option
						myMenu.MenuItems[(int)LocalMenuItems.RemoveWorkspace].Enabled = true;
					}
				}

// JohnRen - Removed because we don't care about limiting a single workspace in an unlicensed version anymore
//				// NOW lets check to see if we should enable the 'New Workspace' menuItem
//				// Get a list of all our projects that the server knows about
//				ArrayList projectsArray = MOG_DBSyncedDataAPI.GetAllSyncedLocations(MOG_ControllerSystem.GetComputerName(), "", "", MOG_ControllerProject.GetUserName_DefaultAdmin());
//
//				// Allow all version types to create at least one workspace
//				if (MOG_Main.IsUnlicensed() &&
//					projectsArray.Count == 0)
//				{
//					// Yup, enable it.  We allow 1 workspace in lite version
//					myMenu.MenuItems[(int)LocalMenuItems.CreateWorkspace].Text = LocalBranch_NewWorkspace;
//					myMenu.MenuItems[(int)LocalMenuItems.CreateWorkspace].Enabled = true;
//				}
//				else if (MOG_Main.IsUnlicensed() &&
//					projectsArray.Count > 0)
//				{
//					// Ok, we have one so now it should be disabled
//					MogUtil_VersionInfo.SetLightVersionControl(myMenu.MenuItems[(int)LocalMenuItems.CreateWorkspace]);
//				}
			}
			catch
			{
			}		
		}

		/// <summary>
		/// GetLatest a selected gameData directory
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LocalBranchExploreSyncMenuItem_Click(object sender, EventArgs e)
		{
			//mParent.BuildMerge(false);
			try
			{
				//if (mSourceFileWatch != null)mSourceFileWatch.EnableRaisingEvents = false;

				string syncNode = mParent.mainForm.LocalBranchMogControl_GameDataDestinationTreeView.MOGGameDataTarget;
				TreeNode targetNode = mParent.mainForm.LocalBranchMogControl_GameDataDestinationTreeView.MOGSelectedNode;

				MOG_ControllerSyncData workspace = mParent.mainForm.LocalBranchMogControl_GameDataDestinationTreeView.MOGCurrentGameDataNode;
				workspace.SyncDataFromSyncLocation(syncNode, "", "", true, null);

				SyncLatestSummaryForm summary = new SyncLatestSummaryForm(workspace.GetSyncLog());
				summary.ShowDialog(mParent.mainForm);

				// Refresh our tree
				string refreshDirectory = syncNode;
				mParent.mainForm.LocalBranchMogControl_GameDataDestinationTreeView.MOGRefreshTree(refreshDirectory, targetNode);
			}
			catch(Exception ex)
			{
				MOG_Prompt.PromptMessage("GetLatest My Workspace", "Could not GetLatest\n\nMessage:\n" + ex.Message, ex.StackTrace);
			}
		}

		/// <summary>
		/// Explore the currently selected folder or asset
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LocalBranchExploreExploreMenuItem_Click(object sender, EventArgs e)
		{
			if (mParent.mainForm.LocalBranchMogControl_GameDataDestinationTreeView.MOGGameDataTarget.Length != 0)
			{
				string target = mParent.mainForm.LocalBranchMogControl_GameDataDestinationTreeView.MOGGameDataTarget;

				// Check if this selected node is a file?
				MOG_ControlsLibrary.Common.MogControl_LocalBranchTreeView.guiAssetTreeTag tag = mParent.mainForm.LocalBranchMogControl_GameDataDestinationTreeView.MOGSelectedNode.Tag as MOG_ControlsLibrary.Common.MogControl_LocalBranchTreeView.guiAssetTreeTag;
				if (tag != null)
				{
					if (tag.TagType == MOG_ControlsLibrary.Common.MogControl_LocalBranchTreeView.guiAssetTreeTag.TREE_FOCUS.FILE)
					{
						// Files need to supply the path
						target = Path.GetDirectoryName(target);
					}
				}
			SpawnDirectory:
				if (Directory.Exists(target))
				{
					guiCommandLine.ShellExecute(target, false);
				}
				else
				{
					if (MOG_Prompt.PromptResponse("This directory does not exist", "Would you like to create this directory?\n\n" + target, MOGPromptButtons.YesNo) == MOGPromptResult.Yes)
					{
						Directory.CreateDirectory(target);
						goto SpawnDirectory;
					}
				}
			}
		}

		/// <summary>
		/// Clean the selected local branch
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LocalBranchExploreCleanMenuItem_Click(object sender, EventArgs e)
		{
			string startingPath = mParent.mainForm.LocalBranchMogControl_GameDataDestinationTreeView.MOGGameDataTarget;
			try
			{
				MogUtil_WorkspaceCleaner cleaner = new MogUtil_WorkspaceCleaner();
				if (cleaner.Clean(startingPath))
				{
					mParent.mainForm.LocalBranchMogControl_GameDataDestinationTreeView.MOGRefresh();
				}
			}
			catch (Exception ex)
			{
				MOG_Prompt.PromptMessage("Clean", "The cleaner exited with the following error (" + ex.Message + ")");
			}
		}

		/// <summary>
		/// Explorer context menu create new local branch
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LocalBranchExploreCreateNewMenuItem_Click(object sender, EventArgs e)
		{
			this.LocalBranchCreateNew(false);
		}

		/// <summary>
		/// Create tab from selected target
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LocalBranchExploreCreateTabMenuItem_Click(object sender, EventArgs e)
		{
			string targetDir = "";
			
			try
			{		
				string userName = MOG_ControllerProject.GetUserName_DefaultAdmin();

				targetDir = mParent.mainForm.LocalBranchMogControl_GameDataDestinationTreeView.MOGGameDataTarget;

				// Create the local tab
				MOG_ControllerSyncData workspace = WorkspaceManager.AddNewWorkspace(targetDir, targetDir, MOG_ControllerProject.GetProjectName(), MOG_ControllerProject.GetBranchName(), "", userName);
				if (workspace != null)
				{
					CreateLocalTab(workspace);
				}
			}
			catch( Exception ex)
			{
				MOG_Prompt.PromptMessage("Create tab to Local Workspace", "\"" + targetDir + "\" could not be created\n\nMessage:\n" + ex.Message, ex.StackTrace);
			}
		}

		/// <summary>
		/// Remove a local branch selected in the GameDataExplorer tree
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LocalBranchExploreRemoveMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				MOG_ControllerSyncData branch = mParent.mainForm.LocalBranchMogControl_GameDataDestinationTreeView.MOGCurrentGameDataNode;
				if (branch != null)
				{
					LocalBranchRemove(branch);
				}
			}
			catch( Exception ex)
			{
				MOG_Prompt.PromptMessage("Remove Local Workspace", "\"" + mParent.mainForm.AssetManagerLocalDataTabControl.SelectedTab.Text + "\" could not be removed\n\nMessage:\n" + ex.Message, ex.StackTrace);
			}
		}

		/// <summary>
		/// Handler for creating a new local branch from the explorer context menu
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LocalBranchExploreCreateLocalBranchMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				MenuItem menuItem = (MenuItem)sender;

				// Find the target
				string target = mParent.mainForm.LocalBranchMogControl_GameDataDestinationTreeView.MOGGameDataTarget;

				// Find the project
				string project = MOG_ControllerProject.GetProjectName();
                
				// Find the branch
				string branch = ((MenuItem)menuItem.Parent).Text;

				// Find the platform
				string platform = menuItem.Text;

				//Find the Active User
				string userName = MOG_ControllerProject.GetUserName_DefaultAdmin();				

				// Create the local branch
				MOG_ControllerSyncData workspace = WorkspaceManager.AddNewWorkspace(target, target, project, branch, platform, userName);
				if (workspace != null)
				{
					CreateLocalBranch(workspace);
				}
				
				// Refresh our local explorer tree
				mParent.mainForm.LocalBranchMogControl_GameDataDestinationTreeView.InitializeProjectsOnly();
			}
			catch
			{
			}
		}

		/// <summary>
		/// Refresh the local explorer tree
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LocalBranchRefreshMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				//Is there a node selected right now?
				if (mParent.mainForm.LocalBranchMogControl_GameDataDestinationTreeView.MOGSelectedNode != null)
				{
					// Yes, so call refresh
					mParent.mainForm.LocalBranchMogControl_GameDataDestinationTreeView.MOGRefresh();
				}
				else
				{
					// No node selected so just refresh our local explorer tree
					mParent.mainForm.LocalBranchMogControl_GameDataDestinationTreeView.InitializeProjectsOnly();
				}
			}
			catch
			{
			}
		}

		/// <summary>
		/// Generate Report on the local explorer tree
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LocalBranchGenerateReportMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				// Kier - TODO - Generate Report???
			}
			catch
			{
			}
		}

		#endregion

		//******************************************************************************
		#region Local branch utility functions
		/// <summary>
		/// Remove a local branch, but not delete it from the target directory
		/// </summary>
		public void LocalBranchRemove(MOG_ControllerSyncData branch)
		{
			try
			{
				if (branch != null)
				{
					// Ask user if we can delete the old directory and all of its files?
					string title = "Remove Local Workspace Directory";
					string message = "When removing this workspace, do you want to delete the old workspace directory and all of its contents?\n" +
									 "NAME: " + branch.GetName() + "\n" +
									 "DIRECTORY: " + branch.GetSyncDirectory();
					MOGPromptResult deleteResult = MOG_Prompt.PromptResponse(title, message, MOGPromptButtons.YesNoCancel);
					if (deleteResult == MOGPromptResult.Cancel)
					{
						// Bail out early
						return;
					}

					// Remove all the updated records
					string filter = branch.GetSyncDirectory() + "\\*";
					if (MOG_DBInboxAPI.InboxRemoveAllAssets("Local", branch.GetUserName(), null, filter))
					{
						// Remove from database
						MOG_DBSyncedDataAPI.RemoveSyncedLocation(MOG_ControllerSystem.GetComputerName(), branch.GetProjectName(), branch.GetPlatformName(), branch.GetSyncDirectory(), branch.GetUserName());

						// Also check if this branch is also the current branch
						if (MOG_ControllerProject.GetCurrentSyncDataController() != null && MOG_ControllerProject.GetCurrentSyncDataController().IsEqual(branch))
						{
							MOG_ControllerProject.SetCurrentSyncDataController(null);
						}

						// Remove all tabs that are associated with this gameData
						foreach (TabPage page in mParent.mainForm.AssetManagerLocalDataTabControl.TabPages)
						{
							// Get the current tab
							MOG_ControllerSyncData tabBranch = (MOG_ControllerSyncData)page.Tag;

							// Is it this same local version?
							if (tabBranch.IsEqual(branch))
							{
								// Also check if this branch is also the current branch
								if (MOG_ControllerProject.GetCurrentSyncDataController() != null && MOG_ControllerProject.GetCurrentSyncDataController().IsEqual(tabBranch))
								{
									MOG_ControllerProject.SetCurrentSyncDataController(null);
								}

								// Remove it from the UserPrefs file
								MogUtils_Settings.Settings.RemoveSection(tabBranch.GetName());
								MogUtils_Settings.Settings.RemovePropertySection(MOG_ControllerProject.GetProjectName() + ".LocalBranches", tabBranch.GetName());
								MogUtils_Settings.Settings.Save();

								// Remove the tab
								mParent.mainForm.AssetManagerLocalDataTabControl.TabPages.Remove(page);

								// Make sure to also remove it from our workspace manager
								WorkspaceManager.RemoveWorkspace(tabBranch);
							}
						}

						// Check if we no longer have any local tabs
						if (mParent.mainForm.AssetManagerLocalDataTabControl.TabPages.Count == 0)
						{
							LocalBranchImageShow = true;
							mParent.mainForm.CustomToolsBox.Visible = false;
							mParent.mainForm.AssetManagerMergeVersionButton.Enabled = false;

							// Clear out MOG Toolbox
							mParent.mainForm.CustomToolsBox.Clear();
						}

						// Refresh our local explorer tree
						mParent.mainForm.LocalBranchMogControl_GameDataDestinationTreeView.InitializeProjectsOnly();

						// Check if we should delete the old workspace directory
						if (deleteResult == MOGPromptResult.Yes)
						{
							// Perform a clean on this directory so the user can decide what should be deleted
							string startingPath = branch.GetSyncDirectory();
							if (startingPath.Length > 0)
							{
								MogUtil_WorkspaceCleaner cleaner = new MogUtil_WorkspaceCleaner();
								if (cleaner.Remove(startingPath))
								{
									// Yes, so call refresh
									mParent.mainForm.LocalBranchMogControl_GameDataDestinationTreeView.MOGRefresh();
								}
							}
						}
					}
					else
					{
						throw new Exception("Database inbox could not remove this synced location!");
					}
				}
			}
			catch( Exception ex)
			{
				MOG_Prompt.PromptMessage("Remove Local Workspace", "\"" + mParent.mainForm.AssetManagerLocalDataTabControl.SelectedTab.Text + "\" could not be removed\n\nMessage:\n" + ex.Message, ex.StackTrace);
			}
		}

		/// <summary>
		/// Opens a windows explorer window in the target directory of this local branch
		/// </summary>
		public void LocalBranchExplore()
		{
			try
			{
				MOG_ControllerSyncData branch = (MOG_ControllerSyncData)mParent.mainForm.AssetManagerLocalDataTabControl.SelectedTab.Tag;				
				guiCommandLine.ShellExecute(branch.GetSyncDirectory(), false);
			}
			catch
			{
				MOG_Prompt.PromptMessage("Explore Project", "\"" + mParent.mainForm.AssetManagerLocalDataTabControl.SelectedTab.Text + "\" Cannot be explored", Environment.StackTrace);
			}
		}

		/// <summary>
		/// Rename the selected tab
		/// </summary>
		public void LocalBranchRename()
		{
			try
			{
				// Get the gameDataController for this tab
				MOG_ControllerSyncData branch = (MOG_ControllerSyncData)mParent.mainForm.AssetManagerLocalDataTabControl.SelectedTab.Tag;

				// Get the old name
				string oldName = branch.GetName();

				// Get the new name for the tab
				string newName = Microsoft.VisualBasic.Interaction.InputBox("Enter new tab name", "Local Workspace tab name", branch.GetName(), -1, -1);

				// Make sure we got a valid string back
				if (newName.Length > 0)
				{
					if (branch.Rename(newName))
					{
						// Rename the settings
						MogUtils_Settings.Settings.RenameSection(mParent.mainForm.AssetManagerLocalDataTabControl.SelectedTab.Text, newName);
						MogUtils_Settings.Settings.Save();

						// Set the tab text to the new name
						mParent.mainForm.AssetManagerLocalDataTabControl.SelectedTab.Text = newName;

						// Update this tab's GameDataController with the new controller with the updated name
						mParent.mainForm.AssetManagerLocalDataTabControl.SelectedTab.Tag = branch;						
					}
					else
					{
						MOG_Prompt.PromptResponse("Rename Local Workspace tab", "We were unable to rename this tab", MOGPromptButtons.OK);
					}
				}				
			}
			catch(Exception ex)
			{
				MOG_Prompt.PromptMessage("Rename Local Workspace tab", "We were unable to rename this tab.\n\nMessage:\n" + ex.Message, ex.StackTrace);
			}
		}

		/// <summary>
		/// Create a new local branch with the use of new branch form window
		/// </summary>
		public void LocalBranchCreateNew(bool checkSelectedNode)
		{
            if (checkSelectedNode) 
			{
				if (mParent.mainForm.LocalBranchMogControl_GameDataDestinationTreeView.MOGSelectedNode != null)
				{
					if (string.Compare(mParent.mainForm.LocalBranchMogControl_GameDataDestinationTreeView.MOGSelectedNode.Text, "<New Workspace>", true) != 0)
					{
						return;
					}
				}
				else
				{
					return;
				}
			}

            // Get a list of all our projects that the server knows about
            ArrayList projectsArray = MOG_DBSyncedDataAPI.GetAllSyncedLocations(MOG_ControllerSystem.GetComputerName(), "", "", MOG_ControllerProject.GetUserName_DefaultAdmin());

            // Allow all version types to create at least one workspace
            if (MOG_Main.IsUnlicensed() &&
				projectsArray.Count > 0)
            {
                return;
            }

			// Make sure we actually have some platforms in this project?
			if (MOG_ControllerProject.GetProject().GetPlatforms().Count > 0)
			{
				CreateNewLocalBranchForm newLocalBranch = new CreateNewLocalBranchForm();

				if (newLocalBranch.ShowDialog() == DialogResult.OK)
				{
					//Get the current user.
					string userName = MOG_ControllerProject.GetUserName_DefaultAdmin();

					// Create the new local branch
					MOG_ControllerSyncData workspace = WorkspaceManager.AddNewWorkspace(newLocalBranch.NewLocalTargetTextBox.Text, newLocalBranch.NewLocalTargetTextBox.Text, MOG_ControllerProject.GetProjectName(), newLocalBranch.NewLocalBranchComboBox.Text, newLocalBranch.NewLocalPlaformComboBox.Text, userName);
					if (workspace != null)
					{
						if (this.CreateLocalBranch(workspace))
						{
							mParent.mainForm.LocalBranchMogControl_GameDataDestinationTreeView.InitializeProjectsOnly();
							this.UpdateActiveLocalBranch();
						}
					}
				}
			}
			else
			{
				// Inform the user they have no platforms in this project
				String message = "Local Workspaces are platform specific so you will not be able to create a Local Workspace until you add a platform to the project.\n\n" + 
								 "Please add a platform to the project using the 'Configure Project Dialog'.";
				MOG_Prompt.PromptMessage("Project has no platforms", message, "", MOG_ALERT_LEVEL.ALERT);
			}
		}

		#endregion 
		#endregion

		private void mSourceFileWatch_Changed(object sender, FileSystemEventArgs e)
		{
		}
	}
}
