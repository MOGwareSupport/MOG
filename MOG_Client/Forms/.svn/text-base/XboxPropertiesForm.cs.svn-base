using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using MOG_Client.Client_Utilities;

using MOG;
using MOG.INI;
using MOG.DOSUTILS;
using MOG.REPORT;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.PROMPT;
using MOG_ControlsLibrary.MogUtils_Settings;



namespace MOG_Client.Forms
{
	/// <summary>
	/// Summary description for XboxPropertiesForm.
	/// </summary>
	public class XboxPropertiesForm : System.Windows.Forms.Form
	{
		private MogMainForm mainForm;
		private string mUserName;
		public string mProjectMap;
		public string mUserMap;
		private System.Windows.Forms.TreeView XboxSincTreeView;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button XboxOkButton;
		private System.Windows.Forms.Button XboxCancelButton;
		private System.Windows.Forms.ContextMenu XboxTreeContextMenu;
		private System.Windows.Forms.MenuItem XboxTreeAddMenuItem;
		private System.Windows.Forms.MenuItem XboxRemoveMenuItem;
		private System.Windows.Forms.Button SyncSaveButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.SaveFileDialog SyncSaveFileDialog;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox SyncUserMapComboBox;
		private System.Windows.Forms.ComboBox SyncProjectMapComboBox;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public XboxPropertiesForm(MogMainForm main, string projectMap, string userMap)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			

			mainForm = main;
			//mSourcePath = "";
			mUserName = MOG_ControllerProject.GetUser().GetUserName();

			mProjectMap = projectMap;
			mUserMap = userMap;
			
