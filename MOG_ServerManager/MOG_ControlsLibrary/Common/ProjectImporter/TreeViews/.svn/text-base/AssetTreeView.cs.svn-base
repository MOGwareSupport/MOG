using System;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Collections;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

using MOG.SYSTEM;
using MOG.PROJECT;
using MOG.PROMPT;
using MOG.PROPERTIES;
using MOG.CONTROLLER.CONTROLLERSYSTEM;

using MOG_ServerManager.Utilities;
using MOG_ControlsLibrary;
using System.ComponentModel;

namespace MOG_ServerManager
{
	/// <summary>
	/// Summary description for AssetTreeView.
	/// </summary>
	public class AssetTreeView : CodersLab.Windows.Controls.TreeView
	{
		#region Member vars
		private AssetTreeNode highlightedNode = null;
		private Color highlightedBackColor;
		private Color highlightedForeColor;
		private ContextMenuStrip contextMenu;
		private DragEventArgs dragEventArgs = null;

		private AssetTreeNode mRootNode = new AssetTreeNode("Project Repository");

		private ToolStripMenuItem miInsert;
		private ToolStripMenuItem miInsertAsSingleAsset;
		private ToolStripMenuItem miInsertDirStructure;
		private ToolStripMenuItem miAssignPlatform;
		private ToolStripMenuItem miCombineAsSingleAsset;

		private ContextMenuStrip cmRightDrag;

		private ArrayList platforms = new ArrayList( new string[] { "All", "PC", "XBox", "GameCube" } );

		private bool retainDirTree = false;


		private System.Windows.Forms.Timer expandNodeTimer = new System.Windows.Forms.Timer();
		private System.Windows.Forms.Timer scrollTimer = new System.Windows.Forms.Timer();
		//private bool timingExpand = false;
		private AssetTreeNode hoverNode = null;

		// my brother treeviews
		private DiskTreeView diskTreeView = null;
		private SelectedTreeView selectedTreeView = null;

		private ArrayList conflictedAssetList = null;
		#endregion
		#region Properties
		public DiskTreeView DiskTreeView
		{
			get { return this.diskTreeView; }
			set { this.diskTreeView = value; }
		}

		public SelectedTreeView SelectedTreeView
		{
			get { return this.selectedTreeView; }
			set { this.selectedTreeView = value; }
		}

		private bool bUseFileExtensionsInAssetNames;
		public bool UseFileExtensionsInAssetNames
		{
			get { return bUseFileExtensionsInAssetNames; }
			set { bUseFileExtensionsInAssetNames = value; }
		}

		public ArrayList Platforms
		{
			get { return this.platforms; }
			set 
			{
				this.platforms = value;
				if (!this.platforms.Contains("All"))
				{
					if (this.platforms.Count > 0)
						this.platforms.Insert(0, "All");
					else
						this.platforms.Add("All");
				}

				BuildContextMenu();
				this.ContextMenuStrip = this.contextMenu;
			}
		}

		public string RootName
		{
			get { return mRootNode.Text; }
			set { mRootNode.Text = value; }
		}
		#endregion
		#region Constants
		
		private const int FILE_INDEX			= 0;
		private const int OPENFOLDER_INDEX		= 1;
		private const int CLOSEDFOLDER_INDEX	= 2;
		private const int ARROW_INDEX			= 3;
		private const int ERROR_INDEX			= 4;
		private const int INFO_INDEX			= 5;
		private const int YELLOWARROW_INDEX		= 6;
		private const int QUESTION_INDEX		= 7;
		private const int BLUEDOT_INDEX			= 8;
		private const int WARNING_INDEX			= 9;

		private Color COLOR_CLASSIFICATION = Color.SaddleBrown;
		private Color COLOR_ASSETSUBFOLDER = Color.Purple;
		private Color COLOR_FILE = Color.RosyBrown;
		private Color COLOR_ASSETFILENAME = Color.Blue;
		
		#endregion
		#region Events
		public event EventHandler DroppedNodes;
		private void RaiseDroppedNodes()
		{
			if (this.DroppedNodes != null)
				this.DroppedNodes(this, new EventArgs());
		}
		#endregion
		#region Constructors
		public AssetTreeView()
		{
			this.LabelEdit = false;
			this.AllowDrop = true;
			this.SelectionMode = CodersLab.Windows.Controls.TreeViewSelectionMode.MultiSelect;

			// dragdrop
			this.ItemDrag += new ItemDragEventHandler(AssetTreeView_ItemDrag);
			this.DragEnter += new DragEventHandler(AssetTreeView_DragEnter);
			this.DragOver += new DragEventHandler(AssetTreeView_DragOver);
			this.DragDrop += new DragEventHandler(AssetTreeView_DragDrop);
			this.DragLeave += new EventHandler(AssetTreeView_DragLeave);

			this.KeyUp += new KeyEventHandler(AssetTreeView_KeyUp);
			this.AfterLabelEdit += new NodeLabelEditEventHandler(AssetTreeView_AfterLabelEdit);
			this.MouseDown += new MouseEventHandler(AssetTreeView_MouseDown);

			// expand timer
			this.expandNodeTimer = new System.Windows.Forms.Timer();
			this.expandNodeTimer.Tick += new EventHandler(AssetTreeView_ExpandNodeTimer_Tick);
			this.expandNodeTimer.Interval = 700;

			// scroll timer
			this.scrollTimer = new System.Windows.Forms.Timer();
			this.scrollTimer.Tick += new EventHandler(AssetTreeView_ScrollTimer_Tick);
			this.scrollTimer.Interval = 200;
			this.hoverNode = null;

			mRootNode = CreateClassificationNode("Project Repository");
			Nodes.Add( mRootNode );

			BuildContextMenu();
			this.ContextMenuStrip = this.contextMenu;

			this.miInsert = new ToolStripMenuItem("Insert here", null, new EventHandler(miInsert_Click));
			this.miInsertAsSingleAsset = new ToolStripMenuItem("Insert as one asset", null, new EventHandler(miInsertAsSingleAsset_Click));
			this.miInsertDirStructure = new ToolStripMenuItem("Insert directory tree as Classifications", null, new EventHandler(miInsertDirStructure_Click));
			
			this.cmRightDrag = new ContextMenuStrip();
			cmRightDrag.Items.Add(miInsert);
			cmRightDrag.Items.Add(miInsertAsSingleAsset);
			cmRightDrag.Items.Add(miInsertDirStructure);

			mRootNode.InAssetTree = true;
		}
		#endregion
		#region Adapted code from http://www.thecodeproject.com/cs/miscctrl/CustomAutoScrollPanel.asp
		// Horizontal scrollbar enum
		private const uint SB_HORZ = 0; 
		// Vertical scrollbar enum
		private const uint SB_VERT = 1; 

		[DllImport("user32.dll")]
		static protected extern int GetScrollRange(System.IntPtr handle, int typeOfSB, ref int pMinPos, ref int pMaxPos);
		static public int GetVScrollMin(TreeView tree)
		{
			int min = 0, max = 0;
			GetScrollRange(tree.Handle, (int)SB_VERT, ref min, ref max);
			return min;
		}
		static public int GetVScrollMax(TreeView tree)
		{
			int min = 0, max = 0;
			GetScrollRange(tree.Handle, (int)SB_VERT, ref min, ref max);
			return max;
		}
		
		// Get the scrollbar position for Control.Handle, handle
		[DllImport("user32.dll")]
		static protected extern int GetScrollPos(System.IntPtr handle, int typeOfSB);
		// Simplification of this method (for inheritance)
		static public int GetHScrollPosition(TreeView tree)
		{
			return GetScrollPos(tree.Handle, (int)SB_HORZ);
		}
		static public int GetVScrollPosition(TreeView tree)
		{
			return GetScrollPos(tree.Handle, (int)SB_VERT);
		}

		// Set the scrollbar position for Control.Handle, handle
		[DllImport("user32.dll")]
		static protected extern int SetScrollPos(System.IntPtr handle, int typeOfSB, int position, bool Redraw);
		// Simplification of this method (for inheritance)
		static public int SetHScrollPosition(TreeView tree, int position)
		{
			return SetScrollPos(tree.Handle, (int)SB_HORZ, position, true);
		}
		static public int SetVScrollPosition(TreeView tree, int position)
		{
			return SetScrollPos(tree.Handle, (int)SB_VERT, position, true);
		}
		#endregion Adapted code from http://www.thecodeproject.com/cs/miscctrl/CustomAutoScrollPanel.asp
		#region Public functions
		private void WriteProtect(AssetTreeNode tn)
		{
			tn.WriteProtected = true;
			foreach (AssetTreeNode subNode in tn.Nodes)
				WriteProtect(subNode);
		}

		public void LoadClassesFromProject(MOG_Project proj)
		{
			this.Nodes.Clear();

			foreach (AssetTreeNode tn in BuildClassTreeFromClassNames(GetClassList(proj)))
			{
				WriteProtect(tn);
				this.Nodes.Add(tn);
				tn.Expand();
			}
		}

