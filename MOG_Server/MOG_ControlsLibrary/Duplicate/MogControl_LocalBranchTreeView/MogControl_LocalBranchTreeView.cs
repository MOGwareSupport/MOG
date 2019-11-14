using System;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using MOG.REPORT;
using MOG.PROMPT;
using MOG.PROGRESS;
using MOG.FILENAME;
using MOG.DOSUTILS;
using MOG.DATABASE;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERSYNCDATA;

using Etier.IconHelper;
using Iesi.Collections;
using MOG_CoreControls;

namespace MOG_Server.MOG_ControlsLibrary.Common
{
	/// <summary>
	/// Summary description for MogControl_LocalBranchTreeView.
	/// </summary>
	public class MogControl_LocalBranchTreeView : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.TreeView GameDataTreeView;
		private bool mShowFiles = false;
		private IconListManager mWindowsExplorerIcons;
		private ImageList mWindowsExplorerIconList;
		private bool mAllowDirectoryOpperations = true;
		private System.Windows.Forms.ImageList DefaultImageList;
		private System.Windows.Forms.ContextMenu LocalDirectoryCommandsContextMenu;
		private System.Windows.Forms.MenuItem LocalDirectoryCreateMenuItem;
		private System.Windows.Forms.MenuItem LocalDirectoryDeleteMenuItem;
		private System.Windows.Forms.MenuItem LocalDirectoryRenameMenuItem;
		private System.Windows.Forms.MenuItem LocalDirectorySep1MenuItem;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem LocalDirectoryRefreshMenuItem;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.ComponentModel.IContainer components;

		public delegate void TreeViewEvent(object sender, System.Windows.Forms.TreeViewEventArgs e);
		public delegate void TreeViewClickEvent(object sender, System.EventArgs e);
		
		[Category("Behavior"), Description("Occures when selection has been changed")]
		public event TreeViewEvent MOGAfterTargetSelect;

		[Category("Behavior"), Description("Occures after click event")]
		public event TreeViewClickEvent MOGTreeClick;

		#region Getters and setters
		public string MOGGameDataTarget
		{
			get 
			{ 
				try
				{
					guiAssetTreeTag tag = (guiAssetTreeTag)GameDataTreeView.SelectedNode.Tag;
					return tag.FullFilename;
				}
				catch
				{
					return "";
				}
			}
		}

		public TreeNode MOGSelectedNode
		{
			get { return GameDataTreeView.SelectedNode; }
		}

		public MOG_ControllerSyncData MOGCurrentGameDataNode
		{
			get 
			{ 
				try
				{
					guiAssetTreeTag tag = (guiAssetTreeTag)GameDataTreeView.SelectedNode.Tag;
					return (MOG_ControllerSyncData)tag.Object;
				}
				catch
				{
					return null;
				}
			}
		}

		public bool MOGShowFiles
		{
			get { return mShowFiles; }
			set { mShowFiles = value; }
		}

		public ContextMenu MogContextMenu
		{
			get { return GameDataTreeView.ContextMenu; }
			set 
			{
				GameDataTreeView.ContextMenu = new ContextMenu();
				GameDataTreeView.ContextMenu.MergeMenu(value);
				
				if (MOGAllowDirectoryOpperations)
				{
					GameDataTreeView.ContextMenu.MergeMenu(this.LocalDirectoryCommandsContextMenu);
					GameDataTreeView.ContextMenu.Popup +=new EventHandler(this.LocalDirectoryCommandsContextMenu_Popup);
				}
			}
		}

		public bool MOGAllowDirectoryOpperations
		{
			get { return mAllowDirectoryOpperations; }
			set { mAllowDirectoryOpperations = value; }
		}
		#endregion

		public MogControl_LocalBranchTreeView()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			
			try
			{
				// Load the folder icon
				mWindowsExplorerIconList = new ImageList();
				mWindowsExplorerIconList.ColorDepth = ColorDepth.Depth32Bit;
				mWindowsExplorerIconList.ImageSize = new System.Drawing.Size( 16, 16 );
			
				foreach (Image icon in this.DefaultImageList.Images)
				{
					mWindowsExplorerIconList.Images.Add(icon);
				}

				mWindowsExplorerIcons = new IconListManager(mWindowsExplorerIconList, Etier.IconHelper.IconReader.IconSize.Small);

				GameDataTreeView.ImageList = mWindowsExplorerIconList;
			}
			catch
			{
			}
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
        
