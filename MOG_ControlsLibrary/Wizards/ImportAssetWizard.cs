using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;

using MOG;
using MOG.FILENAME;
using MOG.PROPERTIES;
using MOG.DOSUTILS;
using MOG.PLATFORM;
using MOG.PROJECT;
using MOG.REPORT;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERSYNCDATA;
using MOG.CONTROLLER.CONTROLLERPACKAGE;
using MOG.CONTROLLER.CONTROLLERREPOSITORY;
using MOG.PROMPT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.CONTROLLER.CONTROLLERINBOX;

namespace MOG_ControlsLibrary.Forms.Wizards
{
	public partial class ImportAssetWizard : Form
	{
		private string mSeed_AssetLabel = "";
		private string mSeed_AssetClassification = "";
		private string mSeed_AssetPlatform = "";
		private string mSeed_AssetSyncTarget = "";
		private ArrayList mSeed_AssetProperties = new ArrayList();

		private string mTargetSouceFileName;
		private ArrayList mTargetImportProperties;
		private MOG_Filename mTargetImportMOGName;
		private string mTargetImportSyncPath = "";

		private ArrayList mInternalPotentialMatches = new ArrayList();
		private bool mHasMultiples = false;
		private string mInternalLastSelectedClassification;

		private MOG_Properties mProperties = null;

		public enum StartScreenEnum
		{
			Normal,
			PackageAssignment,
			SyncTargetAssignment,
		}
		private StartScreenEnum mStartScreen = StartScreenEnum.Normal; // Set the default to normal

		#region Getters/Setters
		public StartScreenEnum Seed_StartScreen
		{
			get { return mStartScreen; }
			set { mStartScreen = value; }
		}
		public string Seed_AssetLabel
		{
			set { mSeed_AssetLabel = value; }
		}
		public string Seed_AssetClassification
		{
			set { mSeed_AssetClassification = value; }
		}
		public string Seed_AssetPlatform
		{
			set { mSeed_AssetPlatform = value; }
		}
		public string Seed_AssetSyncTarget
		{
			set { mSeed_AssetSyncTarget = value; }
		}
		public ArrayList Seed_AssetProperties
		{
			set {	mSeed_AssetProperties = value;
					// Also seed the mTargetImportProperties so ALL seeded props will get passed through the wizard
					mTargetImportProperties.Clear();
					mTargetImportProperties.AddRange(value);
				}
		}

		public string ImportSourceFilename
		{
			set { mTargetSouceFileName = value; }
			get { return mTargetSouceFileName; }
		}
		public MOG_Filename ImportFinalMOGFilename
		{
			set { mTargetImportMOGName = value; }
			get { return mTargetImportMOGName; }
		}
		/// <summary>
		/// Get the MOG_Properties for this asset
		/// </summary>
		public ArrayList ImportPropertyArray
		{
			get { return mTargetImportProperties; }
		}

		public ArrayList ImportPotentialMatches
		{
			set { mInternalPotentialMatches = value; }
			get { return mInternalPotentialMatches; }
		}
		public bool ImportHasMultiples
		{
			set { mHasMultiples = value; }
			get { return mHasMultiples; }
		}
		public bool ImportMultipleApplyToAll
		{
			get { return ImportEndMultiImportApplyToAllCheckBox.Checked; }
		}
		public bool ImportShowExtension
		{
			get { return this.cbAssetExtensions.Checked; }
		}
		#endregion

		public ImportAssetWizard()
		{
			InitializeComponent();

			mTargetImportProperties = new ArrayList();
			ImportMogRepositoryTreeView.ContextMenuStrip = ImportSelectClassContextMenu;
		}

