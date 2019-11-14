using System;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using MOG.COMMAND;
using MOG.PROPERTIES;
using MOG.PROJECT;
using MOG.DATABASE;
using MOG.FILENAME;
using MOG;
using MOG.PROMPT;
using MOG.PROGRESS;
using MOG.CONTROLLER.CONTROLLERASSET;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERLIBRARY;
using MOG.CONTROLLER.CONTROLLERREPOSITORY;

using MOG_Client;
using MOG_Client.Client_Utilities;
using MOG_Client.Client_Mog_Utilities;
using MOG_Client.Client_Mog_Utilities.AssetOptions;
using MOG_ControlsLibrary.Controls;
using MOG.DOSUTILS;
using MOG_Client.Forms;
using System.Collections.Generic;

namespace MOG_ControlsLibrary.Common
{
	/// <summary>
	/// Summary description for MogControl_LibraryViewer.
	/// </summary>
	public class MogControl_LibraryExplorer : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Splitter splitter;
		private System.Windows.Forms.Panel LibraryTargetPanel;
		private System.Windows.Forms.TextBox LibraryTargetTextBox;
		private System.Windows.Forms.Button LibraryBrowseButton;
		private System.Windows.Forms.Label LibraryTargetLabel;
		public MOG_ControlsLibrary.Common.MogControl_RepositoryTreeViews.MogControl_LibraryTreeView LibraryTreeView;
        public MOG_ControlsLibrary.Common.MogControl_LibraryListView.MogControl_LibraryListView LibraryListView;
		private PictureBox LibraryWorkingPictureBox;
		private IContainer components;
		private Panel LibraryTopPanel;
		private Button LibraryRefreshButton;
		private CheckBox Library_AutoImport_CheckBox;
		private ToolTip ExplorerToolTip;
		private Button LibraryExploreButton;

