using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Net.Mail;
using System.Threading;

using MOG_Client.Client_Gui;
using MOG;
using MOG.TIME;
using MOG.COMMAND;
using MOG.FILENAME;
using MOG.PROJECT;
using MOG.USER;
using MOG.REPORT;
using MOG.CONTROLLER.CONTROLLERPROJECT;

using MOG_Client.Client_Mog_Utilities.AssetOptions;
using MOG_Client.Client_Utilities;

using MOG_ControlsLibrary.Common.MogControl_RepositoryTreeViews;
using MOG_ControlsLibrary.Controls;
using MOG_ControlsLibrary.Utils;
using MOG_ControlsLibrary.MogUtils_Settings;
using MOG.PROPERTIES;
using MOG.CONTROLLER.CONTROLLERSYNCDATA;
using System.IO;
using MOG.DOSUTILS;
using MOG.CONTROLLER.CONTROLLERINBOX;
using MOG.CONTROLLER.CONTROLLERASSET;
using MOG.CONTROLLER.CONTROLLERREPOSITORY;

namespace MOG_Client
{
	/// <summary>
	/// Summary description for FormBlessInfo.
	/// </summary>
	public class BlessInfoForm : System.Windows.Forms.Form
	{
		private bool mIsBlessingAsset;
		private bool mBlessValidated;
		private MogMainForm mainForm;
		private Thread mSendMail;
        private string mMailServer;
		private MailMessage mMessage;

		public System.Windows.Forms.RichTextBox BlessInfoBlessCommentRichTextBox;
		private System.Windows.Forms.Button BlessInfoOkButton;
		private System.Windows.Forms.Button BlessInfoCancelButton;
		private System.Windows.Forms.Panel BottomPanel;
		private System.Windows.Forms.Panel BlessInfoBlessCommentPanel;
		private System.Windows.Forms.Panel BlessInfoBlessCommentEmailPanel;
		private System.Windows.Forms.Label BlessInfoBlessCommentEmaillabel;
		private System.Windows.Forms.Button button1;
		public System.Windows.Forms.TreeView BlessInfoBlessCommentEmailCheckedTreeView;
		public System.Windows.Forms.ListView BlessInfoBlessFilesCheckedListView;
		private System.Windows.Forms.ColumnHeader BlessAssetColumnHeader;
		private System.Windows.Forms.Panel BlessInfoDetailsPanel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label BlessTargetLabel;
		private System.Windows.Forms.Label BlessProjectBranchLabel;
		private System.Windows.Forms.Label BlessProjectLabel;
		internal Splitter BlessSplitter;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ToolTip MOGToolTip;
		private CheckBox BlessMaintainLockCheckBox;
		private System.ComponentModel.IContainer components;
		private ColumnHeader BlessEmailColumnHeader;
		private PictureBox pictureBox1;
		private ContextMenuStrip BlessFilesContextMenuStrip;
		private ToolStripMenuItem checkToolStripMenuItem;
		private ToolStripMenuItem unCheckToolStripMenuItem;
		private ToolStripSeparator toolStripMenuItem1;
		private ToolStripMenuItem resolveToolStripMenuItem;

		private bool mMaintainLock;
		public bool MaintainLock
		{
			get { return mMaintainLock; }
			set { mMaintainLock = value; }
		}	

		public BlessInfoForm(ArrayList items, bool bIsBlessingAsset)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			mIsBlessingAsset = bIsBlessingAsset;
			mBlessValidated = true;
			BlessInfoBlessFilesCheckedListView.SmallImageList = MogUtil_AssetIcons.Images;
			BlessInfoBlessFilesCheckedListView.LargeImageList = MogUtil_AssetIcons.Images;

			foreach (Mog_BaseTag tag in items)
			{
				if (tag.Execute)
				{
					MOG_Filename assetName = new MOG_Filename(tag.FullFilename);

					// If we have no version (which should be the case for tree nodes)...
					if (assetName.IsBlessed() && assetName.GetVersionTimeStamp().Length == 0)
					{
						// Get a valid version
						assetName = MOG_ControllerProject.GetAssetCurrentBlessedVersionPath( assetName );
					}

					ListViewItem blessItem = CreateBlessItem(assetName);
					BlessInfoBlessFilesCheckedListView.Items.Add(blessItem);
				}
			}

			if (mIsBlessingAsset)
			{
				AddEmails();
			}
			else
			{
				BlessInfoBlessCommentEmailPanel.Visible = false;
			}


			// Set the details
			BlessProjectLabel.Text = MOG_ControllerProject.GetProjectName();
			BlessProjectBranchLabel.Text = MOG_ControllerProject.GetBranchName();
			BlessTargetLabel.Text = MOG_ControllerInbox.GetAssetBlessTarget(null, null);
			
			// Color change branch if we are not in current
			if (string.Compare(MOG_ControllerProject.GetBranchName(), "Current", true) != 0)
			{
				BlessProjectBranchLabel.ForeColor = Color.DarkBlue;
			}
			else
			{
				BlessProjectBranchLabel.ForeColor = Color.FromKnownColor(KnownColor.ControlText);
			}

			// Color change target if we are not in masterData
			if (string.Compare(MOG_ControllerProject.GetUser().GetBlessTarget(), "MasterData", true) != 0)
			{
				BlessTargetLabel.ForeColor = Color.DarkBlue;
			}
			else
			{
				BlessTargetLabel.ForeColor = Color.FromKnownColor(KnownColor.ControlText);
			}

			// Check if this is the bless dialog?
			if (mIsBlessingAsset)
			{
				// Make sure these are visible because this is a bless dialog
				BlessProjectBranchLabel.Visible = true;
				label2.Visible = true;
				BlessTargetLabel.Visible = true;
				label3.Visible = true;
			}
			else
			{
				// We can hide these because this isn't a bless dialog
				BlessProjectBranchLabel.Visible = false;
				label2.Visible = false;
				BlessTargetLabel.Visible = false;
				label3.Visible = false;
			}

			// Disable the notify treeView if the Smtp server is not specified
			if (MOG_ControllerProject.GetProject().GetConfigFile().KeyExist("PROJECT", "EmailSmtp") == false)
			{
				this.BlessInfoBlessCommentEmailCheckedTreeView.Enabled = false;
			}