		/// <summary>
		/// Initialize the Explorer treeView
		/// </summary>
		public void Initialize()
		{
			GameDataTreeView.BeginUpdate();
			GameDataTreeView.Nodes.Clear();
					
			// Create MOG projects root
			PopulateMogProjects();
			
			// Get all local drives available on this computer
			PopulateLocalDrives(DRIVE_TYPE_HD);
			
			GameDataTreeView.EndUpdate();
		}

		/// <summary>
		/// Initialize the Explorer treeView
		/// </summary>
		public void InitializeDrivesOnly()
		{
			GameDataTreeView.BeginUpdate();
			GameDataTreeView.Nodes.Clear();
					
			// Get all local drives available on this computer
			PopulateLocalDrives(DRIVE_TYPE_HD);
			
			GameDataTreeView.EndUpdate();
		}

		/// <summary>
		/// Initialize the Explorer treeView
		/// </summary>
		public void InitializeNetworkDrivesOnly()
		{
			GameDataTreeView.BeginUpdate();
			GameDataTreeView.Nodes.Clear();
					
			// Get all local drives available on this computer
			PopulateLocalDrives(DRIVE_TYPE_NETWORK);
			
			GameDataTreeView.EndUpdate();
		}

		/// <summary>
		/// Populate all the mog aware directories within the 'MyProjects' node
		/// </summary>
		private void PopulateMogProjects()
		{
			TreeNode mogProjects = new TreeNode("MyProjects", 1, 1);
			GameDataTreeView.Nodes.Add(mogProjects);
			
			string userName = MOG_ControllerProject.GetUserName_DefaultAdmin();

			// Get a list of all our projects that the server knows about
			ArrayList projectsArray = MOG_DBSyncedDataAPI.GetAllSyncedLocations(MOG_ControllerSystem.GetComputerName(), null, userName);
			if (projectsArray != null && projectsArray.Count > 0)
			{
				foreach (MOG_DBSyncedLocationInfo project in projectsArray)
				{
					// Hash our local tab name						
					string LocalBranchName ="@" + project.mWorkingDirectory;

					MOG_ControllerSyncData targetGameData = new MOG_ControllerSyncData(project.mWorkingDirectory, project.mProjectName, project.mBranchName, project.mPlatformName, userName);

					TreeNode projectNode = new TreeNode(LocalBranchName, 4, 4);
					projectNode.Tag = new guiAssetTreeTag(project.mWorkingDirectory, projectNode, targetGameData);

					FillDirectory(projectNode, project.mWorkingDirectory, targetGameData);

					mogProjects.Nodes.Add(projectNode);
				}
			}

			// Create the helper project
			mogProjects.Nodes.Add("<Create new local branch>");

			mogProjects.Expand();
		}

		/// <summary>
		/// Create all the treeNodes corresponding with the users local drives
		/// </summary>
		private void PopulateLocalDrives(int driveTypesToInclude)
		{
			// Create computer root
			TreeNode computer = new TreeNode("MyComputer", 0, 0);
			GameDataTreeView.Nodes.Add(computer);

			foreach (string drive in Directory.GetLogicalDrives())
			{
				// Only add local physical hard drives to our tree
				int type = (int)GetDriveType(drive);
				if (type == driveTypesToInclude)
				{
					if (Directory.Exists(drive))
					{
						TreeNode driveNode = new TreeNode(drive, 2, 2);
						driveNode.Tag = new guiAssetTreeTag(drive, driveNode);
						computer.Nodes.Add(driveNode);

						FillDirectory(driveNode, drive, null);
					}
				}
			}

			computer.Expand();
		}

		public void MOGCreateGhostedTree(MOG_ControllerSyncData gameDataHandle, TreeNode parent, ArrayList ArrayOfMOG_Filenames)
		{
			TreeNode lastCreatedNode = null;
			SortedSet directories = new SortedSet();
			foreach (string asset in ArrayOfMOG_Filenames)
			{
				string targetFile = gameDataHandle.GetSyncDirectory() + "\\" + asset;
				if (directories.Add(Path.GetDirectoryName(asset)))
				{
					lastCreatedNode = CreateTreePath(gameDataHandle, targetFile);
					parent.Nodes.Add(lastCreatedNode);	
				}
			}
		}

		private TreeNode CreateTreePath(MOG_ControllerSyncData gameDataHandle, string sourcePath)
		{
			TreeNode node = this.CreateDirectoryNode(sourcePath, null);
			node.ImageIndex = 5;
			return node;
		}

