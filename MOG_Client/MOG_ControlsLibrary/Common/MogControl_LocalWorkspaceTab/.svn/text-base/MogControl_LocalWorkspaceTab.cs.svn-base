using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using EV.Windows.Forms;
using MOG_ControlsLibrary.Common.MOG_ContextMenu;
using MOG_ControlsLibrary.Utils;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using System.Collections;
using MOG_Client.Client_Mog_Utilities;
using MOG.PROMPT;
using MOG.CONTROLLER.CONTROLLERSYNCDATA;
using MOG_ControlsLibrary.MogForm_CreateTag;
using MOG.DATABASE;
using System.IO;
using MOG_ControlsLibrary.MogUtils_Settings;

namespace MOG_Client.Client_Gui.AssetManager_Helper
{
	public partial class MogControl_LocalWorkspaceTab : UserControl
	{
		private MogMainForm mainForm;
		private ListViewSortManager mManager;
		private MogControl_AssetContextMenu mWorkspaceContextMenu;
		private MOG_ControllerSyncData mWorkspace;

		public enum WorkspaceActivity
		{
			NoUpdate,
			AutoUpdate,
			AutoUpdateAlways,
		}

		#region Getters / Setters

		public bool AutoUpdate
		{
			get
			{
				return autoUpdateToolStripMenuItem.Checked;
			}
			set
			{
				autoUpdateToolStripMenuItem.Checked = value;
			}
		}

		public bool WorkspaceAutoPackage
		{
			get { return autoPackageToolStripMenuItem.Checked; }
			set { autoPackageToolStripMenuItem.Checked = value; }
		}

		public string WorkspaceName
		{
			get { return mWorkspace.GetName(); }
			set { mWorkspace.SetName(value); }
		}

		#endregion

		public MogControl_LocalWorkspaceTab(MogMainForm parent, MOG_ControllerSyncData workspace)
		{
			InitializeComponent();

			mWorkspace = workspace;
			mainForm = parent;

			InitializeWorkspace();
		}

		#region Initialize
		private void InitializeWorkspace()
		{
			mManager = new ListViewSortManager(WorkSpaceListView, new Type[] {
																				   typeof(ListViewTextCaseInsensitiveSort),
																				   typeof(ListViewTextCaseInsensitiveSort),
																				   typeof(ListViewTextCaseInsensitiveSort),
																				   typeof(ListViewDateSort),
																				   typeof(ListViewStringSizeSort),
																				   typeof(ListViewTextCaseInsensitiveSort),
																				   typeof(ListViewTextCaseInsensitiveSort),
																				   typeof(ListViewTextCaseInsensitiveSort),
																				   typeof(ListViewTextCaseInsensitiveSort),
																				   typeof(ListViewTextCaseInsensitiveSort),
																				   typeof(ListViewTextCaseInsensitiveSort),
																				   typeof(ListViewTextCaseInsensitiveSort),
																				   typeof(ListViewTextCaseInsensitiveSort)
																					   });

			mWorkspaceContextMenu = new MogControl_AssetContextMenu("NAME, CLASS, TARGETPATH, DATE, SIZE, STATE, PLATFORM, CREATOR, RESPPARTY, OPTIONS, FULLNAME, BOX, GROUP", WorkSpaceListView);

			WorkSpaceListView.ContextMenuStrip = mWorkspaceContextMenu.InitializeContextMenu("{Updated}");
			WorkSpaceListView.AllowColumnReorder = false;
			WorkSpaceListView.MultiSelect = true;
			WorkSpaceListView.SmallImageList = MogUtil_AssetIcons.Images;
			WorkSpaceListView.StateImageList = mainForm.mAssetManager.mAssetStatus.StateImageList; //MogUtil_AssetIcons.StateImages;
			WorkSpaceListView.FullRowSelect = true;

			WorkSpaceListView.AllowDrop = true;
			WorkSpaceListView.DragOver += new System.Windows.Forms.DragEventHandler(this.PlatformViews_DragOver);
			WorkSpaceListView.DragDrop += new System.Windows.Forms.DragEventHandler(this.PlatformViews_DragDrop);
		}

