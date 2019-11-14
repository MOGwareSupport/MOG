using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;

using MOG_ServerManager.Utilities;
using MOG.PROMPT;
using MOG.PROGRESS;
using MOG_ControlsLibrary;
using System.ComponentModel;

namespace MOG_ServerManager
{
	/// <summary>
	/// Summary description for SelectedTreeView.
	/// </summary>
	public class SelectedTreeView : CodersLab.Windows.Controls.TreeView
	{
		#region Member vars
		private bool rightButtonClicked = false;

		// my brother treeviews
		private DiskTreeView diskTreeView = null;
		private AssetTreeView assetTreeView = null;

		// Items for the context menu
		ToolStripMenuItem miExpandToRed = null;
		ToolStripMenuItem miRemove = null;
		ToolStripMenuItem miShowAsset = null;

		#endregion
		#region Properties
		public DiskTreeView DiskTreeView
		{
			get { return this.diskTreeView; }
			set { this.diskTreeView = value; }
		}

		public AssetTreeView AssetTreeView
		{
			get { return this.assetTreeView; }
			set { this.assetTreeView = value; }
		}
		#endregion
		#region Constants
		private const int FILE_INDEX			= 0;
		private const int OPENFOLDER_INDEX		= 1;
		private const int CLOSEDFOLDER_INDEX	= 2;
		private const int ARROW_INDEX			= 3;
		private const int ERROR_INDEX			= 4;
		private const int INFO_INDEX			= 5;
		private const int YELLOWARROW_INDEX		= 6;
		private const int QUESTION_INDEX		= 7;
		private const int BLUEDOT_INDEX			= 8;
		private const int WARNING_INDEX			= 9;
		#endregion
		#region Constructor
		public SelectedTreeView()
		{
			this.SelectionMode = CodersLab.Windows.Controls.TreeViewSelectionMode.MultiSelect;
			this.KeyUp += new KeyEventHandler(SelectedTreeView_KeyUp);
			this.MouseDown += new MouseEventHandler(SelectedTreeView_MouseDown);
			this.ItemDrag += new ItemDragEventHandler(SelectedTreeView_ItemDrag);
			
			this.AllowDrop = true;
			this.DragEnter += new DragEventHandler(SelectedTreeView_DragEnter);
			this.DragDrop += new DragEventHandler(SelectedTreeView_DragDrop);


			// build the context menu
			this.miExpandToRed = new ToolStripMenuItem("Show unplaced", null, new EventHandler(miExpandToRed_Click));
			this.miRemove = new ToolStripMenuItem("Remove", null, new EventHandler(miRemove_Click));
			this.miShowAsset = new ToolStripMenuItem("Show Asset", null, new EventHandler(miShowAsset_Click));
			this.ContextMenuStrip = new ContextMenuStrip();
			this.ContextMenuStrip.Items.Add(miExpandToRed);
			this.ContextMenuStrip.Items.Add(miShowAsset);
			this.ContextMenuStrip.Items.Add(new ToolStripSeparator());
			this.ContextMenuStrip.Items.Add(miRemove);
			this.ContextMenuStrip.Opening += ContextMenu_Popup;
		}
		#endregion
		#region Public functions
		public void ResetEverything()
		{
			this.Nodes.Clear();
		}

		public void RefreshRedStatus()
		{
			// save a copy of the selected nodes
			ArrayList selNodes = new ArrayList();
			foreach (AssetTreeNode tn in this.SelectedNodes)
			{
				selNodes.Add(tn);
			}
			this.SelectedNodes.Clear();
			
			foreach (AssetTreeNode tn in this.Nodes)
			{
				RefreshRedStatus(tn);
			}
			
			// restore selected nodes
			foreach (AssetTreeNode tn in selNodes)
			{
				this.SelectedNodes.Add(tn);
			}
		}

		public void RefreshColorStatusAboveNode(AssetTreeNode tn)
		{
			// unselect
			if (this.SelectedNodes.Contains(tn))
				this.SelectedNodes.Remove(tn);

			// Check if we have a parent?
			if (tn.Parent != null)
			{
				// Default the desired color to our node's color
				Color desiredColor = tn.ForeColor;

				// Check if we are not red?
				if (desiredColor != Color.Red)
				{
					// Check if anybody is red?
					foreach (AssetTreeNode subNode in tn.Nodes)
					{
						if (subNode.ForeColor == Color.Red)
						{
							// Indicate our new desired color is Red
							desiredColor = Color.Red;
							break;
						}
					}
				}

				// Check if our Parent needs a color change?
				if (tn.Parent.ForeColor != desiredColor)
				{
					// Force our Parent to match our color
					tn.Parent.ForeColor = desiredColor;
					// Have our parent check their parent
					RefreshColorStatusAboveNode(tn.Parent as AssetTreeNode);
				}
			}
		}

