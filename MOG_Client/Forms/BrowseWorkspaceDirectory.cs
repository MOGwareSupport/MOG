using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using MOG.DOSUTILS;
using MOG.PROMPT;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG_ControlsLibrary.Common.MogControl_LocalBranchTreeView;

namespace MOG_Client.Forms
{
	/// <summary>
	/// Summary description for BrowseWorkspaceDirectory.
	/// </summary>
	public class BrowseWorkspaceDirectory : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button BrowseOkButton;
		private System.Windows.Forms.Button BrowseCancelButton;
		private MogControl_LocalBranchTreeView MogControl_LocalBranchTreeView;
		private System.Windows.Forms.TextBox BrowseTargetTextBox;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public string SelectedFolder
		{
			get { return BrowseTargetTextBox.Text; }
		}

		public BrowseWorkspaceDirectory()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			MogControl_LocalBranchTreeView.InitializeDrivesOnly();

			// Set default
			this.BrowseTargetTextBox.Text = "C:\\" + MOG_ControllerProject.GetProjectName();
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(BrowseWorkspaceDirectory));
			this.MogControl_LocalBranchTreeView = new MOG_ControlsLibrary.Common.MogControl_LocalBranchTreeView.MogControl_LocalBranchTreeView();
			this.BrowseOkButton = new System.Windows.Forms.Button();
			this.BrowseCancelButton = new System.Windows.Forms.Button();
			this.BrowseTargetTextBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// MogControl_LocalBranchTreeView
			// 
			this.MogControl_LocalBranchTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.MogControl_LocalBranchTreeView.Location = new System.Drawing.Point(5, 32);
			this.MogControl_LocalBranchTreeView.MOGAllowDirectoryOpperations = true;
			this.MogControl_LocalBranchTreeView.MOGShowFiles = false;
			this.MogControl_LocalBranchTreeView.MOGSorted = true;
			this.MogControl_LocalBranchTreeView.Name = "MogControl_LocalBranchTreeView";
			this.MogControl_LocalBranchTreeView.Size = new System.Drawing.Size(280, 192);
			this.MogControl_LocalBranchTreeView.TabIndex = 0;
			this.MogControl_LocalBranchTreeView.MOGAfterTargetSelect += new MOG_ControlsLibrary.Common.MogControl_LocalBranchTreeView.MogControl_LocalBranchTreeView.TreeViewEvent(this.MogControl_LocalBranchTreeView_MOGAfterTargetSelect);
			// 
			// BrowseOkButton
			// 
			this.BrowseOkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.BrowseOkButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.BrowseOkButton.Location = new System.Drawing.Point(208, 240);
			this.BrowseOkButton.Name = "BrowseOkButton";
			this.BrowseOkButton.TabIndex = 1;
			this.BrowseOkButton.Text = "Ok";
			// 
			// BrowseCancelButton
			// 
			this.BrowseCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.BrowseCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.BrowseCancelButton.Location = new System.Drawing.Point(128, 240);
			this.BrowseCancelButton.Name = "BrowseCancelButton";
			this.BrowseCancelButton.TabIndex = 2;
			this.BrowseCancelButton.Text = "Cancel";
			// 
			// BrowseTargetTextBox
			// 
			this.BrowseTargetTextBox.Location = new System.Drawing.Point(5, 8);
			this.BrowseTargetTextBox.Name = "BrowseTargetTextBox";
			this.BrowseTargetTextBox.Size = new System.Drawing.Size(281, 20);
			this.BrowseTargetTextBox.TabIndex = 3;
			this.BrowseTargetTextBox.Text = "";
			this.BrowseTargetTextBox.TextChanged += new System.EventHandler(this.BrowseTargetTextBox_TextChanged);
			// 
			// BrowseWorkspaceDirectory
			// 
			this.AcceptButton = this.BrowseOkButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.BrowseCancelButton;
			this.ClientSize = new System.Drawing.Size(292, 269);
			this.Controls.Add(this.BrowseTargetTextBox);
			this.Controls.Add(this.BrowseCancelButton);
			this.Controls.Add(this.BrowseOkButton);
			this.Controls.Add(this.MogControl_LocalBranchTreeView);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.Name = "BrowseWorkspaceDirectory";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Browse Workspace Directory";
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.BrowseWorkspaceDirectory_KeyDown);
			this.Closing += new System.ComponentModel.CancelEventHandler(this.BrowseWorkspaceDirectory_Closing);
			this.ResumeLayout(false);

		}
		#endregion

		private void BrowseWorkspaceDirectory_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (this.DialogResult == DialogResult.OK)
			{
				if (!DosUtils.DirectoryExist(this.BrowseTargetTextBox.Text))
				{
					if (!DosUtils.DirectoryCreate(this.BrowseTargetTextBox.Text))
					{
						MOG_Prompt.PromptMessage("Create local Workspace", "Could not create target directory(" + this.BrowseTargetTextBox.Text + ")!");
						e.Cancel = true;
					}
				}
			}
		}

		private void MogControl_LocalBranchTreeView_MOGAfterTargetSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			// Set default
			this.BrowseTargetTextBox.Text = MogControl_LocalBranchTreeView.MOGGameDataTarget;
			AutoGenerateSyncPath();
			UpdateBrowseTargetTextBox();
		}

		private void BrowseTargetTextBox_TextChanged(object sender, System.EventArgs e)
		{
			UpdateBrowseTargetTextBox();
		}

		private void AutoGenerateSyncPath()
		{
			// Check if there is anything in there yet?
			if (BrowseTargetTextBox.Text.Length > 0)
			{
				// Get the project information for building the default dire name
				string path = BrowseTargetTextBox.Text;
				// Check if this path is missing the name of the project?
				string projName = MOG_ControllerProject.GetProjectName();
				if (path.IndexOf(projName, StringComparison.CurrentCultureIgnoreCase) == -1)
				{
					string dirName = projName;

// This would be nice but this control knows nothing about the wizard's selected branch
//					// Check if there is more than one platform?
//					if (NewLocalPlaformComboBox.Items.Count > 1)
//					{
//						// Append on the specified platform
//						string platform = NewLocalPlaformComboBox.Text;
//						dirName += "." + platform;
//					}
					// Append on the generate dirName
					path = Path.Combine(path, dirName);
				}

				// Use the local branch tree view and append on the project's name & platform
				BrowseTargetTextBox.Text = path;
			}
		}

		private void UpdateBrowseTargetTextBox()
		{
			BrowseTargetTextBox.BackColor = Color.Tomato;

			if (BrowseTargetTextBox.Text.Length > 0)
			{
				// Lets make sure that the selected target has a path root and at least one directory
				string root = Path.GetPathRoot(BrowseTargetTextBox.Text.TrimStart("\\".ToCharArray()));
				// Do we have a drive letter?
				if (root.Length > 0)
				{
					string dir = Path.GetDirectoryName(BrowseTargetTextBox.Text);
					if (dir != null && dir.Length > 0)
					{
						// Clean it up
						root = root.TrimEnd("\\".ToCharArray()).ToLower();

						// Does the target have that root with a '\' and at least one directory folder
						if (BrowseTargetTextBox.Text.ToLower().StartsWith(root + "\\") == true && dir.Length > 0)
						{
							BrowseTargetTextBox.BackColor = Color.PaleGreen;
							return;
						}
					}
				}
			}
		}

		private void BrowseWorkspaceDirectory_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			UpdateBrowseTargetTextBox();
		}
	}
}
