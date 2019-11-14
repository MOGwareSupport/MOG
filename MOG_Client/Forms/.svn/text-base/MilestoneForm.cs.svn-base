using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

using MOG;
using MOG.REPORT;
using MOG.PROJECT;
using MOG.PLATFORM;
using MOG.DOSUTILS;
using MOG.INI;

namespace MOG_Client.Forms
{
	/// <summary>
	/// Summary description for MilestoneForm.
	/// </summary>
	public class MilestoneForm : System.Windows.Forms.Form
	{
		public System.Windows.Forms.TextBox MilestoneTargetTextBox;
		private System.Windows.Forms.Button BrowseButton;
		public System.Windows.Forms.ComboBox MilestoneSourceComboBox;
		public System.Windows.Forms.ComboBox MilestonePlatformComboBox;
		private System.Windows.Forms.FolderBrowserDialog MOGFolderBrowserDialog;
		private BASE mMog;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		public System.Windows.Forms.ComboBox PatchNameComboBox;
		public System.Windows.Forms.TextBox PatchNameTextBox;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public MilestoneForm(BASE mog)
		{
			mMog = mog;
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializePlatforms();
            InitializeSourceVersions();
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MilestoneForm));
			this.MilestoneTargetTextBox = new System.Windows.Forms.TextBox();
			this.BrowseButton = new System.Windows.Forms.Button();
			this.MilestoneSourceComboBox = new System.Windows.Forms.ComboBox();
			this.MilestonePlatformComboBox = new System.Windows.Forms.ComboBox();
			this.MOGFolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.PatchNameComboBox = new System.Windows.Forms.ComboBox();
			this.PatchNameTextBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// MilestoneTargetTextBox
			// 
			this.MilestoneTargetTextBox.Location = new System.Drawing.Point(8, 80);
			this.MilestoneTargetTextBox.Name = "MilestoneTargetTextBox";
			this.MilestoneTargetTextBox.Size = new System.Drawing.Size(352, 20);
			this.MilestoneTargetTextBox.TabIndex = 0;
			this.MilestoneTargetTextBox.Text = "Milestone Target";
			this.MilestoneTargetTextBox.TextChanged += new System.EventHandler(this.MilestoneTargetTextBox_TextChanged);
			// 
			// BrowseButton
			// 
			this.BrowseButton.Location = new System.Drawing.Point(360, 80);
			this.BrowseButton.Name = "BrowseButton";
			this.BrowseButton.Size = new System.Drawing.Size(24, 23);
			this.BrowseButton.TabIndex = 1;
			this.BrowseButton.Text = "Browse";
			this.BrowseButton.Click += new System.EventHandler(this.BrowseButton_Click);
			// 
			// MilestoneSourceComboBox
			// 
			this.MilestoneSourceComboBox.Location = new System.Drawing.Point(8, 56);
			this.MilestoneSourceComboBox.Name = "MilestoneSourceComboBox";
			this.MilestoneSourceComboBox.Size = new System.Drawing.Size(352, 21);
			this.MilestoneSourceComboBox.TabIndex = 2;
			this.MilestoneSourceComboBox.Text = "Milestone Source";
			// 
			// MilestonePlatformComboBox
			// 
			this.MilestonePlatformComboBox.Location = new System.Drawing.Point(8, 32);
			this.MilestonePlatformComboBox.Name = "MilestonePlatformComboBox";
			this.MilestonePlatformComboBox.Size = new System.Drawing.Size(80, 21);
			this.MilestonePlatformComboBox.TabIndex = 3;
			this.MilestonePlatformComboBox.Text = "Platform";
			this.MilestonePlatformComboBox.SelectedIndexChanged += new System.EventHandler(this.MilestonePlatformComboBox_SelectedIndexChanged);
			// 
			// button1
			// 
			this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button1.Location = new System.Drawing.Point(200, 136);
			this.button1.Name = "button1";
			this.button1.TabIndex = 4;
			this.button1.Text = "Ok";
			// 
			// button2
			// 
			this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button2.Location = new System.Drawing.Point(120, 136);
			this.button2.Name = "button2";
			this.button2.TabIndex = 5;
			this.button2.Text = "Cancel";
			// 
			// PatchNameComboBox
			// 
			this.PatchNameComboBox.Location = new System.Drawing.Point(8, 104);
			this.PatchNameComboBox.Name = "PatchNameComboBox";
			this.PatchNameComboBox.Size = new System.Drawing.Size(352, 21);
			this.PatchNameComboBox.TabIndex = 7;
			this.PatchNameComboBox.Text = "Patch Build Name";
			// 
			// PatchNameTextBox
			// 
			this.PatchNameTextBox.Location = new System.Drawing.Point(216, 32);
			this.PatchNameTextBox.Name = "PatchNameTextBox";
			this.PatchNameTextBox.Size = new System.Drawing.Size(144, 20);
			this.PatchNameTextBox.TabIndex = 8;
			this.PatchNameTextBox.Text = "PatchNameTextBox";
			// 
			// MilestoneForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(392, 165);
			this.Controls.Add(this.PatchNameTextBox);
			this.Controls.Add(this.PatchNameComboBox);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.MilestonePlatformComboBox);
			this.Controls.Add(this.MilestoneSourceComboBox);
			this.Controls.Add(this.BrowseButton);
			this.Controls.Add(this.MilestoneTargetTextBox);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "MilestoneForm";
			this.Text = "Create Milestone";
			this.ResumeLayout(false);

		}
		#endregion

		private void InitializePlatforms()
		{
			MilestonePlatformComboBox.Items.Clear();

			// Add a item for every platform
			ArrayList platforms = mMog.GetProject().GetPlatforms();
			for (int p = 0; p < platforms.Count; p++)
			{
				MOG_Platform platform = (MOG_Platform)platforms[p];
				MilestonePlatformComboBox.Items.Add(platform.mPlatformName);
			}

            //MilestonePlatformComboBox.Items.Add("All");
		}

		private void InitializeSourceVersions()
		{
			MilestoneSourceComboBox.Items.Clear();

			// Add the local version first
			string platform = MilestonePlatformComboBox.Text;
			if (mMog.GetProject().GetPlatform(platform) != null)
			{
				MilestoneSourceComboBox.Items.Add(string.Concat(mMog.GetProject().GetPlatform(platform).mPlatformTargetPath, "\\MOG\\LocalVersion.info"));
			}
			else if (string.Compare(platform, "All", true) == 0 )
			{
				// error out for now
				MOG_REPORT.ShowMessageBox("ERROR", "All platform not supported yet.", MessageBoxButtons.OK);
			}
			
			// Add the global current version next
			string globalPath = string.Concat(mMog.GetProject().GetProjectPath(), "\\Versions\\");
			MilestoneSourceComboBox.Items.Add(string.Concat(globalPath, "Current.info"));

			foreach (FileInfo file in DosUtils.FileGetList(globalPath, "*.info"))
			{
				if (string.Compare(file.Name, "Current.info", true) != 0)
				{
					MilestoneSourceComboBox.Items.Add(string.Concat(globalPath, file.Name));
				}
			}
		}

		private void BrowseButton_Click(object sender, System.EventArgs e)
		{
			// show the window
			if (MOGFolderBrowserDialog.ShowDialog(this) == DialogResult.OK)
			{
				MilestoneTargetTextBox.Text = MOGFolderBrowserDialog.SelectedPath;
			}
		}

		private void MilestonePlatformComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			InitializeSourceVersions();
			return;

