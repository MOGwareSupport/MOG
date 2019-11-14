using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;

using MOG.PROPERTIES;
using MOG.PROMPT;
using MOG.FILENAME;
using MOG.REPORT;
using MOG.TIME;

using MOG_ControlsLibrary.Utils;
using System.Collections.Generic;
using MOG.CONTROLLER.CONTROLLERPACKAGE;
using MOG;
using MOG.CONTROLLER.CONTROLLERREPOSITORY;
using MOG.CONTROLLER.CONTROLLERPROJECT;

namespace MOG_ControlsLibrary.Controls
{
	/// <summary>
	/// Summary description for MogControl_BaseTreeView.
	/// </summary>
	public abstract class MogControl_BaseTreeView : TreeView
	{
		#region Extra Icon Code
		/// <summary>
		/// Images we are adding (for all TreeViews) to the MogUtil_AssetIcons static ImageList...
		/// </summary>
		public enum ExtraImageTypes
		{
			// !!  IMPORTANT NOTE:  !!
			//  The order of this enum must match the order of our ExtraImages ImageList *exactly*
			Relationships,
			BaseFolder,
			PackageGroup,
			PackageObject,
			Package,
			GeneralAsset,
		};
		// !! IMPORTANT NOTE !!
		//  For each custom icon you would like to add, it is strongly suggested you use a 
		//   string constant here, rather than using string literals
		public const string Relationship_ImageText = "relationships";
		public const string BaseFolder_ImageText = "base-folder";
		public const string PackageGroup_ImageText = "package-group";
		public const string PackageObject_ImageText = "package-object";
		public const string Package_ImageText = "package-icon";
		public const string Asset_ImageText = "general-asset-icon";

		public static readonly System.Drawing.Color CurrentVersion_Color = System.Drawing.Color.Blue;
		protected bool bIgnoreEvents = false;  // Indicates whether we will ignore our before and after select events

		#region Custom Event Booleans
		private bool mDragDropEnabled = false;
		private bool mLabelEditEnabled = false;

		/// <summary>
		/// Determines whether we have initialized our DragDrop capabilities...
		/// </summary>
		[Browsable(false)]
		public bool IsDragDropInitialized
		{
			get { return mDragDropEnabled; }
		}

		/// <summary>
		/// Determines whether our user can do a LabelEdit within this TreeView
		/// </summary>
		[Browsable(false)]
		public bool IsLabelEditInitialized
		{
			get { return mLabelEditEnabled; }
		}
		#endregion Custom Event Booleans


		// Global for deciding whether or not we should expand our assets (default is true)
		private bool bPersistantHighlightSelectedNode;
		/// <summary>
		/// Allows designer to decide whether or not we should show Assets within a RepositoryTreeView
		/// </summary>
		[Category("Appearance"),
		Description("If TRUE, tree will change the backcolor of the selected node so it is visible when not active"),]
		public bool PersistantHighlightSelectedNode
		{
			get
			{
				return this.bPersistantHighlightSelectedNode;
			}
			set
			{
				this.bPersistantHighlightSelectedNode = value;
			}
		}

		/// <summary>
		/// Add all our Extra Icons inside ExtraImages (member variable) in the order
		///  they exist in our ImageList.  If programmer does not add things in the 
		///  right order, an Exception will be thrown by this method (see Important Notes
		///  above).
		/// </summary>
		protected void AddExtraIcons()
		{
			int i = 0;
			string keyString;
			foreach (Image image in ExtraImages.Images)
			{
				switch (i)
				{
				case (int)ExtraImageTypes.Relationships:
					keyString = Relationship_ImageText;
					break;
				case (int)ExtraImageTypes.BaseFolder:
					keyString = BaseFolder_ImageText;
					break;
				case (int)ExtraImageTypes.PackageGroup:
					keyString = PackageGroup_ImageText;
					break;
				case (int)ExtraImageTypes.PackageObject:
					keyString = PackageObject_ImageText;
					break;
				case (int)ExtraImageTypes.Package:
					keyString = Package_ImageText;
					break;
				case (int)ExtraImageTypes.GeneralAsset:
					keyString = Asset_ImageText;
					break;
				default:
					throw new Exception("Programmer has added an image to the 'ExtraImages' ImageList in "
						+ "MogControl_BaseTreeView without properly adding a const string and or an enum "
						+ "for that Image!!");
				}
				MogUtil_AssetIcons.AddIcon(image, keyString);
				++i;
			}
		}
		#endregion Extra Icon Code

