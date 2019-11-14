namespace MOG_ControlsLibrary
{
	public class MOG_AssetTreeTag
	{
		public enum TREE_FOCUS {BASE, KEY, CLASS, SUBCLASS, LABEL, VERSION, SUBVERSION, PACKAGE};
		
		protected TREE_FOCUS mLevel;
		protected string mFullFilename;
		protected bool mExecuteCommand;
		protected object mItemPtr;
		protected object mObj;
		
		public TREE_FOCUS Level { get{return mLevel;} set{mLevel = value;}}		// The node level within the tree
		public string FullFilename { get{return mFullFilename;} }				// Full filename to this asset
		public bool Execute { get{return mExecuteCommand;} }					// Should we allow tools to be run on this node?

		public MOG_AssetTreeTag(string fullname, object item, object obj)
		{
			mObj = obj;
			mFullFilename = fullname;
			mExecuteCommand = true;
			mItemPtr = item;
		}
		public MOG_AssetTreeTag(string fullname, object item)
		{
			mFullFilename = fullname;
			mExecuteCommand = true;
			mItemPtr = item;
		}

		public MOG_AssetTreeTag(string fullname, TREE_FOCUS level)
		{
			mLevel = level;
			mFullFilename = fullname;
			mExecuteCommand = false;
		}

		public MOG_AssetTreeTag(string fullname)
		{
			mLevel = TREE_FOCUS.KEY;
			mFullFilename = fullname;
			mExecuteCommand = false;
		}

		protected MOG_AssetTreeTag()
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
}