using System;
using System.Drawing;
using System.Threading;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Reflection;
using Microsoft.Win32;
using System.Runtime.InteropServices;

using System.Diagnostics;
using AppLoading;
using MOG_ControlsLibrary;

using MOG.INI;

namespace MOG_Client_Loader
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class FormLoader : System.Windows.Forms.Form
	{
		private const string LoaderTargetDirectory = "Current";
		private const string LoaderConfigFile = "Loader.Ini";
		private const string SystemConfigFile = "MOGConfig.ini";

		#region User variables
		private string gBinLocation;
		private string gExeFile;
		private Thread mMainProcess;
		private SplashForm mSplash;
		private string mRepositoryPath = "";
		public bool   DisableEvents;
		public bool   allUpToDate;
		public bool	  UserAbort;
		public bool   bRunning;
		public bool	  bCopying;
		#endregion
		#region system variables
		private System.Windows.Forms.RichTextBox MapLogs;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.TextBox TargetDir;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ListView CheckListBox;
		private System.Windows.Forms.Panel panel5;
		private System.Windows.Forms.Panel LogPanel;
		private System.Windows.Forms.Panel MapLogsPanel2;
		private System.Windows.Forms.Panel ButtonsPanel;
		private System.Windows.Forms.Button WhatsNewButton;
		private System.Windows.Forms.Button UpdateButton;
		private System.Windows.Forms.Button RunButton;
		private System.Windows.Forms.Button UpdateRunButton;
		private System.Windows.Forms.CheckedListBox LoaderVersionCheckedListBox;
		private System.Windows.Forms.CheckBox UpdateAndRunCheckBox;
		private System.Windows.Forms.Panel MainFilePanel;
		#endregion

		public FormLoader(SplashForm splash)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			mSplash = splash;
			UserAbort = false;
			bRunning = false;

			MOG_Ini loader = new MOG_Ini(LoaderConfigFile);
			if (loader.KeyExist("LOADER", "UpdateRun")) UpdateAndRunCheckBox.Checked = Convert.ToBoolean(loader.GetString("LOADER", "UpdateRun"));

			mMainProcess = new Thread(new ThreadStart(this.Process));
			mMainProcess.Name = "FormLoader.cs::MainProcessThread";
			mMainProcess.Start();
		}

		/// <summary
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		private void Process()
		{
			// Update drive mappings
			//UpdateDriveMappings();

			DisableEvents = true;
			allUpToDate   = true;

			if (InitializeMogRepository())
			{
				// Load up the target ini
				LoadSysIni();

				DisableEvents = false;

				if (!allUpToDate)
				{
					mSplash.Visible = false;
				}
			
				if (!bRunning)
				{
					RunBinary();
				}
			}
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FormLoader));
			this.CheckListBox = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.MapLogs = new System.Windows.Forms.RichTextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.UpdateButton = new System.Windows.Forms.Button();
			this.RunButton = new System.Windows.Forms.Button();
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.TargetDir = new System.Windows.Forms.TextBox();
			this.LogPanel = new System.Windows.Forms.Panel();
			this.MapLogsPanel2 = new System.Windows.Forms.Panel();
			this.WhatsNewButton = new System.Windows.Forms.Button();
			this.ButtonsPanel = new System.Windows.Forms.Panel();
			this.UpdateAndRunCheckBox = new System.Windows.Forms.CheckBox();
			this.UpdateRunButton = new System.Windows.Forms.Button();
			this.MainFilePanel = new System.Windows.Forms.Panel();
			this.LoaderVersionCheckedListBox = new System.Windows.Forms.CheckedListBox();
			this.panel5 = new System.Windows.Forms.Panel();
			this.LogPanel.SuspendLayout();
			this.MapLogsPanel2.SuspendLayout();
			this.ButtonsPanel.SuspendLayout();
			this.MainFilePanel.SuspendLayout();
			this.panel5.SuspendLayout();
			this.SuspendLayout();
			// 
			// CheckListBox
			// 
			this.CheckListBox.CheckBoxes = true;
			this.CheckListBox.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						   this.columnHeader1});
			this.CheckListBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.CheckListBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.CheckListBox.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.CheckListBox.Location = new System.Drawing.Point(0, 94);
			this.CheckListBox.Name = "CheckListBox";
			this.CheckListBox.Size = new System.Drawing.Size(332, 178);
			this.CheckListBox.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.CheckListBox.TabIndex = 0;
			this.CheckListBox.View = System.Windows.Forms.View.Details;
			this.CheckListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.CheckListBox_ItemCheck);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Filename";
			this.columnHeader1.Width = 292;
			// 
			// imageList1
			// 
			this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// MapLogs
			// 
			this.MapLogs.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.MapLogs.Location = new System.Drawing.Point(0, 28);
			this.MapLogs.Name = "MapLogs";
			this.MapLogs.Size = new System.Drawing.Size(444, 64);
			this.MapLogs.TabIndex = 1;
			this.MapLogs.Text = "";
			// 
			// label1
			// 
			this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label1.Location = new System.Drawing.Point(0, 4);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 16);
			this.label1.TabIndex = 2;
			this.label1.Text = "Mapping logs:";
			// 
			// UpdateButton
			// 
			this.UpdateButton.Enabled = false;
			this.UpdateButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.UpdateButton.Location = new System.Drawing.Point(8, 8);
			this.UpdateButton.Name = "UpdateButton";
			this.UpdateButton.Size = new System.Drawing.Size(96, 23);
			this.UpdateButton.TabIndex = 3;
			this.UpdateButton.Text = "Update";
			this.UpdateButton.Click += new System.EventHandler(this.btnUpdate_Click);
			// 
			// RunButton
			// 
			this.RunButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.RunButton.Location = new System.Drawing.Point(8, 32);
			this.RunButton.Name = "RunButton";
			this.RunButton.Size = new System.Drawing.Size(96, 23);
			this.RunButton.TabIndex = 4;
			this.RunButton.Text = "Run";
			this.RunButton.Click += new System.EventHandler(this.btnRun_Click);
			// 
			// progressBar1
			// 
			this.progressBar1.Location = new System.Drawing.Point(8, 112);
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(88, 23);
			this.progressBar1.TabIndex = 5;
			this.progressBar1.Value = 50;
			// 
			// timer1
			// 
			this.timer1.Interval = 10;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// TargetDir
			// 
			this.TargetDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.TargetDir.Location = new System.Drawing.Point(16, 8);
			this.TargetDir.Name = "TargetDir";
			this.TargetDir.Size = new System.Drawing.Size(312, 20);
			this.TargetDir.TabIndex = 6;
			this.TargetDir.Text = "";
			this.TargetDir.TextChanged += new System.EventHandler(this.TargetDir_TextChanged);
			// 
			// LogPanel
			// 
			this.LogPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.LogPanel.Controls.Add(this.MapLogsPanel2);
			this.LogPanel.Controls.Add(this.MapLogs);
			this.LogPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.LogPanel.Location = new System.Drawing.Point(0, 308);
			this.LogPanel.Name = "LogPanel";
			this.LogPanel.Size = new System.Drawing.Size(448, 96);
			this.LogPanel.TabIndex = 7;
			// 
			// MapLogsPanel2
			// 
			this.MapLogsPanel2.Controls.Add(this.label1);
			this.MapLogsPanel2.Controls.Add(this.WhatsNewButton);
			this.MapLogsPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.MapLogsPanel2.Location = new System.Drawing.Point(0, 0);
			this.MapLogsPanel2.Name = "MapLogsPanel2";
			this.MapLogsPanel2.Size = new System.Drawing.Size(444, 28);
			this.MapLogsPanel2.TabIndex = 2;
			// 
			// WhatsNewButton
			// 
			this.WhatsNewButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.WhatsNewButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.WhatsNewButton.Location = new System.Drawing.Point(320, 2);
			this.WhatsNewButton.Name = "WhatsNewButton";
			this.WhatsNewButton.Size = new System.Drawing.Size(120, 23);
			this.WhatsNewButton.TabIndex = 6;
			this.WhatsNewButton.Text = "Whats new notes";
			this.WhatsNewButton.Click += new System.EventHandler(this.WhatsNewButton_Click);
			// 
			// ButtonsPanel
			// 
			this.ButtonsPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.ButtonsPanel.Controls.Add(this.UpdateAndRunCheckBox);
			this.ButtonsPanel.Controls.Add(this.UpdateRunButton);
			this.ButtonsPanel.Controls.Add(this.UpdateButton);
			this.ButtonsPanel.Controls.Add(this.progressBar1);
			this.ButtonsPanel.Controls.Add(this.RunButton);
			this.ButtonsPanel.Dock = System.Windows.Forms.DockStyle.Left;
			this.ButtonsPanel.Location = new System.Drawing.Point(0, 0);
			this.ButtonsPanel.Name = "ButtonsPanel";
			this.ButtonsPanel.Size = new System.Drawing.Size(112, 308);
			this.ButtonsPanel.TabIndex = 8;
			// 
			// UpdateAndRunCheckBox
			// 
			this.UpdateAndRunCheckBox.Checked = true;
			this.UpdateAndRunCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.UpdateAndRunCheckBox.Location = new System.Drawing.Point(8, 64);
			this.UpdateAndRunCheckBox.Name = "UpdateAndRunCheckBox";
			this.UpdateAndRunCheckBox.TabIndex = 7;
			this.UpdateAndRunCheckBox.Text = "Always update and run";
			this.UpdateAndRunCheckBox.Click += new System.EventHandler(this.UpdateAndRunCheckBox_Click);
			// 
			// UpdateRunButton
			// 
			this.UpdateRunButton.Enabled = false;
			this.UpdateRunButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.UpdateRunButton.Location = new System.Drawing.Point(7, 88);
			this.UpdateRunButton.Name = "UpdateRunButton";
			this.UpdateRunButton.Size = new System.Drawing.Size(96, 23);
			this.UpdateRunButton.TabIndex = 6;
			this.UpdateRunButton.Text = "Update and Run";
			this.UpdateRunButton.Click += new System.EventHandler(this.button1_Click);
			// 
			// MainFilePanel
			// 
			this.MainFilePanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.MainFilePanel.Controls.Add(this.CheckListBox);
			this.MainFilePanel.Controls.Add(this.LoaderVersionCheckedListBox);
			this.MainFilePanel.Controls.Add(this.panel5);
			this.MainFilePanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.MainFilePanel.Location = new System.Drawing.Point(112, 0);
			this.MainFilePanel.Name = "MainFilePanel";
			this.MainFilePanel.Size = new System.Drawing.Size(336, 308);
			this.MainFilePanel.TabIndex = 9;
			// 
			// LoaderVersionCheckedListBox
			// 
			this.LoaderVersionCheckedListBox.Dock = System.Windows.Forms.DockStyle.Top;
			this.LoaderVersionCheckedListBox.Location = new System.Drawing.Point(0, 0);
			this.LoaderVersionCheckedListBox.Name = "LoaderVersionCheckedListBox";
			this.LoaderVersionCheckedListBox.Size = new System.Drawing.Size(332, 94);
			this.LoaderVersionCheckedListBox.TabIndex = 2;
			this.LoaderVersionCheckedListBox.SelectedIndexChanged += new System.EventHandler(this.checkedListBox1_SelectedIndexChanged);
			// 
			// panel5
			// 
			this.panel5.Controls.Add(this.TargetDir);
			this.panel5.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel5.Location = new System.Drawing.Point(0, 272);
			this.panel5.Name = "panel5";
			this.panel5.Size = new System.Drawing.Size(332, 32);
			this.panel5.TabIndex = 1;
			// 
			// FormLoader
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(448, 404);
			this.Controls.Add(this.MainFilePanel);
			this.Controls.Add(this.ButtonsPanel);
			this.Controls.Add(this.LogPanel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.Name = "FormLoader";
			this.Opacity = 0;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Loader";
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormLoader_KeyDown);
			this.Closed += new System.EventHandler(this.FormLoader_Closed);
			this.Activated += new System.EventHandler(this.FormLoader_Activated);
			this.LogPanel.ResumeLayout(false);
			this.MapLogsPanel2.ResumeLayout(false);
			this.ButtonsPanel.ResumeLayout(false);
			this.MainFilePanel.ResumeLayout(false);
			this.panel5.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
            //Application.Run(new FormLoader());
		}

		[DllImport("User32.dll", CharSet=CharSet.Auto)]
		public static extern int SendMessageTimeout(
			IntPtr hWnd,
			[MarshalAs(UnmanagedType.U4)] int Msg,
			IntPtr wParam,
			IntPtr lParam,
			[MarshalAs(UnmanagedType.U4)] int fuFlags,
			[MarshalAs(UnmanagedType.U4)] int uTimeout,
			[MarshalAs(UnmanagedType.U4)] ref int lpdwResult);

		public const int HWND_BROADCAST = 0xffff;
		public const int WM_SETTINGCHANGE = 0x001A;
		public const int SMTO_NORMAL = 0x0000;
		public const int SMTO_BLOCK = 0x0001;
		public const int SMTO_ABORTIFHUNG = 0x0002;
		public const int SMTO_NOTIMEOUTIFNOTHUNG = 0x0008;

		private void UpdateDriveMappings()
		{
			string output, command;

			MOG_Ini loader = new MOG_Ini(LoaderConfigFile);

			if (loader.KeyExist("LOADER", "updateDriveMappings"))
			{
				if (loader.GetString("LOADER", "updateDriveMappings") == "0")
				{
					return;
				}
			}
			
			command = String.Concat("\\\\Gandalf\\Main FileServer - Advent\\S - System Drive\\", "MOG_Drive_Update.exe");
			
			// Run the batch file.
			Process p = new Process();
			p.StartInfo.FileName = command;

			p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			p.StartInfo.CreateNoWindow = true;

			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.UseShellExecute = false;

			p.Start();
			p.WaitForExit();
			output = p.StandardOutput.ReadToEnd();

			p.Close();

			MapLogs.Text = output;
		}

		private void ZipProgressBar(int step)
		{
			progressBar1.Value = 0;
			progressBar1.Step = step;
			timer1.Enabled = true;
			timer1.Start();
		}

		public bool InitializeMogRepository()
		{
			// FIRST check if we have a valid repository saved in our loader file
			// Load system Ini
			MOG_Ini loader = new MOG_Ini(LoaderConfigFile);
			if (loader != null && loader.CountSections() > 0)
			{
				if (loader.SectionExist("Loader"))
				{
					if (loader.KeyExist("Loader", "SystemRepositoryPath"))
					{
						// Lets verify if the repository that was saved is really still a repository?  
						if (Directory.Exists(loader.GetString("Loader", "SystemRepositoryPath") + "\\Tools") &&
                            Directory.Exists(loader.GetString("Loader", "SystemRepositoryPath") + "\\ProjectTemplates") &&
							Directory.Exists(loader.GetString("Loader", "SystemRepositoryPath") + "\\Updates"))
						{
							mRepositoryPath = loader.GetString("Loader", "SystemRepositoryPath");

							// SECOND make sure that we have the mog.ini saved correctly in the current directory
							if (File.Exists(Environment.CurrentDirectory + "\\" + LoaderTargetDirectory + "\\MOG.ini"))
							{
								// Also make sure it has a valid repository
								MOG_Ini targetLoader = new MOG_Ini(Environment.CurrentDirectory + "\\" + LoaderTargetDirectory + "\\MOG.ini");
								if (targetLoader != null && targetLoader.CountSections() > 0)
								{
									if (targetLoader.SectionExist("MOG"))
									{
										if (targetLoader.KeyExist("MOG", "SystemRepositoryPath"))
										{
											// Lets verify if the repository that was saved is really still a repository?  
											if (Directory.Exists(targetLoader.GetString("MOG", "SystemRepositoryPath") + "\\Tools") &&
                                                Directory.Exists(targetLoader.GetString("MOG", "SystemRepositoryPath") + "\\ProjectTemplates") &&
												Directory.Exists(targetLoader.GetString("MOG", "SystemRepositoryPath") + "\\Updates"))
											{
												return true;
											}
										}
									}
								}
							}							
						}
					}
				}
			}

			// If any of the above conditions fail, have the user locate our repository path for us
			MogForm_RepositoryBrowser_ClientLoader form = new MogForm_RepositoryBrowser_ClientLoader();
					
			LocateRepository:
				try
				{
					mSplash.Opacity = 0;
					this.WindowState = FormWindowState.Minimized;
					this.Opacity = 1;										
					if (form.ShowDialog() == DialogResult.OK)
					{
						// Determine if the path selected is valid
						if (!File.Exists(form.MOGSelectedRepository + "\\MogRepository.ini"))
						{
							MessageBox.Show(this, "The selected path is not a valid MOG repository.", "Invalid repository");
							goto LocateRepository;
						}
						else
						{
							// Load the MogRepository.ini file found at the location specified by the user
							MOG_Ini repository = new MOG_Ini(form.MOGSelectedRepository + "\\MogRepository.ini");

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
					else
					{
						return false;
					}
				}
				catch(Exception e)
				{
					MessageBox.Show(this, e.Message, "Invalid repository");
					goto LocateRepository;
				}
					
            
			// Double check that we got a valid mog repository path
			if (mRepositoryPath.Length == 0)
			{
				goto LocateRepository;
			}
			else
			{
				// Yup, all is well save it out
				loader.PutString("Loader", "SystemRepositoryPath", mRepositoryPath);
				loader.Save();
				
				// Save out our MOG.ini
				SaveMOGConfiguration();

				this.Opacity = 0;
				mSplash.Opacity = 1;
				this.WindowState = FormWindowState.Normal;
				return true;
			}						
		}

		public void SaveMOGConfiguration()
		{
			// Save out our MOG.ini
			MOG_Ini repositoryIni = new MOG_Ini(Environment.CurrentDirectory + "\\" + LoaderTargetDirectory + "\\MOG.ini");

			// Save repository
			repositoryIni.PutString("MOG", "SystemRepositoryPath", mRepositoryPath);
			repositoryIni.PutString("MOG", "SystemConfiguration", "{SystemRepositoryPath}\\Tools\\" + SystemConfigFile);

			// Save the ini
			repositoryIni.Save();
		}

		public void LoadSysIni()
		{
			try
			{
				// Start the progressBar
				ZipProgressBar(10);
			
				// Load system Ini
				MOG_Ini loader = new MOG_Ini(LoaderConfigFile);

				// Clear our box
				CheckListBox.Items.Clear();

				// Locate the Master Archive drive
				for (int i = 0; i < loader.CountKeys("UPDATE"); i++)
				{
					string SectionToBeUpdated = loader.GetKeyNameByIndexSLOW("UPDATE", i);

					// Get bin locations
					string gSourceLocation = mRepositoryPath + "\\" + loader.GetString(SectionToBeUpdated, "BINLOCATION");
					string TargetLocation = loader.GetString("LOADER", "BINLOCATION");

					if (TargetLocation.CompareTo("NONE") == 0 )
					{
						TargetLocation = String.Concat(Environment.CurrentDirectory, "\\" + LoaderTargetDirectory);

						DirectoryInfo dir = new DirectoryInfo(TargetLocation);
						if ( dir.Exists != true )
						{
							dir.Create();
						}
					}

					// Make sure we have a valid fileList in the target drive
					if (!File.Exists(gSourceLocation + "\\FileList.ini"))
					{
						throw new Exception("Current version of (" + SectionToBeUpdated + ") does not exist at:\n\n " + gSourceLocation + "\n\n Aborting...");
					}

					// Load the filelist associated with this section
					MOG_Ini files = new MOG_Ini(gSourceLocation + "\\FileList.ini");


					// Either update the directory box or the ini
					if(TargetDir.Text == "")
					{
						if (gSourceLocation.CompareTo("") == 0)
						{
							TargetDir.Text = String.Concat(Environment.CurrentDirectory, "\\" + LoaderTargetDirectory);
							gBinLocation = TargetDir.Text;
							loader.PutString("LOADER", "BINLOCATION", TargetDir.Text);
							loader.Save();
						}
						else
						{
							TargetDir.Text = TargetLocation;
							gBinLocation = TargetLocation;
							loader.PutString("LOADER", "BINLOCATION", TargetDir.Text);
							loader.Save();
						}
					}
					else
					{
						gBinLocation = TargetDir.Text;
						loader.PutString("LOADER", "BINLOCATION", TargetDir.Text);
						loader.Save();						
					}

					// Get the executable to launch with the run button
					string exeFile = files.GetString(SectionToBeUpdated, "EXE");

					// Make sure the exe is not NONE
					if (String.Compare(exeFile, "NONE", true) != 0)
					{
						gExeFile = exeFile;
					}

					// Walk thru all files
					for(int x=0; x<files.CountKeys("FILES"); x++)
					{
						ListViewItem item = new ListViewItem();
						string curFile = String.Concat(TargetLocation, "\\", files.GetKeyNameByIndexSLOW("FILES", x));
						string newFile = String.Concat(gSourceLocation, "\\", files.GetKeyNameByIndexSLOW("FILES", x));

						// Check current version against new version	
						FileInfo CurrentBin, NewBin;

						// Check for special folders
						if (curFile.IndexOf("<WIN_SYSTEM>") != -1)
						{
							// Fix path to reflect new dir
							curFile = string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.System), curFile.Substring(curFile.IndexOf(">")+1));
							newFile = string.Concat(newFile.Substring(0, newFile.IndexOf("<")), newFile.Substring(newFile.IndexOf(">")+1));
						}

						CurrentBin = new FileInfo(curFile);
						NewBin = new FileInfo(newFile);

						// Add file to the menu
						item.Text = files.GetKeyNameByIndexSLOW("FILES", x);
						item.SubItems.Add(newFile);
						item.SubItems.Add(gSourceLocation);  // Source directory

						if( (CurrentBin.LastWriteTime.CompareTo(NewBin.LastWriteTime) != 0) || !(CurrentBin.Exists) )
						{
							Debug.Write("Copy - " + curFile + " \tCurrent:" + CurrentBin.LastWriteTime.ToString() + " New:" + NewBin.LastWriteTime.ToString(), "\nNew Check");
						}
						else
						{
							Debug.Write("Skip - " + curFile + " \tCurrent:" + CurrentBin.LastWriteTime.ToString() + " New:" + NewBin.LastWriteTime.ToString(), "\nNew Check");
						}
					
						if( (CurrentBin.LastWriteTime.CompareTo(NewBin.LastWriteTime) != 0) || !(CurrentBin.Exists) )
						{
							if ( NewBin.Exists )
							{
								// Enable buttons
								UpdateButton.Enabled = true;
								UpdateRunButton.Enabled = true;
								RunButton.Enabled = true;
								allUpToDate = false;

								// Check the old file
								item.Checked = true;
							}
							else
							{
								item.Remove();
							}
						}

						// Make sure we don't add an asset twice to the list
						bool alreadyExists = false;
						foreach (ListViewItem lItem in CheckListBox.Items)
						{
							if (String.Compare(lItem.Text, item.Text) == 0)
							{
								alreadyExists = true;
							}						
						}

						if (alreadyExists == false)
						{
							CheckListBox.Items.Add(item);
						}
					}
				}

				if (loader.KeyExist("LOADER", "UpdateRun") && !allUpToDate)
				{
					if (string.Compare(loader.GetString("LOADER", "UpdateRun"), "true", true) == 0)
					{						
						mSplash.updateSplash("Auto Updating to current version...", 500);
					
						if (!UserAbort)
						{
							if (UpdateBinary())
							{
								allUpToDate = true;
								DisableEvents = false;
								RunBinary();
							}
						}
					}
				}
			}
			catch(Exception e)
			{
				MessageBox.Show(this, e.Message, "Error updating to latest version");
				allUpToDate = false;
				Application.Exit();
			}
		}

		private void RunBinary()
		{
			if (!DisableEvents && !UserAbort)
			{
				DisableEvents = true;

				// Check if any exe was passed in on the commandline that would override the one found in the loader.ini
				if (mSplash.mArgs != null && mSplash.mArgs.Length > 0)
				{
					foreach(string argument in mSplash.mArgs)
					{
						// Make sure this executable exists
						if (File.Exists(gBinLocation + "\\" + argument))
						{
							gExeFile = argument;
						}
						else
						{
							MessageBox.Show(gBinLocation + "\\" + argument + "\n\nDoes not exist!  Running:" + gExeFile + " instead.", "Commandline argument does not exist!", MessageBoxButtons.OK);
						}
					}
				}

				string systemCommand = gExeFile;

				// Change our dir to the target dir
				Environment.CurrentDirectory = gBinLocation;

				// Update the batch file.
				//UpdateCommandLineBat();

				// Run the batch file.
				Process p = new Process();
				p.StartInfo.FileName = systemCommand;

				p.StartInfo.UseShellExecute = false;
				
				p.Start();
				bRunning = true;
				p.Close();

				Application.Exit();				
			}
		}

		private bool UpdateBinary()
		{
			string source, target;
			bool success = true;

			progressBar1.Value = 0;			
			bCopying = true;
			mSplash.intializeProgressBar(CheckListBox.Items.Count);

			foreach (ListViewItem item in CheckListBox.Items)
			{
				if (item.Checked)
				{
					// Field 1 has the full source for the item
					source = item.SubItems[1].Text;
					target = String.Concat(gBinLocation, "\\", item.Text);

					FileInfo file = new FileInfo(source);

				// Main copy loop
				CopyFile:

					try
					{
						mSplash.updateSplash(item.Text, 0);
						file.CopyTo(target, true);
					}
					catch(Exception e2)
					{
						// string message = String.Concat("File ", source, " did not copy!");
						DialogResult rc = MessageBox.Show(e2.Message, "", MessageBoxButtons.AbortRetryIgnore);

						switch(rc)
						{
							case DialogResult.Abort:
								success = false;
								Application.Exit();
								break;
							case DialogResult.Retry:
								goto CopyFile;
							case DialogResult.Ignore:
								success = false;
								continue;
						}						
					}
								
					progressBar1.PerformStep();
					item.Checked = false;
				}
			}
            
			bCopying = false;
			progressBar1.Value = 0;

			if (success)
			{
				UpdateRunButton.Enabled = false;
				UpdateButton.Enabled = false;
			}

			return success;
		}
	
		private void btnUpdate_Click(object sender, System.EventArgs e)
		{
			UserAbort = false;
			UpdateBinary();
		}			

		private void btnRun_Click(object sender, System.EventArgs e)
		{
			UserAbort = false;
			RunBinary();
		}

		private void timer1_Tick(object sender, System.EventArgs e)
		{
			if(progressBar1.Value!=100)
			{
				progressBar1.PerformStep();
			}
			else
			{
				timer1.Enabled = false;
				progressBar1.Value = 0;
			}
			
		
		}

		private void TargetDir_TextChanged(object sender, System.EventArgs e)
		{
			if (!DisableEvents)
				LoadSysIni();
		}

		private void CheckListBox_ItemCheck(object sender, System.Windows.Forms.ItemCheckEventArgs e)
		{
			//return;

			if (!DisableEvents)
			{
				UserAbort = false;
				UpdateButton.Enabled = false;
				UpdateRunButton.Enabled = false;
				int NumberFound = 0;

				// Check for any items tat are already checked
				foreach (ListViewItem item in CheckListBox.Items)
				{
					if (item.Checked)
					{
						UpdateButton.Enabled = true;
						UpdateRunButton.Enabled = true;
						NumberFound += 1;
					}
				}

				// Check if the item we clicked is being checked or unchecked
				if (NumberFound == 1)
				{
					if (e.NewValue == CheckState.Unchecked)
					{
						UpdateButton.Enabled = false;
						UpdateRunButton.Enabled = true;
					}
				}
				else if (NumberFound == 0)
				{
					if (e.NewValue == CheckState.Checked)
					{
						UpdateButton.Enabled = true;
						UpdateRunButton.Enabled = true;
					}
				}
			}
		}

		private void FormLoader_Activated(object sender, System.EventArgs e)
		{
			if (allUpToDate)
			{
				RunBinary();
			}
			else
			{
				if (UpdateAndRunCheckBox.Checked)
				{
					this.Opacity = 0;
				}
				else
				{
					this.Opacity = 1;
				}
			}
		}

		private void FormLoader_Closed(object sender, System.EventArgs e)
		{
			Application.Exit();
		}

		private void WhatsNewButton_Click(object sender, System.EventArgs e)
		{
			UserAbort = false;
			NotesForm notes = new NotesForm();
			notes.ShowDialog(this);
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			UserAbort = false;
			if (UpdateBinary())
			{
				RunBinary();
			}
		}

		private void checkedListBox1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
		
		}

		private void UpdateAndRunCheckBox_Click(object sender, System.EventArgs e)
		{
			// Save this setting
			MOG_Ini loader = new MOG_Ini("MOG.ini");
			loader.PutString("LOADER", "UpdateRun", UpdateAndRunCheckBox.Checked.ToString());
			loader.Save();
			UserAbort = false;
		}

		private void FormLoader_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			UserAbort = true;
			this.Opacity = 1;
			mSplash.Visible = false;			
		}
	}
}
