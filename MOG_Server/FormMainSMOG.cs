using System;
using System.Drawing;
using System.Reflection;
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;


using MOG;
using MOG.INI;
using MOG.DOSUTILS;
using MOG.REPORT;
using MOG.PROMPT;
using MOG.PROGRESS;
using MOG.PROJECT;
using MOG.TIME;
using MOG.FILENAME;

using MOG_Server.Server_Gui;
using MOG_Server.Server_Mog_Utilities;
using MOG_Server.MOG_ControlsLibrary.Admin;
using MOG_Server.MOG_ControlsLibrary.Common;
using MOG_Server.Server_Gui.Forms.MOGAutotest;

using MOG_Server;
using MOG_Server.Server_Utilities;
using MOG_Server.Server_Gui.guiConfigurationsHelpers;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.PROPERTIES;

using MOG_ControlsLibrary;
using MOG_ControlsLibrary.Forms;
using MOG_CoreControls.MogUtil_DepthManager;
using MOG_CoreControls;
using System.Collections.Generic;
using MOG_Server.Server_Gui.Forms;

namespace Server_Gui
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class FormMainSMOG : System.Windows.Forms.Form
	{
		[DllImport("kernel32.dll")]
		public static extern long GetDriveType(string driveLetter);
		private const int DRIVE_TYPE_FLOPPY		= 2;
		private const int DRIVE_TYPE_HD			= 3;
		private const int DRIVE_TYPE_NETWORK	= 4;
		private const int DRIVE_TYPE_CD			= 5;
		private const int DRIVE_TYPE_RAM		= 6;

		private static int WM_QUERYENDSESSION = 0x11;

		#region Member vars
		
		public UserPrefs mUserPrefs;
		//public Configuration mConfigurations;
		public guiLocks mLocks;
		public guiPendingCommands mPendingCommands;
		public EventCallbacks mEventCallbacks;
		public bool bCheckAutobuild;
		public Thread mMogProcess;
		public guiMonitor mMonitor;
		public string smogIniFilename;
		String mMogIni = "";
		
		public bool createDemoProject = false;		// used only with FirstRunWizard
		#endregion
		#region Enums, etc.
		public enum MAIN_PAGES 
		{
			SMOG_CONNECTIONS,
			SMOG_LOCKS,
			SMOG_COMMANDS,
			SMOG_BACKUP,
		};

		public enum MAIN_IMAGES 
		{
			FOLDER_CLOSED,
			FOLDER_OPEN,
			LED_GO,
			LED_STOP,
		};

		#endregion
		#region System definitions
		public System.Windows.Forms.TabControl SMOG_Main_TabControl;
		private System.Windows.Forms.ColumnHeader colMachineName;
		private System.Windows.Forms.ColumnHeader colIpAddress;
		private System.Windows.Forms.ColumnHeader colNetworkId;
		private System.Windows.Forms.ColumnHeader colConnectionType;
		public System.Windows.Forms.ListView lviewMonitor;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ColumnHeader columnHeader5;
		private System.Windows.Forms.TabPage ServerMonitorTabPage;
		private System.Windows.Forms.TabPage ServerLoLocksTabPage;
		public System.Windows.Forms.ListView LocksListView;
		private System.Windows.Forms.Panel LocksLeftPanel;
		private System.Windows.Forms.TabPage ServerCommandsTabPage;
		private System.Windows.Forms.ColumnHeader columnHeader6;
		private System.Windows.Forms.ColumnHeader columnHeader8;
		private System.Windows.Forms.ColumnHeader columnHeader9;
		private System.Windows.Forms.ColumnHeader columnHeader10;
		private System.Windows.Forms.ColumnHeader columnHeader12;
		private System.Windows.Forms.ColumnHeader columnHeader13;
		private System.Windows.Forms.ColumnHeader columnHeader14;
		private System.Windows.Forms.ColumnHeader columnHeader15;
		private System.Windows.Forms.ColumnHeader columnHeader16;
		private System.Windows.Forms.ColumnHeader columnHeader17;
		private System.Windows.Forms.ColumnHeader columnHeader18;
		private System.Windows.Forms.ColumnHeader columnHeader19;
		public System.Windows.Forms.ListView CommandspendingListView;
		private System.Windows.Forms.ColumnHeader CommandsPendingTypeColumnHeader;
		private System.Windows.Forms.ColumnHeader CommandsPendingNameColumnHeader;
		private System.Windows.Forms.ColumnHeader CommandsPendingMachineColumnHeader;
		private System.Windows.Forms.ColumnHeader CommandsPendingIpColumnHeader;
		private System.Windows.Forms.ColumnHeader CommandsPendingIdColumnHeader;
		private System.Windows.Forms.Timer IdleSlaveTimer;
		public System.Windows.Forms.ImageList SMOGGlobalImageList;
		private System.Windows.Forms.ContextMenu ConnectionsContextMenu;
		private System.Windows.Forms.MenuItem ConnectionKillMenuItem;
		private System.Windows.Forms.ContextMenu LocksContextMenu;
		private System.Windows.Forms.MenuItem LocksKillLockMenuItem;
		private System.Windows.Forms.MenuItem ConnectionLaunchMenuItem;
		private System.Windows.Forms.Splitter LocksSplitter;
		public System.Windows.Forms.ListView LocksRequestLocksListView;
		private System.Windows.Forms.ColumnHeader RequestLockNameColumnHeader;
		private System.Windows.Forms.ColumnHeader RequestLockIpColumnHeader;
		private System.Windows.Forms.ColumnHeader RequestLockMachineColumnHeader;
		private System.Windows.Forms.ColumnHeader RequestLockNetIdColumnHeader;
		private System.Windows.Forms.ColumnHeader RequestLockDescriptionColumnHeader;
		private System.Windows.Forms.ContextMenu RequestLocksContextMenu;
		private System.Windows.Forms.ColumnHeader RequestLockTypeColumnHeader;
		private System.Windows.Forms.ColumnHeader CommandsPendingTimeColumnHeader;
		private System.Windows.Forms.ImageList LocksImageList;
		private System.Windows.Forms.ImageList CommandImageList;
		private System.Windows.Forms.ColumnHeader CommandsPendingSlaveColumnHeader;
		private System.Windows.Forms.ColumnHeader LocksTypeColumnHeader;
		private System.Windows.Forms.ColumnHeader LocksNameColumnHeader;
		private System.Windows.Forms.ColumnHeader LocksIpColumnHeader;
		private System.Windows.Forms.ColumnHeader LocksMachineColumnHeader;
		private System.Windows.Forms.ColumnHeader LocksNetIdColumnHeader;
		private System.Windows.Forms.ColumnHeader LocksDescriptionColumnHeader;
		private System.Windows.Forms.ColumnHeader LocksTimeColumnHeader;
		private System.Windows.Forms.ColumnHeader RequestLockTimeColumnHeader;
		private System.Windows.Forms.ColumnHeader LocksUserColumnHeader;
		private System.Windows.Forms.ColumnHeader RequestLockUserColumnHeader;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.ContextMenu CommandsContextMenu;
		private System.Windows.Forms.MenuItem CommandsKillMenuItem;
		private System.Windows.Forms.MenuItem ConnectionRetaskCommandMenuItem;
		private System.Windows.Forms.ColumnHeader CommandsPendingCommandIdColumnHeader;
		private System.Windows.Forms.HelpProvider helpProvider;
		private System.Windows.Forms.ToolTip MOGServerToolTip;
		private System.Windows.Forms.MenuItem ConnectionLaunchTestMenuItem;
		private System.Windows.Forms.NotifyIcon ServerProcessNotifyIcon;
		private System.Windows.Forms.ContextMenu ServerNotifyIconContextMenu;
		private System.Windows.Forms.MenuItem ServerNotifyOpenMenuItem;
		private System.Windows.Forms.MenuItem ServerNotifySetRepoMenuItem;
		private System.Windows.Forms.MenuItem ServerNotifySep1MenuItem;
		private System.Windows.Forms.MenuItem ServerNotifySetSQLMenuItem;
		private System.Windows.Forms.MenuItem ServerNotifySep2MenuItem;
		private System.Windows.Forms.MenuItem ServerNotifyExitMenuItem;
		private System.Windows.Forms.Panel ServerButtonPanel;
		private System.Windows.Forms.StatusBar ServerStatusBar;
		private System.Windows.Forms.Button ServerCloseButton;
		private MenuItem ServerInstallLicenseMenuItem;
		private MenuItem menuItem2;
		private System.Windows.Forms.ImageList MainTabImageList;
		#endregion
		#region Constructor

		//CriticalEventViewerControl criticalEventViewer = new CriticalEventViewerControl();
		public FormMainSMOG(string[] args)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();			

			// Parse command line options
			if (args.Length > 0)
			{
				foreach(String arg in args)
				{
					if (arg.IndexOf("mog=") != -1)
					{
						mMogIni = arg.Substring(arg.IndexOf("=")+1);
					}
				}
			}

			this.createDemoProject = false;
			this.smogIniFilename = Environment.CurrentDirectory + "\\smog.ini";
			mMogIni = DoInitialRepositoryLogic(mMogIni);
			if (mMogIni.Length == 0)
			{
				MOG_Prompt.PromptResponse("Server Initialization Failed", "No valid Repository was selected.\nServer is shutting down.", Environment.StackTrace, MOGPromptButtons.OK);
				MOG_Main.Shutdown();
// The server ain't shutting down...Grrr 
// We need to have an init call where all this happens instead of doing it in the constructor so we can monitor a failure
//				Application.Exit();
//				Application.DoEvents();
				return;
			}

			InitializeMOGServer();

			// Initialize the callbacks
			mEventCallbacks = new EventCallbacks(this);

			// Initialise the SQL Database
			MOG_ControllerSystem.InitializeDatabase("", "", "");

			// Load user preferences
			string prefsFile = String.Concat(Environment.CurrentDirectory, "\\SMOGUserPrefs.ini");
			mUserPrefs = new UserPrefs(prefsFile);
			mUserPrefs.Load(this);

			// Initialize the monitor
			mMonitor = new guiMonitor(this);
			mMonitor.DisplayUpdateExistingConnections();

			// Startup the mog process loop
			if (mMogProcess == null)
			{
				mMogProcess = new Thread(new ThreadStart(this.MogProcess));
				mMogProcess.IsBackground = true;
				mMogProcess.Start();
			}

			MOG_Ini mIni = null;
			string dumpFilename =  MOG_Main.GetExecutablePath() + "\\ServerStateDump.info";

			try
			{
				// Establish proper settings in our ini
				mIni = new MOG_Ini(MOG_Main.GetExecutablePath() + "\\mog.ini");
				mIni.PutString("MOG", "ValidMogRepository", "");
				mIni.PutString("SQL", "ConnectionString", MOG_ControllerSystem.GetDB().GetConnectionString());
				mIni.Save();

				// load up previous server state
				bool bServerRunning = mIni.KeyExist("SERVERSTATUS", "ServerRunning") ? mIni.GetBool("SERVERSTATUS", "ServerRunning") : false;

				if (File.Exists(dumpFilename) && !bServerRunning)
				{
					try
					{
						MOG_CommandServerCS cmdServer = (MOG_CommandServerCS)MOG_ControllerSystem.GetCommandManager();
						if (cmdServer != null)
						{
							cmdServer.RestoreServerStateFromFile(dumpFilename);
						}
					}
					catch
					{
					}
				}
				else
				{
					MOG_Prompt.PromptMessage("MOG Server Warning", 	"The MOG Server has detected it was not properly shutdown.\n" + 
																	"Any commands that were being processed around the time of shutdown may need to be manually verified for integrity.");
				}
			}
			catch
			{
			}
			finally
			{
				if(File.Exists(dumpFilename))
				{
					File.Delete(dumpFilename);
				}

				// Indicate that we are in a running state
				mIni.PutBool("SERVERSTATUS", "ServerRunning",true);
				mIni.Save();
				mIni.Close();
			}

			// refresh commands listview in case we restored any
			if (mPendingCommands == null)
			{
				mPendingCommands = new guiPendingCommands(this);
			}
			else
			{
				mPendingCommands.RefreshWindow(null);
			}

			Bitmap home = new Bitmap(this.SMOGGlobalImageList.Images[0]);			
			this.ServerProcessNotifyIcon.Icon = System.Drawing.Icon.FromHandle(home.GetHicon());

			// Now that we are running, set us in silent mode 
			MOG_Prompt.SetMode(MOG_PROMPT_MODE_TYPE.MOG_PROMPT_SILENT);

			MOG_Progress.ClientForm = this;
			MOG_Prompt.ClientForm = this;

			DepthManager.InitParent(this, FormDepth.BACK);
		}

		public void InitializeMOGServer()
		{
			try
			{
				// Check to see if we can init the server?
				if (!MOG_MainCS.Init_Server(mMogIni, "Server"))
				{
					string message = "Server was unable to initialize because there is another server already running.";
					throw (new Exception(message));
				}
			}
			catch (Exception e)
			{
				string message = "";
				if (e.Message.IndexOf("Only one usage of each socket") > -1)
				{
					message += "It seems another instance of MOG_Server is running on this machine\n\n" + e.Message;
				}
				else
				{
					message = e.Message;
				}
				throw new Exception(message, e);
			}
		}
		#endregion
		#region Dispose() and InitializeComponent()
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMainSMOG));
			this.SMOG_Main_TabControl = new System.Windows.Forms.TabControl();
			this.ServerMonitorTabPage = new System.Windows.Forms.TabPage();
			this.lviewMonitor = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
			this.ConnectionsContextMenu = new System.Windows.Forms.ContextMenu();
			this.ConnectionKillMenuItem = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.ConnectionRetaskCommandMenuItem = new System.Windows.Forms.MenuItem();
			this.ConnectionLaunchMenuItem = new System.Windows.Forms.MenuItem();
			this.ConnectionLaunchTestMenuItem = new System.Windows.Forms.MenuItem();
			this.ServerLoLocksTabPage = new System.Windows.Forms.TabPage();
			this.LocksLeftPanel = new System.Windows.Forms.Panel();
			this.LocksRequestLocksListView = new System.Windows.Forms.ListView();
			this.RequestLockTypeColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.RequestLockNameColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.RequestLockIpColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.RequestLockTimeColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.RequestLockMachineColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.RequestLockUserColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.RequestLockNetIdColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.RequestLockDescriptionColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.LocksImageList = new System.Windows.Forms.ImageList(this.components);
			this.LocksSplitter = new System.Windows.Forms.Splitter();
			this.LocksListView = new System.Windows.Forms.ListView();
			this.LocksTypeColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.LocksNameColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.LocksIpColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.LocksTimeColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.LocksMachineColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.LocksUserColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.LocksNetIdColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.LocksDescriptionColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.LocksContextMenu = new System.Windows.Forms.ContextMenu();
			this.LocksKillLockMenuItem = new System.Windows.Forms.MenuItem();
			this.ServerCommandsTabPage = new System.Windows.Forms.TabPage();
			this.CommandspendingListView = new System.Windows.Forms.ListView();
			this.CommandsPendingTypeColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.CommandsPendingNameColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.CommandsPendingMachineColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.CommandsPendingIpColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.CommandsPendingCommandIdColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.CommandsPendingIdColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.CommandsPendingTimeColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.CommandsPendingSlaveColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.CommandsContextMenu = new System.Windows.Forms.ContextMenu();
			this.CommandsKillMenuItem = new System.Windows.Forms.MenuItem();
			this.CommandImageList = new System.Windows.Forms.ImageList(this.components);
			this.MainTabImageList = new System.Windows.Forms.ImageList(this.components);
			this.SMOGGlobalImageList = new System.Windows.Forms.ImageList(this.components);
			this.colMachineName = new System.Windows.Forms.ColumnHeader();
			this.colIpAddress = new System.Windows.Forms.ColumnHeader();
			this.colNetworkId = new System.Windows.Forms.ColumnHeader();
			this.colConnectionType = new System.Windows.Forms.ColumnHeader();
			this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader8 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader9 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader10 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader12 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader13 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader14 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader15 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader16 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader17 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader18 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader19 = new System.Windows.Forms.ColumnHeader();
			this.IdleSlaveTimer = new System.Windows.Forms.Timer(this.components);
			this.RequestLocksContextMenu = new System.Windows.Forms.ContextMenu();
			this.helpProvider = new System.Windows.Forms.HelpProvider();
			this.MOGServerToolTip = new System.Windows.Forms.ToolTip(this.components);
			this.ServerProcessNotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
			this.ServerNotifyIconContextMenu = new System.Windows.Forms.ContextMenu();
			this.ServerNotifyOpenMenuItem = new System.Windows.Forms.MenuItem();
			this.ServerNotifySep1MenuItem = new System.Windows.Forms.MenuItem();
			this.ServerNotifySetRepoMenuItem = new System.Windows.Forms.MenuItem();
			this.ServerNotifySetSQLMenuItem = new System.Windows.Forms.MenuItem();
			this.ServerNotifySep2MenuItem = new System.Windows.Forms.MenuItem();
			this.ServerInstallLicenseMenuItem = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.ServerNotifyExitMenuItem = new System.Windows.Forms.MenuItem();
			this.ServerButtonPanel = new System.Windows.Forms.Panel();
			this.ServerCloseButton = new System.Windows.Forms.Button();
			this.ServerStatusBar = new System.Windows.Forms.StatusBar();
			this.SMOG_Main_TabControl.SuspendLayout();
			this.ServerMonitorTabPage.SuspendLayout();
			this.ServerLoLocksTabPage.SuspendLayout();
			this.LocksLeftPanel.SuspendLayout();
			this.ServerCommandsTabPage.SuspendLayout();
			this.ServerButtonPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// SMOG_Main_TabControl
			// 
			this.SMOG_Main_TabControl.Controls.Add(this.ServerMonitorTabPage);
			this.SMOG_Main_TabControl.Controls.Add(this.ServerLoLocksTabPage);
			this.SMOG_Main_TabControl.Controls.Add(this.ServerCommandsTabPage);
			this.SMOG_Main_TabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.SMOG_Main_TabControl.ImageList = this.MainTabImageList;
			this.SMOG_Main_TabControl.Location = new System.Drawing.Point(0, 0);
			this.SMOG_Main_TabControl.Name = "SMOG_Main_TabControl";
			this.SMOG_Main_TabControl.SelectedIndex = 0;
			this.SMOG_Main_TabControl.Size = new System.Drawing.Size(394, 240);
			this.SMOG_Main_TabControl.TabIndex = 1;
			this.SMOG_Main_TabControl.KeyUp += new System.Windows.Forms.KeyEventHandler(this.SMOG_Main_TabControl_KeyUp);
			this.SMOG_Main_TabControl.SelectedIndexChanged += new System.EventHandler(this.SMOG_Main_TabControl_SelectedIndexChanged);
			// 
			// ServerMonitorTabPage
			// 
			this.ServerMonitorTabPage.Controls.Add(this.lviewMonitor);
			this.ServerMonitorTabPage.ImageIndex = 0;
			this.ServerMonitorTabPage.Location = new System.Drawing.Point(4, 23);
			this.ServerMonitorTabPage.Name = "ServerMonitorTabPage";
			this.ServerMonitorTabPage.Size = new System.Drawing.Size(386, 213);
			this.ServerMonitorTabPage.TabIndex = 0;
			this.ServerMonitorTabPage.Text = "Connections";
			// 
			// lviewMonitor
			// 
			this.lviewMonitor.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
			this.lviewMonitor.ContextMenu = this.ConnectionsContextMenu;
			this.lviewMonitor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lviewMonitor.FullRowSelect = true;
			this.lviewMonitor.Location = new System.Drawing.Point(0, 0);
			this.lviewMonitor.Name = "lviewMonitor";
			this.lviewMonitor.Size = new System.Drawing.Size(386, 213);
			this.lviewMonitor.TabIndex = 1;
			this.lviewMonitor.UseCompatibleStateImageBehavior = false;
			this.lviewMonitor.View = System.Windows.Forms.View.Details;
			this.lviewMonitor.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lviewMonitor_KeyUp);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Machine Name";
			this.columnHeader1.Width = 200;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "IP Address";
			this.columnHeader2.Width = 100;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Net ID";
			this.columnHeader3.Width = 90;
			// 
			// columnHeader4
			// 
			this.columnHeader4.Text = "Type";
			this.columnHeader4.Width = 70;
			// 
			// columnHeader5
			// 
			this.columnHeader5.Text = "Information";
			this.columnHeader5.Width = 350;
			// 
			// ConnectionsContextMenu
			// 
			this.ConnectionsContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.ConnectionKillMenuItem,
            this.menuItem1,
            this.ConnectionRetaskCommandMenuItem,
            this.ConnectionLaunchMenuItem,
            this.ConnectionLaunchTestMenuItem});
			// 
			// ConnectionKillMenuItem
			// 
			this.ConnectionKillMenuItem.Index = 0;
			this.ConnectionKillMenuItem.Text = "Kill connection";
			this.ConnectionKillMenuItem.Click += new System.EventHandler(this.ConnectionKillMenuItem_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 1;
			this.menuItem1.Text = "-";
			// 
			// ConnectionRetaskCommandMenuItem
			// 
			this.ConnectionRetaskCommandMenuItem.Index = 2;
			this.ConnectionRetaskCommandMenuItem.Text = "Retask Command";
			this.ConnectionRetaskCommandMenuItem.Click += new System.EventHandler(this.ConnectionRetaskCommandMenuItem_Click);
			// 
			// ConnectionLaunchMenuItem
			// 
			this.ConnectionLaunchMenuItem.Index = 3;
			this.ConnectionLaunchMenuItem.Text = "Launch Slave";
			this.ConnectionLaunchMenuItem.Click += new System.EventHandler(this.ConnectionLaunchMenuItem_Click);
			// 
			// ConnectionLaunchTestMenuItem
			// 
			this.ConnectionLaunchTestMenuItem.Index = 4;
			this.ConnectionLaunchTestMenuItem.Text = "Launch Auto-Test";
			this.ConnectionLaunchTestMenuItem.Click += new System.EventHandler(this.ConnectionLaunchTestMenuItem_Click);
			// 
			// ServerLoLocksTabPage
			// 
			this.ServerLoLocksTabPage.Controls.Add(this.LocksLeftPanel);
			this.ServerLoLocksTabPage.ImageIndex = 3;
			this.ServerLoLocksTabPage.Location = new System.Drawing.Point(4, 23);
			this.ServerLoLocksTabPage.Name = "ServerLoLocksTabPage";
			this.ServerLoLocksTabPage.Size = new System.Drawing.Size(386, 213);
			this.ServerLoLocksTabPage.TabIndex = 2;
			this.ServerLoLocksTabPage.Text = "Locks";
			// 
			// LocksLeftPanel
			// 
			this.LocksLeftPanel.Controls.Add(this.LocksRequestLocksListView);
			this.LocksLeftPanel.Controls.Add(this.LocksSplitter);
			this.LocksLeftPanel.Controls.Add(this.LocksListView);
			this.LocksLeftPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.LocksLeftPanel.Location = new System.Drawing.Point(0, 0);
			this.LocksLeftPanel.Name = "LocksLeftPanel";
			this.LocksLeftPanel.Size = new System.Drawing.Size(386, 213);
			this.LocksLeftPanel.TabIndex = 0;
			// 
			// LocksRequestLocksListView
			// 
			this.LocksRequestLocksListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.RequestLockTypeColumnHeader,
            this.RequestLockNameColumnHeader,
            this.RequestLockIpColumnHeader,
            this.RequestLockTimeColumnHeader,
            this.RequestLockMachineColumnHeader,
            this.RequestLockUserColumnHeader,
            this.RequestLockNetIdColumnHeader,
            this.RequestLockDescriptionColumnHeader});
			this.LocksRequestLocksListView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.LocksRequestLocksListView.Location = new System.Drawing.Point(0, 400);
			this.LocksRequestLocksListView.Name = "LocksRequestLocksListView";
			this.LocksRequestLocksListView.Size = new System.Drawing.Size(386, 0);
			this.LocksRequestLocksListView.SmallImageList = this.LocksImageList;
			this.LocksRequestLocksListView.TabIndex = 2;
			this.LocksRequestLocksListView.UseCompatibleStateImageBehavior = false;
			this.LocksRequestLocksListView.View = System.Windows.Forms.View.Details;
			// 
			// RequestLockTypeColumnHeader
			// 
			this.RequestLockTypeColumnHeader.Text = "Type";
			this.RequestLockTypeColumnHeader.Width = 97;
			// 
			// RequestLockNameColumnHeader
			// 
			this.RequestLockNameColumnHeader.Text = "Lock Name";
			this.RequestLockNameColumnHeader.Width = 381;
			// 
			// RequestLockIpColumnHeader
			// 
			this.RequestLockIpColumnHeader.Text = "IP Address";
			this.RequestLockIpColumnHeader.Width = 90;
			// 
			// RequestLockTimeColumnHeader
			// 
			this.RequestLockTimeColumnHeader.Text = "Time";
			this.RequestLockTimeColumnHeader.Width = 144;
			// 
			// RequestLockMachineColumnHeader
			// 
			this.RequestLockMachineColumnHeader.Text = "Machine";
			this.RequestLockMachineColumnHeader.Width = 80;
			// 
			// RequestLockUserColumnHeader
			// 
			this.RequestLockUserColumnHeader.Text = "User";
			// 
			// RequestLockNetIdColumnHeader
			// 
			this.RequestLockNetIdColumnHeader.Text = "Net ID";
			this.RequestLockNetIdColumnHeader.Width = 50;
			// 
			// RequestLockDescriptionColumnHeader
			// 
			this.RequestLockDescriptionColumnHeader.Text = "Description";
			this.RequestLockDescriptionColumnHeader.Width = 290;
			// 
			// LocksImageList
			// 
			this.LocksImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("LocksImageList.ImageStream")));
			this.LocksImageList.TransparentColor = System.Drawing.Color.Transparent;
			this.LocksImageList.Images.SetKeyName(0, "");
			this.LocksImageList.Images.SetKeyName(1, "");
			// 
			// LocksSplitter
			// 
			this.LocksSplitter.Dock = System.Windows.Forms.DockStyle.Top;
			this.LocksSplitter.Location = new System.Drawing.Point(0, 392);
			this.LocksSplitter.Name = "LocksSplitter";
			this.LocksSplitter.Size = new System.Drawing.Size(386, 8);
			this.LocksSplitter.TabIndex = 1;
			this.LocksSplitter.TabStop = false;
			// 
			// LocksListView
			// 
			this.LocksListView.AllowColumnReorder = true;
			this.LocksListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.LocksTypeColumnHeader,
            this.LocksNameColumnHeader,
            this.LocksIpColumnHeader,
            this.LocksTimeColumnHeader,
            this.LocksMachineColumnHeader,
            this.LocksUserColumnHeader,
            this.LocksNetIdColumnHeader,
            this.LocksDescriptionColumnHeader});
			this.LocksListView.ContextMenu = this.LocksContextMenu;
			this.LocksListView.Dock = System.Windows.Forms.DockStyle.Top;
			this.LocksListView.FullRowSelect = true;
			this.LocksListView.Location = new System.Drawing.Point(0, 0);
			this.LocksListView.Name = "LocksListView";
			this.LocksListView.Size = new System.Drawing.Size(386, 392);
			this.LocksListView.SmallImageList = this.LocksImageList;
			this.LocksListView.TabIndex = 0;
			this.LocksListView.UseCompatibleStateImageBehavior = false;
			this.LocksListView.View = System.Windows.Forms.View.Details;
			// 
			// LocksTypeColumnHeader
			// 
			this.LocksTypeColumnHeader.Text = "Type";
			this.LocksTypeColumnHeader.Width = 96;
			// 
			// LocksNameColumnHeader
			// 
			this.LocksNameColumnHeader.Text = "Asset Name";
			this.LocksNameColumnHeader.Width = 389;
			// 
			// LocksIpColumnHeader
			// 
			this.LocksIpColumnHeader.Text = "IP Address";
			this.LocksIpColumnHeader.Width = 90;
			// 
			// LocksTimeColumnHeader
			// 
			this.LocksTimeColumnHeader.Text = "Time";
			this.LocksTimeColumnHeader.Width = 126;
			// 
			// LocksMachineColumnHeader
			// 
			this.LocksMachineColumnHeader.Text = "Machine";
			this.LocksMachineColumnHeader.Width = 80;
			// 
			// LocksUserColumnHeader
			// 
			this.LocksUserColumnHeader.Text = "User";
			// 
			// LocksNetIdColumnHeader
			// 
			this.LocksNetIdColumnHeader.Text = "Net ID";
			this.LocksNetIdColumnHeader.Width = 50;
			// 
			// LocksDescriptionColumnHeader
			// 
			this.LocksDescriptionColumnHeader.Text = "Description";
			this.LocksDescriptionColumnHeader.Width = 240;
			// 
			// LocksContextMenu
			// 
			this.LocksContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.LocksKillLockMenuItem});
			// 
			// LocksKillLockMenuItem
			// 
			this.LocksKillLockMenuItem.Index = 0;
			this.LocksKillLockMenuItem.Text = "Kill Lock";
			this.LocksKillLockMenuItem.Click += new System.EventHandler(this.LocksKillLockMenuItem_Click);
			// 
			// ServerCommandsTabPage
			// 
			this.ServerCommandsTabPage.Controls.Add(this.CommandspendingListView);
			this.ServerCommandsTabPage.ImageIndex = 5;
			this.ServerCommandsTabPage.Location = new System.Drawing.Point(4, 23);
			this.ServerCommandsTabPage.Name = "ServerCommandsTabPage";
			this.ServerCommandsTabPage.Size = new System.Drawing.Size(386, 213);
			this.ServerCommandsTabPage.TabIndex = 3;
			this.ServerCommandsTabPage.Text = "Commands";
			// 
			// CommandspendingListView
			// 
			this.CommandspendingListView.AllowColumnReorder = true;
			this.CommandspendingListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.CommandsPendingTypeColumnHeader,
            this.CommandsPendingNameColumnHeader,
            this.CommandsPendingMachineColumnHeader,
            this.CommandsPendingIpColumnHeader,
            this.CommandsPendingCommandIdColumnHeader,
            this.CommandsPendingIdColumnHeader,
            this.CommandsPendingTimeColumnHeader,
            this.CommandsPendingSlaveColumnHeader});
			this.CommandspendingListView.ContextMenu = this.CommandsContextMenu;
			this.CommandspendingListView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.CommandspendingListView.FullRowSelect = true;
			this.CommandspendingListView.Location = new System.Drawing.Point(0, 0);
			this.CommandspendingListView.MultiSelect = false;
			this.CommandspendingListView.Name = "CommandspendingListView";
			this.CommandspendingListView.Size = new System.Drawing.Size(386, 213);
			this.CommandspendingListView.SmallImageList = this.CommandImageList;
			this.CommandspendingListView.TabIndex = 1;
			this.CommandspendingListView.UseCompatibleStateImageBehavior = false;
			this.CommandspendingListView.View = System.Windows.Forms.View.Details;
			// 
			// CommandsPendingTypeColumnHeader
			// 
			this.CommandsPendingTypeColumnHeader.Text = "Command Type";
			this.CommandsPendingTypeColumnHeader.Width = 180;
			// 
			// CommandsPendingNameColumnHeader
			// 
			this.CommandsPendingNameColumnHeader.Text = "Asset Name";
			this.CommandsPendingNameColumnHeader.Width = 372;
			// 
			// CommandsPendingMachineColumnHeader
			// 
			this.CommandsPendingMachineColumnHeader.Text = "Machine";
			// 
			// CommandsPendingIpColumnHeader
			// 
			this.CommandsPendingIpColumnHeader.Text = "IP Address";
			this.CommandsPendingIpColumnHeader.Width = 25;
			// 
			// CommandsPendingCommandIdColumnHeader
			// 
			this.CommandsPendingCommandIdColumnHeader.Text = "Net ID";
			this.CommandsPendingCommandIdColumnHeader.Width = 56;
			// 
			// CommandsPendingIdColumnHeader
			// 
			this.CommandsPendingIdColumnHeader.Text = "Command ID";
			this.CommandsPendingIdColumnHeader.Width = 25;
			// 
			// CommandsPendingTimeColumnHeader
			// 
			this.CommandsPendingTimeColumnHeader.Text = "Time";
			this.CommandsPendingTimeColumnHeader.Width = 123;
			// 
			// CommandsPendingSlaveColumnHeader
			// 
			this.CommandsPendingSlaveColumnHeader.Text = "Slave";
			this.CommandsPendingSlaveColumnHeader.Width = 90;
			// 
			// CommandsContextMenu
			// 
			this.CommandsContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.CommandsKillMenuItem});
			// 
			// CommandsKillMenuItem
			// 
			this.CommandsKillMenuItem.Index = 0;
			this.CommandsKillMenuItem.Text = "Kill Command";
			this.CommandsKillMenuItem.Click += new System.EventHandler(this.CommandsKillMenuItem_Click);
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
			this.CommandImageList.Images.SetKeyName(7, "");
			// 
			// MainTabImageList
			// 
			this.MainTabImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("MainTabImageList.ImageStream")));
			this.MainTabImageList.TransparentColor = System.Drawing.Color.Transparent;
			this.MainTabImageList.Images.SetKeyName(0, "");
			this.MainTabImageList.Images.SetKeyName(1, "");
			this.MainTabImageList.Images.SetKeyName(2, "");
			this.MainTabImageList.Images.SetKeyName(3, "");
			this.MainTabImageList.Images.SetKeyName(4, "");
			this.MainTabImageList.Images.SetKeyName(5, "");
			this.MainTabImageList.Images.SetKeyName(6, "");
			// 
			// SMOGGlobalImageList
			// 
			this.SMOGGlobalImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("SMOGGlobalImageList.ImageStream")));
			this.SMOGGlobalImageList.TransparentColor = System.Drawing.Color.Transparent;
			this.SMOGGlobalImageList.Images.SetKeyName(0, "");
			this.SMOGGlobalImageList.Images.SetKeyName(1, "");
			this.SMOGGlobalImageList.Images.SetKeyName(2, "");
			this.SMOGGlobalImageList.Images.SetKeyName(3, "");
			// 
			// colMachineName
			// 
			this.colMachineName.Text = "Machine";
			this.colMachineName.Width = 200;
			// 
			// colIpAddress
			// 
			this.colIpAddress.Text = "IP Address";
			this.colIpAddress.Width = 100;
			// 
			// colNetworkId
			// 
			this.colNetworkId.Text = "Net ID";
			this.colNetworkId.Width = 90;
			// 
			// colConnectionType
			// 
			this.colConnectionType.Text = "Connection Type";
			this.colConnectionType.Width = 180;
			// 
			// columnHeader6
			// 
			this.columnHeader6.Text = "Lock Type";
			this.columnHeader6.Width = 180;
			// 
			// columnHeader8
			// 
			this.columnHeader8.Text = "Asset Name";
			this.columnHeader8.Width = 100;
			// 
			// columnHeader9
			// 
			this.columnHeader9.Text = "IP Address";
			// 
			// columnHeader10
			// 
			this.columnHeader10.Text = "Machine";
			// 
			// columnHeader12
			// 
			this.columnHeader12.Text = "Net ID";
			// 
			// columnHeader13
			// 
			this.columnHeader13.Text = "Description";
			this.columnHeader13.Width = 200;
			// 
			// columnHeader14
			// 
			this.columnHeader14.Text = "Lock Type";
			this.columnHeader14.Width = 180;
			// 
			// columnHeader15
			// 
			this.columnHeader15.Text = "Asset Name";
			this.columnHeader15.Width = 100;
			// 
			// columnHeader16
			// 
			this.columnHeader16.Text = "IP Address";
			// 
			// columnHeader17
			// 
			this.columnHeader17.Text = "Machine";
			// 
			// columnHeader18
			// 
			this.columnHeader18.Text = "Net ID";
			// 
			// columnHeader19
			// 
			this.columnHeader19.Text = "Description";
			this.columnHeader19.Width = 200;
			// 
			// IdleSlaveTimer
			// 
			this.IdleSlaveTimer.Enabled = true;
			this.IdleSlaveTimer.Interval = 4000;
			this.IdleSlaveTimer.Tick += new System.EventHandler(this.IdleSlaveTimer_Tick);
			// 
			// helpProvider
			// 
			this.helpProvider.HelpNamespace = "moghelp.chm";
			// 
			// ServerProcessNotifyIcon
			// 
			this.ServerProcessNotifyIcon.ContextMenu = this.ServerNotifyIconContextMenu;
			this.ServerProcessNotifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("ServerProcessNotifyIcon.Icon")));
			this.ServerProcessNotifyIcon.Text = "MOG Server.  <Right click> here to access other  options";
			this.ServerProcessNotifyIcon.Visible = true;
			this.ServerProcessNotifyIcon.DoubleClick += new System.EventHandler(this.ServerProcessNotifyIcon_DoubleClick);
			// 
			// ServerNotifyIconContextMenu
			// 
			this.ServerNotifyIconContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.ServerNotifyOpenMenuItem,
            this.ServerNotifySep1MenuItem,
            this.ServerNotifySetRepoMenuItem,
            this.ServerNotifySetSQLMenuItem,
            this.ServerNotifySep2MenuItem,
            this.ServerInstallLicenseMenuItem,
            this.menuItem2,
            this.ServerNotifyExitMenuItem});
			// 
			// ServerNotifyOpenMenuItem
			// 
			this.ServerNotifyOpenMenuItem.Index = 0;
			this.ServerNotifyOpenMenuItem.Text = "Open...";
			this.ServerNotifyOpenMenuItem.Click += new System.EventHandler(this.ServerNotifyOpenMenuItem_Click);
			// 
			// ServerNotifySep1MenuItem
			// 
			this.ServerNotifySep1MenuItem.Index = 1;
			this.ServerNotifySep1MenuItem.Text = "-";
			// 
			// ServerNotifySetRepoMenuItem
			// 
			this.ServerNotifySetRepoMenuItem.Index = 2;
			this.ServerNotifySetRepoMenuItem.Text = "Set MOG Repository";
			this.ServerNotifySetRepoMenuItem.Click += new System.EventHandler(this.ServerNotifySetRepoMenuItem_Click);
			// 
			// ServerNotifySetSQLMenuItem
			// 
			this.ServerNotifySetSQLMenuItem.Index = 3;
			this.ServerNotifySetSQLMenuItem.Text = "Set SQL Connection";
			this.ServerNotifySetSQLMenuItem.Click += new System.EventHandler(this.ServerNotifySetSQLMenuItem_Click);
			// 
			// ServerNotifySep2MenuItem
			// 
			this.ServerNotifySep2MenuItem.Index = 4;
			this.ServerNotifySep2MenuItem.Text = "-";
			// 
			// ServerInstallLicenseMenuItem
			// 
			this.ServerInstallLicenseMenuItem.Index = 5;
			this.ServerInstallLicenseMenuItem.Text = "Install/Update License...";
			this.ServerInstallLicenseMenuItem.Click += new System.EventHandler(this.ServerInstallLicenseMenuItem_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 6;
			this.menuItem2.Text = "-";
			// 
			// ServerNotifyExitMenuItem
			// 
			this.ServerNotifyExitMenuItem.Index = 7;
			this.ServerNotifyExitMenuItem.Text = "Shutdown";
			this.ServerNotifyExitMenuItem.Click += new System.EventHandler(this.ServerNotifyExitMenuItem_Click);
			// 
			// ServerButtonPanel
			// 
			this.ServerButtonPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.ServerButtonPanel.Controls.Add(this.ServerCloseButton);
			this.ServerButtonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.ServerButtonPanel.Location = new System.Drawing.Point(0, 240);
			this.ServerButtonPanel.Name = "ServerButtonPanel";
			this.ServerButtonPanel.Size = new System.Drawing.Size(394, 32);
			this.ServerButtonPanel.TabIndex = 2;
			// 
			// ServerCloseButton
			// 
			this.ServerCloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.ServerCloseButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.ServerCloseButton.Location = new System.Drawing.Point(282, 2);
			this.ServerCloseButton.Name = "ServerCloseButton";
			this.ServerCloseButton.Size = new System.Drawing.Size(104, 23);
			this.ServerCloseButton.TabIndex = 0;
			this.ServerCloseButton.Text = "Close";
			this.ServerCloseButton.Click += new System.EventHandler(this.ServerCloseButton_Click);
			// 
			// ServerStatusBar
			// 
			this.ServerStatusBar.Location = new System.Drawing.Point(0, 272);
			this.ServerStatusBar.Name = "ServerStatusBar";
			this.ServerStatusBar.Size = new System.Drawing.Size(394, 22);
			this.ServerStatusBar.TabIndex = 3;
			// 
			// FormMainSMOG
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(394, 294);
			this.ControlBox = false;
			this.Controls.Add(this.SMOG_Main_TabControl);
			this.Controls.Add(this.ServerButtonPanel);
			this.Controls.Add(this.ServerStatusBar);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(400, 300);
			this.Name = "FormMainSMOG";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "SMOG";
			this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
			this.Closing += new System.ComponentModel.CancelEventHandler(this.FormMainSMOG_Closing);
			this.Load += new System.EventHandler(this.FormMainSMOG_Load);
			this.SMOG_Main_TabControl.ResumeLayout(false);
			this.ServerMonitorTabPage.ResumeLayout(false);
			this.ServerLoLocksTabPage.ResumeLayout(false);
			this.LocksLeftPanel.ResumeLayout(false);
			this.ServerCommandsTabPage.ResumeLayout(false);
			this.ServerButtonPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
		#endregion
		#region Main

		[DllImport("user32.dll")] private static extern 
			bool SetForegroundWindow(IntPtr hWnd);
		[DllImport("user32.dll")] private static extern 
			bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
		[DllImport("user32.dll")] private static extern 
			bool IsIconic(IntPtr hWnd);

		private const int SW_HIDE = 0;
		private const int SW_SHOWNORMAL = 1;
		private const int SW_SHOWMINIMIZED = 2;
		private const int SW_SHOWMAXIMIZED = 3;
		private const int SW_SHOWNOACTIVATE = 4;
		private const int SW_RESTORE = 9;
		private const int SW_SHOWDEFAULT = 10;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args) 
		{
            Control.CheckForIllegalCrossThreadCalls = false;
            
            try
			{
//				Process smogProcess = RunningInstance();
//				if (smogProcess != null)
//				{
//					//Utils.ShowMessageBoxExclamation("A copy of the MOG Server is already running on this machine", "Duplicate Server Instance");
//
//					// get the window handle
//					IntPtr hWnd = smogProcess.MainWindowHandle;
//
//					// if iconic, we need to restore the window
//					if (IsIconic(hWnd))
//					{
//						ShowWindowAsync(hWnd, SW_RESTORE);
//					}
//
//					// bring it to the foreground
//					SetForegroundWindow(hWnd);
//					return;
//				}

				Application.Run(new FormMainSMOG(args));
			}
			catch(Exception e)
			{
				MOG_Report.ReportMessage("Mog Initialization Error", e.Message, e.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.ERROR);
			}
		}

