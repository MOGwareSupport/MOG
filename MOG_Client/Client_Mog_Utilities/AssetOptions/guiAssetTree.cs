using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

using MOG;
using MOG.INI;
using MOG.TIME;
using MOG.REPORT;
using MOG.PROJECT;
using MOG.PROPERTIES;
using MOG.FILENAME;
using MOG.DOSUTILS;
using MOG.DATABASE;
using MOG_Client.Client_Mog_Utilities;
using MOG.CONTROLLER.CONTROLLERREPOSITORY;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;

using MOG_ControlsLibrary;
using MOG_ControlsLibrary.Utils;



namespace MOG_Client.Client_Mog_Utilities.AssetOptions
{
	/// <summary>
	/// Summary description for guiAssetTree.
	/// </summary>
	public class guiAssetTree
	{
//		static bool bFullRootView = false;
		public const string VERSION_TOKEN = "All Revisions";

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
		#endregion

		public guiAssetTree()
		{
		}

		/// <summary>
		/// AssetTreeViewCreate creates tree nodes based on a valid version.info file.  It does so one level at a time and not all at once.
		/// </summary>
		/// <param name="versionInfoFilename"></param>
		/// <param name="tree"></param>
		/// <param name="parent"></param>
		/// <param name="level"></param>
		static public void AssetTreeViewCreate(string versionInfoFilename, TreeView tree, TreeNode parent, guiAssetTreeTag.TREE_FOCUS level, bool createVersionNode)
		{
			/// Changes to this function could have side-effects in many other areas...
			int defaultImageIndex = 0;
//			if (level == guiAssetTreeTag.TREE_FOCUS.BASE)
//			{
//				bFullRootView = true;
//			}

			// Find the items in the ASSETS section, set value of `contents`
			ArrayList contents = new ArrayList();
// TODO Rewrite this!
//			switch(level)
//			{
//				case guiAssetTreeTag.TREE_FOCUS.BASE:
//					contents = MOG_DBAssetAPI.GetAllUniqueProjectKeys();
//					break;
//				case guiAssetTreeTag.TREE_FOCUS.KEY:
//					if (parent != null)
//					{
//						// Takes project key from last iteration
//						contents = MOG_DBAssetAPI.GetAllUniqueAssetTypes(parent.Text);  
//					}
//					else
//					{
//						contents = MOG_DBAssetAPI.GetAllUniqueAssetTypes(MOG_ControllerProject.GetProject().GetProjectKey());
//					}
//					break;
//				case guiAssetTreeTag.TREE_FOCUS.CLASS:
//					if (parent.Parent != null)
//					{
//						contents = MOG_DBAssetAPI.GetAllUniqueAssetGroups(parent.Parent.Text, parent.Text);
//					}
//					else
//					{
//						contents = MOG_DBAssetAPI.GetAllUniqueAssetGroups(MOG_ControllerProject.GetProject().GetProjectKey(), parent.Text);						
//					}
//					break;
//				case guiAssetTreeTag.TREE_FOCUS.SUBCLASS:
//					if (parent.Parent.Parent != null)
//					{
//						contents = MOG_DBAssetAPI.GetAllUniqueAssetSubGroups(parent.Parent.Parent.Text, parent.Parent.Text, parent.Text);
//					}
//					else
//					{
//						contents = MOG_DBAssetAPI.GetAllUniqueAssetSubGroups(MOG_ControllerProject.GetProject().GetProjectKey(), parent.Parent.Text, parent.Text);
//					}
//					break;
//				case guiAssetTreeTag.TREE_FOCUS.LABEL:
//					// If we have a super-parent and this node's parent does not have a filename in it.
//					if (parent.Parent.Parent.Parent != null && parent.Text.IndexOf(".") == -1)
//					{
//						// Check to see if this super-Parent.Text is the name of a package
//						if(parent.Parent.Parent.Parent.Text.IndexOf("\\") == -1)
//						{
//							contents = MOG_DBAssetAPI.GetAllUniqueFullAssetLabels(parent.Parent.Parent.Parent.Text, parent.Parent.Parent.Text, parent.Parent.Text, parent.Text);
//						}
//						else
//						{
//							string[] assetParts = parent.Text.Split('.');
//							if(assetParts.Length > 3)
//								contents = MOG_DBAssetAPI.GetAllUniqueFullAssetLabels(assetParts[0], assetParts[1], assetParts[2], assetParts[3]);
//							else
//								return;
//						}
//					}
//					else
//					{
//						ArrayList assets = MOG_DBAssetAPI.GetSyncTargetFileAssetLinksFileNameOnly( parent.Text );
//						if(assets.Count > 0)
//						{
//							string assetName = (string)assets[0];
//							contents.Add( assetName );
//						}
//					}
//					break;
//				case guiAssetTreeTag.TREE_FOCUS.VERSION:
//					assetTreeViewCreate_AddVersionNodes(tree, parent, defaultImageIndex);
//					return;
//				case guiAssetTreeTag.TREE_FOCUS.SUBVERSION:
//					// Add the files nodes
//					assetTreeViewCreate_AddFilesNodes(parent, defaultImageIndex);  
//					MOG_Filename asset =  new MOG_Filename(((guiAssetTreeTag)parent.Tag).FullFilename);
//					// If this is an asset only, add a package node placeholder for further expansion
//					if(!asset.IsPackage())
//					{
//						MOG_Properties pProperties = new MOG_Properties(asset);
//						bool isPackaged = pProperties.IsPackagedAsset();
//						if(isPackaged)
//						{
//							TreeNode packageRoot = new TreeNode("Packages", defaultImageIndex, 0);
//							packageRoot.Tag = parent.Tag;
//							((guiAssetTreeTag)packageRoot.Tag).Level = guiAssetTreeTag.TREE_FOCUS.SUBVERSION;
//							packageRoot.Nodes.Add(new TreeNode(""));
//							parent.Nodes.Add(packageRoot);
//						}
//						return;
//					}
//					else
//					{
//						//GLK: Fill up contained assets for a package
//						TreeNode assetRoot = new TreeNode("Contained Assets", defaultImageIndex, 0);
//						guiAssetTreeTag parentTag = (guiAssetTreeTag)parent.Tag;
//						assetRoot.Tag = new guiAssetTreeTag(parentTag.FullFilename, 
//							guiAssetTreeTag.TREE_FOCUS.SUBCLASS, true);
//						((guiAssetTreeTag)assetRoot.Tag).Level = guiAssetTreeTag.TREE_FOCUS.SUBCLASS;
//						string packageName;
//						// If this node is not a Version Node
//						if(parent.Parent.Text != VERSION_TOKEN)
//							packageName = parent.Parent.Text;
//						else
//							packageName = parent.Parent.Parent.Text;
//						//assetRoot.Nodes.Add(new TreeNode(""));
//
//						//GLK: Not fully implemented yet.  I can use MOG_Filename(parentTag.FullFilename) to get the version.
//						MOG_Filename parentFilename = new MOG_Filename(parentTag.FullFilename);
//						contents = MOG_DBPackageAPI.GetAllAssetsInPackage( parentFilename, parentFilename.GetVersionTimeStamp() );
//
//						assetTreeViewCreate_AddNodes(contents, versionInfoFilename, tree, assetRoot, 
//							guiAssetTreeTag.TREE_FOCUS.LABEL, defaultImageIndex, createVersionNode);
//						parent.Nodes.Add(assetRoot);
//						return;
//					}
//					// At this level (below), we go back to TREE_FOCUS.LABEL for recursion
//				case guiAssetTreeTag.TREE_FOCUS.PACKAGE:
//					assetTreeViewCreate_AddPackagesNodes(parent, defaultImageIndex);
//					return;
//			}

			// Add nodes for any TREE_FOCUS other than the last three (above)
			assetTreeViewCreate_AddNodes(contents, versionInfoFilename, tree, parent, 
				level, defaultImageIndex, createVersionNode);
		}