		public void RefreshTree(TreeNodeCollection nodes)
		{
			if (nodes != null)
			{
				this.Nodes.Clear();

				foreach (AssetTreeNode tn in nodes)
				{
					AssetTreeNode newNode = EncodeDiskNodeToSelected(tn);
					if (newNode != null)
						this.Nodes.Add(newNode);
				}

				ExpandAll();
			}
		
		}
		#endregion
		#region Private functions
		private AssetTreeNode EncodeDiskNodeToSelected(AssetTreeNode tn)
		{
			if (!tn.Checked)
				return null;

			AssetTreeNode newNode = new AssetTreeNode(tn.Text);
			newNode.InSelectedTree = true;
			newNode.ForeColor = Color.Red;

			if (newNode.IsAFolder)
			{
				newNode.ImageIndex = CLOSEDFOLDER_INDEX;
				newNode.SelectedImageIndex = CLOSEDFOLDER_INDEX;
			}
			else
			{
				newNode.ImageIndex = QUESTION_INDEX;
				newNode.SelectedImageIndex = QUESTION_INDEX;
			}
			
			foreach (AssetTreeNode subNode in tn.Nodes)
			{
				TreeNode newSubNode = EncodeDiskNodeToSelected(subNode);
				if (newSubNode != null)
					newNode.Nodes.Add(newSubNode);
			}

			newNode.DiskNode = tn;
			newNode.MasterNode = tn.MasterNode;
			tn.SelectedNode = newNode;

			return newNode;
		}
	
		private void RefreshRedStatus(AssetTreeNode tn)
		{
			// unselect
			if (this.SelectedNodes.Contains(tn))
				this.SelectedNodes.Remove(tn);

			foreach (AssetTreeNode subNode in tn.Nodes)
			{
				RefreshRedStatus(subNode);
			}

			if (tn.IsAFolder)
			{
				bool allGreen = true;

				foreach (AssetTreeNode subNode in tn.Nodes)
				{
					if (subNode.ForeColor != Color.Green)
					{
						allGreen = false;
						break;
					}
				}
					
				if (allGreen)
					tn.ForeColor = Color.Green;
				else
					tn.ForeColor = Color.Red;
			}
		}

		private AssetTreeNode ConvertDiskNode(AssetTreeNode tn)
		{
			AssetTreeNode newNode = new AssetTreeNode(tn.Text);
			newNode.InSelectedTree = true;
			newNode.ForeColor = Color.Red;

			newNode.IsAFile = tn.IsAFile;
			newNode.IsAFolder = tn.IsAFolder;
			newNode.IsAnAssetFilename = tn.IsAnAssetFilename;
			newNode.MasterNode = tn.MasterNode;
			newNode.DiskNode = tn.DiskNode;
			newNode.SelectedNode = tn.SelectedNode;
			newNode.FileFullPath = tn.FileFullPath;
			newNode.RelativePath = tn.RelativePath;

			if (newNode.IsAFolder)
			{
				newNode.ImageIndex = CLOSEDFOLDER_INDEX;
				newNode.SelectedImageIndex = CLOSEDFOLDER_INDEX;
			}
			else
			{
				newNode.ImageIndex = QUESTION_INDEX;
				newNode.SelectedImageIndex = QUESTION_INDEX;
			}

			// link everything together
			newNode.DiskNode = tn;
			newNode.MasterNode = tn.MasterNode;
			tn.SelectedNode = newNode;

			return newNode;
		}

		private void ShowUnplaced(AssetTreeNode targetNode)
		{
			// assume global unless instructed otherwise
			TreeNodeCollection nodes = this.Nodes;
			if (targetNode != null)
				nodes = targetNode.Nodes;

			foreach (AssetTreeNode tn in nodes)
			{
				if (tn.ForeColor == Color.Red)
					tn.EnsureVisible();

				// recurse
				ShowUnplaced(tn);
			}
		}