		public void RefreshWorkspaceToolbar()
		{
			// Update the 'ActiveTag'
			UpdateGetActiveButton();

			string displayStyle = MogUtils_Settings.LoadSetting_default(WorkspaceName, "DisplayStyle", "ImageAndText");
			switch (displayStyle.ToLower())
			{
				case "image": SetImageOnlyDisplayStyle(); break;
				case "imageandtext": SetImageAndTextDisplayStyle(); break;
			}

			GetLatestFiltersToolStripMenuItem.Tag = MogUtils_Settings.LoadSetting_default(WorkspaceName, "GetLatestFilter", "");
			GetLatestTagsToolStripMenuItem.Tag = MogUtils_Settings.LoadSetting_default(WorkspaceName, "GetLatestTag", "");
			GetLatestUpdateModifiedMissingToolStripMenuItem.Checked = MogUtils_Settings.LoadBoolSetting(WorkspaceName, "GetLatestForce", false);
			GetLatestCleanUnknownFilesToolStripMenuItem.Checked = MogUtils_Settings.LoadBoolSetting(WorkspaceName, "CleanUnknownFiles", false);
			autoPackageToolStripMenuItem.Checked = MogUtils_Settings.LoadBoolSetting(WorkspaceName, "AutoPackage", true);
            alwaysActiveToolStripMenuItem.Checked = MogUtils_Settings.LoadBoolSetting(WorkspaceName, "AlwaysActive", false);
            autoUpdateToolStripMenuItem.Checked = MogUtils_Settings.LoadBoolSetting(WorkspaceName, "AutoUpdate", true);

			if (mWorkspace != null)
			{
				mWorkspace.SetAlwaysActive(alwaysActiveToolStripMenuItem.Checked);
				mWorkspace.EnableAutoPackaging(autoPackageToolStripMenuItem.Checked);
				mWorkspace.EnableAutoUpdating(autoUpdateToolStripMenuItem.Checked);
			}

            UpdateAutoPackageToolStripMenuItemVisuals();
			UpdateAutoUpdateToolStripMenuItemVisuals();
			UpdateGetLatestUpdateModifiedMissingToolStripMenuItemVisuals();
			UpdateGetActiveButton();

			ResetToolTip(GetLatestToolStripButton);
		}
		#endregion

		#region Drag / Drop

		/// <summary>
		/// Drag and drop routine for the platforms ListView
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PlatformViews_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
		{
			// Make sure we only display the drag-n-drop icon (via Windows) if we have the right data...
			if (e.Data.GetData("ArrayListAssetManager") != null)
			{
				e.Effect = DragDropEffects.Copy;
			}
		}

		/// <summary>
		/// Drag and drop routine for the platforms ListView
		/// </summary>
		private void PlatformViews_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{
			if (MOG_ControllerProject.GetCurrentSyncDataController() != null)
			{
				// We will only accept a ArrayListAssetManager type object
				if (e.Data.GetDataPresent("ArrayListAssetManager"))
				{
					// Get our array list
					ArrayList items = e.Data.GetData("ArrayListAssetManager") as ArrayList;
					List<string> filenames = new List<string>();

					foreach (string item in items)
					{
						filenames.Add(item);
					}

					guiAssetController.UpdateLocal(filenames, true);
				}
			}
			else
			{
				MOG_Prompt.PromptMessage("Update Asset", "Could not update asset to your workspace because no workspace was found!", "", MOG_ALERT_LEVEL.ERROR);
			}
		}
		#endregion

		#region Filters / Tags
		private void PopulateFilters()
		{
			GetLatestFiltersToolStripMenuItem.DropDownItems.Clear();

			// Get the last set active tag
			string active = "";
			if (GetLatestFiltersToolStripMenuItem.Tag is string)
			{
				active = GetLatestFiltersToolStripMenuItem.Tag as string;
				AddFilterItem(active, "None");
			}
			else
			{
				AddFilterItem("None", "None");
			}

			// Get the users tools
			if (MOG_ControllerProject.GetUser() != null)
			{
				// Get all the filter files found within the users tools directory
				foreach (string filter in Directory.GetFiles(MOG_ControllerProject.GetUser().GetUserToolsPath(), "*.sync"))
				{
					AddFilterItem(active, filter);
				}
			}

			if (!DesignMode)
			{
				// Get project tools
				// Get all the filter files found within the users tools directory
				string toolsPath = Path.Combine(MOG_ControllerProject.GetProjectPath(), "Tools");
				foreach (string filter in Directory.GetFiles(toolsPath, "*.sync"))
				{
					AddFilterItem(active, filter);
				}
			}
		}