		static protected void assetTreeViewCreate_AddPackagesNodes(TreeNode parent, int defaultIndex)
		{
			// Clear any ghost node(s)
			parent.Nodes.Clear();

			// Create a tag for local use
			guiAssetTreeTag parentTag = (guiAssetTreeTag)parent.Tag;

			// Populate packages node
			ArrayList packages = MOG_DBPackageAPI.GetPackageGroupsForAsset(new MOG_Filename(parentTag.FullFilename));
			foreach(string package in packages)
			{
				TreeNode packageNode = new TreeNode(package, defaultIndex, 0);
				packageNode.Tag = parentTag;
				// Needs to be LABEL for recursion
				((guiAssetTreeTag)packageNode.Tag).Level = guiAssetTreeTag.TREE_FOCUS.LABEL;

				assetTreeViewCreate_AddIndividualNodes("", package, null, parent,
					guiAssetTreeTag.TREE_FOCUS.LABEL, defaultIndex, true);
			}
		}

		static protected void assetTreeViewCreate_AddFilesNodes(TreeNode parent, int defaultIndex)
		{
			// Create files node
			TreeNode filesRoot = new TreeNode("Files", defaultIndex, 0);
			guiAssetTreeTag parentTag = (guiAssetTreeTag)parent.Tag;
			filesRoot.Tag = parentTag;
			((guiAssetTreeTag)filesRoot.Tag).Level = guiAssetTreeTag.TREE_FOCUS.SUBVERSION;

			// Populate files
			TreeNode sourceRoot = new TreeNode("Imported Files", defaultIndex, 0);
			sourceRoot.Tag = filesRoot.Tag;
			TreeNode gameDataRoot = new TreeNode("Processed Files", defaultIndex, 0);
			gameDataRoot.Tag = filesRoot.Tag;

			DirectoryInfo[] directories = DosUtils.DirectoryGetList(parentTag.FullFilename, "");

			// Go through each platform only for the asset of this parent node
			foreach(DirectoryInfo directory in directories)
			{
				// If this is a gamedata, add it with its subnode and file(s)
				if(directory.Name.ToLower().IndexOf("gamedata") > -1)
				{
					// Get rid of gamedata from directory.Name
					string gamedataName = directory.Name.ToLower().Replace("files.", "");
					// Create a new node for each platform
					TreeNode platformNode = new TreeNode(gamedataName, defaultIndex, 0);
					platformNode.Tag = filesRoot.Tag;

					FileInfo[] files = DosUtils.FileGetList(directory.FullName, "");
					// Add files in gamedata directory to platformNode
					if(files != null)
					{
						// Add each file to each gamedata<platform> node
						foreach(FileInfo file in files)
						{
							TreeNode fileNode = assetTreeViewCreate_GetTreeNodeWithIcon(file.Name, defaultIndex);
							fileNode.Tag = filesRoot.Tag;
							platformNode.Nodes.Add(fileNode);
						}
					}

					gameDataRoot.Nodes.Add(platformNode);
				}
					// Else if this is a files folder, add it to the source node
				else if(directory.Name.ToLower().IndexOf("files") > -1)
				{
					FileInfo[] files = DosUtils.FileGetList(directory.FullName, "");
					if(files != null)
					{
						foreach(FileInfo file in files)
						{
							TreeNode fileNode = assetTreeViewCreate_GetTreeNodeWithIcon(file.Name, defaultIndex);
							fileNode.Tag = filesRoot.Tag;
							sourceRoot.Nodes.Add(fileNode);
						}
					}
				}
			}

			filesRoot.Nodes.Clear();
			filesRoot.Nodes.AddRange(new TreeNode[]{sourceRoot, gameDataRoot});
			parent.Nodes.Add(filesRoot);		
		}