		private ArrayList GetClassList(MOG_Project proj)
		{
			ArrayList classNames = new ArrayList();
			foreach (string className in proj.GetRootClassificationNames())
				classNames.AddRange( GetClassList_Helper(proj, className) );

			return classNames;
		}

		private ArrayList GetClassList_Helper(MOG_Project proj, string baseClassName)
		{
			ArrayList classNames = new ArrayList();
			classNames.Add(baseClassName);
			
			foreach (string className in proj.GetSubClassificationNames(baseClassName))
				classNames.AddRange( GetClassList_Helper(proj, baseClassName+"~"+className) );

			return classNames;
		}

		public void ResetEverything()
		{
			UnhighlightNode();
			this.Nodes.Clear();
		}

		public void RemoveNode(AssetTreeNode tn)
		{
			// Get the pointer to our other tree view now because we won't be able to after the selected nodes have been deleted
			SelectedTreeView treeView = ((SelectedTreeView)(tn.SelectedNode.TreeView));

			if (tn.IsAClassification)
			{
				DeleteClassificationNode(tn);
			}
			else if (tn.IsAnAssetFilename)
			{
				DeleteAssetFilenameNode(tn);
			}
			else if (tn.IsAFile)
			{
				AssetTreeNode parentNode = tn.Parent as AssetTreeNode;

				DeleteFileNode(tn);
				
				// if parentNode is an empty asset filename node
				if ( parentNode != null  &&  parentNode.IsAnAssetFilename  &&  parentNode.Nodes.Count == 0 )
					parentNode.Remove();
			}

			// Refresh the red status on the linked tree view
			treeView.RefreshRedStatus();
		}

		public void DeleteAllAssets()
		{
			ArrayList nodeList = GetAllAssetFilenameNodes_Recursive(this.Nodes);
			foreach (AssetTreeNode assetFilenameNode in nodeList)
				DeleteAssetFilenameNode(assetFilenameNode);

			if (this.selectedTreeView != null)
				this.selectedTreeView.RefreshRedStatus();

		}

		#region Default classification tree loader
		public bool LoadDefaultConfigurationsFromFiles(string infoPath)
		{
			if (!Directory.Exists(infoPath))
				return false;

			this.Nodes.Clear();

			string[] filenames = Directory.GetFiles(infoPath, "{ProjectName}*.info");
			
			// extract just the classification names from the filenames
			for (int i = 0; i < filenames.Length; i++)
			{
				filenames[i] = filenames[i].Replace("{ProjectName}", mRootNode.Text);
				filenames[i] = Path.GetFileNameWithoutExtension(filenames[i]);
			}

			foreach (AssetTreeNode tn in BuildClassTreeFromClassNames( new ArrayList(filenames) ))
				this.Nodes.Add(tn);

			if (this.Nodes.Count > 0)
			{
				mRootNode = this.Nodes[0] as AssetTreeNode;
				mRootNode.Expand();
			}

			return true;
		}

		private int CountTildes(string searchString)
		{
			int count = 0;

			foreach (char c in searchString.ToCharArray())
			{
				if (c == '~')
					++count;
			}

			return count;
		}

		private string GetParentClassName(string fullClassName)
		{
			if (fullClassName.IndexOf("~") == -1)
				return "";			// has no parent, so return empty string
			else
				return fullClassName.Substring( 0, fullClassName.LastIndexOf("~") );	// return everything before the last ~
		}

		private string GetLeafClassName(string fullClassName)
		{
			if (fullClassName.IndexOf("~") == -1)
				return fullClassName;		// this is a leaf (or a root), so return the full name
			else
				return fullClassName.Substring( fullClassName.LastIndexOf("~")+1 );		// return everything after the last ~
		}

		private ArrayList BuildClassTreeFromClassNames(ArrayList classNames)
		{
			ArrayList classNodes = new ArrayList();

			if (classNames == null)
				return classNodes;

			// now, sort them into ArrayLists based on how many tildes (~) they have
			int numTildes = 0;		// start with no tildes (i.e., the root classifications)
			ArrayList classLists = new ArrayList();
			while (classNames.Count > 0)
			{
				ArrayList classList = new ArrayList();
				foreach (string className in classNames)
				{
					if (CountTildes(className) == numTildes)
						classList.Add(className);
				}

				// now, remove all the class names with 'numTildes' tildes from allClassNames
				foreach (string className in classList)
					classNames.Remove(className);

				// add classList to our list of lists
				classLists.Add(classList);

				++numTildes;
			}


			//	private const int FILE_ICON_INDEX			= 0;
			//	private const int CLOSED_FOLDER_ICON_INDEX	= 1;
			//	private const int OPEN_FOLDER_ICON_INDEX	= 2;
			//	private const int DEFAULT_ASSET_ICON		= 3;

			// okay, now we have an ArrayList of ArrayLists, each of which contains the class names with the number of blah blah blah
			util_MapList nodeMap = new util_MapList();
			for (int i = 0; i < classLists.Count; i++)
			{
				ArrayList classList = (ArrayList)classLists[i];
				foreach (string className in classList)
				{
					if (i == 0)
					{
						// root nodes are a special case
						AssetTreeNode rootNode = CreateClassificationNode(className);

						//rootNode.ForeColor = Color.Red;
						nodeMap.Add(className.ToLower(), rootNode);		// add so we can look up the node later by its classification
						classNodes.Add(rootNode);						// add it to return value
					}
					else
					{
						// try to look up parent node
						string parentClassName = GetParentClassName(className);
						string classLeafName = GetLeafClassName(className);

						AssetTreeNode parentNode = nodeMap.Get( parentClassName.ToLower() ) as AssetTreeNode;

						if (parentNode != null)
						{
							// if we found a parent node, add a new node representing className to it, otherwise ignore
							AssetTreeNode classNode = CreateClassificationNode(classLeafName);

							parentNode.Nodes.Add(classNode);
							nodeMap.Add(className.ToLower(), classNode);		// add for lookup later (to add children to this node)
						}
					}
				}
			}
			
			return classNodes;
		}
		#endregion
		#endregion
		#region Private functions

		private string FixupAssetName(string text)
		{
			return MOG_ControllerSystem.ReplaceInvalidCharacters(text);
		}
		#region Utility functions

		// get the unique roots of nodes
		private ArrayList GetRootNodes(ArrayList nodes)
		{
			ArrayList roots = new ArrayList();
			ArrayList removeList = new ArrayList();

			// make a copy of nodes to work on
			foreach (AssetTreeNode tn in nodes)
				roots.Add(tn);

			bool iterate = true;
			while (iterate)
			{
				iterate = false;

				foreach (AssetTreeNode tn in roots)
				{
					// if tn's parent is in roots, then tn is not a root -- remove it
					if (tn.Parent != null  &&  roots.Contains(tn.Parent))
					{
						removeList.Add(tn);
						iterate = true;
					}
				}

				// remove those nodes marked for removal (those whose parents are also in roots)
				foreach (AssetTreeNode tn in removeList)
					roots.Remove(tn);
			}

			return roots;
		}

		private bool AllFilesArePlaced(ArrayList nodeList)
		{
			if (nodeList == null)
				return false;

			foreach (AssetTreeNode node in nodeList)
			{
				if (node.IsAFile  &&  node.AssetNode == null)
					return false;
				else if (node.IsAFolder)
				{
					// recurse
					foreach (AssetTreeNode subNode in node.Nodes)
					{
						ArrayList subNodeList = new ArrayList();
						subNodeList.Add(subNode);

						if (!AllFilesArePlaced(subNodeList))
							return false;
					}
				}
			}

			return true;
		}

		private ArrayList GetFlattenedNodes(ArrayList nodes)
		{
			ArrayList flatList = new ArrayList();
			
			if (nodes == null)
				return flatList;

			foreach (AssetTreeNode tn in nodes)
			{
				if (tn.IsAFile)
					flatList.Add(tn);
				else if (tn.IsAFolder)
					flatList.AddRange( GetFlattenedNodes(tn.Nodes) );
			}

			return flatList;
		}

		private ArrayList GetFlattenedNodes(TreeNodeCollection nodes)
		{
			ArrayList flatList = new ArrayList();
			
			if (nodes == null)
				return flatList;

			foreach (AssetTreeNode tn in nodes)
			{
				if (tn.IsAFile)
					flatList.Add(tn);
				else if (tn.IsAFolder)
					flatList.AddRange( GetFlattenedNodes(tn.Nodes) );
			}

			return flatList;
		}

		private ArrayList CopyExpandedState(TreeNodeCollection nodes)
		{
			ArrayList expandedList = new ArrayList();
			if (nodes == null)
				return expandedList;

			foreach (TreeNode tn in nodes)
			{
				if (tn.TreeView != null  &&  tn.IsExpanded)
					expandedList.Add(tn.FullPath);

				// recurse
				expandedList.AddRange( CopyExpandedState(tn.Nodes) );
			}

			return expandedList;
		}

