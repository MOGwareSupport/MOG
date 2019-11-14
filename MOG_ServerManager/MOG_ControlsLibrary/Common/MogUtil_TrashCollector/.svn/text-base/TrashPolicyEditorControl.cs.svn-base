using System;
using System.IO;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;

using MOG;
using MOG.INI;
using MOG.USER;
using MOG.PROMPT;
using MOG.PROJECT;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;

using MOG_ServerManager.Utilities;

namespace MOG_ServerManager
{
	/// <summary>
	/// Summary description for TrashPolicyEditorControl.
	/// </summary>
	public class TrashPolicyEditorControl : System.Windows.Forms.UserControl
	{
		#region System definitions

		private System.Windows.Forms.Button btnPurge;
		private System.Windows.Forms.Button btnReset;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.TreeView treeView;
		private System.Windows.Forms.CheckBox chbxOverridesGlobal;
		private System.Windows.Forms.CheckBox chbxOverridesProject;
		private System.Windows.Forms.NumericUpDown nudDays;
		private System.Windows.Forms.NumericUpDown nudHours;
		private System.Windows.Forms.NumericUpDown nudMinutes;
		private System.Windows.Forms.CheckBox chbxInterval;
		private System.Windows.Forms.Label lblDays;
		private System.Windows.Forms.Label lblHours;
		private System.Windows.Forms.Label lblMinutes;
		private System.Windows.Forms.Button btnSavePolicy;
		private System.Windows.Forms.Label lblFilename;
		private System.Windows.Forms.GroupBox gbConfig;
		private System.Windows.Forms.Label lblScopeLabel;
		private System.Windows.Forms.Label lblScope;
		private System.Windows.Forms.Button btnDeletePolicy;
		private System.Windows.Forms.Button btnBrowseRepository;
		private System.Windows.Forms.TextBox tbRepositoryLocation;
		private System.Windows.Forms.Button btnRefreshTree;
		private System.Windows.Forms.Button btnViewCommands;
		private System.Windows.Forms.Button btnTargets;
		private System.Windows.Forms.Button btnForcePurge;
		private System.Windows.Forms.Button btnProgramPurgeTimer;
		private System.Windows.Forms.CheckBox chbxPurgeTimerEnabled;
		private System.Windows.Forms.TextBox tbPurgeTimerInterval;
		private System.Windows.Forms.Label lblPurgeTimerSeconds;
		private System.Windows.Forms.NumericUpDown nudMegs;
		private System.Windows.Forms.CheckBox chbxMegs;
		private System.Windows.Forms.Label lblMegs;
		private System.Windows.Forms.NumericUpDown nudAssetAgeMinutes;
		private System.Windows.Forms.NumericUpDown nudAssetAgeDays;
		private System.Windows.Forms.NumericUpDown nudAssetAgeHours;
		private System.Windows.Forms.Label lblAssetAgeHours;
		private System.Windows.Forms.Label lblAssetAgeMinutes;
		private System.Windows.Forms.CheckBox chbxAssetAge;
		private System.Windows.Forms.Label lblAssetAgeDays;
		private System.Windows.Forms.ContextMenuStrip cmTrashCollectionTree;
		private System.Windows.Forms.ToolStripMenuItem miBrowseToTrashBin;
		private System.Windows.Forms.ToolStripMenuItem miSavePolicy;
		private System.Windows.Forms.ToolStripMenuItem miDeletePolicy;
		private System.Windows.Forms.ToolStripSeparator menuItem1;
		private System.Windows.Forms.ToolStripMenuItem miRefresh;
		private System.Windows.Forms.ToolStripMenuItem miForcePurge;
		private System.Windows.Forms.ToolStripSeparator menuItem2;
		private System.Windows.Forms.CheckBox chbxAutoSave;
		private System.Windows.Forms.ToolStripMenuItem miEditConfigFile;
		private System.Windows.Forms.ToolStripSeparator menuItem4;
		private System.Windows.Forms.ToolStripMenuItem miViewLogfile;
		private System.Windows.Forms.ToolStripMenuItem miClearLogfile;
		private System.Windows.Forms.ToolStripSeparator menuItem5;
		private System.Windows.Forms.ToolStripMenuItem miCreateTrashBin;
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
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
			this.btnPurge = new System.Windows.Forms.Button();
			this.btnReset = new System.Windows.Forms.Button();
			this.btnSave = new System.Windows.Forms.Button();
			this.treeView = new System.Windows.Forms.TreeView();
			this.cmTrashCollectionTree = new System.Windows.Forms.ContextMenuStrip();
			this.miBrowseToTrashBin = new System.Windows.Forms.ToolStripMenuItem();
			this.miCreateTrashBin = new System.Windows.Forms.ToolStripMenuItem();
			this.miEditConfigFile = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItem4 = new System.Windows.Forms.ToolStripSeparator();
			this.miForcePurge = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.miSavePolicy = new System.Windows.Forms.ToolStripMenuItem();
			this.miDeletePolicy = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItem2 = new System.Windows.Forms.ToolStripSeparator();
			this.miClearLogfile = new System.Windows.Forms.ToolStripMenuItem();
			this.miViewLogfile = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItem5 = new System.Windows.Forms.ToolStripSeparator();
			this.miRefresh = new System.Windows.Forms.ToolStripMenuItem();
			this.chbxOverridesGlobal = new System.Windows.Forms.CheckBox();
			this.chbxOverridesProject = new System.Windows.Forms.CheckBox();
			this.nudDays = new System.Windows.Forms.NumericUpDown();
			this.nudHours = new System.Windows.Forms.NumericUpDown();
			this.nudMinutes = new System.Windows.Forms.NumericUpDown();
			this.lblDays = new System.Windows.Forms.Label();
			this.lblHours = new System.Windows.Forms.Label();
			this.lblMinutes = new System.Windows.Forms.Label();
			this.chbxInterval = new System.Windows.Forms.CheckBox();
			this.btnSavePolicy = new System.Windows.Forms.Button();
			this.lblFilename = new System.Windows.Forms.Label();
			this.lblScopeLabel = new System.Windows.Forms.Label();
			this.gbConfig = new System.Windows.Forms.GroupBox();
			this.chbxAutoSave = new System.Windows.Forms.CheckBox();
			this.nudAssetAgeMinutes = new System.Windows.Forms.NumericUpDown();
			this.nudAssetAgeDays = new System.Windows.Forms.NumericUpDown();
			this.nudAssetAgeHours = new System.Windows.Forms.NumericUpDown();
			this.lblAssetAgeHours = new System.Windows.Forms.Label();
			this.lblAssetAgeMinutes = new System.Windows.Forms.Label();
			this.chbxAssetAge = new System.Windows.Forms.CheckBox();
			this.lblAssetAgeDays = new System.Windows.Forms.Label();
			this.nudMegs = new System.Windows.Forms.NumericUpDown();
			this.chbxMegs = new System.Windows.Forms.CheckBox();
			this.lblMegs = new System.Windows.Forms.Label();
			this.btnDeletePolicy = new System.Windows.Forms.Button();
			this.lblScope = new System.Windows.Forms.Label();
			this.btnBrowseRepository = new System.Windows.Forms.Button();
			this.tbRepositoryLocation = new System.Windows.Forms.TextBox();
			this.btnRefreshTree = new System.Windows.Forms.Button();
			this.btnViewCommands = new System.Windows.Forms.Button();
			this.btnTargets = new System.Windows.Forms.Button();
			this.btnForcePurge = new System.Windows.Forms.Button();
			this.tbPurgeTimerInterval = new System.Windows.Forms.TextBox();
			this.btnProgramPurgeTimer = new System.Windows.Forms.Button();
			this.chbxPurgeTimerEnabled = new System.Windows.Forms.CheckBox();
			this.lblPurgeTimerSeconds = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.nudDays)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudHours)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMinutes)).BeginInit();
			this.gbConfig.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudAssetAgeMinutes)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudAssetAgeDays)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudAssetAgeHours)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMegs)).BeginInit();
			this.SuspendLayout();
			// 
			// btnPurge
			// 
			this.btnPurge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnPurge.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnPurge.Location = new System.Drawing.Point(320, 168);
			this.btnPurge.Name = "btnPurge";
			this.btnPurge.TabIndex = 1;
			this.btnPurge.Text = "Purge";
			this.btnPurge.Click += new System.EventHandler(this.btnPurge_Click);
			// 
			// btnReset
			// 
			this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnReset.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnReset.Location = new System.Drawing.Point(320, 192);
			this.btnReset.Name = "btnReset";
			this.btnReset.TabIndex = 2;
			this.btnReset.Text = "Reset";
			this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
			// 
			// btnSave
			// 
			this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnSave.Location = new System.Drawing.Point(320, 216);
			this.btnSave.Name = "btnSave";
			this.btnSave.TabIndex = 3;
			this.btnSave.Text = "Save";
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// treeView
			// 
			this.treeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.treeView.ContextMenuStrip = this.cmTrashCollectionTree;
			this.treeView.HideSelection = false;
			this.treeView.ImageIndex = -1;
			this.treeView.Location = new System.Drawing.Point(8, 8);
			this.treeView.Name = "treeView";
			this.treeView.SelectedImageIndex = -1;
			this.treeView.Size = new System.Drawing.Size(296, 344);
			this.treeView.TabIndex = 4;
			this.treeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeView_MouseDown);
			this.treeView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.treeView_KeyUp);
			// 
			// cmTrashCollectionTree
			// 
			this.cmTrashCollectionTree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
																								  this.miBrowseToTrashBin,
																								  this.miCreateTrashBin,
																								  this.miEditConfigFile,
																								  this.menuItem4,
																								  this.miForcePurge,
																								  this.menuItem1,
																								  this.miSavePolicy,
																								  this.miDeletePolicy,
																								  this.menuItem2,
																								  this.miClearLogfile,
																								  this.miViewLogfile,
																								  this.menuItem5,
																								  this.miRefresh});
			this.cmTrashCollectionTree.Opening += this.cmTrashCollectionTree_Popup;
			// 
			// miBrowseToTrashBin
			// 
			this.miBrowseToTrashBin.Text = "&Browse to Trash Bin";
			this.miBrowseToTrashBin.Click += new System.EventHandler(this.miBrowseToTrashBin_Click);
			// 
			// miCreateTrashBin
			// 
			this.miCreateTrashBin.Text = "Create User Trash Bin";
			this.miCreateTrashBin.Click += new System.EventHandler(this.miCreateTrashBin_Click);
			// 
			// miEditConfigFile
			// 
			this.miEditConfigFile.Text = "&Edit configuration file...";
			this.miEditConfigFile.Click += new System.EventHandler(this.miEditConfigFile_Click);
			// 
			// menuItem4
			// 
			this.menuItem4.Text = "-";
			// 
			// miForcePurge
			// 
			this.miForcePurge.Text = "&Force Purge";
			this.miForcePurge.Click += new System.EventHandler(this.miForcePurge_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Text = "-";
			// 
			// miSavePolicy
			// 
			this.miSavePolicy.Text = "&Save Policy";
			this.miSavePolicy.Click += new System.EventHandler(this.miSavePolicy_Click);
			// 
			// miDeletePolicy
			// 
			this.miDeletePolicy.Text = "&Delete Policy";
			this.miDeletePolicy.Click += new System.EventHandler(this.miDeletePolicy_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Text = "-";
			// 
			// miClearLogfile
			// 
			this.miClearLogfile.Text = "&Clear logfile";
			this.miClearLogfile.Click += new System.EventHandler(this.miClearLogfile_Click);
			// 
			// miViewLogfile
			// 
			this.miViewLogfile.Text = "&View logfile...";
			this.miViewLogfile.Click += new System.EventHandler(this.miViewLogfile_Click);
			// 
			// menuItem5
			// 
			this.menuItem5.Text = "-";
			// 
			// miRefresh
			// 
			this.miRefresh.Text = "&Refresh";
			this.miRefresh.Click += new System.EventHandler(this.miRefresh_Click);
			// 
			// chbxOverridesGlobal
			// 
			this.chbxOverridesGlobal.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.chbxOverridesGlobal.Location = new System.Drawing.Point(24, 176);
			this.chbxOverridesGlobal.Name = "chbxOverridesGlobal";
			this.chbxOverridesGlobal.Size = new System.Drawing.Size(120, 16);
			this.chbxOverridesGlobal.TabIndex = 5;
			this.chbxOverridesGlobal.Text = "Overrides Global";
			// 
			// chbxOverridesProject
			// 
			this.chbxOverridesProject.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.chbxOverridesProject.Location = new System.Drawing.Point(24, 192);
			this.chbxOverridesProject.Name = "chbxOverridesProject";
			this.chbxOverridesProject.Size = new System.Drawing.Size(120, 16);
			this.chbxOverridesProject.TabIndex = 6;
			this.chbxOverridesProject.Text = "Overrides Project";
			// 
			// nudDays
			// 
			this.nudDays.Location = new System.Drawing.Point(40, 88);
			this.nudDays.Maximum = new System.Decimal(new int[] {
																	10000,
																	0,
																	0,
																	0});
			this.nudDays.Name = "nudDays";
			this.nudDays.Size = new System.Drawing.Size(48, 20);
			this.nudDays.TabIndex = 7;
			// 
			// nudHours
			// 
			this.nudHours.Location = new System.Drawing.Point(40, 112);
			this.nudHours.Maximum = new System.Decimal(new int[] {
																	 10000,
																	 0,
																	 0,
																	 0});
			this.nudHours.Name = "nudHours";
			this.nudHours.Size = new System.Drawing.Size(48, 20);
			this.nudHours.TabIndex = 8;
			// 
			// nudMinutes
			// 
			this.nudMinutes.Location = new System.Drawing.Point(40, 136);
			this.nudMinutes.Maximum = new System.Decimal(new int[] {
																	   10000,
																	   0,
																	   0,
																	   0});
			this.nudMinutes.Name = "nudMinutes";
			this.nudMinutes.Size = new System.Drawing.Size(48, 20);
			this.nudMinutes.TabIndex = 9;
			// 
			// lblDays
			// 
			this.lblDays.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblDays.Location = new System.Drawing.Point(88, 88);
			this.lblDays.Name = "lblDays";
			this.lblDays.Size = new System.Drawing.Size(56, 16);
			this.lblDays.TabIndex = 10;
			this.lblDays.Text = "Days";
			this.lblDays.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// lblHours
			// 
			this.lblHours.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblHours.Location = new System.Drawing.Point(88, 112);
			this.lblHours.Name = "lblHours";
			this.lblHours.Size = new System.Drawing.Size(56, 16);
			this.lblHours.TabIndex = 11;
			this.lblHours.Text = "Hours";
			this.lblHours.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// lblMinutes
			// 
			this.lblMinutes.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblMinutes.Location = new System.Drawing.Point(88, 136);
			this.lblMinutes.Name = "lblMinutes";
			this.lblMinutes.Size = new System.Drawing.Size(56, 16);
			this.lblMinutes.TabIndex = 12;
			this.lblMinutes.Text = "Minutes";
			this.lblMinutes.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// chbxInterval
			// 
			this.chbxInterval.Checked = true;
			this.chbxInterval.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chbxInterval.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.chbxInterval.Location = new System.Drawing.Point(24, 64);
			this.chbxInterval.Name = "chbxInterval";
			this.chbxInterval.Size = new System.Drawing.Size(120, 16);
			this.chbxInterval.TabIndex = 13;
			this.chbxInterval.Text = "Interval";
			this.chbxInterval.CheckedChanged += new System.EventHandler(this.chbxInterval_CheckedChanged);
			// 
			// btnSavePolicy
			// 
			this.btnSavePolicy.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnSavePolicy.Location = new System.Drawing.Point(192, 224);
			this.btnSavePolicy.Name = "btnSavePolicy";
			this.btnSavePolicy.Size = new System.Drawing.Size(88, 23);
			this.btnSavePolicy.TabIndex = 14;
			this.btnSavePolicy.Text = "Save Policy";
			this.btnSavePolicy.Click += new System.EventHandler(this.btnSavePolicy_Click);
			// 
			// lblFilename
			// 
			this.lblFilename.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left)));
			this.lblFilename.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblFilename.Location = new System.Drawing.Point(24, 264);
			this.lblFilename.Name = "lblFilename";
			this.lblFilename.Size = new System.Drawing.Size(368, 64);
			this.lblFilename.TabIndex = 15;
			// 
			// lblScopeLabel
			// 
			this.lblScopeLabel.Location = new System.Drawing.Point(24, 32);
			this.lblScopeLabel.Name = "lblScopeLabel";
			this.lblScopeLabel.Size = new System.Drawing.Size(40, 16);
			this.lblScopeLabel.TabIndex = 0;
			this.lblScopeLabel.Text = "Scope:";
			// 
			// gbConfig
			// 
			this.gbConfig.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.gbConfig.Controls.Add(this.chbxAutoSave);
			this.gbConfig.Controls.Add(this.nudAssetAgeMinutes);
			this.gbConfig.Controls.Add(this.nudAssetAgeDays);
			this.gbConfig.Controls.Add(this.nudAssetAgeHours);
			this.gbConfig.Controls.Add(this.lblAssetAgeHours);
			this.gbConfig.Controls.Add(this.lblAssetAgeMinutes);
			this.gbConfig.Controls.Add(this.chbxAssetAge);
			this.gbConfig.Controls.Add(this.lblAssetAgeDays);
			this.gbConfig.Controls.Add(this.nudMegs);
			this.gbConfig.Controls.Add(this.chbxMegs);
			this.gbConfig.Controls.Add(this.lblMegs);
			this.gbConfig.Controls.Add(this.btnDeletePolicy);
			this.gbConfig.Controls.Add(this.lblScope);
			this.gbConfig.Controls.Add(this.btnSavePolicy);
			this.gbConfig.Controls.Add(this.nudMinutes);
			this.gbConfig.Controls.Add(this.nudDays);
			this.gbConfig.Controls.Add(this.nudHours);
			this.gbConfig.Controls.Add(this.lblFilename);
			this.gbConfig.Controls.Add(this.lblScopeLabel);
			this.gbConfig.Controls.Add(this.lblHours);
			this.gbConfig.Controls.Add(this.lblMinutes);
			this.gbConfig.Controls.Add(this.chbxInterval);
			this.gbConfig.Controls.Add(this.chbxOverridesGlobal);
			this.gbConfig.Controls.Add(this.chbxOverridesProject);
			this.gbConfig.Controls.Add(this.lblDays);
			this.gbConfig.Location = new System.Drawing.Point(408, 8);
			this.gbConfig.Name = "gbConfig";
			this.gbConfig.Size = new System.Drawing.Size(416, 344);
			this.gbConfig.TabIndex = 17;
			this.gbConfig.TabStop = false;
			this.gbConfig.Text = "Policy Configuration";
			// 
			// chbxAutoSave
			// 
			this.chbxAutoSave.Location = new System.Drawing.Point(176, 200);
			this.chbxAutoSave.Name = "chbxAutoSave";
			this.chbxAutoSave.Size = new System.Drawing.Size(88, 16);
			this.chbxAutoSave.TabIndex = 29;
			this.chbxAutoSave.Text = "Auto-save";
			this.chbxAutoSave.CheckedChanged += new System.EventHandler(this.chbxAutoSave_CheckedChanged);
			// 
			// nudAssetAgeMinutes
			// 
			this.nudAssetAgeMinutes.Location = new System.Drawing.Point(176, 136);
			this.nudAssetAgeMinutes.Maximum = new System.Decimal(new int[] {
																			   10000,
																			   0,
																			   0,
																			   0});
			this.nudAssetAgeMinutes.Name = "nudAssetAgeMinutes";
			this.nudAssetAgeMinutes.Size = new System.Drawing.Size(48, 20);
			this.nudAssetAgeMinutes.TabIndex = 24;
			// 
			// nudAssetAgeDays
			// 
			this.nudAssetAgeDays.Location = new System.Drawing.Point(176, 88);
			this.nudAssetAgeDays.Maximum = new System.Decimal(new int[] {
																			10000,
																			0,
																			0,
																			0});
			this.nudAssetAgeDays.Name = "nudAssetAgeDays";
			this.nudAssetAgeDays.Size = new System.Drawing.Size(48, 20);
			this.nudAssetAgeDays.TabIndex = 22;
			// 
			// nudAssetAgeHours
			// 
			this.nudAssetAgeHours.Location = new System.Drawing.Point(176, 112);
			this.nudAssetAgeHours.Maximum = new System.Decimal(new int[] {
																			 10000,
																			 0,
																			 0,
																			 0});
			this.nudAssetAgeHours.Name = "nudAssetAgeHours";
			this.nudAssetAgeHours.Size = new System.Drawing.Size(48, 20);
			this.nudAssetAgeHours.TabIndex = 23;
			// 
			// lblAssetAgeHours
			// 
			this.lblAssetAgeHours.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblAssetAgeHours.Location = new System.Drawing.Point(224, 112);
			this.lblAssetAgeHours.Name = "lblAssetAgeHours";
			this.lblAssetAgeHours.Size = new System.Drawing.Size(56, 16);
			this.lblAssetAgeHours.TabIndex = 26;
			this.lblAssetAgeHours.Text = "Hours";
			this.lblAssetAgeHours.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// lblAssetAgeMinutes
			// 
			this.lblAssetAgeMinutes.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblAssetAgeMinutes.Location = new System.Drawing.Point(224, 136);
			this.lblAssetAgeMinutes.Name = "lblAssetAgeMinutes";
			this.lblAssetAgeMinutes.Size = new System.Drawing.Size(56, 16);
			this.lblAssetAgeMinutes.TabIndex = 27;
			this.lblAssetAgeMinutes.Text = "Minutes";
			this.lblAssetAgeMinutes.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// chbxAssetAge
			// 
			this.chbxAssetAge.Checked = true;
			this.chbxAssetAge.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chbxAssetAge.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.chbxAssetAge.Location = new System.Drawing.Point(160, 64);
			this.chbxAssetAge.Name = "chbxAssetAge";
			this.chbxAssetAge.Size = new System.Drawing.Size(120, 16);
			this.chbxAssetAge.TabIndex = 28;
			this.chbxAssetAge.Text = "Asset Age";
			this.chbxAssetAge.CheckedChanged += new System.EventHandler(this.chbxAssetAge_CheckedChanged);
			// 
			// lblAssetAgeDays
			// 
			this.lblAssetAgeDays.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblAssetAgeDays.Location = new System.Drawing.Point(224, 88);
			this.lblAssetAgeDays.Name = "lblAssetAgeDays";
			this.lblAssetAgeDays.Size = new System.Drawing.Size(56, 16);
			this.lblAssetAgeDays.TabIndex = 25;
			this.lblAssetAgeDays.Text = "Days";
			this.lblAssetAgeDays.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// nudMegs
			// 
			this.nudMegs.Location = new System.Drawing.Point(304, 88);
			this.nudMegs.Maximum = new System.Decimal(new int[] {
																	100000,
																	0,
																	0,
																	0});
			this.nudMegs.Name = "nudMegs";
			this.nudMegs.Size = new System.Drawing.Size(48, 20);
			this.nudMegs.TabIndex = 19;
			// 
			// chbxMegs
			// 
			this.chbxMegs.Checked = true;
			this.chbxMegs.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chbxMegs.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.chbxMegs.Location = new System.Drawing.Point(288, 64);
			this.chbxMegs.Name = "chbxMegs";
			this.chbxMegs.Size = new System.Drawing.Size(120, 16);
			this.chbxMegs.TabIndex = 21;
			this.chbxMegs.Text = "Trash Bin Size";
			this.chbxMegs.CheckedChanged += new System.EventHandler(this.chbxMegs_CheckedChanged);
			// 
			// lblMegs
			// 
			this.lblMegs.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblMegs.Location = new System.Drawing.Point(352, 88);
			this.lblMegs.Name = "lblMegs";
			this.lblMegs.Size = new System.Drawing.Size(56, 16);
			this.lblMegs.TabIndex = 20;
			this.lblMegs.Text = "Megs";
			this.lblMegs.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// btnDeletePolicy
			// 
			this.btnDeletePolicy.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnDeletePolicy.Location = new System.Drawing.Point(288, 224);
			this.btnDeletePolicy.Name = "btnDeletePolicy";
			this.btnDeletePolicy.Size = new System.Drawing.Size(88, 23);
			this.btnDeletePolicy.TabIndex = 18;
			this.btnDeletePolicy.Text = "Delete Policy";
			this.btnDeletePolicy.Click += new System.EventHandler(this.btnDeletePolicy_Click);
			// 
			// lblScope
			// 
			this.lblScope.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblScope.Location = new System.Drawing.Point(64, 32);
			this.lblScope.Name = "lblScope";
			this.lblScope.Size = new System.Drawing.Size(100, 16);
			this.lblScope.TabIndex = 17;
			// 
			// btnBrowseRepository
			// 
			this.btnBrowseRepository.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnBrowseRepository.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnBrowseRepository.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.btnBrowseRepository.Location = new System.Drawing.Point(280, 368);
			this.btnBrowseRepository.Name = "btnBrowseRepository";
			this.btnBrowseRepository.Size = new System.Drawing.Size(24, 23);
			this.btnBrowseRepository.TabIndex = 18;
			this.btnBrowseRepository.Text = "...";
			this.btnBrowseRepository.Click += new System.EventHandler(this.btnBrowseRepository_Click);
			// 
			// tbRepositoryLocation
			// 
			this.tbRepositoryLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tbRepositoryLocation.Location = new System.Drawing.Point(16, 368);
			this.tbRepositoryLocation.Name = "tbRepositoryLocation";
			this.tbRepositoryLocation.Size = new System.Drawing.Size(256, 20);
			this.tbRepositoryLocation.TabIndex = 19;
			this.tbRepositoryLocation.Text = "";
			// 
			// btnRefreshTree
			// 
			this.btnRefreshTree.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnRefreshTree.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnRefreshTree.Location = new System.Drawing.Point(320, 240);
			this.btnRefreshTree.Name = "btnRefreshTree";
			this.btnRefreshTree.TabIndex = 20;
			this.btnRefreshTree.Text = "Refresh";
			this.btnRefreshTree.Click += new System.EventHandler(this.btnRefreshTree_Click);
			// 
			// btnViewCommands
			// 
			this.btnViewCommands.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnViewCommands.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnViewCommands.Location = new System.Drawing.Point(320, 264);
			this.btnViewCommands.Name = "btnViewCommands";
			this.btnViewCommands.TabIndex = 21;
			this.btnViewCommands.Text = "Commands";
			this.btnViewCommands.Click += new System.EventHandler(this.btnViewCommands_Click);
			// 
			// btnTargets
			// 
			this.btnTargets.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnTargets.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnTargets.Location = new System.Drawing.Point(320, 288);
			this.btnTargets.Name = "btnTargets";
			this.btnTargets.TabIndex = 22;
			this.btnTargets.Text = "Targets";
			this.btnTargets.Click += new System.EventHandler(this.btnTargets_Click);
			// 
			// btnForcePurge
			// 
			this.btnForcePurge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnForcePurge.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnForcePurge.Location = new System.Drawing.Point(320, 312);
			this.btnForcePurge.Name = "btnForcePurge";
			this.btnForcePurge.TabIndex = 23;
			this.btnForcePurge.Text = "Force Purge";
			this.btnForcePurge.Click += new System.EventHandler(this.btnForcePurge_Click);
			// 
			// tbPurgeTimerInterval
			// 
			this.tbPurgeTimerInterval.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.tbPurgeTimerInterval.Enabled = false;
			this.tbPurgeTimerInterval.Location = new System.Drawing.Point(448, 384);
			this.tbPurgeTimerInterval.Name = "tbPurgeTimerInterval";
			this.tbPurgeTimerInterval.Size = new System.Drawing.Size(48, 20);
			this.tbPurgeTimerInterval.TabIndex = 27;
			this.tbPurgeTimerInterval.Text = "";
			// 
			// btnProgramPurgeTimer
			// 
			this.btnProgramPurgeTimer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnProgramPurgeTimer.Enabled = false;
			this.btnProgramPurgeTimer.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnProgramPurgeTimer.Location = new System.Drawing.Point(360, 384);
			this.btnProgramPurgeTimer.Name = "btnProgramPurgeTimer";
			this.btnProgramPurgeTimer.TabIndex = 25;
			this.btnProgramPurgeTimer.Text = "Program";
			this.btnProgramPurgeTimer.Click += new System.EventHandler(this.btnProgramPurgeTimer_Click);
			// 
			// chbxPurgeTimerEnabled
			// 
			this.chbxPurgeTimerEnabled.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.chbxPurgeTimerEnabled.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.chbxPurgeTimerEnabled.Location = new System.Drawing.Point(344, 360);
			this.chbxPurgeTimerEnabled.Name = "chbxPurgeTimerEnabled";
			this.chbxPurgeTimerEnabled.Size = new System.Drawing.Size(136, 16);
			this.chbxPurgeTimerEnabled.TabIndex = 26;
			this.chbxPurgeTimerEnabled.Text = "Purge Timer Enabled";
			this.chbxPurgeTimerEnabled.CheckedChanged += new System.EventHandler(this.chbxPurgeTimerEnabled_CheckedChanged);
			// 
			// lblPurgeTimerSeconds
			// 
			this.lblPurgeTimerSeconds.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.lblPurgeTimerSeconds.Enabled = false;
			this.lblPurgeTimerSeconds.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblPurgeTimerSeconds.Location = new System.Drawing.Point(496, 384);
			this.lblPurgeTimerSeconds.Name = "lblPurgeTimerSeconds";
			this.lblPurgeTimerSeconds.Size = new System.Drawing.Size(56, 23);
			this.lblPurgeTimerSeconds.TabIndex = 0;
			this.lblPurgeTimerSeconds.Text = "seconds";
			this.lblPurgeTimerSeconds.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// TrashPolicyEditorControl
			// 
			this.Controls.Add(this.tbRepositoryLocation);
			this.Controls.Add(this.btnForcePurge);
			this.Controls.Add(this.btnTargets);
			this.Controls.Add(this.btnViewCommands);
			this.Controls.Add(this.btnRefreshTree);
			this.Controls.Add(this.btnBrowseRepository);
			this.Controls.Add(this.gbConfig);
			this.Controls.Add(this.treeView);
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.btnReset);
			this.Controls.Add(this.btnPurge);
			this.Controls.Add(this.tbPurgeTimerInterval);
			this.Controls.Add(this.btnProgramPurgeTimer);
			this.Controls.Add(this.chbxPurgeTimerEnabled);
			this.Controls.Add(this.lblPurgeTimerSeconds);
			this.Name = "TrashPolicyEditorControl";
			this.Size = new System.Drawing.Size(832, 416);
			((System.ComponentModel.ISupportInitialize)(this.nudDays)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudHours)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMinutes)).EndInit();
			this.gbConfig.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.nudAssetAgeMinutes)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudAssetAgeDays)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudAssetAgeHours)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMegs)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion
		#endregion
		#region Member vars
		private TrashCollector trashCollector;
		private ArrayList nodes = new ArrayList();
		private string loadedRepository = "C:\\";
		private Timer purgeTimer = new Timer();
		private Timer resetTimer = new Timer();
		private string logFilename = "";
		private bool logActions = true;
		#endregion
		#region Properties
		public bool LogActions
		{
			get { return this.logActions; }
			set { this.logActions = value; }
		}
		public string LogFilename
		{
			get { return this.logFilename; }
			set { this.logFilename = value; }
		}
		public string LoadedRepository
		{
			get { return this.loadedRepository; }
		}
		#endregion
		#region Constructors
		public TrashPolicyEditorControl()
		{
			InitializeComponent();

			this.logFilename = Environment.CurrentDirectory + "\\trashlog.txt";

			this.trashCollector = new TrashCollector();
			this.trashCollector.LogFilename = this.logFilename;
			this.trashCollector.LogActions = true;

			//LoadMOGRepository("C:\\root\\tools\\mog.ini");
			//this.tbRepositoryLocation.Text = "C:\\root\\tools\\mog.ini";

			this.purgeTimer.Tick += new EventHandler(purgeTimer_Tick);
			this.purgeTimer.Interval = 5000;		// each second
			this.purgeTimer.Enabled = false;
			
			this.tbPurgeTimerInterval.Text = "5";
		}
		#endregion
		#region Public functions
		public void LoadRepository(string iniFilename)
		{
			if (File.Exists(iniFilename))
			{
				this.tbRepositoryLocation.Text = iniFilename;

				this.treeView.Nodes.Clear();
				this.nodes.Clear();
				this.trashCollector.SaveCommands();
				this.trashCollector.ClearCommands();
				LoadMOGRepository(iniFilename);
			}
		}
		#endregion
		#region Private functions
		private void ShowArrayList(ArrayList list)
		{
			if (list == null)
			{
				MessageBox.Show("ShowArrayList(): list = null", "ShowArrayList()");
				return;
			}

			string msg = "";
			foreach (object obj in list)
			{
				if (obj is string)
					msg += (string)obj + "\n";
				else
					msg += obj.ToString() + "\n";
			}

			MessageBox.Show(msg, "ShowArrayList()");
		}

		private void SavePolicyFromGUI(PurgePolicy p)
		{
			if (p == null)
				return;
			
			try 
			{
				if (File.Exists(p.filename))
					File.Delete(p.filename);
			}
			catch (Exception e)
			{
				MessageBox.Show("Couldn't save " + p.filename + "\n\n" + e.Message + "\n" + e.StackTrace, "Exception");
				return;
			}

			MOG_Ini ini = new MOG_Ini(p.filename);
			if (ini == null)
				return;

			ini.PutString("TRASH", "scope", p.scope.ToString());
			ini.PutString("TRASH", "OverridesGlobal", this.chbxOverridesGlobal.Checked.ToString());
			ini.PutString("TRASH", "OverridesProject", this.chbxOverridesProject.Checked.ToString());

			// Make sure this policy is loaded with valid targets
			if (p.targets != null && p.targets.Count > 0)
			{
				if (p.targets[0] is UserTrashBin)
				{
					foreach (UserTrashBin utb in p.targets)
						ini.PutString("TARGETS", utb.TrashPath, "");
				}
				else
				{
					foreach (string trashPath in p.targets)
						ini.PutString("TARGETS", trashPath, "");
				}
			}			

			// Interval
			if (this.chbxInterval.Checked)
			{
				ini.PutString("TRASH", "interval", "");

				ini.PutString("INTERVAL", "LastExecuted", (new DateTime(1776, 7, 4)).ToString());
				ini.PutString("INTERVAL", "DayInterval", this.nudDays.Value.ToString());
				ini.PutString("INTERVAL", "HourInterval", this.nudHours.Value.ToString());
				ini.PutString("INTERVAL", "MinInterval", this.nudMinutes.Value.ToString());
			}
			
			// Asset Age
			if (this.chbxAssetAge.Checked)
			{
				ini.PutString("TRASH", "age", "");

				ini.PutString("AGE", "LastExecuted", (new DateTime(1776, 7, 4)).ToString());
				ini.PutString("AGE", "DayInterval", this.nudAssetAgeDays.Value.ToString());
				ini.PutString("AGE", "HourInterval", this.nudAssetAgeHours.Value.ToString());
				ini.PutString("AGE", "MinInterval", this.nudAssetAgeMinutes.Value.ToString());
			}

			// Megs
			if (this.chbxMegs.Checked)
			{
				ini.PutString("TRASH", "megs", "");
				ini.PutString("MEGS", "LastExecuted", (new DateTime(1776, 7, 4)).ToString());
				ini.PutString("MEGS", "MegLimit", this.nudMegs.Value.ToString());
			}

			ini.Save();
			ini.Close();

			RefreshTreeViewFromDisk();
		}

		private void RefreshTreeViewFromDisk()
		{
			this.treeView.BeginUpdate();
			string path = "";
			if (this.treeView.SelectedNode != null)
				path = this.treeView.SelectedNode.FullPath;
			ArrayList expandedNodes = EncodeExpandedState(this.treeView.Nodes);
			this.treeView.Nodes.Clear();
			this.nodes.Clear();
			this.trashCollector.SaveCommands();
			this.trashCollector.ClearCommands();
			LoadMOGRepository(this.loadedRepository);
			this.treeView.CollapseAll();
			RestoreExpandedState(this.treeView.Nodes, expandedNodes);
			SelectNodeFromPath(this.treeView.Nodes, path);
			if (this.treeView.SelectedNode != null  &&  this.treeView.SelectedNode.Tag is PurgePolicy)
				LoadPolicyToGUI( this.treeView.SelectedNode );
			this.treeView.EndUpdate();
		}

		private void SelectNodeFromPath(TreeNodeCollection nodes, string path)
		{
			if (nodes == null)
				return;

			foreach (TreeNode node in nodes)
			{
				if (path.ToLower() == node.FullPath.ToLower())
				{
					this.treeView.SelectedNode = node;
					node.EnsureVisible();
					return;
				}

				SelectNodeFromPath(node.Nodes, path);
			}
		}

		private ArrayList EncodeExpandedState(TreeNodeCollection nodes)
		{
			ArrayList expandedNodes = new ArrayList();

			foreach (TreeNode node in nodes)
			{
				if (node.IsExpanded)
					expandedNodes.Add(node.FullPath.ToLower());

				expandedNodes.AddRange( EncodeExpandedState(node.Nodes) );
			}

			return expandedNodes;
		}

		private void RestoreExpandedState(TreeNodeCollection nodes, ArrayList expandedNodes)
		{
			foreach (TreeNode node in nodes)
			{
				if (expandedNodes.Contains(node.FullPath.ToLower()))
					node.Expand();

				RestoreExpandedState(node.Nodes, expandedNodes);
			}
		}
		
		private void LoadPolicyToGUI(TreeNode tn)
		{
			if (tn == null)
				return;

			PurgePolicy p = tn.Tag as PurgePolicy;

			if (p != null)
			{
				this.gbConfig.Text = "Policy Configuration - " + tn.Text;
				this.gbConfig.Enabled = true;

				this.lblScope.Text = p.scope.ToString();

				this.chbxOverridesGlobal.Checked = p.overridesGlobal;
				this.chbxOverridesProject.Checked = p.overridesProject;

				if (p.scope == PurgeCommandScope.GLOBAL)
				{
					this.chbxOverridesGlobal.Enabled = false;
					this.chbxOverridesProject.Enabled = false;
				}
				else if (p.scope == PurgeCommandScope.PROJECT)
				{
					this.chbxOverridesGlobal.Enabled = true;
					this.chbxOverridesProject.Enabled = false;
				}
				else if (p.scope == PurgeCommandScope.USER)
				{
					this.chbxOverridesGlobal.Enabled = true;
					this.chbxOverridesProject.Enabled = true;
				}
			
				// Interval
				this.chbxInterval.Checked = p.interval;
				this.nudDays.Value = p.dayInterval;
				this.nudHours.Value = p.hourInterval;
				this.nudMinutes.Value = p.minInterval;

				// Asse Age
				this.chbxAssetAge.Checked = p.age;
				this.nudAssetAgeDays.Value = p.ageDayInterval;
				this.nudAssetAgeHours.Value = p.ageHourInterval;
				this.nudAssetAgeMinutes.Value = p.ageMinInterval;

				// Megs
				this.chbxMegs.Checked = p.megs;
				this.nudMegs.Value = p.megLimit;

				this.lblFilename.Text = p.filename;

				if (p.empty)
					this.lblFilename.Text = this.lblFilename.Text.Insert(0, "( --> !!EMPTY!! <-- )");
			}
		}

		// ------------------------------------------------------------
		//  Loads global, project, and user trash policies based on the MOG ini configuration files specified in
		//   mogConfigFilename
		private void LoadMOGRepository(string mogConfigFilename)
		{
			if (!File.Exists(mogConfigFilename))
				return;

			this.loadedRepository = mogConfigFilename;

			TreeNode rootNode = null;

			// load global policy
			string policyFilename = ExtractTrashPolicyFilename(mogConfigFilename);
			PurgePolicy policy = LoadPolicy( policyFilename );
			
			rootNode = new TreeNode( "Global" );
			if ( policy == null)
			{
				// set up grey (indicating no policy) global node
				policy = new PurgePolicy();
				policy.empty = true;
				policy.filename = policyFilename;
				policy.scope = PurgeCommandScope.GLOBAL;
				policy.targets = ExtractAllUserTrashDirs( mogConfigFilename );
				rootNode.ForeColor = Color.Gray;
			}
			else
			{
				// set up red (loaded) global node
				rootNode.ForeColor = Color.Red;
			}
			rootNode.Tag = (PurgePolicy)policy.Clone();

			foreach (string projName in MOG_ControllerSystem.GetSystem().GetProjectNames())
			{
				MOG_Project proj = MOG_ControllerSystem.GetSystem().GetProject(projName);
				if (proj == null)
					continue;

				// load project policies
				TreeNode projectNode = null;

				policyFilename = proj.GetProjectToolsPath() + "\\trashpolicy.info";
				policy = LoadPolicy( policyFilename );
				
				projectNode = new TreeNode( proj.GetProjectName() );
				if (projectNode != null)
				{
					if ( policy == null )
					{
						// set up grey (indicating no policy) project node
						policy = new PurgePolicy();
						policy.empty = true;
						policy.filename = policyFilename;
						policy.scope = PurgeCommandScope.PROJECT;
						policy.targets = policy.GetTargetDirs();
						projectNode.ForeColor = Color.Gray;
					}
					else
					{
						// set up blue (loaded) project policy node
						projectNode.ForeColor = Color.Blue;
					}
					projectNode.Tag = (PurgePolicy)policy.Clone();

					//// load user policies
					//foreach (MOG_User user in proj.GetUsers())
					//{
					//    string userTrashPolicyFilename = user.GetUserToolsPath() + "\\trashpolicy.info";
					//
					//    TreeNode userNode = new TreeNode( Path.GetFileName(Path.GetDirectoryName(Path.GetDirectoryName(userTrashPolicyFilename))) );
					//
					//    policy = LoadPolicy( userTrashPolicyFilename );
					//    if ( policy == null )
					//    {
					//        // set up grey (indicating no policy) user node
					//        policy = new PurgePolicy();
					//        policy.empty = true;
					//        policy.filename = userTrashPolicyFilename;
					//        policy.scope = PurgeCommandScope.USER;
					//        policy.targets.Add( user.GetUserPath() + "\\Trash" );
					//        userNode.ForeColor = Color.Gray;
					//    }
					//    else
					//    {
					//        // set up green (loaded) user node
					//        userNode.ForeColor = Color.Green;
					//    }
					//    userNode.Tag = (PurgePolicy)policy.Clone();
					//    projectNode.Nodes.Add(userNode);
					//}
				}

				if (rootNode != null && projectNode != null)
					rootNode.Nodes.Add(projectNode);
			}

#if false
			//Add nodes for the deleted projects
			foreach (string projName in MOG_ControllerSystem.GetSystem().GetDeletedProjectNames())
			{
				TreeNode projectNode = new TreeNode(projName);
				projectNode.ForeColor = Color.Red;
				
				if (rootNode != null)
					rootNode.Nodes.Add(projectNode);
			}
#endif

			if (rootNode != null)
			{
				nodes.Add(rootNode);
				treeView.Nodes.Add(rootNode);
				treeView.ExpandAll();
			}
		}

		// ------------------------------------------------------------------------
		//  get the Users token value under [Project]
		private string ExtractUsersDir(string mogIniFilename)
		{
			if ( File.Exists(mogIniFilename) )
				return ExtractUsersDir( new MOG_Ini(mogIniFilename) );
			else
				return "INVALID FILENAME {" + mogIniFilename + "}";
		}
		private string ExtractUsersDir(MOG_Ini ini)
		{
			if (ini == null)
				return "ini was null";

			if (!ini.KeyExist("Project", "Users"))
				return "[Project] : Users key doesn't exist in {" + ini.GetFilename() + "}";

			return ini.GetString("Project", "Users");
		}

		
		// ----------------------------------------------------------------
		//  Returns an ArrayList of UserTrashBins corresponding to all the users
		//  in all the projects specified in mogIniFilename
		private ArrayList ExtractAllUserTrashDirs(string mogIniFilename)
		{
			if (File.Exists(mogIniFilename))
				return ExtractAllUserTrashDirs( new MOG_Ini(mogIniFilename) );
			else
				return new ArrayList();
		}
		private ArrayList ExtractAllUserTrashDirs(MOG_Ini ini)
		{
			ArrayList trashDirs = new ArrayList();

			if (ini == null)
				return trashDirs;

			if (ini.SectionExist("Projects"))
			{
				// this is a global MOG ini file

				// for each project, load its users
				foreach (string projConfigFilename in ExtractProjectConfigFilenames(ini))
					trashDirs.AddRange( ExtractAllUserTrashDirs(projConfigFilename) );
			}
			else if (ini.SectionExist("Users"))
			{
				// this is a project ini file

				// for each user, add its Trash folder
				string userDir = ExtractUsersDir(ini);
				if (Directory.Exists(userDir))
				{
					for (int usrIndex = 0; usrIndex < ini.CountKeys("Users"); usrIndex++)
					{
						string usrName = ini.GetKeyNameByIndexSLOW("Users", usrIndex);
						trashDirs.Add( this.trashCollector.GetTrashBin( userDir + "\\" + usrName + "\\Trash" ) );
					}
				}
			}

			return trashDirs;
		}

		// ------------------------------------------------------------------------------------------------------
		//  ExtractTrashPolicyFilename() - Looks in the MOG INI file specified by passed-in 'ini' and retruns
		//									the value of the 'TrashPurgePolicy' variable under the MOG heading.
		//								   If no 'TrashPurgePolicy' key exists, returns 'trashpolicy.info' in the same
		//									directory that ini's INI file is located (i.e., if ini is editing
		//									'C:\MOG\TOOLS\mog.ini' and no 'TrashPurgePolicy' variable is found
		//									in it, 'C:\MOG\TOOLS\trashpolicy.info' will be returned).
		//								   Does not check to make sure returned file exists.
		//								   If ini is null, returns empty string.
		//								   Takes no action on ini (i.e., doesn't close it when finished)
		private string ExtractTrashPolicyFilename(MOG_Ini ini)
		{
			if (ini == null)
				return "";

			// if this is a global MOG ini
			if (ini.KeyExist("MOG", "TrashPurgePolicy") )
				return ini.GetString("MOG", "TrashPurgePolicy");

			// if this is a project ini
			if (ini.KeyExist("Project", "TrashPurgePolicy"))
				return ini.GetString("Project", "TrashPurgePolicy");
			
			// no path specified in ini, return default policy filename
			return Path.GetDirectoryName( ini.GetFilename() ) + "\\trashpolicy.info";
		}
		private string ExtractTrashPolicyFilename(string mogIniFilename)
		{
			return ExtractTrashPolicyFilename( new MOG_Ini(mogIniFilename) );
		}

		// -------------------------------------------------------------------------
		//  Returns an ArrayList containing the project ConfigFile token values in the passed-in ini
		private ArrayList ExtractProjectConfigFilenames(MOG_Ini ini)
		{
			ArrayList configFilenames = new ArrayList();

			if (ini == null)
				return configFilenames;

			if (ini.SectionExist("Projects"))
			{
				for (int projIndex=0; projIndex<ini.CountKeys("Projects"); projIndex++)
				{
					string projName = ini.GetKeyNameByIndexSLOW("Projects", projIndex);
					if (ini.KeyExist(projName, "ConfigFile"))
					{
						string configFilename = ini.GetString(projName, "ConfigFile");
						string untokenizedConfigFilename = configFilename.ToLower().Replace(MOG_Tokens.GetSystemRepositoryPath().ToLower(), MOG_ControllerSystem.GetSystemRepositoryPath());
						// Temp Code to help migrate old untokenized configFilenames
						if (string.Compare(configFilename, untokenizedConfigFilename, true) == 0)
						{
							// Save out a properly tokenized configFilename
							string tokenizedConfigFilename = configFilename.ToLower().Replace(MOG_ControllerSystem.GetSystemRepositoryPath().ToLower(), MOG_Tokens.GetSystemRepositoryPath());
							MOG_ControllerSystem.GetSystem().GetConfigFile().PutString(projName, "ConfigFile", tokenizedConfigFilename);
							MOG_ControllerSystem.GetSystem().GetConfigFile().Save();
						}
						configFilenames.Add( untokenizedConfigFilename );
					}
				}
			}

			return configFilenames;
		}
		private ArrayList ExtractProjectConfigFilenames(string mogIniFilename)
		{
			return ExtractProjectConfigFilenames( new MOG_Ini(mogIniFilename) );
		}

		// -------------------------------------------------------------------------
		//  Attempts to construct valid trash purge policy filenames from the list
		//  of users (if any) in the passed-in ini (based on ini's path and
		//  the usernames), and returns them in an ArrayList.
		private ArrayList ExtractUserTrashPolicyFilenames(MOG_Ini ini)
		{
			ArrayList policyFilenames = new ArrayList();

			if (ini == null)
				return policyFilenames;

			ArrayList userList = MOG_ControllerProject.GetProject().GetUsers();

			if (userList != null  &&  userList.Count > 0)
			{
				foreach (MOG_User user in userList)
				{
					// construct the base path of the user directories
					string userPath = user.GetUserPath();
					userPath += "\\Tools\\trashpolicy.info";
					policyFilenames.Add( userPath );
				}
			}

			return policyFilenames;
		}
		private ArrayList ExtractUserTrashPolicyFilenames(string mogIniFilename)
		{
			return ExtractUserTrashPolicyFilenames( new MOG_Ini(mogIniFilename) );
		}

		private PurgePolicy LoadPolicy(string configFilename)
		{
			if (!File.Exists(configFilename))
				return null;

			MOG_Ini ini = new MOG_Ini(configFilename);
			if (ini == null)
				return null;

			// check for neccessary sections
			if (!ini.SectionExist("TRASH"))
				return null;

			PurgePolicy policy = new PurgePolicy();
			policy.filename = configFilename;

			// JKB TODO : Put last run code here

			// get scope
			PurgeCommandScope scope = PurgeCommandScope.NONE;
			if (ini.KeyExist("TRASH", "Scope"))
			{
				string scopeString = ini.GetString("TRASH", "Scope");
				if (scopeString.ToLower() == "global")
					scope = PurgeCommandScope.GLOBAL;
				else if (scopeString.ToLower() == "project")
					scope = PurgeCommandScope.PROJECT;
				else if (scopeString.ToLower() == "user")
					scope = PurgeCommandScope.USER;
			}
			policy.scope = scope;

			// get override policy
			bool overridesGlobal = false;		// default to false if key doesn't exist
			bool overridesProject = false;
			if (ini.KeyExist("TRASH", "OverridesGlobal"))
				overridesGlobal = bool.Parse( ini.GetString("TRASH", "OverridesGlobal") );
			if (ini.KeyExist("TRASH", "OverridesProject"))
				overridesProject = bool.Parse( ini.GetString("TRASH", "OverridesProject") );
			policy.overridesGlobal = overridesGlobal;
			policy.overridesProject = overridesProject;


			// get target trash folders
			ArrayList targetBins = new ArrayList();
			if (scope == PurgeCommandScope.GLOBAL)
			{
				foreach (string projName in MOG.CONTROLLER.CONTROLLERSYSTEM.MOG_ControllerSystem.GetSystem().GetProjectNames())
				{
					MOG_Project proj = MOG_ControllerProject.LoginProject(projName, "");
					if (proj != null)
					{
						foreach (MOG_User user in proj.GetUsers())
						{
							UserTrashBin bin = this.trashCollector.GetTrashBin(user.GetUserPath() + "\\Trash");
							if (bin != null)
							{
								bin.OverridesGlobal = overridesGlobal;
								bin.OverridesProject = overridesProject;
								targetBins.Add( bin );
							}
						}
					}

					policy.targets = (ArrayList)targetBins.Clone();
				}
			}
			else if (scope == PurgeCommandScope.PROJECT)
			{
				MOG_Project proj = MOG_ControllerProject.GetProject();
				
				if (proj != null)
				{
					foreach (MOG_User user in proj.GetUsers())
					{
						UserTrashBin bin = this.trashCollector.GetTrashBin(user.GetUserPath() + "\\Trash");
						if (bin != null)
						{
							bin.OverridesGlobal = overridesGlobal;
							bin.OverridesProject = overridesProject;
							targetBins.Add( bin );
						}
					}
				}

				policy.targets = (ArrayList)targetBins.Clone();
			}
			else if (scope == PurgeCommandScope.USER)
			{
				MOG_Project proj = MOG_ControllerProject.GetProject();
				if (proj != null)
				{
					UserTrashBin bin = this.trashCollector.GetTrashBin( Path.GetDirectoryName(Path.GetFullPath(configFilename)) + "\\Trash");
					policy.targets.Add( bin.TrashPath );
					targetBins.Add(bin);
				}
			}

			
			// get commands

			// Interval
			if ( ini.KeyExist("TRASH", "Interval")  &&  ini.SectionExist("INTERVAL") )
			{
				// parse an Interval command
				policy.interval = true;

				// get params
				DateTime lastExecuted = new DateTime(1776, 6, 4);		// assume has never been executed
				int dayInterval = 7;		// assume every week
				int hourInterval = 1;
				int minInterval = 5;

				if (ini.KeyExist("INTERVAL", "LastExecuted"))
					lastExecuted = DateTime.Parse( ini.GetString("INTERVAL", "LastExecuted") );
				if (ini.KeyExist("INTERVAL", "DayInterval"))
					dayInterval = ini.GetValue("INTERVAL", "DayInterval");
				if (ini.KeyExist("INTERVAL", "HourInterval"))
					hourInterval = ini.GetValue("INTERVAL", "HourInterval");
				if (ini.KeyExist("INTERVAL", "MinInterval"))
					minInterval = ini.GetValue("INTERVAL", "MinInterval");

				policy.dayInterval = dayInterval;
				policy.hourInterval = hourInterval;
				policy.minInterval = minInterval;

				PurgeCommand intervalCmd = PurgeCommand.ConstructIntervalCommand(scope, targetBins, dayInterval, hourInterval, minInterval);
				intervalCmd.LastExecuted = lastExecuted;
				intervalCmd.ConfigFilename = configFilename;
				this.trashCollector.AddCommand(intervalCmd);
			}

			// Asset Age
			if ( ini.KeyExist("TRASH", "Age")  &&  ini.SectionExist("AGE") )
			{
				// parse an Interval command
				policy.age = true;

				// get params
				DateTime lastExecuted = new DateTime(1776, 6, 4);		// assume has never been executed
				int dayInterval = 7;		// assume week
				int hourInterval = 0;
				int minInterval = 0;

				if (ini.KeyExist("AGE", "LastExecuted"))
					lastExecuted = DateTime.Parse( ini.GetString("AGE", "LastExecuted") );
				if (ini.KeyExist("AGE", "DayInterval"))
					dayInterval = ini.GetValue("AGE", "DayInterval");
				if (ini.KeyExist("AGE", "HourInterval"))
					hourInterval = ini.GetValue("AGE", "HourInterval");
				if (ini.KeyExist("AGE", "MinInterval"))
					minInterval = ini.GetValue("AGE", "MinInterval");

				policy.ageDayInterval = dayInterval;
				policy.ageHourInterval = hourInterval;
				policy.ageMinInterval = minInterval;

				PurgeCommand intervalCmd = PurgeCommand.ConstructAgeCommand(scope, targetBins, dayInterval, hourInterval, minInterval);
				intervalCmd.ConfigFilename = configFilename;
				intervalCmd.LastExecuted = lastExecuted;
				this.trashCollector.AddCommand(intervalCmd);
			}

			// Megs
			if ( ini.KeyExist("TRASH", "Megs")  &&  ini.SectionExist("MEGS") )
			{
				// parse an Interval command
				policy.megs = true;

				// get params
				DateTime lastExecuted = new DateTime(1776, 6, 4);		// assume has never been executed
				int megLimit = 20;		// assume 20 megs

				if (ini.KeyExist("MEGS", "LastExecuted"))
					lastExecuted = DateTime.Parse( ini.GetString("MEGS", "LastExecuted") );
				if (ini.KeyExist("MEGS", "MegLimit"))
					megLimit = ini.GetValue("MEGS", "MegLimit");

				policy.megLimit = megLimit;

				PurgeCommand intervalCmd = PurgeCommand.ConstructMegsCommand(scope, targetBins, megLimit);
				intervalCmd.LastExecuted = lastExecuted;
				intervalCmd.ConfigFilename = configFilename;
				this.trashCollector.AddCommand(intervalCmd);
			}

			ini.Close();
			
			return policy;
		}


