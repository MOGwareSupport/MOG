using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;

// Sort manager
using EV.Windows.Forms;

using MOG_Client.Client_Mog_Utilities;
using MOG_Client.Client_Mog_Utilities.AssetOptions;
using MOG_ControlsLibrary.Utils;
using MOG_ControlsLibrary.Common.MOG_ContextMenu;
using MOG_ControlsLibrary.Common;

using MOG;
using MOG.DOSUTILS;
using MOG.PROPERTIES;
using MOG.INI;
using MOG.TIME;
using MOG.FILENAME;
using MOG.DATABASE;
using MOG.CONTROLLER.CONTROLLERREPOSITORY;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.PROMPT;
using MOG.REPORT;
using MOG.PROGRESS;
using System.Collections.Generic;
using MOG_ControlsLibrary.MogUtils_Settings;
using MOG_ControlsLibrary.Controls;
using MOG_ControlsLibrary.Utilities;
using MOG_ControlsLibrary;



namespace MOG_Client
{
	/// <summary>
	/// Summary description for SyncLatestForm.
	/// </summary>
	public class SyncLatestForm : Form
	{
		private bool bHaltEvents;
		private class SyncAssetData
		{
			public Color color;
		}

		private class SyncAssetFilename : SyncAssetData
		{
			public MOG_Filename filename;
			public string action;
		}

		private class SyncAssetMessage : SyncAssetData
		{
			public string message;
		}

		private string mExclusions = "";
		public string Exclusions
		{
			get { return mExclusions; }
		}

		private string mInclusions = "";
		public string Inclusions
		{
			get { return mInclusions; }
		}

		private string UserSpecifiedSyncTag
		{
			get { return TagsComboBox.Enabled ? TagsComboBox.Text : ""; }
		}

		public string SyncTag
		{
			get { return (UserSpecifiedSyncTag.Length != 0) ? UserSpecifiedSyncTag : MOG_ControllerProject.GetBranchName(); }
		}

		public const string Nothing_Returned_Text = "No SyncRootNodes Found";
		public enum PostedAssetsColumns { NAME, DATE, ACTION, FULLNAME };

		private List<SyncAssetData> mListViewData = new List<SyncAssetData>();
		private BackgroundWorker mPopulateWorker;
		public MOG_LocalSyncInfo mLocalSyncInfo = null;
		private bool mLoading = false;
		
		private string mDefaultFilter;
		private string mDefaultTag;
		private MogMainForm mainForm;

		private System.Windows.Forms.Label AvailableBuildLabel;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.Button UpdateBuildOkButton;
		private System.Windows.Forms.Button UpdateBuildCancelButton;

		private ListViewSortManager mSortManager;
		private System.Windows.Forms.ImageList UpdateBuildImageList;
		private System.Windows.Forms.Panel TopPanel;
		private System.Windows.Forms.Panel BottomPanel;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label BuildName;
		private System.Windows.Forms.TreeView SyncRootTree;
		public System.Windows.Forms.CheckBox UpdateBuildCheckMissingCheckBox;
		private System.Windows.Forms.Panel UpdateBuildsPostedPanel;
		private System.Windows.Forms.Panel UpdateBuildsPostedTopPanel;
		private System.Windows.Forms.Panel UpdateBuildsPostedBottomPanel;
		public System.Windows.Forms.ListView UpdateBuildsPostedListView;
		private System.Windows.Forms.ColumnHeader UpdateBuildsPostedAssetColumnHeader;
		private System.Windows.Forms.ColumnHeader UpdateBuildsPostedDateColumnHeader;
		private System.Windows.Forms.ColumnHeader UpdateBuildsPostedActionColumnHeader;
		public System.Windows.Forms.Label SelectedClassificationLabel;
		private System.Windows.Forms.RichTextBox richTextBoxLabel;
		private MogControl_SyncTreeFilter SyncTreeFilter;
		private RichTextBox SyncFilterRichTextBox;
		private ComboBox TagsComboBox;
		private NJFLib.Controls.CollapsibleSplitter splitterLeft;
		private Panel panel1;
		private CheckBox TagEnabledCheckBox;
		private ToolTip SyncToolTip;
		private ComboBox SyncFiltersComboBox;
		private CheckBox SyncFilterCheckBox;
		public CheckBox UpdateBuildCleanUnknownFilesCheckBox;

		public MogControl_AssetContextMenu mAssetContextMenu;

		public SyncLatestForm(MogMainForm main, bool adminMode, string defaultTag, string defaultFilter, string defaultBuildType, string defaultPlatform, string currentVersion, bool hide, bool updateModifiedMissing, bool cleanUnknownFiles)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			mDefaultFilter = defaultFilter;
			mDefaultTag = defaultTag;
			mainForm = main;

			UpdateBuildCheckMissingCheckBox.Checked = updateModifiedMissing;
			UpdateBuildCleanUnknownFilesCheckBox.Checked = cleanUnknownFiles;

			// This is for the richTextBox that shows the users why things are certain colors.  I created the text in WordPad (Run-->wordpad.exe), saved the file, then opened that file in notepad to be able to get the source...  MS Word will create a huge source doc, so don't use it to change this text.
			this.richTextBoxLabel.Rtf = @"{\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{\f0\fswiss\fprq2\fcharset0 MS Sans Serif;}{\f1\fswiss\fcharset0 Arial;}}
{\colortbl ;\red255\green0\blue0;\red0\green128\blue0;}
{\*\generator Msftedit 5.41.15.1507;}\viewkind4\uc1\pard\f0\fs17 Legend:  \cf1 To Be Removed\cf0 , To Be Updated, \cf2 To Be Copied\cf0 .\f1\par
}";

			// Make sure we have a GameDataController setup before allowing this window to be created.  We cannot update a machine with no local branch!
			if (MOG_ControllerProject.GetCurrentSyncDataController() == null)
			{
				MOG_Prompt.PromptMessage("Update Build", "Cannot update a build without a valid Local Workspace defined.  Create a Local Workspace first then try again.");
				this.Close();
				return;
			}

			UpdateBuildsPostedListView.SmallImageList = MogUtil_AssetIcons.Images;

			// Assets
			mSortManager = new ListViewSortManager(UpdateBuildsPostedListView, new Type[] {
																							  typeof(ListViewTextCaseInsensitiveSort),
																							  typeof(ListViewDateSort),
																							  typeof(ListViewTextCaseInsensitiveSort),
			});

			SyncTreeFilter.FilterLoaded += new MogControl_SyncTreeFilter.FilterLoadedEvent(SyncTreeFilter_FilterLoaded);

