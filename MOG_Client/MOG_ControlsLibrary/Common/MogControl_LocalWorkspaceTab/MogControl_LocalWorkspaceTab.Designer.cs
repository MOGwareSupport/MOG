using MOG_ControlsLibrary;
namespace MOG_Client.Client_Gui.AssetManager_Helper
{
	partial class MogControl_LocalWorkspaceTab
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.panel1 = new System.Windows.Forms.Panel();
			this.TopPanel = new System.Windows.Forms.Panel();
			this.WorkspaceToolStrip = new System.Windows.Forms.ToolStrip();
			this.ToolStripContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.imageOnlyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.imageAndTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.WorkspaceToolStripButton = new System.Windows.Forms.ToolStripSplitButton();
			this.autoUpdateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.alwaysActiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
			this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.infoToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.createTagToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.exploreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.GetActiveToolStripButton = new System.Windows.Forms.ToolStripSplitButton();
			this.GetLatestToolStripButton = new System.Windows.Forms.ToolStripSplitButton();
			this.GetLatestFiltersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.GetLatestTagsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
			this.GetLatestUpdateModifiedMissingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.GetLatestCleanUnknownFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.PackageToolStripDropDownButton = new System.Windows.Forms.ToolStripSplitButton();
			this.autoPackageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolTips = new System.Windows.Forms.ToolTip(this.components);
			this.WorkSpaceListView = new MOG_ControlsLibrary.MogControl_ListView();
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
			this.panel1.SuspendLayout();
			this.TopPanel.SuspendLayout();
			this.WorkspaceToolStrip.SuspendLayout();
			this.ToolStripContextMenuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.Color.LightSteelBlue;
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel1.Controls.Add(this.WorkSpaceListView);
			this.panel1.Controls.Add(this.TopPanel);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(1236, 282);
			this.panel1.TabIndex = 36;
			// 
			// TopPanel
			// 
			this.TopPanel.BackColor = System.Drawing.SystemColors.Control;
			this.TopPanel.Controls.Add(this.WorkspaceToolStrip);
			this.TopPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.TopPanel.Location = new System.Drawing.Point(0, 0);
			this.TopPanel.Name = "TopPanel";
			this.TopPanel.Size = new System.Drawing.Size(1232, 37);
			this.TopPanel.TabIndex = 40;
			// 
			// WorkspaceToolStrip
			// 
			this.WorkspaceToolStrip.AllowItemReorder = true;
			this.WorkspaceToolStrip.BackColor = System.Drawing.Color.Transparent;
			this.WorkspaceToolStrip.ContextMenuStrip = this.ToolStripContextMenuStrip;
			this.WorkspaceToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.WorkspaceToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator3,
            this.WorkspaceToolStripButton,
            this.toolStripSeparator4,
            this.GetActiveToolStripButton,
            this.GetLatestToolStripButton,
            this.PackageToolStripDropDownButton});
			this.WorkspaceToolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
			this.WorkspaceToolStrip.Location = new System.Drawing.Point(0, 0);
			this.WorkspaceToolStrip.Name = "WorkspaceToolStrip";
			this.WorkspaceToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			this.WorkspaceToolStrip.Size = new System.Drawing.Size(1232, 36);
			this.WorkspaceToolStrip.Stretch = true;
			this.WorkspaceToolStrip.TabIndex = 37;
			// 
			// ToolStripContextMenuStrip
			// 
			this.ToolStripContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.imageOnlyToolStripMenuItem,
            this.imageAndTextToolStripMenuItem});
			this.ToolStripContextMenuStrip.Name = "ToolStripContextMenuStrip";
			this.ToolStripContextMenuStrip.Size = new System.Drawing.Size(160, 48);
			// 
			// imageOnlyToolStripMenuItem
			// 
			this.imageOnlyToolStripMenuItem.Name = "imageOnlyToolStripMenuItem";
			this.imageOnlyToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
			this.imageOnlyToolStripMenuItem.Text = "Image only";
			this.imageOnlyToolStripMenuItem.Click += new System.EventHandler(this.imageOnlyToolStripMenuItem_Click);
			// 
			// imageAndTextToolStripMenuItem
			// 
			this.imageAndTextToolStripMenuItem.Name = "imageAndTextToolStripMenuItem";
			this.imageAndTextToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
			this.imageAndTextToolStripMenuItem.Text = "Image and text";
			this.imageAndTextToolStripMenuItem.Click += new System.EventHandler(this.imageAndTextToolStripMenuItem_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(6, 36);
			// 
			// WorkspaceToolStripButton
			// 
			this.WorkspaceToolStripButton.AutoToolTip = false;
			this.WorkspaceToolStripButton.DropDownButtonWidth = 15;
			this.WorkspaceToolStripButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.autoUpdateToolStripMenuItem,
            this.alwaysActiveToolStripMenuItem,
            this.toolStripMenuItem2,
            this.newToolStripMenuItem,
            this.renameToolStripMenuItem,
            this.infoToolStripMenuItem1,
            this.toolStripSeparator1,
            this.removeToolStripMenuItem,
            this.toolStripSeparator2,
            this.createTagToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exploreToolStripMenuItem});
			this.WorkspaceToolStripButton.Image = global::MOG_Client.Properties.Resources.MogExplore;
			this.WorkspaceToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.WorkspaceToolStripButton.Name = "WorkspaceToolStripButton";
			this.WorkspaceToolStripButton.Size = new System.Drawing.Size(76, 22);
			this.WorkspaceToolStripButton.Text = "Properties";
			this.WorkspaceToolStripButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.WorkspaceToolStripButton.ToolTipText = "Workspace Properties";
			this.WorkspaceToolStripButton.ButtonClick += new System.EventHandler(this.WorkspaceToolStripButton_ButtonClick);
			// 
			// autoUpdateToolStripMenuItem
			// 
			this.autoUpdateToolStripMenuItem.Name = "autoUpdateToolStripMenuItem";
			this.autoUpdateToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.autoUpdateToolStripMenuItem.Text = "Auto Update";
			this.autoUpdateToolStripMenuItem.CheckedChanged += new System.EventHandler(this.WorkspaceAutoUpdateToolStripMenuItem_CheckedChanged);
			this.autoUpdateToolStripMenuItem.Click += new System.EventHandler(this.WorkspaceAutoUpdateToolStripMenuItem_Click);
			// 
			// alwaysActiveToolStripMenuItem
			// 
			this.alwaysActiveToolStripMenuItem.Name = "alwaysActiveToolStripMenuItem";
			this.alwaysActiveToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.alwaysActiveToolStripMenuItem.Text = "Always Active";
			this.alwaysActiveToolStripMenuItem.CheckedChanged += new System.EventHandler(this.WorkspaceAlwaysActiveToolStripMenuItem_CheckedChanged);
			this.alwaysActiveToolStripMenuItem.Click += new System.EventHandler(this.WorkspaceAutoUpdateToolStripMenuItem_Click);
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(149, 6);
			// 
			// newToolStripMenuItem
			// 
			this.newToolStripMenuItem.Image = global::MOG_Client.Properties.Resources.WorkspaceNew;
			this.newToolStripMenuItem.Name = "newToolStripMenuItem";
			this.newToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.newToolStripMenuItem.Text = "New";
			this.newToolStripMenuItem.ToolTipText = "Create a new workspace";
			this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
			// 
			// renameToolStripMenuItem
			// 
			this.renameToolStripMenuItem.Image = global::MOG_Client.Properties.Resources.WorkspaceRename;
			this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
			this.renameToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.renameToolStripMenuItem.Text = "Rename";
			this.renameToolStripMenuItem.ToolTipText = "Rename this workspace as shown in its tab";
			this.renameToolStripMenuItem.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
			// 
			// infoToolStripMenuItem1
			// 
			this.infoToolStripMenuItem1.Enabled = false;
			this.infoToolStripMenuItem1.Name = "infoToolStripMenuItem1";
			this.infoToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
			this.infoToolStripMenuItem1.Text = "Info";
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
			// 
			// removeToolStripMenuItem
			// 
			this.removeToolStripMenuItem.Image = global::MOG_Client.Properties.Resources.WorkspaceRemove;
			this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
			this.removeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.removeToolStripMenuItem.Text = "Remove";
			this.removeToolStripMenuItem.ToolTipText = "Remove this workspace";
			this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(149, 6);
			// 
			// createTagToolStripMenuItem
			// 
			this.createTagToolStripMenuItem.Image = global::MOG_Client.Properties.Resources.CreateTag;
			this.createTagToolStripMenuItem.Name = "createTagToolStripMenuItem";
			this.createTagToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.createTagToolStripMenuItem.Text = "Create Tag";
			this.createTagToolStripMenuItem.ToolTipText = "Create a tag from the version timestamps held within this workspace";
			this.createTagToolStripMenuItem.Click += new System.EventHandler(this.createTagToolStripMenuItem_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(149, 6);
			// 
			// exploreToolStripMenuItem
			// 
			this.exploreToolStripMenuItem.Image = global::MOG_Client.Properties.Resources.MogExplore;
			this.exploreToolStripMenuItem.Name = "exploreToolStripMenuItem";
			this.exploreToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.exploreToolStripMenuItem.Text = "Explore";
			this.exploreToolStripMenuItem.ToolTipText = "Explore this workspace";
			this.exploreToolStripMenuItem.Click += new System.EventHandler(this.exploreToolStripMenuItem_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.BackColor = System.Drawing.Color.Transparent;
			this.toolStripSeparator4.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(6, 36);
			// 
			// GetActiveToolStripButton
			// 
			this.GetActiveToolStripButton.DropDownButtonWidth = 15;
			this.GetActiveToolStripButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.GetLatestCleanUnknownFilesToolStripMenuItem,
			this.GetLatestUpdateModifiedMissingToolStripMenuItem});
			this.GetActiveToolStripButton.Image = global::MOG_Client.Properties.Resources.SyncTag;
			this.GetActiveToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.GetActiveToolStripButton.Name = "GetActiveToolStripButton";
			this.GetActiveToolStripButton.Size = new System.Drawing.Size(55, 33);
			this.GetActiveToolStripButton.Text = "Get ()";
			this.GetActiveToolStripButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.GetActiveToolStripButton.DropDownOpening += new System.EventHandler(this.GetActiveToolStripButton_DropDownOpening);
			this.GetActiveToolStripButton.ButtonClick += new System.EventHandler(this.GetActiveToolStripButton_Click);
			// 
			// GetLatestToolStripButton
			// 
			this.GetLatestToolStripButton.AutoToolTip = false;
			this.GetLatestToolStripButton.DropDownButtonWidth = 15;
			this.GetLatestToolStripButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.GetLatestFiltersToolStripMenuItem,
            this.GetLatestTagsToolStripMenuItem,
            this.toolStripMenuItem4,
            this.GetLatestUpdateModifiedMissingToolStripMenuItem,
			this.GetLatestCleanUnknownFilesToolStripMenuItem});
			this.GetLatestToolStripButton.Image = global::MOG_Client.Properties.Resources.Sync;
			this.GetLatestToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.GetLatestToolStripButton.Name = "GetLatestToolStripButton";
			this.GetLatestToolStripButton.Size = new System.Drawing.Size(77, 33);
			this.GetLatestToolStripButton.Text = "Get Latest";
			this.GetLatestToolStripButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.GetLatestToolStripButton.ToolTipText = "Get Latest";
			this.GetLatestToolStripButton.DropDownOpening += new System.EventHandler(this.GetLatestToolStripButton_DropDownOpening);
			this.GetLatestToolStripButton.ButtonClick += new System.EventHandler(this.GetLatestToolStripButton_Click);
			// 
			// GetLatestFiltersToolStripMenuItem
			// 
			this.GetLatestFiltersToolStripMenuItem.Image = global::MOG_Client.Properties.Resources.Filter;
			this.GetLatestFiltersToolStripMenuItem.Name = "GetLatestFiltersToolStripMenuItem";
			this.GetLatestFiltersToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
			this.GetLatestFiltersToolStripMenuItem.Text = "Filter";
			// 
			// GetLatestTagsToolStripMenuItem
			// 
			this.GetLatestTagsToolStripMenuItem.Image = global::MOG_Client.Properties.Resources.Tag;
			this.GetLatestTagsToolStripMenuItem.Name = "GetLatestTagsToolStripMenuItem";
			this.GetLatestTagsToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
			this.GetLatestTagsToolStripMenuItem.Text = "Tag";
			// 
			// toolStripMenuItem4
			// 
			this.toolStripMenuItem4.Name = "toolStripMenuItem4";
			this.toolStripMenuItem4.Size = new System.Drawing.Size(198, 6);
			// 
			// GetLatestUpdateModifiedMissingToolStripMenuItem
			// 
			this.GetLatestUpdateModifiedMissingToolStripMenuItem.Name = "GetLatestUpdateModifiedMissingToolStripMenuItem";
			this.GetLatestUpdateModifiedMissingToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
			this.GetLatestUpdateModifiedMissingToolStripMenuItem.Text = "Update Modified/Missing";
			this.GetLatestUpdateModifiedMissingToolStripMenuItem.ToolTipText = "Update all missing files and revert all modified files.";
			this.GetLatestUpdateModifiedMissingToolStripMenuItem.CheckedChanged += new System.EventHandler(this.GetLatestUpdateModifiedMissingToolStripMenuItem_CheckedChanged);
			this.GetLatestUpdateModifiedMissingToolStripMenuItem.Click += new System.EventHandler(this.GetLatestUpdateModifiedMissingToolStripMenuItem_Click);
			// 
			// GetLatestCleanUnknownFilesToolStripMenuItem
			// 
			this.GetLatestCleanUnknownFilesToolStripMenuItem.Name = "GetLatestCleanUnknownFilesToolStripMenuItem";
			this.GetLatestCleanUnknownFilesToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
			this.GetLatestCleanUnknownFilesToolStripMenuItem.Text = "Clean Unknown Files";
			this.GetLatestCleanUnknownFilesToolStripMenuItem.ToolTipText = "Cleans any unknown files not tracked in the project.";
			this.GetLatestCleanUnknownFilesToolStripMenuItem.CheckedChanged += new System.EventHandler(this.GetLatestCleanUnknownFilesToolStripMenuItem_CheckedChanged);
			this.GetLatestCleanUnknownFilesToolStripMenuItem.Click += new System.EventHandler(this.GetLatestCleanUnknownFilesToolStripMenuItem_Click);
			// 
			// PackageToolStripDropDownButton
			// 
			this.PackageToolStripDropDownButton.DropDownButtonWidth = 15;
			this.PackageToolStripDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.autoPackageToolStripMenuItem});
			this.PackageToolStripDropDownButton.Image = global::MOG_Client.Properties.Resources.MogRepackage;
			this.PackageToolStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.PackageToolStripDropDownButton.Name = "PackageToolStripDropDownButton";
			this.PackageToolStripDropDownButton.Size = new System.Drawing.Size(67, 33);
			this.PackageToolStripDropDownButton.Text = "Package";
			this.PackageToolStripDropDownButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.PackageToolStripDropDownButton.ButtonClick += new System.EventHandler(this.PackageToolStripDropDownButton_ButtonClick);
			// 
			// autoPackageToolStripMenuItem
			// 
			this.autoPackageToolStripMenuItem.Checked = true;
			this.autoPackageToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.autoPackageToolStripMenuItem.Name = "autoPackageToolStripMenuItem";
			this.autoPackageToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
			this.autoPackageToolStripMenuItem.Text = "Auto Package";
			this.autoPackageToolStripMenuItem.ToolTipText = "Automatically package all packaged assets when they are copied into this workspac" +
				"e";
			this.autoPackageToolStripMenuItem.CheckedChanged += new System.EventHandler(this.autoPackageToolStripMenuItem_CheckedChanged);
			this.autoPackageToolStripMenuItem.Click += new System.EventHandler(this.autoPackageToolStripMenuItem_Click);
			// 
			// WorkSpaceListView
			// 
			this.WorkSpaceListView.AllowColumnReorder = true;
			this.WorkSpaceListView.AllowDrop = true;
			this.WorkSpaceListView.AutoArrange = false;
			this.WorkSpaceListView.BackColor = System.Drawing.SystemColors.Window;
			this.WorkSpaceListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
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
			this.WorkSpaceListView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.WorkSpaceListView.FullRowSelect = true;
			this.WorkSpaceListView.LabelWrap = false;
			this.WorkSpaceListView.Location = new System.Drawing.Point(0, 37);
			this.WorkSpaceListView.Name = "WorkSpaceListView";
			this.WorkSpaceListView.ShowGroups = false;
			this.WorkSpaceListView.Size = new System.Drawing.Size(1232, 241);
			this.WorkSpaceListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.WorkSpaceListView.TabIndex = 33;
			this.WorkSpaceListView.UseCompatibleStateImageBehavior = false;
			this.WorkSpaceListView.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Name";
			this.columnHeader1.Width = 210;
			// 
			// columnHeader7
			// 
			this.columnHeader7.Text = "MOG Classification";
			this.columnHeader7.Width = 0;
			// 
			// columnHeader24
			// 
			this.columnHeader24.Text = "TargetPath";
			this.columnHeader24.Width = 0;
			// 
			// columnHeader15
			// 
			this.columnHeader15.Text = "Date";
			this.columnHeader15.Width = 119;
			// 
			// columnHeader16
			// 
			this.columnHeader16.Text = "Size";
			this.columnHeader16.Width = 91;
			// 
			// columnHeader17
			// 
			this.columnHeader17.Text = "Platform";
			this.columnHeader17.Width = 0;
			// 
			// columnHeader18
			// 
			this.columnHeader18.Text = "State";
			this.columnHeader18.Width = 100;
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
			// MogControl_LocalWorkspaceTab
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel1);
			this.Name = "MogControl_LocalWorkspaceTab";
			this.Size = new System.Drawing.Size(1236, 282);
			this.panel1.ResumeLayout(false);
			this.TopPanel.ResumeLayout(false);
			this.TopPanel.PerformLayout();
			this.WorkspaceToolStrip.ResumeLayout(false);
			this.WorkspaceToolStrip.PerformLayout();
			this.ToolStripContextMenuStrip.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		public MogControl_ListView WorkSpaceListView;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader7;
		private System.Windows.Forms.ColumnHeader columnHeader24;
		private System.Windows.Forms.ColumnHeader columnHeader15;
		private System.Windows.Forms.ColumnHeader columnHeader16;
		private System.Windows.Forms.ColumnHeader columnHeader17;
		private System.Windows.Forms.ColumnHeader columnHeader18;
		private System.Windows.Forms.ColumnHeader columnHeader19;
		private System.Windows.Forms.ColumnHeader columnHeader20;
		private System.Windows.Forms.ColumnHeader columnHeader21;
		private System.Windows.Forms.ColumnHeader columnHeader22;
		private System.Windows.Forms.ColumnHeader columnHeader23;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.ToolTip ToolTips;
		private System.Windows.Forms.ToolStrip WorkspaceToolStrip;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.Panel TopPanel;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripSplitButton WorkspaceToolStripButton;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripSplitButton PackageToolStripDropDownButton;
		private System.Windows.Forms.ToolStripMenuItem autoPackageToolStripMenuItem;
		private System.Windows.Forms.ToolStripSplitButton GetActiveToolStripButton;
		private System.Windows.Forms.ToolStripSplitButton GetLatestToolStripButton;
        private System.Windows.Forms.ToolStripMenuItem GetLatestFiltersToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
		private System.Windows.Forms.ToolStripMenuItem GetLatestUpdateModifiedMissingToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem GetLatestCleanUnknownFilesToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip ToolStripContextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem imageOnlyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem imageAndTextToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem infoToolStripMenuItem1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem createTagToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem autoUpdateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem alwaysActiveToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem GetLatestTagsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exploreToolStripMenuItem;
	}
}
