using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

using MOG;
using MOG.FILENAME;
using MOG.PROJECT;
using MOG.PROPERTIES;
using MOG.PLATFORM;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.DOSUTILS;
using MOG.CONTROLLER.CONTROLLERINBOX;
using MOG.REPORT;
using MOG.PROMPT;
using MOG.PROGRESS;

using MOG_Client.Forms;
using MOG_Client.Client_Mog_Utilities;

using MOG_ControlsLibrary.Utils;
using MOG_CoreControls;
using MOG.CONTROLLER.CONTROLLERASSET;

namespace MOG_Client
{
	/// <summary>
	/// Summary description for RenameAssetForm.
	/// </summary>
	public class RenameAssetForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox RenameNewLabelTextBox;
		private System.Windows.Forms.TextBox RenameOldLabelTextBox;
		private System.Windows.Forms.Button RenameCancelButton;
		private System.Windows.Forms.Button RenameOkButton;

		private string mFullFilename;
		private ArrayList mSourceFiles;
		public bool bRenameSuccessful;
		private bool bInitialized = false;

		private string importFilename;

		public string mCommonClass;
		public string mCommonPlatform;
		public string mCommonLabel;

		public MOG_Filename mNewFilename;
		public MOG_Filename mNewMultiFilename;

		private System.Windows.Forms.Label RenameOldNameLabel;
		private System.Windows.Forms.Label RenameNewNameLabel;
		private System.Windows.Forms.Label RenameLabelLabel;
		private System.Windows.Forms.Label RenamePlatformLabel;
		private System.Windows.Forms.TextBox RenameOldPlatformTextBox;
		private System.Windows.Forms.ComboBox RenameNewPlatformComboBox;
		private System.Windows.Forms.TextBox RenameOldClassNameTextBox;
		private System.Windows.Forms.TextBox RenameNewClassNameTextBox;
		private System.Windows.Forms.Button RenameBrowseClassTreeButton;
		private System.Windows.Forms.ToolTip RenameToolTip;
		private System.Windows.Forms.Panel BottomPanel;
		private System.Windows.Forms.Panel MiddlePanel;
		private System.Windows.Forms.Panel TopPanel;
		private System.Windows.Forms.Panel NameLeftPanel;
		private System.Windows.Forms.Panel NameRightPanel;
		private System.Windows.Forms.Splitter MiddlePanelSplitter1;
		private System.Windows.Forms.ListView RenameListView;
		private System.Windows.Forms.ColumnHeader OldNameColumn;
		private System.Windows.Forms.ColumnHeader NewNameColumn;
		private System.Windows.Forms.ColumnHeader FullFilenameColumn;
		private System.Windows.Forms.Button ApplyButton;
		private System.Windows.Forms.TextBox RenameNewLabelWarningTextBox;
		private System.Windows.Forms.ContextMenuStrip ListViewContextMenu;
		private System.Windows.Forms.ToolStripMenuItem ClassificationMenuItem;
		private System.Windows.Forms.CheckBox RenameFiles;
		private ColumnHeader ImportedFileColumn;
		private System.ComponentModel.IContainer components;

		public RenameAssetForm(MOG_Filename source)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			this.mSourceFiles = new ArrayList(1);
			this.mSourceFiles.Add( source.GetEncodedFilename() );