		private void RestoreExpandedState(TreeNodeCollection nodes, ArrayList expandedList)
		{
			foreach (TreeNode tn in nodes)
			{
				if (expandedList.Contains(tn.FullPath))
					tn.Expand();

				RestoreExpandedState(tn.Nodes, expandedList);
			}
		}

		private string GetCommonStartString(ArrayList strings)
		{
			string commonStart = "";

			if (strings == null  ||  strings.Count <= 0)
				return "";

			if (!Utils.ArrayListTypeCheck(strings, typeof(string)))
				return "";

			string firstString = strings[0] as string;
			if (firstString == null)
				return "";

			int charIndex = 0;
			foreach (char c in firstString.ToLower().ToCharArray())
			{
				foreach (string s in strings)
				{
					if (s.Length <= charIndex  ||  s.ToLower()[charIndex] != c)
						return commonStart;		// if it didn't match, we're done
				}

				// add character
				commonStart += c;
				++charIndex;
			}

			return commonStart;
		}

		public void RebuildSubStructure(AssetTreeNode assetFilenameNode)
		{
			if (assetFilenameNode == null  ||  !assetFilenameNode.IsAnAssetFilename)
				return;

			// save expanded state for later restoration 
			bool afnExpanded = assetFilenameNode.IsExpanded;
			ArrayList expandedList = CopyExpandedState(assetFilenameNode.Nodes);

			// clear out old structure
			assetFilenameNode.Nodes.Clear();

			// build an ArrayList of the full filenames of the nodes to feed to GetCommonDirectoryPath()
			ArrayList filenames = new ArrayList();
			foreach (AssetTreeNode fileNode in assetFilenameNode.fileNodes)
				filenames.Add( fileNode.FileFullPath );

			string commonStart = MOG.CONTROLLER.CONTROLLERASSET.MOG_ControllerAsset.GetCommonDirectoryPath("", filenames);

			for (int i = 0; i < assetFilenameNode.fileNodes.Count; i++)
			{
				string filename = ((AssetTreeNode)assetFilenameNode.fileNodes[i]).FileFullPath;
				string path = "";
				path = Path.GetDirectoryName(filename).Substring(commonStart.Length);
				AssetTreeNode dirNode = AddDirStructureAsNodes(assetFilenameNode, path);
				if (dirNode == null)
					assetFilenameNode.Nodes.Add( (AssetTreeNode)assetFilenameNode.fileNodes[i] );
				else
					dirNode.Nodes.Add( (AssetTreeNode)assetFilenameNode.fileNodes[i] );
			}
			
			// restore expanded state
			RestoreExpandedState(assetFilenameNode.Nodes, expandedList);
			if (afnExpanded)
				assetFilenameNode.Expand();
			else
				assetFilenameNode.Collapse();
		}

		private AssetTreeNode AddDirStructureAsNodes(AssetTreeNode afNode, string path)
		{
			TreeNodeCollection curNodes = afNode.Nodes;
			AssetTreeNode dirNode = null;
			string[] pathElements = path.Trim("\\".ToCharArray()).Split("\\".ToCharArray());
			if (pathElements.Length == 1  &&  pathElements[0] == "")
				return null;

			foreach (string dirName in pathElements)
			{
				dirNode = GetSubnode(curNodes, dirName);
				if (dirNode == null)
				{
					dirNode = this.CreateAssetSubFolderNode(dirName);
					dirNode.AssetFilenameNode = afNode;
					curNodes.Add(dirNode);
				}

				curNodes = dirNode.Nodes;
			}

			return dirNode;
		}

		private AssetTreeNode GetSubnode(TreeNodeCollection nodes, string text)
		{
			foreach (AssetTreeNode tn in nodes)
			{
				if (tn.Text.ToLower() == text.ToLower())
					return tn;
			}

			return null;
		}

		#endregion

		private void RemoveFromTree(TreeNode tn)
		{
			if (tn != null  &&  tn.TreeView != null)
				tn.Remove();
		}

		private AssetTreeNode FindTargetNode(int x, int y)
		{
			return (GetNodeAt( PointToClient(new Point(x, y)) ) as AssetTreeNode);
		}


		#region Node-moving routines
		private void MoveNodes(ArrayList nodeList, AssetTreeNode targetNode)
		{
			if (nodeList == null  ||  nodeList.Count <= 0  ||  targetNode == null)
				return;

			// move each node in turn
			foreach (AssetTreeNode tn in nodeList)
			{
				if (tn == mRootNode)
					continue;		// can't add root node

				if (tn == targetNode)
					continue;		// can't add a node to itself

				if (tn.IsAClassification)
				{
					if (targetNode.IsAClassification)
					{
						// moving a classification to a classification
						RemoveFromTree(tn);
						targetNode.Nodes.Add(tn);
						tn.EnsureVisible();		// we're done
					}
					else if (targetNode.IsAnAssetFilename)
					{
						// moving a classification to an asset filename - no go
						continue;
					}
					else if (targetNode.IsAFile)
					{
						// moving a classification to a file - no go
						continue;
					}
					else if (targetNode.IsAnAssetSubFoler)
					{
						// moving a classification to an asset folder - no go
						continue;
					}
				}
				else if (tn.IsAnAssetFilename)
				{
					if (targetNode.IsAClassification)
					{
						// moving an asset filename to a classification
						RemoveFromTree(tn);
						targetNode.Nodes.Add(tn);
						tn.EnsureVisible();		// we're done
					}
					else if (targetNode.IsAnAssetFilename)
					{
						// moving an asset filename to an asset filename

						// reassign the assetFilename pointers on each of tn's file nodes
						foreach (AssetTreeNode fileNode in tn.fileNodes)
							fileNode.AssetFilenameNode = targetNode;
						
						// copy tn's file nodes to targetNode
						targetNode.fileNodes.AddRange(tn.fileNodes);

						// kill tn
						//if (tn.TreeView != null)
							tn.Remove();

						// rebuild targetNode's substructure
						this.RebuildSubStructure(targetNode);

						targetNode.Expand();
						targetNode.EnsureVisible();
					}
					else if (targetNode.IsAFile)
					{
						// moving an asset filename to a file - no go
						continue;
					}
					else if (targetNode.IsAnAssetSubFoler)
					{
						// moving an asset filename to an asset folder - no go
						continue;
					}
				}
				else if (tn.IsAFile)
				{
					// save the original parent node in case we have to remove it (if it's an empty asset filename node)
					AssetTreeNode parentNode = tn.AssetFilenameNode as AssetTreeNode;

					if (targetNode.IsAClassification)
					{
						// remove tn from its asset filename node and rebuild its substructure
						if (parentNode != null  &&  parentNode.fileNodes.Contains(tn))
							parentNode.fileNodes.Remove(tn);
						this.RebuildSubStructure(parentNode);

						// now repackage tn as a new asset filename node
						AssetTreeNode newAFNode = this.CreateAssetFilenameNode("{All}" + tn.Text);
						targetNode.Nodes.Add(newAFNode);
						newAFNode.fileNodes.Add(tn);
						tn.AssetFilenameNode = newAFNode;
						this.RebuildSubStructure(newAFNode);
						newAFNode.EnsureVisible();

						// do we need to remove tn's old parent? (i.e., is it an empty asset filename node?)
						if (parentNode != null  &&  parentNode.IsAnAssetFilename  &&  parentNode.fileNodes.Count == 0)
							RemoveFromTree(parentNode);
					}
					else if (targetNode.IsAnAssetFilename)
					{
						// moving a file to an asset filename

						// remove tn from its asset filename node
						if (parentNode != null  &&  parentNode.fileNodes.Contains(tn))
							parentNode.fileNodes.Remove(tn);
						this.RebuildSubStructure(parentNode);

						// now add tn to targetNode
						targetNode.fileNodes.Add(tn);
						tn.AssetFilenameNode = targetNode;

						this.RebuildSubStructure(targetNode);
						targetNode.Expand();
						targetNode.EnsureVisible();

						// do we need to remove tn's old parent? (i.e., is it an empty asset filename node?)
						if (parentNode != null  &&  parentNode.IsAnAssetFilename  &&  parentNode.fileNodes.Count == 0)
							RemoveFromTree(parentNode);
					}
					else if (targetNode.IsAFile)
					{
						// moving a file to a file - no go
						continue;
					}
					else if (targetNode.IsAnAssetSubFoler)
					{
						// moving a file to an asset folder - no go
						continue;
					}
				}
				else if (tn.IsAnAssetSubFoler)
				{
					if (targetNode.IsAClassification)
					{
						// moving an asset folder to a classification - no go
						continue;
					}
					else if (targetNode.IsAnAssetFilename)
					{
						// moving an asset folder to an asset filename - no go
						continue;
					}
					else if (targetNode.IsAFile)
					{
						// moving an asset folder to a file - no go
						continue;
					}
					else if (targetNode.IsAnAssetSubFoler)
					{
						// moving an asset folder to an asset folder - no go
						continue;
					}
				}
			}
			
		}
		#endregion