		private void AddFilterItem(string active, string filter)
		{
			string filterName = Path.GetFileNameWithoutExtension(filter);

			ToolStripMenuItem filterItem = new ToolStripMenuItem(filterName);
			filterItem.CheckedChanged += FilterMenuItem_CheckChanged;
			filterItem.Click += FilterMenuItem_Click;

			// Is this tag the last active one?
			if (string.Compare(active, filterName, true) == 0)
			{
				filterItem.Checked = true;
			}
			// Add the names of each one of them to the comboBox
			GetLatestFiltersToolStripMenuItem.DropDownItems.Add(filterItem);
		}

		void FilterMenuItem_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem item = sender as ToolStripMenuItem;
			if (item != null && item.OwnerItem != null)
			{
				item.Checked = !item.Checked;
				GetLatestToolStripButton.ShowDropDown();
				GetLatestFiltersToolStripMenuItem.ShowDropDown();
			}
		}

		void FilterMenuItem_CheckChanged(object sender, EventArgs e)
		{
			ToolStripMenuItem item = sender as ToolStripMenuItem;
			if (item != null && item.OwnerItem != null)
			{
				item.OwnerItem.Tag = item.Text;
				ResetToolTip(item.OwnerItem.OwnerItem as ToolStripSplitButton);

				MogUtils_Settings.SaveSetting(WorkspaceName, "GetLatestFilter", item.Text);
			}
		}

		private void PopulateTags()
		{
			GetLatestTagsToolStripMenuItem.DropDownItems.Clear();

			ArrayList Branches = MOG_DBProjectAPI.GetAllBranchNames();

			if (Branches != null)
			{
				// Get the last set active tag
				string active = "";
				if (GetLatestTagsToolStripMenuItem.Tag is string)
				{
					active = GetLatestTagsToolStripMenuItem.Tag as string;
					AddTagItem(active, "None");
				}
				else
				{
					AddTagItem("None", "None");
				}

				foreach (MOG_DBBranchInfo branch in Branches)
				{
					if (branch.mTag)
					{
						AddTagItem(active, branch.mBranchName);
					}
				}
			}
		}

		private void AddTagItem(string active, string tagName)
		{
			ToolStripMenuItem tag = new ToolStripMenuItem(tagName);
			tag.CheckedChanged += TagMenuItem_CheckChanged;
			tag.Click += TagMenuItem_Click;

			// Is this tag the last active one?
			if (string.Compare(active, tagName, true) == 0)
			{
				tag.Checked = true;
			}

			GetLatestTagsToolStripMenuItem.DropDownItems.Add(tag);
		}

