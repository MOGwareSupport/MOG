using System;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using MOG.TIME;
using MOG.COMMAND;
using MOG.DOSUTILS;
using MOG.FILENAME;
using MOG.DATABASE;
using MOG.PROPERTIES;
using MOG.PROJECT;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERASSET;
using MOG.CONTROLLER.CONTROLLERLIBRARY;
using MOG.CONTROLLER.CONTROLLERSYNCDATA;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.CONTROLLER.CONTROLLERREPOSITORY;

using MOG_ControlsLibrary.Common;
using MOG_ControlsLibrary.Utils;
using MOG_ControlsLibrary.Common.MOG_ContextMenu;
using EV.Windows.Forms;
using System.Collections.Specialized;
using System.Collections.Generic;
using MOG.PROMPT;
using MOG_Client.Client_Mog_Utilities.AssetOptions;
using MOG_Client.Client_Mog_Utilities;
using MOG_Client.Forms;
using MOG_CoreControls;

namespace MOG_ControlsLibrary.Common.MogControl_LibraryListView
{
	/// <summary>
	/// Summary description for MogControl_LibraryListView
	/// </summary>
	public class MogControl_LibraryListView : System.Windows.Forms.UserControl
	{
		private string mCurrentClassification = null;
        public string CurrentClassification
        {
            get { return mCurrentClassification; }
            set 
            { 
                mCurrentClassification = value;
                LibraryListView.Tag = mCurrentClassification;
            }
        }
	
		private MOG_Properties mCurrentClassificationProperties = null;
		private MogControl_AssetContextMenu mContextMenu;
		public MogControl_LibraryExplorer LibraryExplorer = null;
		
		private System.Windows.Forms.ColumnHeader NameColumnHeader;
		private System.Windows.Forms.ColumnHeader UserColumnHeader;
		private System.Windows.Forms.ColumnHeader DateColumnHeader;
		private System.Windows.Forms.ColumnHeader StatusColumnHeader;
		public System.Windows.Forms.ListView LibraryListView;
		private ListViewSortManager mLibrarySortManager;
		private System.Windows.Forms.ColumnHeader FullNameColumnHeader;
        private ColumnHeader LocalFileNameColumnHeader;
        private ColumnHeader RepositoryFileNameColumnHeader;
		private ColumnHeader ClassificationColumnHeader;
		private ColumnHeader CommentColumnHeader;

		private string mLastTopItem = "";
		private ColumnHeader TimeStampColumnHeader;
        private ColumnHeader ExtensionColumnHeader;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

        public delegate void DoubleClickEvent(object sender, EventArgs e);

        [Category("Behavior"), Description("Occures when selection has been changed")]
        public event DoubleClickEvent DoubleClickItem;
		
        public ListViewItem SelectedItem
        {
            get
            {
                if (LibraryListView.SelectedItems != null && LibraryListView.SelectedItems.Count > 0)
                {
                    return LibraryListView.SelectedItems[0];
                }
                else
                {
                    return null;
                }
            }
        }

		public MogControl_LibraryListView()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			mLibrarySortManager = 
				new ListViewSortManager(LibraryListView, new Type[] {	typeof(ListViewTextCaseInsensitiveSort),
																	typeof(ListViewTextCaseInsensitiveSort),
																	typeof(ListViewTextCaseInsensitiveSort),
																	typeof(ListViewTextCaseInsensitiveSort),
																	typeof(ListViewTextCaseInsensitiveSort),
																	typeof(ListViewDateSort),
																	typeof(ListViewDateSort),
																	typeof(ListViewTextCaseInsensitiveSort),
																	typeof(ListViewTextCaseInsensitiveSort),
                                                                    typeof(ListViewTextCaseInsensitiveSort),
                                                                    typeof(ListViewTextCaseInsensitiveSort)});


			mLibrarySortManager.Sort(0, SortOrder.Descending);

