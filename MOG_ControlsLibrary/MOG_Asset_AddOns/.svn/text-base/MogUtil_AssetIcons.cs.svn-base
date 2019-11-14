using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.Diagnostics;
using System.Threading;

using MOG;
using MOG.INI;
using MOG.USER;
using MOG.DOSUTILS;
using MOG.COMMAND;
using MOG.FILENAME;
using MOG.PROJECT;
using MOG.PROPERTIES;
using MOG.REPORT;
using MOG.PROMPT;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;

using Etier.IconHelper;

using Tst;

using MOG_ControlsLibrary;



namespace MOG_ControlsLibrary.Utils
{
	/// <summary>
	/// Summary description for guiAssets.
	/// </summary>
	public class MogUtil_AssetIcons
	{
		static private Mutex mMutex = new Mutex();
		public enum IconType { FILE, ASSET, CLASS };

		#region Variables, Properties
		// Asset icon variables
		static private ImageList mAssetTypeImages = new ImageList();					// Holds all the asset type icons
		static private TstDictionary mAssetTypes = new TstDictionary();				// This will be a look-up table for the imageList allowing us to look up types and get the correct icon

		// File icon variables
		// Win32 wrapper class that returns our indices and adds images for files at runtime
		static private IconListManager mFileTypeManager;

		/// <summary>
		/// ImagesList for all cached classification icons
		/// </summary>
		static public ImageList Images { get { return mAssetTypeImages; } set { mAssetTypeImages = value; } }
		/// <summary>
		/// ArrayList of all the classes that correspond with the cached images
		/// </summary>
		static private TstDictionary TypeNames { get { return mAssetTypes; } set { mAssetTypes = value; } }

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
			mAssetTypes = new TstDictionary();
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

			// Add the active item icon first
			// Get the image
			AddIcon("Images", "BaseNode.bmp", "dot");
			AddIcon("Images", MOG_ControllerProject.GetProjectName() + ".bmp", MOG_ControllerProject.GetProjectName());
			AddIcon("Images", "MOG Relationship.bmp", "Relationships");
			AddIcon("Images", "Group.bmp", "group");
			AddIcon("Images", "FileLocked.png", "locked");

			//mAssetTypeImages.TransparentColor = Color.Magenta;
			mAssetTypeImages.ColorDepth = ColorDepth.Depth32Bit;

			// Load our file icons as small icons
			mFileTypeManager = new IconListManager(mAssetTypeImages, IconReader.IconSize.Small);
		}

		static private void LoadRawIcon(ImageList myImages, TstDictionary myImageNames, string imageFileName, string imageName)
		{
			// Make sure that the icon file exists
			if (DosUtils.FileExist(imageFileName))
			{
				// Get the image
				Image myImage = new Bitmap(imageFileName);

				// Add the image and the type to the arrayLists
				myImages.Images.Add(myImage);
				myImageNames.Add(imageName, myImages.Images.Count - 1);
			}
		}

		static private int LoadIcon(string imageFileName, string assetClassName)
		{
			try
			{
				if (imageFileName != null && imageFileName.Length > 0)
				{
					// Load the icon specified by the Icon= key
					string iconName = imageFileName;

					// Make sure this is a full path, if not append the tools directory to it?
					iconName = MOG_ControllerSystem.LocateTool(imageFileName);

					// Make sure that the icon file exists
					if (iconName.Length > 0 && DosUtils.FileExist(iconName))
					{
						// Get the image
						Image image = new Bitmap(iconName);

						return InternalAddIconToLists(assetClassName, iconName, image);
					}
				}
			}
			catch (Exception e)
			{
				MOG_Report.ReportSilent("Load Icon", e.Message, e.StackTrace, MOG_ALERT_LEVEL.CRITICAL);
			}

			return 0;
		}

		private static int InternalAddIconToLists(string iconName, Image image)
		{
			return InternalAddIconToLists(iconName, iconName, image);
		}

