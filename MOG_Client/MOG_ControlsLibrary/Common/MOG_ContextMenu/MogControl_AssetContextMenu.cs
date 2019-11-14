using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using Microsoft.VisualBasic;
using Microsoft.Win32;
using System.Runtime.InteropServices;

using MOG_Client.Forms.AssetProperties;

using MOG_Client;
using MOG_Client.Forms;
using MOG_Client.Client_Gui;
using MOG_Client.Client_Utilities;
using MOG_Client.Client_Mog_Utilities;
using MOG_ControlsLibrary.MogUtils_Settings;

using MOG;
using MOG.INI;
using MOG.TIME;
using MOG.USER;
using MOG.PROJECT;
using MOG.PLATFORM;
using MOG.PROPERTIES;
using MOG.REPORT;
using MOG.PROMPT;
using MOG.PROGRESS;
using MOG.FILENAME;
using MOG.DOSUTILS;
using MOG.CONTROLLER;
using MOG.COMMAND;
using MOG.DATABASE;
using MOG.CONTROLLER.CONTROLLERASSET;
using MOG.CONTROLLER.CONTROLLERSYNCDATA;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERLIBRARY;
using MOG.CONTROLLER.CONTROLLERPACKAGE;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.CONTROLLER.CONTROLLERINBOX;
using MOG.CONTROLLER.CONTROLLERREPOSITORY;
using MOG.ASSET_STATUS;
using MOG.SYSTEMUTILITIES;

using MOG_CoreControls;
using MOG_ControlsLibrary;
using MOG_ControlsLibrary.Controls;
using MOG_ControlsLibrary.Forms;
using MOG_ControlsLibrary.Utils;
using MOG_ControlsLibrary.Wizards;
using MOG_ControlsLibrary.Common.MogControl_RepositoryTreeViews;
using MOG_ControlsLibrary.Common;
using MOG_Client.Client_Mog_Utilities.AssetOptions;

using Tst;
using Iesi.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.Specialized;
using MOG_Client.Utilities;
using MOG_Client.Properties;
using System.Text;
using MOG_Client.Client_Gui.AssetManager_Helper;
using MOG.EXCEPTION;


namespace MOG_ControlsLibrary.Common.MOG_ContextMenu
{
    // 'Open with' structure
    [Serializable]
    public struct ShellExecuteInfo
    {
        public int Size;
        public uint Mask;
        public IntPtr hwnd;
        public string Verb;
        public string File;
        public string Parameters;
        public string Directory;
        public uint Show;
        public IntPtr InstApp;
        public IntPtr IDList;
        public string Class;
        public IntPtr hkeyClass;
        public uint HotKey;
        public IntPtr Icon;
        public IntPtr Monitor;
    }

	/// <summary>
	/// Populates an Asset-related ContextMenu for use by Mog_Client
	/// </summary>
	public class MogControl_AssetContextMenu : ContextMenuStrip
	{
		public delegate void MogMenuItem_Click(object sender, System.EventArgs e);

        // Code For OpenWithDialog Box
        [DllImport("shell32.dll", SetLastError = true)]
        extern public static bool ShellExecuteEx(ref ShellExecuteInfo lpExecInfo);
        public const uint SW_NORMAL = 1;


		#region MenuItem and other Constants
		public const string ImportMenu_Text					= "{import-asset-tree-form}";

		public const string Group_MenuItemText				= "Group";
		public const string UnGroup_MenuItemText			= "UnGroup";

		public const string Refresh_MenuItemText			= "Refresh";
		
		public const string Bless_MenuItemText				= "Bless...";		
		public const string Process_MenuItemText			= "Rip";
		public const string UpdateLocal_MenuItemText		= "Update Local";
		public const string Rename_MenuItemText				= "Rename...";
		public const string	Explore_MenuItemText			= "Explore";
		
		public const string	View_MenuItemText				= "View";
		public const string	ViewSpecial_MenuItemText		= "View Menu";
		public const string	ViewImported_MenuItemText		= "Imported";
		public const string	ViewSource_MenuItemText			= "Source";
		public const string	ViewProcessed_MenuItemText		= "Processed";

		public const string	DblClick_MenuItemText			= "DoubleClick Default <";

		public const string	Properties_MenuItemText			= "Properties...";
		public const string ShowComments_MenuItemText		= "Show Comments...";
		public const string SendTo_MenuItemText				= "Send To...";
		public const string	CopyTo_MenuItemText				= "Copy To...";
		public const string	Delete_MenuItemText				= "Delete...";
		public const string	Remove_MenuItemText				= "Remove from report...";
		public const string	RevertUpdated_MenuItemText		= "Revert to last GetLatest";
		public const string ClearUpdatedFromList_MenuItemText = "Clear From List";
		public const string Advanced_MenuItemText			= "Advanced";
		
		public const string	Repackage_MenuItemText			= "Repackage Asset";

		public const string Reject_MenuItemText				= "Reject";

		public const string	Restore_MenuItemText			= "Restore";
		public const string	Lock_MenuItemText				= "Lock";
		public const string	UnLock_MenuItemText				= "UnLock";
		public const string	KillLock_MenuItemText			= "Kill Lock";
		public const string	PackageManagement_MenuItemText	= "Package Management...";
		public const string	RemoveFromProject_MenuItemText	= "Remove From Project...";
		public const string	MakeCurrent_MenuItemText		= "Make Current...";
		public const string	GenerateReport_MenuItemText 	= "Generate Report...";
		public const string	ExportBinaries_MenuItemText 	= "Export Binaries";
		public const string	CreatePackage_MenuItemText		= "Create Package...";
		public const string	RebuildPackage_MenuItemText		= "Rebuild Package...";
		public const string	CustomTools_MenuItemText		= "Custom Tools";
		public const string	AdminTools_MenuItemText			= "Admin Tools";
		public const string AdminToolsCleanInvalidItems_MenuItemText = "Admin Tools - Clean Invalid Items";
		public const string AdminToolsReportUnreferencedRevisions_MenuItemText = "Admin Tools - Report Unreferenced Revisions";
		public const string AdminToolsReportNewerUnpostedRevisions_MenuItemText = "Admin Tools - Report newer unposted revisions";
		public const string AdminToolsReportMultiplePackageAssignments_MenuItemText = "Admin Tools - Report multiple package assignments";
		public const string AdminToolsReportCollidingPackageAssignments_MenuItemText = "Admin Tools - Report colliding package assignments";
		public const string AdminToolsReportInvalidPackageAssignments_MenuItemText = "Admin Tools - Report invalid package assignments";
		public const string AdminToolsRepairRevisionMetadata_MenuItemText = "Admin Tools - Repair Revision Metadata";
		public const string AdminToolsVerifyUpdateSqlTables_MenuItemText = "Admin Tools - Verify and Update Sql Tables (All Projects)";
//		public const string	AdminToolsRepairDB_MenuItemText	= "Admin Tools - Repair GameDataTable (missing '\\')";
//		public const string AdminToolsUpdateProjectFor304XDrop_MenuItemText = "Admin Tools - Update project for 3.0.4.X Drop";
				
		public const string Sync_MenuItemText					= "GetLatest...";
		public const string Clean_MenuItemText					= "Clean...";
		public const string LibraryCheckout_MenuItemText		= "Checkout...";
		public const string LibraryCheckin_MenuItemText			= "Checkin...";
		public const string LibraryUndoCheckout_MenuItemText	= "Undo Checkout";
		public const string LibraryAdd_MenuItemText				= "Add";
		public const string LibraryEdit_MenuItemText			= "{LibraryEdit}";
		public const string LibraryView_MenuItemText			= "{LibraryView}";
        public const string LibraryExplore_MenuItemText			= "Explore File";
		public const string LibraryEdit_VisibleMenuItemText		= "Edit...";
        public const string LibraryEditWith_MenuItemText		= "Edit With...";
		public const string LibraryExportToInbox_MenuItemText	= "Export to Inbox";
		public const string LibraryView_VisibleMenuItemText		= "View";
		public const string LibraryViewRevisions_MenuItemText	= "Goto Archived Revisions...";
		public const string LibraryExploreFolder_MenuItemText	= "Explore Folder";
		public const string LibraryCreateFolder_MenuItemText	= "Create Folder...";
		public const string LibraryEditFolder_MenuItemText		= "Edit Folder...";
		public const string LibraryDeleteFolder_MenuItemText	= "Delete Folder";
		public const string LibraryDeleteFile_MenuItemText		= "Delete File";

		public const string LibrarySetTemplate_MenuItemText		= "Set as template...";
	
		public const string KillConnection_MenuItemText		= "Kill Connection";
		public const string StartSlave_MenuItemText			= "Start Slave";
		public const string RetaskCommand_MenuItemText		= "Retask Command";
		public const string RemoteDesktop_MenuItemText		= "Remote Desktop";

		public const string DeleteCommand_MenuItemText		    = "Delete Command";
        public const string DeletePackageCommand_MenuItemText   = "Delete Package Command";
        public const string DeletePostCommand_MenuItemText      = "Delete Post Command";

		public const string RestartCommand_MenuItemText			= "Restart Command";
		public const string ViewCommand_MenuItemText			= "View Command";
		public const string RestartPackageCommand_MenuItemText	= "Restart Package Command";
		public const string RestartPostCommand_MenuItemText		= "Restart Post Command";

        public const string RestartLateResolverCommand_MenuItemText = "Restart LateResolver Command";
        public const string DeleteLateResolverCommand_MenuItemText = "Delete LateResolver Command";

		public const string ProjectGoToAssetCommand_MenuItemText= "Lookup asset(project tab)";
		public const string ProjectCopyAssetToClipboard_MenuItemText = "Copy";
		public const string ProjectPasteAssetFromClipboard_MenuItemText	= "Paste";
					      
		public const string AddClassification_MenuItemText		= "Add Classification...";
		public const string RenameClassification_MenuItemText	= "Rename Classification...";
		public const string DeleteClassification_MenuItemText	= "Delete Classification";
		public const string ClassificationAddDelete_MessageText = "Cannot perform this task, except in "
			+ "Workspace View's (left panel).";

		public const string RenameGroup_MenuItemText		= "Rename Group";
		public const string ChangeProperties_MenuItemText	= "Change Properties";
		public const string ChangePropertiesAdd_MenuItemText = "Create/edit property menu...";
		#endregion MenuItem and other Constants

		#region Member variables, Properties
		private MogControl_ColumnContextMenu mColumnMenu;
		private string mMenuTemplateName;
		private ListView mView;
		private TreeView mTree;
		private bool mShowPropertyMenu = false;
		private string []mColumns;
		private ArrayList mProperties = new ArrayList();
		//		private MOG_Client.MogMainForm mainForm;
		private ArrayList mMenuPrivileges;
		private HybridDictionary mPropertyMenu;

		public ListView SetView 
		{
			get { return mView; }
			set
			{
				mView = value; 
				mTree = null;
			}
		}
		public TreeView SetTree 
		{
			get { return mTree; }
			set
			{
				mTree = value; 
				mView = null;
			}
		}
		#endregion Member variables, Properties

		#region Constructors
		public MogControl_AssetContextMenu(TreeView tree)
			: this("", tree, null){}
		public MogControl_AssetContextMenu(string columns)
			: this( columns, null, null ){}
		public MogControl_AssetContextMenu(string columns, ListView view)
			: this( columns, null, view ){}
		public MogControl_AssetContextMenu(string columns, TreeView tree)
			: this( columns, tree, null ){}

		public MogControl_AssetContextMenu(System.Windows.Forms.ListView.ColumnHeaderCollection columns, ListView view)
		{
			mView = view;
			mTree = null;
			mMenuPrivileges = new ArrayList();

			mColumns = new string[columns.Count];
			for (int x = 0; x < mColumns.Length; x++)
			{
				mColumns[x] = columns[x].Text;
			}

			mColumnMenu = null;

			Opening += ContextMenu_Opening;
		}
		
		/// <param name="columns">Columns to be displayed</param>
		/// <param name="tree">TreeView to be attached</param>
		/// <param name="list">ListView to be attached</param>
		protected MogControl_AssetContextMenu( string columns, TreeView tree, ListView list )
		{
			mView = list;
			mTree = tree;
			mMenuPrivileges = new ArrayList();
			mColumns = columns.Split(",".ToCharArray());
			for (int x = 0; x < mColumns.Length; x++)
			{
				mColumns[x] = mColumns[x].Trim();
			}

			mColumnMenu = null;

			Opening += ContextMenu_Opening;
		}
		#endregion

		#region ColumnContextMenu
		private void ContextMenu_Opening(object sender, CancelEventArgs e)
		{
			Control sourceControl = SourceControl != null ? sourceControl = SourceControl : sourceControl = MogMainForm.MainApp.TopLevelControl;
			if (PopupColumnMenu(sourceControl))
			{
				e.Cancel = true;
			}

			Prevalidate(sender as Control);
		}

		private bool PopupColumnMenu(Control parent)
		{
			if (parent != null)
			{
				Point pt = parent.PointToClient(Cursor.Position);

				// Replace the context menu with a column context menu
				if (IsInColumnHeader(parent as ListView, pt))
				{
					// Determine if we need to create the column menu
					if (mColumnMenu == null)
					{
						// Create the menu
						mColumnMenu = new MogControl_ColumnContextMenu(parent as ListView);
					}

					mColumnMenu.Show(parent, pt);
					return true;
				}
			}

			return false;
		}
		
		bool IsInColumnHeader(ListView view, Point pt)
		{
			bool bIsInHeader = false;

			if (view != null)
			{
				ListViewItem target = view.GetItemAt(pt.X, pt.Y);

				// Determine if we clicked on the column header
				if (target == null && pt.Y <= 16)
				{
					bIsInHeader = true;
				}
				else if (target != null && target.Bounds.Top > pt.Y)
				{
					bIsInHeader = true;
				}
			}

			return bIsInHeader;
		}
		#endregion

		#region Menu_Popup, Initializers, and Populators
		/// <summary>
		/// Initializes context menu for a given menu name (e.g. "Inbox")
		/// </summary>
		/// <returns>The requested context menu</returns>
		public MogControl_AssetContextMenu InitializeContextMenu(string menu)
		{
			// String representing items to be displayed in menu (comma delimited)
			menu = ResolveTemplates(menu);

			string []items = menu.Split(",".ToCharArray());
			foreach (string itemName in items)
			{
				#region Switch over all possible MenuItems
                switch (itemName.Trim())
                {
                    case RemoteDesktop_MenuItemText:
                        Items.Add(CreateItem(RemoteDesktop_MenuItemText, true,
                            Keys.None, new MogMenuItem_Click(MenuItemRemoteDesktop_Click)));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(RemoteDesktop_MenuItemText,
                            MOG_PRIVILEGE.AccessAdminTools, false, false));
                        break;
                    case KillConnection_MenuItemText:
						Items.Add(CreateItem(KillConnection_MenuItemText, true,
							Keys.Delete, new MogMenuItem_Click(MenuItemKillConnection_Click)));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(KillConnection_MenuItemText,
                            MOG_PRIVILEGE.KillConnection, false, false));
                        break;
                    case StartSlave_MenuItemText:
						Items.Add(CreateItem(StartSlave_MenuItemText, true,
							Keys.None, new MogMenuItem_Click(MenuItemStartSlave_Click)));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(StartSlave_MenuItemText,
                            MOG_PRIVILEGE.LaunchSlaveOnRemoteComputer, false, false));
                        break;
                    case RetaskCommand_MenuItemText:
						Items.Add(CreateItem(RetaskCommand_MenuItemText, true,
							Keys.None, new MogMenuItem_Click(MenuItemRetaskCommand_Click)));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(RetaskCommand_MenuItemText,
                            MOG_PRIVILEGE.RetaskCommand, false, false));
                        break;

                    case DeleteCommand_MenuItemText:
						Items.Add(CreateItem(DeleteCommand_MenuItemText, true,
							Keys.None, new MogMenuItem_Click(MenuItemDeleteCommand_Click)));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(DeleteCommand_MenuItemText,
                            MOG_PRIVILEGE.All, false, false));
                        break;

                    case DeletePackageCommand_MenuItemText:
						Items.Add(CreateItem(DeletePackageCommand_MenuItemText, true,
							Keys.None, new MogMenuItem_Click(MenuItemDeletePackageCommand_Click)));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(DeletePackageCommand_MenuItemText,
                            MOG_PRIVILEGE.All, false, false));
                        break;

                    case DeletePostCommand_MenuItemText:
						Items.Add(CreateItem(DeletePostCommand_MenuItemText, true,
							Keys.None, new MogMenuItem_Click(MenuItemDeletePostCommand_Click)));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(DeletePostCommand_MenuItemText,
                            MOG_PRIVILEGE.All, false, false));
                        break;

                    case RestartCommand_MenuItemText:
						Items.Add(CreateItem(RestartCommand_MenuItemText, true,
							Keys.None, new MogMenuItem_Click(MenuItemRestartCommand_Click)));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(RestartCommand_MenuItemText,
                            MOG_PRIVILEGE.All, false, false));
                        break;
                    case ViewCommand_MenuItemText:
						Items.Add(CreateItem(ViewCommand_MenuItemText, true,
							Keys.None, new MogMenuItem_Click(MenuItemViewCommand_Click)));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(ViewCommand_MenuItemText,
                            MOG_PRIVILEGE.All, false, false));
                        break;
                    case RestartPackageCommand_MenuItemText:
						Items.Add(CreateItem(RestartPackageCommand_MenuItemText, true,
							Keys.None, new MogMenuItem_Click(MenuItemRestartPackageCommand_Click)));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(RestartPackageCommand_MenuItemText,
                            MOG_PRIVILEGE.All, false, false));
                        break;

                    case RestartPostCommand_MenuItemText:
						Items.Add(CreateItem(RestartPostCommand_MenuItemText, true,
							Keys.None, new MogMenuItem_Click(MenuItemRestartPostCommand_Click)));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(RestartPostCommand_MenuItemText,
                            MOG_PRIVILEGE.All, false, false));
                        break;

                    case RestartLateResolverCommand_MenuItemText:
						Items.Add(CreateItem(RestartLateResolverCommand_MenuItemText, true,
							Keys.None, new MogMenuItem_Click(MenuItemRestartLateResolverCommand_Click)));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(RestartLateResolverCommand_MenuItemText,
                            MOG_PRIVILEGE.All, false, false));
                        break;

                    case DeleteLateResolverCommand_MenuItemText:
						Items.Add(CreateItem(DeleteLateResolverCommand_MenuItemText, true,
							Keys.None, new MogMenuItem_Click(MenuItemDeleteLateResolverCommand_Click)));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(DeleteLateResolverCommand_MenuItemText,
                            MOG_PRIVILEGE.All, false, false));
                        break;

                    case ProjectGoToAssetCommand_MenuItemText:
						Items.Add(CreateItem(ProjectGoToAssetCommand_MenuItemText, true,
							Keys.None, new MogMenuItem_Click(MenuItemProjectGoToAsset_Click)));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(ProjectGoToAssetCommand_MenuItemText,
                            MOG_PRIVILEGE.All, false, true));
                        break;

					case ProjectCopyAssetToClipboard_MenuItemText:
						Items.Add(CreateItem(ProjectCopyAssetToClipboard_MenuItemText, true,
							Keys.Control | Keys.C, new MogMenuItem_Click(MenuItemCopyAssetToClipboard_Click)));
                        // Add the button Privileges
						mMenuPrivileges.Add(new guiAssetMenuPrivileges(ProjectCopyAssetToClipboard_MenuItemText,
                            MOG_PRIVILEGE.All, false, false));
                        break;

					case ProjectPasteAssetFromClipboard_MenuItemText:
						Items.Add(CreateItem(ProjectPasteAssetFromClipboard_MenuItemText, true,
							Keys.Control | Keys.V, new MogMenuItem_Click(MenuItemPasteAssetFromClipboard_Click)));
                        // Add the button Privileges
						mMenuPrivileges.Add(new guiAssetMenuPrivileges(ProjectPasteAssetFromClipboard_MenuItemText,
                            MOG_PRIVILEGE.All, false, false, false, false));
                        break;
						
                    case Sync_MenuItemText:
						Items.Add(CreateItem(Sync_MenuItemText, true,
							Keys.Control | Keys.S, new MogMenuItem_Click(MenuItemSync_Click)));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(Sync_MenuItemText,
                            MOG_PRIVILEGE.All, false, false));
                        break;
                    case Clean_MenuItemText:
						Items.Add(CreateItem(Clean_MenuItemText, true,
							Keys.Control | Keys.Shift | Keys.C, new MogMenuItem_Click(MenuItemClean_Click)));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(Clean_MenuItemText,
                            MOG_PRIVILEGE.All, false, false));
                        break;
                    case LibraryCheckout_MenuItemText:
						Items.Add(CreateItem(LibraryCheckout_MenuItemText, true,
							Keys.Control | Keys.O, new MogMenuItem_Click(MenuItemLibraryCheckout_Click), Resources.MogCheckout));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(LibraryCheckout_MenuItemText,
                            MOG_PRIVILEGE.All, false, false));
                        break;
                    case LibraryCheckin_MenuItemText:
						Items.Add(CreateItem(LibraryCheckin_MenuItemText, true,
							Keys.Control | Keys.I, new MogMenuItem_Click(MenuItemLibraryCheckin_Click), Resources.MogCheckin));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(LibraryCheckin_MenuItemText,
                            MOG_PRIVILEGE.All, false, false));
                        break;
                    case LibraryUndoCheckout_MenuItemText:
						Items.Add(CreateItem(LibraryUndoCheckout_MenuItemText, true,
							Keys.Control | Keys.Shift | Keys.O, new MogMenuItem_Click(MenuItemUndoCheckout_Click), Resources.MogUndoCheckout));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(LibraryUndoCheckout_MenuItemText,
                            MOG_PRIVILEGE.All, false, false));
                        break;
                    case LibraryAdd_MenuItemText:
						Items.Add(CreateItem(LibraryAdd_MenuItemText, true,
							Keys.Control | Keys.Alt | Keys.A, new MogMenuItem_Click(MenuItemLibraryAdd_Click)));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(LibraryAdd_MenuItemText,
                            MOG_PRIVILEGE.All, false, false));
                        break;
                    case LibraryEdit_MenuItemText:
						Items.Add(CreateItem(LibraryEdit_VisibleMenuItemText, true,
							Keys.Control | Keys.Shift | Keys.E, new MogMenuItem_Click(MenuItemLibraryEdit_Click), Resources.MogEdit));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(LibraryEdit_MenuItemText,
                            MOG_PRIVILEGE.All, false, false));
                        break;
                    case LibraryExportToInbox_MenuItemText:
						Items.Add(CreateItem(LibraryExportToInbox_MenuItemText, true,
							Keys.Control | Keys.Shift | Keys.I, new MogMenuItem_Click(MenuItemLibraryExportInbox_Click)));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(LibraryExportToInbox_MenuItemText,
                            MOG_PRIVILEGE.All, false, false));
                        break;
                    case LibraryView_MenuItemText:
						Items.Add(CreateItem(LibraryView_VisibleMenuItemText, true,
							Keys.Control | Keys.Shift | Keys.V, new MogMenuItem_Click(MenuItemLibraryView_Click), Resources.MogView));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(LibraryView_MenuItemText,
                            MOG_PRIVILEGE.All, false, true));
                        break;
                    case LibraryViewRevisions_MenuItemText:
						Items.Add(CreateItem(LibraryViewRevisions_MenuItemText, true,
							Keys.Control | Keys.R, new MogMenuItem_Click(MenuItemLibraryViewRevisions_Click)));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(LibraryViewRevisions_MenuItemText,
                            MOG_PRIVILEGE.All, false, true));
                        break;
                    case LibraryExploreFolder_MenuItemText:
						Items.Add(CreateItem(LibraryExploreFolder_MenuItemText, true,
							Keys.None, new MogMenuItem_Click(MenuItemExploreLibrary_Click), Resources.MogExplore));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(LibraryExploreFolder_MenuItemText,
                            MOG_PRIVILEGE.All, false, false));
                        break;
                    case LibraryCreateFolder_MenuItemText:
						Items.Add(CreateItem(LibraryCreateFolder_MenuItemText, true,
							Keys.None, new MogMenuItem_Click(MenuItemLibraryCreateFolder_Click), Resources.MogFolderCreate));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(LibraryCreateFolder_MenuItemText,
                            MOG_PRIVILEGE.All, false, false));
                        break;
                    case LibraryEditFolder_MenuItemText:
						Items.Add(CreateItem(LibraryEditFolder_MenuItemText, true,
							Keys.None, new MogMenuItem_Click(MenuItemLibraryEditFolder_Click), Resources.MogExplore));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(LibraryEditFolder_MenuItemText,
                            MOG_PRIVILEGE.All, false, false));
                        break;
                    case LibraryDeleteFolder_MenuItemText:
						Items.Add(CreateItem(LibraryDeleteFolder_MenuItemText, true,
							Keys.Delete, new MogMenuItem_Click(MenuItemLibraryDeleteFolder_Click), Resources.MogFolderDelete));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(LibraryDeleteFolder_MenuItemText,
                            MOG_PRIVILEGE.All, false, false));
                        break;
                    case LibraryDeleteFile_MenuItemText:
						Items.Add(CreateItem(LibraryDeleteFile_MenuItemText, true,
							Keys.Delete, new MogMenuItem_Click(MenuItemLibraryDeleteFile_Click)));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(LibraryDeleteFile_MenuItemText,
                            MOG_PRIVILEGE.All, false, false));
						break;
					case LibrarySetTemplate_MenuItemText:
						Items.Add(CreateItem(LibrarySetTemplate_MenuItemText, true,
							Keys.Delete, new MogMenuItem_Click(MenuItemLibrarySetTemplate_Click)));
						// Add the button Privileges
						mMenuPrivileges.Add(new guiAssetMenuPrivileges(LibrarySetTemplate_MenuItemText,
							MOG_PRIVILEGE.All, false, false));
						break;
                    case Refresh_MenuItemText:
						Items.Add(CreateItem(Refresh_MenuItemText, true,
							Keys.F5, new MogMenuItem_Click(MenuItemRefresh_Click), Resources.MogRefresh));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(Refresh_MenuItemText,
                            MOG_PRIVILEGE.All, false, false, false, false));
                        break;
                    case Bless_MenuItemText:
						Items.Add(CreateItem(Bless_MenuItemText, true,
							Keys.Control | Keys.B, new MogMenuItem_Click(MenuItemBless_Click), Resources.MogBless));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(Bless_MenuItemText,
                            MOG_PRIVILEGE.BlessAssetFromWithinOwnInbox, true, true));
                        break;
//					case CopyEdit_MenuItemText:
//						Items.Add(CreateItem(CopyEdit_MenuItemText, true,	
//							Keys.CtrlO, new MogMenuItem_Click(MenuItemProjectEdit_Click)));
//						// Add the button Privileges
//						mMenuPrivileges.Add(new guiAssetMenuPrivileges(CopyEdit_MenuItemText, 
//							MOG_PRIVILEGE.All, false, true));
//						break;
//					case Edit_MenuItemText:
//						Items.Add(CreateItem(Edit_MenuItemText, true,	
//							Keys.CtrlO, new MogMenuItem_Click(MenuItemEdit_Click)));
//						// Add the button Privileges
//						mMenuPrivileges.Add(new guiAssetMenuPrivileges(Edit_MenuItemText, 
//							MOG_PRIVILEGE.All, true, true));
//						break;
                    case Process_MenuItemText:
						Items.Add(CreateItem(Process_MenuItemText, true,
							Keys.Control | Keys.R, new MogMenuItem_Click(MenuItemProcess_Click), Resources.MogRip));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(Process_MenuItemText,
                            MOG_PRIVILEGE.All, true, true));
                        break;
                    case Repackage_MenuItemText:
						Items.Add(CreateItem(Repackage_MenuItemText, true,
							Keys.None, new MogMenuItem_Click(MenuItemRepackage_Click), Resources.MogRepackage));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(Repackage_MenuItemText,
                            MOG_PRIVILEGE.All, false, true));
                        break;
                    case UpdateLocal_MenuItemText:
						Items.Add(CreateItem(UpdateLocal_MenuItemText, true,
							Keys.Control | Keys.U, new MogMenuItem_Click(MenuItemUpdateLocal_Click), Resources.MogUpdate));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(UpdateLocal_MenuItemText,
                            MOG_PRIVILEGE.All, false, true));
                        break;
                    case Rename_MenuItemText:
						Items.Add(CreateItem(Rename_MenuItemText, true,
							Keys.Control | Keys.N, new MogMenuItem_Click(MenuItemRename_Click), Resources.MogRename));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(Rename_MenuItemText,
                            MOG_PRIVILEGE.RenameLocal, true, true));
                        break;
                    case LibraryExplore_MenuItemText:
						Items.Add(CreateItem(LibraryExplore_MenuItemText, true,
							Keys.Control | Keys.E, new MogMenuItem_Click(MenuItemExploreLibrary_Click), Resources.MogExplore));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(LibraryExplore_MenuItemText,
                            MOG_PRIVILEGE.All, false, false));
                        break;
                    case LibraryEditWith_MenuItemText:
						Items.Add(CreateItem(LibraryEditWith_MenuItemText, true,
							Keys.None, new MogMenuItem_Click(MenuItemLibraryOpenWith_Click)));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(LibraryEditWith_MenuItemText,
                            MOG_PRIVILEGE.All, false, false));
                        break;
                    case Explore_MenuItemText:
						Items.Add(CreateItem(Explore_MenuItemText, true,
							Keys.Control | Keys.E, new MogMenuItem_Click(MenuItemExplore_Click), Resources.MogExplore));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(Explore_MenuItemText,
                            MOG_PRIVILEGE.ExploreAssetDirectory, false, true));
                        break;
                    case DblClick_MenuItemText:
						ToolStripMenuItem DblClickItem = CreateItem(DblClick_MenuItemText, true, Keys.None, null);

						DblClickItem.DropDownItems.Add(CreateItem(View_MenuItemText, true, Keys.None, new MogMenuItem_Click(MenuItemSetDblClick_Click)));
						DblClickItem.DropDownItems.Add(CreateItem(Properties_MenuItemText, true, Keys.None, new MogMenuItem_Click(MenuItemSetDblClick_Click)));

						Items.Add(DblClickItem);
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(ViewSpecial_MenuItemText,
                            MOG_PRIVILEGE.All, false, true));
                        break;
                    case ViewSpecial_MenuItemText:
						ToolStripMenuItem viewSpecialItem = CreateItem(ViewSpecial_MenuItemText, true, Keys.None, null, Resources.MogView);

                        viewSpecialItem.DropDownOpening += new EventHandler(viewSpecialItem_Popup);

						viewSpecialItem.DropDownItems.Add(CreateItem(View_MenuItemText, true, Keys.Control | Keys.V, new MogMenuItem_Click(MenuItemView_Click)));
						viewSpecialItem.DropDownItems.Add(CreateItem(ViewImported_MenuItemText, true, Keys.None, new MogMenuItem_Click(MenuItemView_Click)));
//						viewSpecialItem.DropDownItems.Add(CreateItem(ViewSource_MenuItemText, true, Keys.None, new MogMenuItem_Click(MenuItemView_Click)));
						viewSpecialItem.DropDownItems.Add(CreateItem(ViewProcessed_MenuItemText, true, Keys.None, new MogMenuItem_Click(MenuItemView_Click)));

						Items.Add(viewSpecialItem);
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(ViewSpecial_MenuItemText,
                            MOG_PRIVILEGE.All, false, true));
                        break;
                    case View_MenuItemText:
						Items.Add(CreateItem(View_MenuItemText, true, Keys.Control | Keys.V, new MogMenuItem_Click(MenuItemView_Click), Resources.MogView));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(View_MenuItemText,
                            MOG_PRIVILEGE.All, false, true));
                        break;
                    case Properties_MenuItemText:
						Items.Add(CreateItem(Properties_MenuItemText, true,
							Keys.Control | Keys.Enter, new MogMenuItem_Click(MenuItemProperties_Click), Resources.MogProperties));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(Properties_MenuItemText,
                            MOG_PRIVILEGE.ChangeAssetProperties, false, false));
                        break;
                    case ShowComments_MenuItemText:
						Items.Add(CreateItem(ShowComments_MenuItemText, true,
							Keys.None, new MogMenuItem_Click(MenuItemShowComments_Click), Resources.MogComments));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(ShowComments_MenuItemText,
                            MOG_PRIVILEGE.ChangeAssetProperties, true, true, false, true));
                        break;
                    case SendTo_MenuItemText:
						Items.Add(PopulateCopySendToMenu(SendTo_MenuItemText, new MogMenuItem_Click(MenuItemSendTo_Click), Resources.MogSendTo));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(SendTo_MenuItemText,
                            MOG_PRIVILEGE.All, true, true));
                        break;
                    case CopyTo_MenuItemText:
						Items.Add(PopulateCopySendToMenu(CopyTo_MenuItemText, new MogMenuItem_Click(MenuItemCopyTo_Click), Resources.MogCopyTo));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(CopyTo_MenuItemText,
                            MOG_PRIVILEGE.All, false, true));
                        break;
                    case Delete_MenuItemText:
						Items.Add(CreateItem(Delete_MenuItemText, true,
							Keys.Delete, new MogMenuItem_Click(MenuItemDelete_Click), Resources.MogDelete));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(Delete_MenuItemText,
                            MOG_PRIVILEGE.All, true, true));
                        break;
                    case Remove_MenuItemText:
						Items.Add(CreateItem(Remove_MenuItemText, true,
							Keys.Delete, new MogMenuItem_Click(MenuItemRemove_Click), Resources.MogRemoveFromProject));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(Remove_MenuItemText,
                            MOG_PRIVILEGE.All, false, true));
                        break;
                    case RevertUpdated_MenuItemText:
						Items.Add(CreateItem(RevertUpdated_MenuItemText, true,
							Keys.Delete, new MogMenuItem_Click(MenuItemRemoveUpdated_Click), Resources.MogRemoveUpdate));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(RevertUpdated_MenuItemText,
                            MOG_PRIVILEGE.All, true, true));
                        break;
                    case Restore_MenuItemText:
						Items.Add(CreateItem(Restore_MenuItemText, true,
							Keys.None, new MogMenuItem_Click(MenuItemRestoreTrash_Click), Resources.MogRestore));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(Restore_MenuItemText,
                            MOG_PRIVILEGE.All, true, true));
                        break;
                    case Reject_MenuItemText:
						Items.Add(CreateItem(Reject_MenuItemText, true,
							Keys.None, new MogMenuItem_Click(MenuItemReject_Click)));
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(Reject_MenuItemText,
                            MOG_PRIVILEGE.BlessAssetFromWithinOtherInbox, true, true));
                        break;
                    case Lock_MenuItemText:
						Items.Add(CreateItem(Lock_MenuItemText, true,
							Keys.Control | Keys.L, new MogMenuItem_Click(MenuItemLock_Click)));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(Lock_MenuItemText,
                            MOG_PRIVILEGE.All, false, false));
                        break;
                    case KillLock_MenuItemText:
						Items.Add(CreateItem(KillLock_MenuItemText, true,
							Keys.Control | Keys.Shift | Keys.L, new MogMenuItem_Click(MenuItemKillLock_Click)));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(KillLock_MenuItemText,
                            MOG_PRIVILEGE.KillLock, false, false));
                        break;
                    case UnLock_MenuItemText:
						Items.Add(CreateItem(UnLock_MenuItemText, true,
							Keys.Control | Keys.Shift | Keys.L, new MogMenuItem_Click(MenuItemUnLock_Click)));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(UnLock_MenuItemText,
                            MOG_PRIVILEGE.All, false, true));
                        break;
                    case PackageManagement_MenuItemText:
						Items.Add(CreateItem(PackageManagement_MenuItemText, true,
							Keys.Control | Keys.P, new MogMenuItem_Click(MenuItemPackageManagement_Click), Resources.MogPackageManagement));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(PackageManagement_MenuItemText,
                            MOG_PRIVILEGE.PackageManagement, false, false));
                        break;
                    case RemoveFromProject_MenuItemText:
						Items.Add(CreateItem(RemoveFromProject_MenuItemText, true,
							Keys.Control | Keys.Shift | Keys.R, new MogMenuItem_Click(MenuItemRemoveFromProject_Click), Resources.MogRemoveFromProject));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(RemoveFromProject_MenuItemText,
                            MOG_PRIVILEGE.RemoveAssetFromProject, false, true));
                        break;
                    case MakeCurrent_MenuItemText:
						Items.Add(CreateItem(MakeCurrent_MenuItemText, true,
							Keys.Control | Keys.M, new MogMenuItem_Click(MenuItemMakeCurrent_Click), Resources.MogMakeCurrent));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(MakeCurrent_MenuItemText,
                            MOG_PRIVILEGE.ChangeRevisionOfAsset, false, true));
                        break;
                    case GenerateReport_MenuItemText:
						Items.Add(CreateItem(GenerateReport_MenuItemText, true,
							Keys.Control | Keys.G, new MogMenuItem_Click(MenuItemGenerateReport_Click), Resources.MogGenerateReport));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(GenerateReport_MenuItemText,
                            MOG_PRIVILEGE.All, false, false, false, false, false));
                        break;
                    case ExportBinaries_MenuItemText:
						Items.Add(CreateItem(ExportBinaries_MenuItemText, true,
							Keys.None, new MogMenuItem_Click(MenuItemExportBinaries_Click)));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(ExportBinaries_MenuItemText,
                            MOG_PRIVILEGE.All, false, false));
                        break;
                    case CreatePackage_MenuItemText:
						Items.Add(CreateItem(CreatePackage_MenuItemText, true,
							Keys.None, new MogMenuItem_Click(MenuItemCreatePackage_Click)));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(CreatePackage_MenuItemText,
                            MOG_PRIVILEGE.CreatePackage, false, false));
                        break;
                    case RebuildPackage_MenuItemText:
						Items.Add(CreateItem(RebuildPackage_MenuItemText, true,
							Keys.None, new MogMenuItem_Click(MenuItemRebuildPackage_Click)));
						// Add the button Privileges
						mMenuPrivileges.Add(new guiAssetMenuPrivileges(RebuildPackage_MenuItemText, 
							MOG_PRIVILEGE.RebuildPackage, false, true, false, true));
						break;
                    case ChangeProperties_MenuItemText:
						ToolStripMenuItem ChangeProperties = CreateItem(ChangeProperties_MenuItemText, true,
							Keys.None, new MogMenuItem_Click(MenuItemChangeProperties_Click));

                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(ChangeProperties_MenuItemText,
                            MOG_PRIVILEGE.ChangeAssetProperties, true, false));

						Items.Add(ChangeProperties);

                        // Force the popup method of the context menu to check for property menu's
                        mShowPropertyMenu = true;
                        break;

                    case ChangePropertiesAdd_MenuItemText:
						Items.Add(CreateItem(ChangePropertiesAdd_MenuItemText, true,
							Keys.None, new MogMenuItem_Click(MenuItemChangePropertiesAdd_Click)));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(ChangePropertiesAdd_MenuItemText,
                            MOG_PRIVILEGE.CreatePropertyMenu, true, false));
                        break;

                    case CustomTools_MenuItemText:
						ToolStripMenuItem toolsItem = CreateItem(CustomTools_MenuItemText, true,
							Keys.None, null);

                        // Init the custom tools menu
                        PopulateCustomToolsMenu(toolsItem);

                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(CustomTools_MenuItemText,
                            MOG_PRIVILEGE.All, false, false));

                        // Add this complex menu
						Items.Add(toolsItem);
                        break;
