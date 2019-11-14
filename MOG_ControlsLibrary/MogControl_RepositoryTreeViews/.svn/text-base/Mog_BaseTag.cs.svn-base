using System;
using System.Collections;
using System.Windows.Forms;
using MOG.DATABASE;
using MOG.FILENAME;

namespace MOG_ControlsLibrary.Controls
{
	/// <summary>
	/// Allows for MogControl_BaseLeafTreeView and MogControl_RepositoryTreeView to transfer
	///		information on a per-node basis.
	/// </summary>
	public class Mog_BaseTag
	{
		#region Member variables and Properties
		public int NetworkId;
		public UInt32 CommandId;

		/// <summary>
		/// Returns the MogControl_BaseLeafTreeView.RootTreeType associated with the 
		/// owner of this tag
		/// </summary>
		private PackageNodeTypes mPackageNodeType;
		public PackageNodeTypes PackageNodeType
		{
			get { return mPackageNodeType; }
			set { mPackageNodeType = value; }
		}
		private RepositoryFocusLevel mRepositoryFocus;
		/// <summary>Level of focus this tag receives, programatically.</summary>
		public RepositoryFocusLevel RepositoryFocus
		{
			get { return mRepositoryFocus; }
			set { mRepositoryFocus = value; }
		}
		private LeafFocusLevel mLeafFocus;
		/// <summary>Level of focus this tag receives as a repository node</summary>
		public LeafFocusLevel LeafFocus
		{
			get { return mLeafFocus; }
			set { mLeafFocus = value; }
		}
		private string mFullFilename;
		/// <summary>Full filename of node this tag is attached to</summary>
		public string FullFilename
		{
			get { return mFullFilename; }
			set { mFullFilename = value; }
		}
		private string mPackageFullPath;
		/// <summary>Full package path of node this tag is attached to</summary>
		public string PackageFullPath
		{
			get { return mPackageFullPath; }
			set { mPackageFullPath = value; }
		}
		private bool mExecute;
		/// <summary>Indicates whether or not this tag should be executed</summary>
		public bool Execute
		{
			get { return mExecute; }
			set { mExecute = value; }
		}
		private object mOwner;
		/// <summary>Points to the owner of this Tag, when set</summary>
		public object Owner
		{
			get { return mOwner; }
			set { mOwner = value; }
		}
		private SortedList mAttachedPackageGroupNodes;
		/// <summary>
		/// Used to attach an ArrayList of gamedataFilenames to this tag
		/// </summary>
		public SortedList AttachedPackageGroupNodes
		{
			get { return mAttachedPackageGroupNodes; }
			set { mAttachedPackageGroupNodes = value; }
		}
		private ArrayList mAttachedClassifications;
		/// <summary>
		/// Used to attach a ArrayList of classifications to this tag
		/// </summary>
		public ArrayList AttachedClassifications
		{
			get { return mAttachedClassifications; }
			set { mAttachedClassifications = value; }
		}
		private ArrayList mAttachedRepositoryItems;
		/// <summary>
		/// Used to attach an ArrayList of repositoryItems (packages or assets) to this tag.
		/// </summary>
		public ArrayList AttachedRepositoryItems
		{
			get { return mAttachedRepositoryItems; }
			set { mAttachedRepositoryItems = value; }
		}
		private MOG_DBSyncTargetInfo mAttachedSyncTargetInfo;
		public MOG_DBSyncTargetInfo AttachedSyncTargetInfo
		{
			get { return mAttachedSyncTargetInfo; }
			set { mAttachedSyncTargetInfo = value; }
		}

		private string mPackageFullName;
		/// <summary> Gets and sets the Package Group name for this TreeTag </summary>
		public string PackageFullName
		{
			get
			{
				if (mPackageFullName != null)
					return mPackageFullName;
				else
					return "";
			}
			set
			{
				// This fix was added for SmartBomb
				// Bit of a kludge, but this fixes a problem when the packageName can contain an extra bogus '~'
				if (value.Contains("~{"))
				{
					// Because some of the trees can insert a bogus '~' after the classification we need to add this safeguard
					value = new MOG_Filename(value).GetAssetFullName();
				}

				mPackageFullName = value;
			}
		}
		#endregion Member variables and Properties