		private static int InternalAddIconToLists(string assetTypeName, string iconFileName, Image image)
		{
			int index = 0;

			lock (mAssetTypes)
			{
				try
				{
					// Handle any situation where we might not know the filename of the icon
					if (iconFileName == null)
					{
						iconFileName = assetTypeName;
					}

					// Do we already have this image loaded?
					if (mAssetTypeImages.Images.ContainsKey(iconFileName))
					{
						index = mAssetTypeImages.Images.IndexOfKey(iconFileName);

						//Debug.WriteLine("Returning cached icon - " + iconFileName + " at index - " + index.ToString());

						if (mAssetTypes.Find(iconFileName) == null)
						{
							mAssetTypes.Add(assetTypeName, index);
						}

						//Debug.WriteLine("Returned cached icon - " + iconFileName + " at index - " + index.ToString());
					}
					else
					{
						// Add the image and the type to the arrayLists
						index = mAssetTypeImages.Images.Count;
						//Debug.WriteLine("Addeding icon - " + assetTypeName + " as index - " + index.ToString() + " with key = " + iconFileName);

						mAssetTypeImages.Images.Add(iconFileName, image);
						mAssetTypes.Add(assetTypeName, index);

						//Debug.WriteLine("Added icon - " + assetTypeName + " as index - " + index.ToString() + " with key = " + iconFileName);
					}
				}
				catch (Exception e)
				{
					e.ToString();
				}
			}

			return index;
		}

		static public void ExplorerIconInitialize()
		{
			// Only allow population of the images array once
			if (mAssetTypes.Count > 0)
			{
				return;
			}

			// Add the active item icon first
			// Get the image
			AddIcon("Images", "BaseNode.bmp", "dot");
			AddIcon("Images", "Group.bmp", "group");

			//mAssetTypeImages.TransparentColor = Color.Magenta;
			mAssetTypeImages.ColorDepth = ColorDepth.Depth32Bit;

			// Load our file icons as small icons
			mFileTypeManager = new IconListManager(mAssetTypeImages, IconReader.IconSize.Small);
		}

		static public bool AddIcon(string directory, string filename, string key)
		{
			Tst.TstDictionaryEntry node = mAssetTypes.Find(key);
			if (node == null)
			{
				// Add the active item icon first
				// Get the image
				if (DosUtils.FileExist(MOG_ControllerSystem.LocateTool(directory, filename)))
				{
					string iconName = MOG_ControllerSystem.LocateTool(directory, filename);
					// Get the group image
					Image myImage = new Bitmap(iconName);

					InternalAddIconToLists(key, iconName, myImage);

					return true;
				}
			}
			return false;
		}

		static public void AddCustomIcons(ImageList list, int index, string key)
		{
			try
			{
				if (list.Images.Count >= index)
				{
					Image icon = list.Images[index] as Image;
					AddIcon(icon, key);
				}
			}
			catch (Exception e)
			{
				MOG_Prompt.PromptResponse("Add Custom Image Failed!", e.Message, e.StackTrace, MOGPromptButtons.OK);
			}
		}

		static public void AddCustomIcons(Image icon, string key)
		{
			AddIcon(icon, key);
		}

		static public bool AddIcon(Image icon, string key)
		{
			Tst.TstDictionaryEntry node = mAssetTypes.Find(key);
			if (node == null)
			{
				InternalAddIconToLists(key, icon);
				return true;
			}
			return false;
		}

		static public int GetAssetIconIndex(string filename)
		{
			return GetAssetIconIndex(filename, null);
		}

		static public Image GetAssetIconImage(string filename)
		{
			int index = GetAssetIconIndex(filename, null);
			if (index >= 0)
			{
				return mAssetTypeImages.Images[index];
			}

			return null;
		}

		static public int GetAssetIconIndex(string filename, MOG_Properties properties)
		{
			return GetAssetIconIndex(filename, properties, true);
		}

		static public int GetAssetIconIndex(string filename, MOG_Properties properties, bool bCheckLockedIcons)
		{
			// Construct a filename
			MOG_Filename file = null;
			try
			{
				file = new MOG_Filename(filename);
			}
			catch (Exception e)
			{
				e.ToString();
				return 0;
			}

			string assetName = "default";

			switch (file.GetFilenameType())
			{
			case MOG_FILENAME_TYPE.MOG_FILENAME_Asset:
				assetName = file.GetAssetClassification();
				break;
			case MOG_FILENAME_TYPE.MOG_FILENAME_Group:
				assetName = "group";
				break;
			default:
				assetName = filename;
				break;
			}

			// Check for locks on this asset
			if (bCheckLockedIcons == true &&
				MOG_ControllerProject.IsLocked(file.GetAssetFullName()))
			{
				// Currently we dont distinguish between read and write locks
				return GetLockedIcon(assetName, IconType.ASSET, properties);
			}
			else
			{
				return FindOrAddIcon(ref properties, IconType.ASSET, assetName, assetName + "_ASSET");
			}
		}