//					case "merge version report":
//						//Items.Add(CreateItem("Merge Version Report", true,		Keys.None, new MogMenuItem_Click(MenuItemRebuildPackage_Click)));
//						ToolStripMenuItem MergeReport = CreateItem("Merge Version Report", true,		Keys.None, null);
//						// Add the button Privileges
//						mMenuPrivileges.Add(new guiAssetMenuPrivileges("Merge Version Report", false, false, true));
//									
//						// Add a menu item for each platform
//						ArrayList platforms = MOG_ControllerProject.GetProject().GetPlatforms();
//						for (int p = 0; p < platforms.Count; p++)
//						{
//							MOG_Platform platform = (MOG_Platform)platforms[p];
//							MergeReport.Items.Add(CreateItem(platform.mPlatformName, true, Keys.None, new MogMenuItem_Click(MenuItemGenerateMergeVersionReport_Click)));
//						}
//
//						// Add this complex menu
//						Items.Add(MergeReport);
					//						break;
					case Advanced_MenuItemText:
						ToolStripMenuItem AdvancedItem = CreateItem(Advanced_MenuItemText, true, Keys.None, null);
						// Add the button Privileges
						mMenuPrivileges.Add(new guiAssetMenuPrivileges(Advanced_MenuItemText, MOG_PRIVILEGE.AccessAdminTools, false, false));
						
						// Delete classification
						AdvancedItem.DropDownItems.Add(CreateItem(ClearUpdatedFromList_MenuItemText, true,
							Keys.None, new MogMenuItem_Click(RemoveUpdatedFromList_Click)));
						// Add the button Privileges
						mMenuPrivileges.Add(new guiAssetMenuPrivileges(ClearUpdatedFromList_MenuItemText,
							MOG_PRIVILEGE.All, false, true));

						// Add this complex menu
						Items.Add(AdvancedItem);
						break;
                    case AdminTools_MenuItemText:
						ToolStripMenuItem AdminToolsItem = CreateItem(AdminTools_MenuItemText, true, Keys.None, null);
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(AdminTools_MenuItemText, MOG_PRIVILEGE.AccessAdminTools, false, false));


						//AdminToolsItem.Items.Add(CreateItem(AdminToolsRepairDB_MenuItemText, true,Keys.None, 
                        //	new MogMenuItem_Click(MenuItemAdminToolRepairGameDataTable_Click)));

                        // Add Classification
						AdminToolsItem.DropDownItems.Add(CreateItem(AddClassification_MenuItemText, true,
							Keys.None, new MogMenuItem_Click(MenuItemAddClassification_Click)));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(AddClassification_MenuItemText,
                            MOG_PRIVILEGE.AddClassification, false, false, true));

                        // Rename classification
						AdminToolsItem.DropDownItems.Add(CreateItem(RenameClassification_MenuItemText, true,
							Keys.None, new MogMenuItem_Click(MenuItemRenameClassification_Click)));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(RenameClassification_MenuItemText,
                            MOG_PRIVILEGE.DeleteClassification, false, false, true));

                        // Delete classification
						AdminToolsItem.DropDownItems.Add(CreateItem(DeleteClassification_MenuItemText, true,
							Keys.None, new MogMenuItem_Click(MenuItemDeleteClassification_Click)));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(DeleteClassification_MenuItemText,
                            MOG_PRIVILEGE.DeleteClassification, false, false, true));

                        // Separator
						AdminToolsItem.DropDownItems.Add("-");

						// Clean Invalid Items
						AdminToolsItem.DropDownItems.Add(CreateItem(AdminToolsCleanInvalidItems_MenuItemText, true, Keys.None,
							new MogMenuItem_Click(MenuItemAdminToolsCleanInvalidItems_Click)));
						// Clean Invalid Items
						AdminToolsItem.DropDownItems.Add(CreateItem(AdminToolsReportUnreferencedRevisions_MenuItemText, true, Keys.None,
							new MogMenuItem_Click(MenuItemAdminToolsArchiveUnreferencedRevisions_Click)));
						// Used to repair revision metadata
						AdminToolsItem.DropDownItems.Add(CreateItem(AdminToolsRepairRevisionMetadata_MenuItemText, true, Keys.None,
							new MogMenuItem_Click(MenuItemAdminToolsRepairRevisionMetadata_Click)));
						// Scan for unposted revisions
						AdminToolsItem.DropDownItems.Add(CreateItem(AdminToolsReportNewerUnpostedRevisions_MenuItemText, true, Keys.None,
							new MogMenuItem_Click(MenuItemAdminToolsReportNewerUnpostedRevisions_Click)));
						// Scan for multiple package assignments
						AdminToolsItem.DropDownItems.Add(CreateItem(AdminToolsReportMultiplePackageAssignments_MenuItemText, true, Keys.None,
							new MogMenuItem_Click(MenuItemAdminToolsReportMultiplePackageAssignments_Click)));
						// Scan for colliding package assignments
						AdminToolsItem.DropDownItems.Add(CreateItem(AdminToolsReportCollidingPackageAssignments_MenuItemText, true, Keys.None,
							new MogMenuItem_Click(MenuItemAdminToolsReportCollidingPackageAssignments_Click)));
						// Scan for invalid package assignments
						AdminToolsItem.DropDownItems.Add(CreateItem(AdminToolsReportInvalidPackageAssignments_MenuItemText, true, Keys.None,
							new MogMenuItem_Click(MenuItemAdminToolsReportInvalidPackageAssignments_Click)));
						// Add the button Privileges
						mMenuPrivileges.Add(new guiAssetMenuPrivileges(AdminToolsVerifyUpdateSqlTables_MenuItemText,
							MOG_PRIVILEGE.AccessAdminTools, false, false, true));
						// Fixup database
						AdminToolsItem.DropDownItems.Add(CreateItem(AdminToolsVerifyUpdateSqlTables_MenuItemText, true, Keys.None,
							new MogMenuItem_Click(MenuItemAdminToolsVerifyUpdateSqlTables_Click)));