			LoadUserSyncFiles();
			LoadPlatformSinc();			
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(XboxPropertiesForm));
			this.XboxSincTreeView = new System.Windows.Forms.TreeView();
			this.XboxTreeContextMenu = new System.Windows.Forms.ContextMenu();
			this.XboxTreeAddMenuItem = new System.Windows.Forms.MenuItem();
			this.XboxRemoveMenuItem = new System.Windows.Forms.MenuItem();
			this.panel1 = new System.Windows.Forms.Panel();
			this.XboxCancelButton = new System.Windows.Forms.Button();
			this.XboxOkButton = new System.Windows.Forms.Button();
			this.SyncSaveButton = new System.Windows.Forms.Button();
			this.SyncUserMapComboBox = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.SyncSaveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.SyncProjectMapComboBox = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// XboxSincTreeView
			// 
			this.XboxSincTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.XboxSincTreeView.CheckBoxes = true;
			this.XboxSincTreeView.ContextMenu = this.XboxTreeContextMenu;
			this.XboxSincTreeView.ImageIndex = -1;
			this.XboxSincTreeView.Location = new System.Drawing.Point(0, 112);
			this.XboxSincTreeView.Name = "XboxSincTreeView";
			this.XboxSincTreeView.SelectedImageIndex = -1;
			this.XboxSincTreeView.Size = new System.Drawing.Size(292, 336);
			this.XboxSincTreeView.TabIndex = 0;
			this.XboxSincTreeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.XboxSincTreeView_AfterCheck);
			// 
			// XboxTreeContextMenu
			// 
			this.XboxTreeContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																								this.XboxTreeAddMenuItem,
																								this.XboxRemoveMenuItem});
			// 
			// XboxTreeAddMenuItem
			// 
			this.XboxTreeAddMenuItem.Index = 0;
			this.XboxTreeAddMenuItem.Text = "Add";
			this.XboxTreeAddMenuItem.Click += new System.EventHandler(this.XboxTreeAddMenuItem_Click);
			// 
			// XboxRemoveMenuItem
			// 
			this.XboxRemoveMenuItem.Index = 1;
			this.XboxRemoveMenuItem.Text = "Remove";
			this.XboxRemoveMenuItem.Click += new System.EventHandler(this.XboxRemoveMenuItem_Click);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.XboxCancelButton);
			this.panel1.Controls.Add(this.XboxOkButton);
			this.panel1.Controls.Add(this.SyncSaveButton);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 453);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(296, 40);
			this.panel1.TabIndex = 1;
			// 
			// XboxCancelButton
			// 
			this.XboxCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.XboxCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.XboxCancelButton.Location = new System.Drawing.Point(128, 8);
			this.XboxCancelButton.Name = "XboxCancelButton";
			this.XboxCancelButton.TabIndex = 1;
			this.XboxCancelButton.Text = "Cancel";
			// 
			// XboxOkButton
			// 
			this.XboxOkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.XboxOkButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.XboxOkButton.Location = new System.Drawing.Point(208, 8);
			this.XboxOkButton.Name = "XboxOkButton";
			this.XboxOkButton.TabIndex = 0;
			this.XboxOkButton.Text = "OK";
			// 
			// SyncSaveButton
			// 
			this.SyncSaveButton.Enabled = false;
			this.SyncSaveButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.SyncSaveButton.Location = new System.Drawing.Point(48, 8);
			this.SyncSaveButton.Name = "SyncSaveButton";
			this.SyncSaveButton.TabIndex = 4;
			this.SyncSaveButton.Text = "Save";
			this.SyncSaveButton.Click += new System.EventHandler(this.SyncSaveButton_Click);
			// 
			// SyncUserMapComboBox
			// 
			this.SyncUserMapComboBox.Location = new System.Drawing.Point(8, 80);
			this.SyncUserMapComboBox.Name = "SyncUserMapComboBox";
			this.SyncUserMapComboBox.Size = new System.Drawing.Size(264, 21);
			this.SyncUserMapComboBox.Sorted = true;
			this.SyncUserMapComboBox.TabIndex = 2;
			this.SyncUserMapComboBox.Text = "None";
			this.SyncUserMapComboBox.SelectedIndexChanged += new System.EventHandler(this.SyncFileComboBox_SelectionChangeCommitted);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 64);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(192, 16);
			this.label1.TabIndex = 6;
			this.label1.Text = "User GetLatest file map";
			// 
			// SyncSaveFileDialog
			// 
			this.SyncSaveFileDialog.DefaultExt = "sync";
			this.SyncSaveFileDialog.Filter = "GetLatest Files|*.sync";
			// 
			// SyncProjectMapComboBox
			// 
			this.SyncProjectMapComboBox.Location = new System.Drawing.Point(8, 32);
			this.SyncProjectMapComboBox.Name = "SyncProjectMapComboBox";
			this.SyncProjectMapComboBox.Size = new System.Drawing.Size(264, 21);
			this.SyncProjectMapComboBox.TabIndex = 7;
			this.SyncProjectMapComboBox.Text = "None";
			this.SyncProjectMapComboBox.SelectedIndexChanged += new System.EventHandler(this.SyncFileComboBox_SelectionChangeCommitted);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(208, 16);
			this.label2.TabIndex = 8;
			this.label2.Text = "Project GetLatest file map";
			// 
			// XboxPropertiesForm
			// 
			this.AcceptButton = this.XboxOkButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.XboxCancelButton;
			this.ClientSize = new System.Drawing.Size(296, 493);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.SyncProjectMapComboBox);
			this.Controls.Add(this.XboxSincTreeView);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.SyncUserMapComboBox);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.Name = "XboxPropertiesForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "XboxPropertiesForm";
			this.Closed += new System.EventHandler(this.XboxPropertiesForm_Closed);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private string FormatString(string format)
		{
			// Replace out any string options {}
//			if (format.IndexOf("{projectRoot}") != -1)
//			{
//				format = format.Replace("{projectRoot}", mSourcePath);
//			}

			// Replace out any string options {}
			if (format.IndexOf("{projectName}") != -1)
			{
				format = format.Replace("{projectName}", MOG_ControllerProject.GetProjectName());
			}

			// Replace out any string options {}
			if (format.IndexOf("{LoginUserName}") != -1)
			{
				format = format.Replace("{LoginUserName}", mUserName);
			}

			return format;
		}

		private void LoadUserSyncFiles()
		{
			#region Initialize the project filemaps
			// Get any additional maps
			SyncProjectMapComboBox.Items.Clear();
			FileInfo []files = DosUtils.FileGetList(MOG_ControllerProject.GetProject().GetProjectToolsPath(), "*.sync");
			if(files != null)
			{
				// Add the default 'none'
				SyncProjectMapComboBox.Items.Add("None");

				foreach (FileInfo file in files)
				{
					SyncProjectMapComboBox.Items.Add(file.Name);
				}

				if (files.Length > 0)
				{
					SyncProjectMapComboBox.SelectedIndex = 0;
				}
			}

			// Set the project map
			if (mProjectMap.Length != 0)
			{
				SyncProjectMapComboBox.Text = mProjectMap;
			}
			else
			{
				mProjectMap = "None";
				SyncProjectMapComboBox.Text = mProjectMap;
			}
			#endregion

			#region Initialize the user filemaps
			// Get any aditional sync files
			SyncUserMapComboBox.Items.Clear();
			files = DosUtils.FileGetList(MOG_ControllerProject.GetUser().GetUserToolsPath(), "*.sync");
			if(files != null)
			{
				// Add the default user map
				SyncUserMapComboBox.Items.Add("None");

				foreach (FileInfo file in files)
				{
					SyncUserMapComboBox.Items.Add(file.Name);
				}

				if (files.Length > 0)
				{
					SyncUserMapComboBox.SelectedIndex = 0;
				}
			}

			// Set the user map
			if (mUserMap.Length != 0)
			{
				SyncUserMapComboBox.Text = mUserMap;
			}
			else
			{
				mUserMap = "None";
				SyncUserMapComboBox.Text = mUserMap;
			}
			#endregion
		}

		private void LoadPlatformSinc()
		{

			string userSincFilename = MOG_ControllerProject.GetUser().GetUserToolsPath() + "\\" + SyncUserMapComboBox.Text;
			string platformSincFilename = MOG_ControllerProject.GetProject().GetProjectToolsPath() + "\\" + SyncProjectMapComboBox.Text;
			
			// Open the project sync file
			MOG_Ini ProjectPlatfromSinc = null;
			if (DosUtils.FileExist(platformSincFilename))
			{
				ProjectPlatfromSinc = new MOG_Ini(platformSincFilename);
			}
			else
			{
				ProjectPlatfromSinc =  null;
			}
			
			// Open user sync file
			MOG_Ini userPlatfromSinc = null;
			if (DosUtils.FileExist(userSincFilename))
			{
				userPlatfromSinc = new MOG_Ini(userSincFilename);
			}
			else
			{
				userPlatfromSinc =  null;
			}

			// Populate the tree
			PopulateSyncTree(ProjectPlatfromSinc, userPlatfromSinc);
			

			// Close the ini's
			if (ProjectPlatfromSinc != null)
			{
				ProjectPlatfromSinc.CloseNoSave();
			}

			if (userPlatfromSinc != null)
			{
				userPlatfromSinc.CloseNoSave();
			}
		}

		private void PopulateSyncTree(MOG_Ini ProjectPlatfromSinc, MOG_Ini userPlatfromSinc)
		{
			// Clear our list
			XboxSincTreeView.Nodes.Clear();

			#region Project platform defaults
			// Load the project platform defaults
			TreeNode parentPlatform = new TreeNode(MOG_ControllerProject.GetCurrentSyncDataController().GetPlatformName());				
			if ((ProjectPlatfromSinc != null) && (ProjectPlatfromSinc.SectionExist("xbox")))
			{
				for (int x = 0; x < ProjectPlatfromSinc.CountKeys("Xbox"); x++)
				{
					TreeNode node = new TreeNode(FormatString(ProjectPlatfromSinc.GetKeyNameByIndexSLOW("Xbox", x)));
					TreeNode child = new TreeNode(FormatString(ProjectPlatfromSinc.GetKeyByIndexSLOW("Xbox", x)));
					node.Checked = true;
					node.Nodes.Add(child);

					parentPlatform.Nodes.Add(node);					
				}
			}

			// Add user nodes
			if (userPlatfromSinc != null)
			{
				if (userPlatfromSinc.SectionExist("xbox"))
				{
					for (int x = 0; x < userPlatfromSinc.CountKeys("Xbox"); x++)
					{
						TreeNode node = new TreeNode(FormatString(userPlatfromSinc.GetKeyNameByIndexSLOW("Xbox", x)));
						TreeNode child = new TreeNode(FormatString(userPlatfromSinc.GetKeyByIndexSLOW("Xbox", x)));
						node.Checked = true;
						node.ForeColor = Color.Blue;
						node.Nodes.Add(child);

						parentPlatform.Nodes.Add(node);					
					}
				}
			}
			XboxSincTreeView.Nodes.Add(parentPlatform);
			#endregion

			#region Filemaps
			// Load project Filemaps
			TreeNode parentFileMaps = new TreeNode("FileMap");				
			if ((ProjectPlatfromSinc != null) && (ProjectPlatfromSinc.SectionExist("FileMap")))
			{				
				for (int x = 0; x < ProjectPlatfromSinc.CountKeys("FileMap"); x++)
				{
					TreeNode node = new TreeNode(FormatString(ProjectPlatfromSinc.GetKeyNameByIndexSLOW("FileMap", x)));
					TreeNode child = new TreeNode(FormatString(ProjectPlatfromSinc.GetKeyByIndexSLOW("FileMap", x)));
					node.Checked = true;
					node.Nodes.Add(child);

					parentFileMaps.Nodes.Add(node);
				}
			}

			// Add user nodes
			if (userPlatfromSinc != null)
			{
				if (userPlatfromSinc.SectionExist("FileMap"))
				{
					for (int x = 0; x < userPlatfromSinc.CountKeys("FileMap"); x++)
					{
						TreeNode node = new TreeNode(FormatString(userPlatfromSinc.GetKeyNameByIndexSLOW("FileMap", x)));
						node.ForeColor = Color.Blue;
						node.Checked = true;

						TreeNode child = new TreeNode(FormatString(userPlatfromSinc.GetKeyByIndexSLOW("FileMap", x)));
						node.ForeColor = Color.Blue;
						node.Nodes.Add(child);

						parentFileMaps.Nodes.Add(node);
					}
				}
			}

			XboxSincTreeView.Nodes.Add(parentFileMaps);
			#endregion

			#region Remaps
			// Load Remaps
			TreeNode parentRemaps = new TreeNode("ReMap");				
			if ((ProjectPlatfromSinc != null) && (ProjectPlatfromSinc.SectionExist("ReMap")))
			{				
				for (int x = 0; x < ProjectPlatfromSinc.CountKeys("ReMap"); x++)
				{
					TreeNode node = new TreeNode(FormatString(ProjectPlatfromSinc.GetKeyNameByIndexSLOW("ReMap", x)));
					node.Checked = true;
					node.Nodes.Add(FormatString(ProjectPlatfromSinc.GetKeyByIndexSLOW("ReMap", x)));

					parentRemaps.Nodes.Add(node);
				}
			}

			if ((userPlatfromSinc != null) && (userPlatfromSinc.SectionExist("ReMap")))
			{				
				for (int x = 0; x < userPlatfromSinc.CountKeys("ReMap"); x++)
				{
					TreeNode node = new TreeNode(FormatString(userPlatfromSinc.GetKeyNameByIndexSLOW("ReMap", x)));
					node.Checked = true;
					node.ForeColor = Color.Blue;
					node.Nodes.Add(FormatString(userPlatfromSinc.GetKeyByIndexSLOW("ReMap", x)));

					parentRemaps.Nodes.Add(node);
				}
			}

			XboxSincTreeView.Nodes.Add(parentRemaps);
			#endregion

			#region Exclusions
			// Load Exclusions
			TreeNode parentExclusions = new TreeNode("Exclusion");
				
			if ((ProjectPlatfromSinc != null) && (ProjectPlatfromSinc.SectionExist("Exclusion")))
			{				
				for (int x = 0; x < ProjectPlatfromSinc.CountKeys("Exclusion"); x++)
				{
					TreeNode node = new TreeNode(FormatString(ProjectPlatfromSinc.GetKeyNameByIndexSLOW("Exclusion", x)));
					node.Checked = true;
					node.Nodes.Add(FormatString(ProjectPlatfromSinc.GetKeyByIndexSLOW("Exclusion", x)));

					parentExclusions.Nodes.Add(node);
				}							
			}

			// Add user nodes
			if (userPlatfromSinc != null)
			{
				if (userPlatfromSinc.SectionExist("Exclusion"))
				{
					for (int x = 0; x < userPlatfromSinc.CountKeys("Exclusion"); x++)
					{
						TreeNode node = new TreeNode(FormatString(userPlatfromSinc.GetKeyNameByIndexSLOW("Exclusion", x)));
						node.ForeColor = Color.Blue;
						node.Checked = true;

						TreeNode child = new TreeNode(FormatString(userPlatfromSinc.GetKeyByIndexSLOW("Exclusion", x)));
						node.ForeColor = Color.Blue;
						node.Nodes.Add(child);

						parentExclusions.Nodes.Add(node);
					}
				}
			}	

			XboxSincTreeView.Nodes.Add(parentExclusions);
			#endregion
		}

		private void XboxTreeAddMenuItem_Click(object sender, System.EventArgs e)
		{
			// Setup the FileDialog box
			mainForm.MOGOpenFileDialog.Reset();
			mainForm.MOGOpenFileDialog.DefaultExt = ".adv";
			mainForm.MOGOpenFileDialog.Title = "Select file(s) to exclude";

			// Set the folderBrowser to the current target path
			string start = MOG_ControllerProject.GetCurrentSyncDataController().GetSyncDirectory();
			if (start.Length != 0)
			{				
				mainForm.MOGOpenFileDialog.InitialDirectory = start;
			}

			// show the window
			if (mainForm.MOGOpenFileDialog.ShowDialog(mainForm) == DialogResult.OK)
			{
				string newExclusion = mainForm.MOGOpenFileDialog.FileName.ToLower();

                TreeNode node = new TreeNode(newExclusion);
				node.ForeColor = Color.Red;

				XboxSincTreeView.Nodes.Add(node);
			}			
		}

		private void XboxRemoveMenuItem_Click(object sender, System.EventArgs e)
		{
		
		}

		private void SyncFileComboBox_SelectionChangeCommitted(object sender, System.EventArgs e)
		{
			// Re-populate the Tree
			LoadPlatformSinc();
		}

		private void SyncSaveButton_Click(object sender, System.EventArgs e)
		{
			SyncSaveFileDialog.InitialDirectory = MOG_ControllerProject.GetUser().GetUserToolsPath();
            
			// Save this custom sync file
			if (SyncSaveFileDialog.ShowDialog() == DialogResult.OK)
			{
				string syncFilename = SyncSaveFileDialog.FileName;
				MOG_Ini syncFile = new MOG_Ini(syncFilename);

				foreach (TreeNode sectionNode in XboxSincTreeView.Nodes)
				{
					string section = sectionNode.Text;

					foreach (TreeNode keyNode in sectionNode.Nodes)
					{
						string key = keyNode.Text;

						if (keyNode.Nodes.Count > 0)
						{
							foreach (TreeNode valNode in keyNode.Nodes)
							{
								string val = valNode.Text;

								syncFile.PutString(section, key, val);
							}
						}
						else
						{
							syncFile.PutSectionString(section, key);
						}
					}					
				}

				// Verify that the newly created sync file has the correct number amount of sections
				if (syncFile.SectionExist("Filemap"))
				{
					if (syncFile.CountKeys("Filemap") > 0 && string.Compare(SyncProjectMapComboBox.Text, "None") == 0)
					{
						MOG_Prompt.PromptMessage("Missing syncfile data", "The required 'FILEMAP' section was not found in this custom sync file.  Aborting...");
						syncFile.CloseNoSave();
						return;
					}					
				}
				else
				{
					MOG_Prompt.PromptMessage("Missing syncfile data", "The required 'FILEMAP' section was not found in this custom sync file.  Aborting...");
					syncFile.CloseNoSave();
					return;
				}

				// Make sure we have a valid root definition
				if (syncFile.SectionExist(MOG_ControllerProject.GetCurrentSyncDataController().GetPlatformName()))
				{
					if (syncFile.CountKeys(MOG_ControllerProject.GetCurrentSyncDataController().GetPlatformName()) > 0 && string.Compare(SyncProjectMapComboBox.Text, "None") == 0)
					{
						MOG_Prompt.PromptMessage("Missing syncfile data", "The required '" + MOG_ControllerProject.GetCurrentSyncDataController().GetPlatformName() + "' section was not found in this custom sync file.  Aborting...");
						syncFile.CloseNoSave();
						return;
					}
				}
				else
				{
					MOG_Prompt.PromptMessage("Missing syncfile data", "The required '" + MOG_ControllerProject.GetCurrentSyncDataController().GetPlatformName() + "' section was not found in this custom sync file.  Aborting...");
					syncFile.CloseNoSave();
					return;
				}

				// Save out our new syncFile
				syncFile.Save();
				SyncSaveButton.Enabled = false;

				mUserMap = syncFile.GetFilename();
			}			
		}

		private void XboxSincTreeView_AfterCheck(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			SyncSaveButton.Enabled = true;
		}

		private void XboxPropertiesForm_Closed(object sender, System.EventArgs e)
		{
			string prefsString = MOG_ControllerProject.GetProjectName() + "." + MOG_ControllerProject.GetCurrentSyncDataController().GetPlatformName();

			mUserMap = SyncUserMapComboBox.Text;
			mProjectMap = SyncProjectMapComboBox.Text;

			// Set the full filename of the selected user syncfile
			MogUtils_Settings.SaveSetting(prefsString, "UserSyncFile", mUserMap);
			MogUtils_Settings.SaveSetting(prefsString, "ProjectSyncFile", mProjectMap);
		}
	}
}