            // Prep this feature for MOG Light versions
            if (MOG_Main.IsUnlicensed())
            {
                //BlessInfoBlessCommentEmaillabel.Text = "Email notification to users or groups:";
                BlessInfoBlessCommentEmaillabel.Text = "Email Notifications enabled in 'Full Version'";
                this.BlessInfoBlessCommentEmailCheckedTreeView.Enabled = false;
            }            
						
			if (mIsBlessingAsset)
			{
				// Pre-load notifies
				PreLoadNotifies();
			}
			else
			{
				string fileNames = "";
				foreach( Mog_BaseTag tag in items )
				{
					fileNames += tag.FullFilename + "\r\n\t";
				}

				// Add a default comment if one is not given
				BlessInfoBlessCommentRichTextBox.Text = "Sent by user:" + MOG_ControllerProject.GetUser().GetUserName()	+ "\r\n";
			}

			// Select the comments text
			SetEditCaret();

			// Always check the status of the OK button
			EnableOK();
		}

		private ListViewItem CreateBlessItem(MOG_Filename assetName)
		{
			// Obtain the properties of this asset so we can perform a few additional checks
			MOG_Properties properties = new MOG_Properties(assetName);

			ListViewItem blessItem = new ListViewItem(assetName.GetAssetLabel());
			blessItem.SubItems.Add(properties.BlessEmailNotify);
			blessItem.SubItems.Add(assetName.GetOriginalFilename());
			blessItem.ImageIndex = MogUtil_AssetIcons.GetAssetIconIndex(assetName.GetOriginalFilename());

			UpdateItem(blessItem, assetName, properties);

			return blessItem;
		}