		void TagMenuItem_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem item = sender as ToolStripMenuItem;
			if (item != null && item.OwnerItem != null)
			{
				item.Checked = !item.Checked;
				GetLatestToolStripButton.ShowDropDown();
				GetLatestTagsToolStripMenuItem.ShowDropDown();
			}
		}

		void TagMenuItem_CheckChanged(object sender, EventArgs e)
		{
			ToolStripMenuItem item = sender as ToolStripMenuItem;
			if (item != null && item.OwnerItem != null)
			{
				item.OwnerItem.Tag = item.Text;
				ResetToolTip(item.OwnerItem.OwnerItem as ToolStripSplitButton);

				MogUtils_Settings.SaveSetting(WorkspaceName, "GetLatestTag", item.Text);
			}
		}

		private void ResetToolTip(ToolStripSplitButton item)
		{
			if (item != null)
			{
				string message = "Get latest";

				string filter = "";
				if (GetLatestFiltersToolStripMenuItem.Tag is string)
				{
					filter = GetLatestFiltersToolStripMenuItem.Tag as string;
					if (filter == "None")
					{
						filter = "";
					}
					else
					{
						message += "\n  Filter: " + filter;
					}
				}

				string tag = "";
				if (GetLatestTagsToolStripMenuItem.Tag is string)
				{
					tag = GetLatestTagsToolStripMenuItem.Tag as string;
					if (tag == "None")
					{
						tag = "";
					}
					else
					{
						message += "\n  Tag: " + tag;
					}
				}

				if (GetLatestUpdateModifiedMissingToolStripMenuItem.Checked)
				{
					message += "\n  Force: " + GetLatestUpdateModifiedMissingToolStripMenuItem.Checked.ToString();
				}

				if (GetLatestCleanUnknownFilesToolStripMenuItem.Checked)
				{
					message += "\n  Clean: " + GetLatestCleanUnknownFilesToolStripMenuItem.Checked.ToString();
				}

				item.ToolTipText = message;
			}
		}
		#endregion

		private string UpdateGetActiveButton()
		{
			string activeTag = "";

			// Make sure there is a current 'ActiveTag'?
			if (MOG_ControllerProject.GetProject().GetConfigFile().KeyExist("Project", "ActiveTag"))
			{
				// Get the current 'ActiveTag'
				activeTag = MOG_ControllerProject.GetProject().GetConfigFile().GetString("Project", "ActiveTag");
				// Update the ToolStripButton's text
				GetActiveToolStripButton.Text = "Get (" + activeTag + ")";
				GetActiveToolStripButton.Tag = activeTag;
			}

			// Only show the ToolStripButton if we have an 'ActiveTag'specified
			if (activeTag.Length > 0)
			{
				// Show the active tag button
				GetActiveToolStripButton.Visible = true;
				// We have an active tag so hide the advanced button unless they always want it
				GetLatestToolStripButton.Visible = mainForm.alwaysDisplayAdvancedGetLatestToolStripMenuItem.Checked;
			}
			else
			{
				// No active tag so only show the Advanced button
				GetActiveToolStripButton.Visible = false;
				GetLatestToolStripButton.Visible = true;
		}


			GetActiveToolStripButton.Visible = (activeTag.Length > 0) ? true : false;
            GetLatestToolStripButton.Visible = !(activeTag.Length > 0) || mainForm.alwaysDisplayAdvancedGetLatestToolStripMenuItem.Checked;
			return activeTag;
		}

		private void SetImageOnlyDisplayStyle()
		{
			TopPanel.Height = 23;
			foreach (ToolStripItem item in WorkspaceToolStrip.Items)
			{
				item.DisplayStyle = ToolStripItemDisplayStyle.Image;
			}

			MogUtils_Settings.SaveSetting(WorkspaceName, "DisplayStyle", "Image");
		}

		private void SetImageAndTextDisplayStyle()
		{
			TopPanel.Height = 37;
			foreach (ToolStripItem item in WorkspaceToolStrip.Items)
			{
				item.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
			}

			MogUtils_Settings.SaveSetting(WorkspaceName, "DisplayStyle", "ImageAndText");
		}

        private void UpdateAutoPackageToolStripMenuItemVisuals()
        {
            // Set our current autopackage button
			mWorkspace.EnableAutoPackaging(autoPackageToolStripMenuItem.Checked);

			if (autoPackageToolStripMenuItem.Checked)
			{
				PackageToolStripDropDownButton.Font = new Font(PackageToolStripDropDownButton.Font, FontStyle.Bold);
			}
			else
			{
				PackageToolStripDropDownButton.Font = new Font(PackageToolStripDropDownButton.Font, FontStyle.Regular);
			}

			mainForm.AssetManagerAutoPackageCheckBox.Checked = autoPackageToolStripMenuItem.Checked;
			mainForm.AssetManagerAutoUpdateLocalCheckBox.Checked = autoUpdateToolStripMenuItem.Checked;
		}


        private void UpdateAutoUpdateToolStripMenuItemVisuals()
        {
            // Set our current button status
			mWorkspace.SetAlwaysActive(alwaysActiveToolStripMenuItem.Checked);
			mWorkspace.EnableAutoUpdating(autoUpdateToolStripMenuItem.Checked);
			mainForm.AssetManagerAutoUpdateLocalCheckBox.Checked = autoUpdateToolStripMenuItem.Checked;
			if (autoUpdateToolStripMenuItem.Checked)
			{
				WorkspaceToolStripButton.Font = new Font(WorkspaceToolStripButton.Font, FontStyle.Bold);
			}
			else
			{
				WorkspaceToolStripButton.Font = new Font(WorkspaceToolStripButton.Font, FontStyle.Regular);
			}

			mainForm.mAssetManager.UpdateAssetManagerLocalTabText(true);
		}


		private void UpdateGetLatestUpdateModifiedMissingToolStripMenuItemVisuals()
		{
			ResetToolTip(GetLatestToolStripButton);
			if (GetLatestUpdateModifiedMissingToolStripMenuItem.Checked)
			{
				GetActiveToolStripButton.Font = new Font(GetActiveToolStripButton.Font, FontStyle.Bold);
				GetLatestToolStripButton.Font = new Font(GetLatestToolStripButton.Font, FontStyle.Bold);
			}
			else
			{
				GetActiveToolStripButton.Font = new Font(GetActiveToolStripButton.Font, FontStyle.Regular);
				GetLatestToolStripButton.Font = new Font(GetLatestToolStripButton.Font, FontStyle.Regular);
			}
		}

		#region Control events
		private void exploreToolStripMenuItem_Click(object sender, EventArgs e)
		{
			mainForm.mAssetManager.mLocal.LocalBranchExplore();
		}

		private void removeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MOG_ControllerSyncData tabBranch = mainForm.AssetManagerLocalDataTabControl.SelectedTab.Tag as MOG_ControllerSyncData;
			if (tabBranch != null)
			{
				mainForm.mAssetManager.mLocal.LocalBranchRemove(tabBranch);
			}
		}

		private void GetActiveToolStripButton_Click(object sender, EventArgs e)
		{
			// Update the 'ActiveTab' just in case it changed on us
			string activeTag = UpdateGetActiveButton();
			if (activeTag.Length > 0)
			{
				mainForm.mAssetManager.BuildMerge(false, false, activeTag, "", GetLatestUpdateModifiedMissingToolStripMenuItem.Checked, GetLatestCleanUnknownFilesToolStripMenuItem.Checked);
			}
		}

		private void GetLatestToolStripButton_Click(object sender, EventArgs e)
		{
			string tag = "";

			// Do we have a tag?
			if (GetLatestTagsToolStripMenuItem.Tag is string)
			{
				tag = GetLatestTagsToolStripMenuItem.Tag as string;
				if (tag == "None") tag = "";
			}

			string filter = "";

			// Do we have a filter?
			if (GetLatestFiltersToolStripMenuItem.Tag is string)
			{
				filter = GetLatestFiltersToolStripMenuItem.Tag as string;
				if (filter == "None") filter = "";
			}

			// Launch the sync
			mainForm.mAssetManager.BuildMerge(true, false, tag, filter, GetLatestUpdateModifiedMissingToolStripMenuItem.Checked, GetLatestCleanUnknownFilesToolStripMenuItem.Checked);
		}

		private void PackageToolStripDropDownButton_ButtonClick(object sender, EventArgs e)
		{
			if (mWorkspace.GetLocalPackagingStatus() == MOG_ControllerSyncData.PackageState.PackageState_Pending)
			{
				mWorkspace.BuildPackage();
			}
		}

		private void createTagToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MogForm_CreateTag tag = new MogForm_CreateTag();
			tag.ShowDialog(mainForm);
		}

		private void imageOnlyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetImageOnlyDisplayStyle();
		}

		private void imageAndTextToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetImageAndTextDisplayStyle();
		}

		private void GetActiveToolStripButton_DropDownOpening(object sender, EventArgs e)
		{
			// We need to repopulate the ToolStrip because some of the items are shared with GetLatestToolStripButton
			// This is needed because toolstrips items only get one owner at a time and the items need to be reassigned
			this.GetActiveToolStripButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
					this.GetLatestCleanUnknownFilesToolStripMenuItem,
					this.GetLatestUpdateModifiedMissingToolStripMenuItem});

			PopulateTags();
			PopulateFilters();
		}

		private void GetLatestToolStripButton_DropDownOpening(object sender, EventArgs e)
		{
			// We need to repopulate the ToolStrip because some of the items are shared with GetLatestToolStripButton
			// This is needed because toolstrips items only get one owner at a time and the items need to be reassigned
			this.GetLatestToolStripButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
					this.GetLatestFiltersToolStripMenuItem,
					this.GetLatestTagsToolStripMenuItem,
					this.toolStripMenuItem4,
					this.GetLatestCleanUnknownFilesToolStripMenuItem,
					this.GetLatestUpdateModifiedMissingToolStripMenuItem});

			PopulateTags();
			PopulateFilters();
		}

		private void WorkspaceToolStripButton_ButtonClick(object sender, EventArgs e)
		{
			WorkspaceToolStripButton.ShowDropDown();
		}

		private void autoPackageToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			MogUtils_Settings.SaveSetting(WorkspaceName, "AutoPackage", autoPackageToolStripMenuItem.Checked.ToString());
			mWorkspace.EnableAutoPackaging(autoPackageToolStripMenuItem.Checked);
            UpdateAutoPackageToolStripMenuItemVisuals();
		}       

		private void GetLatestUpdateModifiedMissingToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			MogUtils_Settings.SaveSetting(WorkspaceName, "GetLatestForce", GetLatestUpdateModifiedMissingToolStripMenuItem.Checked.ToString());
			UpdateGetLatestUpdateModifiedMissingToolStripMenuItemVisuals();
		}

		private void GetLatestCleanUnknownFilesToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			MogUtils_Settings.SaveSetting(WorkspaceName, "CleanUnknownFiles", GetLatestCleanUnknownFilesToolStripMenuItem.Checked.ToString());
		}

		private void renameToolStripMenuItem_Click(object sender, EventArgs e)
		{
			mainForm.mAssetManager.mLocal.LocalBranchRename();
		}

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			mainForm.mAssetManager.mLocal.LocalBranchCreateNew(false);
		}

		private void autoPackageToolStripMenuItem_Click(object sender, EventArgs e)
		{
			autoPackageToolStripMenuItem.Checked = !autoPackageToolStripMenuItem.Checked;
			PackageToolStripDropDownButton.ShowDropDown();
		}

		private void GetLatestUpdateModifiedMissingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			GetLatestUpdateModifiedMissingToolStripMenuItem.Checked = !GetLatestUpdateModifiedMissingToolStripMenuItem.Checked;
			// Find out who our parent ToolStrip is?
			if (GetLatestToolStripButton.DropDownItems.Contains(GetLatestUpdateModifiedMissingToolStripMenuItem))
			{
				// Leave this ToolStrip expanded...Better user experience
				GetLatestToolStripButton.ShowDropDown();
			}
			else
			{
				// Leave this ToolStrip expanded...Better user experience
				GetActiveToolStripButton.ShowDropDown();
			}
		}

		private void GetLatestCleanUnknownFilesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			GetLatestCleanUnknownFilesToolStripMenuItem.Checked = !GetLatestCleanUnknownFilesToolStripMenuItem.Checked;
			// Find out who our parent ToolStrip is?
			if (GetLatestToolStripButton.DropDownItems.Contains(GetLatestCleanUnknownFilesToolStripMenuItem))
			{
				// Leave this ToolStrip expanded...Better user experience
				GetLatestToolStripButton.ShowDropDown();
			}
			else
			{
				// Leave this ToolStrip expanded...Better user experience
				GetActiveToolStripButton.ShowDropDown();
			}
		}

        private void WorkspaceAutoUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item != null && item.OwnerItem != null)
            {
                item.Checked = !item.Checked;
                WorkspaceToolStripButton.ShowDropDown();
            }
        }

        private void WorkspaceAutoUpdateToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
			MogUtils_Settings.SaveSetting(WorkspaceName, "AutoUpdate", autoUpdateToolStripMenuItem.Checked.ToString());
			UpdateAutoUpdateToolStripMenuItemVisuals();
        }

		private void WorkspaceAlwaysActiveToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
			MogUtils_Settings.SaveSetting(WorkspaceName, "AlwaysActive", alwaysActiveToolStripMenuItem.Checked.ToString());
			UpdateAutoUpdateToolStripMenuItemVisuals();
        }
        #endregion       
      }
}
		
