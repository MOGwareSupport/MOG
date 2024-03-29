using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Diagnostics;
using System.Threading;

using MOG_Client.Client_Mog_Utilities;
using MOG_Client.Client_Mog_Utilities.AssetOptions;
using MOG_Client.Client_Gui.AssetManager_Helper;
using MOG_Client.Client_Utilities;
using MOG_ControlsLibrary.Utils;
using MOG_ControlsLibrary.Common;

using MOG;
using MOG.INI;
using MOG.USER;
using MOG.DOSUTILS;
using MOG.FILENAME;
using MOG.PROJECT;
using MOG.PLATFORM;
using MOG.PROPERTIES;
using MOG.COMMAND;
using MOG.REPORT;
using MOG.PROMPT;
using MOG.PROGRESS;
using MOG.CONTROLLER;
using MOG.CONTROLLER.CONTROLLERASSET;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.CONTROLLER.CONTROLLERSYNCDATA;
using MOG.CONTROLLER.CONTROLLERPACKAGE;
using MOG.DATABASE;
using MOG.COMMAND.SLAVE;
using MOG.SYSTEMUTILITIES;
using MOG.TIME;

using MOG_ControlsLibrary.Common.MOG_ContextMenu;

using EV.Windows.Forms;
using System.Collections.Specialized;
using MOG_CoreControls;
using System.ComponentModel;
using System.Collections.Generic;
using MOG.ASSET_STATUS;
using MOG.UTILITIES;
using MOG_Client.Utilities;

namespace MOG_Client.Client_Gui
{
	/// <summary>
	/// Summary description for AssetManager.
	/// </summary>
	public class guiAssetManager
	{
		public MogMainForm mainForm;										// Pointer to the main form
		public guiAssetManagerAssets		mAssets;
		public guiAssetManagerLocalData		mLocal;
		public guiAssetManagerGroups		mGroups;
		public guiAssetManagerTrash			mTrash;
		public bool mInitialized;
		public string mCurrentUsersBox;

		public ImageList mAssetTypeImages	= new ImageList();				// Holds all the asset type icons
		public ArrayList mAssetTypes		= new ArrayList();				// This will be a look-up table for the imageList allowing us to look up types and get the correct icon

		public string mInboxAssetsDirectory;
		public string mOutboxAssetsDirectory;
		public string mActiveAssetsDirectory;
		public string mDefaultUpdateBuildType;								// Holds the last used build type
		public string mCurrentVersion;										// Holds the last known updated version number
		public bool	  mHidePlatforms;										// Holds the value of the hide Specific platforms checkbox
		public bool	  mLoadBranches = true;

		public enum AssetBoxColumns { NAME, CLASS, TARGETPATH, DATE, SIZE, PLATFORM, STATE, CREATOR, RESPPARTY, OPTIONS, FULLNAME, BOX, GROUP, TOTAL_COLUMNS};
		public enum LocalBoxColumns { NAME, CLASS, TARGETPATH, DATE, SIZE, PLATFORM, STATE, CREATOR, RESPPARTY, OPTIONS, FULLNAME, BOX, GROUP, TOTAL_COLUMNS};
		public enum TrashBoxColumns { NAME, CLASS, TARGETPATH, DATE, SIZE, PLATFORM, STATE, CREATOR, RESPPARTY, OPTIONS, FULLNAME, BOX, GROUP, TOTAL_COLUMNS};
		public enum ChatArchiveColumns {TIME, INVITED, CREATOR, STATUS, PARTY, FULLNAME};
		public enum InboxTabOrder {ASSETS, MESSAGES, TASKS};
		public enum OutboxTabOrder {ASSETS, MESSAGES, TASKS};
		public enum AssetManagerTabOrder {INBOX, OUTBOX, TRASH};

		public MogControl_AssetContextMenu mInboxContextMenu, mDraftsContextMenu, mOutboxContextMenu, mUpdatedContextMenu, mTrashContextMenu;

		//public ArrayList mListViewSort_Manager;
		public ListViewSortManager mDraftsSortManager, mInboxSortManager, mOutboxSortManager, mTrashSortManager;

		public MOGAssetStatus mAssetStatus;

		public MogUtil_ControlHide mToDo, mLocalExplorer, mTools;

		public guiAssetManager(MogMainForm main)
		{
			mainForm = main;

			mInboxAssetsDirectory = "";
			mOutboxAssetsDirectory = "";
			mDefaultUpdateBuildType = "";

			if (mAssetStatus == null)
			{
				mAssetStatus = new MOGAssetStatus();
			}
			
			//InitializeAssetIcons();			
			mInitialized = false;
		}

		public void DeInitialize()
		{
			mainForm.AssetManagerInboxListView.Items.Clear();
			mainForm.AssetManagerSentListView.Items.Clear();
			mainForm.AssetManagerTrashListView.Items.Clear();

			mainForm.mAssetManager.GetActiveUsersControl().Items.Clear();
			mainForm.mAssetManager.GetActiveUsersControl().Text = "Needs login user";

			mainForm.AssetManagerMergeVersionButton.Enabled = false;
			mainForm.AssetManagerPackageButton.Enabled = false;

			if (mLocal != null)	mLocal.UnLoadUserLocalBranches();
		}

