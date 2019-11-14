using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using System.Threading;

using MOG;
using MOG.DOSUTILS;
using MOG.FILENAME;
using MOG.PLATFORM;
using MOG.PROPERTIES;
using MOG.DATABASE;
using MOG.PROJECT;
using MOG.REPORT;
using MOG.PROMPT;
using MOG.PROGRESS;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERASSET;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.CONTROLLER.CONTROLLERPACKAGE;
using MOG.CONTROLLER.CONTROLLERSYNCDATA;

using MOG_ControlsLibrary.Controls;
using MOG_ControlsLibrary.Utils;

namespace MOG_ControlsLibrary.Forms
{
	/// <summary>
	/// Summary description for ImportAssetTreeForm.
	/// </summary>
	public class ImportAssetTreeForm : System.Windows.Forms.Form
	{
		private const int AdvancedWidth = 624;
		private const int BasicWidth = 400;

		private string			mSeed_AssetLabel = "";
		private string			mSeed_AssetClassification = "";
		private string			mSeed_AssetPlatform = "";
		private string			mSeed_AssetSyncTarget = "";
		private ArrayList		mSeed_AssetProperties = new ArrayList();
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
			set { mSeed_AssetProperties = value; }
		}

		private string mSourceFullFilename;
		private MOG_Filename mFinalAssetName = new MOG_Filename("");
		private MOG_Properties mProperties;
		private ArrayList mMOGPropertyArray = new ArrayList();
		public bool mAdvancedOpened;
		private ArrayList mPotentialMatches = new ArrayList();
		private MogUtil_ControlHide mGameTree;
		private MogUtil_ControlHide mFoundAssets;

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button ImportOkButton;
		private System.Windows.Forms.Button ImportCancelButton;
		private System.Windows.Forms.TextBox ImportAssetNameTextBox;
		public System.Windows.Forms.ComboBox ImportValidPlatformsComboBox;
		public MOG_ControlsLibrary.Controls.MogControl_ImportTreeView ImportMogRepositoryTreeView;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button ImportAdvancedButton;
		private System.Windows.Forms.Button ImportOkAllButton;
		private MogControl_GameDataDestinationTreeView GameDataDestinationTreeView;
		private System.Windows.Forms.Label label3;
		public System.Windows.Forms.TextBox ImportTargetClassTextBox;
		public System.Windows.Forms.TextBox ImportTargetDestinationTextBox;
		private System.Windows.Forms.Panel TreesPanel;
		private System.Windows.Forms.Panel GameDataClassTreesPanel;
		private System.Windows.Forms.Splitter TreesSplitter;
		private System.Windows.Forms.ListView ImportPotentialAssetsListView;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.Splitter FoundAssetsSplitter;
		private System.Windows.Forms.ToolTip MOGToolTip;
		private System.Windows.Forms.Label AdvancedTreesLabel;
		private System.Windows.Forms.Label TargetLabel;
		private System.Windows.Forms.Label NoWorkspaceAvailableLabel;
		private MogControl_PackageManagementTreeView PackageTreeView;
		private CheckBox cbAssetExtensions;
		private System.ComponentModel.IContainer components;

		/// <summary>
		/// Get the MOG_Properties for this asset
		/// </summary>
		public ArrayList MOGPropertyArray
		{
			get { return mMOGPropertyArray; }
		}

		public MOG_Filename FinalAssetName
		{
			get { return mFinalAssetName; }
		}

		public bool MOGShowExtensions
		{
			get { return cbAssetExtensions.Checked; }
		}

		public string GetFixedAssetName()
		{
			return this.mFinalAssetName.GetAssetFullName();
		}
		public ImportAssetTreeForm(MOG_ControlsLibrary.Forms.Wizards.ImportFile fullFilename)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// Initalize the context menus
			ImportMogRepositoryTreeView.ContextMenuStrip = new ContextMenuStrip();
			ImportMogRepositoryTreeView.ContextMenuStrip.Items.Add(new ToolStripMenuItem("Add Classification...", null, AddClassification_Click));
            ImportMogRepositoryTreeView.ContextMenuStrip.Items.Add(new ToolStripMenuItem("Refresh", null, RefreshClassification_Click, Keys.F5));

			// While we're initializing, indicate such to the user...
			this.ImportMogRepositoryTreeView.Visible = false;

			// We want to start this form at transparent while we build it
			//this.Opacity = 0;

			// Set up our variables
			mFinalAssetName = new MOG_Filename();
			mProperties = null;

			mSourceFullFilename = fullFilename.mImportFilename;
			mPotentialMatches = fullFilename.mPotentialFileMatches;
			mMOGPropertyArray.AddRange(mSeed_AssetProperties);

			// Set up our hiders for the advanced sections of the form
			mGameTree = new MogUtil_ControlHide(288, 0, true);
			mFoundAssets = new MogUtil_ControlHide(100, 0, false);

			cbAssetExtensions.Enabled = (DosUtils.PathGetExtension(mSourceFullFilename).Length > 0) ? true : false;

			mAdvancedOpened = false;
			this.FoundAssetsSplitter.SplitPosition = mFoundAssets.Width;

			// Start by populating our comboBox
			PopulateAssetNameAndPlatforms();