//		private PurgePolicy LoadPolicy(string configFilename)
//		{
//			if (!File.Exists(configFilename))
//				return null;
//
//			MOG_Ini ini = new MOG_Ini(configFilename);
//			if (ini == null)
//				return null;
//
//			// check for neccessary sections
//			if (!ini.SectionExist("TRASH"))
//				return null;
//			if (!ini.SectionExist("TARGETS"))
//				return null;
//
//			PurgePolicy policy = new PurgePolicy();
//			policy.filename = configFilename;
//
//			// JKB TODO : Put last run code here
//
//			// get scope
//			PurgeCommandScope scope = PurgeCommandScope.NONE;
//			if (ini.KeyExist("TRASH", "Scope"))
//			{
//				string scopeString = ini.GetString("TRASH", "Scope");
//				if (scopeString.ToLower() == "global")
//					scope = PurgeCommandScope.GLOBAL;
//				else if (scopeString.ToLower() == "project")
//					scope = PurgeCommandScope.PROJECT;
//				else if (scopeString.ToLower() == "user")
//					scope = PurgeCommandScope.USER;
//			}
//			policy.scope = scope;
//
//			// get override policy
//			bool overridesGlobal = false;		// default to false if key doesn't exist
//			bool overridesProject = false;
//			if (ini.KeyExist("TRASH", "OverridesGlobal"))
//				overridesGlobal = bool.Parse( ini.GetString("TRASH", "OverridesGlobal") );
//			if (ini.KeyExist("TRASH", "OverridesProject"))
//				overridesProject = bool.Parse( ini.GetString("TRASH", "OverridesProject") );
//			policy.overridesGlobal = overridesGlobal;
//			policy.overridesProject = overridesProject;
//
//
//			// get target trash folders
//			ArrayList targetBins = new ArrayList();
//			for (int targetIndex=0; targetIndex<ini.CountKeys("TARGETS"); targetIndex++)
//			{
//				UserTrashBin bin = this.trashCollector.GetTrashBin(ini.GetKeyNameByIndexSLOW("TARGETS", targetIndex));
//				if (bin != null)
//				{
//					bin.OverridesGlobal = overridesGlobal;
//					bin.OverridesProject = overridesProject;
//					targetBins.Add( bin );
//				}
//			}
//			policy.targets = (ArrayList)targetBins.Clone();
//
//			
//			// get commands
//
//			// Interval
//			if ( ini.KeyExist("TRASH", "Interval")  &&  ini.SectionExist("INTERVAL") )
//			{
//				// parse an Interval command
//				policy.interval = true;
//
//				// get params
//				DateTime lastExecuted = new DateTime(1776, 6, 4);		// assume has never been executed
//				int dayInterval = 7;		// assume every week
//				int hourInterval = 1;
//				int minInterval = 5;
//
//				if (ini.KeyExist("INTERVAL", "LastExecuted"))
//					lastExecuted = DateTime.Parse( ini.GetString("INTERVAL", "LastExecuted") );
//				if (ini.KeyExist("INTERVAL", "DayInterval"))
//					dayInterval = ini.GetValue("INTERVAL", "DayInterval");
//				if (ini.KeyExist("INTERVAL", "HourInterval"))
//					hourInterval = ini.GetValue("INTERVAL", "HourInterval");
//				if (ini.KeyExist("INTERVAL", "MinInterval"))
//					minInterval = ini.GetValue("INTERVAL", "MinInterval");
//
//				policy.dayInterval = dayInterval;
//				policy.hourInterval = hourInterval;
//				policy.minInterval = minInterval;
//
//				PurgeCommand intervalCmd = PurgeCommand.ConstructIntervalCommand(scope, targetBins, dayInterval, hourInterval, minInterval);
//				intervalCmd.LastExecuted = lastExecuted;
//				intervalCmd.ConfigFilename = configFilename;
//				this.trashCollector.AddCommand(intervalCmd);
//			}
//
//			// Asset Age
//			if ( ini.KeyExist("TRASH", "Age")  &&  ini.SectionExist("AGE") )
//			{
//				// parse an Interval command
//				policy.age = true;
//
//				// get params
//				DateTime lastExecuted = new DateTime(1776, 6, 4);		// assume has never been executed
//				int dayInterval = 7;		// assume week
//				int hourInterval = 0;
//				int minInterval = 0;
//
//				if (ini.KeyExist("AGE", "LastExecuted"))
//					lastExecuted = DateTime.Parse( ini.GetString("AGE", "LastExecuted") );
//				if (ini.KeyExist("AGE", "DayInterval"))
//					dayInterval = ini.GetValue("AGE", "DayInterval");
//				if (ini.KeyExist("AGE", "HourInterval"))
//					hourInterval = ini.GetValue("AGE", "HourInterval");
//				if (ini.KeyExist("AGE", "MinInterval"))
//					minInterval = ini.GetValue("AGE", "MinInterval");
//
//				policy.ageDayInterval = dayInterval;
//				policy.ageHourInterval = hourInterval;
//				policy.ageMinInterval = minInterval;
//
//				PurgeCommand intervalCmd = PurgeCommand.ConstructAgeCommand(scope, targetBins, dayInterval, hourInterval, minInterval);
//				intervalCmd.ConfigFilename = configFilename;
//				intervalCmd.LastExecuted = lastExecuted;
//				this.trashCollector.AddCommand(intervalCmd);
//			}
//
//			// Megs
//			if ( ini.KeyExist("TRASH", "Megs")  &&  ini.SectionExist("MEGS") )
//			{
//				// parse an Interval command
//				policy.megs = true;
//
//				// get params
//				DateTime lastExecuted = new DateTime(1776, 6, 4);		// assume has never been executed
//				int megLimit = 20;		// assume 20 megs
//
//				if (ini.KeyExist("MEGS", "LastExecuted"))
//					lastExecuted = DateTime.Parse( ini.GetString("MEGS", "LastExecuted") );
//				if (ini.KeyExist("MEGS", "MegLimit"))
//					megLimit = ini.GetValue("MEGS", "MegLimit");
//
//				policy.megLimit = megLimit;
//
//				PurgeCommand intervalCmd = PurgeCommand.ConstructMegsCommand(scope, targetBins, megLimit);
//				intervalCmd.LastExecuted = lastExecuted;
//				intervalCmd.ConfigFilename = configFilename;
//				this.trashCollector.AddCommand(intervalCmd);
//			}
//
//			ini.Close();
//			
//			return policy;
//		}
	
		#endregion
		#region Event handlers

		public void btnPurge_Click(object sender, System.EventArgs e)
		{
			this.trashCollector.ProcessCommands();
		}

		private void btnReset_Click(object sender, System.EventArgs e)
		{
			this.trashCollector.ResetCommands();
		}

		private void btnSave_Click(object sender, System.EventArgs e)
		{
			this.trashCollector.SaveCommands();
		}

		private void chbxInterval_CheckedChanged(object sender, System.EventArgs e)
		{
			this.lblDays.Enabled = this.chbxInterval.Checked;
			this.lblHours.Enabled = this.chbxInterval.Checked;
			this.lblMinutes.Enabled = this.chbxInterval.Checked;
			this.nudDays.Enabled = this.chbxInterval.Checked;
			this.nudHours.Enabled = this.chbxInterval.Checked;
			this.nudMinutes.Enabled = this.chbxInterval.Checked;
		}

		private void btnSavePolicy_Click(object sender, System.EventArgs e)
		{
			if (this.treeView.SelectedNode != null  &&  this.treeView.SelectedNode.Tag is PurgePolicy)
			{
				SavePolicyFromGUI( (PurgePolicy)this.treeView.SelectedNode.Tag );
			}
		}

		private void treeView_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			// if we're autosaving, save the old selected node, if any
			TreeNode oldSelectedNode = this.treeView.SelectedNode;
			if (this.chbxAutoSave.Checked  &&  oldSelectedNode != null  &&  oldSelectedNode.Tag is PurgePolicy)
			{
				SavePolicyFromGUI( (PurgePolicy)oldSelectedNode.Tag );
			}

			this.treeView.SelectedNode = this.treeView.GetNodeAt( new Point(e.X, e.Y) );
			if (this.treeView.SelectedNode == oldSelectedNode)
				return;		// don't reload if its the same node

			if (this.treeView.SelectedNode != null  &&  this.treeView.SelectedNode.Tag is PurgePolicy)
			{
				LoadPolicyToGUI( this.treeView.SelectedNode );
			}
			else
			{
				this.gbConfig.Enabled = false;
				this.gbConfig.Text = "Policy Configuration";
			}
		}

		private void btnDeletePolicy_Click(object sender, System.EventArgs e)
		{
			if (this.treeView.SelectedNode != null  &&  this.treeView.SelectedNode.Tag is PurgePolicy)
			{
				string filename = ((PurgePolicy)this.treeView.SelectedNode.Tag).filename;
				
				try
				{
					if (File.Exists(filename))
						File.Delete(filename);
				}
				catch (Exception excp)
				{
					MessageBox.Show("btnDeletePolicy_Click(): Couldn't delete " + filename + "\n\n" + excp.Message + "\n" + excp.StackTrace);
					return;
				}

				RefreshTreeViewFromDisk();
			}
		}

		private void btnBrowseRepository_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.FileName = this.loadedRepository;
			ofd.Title = "Select MOG Repository INI File";
			
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				this.tbRepositoryLocation.Text = ofd.FileName;

				this.treeView.Nodes.Clear();
				this.nodes.Clear();
				this.trashCollector.SaveCommands();
				this.trashCollector.ClearCommands();
				LoadMOGRepository(ofd.FileName);
			}
		}

		private void btnRefreshTree_Click(object sender, System.EventArgs e)
		{
			this.RefreshTreeViewFromDisk();
		}

		private void btnViewCommands_Click(object sender, System.EventArgs e)
		{
			this.trashCollector.DisplayCommands();
		}

		private void btnTargets_Click(object sender, System.EventArgs e)
		{
			if (this.treeView.SelectedNode != null  &&  this.treeView.SelectedNode.Tag is PurgePolicy)
			{
				string msg = "";
				foreach (UserTrashBin utb in ((PurgePolicy)this.treeView.SelectedNode.Tag).targets)
					msg += utb.ToString() + "\n";

				MessageBox.Show(msg, this.treeView.SelectedNode.Text + " Targets");
			}
		}

		private void btnForcePurge_Click(object sender, System.EventArgs e)
		{
			if (this.treeView.SelectedNode != null  &&  this.treeView.SelectedNode.Tag is PurgePolicy)
			{
				PurgePolicy p = (PurgePolicy)this.treeView.SelectedNode.Tag;
				if (p.targets == null)
					return;

				PurgeCommand cmd = new PurgeCommand();
				cmd.Type = PurgeCommandType.FORCE;
				cmd.ConfigFilename = "Forced by Administrator";
				cmd.Scope = PurgeCommandScope.NONE;
				for (int x = 0; x < p.targets.Count; x++)
				{
					UserTrashBin utb = null;
					if (p.targets[x] is string)
					{
						utb = this.trashCollector.GetTrashBin(p.targets[x] as string);
					}
					else if (p.targets[x] is UserTrashBin)
					{
						utb = p.targets[x] as UserTrashBin;
					}
				
					if (this.logActions)
						Utils.AppendLineToFile(this.logFilename, DateTime.Now.ToString() + ":\tTrashPolicyEditorControl: Forcing purge of " + utb.TrashPath);
					
					utb.ExecutePurge(cmd);
					this.trashCollector.SaveCommands();
				}
			}
		}

		private void purgeTimer_Tick(object sender, EventArgs e)
		{
			this.trashCollector.ProcessCommands();
		}

		private void chbxPurgeTimerEnabled_CheckedChanged(object sender, System.EventArgs e)
		{
			this.purgeTimer.Enabled = this.chbxPurgeTimerEnabled.Checked;

			this.btnProgramPurgeTimer.Enabled = this.chbxPurgeTimerEnabled.Checked;
			this.tbPurgeTimerInterval.Enabled = this.chbxPurgeTimerEnabled.Checked;
			this.lblPurgeTimerSeconds.Enabled = this.chbxPurgeTimerEnabled.Checked;
		}

		private void btnProgramPurgeTimer_Click(object sender, System.EventArgs e)
		{
			try 
			{
				int seconds = int.Parse(this.tbPurgeTimerInterval.Text) * 1000;	// 1000 milliseconds to the second (timer is programmed in ms)
				if (seconds <= 0)
				{
					MessageBox.Show("Please enter a positive numeric value");
					this.tbPurgeTimerInterval.Text = (this.purgeTimer.Interval / 1000).ToString();
					this.tbPurgeTimerInterval.Focus();
					this.tbPurgeTimerInterval.SelectAll();
					return;
				}
			
				this.purgeTimer.Interval = seconds;
			}
			catch (System.FormatException)
			{
				MessageBox.Show("Please enter a positive numeric value");
				this.tbPurgeTimerInterval.Text = (this.purgeTimer.Interval / 1000).ToString();
				this.tbPurgeTimerInterval.Focus();
				this.tbPurgeTimerInterval.SelectAll();
				return;
			}
		}

		private void chbxMegs_CheckedChanged(object sender, System.EventArgs e)
		{
			this.nudMegs.Enabled = this.chbxMegs.Checked;
			this.lblMegs.Enabled = this.chbxMegs.Checked;
		}

		private void chbxAssetAge_CheckedChanged(object sender, System.EventArgs e)
		{
			this.nudAssetAgeDays.Enabled = this.chbxAssetAge.Checked;
			this.nudAssetAgeHours.Enabled = this.chbxAssetAge.Checked;
			this.nudAssetAgeMinutes.Enabled = this.chbxAssetAge.Checked;

			this.lblAssetAgeDays.Enabled = this.chbxAssetAge.Checked;
			this.lblAssetAgeHours.Enabled = this.chbxAssetAge.Checked;
			this.lblAssetAgeMinutes.Enabled = this.chbxAssetAge.Checked;
		}

		private void cmTrashCollectionTree_Popup(object sender, System.EventArgs e)
		{
			if (this.treeView.SelectedNode == null)
			{
				this.miSavePolicy.Enabled = false;
				this.miDeletePolicy.Enabled = false;
				this.miBrowseToTrashBin.Enabled = false;
				this.miForcePurge.Enabled = false;
				this.miEditConfigFile.Enabled = false;
				this.miCreateTrashBin.Enabled = false;
			}
			else
			{
				this.miSavePolicy.Enabled = true;
				this.miDeletePolicy.Enabled = true;
				this.miForcePurge.Enabled = true;
				this.miCreateTrashBin.Enabled = true;

				// if this is a user policy node, enable browse to trash bin
				if (this.treeView.SelectedNode.Nodes.Count == 0)
					this.miBrowseToTrashBin.Enabled = true;
				else
					this.miBrowseToTrashBin.Enabled = false;

				// if there is a config file, allow user to edit it
				if (this.treeView.SelectedNode.Tag is PurgePolicy)
					this.miEditConfigFile.Enabled = File.Exists( ((PurgePolicy)this.treeView.SelectedNode.Tag).filename );
				else
					this.miEditConfigFile.Enabled = false;
			}
		}
		
		private void miSavePolicy_Click(object sender, System.EventArgs e)
		{
			this.btnSavePolicy.PerformClick();
		}

		private void miDeletePolicy_Click(object sender, System.EventArgs e)
		{
			this.btnDeletePolicy.PerformClick();
		}

		private void miBrowseToTrashBin_Click(object sender, System.EventArgs e)
		{
			if (this.treeView.SelectedNode != null  &&  this.treeView.SelectedNode.Tag is PurgePolicy)
			{
				PurgePolicy p = (PurgePolicy)this.treeView.SelectedNode.Tag;

				if (p.targets != null  &&  p.targets.Count > 0  &&  p.targets[0] is UserTrashBin)
				{
					string path = ((UserTrashBin)p.targets[0]).TrashPath;
					if (Directory.Exists(path))
						Utils.RunCommand( "explorer", path );
					else
						Utils.ShowMessageBoxExclamation(path + " does not exist", "Missing Trash Bin");
				}
			}
		}

		private void miRefresh_Click(object sender, System.EventArgs e)
		{
			this.btnRefreshTree.PerformClick();
		}

		private void miForcePurge_Click(object sender, System.EventArgs e)
		{
			this.btnForcePurge.PerformClick();
		}

		private void chbxAutoSave_CheckedChanged(object sender, System.EventArgs e)
		{
			this.btnSavePolicy.Enabled = !this.chbxAutoSave.Checked;
			this.btnDeletePolicy.Enabled = !this.chbxAutoSave.Checked;
		}

		private void miEditConfigFile_Click(object sender, System.EventArgs e)
		{
			if (this.treeView.SelectedNode != null  &&  this.treeView.SelectedNode.Tag is PurgePolicy)
			{
				PurgePolicy p = (PurgePolicy)this.treeView.SelectedNode.Tag;
				if (File.Exists(p.filename))
					Utils.RunCommand("notepad", p.filename);
				else
					Utils.ShowMessageBoxExclamation(p.filename + " does not exist", "Missing Configuration File");
			}
		}

		private void miViewLogfile_Click(object sender, System.EventArgs e)
		{
			if (File.Exists(this.logFilename))
				Utils.RunCommand("notepad", this.logFilename);
			else
				Utils.ShowMessageBoxExclamation("Specified log file (\"" + this.logFilename + "\") does not exist", "Missing Log File");
		}

		private void miClearLogfile_Click(object sender, System.EventArgs e)
		{
			if (File.Exists(this.logFilename))
			{
				try 
				{
					File.Delete(this.logFilename);
					Utils.AppendLineToFile(this.logFilename, "");
				}
				catch {}
			}
		}

		private void miCreateTrashBin_Click(object sender, System.EventArgs e)
		{
			if (this.treeView.SelectedNode != null  &&  this.treeView.SelectedNode.Tag is PurgePolicy)
			{
				PurgePolicy p = (PurgePolicy)this.treeView.SelectedNode.Tag;

				if (p.targets != null  &&  p.targets.Count > 0  &&  p.targets[0] is UserTrashBin)
				{
					string path = ((UserTrashBin)p.targets[0]).TrashPath;
					try
					{
						if (!Directory.Exists(path))
							Directory.CreateDirectory(path);
					}
					catch (Exception excp)
					{
						MOG_Prompt.PromptResponse("Exception", "miCreateTrashBin_Click(): Couldn't create " + path + " due to an exception:\n\n" + excp.Message, excp.StackTrace, MOGPromptButtons.OK, MOG_ALERT_LEVEL.CRITICAL);
						return;
					}
				}
			}
		}

		private void treeView_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (this.treeView.SelectedNode != null)
			{
				if (e.KeyData == Keys.Delete)
					this.btnDeletePolicy.PerformClick();
				else if ( (e.KeyData & Keys.S) > 0  &&  (e.KeyData & Keys.Control) > 0 )
					this.btnSavePolicy.PerformClick();
			}
		}
		#endregion
	}

	#region Other classes
	class PurgePolicy : System.ICloneable
	{
		public bool empty = false;

		public bool overridesGlobal;
		public bool overridesProject;

		public PurgeCommandScope scope;

		// Interval
		public bool interval;
		public int dayInterval;
		public int hourInterval;
		public int minInterval;

		// Asset Age
		public bool age;
		public int ageDayInterval;
		public int ageHourInterval;
		public int ageMinInterval;

		// Megs
		public bool megs;
		public int megLimit;

		public ArrayList targets = new ArrayList();

		public string filename;

		public DateTime lastRun = new DateTime(1776, 7, 4);

		public ArrayList GetTargetDirs()
		{
			ArrayList dirs = new ArrayList();
			if (this.targets == null)
			{
				return dirs;
			}
            
			foreach (UserTrashBin bin in this.targets)
			{
				dirs.Add(bin.TrashPath);
			}

			return dirs;
		}

		public PurgePolicy()
		{
		}

		// ------------------------------------------------------------------------------------------------------
		//  This constructor is for creating "empty" PurgePolicies (i.e., to allow a policy to "exist" even when
		//  it has no trash policy config file)
		public PurgePolicy(bool empty, string filename)
		{
			this.empty = empty;
			this.filename = filename;
		}

		#region ICloneable Members

		public object Clone()
		{
			PurgePolicy p = new PurgePolicy();
			
			p.overridesGlobal = this.overridesGlobal;
			p.overridesProject = this.overridesProject;
			p.scope = this.scope;
			p.interval = this.interval;
			p.dayInterval = this.dayInterval;
			p.hourInterval = this.hourInterval;
			p.minInterval = this.minInterval;
			p.targets = (ArrayList)this.targets.Clone();
			p.filename = this.filename;

			p.megs = this.megs;
			p.megLimit = this.megLimit;

			p.age = this.age;
			p.ageDayInterval = this.ageDayInterval;
			p.ageHourInterval = this.ageHourInterval;
			p.ageMinInterval = this.ageMinInterval;

			return p;
		}

		#endregion
	}
	#endregion
}