		private void UpdateItem(ListViewItem item, MOG_Filename assetName, MOG_Properties properties)
		{
			string displayLabel = assetName.GetAssetLabel();
			bool bChecked = true;
			Color color = Color.Black;

			// Check if this is an inbox asset?
			if (assetName.IsDrafts() || assetName.IsInbox())
			{
				// Obtain any lock information for this asset
				MOG_Command lockInfo = MOG_ControllerProject.PersistentLock_Query(assetName.GetAssetFullName());
				if (lockInfo.IsCompleted() &&
					lockInfo.GetCommand() != null)
				{
					// Make sure this is not already me
					if (string.Compare(lockInfo.GetCommand().GetUserName(), MOG_ControllerProject.GetUser().GetUserName(), true) != 0)
					{
						// Show who currently has the lock
						displayLabel += "   (Locked by " + lockInfo.GetCommand().GetUserName() + ")";

						// Check if this asset was instructed to maintain the lock when blessed?  and
						// Check if this asset's creator is still the current lock holder?
						if (properties.MaintainLock == true &&
							string.Compare(properties.Creator, lockInfo.GetCommand().GetUserName(), true) == 0)
						{
							// Since the creator of this asset still has the lock, let this though
							bChecked = true;
						}
						// Check if we are trying to bless this asset from the current lock holder's inbox?  and
						// Make sure we have legitimate permission to do such a thing?
						else if (string.Compare(properties.Owner, lockInfo.GetCommand().GetUserName(), true) == 0 &&
								 MOG_ControllerProject.GetPrivileges().BlessAssetFromWithinOtherInbox)
						{
							// Since the creator of this asset still has the lock, let this though
							bChecked = true;
						}
						// Check if we are not trying to bless this asset?
						else if (!mIsBlessingAsset)
						{
							// We can allow this because it isn't a bless but lets still warn the user
							color = Color.Salmon;
							bChecked = true;
						}
						else
						{
							// Set the color to red so they will not be able to check this item
							color = Color.Red;
							bChecked = false;
						}

						BlessMaintainLockCheckBox.Enabled = false;
						BlessMaintainLockCheckBox.Checked = false;
					}
					else
					{
						BlessMaintainLockCheckBox.Enabled = true;
						if (properties.MaintainLock)
						{
							BlessMaintainLockCheckBox.Checked = true;
						}
					}
				}

				// Check if this is an unblessable asset?
				if (mIsBlessingAsset && properties.UnBlessable)
				{
					bChecked = false;
					displayLabel += "   (UnBlessable - Copied assets are unblessable by default)";
					// Set the color to red so they will not be able to check this item
					color = Color.Red;
				}
				// Make sure this has a package assignment if this is a packaged asset?
				else if (properties.IsPackagedAsset &&
						 MOG_ControllerAsset.ValidateAsset_HasPackageAssignment(properties) == false)
				{
					bChecked = false;
					displayLabel += "   (Missing Package Assignment)";
					// Set the color to red so they will not be able to check this item
					color = Color.Red;
				}
				// Make sure this has a unique package assignment if this is a packaged asset?
				else if (properties.IsPackagedAsset &&
						 MOG_ControllerAsset.ValidateAsset_IsUniquePackageAssignment(properties) == false)
				{
					bChecked = false;

					// Check if we have any colliding assets?
					ArrayList collidingAssets = MOG_ControllerAsset.ValidateAsset_GetCollidingPackageAssignments(properties);
					if (collidingAssets.Count > 0)
					{
						string collidingAssetsText = "";
						foreach(MOG_Filename collidingAssetFilename in collidingAssets)
						{
							if (collidingAssetsText.Length > 0)
							{
								collidingAssetsText += ", ";
							}
							collidingAssetsText += collidingAssetFilename.GetAssetFullName();
						}
						displayLabel += "   (Package Assignment Collision: " + collidingAssetsText + ")";

						// Set the color to red so they will not be able to check this item
						color = Color.Red;
					}
				}
				// Make sure this has a valid package assignment if this is a packaged asset?
				else if (properties.IsPackagedAsset &&
						 MOG_ControllerAsset.ValidateAsset_AllAssignedPackagesExist(properties) == false)
				{
					bChecked = false;

					// Check if we have any missing packages?
					ArrayList missingPackages = MOG_ControllerAsset.ValidateAsset_GetMissingPackages(properties);
					if (missingPackages.Count > 0)
					{
						string missingPackagesText = "";
						foreach (MOG_Filename missingPackageFilename in missingPackages)
						{
							if (missingPackagesText.Length > 0)
							{
								missingPackagesText += ", ";
							}
							missingPackagesText += missingPackageFilename.GetAssetFullName();
						}
						displayLabel += "   (Package Doesn't Exist: " + missingPackagesText + ")";

						// Set the color to red so they will not be able to check this item
						color = Color.Red;
					}
				}
				// Make sure this has a unique sync target if this asset gets synced?
				else if (properties.SyncFiles &&
						 MOG_ControllerAsset.ValidateAsset_IsUniqueSyncTarget(properties) == false)
				{
					bChecked = false;

					// Check if we have any colliding assets?
					ArrayList collidingAssets = MOG_ControllerAsset.ValidateAsset_GetCollidingSyncTargets(properties);
					if (collidingAssets.Count > 0)
					{
						string collidingAssetsText = "";
						foreach(MOG_Filename collidingAssetFilename in collidingAssets)
						{
							if (collidingAssetsText.Length > 0)
							{
								collidingAssetsText += ", ";
							}
							collidingAssetsText += collidingAssetFilename.GetAssetFullName();
						}
						displayLabel += "   (SyncTarget Collision: " + collidingAssetsText + ")";

						// Set the color to red so they will not be able to check this item
						color = Color.Red;
					}
				}
				else
				{
					if (!properties.UnBlessable)
					{
						// Check if we should perform the out-of-date verification?
						if (properties.OutofdateVerification)
						{
							bool bIsOutofdate = false;
							String currentRepositoryRevision = MOG_ControllerProject.GetAssetCurrentVersionTimeStamp(assetName);

							// Check if the asset's PreviousRevision is different than the master repository revision?
							if (string.Compare(properties.PreviousRevision, currentRepositoryRevision, true) != 0)
							{
								// Check if the asset's PreviousRevision is out of date with the master repository revision?
								if (properties.PreviousRevision.Length > 0 &&
									string.Compare(properties.PreviousRevision, currentRepositoryRevision, true) < 0)
								{
									bIsOutofdate = true;
								}
								// Check if this asset's CreatedTime is stake compared to the master repository revision?
								if (string.Compare(properties.CreatedTime, currentRepositoryRevision, true) < 0)
								{
									bIsOutofdate = true;
								}

								// Check if we determined this sucka is out-of-date?
								if (bIsOutofdate)
								{
									MOG_Filename blessedAssetFilename = MOG_ControllerRepository.GetAssetBlessedVersionPath(assetName, currentRepositoryRevision);
									MOG_Properties blessedProperties = new MOG_Properties(blessedAssetFilename);
									displayLabel += "   (Out-of-date     BlessedBy: " + blessedProperties.Owner + "     " + MOG_Time.FormatTimestamp(currentRepositoryRevision, "") + ")";

									// Check the user's privilege
									MOG_Privileges privileges = MOG_ControllerProject.GetPrivileges();
									if (privileges.GetUserPrivilege(MOG_ControllerProject.GetUserName(), MOG_PRIVILEGE.BlessOutofdateAssets))
									{
										color = Color.Salmon;
										bChecked = true;
									}
									else
									{
										// Set the color to red so they will not be able to check this item
										color = Color.Red;
										bChecked = false;
									}
								}
							}
						}

						// Does this asset have a blessTarget?
						string blessTarget = MOG_ControllerInbox.GetAssetBlessTarget(assetName, properties);
						if (blessTarget.Length > 0)
						{
							displayLabel += "   (BlessTarget: " + blessTarget + ")";
						}

						// Check if this asset should have been verified in the user's local workspace?
						if (properties.LocalVerificationBeforeBless)
						{
							// See if this asset exists in the local workspace?
							MOG_ControllerSyncData localWorkspace = MOG_ControllerProject.GetCurrentSyncDataController();
							if (localWorkspace != null)
							{
								// Build the expected path to this asset in our local workspace updated tray
								MOG_Filename workspaceAsset = MOG_Filename.GetLocalUpdatedTrayFilename(localWorkspace.GetSyncDirectory(), assetName);
								// Check if this is a native asset?  and
								// Check if this is a syncing asset?  and
								// Check the timestamp of the local file to ensure this drafts asset is current
								if (properties.NativeDataType &&
									properties.SyncFiles &&
									!localWorkspace.LocalFileVerification(properties))
								{
									bChecked = false;
									displayLabel += "   (Doesn't Match Local File)";
									// Only change the color if it is still the default black
									if (color == Color.Black)
									{
										color = Color.Salmon;
									}
								}
								// Check if it is missing from the local workspace?
								else if (!DosUtils.DirectoryExistFast(workspaceAsset.GetEncodedFilename()))
								{
									bChecked = false;
									displayLabel += "   (Hasn't been updated locally)";
									// Only change the color if it is still the default black
									if (color == Color.Black)
									{
										color = Color.Salmon;
									}
								}
							}
						}

						// Check for any conflicting inbox assets
						ArrayList conflicts = MOG_ControllerInbox.GetInboxConflictsForAsset(assetName, properties.Creator);
						if (conflicts.Count > 0)
						{
							foreach (string conflict in conflicts)
							{
								displayLabel += "   (Conflict: " + conflict + ")";
								// Only change the color if it is still the default black
								if (color == Color.Black)
								{
									color = Color.Salmon;
									bChecked = false;
								}
							}
						}
					}
				}
			}

			// Update the item
			item.Text = displayLabel;
			item.Checked = bChecked;
			item.ForeColor = color;
		}

		public BlessInfoForm(ListView.SelectedListViewItemCollection items, MogMainForm main)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			mainForm = main;
			BlessInfoBlessFilesCheckedListView.SmallImageList = MogUtil_AssetIcons.Images;
			BlessInfoBlessFilesCheckedListView.LargeImageList = MogUtil_AssetIcons.Images;

			// This code will only work in the inbox :(
			foreach(ListViewItem item in items)
			{
				MOG_Filename assetName = new MOG_Filename(item.SubItems[(int)guiAssetManager.AssetBoxColumns.FULLNAME].Text);

				ListViewItem blessItem = CreateBlessItem(assetName);
				
				BlessInfoBlessFilesCheckedListView.Items.Add(blessItem);
			}

			AddEmails();

			// Select the comments text
			SetEditCaret();

			// Pre-load notifies
			PreLoadNotifies();

