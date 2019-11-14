using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.IO;

using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERSYNCDATA;
using MOG.CONTROLLER.CONTROLLERREPOSITORY;
using MOG.FILENAME;
using MOG.DATABASE;
using MOG.DOSUTILS;
using MOG.REPORT;

using MOG_ControlsLibrary.Utils;

namespace MOG_ControlsLibrary.Controls
{
	public class SyncTargetFolder
	{
		private string mPath;
		public string Path
		{
			get { return mPath; }
		}

		private List<MOG_DBSyncTargetInfo> mFiles = new List<MOG_DBSyncTargetInfo>();
		public List<MOG_DBSyncTargetInfo> Files
		{
			get { return mFiles; }
		}

		public SyncTargetFolder(string path)
		{
			mPath = path;
		}

		public void AddFile(MOG_DBSyncTargetInfo file)
		{
			mFiles.Add(file);
		}
	}

	public class SyncTargetPlatform
	{
		private string mName;
		public string Name
		{
			get { return mName; }
		}

		private SortedDictionary<string, SyncTargetFolder> mFolders = new SortedDictionary<string, SyncTargetFolder>();
		public SortedDictionary<string, SyncTargetFolder> Folders
		{
			get { return mFolders; }
		}

		public SyncTargetPlatform(string name)
		{
			mName = name;
		}

		public void AddFolder(string path)
		{
			mFolders.Add(path, new SyncTargetFolder(path));
		}

		public bool HasFolder(string path)
		{
			return mFolders.ContainsKey(path);
		}

		public SyncTargetFolder GetFolder(string path)
		{
			return mFolders[path] as SyncTargetFolder;
		}
	}

	public class SyncTargetFileManager
	{
		private HybridDictionary mPlatforms = new HybridDictionary(true);

		public SyncTargetFileManager()
		{

		}

		public void AddPlatform(string platformName)
		{
			if (!mPlatforms.Contains(platformName))
			{
				mPlatforms.Add(platformName, new SyncTargetPlatform(platformName));
			}
		}

		public SyncTargetPlatform GetPlatform(string platformName)
		{
			if (mPlatforms.Contains(platformName))
			{
				return mPlatforms[platformName] as SyncTargetPlatform;
			}

			return null;
		}

		public void RemoveAllPlatforms()
		{
			mPlatforms.Clear();
		}
	}

	/// <summary>
	/// Summary description for MogControl_SyncTargetTreeView.
	/// </summary>
	public class MogControl_SyncTargetTreeView : MogControl_PropertyClassificationTreeView
	{
		SyncTargetFileManager mSyncTargetFileManager = new SyncTargetFileManager();

		public MogControl_SyncTargetTreeView()
			: base()
		{
		}

		protected override void ExpandTreeDown()
		{
			// Change mouse to hourglass
			UseWaitCursor = true;
			this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

			CreateSyncTargetRootNode();

			this.Cursor = System.Windows.Forms.Cursors.Default;
			UseWaitCursor = false;
		}