//		public static Process RunningInstance() 
//		{ 
//			Process current = Process.GetCurrentProcess(); 
//			Process[] processes = Process.GetProcessesByName (current.ProcessName); 
//
//			//Loop through the running processes in with the same name 
//			foreach (Process process in processes) 
//			{ 
//				//Ignore the current process 
//				if (process.Id != current.Id) 
//				{ 
//					//Make sure that the process is running from the exe file. 
//					if (Assembly.GetExecutingAssembly().Location.
//						Replace("/", "\\") == current.MainModule.FileName) 
// 
//					{  
//						//Return the other process instance.  
//						return process; 
// 
//					}  
//				}  
//			} 
//			//No other instance was found, return null.  
//			return null;  
//		}

		#endregion
		#region Private functions
		/// <summary>
		/// Main idle routine
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MogProcess()
		{
			// Loop until we shutdown
			while(!MOG_Main.IsShutdown())
			{
				// Only process while we are not shutting down?
				if (!MOG_Main.IsShutdownInProgress())
				{
					try
					{
						MOG_Main.Process();
					}
					catch(Exception ex)
					{
						MOG_Report.ReportSilent("MogProcess", ex.Message, ex.StackTrace, MOG_ALERT_LEVEL.CRITICAL);
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

		private string DoInitialRepositoryLogic(string initialIni)
		{
			string repository = MOG_Main.GetDefaultRepositoryPath(initialIni);
			initialIni = MOG_Main.GetDefaultSystemConfigFile(initialIni);

			// Check if this file is missing?
			string file = MOG_Tokens.GetFormattedString(initialIni, "{SystemRepositoryPath}=" + repository);
			if (!DosUtils.FileExist(file))
			{
				// Show the user the Repository Selection Dialog
				MOGIniSelectorForm iniSelect = new MOGIniSelectorForm();
				iniSelect.LoadShallowRepositories();
				if (initialIni.Length > 0)
				{
					iniSelect.AddRepository("Default", Path.GetDirectoryName(Path.GetDirectoryName(initialIni)));
				}
				
				if (iniSelect.ShowDialog() == DialogResult.OK)
				{
					initialIni = iniSelect.SelectedINIPath;
				}
				else
				{
					// Indicate we failed to make a valid selection
					return "";
				}
			}

			return MOG_Tokens.GetFormattedString(initialIni, "{SystemRepositoryPath}=" + repository);
		}

		private string BrowseRepositories(string initialIni)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			//ofd.InitialDirectory = Path.GetDirectoryName(initialIni);
			if (ofd.ShowDialog() == DialogResult.OK)
				initialIni = ofd.FileName;

			return initialIni;
			// Show the user the Repository Selection Dialog
//			MOGIniSelectorForm iniSelect = new MOGIniSelectorForm();
//			//iniSelect.AddRepository("Default", Path.GetDirectoryName(Path.GetDirectoryName(initialIni)));
//			if (iniSelect.ShowDialog() == DialogResult.OK)
//			{
//				initialIni = iniSelect.SelectedINIPath;
//			}
//
//			return initialIni;
		}

		#endregion		
		#region Event handlers

		//override the windows messaging so we can catch a windows shutdown event.
		protected override void WndProc(ref System.Windows.Forms.Message m)
		{
			if (m.Msg == WM_QUERYENDSESSION)
			{
				MOG_Main.Shutdown();
			}
			base.WndProc(ref m);
		}



		/// <summary>
		/// Tab switching event.  Handle all initialization of tab specific class objects
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SMOG_Main_TabControl_SelectedIndexChanged(object sender, System.EventArgs e)
		{			
			// better way
			if (SMOG_Main_TabControl.SelectedTab == this.ServerMonitorTabPage)
			{
				mMonitor.DisplayUpdateExistingConnections();
			}
			else if (SMOG_Main_TabControl.SelectedTab == this.ServerLoLocksTabPage)
			{
				if (mLocks == null)
				{
					mLocks = new guiLocks(this);
				}
				else
				{
					mLocks.Initialize();
				}
			}
			else if (SMOG_Main_TabControl.SelectedTab == this.ServerCommandsTabPage)
			{
				if (mPendingCommands == null)
				{
					mPendingCommands = new guiPendingCommands(this);
				}
				else
				{
					mPendingCommands.RefreshWindow(null);
				}
			}
		}
		
		/// <summary>
		/// Event fired when we are closing.  Save the user prefs
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FormMainSMOG_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// Save the server's settings
			mUserPrefs.Save(this);

			// Time to exit the application
			Shutdown();
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

		private void CommandsSnapShotButton_Click(object sender, System.EventArgs e)
		{
			mPendingCommands.GetCommandSnapShot();
		}

		private void IdleSlaveTimer_Tick(object sender, System.EventArgs e)
		{
			if (mMonitor != null)
				mMonitor.DisplayRefreshSlaveStatus();
		}

		private void ConnectionKillMenuItem_Click(object sender, System.EventArgs e)
		{
			foreach (ListViewItem item in this.lviewMonitor.SelectedItems)
			{
				mMonitor.KillConnection(item.SubItems[(int)guiMonitor.MONITOR_TABS.ID].Text);
			}
		}

		private void LocksKillLockMenuItem_Click(object sender, System.EventArgs e)
		{
			foreach (ListViewItem item in this.LocksListView.SelectedItems)
			{
				string name = item.SubItems[(int)guiLocks.LOCK_COLUMNS.FULLNAME].Text;
				string type = item.SubItems[(int)guiLocks.LOCK_COLUMNS.FULL_TYPE].Text;
				string user = item.SubItems[(int)guiLocks.LOCK_COLUMNS.USER].Text;
				string cpu  = item.SubItems[(int)guiLocks.LOCK_COLUMNS.MACHINE].Text;
				mLocks.KillLock(name, type, user, cpu);
			}
		}


		private void lviewMonitor_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if(e.KeyData == Keys.F5)
			{
				//Refresh the window
				switch(SMOG_Main_TabControl.SelectedTab.Name)
				{
					case "ServerMonitorTabPage": //(int)MAIN_PAGES.SMOG_CONNECTIONS:
						mMonitor.DisplayUpdateExistingConnections();
						break;
					case "ServerCommandsTabPage": //(int)MAIN_PAGES.SMOG_COMMANDS:
						if (mPendingCommands != null)
						{
							mPendingCommands.RefreshWindow(null);
						}
						break;
				}
			}
		}

		private void ConnectionLaunchMenuItem_Click(object sender, System.EventArgs e)
		{
			foreach (ListViewItem item in this.lviewMonitor.SelectedItems)
			{
				MOG_ControllerSystem.LaunchSlave(Convert.ToInt32(item.SubItems[(int)guiMonitor.MONITOR_TABS.ID].Text));
				//mMonitor.KillConnection();
			}			
		}

		private void ConnectionLaunchTestMenuItem_Click(object sender, System.EventArgs e)
		{
			MOGAutoTest testForm = new MOGAutoTest();
			if (testForm.ShowDialog(this) == DialogResult.OK)
			{
				List<string> computerNames = new List<string>();
				List<string> networkIds = new List<string>();

				foreach (ListViewItem item in lviewMonitor.SelectedItems)
				{
					computerNames.Add(item.SubItems[(int)guiMonitor.MONITOR_TABS.MACHINE_NAME].Text);
					networkIds.Add(item.SubItems[(int)guiMonitor.MONITOR_TABS.ID].Text);
				}
				
				List<object> args = new List<object>();
				args.Add(testForm.AutoTestName);
				args.Add(testForm.AutoTestProject);
				args.Add(testForm.AutoTestFile);
				args.Add(testForm.AutoTestDuration);
				args.Add(testForm.AutoTestDelay);
				args.Add(testForm.AutoTestCopyFileLocal);
				args.Add(testForm.AutoTestImportStartIndex);
				args.Add(computerNames);
				args.Add(networkIds);

				ProgressDialog progress = new ProgressDialog("Initiating Automatted Testing Mode", "Please wait while the server incrementally sends commands to the Clients", ConnectionLaunchTest_Worker, args, true);
				progress.ShowDialog();
			}
		}

		private void ConnectionLaunchTest_Worker(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			List<object> args = e.Argument as List<object>;
			
			string testName = args[0] as string;
			string projectName = args[1] as string;
			string importAsset = args[2] as string;
			int duration = (int)args[3];
			int rampTime = (int)args[4];
			bool bCopyFileLocal = (bool)args[5];
			int startingExtensionIndex = (int)args[6];
			List<string> computerNames = args[7] as List<string>;
			List<string> networkIds = args[8] as List<string>;

			int delay = (rampTime * 1000) / computerNames.Count;

			// Get the Server's CommandManager?
			MOG_CommandServerCS cmdServer = MOG_ControllerSystem.GetCommandManager() as MOG_CommandServerCS;
			if (cmdServer != null)
			{
				// Loop through all the selected connections
				for (int i = 0; i < computerNames.Count && !worker.CancellationPending; i++)
				{
					// Update message and check for cancelation
					worker.ReportProgress(i * 100 / computerNames.Count, "Initiating Client on Machine Name: " + computerNames[i]);

					// Initiate the Automated Testing on each selected connection
					int clientNetworkID = Convert.ToInt32(networkIds[i]);
					cmdServer.InitiateAutomatedTesting(clientNetworkID, testName, projectName, importAsset, duration, bCopyFileLocal, startingExtensionIndex);

					// Wait for the desired dalay
					Thread.Sleep(delay);
				}
			}
		}

		private void SMOG_Main_TabControl_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if(e.KeyData == Keys.F5)
			{
				//Refresh the window
				switch(SMOG_Main_TabControl.SelectedTab.Name)
				{
					case "ServerMonitorTabPage":
						mMonitor.DisplayUpdateExistingConnections();
						break;
					case "ServerLoLocksTabPage":
						if (mLocks!= null)
						{
							mLocks.Initialize();
						}
						break;
					case "ServerCommandsTabPage":
						if (mPendingCommands != null)
						{
							mPendingCommands.RefreshWindow(null);
						}
						break;
				}
			}
		}
			
		private void CommandsKillMenuItem_Click(object sender, System.EventArgs e)
		{
			mPendingCommands.KillCommand();
		}

		private void ConnectionRetaskCommandMenuItem_Click(object sender, System.EventArgs e)
		{
			mMonitor.RetaskCommand();
		}

		private void FormMainSMOG_Load(object sender, System.EventArgs e)
		{
		
		}

		#endregion

		private void ServerNotifyExitMenuItem_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void ServerCloseButton_Click(object sender, System.EventArgs e)
		{
			this.WindowState = FormWindowState.Minimized;
			this.ShowInTaskbar = false;
			MOG_Prompt.SetMode(MOG_PROMPT_MODE_TYPE.MOG_PROMPT_SILENT);
		}
		private void ServerNotifyOpenMenuItem_Click(object sender, System.EventArgs e)
		{
			MOG_Prompt.ClearMode(MOG_PROMPT_MODE_TYPE.MOG_PROMPT_SILENT);
			this.WindowState = FormWindowState.Normal;
			this.ShowInTaskbar = true;
		}
		private void ServerProcessNotifyIcon_DoubleClick(object sender, System.EventArgs e)
		{
			MOG_Prompt.ClearMode(MOG_PROMPT_MODE_TYPE.MOG_PROMPT_SILENT);
			this.WindowState = FormWindowState.Normal;
			this.ShowInTaskbar = true;
		}

		private void ServerNotifySetRepoMenuItem_Click(object sender, System.EventArgs e)
		{
			MOG_Prompt.ClearMode(MOG_PROMPT_MODE_TYPE.MOG_PROMPT_SILENT);
			bool bRepositorySelected = false;

			while (!bRepositorySelected)
			{
				MogForm_RepositoryBrowser_Server reposForm = new MogForm_RepositoryBrowser_Server();
				reposForm.ShowDialog(this);
				if (reposForm.DialogResult == DialogResult.OK)
				{
					bRepositorySelected = true;
					
					//Warn the user if the repository was on a local drive
					string drive = Path.GetPathRoot(reposForm.SelectedPath);
					int type = (int)GetDriveType(drive);
					if (type != DRIVE_TYPE_NETWORK)
					{
						if (MOG_Prompt.PromptResponse("Local Drive Warning", "It is not recommended to place a MOG Repository on a local drive because it will not be accessible to other users on the network.", Environment.StackTrace, MOGPromptButtons.OKCancel, MOG_ALERT_LEVEL.ALERT) == MOGPromptResult.Cancel)
							bRepositorySelected = false;
					}
					
					if (bRepositorySelected)
					{
						// Inform the server about the new Repository
						MOG_ControllerSystem.ChangeRepository(reposForm.SelectedPath);
					}
				}
				else
				{
					break;
				}
			}
			MOG_Prompt.SetMode(MOG_PROMPT_MODE_TYPE.MOG_PROMPT_SILENT);
		}

		private void ServerNotifySetSQLMenuItem_Click(object sender, System.EventArgs e)
		{
			MOG_Prompt.ClearMode(MOG_PROMPT_MODE_TYPE.MOG_PROMPT_SILENT);
			SQLConnectForm sqlForm = new SQLConnectForm();
			sqlForm.ShowDialog(this);
			if (sqlForm.DialogResult == DialogResult.OK)
			{
				// Inform the server about the new SQL Connection
				MOG_ControllerSystem.ChangeSQLConnection(sqlForm.ConnectionString);
			}
			MOG_Prompt.SetMode(MOG_PROMPT_MODE_TYPE.MOG_PROMPT_SILENT);
		}

		private void ServerInstallLicenseMenuItem_Click(object sender, EventArgs e)
		{
			InstallUpdateLicenseForm install = new InstallUpdateLicenseForm();
			install.Show();			
		}						
	}
}