		#region Node deletion functions
		private void DeleteAssetFilenameNode(AssetTreeNode afNode)
		{
			if (!afNode.IsAnAssetFilename)
				return;

			ArrayList subNodes = new ArrayList();
			foreach (AssetTreeNode subNode in afNode.fileNodes)
				subNodes.Add(subNode);

			foreach (AssetTreeNode deleteNode in subNodes)
			{
				if (deleteNode.IsAFile)
					DeleteFileNode(deleteNode);
//				else if (deleteNode.IsAnAssetSubFoler)
//					DeleteAssetSubFolderNode(deleteNode);
			}

			RemoveFromTree(afNode);
		}

		private void DeleteAssetSubFolderNode(AssetTreeNode folderNode)
		{
			// Delete the selected list of tree nodes
			foreach (AssetTreeNode subNode in folderNode.Nodes)
			{
				if (subNode.IsAFile)
					DeleteFileNode(subNode);
				else if (subNode.IsAnAssetSubFoler)
					DeleteAssetSubFolderNode(subNode);		// recurse on subdirs
			}

			folderNode.Remove();
		}

		private void DeleteFileNode(AssetTreeNode fileNode)
		{
			if (fileNode.SelectedNode != null)
			{
				fileNode.SelectedNode.SetRed();
//				fileNode.SelectedNode.RefreshRedStatus();
				fileNode.SelectedNode.AssetNode = null;
				RemoveFromTree(fileNode);
			}
		}

		private void DeleteClassificationNode(AssetTreeNode classNode)
		{
			if (classNode.Nodes.Count > 0  &&  MOG_Prompt.PromptResponse("Confirm Classification Removal", "Are you sure you want to remove " + classNode.Text + " and all its subclassifications and contained assets?", MOGPromptButtons.YesNo) != MOGPromptResult.Yes)
				return;

			ArrayList nodeList = GetAllAssetFilenameNodes_Recursive(classNode.Nodes);
			foreach (AssetTreeNode assetFilenameNode in nodeList)
				DeleteAssetFilenameNode(assetFilenameNode);

			classNode.Remove();
		}
		#endregion

		#region Node-creating functions
		private AssetTreeNode CreateClassificationNode(string text)
		{
			text = FixupAssetName(text);
			AssetTreeNode classNode = new AssetTreeNode(text, ARROW_INDEX, COLOR_CLASSIFICATION);
			classNode.InAssetTree = true;
			classNode.IsAClassification = true;
			classNode.InDB = false;
			classNode.FileFullPath = "";

			return classNode;
		}

		private AssetTreeNode CreateAssetSubFolderNode(string text)
		{
			text = FixupAssetName(text);
			AssetTreeNode assetSubDirNode = new AssetTreeNode(text, CLOSEDFOLDER_INDEX, COLOR_ASSETSUBFOLDER);
			assetSubDirNode.InAssetTree = true;
			assetSubDirNode.IsAnAssetSubFoler = true;

			return assetSubDirNode;
		}

		private AssetTreeNode CreateFileNode(AssetTreeNode sourceNode)
		{
			// if sourceNode doesn't already have an associated asset node
			if (sourceNode.AssetNode == null)
			{
				string text = FixupAssetName(sourceNode.Text);
				AssetTreeNode fileNode = new AssetTreeNode(text, FILE_INDEX, COLOR_FILE);
				fileNode.InAssetTree = true;
				sourceNode.AssetNode = fileNode;
				fileNode.IsAFile = true;
				fileNode.FileFullPath = sourceNode.FileFullPath;
				fileNode.RelativePath = sourceNode.RelativePath;
				fileNode.SelectedNode = sourceNode;
				//				fileNode.IsAFile = sourceNode.IsAFile;
				//				fileNode.IsAFolder = sourceNode.IsAFolder;
				//				fileNode.IsAnAssetFilename = sourceNode.IsAnAssetFilename;
				//				fileNode.IsAnAssetSubFoler = sourceNode.IsAnAssetSubFoler;
				//				fileNode.IsAClassification = sourceNode.IsAClassification;

				return fileNode;
			}

			return null;
		}

		private AssetTreeNode CreateAssetFilenameNode(string text)
		{
			text = FixupAssetName(text);
			AssetTreeNode assetNameNode = new AssetTreeNode(text, QUESTION_INDEX, COLOR_ASSETFILENAME);
			assetNameNode.InAssetTree = true;
			assetNameNode.IsAnAssetFilename = true;
			return assetNameNode;
		}

		private AssetTreeNode CreateAssetFilenameNode(AssetTreeNode fileNode)
		{
			string text = FixupAssetName(fileNode.Text);
			AssetTreeNode assetNameNode = new AssetTreeNode("{All}" + text, QUESTION_INDEX, COLOR_ASSETFILENAME);
			assetNameNode.InAssetTree = true;
			assetNameNode.IsAnAssetFilename = true;
			return assetNameNode;
		}
		#endregion

		#region Node insertion functions
		private void InsertNodesAsSingleAsset(DragEventArgs args)
		{
			AssetTreePlacerDragObject dragObj = args.Data.GetData(typeof(AssetTreePlacerDragObject)) as AssetTreePlacerDragObject;

			AssetTreeNode targetNode = GetNodeAt( PointToClient(new Point(args.X, args.Y)) ) as AssetTreeNode;

			if (targetNode == null  ||  dragObj.nodeList.Count <= 0)
				return;

			// setup single asset filename node to add to
			AssetTreeNode assetNameNode = CreateAssetFilenameNode((AssetTreeNode)dragObj.nodeList[0]);
			targetNode.Nodes.Add(assetNameNode);

			foreach (AssetTreeNode tn in dragObj.nodeList)
			{
				// if the current node to be inserted is a file and hasn't already been inserted
				if (tn.IsAFile  &&  tn.AssetNode == null)
				{
					AssetTreeNode assetNode = this.CreateFileNode(tn);//new AssetTreeNode(tn.Text, BLUEDOT_INDEX, Color.RosyBrown);
					assetNode.AssetFilenameNode = assetNameNode;
//					tn.AssetNode = assetNode;
//					assetNode.SelectedNode = tn;
//					assetNode.IsAFile = tn.IsAFile;
//					assetNode.IsAFolder = tn.IsAFolder;
//					assetNode.IsAnAssetFilename = tn.IsAnAssetFilename;

					assetNameNode.fileNodes.Add(assetNode);

					tn.SetGreen();
					SelectedTreeView tv = tn.TreeView as SelectedTreeView;
					if (tv != null)
					{
						tv.RefreshRedStatus();
					}
				}
			}

			RebuildSubStructure(assetNameNode);
			assetNameNode.EnsureVisible();
			assetNameNode.Expand();
		}

		private void InsertNodes(DragEventArgs args)
		{
			// get the dragged object
			AssetTreePlacerDragObject dragObj = args.Data.GetData(typeof(AssetTreePlacerDragObject)) as AssetTreePlacerDragObject;

			// find the target node
			AssetTreeNode targetNode = GetNodeAt( PointToClient(new Point(args.X, args.Y)) ) as AssetTreeNode;

			if (targetNode == null)
				return;

			foreach (AssetTreeNode tn in dragObj.nodeList)
			{
				if (tn.IsAFile  &&  tn.AssetNode == null)
				{
					if (targetNode.IsAnAssetFilename)
					{
						AssetTreeNode fileNode = CreateFileNode(tn);//new AssetTreeNode(tn.Text, BLUEDOT_INDEX, Color.RosyBrown);
						tn.AssetNode = fileNode;
						fileNode.SelectedNode = tn;
						targetNode.fileNodes.Add(fileNode);
						this.RebuildSubStructure(targetNode);
						targetNode.EnsureVisible();
						targetNode.Expand();
					}
					else if (targetNode.IsAClassification)
					{
						// put tn in a new asset filename node
						AssetTreeNode AFNode = this.CreateAssetFilenameNode("{All}" + tn.Text);
						targetNode.Nodes.Add(AFNode);

						// now setup the file node
						AssetTreeNode fileNode = CreateFileNode(tn);//new AssetTreeNode(tn.Text, BLUEDOT_INDEX, Color.RosyBrown);
						tn.AssetNode = fileNode;
						fileNode.SelectedNode = tn;
						fileNode.AssetFilenameNode = AFNode;
						AFNode.fileNodes.Add(fileNode);
						this.RebuildSubStructure(AFNode);

						AFNode.EnsureVisible();
					}

					tn.SetGreen();
					SelectedTreeView tv = tn.TreeView as SelectedTreeView;
					if (tv != null)
					{
						tv.RefreshRedStatus();
					}
				}
			}

			targetNode.Expand();
			
		}