		static protected void assetTreeViewCreate_AddVersionNodes(TreeView tree, TreeNode parent, int defaultIndex)
		{
			// Add version nodes
			TreeNode versionRoot = new TreeNode(VERSION_TOKEN, defaultIndex, 0);
			versionRoot.Tag = parent.Tag;
			parent.Nodes.Clear();

			// Date format string
			string dateFormat = MOG_Tokens.GetMonth_1() + "/" + MOG_Tokens.GetDay_1() + "/" + MOG_Tokens.GetYear_4()
					+ " " + MOG_Tokens.GetHour_1() + ":" + MOG_Tokens.GetMinute_2()	+ " " + MOG_Tokens.GetAMPM();

			// Create a tag for ease of use
			guiAssetTreeTag parentTag = (guiAssetTreeTag)parent.Tag;

			// Get the lastest version string
			string versionStr = MOG_DBAssetAPI.GetAssetVersion(new MOG_Filename(parentTag.FullFilename));

			// Populate HDD versions
			DirectoryInfo[] hddVersions = DosUtils.DirectoryGetList(parentTag.FullFilename, "");
			// Arrange it latest date first
			Array.Reverse(hddVersions);

			// Populate DB versions
			ArrayList dbVersions = MOG_DBAssetAPI.GetAllAssetRevisions(new MOG_Filename(parentTag.FullFilename));

			// Turn off sorting for this part of the tree
			tree.Sorted = false;
			foreach(DirectoryInfo version in hddVersions)
			{
				// Create a new versionNode
				MOG_Filename versionFile = new MOG_Filename(version.FullName);
				string textLabel = "<" + versionFile.GetVersionTimeStampString(dateFormat) + ">";
				TreeNode versionNode = new TreeNode(textLabel, defaultIndex, 0);
				// Add guiAssetTreeTag and change color to indicate HDD-only status
				versionNode.Tag = new guiAssetTreeTag(version.FullName, 
					guiAssetTreeTag.TREE_FOCUS.VERSION, true);

				// If this is a version matched in the DB, color is black
				if(assetTreeViewCreate_IsAssetVersionInDB(dbVersions, version.Name))
				{
					versionNode.ForeColor = Color.Black;
				}
					// Else, turn color gray
				else
				{
					versionNode.ForeColor = Color.DarkGray;
				}

//glk:  This still needs to be tested against the case where there is no directory that matches the verion in versionStr

				// If this is the most recent asset, display it as such
				if(versionStr == versionFile.GetVersionTimeStamp())
				{
					versionNode.ForeColor = Color.Blue;
					// Create a currentVersion node, manually cloning versionNode
					//  glk: object.Clone() does not function properly
					TreeNode currentVersion = new TreeNode(textLabel,
						versionNode.ImageIndex, versionNode.SelectedImageIndex);
					currentVersion.ForeColor = Color.Blue;
					currentVersion.Tag = new guiAssetTreeTag(version.FullName,
						guiAssetTreeTag.TREE_FOCUS.VERSION, true);

					// Keep tree from drawing itself for a bit
					tree.BeginUpdate();

					// Document the horizontal and vertical 
					//  positions of tree's scrollbar using extern functions
					int horizPos = GetHScrollPosition(tree);
					int vertPos = GetVScrollPosition(tree);

					// Expand the tree: This is the operation which causes
					//  really erratic behavior from TreeView Control
					currentVersion.Expand();

					// Add ghost node for further expansion
					currentVersion.Nodes.Add(new TreeNode(""));
					parent.Nodes.Add(currentVersion);

					// Set the scrollbar horizontal and vertical positions
					//  back to what they were.
					SetHScrollPosition(tree, horizPos);
					SetVScrollPosition(tree, vertPos);

					// Allow tree to draw itself
					tree.EndUpdate();
				}
				// Add new placeholder for further expansion
				versionNode.Nodes.Add(new TreeNode(""));
				versionRoot.Nodes.Add(versionNode);
			}
			parent.Nodes.Add(versionRoot);
			// Turn sorting back on
			tree.Sorted = true;
		}

