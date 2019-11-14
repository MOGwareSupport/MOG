using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using MOG;
using MOG.PROJECT;
using MOG.PROPERTIES;
using MOG.DOSUTILS;
using MOG.CONTROLLER.CONTROLLERPROJECT;



namespace MOG_Server
{
	/// <summary>
	/// Summary description for AssetForm.
	/// </summary>
	public class AssetForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox AssetNameTextBox;
		private System.Windows.Forms.TextBox AssetDescriptionTextBox;
		private System.Windows.Forms.TextBox AssetRipperTextBox;
		private System.Windows.Forms.TextBox AssetRipTaskerTextBox;
		private System.Windows.Forms.CheckBox AssetPackageCheckBox;
		private System.Windows.Forms.TextBox AssetIconTextBox;
		private System.Windows.Forms.TextBox AssetFormatTextBox;
		private System.Windows.Forms.Button AssetCancelButton;
		private System.Windows.Forms.Button AssetOkButton;
		private System.Windows.Forms.Button AssetIconBrowseButton;
		private System.Windows.Forms.NumericUpDown AssetArchiveNumberNumericUpDown;
		private System.Windows.Forms.Label AssetArchiveNumberLabel;
		public System.Windows.Forms.PictureBox AssetIconPictureBox;

		MOG_Properties mProperties;
		private System.Windows.Forms.OpenFileDialog AssetIconOpenFileDialog;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public AssetForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(AssetForm));
			this.AssetNameTextBox = new System.Windows.Forms.TextBox();
			this.AssetDescriptionTextBox = new System.Windows.Forms.TextBox();
			this.AssetRipperTextBox = new System.Windows.Forms.TextBox();
			this.AssetRipTaskerTextBox = new System.Windows.Forms.TextBox();
			this.AssetPackageCheckBox = new System.Windows.Forms.CheckBox();
			this.AssetIconTextBox = new System.Windows.Forms.TextBox();
			this.AssetFormatTextBox = new System.Windows.Forms.TextBox();
			this.AssetCancelButton = new System.Windows.Forms.Button();
			this.AssetOkButton = new System.Windows.Forms.Button();
			this.AssetIconBrowseButton = new System.Windows.Forms.Button();
			this.AssetIconPictureBox = new System.Windows.Forms.PictureBox();
			this.AssetArchiveNumberNumericUpDown = new System.Windows.Forms.NumericUpDown();
			this.AssetArchiveNumberLabel = new System.Windows.Forms.Label();
			this.AssetIconOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
			((System.ComponentModel.ISupportInitialize)(this.AssetArchiveNumberNumericUpDown)).BeginInit();
			this.SuspendLayout();
			// 
			// AssetNameTextBox
			// 
			this.AssetNameTextBox.Location = new System.Drawing.Point(8, 40);
			this.AssetNameTextBox.Name = "AssetNameTextBox";
			this.AssetNameTextBox.Size = new System.Drawing.Size(240, 20);
			this.AssetNameTextBox.TabIndex = 0;
			this.AssetNameTextBox.Text = "Name";
			// 
			// AssetDescriptionTextBox
			// 
			this.AssetDescriptionTextBox.Location = new System.Drawing.Point(8, 64);
			this.AssetDescriptionTextBox.Name = "AssetDescriptionTextBox";
			this.AssetDescriptionTextBox.Size = new System.Drawing.Size(240, 20);
			this.AssetDescriptionTextBox.TabIndex = 1;
			this.AssetDescriptionTextBox.Text = "Description";
			// 
			// AssetRipperTextBox
			// 
			this.AssetRipperTextBox.Location = new System.Drawing.Point(8, 88);
			this.AssetRipperTextBox.Name = "AssetRipperTextBox";
			this.AssetRipperTextBox.Size = new System.Drawing.Size(240, 20);
			this.AssetRipperTextBox.TabIndex = 2;
			this.AssetRipperTextBox.Text = "Ripper";
			// 
			// AssetRipTaskerTextBox
			// 
			this.AssetRipTaskerTextBox.Location = new System.Drawing.Point(8, 112);
			this.AssetRipTaskerTextBox.Name = "AssetRipTaskerTextBox";
			this.AssetRipTaskerTextBox.Size = new System.Drawing.Size(240, 20);
			this.AssetRipTaskerTextBox.TabIndex = 3;
			this.AssetRipTaskerTextBox.Text = "Rip Tasker";
			// 
			// AssetPackageCheckBox
			// 
			this.AssetPackageCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.AssetPackageCheckBox.Location = new System.Drawing.Point(104, 216);
			this.AssetPackageCheckBox.Name = "AssetPackageCheckBox";
			this.AssetPackageCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.AssetPackageCheckBox.Size = new System.Drawing.Size(144, 24);
			this.AssetPackageCheckBox.TabIndex = 4;
			this.AssetPackageCheckBox.Text = "Package this asset type";
			// 
			// AssetIconTextBox
			// 
			this.AssetIconTextBox.Location = new System.Drawing.Point(8, 136);
			this.AssetIconTextBox.Name = "AssetIconTextBox";
			this.AssetIconTextBox.Size = new System.Drawing.Size(216, 20);
			this.AssetIconTextBox.TabIndex = 5;
			this.AssetIconTextBox.Text = "Icon";
			// 
			// AssetFormatTextBox
			// 
			this.AssetFormatTextBox.Location = new System.Drawing.Point(8, 160);
			this.AssetFormatTextBox.Name = "AssetFormatTextBox";
			this.AssetFormatTextBox.Size = new System.Drawing.Size(240, 20);
			this.AssetFormatTextBox.TabIndex = 6;
			this.AssetFormatTextBox.Text = "Format";
			// 
			// AssetCancelButton
			// 
			this.AssetCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.AssetCancelButton.Location = new System.Drawing.Point(88, 248);
			this.AssetCancelButton.Name = "AssetCancelButton";
			this.AssetCancelButton.TabIndex = 7;
			this.AssetCancelButton.Text = "Cancel";
			this.AssetCancelButton.Click += new System.EventHandler(this.AssetCancelButton_Click);
			// 
			// AssetOkButton
			// 
			this.AssetOkButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.AssetOkButton.Location = new System.Drawing.Point(168, 248);
			this.AssetOkButton.Name = "AssetOkButton";
			this.AssetOkButton.TabIndex = 8;
			this.AssetOkButton.Text = "Ok";
			this.AssetOkButton.Click += new System.EventHandler(this.AssetOkButton_Click);
			// 
			// AssetIconBrowseButton
			// 
			this.AssetIconBrowseButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.AssetIconBrowseButton.Image = ((System.Drawing.Image)(resources.GetObject("AssetIconBrowseButton.Image")));
			this.AssetIconBrowseButton.Location = new System.Drawing.Point(226, 135);
			this.AssetIconBrowseButton.Name = "AssetIconBrowseButton";
			this.AssetIconBrowseButton.Size = new System.Drawing.Size(23, 23);
			this.AssetIconBrowseButton.TabIndex = 9;
			this.AssetIconBrowseButton.Text = "?";
			this.AssetIconBrowseButton.Click += new System.EventHandler(this.AssetIconBrowseButton_Click);
			// 
			// AssetIconPictureBox
			// 
			this.AssetIconPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("AssetIconPictureBox.Image")));
			this.AssetIconPictureBox.Location = new System.Drawing.Point(10, 5);
			this.AssetIconPictureBox.Name = "AssetIconPictureBox";
			this.AssetIconPictureBox.Size = new System.Drawing.Size(30, 27);
			this.AssetIconPictureBox.TabIndex = 10;
			this.AssetIconPictureBox.TabStop = false;
			// 
			// AssetArchiveNumberNumericUpDown
			// 
			this.AssetArchiveNumberNumericUpDown.Location = new System.Drawing.Point(191, 186);
			this.AssetArchiveNumberNumericUpDown.Name = "AssetArchiveNumberNumericUpDown";
			this.AssetArchiveNumberNumericUpDown.Size = new System.Drawing.Size(56, 20);
			this.AssetArchiveNumberNumericUpDown.TabIndex = 11;
			this.AssetArchiveNumberNumericUpDown.Value = new System.Decimal(new int[] {
																						  7,
																						  0,
																						  0,
																						  0});
			// 
			// AssetArchiveNumberLabel
			// 
			this.AssetArchiveNumberLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.AssetArchiveNumberLabel.Location = new System.Drawing.Point(95, 189);
			this.AssetArchiveNumberLabel.Name = "AssetArchiveNumberLabel";
			this.AssetArchiveNumberLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.AssetArchiveNumberLabel.Size = new System.Drawing.Size(92, 16);
			this.AssetArchiveNumberLabel.TabIndex = 12;
			this.AssetArchiveNumberLabel.Text = "Version Numbers";
			this.AssetArchiveNumberLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// AssetIconOpenFileDialog
			// 
			this.AssetIconOpenFileDialog.DefaultExt = "bmp";
			this.AssetIconOpenFileDialog.Filter = "Image files = (*.bmp)|*.bmp";
			// 
			// AssetForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(640, 446);
			this.Controls.Add(this.AssetArchiveNumberLabel);
			this.Controls.Add(this.AssetArchiveNumberNumericUpDown);
			this.Controls.Add(this.AssetIconPictureBox);
			this.Controls.Add(this.AssetIconBrowseButton);
			this.Controls.Add(this.AssetOkButton);
			this.Controls.Add(this.AssetCancelButton);
			this.Controls.Add(this.AssetFormatTextBox);
			this.Controls.Add(this.AssetIconTextBox);
			this.Controls.Add(this.AssetRipTaskerTextBox);
			this.Controls.Add(this.AssetRipperTextBox);
			this.Controls.Add(this.AssetDescriptionTextBox);
			this.Controls.Add(this.AssetNameTextBox);
			this.Controls.Add(this.AssetPackageCheckBox);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "AssetForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "AssetForm";
			((System.ComponentModel.ISupportInitialize)(this.AssetArchiveNumberNumericUpDown)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		public void AssetPopulate(MOG_Properties asset)
		{
			mProperties = asset;

			this.AssetDescriptionTextBox.Text = mProperties.GetDescription();
			this.AssetFormatTextBox.Text = mProperties.GetGameDataPath();
			this.AssetRipperTextBox.Text = mProperties.GetAssetRipper();
			this.AssetRipTaskerTextBox.Text = mProperties.GetAssetRipTasker();
			this.AssetNameTextBox.Text = mProperties.GetClassification();

			this.AssetIconTextBox.Text = mProperties.GetAssetIcon();
			if (DosUtils.FileExist(string.Concat(MOG_ControllerProject.GetProject().GetProjectToolsPath(), "\\", mProperties.GetAssetIcon())))
			{
				this.AssetIconPictureBox.Image = Image.FromFile(string.Concat(MOG_ControllerProject.GetProject().GetProjectToolsPath(), "\\", mProperties.GetAssetIcon()));
			}

			switch(mProperties.GetPackagedAsset().ToLower())
			{
				case "true":
					this.AssetPackageCheckBox.Checked = true;
					break;
				case "false":
					this.AssetPackageCheckBox.Checked = false;
					break;
				default:
					MessageBox.Show(this, "This asset has an invalid package value");
					break;
			}
		}

		private void AssetSave()
		{
			mProperties.SetDescription(AssetDescriptionTextBox.Text);
			mProperties.SetGameDataPath(AssetFormatTextBox.Text);
			mProperties.SetAssetRipper(AssetRipperTextBox.Text);
			mProperties.SetAssetRipTasker(AssetRipTaskerTextBox.Text);
			mProperties.SetClassification(AssetNameTextBox.Text);

			mProperties.SetAssetIcon(AssetIconTextBox.Text);

			if (AssetPackageCheckBox.Checked == true)
			{
				mProperties.SetPackagedAsset("true");
			}
			else
			{
				mProperties.SetPackagedAsset("false");
			}

//			MOG_ControllerProject.GetProject().AssetSave(mProperties);
		}

		private void AssetOkButton_Click(object sender, System.EventArgs e)
		{
			// Save our ini
			AssetSave();

			// Reload the ini
			MOG_ControllerProject.GetProject().Load();
			this.Close();
		}

		private void AssetCancelButton_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void AssetIconBrowseButton_Click(object sender, System.EventArgs e)
		{
			AssetIconOpenFileDialog.InitialDirectory = string.Concat(MOG_ControllerProject.GetProject().GetProjectToolsPath(), "\\Images");

			if (AssetIconOpenFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				this.AssetIconTextBox.Text = AssetIconOpenFileDialog.FileName;
				this.AssetIconPictureBox.Image = Image.FromFile(AssetIconOpenFileDialog.FileName);
			}
		}
	}
}