		private void InsertFlattenedNodes(DragEventArgs args)
		{
			// make sure we've got some drag data
			if (!args.Data.GetDataPresent(typeof(AssetTreePlacerDragObject)))
				return;

			AssetTreeNode targetNode = FindTargetNode(args.X, args.Y);
			
			if (targetNode == null)
				return;

			if (targetNode.IsAFile  ||  targetNode.IsAnAssetSubFoler)
				targetNode = targetNode.AssetFilenameNode;

			if (targetNode == null  ||  !targetNode.IsAnAssetFilename)
				return;

			// extract the drag data
			AssetTreePlacerDragObject dragObj = args.Data.GetData(typeof(AssetTreePlacerDragObject)) as AssetTreePlacerDragObject;

			// "flatten" the node list - i.e., throw out its directory structure while retaining all the file nodes
			ArrayList flattenedNodeList = GetFlattenedNodes(dragObj.nodeList);

			foreach (AssetTreeNode tn in flattenedNodeList)
			{
				if (tn.AssetNode == null)
				{
					tn.SetGreen();
					SelectedTreeView tv = tn.TreeView as SelectedTreeView;
					if (tv != null)
					{
						tv.RefreshRedStatus();
					}

					AssetTreeNode newFileNode = this.CreateFileNode(tn);
					newFileNode.AssetFilenameNode = targetNode;
					targetNode.fileNodes.Add( newFileNode );
				}
			}

			// now rebuild targetNode's sub-structure
			RebuildSubStructure(targetNode);
			targetNode.EnsureVisible();
			targetNode.Expand();
		}

		private void InsertNodesWithDirTree(AssetTreeNode targetNode, ArrayList nodes)
		{
			if (targetNode == null)
				return;

			// reset conflict list
			this.conflictedAssetList = new ArrayList();

			// and do the insert
			BeginUpdate();
			InsertNodesWithDirTree_Helper(targetNode, nodes);
			EndUpdate();

			// resolve asset conflicts
			if (conflictedAssetList.Count > 0)
			{
				int conflictIndex = 0;

				// generate message
				string msg = "The following files could not be placed as assets because they conflicted with existing assets:\n\n";
				for (conflictIndex = 0; conflictIndex < this.conflictedAssetList.Count; conflictIndex += 2)
					msg += "\t" + ((AssetTreeNode)this.conflictedAssetList[conflictIndex]).FileFullPath + "\n";
				msg += "\n\nWould you like to overwrite the existing assets?";
				
				if (Utils.ShowMessageBoxConfirmation(msg, "Resolve Asset Conflicts") == MOGPromptResult.Yes)
				{
					// perform overwrite
					for (conflictIndex = 0; conflictIndex < conflictedAssetList.Count; conflictIndex += 2)
					{
						AssetTreeNode fileNode = this.conflictedAssetList[conflictIndex] as AssetTreeNode;
						AssetTreeNode assetNode = this.conflictedAssetList[conflictIndex+1] as AssetTreeNode;

						AssetTreeNode parent = assetNode.Parent as AssetTreeNode;
						DeleteAssetFilenameNode(assetNode);

						fileNode.SetGreen();

						// add each file as its own asset filename
						AssetTreeNode newAssetNode = CreateAssetFilenameNode("{All}"+fileNode.Text);
						AssetTreeNode newFileNode = CreateFileNode(fileNode);
						newFileNode.AssetFilenameNode = newAssetNode;
						newAssetNode.fileNodes.Add(newFileNode);
						parent.Nodes.Add(newAssetNode);
						RebuildSubStructure(newAssetNode);
					}
				}
			}

			if (this.selectedTreeView != null)
				this.selectedTreeView.RefreshRedStatus();

			targetNode.Expand();
		}

		private AssetTreeNode GetClassificationChild(AssetTreeNode parent, string className)
		{
			foreach (AssetTreeNode subNode in parent.Nodes)
			{
				if (subNode.IsAClassification  &&  subNode.Text.ToLower() == className.ToLower())
					return subNode;
			}

			return null;
		}

		private AssetTreeNode GetAssetFilenameChild(AssetTreeNode parent, string assetName)
		{
			foreach (AssetTreeNode subNode in parent.Nodes)
			{
				if (subNode.IsAnAssetFilename  &&  subNode.Text.ToLower() == assetName.ToLower())
					return subNode;
			}

			return null;
		}

		private AssetTreeNode FindExistingAssetNode(AssetTreeNode parent, string assetName)
		{
			//Search the children of this parent for the given assetName
			foreach (AssetTreeNode child in parent.Nodes)
			{
				//Check each child
				if (Regex.IsMatch(child.Text, @"^\{.+\}" + assetName + @"(\..*)?", RegexOptions.IgnoreCase))
				{
					return child;
				}
			}

			return null;
		}

		private void InsertNodesWithDirTree_Helper(AssetTreeNode targetNode, ArrayList rootNodes)
		{
			bool ParentHasSyncTarget = false;
			bool AllAssetNodesHaveSameSyncTarget = true;
			string commonFileFullPath = null;

			if (targetNode.FileFullPath != null && targetNode.FileFullPath.Length > 0)
			{
				ParentHasSyncTarget = true;
			}

			foreach (AssetTreeNode tn in rootNodes)
			{
				if (tn.IsAFolder  &&  !AllFilesArePlaced(new ArrayList( new AssetTreeNode[] {tn} )))
				{
					// create a classification node
					AssetTreeNode classNode = GetClassificationChild(targetNode, tn.Text);
					if (classNode == null)
					{
						// if there was no previously existing class node called tn.Text, create one
						classNode = CreateClassificationNode(tn.Text);
						classNode.SelectedNode = tn;
						classNode.FileFullPath = tn.FileFullPath;
					
						tn.AssetNode = classNode;
						classNode.AutoGenerated = true;
						targetNode.Nodes.Add(classNode);
					}

					// create an ArrayList of tn's subnodes for InsertNodesWithDirTree_Helper
					ArrayList subNodes = new ArrayList();
					foreach (AssetTreeNode subNode in tn.Nodes)
						subNodes.Add(subNode);
					
					// ..and recurse
					InsertNodesWithDirTree_Helper(classNode, subNodes);
					
					if (tn.IsExpanded)
						classNode.Expand();
				}
				else if (tn.IsAFile  &&  tn.AssetNode == null)
				{
					// Generate the assetLabel & assetName (Remove all invalid characters)
					string assetLabel = MOG_ControllerSystem.ReplaceInvalidCharacters(tn.Text);
					string assetName = "{All}" + assetLabel;

					// Check if this asset already exists in the tree?
					AssetTreeNode assetFilenameNode = GetAssetFilenameChild(targetNode, assetName);
					if (assetFilenameNode == null)
					{
						// Update the color indicating this has been added to the third tree
						tn.SetGreen();

						// Strip off filename extensions here
						string assetLabelNoExtension = assetLabel;
						int pos = assetLabelNoExtension.LastIndexOf('.');
						if (pos != -1)
						{
							assetLabelNoExtension = assetLabelNoExtension.Substring(0, pos);
						}

						// Check if this is a library asset?
						bool bIsInLibrary = false;
						if (targetNode.TreeView != null)
						{
							string fullPath = targetNode.FullPath + targetNode.TreeView.PathSeparator;
							string testPath = targetNode.TreeView.PathSeparator + "Library" + targetNode.TreeView.PathSeparator;
							if (fullPath.IndexOf(testPath, 0, StringComparison.CurrentCultureIgnoreCase) != -1)
							{
								bIsInLibrary = true;
							}
						}
						AssetTreeNode matchNode = FindExistingAssetNode(targetNode, assetLabelNoExtension);
						if (matchNode == null && 
							!UseFileExtensionsInAssetNames &&
							!bIsInLibrary)
						{
							//The user has requested no extensions and there is no preexisting asset with this same name, STRIP!!!
							assetName = "{All}" + assetLabelNoExtension;
						}
						else if (matchNode != null && matchNode.Nodes.Count > 0)
						{
							//There was a node with the same name and no label, so we're going to put the extension back onto it
							string platform = "{All}";

							//Since we're renaming the node, try to get the already existing platform out of it, default {All} if you can't find it
							Match match = Regex.Match(matchNode.Text, @"^\{.+\}", RegexOptions.IgnoreCase);
							if (match != null)
							{
								//We successfully read the existing platform out of the node, so let's use that
								platform = match.Value;
							}

							//Rename the node using the platform we read and the filename of the file underneath this node
							matchNode.Text = platform + matchNode.Nodes[0].Text;
						}
						
						assetFilenameNode = CreateAssetFilenameNode(assetName);
						AssetTreeNode fileNode = CreateFileNode(tn);
						fileNode.AssetFilenameNode = assetFilenameNode;
						assetFilenameNode.fileNodes.Add(fileNode);
						targetNode.Nodes.Add(assetFilenameNode);
						this.RebuildSubStructure(assetFilenameNode);

						//Check if the target parent node is missing a FileFullPath
						if (!ParentHasSyncTarget && AllAssetNodesHaveSameSyncTarget)
						{
							string testPath = Path.GetDirectoryName(tn.FileFullPath);

							if (commonFileFullPath != null)
							{
								if (String.Compare(commonFileFullPath, testPath, true) != 0)
								{
									AllAssetNodesHaveSameSyncTarget = false;
								}
							}

							commonFileFullPath = testPath;
						}
					}
					else
					{
						// an asset filename node already existed (i.e., there is a conflict) - so add to the conflict list to be resolved later
						this.conflictedAssetList.Add(tn);
						this.conflictedAssetList.Add(assetFilenameNode);
					}
				}
			}

			//Do all the nodes have the same sync target?
			if (AllAssetNodesHaveSameSyncTarget && commonFileFullPath != null)
			{
				//It is fairly safe to assume that we can give this sync target to the parent classification because all the assets have the same one
				targetNode.FileFullPath = commonFileFullPath;
			}
		}