//						// Fixup previous Library revisions for new Library optimizations
//						AdminToolsItem.DropDownItems.Add(CreateItem(AdminToolsUpdateProjectFor304XDrop_MenuItemText, true, Keys.None,
//							new MogMenuItem_Click(MenuItemAdminToolsUpdateProjectFor304XDrop_Click)));

                        // Add this complex menu
						Items.Add(AdminToolsItem);
                        break;
                    case "":
                        break;
                    case AddClassification_MenuItemText:
						Items.Add(CreateItem(AddClassification_MenuItemText, false,
							Keys.None, new MogMenuItem_Click(MenuItemAddClassification_Click)));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(AddClassification_MenuItemText,
                            MOG_PRIVILEGE.AddClassification, false, false, true));
                        break;
                    case DeleteClassification_MenuItemText:
						Items.Add(CreateItem(DeleteClassification_MenuItemText, true,
							Keys.None, new MogMenuItem_Click(MenuItemDeleteClassification_Click)));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(DeleteClassification_MenuItemText,
                            MOG_PRIVILEGE.DeleteClassification, false, false, true));
                        break;
                    case "-":
                        // This is a separator
						Items.Add("-");
                        break;
                    default:
						Items.Add(CreateItem(itemName.Trim(), false,
							Keys.None, null));
                        // Add the button Privileges
                        mMenuPrivileges.Add(new guiAssetMenuPrivileges(itemName.Trim(),
                            MOG_PRIVILEGE.None, true, true));
                        break;
					}
				#endregion Switch over all possible Items
			}

			// Create a before open even handle so we can do special checking and 
			//	formating of our menu based on the asset clicked on when the user opens the context menu
			if (mView != null)
			{
				mView.KeyDown += OnViewKeyDown;
			}
			else if (mTree != null)
			{
				mTree.KeyDown += OnViewKeyDown;
			}

			return this;
		}

		/// <summary>
		/// Decide which template (i.e. which MenuItems to display/activate in the ContextMenu)
		/// </summary>
		private string ResolveTemplates(string template)
		{
			string menuString = "";

            if (template.IndexOf("{") != -1)
            {
                string[] items = template.Split(",".ToCharArray());
                foreach (string templateName in items)
                {
                    // Set our menu type
                    mMenuTemplateName = templateName.Trim("{}".ToCharArray());

                    string CommaSpace = ", ";
                    // Initialize the menuString as it will be displayed
                    switch (templateName.ToLower())
                    {
                        #region Initialize menuString to template
                        case "{inbox}":
                            menuString = "," + Process_MenuItemText
                                + CommaSpace + UpdateLocal_MenuItemText
                                + CommaSpace + Bless_MenuItemText
                                + CommaSpace + "-"
                                + CommaSpace + PackageManagement_MenuItemText
//								+ CommaSpace + Repackage_MenuItemText
//								+ CommaSpace + RebuildPackage_MenuItemText
                                + CommaSpace + "-"
                                + CommaSpace + Refresh_MenuItemText
                                + CommaSpace + "-"
                                + CommaSpace + Lock_MenuItemText
                                + CommaSpace + UnLock_MenuItemText
                                + CommaSpace + "-"
                                + CommaSpace + SendTo_MenuItemText
                                + CommaSpace + CopyTo_MenuItemText
                                + CommaSpace + Reject_MenuItemText
                                + CommaSpace + Delete_MenuItemText
                                + CommaSpace + "-"
                                + CommaSpace + ChangePropertiesAdd_MenuItemText
                                + CommaSpace + ChangeProperties_MenuItemText
                                + CommaSpace + "-"
                                + CommaSpace + Rename_MenuItemText
                                //+ CommaSpace + View_MenuItemText
                                + CommaSpace + ViewSpecial_MenuItemText
                                + CommaSpace + "-"
                                //+ CommaSpace + DblClick_MenuItemText
                                + CommaSpace + Explore_MenuItemText
                                + CommaSpace + ShowComments_MenuItemText
								+ CommaSpace + ProjectCopyAssetToClipboard_MenuItemText
                                + CommaSpace + ProjectPasteAssetFromClipboard_MenuItemText
								+ CommaSpace + Properties_MenuItemText;
                            break;

                        case "{mergeversion}":
                            menuString = "," + Lock_MenuItemText
                                + CommaSpace + UnLock_MenuItemText
                                + CommaSpace + "-"
                                + CommaSpace + View_MenuItemText
                                + CommaSpace + "-"
                                + CommaSpace + ShowComments_MenuItemText
                                + CommaSpace + Properties_MenuItemText;
                            break;

                        case "{outbox}":
                            menuString = CommaSpace + "-"
                            + CommaSpace + Refresh_MenuItemText
							+ CommaSpace + ProjectCopyAssetToClipboard_MenuItemText
                            + CommaSpace + Delete_MenuItemText;
                            break;

                        case "{lockmanager}":
                            menuString = "," + UnLock_MenuItemText
                                + CommaSpace + LibraryCheckin_MenuItemText
                                + CommaSpace + LibraryUndoCheckout_MenuItemText
                                + CommaSpace + "-"
                                + CommaSpace + Refresh_MenuItemText
                                + CommaSpace + "-"
                                + CommaSpace + KillLock_MenuItemText;
                            break;

                        case "{trash}":
                            menuString = "," + Restore_MenuItemText
                                + CommaSpace + View_MenuItemText
                                + CommaSpace + "-"
                                + CommaSpace + Refresh_MenuItemText
                                + CommaSpace + "-"
                                + CommaSpace + Delete_MenuItemText;
                            break;

                        case "{propertiespackages}":
                            menuString = "," + CopyTo_MenuItemText
                                + CommaSpace + "-"
                                + CommaSpace + Lock_MenuItemText
                                + CommaSpace + UnLock_MenuItemText
                                + CommaSpace + "-"
                                + CommaSpace + Repackage_MenuItemText
//								+ CommaSpace + RebuildPackage_MenuItemText
                                + CommaSpace + "-"
                                + CommaSpace + Refresh_MenuItemText
                                + CommaSpace + "-"
                                + CommaSpace + ShowComments_MenuItemText
                                + CommaSpace + Properties_MenuItemText;
                            break;
                        case "{updated}":
							menuString = "," + Advanced_MenuItemText
								+ CommaSpace + CopyTo_MenuItemText
                                + CommaSpace + RevertUpdated_MenuItemText
                                + CommaSpace + "-"
                                + CommaSpace + Lock_MenuItemText
                                + CommaSpace + UnLock_MenuItemText
                                + CommaSpace + "-"
                                + CommaSpace + Repackage_MenuItemText
//								+ CommaSpace + RebuildPackage_MenuItemText
                                + CommaSpace + "-"
                                + CommaSpace + Refresh_MenuItemText
                                + CommaSpace + "-"
                                + CommaSpace + View_MenuItemText
                                + CommaSpace + "-"
                                + CommaSpace + Explore_MenuItemText
								+ CommaSpace + ProjectCopyAssetToClipboard_MenuItemText
                                + CommaSpace + Properties_MenuItemText;
                            break;

                        case "{project}":
                            menuString = "," + GenerateReport_MenuItemText
                                + CommaSpace + AdminTools_MenuItemText
#if !MOG_LIBRARY
                                + CommaSpace + "-"
                                + CommaSpace + PackageManagement_MenuItemText
								+ CommaSpace + RebuildPackage_MenuItemText
#endif
								+ CommaSpace + "-"
								+ CommaSpace + Lock_MenuItemText
                                + CommaSpace + UnLock_MenuItemText
								+ CommaSpace + "-"
#if !MOG_LIBRARY
                                + CommaSpace + UpdateLocal_MenuItemText
                                + CommaSpace + CopyTo_MenuItemText
                                + CommaSpace + "-"
#endif
 + CommaSpace + RemoveFromProject_MenuItemText
                                + CommaSpace + MakeCurrent_MenuItemText
								+ CommaSpace + "-"
#if !MOG_LIBRARY
                                + CommaSpace + ChangePropertiesAdd_MenuItemText
                                + CommaSpace + ChangeProperties_MenuItemText
                                + CommaSpace + "-"
#endif
                                + CommaSpace + Refresh_MenuItemText
                                + CommaSpace + View_MenuItemText
								+ CommaSpace + ProjectCopyAssetToClipboard_MenuItemText
								+ CommaSpace + ProjectPasteAssetFromClipboard_MenuItemText
                                + CommaSpace + "-"
                                + CommaSpace + Explore_MenuItemText
                                + CommaSpace + ShowComments_MenuItemText
                                + CommaSpace + Properties_MenuItemText;
                            break;
						case "{synctargettreeview}":
							menuString = "," + GenerateReport_MenuItemText
								+ CommaSpace + "-"
								+ CommaSpace + PackageManagement_MenuItemText
								+ CommaSpace + "-"
								+ CommaSpace + Lock_MenuItemText
								+ CommaSpace + UnLock_MenuItemText
								+ CommaSpace + "-"
								+ CommaSpace + UpdateLocal_MenuItemText
								+ CommaSpace + CopyTo_MenuItemText
								+ CommaSpace + "-"
								+ CommaSpace + RemoveFromProject_MenuItemText
								+ CommaSpace + MakeCurrent_MenuItemText
								+ CommaSpace + "-"
								+ CommaSpace + ChangePropertiesAdd_MenuItemText
								+ CommaSpace + ChangeProperties_MenuItemText
								+ CommaSpace + "-"
								+ CommaSpace + Refresh_MenuItemText
								+ CommaSpace + View_MenuItemText
								+ CommaSpace + "-"
								+ CommaSpace + Explore_MenuItemText
								+ CommaSpace + ShowComments_MenuItemText
								+ CommaSpace + ProjectCopyAssetToClipboard_MenuItemText
								+ CommaSpace + ProjectPasteAssetFromClipboard_MenuItemText
								+ CommaSpace + Properties_MenuItemText;
							break;
                        case "{report}":
                            menuString = "," + Remove_MenuItemText
								+ CommaSpace + "-"
#if !MOG_LIBRARY
                                + CommaSpace + UpdateLocal_MenuItemText
                                + CommaSpace + CopyTo_MenuItemText
                                + CommaSpace + "-"
                                + CommaSpace + PackageManagement_MenuItemText
								+ CommaSpace + RebuildPackage_MenuItemText
                                + CommaSpace + "-"
#endif
                                + CommaSpace + Lock_MenuItemText
                                + CommaSpace + UnLock_MenuItemText
                                + CommaSpace + "-"
                                + CommaSpace + LibraryViewRevisions_MenuItemText
                                + CommaSpace + RemoveFromProject_MenuItemText
                                + CommaSpace + MakeCurrent_MenuItemText
								+ CommaSpace + "-"
#if !MOG_LIBRARY
                                + CommaSpace + ChangePropertiesAdd_MenuItemText
                                + CommaSpace + ChangeProperties_MenuItemText
                                + CommaSpace + "-"
#endif
                                + CommaSpace + View_MenuItemText
                                + CommaSpace + "-"
                                + CommaSpace + Explore_MenuItemText
                                + CommaSpace + ShowComments_MenuItemText
								+ CommaSpace + ProjectCopyAssetToClipboard_MenuItemText								
                                + CommaSpace + Properties_MenuItemText;
                            break;

                        case "{assetarchive}":
#if !MOG_LIBRARY
                            menuString = "," + CopyTo_MenuItemText
                                + CommaSpace + "-"
#else
							menuString = ""
#endif
                                + CommaSpace + MakeCurrent_MenuItemText
                                + CommaSpace + "-"
                                + CommaSpace + Refresh_MenuItemText
                                + CommaSpace + "-"
                                + CommaSpace + Properties_MenuItemText
                                + CommaSpace + ShowComments_MenuItemText
								+ CommaSpace + ProjectCopyAssetToClipboard_MenuItemText
								+ CommaSpace + ProjectPasteAssetFromClipboard_MenuItemText
                                + CommaSpace + View_MenuItemText;
                            break;
                        case "{librarylistview}":
                            menuString = "," 
                                + CommaSpace + Sync_MenuItemText
                                + CommaSpace + "-"
                                + CommaSpace + LibraryAdd_MenuItemText
                                + CommaSpace + LibraryCheckout_MenuItemText
                                + CommaSpace + LibraryCheckin_MenuItemText
                                + CommaSpace + LibraryUndoCheckout_MenuItemText
                                + CommaSpace + "-"
// JohnRen - Removed because this just confused artists
//								+ CommaSpace + Lock_MenuItemText
//								+ CommaSpace + UnLock_MenuItemText
//								+ CommaSpace + "-"
                                + CommaSpace + LibraryExplore_MenuItemText
                                + CommaSpace + LibraryEdit_MenuItemText
                                + CommaSpace + LibraryEditWith_MenuItemText
                                + CommaSpace + LibraryViewRevisions_MenuItemText
								+ CommaSpace + "-"
#if !MOG_LIBRARY
                                + CommaSpace + LibraryExportToInbox_MenuItemText
                                + CommaSpace + "-"
#endif
                                + CommaSpace + LibraryDeleteFile_MenuItemText
                                + CommaSpace + RemoveFromProject_MenuItemText
                                + CommaSpace + "-"
								+ CommaSpace + ProjectCopyAssetToClipboard_MenuItemText
								+ CommaSpace + ProjectPasteAssetFromClipboard_MenuItemText
								+ CommaSpace + "-"
								+ CommaSpace + GenerateReport_MenuItemText
								+ CommaSpace + "-"
                                + CommaSpace + Refresh_MenuItemText;
                            break;
                        case "{librarytreeview}":
                            menuString = "," 
                                + CommaSpace + Sync_MenuItemText
                                + CommaSpace + Clean_MenuItemText
								+ CommaSpace + "-"
								+ CommaSpace + LibraryAdd_MenuItemText
                                + CommaSpace + LibraryCheckout_MenuItemText
                                + CommaSpace + LibraryCheckin_MenuItemText
                                + CommaSpace + LibraryUndoCheckout_MenuItemText
// JohnRen - Removed because this just confused artists
//                              + CommaSpace + "-"
//								+ CommaSpace + Lock_MenuItemText
//								+ CommaSpace + UnLock_MenuItemText
                                + CommaSpace + "-"
                                + CommaSpace + LibraryExploreFolder_MenuItemText
                                + CommaSpace + LibraryCreateFolder_MenuItemText
                                + CommaSpace + LibraryEditFolder_MenuItemText
                                + CommaSpace + LibraryDeleteFolder_MenuItemText
                                + CommaSpace + "-"
#if !MOG_LIBRARY
                                + CommaSpace + LibraryExportToInbox_MenuItemText
                                + CommaSpace + "-"
#endif
								+ CommaSpace + ProjectCopyAssetToClipboard_MenuItemText
								+ CommaSpace + ProjectPasteAssetFromClipboard_MenuItemText								
								+ CommaSpace + "-"
								+ CommaSpace + GenerateReport_MenuItemText
								+ CommaSpace + "-"
                                + CommaSpace + Refresh_MenuItemText;
                            break;
                        case "{commands}":
                            menuString = CommaSpace + "-"
                                + CommaSpace + Refresh_MenuItemText
                                + CommaSpace + "-"
                                + CommaSpace + RestartCommand_MenuItemText
                                + CommaSpace + ViewCommand_MenuItemText
                                + CommaSpace + "-"
                                + CommaSpace + DeleteCommand_MenuItemText;
                            break;
                        case "{packagecommands}":
                            menuString = CommaSpace + DeletePackageCommand_MenuItemText
                                + CommaSpace + RestartPackageCommand_MenuItemText
                                + CommaSpace + "-"
                                //+ CommaSpace + ProjectGoToAssetCommand_MenuItemText
                                + CommaSpace + Refresh_MenuItemText;
                            break;
                        case "{postcommands}":
                            menuString = CommaSpace + DeletePostCommand_MenuItemText
                                + CommaSpace + RestartPostCommand_MenuItemText
                                + CommaSpace + "-"
                                //+ CommaSpace + ProjectGoToAssetCommand_MenuItemText
                                + CommaSpace + Refresh_MenuItemText;
                            break;
                        case "{lateresolvercommands}":
                            menuString = CommaSpace + DeleteLateResolverCommand_MenuItemText
                                + CommaSpace + RestartLateResolverCommand_MenuItemText
                                + CommaSpace + "-"
                                //+ CommaSpace + ProjectGoToAssetCommand_MenuItemText
                                + CommaSpace + Refresh_MenuItemText;
                            break;
                        case "{connections}":
                            menuString = "," + KillConnection_MenuItemText
                                + CommaSpace + RemoteDesktop_MenuItemText
                                + CommaSpace + "-"
                                + CommaSpace + Refresh_MenuItemText
                                + CommaSpace + "-"
                                + CommaSpace + StartSlave_MenuItemText
                                + CommaSpace + RetaskCommand_MenuItemText;
                            break;
                        case ImportMenu_Text:
                            menuString = "," + AddClassification_MenuItemText
                                + CommaSpace + Properties_MenuItemText;
                            break;
                        case "{propertymenuwizard}":
                            menuString = "," + ChangeProperties_MenuItemText;
                            break;
                        default:
                            menuString = "," + templateName;
                            break;

                        #endregion Initialize menuString
                    }
                }
            }

			return menuString;
		}

		private void RefreshOwningMenu(ToolStripMenuItem item)
		{
			if (item != null)
			{
				ToolStrip menu = item.Owner;
				Prevalidate(menu.Parent);
			}
		}
		
		private void OnViewKeyDown(object sender, KeyEventArgs e)
		{
			Prevalidate(sender as Control);
		}

		private void Prevalidate(Control parent)
		{
			if (parent != null)
			{
				ArrayList selectedItems = ControlGetSelectedItemTags();
				MOG_Filename filename = null;

				foreach (ToolStripItem item in Items)
				{
					item.Visible = true;
				}

				ValidateMenuItemsForPopup(Items, selectedItems, filename);

				// Do algorithm for Property Sub MenuItem Popup
				filename = GeneratePropertySubMenuForPopup(selectedItems, filename);
			}
		}

		private void ValidateMenuItemsForPopup(ToolStripItemCollection menuItemCollection, ArrayList selectedItems, MOG_Filename filename)
		{
			List<ToolStripItem> visibleItems = new List<ToolStripItem>();

			// If we still have no valid MOG_Filename `filename`, and we have items to go through,
			//	we should initialize a MOG_Filename
			if( filename == null && selectedItems.Count > 0 )
			{
				// Get the tag (HACK - we only get the last item in the list when we really should do better!)
				Mog_BaseTag tag = selectedItems[ selectedItems.Count-1 ] as Mog_BaseTag;
				if (tag != null)
				{
					// Create a Filename from the tag
					MOG_Filename testFilename = new MOG_Filename(tag.FullFilename);
					// Is this a valid asset Filename?
					if (testFilename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset ||
						testFilename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Link)
					{
						// Indicate this is an accaptable Filename
						filename = testFilename;
					}
					// Make sure this was is a valid classification?
					else if (MOG_Filename.IsClassificationValidForProject(tag.FullFilename, MOG_ControllerProject.GetProjectName()))
					{
						// Indicate this is an accaptable Filename
						filename = testFilename;
					}
					else
					{
						// At this point, we need to make a few more agressive assumptions
						// Check if this owner is a TreeNode?
						if (tag.Owner is TreeNode)
						{
							// Sometimes we need to ask the owner TreeNode for its FullPath (i.e. LibraryTreeView)
							// Check if this TreeNode is a valid classification?
							TreeNode node = tag.Owner as TreeNode;
							if (MOG_Filename.IsClassificationValidForProject(node.FullPath, MOG_ControllerProject.GetProjectName()))
							{
								filename = new MOG_Filename(node.FullPath);
							}
						}
						else
						{
							filename = testFilename;
						}
					}
				}
			}

			MOG_Properties properties = null;
			if( filename != null )
			{
				// Check if this is an asset
				if (filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
				{
					properties = new MOG_Properties(filename);
				}
				// Check if this is a classification?
				else if (MOG_Filename.IsClassificationValidForProject(filename.GetOriginalFilename(), MOG_ControllerProject.GetProjectName()))
				{
					properties = new MOG_Properties(filename.GetOriginalFilename());
				}
			}
			// Foreach MenuItem in menu's MenuItems, only enable items we want
			foreach (ToolStripItem item in menuItemCollection)
			{
				// If item is a separator, make sure it is visible (until we get to the next for loop)
				if( item is ToolStripSeparator )
				{
					item.Visible = true;				
					visibleItems.Add( item );
				}
				else if (item is ToolStripMenuItem)
				{
					ToolStripMenuItem menuItem = item as ToolStripMenuItem;

					// Foreach valid guiAssetMenuPrivileges object we have in the ArrayList of MenuPrivileges
					foreach (guiAssetMenuPrivileges button in mMenuPrivileges)
					{
						// If this is the item we are looking for in MenuItems...
						if (item.Text == button.Name || item.Text.StartsWith( button.Name ) )
						{
							ValidateButtonPrivileges(button, menuItem, visibleItems, filename, properties, selectedItems);

							// Change our 'View' MenuItem to display HOW we will be viewing what we right-clicked on
							if (item.Text.StartsWith(View_MenuItemText + "<") || item.Text == View_MenuItemText)
							{
								ChangeViewMenuItem(menuItem);
							}

							// Change our DoubleClick MenuItem depending on HOW we want to display it
							if (item.Text.StartsWith(DblClick_MenuItemText))
							{
								ChangeDblClickMenuItem(menuItem);
							}
							
							// Change our 'Rip' MenuItem depending on IF it is a ripped object
							if (item.Text.StartsWith(Process_MenuItemText))
							{
								ChangeRipMenuItem(menuItem, properties);
							}

							// Change our 'Package Management' MenuItem depending on IF it is a ripped object
							if (item.Text.StartsWith(PackageManagement_MenuItemText))
							{
								// Never allow this option for library assets
								if (filename != null &&
									!filename.IsLibrary())
								{
									// Check if this has already been disabled?
									if (item.Enabled)
									{
										ChangePackageMenuItem(menuItem, properties);
									}
								}
								else
								{
									item.Enabled = false;
								}
							}

							// Update our sendTo menu's so they reflect any additions/removals to the users
							if (item.Text == SendTo_MenuItemText)
							{
								// Never allow this option for library assets
								if (filename != null &&
									!filename.IsLibrary())
								{
									UpdateCopySendToMenu(menuItem, new MogMenuItem_Click(MenuItemSendTo_Click));
								}
								else
								{
									item.Enabled = false;
								}
							}
							// Update our copyTo menu's so they reflect any additions/removals to the users
							else if (item.Text == CopyTo_MenuItemText)
							{
								// Never allow this option for library assets
								if (filename != null &&
									!filename.IsLibrary())
								{
									UpdateCopySendToMenu(menuItem, new MogMenuItem_Click(MenuItemCopyTo_Click));
								}
								else
								{
									item.Enabled = false;
								}
							}
							else if (item.Text == UpdateLocal_MenuItemText)
							{
								// Never allow this option for library assets
								if (filename != null &&
									!filename.IsLibrary())
								{
									// All is well...
								}
								else
								{
									item.Enabled = false;
								}
							}
							else if (item.Text == ChangePropertiesAdd_MenuItemText)
							{
								// Never allow this option for library assets
								if (filename != null &&
									!filename.IsLibrary())
								{
									// All is well...
								}
								else
								{
									item.Enabled = false;
								}
								// Check if this item is disabled?
								if (!item.Enabled)
								{
									// Hide this from the user when it is in a disabled state.
									item.Visible = false;
								}
							}
						}
					}

					//Decide which library options will be enabled
					ValidateLibraryOptions(menuItem, filename);

					// If we have our Admin Tools MenuItem selected AND it is visible and enabled (indicating user has rights)...
					if( item.Text == AdminTools_MenuItemText && item.Visible && item.Enabled)
					{
						ValidateMenuItemsForPopup(menuItem.DropDownItems, selectedItems, filename);
					}

					ExcludeForNonArchivalTreeViews(menuItem);
				}
			}

            ProcessVisibleItems( visibleItems );			
		}

		private void ExcludeForNonArchivalTreeViews(ToolStripMenuItem item)
		{
			if(item.Text == MakeCurrent_MenuItemText)
			{
				// If our source is a BaseTreeView, but NOT an ArchivalTreeView...
				if( SourceControl is MogControl_BaseTreeView && !(SourceControl is MogControl_ArchivalTreeView))
				{
					item.Visible = false;
				}
			}
		}

		private void ProcessVisibleItems( List<ToolStripItem> visibleItems )
		{
			// Initialized to hold the last non-separator MenuItem (see next for-loop)
			ToolStripItem previousStandardItem = null;

			// Go through visible items and make sure we don't have any adjacent (extra) separators
			for( int i = 0; i < visibleItems.Count; ++i)
			{
				// Initialize nextItemIndex to i or i+1, whichever is less than visibleItems.Count
				int nextItemIndex = ( i+1 >= visibleItems.Count ) ? i : i+1;
				// Initialize previousItemIndex to i or i-1, whichever is greater than 0
				int previousItemIndex = ( i-1 < 0 ) ? i : i-1;

				// Initialize our items
				ToolStripItem currentItem = visibleItems[i];
				ToolStripItem previousItem = visibleItems[previousItemIndex];
				ToolStripItem nextItem = visibleItems[nextItemIndex];
						
				// If we have a separator, AND 
				//	( previousSeparator was visible OR nextSeparator exists OR we do not have 
				//    an initial item other than a separator )...
				if( currentItem.Text == "-" && ( previousItem.Visible && previousItem.Text == "-" 
					|| nextItem.Text == "-"
					|| previousStandardItem == null ) )
				{
					// Make the current separator non-visible
					currentItem.Visible = false;
				}
					// Else, if we have a non-separator, document it with previousStandardItem pointer
				else if( currentItem.Text != "-" )
				{
					previousStandardItem = currentItem;
				}
			}
		}

		private void ValidateButtonPrivileges(guiAssetMenuPrivileges button, ToolStripMenuItem item, 
			List<ToolStripItem> visibleItems, MOG_Filename filename, MOG_Properties assetProperties, ArrayList selectedItems )
		{
			// If we have a valid filename...
			if (filename != null  || button.AssetExclusive == false)
			{
				bool enabled = true;

				// Check if this command requires a valid asset
				// Check if this command requires a valid asset
				if (filename != null)
				{
					if (button.AssetExclusive)
					{
						enabled &= filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset 
							|| filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Group
							|| filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Link;
					}
						// If we are classification exclusive, validate on that...
					else if (button.ClassificationExclusive)
					{
						enabled &= !(filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset 
							|| filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Group
							|| filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Link);

						// If we are a RepositoryTreeView, make sure we are not Gamedata_View RootTreeType
						enabled &= !(SourceControl is MogControl_SyncTargetTreeView);
					}
							
					// Check if this command requires you to be the local user
					if (button.UserExclusive && !filename.IsLocal())
					{
						enabled &= MOG_ControllerProject.GetUser().GetUserName().ToLower() == filename.GetUserName().ToLower() 
							|| MOG_ControllerProject.GetPrivileges().GetUserPrivilege( MOG_ControllerProject.GetUser().GetUserName(), MOG_PRIVILEGE.PerformAllInboxOpps );
					}

                    // Check if this is disabled for light version
                    if (button.LightVersionDisabled)
                    {
                        enabled &= MogUtil_VersionInfo.SetLightVersionControl(item); 
                    }
										
					enabled &= ProcessOtherExclusions( item, button, filename, enabled, assetProperties );
				}

				// Check if we require a selected item
				if (button.ItemRequired)
				{
					enabled &= selectedItems.Count > 0;
				}

				// Check if this user not granted this privilege to execute this command
				bool grantedPrivilege = MOG_ControllerProject.GetPrivileges().GetUserPrivilege(MOG_ControllerProject.GetUser().GetUserName(), button.MogPrivilege);
				enabled &= grantedPrivilege;

				// Keep our AccessAdminTools from being visible to those without privilege to see it...
				if( button.MogPrivilege == MOG_PRIVILEGE.AccessAdminTools && !grantedPrivilege )
				{
					item.Visible = false;
				}


				item.Enabled = enabled;
			}
			else
			{
				item.Enabled = false;
			}

			// If item is visible, add it to the visibleItems list
			if( item.Visible )
			{
				visibleItems.Add( item );
			}
		}

		private void ValidateLibraryOptions(ToolStripMenuItem item, MOG_Filename filename)
		{
			// Make sure we have a valid filename here or else we will crash
			if (filename != null)
			{
				if (item.Text == Lock_MenuItemText)
				{
					string lockName = "";

					// Check if this is an asset?
					if (filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
					{
						// Use the full name of this asset
						lockName = filename.GetAssetFullName();
					}
					// Check if this is a classification?
					else if (MOG_Filename.IsClassificationValidForProject(filename.GetOriginalFilename(), MOG_ControllerProject.GetProjectName()))
					{
						// Use whatever was specified
						lockName = filename.GetOriginalFilename();
					}

					// Check if we obtained a lockName?
					if (lockName.Length > 0)
					{
						// Check to see if this is already locked
						if (MOG_ControllerProject.IsLocked(lockName))
						{
							// yes, it is already locked
							item.Enabled = false;
						}
						else
						{
							// no it isn't locked
							item.Enabled = true;
						}
					}
					else
					{
						// Hide this item
						item.Visible = false;
					}
				}
				if (item.Text == UnLock_MenuItemText)
				{
					string lockName = "";

					// Check if this is an asset?
					if (filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
					{
						// Use the full name of this asset
						lockName = filename.GetAssetFullName();
					}
					// Check if this is a classification?
					else if (MOG_Filename.IsClassificationValidForProject(filename.GetOriginalFilename(), MOG_ControllerProject.GetProjectName()))
					{
						// Use whatever was specified
						lockName = filename.GetOriginalFilename();
					}

					// Check if we obtained a lockName?
					if (lockName.Length > 0)
					{
						// Check to see if the file is already locked
						if (MOG_ControllerProject.IsLocked(lockName))
						{
							//yes, it is locked, but is it locked by me?
							if (MOG_ControllerProject.IsLockedByMe(lockName))
							{
								item.Enabled = true;
							}
							else
							{
								item.Enabled = false;
							}
						}
						else
						{
							// no it isn't locked
							item.Enabled = false;
						}
					}
					else
					{
						// Hide this item
						item.Visible = false;
					}
				}
				if (item.Text == Sync_MenuItemText)
				{
					// We only want to copy assets which are not locked by us because we will already have a locally modified version
					if (filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
					{
						if (MOG_ControllerProject.IsLockedByMe(filename.GetAssetFullName()))
						{
							item.Enabled = false;
						}
						else
						{
							item.Enabled = true;
						}
					}
					else if (MOG_ControllerProject.IsValidClassification(filename.GetOriginalFilename()))
					{
						item.Enabled = true;
					}
					else
					{
						//This is not a MOG asset
						item.Visible = false;
					}
				}
				if (item.Text == Clean_MenuItemText)
				{
					if (MOG_ControllerProject.IsValidClassification(filename.GetOriginalFilename()))
					{
						item.Enabled = true;
					}
					else
					{
						item.Enabled = false;
					}
				}
				if (item.Text == LibraryViewRevisions_MenuItemText)
				{
					if (filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
					{
						item.Enabled = true;
					}
					else
					{
						//This is not a MOG asset
						item.Visible = false;
					}
				}
				if (item.Text == LibraryExportToInbox_MenuItemText)
				{
					if (filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
					{
						item.Enabled = true;
					}
					else
					{
						//This is not a MOG asset
						item.Visible = false;
					}
				}
				else if (item.Text == LibraryCheckout_MenuItemText)
				{
					// Check if this is a valid asset or classification?
					if (filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
					{
						// Check to see if the file is locked
						if (!MOG_ControllerProject.IsLocked(filename.GetAssetFullName()))
						{
							//no it isn't locked
							item.Enabled = true;
						}
						else
						{
							//yes, it is already locked
							item.Enabled = false;
						}
					}
					else if (MOG_ControllerProject.IsValidClassification(filename.GetOriginalFilename()))
					{
						// Always enable this on a classification
						item.Enabled = true;
					}
					else
					{
						// Hide this item
						item.Visible = false;
					}
				}
				else if (item.Text == LibraryCheckin_MenuItemText ||
							item.Text == LibraryUndoCheckout_MenuItemText)
				{
					// Check if this is a valid asset or classification?
					if (filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
					{
						// Make sure this is a valid Library asset?
						if (filename.IsLibrary())
						{
							//Check to see if the file is available to lock
							if (filename == null || !MOG_ControllerProject.IsLocked(filename.GetAssetFullName()))
							{
								//It is not locked, so that means we don't have it checked out and we can't check it back in
								item.Enabled = false;
							}
							else
							{
								//yes, it is locked, but is it locked by me?
								if (MOG_ControllerProject.IsLockedByMe(filename.GetAssetFullName()))
								{
									item.Enabled = true;
								}
								else
								{
									item.Enabled = false;
								}
							}
						}
						else
						{
							item.Enabled = false;
						}
					}
					else if (MOG_ControllerProject.IsValidClassification(filename.GetOriginalFilename()))
					{
						// Always enable this on a classification
						item.Enabled = true;
					}
					else
					{
						// Hide this item
						item.Visible = false;
					}
				}
				else if (item.Text == LibraryAdd_MenuItemText)
				{
					// Check if this is already a valid asset name?
					if (filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
					{
						// Hide this item
						item.Visible = false;
					}
					else if (MOG_ControllerProject.IsValidClassification(filename.GetOriginalFilename()))
					{
						// Always enable this for classifications
						item.Enabled = true;
					}
					else
					{
						// Enable this item
						item.Enabled = true;
					}
				}
				else if (item.Text == LibraryDeleteFile_MenuItemText)
				{
					// Check if this is already a valid asset name?
					if (filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset ||
						MOG_ControllerProject.IsValidClassification(filename.GetOriginalFilename()))
					{
						// Hide this item
						item.Visible = false;
					}
					else
					{
						// Always enable this for things we don't know about
						item.Enabled = true;
					}
				}
				else if (item.Text == RemoveFromProject_MenuItemText)
				{
					// Check if this is already a valid asset name?
					if (filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
					{
						// Enable this item
						item.Enabled = true;
					}
					// Else, always hide this option
					else
					{
						item.Visible = false;
					}
				}
				// Populate the library 'Open With' context menu
				else if (item.Text == LibraryEditWith_MenuItemText)
				{
					// Check if this is an asset?
					if (filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
					{
						//Check to see if the file is available to lock
						if (!MOG_ControllerProject.IsLocked(filename.GetAssetFullName()))
						{
							//There is no lock, so we can enable this
							item.Enabled = true;
						}
						else
						{
							//It is already locked, but that is okay if it is locked by me
							if (MOG_ControllerProject.IsLockedByMe(filename.GetAssetFullName()))
							{
								item.Enabled = true;
							}
							else
							{
								item.Enabled = false;
							}
						}
					}

					// Clear our sub mennu if one exists
					item.DropDownItems.Clear();

					// Get a list of the appclications that can be used for editing
					// HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts
					RegistryKey HKCU = Registry.CurrentUser;
					RegistryKey rkRun;

					// We need to look up available editors by extension
					string extension = "";
					if (filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
					{
						extension = "." + DosUtils.PathGetExtension(filename.GetAssetFullName());
					}
					else
					{
						extension = "." + filename.GetExtension();
					}

					// Open the regestry key for reading
					rkRun = HKCU.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\" + extension + @"\OpenWithList");
					if (rkRun != null)
					{
						// Walk through each editor listed in this key
						foreach (string app in rkRun.GetValueNames())
						{
							// As long as we are not a MRU cache key, add the app
							if (app != "MRUList")
							{
								// The the exe name
								string applicationName = rkRun.GetValue(app) as string;
								string applicationFriendlyName = "";

								string command = "";
								string args = "";

								// Resolve the full path to the specified exe
								RegistryGetOpenEditApplication(applicationName, "", ref command, ref args);

								// Now we need to put a friendly name with the exe we have
								//HKEY_CURRENT_USER\Software\Microsoft\Windows\ShellNoRoam\MUICache
								RegistryKey rkRun2;

								// Open the MUIcach key to see if our exe is listed there
								rkRun2 = HKCU.OpenSubKey(@"Software\Microsoft\Windows\ShellNoRoam\MUICache");
								if (rkRun2 != null)
								{
									// If it is, get the friendly name found there
									applicationFriendlyName = rkRun2.GetValue(command) as string;

									// If the friendly name cannot be found, lets skip it because it is probably a dll or something like that
									if (applicationFriendlyName == null)
									{
										continue;
									}
								}
								else
								{
									// Worst case, just use the exe name
									applicationFriendlyName = applicationName;
								}

								// Create application shortcut menu item
								ToolStripMenuItem appItem = new ToolStripMenuItem(applicationFriendlyName, null, MenuItemLibraryOpenWithApp_Click);

								// Store the un altered app name in the item tag for later lookup within the click event
								appItem.Tag = applicationName;

								// Add a submenu of these apps
								item.DropDownItems.Add(appItem);
							}
						}

						// When we are all done, add a separator
						item.DropDownItems.Add("-");

						// And add a 'choose program' item incase the user wants to specify a new one
						ToolStripMenuItem Choose = new ToolStripMenuItem("Choose Program...", null, MenuItemLibraryOpenWith_Click);
						item.DropDownItems.Add(Choose);
					}
				}
				else if (item.Text == LibraryEdit_VisibleMenuItemText)
				{
					// Check if this is an asset?
					if (filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
					{
						if (!MOG_ControllerProject.IsLocked(filename.GetAssetFullName()))
						{
							//There is no lock, so we can enable this
							item.Enabled = true;
						}
						else
						{
							//It is already locked, but that is okay if it is locked by me
							if (MOG_ControllerProject.IsLockedByMe(filename.GetAssetFullName()))
							{
								item.Enabled = true;
							}
							else
							{
								item.Enabled = false;
							}
						}
					}
				}
			}
		}

		private void ChangeDblClickMenuItem(ToolStripMenuItem item)
		{
			string viewTarget = guiUserPrefs.LoadPref("ClientPrefs", "ContextMenu", "DoubleClick");
			item.Text = DblClick_MenuItemText + viewTarget + ">";
		}

		private void ChangeRipMenuItem(ToolStripMenuItem item, MOG_Properties properties)
		{			
			if (properties != null && properties.NativeDataType == true)
			{
				item.Text = Process_MenuItemText + "(Setup Wizard)";
			}
			else
			{
				item.Text = Process_MenuItemText;
			}
		}

		private void ChangePackageMenuItem(ToolStripMenuItem item, MOG_Properties properties)
		{
			if (properties != null)
			{
				// Only allow wizards on packaged assets that are in the inbox
				if (!properties.IsPackagedAsset && string.Compare(mMenuTemplateName, "Inbox", true) == 0)
				{
					item.Text = PackageManagement_MenuItemText + "(Setup Wizard)";
				}
				else
				{
					item.Text = PackageManagement_MenuItemText;
				}

				//Only allow package management on packaged assets
				if (properties.IsPackagedAsset)
				{
					item.Enabled = true;
				}
			}
		}


		/// <summary>
		/// Change Text property for View MenuItem's MenuItem
		/// </summary>
		/// <param name="item"></param>
		private void ChangeViewMenuItem(ToolStripMenuItem item)
		{
			string viewTarget = guiUserPrefs.LoadPref("ClientPrefs", "ContextMenu", "ViewTarget");
				
			// Set default if none found in ini
			if (viewTarget.Length == 0)
			{
				viewTarget = ViewProcessed_MenuItemText;
			}

			item.Text = View_MenuItemText + "<" +viewTarget + ">";
		}

		private MOG_Filename GeneratePropertySubMenuForPopup(ArrayList selectedItems, MOG_Filename filename)
		{
			ArrayList assetDirectories = new ArrayList();
			string GlobalPopertiesMenu = "";

			#region Build Change properties (Property Menu) sub menu
			if (mShowPropertyMenu)
			{
				MOG_Properties properties = null;

				if (selectedItems.Count > 1)
				{
					foreach (Mog_BaseTag tag in selectedItems)
					{
						if (tag.Execute)
						{		
							filename = new MOG_Filename(tag.FullFilename);
							assetDirectories.Add(filename);

							try
							{
								// Have we already set the menu variable?
								if (GlobalPopertiesMenu.Length == 0)
								{
									properties = new MOG_Properties(filename);

									// Does this asset have a propertyMenu?
									if (properties.PropertyMenu.Length > 0)
									{					
										// Then set our global menu variable
										GlobalPopertiesMenu = properties.PropertyMenu;
									}
									else
									{
										// No menu, set the global menu variable to not allow propertyMenu's now
										GlobalPopertiesMenu = "*";
										break;
									}
								}
								else
								{
									// Oh, there is already a global menu variable set
									properties = new MOG_Properties(filename);

									// Does it equal this assets menu property?
									if (GlobalPopertiesMenu != properties.PropertyMenu)
									{								
										GlobalPopertiesMenu = "*";
										break;																
									}							
								}
							}
							catch(Exception ex)
							{
								ex.ToString();
							}
						}						
					}				

					// Create the Change Properties tools menu for these selected assets by compatible class
					PopulateChangePropertiesToolsMenu(properties, "", GlobalPopertiesMenu, new MogMenuItem_Click(MenuItemChangeProperties_Click));
				}
				else if (selectedItems.Count == 1)
				{
					foreach (Mog_BaseTag tag in selectedItems)
					{
						if (tag.Execute)
						{
							filename = new MOG_Filename(tag.FullFilename);

							properties = new MOG_Properties(filename);

							// Create the Change Properties tools menu for this asset
							PopulateChangePropertiesToolsMenu(properties, tag.FullFilename, "", new MogMenuItem_Click(MenuItemChangeProperties_Click));
						}
						else if (tag.Owner is TreeNode && ((TreeNode)tag.Owner).TreeView != null)
						{
							// This will handle the classification nodes by resolving the full classification name from the tree itsself
							string fullFilename = ((TreeNode)tag.Owner).FullPath;
							properties = new MOG_Properties(fullFilename);

							// Create the Change Properties tools menu for this asset
							PopulateChangePropertiesToolsMenu(properties, fullFilename, "", new MogMenuItem_Click(MenuItemChangeProperties_Click));
						}
					}
				}
				else
				{
					PopulateChangePropertiesToolsMenu(properties, "", "", new MogMenuItem_Click(MenuItemChangeProperties_Click));
				}
			}

			#endregion
			
			return filename;
		}

		/// <summary>
		/// Handle the exclusion of very, very specific cases
		/// </summary>
		private bool ProcessOtherExclusions(ToolStripMenuItem item, guiAssetMenuPrivileges button, MOG_Filename filename, bool enabled, MOG_Properties assetProperties)
		{
			// If we are a SyncTarget Tree and we have no Parent for our SelectedNode in our FullPath, disable GenerateReport
			if( item.Text == GenerateReport_MenuItemText &&
				SourceControl is MogControl_SyncTargetTreeView )
			{
                MogControl_SyncTargetTreeView view = SourceControl as MogControl_SyncTargetTreeView;
				if( view.SelectedNode.Parent == null )
				{
					enabled &= false;
				}
			}

			// Exclusions that depend onf assetProperties object
			if( assetProperties != null )
			{
				// If we are the Bless MenuItem, AND we are in the Inbox, 
				// If we are unprocessed...
//				enabled &= !(item.Text == Bless_MenuItemText && filename.IsWithinInboxes() && (assetProperties.IsUnprocessed || assetProperties.UnBlessable));
				enabled &= !(item.Text == Bless_MenuItemText && filename.IsWithinInboxes() && (assetProperties.IsUnprocessed));

				// If we have the properties menuItem and we are a SyncTargetTreeView AND we are not an Asset...
				enabled &= !(item.Text == Properties_MenuItemText && filename.GetFilenameType() != MOG_FILENAME_TYPE.MOG_FILENAME_Asset
					&& SourceControl is MogControl_SyncTargetTreeView);

				// If we are unprocessed...
				if (assetProperties.IsUnprocessed)
				{
					// Disable SendTo, CopyTo, UpdateLocal, and Reject
					enabled &= !(item.Text == SendTo_MenuItemText) && !(item.Text == CopyTo_MenuItemText)
						&& !(item.Text == UpdateLocal_MenuItemText) && !(item.Text == Reject_MenuItemText);
				}

				// If we are NOT a packaged Asset, don't display the options for a packaged Asset
				if( item.Text == RebuildPackage_MenuItemText )
				{
					enabled &= assetProperties.IsPackage || assetProperties.IsPackagedAsset;
				}
				if( item.Text == Repackage_MenuItemText )
				{
					enabled &= assetProperties.IsPackagedAsset;
				}

				// If we are Reject MenuItem, disable if we are the creator
				enabled &= !( item.Text == Reject_MenuItemText && assetProperties.Creator.ToLower() == MOG_ControllerProject.GetActiveUser().GetUserName().ToLower() );
			}

			return enabled;
		}

		private void viewSpecialItem_Popup(object sender, EventArgs e)
		{
			string viewTarget = guiUserPrefs.LoadPref("ClientPrefs", "ContextMenu", "ViewTarget");
			
			// Set default if none found in ini
			if (viewTarget.Length == 0)
			{
				viewTarget = ViewProcessed_MenuItemText;
			}

			ToolStripMenuItem viewMenu = (ToolStripMenuItem)sender;
			foreach (ToolStripMenuItem item in viewMenu.DropDownItems)
			{
				if (string.Compare(item.Text, viewTarget, true) == 0)
				{
					item.Checked = true;
				}
				else
				{
					item.Checked = false;
				}

				// Special menu additions to happen on popup
				if (item.Text.StartsWith(View_MenuItemText + "<") || item.Text == View_MenuItemText)
				{
					item.Text = View_MenuItemText + "<" + viewTarget + ">";
				}
			}
		}

		private ToolStripMenuItem CreateItem(string name, bool enabled, Keys shortcut, MogMenuItem_Click method)
		{
			return CreateItem(name, enabled, shortcut, method, null);
		}

		private ToolStripMenuItem CreateItem(string name, bool enabled, Keys shortcut, MogMenuItem_Click method, Image image )
		{
			ToolStripMenuItem Item = new ToolStripMenuItem(name);
			if (method != null)
			{
				Item.Click += new System.EventHandler(method);
			}
			if (shortcut != Keys.None)
			{
				Item.ShowShortcutKeys = true;
				Item.ShortcutKeys = shortcut;
			}
			if (image != null)
			{
				Item.Image = image;
			}
			Item.Enabled = enabled;
			return Item;
		}

		private ToolStripMenuItem PopulateCopySendToMenu(string title, MogMenuItem_Click method)
		{
			return PopulateCopySendToMenu(title, method, null);
		}

		private ToolStripMenuItem PopulateCopySendToMenu(string title, MogMenuItem_Click method, Image image)
		{
			ToolStripMenuItem parent = new ToolStripMenuItem(title);
			if (image != null)
			{
				parent.Image = image;
			}
			UpdateCopySendToMenu(parent, method);
			return parent;
		}
		/// <summary>
		/// TODO: Finish this comment
		/// </summary>
		/// <param name="title"></param>
		/// <param name="method"></param>
		/// <returns></returns>		/// 
		private void UpdateCopySendToMenu(ToolStripMenuItem menu, MogMenuItem_Click method)
		{
			menu.DropDownItems.Clear();

			// Update the recent list
			if (MogUtils_Settings.MogUtils_Settings.Settings.SectionExist("SendCopyToRecents"))
			{
				for (int i = 0; i < MogUtils_Settings.MogUtils_Settings.Settings.CountKeys("SendCopyToRecents"); i++)
				{
					string recentUser = MogUtils_Settings.MogUtils_Settings.Settings.GetKeyNameByIndexSLOW("SendCopyToRecents", i);

					ToolStripMenuItem recentUserItem = new ToolStripMenuItem(recentUser);
					recentUserItem.Click += new System.EventHandler(method);
					menu.DropDownItems.Add(recentUserItem);
				}

				// Add a seperator
				ToolStripSeparator seperatorItem = new ToolStripSeparator();
				menu.DropDownItems.Add(seperatorItem);
			}

			SortedList<string, List<string>> departments = new SortedList<string, List<string>>();

			// Add all the users to the AssignTo subMenu
			ArrayList users = MOG_ControllerProject.GetProject().GetUsers();
			for (int u = 0; u < users.Count; u++)			
			{
				MOG_User user = (MOG_User)users[u];

				// Does this department exist?
				if (departments.ContainsKey(user.GetUserDepartment().ToLower()) == false)
				{
					// No, then add it with the user
					List<string> userList = new List<string>();
					userList.Add(user.GetUserName());

					departments.Add(user.GetUserDepartment().ToLower(), userList);
				}
				else
				{
					// yup, its already there so add this user to the array
					departments[user.GetUserDepartment().ToLower()].Add(user.GetUserName());
				}
			}

			foreach (string department in departments.Keys)
			{
				ToolStripMenuItem departmentItem = new ToolStripMenuItem(department);
				foreach (string userName in departments[department])
				{
					ToolStripMenuItem userNameItem = new ToolStripMenuItem(userName);
					userNameItem.Click += new System.EventHandler(method);
					departmentItem.DropDownItems.Add(userNameItem);
				}
				menu.DropDownItems.Add(departmentItem);				
			}			
		}

		#region Change properties menu
		private void PopulateChangePropertiesToolsMenu(MOG_Properties properties, string assetName, string ContextMenu, MogMenuItem_Click method)
		{
			MOG_Filename MogAssetName = new MOG_Filename(assetName);
			ToolStripMenuItem menuRoot = null;

			// Find the change properties menuItem
			foreach (ToolStripItem item in Items)
			{
				if (MOG.StringUtils.StringCompare(item.Text, "Change Properties*"))
				{
					menuRoot = item as ToolStripMenuItem;
					break;
				}
			}

			try
			{
				if (menuRoot != null)
				{
					string fullFilename = "";
					mPropertyMenu = new HybridDictionary();

					// Do we already have a context menu specified
					if (ContextMenu.Length > 0 && ContextMenu != "*")
					{
						fullFilename = MOG_Tokens.GetFormattedString(ContextMenu, MOG_Tokens.GetProjectTokenSeeds(MOG_ControllerProject.GetProject()));
					}
					else
					{
						// Check if this asset has a menu associated with it						
						if (properties != null && properties.PropertyMenu.Length > 0)
						{	
							fullFilename = MOG_Tokens.GetFormattedString(properties.PropertyMenu, MOG_Tokens.GetProjectTokenSeeds(MOG_ControllerProject.GetProject()));
						}
					}

					if (fullFilename.Length == 0)
					{
						menuRoot.Text = "Change Properties (N/A)";						
						menuRoot.Enabled = false;					
						return;
					}

					// Make sure we got a filename with a full path, if we didn't it is probably a relational path
					if (!Path.IsPathRooted(fullFilename))
					{
						fullFilename = MOG_ControllerProject.GetProject().GetProjectToolsPath() + "\\" + fullFilename;
					}

					// Find and open the menu
					if (DosUtils.FileExist(fullFilename))
					{
						MOG_PropertiesIni ripMenu = new MOG_PropertiesIni(fullFilename);
						if (ripMenu.SectionExist("Property.Menu"))
						{
							// Clear the list
							menuRoot.Enabled = true;
							menuRoot.Text = "Change Properties (" + Path.GetFileNameWithoutExtension(fullFilename) + ")";

							// Clear all the menuItems except the wizard
							menuRoot.DropDownItems.Clear();
							
							string property = "";

							// Create the sub menu
							CreateChangePropertiesSubMenu(property, properties, menuRoot, "Property.Menu", ripMenu, method);							
						}
						else
						{
							throw new Exception("MENU: " + fullFilename + "\n Does not have a [Property.Menu] section! Menu could not load.");
						}
					}
					else
					{
						throw new Exception("MENU: " + fullFilename + "\n Does not exist or is not found.");
					}
				}				
			}
			catch(Exception e)
			{
				//GLK:  Turned this to ReportSilent because, if we show a message, the ContextMenu for this
				//	Asset(s) gets interrupted, making it such that the user *cannot* open the menu to do *ANYTHING*...
				MOG_Report.ReportSilent("Custom Properties Menu", e.Message, e.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.ERROR);
				menuRoot.Text = "Change Properties (N/A)";
				menuRoot.Enabled = false;
			}
		}

		private void CreateChangePropertiesSubMenu(string propertyPath, MOG_Properties properties, ToolStripMenuItem parentMenuItem, string section, MOG_PropertiesIni ripMenu, MogMenuItem_Click method)
		{
			for (int i = 0; i < ripMenu.CountProperties(section, "MenuItem"); i++)
			{
				MOG_Property property = ripMenu.GetPropertyByIndex(section, "MenuItem", i);
				if (property != null)
				{
					string menuItem = property.mPropertyKey;
					if (propertyPath.Length == 0)
					{
						CreateChangePropertiesMenuItem(menuItem, properties, parentMenuItem, section, menuItem, ripMenu, method);
					}
					else
					{					
						CreateChangePropertiesMenuItem(propertyPath + "/" + menuItem, properties, parentMenuItem, section, menuItem, ripMenu, method);
					}
				}
			}
		}

		private void CreateChangePropertiesMenuItem(string propertyPath, MOG_Properties properties, ToolStripMenuItem parentMenuItem, string section, string itemName, MOG_PropertiesIni ripMenu, MogMenuItem_Click method)
		{
			ToolStripMenuItem propertyMenuItem = new ToolStripMenuItem(itemName);
			string command = ripMenu.GetPropertyString(section, "MenuItem", itemName);

			// Check to see if this command is another sub menu
			if (ripMenu.SectionExist(command))
			{
				CreateChangePropertiesSubMenu(propertyPath, properties, propertyMenuItem, command, ripMenu, method);
			}
			else
			{
				// Save our command tag for future reference
				propertyMenuItem.Click += new EventHandler(method);

				// Make sure this looks like a valid property?
				if (command.Contains("="))
				{
					// Is this property already set for this asset?
					string[] parts = command.Substring(0, command.IndexOf("=")).Split("[]{}".ToCharArray());
					if (parts.Length == 5)
					{
						string sectionName = parts[1];
						string propertySection = parts[3];
						string propertyName = parts[4];
						string propertyValue = command.Substring(command.IndexOf("=") + 1);

						if (properties.DoesPropertyExist(sectionName, propertySection, propertyName))
						{
							// Are the set values the same
							if (string.Compare(properties.GetPropertyString(sectionName, propertySection, propertyName), propertyValue, true) == 0)
							{
								// Check our menuItem
								propertyMenuItem.Checked = true;
							}
						}
					}

					// Add to our propertyMenu set for future lookup
					mPropertyMenu[propertyPath] = command;
				}
			}

			parentMenuItem.DropDownItems.Add(propertyMenuItem);
		}
		#endregion

		/// <summary>
		/// TODO: Finish this comment
		/// </summary>
		/// <param name="menu"></param>
		private void PopulateCustomToolsMenu(ToolStripMenuItem menuItem)
		{
			// Init all custom tools
			string customToolsInfoFilename =	MOG_ControllerProject.GetProject().GetProjectToolsPath() + "\\" + 
				MOG_ControllerProject.GetProjectName() + "." +
				"Client" + "." +
				"Assets.*.Info";

			string dir = customToolsInfoFilename.Substring(0, customToolsInfoFilename.LastIndexOf("\\"));
			string wildcard = customToolsInfoFilename.Substring(customToolsInfoFilename.LastIndexOf("\\")+1, (customToolsInfoFilename.Length - customToolsInfoFilename.LastIndexOf("\\"))-1);

			foreach (FileInfo file in DosUtils.FileGetList(dir, wildcard))
			{
				MOG_Ini customTools = new MOG_Ini(file.FullName);

				// Remove the beginning of the info name
				string tmp = wildcard.Substring(0, wildcard.IndexOf("*"));
				string name = file.Name.Replace(tmp , "");

				// Remove the .info
				name = name.Replace(".info", "");
				
				// Create the main Item
				ToolStripMenuItem parent = new ToolStripMenuItem(name);

				if (customTools.SectionExist("USER_TOOLS"))
				{
					for (int i = 0; i < customTools.CountKeys("USER_TOOLS"); i++)
					{
						parent.DropDownItems.Add(CreateItem(customTools.GetKeyNameByIndexSLOW("USER_TOOLS", i), true, Keys.None, new MogMenuItem_Click(MenuItemCustomTools_Click)));
					}
				}

				menuItem.DropDownItems.Add(parent);
			}
		}

		#endregion

		#region UTILITY FUNCTIONS
		/// <summary>
		/// Returns true if the user wants to continue to ShellSpawn more than 10 assets
		/// </summary>
		/// <param name="selectedItems"></param>
		/// <returns></returns>
		private bool UserWantsToShellExecuteManyAssets( ArrayList selectedItems, string menuItemText )
		{
			if( selectedItems.Count > 10 )
			{
				string message = "You have selected to " + menuItemText + " " + selectedItems.Count + " Assets.  "
					+ "\nThis might take a long time and MOG may not respond while all these Assets "
					+ "are opening through Windows(tm).\r\n\r\nAre you sure you want to open all " + selectedItems.Count + " Assets?";
				// If our user does not want to open all these assets, return
				if( MOGPromptResult.Yes != MOG_Prompt.PromptResponse( "Warning -- Many Assets Selected!!", message ))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Locates a string in an array and returns its index
		/// </summary>
		private int ColumnNameFind(string []cols, string name)
		{
			for (int x = 0; x < cols.Length; x++)
			{
				if (String.Compare(cols[x], name, true) == 0)
				{
					return x;
				}
			}

			return -1;
		}

		/// <summary>
		/// Retrieve the text found within a specified column of a listviewItems subitems
		/// </summary>
		private string GetColumnString(string columnName, ListViewItem item)
		{
			int networkIdIndex = ColumnNameFind(mColumns, columnName);
			if (networkIdIndex != -1 && networkIdIndex < item.SubItems.Count)
				return item.SubItems[networkIdIndex].Text;

			return "";
		}

		private ArrayList ControlGetSelectedItemTags()
		{
			ArrayList nodes = new ArrayList();

			// Get the list nodes
			if (mView != null && mTree == null)
			{
				foreach (int i in mView.SelectedIndices)
				{
					ListViewItem item = mView.Items[i];
					int index = ColumnNameFind(mColumns, "Fullname");

					if (index != -1 && index < item.SubItems.Count)
					{
						try
						{
							string fullFilename = item.SubItems[index].Text;

							Mog_BaseTag Tag = new Mog_BaseTag(item, fullFilename, RepositoryFocusLevel.Repository, true);
							Tag.Owner = item;
							
							int networkIdIndex = ColumnNameFind(mColumns, "NetworkId");
							if (networkIdIndex != -1 && networkIdIndex < item.SubItems.Count)
								Tag.NetworkId = Convert.ToInt32(item.SubItems[networkIdIndex].Text);
                            
							int commandIdIndex = ColumnNameFind(mColumns, "CommandId");
							if (commandIdIndex != -1 && commandIdIndex < item.SubItems.Count)
								Tag.CommandId = Convert.ToUInt32(item.SubItems[commandIdIndex].Text);

							nodes.Add(Tag);
						}
						catch(Exception ex)
						{
							MOG_Report.ReportMessage("ListView item index out of range", ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
						}
					}
				}
			}
				// Get the tree nodes
			else if (mView == null && mTree != null)
			{
				if (mTree.SelectedNode != null)
				{
					Mog_BaseTag Tag = (Mog_BaseTag)mTree.SelectedNode.Tag;
					Tag.Owner = mTree.SelectedNode;

					nodes.Add(Tag);
				}
			}
			else
			{
				// No valid nodes
				return null;
			}

			return nodes;
		}

		private string FormatString(Mog_BaseTag tag, string format)
		{
			// Replace out any string options {}
			if (format.IndexOf("{projectRoot}") != -1)
			{
				format = format.Replace("{projectRoot}", MOG_ControllerProject.GetCurrentSyncDataController().GetSyncDirectory());
			}

			// Replace out any string options {}
			if (format.IndexOf("{projectName}") != -1)
			{
				format = format.Replace("{projectName}", MOG_ControllerProject.GetProjectName());
			}

			// Replace out any string options {}
			if (format.IndexOf("{LoginUserName}") != -1)
			{
				format = format.Replace("{LoginUserName}", MOG_ControllerProject.GetActiveUser().GetUserName());
			}

			// Replace out any string options {}
			if (format.IndexOf("{SelectedItem}") != -1)
			{
				format = format.Replace("{SelectedItem}", tag.FullFilename);
			}

			return format;
		}

		private bool ExecucteCustomToolWindow(string toolGroup, string toolName, Mog_BaseTag tag)
		{
			// TODO Remove MainForm

			//			bool completed = false;
			//			// Init all custom tools
			//			string customToolsInfoFilename =	MOG_ControllerProject.GetProject().GetProjectToolsPath() + "\\" + 
			//				MOG_ControllerProject.GetProjectName() + "." +
			//				"Client" + "." +
			//				"Assets.*.Info";
			//
			//			string infoFilename = customToolsInfoFilename.Replace("*", toolGroup);
			//			MOG_Ini customTools = new MOG_Ini(infoFilename);
			//
			//			string command = "";
			//			string argAsset = "";
			//			string dialog = "";
			//			System.Diagnostics.ProcessWindowStyle windowMode = System.Diagnostics.ProcessWindowStyle.Normal;
			//			bool createWindow = false;
			//
			//			if (customTools.SectionExist(toolName))
			//			{
			//				// Get command
			//				if (customTools.KeyExist(toolName, "Command"))
			//				{
			//					command = FormatString(tag, customTools.GetString(toolName, "Command"));
			//				}
			//
			//				// Get argAsset
			//				if (customTools.KeyExist(toolName, "argAsset"))
			//				{
			//					argAsset = FormatString(tag, customTools.GetString(toolName, "argAsset"));
			//				}
			//
			//				// Get dialog if one is present
			//				if (customTools.KeyExist(toolName, "dialog"))
			//				{
			//					dialog = FormatString(tag, customTools.GetString(toolName, "dialog"));
			//					createWindow = true;
			//				}
			//
			//				// Get window mode
			//				if (customTools.KeyExist(toolName, "windowMode"))
			//				{
			//					string mode = FormatString(tag, customTools.GetString(toolName, "windowMode"));
			//
			//					if (string.Compare(mode, "Hidden", true) == 0)
			//					{
			//						windowMode = System.Diagnostics.ProcessWindowStyle.Hidden;
			//					}
			//					else if (string.Compare(mode, "Maximise", true) == 0)
			//					{
			//						windowMode = System.Diagnostics.ProcessWindowStyle.Maximized;
			//					}
			//					else if (string.Compare(mode, "Minimized", true) == 0)
			//					{
			//						windowMode = System.Diagnostics.ProcessWindowStyle.Minimized;
			//					}
			//					else if (string.Compare(mode, "Normal", true) == 0)
			//					{
			//						windowMode = System.Diagnostics.ProcessWindowStyle.Normal;
			//					}
			//					else
			//					{
			//						windowMode = System.Diagnostics.ProcessWindowStyle.Normal;
			//					}
			//				}
			//
			//				if (createWindow)
			//				{
			//					CustomToolOptionsForm form = new CustomToolOptionsForm(MOG_ControllerProject.GetProject().GetProjectToolsPath() + "\\" + dialog);
			//					form.Text = toolName;
			//
			//					if (form.ShowDialog() == DialogResult.OK)
			//					{
			//						// Do stuff
			//					}
			//				}
			//				else
			//				{					
			//					string output="";
			//					Report outputForm = new Report(mainForm);
			//					if (windowMode == System.Diagnostics.ProcessWindowStyle.Hidden)
			//					{
			//						outputForm.LogRichTextBox.Text = "GENERATING REPORT.  PLEASE WAIT...";
			//						outputForm.LogOkButton.Enabled = false;
			//
			//						// Load saved positions
			//						//mainForm.mUserPrefs.Load("ReportForm", outputForm);
			//						guiUserPrefs.Load("ReportForm", outputForm);
			//
			//						outputForm.Show();
			//						Application.DoEvents();
			//					}
			//
			//					if (guiCommandLine.ShellSpawn(command.Trim(), argAsset.Trim(), windowMode, ref output) == 0)
			//					{
			//						completed = true;
			//					}
			//
			//					if (windowMode == System.Diagnostics.ProcessWindowStyle.Hidden)
			//					{							
			//						outputForm.LogRichTextBox.Text = output;
			//						outputForm.LogOkButton.Enabled = true;
			//					}
			//				}
			//			}

			//			return completed;
			return false;
		}
		#endregion

		#region Sample Click Handle
		/*
		private void MenuItemSample_Click(object sender, System.EventArgs e)
		{
			ArrayList selectedItems = ControlGetSelectedItemTags();

			string message = "";
			foreach (Mog_BaseTag tag in selectedItems)
			{				
				if (tag.Execute)
				{
					message = message + tag.FullFilename + "\n";
				}
			}

			if (MOG_Prompt.PromptResponse("Are you sure you want to remove all of these assets from the project?", message, MOGPromptButtons.OKCancel) == MOGPromptResult.OK)
			{
				foreach (Mog_BaseTag tag in selectedItems)
				{				
					if (tag.Execute)
					{
						MOG_Filename filename = new MOG_Filename(tag.FullFilename);
					
						// Make sure we are an asset before showing log
						if (filename.GetType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
						{
							// Place code here							
						}					
					}
				}
			}
		}
		*/
		#endregion
		
		#region CLICK HANDLES

		

		private void MenuItemRemoteDesktop_Click(object sender, System.EventArgs e)
		{
			ArrayList selectedItems = ControlGetSelectedItemTags();
			
			foreach (Mog_BaseTag tag in selectedItems)
			{	
				try
				{
					if (tag.Execute && tag.Owner != null)
					{
						ListViewItem item = tag.Owner as ListViewItem;
						guiCommandLine.ShellSpawn("mstsc.exe", "/v:" + item.Text);
					}
				} 
				catch(Exception ex)
				{
					MOG_Prompt.PromptResponse(RemoteDesktop_MenuItemText, ex.Message, ex.StackTrace, MOGPromptButtons.OK, MOG.PROMPT.MOG_ALERT_LEVEL.ERROR);
				}
			}			
		}
		
		private void MenuItemRefresh_Click(object sender, System.EventArgs e)
		{
			MainMenuViewClass.MOGGlobalViewRefresh(MogMainForm.MainApp);
		}

		private void MenuItemRepackage_Click(object sender, System.EventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;

			ArrayList selectedItems = ControlGetSelectedItemTags();

			string message = "";
			foreach (Mog_BaseTag tag in selectedItems)
			{				
				if (tag.Execute)
				{
					message = message + tag.FullFilename + "\n";
				}
			}

			WorkspaceManager.SuspendPackaging(true);

			foreach (Mog_BaseTag tag in selectedItems)
			{				
				if (tag.Execute)
				{
					MOG_Filename filename = new MOG_Filename(tag.FullFilename);
			
					// Make sure we are an asset before showing log
					if (filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
					{
						// Repackage each asset
						WorkspaceManager.RepackageAssetInWorkspaces(filename, worker);
					}					
				}
			}

			WorkspaceManager.SuspendPackaging(false);
		}

		private void MenuItemKillConnection_Click(object sender, System.EventArgs e)
		{
			ArrayList selectedItems = ControlGetSelectedItemTags();

			foreach (Mog_BaseTag tag in selectedItems)
			{				
				if (tag.Execute)
				{
					MOG_ControllerSystem.ConnectionKill(Convert.ToInt32(tag.FullFilename));
				}
			}
		}

		private void MenuItemStartSlave_Click(object sender, System.EventArgs e)
		{
			ArrayList selectedItems = ControlGetSelectedItemTags();

			foreach (Mog_BaseTag tag in selectedItems)
			{				
				if (tag.Execute)
				{
					MOG_ControllerSystem.LaunchSlave(Convert.ToInt32(tag.FullFilename));
				}
			}
		}	

		private void MenuItemRetaskCommand_Click(object sender, System.EventArgs e)
		{
			ArrayList selectedItems = ControlGetSelectedItemTags();

			foreach (Mog_BaseTag tag in selectedItems)
			{				
				if (tag.Execute)
				{
					MOG_ControllerSystem.RetaskAssignedSlaveCommand(Convert.ToInt32(tag.FullFilename));
				}
			}
		}

		private void MenuItemDeleteCommand_Click(object sender, System.EventArgs e)
		{
			ArrayList selectedItems = ControlGetSelectedItemTags();

			List<Mog_BaseTag> remove = new List<Mog_BaseTag>();

			foreach (Mog_BaseTag tag in selectedItems)
			{				
				if (tag.Execute)
				{
					if (MOG_ControllerSystem.KillCommandFromServer(tag.CommandId))
					{
						remove.Add(tag);
					}
				}
			}

			foreach (Mog_BaseTag item in remove)
			{
				item.ItemRemove();
			}
		}

        private void MenuItemDeletePackageCommand_Click(object sender, System.EventArgs e)
        {
            ArrayList selectedItems = ControlGetSelectedItemTags();

            foreach (Mog_BaseTag tag in selectedItems)
            {
                if (tag.Execute)
                {
					if (MOG_DBPackageCommandAPI.RemovePackageCommand(tag.CommandId))
					{
						MainMenuViewClass.MOGGlobalViewRefresh(MogMainForm.MainApp);
					}
                }
            }
        }		

        private void MenuItemDeletePostCommand_Click(object sender, System.EventArgs e)
        {
            ArrayList selectedItems = ControlGetSelectedItemTags();

            foreach (Mog_BaseTag tag in selectedItems)
            {
                if (tag.Execute)
                {
					if (MOG_DBPostCommandAPI.RemovePost(tag.CommandId))
					{
						MainMenuViewClass.MOGGlobalViewRefresh(MogMainForm.MainApp);
					}
                }
            }
        }

		private void MenuItemRestartCommand_Click(object sender, System.EventArgs e)
		{
			ArrayList selectedItems = ControlGetSelectedItemTags();

			foreach (Mog_BaseTag tag in selectedItems)
			{
				if (tag.Execute)
				{
					ListViewItem post = (ListViewItem)tag.Owner;

					MOG_ControllerProject.StartJob(GetColumnString("Label", post));
				}
			}
		}

		private void MenuItemViewCommand_Click(object sender, System.EventArgs e)
		{
			ArrayList selectedItems = ControlGetSelectedItemTags();

			foreach (Mog_BaseTag tag in selectedItems)
			{
				if (tag.Execute)
				{
					ListViewItem post = (ListViewItem)tag.Owner;
					MOG_Command command = post.Tag as MOG_Command;
					if (command != null)
					{
						CommandViewer viewer = new CommandViewer(command);
						viewer.Show();
					}					
				}
			}
		}

		private void MenuItemRestartPackageCommand_Click(object sender, System.EventArgs e)
        {
            ArrayList selectedItems = ControlGetSelectedItemTags();

            foreach (Mog_BaseTag tag in selectedItems)
            {
                if (tag.Execute)
                {
					MOG_Filename package = new MOG_Filename(tag.FullFilename);
					ListViewItem post = (ListViewItem)tag.Owner;

					MOG_ControllerProject.RestartJob(package, GetColumnString("Version", post), GetColumnString("Label", post));
                }
            }
        }

		private void MenuItemRestartPostCommand_Click(object sender, System.EventArgs e)
        {
            ArrayList selectedItems = ControlGetSelectedItemTags();

            foreach (Mog_BaseTag tag in selectedItems)
            {
                if (tag.Execute)
                {
					MOG_Filename package = new MOG_Filename(tag.FullFilename);
					ListViewItem packageCmd = (ListViewItem)tag.Owner;

					MOG_ControllerProject.RestartJob(package, GetColumnString("Version", packageCmd), GetColumnString("Label", packageCmd));
                }
            }
        }

        private void MenuItemRestartLateResolverCommand_Click(object sender, System.EventArgs e)
        {
            ArrayList selectedItems = ControlGetSelectedItemTags();

            foreach (Mog_BaseTag tag in selectedItems)
            {
                if (tag.Execute)
                {
                    MOG_Filename package = new MOG_Filename(tag.FullFilename);
                    ListViewItem packageCmd = (ListViewItem)tag.Owner;

                    MOG_ControllerProject.RestartJob(package, GetColumnString("Version", packageCmd), GetColumnString("Label", packageCmd));
                }
            }
        }

        private void MenuItemDeleteLateResolverCommand_Click(object sender, System.EventArgs e)
        {
            ArrayList selectedItems = ControlGetSelectedItemTags();

            foreach (Mog_BaseTag tag in selectedItems)
            {
                if (tag.Execute)
                {
                    if (MOG_DBPackageCommandAPI.RemovePackageCommand(tag.CommandId))
                    {
                        MainMenuViewClass.MOGGlobalViewRefresh(MogMainForm.MainApp);
                    }
                }
            }           
        }
		
		private void MenuItemSync_Click(object sender, System.EventArgs e)
		{
			try
			{
				ArrayList selectedItems = ControlGetSelectedItemTags();
				if (selectedItems.Count > 0)
				{
					ArrayList items = new ArrayList();
					bool bAlwaysAskUser = false;

					// Check if this is a tree node with a classification?
					Mog_BaseTag tag0 = selectedItems[0] as Mog_BaseTag;
					if (tag0.Owner is TreeNode && ((TreeNode)tag0.Owner).TreeView != null)
					{
						// We need the classification name, and the place to get it is the fullpath of the treenode
						string classification = MOG_ControllerLibrary.EnsureClassificationIsWithinLibrary(((TreeNode)tag0.Owner).FullPath);

						// Get all the library assets with this classification
						items = MOG.DATABASE.MOG_DBAssetAPI.GetAllAssetsByParentClassification(classification);

// JohnRen - This was rather slow because of the icons and is problematic sync an artist may not have the files locally synced in the firtst place.
// Besides, do we really need to ask the user about what files they are going to sync?
//						// Always ask the user because this was a classification
//						bAlwaysAskUser = true;
					}
					else
					{
						foreach (Mog_BaseTag tag in selectedItems)
						{
							if (tag.Execute)
							{
								items.Add(new MOG_Filename(tag.FullFilename));
							}
						}
					}

					// Check if we have any items?
					if (items.Count > 0)
					{
						// Show the list to the user and obtain confirmation if there are multiples
						selectedItems = GetUserConfirmationIfNeeded(bAlwaysAskUser, items, "GetLatest", "Are you sure you want to sync all of these items?");
						if (selectedItems.Count > 0)
						{
							if (MOG_ControllerLibrary.GetLatest(selectedItems))
							{
								MainMenuViewClass.MOGGlobalViewRefresh(MogMainForm.MainApp);
							}
						}
					}
					else
					{
						// There were no assets found to sync
						MOG_Prompt.PromptResponse("GetLatest", "There were no contained assets found for syncing.");
					}
				}
			}
			catch (Exception ex)
			{
				ex.ToString();
			}
		}

		private void MenuItemClean_Click(object sender, System.EventArgs e)
		{
			try
			{
				ArrayList selectedItems = ControlGetSelectedItemTags();
				if (selectedItems.Count > 0)
				{
					// Check if this is a tree node with a classification?
					Mog_BaseTag tag0 = selectedItems[0] as Mog_BaseTag;
					if (tag0.Owner is TreeNode && ((TreeNode)tag0.Owner).TreeView != null)
					{
						// We need the classification name, and the place to get it is the fullpath of the treenode
						string classification = MOG_ControllerLibrary.EnsureClassificationIsWithinLibrary(((TreeNode)tag0.Owner).FullPath);

						// Detect the proper startingPath
						string startingPath = "";
						if (MOG_Filename.IsLibraryClassification(classification))
						{
							startingPath = MOG_ControllerLibrary.ConstructPathFromLibraryClassification(classification);
						}
						else
						{
							// TODO - We need to decide how to specify the startingPath in non-Library settings
							startingPath = "";
						}

						// Make sure we found a valid startingPath?
						if (startingPath.Length > 0)
						{
							// Construct a cleaner and perform the clean
							MogUtil_WorkspaceCleaner cleaner = new MogUtil_WorkspaceCleaner();
							if (cleaner.Clean(startingPath))
							{
								// Check if this was a library explorer?
								MogControl_LibraryExplorer explorer = ((TreeNode)tag0.Owner).TreeView.Parent as MogControl_LibraryExplorer;
								if (explorer != null)
								{
									// Make sure we refresh the view
									explorer.Refresh();
								}
							}
						}
						else
						{
							// There were no assets after the list was filtered
							MOG_Prompt.PromptResponse("Clean", "Specified directory is invalid.");
						}
					}
				}
			}
			catch (Exception ex)
			{
				ex.ToString();
			}
		}



		private void MenuItemLibraryCheckout_Click(object sender, System.EventArgs e)
		{
			ArrayList selectedItems = ControlGetSelectedItemTags();

			try
			{
				if (selectedItems.Count > 0)
				{
					ArrayList items = new ArrayList();
					bool bAlwaysAskUser = false;

					// Check if this is a treenode with a classification in it?
					Mog_BaseTag tag0 = selectedItems[0] as Mog_BaseTag;
					if (tag0.Owner is TreeNode && ((TreeNode)tag0.Owner).TreeView != null)
					{
						// We need the classification name, and the place to get it is the fullpath of the treenode
						string classification = MOG_ControllerLibrary.EnsureClassificationIsWithinLibrary(((TreeNode)tag0.Owner).FullPath);
						// Get all the library assets with this classification
						items = MOG.DATABASE.MOG_DBAssetAPI.GetAllAssetsByParentClassification(classification);
						// Always ask the user because this was a classification
						bAlwaysAskUser = true;
					}
					else
					{
						foreach (Mog_BaseTag tag in selectedItems)
						{
							if (tag.Execute)
							{
								items.Add(new MOG_Filename(tag.FullFilename));
							}
						}
					}

					// Check if we have any items?
					if (items.Count > 0)
					{
						// Filter the list so we won't try to check in assets that are not checked out
						ArrayList assets = FilterUnavailableAssets(items);
						if (assets.Count > 0)
						{
							// Show the list to the user and obtain confirmation if there are multiples
							selectedItems = GetUserConfirmationIfNeeded(bAlwaysAskUser, assets, "Checkout", "Are you sure you want to check out all of these items?");
							if (selectedItems.Count > 0)
							{
								string comment = "";
								bool maintainLock = false;
								ArrayList selectedItemsWithComments = new ArrayList();
								ArrayList selectedItemsNoComments = new ArrayList();

								GetCommentsForList(CommentType.Checkout, ref comment, ref maintainLock, selectedItems, selectedItemsWithComments, selectedItemsNoComments, false, false);

								if (selectedItemsWithComments.Count > 0)
									MOG_ControllerLibrary.CheckOut(selectedItemsWithComments, comment);

								if (selectedItemsNoComments.Count > 0)
									MOG_ControllerLibrary.CheckOut(selectedItemsNoComments, "");

								RefreshOwningMenu(sender as ToolStripMenuItem);
							}
						}
						else
						{
							// There were no assets after the list was filtered
							MOG_Prompt.PromptResponse("Nothing to check out", "There were no assets found available for checked out.");
						}
					}
				}
			}
			catch (Exception ex)
			{
				ex.ToString();
			}
		}

		private void MenuItemLibraryCheckin_Click(object sender, System.EventArgs e)
		{
			try
			{
				ArrayList selectedItems = ControlGetSelectedItemTags();
				if (selectedItems.Count > 0)
				{
					ArrayList items = new ArrayList();
					bool bAlwaysAskUser = false;

					// Check if this is a treenode with a classification in it?
					Mog_BaseTag tag0 = selectedItems[0] as Mog_BaseTag;
					if (tag0.Owner is TreeNode && ((TreeNode)tag0.Owner).TreeView != null)
					{
						//We need the classification name, and the place to get it is the fullpath of the treenode
						string classification = MOG_ControllerLibrary.EnsureClassificationIsWithinLibrary(((TreeNode)tag0.Owner).FullPath);
						//Get all the library assets with this classification
						items = MOG.DATABASE.MOG_DBAssetAPI.GetAllAssetsByParentClassification(classification);
						// Always ask the user because this was a classification
						bAlwaysAskUser = true;
					}
					else
					{
						foreach (Mog_BaseTag tag in selectedItems)
						{
							if (tag.Execute)
							{
								items.Add(new MOG_Filename(tag.FullFilename));
							}
						}
					}

					// Check if we have any items?
					if (items.Count > 0)
					{
						// Filter the list so we won't try to check in assets that are not checked out
						ArrayList assets = FilterNonCheckedOutAssets(items);
						if (assets.Count > 0)
						{
							// Show the list to the user and obtain confirmation if there are multiples
							selectedItems = GetUserConfirmationIfNeeded(bAlwaysAskUser, assets, "Checkin", "Are you sure you want to check in all of these items?");
							if (selectedItems.Count > 0)
							{
								string comment = "";
								bool maintainLock = false;
								ArrayList selectedItemsWithComments = new ArrayList();
								ArrayList selectedItemsNoComments = new ArrayList();

								GetCommentsForList(CommentType.Checkin, ref comment, ref maintainLock, selectedItems, selectedItemsWithComments, selectedItemsNoComments, false, true);

								if (selectedItemsWithComments.Count > 0)
									MOG_ControllerLibrary.CheckIn(selectedItemsWithComments, comment, maintainLock, "");

								if (selectedItemsNoComments.Count > 0)
									MOG_ControllerLibrary.CheckIn(selectedItemsNoComments, "", false, "");

								RefreshOwningMenu(sender as ToolStripMenuItem);								
							}
						}
						else
						{
							//This asset doesn't have any files associated with it
							MOG_Prompt.PromptResponse("Nothing to check in", "There were no assets found needing to be checked in.");
						}
					}
				}
			}
			catch (Exception ex)
			{
				ex.ToString();
			}
		}

		private ArrayList FilterUnavailableAssets(ArrayList assets)
		{
			ArrayList availableAssets = new ArrayList();

			foreach (MOG_Filename filename in assets)
			{
				if (filename.GetOriginalFilename().Length > 0)
				{
					// Check if this is locked by me?
					if (!MOG_ControllerProject.IsLocked(filename.GetAssetFullName()))
					{
						availableAssets.Add(filename);
					}
				}
			}

			return availableAssets;
		}

		private ArrayList FilterNonCheckedOutAssets(ArrayList assets)
		{
			ArrayList checkedoutAssets = new ArrayList();

			foreach (MOG_Filename filename in assets)
			{
				if (filename.GetOriginalFilename().Length > 0)
				{
					// Check if this is locked by me?
					if (MOG_ControllerProject.IsLockedByMe(filename.GetAssetFullName()))
					{
						checkedoutAssets.Add(filename);
					}
				}
			}

			return checkedoutAssets;
		}

		private ArrayList GetUserConfirmationIfNeeded(bool bAlwaysAskUser, ArrayList assets, string title, string message)
		{
			ArrayList selectedItems = new ArrayList();

			if (bAlwaysAskUser && assets.Count > 1)
			{
				// Convert these to strings so that they can then be converted back to MOG_Filenames...YUCK!!! (We should enhance this API)
				ArrayList assetFileNames = new ArrayList();
				ArrayList labels = new ArrayList();
				string directory = MOG_ControllerLibrary.GetWorkingDirectory();

				foreach (MOG_Filename filename in assets)
				{
					assetFileNames.Add(filename.GetOriginalFilename());

					string adamless = MOG_Filename.GetAdamlessClassification(filename.GetAssetClassification());
					string path = MOG_Filename.GetClassificationPath(adamless);
					string label = directory + "\\" + MOG_ControllerProject.GetProjectName() + "\\" + path + "\\" + filename.GetAssetLabel();
					labels.Add(label);
				}

				// Prompt user so they can see the list of assets
				if (guiConfirmDialog.MessageBoxDialog(title, message, directory, assetFileNames, labels, "\\", MessageBoxButtons.OKCancel) == DialogResult.OK)
				{
					foreach (string fullfilename in guiConfirmDialog.SelectedItems)
					{
						selectedItems.Add(new MOG_Filename(fullfilename));
					}
				}
			}
			else
			{
				// Just give back the user what was sent in
				selectedItems.AddRange(assets);
			}

			return selectedItems;
		}

		private void MenuItemUndoCheckout_Click(object sender, System.EventArgs e)
		{
			try
			{
				ArrayList selectedItems = ControlGetSelectedItemTags();
				if (selectedItems.Count > 0)
				{
					ArrayList items = new ArrayList();
					bool bAlwaysAskUser = false;

					// Check if this is a tree node with a classification?
					Mog_BaseTag tag0 = selectedItems[0] as Mog_BaseTag;
					if (tag0.Owner is TreeNode && ((TreeNode)tag0.Owner).TreeView != null)
					{
						// We need the classification name, and the place to get it is the fullpath of the treenode
						string classification = MOG_ControllerLibrary.EnsureClassificationIsWithinLibrary(((TreeNode)tag0.Owner).FullPath);
						// Get all the library assets with this classification
						items = MOG.DATABASE.MOG_DBAssetAPI.GetAllAssetsByParentClassification(classification);
						// Always ask the user because this was a classification
						bAlwaysAskUser = true;
					}
					else
					{
						foreach (Mog_BaseTag tag in selectedItems)
						{
							if (tag.Execute)
							{
								items.Add(new MOG_Filename(tag.FullFilename));
							}
						}
					}

					// Check if we have any items?
					if (items.Count > 0)
					{
						// Filter the list so we won't try to undo check out on assets that are not checked out
						ArrayList assets = FilterNonCheckedOutAssets(items);
						if (assets.Count > 0)
						{
							// Show the list to the user and obtain confirmation if there are multiples
							selectedItems = GetUserConfirmationIfNeeded(bAlwaysAskUser, assets, "Undo Checkout", "Are you sure you want to undo your checkout on all of these items?");
							if (selectedItems.Count > 0)
							{
								MOG_ControllerLibrary.UndoCheckOut(selectedItems);

								RefreshOwningMenu(sender as ToolStripMenuItem);
							}
						}
						else
						{
							//This asset doesn't have any files associated with it
							MOG_Prompt.PromptResponse("Nothing to undo", "There were no checked out items found.");
						}
					}
				}
			}
			catch (Exception ex)
			{
				ex.ToString();
			}
		}

		private void MenuItemLibraryAdd_Click(object sender, System.EventArgs e)
		{
			try
			{
				ArrayList selectedItems = ControlGetSelectedItemTags();
				if (selectedItems.Count > 0)
				{
					ArrayList items = new ArrayList();
					Mog_BaseTag tag = selectedItems[0] as Mog_BaseTag;
					bool bAskUser = false;
					bool bCanceled = false;
					string startingPath = "";

					// Check if this is a treenode with a classification in it?
					if (tag.Owner is TreeNode && ((TreeNode)tag.Owner).TreeView != null)
					{
						//We need the classification name, and the place to get it is the fullpath of the treenode
						string classification = MOG_ControllerLibrary.EnsureClassificationIsWithinLibrary(((TreeNode)tag.Owner).FullPath);
						// Get the startingPath from the classification
						startingPath = MOG_ControllerLibrary.ConstructPathFromLibraryClassification(classification);
						// Detect the nonMOGFiles
						MogUtil_WorkspaceCleaner cleaner = new MogUtil_WorkspaceCleaner();
						items = cleaner.DetectNonMOGFiles(startingPath);
						if (items.Count > 0)
						{
							// Always ask the user when items were found under a classification
							bAskUser = true;
						}
						else
						{
							// Inform the user that no files were detected
							MOG_Prompt.PromptResponse("Add", "No new files were detected.");
						}
					}
					else
					{
						foreach (Mog_BaseTag item in selectedItems)
						{
							if (item.Execute)
							{
								// Only add files that are actually located in the Library
								if (item.FullFilename.StartsWith(MOG_ControllerLibrary.GetWorkingDirectory(), StringComparison.CurrentCultureIgnoreCase))
								{
									items.Add(item.FullFilename);
								}
							}
						}

						// Calculate the startingPath for this list of items
						startingPath = MOG_ControllerAsset.GetCommonDirectoryPath("", items);
					}

					// Check if we have any items?
					if (bAskUser)
					{
						// Prompt user so they can see the list of assets
						if (guiConfirmDialog.MessageBoxDialog("Add", "Are you sure you want to add all of these items?", startingPath, items, null, "\\", MessageBoxButtons.OKCancel) == DialogResult.OK)
						{
							items = guiConfirmDialog.SelectedItems;
						}
						else
						{
							bCanceled = true;
						}
					}

					// Make sure we haven't been canceled?
					if (!bCanceled)
					{
						// Check if we ended up with anything to add?
						if (items.Count > 0)
						{
							if (MOG_ControllerLibrary.AddFiles(items))
							{
								MainMenuViewClass.MOGGlobalViewRefresh(MogMainForm.MainApp);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				ex.ToString();
			}
		}

		internal void MenuItemLibraryEdit_Click(object sender, System.EventArgs e)
		{
			ArrayList selectedItems = ControlGetSelectedItemTags();

			foreach (Mog_BaseTag tag in selectedItems)
			{
				string comment = "";
				if (tag.Execute)
				{
					try
					{
						string localFile = "";
						bool bCanEdit = false;

						// Check if this is an asset?
						MOG_Filename filename = new MOG_Filename(tag.FullFilename);
						if (filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
						{
							// Using the asset, construct our local file
							localFile = MOG_ControllerLibrary.ConstructLocalFilenameFromAssetName(filename);

							// Check if this asset isn't locked?
							if (!MOG_ControllerProject.IsLocked(filename.GetAssetFullName()))
							{
								// Proceed to check out the asset
								if (GetComment(CommentType.Checkout, ref comment, filename, false, false))
								{
									if (MOG_ControllerLibrary.CheckOut(filename, comment))
									{
										RefreshOwningMenu(sender as ToolStripMenuItem);
										bCanEdit = true;
									}
								}
							}
							else
							{
								// Check if I already have the lock?
								if (MOG_ControllerProject.IsLockedByMe(filename.GetAssetFullName()))
								{
									bCanEdit = true;
								}
							}
						}
						else
						{
							// Assuming this contains a local file, check if this is within our local library?
							if (MOG_ControllerLibrary.IsPathWithinLibrary(filename.GetOriginalFilename()))
							{
								// Looks like we can edit this file
								localFile = filename.GetOriginalFilename();
								bCanEdit = true;
							}
						}

						// Have we determined we can proceed to open the file?
						if (bCanEdit)
						{
							if (DosUtils.FileExistFast(localFile))
							{
								guiCommandLine.ShellSpawn(localFile);
							}
						}
					}
					catch (Exception ex)
					{
						ex.ToString();
					}
				}
			}
		}

        static void OpenWith(string file)
        {
            ShellExecuteInfo sei = new ShellExecuteInfo();
            sei.Size = Marshal.SizeOf(sei);
            sei.Verb = "openas";
            sei.File = file;
            sei.Show = SW_NORMAL;
            if (!ShellExecuteEx(ref sei))
                throw new System.ComponentModel.Win32Exception();
        }
        
        private void MenuItemLibraryOpenWith_Click(object sender, System.EventArgs e)
        {
            ArrayList selectedItems = ControlGetSelectedItemTags();

            foreach (Mog_BaseTag tag in selectedItems)
            {
				string comment = "";
                if (tag.Execute)
                {
                    try
                    {
						string localFile = "";
						bool bCanEdit = false;

						// Check if this is an asset?
                        MOG_Filename filename = new MOG_Filename(tag.FullFilename);
						if (filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
						{
							// Using the asset, construct our local file
							localFile = MOG_ControllerLibrary.ConstructLocalFilenameFromAssetName(filename);

							// Check if this asset isn't locked?
							if (!MOG_ControllerProject.IsLocked(filename.GetAssetFullName()))
							{
								// Proceed to check out the asset
								if (GetComment(CommentType.Checkout, ref comment, filename, false, false))
								{
									if (MOG_ControllerLibrary.CheckOut(filename, comment))
									{
										RefreshOwningMenu(sender as ToolStripMenuItem);
										bCanEdit = true;
									}
								}
							}
							else
							{
								// Check if I already have the lock?
								if (MOG_ControllerProject.IsLockedByMe(filename.GetAssetFullName()))
								{
									bCanEdit = true;
								}
							}
						}
						else
						{
							// Assuming this contains a local file, check if this is within our local library?
							if (MOG_ControllerLibrary.IsPathWithinLibrary(filename.GetOriginalFilename()))
							{
								// Looks like we can edit this file
								localFile = filename.GetOriginalFilename();
								bCanEdit = true;
							}
						}

						// Have we determined we can proceed to open the file?
						if (bCanEdit)
						{
							if (DosUtils.FileExistFast(localFile))
							{
								//This bad boy is checked out, go ahead and edit it
								OpenWith(localFile);
							}
	                    }
                    }
                    catch (Exception ex)
                    {
						ex.ToString();
                    }
                }
            }
        }

        private void MenuItemLibraryOpenWithApp_Click(object sender, System.EventArgs e)
        {
            ArrayList selectedItems = ControlGetSelectedItemTags();

            foreach (Mog_BaseTag tag in selectedItems)
            {
				string comment = "";
                if (tag.Execute)
                {
                    try
                    {
						string localFile = "";
						bool bCanEdit = false;

						// Check if this is an asset?
                        MOG_Filename filename = new MOG_Filename(tag.FullFilename);
						if (filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
						{
							// Using the asset, construct our local file
							localFile = MOG_ControllerLibrary.ConstructLocalFilenameFromAssetName(filename);

							// Check if this asset isn't locked?
							if (!MOG_ControllerProject.IsLocked(filename.GetAssetFullName()))
							{
								// Proceed to check out the asset
								if (GetComment(CommentType.Checkout, ref comment, filename, false, false))
								{
									if (MOG_ControllerLibrary.CheckOut(filename, comment))
									{
										RefreshOwningMenu(sender as ToolStripMenuItem);
										bCanEdit = true;
									}
								}
							}
							else
							{
								// Check if I already have the lock?
								if (MOG_ControllerProject.IsLockedByMe(filename.GetAssetFullName()))
								{
									bCanEdit = true;
								}
							}
						}
						else
						{
							// Assuming this contains a local file, check if this is within our local library?
							if (MOG_ControllerLibrary.IsPathWithinLibrary(filename.GetOriginalFilename()))
							{
								// Looks like we can edit this file
								localFile = filename.GetOriginalFilename();
								bCanEdit = true;
							}
						}

						// Have we determined we can proceed to open the file?
						if (bCanEdit)
						{
							if (DosUtils.FileExistFast(localFile))
							{
								LibrarySpawnOpenWithApp(sender, localFile);
							}
						}
                    }
                    catch (Exception ex)
                    {
						ex.ToString();
                    }
                }
            }
        }

		private static void LibrarySpawnOpenWithApp(object sender, string updatedAssetName)
		{
			string application = (sender as ToolStripMenuItem).Tag as string;
			string command = "";
			string args = "";

			// Now lets separate the command from its arguments
			RegistryGetOpenEditApplication(application, updatedAssetName, ref command, ref args);

			try
			{
				// Ok, spawn the editor with its args for this asset
				guiCommandLine.ShellSpawn(command, args);
			}
			catch (Exception ex)
			{
				MOG_Prompt.PromptResponse("Edit With..", "Editing application tried to open with the following error:\n" + ex.Message);
			}
		}

        private static void RegistryGetOpenEditApplication(string application, string openFile, ref string command, ref string args)
        {
            //This bad boy is checked out, go ahead and edit it
            // HKEY_CLASSES_ROOT\Applications\Acrobat.exe\shell\Open\command
            RegistryKey HKCR = Registry.ClassesRoot;
            RegistryKey rkRun = null;

            for (int i = 0; i < 2; i++)
            {
                switch (i)
                {
                    case 0:
                        rkRun = HKCR.OpenSubKey(@"Applications\\" + application + @"\shell\Open\command");
                        break;
                    case 1:
                        rkRun = HKCR.OpenSubKey(@"Applications\\" + application + @"\shell\Edit\command");
                        break;
                }

                if (rkRun != null) break;
            }

            if (rkRun != null)
            {
                string commandString = rkRun.GetValue("") as string;
                command = DosUtils.FileStripArguments(commandString);
                args = DosUtils.FileGetArguments(commandString).Replace("%1", openFile);

                // Do we have any invalid characters?
                if (command.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                {
                    // Go through our Path::InvalidPathChars to get rid of them...
                    for (int j = 0; j < Path.GetInvalidPathChars().Length; ++j)
                    {
						command = command.Replace(Path.GetInvalidPathChars()[j].ToString(), "");
                    }

                }
            }
        }


		private void MenuItemLibraryView_Click(object sender, System.EventArgs e)
		{
			ArrayList selectedItems = ControlGetSelectedItemTags();

			foreach (Mog_BaseTag tag in selectedItems)
			{
				if (tag.Execute)
				{
					try
					{
						MOG_Filename filename = new MOG_Filename(tag.FullFilename);

						filename = MOG_ControllerProject.GetAssetCurrentBlessedVersionPath( filename );
						
						MOG_Properties pProperties = new MOG_Properties(filename);
						string viewer = pProperties.AssetViewer;

						guiAssetController controller = new guiAssetController();
						controller.View(filename, guiAssetController.AssetDirectories.IMPORTED, viewer);
					}
					catch (Exception ex)
					{
						ex.ToString();
					}
				}
			}
		}

		private void MenuItemLibraryViewRevisions_Click(object sender, System.EventArgs e)
		{
			// Make sure the project tab is unhidden
			MainMenuViewClass.ShowTabPage(MogMainForm.MainApp, MogMainForm.MainApp.VisibleProjectToolStripMenuItem);

			ArrayList selectedItems = ControlGetSelectedItemTags();
			foreach (Mog_BaseTag tag in selectedItems)
			{
				if (tag.Execute)
				{
					try
					{
						MogMainForm.MainApp.MOGTabControl.SelectedTab = MogMainForm.MainApp.MainTabProjectManagerTabPage;
						
						//Select archive view
						MogMainForm.MainApp.ProjectTreeArchiveViewtoolStripButton.PerformClick();

						//expand the tree to the right node
						MOG_Filename asset = new MOG_Filename(tag.FullFilename);
						TreeNode node = MogMainForm.MainApp.ProjectManagerArchiveTreeView.DrillToAssetRevisions(asset.GetAssetOriginalFullName());
						if (node != null)
						{
							//Select the node so people can see it
							MogMainForm.MainApp.ProjectManagerArchiveTreeView.Focus();
							MogMainForm.MainApp.ProjectManagerArchiveTreeView.SelectedNode = node;
						}
					}
					catch
					{
					}
				}
			}
		}

		private void MenuItemProjectGoToAsset_Click(object sender, System.EventArgs e)
		{
			ArrayList selectedItems = ControlGetSelectedItemTags();

			foreach (Mog_BaseTag tag in selectedItems)
			{
				if (tag.Execute)
				{
					try
					{
						MOG_Filename filename = new MOG_Filename(tag.FullFilename);
						MogMainForm.MainApp.MOGTabControl.SelectedTab = MogMainForm.MainApp.MainTabProjectManagerTabPage;

						//Select archive view
						MogMainForm.MainApp.ProjectTreeClassificationViewToolStripButton.PerformClick();

						//expand the tree to the right node
						TreeNode node = MogMainForm.MainApp.ProjectManagerClassificationTreeView.DrillToNodePath(filename.GetAssetFullName());
						if (node != null)
						{
							//Select the node so people can see it
							MogMainForm.MainApp.ProjectManagerClassificationTreeView.Focus();
							MogMainForm.MainApp.ProjectManagerClassificationTreeView.SelectedNode = node;
						}
					}
					catch
					{
					}
				}
			}
		}

		private void MenuItemCopyAssetToClipboard_Click(object sender, System.EventArgs e)
		{
			ArrayList selectedItems = ControlGetSelectedItemTags();
			StringBuilder clipboardText = new StringBuilder();

			foreach (Mog_BaseTag tag in selectedItems)
			{
				try
				{
					// What kind of node is this?
					switch (tag.PackageNodeType)
					{
						case PackageNodeTypes.Class: // This is a class from the project trees
							TreeNode ownerNode = tag.Owner as TreeNode;
							if (ownerNode != null)
							{
								clipboardText.AppendLine(ownerNode.Name);
							}
							break;
						case PackageNodeTypes.Asset:	// This is an asset from the project trees
						case PackageNodeTypes.Package:	// This is a package from the project trees
							MOG_Filename filename = new MOG_Filename(tag.FullFilename);
							clipboardText.AppendLine(filename.GetAssetFullName());
							break;
						case PackageNodeTypes.None: // This is an asset from a listview
							MOG_Filename filename2 = new MOG_Filename(tag.FullFilename);
							if (filename2.GetAssetFullName().Length > 0)
							{
								clipboardText.AppendLine(filename2.GetAssetFullName() + "\\R." + filename2.GetVersionTimeStamp());
							}
							else
							{
								clipboardText.AppendLine(filename2.GetOriginalFilename());
							}
							break;
					}
				}
				catch
				{
				}
			}

			try
			{
				// Put the asset path onto the clipboard
				DataObject dataObject = new DataObject();
				dataObject.SetText(clipboardText.ToString(), TextDataFormat.UnicodeText);
				Clipboard.SetDataObject(dataObject);
			}
			catch
			{
			}
		}

		private void MenuItemPasteAssetFromClipboard_Click(object sender, System.EventArgs e)
		{
			ArrayList selectedItems = ControlGetSelectedItemTags();

			try
			{
				bool bDuplicated = false;
				// Get the asset or assets from the clipboard
				string clipboard = Clipboard.GetText(TextDataFormat.UnicodeText);
				if (clipboard.Length == 0)
				{
					// Check if there are any drop files?
					StringCollection files = Clipboard.GetFileDropList();
					// Create a clipboard string
					foreach(string file in files)
					{
						clipboard += file + "\r\n";
					}
				}
				if (string.IsNullOrEmpty(clipboard) == false)
				{
                    // CopyTo filename array
                    List<MOG_Filename> copyToFilenames = new List<MOG_Filename>();

					// Split any multiples
					string[] paths = clipboard.Split("\n".ToCharArray());
					foreach (string searchPath in paths)
					{
						// Trim off any \n and \r's
						string path = searchPath.Trim();
						if (mTree != null && path != null && path.Length > 0)
						{
							// Now lets try the fast find that uses the nodes key's.  Note this will only work if the nodes already exist or have been created
							TreeNode[] nodes = mTree.Nodes.Find(path, true);
							if (nodes != null && nodes.Length > 0)
							{
								// If we found some, then lets highlight them
								foreach (TreeNode node in nodes)
								{
									node.BackColor = SystemColors.Highlight;
									mTree.SelectedNode = node;
								}
							}
							else
							{
								if (mTree is MogControl_LibraryTreeView)
								{
									MogControl_LibraryTreeView libraryTree = mTree as MogControl_LibraryTreeView;
									TreeNode selectedFolder = libraryTree.SelectedNode;
									if (selectedFolder != null)
									{
										string sourceItem = path;
										string targetPath = MOG_ControllerLibrary.ConstructPathFromLibraryClassification(selectedFolder.FullPath);
										bDuplicated |= Paste(sourceItem, targetPath);
									}										
								}
								else
								{
									// Looks like this path might not have been expanded yet, We need to do the deep drill
									MogControl_BaseTreeView realBaseTree = mTree as MogControl_BaseTreeView;
									if (realBaseTree != null)
									{
										// We need to determine if this path is of an asset or a class
										MOG_Filename test = new MOG_Filename(path);
										if (test.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
										{
											// Ok, this is a filename asset, lets drill for it
											TreeNode node = realBaseTree.DrillToAssetName(path);
											if (node != null)
											{
												node.BackColor = SystemColors.Highlight;
												realBaseTree.SelectedNode = node;
											}
										}
										else
										{
											// This path must be a classification path, lets drill for it
											TreeNode node = realBaseTree.DrillToNodePath(path);
											if (node != null)
											{
												node.BackColor = SystemColors.Highlight;
												realBaseTree.SelectedNode = node;
											}
										}
									}
								}
							}
						}
                        // I'm trying to issolate this list view as the one in the Library tab not the workspace ones
                        if (mView != null && mView is ListView && mView.Tag != null)
                        {
                            if (path != null && path.Length > 0)
                            {
                                // Get the currently selected classification of this listview
                                string currentListViewClassification = mView.Tag as string;
                                if (currentListViewClassification != null)
                                {
                                    string sourceItem = path;
                                    string targetPath = MOG_ControllerLibrary.ConstructPathFromLibraryClassification(currentListViewClassification);
                                    bDuplicated |= Paste(sourceItem, targetPath);
                                }
                            }
                        }
                        // This list view must be the one in the workspace tab
                        else if (mView != null && mView is MogControl_ListView)
                        {
                            MOG_Filename temp = new MOG_Filename(path);
                            MOG_Filename fixedUpName = null;
                            if (temp.IsWithinRepository())
                            {
                                fixedUpName = temp;
                            }
                            else if (temp.GetVersionTimeStamp().Length > 0)
                            {
                                fixedUpName = MOG_ControllerRepository.GetAssetBlessedVersionPath(temp, temp.GetVersionTimeStamp());
                            }
                            else
                            {
                                fixedUpName = MOG_ControllerRepository.GetAssetBlessedVersionPath(temp, MOG_ControllerProject.GetAssetCurrentVersionTimeStamp(temp));
                            }

                            copyToFilenames.Add(fixedUpName);
                        }
					}

                    if (copyToFilenames.Count > 0)
                    {
                        List<object> args = new List<object>();
                        args.Add(copyToFilenames);
                        args.Add(MOG_ControllerProject.GetActiveUserName());
                        args.Add(MOG_ControllerProject.GetActiveUserName());
                        args.Add("");
                        args.Add("Copy");

                        ProgressDialog progress = new ProgressDialog("Preparing to copy", "Please wait while MOG prepares to copy the assets...", CopyTo_Worker, args, true);
                        Control sourceControl = SourceControl != null ? sourceControl = SourceControl : sourceControl = MogMainForm.MainApp.TopLevelControl;
                        progress.ShowDialog(sourceControl.TopLevelControl);
                    }

					if (bDuplicated)
					{
						MainMenuViewClass.MOGGlobalViewRefresh(MogMainForm.MainApp);
					}
				}
			}
			catch(Exception ex)
			{
				ex.ToString();
			}
		}		

		private void MenuItemLibraryEditFolder_Click(object sender, System.EventArgs e)
		{
			try
			{
				ArrayList selectedItems = ControlGetSelectedItemTags();
				foreach (Mog_BaseTag tag in selectedItems)
				{
					try
					{
						// Get our current classification name and project
						string oldClassification = ((TreeNode)tag.Owner).FullPath;
						MOG_Project project = MOG_ControllerProject.GetProject();
						string[] parts = MOG_Filename.SplitClassificationString(oldClassification);

						// Create a form for user input
						AddLibraryFolderWizard wiz = new AddLibraryFolderWizard();
						wiz.AddLibraryFolderName = parts[parts.Length - 1];
						if (wiz.ShowDialog(this.mTree) == DialogResult.OK)
						{
							// Add our classification
							parts[parts.Length - 1] = wiz.AddLibraryFolderName;
							string newClassification = MOG_Filename.JoinClassificationString(parts);

							// Rename the classification
							MOG_ControllerProject.GetProject().ClassificationRename(oldClassification, newClassification);
							MOG_Properties props = MOG_Properties.OpenClassificationProperties(newClassification);
							if (props != null)
							{
								// This is a library
								props.ClassIcon = wiz.AddLibraryFolderIcon;
								props.UnReferencedRevisionHistory = wiz.AddLibraryFolderMaxRevisions.ToString();
								props.Close();
							}
							//GLK:  What I really should be doing here is de-initializing any open treeViews (in Project Tab)...
							RefreshTreeView(tag);
						}
					}
					catch (InvalidCastException ex)
					{
						throw new Exception(ClassificationAddDelete_MessageText, ex);
					}
					catch (Exception ex)
					{
						throw new Exception("Problem finding valid TreeView. " + ClassificationAddDelete_MessageText, ex);
					}
				}
			}
			catch (Exception ex)
			{
				MOG_Report.ReportMessage(AddClassification_MenuItemText, ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
			}			
		}

		private void MenuItemLibraryCreateFolder_Click(object sender, System.EventArgs e)
		{
			try
			{
				ArrayList selectedItems = ControlGetSelectedItemTags();
				foreach (Mog_BaseTag tag in selectedItems)
				{
					try
					{
						// Get our current classification name and project
						string classification = MOG_ControllerLibrary.EnsureClassificationIsWithinLibrary(((TreeNode)tag.Owner).FullPath);
						MOG_Project project = MOG_ControllerProject.GetProject();

						// Create a form for user input
						AddLibraryFolderWizard wiz = new AddLibraryFolderWizard();
						if (wiz.ShowDialog(this.mTree) == DialogResult.OK)
						{
							// Add our classification
							string classificationName = classification + "~" + wiz.AddLibraryFolderName;
							if (!project.ClassificationExists(classificationName))
							{
								//Oh no there's not one!  No big deal, just create it.
								if (project.ClassificationAdd(classificationName))
								{
									MOG_Properties props = MOG_Properties.OpenClassificationProperties(classificationName);
									if (props != null)
									{
										// This is a library
										props.ClassIcon = wiz.AddLibraryFolderIcon;
										props.UnReferencedRevisionHistory = wiz.AddLibraryFolderMaxRevisions.ToString();
										props.Close();
									}
								}
							}
							//GLK:  What I really should be doing here is de-initializing any open treeViews (in Project Tab)...
							RefreshTreeView(tag);
						}
					}
					catch (InvalidCastException ex)
					{
						throw new Exception(ClassificationAddDelete_MessageText, ex);
					}
					catch (Exception ex)
					{
						throw new Exception("Problem finding valid TreeView. " + ClassificationAddDelete_MessageText, ex);
					}
				}
			}
			catch (Exception ex)
			{
				MOG_Report.ReportMessage(AddClassification_MenuItemText, ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
			}			
		}

		private void MenuItemLibraryDeleteFolder_Click(object sender, System.EventArgs e)
		{
			try
			{
				ArrayList selectedItems = ControlGetSelectedItemTags();
				foreach (Mog_BaseTag tag in selectedItems)
				{
					TreeNode owner = tag.Owner as TreeNode;
					if (owner != null)
					{
						// Are we a class?
						string classification = MOG_ControllerLibrary.EnsureClassificationIsWithinLibrary(owner.FullPath);

						if (MOG_ControllerProject.IsValidClassification(classification))
						{
							if (MOGPromptResult.Yes == MOG_Prompt.PromptResponse(LibraryDeleteFolder_MenuItemText, "Are you sure "
								+ "you wish to delete classification, '" + classification + "'?", MOGPromptButtons.YesNo))
							{
								bool bIsClassificationEmpty = true;

								ArrayList assets = MOG_DBAssetAPI.GetAllAssetsInClassificationTree(classification);
								if (assets.Count > 0)
								{
									bIsClassificationEmpty = false;

									string assetList = Environment.NewLine;

									foreach (MOG_Filename asset in assets)
									{
										assetList += Environment.NewLine + asset.GetAssetFullName();
									}

									//Ask the user if he wants to delete all the assets in this classification
									if (MOG_Prompt.PromptResponse(LibraryDeleteFolder_MenuItemText, "This classification contains dependant assets and cannot be deleted as long as they exist.  Would you like to delete all of the assets under this classification?" + assetList, MOGPromptButtons.YesNo) == MOGPromptResult.Yes)
									{
										//remove the dependant assets
										foreach (MOG_Filename asset in assets)
										{
											MOG_DBAssetAPI.RemoveAssetName(asset);
										}

										bIsClassificationEmpty = true;
									}
								}

								if (bIsClassificationEmpty)
								{
									bool successful = MOG_ControllerProject.GetProject().ClassificationRemove(classification);
									if (successful)
									{
										tag.ItemRemove();
									}
								}
							}
						}
						else
							// Nope, we are a folder
						{
							string folderPath = MOG_ControllerLibrary.GetWorkingDirectory() + "\\" + classification.Replace("~", "\\");
							if (MOGPromptResult.Yes == MOG_Prompt.PromptResponse(LibraryDeleteFolder_MenuItemText, "Are you sure "
								+ "you wish to delete folder, '" + folderPath + "'?", MOGPromptButtons.YesNo))
							{
								bool successful = DosUtils.DirectoryDeleteFast(folderPath);
								if (successful)
								{
									tag.ItemRemove();
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				MOG_Report.ReportMessage(LibraryDeleteFolder_MenuItemText, ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
			}
		}		

		private void MenuItemLibraryDeleteFile_Click(object sender, System.EventArgs e)
		{
			try
			{
				bool bDeleted = false;

				ArrayList selectedItems = ControlGetSelectedItemTags();
				foreach (Mog_BaseTag tag in selectedItems)
				{
					// Assuming this is a file, make sure its path is within the library
					if (tag.FullFilename.StartsWith(MOG_ControllerLibrary.GetWorkingDirectory(), StringComparison.CurrentCulture))
					{
						// Delete this local file
						if (DosUtils.FileDeleteFast(tag.FullFilename))
						{
							bDeleted = true;
						}
					}
				}

				// Check if we actually deleted somthing?
				if (bDeleted)
				{
					MainMenuViewClass.MOGGlobalViewRefresh(MogMainForm.MainApp);
				}
			}
			catch (Exception ex)
			{
				MOG_Report.ReportMessage(LibraryDeleteFile_MenuItemText, ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
			}
		}


		private bool Paste(string sourceItem, string targetPath)
		{
			string localSource = sourceItem;
			string localTarget = "";
			string repositorySource = "";

			// Check if the source is an asset?
			MOG_Filename sourceAsset = new MOG_Filename(sourceItem);
			if (sourceAsset.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
			{
				// Convert this asset to a local filename
				localSource = MOG_ControllerLibrary.ConstructLocalFilenameFromAssetName(sourceAsset);
				// Check if this local file has already been synced?
				if (!DosUtils.FileExistFast(localSource))
				{
					// Look to the repository for the source
					repositorySource = MOG_ControllerLibrary.ConstructBlessedFilenameFromAssetName(sourceAsset);
				}
			}

			// Build the new localTarget
			localTarget = Path.Combine(targetPath, Path.GetFileName(localSource));
			// Check if the localSource's path is the same as the targetPath?
			if (string.Compare(Path.GetDirectoryName(localSource), targetPath, true) == 0)
			{
				string tempName = DosUtils.PathGetFileNameWithoutExtension(localSource);
				string tempPath = Path.Combine(MOG_ControllerLibrary.GetWorkingDirectory(), DosUtils.PathGetDirectoryPath(localSource));
				string tempExtenstion = DosUtils.PathGetExtension(localSource);

				// Create a simple copy of this asset
				localTarget = tempPath + "\\" + "Copy of " + tempName + "." + tempExtenstion;
				// Check if this already exists?
				if (DosUtils.FileExistFast(localTarget))
				{
					// How many of these files do we have?
					int count = 1;
					FileInfo[] files = DosUtils.FileGetList(tempPath, "Copy (*) of " + tempName + "." + tempExtenstion);
					if (files != null && files.Length > 0)
					{
						count = files.Length + 1;
					}
					// Add the numerical indicator of the number of these files
					localTarget = tempPath + "\\" + "Copy (" + count.ToString() + ") of " + tempName + "." + tempExtenstion;
				}
			}

			string copySource = localSource;
			string copyTarget = localTarget;
			// Check if we are getting this from the repository?
			if (repositorySource.Length > 0)
			{
				copySource = repositorySource;
			}

			// Copy this local file
			if (DosUtils.CopyFast(copySource, copyTarget, true))
			{
				// Make sure we always clear the ReadOnly on this newly duplicated file
				DosUtils.SetAttributes(copyTarget, FileAttributes.Normal);
				return true;
			}

			return false;
		}


		private void MenuItemLibrarySetTemplate_Click(object sender, System.EventArgs e)
		{
			try
			{
				bool bDeleted = false;

				ArrayList selectedItems = ControlGetSelectedItemTags();
				foreach (Mog_BaseTag tag in selectedItems)
				{
					// Assuming this is a file, make sure its path is within the library
					if (tag.FullFilename.StartsWith(MOG_ControllerLibrary.GetWorkingDirectory(), StringComparison.CurrentCulture))
					{
						// Delete this local file
						if (DosUtils.FileDeleteFast(tag.FullFilename))
						{
							bDeleted = true;
						}
					}
				}

				// Check if we actually deleted somthing?
				if (bDeleted)
				{
					MainMenuViewClass.MOGGlobalViewRefresh(MogMainForm.MainApp);
				}
			}
			catch (Exception ex)
			{
				MOG_Report.ReportMessage(LibraryDeleteFile_MenuItemText, ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
			}
		}
		

		private void MenuItemLibraryExportInbox_Click(object sender, System.EventArgs e)
		{
			ArrayList selectedItems = ControlGetSelectedItemTags();
			ArrayList exportFiles = new ArrayList();

			try
			{
				ArrayList completeAssetList = new ArrayList();
				foreach (Mog_BaseTag tag in selectedItems)
				{
					// Check if this is a valid asset?
					MOG_Filename testFilename = new MOG_Filename(tag.FullFilename);
					if (testFilename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
					{
						// Add this single assset
						completeAssetList.Add(testFilename);
					}
					// Check if this is a classification?
					else if (MOG_ControllerProject.IsValidClassification(((TreeNode)tag.Owner).FullPath))
					{
						// Obtain all the assets listed within this classification
						completeAssetList.AddRange(MOG_DBAssetAPI.GetAllAssetsInClassificationTree(((TreeNode)tag.Owner).FullPath));
					}
				}
					
				// Check if we built an asset list for processing?
				if (completeAssetList.Count > 0)
				{
					// Scan the assetList and determin who might need to be synced
					ArrayList syncList = new ArrayList();
					foreach (MOG_Filename filename in completeAssetList)
					{
						// Only sync files that I don't have a lock on
				        if (!MOG_ControllerProject.IsLockedByMe(filename.GetAssetFullName()))
				        {
							syncList.Add(filename);
				        }
					}
					// GetLatest us to the latest version of these assets
					MOG_ControllerLibrary.GetLatest(syncList);

					// Scan the assetList to make sure the local file exists
					foreach (MOG_Filename filename in completeAssetList)
					{
						// Make sure the local file exists
						string importFile = MOG_ControllerLibrary.ConstructLocalFilenameFromAssetName(filename);
						if (DosUtils.FileExistFast(importFile))
						{
							exportFiles.Add(importFile);
						}
					}
				}

				// Do the Export
				if (exportFiles.Count > 0)
				{
					// Create our export string array
					string []sourceExportAssets = new string[exportFiles.Count];
					for (int j = 0; j < exportFiles.Count; j++)
					{
						sourceExportAssets[j] = exportFiles[j] as string;
					}
							
					// Import the binary using our standard import routines
					guiAssetController.ImportSeparately(sourceExportAssets, false);
				}
			}
			catch(Exception ex)
			{
				MOG_Report.ReportMessage("Import error", ex.Message, ex.StackTrace, MOG_ALERT_LEVEL.CRITICAL);
			}
		}		

		/// <summary>
		/// Export out the target binaries of a group of assets to a user specified target
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MenuItemExportBinaries_Click(object sender, System.EventArgs e)
		{
			// TODO - Remove MogMainForm!
			//			ArrayList selectedItems = ControlGetSelectedItemTags();
			//
			//			ArrayList messageAssets = new ArrayList();
			//			foreach (Mog_BaseTag tag in selectedItems)
			//			{				
			//				if (tag.Execute)
			//				{
			//					messageAssets.Add(tag.FullFilename);
			//				}
			//			}
			//
			//			if (guiConfirmDialog.MessageBoxDialog(ExportBinaries_MenuItemText, "Are you sure you want to export all of these assets from the game?", messageAssets, MOGPromptButtons.OKCancel) == DialogResult.OK)
			//			{
			//				// Get the target directory
			//				string targetPath = "c:\\";
			//				mainForm.MOGFolderBrowserDialog.SelectedPath = targetPath;
			//				
			//				// show the window
			//				if (mainForm.MOGFolderBrowserDialog.ShowDialog() == DialogResult.OK)
			//				{
			//					targetPath = mainForm.MOGFolderBrowserDialog.SelectedPath;
			//
			//					CallbackDialogForm progress = new CallbackDialogForm();
			//					progress.DialogInitialize("Export Binary", string.Concat("Exporting Binary(s):\n") , "Cancel");
			//				
			//					int f = 0;
			//					int total = guiConfirmDialog.SelectedItems.Count;
			//					bool canceled = false;
			//
			//					try
			//					{
			//						foreach (string file in guiConfirmDialog.SelectedItems)
			//						{	
			//							MOG_Filename filename = new MOG_Filename(file);
			//					
			//								// Make sure we are an asset before showing log
			//							if (filename.GetType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
			//							{
			//								// Check to see if this is a blesses asset
			//								filename = FixupAssetPath("Export Binary", filename));
			//
			//								guiAssetController asset = new guiAssetController(mainForm);
			//
			//								ArrayList binaries = asset.LocateAssetBinary(filename);
			//
			//								// Copy to the target directory
			//								foreach (string binary in binaries)
			//								{
			//									progress.DialogUpdate((f++ * 100) / total, filename.GetAssetName());
			//									Application.DoEvents();
			//									
			//									// Check for cancel
			//									canceled = progress.DialogProcess();
			//									if (canceled)
			//									{
			//										throw new Exception();
			//									}
			//
			//								CheckFile:
			//									if (DosUtils.FileExist(targetPath + "\\" + Path.GetFileName(binary)))
			//									{
			//										switch (MOG_Prompt.PromptResponse("Export Binary", "File(" + targetPath + "\\" + Path.GetFileName(binary) + ")\nAlready exists!  Ignore and Overwrite?", MOGPromptButtons.AbortRetryIgnore))
			//										{
			//											case MOGPromptResult.Abort:
			//												canceled = true;
			//												throw new Exception();
			//											case MOGPromptResult.Retry:
			//												goto CheckFile;
			//											case MOGPromptResult.Ignore:
			//												break;
			//										}
			//									}
			//
			//									if (!DosUtils.FileCopy(binary, targetPath + "\\" + Path.GetFileName(binary)))
			//									{
			//										// Error
			//										throw new Exception("Could not copy file(" + binary + ")\nAborting...");
			//									}
			//								}								
			//							}
			//						}
			//					}
			//					catch(Exception	ex)
			//					{
			//						if (!canceled)
			//						{
			//							MOG_Report.ReportMessage("Export Binary", ex.Message, MOGPromptButtons.OK);
			//						}
			//					}
			//
			//					finally
			//					{
			//						progress.DialogKill();
			//					}					
			//				}
			//			}
		}

		/// <summary>
		/// Create a new package container in the repository
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MenuItemCreatePackage_Click(object sender, System.EventArgs e)
		{
			try
			{
				ArrayList selectedItems = ControlGetSelectedItemTags();

				string message = "";
				foreach (Mog_BaseTag tag in selectedItems)
				{				
					if (tag.Execute)
					{
						message = tag.FullFilename;
					}
				}
			
				CreateNewPackageForm importDialog = new CreateNewPackageForm();
				if (importDialog.ShowDialog() == DialogResult.OK)
				{

				}
			}	
			catch(Exception ex)
			{
				MOG_Report.ReportMessage(CreatePackage_MenuItemText, ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
			}
		}

		/// <summary>
		/// Handle a custom tool click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MenuItemCustomTools_Click(object sender, System.EventArgs e)
		{
			ArrayList selectedItems = ControlGetSelectedItemTags();
			ToolStripMenuItem tool = (ToolStripMenuItem)sender;
			ToolStripMenuItem toolGroup = (ToolStripMenuItem)tool.OwnerItem;

			foreach (Mog_BaseTag tag in selectedItems)
			{				
				if (tag.Execute)
				{
					if (ExecucteCustomToolWindow(toolGroup.Text, tool.Text, tag))
					{
						MOG_Prompt.PromptResponse("Custom Tool", toolGroup.Text + " ran successfully");
					}
				}
			}
		}

		private void MenuItemGroup_Click(object sender, System.EventArgs e)
		{
			// TODO - Remove MogMainForm
			//			ArrayList selectedItems = ControlGetSelectedItemTags();
			//
			//			guiAssetController asset = new guiAssetController(mainForm);
			//			string groupName = Microsoft.VisualBasic.Interaction.InputBox("Enter new group name", Group_MenuItemText, "NewGroupName", mainForm.Location.X + mainForm.Width/2, mainForm.Location.Y + mainForm.Height/2);
			//					
			//			foreach (Mog_BaseTag tag in selectedItems)
			//			{				
			//				if (tag.Execute)
			//				{
			//					MOG_Filename filename = new MOG_Filename(tag.FullFilename);
			//					
			//					// Make sure we are an asset before showing log
			//					if (filename.GetType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
			//					{
			//						// Kier - We should build the groupFilename once outside of this foreach!!!
			//						MOG_Filename groupFilename = new MOG_Filename(filename.GetPath() + "\\" + groupName);
			//
			//						asset.GroupAsset(groupFilename.GetOriginalFilename(), filename.GetOriginalFilename());
			//					}					
			//				}
			//			}
		}

		private void MenuItemRenameGroup_Click(object sender, System.EventArgs e)
		{
		}

		private void MenuItemUnGroup_Click(object sender, System.EventArgs e)
		{
		}

		private void MenuItemReject_Click( object sender, EventArgs e)
		{
			// Get our tags
			ArrayList selectedItems = ControlGetSelectedItemTags();
			// Store our work
			ArrayList filenames = new ArrayList();
			// Store our message to the user
			string rejectMessage = "Are you sure you would like to reject the following Assets back to " 
				+ "their respective owner(s)?:";

			// Make a list of what we will be rejecting, storing our results...
			foreach( Mog_BaseTag tag in selectedItems )
			{
				MOG_Filename filename = new MOG_Filename( tag.FullFilename );
				rejectMessage += "\r\n" + filename.GetAssetFullName();
				filenames.Add( filename );
			}

			// Ask the user if they want to proceed...
			if( MOGPromptResult.Yes == MOG_Prompt.PromptResponse( "Reject", rejectMessage, MOGPromptButtons.YesNo))
			{
				// Reject each asset
				foreach( MOG_Filename filename in filenames )
				{
					MOG_Properties properties = new MOG_Properties( filename );
					string userName = MOG_ControllerProject.GetUserName();
					MOG_ControllerInbox.Reject(filename, "Rejected by User '" + userName + "'", null);
				}
			}
		}

		private void MenuItemLock_Click(object sender, EventArgs e)
		{
			try
			{
				//string globalComment = "";
				ArrayList selectedItems = ControlGetSelectedItemTags();
				
				//// Resolve group comments
				//ArrayList selectedFilenameItems = new ArrayList();
				//foreach (Mog_BaseTag tag in selectedItems)
				//{
				//    selectedFilenameItems.Add(new MOG_Filename(tag.FullFilename));
				//}

				string comment = "";
				//bool maintainLock = false;
				//ArrayList selectedItemsWithComments = new ArrayList();
				//ArrayList selectedItemsNoComments = new ArrayList();

				//GetCommentsForList(CommentType.Lock, ref comment, ref maintainLock, selectedFilenameItems, selectedItemsWithComments, selectedItemsNoComments, false, false);

				foreach (Mog_BaseTag tag in selectedItems)
				{
					string lockItem = "";

					TreeNode lockNode = null;

					if (tag.Owner is TreeNode)
					{
						lockNode = tag.Owner as TreeNode;

						// Attempt to obtain the lockItem
						// Check if this is an asset?
						MOG_Filename assetFilename = new MOG_Filename(tag.FullFilename);
						if (assetFilename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
						{
							// Set lockItem to the fullname of the asset
							lockItem = assetFilename.GetAssetFullName();

							// Get the comment for this asset, and break out if the user cancels the comment form
							if (GetComment(CommentType.Lock, ref comment, assetFilename, false, false) == false)
							{
								break;
							}
						}
						else
						{
							// Build lockItem from the node's full path
							lockItem = lockNode.FullPath;
						}
					}
					else
					{
						lockItem = tag.FullFilename;
					}

					MOG_Filename filename = new MOG_Filename(lockItem);
					bool bSuccess = false;
					bool bClassificationLock = false;

					// Make sure we are an asset before showing log
					if (filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
					{
						if (GetComment(CommentType.Lock, ref comment, filename, false, false))
						{
							bSuccess = guiAssetSourceLock.RequestPersistentLockWithRetry(filename.GetAssetFullName(), comment);
						}
						else
						{
							return;
						}
					}
					else
					{
						bClassificationLock = true;

						// Just use whatever was sent in plus add a '*'
						string lockName = filename.GetOriginalFilename();
						// Check if this is a valid classification?
						if (MOG_Filename.IsClassificationValidForProject(lockName, MOG_ControllerProject.GetProjectName()))
						{
							// Add on a '*' so that we will cause the entire tree to be locked
							lockName += "*";
						}


						if (GetComment(CommentType.Lock, ref comment, lockName, false, false))
						{
							bSuccess = guiAssetSourceLock.RequestPersistentLockWithRetry(lockName, comment);							
						}
						else
						{
							return;
						}						
					}

					// Update the treeNode icon
					if (bSuccess)
					{
						if (lockNode != null)
						{
							lockNode.ImageIndex = MogUtil_AssetIcons.GetLockedIcon(lockItem, MogUtil_AssetIcons.IconType.CLASS, null);
							lockNode.SelectedImageIndex = lockNode.ImageIndex;

							if (bClassificationLock)
							{
								RefreshTreeView(tag);
							}
						}

						RefreshOwningMenu(sender as ToolStripMenuItem);
					}
				}
			}
			catch (Exception ex)
			{
				MOG_Report.ReportMessage(Lock_MenuItemText, ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
			}
		}

		internal bool GetComment(CommentType commentType, ref string comment, string assetName, bool enableEditCheckbox, bool enableMaintainLock)
		{
			// Get the properties for classification
			MOG_Properties assetsProps = new MOG_Properties(assetName.TrimEnd("*".ToCharArray()));

			return CommentCheckProperty(commentType, ref comment, enableEditCheckbox, enableMaintainLock, assetsProps);
		}

		internal bool GetComment(CommentType commentType, ref string comment, MOG_Filename assetName, bool enableEditCheckbox, bool enableMaintainLock)
		{
			// Get the properties for assetVerifedName
			MOG_Properties assetsProps = new MOG_Properties(assetName);

			return CommentCheckProperty(commentType, ref comment, enableEditCheckbox, enableMaintainLock, assetsProps);
		}

		private static bool CommentCheckProperty(CommentType commentType, ref string comment, bool enableEditCheckbox, bool enableMaintainLock, MOG_Properties properties)
		{
			bool show = false;
			switch (commentType)
			{
				case CommentType.Lock:
				case CommentType.Checkout:
					show = properties.RequireLockComment;
					break;
				case CommentType.Checkin:
					show = properties.ShowPostLockComment;
					break;
			}

			// Do we show the PreComment form?
			if (show)
			{
				// Show the comment window
				AddCommentForm comments = new AddCommentForm(commentType, enableEditCheckbox, enableMaintainLock, properties.MaintainLock);
				if (comments.ShowDialog() == DialogResult.OK)
				{
					// Get the comments
					comment = comments.CommentsRichTextBox.Text;
				}
				else
				{
					return false;
				}
			}

			return true;
		}		

		internal void GetCommentsForList(CommentType commentType, ref string comment, ref bool maintainLock, ArrayList files, ArrayList filesWithComments, ArrayList filesWithNoComments, bool enableEditCheckbox, bool enableMaintainLock)
		{
			bool checkMaintainLock = false;

			foreach (MOG_Filename assetName in files)
			{
				// Get the properties for assetVerifedName
				MOG_Properties assetsProps = new MOG_Properties(assetName);

				bool addToCommentsList = false;
				switch (commentType)
				{
					case CommentType.Lock:
					case CommentType.Checkout:
						addToCommentsList = assetsProps.RequireLockComment;
						break;
					case CommentType.Checkin:
						addToCommentsList = assetsProps.ShowPostLockComment;
						break;
				}

				// Do we show the PreComment form?
				if (addToCommentsList)
				{
					filesWithComments.Add(assetName);
				}
				else
				{
					filesWithNoComments.Add(assetName);
				}

				// Should force check maintainLock?
				checkMaintainLock = assetsProps.MaintainLock;
			}

			if (filesWithComments.Count > 0)
			{
				AddCommentForm comments = new AddCommentForm(commentType, enableEditCheckbox, enableMaintainLock, checkMaintainLock);
				if (comments.ShowDialog() == DialogResult.OK)
				{
					comment = comments.CommentsRichTextBox.Text;
					maintainLock = comments.MaintainLock;
				}
				else
				{
					// The user has canceled the checkin by canceling the comments window
					filesWithComments.Clear();
				}
			}			
		}

		private void MenuItemUnLock_Click(object sender, EventArgs e)
		{
			try
			{
				ArrayList selectedItems = ControlGetSelectedItemTags();
					
				foreach (Mog_BaseTag tag in selectedItems)
				{				
					string lockItem = "";
					TreeNode lockNode = null;

					if (tag.Owner is TreeNode)
					{
						lockNode = tag.Owner as TreeNode;

						// Attempt to obtain the lockItem
						// Check if this is an asset?
						MOG_Filename assetFilename = new MOG_Filename(tag.FullFilename);
						if (assetFilename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
						{
							// Set lockItem to the fullname of the asset
							lockItem = assetFilename.GetAssetFullName();
						}
						else
						{
							// Build lockItem from the node's full path
							lockItem = lockNode.FullPath;
						}
					}
					else
					{
						lockItem = tag.FullFilename;
					}

					MOG_Filename filename = new MOG_Filename(lockItem);
					bool bSuccess = false;
					bool bClassificationLock = false;

					// Make sure we are an asset before showing log
					if (filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)					
					{												
						bSuccess = guiAssetSourceLock.ReleasePersistentLockWithRetry(filename.GetAssetFullName());						
					}
					else
					{
						bClassificationLock = true;

						// Just use whatever was sent in plus add a '*'
						string lockName = filename.GetOriginalFilename();
						// Check if this is a valid classification?
						if (MOG_Filename.IsClassificationValidForProject(lockName, MOG_ControllerProject.GetProjectName()))
						{
							// Add on a '*' so that we will cause the entire tree to be locked
							lockName += "*";
						}

						bSuccess = guiAssetSourceLock.ReleasePersistentLockWithRetry(lockName);						
					}

					// Update the treeNode icon
					if (bSuccess)
					{
						if (lockNode != null)
						{
							lockNode.ImageIndex = MogUtil_AssetIcons.GetAssetIconIndex(lockItem, null, false);
							lockNode.SelectedImageIndex = lockNode.ImageIndex;

							// Classification locks affect more than just one node, so refresh the whole tree
							if (bClassificationLock)
							{
								RefreshTreeView(tag);
							}
						}

						RefreshOwningMenu(sender as ToolStripMenuItem);
					}
				}
			}
			catch(Exception ex)
			{
				MOG_Report.ReportMessage(UnLock_MenuItemText, ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
			}
		}

		private void MenuItemKillLock_Click(object sender, System.EventArgs e)
		{
			try
			{
				ArrayList selectedItems = ControlGetSelectedItemTags();
					
				foreach (Mog_BaseTag tag in selectedItems)
				{				
					if (tag.Execute)
					{
						ListViewItem lockItem = tag.Owner as ListViewItem;

						string commandType = lockItem.SubItems[ColumnNameFind(mColumns, "Type")].Text;
						string assetName = lockItem.SubItems[ColumnNameFind(mColumns, "FullName")].Text;
						string userName = lockItem.SubItems[ColumnNameFind(mColumns, "User")].Text;
						string computerName = lockItem.SubItems[ColumnNameFind(mColumns, "Machine")].Text;
					
						// Make sure we are an asset before showing log
						MOG_ControllerSystem.LockKill(assetName, commandType, userName, computerName);
					}
				}
			}
			catch(Exception ex)
			{
				MOG_Report.ReportMessage(UnLock_MenuItemText, ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
			}
		}

		/// <summary>
		/// Package management opens a special form for adding and removing an asset to a package
		/// </summary>
		private void MenuItemPackageManagement_Click(object sender, EventArgs e)
		{
			ArrayList selectedItems = ControlGetSelectedItemTags();
			ArrayList fixedItems = new ArrayList();

			ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
			if (menuItem != null)
			{
				try
				{
					bool hasBlessedAsset = false;
					foreach (Mog_BaseTag tag in selectedItems)
					{
						// If we are an executable tag...
						if (tag.Execute)
						{
							MOG_Filename filename = new MOG_Filename(tag.FullFilename);
							filename = FixupAssetPath(PackageManagement_MenuItemText, filename);
							// Document whether or not we are dealing with any blessed assets
							hasBlessedAsset |= filename.IsBlessed();
							fixedItems.Add(filename);
						}
						// Else, if we have a TreeNode and it has a TreeView it's attached to (should always be true)
						else if (tag.Owner is TreeNode && ((TreeNode)tag.Owner).TreeView != null)
						{
							string fullPath = ((TreeNode)tag.Owner).FullPath;
							MOG_Filename filename = new MOG_Filename(fullPath);
							hasBlessedAsset |= filename.IsBlessed();
							fixedItems.Add(filename);
						}

					}

					if (fixedItems.Count > 0)
					{
						if (hasBlessedAsset)
						{
							if (DialogResult.Yes != MessageBox.Show(SourceControl,
								"Please note that you will only be able to view \r\n"
								+ "package assignments for this Asset.\r\n\r\n"
								+ "In order to change those assignments, please\r\n"
								+ "copy the Asset to your Inbox, change it, then\r\n"
								+ "re-bless it.\r\n\r\n"
								+ "Continue?", "Blessed Asset Properties are Read Only!  Continue?",
								MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information))
							{
								return;
							}
						}

						ToolStripMenuItem command = sender as ToolStripMenuItem;
						if (command.Text.IndexOf("Setup") != -1)
						{
							MenuItemPackageSetup(fixedItems, hasBlessedAsset);
							return;
						}

						// Launch the Form
						PackageManagementForm form = new PackageManagementForm(fixedItems, hasBlessedAsset);
						Control sourceControl = SourceControl != null ? sourceControl = SourceControl : sourceControl = MogMainForm.MainApp.TopLevelControl;
						form.ShowDialog(sourceControl.TopLevelControl);
					}
				}
				catch (Exception ex)
				{
					MOG_Report.ReportMessage("PackageManagement", ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
				}
			}
		}

		private void MenuItemPackageSetup(ArrayList fixedItems, bool hasBlessedAsset)
		{
			try
			{
				ArrayList selectedItems = ControlGetSelectedItemTags();

				SetupPackagingWizard wiz = new SetupPackagingWizard();

				wiz.mHasBlessedAsset = hasBlessedAsset;
				wiz.assetNames = fixedItems;

				// Open the wizard
				wiz.ShowDialog(MogMainForm.MainApp);
				
			}
			catch(Exception ex)
			{
				MOG_Report.ReportMessage(Properties_MenuItemText, ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
			}
		}


		/// <summary>
		/// Bless an asset into the project
		/// </summary>
		private void MenuItemBless_Click(object sender, System.EventArgs e)
		{
			ArrayList selectedItems = ControlGetSelectedItemTags();
			try
			{
				if (selectedItems.Count > 0)
				{
					List<MOG_Filename> filenames = new List<MOG_Filename>();
					string comment = "";
      
					// Get the comment for this asset or set of assets
					BlessInfoForm bless = new BlessInfoForm(selectedItems, true);

					// Show BlessDialog as a modal dialog and determine if DialogResult = OK.
					if (bless.ShowDialog() == DialogResult.OK)
					{
						// Get the comment file from the control by copying each line into our string
						foreach(string line in bless.BlessInfoBlessCommentRichTextBox.Lines)
						{
							comment = String.Concat(comment, "\n", line);
						}
				
						foreach (MOG_Filename filename in bless.MOGSelectedBlessFiles)
						{
							// Make sure we are an asset before showing log
							if (filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
							{
								// Place code here
								filenames.Add(filename);
							}						
						}

						ToolStripMenuItem menuItem = sender as ToolStripMenuItem;

						List<object> args = new List<object>();
						args.Add(filenames);
						args.Add(comment);
						args.Add(bless.MaintainLock);
						ProgressDialog progress = new ProgressDialog("Blessing assets", "Please wait while MOG blesses the assets...", guiAssetController.BlessAssets_Worker, args, true);
						Control sourceControl = SourceControl != null ? sourceControl = SourceControl : sourceControl = MogMainForm.MainApp.TopLevelControl;
						progress.ShowDialog(sourceControl.TopLevelControl);						
					}
				}
			}
			catch(Exception ex)
			{
				MOG_Report.ReportMessage("Bless", ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
			}
		}

		/// <summary>
		/// Process an asset
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MenuItemProcess_Click(object sender, System.EventArgs e)
		{
			ToolStripMenuItem command = sender as ToolStripMenuItem;
			if (command.Text.IndexOf("Setup") != -1)
			{
				MenuItemRipSetup();
				return;
			}

			ArrayList selectedItems = ControlGetSelectedItemTags();
			List<string> toBeProcessed = new List<string>();
			MOGAssetStatus assetStatus = new MOGAssetStatus();

			try
			{
				// JohnRen - This doesn't seem to be working any more.  Its like the list never redraws.
				foreach (Mog_BaseTag tag in selectedItems)
				{
					if (tag.Execute)
					{
						MOG_Filename filename = new MOG_Filename(tag.FullFilename);

						// Make sure we are an asset before showing log
						if (filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
						{
							ListViewItem item = (ListViewItem)tag.Owner;
							// Set the current state image 						
							item.StateImageIndex = MogMainForm.MainApp.mAssetManager.mAssetStatus.GetStatusInfo(MOG_AssetStatus.GetText(MOG_AssetStatusType.Pending)).IconIndex;
							item.SubItems[(int)guiAssetManager.AssetBoxColumns.STATE].Text = MOG_AssetStatus.GetText(MOG_AssetStatusType.Pending);
							item.ForeColor = MOG_AssetStatus.GetColor(MOG_AssetStatusType.Pending);

							toBeProcessed.Add(filename.GetOriginalFilename());

							//mainForm.mSoundManager.PlayStatusSound("AssetEvents", MOG_AssetStatus.GetText(MOG_AssetStatusType.Pending));
						}
					}
				}

				Thread myProcess = new Thread(new ParameterizedThreadStart(guiAssetController.ProcessAssets));
				myProcess.Name = "AssetContextMenu::ProcessAssets";
				myProcess.Start(toBeProcessed);
			}
			catch(Exception ex)
			{
				MOG_Report.ReportMessage(Process_MenuItemText, ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
			}
		}

		private void MenuItemRipSetup()
		{
			try
			{
				ArrayList selectedItems = ControlGetSelectedItemTags();

				// Create our wizard
				SetupRippingWizard wiz = new SetupRippingWizard();

				foreach (Mog_BaseTag tag in selectedItems)
				{
					if (tag.Execute)
					{
						MOG_Filename filename = new MOG_Filename(tag.FullFilename);

						// Make sure we are an asset before showing log
						if (filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
						{
							// Add the filename for this asset
							wiz.assetNames.Add(filename);

							// Open the actual properties file 
							MOG_Properties properties = MOG_Properties.OpenFileProperties(MOG_ControllerAsset.GetAssetPropertiesFilename(filename));
							if (properties != null)
							{
								// Add the properties for this asset
								wiz.PropertiesList.Add(properties);
							}
						}
					}
				}

				// Open the wizard
				wiz.ShowDialog(MogMainForm.MainApp);

				// Close the properties
				foreach (MOG_Properties newProperties in wiz.PropertiesList)
				{
					newProperties.Close();
				}
			}
			catch(Exception ex)
			{
				MOG_Report.ReportMessage(Properties_MenuItemText, ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
			}
		}

		/// <summary>
		/// Remove asset from current list
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MenuItemRemove_Click(object sender, System.EventArgs e)
		{
			ArrayList selectedItems = ControlGetSelectedItemTags();

			try
			{
				mView.Cursor = Cursors.WaitCursor;
				mView.BeginUpdate();

				foreach (Mog_BaseTag tag in selectedItems)
				{
					if (tag.Execute)
					{
						tag.ItemRemove();
					}
				}

				mView.EndUpdate();
				mView.Cursor = Cursors.Default;
			}
			catch(Exception ex)
			{
				MOG_Report.ReportMessage(Remove_MenuItemText, ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
			}
		}

			/// <summary>
		/// Remove asset from current list
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RemoveUpdatedFromList_Click(object sender, System.EventArgs e)
		{
			ArrayList selectedItems = ControlGetSelectedItemTags();

			try
			{
				foreach (Mog_BaseTag tag in selectedItems)
				{
					if (tag.Execute)
					{
						MOG_Filename filename = new MOG_Filename(tag.FullFilename);
						MOG_ControllerInbox.Delete(filename);
					}
				}
			}
			catch(Exception ex)
			{
				MOG_Report.ReportMessage(Remove_MenuItemText, ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
			}
		}
		/// <summary>
		/// Restore the item in the trash to users inbox
		/// </summary>
		private void MenuItemRestoreTrash_Click(object sender, System.EventArgs e)
		{
			// Get all the selected items into an ArrayList
			ArrayList selectedItems = ControlGetSelectedItemTags();

			List<Mog_BaseTag> tags = new List<Mog_BaseTag>();

			foreach (Mog_BaseTag tag in selectedItems)
			{
				if (tag.Execute)
				{
					tags.Add(tag);
				}
			}

			ProgressDialog progress = new ProgressDialog("Restoring Assets...", "Please wait while MOG restores the assets.", RestoreTrash_Worker, tags, true);
			progress.ShowDialog();
		}

		private void RestoreTrash_Worker(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			List<Mog_BaseTag> tags = e.Argument as List<Mog_BaseTag>;

			for (int i = 0; i < tags.Count && !worker.CancellationPending; i++)
			{
				Mog_BaseTag tag = tags[i];
				MOG_Filename filename = new MOG_Filename(tag.FullFilename);

				string message = "Restoring:\n" +
								 "     " + filename.GetAssetClassification() + "\n" +
								 "     " + filename.GetAssetName();
				worker.ReportProgress(i * 100 / tags.Count, message);
				
				MOG_ControllerInbox.RestoreAsset(filename);
			}
		}

		/// <summary>
		/// Delete an asset from an inbox
		/// </summary>
		private void MenuItemDelete_Click(object sender, System.EventArgs e)
		{
			try
			{
				ArrayList selectedItems = ControlGetSelectedItemTags();
				ArrayList assetList = new ArrayList();
				ArrayList assetLabelsList = new ArrayList();

				string message = "";				
				foreach (Mog_BaseTag tag in selectedItems)
				{				
					if (tag.Execute)
					{
						message = message + tag.FullFilename + "\n";

						// Add asset to our list to be deleted
						assetList.Add(tag.FullFilename);

						// Create a custom label for the confirm dialog
						MOG_Filename assetLabel = new MOG_Filename(tag.FullFilename);
						string subLabel = "";
						string label = "";

						// Get the asset timestamp
						MOG_Time time = new MOG_Time();
						if (assetLabel.GetDeletedTimeStamp().Length != 0)
						{
							time.SetTimeStamp(assetLabel.GetDeletedTimeStamp());
							subLabel = time.FormatString("");
							label = assetLabel.GetAssetName() + "?" + subLabel;
						}
						else if (assetLabel.GetVersionTimeStamp().Length != 0)
						{
							time.SetTimeStamp(assetLabel.GetVersionTimeStamp());
							subLabel = time.FormatString("");
							label = assetLabel.GetAssetName() + "?" + subLabel;
						}
						else
						{
							label = assetLabel.GetAssetFullName();
						}						

						// Add to the list
						assetLabelsList.Add(label);
					}
				}

				// Make sure we actually have something selected?
				if (assetLabelsList.Count > 0)
				{
					if (guiConfirmDialog.MessageBoxDialog("Delete Asset", "Are you sure you want to delete all of these asset(s)?", "", assetList, assetLabelsList, "?", MessageBoxButtons.OKCancel) == DialogResult.OK)
					{
						List<MOG_Filename> filenames = new List<MOG_Filename>();

						foreach (string fullFileName in guiConfirmDialog.SelectedItems)
						{
							filenames.Add(new MOG_Filename(fullFileName));
						}

						ProgressDialog progress = new ProgressDialog("Deleting Asset(s)", "Please wait while MOG deletes these assets.", Delete_Worker, filenames, true);
						progress.ShowDialog();
					}
				}
			}
			catch(Exception ex)
			{
				MOG_Report.ReportMessage(Delete_MenuItemText, ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
			}
		}

		private void Delete_Worker(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			List<MOG_Filename> filenames = e.Argument as List<MOG_Filename>;

			guiAssetController asset = new guiAssetController();

			bool bYesToAll = false;
			bool bNoToAll = false;
			for (int i = 0; i < filenames.Count && !worker.CancellationPending; i++)
			{
				MOG_Filename filename = filenames[i];
				string message = "Deleting:\n" +
								 "     " + filename.GetAssetClassification() + "\n" +
								 "     " + filename.GetAssetName();
				worker.ReportProgress((i * 100) / filenames.Count, message); 
				
				asset.Remove(filename, worker);

				// Check if this filename is within the drafts or inbox?
				if (filename.IsDrafts() ||
					filename.IsInbox())
				{
					// Attempt to obtain Lock information
					MOG_Command lockInfo = MOG_ControllerProject.PersistentLock_Query(filename.GetAssetFullName());
					if (lockInfo != null)
					{
						// Now check if someone had a lock on this asset
						if (lockInfo.IsCompleted() &&
							lockInfo.GetCommand() != null)
						{
							// Check if this asset is being deleted from the lock owner's own inbox?
							if (string.Compare(lockInfo.GetCommand().GetUserName(), filename.GetUserName(), true) == 0)
							{
								bool bDeleteLock = false;

								// Check if the user has already informe dus what to do?
								if (bYesToAll)
								{
									bDeleteLock = true;
								}
								else if (bNoToAll)
								{
								}
								else
								{
									// Warn the user that this asset is locked
									MOGPromptResult result = MOG_Prompt.PromptResponse("Lock Management", "You are deleting a locked asset.  Do you want to delete the lock as well?", MOGPromptButtons.YesNoYesToAllNoToAll);
									switch (result)
									{
										case MOGPromptResult.Yes:
											bDeleteLock = true;
											break;
										case MOGPromptResult.YesToAll:
											bYesToAll = true;
											bDeleteLock = true;
											break;
										case MOGPromptResult.NoToAll:
											bNoToAll = true;
											break;
									}
								}

								// Check if we should delete the lock?
								if (bDeleteLock)
								{
									// Remove the lock using LockKill just in case we aren't the lock owner
									MOG_ControllerSystem.LockKill(lockInfo.GetCommand());
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Remove file from updated window
		/// </summary>
		private void MenuItemRemoveUpdated_Click(object sender, System.EventArgs e)
		{
			try
			{
				ArrayList selectedItems = ControlGetSelectedItemTags();
				List<Mog_BaseTag> tags = new List<Mog_BaseTag>();

				foreach (Mog_BaseTag tag in selectedItems)
				{
					if (tag.Execute)
					{
						tags.Add(tag);
					}
				}

				ProgressDialog progress = new ProgressDialog("Removing asset from local workspace", "Please wait while the asset is removed from the local workspace.", RemoveUpdated_Worker, tags, true);
				progress.ShowDialog();
			}
			catch(Exception ex)
			{
				MOG_Report.ReportMessage(RevertUpdated_MenuItemText, ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
			}
		}

		private void RemoveUpdated_Worker(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			List<Mog_BaseTag> tags = e.Argument as List<Mog_BaseTag>;

			MOG_ControllerSyncData controller = MOG_ControllerProject.GetCurrentSyncDataController();
			if (controller != null)
			{
				WorkspaceManager.SuspendPackaging(true);

				guiAssetController asset = new guiAssetController();

				for (int i = 0; i < tags.Count && !worker.CancellationPending; i++)
				{
					Mog_BaseTag tag = tags[i];
					MOG_Filename filename = new MOG_Filename(tag.FullFilename);

					string message = "Removing:\n" +
									 "     " + filename.GetAssetClassification() + "\n" +
									 "     " + filename.GetAssetName();
					worker.ReportProgress(i * 100 / tags.Count, message);

					if (filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
					{
						asset.RemoveLocal(tag.FullFilename, controller, worker);
					}
				}

				WorkspaceManager.SuspendPackaging(false);
			}
		}

		/// <summary>
		/// Update this asset to the local data
		/// </summary>
		private void MenuItemUpdateLocal_Click(object sender, System.EventArgs e)
		{
			try
			{
				ArrayList selectedItems = ControlGetSelectedItemTags();
			
				// If we have a valid gameDataController?
				if (MOG_ControllerProject.GetCurrentSyncDataController() == null)
				{
					MOG_Prompt.PromptResponse("Update Build", "Cannot update an asset to a Local Workspace without a valid Local Workspace defined.  Create a Local Workspace first then try again.");
				}
				else
				{
					List<Mog_BaseTag> tags = new List<Mog_BaseTag>();
					List<MOG_Filename> updates = new List<MOG_Filename>();
					bool cancel = false;

					if (selectedItems.Count > 30)
					{
						if (MOG_Prompt.PromptResponse("Update Build", "You have selected a large number of items to update.  This could take a while.  Are you sure you want to update these " + selectedItems.Count + " items?", Environment.StackTrace, MOGPromptButtons.YesNo) == MOGPromptResult.No)
						{
							cancel = true;
						}
					}

					if (!cancel)
					{
						foreach (Mog_BaseTag tag in selectedItems)
						{
							if (tag.Execute)
							{
								MOG_Filename filename = new MOG_Filename(tag.FullFilename);

								// Make sure we are an asset before showing log
								if (filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
								{
									filename = FixupAssetPath(UpdateLocal_MenuItemText, filename);
									updates.Add(filename);
								}
							}
						}

						if (updates.Count > 0)
						{
							guiAssetController.UpdateLocal(updates, true);
						}
					}
				}
			}
			catch(Exception ex)
			{
				MOG_Report.ReportMessage(UpdateLocal_MenuItemText, ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
			}
		}
		
		/// <summary>
		/// Rename asset or group of assets
		/// </summary>
		private void MenuItemRename_Click(object sender, System.EventArgs e)
		{
			try
			{
				ArrayList selectedItems = ControlGetSelectedItemTags();
				guiAssetController asset = new guiAssetController();
				ArrayList sourceFiles = new ArrayList();	
					
				// Populate sourceFiles with string FullFilename from tag
				foreach (Mog_BaseTag tag in selectedItems)
				{				
					if (tag.Execute)
					{
						MOG_Filename filename = new MOG_Filename(tag.FullFilename);
					
						// Make sure we are an asset before showing log
						if (filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
						{
							sourceFiles.Add(tag.FullFilename);						
						}
					}
				}

				if (sourceFiles.Count > 0)				
				{
					asset.Rename(sourceFiles);
				}
			}
			catch(Exception ex)
			{
				MOG_Report.ReportMessage(Rename_MenuItemText, ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
			}
		}

		/// <summary>
		/// Open an asset for editing
		/// </summary>
		/// <param name="sender"> 
		/// <param name="e"></param>
		private void MenuItemEdit_Click(object sender, System.EventArgs e)
		{
			try
			{
				ArrayList selectedItems = ControlGetSelectedItemTags();

				foreach (Mog_BaseTag tag in selectedItems)
				{
					if (tag.Execute)
					{
						MOG_Properties assetProperties = new MOG_Properties(new MOG_Filename(tag.FullFilename));

						// Get the desired platform
						string platformName = "";
						if (MOG_ControllerProject.GetCurrentSyncDataController() != null)
						{
							platformName = MOG_ControllerProject.GetCurrentSyncDataController().GetPlatformName();
						}

						string folder = MOG_ControllerAsset.GetAssetProcessedDirectory(assetProperties, platformName);

						foreach (string file in DosUtils.FileGetRecursiveList(folder, "*.*"))
						{
							guiCommandLine.ShellSpawn(file);
						}
					}
				}
			}
			catch(Exception ex)
			{
				MOG_Report.ReportMessage(Explore_MenuItemText, ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
			}
		}

        /// <summary>
        /// Open an explorer window in the target of the selected asset(s)
        /// </summary>
        /// <param name="sender"> Must be a ListView and have a string[] in its Tag describing the columns</param>
        /// <param name="e"></param>
        private void MenuItemExploreLibrary_Click(object sender, System.EventArgs e)
        {
            try
            {
                ArrayList selectedItems = ControlGetSelectedItemTags();
                // If user does not want to continue, don't continue...
                if (UserWantsToShellExecuteManyAssets(selectedItems, Explore_MenuItemText) == false)
                {
                    return;
                }


                foreach (Mog_BaseTag tag in selectedItems)
                {
					string librarySourceDir = "";

					// Check if this is a tree node with a classification?
					Mog_BaseTag tag0 = selectedItems[0] as Mog_BaseTag;
					if (tag0.Owner is TreeNode && ((TreeNode)tag0.Owner).TreeView != null)
					{
						// We need the classification name, and the place to get it is the fullpath of the treenode
						string classification = ((TreeNode)tag0.Owner).FullPath;
						librarySourceDir = MOG_ControllerLibrary.ConstructPathFromLibraryClassification(classification);
					}
					else if (tag.Execute)
					{
						MOG_Filename asset = new MOG_Filename(tag.FullFilename);
						if (asset.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
						{
							librarySourceDir = MOG_ControllerLibrary.ConstructPathFromLibraryClassification(asset.GetAssetClassification());
						}
						else if (DosUtils.FileExistFast(tag.FullFilename))
						{
							librarySourceDir = DosUtils.PathGetDirectoryPath(tag.FullFilename);
						}
					}

					// Check if we found a directory?
					if (librarySourceDir.Length > 0)
					{
						// Check if this directory is missing?
						if (!DosUtils.DirectoryExistFast(librarySourceDir))
						{
							// Create an empty directory because why would we ever want to not allow the user to explore it?
							DosUtils.DirectoryCreate(librarySourceDir);
						}

						// Make sure this directory exists?
						if (DosUtils.DirectoryExistFast(librarySourceDir))
						{
							guiCommandLine.ShellExecute(librarySourceDir, false);
						}
						else
						{
							string message = "This directory does not exist on this machine.\n" + 
											 "DIRECTORY: " + librarySourceDir + "\n\n" + 
											 "Please make sure you can properly sync this directory.";
							MOG_Report.ReportMessage(Explore_MenuItemText, message, "", MOG_ALERT_LEVEL.ALERT);
						}
					}
                }
            }
            catch (Exception ex)
            {
                MOG_Report.ReportMessage(Explore_MenuItemText, ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
            }
        }
		/// <summary>
		/// Open an explorer window in the target of the selected asset(s)
		/// </summary>
		/// <param name="sender"> Must be a ListView and have a string[] in its Tag describing the columns</param>
		/// <param name="e"></param>
		private void MenuItemExplore_Click(object sender, System.EventArgs e)
		{
			try
			{
				ArrayList selectedItems = ControlGetSelectedItemTags();
				// If user does not want to continue, don't continue...
				if(UserWantsToShellExecuteManyAssets(selectedItems, Explore_MenuItemText) == false)
				{
					return;
				}
				

				foreach (Mog_BaseTag tag in selectedItems)
				{
					if (tag.Execute)
					{
						MOG_Filename assetFilename = new MOG_Filename( tag.FullFilename );
						string exploreDirectory = "";

						// Check if this is a locally update asset?
						if (assetFilename.IsLocal())
						{
							// Build this asset's SyncTargetPath
							MOG_Properties properties = new MOG_Properties(assetFilename);
							// Does this asset get synced?
							if (properties.SyncFiles)
							{
								// Explore the local SyncTargetPath for this asset
								string relativePath = MOG_Tokens.GetFormattedString(properties.SyncTargetPath, assetFilename, properties.GetApplicableProperties()).Trim("\\".ToCharArray());
								exploreDirectory = Path.Combine(MOG_ControllerProject.GetWorkspaceDirectory(), relativePath);
							}
							else
							{
								string message = "";

								if (properties.IsPackagedAsset)
								{
									message =  "This is a packaged asset and only exists in a package.\n" + 
											   "Asset: " + assetFilename.GetAssetFullName();
								}
								else
								{
									message =  "This is a non-syncing asset so there is no file to explore.\n" + 
											   "Asset: " + assetFilename.GetAssetFullName();
								}
								MOG_Prompt.PromptMessage("Can't explore this asset", message);
							}
						}
						else
						{
							exploreDirectory = FixupAssetPath("", assetFilename).GetOriginalFilename();
						}

						if (exploreDirectory.Length > 0)
						{
							if (Directory.Exists(exploreDirectory))
							{
								guiCommandLine.ShellExecute(exploreDirectory, false);
							}
							else
							{
								MOG_Prompt.PromptMessage("Containing directory does not exist", exploreDirectory);
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				MOG_Report.ReportMessage(Explore_MenuItemText, ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
			}
		}

		/// <summary>
		/// Open asset properties window
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MenuItemProperties_Click(object sender, System.EventArgs e)
		{
			ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
			if (menuItem != null)
			{
				Control sourceControl = SourceControl != null ? sourceControl = SourceControl : sourceControl = MogMainForm.MainApp.TopLevelControl;
				if (sourceControl != null)
				{
					try
					{
						ArrayList selectedItems = ControlGetSelectedItemTags();

						if (selectedItems.Count > 0)
						{
							ArrayList assetNames = new ArrayList();

							foreach (Mog_BaseTag tag in selectedItems)
							{
								if (tag.Execute)
								{
									string assetName = tag.FullFilename;
									MOG_Filename filename = new MOG_Filename(assetName);

									// Check to see if this is a blessed asset
									filename = FixupAssetPath("Get asset properties", filename);

									assetNames.Add(filename.GetOriginalFilename());
								}
								else if (tag.Owner is TreeNode)
								{
									string classification = ((TreeNode)tag.Owner).FullPath;
									assetNames.Add(classification);
								}
							}

							AssetPropertiesForm properties = new AssetPropertiesForm();
							bool alreadyOpen = AssetPropertiesForm.BringToFrontIfAlreadyOpen(assetNames, properties);
							if (!alreadyOpen)
							{
								properties.Initialize(assetNames);
								properties.Show(sourceControl.TopLevelControl);
							}
						}
					}
					catch (Exception ex)
					{
						MOG_Report.ReportMessage(Properties_MenuItemText, ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
					}
				}
			}
		}

		/// <summary>
		/// Open asset properties window and look at the comments
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MenuItemShowComments_Click(object sender, System.EventArgs e)
		{
			ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
			if (menuItem != null)
			{
				Control sourceControl = SourceControl != null ? sourceControl = SourceControl : sourceControl = MogMainForm.MainApp.TopLevelControl;
				if (sourceControl != null)
				{
					try
					{
						ArrayList selectedItems = ControlGetSelectedItemTags();

						if (selectedItems.Count > 0)
						{
							ArrayList assetNames = new ArrayList();

							foreach (Mog_BaseTag tag in selectedItems)
							{
								if (tag.Execute)
								{
									string assetName = tag.FullFilename;
									MOG_Filename filename = new MOG_Filename(assetName);

									// Check to see if this is a blessed asset
									filename = FixupAssetPath("Get asset properties", filename);

									assetNames.Add(filename.GetOriginalFilename());
								}
								else if (tag.Owner is TreeNode)
								{
									string classification = ((TreeNode)tag.Owner).FullPath;
									assetNames.Add(classification);
								}
							}

							AssetPropertiesForm properties = new AssetPropertiesForm();
							bool alreadyOpen = AssetPropertiesForm.BringToFrontIfAlreadyOpen(assetNames, properties);
							if (!alreadyOpen)
							{
								properties.Initialize(assetNames);
								properties.Show(sourceControl.TopLevelControl);
								properties.SelectTab("PropertiesCommentsTabPage");
							}
						}
					}
					catch (Exception ex)
					{
						MOG_Report.ReportMessage(Properties_MenuItemText, ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
					}
				}
			}
		}
		
		/// <summary>
		/// Launch assets viewer and view this asset
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MenuItemSetDblClick_Click(object sender, System.EventArgs e)
		{
			try
			{
				ToolStripMenuItem target = (ToolStripMenuItem)sender;
				ArrayList selectedItems = ControlGetSelectedItemTags();

				// Get the last saved view target
				string doubleClickCommand = "";

				if (target.Text.StartsWith(DblClick_MenuItemText))
				{
					doubleClickCommand = guiUserPrefs.LoadPref("ClientPrefs", "ContextMenu", "DoubleClick");					
				}
				else
				{
					doubleClickCommand = target.Text;
					MogUtils_Settings.MogUtils_Settings.SaveSetting("ClientPrefs", "ContextMenu", "DoubleClick", doubleClickCommand);
				}
			}
			catch(Exception ex)
			{
				MOG_Report.ReportMessage("Set Double Click", ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
			}
		}

		/// <summary>
		/// Launch assets viewer and view this asset
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MenuItemView_Click(object sender, System.EventArgs e)
		{
			try
			{
				ToolStripMenuItem target = (ToolStripMenuItem)sender;
				ArrayList selectedItems = ControlGetSelectedItemTags();

				// Get the last saved view target
				string viewTarget = "";

				if (target.Text.StartsWith(View_MenuItemText))
				{
					viewTarget = guiUserPrefs.LoadPref("ClientPrefs", "ContextMenu", "ViewTarget");
				}
				else
				{
					viewTarget = target.Text;
					MogUtils_Settings.MogUtils_Settings.SaveSetting("ClientPrefs", "ContextMenu", "ViewTarget", viewTarget);
					target.Checked = true;
				}

				foreach (Mog_BaseTag tag in selectedItems)
				{
					if (tag.Execute)
					{
						guiAssetController.AssetDirectories viewType = guiAssetController.AssetDirectories.PROCESSED;
						switch(viewTarget)
						{
							case ViewSource_MenuItemText:
								viewType = guiAssetController.AssetDirectories.SOURCE;
								break;
							case ViewProcessed_MenuItemText:
								viewType = guiAssetController.AssetDirectories.PROCESSED;
								break;
							case ViewImported_MenuItemText:
								viewType = guiAssetController.AssetDirectories.IMPORTED;
								break;
						}

						string assetName = tag.FullFilename;
						MOG_Filename filename = new MOG_Filename(assetName.ToLower());

						MOG_Properties pProperties = new MOG_Properties(filename);
						string viewer = pProperties.AssetViewer;

						// Check to see if this is a blessed asset
						filename = FixupAssetPath("View Asset", filename);

						guiAssetController controller = new guiAssetController();
						controller.View(filename, viewType, viewer);
					}
				}
			}
			catch(Exception ex)
			{
				MOG_Report.ReportMessage(View_MenuItemText, ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
			}
		}

		/// <summary>
		/// Send 
		/// </summary>
		private void MenuItemSendTo_Click(object sender, System.EventArgs e)
		{
			ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
			if (menuItem != null)
			{
				ArrayList selectedItems = ControlGetSelectedItemTags();

				// Get the comment for this asset or set of assets
				BlessInfoForm bless = new BlessInfoForm(selectedItems, false);
				bless.Text = "SendTo";

				// Show BlessDialog as a modal dialog and determine if DialogResult = OK.
				if (bless.ShowDialog() == DialogResult.OK)
				{
					// Get the comment file from the control by coping each line into our string
					string comment = "";
					foreach (string line in bless.BlessInfoBlessCommentRichTextBox.Lines)
					{
						if (line.Length > 0)
						{
							comment += line + Environment.NewLine;
						}
					}

					List<MOG_Filename> filenames = new List<MOG_Filename>();

					// Get all the checked items from the blessinfo checklist
					foreach (ListViewItem item in bless.BlessInfoBlessFilesCheckedListView.CheckedItems)
					{
						filenames.Add(new MOG_Filename(item.SubItems[2].Text));
					}

					if (filenames.Count > 0)
					{
						List<object> args = new List<object>();
						args.Add(filenames);
						args.Add(MOG_ControllerProject.GetActiveUserName());
						args.Add(menuItem.Text);
						args.Add(comment);
						args.Add("Send");

						ProgressDialog progress = new ProgressDialog("Sending assets", "Please wait while MOG sends the assets.", CopyTo_Worker, args, true);
						Control sourceControl = SourceControl != null ? sourceControl = SourceControl : sourceControl = MogMainForm.MainApp.TopLevelControl;
						progress.ShowDialog(sourceControl.TopLevelControl);						
					}
				}
			}
		}

		/// <summary>
		/// Copy an asset to another users inbox
		/// </summary>
		private void MenuItemCopyTo_Click(object sender, System.EventArgs e)
		{
			ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
			if (menuItem != null)
			{
				ArrayList selectedItems = ControlGetSelectedItemTags();

				// Get the comment for this asset or set of assets
				BlessInfoForm bless = new BlessInfoForm(selectedItems, false);
				bless.Text = "CopyTo";

				// Show BlessDialog as a modal dialog and determine if DialogResult = OK.
				if (bless.ShowDialog() == DialogResult.OK)
				{
					// Get the comment file from the control by coping each line into our string
					string comment = "";
					foreach (string line in bless.BlessInfoBlessCommentRichTextBox.Lines)
					{
						if (line.Length > 0)
						{
							comment += line + Environment.NewLine;
						}
					}

					List<MOG_Filename> filenames = new List<MOG_Filename>();

					// Get all the checked items from the blessinfo checklist
					foreach (ListViewItem item in bless.BlessInfoBlessFilesCheckedListView.CheckedItems)
					{
						filenames.Add(new MOG_Filename(item.SubItems[2].Text));
					}

					if (filenames.Count > 0)
					{
						List<object> args = new List<object>();
						args.Add(filenames);
						args.Add(MOG_ControllerProject.GetActiveUserName());
						args.Add(menuItem.Text);
						args.Add(comment);
						args.Add("Copy");
						
						ProgressDialog progress = new ProgressDialog("Copying assets", "Please wait while MOG copies the assets.", CopyTo_Worker, args, true);
						Control sourceControl = SourceControl != null ? sourceControl = SourceControl : sourceControl = MogMainForm.MainApp.TopLevelControl;
						progress.ShowDialog(sourceControl.TopLevelControl);						
					}
				}
			}
		}

		private void CopyTo_Worker(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			List<object> args = e.Argument as List<object>;
			List<MOG_Filename> filenames = args[0] as List<MOG_Filename>;
			string sourceUser = args[1] as string;
			string targetUser = args[2] as string;
			string comment = args[3] as string;
			string operation = args[4] as string;

			for (int i = 0; i < filenames.Count && !worker.CancellationPending; i++)
			{
				MOG_Filename filename = filenames[i];

				filename = FixupAssetPath("Asset " + operation + " to", filename);

				int percent = (i * 100) / filenames.Count;
				string message = operation + ":\n" +
								 "     " + filename.GetAssetClassification() + "\n" +
								 "     " + filename.GetAssetName();
				worker.ReportProgress(percent, message);

				if (String.Compare(operation, "Copy", true) == 0)
				{
					MOG_ControllerInbox.CopyAssetToUserInbox(filename, targetUser, comment, MOG_AssetStatusType.Sent, worker);
				}
				else if (String.Compare(operation, "Send", true) == 0)
				{
					MOG_ControllerInbox.MoveAssetToUserInbox(filename, targetUser, comment, MOG_AssetStatusType.Sent, worker);
				}

				// Save this user in our recent list
				MogUtils_Settings.MogUtils_Settings.Settings.PutSectionString("SendCopyToRecents", targetUser);
				MogUtils_Settings.MogUtils_Settings.Save();
			}
		}

		/// <summary>
		/// Remove an asset from the project
		/// </summary>
		/// <param name="sender"> Must be a ListView and have a string[] in its Tag describing the columns</param>
		private void MenuItemRemoveFromProject_Click(object sender, System.EventArgs e)
		{
			MOG_Privileges privs = MOG_ControllerProject.GetPrivileges();
			if (privs.GetUserPrivilege(MOG_ControllerProject.GetUserName(), MOG_PRIVILEGE.RemoveAssetFromProject))
			{
				try
				{
					ArrayList selectedItems = ControlGetSelectedItemTags();

					string message = "";
					foreach (Mog_BaseTag tag in selectedItems)
					{
						if (tag.Execute)
						{
							MOG_Filename filename = new MOG_Filename(tag.FullFilename);
							message = message + filename.GetAssetFullName() + "\n";
						}
					}

					message = "Are you sure you want to remove all of these assets from the project?" + Environment.NewLine + message;

					if (MOG_Prompt.PromptResponse("Remove Asset From Project", message, MOGPromptButtons.YesNoCancel) == MOGPromptResult.Yes)
					{
						List<Mog_BaseTag> tags = new List<Mog_BaseTag>();

						foreach (Mog_BaseTag tag in selectedItems)
						{
							if (tag.Execute)
							{
								tags.Add(tag);
							}
						}

						ProgressDialog progress = new ProgressDialog("Removing assets from project", "Please wait while MOG removes the assets from the project.", RemoveFromProject_Worker, tags, true);
						progress.ShowDialog();
					}
				}
				catch (Exception ex)
				{
					MOG_Report.ReportMessage(RemoveFromProject_MenuItemText, ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
				}
			}
			else
			{
				MOG_Prompt.PromptResponse("Insufficient Privileges", "Your privileges do not allow you to remove assets from the project.");
			}
		}

		private void RemoveFromProject_Worker(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;

			List<Mog_BaseTag> tags = e.Argument as List<Mog_BaseTag>;
			List<Mog_BaseTag> results = new List<Mog_BaseTag>();

			// Obtain a unique bless label
			string jobLabel = "RemoveAsset." + MOG_ControllerSystem.GetComputerName() + "." + MOG_Time.GetVersionTimestamp();

			for (int i = 0; i < tags.Count && !worker.CancellationPending; i++)
			{
				Mog_BaseTag tag = tags[i];
				MOG_Filename filename = new MOG_Filename(tag.FullFilename);

				string message = "Removing:\n" +
								 "     " + filename.GetAssetClassification() + "\n" +
								 "     " + filename.GetAssetName();
				worker.ReportProgress(i * 100 / tags.Count, message);
				
				// Make sure we are an asset before showing log
				if (filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
				{
					bool success = true;

// JohnRen - Removed because the DLL will be smart about how to remove an asset missing a revision
//					// Check to see if this is a blessed asset
//					filename = FixupAssetPath("Asset remove from project", filename);
					
					// If it looks like we have a package...
					if (tag.PackageFullName.Length > 0)
					{
						// Remove whichever group(s) or object(s) we have selected
						string groups, objects;
						groups = MOG_ControllerPackage.GetPackageGroups(tag.PackageFullName);
						objects = MOG_ControllerPackage.GetPackageObjects(tag.PackageFullName);

						//If there is an object, concat that onto the group specifier
						if (objects != null && objects.Length > 0)
						{
							if (groups != null && groups.Length > 0)
								groups += "/" + objects;
							else
								groups = objects;
						}

						if (groups != null && groups.Length > 0)
						{
							//There is a group specifier on here, so remove that instead of the actual package
							success &= guiAssetController.RemoveBlessedGroup(filename, groups, false);
						}
						else
						{
							//No group, just remove the package
							success &= guiAssetController.RemoveBlessed(filename, jobLabel);
						}
					}
					else
					{
						success &= guiAssetController.RemoveBlessed(filename, jobLabel);
					}
					
					if (success)
					{
						//Add this to the results so we can remove it once we return from this worker
						results.Add(tag);
					}
				}
			}

			// Start the job
			MOG_ControllerProject.StartJob(jobLabel);

			e.Result = results;
		}

		/// <summary>
		/// Makes the selected assets the current version in the current branch
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MenuItemMakeCurrent_Click(object sender, System.EventArgs e)
		{
			try
			{
				// Scan the list and prepare it for delivery to the DLL
				ArrayList selectedItems = ControlGetSelectedItemTags();
				ArrayList assetFilenames = new ArrayList();

				foreach (Mog_BaseTag tag in selectedItems)
				{
					if (tag.Execute)
					{
						// Check to see if this is a blessed asset
						MOG_Filename filename = FixupAssetPath("Make Current", new MOG_Filename(tag.FullFilename));
						assetFilenames.Add(filename);
					}
				}

				// Check if this request effects more than one asset??
				if (selectedItems.Count > 1)
				{
					string message = "Are you sure you want to make these (" + assetFilenames.Count.ToString() + ") assets current?";
					if (MOG_Prompt.PromptResponse("Restamp Assets", message, MOGPromptButtons.OKCancel) != MOGPromptResult.OK)
					{
						return;
					}
				}

				// Stamp all the specified assets
				if (MOG_ControllerProject.MakeAssetCurrentVersion(assetFilenames, "Made current by " + MOG_ControllerProject.GetUserName_DefaultAdmin()))
				{
					// Check if this request effects more than one asset??
					if (selectedItems.Count > 1)
					{
						// Inform the user this may take a while
						MOG_Prompt.PromptResponse(	"Completed",
													"This change requires Slave processing.\n" +
													"The project will not reflect these changes until all slaves have finished processing the generated commands.\n" +
													"The progress of this task can be monitored in the Connections Tab.");
					}
				}
				else
				{
					MOG_Prompt.PromptResponse("Make Current Failed", "The system was unable to fully complete the task!");
				}
			}
			catch(Exception ex)
			{
				MOG_Report.ReportMessage(MakeCurrent_MenuItemText, ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
			}
		}

		/// <summary>
		/// Deletes the servers package for this asset then sends a package rebuild command to the server
		/// </summary>
		private void MenuItemRebuildPackage_Click(object sender, System.EventArgs e)
		{
			try
			{
				ArrayList packages = new ArrayList();
				string message = "";

				ArrayList selectedItems = ControlGetSelectedItemTags();
				List<MOG_Filename> filenames = new List<MOG_Filename>();

				foreach (Mog_BaseTag tag in selectedItems)
				{
					if (tag.Execute)
					{
						// Get the properties for this package
						MOG_Filename assetFilename = new MOG_Filename(tag.FullFilename);
						MOG_Properties properties = new MOG_Properties(assetFilename);
						properties.SetScope(MOG_ControllerProject.GetPlatformName());

						if (properties.IsPackage)
						{
							// Indicate this to be the only package we will be rebuilding by creating our own package assignment property
							packages.Add(MOG.MOG_PropertyFactory.MOG_Relationships.New_PackageAssignment(MOG_ControllerPackage.GetPackageName(assetFilename.GetAssetFullName()), "", ""));
						}
						else if (properties.IsPackagedAsset)
						{
							// Get all the assigned packages of this asset as our list of package assignment properties
							packages.AddRange(properties.GetApplicablePackages());
						}

						filenames.Add(assetFilename);
					}
				}

				if (packages.Count > 0)
				{
					// Inform the user concerning how many packages will be rebuilt
					message = "";
					foreach (MOG_Property packageAssignment in packages)
					{
						string packageName = MOG_ControllerPackage.GetPackageName(packageAssignment.mPropertyKey);
						message += packageName + Environment.NewLine;
					}
					
					if (MOG_Prompt.PromptResponse("Are you sure you want to rebuild the following packages?", message, Environment.StackTrace, MOGPromptButtons.OKCancel, MOG_ALERT_LEVEL.MESSAGE) == MOGPromptResult.OK)
					{
						ProgressDialog progress = new ProgressDialog("Scheduling the RebuildPackage Commands", "Please wait while the rebuild package commands are scheduled on the server for processing.", RebuildPackages_Worker, filenames, true);
						progress.ShowDialog();
					}
				}
				else
				{
					MOG_Prompt.PromptResponse("No packages rebuilt", "There were no associated packages for the specified asset(s)." + Environment.NewLine + "No packages were rebuilt.");
				}
			}
			catch (Exception ex)
			{
				MOG_Report.ReportMessage(RebuildPackage_MenuItemText, ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
			}
		}

		private void RebuildPackages_Worker(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			List<MOG_Filename> filenames = e.Argument as List<MOG_Filename>;
			int f = 0;

			foreach (MOG_Filename filename in filenames)
			{
				if (filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
				{
					string message = "Scheduling Package Rebuild:\n" +
									 "     " + filename.GetAssetClassification() + "\n" +
									 "     " + filename.GetAssetName();
					worker.ReportProgress(f++ * 100 / filenames.Count, message);

					// Obtain a unique bless label
					string jobLabel = "RebuildPackage." + MOG_ControllerSystem.GetComputerName() + "." + MOG_Time.GetVersionTimestamp();
					// Schedule the rebuild command
					MOG_ControllerPackage.RebuildPackage(filename, jobLabel);
					// Start the job
					MOG_ControllerProject.StartJob(jobLabel);

					// Well, this is a bit of a hack but was the easiest and safest way to ensure unique JobIDs...
					// JobIDs are only accurate to the microsecond so lets sleep for a very short time.
					// Extended this out to a full second because it was causing too much strain on the SQL server sending all these at once
					Thread.Sleep(1000);

					// Check for cancel
					if (worker.CancellationPending)
					{
						break;
					}
				}
			}
		}

		private void MenuItemAdminToolRepairGameDataTable_Click(object sender, System.EventArgs e)
		{
			ArrayList selectedItems = ControlGetSelectedItemTags();

			foreach (Mog_BaseTag tag in selectedItems)
			{
				ArrayList properties = new ArrayList();

				properties.Add(MOG.MOG_PropertyFactory.MOG_Asset_StatsProperties.New_Size(0));
				properties.Add(MOG.MOG_PropertyFactory.MOG_Asset_InfoProperties.New_IsPackagedAsset(false));
				properties.Add(MOG.MOG_PropertyFactory.MOG_Asset_StatsProperties.New_Owner(""));
				properties.Add(MOG.MOG_PropertyFactory.MOG_Asset_StatsProperties.New_Creator(""));
				properties.Add(MOG.MOG_PropertyFactory.MOG_Asset_StatsProperties.New_SourceMachine(""));
				properties.Add(MOG.MOG_PropertyFactory.MOG_Asset_StatsProperties.New_CreatedTime(""));
                properties.Add(MOG.MOG_PropertyFactory.MOG_Sync_OptionsProperties.New_SyncTargetPath(""));

				ArrayList CurrentAssets = GetAssetsList(tag, properties);

				// Quickly calculate our totals
				//int total = FilterAssetListByTreePosition(CurrentAssets, tag.FullFilename);

				// Get the list of colliding assets
				MOG_SystemUtilities.FixGameDataTable_MissingSlashes();
			}	
		}

		private void MenuItemAdminToolsVerifyUpdateSqlTables_Click(object sender, System.EventArgs e)
		{
			MOG_ControllerSystem.GetDB().UpdateSystemTables(MOG_ControllerSystem.GetDB().GetDBVersion(), false);
			foreach(string currentProject in MOG_ControllerSystem.GetSystem().GetProjectNames())
			{
				MOG_ControllerSystem.GetDB().UpdateTables(currentProject, MOG_ControllerSystem.GetDB().GetDBVersion(), true);
			}
		}

		private void MenuItemAdminToolsCleanInvalidItems_Click(object sender, System.EventArgs e)
		{
			MOG_SystemUtilities.CleanInvalidItems();
		}

		private void MenuItemAdminToolsArchiveUnreferencedRevisions_Click(object sender, System.EventArgs e)
		{
			ArrayList selectedItems = ControlGetSelectedItemTags();
			foreach (Mog_BaseTag tag in selectedItems)
			{
				// Get our current classification name and project
				string classification = ((TreeNode)tag.Owner).FullPath;

				// Obtain the list of assets from SystemUtilities
				ArrayList assets = MOG_SystemUtilities.AdminToolsReportUnreferencedRevisions(classification);

				if (assets != null && assets.Count > 0)
				{
					// Display this list in the report
					ListForm listForm = new ListForm("Generate User Report");
					listForm.ListListView.SmallImageList = MogUtil_AssetIcons.Images;

					bool cancelled = false;
					int i = 0;
					int total = selectedItems.Count;
					CallbackDialogForm progress = new CallbackDialogForm();

					listForm.ListListView.BeginUpdate();

					foreach (MOG_Filename asset in assets)
					{
						listForm.ItemAdd(asset, null, "");

						// Update our progress
						progress.DialogUpdate((i++ * 100) / total, string.Concat("Checking...\n", asset.GetAssetName()));

						// Check for cancel
						cancelled = progress.DialogProcess();
						if (cancelled)
						{
							break;
						}
					}

					listForm.ListListView.EndUpdate();

					// Cleanup dialog
					progress.DialogKill();
					progress = null;

					if (!cancelled)
					{
						listForm.UpdateAssetTotals();
					}

					Control sourceControl = SourceControl != null ? sourceControl = SourceControl : sourceControl = MogMainForm.MainApp.TopLevelControl;
					if (sourceControl != null)
					{
						listForm.Show(sourceControl.TopLevelControl);
					}
				}
			}
		}

		private void MenuItemAdminToolsRepairRevisionMetadata_Click(object sender, System.EventArgs e)
		{
			ArrayList selectedItems = ControlGetSelectedItemTags();
			foreach (Mog_BaseTag tag in selectedItems)
			{
				// Get our current classification name and project
				string classification = ((TreeNode)tag.Owner).FullPath;

				// Obtain the list of assets from SystemUtilities
				ArrayList assets = MOG_SystemUtilities.RepairRevisionMetadata(classification);

				if (assets != null && assets.Count > 0)
				{
					// Display this list in the report
					ListForm listForm = new ListForm("Generate User Report");
					listForm.ListListView.SmallImageList = MogUtil_AssetIcons.Images;

					bool cancelled = false;
					int i = 0;
					int total = selectedItems.Count;
					CallbackDialogForm progress = new CallbackDialogForm();

					listForm.ListListView.BeginUpdate();

					foreach (MOG_Filename asset in assets)
					{
						listForm.ItemAdd(asset, null, "");

						// Update our progress
						progress.DialogUpdate((i++ * 100) / total, string.Concat("Checking...\n", asset.GetAssetName()));

						// Check for cancel
						cancelled = progress.DialogProcess();
						if (cancelled)
						{
							break;
						}
					}

					listForm.ListListView.EndUpdate();

					// Cleanup dialog
					progress.DialogKill();
					progress = null;

					if (!cancelled)
					{
						listForm.UpdateAssetTotals();
					}

					Control sourceControl = SourceControl != null ? sourceControl = SourceControl : sourceControl = MogMainForm.MainApp.TopLevelControl;
					if (sourceControl != null)
					{
						listForm.Show(sourceControl.TopLevelControl);
					}
				}
			}
		}


		private void MenuItemAdminToolsReportNewerUnpostedRevisions_Click(object sender, System.EventArgs e)
		{
			ArrayList selectedItems = ControlGetSelectedItemTags();
			foreach (Mog_BaseTag tag in selectedItems)
			{
				// Get our current classification name and project
				string classification = ((TreeNode)tag.Owner).FullPath;

				// Obtain the list of assets from SystemUtilities
				ArrayList assets = MOG_SystemUtilities.FindNewerUnpostedRevisions(classification);

				if (assets != null && assets.Count > 0)
				{
					// Display this list in the report
					ListForm listForm = new ListForm("Generate User Report");
					listForm.ListListView.SmallImageList = MogUtil_AssetIcons.Images;

					bool cancelled = false;
					int i = 0;
					int total = selectedItems.Count;
					CallbackDialogForm progress = new CallbackDialogForm();

					listForm.ListListView.BeginUpdate();

					foreach (MOG_Filename asset in assets)
					{
						listForm.ItemAdd(asset, null, "");

						// Update our progress
						progress.DialogUpdate((i++ * 100) / total, string.Concat("Checking...\n", asset.GetAssetName()));

						// Check for cancel
						cancelled = progress.DialogProcess();
						if (cancelled)
						{
							break;
						}
					}

					listForm.ListListView.EndUpdate();

					// Cleanup dialog
					progress.DialogKill();
					progress = null;

					if (!cancelled)
					{
						listForm.UpdateAssetTotals();
					}

					Control sourceControl = SourceControl != null ? sourceControl = SourceControl : sourceControl = MogMainForm.MainApp.TopLevelControl;
					if (sourceControl != null)
					{
						listForm.Show(sourceControl.TopLevelControl);
					}
				}
			}
		}


		private void MenuItemAdminToolsReportMultiplePackageAssignments_Click(object sender, System.EventArgs e)
		{
			ArrayList selectedItems = ControlGetSelectedItemTags();
			foreach (Mog_BaseTag tag in selectedItems)
			{
				// Get our current classification name and project
				string classification = ((TreeNode)tag.Owner).FullPath;

				// Obtain the list of assets from SystemUtilities
				ArrayList assets = MOG_SystemUtilities.FindAssetWithMultiplePackageAssignments(classification);

				if (assets != null && assets.Count > 0)
				{
					// Display this list in the report
					ListForm listForm = new ListForm("Generate User Report");
					listForm.ListListView.SmallImageList = MogUtil_AssetIcons.Images;

					bool cancelled = false;
					int i = 0;
					int total = selectedItems.Count;
					CallbackDialogForm progress = new CallbackDialogForm();

					listForm.ListListView.BeginUpdate();

					foreach (MOG_Filename asset in assets)
					{
						listForm.ItemAdd(asset, null, "");

						// Update our progress
						progress.DialogUpdate((i++ * 100) / total, string.Concat("Checking...\n", asset.GetAssetName()));

						// Check for cancel
						cancelled = progress.DialogProcess();
						if (cancelled)
						{
							break;
						}
					}

					listForm.ListListView.EndUpdate();

					// Cleanup dialog
					progress.DialogKill();
					progress = null;

					if (!cancelled)
					{
						listForm.UpdateAssetTotals();
					}

					Control sourceControl = SourceControl != null ? sourceControl = SourceControl : sourceControl = MogMainForm.MainApp.TopLevelControl;
					if (sourceControl != null)
					{
						listForm.Show(sourceControl.TopLevelControl);
					}
				}
			}
		}


		private void MenuItemAdminToolsReportCollidingPackageAssignments_Click(object sender, System.EventArgs e)
		{
			ArrayList selectedItems = ControlGetSelectedItemTags();
			foreach (Mog_BaseTag tag in selectedItems)
			{
				// Get our current classification name and project
				string classification = ((TreeNode)tag.Owner).FullPath;

				// Obtain the list of assets from SystemUtilities
				ArrayList assets = MOG_SystemUtilities.FindCollidingPackageAssignments(classification);

				if (assets != null && assets.Count > 0)
				{
					// Display this list in the report
					ListForm listForm = new ListForm("Generate User Report");
					listForm.ListListView.SmallImageList = MogUtil_AssetIcons.Images;

					bool cancelled = false;
					int i = 0;
					int total = selectedItems.Count;
					CallbackDialogForm progress = new CallbackDialogForm();

					listForm.ListListView.BeginUpdate();

					foreach (MOG_Filename asset in assets)
					{
						listForm.ItemAdd(asset, null, "");

						// Update our progress
						progress.DialogUpdate((i++ * 100) / total, string.Concat("Checking...\n", asset.GetAssetName()));

						// Check for cancel
						cancelled = progress.DialogProcess();
						if (cancelled)
						{
							break;
						}
					}

					listForm.ListListView.EndUpdate();

					// Cleanup dialog
					progress.DialogKill();
					progress = null;

					if (!cancelled)
					{
						listForm.UpdateAssetTotals();
					}

					Control sourceControl = SourceControl != null ? sourceControl = SourceControl : sourceControl = MogMainForm.MainApp.TopLevelControl;
					if (sourceControl != null)
					{
						listForm.Show(sourceControl.TopLevelControl);
					}
				}
			}
		}


		private void MenuItemAdminToolsReportInvalidPackageAssignments_Click(object sender, System.EventArgs e)
		{
			ArrayList selectedItems = ControlGetSelectedItemTags();
			foreach (Mog_BaseTag tag in selectedItems)
			{
				// Get our current classification name and project
				string classification = ((TreeNode)tag.Owner).FullPath;

				// Obtain the list of assets from SystemUtilities
				ArrayList assets = MOG_SystemUtilities.FindInvalidPackageAssignments(classification);

				if (assets != null && assets.Count > 0)
				{
					// Display this list in the report
					ListForm listForm = new ListForm("Generate User Report");
					listForm.ListListView.SmallImageList = MogUtil_AssetIcons.Images;

					bool cancelled = false;
					int i = 0;
					int total = selectedItems.Count;
					CallbackDialogForm progress = new CallbackDialogForm();

					listForm.ListListView.BeginUpdate();

					foreach (MOG_Filename asset in assets)
					{
						listForm.ItemAdd(asset, null, "");

						// Update our progress
						progress.DialogUpdate((i++ * 100) / total, string.Concat("Checking...\n", asset.GetAssetName()));

						// Check for cancel
						cancelled = progress.DialogProcess();
						if (cancelled)
						{
							break;
						}
					}

					listForm.ListListView.EndUpdate();

					// Cleanup dialog
					progress.DialogKill();
					progress = null;

					if (!cancelled)
					{
						listForm.UpdateAssetTotals();
					}

					Control sourceControl = SourceControl != null ? sourceControl = SourceControl : sourceControl = MogMainForm.MainApp.TopLevelControl;
					if (sourceControl != null)
					{
						listForm.Show(sourceControl.TopLevelControl);
					}
				}
			}
		}


		private void MenuItemAdminToolsUpdateProjectFor304XDrop_Click(object sender, System.EventArgs e)
		{
			MOG_SystemUtilities.UpdateProjectFor304XDrop();
		}


		/// <summary>
		/// Scans through the selected packages to verify if all their assets are valid
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MenuItemGeneratePackageValidate_Click(object sender, System.EventArgs e)
		{
			ArrayList selectedItems = ControlGetSelectedItemTags();

			string message = "";
			foreach (Mog_BaseTag tag in selectedItems)
			{				
				//if ((tag.Level == RepositoryFocusLevelTREE_FOCUS.SUBCLASS) || (tag.Level == RepositoryFocusLevelTREE_FOCUS.CLASS))
				if( tag.RepositoryFocus == RepositoryFocusLevel.Classification )
				{
					message = message + ((TreeNode)tag.Owner).Text + "\n";
				}
					//else if (tag.Level == RepositoryFocusLevelTREE_FOCUS.LABEL)
				else
				{
					MOG_Prompt.PromptResponse("Package Validate", "This tool is only valid on Keys, Classes, and SubClasses.  Use 'package Asset validate' for actual packages");
					return;
				}
			}

			if (MOG_Prompt.PromptResponse("Are you sure you want to validate these package(s)?", message, MOGPromptButtons.OKCancel) == MOGPromptResult.OK)
			{
				// redo
			}
		}
		
		/// <summary>
		/// Spawns the Report window
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MenuItemGenerateReport_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
			if (menuItem != null)
			{
				Control sourceControl = SourceControl != null ? sourceControl = SourceControl : sourceControl = MogMainForm.MainApp.TopLevelControl;
				if (sourceControl != null)
				{
					CallbackDialogForm progress = null;
					try
					{
						ArrayList selectedItems = ControlGetSelectedItemTags();

						// Open the create list dialog
						NewListForm listDialog = new NewListForm();
						ListForm listForm = new ListForm("Generate User Report");
						listForm.ListListView.SmallImageList = MogUtil_AssetIcons.Images;

						// Load saved settings
						guiUserPrefs.LoadDynamic_LayoutPrefs("NewAssetList", listDialog);

						// This (should) indicate that we have either one ListViewItem or one TreeNode selected
						if (selectedItems.Count == 1 && selectedItems[0] is Mog_BaseTag)
						{
							if (PopulateReportFromDatabase(selectedItems[0] as Mog_BaseTag, listDialog, listForm, progress))
							{
								listForm.Show(sourceControl.TopLevelControl);
							}
						}
						else
						{
							// We have ListViewItems selected
							if (PopulateReportFromExistingItems(selectedItems, listDialog, listForm, progress))
							{
								listForm.Show(sourceControl.TopLevelControl);
							}
						}

						// Save the changes to this dialog
						guiUserPrefs.SaveDynamic_LayoutPrefs("NewAssetList", listDialog);

					}
					catch (Exception ex)
					{
						MOG_Report.ReportMessage(GenerateReport_MenuItemText, ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
					}
					finally
					{
						// If we haven't already killed it, kill our callback form
						if (progress != null)
						{
							// Cleanup dialog
							progress.DialogKill();
							progress = null;
						}
					}
				}
			}
		}

		private bool PopulateReportFromExistingItems(ArrayList selectedItems, NewListForm listDialog, ListForm listForm, CallbackDialogForm progress)
		{
			if (listDialog.ShowDialog() == DialogResult.OK)
			{
				bool cancelled = false;
				int i = 0;
				int total = selectedItems.Count;
				progress = new CallbackDialogForm();

				listForm.ListListView.BeginUpdate();

				foreach (Mog_BaseTag tag in selectedItems)
				{
					listForm.ItemAdd(tag);

					MOG_Filename mogFilename = new MOG_Filename(tag.FullFilename);

					// Update our progress
					progress.DialogUpdate((i++ * 100) / total, string.Concat("Checking...\n", mogFilename.GetAssetName()));

					// Check for cancel
					cancelled = progress.DialogProcess();
					if (cancelled)
					{
						break;
					}
				}

				listForm.ListListView.EndUpdate();

				// Cleanup dialog
				progress.DialogKill();
				progress = null;

				if (!cancelled)
				{
					listForm.UpdateAssetTotals();
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Given one Mog_BaseTag, populate our Report list from the database
		/// </summary>
		private bool PopulateReportFromDatabase(Mog_BaseTag tag, NewListForm listDialog, ListForm listForm, CallbackDialogForm progress)
		{
			if (listDialog.ShowDialog() == DialogResult.OK)
			{
				bool cancelled = false;

				// Get the path of this node
				string rootBranch = tag.ItemFullname().ToLower();

				// Replace the slash and convert the project name with the project key
				rootBranch = rootBranch.Replace("\\", ".");
				rootBranch = rootBranch.Replace(MOG_ControllerProject.GetProjectName().ToLower(), MOG_ControllerProject.GetProjectName().ToLower());

				// Init dialog window
				progress = new CallbackDialogForm();
				progress.DialogInitialize("Creating Asset List", "Initializing . . .", "Cancel");
				Application.DoEvents();

				// Precache the project
				MOG_ControllerSystem.GetDB().GetDBCache().GetAssetNameCache().PopulateCacheFromSQL(MOG_DBQueryBuilderAPI.MOG_DBCacheQueries.PopulateCache_AssetNameCache(), "CACHEID", "CACHENAME");
				MOG_ControllerSystem.GetDB().GetDBCache().GetAssetVersionCache().PopulateCacheFromSQL(MOG_DBQueryBuilderAPI.MOG_DBCacheQueries.PopulateCache_AssetVersionCache(), "CACHEID", "CACHENAME", "CACHELIST");

				ArrayList CurrentAssets;
				int total;

				// Build a list of properties that we care about
				ArrayList properties = new ArrayList();
				properties.Add(MOG.MOG_PropertyFactory.MOG_Asset_InfoProperties.New_IsPackage(false));
				properties.Add(MOG.MOG_PropertyFactory.MOG_Asset_StatsProperties.New_Size(-1));
				properties.Add(MOG.MOG_PropertyFactory.MOG_Asset_StatsProperties.New_Owner(""));
				properties.Add(MOG.MOG_PropertyFactory.MOG_Asset_StatsProperties.New_Creator(""));
				properties.Add(MOG.MOG_PropertyFactory.MOG_Asset_StatsProperties.New_SourceMachine(""));
				properties.Add(MOG.MOG_PropertyFactory.MOG_Asset_StatsProperties.New_CreatedTime(""));
				properties.Add(MOG.MOG_PropertyFactory.MOG_Asset_StatsProperties.New_LastComment(""));
				properties.Add(MOG.MOG_PropertyFactory.MOG_Sync_OptionsProperties.New_SyncTargetPath(""));
				properties.Add(MOG.MOG_PropertyFactory.MOG_Asset_InfoProperties.New_AssetIcon(""));
				properties.Add(MOG.MOG_PropertyFactory.MOG_Classification_InfoProperties.New_ClassIcon(""));

				if (listDialog.ListSearchAllCheckBox.Checked)
				{
					CurrentAssets = GetAllAssetsList(tag);
					total = FilterAssetListByTreePosition(CurrentAssets, rootBranch);
				}
				else
				{
					CurrentAssets = GetAssetsList(tag, properties);
					total = CurrentAssets.Count;
				}

				listForm.ListListView.BeginUpdate();

				cancelled = PopulateReportFromCurrentAssets(CurrentAssets, listDialog, listForm, progress, properties, total);

				listForm.ListListView.EndUpdate();

				// Cleanup dialog
				progress.DialogKill();
				progress = null;

				if (!cancelled)
				{
					listForm.UpdateAssetTotals();
					return true;
				}
			}

			return false;
		}

		private bool PopulateReportFromCurrentAssets(ArrayList CurrentAssets, NewListForm listDialog, ListForm listForm, CallbackDialogForm progress, ArrayList properties, int total)
		{
			int i = 0;
			bool cancelled = false;

			foreach (object currentAsset in CurrentAssets)
			{
				MOG_Filename mogFilename = null;
				ArrayList mogProperties = null;

				if (currentAsset is MOG_DBAssetProperties)
				{
					MOG_DBAssetProperties dbAssetProperties = currentAsset as MOG_DBAssetProperties;

					MOG_Time assetTime = new MOG_Time(dbAssetProperties.mAsset.GetVersionTimeStamp());
					
					// Generate the asset full name with properties for the filtering functions
					string assetName = "";
					assetName += dbAssetProperties.mAsset.GetOriginalFilename();
					foreach (MOG_Property property in dbAssetProperties.mProperties)
					{
						assetName += "." + property.mValue;
					}

					// Check if this asset is filtered?
					if (ListCreateDateRangeCheck(assetName, listDialog, assetTime))
					{
						mogFilename = dbAssetProperties.mAsset;
						mogProperties = dbAssetProperties.mProperties;
					}
				}
				else if (currentAsset is MOG_Filename)
				{
					mogFilename = currentAsset as MOG_Filename;
					mogProperties = properties;
				}
				else
				{
					//we have a problem, report and return...
					MOG_Report.ReportMessage(GenerateReport_MenuItemText, "Invalid type for generating report: " + currentAsset.GetType().UnderlyingSystemType.ToString(), Environment.StackTrace, MOG_ALERT_LEVEL.ERROR);
					cancelled = true;
					break;
				}

				if (mogFilename != null)
				{
					// Exclude a ton of items because it wastes too much time to show everything
					if (i++ % 10 == 0)
					{
						// Update our progress
						progress.DialogUpdate((i * 100) / total, string.Concat("Checking...\n", mogFilename.GetAssetName()));

						// Check for cancel
						cancelled = progress.DialogProcess();
						if (cancelled)
						{
							cancelled = true;
							break;
						}
					}

					if (mogProperties != null)
					{
						MOG_Properties pProperties = new MOG_Properties(mogProperties);
						listForm.ItemAdd(mogFilename, pProperties, "");
					}
				}
			}

			return cancelled;
		}

		private bool ListCreateDateRangeCheck(string asset, NewListForm listDialog, MOG_Time assetTime)
		{
			bool passed = true;

			// Only preform the check if our date range checkBox is true
			if (listDialog.ListDateRangeRadioButton.Checked)
			{
				MOG_Time startTime = new MOG_Time();
				MOG_Time endTime = new MOG_Time();

				startTime.FromDateTime(listDialog.ListStartDateTimePicker.Value);
				endTime.FromDateTime(listDialog.ListEndDateTimePicker.Value);

				if (listDialog.ListStartDateTimePicker.Checked == false)
				{
					startTime.SubtractYears(50);
				}
				
				if (listDialog.ListEndDateTimePicker.Checked == false)
				{
					endTime.AddYears(50);
				}

				if (startTime.Compare(endTime) == 0)
				{
					endTime.AddHours(23.9);
				}

				if (!(assetTime.Compare(startTime) >= 1 && assetTime.Compare(endTime) <= -1))
				{
					passed = false;
				}
			}
			else if (listDialog.ListTimeRangeRadioButton.Checked)
			{
				MOG_Time startTime = new MOG_Time();
				MOG_Time endTime = new MOG_Time();
				
				startTime.SubtractHours(Convert.ToDouble(listDialog.ListHoursTextBox.Text));
								
				if (!(assetTime.Compare(startTime) >= 1 && assetTime.Compare(endTime) <= -1))
				{
					passed = false;
				}
			}

			if (listDialog.ListFilterCheckBox.Checked)
			{
				passed &= ListCreateFilterCheck(asset, listDialog);
			}

			return passed;
		}

		private bool ListCreateFilterCheck(string asset, NewListForm listDialog)
		{
			bool retVal = true;
			string []searchStrings = listDialog.ListFilterTextBox.Text.Split(new Char[] {','});

			foreach (string searchString in searchStrings)
			{
				
				if (listDialog.ListInclusionRadioButton.Checked)
				{
					retVal = true;
				}
				else if (listDialog.ListExclusionRadioButton.Checked)
				{
					retVal = false;
				}
				
				// Check if there is a match
				if (asset.ToLower().IndexOf(searchString.Trim().ToLower()) >= 0)
				{
					return retVal;
				}
			}
				
			return !retVal;
		}

		/// <summary>
		/// Change properties event handler
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MenuItemChangePropertiesAdd_Click(object sender, System.EventArgs e)
		{
			ArrayList selectedItems = ControlGetSelectedItemTags();
			MOG_Filename filename;
			string classification = ""; ;
			MOG_Properties properties = null;
			bool isClassification = false;
			
			foreach (Mog_BaseTag tag in selectedItems)
			{
				if (tag.Execute)
				{
					// Check if this asset is in the repository?
					MOG_Filename testName = new MOG_Filename(tag.FullFilename.ToLower());
					filename = new MOG_Filename(tag.FullFilename);
					properties = new MOG_Properties(filename);
				}
				else if (tag.Owner is TreeNode && ((TreeNode)tag.Owner).TreeView != null)
				{
					isClassification = true;
					// This will handle the classification nodes by resolving the full classification name from the tree itsself
					classification = ((TreeNode)tag.Owner).FullPath;
					properties = new MOG_Properties(classification);					
				}
			}

			// Show the wizard
			AddRipPropertyWizard wizard = new AddRipPropertyWizard(properties);
			if (wizard.ShowDialog() == DialogResult.OK)
			{
				// Save this new menu property change onto all the assets or classes
				if (isClassification)
				{
					MOG_Properties classProperties = MOG_Properties.OpenClassificationProperties(classification);

					// Update all selected assets with changes
					classProperties.PropertyMenu = wizard.PropertyMenu;
					classProperties.Save();
				}
				else
				{
					// Save the property change to each of the selected assets
					foreach (Mog_BaseTag tag in selectedItems)
					{
						if (tag.Execute)
						{
							filename = new MOG_Filename(tag.FullFilename);
							// Check to see if this is a blessed asset
							filename = FixupAssetPath("Change asset properties", filename);

							// Make sure we are an asset before showing log
							if (filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
							{
								MOG_ControllerAsset assetController = MOG_ControllerAsset.OpenAsset(filename);
								if (assetController != null)
								{
									// Update all selected assets with changes
									assetController.GetProperties().PropertyMenu = wizard.PropertyMenu;

									// Close the asset's controller
									assetController.Close();
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Change properties event handler
		/// </summary>
		private void MenuItemChangeProperties_Click(object sender, System.EventArgs e)
		{
			try
			{
				ArrayList selectedItems = ControlGetSelectedItemTags();
				MOG_Ini specialMenu = null;
				ToolStripMenuItem item = (ToolStripMenuItem)sender;
				string command = "";
				bool bRepositoryAsset = false;
				bool bIsClassification = false;

				MOG_Filename filename;
				MOG_Properties properties;

				foreach (Mog_BaseTag tag in selectedItems)
				{
					if (tag.Execute)
					{
						// Check if this asset is in the repository?
						MOG_Filename testName = new MOG_Filename(tag.FullFilename.ToLower());
						if (testName.IsWithinRepository())
						{
							bRepositoryAsset = true;
						}

						filename = new MOG_Filename(tag.FullFilename);
						properties = new MOG_Properties(filename);
					}
					else if (tag.Owner is TreeNode && ((TreeNode)tag.Owner).TreeView != null)
					{
						// This will handle the classification nodes by resolving the full classification name from the tree itsself
						string fullFilename = ((TreeNode)tag.Owner).FullPath;
						properties = new MOG_Properties(fullFilename);
						bIsClassification = true;
					}

					if (specialMenu == null)
					{
						#region Get label and command for properties.info
						string propertyPath = "";
						try
						{
							ToolStripMenuItem tempItem = item;

							while (tempItem != null)
							{
								// Signifies we are at the top of the tree
								if (MOG.StringUtils.StringCompare(tempItem.Text, "Change Properties*"))
								{
									break;
								}

								if (propertyPath.Length == 0)
								{
									propertyPath = tempItem.Text;
								}
								else
								{
									propertyPath = tempItem.Text + "/" + propertyPath;
								}

								tempItem = (ToolStripMenuItem)tempItem.OwnerItem;
							}
						}
						catch
						{
						}

						command = (string)mPropertyMenu[propertyPath];
						#endregion
					}
				}

				// Make sure we have a proprty to do anything with
				if (!string.IsNullOrEmpty(command))
				{
					// Verify that the command string has the correct number of parameters
					string[] leftParts = command.Split("=".ToCharArray());
					if (leftParts.Length < 2)
					{
						string title = "Change Properties Failed";
						string message = "Invalid property format specified.\n" +
										 "Specified Format: " + command + "\n" +
										 "   Proper Format: [Section]{PropertyGroup}PropertyName=PropertyValue";
						MOG_Prompt.PromptMessage(title, message);
						return;
					}

					string[] testParts = leftParts[0].Split("[]{}".ToCharArray());
					if (testParts.Length != 5)
					{
						string title = "Change Properties Failed";
						string message = "Invalid property format specified.\n" +
										 "SPECIFIED FORMAT: " + command + "\n" +
										 "   Proper Format: [Section]{PropertyGroup}PropertyName=PropertyValue";
						MOG_Prompt.PromptResponse(title, message);
						return;
					}

					// Check if this property should be cleared?
					bool bClearProperty = false;
					// Check if this item was already checked?
					if (item.Checked)
					{
						// Ask the user if they want to clear this property?
						string title = "Clear Property?";
						string message = "Is it your intention to toggle this property so that gets cleared?\n" +
										 "PROPERTY: " + item.Text + "\n\n" +
										 "Would you like to have this property cleared?";
						if (MOG_Prompt.PromptResponse(title, message, MOGPromptButtons.YesNo) == MOGPromptResult.Yes)
						{
							bClearProperty = true;
						}
					}

					// Create our list of assets
					ArrayList assets = new ArrayList();
					foreach (Mog_BaseTag tag in selectedItems)
					{
						if (tag.Execute)
						{
							// Convert the full file name into a MOG_Filename
							MOG_Filename asset = new MOG_Filename(tag.FullFilename);

							// Add to our asset list
							assets.Add(asset);
						}
						else if (tag.Owner is TreeNode && ((TreeNode)tag.Owner).TreeView != null)
						{
							// Get the class name
							string classification = ((TreeNode)tag.Owner).FullPath;

							// Get all the assets in this class
							ArrayList assetsInClass = MOG_ControllerAsset.GetAssetsByClassification(classification);

							// Add all these assets to our assets list
							foreach (MOG_Filename asset in assetsInClass)
							{
								assets.Add(asset);
							}

							// Now save this property change to the classification
							string section, propertySection, key, val;
							leftParts = command.Split("=".ToCharArray());
							if (leftParts.Length != 2)
							{
								string title = "Change Properties Failed";
								string message = "Invalid property format specified.\n" +
												 "Specified Format: " + command + "\n" +
												 "   Proper Format: [Section]{PropertyGroup}PropertyName=PropertyValue";
								MOG_Prompt.PromptMessage(title, message);
								return;
							}
							string[] parts = leftParts[0].Split("[]{}".ToCharArray());
							if (parts.Length == 5)
							{
								try
								{
									section = parts[1];
									propertySection = parts[3];
									key = parts[4];
									val = command.Substring(command.IndexOf("=") + 1);
								}
								catch
								{
									string title = "Change Properties Failed";
									string message = "Invalid property format specified.\n" +
													 "Specified Format: " + command + "\n" +
													 "   Proper Format: [Section]{PropertyGroup}PropertyName=PropertyValue";
									MOG_Prompt.PromptMessage(title, message);
									return;
								}

								MOG_Properties classProperties = MOG_Properties.OpenClassificationProperties(classification);

								// Update selected properties with change
								// Check if we want to clear the property?
								if (bClearProperty)
								{
									// Check if this property was inherited?
									if (classProperties.IsPropertyAlreadyInherited(section, propertySection, key, val))
									{
										// Set it to be clear
										classProperties.SetProperty(section, propertySection, key, "None");
									}
									else
									{
										// Remove the property
										classProperties.RemoveProperty(section, propertySection, key);
									}
								}
								else
								{
									// Set the new property
									classProperties.SetProperty(section, propertySection, key, val);
								}
								classProperties.Save();
							}

						}
					}

					// Check if we encountered a repository or classification asset?
					if (bRepositoryAsset || bIsClassification)
					{
						// Check if there were any assets found under this classification?
						if (assets.Count > 0)
						{
							// Confirm that the user wants to send all the assets to their inbox
							string title = "Change Asset Properties?";
							string message = "These assets ( " + assets.Count + ") will need be copied to your MOG Inbox before the property change can occur on these assets.\n\n" +
												"Would you like to proceed?";
							if (MOG_Prompt.PromptResponse(title, message, MOGPromptButtons.YesNo) == MOGPromptResult.No)
							{
								return;
							}
						}
					}

					ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
					List<object> args = new List<object>();
					args.Add(assets);
					args.Add(command);
					args.Add(bClearProperty);
					ProgressDialog progress = new ProgressDialog("Changing Properties", "Please wait while the assets are modified within your inbox.", ChangeProperty_Worker, args, true);
					Control sourceControl = SourceControl != null ? sourceControl = SourceControl : sourceControl = MogMainForm.MainApp.TopLevelControl;
					progress.ShowDialog(sourceControl.TopLevelControl);
				}
			}
			catch (Exception ex)
			{
				MOG_Report.ReportMessage(ChangeProperties_MenuItemText, ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
			}
		}

		private void ChangeProperty_Worker(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			List<object> args = e.Argument as List<object>;
			ArrayList assets = args[0] as ArrayList;
			string command = args[1] as string;
			bool bClearProperty = (bool)args[2];

			// Copy each of the assets to the users inbox with the changed settings...
			for (int f = 0; f < assets.Count && !worker.CancellationPending; f++)
			{
				MOG_Filename assetFilename = assets[f] as MOG_Filename;

				// Check to see if this is a blessed asset
				MOG_Filename fixedAssetFilename = FixupAssetPath("Change asset properties", assetFilename);

				// Make sure we are an asset before showing log
				if (fixedAssetFilename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
				{
					MOG_ControllerAsset assetController = MOG_ControllerAsset.OpenAsset(fixedAssetFilename);
					if (assetController != null)
					{
						string message = "Adding:\n" +
										 "     " + fixedAssetFilename.GetAssetClassification() + "\n" +
										 "     " + fixedAssetFilename.GetAssetName();
						worker.ReportProgress(f * 100 / assets.Count, message);

						// Check if this asset is in the repository?
						if (fixedAssetFilename.IsWithinRepository())
						{
							// Copy the asset to the user's drafts before applying the new property
							MOG_Filename targetFilename = new MOG_Filename(assetController.GetAssetFilename().GetAssetEncodedInboxPath("Drafts"));
							if (MOG_ControllerInbox.Copy(assetController, targetFilename, MOG_AssetStatusType.Modifying, worker))
							{
								// Close the repository asset
								assetController.Close();
								// Open the newly copied asset and apply the property change
								assetController = MOG_ControllerAsset.OpenAsset(targetFilename);
							}
						}
						// Check if this asset is already within the inboxes?
						if (fixedAssetFilename.IsWithinInboxes())
						{
							// Check if the asset is outside of the drafts?
							if (!fixedAssetFilename.IsDrafts())
							{
								// Move the asset to the user's drafts before we apply the new property change
								MOG_Filename targetFilename = new MOG_Filename(assetController.GetAssetFilename().GetAssetEncodedInboxPath("Drafts"));
								MOG_ControllerInbox.Move(assetController, targetFilename, MOG_AssetStatusType.Modifying);
							}
						}

						// Apply the property change
						if (assetController != null)
						{
							string section, propertySection, key, val;
							string[] parts = command.Substring(0, command.IndexOf("=")).Split("[]{}".ToCharArray());
							if (parts.Length == 5)
							{
								section = parts[1];
								propertySection = parts[3];
								key = parts[4];
								val = command.Substring(command.IndexOf("=") + 1);

								// Update all selected assets with change
								// Check if we want to clear the property?
								if (bClearProperty)
								{
									// Check if this property was inherited?
									if (assetController.GetProperties().IsPropertyAlreadyInherited(section, propertySection, key, val))
									{
										// Set it to be clear
										assetController.GetProperties().SetProperty(section, propertySection, key, "None");
									}
									else
									{
										assetController.GetProperties().RemoveProperty(section, propertySection, key);
									}
								}
								else
								{
									assetController.GetProperties().SetProperty(section, propertySection, key, val);
								}

								// Update the inbox views
								if (assetController.GetProperties().NativeDataType)
								{
									MOG_ControllerInbox.UpdateInboxView(assetController, MOG_AssetStatusType.Processed);
								}
								else
								{
									MOG_ControllerInbox.UpdateInboxView(assetController, MOG_AssetStatusType.Modified);
								}
							}
						}

						// Close the asset's controller
						assetController.Close();
					}
				}
			}
		}

		private void MenuItemAddClassification_Click(object sender, EventArgs e)
		{
			MOG_Privileges privs = MOG_ControllerProject.GetPrivileges();
			if (privs.GetUserPrivilege(MOG_ControllerProject.GetUserName(), MOG_PRIVILEGE.AddClassification))
			{
				try
				{
					ArrayList selectedItems = ControlGetSelectedItemTags();
					foreach (Mog_BaseTag tag in selectedItems)
					{
						try
						{
							// Get our current classification name and project
							string classification = ((TreeNode)tag.Owner).FullPath;

							// Create a form for user input
							ClassificationCreateForm ccf = new ClassificationCreateForm(classification);

							// Show our form
							if (ccf.ShowDialog(this.mTree) == DialogResult.OK)
							{
								//GLK:  What I really should be doing here is de-initializing any open treeViews (in Project Tab)...
								RefreshTreeView(tag);
							}
						}
						catch (InvalidCastException ex)
						{
							throw new Exception(ClassificationAddDelete_MessageText, ex);
						}
						catch (Exception ex)
						{
							throw new Exception("Problem finding valid TreeView. " + ClassificationAddDelete_MessageText, ex);
						}
					}
				}
				catch (Exception ex)
				{
					MOG_Report.ReportMessage(AddClassification_MenuItemText, ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
				}
			}
			else
			{
				MOG_Prompt.PromptResponse("Insufficient Privileges", "Your privileges do not allow you to add classifications to the project.");
			}
		}

		private void MenuItemRenameClassification_Click(object sender, EventArgs e)
		{
			try
			{
				ArrayList selectedItems = ControlGetSelectedItemTags();
				foreach( Mog_BaseTag tag in selectedItems )
				{
					try
					{
						if( tag.Owner != null && tag.Owner is TreeNode && ((TreeNode)tag.Owner).TreeView is MogControl_BaseTreeView)
						{
							MogControl_BaseTreeView view = (MogControl_BaseTreeView)((TreeNode)tag.Owner).TreeView;
							if (view.SelectedNode != null)
							{
								view.LabelEdit = true;
								view.SelectedNode.BeginEdit();
							}
						}						
					}
					catch( InvalidCastException ex )
					{
						throw new Exception(ClassificationAddDelete_MessageText, ex );
					}
					catch( Exception ex )
					{
						throw new Exception("Problem finding valid TreeView. " + ClassificationAddDelete_MessageText, ex );
					}
				}
			}
			catch(Exception ex)
			{
				MOG_Report.ReportMessage(AddClassification_MenuItemText, ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
			}
		}

		private void RefreshTreeView( Mog_BaseTag tag )
		{
			if( tag.Owner != null && tag.Owner is TreeNode && ((TreeNode)tag.Owner).TreeView is MogControl_BaseTreeView)
			{
				MogControl_BaseTreeView view = (MogControl_BaseTreeView)((TreeNode)tag.Owner).TreeView;
				view.DeInitialize();
				view.Initialize();
			}
		}

		private void MenuItemDeleteClassification_Click(object sender, EventArgs e)
		{
			try
			{
				ArrayList selectedItems = ControlGetSelectedItemTags();
				foreach( Mog_BaseTag tag in selectedItems )
				{
					if( MOGPromptResult.Yes == MOG_Prompt.PromptResponse( DeleteClassification_MenuItemText, "Are you sure "
						+ "you wish to delete classification, '" + ((TreeNode)tag.Owner).FullPath + "'?", MOGPromptButtons.YesNo))
					{
						try
						{
							bool successful = false;
							string classification = ((TreeNode)tag.Owner).FullPath;

							try
							{
								successful = MOG_ControllerProject.GetProject().ClassificationRemove( classification );
							}
							catch(MOG_Exception ex)
							{
								// Check if this was due to dependant data?
								if (ex.GetType() == MOG_Exception.MOG_EXCEPTION_TYPE.MOG_EXCEPTION_OutstandingDependency)
								{
									// Let's ask the user if they still want to purge an empty classifications
									string message = "Dependant assets within this classification are preventing a full removal.\n" + 
													 "   CLASSIFICATION: " + classification + "\n\n" +
													 "Would you still like MOG to remove any empty children classifciations?";
									if (MOG_Prompt.PromptResponse("Do you still want to remove any empty children classifications?", message, MOGPromptButtons.YesNo) == MOGPromptResult.Yes)
									{
										ArrayList childClassifications = MOG_DBAssetAPI.GetAllActiveClassificationsByRootClassification(classification);
										foreach (string childClassification in childClassifications)
										{
											// Check if this classification still exists?
											// This is just in case an earlier remove purged this one as well
											if (MOG_ControllerProject.GetProject().ClassificationExists(childClassification))
											{
												try
												{
													// Try to remove this childClassification
													MOG_ControllerProject.GetProject().ClassificationRemove(childClassification);
												}
												catch
												{
													// Eat all errors
												}
											}
										}
									}
								}
								else
								{
									// Show the user the raw exception
									MOG_Prompt.PromptMessage(AddClassification_MenuItemText, ex.Message);
								}
							}
							
							if( successful )
							{
								tag.ItemRemove();
							}
						}
						// Catch if we accidentally allow DeleteClassification from a ListView item...
						catch( InvalidCastException castException )
						{
							throw new Exception(ClassificationAddDelete_MessageText, castException );
						}
					}				
				}
			}
			catch(Exception ex)
			{
				MOG_Report.ReportMessage(AddClassification_MenuItemText, ex.Message, ex.StackTrace, MOG_ALERT_LEVEL.CRITICAL);
			}
		}
		#endregion CLICK HANDLES

		#region OLD UTILITY FUNCTIONS
		/// <summary>
		/// FixupAssetPath takes a mog_filename and determines if it is in the blessed tree.  If so
		/// it updates the filename to have the version number of the current version.
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		private MOG_Filename FixupAssetPath(string ErrorTitle, MOG_Filename filename)
		{
			// Check if we are a blessed asset
			if (filename.IsBlessed())
			{
				// Check if we are missing a version timestamp
				if (filename.GetVersionTimeStamp().Length == 0)
				{
					// Attempt to obtain the current version of this asset for our branch
					string newTimeStamp = MOG_ControllerProject.GetAssetCurrentVersionTimeStamp(filename);
					if (newTimeStamp.Length == 0)
					{
						// Get all revisions of this asset and lets use the topmost one...
						ArrayList revisions = MOG_DBAssetAPI.GetAllAssetRevisions(filename);
						if (revisions != null && revisions.Count > 0)
						{
							// Put oldest first by sorting
							revisions.Sort(null);
							newTimeStamp = (string)revisions[revisions.Count - 1];
						}
					}

					// Generate a path to the blessed asset of this newTimeStamp
					filename = MOG_ControllerRepository.GetAssetBlessedVersionPath(filename, newTimeStamp);
				}
			}
			else
			{
				// Check if we already have a version we can use
				if (filename.GetVersionTimeStamp().Length > 0)
				{
					// Create the path to the blessed asset of this version
					filename = MOG_ControllerRepository.GetAssetBlessedVersionPath(filename, filename.GetVersionTimeStamp());
				}
			}

			return filename;
		}

		/// <summary>
		/// This fuction returns a filtered list based on the level of item selected in a project assetTree
		/// </summary>
		/// <param name="tag"></param>
		/// <returns>ArrayList</returns>
		private ArrayList GetAssetsList(Mog_BaseTag tag, ArrayList properties)
		{
			ArrayList CurrentAssets = new ArrayList();

			if (tag.Owner is TreeNode && ((TreeNode)tag.Owner).TreeView != null)
			{
				string root = ((TreeNode)tag.Owner).Text;
				string fullPath = ((TreeNode)tag.Owner).FullPath;
                TreeView view = ((TreeNode)tag.Owner).TreeView;
				
				// Get just what we need from the database
				switch (tag.RepositoryFocus)
				{
					case RepositoryFocusLevel.Classification:
                        if( view is MogControl_ArchivalTreeView )
                        {
							CurrentAssets = MOG_DBReports.RunReport_GetArchiveAssetsByClassification(fullPath, properties);
							//CurrentAssets = MOG_DBAssetAPI.GetAllArchivedAssetsByParentClassificationWithProperties(fullPath, properties);
                        }
                        else if( view is MogControl_FullTreeView || view is MogControl_PackageTreeView )
                        {
							CurrentAssets = MOG_DBReports.RunReport_GetActivePackagesByClassification(fullPath, properties);
                           //CurrentAssets = MOG_DBAssetAPI.GetAllAssetsByParentClassificationWithProperties(fullPath, properties);
                        }
                        else if( view is MogControl_SyncTargetTreeView )
                        {
                            string platform = "";
                            string gamePath = "";
							// Get the first three parts of our full path
                            string []parts = fullPath.Split("~".ToCharArray(), 3);
							// Get our platform and path from our parts...
                            if (parts != null && parts.Length == 3)
                            {
                                platform = parts[1];
                                gamePath = parts[2].Replace("~", "\\");
                            }
								// Else, just get our platform
                            else if (parts != null && parts.Length == 2)
                            {
                                platform = parts[1];
                                gamePath = "";									
                            }
								// Else, instead of warning the user, we're going to find a way to disable this option...
                            else
                            {
								MOG_Prompt.PromptResponse("Generate Report", "Cannot generate report at this level!");
                                break;
                            }
// JohnRen - Removed because this would show many many duplicates and unrelated platform assets
//							CurrentAssets = MOG_DBReports.RunReport_GetActiveAssetsBySyncTarget(gamePath, platform, properties);
							CurrentAssets = MOG_DBAssetAPI.GetAllAssetsBySyncLocationWithProperties(gamePath, platform, properties);
                        }
//                      else if( view is MogControl_PackageTreeView )
//                      {
//                          MOG_Report.ReportMessage("Generate Report", "Reports off of the package view are not yet implemented!");
//                      }
						else
						{
							CurrentAssets = MOG_DBReports.RunReport_GetAssetsByClassification(fullPath, properties, true);
						}
						break;
					case RepositoryFocusLevel.Repository:
						MOG_Filename assetName = new MOG_Filename(tag.FullFilename);
						MOG_DBAssetProperties assetDBName = new MOG_DBAssetProperties();
						MOG_Properties mogAssetProperties = new MOG_Properties( assetName );

						ArrayList assetProperties = new ArrayList();
						assetProperties.Add(MOG.MOG_PropertyFactory.MOG_Asset_StatsProperties.New_Size(0));
						assetProperties.Add(MOG.MOG_PropertyFactory.MOG_Asset_InfoProperties.New_IsPackage(mogAssetProperties.IsPackage));
						assetProperties.Add(MOG.MOG_PropertyFactory.MOG_Asset_StatsProperties.New_Owner(""));
						assetProperties.Add(MOG.MOG_PropertyFactory.MOG_Asset_StatsProperties.New_Creator(""));
						assetProperties.Add(MOG.MOG_PropertyFactory.MOG_Asset_StatsProperties.New_SourceMachine(""));
                        assetProperties.Add(MOG.MOG_PropertyFactory.MOG_Sync_OptionsProperties.New_SyncTargetPath(""));

						// Get the latest version of this asset, if it does not have a version?
						if (assetName.GetVersionTimeStamp().Length != 0)
						{
							if(assetName.IsBlessed())
							{
								assetProperties.Add(MOG.MOG_PropertyFactory.MOG_Asset_StatsProperties.New_CreatedTime(MOG_DBAssetAPI.GetAssetVersion(assetName)));
							}
						}
						
                        // If we have an "All Revisions" node get all revisions...
						if( view is MogControl_ArchivalTreeView && ((TreeNode)tag.Owner).Text == MogControl_BaseTreeView.Revisions_Text )
						{
							// Get a detailed report of all revisions for this asset
							CurrentAssets = MOG_DBReports.RunReport_GetAllRevisionsOfAsset(assetName, properties);
						}
						else if(mogAssetProperties.IsPackage)
						{
							string revision = assetName.GetVersionTimeStamp();
							if (revision.Length == 0)
							{
								revision = MOG_ControllerProject.GetAssetCurrentVersionTimeStamp(assetName);
								assetName = MOG_ControllerRepository.GetAssetBlessedVersionPath(assetName, revision);
							}
							CurrentAssets = MOG_DBReports.RunReport_GetPackageAssets(assetName, properties, assetName.GetAssetPlatform());
						}
						else
						{
							// Get a detailed report of all revisions for this asset
							CurrentAssets = MOG_DBReports.RunReport_GetAllRevisionsOfAsset(assetName, properties);
                        }
						break;
					default:
						// Now walk through our current branch ASSETS section and build our list
						CurrentAssets = MOG_DBAssetAPI.GetAllAssets();
						break;
				}						
			}
			else
			{
				// Since we know this is not a tree node, lets assume this could be an asset?
				MOG_Filename assetFilename = new MOG_Filename(tag.FullFilename);
				if (assetFilename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
				{
					// Get a detailed report of all revisions for this asset
					CurrentAssets = MOG_DBReports.RunReport_GetAllRevisionsOfAsset(assetFilename, properties);
				}
			}

			return CurrentAssets;
		}

		/// <summary>
		/// This fuction returns a filtered list of all assets in the database that have a field that matches the level of item selected in a project assetTree
		/// </summary>
		/// <param name="tag"></param>
		/// <returns>ArrayList</returns>
		private ArrayList GetAllAssetsList(Mog_BaseTag tag)
		{
			ArrayList CurrentAssets = new ArrayList();
// TODO - Rewite this!
//			if (tag.mItemPtr.GetType() == new TreeNode().GetType())
//			{
//				string root = ((TreeNode)tag.mItemPtr).Text;
//					
//				// Get just what we need from the database
//				switch (tag.Level)
//				{
//					case RepositoryFocusLevelTREE_FOCUS.KEY:
//						CurrentAssets = MOG_DBAssetAPI.GetAllAssetsByType(root);
//						break;
//					case RepositoryFocusLevelTREE_FOCUS.CLASS:
//						CurrentAssets = MOG_DBAssetAPI.GetAllAssetsByGroup(root);
//						break;
//					case RepositoryFocusLevelTREE_FOCUS.SUBCLASS:
//						CurrentAssets = MOG_DBAssetAPI.GetAllAssetsBySubGroup(root);
//						break;
//					case RepositoryFocusLevelTREE_FOCUS.LABEL:
//						break;
//					default:
//						// Now walk through our current branch ASSETS section and build our list
//						CurrentAssets = MOG_DBAssetAPI.GetAllAssets();
//						break;
//				}						
//			}
//			else
//			{
//				// Now walk through our current branch ASSETS section and build our list
//				CurrentAssets = MOG_DBAssetAPI.GetAllAssets();
//			}

			return CurrentAssets;
		}

		/// <summary>
		/// Calulates total size and removes assets that dont match the filter criteria
		/// </summary>
		/// <param name="assets"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		private int FilterAssetList(ArrayList assets, string filter)
		{
			// Quickly calculate our totals
			for (int i = 0; i < assets.Count;)
			{
				MOG_Filename currentAsset = (MOG_Filename)assets[i];
				if (currentAsset.GetAssetName().IndexOf(filter) != -1)
				{
					i++;
				}
				else
				{
					assets.RemoveAt(i);					
				}
			}

			return assets.Count;
		}

		/// <summary>
		/// Calulates total size and removes assets that dont match the filter criteria as determined by the selected tree node
		/// </summary>
		/// <param name="assets"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		private int FilterAssetListByTreePosition(ArrayList assets, string treePath)
		{
			// Get the path of this node
			string rootBranch = treePath.ToLower();

			// Replace the slash and convert the project name with the project key
			rootBranch = rootBranch.Replace("\\", ".");
			rootBranch = rootBranch.Replace(MOG_ControllerProject.GetProjectName().ToLower(), MOG_ControllerProject.GetProjectName().ToLower());

			// Quickly calculate our totals
			for (int i = 0; i < assets.Count;)
			{
				MOG_Filename currentAsset = (MOG_Filename)assets[i];
				if (currentAsset.GetAssetName().IndexOf(rootBranch) != -1)
				{
					i++;
				}
				else
				{
					assets.RemoveAt(i);					
				}
			}

			return assets.Count;
		}
		#endregion OLD UTILITY FUNCTIONSM
	}

	#region Helper Classes
	/// <summary>
	/// Inherits from MenuItem to add Tag attribute (standard in most other controls)
	/// </summary>
	public class MogControl_MenuItem : ToolStripMenuItem
	{
		#region Member variables, Properties
		private bool bIsEmpty;
		public bool IsEmpty
		{
			get{	return bIsEmpty;	}
		}
		#endregion Member variables, Properties

		public MogControl_MenuItem()
			: this("")
		{
			this.bIsEmpty = true;
		}

		public MogControl_MenuItem(string text)
			: this(text, null)
		{
			this.bIsEmpty = true;
		}

		public MogControl_MenuItem(string text, object tag)
			:base(text)
		{
			this.Tag = tag;
			this.bIsEmpty = false;
		}
	}

	public class guiAssetMenuPrivileges
	{
		private string mName;
		private bool mUserExclusive;
		private bool mAssetExclusive;
		private bool mItemRequired;
		private bool mClassificationExclusive;
        private bool mLightVersionDisable;
		private MOG_PRIVILEGE mMogPrivilege;        

		public bool ClassificationExclusive{ get{return mClassificationExclusive;} set{mClassificationExclusive = value;}}
		public string Name { get{return mName;} set{mName = value;}}
		public bool UserExclusive { get{return mUserExclusive;} set{mUserExclusive = value;}}
		public bool AssetExclusive { get{return mAssetExclusive;} set{mAssetExclusive = value;}}
		public bool ItemRequired { get{return mItemRequired;} set{mItemRequired = value;}}
        public bool LightVersionDisabled { get { return mLightVersionDisable; } set { mLightVersionDisable = value; } }
		public MOG_PRIVILEGE MogPrivilege{ get{return mMogPrivilege;} set{mMogPrivilege = value;}}

		public guiAssetMenuPrivileges(string name, MOG_PRIVILEGE privilege, bool user, bool asset, bool classificationExclusive, bool selectedItemRequired, bool lightVersionDisabled) 
		{ 
			Name = name; 
			mMogPrivilege = privilege; 
			UserExclusive = user; 
			AssetExclusive = asset;
			ClassificationExclusive = classificationExclusive;
			mItemRequired = selectedItemRequired;
            mLightVersionDisable = lightVersionDisabled;
		}
        public guiAssetMenuPrivileges(string name, MOG_PRIVILEGE privilege, bool user, bool assetExclusive, bool classificationExclusive, bool selectedItemRequired)
            : this(name, privilege, user, assetExclusive, classificationExclusive, selectedItemRequired, false)
        {
        }
		public guiAssetMenuPrivileges(string name, MOG_PRIVILEGE privilege, bool user, bool assetExclusive, bool classificationExclusive ) 
			: this( name, privilege, user, assetExclusive, classificationExclusive, true, false )
		{			
		}
		public guiAssetMenuPrivileges(string name, MOG_PRIVILEGE privilege, bool user, bool assetExclusive ) 
			: this( name, privilege, user, assetExclusive, false, true, false )
		{			
		}
	}
	#endregion Helper Classes
}