		private void DeleteNode(AssetTreeNode selectNode)
		{
			if (selectNode == null)
				return;

			// recurse first
			foreach (AssetTreeNode tn in selectNode.Nodes)
				DeleteNode(tn);

			try
			{
				// now do me
				if (selectNode.AssetNode != null)
				{
					// deal with the asset node
					if (selectNode.AssetNode.IsAFile)
					{
						selectNode.AssetNode.AssetFilenameNode.fileNodes.Remove(selectNode.AssetNode);
						if (this.assetTreeView != null)
							this.assetTreeView.RebuildSubStructure(selectNode.AssetNode.AssetFilenameNode);

						if (selectNode.AssetNode.AssetFilenameNode.fileNodes.Count == 0  &&  selectNode.AssetNode.AssetFilenameNode.TreeView != null)
							selectNode.AssetNode.AssetFilenameNode.Remove();
					}
					else if (selectNode.AssetNode.IsAnAssetFilename  &&  selectNode.AssetNode.fileNodes.Count == 0  &&  selectNode.AssetNode.TreeView != null)
						selectNode.AssetNode.Remove();
					else if (selectNode.AssetNode.IsAClassification  &&  selectNode.AssetNode.Nodes.Count == 0  &&  selectNode.AssetNode.AutoGenerated  &&  selectNode.AssetNode.TreeView != null)
						selectNode.AssetNode.Remove();
				}

				selectNode.DiskNode.SelectedNode = null;
			}
			catch (Exception e)
			{
				MOG_Prompt.PromptResponse("Error",e.Message, e.StackTrace, MOGPromptButtons.OK, MOG_ALERT_LEVEL.CRITICAL);
			}
		}

		private AssetTreeNode FindSelectedRoot(AssetTreeNode tn)
		{
			if (!this.SelectedNodes.Contains(tn))
				return null;

			AssetTreeNode selectedParent = tn;
			while (selectedParent.Parent != null  &&  selectedParent.Parent.IsSelected)
				selectedParent = selectedParent.Parent as AssetTreeNode;

			return selectedParent;
		}

		private ArrayList GetSelectedRoots()
		{
			ArrayList selectedRoots = new ArrayList();

			foreach (AssetTreeNode tn in this.SelectedNodes)
			{
				AssetTreeNode selectedRoot = FindSelectedRoot(tn);
				if (!selectedRoots.Contains(selectedRoot))
					selectedRoots.Add(selectedRoot);
			}

			return selectedRoots;
		}

		private void DeleteSelected()
		{
			try
			{
				ArrayList prevNodes = new ArrayList();

				ArrayList selectedRoots = GetSelectedRoots();
				foreach (AssetTreeNode selectedRoot in selectedRoots)
				{
					if (selectedRoot.NextNode != null  &&  !prevNodes.Contains(selectedRoot.NextNode))
						prevNodes.Add( selectedRoot.NextNode );
					if (selectedRoot.PrevNode != null  &&  !prevNodes.Contains(selectedRoot.PrevNode))
						prevNodes.Add( selectedRoot.PrevNode );

					if (selectedRoot.DiskNode != null  &&  selectedRoot.DiskNode.TreeView != null)
						selectedRoot.DiskNode.Checked = false;

					//DeleteNode(selectedRoot);
					//selectedRoot.Remove();
				}

				// fixup selected nodes to avoid crash later on...
				this.SelectedNodes.Clear();
				foreach (TreeNode prevNode in prevNodes)
				{
					if (prevNode.TreeView != null)
					{
						this.SelectedNodes.Add(prevNode);
						break;
					}
				}
			
				if (this.SelectedNodes.Count == 0)
				{
					if (this.Nodes.Count > 0)
						this.SelectedNodes.Add( this.Nodes[0] );
					else
					{
						this.SelectedNodes.Clear();
						this.Nodes.Clear();
					}
				}

				//			foreach (AssetTreeNode tn in this.SelectedNodes)
				//			{
				//				DeleteNode(tn);
				//
				//				DeleteAssetNodes(tn.Nodes);
				//				if (tn.AssetNode != null  &&  tn.AssetNode.TreeView != null  &&  tn.AssetNode.TreeView is AssetTreeView)
				//				{
				//					((AssetTreeView)tn.AssetNode.TreeView).RemoveNode(tn.AssetNode);
				//				}
				//
				//				if (tn.DiskNode != null)
				//					tn.DiskNode.Checked = false;
				//				if (tn.MasterNode != null)
				//					tn.MasterNode.ForeColor = Color.Indigo;
				//
				//				if (tn.TreeView != null)
				//					tn.Remove();
				//			}

			}
			catch (Exception e)
			{
				MOG_Prompt.PromptResponse("Error", e.Message, e.StackTrace, MOGPromptButtons.OK, MOG_ALERT_LEVEL.CRITICAL);				
			}
		}

