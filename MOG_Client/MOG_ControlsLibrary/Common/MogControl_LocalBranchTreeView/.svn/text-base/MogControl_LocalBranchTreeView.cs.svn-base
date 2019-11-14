using System;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using MOG.FILENAME;
using MOG.DOSUTILS;
using MOG.DATABASE;
using MOG.COMMAND;
using MOG.PROMPT;
using MOG.PROGRESS;
using MOG.REPORT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERSYNCDATA;

using MOG_ControlsLibrary;
using MOG_ControlsLibrary.Controls;
using MOG_ControlsLibrary.Utils;

using Tst;
using MOG_CoreControls;
using MOG_Client.Client_Mog_Utilities;
using MOG_Client.Client_Gui.AssetManager_Helper;

namespace MOG_ControlsLibrary.Common.MogControl_LocalBranchTreeView
{
	/// <summary>
	/// Summary description for MogControl_LocalBranchTreeView.
	/// </summary>
	public class MogControl_LocalBranchTreeView : System.Windows.Forms.UserControl
	{
		private enum TreeType {FULL, PROJECT, DRIVES};
		
		// Class variables
		private TreeType mType;
		private bool mShowFiles = false;
		private DirectorySet mSyncTargetFiles;
		private bool mAllowDirectoryOpperations = true;
		
		// Windows controls
		private System.Windows.Forms.TreeView GameDataTreeView;
		private System.Windows.Forms.ImageList DefaultImageList;
		private System.Windows.Forms.ContextMenu LocalDirectoryCommandsContextMenu;
		private System.Windows.Forms.MenuItem LocalDirectoryCreateMenuItem;
		private System.Windows.Forms.MenuItem LocalDirectoryDeleteMenuItem;
		private System.Windows.Forms.MenuItem LocalDirectoryRenameMenuItem;
		private System.Windows.Forms.MenuItem LocalDirectorySep1MenuItem;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem LocalDirectoryRefreshMenuItem;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem LocalDirectoryRebuildMenuItem;
		private MenuItem LocalDirectoryExploreMenuItem;
		private MenuItem menuItem4;
		private System.ComponentModel.IContainer components;

		// Delegates
		public delegate void TreeViewEvent(object sender, System.Windows.Forms.TreeViewEventArgs e);
		public delegate void TreeViewClickEvent(object sender, System.EventArgs e);
		[Category("Behavior"), Description("Occures when selection has been changed")]
		public event TreeViewEvent MOGAfterTargetSelect;
		[Category("Behavior"), Description("Occures after click event")]
		public event TreeViewClickEvent MOGTreeClick;
	
		/// <summary>
		/// Set the back color of the node that matches the gameDataPath that was passed in
		/// </summary>
		/// <param name="gameDataPath"></param>
		public void MOGHighLightGameDataTarget(string gameDataPath)
		{
			TreeNode selected = FindNode(null, gameDataPath);
			
			if (selected != null)
			{
				GameDataTreeView.SelectedNode = selected;
			}
		}

		private void SetStartingPath(string path)
		{
			TreeNode selected = FindNodeDeep(null, path);
			
			if (selected != null)
			{
				GameDataTreeView.SelectedNode = selected;
			}
		}

		private bool mInitialized;
		public bool Initialized
		{
			get { return mInitialized; }
		}
	
		
		#region Getters and setters
		public string MOGGameDataTarget
		{
			get 
			{
				string target = "";

				if (GameDataTreeView != null && GameDataTreeView.SelectedNode != null)
				{
					guiAssetTreeTag tag = (guiAssetTreeTag)GameDataTreeView.SelectedNode.Tag;
					if (tag != null)
					{
						target = tag.FullFilename;
					}
				}

				return target;
			}
		}

		public TreeNode MOGSelectedNode
		{
			get { return GameDataTreeView.SelectedNode; }
		}

		public bool MOGSorted
		{
			get { return GameDataTreeView.Sorted; }
			set { GameDataTreeView.Sorted = value; }
		}