			// Always check the status of the OK button
			EnableOK();
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BlessInfoForm));
			this.BlessInfoBlessCommentRichTextBox = new System.Windows.Forms.RichTextBox();
			this.BlessInfoOkButton = new System.Windows.Forms.Button();
			this.BlessInfoCancelButton = new System.Windows.Forms.Button();
			this.BottomPanel = new System.Windows.Forms.Panel();
			this.button1 = new System.Windows.Forms.Button();
			this.BlessInfoBlessCommentPanel = new System.Windows.Forms.Panel();
			this.label4 = new System.Windows.Forms.Label();
			this.BlessInfoBlessCommentEmailPanel = new System.Windows.Forms.Panel();
			this.BlessInfoBlessCommentEmailCheckedTreeView = new System.Windows.Forms.TreeView();
			this.BlessInfoBlessCommentEmaillabel = new System.Windows.Forms.Label();
			this.BlessInfoBlessFilesCheckedListView = new System.Windows.Forms.ListView();
			this.BlessAssetColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.BlessEmailColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.BlessFilesContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.checkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.unCheckToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.resolveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.BlessInfoDetailsPanel = new System.Windows.Forms.Panel();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.BlessMaintainLockCheckBox = new System.Windows.Forms.CheckBox();
			this.BlessTargetLabel = new System.Windows.Forms.Label();
			this.BlessProjectBranchLabel = new System.Windows.Forms.Label();
			this.BlessProjectLabel = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.BlessSplitter = new System.Windows.Forms.Splitter();
			this.MOGToolTip = new System.Windows.Forms.ToolTip(this.components);
			this.BottomPanel.SuspendLayout();
			this.BlessInfoBlessCommentPanel.SuspendLayout();
			this.BlessInfoBlessCommentEmailPanel.SuspendLayout();
			this.BlessFilesContextMenuStrip.SuspendLayout();
			this.BlessInfoDetailsPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// BlessInfoBlessCommentRichTextBox
			// 
			this.BlessInfoBlessCommentRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.BlessInfoBlessCommentRichTextBox.Location = new System.Drawing.Point(0, 16);
			this.BlessInfoBlessCommentRichTextBox.Name = "BlessInfoBlessCommentRichTextBox";
			this.BlessInfoBlessCommentRichTextBox.Size = new System.Drawing.Size(459, 213);
			this.BlessInfoBlessCommentRichTextBox.TabIndex = 1;
			this.BlessInfoBlessCommentRichTextBox.Text = "";
			this.MOGToolTip.SetToolTip(this.BlessInfoBlessCommentRichTextBox, "Enter comments to be saved with the new assets checked above within the MOG repos" +
					"itory.");
			// 
			// BlessInfoOkButton
			// 
			this.BlessInfoOkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.BlessInfoOkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.BlessInfoOkButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.BlessInfoOkButton.Location = new System.Drawing.Point(483, 5);
			this.BlessInfoOkButton.Name = "BlessInfoOkButton";
			this.BlessInfoOkButton.Size = new System.Drawing.Size(80, 23);
			this.BlessInfoOkButton.TabIndex = 3;
			this.BlessInfoOkButton.Text = "OK";
			this.MOGToolTip.SetToolTip(this.BlessInfoOkButton, "Hit CTRL-Enter to activate this button without needing to click on it");
			this.BlessInfoOkButton.Click += new System.EventHandler(this.BlessInfoOkButton_Click);
			// 
			// BlessInfoCancelButton
			// 
			this.BlessInfoCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.BlessInfoCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.BlessInfoCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.BlessInfoCancelButton.Location = new System.Drawing.Point(571, 5);
			this.BlessInfoCancelButton.Name = "BlessInfoCancelButton";
			this.BlessInfoCancelButton.Size = new System.Drawing.Size(80, 23);
			this.BlessInfoCancelButton.TabIndex = 4;
			this.BlessInfoCancelButton.Text = "Cancel";
			this.MOGToolTip.SetToolTip(this.BlessInfoCancelButton, "You may also hit ESC to escape out of this form");
			// 
			// BottomPanel
			// 
			this.BottomPanel.Controls.Add(this.button1);
			this.BottomPanel.Controls.Add(this.BlessInfoOkButton);
			this.BottomPanel.Controls.Add(this.BlessInfoCancelButton);
			this.BottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.BottomPanel.Location = new System.Drawing.Point(0, 388);
			this.BottomPanel.Name = "BottomPanel";
			this.BottomPanel.Size = new System.Drawing.Size(659, 32);
			this.BottomPanel.TabIndex = 4;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(0, 24);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(8, 8);
			this.button1.TabIndex = 3;
			this.button1.Text = "button1";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// BlessInfoBlessCommentPanel
			// 
			this.BlessInfoBlessCommentPanel.Controls.Add(this.BlessInfoBlessCommentRichTextBox);
			this.BlessInfoBlessCommentPanel.Controls.Add(this.label4);
			this.BlessInfoBlessCommentPanel.Controls.Add(this.BlessInfoBlessCommentEmailPanel);
			this.BlessInfoBlessCommentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.BlessInfoBlessCommentPanel.Location = new System.Drawing.Point(0, 159);
			this.BlessInfoBlessCommentPanel.Name = "BlessInfoBlessCommentPanel";
			this.BlessInfoBlessCommentPanel.Size = new System.Drawing.Size(659, 229);
			this.BlessInfoBlessCommentPanel.TabIndex = 6;
			// 
			// label4
			// 
			this.label4.Dock = System.Windows.Forms.DockStyle.Top;
			this.label4.Location = new System.Drawing.Point(0, 0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(459, 16);
			this.label4.TabIndex = 2;
			this.label4.Text = "Comments:";
			this.MOGToolTip.SetToolTip(this.label4, "Enter comments to be saved with the new assets checked above within the MOG repos" +
					"itory.");
			// 
			// BlessInfoBlessCommentEmailPanel
			// 
			this.BlessInfoBlessCommentEmailPanel.Controls.Add(this.BlessInfoBlessCommentEmailCheckedTreeView);
			this.BlessInfoBlessCommentEmailPanel.Controls.Add(this.BlessInfoBlessCommentEmaillabel);
			this.BlessInfoBlessCommentEmailPanel.Dock = System.Windows.Forms.DockStyle.Right;
			this.BlessInfoBlessCommentEmailPanel.Location = new System.Drawing.Point(459, 0);
			this.BlessInfoBlessCommentEmailPanel.Name = "BlessInfoBlessCommentEmailPanel";
			this.BlessInfoBlessCommentEmailPanel.Size = new System.Drawing.Size(200, 229);
			this.BlessInfoBlessCommentEmailPanel.TabIndex = 1;
			// 
			// BlessInfoBlessCommentEmailCheckedTreeView
			// 
			this.BlessInfoBlessCommentEmailCheckedTreeView.CheckBoxes = true;
			this.BlessInfoBlessCommentEmailCheckedTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.BlessInfoBlessCommentEmailCheckedTreeView.FullRowSelect = true;
			this.BlessInfoBlessCommentEmailCheckedTreeView.HotTracking = true;
			this.BlessInfoBlessCommentEmailCheckedTreeView.Location = new System.Drawing.Point(0, 16);
			this.BlessInfoBlessCommentEmailCheckedTreeView.Name = "BlessInfoBlessCommentEmailCheckedTreeView";
			this.BlessInfoBlessCommentEmailCheckedTreeView.Size = new System.Drawing.Size(200, 213);
			this.BlessInfoBlessCommentEmailCheckedTreeView.Sorted = true;
			this.BlessInfoBlessCommentEmailCheckedTreeView.TabIndex = 2;
			// 
			// BlessInfoBlessCommentEmaillabel
			// 
			this.BlessInfoBlessCommentEmaillabel.Dock = System.Windows.Forms.DockStyle.Top;
			this.BlessInfoBlessCommentEmaillabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.BlessInfoBlessCommentEmaillabel.Location = new System.Drawing.Point(0, 0);
			this.BlessInfoBlessCommentEmaillabel.Name = "BlessInfoBlessCommentEmaillabel";
			this.BlessInfoBlessCommentEmaillabel.Size = new System.Drawing.Size(200, 16);
			this.BlessInfoBlessCommentEmaillabel.TabIndex = 1;
			this.BlessInfoBlessCommentEmaillabel.Text = "Email notification to users or groups:";
			this.BlessInfoBlessCommentEmaillabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// BlessInfoBlessFilesCheckedListView
			// 
			this.BlessInfoBlessFilesCheckedListView.CheckBoxes = true;
			this.BlessInfoBlessFilesCheckedListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.BlessAssetColumnHeader,
            this.BlessEmailColumnHeader});
			this.BlessInfoBlessFilesCheckedListView.ContextMenuStrip = this.BlessFilesContextMenuStrip;
			this.BlessInfoBlessFilesCheckedListView.Dock = System.Windows.Forms.DockStyle.Top;
			this.BlessInfoBlessFilesCheckedListView.FullRowSelect = true;
			this.BlessInfoBlessFilesCheckedListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.BlessInfoBlessFilesCheckedListView.Location = new System.Drawing.Point(0, 64);
			this.BlessInfoBlessFilesCheckedListView.Name = "BlessInfoBlessFilesCheckedListView";
			this.BlessInfoBlessFilesCheckedListView.Size = new System.Drawing.Size(659, 90);
			this.BlessInfoBlessFilesCheckedListView.TabIndex = 0;
			this.BlessInfoBlessFilesCheckedListView.UseCompatibleStateImageBehavior = false;
			this.BlessInfoBlessFilesCheckedListView.View = System.Windows.Forms.View.Details;
			this.BlessInfoBlessFilesCheckedListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.BlessInfoBlessFilesCheckedListView_ItemChecked);
			// 
			// BlessAssetColumnHeader
			// 
			this.BlessAssetColumnHeader.Text = "Asset Name";
			this.BlessAssetColumnHeader.Width = 464;
			// 
			// BlessEmailColumnHeader
			// 
			this.BlessEmailColumnHeader.Text = "Auto Email Notifications";
			this.BlessEmailColumnHeader.Width = 188;
			// 
			// BlessFilesContextMenuStrip
			// 
			this.BlessFilesContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkToolStripMenuItem,
            this.unCheckToolStripMenuItem,
            this.toolStripMenuItem1,
            this.resolveToolStripMenuItem});
			this.BlessFilesContextMenuStrip.Name = "BlessFilesContextMenuStrip";
			this.BlessFilesContextMenuStrip.Size = new System.Drawing.Size(128, 76);
			// 
			// checkToolStripMenuItem
			// 
			this.checkToolStripMenuItem.Enabled = false;
			this.checkToolStripMenuItem.Name = "checkToolStripMenuItem";
			this.checkToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
			this.checkToolStripMenuItem.Text = "Check";
			// 
			// unCheckToolStripMenuItem
			// 
			this.unCheckToolStripMenuItem.Enabled = false;
			this.unCheckToolStripMenuItem.Name = "unCheckToolStripMenuItem";
			this.unCheckToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
			this.unCheckToolStripMenuItem.Text = "UnCheck";
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(124, 6);
			// 
			// resolveToolStripMenuItem
			// 
			this.resolveToolStripMenuItem.Name = "resolveToolStripMenuItem";
			this.resolveToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
			this.resolveToolStripMenuItem.Text = "Resolve";
			this.resolveToolStripMenuItem.Click += new System.EventHandler(this.resolveToolStripMenuItem_Click);
			// 
			// BlessInfoDetailsPanel
			// 
			this.BlessInfoDetailsPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(228)))), ((int)(((byte)(239)))));
			this.BlessInfoDetailsPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.BlessInfoDetailsPanel.Controls.Add(this.pictureBox1);
			this.BlessInfoDetailsPanel.Controls.Add(this.BlessMaintainLockCheckBox);
			this.BlessInfoDetailsPanel.Controls.Add(this.BlessTargetLabel);
			this.BlessInfoDetailsPanel.Controls.Add(this.BlessProjectBranchLabel);
			this.BlessInfoDetailsPanel.Controls.Add(this.BlessProjectLabel);
			this.BlessInfoDetailsPanel.Controls.Add(this.label3);
			this.BlessInfoDetailsPanel.Controls.Add(this.label2);
			this.BlessInfoDetailsPanel.Controls.Add(this.label1);
			this.BlessInfoDetailsPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.BlessInfoDetailsPanel.Location = new System.Drawing.Point(0, 0);
			this.BlessInfoDetailsPanel.Name = "BlessInfoDetailsPanel";
			this.BlessInfoDetailsPanel.Size = new System.Drawing.Size(659, 64);
			this.BlessInfoDetailsPanel.TabIndex = 8;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = global::MOG_Client.Properties.Resources.MogBlessLarge;
			this.pictureBox1.Location = new System.Drawing.Point(4, 8);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(48, 48);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pictureBox1.TabIndex = 11;
			this.pictureBox1.TabStop = false;
			// 
			// BlessMaintainLockCheckBox
			// 
			this.BlessMaintainLockCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.BlessMaintainLockCheckBox.AutoSize = true;
			this.BlessMaintainLockCheckBox.BackColor = System.Drawing.Color.Transparent;
			this.BlessMaintainLockCheckBox.Enabled = false;
			this.BlessMaintainLockCheckBox.Location = new System.Drawing.Point(562, 4);
			this.BlessMaintainLockCheckBox.Name = "BlessMaintainLockCheckBox";
			this.BlessMaintainLockCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.BlessMaintainLockCheckBox.Size = new System.Drawing.Size(89, 17);
			this.BlessMaintainLockCheckBox.TabIndex = 10;
			this.BlessMaintainLockCheckBox.Text = "Maintain lock";
			this.BlessMaintainLockCheckBox.UseVisualStyleBackColor = false;
			this.BlessMaintainLockCheckBox.CheckedChanged += new System.EventHandler(this.BlessMaintainLockCheckBox_CheckedChanged);
			// 
			// BlessTargetLabel
			// 
			this.BlessTargetLabel.BackColor = System.Drawing.Color.Transparent;
			this.BlessTargetLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.BlessTargetLabel.Location = new System.Drawing.Point(176, 40);
			this.BlessTargetLabel.Name = "BlessTargetLabel";
			this.BlessTargetLabel.Size = new System.Drawing.Size(432, 16);
			this.BlessTargetLabel.TabIndex = 5;
			this.BlessTargetLabel.Text = "Target User";
			this.BlessTargetLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// BlessProjectBranchLabel
			// 
			this.BlessProjectBranchLabel.BackColor = System.Drawing.Color.Transparent;
			this.BlessProjectBranchLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.BlessProjectBranchLabel.Location = new System.Drawing.Point(176, 24);
			this.BlessProjectBranchLabel.Name = "BlessProjectBranchLabel";
			this.BlessProjectBranchLabel.Size = new System.Drawing.Size(432, 16);
			this.BlessProjectBranchLabel.TabIndex = 4;
			this.BlessProjectBranchLabel.Text = "Project Branch";
			this.BlessProjectBranchLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// BlessProjectLabel
			// 
			this.BlessProjectLabel.BackColor = System.Drawing.Color.Transparent;
			this.BlessProjectLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.BlessProjectLabel.Location = new System.Drawing.Point(176, 8);
			this.BlessProjectLabel.Name = "BlessProjectLabel";
			this.BlessProjectLabel.Size = new System.Drawing.Size(432, 16);
			this.BlessProjectLabel.TabIndex = 3;
			this.BlessProjectLabel.Text = "Project Name";
			this.BlessProjectLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label3
			// 
			this.label3.BackColor = System.Drawing.Color.Transparent;
			this.label3.Location = new System.Drawing.Point(66, 40);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(104, 16);
			this.label3.TabIndex = 2;
			this.label3.Text = "User Bless Target:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// label2
			// 
			this.label2.BackColor = System.Drawing.Color.Transparent;
			this.label2.Location = new System.Drawing.Point(66, 24);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(104, 16);
			this.label2.TabIndex = 1;
			this.label2.Text = "Branch:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.Color.Transparent;
			this.label1.Location = new System.Drawing.Point(66, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(104, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Project:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// BlessSplitter
			// 
			this.BlessSplitter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.BlessSplitter.Dock = System.Windows.Forms.DockStyle.Top;
			this.BlessSplitter.Location = new System.Drawing.Point(0, 154);
			this.BlessSplitter.Name = "BlessSplitter";
			this.BlessSplitter.Size = new System.Drawing.Size(659, 5);
			this.BlessSplitter.TabIndex = 9;
			this.BlessSplitter.TabStop = false;
			// 
			// BlessInfoForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.BlessInfoCancelButton;
			this.ClientSize = new System.Drawing.Size(659, 420);
			this.Controls.Add(this.BlessInfoBlessCommentPanel);
			this.Controls.Add(this.BlessSplitter);
			this.Controls.Add(this.BlessInfoBlessFilesCheckedListView);
			this.Controls.Add(this.BlessInfoDetailsPanel);
			this.Controls.Add(this.BottomPanel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(427, 233);
			this.Name = "BlessInfoForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Bless";
			this.Load += new System.EventHandler(this.BlessInfoForm_Load);
			this.Shown += new System.EventHandler(this.BlessInfoForm_Shown);
			this.Activated += new System.EventHandler(this.BlessInfoForm_Activated);
			this.Closing += new System.ComponentModel.CancelEventHandler(this.BlessInfoForm_Closing);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BlessInfoForm_FormClosing);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.BlessInfoForm_KeyDown);
			this.BottomPanel.ResumeLayout(false);
			this.BlessInfoBlessCommentPanel.ResumeLayout(false);
			this.BlessInfoBlessCommentEmailPanel.ResumeLayout(false);
			this.BlessFilesContextMenuStrip.ResumeLayout(false);
			this.BlessInfoDetailsPanel.ResumeLayout(false);
			this.BlessInfoDetailsPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void CheckNode(TreeNodeCollection nodes, string name)
		{
			foreach (TreeNode node in nodes)
			{
				if (string.Compare(node.Text, name, true) == 0)
				{
					node.Checked = true;
					if (node.Parent != null)
					{
						node.Parent.Expand();
					}
				}
				else
				{
					// Check children
					CheckNode(node.Nodes, name);
				}
			}
		}

		private void PreLoadNotifies()
		{
			string notifies = guiUserPrefs.LoadPref("BlessInfo", GetPredominateKey());

			string []emails = notifies.Split(new char[] {','});
			foreach (string email in emails)
			{
				CheckNode(BlessInfoBlessCommentEmailCheckedTreeView.Nodes, email);
			}
		}

		private string GetCheckedNodes(System.Windows.Forms.TreeNodeCollection nodes, string checkedString)
		{
			foreach (TreeNode node in nodes)
			{
				if (node.Checked)
				{
					if (checkedString.Length == 0)
					{
						checkedString = node.Text;
					}
					else
					{
						checkedString = checkedString + "," + node.Text;
					}
				}
				else
				{
					if (node.Nodes.Count > 0)
					{
						checkedString = GetCheckedNodes(node.Nodes, checkedString);
					}
				}
			}

			return checkedString;
		}

		private void SaveNotifies()
		{
			string notifies = GetCheckedNodes(BlessInfoBlessCommentEmailCheckedTreeView.Nodes, "");
			MogUtils_Settings.SaveSetting("BlessInfo", GetPredominateKey(), notifies);
		}

		private void AddKey(ArrayList keys, ArrayList keyHits, string key)
		{
			int i = 0;
			foreach (string str in keys)
			{
				if( string.Compare(str, key, true) == 0)
				{
					int val = (int)keyHits[i];
					keyHits[i] = (int)keyHits[i] + 1;

					return;
				}

				i++;
			}

			// No match, add it
			keys.Add(key);
			keyHits.Add(1);
		}

		/// <summary>
		/// Scan all assets tagged for bless and determine what is the predominant asset class for lookup in the 'who to notify' table
		/// </summary>
		/// <returns></returns>
		private string GetPredominateKey()
		{
			string key = "";
			int keyVal = 0;
			ArrayList keys = new ArrayList();
			ArrayList keyHits = new ArrayList();
			foreach(ListViewItem item in BlessInfoBlessFilesCheckedListView.CheckedItems)
			{
				MOG_Filename asset = new MOG_Filename(item.SubItems[2].Text);

				// find out if this key already exists in our array
				AddKey(keys, keyHits, asset.GetAssetClassification());
			}

			int index = 0;
			foreach (int val in keyHits)
			{
				if (val > keyVal)
				{
					key = (string)keys[index];
					keyVal = val;
				}
				index++;
			}

			return key;
		}

		public void SetEditCaret()
		{
			BlessInfoBlessCommentRichTextBox.SelectAll();
			BlessInfoBlessCommentRichTextBox.Focus();
		}

		private void AddEmails()
		{
			BlessInfoBlessCommentEmailCheckedTreeView.Nodes.Clear();

			ArrayList departments = MOG_ControllerProject.GetProject().GetUserDepartments();

			foreach (MOG_UserDepartment department in departments)
			{
				// Make sure we are not adding a black department
				if (department.mDepartmentName.Length > 0)
				{
					TreeNode parent = new TreeNode();
					parent.Text = department.mDepartmentName;
					parent.Tag = department.mDepartmentName;

					BlessInfoBlessCommentEmailCheckedTreeView.Nodes.Add(parent);

					ArrayList users = MOG_ControllerProject.GetProject().GetDepartmentUsers(department.mDepartmentName);

					foreach (MOG_User user in users)
					{
						if (string.Compare(user.GetUserEmailAddress(), "none", true) != 0)
						{
							TreeNode node = new TreeNode();

							node.Text = user.GetUserName();
							node.Tag = user.GetUserEmailAddress();

							parent.Nodes.Add(node);
						}
					}
				}
			}
		}

		/// <summary>
		/// Generate a list of the valid mog files to be blessed
		/// </summary>
		public ArrayList MOGSelectedBlessFiles
		{
			get
			{
				ArrayList SelectedBlessFiles = new ArrayList();
				foreach (ListViewItem assetName in BlessInfoBlessFilesCheckedListView.CheckedItems)
				{
					MOG_Filename filename = new MOG_Filename(assetName.SubItems[2].Text);
					SelectedBlessFiles.Add(filename);
				}

				return SelectedBlessFiles;
			}
		}

		private void BlessInfoOkButton_Click(object sender, System.EventArgs e)
		{		
			// Save notifies for future lookup
			SaveNotifies();

			// Build asset list
			string assetList = "";
			foreach (ListViewItem assetName in BlessInfoBlessFilesCheckedListView.CheckedItems)
			{
				MOG_Filename filename = new MOG_Filename(assetName.SubItems[2].Text);
				if (assetList.Length == 0)
				{
					assetList = filename.GetAssetLabel();
				}
				else
				{
					assetList = assetList + "   " + filename.GetAssetLabel();
				}

				// Send out auto email notifies, Fail if we cannot send the emails
				string []parts = assetName.SubItems[1].Text.Split(";,".ToCharArray());
				foreach (string address in parts)
				{
					mBlessValidated &= SendEmail(address.Trim(), filename.GetAssetLabel());
				}
			}

			if (mBlessValidated)
			{
				// Send admin a copy?
				if (mIsBlessingAsset)
				{
					if (MOG_ControllerProject.GetProject().GetConfigFile().KeyExist("Project", "SendBlessNotifies"))
					{
						if (string.Compare(MOG_ControllerProject.GetProject().GetConfigFile().GetString("Project", "SendBlessNotifies"), "none", true) != 0)
						{
							SendEmail(MOG_ControllerProject.GetProject().GetConfigFile().GetString("Project", "SendBlessNotifies"), "TO: " + GetCheckedNodes(BlessInfoBlessCommentEmailCheckedTreeView.Nodes, "") + "\n\n" + assetList);
						}
					}
				}

				foreach (TreeNode node in BlessInfoBlessCommentEmailCheckedTreeView.Nodes)
				{
					if (node.Checked)
					{
						mBlessValidated = SendEmail((string)node.Tag, assetList);						
					}
					else
					{
						CheckNodeSendEmailChildren(node);
					}
				}

				// Ok start the mail thread
				if (mMessage != null)
				{
					mSendMail = new Thread(new ThreadStart(MailProcess));
					mSendMail.Name = "BlessInfoForm.cs::SendMailThread";
					mSendMail.Start();
				}
			}
		}

		private void CheckNodeSendEmailChildren(TreeNode parent)
		{
			foreach (TreeNode node in parent.Nodes)
			{
				if (node.Checked)
				{
					string assetList = "";
					foreach (ListViewItem assetName in BlessInfoBlessFilesCheckedListView.CheckedItems)
					{
						MOG_Filename filename = new MOG_Filename(assetName.SubItems[2].Text);
						if (assetList.Length == 0)
						{
							assetList = filename.GetAssetLabel();
						}
						else
						{
							assetList = assetList + " " + filename.GetAssetLabel();
						}
					}

					SendEmail((string)node.Tag, assetList);
				}
				else
				{
					CheckNodeSendEmailChildren(node);
				}
			}
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			//BlessInfoBlessCommentRichTextBox.SelectAll();
			//BlessInfoBlessCommentRichTextBox.Focus();
			SaveNotifies();
			BlessInfoOkButton_Click(sender, e);
		}

		private bool SendEmail(string emailAddress, string asset)
		{
			// Check to see if this address is a deparment
			if (emailAddress.IndexOf("@") == -1)
			{
				bool success = true;
				// Send this email to all users in the specified department
				ArrayList users = MOG_ControllerProject.GetProject().GetDepartmentUsers(emailAddress);
				foreach (MOG_User user in users)
				{
					success &= SendEmail(user.GetUserEmailAddress(), asset);
				}

				return success;
			}
			else
			{
				try
				{
					string smtp = "";
					if (MOG_ControllerProject.GetProject().GetConfigFile().KeyExist("PROJECT", "EmailSmtp"))
					{
						smtp = MOG_ControllerProject.GetProject().GetConfigFile().GetString("PROJECT", "EmailSmtp");
					}

					mMailServer = smtp;

					if (mMessage == null)
					{
						// Does our current user have a valid email address to use in the from field?
						if (MOG_ControllerProject.GetUser().GetUserEmailAddress().Length != 0 && MOG_ControllerProject.GetUser().GetUserEmailAddress().Contains("@"))
						{
							mMessage = new MailMessage(MOG_ControllerProject.GetUser().GetUserEmailAddress(), emailAddress);
							mMessage.Subject = "MOG NOTIFICATION";
							mMessage.Body = "You are being notified of the following BLESSED asset(s):\n" +
											"\n" +
								//									"ASEET: " + asset.GetAssetLabel() + "\n" + 
								//									"PLATFORM: " + asset.GetAssetPlatform() + "\n" + 
								//									"CLASSIFICATION: " + asset.GetAssetClassification() + "\n" + 
											asset + "\n" +
											"\n" +
											"Note: If this is a packaged asset it may still be involved with a network package merge." +
											"\n" +
											"\n" +
											"\n" +
											"--------------------------------------------------\n" +
											"Comments:\n" +
											"--------------------------------------------------\n" +
											BlessInfoBlessCommentRichTextBox.Text +
											System.Environment.NewLine;

						}
						else
						{
							MOG_Report.ReportMessage("Could not send email!", string.Format("User ({0}) does not have a valid email address({1})!",
								MOG_ControllerProject.GetUser().GetUserName(),
								MOG_ControllerProject.GetUser().GetUserEmailAddress())
								, "", MOG.PROMPT.MOG_ALERT_LEVEL.ERROR);

							return false;
						}
					}
					else
					{
						mMessage.To.Add(emailAddress);
					}

					Console.WriteLine("Setup to sentTo:{0} - from: {1}", emailAddress, mMessage.From.Address);
					
				}
				catch(Exception ex)
				{
					MOG_Report.ReportMessage("Could not send email!", ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.ERROR);

					return false;
				}
			}

			return true;
		}

		private void MailProcess()
		{
			try
			{
                SmtpClient smtp = new SmtpClient(mMailServer);
				smtp.Send(mMessage);
				foreach (MailAddress address in mMessage.To)
				{
					Console.WriteLine("SentTo:{0} - from: {1}", address.Address, mMessage.From.Address);
				}
			}
			catch(Exception ex)
			{
				MOG_Report.ReportMessage("Could not send email!", ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.ERROR);
			}
		}
		
		private void BlessInfoForm_Activated(object sender, System.EventArgs e)
		{
			SetEditCaret();
		}

		private void BlessInfoForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (DialogResult == DialogResult.OK)
			{
				// Only check for validation when we are in a bless
				if (mIsBlessingAsset)
				{
					if (mBlessValidated)
					{
						// Make sure there is a valid comment
						// KLK - john wanted me to remove the required comment for our commercial users
//						if (BlessInfoBlessCommentRichTextBox.Text.Length == 0 || BlessInfoBlessCommentRichTextBox.Text.Trim().Length == 0)
//						{
//							MessageBox.Show(this, "Please enter a comment to continue...", "Comment is missing.!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
//							e.Cancel = true;
//						}
					}
					else
					{				
						e.Cancel = true;
					}
				}				
			}
		}

		private void BlessInfoForm_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			// If we have CTRL-Enter pressed, go ahead and close the form
			if( e.KeyCode == Keys.Enter && e.Modifiers == Keys.Control )
			{
				e.Handled = true;
				this.BlessInfoOkButton_Click( sender, new EventArgs() );
				this.DialogResult = DialogResult.OK;
			}
		}

		private void BlessInfoBlessFilesCheckedListView_ItemChecked(object sender, ItemCheckedEventArgs e)
		{
			// Check if this item is marked red?
			if (e.Item.ForeColor == Color.Red)
			{
				// Red means they are not allowed to check this item
				e.Item.Checked = false;
			}

			// Always check the status of the OK button
			EnableOK();
		}

		private void EnableOK()
		{
			foreach (ListViewItem item in BlessInfoBlessFilesCheckedListView.Items)
			{
				// Check if this is checked?
				if (item.Checked)
				{
					// Make sure we enable the OK button
					BlessInfoOkButton.Enabled = true;
					return;
				}
			}

			// Make sure we enable the OK button
			BlessInfoOkButton.Enabled = false;
		}

		private void BlessMaintainLockCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			mMaintainLock = BlessMaintainLockCheckBox.Checked;
		}

		private void BlessInfoForm_Load(object sender, EventArgs e)
		{
			guiUserPrefs.LoadDynamic_LayoutPrefs("BlessInfo", this);			
		}

		private void BlessInfoForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			guiUserPrefs.SaveDynamic_LayoutPrefs("BlessInfo", this);
		}

		private void resolveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach(ListViewItem item in BlessInfoBlessFilesCheckedListView.SelectedItems)
			{
				// Reimport the asset into the drafts again
				MOG_Filename assetFilename = new MOG_Filename(item.SubItems[2].Text);
				if (MOG_ControllerAsset.ReimportAssetUsingLocalFiles(assetFilename, MOG_ControllerProject.GetWorkspaceDirectory()))
				{
					MOG_Properties properties = new MOG_Properties(assetFilename);
					UpdateItem(item, assetFilename, properties);
				}
				else
				{
					string message = "Failed to resolve asset using local files.\n" +
									 "ASSET: " + assetFilename.GetAssetFullName() + "\n\n" +
									 "Only native syncing assets can be resolved with local files";
					MOG_Report.ReportMessage("Resolve Failed", message, "", MOG.PROMPT.MOG_ALERT_LEVEL.ERROR);
				}
			}
		}

		private void BlessInfoForm_Shown(object sender, EventArgs e)
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
		}
	}
}
