using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using AppLoading;
using MOG;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERSYNCDATA;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.DOSUTILS;
using MOG.FILENAME;
using MOG.PROGRESS;
using MOG.PROJECT;
using MOG.PROMPT;
using MOG.REPORT;
using MOG_Client.Client_Gui;
using MOG_Client.Client_Mog_Utilities;
using MOG_Client.Client_Utilities;
using MOG_ControlsLibrary;
using MOG_ControlsLibrary.Common;
using MOG_ControlsLibrary.Common.MogControl_LocalBranchTreeView;
using MOG_ControlsLibrary.Common.MogControl_PictureBoxEx;
using MOG_ControlsLibrary.Controls;
using MOG_ControlsLibrary.Forms;
using MOG_ControlsLibrary.MogForm_CreateTag;
using MOG_ControlsLibrary.MogUtils_Settings;
using MOG_ControlsLibrary.Utilities;
using MOG_CoreControls;
using MOG_CoreControls.MogUtil_DepthManager;

/// NOTE: Changing columnHeader names for Workspace boxes **WILL BREAK VIEW-UPDATE FUNCTIONALITY**
///   As of 2/2/2006 the list of places to fix are:
///		guiAssetManagerAssets.cs(226)

namespace MOG_Client
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class MogMainForm : System.Windows.Forms.Form
	{
		static public MogMainForm MainApp;
		static public ArrayList ChildForms = new ArrayList();
		private AppDomain currentDomain;

		public const string ClassificationTreeView_Text = "Classification View";
		public const string SyncTargetView_Text = "SyncTarget View";
		public const string PackageView_Text = "Package View";
		public const string ArchivalView_Text = "Archive View";

		private bool mShowOldStyleWorkspaceButtons;
		public bool ShowOldStyleWorkspaceButtons
		{
			get
			{
				return mShowOldStyleWorkspaceButtons;
			}
			set
			{
				mShowOldStyleWorkspaceButtons = value;
				AssetManagerPackageButton.Visible = mShowOldStyleWorkspaceButtons;
				AssetManagerAutoPackageCheckBox.Visible = mShowOldStyleWorkspaceButtons;
				AssetManagerAutoPackageCheckBox.BringToFront();
				AssetManagerAutoUpdateLocalCheckBox.Visible = mShowOldStyleWorkspaceButtons;
				AssetManagerMergeVersionButton.Visible = mShowOldStyleWorkspaceButtons;
			}
		}

		#region User Definitions

		// Tab page objects
		public guiStartup mStartup;
		public guiProjectManager mProjectManager;
		public guiAssetManager mAssetManager;
		public guiSlaveManager mSlaveManager;
		public guiLibraryManager mLibraryManager;
		public guiConnectionManager mConnectionManager;
		//		public guiAssetArchive mAssetArchive;
		//		public guiBugManager mBugManager;
		public guiWebTabManager mWebTabManager;
		public guiLockManager mLockManager;
		public MogUtils_SkinManager mSkins;
		public guiSound mSoundManager;

		public string mSoundScheme;
		public Thread mAssetManagerBackground;
		public Thread mMogProcess;
		//private Thread mSelectProjectTreeThread;
		//private object mSelectedProjectTree;

		public bool AllowConnectionsRefresh = true;

		// Utility objects
		public guiEventCallbacks mEventCallback;
		public string mUserPrefsFile;
		public guiUserPrefs mUserPrefs;
		public bool mShowFilters;
		private readonly guiCommandLine mCommandLineParse;
		public string mVersion;
		public bool mAdministratorMode;

		private bool mbRightButtonDrag = false;
		private bool mbCtrlButtonDrag = false;
		private string[] mDragFiles;

		#endregion
		#region Main_Form_Member_Definitions

		private System.Windows.Forms.ColumnHeader columnHeader9;
		private System.Windows.Forms.ColumnHeader columnHeader10;
		private System.Windows.Forms.ColumnHeader columnHeader11;
		private System.Windows.Forms.ColumnHeader columnHeader12;
		public System.Windows.Forms.ContextMenu mnuExplorer;
		public System.Windows.Forms.MenuItem mnuExplorerAssignTo;
		private System.Windows.Forms.MenuItem mnuExplorerImport;
		private System.Windows.Forms.MenuItem mnuExplorerUser;
		private System.Windows.Forms.MenuItem mnuExplorerSeparator;
		private System.Windows.Forms.MenuItem mnuExplorerProperties;
		private System.Windows.Forms.ColumnHeader columnHeader25;
		private System.Windows.Forms.ColumnHeader columnHeader26;
		private System.Windows.Forms.ColumnHeader columnHeader27;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.Splitter AssetManagerSplitter;
		public System.Timers.Timer ProcessTimer;
		private System.Windows.Forms.Panel AssetManagerPanel;
		public System.Windows.Forms.ImageList MOGImagesImageList;
		public System.Windows.Forms.CheckBox AssetManagerAutoProcessCheckBox;
		public System.Windows.Forms.ComboBox AssetManagerActiveUserComboBox;
		public System.Windows.Forms.Panel AssetManagerBottomPanel;
		private System.Windows.Forms.Splitter SlaveManagerSplitter;
		private System.Windows.Forms.Button SlaveManagerDeleteSlaveButton;
		private System.Windows.Forms.Button SlaveManagerAddSlaveButton;
		private System.Windows.Forms.Splitter LockManagerSplitter;
		private System.Windows.Forms.StatusBar MOGStatusBar;
		internal TabControl MOGTabControl;
		private System.Windows.Forms.Panel SlaveManagerToolsPanel;
		private System.Windows.Forms.Panel LockManagerToolsPanel;
		public System.Windows.Forms.MenuItem AssetArchiveSendToMenuItem;
		private System.Windows.Forms.MenuItem AssetArchiveRemoveMenuItem;
		private System.Windows.Forms.MenuItem AssetArchivePropertiesMenuItem;
		public System.Windows.Forms.ImageList MOGGlobalMainMenuImageList;
		private System.Windows.Forms.ImageList AssetArchiveToolbarImageList;
		public System.Windows.Forms.ImageList ProjectManagerExplorerImageList;
		public System.Windows.Forms.ImageList AssetArchiveAssetsImageList;
		public System.Windows.Forms.StatusBarPanel MOGStatusBarConnectionStatusBarPanel;
		public System.Windows.Forms.StatusBarPanel MOGStatusBarInfoStatusBarPanel;
		public System.Windows.Forms.ImageList StatusBarImageList;
		public System.Windows.Forms.StatusBarPanel MOGStatusBarBlankStatusBarPanel;
		private System.Windows.Forms.Panel LocalBranchRightPanel;
		private System.Windows.Forms.MenuItem AssetArchivePruneMenuItem;
		public System.Windows.Forms.MenuItem AssetArchiveRebuildPackageMenuItem;
		private System.Windows.Forms.MenuItem AssetArchiveViewMenuItem;
		private System.Windows.Forms.MenuItem AssetArchiveSeperatorMenuItem;
		public System.Windows.Forms.CheckBox SlaveManagerOnlyMySlaveCheckBox;
		private System.Windows.Forms.MenuItem AssetManagerChatArchiveViewMenuItem;
		public MogControl_ListView ConnectionsListView;
		public System.Windows.Forms.ColumnHeader SlaveManagerMachineColumnHeader;
		public System.Windows.Forms.ColumnHeader SlaveManagerIpColumnHeader;
		public System.Windows.Forms.ColumnHeader SlaveManagerNetIdColumnHeader;
		public System.Windows.Forms.ColumnHeader SlaveManagerTypeColumnHeader;
		public System.Windows.Forms.ColumnHeader SlaveManagerInformationColumnHeader;
		public ListView ConnectionManagerCommandsListView;
		private System.Windows.Forms.ColumnHeader ConnectionsCommandColumnHeader;
		private System.Windows.Forms.ColumnHeader ConnectionsNameColumnHeader;
		private System.Windows.Forms.ColumnHeader ConnectionsPlatformColumnHeader;
		private System.Windows.Forms.ColumnHeader ConnectionsSlaveColumnHeader;
		private System.Windows.Forms.ColumnHeader ConnectionsJobLabelColumnHeader;
		private System.Windows.Forms.ColumnHeader ConnectionsMachineColumnHeader;
		private System.Windows.Forms.ColumnHeader ConnectionsIpColumnHeader;
		private System.Windows.Forms.ColumnHeader ConnectionsIdColumnHeader;
		private System.Windows.Forms.MenuItem AssetArchiveMakeCurrentMenuItem;
		private System.Windows.Forms.MenuItem AssetArchiveSeperator2MenuItem;
		private System.Windows.Forms.MenuItem AssetArchiveUpdateMenuItem;
		private System.Windows.Forms.MenuItem AssetArchiveRenameMenuItem;
		public System.Windows.Forms.FolderBrowserDialog MOGFolderBrowserDialog;
		public System.Windows.Forms.OpenFileDialog MOGOpenFileDialog;
		private System.Windows.Forms.Panel ConnectionManagerBottomPanel;
		public MogControl_ListView ConnectionManagerMergingListView;
		private System.Windows.Forms.ColumnHeader ConnectionManagerMergeNameColumnHeader;
		private System.Windows.Forms.ColumnHeader ConnectionManagerMergeOwnerColumnHeader;
		private System.Windows.Forms.ColumnHeader ConnectionManagerMergeDateColumnHeader;
		public MogControl_ListView ConnectionManagerPostingListView;
		private System.Windows.Forms.ColumnHeader ConnectionManagerPostNameColumnHeader;
		private System.Windows.Forms.ColumnHeader ConnectionManagerPostOwnerColumnHeader;
		private System.Windows.Forms.ColumnHeader ConnectionManagerPostDateColumnHeader;
		public System.Windows.Forms.Splitter ConnectionManagerMachineCommandSplitter;
		private System.Windows.Forms.MenuItem AssetArchiveListMenuItem;
		private System.Windows.Forms.MenuItem AssetArchiveExploreMenuItem;
		private System.Windows.Forms.ImageList AssetManagerImageList;
		public MogControl_ListView LockManagerPendingListView;
		private System.Windows.Forms.ColumnHeader colClassification;
		private System.Windows.Forms.ColumnHeader colTime;
		public MogControl_ListView LockManagerLocksListView;
		private System.Windows.Forms.ColumnHeader TypeColumn;
		private System.Windows.Forms.ColumnHeader AssetFullNameColumn;
		private System.Windows.Forms.ColumnHeader IpColumn;
		private System.Windows.Forms.ColumnHeader MachineColumn;
		private System.Windows.Forms.ColumnHeader NetIdColumn;
		private System.Windows.Forms.ColumnHeader DescriptionColumn;
		private System.Windows.Forms.Splitter LockManagerPendingSplitter;
		private System.Windows.Forms.ColumnHeader TimeColumn;
		public System.Windows.Forms.ContextMenu AssetInboxColumnContextMenu;
		public System.Windows.Forms.StatusBarPanel MOGStatusBarProjectStatusBarPanel;
		public System.Windows.Forms.StatusBarPanel MOGStatusBarBranchStatusBarPanel;
		public System.Windows.Forms.StatusBarPanel MOGStatusBarUserBarPanel;
		private System.Windows.Forms.ColumnHeader UserColumn;
		private System.Windows.Forms.ColumnHeader colLabel;
		private System.Windows.Forms.ColumnHeader colMachine;
		private System.Windows.Forms.ColumnHeader colNetId;
		private System.Windows.Forms.ColumnHeader colDescription;
		private System.Windows.Forms.ColumnHeader colIp;
		private System.Windows.Forms.ColumnHeader colUser;
		public System.Windows.Forms.ImageList AssetManagerStateImageList;
		public System.Windows.Forms.ContextMenuStrip LocalBranchContextMenu;
		private System.Windows.Forms.StatusBarPanel MOGStatusBarConnectionEmptyBarPanel;
		private System.Windows.Forms.ColumnHeader AssetManagerInboxAssetsFullnameColumnHeader;
		private System.Windows.Forms.ColumnHeader AssetManagerInboxAssetsBoxColumnHeader;
		public MogControl_ListView AssetManagerInboxListView;
		public System.Windows.Forms.ImageList MainTabPagesImageList;
		public System.Windows.Forms.TabControl AssetManagerInboxTabControl;
		public System.Windows.Forms.TabPage AssetManagerInboxAssetsTabPage;
		public System.Windows.Forms.TabPage AssetManagerAssetOutboxAssetsTabPage;
		public MogControl_ListView AssetManagerSentListView;
		public System.Windows.Forms.TabPage AssetManagerAssetTrashAssetsTabPage;
		public System.Windows.Forms.Label AssetManagerTrashTotalSizeLabel;
		public MogControl_ListView AssetManagerTrashListView;
		private System.Windows.Forms.ColumnHeader AssetManagerInboxAssetsNameColumnHeader;
		private System.Windows.Forms.ColumnHeader AssetManagerInboxAssetsDateColumnHeader;
		private System.Windows.Forms.ColumnHeader AssetManagerInboxAssetsSizeColumnHeader;
		private System.Windows.Forms.ColumnHeader AssetManagerInboxAssetsOwnerColumnHeader;
		private System.Windows.Forms.ColumnHeader AssetManagerInboxAssetsPartyColumnHeader;
		private System.Windows.Forms.ColumnHeader AssetManagerInboxAssetsStateColumnHeader;
		private System.Windows.Forms.ColumnHeader AssetManagerInboxAssetsOptionsColumnHeader;
		private System.Windows.Forms.ColumnHeader AssetManagerOutboxNameColumnHeader;
		private System.Windows.Forms.ColumnHeader AssetManagerOutboxDateColumnHeader;
		private System.Windows.Forms.ColumnHeader AssetManagerOutboxSizeColumnHeader;
		private System.Windows.Forms.ColumnHeader AssetManagerOutboxCreatorColumnHeader;
		private System.Windows.Forms.ColumnHeader AssetManagerOutboxPartyColumnHeader;
		private System.Windows.Forms.ColumnHeader AssetManagerOutboxStateColumnHeader;
		private System.Windows.Forms.ColumnHeader AssetManagerTrashNameColumnHeader;
		private System.Windows.Forms.ColumnHeader AssetManagerTrashSizeColumnHeader;
		private System.Windows.Forms.ToolStripMenuItem LocalBranchExploreMenuItem;
		public System.Windows.Forms.ImageList MOGTasksImageList;
		private System.Windows.Forms.HelpProvider MOGHelpProvider;
		private System.Windows.Forms.ColumnHeader AssetManagerInboxClassColumnHeader;
		private System.Windows.Forms.ColumnHeader AssetManagerInboxPlatformColumnHeader;
		private System.Windows.Forms.ColumnHeader AssetManagerOutboxClassColumnHeader;
		private System.Windows.Forms.ColumnHeader AssetManagerOutboxPlatformColumnHeader;
		public MogControl_LocalBranchTreeView LocalBranchMogControl_GameDataDestinationTreeView;
		public System.Windows.Forms.Splitter AssetManagerLocalDataExplorerSplitter;
		private System.Windows.Forms.ToolStripMenuItem LocalBranchRenameMenuItem;
		public ToolStripMenuItem LocalBranchAddMenuItem;
		private System.Windows.Forms.ToolStripMenuItem LocalBranchRemoveMenuItem;
		private System.Windows.Forms.ImageList CommandImageList;
		private System.Windows.Forms.ColumnHeader AssetManagerTrashClassificationColumnHeader;
		private System.Windows.Forms.ColumnHeader AssetManagerTrashDateDeletedColumnHeader;
		private System.Windows.Forms.ColumnHeader AssetManagerTrashPlatformColumnHeader;
		private System.Windows.Forms.ColumnHeader AssetManagerTrashAssetStateColumnHeader;
		private System.Windows.Forms.ColumnHeader AssetManagerTrashAssetOwnerColumnHeader;
		private System.Windows.Forms.ColumnHeader AssetManagerTrashAssetPartyColumnHeader;
		private System.Windows.Forms.ColumnHeader AssetManagerTrashAssetOptionsColumnHeader;
		private System.Windows.Forms.ColumnHeader AssetManagerTrashAssetFullnameColumnHeader;
		private System.Windows.Forms.ColumnHeader AssetManagerTrashAssetBoxColumnHeader;
		private System.Windows.Forms.ColumnHeader AssetManagerOutboxOptionsColumnHeader;
		private System.Windows.Forms.ColumnHeader AssetManagerOutboxFullnameColumnHeader;
		private System.Windows.Forms.ColumnHeader AssetManagerOutboxBoxColumnHeader;
		public System.Windows.Forms.CheckBox AssetManagerAutoImportCheckBox;
		private System.Windows.Forms.ImageList WorkSpaceButtonImageList;
		private System.Windows.Forms.ImageList CustomToolsButtonImageList;
		public System.Windows.Forms.TabControl AssetManagerLocalDataTabControl;
		public System.Windows.Forms.Button AssetManagerPackageButton;
		public System.Windows.Forms.CheckBox AssetManagerAutoPackageCheckBox;
		public System.Windows.Forms.CheckBox AssetManagerAutoUpdateLocalCheckBox;
		public System.Windows.Forms.Button AssetManagerMergeVersionButton;
		public System.Windows.Forms.Splitter AssetManagerLocalDataSplitter;
		private System.Windows.Forms.Panel LocalBranchMiddlePanel;
		public System.Windows.Forms.TextBox LocalWorkspaceSyncTargetTextBox;
		private System.Windows.Forms.Panel LocalBranchLeftPanel;
		public MOG_Client.Forms.ToolBox CustomToolsBox;
		public System.Windows.Forms.PictureBox SkinInboxEndPictureBox;
		public MogControl_PictureBoxEx SkinInboxButtonPictureBox;
		public MogControl_PictureBoxEx SkinMyWorkspacePictureBox;
		public MogControl_PictureBoxEx SkinLocalExplorerButtonPictureBox;
		public MogControl_PictureBoxEx SkinToolboxButtonPictureBox;
		public System.Windows.Forms.PictureBox SkinLocalExplorerEndPictureBox;
		public System.Windows.Forms.PictureBox SkinMyWorkspaceEndPictureBox;
		public System.Windows.Forms.PictureBox SkinToolboxEndPictureBox;
		public System.Windows.Forms.Panel AssetManagerGraphicPanel;
		public System.Windows.Forms.TabPage AssetManagerDraftsAssetsTabPage;
		public MogControl_ListView AssetManagerDraftsListView;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader7;
		private System.Windows.Forms.ColumnHeader columnHeader15;
		private System.Windows.Forms.ColumnHeader columnHeader16;
		private System.Windows.Forms.ColumnHeader columnHeader17;
		private System.Windows.Forms.ColumnHeader columnHeader18;
		private System.Windows.Forms.ColumnHeader columnHeader19;
		private System.Windows.Forms.ColumnHeader columnHeader20;
		private System.Windows.Forms.ColumnHeader columnHeader21;
		private System.Windows.Forms.ColumnHeader columnHeader22;
		private System.Windows.Forms.ColumnHeader columnHeader23;
		private System.Windows.Forms.ColumnHeader columnHeader24;
		private System.Windows.Forms.ColumnHeader AssetManagerInboxTargetPathColumnHeader;
		private System.Windows.Forms.ColumnHeader AssetManagerOutboxTargetPathColumnHeader;
		public MogControl_LibraryExplorer LibraryExplorer;
		private System.Windows.Forms.Label ConnectionsLabel;
		private System.Windows.Forms.ColumnHeader AssetManagerTrashTargetPathBoxColumnHeader;
		public MogControl_PictureBoxEx SkinInboxMidPictureBox;
		public MogControl_PictureBoxEx SkinMyWorkspaceMidPictureBox;
		internal Panel ProjectManagerRepositoryTreePanel;
		public MogControl_FullTreeView ProjectManagerClassificationTreeView;
		public MogControl_PackageTreeView ProjectManagerPackageTreeView;
		public MogControl_ArchivalTreeView ProjectManagerArchiveTreeView;
		public MogControl_SyncTargetTreeView ProjectManagerSyncTargetTreeView;
		private System.Windows.Forms.Panel ProjectManagerTasksPanel;
		public System.Windows.Forms.Button AssetManagerLogoutButton;
		public System.Windows.Forms.TabPage MainTabAssetManagerTabPage;
		public System.Windows.Forms.TabPage MainTabLibraryTabPage;
		public System.Windows.Forms.TabPage MainTabProjectManagerTabPage;
		public System.Windows.Forms.TabPage MainTabConnectionManagerTabPage;
		public System.Windows.Forms.TabPage MainTabLockManagerTabPage;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ContextMenu DragDropContextMenu;
		private System.Windows.Forms.MenuItem ImportAssetSingleMenuItem;
		private System.Windows.Forms.MenuItem ImportAssetSeparateMenuItem;
		private TabPage MainTabStartupTabPage;
		public WebBrowser MOGWelcomeWebBrowser;
		private ToolStripSeparator LocalBranchSep1MenuItem;
		private ToolStripSeparator LocalBranchSep2MenuItem;
		internal TabControl ConnectionsBottomTabControl;
		private TabPage ConnectionsPendingCommandsTabPage;
		private TabPage ConnectionsPendingPackageTabPage;
		private TabPage ConnectionsPendingPostTabPage;
		private ColumnHeader ConnectionManagerClassColumnHeader;
		private ColumnHeader ConnectionManagerVersionColumnHeader;
		private ColumnHeader ConnectionManagerPostClassColumnHeader;
		public StatusBarPanel MOGStatusBarPlatformBarPanel;
		public MOG_Client.Client_Mog_Utilities.Tasks.TaskWindow AssetManagerTaskWindow;
		public Splitter AssetManagerTasksSplitter;
		public MOG_Client.Client_Mog_Utilities.Tasks.TaskWindow ProjectManagerTaskWindow;
		public Splitter splitter1;
		public ComboBox AssetManagerActiveUserDepartmentsComboBox;
		private TabPage ConnectionsLateResolversTabPage;
		public MogControl_ListView ConnectionManagerLateResolversListView;
		private ColumnHeader columnHeader28;
		private ColumnHeader columnHeader29;
		private ColumnHeader columnHeader30;
		private ColumnHeader columnHeader31;
		private ColumnHeader columnHeader32;
		private ColumnHeader LabelColumn;
		private ColumnHeader ClassificationColumn;
		private ColumnHeader colAssetFullname;
		private ColumnHeader colType;
		public Button GetLatestAdvancedButton;
		private ColumnHeader columnHeader2;
		private ColumnHeader AssetManagerInboxAssetsGroupColumnHeader;
		private ColumnHeader AssetManagerOutboxGroupColumnHeader;
		private ColumnHeader AssetManagerTrashAssetGroupColumnHeader;
		private NotifyIcon MogNotifyIcon;
		private MenuItem ImportAssetSeparatorMenuItem;
		private MenuItem ImportAssetSeparateLooseMenuItem;
		private MenuItem ImportAssetSingleLooseMenuItem;
		private ToolStripSeparator LocalBranchSep3MenuItem;
		private ToolStripMenuItem LocalBranchTagMenuItem;
		private ToolStrip LockManagerToolStrip;
		public ToolStripButton LockManagerToggleFilterToolStripButton;
		private ToolStripSeparator toolStripSeparator1;
		public ToolStripTextBox LockManagerFilterToolStripTextBox;
		private ToolStripLabel LockManagerLabelToolStripLabel;
		private TabPage tabPage1;
		internal ToolStrip ProjectManagerTreesToolStrip;
		internal ToolStripButton ProjectTreePackageViewToolStripButton;
		internal ToolStripButton ProjectTreeSyncViewToolStripButton;
		internal ToolStripButton ProjectTreeClassificationViewToolStripButton;
		internal ToolStripButton ProjectTreeArchiveViewtoolStripButton;
		internal ToolStripSeparator toolStripSeparator2;
		private ToolStripButton ProjectTreeRefreshToolStripButton;
		internal MenuStrip MainMogMenuStrip;
		private ToolStripMenuItem fileToolStripMenuItem;
		internal ToolStripMenuItem setMOGRepositoryToolStripMenuItem;
		internal ToolStripMenuItem testSQLConnectionToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator3;
		private ToolStripMenuItem quitToolStripMenuItem;
		private ToolStripMenuItem editToolStripMenuItem;
		private ToolStripMenuItem selectAllToolStripMenuItem;
		private ToolStripMenuItem invertSelectionToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator4;
		private ToolStripMenuItem saveUserPrefsToolStripMenuItem;
		internal ToolStripMenuItem viewToolStripMenuItem;
		internal ToolStripMenuItem remoteMachinesToolStripMenuItem;
		private ToolStripMenuItem addToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator7;
		private ToolStripMenuItem refreshToolStripMenuItem;
		internal ToolStripMenuItem rebuildInboxToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator8;
		private ToolStripMenuItem startupToolStripMenuItem;
		private ToolStripMenuItem workspaceToolStripMenuItem;
		private ToolStripMenuItem projectToolStripMenuItem;
		private ToolStripMenuItem libraryToolStripMenuItem;
		private ToolStripMenuItem connectionsToolStripMenuItem;
		private ToolStripMenuItem locksToolStripMenuItem;
		internal ToolStripSeparator toolStripSeparator9;
		internal ToolStripMenuItem myLocalExplorerToolStripMenuItem;
		internal ToolStripMenuItem myToolboxToolStripMenuItem;
		internal ToolStripSeparator toolStripSeparator10;
		internal ToolStripMenuItem showGroupsToolStripMenuItem;
		private ToolStripMenuItem toolsToolStripMenuItem;
		internal ToolStripMenuItem requestBuildToolStripMenuItem;
		internal ToolStripSeparator toolStripSeparator5;
		private ToolStripMenuItem loadUserReportToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator6;
		internal ToolStripMenuItem serverManagerToolStripMenuItem;
		internal ToolStripMenuItem eventViewerToolStripMenuItem;
		internal ToolStripMenuItem configureProjectToolStripMenuItem;
		internal ToolStripMenuItem projectsToolStripMenuItem;
		internal ToolStripMenuItem branchesToolStripMenuItem;
		private ToolStripMenuItem aboutToolStripMenuItem;
		internal ToolStripMenuItem helpFromMOGWikiToolStripMenuItem;
		internal ToolStripSeparator toolStripSeparator11;
		internal ToolStripMenuItem mOGOnTheWebToolStripMenuItem;
		internal ToolStripSeparator toolStripSeparator12;
		internal ToolStripMenuItem issueTrackerToolStripMenuItem;
		internal ToolStripMenuItem discussionForumsToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator13;
		internal ToolStripMenuItem aboutMOGToolStripMenuItem;
		internal ToolStripMenuItem optionsToolStripMenuItem;
		internal ToolStripMenuItem minimizeToSystemTrayToolStripMenuItem;
        internal ToolStripMenuItem alwaysDisplayAdvancedGetLatestToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator14;
		private ToolStripButton ProjectTreesShowDescriptionsToolStripButton;
		private ToolStripButton ProjectTreesShowToolTipsToolStripButton;
		private ToolStripButton ProjectTreeCollapseToolStripButton;
		private ToolStripButton ProjectTreeExpandToolStripButton;
		private ContextMenuStrip MainMogTreeContextMenuStrip;
		private ToolStripMenuItem hideToolStripMenuItem;
		private ToolStripSeparator toolStripMenuItem1;
		internal ToolStripMenuItem TabPageShowToolStripMenuItem;
		internal ToolStripSeparator toolStripMenuItem2;
		private ToolStripMenuItem ViaibleTabsToolStripMenuItem;
		internal ToolStripMenuItem VisibleStartToolStripMenuItem;
		internal ToolStripMenuItem VisibleWorkspaceToolStripMenuItem;
		internal ToolStripMenuItem VisibleProjectToolStripMenuItem;
		internal ToolStripMenuItem VisibleLibraryToolStripMenuItem;
		internal ToolStripMenuItem VisibleConnectionsToolStripMenuItem;
		internal ToolStripMenuItem VisibleLocksToolStripMenuItem;
		internal ToolStripMenuItem showOldStyleWorkspaceButtonsToolStripMenuItem;
		private ToolStripMenuItem soundToolStripMenuItem;
		internal ToolStripMenuItem soundEnableToolStripMenuItem;
		internal ToolStripMenuItem themeToolStripMenuItem;

		private System.Windows.Forms.ToolTip MogToolTip;
		#endregion

		public MogMainForm(string[] args)
		{
			// Parse the command line
			mCommandLineParse = new guiCommandLine(this);
			mCommandLineParse.ParseArgs(args, "");

			// Required for Windows Form Designer support
			InitializeComponent();

			// DVC 
			CustomToolsBox.MainForm = this;
			LocalBranchRightPanel.ContextMenu = CustomToolsBox.MainContext;

			MOG.UITYPESEDITORS.MOGToolTypeEditor.ShowToolsBrowserFuncion = MogForm_MogToolBrowser.ShowBrowser;
		}

		public void InitializeSystem(SplashForm SplashScreen)
		{
			currentDomain = AppDomain.CurrentDomain;
			currentDomain.UnhandledException += currentDomain_UnhandledException;
			// Get our current version number
			mVersion = SplashScreen.tempVersion;

			// Initialize the .Net style menus
			SplashScreen.updateSplash("Initializing menu system...", 10);

			// Initialize MOG
			SplashScreen.updateSplash("Connecting MOG SQL\\Server...", 10);

			try
			{
				bool fullyConnected = false;
				// Keep trying until we have been able to connect
				while (!fullyConnected)
				{
					// Make sure we go online before we attempt a reconnect
					MOG_ControllerSystem.GoOnline();

					// Make sure we have a connection...
					fullyConnected = MOG_Main.Init_Client(mCommandLineParse.configurationIni, "Client");
					if (!fullyConnected)
					{
						// Update the gui
						guiStartup.ConnectionStatus(this, false);

						// Inform the user that we failed to connect to a server
						MOG_Prompt.PromptMessage("Connection Failed", "Failed to establish a connection with the MOG Server.\n" +
																		"     1) The network could be down?\n" +
																		"     2) MOG Server not currently running?\n" +
																		"     3) Firewall preventing port communications?");

						if (MainMenuFileClass.MOGGlobalRepositoryLogin(this, false) == false)
						{
							MOG_Prompt.PromptMessage("Mog Initialization Error", "No active or valid MOG server / SQL server selected!\nExiting...", Environment.StackTrace);
							Shutdown();
							return;
						}
					}
				}

				// Update the gui
				guiStartup.ConnectionStatus(this, true);
			}
			catch (Exception e)
			{
				MOG_Prompt.PromptResponse("Mog Initialization Error", e.Message + "\n\nMOG must now shutdown!", e.StackTrace, MOGPromptButtons.OK, MOG_ALERT_LEVEL.ERROR);
				//MOG.CONTROLLER.CONTROLLERSYSTEM.MOG_ControllerSystem.NotifySystemException("Mog Initialization Error" + e.Message, e.StackTrace);
				Shutdown();
				return;
			}

			// Sound manager
			SplashScreen.updateSplash("Initializing soundManager...", 0);
			mSoundManager = new guiSound(this, SplashScreen, "MOG_Sounds.ini", mSoundScheme);
			
			// Setup our web tab manager
			mWebTabManager = new guiWebTabManager(this);

			SplashScreen.updateSplash("Initializing callbacks...", 0);
			// Initialize the event callbacks
			mEventCallback = new guiEventCallbacks(this);

			SplashScreen.updateSplash("Initializing SQL database...", 0);

			// Initalize SQL Database
			MOG_ControllerSystem.InitializeDatabase("", "", "");

			// Initialize Mog client Startup
			SplashScreen.updateSplash("Initializing startup...", 0);
			mStartup = new guiStartup(this);

#if !MOG_LIBRARY
			// Initialize AssetManager
			SplashScreen.updateSplash("Initializing asset manager...", 0);
			mAssetManager = new guiAssetManager(this);
#endif

			SplashScreen.updateSplash("Initializing core MOG thread...", 0);

			// Load user preferences
			SplashScreen.updateSplash("Loading user prefs...", 0);
			mUserPrefsFile = String.Concat(Environment.CurrentDirectory, "\\MOGUserPrefs.ini");
			mUserPrefs = new guiUserPrefs(mUserPrefsFile, this);
                        
#if !MOG_LIBRARY
			// Load our button style
			showOldStyleWorkspaceButtonsToolStripMenuItem.Checked = MogUtils_Settings.LoadBoolSetting("Workspace", "ShowOldStyleWorkspaceButtons", false);
			ShowOldStyleWorkspaceButtons = showOldStyleWorkspaceButtonsToolStripMenuItem.Checked;
#endif

			SplashScreen.updateSplash("Setting draw style...", 0);

			mAdministratorMode = false;
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.UserPaint, true);

			try
			{
				// Indicate we are down initializing while we wait for the ConnectionNew command from the server
				SplashScreen.updateSplash("Done", 1);

				// Ensure we have received NetworkID from the server?
				MOG_ControllerSystem.RequireNetworkIDInitialization();
			}
			catch
			{
			}

			// If there are no projects the client really cannot run correctly so prompt the user to tell him that we are going to start up the Server Manager
			if (MOG_ControllerSystem.GetSystem().GetProjectNames().Count == 0)
			{
				MOG_Prompt.PromptResponse("No Projects present!", "There are currently no projects.  We will now start the Server Manager to create one.", MOGPromptButtons.OK);
				
				DosUtils.SpawnCommand("MOG_ServerManager.exe", "");
				Shutdown();
			}

			mUserPrefs.LoadStatic_ProjectPrefs();

			// Make sure we are logged into a valid project
			if (!MOG_ControllerProject.IsProject() ||
				!MOG_ControllerProject.IsUser())
			{
				MainMenuProjectsClass.MOGGlobalLaunchProjectLogin("Select valid project", true);
			}

			// Load the tab visibility settings
			foreach (ToolStripMenuItem tabViewableMenu in ViaibleTabsToolStripMenuItem.DropDownItems)
			{
				// In order to simplify the initial user experience, lets not show everything by default
				bool defaultSetting = false;
#if MOG_LIBRARY
				if (tabViewableMenu == VisibleLibraryToolStripMenuItem)
				{
					defaultSetting = true;
				}
#endif
#if !MOG_LIBRARY
				if (tabViewableMenu == VisibleStartToolStripMenuItem ||
					tabViewableMenu == VisibleLibraryToolStripMenuItem ||
					tabViewableMenu == VisibleWorkspaceToolStripMenuItem ||
					tabViewableMenu == VisibleProjectToolStripMenuItem)
				{
					defaultSetting = true;
				}
#endif
				MainMenuViewClass.LoadVisibleTab(this, tabViewableMenu, MogUtils_Settings.LoadBoolSetting("VisibleTabs", tabViewableMenu.Text, defaultSetting));
			}

#if MOG_LIBRARY
			SplashScreen.updateSplash("Initializing MOG Library...", 0);
			InitializeMogLibrary();
#endif
			mUserPrefs.LoadStaticForm_LayoutPrefs();

            mSoundManager.PlayStatusSound("ClientEvents", "Startup");

			// Check active tab
			if (MOG_ControllerProject.GetActiveTabName().Length == 0)
			{
				// Make sure we have an active tab
				if (MOGTabControl.SelectedTab != null)
				{
					MOG_ControllerProject.SetActiveTabName(MOGTabControl.SelectedTab.Text);

					InitializeMainTabPage(MOGTabControl.SelectedTab);
				}
			}

			SplashScreen.updateSplash("Initializing skins...", 0);
			LoadSkins();			
			
			SplashScreen.mInitializationComplete = true;

			MainApp = this;
			MOG_Progress.ClientForm = this;
			MOG_Prompt.ClientForm = this;

			DepthManager.InitParent(this, FormDepth.BACK);

		}

		private void InitializeMogLibrary()
		{
			// Remove all tabs but the library tab
			MainMenuViewClass.SetMogLibraryMode(this);
			MainMenuToolsClass.SetMogLibraryMode(this);
			MainMenuEditClass.SetMogLibraryMode(this);
			MainMenuAboutClass.SetMogLibraryMode(this);
			guiProjectManager.SetMogLibraryMode(this);

			Text = "Mog Library";
		}

		static void currentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Exception ex = e.ExceptionObject as Exception;
			if (ex != null)
			{
				MOG_Report.ReportMessage("Main", ex.Message, ex.StackTrace, MOG_ALERT_LEVEL.CRITICAL);
				// Write our problem out to our Output window in VS.NET
				System.Diagnostics.Debug.WriteLine(e + "\n" + ex.StackTrace);

				// Exit our application so we can set a breakpoint here and try again
				MainApp.Shutdown();
			}
		}


		public string RefreshConnectionToolText()
		{
			// get SQL server and MOG Repository drive mapping info
			string connectionString = MOG_ControllerSystem.GetDB().GetConnectionString();
			string sqlServerName = "NONE";
			if (connectionString.ToLower().IndexOf("data source") != -1)
			{
				// parse out sql server name
				//sqlServerName = connectionString.Substring(connectionString.ToLower().IndexOf("data source=")+13, 
				string[] substrings = connectionString.Split(";".ToCharArray());

				// look for correct part o' the connection string
				foreach (string substring in substrings)
				{
					if (substring.ToLower().StartsWith("data source="))
					{
						sqlServerName = substring.Substring(substring.IndexOf("=") + 1).ToUpper();
					}
				}
			}

			try
			{
				// get MOG repository drive and mapping info
				string mogDrive = MOG_ControllerSystem.GetSystemRepositoryPath();
				char mogDriveLetter = Path.GetPathRoot(mogDrive)[0];
				string mogDriveTarget = new NetworkDriveMapper().GetDriveMapping(mogDriveLetter);

				// Connected to X-SERVER @IP 192.168.5.5; SQL Server: JBIANCHI; MOG Repository M: mapped to \\GX\MOG
				//ConnectionString=Packet size=4096;integrated security=SSPI;data source=NEMESIS;persist security info=False;initial catalog=mog16;

				return string.Concat("Connected to ", MOG_ControllerSystem.GetServerComputerName(), " (IP ", MOG_ControllerSystem.GetServerComputerIP(), ")\nSQL SERVER: ", sqlServerName, "\nMOG Repository ", mogDrive, " mapped to ", mogDriveTarget);
			}
			catch (Exception e)
			{
				MOG_Report.ReportMessage("RefreshConnectionToolText", e.Message, e.StackTrace, MOG_ALERT_LEVEL.CRITICAL);
			}

			return "Connected";
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MogMainForm));
			this.MOGTabControl = new System.Windows.Forms.TabControl();
			this.MainTabStartupTabPage = new System.Windows.Forms.TabPage();
			this.MOGWelcomeWebBrowser = new System.Windows.Forms.WebBrowser();
			this.MainTabAssetManagerTabPage = new System.Windows.Forms.TabPage();
			this.AssetManagerPanel = new System.Windows.Forms.Panel();
			this.AssetManagerInboxTabControl = new System.Windows.Forms.TabControl();
			this.AssetManagerDraftsAssetsTabPage = new System.Windows.Forms.TabPage();
			this.AssetManagerDraftsListView = new MOG_ControlsLibrary.MogControl_ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader7 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader24 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader15 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader16 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader17 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader18 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader19 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader20 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader21 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader22 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader23 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.MOGImagesImageList = new System.Windows.Forms.ImageList(this.components);
			this.AssetManagerInboxAssetsTabPage = new System.Windows.Forms.TabPage();
			this.AssetManagerInboxListView = new MOG_ControlsLibrary.MogControl_ListView();
			this.AssetManagerInboxAssetsNameColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.AssetManagerInboxClassColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.AssetManagerInboxTargetPathColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.AssetManagerInboxAssetsDateColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.AssetManagerInboxAssetsSizeColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.AssetManagerInboxPlatformColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.AssetManagerInboxAssetsStateColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.AssetManagerInboxAssetsOwnerColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.AssetManagerInboxAssetsPartyColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.AssetManagerInboxAssetsOptionsColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.AssetManagerInboxAssetsFullnameColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.AssetManagerInboxAssetsBoxColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.AssetManagerInboxAssetsGroupColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.AssetManagerAssetOutboxAssetsTabPage = new System.Windows.Forms.TabPage();
			this.AssetManagerSentListView = new MOG_ControlsLibrary.MogControl_ListView();
			this.AssetManagerOutboxNameColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.AssetManagerOutboxClassColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.AssetManagerOutboxTargetPathColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.AssetManagerOutboxDateColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.AssetManagerOutboxSizeColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.AssetManagerOutboxPlatformColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.AssetManagerOutboxStateColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.AssetManagerOutboxCreatorColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.AssetManagerOutboxPartyColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.AssetManagerOutboxOptionsColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.AssetManagerOutboxFullnameColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.AssetManagerOutboxBoxColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.AssetManagerOutboxGroupColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.AssetManagerAssetTrashAssetsTabPage = new System.Windows.Forms.TabPage();
			this.AssetManagerTrashTotalSizeLabel = new System.Windows.Forms.Label();
			this.AssetManagerTrashListView = new MOG_ControlsLibrary.MogControl_ListView();
			this.AssetManagerTrashNameColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.AssetManagerTrashClassificationColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.AssetManagerTrashTargetPathBoxColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.AssetManagerTrashDateDeletedColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.AssetManagerTrashSizeColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.AssetManagerTrashPlatformColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.AssetManagerTrashAssetStateColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.AssetManagerTrashAssetOwnerColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.AssetManagerTrashAssetPartyColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.AssetManagerTrashAssetOptionsColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.AssetManagerTrashAssetFullnameColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.AssetManagerTrashAssetBoxColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.AssetManagerTrashAssetGroupColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.AssetManagerTasksSplitter = new System.Windows.Forms.Splitter();
			this.AssetManagerTaskWindow = new MOG_Client.Client_Mog_Utilities.Tasks.TaskWindow();
			this.AssetManagerGraphicPanel = new System.Windows.Forms.Panel();
			this.AssetManagerActiveUserDepartmentsComboBox = new System.Windows.Forms.ComboBox();
			this.AssetManagerLogoutButton = new System.Windows.Forms.Button();
			this.AssetManagerActiveUserComboBox = new System.Windows.Forms.ComboBox();
			this.AssetManagerAutoImportCheckBox = new System.Windows.Forms.CheckBox();
			this.SkinInboxButtonPictureBox = new MOG_ControlsLibrary.Common.MogControl_PictureBoxEx.MogControl_PictureBoxEx();
			this.AssetManagerAutoProcessCheckBox = new System.Windows.Forms.CheckBox();
			this.SkinInboxEndPictureBox = new System.Windows.Forms.PictureBox();
			this.SkinInboxMidPictureBox = new MOG_ControlsLibrary.Common.MogControl_PictureBoxEx.MogControl_PictureBoxEx();
			this.AssetManagerSplitter = new System.Windows.Forms.Splitter();
			this.AssetManagerBottomPanel = new System.Windows.Forms.Panel();
			this.LocalBranchMiddlePanel = new System.Windows.Forms.Panel();
			this.AssetManagerAutoPackageCheckBox = new System.Windows.Forms.CheckBox();
			this.AssetManagerPackageButton = new System.Windows.Forms.Button();
			this.GetLatestAdvancedButton = new System.Windows.Forms.Button();
			this.AssetManagerAutoUpdateLocalCheckBox = new System.Windows.Forms.CheckBox();
			this.AssetManagerMergeVersionButton = new System.Windows.Forms.Button();
			this.AssetManagerLocalDataTabControl = new System.Windows.Forms.TabControl();
			this.LocalBranchContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.LocalBranchExploreMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.LocalBranchSep1MenuItem = new System.Windows.Forms.ToolStripSeparator();
			this.LocalBranchRenameMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.LocalBranchSep2MenuItem = new System.Windows.Forms.ToolStripSeparator();
			this.LocalBranchAddMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.LocalBranchRemoveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.LocalBranchSep3MenuItem = new System.Windows.Forms.ToolStripSeparator();
			this.LocalBranchTagMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.SkinMyWorkspaceEndPictureBox = new System.Windows.Forms.PictureBox();
			this.SkinMyWorkspacePictureBox = new MOG_ControlsLibrary.Common.MogControl_PictureBoxEx.MogControl_PictureBoxEx();
			this.SkinMyWorkspaceMidPictureBox = new MOG_ControlsLibrary.Common.MogControl_PictureBoxEx.MogControl_PictureBoxEx();
			this.LocalWorkspaceSyncTargetTextBox = new System.Windows.Forms.TextBox();
			this.AssetManagerLocalDataExplorerSplitter = new System.Windows.Forms.Splitter();
			this.LocalBranchLeftPanel = new System.Windows.Forms.Panel();
			this.SkinLocalExplorerEndPictureBox = new System.Windows.Forms.PictureBox();
			this.LocalBranchMogControl_GameDataDestinationTreeView = new MOG_ControlsLibrary.Common.MogControl_LocalBranchTreeView.MogControl_LocalBranchTreeView();
			this.SkinLocalExplorerButtonPictureBox = new MOG_ControlsLibrary.Common.MogControl_PictureBoxEx.MogControl_PictureBoxEx();
			this.AssetManagerLocalDataSplitter = new System.Windows.Forms.Splitter();
			this.LocalBranchRightPanel = new System.Windows.Forms.Panel();
			this.SkinToolboxEndPictureBox = new System.Windows.Forms.PictureBox();
			this.CustomToolsBox = new MOG_Client.Forms.ToolBox();
			this.SkinToolboxButtonPictureBox = new MOG_ControlsLibrary.Common.MogControl_PictureBoxEx.MogControl_PictureBoxEx();
			this.MainTabProjectManagerTabPage = new System.Windows.Forms.TabPage();
			this.ProjectManagerTasksPanel = new System.Windows.Forms.Panel();
			this.ProjectManagerRepositoryTreePanel = new System.Windows.Forms.Panel();
			this.ProjectManagerTreesToolStrip = new System.Windows.Forms.ToolStrip();
			this.ProjectTreeClassificationViewToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.ProjectTreePackageViewToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.ProjectTreeSyncViewToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.ProjectTreeArchiveViewtoolStripButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.ProjectTreeRefreshToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.ProjectTreeCollapseToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.ProjectTreeExpandToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.ProjectTreesShowDescriptionsToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
			this.ProjectTreesShowToolTipsToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.ProjectManagerClassificationTreeView = new MOG_ControlsLibrary.Controls.MogControl_FullTreeView();
			this.ProjectManagerPackageTreeView = new MOG_ControlsLibrary.Controls.MogControl_PackageTreeView();
			this.ProjectManagerArchiveTreeView = new MOG_ControlsLibrary.Controls.MogControl_ArchivalTreeView();
			this.ProjectManagerSyncTargetTreeView = new MOG_ControlsLibrary.Controls.MogControl_SyncTargetTreeView();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.ProjectManagerTaskWindow = new MOG_Client.Client_Mog_Utilities.Tasks.TaskWindow();
			this.MainTabLibraryTabPage = new System.Windows.Forms.TabPage();
			this.LibraryExplorer = new MOG_ControlsLibrary.Common.MogControl_LibraryExplorer();
			this.MainTabConnectionManagerTabPage = new System.Windows.Forms.TabPage();
			this.ConnectionsListView = new MOG_ControlsLibrary.MogControl_ListView();
			this.SlaveManagerMachineColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.SlaveManagerIpColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.SlaveManagerNetIdColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.SlaveManagerTypeColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.SlaveManagerInformationColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.ConnectionsLabel = new System.Windows.Forms.Label();
			this.ConnectionManagerMachineCommandSplitter = new System.Windows.Forms.Splitter();
			this.ConnectionManagerBottomPanel = new System.Windows.Forms.Panel();
			this.ConnectionsBottomTabControl = new System.Windows.Forms.TabControl();
			this.ConnectionsPendingCommandsTabPage = new System.Windows.Forms.TabPage();
			this.ConnectionManagerCommandsListView = new System.Windows.Forms.ListView();
			this.ConnectionsCommandColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.ConnectionsNameColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.ConnectionsPlatformColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.ConnectionsSlaveColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.ConnectionsJobLabelColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.ConnectionsMachineColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.ConnectionsIpColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.ConnectionsIdColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.CommandImageList = new System.Windows.Forms.ImageList(this.components);
			this.ConnectionsPendingPackageTabPage = new System.Windows.Forms.TabPage();
			this.ConnectionManagerMergingListView = new MOG_ControlsLibrary.MogControl_ListView();
			this.ConnectionManagerMergeNameColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.ConnectionManagerClassColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.ConnectionManagerVersionColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.ConnectionManagerMergeOwnerColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.ConnectionManagerMergeDateColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.ConnectionsPendingPostTabPage = new System.Windows.Forms.TabPage();
			this.ConnectionManagerPostingListView = new MOG_ControlsLibrary.MogControl_ListView();
			this.ConnectionManagerPostNameColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.ConnectionManagerPostClassColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.ConnectionManagerPostDateColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.ConnectionManagerPostOwnerColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.ConnectionsLateResolversTabPage = new System.Windows.Forms.TabPage();
			this.ConnectionManagerLateResolversListView = new MOG_ControlsLibrary.MogControl_ListView();
			this.columnHeader28 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader29 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader30 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader31 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader32 = new System.Windows.Forms.ColumnHeader();
			this.SlaveManagerSplitter = new System.Windows.Forms.Splitter();
			this.SlaveManagerToolsPanel = new System.Windows.Forms.Panel();
			this.SlaveManagerOnlyMySlaveCheckBox = new System.Windows.Forms.CheckBox();
			this.SlaveManagerDeleteSlaveButton = new System.Windows.Forms.Button();
			this.SlaveManagerAddSlaveButton = new System.Windows.Forms.Button();
			this.MainTabLockManagerTabPage = new System.Windows.Forms.TabPage();
			this.LockManagerLocksListView = new MOG_ControlsLibrary.MogControl_ListView();
			this.LabelColumn = new System.Windows.Forms.ColumnHeader();
			this.ClassificationColumn = new System.Windows.Forms.ColumnHeader();
			this.UserColumn = new System.Windows.Forms.ColumnHeader();
			this.DescriptionColumn = new System.Windows.Forms.ColumnHeader();
			this.MachineColumn = new System.Windows.Forms.ColumnHeader();
			this.IpColumn = new System.Windows.Forms.ColumnHeader();
			this.NetIdColumn = new System.Windows.Forms.ColumnHeader();
			this.TimeColumn = new System.Windows.Forms.ColumnHeader();
			this.AssetFullNameColumn = new System.Windows.Forms.ColumnHeader();
			this.TypeColumn = new System.Windows.Forms.ColumnHeader();
			this.label1 = new System.Windows.Forms.Label();
			this.LockManagerSplitter = new System.Windows.Forms.Splitter();
			this.LockManagerToolsPanel = new System.Windows.Forms.Panel();
			this.LockManagerToolStrip = new System.Windows.Forms.ToolStrip();
			this.LockManagerToggleFilterToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.LockManagerLabelToolStripLabel = new System.Windows.Forms.ToolStripLabel();
			this.LockManagerFilterToolStripTextBox = new System.Windows.Forms.ToolStripTextBox();
			this.LockManagerPendingSplitter = new System.Windows.Forms.Splitter();
			this.label2 = new System.Windows.Forms.Label();
			this.LockManagerPendingListView = new MOG_ControlsLibrary.MogControl_ListView();
			this.colLabel = new System.Windows.Forms.ColumnHeader();
			this.colClassification = new System.Windows.Forms.ColumnHeader();
			this.colUser = new System.Windows.Forms.ColumnHeader();
			this.colDescription = new System.Windows.Forms.ColumnHeader();
			this.colMachine = new System.Windows.Forms.ColumnHeader();
			this.colIp = new System.Windows.Forms.ColumnHeader();
			this.colNetId = new System.Windows.Forms.ColumnHeader();
			this.colTime = new System.Windows.Forms.ColumnHeader();
			this.colAssetFullname = new System.Windows.Forms.ColumnHeader();
			this.colType = new System.Windows.Forms.ColumnHeader();
			this.MainMogTreeContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.hideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.TabPageShowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.MainTabPagesImageList = new System.Windows.Forms.ImageList(this.components);
			this.AssetInboxColumnContextMenu = new System.Windows.Forms.ContextMenu();
			this.ProjectManagerExplorerImageList = new System.Windows.Forms.ImageList(this.components);
			this.AssetManagerImageList = new System.Windows.Forms.ImageList(this.components);
			this.MOGTasksImageList = new System.Windows.Forms.ImageList(this.components);
			this.AssetArchiveToolbarImageList = new System.Windows.Forms.ImageList(this.components);
			this.AssetArchiveAssetsImageList = new System.Windows.Forms.ImageList(this.components);
			this.MOGGlobalMainMenuImageList = new System.Windows.Forms.ImageList(this.components);
			this.AssetArchiveSendToMenuItem = new System.Windows.Forms.MenuItem();
			this.AssetArchiveUpdateMenuItem = new System.Windows.Forms.MenuItem();
			this.AssetArchiveRenameMenuItem = new System.Windows.Forms.MenuItem();
			this.AssetArchiveRebuildPackageMenuItem = new System.Windows.Forms.MenuItem();
			this.AssetArchiveRemoveMenuItem = new System.Windows.Forms.MenuItem();
			this.AssetArchiveMakeCurrentMenuItem = new System.Windows.Forms.MenuItem();
			this.AssetArchiveSeperator2MenuItem = new System.Windows.Forms.MenuItem();
			this.AssetArchivePruneMenuItem = new System.Windows.Forms.MenuItem();
			this.AssetArchiveSeperatorMenuItem = new System.Windows.Forms.MenuItem();
			this.AssetArchivePropertiesMenuItem = new System.Windows.Forms.MenuItem();
			this.AssetArchiveExploreMenuItem = new System.Windows.Forms.MenuItem();
			this.AssetArchiveListMenuItem = new System.Windows.Forms.MenuItem();
			this.AssetArchiveViewMenuItem = new System.Windows.Forms.MenuItem();
			this.AssetManagerChatArchiveViewMenuItem = new System.Windows.Forms.MenuItem();
			this.mnuExplorerAssignTo = new System.Windows.Forms.MenuItem();
			this.mnuExplorerUser = new System.Windows.Forms.MenuItem();
			this.mnuExplorerSeparator = new System.Windows.Forms.MenuItem();
			this.mnuExplorerImport = new System.Windows.Forms.MenuItem();
			this.mnuExplorerProperties = new System.Windows.Forms.MenuItem();
			this.columnHeader9 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader10 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader11 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader12 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader25 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader26 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader27 = new System.Windows.Forms.ColumnHeader();
			this.MOGStatusBar = new System.Windows.Forms.StatusBar();
			this.MOGStatusBarInfoStatusBarPanel = new System.Windows.Forms.StatusBarPanel();
			this.MOGStatusBarBlankStatusBarPanel = new System.Windows.Forms.StatusBarPanel();
			this.MOGStatusBarProjectStatusBarPanel = new System.Windows.Forms.StatusBarPanel();
			this.MOGStatusBarBranchStatusBarPanel = new System.Windows.Forms.StatusBarPanel();
			this.MOGStatusBarPlatformBarPanel = new System.Windows.Forms.StatusBarPanel();
			this.MOGStatusBarUserBarPanel = new System.Windows.Forms.StatusBarPanel();
			this.MOGStatusBarConnectionStatusBarPanel = new System.Windows.Forms.StatusBarPanel();
			this.MOGStatusBarConnectionEmptyBarPanel = new System.Windows.Forms.StatusBarPanel();
			this.ProcessTimer = new System.Timers.Timer();
			this.StatusBarImageList = new System.Windows.Forms.ImageList(this.components);
			this.MOGFolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.MOGOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.MogToolTip = new System.Windows.Forms.ToolTip(this.components);
			this.MOGHelpProvider = new System.Windows.Forms.HelpProvider();
			this.WorkSpaceButtonImageList = new System.Windows.Forms.ImageList(this.components);
			this.CustomToolsButtonImageList = new System.Windows.Forms.ImageList(this.components);
			this.DragDropContextMenu = new System.Windows.Forms.ContextMenu();
			this.ImportAssetSeparateMenuItem = new System.Windows.Forms.MenuItem();
			this.ImportAssetSingleMenuItem = new System.Windows.Forms.MenuItem();
			this.ImportAssetSeparatorMenuItem = new System.Windows.Forms.MenuItem();
			this.ImportAssetSeparateLooseMenuItem = new System.Windows.Forms.MenuItem();
			this.ImportAssetSingleLooseMenuItem = new System.Windows.Forms.MenuItem();
			this.MogNotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
			this.MainMogMenuStrip = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.setMOGRepositoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.testSQLConnectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.invertSelectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.soundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.soundEnableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.themeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.minimizeToSystemTrayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alwaysDisplayAdvancedGetLatestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.showOldStyleWorkspaceButtonsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveUserPrefsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.remoteMachinesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
			this.ViaibleTabsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.VisibleStartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.VisibleWorkspaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.VisibleProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.VisibleLibraryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.VisibleConnectionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.VisibleLocksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
			this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.rebuildInboxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
			this.startupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.workspaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.projectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.libraryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.connectionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.locksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
			this.myLocalExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.myToolboxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
			this.showGroupsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.requestBuildToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.loadUserReportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			this.serverManagerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.eventViewerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.configureProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.projectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.branchesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpFromMOGWikiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.issueTrackerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
			this.mOGOnTheWebToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
			this.discussionForumsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
			this.aboutMOGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.MOGTabControl.SuspendLayout();
			this.MainTabStartupTabPage.SuspendLayout();
			this.MainTabAssetManagerTabPage.SuspendLayout();
			this.AssetManagerPanel.SuspendLayout();
			this.AssetManagerInboxTabControl.SuspendLayout();
			this.AssetManagerDraftsAssetsTabPage.SuspendLayout();
			this.AssetManagerInboxAssetsTabPage.SuspendLayout();
			this.AssetManagerAssetOutboxAssetsTabPage.SuspendLayout();
			this.AssetManagerAssetTrashAssetsTabPage.SuspendLayout();
			this.AssetManagerGraphicPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.SkinInboxButtonPictureBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.SkinInboxEndPictureBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.SkinInboxMidPictureBox)).BeginInit();
			this.AssetManagerBottomPanel.SuspendLayout();
			this.LocalBranchMiddlePanel.SuspendLayout();
			this.AssetManagerLocalDataTabControl.SuspendLayout();
			this.LocalBranchContextMenu.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.SkinMyWorkspaceEndPictureBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.SkinMyWorkspacePictureBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.SkinMyWorkspaceMidPictureBox)).BeginInit();
			this.LocalBranchLeftPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.SkinLocalExplorerEndPictureBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.SkinLocalExplorerButtonPictureBox)).BeginInit();
			this.LocalBranchRightPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.SkinToolboxEndPictureBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.SkinToolboxButtonPictureBox)).BeginInit();
			this.MainTabProjectManagerTabPage.SuspendLayout();
			this.ProjectManagerTasksPanel.SuspendLayout();
			this.ProjectManagerRepositoryTreePanel.SuspendLayout();
			this.ProjectManagerTreesToolStrip.SuspendLayout();
			this.MainTabLibraryTabPage.SuspendLayout();
			this.MainTabConnectionManagerTabPage.SuspendLayout();
			this.ConnectionManagerBottomPanel.SuspendLayout();
			this.ConnectionsBottomTabControl.SuspendLayout();
			this.ConnectionsPendingCommandsTabPage.SuspendLayout();
			this.ConnectionsPendingPackageTabPage.SuspendLayout();
			this.ConnectionsPendingPostTabPage.SuspendLayout();
			this.ConnectionsLateResolversTabPage.SuspendLayout();
			this.SlaveManagerToolsPanel.SuspendLayout();
			this.MainTabLockManagerTabPage.SuspendLayout();
			this.LockManagerToolStrip.SuspendLayout();
			this.MainMogTreeContextMenuStrip.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.MOGStatusBarInfoStatusBarPanel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.MOGStatusBarBlankStatusBarPanel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.MOGStatusBarProjectStatusBarPanel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.MOGStatusBarBranchStatusBarPanel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.MOGStatusBarPlatformBarPanel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.MOGStatusBarUserBarPanel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.MOGStatusBarConnectionStatusBarPanel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.MOGStatusBarConnectionEmptyBarPanel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ProcessTimer)).BeginInit();
			this.MainMogMenuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// MOGTabControl
			// 
			this.MOGTabControl.AllowDrop = true;
			this.MOGTabControl.Controls.Add(this.MainTabStartupTabPage);
			this.MOGTabControl.Controls.Add(this.MainTabLibraryTabPage);
			this.MOGTabControl.Controls.Add(this.MainTabAssetManagerTabPage);
			this.MOGTabControl.Controls.Add(this.MainTabProjectManagerTabPage);
			this.MOGTabControl.Controls.Add(this.MainTabConnectionManagerTabPage);
			this.MOGTabControl.Controls.Add(this.MainTabLockManagerTabPage);
			this.MOGTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.MOGTabControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MOGTabControl.HotTrack = true;
			this.MOGTabControl.Location = new System.Drawing.Point(0, 24);
			this.MOGTabControl.Name = "MOGTabControl";
			this.MOGTabControl.SelectedIndex = 0;
			this.MOGTabControl.Size = new System.Drawing.Size(970, 518);
			this.MOGTabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
			this.MOGTabControl.TabIndex = 2;
			this.MOGTabControl.DragOver += new System.Windows.Forms.DragEventHandler(this.MOGTabControl_DragOver);
			this.MOGTabControl.SelectedIndexChanged += new System.EventHandler(this.MOG_TabControl_SelectedIndexChanged);
			// 
			// MainTabStartupTabPage
			// 
			this.MainTabStartupTabPage.BackColor = System.Drawing.Color.Transparent;
			this.MainTabStartupTabPage.Controls.Add(this.MOGWelcomeWebBrowser);
			this.MainTabStartupTabPage.Location = new System.Drawing.Point(4, 22);
			this.MainTabStartupTabPage.Name = "MainTabStartupTabPage";
			this.MainTabStartupTabPage.Padding = new System.Windows.Forms.Padding(3);
			this.MainTabStartupTabPage.Size = new System.Drawing.Size(962, 492);
			this.MainTabStartupTabPage.TabIndex = 5;
			this.MainTabStartupTabPage.Tag = "0";
			this.MainTabStartupTabPage.Text = "Start";
			this.MainTabStartupTabPage.UseVisualStyleBackColor = true;
			// 
			// MOGWelcomeWebBrowser
			// 
			this.MOGWelcomeWebBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
			this.MOGWelcomeWebBrowser.Location = new System.Drawing.Point(3, 3);
			this.MOGWelcomeWebBrowser.MinimumSize = new System.Drawing.Size(20, 20);
			this.MOGWelcomeWebBrowser.Name = "MOGWelcomeWebBrowser";
			this.MOGWelcomeWebBrowser.Size = new System.Drawing.Size(956, 486);
			this.MOGWelcomeWebBrowser.TabIndex = 0;
			// 
			// MainTabAssetManagerTabPage
			// 
			this.MainTabAssetManagerTabPage.BackColor = System.Drawing.Color.Transparent;
			this.MainTabAssetManagerTabPage.Controls.Add(this.AssetManagerPanel);
			this.MainTabAssetManagerTabPage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MainTabAssetManagerTabPage.ImageIndex = 0;
			this.MainTabAssetManagerTabPage.Location = new System.Drawing.Point(4, 22);
			this.MainTabAssetManagerTabPage.Name = "MainTabAssetManagerTabPage";
			this.MainTabAssetManagerTabPage.Size = new System.Drawing.Size(962, 492);
			this.MainTabAssetManagerTabPage.TabIndex = 1;
			this.MainTabAssetManagerTabPage.Tag = "2";
			this.MainTabAssetManagerTabPage.Text = "Workspace";
			this.MainTabAssetManagerTabPage.ToolTipText = "Select this tab to view your inbox of working assets";
			this.MainTabAssetManagerTabPage.UseVisualStyleBackColor = true;
			// 
			// AssetManagerPanel
			// 
			this.AssetManagerPanel.AutoScroll = true;
			this.AssetManagerPanel.AutoScrollMinSize = new System.Drawing.Size(400, 0);
			this.AssetManagerPanel.BackColor = System.Drawing.SystemColors.ControlDark;
			this.AssetManagerPanel.Controls.Add(this.AssetManagerInboxTabControl);
			this.AssetManagerPanel.Controls.Add(this.AssetManagerTasksSplitter);
			this.AssetManagerPanel.Controls.Add(this.AssetManagerTaskWindow);
			this.AssetManagerPanel.Controls.Add(this.AssetManagerGraphicPanel);
			this.AssetManagerPanel.Controls.Add(this.AssetManagerSplitter);
			this.AssetManagerPanel.Controls.Add(this.AssetManagerBottomPanel);
			this.AssetManagerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.AssetManagerPanel.Location = new System.Drawing.Point(0, 0);
			this.AssetManagerPanel.Name = "AssetManagerPanel";
			this.AssetManagerPanel.Size = new System.Drawing.Size(962, 492);
			this.AssetManagerPanel.TabIndex = 2;
			// 
			// AssetManagerInboxTabControl
			// 
			this.AssetManagerInboxTabControl.AllowDrop = true;
			this.AssetManagerInboxTabControl.Controls.Add(this.AssetManagerDraftsAssetsTabPage);
			this.AssetManagerInboxTabControl.Controls.Add(this.AssetManagerInboxAssetsTabPage);
			this.AssetManagerInboxTabControl.Controls.Add(this.AssetManagerAssetOutboxAssetsTabPage);
			this.AssetManagerInboxTabControl.Controls.Add(this.AssetManagerAssetTrashAssetsTabPage);
			this.AssetManagerInboxTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.AssetManagerInboxTabControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.AssetManagerInboxTabControl.HotTrack = true;
			this.AssetManagerInboxTabControl.Location = new System.Drawing.Point(160, 44);
			this.AssetManagerInboxTabControl.Multiline = true;
			this.AssetManagerInboxTabControl.Name = "AssetManagerInboxTabControl";
			this.AssetManagerInboxTabControl.Padding = new System.Drawing.Point(20, 5);
			this.AssetManagerInboxTabControl.SelectedIndex = 0;
			this.AssetManagerInboxTabControl.ShowToolTips = true;
			this.AssetManagerInboxTabControl.Size = new System.Drawing.Size(802, 158);
			this.AssetManagerInboxTabControl.TabIndex = 7;
			this.AssetManagerInboxTabControl.DragDrop += new System.Windows.Forms.DragEventHandler(this.AssetManagerInboxTabControl_DragDrop);
			this.AssetManagerInboxTabControl.DragEnter += new System.Windows.Forms.DragEventHandler(this.AssetManagerInboxTabControl_DragEnter);
			this.AssetManagerInboxTabControl.SelectedIndexChanged += new System.EventHandler(this.AssetManagerBoxesTabControl_SelectedIndexChanged);
			// 
			// AssetManagerDraftsAssetsTabPage
			// 
			this.AssetManagerDraftsAssetsTabPage.Controls.Add(this.AssetManagerDraftsListView);
			this.AssetManagerDraftsAssetsTabPage.Location = new System.Drawing.Point(4, 26);
			this.AssetManagerDraftsAssetsTabPage.Name = "AssetManagerDraftsAssetsTabPage";
			this.AssetManagerDraftsAssetsTabPage.Size = new System.Drawing.Size(794, 128);
			this.AssetManagerDraftsAssetsTabPage.TabIndex = 3;
			this.AssetManagerDraftsAssetsTabPage.Text = "Drafts";
			// 
			// AssetManagerDraftsListView
			// 
			this.AssetManagerDraftsListView.AllowColumnReorder = true;
			this.AssetManagerDraftsListView.AllowDrop = true;
			this.AssetManagerDraftsListView.AutoArrange = false;
			this.AssetManagerDraftsListView.BackColor = System.Drawing.SystemColors.Window;
			this.AssetManagerDraftsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader7,
            this.columnHeader24,
            this.columnHeader15,
            this.columnHeader16,
            this.columnHeader17,
            this.columnHeader18,
            this.columnHeader19,
            this.columnHeader20,
            this.columnHeader21,
            this.columnHeader22,
            this.columnHeader23,
            this.columnHeader2});
			this.AssetManagerDraftsListView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.AssetManagerDraftsListView.FullRowSelect = true;
			this.MOGHelpProvider.SetHelpKeyword(this.AssetManagerDraftsListView, "Workspace");
			this.MOGHelpProvider.SetHelpString(this.AssetManagerDraftsListView, "Workspace");
			this.AssetManagerDraftsListView.LabelWrap = false;
			this.AssetManagerDraftsListView.Location = new System.Drawing.Point(0, 0);
			this.AssetManagerDraftsListView.Name = "AssetManagerDraftsListView";
			this.AssetManagerDraftsListView.ShowGroups = false;
			this.MOGHelpProvider.SetShowHelp(this.AssetManagerDraftsListView, true);
			this.AssetManagerDraftsListView.Size = new System.Drawing.Size(794, 128);
			this.AssetManagerDraftsListView.SmallImageList = this.MOGImagesImageList;
			this.AssetManagerDraftsListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.AssetManagerDraftsListView.TabIndex = 1;
			this.AssetManagerDraftsListView.UseCompatibleStateImageBehavior = false;
			this.AssetManagerDraftsListView.View = System.Windows.Forms.View.Details;
			this.AssetManagerDraftsListView.DoubleClick += new System.EventHandler(this.AssetManagerInboxAssetListView_DoubleClick);
			this.AssetManagerDraftsListView.DragDrop += new System.Windows.Forms.DragEventHandler(this.AssetManagerInboxAssetListView_DragDrop);
			this.AssetManagerDraftsListView.DragEnter += new System.Windows.Forms.DragEventHandler(this.AssetManagerInboxAssetListView_DragEnter);
			this.AssetManagerDraftsListView.DragLeave += new System.EventHandler(this.AssetManagerDraftsListView_DragLeave);
			this.AssetManagerDraftsListView.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.AssetManagerInboxAssetListView_HelpRequested);
			this.AssetManagerDraftsListView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.AssetManagerDraftsListView_ItemDrag);
			this.AssetManagerDraftsListView.DragOver += new System.Windows.Forms.DragEventHandler(this.AssetManagerInboxAssetListView_DragOver);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Name";
			this.columnHeader1.Width = 150;
			// 
			// columnHeader7
			// 
			this.columnHeader7.Text = "MOG Classification";
			this.columnHeader7.Width = 180;
			// 
			// columnHeader24
			// 
			this.columnHeader24.Text = "TargetPath";
			this.columnHeader24.Width = 95;
			// 
			// columnHeader15
			// 
			this.columnHeader15.Text = "Date";
			this.columnHeader15.Width = 160;
			// 
			// columnHeader16
			// 
			this.columnHeader16.Text = "Size";
			this.columnHeader16.Width = 100;
			// 
			// columnHeader17
			// 
			this.columnHeader17.Text = "Platform";
			this.columnHeader17.Width = 0;
			// 
			// columnHeader18
			// 
			this.columnHeader18.Text = "State";
			this.columnHeader18.Width = 0;
			// 
			// columnHeader19
			// 
			this.columnHeader19.Text = "Creator";
			this.columnHeader19.Width = 0;
			// 
			// columnHeader20
			// 
			this.columnHeader20.Text = "Owner";
			this.columnHeader20.Width = 0;
			// 
			// columnHeader21
			// 
			this.columnHeader21.Text = "Options";
			this.columnHeader21.Width = 0;
			// 
			// columnHeader22
			// 
			this.columnHeader22.Text = "Fullname";
			this.columnHeader22.Width = 0;
			// 
			// columnHeader23
			// 
			this.columnHeader23.Text = "Box";
			this.columnHeader23.Width = 0;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Group";
			this.columnHeader2.Width = 0;
			// 
			// MOGImagesImageList
			// 
			this.MOGImagesImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("MOGImagesImageList.ImageStream")));
			this.MOGImagesImageList.TransparentColor = System.Drawing.Color.Transparent;
			this.MOGImagesImageList.Images.SetKeyName(0, "");
			this.MOGImagesImageList.Images.SetKeyName(1, "");
			this.MOGImagesImageList.Images.SetKeyName(2, "");
			this.MOGImagesImageList.Images.SetKeyName(3, "");
			this.MOGImagesImageList.Images.SetKeyName(4, "");
			// 
			// AssetManagerInboxAssetsTabPage
			// 
			this.AssetManagerInboxAssetsTabPage.BackColor = System.Drawing.SystemColors.Control;
			this.AssetManagerInboxAssetsTabPage.Controls.Add(this.AssetManagerInboxListView);
			this.AssetManagerInboxAssetsTabPage.ImageIndex = 0;
			this.AssetManagerInboxAssetsTabPage.Location = new System.Drawing.Point(4, 26);
			this.AssetManagerInboxAssetsTabPage.Name = "AssetManagerInboxAssetsTabPage";
			this.AssetManagerInboxAssetsTabPage.Size = new System.Drawing.Size(794, 128);
			this.AssetManagerInboxAssetsTabPage.TabIndex = 0;
			this.AssetManagerInboxAssetsTabPage.Text = "Inbox";
			this.AssetManagerInboxAssetsTabPage.ToolTipText = "Click here fo all active working files in the selected users inbox";
			// 
			// AssetManagerInboxListView
			// 
			this.AssetManagerInboxListView.AllowColumnReorder = true;
			this.AssetManagerInboxListView.AutoArrange = false;
			this.AssetManagerInboxListView.BackColor = System.Drawing.SystemColors.Window;
			this.AssetManagerInboxListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.AssetManagerInboxAssetsNameColumnHeader,
            this.AssetManagerInboxClassColumnHeader,
            this.AssetManagerInboxTargetPathColumnHeader,
            this.AssetManagerInboxAssetsDateColumnHeader,
            this.AssetManagerInboxAssetsSizeColumnHeader,
            this.AssetManagerInboxPlatformColumnHeader,
            this.AssetManagerInboxAssetsStateColumnHeader,
            this.AssetManagerInboxAssetsOwnerColumnHeader,
            this.AssetManagerInboxAssetsPartyColumnHeader,
            this.AssetManagerInboxAssetsOptionsColumnHeader,
            this.AssetManagerInboxAssetsFullnameColumnHeader,
            this.AssetManagerInboxAssetsBoxColumnHeader,
            this.AssetManagerInboxAssetsGroupColumnHeader});
			this.AssetManagerInboxListView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.AssetManagerInboxListView.FullRowSelect = true;
			this.MOGHelpProvider.SetHelpKeyword(this.AssetManagerInboxListView, "Workspace");
			this.MOGHelpProvider.SetHelpString(this.AssetManagerInboxListView, "Workspace");
			this.AssetManagerInboxListView.LabelWrap = false;
			this.AssetManagerInboxListView.Location = new System.Drawing.Point(0, 0);
			this.AssetManagerInboxListView.Name = "AssetManagerInboxListView";
			this.AssetManagerInboxListView.ShowGroups = false;
			this.MOGHelpProvider.SetShowHelp(this.AssetManagerInboxListView, true);
			this.AssetManagerInboxListView.Size = new System.Drawing.Size(794, 128);
			this.AssetManagerInboxListView.SmallImageList = this.MOGImagesImageList;
			this.AssetManagerInboxListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.AssetManagerInboxListView.TabIndex = 0;
			this.AssetManagerInboxListView.UseCompatibleStateImageBehavior = false;
			this.AssetManagerInboxListView.View = System.Windows.Forms.View.Details;
			this.AssetManagerInboxListView.DoubleClick += new System.EventHandler(this.AssetManagerInboxAssetListView_DoubleClick);
			this.AssetManagerInboxListView.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.AssetManagerInboxAssetListView_HelpRequested);
			this.AssetManagerInboxListView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.AssetManagerInboxAssetListView_ItemDrag);
			this.AssetManagerInboxListView.Click += new System.EventHandler(this.AssetManagerInboxAssetListView_Click);
			// 
			// AssetManagerInboxAssetsNameColumnHeader
			// 
			this.AssetManagerInboxAssetsNameColumnHeader.Text = "Name";
			this.AssetManagerInboxAssetsNameColumnHeader.Width = 150;
			// 
			// AssetManagerInboxClassColumnHeader
			// 
			this.AssetManagerInboxClassColumnHeader.Text = "MOG Classification";
			this.AssetManagerInboxClassColumnHeader.Width = 180;
			// 
			// AssetManagerInboxTargetPathColumnHeader
			// 
			this.AssetManagerInboxTargetPathColumnHeader.Text = "TargetPath";
			this.AssetManagerInboxTargetPathColumnHeader.Width = 95;
			// 
			// AssetManagerInboxAssetsDateColumnHeader
			// 
			this.AssetManagerInboxAssetsDateColumnHeader.Text = "Date";
			this.AssetManagerInboxAssetsDateColumnHeader.Width = 160;
			// 
			// AssetManagerInboxAssetsSizeColumnHeader
			// 
			this.AssetManagerInboxAssetsSizeColumnHeader.Text = "Size";
			this.AssetManagerInboxAssetsSizeColumnHeader.Width = 100;
			// 
			// AssetManagerInboxPlatformColumnHeader
			// 
			this.AssetManagerInboxPlatformColumnHeader.Text = "Platform";
			this.AssetManagerInboxPlatformColumnHeader.Width = 0;
			// 
			// AssetManagerInboxAssetsStateColumnHeader
			// 
			this.AssetManagerInboxAssetsStateColumnHeader.Text = "State";
			this.AssetManagerInboxAssetsStateColumnHeader.Width = 0;
			// 
			// AssetManagerInboxAssetsOwnerColumnHeader
			// 
			this.AssetManagerInboxAssetsOwnerColumnHeader.Text = "Creator";
			this.AssetManagerInboxAssetsOwnerColumnHeader.Width = 0;
			// 
			// AssetManagerInboxAssetsPartyColumnHeader
			// 
			this.AssetManagerInboxAssetsPartyColumnHeader.Text = "Owner";
			this.AssetManagerInboxAssetsPartyColumnHeader.Width = 0;
			// 
			// AssetManagerInboxAssetsOptionsColumnHeader
			// 
			this.AssetManagerInboxAssetsOptionsColumnHeader.Text = "Options";
			this.AssetManagerInboxAssetsOptionsColumnHeader.Width = 0;
			// 
			// AssetManagerInboxAssetsFullnameColumnHeader
			// 
			this.AssetManagerInboxAssetsFullnameColumnHeader.Text = "Fullname";
			this.AssetManagerInboxAssetsFullnameColumnHeader.Width = 0;
			// 
			// AssetManagerInboxAssetsBoxColumnHeader
			// 
			this.AssetManagerInboxAssetsBoxColumnHeader.Text = "Box";
			this.AssetManagerInboxAssetsBoxColumnHeader.Width = 0;
			// 
			// AssetManagerInboxAssetsGroupColumnHeader
			// 
			this.AssetManagerInboxAssetsGroupColumnHeader.Text = "Group";
			this.AssetManagerInboxAssetsGroupColumnHeader.Width = 0;
			// 
			// AssetManagerAssetOutboxAssetsTabPage
			// 
			this.AssetManagerAssetOutboxAssetsTabPage.BackColor = System.Drawing.SystemColors.Control;
			this.AssetManagerAssetOutboxAssetsTabPage.Controls.Add(this.AssetManagerSentListView);
			this.AssetManagerAssetOutboxAssetsTabPage.ImageIndex = 1;
			this.AssetManagerAssetOutboxAssetsTabPage.Location = new System.Drawing.Point(4, 26);
			this.AssetManagerAssetOutboxAssetsTabPage.Name = "AssetManagerAssetOutboxAssetsTabPage";
			this.AssetManagerAssetOutboxAssetsTabPage.Size = new System.Drawing.Size(794, 128);
			this.AssetManagerAssetOutboxAssetsTabPage.TabIndex = 1;
			this.AssetManagerAssetOutboxAssetsTabPage.Text = "Sent";
			this.AssetManagerAssetOutboxAssetsTabPage.ToolTipText = "Check here for the status of outgoing assets such as a bless or sendTo";
			this.AssetManagerAssetOutboxAssetsTabPage.Visible = false;
			// 
			// AssetManagerSentListView
			// 
			this.AssetManagerSentListView.BackColor = System.Drawing.SystemColors.Window;
			this.AssetManagerSentListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.AssetManagerOutboxNameColumnHeader,
            this.AssetManagerOutboxClassColumnHeader,
            this.AssetManagerOutboxTargetPathColumnHeader,
            this.AssetManagerOutboxDateColumnHeader,
            this.AssetManagerOutboxSizeColumnHeader,
            this.AssetManagerOutboxPlatformColumnHeader,
            this.AssetManagerOutboxStateColumnHeader,
            this.AssetManagerOutboxCreatorColumnHeader,
            this.AssetManagerOutboxPartyColumnHeader,
            this.AssetManagerOutboxOptionsColumnHeader,
            this.AssetManagerOutboxFullnameColumnHeader,
            this.AssetManagerOutboxBoxColumnHeader,
            this.AssetManagerOutboxGroupColumnHeader});
			this.AssetManagerSentListView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.AssetManagerSentListView.FullRowSelect = true;
			this.AssetManagerSentListView.Location = new System.Drawing.Point(0, 0);
			this.AssetManagerSentListView.Name = "AssetManagerSentListView";
			this.AssetManagerSentListView.ShowGroups = false;
			this.AssetManagerSentListView.Size = new System.Drawing.Size(794, 128);
			this.AssetManagerSentListView.SmallImageList = this.MOGImagesImageList;
			this.AssetManagerSentListView.TabIndex = 3;
			this.AssetManagerSentListView.UseCompatibleStateImageBehavior = false;
			this.AssetManagerSentListView.View = System.Windows.Forms.View.Details;
			// 
			// AssetManagerOutboxNameColumnHeader
			// 
			this.AssetManagerOutboxNameColumnHeader.Text = "Name";
			this.AssetManagerOutboxNameColumnHeader.Width = 150;
			// 
			// AssetManagerOutboxClassColumnHeader
			// 
			this.AssetManagerOutboxClassColumnHeader.Text = "MOG Classification";
			this.AssetManagerOutboxClassColumnHeader.Width = 180;
			// 
			// AssetManagerOutboxTargetPathColumnHeader
			// 
			this.AssetManagerOutboxTargetPathColumnHeader.Text = "TargetPath";
			this.AssetManagerOutboxTargetPathColumnHeader.Width = 95;
			// 
			// AssetManagerOutboxDateColumnHeader
			// 
			this.AssetManagerOutboxDateColumnHeader.Text = "Date";
			this.AssetManagerOutboxDateColumnHeader.Width = 160;
			// 
			// AssetManagerOutboxSizeColumnHeader
			// 
			this.AssetManagerOutboxSizeColumnHeader.Text = "Size";
			this.AssetManagerOutboxSizeColumnHeader.Width = 100;
			// 
			// AssetManagerOutboxPlatformColumnHeader
			// 
			this.AssetManagerOutboxPlatformColumnHeader.Text = "Platform";
			this.AssetManagerOutboxPlatformColumnHeader.Width = 0;
			// 
			// AssetManagerOutboxStateColumnHeader
			// 
			this.AssetManagerOutboxStateColumnHeader.Text = "State";
			this.AssetManagerOutboxStateColumnHeader.Width = 100;
			// 
			// AssetManagerOutboxCreatorColumnHeader
			// 
			this.AssetManagerOutboxCreatorColumnHeader.Text = "Creator";
			this.AssetManagerOutboxCreatorColumnHeader.Width = 0;
			// 
			// AssetManagerOutboxPartyColumnHeader
			// 
			this.AssetManagerOutboxPartyColumnHeader.Text = "Owner";
			this.AssetManagerOutboxPartyColumnHeader.Width = 0;
			// 
			// AssetManagerOutboxOptionsColumnHeader
			// 
			this.AssetManagerOutboxOptionsColumnHeader.Text = "Options";
			this.AssetManagerOutboxOptionsColumnHeader.Width = 0;
			// 
			// AssetManagerOutboxFullnameColumnHeader
			// 
			this.AssetManagerOutboxFullnameColumnHeader.Text = "FullName";
			this.AssetManagerOutboxFullnameColumnHeader.Width = 0;
			// 
			// AssetManagerOutboxBoxColumnHeader
			// 
			this.AssetManagerOutboxBoxColumnHeader.Text = "Box";
			this.AssetManagerOutboxBoxColumnHeader.Width = 0;
			// 
			// AssetManagerOutboxGroupColumnHeader
			// 
			this.AssetManagerOutboxGroupColumnHeader.Text = "Group";
			this.AssetManagerOutboxGroupColumnHeader.Width = 0;
			// 
			// AssetManagerAssetTrashAssetsTabPage
			// 
			this.AssetManagerAssetTrashAssetsTabPage.BackColor = System.Drawing.SystemColors.Control;
			this.AssetManagerAssetTrashAssetsTabPage.Controls.Add(this.AssetManagerTrashTotalSizeLabel);
			this.AssetManagerAssetTrashAssetsTabPage.Controls.Add(this.AssetManagerTrashListView);
			this.AssetManagerAssetTrashAssetsTabPage.ImageIndex = 2;
			this.AssetManagerAssetTrashAssetsTabPage.Location = new System.Drawing.Point(4, 26);
			this.AssetManagerAssetTrashAssetsTabPage.Name = "AssetManagerAssetTrashAssetsTabPage";
			this.AssetManagerAssetTrashAssetsTabPage.Size = new System.Drawing.Size(794, 128);
			this.AssetManagerAssetTrashAssetsTabPage.TabIndex = 2;
			this.AssetManagerAssetTrashAssetsTabPage.Text = "Trash";
			this.AssetManagerAssetTrashAssetsTabPage.ToolTipText = "Check here for assets that may have been deleted or moved by the system";
			this.AssetManagerAssetTrashAssetsTabPage.Visible = false;
			// 
			// AssetManagerTrashTotalSizeLabel
			// 
			this.AssetManagerTrashTotalSizeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.AssetManagerTrashTotalSizeLabel.Location = new System.Drawing.Point(0, 0);
			this.AssetManagerTrashTotalSizeLabel.Name = "AssetManagerTrashTotalSizeLabel";
			this.AssetManagerTrashTotalSizeLabel.Size = new System.Drawing.Size(240, 16);
			this.AssetManagerTrashTotalSizeLabel.TabIndex = 7;
			this.AssetManagerTrashTotalSizeLabel.Text = "Total Size:";
			// 
			// AssetManagerTrashListView
			// 
			this.AssetManagerTrashListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.AssetManagerTrashListView.AutoArrange = false;
			this.AssetManagerTrashListView.BackColor = System.Drawing.SystemColors.Window;
			this.AssetManagerTrashListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.AssetManagerTrashNameColumnHeader,
            this.AssetManagerTrashClassificationColumnHeader,
            this.AssetManagerTrashTargetPathBoxColumnHeader,
            this.AssetManagerTrashDateDeletedColumnHeader,
            this.AssetManagerTrashSizeColumnHeader,
            this.AssetManagerTrashPlatformColumnHeader,
            this.AssetManagerTrashAssetStateColumnHeader,
            this.AssetManagerTrashAssetOwnerColumnHeader,
            this.AssetManagerTrashAssetPartyColumnHeader,
            this.AssetManagerTrashAssetOptionsColumnHeader,
            this.AssetManagerTrashAssetFullnameColumnHeader,
            this.AssetManagerTrashAssetBoxColumnHeader,
            this.AssetManagerTrashAssetGroupColumnHeader});
			this.AssetManagerTrashListView.ForeColor = System.Drawing.SystemColors.ControlText;
			this.AssetManagerTrashListView.FullRowSelect = true;
			this.AssetManagerTrashListView.LabelWrap = false;
			this.AssetManagerTrashListView.Location = new System.Drawing.Point(0, 16);
			this.AssetManagerTrashListView.Name = "AssetManagerTrashListView";
			this.AssetManagerTrashListView.ShowGroups = false;
			this.AssetManagerTrashListView.Size = new System.Drawing.Size(794, 117);
			this.AssetManagerTrashListView.SmallImageList = this.MOGImagesImageList;
			this.AssetManagerTrashListView.TabIndex = 6;
			this.AssetManagerTrashListView.UseCompatibleStateImageBehavior = false;
			this.AssetManagerTrashListView.View = System.Windows.Forms.View.Details;
			// 
			// AssetManagerTrashNameColumnHeader
			// 
			this.AssetManagerTrashNameColumnHeader.Text = "Name";
			this.AssetManagerTrashNameColumnHeader.Width = 283;
			// 
			// AssetManagerTrashClassificationColumnHeader
			// 
			this.AssetManagerTrashClassificationColumnHeader.Text = "MOG Classification";
			this.AssetManagerTrashClassificationColumnHeader.Width = 363;
			// 
			// AssetManagerTrashTargetPathBoxColumnHeader
			// 
			this.AssetManagerTrashTargetPathBoxColumnHeader.Text = "Target Path";
			this.AssetManagerTrashTargetPathBoxColumnHeader.Width = 95;
			// 
			// AssetManagerTrashDateDeletedColumnHeader
			// 
			this.AssetManagerTrashDateDeletedColumnHeader.Text = "Date Deleted";
			this.AssetManagerTrashDateDeletedColumnHeader.Width = 144;
			// 
			// AssetManagerTrashSizeColumnHeader
			// 
			this.AssetManagerTrashSizeColumnHeader.Text = "Size";
			this.AssetManagerTrashSizeColumnHeader.Width = 77;
			// 
			// AssetManagerTrashPlatformColumnHeader
			// 
			this.AssetManagerTrashPlatformColumnHeader.Text = "Platform";
			this.AssetManagerTrashPlatformColumnHeader.Width = 0;
			// 
			// AssetManagerTrashAssetStateColumnHeader
			// 
			this.AssetManagerTrashAssetStateColumnHeader.Text = "AssetState";
			this.AssetManagerTrashAssetStateColumnHeader.Width = 0;
			// 
			// AssetManagerTrashAssetOwnerColumnHeader
			// 
			this.AssetManagerTrashAssetOwnerColumnHeader.Text = "AssetOwner";
			this.AssetManagerTrashAssetOwnerColumnHeader.Width = 0;
			// 
			// AssetManagerTrashAssetPartyColumnHeader
			// 
			this.AssetManagerTrashAssetPartyColumnHeader.Text = "AssetParty";
			this.AssetManagerTrashAssetPartyColumnHeader.Width = 0;
			// 
			// AssetManagerTrashAssetOptionsColumnHeader
			// 
			this.AssetManagerTrashAssetOptionsColumnHeader.Text = "AssetOptions";
			this.AssetManagerTrashAssetOptionsColumnHeader.Width = 0;
			// 
			// AssetManagerTrashAssetFullnameColumnHeader
			// 
			this.AssetManagerTrashAssetFullnameColumnHeader.Text = "Fullname";
			this.AssetManagerTrashAssetFullnameColumnHeader.Width = 0;
			// 
			// AssetManagerTrashAssetBoxColumnHeader
			// 
			this.AssetManagerTrashAssetBoxColumnHeader.Text = "Box";
			this.AssetManagerTrashAssetBoxColumnHeader.Width = 0;
			// 
			// AssetManagerTrashAssetGroupColumnHeader
			// 
			this.AssetManagerTrashAssetGroupColumnHeader.Text = "Group";
			this.AssetManagerTrashAssetGroupColumnHeader.Width = 0;
			// 
			// AssetManagerTasksSplitter
			// 
			this.AssetManagerTasksSplitter.BackColor = System.Drawing.SystemColors.ControlDark;
			this.AssetManagerTasksSplitter.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.AssetManagerTasksSplitter.Location = new System.Drawing.Point(152, 44);
			this.AssetManagerTasksSplitter.MinExtra = 0;
			this.AssetManagerTasksSplitter.MinSize = 0;
			this.AssetManagerTasksSplitter.Name = "AssetManagerTasksSplitter";
			this.AssetManagerTasksSplitter.Size = new System.Drawing.Size(8, 158);
			this.AssetManagerTasksSplitter.TabIndex = 12;
			this.AssetManagerTasksSplitter.TabStop = false;
			// 
			// AssetManagerTaskWindow
			// 
			this.AssetManagerTaskWindow.Dock = System.Windows.Forms.DockStyle.Left;
			this.AssetManagerTaskWindow.Location = new System.Drawing.Point(0, 44);
			this.AssetManagerTaskWindow.Name = "AssetManagerTaskWindow";
			this.AssetManagerTaskWindow.Size = new System.Drawing.Size(152, 158);
			this.AssetManagerTaskWindow.TabIndex = 11;
			this.AssetManagerTaskWindow.TaskEditorEnabled = false;
			this.AssetManagerTaskWindow.TaskListViewEnabled = false;
			this.AssetManagerTaskWindow.Visible = false;
			// 
			// AssetManagerGraphicPanel
			// 
			this.AssetManagerGraphicPanel.Controls.Add(this.AssetManagerActiveUserDepartmentsComboBox);
			this.AssetManagerGraphicPanel.Controls.Add(this.AssetManagerLogoutButton);
			this.AssetManagerGraphicPanel.Controls.Add(this.AssetManagerActiveUserComboBox);
			this.AssetManagerGraphicPanel.Controls.Add(this.AssetManagerAutoImportCheckBox);
			this.AssetManagerGraphicPanel.Controls.Add(this.SkinInboxButtonPictureBox);
			this.AssetManagerGraphicPanel.Controls.Add(this.AssetManagerAutoProcessCheckBox);
			this.AssetManagerGraphicPanel.Controls.Add(this.SkinInboxEndPictureBox);
			this.AssetManagerGraphicPanel.Controls.Add(this.SkinInboxMidPictureBox);
			this.AssetManagerGraphicPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.AssetManagerGraphicPanel.Location = new System.Drawing.Point(0, 0);
			this.AssetManagerGraphicPanel.Name = "AssetManagerGraphicPanel";
			this.AssetManagerGraphicPanel.Size = new System.Drawing.Size(962, 44);
			this.AssetManagerGraphicPanel.TabIndex = 10;
			// 
			// AssetManagerActiveUserDepartmentsComboBox
			// 
			this.AssetManagerActiveUserDepartmentsComboBox.BackColor = System.Drawing.SystemColors.Control;
			this.AssetManagerActiveUserDepartmentsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.AssetManagerActiveUserDepartmentsComboBox.ForeColor = System.Drawing.SystemColors.WindowText;
			this.AssetManagerActiveUserDepartmentsComboBox.Location = new System.Drawing.Point(201, 14);
			this.AssetManagerActiveUserDepartmentsComboBox.MaxDropDownItems = 17;
			this.AssetManagerActiveUserDepartmentsComboBox.Name = "AssetManagerActiveUserDepartmentsComboBox";
			this.AssetManagerActiveUserDepartmentsComboBox.Size = new System.Drawing.Size(170, 21);
			this.AssetManagerActiveUserDepartmentsComboBox.Sorted = true;
			this.AssetManagerActiveUserDepartmentsComboBox.TabIndex = 8;
			this.MogToolTip.SetToolTip(this.AssetManagerActiveUserDepartmentsComboBox, "Select a department");
			this.AssetManagerActiveUserDepartmentsComboBox.SelectedIndexChanged += new System.EventHandler(this.AssetManagerActiveUserDepartmentsComboBox_SelectedIndexChanged);
			// 
			// AssetManagerLogoutButton
			// 
			this.AssetManagerLogoutButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.AssetManagerLogoutButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(105)))), ((int)(((byte)(218)))));
			this.AssetManagerLogoutButton.Enabled = false;
			this.AssetManagerLogoutButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.AssetManagerLogoutButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.AssetManagerLogoutButton.ForeColor = System.Drawing.Color.White;
			this.AssetManagerLogoutButton.Location = new System.Drawing.Point(864, 15);
			this.AssetManagerLogoutButton.Name = "AssetManagerLogoutButton";
			this.AssetManagerLogoutButton.Size = new System.Drawing.Size(80, 16);
			this.AssetManagerLogoutButton.TabIndex = 6;
			this.AssetManagerLogoutButton.Text = "Logout...";
			this.MogToolTip.SetToolTip(this.AssetManagerLogoutButton, "Logout and login to new project or as new user");
			this.AssetManagerLogoutButton.UseVisualStyleBackColor = false;
			this.AssetManagerLogoutButton.Click += new System.EventHandler(this.AssetManagerLogoutButton_Click);
			// 
			// AssetManagerActiveUserComboBox
			// 
			this.AssetManagerActiveUserComboBox.BackColor = System.Drawing.SystemColors.Control;
			this.AssetManagerActiveUserComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.AssetManagerActiveUserComboBox.ForeColor = System.Drawing.SystemColors.WindowText;
			this.AssetManagerActiveUserComboBox.Location = new System.Drawing.Point(377, 14);
			this.AssetManagerActiveUserComboBox.MaxDropDownItems = 17;
			this.AssetManagerActiveUserComboBox.Name = "AssetManagerActiveUserComboBox";
			this.AssetManagerActiveUserComboBox.Size = new System.Drawing.Size(170, 21);
			this.AssetManagerActiveUserComboBox.Sorted = true;
			this.AssetManagerActiveUserComboBox.TabIndex = 0;
			this.MogToolTip.SetToolTip(this.AssetManagerActiveUserComboBox, "Select a users inboxes");
			this.AssetManagerActiveUserComboBox.SelectedIndexChanged += new System.EventHandler(this.cboxAssetManagerUser_SelectedIndexChanged);
			this.AssetManagerActiveUserComboBox.TextChanged += new System.EventHandler(this.AssetManagerActiveUserComboBox_TextChanged);
			this.AssetManagerActiveUserComboBox.Click += new System.EventHandler(this.AssetManagerActiveUserComboBox_Click);
			// 
			// AssetManagerAutoImportCheckBox
			// 
			this.AssetManagerAutoImportCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.AssetManagerAutoImportCheckBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(105)))), ((int)(((byte)(217)))));
			this.AssetManagerAutoImportCheckBox.Checked = true;
			this.AssetManagerAutoImportCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.AssetManagerAutoImportCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.AssetManagerAutoImportCheckBox.ForeColor = System.Drawing.Color.White;
			this.AssetManagerAutoImportCheckBox.Location = new System.Drawing.Point(746, 16);
			this.AssetManagerAutoImportCheckBox.Name = "AssetManagerAutoImportCheckBox";
			this.AssetManagerAutoImportCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.AssetManagerAutoImportCheckBox.Size = new System.Drawing.Size(98, 16);
			this.AssetManagerAutoImportCheckBox.TabIndex = 4;
			this.AssetManagerAutoImportCheckBox.Text = "Auto Import";
			this.MogToolTip.SetToolTip(this.AssetManagerAutoImportCheckBox, "MOG will attempt to auto detect the correct Asset name for the imported file");
			this.AssetManagerAutoImportCheckBox.UseVisualStyleBackColor = false;
			// 
			// SkinInboxButtonPictureBox
			// 
			this.SkinInboxButtonPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("SkinInboxButtonPictureBox.Image")));
			this.SkinInboxButtonPictureBox.Location = new System.Drawing.Point(0, 0);
			this.SkinInboxButtonPictureBox.Name = "SkinInboxButtonPictureBox";
			this.SkinInboxButtonPictureBox.RightBorderCopyWidth = 5;
			this.SkinInboxButtonPictureBox.Size = new System.Drawing.Size(192, 44);
			this.SkinInboxButtonPictureBox.TabIndex = 6;
			this.SkinInboxButtonPictureBox.TabStop = false;
			this.MogToolTip.SetToolTip(this.SkinInboxButtonPictureBox, "Click here to return to your drafts");
			this.SkinInboxButtonPictureBox.MouseLeave += new System.EventHandler(this.SkinInboxButtonPictureBox_MouseLeave);
			this.SkinInboxButtonPictureBox.Click += new System.EventHandler(this.AssetManagerHomeButton_Click);
			this.SkinInboxButtonPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SkinInboxButtonPictureBox_MouseDown);
			this.SkinInboxButtonPictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SkinInboxButtonPictureBox_MouseUp);
			this.SkinInboxButtonPictureBox.MouseEnter += new System.EventHandler(this.SkinInboxButtonPictureBox_MouseEnter);
			// 
			// AssetManagerAutoProcessCheckBox
			// 
			this.AssetManagerAutoProcessCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.AssetManagerAutoProcessCheckBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(105)))), ((int)(((byte)(217)))));
			this.AssetManagerAutoProcessCheckBox.Checked = true;
			this.AssetManagerAutoProcessCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.AssetManagerAutoProcessCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.AssetManagerAutoProcessCheckBox.ForeColor = System.Drawing.Color.White;
			this.AssetManagerAutoProcessCheckBox.Location = new System.Drawing.Point(644, 16);
			this.AssetManagerAutoProcessCheckBox.Name = "AssetManagerAutoProcessCheckBox";
			this.AssetManagerAutoProcessCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.AssetManagerAutoProcessCheckBox.Size = new System.Drawing.Size(96, 16);
			this.AssetManagerAutoProcessCheckBox.TabIndex = 1;
			this.AssetManagerAutoProcessCheckBox.Text = "Auto Rip";
			this.MogToolTip.SetToolTip(this.AssetManagerAutoProcessCheckBox, "Auto process any newly imported asset");
			this.AssetManagerAutoProcessCheckBox.UseVisualStyleBackColor = false;
			// 
			// SkinInboxEndPictureBox
			// 
			this.SkinInboxEndPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.SkinInboxEndPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("SkinInboxEndPictureBox.Image")));
			this.SkinInboxEndPictureBox.Location = new System.Drawing.Point(946, 0);
			this.SkinInboxEndPictureBox.Name = "SkinInboxEndPictureBox";
			this.SkinInboxEndPictureBox.Size = new System.Drawing.Size(16, 44);
			this.SkinInboxEndPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.SkinInboxEndPictureBox.TabIndex = 7;
			this.SkinInboxEndPictureBox.TabStop = false;
			// 
			// SkinInboxMidPictureBox
			// 
			this.SkinInboxMidPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.SkinInboxMidPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("SkinInboxMidPictureBox.Image")));
			this.SkinInboxMidPictureBox.Location = new System.Drawing.Point(192, 0);
			this.SkinInboxMidPictureBox.Name = "SkinInboxMidPictureBox";
			this.SkinInboxMidPictureBox.RightBorderCopyWidth = 5;
			this.SkinInboxMidPictureBox.Size = new System.Drawing.Size(754, 44);
			this.SkinInboxMidPictureBox.TabIndex = 7;
			this.SkinInboxMidPictureBox.TabStop = false;
			this.MogToolTip.SetToolTip(this.SkinInboxMidPictureBox, "Click here to return to your drafts");
			// 
			// AssetManagerSplitter
			// 
			this.AssetManagerSplitter.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.AssetManagerSplitter.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.AssetManagerSplitter.Location = new System.Drawing.Point(0, 202);
			this.AssetManagerSplitter.MinExtra = 5;
			this.AssetManagerSplitter.MinSize = 3;
			this.AssetManagerSplitter.Name = "AssetManagerSplitter";
			this.AssetManagerSplitter.Size = new System.Drawing.Size(962, 10);
			this.AssetManagerSplitter.TabIndex = 3;
			this.AssetManagerSplitter.TabStop = false;
			// 
			// AssetManagerBottomPanel
			// 
			this.AssetManagerBottomPanel.BackColor = System.Drawing.SystemColors.Control;
			this.AssetManagerBottomPanel.Controls.Add(this.LocalBranchMiddlePanel);
			this.AssetManagerBottomPanel.Controls.Add(this.AssetManagerLocalDataExplorerSplitter);
			this.AssetManagerBottomPanel.Controls.Add(this.LocalBranchLeftPanel);
			this.AssetManagerBottomPanel.Controls.Add(this.AssetManagerLocalDataSplitter);
			this.AssetManagerBottomPanel.Controls.Add(this.LocalBranchRightPanel);
			this.AssetManagerBottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.AssetManagerBottomPanel.Location = new System.Drawing.Point(0, 212);
			this.AssetManagerBottomPanel.Name = "AssetManagerBottomPanel";
			this.AssetManagerBottomPanel.Size = new System.Drawing.Size(962, 280);
			this.AssetManagerBottomPanel.TabIndex = 5;
			// 
			// LocalBranchMiddlePanel
			// 
			this.LocalBranchMiddlePanel.AutoScroll = true;
			this.LocalBranchMiddlePanel.AutoScrollMinSize = new System.Drawing.Size(446, 48);
			this.LocalBranchMiddlePanel.BackColor = System.Drawing.SystemColors.ControlDark;
			this.LocalBranchMiddlePanel.Controls.Add(this.AssetManagerAutoUpdateLocalCheckBox);
			this.LocalBranchMiddlePanel.Controls.Add(this.AssetManagerAutoPackageCheckBox);
			this.LocalBranchMiddlePanel.Controls.Add(this.AssetManagerPackageButton);
			this.LocalBranchMiddlePanel.Controls.Add(this.GetLatestAdvancedButton);
			this.LocalBranchMiddlePanel.Controls.Add(this.AssetManagerMergeVersionButton);
			this.LocalBranchMiddlePanel.Controls.Add(this.AssetManagerLocalDataTabControl);
			this.LocalBranchMiddlePanel.Controls.Add(this.SkinMyWorkspaceEndPictureBox);
			this.LocalBranchMiddlePanel.Controls.Add(this.SkinMyWorkspacePictureBox);
			this.LocalBranchMiddlePanel.Controls.Add(this.SkinMyWorkspaceMidPictureBox);
			this.LocalBranchMiddlePanel.Controls.Add(this.LocalWorkspaceSyncTargetTextBox);
			this.LocalBranchMiddlePanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.LocalBranchMiddlePanel.Location = new System.Drawing.Point(200, 0);
			this.LocalBranchMiddlePanel.Name = "LocalBranchMiddlePanel";
			this.LocalBranchMiddlePanel.Size = new System.Drawing.Size(504, 280);
			this.LocalBranchMiddlePanel.TabIndex = 10;
			// 
			// AssetManagerAutoPackageCheckBox
			// 
			this.AssetManagerAutoPackageCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.AssetManagerAutoPackageCheckBox.BackColor = System.Drawing.Color.Transparent;
			this.AssetManagerAutoPackageCheckBox.Checked = true;
			this.AssetManagerAutoPackageCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.AssetManagerAutoPackageCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.AssetManagerAutoPackageCheckBox.ForeColor = System.Drawing.Color.White;
			this.AssetManagerAutoPackageCheckBox.Location = new System.Drawing.Point(362, 9);
			this.AssetManagerAutoPackageCheckBox.Name = "AssetManagerAutoPackageCheckBox";
			this.AssetManagerAutoPackageCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.AssetManagerAutoPackageCheckBox.Size = new System.Drawing.Size(16, 14);
			this.AssetManagerAutoPackageCheckBox.TabIndex = 28;
			this.MogToolTip.SetToolTip(this.AssetManagerAutoPackageCheckBox, "Launch package merge command whenever a local asset is updated");
			this.AssetManagerAutoPackageCheckBox.UseVisualStyleBackColor = false;
			this.AssetManagerAutoPackageCheckBox.CheckedChanged += new System.EventHandler(this.AssetManagerAutoPackageCheckBox_CheckedChanged);
			// 
			// AssetManagerPackageButton
			// 
			this.AssetManagerPackageButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.AssetManagerPackageButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(105)))), ((int)(((byte)(218)))));
			this.AssetManagerPackageButton.Enabled = false;
			this.AssetManagerPackageButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.AssetManagerPackageButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.AssetManagerPackageButton.ForeColor = System.Drawing.Color.White;
			this.AssetManagerPackageButton.Location = new System.Drawing.Point(285, 7);
			this.AssetManagerPackageButton.Name = "AssetManagerPackageButton";
			this.AssetManagerPackageButton.Size = new System.Drawing.Size(95, 18);
			this.AssetManagerPackageButton.TabIndex = 5;
			this.AssetManagerPackageButton.Text = "Package";
			this.MogToolTip.SetToolTip(this.AssetManagerPackageButton, "Launch the package merge command on your local machine");
			this.AssetManagerPackageButton.UseVisualStyleBackColor = false;
			this.AssetManagerPackageButton.Click += new System.EventHandler(this.AssetManagerPackageButton_Click);
			// 
			// GetLatestAdvancedButton
			// 
			this.GetLatestAdvancedButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.GetLatestAdvancedButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(105)))), ((int)(((byte)(218)))));
			this.GetLatestAdvancedButton.FlatAppearance.BorderSize = 0;
			this.GetLatestAdvancedButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.GetLatestAdvancedButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
			this.GetLatestAdvancedButton.ForeColor = System.Drawing.Color.White;
			this.GetLatestAdvancedButton.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
			this.GetLatestAdvancedButton.Location = new System.Drawing.Point(96, 7);
			this.GetLatestAdvancedButton.Name = "GetLatestAdvancedButton";
			this.GetLatestAdvancedButton.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.GetLatestAdvancedButton.Size = new System.Drawing.Size(88, 18);
			this.GetLatestAdvancedButton.TabIndex = 29;
			this.GetLatestAdvancedButton.Text = "Get ";
			this.GetLatestAdvancedButton.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.MogToolTip.SetToolTip(this.GetLatestAdvancedButton, "Get the most current data from the server");
			this.GetLatestAdvancedButton.UseVisualStyleBackColor = false;
			this.GetLatestAdvancedButton.Visible = false;
			this.GetLatestAdvancedButton.Click += new System.EventHandler(this.GetLatestAdvanced_Click);
			// 
			// AssetManagerAutoUpdateLocalCheckBox
			// 
			this.AssetManagerAutoUpdateLocalCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.AssetManagerAutoUpdateLocalCheckBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(82)))), ((int)(((byte)(172)))));
			this.AssetManagerAutoUpdateLocalCheckBox.Checked = true;
			this.AssetManagerAutoUpdateLocalCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.AssetManagerAutoUpdateLocalCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.AssetManagerAutoUpdateLocalCheckBox.ForeColor = System.Drawing.Color.White;
			this.AssetManagerAutoUpdateLocalCheckBox.Location = new System.Drawing.Point(383, 8);
			this.AssetManagerAutoUpdateLocalCheckBox.Name = "AssetManagerAutoUpdateLocalCheckBox";
			this.AssetManagerAutoUpdateLocalCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.AssetManagerAutoUpdateLocalCheckBox.Size = new System.Drawing.Size(105, 16);
			this.AssetManagerAutoUpdateLocalCheckBox.TabIndex = 3;
			this.AssetManagerAutoUpdateLocalCheckBox.Text = "Auto Update";
			this.MogToolTip.SetToolTip(this.AssetManagerAutoUpdateLocalCheckBox, "Copy any new asset within your inbox to your local workspace");
			this.AssetManagerAutoUpdateLocalCheckBox.UseVisualStyleBackColor = false;
			this.AssetManagerAutoUpdateLocalCheckBox.CheckedChanged += new System.EventHandler(this.AssetManagerAutoUpdateLocalCheckBox_CheckedChanged);
			// 
			// AssetManagerMergeVersionButton
			// 
			this.AssetManagerMergeVersionButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.AssetManagerMergeVersionButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(105)))), ((int)(((byte)(218)))));
			this.AssetManagerMergeVersionButton.Enabled = false;
			this.AssetManagerMergeVersionButton.FlatAppearance.BorderSize = 0;
			this.AssetManagerMergeVersionButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.AssetManagerMergeVersionButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.AssetManagerMergeVersionButton.ForeColor = System.Drawing.Color.White;
			this.AssetManagerMergeVersionButton.Location = new System.Drawing.Point(185, 7);
			this.AssetManagerMergeVersionButton.Name = "AssetManagerMergeVersionButton";
			this.AssetManagerMergeVersionButton.Size = new System.Drawing.Size(96, 18);
			this.AssetManagerMergeVersionButton.TabIndex = 2;
			this.AssetManagerMergeVersionButton.Text = "Get latest";
			this.AssetManagerMergeVersionButton.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
			this.MogToolTip.SetToolTip(this.AssetManagerMergeVersionButton, "Get the most current data from the server");
			this.AssetManagerMergeVersionButton.UseVisualStyleBackColor = false;
			this.AssetManagerMergeVersionButton.Click += new System.EventHandler(this.GetLatest_Click);
			// 
			// AssetManagerLocalDataTabControl
			// 
			this.AssetManagerLocalDataTabControl.ContextMenuStrip = this.LocalBranchContextMenu;
			this.AssetManagerLocalDataTabControl.Controls.Add(this.tabPage1);
			this.AssetManagerLocalDataTabControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.AssetManagerLocalDataTabControl.HotTrack = true;
			this.AssetManagerLocalDataTabControl.Location = new System.Drawing.Point(1, 34);
			this.AssetManagerLocalDataTabControl.Multiline = true;
			this.AssetManagerLocalDataTabControl.Name = "AssetManagerLocalDataTabControl";
			this.AssetManagerLocalDataTabControl.Padding = new System.Drawing.Point(0, 0);
			this.AssetManagerLocalDataTabControl.SelectedIndex = 0;
			this.AssetManagerLocalDataTabControl.ShowToolTips = true;
			this.AssetManagerLocalDataTabControl.Size = new System.Drawing.Size(497, 208);
			this.AssetManagerLocalDataTabControl.TabIndex = 17;
			this.AssetManagerLocalDataTabControl.SelectedIndexChanged += new System.EventHandler(this.LocalBranchTabControl_SelectedIndexChanged);
			// 
			// LocalBranchContextMenu
			// 
			this.LocalBranchContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LocalBranchExploreMenuItem,
            this.LocalBranchSep1MenuItem,
            this.LocalBranchRenameMenuItem,
            this.LocalBranchSep2MenuItem,
            this.LocalBranchAddMenuItem,
            this.LocalBranchRemoveMenuItem,
            this.LocalBranchSep3MenuItem,
            this.LocalBranchTagMenuItem});
			this.LocalBranchContextMenu.Name = "LocalBranchContextMenu";
			this.LocalBranchContextMenu.Size = new System.Drawing.Size(181, 132);
			// 
			// LocalBranchExploreMenuItem
			// 
			this.LocalBranchExploreMenuItem.Name = "LocalBranchExploreMenuItem";
			this.LocalBranchExploreMenuItem.Size = new System.Drawing.Size(180, 22);
			this.LocalBranchExploreMenuItem.Text = "Explore Tab";
			this.LocalBranchExploreMenuItem.Click += new System.EventHandler(this.LocalBranchExploreMenuItem_Click);
			// 
			// LocalBranchSep1MenuItem
			// 
			this.LocalBranchSep1MenuItem.Name = "LocalBranchSep1MenuItem";
			this.LocalBranchSep1MenuItem.Size = new System.Drawing.Size(177, 6);
			// 
			// LocalBranchRenameMenuItem
			// 
			this.LocalBranchRenameMenuItem.Name = "LocalBranchRenameMenuItem";
			this.LocalBranchRenameMenuItem.Size = new System.Drawing.Size(180, 22);
			this.LocalBranchRenameMenuItem.Text = "Rename Tab";
			this.LocalBranchRenameMenuItem.Click += new System.EventHandler(this.LocalBranchRenameMenuItem_Click);
			// 
			// LocalBranchSep2MenuItem
			// 
			this.LocalBranchSep2MenuItem.Name = "LocalBranchSep2MenuItem";
			this.LocalBranchSep2MenuItem.Size = new System.Drawing.Size(177, 6);
			// 
			// LocalBranchAddMenuItem
			// 
			this.LocalBranchAddMenuItem.Name = "LocalBranchAddMenuItem";
			this.LocalBranchAddMenuItem.Size = new System.Drawing.Size(180, 22);
			this.LocalBranchAddMenuItem.Text = "New Workspace";
			this.LocalBranchAddMenuItem.Click += new System.EventHandler(this.LocalBranchAddMenuItem_Click);
			// 
			// LocalBranchRemoveMenuItem
			// 
			this.LocalBranchRemoveMenuItem.Name = "LocalBranchRemoveMenuItem";
			this.LocalBranchRemoveMenuItem.Size = new System.Drawing.Size(180, 22);
			this.LocalBranchRemoveMenuItem.Text = "Remove Workspace";
			this.LocalBranchRemoveMenuItem.Click += new System.EventHandler(this.LocalBranchRemoveMenuItem_Click);
			// 
			// LocalBranchSep3MenuItem
			// 
			this.LocalBranchSep3MenuItem.Name = "LocalBranchSep3MenuItem";
			this.LocalBranchSep3MenuItem.Size = new System.Drawing.Size(177, 6);
			// 
			// LocalBranchTagMenuItem
			// 
			this.LocalBranchTagMenuItem.Name = "LocalBranchTagMenuItem";
			this.LocalBranchTagMenuItem.Size = new System.Drawing.Size(180, 22);
			this.LocalBranchTagMenuItem.Text = "Tag workspace";
			this.LocalBranchTagMenuItem.Click += new System.EventHandler(this.LocalBranchTagMenuItem_Click);
			// 
			// tabPage1
			// 
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Size = new System.Drawing.Size(489, 182);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "tabPage1";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// SkinMyWorkspaceEndPictureBox
			// 
			this.SkinMyWorkspaceEndPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.SkinMyWorkspaceEndPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("SkinMyWorkspaceEndPictureBox.Image")));
			this.SkinMyWorkspaceEndPictureBox.Location = new System.Drawing.Point(480, 0);
			this.SkinMyWorkspaceEndPictureBox.Name = "SkinMyWorkspaceEndPictureBox";
			this.SkinMyWorkspaceEndPictureBox.Size = new System.Drawing.Size(24, 28);
			this.SkinMyWorkspaceEndPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.SkinMyWorkspaceEndPictureBox.TabIndex = 26;
			this.SkinMyWorkspaceEndPictureBox.TabStop = false;
			// 
			// SkinMyWorkspacePictureBox
			// 
			this.SkinMyWorkspacePictureBox.BackColor = System.Drawing.Color.Transparent;
			this.SkinMyWorkspacePictureBox.Image = ((System.Drawing.Image)(resources.GetObject("SkinMyWorkspacePictureBox.Image")));
			this.SkinMyWorkspacePictureBox.Location = new System.Drawing.Point(0, 0);
			this.SkinMyWorkspacePictureBox.Name = "SkinMyWorkspacePictureBox";
			this.SkinMyWorkspacePictureBox.RightBorderCopyWidth = 5;
			this.SkinMyWorkspacePictureBox.Size = new System.Drawing.Size(140, 28);
			this.SkinMyWorkspacePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.SkinMyWorkspacePictureBox.TabIndex = 18;
			this.SkinMyWorkspacePictureBox.TabStop = false;
			this.MogToolTip.SetToolTip(this.SkinMyWorkspacePictureBox, "Click here to create your workspace");
			this.SkinMyWorkspacePictureBox.MouseLeave += new System.EventHandler(this.LocalBranchPictureBox_MouseLeave);
			this.SkinMyWorkspacePictureBox.Click += new System.EventHandler(this.LocalBranchPictureBox_Click);
			this.SkinMyWorkspacePictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LocalBranchPictureBox_MouseDown);
			this.SkinMyWorkspacePictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.LocalBranchPictureBox_MouseUp);
			this.SkinMyWorkspacePictureBox.MouseEnter += new System.EventHandler(this.LocalBranchPictureBox_MouseEnter);
			// 
			// SkinMyWorkspaceMidPictureBox
			// 
			this.SkinMyWorkspaceMidPictureBox.BackColor = System.Drawing.Color.Transparent;
			this.SkinMyWorkspaceMidPictureBox.Dock = System.Windows.Forms.DockStyle.Top;
			this.SkinMyWorkspaceMidPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("SkinMyWorkspaceMidPictureBox.Image")));
			this.SkinMyWorkspaceMidPictureBox.Location = new System.Drawing.Point(0, 0);
			this.SkinMyWorkspaceMidPictureBox.Name = "SkinMyWorkspaceMidPictureBox";
			this.SkinMyWorkspaceMidPictureBox.RightBorderCopyWidth = 5;
			this.SkinMyWorkspaceMidPictureBox.Size = new System.Drawing.Size(504, 32);
			this.SkinMyWorkspaceMidPictureBox.TabIndex = 27;
			this.SkinMyWorkspaceMidPictureBox.TabStop = false;
			// 
			// LocalWorkspaceSyncTargetTextBox
			// 
			this.LocalWorkspaceSyncTargetTextBox.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.LocalWorkspaceSyncTargetTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LocalWorkspaceSyncTargetTextBox.ForeColor = System.Drawing.SystemColors.HotTrack;
			this.LocalWorkspaceSyncTargetTextBox.Location = new System.Drawing.Point(0, 260);
			this.LocalWorkspaceSyncTargetTextBox.Name = "LocalWorkspaceSyncTargetTextBox";
			this.LocalWorkspaceSyncTargetTextBox.ReadOnly = true;
			this.LocalWorkspaceSyncTargetTextBox.Size = new System.Drawing.Size(504, 20);
			this.LocalWorkspaceSyncTargetTextBox.TabIndex = 21;
			this.LocalWorkspaceSyncTargetTextBox.Visible = false;
			// 
			// AssetManagerLocalDataExplorerSplitter
			// 
			this.AssetManagerLocalDataExplorerSplitter.BackColor = System.Drawing.SystemColors.ControlDark;
			this.AssetManagerLocalDataExplorerSplitter.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.AssetManagerLocalDataExplorerSplitter.Location = new System.Drawing.Point(192, 0);
			this.AssetManagerLocalDataExplorerSplitter.MinExtra = 0;
			this.AssetManagerLocalDataExplorerSplitter.MinSize = 0;
			this.AssetManagerLocalDataExplorerSplitter.Name = "AssetManagerLocalDataExplorerSplitter";
			this.AssetManagerLocalDataExplorerSplitter.Size = new System.Drawing.Size(8, 280);
			this.AssetManagerLocalDataExplorerSplitter.TabIndex = 9;
			this.AssetManagerLocalDataExplorerSplitter.TabStop = false;
			this.AssetManagerLocalDataExplorerSplitter.SplitterMoving += new System.Windows.Forms.SplitterEventHandler(this.AssetManagerLocalDataExplorerSplitter_SplitterMoving);
			// 
			// LocalBranchLeftPanel
			// 
			this.LocalBranchLeftPanel.Controls.Add(this.SkinLocalExplorerEndPictureBox);
			this.LocalBranchLeftPanel.Controls.Add(this.LocalBranchMogControl_GameDataDestinationTreeView);
			this.LocalBranchLeftPanel.Controls.Add(this.SkinLocalExplorerButtonPictureBox);
			this.LocalBranchLeftPanel.Dock = System.Windows.Forms.DockStyle.Left;
			this.LocalBranchLeftPanel.Location = new System.Drawing.Point(0, 0);
			this.LocalBranchLeftPanel.Name = "LocalBranchLeftPanel";
			this.LocalBranchLeftPanel.Size = new System.Drawing.Size(192, 280);
			this.LocalBranchLeftPanel.TabIndex = 23;
			// 
			// SkinLocalExplorerEndPictureBox
			// 
			this.SkinLocalExplorerEndPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.SkinLocalExplorerEndPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("SkinLocalExplorerEndPictureBox.Image")));
			this.SkinLocalExplorerEndPictureBox.Location = new System.Drawing.Point(176, 0);
			this.SkinLocalExplorerEndPictureBox.Name = "SkinLocalExplorerEndPictureBox";
			this.SkinLocalExplorerEndPictureBox.Size = new System.Drawing.Size(16, 28);
			this.SkinLocalExplorerEndPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.SkinLocalExplorerEndPictureBox.TabIndex = 25;
			this.SkinLocalExplorerEndPictureBox.TabStop = false;
			// 
			// LocalBranchMogControl_GameDataDestinationTreeView
			// 
			this.LocalBranchMogControl_GameDataDestinationTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.LocalBranchMogControl_GameDataDestinationTreeView.Location = new System.Drawing.Point(0, 28);
			this.LocalBranchMogControl_GameDataDestinationTreeView.MOGAllowDirectoryOpperations = false;
			this.LocalBranchMogControl_GameDataDestinationTreeView.MOGShowFiles = true;
			this.LocalBranchMogControl_GameDataDestinationTreeView.MOGSorted = false;
			this.LocalBranchMogControl_GameDataDestinationTreeView.Name = "LocalBranchMogControl_GameDataDestinationTreeView";
			this.LocalBranchMogControl_GameDataDestinationTreeView.Size = new System.Drawing.Size(192, 252);
			this.LocalBranchMogControl_GameDataDestinationTreeView.TabIndex = 8;
			this.LocalBranchMogControl_GameDataDestinationTreeView.MOGAfterTargetSelect += new MOG_ControlsLibrary.Common.MogControl_LocalBranchTreeView.MogControl_LocalBranchTreeView.TreeViewEvent(this.LocalBranchMogControl_GameDataDestinationTreeView_AfterTargetSelect);
			this.LocalBranchMogControl_GameDataDestinationTreeView.MOGTreeClick += new MOG_ControlsLibrary.Common.MogControl_LocalBranchTreeView.MogControl_LocalBranchTreeView.TreeViewClickEvent(this.LocalBranchMogControl_GameDataDestinationTreeView_MOGTreeClick);
			// 
			// SkinLocalExplorerButtonPictureBox
			// 
			this.SkinLocalExplorerButtonPictureBox.BackColor = System.Drawing.Color.Transparent;
			this.SkinLocalExplorerButtonPictureBox.Dock = System.Windows.Forms.DockStyle.Top;
			this.SkinLocalExplorerButtonPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("SkinLocalExplorerButtonPictureBox.Image")));
			this.SkinLocalExplorerButtonPictureBox.Location = new System.Drawing.Point(0, 0);
			this.SkinLocalExplorerButtonPictureBox.Name = "SkinLocalExplorerButtonPictureBox";
			this.SkinLocalExplorerButtonPictureBox.RightBorderCopyWidth = 2;
			this.SkinLocalExplorerButtonPictureBox.Size = new System.Drawing.Size(192, 28);
			this.SkinLocalExplorerButtonPictureBox.TabIndex = 24;
			this.SkinLocalExplorerButtonPictureBox.TabStop = false;
			this.MogToolTip.SetToolTip(this.SkinLocalExplorerButtonPictureBox, "The local workspace explorer allows you to browse all the local workspaces that y" +
					"ou have created within MOG.");
			// 
			// AssetManagerLocalDataSplitter
			// 
			this.AssetManagerLocalDataSplitter.BackColor = System.Drawing.SystemColors.ControlDark;
			this.AssetManagerLocalDataSplitter.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.AssetManagerLocalDataSplitter.Dock = System.Windows.Forms.DockStyle.Right;
			this.AssetManagerLocalDataSplitter.Location = new System.Drawing.Point(704, 0);
			this.AssetManagerLocalDataSplitter.MinExtra = 0;
			this.AssetManagerLocalDataSplitter.MinSize = 0;
			this.AssetManagerLocalDataSplitter.Name = "AssetManagerLocalDataSplitter";
			this.AssetManagerLocalDataSplitter.Size = new System.Drawing.Size(8, 280);
			this.AssetManagerLocalDataSplitter.TabIndex = 21;
			this.AssetManagerLocalDataSplitter.TabStop = false;
			this.AssetManagerLocalDataSplitter.SplitterMoving += new System.Windows.Forms.SplitterEventHandler(this.AssetManagerLocalDataSplitter_SplitterMoving);
			// 
			// LocalBranchRightPanel
			// 
			this.LocalBranchRightPanel.Controls.Add(this.SkinToolboxEndPictureBox);
			this.LocalBranchRightPanel.Controls.Add(this.CustomToolsBox);
			this.LocalBranchRightPanel.Controls.Add(this.SkinToolboxButtonPictureBox);
			this.LocalBranchRightPanel.Dock = System.Windows.Forms.DockStyle.Right;
			this.LocalBranchRightPanel.Location = new System.Drawing.Point(712, 0);
			this.LocalBranchRightPanel.Name = "LocalBranchRightPanel";
			this.LocalBranchRightPanel.Size = new System.Drawing.Size(250, 280);
			this.LocalBranchRightPanel.TabIndex = 3;
			this.LocalBranchRightPanel.Layout += new System.Windows.Forms.LayoutEventHandler(this.LocalBranchRightPanel_Layout);
			// 
			// SkinToolboxEndPictureBox
			// 
			this.SkinToolboxEndPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.SkinToolboxEndPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("SkinToolboxEndPictureBox.Image")));
			this.SkinToolboxEndPictureBox.Location = new System.Drawing.Point(233, 0);
			this.SkinToolboxEndPictureBox.Name = "SkinToolboxEndPictureBox";
			this.SkinToolboxEndPictureBox.Size = new System.Drawing.Size(16, 28);
			this.SkinToolboxEndPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.SkinToolboxEndPictureBox.TabIndex = 27;
			this.SkinToolboxEndPictureBox.TabStop = false;
			// 
			// CustomToolsBox
			// 
			this.CustomToolsBox.AutoScroll = true;
			this.CustomToolsBox.AutoScrollMinSize = new System.Drawing.Size(16, 16);
			this.CustomToolsBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.CustomToolsBox.Location = new System.Drawing.Point(0, 28);
			this.CustomToolsBox.MainForm = null;
			this.CustomToolsBox.Name = "CustomToolsBox";
			this.CustomToolsBox.Size = new System.Drawing.Size(250, 252);
			this.CustomToolsBox.TabIndex = 13;
			// 
			// SkinToolboxButtonPictureBox
			// 
			this.SkinToolboxButtonPictureBox.BackColor = System.Drawing.Color.Transparent;
			this.SkinToolboxButtonPictureBox.Dock = System.Windows.Forms.DockStyle.Top;
			this.SkinToolboxButtonPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("SkinToolboxButtonPictureBox.Image")));
			this.SkinToolboxButtonPictureBox.Location = new System.Drawing.Point(0, 0);
			this.SkinToolboxButtonPictureBox.Name = "SkinToolboxButtonPictureBox";
			this.SkinToolboxButtonPictureBox.RightBorderCopyWidth = 5;
			this.SkinToolboxButtonPictureBox.Size = new System.Drawing.Size(250, 28);
			this.SkinToolboxButtonPictureBox.TabIndex = 9;
			this.SkinToolboxButtonPictureBox.TabStop = false;
			this.MogToolTip.SetToolTip(this.SkinToolboxButtonPictureBox, "After creating your workspace, click here to add tools (after clicking this butto" +
					"n, right click to add tools)");
			this.SkinToolboxButtonPictureBox.MouseLeave += new System.EventHandler(this.CustomToolsPictureBox_MouseLeave);
			this.SkinToolboxButtonPictureBox.Click += new System.EventHandler(this.CustomToolsPictureBox_Click);
			this.SkinToolboxButtonPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CustomToolsPictureBox_MouseDown);
			this.SkinToolboxButtonPictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.CustomToolsPictureBox_MouseUp);
			this.SkinToolboxButtonPictureBox.MouseEnter += new System.EventHandler(this.CustomToolsPictureBox_MouseEnter);
			// 
			// MainTabProjectManagerTabPage
			// 
			this.MainTabProjectManagerTabPage.BackColor = System.Drawing.Color.Transparent;
			this.MainTabProjectManagerTabPage.Controls.Add(this.ProjectManagerTasksPanel);
			this.MainTabProjectManagerTabPage.Controls.Add(this.splitter1);
			this.MainTabProjectManagerTabPage.Controls.Add(this.ProjectManagerTaskWindow);
			this.MainTabProjectManagerTabPage.ImageIndex = 1;
			this.MainTabProjectManagerTabPage.Location = new System.Drawing.Point(4, 22);
			this.MainTabProjectManagerTabPage.Name = "MainTabProjectManagerTabPage";
			this.MainTabProjectManagerTabPage.Size = new System.Drawing.Size(962, 492);
			this.MainTabProjectManagerTabPage.TabIndex = 0;
			this.MainTabProjectManagerTabPage.Tag = "3";
			this.MainTabProjectManagerTabPage.Text = "Project";
			this.MainTabProjectManagerTabPage.ToolTipText = "Select this tab to view all blessed asset version histories for your current proj" +
				"ect and branch";
			this.MainTabProjectManagerTabPage.UseVisualStyleBackColor = true;
			// 
			// ProjectManagerTasksPanel
			// 
			this.ProjectManagerTasksPanel.BackColor = System.Drawing.SystemColors.Control;
			this.ProjectManagerTasksPanel.Controls.Add(this.ProjectManagerRepositoryTreePanel);
			this.ProjectManagerTasksPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ProjectManagerTasksPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ProjectManagerTasksPanel.Location = new System.Drawing.Point(0, 0);
			this.ProjectManagerTasksPanel.Name = "ProjectManagerTasksPanel";
			this.ProjectManagerTasksPanel.Size = new System.Drawing.Size(613, 492);
			this.ProjectManagerTasksPanel.TabIndex = 5;
			// 
			// ProjectManagerRepositoryTreePanel
			// 
			this.ProjectManagerRepositoryTreePanel.Controls.Add(this.ProjectManagerTreesToolStrip);
			this.ProjectManagerRepositoryTreePanel.Controls.Add(this.ProjectManagerClassificationTreeView);
			this.ProjectManagerRepositoryTreePanel.Controls.Add(this.ProjectManagerPackageTreeView);
			this.ProjectManagerRepositoryTreePanel.Controls.Add(this.ProjectManagerArchiveTreeView);
			this.ProjectManagerRepositoryTreePanel.Controls.Add(this.ProjectManagerSyncTargetTreeView);
			this.ProjectManagerRepositoryTreePanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ProjectManagerRepositoryTreePanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ProjectManagerRepositoryTreePanel.Location = new System.Drawing.Point(0, 0);
			this.ProjectManagerRepositoryTreePanel.Name = "ProjectManagerRepositoryTreePanel";
			this.ProjectManagerRepositoryTreePanel.Size = new System.Drawing.Size(613, 492);
			this.ProjectManagerRepositoryTreePanel.TabIndex = 7;
			// 
			// ProjectManagerTreesToolStrip
			// 
			this.ProjectManagerTreesToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ProjectTreeClassificationViewToolStripButton,
            this.ProjectTreePackageViewToolStripButton,
            this.ProjectTreeSyncViewToolStripButton,
            this.ProjectTreeArchiveViewtoolStripButton,
            this.toolStripSeparator2,
            this.ProjectTreeRefreshToolStripButton,
            this.ProjectTreeCollapseToolStripButton,
            this.ProjectTreeExpandToolStripButton,
            this.ProjectTreesShowDescriptionsToolStripButton,
            this.toolStripSeparator14,
            this.ProjectTreesShowToolTipsToolStripButton});
			this.ProjectManagerTreesToolStrip.Location = new System.Drawing.Point(0, 0);
			this.ProjectManagerTreesToolStrip.Name = "ProjectManagerTreesToolStrip";
			this.ProjectManagerTreesToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.ProjectManagerTreesToolStrip.Size = new System.Drawing.Size(613, 25);
			this.ProjectManagerTreesToolStrip.TabIndex = 6;
			// 
			// ProjectTreeClassificationViewToolStripButton
			// 
			this.ProjectTreeClassificationViewToolStripButton.Checked = true;
			this.ProjectTreeClassificationViewToolStripButton.CheckOnClick = true;
			this.ProjectTreeClassificationViewToolStripButton.CheckState = System.Windows.Forms.CheckState.Checked;
			this.ProjectTreeClassificationViewToolStripButton.Image = global::MOG_Client.Properties.Resources.Classification_base_Blue_;
			this.ProjectTreeClassificationViewToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.ProjectTreeClassificationViewToolStripButton.Name = "ProjectTreeClassificationViewToolStripButton";
			this.ProjectTreeClassificationViewToolStripButton.Size = new System.Drawing.Size(114, 22);
			this.ProjectTreeClassificationViewToolStripButton.Text = "Classification View";
			this.ProjectTreeClassificationViewToolStripButton.Click += new System.EventHandler(this.ProjectTreeClassificationViewToolStripButton_Click);
			// 
			// ProjectTreePackageViewToolStripButton
			// 
			this.ProjectTreePackageViewToolStripButton.CheckOnClick = true;
			this.ProjectTreePackageViewToolStripButton.Image = global::MOG_Client.Properties.Resources.Package;
			this.ProjectTreePackageViewToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.ProjectTreePackageViewToolStripButton.Name = "ProjectTreePackageViewToolStripButton";
			this.ProjectTreePackageViewToolStripButton.Size = new System.Drawing.Size(92, 22);
			this.ProjectTreePackageViewToolStripButton.Text = "Package View";
			this.ProjectTreePackageViewToolStripButton.Click += new System.EventHandler(this.ProjectTreePackageViewToolStripButton_Click);
			// 
			// ProjectTreeSyncViewToolStripButton
			// 
			this.ProjectTreeSyncViewToolStripButton.CheckOnClick = true;
			this.ProjectTreeSyncViewToolStripButton.Image = global::MOG_Client.Properties.Resources.MOG_Sync_Options;
			this.ProjectTreeSyncViewToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.ProjectTreeSyncViewToolStripButton.Name = "ProjectTreeSyncViewToolStripButton";
			this.ProjectTreeSyncViewToolStripButton.Size = new System.Drawing.Size(75, 22);
			this.ProjectTreeSyncViewToolStripButton.Text = "Sync View";
			this.ProjectTreeSyncViewToolStripButton.Click += new System.EventHandler(this.ProjectTreeSyncViewToolStripButton_Click);
			// 
			// ProjectTreeArchiveViewtoolStripButton
			// 
			this.ProjectTreeArchiveViewtoolStripButton.CheckOnClick = true;
			this.ProjectTreeArchiveViewtoolStripButton.Image = global::MOG_Client.Properties.Resources.Archive;
			this.ProjectTreeArchiveViewtoolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.ProjectTreeArchiveViewtoolStripButton.Name = "ProjectTreeArchiveViewtoolStripButton";
			this.ProjectTreeArchiveViewtoolStripButton.Size = new System.Drawing.Size(88, 22);
			this.ProjectTreeArchiveViewtoolStripButton.Text = "Archive View";
			this.ProjectTreeArchiveViewtoolStripButton.Click += new System.EventHandler(this.ProjectTreeArchiveViewtoolStripButton_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
			// 
			// ProjectTreeRefreshToolStripButton
			// 
			this.ProjectTreeRefreshToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.ProjectTreeRefreshToolStripButton.Image = global::MOG_Client.Properties.Resources.MogRefresh;
			this.ProjectTreeRefreshToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.ProjectTreeRefreshToolStripButton.Name = "ProjectTreeRefreshToolStripButton";
			this.ProjectTreeRefreshToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.ProjectTreeRefreshToolStripButton.Text = "Refresh";
			this.ProjectTreeRefreshToolStripButton.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
			this.ProjectTreeRefreshToolStripButton.Click += new System.EventHandler(this.ProjectTreeRefreshToolStripButton_Click);
			// 
			// ProjectTreeCollapseToolStripButton
			// 
			this.ProjectTreeCollapseToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.ProjectTreeCollapseToolStripButton.Image = global::MOG_Client.Properties.Resources.ProjectTreeCollapseAll;
			this.ProjectTreeCollapseToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.ProjectTreeCollapseToolStripButton.Name = "ProjectTreeCollapseToolStripButton";
			this.ProjectTreeCollapseToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.ProjectTreeCollapseToolStripButton.Text = "Collapse selected node";
			this.ProjectTreeCollapseToolStripButton.Click += new System.EventHandler(this.ProjectTreeCollapseToolStripButton_Click);
			// 
			// ProjectTreeExpandToolStripButton
			// 
			this.ProjectTreeExpandToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.ProjectTreeExpandToolStripButton.Image = global::MOG_Client.Properties.Resources.ProjectTreeExpandAll;
			this.ProjectTreeExpandToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.ProjectTreeExpandToolStripButton.Name = "ProjectTreeExpandToolStripButton";
			this.ProjectTreeExpandToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.ProjectTreeExpandToolStripButton.Text = "Expand entire tree";
			this.ProjectTreeExpandToolStripButton.Visible = false;
			this.ProjectTreeExpandToolStripButton.Click += new System.EventHandler(this.ProjectTreeExpandToolStripButton_Click);
			// 
			// ProjectTreesShowDescriptionsToolStripButton
			// 
			this.ProjectTreesShowDescriptionsToolStripButton.CheckOnClick = true;
			this.ProjectTreesShowDescriptionsToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.ProjectTreesShowDescriptionsToolStripButton.Image = global::MOG_Client.Properties.Resources.MogComments;
			this.ProjectTreesShowDescriptionsToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.ProjectTreesShowDescriptionsToolStripButton.Name = "ProjectTreesShowDescriptionsToolStripButton";
			this.ProjectTreesShowDescriptionsToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.ProjectTreesShowDescriptionsToolStripButton.Text = "Show descriptions";
			this.ProjectTreesShowDescriptionsToolStripButton.Visible = false;
			this.ProjectTreesShowDescriptionsToolStripButton.Click += new System.EventHandler(this.ProjectTreesShowDescriptionsToolStripButton_Click);
			// 
			// toolStripSeparator14
			// 
			this.toolStripSeparator14.Name = "toolStripSeparator14";
			this.toolStripSeparator14.Size = new System.Drawing.Size(6, 25);
			this.toolStripSeparator14.Visible = false;
			// 
			// ProjectTreesShowToolTipsToolStripButton
			// 
			this.ProjectTreesShowToolTipsToolStripButton.CheckOnClick = true;
			this.ProjectTreesShowToolTipsToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.ProjectTreesShowToolTipsToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("ProjectTreesShowToolTipsToolStripButton.Image")));
			this.ProjectTreesShowToolTipsToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.ProjectTreesShowToolTipsToolStripButton.Name = "ProjectTreesShowToolTipsToolStripButton";
			this.ProjectTreesShowToolTipsToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.ProjectTreesShowToolTipsToolStripButton.Text = "Show ToolTips";
			this.ProjectTreesShowToolTipsToolStripButton.Visible = false;
			this.ProjectTreesShowToolTipsToolStripButton.Click += new System.EventHandler(this.ProjectTreesShowToolTipsToolStripButton_Click);
			// 
			// ProjectManagerClassificationTreeView
			// 
			this.ProjectManagerClassificationTreeView.AllowDrop = true;
			this.ProjectManagerClassificationTreeView.ArchivedNodeForeColor = System.Drawing.SystemColors.WindowText;
			this.ProjectManagerClassificationTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ProjectManagerClassificationTreeView.ExclusionList = "";
			this.ProjectManagerClassificationTreeView.ExpandAssets = true;
			this.ProjectManagerClassificationTreeView.ExpandPackageGroupAssets = true;
			this.ProjectManagerClassificationTreeView.ExpandPackageGroups = false;
			this.ProjectManagerClassificationTreeView.FocusForAssetNodes = MOG_ControlsLibrary.Controls.LeafFocusLevel.RepositoryItems;
			this.ProjectManagerClassificationTreeView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ProjectManagerClassificationTreeView.HotTracking = true;
			this.ProjectManagerClassificationTreeView.ImageIndex = 0;
			this.ProjectManagerClassificationTreeView.LastNodePath = null;
			this.ProjectManagerClassificationTreeView.Location = new System.Drawing.Point(0, 0);
			this.ProjectManagerClassificationTreeView.Name = "ProjectManagerClassificationTreeView";
			this.ProjectManagerClassificationTreeView.NodesDefaultToChecked = false;
			this.ProjectManagerClassificationTreeView.PathSeparator = "~";
			this.ProjectManagerClassificationTreeView.PersistantHighlightSelectedNode = false;
			this.ProjectManagerClassificationTreeView.SelectedImageIndex = 0;
			this.ProjectManagerClassificationTreeView.ShowAssets = true;
			this.ProjectManagerClassificationTreeView.ShowDescription = false;
			this.ProjectManagerClassificationTreeView.ShowPlatformSpecific = false;
			this.ProjectManagerClassificationTreeView.ShowToolTips = false;
			this.ProjectManagerClassificationTreeView.Size = new System.Drawing.Size(613, 492);
			this.ProjectManagerClassificationTreeView.TabIndex = 7;
			// 
			// ProjectManagerPackageTreeView
			// 
			this.ProjectManagerPackageTreeView.AllowItemDrag = false;
			this.ProjectManagerPackageTreeView.ArchivedNodeForeColor = System.Drawing.SystemColors.WindowText;
			this.ProjectManagerPackageTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ProjectManagerPackageTreeView.ExclusionList = "";
			this.ProjectManagerPackageTreeView.ExpandAssets = true;
			this.ProjectManagerPackageTreeView.ExpandPackageGroupAssets = true;
			this.ProjectManagerPackageTreeView.ExpandPackageGroups = false;
			this.ProjectManagerPackageTreeView.FocusForAssetNodes = MOG_ControlsLibrary.Controls.LeafFocusLevel.PackageGroup;
			this.ProjectManagerPackageTreeView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ProjectManagerPackageTreeView.HotTracking = true;
			this.ProjectManagerPackageTreeView.ImageIndex = 0;
			this.ProjectManagerPackageTreeView.LastNodePath = null;
			this.ProjectManagerPackageTreeView.Location = new System.Drawing.Point(0, 0);
			this.ProjectManagerPackageTreeView.Name = "ProjectManagerPackageTreeView";
			this.ProjectManagerPackageTreeView.NodesDefaultToChecked = false;
			this.ProjectManagerPackageTreeView.PathSeparator = "~";
			this.ProjectManagerPackageTreeView.PersistantHighlightSelectedNode = false;
			this.ProjectManagerPackageTreeView.SelectedImageIndex = 0;
			this.ProjectManagerPackageTreeView.ShowAssets = true;
			this.ProjectManagerPackageTreeView.ShowDescription = false;
			this.ProjectManagerPackageTreeView.ShowPlatformSpecific = false;
			this.ProjectManagerPackageTreeView.ShowToolTips = false;
			this.ProjectManagerPackageTreeView.Size = new System.Drawing.Size(613, 492);
			this.ProjectManagerPackageTreeView.TabIndex = 8;
			// 
			// ProjectManagerArchiveTreeView
			// 
			this.ProjectManagerArchiveTreeView.AllowDrop = true;
			this.ProjectManagerArchiveTreeView.ArchivedNodeForeColor = System.Drawing.Color.Red;
			this.ProjectManagerArchiveTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ProjectManagerArchiveTreeView.ExclusionList = "";
			this.ProjectManagerArchiveTreeView.ExpandAssets = true;
			this.ProjectManagerArchiveTreeView.ExpandPackageGroupAssets = true;
			this.ProjectManagerArchiveTreeView.ExpandPackageGroups = false;
			this.ProjectManagerArchiveTreeView.FocusForAssetNodes = MOG_ControlsLibrary.Controls.LeafFocusLevel.RepositoryItems;
			this.ProjectManagerArchiveTreeView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ProjectManagerArchiveTreeView.HotTracking = true;
			this.ProjectManagerArchiveTreeView.ImageIndex = 0;
			this.ProjectManagerArchiveTreeView.LastNodePath = null;
			this.ProjectManagerArchiveTreeView.Location = new System.Drawing.Point(0, 0);
			this.ProjectManagerArchiveTreeView.Name = "ProjectManagerArchiveTreeView";
			this.ProjectManagerArchiveTreeView.NodesDefaultToChecked = false;
			this.ProjectManagerArchiveTreeView.PathSeparator = "~";
			this.ProjectManagerArchiveTreeView.PersistantHighlightSelectedNode = false;
			this.ProjectManagerArchiveTreeView.SelectedImageIndex = 0;
			this.ProjectManagerArchiveTreeView.ShowAssets = true;
			this.ProjectManagerArchiveTreeView.ShowDescription = false;
			this.ProjectManagerArchiveTreeView.ShowPlatformSpecific = false;
			this.ProjectManagerArchiveTreeView.ShowToolTips = false;
			this.ProjectManagerArchiveTreeView.Size = new System.Drawing.Size(613, 492);
			this.ProjectManagerArchiveTreeView.TabIndex = 9;
			// 
			// ProjectManagerSyncTargetTreeView
			// 
			this.ProjectManagerSyncTargetTreeView.AllowDrop = true;
			this.ProjectManagerSyncTargetTreeView.ArchivedNodeForeColor = System.Drawing.SystemColors.WindowText;
			this.ProjectManagerSyncTargetTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ProjectManagerSyncTargetTreeView.ExclusionList = "";
			this.ProjectManagerSyncTargetTreeView.ExpandAssets = true;
			this.ProjectManagerSyncTargetTreeView.ExpandPackageGroupAssets = true;
			this.ProjectManagerSyncTargetTreeView.ExpandPackageGroups = false;
			this.ProjectManagerSyncTargetTreeView.FocusForAssetNodes = MOG_ControlsLibrary.Controls.LeafFocusLevel.RepositoryItems;
			this.ProjectManagerSyncTargetTreeView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ProjectManagerSyncTargetTreeView.HotTracking = true;
			this.ProjectManagerSyncTargetTreeView.ImageIndex = 0;
			this.ProjectManagerSyncTargetTreeView.LastNodePath = null;
			this.ProjectManagerSyncTargetTreeView.Location = new System.Drawing.Point(0, 0);
			this.ProjectManagerSyncTargetTreeView.Name = "ProjectManagerSyncTargetTreeView";
			this.ProjectManagerSyncTargetTreeView.NodesDefaultToChecked = false;
			this.ProjectManagerSyncTargetTreeView.PathSeparator = "~";
			this.ProjectManagerSyncTargetTreeView.PersistantHighlightSelectedNode = false;
			this.ProjectManagerSyncTargetTreeView.SelectedImageIndex = 0;
			this.ProjectManagerSyncTargetTreeView.ShowAssets = true;
			this.ProjectManagerSyncTargetTreeView.ShowDescription = false;
			this.ProjectManagerSyncTargetTreeView.ShowPlatformSpecific = false;
			this.ProjectManagerSyncTargetTreeView.ShowToolTips = false;
			this.ProjectManagerSyncTargetTreeView.Size = new System.Drawing.Size(613, 492);
			this.ProjectManagerSyncTargetTreeView.TabIndex = 10;
			// 
			// splitter1
			// 
			this.splitter1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
			this.splitter1.Location = new System.Drawing.Point(613, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(10, 492);
			this.splitter1.TabIndex = 10;
			this.splitter1.TabStop = false;
			// 
			// ProjectManagerTaskWindow
			// 
			this.ProjectManagerTaskWindow.Dock = System.Windows.Forms.DockStyle.Right;
			this.ProjectManagerTaskWindow.Location = new System.Drawing.Point(623, 0);
			this.ProjectManagerTaskWindow.Name = "ProjectManagerTaskWindow";
			this.ProjectManagerTaskWindow.Size = new System.Drawing.Size(339, 492);
			this.ProjectManagerTaskWindow.TabIndex = 8;
			this.ProjectManagerTaskWindow.TaskEditorEnabled = false;
			this.ProjectManagerTaskWindow.TaskListViewEnabled = false;
			this.ProjectManagerTaskWindow.Visible = false;
			// 
			// MainTabLibraryTabPage
			// 
			this.MainTabLibraryTabPage.BackColor = System.Drawing.Color.Transparent;
			this.MainTabLibraryTabPage.Controls.Add(this.LibraryExplorer);
			this.MainTabLibraryTabPage.Location = new System.Drawing.Point(4, 22);
			this.MainTabLibraryTabPage.Name = "MainTabLibraryTabPage";
			this.MainTabLibraryTabPage.Size = new System.Drawing.Size(962, 492);
			this.MainTabLibraryTabPage.TabIndex = 4;
			this.MainTabLibraryTabPage.Tag = "1";
			this.MainTabLibraryTabPage.Text = "Library";
			this.MainTabLibraryTabPage.UseVisualStyleBackColor = true;
			// 
			// LibraryExplorer
			// 
			this.LibraryExplorer.AutoImport = true;
			this.LibraryExplorer.BackColor = System.Drawing.SystemColors.Control;
			this.LibraryExplorer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.LibraryExplorer.Location = new System.Drawing.Point(0, 0);
			this.LibraryExplorer.Name = "LibraryExplorer";
			this.LibraryExplorer.Size = new System.Drawing.Size(962, 492);
			this.LibraryExplorer.TabIndex = 0;
			// 
			// MainTabConnectionManagerTabPage
			// 
			this.MainTabConnectionManagerTabPage.BackColor = System.Drawing.Color.Transparent;
			this.MainTabConnectionManagerTabPage.Controls.Add(this.ConnectionsListView);
			this.MainTabConnectionManagerTabPage.Controls.Add(this.ConnectionsLabel);
			this.MainTabConnectionManagerTabPage.Controls.Add(this.ConnectionManagerMachineCommandSplitter);
			this.MainTabConnectionManagerTabPage.Controls.Add(this.ConnectionManagerBottomPanel);
			this.MainTabConnectionManagerTabPage.Controls.Add(this.SlaveManagerSplitter);
			this.MainTabConnectionManagerTabPage.Controls.Add(this.SlaveManagerToolsPanel);
			this.MainTabConnectionManagerTabPage.ImageIndex = 2;
			this.MainTabConnectionManagerTabPage.Location = new System.Drawing.Point(4, 22);
			this.MainTabConnectionManagerTabPage.Name = "MainTabConnectionManagerTabPage";
			this.MainTabConnectionManagerTabPage.Size = new System.Drawing.Size(962, 492);
			this.MainTabConnectionManagerTabPage.TabIndex = 2;
			this.MainTabConnectionManagerTabPage.Tag = "4";
			this.MainTabConnectionManagerTabPage.Text = "Connections";
			this.MainTabConnectionManagerTabPage.ToolTipText = "Select this tab to view current connections to the server and slave command traff" +
				"ic";
			this.MainTabConnectionManagerTabPage.UseVisualStyleBackColor = true;
			// 
			// ConnectionsListView
			// 
			this.ConnectionsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.SlaveManagerMachineColumnHeader,
            this.SlaveManagerIpColumnHeader,
            this.SlaveManagerNetIdColumnHeader,
            this.SlaveManagerTypeColumnHeader,
            this.SlaveManagerInformationColumnHeader});
			this.ConnectionsListView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ConnectionsListView.FullRowSelect = true;
			this.ConnectionsListView.GridLines = true;
			this.ConnectionsListView.Location = new System.Drawing.Point(0, 64);
			this.ConnectionsListView.Name = "ConnectionsListView";
			this.ConnectionsListView.Size = new System.Drawing.Size(962, 124);
			this.ConnectionsListView.SmallImageList = this.MOGImagesImageList;
			this.ConnectionsListView.TabIndex = 1;
			this.ConnectionsListView.UseCompatibleStateImageBehavior = false;
			this.ConnectionsListView.View = System.Windows.Forms.View.Details;
			this.ConnectionsListView.SelectedIndexChanged += new System.EventHandler(this.ConnectionsListView_SelectedIndexChanged);
			// 
			// SlaveManagerMachineColumnHeader
			// 
			this.SlaveManagerMachineColumnHeader.Text = "Machine";
			this.SlaveManagerMachineColumnHeader.Width = 125;
			// 
			// SlaveManagerIpColumnHeader
			// 
			this.SlaveManagerIpColumnHeader.Text = "Ip Address";
			this.SlaveManagerIpColumnHeader.Width = 100;
			// 
			// SlaveManagerNetIdColumnHeader
			// 
			this.SlaveManagerNetIdColumnHeader.Text = "Network Id";
			this.SlaveManagerNetIdColumnHeader.Width = 66;
			// 
			// SlaveManagerTypeColumnHeader
			// 
			this.SlaveManagerTypeColumnHeader.Text = "Type";
			this.SlaveManagerTypeColumnHeader.Width = 100;
			// 
			// SlaveManagerInformationColumnHeader
			// 
			this.SlaveManagerInformationColumnHeader.Text = "Information";
			this.SlaveManagerInformationColumnHeader.Width = 450;
			// 
			// ConnectionsLabel
			// 
			this.ConnectionsLabel.Dock = System.Windows.Forms.DockStyle.Top;
			this.ConnectionsLabel.Location = new System.Drawing.Point(0, 45);
			this.ConnectionsLabel.Name = "ConnectionsLabel";
			this.ConnectionsLabel.Size = new System.Drawing.Size(962, 19);
			this.ConnectionsLabel.TabIndex = 9;
			this.ConnectionsLabel.Text = "Current active connections:";
			// 
			// ConnectionManagerMachineCommandSplitter
			// 
			this.ConnectionManagerMachineCommandSplitter.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.ConnectionManagerMachineCommandSplitter.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.ConnectionManagerMachineCommandSplitter.Location = new System.Drawing.Point(0, 188);
			this.ConnectionManagerMachineCommandSplitter.Name = "ConnectionManagerMachineCommandSplitter";
			this.ConnectionManagerMachineCommandSplitter.Size = new System.Drawing.Size(962, 8);
			this.ConnectionManagerMachineCommandSplitter.TabIndex = 8;
			this.ConnectionManagerMachineCommandSplitter.TabStop = false;
			// 
			// ConnectionManagerBottomPanel
			// 
			this.ConnectionManagerBottomPanel.Controls.Add(this.ConnectionsBottomTabControl);
			this.ConnectionManagerBottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.ConnectionManagerBottomPanel.Location = new System.Drawing.Point(0, 196);
			this.ConnectionManagerBottomPanel.Name = "ConnectionManagerBottomPanel";
			this.ConnectionManagerBottomPanel.Size = new System.Drawing.Size(962, 296);
			this.ConnectionManagerBottomPanel.TabIndex = 7;
			// 
			// ConnectionsBottomTabControl
			// 
			this.ConnectionsBottomTabControl.Controls.Add(this.ConnectionsPendingCommandsTabPage);
			this.ConnectionsBottomTabControl.Controls.Add(this.ConnectionsPendingPackageTabPage);
			this.ConnectionsBottomTabControl.Controls.Add(this.ConnectionsPendingPostTabPage);
			this.ConnectionsBottomTabControl.Controls.Add(this.ConnectionsLateResolversTabPage);
			this.ConnectionsBottomTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ConnectionsBottomTabControl.Location = new System.Drawing.Point(0, 0);
			this.ConnectionsBottomTabControl.Name = "ConnectionsBottomTabControl";
			this.ConnectionsBottomTabControl.SelectedIndex = 0;
			this.ConnectionsBottomTabControl.ShowToolTips = true;
			this.ConnectionsBottomTabControl.Size = new System.Drawing.Size(962, 296);
			this.ConnectionsBottomTabControl.TabIndex = 12;
			// 
			// ConnectionsPendingCommandsTabPage
			// 
			this.ConnectionsPendingCommandsTabPage.Controls.Add(this.ConnectionManagerCommandsListView);
			this.ConnectionsPendingCommandsTabPage.Location = new System.Drawing.Point(4, 22);
			this.ConnectionsPendingCommandsTabPage.Name = "ConnectionsPendingCommandsTabPage";
			this.ConnectionsPendingCommandsTabPage.Padding = new System.Windows.Forms.Padding(3);
			this.ConnectionsPendingCommandsTabPage.Size = new System.Drawing.Size(954, 270);
			this.ConnectionsPendingCommandsTabPage.TabIndex = 0;
			this.ConnectionsPendingCommandsTabPage.Text = "Pending Commands";
			this.ConnectionsPendingCommandsTabPage.ToolTipText = "Commands pending execution by slaves";
			this.ConnectionsPendingCommandsTabPage.UseVisualStyleBackColor = true;
			// 
			// ConnectionManagerCommandsListView
			// 
			this.ConnectionManagerCommandsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ConnectionsCommandColumnHeader,
            this.ConnectionsNameColumnHeader,
            this.ConnectionsPlatformColumnHeader,
            this.ConnectionsSlaveColumnHeader,
            this.ConnectionsJobLabelColumnHeader,
            this.ConnectionsMachineColumnHeader,
            this.ConnectionsIpColumnHeader,
            this.ConnectionsIdColumnHeader});
			this.ConnectionManagerCommandsListView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ConnectionManagerCommandsListView.FullRowSelect = true;
			this.ConnectionManagerCommandsListView.Location = new System.Drawing.Point(3, 3);
			this.ConnectionManagerCommandsListView.Name = "ConnectionManagerCommandsListView";
			this.ConnectionManagerCommandsListView.Size = new System.Drawing.Size(948, 264);
			this.ConnectionManagerCommandsListView.SmallImageList = this.CommandImageList;
			this.ConnectionManagerCommandsListView.TabIndex = 6;
			this.ConnectionManagerCommandsListView.UseCompatibleStateImageBehavior = false;
			this.ConnectionManagerCommandsListView.View = System.Windows.Forms.View.Details;
			this.ConnectionManagerCommandsListView.SelectedIndexChanged += new System.EventHandler(this.ConnectionManagerCommandsListView_SelectedIndexChanged);
			// 
			// ConnectionsCommandColumnHeader
			// 
			this.ConnectionsCommandColumnHeader.Text = "Command Type";
			this.ConnectionsCommandColumnHeader.Width = 200;
			// 
			// ConnectionsNameColumnHeader
			// 
			this.ConnectionsNameColumnHeader.Text = "Asset Name";
			this.ConnectionsNameColumnHeader.Width = 400;
			// 
			// ConnectionsPlatformColumnHeader
			// 
			this.ConnectionsPlatformColumnHeader.Text = "Platform";
			this.ConnectionsPlatformColumnHeader.Width = 75;
			// 
			// ConnectionsSlaveColumnHeader
			// 
			this.ConnectionsSlaveColumnHeader.Text = "Slave";
			this.ConnectionsSlaveColumnHeader.Width = 75;
			// 
			// ConnectionsJobLabelColumnHeader
			// 
			this.ConnectionsJobLabelColumnHeader.Text = "Job Label";
			this.ConnectionsJobLabelColumnHeader.Width = 200;
			// 
			// ConnectionsMachineColumnHeader
			// 
			this.ConnectionsMachineColumnHeader.Text = "Machine";
			this.ConnectionsMachineColumnHeader.Width = 75;
			// 
			// ConnectionsIpColumnHeader
			// 
			this.ConnectionsIpColumnHeader.Text = "Ip";
			this.ConnectionsIpColumnHeader.Width = 85;
			// 
			// ConnectionsIdColumnHeader
			// 
			this.ConnectionsIdColumnHeader.Text = "Id";
			this.ConnectionsIdColumnHeader.Width = 75;
			// 
			// CommandImageList
			// 
			this.CommandImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("CommandImageList.ImageStream")));
			this.CommandImageList.TransparentColor = System.Drawing.Color.Transparent;
			this.CommandImageList.Images.SetKeyName(0, "");
			this.CommandImageList.Images.SetKeyName(1, "");
			this.CommandImageList.Images.SetKeyName(2, "");
			this.CommandImageList.Images.SetKeyName(3, "");
			this.CommandImageList.Images.SetKeyName(4, "");
			this.CommandImageList.Images.SetKeyName(5, "");
			this.CommandImageList.Images.SetKeyName(6, "");
			// 
			// ConnectionsPendingPackageTabPage
			// 
			this.ConnectionsPendingPackageTabPage.Controls.Add(this.ConnectionManagerMergingListView);
			this.ConnectionsPendingPackageTabPage.Location = new System.Drawing.Point(4, 22);
			this.ConnectionsPendingPackageTabPage.Name = "ConnectionsPendingPackageTabPage";
			this.ConnectionsPendingPackageTabPage.Padding = new System.Windows.Forms.Padding(3);
			this.ConnectionsPendingPackageTabPage.Size = new System.Drawing.Size(954, 270);
			this.ConnectionsPendingPackageTabPage.TabIndex = 1;
			this.ConnectionsPendingPackageTabPage.Text = "Pending Package";
			this.ConnectionsPendingPackageTabPage.ToolTipText = "Assets pending to be packaged";
			this.ConnectionsPendingPackageTabPage.UseVisualStyleBackColor = true;
			// 
			// ConnectionManagerMergingListView
			// 
			this.ConnectionManagerMergingListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ConnectionManagerMergeNameColumnHeader,
            this.ConnectionManagerClassColumnHeader,
            this.ConnectionManagerVersionColumnHeader,
            this.ConnectionManagerMergeOwnerColumnHeader,
            this.ConnectionManagerMergeDateColumnHeader});
			this.ConnectionManagerMergingListView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ConnectionManagerMergingListView.FullRowSelect = true;
			this.ConnectionManagerMergingListView.Location = new System.Drawing.Point(3, 3);
			this.ConnectionManagerMergingListView.Name = "ConnectionManagerMergingListView";
			this.ConnectionManagerMergingListView.Size = new System.Drawing.Size(948, 264);
			this.ConnectionManagerMergingListView.SmallImageList = this.MOGImagesImageList;
			this.ConnectionManagerMergingListView.TabIndex = 7;
			this.ConnectionManagerMergingListView.UseCompatibleStateImageBehavior = false;
			this.ConnectionManagerMergingListView.View = System.Windows.Forms.View.Details;
			this.ConnectionManagerMergingListView.SelectedIndexChanged += new System.EventHandler(this.ConnectionManagerMergingListView_SelectedIndexChanged);
			// 
			// ConnectionManagerMergeNameColumnHeader
			// 
			this.ConnectionManagerMergeNameColumnHeader.Text = "Name";
			this.ConnectionManagerMergeNameColumnHeader.Width = 174;
			// 
			// ConnectionManagerClassColumnHeader
			// 
			this.ConnectionManagerClassColumnHeader.Text = "Classification";
			this.ConnectionManagerClassColumnHeader.Width = 214;
			// 
			// ConnectionManagerVersionColumnHeader
			// 
			this.ConnectionManagerVersionColumnHeader.Text = "Date";
			this.ConnectionManagerVersionColumnHeader.Width = 168;
			// 
			// ConnectionManagerMergeOwnerColumnHeader
			// 
			this.ConnectionManagerMergeOwnerColumnHeader.Text = "Target Package";
			this.ConnectionManagerMergeOwnerColumnHeader.Width = 312;
			// 
			// ConnectionManagerMergeDateColumnHeader
			// 
			this.ConnectionManagerMergeDateColumnHeader.Text = "User";
			this.ConnectionManagerMergeDateColumnHeader.Width = 90;
			// 
			// ConnectionsPendingPostTabPage
			// 
			this.ConnectionsPendingPostTabPage.Controls.Add(this.ConnectionManagerPostingListView);
			this.ConnectionsPendingPostTabPage.Location = new System.Drawing.Point(4, 22);
			this.ConnectionsPendingPostTabPage.Name = "ConnectionsPendingPostTabPage";
			this.ConnectionsPendingPostTabPage.Padding = new System.Windows.Forms.Padding(3);
			this.ConnectionsPendingPostTabPage.Size = new System.Drawing.Size(954, 270);
			this.ConnectionsPendingPostTabPage.TabIndex = 2;
			this.ConnectionsPendingPostTabPage.Text = "Pending Post";
			this.ConnectionsPendingPostTabPage.ToolTipText = "Assets pending to be posted to MasterData";
			this.ConnectionsPendingPostTabPage.UseVisualStyleBackColor = true;
			// 
			// ConnectionManagerPostingListView
			// 
			this.ConnectionManagerPostingListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ConnectionManagerPostNameColumnHeader,
            this.ConnectionManagerPostClassColumnHeader,
            this.ConnectionManagerPostDateColumnHeader,
            this.ConnectionManagerPostOwnerColumnHeader});
			this.ConnectionManagerPostingListView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ConnectionManagerPostingListView.FullRowSelect = true;
			this.ConnectionManagerPostingListView.Location = new System.Drawing.Point(3, 3);
			this.ConnectionManagerPostingListView.Name = "ConnectionManagerPostingListView";
			this.ConnectionManagerPostingListView.Size = new System.Drawing.Size(948, 264);
			this.ConnectionManagerPostingListView.SmallImageList = this.MOGImagesImageList;
			this.ConnectionManagerPostingListView.TabIndex = 8;
			this.ConnectionManagerPostingListView.UseCompatibleStateImageBehavior = false;
			this.ConnectionManagerPostingListView.View = System.Windows.Forms.View.Details;
			this.ConnectionManagerPostingListView.SelectedIndexChanged += new System.EventHandler(this.ConnectionManagerPostingListView_SelectedIndexChanged);
			// 
			// ConnectionManagerPostNameColumnHeader
			// 
			this.ConnectionManagerPostNameColumnHeader.Text = "Name";
			this.ConnectionManagerPostNameColumnHeader.Width = 242;
			// 
			// ConnectionManagerPostClassColumnHeader
			// 
			this.ConnectionManagerPostClassColumnHeader.Text = "Classification";
			this.ConnectionManagerPostClassColumnHeader.Width = 436;
			// 
			// ConnectionManagerPostDateColumnHeader
			// 
			this.ConnectionManagerPostDateColumnHeader.Text = "Date";
			this.ConnectionManagerPostDateColumnHeader.Width = 169;
			// 
			// ConnectionManagerPostOwnerColumnHeader
			// 
			this.ConnectionManagerPostOwnerColumnHeader.Text = "Owner";
			this.ConnectionManagerPostOwnerColumnHeader.Width = 113;
			// 
			// ConnectionsLateResolversTabPage
			// 
			this.ConnectionsLateResolversTabPage.Controls.Add(this.ConnectionManagerLateResolversListView);
			this.ConnectionsLateResolversTabPage.Location = new System.Drawing.Point(4, 22);
			this.ConnectionsLateResolversTabPage.Name = "ConnectionsLateResolversTabPage";
			this.ConnectionsLateResolversTabPage.Size = new System.Drawing.Size(954, 270);
			this.ConnectionsLateResolversTabPage.TabIndex = 3;
			this.ConnectionsLateResolversTabPage.Text = "Late Resolvers";
			this.ConnectionsLateResolversTabPage.ToolTipText = "Known package-to-package missing asset references";
			this.ConnectionsLateResolversTabPage.UseVisualStyleBackColor = true;
			// 
			// ConnectionManagerLateResolversListView
			// 
			this.ConnectionManagerLateResolversListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader28,
            this.columnHeader29,
            this.columnHeader30,
            this.columnHeader31,
            this.columnHeader32});
			this.ConnectionManagerLateResolversListView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ConnectionManagerLateResolversListView.FullRowSelect = true;
			this.ConnectionManagerLateResolversListView.Location = new System.Drawing.Point(0, 0);
			this.ConnectionManagerLateResolversListView.Name = "ConnectionManagerLateResolversListView";
			this.ConnectionManagerLateResolversListView.Size = new System.Drawing.Size(954, 270);
			this.ConnectionManagerLateResolversListView.SmallImageList = this.MOGImagesImageList;
			this.ConnectionManagerLateResolversListView.TabIndex = 8;
			this.ConnectionManagerLateResolversListView.UseCompatibleStateImageBehavior = false;
			this.ConnectionManagerLateResolversListView.View = System.Windows.Forms.View.Details;
			this.ConnectionManagerLateResolversListView.SelectedIndexChanged += new System.EventHandler(this.ConnectionManagerLateResolversListView_SelectedIndexChanged);
			// 
			// columnHeader28
			// 
			this.columnHeader28.Text = "Broken Package";
			this.columnHeader28.Width = 174;
			// 
			// columnHeader29
			// 
			this.columnHeader29.Text = "Classification";
			this.columnHeader29.Width = 214;
			// 
			// columnHeader30
			// 
			this.columnHeader30.Text = "Date";
			this.columnHeader30.Width = 168;
			// 
			// columnHeader31
			// 
			this.columnHeader31.Text = "Dependant Package";
			this.columnHeader31.Width = 312;
			// 
			// columnHeader32
			// 
			this.columnHeader32.Text = "User";
			this.columnHeader32.Width = 90;
			// 
			// SlaveManagerSplitter
			// 
			this.SlaveManagerSplitter.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.SlaveManagerSplitter.Dock = System.Windows.Forms.DockStyle.Top;
			this.SlaveManagerSplitter.Location = new System.Drawing.Point(0, 40);
			this.SlaveManagerSplitter.Name = "SlaveManagerSplitter";
			this.SlaveManagerSplitter.Size = new System.Drawing.Size(962, 5);
			this.SlaveManagerSplitter.TabIndex = 5;
			this.SlaveManagerSplitter.TabStop = false;
			// 
			// SlaveManagerToolsPanel
			// 
			this.SlaveManagerToolsPanel.BackColor = System.Drawing.SystemColors.Control;
			this.SlaveManagerToolsPanel.Controls.Add(this.SlaveManagerOnlyMySlaveCheckBox);
			this.SlaveManagerToolsPanel.Controls.Add(this.SlaveManagerDeleteSlaveButton);
			this.SlaveManagerToolsPanel.Controls.Add(this.SlaveManagerAddSlaveButton);
			this.SlaveManagerToolsPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.SlaveManagerToolsPanel.Location = new System.Drawing.Point(0, 0);
			this.SlaveManagerToolsPanel.Name = "SlaveManagerToolsPanel";
			this.SlaveManagerToolsPanel.Size = new System.Drawing.Size(962, 40);
			this.SlaveManagerToolsPanel.TabIndex = 4;
			// 
			// SlaveManagerOnlyMySlaveCheckBox
			// 
			this.SlaveManagerOnlyMySlaveCheckBox.Location = new System.Drawing.Point(112, 8);
			this.SlaveManagerOnlyMySlaveCheckBox.Name = "SlaveManagerOnlyMySlaveCheckBox";
			this.SlaveManagerOnlyMySlaveCheckBox.Size = new System.Drawing.Size(264, 24);
			this.SlaveManagerOnlyMySlaveCheckBox.TabIndex = 5;
			this.SlaveManagerOnlyMySlaveCheckBox.Text = "Local processing - Only use my slave";
			this.SlaveManagerOnlyMySlaveCheckBox.CheckedChanged += new System.EventHandler(this.SlaveManagerOnlyMySlaveCheckBox_CheckedChanged);
			// 
			// SlaveManagerDeleteSlaveButton
			// 
			this.SlaveManagerDeleteSlaveButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.SlaveManagerDeleteSlaveButton.Location = new System.Drawing.Point(64, 8);
			this.SlaveManagerDeleteSlaveButton.Name = "SlaveManagerDeleteSlaveButton";
			this.SlaveManagerDeleteSlaveButton.Size = new System.Drawing.Size(40, 23);
			this.SlaveManagerDeleteSlaveButton.TabIndex = 4;
			this.SlaveManagerDeleteSlaveButton.Text = "Del";
			this.SlaveManagerDeleteSlaveButton.Click += new System.EventHandler(this.SlaveManagerDeleteSlaveButton_Click);
			// 
			// SlaveManagerAddSlaveButton
			// 
			this.SlaveManagerAddSlaveButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.SlaveManagerAddSlaveButton.Location = new System.Drawing.Point(16, 8);
			this.SlaveManagerAddSlaveButton.Name = "SlaveManagerAddSlaveButton";
			this.SlaveManagerAddSlaveButton.Size = new System.Drawing.Size(40, 23);
			this.SlaveManagerAddSlaveButton.TabIndex = 3;
			this.SlaveManagerAddSlaveButton.Text = "Add";
			this.SlaveManagerAddSlaveButton.Click += new System.EventHandler(this.SlaveManagerAddSlaveButton_Click);
			// 
			// MainTabLockManagerTabPage
			// 
			this.MainTabLockManagerTabPage.BackColor = System.Drawing.Color.Transparent;
			this.MainTabLockManagerTabPage.Controls.Add(this.LockManagerLocksListView);
			this.MainTabLockManagerTabPage.Controls.Add(this.label1);
			this.MainTabLockManagerTabPage.Controls.Add(this.LockManagerSplitter);
			this.MainTabLockManagerTabPage.Controls.Add(this.LockManagerToolsPanel);
			this.MainTabLockManagerTabPage.Controls.Add(this.LockManagerToolStrip);
			this.MainTabLockManagerTabPage.Controls.Add(this.LockManagerPendingSplitter);
			this.MainTabLockManagerTabPage.Controls.Add(this.label2);
			this.MainTabLockManagerTabPage.Controls.Add(this.LockManagerPendingListView);
			this.MainTabLockManagerTabPage.ImageIndex = 3;
			this.MainTabLockManagerTabPage.Location = new System.Drawing.Point(4, 22);
			this.MainTabLockManagerTabPage.Name = "MainTabLockManagerTabPage";
			this.MainTabLockManagerTabPage.Size = new System.Drawing.Size(962, 492);
			this.MainTabLockManagerTabPage.TabIndex = 3;
			this.MainTabLockManagerTabPage.Tag = "5";
			this.MainTabLockManagerTabPage.Text = "Locks";
			this.MainTabLockManagerTabPage.ToolTipText = "Select this tab to view persistant and roaming file locks";
			this.MainTabLockManagerTabPage.UseVisualStyleBackColor = true;
			// 
			// LockManagerLocksListView
			// 
			this.LockManagerLocksListView.AllowColumnReorder = true;
			this.LockManagerLocksListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.LabelColumn,
            this.ClassificationColumn,
            this.UserColumn,
            this.DescriptionColumn,
            this.MachineColumn,
            this.IpColumn,
            this.NetIdColumn,
            this.TimeColumn,
            this.AssetFullNameColumn,
            this.TypeColumn});
			this.LockManagerLocksListView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.LockManagerLocksListView.FullRowSelect = true;
			this.LockManagerLocksListView.Location = new System.Drawing.Point(0, 55);
			this.LockManagerLocksListView.Name = "LockManagerLocksListView";
			this.LockManagerLocksListView.Size = new System.Drawing.Size(962, 238);
			this.LockManagerLocksListView.TabIndex = 6;
			this.LockManagerLocksListView.UseCompatibleStateImageBehavior = false;
			this.LockManagerLocksListView.View = System.Windows.Forms.View.Details;
			// 
			// LabelColumn
			// 
			this.LabelColumn.Text = "Filename";
			this.LabelColumn.Width = 250;
			// 
			// ClassificationColumn
			// 
			this.ClassificationColumn.Text = "Classification";
			this.ClassificationColumn.Width = 400;
			// 
			// UserColumn
			// 
			this.UserColumn.Text = "User";
			this.UserColumn.Width = 75;
			// 
			// DescriptionColumn
			// 
			this.DescriptionColumn.Text = "Description";
			this.DescriptionColumn.Width = 300;
			// 
			// MachineColumn
			// 
			this.MachineColumn.Text = "Machine";
			this.MachineColumn.Width = 0;
			// 
			// IpColumn
			// 
			this.IpColumn.Text = "Ip";
			this.IpColumn.Width = 0;
			// 
			// NetIdColumn
			// 
			this.NetIdColumn.Text = "Net Id";
			this.NetIdColumn.Width = 0;
			// 
			// TimeColumn
			// 
			this.TimeColumn.Text = "Time";
			this.TimeColumn.Width = 0;
			// 
			// AssetFullNameColumn
			// 
			this.AssetFullNameColumn.Text = "Fullname";
			this.AssetFullNameColumn.Width = 0;
			// 
			// TypeColumn
			// 
			this.TypeColumn.Text = "Type";
			this.TypeColumn.Width = 0;
			// 
			// label1
			// 
			this.label1.Dock = System.Windows.Forms.DockStyle.Top;
			this.label1.Location = new System.Drawing.Point(0, 39);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(962, 16);
			this.label1.TabIndex = 8;
			this.label1.Text = "Active Locks";
			// 
			// LockManagerSplitter
			// 
			this.LockManagerSplitter.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.LockManagerSplitter.Dock = System.Windows.Forms.DockStyle.Top;
			this.LockManagerSplitter.Location = new System.Drawing.Point(0, 34);
			this.LockManagerSplitter.Name = "LockManagerSplitter";
			this.LockManagerSplitter.Size = new System.Drawing.Size(962, 5);
			this.LockManagerSplitter.TabIndex = 3;
			this.LockManagerSplitter.TabStop = false;
			// 
			// LockManagerToolsPanel
			// 
			this.LockManagerToolsPanel.BackColor = System.Drawing.SystemColors.Control;
			this.LockManagerToolsPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.LockManagerToolsPanel.Location = new System.Drawing.Point(0, 0);
			this.LockManagerToolsPanel.Name = "LockManagerToolsPanel";
			this.LockManagerToolsPanel.Size = new System.Drawing.Size(962, 34);
			this.LockManagerToolsPanel.TabIndex = 2;
			this.LockManagerToolsPanel.Visible = false;
			// 
			// LockManagerToolStrip
			// 
			this.LockManagerToolStrip.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.LockManagerToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.LockManagerToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LockManagerToggleFilterToolStripButton,
            this.toolStripSeparator1,
            this.LockManagerLabelToolStripLabel,
            this.LockManagerFilterToolStripTextBox});
			this.LockManagerToolStrip.Location = new System.Drawing.Point(0, 293);
			this.LockManagerToolStrip.Name = "LockManagerToolStrip";
			this.LockManagerToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.LockManagerToolStrip.Size = new System.Drawing.Size(962, 25);
			this.LockManagerToolStrip.TabIndex = 0;
			this.LockManagerToolStrip.Text = "toolStrip1";
			// 
			// LockManagerToggleFilterToolStripButton
			// 
			this.LockManagerToggleFilterToolStripButton.Checked = true;
			this.LockManagerToggleFilterToolStripButton.CheckOnClick = true;
			this.LockManagerToggleFilterToolStripButton.CheckState = System.Windows.Forms.CheckState.Checked;
			this.LockManagerToggleFilterToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.LockManagerToggleFilterToolStripButton.Image = global::MOG_Client.Properties.Resources.Mog_Filter_button;
			this.LockManagerToggleFilterToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.LockManagerToggleFilterToolStripButton.Name = "LockManagerToggleFilterToolStripButton";
			this.LockManagerToggleFilterToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.LockManagerToggleFilterToolStripButton.Text = "Filter locks by the filter string";
			this.LockManagerToggleFilterToolStripButton.Click += new System.EventHandler(this.LockManagerToggleFilterToolStripButton_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// LockManagerLabelToolStripLabel
			// 
			this.LockManagerLabelToolStripLabel.Name = "LockManagerLabelToolStripLabel";
			this.LockManagerLabelToolStripLabel.Size = new System.Drawing.Size(61, 22);
			this.LockManagerLabelToolStripLabel.Text = "Filter string";
			// 
			// LockManagerFilterToolStripTextBox
			// 
			this.LockManagerFilterToolStripTextBox.Name = "LockManagerFilterToolStripTextBox";
			this.LockManagerFilterToolStripTextBox.Size = new System.Drawing.Size(350, 25);
			this.LockManagerFilterToolStripTextBox.TextChanged += new System.EventHandler(this.LockManagerFilterToolStripTextBox_TextChanged);
			// 
			// LockManagerPendingSplitter
			// 
			this.LockManagerPendingSplitter.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.LockManagerPendingSplitter.Location = new System.Drawing.Point(0, 318);
			this.LockManagerPendingSplitter.Name = "LockManagerPendingSplitter";
			this.LockManagerPendingSplitter.Size = new System.Drawing.Size(962, 8);
			this.LockManagerPendingSplitter.TabIndex = 7;
			this.LockManagerPendingSplitter.TabStop = false;
			// 
			// label2
			// 
			this.label2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.label2.Location = new System.Drawing.Point(0, 326);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(962, 16);
			this.label2.TabIndex = 9;
			this.label2.Text = "Pending Locks";
			// 
			// LockManagerPendingListView
			// 
			this.LockManagerPendingListView.AllowColumnReorder = true;
			this.LockManagerPendingListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colLabel,
            this.colClassification,
            this.colUser,
            this.colDescription,
            this.colMachine,
            this.colIp,
            this.colNetId,
            this.colTime,
            this.colAssetFullname,
            this.colType});
			this.LockManagerPendingListView.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.LockManagerPendingListView.FullRowSelect = true;
			this.LockManagerPendingListView.Location = new System.Drawing.Point(0, 342);
			this.LockManagerPendingListView.Name = "LockManagerPendingListView";
			this.LockManagerPendingListView.Size = new System.Drawing.Size(962, 150);
			this.LockManagerPendingListView.TabIndex = 5;
			this.LockManagerPendingListView.UseCompatibleStateImageBehavior = false;
			this.LockManagerPendingListView.View = System.Windows.Forms.View.Details;
			// 
			// colLabel
			// 
			this.colLabel.Text = "Filename";
			this.colLabel.Width = 250;
			// 
			// colClassification
			// 
			this.colClassification.Text = "Classification";
			this.colClassification.Width = 400;
			// 
			// colUser
			// 
			this.colUser.Text = "User";
			this.colUser.Width = 75;
			// 
			// colDescription
			// 
			this.colDescription.Text = "Description";
			this.colDescription.Width = 300;
			// 
			// colMachine
			// 
			this.colMachine.Text = "Machine";
			this.colMachine.Width = 0;
			// 
			// colIp
			// 
			this.colIp.Text = "Ip";
			this.colIp.Width = 0;
			// 
			// colNetId
			// 
			this.colNetId.Text = "Net Id";
			this.colNetId.Width = 0;
			// 
			// colTime
			// 
			this.colTime.Text = "Time";
			this.colTime.Width = 0;
			// 
			// colAssetFullname
			// 
			this.colAssetFullname.Text = "AssetFullname";
			this.colAssetFullname.Width = 0;
			// 
			// colType
			// 
			this.colType.Text = "Type";
			this.colType.Width = 0;
			// 
			// MainMogTreeContextMenuStrip
			// 
			this.MainMogTreeContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hideToolStripMenuItem,
            this.toolStripMenuItem1,
            this.TabPageShowToolStripMenuItem});
			this.MainMogTreeContextMenuStrip.Name = "MainMogTreeContextMenuStrip";
			this.MainMogTreeContextMenuStrip.Size = new System.Drawing.Size(112, 54);
			// 
			// hideToolStripMenuItem
			// 
			this.hideToolStripMenuItem.Name = "hideToolStripMenuItem";
			this.hideToolStripMenuItem.Size = new System.Drawing.Size(111, 22);
			this.hideToolStripMenuItem.Text = "Hide";
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(108, 6);
			// 
			// TabPageShowToolStripMenuItem
			// 
			this.TabPageShowToolStripMenuItem.Name = "TabPageShowToolStripMenuItem";
			this.TabPageShowToolStripMenuItem.Size = new System.Drawing.Size(111, 22);
			this.TabPageShowToolStripMenuItem.Text = "Show";
			// 
			// MainTabPagesImageList
			// 
			this.MainTabPagesImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("MainTabPagesImageList.ImageStream")));
			this.MainTabPagesImageList.TransparentColor = System.Drawing.Color.Transparent;
			this.MainTabPagesImageList.Images.SetKeyName(0, "");
			this.MainTabPagesImageList.Images.SetKeyName(1, "");
			this.MainTabPagesImageList.Images.SetKeyName(2, "");
			this.MainTabPagesImageList.Images.SetKeyName(3, "");
			this.MainTabPagesImageList.Images.SetKeyName(4, "");
			this.MainTabPagesImageList.Images.SetKeyName(5, "");
			this.MainTabPagesImageList.Images.SetKeyName(6, "");
			this.MainTabPagesImageList.Images.SetKeyName(7, "");
			this.MainTabPagesImageList.Images.SetKeyName(8, "");
			this.MainTabPagesImageList.Images.SetKeyName(9, "");
			this.MainTabPagesImageList.Images.SetKeyName(10, "");
			// 
			// ProjectManagerExplorerImageList
			// 
			this.ProjectManagerExplorerImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ProjectManagerExplorerImageList.ImageStream")));
			this.ProjectManagerExplorerImageList.TransparentColor = System.Drawing.Color.Magenta;
			this.ProjectManagerExplorerImageList.Images.SetKeyName(0, "");
			this.ProjectManagerExplorerImageList.Images.SetKeyName(1, "");
			this.ProjectManagerExplorerImageList.Images.SetKeyName(2, "");
			// 
			// AssetManagerImageList
			// 
			this.AssetManagerImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("AssetManagerImageList.ImageStream")));
			this.AssetManagerImageList.TransparentColor = System.Drawing.Color.Transparent;
			this.AssetManagerImageList.Images.SetKeyName(0, "");
			this.AssetManagerImageList.Images.SetKeyName(1, "");
			this.AssetManagerImageList.Images.SetKeyName(2, "");
			this.AssetManagerImageList.Images.SetKeyName(3, "");
			this.AssetManagerImageList.Images.SetKeyName(4, "");
			// 
			// MOGTasksImageList
			// 
			this.MOGTasksImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("MOGTasksImageList.ImageStream")));
			this.MOGTasksImageList.TransparentColor = System.Drawing.Color.Transparent;
			this.MOGTasksImageList.Images.SetKeyName(0, "");
			this.MOGTasksImageList.Images.SetKeyName(1, "");
			this.MOGTasksImageList.Images.SetKeyName(2, "");
			this.MOGTasksImageList.Images.SetKeyName(3, "");
			this.MOGTasksImageList.Images.SetKeyName(4, "");
			this.MOGTasksImageList.Images.SetKeyName(5, "");
			this.MOGTasksImageList.Images.SetKeyName(6, "");
			this.MOGTasksImageList.Images.SetKeyName(7, "");
			this.MOGTasksImageList.Images.SetKeyName(8, "");
			// 
			// AssetArchiveToolbarImageList
			// 
			this.AssetArchiveToolbarImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("AssetArchiveToolbarImageList.ImageStream")));
			this.AssetArchiveToolbarImageList.TransparentColor = System.Drawing.Color.Magenta;
			this.AssetArchiveToolbarImageList.Images.SetKeyName(0, "");
			this.AssetArchiveToolbarImageList.Images.SetKeyName(1, "");
			this.AssetArchiveToolbarImageList.Images.SetKeyName(2, "");
			this.AssetArchiveToolbarImageList.Images.SetKeyName(3, "");
			this.AssetArchiveToolbarImageList.Images.SetKeyName(4, "");
			// 
			// AssetArchiveAssetsImageList
			// 
			this.AssetArchiveAssetsImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("AssetArchiveAssetsImageList.ImageStream")));
			this.AssetArchiveAssetsImageList.TransparentColor = System.Drawing.Color.Transparent;
			this.AssetArchiveAssetsImageList.Images.SetKeyName(0, "");
			this.AssetArchiveAssetsImageList.Images.SetKeyName(1, "");
			this.AssetArchiveAssetsImageList.Images.SetKeyName(2, "");
			this.AssetArchiveAssetsImageList.Images.SetKeyName(3, "");
			this.AssetArchiveAssetsImageList.Images.SetKeyName(4, "");
			this.AssetArchiveAssetsImageList.Images.SetKeyName(5, "");
			// 
			// MOGGlobalMainMenuImageList
			// 
			this.MOGGlobalMainMenuImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("MOGGlobalMainMenuImageList.ImageStream")));
			this.MOGGlobalMainMenuImageList.TransparentColor = System.Drawing.Color.Magenta;
			this.MOGGlobalMainMenuImageList.Images.SetKeyName(0, "");
			this.MOGGlobalMainMenuImageList.Images.SetKeyName(1, "");
			this.MOGGlobalMainMenuImageList.Images.SetKeyName(2, "");
			this.MOGGlobalMainMenuImageList.Images.SetKeyName(3, "");
			this.MOGGlobalMainMenuImageList.Images.SetKeyName(4, "");
			this.MOGGlobalMainMenuImageList.Images.SetKeyName(5, "");
			this.MOGGlobalMainMenuImageList.Images.SetKeyName(6, "");
			this.MOGGlobalMainMenuImageList.Images.SetKeyName(7, "");
			this.MOGGlobalMainMenuImageList.Images.SetKeyName(8, "");
			this.MOGGlobalMainMenuImageList.Images.SetKeyName(9, "");
			// 
			// AssetArchiveSendToMenuItem
			// 
			this.AssetArchiveSendToMenuItem.Index = -1;
			this.AssetArchiveSendToMenuItem.Text = "";
			// 
			// AssetArchiveUpdateMenuItem
			// 
			this.AssetArchiveUpdateMenuItem.Index = -1;
			this.AssetArchiveUpdateMenuItem.Text = "";
			// 
			// AssetArchiveRenameMenuItem
			// 
			this.AssetArchiveRenameMenuItem.Index = -1;
			this.AssetArchiveRenameMenuItem.Text = "";
			// 
			// AssetArchiveRebuildPackageMenuItem
			// 
			this.AssetArchiveRebuildPackageMenuItem.Index = -1;
			this.AssetArchiveRebuildPackageMenuItem.Text = "";
			// 
			// AssetArchiveRemoveMenuItem
			// 
			this.AssetArchiveRemoveMenuItem.Index = -1;
			this.AssetArchiveRemoveMenuItem.Text = "";
			// 
			// AssetArchiveMakeCurrentMenuItem
			// 
			this.AssetArchiveMakeCurrentMenuItem.Index = -1;
			this.AssetArchiveMakeCurrentMenuItem.Text = "";
			// 
			// AssetArchiveSeperator2MenuItem
			// 
			this.AssetArchiveSeperator2MenuItem.Index = -1;
			this.AssetArchiveSeperator2MenuItem.Text = "";
			// 
			// AssetArchivePruneMenuItem
			// 
			this.AssetArchivePruneMenuItem.Index = -1;
			this.AssetArchivePruneMenuItem.Text = "";
			// 
			// AssetArchiveSeperatorMenuItem
			// 
			this.AssetArchiveSeperatorMenuItem.Index = -1;
			this.AssetArchiveSeperatorMenuItem.Text = "";
			// 
			// AssetArchivePropertiesMenuItem
			// 
			this.AssetArchivePropertiesMenuItem.Index = -1;
			this.AssetArchivePropertiesMenuItem.Text = "";
			// 
			// AssetArchiveExploreMenuItem
			// 
			this.AssetArchiveExploreMenuItem.Index = -1;
			this.AssetArchiveExploreMenuItem.Text = "";
			// 
			// AssetArchiveListMenuItem
			// 
			this.AssetArchiveListMenuItem.Index = -1;
			this.AssetArchiveListMenuItem.Text = "";
			// 
			// AssetArchiveViewMenuItem
			// 
			this.AssetArchiveViewMenuItem.Index = -1;
			this.AssetArchiveViewMenuItem.Text = "";
			// 
			// AssetManagerChatArchiveViewMenuItem
			// 
			this.AssetManagerChatArchiveViewMenuItem.Index = -1;
			this.AssetManagerChatArchiveViewMenuItem.Text = "";
			// 
			// mnuExplorerAssignTo
			// 
			this.mnuExplorerAssignTo.Index = -1;
			this.mnuExplorerAssignTo.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuExplorerUser,
            this.mnuExplorerSeparator});
			this.mnuExplorerAssignTo.Text = "Assign To";
			// 
			// mnuExplorerUser
			// 
			this.mnuExplorerUser.Index = 0;
			this.mnuExplorerUser.Text = "User";
			// 
			// mnuExplorerSeparator
			// 
			this.mnuExplorerSeparator.Index = 1;
			this.mnuExplorerSeparator.Text = "-";
			// 
			// mnuExplorerImport
			// 
			this.mnuExplorerImport.Index = -1;
			this.mnuExplorerImport.Text = "Import";
			// 
			// mnuExplorerProperties
			// 
			this.mnuExplorerProperties.Index = -1;
			this.mnuExplorerProperties.Text = "Properties";
			// 
			// columnHeader9
			// 
			this.columnHeader9.Text = "Name";
			this.columnHeader9.Width = 462;
			// 
			// columnHeader10
			// 
			this.columnHeader10.Text = "Date";
			this.columnHeader10.Width = 110;
			// 
			// columnHeader11
			// 
			this.columnHeader11.Text = "Creator";
			// 
			// columnHeader12
			// 
			this.columnHeader12.Text = "Responsible Party";
			this.columnHeader12.Width = 100;
			// 
			// columnHeader25
			// 
			this.columnHeader25.Text = "Name";
			this.columnHeader25.Width = 250;
			// 
			// columnHeader26
			// 
			this.columnHeader26.Text = "State";
			this.columnHeader26.Width = 200;
			// 
			// columnHeader27
			// 
			this.columnHeader27.Text = "Other";
			this.columnHeader27.Width = 298;
			// 
			// MOGStatusBar
			// 
			this.MOGStatusBar.Location = new System.Drawing.Point(0, 542);
			this.MOGStatusBar.Name = "MOGStatusBar";
			this.MOGStatusBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.MOGStatusBarInfoStatusBarPanel,
            this.MOGStatusBarBlankStatusBarPanel,
            this.MOGStatusBarProjectStatusBarPanel,
            this.MOGStatusBarBranchStatusBarPanel,
            this.MOGStatusBarPlatformBarPanel,
            this.MOGStatusBarUserBarPanel,
            this.MOGStatusBarConnectionStatusBarPanel,
            this.MOGStatusBarConnectionEmptyBarPanel});
			this.MOGStatusBar.ShowPanels = true;
			this.MOGStatusBar.Size = new System.Drawing.Size(970, 24);
			this.MOGStatusBar.SizingGrip = false;
			this.MOGStatusBar.TabIndex = 3;
			this.MOGStatusBar.Text = "Ready";
			// 
			// MOGStatusBarInfoStatusBarPanel
			// 
			this.MOGStatusBarInfoStatusBarPanel.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
			this.MOGStatusBarInfoStatusBarPanel.Name = "MOGStatusBarInfoStatusBarPanel";
			this.MOGStatusBarInfoStatusBarPanel.Text = "Ready";
			this.MOGStatusBarInfoStatusBarPanel.ToolTipText = "Ready";
			this.MOGStatusBarInfoStatusBarPanel.Width = 47;
			// 
			// MOGStatusBarBlankStatusBarPanel
			// 
			this.MOGStatusBarBlankStatusBarPanel.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
			this.MOGStatusBarBlankStatusBarPanel.BorderStyle = System.Windows.Forms.StatusBarPanelBorderStyle.Raised;
			this.MOGStatusBarBlankStatusBarPanel.MinWidth = 50;
			this.MOGStatusBarBlankStatusBarPanel.Name = "MOGStatusBarBlankStatusBarPanel";
			this.MOGStatusBarBlankStatusBarPanel.Width = 677;
			// 
			// MOGStatusBarProjectStatusBarPanel
			// 
			this.MOGStatusBarProjectStatusBarPanel.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
			this.MOGStatusBarProjectStatusBarPanel.Name = "MOGStatusBarProjectStatusBarPanel";
			this.MOGStatusBarProjectStatusBarPanel.Text = "Project";
			this.MOGStatusBarProjectStatusBarPanel.ToolTipText = "Current logged in project";
			this.MOGStatusBarProjectStatusBarPanel.Width = 49;
			// 
			// MOGStatusBarBranchStatusBarPanel
			// 
			this.MOGStatusBarBranchStatusBarPanel.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
			this.MOGStatusBarBranchStatusBarPanel.Name = "MOGStatusBarBranchStatusBarPanel";
			this.MOGStatusBarBranchStatusBarPanel.Text = "Branch";
			this.MOGStatusBarBranchStatusBarPanel.ToolTipText = "Current logged in branch of the selected project";
			this.MOGStatusBarBranchStatusBarPanel.Width = 50;
			// 
			// MOGStatusBarPlatformBarPanel
			// 
			this.MOGStatusBarPlatformBarPanel.Name = "MOGStatusBarPlatformBarPanel";
			this.MOGStatusBarPlatformBarPanel.Text = "PC";
			this.MOGStatusBarPlatformBarPanel.Width = 38;
			// 
			// MOGStatusBarUserBarPanel
			// 
			this.MOGStatusBarUserBarPanel.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
			this.MOGStatusBarUserBarPanel.Name = "MOGStatusBarUserBarPanel";
			this.MOGStatusBarUserBarPanel.Text = "User";
			this.MOGStatusBarUserBarPanel.ToolTipText = "Current login user";
			this.MOGStatusBarUserBarPanel.Width = 38;
			// 
			// MOGStatusBarConnectionStatusBarPanel
			// 
			this.MOGStatusBarConnectionStatusBarPanel.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
			this.MOGStatusBarConnectionStatusBarPanel.Name = "MOGStatusBarConnectionStatusBarPanel";
			this.MOGStatusBarConnectionStatusBarPanel.Text = "Unknown";
			this.MOGStatusBarConnectionStatusBarPanel.ToolTipText = "Your network status is unknown";
			this.MOGStatusBarConnectionStatusBarPanel.Width = 61;
			// 
			// MOGStatusBarConnectionEmptyBarPanel
			// 
			this.MOGStatusBarConnectionEmptyBarPanel.BorderStyle = System.Windows.Forms.StatusBarPanelBorderStyle.None;
			this.MOGStatusBarConnectionEmptyBarPanel.Name = "MOGStatusBarConnectionEmptyBarPanel";
			this.MOGStatusBarConnectionEmptyBarPanel.Width = 10;
			// 
			// ProcessTimer
			// 
			this.ProcessTimer.SynchronizingObject = this;
			// 
			// StatusBarImageList
			// 
			this.StatusBarImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("StatusBarImageList.ImageStream")));
			this.StatusBarImageList.TransparentColor = System.Drawing.Color.Transparent;
			this.StatusBarImageList.Images.SetKeyName(0, "");
			this.StatusBarImageList.Images.SetKeyName(1, "");
			this.StatusBarImageList.Images.SetKeyName(2, "");
			this.StatusBarImageList.Images.SetKeyName(3, "");
			// 
			// MOGHelpProvider
			// 
			this.MOGHelpProvider.HelpNamespace = "moghelp.chm";
			// 
			// WorkSpaceButtonImageList
			// 
			this.WorkSpaceButtonImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("WorkSpaceButtonImageList.ImageStream")));
			this.WorkSpaceButtonImageList.TransparentColor = System.Drawing.Color.Transparent;
			this.WorkSpaceButtonImageList.Images.SetKeyName(0, "");
			this.WorkSpaceButtonImageList.Images.SetKeyName(1, "");
			this.WorkSpaceButtonImageList.Images.SetKeyName(2, "");
			// 
			// CustomToolsButtonImageList
			// 
			this.CustomToolsButtonImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("CustomToolsButtonImageList.ImageStream")));
			this.CustomToolsButtonImageList.TransparentColor = System.Drawing.Color.Transparent;
			this.CustomToolsButtonImageList.Images.SetKeyName(0, "");
			this.CustomToolsButtonImageList.Images.SetKeyName(1, "");
			this.CustomToolsButtonImageList.Images.SetKeyName(2, "");
			// 
			// DragDropContextMenu
			// 
			this.DragDropContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.ImportAssetSeparateMenuItem,
            this.ImportAssetSingleMenuItem,
            this.ImportAssetSeparatorMenuItem,
            this.ImportAssetSeparateLooseMenuItem,
            this.ImportAssetSingleLooseMenuItem});
			// 
			// ImportAssetSeparateMenuItem
			// 
			this.ImportAssetSeparateMenuItem.Index = 0;
			this.ImportAssetSeparateMenuItem.Text = "Import as separate assets";
			this.ImportAssetSeparateMenuItem.Click += new System.EventHandler(this.ImportAssetSeparateMenuItem_Click);
			// 
			// ImportAssetSingleMenuItem
			// 
			this.ImportAssetSingleMenuItem.Index = 1;
			this.ImportAssetSingleMenuItem.Text = "Import as a single asset";
			this.ImportAssetSingleMenuItem.Click += new System.EventHandler(this.ImportAssetSingleMenuItem_Click);
			// 
			// ImportAssetSeparatorMenuItem
			// 
			this.ImportAssetSeparatorMenuItem.Index = 2;
			this.ImportAssetSeparatorMenuItem.Text = "-";
			// 
			// ImportAssetSeparateLooseMenuItem
			// 
			this.ImportAssetSeparateLooseMenuItem.Index = 3;
			this.ImportAssetSeparateLooseMenuItem.Text = "Import as seprate assets (Loose Matching)";
			this.ImportAssetSeparateLooseMenuItem.Click += new System.EventHandler(this.ImportAssetSeparateLooseMenuItem_Click);
			// 
			// ImportAssetSingleLooseMenuItem
			// 
			this.ImportAssetSingleLooseMenuItem.Index = 4;
			this.ImportAssetSingleLooseMenuItem.Text = "Import as a single asset (Loose Matching)";
			this.ImportAssetSingleLooseMenuItem.Click += new System.EventHandler(this.ImportAssetSingleLooseMenuItem_Click);
			// 
			// MogNotifyIcon
			// 
			this.MogNotifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("MogNotifyIcon.Icon")));
			this.MogNotifyIcon.Text = "MOG";
			this.MogNotifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.MogNotifyIcon_MouseDoubleClick);
			// 
			// MainMogMenuStrip
			// 
			this.MainMogMenuStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
			this.MainMogMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.projectsToolStripMenuItem,
            this.branchesToolStripMenuItem,
            this.aboutToolStripMenuItem});
			this.MainMogMenuStrip.Location = new System.Drawing.Point(0, 0);
			this.MainMogMenuStrip.Name = "MainMogMenuStrip";
			this.MainMogMenuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			this.MainMogMenuStrip.Size = new System.Drawing.Size(970, 24);
			this.MainMogMenuStrip.TabIndex = 10;
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setMOGRepositoryToolStripMenuItem,
            this.testSQLConnectionToolStripMenuItem,
            this.toolStripSeparator3,
            this.quitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// setMOGRepositoryToolStripMenuItem
			// 
			this.setMOGRepositoryToolStripMenuItem.Name = "setMOGRepositoryToolStripMenuItem";
			this.setMOGRepositoryToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
			this.setMOGRepositoryToolStripMenuItem.Text = "&Set MOG Repository";
			this.setMOGRepositoryToolStripMenuItem.Click += new System.EventHandler(this.setMOGRepositoryToolStripMenuItem_Click);
			// 
			// testSQLConnectionToolStripMenuItem
			// 
			this.testSQLConnectionToolStripMenuItem.Name = "testSQLConnectionToolStripMenuItem";
			this.testSQLConnectionToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
			this.testSQLConnectionToolStripMenuItem.Text = "&Test SQL Connection";
			this.testSQLConnectionToolStripMenuItem.Click += new System.EventHandler(this.testSQLConnectionToolStripMenuItem_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(182, 6);
			// 
			// quitToolStripMenuItem
			// 
			this.quitToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("quitToolStripMenuItem.Image")));
			this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
			this.quitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
			this.quitToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
			this.quitToolStripMenuItem.Text = "&Quit";
			this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectAllToolStripMenuItem,
            this.invertSelectionToolStripMenuItem,
            this.toolStripSeparator4,
            this.optionsToolStripMenuItem,
            this.saveUserPrefsToolStripMenuItem});
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.editToolStripMenuItem.Text = "&Edit";
			// 
			// selectAllToolStripMenuItem
			// 
			this.selectAllToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("selectAllToolStripMenuItem.Image")));
			this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
			this.selectAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
			this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
			this.selectAllToolStripMenuItem.Text = "Select &All";
			this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
			// 
			// invertSelectionToolStripMenuItem
			// 
			this.invertSelectionToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("invertSelectionToolStripMenuItem.Image")));
			this.invertSelectionToolStripMenuItem.Name = "invertSelectionToolStripMenuItem";
			this.invertSelectionToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.A)));
			this.invertSelectionToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
			this.invertSelectionToolStripMenuItem.Text = "&Invert Selection...";
			this.invertSelectionToolStripMenuItem.Click += new System.EventHandler(this.invertSelectionToolStripMenuItem_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(239, 6);
			// 
			// optionsToolStripMenuItem
			// 
			this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.soundToolStripMenuItem,
            this.minimizeToSystemTrayToolStripMenuItem,
            this.alwaysDisplayAdvancedGetLatestToolStripMenuItem,
            this.showOldStyleWorkspaceButtonsToolStripMenuItem});
			this.optionsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("optionsToolStripMenuItem.Image")));
			this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
			this.optionsToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
			this.optionsToolStripMenuItem.Text = "Options";
			// 
			// soundToolStripMenuItem
			// 
			this.soundToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.soundEnableToolStripMenuItem,
            this.themeToolStripMenuItem});
			this.soundToolStripMenuItem.Name = "soundToolStripMenuItem";
			this.soundToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
			this.soundToolStripMenuItem.Text = "&Sound";
			// 
			// soundEnableToolStripMenuItem
			// 
			this.soundEnableToolStripMenuItem.Checked = true;
			this.soundEnableToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.soundEnableToolStripMenuItem.Name = "soundEnableToolStripMenuItem";
			this.soundEnableToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
			this.soundEnableToolStripMenuItem.Text = "&Enabled";
			this.soundEnableToolStripMenuItem.Click += new System.EventHandler(this.soundEnableToolStripMenuItem_Click);
			// 
			// themeToolStripMenuItem
			// 
			this.themeToolStripMenuItem.Name = "themeToolStripMenuItem";
			this.themeToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
			this.themeToolStripMenuItem.Text = "Theme";
			// 
			// minimizeToSystemTrayToolStripMenuItem
			// 
			this.minimizeToSystemTrayToolStripMenuItem.Checked = false;
			this.minimizeToSystemTrayToolStripMenuItem.CheckOnClick = true;
			this.minimizeToSystemTrayToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.minimizeToSystemTrayToolStripMenuItem.Name = "minimizeToSystemTrayToolStripMenuItem";
			this.minimizeToSystemTrayToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
			this.minimizeToSystemTrayToolStripMenuItem.Text = "Minimize to system tray";
            // 
            // alwaysDisplayGetAdvancedLatestToolStripMenuItem
            // 
            this.alwaysDisplayAdvancedGetLatestToolStripMenuItem.Checked = false;
            this.alwaysDisplayAdvancedGetLatestToolStripMenuItem.CheckOnClick = true;
            this.alwaysDisplayAdvancedGetLatestToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.alwaysDisplayAdvancedGetLatestToolStripMenuItem.Name = "alwaysDisplayAdvancedGetLatestToolStripMenuItem";
            this.alwaysDisplayAdvancedGetLatestToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
            this.alwaysDisplayAdvancedGetLatestToolStripMenuItem.Text = "Always display advanced 'Get Latest' button";
			this.alwaysDisplayAdvancedGetLatestToolStripMenuItem.CheckedChanged += new System.EventHandler(this.alwaysDisplayAdvancedGetLatestToolStripMenuItem_Click);
			// 
			// showOldStyleWorkspaceButtonsToolStripMenuItem
			// 
			this.showOldStyleWorkspaceButtonsToolStripMenuItem.CheckOnClick = true;
			this.showOldStyleWorkspaceButtonsToolStripMenuItem.Name = "showOldStyleWorkspaceButtonsToolStripMenuItem";
			this.showOldStyleWorkspaceButtonsToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
			this.showOldStyleWorkspaceButtonsToolStripMenuItem.Text = "Show old style workspace buttons";
			this.showOldStyleWorkspaceButtonsToolStripMenuItem.Click += new System.EventHandler(this.showOldStyleWorkspaceButtonsToolStripMenuItem_Click);
			// 
			// saveUserPrefsToolStripMenuItem
			// 
			this.saveUserPrefsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveUserPrefsToolStripMenuItem.Image")));
			this.saveUserPrefsToolStripMenuItem.Name = "saveUserPrefsToolStripMenuItem";
			this.saveUserPrefsToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
			this.saveUserPrefsToolStripMenuItem.Text = "&Save user prefs";
			this.saveUserPrefsToolStripMenuItem.Click += new System.EventHandler(this.saveUserPrefsToolStripMenuItem_Click);
			// 
			// viewToolStripMenuItem
			// 
			this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.remoteMachinesToolStripMenuItem,
            this.toolStripMenuItem2,
            this.ViaibleTabsToolStripMenuItem,
            this.toolStripSeparator7,
            this.refreshToolStripMenuItem,
            this.rebuildInboxToolStripMenuItem,
            this.toolStripSeparator8,
            this.startupToolStripMenuItem,
            this.workspaceToolStripMenuItem,
            this.projectToolStripMenuItem,
            this.libraryToolStripMenuItem,
            this.connectionsToolStripMenuItem,
            this.locksToolStripMenuItem,
            this.toolStripSeparator9,
            this.myLocalExplorerToolStripMenuItem,
            this.myToolboxToolStripMenuItem,
            this.toolStripSeparator10,
            this.showGroupsToolStripMenuItem});
			this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
			this.viewToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
			this.viewToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
			this.viewToolStripMenuItem.Text = "&View";
			this.viewToolStripMenuItem.DropDownOpening += new System.EventHandler(this.viewToolStripMenuItem_DropDownOpening);
			// 
			// remoteMachinesToolStripMenuItem
			// 
			this.remoteMachinesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem});
			this.remoteMachinesToolStripMenuItem.Name = "remoteMachinesToolStripMenuItem";
			this.remoteMachinesToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
			this.remoteMachinesToolStripMenuItem.Text = "&Remote Machines";
			// 
			// addToolStripMenuItem
			// 
			this.addToolStripMenuItem.Name = "addToolStripMenuItem";
			this.addToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
			this.addToolStripMenuItem.Text = "Add..";
			this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(206, 6);
			// 
			// ViaibleTabsToolStripMenuItem
			// 
			this.ViaibleTabsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.VisibleStartToolStripMenuItem,
            this.VisibleWorkspaceToolStripMenuItem,
            this.VisibleProjectToolStripMenuItem,
            this.VisibleLibraryToolStripMenuItem,
            this.VisibleConnectionsToolStripMenuItem,
            this.VisibleLocksToolStripMenuItem});
			this.ViaibleTabsToolStripMenuItem.Name = "ViaibleTabsToolStripMenuItem";
			this.ViaibleTabsToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
			this.ViaibleTabsToolStripMenuItem.Text = "Visible Tabs";
			// 
			// VisibleStartToolStripMenuItem
			// 
			this.VisibleStartToolStripMenuItem.Checked = true;
			this.VisibleStartToolStripMenuItem.CheckOnClick = true;
			this.VisibleStartToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.VisibleStartToolStripMenuItem.Name = "VisibleStartToolStripMenuItem";
			this.VisibleStartToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
			this.VisibleStartToolStripMenuItem.Tag = "0";
			this.VisibleStartToolStripMenuItem.Text = "Start";
			this.VisibleStartToolStripMenuItem.Click += new System.EventHandler(this.VisibleTabsToolStripMenuItem_Click);
			// 
			// VisibleLibraryToolStripMenuItem
			// 
			this.VisibleLibraryToolStripMenuItem.Checked = true;
			this.VisibleLibraryToolStripMenuItem.CheckOnClick = true;
			this.VisibleLibraryToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.VisibleLibraryToolStripMenuItem.Name = "VisibleLibraryToolStripMenuItem";
			this.VisibleLibraryToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
			this.VisibleLibraryToolStripMenuItem.Tag = "1";
			this.VisibleLibraryToolStripMenuItem.Text = "Library";
			this.VisibleLibraryToolStripMenuItem.Click += new System.EventHandler(this.VisibleTabsToolStripMenuItem_Click);
			// 
			// VisibleWorkspaceToolStripMenuItem
			// 
			this.VisibleWorkspaceToolStripMenuItem.Checked = true;
			this.VisibleWorkspaceToolStripMenuItem.CheckOnClick = true;
			this.VisibleWorkspaceToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.VisibleWorkspaceToolStripMenuItem.Name = "VisibleWorkspaceToolStripMenuItem";
			this.VisibleWorkspaceToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
			this.VisibleWorkspaceToolStripMenuItem.Tag = "2";
			this.VisibleWorkspaceToolStripMenuItem.Text = "Workspace";
			this.VisibleWorkspaceToolStripMenuItem.Click += new System.EventHandler(this.VisibleTabsToolStripMenuItem_Click);
			// 
			// VisibleProjectToolStripMenuItem
			// 
			this.VisibleProjectToolStripMenuItem.Checked = true;
			this.VisibleProjectToolStripMenuItem.CheckOnClick = true;
			this.VisibleProjectToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.VisibleProjectToolStripMenuItem.Name = "VisibleProjectToolStripMenuItem";
			this.VisibleProjectToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
			this.VisibleProjectToolStripMenuItem.Tag = "3";
			this.VisibleProjectToolStripMenuItem.Text = "Project";
			this.VisibleProjectToolStripMenuItem.Click += new System.EventHandler(this.VisibleTabsToolStripMenuItem_Click);
			// 
			// VisibleConnectionsToolStripMenuItem
			// 
			this.VisibleConnectionsToolStripMenuItem.Checked = true;
			this.VisibleConnectionsToolStripMenuItem.CheckOnClick = true;
			this.VisibleConnectionsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.VisibleConnectionsToolStripMenuItem.Name = "VisibleConnectionsToolStripMenuItem";
			this.VisibleConnectionsToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
			this.VisibleConnectionsToolStripMenuItem.Tag = "4";
			this.VisibleConnectionsToolStripMenuItem.Text = "Connections";
			this.VisibleConnectionsToolStripMenuItem.Click += new System.EventHandler(this.VisibleTabsToolStripMenuItem_Click);
			// 
			// VisibleLocksToolStripMenuItem
			// 
			this.VisibleLocksToolStripMenuItem.Checked = true;
			this.VisibleLocksToolStripMenuItem.CheckOnClick = true;
			this.VisibleLocksToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.VisibleLocksToolStripMenuItem.Name = "VisibleLocksToolStripMenuItem";
			this.VisibleLocksToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
			this.VisibleLocksToolStripMenuItem.Tag = "5";
			this.VisibleLocksToolStripMenuItem.Text = "Locks";
			this.VisibleLocksToolStripMenuItem.Click += new System.EventHandler(this.VisibleTabsToolStripMenuItem_Click);
			// 
			// toolStripSeparator7
			// 
			this.toolStripSeparator7.Name = "toolStripSeparator7";
			this.toolStripSeparator7.Size = new System.Drawing.Size(206, 6);
			// 
			// refreshToolStripMenuItem
			// 
			this.refreshToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("refreshToolStripMenuItem.Image")));
			this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
			this.refreshToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
			this.refreshToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
			this.refreshToolStripMenuItem.Text = "&Refresh";
			this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
			// 
			// rebuildInboxToolStripMenuItem
			// 
			this.rebuildInboxToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("rebuildInboxToolStripMenuItem.Image")));
			this.rebuildInboxToolStripMenuItem.Name = "rebuildInboxToolStripMenuItem";
			this.rebuildInboxToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F5)));
			this.rebuildInboxToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
			this.rebuildInboxToolStripMenuItem.Text = "Re&build Inbox";
			this.rebuildInboxToolStripMenuItem.Click += new System.EventHandler(this.rebuildInboxToolStripMenuItem_Click);
			// 
			// toolStripSeparator8
			// 
			this.toolStripSeparator8.Name = "toolStripSeparator8";
			this.toolStripSeparator8.Size = new System.Drawing.Size(206, 6);
			// 
			// startupToolStripMenuItem
			// 
			this.startupToolStripMenuItem.Name = "startupToolStripMenuItem";
			this.startupToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F2)));
			this.startupToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
			this.startupToolStripMenuItem.Text = "Startup";
			this.startupToolStripMenuItem.Click += new System.EventHandler(this.startupToolStripMenuItem_Click);
			// 
			// workspaceToolStripMenuItem
			// 
			this.workspaceToolStripMenuItem.Name = "workspaceToolStripMenuItem";
			this.workspaceToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F3)));
			this.workspaceToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
			this.workspaceToolStripMenuItem.Text = "Workspace";
			this.workspaceToolStripMenuItem.Click += new System.EventHandler(this.workspaceToolStripMenuItem_Click);
			// 
			// projectToolStripMenuItem
			// 
			this.projectToolStripMenuItem.Name = "projectToolStripMenuItem";
			this.projectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F4)));
			this.projectToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
			this.projectToolStripMenuItem.Text = "Project";
			this.projectToolStripMenuItem.Click += new System.EventHandler(this.projectToolStripMenuItem_Click);
			// 
			// libraryToolStripMenuItem
			// 
			this.libraryToolStripMenuItem.Name = "libraryToolStripMenuItem";
			this.libraryToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F5)));
			this.libraryToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
			this.libraryToolStripMenuItem.Text = "Library";
			this.libraryToolStripMenuItem.Click += new System.EventHandler(this.libraryToolStripMenuItem_Click);
			// 
			// connectionsToolStripMenuItem
			// 
			this.connectionsToolStripMenuItem.Name = "connectionsToolStripMenuItem";
			this.connectionsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F6)));
			this.connectionsToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
			this.connectionsToolStripMenuItem.Text = "Connections";
			this.connectionsToolStripMenuItem.Click += new System.EventHandler(this.connectionsToolStripMenuItem_Click);
			// 
			// locksToolStripMenuItem
			// 
			this.locksToolStripMenuItem.Name = "locksToolStripMenuItem";
			this.locksToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F7)));
			this.locksToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
			this.locksToolStripMenuItem.Text = "Locks";
			this.locksToolStripMenuItem.Click += new System.EventHandler(this.locksToolStripMenuItem_Click);
			// 
			// toolStripSeparator9
			// 
			this.toolStripSeparator9.Name = "toolStripSeparator9";
			this.toolStripSeparator9.Size = new System.Drawing.Size(206, 6);
			// 
			// myLocalExplorerToolStripMenuItem
			// 
			this.myLocalExplorerToolStripMenuItem.Name = "myLocalExplorerToolStripMenuItem";
			this.myLocalExplorerToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F2)));
			this.myLocalExplorerToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
			this.myLocalExplorerToolStripMenuItem.Text = "My &Local Explorer";
			this.myLocalExplorerToolStripMenuItem.Click += new System.EventHandler(this.myLocalExplorerToolStripMenuItem_Click);
			// 
			// myToolboxToolStripMenuItem
			// 
			this.myToolboxToolStripMenuItem.Name = "myToolboxToolStripMenuItem";
			this.myToolboxToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F3)));
			this.myToolboxToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
			this.myToolboxToolStripMenuItem.Text = "My &Toolbox";
			this.myToolboxToolStripMenuItem.Click += new System.EventHandler(this.myToolboxToolStripMenuItem_Click);
			// 
			// toolStripSeparator10
			// 
			this.toolStripSeparator10.Name = "toolStripSeparator10";
			this.toolStripSeparator10.Size = new System.Drawing.Size(206, 6);
			// 
			// showGroupsToolStripMenuItem
			// 
			this.showGroupsToolStripMenuItem.Checked = true;
			this.showGroupsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.showGroupsToolStripMenuItem.Name = "showGroupsToolStripMenuItem";
			this.showGroupsToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
			this.showGroupsToolStripMenuItem.Text = "Show Groups";
			this.showGroupsToolStripMenuItem.Click += new System.EventHandler(this.showGroupsToolStripMenuItem_Click);
			// 
			// toolsToolStripMenuItem
			// 
			this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.requestBuildToolStripMenuItem,
            this.toolStripSeparator5,
            this.loadUserReportToolStripMenuItem,
            this.toolStripSeparator6,
            this.serverManagerToolStripMenuItem,
            this.eventViewerToolStripMenuItem,
            this.configureProjectToolStripMenuItem});
			this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
			this.toolsToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.toolsToolStripMenuItem.Text = "&Tools";
			// 
			// requestBuildToolStripMenuItem
			// 
			this.requestBuildToolStripMenuItem.Name = "requestBuildToolStripMenuItem";
			this.requestBuildToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
			this.requestBuildToolStripMenuItem.Text = "&Request Build...";
			this.requestBuildToolStripMenuItem.Click += new System.EventHandler(this.requestBuildToolStripMenuItem_Click);
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new System.Drawing.Size(178, 6);
			// 
			// loadUserReportToolStripMenuItem
			// 
			this.loadUserReportToolStripMenuItem.Name = "loadUserReportToolStripMenuItem";
			this.loadUserReportToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
			this.loadUserReportToolStripMenuItem.Text = "&Load User Report...";
			this.loadUserReportToolStripMenuItem.Click += new System.EventHandler(this.loadUserReportToolStripMenuItem_Click);
			// 
			// toolStripSeparator6
			// 
			this.toolStripSeparator6.Name = "toolStripSeparator6";
			this.toolStripSeparator6.Size = new System.Drawing.Size(178, 6);
			// 
			// serverManagerToolStripMenuItem
			// 
			this.serverManagerToolStripMenuItem.Image = global::MOG_Client.Properties.Resources.MogServerManager;
			this.serverManagerToolStripMenuItem.Name = "serverManagerToolStripMenuItem";
			this.serverManagerToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
			this.serverManagerToolStripMenuItem.Text = "Server &Manager...";
			this.serverManagerToolStripMenuItem.Click += new System.EventHandler(this.serverManagerToolStripMenuItem_Click);
			// 
			// eventViewerToolStripMenuItem
			// 
			this.eventViewerToolStripMenuItem.Image = global::MOG_Client.Properties.Resources.MOGEventViewer;
			this.eventViewerToolStripMenuItem.Name = "eventViewerToolStripMenuItem";
			this.eventViewerToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
			this.eventViewerToolStripMenuItem.Text = "Event &Viewer...";
			this.eventViewerToolStripMenuItem.Click += new System.EventHandler(this.eventViewerToolStripMenuItem_Click);
			// 
			// configureProjectToolStripMenuItem
			// 
			this.configureProjectToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("configureProjectToolStripMenuItem.Image")));
			this.configureProjectToolStripMenuItem.Name = "configureProjectToolStripMenuItem";
			this.configureProjectToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
			this.configureProjectToolStripMenuItem.Text = "Configure Project...";
			this.configureProjectToolStripMenuItem.Click += new System.EventHandler(this.configureProjectToolStripMenuItem_Click);
			// 
			// projectsToolStripMenuItem
			// 
			this.projectsToolStripMenuItem.Name = "projectsToolStripMenuItem";
			this.projectsToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
			this.projectsToolStripMenuItem.Text = "&Projects";
			this.projectsToolStripMenuItem.DropDownOpening += new System.EventHandler(this.projectsToolStripMenuItem_DropDownOpening);
			// 
			// branchesToolStripMenuItem
			// 
			this.branchesToolStripMenuItem.Name = "branchesToolStripMenuItem";
			this.branchesToolStripMenuItem.Size = new System.Drawing.Size(63, 20);
			this.branchesToolStripMenuItem.Text = "&Branches";
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpFromMOGWikiToolStripMenuItem,
            this.issueTrackerToolStripMenuItem,
            this.toolStripSeparator12,
            this.mOGOnTheWebToolStripMenuItem,
            this.toolStripSeparator11,
            this.discussionForumsToolStripMenuItem,
            this.toolStripSeparator13,
            this.aboutMOGToolStripMenuItem});
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
			this.aboutToolStripMenuItem.Text = "&About";
			// 
			// helpFromMOGWikiToolStripMenuItem
			// 
			this.helpFromMOGWikiToolStripMenuItem.Name = "helpFromMOGWikiToolStripMenuItem";
			this.helpFromMOGWikiToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
			this.helpFromMOGWikiToolStripMenuItem.Text = "Help from MOG &Wiki...";
			this.helpFromMOGWikiToolStripMenuItem.Click += new System.EventHandler(this.helpFromMOGWikiToolStripMenuItem_Click);
			// 
			// issueTrackerToolStripMenuItem
			// 
			this.issueTrackerToolStripMenuItem.Name = "issueTrackerToolStripMenuItem";
			this.issueTrackerToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
			this.issueTrackerToolStripMenuItem.Text = "&Issue Tracker...";
			this.issueTrackerToolStripMenuItem.Click += new System.EventHandler(this.issueTrackerToolStripMenuItem_Click);
			// 
			// toolStripSeparator12
			// 
			this.toolStripSeparator12.Name = "toolStripSeparator12";
			this.toolStripSeparator12.Size = new System.Drawing.Size(188, 6);
			// 
			// mOGOnTheWebToolStripMenuItem
			// 
			this.mOGOnTheWebToolStripMenuItem.Name = "mOGOnTheWebToolStripMenuItem";
			this.mOGOnTheWebToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
			this.mOGOnTheWebToolStripMenuItem.Text = "&MOG on the web...";
			this.mOGOnTheWebToolStripMenuItem.Click += new System.EventHandler(this.mOGOnTheWebToolStripMenuItem_Click);
			// 
			// toolStripSeparator11
			// 
			this.toolStripSeparator11.Name = "toolStripSeparator11";
			this.toolStripSeparator11.Size = new System.Drawing.Size(188, 6);
			// 
			// discussionForumsToolStripMenuItem
			// 
			this.discussionForumsToolStripMenuItem.Name = "discussionForumsToolStripMenuItem";
			this.discussionForumsToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
			this.discussionForumsToolStripMenuItem.Text = "Discussion &Forums...";
			this.discussionForumsToolStripMenuItem.Click += new System.EventHandler(this.discussionForumsToolStripMenuItem_Click);
			// 
			// toolStripSeparator13
			// 
			this.toolStripSeparator13.Name = "toolStripSeparator13";
			this.toolStripSeparator13.Size = new System.Drawing.Size(188, 6);
			// 
			// aboutMOGToolStripMenuItem
			// 
			this.aboutMOGToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("aboutMOGToolStripMenuItem.Image")));
			this.aboutMOGToolStripMenuItem.Name = "aboutMOGToolStripMenuItem";
			this.aboutMOGToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
			this.aboutMOGToolStripMenuItem.Text = "&About MOG...";
			this.aboutMOGToolStripMenuItem.Click += new System.EventHandler(this.aboutMOGToolStripMenuItem_Click);
			// 
			// MogMainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(970, 566);
			this.Controls.Add(this.MOGTabControl);
			this.Controls.Add(this.MOGStatusBar);
			this.Controls.Add(this.MainMogMenuStrip);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.IsMdiContainer = true;
			this.MinimumSize = new System.Drawing.Size(800, 600);
			this.Name = "MogMainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "MOG - The Automated Asset Pipeline";
			this.Load += new System.EventHandler(this.MogMainForm_Load);
			this.Closed += new System.EventHandler(this.MogMainForm_Closed);
			this.Closing += new System.ComponentModel.CancelEventHandler(this.MogMainForm_Closing);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MogMainForm_FormClosing);
			this.Resize += new System.EventHandler(this.MogMainForm_Resize);
			this.MOGTabControl.ResumeLayout(false);
			this.MainTabStartupTabPage.ResumeLayout(false);
			this.MainTabAssetManagerTabPage.ResumeLayout(false);
			this.AssetManagerPanel.ResumeLayout(false);
			this.AssetManagerInboxTabControl.ResumeLayout(false);
			this.AssetManagerDraftsAssetsTabPage.ResumeLayout(false);
			this.AssetManagerInboxAssetsTabPage.ResumeLayout(false);
			this.AssetManagerAssetOutboxAssetsTabPage.ResumeLayout(false);
			this.AssetManagerAssetTrashAssetsTabPage.ResumeLayout(false);
			this.AssetManagerGraphicPanel.ResumeLayout(false);
			this.AssetManagerGraphicPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.SkinInboxButtonPictureBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.SkinInboxEndPictureBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.SkinInboxMidPictureBox)).EndInit();
			this.AssetManagerBottomPanel.ResumeLayout(false);
			this.LocalBranchMiddlePanel.ResumeLayout(false);
			this.LocalBranchMiddlePanel.PerformLayout();
			this.AssetManagerLocalDataTabControl.ResumeLayout(false);
			this.LocalBranchContextMenu.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.SkinMyWorkspaceEndPictureBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.SkinMyWorkspacePictureBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.SkinMyWorkspaceMidPictureBox)).EndInit();
			this.LocalBranchLeftPanel.ResumeLayout(false);
			this.LocalBranchLeftPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.SkinLocalExplorerEndPictureBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.SkinLocalExplorerButtonPictureBox)).EndInit();
			this.LocalBranchRightPanel.ResumeLayout(false);
			this.LocalBranchRightPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.SkinToolboxEndPictureBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.SkinToolboxButtonPictureBox)).EndInit();
			this.MainTabProjectManagerTabPage.ResumeLayout(false);
			this.ProjectManagerTasksPanel.ResumeLayout(false);
			this.ProjectManagerRepositoryTreePanel.ResumeLayout(false);
			this.ProjectManagerRepositoryTreePanel.PerformLayout();
			this.ProjectManagerTreesToolStrip.ResumeLayout(false);
			this.ProjectManagerTreesToolStrip.PerformLayout();
			this.MainTabLibraryTabPage.ResumeLayout(false);
			this.MainTabConnectionManagerTabPage.ResumeLayout(false);
			this.ConnectionManagerBottomPanel.ResumeLayout(false);
			this.ConnectionsBottomTabControl.ResumeLayout(false);
			this.ConnectionsPendingCommandsTabPage.ResumeLayout(false);
			this.ConnectionsPendingPackageTabPage.ResumeLayout(false);
			this.ConnectionsPendingPostTabPage.ResumeLayout(false);
			this.ConnectionsLateResolversTabPage.ResumeLayout(false);
			this.SlaveManagerToolsPanel.ResumeLayout(false);
			this.MainTabLockManagerTabPage.ResumeLayout(false);
			this.MainTabLockManagerTabPage.PerformLayout();
			this.LockManagerToolStrip.ResumeLayout(false);
			this.LockManagerToolStrip.PerformLayout();
			this.MainMogTreeContextMenuStrip.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.MOGStatusBarInfoStatusBarPanel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.MOGStatusBarBlankStatusBarPanel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.MOGStatusBarProjectStatusBarPanel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.MOGStatusBarBranchStatusBarPanel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.MOGStatusBarPlatformBarPanel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.MOGStatusBarUserBarPanel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.MOGStatusBarConnectionStatusBarPanel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.MOGStatusBarConnectionEmptyBarPanel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ProcessTimer)).EndInit();
			this.MainMogMenuStrip.ResumeLayout(false);
			this.MainMogMenuStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			CheckForIllegalCrossThreadCalls = false;

			try
			{
				Application.Run(new MogMainForm(args));
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace);
			}

			// Give the mog server some time to read our socket before we shut down
			Thread.Sleep(500);
		}

		private void MogProcess()
		{
			// Loop until we shutdown
			while (!MOG_Main.IsShutdown())
			{
				// Only process while we are not shutting down?
				if (!MOG_Main.IsShutdownInProgress())
				{
					try
					{
						MOG_Main.Process();
					}
					catch (ThreadAbortException ex)
					{
						// Eat our ThreadAbortException, since this means we were shutting down anyway
						ex.ToString();
					}
					catch (Exception ex)
					{
						MOG_Report.ReportMessage("MOG Process", ex.Message, ex.StackTrace, MOG_ALERT_LEVEL.CRITICAL);
					}
				}

				Thread.Sleep(1);
			}

			// Inform our parent thread that we have exited
			mMogProcess = null;

			// Gracefully wait a while to see if our parent thread will terminate the application for us?
			Thread.Sleep(500);
			// At this point, let's take the inititive and shut ourselves down
			Application.Exit();
		}

		#region Project Manager Page Events

		private void ProjectTreeRefreshToolStripButton_Click(object sender, EventArgs e)
		{
			MainMenuViewClass.MOGGlobalViewRefresh(this);
		}

		private void ProjectTreeCollapseToolStripButton_Click(object sender, EventArgs e)
		{
			mProjectManager.Collapse();
		}

		private void ProjectTreeExpandToolStripButton_Click(object sender, EventArgs e)
		{
			mProjectManager.Expand();
		}	

		private void ProjectTreePackageViewToolStripButton_Click(object sender, EventArgs e)
		{
			if (mProjectManager != null)
			{
				mProjectManager.ProjectTreeButtonClick(ProjectTreePackageViewToolStripButton, ProjectManagerPackageTreeView, "Package");
			}
		}

		private void ProjectTreeSyncViewToolStripButton_Click(object sender, EventArgs e)
		{
			if (mProjectManager != null)
			{
				mProjectManager.ProjectTreeButtonClick(ProjectTreeSyncViewToolStripButton, ProjectManagerSyncTargetTreeView, "Sync");
			}			
		}

		private void ProjectTreeClassificationViewToolStripButton_Click(object sender, EventArgs e)
		{
			if (mProjectManager != null)
			{
				mProjectManager.ProjectTreeButtonClick(ProjectTreeClassificationViewToolStripButton, ProjectManagerClassificationTreeView, "Classification");
			}			
		}

		private void ProjectTreeArchiveViewtoolStripButton_Click(object sender, EventArgs e)
		{
			if (mProjectManager != null)
			{
				mProjectManager.ProjectTreeButtonClick(ProjectTreeArchiveViewtoolStripButton, ProjectManagerArchiveTreeView, "Archive");
			}			
		}

		#endregion
		#region Asset Manager Page events
		/// <summary>
		/// Remove the new state
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AssetManagerInboxAssetListView_Click(object sender, System.EventArgs e)
		{
			foreach (ListViewItem item in AssetManagerInboxListView.SelectedItems)
			{
				try
				{
					//string fullPath = item.SubItems[mAssetManager.ColumnNameFind(currentViewAdd.Columns, "Fullname")].Text;
					//DirectoryInfo dir = new DirectoryInfo(fullPath);
					//dir.CreationTime = dir.CreationTime.AddHours(-2);
					item.Font = new Font(item.Font.FontFamily, item.Font.Size, FontStyle.Regular);
				}
				catch
				{
				}
			}
		}

		/// <summary>
		/// Removes or adds inbox columns
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void MOGAssetInboxMenuItem_Click(object sender, System.EventArgs e)
		{
			MenuItem item = (MenuItem)sender;
			item.Checked = !item.Checked;
			foreach (ColumnHeader header in AssetManagerInboxListView.Columns)
			{
				if (string.Compare(item.Text, header.Text, true) == 0)
				{
					if (!item.Checked)
					{
						header.Width = 0;
					}
					else
					{
						header.Width = 120;
					}
				}
			}
		}

		/// <summary>
		/// Re-populate the workspace client windows
		/// </summary>
		public void RefreshClientAssetWindow()
		{
			mAssetManager.RefreshActiveWindow();
		}

		/// <summary>
		/// Select the active platform
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LocalBranchTabControl_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (mAssetManager.mLocal != null &&
				AssetManagerLocalDataTabControl.SelectedTab != null &&
				AssetManagerLocalDataTabControl.TabPages.Count > 0)
			{
				mAssetManager.mLocal.UpdateActiveLocalBranch();
			}
		}

		/// <summary>
		/// Change the active user button on the Asset Manager page
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cboxAssetManagerUser_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			mAssetManager.SetUserInbox(AssetManagerActiveUserComboBox.Text);
		}

		private void AssetManagerActiveUserDepartmentsComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			List<string> users = guiStartup.GetUsers(MOG_ControllerProject.GetProjectName(), AssetManagerActiveUserDepartmentsComboBox.Text);
			AssetManagerActiveUserComboBox.Items.Clear();
			AssetManagerActiveUserComboBox.Items.AddRange(users.ToArray());
		}

		private void AssetManagerActiveUserComboBox_Click(object sender, System.EventArgs e)
		{
			AssetManagerActiveUserComboBox.Cursor = Cursors.Arrow;
			AssetManagerActiveUserComboBox.DroppedDown = true;
			AssetManagerActiveUserComboBox.Cursor = Cursors.Default;
		}

		private void AssetManagerActiveUserComboBox_TextChanged(object sender, System.EventArgs e)
		{
			mAssetManager.SetUserInbox(AssetManagerActiveUserComboBox.Text);
		}

		/// <summary>
		/// Switch to the login users box
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AssetManagerHomeButton_Click(object sender, System.EventArgs e)
		{
			// Set the active user
			AssetManagerActiveUserComboBox.Text = MOG_ControllerProject.GetUser().GetUserName();
			mAssetManager.SetUserInbox(MOG_ControllerProject.GetUser().GetUserName());
			AssetManagerInboxTabControl.SelectedTab = AssetManagerDraftsAssetsTabPage;
		}

		private void GetLatest_Click(object sender, EventArgs e)
		{
			mAssetManager.BuildMerge(true, false);
		}

		private void GetLatestAdvanced_Click(object sender, EventArgs e)
		{
			mAssetManager.BuildMerge(true, false);
		}

		// Package button will fire a package command to the server which will in turn
		// tell the editor or apawn ucc
		private void AssetManagerPackageButton_Click(object sender, System.EventArgs e)
		{
			MOG_ControllerSyncData sync = MOG_ControllerProject.GetCurrentSyncDataController();
			if (sync != null)
			{
				MOG_ControllerSyncData.PackageState state = sync.GetLocalPackagingStatus();
				if (state == MOG_ControllerSyncData.PackageState.PackageState_Pending ||
					state == MOG_ControllerSyncData.PackageState.PackageState_Error)
				{
					sync.BuildPackage();
				}
			}
		}

		// Explore a group in the Inbox.Assets directory
		private void AssetManagerInboxAssetListView_DoubleClick(object sender, System.EventArgs e)
		{
			// if we are an asset, get the properties
			mAssetManager.mAssets.DoubleClick();
		}

		// Update the inbox
		private void AssetManagerBoxesTabControl_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			RefreshClientAssetWindow();
		}

		#region Drag and drop events

		private void ImportAssetSeparateMenuItem_Click(object sender, System.EventArgs e)
		{
			if (mDragFiles != null)
			{
				List<string> items = new List<string>();

				// Check for any directories and explode them
				foreach (string item in mDragFiles)
				{
					// Check if this represents an entire directory?
					if (Directory.Exists(item))
					{
						// Get all of the contained items within this directory
						items.AddRange(Directory.GetFiles(item, "*.*", SearchOption.AllDirectories));
					}
					else
					{
						// Just add this single item
						items.Add(item);
					}
				}

				mAssetManager.mAssets.AssetImportFromShell(items.ToArray(), true, false);
				mDragFiles = null;
			}
		}

		private void ImportAssetSingleMenuItem_Click(object sender, System.EventArgs e)
		{
			if (mDragFiles != null)
			{
				mAssetManager.mAssets.AssetImportFromShell(mDragFiles, false, false);
				mDragFiles = null;
			}
		}

		private void ImportAssetSeparateLooseMenuItem_Click(object sender, EventArgs e)
		{
			if (mDragFiles != null)
			{
				List<string> items = new List<string>();

				// Check for any directories and explode them
				foreach (string item in mDragFiles)
				{
					// Check if this represents an entire directory?
					if (Directory.Exists(item))
					{
						// Get all of the contained items within this directory
						items.AddRange(Directory.GetFiles(item, "*.*", SearchOption.AllDirectories));
					}
					else
					{
						// Just add this single item
						items.Add(item);
					}
				}

				mAssetManager.mAssets.AssetImportFromShell(items.ToArray(), true, true);
				mDragFiles = null;
			}
		}

		private void ImportAssetSingleLooseMenuItem_Click(object sender, EventArgs e)
		{
			if (mDragFiles != null)
			{
				mAssetManager.mAssets.AssetImportFromShell(mDragFiles, false, true);
				mDragFiles = null;
			}
		}	

		/// <summary>
		/// Accept system shell Drag & Drop opperations.
		/// This routine actually gets the list of files dropped
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AssetManagerInboxAssetListView_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{
			// If this is a drag/drop we can handle...
			if (e.Data.GetDataPresent(DataFormats.FileDrop) ||
				e.Data.GetDataPresent("ArrayListAssetManager") ||
				e.Data.GetDataPresent("LocalWorkspaceTree")
				)
			{
				// Force Windows to give this Form focus
				Activate();
			}

			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				//Save this list of files so the context menu handlers can use them
				mDragFiles = (string[])e.Data.GetData("FileDrop", false);

				//Check if the right mouse button is down
				if (mbRightButtonDrag)
				{
					DragDropContextMenu.Show(this, PointToClient(new Point(e.X, e.Y)));
				}
				else
				{
					mAssetManager.mAssets.AssetImportFromShell(mDragFiles, true, false);
				}

				if (mbCtrlButtonDrag)
				{
					mbCtrlButtonDrag = false;
					AssetManagerAutoImportCheckBox.Checked = !AssetManagerAutoImportCheckBox.Checked;
				}
			}
			else if (e.Data.GetDataPresent("ArrayListAssetManager"))
			{
				Point clientPoint = AssetManagerInboxListView.PointToClient(new Point(e.X, e.Y));
				ListViewItem hoverItem = AssetManagerInboxListView.GetItemAt(clientPoint.X, clientPoint.Y);
			}
			else if (e.Data.GetDataPresent("LocalWorkspaceTree"))
			{
				// Get our array list
				ArrayList items = (ArrayList)e.Data.GetData("LocalWorkspaceTree");
				if (items != null)
				{
					string[] files = new string[items.Count];

					int x = 0;
					foreach (string file in items)
					{
						files[x++] = file;
					}

					mAssetManager.mAssets.AssetImportFromShell(files, true, false);
				}
			}
		}


		/// <summary>
		/// Accept system shell Drag & Drop opperations.
		/// This routine tells the form that it can accept Drag files from the system shell
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AssetManagerInboxAssetListView_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
		{
			// If the drop comes from the windows shell, allow it
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effect = DragDropEffects.All;
				return;
			}

			e.Effect = DragDropEffects.None;
		}

		/// <summary>
		/// Handle an item being dragged over this listView control.  We are looking for a valid asset type 
		/// that accepts a drop event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AssetManagerInboxAssetListView_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effect = DragDropEffects.All;
				mbRightButtonDrag = (e.KeyState & 0x02) != 0 ? true : false;

				//Make the CTRL toggle the AutoImport checkbox
				if ((!mbCtrlButtonDrag && (e.KeyState & 0x08) != 0) ||
					(mbCtrlButtonDrag && (e.KeyState & 0x08) == 0))
				{
					mbCtrlButtonDrag = !mbCtrlButtonDrag;
					AssetManagerAutoImportCheckBox.Checked = !AssetManagerAutoImportCheckBox.Checked;
				}
				return;
			}
			else if (e.Data.GetDataPresent("ArrayListAssetManager"))
			{
				Point clientPoint = AssetManagerInboxListView.PointToClient(new Point(e.X, e.Y));
				ListViewItem hoverItem = AssetManagerInboxListView.GetItemAt(clientPoint.X, clientPoint.Y);

				if (hoverItem != null)
				{
					MOG_Filename assetName = new MOG_Filename(hoverItem.SubItems[(int)guiAssetManager.AssetBoxColumns.FULLNAME].Text);
					if (assetName.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Group)
					{
						e.Effect = DragDropEffects.All;
						return;
					}
				}
			}
			else if (e.Data.GetDataPresent("LocalWorkspaceTree"))
			{
				e.Effect = DragDropEffects.Copy;
				return;
			}
			e.Effect = DragDropEffects.None;
		}

		/// <summary>
		/// Drag and drop initializer
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AssetManagerInboxAssetListView_ItemDrag(object sender, System.Windows.Forms.ItemDragEventArgs e)
		{
			// Create our list holders
			ArrayList items = new ArrayList();

			// This code will only work in the inbox :(
			foreach (ListViewItem item in AssetManagerInboxListView.SelectedItems)
			{
				// Add the text and a reference to the item to our lists
				items.Add(item.SubItems[(int)guiAssetManager.AssetBoxColumns.FULLNAME].Text);
			}

			// Create a new Data object for the send
			DataObject send = new DataObject("ArrayListAssetManager", items);

			// Fire the DragDrop event
			DragDropEffects dde1 = DoDragDrop(send, DragDropEffects.Copy);
		}

		private void AssetManagerDraftsListView_ItemDrag(object sender, System.Windows.Forms.ItemDragEventArgs e)
		{
			// Create our list holders
			ArrayList items = new ArrayList();

			// This code will only work in the inbox :(
			foreach (ListViewItem item in AssetManagerDraftsListView.SelectedItems)
			{
				// Add the text and a reference to the item to our lists
				items.Add(item.SubItems[(int)guiAssetManager.AssetBoxColumns.FULLNAME].Text);
			}

			// Create a new Data object for the send
			DataObject send = new DataObject("ArrayListAssetManager", items);

			// Fire the DragDrop event
			DragDropEffects dde1 = DoDragDrop(send, DragDropEffects.Copy);
		}
		#endregion

		private void AssetManagerLocalDataExplorerSplitter_SplitterMoving(object sender, System.Windows.Forms.SplitterEventArgs e)
		{
			mAssetManager.mLocalExplorer.ChangeWidth(AssetManagerLocalDataExplorerSplitter.SplitPosition);
		}

		private void AssetManagerLocalDataSplitter_SplitterMoving(object sender, System.Windows.Forms.SplitterEventArgs e)
		{
			mAssetManager.mTools.ChangeWidth(AssetManagerLocalDataSplitter.SplitPosition);
		}


		/// <summary>
		/// Makes sure that our LocalBranchRightPanel does not 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LocalBranchRightPanel_Layout(object sender, LayoutEventArgs e)
		{
			// If MOG is ording us not to change width AND we have an AffectedControl
			//  AND that AffectedControl is the LocalBranchRightPanel...
//			if( !MainMenuViewClass.bChangeLocalBranchRightPanelWidth && e.AffectedControl != null
//				&& e.AffectedControl.GetType().UnderlyingSystemType == typeof(System.Windows.Forms.Panel) )
//			{
//				// Change our width back to what it was (before Splitter set it back to 125)
//				this.LocalBranchRightPanel.Width = MainMenuViewClass.LocalBranchRightPanelSavedWidth;
//				// Now we will allow ourselves to change width again (to whatever user wants)
//				MainMenuViewClass.bChangeLocalBranchRightPanelWidth = true;
//			}

		}

		private void CustomToolsPictureBox_Click(object sender, System.EventArgs e)
		{
			if (MOG_ControllerProject.GetCurrentSyncDataController() != null)
			{
				CustomToolsBox.MainContext.Show((Control)sender, new Point(5, 5));
				CustomToolsBox.Visible = true;
			}
			else
			{
				DialogResult result = MessageBox.Show(this, "Please create a Local Gamedata Tab (to the left) first!\r\n"
					+ "Would you like to create one now?",
					"No Gamedata Tab(s) Detected", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
				if (result == DialogResult.Yes)
				{
					LocalBranchPictureBox_Click(sender, e);
				}
			}
		}

		/// <summary>
		/// This method handles the case in which the user tries to import into another part of the 
		///  Inbox other than Drafts
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AssetManagerInboxTabControl_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{
			// If this is a drag/drop we can handle...
			if (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent("ArrayListAssetManager"))
			{
				// Force Windows to give this Form focus
				Activate();

				// Set the active user to make sure we return them to their own boxes
				AssetManagerActiveUserComboBox.Text = MOG_ControllerProject.GetUser().GetUserName();
				mAssetManager.SetUserInbox(MOG_ControllerProject.GetUser().GetUserName());

				// Force the user over to their drafts tab
				AssetManagerInboxTabControl.SelectedTab = AssetManagerDraftsAssetsTabPage;
			}
			else
			{
				return;
			}

			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] files = (string[])e.Data.GetData("FileDrop", false);
				mAssetManager.mAssets.AssetImportFromShell(files, true, false);
			}
			else if (e.Data.GetDataPresent("ArrayListAssetManager"))
			{
				// Get our array list
				ArrayList assets = (ArrayList)e.Data.GetData("ArrayListAssetManager");

				Point clientPoint = AssetManagerInboxListView.PointToClient(new Point(e.X, e.Y));
				ListViewItem hoverItem = AssetManagerInboxListView.GetItemAt(clientPoint.X, clientPoint.Y);
			}
		}
		private void AssetManagerInboxTabControl_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
		{
			// If the drop comes from the windows shell, allow it
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effect = DragDropEffects.All;
				return;
			}

			e.Effect = DragDropEffects.None;
		}

		private void AssetManagerDraftsListView_DragLeave(object sender, System.EventArgs e)
		{
			if (mbCtrlButtonDrag)
			{
				mbCtrlButtonDrag = false;
				AssetManagerAutoImportCheckBox.Checked = !AssetManagerAutoImportCheckBox.Checked;
			}
		}

		#region Animated buttons
		private void LoadSkins()
		{
			mSkins = new MogUtils_SkinManager(Environment.CurrentDirectory + "\\MogClient_Skin.info");
			mSkins.DownloadSkins();

			mSkins.Load("Workspace");

			// Load the inbox skins
			SkinInboxButtonPictureBox.Image = mSkins.GetImage("Workspace", "{Inbox}Button.Rest");
			SkinInboxMidPictureBox.Image = mSkins.GetImage("Workspace", "{Inbox}BackgroundBarMiddle");
			SkinInboxEndPictureBox.Image = mSkins.GetImage("Workspace", "{Inbox}BackgroundBarEnd");

			AssetManagerGraphicPanel.Size = new Size(AssetManagerGraphicPanel.Width, mSkins.GetImage("Workspace", "{Inbox}BackgroundBarMiddle").Height);

			// Load the MyWorkspace skins
			SkinMyWorkspacePictureBox.Image = mSkins.GetImage("Workspace", "{MyWorkspace}Button.Rest");
			SkinMyWorkspaceEndPictureBox.Image = mSkins.GetImage("Workspace", "{MyWorkspace}BackgroundBarEnd");
			SkinMyWorkspaceMidPictureBox.Image = mSkins.GetImage("Workspace", "{MyWorkspace}BackgroundBarMiddle");

			// Load the ToolBox skins
			SkinToolboxButtonPictureBox.Image = mSkins.GetImage("Workspace", "{MyTools}Button.Rest");
			SkinToolboxEndPictureBox.Image = mSkins.GetImage("Workspace", "{MyTools}BackgroundBarEnd");

			// Load the Explorer window skins
			SkinLocalExplorerButtonPictureBox.Image = mSkins.GetImage("Workspace", "{LocalExplorer}Button.Rest");
			SkinLocalExplorerEndPictureBox.Image = mSkins.GetImage("Workspace", "{LocalExplorer}BackgroundBarEnd");
		}

		private void LocalBranchPictureBox_Click(object sender, System.EventArgs e)
		{
			mAssetManager.mLocal.LocalBranchCreateNew(false);
		}

		private void LocalBranchPictureBox_MouseLeave(object sender, System.EventArgs e)
		{
			SkinMyWorkspacePictureBox.Image = mSkins.GetImage("Workspace", "{MyWorkspace}Button.Rest");
		}

		private void LocalBranchPictureBox_MouseEnter(object sender, System.EventArgs e)
		{
			SkinMyWorkspacePictureBox.Image = mSkins.GetImage("Workspace", "{MyWorkspace}Button.Hover");
		}

		private void LocalBranchPictureBox_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			SkinMyWorkspacePictureBox.Image = mSkins.GetImage("Workspace", "{MyWorkspace}Button.Pressed");
		}

		private void LocalBranchPictureBox_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			SkinMyWorkspacePictureBox.Image = mSkins.GetImage("Workspace", "{MyWorkspace}Button.Hover");
		}

		private void CustomToolsPictureBox_MouseLeave(object sender, System.EventArgs e)
		{
			SkinToolboxButtonPictureBox.Image = mSkins.GetImage("Workspace", "{MyTools}Button.Rest");
		}

		private void CustomToolsPictureBox_MouseEnter(object sender, System.EventArgs e)
		{
			SkinToolboxButtonPictureBox.Image = mSkins.GetImage("Workspace", "{MyTools}Button.Hover");
		}

		private void CustomToolsPictureBox_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			SkinToolboxButtonPictureBox.Image = mSkins.GetImage("Workspace", "{MyTools}Button.Pressed");
		}

		private void CustomToolsPictureBox_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			SkinToolboxButtonPictureBox.Image = mSkins.GetImage("Workspace", "{MyTools}Button.Hover");
		}

		private void SkinInboxButtonPictureBox_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			SkinInboxButtonPictureBox.Image = mSkins.GetImage("Workspace", "{Inbox}Button.Pressed");
		}

		private void SkinInboxButtonPictureBox_MouseEnter(object sender, System.EventArgs e)
		{
			SkinInboxButtonPictureBox.Image = mSkins.GetImage("Workspace", "{Inbox}Button.Hover");
		}

		private void SkinInboxButtonPictureBox_MouseLeave(object sender, System.EventArgs e)
		{
			SkinInboxButtonPictureBox.Image = mSkins.GetImage("Workspace", "{Inbox}Button.Rest");
		}

		private void SkinInboxButtonPictureBox_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			SkinInboxButtonPictureBox.Image = mSkins.GetImage("Workspace", "{Inbox}Button.Hover");
		}
		#endregion

		#region Local Data events
		private void LocalBranchAddMenuItem_Click(object sender, System.EventArgs e)
		{
			mAssetManager.mLocal.LocalBranchCreateNew(false);
		}

		private void LocalBranchRemoveMenuItem_Click(object sender, System.EventArgs e)
		{
			MOG_ControllerSyncData tabBranch = (MOG_ControllerSyncData)AssetManagerLocalDataTabControl.SelectedTab.Tag;
			if (tabBranch != null)
			{
				mAssetManager.mLocal.LocalBranchRemove(tabBranch);
			}
		}

		private void LocalBranchExploreMenuItem_Click(object sender, System.EventArgs e)
		{
			mAssetManager.mLocal.LocalBranchExplore();
		}

		private void LocalBranchRenameMenuItem_Click(object sender, System.EventArgs e)
		{
			mAssetManager.mLocal.LocalBranchRename();
		}

		private void LocalBranchTagMenuItem_Click(object sender, EventArgs e)
		{
			MogForm_CreateTag tag = new MogForm_CreateTag();
			tag.ShowDialog(this);
		}

		private void LocalBranchMogControl_GameDataDestinationTreeView_AfterTargetSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			if (e.Node != null)
			{
				MOG_ControlsLibrary.Common.MogControl_GameDataDestinationTreeView.guiAssetTreeTag tag = e.Node.Tag as MOG_ControlsLibrary.Common.MogControl_GameDataDestinationTreeView.guiAssetTreeTag;
				if (tag != null)
				{
					// Get the handle to this tabs gameData controller
					MOG_ControllerSyncData targetGameData = tag.Object as MOG_ControllerSyncData;
					if (targetGameData != null)
					{
						// Select the approptiate Local Branch tabView
						mAssetManager.mLocal.SetActiveLocalBranchTab(targetGameData);
					}
				}
			}
		}

		private void LocalBranchMogControl_GameDataDestinationTreeView_MOGTreeClick(object sender, System.EventArgs e)
		{
			// Only create the new workspace if we left clicked
			MouseEventArgs mouse = e as MouseEventArgs;
			if (mouse != null && mouse.Button == MouseButtons.Left)
			{
				mAssetManager.mLocal.LocalBranchCreateNew(true);
			}
		}

		#endregion
		#endregion
		#region Slave Manager Page Events

		// Create a new slave
		private void SlaveManagerAddSlaveButton_Click(object sender, System.EventArgs e)
		{
			mSlaveManager.SlaveAdd();
		}

		// Remove a slave
		private void SlaveManagerDeleteSlaveButton_Click(object sender, System.EventArgs e)
		{
			mSlaveManager.SlaveDel();
		}

		private void SlaveManagerOnlyMySlaveCheckBox_CheckedChanged(object sender, System.EventArgs e)
		{
			MOG_ControllerSystem.GetCommandManager().mUseOnlyMySlaves = ((CheckBox)sender).Checked;
		}

		#endregion
		#region Main Mog Form Events

		/// <summary>
		/// FormMainMog_Resize()
		/// Handle the resizing of the form
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MogMainForm_Resize(object sender, System.EventArgs e)
		{
			if (WindowState == FormWindowState.Minimized && minimizeToSystemTrayToolStripMenuItem.Checked)
			{
				ShowInTaskbar = false;
				MogNotifyIcon.Visible = true;
			}
		}

		private void MogNotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			MogNotifyIcon.Visible = false;
			ShowInTaskbar = true; 
			WindowState = FormWindowState.Normal;
		}

		/// <summary>
		/// Form is closing, Save the user preferences
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MogMainForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// Save user prefs
			mUserPrefs.SaveStaticForm_LayoutPrefs();
			guiUserPrefs.SaveDynamic_LayoutPrefs("AssetManager", this);
			guiUserPrefs.SaveDynamic_LayoutPrefs("LibraryManager", this);
			guiUserPrefs.SaveStatic_ProjectPrefs();
			mUserPrefs.Save();

		}

		private void MogMainForm_Closed(object sender, System.EventArgs e)
		{
			// Play the exit sound
			mSoundManager.PlayStatusSound("ClientEvents", "Shutdown");
			while (mSoundManager.Playing())
			{
				Thread.Sleep(100);
			}

			Shutdown();
		}

		private void MogMainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (WindowState == FormWindowState.Maximized)
			{
				MogUtils_Settings.Settings.PutPropertyString("GuiLayout", "Gui", "Width", Convert.ToString(Width - 8));
				MogUtils_Settings.Settings.PutPropertyString("GuiLayout", "Gui", "Height", Convert.ToString(Height - 8));
			}
			// Else save normally...
			else
			{
				MogUtils_Settings.SaveSetting("GuiLayout", "Gui", "Width", Convert.ToString(Width));
				MogUtils_Settings.SaveSetting("GuiLayout", "Gui", "Height", Convert.ToString(Height));
			}

			MogUtils_Settings.SaveSetting("GuiLayout", "Gui", "top", Convert.ToString(Top));
			MogUtils_Settings.SaveSetting("GuiLayout", "Gui", "left", Convert.ToString(Left));

			MogUtils_Settings.Save();
		}

		private void MogMainForm_Load(object sender, EventArgs e)
		{
			// Set the size
			Width = MogUtils_Settings.LoadIntSetting("GuiLayout", "Gui", "Width", 800);
			Height = MogUtils_Settings.LoadIntSetting("GuiLayout", "Gui", "Height", 600);
			// Set the location on screen
			if (MogUtils_Settings.Settings.PropertyExist("GuiLayout", "Gui", "left")) Left = MogUtils_Settings.LoadIntSetting("GuiLayout", "Gui", "left");
			if (MogUtils_Settings.Settings.PropertyExist("GuiLayout", "Gui", "top")) Top = MogUtils_Settings.LoadIntSetting("GuiLayout", "Gui", "top");

			// Check for client being visible
			MiscUtilities.EnsureFormLocatedOnValidMonitors(this);

			// Startup the mog process loop
			if (mMogProcess == null)
			{
				mMogProcess = new Thread(this.MogProcess);
				mMogProcess.Name = "MOGMainForm.cs::MogProcess";
				mMogProcess.Start();
			}
		}		

		/// <summary>
		/// Handles the intialization of starting prefs for each Main Mog Tab
		/// </summary>
		/// <param name="tabNumber"></param>
		public void MOG_TabControl_InitializeTabPage(string tabName)
		{
			switch (tabName)
			{
			case "MainTabProjectManagerTabPage":
				if (mProjectManager == null)
				{
					mProjectManager = new guiProjectManager(this);
				}
				break;
			case "MainTabAssetManagerTabPage":
				if (mAssetManager == null)
				{
					mAssetManager = new guiAssetManager(this);					
				}

				if (mAssetManager != null && mAssetManager.mLocal != null && mAssetManager.mLoadBranches)
				{
					mAssetManager.mLoadBranches = false;
					mAssetManager.mLocal.LoadUserLocalBranches();
				}
				break;

			case "MainTabConnectionManagerTabPage":
				if (mSlaveManager == null)
				{
					mSlaveManager = new guiSlaveManager(this);
				}

				if (mConnectionManager == null)
				{
					mConnectionManager = new guiConnectionManager(this);
				}
				mConnectionManager.Initialize();
				break;

			case "MainTabLockManagerTabPage":
				if (mLockManager == null)
				{
					mLockManager = new guiLockManager(this);
				}
				mLockManager.Initialize();
				break;

			case "MainTabLibraryTabPage":
				if (mLibraryManager == null)
				{
					mLibraryManager = new guiLibraryManager(this);
					mLibraryManager.Initialize();
				}
				break;
			}
		}

		/// <summary>
		/// This event will be called each time the main tab control is changed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MOG_TabControl_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (MOGTabControl.SelectedTab != null)
				InitializeMainTabPage(MOGTabControl.SelectedTab);
		}

		public void InitializeMainTabPage(TabPage tab)
		{
			guiStartup.StatusBarClear(this);
			MOG_TabControl_InitializeTabPage(tab.Name);

			switch (tab.Name)
			{
				case "MainTabProjectManagerTabPage":
					if (MOG_ControllerProject.IsProject())
					{
						MOG_ControllerProject.SetActiveTabName("Project");
					}

					MainMenuViewClass.MOGGlobalViewRefresh(this);
					break;

				case "MainTabAssetManagerTabPage":
					if (MOG_ControllerProject.IsProject())
					{
						MOG_ControllerProject.SetActiveTabName("Workspace");
					}

					RefreshClientAssetWindow();
					break;

				case "MainTabConnectionManagerTabPage":
					if (MOG_ControllerProject.IsProject())
					{
						MOG_ControllerProject.SetActiveTabName("Connections");
					}

					mConnectionManager.Refresh();
					break;

				case "MainTabLockManagerTabPage":
					if (MOG_ControllerProject.IsProject())
					{
						MOG_ControllerProject.SetActiveTabName("Locks");
					}
					break;

				case "MainTabLibraryTabPage":
					if (MOG_ControllerProject.IsProject())
					{
						MOG_ControllerProject.SetActiveTabName("Library");
					}
					break;
				default:
					break;
			}
		}

		/// <summary>
		/// Call this with a tab name to get its index
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private int MOG_TabControl_GetTabIndexFromName(string tabName)
		{
			int index = 0;
			foreach (TabPage page in this.MOGTabControl.TabPages)
			{
				if (string.Compare(tabName, page.Name, true) == 0)
				{
					return index;
				}
				index++;
			}

			return -1;
		}

		internal void MOG_TabControl_HideViewTabMenuItem(TabPage page, bool bVisible)
		{
			switch(page.Name)
			{
				case "MainTabStartupTabPage":
					startupToolStripMenuItem.Visible = bVisible;
					break;
				case "MainTabAssetManagerTabPage":
					workspaceToolStripMenuItem.Visible = bVisible;
					break;
				case "MainTabProjectManagerTabPage":
					projectToolStripMenuItem.Visible = bVisible;
					break;
				case "MainTabLibraryTabPage":
					libraryToolStripMenuItem.Visible = bVisible;
					break;
				case "MainTabConnectionManagerTabPage":
					connectionsToolStripMenuItem.Visible = bVisible;
					break;
				case "MainTabLockManagerTabPage":
					locksToolStripMenuItem.Visible = bVisible;
					break;
			}
		}

		#region Main Menu Click Handles
		/// <summary>
		/// MainMenu - File
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void setMOGRepositoryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MainMenuFileClass.MOGGlobalRepositoryLogin(this, true);			
		}

		private void testSQLConnectionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MainMenuFileClass.MOGGlobalSQLLogin();
		}

		private void quitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MainMenuFileClass.MOGGlobalQuit(this);
		}

		/// <summary>
		/// MainMenu - Edit
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MainMenuViewClass.MOGGlobalViewSelectAll(this);
		}

		private void invertSelectionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MainMenuViewClass.MOGGlobalViewInvertAll(this);
		}

		private void saveUserPrefsToolStripMenuItem_Click(object sender, EventArgs e)
		{				
			MainMenuToolsClass.MOGGlobalToolsSavePrefs(this);
		}

		private void alwaysDisplayAdvancedGetLatestToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// Immediately refresh the local workspace buttons
			mAssetManager.mLocal.RefreshWindowToolbar();
			// Save the new setting
            MogUtils_Settings.SaveSetting("Client", "AlwaysDisplayAdvancedGetLatest", alwaysDisplayAdvancedGetLatestToolStripMenuItem.Checked.ToString());
		}

		private void showOldStyleWorkspaceButtonsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ShowOldStyleWorkspaceButtons = showOldStyleWorkspaceButtonsToolStripMenuItem.Checked;
			MogUtils_Settings.SaveSetting("Workspace", "ShowOldStyleWorkspaceButtons", showOldStyleWorkspaceButtonsToolStripMenuItem.Checked.ToString());
		}

		/// <summary>
		/// MainMenu - View
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		 private void viewToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
		{
			MainMenuViewClass.MOGGlobalViewRemoteMachineInit(this);
		}

		private void addToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MainMenuViewClass.MOGGlobalViewRemoteMachineAdd(this);
		}

		private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MainMenuViewClass.MOGGlobalViewRefresh(this);			
		}

		private void rebuildInboxToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MainMenuViewClass.MOGGlobalViewRebuildInbox(this);
		}

		private void startupToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MOGTabControl.SelectedIndex = MOG_TabControl_GetTabIndexFromName("MainTabStartupTabPage");
		}

		private void workspaceToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MOGTabControl.SelectedIndex = MOG_TabControl_GetTabIndexFromName("MainTabAssetManagerTabPage");
		}

		private void projectToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MOGTabControl.SelectedIndex = MOG_TabControl_GetTabIndexFromName("MainTabProjectManagerTabPage");
		}

		private void libraryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MOGTabControl.SelectedIndex = MOG_TabControl_GetTabIndexFromName("MainTabLibraryTabPage");
		}

		private void connectionsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MOGTabControl.SelectedIndex = MOG_TabControl_GetTabIndexFromName("MainTabConnectionManagerTabPage");
		}

		private void locksToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MOGTabControl.SelectedIndex = MOG_TabControl_GetTabIndexFromName("MainTabLockManagerTabPage");
		}

		private void myLocalExplorerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// Check or uncheck
			ToolStripMenuItem item = sender as ToolStripMenuItem;
			MainMenuViewClass.MOGGlobalViewMyWorkspace(this);
			if (item != null) item.Checked = mAssetManager.mLocalExplorer.Opened;
		}

		private void myToolboxToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// Check or uncheck
			ToolStripMenuItem item = sender as ToolStripMenuItem;
			MainMenuViewClass.MOGGlobalViewMyToolbox(this);
			if (item != null) item.Checked = mAssetManager.mTools.Opened;
		}

		private void showGroupsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			showGroupsToolStripMenuItem.Checked = mAssetManager.mGroups.Enable(!showGroupsToolStripMenuItem.Checked);
			if (mLockManager != null) mLockManager.mGroups.Enable(showGroupsToolStripMenuItem.Checked);
			if (mConnectionManager != null) mConnectionManager.mGroups.Enable(showGroupsToolStripMenuItem.Checked);
		}

		private void requestBuildToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MOG_Privileges privs = MOG_ControllerProject.GetPrivileges();

			if (privs.GetUserPrivilege(MOG_ControllerProject.GetUserName(), MOG_PRIVILEGE.RequestBuild))
			{
				MainMenuToolsClass.MOGGlobalToolsRequestBuildForm(this);
			}
			else
			{
				MOG_Prompt.PromptResponse("Insufficient Privileges", "Your privileges do not allow you to request a build.");
			}
		}

		private void loadUserReportToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MainMenuToolsClass.MOGGlobalToolsLoadReport(this);	
		}

		private void soundEnableToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool enabled;
			enabled = false;

			// Check or uncheck
			ToolStripMenuItem item = sender as ToolStripMenuItem;
			if (item != null)
			{
				if (item.Checked)
				{
					enabled = item.Checked = false;
				}
				else
				{
					enabled = item.Checked = true;
				}
			}
			MainMenuToolsClass.MOGGlobalToolsSoundEnable(this, enabled);
		}

		private void serverManagerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MainMenuToolsClass.MOGGlobalToolsLaunchServerManager(this);
		}

		private void eventViewerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MainMenuToolsClass.MOGGlobalToolsLaunchEventViewer(this);			
		}

		private void configureProjectToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MOG_Project project = MOG_ControllerProject.GetProject();
			//MOG_ControllerProject.LoginProject(project.GetProjectName(), "Current");
			if (project != null)
			{
				if (MOG_ControllerProject.GetPrivileges().GetUserPrivilege(MOG_ControllerProject.GetUserName(), MOG_PRIVILEGE.ConfigureProjectSettings))
				{
					project.Reload();
					ConfigureProjectForm configForm = new ConfigureProjectForm(project);
					configForm.ShowDialog(this);
				}
				else
				{
					MOG_Prompt.PromptResponse("Privilege Not Allowed", "The user, '" + MOG_ControllerProject.GetUserName() + "', "
						+ "does not have permission to configure the Project.");
				}
			}
		}

		/// <summary>
		/// MainMenu - Projects
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void projectsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
		{
			MainMenuProjectsClass.MOGGlobalProjectsInit(true);
			guiProject.UpdateGuiProject(MOG_ControllerProject.GetProjectName());
		}	

		/// <summary>
		/// MainMenu - About
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void helpFromMOGWikiToolStripMenuItem_Click(object sender, EventArgs e)
		{
			guiCommandLine.ShellSpawn("http://wiki.mogware.net");
		}

		private void mOGOnTheWebToolStripMenuItem_Click(object sender, EventArgs e)
		{
			guiCommandLine.ShellSpawn("www.mogware.com");			
		}

		private void issueTrackerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			guiCommandLine.ShellSpawn("http://bugs.mogware.net");			
		}

		private void discussionForumsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			guiCommandLine.ShellSpawn("http://support.mogware.net");
		}

		private void aboutMOGToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string WhatsNewFilename;
			WhatsNewFilename = MOG_Main.GetExecutablePath() + "\\WhatsNew.txt";
			AboutForm about = new AboutForm(WhatsNewFilename);
			about.ShowDialog(this);
		}
		#endregion

		private void AssetManagerLogoutButton_Click(object sender, System.EventArgs e)
		{
			MainMenuProjectsClass.MOGGlobalLaunchProjectLogin(MOG_ControllerProject.GetProjectName(), false);
		}

		/// <summary>
		/// Handle a help request on a form control
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="hlpevent"></param>
		private void AssetManagerInboxAssetListView_HelpRequested(object sender, System.Windows.Forms.HelpEventArgs hlpevent)
		{
			//			int i = 0;
		}

		/// <summary>
		/// Allow our user to change the MOG Tab he/she is in by Dragging Over it.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MOGTabControl_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
		{
			// Get an nice identifier for our sender
			TabControl tabControl = (TabControl)sender;

			if (e.Data.GetDataPresent("filename"))
			{
				// Get the point on MOGMainForm we are at
				Point tabPoint = PointToClient(new Point(e.X, e.Y));
				// Foreach tab in tabControl, decide what we should do...
				for (int i = 0; i < tabControl.TabCount; ++i)
				{
					// If we have either the AssetManager or Library tab pages, we can select...
					if (tabControl.TabPages[i] == MainTabAssetManagerTabPage
						|| tabControl.TabPages[i] == MainTabLibraryTabPage)
					{
						// Get a rectangle representing the area of the tab
						Rectangle tabRectangle = tabControl.GetTabRect(i);
						// Get our tabWidth + tabPoint.X (our greatest X-point)
						int tabWidthX = tabRectangle.X + tabRectangle.Width;
						// Get our tabHeight + tabPoint.Y (our greatest Y-point)
						int tabHeightY = tabRectangle.Y + tabRectangle.Height;
						// If tab's X is less than mouse X AND tab's Width is greater than mouse X
						//   AND tab's Y is less than mouse Y AND tab's Height is greater than mouse Y
						// Summary: We are pointing inside one of our tabs...
						if (tabRectangle.X <= tabPoint.X && tabWidthX >= tabPoint.X
							&& tabRectangle.Y <= tabPoint.Y && tabHeightY >= tabPoint.Y)
						{
							// Change our selected index to the tab we are dragging over
							tabControl.SelectedIndex = i;
						}
					}
				}
			}
		}
		#endregion
		#region Connection Manager page events
		private void ConnectionsListView_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			mConnectionManager.UpdateConnectionStatus();
		}

		private void ConnectionManagerCommandsListView_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			mConnectionManager.UpdateCommandStatus();
		}

		private void ConnectionManagerMergingListView_SelectedIndexChanged(object sender, EventArgs e)
		{
			mConnectionManager.UpdateMergeStatus();
		}

		private void ConnectionManagerPostingListView_SelectedIndexChanged(object sender, EventArgs e)
		{
			mConnectionManager.UpdatePostStatus();
		}

		private void ConnectionManagerLateResolversListView_SelectedIndexChanged(object sender, EventArgs e)
		{
			mConnectionManager.UpdateLateResolverStatus();
		}
		#endregion

		public delegate void MainFormStatusBarUpdateInvoker(string modeText, string message);
		public void MainFormStatusBarUpdate(string modeText, string message)
		{
			if (InvokeRequired)
			{
				if (IsHandleCreated)
				{
					BeginInvoke(new MainFormStatusBarUpdateInvoker(MainFormStatusBarUpdateByInvoke), new object[] { modeText, message });
				}
			}
			else
			{
				MainFormStatusBarUpdateByInvoke(modeText, message);
			}
		}
		private void MainFormStatusBarUpdateByInvoke(string modeText, string message)
		{
			MOGStatusBarInfoStatusBarPanel.Text = modeText;
			MOGStatusBarBlankStatusBarPanel.Text = message;
		}

		private void AssetManagerAutoPackageCheckBox_CheckedChanged(object sender, System.EventArgs e)
		{
			MOG_ControllerSyncData sync = MOG_ControllerProject.GetCurrentSyncDataController();
			if (sync != null)
			{
				sync.EnableAutoPackaging(AssetManagerAutoPackageCheckBox.Checked);

				// Attempt to initiate the auto packaging
				sync.AutoPackage();
			}

			// Update workspace tab
			if (mAssetManager != null)
				if (mAssetManager.mLocal != null) mAssetManager.mLocal.UpdateActiveLocalBranchAutoPackage();
		}
		private void AssetManagerAutoUpdateLocalCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			// Update workspace tab
			if (mAssetManager != null)
				if (mAssetManager.mLocal != null) mAssetManager.mLocal.UpdateActiveLocalBranchAutoUpdate();
		}	

		private void LockManagerToggleFilterToolStripButton_Click(object sender, EventArgs e)
		{
			if (mLockManager != null)
				mLockManager.ToggleFilter(LockManagerToggleFilterToolStripButton.Checked);
		}

		private void LockManagerFilterToolStripTextBox_TextChanged(object sender, EventArgs e)
		{
			if (mLockManager != null)
				mLockManager.AdjustFilter(LockManagerFilterToolStripTextBox.Text);
		}

		private void ProjectTreesShowToolTipsToolStripButton_Click(object sender, EventArgs e)
		{
			ProjectManagerClassificationTreeView.ShowToolTips = ProjectTreesShowToolTipsToolStripButton.Checked;
		}

		private void ProjectTreesShowDescriptionsToolStripButton_Click(object sender, EventArgs e)
		{
			ProjectManagerClassificationTreeView.ShowDescription = ProjectTreesShowDescriptionsToolStripButton.Checked;
		}

		private void VisibleTabsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem item = sender as ToolStripMenuItem;
			if (item != null)
			{
				MainMenuViewClass.ToggleVisibleTab(this, item);
				MogUtils_Settings.SaveSetting("VisibleTabs", item.Text, item.Checked.ToString());
			}
		}

		internal void Shutdown()
		{
			// Inform MOG we are shutting down
			MOG_Main.Shutdown();

			// Play nice and let the processing thread shutdown on it's own
			int waitTime = 0;
			int sleepTime = 10;
			while (mMogProcess != null)
			{
				// Gracefully sleep while we wait for the process thread to shutdown
				Thread.Sleep(sleepTime);
				waitTime += sleepTime;
				// Check if we have waited long enough?
				if (waitTime >= 1000)
				{
					// Forcible terminate the processing thread
					mMogProcess.Abort();
					mMogProcess = null;
				}
			}

			// Terminate our application
			Application.Exit();
		}
	}
}