		public MogControl_LibraryExplorer()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
		}

		public void Initialize(MethodInvoker onCompletedEvent)
		{
			this.LibraryListView.Initialize();
			this.LibraryListView.LibraryExplorer = this;

			// Initialize the icons
			this.LibraryTreeView.ImageList = MOG_ControlsLibrary.Utils.MogUtil_AssetIcons.Images;

			// Show our sync target
			string workingDir = guiUserPrefs.LoadPref("Library", "TargetDirectory");
			
			if (workingDir == null || workingDir.Length == 0)
			{
				workingDir = @"C:\MOG_Library";
				MogUtils_Settings.MogUtils_Settings.SaveSetting("Library", "TargetDirectory", workingDir);
			}

			MOG_ControllerLibrary.SetWorkingDirectory(workingDir);
			LibraryTargetTextBox.Text = "(" + workingDir + ")";
			LibraryTargetTextBox.Tag = workingDir;

			this.LibraryTreeView.Initialize(onCompletedEvent);			
			this.LibraryTreeView.LibraryExplorer = this;
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.splitter = new System.Windows.Forms.Splitter();
			this.LibraryTargetPanel = new System.Windows.Forms.Panel();
			this.LibraryExploreButton = new System.Windows.Forms.Button();
			this.LibraryWorkingPictureBox = new System.Windows.Forms.PictureBox();
			this.LibraryTargetLabel = new System.Windows.Forms.Label();
			this.LibraryBrowseButton = new System.Windows.Forms.Button();
			this.LibraryTargetTextBox = new System.Windows.Forms.TextBox();
			this.LibraryTopPanel = new System.Windows.Forms.Panel();
			this.Library_AutoImport_CheckBox = new System.Windows.Forms.CheckBox();
			this.LibraryRefreshButton = new System.Windows.Forms.Button();
			this.LibraryListView = new MOG_ControlsLibrary.Common.MogControl_LibraryListView.MogControl_LibraryListView();
			this.LibraryTreeView = new MOG_ControlsLibrary.Common.MogControl_RepositoryTreeViews.MogControl_LibraryTreeView();
			this.ExplorerToolTip = new System.Windows.Forms.ToolTip(this.components);
			this.LibraryTargetPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.LibraryWorkingPictureBox)).BeginInit();
			this.LibraryTopPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitter
			// 
			this.splitter.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.splitter.Location = new System.Drawing.Point(256, 33);
			this.splitter.Name = "splitter";
			this.splitter.Size = new System.Drawing.Size(8, 325);
			this.splitter.TabIndex = 0;
			this.splitter.TabStop = false;
			// 
			// LibraryTargetPanel
			// 
			this.LibraryTargetPanel.BackColor = System.Drawing.SystemColors.Control;
			this.LibraryTargetPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.LibraryTargetPanel.Controls.Add(this.LibraryExploreButton);
			this.LibraryTargetPanel.Controls.Add(this.LibraryWorkingPictureBox);
			this.LibraryTargetPanel.Controls.Add(this.LibraryTargetLabel);
			this.LibraryTargetPanel.Controls.Add(this.LibraryBrowseButton);
			this.LibraryTargetPanel.Controls.Add(this.LibraryTargetTextBox);
			this.LibraryTargetPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.LibraryTargetPanel.Location = new System.Drawing.Point(0, 358);
			this.LibraryTargetPanel.Name = "LibraryTargetPanel";
			this.LibraryTargetPanel.Size = new System.Drawing.Size(904, 50);
			this.LibraryTargetPanel.TabIndex = 2;
			// 
			// LibraryExploreButton
			// 
			this.LibraryExploreButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.LibraryExploreButton.Location = new System.Drawing.Point(846, 21);
			this.LibraryExploreButton.Name = "LibraryExploreButton";
			this.LibraryExploreButton.Size = new System.Drawing.Size(53, 24);
			this.LibraryExploreButton.TabIndex = 4;
			this.LibraryExploreButton.Text = "Explore";
			this.LibraryExploreButton.UseVisualStyleBackColor = true;
			this.LibraryExploreButton.Click += new System.EventHandler(this.LibraryExploreButton_Click);
			// 
			// LibraryWorkingPictureBox
			// 
			this.LibraryWorkingPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.LibraryWorkingPictureBox.Image = global::MOG_Client.Properties.Resources.MOGLibrarytarget;
			this.LibraryWorkingPictureBox.Location = new System.Drawing.Point(3, 0);
			this.LibraryWorkingPictureBox.Name = "LibraryWorkingPictureBox";
			this.LibraryWorkingPictureBox.Size = new System.Drawing.Size(49, 48);
			this.LibraryWorkingPictureBox.TabIndex = 3;
			this.LibraryWorkingPictureBox.TabStop = false;
			// 
			// LibraryTargetLabel
			// 
			this.LibraryTargetLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.LibraryTargetLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LibraryTargetLabel.Location = new System.Drawing.Point(58, -3);
			this.LibraryTargetLabel.Name = "LibraryTargetLabel";
			this.LibraryTargetLabel.Size = new System.Drawing.Size(146, 23);
			this.LibraryTargetLabel.TabIndex = 2;
			this.LibraryTargetLabel.Text = "Library Working Folder:";
			this.LibraryTargetLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// LibraryBrowseButton
			// 
			this.LibraryBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.LibraryBrowseButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.LibraryBrowseButton.Location = new System.Drawing.Point(821, 21);
			this.LibraryBrowseButton.Name = "LibraryBrowseButton";
			this.LibraryBrowseButton.Size = new System.Drawing.Size(24, 24);
			this.LibraryBrowseButton.TabIndex = 1;
			this.LibraryBrowseButton.Text = "...";
			this.LibraryBrowseButton.Click += new System.EventHandler(this.LibraryBrowseButton_Click);
			// 
			// LibraryTargetTextBox
			// 
			this.LibraryTargetTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.LibraryTargetTextBox.Location = new System.Drawing.Point(61, 23);
			this.LibraryTargetTextBox.Name = "LibraryTargetTextBox";
			this.LibraryTargetTextBox.ReadOnly = true;
			this.LibraryTargetTextBox.Size = new System.Drawing.Size(756, 20);
			this.LibraryTargetTextBox.TabIndex = 0;
			// 
			// LibraryTopPanel
			// 
			this.LibraryTopPanel.Controls.Add(this.Library_AutoImport_CheckBox);
			this.LibraryTopPanel.Controls.Add(this.LibraryRefreshButton);
			this.LibraryTopPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.LibraryTopPanel.Location = new System.Drawing.Point(0, 0);
			this.LibraryTopPanel.Name = "LibraryTopPanel";
			this.LibraryTopPanel.Size = new System.Drawing.Size(904, 33);
			this.LibraryTopPanel.TabIndex = 3;
			// 
			// Library_AutoImport_CheckBox
			// 
			this.Library_AutoImport_CheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.Library_AutoImport_CheckBox.AutoSize = true;
			this.Library_AutoImport_CheckBox.Checked = true;
			this.Library_AutoImport_CheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.Library_AutoImport_CheckBox.Location = new System.Drawing.Point(737, 8);
			this.Library_AutoImport_CheckBox.Name = "Library_AutoImport_CheckBox";
			this.Library_AutoImport_CheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.Library_AutoImport_CheckBox.Size = new System.Drawing.Size(80, 17);
			this.Library_AutoImport_CheckBox.TabIndex = 7;
			this.Library_AutoImport_CheckBox.Text = "Auto Import";
			this.ExplorerToolTip.SetToolTip(this.Library_AutoImport_CheckBox, "Attempt to auto assign library location based on file path");
			this.Library_AutoImport_CheckBox.UseVisualStyleBackColor = true;
			// 
			// LibraryRefreshButton
			// 
			this.LibraryRefreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.LibraryRefreshButton.Location = new System.Drawing.Point(823, 4);
			this.LibraryRefreshButton.Name = "LibraryRefreshButton";
			this.LibraryRefreshButton.Size = new System.Drawing.Size(75, 23);
			this.LibraryRefreshButton.TabIndex = 6;
			this.LibraryRefreshButton.Text = "Refresh";
			this.LibraryRefreshButton.UseVisualStyleBackColor = true;
			this.LibraryRefreshButton.Click += new System.EventHandler(this.LibraryRefreshButton_Click);
			// 
			// LibraryListView
			// 
			this.LibraryListView.AllowDrop = true;
			this.LibraryListView.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.LibraryListView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.LibraryListView.Location = new System.Drawing.Point(264, 33);
			this.LibraryListView.Name = "LibraryListView";
			this.LibraryListView.Size = new System.Drawing.Size(640, 325);
			this.LibraryListView.TabIndex = 0;
			// 
			// LibraryTreeView
			// 
			this.LibraryTreeView.AllowDrop = true;
			this.LibraryTreeView.ArchivedNodeForeColor = System.Drawing.SystemColors.WindowText;
			this.LibraryTreeView.Dock = System.Windows.Forms.DockStyle.Left;
			this.LibraryTreeView.ExclusionList = "";
			this.LibraryTreeView.ExpandAssets = true;
			this.LibraryTreeView.ExpandPackageGroupAssets = true;
			this.LibraryTreeView.ExpandPackageGroups = false;
			this.LibraryTreeView.FocusForAssetNodes = MOG_ControlsLibrary.Controls.LeafFocusLevel.RepositoryItems;
			this.LibraryTreeView.FullRowSelect = true;
			this.LibraryTreeView.HideSelection = false;
			this.LibraryTreeView.HotTracking = true;
			this.LibraryTreeView.ImageIndex = 0;
			this.LibraryTreeView.Location = new System.Drawing.Point(0, 33);
			this.LibraryTreeView.Name = "LibraryTreeView";
			this.LibraryTreeView.NodesDefaultToChecked = false;
			this.LibraryTreeView.PathSeparator = "~";
			this.LibraryTreeView.PersistantHighlightSelectedNode = false;
			this.LibraryTreeView.SelectedImageIndex = 0;
			this.LibraryTreeView.ShowAssets = false;
			this.LibraryTreeView.Size = new System.Drawing.Size(256, 325);
			this.LibraryTreeView.TabIndex = 0;
			this.LibraryTreeView.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.LibraryTreeView_BeforeSelect);
			this.LibraryTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.LibraryTreeView_AfterSelect);
			// 
			// MogControl_LibraryExplorer
			// 
			this.Controls.Add(this.LibraryListView);
			this.Controls.Add(this.splitter);
			this.Controls.Add(this.LibraryTreeView);
			this.Controls.Add(this.LibraryTopPanel);
			this.Controls.Add(this.LibraryTargetPanel);
			this.Name = "MogControl_LibraryExplorer";
			this.Size = new System.Drawing.Size(904, 408);
			this.LibraryTargetPanel.ResumeLayout(false);
			this.LibraryTargetPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.LibraryWorkingPictureBox)).EndInit();
			this.LibraryTopPanel.ResumeLayout(false);
			this.LibraryTopPanel.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		public bool AutoImport
		{
			get { return Library_AutoImport_CheckBox.Checked; }
			set { Library_AutoImport_CheckBox.Checked = value; }
		}

		private void LibraryTreeView_BeforeSelect(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		{
			LibraryListView.LibraryListView.Items.Clear();
			LibraryListView.LibraryListView.Items.Add("Retrieving Information...");
			LibraryListView.LibraryListView.Refresh();
		}

		private void LibraryTreeView_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			base.Refresh();

			// We do not allow any items to be shown in classifications that are not in the library folder
			if (e.Node.FullPath.ToLower().IndexOf("library") != -1)
			{
				LibraryListView.Populate(e.Node.FullPath);
			}
			else
			{
				LibraryListView.LibraryListView.Items.Clear();
			}

			LibraryTargetTextBox.Text = "(" + MOG_ControllerLibrary.GetWorkingDirectory() + ")" + "\\" + e.Node.FullPath.Replace("~", "\\");
		}

		override public void Refresh()
		{
			// We only need to call the ListView's Deinitialize becasue the TreeView will eventually cause it to be repopulated 
			LibraryListView.DeInitialize();
			// Refresh the tree view
			LibraryTreeView.DeInitialize();
			LibraryTreeView.Initialize();
			
			// Show our sync target
			this.LibraryTargetTextBox.Text = "(" + MOG_ControllerLibrary.GetWorkingDirectory() + ")";
			this.LibraryTargetTextBox.Tag = MOG_ControllerLibrary.GetWorkingDirectory();

			base.Refresh();
		}

		public void RefreshItem(MOG_Command command)
		{
// JohnRen - Untested and need to finish before enabling
//			LibraryTreeView.RefreshItem(command);
			LibraryListView.RefreshItem(command);
		}

		private void LibraryBrowseButton_Click(object sender, System.EventArgs e)
		{
			FolderBrowserDialog dialog = new FolderBrowserDialog();

			dialog.RootFolder = Environment.SpecialFolder.Desktop;
			dialog.ShowNewFolderButton = true;
			dialog.SelectedPath = MOG_ControllerLibrary.GetWorkingDirectory();
			dialog.Description = "Select a location to be the Library target.";
			
			DialogResult result = dialog.ShowDialog();
			if (result == DialogResult.OK)
			{
				MOG_ControllerLibrary.SetWorkingDirectory(dialog.SelectedPath);
				LibraryTargetTextBox.Text = "(" + MOG_ControllerLibrary.GetWorkingDirectory() + ")";
				LibraryTargetTextBox.Tag = MOG_ControllerLibrary.GetWorkingDirectory();

				MogUtils_Settings.MogUtils_Settings.SaveSetting("Library", "TargetDirectory", dialog.SelectedPath);
			}
		}

		public bool IsAutoImportChecked()
		{
			return Library_AutoImport_CheckBox.Checked;
		}

		private void LibraryExploreButton_Click(object sender, EventArgs e)
		{
			if (MOG_ControllerLibrary.GetWorkingDirectory() != "")
			{
				guiCommandLine.ShellSpawn(MOG_ControllerLibrary.GetWorkingDirectory());
			}
		}

       	private void LibraryRefreshButton_Click(object sender, EventArgs e)
		{
			MainMenuViewClass.MOGGlobalViewRefresh(MogMainForm.MainApp);
		}
	}

	#region File importation routines for use by the MOG Client Library tab
	/// <summary>
	/// File importation routines for use by the MOG Client Library tab
	/// </summary>
	public class LibraryFileImporter
	{
		public static bool PromptUserForConfirmation(string[] filenames, string classification)
		{
			bool bProceed = true;

			// Make sure we have valid information?
			if (filenames == null || filenames.Length == 0)
			{
				MOG_Prompt.PromptMessage("Add Files", "Unable to add files because no files were specified.");
				bProceed = false;
			}
			else if (string.IsNullOrEmpty(classification))
			{
				MOG_Prompt.PromptMessage("Add Files", "Unable to add files because no classification was specified.");
				bProceed = false;
			}
			else
			{
				// Check if this file is already located within the library
				if (MOG_ControllerLibrary.IsPathWithinLibrary(Path.GetDirectoryName(filenames[0])))
				{
					// Assume all the files have the same path and construct a classification from the path of the first file
					string libraryClassification = MOG_ControllerLibrary.ConstructLibraryClassificationFromPath(Path.GetDirectoryName(filenames[0]));
					if (libraryClassification != classification)
					{
						// Prompt the user about what they want
						string message = "This file is located elsewhere within the 'Library Working Folder'.\n\n" +
										 "Are you sure you want to add this file to this new location as well?";
						if (filenames.Length > 2)
						{
							message = "These files are located elsewhere within the 'Library Working Folder'.\n\n" +
									  "Are you sure you want to add these files to this new location as well?";
						}
						if (MOG_Prompt.PromptResponse("Add Files", message, MOGPromptButtons.YesNo) == MOGPromptResult.No)
						{
							// Early out because they clicked cancel
							bProceed = false;
						}
					}
				}
				else
				{
					// Ask user for confirmation that they really want
					string msg = "";
					if (filenames.Length < 20)
					{
						msg += "Are you sure you want to import the following?\n\n";

						// Build a file/dir list for the confirmation message
						foreach (string filename in filenames)
						{
							msg += Path.GetFileName(filename) + "\n";
						}
					}
					else
					{
						// There are too many files to display in a MessageBox, so just ask for generic confirmation
						msg = "Are you sure you want to import these " + filenames.Length.ToString() + " items?";
					}

					if (MOG_Prompt.PromptResponse("Confirm File Importation", msg, MOGPromptButtons.YesNo) == MOGPromptResult.No)
					{
						bProceed = false;
					}
				}
			}

			return bProceed;
		}

		// Import a list of files and/or directories from a drag-drop operation to a specific classification
		public static void ImportFiles(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			List<object> args = e.Argument as List<object>;
			string[] filenames = args[0] as string[];
			string classification = args[1] as string;
			
			if (filenames != null  &&  filenames.Length > 0)
			{
				try
				{
					// Sort files and dirs
					ArrayList fileList = new ArrayList();
					ArrayList dirList = new ArrayList();
					foreach (string filename in filenames)
					{
						if (Directory.Exists(filename))
						{
							dirList.Add(filename);
						}
						else if (File.Exists(filename))
						{
							fileList.Add(filename);
						}
					}

					// Add each directory seperately
					foreach (string dirname in dirList)
					{
						MOG_ControllerLibrary.AddDirectory(dirname, classification);
					}

					// Add all the files
					MOG_ControllerLibrary.AddFiles(fileList, classification);
				}
				catch
				{
				}
			}
		}

		// Copy a list of files and/or directories from a drag-drop operation to the user's local working directory
		public static void CopyFiles(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			List<object> args = e.Argument as List<object>;
			string[] filenames = args[0] as string[];
			string classification = args[1] as string;

			if (filenames != null  &&  filenames.Length > 0)
			{
				try
				{
					// Sort files and dirs
					ArrayList fileList = new ArrayList();
					ArrayList dirList = new ArrayList();
					foreach (string filename in filenames)
					{
						if (Directory.Exists(filename))
						{
							dirList.Add(filename);
						}
						else if (File.Exists(filename))
						{
							fileList.Add(filename);
						}
					}

					string targetPath = MOG_ControllerLibrary.ConstructPathFromLibraryClassification(classification);

					// Simply copy the files
					foreach (string dirname in dirList)
					{
						string target = Path.Combine(targetPath, Path.GetFileName(dirname));
						MOG_ControllerSystem.DirectoryCopyEx(dirname, target, worker);
					}
					// Simply copy the files
					foreach (string filename in fileList)
					{
						string target = Path.Combine(targetPath, Path.GetFileName(filename));
						MOG_ControllerSystem.FileCopyEx(filename, target, true, true, worker);
					}
				}
				catch
				{
				}
			}
		}
}
	#endregion

}