		#region Support functions
		private MOG_Properties GetAssetProperties(MOG_Filename assetFilename)
		{
			// Create a blank properties that we can modify
			MOG_Properties Properties = new MOG_Properties(new ArrayList());

			// Check what we have to use
			if (mTargetImportMOGName != null)
			{
				if (mTargetImportMOGName.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
				{
					// Get the current properties for the asset
					MOG_Properties AssetProperties = new MOG_Properties(assetFilename);
					if (AssetProperties != null)
					{
						// Now take the properties in for this Asset and push them into our properties array
						Properties.SetProperties(AssetProperties.GetPropertyList());
					}
				}
				else
				{
					// Check if we at least have a classification?
					if (mInternalLastSelectedClassification.Length > 0)
					{
						// Get the current properties for the classification
						MOG_Properties AssetProperties = new MOG_Properties(mInternalLastSelectedClassification);
						if (AssetProperties != null)
						{
							// Now take the properties in for this Asset and push them into our properties array
							Properties.SetProperties(AssetProperties.GetPropertyList());
						}
					}
				}
			}

			// Check if we have any seeded properties?
			if (mSeed_AssetProperties.Count > 0)
			{
				// Make sure we add any seeded properties as well
				Properties.SetProperties(mSeed_AssetProperties);
			}

			return Properties;
		}

		private string CreateDefaultClassification(string sourceFile)
		{
			string classification = "";

			MOG_ControllerSyncData workspace = MOG_ControllerProject.GetCurrentSyncDataController();

			// Do we have a workspace?
			if (workspace != null && sourceFile.IndexOf(workspace.GetSyncDirectory(), StringComparison.CurrentCultureIgnoreCase) != -1)
			{
				// Remove the Workspace dir
				classification = sourceFile.Substring(workspace.GetSyncDirectory().Length);
			}
			else
			{
				// Remove root
				string root = Path.GetPathRoot(sourceFile);

				classification = Path.GetDirectoryName(sourceFile).Substring(root.Length);
			}

			// Put all these assets in a generic unclassified catagory
			//            classification = "UnClassified" + classification;                

			// Replace slashes with ~
			classification = classification.TrimStart("\\/".ToCharArray());
			classification = classification.Replace("\\", "~");
			classification = classification.Replace("/", "~");

			return classification;
		}

		/// <summary>
		/// Populates our platforms combobox and asset-related textBoxes
		/// </summary>
		private void PopulateAssetNameAndPlatforms()
		{
			// Check if we have been seeded?
			if (mSeed_AssetLabel.Length > 0)
			{
				ImportAssetNameTextBox.Text = mSeed_AssetLabel;
			}
			else if (cbAssetExtensions.Checked)
			{
				ImportAssetNameTextBox.Text = DosUtils.PathGetFileName(mTargetSouceFileName);
			}
			else
			{
				ImportAssetNameTextBox.Text = DosUtils.PathGetFileNameWithoutExtension(mTargetSouceFileName);
			}

			// Populate platforms combobox			
			this.ImportValidPlatformsComboBox.Items.Clear();
			this.ImportValidPlatformsComboBox.Items.Add("All");

			// Add all the platforms
			System.Collections.IEnumerator myEnumerator = MOG_ControllerProject.GetProject().GetPlatforms().GetEnumerator();
			while (myEnumerator.MoveNext())
			{
				ImportValidPlatformsComboBox.Items.Add(((MOG_Platform)myEnumerator.Current).mPlatformName);
			}
			ImportValidPlatformsComboBox.SelectedIndex = 0;
		}

		private void EvaluateMogAssetName()
		{
			string platform = this.ImportValidPlatformsComboBox.Text;
			string assetName = this.ImportAssetNameTextBox.Text;

			// Set our new name
			mTargetImportMOGName = MOG_Filename.CreateAssetName(mInternalLastSelectedClassification, platform, assetName);
			this.ImportTargetClassTextBox.Text = mTargetImportMOGName.GetAssetFullName();
		}

		/// <summary>
		/// Part of PopulateAssetNameAndPlatformss()
		/// </summary>
		private void InitializePotentialMatches()
		{
			// Clear the list
			ImportExistingListView.Items.Clear();

			// If we have potential matches list them
			if (mInternalPotentialMatches != null && 
				mInternalPotentialMatches.Count > 0)
			{
				foreach (MOG_Filename file in mInternalPotentialMatches)
				{
					// Make sure this isn't a fake item
					if (file.GetAssetFullName().Length > 0)
					{
						ListViewItem item = new ListViewItem();

						item.Text = file.GetAssetFullName();
						item.SubItems.Add(MOG_ControllerProject.GetAssetCurrentBlessedVersionPath(file).GetVersionTimeStampString(""));
						item.SubItems.Add("");
						item.SubItems.Add(file.GetAssetFullName());

						ImportExistingListView.Items.Add(item);
					}
				}

				// Check if this was the only one?
				if (ImportExistingListView.Items.Count == 1)
				{
					// Automatically select the first and only item
					ImportExistingListView.Items[0].Selected = true;
				}
			}
		}

		/// <summary>
		/// Create a sync property where appropriate based on the passed in importTargetDirectory
		/// If inherited properties are passed in they will look like: "ClassName\ClassSubName"
		/// If the user bowsed and selected an import path it will look like: "c:\MyProject\FolderName\FolderSubName"
		/// </summary>
		/// <param name="importTargetDirectory"></param>
		/// <returns></returns>
		private string CreateSyncDirectoryProperty(string importTargetDirectory)
		{
			string targetPath = importTargetDirectory;

			// Clear any previous properties
			mTargetImportProperties.Clear();
			// Add our original seeded properties so ALL seeded props will get passed through the wizard
			mTargetImportProperties.AddRange(mSeed_AssetProperties);

			// Check if this import file is comming from within the current workspace?
			if (DosUtils.PathIsWithinPath(MOG_ControllerProject.GetWorkspaceDirectory(), importTargetDirectory))
			{
				// Make this path relative
				targetPath = DosUtils.PathMakeRelativePath(MOG_ControllerProject.GetWorkspaceDirectory(), importTargetDirectory);
			}

			// Add our new package to our properies
			mTargetImportProperties.Add(MOG.MOG_PropertyFactory.MOG_Sync_OptionsProperties.New_SyncTargetPath(targetPath));

			return targetPath;
		}

		private string CreateTargetPackageProperty(string packageFullMogName, string packageFullFilename)
		{
			// If we are dealing with an asset...
			MOG_Filename package = new MOG_Filename(packageFullFilename);
			if (package.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
			{
				// Make sure this is a real package
				MOG_Properties packageProperties = new MOG_Properties(package);
				if (packageProperties.IsPackage)
				{
					// Clear any previous packages
					mTargetImportProperties.Clear();
					// Add our original seeded properties so ALL seeded props will get passed through the wizard
					mTargetImportProperties.AddRange(mSeed_AssetProperties);

					// Setup our new package
					MOG_Property temp = MOG.MOG_PropertyFactory.MOG_Relationships.New_PackageAssignment(MOG_ControllerPackage.GetPackageName(packageFullMogName),
																			MOG_ControllerPackage.GetPackageGroups(packageFullMogName),
																			MOG_ControllerPackage.GetPackageObjects(packageFullMogName));
					// Add it to our properies
					mTargetImportProperties.Add(temp);

					return temp.mPropertyKey;
				}
			}

			return "";
		}

		private void AddClassification()
		{
			MOG_Privileges privs = MOG_ControllerProject.GetPrivileges();
			if (privs.GetUserPrivilege(MOG_ControllerProject.GetUserName(), MOG_PRIVILEGE.AddClassification))
			{
				try
				{
					try
					{
						TreeNode rootClass = ImportMogRepositoryTreeView.SelectedNode;

						if (rootClass != null)
						{
							// Get our current classification name and project
							string classification = rootClass.FullPath;

							// Create a form for user input
							ClassificationCreateForm ccf = new ClassificationCreateForm(classification);

							// Show our form
							if (ccf.ShowDialog() == DialogResult.OK)
							{
								ImportMogRepositoryTreeView.DeInitialize();

								//Tell the treeview to drill to the new classification when it reinitializes
								ImportMogRepositoryTreeView.LastNodePath = ccf.FullClassificationName;

								ImportMogRepositoryTreeView.Initialize();
							}
						}
					}
					catch (InvalidCastException ex)
					{
						throw new Exception("Add Classification", ex);
					}
					catch (Exception ex)
					{
						throw new Exception("Problem finding valid TreeView. " + "Add Classification", ex);
					}
				}
				catch (Exception ex)
				{
					MOG_Report.ReportMessage("Add Classification", ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
				}
			}
			else
			{
				MOG_Prompt.PromptResponse("Insufficient Privileges", "Your privileges do not allow you to add classifications to the project.");
			}
		}

		#endregion

		#region Wizard page show/hide events
		private void ImportAssetNameWizardPage_ValidateBeforeShow(object sender, MOG_ControlsLibrary.Utils.PageEventArgs e)
		{
			bool bSkipPage = false;
			string defaultText = "";

			PopulateAssetNameAndPlatforms();

			// Check if we have been seeded?
			if (mSeed_AssetLabel.Length > 0)
			{
				defaultText = mSeed_AssetLabel;
				bSkipPage = true;
			}
			else
			{
				// Load the properties of this asset based on the previously selected classification
				MOG_Properties classificationProperties = GetAssetProperties(mTargetImportMOGName);
				// Populate cbAssetExtensions from the asset's properties
				switch (classificationProperties.DefaultAssetNameIncludeExtension)
				{
					case MOG_DefaultPrompt.PromptDefaultNo:
						cbAssetExtensions.Checked = false;
						break;
					case MOG_DefaultPrompt.PromptDefaultYes:
						cbAssetExtensions.Checked = true;
						break;
					case MOG_DefaultPrompt.No:
						cbAssetExtensions.Checked = false;
						defaultText = DosUtils.PathGetFileNameWithoutExtension(mTargetSouceFileName);
						// Check if we have clearance to skip ahead?
						if (WizardUseClassificationDefaultPropertiesCheckBox.Checked)
						{
							bSkipPage = true;
						}
						break;
					case MOG_DefaultPrompt.Yes:
						cbAssetExtensions.Checked = true;
						defaultText = DosUtils.PathGetFileName(mTargetSouceFileName);
						// Check if we have clearance to skip ahead?
						if (WizardUseClassificationDefaultPropertiesCheckBox.Checked)
						{
							bSkipPage = true;
						}
						break;
				}
			}

			// Set the default text
			ImportAssetNameTextBox.Text = defaultText;
			EvaluateMogAssetName();
			// Load the properties for this asset
			mProperties = GetAssetProperties(mTargetImportMOGName);

			// Check if we should skip this page?
			if (bSkipPage)
			{
				// Check if we have clearance to skip ahead?
				if (WizardUseClassificationDefaultPropertiesCheckBox.Checked)
				{
					// Set our next page
					e.Page = ImportPlatformWizardPage;
				}
			}
		}

		private void ImportAssetNameWizardPage_ShowFromNext(object sender, EventArgs e)
		{
			PopulateAssetNameAndPlatforms();
		}

		private void ImportClassQuestionWizardPage_CloseFromNext(object sender, MOG_ControlsLibrary.Utils.PageEventArgs e)
		{
		}

		private void ImportSelectClassWizardPage_ShowFromNext(object sender, EventArgs e)
		{
			// Load the UseInheritedProperties setting
			if (MogUtils_Settings.MogUtils_Settings.SettingExist("ImportWizard", "UseInheritedProperties"))
			{
				try
				{
					bool check = Convert.ToBoolean(MogUtils_Settings.MogUtils_Settings.LoadSetting("ImportWizard", "UseInheritedProperties"));
					WizardUseClassificationDefaultPropertiesCheckBox.Checked = check;
				}
				catch { }
			}
			// Load the FilterClass setting
			if (MogUtils_Settings.MogUtils_Settings.SettingExist("ImportWizard", "FilterClass"))
			{
				try
				{
					bool check = Convert.ToBoolean(MogUtils_Settings.MogUtils_Settings.LoadSetting("ImportWizard", "FilterClass"));
					WizardClassificationUseInclusionFiltersCheckBox.Checked = check;
				}
				catch { }
			}

			InitializeClassificationTreeView(false);

			// Only enable the next button if a classification has been selected
			if (this.ImportTargetClassTextBox.Text.Length == 0)
			{
				NewAssetImportWizard.NextEnabled = false;
			}
		}

		private void WizardClassificationUseInclusionFiltersCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			InitializeClassificationTreeView(true);
			MogUtils_Settings.MogUtils_Settings.SaveSetting("ImportWizard", "FilterClass", WizardClassificationUseInclusionFiltersCheckBox.Checked.ToString());
		}

		private void InitializeClassificationTreeView(bool forceRebuild)
		{
			// Initialize our classification tree if it isn't already
			if (!ImportMogRepositoryTreeView.IsInitialized || forceRebuild)
			{
				// Should we filter?
				if (WizardClassificationUseInclusionFiltersCheckBox.Checked)
				{
					// Build a filter including appropriate wildcards
					string filter = "*." + DosUtils.PathGetExtension(ImportSourceFilename) + "*";
					ImportMogRepositoryTreeView.MogPropertyList.Add(MOG.MOG_PropertyFactory.MOG_Classification_InfoProperties.New_FilterInclusions(filter));
				}
				else
				{
					ImportMogRepositoryTreeView.MogPropertyList.Clear();
				}

				ImportMogRepositoryTreeView.Initialize();
				ImportMogRepositoryTreeView.mWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(mWorker_RunWorkerCompleted);
			}
		}

		void mWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (MogUtils_Settings.MogUtils_Settings.SettingExist("ImportWizard", "LastClass"))
			{
				string savedClass = MogUtils_Settings.MogUtils_Settings.LoadSetting("ImportWizard", "LastClass");
				TreeNode node = ImportMogRepositoryTreeView.DrillToNodePath(savedClass);
				if (node != null)
				{
// JohnRen - Removed because this was causing an extra event to come in on the AfterSelect which was clearing out LastPackage and LastDir settings
//					ImportMogRepositoryTreeView.Focus();
					ImportMogRepositoryTreeView.SelectedNode = node;
				}
			}
		}

		private void ImportSelectClassWizardPage_ValidateBeforeShow(object sender, MOG_ControlsLibrary.Utils.PageEventArgs e)
		{
			bool bSkipPage = false;

			// Make sure we show the user what we are importing
			this.Text = "Importing: " + DosUtils.PathGetFileName(mTargetSouceFileName);

			// Check if the platform has been seeded
			if (mSeed_AssetClassification.Length > 0)
			{
				// Force the seeded value
				mInternalLastSelectedClassification = mSeed_AssetClassification;
				bSkipPage = true;

				// Force this since we seeded the classification
				WizardUseClassificationDefaultPropertiesCheckBox.Checked = true;
			}

			// We must have a valid gameData controller to do this smart lookup
			MOG_ControllerSyncData sync = MOG_ControllerProject.GetCurrentSyncDataController();
			if (sync != null)
			{
				// Check for better match from source path of import file
				// Check if this source file is within our workspace?
				string fullPath = Path.GetDirectoryName(this.mTargetSouceFileName);
				if (DosUtils.PathIsWithinPath(sync.GetSyncDirectory(), fullPath))
				{
					// Attempt to match the relative path of the source file to our classification tree
					string relativePath = DosUtils.PathMakeRelativePath(sync.GetSyncDirectory(), fullPath);
					string classification = MOG_ControllerProject.GetProjectName() + "~" + relativePath.Replace("\\", "~");

					// Check to see if we get a valid class from this path
					if (MOG_ControllerProject.IsValidClassification(classification))
					{
						// Force the seeded value
						mInternalLastSelectedClassification = classification;
						bSkipPage = true;

						// Force this since we seeded the classification
						WizardUseClassificationDefaultPropertiesCheckBox.Checked = true;
					}
				}
			}

			// Check if we should skip this page?
			if (bSkipPage)
			{
				// Indicate our next page
				e.Page = this.ImportAssetNameWizardPage;
			}
		}

		private void ImportSelectClassWizardPage_CloseFromNext(object sender, MOG_ControlsLibrary.Utils.PageEventArgs e)
		{
			// Check if the the import file is coming from a workspace?
			string workspaceRoot = MOG_ControllerSyncData.DetectWorkspaceRoot(Path.GetDirectoryName(mTargetSouceFileName));
			if (workspaceRoot.Length > 0)
			{
				string relativePath = DosUtils.PathMakeRelativePath(workspaceRoot, Path.GetDirectoryName(mTargetSouceFileName));

				// Get the SyncTargetPath of this classification
				MOG_Properties props = new MOG_Properties(this.mInternalLastSelectedClassification);
				// Check if this file will be synced back down?
				if (props.SyncFiles)
				{
					string inheritedSyncTargetPath = MOG_Tokens.GetFormattedString(props.SyncTargetPath, mTargetImportMOGName, props.GetPropertyList());
					if (inheritedSyncTargetPath.Length > 0)
					{
						// Make sure this asset is being imported from the expected location
						if (string.Compare(inheritedSyncTargetPath, relativePath, true) != 0)
						{
							// Warn the user they are doing something odd
							string message = "The file being imported is comming from:\n" +
											 "     {Workspace}\\" + relativePath + "\n" +
											 "This classification's SyncTargetPath is:\n" +
											 "     {Workspace}\\" + inheritedSyncTargetPath + "\n\n" +
											 "Selecting 'Yes' will override this classification's preset SyncTargetPath in favor of this file's current location.\n" +
											 "If this was unintentional, it is recommended you move the file to the classification's SyncTargetPath and try again.";
							if (MOG_Prompt.PromptResponse("Ignore Classification's SyncTarget?", message, MOGPromptButtons.YesNo) == MOGPromptResult.Yes)
							{
								// Force the asset's sync target
								mSeed_AssetSyncTarget = relativePath;
							}
							else
							{
								// Force us to remain on this page
								e.Page = this.ImportSelectClassWizardPage;
								NewAssetImportWizard.NextEnabled = false;
							}
						}
					}
				}
			}

			// Make sure we have a selected node
			if (ImportMogRepositoryTreeView.SelectedNode != null)
			{
				MogUtils_Settings.MogUtils_Settings.SaveSetting("ImportWizard", "LastClass", ImportMogRepositoryTreeView.SelectedNode.FullPath);
				MogUtils_Settings.MogUtils_Settings.Save();
			}
		}

		private void ImportTargetDirWizardPage_ShowFromNext(object sender, EventArgs e)
		{
			Initialize_ImportTargetDirWizardPage();
		}

		private void Initialize_ImportTargetDirWizardPage()
		{
			if (!GameDataDestinationTreeView.Initialized)
			{
				// Initialize with some reasonable defaults
				string workspaceDirectory = MOG_ControllerProject.GetProjectName();
				string platform = mTargetImportMOGName.GetAssetPlatform();
				// Check if we have a current workspace?
				if (MOG_ControllerProject.GetCurrentSyncDataController() != null)
				{
					workspaceDirectory = MOG_ControllerProject.GetWorkspaceDirectory();
					platform = MOG_ControllerProject.GetPlatformName();
				}
				GameDataDestinationTreeView.InitializeVirtual(workspaceDirectory, platform);
			}

			// Check if this import file is comming from within the current workspace?
			if (DosUtils.PathIsWithinPath(MOG_ControllerProject.GetWorkspaceDirectory(), mTargetSouceFileName))
			{
				// Drill down to the proper node
				string relativePath = DosUtils.PathMakeRelativePath(MOG_ControllerProject.GetWorkspaceDirectory(), mTargetSouceFileName);
				GameDataDestinationTreeView.VirtualFindAndExpand(GameDataDestinationTreeView.MOGRootNode.Nodes, relativePath);
			}
			// Check what the user used last time?
			else if (MogUtils_Settings.MogUtils_Settings.SettingExist("ImportWizard", "LastDir"))
			{
				string savedDir = MogUtils_Settings.MogUtils_Settings.LoadSetting("ImportWizard", "LastDir");
				GameDataDestinationTreeView.VirtualFindAndExpand(GameDataDestinationTreeView.MOGRootNode.Nodes, savedDir);
			}

			if (ImportTargetDirTextBox.Text.Length == 0)
			{
				// Disable the next button till a valid target is set
				NewAssetImportWizard.NextEnabled = false;
			}
		}

		private void ImportEndWizardPage_ShowFromNext(object sender, EventArgs e)
		{
			// Set the wizard's final values
			ImportEndSourceTextBox.Text = mTargetSouceFileName;
			ImportEndMogTextBox.Text = mTargetImportMOGName.GetAssetFullName();
			ImportEndPlatformTextBox.Text = ImportValidPlatformsComboBox.Text;
			// Check if this is a syncing asset
			if (mProperties.SyncFiles)
			{
				// Obtain the sync target path...Show something nice and pretty if there is none
				ImportEndTargetTextBox.Text = (mTargetImportSyncPath.Length > 0) ? mTargetImportSyncPath : "{Project Root} ";
				ImportEndTargetTextBox.BackColor = Color.LightGreen;
			}
			// Check if this is a PackagedAsset?
			else if (mProperties.IsPackagedAsset)
			{
				// Obtain the package assignment
				ImportEndTargetTextBox.Text = mTargetImportSyncPath;
				ImportEndTargetTextBox.BackColor = Color.LightGreen;
			}
			else
			{
				// Indicate this will be unused
				ImportEndTargetTextBox.Text = "n/a";
				ImportEndTargetTextBox.BackColor = Color.LightGray;
			}

			// Enable 'Apply to All' if multi file importing
			if (ImportHasMultiples)
			{
				ImportEndMultiImportLabel.Visible = true;
				ImportEndMultiImport2Label.Visible = true;
				ImportEndMultiImportApplyToAllCheckBox.Visible = true;
			}
		}

		private void ImportTargetPackageWizardPage_ValidateBeforeShow(object sender, MOG_ControlsLibrary.Utils.PageEventArgs e)
		{
			if (mProperties.IsPackagedAsset == true)
			{
				// Make sure we actually have package assignments
				if (mProperties.GetPackages().Count != 0)
				{
					// Use the first one and update the GUI's controls
					MOG_Property packageAssignment = mProperties.GetPackages()[0] as MOG_Property;
					if (packageAssignment != null)
					{
						// Get the mog package name
						string package = packageAssignment.mPropertyKey;
						// Get the full path to where the package is stored on the network
						MOG_Filename packageFullPath = MOG_ControllerProject.GetAssetCurrentBlessedPath(new MOG_Filename(package));
						// Set our target path
						mTargetImportSyncPath = ImportTargetDirTextBox.Text = package;
					}

					// Push all the package assignment properties
					mTargetImportProperties.AddRange(mProperties.GetPackages());

					// Check if we have clearance to skip ahead?
					if (WizardUseClassificationDefaultPropertiesCheckBox.Checked)
					{
						// Set our next page to the end
						e.Page = ImportEndWizardPage;
					}
				}
			}
			else
			{
				// Indicate our next page
				e.Page = ImportTargetDirWizardPage;
			}
		}

		private void ImportTargetPackageWizardPage_CloseFromNext(object sender, MOG_ControlsLibrary.Utils.PageEventArgs e)
		{
			mTargetImportSyncPath = ImportTargetPackageTextBox.Text;
			CreateTargetPackageProperty(PackageTreeView.SelectedPackageFullName, PackageTreeView.SelectedPackageFullFilename);
			e.Page = ImportEndWizardPage;

			MogUtils_Settings.MogUtils_Settings.SaveSetting("ImportWizard", "LastPackage", ImportTargetPackageTextBox.Text);
		}

		private void ImportTargetPackageWizardPage_ShowFromNext(object sender, EventArgs e)
		{
			Initialize_ImportTargetPackageWizardPage();
		}

		private void Initialize_ImportTargetPackageWizardPage()
		{
			// Only refresh this tree if we havn't built it before
			if (!PackageTreeView.TreeView.IsInitialized)
			{
				PackageTreeView.TreeView.Initialize(Initialize_PackageTreeView_RunWorkerCompleted);

				// Show all available packages
				PackageTreeView.TreeView.ShowPlatformSpecific = true;
			}
		}

		private void Initialize_PackageTreeView_RunWorkerCompleted()
		{
			// I'm not sure why this wizard works differently than the advance import dialog...
			// Use the LastNodePath so the newly create package will get auto selected
			if (PackageTreeView.TreeView.LastNodePath != null &&
				PackageTreeView.TreeView.LastNodePath.Length > 0)
			{
				this.ImportTargetPackageTextBox.Text = PackageTreeView.TreeView.LastNodePath;
			}

			// Check if we are still missing a target?
			if (this.ImportTargetPackageTextBox.Text.Length > 0)
			{
				PackageTreeView.TreeView.SelectedNode = PackageTreeView.TreeView.DrillToAssetName(this.ImportTargetPackageTextBox.Text);
			}
			else
			{
				// Check what the user used last time?
				if (MogUtils_Settings.MogUtils_Settings.SettingExist("ImportWizard", "LastPackage"))
				{
					this.ImportTargetPackageTextBox.Text = MogUtils_Settings.MogUtils_Settings.LoadSetting("ImportWizard", "LastPackage");
				}
			}

			if (ImportTargetPackageTextBox.Text.Length == 0)
			{
				NewAssetImportWizard.NextEnabled = false;
			}
		}

		private void ImportTargetDirWizardPage_ValidateBeforeShow(object sender, MOG_ControlsLibrary.Utils.PageEventArgs e)
		{
			// Check if we are a syncing asset
			if (mProperties.SyncFiles == true)
			{
				string header = "{Project Root}: ";

				// Default to the seeded value
				string unformattedSyncTarget = mSeed_AssetSyncTarget;
				// Check if we still need something?
				if (unformattedSyncTarget.Length == 0)
				{
					// Use our inherited property
					unformattedSyncTarget = mProperties.SyncTargetPath;
					if (unformattedSyncTarget.Length > 0)
					{
						header = "{Inherited}: ";
					}
				}
				// Check if we still need something?
				if (unformattedSyncTarget.Length == 0)
				{
					// We must have a valid gameData controller to do this assumption
					MOG_ControllerSyncData sync = MOG_ControllerProject.GetCurrentSyncDataController();
					if (sync != null)
					{
						// Check for better match from source path of import file
						// Check if this source file is within our workspace?
						string fullPath = Path.GetDirectoryName(this.mTargetSouceFileName);
						if (DosUtils.PathIsWithinPath(sync.GetSyncDirectory(), fullPath))
						{
							// Respect where this asset is comming from and skip to the end
							string relativePath = DosUtils.PathMakeRelativePath(sync.GetSyncDirectory(), fullPath);
							unformattedSyncTarget = relativePath;
							header = "{Implied}: ";
						}
					}
				}

				// Check if we have something to use?
				if (unformattedSyncTarget.Length > 0)
				{
					string formattedSyncTarget = MOG_Tokens.GetFormattedString(unformattedSyncTarget, mTargetImportMOGName, mProperties.GetPropertyList());
					ImportTargetDirTextBox.Text = CreateSyncDirectoryProperty(unformattedSyncTarget);
					mTargetImportSyncPath = header + ImportTargetDirTextBox.Text;

					// Check if we have clearance to skip ahead?
					if (WizardUseClassificationDefaultPropertiesCheckBox.Checked)
					{
						// Go to the done page
						e.Page = ImportEndWizardPage;
					}
				}
			}
            else
            {
				// Go to the done page
				e.Page = ImportEndWizardPage;
            }
		}

		private void ImportTargetDirWizardPage_CloseFromNext(object sender, MOG_ControlsLibrary.Utils.PageEventArgs e)
		{
			// Create the sync target property
			mTargetImportSyncPath = CreateSyncDirectoryProperty(ImportTargetDirTextBox.Text);

			// Are we a classless import?
			if (ImportClassifyNoRadioButton.Checked)
			{
				// Create a class for this asset
				mInternalLastSelectedClassification = CreateDefaultClassification(mTargetImportSyncPath);

				// Evaluate the MOG filename to set all our internal vars
				EvaluateMogAssetName();
			}

			MogUtils_Settings.MogUtils_Settings.SaveSetting("ImportWizard", "LastDir", mTargetImportSyncPath);
			MogUtils_Settings.MogUtils_Settings.Save();
		}

		private void ImportPreviousWizardPage_CloseFromNext(object sender, MOG_ControlsLibrary.Utils.PageEventArgs e)
		{
			if (ImportExistingYesRadioButton.Checked)
			{
				if (ImportExistingListView.SelectedItems != null &&
					ImportExistingListView.SelectedItems.Count == 1)
				{
					// Get the asset full filename
					MOG_Filename assetFilename = new MOG_Filename(ImportExistingListView.SelectedItems[0].SubItems[3].Text);

					// Set the wizard's final values
					ImportEndSourceTextBox.Text = mTargetSouceFileName;
					ImportEndMogTextBox.Text = assetFilename.GetAssetFullName();
					ImportEndPlatformTextBox.Text = assetFilename.GetAssetPlatform();
					ImportEndTargetTextBox.Text = "Will be assumed when reimported using asset's previous properties";

					// Shut down wizard
					DialogResult = DialogResult.OK;
					Close();
				}
			}
		}

		private void ImportPreviousWizardPage_ShowFromNext(object sender, EventArgs e)
		{
			// Create a listviewItem for each item listed in the possible matches
			InitializePotentialMatches();

			NewAssetImportWizard.NextEnabled = false;
		}

		private void ImportStartWizardPage_ValidateBeforeShow(object sender, MOG_ControlsLibrary.Utils.PageEventArgs e)
		{
			// Has the user told us to only start with the Wizard
			string StartupMode = MogUtils_Settings.MogUtils_Settings.LoadSetting("ImportWizard", "StartupMode");
			if (StartupMode == "Wizard")
			{
				// Skip this page
				e.Page = ImportSelectClassWizardPage;
			}
		}

		private void ImportStartWizardPage_CloseFromNext(object sender, MOG_ControlsLibrary.Utils.PageEventArgs e)
		{
			// Should we save the startup default?
			if (ImportStartSetDefaultCheckBox.Checked)
			{
				// Save default preferance
				string mode = (ImportAdvancedRadioButton.Checked) ? "Advanced" : "Wizard";
				MogUtils_Settings.MogUtils_Settings.SaveSetting("ImportWizard", "StartupMode", mode);
			}

			// Has the user told us to only start with the Wizard
			string StartupMode = MogUtils_Settings.MogUtils_Settings.LoadSetting("ImportWizard", "StartupMode");
			if (StartupMode == "Wizard")
			{
				ImportStartSetDefaultCheckBox.Checked = true;
				NewAssetImportWizard.ActivateNextPage(2);
			}
			else if (StartupMode == "Advanced" ||
					 ImportAdvancedRadioButton.Checked)
			{
				// If the advanced MOG Asset Importer is checked, we need to exit this wizard and load the Importer
				DialogResult = DialogResult.Ignore;
				Close();
			}
		}

		private void CheckPotentialMatches()
		{
			bool SkipPotentialMatches = false;

			// Check if we have any potential matches
			if (mInternalPotentialMatches == null)
			{
				SkipPotentialMatches = true;
			}
			else if (mInternalPotentialMatches != null && mInternalPotentialMatches.Count == 0)
			{
				SkipPotentialMatches = true;
			}

			if (SkipPotentialMatches)
			{
				NewAssetImportWizard.ActivateNextPage(ImportStartWizardPage);
			}
			else
			{
				// Create a listviewItem for each item listed in the possible matches
				InitializePotentialMatches();
				NewAssetImportWizard.NextEnabled = (ImportExistingListView.SelectedItems.Count == 0) ? false : true;
			}
		}

		private void ImportPlatformWizardPage_ValidateBeforeShow(object sender, MOG_ControlsLibrary.Utils.PageEventArgs e)
		{
			bool bSkipPage = false;
			string defaultPlatform = "All";

			// Check if there is a mSeed_AssetPlatform?
			if (mSeed_AssetPlatform.Length > 0)
			{
				// Use the seeded value
				defaultPlatform = mSeed_AssetPlatform;
				bSkipPage = true;
			}
			else
			{
				// Check if we have a platform specified in our properties?
				if (mProperties.DefaultAssetNamePlatform.Length > 0)
				{
					// Attempt to use the default from our properties
					defaultPlatform = mProperties.DefaultAssetNamePlatform;
					// Check if we have clearance to skip ahead?
					if (WizardUseClassificationDefaultPropertiesCheckBox.Checked)
					{
						bSkipPage = true;
					}
				}
			}

			// Set our default platform text
			ImportValidPlatformsComboBox.Text = defaultPlatform;
			EvaluateMogAssetName();

			// Check if we should skip this page?
			if (bSkipPage)
			{
				// Set our next page
				e.Page = ImportTargetPackageWizardPage;
			}
		}

		// Skip 'NO CLASSIFIY' option wizard page
		private void ImportPlatformWizardPage_CloseFromNext(object sender, MOG_ControlsLibrary.Utils.PageEventArgs e)
		{
			// Evaluate the MOG filename to set all our internal vars
			EvaluateMogAssetName();
		}

		#endregion

		#region Control Events

		private void ImportAssetWizard_Load(object sender, EventArgs e)
		{
			ImportSourceFileTextBox.Text = ImportSourceFilename;
			ImportPreviousSourceTextBox.Text = ImportSourceFilename;

			// Here is a whole bunch of junk we are going to try to make sure this form shows up on top and that it can be reachable if not
			if (Parent == null) ShowInTaskbar = true;
			TopLevel = true;
			BringToFront();
			Activate();
			Focus();
		}

		private void cbAssetExtensions_CheckedChanged(object sender, EventArgs e)
		{
			PopulateAssetNameAndPlatforms();
		}

		private void ImportMogRepositoryTreeView_AfterSelect(object sender, TreeViewEventArgs e)
		{
			// So long as we have a selectedNode, store its fullPath as the classification...
			if (ImportMogRepositoryTreeView.SelectedNode != null)
			{
				this.mInternalLastSelectedClassification = this.ImportMogRepositoryTreeView.SelectedNode.FullPath;

				EvaluateMogAssetName();

				// Check if we just change from the last saved classification?
				if (string.Compare(this.mInternalLastSelectedClassification, MogUtils_Settings.MogUtils_Settings.LoadSetting("ImportWizard", "LastClass"), true) != 0)
				{
					// Clear our saved settings because the user just selected a different classification than last time
					MogUtils_Settings.MogUtils_Settings.SaveSetting("ImportWizard", "LastDir", "");
					GameDataDestinationTreeView.MOGSelectedNode = null;
					ImportTargetDirTextBox.Text = "";
					MogUtils_Settings.MogUtils_Settings.SaveSetting("ImportWizard", "LastPackage", "");
					PackageTreeView.TreeView.SelectedNode = null;
					ImportTargetPackageTextBox.Text = "";
				}

				// We now have a valid classification name
				NewAssetImportWizard.NextEnabled = true;
			}
		}

		private void GameDataDestinationTreeView_AfterTargetSelect(object sender, TreeViewEventArgs e)
		{
			// Do we have a valid target?
			if (GameDataDestinationTreeView.MOGGameDataTarget != "")
			{
				string targetPath = GameDataDestinationTreeView.MOGGameDataTarget;

				// Initialize with some reasonable defaults
				string root = MOG_ControllerProject.GetProjectName();
				// Check if we have a current workspace?
				if (MOG_ControllerProject.GetCurrentSyncDataController() != null)
				{
					root = MOG_ControllerProject.GetWorkspaceDirectory();
				}
				// Check if this import file is comming from within the current workspace?
				if (DosUtils.PathIsWithinPath(root, targetPath))
				{
					// Drill down to the proper node
					targetPath = DosUtils.PathMakeRelativePath(root, targetPath);
				}

				// Set our EditBox to show the selected path
				ImportTargetDirTextBox.Text = targetPath;
				ImportTargetDirTextBox.BackColor = Color.LightGreen;

				// We now have a valid classification name
				NewAssetImportWizard.NextEnabled = true;
			}
		}


		private void PackageTreeView_AfterPackageSelect(object sender, TreeViewEventArgs e)
		{
			// Do we have a valid package?
			if (PackageTreeView.SelectedPackageFullFilename != "")
			{
				string fullPackageName = PackageTreeView.SelectedPackageFullName;
				string fullPackageFilename = PackageTreeView.SelectedPackageFullFilename;

				// Set our EditBox and create our property
				ImportTargetPackageTextBox.Text = CreateTargetPackageProperty(fullPackageName, fullPackageFilename); ;

				// ReEnable the next button
				NewAssetImportWizard.NextEnabled = true;
			}
		}

		private void ImportExistingListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			if (ImportExistingListView.SelectedItems != null &&
				ImportExistingListView.SelectedItems.Count == 1)
			{
				NewAssetImportWizard.NextEnabled = true;
			}
			else
			{
				NewAssetImportWizard.NextEnabled = false;
			}
		}