			mAssetContextMenu = new MogControl_AssetContextMenu("NAME, DATE, ACTION, FULLNAME", UpdateBuildsPostedListView);
			UpdateBuildsPostedListView.ContextMenuStrip = mAssetContextMenu.InitializeContextMenu("{MergeVersion}");

			mPopulateWorker = new BackgroundWorker();
			mPopulateWorker.WorkerReportsProgress = true;
			mPopulateWorker.WorkerSupportsCancellation = true;
			mPopulateWorker.DoWork += Populate_Worker;
			mPopulateWorker.RunWorkerCompleted += Populate_Completed;
			//Start the worker in the Tree's afterselect event handler

			//PopulateSyncTargetTreeView(SyncRootTree);

			// Lock out the merge till the form finishes its loading
			UpdateBuildOkButton.Enabled = false;

			// Setup our sync tags
			PopulateTags();

			BuildName.Text = MOG_ControllerProject.GetBranchName();
		}

		private void UpdateFilterDropDown(string selectedText)
		{
			SyncFiltersComboBox.Items.Clear();
			SyncFiltersComboBox.Text = "";

			// Get the users tools
			if (MOG_ControllerProject.GetUser() != null)
			{
				// Get all the filter files found within the users tools directory
				foreach (string filter in Directory.GetFiles(MOG_ControllerProject.GetUser().GetUserToolsPath(), "*.sync"))
				{
					// Add the names of each one of them to the comboBox
					int index = SyncFiltersComboBox.Items.Add(new ComboBoxItem(Path.GetFileNameWithoutExtension(filter), Path.Combine(MOG_ControllerProject.GetUser().GetUserToolsPath(), filter)));
					SyncFiltersComboBox.AutoCompleteCustomSource.Add(Path.GetFileNameWithoutExtension(filter));

					if (Path.GetFileNameWithoutExtension(filter) == selectedText)
					{
						SyncFiltersComboBox.SelectedIndex = index;
					}
				}
			}

			if (!DesignMode)
			{
				// Get project tools
				// Get all the filter files found within the users tools directory
				string toolsPath = Path.Combine(MOG_ControllerProject.GetProjectPath(), "Tools");
				foreach (string filter in Directory.GetFiles(toolsPath, "*.sync"))
				{
					// Add the names of each one of them to the comboBox
					int index = SyncFiltersComboBox.Items.Add(new ComboBoxItem(Path.GetFileNameWithoutExtension(filter), Path.Combine(toolsPath, filter)));
					SyncFiltersComboBox.AutoCompleteCustomSource.Add(Path.GetFileNameWithoutExtension(filter));

					if (Path.GetFileNameWithoutExtension(filter) == selectedText)
					{
						SyncFiltersComboBox.SelectedIndex = index;
					}
				}
			}

			if (SyncFiltersComboBox.Items.Count == 0)
			{
				SyncFilterCheckBox.Checked = false;
			}
		}

		private void PopulateTags()
		{
			ArrayList Branches = MOG_DBProjectAPI.GetAllBranchNames();

			if (Branches != null)
			{
				foreach (MOG_DBBranchInfo branch in Branches)
				{
					if (branch.mTag)
					{
						TagsComboBox.Items.Add(branch.mBranchName);
						TagsComboBox.AutoCompleteCustomSource.Add(branch.mBranchName);
					}
				}
			}
		}

		private void Populate_Worker(object sender, DoWorkEventArgs e)
		{
			UpdatePostedInformationList(sender as BackgroundWorker, e.Argument as string);
		}

		private void Populate_Completed(object sender, RunWorkerCompletedEventArgs e)
		{
			UpdateBuildsPostedListView.VirtualListSize = 0;
			UpdateBuildsPostedListView.TopItem = null;
			UpdateBuildsPostedListView.VirtualListSize = (mListViewData.Count > 0) ? mListViewData.Count : 1;

			UpdateBuildsPostedListView.Cursor = Cursors.Default;

			UpdateBuildsPostedListView.Refresh();

			bHaltEvents = false;

			// Now let the user continue
			CheckForUpdateBuildOkButtonEnable();
		}

		/// <summary>
		/// This function will populate the postedBuild information listView 
		/// </summary>
		private void UpdatePostedInformationList(BackgroundWorker worker, string classification)
		{
			mListViewData.Clear();

			// Make sure we have a valid classification, if not, assign our project name
			if (classification == null)
			{
				classification = MOG_ControllerProject.GetProjectName();
			}

			// Get our last update time
			string lastUpdate = "0";
			string GamedataPath = MOG_ControllerProject.GetCurrentSyncDataController().GetSyncDirectory();

			string projectPrefsFilename = GamedataPath + "\\MOG\\Sync.Info";
			if (DosUtils.FileExist(projectPrefsFilename))
			{
				MOG_Ini currentProjectPrefs = new MOG_Ini(projectPrefsFilename);
				if (currentProjectPrefs.SectionExist("Workspace"))
				{
					if (currentProjectPrefs.KeyExist("Workspace", "LastUpdated"))
					{
						lastUpdate = currentProjectPrefs.GetString("Workspace", "LastUpdated");
					}
				}
			}

			string computerName = MOG_ControllerSystem.GetComputerName();
			string projectName = MOG_ControllerProject.GetProjectName();
			string platformName = MOG_ControllerProject.GetPlatformName();
			string syncDirectory = MOG_ControllerProject.GetCurrentSyncDataController().GetSyncDirectory();
			string updateTag = SyncTag;
			string userName = MOG_ControllerProject.GetUserName();

			// Check if we even need to bother rebuilding the entire LocalSyncInfo?
			// Sometimes, what we already have is good enough
			if (mLocalSyncInfo == null ||
				mLocalSyncInfo.GetSyncedLocationInfo().mBranchName != updateTag)
			{
				//we need to make sure we have an appropriately instantiated LocalSyncInfo object before we show what will be updated.
				mLocalSyncInfo = new MOG_LocalSyncInfo(computerName, projectName, platformName, syncDirectory, updateTag, userName, classification, "");
			}

			// Make sure we have obtained our mLocalSyncInfo
			if (mLocalSyncInfo != null)
			{
				// Start populating the dialog...
				ArrayList updateAssetList = new ArrayList();
				ArrayList revertAssetList = new ArrayList();
				foreach (MOG_DBUpdatePair pair in mLocalSyncInfo.GetUpdateAssetList())
				{
					// Check if the 
					if (string.Compare(pair.mNewVersion.GetVersionTimeStamp(), pair.mOldVersion.GetVersionTimeStamp(), true) < 0)
					{
						revertAssetList.Add(pair.mNewVersion);
					}
					else
					{
						updateAssetList.Add(pair.mNewVersion);
					}
				}

				// Add remove assets to our ListView
				CreateListViewItems(worker, mLocalSyncInfo.GetRemoveAssetList(), Color.OrangeRed, "Assets that need to be removed", "Remove");

				// Add pairs to our listView
				CreateListViewItems(worker, revertAssetList, Color.DarkGreen, "Assets that need to be reverted", "Revert");
				// Add pairs to our listView
				CreateListViewItems(worker, updateAssetList, Color.Green, "Assets that need to be updated", "Update");

				// Add new assets to our ListView
				CreateListViewItems(worker, mLocalSyncInfo.GetAddAssetList(), Color.Green, "Assets that need to be copied", "New");
			}
		}

		private void UpdateBuildsPostedListView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
		{
			if (mPopulateWorker.IsBusy || mListViewData.Count == 0)
			{
				if (mPopulateWorker.IsBusy)
				{
					e.Item = new ListViewItem("Retrieving Information...");
				}
				else
				{
					e.Item = new ListViewItem("Nothing New!");
				}

				//The ListView requires the subitem count to match the column count when in virtual mode
				while (e.Item.SubItems.Count < UpdateBuildsPostedListView.Columns.Count)
				{
					e.Item.SubItems.Add("");
				}
			}
			else
			{
				SyncAssetData data = mListViewData[e.ItemIndex];

				e.Item = new ListViewItem();
				e.Item.ForeColor = data.color;
				if (data is SyncAssetFilename)
				{
					SyncAssetFilename filename = data as SyncAssetFilename;
					e.Item.ImageIndex = MogUtil_AssetIcons.GetAssetIconIndex(filename.filename.GetAssetFullName(), null, false);
					e.Item.Text = filename.filename.GetAssetName();
					e.Item.SubItems.Add(filename.filename.GetVersionTimeStampString(""));
					e.Item.SubItems.Add(filename.action);
					e.Item.SubItems.Add(filename.filename.GetOriginalFilename());
				}
				else if (data is SyncAssetMessage)
				{
					SyncAssetMessage message = data as SyncAssetMessage;
					e.Item.Text = message.message;
				}
			}
		}

		private void CreateListViewItems(BackgroundWorker worker, ArrayList assets, Color textColor, string message, string action)
		{
			if (assets != null)
			{
				List<SyncAssetData> addList = new List<SyncAssetData>();
				List<SyncAssetData> insertList = new List<SyncAssetData>();
				
				// Get local mem copy of these variables
				string exclusion = Exclusions;
				string inclusion = Inclusions;

				// Populate our UpdateBuildsListView with all assets that are newer
				assets.Sort();
				foreach (MOG_Filename filename in assets)
				{
					if (worker.CancellationPending)
					{
						return;
					}

					// Is this asset filtered?
					if (mLocalSyncInfo.IsAssetIncluded(filename, exclusion, inclusion) == false)
					{
						SyncAssetFilename data = new SyncAssetFilename();
						data.color = Color.Gray;
						data.filename = filename;
						data.action = "Filtered";

						addList.Add(data);
					}
					else
					{
						SyncAssetFilename data = new SyncAssetFilename();
						data.color = textColor;
						data.filename = filename;
						data.action = action;

						insertList.Add(data);
					}
				}

				// Add the prepared lists
				mListViewData.InsertRange(0, insertList);
				mListViewData.AddRange(addList);
			}
			else
			{
				// No assets found, add an all-is-well item
				SyncAssetMessage data = new SyncAssetMessage();
				data.color = textColor;
				data.message = message;

				mListViewData.Add(data);
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SyncLatestForm));
			this.UpdateBuildOkButton = new System.Windows.Forms.Button();
			this.UpdateBuildCancelButton = new System.Windows.Forms.Button();
			this.AvailableBuildLabel = new System.Windows.Forms.Label();
			this.UpdateBuildImageList = new System.Windows.Forms.ImageList(this.components);
			this.TopPanel = new System.Windows.Forms.Panel();
			this.BuildName = new System.Windows.Forms.Label();
			this.BottomPanel = new System.Windows.Forms.Panel();
			this.UpdateBuildCleanUnknownFilesCheckBox = new System.Windows.Forms.CheckBox();
			this.SyncFilterRichTextBox = new System.Windows.Forms.RichTextBox();
			this.UpdateBuildCheckMissingCheckBox = new System.Windows.Forms.CheckBox();
			this.SelectedClassificationLabel = new System.Windows.Forms.Label();
			this.panel2 = new System.Windows.Forms.Panel();
			this.UpdateBuildsPostedPanel = new System.Windows.Forms.Panel();
			this.UpdateBuildsPostedBottomPanel = new System.Windows.Forms.Panel();
			this.splitterLeft = new NJFLib.Controls.CollapsibleSplitter();
			this.SyncTreeFilter = new MOG_ControlsLibrary.Controls.MogControl_SyncTreeFilter();
			this.panel1 = new System.Windows.Forms.Panel();
			this.SyncFilterCheckBox = new System.Windows.Forms.CheckBox();
			this.SyncFiltersComboBox = new System.Windows.Forms.ComboBox();
			this.TagEnabledCheckBox = new System.Windows.Forms.CheckBox();
			this.UpdateBuildsPostedListView = new System.Windows.Forms.ListView();
			this.UpdateBuildsPostedAssetColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.UpdateBuildsPostedDateColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.UpdateBuildsPostedActionColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.TagsComboBox = new System.Windows.Forms.ComboBox();
			this.UpdateBuildsPostedTopPanel = new System.Windows.Forms.Panel();
			this.richTextBoxLabel = new System.Windows.Forms.RichTextBox();
			this.SyncRootTree = new System.Windows.Forms.TreeView();
			this.SyncToolTip = new System.Windows.Forms.ToolTip(this.components);
			this.TopPanel.SuspendLayout();
			this.BottomPanel.SuspendLayout();
			this.panel2.SuspendLayout();
			this.UpdateBuildsPostedPanel.SuspendLayout();
			this.UpdateBuildsPostedBottomPanel.SuspendLayout();
			this.panel1.SuspendLayout();
			this.UpdateBuildsPostedTopPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// UpdateBuildOkButton
			// 
			this.UpdateBuildOkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.UpdateBuildOkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.UpdateBuildOkButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.UpdateBuildOkButton.Location = new System.Drawing.Point(650, 15);
			this.UpdateBuildOkButton.Name = "UpdateBuildOkButton";
			this.UpdateBuildOkButton.Size = new System.Drawing.Size(80, 23);
			this.UpdateBuildOkButton.TabIndex = 1;
			this.UpdateBuildOkButton.Text = "Ok";
			// 
			// UpdateBuildCancelButton
			// 
			this.UpdateBuildCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.UpdateBuildCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.UpdateBuildCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.UpdateBuildCancelButton.Location = new System.Drawing.Point(738, 15);
			this.UpdateBuildCancelButton.Name = "UpdateBuildCancelButton";
			this.UpdateBuildCancelButton.Size = new System.Drawing.Size(80, 23);
			this.UpdateBuildCancelButton.TabIndex = 2;
			this.UpdateBuildCancelButton.Text = "Cancel";
			// 
			// AvailableBuildLabel
			// 
			this.AvailableBuildLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.AvailableBuildLabel.Location = new System.Drawing.Point(8, 8);
			this.AvailableBuildLabel.Name = "AvailableBuildLabel";
			this.AvailableBuildLabel.Size = new System.Drawing.Size(200, 16);
			this.AvailableBuildLabel.TabIndex = 3;
			this.AvailableBuildLabel.Text = "Recent asset changes in this branch:";
			// 
			// UpdateBuildImageList
			// 
			this.UpdateBuildImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("UpdateBuildImageList.ImageStream")));
			this.UpdateBuildImageList.TransparentColor = System.Drawing.Color.Magenta;
			this.UpdateBuildImageList.Images.SetKeyName(0, "");
			this.UpdateBuildImageList.Images.SetKeyName(1, "");
			this.UpdateBuildImageList.Images.SetKeyName(2, "");
			// 
			// TopPanel
			// 
			this.TopPanel.BackColor = System.Drawing.SystemColors.Control;
			this.TopPanel.Controls.Add(this.BuildName);
			this.TopPanel.Controls.Add(this.AvailableBuildLabel);
			this.TopPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.TopPanel.Location = new System.Drawing.Point(0, 0);
			this.TopPanel.Name = "TopPanel";
			this.TopPanel.Size = new System.Drawing.Size(830, 24);
			this.TopPanel.TabIndex = 15;
			// 
			// BuildName
			// 
			this.BuildName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.BuildName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
			this.BuildName.Location = new System.Drawing.Point(208, 8);
			this.BuildName.Name = "BuildName";
			this.BuildName.Size = new System.Drawing.Size(528, 16);
			this.BuildName.TabIndex = 4;
			this.BuildName.Text = "Current";
			// 
			// BottomPanel
			// 
			this.BottomPanel.BackColor = System.Drawing.SystemColors.Control;
			this.BottomPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.BottomPanel.Controls.Add(this.UpdateBuildCleanUnknownFilesCheckBox);
			this.BottomPanel.Controls.Add(this.SyncFilterRichTextBox);
			this.BottomPanel.Controls.Add(this.UpdateBuildCheckMissingCheckBox);
			this.BottomPanel.Controls.Add(this.UpdateBuildCancelButton);
			this.BottomPanel.Controls.Add(this.UpdateBuildOkButton);
			this.BottomPanel.Controls.Add(this.SelectedClassificationLabel);
			this.BottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.BottomPanel.Location = new System.Drawing.Point(0, 426);
			this.BottomPanel.Name = "BottomPanel";
			this.BottomPanel.Size = new System.Drawing.Size(830, 51);
			this.BottomPanel.TabIndex = 16;
			// 
			// UpdateBuildCleanUnknownFilesCheckBox
			// 
			this.UpdateBuildCleanUnknownFilesCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.UpdateBuildCleanUnknownFilesCheckBox.Location = new System.Drawing.Point(439, 7);
			this.UpdateBuildCleanUnknownFilesCheckBox.Name = "UpdateBuildCleanUnknownFilesCheckBox";
			this.UpdateBuildCleanUnknownFilesCheckBox.Size = new System.Drawing.Size(205, 17);
			this.UpdateBuildCleanUnknownFilesCheckBox.TabIndex = 6;
			this.UpdateBuildCleanUnknownFilesCheckBox.Text = "Clean Unknown Files (Slower)";
			this.UpdateBuildCleanUnknownFilesCheckBox.UseVisualStyleBackColor = true;
			// 
			// SyncFilterRichTextBox
			// 
			this.SyncFilterRichTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.SyncFilterRichTextBox.BackColor = System.Drawing.SystemColors.Control;
			this.SyncFilterRichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.SyncFilterRichTextBox.Location = new System.Drawing.Point(3, -2);
			this.SyncFilterRichTextBox.Name = "SyncFilterRichTextBox";
			this.SyncFilterRichTextBox.Size = new System.Drawing.Size(409, 51);
			this.SyncFilterRichTextBox.TabIndex = 5;
			this.SyncFilterRichTextBox.Text = "";
			// 
			// UpdateBuildCheckMissingCheckBox
			// 
			this.UpdateBuildCheckMissingCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.UpdateBuildCheckMissingCheckBox.Location = new System.Drawing.Point(439, 24);
			this.UpdateBuildCheckMissingCheckBox.Name = "UpdateBuildCheckMissingCheckBox";
			this.UpdateBuildCheckMissingCheckBox.Size = new System.Drawing.Size(205, 21);
			this.UpdateBuildCheckMissingCheckBox.TabIndex = 3;
			this.UpdateBuildCheckMissingCheckBox.Text = "Update Modified/Missing (Slower)";
			// 
			// SelectedClassificationLabel
			// 
			this.SelectedClassificationLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.SelectedClassificationLabel.AutoSize = true;
			this.SelectedClassificationLabel.Location = new System.Drawing.Point(136, 23);
			this.SelectedClassificationLabel.Name = "SelectedClassificationLabel";
			this.SelectedClassificationLabel.Size = new System.Drawing.Size(0, 13);
			this.SelectedClassificationLabel.TabIndex = 4;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.UpdateBuildsPostedPanel);
			this.panel2.Controls.Add(this.SyncRootTree);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(0, 24);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(830, 402);
			this.panel2.TabIndex = 18;
			// 
			// UpdateBuildsPostedPanel
			// 
			this.UpdateBuildsPostedPanel.Controls.Add(this.UpdateBuildsPostedBottomPanel);
			this.UpdateBuildsPostedPanel.Controls.Add(this.UpdateBuildsPostedTopPanel);
			this.UpdateBuildsPostedPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.UpdateBuildsPostedPanel.Location = new System.Drawing.Point(0, 0);
			this.UpdateBuildsPostedPanel.Name = "UpdateBuildsPostedPanel";
			this.UpdateBuildsPostedPanel.Size = new System.Drawing.Size(830, 402);
			this.UpdateBuildsPostedPanel.TabIndex = 17;
			// 
			// UpdateBuildsPostedBottomPanel
			// 
			this.UpdateBuildsPostedBottomPanel.Controls.Add(this.splitterLeft);
			this.UpdateBuildsPostedBottomPanel.Controls.Add(this.panel1);
			this.UpdateBuildsPostedBottomPanel.Controls.Add(this.SyncTreeFilter);
			this.UpdateBuildsPostedBottomPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.UpdateBuildsPostedBottomPanel.Location = new System.Drawing.Point(0, 32);
			this.UpdateBuildsPostedBottomPanel.Name = "UpdateBuildsPostedBottomPanel";
			this.UpdateBuildsPostedBottomPanel.Size = new System.Drawing.Size(830, 370);
			this.UpdateBuildsPostedBottomPanel.TabIndex = 20;
			// 
			// splitterLeft
			// 
			this.splitterLeft.AnimationDelay = 5;
			this.splitterLeft.AnimationStep = 80;
			this.splitterLeft.BorderStyle3D = System.Windows.Forms.Border3DStyle.RaisedInner;
			this.splitterLeft.ControlToHide = this.SyncTreeFilter;
			this.splitterLeft.ExpandParentForm = false;
			this.splitterLeft.Location = new System.Drawing.Point(208, 0);
			this.splitterLeft.Name = "splitterTop";
			this.splitterLeft.TabIndex = 22;
			this.splitterLeft.TabStop = false;
			this.splitterLeft.UseAnimations = false;
			this.splitterLeft.VisualStyle = NJFLib.Controls.VisualStyles.XP;
			// 
			// SyncTreeFilter
			// 
			this.SyncTreeFilter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(228)))), ((int)(((byte)(239)))));
			this.SyncTreeFilter.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.SyncTreeFilter.Dock = System.Windows.Forms.DockStyle.Left;
			this.SyncTreeFilter.Location = new System.Drawing.Point(0, 0);
			this.SyncTreeFilter.MinimumSize = new System.Drawing.Size(0, 120);
			this.SyncTreeFilter.Name = "SyncTreeFilter";
			this.SyncTreeFilter.Size = new System.Drawing.Size(208, 370);
			this.SyncTreeFilter.TabIndex = 20;
			this.SyncTreeFilter.Visible = false;
			this.SyncTreeFilter.ConfigureFilter += new MOG_ControlsLibrary.Controls.MogControl_SyncTreeFilter.ConfigureFiltersEvent(this.SyncTreeFilter_ConfigureFilter);
			this.SyncTreeFilter.CheckedStateChanged += new MOG_ControlsLibrary.Controls.MogControl_SyncTreeFilter.TreeViewEvent(this.SyncTreeFilter_CheckedStateChanged);
			this.SyncTreeFilter.TreeInitialized += new MOG_ControlsLibrary.Controls.MogControl_SyncTreeFilter.TreeViewEvent(this.SyncTreeFilter_TreeInitialized);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.SyncFilterCheckBox);
			this.panel1.Controls.Add(this.SyncFiltersComboBox);
			this.panel1.Controls.Add(this.TagEnabledCheckBox);
			this.panel1.Controls.Add(this.UpdateBuildsPostedListView);
			this.panel1.Controls.Add(this.TagsComboBox);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(208, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(622, 370);
			this.panel1.TabIndex = 23;
			// 
			// SyncFilterCheckBox
			// 
			this.SyncFilterCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.SyncFilterCheckBox.Checked = true;
			this.SyncFilterCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.SyncFilterCheckBox.Image = global::MOG_Client.Properties.Resources.Filter;
			this.SyncFilterCheckBox.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.SyncFilterCheckBox.Location = new System.Drawing.Point(14, 345);
			this.SyncFilterCheckBox.Name = "SyncFilterCheckBox";
			this.SyncFilterCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.SyncFilterCheckBox.Size = new System.Drawing.Size(124, 22);
			this.SyncFilterCheckBox.TabIndex = 23;
			this.SyncFilterCheckBox.Text = "Sync with filter";
			this.SyncFilterCheckBox.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.SyncFilterCheckBox.UseVisualStyleBackColor = true;
			this.SyncFilterCheckBox.CheckedChanged += new System.EventHandler(this.SyncFilterCheckBox_CheckedChanged);
			// 
			// SyncFiltersComboBox
			// 
			this.SyncFiltersComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.SyncFiltersComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.SyncFiltersComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
			this.SyncFiltersComboBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.SyncFiltersComboBox.FormattingEnabled = true;
			this.SyncFiltersComboBox.Location = new System.Drawing.Point(144, 346);
			this.SyncFiltersComboBox.Name = "SyncFiltersComboBox";
			this.SyncFiltersComboBox.Size = new System.Drawing.Size(468, 21);
			this.SyncFiltersComboBox.TabIndex = 22;
			this.SyncFiltersComboBox.SelectedIndexChanged += new System.EventHandler(this.SyncFiltersComboBox_SelectedIndexChanged);
			this.SyncFiltersComboBox.Validated += new System.EventHandler(this.SyncFiltersComboBox_Validated);
			// 
			// TagEnabledCheckBox
			// 
			this.TagEnabledCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.TagEnabledCheckBox.Checked = true;
			this.TagEnabledCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.TagEnabledCheckBox.Image = global::MOG_Client.Properties.Resources.Tag;
			this.TagEnabledCheckBox.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.TagEnabledCheckBox.Location = new System.Drawing.Point(14, 319);
			this.TagEnabledCheckBox.Name = "TagEnabledCheckBox";
			this.TagEnabledCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.TagEnabledCheckBox.Size = new System.Drawing.Size(124, 22);
			this.TagEnabledCheckBox.TabIndex = 21;
			this.TagEnabledCheckBox.Text = "Sync to tag";
			this.TagEnabledCheckBox.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.TagEnabledCheckBox.UseVisualStyleBackColor = true;
			this.TagEnabledCheckBox.CheckedChanged += new System.EventHandler(this.TagEnabledCheckBox_CheckedChanged);
			// 
			// UpdateBuildsPostedListView
			// 
			this.UpdateBuildsPostedListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.UpdateBuildsPostedListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.UpdateBuildsPostedAssetColumnHeader,
            this.UpdateBuildsPostedDateColumnHeader,
            this.UpdateBuildsPostedActionColumnHeader});
			this.UpdateBuildsPostedListView.FullRowSelect = true;
			this.UpdateBuildsPostedListView.Location = new System.Drawing.Point(6, 3);
			this.UpdateBuildsPostedListView.Name = "UpdateBuildsPostedListView";
			this.UpdateBuildsPostedListView.Size = new System.Drawing.Size(616, 313);
			this.UpdateBuildsPostedListView.TabIndex = 19;
			this.UpdateBuildsPostedListView.UseCompatibleStateImageBehavior = false;
			this.UpdateBuildsPostedListView.View = System.Windows.Forms.View.Details;
			this.UpdateBuildsPostedListView.VirtualMode = true;
			this.UpdateBuildsPostedListView.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.UpdateBuildsPostedListView_RetrieveVirtualItem);
			// 
			// UpdateBuildsPostedAssetColumnHeader
			// 
			this.UpdateBuildsPostedAssetColumnHeader.Text = "Asset";
			this.UpdateBuildsPostedAssetColumnHeader.Width = 286;
			// 
			// UpdateBuildsPostedDateColumnHeader
			// 
			this.UpdateBuildsPostedDateColumnHeader.Text = "Date";
			this.UpdateBuildsPostedDateColumnHeader.Width = 130;
			// 
			// UpdateBuildsPostedActionColumnHeader
			// 
			this.UpdateBuildsPostedActionColumnHeader.Text = "Action";
			// 
			// TagsComboBox
			// 
			this.TagsComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.TagsComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.TagsComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
			this.TagsComboBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.TagsComboBox.FormattingEnabled = true;
			this.TagsComboBox.Location = new System.Drawing.Point(144, 320);
			this.TagsComboBox.Name = "TagsComboBox";
			this.TagsComboBox.Size = new System.Drawing.Size(468, 21);
			this.TagsComboBox.TabIndex = 1;
			this.TagsComboBox.SelectionChangeCommitted += new System.EventHandler(this.TagsComboBox_SelectionChangeCommitted);
			this.TagsComboBox.TextUpdate += new System.EventHandler(this.TagsComboBox_TextUpdate);
			// 
			// UpdateBuildsPostedTopPanel
			// 
			this.UpdateBuildsPostedTopPanel.BackColor = System.Drawing.SystemColors.Control;
			this.UpdateBuildsPostedTopPanel.Controls.Add(this.richTextBoxLabel);
			this.UpdateBuildsPostedTopPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.UpdateBuildsPostedTopPanel.Location = new System.Drawing.Point(0, 0);
			this.UpdateBuildsPostedTopPanel.Name = "UpdateBuildsPostedTopPanel";
			this.UpdateBuildsPostedTopPanel.Size = new System.Drawing.Size(830, 32);
			this.UpdateBuildsPostedTopPanel.TabIndex = 19;
			// 
			// richTextBoxLabel
			// 
			this.richTextBoxLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.richTextBoxLabel.BackColor = System.Drawing.SystemColors.Control;
			this.richTextBoxLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.richTextBoxLabel.Location = new System.Drawing.Point(8, 8);
			this.richTextBoxLabel.Name = "richTextBoxLabel";
			this.richTextBoxLabel.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
			this.richTextBoxLabel.Size = new System.Drawing.Size(814, 22);
			this.richTextBoxLabel.TabIndex = 0;
			this.richTextBoxLabel.Text = "Filled at runtime.......";
			// 
			// SyncRootTree
			// 
			this.SyncRootTree.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.SyncRootTree.Location = new System.Drawing.Point(0, 0);
			this.SyncRootTree.Name = "SyncRootTree";
			this.SyncRootTree.PathSeparator = "~";
			this.SyncRootTree.Size = new System.Drawing.Size(112, 171);
			this.SyncRootTree.TabIndex = 15;
			this.SyncRootTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.SyncRootTree_AfterSelect);
			this.SyncRootTree.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.SyncRootTree_BeforeSelect);
			// 
			// SyncLatestForm
			// 
			this.AcceptButton = this.UpdateBuildOkButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.UpdateBuildCancelButton;
			this.ClientSize = new System.Drawing.Size(830, 477);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.TopPanel);
			this.Controls.Add(this.BottomPanel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(448, 440);
			this.Name = "SyncLatestForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "GetLatest Project";
			this.Load += new System.EventHandler(this.SyncLatestForm_Load);
			this.Shown += new System.EventHandler(this.SyncLatestForm_Shown);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SyncLatestForm_FormClosing);
			this.TopPanel.ResumeLayout(false);
			this.BottomPanel.ResumeLayout(false);
			this.BottomPanel.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.UpdateBuildsPostedPanel.ResumeLayout(false);
			this.UpdateBuildsPostedBottomPanel.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.UpdateBuildsPostedTopPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void SyncLatestForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (mPopulateWorker.IsBusy)
			{
				mPopulateWorker.CancelAsync();
			}
		}

		private TreeNode previouslySelectedNode;
		private void SyncRootTree_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			// If we should ignore the "Nothing returned from database" node, ignore it...
			if (e.Node.Text != Nothing_Returned_Text)
			{
				// If we have a different node selected than the last selected node...
				if (previouslySelectedNode == null || previouslySelectedNode != e.Node)
				{
					// Make sure we have a Tag and that it is a string...
					string classification = e.Node.Tag as string;
					if (classification != null)
					{
						// Start up a new worker with our syncNodeRoot text
						StartWorker(classification);

						previouslySelectedNode = e.Node;
						SelectedClassificationLabel.Text = classification;
					}
					else
					{
						//report to let our programmer know he/she messed up...
						MOG_Report.ReportMessage("Invalid Tag", "Tag for variable `e.Node.Tag` not set correctly by programmer", Environment.StackTrace, MOG_ALERT_LEVEL.ERROR);
					}
				}
			}
		}

		/// <summary>
		/// Makes sure that we keep our selected SyncRootNode highlighted
		/// </summary>
		private void SyncRootTree_BeforeSelect(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		{
			if (previouslySelectedNode != null)
			{
				previouslySelectedNode.BackColor = SyncRootTree.BackColor;
			}
			e.Node.BackColor = SystemColors.Highlight;
		}


		private void PopulateSyncTargetTreeView(TreeView viewToPopulate)
		{
			// If we have any nodes, clear them
			if (viewToPopulate.Nodes.Count > 0)
			{
				viewToPopulate.Nodes.Clear();
			}

			// Create our property and query the DB
			ArrayList classifications = MOG.DATABASE.MOG_DBAssetAPI.GetAllActiveClassifications(MOG.MOG_PropertyFactory.MOG_Sync_OptionsProperties.New_SyncLabel(""));
			// Be sure we are sorted (just in case)
			classifications.Sort();

			// Add our full project SyncRootNode
			string projectName = MOG_ControllerProject.GetProjectName();
			TreeNode projectRoot = new TreeNode(projectName);
			projectRoot.Tag = projectName;
			viewToPopulate.Nodes.Add(projectRoot);

			// Foreach classification we got with a SyncRootLabel, add it
			foreach (string classification in classifications)
			{
				string syncRootName = MOG_Properties.OpenClassificationProperties(classification).SyncLabel;
				// If our syncRootName is the empty string, ignore it
				if (syncRootName.Length > 0)
				{
					TreeNode nodeToAdd = new TreeNode(syncRootName);
					nodeToAdd.Tag = classification;
					AddSyncRootNodeToNodeCollection(viewToPopulate.Nodes, nodeToAdd);
				}
			}

			viewToPopulate.ExpandAll();

			// If we have nodes, select the first node...
			if (viewToPopulate.Nodes.Count > 0)
			{
				viewToPopulate.SelectedNode = viewToPopulate.Nodes[0];
			}
			// Otherwise, add our project name and select it...
			else
			{
				TreeNode projectNode = viewToPopulate.Nodes.Add(MOG_ControllerProject.GetProjectName());
				projectNode.Tag = projectNode.Text;
				viewToPopulate.SelectedNode = projectNode;
			}
		}

		/// <summary>
		/// Go down the tree and find where we want our node to be
		/// </summary>
		private void AddSyncRootNodeToNodeCollection(TreeNodeCollection nodes, TreeNode syncRootNode)
		{
			string classification = syncRootNode.Tag.ToString();

			// Foreach node, check its tag to see if this syncRootNode should be underneath it
			foreach (TreeNode node in nodes)
			{
				string superClassification = node.Tag.ToString();
				if (classification.IndexOf(superClassification) > -1)
				{
					AddSyncRootNodeToNodeCollection(node.Nodes, syncRootNode);
					return;
				}
			}

			//If we are at the end of our recursive process (or a parent classification for this 
			//  syncRootNode was not found, add the node
			nodes.Add(syncRootNode);
		}

		private void SyncTreeFilter_CheckedStateChanged(object sender, TreeViewEventArgs e)
		{
			mExclusions = SyncTreeFilter.Filter.GetExclusionString();
			mInclusions = SyncTreeFilter.Filter.GetInclusionString();

			PostFilterLoaded(e);
		}

		private void SyncTreeFilter_TreeInitialized(object sender, TreeViewEventArgs e)
		{
			PostFilterLoaded(e);
		}

		private void PostFilterLoaded(TreeViewEventArgs e)
		{
			try
			{
				ResetList(e);
				string filter = "SyncFilter:\nExclude=" + Exclusions.Replace(",", "\n") + "\nInclude=" + Inclusions.Replace(",", "\n");
#if DEBUG
				SyncFilterRichTextBox.Text = filter;
#endif
				SyncToolTip.SetToolTip(SyncFiltersComboBox, filter);
			}
			catch { }
		}

		private void ResetList(TreeViewEventArgs e)
		{
			if (bHaltEvents == false)
			{
				bHaltEvents = true;
				// Update our sync list with these new exclusions

				// Make sure we have a Tag and that it is a string...
				if (UpdateBuildsPostedListView.VirtualListSize > 0)
				{
					UpdateBuildsPostedListView.TopItem = UpdateBuildsPostedListView.Items[0];
				}
				else
				{
					UpdateBuildsPostedListView.TopItem = null;
				}
				UpdateBuildsPostedListView.VirtualListSize = 1;
				UpdateBuildsPostedListView.Cursor = Cursors.WaitCursor;

				// Start up a new worker with our syncNodeRoot text
				StartWorker(null);

				if (e != null)
				{
					previouslySelectedNode = e.Node;
				}
				SelectedClassificationLabel.Text = "";
			}
		}

		private void SyncLatestForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (DialogResult != DialogResult.Cancel)
			{
				string scope = MOG_ControllerProject.GetProjectName() + "." + MOG_ControllerProject.GetWorkspaceDirectory() + ".";

				MogUtils_Settings.SaveSetting(this.Name, scope + "SyncFilter", "Visible", SyncTreeFilter.Visible.ToString());
				MogUtils_Settings.SaveSetting(this.Name, scope + "Tag", "Enabled", TagEnabledCheckBox.Checked.ToString());
				MogUtils_Settings.SaveSetting(this.Name, scope + "Tag", "LastSelected", TagsComboBox.Text);
				MogUtils_Settings.SaveSetting(this.Name, scope + "Filters", "Enabled", SyncFilterCheckBox.Checked.ToString());
				MogUtils_Settings.SaveSetting(this.Name, scope + "Filters", "LastSelected", SyncFiltersComboBox.Text);
				MogUtils_Settings.SaveSetting(this.Name, "Size", "Width", Width.ToString());
				MogUtils_Settings.SaveSetting(this.Name, "Size", "Height", Height.ToString());
				MogUtils_Settings.SaveSetting(this.Name, "Size", "Top", Top.ToString());
				MogUtils_Settings.SaveSetting(this.Name, "Size", "Left", Left.ToString());

				string filter = SyncFiltersComboBox.Text;
				if (!SyncFilterCheckBox.Checked)
				{
					filter = "None";
				}

				string tag = TagsComboBox.Text;
				if (!TagEnabledCheckBox.Checked)
				{
					tag = "None";
				}

				MogUtils_Settings.SaveSetting(mainForm.mAssetManager.mLocal.GetActiveLocalTab().Text, "GetLatestFilter", filter);
				MogUtils_Settings.SaveSetting(mainForm.mAssetManager.mLocal.GetActiveLocalTab().Text, "GetLatestTag", tag);
				MogUtils_Settings.SaveSetting(mainForm.mAssetManager.mLocal.GetActiveLocalTab().Text, "GetLatestForce", UpdateBuildCheckMissingCheckBox.Checked.ToString());
				MogUtils_Settings.SaveSetting(mainForm.mAssetManager.mLocal.GetActiveLocalTab().Text, "CleanUnknownFiles", UpdateBuildCleanUnknownFilesCheckBox.Checked.ToString());
				mainForm.mAssetManager.mLocal.RefreshWindowToolbar();
			}
		}

		private void SyncLatestForm_Load(object sender, EventArgs e)
		{
			mLoading = true;

			string scope = MOG_ControllerProject.GetProjectName() + "." + MOG_ControllerProject.GetWorkspaceDirectory() + ".";

			SyncTreeFilter.Visible = MogUtils_Settings.LoadBoolSetting(this.Name, scope + "SyncFilter", "Visible", false);

			TagsComboBox.Text = mDefaultTag;				
			TagEnabledCheckBox.Checked = (TagsComboBox.Text.Length > 0);
			TagsComboBox.Select(0, 0);

			SyncFiltersComboBox.Text = mDefaultFilter;
			SyncFilterCheckBox.Checked = (SyncFiltersComboBox.Text.Length > 0);
			SyncFiltersComboBox.Select(0, 0);

			// Load appropriate Filter
			if (SyncFilterCheckBox.Checked && SyncFiltersComboBox.Text.Length > 0)
			{
				LoadFilter(SyncFiltersComboBox.Text);
			}
			else if (SyncFilterCheckBox.Checked && SyncFiltersComboBox.Text.Length == 0)
			{
				SyncFilterCheckBox.Checked = false;
			}

			// Load previous sizes
			if (MogUtils_Settings.SettingExist(this.Name, "Size", "Width"))
			{
				Width = MogUtils_Settings.LoadIntSetting(this.Name, "Size", "Width", 25);
				Height = MogUtils_Settings.LoadIntSetting(this.Name, "Size", "Height", 25);
			}

			if (MogUtils_Settings.SettingExist(this.Name, "Size", "Top"))
			{
				Top = MogUtils_Settings.LoadIntSetting(this.Name, "Size", "Top", 25);
				Left = MogUtils_Settings.LoadIntSetting(this.Name, "Size", "Left", 25);
			}

			MiscUtilities.EnsureFormLocatedOnValidMonitors(this);

			UpdateFilterDropDown(SyncFiltersComboBox.Text);

			mLoading = false;
		}

		private void SyncLatestForm_Shown(object sender, EventArgs e)
		{
			e.ToString();
			ResetList(null);
		}

		private void TagsComboBox_TextUpdate(object sender, EventArgs e)
		{
			if (!mLoading)
			{
				if (TagsComboBox.AutoCompleteCustomSource.Contains(UserSpecifiedSyncTag))
				{
					StartWorker("");
				}
				CheckForUpdateBuildOkButtonEnable();
			}
		}

		private void StartWorker(Object obj)
		{
			// If our worker is already busy, cancel it and wait for it to say it's finished
			if (mPopulateWorker.IsBusy)
			{
				mPopulateWorker.CancelAsync();
				while (mPopulateWorker.IsBusy)
				{
					Application.DoEvents();
					Thread.Sleep(100);
				}
			}

			// Clear the list and inform the user we are 'Retrieving Information...'
			if (UpdateBuildsPostedListView.VirtualListSize > 0)
			{
				UpdateBuildsPostedListView.TopItem = UpdateBuildsPostedListView.Items[0];
			}
			else
			{
				UpdateBuildsPostedListView.TopItem = null;
			}
			UpdateBuildsPostedListView.VirtualListSize = 1;
			UpdateBuildsPostedListView.Cursor = Cursors.WaitCursor;

			mPopulateWorker.RunWorkerAsync(obj);

			CheckForUpdateBuildOkButtonEnable();
		}

		private void TagEnabledCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			TagsComboBox.Enabled = TagEnabledCheckBox.Checked;
			TagsComboBox.Select(0, 0);

			if (!mLoading)
			{
				try
				{
					StartWorker("");
				}
				catch (Exception ex)
				{
					// Just eat this exception because the user will resolve this one on their own without a need for a message.  IT usually happens if the user double clicks the TagCheckbox
					if (ex.Message == "This BackgroundWorker is currently busy and cannot run multiple tasks concurrently.")
					{
						return;
					}
					else throw ex;
				}
			}
		}

		private void SyncFilterCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			SyncFiltersComboBox.Enabled = SyncFilterCheckBox.Checked;
			SyncFiltersComboBox.Select(0, 0);

			if (!mLoading)
			{
				// If we are disabled, clear the filter
				if (!SyncFiltersComboBox.Enabled)
				{
					LoadFilter("");
				}
				else
				{
					LoadFilter(SyncFiltersComboBox.Text);
				}
			}
		}

		private void TagsComboBox_SelectionChangeCommitted(object sender, EventArgs e)
		{
			if (!mLoading)
			{
				StartWorker("");
			}
		}

		private void CheckForUpdateBuildOkButtonEnable()
		{
			bool bEnabled = true;

			// Check if our thread is busy?
			if (mPopulateWorker.IsBusy)
			{
				bEnabled = false;
			}
			// Check if we have an invalid UserSpecifiedSyncTag
			else if (TagsComboBox.Enabled && 
					 TagsComboBox.Text.Length == 0)
			{
				bEnabled = false;
			}

			UpdateBuildOkButton.Enabled = bEnabled;
		}

		private void SyncFiltersComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			LoadFilter(SyncFiltersComboBox.Text);
		}

		private void LoadFilter(string filterName)
		{
			if (filterName.Length > 0 && SyncFiltersComboBox.Enabled)
			{
				// Is this an already fully qualified name?
				if (DosUtils.ExistFast(filterName) == false)
				{
					// No, then lets assume it came from the user's tools directory
					filterName = MOG_ControllerSystem.LocateTool(filterName + ".sync");
				}

				// Do we have a valid filter filename
				if (filterName.Length != 0)
				{
					SyncFiltersComboBox.Text = Path.GetFileNameWithoutExtension(filterName);
					SyncFiltersComboBox.Tag = filterName;

					if (DosUtils.ExistFast(filterName))
					{
						SyncFilter mFilter = new SyncFilter();
						if (mFilter.Load(SyncFiltersComboBox.Tag as string))
						{
							mExclusions = mFilter.GetExclusionString();
							mInclusions = mFilter.GetInclusionString();

							PostFilterLoaded(null);
						}
					}
				}
				else
				{
					// Clear our lists
					mExclusions = "";
					mInclusions = "";

					// Clear our tool tips
					PostFilterLoaded(null);
				}
			}
			else
			{
				// Clear our lists
				mExclusions = "";
				mInclusions = "";

				// Clear our tool tips
				PostFilterLoaded(null);
			}
		}

		void SyncTreeFilter_FilterLoaded(string filterName)
		{
			mExclusions = SyncTreeFilter.Filter.GetExclusionString();
			mInclusions = SyncTreeFilter.Filter.GetInclusionString();
		}

		private void SyncTreeFilter_ConfigureFilter(object sender, bool enabled)
		{
			if (enabled)
			{
				UpdateBuildOkButton.Enabled = false;
				UpdateBuildCancelButton.Enabled = false;
				UpdateBuildsPostedListView.BackColor = Color.FromArgb(240, 240, 240);
			}
			else
			{
				UpdateBuildOkButton.Enabled = true;
				UpdateBuildCancelButton.Enabled = true;
				UpdateBuildsPostedListView.BackColor = SystemColors.Window;

				LoadFilter(SyncFiltersComboBox.Text);
				UpdateFilterDropDown("");
			}
		}

		private void SyncFiltersComboBox_Validated(object sender, EventArgs e)
		{
			if (!mLoading)
			{
				// Check if the user just set the filter to blank?
				if (SyncFiltersComboBox.Text.Length == 0)
				{
					// Force uncheck the filter's checkbox
					SyncFilterCheckBox.Checked = false;
				}
			}
		}
	}

	class ComboBoxItem
	{
		public string Name;
		public string FullPath;

		public ComboBoxItem(string name, string fullpath)
		{
			Name = name;
			FullPath = fullpath;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
