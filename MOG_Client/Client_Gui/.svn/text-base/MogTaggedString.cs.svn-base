using System;
using System.IO;
using MOG.CONTROLLER.CONTROLLERPROJECT;

// Offer this class to everyone who includes MOG_Client...
namespace MOG_Client
{
	/// <summary>
	/// A standard C# string with an object that can be attached to it.
	/// </summary>
	public class MogTaggedString
	{
		public enum FilenameStyleTypes
		{
			FullFilename,
			WorkspaceRelativeFilename,
			FolderRelativeFilename,
			BaseFilename,
			Raw
		}

		private string mName;
		public string Name
		{
			get { return mName; }
			set { mName = value; }
		}
		private object mAttachedItem;
		/// <summary>
		/// The object attached to this MogTaggedString
		/// </summary>
		public object AttachedItem
		{
			get
			{
				if( mAttachedItem != null )
					return mAttachedItem;
				else
					return "";
			}
			set
			{
				this.mAttachedItem = value;
			}
		}

		private string mFullName;
		public string FullName
		{
			get { return mFullName; }
			set { mFullName = value; }
		}

		private string mWorkspaceRelativeName;
		public string WorkspaceRelativeName
		{
			get { return mWorkspaceRelativeName; }
			set { mWorkspaceRelativeName = value; }
		}

		private string mFolderRelativeName;
		public string FolderRelativeName
		{
			get { return mFolderRelativeName; }
			set { mFolderRelativeName = value; }
		}

		private FilenameStyleTypes mFilenameStyle = FilenameStyleTypes.Raw;
		public FilenameStyleTypes FilenameStyle
		{
			get { return mFilenameStyle; }
			set { mFilenameStyle = value; }
		}	

		/// <summary>
		/// Initialize a new MogTaggedString, which can have an object it points to.
		/// </summary>
		/// <param name="nameString">The actual string to be displayed to the user</param>
		/// <param name="attachedItem">The attached object for design-time/run-time use</param>
		/// <param name="filenameStyle"></param>
		public MogTaggedString(string nameString, object attachedItem, FilenameStyleTypes filenameStyle, string relativeFolder)
		{
			this.FullName = nameString;
			this.Name = Path.GetFileName(FullName);
			this.mAttachedItem = attachedItem;
			this.mFilenameStyle = filenameStyle;

			// Build the WorkspaceRelativeName
			this.mWorkspaceRelativeName = nameString;
			string testDirectory = MOG_ControllerProject.GetWorkspaceDirectory().TrimEnd("\\".ToCharArray()) + "\\";
			if (WorkspaceRelativeName.StartsWith(testDirectory, StringComparison.CurrentCultureIgnoreCase))
			{
				WorkspaceRelativeName = WorkspaceRelativeName.Substring(testDirectory.Length);
			}

			// Build the FolderRelativeName
			this.mFolderRelativeName = nameString;
			testDirectory = relativeFolder.TrimEnd("\\".ToCharArray()) + "\\";
			if (FolderRelativeName.StartsWith(testDirectory, StringComparison.CurrentCultureIgnoreCase))
			{
				FolderRelativeName = FolderRelativeName.Substring(testDirectory.Length);
			}
		}

		/// <summary>
		/// Returns the display name of this MogTaggedString
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			switch (FilenameStyle)
			{
				case FilenameStyleTypes.FullFilename:
					if (FullName != null)
						return FullName;
					else
						return "";
				case FilenameStyleTypes.WorkspaceRelativeFilename:
					if (WorkspaceRelativeName != null)
						return WorkspaceRelativeName;
					else
						return "";
				case FilenameStyleTypes.FolderRelativeFilename:
					if (FolderRelativeName != null)
						return FolderRelativeName;
					else
						return "";
				case FilenameStyleTypes.BaseFilename:
					if (FullName != null)
						return Path.GetFileName(FullName);
					else
						return "";
				case FilenameStyleTypes.Raw:
					if (Name != null)
						return Name;
					else
						return "";
				default:
					if (Name != null)
						return Name;
					else
						return "";
			}
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			MogTaggedString right = obj as MogTaggedString;
			if (right != null)
			{
				return string.Compare(FullName, right.FullName, true) == 0;
			}
			return base.Equals(obj);
		}
	}
}