			this.ImportMogRepositoryTreeView.Visible = true;
		}

		private void AddClassification_Click(object sender, EventArgs e)
		{
			MOG_Privileges privs = MOG_ControllerProject.GetPrivileges();
			if (privs.GetUserPrivilege(MOG_ControllerProject.GetUserName(), MOG_PRIVILEGE.AddClassification))
			{
				ToolStripMenuItem item = sender as ToolStripMenuItem;
				if (item != null)
				{
					ContextMenuStrip menu = item.Owner as ContextMenuStrip;
					if (menu != null)
					{
						Control control = menu.SourceControl;
						if (control != null)
						{
							if (ImportMogRepositoryTreeView.SelectedNode != null)
							{
								string classification = ImportMogRepositoryTreeView.SelectedNode.FullPath;

                                if (ImportMogRepositoryTreeView.SelectedNode != null)
                                {
                                    ImportMogRepositoryTreeView.SelectedNode.Collapse();
                                    ImportMogRepositoryTreeView.SelectedNode.Nodes.Clear();
                                    ImportMogRepositoryTreeView.SelectedNode.Nodes.Add(MogControl_BaseTreeView.Blank_Node_Text);
                                }

								ClassificationCreateForm form = new ClassificationCreateForm(classification);
								if (form.ShowDialog(control.TopLevelControl) == DialogResult.OK)
								{
									// Automatically set us to the classification we just created
									ImportTargetClassTextBox.Text = form.FullClassificationName;                                    
								}
							}
						}
					}
				}
			}
			else
			{
				MOG_Prompt.PromptResponse("Insufficient Privileges", "Your privileges do not allow you to add classifications to the project.");
			}
		}

        private void RefreshClassification_Click(object sender, EventArgs e)
        {
            ImportMogRepositoryTreeView.Initialize();
        }

		/// <summary>
		/// Populates our platforms combobox and asset-related textBoxes
		/// </summary>
		private void PopulateAssetNameAndPlatforms()
		{
			// Populate platforms combobox
			this.ImportValidPlatformsComboBox.Items.Clear();
			this.ImportValidPlatformsComboBox.Items.Add("All");

			// Add all the platforms
			System.Collections.IEnumerator myEnumerator = MOG_ControllerProject.GetProject().GetPlatforms().GetEnumerator();
			while (myEnumerator.MoveNext())
			{
				ImportValidPlatformsComboBox.Items.Add(((MOG_Platform)myEnumerator.Current).mPlatformName);
			}

			// If we have potential matches list them
			InitializePotentialMatches();
		}

		/// <summary>
		/// Part of InitializePotentialMatches()
		/// </summary>
		private void InitializePotentialMatches()
		{
			// If we have potential matches list them
			if (mPotentialMatches != null && mPotentialMatches.Count > 0)
			{
				foreach (MOG_Filename file in mPotentialMatches)
				{
					ListViewItem item = new ListViewItem();

					item.Text = file.GetAssetFullName();
					item.SubItems.Add(MOG_ControllerProject.GetAssetCurrentBlessedVersionPath(file).GetVersionTimeStampString(""));
					item.SubItems.Add("");
					item.SubItems.Add(file.GetNotSureDefaultEncodedFilename());

					ImportPotentialAssetsListView.Items.Add(item);
				}

				this.ImportPotentialAssetsListView.Enabled = true;

				if (!mFoundAssets.Opened)
				{
					this.FoundAssetsSplitter.SplitPosition = mFoundAssets.ToggleWidth();
				}
			}
		}

		private void UpdateFinalAssetName()
		{
			string classification = this.ImportTargetClassTextBox.Text;
			string platform = ImportValidPlatformsComboBox.Text;
			string assetName = ImportAssetNameTextBox.Text;

			// Validate this information to see if we can create our legitimate asset name?
			if (classification.Length > 0 &&
				platform.Length > 0 &&
				assetName.Length > 0)
			{
				// Generate our proposed asset name
				MOG_Filename generatedAssetName = MOG_Filename.CreateAssetName(classification, platform, assetName);
				if (generatedAssetName != null)
				{
					// Check to see if the asset name needs to be changed?
					if (string.Compare(generatedAssetName.GetAssetFullName(), mFinalAssetName.GetAssetFullName(), true) != 0)
					{
						mFinalAssetName = generatedAssetName;

						// Indicate the asset name in the title bar
						this.Text = "Import ( " + mFinalAssetName.GetFilename() + " )";

						// Update the precached properties
						UpdateAssetPropertiesIncludingSeeds();
					}
				}
			}
		}

		private void UpdateAssetTargetTree()
		{
			if (mProperties != null)
			{
				if (mProperties.IsPackagedAsset)
				{
					SwitchTreeViews("packages");

					// Check if the tree is out-of-sync?
					this.PackageTreeView.TreeView.SelectedNode = PackageTreeView.TreeView.DrillToPackage(this.ImportTargetDestinationTextBox.Text);
				}
				else if (mProperties.SyncFiles)
				{
					SwitchTreeViews("gamedata");

					// Check if the tree is out-of-sync?
					if (GameDataDestinationTreeView.MOGSelectedNode == null ||
						string.Compare(this.ImportTargetDestinationTextBox.Text, GameDataDestinationTreeView.MOGSelectedNode.FullPath, true) != 0)
					{
						if (this.ImportTargetDestinationTextBox.Text.Length > 0)
						{
							// Attempt to drill to the specified path
							string relativeSyncTargetPath = DosUtils.PathMakeRelativePath(this.GameDataDestinationTreeView.MOGRootNode.FullPath, this.ImportTargetDestinationTextBox.Text);
							this.GameDataDestinationTreeView.MOGSelectedNode = MogUtil_ClassificationTrees.FindAndExpandTreeNodeFromFullPath(this.GameDataDestinationTreeView.MOGRootNode.Nodes, "\\", relativeSyncTargetPath);
						}
						else
						{
							this.GameDataDestinationTreeView.MOGSelectedNode = null;
						}
					}
				}
				else
				{
					SwitchTreeViews("none");
				}
			}
		}

		private void SwitchTreeViews(string type)
		{
			switch (type)
			{
			case "none":
				this.PackageTreeView.Visible = false;
				this.GameDataDestinationTreeView.Visible = false;
				break;

			case "packages":
				// Check if we are switching trees?
				if (this.PackageTreeView.Visible == false)
				{
					// Only refresh this tree if we havn't built it before
					if (!PackageTreeView.TreeView.IsInitialized)
					{
						PackageTreeView.TreeView.Initialize(Initialize_PackageTreeView_RunWorkerCompleted);
						PackageTreeView.TreeView.AfterSelect += new TreeViewEventHandler(PackageTreeView_AfterSelect);
					}

					// Swap trees
					this.PackageTreeView.Visible = true;
					this.GameDataDestinationTreeView.Visible = false;

					// Update controls
					this.TargetLabel.Text = "Target package:";
					this.AdvancedTreesLabel.Text = "Browse target package for this asset:";
					this.PackageTreeView.Dock = DockStyle.Fill;
					this.PackageTreeView.BringToFront();
				}
				break;

			case "gamedata":
				// Check if we are switching trees?
				if (this.GameDataDestinationTreeView.Visible == false)
				{
					// Swap trees
					this.PackageTreeView.Visible = false;
					this.GameDataDestinationTreeView.Visible = true;

					// Update controls
					this.TargetLabel.Text = "Target hard drive destination:";
					this.AdvancedTreesLabel.Text = "Browse local target for this asset:";
					this.GameDataDestinationTreeView.Dock = DockStyle.Fill;
					this.GameDataDestinationTreeView.BringToFront();
				}
				break;
			}
		}

		private void Initialize_PackageTreeView_RunWorkerCompleted()
		{
			// Check if we are still missing a target?
			if (this.ImportTargetDestinationTextBox.Text.Length > 0)
			{
				PackageTreeView.TreeView.SelectedNode = PackageTreeView.TreeView.DrillToAssetName(this.ImportTargetDestinationTextBox.Text);
			}
			else
			{
				// Check what the user used last time?
				if (MogUtils_Settings.MogUtils_Settings.SettingExist("ImportWizard", "LastPackage"))
				{
					this.ImportTargetDestinationTextBox.Text = MogUtils_Settings.MogUtils_Settings.LoadSetting("ImportWizard", "LastPackage");
				}
			}
		}

		private void PopulateTreeViewData()
		{
			// Load prefs
			this.Opacity = 100;
			// If we loaded our settings, but did not finish loading our RepositoryTreeView, lets finish it now...
			if (!ImportMogRepositoryTreeView.IsInitialized)
			{
				ImportMogRepositoryTreeView.Initialize(mLoadingWorker_RunWorkerCompleted);
			}

			// Go ahead and select the first platform on the list
			ImportValidPlatformsComboBox.SelectedIndex = 0;
		}

		private void mLoadingWorker_RunWorkerCompleted()
		{
			// Make sure we repopulate the dialog according to any seeded values
			SeedDialog();
		}

		private void SeedDialog()
		{
			// Seed the asset's classification?
			if (mSeed_AssetClassification.Length > 0)
			{
				ImportTargetClassTextBox.Text = mSeed_AssetClassification;
			}

			// Seed the asset name's label?
			if (mSeed_AssetLabel.Length > 0)
			{
				ImportAssetNameTextBox.Text = mSeed_AssetLabel;
			}
			else if (cbAssetExtensions.Checked)
			{
				ImportAssetNameTextBox.Text = DosUtils.PathGetFileName(mSourceFullFilename);
			}
			else
			{
				ImportAssetNameTextBox.Text = DosUtils.PathGetFileNameWithoutExtension(mSourceFullFilename);
			}

			// Seed the asset's platform?
			if (mSeed_AssetPlatform.Length > 0)
			{
				ImportValidPlatformsComboBox.Text = mSeed_AssetPlatform;
			}

			// Check if we still have nothing selected yet?
			// Check if this was the only one?
			if (ImportMogRepositoryTreeView.SelectedNode == null &&
				ImportPotentialAssetsListView.Items.Count == 1)
			{
				// Automatically select the first and only item
				ImportPotentialAssetsListView.Items[0].Selected = true;
			}

			// Check if we have nothing selected yet?
			if (ImportMogRepositoryTreeView.SelectedNode == null)
			{
				// We must have a valid gameData controller to do this smart lookup
				string workspaceDirectory = MOG_ControllerProject.GetWorkspaceDirectory();
				if (workspaceDirectory.Length > 0)
				{
					// Check for better match from source path of import file
					// Check if this source file is within our workspace?
					string fullPath = Path.GetDirectoryName(this.mSourceFullFilename);
					if (DosUtils.PathIsWithinPath(workspaceDirectory, fullPath))
					{
						// Attempt to match the relative path of the source file to our classification tree
						string relativePath = DosUtils.PathMakeRelativePath(workspaceDirectory, fullPath);
						string classification = MOG_ControllerProject.GetProjectName() + "~" + relativePath.Replace("\\", "~");

						// Check to see if we get a valid class from this path
						if (MOG_ControllerProject.IsValidClassification(classification))
						{
							ImportTargetClassTextBox.Text = classification;
						}
					}
				}
			}

			// Get the previously selected item
			string savedClass = MogUtils_Settings.MogUtils_Settings.LoadSetting("ImportWizard", "LastClass");
			// Check if we still have nothing selected yet?
			// Check if we have a previously selected item?
			if (ImportMogRepositoryTreeView.SelectedNode == null &&
				savedClass.Length > 0)
			{
				ImportTargetClassTextBox.Text = savedClass;
			}
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportAssetTreeForm));
			this.ImportAssetNameTextBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.ImportValidPlatformsComboBox = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.ImportOkButton = new System.Windows.Forms.Button();
			this.ImportCancelButton = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.ImportAdvancedButton = new System.Windows.Forms.Button();
			this.ImportOkAllButton = new System.Windows.Forms.Button();
			this.ImportTargetClassTextBox = new System.Windows.Forms.TextBox();
			this.ImportTargetDestinationTextBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.TargetLabel = new System.Windows.Forms.Label();
			this.TreesPanel = new System.Windows.Forms.Panel();
			this.GameDataClassTreesPanel = new System.Windows.Forms.Panel();
			this.NoWorkspaceAvailableLabel = new System.Windows.Forms.Label();
			this.GameDataDestinationTreeView = new MOG_ControlsLibrary.Controls.MogControl_GameDataDestinationTreeView();
			this.PackageTreeView = new MOG_ControlsLibrary.Controls.MogControl_PackageManagementTreeView();
			this.TreesSplitter = new System.Windows.Forms.Splitter();
			this.ImportMogRepositoryTreeView = new MOG_ControlsLibrary.Controls.MogControl_ImportTreeView();
			this.FoundAssetsSplitter = new System.Windows.Forms.Splitter();
			this.ImportPotentialAssetsListView = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.MOGToolTip = new System.Windows.Forms.ToolTip(this.components);
			this.AdvancedTreesLabel = new System.Windows.Forms.Label();
			this.cbAssetExtensions = new System.Windows.Forms.CheckBox();
			this.TreesPanel.SuspendLayout();
			this.GameDataClassTreesPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// ImportAssetNameTextBox
			// 
			this.ImportAssetNameTextBox.Location = new System.Drawing.Point(8, 32);
			this.ImportAssetNameTextBox.Name = "ImportAssetNameTextBox";
			this.ImportAssetNameTextBox.Size = new System.Drawing.Size(291, 20);
			this.ImportAssetNameTextBox.TabIndex = 0;
			this.ImportAssetNameTextBox.Text = "Default Asset Name";
			this.ImportAssetNameTextBox.TextChanged += new System.EventHandler(this.ImportAssetNameTextBox_TextChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(192, 16);
			this.label1.TabIndex = 1;
			this.label1.Text = "Asset Name";
			// 
			// ImportValidPlatformsComboBox
			// 
			this.ImportValidPlatformsComboBox.Location = new System.Drawing.Point(406, 6);
			this.ImportValidPlatformsComboBox.Name = "ImportValidPlatformsComboBox";
			this.ImportValidPlatformsComboBox.Size = new System.Drawing.Size(96, 21);
			this.ImportValidPlatformsComboBox.TabIndex = 2;
			this.ImportValidPlatformsComboBox.Text = "All";
			this.ImportValidPlatformsComboBox.SelectedIndexChanged += new System.EventHandler(this.ImportValidPlatformsComboBox_SelectedIndexChanged);
			this.ImportValidPlatformsComboBox.TextChanged += new System.EventHandler(this.ImportValidPlatformsComboBox_TextChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(318, 9);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(88, 16);
			this.label2.TabIndex = 3;
			this.label2.Text = "Target Platform";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// ImportOkButton
			// 
			this.ImportOkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.ImportOkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.ImportOkButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.ImportOkButton.Location = new System.Drawing.Point(224, 375);
			this.ImportOkButton.Name = "ImportOkButton";
			this.ImportOkButton.Size = new System.Drawing.Size(75, 23);
			this.ImportOkButton.TabIndex = 6;
			this.ImportOkButton.Text = "Ok";
			// 
			// ImportCancelButton
			// 
			this.ImportCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.ImportCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.ImportCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.ImportCancelButton.Location = new System.Drawing.Point(64, 375);
			this.ImportCancelButton.Name = "ImportCancelButton";
			this.ImportCancelButton.Size = new System.Drawing.Size(75, 23);
			this.ImportCancelButton.TabIndex = 8;
			this.ImportCancelButton.Text = "Cancel";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(16, 64);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(280, 23);
			this.label4.TabIndex = 13;
			this.label4.Text = "Browse target classification:";
			// 
			// ImportAdvancedButton
			// 
			this.ImportAdvancedButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.ImportAdvancedButton.Location = new System.Drawing.Point(221, 3);
			this.ImportAdvancedButton.Name = "ImportAdvancedButton";
			this.ImportAdvancedButton.Size = new System.Drawing.Size(80, 24);
			this.ImportAdvancedButton.TabIndex = 1;
			this.ImportAdvancedButton.Text = "Advanced >>";
			this.ImportAdvancedButton.Click += new System.EventHandler(this.ImportAdvancedButton_Click);
			// 
			// ImportOkAllButton
			// 
			this.ImportOkAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.ImportOkAllButton.DialogResult = System.Windows.Forms.DialogResult.Yes;
			this.ImportOkAllButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.ImportOkAllButton.Location = new System.Drawing.Point(144, 375);
			this.ImportOkAllButton.Name = "ImportOkAllButton";
			this.ImportOkAllButton.Size = new System.Drawing.Size(75, 23);
			this.ImportOkAllButton.TabIndex = 7;
			this.ImportOkAllButton.Text = "Ok To All";
			// 
			// ImportTargetClassTextBox
			// 
			this.ImportTargetClassTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.ImportTargetClassTextBox.Location = new System.Drawing.Point(16, 295);
			this.ImportTargetClassTextBox.Name = "ImportTargetClassTextBox";
			this.ImportTargetClassTextBox.Size = new System.Drawing.Size(272, 20);
			this.ImportTargetClassTextBox.TabIndex = 17;
			this.ImportTargetClassTextBox.BackColor = Color.Tomato;
			this.ImportTargetClassTextBox.TabStop = false;
			this.ImportTargetClassTextBox.TextChanged += new System.EventHandler(this.ImportTargetClassTextBox_TextChanged);
			// 
			// ImportTargetDestinationTextBox
			// 
			this.ImportTargetDestinationTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.ImportTargetDestinationTextBox.Location = new System.Drawing.Point(16, 335);
			this.ImportTargetDestinationTextBox.Name = "ImportTargetDestinationTextBox";
			this.ImportTargetDestinationTextBox.Size = new System.Drawing.Size(272, 20);
			this.ImportTargetDestinationTextBox.TabIndex = 5;
			this.ImportTargetDestinationTextBox.BackColor = Color.LightGray;
			this.ImportTargetDestinationTextBox.Enabled = false;
			this.ImportTargetDestinationTextBox.Text = "n/a";
			this.ImportTargetDestinationTextBox.TextChanged += new System.EventHandler(this.ImportTargetDestinationTextBox_TextChanged);
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label3.Location = new System.Drawing.Point(16, 279);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(288, 16);
			this.label3.TabIndex = 19;
			this.label3.Text = "Target Classification:";
			// 
			// TargetLabel
			// 
			this.TargetLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.TargetLabel.Location = new System.Drawing.Point(16, 319);
			this.TargetLabel.Name = "TargetLabel";
			this.TargetLabel.Size = new System.Drawing.Size(288, 16);
			this.TargetLabel.TabIndex = 20;
			this.TargetLabel.Text = "Target Destination:";
			// 
			// TreesPanel
			// 
			this.TreesPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.TreesPanel.Controls.Add(this.GameDataClassTreesPanel);
			this.TreesPanel.Controls.Add(this.TreesSplitter);
			this.TreesPanel.Controls.Add(this.ImportMogRepositoryTreeView);
			this.TreesPanel.Controls.Add(this.FoundAssetsSplitter);
			this.TreesPanel.Controls.Add(this.ImportPotentialAssetsListView);
			this.TreesPanel.Location = new System.Drawing.Point(8, 88);
			this.TreesPanel.Name = "TreesPanel";
			this.TreesPanel.Size = new System.Drawing.Size(296, 183);
			this.TreesPanel.TabIndex = 21;
			// 
			// GameDataClassTreesPanel
			// 
			this.GameDataClassTreesPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.GameDataClassTreesPanel.Controls.Add(this.NoWorkspaceAvailableLabel);
			this.GameDataClassTreesPanel.Controls.Add(this.GameDataDestinationTreeView);
			this.GameDataClassTreesPanel.Controls.Add(this.PackageTreeView);
			this.GameDataClassTreesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.GameDataClassTreesPanel.Location = new System.Drawing.Point(296, 0);
			this.GameDataClassTreesPanel.Name = "GameDataClassTreesPanel";
			this.GameDataClassTreesPanel.Size = new System.Drawing.Size(0, 165);
			this.GameDataClassTreesPanel.TabIndex = 17;
			// 
			// NoWorkspaceAvailableLabel
			// 
			this.NoWorkspaceAvailableLabel.Location = new System.Drawing.Point(0, 4);
			this.NoWorkspaceAvailableLabel.Name = "NoWorkspaceAvailableLabel";
			this.NoWorkspaceAvailableLabel.Size = new System.Drawing.Size(245, 73);
			this.NoWorkspaceAvailableLabel.TabIndex = 17;
			this.NoWorkspaceAvailableLabel.Text = "";
			// 
			// GameDataDestinationTreeView
			// 
			this.GameDataDestinationTreeView.Location = new System.Drawing.Point(107, 92);
			this.GameDataDestinationTreeView.MOGSelectedNode = null;
			this.GameDataDestinationTreeView.MOGShowFiles = false;
			this.GameDataDestinationTreeView.Name = "GameDataDestinationTreeView";
			this.GameDataDestinationTreeView.Size = new System.Drawing.Size(137, 66);
			this.GameDataDestinationTreeView.TabIndex = 16;
			this.MOGToolTip.SetToolTip(this.GameDataDestinationTreeView, "Select a target directory for this asset when copied to your local workspace");
			this.GameDataDestinationTreeView.Visible = false;
			this.GameDataDestinationTreeView.AfterTargetSelect += new MOG_ControlsLibrary.Controls.MogControl_GameDataDestinationTreeView.TreeViewEvent(this.GameDataDestinationTreeView_AfterTargetSelect);
			// 
			// PackageTreeView
			// 
			this.PackageTreeView.ExpandAssets = false;
			this.PackageTreeView.Location = new System.Drawing.Point(3, 92);
			this.PackageTreeView.Name = "PackageTreeView";
			this.PackageTreeView.Size = new System.Drawing.Size(98, 66);
			this.PackageTreeView.TabIndex = 18;
			this.PackageTreeView.UsePlatformSpecificCheckBox = false;
			this.PackageTreeView.Visible = false;
			this.PackageTreeView.AfterPackageSelect += new System.Windows.Forms.TreeViewEventHandler(this.PackageTreeView_AfterPackageSelect);
			// 
			// TreesSplitter
			// 
			this.TreesSplitter.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.TreesSplitter.Location = new System.Drawing.Point(288, 0);
			this.TreesSplitter.MinExtra = 0;
			this.TreesSplitter.MinSize = 0;
			this.TreesSplitter.Name = "TreesSplitter";
			this.TreesSplitter.Size = new System.Drawing.Size(8, 165);
			this.TreesSplitter.TabIndex = 18;
			this.TreesSplitter.TabStop = false;
			// 
			// ImportMogRepositoryTreeView
			// 
			this.ImportMogRepositoryTreeView.ArchivedNodeForeColor = System.Drawing.SystemColors.WindowText;
			this.ImportMogRepositoryTreeView.Dock = System.Windows.Forms.DockStyle.Left;
			this.ImportMogRepositoryTreeView.ExclusionList = MOG_Filename.GetProjectLibraryClassificationString();
			this.ImportMogRepositoryTreeView.ExpandAssets = false;
			this.ImportMogRepositoryTreeView.ExpandPackageGroupAssets = true;
			this.ImportMogRepositoryTreeView.ExpandPackageGroups = false;
			this.ImportMogRepositoryTreeView.FocusForAssetNodes = MOG_ControlsLibrary.Controls.LeafFocusLevel.RepositoryItems;
			this.ImportMogRepositoryTreeView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ImportMogRepositoryTreeView.HotTracking = true;
			this.ImportMogRepositoryTreeView.ImageIndex = 0;
			this.ImportMogRepositoryTreeView.LastNodePath = null;
			this.ImportMogRepositoryTreeView.Location = new System.Drawing.Point(0, 0);
			this.ImportMogRepositoryTreeView.Name = "ImportMogRepositoryTreeView";
			this.ImportMogRepositoryTreeView.NodesDefaultToChecked = false;
			this.ImportMogRepositoryTreeView.PathSeparator = "~";
			this.ImportMogRepositoryTreeView.PersistantHighlightSelectedNode = false;
			this.ImportMogRepositoryTreeView.SelectedImageIndex = 0;
			this.ImportMogRepositoryTreeView.ShowAssets = false;
			this.ImportMogRepositoryTreeView.ShowDescription = false;
			this.ImportMogRepositoryTreeView.ShowPlatformSpecific = false;
			this.ImportMogRepositoryTreeView.ShowToolTips = false;
			this.ImportMogRepositoryTreeView.Size = new System.Drawing.Size(288, 165);
			this.ImportMogRepositoryTreeView.TabIndex = 3;
			this.ImportMogRepositoryTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ImportMogRepositoryTreeView_AfterSelect);
			// 
			// FoundAssetsSplitter
			// 
			this.FoundAssetsSplitter.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.FoundAssetsSplitter.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.FoundAssetsSplitter.Location = new System.Drawing.Point(0, 165);
			this.FoundAssetsSplitter.MinExtra = 0;
			this.FoundAssetsSplitter.MinSize = 0;
			this.FoundAssetsSplitter.Name = "FoundAssetsSplitter";
			this.FoundAssetsSplitter.Size = new System.Drawing.Size(296, 10);
			this.FoundAssetsSplitter.TabIndex = 23;
			this.FoundAssetsSplitter.TabStop = false;
			// 
			// ImportPotentialAssetsListView
			// 
			this.ImportPotentialAssetsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
			this.ImportPotentialAssetsListView.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.ImportPotentialAssetsListView.Enabled = false;
			this.ImportPotentialAssetsListView.FullRowSelect = true;
			this.ImportPotentialAssetsListView.HideSelection = false;
			this.ImportPotentialAssetsListView.Location = new System.Drawing.Point(0, 175);
			this.ImportPotentialAssetsListView.Name = "ImportPotentialAssetsListView";
			this.ImportPotentialAssetsListView.Size = new System.Drawing.Size(296, 8);
			this.ImportPotentialAssetsListView.TabIndex = 22;
			this.ImportPotentialAssetsListView.UseCompatibleStateImageBehavior = false;
			this.ImportPotentialAssetsListView.View = System.Windows.Forms.View.Details;
			this.ImportPotentialAssetsListView.SelectedIndexChanged += new System.EventHandler(this.ImportPotentialAssetsListView_SelectedIndexChanged);
			this.ImportPotentialAssetsListView.DoubleClick += new System.EventHandler(this.ImportPotentialAssetsListView_DoubleClick);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "AssetName";
			this.columnHeader1.Width = 351;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Version";
			this.columnHeader2.Width = 142;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Owner";
			// 
			// AdvancedTreesLabel
			// 
			this.AdvancedTreesLabel.Location = new System.Drawing.Point(312, 64);
			this.AdvancedTreesLabel.Name = "AdvancedTreesLabel";
			this.AdvancedTreesLabel.Size = new System.Drawing.Size(256, 23);
			this.AdvancedTreesLabel.TabIndex = 22;
			this.AdvancedTreesLabel.Text = "Browse ::";
			// 
			// cbAssetExtensions
			// 
			this.cbAssetExtensions.AutoSize = true;
			this.cbAssetExtensions.Location = new System.Drawing.Point(315, 35);
			this.cbAssetExtensions.Name = "cbAssetExtensions";
			this.cbAssetExtensions.Size = new System.Drawing.Size(187, 17);
			this.cbAssetExtensions.TabIndex = 23;
			this.cbAssetExtensions.Text = "Use file extensions in asset names";
			this.cbAssetExtensions.UseVisualStyleBackColor = true;
			this.cbAssetExtensions.CheckedChanged += new System.EventHandler(this.cbAssetExtensions_CheckedChanged);
			// 
			// ImportAssetTreeForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(312, 405);
			this.Controls.Add(this.cbAssetExtensions);
			this.Controls.Add(this.AdvancedTreesLabel);
			this.Controls.Add(this.TreesPanel);
			this.Controls.Add(this.TargetLabel);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.ImportTargetDestinationTextBox);
			this.Controls.Add(this.ImportTargetClassTextBox);
			this.Controls.Add(this.ImportAssetNameTextBox);
			this.Controls.Add(this.ImportOkAllButton);
			this.Controls.Add(this.ImportAdvancedButton);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.ImportCancelButton);
			this.Controls.Add(this.ImportOkButton);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.ImportValidPlatformsComboBox);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(320, 431);
			this.Name = "ImportAssetTreeForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Import";
			this.Shown += new System.EventHandler(this.ImportAssetTreeForm_Shown);
			this.Closing += new System.ComponentModel.CancelEventHandler(this.ImportAssetTreeForm_Closing);
			this.TreesPanel.ResumeLayout(false);
			this.GameDataClassTreesPanel.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// Makes sure that our LastSelectedPackage property is current
		/// </summary>
		private void PackageTreeView_AfterSelect(object sender, TreeViewEventArgs e)
		{
		}

		private void PackageTreeView_AfterPackageSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			MOG_Filename package = new MOG_Filename(this.PackageTreeView.SelectedPackageFullFilename);
			// If we are dealing with an asset...
			if (package.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
			{
				Mog_BaseTag tag = e.Node.Tag as Mog_BaseTag;
				if (tag != null)
				{
					Mog_BaseTag packageAssignment = e.Node.Tag as Mog_BaseTag;
					this.ImportTargetDestinationTextBox.Text = ((Mog_BaseTag)e.Node.Tag).PackageFullName;
				}
			}
		}

		private void GameDataDestinationTreeView_AfterTargetSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			this.ImportTargetDestinationTextBox.Text = this.GameDataDestinationTreeView.MOGGameDataTarget;
		}

		/// <summary>
		/// Display advanced options to the user
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ImportAdvancedButton_Click(object sender, System.EventArgs e)
		{
			ToggleAdvancedMode();
		}

		private void ToggleAdvancedMode()
		{
			// Only open the FoundAssets splitter if we have potential matches
			if (mPotentialMatches != null && mPotentialMatches.Count > 0 && mFoundAssets.Opened == false)
			{
				this.FoundAssetsSplitter.SplitPosition = mFoundAssets.ToggleWidth();
			}

			// If we already bigger than the minimims, then we are open
			if (Width > this.MinimumSize.Width && Height > this.MinimumSize.Height)
			{
				mAdvancedOpened = true;
			}

			if (!mAdvancedOpened)
			{
				this.Width = Width + 300;
				this.Height = Height + 200;
				mAdvancedOpened = true;
				this.ImportAdvancedButton.Text = "Advanced <<";
			}
			else
			{
				this.Width = 0;
				this.Height = 0;
				mAdvancedOpened = false;
				this.ImportAdvancedButton.Text = "Advanced >>";
			}
		}

		private void UpdateDefaultAssetNameIncludeExtension()
		{
			// Respect the DefaultAssetNameIncludeExtension property of this classification
			switch (mProperties.DefaultAssetNameIncludeExtension)
			{
				case MOG_DefaultPrompt.No:
				case MOG_DefaultPrompt.PromptDefaultNo:
					cbAssetExtensions.Checked = false;
					break;
				case MOG_DefaultPrompt.Yes:
				case MOG_DefaultPrompt.PromptDefaultYes:
					cbAssetExtensions.Checked = true;
					break;
			}
		}

		private void UpdateDefaultAssetNamePlatform()
		{
			// Respect the DefaultAssetNamePlatform property of this classification
			if (mProperties.DefaultAssetNamePlatform.Length > 0)
			{
				ImportValidPlatformsComboBox.Text = mProperties.DefaultAssetNamePlatform;
			}
		}

		private void UpdatePotentialAssetList()
		{
			// Make sure we have a fully quantified asset name?
			if (mFinalAssetName.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
			{
				// Check if we already have something selected?
				if (ImportPotentialAssetsListView.SelectedItems != null)
				{
					// Clear whatever is currently selected
					ImportPotentialAssetsListView.SelectedItems.Clear();
				}

				// Check if the potential assets has any populated nodes??
				if (ImportPotentialAssetsListView.Items.Count > 0)
				{
					// See if we match any of the assets listed in the potential matches?
					foreach (ListViewItem item in ImportPotentialAssetsListView.Items)
					{
						// Check if this is an exact match?
						if (string.Compare(item.Text, mFinalAssetName.GetAssetFullName(), true) == 0)
						{
							item.Selected = true;
						}
						else
						{
							item.Selected = false;
						}
					}
				}
			}
		}

		private void UpdateAssetTargetFromInheritedProperties()
		{
			string newTarget = "";

			// Check if this has been determined to be a packaged asset?
			if (mProperties.IsPackagedAsset)
			{
				// Add all the default packages for this object
				foreach (MOG_Property property in mProperties.GetPackages())
				{
					// Check if we already have a target?
					if (newTarget.Length > 0)
					{
						// Add in a simple ',' delimitor
						newTarget = newTarget + ", ";
					}

					newTarget = newTarget + property.mPropertyKey;
				}
			}
			else if (mProperties.SyncFiles)
			{
				// Add our new syncTarget using the relative path
				string relativeSyncTargetPath = MOG_Tokens.GetFormattedString(mProperties.SyncTargetPath, mFinalAssetName, mProperties.GetPropertyList());
				if (relativeSyncTargetPath.Length > 0)
				{
					string fullSyncTargetPath = Path.Combine(GameDataDestinationTreeView.MOGRootNode.Text, relativeSyncTargetPath);
					newTarget = fullSyncTargetPath;
				}
			}
			else
			{
				newTarget = "n/a";
			}

			this.ImportTargetDestinationTextBox.Text = newTarget;
		}

		private void ImportMogRepositoryTreeView_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			// Check if we have a platform specified?
			if (ImportMogRepositoryTreeView.SelectedNode != null)
			{
				string classification = ImportMogRepositoryTreeView.SelectedNode.FullPath;
				this.ImportTargetClassTextBox.Text = classification;
			}
			else
			{
				this.ImportTargetClassTextBox.Text = "";
			}
		}

		private void ImportValidPlatformsComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
		}

		private void ImportValidPlatformsComboBox_TextChanged(object sender, System.EventArgs e)
		{
			// Check if this is blank?  or
			// Check if this is an invalid platform?  or
			// Check if this contains any invalid characters?
			if (ImportValidPlatformsComboBox.Text.Length == 0 ||
				!MOG_ControllerProject.IsValidPlatform(ImportValidPlatformsComboBox.Text) || 
				MOG_ControllerSystem.InvalidMOGCharactersCheck(ImportValidPlatformsComboBox.Text, true))
			{
				ImportValidPlatformsComboBox.BackColor = Color.Tomato;
			}
			else
			{
				ImportValidPlatformsComboBox.BackColor = Color.PaleGreen;

				UpdateFinalAssetName();

				// Re-initialize the GameDestinationTreeView to reflect this platform
				GameDataDestinationTreeView.ReinitializeVirtual(ImportValidPlatformsComboBox.Text);
			}
		}

		private void ImportAssetNameTextBox_TextChanged(object sender, System.EventArgs e)
		{
			// Check if this is blank?  or
			// Check if this contains any invalid characters?
			if (ImportAssetNameTextBox.Text.Length == 0 ||
				MOG_ControllerSystem.InvalidMOGCharactersCheck(ImportAssetNameTextBox.Text, true))
			{
				ImportAssetNameTextBox.BackColor = Color.Tomato;
			}
			else
			{
				ImportAssetNameTextBox.BackColor = Color.PaleGreen;
			}

			UpdateFinalAssetName();
		}

		private void ImportPotentialAssetsListView_DoubleClick(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		private void ImportPotentialAssetsListView_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			// See if we can populate our trees with this potential match
			foreach (ListViewItem item in ImportPotentialAssetsListView.SelectedItems)
			{
				MOG_Filename asset = new MOG_Filename(item.SubItems[3].Text);

				this.ImportTargetClassTextBox.Text = asset.GetAssetClassification();
				this.ImportValidPlatformsComboBox.Text = asset.GetAssetPlatform();
				this.ImportAssetNameTextBox.Text = asset.GetAssetLabel();
			}
		}

		private TreeNode SelectClassificationNode(string Classification)
		{
			return MogUtil_ClassificationTrees.FindAndExpandTreeNodeFromFullPath(this.ImportMogRepositoryTreeView.Nodes, "~", Classification);
		}

		private void ImportTargetClassTextBox_TextChanged(object sender, System.EventArgs e)
		{
			// Check if this is blank?  or
			// Check if this contains any invalid characters?
			if (ImportTargetClassTextBox.Text.Length == 0 ||
				MOG_ControllerSystem.InvalidWindowsPathCharactersCheck(ImportTargetClassTextBox.Text, true))
			{
				ImportTargetClassTextBox.BackColor = Color.Tomato;
			}
			else
			{
				ImportTargetClassTextBox.BackColor = Color.PaleGreen;

				MogUtils_Settings.MogUtils_Settings.SaveSetting("ImportWizard", "LastClass", ImportTargetClassTextBox.Text);

				// Update the asset name based on this classification's properties
				UpdateRepositoryTree();
				UpdateFinalAssetName();
				UpdateAssetPropertiesIncludingSeeds();
				UpdateAssetTargetTree();
				UpdateDefaultAssetNameIncludeExtension();
				UpdateDefaultAssetNamePlatform();
				UpdatePotentialAssetList();
				UpdateAssetTargetFromInheritedProperties();
			}
		}

		private void UpdateRepositoryTree()
		{
			// Check if we have nothing selected?  or
			// Check if we have a mismatch?
			if (ImportMogRepositoryTreeView.SelectedNode == null ||
				string.Compare(ImportTargetClassTextBox.Text, ImportMogRepositoryTreeView.SelectedNode.FullPath, true) != 0)
			{
				ImportMogRepositoryTreeView.SelectedNode = ImportMogRepositoryTreeView.DrillToNodePath(ImportTargetClassTextBox.Text);
			}
		}

		private void ImportTargetDestinationTextBox_TextChanged(object sender, System.EventArgs e)
		{
			// Check if this is a package asset?
			if (mProperties.IsPackagedAsset)
			{
				ImportTargetDestinationTextBox.Enabled = true;

				// Check if this is blank?  or
				// Check if this contains any invalid characters?
				if (ImportTargetDestinationTextBox.Text.Length == 0)
				{
					ImportTargetDestinationTextBox.BackColor = Color.Tomato;
				}
				else
				{
					MogUtils_Settings.MogUtils_Settings.SaveSetting("ImportWizard", "LastPackage", this.ImportTargetDestinationTextBox.Text);
					ImportTargetDestinationTextBox.BackColor = Color.PaleGreen;
				}
			}
			// Check if this syncs files?
			else if (mProperties.SyncFiles)
			{
				ImportTargetDestinationTextBox.Enabled = true;

				// Check if this is blank?  or
				// Check if this contains any invalid characters?
				if (ImportTargetDestinationTextBox.Text.Length == 0 ||
					MOG_ControllerSystem.InvalidWindowsPathCharactersCheck(ImportTargetDestinationTextBox.Text, true))
				{
					ImportTargetDestinationTextBox.BackColor = Color.Tomato;
				}
				else
				{
					ImportTargetDestinationTextBox.BackColor = Color.PaleGreen;
					MogUtils_Settings.MogUtils_Settings.SaveSetting("ImportWizard", "LastDir", this.ImportTargetDestinationTextBox.Text);
				}
			}
			else
			{
				ImportTargetDestinationTextBox.Enabled = false;
				ImportTargetDestinationTextBox.BackColor = Color.LightGray;
				ImportTargetDestinationTextBox.Text = "n/a";
			}

			// Make sure the tree also matches
			UpdateAssetTargetTree();
		}

		#region Closing Methods
		private void ImportAssetTreeForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// If our user clicked OK or Yes...
			if (DialogResult == DialogResult.OK || DialogResult == DialogResult.Yes)
			{
				ValidateImportForClose(e);
			}
		}

		private void ValidateImportForClose(System.ComponentModel.CancelEventArgs e)
		{
			// Clear our property array
			mMOGPropertyArray.Clear();

			// Make sure we have a valid classification?
			if (ImportTargetClassTextBox.Text.Length > 0)
			{
				// Make sure our platform combo box isn't in an error state?
				if (ImportValidPlatformsComboBox.BackColor != Color.Tomato)
				{
					// Check if we are missing a target?
					if (ImportTargetDestinationTextBox.Enabled == false)
					{
						// No target needed
					}
					else if (ImportTargetDestinationTextBox.Text.Length > 0)
					{
						// Check if we are a package asset?
						if (mProperties.IsPackagedAsset)
						{
							// Break up the package assignment
							string packageName = MOG_ControllerPackage.GetPackageName(ImportTargetDestinationTextBox.Text);
							string packageGroup = MOG_ControllerPackage.GetPackageGroups(ImportTargetDestinationTextBox.Text);
							string packageObject = MOG_ControllerPackage.GetPackageObjects(ImportTargetDestinationTextBox.Text);
							// Create the package assignment property
							mMOGPropertyArray.Add(MOG.MOG_PropertyFactory.MOG_Relationships.New_PackageAssignment(packageName, packageGroup, packageObject));
						}
						// Check if we are a syncing asset?
						else if (mProperties.SyncFiles)
						{
							// Get the inheritedSyncTarget
							string inheritedSyncTargetPath = MOG_Tokens.GetFormattedString(mProperties.SyncTargetPath, mFinalAssetName, mProperties.GetPropertyList());
							// Get a relativeTargetPath from what is currently selected
							string relativeTargetPath = DosUtils.PathMakeRelativePath(GameDataDestinationTreeView.MOGRootNode.Text, ImportTargetDestinationTextBox.Text);
							// Check if this is different than our formatted inheritedSyncTarget?
							if (string.Compare(relativeTargetPath, inheritedSyncTargetPath, true) != 0)
							{
								// Create our unique SyncTargetPath property
								mMOGPropertyArray.Add(MOG.MOG_PropertyFactory.MOG_Sync_OptionsProperties.New_SyncTargetPath(relativeTargetPath));
							}
						}
					}
					else
					{
						if (mProperties.IsPackagedAsset)
						{
							MOG_Prompt.PromptMessage("Missing Target Package", "This asset was classified as a packaged asset yet it is missing a target package assignment.  Please specify a target package for this asset.");
						}
						else if (mProperties.SyncFiles)
						{
							MOG_Prompt.PromptMessage("Missing SyncTargetPath", "This asset is missing a valid sync target location.  Please specify where in the project this asset should be copied when synced.");
						}

						if (!mAdvancedOpened)
						{
							// Force open the advanced mode because we are missing a target
							ToggleAdvancedMode();
						}

						e.Cancel = true;
					}
				}
				else
				{
					MOG_Prompt.PromptMessage("Missing Valid Platform", "This asset was not identified with a valid platform.  Please specify a proper platform scope for this asset.");

					if (!mAdvancedOpened)
					{
						// Force open the advanced mode because we are missing a target
						ToggleAdvancedMode();
					}

					e.Cancel = true;
				}
			}
			else
			{
				MOG_Prompt.PromptMessage("Missing classification", "All assets must be classified when they are imported.  Please specify a classification for this asset.");
				e.Cancel = true;
			}
		}

		#endregion Closing Methods

		private void cbAssetExtensions_CheckedChanged(object sender, EventArgs e)
		{
			if (cbAssetExtensions.Checked)
			{
				ImportAssetNameTextBox.Text = DosUtils.PathGetFileName(mSourceFullFilename);
			}
			else
			{
				ImportAssetNameTextBox.Text = DosUtils.PathGetFileNameWithoutExtension(mSourceFullFilename);
			}

			UpdateFinalAssetName();
		}

		private void UpdateAssetPropertiesIncludingSeeds()
		{
			// Create a blank properties that we can modify
			mProperties = new MOG_Properties(new ArrayList());

			// Get the current properties for the asset
			MOG_Properties AssetProperties = null;
			// Determin if we should get the properties of th e asset or the classification?
			if (mFinalAssetName.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset &&
				MOG_ControllerProject.DoesAssetExists(mFinalAssetName))
			{
				// Get the properties of this asset
				AssetProperties = new MOG_Properties(mFinalAssetName);
			}
			else if (ImportTargetClassTextBox.Text.Length > 0)
			{
				// Get the properties of this asset
				AssetProperties = new MOG_Properties(ImportTargetClassTextBox.Text);
			}

			// Check if we obtained any asset or classification properties?
			if (AssetProperties != null)
			{
				// Now take the properties in for this Asset and push thin into our properties
				mProperties.SetProperties(AssetProperties.GetPropertyList());
			}

			// Check if we have any seeded properties?
			if (mSeed_AssetProperties.Count > 0)
			{
				// Make sure we add any seeded properties as well
				mProperties.SetProperties(mSeed_AssetProperties);
			}
		}

		private void ImportAssetTreeForm_Shown(object sender, EventArgs e)
		{
			bool bIsVisible = false;

			// Scan all of the screens
			foreach (Screen screen in Screen.AllScreens)
			{
				// Check if we are visible?
				if (screen.Bounds.IntersectsWith(this.Bounds))
				{
					bIsVisible = true;
				}
			}

			// Check if we were not visible?
			if (!bIsVisible)
			{
				// Force us to the middle of the primary screen
				Point center = new Point();
				center.X = ((Screen.PrimaryScreen.Bounds.Right - Screen.PrimaryScreen.Bounds.Left) / 2) - this.Size.Width / 2;
				center.Y = ((Screen.PrimaryScreen.Bounds.Bottom - Screen.PrimaryScreen.Bounds.Top) /2) - this.Size.Height / 2;
				this.Location = center;
			}

			// Finish initializing this form's data (this initiates a background thread!)
			PopulateTreeViewData();

			// Check if we are missing a parent window?
			if (this.Parent == null)
			{
				// This dialog should be pushed to topmost when it doesn't have a parent or else it can ger lost behind other apps.
				// You would think this is simple but for some reason MOG has really struggled with these dialogs being kept on top...
				// We have tried it all and finally ended up with this...toggling the TopMost mode seems to be working 100% of the time.
				this.TopMost = true;
				this.TopMost = false;
				this.TopMost = true;
			}
		}
	}
}
