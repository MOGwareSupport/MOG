using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;

using MOG;
using MOG.INI;
using MOG.PROMPT;
using MOG.REPORT;
using MOG.PROGRESS;
using MOG.PROJECT;
using MOG.DOSUTILS;
using MOG.FILENAME;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERDEMOPROJECT;
using MOG.DATABASE;
using MOG.PROPERTIES;

using MOG_ControlsLibrary;
using MOG_ServerManager.Utilities;
using AppLoading;

using MOG_ControlsLibrary.Utils;
using MOG_ControlsLibrary.Forms;
using MOG_CoreControls.MogUtil_DepthManager;
using MOG_CoreControls;
using System.Collections.Generic;

namespace MOG_ServerManager
{
	/// <summary>
	/// Main form of the Server Manager
	/// </summary>
	public class MOG_ServerManagerMainForm: System.Windows.Forms.Form
	{
		[DllImport("kernel32.dll")]
		public static extern long GetDriveType(string driveLetter);
		private const int DRIVE_TYPE_FLOPPY		= 2;
		private const int DRIVE_TYPE_HD			= 3;
		private const int DRIVE_TYPE_NETWORK	= 4;
		private const int DRIVE_TYPE_CD			= 5;
		private const int DRIVE_TYPE_RAM		= 6;
		public TrashPolicyEditorControl trashPolicyEditorControl1;

		#region System defs

