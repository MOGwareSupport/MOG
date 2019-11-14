using System;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using Etier.IconHelper;

namespace MOG_ControlsLibrary
{
	/// <summary>
	/// Summary description for MogControl_LocalBranchTreeView.
	/// </summary>
	public class MogControl_LocalRepositoryTreeView_Client : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.TreeView GameDataTreeView;
		private bool mShowFiles = false;
		private IconListManager mWindowsExplorerIcons;
		private ImageList mWindowsExplorerIconList;
		private bool mAllowDirectoryOpperations = true;
		private System.Windows.Forms.ImageList DefaultImageList;
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
					MOG_AssetTreeTag tag = (MOG_AssetTreeTag)GameDataTreeView.SelectedNode.Tag;
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

	
		public bool MOGShowFiles
		{
			get { return mShowFiles; }
			set { mShowFiles = value; }
		}

		public bool MOGAllowDirectoryOpperations
		{
			get { return mAllowDirectoryOpperations; }
			set { mAllowDirectoryOpperations = value; }
		}
		#endregion

		public MogControl_LocalRepositoryTreeView_Client()
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

		// Icons
		private const int MY_COMPUTER		= 0;
		private const int MY_PROJECTS		= 1;
		private const int DRIVE				= 2;
		private const int MOG_DRIVE			= 3;
		private const int FOLDER			= 4;
		private const int MOG_REPOSITORY	= 5;

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
		public void InitializeNetworkDrivesOnly()
		{
			GameDataTreeView.BeginUpdate();
			GameDataTreeView.Nodes.Clear();
					
			// Get all local drives available on this computer
			PopulateLocalDrives();
			
			GameDataTreeView.EndUpdate();
		}
		
		/// <summary>
		/// Create all the treeNodes corresponding with the users local drives
		/// </summary>
		private void PopulateLocalDrives()
		{
			// Create computer root
			TreeNode computer = new TreeNode("MyComputer", MY_COMPUTER, MY_COMPUTER);
			GameDataTreeView.Nodes.Add(computer);

			foreach (string drive in Directory.GetLogicalDrives())
			{
				try
				{
					// Only add local physical hard drives to our tree
					int type = (int)GetDriveType(drive);
					if (type == DRIVE_TYPE_HD || type == DRIVE_TYPE_NETWORK)
					{
						if (Directory.Exists(drive))
						{
							TreeNode driveNode = new TreeNode(drive, DRIVE, DRIVE);
							driveNode.Tag = new MOG_AssetTreeTag(drive, driveNode);

							if (File.Exists(drive + "\\MogRepository.ini"))
							{
								driveNode.ImageIndex = MOG_DRIVE;
								driveNode.SelectedImageIndex = MOG_DRIVE;
							}
							else
							{
								driveNode.ImageIndex = DRIVE;
								driveNode.SelectedImageIndex = DRIVE;
							}

							computer.Nodes.Add(driveNode);

							FillDirectory(driveNode, drive);
						}
					}
				}
				catch
				{
				}
			}

			computer.Expand();
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
						MOG_AssetTreeTag tag = (MOG_AssetTreeTag)node.Tag;
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

		/// <summary>
		/// Fill the current parent node with all the files and directories found in the directory name provided
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="directoryName"></param>
		private void FillDirectory(TreeNode parent, string directoryName)
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
				{
					throw new DirectoryNotFoundException ("directory does not exist:" + directoryName);
				}

				// Add each found directory to the tree
				foreach(DirectoryInfo di in dir.GetDirectories())
				{
					try
					{
						// Ignore hidden folders
						if ((di.Attributes & FileAttributes.Hidden) == 0)
						{
							// Create the node
							TreeNode child = CreateDirectoryNode(di.FullName);

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
					}
					catch (Exception e)
					{
						//We can't add this node for some reason, just move onto the next one
						System.Console.Out.WriteLine(e.Message);
					}
				}

				// Now get the files
				if (MOGShowFiles)
				{
					foreach(FileInfo fi in dir.GetFiles())
					{
						TreeNode child = new TreeNode(fi.Name, mWindowsExplorerIcons.AddFileIcon( fi.FullName ), mWindowsExplorerIcons.AddFileIcon( fi.FullName ));
						child.Tag = new MOG_AssetTreeTag(fi.FullName, child);
 
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
		private TreeNode CreateDirectoryNode(string fullname)
		{
			string name = fullname;

			try
			{
				name = Path.GetFileName(fullname);
			}
			catch
			{
			}

			TreeNode directory = new TreeNode(name);
			directory.Tag = new MOG_AssetTreeTag(fullname, directory);

			if (File.Exists(fullname + "\\MogRepository.ini"))
			{
				directory.ImageIndex = MOG_REPOSITORY;
				directory.SelectedImageIndex = MOG_REPOSITORY;
			}
			else
			{
				directory.ImageIndex = FOLDER;
				directory.SelectedImageIndex = FOLDER;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MogControl_LocalRepositoryTreeView_Client));
			this.GameDataTreeView = new System.Windows.Forms.TreeView();
			this.DefaultImageList = new System.Windows.Forms.ImageList(this.components);
			this.SuspendLayout();
			// 
			// GameDataTreeView
			// 
			this.GameDataTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
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
			this.GameDataTreeView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.GameDataTreeView_BeforeExpand);
			// 
			// DefaultImageList
			// 
			this.DefaultImageList.ImageSize = new System.Drawing.Size(16, 16);
			this.DefaultImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("DefaultImageList.ImageStream")));
			this.DefaultImageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// MogControl_LocalRepositoryTreeView_Client
			// 
			this.Controls.Add(this.GameDataTreeView);
			this.Name = "MogControl_LocalRepositoryTreeView_Client";
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
					MOG_AssetTreeTag tag = (MOG_AssetTreeTag)e.Node.Tag;
					if (Path.IsPathRooted(tag.FullFilename))
					{
						FillDirectory(e.Node, tag.FullFilename);
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
		/// Refresh the explorer window
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LocalDirectoryRefreshMenuItem_Click(object sender, System.EventArgs e)
		{
			this.InitializeNetworkDrivesOnly();
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
}