		private void DeleteAssetNodes(AssetTreeNode tn)
		{
			DeleteAssetNodes(tn.Nodes);

			if (tn.IsAFile  &&  tn.AssetNode != null)
				((AssetTreeView)tn.AssetNode.TreeView).RemoveNode(tn.AssetNode);
		}

		private void DeleteAssetNodes(TreeNodeCollection nodes)
		{
			foreach (AssetTreeNode tn in nodes)
			{
				DeleteAssetNodes(tn.Nodes);

				if (tn.IsAFile  &&  tn.AssetNode != null)
					((AssetTreeView)tn.AssetNode.TreeView).RemoveNode(tn.AssetNode);

				if (tn.IsAFolder  &&  tn.AssetNode != null  &&  tn.fileNodes.Count == 0)
					((AssetTreeView)tn.AssetNode.TreeView).RemoveNode(tn.AssetNode);

			}
		}


		#endregion
		#region Internal functions
		internal void RemoveNode(AssetTreeNode tn)
		{
			if (tn == null  ||  tn.SelectedNode == null)
				return;

			AssetTreeNode selectedNode = tn.SelectedNode;
			DeleteNode(selectedNode);

			if (selectedNode.TreeView != null)
				selectedNode.Remove();

//			if (tn.SelectedNode != null  &&  tn.SelectedNode.TreeView != null)
//			{
//				DeleteAssetNodes(tn);
//				tn.SelectedNode.Remove();
//				tn.SetSelectedNode_Recursive(null);
//			}
		}

		internal void NewNode(AssetTreeNode tn)
		{
			if (tn == null  ||  tn.SelectedNode != null)
				return;

			AssetTreeNode newNode = ConvertDiskNode(tn);

			AssetTreeNode parentNode = tn.Parent as AssetTreeNode;
			AssetTreeNode curNode = newNode;
			while (parentNode != null)
			{
				// if parentNode has a corresponding selected node in this treeview already
				if (parentNode.SelectedNode != null)
				{
					// add curNode tree to it
					parentNode.SelectedNode.Nodes.Add( curNode );
					if (parentNode.IsExpanded)
					{
						parentNode.SelectedNode.Expand();
					}
//					else
//					{
//						parentNode.SelectedNode.Collapse();
//					}

					break;
				}
				else
				{
					// make new and chain up
					parentNode.SelectedNode = ConvertDiskNode(parentNode);
					parentNode.SelectedNode.Nodes.Add(curNode);
					if (parentNode.IsExpanded)
						parentNode.SelectedNode.Expand();
//					else
//						parentNode.SelectedNode.Collapse();
					
					curNode = parentNode.SelectedNode;
					parentNode = parentNode.Parent as AssetTreeNode;
				}
			}

			// no parent node?  just add it to the tree view itself
			if (parentNode == null)
			{
				// insert it in the right place
				int index = CalculateRootInsertionIndex(Utils.GetRootNode(tn) as AssetTreeNode, curNode);
				if (index == -1)
					this.Nodes.Add(curNode);
				else
					this.Nodes.Insert(index, curNode);
			}
			//FillInSubTree(newNode);
		}

		private void FillInSubTree(AssetTreeNode selectNode)
		{
			
			AssetTreeNode diskNode = selectNode.DiskNode;
			if (diskNode != null)
			{
				foreach (AssetTreeNode subDiskNode in diskNode.Nodes)
					selectNode.Nodes.Add( ConvertDiskNode(subDiskNode) );

				foreach (AssetTreeNode subSelectNode in selectNode.Nodes)
					FillInSubTree(subSelectNode);

				if (diskNode.IsExpanded)
					selectNode.Expand();
//				else
//					selectNode.Collapse();
			}
		}

