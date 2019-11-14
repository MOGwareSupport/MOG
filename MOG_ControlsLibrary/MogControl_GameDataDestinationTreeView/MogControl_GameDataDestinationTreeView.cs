using System;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using MOG.PROMPT;
using MOG.REPORT;
using MOG.DATABASE;
using MOG.DOSUTILS;
using MOG.CONTROLLER.CONTROLLERPROJECT;

using MOG_ControlsLibrary;
using MOG_ControlsLibrary.Controls;
using Tst;

using Etier.IconHelper;
using System.Collections.Specialized;

namespace MOG_ControlsLibrary.Controls
{
	/// <summary>
	/// Summary description for MogControl_GameDataDestinationTreeView.
	/// </summary>
	public class MogControl_GameDataDestinationTreeView : System.Windows.Forms.UserControl
	{
		public System.Windows.Forms.TreeView GameDataTreeView;
		private TreeNode mLastSelectedNode;
		private bool mShowFiles = false;
		private bool mInitialized = false;
		private TreeNode mRoot;
		private IconListManager mWindowsExplorerIcons;
		public const string NoWorkspaceText = "Local Workpace not defined. "
			+"To see this game data tree, make sure "
			+"to define a Local Workspace in the lower-left corner of MOG's Workspace tab.";
		private ImageList mWindowsExplorerIconList;
		private System.Windows.Forms.ImageList DefaultImageList;
		private System.Windows.Forms.ContextMenuStrip GameDataContextMenu;
		private System.Windows.Forms.ToolStripMenuItem GameDataCreateMenuItem;
		public System.Windows.Forms.ToolStripMenuItem GameDataRenameMenuItem;
		public System.Windows.Forms.ToolStripMenuItem GameDataDeleteMenuItem;
		private System.ComponentModel.IContainer components;

		public delegate void TreeViewEvent(object sender, System.Windows.Forms.TreeViewEventArgs e);
		
		[Category("Behavior"), Description("Occures when selection has been changed")]
		public event TreeViewEvent AfterTargetSelect;

		private DirectorySet mSyncTargetFiles;
		private System.Windows.Forms.Label NoLocalDataLabel;
		private bool mVirtual = false;

		#region getters
		public string MOGGameDataTarget
		{
			get
			{ 
				if (GameDataTreeView.SelectedNode != null)
				{
					return GameDataTreeView.SelectedNode.FullPath;
				}

				return "";
			}
		}

		public TreeNodeCollection MOGNodes
		{
			get { return GameDataTreeView.Nodes; }
		}

		public TreeNode MOGRootNode
		{
			get { return this.mRoot; }
		}

		public TreeNode MOGSelectedNode
		{
			get { return GameDataTreeView.SelectedNode; }
			set { GameDataTreeView.SelectedNode = value; }
		}

		public bool Initialized
		{
			get { return mInitialized; }
		}

		public bool MOGShowFiles
		{
			get { return mShowFiles; }
			set { mShowFiles = value; }
		}
		#endregion

		public MogControl_GameDataDestinationTreeView()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			
			// Set our NoLocalDataLabel.Text
			this.NoLocalDataLabel.Text = NoWorkspaceText;

			// Load the folder icon
			mWindowsExplorerIconList = new ImageList();
			mWindowsExplorerIconList.ColorDepth = ColorDepth.Depth32Bit;
			mWindowsExplorerIconList.ImageSize = new System.Drawing.Size( 16, 16 );
			mWindowsExplorerIconList.Images.Add(this.DefaultImageList.Images[0]);

			mWindowsExplorerIcons = new IconListManager(mWindowsExplorerIconList, Etier.IconHelper.IconReader.IconSize.Small);

			GameDataTreeView.ImageList = mWindowsExplorerIconList;
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

		public void ReinitializeVirtual(string platform)
		{
			mInitialized = false;
			InitializeVirtual(platform);
		}

		public void InitializeVirtual(string platform)
		{
			// Default us to the current workspace directory
			string workspaceDirectory = MOG_ControllerProject.GetWorkspaceDirectory();
			// Check if we have no workspace directory?
			if (workspaceDirectory.Length == 0)
			{
				// Default to the project name
				workspaceDirectory = MOG_ControllerProject.GetProjectName();
			}

			InitializeVirtual(workspaceDirectory, platform);
		}