//			string platform = MilestonePlatformComboBox.Text;
//			string old = "f";
//			if (mMog.GetProject().GetPlatform(platform) != null)
//			{
//				if (MilestoneSourceComboBox.Text.IndexOf(mMog.GetProject().GetPlatform(platform).mPlatformTargetPath) != -1)
//				{
//					MilestoneSourceComboBox.Text = MilestoneSourceComboBox.Text.Replace(old, mMog.GetProject().GetPlatform(platform).mPlatformTargetPath);
//				}
//			}
		}

		private void MilestoneTargetTextBox_TextChanged(object sender, System.EventArgs e)
		{
			// Check if any other patches exist
			string patchInfo = string.Concat(MilestoneTargetTextBox.Text, "\\MOG\\Patches\\Patches.Info");
			if (DosUtils.FileExist(patchInfo))
			{
				PatchNameComboBox.Items.Clear();

				// Open it and add it to our patch list
				MOG_Ini patches = new MOG_Ini(patchInfo);

				// Add a key for our local build first.
				PatchNameComboBox.Items.Add("Local");

				if (patches.SectionExist("PATCHES"))
				{
					for (int i = 0; i < patches.CountKeys("PATCHES"); i++)
					{
						PatchNameComboBox.Items.Add(patches.GetKeyNameByIndex("PATCHES", i));
					}
				}

				if (patches.SectionExist("Local") && patches.KeyExist("Local", "Current"))
				{
					PatchNameComboBox.Text = patches.GetString("Local", "Current");
				}
				else
				{
					PatchNameComboBox.Text = "Unknown";
				}
			}            
		}
	}
}