		#region Global Variables and Properties
		//protected MogControl_AssetContextMenu mAssetContextMenu;
		// String for our AssetContextMenu template
		public const string ContextMenuTemplate_Text = "{Project}";

		// Variables for current settings that we want as globals for easy access (only)
		protected string mMogPath;
		protected string mProjectName;

		// ArrayList of the fullPaths of currently expanded nodes found when tree was DeInitialized
		protected List<string> mCurrentlyExpandedNodes;

		protected Point mSavedScrollPosition;

		protected ArrayList mBlessCommandBuffer;

		// Determine whether our TreeView has been initialized or not
		protected bool bIsInitialized;
		/// <summary>
		/// Read-only property to find out if tree is initialized
		/// </summary>
		public bool IsInitialized
		{
			get { return bIsInitialized; }
		}

		private string mLastNodePath;
		/// <summary>
		/// Represents path to last node that I clicked on.
		/// </summary>
		public string LastNodePath
		{
			get { return mLastNodePath; }
			set { mLastNodePath = value; }
		}

		private bool mShowToolTips;
		public bool ShowToolTips
		{
			get { return mShowToolTips; }
			set 
			{
				ShowNodeToolTips = value;
				mShowToolTips = value; 
			}
		}

		private bool mShowDiscription;
		public bool ShowDescription
		{
			get { return mShowDiscription; }
			set { mShowDiscription = value; }
		}
	
	

		/// <summary> Text to indicate we have a blank node. </summary>
		public const string Blank_Node_Text = "<BLANK>";
		public const string Revisions_Text = "All Revisions";
		public const string Contained_Assets_Text = "Package Contents";
		public const string NothingReturned_Text = "No valid classifications found!";
		public const string Packages_Text = "Packages";
		public const string Imported_Files_Text = "Imported Files";
		public const string Processed_Files_Text = "Processed Files";
		public const string Current_Text = "Current";
		//public const string	Files_Text = "Files";
		public const string Assets_Text = "Assets";
		protected ImageList ExtraImages;
		private System.ComponentModel.IContainer components;
		public const string Relationships_Text = "Relationships";
		#endregion Global Variables and Properties

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		//private System.ComponentModel.Container components = null;

		public MogControl_BaseTreeView()
			: base()
		{
			bIsInitialized = false;

			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			ImageList = MogUtil_AssetIcons.Images;
		}

		/// <summary>
		/// Force re-initialization of TreeView
		/// </summary>
		public void DeInitialize()
		{
			bIsInitialized = false;
			mCurrentlyExpandedNodes = GetArrayListOfExpandedNodesPaths(Nodes);
			mLastNodePath = (SelectedNode != null) ? SelectedNode.FullPath : "";
			mSavedScrollPosition = new Point(GetHScrollPosition(this), GetVScrollPosition(this));
		}

