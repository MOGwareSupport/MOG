using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using MOG.PROPERTIES;
using MOG.FILENAME;
using MOG.REPORT;


namespace MOG_Client.Forms.AssetProperties
{
	/// <summary>
	/// Summary description for MultiPackageControl.
	/// </summary>
	public class MultiPackageControl : System.Windows.Forms.UserControl
	{
		private object[] mPropertiesList;
		public object[] PropertiesList
		{
			get
			{
				if(this.mPropertiesList == null)
				{
					throw new Exception("PropertiesList was never set.");
				}
				return mPropertiesList;
			}
			set
			{
				this.mPropertiesList = value;
			}
		}
		private bool mDisableEvents = false;
		public bool mResetWildcardCheck = false;
		public AssetPropertiesForm mParent;
		private System.Windows.Forms.Panel PropertiesMultiAssetPanel;
		private System.Windows.Forms.Button ExploreMultiWorkDirButton;
		private System.Windows.Forms.Button ExplorerMultiPakToolButton;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.GroupBox PropertiesMultiPackageFileGroupBox;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.CheckBox PropertiesMultiClusterCheckBox;
		private System.Windows.Forms.GroupBox PropertiesMultiPackagingEventsGroupBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox PropertiesMultiCleanWorkDirCheckBox;
		private System.Windows.Forms.GroupBox PackageWorkFolderGroupBox;
		private System.Windows.Forms.ComboBox PropertiesPackageInputComboBox;
		private System.Windows.Forms.ComboBox PropertiesMultiPakToolComboBox;
		private System.Windows.Forms.ComboBox PropertiesPackageOutputComboBox;
		private System.Windows.Forms.ComboBox PropertiesPackageEventsPreComboBox;
		private System.Windows.Forms.ComboBox PropertiesPackageEventsPostComboBox;
		private System.Windows.Forms.ComboBox PropertiesMultiResolverComboBox;
		private System.Windows.Forms.ComboBox PropertiesMultiPakDelComboBox;
		private System.Windows.Forms.ComboBox PropertiesMultiWorkDirComboBox;
		private System.Windows.Forms.Button PropertiesPackageEventsPreButton;
		private System.Windows.Forms.Button PropertiesPackageEventsPostButton;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		// Delegates
		public delegate void PropertyChangedEvent(object sender, System.EventArgs e);
		[Category("Behavior"), Description("Occures after a property is changed")]
		public event PropertyChangedEvent ProptertyChanged;

