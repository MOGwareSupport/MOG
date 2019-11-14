using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

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
	/// Summary description for guAssetGamedataTree.
	/// </summary>
	public class guAssetGamedataTree 
	{
		// For use with populating all platforms
		public const string ALL_PLATFORMS = "all";

		public guAssetGamedataTree()
		{}

		static public void AssetTreeViewCreate(string versionInfoFilename, TreeView tree, TreeNode parent, guiAssetGamedataTreeTag.TREE_FOCUS level, bool createChildNode)
		{
			/// Changes to this function could have side-effects in many other areas...
			int defaultImageIndex = 0;

			guiAssetGamedataTreeTag currentTag = null;
			if(parent != null)	currentTag = (guiAssetGamedataTreeTag)parent.Tag;

			// Find the items in the ASSETS section, set value of `contents`
			ArrayList contents = new ArrayList();
			switch(level)
			{
				case guiAssetGamedataTreeTag.TREE_FOCUS.BASE:
// TODO - Check this!
//					contents = MOG_DBAssetAPI.GetAllUniqueProjectKeys();
					break;
				case guiAssetGamedataTreeTag.TREE_FOCUS.PLATFORM:
					string[] platformNames = MOG_ControllerProject.GetProject().GetPlatformNames();
					foreach(string platformName in platformNames)
					{
						contents.Add(platformName);
					}
					break;
				case guiAssetGamedataTreeTag.TREE_FOCUS.KEY:

					// If we have no ListForNode ArrayList, start fresh
					if(currentTag == null || currentTag.ListForNode.Count < 1)
					{
						contents = MOG_DBAssetAPI.GetAllProjectSyncTargetFilesForPlatform(parent.Text);
						assetTreeViewCreate_ParseSyncTargetFileNodes(contents, parent, versionInfoFilename, level);
					}
						//Else we are in a subNode and need to process a ListForNode
					else
					{
						assetTreeViewCreate_ParseSyncTargetFileNodes(currentTag.ListForNode, parent, versionInfoFilename, level);
					}
					return;
				default:
					return;
			}
			assetTreeViewCreate_AddNodes(contents, versionInfoFilename, tree, parent, 
				level, defaultImageIndex, true);
		}

		static protected void assetTreeViewCreate_ParseSyncTargetFileNodes(ArrayList contents, TreeNode parent,
			string versionInfoFilename, guiAssetGamedataTreeTag.TREE_FOCUS level, bool isInsideAFileRoot)
		{
			string	lastRoot = string.Empty, 
					currentRoot = string.Empty;
			string	lastNodeName = string.Empty;

			ArrayList subContents = new ArrayList();

			for(int i = 0; i < contents.Count; ++i)
			{
				string fullFileName = (string)contents[i];

				// If we have an initial backslash, get rid of it
				if(fullFileName.IndexOf("\\") == 0)
					fullFileName = fullFileName.Substring(("\\").Length);

				// True if we need to create a parent node
				bool isNotAFile = (fullFileName.IndexOf("\\") > 0);

				string rootName = string.Empty;
				string modifiedFilename = string.Empty;

				if(isNotAFile)
				{
					rootName = fullFileName.Substring(0, fullFileName.IndexOf("\\"));
					if(fullFileName.IndexOf("\\") != -1)
						modifiedFilename = fullFileName.Replace(rootName + "\\", "");
					rootName = rootName.ToLower();
				}
				else
				{
					modifiedFilename = fullFileName;
				}

				// If we have a duplicate, skip it.
				if(modifiedFilename == lastNodeName)
					continue;

				// If we have our first deeper level, recursively go through it
				if(isNotAFile && lastRoot == string.Empty && currentRoot == string.Empty)
				{
					//Console.Write("We need a sublevel");
					lastRoot = rootName;
					currentRoot = rootName;
				}
					// For proceeding deeper levels, go through them.
				else if(isNotAFile && currentRoot != rootName)
				{
					//Console.Write("Sublevel needed.");

					CheckIfAlreadyAdded(subContents, currentRoot, parent, versionInfoFilename, level);
					subContents.Clear();
					
					lastRoot = currentRoot;
					currentRoot = rootName;
				}
					// If this is the last level, add the node only if it's not there,
					//  add the files regardless of the existence of the node.
				else if(isNotAFile && (i == (contents.Count - 1)))
				{
					CheckIfAlreadyAdded(subContents, rootName, parent, versionInfoFilename, level);
					subContents.Clear();
				}
					// If this is a gamedata filename...
				else if(!isNotAFile && (fullFileName.IndexOf(".") > -1  || fullFileName.IndexOf("\\") == -1))
				{
					TreeNode fileNode = new TreeNode(fullFileName);
					string platform = string.Empty;
					string fullname = ToString(parent, fullFileName, ref platform);
					string projectPath = MOG_ControllerProject.GetProject().GetProjectPath();

					fullname = projectPath + "\\GameData\\" + platform + fullname;
					fileNode.Tag = new guiAssetTreeTag( fullname, guiAssetTreeTag.TREE_FOCUS.SUBCLASS, true );
					// Add placeholder node for further expansion down
					fileNode.Nodes.Add( new TreeNode("") );
					parent.Nodes.Add(fileNode);
				}

				lastNodeName = modifiedFilename;
				subContents.Add(modifiedFilename);

			}
		}

		static protected void assetTreeViewCreate_ParseSyncTargetFileNodes(ArrayList contents, TreeNode parent,
			string versionInfoFilename, guiAssetGamedataTreeTag.TREE_FOCUS level)
		{
			assetTreeViewCreate_ParseSyncTargetFileNodes(contents, parent, versionInfoFilename, level, false);
		}

		static protected string ToString(TreeNode parent, string fileName, ref string platform)
		{
			if(parent != null)
			{
				fileName = parent.Text + "\\" + fileName;
				return ToString(parent.Parent, fileName, ref platform);
			}
			else
			{
				// Get rid of the top two parent node.Texts before we return result
				int indexOfFirstBackSlash = fileName.IndexOf("\\");
				int indexOfSecondBackSlash = fileName.IndexOf("\\", indexOfFirstBackSlash + 1);
				platform = fileName.Substring(indexOfFirstBackSlash + 1, indexOfSecondBackSlash - indexOfFirstBackSlash - 1);
				return fileName.Substring(indexOfSecondBackSlash);
			}
		}

		static protected void CheckIfAlreadyAdded(ArrayList subContents, string rootName, TreeNode parent,
			string versionInfoFilename, guiAssetGamedataTreeTag.TREE_FOCUS level)
		{
			bool isAlreadyAdded = false;
			TreeNode alreadyNamedNode = null;

			foreach(TreeNode node in parent.Nodes)
			{
				if(node.Text.ToLower() == rootName)
				{
					isAlreadyAdded = true;
					alreadyNamedNode = node;
				}
			}
			if(!isAlreadyAdded)
			{
				TreeNode newParent = new TreeNode(rootName);
				// Create a tag for this node with subContents attached by a new reference
				newParent.Tag = new guiAssetGamedataTreeTag("", guiAssetGamedataTreeTag.TREE_FOCUS.PLATFORM, 
					false, new ArrayList(subContents));

				// Formerly used for recursion
//				assetTreeViewCreate_ParseSyncTargetFileNodes(subContents, newParent, versionInfoFilename, 
//					level, true);
				// Add a blank node for expansion
				newParent.Nodes.Add(new TreeNode(""));
				parent.Nodes.Add(newParent);
			}
			else
			{
				// Add the new data we have in subContents to what this node already has.
				guiAssetGamedataTreeTag alreadyNamedTag = (guiAssetGamedataTreeTag)alreadyNamedNode.Tag;
				ArrayList addMoreToCurrent = new ArrayList();
				foreach(string member in alreadyNamedTag.ListForNode)
				{
					addMoreToCurrent.Add(member);
				}
				foreach(string member in subContents)
				{
					addMoreToCurrent.Add(member);
				}
				alreadyNamedNode.Tag = new guiAssetGamedataTreeTag(alreadyNamedTag.FullFilename,
					alreadyNamedTag.Level, alreadyNamedTag.Execute, new ArrayList(addMoreToCurrent));
				// This was formerly used for recursion...
//				assetTreeViewCreate_ParseSyncTargetFileNodes( subContents, alreadyNamedNode, versionInfoFilename, 
//					level, true);
			}
		}

		static protected void assetTreeViewCreate_AddNodes(ArrayList contents, string versionInfoFilename, 
			TreeView tree, TreeNode parent,	guiAssetGamedataTreeTag.TREE_FOCUS level, int defaultImageIndex, bool createChildNode)
		{
			foreach(string name in contents)
			{
				assetTreeViewCreate_AddIndividualNodes(versionInfoFilename, name, tree, parent, level, 
					defaultImageIndex, createChildNode);
			}
		}

		static protected void assetTreeViewCreate_AddIndividualNodes(string versionInfoFilename, string name,
			TreeView tree, TreeNode parent,	guiAssetGamedataTreeTag.TREE_FOCUS level, int defaultImageIndex, bool createChildNode)
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
				tag = new guiAssetGamedataTreeTag(fullname.GetEncodedFilename(), level, true);
			}
			else
			{
				tag = new guiAssetGamedataTreeTag(asset.GetEncodedFilename(), level, false);
			}

			// Save the type of node this is (Key, Class, Group...)
			node.Tag = tag;

			// Don't add an extra child if we are at the end of the chain
			if (/*level != guiAssetGamedataTreeTag.TREE_FOCUS.LABEL ||*/ createChildNode)
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

		static protected TreeNode assetTreeViewCreate_GetTreeNodeWithIcon(string name, int defaultIndex)
		{
			// Set the imageIndex for a new TreeNode, create the TreeNode, and create a blank tag for the new node
			int imageIndex = MogUtil_AssetIcons.GetClassIconIndex( name );
			if (imageIndex == 0) imageIndex = defaultIndex;
			return new TreeNode(name, imageIndex , 0);
		}

		static protected MOG_Filename GetAssetFullname(MOG_Filename asset, string contentsInfo)
		{
			// The source will be out in the blessed asset tree
			return MOG_ControllerRepository.GetAssetBlessedPath(asset);
		}
	} // end class
} // end namespace 