			// Drag drop stuff, added by JKB 3 Jan 06
			this.AllowDrop = true;
			this.DragEnter += new DragEventHandler(MogControl_LibraryListView_DragEnter);
			this.DragDrop += new DragEventHandler(MogControl_LibraryListView_DragDrop);
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "Test"}, -1, System.Drawing.Color.Empty, System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(255)))), ((int)(((byte)(221))))), null);
            this.LibraryListView = new System.Windows.Forms.ListView();
            this.NameColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.ClassificationColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.UserColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.CommentColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.TimeStampColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.DateColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.StatusColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.FullNameColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.LocalFileNameColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.RepositoryFileNameColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.ExtensionColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // LibraryListView
            // 
            this.LibraryListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.NameColumnHeader,
            this.ExtensionColumnHeader,
            this.ClassificationColumnHeader,
            this.UserColumnHeader,
            this.CommentColumnHeader,
            this.TimeStampColumnHeader,
            this.DateColumnHeader,
            this.StatusColumnHeader,
            this.FullNameColumnHeader,
            this.LocalFileNameColumnHeader,
            this.RepositoryFileNameColumnHeader});
            this.LibraryListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LibraryListView.FullRowSelect = true;
            this.LibraryListView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
            this.LibraryListView.LabelEdit = true;
            this.LibraryListView.Location = new System.Drawing.Point(0, 0);
            this.LibraryListView.Name = "LibraryListView";
            this.LibraryListView.Size = new System.Drawing.Size(904, 344);
            this.LibraryListView.TabIndex = 0;
            this.LibraryListView.UseCompatibleStateImageBehavior = false;
            this.LibraryListView.View = System.Windows.Forms.View.Details;
            this.LibraryListView.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.LibraryListView_AfterLabelEdit);
            this.LibraryListView.DoubleClick += new System.EventHandler(this.LibraryListView_DoubleClick);
            this.LibraryListView.BeforeLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.LibraryListView_BeforeLabelEdit);
            this.LibraryListView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.LibraryListView_ItemDrag);
            // 
            // NameColumnHeader
            // 
            this.NameColumnHeader.Text = "Name";
            this.NameColumnHeader.Width = 134;
            // 
            // ClassificationColumnHeader
            // 
            this.ClassificationColumnHeader.Text = "Classification";
            this.ClassificationColumnHeader.Width = 0;
            // 
            // UserColumnHeader
            // 
            this.UserColumnHeader.Text = "User";
            this.UserColumnHeader.Width = 68;
            // 
            // CommentColumnHeader
            // 
            this.CommentColumnHeader.Text = "Comment";
            // 
            // TimeStampColumnHeader
            // 
            this.TimeStampColumnHeader.Text = "Local Timestamp";
            this.TimeStampColumnHeader.Width = 105;
            // 
            // DateColumnHeader
            // 
            this.DateColumnHeader.Text = "Server Timestamp";
            this.DateColumnHeader.Width = 118;
            // 
            // StatusColumnHeader
            // 
            this.StatusColumnHeader.Text = "Status";
            this.StatusColumnHeader.Width = 108;
            // 
            // FullNameColumnHeader
            // 
            this.FullNameColumnHeader.Text = "Fullname";
            this.FullNameColumnHeader.Width = 0;
            // 
            // LocalFileNameColumnHeader
            // 
            this.LocalFileNameColumnHeader.Text = "LocalFile";
            this.LocalFileNameColumnHeader.Width = 0;
            // 
            // RepositoryFileNameColumnHeader
            // 
            this.RepositoryFileNameColumnHeader.Text = "RepositoryFile";
            this.RepositoryFileNameColumnHeader.Width = 0;
            // 
            // ExtensionColumnHeader
            // 
            this.ExtensionColumnHeader.Text = "Extension";
            // 
            // MogControl_LibraryListView
            // 
            this.Controls.Add(this.LibraryListView);
            this.Name = "MogControl_LibraryListView";
            this.Size = new System.Drawing.Size(904, 344);
            this.ResumeLayout(false);

		}
		#endregion

		public void DeInitialize()
		{
			if (this.LibraryListView != null &&
				this.LibraryListView.TopItem != null)
			{
				mLastTopItem = this.LibraryListView.TopItem.Text;
			}
		}

		public void Initialize()
		{
			this.LibraryListView.SmallImageList = MOG_ControlsLibrary.Utils.MogUtil_AssetIcons.Images;
			
			// Add our context menu
            mContextMenu = new MOG_ControlsLibrary.Common.MOG_ContextMenu.MogControl_AssetContextMenu("NAME, EXTENSION, CLASSIFICATION, USER, COMMENT, TIMESTAMP, DATE, STATUS, FULLNAME, LOCALFILE, REPOSITORYFILE", this.LibraryListView);
			this.LibraryListView.ContextMenuStrip = mContextMenu.InitializeContextMenu("{LibraryListView}");

			LibraryListView.Items.Clear();
		}

		#region Drag-drop event handlers
		public void MogControl_LibraryListView_DragEnter(object sender, DragEventArgs args)
		{
			// Only accept FileDrops from Windows
			if (args.Data.GetDataPresent(DataFormats.FileDrop))
			{
				args.Effect = args.AllowedEffect;
			}
			else
			{
				args.Effect = DragDropEffects.None;
			}
		}

		public void MogControl_LibraryListView_DragDrop(object sender, DragEventArgs args)
		{
			// Extract the filenames
			string[] filenames = (string[])args.Data.GetData("FileDrop", false);
			if (filenames != null  &&  filenames.Length > 0)
			{
				bool bCopyFiles = true;
				bool bAutoAddFiles = false;
				bool bPromptUser = false;
				bool bCancel = false;

				// Check if thes files are coming from the same spot?
				string classification = this.CurrentClassification;
				string classificationPath = MOG_ControllerLibrary.ConstructPathFromLibraryClassification(classification);
				// Get the common directory scope of the items
				ArrayList items = new ArrayList(filenames);
				string rootPath = MOG_ControllerAsset.GetCommonDirectoryPath("", items);
				if (rootPath.StartsWith(classificationPath))
				{
					bCopyFiles = false;
				}

				// Check if auto import is checked?
				if (this.LibraryExplorer.IsAutoImportChecked())
				{
					// Automatically add the file on the server
					bAutoAddFiles = true;
					bPromptUser = true;

					// Check if these files are already within the library?
					if (MOG_ControllerLibrary.IsPathWithinLibrary(rootPath))
					{
						// Ignore what the user specified and rely on the classification generated from the filenames
						classification = "";
						bPromptUser = false;
						bCopyFiles = false;
					}
				}

				// Promt the user for confirmation before we import these files
				if (bPromptUser)
				{
					// Prompt the user and allow them to cancel
					if (LibraryFileImporter.PromptUserForConfirmation(filenames, classification) == false)
					{
						bCancel = true;
					}
				}

				// Make sure we haven't canceled
				if (!bCancel)
				{
					if (bCopyFiles)
					{
						// Import the files
						List<object> arguments = new List<object>();
						arguments.Add(filenames);
						arguments.Add(classification);
						ProgressDialog progress = new ProgressDialog("Copying Files", "Please wait while the files are copied", LibraryFileImporter.CopyFiles, arguments, true);
						progress.ShowDialog();
					}
				}

				// Make sure we haven't canceled
				if (!bCancel)
				{
					if (bAutoAddFiles)
					{
						// Import the files
						List<object> arguments = new List<object>();
						arguments.Add(filenames);
						arguments.Add(classification);
						ProgressDialog progress = new ProgressDialog("Copying Files", "Please wait while the files are copied", LibraryFileImporter.ImportFiles, arguments, true);
						progress.ShowDialog();
					}
				}

				// Refresh view
				this.LibraryExplorer.Refresh();
			}
		}
		#endregion

		public void Populate(string classification)
		{
			CurrentClassification = classification;
			mCurrentClassificationProperties = new MOG_Properties(CurrentClassification);

			// For speed purposes, create 3 HybridDictionary lists
			// Populate the files that exist on the local hardrive
			string drivePath = Path.Combine(MOG_ControllerLibrary.GetWorkingDirectory(), MOG_Filename.GetClassificationPath(classification));
			string[] files = new string[] { };
			if (DosUtils.DirectoryExistFast(drivePath))
			{
				files = Directory.GetFiles(drivePath);
			}
			HybridDictionary filesOnDisk = new HybridDictionary();
			foreach(string file in files)
			{
				filesOnDisk[Path.GetFileName(file)] = file;
			}
			// Populate the assets that exist in MOG
            ArrayList assets = MOG_ControllerAsset.GetAssetsByClassification(classification);
			HybridDictionary assetsInMOG = new HybridDictionary();
			foreach(MOG_Filename asset in assets)
			{
				assetsInMOG[asset.GetAssetLabel()] = asset;
			}
			// Create a master list
			HybridDictionary masterList = new HybridDictionary();
			foreach(string file in filesOnDisk.Values)
			{
				masterList[Path.GetFileName(file)] = Path.GetFileName(file);
			}
			foreach (MOG_Filename asset in assetsInMOG.Values)
			{
				masterList[asset.GetAssetLabel()] = asset.GetAssetLabel();
			}


			// Rebuild our LibraryListView
			LibraryListView.BeginUpdate();
			mLibrarySortManager.SortEnabled = false;

			LibraryListView.Items.Clear();

			foreach(string file in masterList.Keys)
			{
				// Check if this file is in MOG?
				if (assetsInMOG.Contains(file))
				{
					MOG_Filename asset = assetsInMOG[file] as MOG_Filename;

					// Create the ListView item  for this asset
					ListViewItem item = CreateListViewItemForAsset(asset);
					LibraryListView.Items.Add(item);
				}
				else
				{
					string fullFilename = filesOnDisk[file] as string;
					bool bIsValid = true;

					// Check the classification's inclusion filter.
					if (mCurrentClassificationProperties.FilterInclusions.Length > 0)
					{
						MOG.FilePattern inclusions = new MOG.FilePattern(mCurrentClassificationProperties.FilterInclusions);
						if (inclusions.IsFilePatternMatch(Path.GetFileName(fullFilename)) == false)
						{
							bIsValid = false;
						}
					}
					// Check the classification's exclusion filter.
					if (mCurrentClassificationProperties.FilterExclusions.Length > 0)
					{
						MOG.FilePattern exclusions = new MOG.FilePattern(mCurrentClassificationProperties.FilterExclusions);
						if (exclusions.IsFilePatternMatch(Path.GetFileName(fullFilename)) == true)
						{
							bIsValid = false;
						}
					}

					// Check if we determined this to be a valid file to show?
					if (bIsValid)
					{
						ListViewItem item = CreateListViewItemForFile(fullFilename);
						UpdateListViewItemColors(item, "Unknown");
						LibraryListView.Items.Add(item);
					}
				}
			}

			mLibrarySortManager.SortEnabled = true;
			LibraryListView.EndUpdate();

			// Check if we have a mLastTopItem specified?
			if (mLastTopItem.Length > 0)
			{
				LibraryListView.TopItem = LibraryListView.FindItemWithText(mLastTopItem);
				mLastTopItem = "";
			}
		}

		private void UpdateItem(ListViewItem item)
		{
			string status = "";
			string username = "";
			string comment = "";
			string localTimestamp = "";

			// Find our desired columns
			int statusIdx = FindColumn("Status");
			int userIdx = FindColumn("User");
			int commentIdx = FindColumn("Comment");
			int localTimestampIdx = FindColumn("Local Timestamp");
			int serverTimestampIdx = FindColumn("Server Timestamp");
			int localFileIdx = FindColumn("LocalFile");
			int repositoryFileIdx = FindColumn("RepositoryFile");

			string repositoryFile = item.SubItems[repositoryFileIdx].Text;
			MOG_Filename repositoryAssetFilename = new MOG_Filename(repositoryFile);

			// Check if this file exist locally?
			string localFile = item.SubItems[localFileIdx].Text;
			if (localFile.Length != 0)
			{
				// Obtain the localFile info
				FileInfo fileInfo = new FileInfo(localFile);
				// Does this local file exist?
				if (fileInfo != null && fileInfo.Exists)
				{
					// Compare our local file's timestamp to the server's revision
					localTimestamp = MOG_Time.GetVersionTimestamp(fileInfo.LastWriteTime);
					if (localTimestamp == repositoryAssetFilename.GetVersionTimeStamp())
					{
						// Indicate this item is synced and up-to-date
						status = "Up-to-date";
					}
					else
					{
						// Indicate this item is synced
						status = "Out-of-date";
					}
				}
				else
				{
					// Indicate this item is not synced
					status = "unSynced";
				}
			}
			else
			{
				// Indicate this item is not synced
				status = "unSynced";
			}

			// Check if this file exists in the repository?
			if (repositoryFile.Length != 0)
			{
				// Check the lock statusIdx of the asset
				MOG_Command sourceLock = MOG_ControllerProject.PersistentLock_Query(repositoryAssetFilename.GetAssetFullName());
				if (sourceLock.IsCompleted() && sourceLock.GetCommand() != null)
				{
					MOG_Command lockHolder = sourceLock.GetCommand();

					// Obtain the lock info
					item.ImageIndex = MogUtil_AssetIcons.GetLockedBinaryIcon(repositoryFile);
					username = lockHolder.GetUserName();
					comment = lockHolder.GetDescription();

					// Check if this is locked by me?
					if (username == MOG_ControllerProject.GetUserName())
					{
						status = "CheckedOut";
					}
					else
					{
						status = "Locked";
					}
				}
				else
				{
					// Update this file's icon
					item.ImageIndex = MogUtil_AssetIcons.GetFileIconIndex(repositoryFile);
				}
			}

			// Update the item with the new information
			item.SubItems[statusIdx].Text = status;
			item.SubItems[userIdx].Text = username;
			item.SubItems[commentIdx].Text = comment;
			item.SubItems[localTimestampIdx].Text = MogUtils_StringVersion.VersionToString(localTimestamp);
			item.SubItems[serverTimestampIdx].Text = MogUtils_StringVersion.VersionToString(repositoryAssetFilename.GetVersionTimeStamp());

			// Update the color for this locked item
			UpdateListViewItemColors(item, status);
		}

		private ListViewItem CreateListViewItemForAsset(MOG_Filename asset)
		{
			ListViewItem item = null;

			// Only put the asset in the list if it is actually a library asset
			if (asset.IsLibrary())
			{
				// Make sure we have something valid in our Filename
				if (asset.GetAssetLabel().Length > 0)
				{
					item = new ListViewItem(asset.GetAssetLabel());

					// Get the source imported file
					MOG_Filename repositoryAssetFilename = MOG_ControllerRepository.GetAssetBlessedVersionPath(asset, asset.GetVersionTimeStamp());
					string repositoryFile = MOG_ControllerLibrary.ConstructBlessedFilenameFromAssetName(repositoryAssetFilename);
					string localFile = MOG_ControllerLibrary.ConstructLocalFilenameFromAssetName(repositoryAssetFilename);
                    string extension = DosUtils.PathGetExtension(localFile);

					// Populate the item
                    item.SubItems.Add(extension);	                            // Extension
					item.SubItems.Add(asset.GetAssetClassification());			// Classification
					item.SubItems.Add("");										// User
					item.SubItems.Add("");										// Comment
					item.SubItems.Add("");										// Local TimeStamp
					item.SubItems.Add(asset.GetVersionTimeStampString(""));		// Server Timestamp
					item.SubItems.Add("New");									// Status
					item.SubItems.Add(asset.GetAssetFullName());				// Fullname
					item.SubItems.Add(localFile);								// LocalFile
					item.SubItems.Add(repositoryFile);							// RepositoryFile                    

					// Update the item
					UpdateItem(item);
				}
			}

			return item;
		}

		private ListViewItem CreateListViewItemForFile(string localFile)
		{
			ListViewItem item = null;

			// Only put the asset in the list if it is actually a library asset
			if (MOG_ControllerLibrary.IsPathWithinLibrary(localFile))
			{
				item = new ListViewItem(Path.GetFileName(localFile));

				string classification = MOG_ControllerLibrary.ConstructLibraryClassificationFromPath(localFile);
				string localTimestamp = GetLocalFileTimestamp(localFile);
				string fullname = localFile;
				string status = "Unknown";
                string extension = DosUtils.PathGetExtension(localFile);

				// Populate the item
				item.ImageIndex = MogUtil_AssetIcons.GetFileIconIndex(localFile);
                item.SubItems.Add(extension);               // Extension
				item.SubItems.Add(classification);			// Classification
				item.SubItems.Add("");						// User
				item.SubItems.Add("");						// Comment
				item.SubItems.Add(localTimestamp);			// Local TimeStamp
				item.SubItems.Add("");						// Server Timestamp
				item.SubItems.Add(status);					// Status
				item.SubItems.Add(fullname);				// Fullname
				item.SubItems.Add(localFile);				// LocalFile
				item.SubItems.Add("");						// RepositoryFile
                
				UpdateListViewItemColors(item, status);
			}

			return item;
		}

		private string GetLocalFileTimestamp(string localFile)
		{
			string localTimeStamp = "";

			FileInfo fileInfo = new FileInfo(localFile);
			if (fileInfo != null && fileInfo.Exists)
			{
				localTimeStamp = MOG_Time.FormatTimestamp(MOG_Time.GetVersionTimestamp(fileInfo.LastWriteTime), "");
			}

			return localTimeStamp;
		}

		private void UpdateListViewItemColors(ListViewItem item, string status)
		{
			switch (status)
			{
				case "Unknown":
					item.ForeColor = Color.LightGray;
					item.BackColor = SystemColors.Window;
					break;
				case "Up-to-date":
					item.ForeColor = Color.Black;
					item.BackColor = SystemColors.Window;
					break;
				case "Out-of-date":
					item.ForeColor = Color.DimGray;
					item.BackColor = SystemColors.Window;
					break;
				case "unSynced":
					item.ForeColor = Color.DimGray;
					item.BackColor = SystemColors.Window;
					break;
				case "Locked":
					item.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(246)))), ((int)(((byte)(246)))));
					break;
				case "CheckedOut":
					item.ForeColor = Color.Black;
					item.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(255)))), ((int)(((byte)(242)))));
					break;
				case "New":
					item.ForeColor = Color.DarkBlue;
					item.BackColor = SystemColors.Window;
					break;
				default:
					item.ForeColor = Color.Black;
					item.BackColor = SystemColors.Window;
					break;
			}
		}

		override public void Refresh()
		{
			Populate(CurrentClassification);

			base.Refresh();
		}

		public int FindColumn(string name)
		{
			int x = 0;
			foreach (ColumnHeader column in LibraryListView.Columns)
			{
				if (string.Compare(column.Text, name, true) == 0)
				{
					return x;
				}

				x++;
			}
			
			return 0;
		}
		
		internal int FindListItem(string name)
		{
			int column = FindColumn("Fullname");
			if (column >= 0)
			{
				for (int i = 0; i < LibraryListView.Items.Count; i++)
				{
					ListViewItem item = LibraryListView.Items[i];
					string test = item.SubItems[column].Text;
					
					if (String.Compare(name, test, true) == 0)
					{
						return i;
					}
				}
			}

			return -1;
		}

		protected string GetItemFullName(ListViewItem item)
		{
			int column = FindColumn("Fullname");
			if (column >= 0)
			{
				return item.SubItems[column].Text;
			}

			return null;
		}
		
		public void RefreshItem(MOG_Command command)
		{
			// No reason to bother if they have no library working directory
			if (MOG_ControllerLibrary.GetWorkingDirectory().Length == 0)
			{
				return;
			}

			// Make sure this contains an encapsulated command?
			MOG_Command encapsulatedCommand = command.GetCommand();
			if (encapsulatedCommand != null)
			{
				// No reason to bother if they are in a different project
				if (string.Compare(MOG_ControllerProject.GetProjectName(), encapsulatedCommand.GetProject(), true) != 0)
				{
					return;
				}

				// Check if this encapsulatedCommand contains a valid assetFilename?
				if (encapsulatedCommand.GetAssetFilename().GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
				{
					// No reason to bother if this asset's classification doesn't match their mCurrentClassification
					if (string.Compare(CurrentClassification, encapsulatedCommand.GetAssetFilename().GetAssetClassification(), true) != 0)
					{
						return;
					}
				}

				// Can we find the asset node of this file?
				int itemid = FindListItem(encapsulatedCommand.GetAssetFilename().GetAssetFullName());
				if (itemid == -1)
				{
					// Check if this was a post command?
					if (encapsulatedCommand.GetCommandType() == MOG_COMMAND_TYPE.MOG_COMMAND_Post)
					{
						// Create a new item
						ListViewItem item = CreateListViewItemForAsset(encapsulatedCommand.GetAssetFilename());
						if (item != null)
						{
							LibraryListView.Items.Add(item);
							itemid = FindListItem(encapsulatedCommand.GetAssetFilename().GetAssetFullName());
						}
					}
				}

				// Now check if we finally found our itemid?
				if (itemid != -1)
				{
					ListViewItem item = LibraryListView.Items[itemid];
					if (item != null)
					{
						int classificationIdx = FindColumn("Classification");
						int nameIdx = FindColumn("Name");
						int statusIdx = FindColumn("Status");
						int userIdx = FindColumn("User");
						int commentIdx = FindColumn("Comment");
						int localFileIdx = FindColumn("LocalFile");
						int repositoryFileIdx = FindColumn("RepositoryFile");
						int localTimestampIdx = FindColumn("Local Timestamp");
						int serverTimestampIdx = FindColumn("Server Timestamp");
						int fullname = FindColumn("Fullname");
                        int extensionIdx = FindColumn("Extension");

						// Determin the type of encapsulated command
						switch (encapsulatedCommand.GetCommandType())
						{
							case MOG_COMMAND_TYPE.MOG_COMMAND_LockWriteRelease:
							case MOG_COMMAND_TYPE.MOG_COMMAND_LockReadRelease:
								UpdateItem(item);
								break;

							case MOG_COMMAND_TYPE.MOG_COMMAND_LockWriteRequest:
							case MOG_COMMAND_TYPE.MOG_COMMAND_LockReadRequest:
								string status = "";
								string comment = encapsulatedCommand.GetDescription();
								string username = encapsulatedCommand.GetUserName();
								if (String.Compare(MOG_ControllerProject.GetUserName(), encapsulatedCommand.GetUserName(), true) == 0)
								{
									status = "CheckedOut";
								}
								else
								{
									status = "Locked";
								}

								item.ImageIndex = MogUtil_AssetIcons.GetLockedBinaryIcon(item.SubItems[repositoryFileIdx].Text);
								item.SubItems[commentIdx].Text = comment;
								item.SubItems[userIdx].Text = username;
								item.SubItems[statusIdx].Text = status;

								UpdateListViewItemColors(item, status);
								break;

							case MOG_COMMAND_TYPE.MOG_COMMAND_Post:
								item.SubItems[repositoryFileIdx].Text = MOG_ControllerLibrary.ConstructBlessedFilenameFromAssetName(encapsulatedCommand.GetAssetFilename());
								UpdateItem(item);
								break;

							case MOG_COMMAND_TYPE.MOG_COMMAND_RemoveAssetFromProject:
								// Make sure to remove this file just incase it had been previously synced
								MOG_ControllerLibrary.Unsync(encapsulatedCommand.GetAssetFilename());
								// Proceed to delete this item
								RemoveItem(itemid);
								break;
						}

						// Update the item's colors
						UpdateListViewItemColors(item, item.SubItems[statusIdx].Text);
					}
				}
			}
		}

        private void LibraryListView_DoubleClick(object sender, EventArgs e)
        {
            if (DoubleClickItem != null)
            {
                DoubleClickItem.Invoke(sender, e);
            }

			mContextMenu.MenuItemLibraryEdit_Click(sender, e);
        }

		private void EditLibraryAsset(string librarySource)
		{
			if (File.Exists(librarySource))
			{
				guiCommandLine.ShellSpawn(librarySource);
			}
			else
			{
				MOG_Prompt.PromptMessage("Edit Error", "This asset does not exist or is missing.  CheckOut and try again...");
			}
		}

		private void LibraryListView_ItemDrag(object sender, ItemDragEventArgs e)
		{
			ArrayList items = new ArrayList();

			foreach (ListViewItem item in LibraryListView.SelectedItems)
			{
				string fullname = GetItemFullName(item);
				items.Add(fullname);
			}

			DataObject data = new DataObject("LibraryListItems", items);
			LibraryListView.DoDragDrop(data, DragDropEffects.All);
		}

		public void RemoveItem(int index)
		{
			LibraryListView.Items.RemoveAt(index);
		}

		private void LibraryListView_AfterLabelEdit(object sender, LabelEditEventArgs e)
		{
			if (e.Label != null)
			{
				// Rename the asset
				ListViewItem renamedAsset = LibraryListView.Items[e.Item];
				string fullName = GetItemFullName(renamedAsset);
				string label = renamedAsset.SubItems[FindColumn("Name")].Text;
                string extension = DosUtils.PathGetExtension(fullName);

				string rename = fullName.Replace(label, e.Label);

				if (DosUtils.FileExistFast(rename))
				{
					MOG_Prompt.PromptMessage("Rename Error", "Cannot rename (" + label + ") to (" + e.Label + ") because this asset already exists!");
					e.CancelEdit = true;
				}
				else
				{
					if (!DosUtils.RenameFast(fullName, rename, false))
					{
						MOG_Prompt.PromptMessage("Rename Error", DosUtils.GetLastError());
						e.CancelEdit = true;
					}
					else
					{
						// Update the full filename
						renamedAsset.SubItems[FindColumn("Fullname")].Text = rename;
                        renamedAsset.SubItems[FindColumn("Extension")].Text = extension;
					}
				}
			}
		}

		private void LibraryListView_BeforeLabelEdit(object sender, LabelEditEventArgs e)
		{
			ListViewItem renamedAsset = LibraryListView.Items[e.Item];
			string fullName = GetItemFullName(renamedAsset);

			// We can only rename assets that are not checked into MOG
			if (fullName.StartsWith(MOG_ControllerLibrary.GetWorkingDirectory()) == false)
			{
				e.CancelEdit = true;
			}
		}
	}
}