		public void InitializeVirtual(string workspaceDirectory, string platform)
		{
			if (!mInitialized)
			{
				GameDataTreeView.BeginUpdate();

				GameDataTreeView.Nodes.Clear();
				mVirtual = true;

				// Populate our project tree
				ArrayList virtualDirectories = MOG_DBAssetAPI.GetAllProjectSyncTargetFilesForPlatform(platform);
				ArrayList fixedUpVirtualDirectories = new ArrayList();
				foreach (string file in virtualDirectories)
				{
					// Only add files that have '\\' in front of them.  These are files.  Those
					// without '\\' are packages and the files that go into them
					if (file.StartsWith("\\"))
					{
						fixedUpVirtualDirectories.Add(file.TrimStart("\\".ToCharArray()));
					}
					else
					{
						// File did not have a starting '\', so just add it :)
						fixedUpVirtualDirectories.Add(file);
					}
				}

				mSyncTargetFiles = new DirectorySet(fixedUpVirtualDirectories);

				mRoot = new TreeNode(workspaceDirectory);
				mRoot.Tag = new guiAssetTreeTag(workspaceDirectory, mRoot, false);

				VirtualExpand(mRoot, "", workspaceDirectory);

				FillDirectory(mRoot, workspaceDirectory, SystemColors.GrayText);

				GameDataTreeView.Nodes.Add(mRoot);

				mRoot.Expand();

				mInitialized = true;

				// If we have nothing for this tree, we should disable it...
				if (GameDataTreeView.Nodes.Count < 1)
				{
					GameDataTreeView.Visible = false;
					NoLocalDataLabel.Visible = true;
				}
				else
				{
					GameDataTreeView.Visible = true;
					NoLocalDataLabel.Visible = false;
				}

				GameDataTreeView.EndUpdate();
			}
		}

		#region Virtual tree creation and expansion

		/// <summary>
		/// Fetch the next set of files and folders for this parent folder
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="fullPath"></param>
		/// <param name="targetGameData"></param>
		private void VirtualExpand(TreeNode parent, string fullPath, string workspaceDirectory)
		{
			try
			{
				// If we have a gameData target we need to remove it from our path prepatory to the Dictionary lookup of the children of this folder
				if (DosUtils.PathIsWithinPath(workspaceDirectory, fullPath))
				{
					// Also remove any begininng backslashes
					fullPath = DosUtils.PathMakeRelativePath(workspaceDirectory, fullPath);
				}

				HybridDictionary dic = mSyncTargetFiles.GetFiles(fullPath);
				if (dic != null)
				{
					// Search our btree for the children of this folder
					IDictionaryEnumerator file = dic.GetEnumerator();

					// Now itterate through those children
					while (file.MoveNext())
					{
						string fullDirectoryName = "";
						// we now need to reconstruct the full path to this file or folder
						if (fullPath.Length == 0)
						{
							fullDirectoryName = workspaceDirectory + "\\" + file.Key;
						}
						else
						{
							fullDirectoryName = workspaceDirectory + "\\" + fullPath + "\\" + file.Key;
						}

						DirectorySetInfo fileInfo = (DirectorySetInfo)file.Value;

						// Create the node
						TreeNode newNode = CreateTreeNodeFullPath(GameDataTreeView, parent, workspaceDirectory, "\\", file.Key as string, fullDirectoryName, fileInfo, 0, 0, 0, 0, true);

						// If this is a newly created node and it is a folder, we assume it has children and create a black node beneath it
						if (newNode != null && newNode.Nodes.Count == 0)
						{
							guiAssetTreeTag info = (guiAssetTreeTag)newNode.Tag;
							string relationalPath = "";

							// Get a relational path to this folder
							if (fullPath.Length == 0)
							{
								relationalPath = file.Key as string;
							}
							else
							{
								relationalPath = fullPath + "\\" + file.Key;
							}

							// If this file is a folder and has children, create a place holder 'blank'
							if (info != null && info.TagType == guiAssetTreeTag.TREE_FOCUS.FOLDER && mSyncTargetFiles.HasFolderChildren(relationalPath, false))
							{
								newNode.Nodes.Add("BLANK");
							}
						}
					}
				}
			}
			catch
			{
			}
		}
		
