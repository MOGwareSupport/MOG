using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MOG_Server_Loader
{
	/// <summary>
	/// Summary description for ConfigWizard.
	/// </summary>
	public class ConfigWizard : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox tbNewRepositoryLocation;
		private System.Windows.Forms.Button btnBrowseNewRepository;
		private System.Windows.Forms.Button btnBrowseExistingRepository;
		private System.Windows.Forms.TextBox tbExistingRepositoryLocation;
		private System.Windows.Forms.RadioButton rbCreateNewRepository;
		private System.Windows.Forms.RadioButton rbSelectExistingRepository;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
		private System.Windows.Forms.Button btnOK;
		private MOG_Server_Loader.SQLConnectControl sqlConnectControl;
		private System.Windows.Forms.TextBox tbRepositoryName;
		private System.Windows.Forms.Label lblRepositoryName;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ConfigWizard()
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
			this.tbNewRepositoryLocation = new System.Windows.Forms.TextBox();
			this.btnBrowseNewRepository = new System.Windows.Forms.Button();
			this.btnBrowseExistingRepository = new System.Windows.Forms.Button();
			this.tbExistingRepositoryLocation = new System.Windows.Forms.TextBox();
			this.rbCreateNewRepository = new System.Windows.Forms.RadioButton();
			this.rbSelectExistingRepository = new System.Windows.Forms.RadioButton();
			this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.sqlConnectControl = new MOG_Server_Loader.SQLConnectControl();
			this.btnOK = new System.Windows.Forms.Button();
			this.tbRepositoryName = new System.Windows.Forms.TextBox();
			this.lblRepositoryName = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// tbNewRepositoryLocation
			// 
			this.tbNewRepositoryLocation.Location = new System.Drawing.Point(48, 144);
			this.tbNewRepositoryLocation.Name = "tbNewRepositoryLocation";
			this.tbNewRepositoryLocation.Size = new System.Drawing.Size(296, 20);
			this.tbNewRepositoryLocation.TabIndex = 0;
			this.tbNewRepositoryLocation.Text = "";
			// 
			// btnBrowseNewRepository
			// 
			this.btnBrowseNewRepository.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.btnBrowseNewRepository.Location = new System.Drawing.Point(352, 144);
			this.btnBrowseNewRepository.Name = "btnBrowseNewRepository";
			this.btnBrowseNewRepository.Size = new System.Drawing.Size(24, 23);
			this.btnBrowseNewRepository.TabIndex = 1;
			this.btnBrowseNewRepository.Text = "...";
			this.btnBrowseNewRepository.Click += new System.EventHandler(this.btnBrowseNewRepository_Click);
			// 
			// btnBrowseExistingRepository
			// 
			this.btnBrowseExistingRepository.Enabled = false;
			this.btnBrowseExistingRepository.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.btnBrowseExistingRepository.Location = new System.Drawing.Point(352, 192);
			this.btnBrowseExistingRepository.Name = "btnBrowseExistingRepository";
			this.btnBrowseExistingRepository.Size = new System.Drawing.Size(24, 23);
			this.btnBrowseExistingRepository.TabIndex = 3;
			this.btnBrowseExistingRepository.Text = "...";
			this.btnBrowseExistingRepository.Click += new System.EventHandler(this.btnBrowseExistingRepository_Click);
			// 
			// tbExistingRepositoryLocation
			// 
			this.tbExistingRepositoryLocation.Enabled = false;
			this.tbExistingRepositoryLocation.Location = new System.Drawing.Point(48, 192);
			this.tbExistingRepositoryLocation.Name = "tbExistingRepositoryLocation";
			this.tbExistingRepositoryLocation.Size = new System.Drawing.Size(296, 20);
			this.tbExistingRepositoryLocation.TabIndex = 2;
			this.tbExistingRepositoryLocation.Text = "";
			// 
			// rbCreateNewRepository
			// 
			this.rbCreateNewRepository.Checked = true;
			this.rbCreateNewRepository.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.rbCreateNewRepository.Location = new System.Drawing.Point(32, 128);
			this.rbCreateNewRepository.Name = "rbCreateNewRepository";
			this.rbCreateNewRepository.Size = new System.Drawing.Size(184, 16);
			this.rbCreateNewRepository.TabIndex = 4;
			this.rbCreateNewRepository.TabStop = true;
			this.rbCreateNewRepository.Text = "Create a new MOG Repository";
			this.rbCreateNewRepository.CheckedChanged += new System.EventHandler(this.rbCreateNewRepository_CheckedChanged);
			// 
			// rbSelectExistingRepository
			// 
			this.rbSelectExistingRepository.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.rbSelectExistingRepository.Location = new System.Drawing.Point(32, 176);
			this.rbSelectExistingRepository.Name = "rbSelectExistingRepository";
			this.rbSelectExistingRepository.Size = new System.Drawing.Size(216, 16);
			this.rbSelectExistingRepository.TabIndex = 5;
			this.rbSelectExistingRepository.Text = "Select an existing MOG Repository";
			this.rbSelectExistingRepository.CheckedChanged += new System.EventHandler(this.rbSelectExistingRepository_CheckedChanged);
			// 
			// sqlConnectControl
			// 
			this.sqlConnectControl.ButtonsVisible = false;
			this.sqlConnectControl.Cursor = System.Windows.Forms.Cursors.Default;
			this.sqlConnectControl.DatabaseName = "mog16";
			this.sqlConnectControl.DataSource = "NEMESIS";
			this.sqlConnectControl.InitialCatalog = "mog";
			this.sqlConnectControl.Location = new System.Drawing.Point(408, 40);
			this.sqlConnectControl.Name = "sqlConnectControl";
			this.sqlConnectControl.OKButtonVisible = false;
			this.sqlConnectControl.ServerName = "NEMESIS";
			this.sqlConnectControl.Size = new System.Drawing.Size(312, 328);
			this.sqlConnectControl.SQLCancelButtonVisible = false;
			this.sqlConnectControl.SQLTestButtonVisible = true;
			this.sqlConnectControl.TabIndex = 6;
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(648, 392);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 7;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// tbRepositoryName
			// 
			this.tbRepositoryName.Location = new System.Drawing.Point(48, 240);
			this.tbRepositoryName.Name = "tbRepositoryName";
			this.tbRepositoryName.Size = new System.Drawing.Size(184, 20);
			this.tbRepositoryName.TabIndex = 8;
			this.tbRepositoryName.Text = "NewRepository";
			// 
			// lblRepositoryName
			// 
			this.lblRepositoryName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblRepositoryName.Location = new System.Drawing.Point(32, 224);
			this.lblRepositoryName.Name = "lblRepositoryName";
			this.lblRepositoryName.Size = new System.Drawing.Size(104, 16);
			this.lblRepositoryName.TabIndex = 9;
			this.lblRepositoryName.Text = "Repository Name";
			// 
			// ConfigWizard
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(744, 446);
			this.ControlBox = false;
			this.Controls.Add(this.lblRepositoryName);
			this.Controls.Add(this.tbRepositoryName);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.sqlConnectControl);
			this.Controls.Add(this.rbSelectExistingRepository);
			this.Controls.Add(this.rbCreateNewRepository);
			this.Controls.Add(this.btnBrowseExistingRepository);
			this.Controls.Add(this.tbExistingRepositoryLocation);
			this.Controls.Add(this.btnBrowseNewRepository);
			this.Controls.Add(this.tbNewRepositoryLocation);
			this.Name = "ConfigWizard";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Config Wizard";
			this.ResumeLayout(false);

		}
		#endregion

		private bool success = false;

		public string ConnectionString
		{
			get 
			{
				this.sqlConnectControl.BuildConnectionString();
				return this.sqlConnectControl.ConnectionString;
			}
		}

		public string RepositoryName
		{
			get
			{
				if (this.tbRepositoryName.Text != "")
					return this.tbRepositoryName.Text;
				else
					return "NewRepository";
			}
		}

		public bool Success
		{
			get 
			{
				return this.success;
			}
		}

		public string RepositoryPath
		{
			get
			{
				if (this.rbCreateNewRepository.Checked)
					return this.tbNewRepositoryLocation.Text;
				else if (this.rbSelectExistingRepository.Checked)
					return this.tbExistingRepositoryLocation.Text;

				return "";
			}
		}

		private bool CopyDirectory(string source, string dest)
		{
			if (!Directory.Exists(source))
				return false;
			
			// remove trailing backslashes if any
			if (source.EndsWith("\\"))
				source = source.Substring(0, source.Length-1);
			if (dest.EndsWith("\\"))
				dest = dest.Substring(0, dest.Length-1);

			if (!Directory.Exists(dest))
			{
				if ( !Directory.CreateDirectory(dest).Exists )
					return false;
			}

			// copy files and subdirs
			string[] members = Directory.GetFileSystemEntries(source);	// get full names of all files and directories
			foreach (string member in members)
			{
				if (Directory.Exists(member))	// if its a directory
				{
					if (!CopyDirectory(member, dest + "\\" + Path.GetFileName(member)))
						return false;
				}
				else
					File.Copy(member, dest + "\\" + Path.GetFileName(member), true);
			}

			return true;
		}

		private bool Finish()
		{
			if (this.rbCreateNewRepository.Checked)
			{
				// create new mog repository
				string blankReposDir = Environment.CurrentDirectory + "\\setup\\Repository\\MOG";
				if (Directory.Exists(blankReposDir))
				{
					if ( !CopyDirectory(blankReposDir, this.tbNewRepositoryLocation.Text) )
					{
						MessageBox.Show("Couldn't copy " + blankReposDir, "Copy Error");
						return false;
					}
				}
				else
				{
					MessageBox.Show("Couldn't find " + blankReposDir, "Missing Source Repository");
					return false;
				}

				// fixup the INI files within the new repository to point to the correct directories
				string mogIniFile = this.tbNewRepositoryLocation.Text + "\\Tools\\MOGConfig.ini";
				if (File.Exists( mogIniFile ))
				{
					MOG_Ini ini = new MOG_Ini( mogIniFile );
					ini.PutString("MOG", "SystemRepositoryPath", this.tbNewRepositoryLocation.Text);
					ini.PutString("MOG", "Tools", "{SystemRepositoryPath}\\Tools");
					ini.PutString("MOG", "Projects", "{SystemRepositoryPath}\\Projects");
					
					ini.Save();
					ini.Close();
				}
				else
					MessageBox.Show(mogIniFile + " doesn't exist" , "Missing INI File");

				// create MogRepository.ini
				MOG_Ini repositoryIni = new MOG_Ini(this.tbNewRepositoryLocation.Text + "\\MogRepository.ini");
				repositoryIni.PutString("Mog_Repositories", this.RepositoryName, "");
				repositoryIni.PutString(this.RepositoryName, "SystemRepositoryPath", this.tbNewRepositoryLocation.Text);
				repositoryIni.PutString(this.RepositoryName, "SystemConfiguration", "{SystemRepositoryPath}\\Tools\\MOGConfig.ini");
				repositoryIni.Save();
				repositoryIni.Close();

				// copy it to the root if possible
				if (this.tbNewRepositoryLocation.Text.Length > 3)
				{
					// if it's not already pointing to the root
					string root = this.tbNewRepositoryLocation.Text.Substring(0, 3);
					File.Copy(this.tbNewRepositoryLocation.Text + "\\MogRepository.ini", root + "MogRepository.ini", true);
				}
			}
			else if (this.rbSelectExistingRepository.Checked)
			{
			}

			return true;
		}


		private void rbCreateNewRepository_CheckedChanged(object sender, System.EventArgs e)
		{
			this.tbNewRepositoryLocation.Enabled = this.rbCreateNewRepository.Checked;
			this.btnBrowseNewRepository.Enabled = this.rbCreateNewRepository.Checked;
		}

		private void rbSelectExistingRepository_CheckedChanged(object sender, System.EventArgs e)
		{
			this.tbExistingRepositoryLocation.Enabled = this.rbSelectExistingRepository.Checked;
			this.btnBrowseExistingRepository.Enabled = this.rbSelectExistingRepository.Checked;
		}

		private void btnBrowseNewRepository_Click(object sender, System.EventArgs e)
		{
			if (this.folderBrowserDialog.ShowDialog() == DialogResult.OK)
			{
				this.tbNewRepositoryLocation.Text = this.folderBrowserDialog.SelectedPath;
				this.tbRepositoryName.Focus();
				this.tbRepositoryName.SelectAll();
			}
		}

		private void btnBrowseExistingRepository_Click(object sender, System.EventArgs e)
		{
			if (this.folderBrowserDialog.ShowDialog() == DialogResult.OK)
			{
				this.tbExistingRepositoryLocation.Text = this.folderBrowserDialog.SelectedPath;
				this.tbRepositoryName.Focus();
				this.tbRepositoryName.SelectAll();
			}
		}

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			this.success = Finish();
			Hide();
		}
	}
}
