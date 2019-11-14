namespace MOG_ControlsLibrary.Controls
{
	partial class MogControl_SyncTreeFilter
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
			this.SyncFilterComboBox = new System.Windows.Forms.ComboBox();
			this.SyncFilterLabel = new System.Windows.Forms.Label();
			this.SyncToolTips = new System.Windows.Forms.ToolTip(this.components);
			this.SyncShowAssetsCheckBox = new System.Windows.Forms.CheckBox();
			this.ClassificationTreeView = new MOG_ControlsLibrary.Controls.MogControl_SyncTreeView();
			this.CloseButton = new System.Windows.Forms.Button();
			this.SyncConfigureButton = new System.Windows.Forms.Button();
			this.SyncSaveButton = new System.Windows.Forms.Button();
			this.SyncPromoteButton = new System.Windows.Forms.Button();
			this.SyncAddButton = new System.Windows.Forms.Button();
			this.SyncDelButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// SyncFilterComboBox
			// 
			this.SyncFilterComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.SyncFilterComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.SyncFilterComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
			this.SyncFilterComboBox.Enabled = false;
			this.SyncFilterComboBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.SyncFilterComboBox.FormattingEnabled = true;
			this.SyncFilterComboBox.Location = new System.Drawing.Point(15, 50);
			this.SyncFilterComboBox.Name = "SyncFilterComboBox";
			this.SyncFilterComboBox.Size = new System.Drawing.Size(274, 21);
			this.SyncFilterComboBox.TabIndex = 1;
			this.SyncToolTips.SetToolTip(this.SyncFilterComboBox, "Select a filter preset");
			this.SyncFilterComboBox.SelectedIndexChanged += new System.EventHandler(this.SyncFilterComboBox_SelectedIndexChanged);
			// 
			// SyncFilterLabel
			// 
			this.SyncFilterLabel.AutoSize = true;
			this.SyncFilterLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.SyncFilterLabel.ForeColor = System.Drawing.SystemColors.ControlText;
			this.SyncFilterLabel.Location = new System.Drawing.Point(12, 32);
			this.SyncFilterLabel.Name = "SyncFilterLabel";
			this.SyncFilterLabel.Size = new System.Drawing.Size(208, 13);
			this.SyncFilterLabel.TabIndex = 2;
			this.SyncFilterLabel.Text = "Select desired sync filter:     (Close to sync)";
			this.SyncFilterLabel.Visible = false;
			// 
			// SyncShowAssetsCheckBox
			// 
			this.SyncShowAssetsCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.SyncShowAssetsCheckBox.AutoSize = true;
			this.SyncShowAssetsCheckBox.Location = new System.Drawing.Point(178, 293);
			this.SyncShowAssetsCheckBox.Name = "SyncShowAssetsCheckBox";
			this.SyncShowAssetsCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.SyncShowAssetsCheckBox.Size = new System.Drawing.Size(87, 17);
			this.SyncShowAssetsCheckBox.TabIndex = 9;
			this.SyncShowAssetsCheckBox.Text = "Show Assets";
			this.SyncShowAssetsCheckBox.UseVisualStyleBackColor = true;
			this.SyncShowAssetsCheckBox.Visible = false;
			this.SyncShowAssetsCheckBox.Validated += new System.EventHandler(this.SyncShowAssetsCheckBox_Validated);
			this.SyncShowAssetsCheckBox.CheckedChanged += new System.EventHandler(this.SyncShowAssetsCheckBox_CheckedChanged);
			// 
			// ClassificationTreeView
			// 
			this.ClassificationTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.ClassificationTreeView.ArchivedNodeForeColor = System.Drawing.SystemColors.WindowText;
			this.ClassificationTreeView.CheckBoxes = true;
			this.ClassificationTreeView.ExclusionList = null;
			this.ClassificationTreeView.ExpandAssets = false;
			this.ClassificationTreeView.ExpandPackageGroupAssets = false;
			this.ClassificationTreeView.ExpandPackageGroups = false;
			this.ClassificationTreeView.FocusForAssetNodes = MOG_ControlsLibrary.Controls.LeafFocusLevel.RepositoryItems;
			this.ClassificationTreeView.HotTracking = true;
			this.ClassificationTreeView.ImageIndex = 0;
			this.ClassificationTreeView.LastNodePath = null;
			this.ClassificationTreeView.Location = new System.Drawing.Point(15, 77);
			this.ClassificationTreeView.Name = "ClassificationTreeView";
			this.ClassificationTreeView.NodesDefaultToChecked = true;
			this.ClassificationTreeView.PathSeparator = "~";
			this.ClassificationTreeView.PersistantHighlightSelectedNode = false;
			this.ClassificationTreeView.SelectedImageIndex = 0;
			this.ClassificationTreeView.ShowAssets = false;
			this.ClassificationTreeView.ShowDescription = false;
			this.ClassificationTreeView.ShowPlatformSpecific = false;
			this.ClassificationTreeView.ShowToolTips = false;
			this.ClassificationTreeView.Size = new System.Drawing.Size(274, 206);
			this.ClassificationTreeView.TabIndex = 3;
			this.ClassificationTreeView.Visible = false;
			this.ClassificationTreeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.ClassificationTreeView_AfterCheck);
			// 
			// CloseButton
			// 
			this.CloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.CloseButton.Image = global::MOG_ControlsLibrary.Properties.Resources.Close;
			this.CloseButton.Location = new System.Drawing.Point(281, 2);
			this.CloseButton.Name = "CloseButton";
			this.CloseButton.Size = new System.Drawing.Size(23, 23);
			this.CloseButton.TabIndex = 10;
			this.CloseButton.UseVisualStyleBackColor = true;
			this.CloseButton.Visible = false;
			this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
			// 
			// SyncConfigureButton
			// 
			this.SyncConfigureButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.SyncConfigureButton.Image = global::MOG_ControlsLibrary.Properties.Resources.Filter;
			this.SyncConfigureButton.Location = new System.Drawing.Point(15, 11);
			this.SyncConfigureButton.Name = "SyncConfigureButton";
			this.SyncConfigureButton.Size = new System.Drawing.Size(274, 23);
			this.SyncConfigureButton.TabIndex = 8;
			this.SyncConfigureButton.Text = "Configure Filters";
			this.SyncConfigureButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.SyncConfigureButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.SyncToolTips.SetToolTip(this.SyncConfigureButton, "Configure filters");
			this.SyncConfigureButton.UseVisualStyleBackColor = true;
			this.SyncConfigureButton.Click += new System.EventHandler(this.SyncConfigureButton_Click);
			// 
			// SyncSaveButton
			// 
			this.SyncSaveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.SyncSaveButton.Enabled = false;
			this.SyncSaveButton.Image = global::MOG_ControlsLibrary.Properties.Resources.Save;
			this.SyncSaveButton.Location = new System.Drawing.Point(266, 289);
			this.SyncSaveButton.Name = "SyncSaveButton";
			this.SyncSaveButton.Size = new System.Drawing.Size(23, 23);
			this.SyncSaveButton.TabIndex = 4;
			this.SyncToolTips.SetToolTip(this.SyncSaveButton, "Update this filter with these new changes");
			this.SyncSaveButton.UseVisualStyleBackColor = true;
			this.SyncSaveButton.Visible = false;
			this.SyncSaveButton.Click += new System.EventHandler(this.SyncSaveButton_Click);
			// 
			// SyncPromoteButton
			// 
			this.SyncPromoteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.SyncPromoteButton.Enabled = false;
			this.SyncPromoteButton.Image = global::MOG_ControlsLibrary.Properties.Resources.Up;
			this.SyncPromoteButton.Location = new System.Drawing.Point(218, 48);
			this.SyncPromoteButton.Name = "SyncPromoteButton";
			this.SyncPromoteButton.Size = new System.Drawing.Size(23, 23);
			this.SyncPromoteButton.TabIndex = 7;
			this.SyncToolTips.SetToolTip(this.SyncPromoteButton, "Promote this filter to the team");
			this.SyncPromoteButton.UseVisualStyleBackColor = true;
			this.SyncPromoteButton.Visible = false;
			this.SyncPromoteButton.Click += new System.EventHandler(this.SyncPromoteButton_Click);
			// 
			// SyncAddButton
			// 
			this.SyncAddButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.SyncAddButton.Image = global::MOG_ControlsLibrary.Properties.Resources.Add;
			this.SyncAddButton.Location = new System.Drawing.Point(242, 48);
			this.SyncAddButton.Name = "SyncAddButton";
			this.SyncAddButton.Size = new System.Drawing.Size(23, 23);
			this.SyncAddButton.TabIndex = 6;
			this.SyncToolTips.SetToolTip(this.SyncAddButton, "Add this tree layout as a new filter...");
			this.SyncAddButton.UseVisualStyleBackColor = true;
			this.SyncAddButton.Visible = false;
			this.SyncAddButton.Click += new System.EventHandler(this.SyncAddButton_Click);
			// 
			// SyncDelButton
			// 
			this.SyncDelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.SyncDelButton.Image = global::MOG_ControlsLibrary.Properties.Resources.Remove;
			this.SyncDelButton.Location = new System.Drawing.Point(266, 48);
			this.SyncDelButton.Name = "SyncDelButton";
			this.SyncDelButton.Size = new System.Drawing.Size(23, 23);
			this.SyncDelButton.TabIndex = 5;
			this.SyncToolTips.SetToolTip(this.SyncDelButton, "Delete this filter");
			this.SyncDelButton.UseVisualStyleBackColor = true;
			this.SyncDelButton.Visible = false;
			this.SyncDelButton.Click += new System.EventHandler(this.SyncDelButton_Click);
			// 
			// MogControl_SyncTreeFilter
			// 
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(228)))), ((int)(((byte)(239)))));
			this.Controls.Add(this.SyncFilterLabel);
			this.Controls.Add(this.CloseButton);
			this.Controls.Add(this.SyncShowAssetsCheckBox);
			this.Controls.Add(this.SyncConfigureButton);
			this.Controls.Add(this.SyncSaveButton);
			this.Controls.Add(this.ClassificationTreeView);
			this.Controls.Add(this.SyncFilterComboBox);
			this.Controls.Add(this.SyncPromoteButton);
			this.Controls.Add(this.SyncAddButton);
			this.Controls.Add(this.SyncDelButton);
			this.MinimumSize = new System.Drawing.Size(150, 120);
			this.Name = "MogControl_SyncTreeFilter";
			this.Size = new System.Drawing.Size(305, 318);
			this.Load += new System.EventHandler(this.MogControl_SyncTreeFilter_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox SyncFilterComboBox;
		private System.Windows.Forms.Label SyncFilterLabel;
		private MOG_ControlsLibrary.Controls.MogControl_SyncTreeView ClassificationTreeView;
		private System.Windows.Forms.Button SyncSaveButton;
		private System.Windows.Forms.Button SyncDelButton;
		private System.Windows.Forms.Button SyncAddButton;
		private System.Windows.Forms.Button SyncPromoteButton;
		private System.Windows.Forms.ToolTip SyncToolTips;
		private System.Windows.Forms.Button SyncConfigureButton;
		private System.Windows.Forms.CheckBox SyncShowAssetsCheckBox;
		private System.Windows.Forms.Button CloseButton;
	}
}
