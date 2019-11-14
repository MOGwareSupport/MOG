using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System;

using MOG.FILENAME;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.PROMPT;
using System.Collections.Specialized;
using MOG.REPORT;

namespace MOG_ControlsLibrary
{
	/// <summary>
	/// A Directory set is a fast data tree of files and folders that is stored in ram like a directory structure
	/// </summary>
	public class DirectorySet
	{
		protected HybridDictionary mBank;

		/// <summary>
		/// Returns a set of files that are contained within the desired folder path
		/// </summary>
		/// <param name="folder"></param>
		/// <returns></returns>
		public HybridDictionary GetFiles(string folder)
		{
			if (folder.Length == 0)
			{
				folder = "<Root>";
			}

			if (mBank.Contains(folder))
			{
				DirectorySetInfo currentFolder = mBank[folder] as DirectorySetInfo;
				return currentFolder.Contents;
			}

			return null;
		}

		/// <summary>
		/// Returns a set of files that are contained within the desired folder path
		/// </summary>
		/// <param name="folder"></param>
		/// <returns></returns>
		public bool HasChildren(string folder, bool caseSensitive)
		{
			if (folder.Length == 0)
			{
				folder = "<Root>";
			}

			if (mBank.Contains(folder))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Returns a set of files that are contained within the desired folder path
		/// </summary>
		/// <param name="folder"></param>
		/// <returns></returns>
		public bool HasFolderChildren(string folder, bool caseSensitive)
		{
			bool bFolder = false;

			if (folder.Length == 0)
			{
				folder = "<Root>";
			}

			if (mBank.Contains(folder))
			{
				DirectorySetInfo currentFolder = mBank[folder] as DirectorySetInfo;
				

				IDictionaryEnumerator file = currentFolder.Contents.GetEnumerator();

				// Now itterate through those children
				while(file.MoveNext())
				{
					DirectorySetInfo fileInfo = file.Value as DirectorySetInfo;
					if (fileInfo.Type == DirectorySetInfo.TYPE.Folder || fileInfo.Type == DirectorySetInfo.TYPE.FolderName)
					{
						bFolder = true;
						break;
					}
				}
			}

			return bFolder;
		}

		/// <summary>
		/// Constructor takes an arraylist of files.  We take this files and create a ram tree
		/// </summary>
		/// <param name="files"></param>
		public DirectorySet(ArrayList files)
		{
			mBank = new HybridDictionary(true);
			foreach (string file in files)
			{
				try
				{
					AddFilename(Path.GetDirectoryName(file), file, false);
				}
				catch(Exception e)
				{
					if (e.Message == "trying to add empty key")
					{
						string message = "Trying to add an asset with an empty key!\n\n" 
							+ "Binary Name: " 
							+  file + "\n";

						MOG_Prompt.PromptMessage("Directory Set", message, e.StackTrace);
					}
					else if (e.Message == "duplicate key")
					{
						string message = "Two or more duplicate asset files are named the same and are pointing to the same target in your project!\n\n" 
							+ "Binary Name: " 
							+  file + "\n"
							+ "Asset Name(s):" + "\n\tCan't determine without valid local workspace!\n"
							+ "\nRemove one from the project trees to fix this conflict! Until this is fixed this asset will not be shown in your asset tree.";

						if ( MOG_ControllerProject.GetCurrentSyncDataController() != null)
						{
							string duplicateAssets = "";
							ArrayList Assets = MOG_ControllerProject.MapFilenameToAssetName(file, MOG_ControllerProject.GetCurrentSyncDataController().GetPlatformName());
							if (Assets != null)
							{
								foreach (MOG_Filename asset in Assets)
								{
									// Make sure we have a valid match?
									if (asset != null &&
										asset.GetOriginalFilename().Length > 0)
									{
										if (duplicateAssets.Length == 0)
										{
											duplicateAssets = "Asset Name(s): \n\t" + asset.GetAssetFullName();
										}
										else
										{
											duplicateAssets = duplicateAssets + "\n\t" + asset.GetAssetFullName();
										}
									}
								}
							}
						
							message = "Two or more duplicate asset files are named the same and are pointing to the same target in your project!\n\n" 
								+ "Binary Name: " 
								+  file + "\n"
								+ duplicateAssets + "\n"
								+ "\nRemove one of the above Assets from the project trees to fix this conflict! Until this is fixed this asset will not be shown in your  tree.";
						}

						MOG_Prompt.PromptMessage("Directory Set", message, e.StackTrace);
					}
					else
					{
						string message = "Unhandled exception occurred while trying to add this file.\n"
							+ "FILE: " + file + "\n\n"
							+ e.Message;
						MOG_Prompt.PromptMessage("Directory Set", message, e.StackTrace);
					}
				}
			}
		}

		/// <summary>
		/// Add a folder or file to the tree
		/// </summary>
		/// <param name="directory"></param>
		/// <param name="file"></param>
		private void AddFilename(string directory, string file, bool isFolder)
		{
			// Blank directories are not allowed in the b-tree so I convert to a <root> token
			if (directory.Length == 0)
			{
				directory = "<Root>";
			}

			DirectorySetInfo currentFolder = null;

			// Does this folder already exist?
			if (mBank.Contains(directory) == false)
			{
				// If we are not the root, we should add it to our parent folder
				if (directory != "<Root>")
				{
					// Add this folder to its parent
					string parentDir = GetParentDirectory(directory);
					AddFilename(parentDir, directory, true);
				}

				// Make a new folder branch
				currentFolder = new DirectorySetInfo(directory, DirectorySetInfo.TYPE.Folder);

				// Add it to our main tree
				mBank.Add(directory, currentFolder);
			}
			else
			{
				// Ah, it exists.  Get a handle to it
				currentFolder = mBank[directory] as DirectorySetInfo;
			}

			// Now we add the file or folder to this current folder
			if (isFolder)
			{
				// We only want to add the folder name proper to this current folder, so strip off every thing but the name
				string subFolder = RemoveParentDirectory(file, directory);
				subFolder = subFolder.TrimStart("\\".ToCharArray());
				currentFolder.AddFolder(subFolder);				
			}
			else
			{
				currentFolder.AddFile(Path.GetFileName(file));				
			}
		}
        
		private string RemoveParentDirectory(string folder, string parentFolder)
		{
			string subFolder = "";

			// Do we have a parent directory
			if (folder.IndexOf("\\") != -1)
			{
				int x = 0;
				while(x < parentFolder.Length && parentFolder[x] == folder[x])
				{
					subFolder = folder.Substring(++x);
				}

				// now remove any leading '\\'
				subFolder = subFolder.TrimStart("\\".ToCharArray());
			}
			else
			{
				subFolder = folder;
			}
			return subFolder;
		}

		private string GetParentDirectory(string folder)
		{
			string parent = "";

			if (folder.LastIndexOf("\\") != -1)
			{
				int width = folder.Length - folder.LastIndexOf("\\");
				parent = folder.Substring(0, folder.Length - width);
			}
			return parent;
		}	
	}

