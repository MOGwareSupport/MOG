using System;
using System.IO;
using System.Windows.Forms;
using System.Collections;

using MOG_Client.Forms;

using MOG;
using MOG.FILENAME;
using MOG.DOSUTILS;
using MOG.REPORT;
using MOG.PROMPT;
using MOG.PROPERTIES;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.CONTROLLER.CONTROLLERINBOX;

using MOG_ControlsLibrary.Utils;
using MOG_ControlsLibrary.Forms.Wizards;
using MOG_ControlsLibrary.Forms;
using System.Collections.Generic;


namespace MOG_Client.Client_Mog_Utilities
{
	/// <summary>
	/// Summary description for guiAssetImportCheck.
	/// </summary>
	public class guiAssetImportCheck
	{
		public const string ImportFilter_String = "{}~'$:?*<>/\\|=[]";
		private bool mOkToAll;
		private bool mShowExtensions;
		public bool mRenameSource;

		// Used for OkToAll button
		private ArrayList mPreviousProperties;
		private string mPreviousClassifiaton;
		private string mPreviousPlatform;

		public guiAssetImportCheck()
		{
			mOkToAll = false;
			mShowExtensions = false;
			mRenameSource = false;		// This must be kept in synch with the default value of the forms check box!!
			mPreviousProperties = new ArrayList();
			mPreviousClassifiaton = "";
			mPreviousPlatform = "";
		}

		public bool CheckImportAssetNames(List<ImportFile> sourceFullNames, ref List<string> newAssetNames, ref List<ArrayList> newAssetProperties)
		{
			foreach (ImportFile SourceFile in sourceFullNames)
			{
				// Strip sourceFullNames[f] down to only the name
				string assetName = Path.GetFileName(SourceFile.mImportFilename);
				try
				{
					ArrayList MogPropertyArray = new ArrayList();
					string targetName = FixName(assetName, SourceFile, ref MogPropertyArray);
					if (targetName != null)
					{
						newAssetNames.Add(targetName);
						newAssetProperties.Add(MogPropertyArray);
					}
					else
					{
						//if FixName returns null that means the user cancelled
						return false;
					}
				}
				catch (Exception e)
				{
					MOG_Prompt.PromptMessage("Import FixName failed!", SourceFile + "\n\n" + e.Message, e.StackTrace);

					// Let our caller know that we didn't get a successful import on this asset
					SourceFile.mImportFilename = "";
					newAssetNames.Add("");
					newAssetProperties.Add(new ArrayList());
				}
			}

			return true;
		}

		public bool CheckImportAssetName(string sourceFullName, ref string newAssetName, ref ArrayList properties)
		{
			return CheckImportAssetName(sourceFullName, ref newAssetName, ref properties, new ArrayList());
		}

		public bool CheckImportAssetName(string sourceFullName, ref string newAssetName, ref ArrayList properties, ArrayList potentialMatches)
		{
			// Strip sourceFullName down to only the name
			string assetName = Path.GetFileName(sourceFullName);
			try
			{
				ImportFile sourceFile = new ImportFile(sourceFullName);
				sourceFile.mPotentialFileMatches = potentialMatches;
				newAssetName = FixName(assetName, sourceFile, ref properties);
			}
			catch(Exception e)
			{
				MOG_Prompt.PromptMessage("Import FixName failed!", sourceFullName + "\n\n" + e.Message, e.StackTrace);
			}

			return (newAssetName != null);
		}

		private string FixName(string name, ImportFile fullFilename, ref ArrayList properties)
		{
			string newName = null;
			bool succeed = false;
			bool cancel = false;

			// Have we previously specified whether or not to apply changes to all names?
			if (mOkToAll == false)
            {
                // Looks like we need to fix up the name...
                ImportAssetTreeForm mImportForm = new ImportAssetTreeForm(fullFilename);
				
                // Keep trying until we either fix the name or we cancel.
                while (!succeed && !cancel)
                {
                    // Show BlessDialog as a modal dialog and determine if DialogResult = OK.
					mImportForm.ShowDialog(MogMainForm.MainApp);
                    DialogResult rc = mImportForm.DialogResult;

					// Save our extension settings for multi import
					mShowExtensions = mImportForm.MOGShowExtensions;

                    // Check dialog results
					if (rc == DialogResult.Cancel)
					{
						// This means the user clicked cancel
						cancel = true;
					}
					else
					{
						if (rc == DialogResult.Yes)
						{
							// This is the OkToAll button
							mOkToAll = true;
						}

						// Construct the new name based on the dialog fields
						newName = mImportForm.GetFixedAssetName();

						// Always check each name to make sure it worked...
						succeed = CheckName(newName);
						if (succeed)
						{
							properties.AddRange(mImportForm.MOGPropertyArray);

							// Set previous settings for future imports
							mPreviousProperties = new ArrayList();					
							mPreviousProperties.AddRange(mImportForm.MOGPropertyArray);
							mPreviousClassifiaton = mImportForm.FinalAssetName.GetAssetClassification();
							mPreviousPlatform = mImportForm.FinalAssetName.GetAssetPlatform();
						}
					}
                }
            }
            else
            {
				// Since the OkToAll button was previously selected, just fix this name the same way w/o asking
				// Make sure to create mog names that match our previous settings of 'Extension' or 'No Extension'
				if (mShowExtensions)
				{
					name = DosUtils.PathGetFileName(fullFilename.mImportFilename);
				}
				else
				{
					name = DosUtils.PathGetFileNameWithoutExtension(fullFilename.mImportFilename);
				}

                MOG_Filename newAssetName = MOG_Filename.CreateAssetName(mPreviousClassifiaton, mPreviousPlatform, name);
                newName = newAssetName.GetAssetFullName();

                // Always check each name to make sure it worked...
                succeed = CheckName(newName);
                if (succeed == false)
                {
                    // Wow, this current name failed after a previous one succeeded...force the dialog to display agian.
                    mOkToAll = false;
                    newName = FixName(name, fullFilename, ref properties);
                }
                else
                {
                    // Load previous properties set on earlier imports
                    properties.AddRange(mPreviousProperties);
                }
            }

			return newName;
		}

		~guiAssetImportCheck()
		{
			// Destroy all temp files from import FixName
			string targetDirectory = String.Concat(MOG_ControllerProject.GetActiveUser().GetUserToolsPath(), "\\", MOG_ControllerSystem.GetComputerName());
			DosUtils.DirectoryDelete(targetDirectory);
		}

		/// <summary>
		/// Make sure this asset is MOG Compliant
		/// </summary>
		private bool CheckName(string filename)
		{
			MOG_Filename assetName;
			try
			{
				assetName = new MOG_Filename(filename);
			}
			catch
			{
				throw new Exception("Unable to convert filename, '" + filename + "' to a valid MOG Filename.");
			}

			// No asset is valid if it has the '[' character
				// Original string == "[]'`,$^:?*<>/\\%#!|="
				// glk:  Kier and I changed this so that ONLY Windows, MOG-Specific, and INI chars are filtered
			if (assetName.GetAssetLabel().Split(ImportFilter_String.ToCharArray()).Length > 1) 
			{
				throw new Exception("Non-valid character in import file: \n\n\t "+ImportFilter_String);
			}

			// Make sure the asset name is valid
			if(assetName.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Unknown && MOG_ControllerProject.ValidateAssetFilename(assetName) == false)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
	}
}