		/// <summary>
		/// Thread safe find and add function for icon management
		/// </summary>
		private static int FindOrAddIcon(ref MOG_Properties properties, IconType iconType, string assetPropertiesName, string assetName)
		{
			// Make sure we are mutex safe
			mMutex.WaitOne();

			try
			{
				// Does this icon key exist already
				TstDictionaryEntry node = mAssetTypes.Find(assetName);
				if (node != null && node.IsKey)
				{
					// Return the icon index then
					return (int)node.Value;
				}
				else
				{
					// Do we have a valid properties object?
					if (properties == null)
					{
						// If we didn't get anything, we need to load this icon into our array
						properties = new MOG_Properties(assetPropertiesName);
					}
					else if (properties != null && properties.AssetIcon.Length == 0 || properties.ClassIcon.Length == 0)
					{
						properties = new MOG_Properties(assetPropertiesName);
					}

					// Lets now populate our icon name to load
					string iconName = "";

					// What kind of icon is this?
					switch (iconType)
					{
					case IconType.ASSET:
						iconName = properties.AssetIcon;
						break;
					case IconType.CLASS:
						iconName = properties.ClassIcon;
						break;
					default:
						return 0;
					}

					// Load this icon into our list of icons and return its newly created index
					return LoadIcon(iconName, assetName);
				}
			}
			catch
			{
				return 0;
			}
			finally
			{
				// Realease our mutex
				mMutex.ReleaseMutex();
			}
		}

		static public int GetClassIconIndex(string filename)
		{
			return GetClassIconIndex(filename, null);
		}

		/// <summary> SetAssetIcon
		/// Searches through mAssetTypes to find the matching key with 
		/// that of the filename.  Then returns the index
		/// </summary>
		/// <param name="filename"></param>
		/// <returns>index of icon in the mAssetTypeImages list</returns>
		static public int GetClassIconIndex(string filename, MOG_Properties properties)
		{
			// Construct a filename
			MOG_Filename file = null;
			try
			{
				file = new MOG_Filename(filename);
			}
			catch (Exception e)
			{
				e.ToString();
				return 0;
			}

			string classification;
			string lockName;

			switch (file.GetFilenameType())
			{
			case MOG_FILENAME_TYPE.MOG_FILENAME_Asset:
				classification = file.GetAssetClassification();
				lockName = file.GetAssetFullName();
				break;
			case MOG_FILENAME_TYPE.MOG_FILENAME_Group:
				classification = "group";
				lockName = file.GetAssetFullName();
				break;
			default:
				classification = filename;
				lockName = classification;
				break;
			}

			// Check for locks on this asset
			if (MOG_ControllerProject.IsLocked(lockName))
			{
				// Currently we dont distinguish between read and write locks
				return GetLockedIcon(lockName, IconType.CLASS, properties);
			}
			else
			{
				return FindOrAddIcon(ref properties, IconType.CLASS, classification, classification);
			}
		} // end ()

		static public int GetLockedIcon(string assetName, IconType iconType, MOG_Properties properties)
		{
			try
			{
				// Check to see if we can find this loced icon in our cache
				string lockedIconKeyName = assetName + "_" + iconType.ToString() + "_locked";
				TstDictionaryEntry assetLocked = mAssetTypes.Find(lockedIconKeyName);
				if (assetLocked != null)
				{
					// Yup, return the index
					return (int)assetLocked.Value;
				}
				else
				{
					// *******************************************
					// Nope, We are going to have to create it
					// *******************************************
					// We need 3 things:
					// 1) Lock icon
					// 2) Asset or class icon
					// 3) New overlayed icon

					// Setup some new containers
					Bitmap myImage = null;
					Bitmap lockSource = null;
					Bitmap source = null;

					// 1) Can we find the locked image
					TstDictionaryEntry nodeLocked = mAssetTypes.Find("locked");
					if (nodeLocked != null && nodeLocked.IsKey)
					{
						// Great, get a copy of that
						lockSource = (Bitmap)mAssetTypeImages.Images[(int)nodeLocked.Value];

						// Do we have a valid properties object?
						if (properties == null)
						{
							// If we didn't get anything, we need to load this icon into our array
							properties = new MOG_Properties(assetName);
						}

						// Lets now populate our icon name to load
						string assetIconName = "";
						string nodeIconName = "";
						
						// What kind of icon is this?
						switch (iconType)
						{
							case IconType.ASSET:
								assetIconName = properties.AssetIcon;
								nodeIconName = assetName + "_ASSET";
								break;
							case IconType.CLASS:
								assetIconName = properties.ClassIcon;
								nodeIconName = assetName + "_CLASS";
								break;
							default:
								return 0;
						}

						// 2) Can we find the class icon for this asset
						TstDictionaryEntry nodeSource = mAssetTypes.Find(nodeIconName);
						if (nodeSource != null && nodeSource.IsKey)
						{
							// Great get a copy of that
							source = (Bitmap)mAssetTypeImages.Images[(int)nodeSource.Value];
						}
						else
						{
							// If we didn't get anything, we need to load this icon into our array
							int newIconIndex = LoadIcon(assetIconName, nodeIconName);
							source = (Bitmap)mAssetTypeImages.Images[newIconIndex];
						}

						// 3) Ok, if we got a lockSource and a class source icon, lets attempt to overlay them
						if (source != null && lockSource != null)
						{
							myImage = BitmapManipulator.OverlayBitmap(source, lockSource, 100, BitmapManipulator.ImageCornerEnum.BottomRight);
						}

						// Did the overlay work?
						if (myImage != null)
						{
							//Debug.WriteLine("Requesting icon - " + lockedIconKeyName);
							return InternalAddIconToLists(lockedIconKeyName, myImage);														
						}
					}
					else
					{
						// Try to just turn the source icon red or something
						string message = "We could not locate the (FileLocked.png)lock icon! Make sure that it is located in one of your images directories within the MOG repository!";
						MOG_Report.ReportSilent("Load Icon", message, Environment.StackTrace, MOG_ALERT_LEVEL.ERROR);
					}
				}
			}
			catch (Exception e)
			{
				MOG_Report.ReportSilent("Get Locked Icon", e.Message, e.StackTrace, MOG_ALERT_LEVEL.CRITICAL);
			}

			return 0;
		}

