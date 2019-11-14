using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG_Client.Client_Gui;
using MOG.DOSUTILS;
using MOG.PROMPT;

using MOG_ControlsLibrary;
using System.Collections.Generic;
using MOG_ControlsLibrary.Common.MogControl_LocalBranchTreeView;
using MOG.DATABASE;
using MOG.CONTROLLER.CONTROLLERSYSTEM;

namespace MOG_Client.Forms
{
	/// <summary>
	/// Summary description for CreateNewLocalBranchForm.
	/// </summary>
	public class CreateNewLocalBranchForm : System.Windows.Forms.Form
	{
		#region Form variables
		public System.Windows.Forms.ComboBox NewLocalPlaformComboBox;
		private System.Windows.Forms.Button NewLocalOkButton;
		private System.Windows.Forms.Button NewLocalCancelButton;
		public System.Windows.Forms.ComboBox NewLocalBranchComboBox;
		public MogControl_LocalBranchTreeView NewLocalMogControl_LocalBranchTreeView;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		public System.Windows.Forms.TextBox NewLocalTargetTextBox;
		private System.Windows.Forms.Label label4;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		#endregion

		public CreateNewLocalBranchForm()
		{
			InitializeComponent();

			NewLocalMogControl_LocalBranchTreeView.MOGSorted = true;

			// Initialize the local branch view to show only drives
			NewLocalMogControl_LocalBranchTreeView.InitializeDrivesOnly();

			// Intialize the branch
			List<string> branches = guiStartup.GetBranches(MOG_ControllerProject.GetProjectName());
			NewLocalBranchComboBox.Items.Clear();
			NewLocalBranchComboBox.Items.AddRange(branches.ToArray());
			NewLocalBranchComboBox.SelectedIndex = 0;

			NewLocalBranchComboBox.Text = MOG_ControllerProject.GetBranchName();
			UpdateLocalBranchComboBox();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateNewLocalBranchForm));
			this.NewLocalPlaformComboBox = new System.Windows.Forms.ComboBox();
			this.NewLocalOkButton = new System.Windows.Forms.Button();
			this.NewLocalCancelButton = new System.Windows.Forms.Button();
			this.NewLocalBranchComboBox = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.NewLocalTargetTextBox = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.NewLocalMogControl_LocalBranchTreeView = new MOG_ControlsLibrary.Common.MogControl_LocalBranchTreeView.MogControl_LocalBranchTreeView();
			this.SuspendLayout();
			// 
			// NewLocalPlaformComboBox
			// 
			this.NewLocalPlaformComboBox.Location = new System.Drawing.Point(8, 62);
			this.NewLocalPlaformComboBox.Name = "NewLocalPlaformComboBox";
			this.NewLocalPlaformComboBox.Size = new System.Drawing.Size(121, 21);
			this.NewLocalPlaformComboBox.TabIndex = 1;
			this.NewLocalPlaformComboBox.Text = "Select platform";
			this.NewLocalPlaformComboBox.TextChanged += new System.EventHandler(this.NewLocalPlaformComboBox_TextChanged);
			// 
			// NewLocalOkButton
			// 
			this.NewLocalOkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.NewLocalOkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.NewLocalOkButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.NewLocalOkButton.Location = new System.Drawing.Point(158, 298);
			this.NewLocalOkButton.Name = "NewLocalOkButton";
			this.NewLocalOkButton.Size = new System.Drawing.Size(75, 23);
			this.NewLocalOkButton.TabIndex = 3;
			this.NewLocalOkButton.Text = "OK";
			// 
			// NewLocalCancelButton
			// 
			this.NewLocalCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.NewLocalCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.NewLocalCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.NewLocalCancelButton.Location = new System.Drawing.Point(78, 298);
			this.NewLocalCancelButton.Name = "NewLocalCancelButton";
			this.NewLocalCancelButton.Size = new System.Drawing.Size(75, 23);
			this.NewLocalCancelButton.TabIndex = 4;
			this.NewLocalCancelButton.Text = "Cancel";
			// 
			// NewLocalBranchComboBox
			// 
			this.NewLocalBranchComboBox.Location = new System.Drawing.Point(8, 16);
			this.NewLocalBranchComboBox.Name = "NewLocalBranchComboBox";
			this.NewLocalBranchComboBox.Size = new System.Drawing.Size(121, 21);
			this.NewLocalBranchComboBox.TabIndex = 5;
			this.NewLocalBranchComboBox.Text = "Branch";
			this.NewLocalBranchComboBox.TextChanged += new System.EventHandler(this.NewLocalBranchComboBox_TextChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(192, 16);
			this.label1.TabIndex = 7;
			this.label1.Text = "Select current branch";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 46);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(192, 16);
			this.label2.TabIndex = 8;
			this.label2.Text = "Select current platform";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 144);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(192, 16);
			this.label3.TabIndex = 9;
			this.label3.Text = "Select target directory for update";
			// 
			// NewLocalTargetTextBox
			// 
			this.NewLocalTargetTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.NewLocalTargetTextBox.Location = new System.Drawing.Point(8, 112);
			this.NewLocalTargetTextBox.Name = "NewLocalTargetTextBox";
			this.NewLocalTargetTextBox.Size = new System.Drawing.Size(224, 20);
			this.NewLocalTargetTextBox.TabIndex = 10;
			this.NewLocalTargetTextBox.TextChanged += new System.EventHandler(this.NewLocalTargetTextBox_TextChanged);
			this.NewLocalTargetTextBox.Validated += new System.EventHandler(this.NewLocalTargetTextBox_Validated);
			this.NewLocalTargetTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CreateNewLocalBranchForm_KeyDown);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(8, 96);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(192, 16);
			this.label4.TabIndex = 11;
			this.label4.Text = "Current Target Workspace";
			// 
			// NewLocalMogControl_LocalBranchTreeView
			// 
			this.NewLocalMogControl_LocalBranchTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.NewLocalMogControl_LocalBranchTreeView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.NewLocalMogControl_LocalBranchTreeView.Location = new System.Drawing.Point(8, 160);
			this.NewLocalMogControl_LocalBranchTreeView.MOGAllowDirectoryOpperations = true;
			this.NewLocalMogControl_LocalBranchTreeView.MOGShowFiles = false;
			this.NewLocalMogControl_LocalBranchTreeView.MOGSorted = false;
			this.NewLocalMogControl_LocalBranchTreeView.Name = "NewLocalMogControl_LocalBranchTreeView";
			this.NewLocalMogControl_LocalBranchTreeView.Size = new System.Drawing.Size(230, 136);
			this.NewLocalMogControl_LocalBranchTreeView.TabIndex = 6;
			this.NewLocalMogControl_LocalBranchTreeView.MOGAfterTargetSelect += new MOG_ControlsLibrary.Common.MogControl_LocalBranchTreeView.MogControl_LocalBranchTreeView.TreeViewEvent(this.NewLocalMogControl_LocalBranchTreeView_MOGAfterTargetSelect);
			// 
			// CreateNewLocalBranchForm
			// 
			this.AcceptButton = this.NewLocalOkButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.NewLocalCancelButton;
			this.ClientSize = new System.Drawing.Size(248, 329);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.NewLocalTargetTextBox);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.NewLocalMogControl_LocalBranchTreeView);
			this.Controls.Add(this.NewLocalBranchComboBox);
			this.Controls.Add(this.NewLocalCancelButton);
			this.Controls.Add(this.NewLocalOkButton);
			this.Controls.Add(this.NewLocalPlaformComboBox);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.Name = "CreateNewLocalBranchForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Create new Workspace";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.CreateNewLocalBranchForm_Closing);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CreateNewLocalBranchForm_KeyDown);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// Prepare the target workspace directory by creating a folder as designated by the NewLocalTargetTextBox
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CreateNewLocalBranchForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (this.DialogResult == DialogResult.OK)
			{
				if (!DosUtils.DirectoryExist(this.NewLocalTargetTextBox.Text))
				{
					if (!DosUtils.DirectoryCreate(this.NewLocalTargetTextBox.Text))
					{
						MOG_Prompt.PromptMessage("Create local Workspace", "Could not create target directory(" + this.NewLocalTargetTextBox.Text + ")!");
						e.Cancel = true;
					}
				}
			}
		}

		/// <summary>
		/// Update the NewLocalTargetTextBox with the location of the selected tree node
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void NewLocalMogControl_LocalBranchTreeView_MOGAfterTargetSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			// Attempt to auto generate the sync path
			AutoGenerateSyncPath();

			UpdateOkButton();
		}

		/// <summary>
		/// Double check that all selected targets are valid directories and optionally append the project name to all selected TreeNodes
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void NewLocalTargetTextBox_TextChanged(object sender, System.EventArgs e)
		{
			UpdateLocalTargetTextBox();
		}

		private void UpdateLocalTargetTextBox()
		{
			NewLocalTargetTextBox.BackColor = Color.Tomato;

			// Check if there is anything in there yet?
			if (NewLocalTargetTextBox.Text.Length > 0)
			{
				// Lets make sure that the selected target has a path root and at least one directory
				// Make sure this path is rooted
				if (!MOG_ControllerSystem.InvalidWindowsPathCharactersCheck(NewLocalTargetTextBox.Text, true) &&
					Path.IsPathRooted(NewLocalTargetTextBox.Text))
				{
					NewLocalTargetTextBox.BackColor = Color.PaleGreen;
				}
			}
		}

		private void AutoGenerateSyncPath()
		{
			// Only auto generate when we have a path selected in the tree view
			if (NewLocalMogControl_LocalBranchTreeView.MOGGameDataTarget.Length > 0)
			{
				// Get the project information for building the default dire name
				string path = NewLocalMogControl_LocalBranchTreeView.MOGGameDataTarget;
				// Check if this path is missing the name of the project?
				string projName = MOG_ControllerProject.GetProjectName();
				if (path.IndexOf(projName, StringComparison.CurrentCultureIgnoreCase) == -1)
				{
					string dirName = projName;

					// Check if there is more than one platform?
					if (NewLocalPlaformComboBox.Items.Count > 1)
					{
						// Append on the specified platform
						string platform = NewLocalPlaformComboBox.Text;
						dirName += "." + platform;
					}
					// Append on the generate dirName
					path = Path.Combine(path, dirName);
				}

				// Use the local branch tree view and append on the project's name & platform
				NewLocalTargetTextBox.Text = path;
			}
		}

		/// <summary>
		/// Abort the adding of the project name to every change of the target directory
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CreateNewLocalBranchForm_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
		}

		private void NewLocalTargetTextBox_Validated(object sender, EventArgs e)
		{
			// Make sure we trim off any trailing '\'
			NewLocalTargetTextBox.Text = NewLocalTargetTextBox.Text.Trim("\\".ToCharArray());
		}

		private void NewLocalBranchComboBox_TextChanged(object sender, EventArgs e)
		{
			UpdateLocalBranchComboBox();
		}

		private void UpdateLocalBranchComboBox()
		{
			NewLocalBranchComboBox.BackColor = Color.Tomato;

			// Check if the selected text matches one of our branches?
			foreach (string branch in NewLocalBranchComboBox.Items)
			{
				// Check for a match
				if (string.Compare(branch, NewLocalBranchComboBox.Text, true) == 0)
				{
					// Looks like this is a valid branch
					NewLocalBranchComboBox.BackColor = Color.PaleGreen;

					// Reload the available platforms for this branch
					ArrayList platforms = MOG_DBProjectAPI.GetPlatformsInBranch(branch);
					NewLocalPlaformComboBox.Items.Clear();
					NewLocalPlaformComboBox.Items.AddRange(platforms.ToArray());
					NewLocalPlaformComboBox.SelectedIndex = 0;
					// Update the platform ComboBox
					UpdateLocalPlaformComboBox();
				}
			}

			UpdateOkButton();
		}

		private void NewLocalPlaformComboBox_TextChanged(object sender, EventArgs e)
		{
			UpdateLocalPlaformComboBox();
		}

		private void UpdateLocalPlaformComboBox()
		{
			NewLocalPlaformComboBox.BackColor = Color.Tomato;

			// Check if the selected text matches one of our branches?
			foreach (string platform in NewLocalPlaformComboBox.Items)
			{
				// Check for a match
				if (string.Compare(platform, NewLocalPlaformComboBox.Text, true) == 0)
				{
					// Looks like this is a valid branch
					NewLocalPlaformComboBox.BackColor = Color.PaleGreen;
				}
			}

			// Update the sync target
			UpdateLocalTargetTextBox();

			// Attempt to auto generate the sync path
			AutoGenerateSyncPath();

			UpdateOkButton();
		}

		private void UpdateOkButton()
		{
			NewLocalOkButton.Enabled = false;

			// Make sure we don't have any red
			if (NewLocalBranchComboBox.BackColor != Color.Tomato &&
				NewLocalPlaformComboBox.BackColor != Color.Tomato &&
				NewLocalTargetTextBox.BackColor != Color.Tomato)
			{
				NewLocalOkButton.Enabled = true;
			}
		}
	}
}