		private void ImportExistingListView_DoubleClick(object sender, EventArgs e)
		{
			if (ImportExistingListView.SelectedItems != null && 
				ImportExistingListView.SelectedItems.Count == 1)
			{
				NewAssetImportWizard.NextEnabled = true;
				NewAssetImportWizard.Next();
			}
			else
			{
				NewAssetImportWizard.NextEnabled = false;
			}
		}

		private void ImportExistingRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			ImportExistingListView.Enabled = ImportExistingYesRadioButton.Checked;
		}

		private void ImportExistingNoRadioButton_Click(object sender, EventArgs e)
		{
			if (ImportExistingNoRadioButton.Checked)
			{
				NewAssetImportWizard.NextEnabled = true;
			}
			else
			{
				NewAssetImportWizard.NextEnabled = false;
			}
		}

		private void ImportAddMenuItem_Click(object sender, EventArgs e)
		{
			AddClassification();
		}

		private void ImportClassificationAddButton_Click(object sender, EventArgs e)
		{
			AddClassification();
		}
		#endregion

		private void NewAssetImportWizard_Load(object sender, EventArgs e)
		{
			CheckPotentialMatches();
		}

		private void WizardUseClassificationDefaultPropertiesCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			MogUtils_Settings.MogUtils_Settings.SaveSetting("ImportWizard", "UseInheritedProperties", WizardUseClassificationDefaultPropertiesCheckBox.Checked.ToString());
			MogUtils_Settings.MogUtils_Settings.Save();
		}

		private void ImportTargetClassTextBox_TextChanged(object sender, EventArgs e)
		{
			// Check if this is blank?  or
			// Check if this contains any invalid characters?
			if (ImportTargetClassTextBox.Text.Length == 0 ||
				MOG_ControllerSystem.InvalidMOGCharactersCheck(ImportAssetNameTextBox.Text, true))
			{
				ImportTargetClassTextBox.BackColor = Color.Tomato;
			}
			else
			{
				ImportTargetClassTextBox.BackColor = Color.LightGreen;
			}
		}

		private void ImportAssetNameTextBox_TextChanged(object sender, EventArgs e)
		{
			// Check if this is blank?  or
			// Check if this contains any invalid characters?
			if (ImportAssetNameTextBox.Text.Length == 0 ||
				MOG_ControllerSystem.InvalidMOGCharactersCheck(ImportAssetNameTextBox.Text, true))
			{
				ImportAssetNameTextBox.BackColor = Color.Tomato;
				NewAssetImportWizard.NextEnabled = false;
			}
			else
			{
				ImportAssetNameTextBox.BackColor = Color.LightGreen;
				NewAssetImportWizard.NextEnabled = true;
			}
		}

		private void ImportValidPlatformsComboBox_TextChanged(object sender, EventArgs e)
		{
			// Check if this is blank?  or
			// Check if this contains any invalid characters?
			if (ImportValidPlatformsComboBox.Text.Length == 0 ||
				!MOG_ControllerProject.IsValidPlatform(ImportValidPlatformsComboBox.Text) || 
				MOG_ControllerSystem.InvalidMOGCharactersCheck(ImportAssetNameTextBox.Text, true))
			{
				ImportValidPlatformsComboBox.BackColor = Color.Tomato;
				NewAssetImportWizard.NextEnabled = false;
			}
			else
			{
				ImportValidPlatformsComboBox.BackColor = Color.LightGreen;
				NewAssetImportWizard.NextEnabled = true;
			}
		}

		private void ImportTargetPackageTextBox_TextChanged(object sender, EventArgs e)
		{
			// Check if this is blank?  or
			// Check if this contains any invalid characters?
			if (ImportTargetPackageTextBox.Text.Length == 0 ||
				MOG_ControllerSystem.InvalidMOGCharactersCheck(ImportAssetNameTextBox.Text, true))
			{
				ImportTargetPackageTextBox.BackColor = Color.Tomato;
				NewAssetImportWizard.NextEnabled = false;
			}
			else
			{
				ImportTargetPackageTextBox.BackColor = Color.LightGreen;
				NewAssetImportWizard.NextEnabled = true;
			}
		}

		private void ImportTargetDirTextBox_TextChanged(object sender, EventArgs e)
		{
			// Check if this is blank?  or
			// Check if this contains any invalid characters?
			if (MOG_ControllerSystem.InvalidMOGCharactersCheck(ImportAssetNameTextBox.Text, true))
			{
				ImportTargetDirTextBox.BackColor = Color.Tomato;
				NewAssetImportWizard.NextEnabled = false;
			}
			else
			{
				ImportTargetDirTextBox.BackColor = Color.LightGreen;
				NewAssetImportWizard.NextEnabled = true;
			}
		}		
	}

    public class ImportFile
    {
        public string mImportFilename = "";
        public ArrayList mPotentialFileMatches = new ArrayList();

        public ImportFile(string file) { mImportFilename = file; }
    }

}