		private System.Windows.Forms.Panel VersionManagerMainPanel;
		private System.Windows.Forms.Panel VersionManagerTopPanel;
		private System.Windows.Forms.Panel VersionClientServerPanel;
		public System.Windows.Forms.ListView ClientListView;
		private System.Windows.Forms.Splitter VersionSplitter;
		public System.Windows.Forms.ListView ServerListView;
		private System.Windows.Forms.Label VersionManagerTopLabel;
		private System.Windows.Forms.Splitter VersionManagerHorizSplitter;
		private System.Windows.Forms.Panel VersionDescriptionPanel;
		public System.Windows.Forms.ListView VersionFilesListView;
		private System.Windows.Forms.Splitter VersionFilesSplitter;
		public System.Windows.Forms.RichTextBox VersionDescription;
		private System.Windows.Forms.Label DescriptionTitle;
		private System.Windows.Forms.ColumnHeader VersionClientNameColumnHeader;
		private System.Windows.Forms.ColumnHeader VersionClientVersionColumnHeader;
		private System.Windows.Forms.ColumnHeader VersionServerNameColumnHeader;
		private System.Windows.Forms.ColumnHeader VersionServerVersionColumnHeader;
		private System.Windows.Forms.ColumnHeader VersionsFIlesNameColumnHeader;
		private System.Windows.Forms.ColumnHeader VersionsFIlesDateColumnHeader;
		private System.Windows.Forms.ColumnHeader VersionsFIlesSizeColumnHeader;
		public System.Windows.Forms.Button VersionDeployButton;
		private System.Windows.Forms.Button VersionRefreshButton;
		private System.ComponentModel.IContainer components;
		public System.Windows.Forms.ListView lvProjects;
		private System.Windows.Forms.ColumnHeader colProject;
		private System.Windows.Forms.ColumnHeader colLocation;
		private System.Windows.Forms.ColumnHeader colUsers;
		private System.Windows.Forms.ColumnHeader colUsersLogged;
		private System.Windows.Forms.ColumnHeader colSlaves;
		private System.Windows.Forms.ContextMenuStrip cmProjects;
		public System.Windows.Forms.ToolStripMenuItem miNewProject;
		public System.Windows.Forms.ToolStripMenuItem miDuplicateProject;
		private System.Windows.Forms.ToolStripMenuItem miRemoveProject;
		public System.Windows.Forms.ToolStripMenuItem miConfigureProject;
		private System.Windows.Forms.ToolStripMenuItem miRemovedProjects;
		private System.Windows.Forms.ToolStripSeparator menuItem2;
		private System.Windows.Forms.ToolStripSeparator menuItem3;
		private System.Windows.Forms.ToolStripMenuItem miImportFiles;
		private System.Windows.Forms.MainMenu mainMenu;
		private System.Windows.Forms.MenuItem miMainMOG;
		private System.Windows.Forms.MenuItem miMainProject;
		private System.Windows.Forms.MenuItem miExit;
		private System.Windows.Forms.MenuItem miChangeSQLConnection;
		private System.Windows.Forms.MenuItem miChangeRepository;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.TabPage tpProjects;
		private System.Windows.Forms.MenuItem miCycleServer;
		private System.Windows.Forms.GroupBox gbSystemInfo;
		private System.Windows.Forms.Label lblServerVersion;
		private System.Windows.Forms.Label lblServerVersionLabel;
		private System.Windows.Forms.Label lblSQLServer;
		private System.Windows.Forms.Label lblSQLServerLabel;
		private System.Windows.Forms.Label lblRepositoryLocation;
		private System.Windows.Forms.Label lblReposLocationLabel;
		private System.Windows.Forms.ToolTip ServerManagerToolTip;
		private System.Windows.Forms.ImageList MainTabImageList;
		private System.Windows.Forms.MenuItem miMainView;
		private System.Windows.Forms.MenuItem miViewRefresh;
		public System.Windows.Forms.TabControl MOGMainTabControl;
		private System.Windows.Forms.TabPage tpTrash;
		private System.Windows.Forms.TabPage tpVersions;
		public System.Windows.Forms.ListView lvDemoProjects;
		private System.Windows.Forms.ColumnHeader chDemoProject;
		private System.Windows.Forms.ColumnHeader chDemoProjectDescription;
		private System.Windows.Forms.Button btnImportProjectFromDemo;
		private System.Windows.Forms.Button btnImportDemoFromProject;
		private System.Windows.Forms.StatusBar ServerManagerStatusBar;
		private System.Windows.Forms.TabPage tbStart;
		public System.Windows.Forms.RichTextBox StartPageInfoRichTextBox;
		private System.Windows.Forms.PictureBox StartPictureBox;
		private System.Windows.Forms.PictureBox HelpBarPictureBox;
		private System.Windows.Forms.ContextMenuStrip cmDemoProjects;
		private System.Windows.Forms.ToolStripMenuItem miRemoveDemoProject;
		private System.Windows.Forms.ToolStripMenuItem miExploreDemoProject;
		private System.Windows.Forms.ToolStripMenuItem miImportDemoProject;
		private System.Windows.Forms.ToolStripMenuItem miDemoProjectDetails;
		public System.Windows.Forms.Button VersionImportButton;
		private System.Windows.Forms.OpenFileDialog VersionImportDropDialog;
		private System.Windows.Forms.ContextMenuStrip VersionContextMenu;
		private System.Windows.Forms.ToolStripMenuItem VersionDeleteMenuItem;
		private System.Windows.Forms.Button ProjectImportDemoButton;
		private WebBrowser StartupWebBrowser;
		private TabPage tbLicensing;
		private TabPage tbRemoteServers;
		public MogControl_RemoteServers RemoteServerSettings;
		private System.Windows.Forms.ColumnHeader colPlatforms;


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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MOG_ServerManagerMainForm));
			this.MOGMainTabControl = new System.Windows.Forms.TabControl();
			this.tbStart = new System.Windows.Forms.TabPage();
			this.StartupWebBrowser = new System.Windows.Forms.WebBrowser();
			this.StartPictureBox = new System.Windows.Forms.PictureBox();
			this.tpProjects = new System.Windows.Forms.TabPage();
			this.ProjectImportDemoButton = new System.Windows.Forms.Button();
			this.btnImportDemoFromProject = new System.Windows.Forms.Button();
			this.btnImportProjectFromDemo = new System.Windows.Forms.Button();
			this.lvDemoProjects = new System.Windows.Forms.ListView();
			this.chDemoProject = new System.Windows.Forms.ColumnHeader();
			this.chDemoProjectDescription = new System.Windows.Forms.ColumnHeader();
			this.cmDemoProjects = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.miImportDemoProject = new System.Windows.Forms.ToolStripMenuItem();
			this.miRemoveDemoProject = new System.Windows.Forms.ToolStripMenuItem();
			this.miExploreDemoProject = new System.Windows.Forms.ToolStripMenuItem();
			this.miDemoProjectDetails = new System.Windows.Forms.ToolStripMenuItem();
			this.gbSystemInfo = new System.Windows.Forms.GroupBox();
			this.lblServerVersion = new System.Windows.Forms.Label();
			this.lblServerVersionLabel = new System.Windows.Forms.Label();
			this.lblSQLServer = new System.Windows.Forms.Label();
			this.lblSQLServerLabel = new System.Windows.Forms.Label();
			this.lblRepositoryLocation = new System.Windows.Forms.Label();
			this.lblReposLocationLabel = new System.Windows.Forms.Label();
			this.lvProjects = new System.Windows.Forms.ListView();
			this.colProject = new System.Windows.Forms.ColumnHeader();
			this.colLocation = new System.Windows.Forms.ColumnHeader();
			this.colUsers = new System.Windows.Forms.ColumnHeader();
			this.colUsersLogged = new System.Windows.Forms.ColumnHeader();
			this.colSlaves = new System.Windows.Forms.ColumnHeader();
			this.colPlatforms = new System.Windows.Forms.ColumnHeader();
			this.cmProjects = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.miNewProject = new System.Windows.Forms.ToolStripMenuItem();
			this.miDuplicateProject = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItem2 = new System.Windows.Forms.ToolStripSeparator();
			this.miConfigureProject = new System.Windows.Forms.ToolStripMenuItem();
			this.miImportFiles = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItem3 = new System.Windows.Forms.ToolStripSeparator();
			this.miRemoveProject = new System.Windows.Forms.ToolStripMenuItem();
			this.miRemovedProjects = new System.Windows.Forms.ToolStripMenuItem();
			this.tbLicensing = new System.Windows.Forms.TabPage();
			this.tpVersions = new System.Windows.Forms.TabPage();
			this.VersionImportButton = new System.Windows.Forms.Button();
			this.VersionManagerMainPanel = new System.Windows.Forms.Panel();
			this.VersionManagerTopPanel = new System.Windows.Forms.Panel();
			this.VersionClientServerPanel = new System.Windows.Forms.Panel();
			this.ClientListView = new System.Windows.Forms.ListView();
			this.VersionClientNameColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.VersionClientVersionColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.VersionContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.VersionDeleteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.VersionSplitter = new System.Windows.Forms.Splitter();
			this.ServerListView = new System.Windows.Forms.ListView();
			this.VersionServerNameColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.VersionServerVersionColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.VersionManagerTopLabel = new System.Windows.Forms.Label();
			this.VersionManagerHorizSplitter = new System.Windows.Forms.Splitter();
			this.VersionDescriptionPanel = new System.Windows.Forms.Panel();
			this.VersionFilesListView = new System.Windows.Forms.ListView();
			this.VersionsFIlesNameColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.VersionsFIlesDateColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.VersionsFIlesSizeColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.VersionFilesSplitter = new System.Windows.Forms.Splitter();
			this.VersionDescription = new System.Windows.Forms.RichTextBox();
			this.DescriptionTitle = new System.Windows.Forms.Label();
			this.VersionDeployButton = new System.Windows.Forms.Button();
			this.VersionRefreshButton = new System.Windows.Forms.Button();
			this.tpTrash = new System.Windows.Forms.TabPage();
			this.trashPolicyEditorControl1 = new MOG_ServerManager.TrashPolicyEditorControl();
			this.tbRemoteServers = new System.Windows.Forms.TabPage();
			this.RemoteServerSettings = new MOG_ServerManager.MogControl_RemoteServers();
			this.MainTabImageList = new System.Windows.Forms.ImageList(this.components);
			this.StartPageInfoRichTextBox = new System.Windows.Forms.RichTextBox();
			this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
			this.miMainMOG = new System.Windows.Forms.MenuItem();
			this.miChangeSQLConnection = new System.Windows.Forms.MenuItem();
			this.miChangeRepository = new System.Windows.Forms.MenuItem();
			this.miCycleServer = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.miExit = new System.Windows.Forms.MenuItem();
			this.miMainView = new System.Windows.Forms.MenuItem();
			this.miViewRefresh = new System.Windows.Forms.MenuItem();
			this.miMainProject = new System.Windows.Forms.MenuItem();
			this.ServerManagerToolTip = new System.Windows.Forms.ToolTip(this.components);
			this.ServerManagerStatusBar = new System.Windows.Forms.StatusBar();
			this.VersionImportDropDialog = new System.Windows.Forms.OpenFileDialog();
			this.HelpBarPictureBox = new System.Windows.Forms.PictureBox();
			this.MOGMainTabControl.SuspendLayout();
			this.tbStart.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.StartPictureBox)).BeginInit();
			this.tpProjects.SuspendLayout();
			this.cmDemoProjects.SuspendLayout();
			this.gbSystemInfo.SuspendLayout();
			this.cmProjects.SuspendLayout();
			this.tpVersions.SuspendLayout();
			this.VersionManagerMainPanel.SuspendLayout();
			this.VersionManagerTopPanel.SuspendLayout();
			this.VersionClientServerPanel.SuspendLayout();
			this.VersionContextMenu.SuspendLayout();
			this.VersionDescriptionPanel.SuspendLayout();
			this.tpTrash.SuspendLayout();
			this.tbRemoteServers.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.HelpBarPictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// MOGMainTabControl
			// 
			this.MOGMainTabControl.Controls.Add(this.tbStart);
			this.MOGMainTabControl.Controls.Add(this.tpProjects);
			this.MOGMainTabControl.Controls.Add(this.tbLicensing);
			this.MOGMainTabControl.Controls.Add(this.tpVersions);
			this.MOGMainTabControl.Controls.Add(this.tpTrash);
			this.MOGMainTabControl.Controls.Add(this.tbRemoteServers);
			this.MOGMainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.MOGMainTabControl.ImageList = this.MainTabImageList;
			this.MOGMainTabControl.ItemSize = new System.Drawing.Size(110, 19);
			this.MOGMainTabControl.Location = new System.Drawing.Point(0, 0);
			this.MOGMainTabControl.Name = "MOGMainTabControl";
			this.MOGMainTabControl.SelectedIndex = 0;
			this.MOGMainTabControl.Size = new System.Drawing.Size(754, 370);
			this.MOGMainTabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
			this.MOGMainTabControl.TabIndex = 0;
			this.MOGMainTabControl.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MOG_ServerManagerMainForm_KeyUp);
			this.MOGMainTabControl.SelectedIndexChanged += new System.EventHandler(this.MOGMainTabControl_SelectedIndexChanged);
			// 
			// tbStart
			// 
			this.tbStart.BackColor = System.Drawing.Color.White;
			this.tbStart.Controls.Add(this.StartupWebBrowser);
			this.tbStart.Controls.Add(this.StartPictureBox);
			this.tbStart.Location = new System.Drawing.Point(4, 23);
			this.tbStart.Name = "tbStart";
			this.tbStart.Size = new System.Drawing.Size(746, 343);
			this.tbStart.TabIndex = 3;
			this.tbStart.Text = "Start";
			this.tbStart.UseVisualStyleBackColor = true;
			// 
			// StartupWebBrowser
			// 
			this.StartupWebBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
			this.StartupWebBrowser.Location = new System.Drawing.Point(0, 0);
			this.StartupWebBrowser.MinimumSize = new System.Drawing.Size(20, 20);
			this.StartupWebBrowser.Name = "StartupWebBrowser";
			this.StartupWebBrowser.Size = new System.Drawing.Size(746, 343);
			this.StartupWebBrowser.TabIndex = 0;
			this.StartupWebBrowser.Url = new System.Uri("http://www.mogware.com/Startup/ServerManager", System.UriKind.Absolute);
			this.StartupWebBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.StartupWebBrowser_DocumentCompleted);
			// 
			// StartPictureBox
			// 
			this.StartPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.StartPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.StartPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("StartPictureBox.Image")));
			this.StartPictureBox.Location = new System.Drawing.Point(0, 0);
			this.StartPictureBox.Name = "StartPictureBox";
			this.StartPictureBox.Size = new System.Drawing.Size(746, 343);
			this.StartPictureBox.TabIndex = 0;
			this.StartPictureBox.TabStop = false;
			// 
			// tpProjects
			// 
			this.tpProjects.Controls.Add(this.ProjectImportDemoButton);
			this.tpProjects.Controls.Add(this.btnImportDemoFromProject);
			this.tpProjects.Controls.Add(this.btnImportProjectFromDemo);
			this.tpProjects.Controls.Add(this.lvDemoProjects);
			this.tpProjects.Controls.Add(this.gbSystemInfo);
			this.tpProjects.Controls.Add(this.lvProjects);
			this.tpProjects.ImageIndex = 2;
			this.tpProjects.Location = new System.Drawing.Point(4, 23);
			this.tpProjects.Name = "tpProjects";
			this.tpProjects.Size = new System.Drawing.Size(746, 343);
			this.tpProjects.TabIndex = 2;
			this.tpProjects.Text = "Projects";
			this.tpProjects.UseVisualStyleBackColor = true;
			// 
			// ProjectImportDemoButton
			// 
			this.ProjectImportDemoButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.ProjectImportDemoButton.Enabled = false;
			this.ProjectImportDemoButton.Location = new System.Drawing.Point(600, 312);
			this.ProjectImportDemoButton.Name = "ProjectImportDemoButton";
			this.ProjectImportDemoButton.Size = new System.Drawing.Size(128, 23);
			this.ProjectImportDemoButton.TabIndex = 26;
			this.ProjectImportDemoButton.Text = "Import Demo";
			// 
			// btnImportDemoFromProject
			// 
			this.btnImportDemoFromProject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnImportDemoFromProject.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnImportDemoFromProject.Location = new System.Drawing.Point(306, 275);
			this.btnImportDemoFromProject.Name = "btnImportDemoFromProject";
			this.btnImportDemoFromProject.Size = new System.Drawing.Size(40, 24);
			this.btnImportDemoFromProject.TabIndex = 25;
			this.btnImportDemoFromProject.Text = "-->";
			this.ServerManagerToolTip.SetToolTip(this.btnImportDemoFromProject, "Create a Demo Project from and existing project");
			this.btnImportDemoFromProject.Click += new System.EventHandler(this.btnImportDemoFromProject_Click);
			// 
			// btnImportProjectFromDemo
			// 
			this.btnImportProjectFromDemo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnImportProjectFromDemo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnImportProjectFromDemo.Location = new System.Drawing.Point(306, 243);
			this.btnImportProjectFromDemo.Name = "btnImportProjectFromDemo";
			this.btnImportProjectFromDemo.Size = new System.Drawing.Size(40, 24);
			this.btnImportProjectFromDemo.TabIndex = 24;
			this.btnImportProjectFromDemo.Text = "<--";
			this.ServerManagerToolTip.SetToolTip(this.btnImportProjectFromDemo, "Import a project from a Demo Project");
			this.btnImportProjectFromDemo.Click += new System.EventHandler(this.miImportDemoProject_Click);
			// 
			// lvDemoProjects
			// 
			this.lvDemoProjects.AllowDrop = true;
			this.lvDemoProjects.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lvDemoProjects.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chDemoProject,
            this.chDemoProjectDescription});
			this.lvDemoProjects.ContextMenuStrip = this.cmDemoProjects;
			this.lvDemoProjects.FullRowSelect = true;
			this.lvDemoProjects.Location = new System.Drawing.Point(354, 176);
			this.lvDemoProjects.Name = "lvDemoProjects";
			this.lvDemoProjects.Size = new System.Drawing.Size(376, 123);
			this.lvDemoProjects.TabIndex = 23;
			this.lvDemoProjects.UseCompatibleStateImageBehavior = false;
			this.lvDemoProjects.View = System.Windows.Forms.View.Details;
			this.lvDemoProjects.DragDrop += new System.Windows.Forms.DragEventHandler(this.lvDemoProjects_DragDrop);
			this.lvDemoProjects.DragEnter += new System.Windows.Forms.DragEventHandler(this.lvDemoProjects_DragEnter);
			this.lvDemoProjects.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MOG_ServerManagerMainForm_KeyUp);
			this.lvDemoProjects.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.lvDemoProjects_ItemDrag);
			// 
			// chDemoProject
			// 
			this.chDemoProject.Text = "Demo Project";
			this.chDemoProject.Width = 143;
			// 
			// chDemoProjectDescription
			// 
			this.chDemoProjectDescription.Text = "Description";
			this.chDemoProjectDescription.Width = 200;
			// 
			// cmDemoProjects
			// 
			this.cmDemoProjects.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miImportDemoProject,
            this.miRemoveDemoProject,
            this.miExploreDemoProject,
            this.miDemoProjectDetails});
			this.cmDemoProjects.Name = "cmDemoProjects";
			this.cmDemoProjects.Size = new System.Drawing.Size(147, 92);
			// 
			// miImportDemoProject
			// 
			this.miImportDemoProject.Name = "miImportDemoProject";
			this.miImportDemoProject.Size = new System.Drawing.Size(146, 22);
			this.miImportDemoProject.Text = "Import";
			this.miImportDemoProject.Click += new System.EventHandler(this.miImportDemoProject_Click);
			// 
			// miRemoveDemoProject
			// 
			this.miRemoveDemoProject.Name = "miRemoveDemoProject";
			this.miRemoveDemoProject.ShortcutKeys = System.Windows.Forms.Keys.Delete;
			this.miRemoveDemoProject.Size = new System.Drawing.Size(146, 22);
			this.miRemoveDemoProject.Text = "&Remove";
			this.miRemoveDemoProject.Click += new System.EventHandler(this.miRemoveDemoProject_Click);
			// 
			// miExploreDemoProject
			// 
			this.miExploreDemoProject.Name = "miExploreDemoProject";
			this.miExploreDemoProject.Size = new System.Drawing.Size(146, 22);
			this.miExploreDemoProject.Text = "Explore";
			this.miExploreDemoProject.Click += new System.EventHandler(this.miExploreDemoProject_Click);
			// 
			// miDemoProjectDetails
			// 
			this.miDemoProjectDetails.Name = "miDemoProjectDetails";
			this.miDemoProjectDetails.Size = new System.Drawing.Size(146, 22);
			this.miDemoProjectDetails.Text = "Details...";
			this.miDemoProjectDetails.Click += new System.EventHandler(this.miDemoProjectDetails_Click);
			// 
			// gbSystemInfo
			// 
			this.gbSystemInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.gbSystemInfo.Controls.Add(this.lblServerVersion);
			this.gbSystemInfo.Controls.Add(this.lblServerVersionLabel);
			this.gbSystemInfo.Controls.Add(this.lblSQLServer);
			this.gbSystemInfo.Controls.Add(this.lblSQLServerLabel);
			this.gbSystemInfo.Controls.Add(this.lblRepositoryLocation);
			this.gbSystemInfo.Controls.Add(this.lblReposLocationLabel);
			this.gbSystemInfo.Location = new System.Drawing.Point(306, 16);
			this.gbSystemInfo.Name = "gbSystemInfo";
			this.gbSystemInfo.Size = new System.Drawing.Size(432, 152);
			this.gbSystemInfo.TabIndex = 22;
			this.gbSystemInfo.TabStop = false;
			this.gbSystemInfo.Text = "System Info";
			// 
			// lblServerVersion
			// 
			this.lblServerVersion.Location = new System.Drawing.Point(152, 24);
			this.lblServerVersion.Name = "lblServerVersion";
			this.lblServerVersion.Size = new System.Drawing.Size(272, 16);
			this.lblServerVersion.TabIndex = 26;
			// 
			// lblServerVersionLabel
			// 
			this.lblServerVersionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblServerVersionLabel.Location = new System.Drawing.Point(10, 24);
			this.lblServerVersionLabel.Name = "lblServerVersionLabel";
			this.lblServerVersionLabel.Size = new System.Drawing.Size(136, 16);
			this.lblServerVersionLabel.TabIndex = 25;
			this.lblServerVersionLabel.Text = "Server Manager Version:";
			this.lblServerVersionLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// lblSQLServer
			// 
			this.lblSQLServer.Location = new System.Drawing.Point(152, 128);
			this.lblSQLServer.Name = "lblSQLServer";
			this.lblSQLServer.Size = new System.Drawing.Size(272, 16);
			this.lblSQLServer.TabIndex = 24;
			// 
			// lblSQLServerLabel
			// 
			this.lblSQLServerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblSQLServerLabel.Location = new System.Drawing.Point(19, 128);
			this.lblSQLServerLabel.Name = "lblSQLServerLabel";
			this.lblSQLServerLabel.Size = new System.Drawing.Size(128, 16);
			this.lblSQLServerLabel.TabIndex = 23;
			this.lblSQLServerLabel.Text = "SQL Server:";
			this.lblSQLServerLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// lblRepositoryLocation
			// 
			this.lblRepositoryLocation.Location = new System.Drawing.Point(152, 40);
			this.lblRepositoryLocation.Name = "lblRepositoryLocation";
			this.lblRepositoryLocation.Size = new System.Drawing.Size(272, 80);
			this.lblRepositoryLocation.TabIndex = 22;
			// 
			// lblReposLocationLabel
			// 
			this.lblReposLocationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblReposLocationLabel.Location = new System.Drawing.Point(19, 40);
			this.lblReposLocationLabel.Name = "lblReposLocationLabel";
			this.lblReposLocationLabel.Size = new System.Drawing.Size(128, 16);
			this.lblReposLocationLabel.TabIndex = 21;
			this.lblReposLocationLabel.Text = "Repository Location:";
			this.lblReposLocationLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// lvProjects
			// 
			this.lvProjects.AllowDrop = true;
			this.lvProjects.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lvProjects.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colProject,
            this.colLocation,
            this.colUsers,
            this.colUsersLogged,
            this.colSlaves,
            this.colPlatforms});
			this.lvProjects.ContextMenuStrip = this.cmProjects;
			this.lvProjects.FullRowSelect = true;
			this.lvProjects.Location = new System.Drawing.Point(8, 8);
			this.lvProjects.MultiSelect = false;
			this.lvProjects.Name = "lvProjects";
			this.lvProjects.Size = new System.Drawing.Size(290, 323);
			this.lvProjects.TabIndex = 1;
			this.lvProjects.UseCompatibleStateImageBehavior = false;
			this.lvProjects.View = System.Windows.Forms.View.Details;
			this.lvProjects.SelectedIndexChanged += new System.EventHandler(this.lvProjects_SelectedIndexChanged);
			this.lvProjects.DoubleClick += new System.EventHandler(this.lvProjects_DoubleClick);
			this.lvProjects.DragDrop += new System.Windows.Forms.DragEventHandler(this.lvProjects_DragDrop);
			this.lvProjects.DragEnter += new System.Windows.Forms.DragEventHandler(this.lvProjects_DragEnter);
			this.lvProjects.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MOG_ServerManagerMainForm_KeyUp);
			this.lvProjects.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.lvProjects_ItemDrag);
			// 
			// colProject
			// 
			this.colProject.Text = "Project";
			this.colProject.Width = 90;
			// 
			// colLocation
			// 
			this.colLocation.Text = "Location";
			this.colLocation.Width = 200;
			// 
			// colUsers
			// 
			this.colUsers.Text = "Users";
			// 
			// colUsersLogged
			// 
			this.colUsersLogged.Text = "Logged in";
			// 
			// colSlaves
			// 
			this.colSlaves.Text = "Slaves";
			// 
			// colPlatforms
			// 
			this.colPlatforms.Text = "Platforms";
			// 
			// cmProjects
			// 
			this.cmProjects.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miNewProject,
            this.miDuplicateProject,
            this.menuItem2,
            this.miConfigureProject,
            this.miImportFiles,
            this.menuItem3,
            this.miRemoveProject,
            this.miRemovedProjects});
			this.cmProjects.Name = "cmProjects";
			this.cmProjects.Size = new System.Drawing.Size(185, 126);
			// 
			// miNewProject
			// 
			this.miNewProject.Name = "miNewProject";
			this.miNewProject.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
			this.miNewProject.Size = new System.Drawing.Size(184, 22);
			this.miNewProject.Text = "&New Project";
			this.miNewProject.Click += new System.EventHandler(this.miNewProject_Click);
			// 
			// miDuplicateProject
			// 
			this.miDuplicateProject.Name = "miDuplicateProject";
			this.miDuplicateProject.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
			this.miDuplicateProject.Size = new System.Drawing.Size(184, 22);
			this.miDuplicateProject.Text = "&Duplicate Project";
			this.miDuplicateProject.Click += new System.EventHandler(this.miDuplicateProject_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Name = "menuItem2";
			this.menuItem2.Size = new System.Drawing.Size(181, 6);
			// 
			// miConfigureProject
			// 
			this.miConfigureProject.Name = "miConfigureProject";
			this.miConfigureProject.Size = new System.Drawing.Size(184, 22);
			this.miConfigureProject.Text = "Configure";
			this.miConfigureProject.Click += new System.EventHandler(this.miConfigureProject_Click);
			// 
			// miImportFiles
			// 
			this.miImportFiles.Name = "miImportFiles";
			this.miImportFiles.Size = new System.Drawing.Size(184, 22);
			this.miImportFiles.Text = "Import Files";
			this.miImportFiles.Click += new System.EventHandler(this.miImportFiles_Click);
			// 
			// menuItem3
			// 
			this.menuItem3.Name = "menuItem3";
			this.menuItem3.Size = new System.Drawing.Size(181, 6);
			// 
			// miRemoveProject
			// 
			this.miRemoveProject.Name = "miRemoveProject";
			this.miRemoveProject.ShortcutKeys = System.Windows.Forms.Keys.Delete;
			this.miRemoveProject.Size = new System.Drawing.Size(184, 22);
			this.miRemoveProject.Text = "&Remove";
			this.miRemoveProject.Click += new System.EventHandler(this.miRemoveProject_Click);
			// 
			// miRemovedProjects
			// 
			this.miRemovedProjects.Name = "miRemovedProjects";
			this.miRemovedProjects.Size = new System.Drawing.Size(184, 22);
			this.miRemovedProjects.Text = "Removed Projects...";
			this.miRemovedProjects.Click += new System.EventHandler(this.miRemovedProjects_Click);
			// 
			// tbLicensing
			// 
			this.tbLicensing.ImageIndex = 7;
			this.tbLicensing.Location = new System.Drawing.Point(4, 23);
			this.tbLicensing.Name = "tbLicensing";
			this.tbLicensing.Size = new System.Drawing.Size(746, 343);
			this.tbLicensing.TabIndex = 4;
			this.tbLicensing.Text = "Licensing";
			this.tbLicensing.UseVisualStyleBackColor = true;
			// 
			// tpVersions
			// 
			this.tpVersions.Controls.Add(this.VersionImportButton);
			this.tpVersions.Controls.Add(this.VersionManagerMainPanel);
			this.tpVersions.Controls.Add(this.VersionDeployButton);
			this.tpVersions.Controls.Add(this.VersionRefreshButton);
			this.tpVersions.ImageIndex = 6;
			this.tpVersions.Location = new System.Drawing.Point(4, 23);
			this.tpVersions.Name = "tpVersions";
			this.tpVersions.Size = new System.Drawing.Size(746, 343);
			this.tpVersions.TabIndex = 1;
			this.tpVersions.Text = "Versions";
			this.tpVersions.UseVisualStyleBackColor = true;
			// 
			// VersionImportButton
			// 
			this.VersionImportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.VersionImportButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.VersionImportButton.Location = new System.Drawing.Point(16, 307);
			this.VersionImportButton.Name = "VersionImportButton";
			this.VersionImportButton.Size = new System.Drawing.Size(128, 23);
			this.VersionImportButton.TabIndex = 14;
			this.VersionImportButton.Text = "Import version drop";
			this.VersionImportButton.Click += new System.EventHandler(this.VersionImportButton_Click);
			// 
			// VersionManagerMainPanel
			// 
			this.VersionManagerMainPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.VersionManagerMainPanel.Controls.Add(this.VersionManagerTopPanel);
			this.VersionManagerMainPanel.Controls.Add(this.VersionManagerHorizSplitter);
			this.VersionManagerMainPanel.Controls.Add(this.VersionDescriptionPanel);
			this.VersionManagerMainPanel.Location = new System.Drawing.Point(8, 8);
			this.VersionManagerMainPanel.Name = "VersionManagerMainPanel";
			this.VersionManagerMainPanel.Size = new System.Drawing.Size(730, 291);
			this.VersionManagerMainPanel.TabIndex = 13;
			// 
			// VersionManagerTopPanel
			// 
			this.VersionManagerTopPanel.Controls.Add(this.VersionClientServerPanel);
			this.VersionManagerTopPanel.Controls.Add(this.VersionManagerTopLabel);
			this.VersionManagerTopPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.VersionManagerTopPanel.Location = new System.Drawing.Point(0, 0);
			this.VersionManagerTopPanel.Name = "VersionManagerTopPanel";
			this.VersionManagerTopPanel.Size = new System.Drawing.Size(730, 138);
			this.VersionManagerTopPanel.TabIndex = 11;
			// 
			// VersionClientServerPanel
			// 
			this.VersionClientServerPanel.Controls.Add(this.ClientListView);
			this.VersionClientServerPanel.Controls.Add(this.VersionSplitter);
			this.VersionClientServerPanel.Controls.Add(this.ServerListView);
			this.VersionClientServerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.VersionClientServerPanel.Location = new System.Drawing.Point(0, 17);
			this.VersionClientServerPanel.Name = "VersionClientServerPanel";
			this.VersionClientServerPanel.Size = new System.Drawing.Size(730, 121);
			this.VersionClientServerPanel.TabIndex = 8;
			// 
			// ClientListView
			// 
			this.ClientListView.AllowDrop = true;
			this.ClientListView.CheckBoxes = true;
			this.ClientListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.VersionClientNameColumnHeader,
            this.VersionClientVersionColumnHeader});
			this.ClientListView.ContextMenuStrip = this.VersionContextMenu;
			this.ClientListView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ClientListView.FullRowSelect = true;
			this.ClientListView.GridLines = true;
			this.ClientListView.Location = new System.Drawing.Point(432, 0);
			this.ClientListView.MultiSelect = false;
			this.ClientListView.Name = "ClientListView";
			this.ClientListView.Size = new System.Drawing.Size(298, 121);
			this.ClientListView.Sorting = System.Windows.Forms.SortOrder.Descending;
			this.ClientListView.TabIndex = 1;
			this.ClientListView.UseCompatibleStateImageBehavior = false;
			this.ClientListView.View = System.Windows.Forms.View.Details;
			this.ClientListView.SelectedIndexChanged += new System.EventHandler(this.ClientListView_SelectedIndexChanged);
			this.ClientListView.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.ClientListView_ItemCheck);
			this.ClientListView.DragDrop += new System.Windows.Forms.DragEventHandler(this.ClientListView_DragDrop);
			this.ClientListView.DragOver += new System.Windows.Forms.DragEventHandler(this.ClientListView_DragOver);
			// 
			// VersionClientNameColumnHeader
			// 
			this.VersionClientNameColumnHeader.Text = "Name";
			this.VersionClientNameColumnHeader.Width = 396;
			// 
			// VersionClientVersionColumnHeader
			// 
			this.VersionClientVersionColumnHeader.Text = "Version";
			this.VersionClientVersionColumnHeader.Width = 158;
			// 
			// VersionContextMenu
			// 
			this.VersionContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.VersionDeleteMenuItem});
			this.VersionContextMenu.Name = "VersionContextMenu";
			this.VersionContextMenu.Size = new System.Drawing.Size(139, 26);
			// 
			// VersionDeleteMenuItem
			// 
			this.VersionDeleteMenuItem.Name = "VersionDeleteMenuItem";
			this.VersionDeleteMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
			this.VersionDeleteMenuItem.Size = new System.Drawing.Size(138, 22);
			this.VersionDeleteMenuItem.Text = "Delete";
			this.VersionDeleteMenuItem.Click += new System.EventHandler(this.VersionDeleteMenuItem_Click);
			// 
			// VersionSplitter
			// 
			this.VersionSplitter.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.VersionSplitter.Location = new System.Drawing.Point(424, 0);
			this.VersionSplitter.Name = "VersionSplitter";
			this.VersionSplitter.Size = new System.Drawing.Size(8, 121);
			this.VersionSplitter.TabIndex = 2;
			this.VersionSplitter.TabStop = false;
			// 
			// ServerListView
			// 
			this.ServerListView.AllowDrop = true;
			this.ServerListView.CheckBoxes = true;
			this.ServerListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.VersionServerNameColumnHeader,
            this.VersionServerVersionColumnHeader});
			this.ServerListView.ContextMenuStrip = this.VersionContextMenu;
			this.ServerListView.Dock = System.Windows.Forms.DockStyle.Left;
			this.ServerListView.FullRowSelect = true;
			this.ServerListView.GridLines = true;
			this.ServerListView.Location = new System.Drawing.Point(0, 0);
			this.ServerListView.MultiSelect = false;
			this.ServerListView.Name = "ServerListView";
			this.ServerListView.Size = new System.Drawing.Size(424, 121);
			this.ServerListView.Sorting = System.Windows.Forms.SortOrder.Descending;
			this.ServerListView.TabIndex = 0;
			this.ServerListView.UseCompatibleStateImageBehavior = false;
			this.ServerListView.View = System.Windows.Forms.View.Details;
			this.ServerListView.SelectedIndexChanged += new System.EventHandler(this.ServerListView_SelectedIndexChanged);
			this.ServerListView.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.ServerListView_ItemCheck);
			this.ServerListView.DragDrop += new System.Windows.Forms.DragEventHandler(this.ClientListView_DragDrop);
			this.ServerListView.DragOver += new System.Windows.Forms.DragEventHandler(this.ClientListView_DragOver);
			// 
			// VersionServerNameColumnHeader
			// 
			this.VersionServerNameColumnHeader.Text = "Name";
			this.VersionServerNameColumnHeader.Width = 305;
			// 
			// VersionServerVersionColumnHeader
			// 
			this.VersionServerVersionColumnHeader.Text = "Version";
			this.VersionServerVersionColumnHeader.Width = 115;
			// 
			// VersionManagerTopLabel
			// 
			this.VersionManagerTopLabel.BackColor = System.Drawing.SystemColors.ControlDark;
			this.VersionManagerTopLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.VersionManagerTopLabel.Dock = System.Windows.Forms.DockStyle.Top;
			this.VersionManagerTopLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.VersionManagerTopLabel.Location = new System.Drawing.Point(0, 0);
			this.VersionManagerTopLabel.Name = "VersionManagerTopLabel";
			this.VersionManagerTopLabel.Size = new System.Drawing.Size(730, 17);
			this.VersionManagerTopLabel.TabIndex = 5;
			this.VersionManagerTopLabel.Text = "Available Server and Client Versions";
			// 
			// VersionManagerHorizSplitter
			// 
			this.VersionManagerHorizSplitter.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.VersionManagerHorizSplitter.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.VersionManagerHorizSplitter.Location = new System.Drawing.Point(0, 138);
			this.VersionManagerHorizSplitter.Name = "VersionManagerHorizSplitter";
			this.VersionManagerHorizSplitter.Size = new System.Drawing.Size(730, 9);
			this.VersionManagerHorizSplitter.TabIndex = 12;
			this.VersionManagerHorizSplitter.TabStop = false;
			// 
			// VersionDescriptionPanel
			// 
			this.VersionDescriptionPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.VersionDescriptionPanel.Controls.Add(this.VersionFilesListView);
			this.VersionDescriptionPanel.Controls.Add(this.VersionFilesSplitter);
			this.VersionDescriptionPanel.Controls.Add(this.VersionDescription);
			this.VersionDescriptionPanel.Controls.Add(this.DescriptionTitle);
			this.VersionDescriptionPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.VersionDescriptionPanel.Location = new System.Drawing.Point(0, 147);
			this.VersionDescriptionPanel.Name = "VersionDescriptionPanel";
			this.VersionDescriptionPanel.Size = new System.Drawing.Size(730, 144);
			this.VersionDescriptionPanel.TabIndex = 9;
			// 
			// VersionFilesListView
			// 
			this.VersionFilesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.VersionsFIlesNameColumnHeader,
            this.VersionsFIlesDateColumnHeader,
            this.VersionsFIlesSizeColumnHeader});
			this.VersionFilesListView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.VersionFilesListView.FullRowSelect = true;
			this.VersionFilesListView.Location = new System.Drawing.Point(352, 16);
			this.VersionFilesListView.Name = "VersionFilesListView";
			this.VersionFilesListView.Size = new System.Drawing.Size(374, 124);
			this.VersionFilesListView.TabIndex = 4;
			this.VersionFilesListView.UseCompatibleStateImageBehavior = false;
			this.VersionFilesListView.View = System.Windows.Forms.View.Details;
			// 
			// VersionsFIlesNameColumnHeader
			// 
			this.VersionsFIlesNameColumnHeader.Text = "Name";
			this.VersionsFIlesNameColumnHeader.Width = 292;
			// 
			// VersionsFIlesDateColumnHeader
			// 
			this.VersionsFIlesDateColumnHeader.Text = "Date";
			this.VersionsFIlesDateColumnHeader.Width = 135;
			// 
			// VersionsFIlesSizeColumnHeader
			// 
			this.VersionsFIlesSizeColumnHeader.Text = "Size";
			this.VersionsFIlesSizeColumnHeader.Width = 114;
			// 
			// VersionFilesSplitter
			// 
			this.VersionFilesSplitter.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.VersionFilesSplitter.Location = new System.Drawing.Point(344, 16);
			this.VersionFilesSplitter.Name = "VersionFilesSplitter";
			this.VersionFilesSplitter.Size = new System.Drawing.Size(8, 124);
			this.VersionFilesSplitter.TabIndex = 5;
			this.VersionFilesSplitter.TabStop = false;
			// 
			// VersionDescription
			// 
			this.VersionDescription.Dock = System.Windows.Forms.DockStyle.Left;
			this.VersionDescription.Location = new System.Drawing.Point(0, 16);
			this.VersionDescription.Name = "VersionDescription";
			this.VersionDescription.ReadOnly = true;
			this.VersionDescription.Size = new System.Drawing.Size(344, 124);
			this.VersionDescription.TabIndex = 2;
			this.VersionDescription.Text = "";
			// 
			// DescriptionTitle
			// 
			this.DescriptionTitle.BackColor = System.Drawing.SystemColors.ControlDark;
			this.DescriptionTitle.Dock = System.Windows.Forms.DockStyle.Top;
			this.DescriptionTitle.Location = new System.Drawing.Point(0, 0);
			this.DescriptionTitle.Name = "DescriptionTitle";
			this.DescriptionTitle.Size = new System.Drawing.Size(726, 16);
			this.DescriptionTitle.TabIndex = 3;
			this.DescriptionTitle.Text = "Version Description";
			// 
			// VersionDeployButton
			// 
			this.VersionDeployButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.VersionDeployButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.VersionDeployButton.Location = new System.Drawing.Point(626, 307);
			this.VersionDeployButton.Name = "VersionDeployButton";
			this.VersionDeployButton.Size = new System.Drawing.Size(104, 23);
			this.VersionDeployButton.TabIndex = 12;
			this.VersionDeployButton.Text = "Deploy Updates";
			this.VersionDeployButton.Click += new System.EventHandler(this.VersionDeployButton_Click);
			// 
			// VersionRefreshButton
			// 
			this.VersionRefreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.VersionRefreshButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.VersionRefreshButton.Location = new System.Drawing.Point(514, 307);
			this.VersionRefreshButton.Name = "VersionRefreshButton";
			this.VersionRefreshButton.Size = new System.Drawing.Size(104, 23);
			this.VersionRefreshButton.TabIndex = 11;
			this.VersionRefreshButton.Text = "Refresh";
			this.VersionRefreshButton.Click += new System.EventHandler(this.VersionRefreshButton_Click);
			// 
			// tpTrash
			// 
			this.tpTrash.Controls.Add(this.trashPolicyEditorControl1);
			this.tpTrash.ImageIndex = 4;
			this.tpTrash.Location = new System.Drawing.Point(4, 23);
			this.tpTrash.Name = "tpTrash";
			this.tpTrash.Size = new System.Drawing.Size(746, 343);
			this.tpTrash.TabIndex = 0;
			this.tpTrash.Text = "Trash";
			this.tpTrash.UseVisualStyleBackColor = true;
			// 
			// trashPolicyEditorControl1
			// 
			this.trashPolicyEditorControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.trashPolicyEditorControl1.Location = new System.Drawing.Point(0, 0);
			this.trashPolicyEditorControl1.LogActions = true;
			this.trashPolicyEditorControl1.LogFilename = "C:\\TEMP\\MOG\\MOG_ServerTabs\\trashlog.txt";
			this.trashPolicyEditorControl1.Name = "trashPolicyEditorControl1";
			this.trashPolicyEditorControl1.Size = new System.Drawing.Size(746, 343);
			this.trashPolicyEditorControl1.TabIndex = 0;
			// 
			// tbRemoteServers
			// 
			this.tbRemoteServers.Controls.Add(this.RemoteServerSettings);
			this.tbRemoteServers.ImageIndex = 7;
			this.tbRemoteServers.Location = new System.Drawing.Point(4, 23);
			this.tbRemoteServers.Name = "tbRemoteServers";
			this.tbRemoteServers.Size = new System.Drawing.Size(746, 343);
			this.tbRemoteServers.TabIndex = 5;
			this.tbRemoteServers.Text = "Remote Servers";
			this.tbRemoteServers.UseVisualStyleBackColor = true;
			// 
			// RemoteServerSettings
			// 
			this.RemoteServerSettings.AutoSize = true;
			this.RemoteServerSettings.Dock = System.Windows.Forms.DockStyle.Fill;
			this.RemoteServerSettings.EnableRemoteServers = false;
			this.RemoteServerSettings.Location = new System.Drawing.Point(0, 0);
			this.RemoteServerSettings.Name = "RemoteServerSettings";
			this.RemoteServerSettings.Size = new System.Drawing.Size(746, 343);
			this.RemoteServerSettings.TabIndex = 0;
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
			this.MainTabImageList.Images.SetKeyName(7, "MOG RemoteServers.png");
			// 
			// StartPageInfoRichTextBox
			// 
			this.StartPageInfoRichTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.StartPageInfoRichTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(217)))), ((int)(((byte)(230)))));
			this.StartPageInfoRichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.StartPageInfoRichTextBox.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.StartPageInfoRichTextBox.DetectUrls = false;
			this.StartPageInfoRichTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.StartPageInfoRichTextBox.ForeColor = System.Drawing.Color.SteelBlue;
			this.StartPageInfoRichTextBox.Location = new System.Drawing.Point(192, 390);
			this.StartPageInfoRichTextBox.Name = "StartPageInfoRichTextBox";
			this.StartPageInfoRichTextBox.ReadOnly = true;
			this.StartPageInfoRichTextBox.Size = new System.Drawing.Size(552, 112);
			this.StartPageInfoRichTextBox.TabIndex = 1;
			this.StartPageInfoRichTextBox.Text = resources.GetString("StartPageInfoRichTextBox.Text");
			this.StartPageInfoRichTextBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.StartPageInfoRichTextBox_MouseUp);
			this.StartPageInfoRichTextBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.StartPageInfoRichTextBox_MouseMove);
			// 
			// mainMenu
			// 
			this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miMainMOG,
            this.miMainView,
            this.miMainProject});
			// 
			// miMainMOG
			// 
			this.miMainMOG.Index = 0;
			this.miMainMOG.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miChangeSQLConnection,
            this.miChangeRepository,
            this.miCycleServer,
            this.menuItem4,
            this.miExit});
			this.miMainMOG.Text = "MOG";
			// 
			// miChangeSQLConnection
			// 
			this.miChangeSQLConnection.Index = 0;
			this.miChangeSQLConnection.Text = "Change SQL Connection";
			this.miChangeSQLConnection.Visible = false;
			this.miChangeSQLConnection.Click += new System.EventHandler(this.miChangeSQLConnection_Click);
			// 
			// miChangeRepository
			// 
			this.miChangeRepository.Index = 1;
			this.miChangeRepository.Text = "Change MOG Repository";
			this.miChangeRepository.Visible = false;
			this.miChangeRepository.Click += new System.EventHandler(this.miChangeRepository_Click);
			// 
			// miCycleServer
			// 
			this.miCycleServer.Index = 2;
			this.miCycleServer.Text = "Cycle Server";
			this.miCycleServer.Visible = false;
			this.miCycleServer.Click += new System.EventHandler(this.miCycleServer_Click);
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 3;
			this.menuItem4.Text = "-";
			// 
			// miExit
			// 
			this.miExit.Index = 4;
			this.miExit.Text = "Exit";
			this.miExit.Click += new System.EventHandler(this.miExit_Click);
			// 
			// miMainView
			// 
			this.miMainView.Index = 1;
			this.miMainView.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miViewRefresh});
			this.miMainView.Text = "View";
			// 
			// miViewRefresh
			// 
			this.miViewRefresh.Index = 0;
			this.miViewRefresh.Shortcut = System.Windows.Forms.Shortcut.F5;
			this.miViewRefresh.Text = "Refresh";
			this.miViewRefresh.Click += new System.EventHandler(this.miViewRefresh_Click);
			// 
			// miMainProject
			// 
			this.miMainProject.Index = 2;
			this.miMainProject.Text = "Project";
			this.miMainProject.Popup += new System.EventHandler(this.miMainProject_Popup);
			// 
			// ServerManagerStatusBar
			// 
			this.ServerManagerStatusBar.Location = new System.Drawing.Point(0, 518);
			this.ServerManagerStatusBar.Name = "ServerManagerStatusBar";
			this.ServerManagerStatusBar.Size = new System.Drawing.Size(754, 24);
			this.ServerManagerStatusBar.TabIndex = 1;
			// 
			// VersionImportDropDialog
			// 
			this.VersionImportDropDialog.DefaultExt = "zip";
			this.VersionImportDropDialog.Filter = "Archive Files|*.zip";
			this.VersionImportDropDialog.Multiselect = true;
			this.VersionImportDropDialog.Title = "Select drop archive to import...";
			// 
			// HelpBarPictureBox
			// 
			this.HelpBarPictureBox.BackColor = System.Drawing.Color.White;
			this.HelpBarPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.HelpBarPictureBox.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.HelpBarPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("HelpBarPictureBox.Image")));
			this.HelpBarPictureBox.Location = new System.Drawing.Point(0, 370);
			this.HelpBarPictureBox.Name = "HelpBarPictureBox";
			this.HelpBarPictureBox.Size = new System.Drawing.Size(754, 148);
			this.HelpBarPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.HelpBarPictureBox.TabIndex = 2;
			this.HelpBarPictureBox.TabStop = false;
			this.HelpBarPictureBox.Click += new System.EventHandler(this.HelpBarPictureBox_Click);
			// 
			// MOG_ServerManagerMainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(754, 542);
			this.Controls.Add(this.MOGMainTabControl);
			this.Controls.Add(this.StartPageInfoRichTextBox);
			this.Controls.Add(this.HelpBarPictureBox);
			this.Controls.Add(this.ServerManagerStatusBar);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Menu = this.mainMenu;
			this.MinimumSize = new System.Drawing.Size(762, 576);
			this.Name = "MOG_ServerManagerMainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Server Manager";
			this.Load += new System.EventHandler(this.MOG_ServerManagerMainForm_Load);
			this.Closed += new System.EventHandler(this.MOG_ServerManagerMainForm_Closed);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MOG_ServerManagerMainForm_KeyUp);
			this.MOGMainTabControl.ResumeLayout(false);
			this.tbStart.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.StartPictureBox)).EndInit();
			this.tpProjects.ResumeLayout(false);
			this.cmDemoProjects.ResumeLayout(false);
			this.gbSystemInfo.ResumeLayout(false);
			this.cmProjects.ResumeLayout(false);
			this.tpVersions.ResumeLayout(false);
			this.VersionManagerMainPanel.ResumeLayout(false);
			this.VersionManagerTopPanel.ResumeLayout(false);
			this.VersionClientServerPanel.ResumeLayout(false);
			this.VersionContextMenu.ResumeLayout(false);
			this.VersionDescriptionPanel.ResumeLayout(false);
			this.tpTrash.ResumeLayout(false);
			this.tbRemoteServers.ResumeLayout(false);
			this.tbRemoteServers.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.HelpBarPictureBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
            Control.CheckForIllegalCrossThreadCalls = false;

            Application.Run(new MOG_ServerManagerMainForm());
		}
		#endregion
		#region User defs
		/// <summary>
		/// Support for version deployment
		/// </summary>
		public VersionManagerClass mVersionManager;
		public Thread mMogProcess;
		#endregion
		#region Constructor
		public MOG_ServerManagerMainForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

		public void InitializeSystem(SplashForm SplashScreen)
		{
			// Initialize the .Net style menus
			SplashScreen.updateSplash("Initializing menu system...", 10);

			// Initialize MOG
			SplashScreen.updateSplash("Connecting MOG SQL\\Server...", 10);

			// Check to see if we can init the server
			if (!MOG_Main.Init_Client("", "Server Manager"))
			{
// JohnRen - Removed because there is no reason for the ServerManager to care about being connected to the server
//			 The Only important critical information we obtain from the server is the database connection string which
//			 most likely already exists in the local MOG.ini file.
//				string message = "Failed to initialize.\nPlease check to confirm your MOG Server is running.";
//				throw( new Exception(message));
				// At leat inform the user that we failed to connect and will continue in an offline mode
				string message = "Failed to connect to the MOG Server.\n" + 
								 "Please be advised that you will most likely experience limited functionality during this session.";
				MOG_Report.ReportMessage("Connection Failed", message, "", MOG_ALERT_LEVEL.ALERT);
			}

			// Load trash control
			SplashScreen.updateSplash("Initializing trash manager...", 0);
			this.trashPolicyEditorControl1.LoadRepository(MOG_ControllerSystem.GetSystem().GetConfigFilename());

			SplashScreen.updateSplash("Initializing version manager...", 0);
			mVersionManager = new VersionManagerClass(this);					// DVC create the VersionManager class
			mVersionManager.BuildVersionsList();								// DVC Load and Populate lists

			// Load up project info
			SplashScreen.updateSplash("Loading project info...", 0);
			RefreshProjectsListView();

			// Refresh projects menu with latest project data
			RebuildProjectMenu();

			// Setup repository label
			this.lblRepositoryLocation.Text = MOG_ControllerSystem.GetSystem().GetSystemRepositoryPath();

			// Setup version label
			FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(Application.ExecutablePath);
			if (fileVersionInfo != null)
			{
				this.lblServerVersion.Text = "MOG ServerManager 2005 - " + fileVersionInfo.FileVersion;
			}

			// Get the SQL connection string info from mog.ini, if it exists
			//if (File.Exists(Application.StartupPath + "\\mog.ini"))
			{
				//MOG_Ini mIni = new MOG_Ini(Application.StartupPath + "\\mog.ini");
				
				// Parse the sql server name from connection string
				//if (mIni.KeyExist("SQL", "ConnectionString"))
				{
					// Packet size=4096;integrated security=SSPI;data source=nemesis;persist security info=False;initial catalog=mog;
					string connectionString = MOG_ControllerSystem.GetDB().GetConnectionString();
					//string connectionString = mIni.GetString("SQL", "ConnectionString");
					string sqlServerName = "";
					string sqlDatabaseName = "";
					string[] parts = connectionString.Split(";".ToCharArray());
					foreach (string part in parts)
					{
						if (part.ToLower().StartsWith("data source="))
							sqlServerName = part.Substring( part.ToLower().IndexOf("data source=")+12 ).ToUpper();
						if (part.ToLower().StartsWith("initial catalog="))
							sqlDatabaseName = part.Substring( part.ToLower().IndexOf("initial catalog=")+16 );
					}

					this.lblSQLServer.Text = sqlServerName + " (" + sqlDatabaseName + ")";
					//this.ServerManagerToolTip.SetToolTip( this.lblSQLServerLabel, connectionString );
					//this.ServerManagerToolTip.SetToolTip( this.lblSQLServer, connectionString );
				}
				//mIni.Close();
			}

			// Get latest demo project info
			RefreshDemoProjectsListView();

			// If there are no projects, automatically pop up the new project dialog
			if (MOG_ControllerSystem.GetSystem().GetProjectNames().Count == 0)
			{
				NewProject();
			}

			MOG_Prompt.ClientForm = this;
			MOG_Progress.ClientForm = this;

			// Load remote servers control
			SplashScreen.updateSplash("Initializing remote servers manager...", 0);
			RemoteServerSettings.Initialize();

			DepthManager.InitParent(this, FormDepth.BACK);

			SplashScreen.updateSplash("Done", 1);
			SplashScreen.mInitializationComplete = true;

			MogUtil_HelpManager.LoadHelpText(this, "WelcomeServerManager.rtf");

			// Create the web browser
			WebBrowser webPage = new WebBrowser();
			webPage.Dock = DockStyle.Fill;
			// Login to the selected url
			string serverMacAddress = MOG_ControllerSystem.GetServerComputerMacAddress();
			string url = "http://www.mogware.net/subscriptions/subscription.php?phys_address=" + serverMacAddress;
			webPage.Navigate(url);
			this.tbLicensing.Controls.Add(webPage);
		}
		#endregion

		private void MogProcess()
		{
			while(!MOG_Main.IsShutdown())
			{
				try
				{
					MOG_Main.Process();
				}
				catch(Exception ex)
				{
					MOG_Report.ReportMessage("MOG Process", ex.Message, ex.StackTrace, MOG_ALERT_LEVEL.CRITICAL);
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

		#region Member functions


		private void RebuildProjectMenu()
		{
			// Make sure the system has the latest project info
			MOG_ControllerSystem.GetSystem().Load();
			
			// Add persistent items
			this.miMainProject.MenuItems.Clear();
			this.miMainProject.MenuItems.Add("New Project...", new EventHandler(miMainNewProject_Click));
			this.miMainProject.MenuItems.Add("-");

			// Make a MenuItem for each project
			foreach (string projName in MOG_ControllerSystem.GetSystem().GetProjectNames())
			{
				MenuItem miProj = new MenuItem(projName);

				// Setup its submenu
				miProj.MenuItems.Add("Configure", new EventHandler(miMainConfigure_Click));
				miProj.MenuItems.Add("Import Files...", new EventHandler(miMainImportFiles_Click));
				miProj.MenuItems.Add("-");
				miProj.MenuItems.Add("Remove", new EventHandler(miMainRemove_Click));

				this.miMainProject.MenuItems.Add(miProj);
			}

			// Make a MenuItem for demo projects
			MenuItem miDemoProjects = new MenuItem("Demo Projects");

			string demoPath = MOG_ControllerSystem.GetSystemDemoProjectsPath();
			if (!DosUtils.DirectoryExistFast(demoPath))
			{
				// If no demos, return
				return;
			}

			// Build submenu for each demo project
			foreach (string demoProjectPath in Directory.GetDirectories(demoPath))
			{
				string demoProjectName = Path.GetFileName(demoProjectPath);
				//string demoProjectDescription = "";

				MenuItem miDemoProj = new MenuItem(demoProjectName);
				miDemoProjects.MenuItems.Add(miDemoProj);

				
				// load name and description from info file if there is one
				//				if (File.Exists(demoProjectPath + "\\" + "DemoProjectInfo.Info"))
				//				{
				//					MOG_Ini demoProjIni = new MOG_Ini(demoProjectPath + "\\DemoProjectInfo.Info");
				//
				//					if (demoProjIni.SectionExist("PROJECT"))
				//					{
				//						if (demoProjIni.KeyExist("PROJECT", "Name"))
				//							demoProjectName = demoProjIni.GetString("PROJECT", "Name");
				//
				//						if (demoProjIni.KeyExist("PROJECT", "Description"))
				//							demoProjectDescription = demoProjIni.GetString("PROJECT", "Description");
				//					}
				//
				//					demoProjIni.Close();
			}

			this.miMainProject.MenuItems.Add(miDemoProjects);
		}

		/// <summary>
		/// For automatically purging deleted projects
		/// </summary>
		/// <param name="projName">The name of the deleted project</param>
		/// <param name="iniFilename">The main system INI file</param>
		/// <returns>True on success, else false</returns>
		private bool PurgeDeletedProject(string projName, string iniFilename)
		{
			bool deleted = false;

			// open the ini file
			MOG_Ini ini = new MOG_Ini();
			if (ini.Open(iniFilename, FileShare.ReadWrite))
			{
				string deletedProjPath = MOG_ControllerSystem.GetSystemDeletedProjectsPath() + "\\" + projName;
				if (DosUtils.DirectoryDeleteFast(deletedProjPath, true))
				{
					// make sure projName is a deleted project
					if (ini.SectionExist(projName + ".Deleted"))
					{
						ini.RemoveString("Projects.Deleted", projName);
						ini.RemoveSection(projName + ".Deleted");

						ini.Save();
					}

					deleted = true;
				}

				ini.Close();
			}

			return deleted;
		}

		private void RemoveProject(string projName)
		{
			// Remove project called 'projName' from MOG
			if ( MOG_Prompt.PromptResponse("Confirm Project Deletion", "Are you sure you want to remove this project?\n\n" + projName, MOGPromptButtons.YesNo ) == MOGPromptResult.No )
			{
				return;
			}

			ProgressDialog progress = new ProgressDialog("Removing Project", "Please wait while '" + projName + "' is removed.", RemoveProject_Worker, projName, false);
			progress.ShowDialog(this);

			RefreshProjectsListView();
		}

		private void RemoveProject_Worker(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			string projName = e.Argument as string;

			MOG_Project proj = MOG_ControllerProject.LoginProject(projName, "");
			if (proj != null)
			{
				// rename directory
				string dirName = proj.GetProjectPath();
				if (DosUtils.DirectoryExistFast(MOG_ControllerSystem.GetSystemDeletedProjectsPath() + "\\" + projName))
				{
					// A project of this name has already been removed
					if (MOG_Prompt.PromptResponse("Project Name Conflict",
						"A project named '" + projName + "' has already been deleted, but not yet purged.\nWould you like to purge it?", MOGPromptButtons.YesNo) != MOGPromptResult.Yes)
					{
						return;
					}

					// Purge it for them
					PurgeDeletedProject(projName, MOG_ControllerSystem.GetSystem().GetConfigFilename());
				}

				if (DosUtils.DirectoryExistFast(dirName))
				{
					// Backup the project's database
					MOG_Database.ExportProjectTables(projName, dirName);

					try
					{
						DosUtils.DirectoryMoveFast(dirName, MOG_ControllerSystem.GetSystemDeletedProjectsPath() + "\\" + projName, true);
					}
					catch (Exception ex)
					{
						MOG_Prompt.PromptResponse("Project Removal Failure", "Couldn't remove project " + projName + ".  This is probably due to a sharing violation.\nPlease close all applications and windows that may have documents open from this project and try again.", ex.StackTrace, MOGPromptButtons.OK, MOG_ALERT_LEVEL.CRITICAL);
						return;
					}
				}
			}

			// Remove the project from the database
			MOG_ControllerSystem.GetDB().DeleteProjectTables(projName);

			// Mark project as having been deleted in the INI file
			try
			{
				MOG_Ini ini = MOG_ControllerSystem.GetSystem().GetConfigFile();

				// Move the project over the deleted projects list
				ini.RemoveString("projects", projName);
				ini.PutString("projects.deleted", projName, "");
				ini.RenameSection(projName, projName + ".deleted");

				MOG_ControllerSystem.GetSystem().GetProjectNames().Remove(projName);

				ini.Save();
			}
			catch (Exception ex)
			{
				Utils.ShowMessageBoxExclamation("Couldn't update main MOG INI file!", "WARNING");
				ex.ToString();
			}
		}

		private void NewProject()
		{
			CreateNewProjectForm cnpf = new CreateNewProjectForm("Project Name");

			// Update cnpf's list of projects
			cnpf.ExistingProjects.Clear();
			foreach (ListViewItem projItem in this.lvProjects.Items)
			{
				cnpf.ExistingProjects.Add(projItem.Text);
			}

			cnpf.ShowDialog(this);

            // Check if we should now launch the client
            if (cnpf.DialogResult != DialogResult.Cancel && cnpf.cbLaunchClient.Checked)
            {
				string client = MOG.MOG_Main.GetExecutablePath() + "\\MOG_Client.exe";
				if (DosUtils.FileExistFast(client))
				{
					guiCommandLine.ShellSpawn(MOG.MOG_Main.GetExecutablePath() + "\\MOG_Client.exe");
				}
            }
			
			// Make sure the new project is visible
			RefreshProjectsListView();
		}

		private void ImportFilesToProject(string projName)
		{
			// Setup the form
			FileImportForm fileImportForm = new FileImportForm( MOG_ControllerProject.LoginProject(projName, "") );
			fileImportForm.ShowDialog(this);
		}

		private void ConfigureProject(string projName)
		{
			// Login selected project
			MOG_ControllerProject.LoginProject(projName, "");

			// Now get the project structure back out
			MOG_Project proj = MOG_ControllerProject.GetProject();
			if (proj != null)
			{
				ConfigureProjectForm configProjForm = new ConfigureProjectForm(proj);

				// Show form and refresh the project list
				configProjForm.ShowDialog(this);
				RefreshProjectsListView();
			}
		}

		private void RefreshDemoProjectsListView()
		{
			// Load demo project listview
			this.lvDemoProjects.Items.Clear();

            ArrayList demoProjectNames = MOG_ControllerDemoProject.GetAllDemoProjectNames();

            foreach (string demoProjectName in demoProjectNames)
			{
				// Setup the listviewitem
				ListViewItem projItem = new ListViewItem(demoProjectName);
				projItem.SubItems.Add("DemoProject");
                projItem.SubItems.Add(MOG_ControllerSystem.GetSystemDemoProjectsPath() + "\\" + demoProjectName);		// not displayed but used to find the project path later
				this.lvDemoProjects.Items.Add(projItem);
			}
		}

		private void RefreshProjectsListView()
		{
			// reload the projects
			MOG_ControllerSystem.GetSystem().Load();

			// Clear all the projects
			this.lvProjects.Items.Clear();

			// Get all the projects listed in the mog system
			foreach (string projectName in MOG_ControllerSystem.GetSystem().GetProjectNames())
			{
				MOG_Project pProject = MOG_ControllerSystem.GetSystem().GetProject(projectName);
				if (pProject != null)
				{
					ListViewItem project = new ListViewItem();

					project.Text = projectName;
					project.SubItems.Add( pProject.GetProjectPath() );
					project.SubItems.Add(pProject.GetUsers().Count.ToString());
					project.SubItems.Add( "No" );
					project.SubItems.Add( "??" );
					
					string platforms = "";
					foreach (string platName in pProject.GetPlatformNames())
					{
						platforms += platName + ", ";
					}

					platforms = platforms.Trim(", ".ToCharArray());
					project.SubItems.Add( platforms );

					this.lvProjects.Items.Add(project);
				}
			}
		}
		#endregion
		#region Versioning functionality (Dallan)
		//####################################################################
		//####################################################################

		// -------------------------------------------------------------------
		// DVC - Refresh Button was clicked
		private void VersionMan_UpdateClick(object sender, System.EventArgs e)
		{
			mVersionManager.BuildVersionsList();

			mVersionManager.MarkCurrentVersion(ServerListView, "Server");
			mVersionManager.MarkCurrentVersion(ClientListView, "Client");
			mVersionManager.MarkCurrentVersion(ClientListView, "MOGBridge");
		}

		/// <summary>
		/// Only allow one item to be checked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ServerListView_ItemCheck(object sender, System.Windows.Forms.ItemCheckEventArgs e)
		{
			if(mVersionManager.bCheckOverride)	// we set this check state within this function so avoid recursive state
				return;

			// Only allow one checked server
			for (int i = 0; i < ServerListView.Items.Count; i++)
			{
				if (i != e.Index)
				{
					mVersionManager.bCheckOverride = true;
					ServerListView.Items[i].Checked = false;
					mVersionManager.bCheckOverride = false;
				}
			}

			ServerListView.Items[e.Index].Selected = true;
		}

		/// <summary>
		/// Only allow one item of a givent type to be checked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ClientListView_ItemCheck(object sender, System.Windows.Forms.ItemCheckEventArgs e)
		{
			// Only allow one type of versionNode to be selected
			if(mVersionManager.bCheckOverride)	// we set this check state within this function so avoid recursive state
				return;

			try
			{
				VersionNode checkedVersion = (VersionNode)ClientListView.Items[e.Index].Tag;

				// Only allow one checked server
				for (int i = 0; i < ClientListView.Items.Count; i++)
				{
					VersionNode version = (VersionNode)ClientListView.Items[i].Tag;

					// If we are not the newly selected item and our types are the same, uncheck us?
					if (i != e.Index && string.Compare(version.Type, checkedVersion.Type, true) == 0)
					{
						mVersionManager.bCheckOverride = true;
						ClientListView.Items[i].Checked = false;
						mVersionManager.bCheckOverride = false;
					}
				}

				ClientListView.Items[e.Index].Selected = true;
			}
			catch
			{
			}
		}

		/// <summary>
		/// Update our compatible clients as well as update the discription and files in info window
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ServerListView_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			try
			{
				ClientListView.Items.Clear();
			    
				// get list if compatable clients
				ArrayList list = new ArrayList();
				foreach( ListViewItem serverItem in ServerListView.SelectedItems)
				{
					VersionNode serverVersion = (VersionNode)serverItem.Tag;
					ArrayList compatibleClients = mVersionManager.GetCompatibleBuilds(serverItem);
				
					foreach(VersionNode version in compatibleClients)
					{
						ListViewItem item = new ListViewItem(version.Name);
						item.SubItems.Add(version.Version);

						if(version.MinorVersion >= serverVersion.MinorVersion)
						{
							item.Font = new Font(item.Font.FontFamily, item.Font.Size, FontStyle.Bold);
						}
						item.ImageIndex = version.ImageIndex;
						item.Tag = version;
				
						ClientListView.Items.Add(item);
					}

					// Check if we still need to load our description?
					if (serverVersion.Description == null)
					{
						serverVersion.Description	= VersionManagerClass.LoadFileByLines(serverVersion.SourceDirectory + "\\WHATSNEW.TXT");
					}
					// Get the description of this selected item
					VersionDescription.Text = serverVersion.Description;
					mVersionManager.GetFileList(serverVersion);
				}

				//				// Get the description of this selected item
				//				foreach (ListViewItem item in ServerListView.SelectedItems)
				//				{
				//					VersionNode version = (VersionNode)item.Tag;
				//					VersionDescription.Text = version.Description;
				//					mVersionManager.GetFileList(version);
				//				}

				mVersionManager.MarkCurrentVersion(ClientListView, "Client");
				mVersionManager.MarkCurrentVersion(ClientListView, "MOGBridge");
			}
			catch(Exception ex)
			{
				MOG_Report.ReportMessage("VersionManager::Server selected node change", ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.ERROR);
			}
		}

		/// <summary>
		/// Update our description and files list of the selected item
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ClientListView_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			try
			{
				// Get the description of this selected item
				foreach (ListViewItem item in ClientListView.SelectedItems)
				{
					VersionNode version = (VersionNode)item.Tag;
					if (this.ActiveControl == ClientListView)
					{
						// Check if we still need to load our description?
						if (version.Description == null)
						{
							version.Description	= VersionManagerClass.LoadFileByLines(version.SourceDirectory + "\\WHATSNEW.TXT");
						}
						VersionDescription.Text = version.Description;					
						mVersionManager.GetFileList(version);
					}
				}
			}
			catch(Exception ex)
			{
				MOG_Report.ReportMessage("VersionManager::Server selected node change", ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.ERROR);
			}
		}

		//####################################################################
		//####################################################################
		#endregion
		#region Event handlers
		private void miViewRefresh_Click(object sender, System.EventArgs e)
		{
			// Refresh the window depending on what tab we're on
			switch (this.MOGMainTabControl.SelectedTab.Name)
			{
				case "tpProjects":
					RefreshProjectsListView();
					break;
				case "tpTrash":
					break;
				case "tpVersions":
					if (mVersionManager != null)
					{
						VersionRefreshButton.PerformClick();
					}
					break;
			}
		}

		private void lvProjects_DoubleClick(object sender, System.EventArgs e)
		{
			// Run project configurer on double click
			if (this.lvProjects.SelectedItems.Count > 0)
			{
				ConfigureProject(this.lvProjects.SelectedItems[0].Text);
			}
		}

		private void lvProjects_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			// Login the project they clicked on
			if (this.lvProjects.SelectedItems.Count > 0)
			{
				MOG_ControllerProject.LoginProject(this.lvProjects.SelectedItems[0].Text, "");
			}

			// Update the "logged-in" column of the projects list view
			foreach (ListViewItem item in this.lvProjects.Items)
			{
				if (item.Selected)
				{
					item.SubItems[3].Text = "Yes";
				}
				else
				{
					item.SubItems[3].Text = "No";
				}
			}
		}

		public void miNewProject_Click(object sender, System.EventArgs e)
		{
			NewProject();
		}

		public void miDuplicateProject_Click(object sender, System.EventArgs e)
		{
			MOG_Project selectedProject = MOG_ControllerProject.GetProject();
			string dupProjectName = Microsoft.VisualBasic.Interaction.InputBox("Please name the new project.", "Duplicate Project", selectedProject.GetProjectName() + "_dup", -1, -1);
			if (dupProjectName.Length != 0)
			{
				// Check if this Project already exists?
				MOG_Project existingProj = (MOG_ControllerSystem.GetSystem().GetProject(dupProjectName));
				if (existingProj != null)
				{
					// Prompt the user to make sure they want to overwrite this existing project
					string message = "\n" + 
									 "PROJECT: " + dupProjectName;
					MOGPromptResult result = MOG_Prompt.PromptResponse("Overwite existing project?", message, MOGPromptButtons.YesNoCancel);
					if (result != MOGPromptResult.Yes)
					{
						// Bail without doing anything further
						return;
					}

					// Make sure there isn;t one already in our way
					RemoveProject(dupProjectName);
				}

				// Duplicate the project
				selectedProject.DuplicateProject(dupProjectName);

				// Make sure the new project is visible
				RefreshProjectsListView();
			}
		}

		private void VersionRefreshButton_Click(object sender, System.EventArgs e)
		{
			mVersionManager.BuildVersionsList();

			mVersionManager.MarkCurrentVersion(ServerListView, "Server");
			mVersionManager.MarkCurrentVersion(ClientListView, "Client");
			mVersionManager.MarkCurrentVersion(ClientListView, "MOGBridge");
		}

		public void VersionDeployButton_Click(object sender, System.EventArgs e)
		{
			string message = "Are you sure you wish deploy the selected items?\n";
			MOGPromptResult result = MOG_Prompt.PromptResponse("Deploy Versions", message, MOGPromptButtons.YesNo);
			if(result == MOGPromptResult.No)
			{
				return;
			}

			try
			{
				// Deploy the server
				// ----------------------------------------------------
				mVersionManager.DeployCheckedItems(ServerListView, mVersionManager.DeploymentDirectory, mVersionManager.DeploymentTarget);
				
				// Deploy all client type modules
				// ----------------------------------------------------
				mVersionManager.DeployCheckedItems(ClientListView, mVersionManager.DeploymentDirectory, mVersionManager.DeploymentTarget);
				
				mVersionManager.MarkCurrentVersion(ServerListView, "Server");
				mVersionManager.MarkCurrentVersion(ClientListView, "Client");
				mVersionManager.MarkCurrentVersion(ClientListView, "MOGBridge");
			}
			catch(Exception ex)
			{
				MOG_Report.ReportMessage("Version Deploy", ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.ERROR);
			}
		}

		private void ClientListView_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
		{
			// If the drop comes from the windows shell, allow it
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effect = DragDropEffects.All;
				return;
			}
			
			e.Effect = DragDropEffects.None; 
		}

		private void ClientListView_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				//Save this list of files so the context menu handlers can use them
				string[] DragFiles = (string[])e.Data.GetData("FileDrop", false);  
				
				ArrayList fileNames = new ArrayList();
				foreach (string file in DragFiles)
				{
					fileNames.Add(file);
				}

				VersionImportDrop(fileNames);
			}
		}

		private void VersionImportButton_Click(object sender, System.EventArgs e)
		{
			if (VersionImportDropDialog.ShowDialog(this) == DialogResult.OK)
			{
				ArrayList fileNames = new ArrayList();
				foreach (string file in VersionImportDropDialog.FileNames)
				{
					fileNames.Add(file);
				}

				VersionImportDrop(fileNames);
			}
		}

		private void VersionImportDrop(ArrayList fileNames)
		{
			ProgressDialog progress = new ProgressDialog("Importing version drops", "Please wait while MOG imports the drops...", VersionImportDrop_Worker, fileNames, false);
			progress.ShowDialog();

			// Refresh window
			VersionRefreshButton_Click(null, new EventArgs());
		}

		void VersionImportDrop_Worker(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			ArrayList fileNames = e.Argument as ArrayList;
			
			string unzip = MOG_ControllerSystem.LocateTool("paext.exe");

			for (int i = 0; i < fileNames.Count && !worker.CancellationPending; i++)
			{
				string zipFilename = fileNames[i] as string;
				string module = "";

				// Determine which module this is
				if (zipFilename.IndexOf("client", StringComparison.CurrentCultureIgnoreCase) >= 0)
				{
					module = "Client";
				}
				else if (zipFilename.IndexOf("server", StringComparison.CurrentCultureIgnoreCase) >= 0)
				{
					module = "Server";
				}
				else if (zipFilename.IndexOf("bridge", StringComparison.CurrentCultureIgnoreCase) >= 0)
				{
					module = "MOGBridge";
				}
				else
				{
					MOG_Prompt.PromptResponse("Unable to determine drop module!", "The drop (" + zipFilename + ") does not contain a recognizable module name (Client, Server, MOGBridge) so we cannot continue!");
					continue;
				}

				string targetFolder = mVersionManager.DeploymentDirectory + "\\" + module + "\\" + Path.GetFileNameWithoutExtension(zipFilename);
				string output = "";

				worker.ReportProgress(i * 100 / fileNames.Count, Path.GetFileNameWithoutExtension(zipFilename));
				
				if (DosUtils.DirectoryExistFast(targetFolder) == false)
				{
					// Execute the tool
					guiCommandLine.ShellExecute(unzip, "-p\"" + targetFolder + "\" \"" + zipFilename + "\"", ProcessWindowStyle.Hidden, ref output);
				}
			}
		}

		private void VersionDeleteMenuItem_Click(object sender, System.EventArgs e)
		{
			try
			{
				ListView target = this.ActiveControl as ListView;
				foreach (ListViewItem item in target.SelectedItems)
				{
					if (MOG_Prompt.PromptResponse("Delete Version", "Are you sure you want to delete:\n" + item.Text, MOGPromptButtons.OKCancel) == MOGPromptResult.OK)
					{
						// Get our version node from this item
						VersionNode targetVersion = (VersionNode)item.Tag;

						DosUtils.DirectoryDeleteFast(targetVersion.SourceDirectory);
					}
				}
			}
			catch(Exception ex)
			{
				MOG_Prompt.PromptResponse("Delete version error", ex.Message, ex.StackTrace, MOGPromptButtons.OK);
			}

			// Refresh window
			this.VersionRefreshButton_Click(null, new EventArgs());
		}

		private void miRemoveProject_Click(object sender, System.EventArgs e)
		{
			if (this.lvProjects.SelectedItems.Count > 0)
			{
				RemoveProject(this.lvProjects.SelectedItems[0].Text);
			}
		}

		public void miConfigureProject_Click(object sender, System.EventArgs e)
		{
			if (this.lvProjects.SelectedItems.Count > 0)
			{
				ConfigureProject(this.lvProjects.SelectedItems[0].Text);
			}
		}
		
		private void miRemovedProjects_Click(object sender, System.EventArgs e)
		{
			MOGRemovedProjectsManagerForm rpm = new MOGRemovedProjectsManagerForm(MOG_ControllerSystem.GetSystem().GetConfigFilename(), MOG_ControllerSystem.GetSystem().GetSystemProjectsPath());
			rpm.ShowDialog();

			// Make sure deleted project isn't still displayed
			RefreshProjectsListView();
		}

		private void miImportFiles_Click(object sender, System.EventArgs e)
		{
			if (this.lvProjects.SelectedItems.Count > 0)
			{
				ImportFilesToProject(this.lvProjects.SelectedItems[0].Text);
			}
		}

		private void cmProjects_Popup(object sender, CancelEventArgs e)
		{
			// Set up the project context menu based on what's selected in the list view
			if (this.lvProjects.SelectedItems.Count > 0)
			{
				this.miConfigureProject.Enabled = true;
				this.miImportFiles.Enabled = true;
				this.miRemoveProject.Enabled = true;
			}
			else
			{
				this.miConfigureProject.Enabled = false;
				this.miImportFiles.Enabled = false;
				this.miRemoveProject.Enabled = false;
			}
		}

		private void cmDemoProjects_Popup(object sender, CancelEventArgs e)
		{
			// Set up the demo project context menu based on what's selected in the list view
			if (lvDemoProjects.SelectedItems.Count > 0)
			{
				miRemoveDemoProject.Enabled = true;
				miExploreDemoProject.Enabled = true;
				miImportDemoProject.Enabled = true;
				miDemoProjectDetails.Enabled = true;
			}
			else
			{
				miRemoveDemoProject.Enabled = false;
				miExploreDemoProject.Enabled = false;
				miImportDemoProject.Enabled = false;
				miDemoProjectDetails.Enabled = false;
			}
		}

		private void miExit_Click(object sender, System.EventArgs e)
		{
			Shutdown();
		}

		private void miMainProject_Popup(object sender, System.EventArgs e)
		{
			// Rebuild before we display to make sure we've got the latest project info
			RebuildProjectMenu();
		}

		private void miMainNewProject_Click(object sender, EventArgs args)
		{
			NewProject();
		}

		private void miMainConfigure_Click(object sender, EventArgs args)
		{
			if (sender is MenuItem)
			{
				// Decode project name from sender and configure it
				ConfigureProject( ((MenuItem)((MenuItem)sender).Parent).Text );
			}
		}

		private void miMainImportFiles_Click(object sender, EventArgs args)
		{
			if (sender is MenuItem)
			{
				// Decode project name from sender and import files to it
				ImportFilesToProject( ((MenuItem)((MenuItem)sender).Parent).Text );
			}
		}

		private void miMainRemove_Click(object sender, EventArgs args)
		{
			if (sender is MenuItem)
			{
				// Decode project name from sender and remove it
				RemoveProject( ((MenuItem)((MenuItem)sender).Parent).Text );
			}
		}

		private void miCycleServer_Click(object sender, System.EventArgs e)
		{
			// Server is always connection 1
			MOG.CONTROLLER.CONTROLLERSYSTEM.MOG_ControllerSystem.ConnectionKill(1);
		}

		private void MOG_ServerManagerMainForm_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyData == Keys.F5)
			{
				if (this.MOGMainTabControl.SelectedTab == this.tpProjects)
				{
					// Refresh projects and demo projects list views
					RefreshProjectsListView();
					RefreshDemoProjectsListView();
				}
			}
		}

		private void lvProjects_ItemDrag(object sender, System.Windows.Forms.ItemDragEventArgs e)
		{
			ArrayList items = new ArrayList();
			
			foreach (ListViewItem projItem in this.lvProjects.SelectedItems)
			{
				items.Add(projItem.Text);
			}

			DataObject data = new DataObject("ProjectListData", items);
			this.lvDemoProjects.DoDragDrop(data, DragDropEffects.All);
		}

		private void lvDemoProjects_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
		{
			//Disable all drag-drops until we've verified that it should be allowed
			e.Effect = DragDropEffects.None;
			
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				// from Windows
				string[] draggedFiles = (string[])e.Data.GetData("FileDrop", false);
				
				//Go through all the folders in the list and check to see if they are valid projects or not
				foreach (string draggedFile in draggedFiles)
				{
					if (MOG_ControllerDemoProject.ContainsDemoProject(draggedFile))
					{
						//As soon as one item in the list passes the test, let's allow dropping
						//The drop handler can decide which ones to actually import after that
						e.Effect = e.AllowedEffect;
						break;
					}
				}
			}
			else if (e.Data.GetDataPresent("ProjectListData"))
			{
				// from the projects list view
				// All projects in the projects list view are demoizable
				e.Effect = e.AllowedEffect;
			}
		}

		private void lvDemoProjects_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{
			// If files were dragged in from Windows
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				// Grab the full paths of the dragged-in files/dirs
				string[] draggedFiles = (string[])e.Data.GetData("FileDrop", false);
				foreach (string draggedFile in draggedFiles)
				{
					List<string> files = new List<string>();
					files.Add(draggedFile);

					AddDemoProjects(files);
				}
			}
			else if (e.Data.GetDataPresent("ProjectListData"))	// if files were dragged from the project list view
			{
				// Extract list of project names
				ArrayList projectList = e.Data.GetData("ProjectListData") as ArrayList;

				// Make sure demo projects path exists
				string demoProjectsPath = MOG_ControllerSystem.GetSystemDemoProjectsPath();
				if (!DosUtils.DirectoryExistFast(demoProjectsPath))
				{
					if (!DosUtils.DirectoryCreate(demoProjectsPath))
					{
						MOG_Prompt.PromptMessage("Failed to Create Directory", "The Server failed to create a new Demos directory in the MOG Repository");
					}
				}

				List<string> projectNames = new List<string>();

				foreach (string projName in projectList)
				{
					projectNames.Add(projName);
				}

				DemoizeProjects(projectNames);
			}
		}


		private void lvProjects_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
		{
			//Disable all drag-drops until we've verified that it should be allowed
			e.Effect = DragDropEffects.None;

			if (e.Data.GetDataPresent("DemoProjectListData"))
			{
				//Always allow dragging from the demo projects list view
				e.Effect = e.AllowedEffect;
			}
		}

		private void lvProjects_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{
			// If data was dragged from the demo projects listview
			if (e.Data.GetDataPresent("DemoProjectListData"))
			{
				// Extract the list of demo projects to create
				ArrayList demoList = e.Data.GetData("DemoProjectListData") as ArrayList;
				List<string> projectNames = new List<string>();

				// Import each demo project
				foreach (ListViewItem projItem in demoList)
				{
					projectNames.Add(projItem.SubItems[0].Text);
				}

				if (projectNames.Count > 0)
				{
					ImportDemoProjects(projectNames);
				}
			}
		}

		private void lvDemoProjects_ItemDrag(object sender, System.Windows.Forms.ItemDragEventArgs e)
		{
			ArrayList items = new ArrayList();
			
			foreach (ListViewItem projItem in this.lvDemoProjects.SelectedItems)
			{
				items.Add(projItem);
			}

			DataObject data = new DataObject("DemoProjectListData", items);
			this.lvDemoProjects.DoDragDrop(data, DragDropEffects.All);
		}

		private void btnImportDemoFromProject_Click(object sender, System.EventArgs e)
		{
            if (lvProjects.SelectedItems.Count > 0)
            {
				List<string> projectNames = new List<string>();

				foreach (ListViewItem projItem in lvProjects.SelectedItems)
                {
					projectNames.Add(projItem.Text);
                }

				if (projectNames.Count > 0)
				{
					DemoizeProjects(projectNames);
				}
            }
		}

		private void miChangeSQLConnection_Click(object sender, System.EventArgs e)
		{
			SQLConnectForm sqlForm = new SQLConnectForm();
			if (sqlForm.ShowDialog() == DialogResult.OK)
			{
				// Inform the server about the new SQL Connection
				MOG_ControllerSystem.ChangeSQLConnection(sqlForm.ConnectionString);
			}
		}

		private void miChangeRepository_Click(object sender, System.EventArgs e)
		{
			bool bRepositorySelected = false;

			while (!bRepositorySelected)
			{
				MogForm_RepositoryBrowser_ServerManager reposForm = new MogForm_RepositoryBrowser_ServerManager();
				if (reposForm.ShowDialog() == DialogResult.OK)
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
		}

		private void miImportDemoProject_Click(object sender, System.EventArgs e)
		{
			if (lvDemoProjects.SelectedItems.Count > 0)
			{
				List<string> projectNames = new List<string>();

				foreach (ListViewItem item in lvDemoProjects.SelectedItems)
				{
					projectNames.Add(item.SubItems[0].Text);
				}

				if (projectNames.Count > 0)
				{
					ImportDemoProjects(projectNames);
				}
			}
		}
		
		private void miRemoveDemoProject_Click(object sender, System.EventArgs e)
		{
			// Delete a demo project?
			if (this.lvDemoProjects.SelectedItems.Count > 0)
			{
				string projectNamesString = "Are you sure you want to remove these demo projects?\n";

				foreach (ListViewItem item in lvDemoProjects.SelectedItems)
				{
					projectNamesString += item.Text + "\n";
				}

				if (MOG_Prompt.PromptResponse("Confirm Demo Project Removal", projectNamesString, MOGPromptButtons.YesNo) == MOGPromptResult.Yes)
				{
					List<string> projectNames = new List<string>();

					foreach (ListViewItem item in lvDemoProjects.SelectedItems)
					{
						projectNames.Add(item.SubItems[0].Text);
					}

					RemoveDemoProjects(projectNames);
				}
			}
		}

		private void miExploreDemoProject_Click(object sender, System.EventArgs e)
		{
			if (lvDemoProjects.SelectedItems.Count > 0)
			{
				foreach (ListViewItem item in lvDemoProjects.SelectedItems)
				{
					//SubItems[2] is the filename
					string path = item.SubItems[2].Text;

					Process p = new Process();
					p.StartInfo.FileName = path;
					p.StartInfo.Arguments = "";
					p.StartInfo.WorkingDirectory = Path.GetDirectoryName(path);
					p.StartInfo.WindowStyle = ProcessWindowStyle.Normal;

					p.Start();
				}
			}
		}

		private void miDemoProjectDetails_Click(object sender, System.EventArgs e)
		{
			if (lvDemoProjects.SelectedItems.Count > 0)
			{
				foreach (ListViewItem item in lvDemoProjects.SelectedItems)
				{
					string detailsFile = item.SubItems[2].Text + @"\ProjectDetails.Info";

					if (!DosUtils.FileExistFast(detailsFile))
					{
						//There is currently no details file in the tools directory, let's make one
						FileStream fs = File.Create(detailsFile);
						fs.Close();
					}

					//Start me up!
					Process p = new Process();
					p.StartInfo.FileName = detailsFile;
					p.StartInfo.Arguments = "";
					p.StartInfo.WorkingDirectory = Path.GetDirectoryName(detailsFile);
					p.StartInfo.WindowStyle = ProcessWindowStyle.Normal;

					p.Start();
				}
			}
		}
		#endregion

		private void MOG_ServerManagerMainForm_Closed(object sender, System.EventArgs e)
		{
			Shutdown();
		}

		#region Help footer methods
		private void StartPageInfoRichTextBox_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			MogUtil_HelpManager.HelpTextAction(this, true);
		}

		private void StartPageInfoRichTextBox_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			MogUtil_HelpManager.HelpTextAction(this, false);			
		}

		private void HelpBarPictureBox_Click(object sender, System.EventArgs e)
		{
			this.MOGMainTabControl.SelectedIndex = 0;
		}

		private void MOGMainTabControl_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			switch(MOGMainTabControl.SelectedTab.Name)
			{
				case "tbStart":
					MogUtil_HelpManager.LoadHelpText(this, "WelcomeServerManager.rtf");			
					break;
				case "tpProjects":
					MogUtil_HelpManager.LoadHelpText(this, "ProjectConfiguration.rtf");			
					break;
				case "tpTrash":
					MogUtil_HelpManager.LoadHelpText(this, "TrashPolicy.rtf");			
					break;
				case "tpVersions":
					MogUtil_HelpManager.LoadHelpText(this, "VersionManager.rtf");			
					break;
				case "tbLicensing":
					MogUtil_HelpManager.LoadHelpText(this, "Licensing.rtf");
					break;
				case "tbRemoteServers":
					MogUtil_HelpManager.LoadHelpText(this, "RemoteServers.rtf");
					break;
			}
		}
		#endregion				

		private void StartupWebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			if (StartupWebBrowser.IsOffline || StartupWebBrowser.Document.Title == "Cannot find server")
			{
				StartupWebBrowser.Visible = false;
			}
		}

		private void ImportDemoProjects(List<string> projectNames)
		{
			if (projectNames.Count > 0)
			{
				ProgressDialog progress = new ProgressDialog("Importing Demo Projects", "Please wait while MOG imports the demo projects...", ImportDemoProjects_Worker, projectNames, true);
				progress.ShowDialog(this);
			}

			RefreshProjectsListView();
		}

		private void ImportDemoProjects_Worker(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			List<string> projectNames = e.Argument as List<string>;

			if (projectNames != null && projectNames.Count > 0)
			{
				for (int i = 0; i < projectNames.Count && !worker.CancellationPending; i++)
				{
					worker.ReportProgress(i * 100 / projectNames.Count, "Importing: " + projectNames[i]);

					MOG_ControllerDemoProject.ImportDemoProject(projectNames[i], worker);
				}
			}
		}

		private void DemoizeProjects(List<string> projectNames)
		{
			if (projectNames.Count > 0)
			{
				ProgressDialog progress = new ProgressDialog("Demoizing Project", "Please wait while MOG demoizes the projects...", DemoizeProjects_Worker, projectNames, false);
				progress.ShowDialog(this);
			}

			RefreshDemoProjectsListView();
		}

		private void DemoizeProjects_Worker(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			List<string> projectNames = e.Argument as List<string>;

			for (int i = 0; i < projectNames.Count && !worker.CancellationPending; i++)
			{
				worker.ReportProgress(i * 100 / projectNames.Count, "Demoizing " + projectNames[i]);

				MOG_ControllerDemoProject.DemoizeProject(projectNames[i], worker);
			}
		}

		private void AddDemoProjects(List<string> files)
		{
			if (files.Count > 0)
			{
				ProgressDialog progress = new ProgressDialog("Adding Demo Project", "Please wait while MOG adds the demo projects...", AddDemoProjects_Worker, files, true);
				progress.ShowDialog(this);
			}

			RefreshDemoProjectsListView();
		}

		private void AddDemoProjects_Worker(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			List<string> files = e.Argument as List<string>;

			for (int i = 0; i < files.Count; i++)
			{
				worker.ReportProgress(i * 100 / files.Count, "Adding " + files[i]);

				MOG_ControllerDemoProject.AddDemoProject(files[i], worker);
			}
		}

		private void RemoveDemoProjects(List<string> projectNames)
		{
			if (projectNames.Count > 0)
			{
				ProgressDialog progress = new ProgressDialog("Removing Demo Projects", "Please wait while MOG removes the demo projects...", RemoveDemoProjects_Worker, projectNames, true);
				progress.ShowDialog(this);
			}
			
			RefreshDemoProjectsListView();
		}

		private void RemoveDemoProjects_Worker(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			List<string> projectNames = e.Argument as List<string>;

			for (int i = 0; i < projectNames.Count; i++)
			{
				worker.ReportProgress(i * 100 / projectNames.Count, "Removing " + projectNames[i]);

				MOG_ControllerDemoProject.RemoveDemoProject(projectNames[i], worker);
			}
		}

		private void MOG_ServerManagerMainForm_Load(object sender, EventArgs e)
		{
			// Startup the mog process loop
			if (mMogProcess == null)
			{
				mMogProcess = new Thread(new ThreadStart(this.MogProcess));
				mMogProcess.Name = "MOG_ServerManagerMainForm.cs::MogProcess";
				mMogProcess.Start();
			}
		}
	}
}


