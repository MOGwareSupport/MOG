using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Reflection;

using MOG_Client.Client_Gui.ProjectManager_Helper;
using MOG_Client.Client_Mog_Utilities;
using MOG_Client.Client_Mog_Utilities.AssetOptions;
using MOG_Client.Forms;
using MOG_Client.Client_Utilities;
using MOG_ControlsLibrary.Common.MogControl_RepositoryTreeViews;
using MOG_ControlsLibrary.Common.MOG_ContextMenu;
using MOG_ControlsLibrary.Utils;

using MOG;
using MOG.INI;
using MOG.TIME;
using MOG.PROJECT;
using MOG.PLATFORM;
using MOG.PROPERTIES;
using MOG.COMMAND;
using MOG.USER;
using MOG.REPORT;
using MOG.DOSUTILS;
using MOG.FILENAME;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG_ControlsLibrary.MogUtils_Settings;
using MOG_ControlsLibrary.Controls;
using MOG.CONTROLLER.CONTROLLERREPOSITORY;
using System.Collections.Generic;
using MOG.PROMPT;


// ===============================================
// Project Manager
// ===============================================

namespace MOG_Client.Client_Gui
{
	/// <summary>
	/// Summary description for ProjectManager.
	/// </summary>
	public class guiProjectManager
	{
		private MogMainForm mainForm;
		private ImageList mSmallImageList	= new ImageList();
		private ImageList mLargeImageList	= new ImageList();
		private ArrayList mAssetTypes		= new ArrayList();
		private bool mLoaded = false;

		public enum TaskColumns {TITLE, STARTDATE, DUEDATE, TIME, STATUS, CREATOR, ASSIGNEDTO, ASSET};
		public enum AssetFields {KEY, PACKAGE, GROUP, LABEL, COUNT};

		public guiProjectManager(MogMainForm main)
		{
			mainForm = main;

			ProjectTreeSetFilter(MogUtils_Settings.LoadSetting_default("ProjectTreeFilter", "current", "Classification"));

			Initialize();

			mLoaded = true;
		}

		public void Initialize()
		{
			if (!MOG_ControllerProject.IsProject() || !MOG_ControllerProject.IsUser())
			{
				return;
			}
			
			MogControl_AssetContextMenu classificationTreeContextMenu = new MogControl_AssetContextMenu(mainForm.ProjectManagerClassificationTreeView);
			MogControl_AssetContextMenu packageTreeContextMenu = new MogControl_AssetContextMenu(mainForm.ProjectManagerPackageTreeView);
            MogControl_AssetContextMenu syncTargetContextMenu = new MogControl_AssetContextMenu(mainForm.ProjectManagerSyncTargetTreeView);
            MogControl_AssetContextMenu archiveContextMenu = new MogControl_AssetContextMenu(mainForm.ProjectManagerArchiveTreeView);

			// Add context menus for our ProjMgr Explorer TreeViews
			mainForm.ProjectManagerClassificationTreeView.ContextMenuStrip = classificationTreeContextMenu.InitializeContextMenu("{Project}");
			mainForm.ProjectManagerPackageTreeView.ContextMenuStrip = packageTreeContextMenu.InitializeContextMenu("{Project}");
			mainForm.ProjectManagerSyncTargetTreeView.ContextMenuStrip = syncTargetContextMenu.InitializeContextMenu("{SyncTargetTreeView}");
			mainForm.ProjectManagerArchiveTreeView.ContextMenuStrip = archiveContextMenu.InitializeContextMenu("{Project}");

			// Make sure we show all of the packages
			mainForm.ProjectManagerClassificationTreeView.ShowPlatformSpecific = true;
			mainForm.ProjectManagerPackageTreeView.ShowPlatformSpecific = true;

			// Build (or rebuild) our trees
			BuildRepositoryTrees(true);

			// Re-apply our current tree sort
			if (mLoaded) ResetPackageTreeSort();

#if MOG_LIBRARY
			ProjectTreeButtonClick(mainForm.ProjectTreeArchiveViewtoolStripButton, mainForm.ProjectManagerArchiveTreeView, "Archive");
#endif

			// Initialize task tree
			//mainForm.ProjectManagerTaskWindow.InitializeForProject();
		}