			mFullFilename = source.GetNotSureDefaultEncodedFilename();
            InitializeAssetName(source);
		}

		public RenameAssetForm(ArrayList sourceFiles)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			mSourceFiles = sourceFiles;
            
			InitializeAssetNames(sourceFiles);
		}

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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RenameAssetForm));
			this.RenameCancelButton = new System.Windows.Forms.Button();
			this.RenameOkButton = new System.Windows.Forms.Button();
			this.RenameNewLabelTextBox = new System.Windows.Forms.TextBox();
			this.RenameOldClassNameTextBox = new System.Windows.Forms.TextBox();
			this.RenameOldNameLabel = new System.Windows.Forms.Label();
			this.RenameNewNameLabel = new System.Windows.Forms.Label();
			this.RenameOldLabelTextBox = new System.Windows.Forms.TextBox();
			this.RenameLabelLabel = new System.Windows.Forms.Label();
			this.RenamePlatformLabel = new System.Windows.Forms.Label();
			this.RenameOldPlatformTextBox = new System.Windows.Forms.TextBox();
			this.RenameNewPlatformComboBox = new System.Windows.Forms.ComboBox();
			this.RenameNewClassNameTextBox = new System.Windows.Forms.TextBox();
			this.RenameBrowseClassTreeButton = new System.Windows.Forms.Button();
			this.RenameToolTip = new System.Windows.Forms.ToolTip(this.components);
			this.MiddlePanelSplitter1 = new System.Windows.Forms.Splitter();
			this.RenameNewLabelWarningTextBox = new System.Windows.Forms.TextBox();
			this.RenameFiles = new System.Windows.Forms.CheckBox();
			this.BottomPanel = new System.Windows.Forms.Panel();
			this.ApplyButton = new System.Windows.Forms.Button();
			this.MiddlePanel = new System.Windows.Forms.Panel();
			this.NameLeftPanel = new System.Windows.Forms.Panel();
			this.NameRightPanel = new System.Windows.Forms.Panel();
			this.TopPanel = new System.Windows.Forms.Panel();
			this.RenameListView = new System.Windows.Forms.ListView();
			this.OldNameColumn = new System.Windows.Forms.ColumnHeader();
			this.NewNameColumn = new System.Windows.Forms.ColumnHeader();
			this.FullFilenameColumn = new System.Windows.Forms.ColumnHeader();
			this.ImportedFileColumn = new System.Windows.Forms.ColumnHeader();
			this.ListViewContextMenu = new System.Windows.Forms.ContextMenuStrip();
			this.ClassificationMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.BottomPanel.SuspendLayout();
			this.MiddlePanel.SuspendLayout();
			this.NameLeftPanel.SuspendLayout();
			this.NameRightPanel.SuspendLayout();
			this.TopPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// RenameCancelButton
			// 
			this.RenameCancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.RenameCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.RenameCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.RenameCancelButton.Location = new System.Drawing.Point(533, 7);
			this.RenameCancelButton.Name = "RenameCancelButton";
			this.RenameCancelButton.Size = new System.Drawing.Size(75, 24);
			this.RenameCancelButton.TabIndex = 7;
			this.RenameCancelButton.Text = "Cancel";
			// 
			// RenameOkButton
			// 
			this.RenameOkButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.RenameOkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.RenameOkButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.RenameOkButton.Location = new System.Drawing.Point(453, 7);
			this.RenameOkButton.Name = "RenameOkButton";
			this.RenameOkButton.Size = new System.Drawing.Size(75, 24);
			this.RenameOkButton.TabIndex = 6;
			this.RenameOkButton.Text = "OK";
			this.RenameOkButton.Click += new System.EventHandler(this.RenameOkButton_Click);
			// 
			// RenameNewLabelTextBox
			// 
			this.RenameNewLabelTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.RenameNewLabelTextBox.Location = new System.Drawing.Point(104, 48);
			this.RenameNewLabelTextBox.Name = "RenameNewLabelTextBox";
			this.RenameNewLabelTextBox.Size = new System.Drawing.Size(256, 20);
			this.RenameNewLabelTextBox.TabIndex = 15;
			this.RenameNewLabelTextBox.Text = "*";
			this.RenameToolTip.SetToolTip(this.RenameNewLabelTextBox, "New Label for Asset (does not apply to more than one Asset)");
			this.RenameNewLabelTextBox.TextChanged += new System.EventHandler(this.RenameNewLabelTextBox_TextChanged);
			// 
			// RenameOldClassNameTextBox
			// 
			this.RenameOldClassNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.RenameOldClassNameTextBox.Location = new System.Drawing.Point(8, 8);
			this.RenameOldClassNameTextBox.Name = "RenameOldClassNameTextBox";
			this.RenameOldClassNameTextBox.ReadOnly = true;
			this.RenameOldClassNameTextBox.Size = new System.Drawing.Size(711, 20);
			this.RenameOldClassNameTextBox.TabIndex = 20;
			this.RenameOldClassNameTextBox.TabStop = false;
			this.RenameOldClassNameTextBox.Text = "(Class Name)";
			this.RenameToolTip.SetToolTip(this.RenameOldClassNameTextBox, "Old Class Name for Asset");
			// 
			// RenameOldNameLabel
			// 
			this.RenameOldNameLabel.Location = new System.Drawing.Point(8, 8);
			this.RenameOldNameLabel.Name = "RenameOldNameLabel";
			this.RenameOldNameLabel.Size = new System.Drawing.Size(96, 16);
			this.RenameOldNameLabel.TabIndex = 21;
			this.RenameOldNameLabel.Text = "Old Name(s):";
			// 
			// RenameNewNameLabel
			// 
			this.RenameNewNameLabel.Location = new System.Drawing.Point(8, 32);
			this.RenameNewNameLabel.Name = "RenameNewNameLabel";
			this.RenameNewNameLabel.Size = new System.Drawing.Size(96, 16);
			this.RenameNewNameLabel.TabIndex = 22;
			this.RenameNewNameLabel.Text = "New Class Name";
			// 
			// RenameOldLabelTextBox
			// 
			this.RenameOldLabelTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.RenameOldLabelTextBox.Location = new System.Drawing.Point(104, 48);
			this.RenameOldLabelTextBox.Name = "RenameOldLabelTextBox";
			this.RenameOldLabelTextBox.ReadOnly = true;
			this.RenameOldLabelTextBox.Size = new System.Drawing.Size(256, 20);
			this.RenameOldLabelTextBox.TabIndex = 5;
			this.RenameOldLabelTextBox.Text = "(Asset Name)";
			this.RenameToolTip.SetToolTip(this.RenameOldLabelTextBox, "Old Label for Asset");
			// 
			// RenameLabelLabel
			// 
			this.RenameLabelLabel.Location = new System.Drawing.Point(104, 32);
			this.RenameLabelLabel.Name = "RenameLabelLabel";
			this.RenameLabelLabel.Size = new System.Drawing.Size(80, 16);
			this.RenameLabelLabel.TabIndex = 33;
			this.RenameLabelLabel.Text = "Asset Name";
			// 
			// RenamePlatformLabel
			// 
			this.RenamePlatformLabel.Location = new System.Drawing.Point(0, 32);
			this.RenamePlatformLabel.Name = "RenamePlatformLabel";
			this.RenamePlatformLabel.Size = new System.Drawing.Size(88, 16);
			this.RenamePlatformLabel.TabIndex = 34;
			this.RenamePlatformLabel.Text = "Platform";
			// 
			// RenameOldPlatformTextBox
			// 
			this.RenameOldPlatformTextBox.Location = new System.Drawing.Point(0, 8);
			this.RenameOldPlatformTextBox.Name = "RenameOldPlatformTextBox";
			this.RenameOldPlatformTextBox.ReadOnly = true;
			this.RenameOldPlatformTextBox.Size = new System.Drawing.Size(96, 20);
			this.RenameOldPlatformTextBox.TabIndex = 27;
			this.RenameOldPlatformTextBox.TabStop = false;
			this.RenameOldPlatformTextBox.Text = "(Platform)";
			this.RenameToolTip.SetToolTip(this.RenameOldPlatformTextBox, "Old Platform for Asset");
			// 
			// RenameNewPlatformComboBox
			// 
			this.RenameNewPlatformComboBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.RenameNewPlatformComboBox.ItemHeight = 13;
			this.RenameNewPlatformComboBox.Location = new System.Drawing.Point(0, 48);
			this.RenameNewPlatformComboBox.Name = "RenameNewPlatformComboBox";
			this.RenameNewPlatformComboBox.Size = new System.Drawing.Size(96, 21);
			this.RenameNewPlatformComboBox.TabIndex = 4;
			this.RenameNewPlatformComboBox.Text = "All";
			this.RenameToolTip.SetToolTip(this.RenameNewPlatformComboBox, "New Platform for Asset(s)");
			this.RenameNewPlatformComboBox.SelectedIndexChanged += new System.EventHandler(this.RenameNewPlatformComboBox_SelectedIndexChanged);
			this.RenameNewPlatformComboBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.RenameNewPlatformComboBox_KeyDown);
			// 
			// RenameNewClassNameTextBox
			// 
			this.RenameNewClassNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.RenameNewClassNameTextBox.BackColor = System.Drawing.SystemColors.Window;
			this.RenameNewClassNameTextBox.Location = new System.Drawing.Point(8, 48);
			this.RenameNewClassNameTextBox.Name = "RenameNewClassNameTextBox";
			this.RenameNewClassNameTextBox.Size = new System.Drawing.Size(679, 20);
			this.RenameNewClassNameTextBox.TabIndex = 3;
			this.RenameNewClassNameTextBox.Text = "Class Name";
			this.RenameToolTip.SetToolTip(this.RenameNewClassNameTextBox, "New Class Name for Asset(s)");
			this.RenameNewClassNameTextBox.TextChanged += new System.EventHandler(this.RenameNewClassNameTextBox_TextChanged);
			// 
			// RenameBrowseClassTreeButton
			// 
			this.RenameBrowseClassTreeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.RenameBrowseClassTreeButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.RenameBrowseClassTreeButton.Location = new System.Drawing.Point(695, 48);
			this.RenameBrowseClassTreeButton.Name = "RenameBrowseClassTreeButton";
			this.RenameBrowseClassTreeButton.Size = new System.Drawing.Size(24, 20);
			this.RenameBrowseClassTreeButton.TabIndex = 36;
			this.RenameBrowseClassTreeButton.Text = "...";
			this.RenameToolTip.SetToolTip(this.RenameBrowseClassTreeButton, "Browse Class Tree for Class Name");
			this.RenameBrowseClassTreeButton.Click += new System.EventHandler(this.RenameBrowseClassTreeButton_Click);
			// 
			// MiddlePanelSplitter1
			// 
			this.MiddlePanelSplitter1.BackColor = System.Drawing.SystemColors.Control;
			this.MiddlePanelSplitter1.Dock = System.Windows.Forms.DockStyle.Right;
			this.MiddlePanelSplitter1.Location = new System.Drawing.Point(719, 0);
			this.MiddlePanelSplitter1.Name = "MiddlePanelSplitter1";
			this.MiddlePanelSplitter1.Size = new System.Drawing.Size(8, 72);
			this.MiddlePanelSplitter1.TabIndex = 39;
			this.MiddlePanelSplitter1.TabStop = false;
			this.RenameToolTip.SetToolTip(this.MiddlePanelSplitter1, "Resize the Label and Class Name fields");
			// 
			// RenameNewLabelWarningTextBox
			// 
			this.RenameNewLabelWarningTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.RenameNewLabelWarningTextBox.Location = new System.Drawing.Point(104, 8);
			this.RenameNewLabelWarningTextBox.Name = "RenameNewLabelWarningTextBox";
			this.RenameNewLabelWarningTextBox.ReadOnly = true;
			this.RenameNewLabelWarningTextBox.Size = new System.Drawing.Size(256, 20);
			this.RenameNewLabelWarningTextBox.TabIndex = 0;
			this.RenameNewLabelWarningTextBox.TabStop = false;
			this.RenameNewLabelWarningTextBox.Text = "Unable to rename multiple Asset Names...";
			this.RenameToolTip.SetToolTip(this.RenameNewLabelWarningTextBox, "New Label for Asset (does not apply to more than one Asset)");
			// 
			// RenameFiles
			// 
			this.RenameFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.RenameFiles.Location = new System.Drawing.Point(777, 0);
			this.RenameFiles.Name = "RenameFiles";
			this.RenameFiles.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.RenameFiles.Size = new System.Drawing.Size(310, 24);
			this.RenameFiles.TabIndex = 0;
			this.RenameFiles.Text = "Rename imported and processed files to match asset name";
			this.RenameToolTip.SetToolTip(this.RenameFiles, "This option will also rename any contained files that exactly match the asset\'s n" +
					"ame.");
			this.RenameFiles.CheckedChanged += new System.EventHandler(this.RenameFiles_CheckedChanged);
			// 
			// BottomPanel
			// 
			this.BottomPanel.Controls.Add(this.RenameCancelButton);
			this.BottomPanel.Controls.Add(this.RenameOkButton);
			this.BottomPanel.Controls.Add(this.ApplyButton);
			this.BottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.BottomPanel.Location = new System.Drawing.Point(0, 252);
			this.BottomPanel.Name = "BottomPanel";
			this.BottomPanel.Size = new System.Drawing.Size(1095, 40);
			this.BottomPanel.TabIndex = 37;
			// 
			// ApplyButton
			// 
			this.ApplyButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.ApplyButton.DialogResult = System.Windows.Forms.DialogResult.Retry;
			this.ApplyButton.Enabled = false;
			this.ApplyButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.ApplyButton.Location = new System.Drawing.Point(617, 7);
			this.ApplyButton.Name = "ApplyButton";
			this.ApplyButton.Size = new System.Drawing.Size(75, 24);
			this.ApplyButton.TabIndex = 8;
			this.ApplyButton.Text = "Apply";
			this.ApplyButton.Visible = false;
			// 
			// MiddlePanel
			// 
			this.MiddlePanel.Controls.Add(this.MiddlePanelSplitter1);
			this.MiddlePanel.Controls.Add(this.NameLeftPanel);
			this.MiddlePanel.Controls.Add(this.NameRightPanel);
			this.MiddlePanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.MiddlePanel.Location = new System.Drawing.Point(0, 180);
			this.MiddlePanel.Name = "MiddlePanel";
			this.MiddlePanel.Size = new System.Drawing.Size(1095, 72);
			this.MiddlePanel.TabIndex = 38;
			// 
			// NameLeftPanel
			// 
			this.NameLeftPanel.Controls.Add(this.RenameBrowseClassTreeButton);
			this.NameLeftPanel.Controls.Add(this.RenameOldClassNameTextBox);
			this.NameLeftPanel.Controls.Add(this.RenameNewNameLabel);
			this.NameLeftPanel.Controls.Add(this.RenameNewClassNameTextBox);
			this.NameLeftPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.NameLeftPanel.Location = new System.Drawing.Point(0, 0);
			this.NameLeftPanel.Name = "NameLeftPanel";
			this.NameLeftPanel.Size = new System.Drawing.Size(727, 72);
			this.NameLeftPanel.TabIndex = 37;
			// 
			// NameRightPanel
			// 
			this.NameRightPanel.Controls.Add(this.RenameNewLabelWarningTextBox);
			this.NameRightPanel.Controls.Add(this.RenameLabelLabel);
			this.NameRightPanel.Controls.Add(this.RenamePlatformLabel);
			this.NameRightPanel.Controls.Add(this.RenameOldPlatformTextBox);
			this.NameRightPanel.Controls.Add(this.RenameNewPlatformComboBox);
			this.NameRightPanel.Controls.Add(this.RenameNewLabelTextBox);
			this.NameRightPanel.Controls.Add(this.RenameOldLabelTextBox);
			this.NameRightPanel.Dock = System.Windows.Forms.DockStyle.Right;
			this.NameRightPanel.Location = new System.Drawing.Point(727, 0);
			this.NameRightPanel.Name = "NameRightPanel";
			this.NameRightPanel.Size = new System.Drawing.Size(368, 72);
			this.NameRightPanel.TabIndex = 38;
			// 
			// TopPanel
			// 
			this.TopPanel.Controls.Add(this.RenameFiles);
			this.TopPanel.Controls.Add(this.RenameListView);
			this.TopPanel.Controls.Add(this.RenameOldNameLabel);
			this.TopPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TopPanel.Location = new System.Drawing.Point(0, 0);
			this.TopPanel.Name = "TopPanel";
			this.TopPanel.Size = new System.Drawing.Size(1095, 180);
			this.TopPanel.TabIndex = 39;
			// 
			// RenameListView
			// 
			this.RenameListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.RenameListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.OldNameColumn,
            this.NewNameColumn,
            this.FullFilenameColumn,
            this.ImportedFileColumn});
			this.RenameListView.ContextMenuStrip = this.ListViewContextMenu;
			this.RenameListView.FullRowSelect = true;
			this.RenameListView.Location = new System.Drawing.Point(8, 24);
			this.RenameListView.MultiSelect = false;
			this.RenameListView.Name = "RenameListView";
			this.RenameListView.Size = new System.Drawing.Size(1079, 152);
			this.RenameListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.RenameListView.TabIndex = 2;
			this.RenameListView.UseCompatibleStateImageBehavior = false;
			this.RenameListView.View = System.Windows.Forms.View.Details;
			// 
			// OldNameColumn
			// 
			this.OldNameColumn.Text = "Old Name";
			this.OldNameColumn.Width = 229;
			// 
			// NewNameColumn
			// 
			this.NewNameColumn.Text = "New Name";
			this.NewNameColumn.Width = 265;
			// 
			// FullFilenameColumn
			// 
			this.FullFilenameColumn.Text = "Current Path";
			this.FullFilenameColumn.Width = 308;
			// 
			// ImportedFileColumn
			// 
			this.ImportedFileColumn.Text = "Imported Filename";
			this.ImportedFileColumn.Width = 272;
			// 
			// ListViewContextMenu
			// 
			this.ListViewContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ClassificationMenuItem});
			this.ListViewContextMenu.Opening += this.ListViewContextMenu_Popup;
			// 
			// ClassificationMenuItem
			// 
			this.ClassificationMenuItem.Text = "AssetClassificationGoesHere";
			this.ClassificationMenuItem.Click += new System.EventHandler(this.ClassificationMenuItem_Click);
			// 
			// RenameAssetForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(1095, 292);
			this.Controls.Add(this.TopPanel);
			this.Controls.Add(this.MiddlePanel);
			this.Controls.Add(this.BottomPanel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(584, 232);
			this.Name = "RenameAssetForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "RenameAssetForm";
			this.SizeChanged += new System.EventHandler(this.RenameAssetForm_SizeChanged);
			this.BottomPanel.ResumeLayout(false);
			this.MiddlePanel.ResumeLayout(false);
			this.NameLeftPanel.ResumeLayout(false);
			this.NameLeftPanel.PerformLayout();
			this.NameRightPanel.ResumeLayout(false);
			this.NameRightPanel.PerformLayout();
			this.TopPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void InitializeAssetName(MOG_Filename mogFilename)
		{
			InitializePlatformComboBox();
			InitializeTextBoxes( mogFilename.GetAssetClassification(), 
				mogFilename.GetAssetPlatform(), mogFilename.GetAssetName() );		
		}

		private void InitializeAssetNames(ArrayList sourceFiles)
		{
			RenameListView.Items.Clear();

			InitializePlatformComboBox();

			string listOfBlessedAssets = "";

			// Check for presence of wildcards
			foreach (string fullFilename in sourceFiles)
			{
				MOG_Filename asset = new MOG_Filename(fullFilename);
				// If this Asset has been previously blessed...
				if( CheckIfAssetHasBeenBlessed( asset ) )
				{
					listOfBlessedAssets += asset.GetAssetFullName() + "\r\n";
				}

				// Get the imported filenames
				ArrayList importFiles = DosUtils.FileGetRecursiveList(MOG_ControllerAsset.GetAssetImportedDirectory(MOG_Properties.OpenFileProperties(fullFilename + "\\Properties.info")), "*.*");
				if (importFiles.Count > 1)
				{
					// If there are more that one, then we cannot rename the files of this asset
					RenameFiles.Checked = false;
					RenameFiles.Enabled = false;
					importFilename = "*Complex asset*";
				}
				else
				{					
					String importFile = importFiles[0] as string;

					// Does this asset label match the imported filename?
					if (string.Compare(DosUtils.PathGetFileNameWithoutExtension(importFile), DosUtils.PathGetFileNameWithoutExtension(asset.GetAssetLabel()), true) == 0)
					{
						// All is good then
						importFilename = DosUtils.PathGetFileName(importFile);
					}
					else
					{
						// We cannot rename the files of this asset because the label and the imported filename do not match
						RenameFiles.Checked = false;
						RenameFiles.Enabled = false;
						importFilename = string.Format("Asset label({0}) and imported filename({1}) do not match!", DosUtils.PathGetFileNameWithoutExtension(asset.GetAssetLabel()), DosUtils.PathGetFileNameWithoutExtension(importFile));
					}

				}

				mFullFilename = fullFilename;
				ListViewItem item = RenameListView.Items.Add(asset.GetAssetFullName());
				item.SubItems.Add( asset.GetAssetFullName() );
				item.SubItems.Add( asset.GetAssetEncodedPath() );
				item.SubItems.Add( importFilename );				
				item.Selected = true;

				CheckStringForMatch(ref mCommonClass, asset.GetAssetClassification());
				CheckStringForMatch(ref mCommonPlatform, asset.GetAssetPlatform());
				CheckStringForMatch(ref mCommonLabel, asset.GetAssetLabel());
			}

			// If we have any Blessed Assets and we don't have privilege to rename them, warn the user
			if( listOfBlessedAssets.Length > 0 && !CheckPrivilegeToRename() )
			{
				MOG_Prompt.PromptMessage( "Insufficient privileges to rename already blessed assets",
										  "You do not have permission to rename these previously blessed assets:\r\n" + listOfBlessedAssets);
			}
			// Else, If we have any Blessed Assets, warn user about the rename
			else if( listOfBlessedAssets.Length > 0 )
			{
				MOG_Prompt.PromptMessage( "Inbox renames don't rename previously blessed assets", 
										  "The following blessed assets will still exist when renamed assets are blessed:\r\n" + listOfBlessedAssets);
			}

			RenameListView.Select();

			InitializeTextBoxes( mCommonClass, mCommonPlatform, mCommonLabel );

			// Make it so that our user will hopefully type over the "*" when assigning a classification...
			if( mCommonClass == "*" )
			{
				this.RenameNewClassNameTextBox.SelectAll();
			}

			bInitialized = true;
		} // end ()

		private void InitializeTextBoxes(  string className, string platformName, string labelName )
		{
			this.RenameNewClassNameTextBox.Text = this.RenameOldClassNameTextBox.Text = mCommonClass = className;
			if( className == "*" )
			{
				this.RenameToolTip.SetToolTip( this.RenameNewClassNameTextBox, "Please select a new Classification to assign these Assets to." );
			}
			this.RenameNewPlatformComboBox.Text = this.RenameOldPlatformTextBox.Text = mCommonPlatform = platformName;
			this.RenameNewLabelTextBox.Text = this.RenameOldLabelTextBox.Text = mCommonLabel = labelName;
			if( labelName == "*" )
			{
				this.RenameNewLabelTextBox.Visible = false;
				this.RenameNewLabelWarningTextBox.Visible = true;
			}
			else
			{
				this.RenameNewLabelTextBox.Visible = true;
				this.RenameNewLabelWarningTextBox.Visible = false;
			}
		}

		private void InitializePlatformComboBox()
		{
			RenameNewPlatformComboBox.Items.Clear();
			RenameNewPlatformComboBox.Items.Add("All");
			
			// Add all the platforms
			System.Collections.IEnumerator myEnumerator = MOG_ControllerProject.GetProject().GetPlatforms().GetEnumerator();
			while ( myEnumerator.MoveNext() )
			{
				RenameNewPlatformComboBox.Items.Add(((MOG_Platform)myEnumerator.Current).mPlatformName);
			}
		}

		private bool CheckStringForMatch(ref string target, string source)
		{
			// Fill in the name auto if it is uninitialized
			if (target == null)
			{
				target = source;
			}
			else
			{
				// See if this is the same as the name we already ran into
				if ( (string.Compare(target, source, true) != 0) && (string.Compare(target, "*") != 0))
				{
					// Not the same, then this field cannot be assumed to be the same for all assets
					target = "*";
					return true;
				}
			}

			return false;
		}

		private void RenameOkButton_Click(object sender, System.EventArgs e)
		{
			mNewFilename = new MOG_Filename( this.RenameNewClassNameTextBox.Text 
				+ "{" + this.RenameNewPlatformComboBox.Text + "}" 
				+ this.RenameNewLabelTextBox.Text );
			mNewMultiFilename = new MOG_Filename( this.mCommonClass + "{" + this.mCommonPlatform + "}" + this.mCommonLabel );

			mCommonClass = RenameNewClassNameTextBox.Text;
			mCommonPlatform = RenameNewPlatformComboBox.Text;
			mCommonLabel = RenameNewLabelTextBox.Text;

			// Validate the platform
			if (string.Compare(mCommonPlatform, "*", true) != 0 &&
				!MOG_ControllerProject.IsValidPlatform(mCommonPlatform))
			{
				// Inform them that this is an invalid platform
				string message = string.Concat(	"The specified platform does not exist.\n\n",
												"The asset was not renamed.");
				MOG_Prompt.PromptResponse("Asset Rename Failed", message, null, MOGPromptButtons.OK, MOG_ALERT_LEVEL.ALERT);
				return;
			}

			Rename();
		}

		private void Rename()
		{
			ProgressDialog progress = new ProgressDialog("Renaming Asset(s)", "Please wait while MOG renames the assset(s)...", Rename_Worker, null, true);
			progress.ShowDialog(this);
		}

		private void Rename_Worker(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;

			bRenameSuccessful = true;

			try
			{
				// Store a list of assets we were unable to rename
				string listOfFailedRenames = "";

				// Iterates through asset(s) to be renamed.
				for (int i = 0; i < mSourceFiles.Count && !worker.CancellationPending; i++)
				{
					string asset = mSourceFiles[i] as string;

					// We jump here when we ignore errors (for missing files)
					MOG_Filename assetName = new MOG_Filename(asset);
					string targetName = GetTargetName(assetName);

					// Rename the file with a new MOG_Controller
					MOG_ControllerInbox.Rename(assetName, targetName, this.RenameFiles.Checked);

					worker.ReportProgress(i * 100 / mSourceFiles.Count);
				}

				if (listOfFailedRenames.Length > 0)
				{
					MOG_Prompt.PromptMessage("Failed Rename", "Unable to rename the following because "
						+ "you account does not have the permission to do so:\r\n\r\n"
						+ listOfFailedRenames);
				}
			}
			catch (Exception ex)
			{
				MOG_Report.ReportMessage("Rename ERROR!", "The following error occured: \n"
					+ ex.Source + " <-- " + ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
				bRenameSuccessful = false;
			}
		}

		private string GetTargetName( MOG_Filename assetCheck, string classification, string platform, string label)
		{
			// Rename according to pattern
			string targetName = "";
						
			// Attach classification (e.g. "textures~humans")
			if ( classification == "*")  
			{
				targetName += assetCheck.GetAssetClassification();
			}
			else
			{
				targetName += classification;
			}

			// Attach platform label (e.g. 'all' OR 'pc' OR 'xbox')
			if (platform == "*") 
			{
				targetName += "{" + assetCheck.GetAssetPlatform() + "}";
			}
			else
			{
				targetName += "{" + platform + "}";
			}

			// Attach label for files (e.g. 'body')
			if (label == "*") 
			{
				targetName += assetCheck.GetAssetLabel();
			}
			else
			{
				targetName += label;
			}

			return targetName;
		}
		private string GetTargetName(MOG_Filename assetCheck)
		{				
			return GetTargetName( assetCheck, this.mCommonClass, this.mCommonPlatform, this.mCommonLabel );
		}

		/// <summary>
		/// Returns true if asset has been blessed.
		/// </summary>
		/// <param name="asset"></param>
		/// <returns></returns>
		private bool CheckIfAssetHasBeenBlessed( MOG_Filename asset )
		{
			// Create MOG_Filename for this asset
			string assetVersion = MOG.DATABASE.MOG_DBAssetAPI.GetAssetVersion( asset );

			return (assetVersion != null && assetVersion.Length > 0);
		}
		/// <summary>
		/// Returns true if user has privilege to rename a blessed asset
		/// </summary>
		/// <returns></returns>
		private bool CheckPrivilegeToRename()
		{
			string userName = MOG_ControllerProject.GetUser().GetUserName();
			return MOG_ControllerProject.GetPrivileges().GetUserPrivilege( userName, MOG_PRIVILEGE.RenameBlessedAsset );
		}

		#region Events that affect ListViewSubItems
		/// <summary>
		/// Keeps user from renaming multiple asset labels.
		/// </summary>
		private void RenameNewLabelTextBox_TextChanged(object sender, System.EventArgs e)
		{
			try
			{
				// Check for invalid characters in the new name
				if (MOG_ControllerSystem.InvalidMOGCharactersCheck(RenameNewLabelTextBox.Text, true))
				{
					RenameNewLabelTextBox.Text = MOG_ControllerSystem.ReplaceInvalidCharacters(RenameNewLabelTextBox.Text);
				}

				// For Rename Label, we only need to worry about one asset...
				if( mFullFilename != null )
				{
					MOG_Filename currentFilename = new MOG_Filename( this.mFullFilename );
					string targetName = GetTargetName( currentFilename, RenameNewClassNameTextBox.Text,
						RenameNewPlatformComboBox.Text, RenameNewLabelTextBox.Text );

					ChangeAssetFilenameInListView( targetName );

					// Update the imported files column
					if (RenameFiles.Checked && bInitialized)
					{
						ChangeAssetImportnameInListView(RenameNewLabelTextBox.Text);
					}
				}
			}
				// Eat any errors we get
			catch( Exception ex )
			{
				MOG_Prompt.PromptMessage("Error With Value", ex.Message, ex.StackTrace, MOG_ALERT_LEVEL.ALERT);
			}
		}

		private void ChangeAssetImportnameInListView(string newName)
		{
			foreach (ListViewItem item in RenameListView.Items)
			{
				// Get the old name
				string importFileName = item.SubItems[3].Text;

				// Replace with the new name
				importFileName = importFileName.Replace(DosUtils.PathGetFileNameWithoutExtension(importFileName), newName);

				// Set it
				item.SubItems[3].Text = importFileName;
			}
		}

		private void RenameNewClassNameTextBox_TextChanged(object sender, System.EventArgs e)
		{
			try
			{
				// Ignore the '*' because it is always set that way when there are multiple assets spanning multiple classifications
				if (RenameNewClassNameTextBox.Text != "*")
				{
					// Check for invalid characters in the new name
					if (MOG_ControllerSystem.InvalidMOGCharactersCheck(RenameNewClassNameTextBox.Text, true))
					{
						RenameNewClassNameTextBox.Text = MOG_ControllerSystem.ReplaceInvalidCharacters(RenameNewClassNameTextBox.Text);
					}
				}

				ChangeAssetFilenamesInListView();
			}
				// Eat any errors we get
			catch( Exception ex )
			{
				MessageBox.Show(this, ex.ToString());
			}
		}

		private void RenameNewPlatformComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			try
			{
				// Check for invalid characters in the new name
				if (MOG_ControllerSystem.InvalidMOGCharactersCheck(RenameNewPlatformComboBox.Text, true))
				{
					RenameNewPlatformComboBox.Text = MOG_ControllerSystem.ReplaceInvalidCharacters(RenameNewPlatformComboBox.Text);
				}

				ChangeAssetFilenamesInListView();
			}
			// Eat any errors we get
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.ToString());
			}
		}

		private void ChangeAssetFilenamesInListView()
		{
			foreach( ListViewItem item in RenameListView.Items )
			{
				if( item.SubItems.Count > 0 && item.SubItems[0] != null )
				{
					MOG_Filename currentFilename = new MOG_Filename( item.SubItems[0].Text );
					string targetName = GetTargetName( currentFilename, RenameNewClassNameTextBox.Text,
						RenameNewPlatformComboBox.Text, RenameNewLabelTextBox.Text );

					ChangeAssetFilenameInListViewItem( item, targetName );
				}
			}
		}
		/// <summary>
		/// Foreach ListViewItem in our ListView, change it with ChangeAssetFilenameInListViewItem()
		/// </summary>
		/// <param name="newAssetFilename"></param>
		private void ChangeAssetFilenameInListView( string newAssetFilename )
		{
			foreach( ListViewItem item in RenameListView.Items )
			{
				ChangeAssetFilenameInListViewItem( item, newAssetFilename );
			}
		}

		/// Change ListViewItem's New Name column to what it's supposed to be and change color to Red if 
		///  there was a change, else change it back to ControlText
		private void ChangeAssetFilenameInListViewItem( ListViewItem item, string newAssetFilename )
		{
			if( item != null && item.SubItems.Count > 1 && item.SubItems[1] != null)
			{
				item.SubItems[1].Text = newAssetFilename;
				if( item.SubItems[1].Text != item.SubItems[0].Text )
				{
					item.ForeColor = Color.Red;
				}
				else
				{					
					item.ForeColor = SystemColors.ControlText;
				}
			}
		}
		#endregion Events that affect ListViewSubItems

		/// <summary>
		/// Keep form from being down-sized so much that it becomes impossible to use
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RenameAssetForm_SizeChanged(object sender, System.EventArgs e)
		{
			int minWidth = 360;
			int minHeight = 200;

			Form form = (Form)sender;
			if( form.Size.Width < minWidth )
			{
				form.Size = new Size( minWidth, form.Size.Height );
			}
			if( form.Size.Height < minHeight )
			{
				form.Size = new Size( form.Size.Width, minHeight);
			}
		}

		private void RenameBrowseClassTreeButton_Click(object sender, System.EventArgs e)
		{
			BrowseClassTreeForm bctForm = new BrowseClassTreeForm();
			// Set our tag to its default value (in case the user clicks OK without selecting anything)
			bctForm.Tag = this.RenameNewClassNameTextBox.Text;
			if (bctForm.ShowDialog(this) == DialogResult.OK)
			{
				string className = (string)bctForm.Tag;
				this.RenameNewClassNameTextBox.Text = className;
			}
		}

		private void ListViewContextMenu_Popup(object sender, CancelEventArgs e)
		{
			try
			{
				if( RenameListView.SelectedItems.Count < 1 )
				{
					ClassificationMenuItem.Visible = false;
				}
				else
				{
					ClassificationMenuItem.Visible = true;
					ClassificationMenuItem.Text = "Use classification: `";
					ClassificationMenuItem.Text += GetSelectedClassification();
					ClassificationMenuItem.Text += "`";
				}
			}
			catch( Exception ex )
			{
				MessageBox.Show(this, ex.ToString());
			}
		}

		private string GetSelectedClassification()
		{
			if( this.RenameListView.SelectedItems.Count > 0 && RenameListView.SelectedItems[0] != null )
			{
				return (new MOG_Filename( RenameListView.SelectedItems[0].Text )).GetAssetClassification();
			}
			return "";
		}


		private void ClassificationMenuItem_Click(object sender, System.EventArgs e)
		{
			RenameNewClassNameTextBox.Text = GetSelectedClassification();
		}


		private void RenameNewPlatformComboBox_KeyDown(object sender, KeyEventArgs e)
		{
			e.SuppressKeyPress = true;
		}

		private void RenameFiles_CheckedChanged(object sender, EventArgs e)
		{
			if (bInitialized)
			{
				// Are we reverting?
				if (RenameFiles.Checked == false)
				{
					// If so, then reset this import filename to what it was on init
					ChangeAssetImportnameInListView(DosUtils.PathGetFileNameWithoutExtension(importFilename));
				}
				else
				{
					// If not, then update the import filename
					ChangeAssetImportnameInListView(RenameNewLabelTextBox.Text);
				}
			}
		}
	} // end class
} // end ns
