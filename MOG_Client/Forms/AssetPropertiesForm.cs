using System;
using System.Drawing;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;

using System.Drawing.Design;
using System.ComponentModel.Design;

using MOG;
using MOG.INI;
using MOG.TIME;
using MOG.FILENAME;
using MOG.PROJECT;
using MOG.PROPERTIES;
using MOG.DATABASE;
using MOG.DOSUTILS;
using MOG.COMMAND;
using MOG.CONTROLLER;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERASSET;
using MOG.CONTROLLER.CONTROLLERREPOSITORY;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.CONTROLLER.CONTROLLERINBOX;
using MOG.CONTROLLER.CONTROLLERPACKAGE;
using MOG.ASSET_STATUS;
using MOG.REPORT;
using MOG.PROMPT;
using MOG.PROGRESS;
using MOG.UITYPESEDITORS;

using MOG_Client.Client_Mog_Utilities;
using MOG_Client.Client_Mog_Utilities.AssetOptions;

using MOG_ControlsLibrary;
using MOG_ControlsLibrary.Utils;
using MOG_ControlsLibrary.Common.MOG_ContextMenu;

using MOG_Client.Client_Utilities;
using MOG_Client.Forms.AssetProperties;
using MOG_Client.Forms;

using Tst;
using MOG_CoreControls;

namespace MOG_Client
{
	/// <summary>
	/// Summary description for AssetPropertiesForm.
	/// </summary>
	public class AssetPropertiesForm : System.Windows.Forms.Form
	{
		#region User variables
		// Help us ignore KeyPressEvents in the main Form while we're working on in custom textBoxes and comboBoxes
		private bool bIgnoreFormKeyPressEvent = false;
		public bool mResetWildcardCheck = false;
		public const string Multiple_Assets_Text = "Multiple Assets Selected...";
		public const string InvalidTypeComboBox_Text = "Invalid type selected: ";
		private static ArrayList mOpenAssetProperties = new ArrayList();
		private object[] mPropertiesList;
		private bool bFormContainsBlessedAsset;
		private ImageList mPlatformImageList;
		private string mCurrentPlatform = "All";
		private TstDictionary mPlatforms;
		private bool mDisableEvents = false;
		private bool mViewPropertiesInSimpleMode;
		public MogControl_AssetContextMenu mPackageContextMenu;
		private ArrayList mAssetNames = null;
		private string mLastTabName;
		private ArrayList AssetNames
		{
			get
			{
				if( mAssetNames != null )
				{
					return mAssetNames;
				}
				else
				{
					return new ArrayList(0);
				}
			}
		}
		public bool ViewPropertiesInSimpleMode
		{
			get { return mViewPropertiesInSimpleMode; }
			set 
			{				
				TogglePropertiesViewToSimpleMode(value);				
				mViewPropertiesInSimpleMode = value;
			}
		}
	
		/// <summary>
		/// Stores the state of whether or not our Ripping was Enabled (for switching tabs)
		/// </summary>
		public object PropertiesRippingLastStateEnabled;

		#endregion
		#region Form variables
		private System.Windows.Forms.Button AssetPropertiesCloseButton;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ContextMenuStrip AssetPropertiesNewContextMenu;
		private System.Windows.Forms.ToolStripMenuItem AssetPropertiesNewMenuItem;
		private System.Windows.Forms.ToolStripMenuItem AssetPropertiesDelMenuItem;
		private System.Windows.Forms.Button AssetPropertiesExploreButton;
		private System.Windows.Forms.Label AssetPropertiesRipLogLabel;
		private System.Windows.Forms.ToolStripSeparator AssetPropertiesSeparatorMenuItem;
		private System.Windows.Forms.ToolStripMenuItem AssetPropertiesSpecialMenuItem;
		public System.Windows.Forms.Label AssetPropertiesAssetVersion;
		public System.Windows.Forms.PictureBox AssetPropertiesIconPictureBox;
		private System.Windows.Forms.TabPage PropertiesGeneralTabPage;
		private System.Windows.Forms.TabPage PropertiesLogTabPage;
		private System.Windows.Forms.TextBox PropertiesNameTextBox;
		public System.Windows.Forms.Label PropertiesVersionLabel;
		private System.Windows.Forms.Panel panel4;
		public System.Windows.Forms.Label label1;
		public System.Windows.Forms.Label label2;
		public System.Windows.Forms.Label label3;
		public System.Windows.Forms.Label label4;
		private System.Windows.Forms.TabPage PropertiesCommentsTabPage;
		private System.Windows.Forms.RichTextBox PropertiesCommentsRichTextBox;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label PropertiesLocationLabel;
		private System.Windows.Forms.Label PropertiesCreatedLabel;
		private System.Windows.Forms.Label PropertiesSizeLabel;
		private System.Windows.Forms.Label PropertiesLastBlessedLabel;
		private System.Windows.Forms.Panel panel7;
		private System.Windows.Forms.Panel panel8;
		private System.Windows.Forms.Label label6;
		public System.Windows.Forms.Label LockInfoLabel;
		private System.Windows.Forms.ImageList PropertiesImageList;
		private System.Windows.Forms.ToolTip ToolTip;
		public System.Windows.Forms.TabControl PropertiesTabControl;
		private System.Windows.Forms.ErrorProvider PropertiesErrorProvider;
		private System.Windows.Forms.TabPage PropertiesRippingTabPage;
		private System.Windows.Forms.RadioButton PropertiesSingleAssetRadioButton;
		private System.Windows.Forms.RadioButton PropertiesMultiAssetRadioButton;
		private System.Windows.Forms.RadioButton PropertiesNoPackagingRadioButton;
		private System.Windows.Forms.GroupBox PropertiesPackagingParametersGroupBox;
		private System.Windows.Forms.Button AssetPropertiesApplyButton;
		private System.Windows.Forms.Button AssetPropertiesCancelButton;
		private System.Windows.Forms.TreeView PropertiesUsersPropertiesTreeView;
		private System.Windows.Forms.ImageList UserPropertiesImageList;
		private System.Windows.Forms.GroupBox groupBox7;
		private System.Windows.Forms.RadioButton PropertiesRippingNoRipRadioButton;
		private System.Windows.Forms.RadioButton PropertiesRippingEnableRipRadioButton;
		private System.Windows.Forms.PropertyGrid PropertiesAdvancedPropertyGrid;
		private System.Windows.Forms.Button PropertiesAdvancedButton;
		private System.Windows.Forms.TabPage PropertiesTabPage;
		private System.Windows.Forms.ToolBar AssetPropertyGridToolBar;
		private System.Windows.Forms.Panel MainButtonPanel;
		private System.Windows.Forms.Panel DividerLinePanel4;
		private System.Windows.Forms.Panel DividerLinePanel3;
		private System.Windows.Forms.Panel DividerLinePanel2;
		private System.Windows.Forms.Panel DividerLinePanel1;
		private System.Windows.Forms.Panel ButtomButtonPanel;
		private MOG_Client.Forms.AssetProperties.SinglePackageControl SinglePackageControl;
		private MOG_Client.Forms.AssetProperties.MultiPackageControl MultiPackageControl;
		private System.Windows.Forms.Button PackageWizardButton;
		private System.Windows.Forms.GroupBox PackageTypeGroupBox;
		private System.Windows.Forms.TabPage PropertiesPackageTabPage;
		private System.Windows.Forms.TabPage PropertiesAssetPackagingTabPage;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button PropertiesPackageManagementButton;
		private System.Windows.Forms.ListView PropertiesAssignedPackagesListView;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.GroupBox PackagingAssetCommandsGroupBox;
		private MOG_Client.Forms.AssetProperties.RippersControl RippersControl;
		private System.Windows.Forms.ComboBox PropertiesPackagingAddComboBox;
		private System.Windows.Forms.ComboBox PropertiesPackagingRemoveComboBox;
		private System.Windows.Forms.Panel PropertiesSimpleViewPanel;
		private System.Windows.Forms.ToolBar PropertiesTreeViewToolbar;
		private System.Windows.Forms.ToolBarButton PropertiesTreeViewViewNonInheritedButton;
		private System.Windows.Forms.ToolBarButton PropertiesTreeViewViewAllButton;
		private MOG_Client.Forms.ArgumentControl.MogArgumentsButton mogAddArgumentsButton;
		private MOG_Client.Forms.ArgumentControl.MogArgumentsButton mogRemoveArgumentsButton;
		private MOG_Client.Forms.AssetProperties.MogControl_ViewTokenTextBox PackagingRemoveMogControl_ViewTokenTextBox;
		private MOG_Client.Forms.AssetProperties.MogControl_ViewTokenTextBox PackagingAddMogControl_ViewTokenTextBox;
		private System.Windows.Forms.Button PropertiesRippingWizardButton;
		private System.Windows.Forms.TabControl AssetLogsTabControl;
		#endregion		

		#region Constructor and destructor
		public AssetPropertiesForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			this.bFormContainsBlessedAsset = false;
			mViewPropertiesInSimpleMode = true;
			this.GotFocus += new EventHandler(AssetPropertiesForm_GotFocus);