		private int CalculateRootInsertionIndex(AssetTreeNode diskRootNode, AssetTreeNode selectedRootNode)
		{
			DiskTreeView diskTV = diskRootNode.TreeView as DiskTreeView;
			
			foreach (AssetTreeNode selNode in this.Nodes)
			{
				if (selNode.DiskNode != null)
				{
					if (diskRootNode.Index < selNode.DiskNode.Index)
						return selNode.Index;
				}
			}

			return -1;
		}

		#endregion
		#region Event handlers
		private void SelectedTreeView_DragEnter(object sender, DragEventArgs args)
		{
			if (args.Data.GetDataPresent(typeof(AssetTreePlacerDragObject)))
			{
				args.Effect = args.AllowedEffect;
			}
		}

		private void SelectedTreeView_DragDrop(object sender, DragEventArgs args)
		{
			AssetTreePlacerDragObject dragObj = args.Data.GetData(typeof(AssetTreePlacerDragObject)) as AssetTreePlacerDragObject;
			
			if (dragObj == null)
				return;

			Cursor.Current = Cursors.WaitCursor;

			if (dragObj.source == AssetTreePlacerDragObject.DragObjectSource.DISKTREEVIEW)
			{
				// place nodes in selected tree
				ArrayList roots = AssetTreeNode.GetUniqueRootNodes(dragObj.nodeList);

				foreach (TreeNode root in roots)
				{
					if (!root.Checked)
						root.Checked = true;
				}
			}
		}

		private void ContextMenu_Popup(object sender, CancelEventArgs args)
		{
			// Should the Show Asset menu item be enabled? (i.e., are there any selected nodes that have been placed as assets
			this.miShowAsset.Enabled = false;
			foreach (AssetTreeNode atn in this.SelectedNodes)
			{
				if (atn.AssetNode != null)
				{
					// If this node has a corresponding asset node, enable the menu item and don't bother checking the rest
					this.miShowAsset.Enabled = true;
					break;
				}
			}
		}

		private void miExpandToRed_Click(object sender, EventArgs args)
		{
			AssetTreeNode targetNode = GetNodeAt( PointToClient(new Point(Cursor.Position.X, Cursor.Position.Y)) ) as AssetTreeNode;
			ShowUnplaced(targetNode);
			if (targetNode != null)
			{
				targetNode.EnsureVisible();
			}
		}

		private void miShowAsset_Click(object sender, EventArgs args)
		{
			// Clear the selection of the asset tree view because we're going to select all teh counterpart nodes in our SelectedNodes
			if (this.assetTreeView != null)
			{
				this.assetTreeView.SelectedNodes.Clear();
			}

			foreach (AssetTreeNode atn in this.SelectedNodes)
			{
				if (atn.AssetNode != null)
				{
					atn.AssetNode.EnsureVisible();

					// Add the counterpart asset node to the asset tree view's selected nodes if possible
					if (this.assetTreeView != null)
					{
						this.assetTreeView.SelectedNodes.Add(atn.AssetNode);
					}
				}
			}
		}
		
		private void miRemove_Click(object sender, EventArgs args)
		{
			DeleteSelected();
		}


		private void SelectedTreeView_ItemDrag(object sender, System.Windows.Forms.ItemDragEventArgs args)
		{
			ArrayList nodeList = new ArrayList();
			foreach (AssetTreeNode tn in this.SelectedNodes)
				nodeList.Add(tn);

			AssetTreePlacerDragObject dragObj = new AssetTreePlacerDragObject(AssetTreePlacerDragObject.DragObjectSource.SELECTTREEVIEW);
			dragObj.button = (this.rightButtonClicked) ? MouseButtons.Right : MouseButtons.Left;//args.Button;
			dragObj.nodeList = nodeList;			

			this.DoDragDrop(dragObj, DragDropEffects.Copy);
		}

		private void SelectedTreeView_KeyUp(object sender, KeyEventArgs args)
		{
			if (args.KeyData == Keys.Delete)
			{
				DeleteSelected();
			}
			else if (args.KeyData == Keys.F5)
			{
				RefreshRedStatus();
			}
		}

		private void SelectedTreeView_MouseDown(object sender, MouseEventArgs args)
		{
			this.rightButtonClicked = (args.Button == MouseButtons.Right);
		}
	}
		#endregion

}




