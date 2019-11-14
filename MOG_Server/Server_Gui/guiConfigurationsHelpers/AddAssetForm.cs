using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using MOG;
using MOG.INI;
using MOG.USER;
using MOG.FILENAME;
using MOG.PROJECT;
using MOG.ASSETINFO;

namespace MOG_Server.Server_Gui.guiConfigurationsHelpers
{
	/// <summary>
	/// Summary description for AddAssetForm.
	/// </summary>
	public class AddAssetForm : System.Windows.Forms.Form
	{
		#region User definitions
		BASE mog;
		
		public string AssetName { get {return NameTextBox.Text;} }
		public string Description { get {return DescriptionTextBox.Text;} }
		public string RipTasker { get {return RipTaskerTextBox.Text;} }
		public string Ripper { get {return RipperTextBox.Text;} }
		public string FormatString { get {return FormatStringTextBox.Text;} }
		public string BlessFormatString { get {return BlessFormatStringTextBox.Text;} }
		public string AssetIcon { get {return IconTextBox.Text;} }
		public string Package { get {return PackageNameTextBox.Text;} }
		public string Viewer { get {return this.ViewerTextBox.Text;} }
		public bool CanBePackaged { get {return PackageCheckBox.Checked;} }
		public bool CanBeUpdated { get {return UpdatableCheckBox.Checked;} }
		public bool IsNativeDataType { get {return NativeCheckBox.Checked;} }
		public bool DivergentData { get {return DivergentDataCheckBox.Checked;} }

		private ArrayList validMogTokens;

		#endregion

		#region System definitions