		static public int GetLockedAssetIcon(MOG_Filename file)
		{
			try
			{
				// Check to see if we can find this loced icon in our cache
				TstDictionaryEntry assetLocked = mAssetTypes.Find(file.GetAssetClassification() + "ASSETICON_locked");
				if (assetLocked != null)
				{
					// Yup, return the index
					return (int)assetLocked.Value;
				}
				else
				{
					// Nope, We are going to have to create it

					// Setup some new containers
					Bitmap myImage = null;
					Bitmap lockSource = null;
					Bitmap source = null;

					// Can we find the locked image
					TstDictionaryEntry nodeLocked = mAssetTypes.Find("locked");
					if (nodeLocked != null && nodeLocked.IsKey)
					{
						// Great, get a copy of that
						lockSource = (Bitmap)mAssetTypeImages.Images[(int)nodeLocked.Value];

						// Can we find the class icon for this asset
						TstDictionaryEntry nodeSource = mAssetTypes.Find(file.GetAssetClassification() + "_ASSET");
						if (nodeSource != null && nodeSource.IsKey)
						{
							// Great get a copy of that
							source = (Bitmap)mAssetTypeImages.Images[(int)nodeSource.Value];
						}
						else
						{
							// If we didn't get anything, we need to load this icon into our array
							MOG_Properties properties = new MOG_Properties(file.GetAssetClassification());
							source = (Bitmap)mAssetTypeImages.Images[LoadIcon(properties.AssetIcon, file.GetAssetClassification() + "_ASSET")];
						}

						// Ok, if we got a lockSource and a class source icon, lets attempt to overlay them
						if (source != null && lockSource != null)
						{
							myImage = BitmapManipulator.OverlayBitmap(source, lockSource, 100, BitmapManipulator.ImageCornerEnum.BottomRight);
						}

						// Did the overlay work?
						if (myImage != null)
						{
							// Add the image and the type to the arrayLists
							string iconName = file.GetAssetClassification() + "ASSETICON_locked";
							return InternalAddIconToLists(iconName, myImage);							
						}
					}
					else
					{
						// Try to just turn the source icon red or something
						string message = "We could not locate the (FileLocked.png)lock icon! Make sure that it is located in one of your images directories within the MOG repository!";
						MOG_Report.ReportSilent("Load Icon", message, "No StackTrace available", MOG_ALERT_LEVEL.ERROR);
					}
				}
			}
			catch (Exception e)
			{
				MOG_Report.ReportSilent("Get Locked Icon", e.Message, e.StackTrace, MOG_ALERT_LEVEL.CRITICAL);
			}

			return 0;
		}

		static public int GetBinaryLockedOrUnlockedIcon(string lockFilename, string filename)
		{
			// Check for locks on this asset
			if (MOG_ControllerProject.IsLocked(lockFilename))
			{
				// Currently we dont distinguish between read and write locks
				return GetLockedBinaryIcon(filename);
			}
			else
			{
				return GetFileIconIndex(filename);
			}
		}