			mPackageContextMenu = new MogControl_AssetContextMenu("NAME, PACKAGE, GROUPPATH, FULLNAME", this.PropertiesAssignedPackagesListView);
			this.PropertiesAssignedPackagesListView.ContextMenuStrip = mPackageContextMenu.InitializeContextMenu("{PropertiesPackages}");
			PropertiesAssignedPackagesListView.SmallImageList = MogUtil_AssetIcons.Images;
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
		#endregion
		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AssetPropertiesForm));
			this.AssetPropertiesNewContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.AssetPropertiesNewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.AssetPropertiesDelMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.AssetPropertiesSeparatorMenuItem = new System.Windows.Forms.ToolStripSeparator();
			this.AssetPropertiesSpecialMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.PropertiesImageList = new System.Windows.Forms.ImageList(this.components);
			this.AssetPropertiesCloseButton = new System.Windows.Forms.Button();
			this.AssetPropertiesIconPictureBox = new System.Windows.Forms.PictureBox();
			this.AssetPropertiesAssetVersion = new System.Windows.Forms.Label();
			this.AssetPropertiesRipLogLabel = new System.Windows.Forms.Label();
			this.MainButtonPanel = new System.Windows.Forms.Panel();
			this.AssetPropertiesCancelButton = new System.Windows.Forms.Button();
			this.AssetPropertiesApplyButton = new System.Windows.Forms.Button();
			this.AssetPropertiesExploreButton = new System.Windows.Forms.Button();
			this.PropertiesTabControl = new System.Windows.Forms.TabControl();
			this.PropertiesGeneralTabPage = new System.Windows.Forms.TabPage();
			this.LockInfoLabel = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.DividerLinePanel4 = new System.Windows.Forms.Panel();
			this.PropertiesLastBlessedLabel = new System.Windows.Forms.Label();
			this.PropertiesSizeLabel = new System.Windows.Forms.Label();
			this.PropertiesCreatedLabel = new System.Windows.Forms.Label();
			this.PropertiesLocationLabel = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.DividerLinePanel3 = new System.Windows.Forms.Panel();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.DividerLinePanel2 = new System.Windows.Forms.Panel();
			this.PropertiesVersionLabel = new System.Windows.Forms.Label();
			this.DividerLinePanel1 = new System.Windows.Forms.Panel();
			this.panel7 = new System.Windows.Forms.Panel();
			this.panel8 = new System.Windows.Forms.Panel();
			this.panel4 = new System.Windows.Forms.Panel();
			this.PropertiesNameTextBox = new System.Windows.Forms.TextBox();
			this.PropertiesRippingTabPage = new System.Windows.Forms.TabPage();
			this.PropertiesRippingWizardButton = new System.Windows.Forms.Button();
			this.groupBox7 = new System.Windows.Forms.GroupBox();
			this.PropertiesRippingEnableRipRadioButton = new System.Windows.Forms.RadioButton();
			this.PropertiesRippingNoRipRadioButton = new System.Windows.Forms.RadioButton();
			this.RippersControl = new MOG_Client.Forms.AssetProperties.RippersControl();
			this.PropertiesAssetPackagingTabPage = new System.Windows.Forms.TabPage();
			this.PackagingAssetCommandsGroupBox = new System.Windows.Forms.GroupBox();
			this.mogRemoveArgumentsButton = new MOG_Client.Forms.ArgumentControl.MogArgumentsButton();
			this.PropertiesPackagingRemoveComboBox = new System.Windows.Forms.ComboBox();
			this.mogAddArgumentsButton = new MOG_Client.Forms.ArgumentControl.MogArgumentsButton();
			this.PropertiesPackagingAddComboBox = new System.Windows.Forms.ComboBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.PackagingAddMogControl_ViewTokenTextBox = new MOG_Client.Forms.AssetProperties.MogControl_ViewTokenTextBox();
			this.PackagingRemoveMogControl_ViewTokenTextBox = new MOG_Client.Forms.AssetProperties.MogControl_ViewTokenTextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.PropertiesPackageManagementButton = new System.Windows.Forms.Button();
			this.PropertiesAssignedPackagesListView = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.PropertiesPackageTabPage = new System.Windows.Forms.TabPage();
			this.PackageWizardButton = new System.Windows.Forms.Button();
			this.PropertiesPackagingParametersGroupBox = new System.Windows.Forms.GroupBox();
			this.MultiPackageControl = new MOG_Client.Forms.AssetProperties.MultiPackageControl();
			this.SinglePackageControl = new MOG_Client.Forms.AssetProperties.SinglePackageControl();
			this.PackageTypeGroupBox = new System.Windows.Forms.GroupBox();
			this.PropertiesNoPackagingRadioButton = new System.Windows.Forms.RadioButton();
			this.PropertiesSingleAssetRadioButton = new System.Windows.Forms.RadioButton();
			this.PropertiesMultiAssetRadioButton = new System.Windows.Forms.RadioButton();
			this.PropertiesTabPage = new System.Windows.Forms.TabPage();
			this.PropertiesSimpleViewPanel = new System.Windows.Forms.Panel();
			this.PropertiesTreeViewToolbar = new System.Windows.Forms.ToolBar();
			this.PropertiesTreeViewViewNonInheritedButton = new System.Windows.Forms.ToolBarButton();
			this.PropertiesTreeViewViewAllButton = new System.Windows.Forms.ToolBarButton();
			this.PropertiesUsersPropertiesTreeView = new System.Windows.Forms.TreeView();
			this.PropertiesAdvancedButton = new System.Windows.Forms.Button();
			this.PropertiesAdvancedPropertyGrid = new System.Windows.Forms.PropertyGrid();
			this.PropertiesLogTabPage = new System.Windows.Forms.TabPage();
			this.AssetLogsTabControl = new System.Windows.Forms.TabControl();
			this.PropertiesCommentsTabPage = new System.Windows.Forms.TabPage();
			this.label5 = new System.Windows.Forms.Label();
			this.PropertiesCommentsRichTextBox = new System.Windows.Forms.RichTextBox();
			this.UserPropertiesImageList = new System.Windows.Forms.ImageList(this.components);
			this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
			this.PropertiesErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			this.AssetPropertyGridToolBar = new System.Windows.Forms.ToolBar();
			this.ButtomButtonPanel = new System.Windows.Forms.Panel();
			this.AssetPropertiesNewContextMenu.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.AssetPropertiesIconPictureBox)).BeginInit();
			this.MainButtonPanel.SuspendLayout();
			this.PropertiesTabControl.SuspendLayout();
			this.PropertiesGeneralTabPage.SuspendLayout();
			this.DividerLinePanel1.SuspendLayout();
			this.panel7.SuspendLayout();
			this.PropertiesRippingTabPage.SuspendLayout();
			this.groupBox7.SuspendLayout();
			this.PropertiesAssetPackagingTabPage.SuspendLayout();
			this.PackagingAssetCommandsGroupBox.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.PropertiesPackageTabPage.SuspendLayout();
			this.PropertiesPackagingParametersGroupBox.SuspendLayout();
			this.PackageTypeGroupBox.SuspendLayout();
			this.PropertiesTabPage.SuspendLayout();
			this.PropertiesSimpleViewPanel.SuspendLayout();
			this.PropertiesLogTabPage.SuspendLayout();
			this.PropertiesCommentsTabPage.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.PropertiesErrorProvider)).BeginInit();
			this.ButtomButtonPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// AssetPropertiesNewContextMenu
			// 
			this.AssetPropertiesNewContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AssetPropertiesNewMenuItem,
            this.AssetPropertiesDelMenuItem,
            this.AssetPropertiesSeparatorMenuItem,
            this.AssetPropertiesSpecialMenuItem});
			this.AssetPropertiesNewContextMenu.Name = "AssetPropertiesNewContextMenu";
			this.AssetPropertiesNewContextMenu.Size = new System.Drawing.Size(108, 76);
			// 
			// AssetPropertiesNewMenuItem
			// 
			this.AssetPropertiesNewMenuItem.Enabled = false;
			this.AssetPropertiesNewMenuItem.Name = "AssetPropertiesNewMenuItem";
			this.AssetPropertiesNewMenuItem.Size = new System.Drawing.Size(107, 22);
			this.AssetPropertiesNewMenuItem.Text = "New";
			// 
			// AssetPropertiesDelMenuItem
			// 
			this.AssetPropertiesDelMenuItem.Name = "AssetPropertiesDelMenuItem";
			this.AssetPropertiesDelMenuItem.Size = new System.Drawing.Size(107, 22);
			this.AssetPropertiesDelMenuItem.Text = "Delete";
			this.AssetPropertiesDelMenuItem.Click += new System.EventHandler(this.AssetPropertiesDelMenuItem_Click);
			// 
			// AssetPropertiesSeparatorMenuItem
			// 
			this.AssetPropertiesSeparatorMenuItem.Name = "AssetPropertiesSeparatorMenuItem";
			this.AssetPropertiesSeparatorMenuItem.Size = new System.Drawing.Size(104, 6);
			// 
			// AssetPropertiesSpecialMenuItem
			// 
			this.AssetPropertiesSpecialMenuItem.Enabled = false;
			this.AssetPropertiesSpecialMenuItem.Name = "AssetPropertiesSpecialMenuItem";
			this.AssetPropertiesSpecialMenuItem.Size = new System.Drawing.Size(107, 22);
			this.AssetPropertiesSpecialMenuItem.Text = "Special";
			// 
			// PropertiesImageList
			// 
			this.PropertiesImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("PropertiesImageList.ImageStream")));
			this.PropertiesImageList.TransparentColor = System.Drawing.Color.Transparent;
			this.PropertiesImageList.Images.SetKeyName(0, "");
			// 
			// AssetPropertiesCloseButton
			// 
			this.AssetPropertiesCloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.AssetPropertiesCloseButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.AssetPropertiesCloseButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.AssetPropertiesCloseButton.Location = new System.Drawing.Point(134, 31);
			this.AssetPropertiesCloseButton.Name = "AssetPropertiesCloseButton";
			this.AssetPropertiesCloseButton.Size = new System.Drawing.Size(88, 23);
			this.AssetPropertiesCloseButton.TabIndex = 1;
			this.AssetPropertiesCloseButton.Text = "Ok";
			this.AssetPropertiesCloseButton.Click += new System.EventHandler(this.AssetPropertiesCloseButton_Click);
			// 
			// AssetPropertiesIconPictureBox
			// 
			this.AssetPropertiesIconPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("AssetPropertiesIconPictureBox.Image")));
			this.AssetPropertiesIconPictureBox.Location = new System.Drawing.Point(13, 16);
			this.AssetPropertiesIconPictureBox.Name = "AssetPropertiesIconPictureBox";
			this.AssetPropertiesIconPictureBox.Size = new System.Drawing.Size(32, 24);
			this.AssetPropertiesIconPictureBox.TabIndex = 8;
			this.AssetPropertiesIconPictureBox.TabStop = false;
			// 
			// AssetPropertiesAssetVersion
			// 
			this.AssetPropertiesAssetVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.AssetPropertiesAssetVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.AssetPropertiesAssetVersion.Location = new System.Drawing.Point(16, 56);
			this.AssetPropertiesAssetVersion.Name = "AssetPropertiesAssetVersion";
			this.AssetPropertiesAssetVersion.Size = new System.Drawing.Size(190, 16);
			this.AssetPropertiesAssetVersion.TabIndex = 7;
			this.AssetPropertiesAssetVersion.Text = "Version:";
			// 
			// AssetPropertiesRipLogLabel
			// 
			this.AssetPropertiesRipLogLabel.Location = new System.Drawing.Point(16, 8);
			this.AssetPropertiesRipLogLabel.Name = "AssetPropertiesRipLogLabel";
			this.AssetPropertiesRipLogLabel.Size = new System.Drawing.Size(224, 16);
			this.AssetPropertiesRipLogLabel.TabIndex = 5;
			this.AssetPropertiesRipLogLabel.Text = "Rip Logs:";
			// 
			// MainButtonPanel
			// 
			this.MainButtonPanel.Controls.Add(this.AssetPropertiesCancelButton);
			this.MainButtonPanel.Controls.Add(this.AssetPropertiesApplyButton);
			this.MainButtonPanel.Controls.Add(this.AssetPropertiesCloseButton);
			this.MainButtonPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.MainButtonPanel.Location = new System.Drawing.Point(0, 0);
			this.MainButtonPanel.Name = "MainButtonPanel";
			this.MainButtonPanel.Size = new System.Drawing.Size(430, 57);
			this.MainButtonPanel.TabIndex = 5;
			// 
			// AssetPropertiesCancelButton
			// 
			this.AssetPropertiesCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.AssetPropertiesCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.AssetPropertiesCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.AssetPropertiesCancelButton.Location = new System.Drawing.Point(234, 31);
			this.AssetPropertiesCancelButton.Name = "AssetPropertiesCancelButton";
			this.AssetPropertiesCancelButton.Size = new System.Drawing.Size(88, 23);
			this.AssetPropertiesCancelButton.TabIndex = 3;
			this.AssetPropertiesCancelButton.Text = "Cancel";
			this.AssetPropertiesCancelButton.Click += new System.EventHandler(this.AssetPropertiesCancelButton_Click);
			// 
			// AssetPropertiesApplyButton
			// 
			this.AssetPropertiesApplyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.AssetPropertiesApplyButton.Enabled = false;
			this.AssetPropertiesApplyButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.AssetPropertiesApplyButton.Location = new System.Drawing.Point(334, 31);
			this.AssetPropertiesApplyButton.Name = "AssetPropertiesApplyButton";
			this.AssetPropertiesApplyButton.Size = new System.Drawing.Size(88, 23);
			this.AssetPropertiesApplyButton.TabIndex = 2;
			this.AssetPropertiesApplyButton.Text = "Apply";
			this.AssetPropertiesApplyButton.Click += new System.EventHandler(this.AssetPropertiesApplyButton_Click);
			// 
			// AssetPropertiesExploreButton
			// 
			this.AssetPropertiesExploreButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.AssetPropertiesExploreButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.AssetPropertiesExploreButton.Location = new System.Drawing.Point(16, 505);
			this.AssetPropertiesExploreButton.Name = "AssetPropertiesExploreButton";
			this.AssetPropertiesExploreButton.Size = new System.Drawing.Size(88, 23);
			this.AssetPropertiesExploreButton.TabIndex = 3;
			this.AssetPropertiesExploreButton.Text = "Explore Asset...";
			this.ToolTip.SetToolTip(this.AssetPropertiesExploreButton, "Explores into the Asset/Package (does not open the file)");
			this.AssetPropertiesExploreButton.Click += new System.EventHandler(this.AssetPropertiesExploreButton_Click);
			// 
			// PropertiesTabControl
			// 
			this.PropertiesTabControl.Controls.Add(this.PropertiesGeneralTabPage);
			this.PropertiesTabControl.Controls.Add(this.PropertiesRippingTabPage);
			this.PropertiesTabControl.Controls.Add(this.PropertiesAssetPackagingTabPage);
			this.PropertiesTabControl.Controls.Add(this.PropertiesPackageTabPage);
			this.PropertiesTabControl.Controls.Add(this.PropertiesTabPage);
			this.PropertiesTabControl.Controls.Add(this.PropertiesLogTabPage);
			this.PropertiesTabControl.Controls.Add(this.PropertiesCommentsTabPage);
			this.PropertiesTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PropertiesTabControl.Location = new System.Drawing.Point(0, 0);
			this.PropertiesTabControl.Name = "PropertiesTabControl";
			this.PropertiesTabControl.SelectedIndex = 0;
			this.PropertiesTabControl.Size = new System.Drawing.Size(430, 558);
			this.PropertiesTabControl.TabIndex = 7;
			this.PropertiesTabControl.SelectedIndexChanged += new System.EventHandler(this.PropertiesTabControl_SelectedIndexChanged);
			// 
			// PropertiesGeneralTabPage
			// 
			this.PropertiesGeneralTabPage.Controls.Add(this.LockInfoLabel);
			this.PropertiesGeneralTabPage.Controls.Add(this.label6);
			this.PropertiesGeneralTabPage.Controls.Add(this.DividerLinePanel4);
			this.PropertiesGeneralTabPage.Controls.Add(this.PropertiesLastBlessedLabel);
			this.PropertiesGeneralTabPage.Controls.Add(this.PropertiesSizeLabel);
			this.PropertiesGeneralTabPage.Controls.Add(this.PropertiesCreatedLabel);
			this.PropertiesGeneralTabPage.Controls.Add(this.PropertiesLocationLabel);
			this.PropertiesGeneralTabPage.Controls.Add(this.label4);
			this.PropertiesGeneralTabPage.Controls.Add(this.label3);
			this.PropertiesGeneralTabPage.Controls.Add(this.DividerLinePanel3);
			this.PropertiesGeneralTabPage.Controls.Add(this.label2);
			this.PropertiesGeneralTabPage.Controls.Add(this.label1);
			this.PropertiesGeneralTabPage.Controls.Add(this.DividerLinePanel2);
			this.PropertiesGeneralTabPage.Controls.Add(this.PropertiesVersionLabel);
			this.PropertiesGeneralTabPage.Controls.Add(this.DividerLinePanel1);
			this.PropertiesGeneralTabPage.Controls.Add(this.PropertiesNameTextBox);
			this.PropertiesGeneralTabPage.Controls.Add(this.AssetPropertiesIconPictureBox);
			this.PropertiesGeneralTabPage.Controls.Add(this.AssetPropertiesAssetVersion);
			this.PropertiesGeneralTabPage.Controls.Add(this.AssetPropertiesExploreButton);
			this.PropertiesGeneralTabPage.Location = new System.Drawing.Point(4, 22);
			this.PropertiesGeneralTabPage.Name = "PropertiesGeneralTabPage";
			this.PropertiesGeneralTabPage.Size = new System.Drawing.Size(422, 532);
			this.PropertiesGeneralTabPage.TabIndex = 0;
			this.PropertiesGeneralTabPage.Text = "General";
			this.PropertiesGeneralTabPage.ToolTipText = "General asset information";
			// 
			// LockInfoLabel
			// 
			this.LockInfoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.LockInfoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LockInfoLabel.Location = new System.Drawing.Point(80, 248);
			this.LockInfoLabel.Name = "LockInfoLabel";
			this.LockInfoLabel.Size = new System.Drawing.Size(326, 238);
			this.LockInfoLabel.TabIndex = 25;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(16, 248);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(56, 16);
			this.label6.TabIndex = 24;
			this.label6.Text = "Lock Info:";
			// 
			// DividerLinePanel4
			// 
			this.DividerLinePanel4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.DividerLinePanel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.DividerLinePanel4.Location = new System.Drawing.Point(16, 232);
			this.DividerLinePanel4.Name = "DividerLinePanel4";
			this.DividerLinePanel4.Size = new System.Drawing.Size(392, 3);
			this.DividerLinePanel4.TabIndex = 22;
			// 
			// PropertiesLastBlessedLabel
			// 
			this.PropertiesLastBlessedLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesLastBlessedLabel.Location = new System.Drawing.Point(88, 200);
			this.PropertiesLastBlessedLabel.Name = "PropertiesLastBlessedLabel";
			this.PropertiesLastBlessedLabel.Size = new System.Drawing.Size(222, 16);
			this.PropertiesLastBlessedLabel.TabIndex = 21;
			this.PropertiesLastBlessedLabel.Text = "Tuesday";
			// 
			// PropertiesSizeLabel
			// 
			this.PropertiesSizeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesSizeLabel.Location = new System.Drawing.Point(88, 136);
			this.PropertiesSizeLabel.Name = "PropertiesSizeLabel";
			this.PropertiesSizeLabel.Size = new System.Drawing.Size(318, 16);
			this.PropertiesSizeLabel.TabIndex = 20;
			this.PropertiesSizeLabel.Text = "100 KB";
			// 
			// PropertiesCreatedLabel
			// 
			this.PropertiesCreatedLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesCreatedLabel.Location = new System.Drawing.Point(88, 176);
			this.PropertiesCreatedLabel.Name = "PropertiesCreatedLabel";
			this.PropertiesCreatedLabel.Size = new System.Drawing.Size(318, 16);
			this.PropertiesCreatedLabel.TabIndex = 19;
			this.PropertiesCreatedLabel.Text = "Monday";
			// 
			// PropertiesLocationLabel
			// 
			this.PropertiesLocationLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesLocationLabel.Location = new System.Drawing.Point(88, 96);
			this.PropertiesLocationLabel.Name = "PropertiesLocationLabel";
			this.PropertiesLocationLabel.Size = new System.Drawing.Size(318, 40);
			this.PropertiesLocationLabel.TabIndex = 18;
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(16, 200);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(214, 16);
			this.label4.TabIndex = 17;
			this.label4.Text = "Last Blessed:";
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(16, 176);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(190, 16);
			this.label3.TabIndex = 16;
			this.label3.Text = "Created:";
			// 
			// DividerLinePanel3
			// 
			this.DividerLinePanel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.DividerLinePanel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.DividerLinePanel3.Location = new System.Drawing.Point(16, 160);
			this.DividerLinePanel3.Name = "DividerLinePanel3";
			this.DividerLinePanel3.Size = new System.Drawing.Size(392, 3);
			this.DividerLinePanel3.TabIndex = 15;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(16, 136);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(190, 16);
			this.label2.TabIndex = 14;
			this.label2.Text = "Size:";
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(16, 96);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(190, 16);
			this.label1.TabIndex = 13;
			this.label1.Text = "Location:";
			// 
			// DividerLinePanel2
			// 
			this.DividerLinePanel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.DividerLinePanel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.DividerLinePanel2.Location = new System.Drawing.Point(16, 80);
			this.DividerLinePanel2.Name = "DividerLinePanel2";
			this.DividerLinePanel2.Size = new System.Drawing.Size(392, 3);
			this.DividerLinePanel2.TabIndex = 12;
			// 
			// PropertiesVersionLabel
			// 
			this.PropertiesVersionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesVersionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.PropertiesVersionLabel.Location = new System.Drawing.Point(88, 56);
			this.PropertiesVersionLabel.Name = "PropertiesVersionLabel";
			this.PropertiesVersionLabel.Size = new System.Drawing.Size(318, 16);
			this.PropertiesVersionLabel.TabIndex = 11;
			this.PropertiesVersionLabel.Text = "Local Version";
			// 
			// DividerLinePanel1
			// 
			this.DividerLinePanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.DividerLinePanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.DividerLinePanel1.Controls.Add(this.panel7);
			this.DividerLinePanel1.Controls.Add(this.panel4);
			this.DividerLinePanel1.Location = new System.Drawing.Point(16, 48);
			this.DividerLinePanel1.Name = "DividerLinePanel1";
			this.DividerLinePanel1.Size = new System.Drawing.Size(392, 3);
			this.DividerLinePanel1.TabIndex = 10;
			// 
			// panel7
			// 
			this.panel7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.panel7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel7.Controls.Add(this.panel8);
			this.panel7.Location = new System.Drawing.Point(-1, -1);
			this.panel7.Name = "panel7";
			this.panel7.Size = new System.Drawing.Size(392, 3);
			this.panel7.TabIndex = 12;
			// 
			// panel8
			// 
			this.panel8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.panel8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel8.Location = new System.Drawing.Point(-1, -1);
			this.panel8.Name = "panel8";
			this.panel8.Size = new System.Drawing.Size(342, 3);
			this.panel8.TabIndex = 11;
			// 
			// panel4
			// 
			this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel4.Location = new System.Drawing.Point(-1, -1);
			this.panel4.Name = "panel4";
			this.panel4.Size = new System.Drawing.Size(336, 3);
			this.panel4.TabIndex = 11;
			// 
			// PropertiesNameTextBox
			// 
			this.PropertiesNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesNameTextBox.Location = new System.Drawing.Point(56, 16);
			this.PropertiesNameTextBox.Name = "PropertiesNameTextBox";
			this.PropertiesNameTextBox.Size = new System.Drawing.Size(350, 20);
			this.PropertiesNameTextBox.TabIndex = 9;
			this.PropertiesNameTextBox.Text = "My Asset";
			// 
			// PropertiesRippingTabPage
			// 
			this.PropertiesRippingTabPage.Controls.Add(this.PropertiesRippingWizardButton);
			this.PropertiesRippingTabPage.Controls.Add(this.groupBox7);
			this.PropertiesRippingTabPage.Controls.Add(this.RippersControl);
			this.PropertiesRippingTabPage.Location = new System.Drawing.Point(4, 22);
			this.PropertiesRippingTabPage.Name = "PropertiesRippingTabPage";
			this.PropertiesRippingTabPage.Size = new System.Drawing.Size(422, 532);
			this.PropertiesRippingTabPage.TabIndex = 5;
			this.PropertiesRippingTabPage.Text = "Ripping";
			this.PropertiesRippingTabPage.ToolTipText = "Options for setting up asset rippers";
			// 
			// PropertiesRippingWizardButton
			// 
			this.PropertiesRippingWizardButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.PropertiesRippingWizardButton.Image = ((System.Drawing.Image)(resources.GetObject("PropertiesRippingWizardButton.Image")));
			this.PropertiesRippingWizardButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.PropertiesRippingWizardButton.Location = new System.Drawing.Point(8, 497);
			this.PropertiesRippingWizardButton.Name = "PropertiesRippingWizardButton";
			this.PropertiesRippingWizardButton.Size = new System.Drawing.Size(192, 23);
			this.PropertiesRippingWizardButton.TabIndex = 9;
			this.PropertiesRippingWizardButton.Text = "Run Asset Ripping Setup Wizard";
			this.PropertiesRippingWizardButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.PropertiesRippingWizardButton.Click += new System.EventHandler(this.PropertiesRippingWizardButton_Click);
			// 
			// groupBox7
			// 
			this.groupBox7.Controls.Add(this.PropertiesRippingEnableRipRadioButton);
			this.groupBox7.Controls.Add(this.PropertiesRippingNoRipRadioButton);
			this.groupBox7.Location = new System.Drawing.Point(16, 8);
			this.groupBox7.Name = "groupBox7";
			this.groupBox7.Size = new System.Drawing.Size(128, 64);
			this.groupBox7.TabIndex = 8;
			this.groupBox7.TabStop = false;
			this.groupBox7.Text = "Asset type";
			// 
			// PropertiesRippingEnableRipRadioButton
			// 
			this.PropertiesRippingEnableRipRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PropertiesRippingEnableRipRadioButton.Location = new System.Drawing.Point(16, 37);
			this.PropertiesRippingEnableRipRadioButton.Name = "PropertiesRippingEnableRipRadioButton";
			this.PropertiesRippingEnableRipRadioButton.Size = new System.Drawing.Size(104, 24);
			this.PropertiesRippingEnableRipRadioButton.TabIndex = 1;
			this.PropertiesRippingEnableRipRadioButton.Text = "Enable Ripping";
			this.PropertiesRippingEnableRipRadioButton.CheckedChanged += new System.EventHandler(this.PropertiesRippingEnableRipRadioButton_CheckedChanged);
			// 
			// PropertiesRippingNoRipRadioButton
			// 
			this.PropertiesRippingNoRipRadioButton.Checked = true;
			this.PropertiesRippingNoRipRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PropertiesRippingNoRipRadioButton.Location = new System.Drawing.Point(16, 13);
			this.PropertiesRippingNoRipRadioButton.Name = "PropertiesRippingNoRipRadioButton";
			this.PropertiesRippingNoRipRadioButton.Size = new System.Drawing.Size(104, 24);
			this.PropertiesRippingNoRipRadioButton.TabIndex = 0;
			this.PropertiesRippingNoRipRadioButton.TabStop = true;
			this.PropertiesRippingNoRipRadioButton.Text = "No Ripping";
			this.PropertiesRippingNoRipRadioButton.CheckedChanged += new System.EventHandler(this.PropertiesRippingNoRipRadioButton_CheckedChanged);
			// 
			// RippersControl
			// 
			this.RippersControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.RippersControl.Location = new System.Drawing.Point(8, 80);
			this.RippersControl.Name = "RippersControl";
			this.RippersControl.PropertiesList = null;
			this.RippersControl.Size = new System.Drawing.Size(404, 409);
			this.RippersControl.TabIndex = 0;
			this.RippersControl.Visible = false;
			this.RippersControl.ProptertyChanged += new MOG_Client.Forms.AssetProperties.RippersControl.PropertyChangedEvent(this.Properties_MainPropertyChangedEvent);
			// 
			// PropertiesAssetPackagingTabPage
			// 
			this.PropertiesAssetPackagingTabPage.Controls.Add(this.PackagingAssetCommandsGroupBox);
			this.PropertiesAssetPackagingTabPage.Controls.Add(this.button1);
			this.PropertiesAssetPackagingTabPage.Controls.Add(this.groupBox2);
			this.PropertiesAssetPackagingTabPage.Location = new System.Drawing.Point(4, 22);
			this.PropertiesAssetPackagingTabPage.Name = "PropertiesAssetPackagingTabPage";
			this.PropertiesAssetPackagingTabPage.Size = new System.Drawing.Size(422, 532);
			this.PropertiesAssetPackagingTabPage.TabIndex = 7;
			this.PropertiesAssetPackagingTabPage.Text = "Packaging";
			this.PropertiesAssetPackagingTabPage.ToolTipText = "Options for setting up asset packaging";
			// 
			// PackagingAssetCommandsGroupBox
			// 
			this.PackagingAssetCommandsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.PackagingAssetCommandsGroupBox.Controls.Add(this.mogRemoveArgumentsButton);
			this.PackagingAssetCommandsGroupBox.Controls.Add(this.mogAddArgumentsButton);
			this.PackagingAssetCommandsGroupBox.Controls.Add(this.PropertiesPackagingRemoveComboBox);
			this.PackagingAssetCommandsGroupBox.Controls.Add(this.PropertiesPackagingAddComboBox);
			this.PackagingAssetCommandsGroupBox.Controls.Add(this.label8);
			this.PackagingAssetCommandsGroupBox.Controls.Add(this.label7);
			this.PackagingAssetCommandsGroupBox.Controls.Add(this.PackagingAddMogControl_ViewTokenTextBox);
			this.PackagingAssetCommandsGroupBox.Controls.Add(this.PackagingRemoveMogControl_ViewTokenTextBox);
			this.PackagingAssetCommandsGroupBox.Enabled = false;
			this.PackagingAssetCommandsGroupBox.Location = new System.Drawing.Point(8, 216);
			this.PackagingAssetCommandsGroupBox.Name = "PackagingAssetCommandsGroupBox";
			this.PackagingAssetCommandsGroupBox.Size = new System.Drawing.Size(404, 136);
			this.PackagingAssetCommandsGroupBox.TabIndex = 17;
			this.PackagingAssetCommandsGroupBox.TabStop = false;
			this.PackagingAssetCommandsGroupBox.Text = "Asset Packaging Commands";
			// 
			// mogRemoveArgumentsButton
			// 
			this.mogRemoveArgumentsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.mogRemoveArgumentsButton.ButtonText = ">";
			this.mogRemoveArgumentsButton.Location = new System.Drawing.Point(375, 79);
			this.mogRemoveArgumentsButton.MOGAssetFilename = null;
			this.mogRemoveArgumentsButton.Name = "mogRemoveArgumentsButton";
			this.mogRemoveArgumentsButton.Size = new System.Drawing.Size(24, 23);
			this.mogRemoveArgumentsButton.TabIndex = 34;
			this.mogRemoveArgumentsButton.TargetComboBox = this.PropertiesPackagingRemoveComboBox;
			this.mogRemoveArgumentsButton.TargetTextBox = null;
			// 
			// PropertiesPackagingRemoveComboBox
			// 
			this.PropertiesPackagingRemoveComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesPackagingRemoveComboBox.Items.AddRange(new object[] {
            "None",
            "Inherited"});
			this.PropertiesPackagingRemoveComboBox.Location = new System.Drawing.Point(80, 80);
			this.PropertiesPackagingRemoveComboBox.Name = "PropertiesPackagingRemoveComboBox";
			this.PropertiesPackagingRemoveComboBox.Size = new System.Drawing.Size(288, 21);
			this.PropertiesPackagingRemoveComboBox.TabIndex = 32;
			this.PropertiesPackagingRemoveComboBox.Leave += new System.EventHandler(this.PropertiesPackagingRemoveComboBox_Leave);
			this.PropertiesPackagingRemoveComboBox.Validating += new System.ComponentModel.CancelEventHandler(this.PropertiesPackagingRemoveComboBox_Validating);
			this.PropertiesPackagingRemoveComboBox.Validated += new System.EventHandler(this.Properties_MainPropertyChangedEvent);
			this.PropertiesPackagingRemoveComboBox.TextChanged += new System.EventHandler(this.PropertiesPackagingAddComboBox_TextChanged);
			// 
			// mogAddArgumentsButton
			// 
			this.mogAddArgumentsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.mogAddArgumentsButton.ButtonText = ">";
			this.mogAddArgumentsButton.Location = new System.Drawing.Point(375, 24);
			this.mogAddArgumentsButton.MOGAssetFilename = null;
			this.mogAddArgumentsButton.Name = "mogAddArgumentsButton";
			this.mogAddArgumentsButton.Size = new System.Drawing.Size(24, 23);
			this.mogAddArgumentsButton.TabIndex = 33;
			this.mogAddArgumentsButton.TargetComboBox = this.PropertiesPackagingAddComboBox;
			this.mogAddArgumentsButton.TargetTextBox = null;
			// 
			// PropertiesPackagingAddComboBox
			// 
			this.PropertiesPackagingAddComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesPackagingAddComboBox.Items.AddRange(new object[] {
            "None",
            "Inherited"});
			this.PropertiesPackagingAddComboBox.Location = new System.Drawing.Point(80, 24);
			this.PropertiesPackagingAddComboBox.Name = "PropertiesPackagingAddComboBox";
			this.PropertiesPackagingAddComboBox.Size = new System.Drawing.Size(288, 21);
			this.PropertiesPackagingAddComboBox.TabIndex = 31;
			this.PropertiesPackagingAddComboBox.Leave += new System.EventHandler(this.PropertiesPackagingAddComboBox_Leave);
			this.PropertiesPackagingAddComboBox.Validating += new System.ComponentModel.CancelEventHandler(this.PropertiesPackagingAddComboBox_Validating);
			this.PropertiesPackagingAddComboBox.Validated += new System.EventHandler(this.Properties_MainPropertyChangedEvent);
			this.PropertiesPackagingAddComboBox.TextChanged += new System.EventHandler(this.PropertiesPackagingAddComboBox_TextChanged);
			// 
			// label8
			// 
			this.label8.Image = ((System.Drawing.Image)(resources.GetObject("label8.Image")));
			this.label8.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label8.Location = new System.Drawing.Point(16, 80);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(64, 16);
			this.label8.TabIndex = 14;
			this.label8.Text = "     Remove";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label7
			// 
			this.label7.Image = ((System.Drawing.Image)(resources.GetObject("label7.Image")));
			this.label7.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label7.Location = new System.Drawing.Point(16, 26);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(48, 16);
			this.label7.TabIndex = 12;
			this.label7.Text = "     Add";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// PackagingAddMogControl_ViewTokenTextBox
			// 
			this.PackagingAddMogControl_ViewTokenTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.PackagingAddMogControl_ViewTokenTextBox.BackColor = System.Drawing.SystemColors.Control;
			this.PackagingAddMogControl_ViewTokenTextBox.Location = new System.Drawing.Point(81, 49);
			this.PackagingAddMogControl_ViewTokenTextBox.MOGAssetFilename = null;
			this.PackagingAddMogControl_ViewTokenTextBox.Name = "PackagingAddMogControl_ViewTokenTextBox";
			this.PackagingAddMogControl_ViewTokenTextBox.Size = new System.Drawing.Size(285, 24);
			this.PackagingAddMogControl_ViewTokenTextBox.SourceComboBox = this.PropertiesPackagingAddComboBox;
			this.PackagingAddMogControl_ViewTokenTextBox.SourceTextBox = null;
			this.PackagingAddMogControl_ViewTokenTextBox.TabIndex = 18;
			this.ToolTip.SetToolTip(this.PackagingAddMogControl_ViewTokenTextBox, "Resolved token window for add command");
			// 
			// PackagingRemoveMogControl_ViewTokenTextBox
			// 
			this.PackagingRemoveMogControl_ViewTokenTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.PackagingRemoveMogControl_ViewTokenTextBox.BackColor = System.Drawing.SystemColors.Control;
			this.PackagingRemoveMogControl_ViewTokenTextBox.Location = new System.Drawing.Point(81, 105);
			this.PackagingRemoveMogControl_ViewTokenTextBox.MOGAssetFilename = null;
			this.PackagingRemoveMogControl_ViewTokenTextBox.Name = "PackagingRemoveMogControl_ViewTokenTextBox";
			this.PackagingRemoveMogControl_ViewTokenTextBox.Size = new System.Drawing.Size(286, 24);
			this.PackagingRemoveMogControl_ViewTokenTextBox.SourceComboBox = this.PropertiesPackagingRemoveComboBox;
			this.PackagingRemoveMogControl_ViewTokenTextBox.SourceTextBox = null;
			this.PackagingRemoveMogControl_ViewTokenTextBox.TabIndex = 19;
			this.ToolTip.SetToolTip(this.PackagingRemoveMogControl_ViewTokenTextBox, "Resolved token window for remove command");
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.button1.Enabled = false;
			this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
			this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.button1.Location = new System.Drawing.Point(8, 501);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(216, 24);
			this.button1.TabIndex = 16;
			this.button1.Text = "      Run Asset Packaging Setup Wizard";
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.PropertiesPackageManagementButton);
			this.groupBox2.Controls.Add(this.PropertiesAssignedPackagesListView);
			this.groupBox2.Location = new System.Drawing.Point(8, 16);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(404, 184);
			this.groupBox2.TabIndex = 15;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Current Package Assignments";
			// 
			// PropertiesPackageManagementButton
			// 
			this.PropertiesPackageManagementButton.Image = ((System.Drawing.Image)(resources.GetObject("PropertiesPackageManagementButton.Image")));
			this.PropertiesPackageManagementButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.PropertiesPackageManagementButton.Location = new System.Drawing.Point(8, 156);
			this.PropertiesPackageManagementButton.Name = "PropertiesPackageManagementButton";
			this.PropertiesPackageManagementButton.Size = new System.Drawing.Size(144, 23);
			this.PropertiesPackageManagementButton.TabIndex = 14;
			this.PropertiesPackageManagementButton.Text = "Package Management...";
			this.PropertiesPackageManagementButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.ToolTip.SetToolTip(this.PropertiesPackageManagementButton, "Open Package Management...");
			this.PropertiesPackageManagementButton.Click += new System.EventHandler(this.PropertiesPackageManagementButton_Click);
			// 
			// PropertiesAssignedPackagesListView
			// 
			this.PropertiesAssignedPackagesListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesAssignedPackagesListView.BackColor = System.Drawing.SystemColors.Control;
			this.PropertiesAssignedPackagesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
			this.PropertiesAssignedPackagesListView.Enabled = false;
			this.PropertiesAssignedPackagesListView.FullRowSelect = true;
			this.PropertiesAssignedPackagesListView.GridLines = true;
			this.PropertiesAssignedPackagesListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.PropertiesAssignedPackagesListView.Location = new System.Drawing.Point(8, 16);
			this.PropertiesAssignedPackagesListView.Name = "PropertiesAssignedPackagesListView";
			this.PropertiesAssignedPackagesListView.Size = new System.Drawing.Size(388, 136);
			this.PropertiesAssignedPackagesListView.Sorting = System.Windows.Forms.SortOrder.Descending;
			this.PropertiesAssignedPackagesListView.TabIndex = 13;
			this.PropertiesAssignedPackagesListView.UseCompatibleStateImageBehavior = false;
			this.PropertiesAssignedPackagesListView.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Asset";
			this.columnHeader1.Width = 87;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Package";
			this.columnHeader2.Width = 152;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Group";
			this.columnHeader3.Width = 77;
			// 
			// PropertiesPackageTabPage
			// 
			this.PropertiesPackageTabPage.Controls.Add(this.PackageWizardButton);
			this.PropertiesPackageTabPage.Controls.Add(this.PropertiesPackagingParametersGroupBox);
			this.PropertiesPackageTabPage.Controls.Add(this.PackageTypeGroupBox);
			this.PropertiesPackageTabPage.Location = new System.Drawing.Point(4, 22);
			this.PropertiesPackageTabPage.Name = "PropertiesPackageTabPage";
			this.PropertiesPackageTabPage.Size = new System.Drawing.Size(422, 532);
			this.PropertiesPackageTabPage.TabIndex = 4;
			this.PropertiesPackageTabPage.Text = "Package";
			this.PropertiesPackageTabPage.ToolTipText = "Options for a package file";
			// 
			// PackageWizardButton
			// 
			this.PackageWizardButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.PackageWizardButton.Enabled = false;
			this.PackageWizardButton.Image = ((System.Drawing.Image)(resources.GetObject("PackageWizardButton.Image")));
			this.PackageWizardButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.PackageWizardButton.Location = new System.Drawing.Point(8, 501);
			this.PackageWizardButton.Name = "PackageWizardButton";
			this.PackageWizardButton.Size = new System.Drawing.Size(178, 24);
			this.PackageWizardButton.TabIndex = 16;
			this.PackageWizardButton.Text = "      Run Package Setup Wizard";
			this.PackageWizardButton.Click += new System.EventHandler(this.PackageWizardButton_Click);
			// 
			// PropertiesPackagingParametersGroupBox
			// 
			this.PropertiesPackagingParametersGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesPackagingParametersGroupBox.Controls.Add(this.MultiPackageControl);
			this.PropertiesPackagingParametersGroupBox.Controls.Add(this.SinglePackageControl);
			this.PropertiesPackagingParametersGroupBox.Location = new System.Drawing.Point(8, 112);
			this.PropertiesPackagingParametersGroupBox.Name = "PropertiesPackagingParametersGroupBox";
			this.PropertiesPackagingParametersGroupBox.Size = new System.Drawing.Size(408, 381);
			this.PropertiesPackagingParametersGroupBox.TabIndex = 12;
			this.PropertiesPackagingParametersGroupBox.TabStop = false;
			this.PropertiesPackagingParametersGroupBox.Text = "Package Properties";
			this.PropertiesPackagingParametersGroupBox.Visible = false;
			// 
			// MultiPackageControl
			// 
			this.MultiPackageControl.Location = new System.Drawing.Point(8, 16);
			this.MultiPackageControl.Name = "MultiPackageControl";
			this.MultiPackageControl.Size = new System.Drawing.Size(240, 400);
			this.MultiPackageControl.TabIndex = 1;
			this.MultiPackageControl.Visible = false;
			this.MultiPackageControl.ProptertyChanged += new MOG_Client.Forms.AssetProperties.MultiPackageControl.PropertyChangedEvent(this.Properties_MainPropertyChangedEvent);
			// 
			// SinglePackageControl
			// 
			this.SinglePackageControl.Location = new System.Drawing.Point(248, 16);
			this.SinglePackageControl.Name = "SinglePackageControl";
			this.SinglePackageControl.PropertiesList = null;
			this.SinglePackageControl.Size = new System.Drawing.Size(224, 320);
			this.SinglePackageControl.TabIndex = 0;
			this.SinglePackageControl.Visible = false;
			this.SinglePackageControl.ProptertyChanged += new MOG_Client.Forms.AssetProperties.SinglePackageControl.PropertyChangedEvent(this.Properties_MainPropertyChangedEvent);
			// 
			// PackageTypeGroupBox
			// 
			this.PackageTypeGroupBox.Controls.Add(this.PropertiesNoPackagingRadioButton);
			this.PackageTypeGroupBox.Controls.Add(this.PropertiesSingleAssetRadioButton);
			this.PackageTypeGroupBox.Controls.Add(this.PropertiesMultiAssetRadioButton);
			this.PackageTypeGroupBox.Location = new System.Drawing.Point(8, 8);
			this.PackageTypeGroupBox.Name = "PackageTypeGroupBox";
			this.PackageTypeGroupBox.Size = new System.Drawing.Size(184, 96);
			this.PackageTypeGroupBox.TabIndex = 11;
			this.PackageTypeGroupBox.TabStop = false;
			this.PackageTypeGroupBox.Text = "Packaging Type";
			// 
			// PropertiesNoPackagingRadioButton
			// 
			this.PropertiesNoPackagingRadioButton.Checked = true;
			this.PropertiesNoPackagingRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PropertiesNoPackagingRadioButton.Location = new System.Drawing.Point(24, 16);
			this.PropertiesNoPackagingRadioButton.Name = "PropertiesNoPackagingRadioButton";
			this.PropertiesNoPackagingRadioButton.Size = new System.Drawing.Size(144, 24);
			this.PropertiesNoPackagingRadioButton.TabIndex = 11;
			this.PropertiesNoPackagingRadioButton.TabStop = true;
			this.PropertiesNoPackagingRadioButton.Text = "No Packaging";
			this.PropertiesNoPackagingRadioButton.CheckedChanged += new System.EventHandler(this.PropertiesNoPackagingRadioButton_CheckedChanged);
			// 
			// PropertiesSingleAssetRadioButton
			// 
			this.PropertiesSingleAssetRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PropertiesSingleAssetRadioButton.Location = new System.Drawing.Point(24, 40);
			this.PropertiesSingleAssetRadioButton.Name = "PropertiesSingleAssetRadioButton";
			this.PropertiesSingleAssetRadioButton.Size = new System.Drawing.Size(144, 24);
			this.PropertiesSingleAssetRadioButton.TabIndex = 1;
			this.PropertiesSingleAssetRadioButton.Text = "Simple Packaging";
			this.PropertiesSingleAssetRadioButton.CheckedChanged += new System.EventHandler(this.PropertiesSingleAssetRadioButton_CheckedChanged);
			// 
			// PropertiesMultiAssetRadioButton
			// 
			this.PropertiesMultiAssetRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PropertiesMultiAssetRadioButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.PropertiesMultiAssetRadioButton.Location = new System.Drawing.Point(24, 64);
			this.PropertiesMultiAssetRadioButton.Name = "PropertiesMultiAssetRadioButton";
			this.PropertiesMultiAssetRadioButton.Size = new System.Drawing.Size(144, 24);
			this.PropertiesMultiAssetRadioButton.TabIndex = 10;
			this.PropertiesMultiAssetRadioButton.Text = "TaskFile Packaging";
			this.PropertiesMultiAssetRadioButton.CheckedChanged += new System.EventHandler(this.PropertiesMultiAssetRadioButton_CheckedChanged);
			// 
			// PropertiesTabPage
			// 
			this.PropertiesTabPage.Controls.Add(this.PropertiesSimpleViewPanel);
			this.PropertiesTabPage.Controls.Add(this.PropertiesAdvancedButton);
			this.PropertiesTabPage.Controls.Add(this.PropertiesAdvancedPropertyGrid);
			this.PropertiesTabPage.Location = new System.Drawing.Point(4, 22);
			this.PropertiesTabPage.Name = "PropertiesTabPage";
			this.PropertiesTabPage.Size = new System.Drawing.Size(422, 532);
			this.PropertiesTabPage.TabIndex = 6;
			this.PropertiesTabPage.Text = "Properties";
			this.PropertiesTabPage.ToolTipText = "Full properties";
			// 
			// PropertiesSimpleViewPanel
			// 
			this.PropertiesSimpleViewPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesSimpleViewPanel.Controls.Add(this.PropertiesTreeViewToolbar);
			this.PropertiesSimpleViewPanel.Controls.Add(this.PropertiesUsersPropertiesTreeView);
			this.PropertiesSimpleViewPanel.Location = new System.Drawing.Point(0, 0);
			this.PropertiesSimpleViewPanel.Name = "PropertiesSimpleViewPanel";
			this.PropertiesSimpleViewPanel.Size = new System.Drawing.Size(420, 497);
			this.PropertiesSimpleViewPanel.TabIndex = 5;
			// 
			// PropertiesTreeViewToolbar
			// 
			this.PropertiesTreeViewToolbar.AutoSize = false;
			this.PropertiesTreeViewToolbar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.PropertiesTreeViewViewNonInheritedButton,
            this.PropertiesTreeViewViewAllButton});
			this.PropertiesTreeViewToolbar.DropDownArrows = true;
			this.PropertiesTreeViewToolbar.Enabled = false;
			this.PropertiesTreeViewToolbar.Location = new System.Drawing.Point(0, 0);
			this.PropertiesTreeViewToolbar.Name = "PropertiesTreeViewToolbar";
			this.PropertiesTreeViewToolbar.ShowToolTips = true;
			this.PropertiesTreeViewToolbar.Size = new System.Drawing.Size(420, 32);
			this.PropertiesTreeViewToolbar.TabIndex = 1;
			// 
			// PropertiesTreeViewViewNonInheritedButton
			// 
			this.PropertiesTreeViewViewNonInheritedButton.Name = "PropertiesTreeViewViewNonInheritedButton";
			this.PropertiesTreeViewViewNonInheritedButton.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			// 
			// PropertiesTreeViewViewAllButton
			// 
			this.PropertiesTreeViewViewAllButton.Name = "PropertiesTreeViewViewAllButton";
			this.PropertiesTreeViewViewAllButton.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			// 
			// PropertiesUsersPropertiesTreeView
			// 
			this.PropertiesUsersPropertiesTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesUsersPropertiesTreeView.ContextMenuStrip = this.AssetPropertiesNewContextMenu;
			this.PropertiesUsersPropertiesTreeView.HotTracking = true;
			this.PropertiesUsersPropertiesTreeView.Location = new System.Drawing.Point(0, 32);
			this.PropertiesUsersPropertiesTreeView.Name = "PropertiesUsersPropertiesTreeView";
			this.PropertiesUsersPropertiesTreeView.ShowNodeToolTips = true;
			this.PropertiesUsersPropertiesTreeView.Size = new System.Drawing.Size(420, 465);
			this.PropertiesUsersPropertiesTreeView.Sorted = true;
			this.PropertiesUsersPropertiesTreeView.TabIndex = 0;
			this.PropertiesUsersPropertiesTreeView.DoubleClick += new System.EventHandler(this.PropertiesUsersPropertiesTreeView_DoubleClick);
			// 
			// PropertiesAdvancedButton
			// 
			this.PropertiesAdvancedButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesAdvancedButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PropertiesAdvancedButton.Location = new System.Drawing.Point(329, 505);
			this.PropertiesAdvancedButton.Name = "PropertiesAdvancedButton";
			this.PropertiesAdvancedButton.Size = new System.Drawing.Size(88, 23);
			this.PropertiesAdvancedButton.TabIndex = 4;
			this.PropertiesAdvancedButton.Text = "Advanced>>>";
			this.PropertiesAdvancedButton.Click += new System.EventHandler(this.PropertiesAdvancedButton_Click);
			// 
			// PropertiesAdvancedPropertyGrid
			// 
			this.PropertiesAdvancedPropertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesAdvancedPropertyGrid.HelpVisible = false;
			this.PropertiesAdvancedPropertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.PropertiesAdvancedPropertyGrid.Location = new System.Drawing.Point(0, 0);
			this.PropertiesAdvancedPropertyGrid.Name = "PropertiesAdvancedPropertyGrid";
			this.PropertiesAdvancedPropertyGrid.Size = new System.Drawing.Size(422, 500);
			this.PropertiesAdvancedPropertyGrid.TabIndex = 3;
			this.PropertiesAdvancedPropertyGrid.ToolbarVisible = false;
			this.PropertiesAdvancedPropertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.PropertiesAdvancedPropertyGrid_PropertyValueChanged);
			// 
			// PropertiesLogTabPage
			// 
			this.PropertiesLogTabPage.Controls.Add(this.AssetLogsTabControl);
			this.PropertiesLogTabPage.Controls.Add(this.AssetPropertiesRipLogLabel);
			this.PropertiesLogTabPage.Location = new System.Drawing.Point(4, 22);
			this.PropertiesLogTabPage.Name = "PropertiesLogTabPage";
			this.PropertiesLogTabPage.Size = new System.Drawing.Size(422, 532);
			this.PropertiesLogTabPage.TabIndex = 2;
			this.PropertiesLogTabPage.Text = "Ripper Logs";
			this.PropertiesLogTabPage.ToolTipText = "Logs collected from asset rips";
			// 
			// AssetLogsTabControl
			// 
			this.AssetLogsTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.AssetLogsTabControl.Location = new System.Drawing.Point(8, 24);
			this.AssetLogsTabControl.Name = "AssetLogsTabControl";
			this.AssetLogsTabControl.SelectedIndex = 0;
			this.AssetLogsTabControl.Size = new System.Drawing.Size(406, 502);
			this.AssetLogsTabControl.TabIndex = 6;
			// 
			// PropertiesCommentsTabPage
			// 
			this.PropertiesCommentsTabPage.Controls.Add(this.label5);
			this.PropertiesCommentsTabPage.Controls.Add(this.PropertiesCommentsRichTextBox);
			this.PropertiesCommentsTabPage.Location = new System.Drawing.Point(4, 22);
			this.PropertiesCommentsTabPage.Name = "PropertiesCommentsTabPage";
			this.PropertiesCommentsTabPage.Size = new System.Drawing.Size(422, 532);
			this.PropertiesCommentsTabPage.TabIndex = 3;
			this.PropertiesCommentsTabPage.Text = "Comments";
			this.PropertiesCommentsTabPage.ToolTipText = "Bless logs for this asset";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(16, 8);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(100, 16);
			this.label5.TabIndex = 1;
			this.label5.Text = "Comments Log:";
			// 
			// PropertiesCommentsRichTextBox
			// 
			this.PropertiesCommentsRichTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesCommentsRichTextBox.Location = new System.Drawing.Point(8, 24);
			this.PropertiesCommentsRichTextBox.Name = "PropertiesCommentsRichTextBox";
			this.PropertiesCommentsRichTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.PropertiesCommentsRichTextBox.ReadOnly = true;
			this.PropertiesCommentsRichTextBox.Size = new System.Drawing.Size(406, 502);
			this.PropertiesCommentsRichTextBox.TabIndex = 0;
			this.PropertiesCommentsRichTextBox.Text = "";
			this.PropertiesCommentsRichTextBox.WordWrap = false;
			// 
			// UserPropertiesImageList
			// 
			this.UserPropertiesImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("UserPropertiesImageList.ImageStream")));
			this.UserPropertiesImageList.TransparentColor = System.Drawing.Color.Transparent;
			this.UserPropertiesImageList.Images.SetKeyName(0, "");
			this.UserPropertiesImageList.Images.SetKeyName(1, "");
			this.UserPropertiesImageList.Images.SetKeyName(2, "");
			this.UserPropertiesImageList.Images.SetKeyName(3, "");
			this.UserPropertiesImageList.Images.SetKeyName(4, "");
			this.UserPropertiesImageList.Images.SetKeyName(5, "");
			this.UserPropertiesImageList.Images.SetKeyName(6, "");
			this.UserPropertiesImageList.Images.SetKeyName(7, "");
			this.UserPropertiesImageList.Images.SetKeyName(8, "");
			this.UserPropertiesImageList.Images.SetKeyName(9, "");
			this.UserPropertiesImageList.Images.SetKeyName(10, "");
			this.UserPropertiesImageList.Images.SetKeyName(11, "");
			this.UserPropertiesImageList.Images.SetKeyName(12, "");
			this.UserPropertiesImageList.Images.SetKeyName(13, "");
			this.UserPropertiesImageList.Images.SetKeyName(14, "");
			this.UserPropertiesImageList.Images.SetKeyName(15, "");
			this.UserPropertiesImageList.Images.SetKeyName(16, "");
			this.UserPropertiesImageList.Images.SetKeyName(17, "");
			this.UserPropertiesImageList.Images.SetKeyName(18, "");
			this.UserPropertiesImageList.Images.SetKeyName(19, "");
			// 
			// PropertiesErrorProvider
			// 
			this.PropertiesErrorProvider.ContainerControl = this;
			this.PropertiesErrorProvider.Icon = ((System.Drawing.Icon)(resources.GetObject("PropertiesErrorProvider.Icon")));
			// 
			// AssetPropertyGridToolBar
			// 
			this.AssetPropertyGridToolBar.AutoSize = false;
			this.AssetPropertyGridToolBar.ButtonSize = new System.Drawing.Size(24, 24);
			this.AssetPropertyGridToolBar.DropDownArrows = true;
			this.AssetPropertyGridToolBar.ImageList = this.PropertiesImageList;
			this.AssetPropertyGridToolBar.Location = new System.Drawing.Point(0, 0);
			this.AssetPropertyGridToolBar.Name = "AssetPropertyGridToolBar";
			this.AssetPropertyGridToolBar.ShowToolTips = true;
			this.AssetPropertyGridToolBar.Size = new System.Drawing.Size(430, 26);
			this.AssetPropertyGridToolBar.TabIndex = 5;
			this.AssetPropertyGridToolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.AssetPropertyGridToolBar_ButtonClick);
			// 
			// ButtomButtonPanel
			// 
			this.ButtomButtonPanel.Controls.Add(this.AssetPropertyGridToolBar);
			this.ButtomButtonPanel.Controls.Add(this.MainButtonPanel);
			this.ButtomButtonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.ButtomButtonPanel.Location = new System.Drawing.Point(0, 558);
			this.ButtomButtonPanel.Name = "ButtomButtonPanel";
			this.ButtomButtonPanel.Size = new System.Drawing.Size(430, 57);
			this.ButtomButtonPanel.TabIndex = 8;
			// 
			// AssetPropertiesForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(430, 615);
			this.Controls.Add(this.PropertiesTabControl);
			this.Controls.Add(this.ButtomButtonPanel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(392, 488);
			this.Name = "AssetPropertiesForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "AssetPropertiesForm";
			this.Activated += new System.EventHandler(this.AssetPropertiesForm_Activated);
			this.Closing += new System.ComponentModel.CancelEventHandler(this.AssetPropertiesForm_Closing);
			this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.AssetPropertiesForm_KeyPress);
			this.Load += new System.EventHandler(this.AssetPropertiesForm_Load);
			this.AssetPropertiesNewContextMenu.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.AssetPropertiesIconPictureBox)).EndInit();
			this.MainButtonPanel.ResumeLayout(false);
			this.PropertiesTabControl.ResumeLayout(false);
			this.PropertiesGeneralTabPage.ResumeLayout(false);
			this.PropertiesGeneralTabPage.PerformLayout();
			this.DividerLinePanel1.ResumeLayout(false);
			this.panel7.ResumeLayout(false);
			this.PropertiesRippingTabPage.ResumeLayout(false);
			this.groupBox7.ResumeLayout(false);
			this.PropertiesAssetPackagingTabPage.ResumeLayout(false);
			this.PackagingAssetCommandsGroupBox.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.PropertiesPackageTabPage.ResumeLayout(false);
			this.PropertiesPackagingParametersGroupBox.ResumeLayout(false);
			this.PackageTypeGroupBox.ResumeLayout(false);
			this.PropertiesTabPage.ResumeLayout(false);
			this.PropertiesSimpleViewPanel.ResumeLayout(false);
			this.PropertiesLogTabPage.ResumeLayout(false);
			this.PropertiesCommentsTabPage.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.PropertiesErrorProvider)).EndInit();
			this.ButtomButtonPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region Initializing functions
		/// <summary>
		/// Initialize for one asset
		/// </summary>
		/// <param name="assetName"></param>
		public void Initialize(string assetName)
		{
			ArrayList assetNames = new ArrayList(1);
			assetNames.Add( assetName );
			Initialize( assetNames );
		} // end ()

		/// <summary>
		/// Initialize for multiple properties, all the values of each property
		///   should be the same for us to display them to the user.
		///   (i.e. wherever values conflict, we should be displaying blank fields)
		/// </summary>
		public void Initialize(ArrayList assetNames)
		{
			// Make sure our Properties settings are correct (override the Designer)

			this.PropertiesAdvancedPropertyGrid.HelpVisible = true;
			this.PropertiesAdvancedPropertyGrid.ToolbarVisible = true;

			// Make sure assetNames is valid
			if (assetNames == null || assetNames.Count < 1 )
			{
				return;
			}

			InitializeIcons();

			this.mAssetNames = assetNames;

			// Set the title
			if (assetNames.Count > 1)
			{
				this.Text = " " + assetNames.Count.ToString() + " Properties ";
				
				// Show whether we have one or more than one Assets
				this.Text += Multiple_Assets_Text;
			}
			else
			{
				MOG_Filename MOGassetName = new MOG_Filename(assetNames[0].ToString());

				if (MOGassetName.GetAssetLabel().Length > 0)
				{
					this.Text = MOGassetName.GetAssetLabel();
				}
				else
				{
					// This must be a classification
					this.Text = assetNames[0].ToString();

					// Disable the explore button for this type
					this.AssetPropertiesExploreButton.Enabled = false;
				}
			}
			
			// new SelectedObjects array for the property grid
			mPropertiesList = new object[assetNames.Count];
			PropertiesAssignedPackagesListView.Items.Clear();

			if (assetNames.Count > 1)
			{
				this.PropertiesTabControl.TabPages.Remove(this.PropertiesLogTabPage);
				this.PropertiesTabControl.TabPages.Remove(this.PropertiesCommentsTabPage);
			}

			// Reload sizes
			guiUserPrefs.LoadDynamic_LayoutPrefs("AssetProperties", this);
			InitializeAssets();

			if (assetNames.Count > 1)
			{
				// Make sure we set this new multi mode after we have loaded our user prefs
				ViewPropertiesInSimpleMode = false;
				
				// We cannot view the properties in simple view with multiple assets selected
				this.PropertiesAdvancedButton.Enabled = false;
			}

			InitializeVersionInfo();

			if(this.bFormContainsBlessedAsset)
			{
				DisablePropertiesAdvancedPropertyGrid();
			}

			this.PropertiesAdvancedPropertyGrid.SelectedObjects = mPropertiesList;

			// Check if we need to initialize the comments tab?
			if (PropertiesTabControl.SelectedTab == PropertiesCommentsTabPage)
			{
				InitializeCommentsTabPage();
			}
		}

		private void InitializeAssets()
		{
			InitializeToolBarForAllAssets();

			for (int assetIndex = 0; assetIndex < AssetNames.Count; assetIndex++)
			{
				string assetName = AssetNames[assetIndex] as string;
				MOG_Filename currentFilename = new MOG_Filename(assetName);

				if (currentFilename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
				{
					this.bFormContainsBlessedAsset |= currentFilename.IsBlessed() || !currentFilename.IsWithinInboxes();
				}

				IntializeGeneralTabPage(currentFilename, AssetNames);

				if (AssetNames.Count <= 1)
				{
					InitializeLogsTabPage(null, currentFilename);
				}

				InitializePropertiesPages(mPropertiesList, currentFilename, "All", assetIndex);
			}
		}

		/// <summary>
		/// Used throughout AssetPropertiesForm to make sure we have the correct values in each of our fields.
		/// </summary>
		/// <param name="platform"></param>
		private void ReInitialize(string platform)
		{
			ReInitializePages(platform, new ReInitializeMethod[]
				{
					new ReInitializeMethod(InitializeUserPropertiesTreeView),
					new ReInitializeMethod(InitializeRipperInfo),
					new ReInitializeMethod(InitializePackagingInfo),
					new ReInitializeMethod(InitializePackageInfo)
				});
		} // end ()

		/// <summary>
		/// Delegate for our methods used in re-initialization of a given page or set of pages.  
		///   Valid methods (2006 Apr 17) are:
		///   InitializeUserPropertiesTreeView
		///   InitializeRipperInfo
		///   InitializePackagingInfo
		///   InitializePackageInfo
		///   
		/// </InitializeRipperInfo),summary>
		delegate void ReInitializeMethod(MOG_Properties properties, MOG_Filename currentFilename);
		
		private void ReInitializePage(string platform, ReInitializeMethod initializer)
		{
			ReInitializePages(platform, new ReInitializeMethod[]{initializer});
		}

		private void ReInitializePages(string platform, ReInitializeMethod[] initializers)
		{
			// Set the platform if it is not specified to the last known platform set for this form
			if ((platform != null && platform.Length > 0) == false)
			{
				platform = mCurrentPlatform;
			}

			// Reset our package list
			if(PropertiesAssignedPackagesListView.Items.Count > 0)
			{
				PropertiesAssignedPackagesListView.Items.Clear();
			}

			this.SinglePackageControl.mResetWildcardCheck = true;
			this.MultiPackageControl.mResetWildcardCheck = true;
			this.RippersControl.mResetWildcardCheck = true;
			this.mResetWildcardCheck = true;
			
			foreach (string assetName in mAssetNames)
			{
				try
				{
					this.ReInitializePage_Helper( platform, new MOG_Filename(assetName), initializers);
				}
				catch( Exception ex )
				{
					ex.ToString();
				}
			}
		}

		private const int IMG_SECTION		= 0;
		private const int IMG_PROPERTI		= 1;
		private const int IMG_KEY			= 2;
		private const int IMG_VALUE			= 3;
		private const int IMG_PROPERTIES	= 4;
		private const int IMG_INFO			= 5;
		private const int IMG_MOGPROPERTY	= 6;
		private const int IMG_IMPORTED		= 7;
		private const int IMG_PROCESSED		= 8;
		private const int IMG_RELATION		= 9;
		private const int IMG_ASSET			= 10;

		private const int IMG_ASSETINFO			= 11;
		private const int IMG_ASSETSTATS		= 12;
		private const int IMG_CLASSINFO			= 13;
		private const int IMG_BUILDOPTIONS		= 14;
		private const int IMG_SYNCOPTIONS		= 15;
		private const int IMG_PACKAGEOPTIONS	= 16;
		private const int IMG_PACKAGECOMMANDS	= 17;

		private const int IMG_PROPERTY			= 18;
		private const int IMG_PACKAGE			= 19;		

		private void InitializeIcons()
		{
			MogUtil_AssetIcons.AddCustomIcons(UserPropertiesImageList, IMG_SECTION,		"IMG_SECTION");
			MogUtil_AssetIcons.AddCustomIcons(UserPropertiesImageList, IMG_PROPERTI,	"IMG_PROPERTI");
			MogUtil_AssetIcons.AddCustomIcons(UserPropertiesImageList, IMG_KEY,			"IMG_KEY");
			MogUtil_AssetIcons.AddCustomIcons(UserPropertiesImageList, IMG_VALUE,		"IMG_VALUE");
			MogUtil_AssetIcons.AddCustomIcons(UserPropertiesImageList, IMG_PROPERTIES,	"IMG_PROPERTIES");
			MogUtil_AssetIcons.AddCustomIcons(UserPropertiesImageList, IMG_INFO,		"IMG_INFO");
			MogUtil_AssetIcons.AddCustomIcons(UserPropertiesImageList, IMG_MOGPROPERTY, "IMG_MOGPROPERTY");
			MogUtil_AssetIcons.AddCustomIcons(UserPropertiesImageList, IMG_IMPORTED,	"IMG_IMPORTED");
			MogUtil_AssetIcons.AddCustomIcons(UserPropertiesImageList, IMG_PROCESSED,	"IMG_PROCESSED");
			MogUtil_AssetIcons.AddCustomIcons(UserPropertiesImageList, IMG_RELATION,	"IMG_RELATION");
			MogUtil_AssetIcons.AddCustomIcons(UserPropertiesImageList, IMG_ASSET,		"IMG_ASSET");

			MogUtil_AssetIcons.AddCustomIcons(UserPropertiesImageList, IMG_ASSETINFO,		"IMG_ASSETINFO");
			MogUtil_AssetIcons.AddCustomIcons(UserPropertiesImageList, IMG_ASSETSTATS,		"IMG_ASSETSTATS");
			MogUtil_AssetIcons.AddCustomIcons(UserPropertiesImageList, IMG_CLASSINFO,		"IMG_CLASSINFO");
			MogUtil_AssetIcons.AddCustomIcons(UserPropertiesImageList, IMG_BUILDOPTIONS,	"IMG_BUILDOPTIONS");
			MogUtil_AssetIcons.AddCustomIcons(UserPropertiesImageList, IMG_SYNCOPTIONS,		"IMG_SYNCOPTIONS");
			MogUtil_AssetIcons.AddCustomIcons(UserPropertiesImageList, IMG_PACKAGEOPTIONS,	"IMG_PACKAGEOPTIONS");
			MogUtil_AssetIcons.AddCustomIcons(UserPropertiesImageList, IMG_PACKAGECOMMANDS,	"IMG_PACKAGECOMMANDS");
			MogUtil_AssetIcons.AddCustomIcons(UserPropertiesImageList, IMG_PROPERTY,		"IMG_PROPERTY");
			MogUtil_AssetIcons.AddCustomIcons(UserPropertiesImageList, IMG_PACKAGE,			"IMG_PACKAGE");
			this.PropertiesUsersPropertiesTreeView.ImageList = MogUtil_AssetIcons.Images;
		}

		/// <summary>
		/// Show our user that we found no Rip Logs.
		/// </summary>
		private void InitializeLogsTabPageToNothing()
		{
			TabPage noRipLogsFound = new TabPage( "No Rip Log(s) found.");

			RichTextBox logOutput = new RichTextBox();
			logOutput.Dock = DockStyle.Fill;

			logOutput.Text =
				"\n\n" +
				"--------------------------------------------------\n" + 
				"NO LOGS FOUND.\n" + 
				"--------------------------------------------------\n";

			noRipLogsFound.Controls.Add(logOutput);


			this.AssetLogsTabControl.Controls.Add(noRipLogsFound);
		}

		private void InitializeLogsTabPage(MOG_Properties properties, MOG_Filename currentFilename)
		{
			// Open the logs
			if (Directory.Exists(currentFilename.GetEncodedFilename()))
			{
				FileInfo []files = DosUtils.FileGetList(currentFilename.GetEncodedFilename(), "*.log");
				if ( files != null)
				{
					// Reset the TabView
					this.AssetLogsTabControl.TabPages.Clear();

					foreach (FileInfo file in files)
					{
						// Do not add the comments log to the rippers logs tab page
						if (string.Compare(file.Name, "Comments.log", true) != 0)
						{
							TabPage riplog = new TabPage(file.Name);
							RichTextBox logOutput = new RichTextBox();
							logOutput.Dock = DockStyle.Fill;

							logOutput.Text = file.OpenText().ReadToEnd();

							riplog.Controls.Add(logOutput);

							this.AssetLogsTabControl.SuspendLayout();
							this.AssetLogsTabControl.Controls.Add(riplog);						
							this.AssetLogsTabControl.ResumeLayout();
						}
					}
				}
			}

			// If we have no logs, remove this tab
			if (AssetLogsTabControl.TabPages.Count == 0)
			{
				this.PropertiesTabControl.TabPages.Remove(this.PropertiesLogTabPage);
			}
		}

		private void InitializeCommentsTabPage()
		{
			PropertiesCommentsRichTextBox.Text = "";

			// Check if we have only one asset?
			MOG_Filename asset = null;
			if (AssetNames.Count == 1)
			{
				asset = new MOG_Filename(AssetNames[0] as string);
			}

			// Check if we have an asset to show comment for?
			if (asset != null &&
				asset.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
			{
				// Check if this asset is in the inbox?
				if (asset.IsWithinInboxes())
				{
					// Check if this asset has a comments file?
					string commentFilename = MOG_ControllerAsset.GetAssetCommentsFilename(asset);
					if (DosUtils.FileExistFast(commentFilename))
					{
						FileInfo commentFileInfo = new FileInfo(commentFilename);
						PropertiesCommentsRichTextBox.Text += commentFileInfo.OpenText().ReadToEnd();
					}
				}

				// Obtain the comment history for this asset
				ArrayList revisions = new ArrayList();

// This was a good idea but assets in the project's trees always have a revision so it never shows a full history
//				// Get the revisions of this asset that we want to show a comment for
//				if (asset.GetVersionTimeStamp().Length > 0)
//				{
//					revisions.Add(asset.GetVersionTimeStamp());
//				}
//				else
//				{
					revisions = MOG_DBAssetAPI.GetAllAssetRevisions(asset);
//				}

				// Gather all of the comments for these revisions
				foreach (string revision in revisions)
				{
					// Get the comment log for this revision
					MOG_Filename blessedAssetFilename = MOG_ControllerRepository.GetAssetBlessedVersionPath(asset, revision);
					string commentFilename = MOG_ControllerAsset.GetAssetCommentsFilename(blessedAssetFilename);

					// Add the comments log if it exists
					FileInfo commentFileInfo = new FileInfo(commentFilename);
					if (commentFileInfo.Exists)
					{
						PropertiesCommentsRichTextBox.Text += commentFileInfo.OpenText().ReadToEnd();
					}
				}
			}
			else
			{
				// If this ain't an asset, remove this tab
				this.PropertiesTabControl.TabPages.Remove(this.PropertiesCommentsTabPage);
			}
		}

		private void IntializeGeneralTabPage(MOG_Filename currentFilename, ArrayList assetNames)
		{
			PropertiesNameTextBox.Text = assetNames.Count > 1 ? assetNames.Count.ToString() + " Properties" : "Properties for " + assetNames[0].ToString();

			// Get the asset image
			if (assetNames.Count <= 1)
			{
				AssetPropertiesIconPictureBox.Image = MogUtil_AssetIcons.GetAssetIconImage( currentFilename.GetEncodedFilename() );
			}
				
			// Update asset version
			if (currentFilename.GetVersionTimeStamp().Length != 0)
			{
				PropertiesVersionLabel.Text = assetNames.Count > 1 ? Multiple_Assets_Text : MogUtils_StringVersion.VersionToString(currentFilename.GetVersionTimeStamp());
			}
			else
			{
				// Check to see if this asset was blessed before?
				string blessedVersion = MOG_DBAssetAPI.GetAssetVersion(currentFilename);
				if (blessedVersion.Length == 0)
				{
					PropertiesVersionLabel.Text = assetNames.Count > 1 ? Multiple_Assets_Text : "Local";
				}
				else
				{
					// Display its version
					PropertiesVersionLabel.Text = assetNames.Count > 1 ? Multiple_Assets_Text : MogUtils_StringVersion.VersionToString(blessedVersion);
				}
			}

			string assetName = currentFilename.GetEncodedFilename();

			// Update lock information
			if (MOG_ControllerProject.IsLocked(assetName))
			{
				if (guiAssetSourceLock.lockHolder != null)
				{
					MOG_Time time = new MOG_Time(guiAssetSourceLock.lockHolder.GetCommandTimeStamp());
					LockInfoLabel.Text += assetName + "(" + time.FormatString("") + ")";
					if (assetNames.Count > 1)
					{
						LockInfoLabel.Text += "\r\n";
					}
				}
			}

			// Update location
			PropertiesLocationLabel.Text += currentFilename.GetPath();
			if (assetNames.Count > 1)
			{
				PropertiesLocationLabel.Text += "\r\n";
			}

			// Update size
			PropertiesSizeLabel.Text = assetNames.Count > 1 ? Multiple_Assets_Text : guiAssetController.GetAssetSize(currentFilename.GetEncodedFilename());
		}

		private void InitializePlatformIcons( string[] platforms )
		{
			// Check that we have an empty ImageList...
			if( this.mPlatformImageList != null && this.mPlatformImageList.Images.Count > 0 )
			{
				this.mPlatformImageList.Images.Clear();
			}
			else
			{
				this.mPlatformImageList = new ImageList();
			}

			// Check that we have an empty TstDictionary...
			if( this.mPlatforms != null && this.mPlatforms.Count > 0 )
			{
				this.mPlatforms.Clear();
			}
			else
			{
				this.mPlatforms = new TstDictionary();
			}

			// Add each platform to our images
			foreach( string platform in platforms )
			{
				string iconFilename = MOG_ControllerSystem.LocateTool("Images\\Platforms", platform + ".bmp");
				if (DosUtils.FileExist(iconFilename))
				{
					// Get the image
					Image myImage = new Bitmap(iconFilename);
					
					// Add the image and the type to the arrayLists
					this.mPlatformImageList.Images.Add(myImage);

					this.mPlatforms.Add( platform.ToLower(), mPlatformImageList.Images.Count - 1 );
				}
			}

			// Add the .All Icon
			Image myAllImage = new Bitmap(this.PropertiesImageList.Images[0]);
			this.mPlatformImageList.Images.Add(myAllImage);
			this.mPlatforms.Add( "all", mPlatformImageList.Images.Count - 1 );
		}


		private void InitializeVersionInfo()
		{
			if( AssetNames.Count == 1 )
			{
				MOG_Filename currentFilename = new MOG_Filename(AssetNames[0].ToString());

				// Update Version
				if (currentFilename.IsBlessed())
				{
					// Check if this asset has a version number?
					if (currentFilename.GetVersionTimeStamp().Length == 0)
					{
						// Get the latest version from the database
						currentFilename.SetVersionTimeStamp(MOG_DBAssetAPI.GetAssetVersion(currentFilename));
					}

					PropertiesLastBlessedLabel.Text += currentFilename.GetVersionTimeStampString("") + "   ";

					// Get the last person to bless this asset
					PropertiesCreatedLabel.Text = MOG_DBAssetAPI.GetAssetVersionProperty(currentFilename, currentFilename.GetVersionTimeStamp(), "Properties", "{Asset Stats}Creator");
				}
				else
				{
					PropertiesLastBlessedLabel.Text = "UnBlessed";
					// Get the last person to bless this asset
					PropertiesCreatedLabel.Text = "None";
				}
			}
			else
			{
				PropertiesLastBlessedLabel.Text = Multiple_Assets_Text;
				// Get the last person to bless this asset
				PropertiesCreatedLabel.Text = Multiple_Assets_Text;
			}
		}

		private void InitializeToolBarForAllAssets()
		{
			// Get our platforms
			string[] platforms = MOG_ControllerProject.GetProject().GetPlatformNames();
			// Add "All" to our platforms
			string[] platformsPlusAll = new string[platforms.Length+1];
			for( int i = 0; i < platformsPlusAll.Length; ++i )
			{
				if( i == 0 )
				{
					platformsPlusAll[i] = "All";
				}
					// else (i > 0) == true
				else
				{
					platformsPlusAll[i] = platforms[i-1];
				}
			}

			InitializePlatformIcons( platformsPlusAll );

			this.AssetPropertyGridToolBar.ImageList = this.mPlatformImageList;

			// Foreach platform (other than all)
			foreach( string platform in platformsPlusAll )
			{
				ToolBarButton platformButton = new ToolBarButton();
				platformButton.Style = ToolBarButtonStyle.ToggleButton;
				platformButton.ToolTipText =  "Click here to change properties specific to " + platform;
				platformButton.Tag = platform;
				if( this.mPlatforms.Contains( platform.ToLower() ) )
				{
					platformButton.ImageIndex = (int)this.mPlatforms.Find( platform.ToLower() ).Value;
				}
				else
				{
					platformButton.ImageIndex = 0;
				}
				this.AssetPropertyGridToolBar.Buttons.Add( platformButton );
			}

			// Set us to All by default
			this.AssetPropertyGridToolBar.Buttons[0].Pushed = true;
		}

		/// <summary>
		/// Used only once on Initialization, this method makes it so we can disable
		/// </summary>
		private void DisablePropertiesAdvancedPropertyGrid()
		{
			// If we want to disable this control...
			foreach (object test in mPropertiesList)
			{
				MOG_Properties props = test as MOG_Properties;
				if (props != null)
				{
					props.DisableModification();
				}
			}
		}

		/// <summary>
		/// Only used for the Initialize() method to create an initial state for all Properties Pages...
		/// </summary>
		private void InitializePropertiesPages( object[] propertiesList, MOG_Filename currentFilename, string platform, int assetIndex )
		{
			// Check if this is an asset?
			if (currentFilename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
			{
				// Open the actual properties file 
				MOG_Properties properties = MOG_Properties.OpenFileProperties(MOG_ControllerAsset.GetAssetPropertiesFilename(currentFilename));
				if (properties != null)
				{
					//this is an Asset
					if (currentFilename.IsBlessed() || !currentFilename.IsWithinInboxes())
					{
						//get a read-only properties
						this.SinglePackageControl.Enabled = false;
						this.MultiPackageControl.Enabled = false;
						this.RippersControl.Enabled = false;
						this.PropertiesUsersPropertiesTreeView.Enabled = false;
					}

					// Set the platform scope
					properties.SetScope(platform);
					if (platform.ToLower() != ("All").ToLower())
					{
						properties.SetScopeExplicitMode(true);
					}
					else
					{
						properties.SetScopeExplicitMode(false);
					}

					propertiesList[assetIndex] = properties;

					// Set the Simple package properties control to have a reference of our loaded properties
					this.SinglePackageControl.PropertiesList = propertiesList;
					this.MultiPackageControl.PropertiesList = propertiesList;
					this.RippersControl.PropertiesList = propertiesList;

					this.MultiPackageControl.mParent = this;
					this.SinglePackageControl.mParent = this;
					this.RippersControl.mParent = this;

					InitializeUserPropertiesTreeView(properties);
					InitializeRipperInfo(properties, currentFilename);
					InitializePackagingInfo(properties, currentFilename);
					InitializePackageInfo(properties, currentFilename);
				}
				else
				{
					// Failed to open the properties
				}
			}
			else
			{
				//we should be looking at a classification
				MOG_Properties properties = MOG_Properties.OpenClassificationProperties(currentFilename.GetOriginalFilename());
				if (properties != null)
				{
					propertiesList[assetIndex] = properties;

					// Set the Simple package properties control to have a reference of our loaded properties
					this.SinglePackageControl.PropertiesList = propertiesList;
					this.MultiPackageControl.PropertiesList = propertiesList;
					this.RippersControl.PropertiesList = propertiesList;

					this.MultiPackageControl.mParent = this;
					this.SinglePackageControl.mParent = this;
					this.RippersControl.mParent = this;

					InitializeUserPropertiesTreeView(properties);
					InitializeRipperInfo(properties, currentFilename);
					InitializePackagingInfo(properties, currentFilename);
					InitializePackageInfo(properties, currentFilename);
				}
				else
				{
					// Failed to open the properties
				}
			}
		}

		private void ReInitializePage_Helper(string platform, MOG_Filename currentFilename, ReInitializeMethod[] initializers)
		{
			foreach (MOG_Properties properties in mPropertiesList)
			{
				// Set the platform scope
				properties.SetScope( platform );
				if( platform.ToLower() != ("All").ToLower() )
				{
					properties.SetScopeExplicitMode(true);					
				}
				else
				{
					properties.SetScopeExplicitMode(false);
				}

				foreach(ReInitializeMethod initializer in initializers)
				{
					initializer(properties, currentFilename);
				}
			}
		}

		private ArrayList GetDisplayableProperties(ArrayList properties, PropertyDescriptorCollection descriptors)
		{
			ArrayList returnProperties = new ArrayList();
			foreach(MOG_Property property in properties)
			{
				if(PropertyIsVisible(property, descriptors))
				{
					returnProperties.Add(property);
				}
			}
			return returnProperties;
		}

		private bool PropertyIsVisible(MOG_Property property, PropertyDescriptorCollection descriptors)
		{
			foreach(PropertyDescriptor descriptor in descriptors)
			{
				// If we found a match in our displayable properties, display it...
				if(property.mPropertyKey.ToLower() == descriptor.DisplayName.ToLower())
				{
					return true;
				}
			}
			return false;
		}

		private void InitializeUserPropertiesTreeView(MOG_Properties properties, MOG_Filename thisMakesTheMethodAReInitializeMethod)
		{
			InitializeUserPropertiesTreeView(properties);
		}

		private void InitializeUserPropertiesTreeView(MOG_Properties properties)
		{
			try
			{
				// Only bother to do this work if we are actually viewing the simple property tree view
				// If we have more than one property selected, ignore this code (this is a patch--glk)
// This would be a better way to check this but for some reason, I can't set Visible to true when the form is getting created???
//				if (this.PropertiesSimpleViewPanel.Visible &&
// Use this gross hard-coded hack to check the state of this button because this does get updated correctly
				if (PropertiesAdvancedButton.Text == "Advanced>>>" &&
					this.mPropertiesList.Length == 1)
				{
					PropertiesUsersPropertiesTreeView.Nodes.Clear();
					ArrayList assetProperties = properties.GetApplicableProperties();
					PropertyDescriptorCollection thisPropertyDescriptionCollection = TypeDescriptor.GetProperties(properties);

					foreach (MOG_Property assetProperty in assetProperties)
					{
						string section = assetProperty.mSection;
						string property = "";
						string key = assetProperty.mKey;
						string val = assetProperty.mValue;
						string inheritedString = "";

						if (properties.GetNonInheritedPropertiesIni().IsProperty(assetProperty.mKey))
						{
							property = assetProperty.mPropertySection;
							key = assetProperty.mPropertyKey;
							val = assetProperty.mPropertyValue;
						}
	
						// Check if this property is in the standard properties section?
						if (string.Compare("Properties", section, true) == 0)
						{
							// Check for the browsable attribute to make sure it is okay to show this thing
							PropertyDescriptor thisDescriptor = thisPropertyDescriptionCollection[key];
							if (thisDescriptor != null)
							{
								AttributeCollection attributes = thisDescriptor.Attributes;
								if (attributes != null)
								{
									BrowsableAttribute browse = (BrowsableAttribute)attributes[typeof(BrowsableAttribute)];
									if (browse != null)
									{
										// Check if this is browsable?
										if (browse.Browsable == false)
										{
											// This non-browsable property should not be added to the tree
											continue;
										}
									}
								}
							}
						}

						// Find out what classifications is responsible for this inherited property
						inheritedString = properties.GetInheritedPropertyClassification(assetProperty);
						this.CreateNewTreeNode(this.PropertiesUsersPropertiesTreeView, section, property, key, val, inheritedString);
					}
				}
			}
			catch (Exception e)
			{
				e.ToString();
			}
		}

		private void InitializePackagingInfo(MOG_Properties properties, MOG_Filename currentFilename)
		{
			mDisableEvents = true;
			
			// Add the current package assignments
			//if (properties.IsPackagedAsset)
			{
				foreach (MOG_Property package in properties.GetPackages())
				{
					ListViewItem item = new ListViewItem();
					
					// Add our asset name
					item.Text = currentFilename.GetAssetLabel();

					// Add the package we are assigned to
					MOG_Filename packageFile = new MOG_Filename(MOG_ControllerPackage.GetPackageName(package.mPropertyKey));
					item.SubItems.Add(packageFile.GetAssetLabel());

					// Add the group we are assigned to within that package
					item.SubItems.Add(MOG_ControllerPackage.GetPackageGroups(package.mPropertyKey));

					// Add full filename
					item.SubItems.Add(MOG_ControllerRepository.GetRepositoryPath() + "\\" + packageFile.GetNotSureDefaultEncodedFilename());

					item.ImageIndex = MogUtil_AssetIcons.GetAssetIconIndex(packageFile.GetNotSureDefaultEncodedFilename());

					this.PropertiesAssignedPackagesListView.Items.Add(item);
				}

				PropertiesAssignedPackagesListView.Enabled = true;

				// Set the asset names for the token helper windows
                this.PackagingAddMogControl_ViewTokenTextBox.MOGAssetFilename = currentFilename;
                this.PackagingRemoveMogControl_ViewTokenTextBox.MOGAssetFilename = currentFilename;
				this.mogAddArgumentsButton.MOGAssetFilename = currentFilename;
                this.mogRemoveArgumentsButton.MOGAssetFilename = currentFilename;
				
				// Load the add and remove commands
				SetComboBoxProperty(this.PropertiesPackagingAddComboBox, properties.PackageCommand_Add, false);
				SetComboBoxProperty(this.PropertiesPackagingRemoveComboBox, properties.PackageCommand_Remove, false);
				mResetWildcardCheck = false;
				PackagingAssetCommandsGroupBox.Enabled = true;
			}
//			else
//			{
//				PropertiesAssignedPackagesListView.Enabled = false;
//				PackagingAssetCommandsGroupBox.Enabled = false;
//			}
			mDisableEvents = false;
		}

		private void InitializePackageInfo(MOG_Properties properties, MOG_Filename currentFilename)
		{
			mDisableEvents = true;
			// Determine if we are single or multi asset
			switch(properties.PackageStyle)
			{
				case MOG_PackageStyle.Disabled:
					this.PropertiesNoPackagingRadioButton.Checked = true;
					break;
				case MOG_PackageStyle.Simple:
					this.PropertiesSingleAssetRadioButton.Checked = true;
					break;
				case MOG_PackageStyle.TaskFile:
					this.PropertiesMultiAssetRadioButton.Checked = true;
					break;
			}			
			// Put in the single asset commands
			this.SinglePackageControl.InitializePackagingInfo(properties, currentFilename);

			// Put in the Multi-asset commands
			this.MultiPackageControl.InitializePackagingInfo(properties, currentFilename);
			
			mDisableEvents = false;
		}

		private void InitializeRipperInfo(MOG_Properties properties)
		{
			this.InitializeRipperInfo(properties, null);
		}

		private void InitializeRipperInfo(MOG_Properties properties, MOG_Filename currentFilename)
		{
			mDisableEvents = true;

			this.RippersControl.InitializeRipperInfo(properties, currentFilename);
            
			// If we have a PropertiesRippingLastStateEnabled variable that has a value...
			if(this.PropertiesRippingLastStateEnabled != null)
			{
				// If the value was true, set Enable Rip to true...
				if((Boolean)this.PropertiesRippingLastStateEnabled)
				{
					this.PropertiesRippingEnableRipRadioButton.Checked = true;
				}
				// Else, set No Rip to true...
				else
				{
					this.PropertiesRippingNoRipRadioButton.Checked = true;
				}
			}
			// Else, we set our RadioButtons to what our properties.NativeDataType indicates they should be
			else
			{
				this.PropertiesRippingNoRipRadioButton.Checked = properties.NativeDataType;
				this.PropertiesRippingEnableRipRadioButton.Checked = !properties.NativeDataType;
			}

			mDisableEvents = false;
		}
		#endregion
		#region Saving Properties Functions
		private void SavePropertiesChanges(bool close)
		{
			ProgressDialog progress = new ProgressDialog("Updating Properties...", "Saving and closing properties.", SavePropertiesChanges_Worker, close, true);
			progress.ShowDialog();
		}

		private void SavePropertiesChanges_Worker(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			bool close = (bool)e.Argument;

			string couldNotSaveStr = "";

			// Go through each properties and decide whether or not we can save
			for (int i = 0; i < mPropertiesList.Length && !worker.CancellationPending; i++)
			{
				MOG_Properties properties = mPropertiesList[i] as MOG_Properties;

				// Save any changes made to the current properties tab
				switch (PropertiesTabControl.SelectedTab.Name)
				{
				case "PropertiesRippingTabPage":
					this.SaveRippingChanges(properties);
					break;
				case "PropertiesAssetPackagingTabPage":
					this.SavePackagingChanges(properties);
					break;
				case "PropertiesPackageTabPage":
					this.SavePackageChanges(properties);
					break;
				}

				MOG_Filename assetFilename = properties.GetAssetFilename();
				string filename = properties.GetNonInheritedPropertiesIni().GetFilename();

				string message = "Saving:\n" +
								 "     " + assetFilename.GetAssetClassification() + "\n" +
								 "     " + assetFilename.GetAssetName();
				worker.ReportProgress(i * 100 / mPropertiesList.Length, message);

				// If this file exists...
				if (filename.Length > 0 && DosUtils.FileExist(filename))
				{
					// Indicate this property was changed
					properties.ModifiedTime = MOG_Time.GetVersionTimestamp();

					// Determin our desired finalStatus
					MOG_AssetStatusType originalStatus = MOG_AssetStatus.GetType(properties.Status);
					MOG_AssetStatusType finalStatus = originalStatus;
					// Update the inbox views
					if (!properties.NativeDataType)
					{
						finalStatus = MOG_AssetStatusType.Modified;
					}

					if (close)
					{
						properties.Close();
					}
					else
					{
						properties.Save();
					}

					// Check if the finalStatus is defferent than the originalStatus?
					if (finalStatus != originalStatus)
					{
						// Open the asset's controller
						MOG_ControllerAsset asset = MOG_ControllerAsset.OpenAsset(assetFilename);
						if (asset != null)
						{
							// Stamp the new status into the asset
							MOG_ControllerInbox.UpdateInboxView(asset, finalStatus);
							asset.Close();
						}
					}
// Do we really want to cause this to happen?  I need to consult with Kier on this issue...
//					else
//					{
//						// Send out a fake view update for triggering any inbox events because we just modified this asset's properties
//						MOG_ControllerInbox.UpdateInboxView(finalStatus, assetFilename, assetFilename);
//					}
				}
				// Else if this is a classification...
				else if (filename.Length < 1)
				{
					if (close)
					{
						properties.Close();
					}
					else
					{
						properties.Save();
					}
				}
				else
				{
					couldNotSaveStr += filename;
				}
			}

			// If we were unable to save a properties, let the user know
			if (couldNotSaveStr.Length > 0)
			{
				MOG_Prompt.PromptMessage("Save Properties", "The following Asset(s) were moved or changed:\r\n\r\n"
					+ couldNotSaveStr + "\r\n\r\nMOG was unable to save them.");

			}
			else
			{
				AssetPropertiesApplyButton.Enabled = false;
			}
		}

		private void SavePackagingChanges(MOG_Properties properties)
		{
			if (properties == null)
			{
				SavePackagingChanges();
			}
			else
			{
				if (this.PropertiesNoPackagingRadioButton.Checked)
				{
					properties.PackageStyle = MOG_PackageStyle.Disabled;
				}
				else if (this.PropertiesSingleAssetRadioButton.Checked)
				{
					properties.PackageStyle = MOG_PackageStyle.Simple;
					this.SinglePackageControl.SavePackagingChanges(properties);
				}
				else if (this.PropertiesMultiAssetRadioButton.Checked)
				{
					properties.PackageStyle = MOG_PackageStyle.TaskFile;
					this.MultiPackageControl.SavePackagingChanges(properties);
				}
		
				// Save the add and remove commands
				properties.PackageCommand_Add = this.PropertiesPackagingAddComboBox.Text;
				properties.PackageCommand_Remove = this.PropertiesPackagingRemoveComboBox.Text;
			}
		}

		private void SavePackageChanges(MOG_Properties properties)
		{
			// Temp handler!
			if (properties == null)
			{
				properties = this.PropertiesAdvancedPropertyGrid.SelectedObject as MOG_Properties;
			}

			if (this.PropertiesNoPackagingRadioButton.Checked)
			{
				properties.PackageStyle = MOG_PackageStyle.Disabled;
			}
			else if (this.PropertiesSingleAssetRadioButton.Checked)
			{
				properties.PackageStyle = MOG_PackageStyle.Simple;
				this.SinglePackageControl.SavePackagingChanges(properties);
			}
			else if (this.PropertiesMultiAssetRadioButton.Checked)
			{
				properties.PackageStyle = MOG_PackageStyle.TaskFile;
				this.MultiPackageControl.SavePackagingChanges(properties);
			}
		}

		/// <summary>
		/// Save Packaging Changes for everything in our mPropertiesList
		/// </summary>
		private void SavePackagingChanges()
		{
			// Go through each MOG_Properties object...
			foreach(MOG_Properties properties in this.mPropertiesList)
			{
				// So long as our properties is NOT NULL -- critical (stack overflow if it is)
				if(properties != null)
				{
					SavePackagingChanges(properties);
				}
			}
		}

		private void SaveRippingChanges(MOG_Properties properties)
		{
			if (properties == null)
			{
				SaveRippingChanges();
			}
			else
			{
				if (this.PropertiesRippingNoRipRadioButton.Checked)
				{
					properties.AssetRipper = "None";
					properties.AssetRipTasker = "None";
				}
								
				this.RippersControl.SaveRipperChanges(properties);
			}			
		}
		
		/// <summary>
		/// Save Ripping Changes for all properties in our mPropertiesList
		/// </summary>
		private void SaveRippingChanges()
		{
			// Go through each properties, saving the ripping values
			foreach(MOG_Properties properties in this.mPropertiesList)
			{
				// So long as properties is not null, we won't get a stack overflow...
				if(properties != null)
				{
					SaveRippingChanges(properties);
				}

			}
		}
		
		#endregion
		#region Properties tab functions
		private void ChangePropertyGridPlatform( string platform )
		{
			// If we have a properties object selected...
			if(this.mPropertiesList != null)
			{
				// Set our scope foreach properties object
				foreach(MOG_Properties properties in this.mPropertiesList)
				{
					properties.SetScope( platform );
					if( platform.ToLower() != ("All").ToLower() )
					{
						properties.SetScopeExplicitMode(true);					
					}
					else
					{
						properties.SetScopeExplicitMode(false);
					}
					InitializeUserPropertiesTreeView(properties);
					InitializeRipperInfo(properties);
				}
			}
		}

		/// <summary>
		/// Indicates that one of our MOG_Properties objects have been modified.  Should only be called
		///  if we know for sure that the object *has* been modified.
		/// </summary>
		private void PropertiesModified()
		{
			if (!mDisableEvents)
			{
				// Check if we have any properties loaded?
				if (mPropertiesList.Length > 0)
				{
					this.AssetPropertiesApplyButton.Enabled = true;
				}
			}
		}

		private void TogglePropertiesViewToSimpleMode(bool simpleMode)
		{
			if (simpleMode)
			{
				this.PropertiesSimpleViewPanel.Visible = true;
				this.PropertiesAdvancedPropertyGrid.Visible = false;
				PropertiesAdvancedButton.Text = "Advanced>>>";
			}
			else
			{
				this.PropertiesSimpleViewPanel.Visible = false;
				this.PropertiesAdvancedPropertyGrid.Visible = true;
				PropertiesAdvancedButton.Text = "<<<Simple";
			}
		}
		#endregion
		#region Packaging tab functions
		private void SetTextBoxProperty(TextBox EditTextBox, string newVal)
		{
			// Check if this new text is the same as the one already placed by a previous asset properties
			if (EditTextBox.Tag == null && EditTextBox.Text.Length > 0 && string.Compare(EditTextBox.Text, newVal, true) != 0)
			{
				// If they don't match, then we have to clear this value
				EditTextBox.Text = "";
				EditTextBox.Tag = "Already set";
			}
			else
			{
				EditTextBox.Text = newVal;
			}
		}

		private void SetComboBoxProperty(ComboBox EditComboBox, string newVal, bool force)
		{
			if (mResetWildcardCheck)
			{
				EditComboBox.Text = newVal;
				EditComboBox.Tag = null;
			}
			else
			{
				// Check if this new text is the same as the one already placed by a previous asset properties
				if (!force && EditComboBox.Tag == null && EditComboBox.Text.Length > 0 && string.Compare(EditComboBox.Text, newVal, true) != 0)
				{
					// If they don't match, then we have to clear this value
					EditComboBox.Text = "";
					EditComboBox.Tag = "Already set";
				}
				else
				{
					EditComboBox.Text = newVal;
				}
			}
		}

		private void SetTextBoxProperty(CheckBox EditCheckBox, MOG_InheritedBoolean newVal)
		{
			CheckState state = CheckState.Unchecked;
			switch (newVal)
			{
				case MOG_InheritedBoolean.True:
					state = CheckState.Checked;
					break;
				case MOG_InheritedBoolean.False:
					state = CheckState.Unchecked;
					break;
				case MOG_InheritedBoolean.Inherited:
					state = CheckState.Indeterminate;
					break;
			}

			// Check if this new text is the same as the one already placed by a previous asset properties
			if (EditCheckBox.Tag != null && EditCheckBox.CheckState != state)
			{
				// If they don't match, then we have to clear this value
				EditCheckBox.CheckState = CheckState.Indeterminate;
				EditCheckBox.Tag = "Already set";
			}
			else
			{
				EditCheckBox.CheckState = state;
			}
		}
		#endregion
		#region User Properties Tab Functions
		private TreeNode FindNode(TreeNodeCollection nodes, string title)
		{
			if (nodes != null)
			{
				foreach (TreeNode node in nodes)
				{
					if (string.Compare(node.Text, title, true) == 0)
					{
						return node;
					}
				}
			}
			return null;
		}

		private string AssetPropertiesContextMenu_AddSpecial(MOG_Ini menu, ref MenuItem item, string menuText)
		{
			if (menu.SectionExist(menuText))
			{
				MenuItem subItem = new MenuItem();
				subItem.Text = menuText;

				for (int x=0; x<menu.CountKeys(menuText); x++)
				{
					string label = menu.GetKeyNameByIndexSLOW(menuText, x);
					string newItem = AssetPropertiesContextMenu_AddSpecial(menu, ref subItem, label);
					if (newItem.Length != 0)
					{
						//subItem.MenuItems.Add(newItem, new EventHandler (AssetPropertiesNewContextMenu_Click));
					}
				}

				item.MenuItems.Add(subItem);
				return "";
			}
			
			return menuText;
		}

		/// <summary>
		/// Populate the properties context menu with the correct menu items based on the key of the current asset
		/// </summary>
		private void AssetPropertiesNewContextMenu_Popup(object sender, CancelEventArgs e)
		{
			// If we only have one asset selected
			if( this.AssetNames.Count == 1 )
			{
				MOG_Filename currentFilename = new MOG_Filename(AssetNames[0].ToString());
				// Populate the menu with the correct menu items based on the key
				string key = currentFilename.GetAssetClassification().ToLower();

				// Check to see if there is a special menu options file
				string location = string.Concat(MOG_ControllerProject.GetProject().GetProjectToolsPath(), "\\", key, ".info");
				if (DosUtils.FileExist(location))
				{
					MOG_Ini specialMenu = new MOG_Ini(location);
			
					if (specialMenu.SectionExist("Options_Menu"))
					{
						// Clear the list
						AssetPropertiesSpecialMenuItem.DropDownItems.Clear();

						//AssetPropertiesSpecialMenuItem.MenuItems.Add(AssetPropertiesContextMenu_AddSpecial(specialMenu, ref AssetPropertiesSpecialMenuItem, "Options_Menu"), new EventHandler (AssetPropertiesNewContextMenu_Click));
					}
				}
			}
		}

		private int GetPropertyImageIndex(TreeNode parent, string label)
		{
			MOG_Properties properties = this.PropertiesAdvancedPropertyGrid.SelectedObject as MOG_Properties;

			switch (label.ToLower())
			{
				case "properties":
					return MogUtil_AssetIcons.GetClassIconIndex("IMG_PROPERTIES", properties);
				case "asset info":
					return MogUtil_AssetIcons.GetClassIconIndex("IMG_ASSETINFO", properties);
				case "asset stats":
					return MogUtil_AssetIcons.GetClassIconIndex("IMG_ASSETSTATS", properties);
				case "asset options":
					return MogUtil_AssetIcons.GetClassIconIndex("IMG_KEY", properties);
				case "classification info":
					return MogUtil_AssetIcons.GetClassIconIndex("IMG_CLASSINFO", properties);
				case "sync options":
					return MogUtil_AssetIcons.GetClassIconIndex("IMG_SYNCOPTIONS", properties);
				case "rip options":
					return MogUtil_AssetIcons.GetClassIconIndex("IMG_PROPERTIES", properties);					
				case "package options":
					return MogUtil_AssetIcons.GetClassIconIndex("IMG_PACKAGEOPTIONS", properties);
				case "packaging commands":
					return MogUtil_AssetIcons.GetClassIconIndex("IMG_PACKAGECOMMANDS", properties);
				case "build options":
					return MogUtil_AssetIcons.GetClassIconIndex("IMG_BUILDOPTIONS", properties);
				case "files.imported":
					return MogUtil_AssetIcons.GetClassIconIndex("IMG_IMPORTED", properties);
				case "files.processed":
					return MogUtil_AssetIcons.GetClassIconIndex("IMG_PROCESSED", properties);
				case "relationships":
					return MogUtil_AssetIcons.GetClassIconIndex("IMG_RELATION", properties);
				case "packages":
					return MogUtil_AssetIcons.GetClassIconIndex("IMG_PACKAGE", properties);
				default:
					return MogUtil_AssetIcons.GetClassIconIndex("IMG_PROPERTY", properties);
			}
		}

		private void CreateNewTreeNode(TreeView view, string section, string property, string key, string val, string inheritedString)
		{
			TreeNode nodeSection = null;
			TreeNode nodeProperty = null;
			TreeNode nodeKey = null;
			TreeNode nodeValue = null;
			string InheritanceState = inheritedString.Length == 0 ? "" : "Inherited from:\n   " + inheritedString;
			key = inheritedString.Length == 0 ? key : key + "*";

			nodeSection = FindNode(view.Nodes, section);
			if(nodeSection != null)
			{
				nodeProperty = FindNode(nodeSection.Nodes, property);
				if(nodeProperty != null)
				{
					nodeKey = FindNode(nodeProperty.Nodes, key);
					if(nodeKey != null)
					{
						nodeValue = FindNode(nodeKey.Nodes, val);						
					}
				}
			}

			// Add or update the found tree nodes
			if (nodeSection != null)
			{
				if (nodeProperty != null)
				{				
					if (nodeKey != null)
					{
						if (nodeValue != null)
						{
							// We are already up to date
						}
						else
						{
							// This must be a simple change on the value of the branch
							nodeKey.NextNode.Text = val;
							nodeKey.ToolTipText = InheritanceState;
						}
					}
					else
					{
						if (val.Length > 0)
						{
							// Found the section but is missing new key and value
							int imageIndex = GetPropertyImageIndex(nodeProperty, key);
							nodeKey = new TreeNode(key + " = " + val, imageIndex, imageIndex);
							nodeKey.Tag = new MOG_Property(section, property, key, val);
							nodeKey.ToolTipText = InheritanceState;

							nodeProperty.Nodes.Add(nodeKey);
						}
						else
						{
							// Found the section but is missing new key and value
							int imageIndex = GetPropertyImageIndex(nodeProperty, key);
							nodeKey = new TreeNode(key, imageIndex, imageIndex);
							nodeKey.Tag = new MOG_Property(section, property, key, val);
							nodeKey.ToolTipText = InheritanceState;

							nodeProperty.Nodes.Add(nodeKey);
						}
					}
				}
				else
				{
					// Found the section but is missing new property, key and value
					int imageIndex = GetPropertyImageIndex(nodeSection, property);
					nodeProperty = new TreeNode(property, imageIndex, imageIndex);
					nodeProperty.Tag = new MOG_Property(section, property, key, val);
					
					int imageIndex2 = GetPropertyImageIndex(nodeProperty, key);
					nodeKey = new TreeNode(key + " = " + val, imageIndex2, imageIndex2);
					nodeKey.Tag = new MOG_Property(section, property, key, val);
					nodeKey.ToolTipText = InheritanceState;

					nodeProperty.Nodes.Add(nodeKey);
					nodeSection.Nodes.Add(nodeProperty);
				}
			}
			else
			{
				// We didn't find anything, create full branch
				int imageIndex = GetPropertyImageIndex(null, section);
				nodeSection = new TreeNode(section, imageIndex, imageIndex);
				nodeSection.Tag = new MOG_Property(section, property, key, val);

				if (val.Length > 0)
				{
					int imageIndex2 = GetPropertyImageIndex(nodeSection, property);
					nodeProperty = new TreeNode(property, imageIndex2, imageIndex2);					
					nodeProperty.Tag = new MOG_Property(section, property, key, val);

					int imageIndex3 = GetPropertyImageIndex(nodeProperty, key);
					nodeKey = new TreeNode(key + " = " + val, imageIndex3, imageIndex3);
					nodeKey.Tag = new MOG_Property(section, property, key, val);
					nodeKey.ToolTipText = InheritanceState;

					nodeProperty.Nodes.Add(nodeKey);
				}
				else if (property.Length > 0)
				{
					int imageIndex2 = GetPropertyImageIndex(nodeSection, property);
					nodeProperty = new TreeNode(property, imageIndex2, imageIndex2);					
					nodeProperty.Tag = new MOG_Property(section, property, key, val);

					int imageIndex3 = GetPropertyImageIndex(nodeProperty, key);
					nodeKey = new TreeNode(key, imageIndex3, imageIndex3);
					nodeKey.Tag = new MOG_Property(section, property, key, val);
					nodeKey.ToolTipText = InheritanceState;

					nodeProperty.Nodes.Add(nodeKey);
				}
				else
				{
					int imageIndex2 = GetPropertyImageIndex(nodeSection, key);
					nodeProperty = new TreeNode(key, imageIndex2, imageIndex2);					
					nodeProperty.Tag = new MOG_Property(section, property, key, val);
				}


				nodeSection.Expand();
				nodeSection.Nodes.Add(nodeProperty);

				view.Nodes.Add(nodeSection);
			}
		}
		#endregion

		#region AssetPropertiesFormContainer (AssetPropertiesFormContainer) code
		public struct AssetPropertiesFormContainer
		{
			public string assetName;
			public AssetPropertiesForm attachedForm;
			public AssetPropertiesFormContainer( string assetName, AssetPropertiesForm attachedForm )
			{
				this.assetName = assetName;
				this.attachedForm = attachedForm;
			}

			public override string ToString()
			{
				return this.assetName;
			}

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="assetsToOpen"></param>
		/// <returns>True if form is already open</returns>
		public static bool BringToFrontIfAlreadyOpen( ArrayList assetsToOpen, AssetPropertiesForm formToAttach )
		{
			ArrayList formsOpenedSoFar = new ArrayList();
			ArrayList assetsAlreadyOpened = new ArrayList();
			foreach( string assetName in assetsToOpen )
			{
				foreach( AssetPropertiesFormContainer container in mOpenAssetProperties )
				{
					if (container.ToString().ToLower() == assetName.ToLower())
					{
						if( !formsOpenedSoFar.Contains( container.attachedForm ) )
						{
							container.attachedForm.Activate();
							formsOpenedSoFar.Add( container.attachedForm );
						}
						assetsAlreadyOpened.Add( assetName );
						break;
					}
				}
			}

			// Remove assetNames that we've already opened
			foreach( string assetName in assetsAlreadyOpened )
			{
				assetsToOpen.Remove( assetName );
			}

			// Add assetNames we're about to open to the list of opened assets
			foreach( string assetName in assetsToOpen )
			{
				mOpenAssetProperties.Add( new AssetPropertiesFormContainer( assetName, formToAttach ) );
			}

			// If we still have assetsToOpen after our little algorithm, return true...
			if( assetsToOpen.Count > 0 )
			{
				// We are not already open
				return false;
			}
			// We are already open
			return true;
		}

		public static void RemoveAssetNamesFromOpenPropertiesList( ArrayList assetNames )
		{
			ArrayList containersToRemove = new ArrayList();
			foreach( string assetName in assetNames )
			{
				foreach( AssetPropertiesFormContainer container in mOpenAssetProperties )
				{
					if( container.assetName.ToLower() == assetName.ToLower() )
					{
						containersToRemove.Add( container );
					}
				}
			}
			foreach( AssetPropertiesFormContainer container in containersToRemove )
			{
				mOpenAssetProperties.Remove( container );
			}
		}
		#endregion

		#region Property editors		
		private void PropertiesUsersPropertiesTreeView_DoubleClick(object sender, System.EventArgs e)
		{
			if (PropertiesUsersPropertiesTreeView.SelectedNode != null)
			{
				try
				{
					MOG_Property property = PropertiesUsersPropertiesTreeView.SelectedNode.Tag as MOG_Property;
					if (property != null)
					{
						// If we have a null Parent or a null Parent.Parent for our TreeNode, we are less than depth of 3, so
						//  we can ignore this double-click
						if(PropertiesUsersPropertiesTreeView.SelectedNode.Parent == null 
							|| PropertiesUsersPropertiesTreeView.SelectedNode.Parent.Parent == null)
						{
							return;
						}

						// Get our editor and proceed to allow the user to change this property
						UITypeEditor editor = GetPropertyEditor(property.mPropertyKey);
						// If we found an editor...
						if(editor != null)
						{
							// Get the new value from the editor
							object setToValue = GetValueForProperty(editor, property.mPropertyValue);
							string newValue = setToValue.ToString();
							// Make sure there was something selected...Cancel will return ""
							if (newValue.Length > 0)
							{
								if (editor.GetType() == typeof(MOG.UITYPESEDITORS.MOGToolTypeEditor))
								{
									// Always make sure we bust this down to the relative path within the tools dir
									newValue = MOG_ControllerSystem.InternalizeTool(newValue, "");
								}

								// If this is a icon change, notify the user that these settings will not be seen till a re-login
								// Check for 'AssetIcon' or 'ClassIcon'
								if (property.mPropertyKey.ToLower().IndexOf("icon") != -1)
								{
									MOG_Prompt.PromptResponse(property.mPropertySection, "This icon change will not be visible until you re-login to this project");
								}

								// Fixup the selected node's text
								PropertiesUsersPropertiesTreeView.SelectedNode.Text = property.mPropertyKey + " = " + newValue;

								// Set the property value
								foreach (MOG_Properties properties in this.mPropertiesList)
								{
									properties.SetProperty(property.mSection, property.mPropertySection, property.mPropertyKey, newValue);
								}
								PropertiesModified();
							}
						}
						// We need to do some custom editing...
						else
						{
							PropertyDescriptor descriptor = GetPropertyDescriptor(property.mPropertyKey);

							// If this is a visible property we can edit...
							if( descriptor != null)
							{
								if(descriptor.PropertyType.AssemblyQualifiedName == (typeof(string)).AssemblyQualifiedName)
								{
									CreateInstantTextBox(descriptor);
								}
								else if(descriptor.PropertyType.AssemblyQualifiedName == (typeof(MOG_InheritedBoolean)).AssemblyQualifiedName)
								{
									CreateInstantComboBox(descriptor);
								}
								else if(descriptor.PropertyType.AssemblyQualifiedName == (typeof(bool)).AssemblyQualifiedName)
								{
									CreateInstantComboBox(descriptor);
								}
								bIgnoreFormKeyPressEvent = true;
							}
							// Else, if we only have one MOG_Properties and this is a custom property the user has created, display a text editor
							else if(mPropertiesList.Length == 1 && string.Compare(property.mSection, "properties", true) != 0) 
							{
								CreateInstantTextBox(property);
								bIgnoreFormKeyPressEvent = true;
							}
						}
					}
				}
				catch( Exception ex)
				{
					MOG_Prompt.PromptMessage("Error Editting Property", ex.Message, ex.StackTrace, MOG_ALERT_LEVEL.ERROR);
				}
			}
		}

		/// <summary>
		/// Makes a ComboBox for either an InheritedBoolean or a regular bool, such that, when
		///  ComboBox looses focus, it will dispose itself.
		/// </summary>
		/// <param name="descriptor"></param>
		private void CreateInstantComboBox(PropertyDescriptor descriptor)
		{
			// Store what we want from our PropertyDescriptor
			Type typeToCreate = descriptor.PropertyType;
			string propertyName = descriptor.Name;
			object currentValue = GetDescriptorValues(descriptor, mPropertiesList);

			// Get the bounds within which we will be placing our ComboBox
			Rectangle boundsForComboBox = GetOverlayControlBounds(this.PropertiesUsersPropertiesTreeView.SelectedNode.Bounds, this.PropertiesUsersPropertiesTreeView.Bounds);

			// Create our ComboBox and add it to our PropertiesTabPage
			ComboBox propertyComboBox = new ComboBox();
			this.PropertiesTabPage.Controls.Add(propertyComboBox);

			// Populate our ComboBox with meaningful information and select the current Property value
			AddComboBoxItems(propertyComboBox, typeToCreate);
			SetComboBoxIndexForCurrentValue(propertyComboBox, currentValue);

			// Setup how our ComboBox will display
			propertyComboBox.Bounds = boundsForComboBox;
			propertyComboBox.Tag = descriptor;
			propertyComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			propertyComboBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
			propertyComboBox.SelectedIndexChanged += new EventHandler(propertyComboBox_SelectedIndexChanged);
			propertyComboBox.KeyPress += new KeyPressEventHandler(propertyComboBox_KeyPress);
			propertyComboBox.LostFocus += new EventHandler(propertyComboBox_LostFocus);

			// Bring to front and give focus to our ComboBox
			propertyComboBox.BringToFront();
			propertyComboBox.Focus();
			PropertiesTabPage.Refresh();
		}

		
		/// <summary>
		/// This creates a TextBox within which to edit strings
		/// </summary>
		/// <param name="descriptor"></param>
		private void CreateInstantTextBox(PropertyDescriptor descriptor)
		{
			CreateInstantTextBox(descriptor as object);
		}

		private void CreateInstantTextBox(MOG_Property property)
		{
			CreateInstantTextBox(property as object);
		}

		private void CreateInstantTextBox(object propertyOrDescriptor)
		{
			PropertyDescriptor descriptor = propertyOrDescriptor as PropertyDescriptor;
			MOG_Property property = propertyOrDescriptor as MOG_Property;

			// Gets bounds for our TextBox
			Rectangle boundsForTextBox = GetOverlayControlBounds(this.PropertiesUsersPropertiesTreeView.SelectedNode.Bounds, this.PropertiesUsersPropertiesTreeView.Bounds);

			// Create our Textbox
			TextBox propertyTextBox = new TextBox();
			this.PropertiesTabPage.Controls.Add(propertyTextBox);

			// Assign our tag based on whether we have a descriptor or not...
			if(descriptor != null)
			{
				// Set our current value
				propertyTextBox.Text = this.GetDescriptorValues(descriptor, this.mPropertiesList) as string;
				propertyTextBox.Tag = descriptor;
			}
			else
			{
				propertyTextBox.Text = property.mValue;
				propertyTextBox.Tag = property;
			}

			// Setup how we will display
			propertyTextBox.Bounds = boundsForTextBox;
			propertyTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
			//propertyTextBox.TextChanged += new EventHandler(propertyTextBox_TextChanged);
			propertyTextBox.KeyPress += new KeyPressEventHandler(propertyTextBox_KeyPress);
			propertyTextBox.LostFocus += new EventHandler(propertyTextBox_LostFocus);

			// Bring to front and give focus to our ComboBox
			propertyTextBox.BringToFront();
			propertyTextBox.Focus();
			PropertiesTabPage.Refresh();
		}

		/// <summary>
		/// Takes a PropertyDescriptor and list of MOG_Properties objects and returns the 
		///  value of all those MOG_Properties, so long as all the values are the same.  If 
		///  values are not the same, function returns `null`.
		/// </summary>
		/// <param name="descriptor"></param>
		/// <param name="properties"></param>
		/// <returns>Returns null if collective properties' value is indeterminant</returns>
		private object GetDescriptorValues(PropertyDescriptor descriptor, object[] properties)
		{
			object value = null;
			try
			{
				// Go through each of our MOG_Properties
				for(int i = 0; i < properties.Length; ++i)
				{
					MOG_Properties property = (MOG_Properties)properties[i];

					// Try getting our current value through .NET
					object currentValue = null;
					try
					{
						currentValue = descriptor.GetValue(property);
					}
					// Catch-and-eat for if we have a .NET-internal error
					catch(System.Reflection.TargetInvocationException ex)
					{
						System.Diagnostics.Debug.WriteLine(ex.ToString());
					}

					// If value is null and we are the first iteration, we need to set it...
					if(value == null && i == 0)
					{
						// Set our value to be the first element's value
						value = currentValue;
					}
						// Else we are not the first iteration
					else if(i > 0)
					{
						// If value and current value do not match a state of null-ness (either `null` or `not null` for both)
						if((currentValue == null && value != null)
							|| (currentValue != null && value == null))
						{
							// Return null because the collective value is indeterminant
							return null;
						}
						// Else, so long as we don't have a null currentValue...
						else if(currentValue != null)
						{
							// Get an object for comparison
							IComparable comparable = currentValue as IComparable;
							// If we got a comparable object, that is the same as our existing value...
							if(comparable != null && comparable.CompareTo(value) == 0)
							{
								// Go to next iteration
								continue;
							}
							// Else, if we can't compare, return null
							else
							{
								// Collective value indeterminant
								return null;
							}
						}
						// Else, we continue with a null currentValue and value
					}
				}
			}
			catch(Exception ex)
			{
				MOG_Report.ReportMessage("Programmer Error", ex.Message, ex.StackTrace, MOG_ALERT_LEVEL.ERROR);
			}

			// If we made it this far, return the collective value
			return value;
		}// end ()

		private void SetDescriptorValues(PropertyDescriptor descriptor, object[] properties, object value)
		{
			// Set each descriptor to the value provided...
			foreach(MOG_Properties property in properties)
			{
				descriptor.SetValue(property, value);
				// If our MOG_Properties is saying it is modified, let our Form know to display the Apply button...
				if(property.IsModified())
				{
					this.PropertiesModified();
				}
			}
		}

//		private void propertyTextBox_TextChanged(object sender, EventArgs e)
//		{
//			propertyTextBox_commitChange((TextBox)sender);
//		}

		private void propertyTextBox_KeyPress(object sender, KeyPressEventArgs e)
		{
			TextBox tb = (TextBox)sender;
			// If user hit enter, dedicate our change(s) and close our ComboBox
			if(e.KeyChar == (char)Keys.Enter)
			{
				// Mark our event as handled
				e.Handled = true;
				// Give focus back to our TreeView, making it so we commit our changes
				this.PropertiesUsersPropertiesTreeView.Focus();
			}
				// Else, if the user has chosen to escape, ignore any further changes...
			else if(e.KeyChar == (char)Keys.Escape)
			{
				// Mark as handled and dispose our textBox
				e.Handled = true;
				tb.Dispose();
			}            
		}

		private void propertyTextBox_commitChange(TextBox textBox)
		{
			try
			{
				// If we have a PropertyDescriptor, save via .NET ICustomDescriptor interface (via MOG_Properties)...
				PropertyDescriptor descriptor = textBox.Tag as PropertyDescriptor;
				if(descriptor != null)
				{
					SetDescriptorValues(descriptor, this.mPropertiesList, textBox.Text);
				}

				// If we have a MOG_Property, save by using MOG_Properties::SetProperty()
				MOG_Property property = textBox.Tag as MOG_Property;
				if(property != null)
				{
					foreach(MOG_Properties properties in this.mPropertiesList)
					{
						properties.SetProperty(property.mSection, property.mPropertySection, property.mPropertyKey, textBox.Text);
					}
					PropertiesModified();
				}
			}
			catch( Exception ex)
			{
				ex.ToString();
			}
		}


		private void propertyTextBox_LostFocus(object sender, EventArgs e)
		{
			TextBox tb = (TextBox)sender;
			try
			{
				// If we are not disposing, save our information
				if(tb.Disposing == false)
				{
					// Get our SelectedNode.Text in parts...
					string textUpToEqualsChar = this.PropertiesUsersPropertiesTreeView.SelectedNode.Text;
					int indexOfEquals = textUpToEqualsChar.IndexOf("=");
					// If we have an Equals (indicating this is not an Assigned Package or other such property)...
					if(indexOfEquals > -1 && indexOfEquals < textUpToEqualsChar.Length+1)
					{
						// Commit our change before we go changing the GUI
						propertyTextBox_commitChange(tb);

						// Get our text up to the equals character
						textUpToEqualsChar = textUpToEqualsChar.Substring(0, indexOfEquals+1);

						// Set our SelectedNode.Text to reflect the new value
						PropertyDescriptor descriptor = tb.Tag as PropertyDescriptor;
						if(descriptor == null)
						{
							this.PropertiesUsersPropertiesTreeView.SelectedNode.Text = textUpToEqualsChar + " " + tb.Text;
							MOG_Property property = tb.Tag as MOG_Property;
							if(property != null)
							{
								property.mValue = tb.Text;
							}
						}
						else
						{
							this.PropertiesUsersPropertiesTreeView.SelectedNode.Text = textUpToEqualsChar + " " + GetDescriptorValues(descriptor, this.mPropertiesList) as string;
						}
					}
				}
			}
			catch(Exception ex)
			{
				ex.ToString();
			}
			// Now dispose our comboBox
			tb.Dispose();
			this.bIgnoreFormKeyPressEvent = false;
		}

		/// <summary>
		/// Return a Rectangle that represents the bounds for a new user control to be placed
		///  over the top of an existing control using a rectangle represending the Height and Location and a 
		///  separate rectangle representing the maximum Width
		/// </summary>
		/// <param name="heightAndLocation"></param>
		/// <param name="width"></param>
		/// <returns></returns>
		private Rectangle GetOverlayControlBounds(Rectangle heightAndLocation, Rectangle containerWidth)
		{
			const int scrollBarWidth = 20;  // Width of treeView scrollbar (if activated)--ideally, we'd be getting this from Windows
			int modifiedWidth = containerWidth.Width - heightAndLocation.X - scrollBarWidth;
			int modifiedY = heightAndLocation.Y + containerWidth.Y;
			int modifiedX = heightAndLocation.X + containerWidth.X;
			return new Rectangle(new Point(modifiedX, modifiedY), new Size(modifiedWidth, heightAndLocation.Height));
		}

		private void SetComboBoxIndexForCurrentValue(ComboBox comboBox, object value)
		{
			if(value != null)
			{
				foreach( object item in comboBox.Items)
				{
					if (item.ToString().ToLower() == value.ToString().ToLower())
					{
						comboBox.SelectedItem = item;
						return;
					}
				}
			}
			// Else, we should be blank
		}

		private void AddComboBoxItems(ComboBox comboBox, Type typeToFind)
		{
			if(typeof(MOG_InheritedBoolean).AssemblyQualifiedName == typeToFind.AssemblyQualifiedName)
			{
				comboBox.Items.Add(MOG_InheritedBoolean.True);
				comboBox.Items.Add(MOG_InheritedBoolean.False);
				comboBox.Items.Add(MOG_InheritedBoolean.Inherited);
			}
			else if(typeof(bool).AssemblyQualifiedName == typeToFind.AssemblyQualifiedName)
			{
				comboBox.Items.Add(true);
				comboBox.Items.Add(false);
			}
			else
			{
				comboBox.Items.Add(InvalidTypeComboBox_Text + typeToFind.Name);
			}
		}

		/// <summary>
		/// If our user moves off the control, get rid of it (duplicates functionality in PropertyGrid)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void propertyComboBox_LostFocus(object sender, EventArgs e)
		{
			ComboBox cb = (ComboBox)sender;
			try
			{
				// Get our SelectedNode.Text
				string textUpToEqualsChar = this.PropertiesUsersPropertiesTreeView.SelectedNode.Text;
				int indexOfEquals = textUpToEqualsChar.IndexOf("=");
				if(indexOfEquals > -1 && indexOfEquals < textUpToEqualsChar.Length)
				{
					textUpToEqualsChar = textUpToEqualsChar.Substring(0, indexOfEquals+1);
					// Set our SelectedNode.Text to reflect the new value
					this.PropertiesUsersPropertiesTreeView.SelectedNode.Text = textUpToEqualsChar + " " + cb.SelectedItem.ToString();
				}
			}
			catch(Exception ex)
			{
				ex.ToString();
			}
			// Now dispose our comboBox
			cb.Dispose();
			bIgnoreFormKeyPressEvent = false;
		}

		private void propertyComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBox cb = (ComboBox)sender;
			propertyComboBox_commitChange(cb);
		}

		private void propertyComboBox_KeyPress(object sender, KeyPressEventArgs e)
		{
			ComboBox cb = (ComboBox)sender;
			// If user hit enter, dedicate our change(s) and close our ComboBox
			if(e.KeyChar == (char)Keys.Enter)
			{
				propertyComboBox_commitChange(cb);
				// Give focus back to our TreeView (spawning our LostFocus handler)
				this.PropertiesUsersPropertiesTreeView.Focus();
			}
				// Else, if the user has chosen to escape, ignore any further changes...
			else if(e.KeyChar == (char)Keys.Escape)
			{
				cb.Dispose();
			}
		}

		private void propertyComboBox_commitChange(ComboBox comboBox)
		{
			try
			{
				PropertyDescriptor descriptor = comboBox.Tag as PropertyDescriptor;
				if(descriptor != null)
				{
					SetDescriptorValues(descriptor, this.mPropertiesList, comboBox.SelectedItem);
				}
			}
			catch( Exception ex)
			{
				ex.ToString();
			}
		}

		private PropertyDescriptor GetPropertyDescriptor(string propertyToLookFor)
		{
			if( this.mPropertiesList != null)
			{
				// Foreach MOG_Properties object in our SelectedObjects, go ahead and find our propertyToLookFor...
				foreach( MOG_Properties properties in this.mPropertiesList)
				{
					// Foreach PropertyDescriptor, see if we find what we're looking for
					PropertyDescriptorCollection propCollection = properties.GetProperties(new Attribute[] { new BrowsableAttribute(true) });
					foreach( PropertyDescriptor descriptor in propCollection)
					{
						// If our displayName == the propertyToLookFor, we can get our editor and newValue...
						if( descriptor.DisplayName.ToLower() == propertyToLookFor.ToLower())
						{
							return descriptor;
						}
					}
				}				
			}
			return null;
		}
		
		/// <summary>
		/// Returns the first editor found for the propertyToLookFor (which should be the editor for all properties of that type)
		/// </summary>
		/// <param name="propertyToLookFor"></param>
		/// <returns></returns>
		public UITypeEditor GetPropertyEditor(string propertyToLookFor)
		{
			UITypeEditor editor = null;
			// If we have selected objects...
			if( this.mPropertiesList != null)
			{
				// Foreach MOG_Properties object in our SelectedObjects, go ahead and find our propertyToLookFor...
				foreach( MOG_Properties properties in mPropertiesList)
				{
					// Start algorithm to find Editor
					// If we don't have an editor yet, find it and get the value we need...
					if(editor == null)
					{
						// Get our collection of properties
						PropertyDescriptorCollection propCollection = properties.GetProperties(new Attribute[] { new BrowsableAttribute(true) });
						// Foreach PropertyDescriptor, see if we find what we're looking for
						foreach( PropertyDescriptor descriptor in propCollection)
						{
							// If our displayName == the propertyToLookFor, we can get our editor and newValue...
							if( descriptor.DisplayName.ToLower() == propertyToLookFor.ToLower())
							{
								return descriptor.GetEditor(typeof(UITypeEditor)) as UITypeEditor;
							}
						}
					}
				}				
			}
			return editor;
		}

		/// <summary>
		/// Get value from MOG_Properties selected in PropertiesAdvancedPropertyGrid.SelectedObjects.  
		/// This allows .NET to open the specific TypeEditor associated with the Property desired.
		/// </summary>
		/// <param name="propertyToLookFor">string of the Property name (e.g. "AssetRipper" xor "AssetRipTasker")</param>
		/// <param name="oldValue"></param>
		/// <returns>oldValue, if no value found</returns>
		public object GetValueForProperty(string propertyToLookFor, object oldValue)
		{
			UITypeEditor editor = GetPropertyEditor(propertyToLookFor);
			if(editor != null)
			{
				return GetValueForProperty(editor, oldValue);
			}
			return null;
		}


		/// <summary>
		/// Given an editor, gets the value of a property
		/// </summary>
		/// <param name="editor"></param>
		/// <param name="oldValue"></param>
		/// <returns></returns>
		public object GetValueForProperty(UITypeEditor editor, object oldValue)
		{
			object newValue = null;

			try
			{
				newValue = editor.EditValue(null, null, oldValue);
			}
			catch(Exception ex)
			{
				MOG_Report.ReportMessage("Error editting Property",
					"At runtime, the editor was unable to be used.\r\n\r\n" + ex.Message + "\r\n\r\n", ex.StackTrace, 
					MOG_ALERT_LEVEL.ERROR);

			}

			if (newValue == null)
			{
				return oldValue;
			}
			else if ((newValue as string) != null && ((string)newValue).Length == 0)
			{
				return oldValue;
			}
			else
			{
				return newValue;
			}
		}

		private void PropertiesTabControl_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			TabControl tabControl = (TabControl)sender;
			try
			{
				// If we have been modified, save our changes
//				if(mModified)
//				{
					switch(mLastTabName)
					{
						case "PropertiesRippingTabPage":
							this.SaveRippingChanges(null);
							break;
						case "PropertiesAssetPackagingTabPage":
							this.SavePackagingChanges(null);
							break;
						case "PropertiesPackageTabPage":
							this.SavePackageChanges(null);
							break;
					}
//				}

				// Decide which page we need to re-initialize here...
				ReInitializeMethod initializer = null;
				// If we have the PropertiesTabPage, re-init it
				if(tabControl.SelectedTab == this.PropertiesTabPage)
				{
					initializer = new ReInitializeMethod(InitializeUserPropertiesTreeView);
				}
				// If we have the ripping, re-init
				else if(tabControl.SelectedTab == this.PropertiesRippingTabPage)
				{
					initializer = new ReInitializeMethod(InitializeRipperInfo);
				}
				// If we have the Packaging tab, re-init
				else if(tabControl.SelectedTab == this.PropertiesAssetPackagingTabPage)
				{
					initializer = new ReInitializeMethod(InitializePackagingInfo);
				}
				// If we have the logs page, re-init
				else if(tabControl.SelectedTab == this.PropertiesLogTabPage)
				{
					initializer = new ReInitializeMethod(InitializeLogsTabPage);
				}
				// Last, if we have the Package tab, re-init
				else if(tabControl.SelectedTab == this.PropertiesPackageTabPage)
				{
					initializer = new ReInitializeMethod(InitializePackageInfo);
				}
				else if (tabControl.SelectedTab == this.PropertiesCommentsTabPage)
				{
					InitializeCommentsTabPage();
				}
				
				// This is really the only place we should be re-initializing a page....
				if(initializer != null)
				{
					this.ReInitializePage("", initializer);
				}

				mLastTabName = PropertiesTabControl.SelectedTab.Name;
			}
			catch(Exception ex)
			{
				MOG_Report.ReportMessage("Error Switching AssetPropertiesForm TabPage", ex.Message, ex.StackTrace, MOG_ALERT_LEVEL.ERROR);
			}
		}
		#endregion

		#region FormEvents functions (Close, click events, etc...)
		private void AssetPropertiesCloseButton_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void AssetPropertiesCancelButton_Click(object sender, System.EventArgs e)
		{
			mDisableEvents = true;
			this.Close();
		}

		private void PropertiesAdvancedButton_Click(object sender, System.EventArgs e)
		{
			this.ViewPropertiesInSimpleMode = !this.ViewPropertiesInSimpleMode;
			this.ReInitializePage("", new ReInitializeMethod(InitializeUserPropertiesTreeView));
		}

		private void AssetPropertiesExploreButton_Click(object sender, System.EventArgs e)
		{
			// If we only have one asset selected
			if( this.AssetNames.Count == 1 )
			{
				// Open Explorer at asset's location.
				guiCommandLine.ShellExecute(AssetNames[0].ToString());
			}
			else
			{
				if( MOGPromptResult.Yes == MOG_Prompt.PromptResponse( "Explore Multiple Assets", "Are you sure you would like to "
					+ "open Windows Explorer \r\nwindows for the " + this.mAssetNames.Count + " Assets selected?",
					MOGPromptButtons.YesNo))
				{
					foreach( string assetName in this.mAssetNames )
					{
						guiCommandLine.ShellExecute( assetName );
					}
				}
			}
		}

		/// <summary>
		/// Try to save out our properties, depending on what the user wants.
		/// </summary>
		private void AssetPropertiesForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (!mDisableEvents)
			{
				// Save sizes
				guiUserPrefs.SaveDynamic_LayoutPrefs("AssetProperties", this);
				try
				{
					bool bModified = false;

					// Figure out if anything has been modified and store the names of those modified assets
					foreach( MOG_Properties properties in mPropertiesList )
					{
						// Ask the property if it is modifed?
						if (properties.IsModified())
						{
							bModified = true;
							break;
						}
					}

					// If even one asset was modified, start saving it based on user preference
					if( bModified )
					{
						switch (MOG_Prompt.PromptResponse("Apply Property Changes?", "Would you like to apply these changes now?", MOGPromptButtons.YesNoCancel ) )
						{
							case MOGPromptResult.Yes:
								SavePropertiesChanges(true);
								break;
							case MOGPromptResult.No:						
								foreach( MOG_Properties properties in mPropertiesList )
								{
									properties.Close( false );
								}
								break;
							case MOGPromptResult.Cancel:
								e.Cancel = true;
								return;
						}
					}
					RemoveAssetNamesFromOpenPropertiesList( this.AssetNames );
				}
				catch( Exception ex )
				{
					MOG_Prompt.PromptMessage( "Properties Form", "Unable to close properties form properly.", ex.StackTrace );
				}
			}
			else
			{
				foreach( MOG_Properties properties in mPropertiesList )
				{
					properties.Close( false );
				}
				RemoveAssetNamesFromOpenPropertiesList( this.AssetNames );
			}
		}
	
		/// <summary>
		/// Activates the platform-specific scoped properties inside the PropertyGrid for MOG_Properties.
		///  Makes sure that the user has non-unpressed a button, and makes sure that only one button
		///  can be pressed at a time.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AssetPropertyGridToolBar_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			// Make sure no other buttons are pushed down
			foreach( ToolBarButton button in this.AssetPropertyGridToolBar.Buttons )
			{
				// If our reference is not the same as the clicked reference...
				if( button != e.Button )
				{
					button.Pushed = false;
				}
			}

			string platform = e.Button.Tag.ToString();
			mCurrentPlatform = platform;

			// Set all the tab names to their platform specific version
			foreach (TabPage page in this.PropertiesTabControl.TabPages)
			{
				if (page.Tag == null)
				{
					page.Tag = page.Text;
				}
				page.Text = platform + " " + page.Tag as string;
			}

			// Based on which platform is pushed, go ahead and change our propertyGrid
			ChangePropertyGridPlatform( platform );

			// Reset this property page to reflect the platform change
			ReInitializePage(platform, new ReInitializeMethod(InitializeUserPropertiesTreeView));

			// If the button our user clicked on is not coming up as pushed, push it
			if( !e.Button.Pushed )
			{
				e.Button.Pushed = true;
			}

			// This is kind of gross but the property grid wouldn't refresh when we changed the scope of the properties beneath it...this makes sure it refreshes
			this.PropertiesAdvancedPropertyGrid.SelectedObjects = mPropertiesList;
		}

		private void PropertiesSingleAssetRadioButton_CheckedChanged(object sender, System.EventArgs e)
		{
			this.SinglePackageControl.Visible = PropertiesSingleAssetRadioButton.Checked;
			this.SinglePackageControl.Dock = DockStyle.Fill;
			PropertiesModified();
		}

		private void PropertiesRippingNoRipRadioButton_CheckedChanged(object sender, System.EventArgs e)
		{
			this.RippersControl.Visible = !PropertiesRippingNoRipRadioButton.Checked;
			this.PropertiesRippingLastStateEnabled = !PropertiesRippingNoRipRadioButton.Checked;
			PropertiesModified();
		}

		private void PropertiesRippingEnableRipRadioButton_CheckedChanged(object sender, System.EventArgs e)
		{
			this.RippersControl.Visible = PropertiesRippingEnableRipRadioButton.Checked;
			this.PropertiesRippingLastStateEnabled = PropertiesRippingEnableRipRadioButton.Checked;
			PropertiesModified();
		}

		private void PropertiesMultiAssetRadioButton_CheckedChanged(object sender, System.EventArgs e)
		{
			this.MultiPackageControl.Visible = PropertiesMultiAssetRadioButton.Checked;
			this.MultiPackageControl.Dock = DockStyle.Fill;
			PropertiesModified();
		}

		private void PropertiesNoPackagingRadioButton_CheckedChanged(object sender, System.EventArgs e)
		{
			this.PropertiesPackagingParametersGroupBox.Visible = !PropertiesNoPackagingRadioButton.Checked;
			PropertiesModified();
		}	

		private void Properties_MainPropertyChangedEvent(object sender, System.EventArgs e)
		{
			PropertiesModified();
		}

		private void AssetPropertiesForm_Load(object sender, System.EventArgs e)
		{
			// Set the explore button to be enabled according to the privileges of this user
			if (AssetPropertiesExploreButton.Enabled)
			{
				AssetPropertiesExploreButton.Enabled = MOG_ControllerProject.GetPrivileges().GetUserPrivilege( MOG_ControllerProject.GetUser().GetUserName(), MOG_PRIVILEGE.ExploreAssetDirectory);
			}
		}

		private void AssetPropertiesApplyButton_Click(object sender, System.EventArgs e)
		{
			SavePropertiesChanges(false);			
		}

		/// <summary>
		/// Remove a key
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AssetPropertiesDelMenuItem_Click(object sender, System.EventArgs e)
		{
			try
			{
				// Get the selected tree node
				TreeNode node = this.PropertiesUsersPropertiesTreeView.SelectedNode;
				if (node != null)
				{
					// Get the property to remove
					MOG_Property property = node.Tag as MOG_Property;
					if (property != null)
					{
						// Save whether or not we should remove our node...
						bool removeNode = false;
						// Remove from each of our MOG_Properties, as applicable...
						foreach(MOG_Properties properties in this.mPropertiesList)
						{
							// Remove the property
							//MOG_Properties properties = this.PropertiesAdvancedPropertyGrid.SelectedObject as MOG_Properties;

							// Determine how deep of a remove this is
							string targetNode = node.Text;
							if (string.Compare(targetNode, property.mSection, true) == 0)
							{
								if (properties.GetNonInheritedPropertiesIni().RemoveSection(property.mSection) == 1)
								{
									removeNode |= true;

									// Mark our properties as modified
									PropertiesModified();
								}
							}
							else if (string.Compare(targetNode, property.mPropertySection, true) == 0)
							{
								if (properties.GetNonInheritedPropertiesIni().RemovePropertySection(property.mSection, property.mPropertySection) == 1)
								{
									removeNode |= true;

									// Mark our properties as modified
									PropertiesModified();
								}
							}
							else if (string.Compare(targetNode, property.mPropertyKey, true) == 0)
							{
								if (properties.GetNonInheritedPropertiesIni().RemovePropertyString(property.mSection, property.mPropertySection, property.mPropertyKey) == 1)
								{
									removeNode |= true;

									// Mark our properties as modified
									PropertiesModified();
								}
							}
							else
							{
								if (properties.RemoveProperty(property.mSection, property.mPropertySection, property.mPropertyKey))
								{
									removeNode |= true;

									// Mark our properties as modified
									PropertiesModified();
								}
							}
						} // end foreach

						// Remove the node
						if(removeNode)
						{
							node.Remove();
						}
					} // end if
				} // end if
			}
			catch(Exception ex)
			{
				MOG_Report.ReportMessage("Delete property", ex.Message, ex.StackTrace, MOG_ALERT_LEVEL.ERROR);
			}
		}
		private void PropertiesPackageManagementButton_Click(object sender, System.EventArgs e)
		{
			bool showDialog = true;
			bool isModified = false;
			
			//Go through all the open properties and see if any of them have been modified
			foreach (MOG_Properties props in this.mPropertiesList)
			{
				if (props.IsModified())
				{
					isModified = true;
					break;
				}
			}

			if (isModified)
			{
				//At least one of the properties has been modified, so the user has a decision to make
				if (MOG_Prompt.PromptResponse("Properties Modified", "Some properties have been modified.  MOG needs to save these properties before entering Package Management.  Would you like to save them now and enter Package Management?", MOGPromptButtons.YesNo) == MOGPromptResult.Yes)
				{
					foreach (MOG_Properties props in mPropertiesList)
					{
						props.Save();
					}
				}
				else
				{
					showDialog = false;
				}
			}

			if (showDialog)
			{
				try
				{
					bool hasBlessedAsset = bFormContainsBlessedAsset;			
					if( hasBlessedAsset )
					{
						if( MOGPromptResult.Yes != MOG_Prompt.PromptResponse("Blessed Asset Properties are Read Only!  Continue?",
							"Please note that you will only be able to view \r\n"
							+ "package assignments for this Asset.\r\n\r\n"
							+ "In order to change those assignments, please\r\n"
							+ "copy the Asset to your Inbox, change it, then\r\n"
							+ "re-bless it.\r\n\r\n"
							+ "Continue?",
							MOGPromptButtons.YesNoCancel) )
						{
							return;
						}
					}
					ArrayList fixedNames = new ArrayList();
					foreach (string name in mAssetNames)
					{
						fixedNames.Add(new MOG_Filename(name));
					}

					// Launch the Form
					PackageManagementForm form = new PackageManagementForm(fixedNames, hasBlessedAsset);
					form.ShowDialog(this);

					Initialize(mAssetNames);
				}
				catch(Exception ex)
				{
					MOG_Report.ReportMessage("PackageManagement", ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
				}
			}
		}

		private void ExploreRipperButton_Click(object sender, System.EventArgs e)
		{
			//this.PropertiesRippingRipperTextBox.Text = GetValueForProperty("AssetRipper", this.PropertiesRippingRipperTextBox.Text) as string;
		}

		private void ExploreSlaveTaskerButton_Click(object sender, System.EventArgs e)
		{
			//this.PropertiesRippingSlaveTaskerTextBox.Text = GetValueForProperty("AssetRipTasker", PropertiesRippingSlaveTaskerTextBox.Text) as string;
		}

		private void ExplorerMultiPakToolButton_Click(object sender, System.EventArgs e)
		{
			//this.PropertiesMultiPakToolTextBox.Text = GetValueForProperty("PackageTool", PropertiesMultiPakToolTextBox.Text) as string;
		}
		
		private void AssetWizardButton_Click(object sender, System.EventArgs e)
		{
			SetupPackagingWizard form = new SetupPackagingWizard();
			if (form.ShowDialog() == DialogResult.OK)
			{				
				ArrayList settings = new ArrayList();
				
				settings.Add(new WizardSetting("PackageAddFile", form.PackageSimpleAddTextBox.Text));
				settings.Add(new WizardSetting("PackageRemoveFile", form.PackageSimpleRemoveTextBox.Text));

				this.SinglePackageControl.IntializeWizardSettings(settings);
				this.PropertiesSingleAssetRadioButton.Checked = true;				
			}
		}

		private void PackageWizardButton_Click(object sender, System.EventArgs e)
		{
			SetupPackageForm form = new SetupPackageForm();
			if (form.ShowDialog() == DialogResult.OK)
			{
			}
		}

		private void AssetPropertiesForm_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if(e.KeyChar == (char)13)
			{
				this.PropertiesTabControl.Focus();
			}
			// Else, If we are not ignoring events and we have our escape character pressed...
			else if(bIgnoreFormKeyPressEvent == false && e.KeyChar == (char)Keys.Escape)
			{
				AssetPropertiesCancelButton_Click(sender, e as EventArgs);
			}
		}

		private void PropertiesPackagingAddComboBox_Leave(object sender, System.EventArgs e)
		{
			foreach (MOG_Properties properties in this.mPropertiesList)
			{
				properties.PackageCommand_Add = this.PropertiesPackagingAddComboBox.Text;
			}
		}

		private void PropertiesPackagingAddComboBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			foreach (MOG_Properties properties in this.mPropertiesList)
			{
				SetComboBoxProperty(this.PropertiesPackagingAddComboBox, properties.PackageCommand_Add, true);
			}
		}

		private void PropertiesPackagingRemoveComboBox_Leave(object sender, System.EventArgs e)
		{
			foreach (MOG_Properties properties in this.mPropertiesList)
			{
				properties.PackageCommand_Remove = this.PropertiesPackagingRemoveComboBox.Text;
			}
		}

		private void PropertiesPackagingRemoveComboBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			foreach (MOG_Properties properties in this.mPropertiesList)
			{
				SetComboBoxProperty(this.PropertiesPackagingRemoveComboBox, properties.PackageCommand_Remove, true);
			}
		}
		#endregion

		private void PropertiesAdvancedPropertyGrid_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e)
		{
			// Now, this is handy only for debug purposes.  Before, it was being used (unnecessarily) to logically re-init the whole form...
			PropertiesModified();

			// If this is a icon change, notify the user that these settings will not be seen till a re-login
			// Check for 'AssetIcon' or 'ClassIcon'
			if (e.ChangedItem.Label.ToLower().IndexOf("icon") != -1)
			{
				MOG_Prompt.PromptResponse(e.ChangedItem.Label, "This icon change will not be visible until you re-login to this project");
			}
		}

		private void PropertiesPackagingAddComboBox_TextChanged(object sender, System.EventArgs e)
		{
			PropertiesModified();
		}

		#region MOG Depth Manager debug information events (Activated and GotFocus)
		private void AssetPropertiesForm_Activated(object sender, System.EventArgs e)
		{
			Form frm = sender as Form;
			Debug.WriteLine(frm.ActiveControl.Name, "Activated");
		}

		private void AssetPropertiesForm_GotFocus(object sender, EventArgs e)
		{
			Form frm = sender as Form;
			Debug.WriteLine(frm.ActiveControl.Name, "Focused");
		}
		#endregion 

		private void PropertiesRippingWizardButton_Click(object sender, System.EventArgs e)
		{
			// Create our wizard
			SetupRippingWizard wiz = new SetupRippingWizard();

			foreach (MOG_Properties properties in mPropertiesList)
			{				
				// Add the filename for this asset
				wiz.PropertiesList.Add(properties);					
			}

			wiz.ShowAdvancedWizard = true;

			// Open the wizard
			if (wiz.ShowDialog(this) == DialogResult.OK)
			{
				this.ReInitialize("");
			}
		}

		public void SelectTab(string tabPageName)
		{
			// This can crash if the asset doesn't have any comments
			try
			{
				PropertiesTabControl.SelectTab(tabPageName);
			}
			catch
			{
			}
		}
	}
}