		public Mog_BaseTag(object owner, string fullFilename)
		{
			Owner = owner;
			FullFilename = fullFilename;
		}
		public Mog_BaseTag(object owner, string fullFilename, RepositoryFocusLevel repositoryFocus, bool execute)
			: this(owner, fullFilename)
		{
			mRepositoryFocus = repositoryFocus;
			mExecute = execute;
		}
		public Mog_BaseTag(object owner, string fullFilename, LeafFocusLevel leafFocus, bool execute)
			: this(owner, fullFilename)
		{
			mRepositoryFocus = RepositoryFocusLevel.Repository;

			mLeafFocus = leafFocus;
			mExecute = execute;
		}



		/// <summary> Override of object.ToString() </summary>
		/// <returns>Name string of Mog_BaseTag</returns>
		public override string ToString()
		{
			if (FullFilename != null && FullFilename.Length > 0)
			{
				return FullFilename;
			}
			else
			{
				return "";
			}
		}

		/// <summary>
		/// Remove the mItemPtr this Tag is attached to.
		/// </summary>
		public void ItemRemove()
		{
			if (mOwner is ListViewItem)
			{
				ListViewItem item = mOwner as ListViewItem;
				item.Remove();
			}
			else if (mOwner is TreeNode)
			{
				TreeNode node = mOwner as TreeNode;

				//Items in the archive treeview turn to red first, then get removed on the second delete
				MogControl_ArchivalTreeView archiveTree = node.TreeView as MogControl_ArchivalTreeView;
				if (archiveTree != null && node.ForeColor != MogControl_ArchivalTreeView.Archive_Color)
				{
					//This is only the first delete, so reinitializing the tree will change the item to red.
					archiveTree.DeInitialize();
					archiveTree.Initialize();
				}
				else
				{
					//Nothing fancy, just remove the node
					node.Remove();
				}
			}
		}

		/// <summary>
		/// Get the ListViewItem.Text or TreeNode.FullPath attribute this Tag is attached to.
		/// </summary>
		/// <returns>ListViewItem.Text or TreeNode.FullPath</returns>
		public string ItemFullname()
		{
			if (mOwner.GetType().ToString() == "System.Windows.Forms.ListViewItem")
			{
				ListViewItem item = (ListViewItem)mOwner;
				return item.Text;
			}
			else if (mOwner.GetType().ToString() == "System.Windows.Forms.TreeNode")
			{
				TreeNode node = (TreeNode)mOwner;
				return node.FullPath;
			}

			return "";
		}
	}

	/// <summary>Indicates the type of focus this tag should receive, programatically. (Repository and BaseLeaf)</summary>
	public enum RepositoryFocusLevel
	{
		/// <summary>Classification level: Anything before we get to the asset/package name</summary>
		Classification,
		/// <summary>Repository level: Indicates we are attached to a asset or package</summary>
		Repository,
		/// <summary>Total enums in this enumeration</summary>
		Count,
	};

	/// <summary>Indicates the level of focus we are at inside the repository item node. (BaseLeaf only)</summary>
	public enum LeafFocusLevel
	{
		/// <summary>Applies for version nodes only</summary>
		Version,
		/// <summary>Applies for items contained within a specific version of a repository item</summary>
		RepositoryItems,
		/// <summary>Applies to Contained Assets or Packages node</summary>
		ContainedItems,
		/// <summary>Applies to package group nodes (folder) within a packages contained items </summary>
		PackageGroup,
		/// <summary>Total enums in this enumeration</summary>
		Count,
	};

	// glk: Comment this out to find out what else I touched before the big file-overwrite accident...
	public enum PackageNodeTypes
	{
		None,
		Class,
		Asset,
		Package,
		Group,
		Object,
	};
}