		static public int GetLockedBinaryIcon(string filename)
		{
			try
			{
				// Check to see if we can find this loced icon in our cache
				TstDictionaryEntry assetLocked = mAssetTypes.Find(Path.GetFileName(filename) + "_locked");
				if (assetLocked != null && assetLocked.IsKey)
				{
					// Yup, return the index
					return (int)assetLocked.Value;
				}
				else
				{
					// Nope, We are going to have to create it

					// Setup some new containers
					Bitmap newLockedIcon = null;
					Bitmap lockIconSource = null;
					Bitmap assetIconSource = null;


					// Can we find the locked image
					TstDictionaryEntry nodeLocked = mAssetTypes.Find("locked");
					if (nodeLocked != null && nodeLocked.IsKey)
					{
						// Great, get a copy of that
						lockIconSource = (Bitmap)mAssetTypeImages.Images[(int)nodeLocked.Value];

						// Now try and get the icon of this asset
						// Have we seen this type of asset by its extension?
						if (mFileTypeManager.ExtensionListHasKey(filename))
						{
							// Great get a copy of that
							assetIconSource = (Bitmap)mFileTypeManager.GetImage(filename);
						}
						else
						{
							// No, ok try and add it from the file its self
							if (File.Exists(filename))
							{
								mFileTypeManager.AddFileIcon(filename);
								assetIconSource = (Bitmap)mFileTypeManager.GetImage(filename);
							}
							else
							{
								// Use the default icon
								assetIconSource = (Bitmap)mAssetTypeImages.Images[0];
							}
						}

						// Ok, if we got a lockSource and a class source icon, lets attempt to overlay them
						if (assetIconSource != null && lockIconSource != null)
						{
							newLockedIcon = BitmapManipulator.OverlayBitmap(assetIconSource, lockIconSource, 100, BitmapManipulator.ImageCornerEnum.BottomRight);
						}

						// Did the overlay work?
						if (newLockedIcon != null)
						{
							// Add the image and the type to the arrayLists
							string iconName = Path.GetFileName(filename) + "_locked";
							return InternalAddIconToLists(iconName, newLockedIcon);							
						}
					}
				}
			}
			catch (Exception e)
			{
				MOG_Report.ReportSilent("Get Locked Binary Icon", e.Message, e.StackTrace, MOG_ALERT_LEVEL.CRITICAL);
			}

			return 0;
		}

		/// <summary>
		/// Gets the icon for a file, returns GetClassIconIndex() if filename is not fully qualifed.
		/// </summary>
		/// <param name="filename">Fully qualified Windows filename of the file to be given an icon</param>
		/// <returns>Index of small icon for the file</returns>
		static public int GetFileIconIndex(string filename)
		{
			if (mFileTypeManager == null)
			{
				mFileTypeManager = new IconListManager(mAssetTypeImages, IconReader.IconSize.Small);
			}

			int returnInt = 0;

			// Try getting a File Icon
			if (File.Exists(filename))
			{
				returnInt = mFileTypeManager.AddFileIcon(filename);
			}

			// If we got the default index, return our Class Icon index
			if (returnInt == 0)
				return GetAssetIconIndex(filename, null);
			else
				return returnInt;
		}

		static public int GetFileIconIndex(string filename, string overlayIcon)
		{
			if (mFileTypeManager == null)
			{
				mFileTypeManager = new IconListManager(mAssetTypeImages, IconReader.IconSize.Small);
			}

			int returnInt = 0;

			// Make sure this is a full path, if not append the tools directory to it?
			string overlayIconFilename = MOG_ControllerSystem.LocateTool(overlayIcon);

			// Try getting a File Icon
			if (File.Exists(filename) && File.Exists(overlayIconFilename))
			{
				returnInt = mFileTypeManager.AddFileOverlayIcon(filename, "MISSING", overlayIconFilename);
			}

			// If we got the default index, return our Class Icon index
			if (returnInt == 0)
				return GetAssetIconIndex(filename, null);
			else
				return returnInt;
		}

		static public int GetFileIconIndex(string filename, MOG_Properties properties)
		{
			if (mFileTypeManager == null)
			{
				mFileTypeManager = new IconListManager(mAssetTypeImages, IconReader.IconSize.Small);
			}

			int returnInt = 0;

			if (properties == null)
			{
				// Try getting a File Icon
				if (File.Exists(filename))
				{
					returnInt = mFileTypeManager.AddFileIcon(filename);
				}
			}

			// If we got the default index, return our Class Icon index
			if (returnInt == 0)
				return GetAssetIconIndex(filename, properties);
			else
				return returnInt;
		}
	}
}