		public MOG_ControllerSyncData MOGCurrentGameDataNode
		{
			get 
			{ 
				if (GameDataTreeView.SelectedNode != null)
				{
					guiAssetTreeTag tag = GameDataTreeView.SelectedNode.Tag as guiAssetTreeTag;
					if (tag != null)
					{
						return tag.Object as MOG_ControllerSyncData;
					}
				}

				return null;
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
		
		#region Component Designer generated code
	
		public MogControl_LocalBranchTreeView()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
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

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Node1", 4, 0);
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MogControl_LocalBranchTreeView));
			this.GameDataTreeView = new System.Windows.Forms.TreeView();
			this.LocalDirectoryCommandsContextMenu = new System.Windows.Forms.ContextMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.LocalDirectoryRefreshMenuItem = new System.Windows.Forms.MenuItem();
			this.LocalDirectoryRebuildMenuItem = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.LocalDirectoryCreateMenuItem = new System.Windows.Forms.MenuItem();
			this.LocalDirectoryRenameMenuItem = new System.Windows.Forms.MenuItem();
			this.LocalDirectorySep1MenuItem = new System.Windows.Forms.MenuItem();
			this.LocalDirectoryDeleteMenuItem = new System.Windows.Forms.MenuItem();
			this.DefaultImageList = new System.Windows.Forms.ImageList(this.components);
			this.LocalDirectoryExploreMenuItem = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// GameDataTreeView
			// 
			this.GameDataTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.GameDataTreeView.ContextMenu = this.LocalDirectoryCommandsContextMenu;
			this.GameDataTreeView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GameDataTreeView.ForeColor = System.Drawing.SystemColors.WindowText;
			this.GameDataTreeView.ImageIndex = 0;
			this.GameDataTreeView.ImageList = this.DefaultImageList;
			this.GameDataTreeView.Indent = 19;
			this.GameDataTreeView.Location = new System.Drawing.Point(0, 0);
			this.GameDataTreeView.Name = "GameDataTreeView";
			treeNode1.ImageIndex = 4;
			treeNode1.Name = "";
			treeNode1.SelectedImageIndex = 0;
			treeNode1.Text = "Node1";
			this.GameDataTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
			this.GameDataTreeView.SelectedImageIndex = 0;
			this.GameDataTreeView.Size = new System.Drawing.Size(122, 112);
			this.GameDataTreeView.TabIndex = 0;
			this.GameDataTreeView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.GameDataTreeView_BeforeExpand);
			this.GameDataTreeView.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.GameDataTreeView_AfterLabelEdit);
			this.GameDataTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.GameDataTreeView_AfterSelect);
			this.GameDataTreeView.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.GameDataTreeView_BeforeSelect);
			this.GameDataTreeView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.GameDataTreeView_ItemDrag);
			this.GameDataTreeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GameDataTreeView_MouseDown);
			this.GameDataTreeView.Click += new System.EventHandler(this.GameDataTreeView_Click);
			// 
			// LocalDirectoryCommandsContextMenu
			// 
			this.LocalDirectoryCommandsContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.LocalDirectoryRefreshMenuItem,
            this.LocalDirectoryRebuildMenuItem,
            this.menuItem2,
            this.LocalDirectoryExploreMenuItem,
            this.menuItem4,
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
			this.LocalDirectoryRefreshMenuItem.Shortcut = System.Windows.Forms.Shortcut.F5;
			this.LocalDirectoryRefreshMenuItem.Text = "Refresh";
			this.LocalDirectoryRefreshMenuItem.Click += new System.EventHandler(this.LocalDirectoryRefreshMenuItem_Click);
			// 
			// LocalDirectoryRebuildMenuItem
			// 
			this.LocalDirectoryRebuildMenuItem.Index = 2;
			this.LocalDirectoryRebuildMenuItem.Shortcut = System.Windows.Forms.Shortcut.ShiftF5;
			this.LocalDirectoryRebuildMenuItem.Text = "Rebuild Tree";
			this.LocalDirectoryRebuildMenuItem.Click += new System.EventHandler(this.LocalDirectoryRebuildMenuItem_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 3;
			this.menuItem2.Text = "-";
			// 
			// LocalDirectoryCreateMenuItem
			// 
			this.LocalDirectoryCreateMenuItem.Index = 6;
			this.LocalDirectoryCreateMenuItem.Text = "Create Directory";
			this.LocalDirectoryCreateMenuItem.Click += new System.EventHandler(this.LocalDirectoryCreateMenuItem_Click);
			// 
			// LocalDirectoryRenameMenuItem
			// 
			this.LocalDirectoryRenameMenuItem.Index = 7;
			this.LocalDirectoryRenameMenuItem.Text = "Rename Directory";
			this.LocalDirectoryRenameMenuItem.Click += new System.EventHandler(this.LocalDirectoryRenameMenuItem_Click);
			// 
			// LocalDirectorySep1MenuItem
			// 
			this.LocalDirectorySep1MenuItem.Index = 8;
			this.LocalDirectorySep1MenuItem.Text = "-";
			// 
			// LocalDirectoryDeleteMenuItem
			// 
			this.LocalDirectoryDeleteMenuItem.Index = 9;
			this.LocalDirectoryDeleteMenuItem.Text = "Delete Directory";
			this.LocalDirectoryDeleteMenuItem.Click += new System.EventHandler(this.LocalDirectoryDeleteMenuItem_Click);
			// 
			// DefaultImageList
			// 
			this.DefaultImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("DefaultImageList.ImageStream")));
			this.DefaultImageList.TransparentColor = System.Drawing.Color.Transparent;
			this.DefaultImageList.Images.SetKeyName(0, "");
			this.DefaultImageList.Images.SetKeyName(1, "");
			this.DefaultImageList.Images.SetKeyName(2, "");
			this.DefaultImageList.Images.SetKeyName(3, "");
			this.DefaultImageList.Images.SetKeyName(4, "");
			this.DefaultImageList.Images.SetKeyName(5, "");
			this.DefaultImageList.Images.SetKeyName(6, "");
			this.DefaultImageList.Images.SetKeyName(7, "");
			this.DefaultImageList.Images.SetKeyName(8, "");
			this.DefaultImageList.Images.SetKeyName(9, "");
			this.DefaultImageList.Images.SetKeyName(10, "");
			this.DefaultImageList.Images.SetKeyName(11, "");
			this.DefaultImageList.Images.SetKeyName(12, "");
			this.DefaultImageList.Images.SetKeyName(13, "");
			this.DefaultImageList.Images.SetKeyName(14, "");
			// 
			// LocalDirectoryExploreMenuItem
			// 
			this.LocalDirectoryExploreMenuItem.Index = 4;
			this.LocalDirectoryExploreMenuItem.Text = "Explore";
			this.LocalDirectoryExploreMenuItem.Click += new System.EventHandler(this.LocalDirectoryExploreMenuItem_Click);
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 5;
			this.menuItem4.Text = "-";
			// 
			// MogControl_LocalBranchTreeView
			// 
			this.Controls.Add(this.GameDataTreeView);
			this.Name = "MogControl_LocalBranchTreeView";
			this.Size = new System.Drawing.Size(120, 112);
			this.ResumeLayout(false);

		}
		#endregion

		#region Initializers
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

		private void InitializeIcons()
		{
			int i = 0;
			foreach (Image icon in this.DefaultImageList.Images)
			{
				switch(i)
				{
					case 0:	MogUtil_AssetIcons.AddIcon(icon, "mycomputer"); break;
					case 1:	MogUtil_AssetIcons.AddIcon(icon, "myprojects"); break;
					case 2:	MogUtil_AssetIcons.AddIcon(icon, "drive"); break;
					case 3:	MogUtil_AssetIcons.AddIcon(icon, "folder"); break;
					case 4:	MogUtil_AssetIcons.AddIcon(icon, "folderactive"); break;
					case 5:	MogUtil_AssetIcons.AddIcon(icon, "mogfolder"); break;
					case 6:	MogUtil_AssetIcons.AddIcon(icon, "mogfolderactive"); break;
					case 7:	MogUtil_AssetIcons.AddIcon(icon, "virtualfolder"); break;
					case 8:	MogUtil_AssetIcons.AddIcon(icon, "virtualfolderactive"); break;
					case 9:	MogUtil_AssetIcons.AddIcon(icon, "virtualfile"); break;
					case 10:MogUtil_AssetIcons.AddIcon(icon, "virtualfileactive"); break;
					case 11:MogUtil_AssetIcons.AddIcon(icon, "file"); break;
					case 12:MogUtil_AssetIcons.AddIcon(icon, "newworkspace"); break;
					case 13:MogUtil_AssetIcons.AddIcon(icon, "virtualfolderwin"); break;
					case 14:MogUtil_AssetIcons.AddIcon(icon, "virtualfolderwinactive"); break;					
				}
				i++;
			}

			GameDataTreeView.ImageList = MogUtil_AssetIcons.Images;
		}
        
		/// <summary>
		/// Initialize the Explorer treeView
		/// </summary>
		public void Initialize()
		{
			GameDataTreeView.BeginUpdate();
			GameDataTreeView.Nodes.Clear();

			mType = TreeType.FULL;

			// Setup our icons
			InitializeIcons();
					
			// Create MOG projects root
			PopulateMogProjects();
			
			// Get all local drives available on this computer
			PopulateLocalDrives();
			
			GameDataTreeView.EndUpdate();

			mInitialized = true;
		}

		/// <summary>
		/// Initialize the Explorer treeView
		/// </summary>
		public void InitializeProjectsOnly()
		{
			GameDataTreeView.BeginUpdate();
			GameDataTreeView.Nodes.Clear();

			mType = TreeType.PROJECT;

			// Setup our icons
			InitializeIcons();
					
			// Create MOG projects root
			PopulateMogProjects();
			
			GameDataTreeView.EndUpdate();

			MOG_ControllerSyncData gamedata = MOG_ControllerProject.GetCurrentSyncDataController();
			if (gamedata != null)
			{
				MOGHighLightGameDataTarget(gamedata.GetSyncDirectory());
			}
			mInitialized = true;
		}

		/// <summary>
		/// Initialize the Explorer treeView
		/// </summary>
		public void InitializeDrivesOnly()
		{
			GameDataTreeView.BeginUpdate();
			GameDataTreeView.Nodes.Clear();

			mType = TreeType.DRIVES;

			// Setup our icons
			InitializeIcons();
					
			// Get all local drives available on this computer
			PopulateLocalDrives();
			
			GameDataTreeView.EndUpdate();
			mInitialized = true;
		}

		/// <summary>
		/// Populate all the mog aware directories within the 'MyProjects' node
		/// </summary>
		private void PopulateMogProjects()
		{
			TreeNode mogProjects = new TreeNode("My Workspaces", MogUtil_AssetIcons.GetClassIconIndex("myprojects"), MogUtil_AssetIcons.GetClassIconIndex("myprojects"));
			mogProjects.NodeFont = new Font(GameDataTreeView.Font.FontFamily, GameDataTreeView.Font.Size, FontStyle.Regular);
			GameDataTreeView.Nodes.Add(mogProjects);
			
			string userName = MOG_ControllerProject.GetUserName_DefaultAdmin();

			// Get a list of all our projects that the server knows about
			ArrayList projectsArray = MOG_DBSyncedDataAPI.GetAllSyncedLocations(MOG_ControllerSystem.GetComputerName(), "", "", userName);
			if (projectsArray != null && projectsArray.Count > 0)
			{
				foreach (MOG_DBSyncedLocationInfo project in projectsArray)
				{
					// Hash our local tab name						
					string LocalBranchName = project.mWorkingDirectory;

					MOG_ControllerSyncData targetGameData = WorkspaceManager.AddNewWorkspace(LocalBranchName, project.mWorkingDirectory, project.mProjectName, project.mBranchName, project.mPlatformName,userName);

					TreeNode projectNode = new TreeNode(LocalBranchName, MogUtil_AssetIcons.GetClassIconIndex("mogfolder"), MogUtil_AssetIcons.GetClassIconIndex("mogfolderactive"));
					projectNode.Tag = new guiAssetTreeTag(project.mWorkingDirectory, projectNode, targetGameData, true);
					projectNode.NodeFont = new Font(GameDataTreeView.Font.FontFamily, GameDataTreeView.Font.Size, FontStyle.Regular);

					// Populate our project tree
					ArrayList virtualDirectories = MOG_DBAssetAPI.GetAllProjectSyncTargetFilesForPlatform(targetGameData.GetPlatformName());

				BuildDataSet:
					mSyncTargetFiles = null;
					try
					{
						mSyncTargetFiles = new DirectorySet(virtualDirectories);
					}
					catch(Exception e)
					{
						MOGPromptResult rc = MOG_Prompt.PromptResponse("Local Workspace Explorer", e.Message, MOGPromptButtons.AbortRetryIgnore);
						switch(rc)
						{
							case MOGPromptResult.Abort:
								return;
							case MOGPromptResult.Ignore:
								break;
							case MOGPromptResult.Retry:
								goto BuildDataSet;
						}
					}

					// This is a temp holder that allows our root folder to get all the correct directoryFill events that happen when the user expands the tree for the first time.
					// Do not remove
					projectNode.Nodes.Add("BLANK");

					// Expand the tree node
					VirtualExpand(projectNode, "", targetGameData);
					
					mogProjects.Nodes.Add(projectNode);
				}
			}

			// Create the helper project
			TreeNode newWorkspace = new TreeNode("<New Workspace>", MogUtil_AssetIcons.GetClassIconIndex("newworkspace"), MogUtil_AssetIcons.GetClassIconIndex("newworkspace"));

            // Allow all version types to create at least one workspace
            if (projectsArray.Count > 0)
            {
                MogUtil_VersionInfo.SetLightVersionControl(newWorkspace);
            }

			newWorkspace.NodeFont = new Font(GameDataTreeView.Font.FontFamily, GameDataTreeView.Font.Size, FontStyle.Regular);
			mogProjects.Nodes.Add(newWorkspace);

			mogProjects.Expand();
		}

		/// <summary>
		/// Create all the treeNodes corresponding with the users local drives
		/// </summary>
		private void PopulateLocalDrives()
		{
			// Create computer root
			TreeNode computer = new TreeNode("MyComputer", MogUtil_AssetIcons.GetClassIconIndex("mycomputer"), MogUtil_AssetIcons.GetClassIconIndex("mycomputer"));
			computer.NodeFont = new Font(GameDataTreeView.Font.FontFamily, GameDataTreeView.Font.Size, FontStyle.Regular);
			GameDataTreeView.Nodes.Add(computer);

			foreach (string drive in Directory.GetLogicalDrives())
			{
				// Only add local physical hard drives to our tree
				int type = (int)GetDriveType(drive);
				if (type == DRIVE_TYPE_HD)
				{
					if (Directory.Exists(drive))
					{
						TreeNode driveNode = new TreeNode(drive, MogUtil_AssetIcons.GetClassIconIndex("drive"), MogUtil_AssetIcons.GetClassIconIndex("drive"));
						driveNode.Tag = new guiAssetTreeTag(drive, driveNode, false);
						driveNode.NodeFont = new Font(GameDataTreeView.Font.FontFamily, GameDataTreeView.Font.Size, FontStyle.Regular);
						computer.Nodes.Add(driveNode);

						FillDirectory(driveNode, drive, null);
					}
				}
			}

			computer.Expand();
		}

		#endregion

		#region Virtual tree creation and expansion

		public struct KeyInfoPair
		{
		public	DirectorySetInfo	info;
		public	string				key;
		};

		/// <summary>
		/// Fetch the next set of files and folders for this parent folder
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="fullPath"></param>
		/// <param name="targetGameData"></param>
		private void VirtualExpand(TreeNode parent, string fullPath, MOG_ControllerSyncData targetGameData)
		{
			try
			{
				// If we have a gameData target we need to remove it from our path prepatory to the Dictionary lookup of the children of this folder
				if (targetGameData != null)
				{
					// Also remove any begininng backslashes
					fullPath = fullPath.Replace(targetGameData.GetSyncDirectory(), "").TrimStart("\\".ToCharArray());
				}

				guiAssetTreeTag parentInfo = (guiAssetTreeTag)parent.Tag;
				if (parentInfo != null && parentInfo.TagType == guiAssetTreeTag.TREE_FOCUS.FOLDER)
				{
					// Search our btree for the children of this folder
					if (this.mSyncTargetFiles.GetFiles(fullPath) != null)
					{
						IDictionaryEnumerator file = this.mSyncTargetFiles.GetFiles(fullPath).GetEnumerator();
												
						const byte FOLDER = 0;
						const byte FILE = 1;
						const byte TYPE_MAX = 2;


						SortedList []keyInfoLists = new SortedList[TYPE_MAX];
						keyInfoLists[FOLDER] = new SortedList();
						keyInfoLists[FILE] = new SortedList();
                        
						// First thing we want to do is iterate through all the children and separate them by type (file or folder)
						while(file.MoveNext())
						{
							DirectorySetInfo fileInfo = (DirectorySetInfo)file.Value;
							KeyInfoPair pair;
							pair.info = fileInfo;
							pair.key = file.Key as string;
							
							//Is it a file or a folder?
							if (fileInfo.Type == DirectorySetInfo.TYPE.File)
							{
								//it's a file, so put it in the file list
								keyInfoLists[FILE].Add(pair.key, pair);
							}
							else
							{
								//this is a folder, put it in the folder list
								keyInfoLists[FOLDER].Add(pair.key, pair);
							}
						}
						
						// Now all we have to do is go through the lists and put everything into the tree
						for (int list = 0; list < TYPE_MAX; list++)
						{
							for (int i = 0; i < keyInfoLists[list].Count; i++)
							{
								KeyInfoPair pair = (KeyInfoPair)keyInfoLists[list].GetByIndex(i);

								string key = pair.key as string;
								DirectorySetInfo fileInfo = pair.info;

								string fullDirectoryName = "";
								// we now need to reconstruct the full path to this file or folder
								if (fullPath.Length == 0)
								{
									fullDirectoryName = targetGameData.GetSyncDirectory() + "\\" + fileInfo.Name;
								}
								else
								{
									fullDirectoryName = targetGameData.GetSyncDirectory() + "\\" + fullPath + "\\" + fileInfo.Name;
								}

								//System.Diagnostics.Debug.WriteLine(fullDirectoryName + " = " + fileInfo.Type);

								TreeNode foundNode = FindNode(parent, fullDirectoryName);
								if (foundNode != null)
								{
									// Check if this already existing foundNode is expanded or if it has been constructed?
									if (foundNode.IsExpanded ||
										!(foundNode.Nodes.Count == 1 && foundNode.Nodes[0].Text.Equals("Blank", StringComparison.CurrentCultureIgnoreCase)))
									{
										// Fixup this foundNode because it has been expanded
										VirtualExpand(foundNode, fullPath + "\\" + foundNode.Text, targetGameData);
									}

									// Update this node's image
									UpdateNode(foundNode);
								}
								else
								{
									// Create the node
									TreeNode newNode = CreateTreeNodeFullPath(GameDataTreeView, parent, targetGameData, "\\", fileInfo.Name, fullDirectoryName, fileInfo);
									// If this is a newly created node and it is a folder, we assume it has children and create a blank node beneath it
									if (newNode != null && newNode.Nodes.Count == 0)
									{
										guiAssetTreeTag info = (guiAssetTreeTag)newNode.Tag;

										string relationalPath = "";

										// Get a relational path to this folder
										if (fullPath.Length == 0)
										{
											relationalPath = key;
										}
										else
										{
											relationalPath = fullPath + "\\" + key;
										}

										if (info != null && info.TagType == guiAssetTreeTag.TREE_FOCUS.FOLDER && mSyncTargetFiles.HasChildren(relationalPath, false))
										{
											newNode.Nodes.Add("BLANK");
										}
									}
								}
							}
						}
					}
				}
				else
				{
					// we now need to reconstruct the full path to this file or folder
					string fullFilename = fullPath;
					// Check if fullFilename is missing a root?
					if (!Path.IsPathRooted(fullFilename))
					{
						// Assume this is a relative filename and append on the current workspace directory
						fullFilename = Path.Combine(targetGameData.GetSyncDirectory(), fullFilename);
					}
					string fileName = Path.GetFileName(fullPath);
					
					DirectorySetInfo fileInfo = new DirectorySetInfo(fullFilename, DirectorySetInfo.TYPE.File);

					// Create the node
					TreeNode newNode = CreateTreeNodeFullPath(GameDataTreeView, parent, targetGameData, "\\", fileName, fullFilename, fileInfo);
				}
			}
			catch(Exception e)
			{
				MOG_Report.ReportMessage("Expand My Workspace Explorer", e.Message, e.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
			}
		}

		/// <summary>
		/// Create a virtual node based on its fullpath
		/// </summary>
		/// <param name="tree"></param>
		/// <param name="parent"></param>
		/// <param name="controller"></param>
		/// <param name="delimiter"></param>
		/// <param name="fullPath"></param>
		/// <param name="verifyFilename"></param>
		/// <param name="VirtualImageIndex"></param>
		/// <param name="NonVirtualImageIndex"></param>
		/// <param name="VirtualFileImageIndex"></param>
		/// <param name="NonVirtualFileImageIndex"></param>
		/// <returns></returns>
		private TreeNode CreateTreeNodeFullPath( TreeView tree, TreeNode parent, MOG_ControllerSyncData controller, string delimiter, string fullPath, string verifyFilename, DirectorySetInfo fileInfo)
		{
			TreeNodeCollection topNodes = null;
			
			// Get our collections to search
			if (parent != null)
			{
				topNodes = parent.Nodes;
			}
			else
			{
				topNodes = tree.Nodes;
			}

			// Split the full path by the delimiter passed in
			string[] lastNodeParts = fullPath.Split( delimiter.ToCharArray() );
			
			// find the first name
			TreeNode alreadyExistNode = null;
			if (string.Compare(parent.Text, lastNodeParts[0], true) == 0)
			{
				alreadyExistNode = parent;
			}
			else
			{
				alreadyExistNode = FindTreeNode(topNodes, lastNodeParts[0]);
			}

			// if exist find then next from the children of the first
			if (alreadyExistNode != null)
			{
				// Update this node's image
				UpdateNodeImage(alreadyExistNode);

				string LastNodePath = "";
				// Update our path to be less than what it was
				for (int i = 1; i < lastNodeParts.Length; i++)
				{
					if (LastNodePath.Length == 0)
					{
						LastNodePath = lastNodeParts[i];
					}
					else
					{
						LastNodePath = LastNodePath + "\\" + lastNodeParts[i];
					}
				}
				
				if (LastNodePath.Length > 0)
				{
					// recurse into this function for the next node
					parent = CreateTreeNodeFullPath(tree, alreadyExistNode, controller, delimiter, LastNodePath, verifyFilename, fileInfo);
				}
			}
				// if not, create it and all its children right here
			else
			{
				foreach (string nodeLeafName in lastNodeParts)
				{
					if (nodeLeafName.Length > 0)
					{
						// Create a node
						TreeNode newChild = new TreeNode(nodeLeafName);
						guiAssetTreeTag info = new guiAssetTreeTag(verifyFilename, newChild, controller, true);
						newChild.NodeFont = new Font(GameDataTreeView.Font.FontFamily, GameDataTreeView.Font.Size, FontStyle.Regular);
						
						// Is this a file or directory
						if (fileInfo.Type == DirectorySetInfo.TYPE.File)
						{
							// Set our type
							info.TagType = guiAssetTreeTag.TREE_FOCUS.FILE;
						} 
						else
						{
							// Set our type
							info.TagType = guiAssetTreeTag.TREE_FOCUS.FOLDER;
						}

						newChild.Tag = info;

						// Update this node's image
						UpdateNodeImage(newChild);
						
						if (parent != null)
						{
							parent.Nodes.Add(newChild);
							parent = newChild;
						}
						else
						{
							int index = tree.Nodes.Add(newChild);
							parent = tree.Nodes[index];
						}
					}
				}

				return parent;
			}

			return null;
		}

		private void UpdateNodeTree(TreeNode node)
		{
			// Make sure this node is valid
			while (node != null)
			{
				// Update this node
				UpdateNode(node);

				// Drill up the tree
				node = node.Parent;
			}
		}

		private void UpdateNode(TreeNode node)
		{
			// Update this node's image
			UpdateNodeImage(node);
		}

		private void UpdateNodeImage(TreeNode node)
		{
			// Get this node's info
			guiAssetTreeTag info = node.Tag as guiAssetTreeTag;
			if (info != null)
			{
				// Is this a file or directory
				if (info.TagType == guiAssetTreeTag.TREE_FOCUS.FILE)
				{
					// Must be a file
					if (File.Exists(info.FullFilename))
					{
						// Use the file's image since it exists
						node.ImageIndex = MogUtil_AssetIcons.GetFileIconIndex(info.FullFilename);
						node.SelectedImageIndex = MogUtil_AssetIcons.GetFileIconIndex(info.FullFilename);
						node.ForeColor = SystemColors.ControlText;
					}
					else
					{
						node.ImageIndex = MogUtil_AssetIcons.GetClassIconIndex("virtualfile");
						node.SelectedImageIndex = MogUtil_AssetIcons.GetClassIconIndex("virtualfileactive");
						node.ForeColor = SystemColors.GrayText;
					}
				}
				else
				{
					// Must be a directory
					if (Directory.Exists(info.FullFilename))
					{
						node.ImageIndex = MogUtil_AssetIcons.GetClassIconIndex("folder");
						node.SelectedImageIndex = MogUtil_AssetIcons.GetClassIconIndex("folderactive");
						node.ForeColor = SystemColors.ControlText;
					}
					else
					{
						node.ImageIndex = MogUtil_AssetIcons.GetClassIconIndex("virtualfolder");
						node.SelectedImageIndex = MogUtil_AssetIcons.GetClassIconIndex("virtualfolderactive");
						node.ForeColor = SystemColors.GrayText;
					}
				}
			}
		}

		/// <summary>
		/// Given a Collection of TreeNodes, find the node with Text == name (non-recursive)
		/// </summary>
		/// <param name="Nodes">The collection to look in</param>
		/// <param name="name">The Text value to search for</param>
		/// <returns>Null, if no node found, else returns TreeNode with Text matching `name`</returns>
		private TreeNode FindTreeNode(TreeNodeCollection Nodes, string name)
		{
			// Make sure we have valid nodes (which we should always have valid input, but better to check)
			if( Nodes != null && Nodes.Count > 0 )
			{				
				foreach( TreeNode node in Nodes )
				{
					// Does this node match the fist part of our split path?
					if( string.Compare(node.Text, name, true) == 0 )
					{
						return node;
					} 
				}
			}

			return null;
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
						if(string.Compare(node.Text, Path.GetFileName(title), true) == 0)
						{
							if (node.Tag != null)
							{
								guiAssetTreeTag tag = (guiAssetTreeTag)node.Tag;
								if (string.Compare(tag.FullFilename, title, true) == 0)
								{
									return node;
								}
							}
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

		/// <summary>
		/// Find and expand the selected virtual folder
		/// </summary>
		/// <param name="fullNodeName"></param>
		/// <param name="fullName"></param>
		private void VirtualFindAndExpand(string fullNodeName, string fullName)
		{
			// split up the name by backslashes
			string []parts = fullNodeName.Split("\\".ToCharArray());
			if (parts != null && parts.Length > 0)
			{
				// Keep track of nodes that could not be found because of the split i.e. 'c:\test' will have to be found in 2 passes
				string couldNotFind = "";

				// Get our current set of nodes to search
				TreeNodeCollection nodes = GameDataTreeView.Nodes;

				// For each part, lets search and expand down the tree
				foreach (string name in parts)
				{
					string findString = name;

					// If we have a previous string that could not be found, append it to this new name and try to find that one
					if (couldNotFind.Length > 0)
					{
						findString = couldNotFind + "\\" + name;
					}

					// Perform the search
					TreeNode node = FindTreeNode(nodes, findString);
					if (node != null)
					{
						// If we find it, expand that treenode if we have more drilling to do...
						if (name != parts[parts.Length-1])
						{
							node.Expand();
						}

						// Set our nodes to seach to the children of this node
						nodes = node.Nodes;

						// Set this node as selected
						GameDataTreeView.SelectedNode = node;

						// Clear our could not find string
						couldNotFind = "";
					}
					else
					{
						// OOps, we'll have to look for this one compined with the next
						couldNotFind = name;
					}
				}
			}
		}
		#endregion
		
		#region Refreshing of tree methods
		public void MOGRefresh()
		{
			try
			{
				string fullName = "";
				string fullNodeName = "";
				guiAssetTreeTag info = null;

				if (GameDataTreeView.SelectedNode != null)
				{
					// Save current selected folder
					info = (guiAssetTreeTag)GameDataTreeView.SelectedNode.Tag;
					if (info != null)
					{
						fullName = info.FullFilename;
						fullNodeName = GameDataTreeView.SelectedNode.FullPath;
					}
				}

				// Initialize tree						
				switch(mType)
				{
					case TreeType.FULL:
						Initialize();
						break;
					case TreeType.PROJECT:
						InitializeProjectsOnly();
						break;
					case TreeType.DRIVES:
						InitializeDrivesOnly();
						break;
				}

				if (info != null && info.Virtual && fullName.Length > 0 && fullNodeName.Length > 0)
				{
					// Re-open the tree to this position
					VirtualFindAndExpand(fullNodeName, fullName);
				}
			}
			catch(Exception e)
			{
				MOGPromptResult rc = MOG_Prompt.PromptResponse("Local Workspace Explorer", e.Message, MOGPromptButtons.AbortRetryIgnore);
				switch(rc)
				{
					case MOGPromptResult.Abort:
						return;
					case MOGPromptResult.Ignore:
						break;
					case MOGPromptResult.Retry:
						MOGRefresh();
						break;
				}
			}
		}

		public void MOGRefreshTree(string optionalRoot, TreeNode rootNode)
		{
			string root = "";
			if (optionalRoot != null && optionalRoot.Length > 0)
			{
				root = optionalRoot;
			}

			//root = root.ToLower();
			if (rootNode == null)
			{
				if (this.GameDataTreeView.TopNode != null)
				{
					rootNode = this.GameDataTreeView.TopNode;
				}
				else
				{
					rootNode = this.GameDataTreeView.Nodes[0];
				}
			}

			TreeNode projectNode = rootNode;
			if (rootNode.Parent != null && rootNode.Parent.Text != "My Workspaces")
			{
				projectNode = rootNode.Parent;
			}

			// Populate our project tree
			guiAssetTreeTag tag = rootNode.Tag as guiAssetTreeTag;
			if (tag != null)
			{
				MOG_ControllerSyncData workspace = tag.Object as MOG_ControllerSyncData;
				if (workspace != null)
				{
					ArrayList virtualDirectories = MOG_DBAssetAPI.GetAllProjectSyncTargetFilesForPlatform(workspace.GetPlatformName());

					// Set up the directory set object from the list of dirs recieved from the SQL server
					mSyncTargetFiles = new DirectorySet(virtualDirectories);

					// Expand root one level
					VirtualExpand(rootNode, root, workspace);

					UpdateNodeTree(rootNode);
				}
			}
		}
		#endregion

		#region Directory node creation
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
				DirectoryInfo dir = new DirectoryInfo(directoryName);
				if (dir.Exists)
				{
					// Add each found directory to the tree
					foreach (DirectoryInfo di in dir.GetDirectories())
					{
						// Only display non-hidden folders
						if (Convert.ToBoolean(di.Attributes & FileAttributes.Hidden) == false)
						{
							// Check to see if this directory is currently created from the database
							if (this.FindNode(parent, di.FullName) == null)
							{
								// Create the node
								TreeNode child = CreateDirectoryNode(di.FullName, null);

								// Get the correct color for the text of this file
								switch (this.mType)
								{
									case TreeType.PROJECT:
										child.ImageIndex = MogUtil_AssetIcons.GetClassIconIndex("virtualfolderwin");
										child.SelectedImageIndex = MogUtil_AssetIcons.GetClassIconIndex("virtualfolderwinactive");
										child.ForeColor = SystemColors.InactiveCaptionText;
										break;
									default:
										UpdateNodeTree(child);
										break;
								}

								try
								{
									// Check if the child has children
									if (di.GetDirectories().Length != 0 || (di.GetFiles().Length != 0 && MOGShowFiles))
									{
										// If so, add a temp child
										child.Nodes.Add("BLANK");
									}
								}
								catch
								{
								}

								// If we have a parent add this child
								if (parent != null)
								{
									parent.Nodes.Insert(0, child);
								}
								else
								{
									// Else add it to the master tree
									GameDataTreeView.Nodes.Insert(0, child);
								}
							}
						}
					}

					// Now get the files
					if (MOGShowFiles)
					{
						foreach (FileInfo fi in dir.GetFiles())
						{
							// Only display non-hidden files
							if (Convert.ToBoolean(fi.Attributes & FileAttributes.Hidden) == false)
							{
								// Check to see if this file is currently created from the database
								if (this.FindNode(parent, fi.FullName) == null)
								{
									//TreeNode child = new TreeNode(fi.Name, MogUtil_AssetIcons.GetFileIconIndex( fi.FullName ), MogUtil_AssetIcons.GetFileIconIndex( fi.FullName ));
									TreeNode child = new TreeNode(fi.Name, MogUtil_AssetIcons.GetFileIconIndex(fi.FullName, "Images\\NonMogFile.png"), MogUtil_AssetIcons.GetFileIconIndex(fi.FullName, "Images\\NonMogFile.png"));
									child.Tag = new guiAssetTreeTag(fi.FullName, child, false, guiAssetTreeTag.TREE_FOCUS.FILE);

									// Get the correct color for the text of this file
									switch (this.mType)
									{
										case TreeType.PROJECT:
											child.ForeColor = SystemColors.InactiveCaptionText;
											break;
										default:
											UpdateNodeTree(child);
											break;
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
							}
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
			
			string userName = MOG_ControllerProject.GetUserName_DefaultAdmin();


			TreeNode directory = new TreeNode(name);
			directory.Tag = new guiAssetTreeTag(fullname, directory, false);

			// This is a virtual directory
			if (targetGameData != null)
			{
				directory.ImageIndex = MogUtil_AssetIcons.GetClassIconIndex("virtualfolder");
				directory.SelectedImageIndex = MogUtil_AssetIcons.GetClassIconIndex("virtualfolderactive");
				directory.Tag = new guiAssetTreeTag(fullname, directory, targetGameData, false);
			}
			else
			{                			
				// Is this a MOG controlled directory - Always remove ' characters when talking to the database?
				if (MOG.DATABASE.MOG_DBSyncedDataAPI.DoesSyncedLocationExist(MOG_ControllerSystem.GetComputerName(), null, null, fullname.Replace("'",""),userName))
				{
					ArrayList gameDataArray = MOG.DATABASE.MOG_DBSyncedDataAPI.GetAllSyncedLocations(MOG_ControllerSystem.GetComputerName(), fullname.Replace("'",""), "", userName);

					if (gameDataArray.Count == 1)
					{
						MOG_DBSyncedLocationInfo dbLocation = (MOG_DBSyncedLocationInfo)gameDataArray[0];
						MOG_ControllerSyncData gameData = WorkspaceManager.AddNewWorkspace(dbLocation);
						if (gameData != null)
						{
							directory.Tag = new guiAssetTreeTag(fullname, directory, gameData, true);
						}
					}
					else if (gameDataArray.Count > 1)
					{
						MOG_Prompt.PromptMessage("Create Mog Aware directory", "Got back too many gameDataControllers for this directory!\n\nDIRECTORY: " + fullname + "\n\nTaking first one...");
						MOG_DBSyncedLocationInfo dbLocation = (MOG_DBSyncedLocationInfo)gameDataArray[0];
						MOG_ControllerSyncData gameData = WorkspaceManager.AddNewWorkspace(dbLocation);
						if (gameData != null)
						{
							directory.Tag = new guiAssetTreeTag(fullname, directory, gameData, true);
						}
					}
					else if (gameDataArray.Count == 0)
					{
						MOG_Prompt.PromptMessage("Create Mog Aware directory", "Got back zero gameDataControllers for this directory!\n\nDIRECTORY: " + fullname);
						return directory;
					}
				
					directory.ImageIndex = MogUtil_AssetIcons.GetClassIconIndex("mogfolder");
					directory.SelectedImageIndex = MogUtil_AssetIcons.GetClassIconIndex("mogfolderactive");
				}
				else
				{
					directory.ImageIndex = MogUtil_AssetIcons.GetClassIconIndex("folder");
					directory.SelectedImageIndex = MogUtil_AssetIcons.GetClassIconIndex("folderactive");
				}
			}

			return directory;
		}
		#endregion

		#region Main tree events
		/// <summary>
		/// Go to the drive and get all the files and directories directly under it
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void GameDataTreeView_BeforeExpand(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		{
			// Get our initial root path from our treeView Tag
		//	string rootPath = (string)e.Node.TreeView.Tag;

			guiAssetTreeTag dirTag = null;
			MOG_ControllerSyncData targetGamdeData = null;

			if (e.Node.Nodes[0].Text == "BLANK")
			{
				try
				{
					dirTag = (guiAssetTreeTag)e.Node.Tag;					

					// Check if this directory has a targetGamdeData
					if (dirTag.Object != null)
					{
						targetGamdeData = (MOG_ControllerSyncData)dirTag.Object;
					}

					if (dirTag.Virtual && mSyncTargetFiles != null)
					{
						// Clear out the temp node
						if (e.Node.Nodes[0].Text == "BLANK")
						{
							e.Node.Nodes.Clear();
						}
					
						// Ok, do the virtual expand
						this.VirtualExpand(e.Node, dirTag.FullFilename, targetGamdeData);
					}
				}
				catch
				{
				}
				FillDirectory(e.Node, dirTag.FullFilename, targetGamdeData);
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

			// Highlight the node for the current workspace
			if (GameDataTreeView.SelectedNode != null)
			{
				//Get the root node (called "My Workspaces")
				TreeNode rootNode = GameDataTreeView.Nodes[0];
				TreeNode workspaceNode = null;
				
				//Whatever node we selected, get the associated workspace node and highlight it
				guiAssetTreeTag tag = (guiAssetTreeTag)GameDataTreeView.SelectedNode.Tag;
				if (tag != null && tag.Object != null)
				{
					//Get the gamedata handle out of the tag for this node
					MOG_ControllerSyncData gameDataHandle  = (MOG_ControllerSyncData)tag.Object;
					
					if (gameDataHandle != null)
					{
						//Find the workspace node associated with this gamedata sync directory
						workspaceNode = FindNode(null, gameDataHandle.GetSyncDirectory());
					}
				}
				
				if (rootNode != null && workspaceNode != null && workspaceNode.Parent == rootNode)
				{
					//Go through all the workspace nodes and highlight only the current workspace
					foreach (TreeNode node in rootNode.Nodes)
					{
						if (workspaceNode == node)
						{
							//This is the one, highlight it
							node.NodeFont = new Font(GameDataTreeView.Font.FontFamily, GameDataTreeView.Font.Size, FontStyle.Bold);
						}
						else
						{
							//this is not the current workspace, so de-highlight it
							node.NodeFont = new Font(GameDataTreeView.Font.FontFamily, GameDataTreeView.Font.Size, FontStyle.Regular);
						}
					}
				}
			}
		}

		/// <summary>
		/// Clear previous selected node colors
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void GameDataTreeView_BeforeSelect(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		{
			
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

				string targetDirectoryName = tag.FullFilename;
				if (e.Label != null)
				{
					targetDirectoryName = tag.FullFilename.Replace(e.Node.Text, e.Label);
				
					if (!Directory.Exists(targetDirectoryName))
					{					
						DirectoryInfo dir = new DirectoryInfo(tag.FullFilename);
						dir.MoveTo(targetDirectoryName);

						e.Node.Tag = new guiAssetTreeTag(targetDirectoryName, e.Node, false);

						GameDataTreeView.LabelEdit = false;
						GameDataTreeView.SelectedNode = null;
						GameDataTreeView.SelectedNode = e.Node;
					}
					else
					{
						MOG_Prompt.PromptMessage("Rename Directory", "A name of that directory already exist");
						e.CancelEdit = true;
						return;
					}
				}
			}
			catch
			{
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

		private void GameDataTreeView_ItemDrag(object sender, System.Windows.Forms.ItemDragEventArgs e)
		{
			// Create our list holders
			ArrayList items = new ArrayList();

			// Make sure a valid TreeNode item is selected and that we have privileges
			if (e.Item != null)
			{
				TreeNode node = e.Item as TreeNode;
				guiAssetTreeTag tag = node.Tag as guiAssetTreeTag;

				// Make sure we are not editing the adam node
				if (node != null && tag != null && tag.TagType == guiAssetTreeTag.TREE_FOCUS.FILE)
				{
					// Add the text and a reference to the item to our lists
					items.Add(tag.FullFilename);
			
					// Create a new Data object for the send
					DataObject send = new DataObject("LocalWorkspaceTree", items);

					// Fire the DragDrop event
					DragDropEffects dde1=DoDragDrop(send, DragDropEffects.Copy);		
				}
			}
		}
		#endregion

		#region Context menu click handlers
		/// <summary>
		/// Refresh the explorer window
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LocalDirectoryRefreshMenuItem_Click(object sender, System.EventArgs e)
		{
			MOGRefresh();
		}

		
		/// <summary>
		/// Rebuild the tree
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LocalDirectoryRebuildMenuItem_Click(object sender, System.EventArgs e)
		{
			MOGRefresh();
		}

		/// <summary>
		/// Explore the selected tree node
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LocalDirectoryExploreMenuItem_Click(object sender, EventArgs e)
		{
			if (GameDataTreeView.SelectedNode != null)
			{
				guiAssetTreeTag tag = GameDataTreeView.SelectedNode.Tag as guiAssetTreeTag;
				if (tag != null)
				{
					string directory = Path.GetDirectoryName(tag.FullFilename);
					guiCommandLine.ShellExecute(directory, false);
				}
			
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
		/// Create directory
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LocalDirectoryCreateMenuItem_Click(object sender, System.EventArgs e)
		{
			try
			{
				string dirName = this.MOGGameDataTarget;
				if (!this.MOGGameDataTarget.EndsWith("\\"))
				{
					dirName += "\\";
				}

				if (Directory.CreateDirectory(dirName + "NewDirectory") != null)
				{
					TreeNode directory = CreateDirectoryNode(dirName + "NewDirectory", null);
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
							MOG_Prompt.PromptMessage("Delete Directory", "Cannot delete a directory that is a MOG Local Workspace! Remove this Workspace first then try again.");
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
					
					ProgressDialog progress = new ProgressDialog("Deleting Directory", "Please wait while the directory is deleted...", LocalDirectoryDelete_Worker, FilesToDelete, true);
					progress.ShowDialog();

					// Now delete all the files left behind
					Directory.Delete(tag.FullFilename, true);
					
					// Remove the node
					GameDataTreeView.SelectedNode.Remove();
				}
			}
			catch( Exception ex )
			{
				MOG_Prompt.PromptMessage("Delete Directory", "Could not delete this directory!\n\nMessage:" + ex.Message);
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
		/// <param name="sender"></param>
		/// <param name="e"></param>
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

		#endregion		
	}

	#region guiAssetTreeTag
	/// <summary>
	/// Summary description for guiAssetTreeTag.
	/// </summary>
	public class guiAssetTreeTag
	{
		public enum TREE_FOCUS {FOLDER, FILE};
		
		protected TREE_FOCUS mLevel;
		protected string mFullFilename;
		protected bool mExecuteCommand;
		protected object mItemPtr;
		protected bool mVirtual;
		protected object mObj;
		
		public TREE_FOCUS TagType { get{return mLevel;} set{mLevel = value;}}		// The node level within the tree
		public string FullFilename { get{return mFullFilename;} }				// Full filename to this asset
		public bool Execute { get{return mExecuteCommand;} }					// Should we allow tools to be run on this node?
		public bool Virtual { get{return mVirtual;} }							// Should we allow tools to be run on this node?
		
		public guiAssetTreeTag(string fullname, object item, object obj, bool virt)
		{
			mObj = obj;
			mFullFilename = fullname;
			mExecuteCommand = true;
			mItemPtr = item;
			mVirtual = virt;
		}
		public guiAssetTreeTag(string fullname, object item, bool virt, TREE_FOCUS level)
		{
			mFullFilename = fullname;
			mExecuteCommand = true;
			mItemPtr = item;
			mVirtual = virt;
			mLevel = level;
		}

		public guiAssetTreeTag(string fullname, object item, bool virt)
		{
			mFullFilename = fullname;
			mExecuteCommand = true;
			mItemPtr = item;
			mVirtual = virt;
		}

		public guiAssetTreeTag(string fullname, TREE_FOCUS level)
		{
			mLevel = level;
			mFullFilename = fullname;
			mExecuteCommand = false;
		}

//		public guiAssetTreeTag(string fullname)
//		{
//			mLevel = TREE_FOCUS.KEY;
//			mFullFilename = fullname;
//			mExecuteCommand = false;
//		}

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

