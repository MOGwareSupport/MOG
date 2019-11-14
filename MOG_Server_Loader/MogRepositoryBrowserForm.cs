using System;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using MOG;

namespace MOG_Server_Loader
{
	/// <summary>
	/// Summary description for MogRepositoryBrowserForm.
	/// </summary>
	public class MogRepositoryBrowserForm : System.Windows.Forms.Form
	{
		#region System defs
		private System.Windows.Forms.TreeView tvDiskTree;
		private System.Windows.Forms.Panel pnlTreeViewsBack;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.TreeView tvReposView;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnPlaceRepository;
		private System.Windows.Forms.ContextMenu cmDiskView;
		private System.Windows.Forms.MenuItem miPlaceRepository;
		private System.Windows.Forms.MenuItem miRefreshDiskTree;
		private System.Windows.Forms.ImageList imageList;
		private System.Windows.Forms.Label lblSelectedPath;
		private System.Windows.Forms.MenuItem miShowRepositoryInfo;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.ContextMenu cmRepositoryView;
		private System.Windows.Forms.MenuItem miEditIni;
		private System.Windows.Forms.MenuItem miCreateFolder;
		private System.Windows.Forms.MenuItem miRemoveFolder;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem miClean;
		private System.ComponentModel.IContainer components;

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
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MogRepositoryBrowserForm));
			this.tvDiskTree = new System.Windows.Forms.TreeView();
			this.cmDiskView = new System.Windows.Forms.ContextMenu();
			this.miPlaceRepository = new System.Windows.Forms.MenuItem();
			this.miShowRepositoryInfo = new System.Windows.Forms.MenuItem();
			this.miClean = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.miCreateFolder = new System.Windows.Forms.MenuItem();
			this.miRemoveFolder = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.miRefreshDiskTree = new System.Windows.Forms.MenuItem();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.pnlTreeViewsBack = new System.Windows.Forms.Panel();
			this.tvReposView = new System.Windows.Forms.TreeView();
			this.cmRepositoryView = new System.Windows.Forms.ContextMenu();
			this.miEditIni = new System.Windows.Forms.MenuItem();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnPlaceRepository = new System.Windows.Forms.Button();
			this.lblSelectedPath = new System.Windows.Forms.Label();
			this.pnlTreeViewsBack.SuspendLayout();
			this.SuspendLayout();
			// 
			// tvDiskTree
			// 
			this.tvDiskTree.ContextMenu = this.cmDiskView;
			this.tvDiskTree.Dock = System.Windows.Forms.DockStyle.Left;
			this.tvDiskTree.HideSelection = false;
			this.tvDiskTree.ImageList = this.imageList;
			this.tvDiskTree.Location = new System.Drawing.Point(0, 0);
			this.tvDiskTree.Name = "tvDiskTree";
			this.tvDiskTree.Size = new System.Drawing.Size(240, 312);
			this.tvDiskTree.TabIndex = 0;
			this.tvDiskTree.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tvDiskTree_MouseDown);
			this.tvDiskTree.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.tvDiskTree_AfterLabelEdit);
			this.tvDiskTree.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvDiskTree_BeforeExpand);
			// 
			// cmDiskView
			// 
			this.cmDiskView.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					   this.miPlaceRepository,
																					   this.miShowRepositoryInfo,
																					   this.miClean,
																					   this.menuItem1,
																					   this.miCreateFolder,
																					   this.miRemoveFolder,
																					   this.menuItem2,
																					   this.miRefreshDiskTree});
			this.cmDiskView.Popup += new System.EventHandler(this.cmDiskView_Popup);
			// 
			// miPlaceRepository
			// 
			this.miPlaceRepository.Index = 0;
			this.miPlaceRepository.Shortcut = System.Windows.Forms.Shortcut.Ins;
			this.miPlaceRepository.Text = "Place Repository here...";
			this.miPlaceRepository.Click += new System.EventHandler(this.miPlaceRepository_Click);
			// 
			// miShowRepositoryInfo
			// 
			this.miShowRepositoryInfo.Checked = true;
			this.miShowRepositoryInfo.Index = 1;
			this.miShowRepositoryInfo.Text = "Show Repository Info";
			this.miShowRepositoryInfo.Click += new System.EventHandler(this.miShowRepositoryInfo_Click);
			// 
			// miClean
			// 
			this.miClean.Index = 2;
			this.miClean.Text = "Clean...";
			this.miClean.Click += new System.EventHandler(this.miClean_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 3;
			this.menuItem1.Text = "-";
			// 
			// miCreateFolder
			// 
			this.miCreateFolder.Index = 4;
			this.miCreateFolder.Shortcut = System.Windows.Forms.Shortcut.CtrlN;
			this.miCreateFolder.Text = "New Folder";
			this.miCreateFolder.Click += new System.EventHandler(this.miCreateFolder_Click);
			// 
			// miRemoveFolder
			// 
			this.miRemoveFolder.Index = 5;
			this.miRemoveFolder.Shortcut = System.Windows.Forms.Shortcut.Del;
			this.miRemoveFolder.Text = "Remove Folder";
			this.miRemoveFolder.Click += new System.EventHandler(this.miRemoveFolder_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 6;
			this.menuItem2.Text = "-";
			// 
			// miRefreshDiskTree
			// 
			this.miRefreshDiskTree.DefaultItem = true;
			this.miRefreshDiskTree.Index = 7;
			this.miRefreshDiskTree.Shortcut = System.Windows.Forms.Shortcut.F5;
			this.miRefreshDiskTree.Text = "Refresh";
			this.miRefreshDiskTree.Click += new System.EventHandler(this.miRefreshDiskTree_Click);
			// 
			// imageList
			// 
			this.imageList.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// pnlTreeViewsBack
			// 
			this.pnlTreeViewsBack.Controls.Add(this.tvReposView);
			this.pnlTreeViewsBack.Controls.Add(this.splitter1);
			this.pnlTreeViewsBack.Controls.Add(this.tvDiskTree);
			this.pnlTreeViewsBack.Location = new System.Drawing.Point(8, 24);
			this.pnlTreeViewsBack.Name = "pnlTreeViewsBack";
			this.pnlTreeViewsBack.Size = new System.Drawing.Size(496, 312);
			this.pnlTreeViewsBack.TabIndex = 1;
			// 
			// tvReposView
			// 
			this.tvReposView.ContextMenu = this.cmRepositoryView;
			this.tvReposView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tvReposView.HideSelection = false;
			this.tvReposView.ImageList = this.imageList;
			this.tvReposView.Location = new System.Drawing.Point(243, 0);
			this.tvReposView.Name = "tvReposView";
			this.tvReposView.Size = new System.Drawing.Size(253, 312);
			this.tvReposView.TabIndex = 2;
			this.tvReposView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tvReposView_MouseDown);
			this.tvReposView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tvReposView_KeyUp);
			// 
			// cmRepositoryView
			// 
			this.cmRepositoryView.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																							 this.miEditIni});
			this.cmRepositoryView.Popup += new System.EventHandler(this.cmRepositoryView_Popup);
			// 
			// miEditIni
			// 
			this.miEditIni.DefaultItem = true;
			this.miEditIni.Index = 0;
			this.miEditIni.Text = "Edit...";
			this.miEditIni.Click += new System.EventHandler(this.miEditIni_Click);
			// 
			// splitter1
			// 
			this.splitter1.Location = new System.Drawing.Point(240, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(3, 312);
			this.splitter1.TabIndex = 1;
			this.splitter1.TabStop = false;
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.Location = new System.Drawing.Point(424, 352);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 2;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			this.btnOK.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tvReposView_KeyUp);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(344, 352);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tvReposView_KeyUp);
			// 
			// btnPlaceRepository
			// 
			this.btnPlaceRepository.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnPlaceRepository.Location = new System.Drawing.Point(16, 352);
			this.btnPlaceRepository.Name = "btnPlaceRepository";
			this.btnPlaceRepository.Size = new System.Drawing.Size(112, 23);
			this.btnPlaceRepository.TabIndex = 4;
			this.btnPlaceRepository.Text = "Place Repository";
			this.btnPlaceRepository.Click += new System.EventHandler(this.btnPlaceRepository_Click);
			this.btnPlaceRepository.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tvReposView_KeyUp);
			// 
			// lblSelectedPath
			// 
			this.lblSelectedPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblSelectedPath.Location = new System.Drawing.Point(16, 8);
			this.lblSelectedPath.Name = "lblSelectedPath";
			this.lblSelectedPath.Size = new System.Drawing.Size(472, 16);
			this.lblSelectedPath.TabIndex = 5;
			// 
			// MogRepositoryBrowserForm
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(512, 390);
			this.Controls.Add(this.lblSelectedPath);
			this.Controls.Add(this.btnPlaceRepository);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.pnlTreeViewsBack);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "MogRepositoryBrowserForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Browse MOG Repositories";
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tvReposView_KeyUp);
			this.pnlTreeViewsBack.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
		#endregion
		#region Member vars
		private rTreeNode myComputerNode = null;
		private bool forceRepositorySelection = false;
		private string selectedPath = "";
		#endregion
		#region Properties
		public string OKButtonText
		{
			get { return this.btnOK.Text; }
			set { this.btnOK.Text = value; }
		}

		public string CancelButtonText
		{
			get { return this.btnCancel.Text; }
			set { this.btnCancel.Text = value; }
		}

		public bool ShowPlaceNewRepositoryButton
		{
			get { return this.btnPlaceRepository.Visible; }
			set { this.btnPlaceRepository.Visible = value; }
		}

		public bool ShowRepositoryInfoVisibleMenuItem
		{
			get { return this.miShowRepositoryInfo.Visible; }
			set { this.miShowRepositoryInfo.Visible = value; }
		}

		public string SelectedPath
		{
			get { return this.selectedPath; }
		}

		public bool RepositoryViewVisible
		{
			get { return this.tvReposView.Visible; }
			set	{ SetRepositoryViewVisible(value); }
		}

		public bool ForceRepositorySelection
		{
			get { return this.forceRepositorySelection; }
			set
			{
				this.forceRepositorySelection = value;
				if (value)
				{
					this.btnOK.Enabled = SelectionValidRepository();
				}
				else
				{
					this.btnOK.Enabled = true;
				}
			}
		}
		#endregion
		#region Constants
		private int IMGINDEX_MYCOMPUTER		= 0;
		private int IMGINDEX_DISKDRIVE		= 1;
		private int IMGINDEX_FOLDER			= 2;
		private int IMGINDEX_MOGDRIVE		= 3;
		private int IMGINDEX_MOGFOLDER		= 4;
		private int IMGINDEX_BROKENDRIVE	= 5;
		private int IMGINDEX_BROKENFOLDER	= 6;
		private int IMGINDEX_INIFILE		= 7;
		#endregion
		#region Constructors
		public MogRepositoryBrowserForm()
		{
			InitializeComponent();

			RefreshDiskTree();
		}
		#endregion
		#region Member functions

		#region Utils

		#region Repository Creation
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

		private bool PlaceRepository(string path)
		{
			if (!Directory.Exists(path))
				return false;

			return PerformRepositoryCreation(path, true);
		}

		private bool PerformRepositoryCreation(string path, bool confirm)
		{
			// confirm repository creation
			if (confirm  &&  MessageBox.Show("Are you sure you want to create a new MOG Repository at '" + path + "'?   ", 
				"Confirm Repository Creation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
			{
				return false;
			}

			// Warn the user if the repository was on a local drive
			string drive = Path.GetPathRoot(path);
			int type = (int)GetDriveType(drive);
			if (type != DRIVE_TYPE_NETWORK)
			{
				if (MessageBox.Show(this, "It is not recommended to place a MOG Repository on a local drive because it will not be accessible to other users on the network.", "Local Drive Warning", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
				{
					return false;
				}
			}

			if (CreateMOGRepository( path ))
			{
				MessageBox.Show("New MOG Repository created at '" + path + "'", "Creation Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);
				//infoBox.Dispose();
				//Cursor.Current = Cursors.Default;
				return true;
			}
			else
			{
				MessageBox.Show("Failed to create a MOG Repository at '" + path + "'", "Creation Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				//infoBox.Dispose();
				//Cursor.Current = Cursors.Default;
				return false;
			}
		}

		private bool CreateMOGRepository(string repositoryPath)
		{
			bool bFailed = false;

			// Make sure we have a valid repository specified?
			if (repositoryPath.Length != 0)
			{
				// Get the root of this path making sure it ends with a backslash?
				string rootPath = Path.GetPathRoot(repositoryPath);
				if (!rootPath.EndsWith("\\"))
				{
					rootPath = rootPath + "\\";
				}

				// Attempt to create a test file to assertain whether we have proper permissions
				try
				{
					string TestFilename = rootPath + "TestWriteAccess";
					StreamWriter sw = new StreamWriter(TestFilename);
					sw.WriteLine("This is a temporary file used to test write permissions.");
					sw.Close();
					File.Delete(TestFilename);
				}
				catch
				{
					MessageBox.Show("The specified repository is read only.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return false;
				}

				// Build a list of all existing repository names within this root path
				ArrayList existingRepositoryNames = new ArrayList();
				// Check if there is already a MOGRepository listed within this path?
				string rootRepositoryFilename = rootPath + "MOGRepository.ini";
				if (File.Exists(rootRepositoryFilename))
				{
					// Open the MOGRepository file
					MOG_Ini rootIni = new MOG_Ini(rootRepositoryFilename);
					if (rootIni.SectionExist("MOG_REPOSITORIES"))
					{
						// Extract all the listed repository names
						for (int i = 0; i < rootIni.CountKeys("MOG_REPOSITORIES"); i++)
						{
							existingRepositoryNames.Add(rootIni.GetKeyNameByIndex("MOG_REPOSITORIES", i));
						}
					}
					rootIni.Close();
				}

				// Get a name for the new repository
				string repositoryName = "";
				
				// Make sure we keep trying until we fall the the very bottom and establish our valid repository name
				while (true)
				{
					repositoryName = Microsoft.VisualBasic.Interaction.InputBox("Repository Name", "Please enter a name for the new MOG Repository", "MOG_REPOSITORY",-1,-1);
					
					// Make sure the name they entered is valid
					if (repositoryName == "")
					{
						MessageBox.Show("Please enter a name for the new MOG Repository", "Invalid Name", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						continue;
					}
					
					// Make sure this name isn't already listed?
					foreach (string reposName in existingRepositoryNames)
					{
						if (string.Compare(repositoryName, reposName, true) == 0)
						{
							MessageBox.Show("A MOG Repository of the name you specified already exists", "Name Exists", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
							continue;
						}
					}
				
					break;
				}

				// create new mog repository
				InformerBoxForm infoBox = new InformerBoxForm("Creating MOG Repository...", "Please Wait");
				infoBox.Show();
				Application.DoEvents();
				Cursor.Current = Cursors.WaitCursor;
				string blankReposDir = MOG_Main.GetExecutablePath_StripCurrentDirectory() + "\\setup\\Repository\\MOG";
				if (Directory.Exists(blankReposDir))
				{
					// Copy the blank template repository over
					if (CopyDirectory(blankReposDir, repositoryPath))
					{
						// Fixup the INI files within the new repository to point to the correct directories
						// Update the Tools primary config file within the Repository's Tools directory
						MOG_Ini ini = new MOG_Ini(repositoryPath + "\\Tools\\MOGConfig.ini");
						ini.PutString("MOG", "SystemRepositoryPath", repositoryPath);
						ini.PutString("MOG", "Tools", "{SystemRepositoryPath}\\Tools");
						ini.PutString("MOG", "Projects", "{SystemRepositoryPath}\\Projects");
						ini.Save();
						ini.Close();
						
						// Create the repository's MogRepository.ini
						MOG_Ini repositoryIni = new MOG_Ini(repositoryPath + "\\MogRepository.ini");
						repositoryIni.PutString("Mog_Repositories", repositoryName, "");
						repositoryIni.PutString(repositoryName, "SystemRepositoryPath", repositoryPath);
						repositoryIni.PutString(repositoryName, "SystemConfiguration", "{SystemRepositoryPath}\\Tools\\MOGConfig.ini");
						repositoryIni.Save();
						repositoryIni.Close();

						// Write our repository information into the rootRepositoryFilename
						MOG_Ini rootIni = new MOG_Ini(rootRepositoryFilename);
						rootIni.PutString("Mog_Repositories", repositoryName, "");
						rootIni.PutString(repositoryName, "SystemRepositoryPath", repositoryPath);
						rootIni.PutString(repositoryName, "SystemConfiguration", "{SystemRepositoryPath}\\Tools\\MOGConfig.ini");
						rootIni.Save();
						rootIni.Close();
					}
					else
					{
						MessageBox.Show("Couldn't copy '" + blankReposDir + "'", "Copy Error");
						bFailed = true;
					}
				}
				else
				{
					MessageBox.Show("Couldn't find '" + blankReposDir + "'", "Missing Source Repository");
					bFailed = true;
				}

				// Clean up
				infoBox.Dispose();
				Cursor.Current = Cursors.Default;
			}
			else
			{
				MessageBox.Show("Invalid Repository Path Specified", "Creation Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				bFailed = true;
			}

			if (!bFailed)
			{
				return true;
			}

			return false;
		}
		#endregion

		private void PopulateNode(rTreeNode rtn)
		{
			if (rtn == null  ||  rtn.Populated)
				return;

			rtn.Nodes.Clear();

			//MessageBox.Show(rtn.DiskPath + " contains " + Directory.GetDirectories(rtn.DiskPath).Length.ToString() + " directories");
			Cursor.Current = Cursors.WaitCursor;

			foreach (string subdir in Directory.GetDirectories(rtn.DiskPath))
			{
				rTreeNode node = null;

				try
				{
					if (ContainsMogRepository_Shallow(subdir))
					{
						node = new rTreeNode( Path.GetFileName(subdir), subdir, this.IMGINDEX_MOGFOLDER );
						node.IsFolder = true;
						if (Directory.GetDirectories(subdir).Length > 0)
							node.Nodes.Add(new rTreeNode("DUMMYNODE"));
							
						rtn.Nodes.Add( node );
					}
					else if (File.Exists(subdir + "\\MogRepository.ini"))
					{
						node = new rTreeNode( Path.GetFileName(subdir), subdir, this.IMGINDEX_BROKENFOLDER );
						node.IsFolder = true;
						if (Directory.GetDirectories(subdir).Length > 0)
							node.Nodes.Add(new rTreeNode("DUMMYNODE"));

						rtn.Nodes.Add( node );
					}
					else
					{
						node = new rTreeNode( Path.GetFileName(subdir), subdir, this.IMGINDEX_FOLDER );
						node.IsFolder = true;
						if (Directory.GetDirectories(subdir).Length > 0)
							node.Nodes.Add(new rTreeNode("DUMMYNODE"));

						rtn.Nodes.Add( node );
					}
				}
				catch
				{
					if (node != null)
					{
						node.ForeColor = Color.Red;
						node.Nodes.Clear();
						rtn.Nodes.Add(node);
					}
				}

			
			}
			

			Cursor.Current = Cursors.Default;
			rtn.Populated = true;
		}

		private rTreeNode GetRootNodeMinusOne(rTreeNode rtn)
		{
			if (rtn == null  ||  rtn.TreeView == null)
				return null;

			rTreeNode parent = rtn;
			while (parent.Parent != null  &&  parent.Parent.Parent != null)
				parent = parent.Parent as rTreeNode;

			return parent;
		}

		private bool SelectionValidRepository()
		{
			if (this.tvDiskTree.SelectedNode == null)
				return false;

			if (this.tvDiskTree.SelectedNode.ImageIndex == this.IMGINDEX_MOGDRIVE  ||  this.tvDiskTree.SelectedNode.ImageIndex == this.IMGINDEX_MOGFOLDER)
				return true;

			return false;
		}

		private void SetRepositoryViewVisible(bool isVisible)
		{
			this.tvReposView.Visible = isVisible;

			// fixup the Dock values and splitter visibility so the disk tree absorbs all the space or makes room for the repository view
			if (this.tvReposView.Visible)
			{
				this.splitter1.Visible = true;
				this.tvDiskTree.Dock = DockStyle.Left;
			}
			else
			{
				this.splitter1.Visible = false;
				this.tvDiskTree.Dock = DockStyle.Fill;
			}
		}
		#endregion


		#region Disk Tree Loading

		private bool ContainsMogRepository_Shallow(string path)
		{
			if (path == null)
				return false;

			path = path.Trim("\\".ToCharArray());

			if (!Directory.Exists(path))
				return false;

			if (!File.Exists(path + "\\MogRepository.ini"))
				return false;

			MOG_Ini ini = new MOG_Ini(path + "\\MogRepository.ini");
			if (!ini.SectionExist("MOG_REPOSITORIES"))
			{
				ini.Close();
				return false;
			}

			// verify repository existence
			for (int i = 0; i < ini.CountKeys("MOG_REPOSITORIES"); i++)
			{
				string sectionName = ini.GetKeyNameByIndex("MOG_REPOSITORIES", i);
				if (ini.SectionExist(sectionName))
				{
					if (ini.KeyExist(sectionName, "SystemRepositoryPath"))
					{
						string reposPath = ini.GetString(sectionName, "SystemRepositoryPath").Trim("\\".ToCharArray());
						if (Directory.Exists(reposPath + "\\Tools"))
						{
							if (Directory.GetFiles(reposPath + "\\Tools", "*.ini").Length > 0)
							{
								// we've found an INI file in a (hopefully) valid repository, return true
								ini.Close();
								return true;
							}
						}
					}
				}
			}

			ini.Close();
			return false;
		}

		private const int DRIVE_TYPE_CD			= 5;
		private const int DRIVE_TYPE_HD			= 3;
		private const int DRIVE_TYPE_RAM		= 6;
		private const int DRIVE_TYPE_NETWORK	= 4;
		private const int DRIVE_TYPE_FLOPPY		= 2;

		[DllImport("kernel32.dll")]
			//TYPE:		
			//5-A CD-ROM drive. 
			//3-A hard drive. 
			//6-A RAM disk. 
			//4-A network drive or a drive located on a network server. 
			//2-A floppy drive or some other removable-disk drive. 
		public static extern long GetDriveType(string driveLetter);

		
		public void RefreshDiskTree()
		{
			this.tvDiskTree.BeginUpdate();

			this.tvDiskTree.Nodes.Clear();
			this.tvReposView.Nodes.Clear();

			this.myComputerNode = new rTreeNode("My Computer", "", this.IMGINDEX_MYCOMPUTER);

			foreach (string driveName in Directory.GetLogicalDrives())
			{
				try
				{
					// Only add local physical hard drives to our tree
					int type = (int)GetDriveType(driveName);
					if (type == DRIVE_TYPE_HD || type == DRIVE_TYPE_NETWORK)
					{
						if (Directory.Exists(driveName))
						{
							if (ContainsMogRepository_Shallow(driveName))
							{
								rTreeNode node = new rTreeNode(driveName, driveName, this.IMGINDEX_MOGDRIVE);
								node.IsDrive = true;
								if (Directory.GetDirectories(driveName).Length > 0)
									node.Nodes.Add(new rTreeNode("DUMMYNODE"));

								this.myComputerNode.Nodes.Add(node);
							}
							else if (File.Exists(driveName + "MogRepository.ini"))
							{
								rTreeNode node = new rTreeNode(driveName, driveName, this.IMGINDEX_BROKENDRIVE);
								node.IsDrive = true;
								if (Directory.GetDirectories(driveName).Length > 0)
									node.Nodes.Add(new rTreeNode("DUMMYNODE"));

								this.myComputerNode.Nodes.Add(node);
							}
							else
							{
								rTreeNode node = new rTreeNode(driveName, driveName, this.IMGINDEX_DISKDRIVE);
								node.IsDrive = true;
								if (Directory.GetDirectories(driveName).Length > 0)
									node.Nodes.Add(new rTreeNode("DUMMYNODE"));

								this.myComputerNode.Nodes.Add(node);
							}
						}
					}
				}
				catch
				{
				}
			}

			this.tvDiskTree.Nodes.Add(this.myComputerNode);
			this.myComputerNode.Populated = true;
			this.myComputerNode.Expand();

			this.tvDiskTree.EndUpdate();
		}
		#endregion


		#region Repository Tree Loading
		private void BuildRepositoryTreeView(string mogRepositoryIni)
		{
			if (mogRepositoryIni == null)
				return;

			mogRepositoryIni = mogRepositoryIni.Trim("\\".ToCharArray());

			// fixup mogRepositoryIni if necessary to contain the MogRepository.ini file
			if (!mogRepositoryIni.ToLower().EndsWith("mogrepository.ini"))
			{
				mogRepositoryIni += "\\MogRepository.ini";
			}

			if (!File.Exists(mogRepositoryIni))
				return;

			MOG_Ini ini = new MOG_Ini(mogRepositoryIni);
			if (!ini.SectionExist("MOG_REPOSITORIES"))
			{
				ini.Close();
				return;
			}

			// we know we have a valid MogRepository.ini, so clear the repository view nodes
			this.tvReposView.BeginUpdate();
			this.tvReposView.Nodes.Clear();

			rTreeNode rootReposNode = new rTreeNode("Repositories on " + Path.GetDirectoryName(mogRepositoryIni), Path.GetDirectoryName(mogRepositoryIni), this.IMGINDEX_MOGDRIVE);
			this.tvReposView.Nodes.Add(rootReposNode);

			// build it
			for (int i = 0; i < ini.CountKeys("MOG_REPOSITORIES"); i++)
			{
				string sectionName = ini.GetKeyNameByIndex("MOG_REPOSITORIES", i);
				if (ini.SectionExist(sectionName))
				{
					if (ini.KeyExist(sectionName, "SystemRepositoryPath"))
					{
						string reposPath = ini.GetString(sectionName, "SystemRepositoryPath").Trim("\\".ToCharArray());
						if (Directory.Exists(reposPath + "\\Tools"))
						{
							// create a node for this repository
							rTreeNode reposNode = new rTreeNode(reposPath, reposPath, this.IMGINDEX_MOGFOLDER);
							rootReposNode.Nodes.Add(reposNode);

							foreach (string iniFilename in Directory.GetFiles(reposPath + "\\Tools", "*.ini"))
							{
								rTreeNode iniNode = new rTreeNode(Path.GetFileName(iniFilename), iniFilename, this.IMGINDEX_INIFILE);
								iniNode.IsINI = true;
								reposNode.Nodes.Add(iniNode);
							}
						}
					}
				}
			}

			rootReposNode.Expand();
			ini.Close();
			this.tvReposView.EndUpdate();
		}
		#endregion

		#endregion
		#region Event handlers

		private void btnPlaceRepository_Click(object sender, System.EventArgs e)
		{
			if (this.tvDiskTree.SelectedNode == null)
				return;

			rTreeNode rtn = this.tvDiskTree.SelectedNode as rTreeNode;
			if (rtn == null  ||  !Directory.Exists(rtn.DiskPath))
				return;

			if (PlaceRepository(rtn.DiskPath))
			{
				// fixup the icons
				if (rtn.IsFolder)
				{
					rtn.ImageIndex = this.IMGINDEX_MOGFOLDER;
					rtn.SelectedImageIndex = this.IMGINDEX_MOGFOLDER;
				}
				else if (rtn.IsDrive)
				{
					rtn.ImageIndex = this.IMGINDEX_MOGDRIVE;
					rtn.SelectedImageIndex = this.IMGINDEX_MOGDRIVE;
				}

				// fix drive root
				rTreeNode driveRoot = GetRootNodeMinusOne(rtn);
				if (driveRoot != null  &&  driveRoot.IsDrive)
				{
					driveRoot.ImageIndex = this.IMGINDEX_MOGDRIVE;
					driveRoot.SelectedImageIndex = this.IMGINDEX_MOGDRIVE;
				}

				// fix ok button
				if (this.forceRepositorySelection)
					this.btnOK.Enabled = SelectionValidRepository();

				// fix node
				rTreeNode rParent = rtn.Parent as rTreeNode;
				if (rParent != null)
				{
					rParent.Populated = false;
					PopulateNode(rParent);
				}

				// load up repository view
				if (this.tvReposView.Visible)
					BuildRepositoryTreeView(rtn.DiskPath);
			}

		}

		private void tvDiskTree_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			rTreeNode rtn = this.tvDiskTree.GetNodeAt(e.X, e.Y) as rTreeNode;
			this.tvDiskTree.SelectedNode = rtn;

			if (rtn != null)
			{
				this.selectedPath = rtn.DiskPath;
				this.lblSelectedPath.Text = rtn.DiskPath;
			}
			else
			{
				this.lblSelectedPath.Text = "";
				this.selectedPath = "";
			}

            bool validRepository = SelectionValidRepository();

			// do we need to manage the OK button?
			if (this.forceRepositorySelection)
			{
				this.btnOK.Enabled = validRepository;
			}

			// do we need to populate the repository view?
			if (this.tvReposView.Visible)
			{
				if (rtn == null  ||  rtn == this.myComputerNode)
				{
					this.tvReposView.Nodes.Clear();
				}
				else if (validRepository)
				{
					//this.tvReposView.Enabled = true;
					BuildRepositoryTreeView(rtn.DiskPath);
				}
				else
				{
					//this.tvReposView.Enabled = false;
					this.tvReposView.Nodes.Clear();
					this.tvReposView.Nodes.Add(new rTreeNode("No valid MOG Repositories", "", this.IMGINDEX_BROKENDRIVE));
				}
			}
		}


		private void miRefreshDiskTree_Click(object sender, System.EventArgs e)
		{
			RefreshDiskTree();
		}

		private void tvDiskTree_BeforeExpand(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		{
			rTreeNode rtn = e.Node as rTreeNode;
			if (rtn == null)
				return;

			if (!rtn.Populated)
			{
				PopulateNode(rtn);
			}
		}

		private void miShowRepositoryInfo_Click(object sender, System.EventArgs e)
		{
			this.miShowRepositoryInfo.Checked = !this.miShowRepositoryInfo.Checked;
			this.SetRepositoryViewVisible(this.miShowRepositoryInfo.Checked);
		}


		private void miEditIni_Click(object sender, System.EventArgs e)
		{
			if (this.tvReposView.SelectedNode == null)
				return;

			rTreeNode rtn = this.tvReposView.SelectedNode as rTreeNode;
			if (rtn == null)
				return;

			Process p = new Process();
			p.StartInfo = new ProcessStartInfo("notepad.exe", rtn.DiskPath);
			p.Start();
		}


		private void cmRepositoryView_Popup(object sender, System.EventArgs e)
		{
			// setup the Edit INI menu item
			if (this.tvReposView.SelectedNode == null)
			{
				this.miEditIni.Enabled = false;
				return;
			}

			rTreeNode rtn = this.tvReposView.SelectedNode as rTreeNode;
			if (rtn == null)
			{
				this.miEditIni.Enabled = false;
				return;
			}

			if (!File.Exists(rtn.DiskPath)  ||  !rtn.IsINI)
			{
				this.miEditIni.Enabled = false;
				return;
			}

			this.miEditIni.Enabled = true;
		}

		private void tvReposView_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			rTreeNode rtn = this.tvReposView.GetNodeAt(e.X, e.Y) as rTreeNode;
			this.tvReposView.SelectedNode = rtn;
		}

		private void tvReposView_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyData == Keys.F5)
				RefreshDiskTree();
		}

		private void miPlaceRepository_Click(object sender, System.EventArgs e)
		{
			this.btnPlaceRepository.PerformClick();		
		}

		private void cmDiskView_Popup(object sender, System.EventArgs e)
		{
			this.miCreateFolder.Enabled = false;
			this.miRemoveFolder.Enabled = false;
			this.miPlaceRepository.Enabled = false;

			if (this.tvDiskTree.SelectedNode != null)
			{
				rTreeNode rtn = this.tvDiskTree.SelectedNode as rTreeNode;
				if (rtn == null)
					return;

				if (rtn.IsFolder)
				{
					this.miCreateFolder.Enabled = true;
					this.miRemoveFolder.Enabled = true;
					this.miPlaceRepository.Enabled = true;
				}
				if (rtn.IsDrive)
				{
					this.miCreateFolder.Enabled = true;
					this.miPlaceRepository.Enabled = true;
				}
			}
		}

		private void miCreateFolder_Click(object sender, System.EventArgs e)
		{
			if (this.tvDiskTree.SelectedNode == null)
			{
				return;
			}

			rTreeNode rtn = this.tvDiskTree.SelectedNode as rTreeNode;
			if (rtn == null  ||  (!rtn.IsFolder && !rtn.IsDrive))
			{
				return;
			}
			
			if (!rtn.Populated)
			{
				PopulateNode(rtn);
			}

			rTreeNode newDirNode = new rTreeNode("NewFolder", rtn.DiskPath, this.IMGINDEX_FOLDER);
			rtn.Nodes.Add(newDirNode);
			this.tvDiskTree.LabelEdit = true;
			newDirNode.EnsureVisible();
			newDirNode.BeginEdit();
		}

		private void miRemoveFolder_Click(object sender, System.EventArgs e)
		{
			if (this.tvDiskTree.SelectedNode == null)
				return;

			rTreeNode rtn = this.tvDiskTree.SelectedNode as rTreeNode;
			if (rtn == null  ||  !rtn.IsFolder)
				return;

			if (MessageBox.Show("WARNING: You are about to delete " + rtn.DiskPath + " and all its sub-folders and files.\nAre you sure you want to proceed?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
			{
				try
				{
					Directory.Delete(rtn.DiskPath, true);
					
					// rebuild rtn's parent in the tree view
					rTreeNode parent = rtn.Parent as rTreeNode;
					if (parent == null)
						return;

					parent.Populated = false;
					PopulateNode(parent);
				}
				catch
				{
					MessageBox.Show("Couldn't delete " + rtn.DiskPath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
			}
		}

		private void tvDiskTree_AfterLabelEdit(object sender, System.Windows.Forms.NodeLabelEditEventArgs e)
		{
			rTreeNode rtn = e.Node as rTreeNode;
			if (rtn == null)
			{
				e.CancelEdit = true;
				return;
			}

			string label = e.Label;
			if (label == null)
			{
				label = rtn.Text;
			}

			string path = rtn.DiskPath.Trim("\\".ToCharArray()) + "\\" + label;
			try
			{
				if (Directory.Exists(path))
				{
					throw new Exception();
				}

				if (!Directory.CreateDirectory(path).Exists)
				{
					throw new Exception();
				}

				rTreeNode parent = rtn.Parent as rTreeNode;
				rtn.EnsureVisible();
				rtn.DiskPath = path;

				// Set the selected node
				this.tvDiskTree.SelectedNode = rtn;
				this.selectedPath = rtn.DiskPath;
				this.lblSelectedPath.Text = rtn.DiskPath;
			}
			catch
			{
				MessageBox.Show("Couldn't create " + path, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				e.CancelEdit = true;
				rtn.Remove();
			}
			finally
			{
				this.tvDiskTree.LabelEdit = false;
			}
		}

		private void miClean_Click(object sender, System.EventArgs e)
		{
			if (this.tvDiskTree.SelectedNode == null)
				return;

			rTreeNode rtn = this.tvDiskTree.SelectedNode as rTreeNode;
			if (rtn == null  ||  (!rtn.IsFolder && !rtn.IsDrive))
				return;

			if (MessageBox.Show("Are you sure you want to clean " + rtn.DiskPath, "Confirm Clean", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
				return;

			string path = rtn.DiskPath.Trim("\\".ToCharArray()) + "\\MogRepository.ini";
			if (File.Exists(path))
			{
				try
				{
					File.Delete(path);
				}
				catch
				{
					MessageBox.Show("Couldn't clean " + rtn.DiskPath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
			}

			MessageBox.Show("Cleaned " + rtn.DiskPath, "Clean Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
			
			// fixup icon
			if (rtn.IsFolder)
			{
				rtn.ImageIndex = this.IMGINDEX_FOLDER;
				rtn.SelectedImageIndex = this.IMGINDEX_FOLDER;
			}
			else if (rtn.IsDrive)
			{
				rtn.ImageIndex = this.IMGINDEX_DISKDRIVE;
				rtn.SelectedImageIndex = this.IMGINDEX_DISKDRIVE;
			}
		}
		#endregion

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			Hide();
		}
	}

	#region Misc
	class rTreeNode : TreeNode
	{
		public string DiskPath = "";
		public bool Populated = false;
		
		public bool IsINI = false;
		public bool IsDrive = false;
		public bool IsFolder = false;

		#region Constructors
		public rTreeNode(string text) :base(text)
		{
		}

		public rTreeNode(string text, string diskPath) :base(text)
		{
			this.DiskPath = diskPath;
		}

		public rTreeNode(string text, int imageIndex) :base(text)
		{
			this.ImageIndex = imageIndex;
			this.SelectedImageIndex = imageIndex;
		}

		public rTreeNode(string text, string diskPath, int imageIndex) :base(text)
		{
			this.DiskPath = diskPath;
			this.ImageIndex = imageIndex;
			this.SelectedImageIndex = imageIndex;
		}
		#endregion
	}
	#endregion
}
