using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;

using MOG;
using MOG.INI;
using MOG.USER;
using MOG.DOSUTILS;
using MOG.FILENAME;
using MOG.PROJECT;
using MOG.REPORT;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;



namespace MOG_Client.Client_Mog_Utilities.AssetOptions
{
	/// <summary>
	/// Summary description for guiAssets.
	/// </summary>
	public class guiAssetIcon
	{
		static private ImageList mAssetTypeImages	= new ImageList();				// Holds all the asset type icons
		static private ArrayList mAssetTypes		= new ArrayList();				// This will be a look-up table for the imageList allowing us to look up types and get the correct icon

		static private ImageList mStateTypeImages	= new ImageList();				// Holds all the asset state icons
		static private ArrayList mStateTypes		= new ArrayList();				// This will be a look-up table for the imageList allowing us to look up states and get the correct icon
		
		static public ImageList Images { get{return mAssetTypeImages;} set{mAssetTypeImages = value;}}
		static public ArrayList TypeNames { get{return mAssetTypes;} set{mAssetTypes = value;}}
		
		static public ImageList StateImages { get{return mStateTypeImages;} set{mStateTypeImages = value;}}
		static public ArrayList StateNames { get{return mStateTypes;} set{mStateTypes = value;}}

		public guiAssetIcon()
		{
		}

		/// <summary>
		/// Reset the currently loaded project icons preparatory to loading a new set
		/// </summary>
		static public void AssetIconsClear()
		{			
			mAssetTypeImages = new ImageList();	
			mAssetTypes = new ArrayList();

			mStateTypeImages = new ImageList();
			mStateTypes = new ArrayList();
		}

		/// <summary> InitializeAssetIcons
		/// Loads all the bmp's specified in the asset declarations
		/// in the Project.ini files.  Each bmp is added to a
		/// list allong with its key added to a corresponding list
		/// for later searching.
		/// </summary>
		static public void AssetIconInitialize()
		{
			// Check to see if our project is loaded
			if (!MOG_ControllerProject.IsProject())
			{
				return;
			}

			// Only allow population of the images array once
			if (mAssetTypes.Count > 0)
			{
				return;
			}
			
			// Add the active item icon first
			// Get the image
			if (DosUtils.FileExist(string.Concat(MOG_ControllerSystem.GetSystem().GetSystemToolsPath(), "\\Images\\SelectIcon.bmp")))
			{
				// Get the group image
				Image myImage = new Bitmap(string.Concat(MOG_ControllerSystem.GetSystem().GetSystemToolsPath(), "\\Images\\SelectIcon.bmp"));

				// Add the image and the type to the arrayLists
				mAssetTypeImages.Images.Add(myImage);
				mAssetTypes.Add("dot");
			}

            // Open the project.ini
			MOG_Ini ini = new MOG_Ini(MOG_ControllerProject.GetProject().GetProjectConfigFilename());

			// Walk through all the assets
			for( int x = 0; x < ini.CountKeys("Assets"); x++)
			{
				// Get the asset name
				string imageName = ini.GetString(ini.GetKeyNameByIndex("Assets",x), "Icon");
				// Check if we have an image?
				if (imageName.Length > 0)
				{
					string assetKey = ini.GetKeyNameByIndex("Assets",x).ToLower();
					LoadIcon(imageName, ini.GetKeyNameByIndex("Assets",x).ToLower());
					
					// Check for a lock image
					string lockImageName = Path.GetFileNameWithoutExtension(imageName) + "_locked";
					string lockFullImageName = imageName.Replace(Path.GetFileNameWithoutExtension(imageName), lockImageName);
					LoadIcon(lockFullImageName, assetKey + "_locked");

					// Check for a ReadLock image
					lockImageName = Path.GetFileNameWithoutExtension(imageName) + "_readlocked";
					lockFullImageName = imageName.Replace(Path.GetFileNameWithoutExtension(imageName), lockImageName);
					LoadIcon(lockFullImageName, assetKey + "_readlocked");
				}
			}

			if (DosUtils.FileExist(string.Concat(MOG_ControllerSystem.GetSystem().GetSystemToolsPath(), "\\Images\\Group.bmp")))
			{
				// Get the group image
				Image myImage = new Bitmap(string.Concat(MOG_ControllerSystem.GetSystem().GetSystemToolsPath(), "\\Images\\Group.bmp"));

				// Add the image and the type to the arrayLists
				mAssetTypeImages.Images.Add(myImage);
				mAssetTypes.Add("group");
			}

			mAssetTypeImages.TransparentColor = Color.Magenta;

			// Initialize state icons
			if (DosUtils.DirectoryExist(MOG_ControllerSystem.GetSystem().GetSystemToolsPath() + "\\Images\\States"))
			{
				FileInfo []stateImages = DosUtils.FileGetList(MOG_ControllerSystem.GetSystem().GetSystemToolsPath() + "\\Images\\States", "*.bmp");

				foreach (FileInfo stateImage in stateImages)
				{
					LoadRawIcon(mStateTypeImages, mStateTypes, stateImage.FullName, Path.GetFileNameWithoutExtension(stateImage.Name));
				}
			}
		}