		protected override void ExpandTreeDown(TreeNode node)
		{
			if (node != null && node.Nodes.Count > 0 && node.Nodes[0].Text == Blank_Node_Text)
			{
				UseWaitCursor = true;
				this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

				if (IsAtAssetLevel(node) && this.ExpandAssets)
				{
					// Expand our Asset/Package nodes
					base.ExpandTreeDown(node);
				}
				else
				{
					node.Nodes.Clear();

					if (node.FullPath.IndexOf(PathSeparator) == -1)
					{
						// This is our first node, populate platform(s)
						ExpandSyncTargetPlatforms(node);
					}
					else
					{
						//This is a platform-specific node...
						string nodePlatform = GetPlatformNameFromFullPath(node.FullPath);

						Mog_BaseTag tag = node.Tag as Mog_BaseTag;
						if (tag != null)
						{
							MOG_Filename filename = new MOG_Filename(tag.FullFilename);
							if (filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
							{
								ExpandSyncTargetAssetNode(node, nodePlatform);
							}
							else
							{
								ExpandSyncTargetSubNodes(node, nodePlatform);
							}
						}
					}
				}
				this.Cursor = System.Windows.Forms.Cursors.Default;
				UseWaitCursor = false;
			}
		}

		/// <summary>
		/// Parses through our mSyncTargetFiles to figure out what should be added where
		/// </summary>
		private void ExpandSyncTargetSubNodes(TreeNode node, string platformName)
		{
			SyncTargetPlatform platform = mSyncTargetFileManager.GetPlatform(platformName);
			ArrayList fileNodes = new ArrayList();
			int baseFolderIndex = MogUtil_AssetIcons.GetClassIconIndex(BaseFolder_ImageText);

			// Go through each entry we created when we initialized
			foreach (KeyValuePair<string, SyncTargetFolder> entry in platform.Folders)
			{
				SyncTargetFolder folder = entry.Value as SyncTargetFolder;
				string relativePath = folder.Path;
				string nodePath = node.FullPath;

				if (String.Compare(relativePath, node.FullPath, true) == 0)
				{
					//This is the folder that matches the node we're expanding
					//Go through and get all the files from the folder so we can add nodes for them
					//We will add these to the current node after we finish going through and adding all the folders
					foreach (MOG_DBSyncTargetInfo info in folder.Files)
					{
						TreeNode fileNode = CreateSyncTargetTreeNode(info, platformName);						
						fileNodes.Add(fileNode);
					}
				}
				else if (relativePath.StartsWith(nodePath + PathSeparator, StringComparison.CurrentCultureIgnoreCase))
				{
					// We found a node with a path that is a parent to us in the hiererchy
					// Get rid of our current node's path (preparatory to using relativePath as a node.Text)
					relativePath = relativePath.Substring(nodePath.Length + PathSeparator.Length);
					if (relativePath.Length > 0)
					{
						//If there's a path separator we only want the first part before the separator
						if (relativePath.Contains(PathSeparator))
						{
							//Just grab the first part of the string, everything before the ~
							relativePath = relativePath.Substring(0, relativePath.IndexOf(PathSeparator));
						}

						if (!SyncTargetSubNodeExists(node, relativePath))
						{
							// create a new subnode to represent this folder
							TreeNode temp = new TreeNode(relativePath, new TreeNode[] { new TreeNode(Blank_Node_Text) });
							temp.Tag = new Mog_BaseTag(temp, temp.Text);
							node.Nodes.Add(temp);
							temp.ImageIndex = baseFolderIndex;
							temp.SelectedImageIndex = temp.ImageIndex;
						}
					}
				}
			}

			// Add all the file nodes we found above...
			foreach (TreeNode fileNode in fileNodes)
			{
				// Add each node and set its icon
				node.Nodes.Add(fileNode);
				Mog_BaseTag tag = fileNode.Tag as Mog_BaseTag;
				if (tag != null)
				{
					string assetFullFilename = tag.FullFilename;
					string foundFilename = FindAssetsFile(fileNode.Text, assetFullFilename);

					//Set the image for this node
					if (foundFilename.Length > 0 || (new MOG_Filename(assetFullFilename)).GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
					{
						//This is either a file or an asset
						SetImageIndices(fileNode, GetAssetFileImageIndex(foundFilename));
					}
					else
					{
						//This is a folder
						fileNode.ImageIndex = baseFolderIndex;
						fileNode.SelectedImageIndex = fileNode.ImageIndex;
					}
				}
			}
		}

		/// <summary>
		/// Adds all the platforms we need for further expansion of the sync target tree
		/// </summary>
		/// <param name="rootNode"></param>
		private void ExpandSyncTargetPlatforms(TreeNode rootNode)
		{
			mSyncTargetFileManager.RemoveAllPlatforms();

			// Populate our platform nodes
			string[] platforms = MOG_ControllerProject.GetProject().GetPlatformNames();
			foreach (string platform in platforms)
			{
				TreeNode platformNode = new TreeNode(platform, new TreeNode[] { new TreeNode(Blank_Node_Text) });
				platformNode.Tag = new Mog_BaseTag(platformNode, platform);
				rootNode.Nodes.Add(platformNode);
				platformNode.ImageIndex = MogUtil_AssetIcons.GetClassIconIndex(BaseFolder_ImageText);
				platformNode.SelectedImageIndex = platformNode.ImageIndex;

				ArrayList gamedataFiles = MOG_ControllerSyncData.GetAllSyncDataFileStructsForPlatform(platformNode.Text);

				// Initialize our tree to this platform
				InitializeSyncTargetFiles(platformNode.Text, gamedataFiles, platformNode.FullPath);

				// Automatically expand each of our platform nodes
				platformNode.Expand();
			}
		}

		/// <summary>
		/// Generates the root node for our project
		/// </summary>
		private TreeNode CreateSyncTargetRootNode()
		{
			Nodes.Clear();
			string projectName = MOG_ControllerProject.GetProjectName();
			TreeNode rootNode = new TreeNode(projectName, new TreeNode[] { new TreeNode(Blank_Node_Text) });
			rootNode.Tag = new Mog_BaseTag(rootNode, rootNode.Text);
			rootNode.Name = rootNode.Text;
			Nodes.Add(rootNode);
			rootNode.ImageIndex = MogUtil_AssetIcons.GetAssetIconIndex(rootNode.Text);

			rootNode.Expand();

			return rootNode;
		}

		/// <summary>
		/// Takes an array of gamedata files (returned from the database) and changes them into 
		///  a SortedList of files organized by (key) the fullpath of the node they should go under and
		///  (value) a SortedList of subNodes.  The SortedList created is saved as global.
		/// </summary>
		/// <param name="gamedataFiles">ArrayList of the gamedata files</param>
		/// <param name="fullPath">The project and platform given from the node.FullPath we are starting with</param>
		private void InitializeSyncTargetFiles(string platform, ArrayList files, string fullPath)
		{
			// Add our platform with a sorted list as its value
			mSyncTargetFileManager.AddPlatform(platform);

			foreach (MOG_DBSyncTargetInfo info in files)
			{
				AddSyncTargetFile(platform, info, fullPath);
			}
		}

		private void AddSyncTargetFile(string platformName, MOG_DBSyncTargetInfo info, string startingPath)
		{
			// Create a string for our gamedata Node's Path
			string gamedataFullpath = startingPath;

			// Don't bother if we haven't been initialized
			if (mSyncTargetFileManager != null)
			{
				// If we have a directory, add a separator and replace any backslashes with separators
				if (info.Path.Length > 0)
				{
					gamedataFullpath += PathSeparator + info.Path.Replace("~", PathSeparator);
				}

				// Get the gamedata files for this platform
				SyncTargetPlatform platform = mSyncTargetFileManager.GetPlatform(platformName);

				//Make sure the platform knows about this folder
				if (!platform.HasFolder(gamedataFullpath))
				{
					platform.AddFolder(gamedataFullpath);
				}

				SyncTargetFolder folder = platform.GetFolder(gamedataFullpath);
				folder.AddFile(info);
			}
		}

		private void ExpandSyncTargetAssetNode(TreeNode gamedataNode, string platform)
		{
			Mog_BaseTag gamedataTag = (Mog_BaseTag)gamedataNode.Tag;
			// If we have valid data...
			if (gamedataTag.AttachedSyncTargetInfo != null)
			{
				// Key is gamedataFilename, Value is gamedataFilenameOnly
				MOG_DBSyncTargetInfo gamedataInfo = gamedataTag.AttachedSyncTargetInfo;
				string currentVersionStamp = gamedataInfo.mVersion;
				MOG_Filename tempFilename = MOG_Filename.CreateAssetName(gamedataInfo.mAssetClassification, gamedataInfo.mAssetPlatform, gamedataInfo.mAssetLabel);
				MOG_Filename assetRealFile = MOG_ControllerRepository.GetAssetBlessedVersionPath(tempFilename, currentVersionStamp);

				// Add the asset this gamedata file is associated with under the oldGamedataNode
				TreeNode assetNode = new TreeNode(assetRealFile.GetAssetFullName(), new TreeNode[] { new TreeNode(Blank_Node_Text) });
				assetNode.Tag = new Mog_BaseTag(assetNode, assetRealFile.GetEncodedFilename(), LeafFocusLevel.RepositoryItems, true);
				assetNode.Name = assetRealFile.GetAssetFullName();
				gamedataNode.Nodes.Add(assetNode);
				SetImageIndices(assetNode, GetAssetFileImageIndex(assetRealFile.GetEncodedFilename()));
			}
		}

		protected TreeNode CreateSyncTargetTreeNode(MOG_DBSyncTargetInfo info, string platform)
		{
			bool ableToGetSourceFileAssetLinks = false;

			string currentVersionStamp = info.mVersion;
			MOG_Filename tempFilename = MOG_Filename.CreateAssetName(info.mAssetClassification, info.mAssetPlatform, info.mAssetLabel);
			MOG_Filename assetRealFile = MOG_ControllerRepository.GetAssetBlessedVersionPath(tempFilename, currentVersionStamp);

			// Create node with FocusLevel that does not plug into the BaseLeafTreeView
			TreeNode node = new TreeNode(info.FilenameOnly, new TreeNode[] { new TreeNode(Blank_Node_Text) });
			Mog_BaseTag tag = new Mog_BaseTag(node, assetRealFile.GetEncodedFilename(), RepositoryFocusLevel.Classification, true);
			node.Name = tempFilename.GetAssetFullName();
			tag.AttachedSyncTargetInfo = info;
			node.Tag = tag;

			string gamedataFilePath;
			if (ableToGetSourceFileAssetLinks)
				gamedataFilePath = assetRealFile.GetEncodedFilename() + "\\Files.Imported\\" + info.mSyncTargetFile;
			else
				gamedataFilePath = assetRealFile.GetEncodedFilename() + "\\Files.Imported\\" + info.FilenameOnly;

			//This is either a file or an asset
			SetImageIndices(node, base.GetAssetFileImageIndex(gamedataFilePath));

			return node;
		}

		#region Utility Methods
		/// <summary>
		/// Returns our platform name based on the FullPath property of TreeNode
		/// </summary>
		/// <param name="fullPath">The TreeNode::FullPath property (must have valid TreeView attached to use)</param>
		/// <returns>Platform name</returns>
		private string GetPlatformNameFromFullPath(string fullPath)
		{
			string[] pathParts = fullPath.Split(PathSeparator.ToCharArray());
			if (pathParts.Length > 1)
			{
				return pathParts[1];
			}
			return "";
		}

		/// <summary>
		/// TODO:  Finish commenting this
		/// </summary>
		/// <param name="node"></param>
		/// <param name="relativePath"></param>
		/// <returns></returns>
		private bool SyncTargetSubNodeExists(TreeNode node, string relativePath)
		{
			foreach (TreeNode subNode in node.Nodes)
			{
				if (String.Compare(subNode.Text, relativePath, true) == 0)
				{
					return true;
				}
			}
			return false;
		}
		#endregion Utility Methods
	}
}