		private System.Windows.Forms.Button btnIconBrowse;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.TextBox RipTaskerTextBox;
		private System.Windows.Forms.TextBox RipperTextBox;
		private System.Windows.Forms.TextBox DescriptionTextBox;
		private System.Windows.Forms.TextBox NameTextBox;
		private System.Windows.Forms.CheckBox PackageCheckBox;
		private System.Windows.Forms.Label NameLabel;
		private System.Windows.Forms.Label DescriptionLabel;
		private System.Windows.Forms.Label RipperLabel;
		private System.Windows.Forms.Label RipTaskerLabel;
		private System.Windows.Forms.Label IconLabel;
		private System.Windows.Forms.Label FormatLabel;
		private System.Windows.Forms.TextBox IconTextBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox FormatStringTextBox;
		private System.Windows.Forms.CheckBox UpdatableCheckBox;
		private System.Windows.Forms.CheckBox NativeCheckBox;
		private System.Windows.Forms.TextBox BlessFormatStringTextBox;
		private System.Windows.Forms.TextBox ViewerTextBox;
		private System.Windows.Forms.TextBox PackageNameTextBox;
		private System.Windows.Forms.GroupBox DescriptionGroupBox;
		private System.Windows.Forms.GroupBox OrganizationGroupBox;
		private System.Windows.Forms.Button btnRipperBrowse;
		private System.Windows.Forms.Button btnRipTaskerBrowse;
		private System.Windows.Forms.Button btnViewerBrowse;
		private System.Windows.Forms.Label ViewerLabel;
		private System.Windows.Forms.Label PackageNameLabel;
		private System.Windows.Forms.PictureBox IconPictureBox;
		private System.Windows.Forms.Button btnBlessFormatStringSubmenu;
		private System.Windows.Forms.Button btnFormatStringSubmenu;
		private System.Windows.Forms.Label ExpandedFormatStringLabel;
		private System.Windows.Forms.Label ExpandedBlessFormatStringLabel;
		private System.Windows.Forms.GroupBox ViewerGroupBox;
		private System.Windows.Forms.GroupBox CustomRipperToolGroupBox;
		private System.Windows.Forms.Label FormatStringRootLabel;
		private System.Windows.Forms.Label BlessFormatStringRootLabel;
		private System.Windows.Forms.ToolTip NewAssetToolTips;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.CheckBox DivergentDataCheckBox;
		private System.ComponentModel.IContainer components;

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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(AddAssetForm));
			this.btnIconBrowse = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.FormatStringTextBox = new System.Windows.Forms.TextBox();
			this.RipTaskerTextBox = new System.Windows.Forms.TextBox();
			this.RipperTextBox = new System.Windows.Forms.TextBox();
			this.DescriptionTextBox = new System.Windows.Forms.TextBox();
			this.NameTextBox = new System.Windows.Forms.TextBox();
			this.PackageCheckBox = new System.Windows.Forms.CheckBox();
			this.NameLabel = new System.Windows.Forms.Label();
			this.DescriptionLabel = new System.Windows.Forms.Label();
			this.RipperLabel = new System.Windows.Forms.Label();
			this.RipTaskerLabel = new System.Windows.Forms.Label();
			this.IconLabel = new System.Windows.Forms.Label();
			this.FormatLabel = new System.Windows.Forms.Label();
			this.IconTextBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.UpdatableCheckBox = new System.Windows.Forms.CheckBox();
			this.ViewerLabel = new System.Windows.Forms.Label();
			this.PackageNameLabel = new System.Windows.Forms.Label();
			this.NativeCheckBox = new System.Windows.Forms.CheckBox();
			this.BlessFormatStringTextBox = new System.Windows.Forms.TextBox();
			this.ViewerTextBox = new System.Windows.Forms.TextBox();
			this.PackageNameTextBox = new System.Windows.Forms.TextBox();
			this.IconPictureBox = new System.Windows.Forms.PictureBox();
			this.DescriptionGroupBox = new System.Windows.Forms.GroupBox();
			this.CustomRipperToolGroupBox = new System.Windows.Forms.GroupBox();
			this.button3 = new System.Windows.Forms.Button();
			this.btnRipTaskerBrowse = new System.Windows.Forms.Button();
			this.btnRipperBrowse = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.btnViewerBrowse = new System.Windows.Forms.Button();
			this.OrganizationGroupBox = new System.Windows.Forms.GroupBox();
			this.BlessFormatStringRootLabel = new System.Windows.Forms.Label();
			this.ExpandedBlessFormatStringLabel = new System.Windows.Forms.Label();
			this.ExpandedFormatStringLabel = new System.Windows.Forms.Label();
			this.btnFormatStringSubmenu = new System.Windows.Forms.Button();
			this.btnBlessFormatStringSubmenu = new System.Windows.Forms.Button();
			this.FormatStringRootLabel = new System.Windows.Forms.Label();
			this.ViewerGroupBox = new System.Windows.Forms.GroupBox();
			this.button1 = new System.Windows.Forms.Button();
			this.NewAssetToolTips = new System.Windows.Forms.ToolTip(this.components);
			this.DivergentDataCheckBox = new System.Windows.Forms.CheckBox();
			this.DescriptionGroupBox.SuspendLayout();
			this.CustomRipperToolGroupBox.SuspendLayout();
			this.OrganizationGroupBox.SuspendLayout();
			this.ViewerGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnIconBrowse
			// 
			this.btnIconBrowse.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.btnIconBrowse.Image = ((System.Drawing.Image)(resources.GetObject("btnIconBrowse.Image")));
			this.btnIconBrowse.Location = new System.Drawing.Point(496, 80);
			this.btnIconBrowse.Name = "btnIconBrowse";
			this.btnIconBrowse.Size = new System.Drawing.Size(24, 23);
			this.btnIconBrowse.TabIndex = 11;
			this.NewAssetToolTips.SetToolTip(this.btnIconBrowse, "Browse for an icon image file");
			this.btnIconBrowse.Click += new System.EventHandler(this.btnIconBrowse_Click);
			// 
			// btnOK
			// 
			this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnOK.Location = new System.Drawing.Point(792, 620);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(67, 24);
			this.btnOK.TabIndex = 60;
			this.btnOK.Text = "OK";
			this.NewAssetToolTips.SetToolTip(this.btnOK, "Accept options specified and create the new asset type");
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnCancel.Location = new System.Drawing.Point(712, 620);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(67, 24);
			this.btnCancel.TabIndex = 65;
			this.btnCancel.Text = "Cancel";
			this.NewAssetToolTips.SetToolTip(this.btnCancel, "Cancel the creation of this asset type");
			// 
			// FormatStringTextBox
			// 
			this.FormatStringTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.FormatStringTextBox.Location = new System.Drawing.Point(216, 40);
			this.FormatStringTextBox.Name = "FormatStringTextBox";
			this.FormatStringTextBox.Size = new System.Drawing.Size(432, 20);
			this.FormatStringTextBox.TabIndex = 40;
			this.FormatStringTextBox.Text = "";
			this.NewAssetToolTips.SetToolTip(this.FormatStringTextBox, "The format string describes where assets of this type will be copied to in your l" +
				"ocal copy of the project");
			this.FormatStringTextBox.TextChanged += new System.EventHandler(this.FormatStringTextBox_TextChanged);
			// 
			// RipTaskerTextBox
			// 
			this.RipTaskerTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.RipTaskerTextBox.Location = new System.Drawing.Point(208, 48);
			this.RipTaskerTextBox.Name = "RipTaskerTextBox";
			this.RipTaskerTextBox.Size = new System.Drawing.Size(272, 20);
			this.RipTaskerTextBox.TabIndex = 20;
			this.RipTaskerTextBox.Text = "None";
			this.NewAssetToolTips.SetToolTip(this.RipTaskerTextBox, "Select the location of a custom rip tasker that organizes the ripper\'s activity");
			// 
			// RipperTextBox
			// 
			this.RipperTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.RipperTextBox.Location = new System.Drawing.Point(208, 24);
			this.RipperTextBox.Name = "RipperTextBox";
			this.RipperTextBox.Size = new System.Drawing.Size(272, 20);
			this.RipperTextBox.TabIndex = 15;
			this.RipperTextBox.Text = "None";
			this.NewAssetToolTips.SetToolTip(this.RipperTextBox, "Select the location of an external ripper program that converts this asset type f" +
				"rom the format it is created in to the format useable by the game");
			// 
			// DescriptionTextBox
			// 
			this.DescriptionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.DescriptionTextBox.Location = new System.Drawing.Point(200, 56);
			this.DescriptionTextBox.Name = "DescriptionTextBox";
			this.DescriptionTextBox.Size = new System.Drawing.Size(280, 20);
			this.DescriptionTextBox.TabIndex = 5;
			this.DescriptionTextBox.Text = "Description";
			this.NewAssetToolTips.SetToolTip(this.DescriptionTextBox, "A description of the asset type");
			// 
			// NameTextBox
			// 
			this.NameTextBox.AccessibleDescription = "";
			this.NameTextBox.AccessibleName = "";
			this.NameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.NameTextBox.Location = new System.Drawing.Point(200, 32);
			this.NameTextBox.Name = "NameTextBox";
			this.NameTextBox.Size = new System.Drawing.Size(280, 20);
			this.NameTextBox.TabIndex = 1;
			this.NameTextBox.Text = "Name";
			this.NewAssetToolTips.SetToolTip(this.NameTextBox, "The name that this asset type will be known by in the current project");
			// 
			// PackageCheckBox
			// 
			this.PackageCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.PackageCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PackageCheckBox.Location = new System.Drawing.Point(216, 208);
			this.PackageCheckBox.Name = "PackageCheckBox";
			this.PackageCheckBox.Size = new System.Drawing.Size(144, 16);
			this.PackageCheckBox.TabIndex = 55;
			this.PackageCheckBox.Text = "Can be packaged?";
			this.NewAssetToolTips.SetToolTip(this.PackageCheckBox, "Check to indicate that this asset type can be packaged (implies that this asset t" +
				"ype cannot be updated)");
			this.PackageCheckBox.CheckedChanged += new System.EventHandler(this.PackageCheckBox_CheckedChanged);
			// 
			// NameLabel
			// 
			this.NameLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.NameLabel.Location = new System.Drawing.Point(88, 32);
			this.NameLabel.Name = "NameLabel";
			this.NameLabel.Size = new System.Drawing.Size(104, 16);
			this.NameLabel.TabIndex = 26;
			this.NameLabel.Text = "Asset name";
			this.NameLabel.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// DescriptionLabel
			// 
			this.DescriptionLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.DescriptionLabel.Location = new System.Drawing.Point(88, 56);
			this.DescriptionLabel.Name = "DescriptionLabel";
			this.DescriptionLabel.Size = new System.Drawing.Size(104, 16);
			this.DescriptionLabel.TabIndex = 27;
			this.DescriptionLabel.Text = "Asset description";
			this.DescriptionLabel.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// RipperLabel
			// 
			this.RipperLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.RipperLabel.Location = new System.Drawing.Point(96, 24);
			this.RipperLabel.Name = "RipperLabel";
			this.RipperLabel.Size = new System.Drawing.Size(104, 16);
			this.RipperLabel.TabIndex = 28;
			this.RipperLabel.Text = "Asset ripper";
			this.RipperLabel.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// RipTaskerLabel
			// 
			this.RipTaskerLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.RipTaskerLabel.Location = new System.Drawing.Point(96, 48);
			this.RipTaskerLabel.Name = "RipTaskerLabel";
			this.RipTaskerLabel.Size = new System.Drawing.Size(104, 16);
			this.RipTaskerLabel.TabIndex = 29;
			this.RipTaskerLabel.Text = "Asset rip tasker";
			this.RipTaskerLabel.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// IconLabel
			// 
			this.IconLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.IconLabel.Location = new System.Drawing.Point(136, 80);
			this.IconLabel.Name = "IconLabel";
			this.IconLabel.Size = new System.Drawing.Size(56, 16);
			this.IconLabel.TabIndex = 30;
			this.IconLabel.Text = "Asset icon";
			this.IconLabel.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// FormatLabel
			// 
			this.FormatLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.FormatLabel.Location = new System.Drawing.Point(112, 40);
			this.FormatLabel.Name = "FormatLabel";
			this.FormatLabel.Size = new System.Drawing.Size(96, 16);
			this.FormatLabel.TabIndex = 31;
			this.FormatLabel.Text = "Format string";
			this.FormatLabel.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// IconTextBox
			// 
			this.IconTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.IconTextBox.Location = new System.Drawing.Point(200, 80);
			this.IconTextBox.Name = "IconTextBox";
			this.IconTextBox.Size = new System.Drawing.Size(280, 20);
			this.IconTextBox.TabIndex = 10;
			this.IconTextBox.Text = "Icon";
			this.NewAssetToolTips.SetToolTip(this.IconTextBox, "Location of an image to serve as an icon.  Upon selection it will be copied to th" +
				"e current project\'s Tools\\Images folder");
			// 
			// label1
			// 
			this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label1.Location = new System.Drawing.Point(96, 96);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(112, 16);
			this.label1.TabIndex = 51;
			this.label1.Text = "Bless format string";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// UpdatableCheckBox
			// 
			this.UpdatableCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.UpdatableCheckBox.Checked = true;
			this.UpdatableCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.UpdatableCheckBox.Location = new System.Drawing.Point(44, 140);
			this.UpdatableCheckBox.Name = "UpdatableCheckBox";
			this.UpdatableCheckBox.Size = new System.Drawing.Size(116, 16);
			this.UpdatableCheckBox.TabIndex = 30;
			this.UpdatableCheckBox.Text = "Can be updated?";
			this.NewAssetToolTips.SetToolTip(this.UpdatableCheckBox, "Check to indicate that this asset type can be dynamically updated (automatically " +
				"disables packaging)");
			this.UpdatableCheckBox.CheckedChanged += new System.EventHandler(this.UpdatableCheckBox_CheckedChanged);
			// 
			// ViewerLabel
			// 
			this.ViewerLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.ViewerLabel.Location = new System.Drawing.Point(96, 32);
			this.ViewerLabel.Name = "ViewerLabel";
			this.ViewerLabel.Size = new System.Drawing.Size(104, 16);
			this.ViewerLabel.TabIndex = 53;
			this.ViewerLabel.Text = "Viewer";
			this.ViewerLabel.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// PackageNameLabel
			// 
			this.PackageNameLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.PackageNameLabel.Enabled = false;
			this.PackageNameLabel.Location = new System.Drawing.Point(104, 176);
			this.PackageNameLabel.Name = "PackageNameLabel";
			this.PackageNameLabel.Size = new System.Drawing.Size(104, 16);
			this.PackageNameLabel.TabIndex = 54;
			this.PackageNameLabel.Text = "Package name";
			this.PackageNameLabel.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// NativeCheckBox
			// 
			this.NativeCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.NativeCheckBox.Checked = true;
			this.NativeCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.NativeCheckBox.Location = new System.Drawing.Point(44, 156);
			this.NativeCheckBox.Name = "NativeCheckBox";
			this.NativeCheckBox.Size = new System.Drawing.Size(116, 16);
			this.NativeCheckBox.TabIndex = 35;
			this.NativeCheckBox.Text = "Native data type?";
			this.NewAssetToolTips.SetToolTip(this.NativeCheckBox, "Check to indicate that this asset type is native, i.e., it does not need any proc" +
				"essing or ripping to be used in the project");
			this.NativeCheckBox.CheckedChanged += new System.EventHandler(this.NativeCheckBox_CheckedChanged);
			// 
			// BlessFormatStringTextBox
			// 
			this.BlessFormatStringTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.BlessFormatStringTextBox.Location = new System.Drawing.Point(216, 96);
			this.BlessFormatStringTextBox.Name = "BlessFormatStringTextBox";
			this.BlessFormatStringTextBox.Size = new System.Drawing.Size(432, 20);
			this.BlessFormatStringTextBox.TabIndex = 45;
			this.BlessFormatStringTextBox.Text = "{ProjectPath}\\Assets\\{AssetKey}\\{AssetClassification}\\{AssetSubClass}\\{AssetName}" +
				"";
			this.NewAssetToolTips.SetToolTip(this.BlessFormatStringTextBox, "The bless format string describes where assets of this type will be blessed (copi" +
				"ed) to in the asset tree");
			this.BlessFormatStringTextBox.TextChanged += new System.EventHandler(this.BlessFormatStringTextBox_TextChanged);
			// 
			// ViewerTextBox
			// 
			this.ViewerTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.ViewerTextBox.Location = new System.Drawing.Point(208, 32);
			this.ViewerTextBox.Name = "ViewerTextBox";
			this.ViewerTextBox.Size = new System.Drawing.Size(272, 20);
			this.ViewerTextBox.TabIndex = 25;
			this.ViewerTextBox.Text = "[ProjectPath]\\MOG\\Tools\\PC\\LaunchExplorerWin\\LaunchExplorerWin.exe";
			this.NewAssetToolTips.SetToolTip(this.ViewerTextBox, "Select the location of a viewer program for viewing/editing this asset type");
			// 
			// PackageNameTextBox
			// 
			this.PackageNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.PackageNameTextBox.Enabled = false;
			this.PackageNameTextBox.Location = new System.Drawing.Point(216, 176);
			this.PackageNameTextBox.Name = "PackageNameTextBox";
			this.PackageNameTextBox.Size = new System.Drawing.Size(192, 20);
			this.PackageNameTextBox.TabIndex = 50;
			this.PackageNameTextBox.Text = "Package name";
			this.NewAssetToolTips.SetToolTip(this.PackageNameTextBox, "The name of the package that this asset type belongs to, if any");
			// 
			// IconPictureBox
			// 
			this.IconPictureBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.IconPictureBox.Location = new System.Drawing.Point(104, 80);
			this.IconPictureBox.Name = "IconPictureBox";
			this.IconPictureBox.Size = new System.Drawing.Size(24, 24);
			this.IconPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.IconPictureBox.TabIndex = 59;
			this.IconPictureBox.TabStop = false;
			// 
			// DescriptionGroupBox
			// 
			this.DescriptionGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.DescriptionGroupBox.Controls.Add(this.IconLabel);
			this.DescriptionGroupBox.Controls.Add(this.btnIconBrowse);
			this.DescriptionGroupBox.Controls.Add(this.IconTextBox);
			this.DescriptionGroupBox.Controls.Add(this.DescriptionTextBox);
			this.DescriptionGroupBox.Controls.Add(this.NameTextBox);
			this.DescriptionGroupBox.Controls.Add(this.NameLabel);
			this.DescriptionGroupBox.Controls.Add(this.DescriptionLabel);
			this.DescriptionGroupBox.Controls.Add(this.IconPictureBox);
			this.DescriptionGroupBox.Location = new System.Drawing.Point(20, 12);
			this.DescriptionGroupBox.Name = "DescriptionGroupBox";
			this.DescriptionGroupBox.Size = new System.Drawing.Size(856, 120);
			this.DescriptionGroupBox.TabIndex = 60;
			this.DescriptionGroupBox.TabStop = false;
			this.DescriptionGroupBox.Text = "Description";
			// 
			// CustomRipperToolGroupBox
			// 
			this.CustomRipperToolGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.CustomRipperToolGroupBox.Controls.Add(this.button3);
			this.CustomRipperToolGroupBox.Controls.Add(this.btnRipTaskerBrowse);
			this.CustomRipperToolGroupBox.Controls.Add(this.btnRipperBrowse);
			this.CustomRipperToolGroupBox.Controls.Add(this.RipperTextBox);
			this.CustomRipperToolGroupBox.Controls.Add(this.RipTaskerLabel);
			this.CustomRipperToolGroupBox.Controls.Add(this.RipTaskerTextBox);
			this.CustomRipperToolGroupBox.Controls.Add(this.RipperLabel);
			this.CustomRipperToolGroupBox.Controls.Add(this.button2);
			this.CustomRipperToolGroupBox.Enabled = false;
			this.CustomRipperToolGroupBox.Location = new System.Drawing.Point(20, 180);
			this.CustomRipperToolGroupBox.Name = "CustomRipperToolGroupBox";
			this.CustomRipperToolGroupBox.Size = new System.Drawing.Size(856, 88);
			this.CustomRipperToolGroupBox.TabIndex = 61;
			this.CustomRipperToolGroupBox.TabStop = false;
			this.CustomRipperToolGroupBox.Text = "Custom Ripper Tool";
			// 
			// button3
			// 
			this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.button3.Location = new System.Drawing.Point(536, 24);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(120, 24);
			this.button3.TabIndex = 56;
			this.button3.Text = "¡mas complicado!";
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// btnRipTaskerBrowse
			// 
			this.btnRipTaskerBrowse.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.btnRipTaskerBrowse.Image = ((System.Drawing.Image)(resources.GetObject("btnRipTaskerBrowse.Image")));
			this.btnRipTaskerBrowse.Location = new System.Drawing.Point(496, 48);
			this.btnRipTaskerBrowse.Name = "btnRipTaskerBrowse";
			this.btnRipTaskerBrowse.Size = new System.Drawing.Size(24, 23);
			this.btnRipTaskerBrowse.TabIndex = 21;
			this.NewAssetToolTips.SetToolTip(this.btnRipTaskerBrowse, "Browse for a rip tasker");
			this.btnRipTaskerBrowse.Click += new System.EventHandler(this.btnRipTaskerBrowse_Click);
			// 
			// btnRipperBrowse
			// 
			this.btnRipperBrowse.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.btnRipperBrowse.Image = ((System.Drawing.Image)(resources.GetObject("btnRipperBrowse.Image")));
			this.btnRipperBrowse.Location = new System.Drawing.Point(496, 24);
			this.btnRipperBrowse.Name = "btnRipperBrowse";
			this.btnRipperBrowse.Size = new System.Drawing.Size(24, 23);
			this.btnRipperBrowse.TabIndex = 16;
			this.NewAssetToolTips.SetToolTip(this.btnRipperBrowse, "Browse for a ripper");
			this.btnRipperBrowse.Click += new System.EventHandler(this.btnRipperBrowse_Click);
			// 
			// button2
			// 
			this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.button2.Location = new System.Drawing.Point(536, 48);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(120, 24);
			this.button2.TabIndex = 55;
			this.button2.Text = "¡mas complicado!";
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// btnViewerBrowse
			// 
			this.btnViewerBrowse.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.btnViewerBrowse.Image = ((System.Drawing.Image)(resources.GetObject("btnViewerBrowse.Image")));
			this.btnViewerBrowse.Location = new System.Drawing.Point(496, 32);
			this.btnViewerBrowse.Name = "btnViewerBrowse";
			this.btnViewerBrowse.Size = new System.Drawing.Size(24, 23);
			this.btnViewerBrowse.TabIndex = 26;
			this.NewAssetToolTips.SetToolTip(this.btnViewerBrowse, "Browse for a viewer");
			this.btnViewerBrowse.Click += new System.EventHandler(this.btnViewerBrowse_Click);
			// 
			// OrganizationGroupBox
			// 
			this.OrganizationGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.OrganizationGroupBox.Controls.Add(this.BlessFormatStringRootLabel);
			this.OrganizationGroupBox.Controls.Add(this.ExpandedBlessFormatStringLabel);
			this.OrganizationGroupBox.Controls.Add(this.ExpandedFormatStringLabel);
			this.OrganizationGroupBox.Controls.Add(this.btnFormatStringSubmenu);
			this.OrganizationGroupBox.Controls.Add(this.btnBlessFormatStringSubmenu);
			this.OrganizationGroupBox.Controls.Add(this.BlessFormatStringTextBox);
			this.OrganizationGroupBox.Controls.Add(this.PackageNameTextBox);
			this.OrganizationGroupBox.Controls.Add(this.FormatStringTextBox);
			this.OrganizationGroupBox.Controls.Add(this.PackageNameLabel);
			this.OrganizationGroupBox.Controls.Add(this.label1);
			this.OrganizationGroupBox.Controls.Add(this.PackageCheckBox);
			this.OrganizationGroupBox.Controls.Add(this.FormatLabel);
			this.OrganizationGroupBox.Controls.Add(this.FormatStringRootLabel);
			this.OrganizationGroupBox.Location = new System.Drawing.Point(20, 364);
			this.OrganizationGroupBox.Name = "OrganizationGroupBox";
			this.OrganizationGroupBox.Size = new System.Drawing.Size(856, 240);
			this.OrganizationGroupBox.TabIndex = 61;
			this.OrganizationGroupBox.TabStop = false;
			this.OrganizationGroupBox.Text = "Organization";
			// 
			// BlessFormatStringRootLabel
			// 
			this.BlessFormatStringRootLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.BlessFormatStringRootLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.BlessFormatStringRootLabel.Location = new System.Drawing.Point(16, 132);
			this.BlessFormatStringRootLabel.Name = "BlessFormatStringRootLabel";
			this.BlessFormatStringRootLabel.Size = new System.Drawing.Size(264, 28);
			this.BlessFormatStringRootLabel.TabIndex = 71;
			this.BlessFormatStringRootLabel.Text = "Bless format string root";
			this.BlessFormatStringRootLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// ExpandedBlessFormatStringLabel
			// 
			this.ExpandedBlessFormatStringLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.ExpandedBlessFormatStringLabel.Location = new System.Drawing.Point(280, 132);
			this.ExpandedBlessFormatStringLabel.Name = "ExpandedBlessFormatStringLabel";
			this.ExpandedBlessFormatStringLabel.Size = new System.Drawing.Size(560, 24);
			this.ExpandedBlessFormatStringLabel.TabIndex = 70;
			this.NewAssetToolTips.SetToolTip(this.ExpandedBlessFormatStringLabel, "An example expansion of the format string");
			// 
			// ExpandedFormatStringLabel
			// 
			this.ExpandedFormatStringLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.ExpandedFormatStringLabel.Location = new System.Drawing.Point(280, 68);
			this.ExpandedFormatStringLabel.Name = "ExpandedFormatStringLabel";
			this.ExpandedFormatStringLabel.Size = new System.Drawing.Size(560, 24);
			this.ExpandedFormatStringLabel.TabIndex = 69;
			this.NewAssetToolTips.SetToolTip(this.ExpandedFormatStringLabel, "An example expansion of the format string");
			// 
			// btnFormatStringSubmenu
			// 
			this.btnFormatStringSubmenu.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.btnFormatStringSubmenu.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.btnFormatStringSubmenu.Location = new System.Drawing.Point(656, 40);
			this.btnFormatStringSubmenu.Name = "btnFormatStringSubmenu";
			this.btnFormatStringSubmenu.Size = new System.Drawing.Size(24, 24);
			this.btnFormatStringSubmenu.TabIndex = 68;
			this.btnFormatStringSubmenu.Text = ">";
			this.btnFormatStringSubmenu.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.NewAssetToolTips.SetToolTip(this.btnFormatStringSubmenu, "See a pop-up list of valid format string tokens");
			this.btnFormatStringSubmenu.Click += new System.EventHandler(this.btnFormatStringSubmenu_Click);
			// 
			// btnBlessFormatStringSubmenu
			// 
			this.btnBlessFormatStringSubmenu.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.btnBlessFormatStringSubmenu.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.btnBlessFormatStringSubmenu.Location = new System.Drawing.Point(656, 96);
			this.btnBlessFormatStringSubmenu.Name = "btnBlessFormatStringSubmenu";
			this.btnBlessFormatStringSubmenu.Size = new System.Drawing.Size(24, 24);
			this.btnBlessFormatStringSubmenu.TabIndex = 67;
			this.btnBlessFormatStringSubmenu.Text = ">";
			this.btnBlessFormatStringSubmenu.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.NewAssetToolTips.SetToolTip(this.btnBlessFormatStringSubmenu, "See a pop-up list of valid bless format string tokens");
			this.btnBlessFormatStringSubmenu.Click += new System.EventHandler(this.btnBlessFormatStringSubmenu_Click);
			// 
			// FormatStringRootLabel
			// 
			this.FormatStringRootLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.FormatStringRootLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FormatStringRootLabel.Location = new System.Drawing.Point(16, 68);
			this.FormatStringRootLabel.Name = "FormatStringRootLabel";
			this.FormatStringRootLabel.Size = new System.Drawing.Size(264, 28);
			this.FormatStringRootLabel.TabIndex = 67;
			this.FormatStringRootLabel.Text = "Format string root";
			this.FormatStringRootLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// ViewerGroupBox
			// 
			this.ViewerGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.ViewerGroupBox.Controls.Add(this.button1);
			this.ViewerGroupBox.Controls.Add(this.ViewerLabel);
			this.ViewerGroupBox.Controls.Add(this.ViewerTextBox);
			this.ViewerGroupBox.Controls.Add(this.btnViewerBrowse);
			this.ViewerGroupBox.Location = new System.Drawing.Point(20, 276);
			this.ViewerGroupBox.Name = "ViewerGroupBox";
			this.ViewerGroupBox.Size = new System.Drawing.Size(856, 80);
			this.ViewerGroupBox.TabIndex = 66;
			this.ViewerGroupBox.TabStop = false;
			this.ViewerGroupBox.Text = "Viewer";
			// 
			// button1
			// 
			this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.button1.Location = new System.Drawing.Point(536, 32);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(120, 24);
			this.button1.TabIndex = 54;
			this.button1.Text = "¡mas complicado!";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// DivergentDataCheckBox
			// 
			this.DivergentDataCheckBox.Location = new System.Drawing.Point(176, 136);
			this.DivergentDataCheckBox.Name = "DivergentDataCheckBox";
			this.DivergentDataCheckBox.Size = new System.Drawing.Size(160, 24);
			this.DivergentDataCheckBox.TabIndex = 67;
			this.DivergentDataCheckBox.Text = "Divergent platform data";
			// 
			// AddAssetForm
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(896, 664);
			this.Controls.Add(this.DivergentDataCheckBox);
			this.Controls.Add(this.ViewerGroupBox);
			this.Controls.Add(this.DescriptionGroupBox);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.CustomRipperToolGroupBox);
			this.Controls.Add(this.OrganizationGroupBox);
			this.Controls.Add(this.UpdatableCheckBox);
			this.Controls.Add(this.NativeCheckBox);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(904, 696);
			this.Name = "AddAssetForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "New Asset";
			this.DescriptionGroupBox.ResumeLayout(false);
			this.CustomRipperToolGroupBox.ResumeLayout(false);
			this.OrganizationGroupBox.ResumeLayout(false);
			this.ViewerGroupBox.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#endregion

		#region Constructors
		public AddAssetForm(BASE mog)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			
			this.Text = "Add Asset";
			this.mog = mog;
			init();
		}
		public AddAssetForm(BASE mog, MOG_AssetInfo assetInfo)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			
			this.Text = string.Concat("Edit Asset - ", assetInfo.mAssetKey);
			this.mog = mog;
			init();

			FillFields(assetInfo);
			this.NameTextBox.Enabled = false;
		}

		public void init() 
		{
			validMogTokens = new ArrayList();
			validMogTokens.AddRange( new string[] {	  
													  //"{FullFilename}".ToLower(),
													  //"{Path}".ToLower(),
													  "{Filename}".ToLower(),
													  "{Extension}".ToLower(),
													  "{ProjectName}".ToLower(),
													  //"{ProjectPath}".ToLower(),
													  "{UserName}".ToLower(),
													  //"{UserPath}".ToLower(),
													  "{BoxName}".ToLower(),
													  //"{BoxPath}".ToLower(),
													  "{AssetName}".ToLower(),
													  "{AssetProjectKey}".ToLower(),
													  "{AssetKey}".ToLower(),
													  "{AssetClassification}".ToLower(),
													  "{AssetSubClass}".ToLower(),
													  "{AssetLabel}".ToLower(),
													  "{AssetPlatform}".ToLower()
												  } );
			
			ExpandedFormatStringLabel.Text = MOGTokenStringSubst(FormatStringTextBox.Text);
			ExpandedBlessFormatStringLabel.Text = MOGTokenStringSubst(BlessFormatStringTextBox.Text);
			
			MOG_Filename tokenSubst = new MOG_Filename( string.Concat( this.mog.GetProject().GetProjectUsersPath(), "\\USER\\INBOX\\", mog.GetProject().GetProjectKey(), ".", NameTextBox.Text, ".PACKAGE.GROUP.USER.all.txt")  );
			this.FormatStringRootLabel.Text = string.Concat( "C:\\", this.mog.GetProject().GetProjectName(), "\\" );
			//this.BlessFormatStringRootLabel.Text;
		}
		#endregion

		#region Member functions
		public MOG_AssetInfo AssetInfo() 
		{
			MOG_AssetInfo mai = new MOG_AssetInfo();
			mai.mAssetKey = this.AssetName;
			mai.mAssetDescription = this.Description;
			mai.mAssetViewer = this.Viewer;
			mai.mAssetFormatString = this.FormatString;
			mai.mAssetBlessedFormatString = this.BlessFormatString;
			mai.mAssetIcon = this.AssetIcon.ToLower().Replace( string.Concat(this.mog.GetProject().GetProjectToolsPath(), "\\").ToLower(), "" );
            
			if (this.IsNativeDataType) 
			{
				mai.mAssetNativeDataType = "true";
				mai.mAssetRipper = "None";
				mai.mAssetRipTasker = "None";
			}
			else 
			{
				mai.mAssetNativeDataType = "false";
				mai.mAssetRipper = this.Ripper.ToLower().Replace( string.Concat(this.mog.GetProject().GetProjectToolsPath(), "\\").ToLower(), "");
				mai.mAssetRipTasker = this.RipTasker.ToLower().Replace( string.Concat(this.mog.GetProject().GetProjectToolsPath(), "\\").ToLower(), "");
			}
			
			if (this.CanBePackaged) 
			{
				mai.mAssetPackage = "true";
				mai.mAssetPackageName = this.Package;
			}
			else
			{
				mai.mAssetPackage = "false";
				mai.mAssetPackageName = "None";
			}

			if (this.DivergentData) 
			{
				mai.mAssetDivergentPlatformDataType = "true";
			}
			else
			{
				mai.mAssetDivergentPlatformDataType = "false";
			}


			if (this.CanBeUpdated) 
			{
				mai.mAssetCanBeUpdated = "true";
			}
			else 
			{
				mai.mAssetCanBeUpdated = "false";
			}

			// temporary
//			mai.mAssetViewer = "[ProjectPath]\\MOG\\Tools\\PC\\LaunchExplorerWin\\LaunchExplorerWin.exe";
			mai.mAssetViewer = this.ViewerTextBox.Text;
			
			return mai;
		}

		private void FillFields(MOG_AssetInfo assetInfo) 
		{
			NameTextBox.Text = assetInfo.mAssetKey;
			DescriptionTextBox.Text = assetInfo.mAssetDescription;
			IconTextBox.Text = assetInfo.mAssetIcon;
			if (File.Exists( string.Concat(this.mog.GetProject().GetProjectToolsPath(), "\\", IconTextBox.Text) )) 
			{
				IconPictureBox.Image = Image.FromFile( string.Concat(this.mog.GetProject().GetProjectToolsPath(), "\\", IconTextBox.Text) );
			}

			PackageCheckBox.Checked = ( assetInfo.IsPackagedAsset() ? true : false );
			UpdatableCheckBox.Checked = ( (assetInfo.mAssetCanBeUpdated.ToLower() == "true") ? true : false );
			NativeCheckBox.Checked = ( assetInfo.IsNativeDataType() ? true : false );

			RipTaskerTextBox.Text = assetInfo.mAssetRipTasker;
			RipperTextBox.Text = assetInfo.mAssetRipper;
			ViewerTextBox.Text = assetInfo.mAssetViewer;
			
			FormatStringTextBox.Text = assetInfo.mAssetFormatString;
			BlessFormatStringTextBox.Text = assetInfo.mAssetBlessedFormatString;

			PackageNameTextBox.Text = assetInfo.mAssetPackageName;
		}

		private string MOGTokenStringSubst(string line) 
		{
			/* ProjectPath}\Assets\
			 * AssetKey}\
			 * AssetClassification}\
			 * AssetSubClass}\
			 * AssetName}
			 */
			
			MOG_Filename tokenSubst = new MOG_Filename( string.Concat( this.mog.GetProject().GetProjectUsersPath(), "\\USER\\INBOX\\", mog.GetProject().GetProjectKey(), ".", NameTextBox.Text, ".PACKAGE.GROUP.USER.all.txt")  );
			string newLine = line.ToString();		// make a copy of the string
			string[] tokens = line.Split("{".ToCharArray());
			foreach (string s in tokens) 
			{
				string token = string.Concat("{", (s.Split("}".ToCharArray()))[0], "}" );
				if ( this.validMogTokens.Contains(token.ToLower()) ) 
				{
					newLine = newLine.Replace(token, tokenSubst.GetFormattedString(token, ""));
				}
			}
			return newLine;
		}

		public static MOG_AssetInfo EditAsset(BASE mog, MOG_AssetInfo assetInfo) 
		{
			AddAssetForm aaf = new AddAssetForm(mog, assetInfo);
			if (aaf.ShowDialog() == DialogResult.OK) 
			{
				return aaf.AssetInfo();
			}

			return null;
		}
		#endregion

		#region Events
		private void btnOK_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void NativeCheckBox_CheckedChanged(object sender, System.EventArgs e)
		{
			if (NativeCheckBox.Checked) 
			{
				CustomRipperToolGroupBox.Enabled = false;
			} 
			else 
			{
				CustomRipperToolGroupBox.Enabled = true;
			}
		}
		
		private void PackageCheckBox_CheckedChanged(object sender, System.EventArgs e)
		{
			if (PackageCheckBox.Checked) 
			{
				UpdatableCheckBox.Checked = false;

				PackageNameLabel.Enabled = true;
				PackageNameTextBox.Enabled = true;
			}
			else 
			{
				// can't have a package name if asset can't be packaged
				PackageNameLabel.Enabled = false;
				PackageNameTextBox.Enabled = false;
			}
		}
		
		private void UpdatableCheckBox_CheckedChanged(object sender, System.EventArgs e)
		{
			if (UpdatableCheckBox.Checked) 
			{
				PackageCheckBox.Checked = false;
			}
		}
		
		private void btnIconBrowse_Click(object sender, System.EventArgs e)
		{
			string imagesDirectory = string.Concat(mog.GetProject().GetProjectToolsPath(), "\\Images");
			if (Directory.Exists(imagesDirectory)) 
			{
				Directory.CreateDirectory(imagesDirectory);
			}

			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Title = "Select icon file";
			ofd.InitialDirectory = imagesDirectory;
			ofd.Filter = "Image files|*.bmp; *.gif; *.jpg; *.jpeg";

			if (ofd.ShowDialog() == DialogResult.OK) 
			{
				try 
				{
					if (!File.Exists(ofd.FileName)) 
					{
						MessageBox.Show("File specified (" + ofd.FileName + ") does not exist", "Missing file");
						return;
					}
					this.IconTextBox.Text = ofd.FileName;
					IconPictureBox.Image = Image.FromFile(ofd.FileName);

					FileInfo finfo = new FileInfo(ofd.FileName);
					if (!ofd.FileName.ToUpper().StartsWith( this.mog.GetProject().GetProjectToolsPath().ToUpper() )) 
					{
						MessageBox.Show("Copying " + finfo.Name + " to\n" + this.mog.GetProject().GetProjectToolsPath() + "\\Images", "File copy notification");
						if (!Directory.Exists( string.Concat(this.mog.GetProject().GetProjectToolsPath(), "\\Images\\") ) )
						{
							Directory.CreateDirectory( string.Concat(this.mog.GetProject().GetProjectToolsPath(), "\\Images\\") );
						}

						string destFilename = string.Concat( this.mog.GetProject().GetProjectToolsPath(), "\\Images\\", finfo.Name);
						File.Copy(ofd.FileName, destFilename, true);
						this.IconTextBox.Text = destFilename;
					}
				}
				catch (Exception) { MessageBox.Show("Invalid image", "Error"); }
			}
		}

		private void btnFormatStringSubmenu_Click(object sender, System.EventArgs e)
		{
			MOG_Filename tokenSubst = new MOG_Filename( string.Concat( this.mog.GetProject().GetProjectUsersPath(), "\\USER\\INBOX\\", mog.GetProject().GetProjectKey(), ".", NameTextBox.Text, ".PACKAGE.GROUP.USER.all.txt")  );
			
			MenuItem[] menuItems = new MenuItem[ this.validMogTokens.Count ];
			for (int i=0; i<menuItems.Length; i++)
			{
				menuItems[i] = new MenuItem( string.Concat(this.validMogTokens[i], "\t", tokenSubst.GetFormattedString((string)this.validMogTokens[i], "")) );
				menuItems[i].Index = i;
				menuItems[i].Click += new System.EventHandler(this.btnFormatStringSubmenuMenuItem_Click);
			}
			
			ContextMenu menu = new ContextMenu(menuItems);
			Point loc = new Point(btnFormatStringSubmenu.Location.X+btnFormatStringSubmenu.Width, btnFormatStringSubmenu.Location.Y);
			menu.Show(OrganizationGroupBox, loc);
		}

		private void btnFormatStringSubmenuMenuItem_Click(object sender, System.EventArgs e) 
		{
			// parse out just the token name
			string token = string.Concat( "{", (((MenuItem)sender).Text.Split("{}".ToCharArray()))[1], "}" );
			FormatStringTextBox.Text += token;
			FormatStringTextBox.Focus();
			FormatStringTextBox.Select( FormatStringTextBox.Text.Length, FormatStringTextBox.Text.Length );
		}

		private void btnBlessFormatStringSubmenu_Click(object sender, System.EventArgs e)
		{
			MOG_Filename tokenSubst = new MOG_Filename( string.Concat( this.mog.GetProject().GetProjectUsersPath(), "\\USER\\INBOX\\", mog.GetProject().GetProjectKey(), ".", NameTextBox.Text, ".PACKAGE.GROUP.USER.all.txt")  );

			MenuItem[] menuItems = new MenuItem[ this.validMogTokens.Count ];
			for (int i=0; i<menuItems.Length; i++)
			{
				menuItems[i] = new MenuItem( string.Concat(this.validMogTokens[i], "\t", tokenSubst.GetFormattedString((string)this.validMogTokens[i], "")) );
				menuItems[i].Index = i;
				menuItems[i].Click += new System.EventHandler(this.btnBlessFormatStringSubmenuMenuItem_Click);
			}

			ContextMenu menu = new ContextMenu(menuItems);
			Point loc = new Point(btnBlessFormatStringSubmenu.Location.X+btnBlessFormatStringSubmenu.Width, btnBlessFormatStringSubmenu.Location.Y);
			menu.Show(OrganizationGroupBox, loc);
		}
		
		private void btnBlessFormatStringSubmenuMenuItem_Click(object sender, System.EventArgs e) 
		{
			// parse out just the token name
			string token = string.Concat( "{", (((MenuItem)sender).Text.Split("{}".ToCharArray()))[1], "}" );
			BlessFormatStringTextBox.Text += token;
			BlessFormatStringTextBox.Focus();
			BlessFormatStringTextBox.Select( BlessFormatStringTextBox.Text.Length, BlessFormatStringTextBox.Text.Length );
		}

		private void FormatStringTextBox_TextChanged(object sender, System.EventArgs e)
		{
            ExpandedFormatStringLabel.Text = MOGTokenStringSubst(FormatStringTextBox.Text);
		}

		private void BlessFormatStringTextBox_TextChanged(object sender, System.EventArgs e)
		{
			ExpandedBlessFormatStringLabel.Text = MOGTokenStringSubst(BlessFormatStringTextBox.Text);
		}

		private void btnRipperBrowse_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Title = "Select Ripper";
			if (File.Exists( RipperTextBox.Text )) 
			{
				ofd.InitialDirectory = (new FileInfo(RipperTextBox.Text)).DirectoryName;
			}
			else 
			{
				ofd.InitialDirectory = this.mog.GetProject().GetProjectToolsPath();
			}
			if (ofd.ShowDialog() == DialogResult.OK) 
			{
				RipperTextBox.Text = ofd.FileName;
				if (!ofd.FileName.ToUpper().StartsWith( this.mog.GetProject().GetProjectToolsPath().ToUpper() )) 
				{
					string msg = string.Concat( "The ripper you selected is not located in the current project's 'Tools' path \n(", this.mog.GetProject().GetProjectToolsPath(), ")\nWould you like to copy it there?");
					if (MessageBox.Show(msg, "Copy ripper to 'Tools' folder?", MessageBoxButtons.YesNo) == DialogResult.Yes) 
					{
						string destFilename = "";
						if (File.Exists(ofd.FileName)) 
						{
							FileInfo finfo = new FileInfo(ofd.FileName);
							destFilename = string.Concat( this.mog.GetProject().GetProjectToolsPath(), "\\", finfo.Name );
							bool overwrite = false;
							if (File.Exists(destFilename)) 
							{
								if (MessageBox.Show("A file of that name exists already in the 'Tools' folder. Overwrite it?", "Overwrite ripper?", MessageBoxButtons.YesNo) == DialogResult.Yes) 
								{
									overwrite = true;
								}
							}
							File.Copy(ofd.FileName, destFilename, overwrite);
						}
						RipperTextBox.Text = destFilename;
					}
				}
			}
		}

		private void btnRipTaskerBrowse_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Title = "Select Rip Tasker";
			if (File.Exists( RipTaskerTextBox.Text )) 
			{
				ofd.InitialDirectory = (new FileInfo(RipTaskerTextBox.Text)).DirectoryName;
			}
			else 
			{
				ofd.InitialDirectory = this.mog.GetProject().GetProjectToolsPath();
			}
			if (ofd.ShowDialog() == DialogResult.OK) 
			{
				RipTaskerTextBox.Text = ofd.FileName;
				if (!ofd.FileName.ToUpper().StartsWith( this.mog.GetProject().GetProjectToolsPath().ToUpper() )) 
				{
					string msg = string.Concat( "The rip tasker you selected is not located in the current project's 'Tools' path \n(", this.mog.GetProject().GetProjectToolsPath(), ")\nWould you like to copy it there?");
					if (MessageBox.Show(msg, "Copy rip tasker to 'Tools' folder?", MessageBoxButtons.YesNo) == DialogResult.Yes) 
					{
						string destFilename = "";
						if (File.Exists(ofd.FileName)) 
						{
							FileInfo finfo = new FileInfo(ofd.FileName);
							destFilename = string.Concat( this.mog.GetProject().GetProjectToolsPath(), "\\", finfo.Name );
							bool overwrite = false;
							if (File.Exists(destFilename)) 
							{
								if (MessageBox.Show("A file of that name exists already in the 'Tools' folder. Overwrite it?", "Overwrite rip tasker?", MessageBoxButtons.YesNo) == DialogResult.Yes) 
								{
									overwrite = true;
								}
							}
							File.Copy(ofd.FileName, destFilename, overwrite);
						}
						RipTaskerTextBox.Text = destFilename;
					}
				}
			}
		}


		private void btnViewerBrowse_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Title = "Select Viewer";
			if (File.Exists( ViewerTextBox.Text )) 
			{
				ofd.InitialDirectory = (new FileInfo(ViewerTextBox.Text)).DirectoryName;
			}
			else 
			{
				ofd.InitialDirectory = this.mog.GetProject().GetProjectToolsPath();
			}
			if (ofd.ShowDialog() == DialogResult.OK) 
			{
				ViewerTextBox.Text = ofd.FileName;
				if (!ofd.FileName.ToUpper().StartsWith( this.mog.GetProject().GetProjectToolsPath().ToUpper() )) 
				{
					string msg = string.Concat( "The viewer you selected is not located in the current project's 'Tools' path \n(", this.mog.GetProject().GetProjectToolsPath(), ")\nWould you like to copy it there?");
					if (MessageBox.Show(msg, "Copy viewer to 'Tools' folder?", MessageBoxButtons.YesNo) == DialogResult.Yes) 
					{
						string destFilename = "";
						if (File.Exists(ofd.FileName)) 
						{
							FileInfo finfo = new FileInfo(ofd.FileName);
							destFilename = string.Concat( this.mog.GetProject().GetProjectToolsPath(), "\\", finfo.Name );
							bool overwrite = false;
							if (File.Exists(destFilename)) 
							{
								if (MessageBox.Show("A file of that name exists already in the 'Tools' folder. Overwrite it?", "Overwrite viewer?", MessageBoxButtons.YesNo) == DialogResult.Yes) 
								{
									overwrite = true;
								}
							}
							File.Copy(ofd.FileName, destFilename, overwrite);
						}
						ViewerTextBox.Text = destFilename;
					}
				}
			}
		}
		
		#endregion

		private void button1_Click(object sender, System.EventArgs e)
		{
			CopyToolForm ctf = new CopyToolForm(CopyToolType.Viewer, this.mog);
			if (ctf.ShowDialog() == DialogResult.OK) 
			{
				this.ViewerTextBox.Text = ctf.ExecutablePath;
			}
		}

		private void button2_Click(object sender, System.EventArgs e)
		{
			CopyToolForm ctf = new CopyToolForm(CopyToolType.RipTasker, this.mog);
			if (ctf.ShowDialog() == DialogResult.OK) 
			{
				this.RipTaskerTextBox.Text = ctf.ExecutablePath;
			}		
		}

		private void button3_Click(object sender, System.EventArgs e)
		{
			CopyToolForm ctf = new CopyToolForm(CopyToolType.Ripper, this.mog);
			if (ctf.ShowDialog() == DialogResult.OK) 
			{
				this.RipperTextBox.Text = ctf.ExecutablePath;
			}		
		}

	
	}
}