		private void InsertDirStructure(DragEventArgs args)
		{
			AssetTreePlacerDragObject dragObj = args.Data.GetData(typeof(AssetTreePlacerDragObject)) as AssetTreePlacerDragObject;

			AssetTreeNode targetNode = GetNodeAt( PointToClient(new Point(args.X, args.Y)) ) as AssetTreeNode;

			if (targetNode != null  &&  !targetNode.IsAFile  &&  !targetNode.IsAnAssetFilename)
			{
				foreach (AssetTreeNode tn in dragObj.nodeList)
				{
					AssetTreeNode classNode = CloneAsClassificationTree(tn);
					if (classNode != null)
						targetNode.Nodes.Add( classNode );
				}

				targetNode.Expand();
			}
		}

//		private void InsertDirStructureAsAssetSubDirs(AssetTreeNode targetNode, TreeNodeCollection nodes)
//		{
//			ArrayList nodeList = new ArrayList();
//			foreach (AssetTreeNode tn in nodes)
//				nodeList.Add(tn);
//
//			InsertDirStructureAsAssetSubDirs(targetNode, nodeList);
//		}
//		private void InsertDirStructureAsAssetSubDirs(AssetTreeNode targetNode, ArrayList nodes)
//		{
//			// asset subdirectories can only be added to asset filename nodes or to other asset subdirectories
//			if (targetNode.IsAnAssetFilename  ||  targetNode.IsAnAssetSubFoler)
//			{
//				foreach (AssetTreeNode tn in nodes)
//				{
//					if (tn.IsAFolder)
//					{
//						AssetTreeNode assetSubDirNode = CreateAssetSubFolderNode(tn.Text);
//						targetNode.Nodes.Add( assetSubDirNode );
//
//						// recurse
//						InsertDirStructureAsAssetSubDirs(assetSubDirNode, tn.Nodes);
//					}
//					else if (tn.IsAFile)
//					{
//						tn.SetGreen();
//						tn.RefreshRedStatus();
//
//						AssetTreeNode fileNode = CreateFileNode(tn);
//						targetNode.Nodes.Add(fileNode);
//					}
//				}
//
//				targetNode.Expand();
//			}
//		}
		#endregion

		private void ResetColors()
		{
			this.SelectedNodes.Clear();
			ResetColors(this.Nodes);
		}

		private void ResetColors(TreeNodeCollection nodes)
		{
			//			private Color COLOR_CLASSIFICATION = Color.SaddleBrown;
			//		private Color COLOR_ASSETSUBFOLDER = Color.Purple;
			//		private Color COLOR_FILE = Color.RosyBrown;
			//		private Color COLOR_ASSETFILENAME = Color.Blue;

			foreach (AssetTreeNode tn in nodes)
			{
				tn.BackColor = this.BackColor;

				if (tn.IsAClassification)
					tn.ForeColor = COLOR_CLASSIFICATION;
				else if (tn.IsAnAssetSubFoler)
					tn.ForeColor = COLOR_ASSETSUBFOLDER;
				else if (tn.IsAFile)
					tn.ForeColor = COLOR_FILE;
				else if (tn.IsAnAssetFilename)
					tn.ForeColor = COLOR_ASSETFILENAME;

				// recurse
				ResetColors(tn.Nodes);
			}
		}

		private AssetTreeNode CloneAsClassificationTree(AssetTreeNode tn)
		{
			if (tn.IsAFolder)
			{
				AssetTreeNode newClass = this.CreateClassificationNode(tn.Text);
				newClass.AutoGenerated = true;
				newClass.FileFullPath = tn.FileFullPath;
				//tn.AssetNode = newClass;

				foreach (AssetTreeNode subNode in tn.Nodes)
				{
					AssetTreeNode child = CloneAsClassificationTree(subNode);
					if (child != null)
						newClass.Nodes.Add(child);
				}

				return newClass;

			}
			else
				return null;
		}

		private ArrayList GetAllAssetFilenameNodes_Recursive(TreeNodeCollection nodes)
		{
			ArrayList assetFilenameNodes = new ArrayList();
			if (nodes == null)
				return assetFilenameNodes;

			foreach (AssetTreeNode subNode in nodes)
			{
				// add it to the list if it's an asset filename node
				if (subNode.IsAnAssetFilename)
					assetFilenameNodes.Add(subNode);

				// recurse
				assetFilenameNodes.AddRange( GetAllAssetFilenameNodes_Recursive(subNode.Nodes) );
			}

			return assetFilenameNodes;
		}

		private void RefreshAssignToPlatformSubmenu()
		{
			this.miAssignPlatform = new ToolStripMenuItem("Assign to platform");
			foreach (string platformName in this.platforms)
			{
				miAssignPlatform.DropDownItems.Add(new ToolStripMenuItem(platformName, null, new EventHandler(miAssignPlatform_Click)));
			}
		}

		private void BuildContextMenu()
		{
			RefreshAssignToPlatformSubmenu();
			
			this.miCombineAsSingleAsset = new ToolStripMenuItem("Combine as single asset");

			this.contextMenu = new ContextMenuStrip();
			this.contextMenu.Items.Add(new ToolStripMenuItem("Add Classification", null, new EventHandler(miAddClassification_Click), Keys.Control | Keys.N));
			this.contextMenu.Items.Add(new ToolStripMenuItem("Add Sibling", null, new EventHandler(miAddSibling_Click), Keys.Control | Keys.S));
			this.contextMenu.Items.Add(new ToolStripMenuItem("Rename", null, new EventHandler(miRename_Click), Keys.F2));
			this.contextMenu.Items.Add(new ToolStripMenuItem("Delete", null, new EventHandler(miDelete_Click)));
			this.contextMenu.Items.Add(new ToolStripSeparator());
			this.contextMenu.Items.Add(this.miAssignPlatform);
			this.contextMenu.Items.Add(this.miCombineAsSingleAsset);
			this.contextMenu.Items.Add(new ToolStripSeparator());
			this.contextMenu.Items.Add(new ToolStripMenuItem("Retain directory structure as Classification tree", null, new EventHandler(miRetainDirTree_Click)));
			this.contextMenu.Items.Add(new ToolStripMenuItem("Show files", null, new EventHandler(miShowFiles_Click)));

			this.contextMenu.Opening += ContextMenu_Popup;
		}

		private void HighlightNode(AssetTreeNode tn)
		{
			UnhighlightNode();
			
			this.highlightedNode = tn;
			this.highlightedForeColor = tn.ForeColor;
			this.highlightedBackColor = tn.BackColor;

			tn.BackColor = SystemColors.Highlight;
			tn.ForeColor = SystemColors.HighlightText;
		}

		private void UnhighlightNode()
		{
			if (this.highlightedNode != null)
			{
				this.highlightedNode.ForeColor = this.highlightedForeColor;
				this.highlightedNode.BackColor = this.highlightedBackColor;
			}
		}
		#endregion
		#region Event handlers

		#region Drag and drop functionality

		private void AssetTreeView_ItemDrag(object sender, ItemDragEventArgs args)
		{
			ArrayList nodeList = new ArrayList();
			foreach (AssetTreeNode tn in this.SelectedNodes)
				nodeList.Add(tn);

			AssetTreePlacerDragObject dragObj = new AssetTreePlacerDragObject(AssetTreePlacerDragObject.DragObjectSource.ASSETTREEVIEW);
			dragObj.button = args.Button;
			dragObj.nodeList = nodeList;

			this.DoDragDrop(dragObj, DragDropEffects.Copy);
		}

		private void AssetTreeView_DragEnter(object sender, DragEventArgs args)
		{
			if (args.Data.GetDataPresent(typeof(AssetTreePlacerDragObject)))
			{
				args.Effect = args.AllowedEffect;
			}
		}

