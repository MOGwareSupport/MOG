using System;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using MOG;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.INI;
using MOG.PROMPT;
using MOG.DATABASE;
using MOG.DOSUTILS;
using MOG.PROGRESS;

using MOG_ServerManager.Utilities;
using MOG_CoreControls;
using System.Collections.Generic;

namespace MOG_ServerManager
{
	/// <summary>
	/// Summary description for MOGRemovedProjectsManager.
	/// </summary>
	public class MOGRemovedProjectsManager : System.Windows.Forms.UserControl
	{
		#region System definitions
		private System.Windows.Forms.ListView lvRemovedProjects;
		private System.Windows.Forms.Label lblRemovedProjects;
		private System.Windows.Forms.Button btnPurge;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.Button btnRestore;
		private System.Windows.Forms.GroupBox gbInfo;
		private System.Windows.Forms.Label lblPathLabel;
		private System.Windows.Forms.Label lblPath;
		private System.Windows.Forms.Button btnPurgeAll;
		private System.Windows.Forms.Button btnRestoreAll;
		private System.Windows.Forms.ContextMenuStrip cmProjects;
		private System.Windows.Forms.ToolStripMenuItem miPurge;
		private System.Windows.Forms.ToolStripMenuItem miPurgeAll;
		private System.Windows.Forms.ToolStripSeparator menuItem2;
		private System.Windows.Forms.ToolStripMenuItem miRestore;
		private System.Windows.Forms.ToolStripMenuItem miRestoreAll;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.lvRemovedProjects = new System.Windows.Forms.ListView();
			this.cmProjects = new System.Windows.Forms.ContextMenuStrip();
			this.miPurge = new System.Windows.Forms.ToolStripMenuItem();
			this.miPurgeAll = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItem2 = new System.Windows.Forms.ToolStripSeparator();
			this.miRestore = new System.Windows.Forms.ToolStripMenuItem();
			this.miRestoreAll = new System.Windows.Forms.ToolStripMenuItem();
			this.lblRemovedProjects = new System.Windows.Forms.Label();
			this.btnPurge = new System.Windows.Forms.Button();
			this.btnClose = new System.Windows.Forms.Button();
			this.btnRestore = new System.Windows.Forms.Button();
			this.gbInfo = new System.Windows.Forms.GroupBox();
			this.lblPath = new System.Windows.Forms.Label();
			this.lblPathLabel = new System.Windows.Forms.Label();
			this.btnPurgeAll = new System.Windows.Forms.Button();
			this.btnRestoreAll = new System.Windows.Forms.Button();
			this.gbInfo.SuspendLayout();
			this.SuspendLayout();
			// 
			// lvRemovedProjects
			// 
			this.lvRemovedProjects.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left)));
			this.lvRemovedProjects.ContextMenuStrip = this.cmProjects;
			this.lvRemovedProjects.Location = new System.Drawing.Point(8, 24);
			this.lvRemovedProjects.Name = "lvRemovedProjects";
			this.lvRemovedProjects.Size = new System.Drawing.Size(152, 184);
			this.lvRemovedProjects.TabIndex = 0;
			this.lvRemovedProjects.View = System.Windows.Forms.View.List;
			this.lvRemovedProjects.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MOGRemovedProjectsManager_KeyDown);
			this.lvRemovedProjects.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lvRemovedProjects_MouseUp);
			// 
			// cmProjects
			// 
			this.cmProjects.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
																					   this.miPurge,
																					   this.miPurgeAll,
																					   this.menuItem2,
																					   this.miRestore,
																					   this.miRestoreAll});
			// 
			// miPurge
			// 
			this.miPurge.Text = "Purge";
			this.miPurge.Click += new System.EventHandler(this.miPurge_Click);
			// 
			// miPurgeAll
			// 
			this.miPurgeAll.Text = "Purge All";
			this.miPurgeAll.Click += new System.EventHandler(this.miPurgeAll_Click);
			// 
			// miRestore
			// 
			this.miRestore.Text = "Restore";
			this.miRestore.Click += new System.EventHandler(this.miRestore_Click);
			// 
			// miRestoreAll
			// 
			this.miRestoreAll.Text = "Restore All";
			this.miRestoreAll.Click += new System.EventHandler(this.miRestoreAll_Click);
			// 
			// lblRemovedProjects
			// 
			this.lblRemovedProjects.Location = new System.Drawing.Point(8, 8);
			this.lblRemovedProjects.Name = "lblRemovedProjects";
			this.lblRemovedProjects.Size = new System.Drawing.Size(152, 16);
			this.lblRemovedProjects.TabIndex = 1;
			this.lblRemovedProjects.Text = "Removed Projects";
			// 
			// btnPurge
			// 
			this.btnPurge.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnPurge.Location = new System.Drawing.Point(176, 56);
			this.btnPurge.Name = "btnPurge";
			this.btnPurge.TabIndex = 2;
			this.btnPurge.Text = "Purge";
			this.btnPurge.Click += new System.EventHandler(this.btnPurge_Click);
			this.btnPurge.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MOGRemovedProjectsManager_KeyDown);
			// 
			// btnClose
			// 
			this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnClose.Location = new System.Drawing.Point(176, 184);
			this.btnClose.Name = "btnClose";
			this.btnClose.TabIndex = 3;
			this.btnClose.Text = "Close";
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			this.btnClose.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MOGRemovedProjectsManager_KeyDown);
			// 
			// btnRestore
			// 
			this.btnRestore.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnRestore.Location = new System.Drawing.Point(176, 128);
			this.btnRestore.Name = "btnRestore";
			this.btnRestore.TabIndex = 4;
			this.btnRestore.Text = "Restore";
			this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
			this.btnRestore.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MOGRemovedProjectsManager_KeyDown);
			// 
			// gbInfo
			// 
			this.gbInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.gbInfo.Controls.Add(this.lblPath);
			this.gbInfo.Controls.Add(this.lblPathLabel);
			this.gbInfo.Location = new System.Drawing.Point(272, 24);
			this.gbInfo.Name = "gbInfo";
			this.gbInfo.Size = new System.Drawing.Size(256, 184);
			this.gbInfo.TabIndex = 0;
			this.gbInfo.TabStop = false;
			this.gbInfo.Text = "Info";
			// 
			// lblPath
			// 
			this.lblPath.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lblPath.Location = new System.Drawing.Point(56, 32);
			this.lblPath.Name = "lblPath";
			this.lblPath.Size = new System.Drawing.Size(192, 136);
			this.lblPath.TabIndex = 1;
			// 
			// lblPathLabel
			// 
			this.lblPathLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblPathLabel.Location = new System.Drawing.Point(16, 32);
			this.lblPathLabel.Name = "lblPathLabel";
			this.lblPathLabel.Size = new System.Drawing.Size(32, 16);
			this.lblPathLabel.TabIndex = 0;
			this.lblPathLabel.Text = "Path:";
			// 
			// btnPurgeAll
			// 
			this.btnPurgeAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnPurgeAll.Location = new System.Drawing.Point(176, 32);
			this.btnPurgeAll.Name = "btnPurgeAll";
			this.btnPurgeAll.TabIndex = 5;
			this.btnPurgeAll.Text = "Purge All";
			this.btnPurgeAll.Click += new System.EventHandler(this.btnPurgeAll_Click);
			// 
			// btnRestoreAll
			// 
			this.btnRestoreAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnRestoreAll.Location = new System.Drawing.Point(176, 104);
			this.btnRestoreAll.Name = "btnRestoreAll";
			this.btnRestoreAll.TabIndex = 6;
			this.btnRestoreAll.Text = "Restore All";
			this.btnRestoreAll.Click += new System.EventHandler(this.btnRestoreAll_Click);
			// 
			// MOGRemovedProjectsManager
			// 
			this.Controls.Add(this.btnRestoreAll);
			this.Controls.Add(this.btnPurgeAll);
			this.Controls.Add(this.gbInfo);
			this.Controls.Add(this.btnRestore);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.btnPurge);
			this.Controls.Add(this.lblRemovedProjects);
			this.Controls.Add(this.lvRemovedProjects);
			this.Name = "MOGRemovedProjectsManager";
			this.Size = new System.Drawing.Size(552, 224);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MOGRemovedProjectsManager_KeyDown);
			this.gbInfo.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
		#endregion
		#region User definitions
		private string iniFilePath;
		private string mogProjectsPath;
		#endregion
		#region Properties
		public bool CloseButtonVisible
		{
			get { return this.btnClose.Visible; }
			set { this.btnClose.Visible = value; }
		}
		
		public bool InfoBoxVisible
		{
			get { return this.gbInfo.Visible; }
			set { this.gbInfo.Visible = value; }
		}
		
		#endregion
		#region Event stuff
		public event EventHandler CloseClicked;

		private void OnCloseClicked()
		{
			if (this.CloseClicked != null)
				this.CloseClicked(this, new EventArgs());
		}
		#endregion
		#region Constructors
		public MOGRemovedProjectsManager()
		{
			InitializeComponent();
		}

		public void Init(string iniFilePath, string mogProjectsPath)
		{
			init(iniFilePath, mogProjectsPath);
		}

		private void init(string iniFilePath, string mogProjectsPath)
		{
			this.iniFilePath = iniFilePath;
			this.mogProjectsPath = mogProjectsPath;

			LoadIniFile(iniFilePath);
		}
		#endregion
		#region Member functions
		private void ReloadIni()
		{
			LoadIniFile(this.iniFilePath);
		}

		private void LoadIniFile(string iniFilePath)
		{
			if (!File.Exists(iniFilePath))
				return;
			
			// setup list view and ini reader
			this.lvRemovedProjects.Items.Clear();
			MOG_Ini ini = new MOG_Ini(iniFilePath);

			// Make sure the section exists?
			if (ini.SectionExist("Projects.Deleted"))
			{
				// for each "deleted" project listed
				for (int i = 0; i < ini.CountKeys("Projects.Deleted"); i++)
				{
					string projName = ini.GetKeyNameByIndexSLOW("Projects.Deleted", i);

					// Check if the deleted project's directory is missing?
					if (!DosUtils.DirectoryExistFast(MOG_ControllerSystem.GetSystemDeletedProjectsPath() + "\\" + projName))
					{
						// Auto clean this deleted project from the ini
						ini.RemoveSection( projName + ".Deleted" );
						ini.RemoveString( "projects.deleted", projName );
						continue;
					}

					ListViewItem item = new ListViewItem();
					item.Text = projName;
//					item.Tag = new RemovedProjectInfo( configFile );
					this.lvRemovedProjects.Items.Add(item);
				}
			}

			ini.Close();
		}

		private void FillInfo(ListViewItem item)
		{
			if (item.Tag != null  &&  item.Tag is RemovedProjectInfo)
			{
				this.gbInfo.Enabled = true;
				this.gbInfo.Text = "Info - " + item.Text;
				this.lblPath.Text = ((RemovedProjectInfo)item.Tag).path;
			}
		}

		private void Purge(string projName) 
		{
			Purge(projName, this.iniFilePath);
		}

		private void Purge(string projName, string iniFilename)
		{
			// make sure iniFilename points to valid file
			if (DosUtils.FileExistFast(iniFilename))
			{
				List<string> args = new List<string>();
				args.Add(projName);
				args.Add(iniFilename);

				string message = "Please wait while MOG purges old deleted project.\n" +
								 "   PROJECT: " + projName;
				ProgressDialog progress = new ProgressDialog("Purging deleted project", message, Purge_Worker, args, false);
				progress.ShowDialog();
			}
		}

		private void Purge_Worker(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			List<string> args = e.Argument as List<string>;
			string projName = args[0];
			string iniFilename = args[1];

			// open the ini file
			MOG_Ini ini = new MOG_Ini(iniFilename);
			if (ini != null)
			{
				bool bFailed = false;

				// Attempt to remove the project
				if (DosUtils.DirectoryExist(MOG_ControllerSystem.GetSystemDeletedProjectsPath() + "\\" + projName))
				{
					if (!DosUtils.DirectoryDeleteFast(MOG_ControllerSystem.GetSystemDeletedProjectsPath() + "\\" + projName))
					{
						Utils.ShowMessageBoxExclamation("Can't purge " + projName + ", probably because of a sharing violation", "Project Removal Failure");
						bFailed = true;
					}
				}

				if (!bFailed)
				{
					// make sure projName is a deleted project
					if (ini.SectionExist(projName + ".Deleted"))
					{
						// Remove the project from the list of deleted projects
						ini.RemoveString("Projects.Deleted", projName);
						ini.RemoveSection(projName + ".Deleted");

						BlankInfoBox();

						ini.Save();
					}
				}

				ini.Close();

				LoadIniFile(iniFilename);
			}
		}

		private void Restore(string projName)
		{
			Restore(projName, this.iniFilePath);
		}

		private void Restore(string projName, string iniFilename)
		{
			// make sure iniFilename points to valid file
			if (File.Exists(iniFilename))
			{
				// make sure directory exists
				if (Directory.Exists(MOG_ControllerSystem.GetSystemDeletedProjectsPath() + "\\" + projName))
				{
					if (DosUtils.DirectoryExistFast(MOG_ControllerSystem.GetSystemProjectsPath() + "\\" + projName))
					{
						// A project of this name already exists
						MOG_Prompt.PromptMessage("Project Name Conflict", "Projects cannot be restored over the top of another active project.");
						return;
					}

					List<string> args = new List<string>();
					args.Add(projName);
					args.Add(iniFilename);
					
					string message = "Please wait while MOG restores deleted project.\n" +
									 "   PROJECT: " + projName;
					ProgressDialog progress = new ProgressDialog("Restoring project", message, Restore_Worker, args, false);
					progress.ShowDialog();
				}
			}
		}

		private void Restore_Worker(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			List<string> args = e.Argument as List<string>;
			string projName = args[0];
			string iniFilename = args[1];

			// open the ini file
			MOG_Ini ini = new MOG_Ini(iniFilename);
			if (ini != null)
			{
				// make sure projName is a deleted project
				if (ini.SectionExist(projName + ".Deleted"))
				{
					// Attempt to move the project's directory
					if (DosUtils.DirectoryMoveFast(MOG_ControllerSystem.GetSystemDeletedProjectsPath() + "\\" + projName, MOG_ControllerSystem.GetSystemProjectsPath() + "\\" + projName, true))
					{
						// Restore the project to the active projects list
						ini.PutString("Projects", projName, "");
						ini.RemoveString("Projects.Deleted", projName);
						ini.RenameSection(projName + ".Deleted", projName);

						// Restore the project's database
						MOG_ControllerSystem.GetDB().VerifyTables(projName);
						MOG_Database.ImportProjectTables(projName, this.mogProjectsPath + "\\" + projName);
					}

					BlankInfoBox();

					ini.Save();
				}

				ini.Close();

				LoadIniFile(iniFilename);
			}
		}

		private void BlankInfoBox()
		{
			this.gbInfo.Enabled = false;
			this.gbInfo.Text = "Info";
			this.lblPath.Text = "";
		}
		#endregion
		#region Event handlers
		private void btnPurge_Click(object sender, System.EventArgs e)
		{
			if (Utils.ShowMessageBoxConfirmation("Are you sure you want to permanently delete the selected projects?", "Confirm Project Deletion") != MOGPromptResult.Yes)
				return;

			foreach (ListViewItem item in this.lvRemovedProjects.SelectedItems)
				Purge(item.Text);
		}

		private void btnPurgeAll_Click(object sender, System.EventArgs e)
		{
			if (Utils.ShowMessageBoxConfirmation("Are you sure you want to permanently delete all removed projects?", "Confirm Project Deletion") != MOGPromptResult.Yes)
				return;

			foreach (ListViewItem item in this.lvRemovedProjects.Items)
				Purge(item.Text);
		}

		private void btnRestoreAll_Click(object sender, System.EventArgs e)
		{
			if (Utils.ShowMessageBoxConfirmation("Are you sure you want to restore all removed projects?", "Confirm Project Restoration") != MOGPromptResult.Yes)
				return;

			foreach (ListViewItem item in this.lvRemovedProjects.Items)
				Restore(item.Text);
		}

		private void btnRestore_Click(object sender, System.EventArgs e)
		{
			foreach (ListViewItem item in this.lvRemovedProjects.SelectedItems)
				Restore(item.Text);
		}

		private void btnClose_Click(object sender, System.EventArgs e)
		{
			OnCloseClicked();
		}

		private void lvRemovedProjects_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (this.lvRemovedProjects.SelectedItems.Count == 1)
				FillInfo( this.lvRemovedProjects.SelectedItems[0] );
			else
			{
                BlankInfoBox();
			}
		}

		private void MOGRemovedProjectsManager_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyData == Keys.Escape)
				this.btnClose.PerformClick();
		}

		private void miPurge_Click(object sender, System.EventArgs e)
		{
			this.btnPurge.PerformClick();
		}

		private void miPurgeAll_Click(object sender, System.EventArgs e)
		{
			this.btnPurgeAll.PerformClick();
		}

		private void miRestore_Click(object sender, System.EventArgs e)
		{
			this.btnRestore.PerformClick();
		}

		private void miRestoreAll_Click(object sender, System.EventArgs e)
		{
			this.btnRestoreAll.PerformClick();
		}
	}
		#endregion
	
	#region Supporting classes
	class RemovedProjectInfo
	{
		public string path;
		
		public RemovedProjectInfo(string path)
		{
			this.path = path;
		}
	}
	#endregion
}


/*
#region User definitions
#endregion
#region System definitions
#endregion
#region Constructors
#endregion
#region Member functions
#endregion
#region Events
#endregion
*/ 