		/// <summary>
		/// This code makes sure that we do not auto-scroll to a different place when we programatically expand a node.
		/// </summary>
		#region Adapted code from http://www.thecodeproject.com/cs/miscctrl/CustomAutoScrollPanel.asp
		// Horizontal scrollbar enum
		private const uint SB_HORZ = 0;
		// Vertical scrollbar enum
		private const uint SB_VERT = 1;

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


		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MogControl_BaseTreeView));
			this.ExtraImages = new System.Windows.Forms.ImageList(this.components);
			this.SuspendLayout();
			// 
			// ExtraImages
			// 
			this.ExtraImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ExtraImages.ImageStream")));
			this.ExtraImages.TransparentColor = System.Drawing.Color.Transparent;
			this.ExtraImages.Images.SetKeyName(0, "");
			this.ExtraImages.Images.SetKeyName(1, "");
			this.ExtraImages.Images.SetKeyName(2, "");
			this.ExtraImages.Images.SetKeyName(3, "");
			this.ExtraImages.Images.SetKeyName(4, "");
			this.ExtraImages.Images.SetKeyName(5, "");
			// 
			// MogControl_BaseTreeView
			// 
			this.HideSelection = false;
			this.HotTracking = true;
			this.PathSeparator = "~";
			this.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.MogControl_BaseTreeView_BeforeExpand);
			this.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.MogControl_BaseTreeView_AfterSelect);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MogControl_BaseTreeView_MouseDown);
			this.ResumeLayout(false);

		}
		#endregion

		protected MethodInvoker OnInitializeDone = null;
		public abstract void Initialize();
		public virtual void Initialize(MethodInvoker OnInitializeDone)
		{
			this.OnInitializeDone = OnInitializeDone;

			Initialize();

			this.OnInitializeDone();
		}

		protected abstract void ExpandTreeDown(TreeNode node);
		protected abstract void ExpandTreeDown();

		#region Utilities
		/// <summary>
		/// Quickly recurse through TreeView, finding all terminal, expanded leaf nodes.
		/// </summary>
		/// <param name="Nodes">The TreeNodeCollection to search through</param>
		/// <returns>ArrayList of the fullPaths of TreeNodes that are terminally expanded nodes in this tree</returns>
		public List<string> GetArrayListOfExpandedNodesPaths(TreeNodeCollection nodes)
		{
			// Store our returnList
			List<string> list = new List<string>();
			
			foreach (TreeNode node in nodes)
			{
				// If it is expanded, we want to know about it...
				if (node.IsExpanded)
				{
					// If we have any sub-expansions, we want to know about them...
					List<string> children = GetArrayListOfExpandedNodesPaths(node.Nodes);
					// If we found no sub-expansions, this is a terminal-expanded node...
					if (children.Count > 0)
					{
						//We got some leaf nodes, so store them
						//No need to store the parent because it is included in the leaf
						list.AddRange(children);
					}
					else
					{
						//This node has no children expanded, so we must add it to the list
						list.Add(node.FullPath);
					}
				}
			}
			
			// Return our full list of leaf nodes
			return list;
		}

		/// <summary>
		/// Find a TreeNode, given its full path.  NOTE: This does not expand down to the node, so if
		///  the TreeView is not expanded, you will receive the closest parent node.
		/// </summary>
		/// <returns>null, if no Node found</returns>
		public TreeNode FindNode(string fullPath)
		{
			//string[] pathParts = fullPath.Split(this.PathSeparator.ToCharArray());
			TreeNode foundNode = null;
			try
			{
				TreeNodeCollection currentNodes = this.Nodes;

				// New fast search
				TreeNode []foundNodes = currentNodes.Find(fullPath, true);
				if (foundNodes != null && foundNodes.Length > 0)
				{
					if (foundNodes.Length == 1)
					{
						return foundNodes[0];
					}
				}
				// KLK - Removed this old way of finding nodes because you can use the trees built in find if you set the name of each node = its fullpath.
				//else
				//{
				//    // Foreach part of our pathParts, look for a node that matches our currentPart...
				//    foreach (string currentPart in pathParts)
				//    {
				//        // Foreach node in currentNodes, see if we match
				//        foreach (TreeNode node in currentNodes)
				//        {
				//            // If we match, set foundNode to node, then exit out of this foreach
				//            if (string.Compare(node.Text, currentPart, true) == 0)
				//            //if (node.Text.ToLower() == currentPart.ToLower())
				//            {
				//                foundNode = node;
				//                break;
				//            }
				//        }

				//        // If our foundNode.Nodes is not the same as our currentNodes collection, continue...
				//        if (foundNode != null && foundNode.Nodes != currentNodes)
				//        {
				//            // Select our next set of Nodes
				//            currentNodes = foundNode.Nodes;
				//        }
				//        // Else, we've found what we're looking for (or `null`), so terminate
				//        else
				//        {
				//            break;
				//        }
				//    }
				//}
			}
			catch (Exception ex)
			{
				MOG_Report.ReportMessage("Error Finding Node", ex.Message, ex.StackTrace, MOG_ALERT_LEVEL.ERROR);
			}

			return foundNode;
		}

		public virtual void MakeAssetCurrent(MOG_Filename assetFilename)
		{
			// Make sure this assetFilename has the info we want
			if (assetFilename != null &&
				assetFilename.GetVersionTimeStamp().Length > 0)
			{
				TreeNode foundNode = FindNode(assetFilename.GetAssetFullName());
				if (foundNode != null)
				{
					// Update this parent node with the new information concerning this asset
					Mog_BaseTag assetTag = foundNode.Tag as Mog_BaseTag;
					if (assetTag != null)
					{
						assetTag.FullFilename = assetFilename.GetOriginalFilename();
					}
				}
				else
				{
					// Try to find the asset's classification node?
					foundNode = FindNode(assetFilename.GetAssetClassification());
					if (foundNode != null)
					{
						// Create a new asset node
						TreeNode assetNode = CreateAssetNode(assetFilename);

						// Find the right spot in the list for this new asset
						int insertPosition = 0;
						foreach (TreeNode node in foundNode.Nodes)
						{
							if (string.Compare(node.Text, assetNode.Text, true) < 0)
							{
								insertPosition++;
							}
							break;
						}
						// Insert the new asset node
						foundNode.Nodes.Insert(insertPosition, assetNode);
					}
				}
			}
		}

		public TreeNode DrillToAssetName(string assetName)
		{
			List<string> parts = DrillToAsset_BuildDrillPathParts(new MOG_Filename(assetName), true);

			return DrillToNode(Nodes, parts);
		}

		public TreeNode DrillToAssetName(MOG_Filename assetFilename, bool includeRevisions)
		{
			List<string> parts = DrillToAsset_BuildDrillPathParts(assetFilename, includeRevisions);

			TreeNode node = DrillToNode(Nodes, parts);

			if (node == null)
			{
				parts = DrillToAsset_BuildDrillPathParts(assetFilename, false);
				node = DrillToNode(Nodes, parts);
			}

			return node;
		}

		public TreeNode DrillToAssetRevisions(string assetName)
		{
			List<string> parts = DrillToAsset_BuildDrillPathParts(new MOG_Filename(assetName), true);

			parts.Add(Revisions_Text);
			
			return DrillToNode(Nodes, parts);
		}

		public TreeNode DrillToPackage(string packageAssignment)
		{
			// Parse package assignment peices
			MOG_Filename packageName = new MOG_Filename(MOG_ControllerPackage.GetPackageName(packageAssignment));
			string[] packageGroups = MOG_ControllerPackage.GetPackageGroupLevels(packageAssignment);
			string[] packageObjects = MOG_ControllerPackage.GetPackageObjectLevels(packageAssignment);

			List<string> parts = DrillToAsset_BuildDrillPathParts(packageName, true);

			// Add on the package groups and package objects
			parts.AddRange(packageGroups);
			parts.AddRange(packageObjects);

			return DrillToNode(Nodes, parts);
		}

		public List<string> DrillToAsset_BuildDrillPathParts(MOG_Filename assetFilename, bool includeRevisions)
		{
			List<string> parts = new List<string>();

			if (Nodes != null)
			{
				if (Nodes.Count > 0)
				{
					if (!String.IsNullOrEmpty(assetFilename.GetOriginalFilename()))
					{
						// Add classification elements
						string classification = assetFilename.GetAssetClassification();
						if (!String.IsNullOrEmpty(classification))
						{
							string[] classificationParts = MOG_Filename.SplitClassificationString(classification);
							parts.AddRange(classificationParts);

							// Add asset name elements
							string assetName = assetFilename.GetAssetName();
							if (!String.IsNullOrEmpty(assetName))
							{
								parts.Add(assetName);

								// Check if we have a version?
								if (!String.IsNullOrEmpty(assetFilename.GetVersionTimeStamp()) && includeRevisions)
								{
									// Date format string
									string dateFormat = MOG_Tokens.GetMonth_1() + "/" + MOG_Tokens.GetDay_1() + "/" + MOG_Tokens.GetYear_4()
											+ " " + MOG_Tokens.GetHour_1() + ":" + MOG_Tokens.GetMinute_2() + " " + MOG_Tokens.GetAMPM();

									parts.Add(Revisions_Text);
									parts.Add("<" + assetFilename.GetVersionTimeStampString(dateFormat) + ">");
								}
							}
						}
					}
				}
			}

			return parts;
		}

		public TreeNode DrillToNodePath(string nodePath)
		{
			TreeNode foundNode = null;

			if (Nodes != null)
			{
				if (Nodes.Count > 0 && !String.IsNullOrEmpty(nodePath))
				{
					string[] parts = nodePath.Split(PathSeparator.ToCharArray());

					foundNode = DrillToNode(Nodes, new List<string>(parts));
				}
			}

			return foundNode;
		}

		private TreeNode DrillToNode(TreeNodeCollection nodes, List<string> nodeTextParts)
		{
			TreeNode foundNode = null;

			if (nodeTextParts.Count > 0)
			{
				ReplaceDummyNode(nodes);

				foreach (TreeNode node in nodes)
				{
					if (String.Compare(node.Text, nodeTextParts[0], true) == 0)
					{
						foundNode = node;
						
						// If this node is not expanded and we have more than one node inside it, expand
						if (!foundNode.IsExpanded)
						{
							if (foundNode.Nodes.Count == 0)
							{
								// we are not expanded (and we have 0 nodes)
								// Add our blank foundNode and expand (so long as we're not at a leaf foundNode)
								if (foundNode.Tag != null && ((Mog_BaseTag)foundNode.Tag).LeafFocus != LeafFocusLevel.RepositoryItems)
								{
									foundNode.Nodes.Add(Blank_Node_Text);
								}
							}
						}
						break;
					}
				}

				nodeTextParts.RemoveAt(0);

				if (foundNode != null && nodeTextParts.Count > 0)
				{
					ExpandTreeDown(foundNode);
					foundNode.Expand();
					return DrillToNode(foundNode.Nodes, nodeTextParts);
				}
			}

			return foundNode;
		}

		protected TreeNode FindLocalNode(TreeNodeCollection nodes, string nodeText)
		{
			foreach (TreeNode node in nodes)
			{
				if (String.Compare(node.Text, nodeText, true) == 0)
				{
					return node;
				}
			}

			return null;
		}

		private void ReplaceDummyNode(TreeNodeCollection nodes)
		{
			// If we have a non-expanded node, expand it before we continue
			if (nodes.Count > 0 && nodes[0].Text == Blank_Node_Text && nodes[0].Parent != null)
			{
				ExpandTreeDown(nodes[0].Parent);
			}
		}

		/// <summary>
		/// Sets both the ImageIndex and the SelectedImageIndex for the given `node` to `index`
		/// </summary>
		protected void SetImageIndices(TreeNode node, int index)
		{
			node.ImageIndex = index;
			node.SelectedImageIndex = index;
		}

		/// <summary>
		/// Gets ImageIndex for an ASSET without searching filesystem
		/// </summary>
		protected int GetAssetImageIndex(string filename)
		{
			return GetImageIndex(filename, false, false);
		}
		/// <summary>
		/// Gets ImageIndex for a Classification, passing `true` for isClassification
		/// </summary>
		protected int GetClassificationImageIndex(string filename)
		{
			return GetImageIndex(filename, false, true);
		}
		/// <summary>
		/// Gets ImageIndex for a System filename, passing `true` for searchFileSystem, if 
		///  the filename returns an index of 0, MOG_Properties will be used to find
		///  a default index other than 0
		/// </summary>
		protected int GetAssetFileImageIndex(string filename)
		{
			return GetImageIndex(filename, true, false);
		}

		/// <summary>
		/// Mother of all ImageIndex getting, not meant to be used outside this class
		/// </summary>
		private int GetImageIndex(string filename, bool searchFileSystem, bool isClassification)
		{

			// If we are a Classification, return our GetClassIconIndex
			if (isClassification)
			{
				return MogUtil_AssetIcons.GetClassIconIndex(filename);
			}

			int iconIndex = 0;

			// If we need to search the FileSystem, do so...
			if (searchFileSystem)
			{
				iconIndex = MogUtil_AssetIcons.GetFileIconIndex(filename);
				// If we got an iconIndex other than 0, return our value...
				if (iconIndex != 0)
				{
					return iconIndex;
				}
				// Else, continue processing
			}

			// Try getting our index through GetAssetIconIndex
			iconIndex = MogUtil_AssetIcons.GetAssetIconIndex(filename);
			// If we got an iconIndex other than 0, return...
			if (iconIndex != 0)
			{
				return iconIndex;
			}

			// Look for our index using our MOG_Filename and MOG_Properties
			MOG_Filename realFile = new MOG_Filename(filename);
			MOG_Properties properties = null;

			// If we have an Asset, get its properties
			if (realFile.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
			{
				properties = new MOG_Properties(realFile);

				// So long as we got a properties...
				if (properties != null)
				{
					iconIndex = MogUtil_AssetIcons.GetAssetIconIndex(properties.AssetIcon);
					// If we have a valid iconIndex, return it
					if (iconIndex != 0)
					{
						return iconIndex;
					}
					// Else if Package, return our default Package icon
					else if (properties.IsPackage)
					{
						return MogUtil_AssetIcons.GetClassIconIndex(Package_ImageText);
					}
					// Else, return our default Asset icon
					else
					{
						return MogUtil_AssetIcons.GetClassIconIndex(Asset_ImageText);
					}
				}
			}

			// If after all our work, we still didn't find an icon (and we know we're not a 
			//  Classification) throw an exception to the programmer
			if (iconIndex == 0)
			{
				// KLK - There is no need to thow if the icon could not be located.  Just return a bogus icon
				//throw new Exception(ToString() + ": Error with TreeNode.ImageIndex on a non-Classification.\r\n"
				//	+ "ImageIndex should not be 0 for a non-Classification");
			}

			return iconIndex;
		}

		protected int GetSpecialImageIndex(string extraImageName)
		{
			return MogUtil_AssetIcons.GetClassIconIndex(extraImageName);
		}
		#endregion Utilities

		#region Event methods
		/// <summary>
		/// Make sure that we keep our currently selected node's FullPath stored in our global variable.
		/// </summary>
		private void MogControl_BaseTreeView_AfterSelect(object sender, TreeViewEventArgs e)
		{
			// If we are not ignoring events...
			if (!bIgnoreEvents)
			{
				// Save the path of our last selected node
				Mog_BaseTag tag = e.Node.Tag as Mog_BaseTag;
				if (tag != null)
				{
					MOG_Filename filename = new MOG_Filename(tag.FullFilename);
					mLastNodePath = filename.GetAssetFullName();
				}
			}
		}

		private void MogControl_BaseTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			ExpandTreeDown(e.Node);
		}

		/// <summary>
		/// Makes sure we have the correct node selected on MouseDown (this supports right-click)
		/// </summary>
		private void MogControl_BaseTreeView_MouseDown(object sender, MouseEventArgs e)
		{
			SelectedNode = GetNodeAt(e.X, e.Y);
			mSavedScrollPosition = new Point(GetHScrollPosition(this), GetVScrollPosition(this));
		}
		#endregion Event methods

		public void InitializeDragDrop()
		{
			AllowDrop = true;
			if (!mDragDropEnabled)
			{
				DragOver += new DragEventHandler(MogControl_BaseTreeView_DragOver);
				ItemDrag += new ItemDragEventHandler(MogControl_BaseTreeView_ItemDrag);
				DragDrop += new DragEventHandler(MogControl_BaseTreeView_DragDrop);
				mDragDropEnabled = true;
			}
		}

		public void InitializeLabelEdit()
		{
			if (!mLabelEditEnabled)
			{
				AfterLabelEdit += new NodeLabelEditEventHandler(MogControl_BaseTreeView_AfterLabelEdit);
				mLabelEditEnabled = true;
			}
		}

		private void MogControl_BaseTreeView_ItemDrag(object sender, ItemDragEventArgs e)
		{
			// Create our list holders
			ArrayList items = new ArrayList();

			// Make sure a valid TreeNode item is selected and that we have privileges
			if (e.Item != null &&
				MOG_ControllerProject.GetPrivileges().GetUserPrivilege(MOG_ControllerProject.GetUser().GetUserName(), MOG.MOG_PRIVILEGE.DeleteClassification))
			{
				TreeNode node = e.Item as TreeNode;

				// Make sure we are not editing the adam node
				if (node != null && node.Parent != null)
				//					string.Compare(node.Text, MOG_ControllerProject.GetProjectName(), true) != 0)
				{
					// Add the text and a reference to the item to our lists
					items.Add(node);

					// Create a new Data object for the send
					DataObject send = new DataObject("ProjectTreeView", items);

					// Fire the DragDrop event
					DragDropEffects dde1 = DoDragDrop(send, DragDropEffects.Move);
				}
			}
		}

		private static string hoverTarget;
		private static DateTime hoverTargetChanged;

		private void MogControl_BaseTreeView_DragOver(object sender, DragEventArgs e)
		{
			// Make sure we only display the drag-n-drop icon (via Windows) if we have the right data...
			if (e.Data.GetData("ProjectTreeView") != null)
			{
				TreeView tree = sender as TreeView;

				// Get the node at the location of our drag
				tree.SelectedNode = tree.GetNodeAt(PointToClient(new Point(e.X, e.Y)));

				if (tree.SelectedNode != null)
				{
					Mog_BaseTag classObj = tree.SelectedNode.Tag as Mog_BaseTag;

					// Make sure this target is a class
					if (classObj != null && classObj.PackageNodeType == PackageNodeTypes.Class)
					{
						e.Effect = DragDropEffects.Move;

						// Chedk if it has been enough time hovering over this classification, if it has expand it.
						if (hoverTarget != tree.SelectedNode.FullPath)
						{
							hoverTarget = tree.SelectedNode.FullPath;
							hoverTargetChanged = DateTime.Now.AddMilliseconds(500);
						}
						else if (hoverTarget == tree.SelectedNode.FullPath &&
							DateTime.Now >= hoverTargetChanged)
						{
							tree.SelectedNode.Expand();
						}
						return;
					}
				}
			}

			e.Effect = DragDropEffects.None;
		}

		private void MogControl_BaseTreeView_DragDrop(object sender, DragEventArgs e)
		{
			// We will only accept a ArrayListAssetManager type object
			if (e.Data.GetDataPresent("ProjectTreeView"))
			{
				// Get our array list
				ArrayList items = (ArrayList)e.Data.GetData("ProjectTreeView");

				foreach (TreeNode classObj in items)
				{
					Mog_BaseTag objInfo = classObj.Tag as Mog_BaseTag;

					bool success = false;
					// If we are a classification
					if (objInfo != null && objInfo.PackageNodeType == PackageNodeTypes.Class)
					{
						// Prompt user to make sure they did this intentionally
						string message = "Are you sure you want to move this classification?\n\n" +
										 "CLASSIFICATION: " + classObj.Text + "\n" +
										 "TARGET: " + SelectedNode.FullPath;
						if (MOG_Prompt.PromptResponse("Move Classification?", message, MOGPromptButtons.YesNoCancel) == MOGPromptResult.Yes)
						{
							// Do classification move here!!!
							success = MOG_ControllerProject.GetProject().ClassificationRename(classObj.FullPath, SelectedNode.FullPath + "~" + classObj.Text);
						}
					}
					// Is it an asset
					else if (objInfo != null && 
							(objInfo.PackageNodeType == PackageNodeTypes.Asset || objInfo.PackageNodeType == PackageNodeTypes.Package))
					{
						// Prompt user to make sure they did this intentionally
						string message = "Are you sure you want to move this asset?\n\n" +
										 "ASSET: " + classObj.Text + "\n" +
										 "NEW CLASSIFICATION: " + SelectedNode.FullPath;
						if (MOG_Prompt.PromptResponse("Move Asset?", message, MOGPromptButtons.YesNoCancel) == MOGPromptResult.Yes)
						{
							// Do Asset move here!!!
							MOG_Filename assetFullname = new MOG_Filename(objInfo.FullFilename);
							success = MOG_ControllerProject.GetProject().AssetRename(classObj.FullPath, SelectedNode.FullPath + "~" + classObj.Text);
						}
					}

					AllowDrop = false;
				}
			}

			AllowDrop = true;
		}

		/// <summary>
		/// This is the after edit event of renamimg a asset classification
		/// </summary>
		private void MogControl_BaseTreeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
		{
			LabelEdit = false;

				// Make sure they want to rename
			string newClassName = SelectedNode.FullPath.Replace(SelectedNode.Text, e.Label);

						// Perform rename here!!!
			if (!MOG_ControllerProject.GetProject().ClassificationRename(SelectedNode.FullPath, newClassName))
			{
				e.CancelEdit = true;
			}
		}

		/// <summary>
		/// Used to get an Asset node with a good node.Tag for this TreeView or any inheriting classes.
		///  Does not use full filename.
		/// </summary>
		protected virtual TreeNode CreateAssetNode(MOG_Filename asset)
		{
			// Create a basic tree node
			TreeNode assetNode = new TreeNode(asset.GetAssetName());
			MOG_Filename assetFile = MOG_ControllerProject.GetAssetCurrentBlessedPath(asset);
			assetNode.Tag = new Mog_BaseTag(assetNode, "", RepositoryFocusLevel.Repository, true);
			assetNode.Name = asset.GetAssetFullName();
			// Initialize the treenode's image
			SetImageIndices(assetNode, GetAssetImageIndex(asset.GetAssetFullName()));

			return assetNode;
		}
	}
}