		/// <summary>
		/// Set the back color of the node that matches the gameDatPath that was passed in
		/// </summary>
		/// <param name="gameDataPath"></param>
		public void MOGHighLightGameDataTarget(string gameDataPath)
		{
			TreeNode selected = FindNode(null, gameDataPath);
			
			if (selected != null)
			{
				selected.BackColor = System.Drawing.SystemColors.ControlDark;
				GameDataTreeView.SelectedNode = selected;
			}
		}

		private void SetStartingPath(string path)
		{
			TreeNode selected = FindNodeDeep(null, path);
			
			if (selected != null)
			{
				selected.BackColor = System.Drawing.SystemColors.ControlDark;
				GameDataTreeView.SelectedNode = selected;
			}
		}

		/// <summary>
		/// Find a node within a nodes children based on its Text value
		/// </summary>
		/// <param name="parentNode"></param>
		/// <param name="title"></param>
		/// <returns></returns>
		private TreeNode FindNode(TreeNode parentNode, string title)
		{
			try
			{
				if (parentNode == null)
				{
					if (this.GameDataTreeView.Nodes != null && this.GameDataTreeView.Nodes.Count > 0)
					{
						parentNode = this.GameDataTreeView.Nodes[0];
					}
				}

				if (parentNode != null)
				{
					foreach (TreeNode node in parentNode.Nodes)
					{
						guiAssetTreeTag tag = (guiAssetTreeTag)node.Tag;
						if (string.Compare(tag.FullFilename, title, true) == 0)
						{
							return node;
						}
					}
				}
			}
			catch
			{
			}

			return null;
		}

		/// <summary>
		/// Search for the text given among all the children of the passed in treeNode
		/// </summary>
		/// <param name="parentNode"></param>
		/// <param name="title"></param>
		/// <returns></returns>
		private TreeNode FindNodeDeep(TreeNode parentNode, string title)
		{
			if (parentNode == null)
			{
				if (this.GameDataTreeView.Nodes != null && this.GameDataTreeView.Nodes.Count > 0)
				{
					parentNode = this.GameDataTreeView.Nodes[0];
				}
			}
			
				// Walk all these nodes
			foreach (TreeNode node in parentNode.Nodes)
			{
				// Is this node it?
				if (string.Compare(node.FullPath, title, true) == 0)
				{
					return node;
				}
					
				if (node.Nodes.Count == 1 && node.Nodes[0].Text == "BLANK")
				{						
					TreeViewCancelEventArgs eventArg = new TreeViewCancelEventArgs(node, false, TreeViewAction.Expand);
					this.GameDataTreeView_BeforeExpand(GameDataTreeView, eventArg);
						

					// If we found it, return it
					TreeNode found = null;
					found = FindNodeDeep(node, title);
					if (found != null)
					{
						return found;
					}
				}
					// If this child has children, are one of them it?
				else if (node.Nodes.Count > 0)
				{
					// If we found it, return it
					TreeNode found = null;
					found = FindNodeDeep(node, title);
					if (found != null)
					{
						return found;
					}
				}
				else
				{
					if (string.Compare(node.FullPath, title, true) == 0)
					{
						return node;
					}
				}
			}
			
			return null;
		}

		private ArrayList GetAllProjectSyncTargetFileForDirectory(MOG_ControllerSyncData targetGameData, string path)
		{
			try
			{
				ArrayList fileList = MOG_DBAssetAPI.GetAllProjectSyncTargetFilesForDirectory(targetGameData.GetSyncDirectory(), path, targetGameData.GetPlatformName());

				string targetDir = path.ToLower().Replace(targetGameData.GetSyncDirectory().ToLower(), "");
				int depth = DosUtils.DirectoryDepth(targetDir);
				
				SortedSet directories = new SortedSet();
				foreach (string file in fileList)
				{	
					if (targetDir.Length == 0)
					{
						directories.Add(targetGameData.GetSyncDirectory() + "\\" + DosUtils.DirectoryGetAtDepth(file, depth));
					}
					else if (file.ToLower().IndexOf(targetDir.ToLower()) != -1)
					{
						directories.Add(targetGameData.GetSyncDirectory() + "\\" + DosUtils.DirectoryGetAtDepth(file, depth));
					}
					else
					{
						directories.Add(targetGameData.GetSyncDirectory() + "\\" + DosUtils.DirectoryGetAtDepth(file, depth));
					}
				}

				ArrayList finalFileList = new ArrayList();
				foreach (string dir in directories)
				{					
					finalFileList.Add(dir);					
				}

				return finalFileList;
			}
			catch
			{
				return new ArrayList();
			}
		}
	
