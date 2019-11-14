using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using MOG_Client.Client_Gui;
using MOG.CONTROLLER.CONTROLLERPROJECT;

using MOG_ControlsLibrary.Utils;
using System.Collections.Generic;
using MOG.DATABASE;
using System.IO;
using MOG.CONTROLLER.CONTROLLERSYSTEM;

namespace MOG_Client.Forms
{
	/// <summary>
	/// Summary description for CreateNewWorkspaceWizard.
	/// </summary>
	public class CreateNewWorkspaceWizard : System.Windows.Forms.Form
	{
		private Gui.Wizard.Wizard NewWorkspaceWizard;
		private Gui.Wizard.WizardPage wizardPage1;
		private Gui.Wizard.InfoPage NewWorkspaceInfoPage;
		private Gui.Wizard.WizardPage wizardPage2;
		private Gui.Wizard.Header NewWorkspaceHeader;
		private System.Windows.Forms.Label label1;
		private Gui.Wizard.WizardPage wizardPage3;
		private Gui.Wizard.Header NewWorkspacePlatformHeader;
		private System.Windows.Forms.ComboBox NewWorkspacePlatformsComboBox;
		private System.Windows.Forms.Label label2;
		private Gui.Wizard.WizardPage wizardPage4;
		private Gui.Wizard.Header header1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox NewWorkspaceBranchComboBox;
		private Gui.Wizard.WizardPage wizardPage5;
		private Gui.Wizard.InfoContainer infoContainer1;
		private System.Windows.Forms.Button NewWorkspaceBrowseButton;
		private System.Windows.Forms.TextBox NewWorkspaceDirectoryTextBox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label NewWorkspaceDirectoryLabel;
		private System.Windows.Forms.Label NewWorkspaceBranchLabel;
        private System.Windows.Forms.Label NewWorkspacePlatformLabel;
        public CheckBox NewWorkspaceMergeCheckBox;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public string NewWorkspaceDirectory
		{
			get { return this.NewWorkspaceDirectoryTextBox.Text; }
		}

		public string NewWorkspaceBranch
		{
			get { return this.NewWorkspaceBranchComboBox.Text; }
		}

		public string NewWorkspacePlatform
		{
			get { return this.NewWorkspacePlatformsComboBox.Text; }
		}

		public CreateNewWorkspaceWizard()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// Intialize the branch
			List<string> branches = guiStartup.GetBranches(MOG_ControllerProject.GetProjectName());
			NewWorkspaceBranchComboBox.Items.Clear();
			NewWorkspaceBranchComboBox.Items.AddRange(branches.ToArray());
			NewWorkspaceBranchComboBox.SelectedIndex = 0;