		public void ClearPackageTreeButtons()
		{
			mainForm.ProjectTreePackageViewToolStripButton.Checked = false;			
			mainForm.ProjectTreeSyncViewToolStripButton.Checked = false;
			mainForm.ProjectTreeClassificationViewToolStripButton.Checked = false;
			mainForm.ProjectTreeArchiveViewtoolStripButton.Checked = false;

			//mainForm.ProjectTreePackageViewToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			//mainForm.ProjectTreeSyncViewToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			//mainForm.ProjectTreeClassificationViewToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			//mainForm.ProjectTreeArchiveViewtoolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
		}

		public void ResetPackageTreeSort()
		{
			if (mainForm.ProjectTreePackageViewToolStripButton.Checked)
			{
				mainForm.ProjectTreePackageViewToolStripButton.PerformClick();
			}
			else if (mainForm.ProjectTreeSyncViewToolStripButton.Checked)
			{
				mainForm.ProjectTreeSyncViewToolStripButton.PerformClick();
			}
			else if (mainForm.ProjectTreeClassificationViewToolStripButton.Checked)
			{
				mainForm.ProjectTreeClassificationViewToolStripButton.PerformClick();
			}
			else if (mainForm.ProjectTreeArchiveViewtoolStripButton.Checked)
			{
				mainForm.ProjectTreeArchiveViewtoolStripButton.PerformClick();
			}
		}
		
		private void ProjectTreeSetFilter(string filter)
		{
			ClearPackageTreeButtons();

			switch(filter)
			{
				case "Classification":
					ProjectTreeButtonClick(mainForm.ProjectTreeClassificationViewToolStripButton, mainForm.ProjectManagerClassificationTreeView, "Classification");
					break;
				case "Package":
					ProjectTreeButtonClick(mainForm.ProjectTreePackageViewToolStripButton, mainForm.ProjectManagerPackageTreeView, "Package");
					break;
				case "Sync":
					ProjectTreeButtonClick(mainForm.ProjectTreeSyncViewToolStripButton, mainForm.ProjectManagerSyncTargetTreeView, "Sync");
					break;
				case "Archive":
					ProjectTreeButtonClick(mainForm.ProjectTreeArchiveViewtoolStripButton, mainForm.ProjectManagerArchiveTreeView, "Archive");
					break;
			}
		}

		internal void ProjectTreeSaveFilter(string filter)
		{
			MogUtils_Settings.SaveSetting("ProjectTreeFilter", "current", filter);
		}

		internal void ProjectTreeButtonClick(ToolStripButton button, object tree, string filterName)
		{
			ClearPackageTreeButtons();
			ProjectTreeSaveFilter(filterName);
			SelectProjectTree(tree);
			button.Checked = true;
			//button.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
		}

		public void SelectProjectTree(object treeToSelect)
		{
			if (treeToSelect is MogControl_BaseTreeView)
			{
				MogControl_BaseTreeView view = (MogControl_BaseTreeView)treeToSelect;
				if (!view.IsInitialized && MOG_ControllerProject.GetProject() != null)
				{
					view.Initialize();
					// If our label edit is not initialized, initialize it...
					if (!view.IsLabelEditInitialized)
					{
						view.InitializeLabelEdit();
					}

					// If we are the classification view, do our dragDrop
					if (view is MogControl_FullTreeView && !view.IsDragDropInitialized)
					{
						view.InitializeDragDrop();
					}
				}
				view.BringToFront();
			}
		}