		private void AssetTreeView_DragOver(object sender, DragEventArgs args)
		{
			// scroll if necessary
			Point p = PointToClient(new Point(args.X, args.Y));

			if (p.Y < 10 || p.Y > Height-10)
			{
				if (!scrollTimer.Enabled)
				{
					//This is almost working, but not quite so diabled it for now. JWW 5/2/06
					//scrollTimer.Start();
				}
			}
			
			AssetTreeNode tn = GetNodeAt(p) as AssetTreeNode;

			// hover node?
			if (this.hoverNode != tn)
			{
				hoverNode = tn;
				
				expandNodeTimer.Stop();
				expandNodeTimer.Start();
			}

			if (tn == null)
			{
				args.Effect = DragDropEffects.None;
				UnhighlightNode();
			}
			else
			{
				args.Effect = args.AllowedEffect;
				HighlightNode(tn);
			}
		}

		private void AssetTreeView_DragLeave(object sender, EventArgs args)
		{
			// kill the hover expander
			this.hoverNode = null;
			this.expandNodeTimer.Stop();
			this.scrollTimer.Stop();
		}

		private void AssetTreeView_DragDrop(object sender, DragEventArgs args)
		{
			AssetTreePlacerDragObject dragObj = args.Data.GetData(typeof(AssetTreePlacerDragObject)) as AssetTreePlacerDragObject;
			
			if (dragObj == null)
				return;

			Cursor.Current = Cursors.WaitCursor;

			this.expandNodeTimer.Stop();
			this.scrollTimer.Stop();
			this.hoverNode = null;

			if (dragObj.source == AssetTreePlacerDragObject.DragObjectSource.SELECTTREEVIEW  ||  dragObj.source == AssetTreePlacerDragObject.DragObjectSource.DISKTREEVIEW)
			{
				// source is the selected treeview
				if (dragObj.button == MouseButtons.Right)
				{
					this.dragEventArgs = args;
					this.cmRightDrag.Show(this, PointToClient(new Point(args.X, args.Y)));
				}
				else
				{
					ArrayList selectedNodes = new ArrayList();
					
					// place nodes in selected tree
					foreach (AssetTreeNode tn in dragObj.nodeList)
					{
						if (!tn.Checked)
						{
							tn.Checked = true;
						}
					}

					ArrayList roots = AssetTreeNode.GetUniqueRootNodes(dragObj.nodeList);

					foreach (AssetTreeNode node in roots)
					{
						if (dragObj.source == AssetTreePlacerDragObject.DragObjectSource.SELECTTREEVIEW)
							selectedNodes.Add(node);
						else
							selectedNodes.Add(node.SelectedNode);
					}

					AssetTreeNode targetNode = FindTargetNode(args.X, args.Y);
					if (targetNode != null)
					{
						if (targetNode.IsAClassification)
						{
							InsertNodesWithDirTree(targetNode, selectedNodes);
						}
						else// if (targetNode.IsAnAssetFilename)
						{
							InsertFlattenedNodes(args);
							//InsertDirStructureAsAssetSubDirs(targetNode, selectedNodes);
						}
					}
				}
			}
			else if (dragObj.source == AssetTreePlacerDragObject.DragObjectSource.ASSETTREEVIEW)
			{
				// source is this AssetTreeView
				AssetTreeNode targetNode = FindTargetNode(args.X, args.Y);

				if (targetNode != null)
				{
					this.SelectedNodes.Clear();
					try
					{
						// get the unique roots to avoid strange results...
						MoveNodes(GetRootNodes(dragObj.nodeList), targetNode);
					}
					catch (Exception e)
					{
						MOG_Prompt.PromptResponse("Error", e.Message, e.StackTrace, MOGPromptButtons.OK, MOG_ALERT_LEVEL.CRITICAL);
					}
				}
			}

			ResetColors();
			RaiseDroppedNodes();

			Cursor.Current = Cursors.Default;
		}

		#endregion

		private void AssetTreeView_ExpandNodeTimer_Tick(object sender, EventArgs args)
		{
			expandNodeTimer.Stop();

			System.Console.Out.WriteLine("ExpandNodeTimerTick");

			if (hoverNode != null  &&  hoverNode.TreeView != null)
			{
				if (!hoverNode.IsExpanded)
					hoverNode.Expand();
			}
		}

		private void AssetTreeView_ScrollTimer_Tick(object sender, EventArgs args)
		{
			System.Console.Out.WriteLine("ScrollTimerTick");
			int vpos = GetVScrollPosition(this);
			int vmin = GetVScrollMin(this);
			int vmax = GetVScrollMax(this);
			Point p = PointToClient(Cursor.Position);

			if (p.Y < 10)
			{
				vpos -= 10;
				if (vpos < vmin)
					vpos = vmin;
				SetVScrollPosition(this, vpos);
			}
			else if (p.Y > Height-10)
			{
				vpos += 10;
				if (vpos > vmax)
					vpos = vmax;
				SetVScrollPosition(this, vpos);
			}
			else
			{
				scrollTimer.Stop();
			}
		}

		private void AssetTreeView_MouseDown(object sender, MouseEventArgs args)
		{
			TreeNode clickedNode = GetNodeAt( PointToClient(new Point(args.X, args.Y)) );
			if (this.SelectedNodes.Contains(clickedNode))
			{
				this.LabelEdit = true;
				clickedNode.BeginEdit();
			}
		}

		private void AssetTreeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs args)
		{
			AssetTreeNode node = args.Node as AssetTreeNode;

			// Check if thisis a valid Lable?
			if (args.Label == null || args.Label == "")
			{
				args.CancelEdit = true;
			}
			// Check if this is the root node?
			else if (args.Node.Parent == null)
			{
				// Inform the user they can't change the root node
				MOG_Prompt.PromptMessage("Cannot Change Root Classification", "The root classification node is reserved and must correspond with the project name.");
				args.CancelEdit = true;
			}
			// Check if this node is an asset?
			else if (node.IsAFile)
			{
				// Inform the user they can't change the root node
				MOG_Prompt.PromptMessage("Cannot Change Filename", "The filename is a result of the file being imported and cannot be changed.");
				args.CancelEdit = true;
			}
			// Check if this node is an asset?
			else if (node.IsAnAssetFilename)
			{
				// Check if the new name is missing a platform identifier?
				if (!args.Label.StartsWith("{") || 
					 args.Label.IndexOf("}") == -1 || 
					 args.Label.IndexOf("}") == args.Label.Length - 1)
				{
					// Inform the user this was an invalid asset name
					MOG_Prompt.PromptMessage("Invalid Asset Name", "Asset names must always begin with a '{Platform}' specifier.");
					args.CancelEdit = true;
				}
			}

			// Make sure we can still proceed with the change?
			if (args.CancelEdit == false)
			{
				if (node.IsAClassification && node.FileFullPath.Length > 0)
				{
					MOG_Prompt.PromptMessage("Renaming a Classification", "Although you have just renamed this classification, MOG will retain the original SyncTarget so the directory structure of the assets within this classification will be preserved.", Environment.StackTrace, MOG_ALERT_LEVEL.MESSAGE);
				}

				// Update the node's text
				args.Node.Text = args.Label;
			}

			// Turn back off the edit mode
			this.LabelEdit = false;
		}

		private void ContextMenu_Popup(object sender, CancelEventArgs args)
		{
			//if (this.SelectedNodes.Count > 0)
			//	this.contextMenu.Items.Add(0, new ToolStripMenuItem( (((AssetTreeNode)this.SelectedNodes[0]).IsAClassification) ? "Class" : "Not a Class" ));

			// rebuild the combine as single asset submenu
			this.miCombineAsSingleAsset.DropDownItems.Clear();
			foreach (AssetTreeNode tn in this.SelectedNodes)
			{
				if (tn.IsAnAssetFilename)
					this.miCombineAsSingleAsset.DropDownItems.Add(new ToolStripMenuItem( tn.Text, null, new EventHandler(miCombineAsSingleAsset_Click) ));
			}
			if (this.miCombineAsSingleAsset.DropDownItems.Count <= 1)
				this.miCombineAsSingleAsset.Enabled = false;
			else
				this.miCombineAsSingleAsset.Enabled = true;
		}

		private void miAddSibling_Click(object sender, EventArgs args)
		{
			if (this.SelectedNodes.Count <= 0)
				return;

			AssetTreeNode clickedNode = this.SelectedNodes[0] as AssetTreeNode;
			
			if ( clickedNode != null  &&  clickedNode.IsAClassification  &&  clickedNode.Parent != null )
			{
				AssetTreeNode newNode = this.CreateClassificationNode("NewClassification");
				clickedNode.Parent.Nodes.Add(newNode);
				newNode.EnsureVisible();
				this.SelectedNodes.Clear();
				this.SelectedNodes.Add(newNode);
				this.LabelEdit = true;
				newNode.BeginEdit();
			}
		}