			// Initialize the platforms
			List<string> platforms = guiStartup.GetPlatforms(MOG_ControllerProject.GetProjectName());
			NewWorkspacePlatformsComboBox.Items.Clear();
			NewWorkspacePlatformsComboBox.Items.AddRange(platforms.ToArray());
			NewWorkspacePlatformsComboBox.SelectedIndex = 0;
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateNewWorkspaceWizard));
			this.NewWorkspaceWizard = new Gui.Wizard.Wizard();
			this.wizardPage3 = new Gui.Wizard.WizardPage();
			this.label2 = new System.Windows.Forms.Label();
			this.NewWorkspacePlatformsComboBox = new System.Windows.Forms.ComboBox();
			this.NewWorkspacePlatformHeader = new Gui.Wizard.Header();
			this.wizardPage4 = new Gui.Wizard.WizardPage();
			this.label3 = new System.Windows.Forms.Label();
			this.NewWorkspaceBranchComboBox = new System.Windows.Forms.ComboBox();
			this.header1 = new Gui.Wizard.Header();
			this.wizardPage1 = new Gui.Wizard.WizardPage();
			this.NewWorkspaceInfoPage = new Gui.Wizard.InfoPage();
			this.wizardPage5 = new Gui.Wizard.WizardPage();
			this.infoContainer1 = new Gui.Wizard.InfoContainer();
			this.NewWorkspaceMergeCheckBox = new System.Windows.Forms.CheckBox();
			this.NewWorkspacePlatformLabel = new System.Windows.Forms.Label();
			this.NewWorkspaceBranchLabel = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.NewWorkspaceDirectoryLabel = new System.Windows.Forms.Label();
			this.wizardPage2 = new Gui.Wizard.WizardPage();
			this.NewWorkspaceBrowseButton = new System.Windows.Forms.Button();
			this.NewWorkspaceDirectoryTextBox = new System.Windows.Forms.TextBox();
			this.NewWorkspaceHeader = new Gui.Wizard.Header();
			this.label1 = new System.Windows.Forms.Label();
			this.NewWorkspaceWizard.SuspendLayout();
			this.wizardPage3.SuspendLayout();
			this.wizardPage4.SuspendLayout();
			this.wizardPage1.SuspendLayout();
			this.wizardPage5.SuspendLayout();
			this.infoContainer1.SuspendLayout();
			this.wizardPage2.SuspendLayout();
			this.SuspendLayout();
			// 
			// NewWorkspaceWizard
			// 
			this.NewWorkspaceWizard.Controls.Add(this.wizardPage2);
			this.NewWorkspaceWizard.Controls.Add(this.wizardPage3);
			this.NewWorkspaceWizard.Controls.Add(this.wizardPage4);
			this.NewWorkspaceWizard.Controls.Add(this.wizardPage1);
			this.NewWorkspaceWizard.Controls.Add(this.wizardPage5);
			this.NewWorkspaceWizard.Dock = System.Windows.Forms.DockStyle.Fill;
			this.NewWorkspaceWizard.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.NewWorkspaceWizard.Location = new System.Drawing.Point(0, 0);
			this.NewWorkspaceWizard.Name = "NewWorkspaceWizard";
			this.NewWorkspaceWizard.Pages.AddRange(new Gui.Wizard.WizardPage[] {
            this.wizardPage1,
            this.wizardPage4,
            this.wizardPage3,
            this.wizardPage2,
            this.wizardPage5});
			this.NewWorkspaceWizard.PushPop = false;
			this.NewWorkspaceWizard.Size = new System.Drawing.Size(489, 353);
			this.NewWorkspaceWizard.TabIndex = 0;
			// 
			// wizardPage3
			// 
			this.wizardPage3.Controls.Add(this.label2);
			this.wizardPage3.Controls.Add(this.NewWorkspacePlatformsComboBox);
			this.wizardPage3.Controls.Add(this.NewWorkspacePlatformHeader);
			this.wizardPage3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.wizardPage3.IsFinishPage = false;
			this.wizardPage3.Location = new System.Drawing.Point(0, 0);
			this.wizardPage3.Name = "wizardPage3";
			this.wizardPage3.Size = new System.Drawing.Size(489, 305);
			this.wizardPage3.TabIndex = 3;
			this.wizardPage3.ShowFromNext += new System.EventHandler(this.wizardPage3_ShowFromNext);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 120);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(384, 16);
			this.label2.TabIndex = 2;
			this.label2.Text = "Select platform for your local workspace:";
			// 
			// NewWorkspacePlatformsComboBox
			// 
			this.NewWorkspacePlatformsComboBox.Location = new System.Drawing.Point(16, 136);
			this.NewWorkspacePlatformsComboBox.Name = "NewWorkspacePlatformsComboBox";
			this.NewWorkspacePlatformsComboBox.Size = new System.Drawing.Size(461, 21);
			this.NewWorkspacePlatformsComboBox.TabIndex = 1;
			this.NewWorkspacePlatformsComboBox.SelectedIndexChanged += new System.EventHandler(this.NewWorkspacePlatformsComboBox_SelectedIndexChanged);
			this.NewWorkspacePlatformsComboBox.TextChanged += new System.EventHandler(this.NewWorkspacePlatformsComboBox_TextChanged);
			// 
			// NewWorkspacePlatformHeader
			// 
			this.NewWorkspacePlatformHeader.BackColor = System.Drawing.SystemColors.Control;
			this.NewWorkspacePlatformHeader.CausesValidation = false;
			this.NewWorkspacePlatformHeader.Description = "Each workspace needs a targeted platform.  MOG uses this information to insure th" +
				"at it delivers the appropriate platform specific data to youe workspace.";
			this.NewWorkspacePlatformHeader.Dock = System.Windows.Forms.DockStyle.Top;
			this.NewWorkspacePlatformHeader.Image = ((System.Drawing.Image)(resources.GetObject("NewWorkspacePlatformHeader.Image")));
			this.NewWorkspacePlatformHeader.Location = new System.Drawing.Point(0, 0);
			this.NewWorkspacePlatformHeader.Name = "NewWorkspacePlatformHeader";
			this.NewWorkspacePlatformHeader.Size = new System.Drawing.Size(489, 64);
			this.NewWorkspacePlatformHeader.TabIndex = 0;
			this.NewWorkspacePlatformHeader.Title = "Select desired platform";
			// 
			// wizardPage4
			// 
			this.wizardPage4.Controls.Add(this.label3);
			this.wizardPage4.Controls.Add(this.NewWorkspaceBranchComboBox);
			this.wizardPage4.Controls.Add(this.header1);
			this.wizardPage4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.wizardPage4.IsFinishPage = false;
			this.wizardPage4.Location = new System.Drawing.Point(0, 0);
			this.wizardPage4.Name = "wizardPage4";
			this.wizardPage4.Size = new System.Drawing.Size(489, 305);
			this.wizardPage4.TabIndex = 4;
			this.wizardPage4.ShowFromNext += new System.EventHandler(this.wizardPage4_ShowFromNext);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(16, 120);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(384, 16);
			this.label3.TabIndex = 4;
			this.label3.Text = "Select branch for your local workspace:";
			// 
			// NewWorkspaceBranchComboBox
			// 
			this.NewWorkspaceBranchComboBox.Location = new System.Drawing.Point(16, 136);
			this.NewWorkspaceBranchComboBox.Name = "NewWorkspaceBranchComboBox";
			this.NewWorkspaceBranchComboBox.Size = new System.Drawing.Size(461, 21);
			this.NewWorkspaceBranchComboBox.TabIndex = 3;
			this.NewWorkspaceBranchComboBox.SelectedIndexChanged += new System.EventHandler(this.NewWorkspaceBranchComboBox_SelectedIndexChanged);
			this.NewWorkspaceBranchComboBox.TextChanged += new System.EventHandler(this.NewWorkspaceBranchComboBox_TextChanged);
			// 
			// header1
			// 
			this.header1.BackColor = System.Drawing.SystemColors.Control;
			this.header1.CausesValidation = false;
			this.header1.Description = "In addition to a target platform, MOG also needs a target branch for this workspa" +
				"ce.";
			this.header1.Dock = System.Windows.Forms.DockStyle.Top;
			this.header1.Image = ((System.Drawing.Image)(resources.GetObject("header1.Image")));
			this.header1.Location = new System.Drawing.Point(0, 0);
			this.header1.Name = "header1";
			this.header1.Size = new System.Drawing.Size(489, 64);
			this.header1.TabIndex = 0;
			this.header1.Title = "Workspace Branch";
			// 
			// wizardPage1
			// 
			this.wizardPage1.Controls.Add(this.NewWorkspaceInfoPage);
			this.wizardPage1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.wizardPage1.IsFinishPage = false;
			this.wizardPage1.Location = new System.Drawing.Point(0, 0);
			this.wizardPage1.Name = "wizardPage1";
			this.wizardPage1.Size = new System.Drawing.Size(489, 305);
			this.wizardPage1.TabIndex = 1;
			// 
			// NewWorkspaceInfoPage
			// 
			this.NewWorkspaceInfoPage.BackColor = System.Drawing.Color.White;
			this.NewWorkspaceInfoPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.NewWorkspaceInfoPage.Image = ((System.Drawing.Image)(resources.GetObject("NewWorkspaceInfoPage.Image")));
			this.NewWorkspaceInfoPage.Location = new System.Drawing.Point(0, 0);
			this.NewWorkspaceInfoPage.Name = "NewWorkspaceInfoPage";
			this.NewWorkspaceInfoPage.PageText = "This wizard walks you through the steps of creating a local workpace for your new" +
				"ly logged in project.";
			this.NewWorkspaceInfoPage.PageTitle = "Welcome to the new local workspace wizard";
			this.NewWorkspaceInfoPage.Size = new System.Drawing.Size(489, 305);
			this.NewWorkspaceInfoPage.TabIndex = 0;
			// 
			// wizardPage5
			// 
			this.wizardPage5.Controls.Add(this.infoContainer1);
			this.wizardPage5.Dock = System.Windows.Forms.DockStyle.Fill;
			this.wizardPage5.IsFinishPage = true;
			this.wizardPage5.Location = new System.Drawing.Point(0, 0);
			this.wizardPage5.Name = "wizardPage5";
			this.wizardPage5.Size = new System.Drawing.Size(489, 305);
			this.wizardPage5.TabIndex = 5;
			this.wizardPage5.ShowFromNext += new System.EventHandler(this.wizardPage5_ShowFromNext);
			// 
			// infoContainer1
			// 
			this.infoContainer1.BackColor = System.Drawing.Color.White;
			this.infoContainer1.Controls.Add(this.NewWorkspaceMergeCheckBox);
			this.infoContainer1.Controls.Add(this.NewWorkspacePlatformLabel);
			this.infoContainer1.Controls.Add(this.NewWorkspaceBranchLabel);
			this.infoContainer1.Controls.Add(this.label5);
			this.infoContainer1.Controls.Add(this.label4);
			this.infoContainer1.Controls.Add(this.NewWorkspaceDirectoryLabel);
			this.infoContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.infoContainer1.Image = ((System.Drawing.Image)(resources.GetObject("infoContainer1.Image")));
			this.infoContainer1.Location = new System.Drawing.Point(0, 0);
			this.infoContainer1.Name = "infoContainer1";
			this.infoContainer1.PageTitle = "Your new local workspace is now complete!";
			this.infoContainer1.Size = new System.Drawing.Size(489, 305);
			this.infoContainer1.TabIndex = 0;
			// 
			// NewWorkspaceMergeCheckBox
			// 
			this.NewWorkspaceMergeCheckBox.AutoSize = true;
			this.NewWorkspaceMergeCheckBox.Checked = true;
			this.NewWorkspaceMergeCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.NewWorkspaceMergeCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.NewWorkspaceMergeCheckBox.Location = new System.Drawing.Point(249, 189);
			this.NewWorkspaceMergeCheckBox.Name = "NewWorkspaceMergeCheckBox";
			this.NewWorkspaceMergeCheckBox.Size = new System.Drawing.Size(243, 18);
			this.NewWorkspaceMergeCheckBox.TabIndex = 13;
			this.NewWorkspaceMergeCheckBox.Text = "Update this local workspace with latest build";
			this.NewWorkspaceMergeCheckBox.UseVisualStyleBackColor = true;
			// 
			// NewWorkspacePlatformLabel
			// 
			this.NewWorkspacePlatformLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.NewWorkspacePlatformLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.NewWorkspacePlatformLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.NewWorkspacePlatformLabel.Location = new System.Drawing.Point(248, 152);
			this.NewWorkspacePlatformLabel.Name = "NewWorkspacePlatformLabel";
			this.NewWorkspacePlatformLabel.Size = new System.Drawing.Size(233, 16);
			this.NewWorkspacePlatformLabel.TabIndex = 12;
			this.NewWorkspacePlatformLabel.Text = "c:\\bla";
			this.NewWorkspacePlatformLabel.BackColor = Color.PaleGreen;
			// 
			// NewWorkspaceBranchLabel
			// 
			this.NewWorkspaceBranchLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.NewWorkspaceBranchLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.NewWorkspaceBranchLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.NewWorkspaceBranchLabel.Location = new System.Drawing.Point(248, 136);
			this.NewWorkspaceBranchLabel.Name = "NewWorkspaceBranchLabel";
			this.NewWorkspaceBranchLabel.Size = new System.Drawing.Size(233, 16);
			this.NewWorkspaceBranchLabel.TabIndex = 11;
			this.NewWorkspaceBranchLabel.Text = "c:\\bla";
			this.NewWorkspaceBranchLabel.BackColor = Color.PaleGreen;
			// 
			// label5
			// 
			this.label5.Font = new System.Drawing.Font("Tahoma", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.Location = new System.Drawing.Point(176, 96);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(248, 24);
			this.label5.TabIndex = 10;
			this.label5.Text = "New local workspace";
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(176, 122);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(64, 48);
			this.label4.TabIndex = 9;
			this.label4.Text = "Target: Branch: Platform:";
			// 
			// NewWorkspaceDirectoryLabel
			// 
			this.NewWorkspaceDirectoryLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.NewWorkspaceDirectoryLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.NewWorkspaceDirectoryLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.NewWorkspaceDirectoryLabel.Location = new System.Drawing.Point(248, 120);
			this.NewWorkspaceDirectoryLabel.Name = "NewWorkspaceDirectoryLabel";
			this.NewWorkspaceDirectoryLabel.Size = new System.Drawing.Size(233, 16);
			this.NewWorkspaceDirectoryLabel.TabIndex = 8;
			this.NewWorkspaceDirectoryLabel.Text = "c:\\bla";
			this.NewWorkspaceDirectoryLabel.BackColor = Color.PaleGreen;
			// 
			// wizardPage2
			// 
			this.wizardPage2.Controls.Add(this.NewWorkspaceBrowseButton);
			this.wizardPage2.Controls.Add(this.NewWorkspaceDirectoryTextBox);
			this.wizardPage2.Controls.Add(this.NewWorkspaceHeader);
			this.wizardPage2.Controls.Add(this.label1);
			this.wizardPage2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.wizardPage2.IsFinishPage = false;
			this.wizardPage2.Location = new System.Drawing.Point(0, 0);
			this.wizardPage2.Name = "wizardPage2";
			this.wizardPage2.Size = new System.Drawing.Size(489, 305);
			this.wizardPage2.TabIndex = 2;
			this.wizardPage2.ShowFromNext += new System.EventHandler(this.wizardPage2_ShowFromNext);
			// 
			// NewWorkspaceBrowseButton
			// 
			this.NewWorkspaceBrowseButton.Location = new System.Drawing.Point(453, 136);
			this.NewWorkspaceBrowseButton.Name = "NewWorkspaceBrowseButton";
			this.NewWorkspaceBrowseButton.Size = new System.Drawing.Size(24, 21);
			this.NewWorkspaceBrowseButton.TabIndex = 2;
			this.NewWorkspaceBrowseButton.Text = "...";
			this.NewWorkspaceBrowseButton.Click += new System.EventHandler(this.NewWorkspaceBrowseButton_Click);
			// 
			// NewWorkspaceDirectoryTextBox
			// 
			this.NewWorkspaceDirectoryTextBox.Location = new System.Drawing.Point(16, 136);
			this.NewWorkspaceDirectoryTextBox.Name = "NewWorkspaceDirectoryTextBox";
			this.NewWorkspaceDirectoryTextBox.Size = new System.Drawing.Size(431, 21);
			this.NewWorkspaceDirectoryTextBox.TabIndex = 1;
			this.NewWorkspaceDirectoryTextBox.TextChanged += new System.EventHandler(this.NewWorkspaceDirectoryTextBox_TextChanged);
			// 
			// NewWorkspaceHeader
			// 
			this.NewWorkspaceHeader.BackColor = System.Drawing.SystemColors.Control;
			this.NewWorkspaceHeader.CausesValidation = false;
			this.NewWorkspaceHeader.Description = "Enter directory for MOG to save all synced data for this new project.";
			this.NewWorkspaceHeader.Dock = System.Windows.Forms.DockStyle.Top;
			this.NewWorkspaceHeader.Image = ((System.Drawing.Image)(resources.GetObject("NewWorkspaceHeader.Image")));
			this.NewWorkspaceHeader.Location = new System.Drawing.Point(0, 0);
			this.NewWorkspaceHeader.Name = "NewWorkspaceHeader";
			this.NewWorkspaceHeader.Size = new System.Drawing.Size(489, 64);
			this.NewWorkspaceHeader.TabIndex = 0;
			this.NewWorkspaceHeader.Title = "Select target directory";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 120);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(392, 23);
			this.label1.TabIndex = 3;
			this.label1.Text = "Enter or browse to a directory to become your new local workspace:";
			// 
			// CreateNewWorkspaceWizard
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(489, 353);
			this.Controls.Add(this.NewWorkspaceWizard);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "CreateNewWorkspaceWizard";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Create New Workspace Wizard";
			this.NewWorkspaceWizard.ResumeLayout(false);
			this.wizardPage3.ResumeLayout(false);
			this.wizardPage4.ResumeLayout(false);
			this.wizardPage1.ResumeLayout(false);
			this.wizardPage5.ResumeLayout(false);
			this.infoContainer1.ResumeLayout(false);
			this.infoContainer1.PerformLayout();
			this.wizardPage2.ResumeLayout(false);
			this.wizardPage2.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		private void NewWorkspaceBrowseButton_Click(object sender, System.EventArgs e)
		{
			BrowseWorkspaceDirectory form = new BrowseWorkspaceDirectory();
			form.ShowDialog(this);
			if( form.DialogResult == DialogResult.OK)
			{
				this.NewWorkspaceDirectoryTextBox.Text = form.SelectedFolder;
			}
		}

		private void wizardPage2_ShowFromNext(object sender, System.EventArgs e)
		{
			UpdateNewWorkspaceDirectoryTextBox();
		}

		private void wizardPage3_ShowFromNext(object sender, System.EventArgs e)
		{
			UpdateNewWorkspacePlatformsComboBox();
		}

		private void wizardPage4_ShowFromNext(object sender, System.EventArgs e)
		{
			UpdateNewWorkspaceBranchComboBox();
		}

		private void wizardPage5_ShowFromNext(object sender, System.EventArgs e)
		{
			NewWorkspaceDirectoryLabel.Text = NewWorkspaceDirectory;
			NewWorkspaceDirectoryLabel.BackColor = NewWorkspaceDirectoryTextBox.BackColor;

			NewWorkspaceBranchLabel.Text = NewWorkspaceBranch;
			NewWorkspaceBranchLabel.BackColor = NewWorkspaceBranchComboBox.BackColor;

			NewWorkspacePlatformLabel.Text = NewWorkspacePlatform;
			NewWorkspacePlatformLabel.BackColor = NewWorkspacePlatformsComboBox.BackColor;
		}

		private void NewWorkspaceBranchComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateNewWorkspaceBranchComboBox();
		}

		private void NewWorkspacePlatformsComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateNewWorkspacePlatformsComboBox();
			AutoGenerateWorkspaceDirectory();
		}

		private void NewWorkspaceBranchComboBox_TextChanged(object sender, EventArgs e)
		{
			UpdateNewWorkspaceBranchComboBox();
		}

		private void UpdateNewWorkspaceBranchComboBox()
		{
			NewWorkspaceBranchComboBox.BackColor = Color.Tomato;

			// Check if the selected text matches one of our branches?
			foreach (string branch in NewWorkspaceBranchComboBox.Items)
			{
				// Check for a match
				if (string.Compare(branch, NewWorkspaceBranchComboBox.Text, true) == 0)
				{
					// Looks like this is a valid branch
					NewWorkspaceBranchComboBox.BackColor = Color.PaleGreen;

					// Reload the available platforms for this branch
					ArrayList platforms = MOG_DBProjectAPI.GetPlatformsInBranch(branch);
					NewWorkspacePlatformsComboBox.Items.Clear();
					NewWorkspacePlatformsComboBox.Items.AddRange(platforms.ToArray());
					NewWorkspacePlatformsComboBox.SelectedIndex = 0;

					// Update the platform ComboBox
					UpdateNewWorkspacePlatformsComboBox();
				}
			}

			// Check if the user can continue?
			NewWorkspaceWizard.NextEnabled = (NewWorkspaceBranchComboBox.BackColor == Color.Tomato) ? false : true;
		}

		private void UpdateNewWorkspacePlatformsComboBox()
		{
			NewWorkspacePlatformsComboBox.BackColor = Color.Tomato;

			// Check if the selected text matches one of our branches?
			foreach (string platform in NewWorkspacePlatformsComboBox.Items)
			{
				// Check for a match
				if (string.Compare(platform, NewWorkspacePlatformsComboBox.Text, true) == 0)
				{
					// Looks like this is a valid branch
					NewWorkspacePlatformsComboBox.BackColor = Color.PaleGreen;
				}
			}

			// Attempt to auto generate the sync path
			AutoGenerateWorkspaceDirectory();

			// Check if the user can continue?
			NewWorkspaceWizard.NextEnabled = (NewWorkspacePlatformsComboBox.BackColor == Color.Tomato) ? false : true;
		}

		private void AutoGenerateWorkspaceDirectory()
		{
			// Initialize default workspace path
			string path = "C:\\" + MOG_ControllerProject.GetProjectName();
			// Check if there is more than one platform?
			if (NewWorkspacePlatformsComboBox.Items.Count > 1)
			{
				// Append on the platform identifier
				path += "." + NewWorkspacePlatformsComboBox.Text;
			}
			NewWorkspaceDirectoryTextBox.Text = path;
		}

		private void NewWorkspacePlatformsComboBox_TextChanged(object sender, EventArgs e)
		{
			UpdateNewWorkspacePlatformsComboBox();
			AutoGenerateWorkspaceDirectory();
		}

		private void NewWorkspaceDirectoryTextBox_TextChanged(object sender, EventArgs e)
		{
			UpdateNewWorkspaceDirectoryTextBox();
		}

		private void UpdateNewWorkspaceDirectoryTextBox()
		{
			NewWorkspaceDirectoryTextBox.BackColor = Color.Tomato;

			if (NewWorkspaceDirectoryTextBox.Text.Length > 0)
			{
				// Lets make sure that the selected target has a path root and at least one directory
				// Make sure this path is rooted
				if (!MOG_ControllerSystem.InvalidWindowsPathCharactersCheck(NewWorkspaceDirectoryTextBox.Text, true) &&
					Path.IsPathRooted(NewWorkspaceDirectoryTextBox.Text))
				{
					NewWorkspaceDirectoryTextBox.BackColor = Color.PaleGreen;
				}
			}

			// Check if the user can continue?
			NewWorkspaceWizard.NextEnabled = (NewWorkspaceDirectoryTextBox.BackColor == Color.Tomato) ? false : true;
		}
	}
}