		public MultiPackageControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MultiPackageControl));
			this.PropertiesMultiAssetPanel = new System.Windows.Forms.Panel();
			this.PackageWorkFolderGroupBox = new System.Windows.Forms.GroupBox();
			this.PropertiesMultiWorkDirComboBox = new System.Windows.Forms.ComboBox();
			this.PropertiesMultiCleanWorkDirCheckBox = new System.Windows.Forms.CheckBox();
			this.label17 = new System.Windows.Forms.Label();
			this.ExploreMultiWorkDirButton = new System.Windows.Forms.Button();
			this.PropertiesMultiPackagingEventsGroupBox = new System.Windows.Forms.GroupBox();
			this.PropertiesPackageEventsPostButton = new System.Windows.Forms.Button();
			this.PropertiesPackageEventsPreButton = new System.Windows.Forms.Button();
			this.PropertiesPackageEventsPostComboBox = new System.Windows.Forms.ComboBox();
			this.PropertiesPackageEventsPreComboBox = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.PropertiesMultiClusterCheckBox = new System.Windows.Forms.CheckBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.PropertiesMultiPakDelComboBox = new System.Windows.Forms.ComboBox();
			this.PropertiesMultiResolverComboBox = new System.Windows.Forms.ComboBox();
			this.label13 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.PropertiesMultiPackageFileGroupBox = new System.Windows.Forms.GroupBox();
			this.PropertiesPackageOutputComboBox = new System.Windows.Forms.ComboBox();
			this.PropertiesMultiPakToolComboBox = new System.Windows.Forms.ComboBox();
			this.PropertiesPackageInputComboBox = new System.Windows.Forms.ComboBox();
			this.label15 = new System.Windows.Forms.Label();
			this.label16 = new System.Windows.Forms.Label();
			this.ExplorerMultiPakToolButton = new System.Windows.Forms.Button();
			this.label14 = new System.Windows.Forms.Label();
			this.PropertiesMultiAssetPanel.SuspendLayout();
			this.PackageWorkFolderGroupBox.SuspendLayout();
			this.PropertiesMultiPackagingEventsGroupBox.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.PropertiesMultiPackageFileGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// PropertiesMultiAssetPanel
			// 
			this.PropertiesMultiAssetPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesMultiAssetPanel.AutoScroll = true;
			this.PropertiesMultiAssetPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.PropertiesMultiAssetPanel.Controls.Add(this.PackageWorkFolderGroupBox);
			this.PropertiesMultiAssetPanel.Controls.Add(this.PropertiesMultiPackagingEventsGroupBox);
			this.PropertiesMultiAssetPanel.Controls.Add(this.PropertiesMultiClusterCheckBox);
			this.PropertiesMultiAssetPanel.Controls.Add(this.groupBox3);
			this.PropertiesMultiAssetPanel.Controls.Add(this.PropertiesMultiPackageFileGroupBox);
			this.PropertiesMultiAssetPanel.Location = new System.Drawing.Point(8, 8);
			this.PropertiesMultiAssetPanel.Name = "PropertiesMultiAssetPanel";
			this.PropertiesMultiAssetPanel.Size = new System.Drawing.Size(312, 456);
			this.PropertiesMultiAssetPanel.TabIndex = 26;
			// 
			// PackageWorkFolderGroupBox
			// 
			this.PackageWorkFolderGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.PackageWorkFolderGroupBox.Controls.Add(this.PropertiesMultiWorkDirComboBox);
			this.PackageWorkFolderGroupBox.Controls.Add(this.PropertiesMultiCleanWorkDirCheckBox);
			this.PackageWorkFolderGroupBox.Controls.Add(this.label17);
			this.PackageWorkFolderGroupBox.Controls.Add(this.ExploreMultiWorkDirButton);
			this.PackageWorkFolderGroupBox.Location = new System.Drawing.Point(8, 376);
			this.PackageWorkFolderGroupBox.Name = "PackageWorkFolderGroupBox";
			this.PackageWorkFolderGroupBox.Size = new System.Drawing.Size(299, 72);
			this.PackageWorkFolderGroupBox.TabIndex = 12;
			this.PackageWorkFolderGroupBox.TabStop = false;
			this.PackageWorkFolderGroupBox.Text = "Packaging Working Folder";
			// 
			// PropertiesMultiWorkDirComboBox
			// 
			this.PropertiesMultiWorkDirComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesMultiWorkDirComboBox.Items.AddRange(new object[] {
																				"None",
																				"Inherited"});
			this.PropertiesMultiWorkDirComboBox.Location = new System.Drawing.Point(80, 21);
			this.PropertiesMultiWorkDirComboBox.Name = "PropertiesMultiWorkDirComboBox";
			this.PropertiesMultiWorkDirComboBox.Size = new System.Drawing.Size(184, 21);
			this.PropertiesMultiWorkDirComboBox.TabIndex = 13;
			this.PropertiesMultiWorkDirComboBox.Validating += new System.ComponentModel.CancelEventHandler(this.PropertiesMultiWorkDirComboBox_Validating);
			this.PropertiesMultiWorkDirComboBox.Validated += new System.EventHandler(this.Properties_PropertyChanged);
			this.PropertiesMultiWorkDirComboBox.Click += new System.EventHandler(this.PropertiesActivateControl_Click);
			this.PropertiesMultiWorkDirComboBox.Leave += new System.EventHandler(this.PropertiesMultiWorkDirComboBox_Leave);
			// 
			// PropertiesMultiCleanWorkDirCheckBox
			// 
			this.PropertiesMultiCleanWorkDirCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PropertiesMultiCleanWorkDirCheckBox.Location = new System.Drawing.Point(80, 48);
			this.PropertiesMultiCleanWorkDirCheckBox.Name = "PropertiesMultiCleanWorkDirCheckBox";
			this.PropertiesMultiCleanWorkDirCheckBox.Size = new System.Drawing.Size(168, 16);
			this.PropertiesMultiCleanWorkDirCheckBox.TabIndex = 14;
			this.PropertiesMultiCleanWorkDirCheckBox.Text = "Clean up directory after merge";
			this.PropertiesMultiCleanWorkDirCheckBox.ThreeState = true;
			this.PropertiesMultiCleanWorkDirCheckBox.CheckStateChanged += new System.EventHandler(this.Properties_PropertyChanged);
			// 
			// label17
			// 
			this.label17.Image = ((System.Drawing.Image)(resources.GetObject("label17.Image")));
			this.label17.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label17.Location = new System.Drawing.Point(8, 22);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(120, 16);
			this.label17.TabIndex = 28;
			this.label17.Text = "      Directory:";
			this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// ExploreMultiWorkDirButton
			// 
			this.ExploreMultiWorkDirButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ExploreMultiWorkDirButton.Location = new System.Drawing.Point(270, 21);
			this.ExploreMultiWorkDirButton.Name = "ExploreMultiWorkDirButton";
			this.ExploreMultiWorkDirButton.Size = new System.Drawing.Size(24, 20);
			this.ExploreMultiWorkDirButton.TabIndex = 30;
			this.ExploreMultiWorkDirButton.TabStop = false;
			this.ExploreMultiWorkDirButton.Text = "...";
			this.ExploreMultiWorkDirButton.Click += new System.EventHandler(this.ExploreMultiWorkDirButton_Click);
			// 
			// PropertiesMultiPackagingEventsGroupBox
			// 
			this.PropertiesMultiPackagingEventsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesMultiPackagingEventsGroupBox.Controls.Add(this.PropertiesPackageEventsPostButton);
			this.PropertiesMultiPackagingEventsGroupBox.Controls.Add(this.PropertiesPackageEventsPreButton);
			this.PropertiesMultiPackagingEventsGroupBox.Controls.Add(this.PropertiesPackageEventsPostComboBox);
			this.PropertiesMultiPackagingEventsGroupBox.Controls.Add(this.PropertiesPackageEventsPreComboBox);
			this.PropertiesMultiPackagingEventsGroupBox.Controls.Add(this.label1);
			this.PropertiesMultiPackagingEventsGroupBox.Controls.Add(this.label2);
			this.PropertiesMultiPackagingEventsGroupBox.Location = new System.Drawing.Point(8, 128);
			this.PropertiesMultiPackagingEventsGroupBox.Name = "PropertiesMultiPackagingEventsGroupBox";
			this.PropertiesMultiPackagingEventsGroupBox.Size = new System.Drawing.Size(299, 72);
			this.PropertiesMultiPackagingEventsGroupBox.TabIndex = 4;
			this.PropertiesMultiPackagingEventsGroupBox.TabStop = false;
			this.PropertiesMultiPackagingEventsGroupBox.Text = "Package Event Commands";
			// 
			// PropertiesPackageEventsPostButton
			// 
			this.PropertiesPackageEventsPostButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesPackageEventsPostButton.Location = new System.Drawing.Point(272, 45);
			this.PropertiesPackageEventsPostButton.Name = "PropertiesPackageEventsPostButton";
			this.PropertiesPackageEventsPostButton.Size = new System.Drawing.Size(24, 21);
			this.PropertiesPackageEventsPostButton.TabIndex = 34;
			this.PropertiesPackageEventsPostButton.TabStop = false;
			this.PropertiesPackageEventsPostButton.Text = "...";
			this.PropertiesPackageEventsPostButton.Click += new System.EventHandler(this.PropertiesPackageEventsPostButton_Click);
			// 
			// PropertiesPackageEventsPreButton
			// 
			this.PropertiesPackageEventsPreButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesPackageEventsPreButton.Location = new System.Drawing.Point(272, 16);
			this.PropertiesPackageEventsPreButton.Name = "PropertiesPackageEventsPreButton";
			this.PropertiesPackageEventsPreButton.Size = new System.Drawing.Size(24, 21);
			this.PropertiesPackageEventsPreButton.TabIndex = 33;
			this.PropertiesPackageEventsPreButton.TabStop = false;
			this.PropertiesPackageEventsPreButton.Text = "...";
			this.PropertiesPackageEventsPreButton.Click += new System.EventHandler(this.PropertiesPackageEventsPreButton_Click);
			// 
			// PropertiesPackageEventsPostComboBox
			// 
			this.PropertiesPackageEventsPostComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesPackageEventsPostComboBox.Items.AddRange(new object[] {
																					 "None",
																					 "Inherited"});
			this.PropertiesPackageEventsPostComboBox.Location = new System.Drawing.Point(120, 44);
			this.PropertiesPackageEventsPostComboBox.Name = "PropertiesPackageEventsPostComboBox";
			this.PropertiesPackageEventsPostComboBox.Size = new System.Drawing.Size(144, 21);
			this.PropertiesPackageEventsPostComboBox.TabIndex = 6;
			this.PropertiesPackageEventsPostComboBox.Validating += new System.ComponentModel.CancelEventHandler(this.PropertiesPackageEventsPostComboBox_Validating);
			this.PropertiesPackageEventsPostComboBox.Validated += new System.EventHandler(this.Properties_PropertyChanged);
			this.PropertiesPackageEventsPostComboBox.Click += new System.EventHandler(this.PropertiesActivateControl_Click);
			this.PropertiesPackageEventsPostComboBox.Leave += new System.EventHandler(this.PropertiesPackageEventsPostComboBox_Leave);
			// 
			// PropertiesPackageEventsPreComboBox
			// 
			this.PropertiesPackageEventsPreComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesPackageEventsPreComboBox.Items.AddRange(new object[] {
																					"None",
																					"Inherited"});
			this.PropertiesPackageEventsPreComboBox.Location = new System.Drawing.Point(120, 16);
			this.PropertiesPackageEventsPreComboBox.Name = "PropertiesPackageEventsPreComboBox";
			this.PropertiesPackageEventsPreComboBox.Size = new System.Drawing.Size(144, 21);
			this.PropertiesPackageEventsPreComboBox.TabIndex = 5;
			this.PropertiesPackageEventsPreComboBox.Validating += new System.ComponentModel.CancelEventHandler(this.PropertiesPackageEventsPreComboBox_Validating);
			this.PropertiesPackageEventsPreComboBox.Validated += new System.EventHandler(this.Properties_PropertyChanged);
			this.PropertiesPackageEventsPreComboBox.Click += new System.EventHandler(this.PropertiesActivateControl_Click);
			this.PropertiesPackageEventsPreComboBox.Leave += new System.EventHandler(this.PropertiesPackageEventsPreComboBox_Leave);
			// 
			// label1
			// 
			this.label1.Image = ((System.Drawing.Image)(resources.GetObject("label1.Image")));
			this.label1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label1.Location = new System.Drawing.Point(16, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(104, 16);
			this.label1.TabIndex = 22;
			this.label1.Text = "      Pre-Merge tool:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.Image = ((System.Drawing.Image)(resources.GetObject("label2.Image")));
			this.label2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label2.Location = new System.Drawing.Point(16, 47);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(112, 16);
			this.label2.TabIndex = 24;
			this.label2.Text = "      Post-Merge tool:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// PropertiesMultiClusterCheckBox
			// 
			this.PropertiesMultiClusterCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PropertiesMultiClusterCheckBox.Location = new System.Drawing.Point(16, 334);
			this.PropertiesMultiClusterCheckBox.Name = "PropertiesMultiClusterCheckBox";
			this.PropertiesMultiClusterCheckBox.Size = new System.Drawing.Size(120, 24);
			this.PropertiesMultiClusterCheckBox.TabIndex = 11;
			this.PropertiesMultiClusterCheckBox.Text = "Cluster Packaging";
			this.PropertiesMultiClusterCheckBox.ThreeState = true;
			this.PropertiesMultiClusterCheckBox.CheckStateChanged += new System.EventHandler(this.Properties_PropertyChanged);
			// 
			// groupBox3
			// 
			this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox3.Controls.Add(this.PropertiesMultiPakDelComboBox);
			this.groupBox3.Controls.Add(this.PropertiesMultiResolverComboBox);
			this.groupBox3.Controls.Add(this.label13);
			this.groupBox3.Controls.Add(this.label10);
			this.groupBox3.Location = new System.Drawing.Point(8, 216);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(299, 80);
			this.groupBox3.TabIndex = 7;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Package Commands";
			// 
			// PropertiesMultiPakDelComboBox
			// 
			this.PropertiesMultiPakDelComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesMultiPakDelComboBox.Items.AddRange(new object[] {
																			   "None",
																			   "Inherited"});
			this.PropertiesMultiPakDelComboBox.Location = new System.Drawing.Point(112, 47);
			this.PropertiesMultiPakDelComboBox.Name = "PropertiesMultiPakDelComboBox";
			this.PropertiesMultiPakDelComboBox.Size = new System.Drawing.Size(184, 21);
			this.PropertiesMultiPakDelComboBox.TabIndex = 9;
			this.PropertiesMultiPakDelComboBox.Validating += new System.ComponentModel.CancelEventHandler(this.PropertiesMultiPakDelComboBox_Validating);
			this.PropertiesMultiPakDelComboBox.Validated += new System.EventHandler(this.Properties_PropertyChanged);
			this.PropertiesMultiPakDelComboBox.Click += new System.EventHandler(this.PropertiesActivateControl_Click);
			this.PropertiesMultiPakDelComboBox.Leave += new System.EventHandler(this.PropertiesMultiPakDelComboBox_Leave);
			// 
			// PropertiesMultiResolverComboBox
			// 
			this.PropertiesMultiResolverComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesMultiResolverComboBox.Items.AddRange(new object[] {
																				 "None",
																				 "Inherited"});
			this.PropertiesMultiResolverComboBox.Location = new System.Drawing.Point(112, 20);
			this.PropertiesMultiResolverComboBox.Name = "PropertiesMultiResolverComboBox";
			this.PropertiesMultiResolverComboBox.Size = new System.Drawing.Size(184, 21);
			this.PropertiesMultiResolverComboBox.TabIndex = 8;
			this.PropertiesMultiResolverComboBox.Validating += new System.ComponentModel.CancelEventHandler(this.PropertiesMultiResolverComboBox_Validating);
			this.PropertiesMultiResolverComboBox.Validated += new System.EventHandler(this.Properties_PropertyChanged);
			this.PropertiesMultiResolverComboBox.Click += new System.EventHandler(this.PropertiesActivateControl_Click);
			this.PropertiesMultiResolverComboBox.Leave += new System.EventHandler(this.PropertiesMultiResolverComboBox_Leave);
			// 
			// label13
			// 
			this.label13.Image = ((System.Drawing.Image)(resources.GetObject("label13.Image")));
			this.label13.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label13.Location = new System.Drawing.Point(8, 24);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(104, 16);
			this.label13.TabIndex = 17;
			this.label13.Text = "      Late Resolver:";
			this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label10
			// 
			this.label10.Image = ((System.Drawing.Image)(resources.GetObject("label10.Image")));
			this.label10.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label10.Location = new System.Drawing.Point(8, 48);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(112, 16);
			this.label10.TabIndex = 15;
			this.label10.Text = "      Delete Package:";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// PropertiesMultiPackageFileGroupBox
			// 
			this.PropertiesMultiPackageFileGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesMultiPackageFileGroupBox.Controls.Add(this.PropertiesPackageOutputComboBox);
			this.PropertiesMultiPackageFileGroupBox.Controls.Add(this.PropertiesMultiPakToolComboBox);
			this.PropertiesMultiPackageFileGroupBox.Controls.Add(this.PropertiesPackageInputComboBox);
			this.PropertiesMultiPackageFileGroupBox.Controls.Add(this.label15);
			this.PropertiesMultiPackageFileGroupBox.Controls.Add(this.label16);
			this.PropertiesMultiPackageFileGroupBox.Controls.Add(this.ExplorerMultiPakToolButton);
			this.PropertiesMultiPackageFileGroupBox.Controls.Add(this.label14);
			this.PropertiesMultiPackageFileGroupBox.Location = new System.Drawing.Point(8, 8);
			this.PropertiesMultiPackageFileGroupBox.Name = "PropertiesMultiPackageFileGroupBox";
			this.PropertiesMultiPackageFileGroupBox.Size = new System.Drawing.Size(299, 104);
			this.PropertiesMultiPackageFileGroupBox.TabIndex = 0;
			this.PropertiesMultiPackageFileGroupBox.TabStop = false;
			this.PropertiesMultiPackageFileGroupBox.Text = "Package Task File";
			// 
			// PropertiesPackageOutputComboBox
			// 
			this.PropertiesPackageOutputComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesPackageOutputComboBox.Items.AddRange(new object[] {
																				 "None",
																				 "Inherited"});
			this.PropertiesPackageOutputComboBox.Location = new System.Drawing.Point(72, 40);
			this.PropertiesPackageOutputComboBox.Name = "PropertiesPackageOutputComboBox";
			this.PropertiesPackageOutputComboBox.Size = new System.Drawing.Size(224, 21);
			this.PropertiesPackageOutputComboBox.TabIndex = 2;
			this.PropertiesPackageOutputComboBox.Validating += new System.ComponentModel.CancelEventHandler(this.PropertiesPackageOutputComboBox_Validating);
			this.PropertiesPackageOutputComboBox.Validated += new System.EventHandler(this.Properties_PropertyChanged);
			this.PropertiesPackageOutputComboBox.Click += new System.EventHandler(this.PropertiesActivateControl_Click);
			this.PropertiesPackageOutputComboBox.Leave += new System.EventHandler(this.PropertiesPackageOutputComboBox_Leave);
			// 
			// PropertiesMultiPakToolComboBox
			// 
			this.PropertiesMultiPakToolComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesMultiPakToolComboBox.Items.AddRange(new object[] {
																				"None",
																				"Inherited"});
			this.PropertiesMultiPakToolComboBox.Location = new System.Drawing.Point(98, 72);
			this.PropertiesMultiPakToolComboBox.Name = "PropertiesMultiPakToolComboBox";
			this.PropertiesMultiPakToolComboBox.Size = new System.Drawing.Size(168, 21);
			this.PropertiesMultiPakToolComboBox.TabIndex = 3;
			this.PropertiesMultiPakToolComboBox.Validating += new System.ComponentModel.CancelEventHandler(this.PropertiesMultiPakToolComboBox_Validating);
			this.PropertiesMultiPakToolComboBox.Validated += new System.EventHandler(this.Properties_PropertyChanged);
			this.PropertiesMultiPakToolComboBox.Click += new System.EventHandler(this.PropertiesActivateControl_Click);
			this.PropertiesMultiPakToolComboBox.Leave += new System.EventHandler(this.PropertiesMultiPakToolComboBox_Leave);
			// 
			// PropertiesPackageInputComboBox
			// 
			this.PropertiesPackageInputComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesPackageInputComboBox.Items.AddRange(new object[] {
																				"None",
																				"Inherited"});
			this.PropertiesPackageInputComboBox.Location = new System.Drawing.Point(72, 16);
			this.PropertiesPackageInputComboBox.Name = "PropertiesPackageInputComboBox";
			this.PropertiesPackageInputComboBox.Size = new System.Drawing.Size(224, 21);
			this.PropertiesPackageInputComboBox.TabIndex = 1;
			this.PropertiesPackageInputComboBox.Validating += new System.ComponentModel.CancelEventHandler(this.PropertiesPackageInputComboBox_Validating);
			this.PropertiesPackageInputComboBox.Validated += new System.EventHandler(this.Properties_PropertyChanged);
			this.PropertiesPackageInputComboBox.Click += new System.EventHandler(this.PropertiesActivateControl_Click);
			this.PropertiesPackageInputComboBox.Leave += new System.EventHandler(this.PropertiesPackageInputComboBox_Leave);
			// 
			// label15
			// 
			this.label15.Image = ((System.Drawing.Image)(resources.GetObject("label15.Image")));
			this.label15.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label15.Location = new System.Drawing.Point(16, 18);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(56, 16);
			this.label15.TabIndex = 22;
			this.label15.Text = "      Input:";
			this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label16
			// 
			this.label16.Image = ((System.Drawing.Image)(resources.GetObject("label16.Image")));
			this.label16.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label16.Location = new System.Drawing.Point(16, 43);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(56, 16);
			this.label16.TabIndex = 24;
			this.label16.Text = "      Output";
			this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// ExplorerMultiPakToolButton
			// 
			this.ExplorerMultiPakToolButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ExplorerMultiPakToolButton.Location = new System.Drawing.Point(272, 72);
			this.ExplorerMultiPakToolButton.Name = "ExplorerMultiPakToolButton";
			this.ExplorerMultiPakToolButton.Size = new System.Drawing.Size(24, 21);
			this.ExplorerMultiPakToolButton.TabIndex = 29;
			this.ExplorerMultiPakToolButton.TabStop = false;
			this.ExplorerMultiPakToolButton.Text = "...";
			this.ExplorerMultiPakToolButton.Click += new System.EventHandler(this.ExplorerMultiPakToolButton_Click);
			// 
			// label14
			// 
			this.label14.Image = ((System.Drawing.Image)(resources.GetObject("label14.Image")));
			this.label14.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label14.Location = new System.Drawing.Point(8, 74);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(104, 16);
			this.label14.TabIndex = 20;
			this.label14.Text = "      TaskFile Tool:";
			this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// MultiPackageControl
			// 
			this.Controls.Add(this.PropertiesMultiAssetPanel);
			this.Name = "MultiPackageControl";
			this.Size = new System.Drawing.Size(328, 472);
			this.PropertiesMultiAssetPanel.ResumeLayout(false);
			this.PackageWorkFolderGroupBox.ResumeLayout(false);
			this.PropertiesMultiPackagingEventsGroupBox.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.PropertiesMultiPackageFileGroupBox.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

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

		private void SetCheckBoxProperty(CheckBox EditCheckBox, MOG_InheritedBoolean newVal)
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

			if (mResetWildcardCheck)
			{
				EditCheckBox.CheckState = state;
				EditCheckBox.Tag = null;
			}
			else
			{
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
		}

		public void InitializePackagingInfo(MOG_Properties properties, MOG_Filename currentFilename)
		{
			mDisableEvents = true;
			// Put in the Multi-asset commands
			SetComboBoxProperty(this.PropertiesMultiPakToolComboBox, properties.TaskFileTool, false);
			SetComboBoxProperty(this.PropertiesPackageInputComboBox, properties.InputPackageTaskFile, false);
			SetComboBoxProperty(this.PropertiesPackageOutputComboBox, properties.OutputPackageTaskFile, false);
			
			SetComboBoxProperty(this.PropertiesMultiPakDelComboBox, properties.PackageCommand_DeletePackageFile, false);

			SetComboBoxProperty(this.PropertiesMultiResolverComboBox, properties.PackageCommand_ResolveLateResolvers, false);
			SetComboBoxProperty(this.PropertiesMultiWorkDirComboBox, properties.PackageWorkspaceDirectory, false);

			SetComboBoxProperty(this.PropertiesPackageEventsPreComboBox, properties.PackagePreMergeEvent, false);
			SetComboBoxProperty(this.PropertiesPackageEventsPostComboBox, properties.PackagePostMergeEvent, false);

			SetCheckBoxProperty(this.PropertiesMultiClusterCheckBox, properties.ClusterPackaging_InheritedBoolean);
			SetCheckBoxProperty(this.PropertiesMultiCleanWorkDirCheckBox, properties.CleanupPackageWorkspaceDirectory_InheritedBoolean);

			mResetWildcardCheck = false;
			mDisableEvents = false;
		}

		public void IntializeWizardSettings(ArrayList commands)
		{
			foreach (WizardSetting setting in commands)
			{
				switch(setting.Command)
				{
					case "PackageDeletePackageFile":
						SetComboBoxProperty(this.PropertiesMultiPakDelComboBox, setting.Setting, false);
						break;
				}
			}
		}

		public void SavePackagingChanges(MOG_Properties properties)
		{			
			switch(this.PropertiesMultiClusterCheckBox.CheckState)
			{
				case CheckState.Checked:
					properties.ClusterPackaging_InheritedBoolean = MOG.PROPERTIES.MOG_InheritedBoolean.True;
					break;
				case CheckState.Unchecked:
					properties.ClusterPackaging_InheritedBoolean = MOG.PROPERTIES.MOG_InheritedBoolean.False;
					break;
				case CheckState.Indeterminate:
					properties.ClusterPackaging_InheritedBoolean = MOG.PROPERTIES.MOG_InheritedBoolean.Inherited;
					break;
			}

			switch(this.PropertiesMultiCleanWorkDirCheckBox.CheckState)
			{
				case CheckState.Checked:
					properties.CleanupPackageWorkspaceDirectory_InheritedBoolean = MOG.PROPERTIES.MOG_InheritedBoolean.True;
					break;
				case CheckState.Unchecked:
					properties.CleanupPackageWorkspaceDirectory_InheritedBoolean = MOG.PROPERTIES.MOG_InheritedBoolean.False;
					break;
				case CheckState.Indeterminate:
					properties.CleanupPackageWorkspaceDirectory_InheritedBoolean = MOG.PROPERTIES.MOG_InheritedBoolean.Inherited;
					break;
			}
		}
		#region ComboBox Edited and validated events

		private void PropertiesActivateControl_Click(object sender, System.EventArgs e)
		{
			this.ActiveControl = sender as Control;
		}

		/// <summary>
		/// Save property on validate
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PropertiesPackageInputComboBox_Leave(object sender, System.EventArgs e)
		{
			foreach (MOG_Properties properties in this.PropertiesList)
			{
				properties.InputPackageTaskFile = this.PropertiesPackageInputComboBox.Text;
			}
		}
		/// <summary>
		/// Load property on validate
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PropertiesPackageInputComboBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			foreach (MOG_Properties properties in this.PropertiesList)
			{
				SetComboBoxProperty(this.PropertiesPackageInputComboBox, properties.InputPackageTaskFile, true);
			}
		}

		

		private void PropertiesPackageOutputComboBox_Leave(object sender, System.EventArgs e)
		{
			foreach (MOG_Properties properties in this.PropertiesList)
			{
				properties.OutputPackageTaskFile = this.PropertiesPackageOutputComboBox.Text;
			}
		}
		private void PropertiesPackageOutputComboBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			foreach (MOG_Properties properties in this.PropertiesList)
			{
				SetComboBoxProperty(this.PropertiesPackageOutputComboBox, properties.OutputPackageTaskFile, true);
			}
		}


		private void PropertiesMultiPakToolComboBox_Leave(object sender, System.EventArgs e)
		{
			foreach (MOG_Properties properties in this.PropertiesList)
			{
				properties.TaskFileTool = this.PropertiesMultiPakToolComboBox.Text;
			}
		}
		private void PropertiesMultiPakToolComboBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			foreach (MOG_Properties properties in this.PropertiesList)
			{	if(properties != null)																					{
				SetComboBoxProperty(this.PropertiesMultiPakToolComboBox, properties.TaskFileTool, true);				}
			}
		}



		private void PropertiesPackageEventsPreComboBox_Leave(object sender, System.EventArgs e)
		{
			foreach (MOG_Properties properties in this.PropertiesList)
			{
				properties.PackagePreMergeEvent = this.PropertiesPackageEventsPreComboBox.Text;
			}
		}
		private void PropertiesPackageEventsPreComboBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			foreach (MOG_Properties properties in this.PropertiesList)
			{
				SetComboBoxProperty(this.PropertiesPackageEventsPreComboBox, properties.PackagePreMergeEvent, true);
			}
		}

		
		
		private void PropertiesPackageEventsPostComboBox_Leave(object sender, System.EventArgs e)
		{
			foreach (MOG_Properties properties in this.PropertiesList)
			{
				properties.PackagePostMergeEvent = this.PropertiesPackageEventsPostComboBox.Text;
			}
		}
		private void PropertiesPackageEventsPostComboBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			foreach (MOG_Properties properties in this.PropertiesList)
			{
				SetComboBoxProperty(this.PropertiesPackageEventsPostComboBox, properties.PackagePostMergeEvent, true);
			}
		}



		private void PropertiesMultiResolverComboBox_Leave(object sender, System.EventArgs e)
		{
			foreach (MOG_Properties properties in this.PropertiesList)
			{
				properties.PackageCommand_ResolveLateResolvers = this.PropertiesMultiResolverComboBox.Text;
			}
		}
		private void PropertiesMultiResolverComboBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			foreach (MOG_Properties properties in this.PropertiesList)
			{
				SetComboBoxProperty(this.PropertiesMultiResolverComboBox, properties.PackageCommand_ResolveLateResolvers, true);
			}
		}

		
		
		private void PropertiesMultiPakDelComboBox_Leave(object sender, System.EventArgs e)
		{
			foreach (MOG_Properties properties in this.PropertiesList)
			{
				properties.PackageCommand_DeletePackageFile = this.PropertiesMultiPakDelComboBox.Text;
			}
		}
		private void PropertiesMultiPakDelComboBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			foreach (MOG_Properties properties in this.PropertiesList)
			{
				SetComboBoxProperty(this.PropertiesMultiPakDelComboBox, properties.PackageCommand_DeletePackageFile, true);
			}
		}



		private void PropertiesMultiWorkDirComboBox_Leave(object sender, System.EventArgs e)
		{
			foreach (MOG_Properties properties in this.PropertiesList)
			{
				properties.PackageWorkspaceDirectory = this.PropertiesMultiWorkDirComboBox.Text;
			}
		}
		private void PropertiesMultiWorkDirComboBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			foreach (MOG_Properties properties in this.PropertiesList)
			{
				SetComboBoxProperty(this.PropertiesMultiWorkDirComboBox, properties.PackageWorkspaceDirectory, true);
			}
		}
		#endregion

		private void Properties_PropertyChanged(object sender, System.EventArgs e)
		{
			// Inform any delegates of this event
			if (ProptertyChanged != null && !mDisableEvents)
			{
				object []args = {sender, e};
				this.Invoke(ProptertyChanged, args);
			}
		}

		private void ExploreMultiWorkDirButton_Click(object sender, System.EventArgs e)
		{
			this.PropertiesMultiWorkDirComboBox.Text = mParent.GetValueForProperty("PackageWorkspaceDirectory", PropertiesMultiWorkDirComboBox.Text) as string;

			foreach (MOG_Properties properties in this.PropertiesList)
			{
				properties.PackageWorkspaceDirectory = this.PropertiesMultiWorkDirComboBox.Text;
			}
		}

		private void ExplorerMultiPakToolButton_Click(object sender, System.EventArgs e)
		{
			this.PropertiesMultiPakToolComboBox.Text = mParent.GetValueForProperty("TaskFileTool", PropertiesMultiPakToolComboBox.Text) as string;

			foreach (MOG_Properties properties in this.PropertiesList)
			{
				properties.TaskFileTool = this.PropertiesMultiPakToolComboBox.Text;
			}
		}

		private void PropertiesPackageEventsPreButton_Click(object sender, System.EventArgs e)
		{
			this.PropertiesPackageEventsPreComboBox.Text = mParent.GetValueForProperty("PackagePreMergeEvent", PropertiesPackageEventsPreComboBox.Text) as string;

			foreach (MOG_Properties properties in this.PropertiesList)
			{
				properties.PackagePreMergeEvent = this.PropertiesPackageEventsPreComboBox.Text;
			}
		}

		private void PropertiesPackageEventsPostButton_Click(object sender, System.EventArgs e)
		{
			this.PropertiesPackageEventsPostComboBox.Text = mParent.GetValueForProperty("PackagePostMergeEvent", PropertiesPackageEventsPostComboBox.Text) as string;

			foreach (MOG_Properties properties in this.PropertiesList)
			{
				properties.PackagePostMergeEvent = this.PropertiesPackageEventsPostComboBox.Text;
			}
		}
	}

	public class WizardSetting
	{
		public string Command;
		public string Setting;

		public WizardSetting(string command, string setting)
		{
			Command = command;
			Setting = setting;
		}
	}
}