		/// <summary>
		/// Fill the current parent node with all the files and directories found in the directory name provided
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="directoryName"></param>
		private void FillDirectory(TreeNode parent, string directoryName, MOG_ControllerSyncData targetGameData)
		{
			// Clear out our temp child
			if (parent != null && (parent.Nodes.Count == 1 && parent.Nodes[0].Text == "BLANK"))
			{
				parent.Nodes.Clear();
			}

			try
			{
				// Get a valid direcotry
				DirectoryInfo dir = new DirectoryInfo(directoryName);

				// Make sure it exits
				if (!dir.Exists)
//				if (!dir.Exists || dir.GetDirectories().Length < GetAllProjectSyncTargetFileForDirectory(targetGameData, directoryName).Count)
//				{
//					// Maybe its a virtual drive
//					if (targetGameData != null)
//					{
//						// Get virtual directories
//						ArrayList virtualDirectories = GetAllProjectSyncTargetFileForDirectory(targetGameData, directoryName); 
//					
//						// Add each found directory to the tree
//						foreach(string virtualDirectory in virtualDirectories)
//						{
//							// Create the node
//							TreeNode child = CreateDirectoryNode(virtualDirectory, targetGameData);
//					
//							// Check if the child has children
//							//						if (di.GetDirectories().Length != 0 || (di.GetFiles().Length != 0 && MOGShowFiles))
//						{
//							// If so, add a temp child
//							child.Nodes.Add("BLANK");
//						}
//
//							// If we have a parent add this child
//							if (parent != null)
//							{
//								parent.Nodes.Add(child);
//							}
//							else
//							{
//								// Else add it to the master tree
//								GameDataTreeView.Nodes.Add(child);
//							}
//						}
//					}
//					else
					{
						throw new DirectoryNotFoundException ("directory does not exist:" + directoryName);
					}
//				}

				// Add each found directory to the tree
				foreach(DirectoryInfo di in dir.GetDirectories())
				{
					// Create the node
					TreeNode child = CreateDirectoryNode(di.FullName, null);
					
					// Check if the child has children
					if (di.GetDirectories().Length != 0 || (di.GetFiles().Length != 0 && MOGShowFiles))
					{
						// If so, add a temp child
						child.Nodes.Add("BLANK");
					}

					// If we have a parent add this child
					if (parent != null)
					{
						parent.Nodes.Add(child);
					}
					else
					{
						// Else add it to the master tree
						GameDataTreeView.Nodes.Add(child);
					}
				}

				// Now get the files
				if (MOGShowFiles)
				{
					foreach(FileInfo fi in dir.GetFiles())
					{
						TreeNode child = new TreeNode(fi.Name, mWindowsExplorerIcons.AddFileIcon( fi.FullName ), mWindowsExplorerIcons.AddFileIcon( fi.FullName ));
						child.Tag = new guiAssetTreeTag(fi.FullName, child);
 
						// If we have a parent add this child
						if (parent != null)
						{
							parent.Nodes.Add(child);
						}
						else
						{
							// Else add it to the master tree
							GameDataTreeView.Nodes.Add(child);
						}
					}
				}
			}
			catch(Exception ex)
			{
				ex.ToString();
			}
		}