		/// <summary>
		/// Update populated tree node icons or status based on updated commands from the eventManager
		/// </summary>
		/// <param name="asset"></param>
		public void UpdateAsset(MOG_Command asset)
		{
			// Encapsulate this method in a try-catch
			try
			{
				// Update code for all persistent lock changes
				if (asset.GetCommand() != null && (asset.ToString().ToLower().IndexOf("lock") != -1))
				{
					MOG_Command userLock = asset.GetCommand();
					MOG_Filename assetName = userLock.GetAssetFilename();

					string nodeKey = assetName.GetAssetFullName();

					// Is this a class?
					if (assetName.GetFilenameType() != MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
					{						
						nodeKey = assetName.GetOriginalFilename().TrimEnd("*".ToCharArray());
					}

					if (mainForm.ProjectManagerClassificationTreeView.IsInitialized)
					{
						FindAndUpdateNodeLockStatus(nodeKey, assetName, mainForm.ProjectManagerClassificationTreeView);
					}
					if (mainForm.ProjectManagerPackageTreeView.IsInitialized)
					{
						FindAndUpdateNodeLockStatus(nodeKey, assetName, mainForm.ProjectManagerPackageTreeView);
					}
					if (mainForm.ProjectManagerArchiveTreeView.IsInitialized)
					{
						FindAndUpdateNodeLockStatus(nodeKey, assetName, mainForm.ProjectManagerArchiveTreeView);
					}
					if (mainForm.ProjectManagerSyncTargetTreeView.IsInitialized)
					{
						FindAndUpdateNodeLockStatus(nodeKey, assetName, mainForm.ProjectManagerSyncTargetTreeView);
					}
				}
			}
			// Catch any .NET-standard errors
			catch (Exception ex)
			{
				// Add MOG_Report stuff here.
				MOG_Report.ReportSilent("UpdateAsset", ex.Message, ex.StackTrace);
			}			
		}

		private void FindAndUpdateNodeLockStatus(string nodeKey, MOG_Filename assetName, MogControl_BaseTreeView tree)
		{
			// Attempt to find this node in our tree
			TreeNode[] foundNodes = tree.Nodes.Find(nodeKey, true);
			if (foundNodes != null && foundNodes.Length > 0)
			{
				foreach (TreeNode node in foundNodes)
				{
					Mog_BaseTag tag = node.Tag as Mog_BaseTag;
					if (tag != null)
					{
						// Is this a sync target node
						if (tag.AttachedSyncTargetInfo != null)
						{
							// Update this assets status 
							string assetRepositoryName = tag.FullFilename + "\\Files.Imported\\" + tag.AttachedSyncTargetInfo.FilenameOnly;
							node.ImageIndex = MogUtil_AssetIcons.GetBinaryLockedOrUnlockedIcon(assetName.GetAssetFullName(), assetRepositoryName);
							node.SelectedImageIndex = node.ImageIndex;
						}
						else
						{
							// Update this assets status 
							node.ImageIndex = MogUtil_AssetIcons.GetAssetIconIndex(assetName.GetAssetFullName());
							node.SelectedImageIndex = node.ImageIndex;
						}
					}
				}
			}
		}

		/// <summary>
		/// For Tree Initialization, only initializes the Tree that the user has/had selected
		/// </summary>
		public void BuildRepositoryTrees()
		{
			BuildRepositoryTrees( false );
		}

		public void BuildRepositoryTrees( bool deInitializeTrees )
		{
			// DeInitialize, if we need to do so
			if( deInitializeTrees )
			{
				DeInitializeRepositoryTreeViews();
			}

			InitializeRepositoryTreeViews();
		}

		public void DeInitializeRepositoryTreeViews()
		{
			// Foreach control in ProjectManagerRepositoryTreePanel, see if we need to bringToFront
			foreach (Control control in mainForm.ProjectManagerRepositoryTreePanel.Controls)
			{
				if (control is MogControl_BaseTreeView)
				{
					MogControl_BaseTreeView view = (MogControl_BaseTreeView)control;
					view.DeInitialize();
				}
			}
		}

		public MogControl_BaseTreeView GetRepositoryTreeView()
		{
			if (mainForm.ProjectTreePackageViewToolStripButton.Checked)
			{
				return mainForm.ProjectManagerPackageTreeView;
			}
			else if (mainForm.ProjectTreeSyncViewToolStripButton.Checked)
			{
				return mainForm.ProjectManagerSyncTargetTreeView;
			}
			else if (mainForm.ProjectTreeClassificationViewToolStripButton.Checked)
			{
				return mainForm.ProjectManagerClassificationTreeView;
			}
			else if (mainForm.ProjectTreeArchiveViewtoolStripButton.Checked)
			{
				return mainForm.ProjectManagerArchiveTreeView;
			}

			return null;
		}

		public void InitializeRepositoryTreeViews()
		{
			MogControl_BaseTreeView tree = GetRepositoryTreeView();
			tree.Initialize();
		}

		private TreeNode FindNodeDeep(TreeNode parentNode, string title)
		{
			if (parentNode != null)
			{
				// Walk all these nodes
				foreach (TreeNode node in parentNode.Nodes)
				{
					// Is this node it?
					if (string.Compare(node.Text, title, true) == 0)
					{
						return node;
					}
					
					// If this child has children, are one of them it?
					if (node.Nodes.Count > 0)
					{
						// If we found it, return it
						TreeNode found = null;
						found = FindNodeDeep(node, title);
						if (found != null)
						{
							return found;
						}
					}
					else
					{
						if (string.Compare(node.Text, title, true) == 0)
						{
							return node;
						}
					}
				}
			}
			return null;
		}

		public void MakeAssetCurrent(MOG_Filename filename)
		{
			if (mainForm.ProjectManagerClassificationTreeView != null)
			{
				mainForm.ProjectManagerClassificationTreeView.MakeAssetCurrent(filename);
			}

			if (mainForm.ProjectManagerPackageTreeView != null)
			{
				mainForm.ProjectManagerPackageTreeView.MakeAssetCurrent(filename);
			}

			if (mainForm.ProjectManagerArchiveTreeView != null)
			{
				mainForm.ProjectManagerArchiveTreeView.MakeAssetCurrent(filename);
			}
		}

		public void RemoveAssetFromProject(MOG_Filename filename)
		{
			if (mainForm.ProjectManagerClassificationTreeView != null)
			{
				TreeNode foundNode = mainForm.ProjectManagerClassificationTreeView.FindNode(filename.GetAssetFullName());
				if (foundNode != null)
				{
					foundNode.Remove();
				}
			}

			if (mainForm.ProjectManagerPackageTreeView != null)
			{
				TreeNode foundNode = mainForm.ProjectManagerPackageTreeView.FindNode(filename.GetAssetFullName());
				if (foundNode != null)
				{
					foundNode.Remove();
				}
			}

			if (mainForm.ProjectManagerArchiveTreeView != null)
			{
//?	RemoveAssetFromProject - Known Issue...We'll never be able to find the right node of assets being purged from the repository because the slave will have already removed the asset's name from the database before we get this event containing an encocded name!
				TreeNode foundNode = mainForm.ProjectManagerArchiveTreeView.FindNode(filename.GetAssetFullName());
				if (foundNode != null)
				{
					// Check if this node was already red?
					if (foundNode.ForeColor == Color.Red)
					{
						foundNode.Remove();
					}
					else
					{
						// Set this node to red for 'removed'
						// Should we set the parent to red too?
						SetParentForecolors(filename, foundNode, MOG_ControllerRepository.GetBlessedRevisions(filename, true, false).Count);

						// Set all their children to red
						SetChildForecolors(foundNode);
					}
				}
			}
		}

		private bool SetParentForecolors(MOG_Filename filename, TreeNode node, int numberOfRevisions)
		{
			node.ForeColor = Color.Red;

			if (node.Text.EndsWith(filename.GetAssetLabel(), StringComparison.CurrentCultureIgnoreCase))
			{
				// We also need to remove the "Current <DATE>" node
				if (node.Nodes != null &&
					node.Nodes.Count > 0 &&
					node.Nodes[0].Text.StartsWith("Current", StringComparison.CurrentCultureIgnoreCase))
				{
					node.Nodes[0].Remove();
				}

				return true;
			}				
			else if (numberOfRevisions == 1)				
			{
				if (node.Parent != null)
				{
					if (SetParentForecolors(filename, node.Parent, numberOfRevisions))
					{
						return true;
					}
				}
			}

			return false;
		}

		private void SetChildForecolors(TreeNode node)
		{
			foreach (TreeNode child in node.Nodes)
			{
				child.ForeColor = node.ForeColor;
				if (child.Nodes.Count > 0)
				{
					SetChildForecolors(child);
				}
			}
		}

		public void Refresh()
		{
			if (mainForm.ProjectManagerClassificationTreeView != null)
			{
				mainForm.ProjectManagerClassificationTreeView.DeInitialize();
				mainForm.ProjectManagerClassificationTreeView.Initialize();
			}
		}

		internal void Collapse()
		{
			MogControl_BaseTreeView treeView = GetRepositoryTreeView();
			if (treeView != null)
			{
				if (treeView.SelectedNode != null)
				{
					treeView.SelectedNode.Collapse();
				}
				else
				{
					MOG_Prompt.PromptResponse("Collapse", "Collapse requires a selected node!");
				}
			}
		}

		internal void Expand()
		{
			MogControl_BaseTreeView treeView = GetRepositoryTreeView();
			if (treeView != null)
			{
				if (treeView.SelectedNode != null)
				{
					treeView.SelectedNode.ExpandAll();
				}
				else
				{
					MOG_Prompt.PromptResponse("Expand", "Expand requires a selected node!");
				}
			}
		}

		internal static void SetMogLibraryMode(MogMainForm mogMainForm)
		{
			mogMainForm.ProjectTreeClassificationViewToolStripButton.Visible = false;
			mogMainForm.ProjectTreePackageViewToolStripButton.Visible = false;
			mogMainForm.ProjectTreeSyncViewToolStripButton.Visible = false;
			mogMainForm.ProjectTreeArchiveViewtoolStripButton.Visible = false;
			mogMainForm.toolStripSeparator2.Visible = false;
		}
	}
}
