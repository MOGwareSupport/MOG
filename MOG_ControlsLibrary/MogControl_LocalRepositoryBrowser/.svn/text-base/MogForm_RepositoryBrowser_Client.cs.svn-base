using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using MOG;
using MOG.INI;
using MOG.DOSUTILS;
using MOG.CONTROLLER.CONTROLLERSYSTEM;

namespace MOG_ControlsLibrary
{
	/// <summary>
	/// Summary description for MogForm_RepositoryBrowser_Client.
	/// </summary>
	public class MogForm_RepositoryBrowser_Client : System.Windows.Forms.Form
	{
		private string mRepositoryPath = "";
		private string mConfigFile = "";
		private bool mForceRestart;

		private MogControl_LocalRepositoryTreeView_Client mogControl_LocalRepositoryTreeView;
		private System.Windows.Forms.Button CloseButton;
		private System.Windows.Forms.Button RefreshButton;
		private System.Windows.Forms.Button RepCancelButton;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public MogForm_RepositoryBrowser_Client(bool forceRestart)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			mForceRestart = forceRestart;

			Cursor.Current = Cursors.WaitCursor;
			mogControl_LocalRepositoryTreeView.InitializeNetworkDrivesOnly();
			Cursor.Current = Cursors.Default;
		}

		public string MOGSelectedRepository
		{
			get { return this.mogControl_LocalRepositoryTreeView.MOGGameDataTarget; }
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MogForm_RepositoryBrowser_Client));
			this.mogControl_LocalRepositoryTreeView = new MogControl_LocalRepositoryTreeView_Client();
			this.CloseButton = new System.Windows.Forms.Button();
			this.RefreshButton = new System.Windows.Forms.Button();
			this.RepCancelButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// mogControl_LocalRepositoryTreeView
			// 
			this.mogControl_LocalRepositoryTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
				| System.Windows.Forms.AnchorStyles.Left)
				| System.Windows.Forms.AnchorStyles.Right)));
			this.mogControl_LocalRepositoryTreeView.Location = new System.Drawing.Point(4, 8);
			this.mogControl_LocalRepositoryTreeView.MOGAllowDirectoryOpperations = true;
			this.mogControl_LocalRepositoryTreeView.MOGShowFiles = false;
			this.mogControl_LocalRepositoryTreeView.Name = "mogControl_LocalRepositoryTreeView";
			this.mogControl_LocalRepositoryTreeView.Size = new System.Drawing.Size(255, 160);
			this.mogControl_LocalRepositoryTreeView.TabIndex = 0;
			// 
			// CloseButton
			// 
			this.CloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.CloseButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.CloseButton.Location = new System.Drawing.Point(200, 168);
			this.CloseButton.Name = "CloseButton";
			this.CloseButton.Size = new System.Drawing.Size(56, 23);
			this.CloseButton.TabIndex = 1;
			this.CloseButton.Text = "OK";
			this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
			// 
			// RefreshButton
			// 
			this.RefreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.RefreshButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.RefreshButton.Location = new System.Drawing.Point(8, 168);
			this.RefreshButton.Name = "RefreshButton";
			this.RefreshButton.Size = new System.Drawing.Size(56, 23);
			this.RefreshButton.TabIndex = 2;
			this.RefreshButton.Text = "Refresh";
			this.RefreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
			// 
			// RepCancelButton
			// 
			this.RepCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.RepCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.RepCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.RepCancelButton.Location = new System.Drawing.Point(136, 168);
			this.RepCancelButton.Name = "RepCancelButton";
			this.RepCancelButton.Size = new System.Drawing.Size(56, 23);
			this.RepCancelButton.TabIndex = 3;
			this.RepCancelButton.Text = "Cancel";
			// 
			// MogForm_RepositoryBrowser_Client
			// 
			this.AcceptButton = this.CloseButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(264, 197);
			this.Controls.Add(this.RepCancelButton);
			this.Controls.Add(this.RefreshButton);
			this.Controls.Add(this.CloseButton);
			this.Controls.Add(this.mogControl_LocalRepositoryTreeView);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.Name = "MogForm_RepositoryBrowser_Client";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Locate a valid MOG repository...";
			this.ResumeLayout(false);

		}
		#endregion

		private void RefreshButton_Click(object sender, System.EventArgs e)
		{
			this.Cursor = Cursors.WaitCursor;
			this.mogControl_LocalRepositoryTreeView.InitializeNetworkDrivesOnly();
			this.Cursor = Cursors.Default;
		}

		private void CloseButton_Click(object sender, System.EventArgs e)
		{
		LocateRepository:

			try
			{
				// Determine if the path selected is valid
				if (!File.Exists(MOGSelectedRepository + "\\MogRepository.ini"))
				{
					MessageBox.Show(this, "The selected path is not a valid MOG repository.", "Invalid repository");
					this.DialogResult = DialogResult.Retry;
					return;
				}
				else
				{
					// Load the MogRepository.ini file found at the location specified by the user
					MOG_Ini repository = new MOG_Ini(MOGSelectedRepository + "\\MogRepository.ini");

					// Does this MogRepository have at least one valid repository path
					if (repository.SectionExist("Mog_Repositories"))
					{
						// If there is only one specified repository, choose that one
						if (repository.CountKeys("Mog_Repositories") == 1)
						{
							// Get the section
							string section = repository.GetKeyNameByIndexSLOW("Mog_Repositories", 0);

							// Get the path from that section
							if (repository.SectionExist(section) && repository.KeyExist(section, "SystemRepositoryPath"))
							{
								mRepositoryPath = repository.GetString(section, "SystemRepositoryPath");

								// Now set the config
								if (repository.SectionExist(section) && repository.KeyExist(section, "SystemConfiguration"))
								{
									mConfigFile = repository.GetString(section, "SystemConfiguration");
								}
								else
								{
									MessageBox.Show(this, "The selected path does not have or is missing a System Configuration file.", "Invalid repository");
									goto LocateRepository;
								}
							}
							else
							{
								MessageBox.Show(this, "The selected path does not have or is missing a repository path.", "Invalid repository");
								goto LocateRepository;
							}
						}
						else if (repository.CountKeys("Mog_Repositories") > 1)
						{
							// The user must now choose which repository to use
							MogForm_MultiRepository multiRep = new MogForm_MultiRepository();
							for (int i = 0; i < repository.CountKeys("Mog_Repositories"); i++)
							{
								multiRep.RepositoryComboBox.Items.Add(repository.GetKeyNameByIndexSLOW("Mog_Repositories", i));
							}
							multiRep.RepositoryComboBox.SelectedIndex = 0;

							// Show the form to the user and have him select between the repository sections found
							if (multiRep.ShowDialog() == DialogResult.OK)
							{
								// Get the section
								string userSection = multiRep.RepositoryComboBox.Text;

								// Get the path from that section
								if (repository.SectionExist(userSection) && repository.KeyExist(userSection, "SystemRepositoryPath"))
								{
									mRepositoryPath = repository.GetString(userSection, "SystemRepositoryPath");

									// Now set the config
									if (repository.SectionExist(userSection) && repository.KeyExist(userSection, "SystemConfiguration"))
									{
										mConfigFile = repository.GetString(userSection, "SystemConfiguration");
									}
									else
									{
										MessageBox.Show(this, "The selected path does not have or is missing a System Configuration file.", "Invalid repository");
										goto LocateRepository;
									}
								}
								else
								{
									MessageBox.Show(this, "The selected path does not have or is missing a repository path.", "Invalid repository");
									goto LocateRepository;
								}
							}
							else
							{
								goto LocateRepository;
							}
						}
						else
						{
							MessageBox.Show(this, "The selected path does not have or is missing a repository path.", "Invalid repository");
							goto LocateRepository;
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.Message, "Invalid repository");
				goto LocateRepository;
			}


			// Double check that we got a valid mog repository path
			if (mRepositoryPath.Length == 0)
			{
				goto LocateRepository;
			}
			else
			{
				// Save out our MOG.ini
				SaveMOGConfiguration();

				if (mForceRestart)
				{
					// Close our application
					MessageBox.Show(this, "Changing of the MOG repository requires MOG to restart.  We are now shutting down the client. When complete, restart MOG for changes to be effective.", "Shutting down MOG", MessageBoxButtons.OK);
				}

			}
		}

		public void SaveMOGConfiguration()
		{
			string configFilename = MOG_Main.FindInstalledConfigFile();
			MOG_Ini ini = new MOG_Ini();

			if (ini.Load(configFilename))
			{
				// Save repository
				ini.PutString("MOG", "SystemRepositoryPath", mRepositoryPath);
				ini.PutString("MOG", "SystemConfiguration", mConfigFile);

				ini.Close();

				// If we are the installed MOG, update the loaders ini too		
				if (DosUtils.FileExistFast(MOG_Main.GetExecutablePath() + "\\..\\Loader.ini"))
				{
					MOG_Ini LoaderIni = new MOG_Ini(MOG_Main.GetExecutablePath() + "\\..\\Loader.ini");

					LoaderIni.PutString("LOADER", "SystemRepositoryPath", mRepositoryPath);

					LoaderIni.Save();
				}
			}
		}
	}
}
