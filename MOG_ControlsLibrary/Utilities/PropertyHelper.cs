using System;
using System.Collections;

using MOG;
using MOG.FILENAME;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERPACKAGE;
using MOG.REPORT;
using MOG.PROMPT;
using MOG.DOSUTILS;

using System.Windows.Forms;
using MOG.CONTROLLER.CONTROLLERLIBRARY;


namespace MOG_ControlsLibrary.Utils
{
	public class PropertyHelper
	{
		public static ArrayList RepairPropertyList(ArrayList properties)
		{
			ArrayList fixedProperties = new ArrayList();

			// Make sure we have properties?
			if (properties != null)
			{
				// Loop through all the specified properties
				foreach (MOG_Property propertyObject in properties)
				{
					// Attempt to fix the property
					fixedProperties.Add(RepairProperty(propertyObject));
				}
			}

			return fixedProperties;
		}

		public static MOG_Property RepairProperty(MOG_Property propertyObject)
		{
			MOG_Property fixedPropertyObject = null;

			string key = propertyObject.mKey; 
			string section = propertyObject.mSection; 
			string propertySection = propertyObject.mPropertySection; 
			string propertyKey = propertyObject.mPropertyKey;
			string propertyValue = propertyObject.mPropertyValue;

			MOG_Property tempProperty = MOG_PropertyFactory.MOG_Relationships.New_RelationshipAssignment("", "", "", "");
			MOG_Property tempPackageProperty = MOG_PropertyFactory.MOG_Relationships.New_PackageAssignment("", "", "");
			MOG_Property tempSourceFileProperty = MOG_PropertyFactory.MOG_Relationships.New_AssetSourceFile("");
								
			// Check if this property is a package relationship?
			if (string.Compare(section, tempPackageProperty.mSection, true) == 0)
			{
				string assetName = MOG_ControllerPackage.GetPackageName(propertyKey);
				string groups = MOG_ControllerPackage.GetPackageGroups(propertyKey);
				string objects = MOG_ControllerPackage.GetPackageObjects(propertyKey);

				// Remap various properties making sure we correct any problem areas
				MOG_Filename assetFilename = null;

				// Check if this property is a SourceFile relationship?
				if (string.Compare(propertySection, tempSourceFileProperty.mPropertySection, true) == 0)
				{
					// Check if the specified file is within the library?
					if (MOG_ControllerLibrary.IsPathWithinLibrary(propertyKey))
					{
						// Map this library file to a real asset name
						assetFilename = MOG_ControllerProject.MapFilenameToLibraryAssetName(assetName, MOG_ControllerProject.GetPlatformName());
						if (assetFilename != null)
						{
							fixedPropertyObject = MOG_PropertyFactory.MOG_Relationships.New_AssetSourceFile(assetFilename.GetFullFilename());
						}
					}
				}
				else
				{
					// Try to find the assetname for the specified asset
					ArrayList assets = MOG_ControllerProject.MapFilenameToAssetName(assetName, MOG_ControllerProject.GetPlatformName(), MOG_ControllerProject.GetWorkspaceDirectory());
					if (assets == null || assets.Count == 0)
					{
						// The package could not be found
						if (string.Compare(propertySection, tempPackageProperty.mPropertySection, true) == 0)
						{
							// Check if we actually had something specified?
							if (assetName.Length > 0)
							{
								// Set the deafult packageFilename info
								string assetClassification = "";
								string assetPlatformName = "All";
								string assetLabel = DosUtils.PathGetFileNameWithoutExtension(assetName);
								string syncTargetPath = DosUtils.PathGetDirectoryPath(assetName);

								// Check if the assetName was already a valid MOG_Filename?
								MOG_Filename packageFilename = new MOG_Filename(assetName);
								if (packageFilename.GetAssetClassification().Length > 0)
								{
									assetClassification = MOG_Filename.AppendAdamObjectNameOnClassification(packageFilename.GetAssetClassification());
								}
								if (packageFilename.GetAssetPlatform().Length > 0)
								{
									assetPlatformName = packageFilename.GetAssetPlatform();
								}
								if (packageFilename.GetAssetLabel().Length > 0)
								{
									assetLabel = packageFilename.GetAssetLabel();
								}

								// Prompt user to complete the unknown information about this packageFile
								string message = "MOG has detected a new package assignment to a previously non-existing package.  Please complete the following red fields so that a proper package can be created in MOG.";
								PackageCreator creator = new PackageCreator();
								creator.Classification = assetClassification;
								creator.PackageName = assetLabel;
								creator.SyncTarget = syncTargetPath;
								creator.Platform = assetPlatformName;
								if (creator.ShowDialog() == DialogResult.OK)
								{
									// Use this newly created packageFilename as our assetFilename to be fixed
									assetFilename = creator.AssetName;
								}
								else
								{
									// The PackageName is invalid
									message = "New Package Not Created.\n" +
											  "The user chose not to create a new package.";
									MOG_Report.ReportMessage("Package Assignment", message, Environment.StackTrace, MOG_ALERT_LEVEL.ERROR);
								}
							}
							else
							{
								// The PackageName is invalid
								string message = "Invalid PackageName specified.\n" +
												 "The packaged asset was not assigned to a package.";
								MOG_Report.ReportMessage("Package Assignment", message, Environment.StackTrace, MOG_ALERT_LEVEL.ERROR);
							}
						}
					}
					else
					{
						// Always use the first one
						assetFilename = assets[0] as MOG_Filename;
						MOG_ControllerProject.MapFilenameToAssetName_WarnAboutAmbiguousMatches(assetName, assets);
					}

					// Now do we finally have a package asset name?
					if (assetFilename != null)
					{
						// Replace the propertyObject with the fixed up one
						fixedPropertyObject = MOG_PropertyFactory.MOG_Relationships.New_RelationshipAssignment(propertySection, assetFilename.GetAssetFullName(), groups, objects);
					}
				}
			}

			// Check if we fixed the property?
			if (fixedPropertyObject != null)
			{
				return fixedPropertyObject;
			}
			return propertyObject;
		}

	}
}
