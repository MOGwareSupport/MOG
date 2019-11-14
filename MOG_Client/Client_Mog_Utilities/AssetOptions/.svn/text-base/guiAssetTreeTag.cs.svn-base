using System;
using System.Collections;
using System.Windows.Forms;


namespace MOG_Client.Client_Mog_Utilities.AssetOptions
{
	/// <summary>
	/// Summary description for guiAssetTreeTag.
	/// </summary>
	public class guiAssetTreeTag
	{
		public enum TREE_FOCUS {BASE, KEY, CLASS, SUBCLASS, LABEL, VERSION, SUBVERSION, PACKAGE};
		
		protected TREE_FOCUS mLevel;
		protected string mFullFilename;
		protected bool mExecuteCommand;
		public object mItemPtr;
		
		public TREE_FOCUS Level { get{return mLevel;} set{mLevel = value;}}		// The node level within the tree
		public string FullFilename { get{return mFullFilename;} }				// Full filename to this asset
		public bool Execute { get{return mExecuteCommand;} }					// Should we allow tools to be run on this node?

		public guiAssetTreeTag(string fullname, TREE_FOCUS level, bool execute)
		{
			mLevel = level;
			mFullFilename = fullname;
			mExecuteCommand = execute;
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
/*
		public ListViewItem Item 
		{
			get
			{
				if (mItemPtr.GetType == Type.GetType("System.Windows.Forms.ListViewItem"))
				{
					return (ListViewItem)mItemPtr;
				}
			}
		}

		public TreeNode Item 
		{
			get
			{
				if (mItemPtr.GetType == Type.GetType("System.Windows.Forms.TreeNode"))
				{
					return (TreeNode)mItemPtr;
				
				}
			}
		}

		public void Item
		{
			set
			{
				mItemPtr = value;
			}
		}
		*/

		public void ItemRemove()
		{
			if (mItemPtr.GetType().ToString() == "System.Windows.Forms.ListViewItem")
			{
				ListViewItem item = (ListViewItem)mItemPtr;
				item.Remove();				
			}
			else if (mItemPtr.GetType().ToString() == "System.Windows.Forms.TreeNode")
			{
				TreeNode node = (TreeNode)mItemPtr;
				node.Remove();
			}
		}

		public string ItemFullname()
		{
			if (mItemPtr.GetType().ToString() == "System.Windows.Forms.ListViewItem")
			{
				ListViewItem item = (ListViewItem)mItemPtr;
				return item.Text;
			}
			else if (mItemPtr.GetType().ToString() == "System.Windows.Forms.TreeNode")
			{
				TreeNode node = (TreeNode)mItemPtr;
				return node.FullPath;
			}

			return "";
		}
	}
}