		private void miDelete_Click( object sender, EventArgs args)
		{
			// We have to do a lot of dancing around here to avoid removing nodes improperly and causing the treeview to throw exceptions, so bear with me...

			ArrayList prevNodes = new ArrayList();
			ArrayList nodesToDelete = new ArrayList();

			foreach (AssetTreeNode tn in this.SelectedNodes)
			{
				if (tn.IsEditing)
					return;				//We're just deleting a letter here, not the whole node!!!!

				if (tn == mRootNode)
					continue;			// can't delete the root node

				if (tn.WriteProtected)
					continue;			// can't delete a write-protected node

				nodesToDelete.Add(tn);
					
				if (tn.NextNode != null  &&  !prevNodes.Contains(tn.NextNode))
					prevNodes.Add(tn.NextNode);
				if (tn.PrevNode != null  &&  !prevNodes.Contains(tn.PrevNode))
					prevNodes.Add(tn.PrevNode);
				//					prevNodes.Add(tn.Parent);
			}

			if (nodesToDelete.Count == 0)
				return;

			this.SelectedNodes.Clear();

			foreach (AssetTreeNode tn in nodesToDelete)
			{
				//this.SelectedNodes.Remove(tn);

				if (tn.IsAnAssetFilename)
				{
					DeleteAssetFilenameNode(tn);
				}
				else if (tn.IsAFile)
				{
					// save parent in case we need to delete it later (if tn is its only child)
					AssetTreeNode parentNode = tn.AssetFilenameNode;

					// if parentNode is an empty asset filename node
					if ( parentNode != null  &&  parentNode.IsAnAssetFilename )
					{
						if (parentNode.fileNodes.Contains(tn))
						{
							parentNode.fileNodes.Remove(tn);
							tn.SelectedNode.SetRed();
							SelectedTreeView tv = tn.SelectedNode.TreeView as SelectedTreeView;
							if (tv != null)
							{
								tv.RefreshRedStatus();
							}
							tn.SelectedNode.AssetNode = null;
							RebuildSubStructure(parentNode);
						}

						if (parentNode.fileNodes.Count == 0)
							parentNode.Remove();
					}
				}
				else if (tn.IsAClassification)
				{
					DeleteClassificationNode(tn);
				}
			}

			// Make sure to clear our selected nodes because we just deleted them
			this.SelectedNodes.Clear();
			foreach (TreeNode p in prevNodes)
			{
				if (p != null  &&  !nodesToDelete.Contains(p)  &&  p.TreeView == this)
				{
					this.SelectedNodes.Add(p);
					break;
				}
			}

			this.Invalidate();
			Application.DoEvents();
			//				foreach (AssetTreeNode prevNode in prevNodes)
			//				{
			//					if (prevNode.TreeView != null)
			//					{
			//						this.SelectedNodes.Add(prevNode);
			//						break;
			//					}
			//				}

			//this.SelectedNodes.Add(mRootNode);

			//				prevNodes.Clear();
			//
			// refresh selected treeview's red status (if we have a pointer to it)
			if (this.selectedTreeView != null)
				this.selectedTreeView.RefreshRedStatus();

			ResetColors();
		}

		private void miAddClassification_Click(object sender, EventArgs args)
		{
			if (this.SelectedNodes.Count <= 0)
			{
				//AssetTreeNode newNode = new AssetTreeNode("NewClassification", ARROW_INDEX, Color.YellowGreen);
				//this.Nodes.Add(newNode);
				//newNode.BeginEdit();
			}
			else if ( !((AssetTreeNode)this.SelectedNodes[0]).IsAnAssetFilename  &&  !((AssetTreeNode)this.SelectedNodes[0]).IsAFile )
			{
				AssetTreeNode newNode = this.CreateClassificationNode("NewClassification");
				this.SelectedNodes[0].Nodes.Add(newNode);
				newNode.EnsureVisible();
				this.SelectedNodes.Clear();
				this.SelectedNodes.Add(newNode);
				this.LabelEdit = true;
				newNode.BeginEdit();
			}
		}

		private void miRename_Click(object sender, EventArgs e)
		{
			if (SelectedNodes.Count > 0)
			{
				LabelEdit = true;
				SelectedNodes[0].BeginEdit();
			}
		}

		private void miShowFiles_Click(object sender, EventArgs args)
		{
			if (this.SelectedNodes.Count > 0  &&  ((AssetTreeNode)this.SelectedNodes[0]).IsAnAssetFilename)
			{
				ArrayList files = new ArrayList();
				foreach (AssetTreeNode tn in ((AssetTreeNode)this.SelectedNodes[0]).fileNodes)
					files.Add( tn.FileFullPath );
				
				MOG_Prompt.PromptResponse("", "Contained files:\n" + Utils.ArrayListToString( files, "\t" ));
			}
		}

		private void miRetainDirTree_Click(object sender, EventArgs args)
		{
			this.retainDirTree = !this.retainDirTree;
			((ToolStripMenuItem)sender).Checked = this.retainDirTree;
		}

		private void AssetTreeView_KeyUp(object sender, KeyEventArgs args)
		{
			if (args.KeyData == Keys.F5)
			{
				ResetColors();
			}
			else if (args.KeyData == Keys.F6)
			{
				this.SelectedNodes.Clear();
				//this.SelectedNodes.Add(mRootNode);
			}
			else if (args.KeyData == Keys.Delete)
			{
				miDelete_Click( sender, args);
			}
		}

		private void miInsert_Click(object sender, EventArgs args)
		{
			if (this.dragEventArgs != null)
			{
				InsertNodes(this.dragEventArgs);
				RaiseDroppedNodes();
			}

			this.dragEventArgs = null;
		}

		private void miInsertAsSingleAsset_Click(object sender, EventArgs args)
		{
			if (this.dragEventArgs != null)
			{
				InsertNodesAsSingleAsset(this.dragEventArgs);
				this.dragEventArgs = null;
				RaiseDroppedNodes();
			}
		}

		private void miInsertDirStructure_Click(object sender, EventArgs args)
		{
			if (this.dragEventArgs != null)
			{
				InsertDirStructure(this.dragEventArgs);
				RaiseDroppedNodes();
			}

			this.dragEventArgs = null;
		}

		private void miCombineAsSingleAsset_Click(object sender, EventArgs args)
		{
			ToolStripMenuItem mItem = sender as ToolStripMenuItem;
			
			// if we got a valid asset filename (the Text property of mItem) and we have some nodes selected to combine
			if (mItem != null  &&  this.SelectedNodes.Count > 0)
			{
				// first off, make sure that we're not combining nodes that are in diffent branches of the tree
				TreeNode parentNode = null;
				foreach (AssetTreeNode assetFilenameNode in this.SelectedNodes)
				{
					if (assetFilenameNode.IsAnAssetFilename)
					{
						// if this is the first asset filename node in SelectedNodes, set its parent
						if (parentNode == null)
							parentNode = assetFilenameNode.Parent;

						if (assetFilenameNode.Parent != parentNode)
						{
							MOG_Prompt.PromptResponse("Invlid Combine","You may only combine assets that share the same parent Classification", "", MOGPromptButtons.OK, MOG_ALERT_LEVEL.ERROR);
							return;
						}
					}
				}

				// setup new asset filename node
				AssetTreeNode newAssetFilenameNode = CreateAssetFilenameNode( mItem.Text );
				parentNode.Nodes.Add(newAssetFilenameNode);

				foreach (AssetTreeNode assetFilenameNode in this.SelectedNodes)
				{
					if (!assetFilenameNode.IsAnAssetFilename)
						continue;

					// add all its files to the new asset filename node
					newAssetFilenameNode.fileNodes.AddRange(assetFilenameNode.fileNodes);

					// reassign the assetFilename pointers on each of the file nodes
					foreach (AssetTreeNode fileNode in assetFilenameNode.fileNodes)
						fileNode.AssetFilenameNode = newAssetFilenameNode;

					assetFilenameNode.fileNodes.Clear();

					// now kill the old assetfilename node
					assetFilenameNode.Remove();

					// ..and rebuild newAssetFilenameNode's substructure
					this.RebuildSubStructure(newAssetFilenameNode);
				}

				// add in the new assetfilename node and expand it
				newAssetFilenameNode.Expand();
				newAssetFilenameNode.EnsureVisible();

				this.SelectedNodes.Clear();
			}
		}

		private void miAssignPlatform_Click(object sender, EventArgs args)
		{
			ToolStripMenuItem mItem = sender as ToolStripMenuItem;
			if (mItem != null)
			{
				string newPlatformName = "{" + mItem.Text + "}";

				// assign each selected node in turn to the new platform name
				foreach (AssetTreeNode tn in this.SelectedNodes)
				{
					// only reassign asset filename nodes	
					if (!tn.IsAnAssetFilename)
						continue;

					// strip off the old platform name and add the new one
					string []parts = tn.Text.Split("{}".ToCharArray(), 3);
					if (parts != null && parts.Length > 2)
					{
						tn.Text = newPlatformName + parts[2];
					}
				}
			}
		}
		#endregion
	}
}




