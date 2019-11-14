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
using MOG.PROPERTIES;
using MOG.REPORT;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;

using Etier.IconHelper;



namespace MOG_ControlsLibrary.Utils
{
	/// <summary>
	/// Summary description for guiAssets.
	/// </summary>
	public class MogUtil_AssetIcons
	{
		#region Variables, Properties
		// Asset icon variables
		static private ImageList mAssetTypeImages	= new ImageList();				// Holds all the asset type icons
		static private ArrayList mAssetTypes		= new ArrayList();				// This will be a look-up table for the imageList allowing us to look up types and get the correct icon

		// State icon variables
		static private ImageList mStateTypeImages	= new ImageList();				// Holds all the asset state icons
		static private ArrayList mStateTypes		= new ArrayList();				// This will be a look-up table for the imageList allowing us to look up states and get the correct icon

		// File icon variables
			// Win32 wrapper class that returns our indices and adds images for files at runtime
		static private IconListManager mFileTypeManager;
		
		/// <summary>
		/// ImagesList for all cached classification icons
		/// </summary>
		static public ImageList Images { get{return mAssetTypeImages;} set{mAssetTypeImages = value;}}
		/// <summary>
		/// ArrayList of all the classes that correspond with the cached images
		/// </summary>
		static private ArrayList TypeNames { get{return mAssetTypes;} set{mAssetTypes = value;}}
		
		/// <summary>
		/// ImageList for all cached state icons
		/// </summary>
		static public ImageList StateImages { get{return mStateTypeImages;} set{mStateTypeImages = value;}}
		/// <summary>
		/// Array list that corresponds to all cached state icons
		/// </summary>
		static private ArrayList StateNames { get{return mStateTypes;} set{mStateTypes = value;}}
		#endregion Variables, Properties

		/// <summary>
		/// Base constructor
		/// </summary>
		public MogUtil_AssetIcons()
		{
		}

		/// <summary>
		/// Reset the currently loaded project icons preparatory to loading a new set
		/// </summary>
		static public void ClassIconsClear()
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
		static public void ClassIconInitialize()
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

			lock (mAssetTypes)
			{
				// Add the active item icon first
				// Get the image
				if (DosUtils.FileExist(MOG_ControllerSystem.LocateTool("Images", "BaseNode.bmp")))
				{
					// Get the group image
					Image myImage = new Bitmap(MOG_ControllerSystem.LocateTool("Images", "BaseNode.bmp"));

					// Add the image and the type to the arrayLists
					mAssetTypeImages.Images.Add(myImage);
					mAssetTypes.Add("dot");
				}

				if (DosUtils.FileExist(MOG_ControllerSystem.LocateTool("Images", "Group.bmp")))
				{
					// Get the group image
					Image myImage = new Bitmap(MOG_ControllerSystem.LocateTool("Images", "Group.bmp"));

					// Add the image and the type to the arrayLists
					mAssetTypeImages.Images.Add(myImage);
					mAssetTypes.Add("group");
				}
			}

			mAssetTypeImages.TransparentColor = Color.Magenta;

			// Initialize state icons
			FileInfo []stateImages = DosUtils.FileGetList(MOG_ControllerSystem.LocateTool("Images\\States", ""), "*.bmp");

			foreach (FileInfo stateImage in stateImages)
			{
				LoadRawIcon(mStateTypeImages, mStateTypes, stateImage.FullName, Path.GetFileNameWithoutExtension(stateImage.Name));
			}

			// Load our file icons as small icons
			mFileTypeManager = new IconListManager( mAssetTypeImages, IconReader.IconSize.Small );
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

		static public void ExplorerIconInitialize()
		{
			// Only allow population of the images array once
			if (mAssetTypes.Count > 0)
			{
				return;
			}

			lock (mAssetTypes)
			{
				// Add the active item icon first
				// Get the image
				if (DosUtils.FileExist(MOG_ControllerSystem.LocateTool("Images", "BaseNode.bmp")))
				{
					// Get the group image
					Image myImage = new Bitmap(MOG_ControllerSystem.LocateTool("Images", "BaseNode.bmp"));

					// Add the image and the type to the arrayLists
					mAssetTypeImages.Images.Add(myImage);
					mAssetTypes.Add("dot");
				}

				if (DosUtils.FileExist(MOG_ControllerSystem.LocateTool("Images", "Group.bmp")))
				{
					// Get the group image
					Image myImage = new Bitmap(MOG_ControllerSystem.LocateTool("Images", "Group.bmp"));

					// Add the image and the type to the arrayLists
					mAssetTypeImages.Images.Add(myImage);
					mAssetTypes.Add("group");
				}
			}

			mAssetTypeImages.TransparentColor = Color.Magenta;

			// Load our file icons as small icons
			mFileTypeManager = new IconListManager( mAssetTypeImages, IconReader.IconSize.Small );
		}

		/// <summary>
		/// Gets the MOGAssetStatus for a given status name
		/// </summary>
		static public int GetStatusIndex(string status)
		{
			// Loops through statusInfos ArrayList in order
			// to find StatusInfo.Text that matches parameter 
			// 'status'
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
		static public int GetClassIconIndex(String filename)
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

			switch(file.GetFilenameType())
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

			// Get the index of the key in the types array
			int x = 0;
			foreach (string classType in mAssetTypes)
			{
				if (string.Compare(classType, classification, true) == 0)
				{
					break;
				}
				else
				{
					x++;
				}
			}
			
			// If the asset was not found, return  for first icon in the list
			if (x >= mAssetTypes.Count)
			{
				return 0;
			}

			// Return the index of the icon
			return x;
		} // end ()

		/// <summary>
		/// Gets the icon for a file, returns GetClassIconIndex() if filename is not fully qualifed.
		/// </summary>
		/// <param name="filename">Fully qualified Windows filename of the file to be given an icon</param>
		/// <returns>Index of small icon for the file</returns>
		static public int GetFileIconIndex( string filename )
		{
			try
			{
				int returnInt = 0;
				if (mFileTypeManager == null)
				{
					mFileTypeManager = new IconListManager( mAssetTypeImages, IconReader.IconSize.Small );
				}

				if (File.Exists(filename))
				{
					// Try getting a File Icon
					returnInt = mFileTypeManager.AddFileIcon( filename );
				}

				// If we got the default index, return our Class Icon index
				if( returnInt == 0 )
					return GetClassIconIndex( filename );
				else
					return returnInt;
			}
			catch( Exception e )
			{
				// If we didn't have a valid, qualified filename, return Class Icon index
				e.ToString();  // This line for debug purposes (keeps VS.NET from warning use that e* is not used)
				return GetClassIconIndex( filename );
			}
		}
	}
}