		public void Initialize()
		{
			mAssets = new guiAssetManagerAssets(this);
			mLocal = new guiAssetManagerLocalData(this);
			mGroups = new guiAssetManagerGroups();
			mTrash = new guiAssetManagerTrash(this);

			// Intialize inbox users combobox
			InitializeUsers();

			// Initialize and Load the user defined local branches
			mLocal.InitializePotentialLocalBranches();
			//mLocal.LoadUserLocalBranches();

			// Initialize control hiders
			mToDo = new MogUtil_ControlHide(168, 0, false);
			mLocalExplorer = new MogUtil_ControlHide(168, 0, false);
			mLocalExplorer.mOpening += new MogUtil_ControlHide.ControlHideOpeningEvent(mLocalExplorer_mOpening);
			mTools = new MogUtil_ControlHide(250, 0, true);

			// Initialize context menus
			mInboxContextMenu = new MogControl_AssetContextMenu("NAME, CLASS, TARGETPATH, DATE, SIZE, PLATFORM, STATE, CREATOR, RESPPARTY, OPTIONS, FULLNAME, BOX, GROUP", mainForm.AssetManagerInboxListView);
			mDraftsContextMenu = new MogControl_AssetContextMenu("NAME, CLASS, TARGETPATH, DATE, SIZE, PLATFORM, STATE, CREATOR, RESPPARTY, OPTIONS, FULLNAME, BOX, GROUP", mainForm.AssetManagerDraftsListView);
			mOutboxContextMenu = new MogControl_AssetContextMenu("NAME, CLASS, TARGETPATH, DATE, SIZE, PLATFORM, STATE, CREATOR, RESPPARTY, OPTIONS, FULLNAME, BOX, GROUP", mainForm.AssetManagerSentListView);
			mTrashContextMenu = new MogControl_AssetContextMenu("NAME, CLASS, TARGETPATH, DATE, SIZE, PLATFORM, STATE, CREATOR, RESPPARTY, OPTIONS, FULLNAME, BOX, GROUP", mainForm.AssetManagerTrashListView);

			// Initialize the inbox icons
			mainForm.AssetManagerInboxListView.SmallImageList = MogUtil_AssetIcons.Images;
			mainForm.AssetManagerInboxListView.StateImageList = mAssetStatus.StateImageList;

			mainForm.AssetManagerDraftsListView.SmallImageList = MogUtil_AssetIcons.Images;
			mainForm.AssetManagerDraftsListView.StateImageList = mAssetStatus.StateImageList;

			// Initialize the outbox icons
			mainForm.AssetManagerSentListView.SmallImageList = MogUtil_AssetIcons.Images;
			mainForm.AssetManagerSentListView.StateImageList = mAssetStatus.StateImageList;

			mGroups.Add(mainForm.AssetManagerDraftsListView, "Group");
			mGroups.Add(mainForm.AssetManagerInboxListView, "Group");
			mGroups.Add(mainForm.AssetManagerSentListView, "Group");
			mGroups.Add(mainForm.AssetManagerTrashListView, "Group");

			// *****************
			// Setup the sort engine for the listView windows
			// *****************

			// Assets
			mDraftsSortManager = new ListViewSortManager(mainForm.AssetManagerDraftsListView, new Type[] {
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
			mInboxSortManager = new ListViewSortManager(mainForm.AssetManagerInboxListView, new Type[] {
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

			mOutboxSortManager = new ListViewSortManager(mainForm.AssetManagerSentListView, new Type[] {
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

			mTrashSortManager = new ListViewSortManager(mainForm.AssetManagerTrashListView, new Type[] {
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
		}

		void mLocalExplorer_mOpening()
		{
			// If the tree has not been initialized, initialize it
			if (!mainForm.LocalBranchMogControl_GameDataDestinationTreeView.Initialized)
			{
				mainForm.LocalBranchMogControl_GameDataDestinationTreeView.InitializeProjectsOnly();
			}
		}

		public void InitializeUsers()
		{
			if (MOG_ControllerProject.IsUser())
			{
				// Set the current active users box
				mCurrentUsersBox = MOG_ControllerProject.GetActiveUser().GetUserName();
			}
			else
			{
				mCurrentUsersBox = "Login to project...";
			}
		}

		public void LatentInitialize()
		{
			mainForm.AssetManagerInboxListView.ContextMenuStrip = mInboxContextMenu.InitializeContextMenu("{Inbox}");
			mainForm.AssetManagerDraftsListView.ContextMenuStrip = mDraftsContextMenu.InitializeContextMenu("{Inbox}");
			mainForm.AssetManagerSentListView.ContextMenuStrip = mOutboxContextMenu.InitializeContextMenu("{Outbox}");
			mainForm.AssetManagerTrashListView.ContextMenuStrip = mTrashContextMenu.InitializeContextMenu("{Trash}");

			if (!MOG_ControllerProject.IsProject() || !MOG_ControllerProject.IsUser())
			{
				return;
			}

			RefreshAllWindows();

			guiUserPrefs.LoadDynamic_LayoutPrefs("AssetManager", mainForm);

			mLocal.InitialiseLocalView();

			mainForm.AssetManagerLocalDataExplorerSplitter.SplitPosition = mLocalExplorer.Width;
			mainForm.myLocalExplorerToolStripMenuItem.Checked = mLocalExplorer.Opened;
			
			mainForm.AssetManagerTasksSplitter.SplitPosition = mToDo.Width;

			MainMenuViewClass.bChangeLocalBranchRightPanelWidth = true;
			mainForm.AssetManagerLocalDataSplitter.SplitPosition = mTools.Width;

			MainMenuViewClass.bChangeLocalBranchRightPanelWidth = false;
			mainForm.myToolboxToolStripMenuItem.Checked = mTools.Opened;


			// Initialize our users toolBox
			MOG_ControllerSyncData sync = MOG_ControllerProject.GetCurrentSyncDataController();
			if (sync != null)
			{
				// Initialize the local tools window
				mainForm.CustomToolsBox.Initialize( sync.GetPlatformName() );	// DVC
			}
			else
			{
				mainForm.CustomToolsBox.Visible = false;
			}

            mInitialized = true;
		}
		
		public ComboBox GetActiveUsersControl()
		{
			return mainForm.AssetManagerActiveUserComboBox;
		}

		public string GetActiveUser()
		{
			return GetActiveUsersControl().Text;
		}

		public void SetActiveUser(string name)
		{
			// Set active users department
			MOG_User user = MOG_ControllerProject.GetProject().GetUser(name);
			if (user != null)
			{
				mainForm.AssetManagerActiveUserDepartmentsComboBox.Text = user.GetUserDepartment();
			}
			GetActiveUsersControl().Text = name;
		}

		public void SetUserInbox(string name)
		{
			if (MOG_ControllerProject.GetProject() != null)
			{
				// Is this user valid?
				if (MOG_ControllerProject.GetProject().GetUser(name) != null)
				{
					// Is this user different from the current user?
					if (string.Compare(name, mCurrentUsersBox, true) != 0)
					{
						MOG_Privileges privs = MOG_ControllerProject.GetPrivileges();

						if (privs.GetUserPrivilege(MOG_ControllerProject.GetUserName(), MOG_PRIVILEGE.ViewOtherUsersInbox) ||
							String.Compare(MOG_ControllerProject.GetUserName(), name, true) == 0)
						{
							mInboxAssetsDirectory = "";
							mOutboxAssetsDirectory = "";
							mCurrentUsersBox = name;

							// Set the active user
							guiUser user = new guiUser(mainForm);
							user.SetActiveUser(name);

							// Are we in our inbox or another teammate
							Color formBackColor = SystemColors.Window;
							if (string.Compare(name, MOG_ControllerProject.GetUserName(), true) != 0)
							{
								// We are not in our boxes, so lets color them
								formBackColor = Color.LavenderBlush;
							}

							mainForm.AssetManagerInboxListView.BackColor = formBackColor;
							mainForm.AssetManagerSentListView.BackColor = formBackColor;
							mainForm.AssetManagerTrashListView.BackColor = formBackColor;
							mainForm.AssetManagerDraftsListView.BackColor = formBackColor;

							RefreshAllWindows();
						}
						else
						{
							MOG_Prompt.PromptResponse("Insufficient Privileges", "Your privileges do not allow you to view other users' inboxes.");
						}
					}
					else
					{
						// User is the same, so just update our gui to show correct department and user
						SetActiveUser(mCurrentUsersBox);
					}
				}
			}
		}

		public void UpdateAssetManagerTabText(MOG_Filename assetFilename)
		{
			// Update each of the tabs text
			if (assetFilename.IsDrafts())
			{
				UpdateAssetManagerDraftsTabText(true);
			}
			if (assetFilename.IsInbox())
			{
				UpdateAssetManagerInboxTabText(true);
			}
			if (assetFilename.IsOutBox())
			{
				UpdateAssetManagerOutboxTabText(true);
			}
			if (assetFilename.IsTrash())
			{
				UpdateAssetManagerTrashTabText(true);
			}
			if (assetFilename.IsLocal())
			{
				UpdateAssetManagerLocalTabText(true);
			}
		}

		public void UpdateAssetManagerDraftsTabText(bool bReportCount)
		{
			string text = "Drafts";
			if (bReportCount &&
				mainForm.AssetManagerDraftsListView.Items.Count > 0)
			{
				text += " (" + mainForm.AssetManagerDraftsListView.Items.Count.ToString() + ")";
			}
			mainForm.AssetManagerDraftsAssetsTabPage.Text = text;
		}

		public void UpdateAssetManagerInboxTabText(bool bReportCount)
		{
			string text = "Inbox";
			if (bReportCount &&
				mainForm.AssetManagerInboxListView.Items.Count > 0)
			{
				text += " (" + mainForm.AssetManagerInboxListView.Items.Count.ToString() + ")";
			}
			mainForm.AssetManagerInboxAssetsTabPage.Text = text;
		}

		public void UpdateAssetManagerOutboxTabText(bool bReportCount)
		{
			string text = "Sent";
			if (bReportCount &&
				mainForm.AssetManagerSentListView.Items.Count > 0)
			{
				text += " (" + mainForm.AssetManagerSentListView.Items.Count.ToString() + ")";
			}
			mainForm.AssetManagerAssetOutboxAssetsTabPage.Text = text;
		}

		public void UpdateAssetManagerTrashTabText(bool bReportCount)
		{
			string text = "Trash";
			if (bReportCount &&
				mainForm.AssetManagerTrashListView.Items.Count > 0)
			{
				text += " (" + mainForm.AssetManagerTrashListView.Items.Count.ToString() + ")";
			}
			mainForm.AssetManagerAssetTrashAssetsTabPage.Text = text;
		}

		public void UpdateAssetManagerLocalTabText(bool bReportCount)
		{
			if (mainForm.AssetManagerLocalDataTabControl != null &&
				mainForm.AssetManagerLocalDataTabControl.SelectedTab != null)
			{
				MOG_ControllerSyncData branch = (MOG_ControllerSyncData)mainForm.AssetManagerLocalDataTabControl.SelectedTab.Tag;
				if (branch != null)
				{
					string text = branch.GetName();
					if (bReportCount)
					{
						if (mLocal.mPlatformListView != null &&
							mLocal.mPlatformListView.Items.Count > 0)
						{
							text += " (" + mLocal.mPlatformListView.Items.Count.ToString() + ")";
						}
					}
					
					// Check if we are always active?
					if (branch.IsAlwaysActive())
					{
						// Add a nice yet simple checkmark at the end of the tab's text to denote we are always active
						int specialMarker = 0x221A;
						text += " " + (char)specialMarker;
					}

					mainForm.AssetManagerLocalDataTabControl.SelectedTab.Text = text;
				}
			}
		}


		/// <summary>
		/// RefreshWindows()
		/// Adds icons to both the outbox and inbox listView windows
		/// for each file found in the contents.info's
		/// </summary>
		public void RefreshActiveWindow()
		{
			// We can only refresh the boxes when our objects have been constructed
			if (mAssets != null)
			{
				// Only refresh the active tab in the inbox
				switch(mainForm.AssetManagerInboxTabControl.SelectedTab.Name)
				{
					case "AssetManagerInboxAssetsTabPage":
						mAssets.RefreshInbox();
						mainForm.AssetManagerInboxListView.Focus();
						break;
					case "AssetManagerDraftsAssetsTabPage":
						mAssets.RefreshDrafts();
						mainForm.AssetManagerDraftsListView.Focus();
						break;
					case "AssetManagerAssetOutboxAssetsTabPage":
						mAssets.RefreshOutbox();
						mainForm.AssetManagerSentListView.Focus();
						break;
					case "AssetManagerAssetTrashAssetsTabPage":
						mTrash.Refresh();
						mainForm.AssetManagerTrashListView.Focus();
						break;				
				}
			}
		}

		public void RefreshAllWindows()
		{
			// We can only refresh the boxes when our objects have been constructed
			if (mAssets != null)
			{
				mAssets.RefreshInbox();
				mainForm.AssetManagerInboxListView.Focus();

				mAssets.RefreshDrafts();
				mainForm.AssetManagerDraftsListView.Focus();

				mAssets.RefreshOutbox();
				mainForm.AssetManagerSentListView.Focus();

				mTrash.Refresh();
				mainForm.AssetManagerTrashListView.Focus();
			}
		}

		/// <summary>
		/// Find the correct listview for this new asset name
		/// </summary>
		/// <param name="box"></param>
		/// <param name="type"></param>
		/// <param name="userName"></param>
		/// <returns></returns>
		public ListView IsolateListView(string box, MOG_FILENAME_TYPE type, string userName)
		{
			// Bail if this is an event for anothers box but not if the username is blank.  Blank user names are from our local data window
			if (MOG_ControllerProject.IsProject() && 
				MOG_ControllerProject.GetActiveUser() != null && 
				(string.Compare(userName, MOG_ControllerProject.GetActiveUser().GetUserName(), true) != 0) && 
				(userName.Length != 0) )
			{
				return null;
			}

			if (string.Compare(box, "Inbox", true) == 0)
			{
				mActiveAssetsDirectory = mInboxAssetsDirectory;
				switch(type)
				{
					case MOG_FILENAME_TYPE.MOG_FILENAME_Asset:
					case MOG_FILENAME_TYPE.MOG_FILENAME_Group:
					case MOG_FILENAME_TYPE.MOG_FILENAME_Link:
						return mainForm.AssetManagerInboxListView;
					default:
						return null;
				}				
			}
			else if (string.Compare(box, "Drafts", true) == 0)
			{
				mActiveAssetsDirectory = mInboxAssetsDirectory;
				switch(type)
				{
					case MOG_FILENAME_TYPE.MOG_FILENAME_Asset:
					case MOG_FILENAME_TYPE.MOG_FILENAME_Group:
					case MOG_FILENAME_TYPE.MOG_FILENAME_Link:
						return mainForm.AssetManagerDraftsListView;
					default:
						return null;
				}				
			}
			else if (string.Compare(box, "Outbox", true) == 0)
			{
				mActiveAssetsDirectory = mOutboxAssetsDirectory;
				switch(type)
				{
					case MOG_FILENAME_TYPE.MOG_FILENAME_Asset:
					case MOG_FILENAME_TYPE.MOG_FILENAME_Group:
					case MOG_FILENAME_TYPE.MOG_FILENAME_Link:
						return mainForm.AssetManagerSentListView;
					default:
						return null;
				}				
			}
			else if (string.Compare(box, "Trash", true) == 0)
			{
				mActiveAssetsDirectory = "";
				switch(type)
				{
					case MOG_FILENAME_TYPE.MOG_FILENAME_Asset:
					case MOG_FILENAME_TYPE.MOG_FILENAME_Group:
					case MOG_FILENAME_TYPE.MOG_FILENAME_Link:
						return mainForm.AssetManagerTrashListView;
					default:
						return null;
				}				
			}
			else if (string.Compare(box, "", true) == 0) 
			{
				mActiveAssetsDirectory = "";
				switch (type)
				{
					case MOG_FILENAME_TYPE.MOG_FILENAME_Asset:	// This is an asset in the local data area
					case MOG_FILENAME_TYPE.MOG_FILENAME_Group:
					case MOG_FILENAME_TYPE.MOG_FILENAME_Link:
						return this.mLocal.GetLocalWorkSpaceListView();
					default:
						return null;
				}
			}


			return null;
		}

		/// <summary>
		/// Update an assets lock status
		/// </summary>
		/// <param name="command"></param>
		public void RefreshLockStatus(MOG_Command command)
		{
			MOG_Command newLock = command.GetCommand();
			mAssets.RefreshLockStatus(newLock);
		}

		/// <summary>
		/// Update the inboxes with the new command that just came in
		/// </summary>
		/// <param name="command"></param>
		public void RefreshWindowsBoth(MOG_Command command)
		{
			MOG_Filename del = new MOG_Filename(command.GetSource());
			MOG_Filename add = new MOG_Filename(command.GetDestination());

			// We need to strip off the root folder so that we get a more accurate user path
			// Without this change, we got y:\\Projects\Users when we selected a root drive at the repository
			string userPath = MOG_ControllerProject.GetActiveUser().GetUserPath().ToLower();

			// Check if the add is within the inboxes
			if (add.IsWithinInboxes())
			{
				// Check if we are not within the path that is relevant for this user?
				if (!add.IsWithinPath(userPath))
				{
					// Eat it
					add.SetFilename("");
				}
			}
			// Check if this is outside our current workspace directory?
			else if (add.IsLocal())
			{
				// Check if we are not within the path that is relevant for this user?
				if (!add.IsWithinPath(MOG_ControllerProject.GetWorkspaceDirectory()))
				{
					// Eat it
					add.SetFilename("");
				}
			}

			// Check if the del is within the inboxes
			if (del.IsWithinInboxes())
			{
				// Check if we are not within the path that is relevant for this user?
				if (!del.IsWithinPath(userPath))
				{
					// Eat it
					del.SetFilename("");
				}
			}
			// Check if this is outside our current workspace directory?
			else if (del.IsLocal())
			{
				// Check if we are not within the path that is relevant for this user?
				if (!del.IsWithinPath(MOG_ControllerProject.GetWorkspaceDirectory()))
				{
					// Eat it
					del.SetFilename("");
				}
			}

			// Determin which one we can test for the asset type?
			MOG_Filename check = (add.GetOriginalFilename().Length != 0) ? add : del;
			switch(check.GetFilenameType())
			{
				case MOG_FILENAME_TYPE.MOG_FILENAME_Group:
				case MOG_FILENAME_TYPE.MOG_FILENAME_Asset:
				case MOG_FILENAME_TYPE.MOG_FILENAME_Link:
					if (mAssets != null)
					{
						mAssets.RefreshBox(add, del, command);
					}
					break;				
			}

			//Application.DoEvents();
		}

		/// <summary>
		/// Find a list view item within a listview by name
		/// </summary>
		/// <param name="name"></param>
		/// <param name="list"></param>
		/// <returns></returns>
		public int ListViewItemFindItem(string name, ListView list)
		{
			int indexCounter = ColumnNameFind(list.Columns, "Name");
			if (indexCounter >= 0)
			{
				foreach (ListViewItem item in list.Items)
				{
					if (string.Compare(name, item.SubItems[indexCounter].Text, true) == 0)
					{
						return item.Index;
					}
				}
			}

			return -1;
		}

		/// <summary>
		/// Locates a string in an array and returns its index or 0, if not found
		/// </summary>
		/// <param name="cols"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public int ColumnNameFind(ListView.ColumnHeaderCollection columns, string name)
		{
			return ColumnNameFind( columns, name, false );
		}

		/// <summary>
		/// USE FOR DEBUGGING PURPOSES ONLY.  If returnNegativeOneIfMissing is `true`, this will return an 
		///  invalid index, which will then cause any non-robust code to throw (thereby allowing you to
		///  find why the first column of a given ListView is being set to the incorrect value).
		/// </summary>
		/// <param name="columns"></param>
		/// <param name="name"></param>
		/// <param name="returnNegativeOneIfMissing">If true, this method will return -1 for a missing name</param>
		/// <returns></returns>
		public int ColumnNameFind(ListView.ColumnHeaderCollection columns, string name, bool returnNegativeOneIfMissing )
		{
			int x = 0;
			foreach (ColumnHeader col in columns)
			{
				if (string.Compare(col.Text, name, true) == 0)
				{
					return x;
				}

				x++;
			}
			
			if( returnNegativeOneIfMissing )
			{
				return -1;
			}
			return 0;
		}

		/// <summary>
		/// Find a list view item withtin a list view bt full name
		/// </summary>
		/// <param name="name"></param>
		/// <param name="list"></param>
		/// <returns></returns>
		public int ListViewItemFindFullItem(MOG_Filename filename, ListView list)
		{
			// I really don't like this solution...but, we need to be able to find the index in the list,
			// In the future, I think we need to handle new unrecorded classifications differently.

			// Attempt to obtain the index of this item in the list
			int index = ListViewItemFindFullItem(filename.GetEncodedFilename(), list);
			if (index == -1)
			{
				// We should look one more time for the full name anytime we fail
				index = ListViewItemFindFullItem(filename.GetFullFilename(), list);
			}

			return index;
		}

		public int ListViewItemFindFullItem(string name, ListView list)
		{
			try
			{
				int indexCounter = ColumnNameFind(list.Columns, "FullName");
				if (indexCounter >= 0)
				{
					foreach (ListViewItem item in list.Items)
					{					
						string source = item.SubItems[indexCounter].Text;
						if(string.Compare(name, source, true) == 0)
						{
							return item.Index;
						}
					}
				}
			}
			catch( Exception ex )
			{
				//ex.ToString();
				System.Diagnostics.Debug.WriteLine("Exception in ListViewItemFindFullItem", ex.Message);
			}

			return -1;
		}


		/// <summary>
		/// Create a new ListView node for the asset manager inboxes
		/// </summary>
		/// <param name="pProperties"></param>
		/// <param name="nodeColor"></param>
		/// <returns></returns>
		public static ListViewItem NewListViewItem(MOG_Filename asset, string status, MOG_Properties pProperties)
		{
			// Create a new list view node
			ListViewItem item = new ListViewItem();

			// Add placeholder SubItems for each column
			while (item.SubItems.Count < (int)AssetBoxColumns.TOTAL_COLUMNS)
			{
				item.SubItems.Add("");
			}

			UpdateListViewItem(item, asset, status, pProperties);

			return item;
		}

		/// <summary>
		/// Update an existing ListView node for the asset manager inboxes
		/// </summary>
		/// <param name="pProperties"></param>
		/// <param name="nodeColor"></param>
		/// <returns></returns>
		public static void UpdateListViewItem(ListViewItem item, MOG_Filename asset, string status, MOG_Properties pProperties)
		{
			string date = "";
			string size = "";
			string creator = "";
			string owner = "";
			string group = "";
			string target = "";

			// Check if we have a properties?
			if (pProperties != null)
			{
				// If we have a valid gameDataController, set our platform scope
				if (MOG_ControllerProject.GetCurrentSyncDataController() != null)
				{
					// Set our current platform
					pProperties.SetScope(MOG_ControllerProject.GetCurrentSyncDataController().GetPlatformName());
				}

				// Gather the following info from our properties
				date = MOG_Time.FormatTimestamp(pProperties.CreatedTime, "");
				size = guiAssetController.FormatSize(pProperties.Size);
				creator = pProperties.Creator;
				owner = pProperties.Owner;
				group = pProperties.Group;

				// Check if this is a packaged asset?
				if (pProperties.IsPackagedAsset)
				{
					// Check if we have have any package assignments in our propeerties?
					ArrayList packages = pProperties.GetPackages();
					if (packages.Count == 0)
					{
						// Indicate this is a packaged asset w/o any package assignments
						target = "Missing package assignment...";
					}
					else if (packages.Count == 1)
					{
						MOG_Property package = packages[0] as MOG_Property;
                        MOG_Filename packageName = new MOG_Filename(MOG_ControllerPackage.GetPackageName(package.mPropertyKey));
                        string packageGroupPath = MOG_ControllerPackage.GetPackageGroups(package.mPropertyKey);
						string displayString = MOG_ControllerPackage.CombinePackageAssignment(packageName.GetAssetLabel(), packageGroupPath, "");
						target = "{Package} " + displayString + "  in  " + MOG_Filename.GetAdamlessClassification(packageName.GetAssetClassification());
					}
					else
					{
						target = "{Package} " + packages.Count + " Assignments...";
					}
				}
				else if (pProperties.SyncFiles)
				{
					// Get the formatted SyncTarget of this asset
					target = MOG_Tokens.GetFormattedString("{Workspace}\\" + pProperties.SyncTargetPath, asset, pProperties.GetPropertyList());
				}
			}

			item.Text = asset.GetAssetLabel();

			// Populate the item's SubItems
			// I tried for a long time to be smart here and use ColumnNameFind(thisListView.Columns, "Name") but
			// I kept running into walls because this function is used by a lot of workers outside of the ListView's thread.
			// So, I gave up and am just going to do it the ugly brute force way!  YUCK!!
			item.SubItems[(int)AssetBoxColumns.NAME].Text = asset.GetAssetLabel();
			item.SubItems[(int)AssetBoxColumns.CLASS].Text = asset.GetAssetClassification();
			item.SubItems[(int)AssetBoxColumns.TARGETPATH].Text = target;
			item.SubItems[(int)AssetBoxColumns.DATE].Text = date;
			item.SubItems[(int)AssetBoxColumns.SIZE].Text = size;
			item.SubItems[(int)AssetBoxColumns.PLATFORM].Text = asset.GetAssetPlatform();
			item.SubItems[(int)AssetBoxColumns.STATE].Text = status;
			item.SubItems[(int)AssetBoxColumns.CREATOR].Text = creator;
			item.SubItems[(int)AssetBoxColumns.RESPPARTY].Text = owner;
			item.SubItems[(int)AssetBoxColumns.OPTIONS].Text = "";
			item.SubItems[(int)AssetBoxColumns.FULLNAME].Text = asset.GetEncodedFilename();
			item.SubItems[(int)AssetBoxColumns.BOX].Text = asset.GetBoxName();
			item.SubItems[(int)AssetBoxColumns.GROUP].Text = group;

			// Set the item's Icons
			item.ImageIndex = MogUtil_AssetIcons.GetFileIconIndex(asset.GetEncodedFilename(), pProperties);

			if (MogMainForm.MainApp != null &&
				MogMainForm.MainApp.mAssetManager != null )
			{
				// mAssetStatus.GetStatusInfo() is sort of a black sheep and should maybe become static
				item.StateImageIndex = MogMainForm.MainApp.mAssetManager.mAssetStatus.GetStatusInfo(status).IconIndex;
			}

			// Set the item's color
			item.ForeColor = MOG_AssetStatus.GetColor(status);
			// Check if this is a local item that has been blessed?
			if (asset.IsLocal() &&
				string.Compare(status, MOG_AssetStatus.GetText(MOG_AssetStatusType.Blessed), true) == 0)
			{
				// Mark local blessed items with light gray
				item.ForeColor = Color.LightGray;
			}
		}		
		
		/// <summary>
		/// Perform a merge with the master repository
		/// </summary>
		/// <param name="silent"></param>
		public void BuildMerge(bool ShowSyncWindow, bool silent)
		{
			BuildMerge(ShowSyncWindow, silent, "", "", false, false);
		}
		public void BuildMerge(bool ShowSyncWindow, bool silent, string tag, string filter, bool updateModifiedMissing, bool cleanUnknownFiles)
		{
			//get the sync data controller
			MOG_ControllerSyncData gameDataHandle = MOG_ControllerProject.GetCurrentSyncDataController();

			//if we don't have one infom the user that we need one
			if (gameDataHandle == null)
			{
				MOG_Prompt.PromptMessage("Update Build", "Cannot update a build without a valid local Workspace and Workspace tab created.  Create a local Workspace first then try again.", Environment.StackTrace);
				return;
			}

			SyncLatestForm update = new SyncLatestForm(mainForm, mainForm.mAdministratorMode, tag, filter, mDefaultUpdateBuildType, gameDataHandle.GetSyncDirectory(), mCurrentVersion, mHidePlatforms, updateModifiedMissing, cleanUnknownFiles);

			try
			{
				//if we are expected to show the SyncWindow
				if (ShowSyncWindow)
				{
					if (update.ShowDialog(mainForm) == DialogResult.OK)
					{
						// Make sure we update our local variables that could get changed within the dialog
						tag = update.SyncTag;
						updateModifiedMissing = update.UpdateBuildCheckMissingCheckBox.Checked;
						cleanUnknownFiles = update.UpdateBuildCleanUnknownFilesCheckBox.Checked;
					}
					else
					{
						//the result is not ok, so do not proceed with the sync
						return;
					}
				}
				
				//get the sync directory from the SyncDataController
				string targetProjectPath = gameDataHandle.GetSyncDirectory();

				// Get the current SyncData controller
				MOG_ControllerSyncData sync = MOG_ControllerProject.GetCurrentSyncDataController();
				if (sync != null)
				{
					// Check if the user has specified a cleaning?
					if (cleanUnknownFiles)
					{
						// Check if we need to clean the workspace?
						string startingPath = sync.GetSyncDirectory();
						try
						{
							MogUtil_WorkspaceCleaner cleaner = new MogUtil_WorkspaceCleaner();
							cleaner.Clean(startingPath);
						}
						catch (Exception ex)
						{
							MOG_Prompt.PromptMessage("Clean", "The cleaner exited with the following error (" + ex.Message + ")");
						}
					}

					// Attempt to use the dialog's MOG_LocalSyncInfo
					MOG_LocalSyncInfo localSyncInfo = update.mLocalSyncInfo;
					if (localSyncInfo == null)
					{
						// Looks like we need to build our own MOG_LocalSyncInfo
						string computerName = MOG_ControllerSystem.GetComputerName();
						string projectName = MOG_ControllerProject.GetProjectName();
						string platformName = MOG_ControllerProject.GetPlatformName();
						string syncDirectory = MOG_ControllerProject.GetCurrentSyncDataController().GetSyncDirectory();
						string userName = MOG_ControllerProject.GetUserName();
						string classification = MOG_ControllerProject.GetProjectName();
						localSyncInfo = new MOG_LocalSyncInfo(computerName, projectName, platformName, syncDirectory, tag, userName, classification, "");
					}

					// Check if we want to update missing
					sync.SyncRepositoryData(MOG_ControllerProject.GetProjectName(), update.Exclusions, update.Inclusions, updateModifiedMissing, localSyncInfo);

					// Show the summary file
					SyncLatestSummaryForm summary = new SyncLatestSummaryForm(sync.GetSyncLog());
					if (silent)
					{
						summary.Show(mainForm);
					}
					else
					{
						summary.ShowDialog(mainForm);
					}
				}

				guiUserPrefs.SaveDynamic_LayoutPrefs("AssetManager", mainForm);

				update.Dispose();
			}
			catch( Exception e)
			{
				MOG_Report.ReportMessage("Update", "Could not perform update due to error:\n\n" + e.Message, e.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.ERROR);
			}
		}

		/// <summary>
		/// Post the build in the users data directory to their inbox
		/// <summary>
		public void BuildPost()
		{
			MOG_ControllerSyncData gameDataHandle = MOG_ControllerProject.GetCurrentSyncDataController();
			try
			{
				string ProjectName		= gameDataHandle.GetProjectName();
				string ProjectDir		= gameDataHandle.GetSyncDirectory();
				string ProjectPlatform	= gameDataHandle.GetPlatformName();
				string ImportName		= string.Concat(MOG_ControllerProject.GetProjectName(), ".Build.Release.", ProjectName, ".", ProjectPlatform);
				string command			= string.Concat(MOG_ControllerProject.GetProject().GetProjectToolsPath(), "\\MOG_UserBless.bat"); 
				string output			= "";

				// Make sure the tool we need exits
				if (DosUtils.FileExist(command))
				{
					guiCommandLine.ShellSpawn(command, string.Concat(ProjectName, " ", ProjectDir, " ", ProjectPlatform, " ", ImportName), ProcessWindowStyle.Normal, ref output);				
				}
				else
				{
					MOG_Prompt.PromptMessage("Tool", string.Concat("This tool(",command,") is missing."), Environment.StackTrace);
				}
			}
			catch( Exception e)
			{
				MOG_Prompt.PromptMessage("Build Post", "Could not perform post due to error:\n\n" + e.Message, e.StackTrace);
			}
		}

		/// <summary>
		/// Determines if the asset specified by file is within the target project path
		/// </summary>
		private bool FindMergedBinary(string file, ArrayList MergeableAssets, string targetProjectPath)
		{
			// Each file will be checked against all the mergable assets in the project
			foreach(string asset in MergeableAssets)
			{
				//string []parts = asset.Split(",;".ToCharArray());

				string targetfile = file.ToLower().Replace(targetProjectPath, "");
				targetfile = targetfile.TrimStart("\\".ToCharArray());

				// If we find it, we will abort with a true
				if (string.Compare(asset.TrimStart("\\".ToCharArray()), targetfile, true) == 0)
				{
					return true;
				}
			}

			// Worst case, we went through them all and failed to find a match
			return false;
		}
	}
}