	public class DirectorySetInfo
	{
		public enum TYPE {Folder, FolderName, File};
		protected int mDepth;
		protected TYPE mType;
		protected HybridDictionary mContents;
		protected string mFolderName;
		protected string mName;
		protected string mParentFolderName;
		
		public int Depth { get{return mDepth;} }								// Directory depth
		public string FolderName { get{return mFolderName;} }					// Directory depth
		public string Name { get{return mName;} }								// Filename
		public string ParentFolderName { get{return mParentFolderName;} }		// Directory depth
		public HybridDictionary Contents { get { return mContents; } }			// Directory contents
		public TYPE Type { get{return mType;} }									// Directory contents

		public DirectorySetInfo(string fullname, TYPE type)
		{
			mDepth = CountDepth(fullname);
			mType = type;
			mName = fullname;

			if (mType == TYPE.Folder)
			{
				mFolderName = fullname;
				mContents = new HybridDictionary(true);
			}
		}
		
		public DirectorySetInfo(string fullname, string parentFolder, TYPE type)
		{
			mDepth = CountDepth(fullname);
			mType = type;
			mParentFolderName = parentFolder;
			mName = fullname;

			if (mType == TYPE.Folder)
			{
				mFolderName = fullname;
				mContents = new HybridDictionary(true);
			}
		}

		public bool AddFile(string filename)
		{
			// We can only add to folder type objects
			if (mType == TYPE.Folder && 
				mContents != null)
			{
				// Make sure it doesn't already exist
				if (!mContents.Contains(filename))
				{
					// Add this file
					mContents.Add(filename, new DirectorySetInfo(filename, FolderName, DirectorySetInfo.TYPE.File));
					//Debug.Write(filename, "\nAdded File to - " + FolderName);			
				}
				else
				{
					// Warn the user they have a colliding asset
					string message = "Multiple assets are syncing to the same local filename.\n" +
									 "FILENAME: " + Path.Combine(FolderName, filename) + "\n\n" +
									 "The coliding assets can be seen from the project's Sync View tree.\n" + 
									 "Please remove the extra colliding assets from the project.";
					MOG_Report.ReportMessage("Colliding Assets Detected", message, "", MOG_ALERT_LEVEL.ERROR);
					return false;
				}

				return true;
			}

			return false;
		}

		public bool AddFolder(string foldername)
		{
			// We can only add to folder type objects
			if (mType == TYPE.Folder &&
				mContents != null)
			{
				// Make sure it doesn't already exist
				if (!mContents.Contains(foldername))
				{
					// Add this folder
					mContents.Add(foldername, new DirectorySetInfo(foldername, FolderName, DirectorySetInfo.TYPE.FolderName));
					//Debug.Write(foldername, "\nAdded Folder to - " + FolderName);			
				}
				return true;
			}

			return false;
		}		

		private int CountDepth(string path)
		{
			int depth = 0;
			int index = path.IndexOf("\\");
			while (index != -1)
			{
				if (path.Length >= index +1)
				{
					index = path.IndexOf("\\", index + 1);
					depth++;
				}
				else
				{
					break;
				}
			}

			return depth;
		}
	}
}