		static private void LoadRawIcon(ImageList myImages, ArrayList myImageNames, string imageFileName, string imageName)
		{
			// Make sure that the icon file exists
			if (DosUtils.FileExist(imageFileName))
			{
				// Get the image
				Image myImage = new Bitmap(imageFileName);

				// Add the image and the type to the arrayLists
				myImages.Images.Add(myImage);
				myImageNames.Add(imageName.ToLower());
			}			
		}

		static private void LoadIcon(string imageFileName, string assetKey)
		{
			// Load the icon specified by the Icon= key
			string iconName = String.Concat(MOG_ControllerProject.GetProject().GetProjectToolsPath(), "\\", imageFileName);

			// Make sure that the icon file exists
			if (DosUtils.FileExist(iconName))
			{
				// Get the image
				Image myImage = new Bitmap(iconName);

				// Add the image and the type to the arrayLists
				mAssetTypeImages.Images.Add(myImage);
				mAssetTypes.Add(assetKey.ToLower());
			}			
		}

		/// <summary>
		/// Gets the MOGAssetStatus for a given status name
		/// </summary>
		static public int GetStatusIndex(string status)
		{
			/// Loops through statusInfos ArrayList in order
			/// to find StatusInfo.Text that matches parameter 
			/// 'status'
			for (int i = 0; i < mStateTypes.Count; i++)
			{
				if (string.Compare( status, (string)mStateTypes[i], true) == 0)
				{
					return i;
				}
			}
 
			// If there was no match, return error
			return GetStatusIndex("unknown");
		} 

		/// <summary> SetAssetIcon
		/// Searches through mAssetTypes to find the matching key with 
		/// that of the filename.  Then returns the index
		/// </summary>
		/// <param name="filename"></param>
		/// <returns>index of icon in the mAssetTypeImages list</returns>
		static public int SetAssetIcon(String filename)
		{
			// Construct a filename
			MOG_Filename file = null;
			try
			{
				file = new MOG_Filename(filename);
			}
			catch(Exception e)
			{
				e.ToString();
				return 0;
			}

			string classification;

			switch(file.GetType())
			{
				case MOG_FILENAME_TYPE.MOG_FILENAME_Asset:
					classification = file.GetAssetClassification();
					break;
				case MOG_FILENAME_TYPE.MOG_FILENAME_Group:
					classification = "group";
					break;
				default:
					classification = filename;
					break;
			}

			// Check for locks on this asset
			if (!guiAssetSourceLock.QueryPersistentLock(file.GetAssetFullName(), true))
			{
				// Check if this asset is locked by our current user
				if (guiAssetSourceLock.OkToBless(file.GetAssetFullName(), true))
				{
					// Look for an icon type of _readLocked
					int y = mAssetTypes.IndexOf(classification + "_readlocked");
					if (y == -1)
					{
						// Can't find it, use default
						return 0;
					}
					else
					{
						// Return the location of the _readLocked icon
						return y;
					}
				}
				else
				{
					// Look for an icon type of _locked
					int y = mAssetTypes.IndexOf(classification + "_locked");
					if (y == -1)
					{
						// Can't find it, use default
						return 0;
					}
					else
					{
						// Return the location of the _locked icon
						return y;
					}
				}
			}
			else
			{			
				// Get the index of the key in the types array
				int x = mAssetTypes.IndexOf(classification);

				// If the asset was not found, return 0 for first icon in the list
				if (x == -1)
				{
					return 0;
				}

				// Return the index of the icon
				return x;
			}
		}
	}
}
