using System;
using System.Collections;

namespace MOG_Client.Client_Mog_Utilities.AssetOptions
{
	/// <summary>
	/// Summary description for guiAssetGamedataTreeTag.
	/// </summary>
	public class guiAssetGamedataTreeTag : guiAssetTreeTag
	{
		// Override TREE_FOCUS and mLevel with `new` keyword
		public new enum TREE_FOCUS { BASE, PLATFORM, KEY, X, Y, Z };
		protected new TREE_FOCUS mLevel;
		public new TREE_FOCUS Level { get{return mLevel;} set{mLevel = value;}}	
		protected ArrayList mListForNode;
		public ArrayList ListForNode
		{
			get
			{
				// If we have nothing, return blank arrayList
				if(mListForNode == null)
					return new ArrayList();
					// Else return the reference we have been assigned
				else
					return mListForNode;
			}
			set	{	this.mListForNode = value;	}
		}

		/// <summary>
		/// Full Constructor with all options enabled.=
		/// </summary>
		/// <param name="fullname"></param>
		/// <param name="level"></param>
		/// <param name="execute"></param>
		/// <param name="listForNode"></param>
		public guiAssetGamedataTreeTag(string fullname, TREE_FOCUS level, bool execute, ArrayList listForNode)
		{
			mLevel = level;
			base.mFullFilename = fullname;
			base.mExecuteCommand = execute;
			mListForNode = listForNode;
		}

		/// <summary>
		/// Instantiates with null for ArrayList (ListForNode)
		/// </summary>
		/// <param name="fullname"></param>
		/// <param name="level"></param>
		/// <param name="execute"></param>
		public guiAssetGamedataTreeTag(string fullname, TREE_FOCUS level, bool execute)
			:base(fullname, (guiAssetTreeTag.TREE_FOCUS)level, execute)
		{		
			this.mLevel = level;
			this.mExecuteCommand = execute;
		}

		/// <summary>
		/// Instantiates with false for execute and null for ArrayList (ListForNode)
		/// </summary>
		/// <param name="fullname"></param>
		/// <param name="level"></param>
		public guiAssetGamedataTreeTag(string fullname, TREE_FOCUS level)
			:this(fullname, level, false, null)
		{		}

		/// <summary>
		/// Instantiates with focus of KEY, false for execute, and null for ArrayList (ListForNode)
		/// </summary>
		/// <param name="fullname"></param>
		public guiAssetGamedataTreeTag(string fullname)
			:this(fullname, TREE_FOCUS.KEY, false, null)
		{		}
	}
}