		static protected bool assetTreeViewCreate_IsAssetVersionInDB(ArrayList dbVersions, string versionName)
		{
			// Search through dbVersions for match to this versionName
			foreach(string version in dbVersions)
			{
				if(versionName == ("r." + version))
					return true;
			}
			return false;
		}

		static protected TreeNode assetTreeViewCreate_GetTreeNodeWithIcon(string name, int defaultIndex)
		{
			// Set the imageIndex for a new TreeNode, create the TreeNode, and create a blank tag for the new node
			int imageIndex = MogUtil_AssetIcons.GetClassIconIndex( name );
			if (imageIndex == 0) imageIndex = defaultIndex;
			return new TreeNode(name, imageIndex , 0);
		}

		static protected void assetTreeViewCreate_AddNodes(ArrayList contents, string versionInfoFilename, 
			TreeView tree, TreeNode parent,	guiAssetTreeTag.TREE_FOCUS level, int defaultImageIndex, bool createChildNode)
		{
			foreach(string name in contents)
			{
				assetTreeViewCreate_AddIndividualNodes(versionInfoFilename, name, tree, parent, level, 
					defaultImageIndex, createChildNode);
			}
		}

		static protected void assetTreeViewCreate_AddIndividualNodes(string versionInfoFilename, string name,
			TreeView tree, TreeNode parent,	guiAssetTreeTag.TREE_FOCUS level, int defaultImageIndex, bool createChildNode)
		{
			// Set the imageIndex for a new TreeNode, create the TreeNode, and create a blank tag for the new node
			TreeNode node = assetTreeViewCreate_GetTreeNodeWithIcon(name, defaultImageIndex);
			guiAssetTreeTag tag = null;
	
			// If `name` already has the project and projectpath, we need to get rid of it.
			string projectPath = MOG_ControllerProject.GetProject().GetProjectPath() + "\\";
			string newName = name.Replace(projectPath, "");

			// Get the blessed path for this assetname
			MOG_Filename asset= new MOG_Filename(projectPath + newName);

			if (asset.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
			{
				MOG_Filename fullname = GetAssetFullname(asset, versionInfoFilename);
				tag = new guiAssetTreeTag(fullname.GetEncodedFilename(), level, true);
			}
			else
			{
				tag = new guiAssetTreeTag(asset.GetEncodedFilename(), level, false);
			}

			// Save the type of node this is (Key, Class, Group...)
			node.Tag = tag;

			// Don't add an extra child if we are at the end of the chain
			if (level != guiAssetTreeTag.TREE_FOCUS.LABEL || createChildNode)
			{
				// Add a blank for this keys child
				node.Nodes.Add("");
			}

			// Check if we should add to the base or to a child
			if (parent == null)
			{
				tree.Nodes.Add(node);
			}
			else
			{
				parent.Nodes.Add(node);
			}		
		}


		static protected MOG_Filename GetAssetFullname(MOG_Filename asset, string contentsInfo)
		{
			return MOG_ControllerRepository.GetAssetBlessedPath(asset);
		}

		
		/// <summary>
		/// Create one new node or update an existing node in the tree.
		/// </summary>
		/// <param name="tree"></param>
		/// <param name="assetName"></param>
		/// <param name="nodeColor"></param>
		static public void AssetTreeViewUpdateNode(string dir, TreeView tree, MOG_Filename assetName, Color nodeColor)
		{
			// Find the type in tree if exists
			TreeNode parent = FindAssetNodeInTree(tree, assetName);

			if (parent != null)
			{
				TreeNode node = new TreeNode();			
				node.Text = assetName.GetAssetLabel();			// Name
				node.ForeColor = nodeColor;						// Color
				node.Checked = true;

				node.ImageIndex = MogUtil_AssetIcons.GetClassIconIndex( assetName.GetEncodedFilename() );

				string fullname = dir + "\\" + assetName.GetEncodedFilename();
				node.Tag = new guiAssetTreeTag(fullname, guiAssetTreeTag.TREE_FOCUS.SUBCLASS, true);
				
				// Add this asset to the tree
				parent.Nodes.Add(node);
			}
		}

		/// <summary>
		/// Find a node within a nodes children based on its Text value
		/// </summary>
		/// <param name="parentNode"></param>
		/// <param name="title"></param>
		/// <returns></returns>
		static protected TreeNode FindNode(TreeNode parentNode, string title)
		{
			if (parentNode != null)
			{
				foreach (TreeNode node in parentNode.Nodes)
				{
					if (string.Compare(node.Text, title, true) == 0)
					{
						return node;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Returns the parent node of this asset type.  We search the key then package and group to find the parent node.
		/// </summary>
		/// <param name="tree"></param>
		/// <param name="assetName"></param>
		/// <returns></returns>
		static protected TreeNode FindAssetNodeInTree(TreeView tree, MOG_Filename assetName)
		{
//			TreeNode keyNode, packageNode, groupNode, assetNode;
			int imageIndex = MogUtil_AssetIcons.GetClassIconIndex(assetName.GetAssetName());
			return new TreeNode();
// TODO - Rebuild this code!!
			
//			// First find the key
//			foreach (TreeNode keyNodes in tree.Nodes)
//			{
//				if (string.Compare(keyNodes.Text, assetName.GetAssetClassification(), true) == 0)
//				{
//					// Find the package
//					packageNode = FindNode(keyNodes, assetName.GetAssetClassification());
//					if (packageNode != null)
//					{
//						// Find the group
//						groupNode = FindNode(packageNode, assetName.GetAssetSubClass());
//						if (groupNode != null)
//						{
//							// Find the asset
//							assetNode = FindNode(groupNode, assetName.GetAssetName());
//							if (assetNode != null)
//							{
//								assetNode.Remove();
//								return groupNode;
//							}
//							else
//							{
//								// This is the group of the new child asset
//								return groupNode;								 
//							}
//						}
//						else
//						{
//							// Create the group
//							groupNode = new TreeNode(assetName.GetAssetSubClass());
//							groupNode.ImageIndex = imageIndex;
//                            groupNode.Tag = new guiAssetTreeTag("", guiAssetTreeTag.TREE_FOCUS.SUBCLASS);
//
//							try
//							{
//								// Add this branch to the tree
//								packageNode.Nodes.Add(groupNode);
//							
//								// This is the group of the new child asset
//								return groupNode;								 
//							}
//							catch(Exception e)
//							{
//								e.ToString();
//								return null;
//							}
//						}
//					}
//					else
//					{
//						// Create the package and the group
//						packageNode = new TreeNode(assetName.GetAssetClassification());
//						packageNode.ImageIndex = imageIndex;
//						packageNode.Tag = new guiAssetTreeTag("", guiAssetTreeTag.TREE_FOCUS.CLASS);
//
//						groupNode = new TreeNode(assetName.GetAssetSubClass());
//						groupNode.ImageIndex = imageIndex;
//						groupNode.Tag = new guiAssetTreeTag("", guiAssetTreeTag.TREE_FOCUS.SUBCLASS);
//						
//						try
//						{
//							packageNode.Nodes.Add(groupNode);
//
//							// Add this branch to the tree
//							keyNodes.Nodes.Add(packageNode);
//							
//							// This is the group of the new child asset
//							return groupNode;								 
//						}
//						catch(Exception e)
//						{
//							e.ToString();
//							return null;
//						}
//					}			
//				}				
//			}

			
//			// Create the key, package and the group
//			keyNode = new TreeNode(assetName.GetAssetKey(), imageIndex, 0);
//			keyNode.Tag = new guiAssetTreeTag("", guiAssetTreeTag.TREE_FOCUS.KEY);
//
//			packageNode = new TreeNode(assetName.GetAssetClassification());
//			packageNode.ImageIndex = imageIndex;
//			packageNode.Tag = new guiAssetTreeTag("", guiAssetTreeTag.TREE_FOCUS.CLASS);
//			keyNode.Nodes.Add(packageNode);
//
//			groupNode = new TreeNode(assetName.GetAssetSubClass());
//			groupNode.ImageIndex = imageIndex;
//			groupNode.Tag = new guiAssetTreeTag("", guiAssetTreeTag.TREE_FOCUS.SUBCLASS);
//			packageNode.Nodes.Add(groupNode);
//			
//			try
//			{
//				// Add this branch to the tree
//				tree.Nodes.Add(keyNode);
//
//				// This is the group of the new child asset
//				return groupNode;
//			}
//			catch(Exception e)
//			{
//				e.ToString();
//				return null;
//			}
		}
	}
}