		/// <summary>
		/// Create a virtual node of directories only based on its fullpath
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
		private TreeNode CreateTreeNodeFullPath( TreeView tree, TreeNode parent, string workspaceDirectory, string delimiter, string fullPath, string verifyFilename, DirectorySetInfo fileInfo,
			int VirtualImageIndex, int NonVirtualImageIndex, int VirtualFileImageIndex, int NonVirtualFileImageIndex, bool directoriesOnly)
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
			if (parent != null && string.Compare(parent.Text, lastNodeParts[0], true) == 0)
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
				// Is this a file or directory
				if (fileInfo.Type == DirectorySetInfo.TYPE.Folder || fileInfo.Type == DirectorySetInfo.TYPE.FolderName)
				{
					// Must be a directory
					if (!DosUtils.PathIsDriveRooted(verifyFilename) ||
						Directory.Exists(verifyFilename))
					{
						alreadyExistNode.ImageIndex = NonVirtualImageIndex;
						alreadyExistNode.SelectedImageIndex = NonVirtualImageIndex;
						alreadyExistNode.ForeColor = SystemColors.ControlText;
					}
					else
					{
						alreadyExistNode.ImageIndex = VirtualImageIndex;
						alreadyExistNode.SelectedImageIndex = VirtualImageIndex;
						alreadyExistNode.ForeColor = SystemColors.GrayText;
					}
				} 
				else
				{
					// Must be a file
					if (!DosUtils.PathIsDriveRooted(verifyFilename) ||
						File.Exists(verifyFilename))
					{
						alreadyExistNode.ImageIndex = NonVirtualFileImageIndex;
						alreadyExistNode.SelectedImageIndex = NonVirtualFileImageIndex;
						alreadyExistNode.ForeColor = SystemColors.ControlText;
					}
					else
					{
						alreadyExistNode.ImageIndex = VirtualFileImageIndex;
						alreadyExistNode.SelectedImageIndex = VirtualFileImageIndex;
						alreadyExistNode.ForeColor = SystemColors.GrayText;
					}
				}

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
					parent = CreateTreeNodeFullPath(tree, alreadyExistNode, workspaceDirectory, delimiter, LastNodePath, verifyFilename, fileInfo, VirtualImageIndex, NonVirtualImageIndex, VirtualFileImageIndex, NonVirtualFileImageIndex, directoriesOnly);
				}
			}
				// if not, create it and all its children right here
			else
			{
				foreach (string nodeLeafName in lastNodeParts)
				{
					if (nodeLeafName.Length > 0)
					{
						// If we are a file and this is a directoriesOnly create, skip
						if (fileInfo.Type == DirectorySetInfo.TYPE.File && directoriesOnly)
						{
							break;
						}

						// Create a node
						TreeNode newChild = new TreeNode(nodeLeafName);
						guiAssetTreeTag info = new guiAssetTreeTag(verifyFilename, newChild, workspaceDirectory, true);
						
						// Is this a file or directory
						if (fileInfo.Type == DirectorySetInfo.TYPE.Folder || fileInfo.Type == DirectorySetInfo.TYPE.FolderName)
						{
							// Set our type
							info.TagType = guiAssetTreeTag.TREE_FOCUS.FOLDER;

							// Must be a directory
							if (!DosUtils.PathIsDriveRooted(verifyFilename) ||
								Directory.Exists(verifyFilename))
							{
								newChild.ImageIndex = NonVirtualImageIndex;
								newChild.SelectedImageIndex = NonVirtualImageIndex;
								newChild.ForeColor = SystemColors.ControlText;
							}
							else
							{
								newChild.ImageIndex = VirtualImageIndex;
								newChild.SelectedImageIndex = VirtualImageIndex;
								newChild.ForeColor = SystemColors.GrayText;
							}
						} 
						else
						{
							// Set our type
							info.TagType = guiAssetTreeTag.TREE_FOCUS.FILE;

							// Must be a file
							if (!DosUtils.PathIsDriveRooted(verifyFilename) ||
								File.Exists(verifyFilename))
							{
								newChild.ImageIndex = NonVirtualFileImageIndex;
								newChild.SelectedImageIndex = NonVirtualFileImageIndex;
								newChild.ForeColor = SystemColors.ControlText;
							}
							else
							{
								newChild.ImageIndex = VirtualFileImageIndex;
								newChild.SelectedImageIndex = VirtualFileImageIndex;
								newChild.ForeColor = SystemColors.GrayText;
							}
						}
						
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

						newChild.Tag = info;
					}
				}

				// If we are a file and this is a directoriesOnly create, skip
				if (fileInfo.Type == DirectorySetInfo.TYPE.File && directoriesOnly)
				{
					return null;
				}
				return parent;
			}

			return null;
		}

		public void VirtualFindAndExpand(TreeNodeCollection nodes, string fullNodeName)
		{
			// split up the name by backslashes
			string[] parts = fullNodeName.Split("\\".ToCharArray());
			if (parts != null && parts.Length > 0)
			{
				// Keep track of nodes that could not be found because of the split i.e. 'c:\test' will have to be found in 2 passes
				string couldNotFind = "";

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
						// If there are nodes under this one?
						if (node.Nodes.Count > 0)
						{
							// Populate the nodes
							ExpandTreeNode(new TreeViewCancelEventArgs(node, false, TreeViewAction.Expand));

							// Now, expand that treenode
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
						// OOps, we'll have to look for this one combined with the next
						couldNotFind = name;
					}
				}
			}
		}
		/// <summary>
		/// Find and expand the selected virtual folder
		/// </summary>
		/// <param name="fullNodeName"></param>
		/// <param name="fullName"></param>
		public void VirtualFindAndExpand(string fullNodeName, string fullName)
		{
			VirtualFindAndExpand(GameDataTreeView.Nodes, fullNodeName);			
		}
		#endregion
		

		public void Initialize(string rootName, string root)
		{
			GameDataTreeView.BeginUpdate();

			GameDataTreeView.Nodes.Clear();
			GameDataTreeView.Tag = root;

			mRoot = new TreeNode(rootName);
			mRoot.ImageIndex = 0;

			FillDirectory(mRoot, root, SystemColors.ControlText);

			GameDataTreeView.Nodes.Add(mRoot);

			mRoot.Expand();
					
			GameDataTreeView.EndUpdate();
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

		private TreeNode FindTreeNode(TreeNode parentNode, string title)
		{
			bool root = false;
			try
			{
				if (parentNode == null)
				{
					if (this.GameDataTreeView.Nodes != null && this.GameDataTreeView.Nodes.Count > 0)
					{
						parentNode = this.GameDataTreeView.Nodes[0];
						root = true;
					}
				}

				if (parentNode != null)
				{
					if (root)
					{
						foreach (TreeNode node in this.GameDataTreeView.Nodes)
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
					else
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
			}
			catch
			{
			}

			return null;
		}

		private TreeNode FindNodeDeep(TreeNode parentNode, string title)
		{
			if (parentNode != null)
			{
				// Is this node it?
				if (string.Compare(parentNode.Text, title, true) == 0)
				{
					return parentNode;
				}
				else
				{
					// Walk all these nodes
					foreach (TreeNode node in parentNode.Nodes)
					{
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
						else if (string.Compare(node.FullPath, title, true) == 0)
						{
							return node;
						}
					}
				}
			}
			
			return null;
		}
	
		private void FillDirectory(TreeNode parent, string directoryName, Color foreColor)
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

				// Make suer it exits
				if (dir.Exists)
				{					
					// Add each found directory to the tree
					foreach(DirectoryInfo di in dir.GetDirectories())
					{
						// Only display non-hidden files
						if (Convert.ToBoolean(di.Attributes & FileAttributes.Hidden) == false)
						{
							// Check to see if this directory is currently created from the database
							TreeNode existingNode = this.FindTreeNode(parent, di.FullName);
							if ( existingNode == null)
							{
								TreeNode child = new TreeNode();
								child.Text = di.Name;
								child.Tag = new guiAssetTreeTag(di.FullName, null, false);
								child.ImageIndex = 0;
								child.ForeColor = foreColor;

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
							else
							{
								// Check if the child has children
								if (existingNode.Nodes.Count == 0 && 
									di.GetDirectories().Length != 0 || 
									(di.GetFiles().Length != 0 && 
									MOGShowFiles))
								{
									// If so, add a temp child
									existingNode.Nodes.Add("BLANK");
								}
							}
						}
					}

					// Now get the files
					if (MOGShowFiles)
					{
						foreach(FileInfo fi in dir.GetFiles())
						{
							// Only display non-hidden files
							if (Convert.ToBoolean(fi.Attributes & FileAttributes.Hidden) == false)
							{
								TreeNode child = new TreeNode();
								child.Text = fi.Name;
								child.Tag = new guiAssetTreeTag(fi.FullName, null, false);
								child.ImageIndex = mWindowsExplorerIcons.AddFileIcon( fi.FullName );
								child.ForeColor = foreColor;
 
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
			catch(Exception ex)
			{
				MOG_Report.ReportMessage("Fill Directory", ex.Message, ex.StackTrace, MOG_ALERT_LEVEL.CRITICAL);
			}
		}		

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Node1", 0, 0);
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MogControl_GameDataDestinationTreeView));
			this.GameDataTreeView = new System.Windows.Forms.TreeView();
			this.GameDataContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.GameDataCreateMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.GameDataRenameMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.GameDataDeleteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.DefaultImageList = new System.Windows.Forms.ImageList(this.components);
			this.NoLocalDataLabel = new System.Windows.Forms.Label();
			this.GameDataContextMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// GameDataTreeView
			// 
			this.GameDataTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.GameDataTreeView.ContextMenuStrip = this.GameDataContextMenu;
			this.GameDataTreeView.HideSelection = false;
			this.GameDataTreeView.ImageIndex = 0;
			this.GameDataTreeView.ImageList = this.DefaultImageList;
			this.GameDataTreeView.Indent = 19;
			this.GameDataTreeView.Location = new System.Drawing.Point(0, 0);
			this.GameDataTreeView.Name = "GameDataTreeView";
			treeNode1.ImageIndex = 0;
			treeNode1.Name = "";
			treeNode1.SelectedImageIndex = 0;
			treeNode1.Text = "Node1";
			this.GameDataTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
			this.GameDataTreeView.SelectedImageIndex = 0;
			this.GameDataTreeView.Size = new System.Drawing.Size(146, 124);
			this.GameDataTreeView.Sorted = true;
			this.GameDataTreeView.TabIndex = 0;
			this.GameDataTreeView.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.GameDataTreeView_AfterLabelEdit);
			this.GameDataTreeView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.GameDataTreeView_BeforeExpand);
			this.GameDataTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.GameDataTreeView_AfterSelect);
			this.GameDataTreeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GameDataTreeView_MouseDown);
			// 
			// GameDataContextMenu
			// 
			this.GameDataContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.GameDataCreateMenuItem,
            this.GameDataRenameMenuItem,
            this.GameDataDeleteMenuItem});
			this.GameDataContextMenu.Name = "GameDataContextMenu";
			this.GameDataContextMenu.Size = new System.Drawing.Size(172, 92);
			this.GameDataContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.GameDataContextMenu_Opening);
			// 
			// GameDataCreateMenuItem
			// 
			this.GameDataCreateMenuItem.Enabled = false;
			this.GameDataCreateMenuItem.Name = "GameDataCreateMenuItem";
			this.GameDataCreateMenuItem.Size = new System.Drawing.Size(171, 22);
			this.GameDataCreateMenuItem.Text = "Create directory";
			this.GameDataCreateMenuItem.Click += new System.EventHandler(this.GameDataCreateMenuItem_Click);
			// 
			// GameDataRenameMenuItem
			// 
			this.GameDataRenameMenuItem.Enabled = false;
			this.GameDataRenameMenuItem.Name = "GameDataRenameMenuItem";
			this.GameDataRenameMenuItem.Size = new System.Drawing.Size(171, 22);
			this.GameDataRenameMenuItem.Text = "Rename Directory";
			this.GameDataRenameMenuItem.Click += new System.EventHandler(this.GameDataRenameMenuItem_Click);
			// 
			// GameDataDeleteMenuItem
			// 
			this.GameDataDeleteMenuItem.Enabled = false;
			this.GameDataDeleteMenuItem.Name = "GameDataDeleteMenuItem";
			this.GameDataDeleteMenuItem.Size = new System.Drawing.Size(171, 22);
			this.GameDataDeleteMenuItem.Text = "Delete Directory";
			this.GameDataDeleteMenuItem.Click += new System.EventHandler(this.GameDataDeleteMenuItem_Click);
			// 
			// DefaultImageList
			// 
			this.DefaultImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("DefaultImageList.ImageStream")));
			this.DefaultImageList.TransparentColor = System.Drawing.Color.Transparent;
			this.DefaultImageList.Images.SetKeyName(0, "");
			// 
			// NoLocalDataLabel
			// 
			this.NoLocalDataLabel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.NoLocalDataLabel.Location = new System.Drawing.Point(0, 0);
			this.NoLocalDataLabel.Name = "NoLocalDataLabel";
			this.NoLocalDataLabel.Size = new System.Drawing.Size(144, 130);
			this.NoLocalDataLabel.TabIndex = 1;
			this.NoLocalDataLabel.Text = "Derned Text Message";
			// 
			// MogControl_GameDataDestinationTreeView
			// 
			this.Controls.Add(this.GameDataTreeView);
			this.Controls.Add(this.NoLocalDataLabel);
			this.Name = "MogControl_GameDataDestinationTreeView";
			this.Size = new System.Drawing.Size(144, 130);
			this.GameDataContextMenu.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void GameDataTreeView_BeforeExpand(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		{
			ExpandTreeNode(e);
		}

		private void ExpandTreeNode(System.Windows.Forms.TreeViewCancelEventArgs e)
		{
			// Get our initial root path from our treeView Tag
			string rootPath = (string)e.Node.TreeView.Tag;

			if (e.Node.Nodes[0].Text == "BLANK")
			{
				try
				{
					guiAssetTreeTag info = null;
					if (mVirtual)
					{
						info = (guiAssetTreeTag)e.Node.Tag;
						if (info != null)
						{
							// Clear out the temp node
							if (e.Node.Nodes.Count == 1 && e.Node.Nodes[0].Text == "BLANK")
							{
								e.Node.Nodes.Clear();
							}

							string workspaceDirectory = info.Object as string;
							VirtualExpand(e.Node, info.FullFilename, workspaceDirectory);

							// Add any non MOG files
							FillDirectory(e.Node, info.FullFilename, SystemColors.GrayText);
						}
					}
					else if (Path.IsPathRooted(e.Node.FullPath))
					{
						//info = (guiAssetTreeTag)e.Node.Tag;
						FillDirectory(e.Node, e.Node.FullPath, SystemColors.ControlText);
					}
					else
					{
						string[] parts = rootPath.Split(":".ToCharArray());
						string drive = parts[0];
						FillDirectory(e.Node, drive + ":\\" + e.Node.FullPath, SystemColors.ControlText);
					}
				}
				catch
				{
				}
			}
		}

		private void GameDataTreeView_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			if (AfterTargetSelect != null)
			{
				object []args = {sender, e};
				this.Invoke(AfterTargetSelect, args);
			}

			mLastSelectedNode = e.Node;
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
			
			string userName = MOG_ControllerProject.GetUserName_DefaultAdmin();

			TreeNode directory = new TreeNode(name);
			directory.ForeColor = SystemColors.GrayText;
			directory.Tag = new guiAssetTreeTag(fullname, directory, false);
			
			return directory;
		}

		private void GameDataCreateMenuItem_Click(object sender, System.EventArgs e)
		{
			try
			{
				// Create a directory as a child directory of a node
				if (GameDataTreeView.SelectedNode != null)
				{
					guiAssetTreeTag tag = GameDataTreeView.SelectedNode.Tag as guiAssetTreeTag;
					
					string directoryPath = tag.FullFilename + "\\NewDirectory";
					TreeNode directory = CreateDirectoryNode( tag.FullFilename + "\\NewDirectory");

					// Now create the folder
					if (!DosUtils.DirectoryCreate(directoryPath))
					{
						MOG_Prompt.PromptResponse("Create Directory", DosUtils.GetLastError(), MOGPromptButtons.OK);
					}
					else
					{
						// Now edit the name of this node
						GameDataTreeView.LabelEdit = true;
						GameDataTreeView.SelectedNode.Nodes.Add( directory );
						GameDataTreeView.SelectedNode = directory;
						GameDataTreeView.SelectedNode.BeginEdit();
					}
				}
				else
				{
					// Create a directory at the root of the project
					string directoryPath = MOG_ControllerProject.GetCurrentSyncDataController().GetSyncDirectory() + "\\NewDirectory";
					TreeNode directory = CreateDirectoryNode( directoryPath);

					// Now create the folder
					if (!DosUtils.DirectoryCreate(directoryPath))
					{
						MOG_Prompt.PromptResponse("Create Directory", DosUtils.GetLastError(), MOGPromptButtons.OK);
					}
					else
					{
						// Now edit the name of this node
						GameDataTreeView.LabelEdit = true;
						GameDataTreeView.SelectedNode = directory;
						directory.BeginEdit();
					}
				}
			}
			catch(Exception ex)
			{
				MOG_Report.ReportMessage("Create Directory", ex.Message, ex.StackTrace, MOG_ALERT_LEVEL.CRITICAL);
			}	
		}

		private void GameDataRenameMenuItem_Click(object sender, System.EventArgs e)
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

		private void GameDataTreeView_AfterLabelEdit(object sender, System.Windows.Forms.NodeLabelEditEventArgs e)
		{
			try
			{	
				// Make sure an edit actually occured
				if (e.Label != null)
				{
					guiAssetTreeTag tag = (guiAssetTreeTag)e.Node.Tag;

					string sourceDirectoryName = tag.FullFilename;
					string targetDirectoryName = tag.FullFilename.Replace(e.Node.Text, e.Label);

					e.Node.Tag = new guiAssetTreeTag(targetDirectoryName, e.Node, false);

					if (!DosUtils.DirectoryMove(sourceDirectoryName, targetDirectoryName))
					{
						MOG.PROMPT.MOG_Prompt.PromptResponse("Rename Directory", DosUtils.GetLastError(), MOGPromptButtons.OK);
						e.Node.Remove();
						e.CancelEdit = true;					
					}

					// Fire off our AfterSelect, so that it updates everything propertly
					e.Node.Text = e.Label;
					this.GameDataTreeView_AfterSelect(this.GameDataTreeView, new TreeViewEventArgs(e.Node, TreeViewAction.Unknown));
				}
				GameDataTreeView.LabelEdit = false;
			}
			catch(Exception ex)
			{
				MOG.PROMPT.MOG_Prompt.PromptResponse("Rename Directory", ex.Message, MOGPromptButtons.OK);
				e.CancelEdit = true;
				e.Node.Remove();
				GameDataTreeView.LabelEdit = false;
			}
		}

		private void GameDataDeleteMenuItem_Click(object sender, System.EventArgs e)
		{
			try
			{
				guiAssetTreeTag tag = (guiAssetTreeTag)GameDataTreeView.SelectedNode.Tag;
				if (MOG_Prompt.PromptResponse("Delete Directory", "Are you sure you wan to delete this directory with all its contents?\n\nDirectory:\n\n" + tag.FullFilename, MOGPromptButtons.YesNo) == MOGPromptResult.Yes)
				{						
					// Remove the node
					GameDataTreeView.SelectedNode.Remove();
				}
			}
			catch( Exception ex )
			{
				MOG_Prompt.PromptMessage("Delete Directory", "Could not delete this directory!\n\nMessage:" + ex.Message);
			}
		}

		private void GameDataContextMenu_Opening(object sender, CancelEventArgs e)
		{
			GameDataCreateMenuItem.Enabled = true;
			this.GameDataDeleteMenuItem.Enabled = false;
			this.GameDataRenameMenuItem.Enabled = false;

			if (GameDataTreeView.SelectedNode != null && GameDataTreeView.SelectedNode.Tag != null)
			{
				guiAssetTreeTag tag = GameDataTreeView.SelectedNode.Tag as guiAssetTreeTag;

				// We can only rename and delete directories that both exist and are non mog controlled (SystemColors.GrayText)
				if (Directory.Exists(tag.FullFilename) && GameDataTreeView.SelectedNode.ForeColor == SystemColors.GrayText)
				{
					this.GameDataDeleteMenuItem.Enabled = true;
					this.GameDataRenameMenuItem.Enabled = true;
				}				
			}			
		}
		
		private void GameDataTreeView_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			GameDataTreeView.SelectedNode = GameDataTreeView.GetNodeAt(new Point(e.X, e.Y));
		}
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