		/// <summary>
		/// Create a Directory TreeNode checking to see if this directory is a MOG sync target
		/// </summary>
		/// <param name="fullname"></param>
		/// <returns></returns>
		private TreeNode CreateDirectoryNode(string fullname, MOG_ControllerSyncData targetGameData)
		{
			string name = fullname;

			try
			{
				name = Path.GetFileName(fullname);
			}
			catch
			{
			}
			//Check active user for null/empty string and default to Admin if that is the case.
			string userName = MOG_ControllerProject.GetUserName_DefaultAdmin();

			TreeNode directory = new TreeNode(name);
			directory.Tag = new guiAssetTreeTag(fullname, directory);

			// This is a virtual directory
			if (targetGameData != null)
			{
				directory.ImageIndex = 5;
				directory.SelectedImageIndex = 5;
				directory.Tag = new guiAssetTreeTag(fullname, directory, targetGameData);
			}
			else
			{                			
				// Is this a MOG controlled directory - Always remove ' characters when talking to the database?
				if (MOG.DATABASE.MOG_DBSyncedDataAPI.DoesSyncedLocationExist(MOG_ControllerSystem.GetComputerName(), null, null, fullname.Replace("'",""),userName))
				{
					ArrayList gameDataArray = MOG.DATABASE.MOG_DBSyncedDataAPI.GetAllSyncedLocations(MOG_ControllerSystem.GetComputerName(),fullname.Replace("'",""),userName);

					if (gameDataArray.Count == 1)
					{
						MOG_DBSyncedLocationInfo dbLocation = (MOG_DBSyncedLocationInfo)gameDataArray[0];
						MOG_ControllerSyncData gameData = new MOG_ControllerSyncData(dbLocation.mWorkingDirectory, dbLocation.mProjectName, dbLocation.mBranchName , dbLocation.mPlatformName, userName);
						directory.Tag = new guiAssetTreeTag(fullname, directory, gameData);
					}
					else if (gameDataArray.Count > 1)
					{
						MOG_Report.ReportMessage("Create Mog Aware directory", "Got back too many gameDataControllers for this directory!\n\nDIRECTORY: " + fullname + "\n\nTaking first one...", Environment.StackTrace, MOG_ALERT_LEVEL.ERROR);
						MOG_DBSyncedLocationInfo dbLocation = (MOG_DBSyncedLocationInfo)gameDataArray[0];
						MOG_ControllerSyncData gameData = new MOG_ControllerSyncData(dbLocation.mWorkingDirectory, dbLocation.mProjectName, dbLocation.mBranchName , dbLocation.mPlatformName, userName);
						directory.Tag = new guiAssetTreeTag(fullname, directory, gameData);
					}
					else if (gameDataArray.Count == 0)
					{
						MOG_Report.ReportMessage("Create Mog Aware directory", "Got back zero gameDataControllers for this directory!\n\nDIRECTORY: " + fullname, Environment.StackTrace, MOG_ALERT_LEVEL.ERROR);
						return directory;
					}
				
					directory.ImageIndex = 4;
					directory.SelectedImageIndex = 4;				
				}
				else
				{
					directory.ImageIndex = 3;
					directory.SelectedImageIndex = 3;
				}
			}

			return directory;
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MogControl_LocalBranchTreeView));
			this.GameDataTreeView = new System.Windows.Forms.TreeView();
			this.LocalDirectoryCommandsContextMenu = new System.Windows.Forms.ContextMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.LocalDirectoryRefreshMenuItem = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.LocalDirectoryCreateMenuItem = new System.Windows.Forms.MenuItem();
			this.LocalDirectoryRenameMenuItem = new System.Windows.Forms.MenuItem();
			this.LocalDirectorySep1MenuItem = new System.Windows.Forms.MenuItem();
			this.LocalDirectoryDeleteMenuItem = new System.Windows.Forms.MenuItem();
			this.DefaultImageList = new System.Windows.Forms.ImageList(this.components);
			this.SuspendLayout();
			// 
			// GameDataTreeView
			// 
			this.GameDataTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.GameDataTreeView.ContextMenu = this.LocalDirectoryCommandsContextMenu;
			this.GameDataTreeView.ImageList = this.DefaultImageList;
			this.GameDataTreeView.Indent = 19;
			this.GameDataTreeView.Location = new System.Drawing.Point(0, 0);
			this.GameDataTreeView.Name = "GameDataTreeView";
			this.GameDataTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
																						 new System.Windows.Forms.TreeNode("Node1", 4, 0)});
			this.GameDataTreeView.Size = new System.Drawing.Size(152, 84);
			this.GameDataTreeView.TabIndex = 0;
			this.GameDataTreeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GameDataTreeView_MouseDown);
			this.GameDataTreeView.Click += new System.EventHandler(this.GameDataTreeView_Click);
			this.GameDataTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.GameDataTreeView_AfterSelect);
			this.GameDataTreeView.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.GameDataTreeView_BeforeSelect);
			this.GameDataTreeView.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.GameDataTreeView_AfterLabelEdit);
			this.GameDataTreeView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.GameDataTreeView_BeforeExpand);
			// 
			// LocalDirectoryCommandsContextMenu
			// 
			this.LocalDirectoryCommandsContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																											  this.menuItem1,
																											  this.LocalDirectoryRefreshMenuItem,
																											  this.menuItem2,
																											  this.LocalDirectoryCreateMenuItem,
																											  this.LocalDirectoryRenameMenuItem,
																											  this.LocalDirectorySep1MenuItem,
																											  this.LocalDirectoryDeleteMenuItem});
			this.LocalDirectoryCommandsContextMenu.Popup += new System.EventHandler(this.LocalDirectoryCommandsContextMenu_Popup);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.Text = "-";
			// 
			// LocalDirectoryRefreshMenuItem
			// 
			this.LocalDirectoryRefreshMenuItem.Index = 1;
			this.LocalDirectoryRefreshMenuItem.Text = "Refresh";
			this.LocalDirectoryRefreshMenuItem.Click += new System.EventHandler(this.LocalDirectoryRefreshMenuItem_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 2;
			this.menuItem2.Text = "-";
			// 
			// LocalDirectoryCreateMenuItem
			// 
			this.LocalDirectoryCreateMenuItem.Index = 3;
			this.LocalDirectoryCreateMenuItem.Text = "Create Directory";
			this.LocalDirectoryCreateMenuItem.Click += new System.EventHandler(this.LocalDirectoryCreateMenuItem_Click);
			// 
			// LocalDirectoryRenameMenuItem
			// 
			this.LocalDirectoryRenameMenuItem.Index = 4;
			this.LocalDirectoryRenameMenuItem.Text = "Rename Directory";
			this.LocalDirectoryRenameMenuItem.Click += new System.EventHandler(this.LocalDirectoryRenameMenuItem_Click);
			// 
			// LocalDirectorySep1MenuItem
			// 
			this.LocalDirectorySep1MenuItem.Index = 5;
			this.LocalDirectorySep1MenuItem.Text = "-";
			// 
			// LocalDirectoryDeleteMenuItem
			// 
			this.LocalDirectoryDeleteMenuItem.Index = 6;
			this.LocalDirectoryDeleteMenuItem.Text = "Delete Directory";
			this.LocalDirectoryDeleteMenuItem.Click += new System.EventHandler(this.LocalDirectoryDeleteMenuItem_Click);
			// 
			// DefaultImageList
			// 
			this.DefaultImageList.ImageSize = new System.Drawing.Size(16, 16);
			this.DefaultImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("DefaultImageList.ImageStream")));
			this.DefaultImageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// MogControl_LocalBranchTreeView
			// 
			this.Controls.Add(this.GameDataTreeView);
			this.Name = "MogControl_LocalBranchTreeView";
			this.Size = new System.Drawing.Size(150, 90);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Go to the drive and get all the files and directories directly under it
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void GameDataTreeView_BeforeExpand(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		{
			// Get our initial root path from our treeView Tag
		//	string rootPath = (string)e.Node.TreeView.Tag;

			if (e.Node.Nodes[0].Text == "BLANK")
			{
				try
				{
					guiAssetTreeTag dirTag = (guiAssetTreeTag)e.Node.Tag;

					MOG_ControllerSyncData targetGamdeData = null;

					// Check if this directory has a targetGamdeData
					if (dirTag.Object != null)
					{
						targetGamdeData = (MOG_ControllerSyncData)dirTag.Object;
					}

					if (Path.IsPathRooted(dirTag.FullFilename))
					{
						FillDirectory(e.Node, dirTag.FullFilename, targetGamdeData);
					}
				}
				catch
				{
				}
			}
		}

		/// <summary>
		/// Events after the user selects a GameDataPath target treeNode
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void GameDataTreeView_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			// Inform any delegates of this event
			if (MOGAfterTargetSelect != null)
			{
				object []args = {sender, e};
				this.Invoke(MOGAfterTargetSelect, args);
			}

			// Highlight the selected node
			if (GameDataTreeView.SelectedNode != null)
			{
				GameDataTreeView.SelectedNode.BackColor = System.Drawing.SystemColors.ControlDark;
			}
		}

		/// <summary>
		/// Clear previous selected node colors
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void GameDataTreeView_BeforeSelect(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		{
			if (GameDataTreeView.SelectedNode != null)
			{
				GameDataTreeView.SelectedNode.BackColor = System.Drawing.SystemColors.Window;
			}
		}

		/// <summary>
		/// Create directory
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LocalDirectoryCreateMenuItem_Click(object sender, System.EventArgs e)
		{
			try
			{
				if (Directory.CreateDirectory(this.MOGGameDataTarget + "NewDirectory") != null)
				{
					TreeNode directory = CreateDirectoryNode(this.MOGGameDataTarget + "NewDirectory", null);
					int index = GameDataTreeView.SelectedNode.Nodes.Add(directory);

					// Now edit the name of this node
					GameDataTreeView.LabelEdit = true;
					GameDataTreeView.SelectedNode = directory;
					GameDataTreeView.SelectedNode.BeginEdit();
				}
			}
			catch(Exception ex)
			{
				ex.ToString();
			}
		}

		/// <summary>
		/// Begin the rename operation for a directory
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LocalDirectoryRenameMenuItem_Click(object sender, System.EventArgs e)
		{
			try
			{
				GameDataTreeView.LabelEdit = true;
				GameDataTreeView.SelectedNode.BeginEdit();
			}
			catch
			{
			}
		}

		/// <summary>
		/// After rename, fixup the directory and update the MOG_ControllerSyncData
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void GameDataTreeView_AfterLabelEdit(object sender, System.Windows.Forms.NodeLabelEditEventArgs e)
		{
			try
			{				
				guiAssetTreeTag tag = (guiAssetTreeTag)e.Node.Tag;

				string targetDirectoryName = tag.FullFilename.Replace(e.Node.Text, e.Label);

				if (!Directory.Exists(targetDirectoryName))
				{					
					DirectoryInfo dir = new DirectoryInfo(tag.FullFilename);
					dir.MoveTo(targetDirectoryName);

					e.Node.Tag = new guiAssetTreeTag(targetDirectoryName, e.Node);

					GameDataTreeView.LabelEdit = false;
				}
				else
				{
					MOG_Report.ReportMessage("Rename Directory", "A name of that directory already exist", Environment.StackTrace, MOG_ALERT_LEVEL.ERROR);
					e.CancelEdit = true;
					return;
				}
			}
			catch
			{
			}
		}

		/// <summary>
		/// Refresh the explorer window
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LocalDirectoryRefreshMenuItem_Click(object sender, System.EventArgs e)
		{
			Initialize();
		}
		
		/// <summary>
		/// Delete directory
		/// </summary>
		private void LocalDirectoryDeleteMenuItem_Click(object sender, System.EventArgs e)
		{
			try
			{
				guiAssetTreeTag tag = (guiAssetTreeTag)GameDataTreeView.SelectedNode.Tag;
				if (MOG_Prompt.PromptResponse("Delete Directory", "Are you sure you wan to delete this directory with all its contents?\n\nDirectory:\n\n" + tag.FullFilename, MOGPromptButtons.YesNo) == MOGPromptResult.Yes)
				{
					if (tag.Object != null)
					{
						// Get our gameData handle from the item's tag
						MOG_ControllerSyncData gameDataHandle  = (MOG_ControllerSyncData)tag.Object;
						if (gameDataHandle != null)
						{
							MOG_Report.ReportMessage("Delete Directory", "Cannot delete a directory that is a MOG Local Workspace! Remove this Workspace first then try again.", Environment.StackTrace, MOG_ALERT_LEVEL.ERROR);
							return;
//							// Remove the synced location
//							if (!MOG_DBSyncedDataAPI.RemoveSyncedLocation(MOG_ControllerSystem.GetComputerName(), gameDataHandle.GetProjectName(), gameDataHandle.GetPlatformName(), gameDataHandle.GetGameDataPath()))
//							{
//								throw new Exception("Database could not remove this synced location!");
//							}
//
//							// Remove all the updated records
//							string filter = gameDataHandle.GetGameDataPath() + "\\*";
//							if (!MOG_DBInboxAPI.InboxRemoveAllAssets("Local", null, null, filter))
//							{
//								throw new Exception("Database inbox could not remove this synced location!");
//							}
						}
					}

					// Now, actually delete the directory
					ArrayList FilesToDelete = DosUtils.FileGetRecursiveList(tag.FullFilename, "*.*");
					
					ProgressDialog progress = new ProgressDialog("Delete Directory", "Deleting...", LocalDirectoryDelete_Worker, FilesToDelete, true);
					if (progress.ShowDialog() == DialogResult.OK)
					{
						// Now delete all the files left behind
						Directory.Delete(tag.FullFilename, true);

						// Remove the node
						GameDataTreeView.SelectedNode.Remove();
					}
				}
			}
			catch( Exception ex )
			{
				MOG_Report.ReportMessage("Delete Directory", "Could not delete this directory!\n\nMessage:" + ex.Message, ex.StackTrace, MOG_ALERT_LEVEL.CRITICAL);
			}
		}

		void LocalDirectoryDelete_Worker(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			ArrayList FilesToDelete = e.Argument as ArrayList;

			for (int i = 0; i < FilesToDelete.Count && !worker.CancellationPending; i++)
			{
				string file = FilesToDelete[i] as string;

				worker.ReportProgress(i * 100 / FilesToDelete.Count);

				// Delete the file
				File.Delete(file);
			}
		}

		/// <summary>
		/// Always select the node under the mouse on mouseDown
		/// </summary>
		private void GameDataTreeView_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			GameDataTreeView.SelectedNode = GameDataTreeView.GetNodeAt(e.X, e.Y);
		}

		/// <summary>
		/// Event for when the explorer contextmenu is opened.
		/// We do some validation for each of the menu items
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LocalDirectoryCommandsContextMenu_Popup(object sender, System.EventArgs e)
		{
			try
			{
				if (MOGAllowDirectoryOpperations == false)
				{
					throw new Exception("All directory opperations are disabled");
				}

				// Check to see if the selected node is a mog target
				guiAssetTreeTag tag = (guiAssetTreeTag)GameDataTreeView.SelectedNode.Tag;
				
				// All tags with objects are MOG_ControllerSyncData's
				// So is this a MOG_ControllerSyncData node?
				if (tag.Object != null)
				{
					// If it is... Disable something?
					
				}
				else
				{
					// If its not... Enable something?
					try
					{
						// Make sure this path is a valid path not a drive root
						if (Path.GetDirectoryName(tag.FullFilename).Length > 0)
						{
							this.LocalDirectoryCreateMenuItem.Enabled = true;
							this.LocalDirectoryDeleteMenuItem.Enabled = true;
							this.LocalDirectoryRenameMenuItem.Enabled = true;
						}
					}
					catch
					{
						// We do not allow delete or rename from root paths like 'c:\'
						this.LocalDirectoryDeleteMenuItem.Enabled = false;
						this.LocalDirectoryRenameMenuItem.Enabled = false;

						try
						{
							if (Path.GetDirectoryName(tag.FullFilename + "\\NewFolder").Length > 0)
							{
								this.LocalDirectoryCreateMenuItem.Enabled = true;						
							}
						}
						catch
						{
						}
					}
				}
			}
			catch
			{
				this.LocalDirectoryCreateMenuItem.Enabled = false;
				this.LocalDirectoryDeleteMenuItem.Enabled = false;
				this.LocalDirectoryRenameMenuItem.Enabled = false;
			}
		}

		private void GameDataTreeView_Click(object sender, System.EventArgs e)
		{
			// Inform any delegates of this event
			if (MOGTreeClick != null)
			{
				object []args = {sender, e};
				this.Invoke(MOGTreeClick, args);
			}
		}

	}

	#region guiAssetTreeTag
	/// <summary>
	/// Summary description for guiAssetTreeTag.
	/// </summary>
	public class guiAssetTreeTag
	{
		public enum TREE_FOCUS {BASE, KEY, CLASS, SUBCLASS, LABEL, VERSION, SUBVERSION, PACKAGE};
		
		protected TREE_FOCUS mLevel;
		protected string mFullFilename;
		protected bool mExecuteCommand;
		protected object mItemPtr;
		protected object mObj;
		
		public TREE_FOCUS Level { get{return mLevel;} set{mLevel = value;}}		// The node level within the tree
		public string FullFilename { get{return mFullFilename;} }				// Full filename to this asset
		public bool Execute { get{return mExecuteCommand;} }					// Should we allow tools to be run on this node?

		public guiAssetTreeTag(string fullname, object item, object obj)
		{
			mObj = obj;
			mFullFilename = fullname;
			mExecuteCommand = true;
			mItemPtr = item;
		}
		public guiAssetTreeTag(string fullname, object item)
		{
			mFullFilename = fullname;
			mExecuteCommand = true;
			mItemPtr = item;
		}

		public guiAssetTreeTag(string fullname, TREE_FOCUS level)
		{
			mLevel = level;
			mFullFilename = fullname;
			mExecuteCommand = false;
		}

		public guiAssetTreeTag(string fullname)
		{
			mLevel = TREE_FOCUS.KEY;
			mFullFilename = fullname;
			mExecuteCommand = false;
		}

		protected guiAssetTreeTag()
		{
		}
		
		public object Item 
		{
			get	{return mItemPtr;}
		}
		
		public object Object
		{
			get	{return mObj;}
		}
	}
	#endregion
}

