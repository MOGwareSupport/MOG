using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;

using CodersLab.Windows.Controls;

using MOG_ServerManager.Utilities;

using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.PROMPT;
using MOG.PROGRESS;
using MOG_ControlsLibrary;
using MOG_CoreControls;
using System.ComponentModel;
using System.Collections.Generic;
using MOG.DOSUTILS;

namespace MOG_ServerManager
{
	/// <summary>
	/// Summary description for DiskTreeView.
	/// </summary>
	public class DiskTreeView : CheckBoxTreeView
	{
		#region Member vars
		private bool includeRootDir = false;
		private string loadedPath = "";
		private bool showProgressBar = true;

		// my brother treeviews
		private AssetTreeView assetTreeView = null;
		private SelectedTreeView selectedTreeView = null;

		private AssetImportPlacer assetImportPlacer = null;

		private ContextMenuStrip cmMenu = new ContextMenuStrip();
		#endregion
		#region Properties
		public AssetImportPlacer AssetImportPlacer
		{
			get { return this.assetImportPlacer; }
			set { this.assetImportPlacer = value; }
		}

		public AssetTreeView AssetTreeView
		{
			get { return this.assetTreeView; }
			set { this.assetTreeView = value; }
		}

		public SelectedTreeView SelectedTreeView
		{
			get { return this.selectedTreeView; }
			set { this.selectedTreeView = value; }
		}

		public bool ShowProgressBar
		{
			get { return this.showProgressBar; }
			set { this.showProgressBar = value; }
		}

		public bool IncludeRootDir
		{
			get { return this.includeRootDir; }
			set { this.includeRootDir = value; }
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
		public DiskTreeView()
		{
			this.ShowLines = false;
			this.ShowRootLines = true;

			this.AllowDrop = true;
			this.DragEnter += new DragEventHandler(DiskTreeView_DragEnter);
			this.DragDrop += new DragEventHandler(DiskTreeView_DragDrop);
			this.ItemDrag += new ItemDragEventHandler(DiskTreeView_ItemDrag);
			this.KeyUp += new KeyEventHandler(DiskTreeView_KeyUp);

			this.cmMenu.Items.Add("Check All", null, new EventHandler(DiskTreeView_miCheckAll));
			this.cmMenu.Items.Add("Uncheck All", null, new EventHandler(DiskTreeView_miUncheckAll));
			this.cmMenu.Items.Add("Check Selection", null, new EventHandler(DiskTreeView_miCheckSelection));
			this.cmMenu.Items.Add("Uncheck Selection", null, new EventHandler(DiskTreeView_miUncheckSelection));
			this.ContextMenuStrip = this.cmMenu;
		}

		#endregion
		#region Public functions

		public void ResetEverything()
		{
			this.Nodes.Clear();
		}

		private int CountSubdirs(string path)
		{
			int count = 0;

			foreach (string dirpath in Directory.GetDirectories(path))
			{
				count++;
				count += CountSubdirs(dirpath);
			}

			return count;
		}

		private void CountSubdirs_Worker(object sender, DoWorkEventArgs e)
		{
			string path = e.Argument as string;
			
			int count = CountSubdirs(path);

			e.Result = count;
		}

		public void LoadTreeFromDisk(string path)
		{
			Nodes.Clear();
			loadedPath = path;

			AddTreeFromDisk(path);
		}

		public void AddTreeFromDisk(string path)
		{
			if (Directory.Exists(path))
			{
// Removed because this double progress dialog was crashing us...besides, reporting path to user is progress enough for now
//				ProgressDialog progress = new ProgressDialog("Counting Subdirectories", "Calculating...", CountSubdirs_Worker, path, false);
//				if (progress.ShowDialog(this) == DialogResult.OK)
//				{
//					int subdirs = (int)progress.WorkerResult;

					List<object> args = new List<object>();
					args.Add(path);
//					args.Add(subdirs);
					
					ProgressDialog progress = new ProgressDialog("Loading Folders", "Please wait while MOG loads the directory tree", CreateTreeFromDisk_Worker, args, true);
					if (progress.ShowDialog(this) == DialogResult.OK)
					{
						// Build the list of children nodes under the root directory
						AssetTreeNode parentnode = progress.WorkerResult as AssetTreeNode;
						// Bring in all of the children nodes so the root directory node won't confuse the user
						foreach(AssetTreeNode node in parentnode.Nodes)
						{
							Nodes.Add(node);
						}
					}
//				}
			}
		}

		private void CreateTreeFromDisk_Worker(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			List<object> args = e.Argument as List<object>;
			string path = args[0] as string;
//			int subdirs = (int)args[1];

			e.Result = CreateTreeFromDisk(path, path, worker);
		}

		private AssetTreeNode CreateTreeFromDisk(string rootPath, string path, BackgroundWorker worker)
		{
			if (Directory.Exists(path))
			{
				// Show the relative path so we have more real estate in the progress dialog
				string relativePath = DosUtils.PathMakeRelativePath(rootPath, path);
				worker.ReportProgress(0, "Loading:\n" + relativePath);

				AssetTreeNode node = CreateFolderNode(path);
				
				//Add subdirectories
				foreach (string subdir in Directory.GetDirectories(path))
				{
					node.Nodes.Add(CreateTreeFromDisk(rootPath, subdir, worker));
				}

				//Add files
				foreach (string file in Directory.GetFiles(path))
				{
					node.Nodes.Add(CreateFileNode(file, path));
				}

				return node;
			}

			return null;
		}
		#endregion
		#region Private functions
		private AssetTreeNode CreateFolderNode(string path)
		{
			AssetTreeNode node = new AssetTreeNode(Path.GetFileName(path), CLOSEDFOLDER_INDEX, Color.DarkGoldenrod);
			node.FileFullPath = path;
			node.InDiskTree = true;
			node.IsAFolder = true;
			return node;
		}
		
		private AssetTreeNode CreateFileNode(string path)
		{
			AssetTreeNode node =  new AssetTreeNode(Path.GetFileName(path), FILE_INDEX, Color.DarkMagenta);
			node.InDiskTree = true;
			node.FileFullPath = path;
			node.RelativePath = path.ToLower().Replace( this.loadedPath.ToLower(), "" ).Trim("\\".ToCharArray());
			node.IsAFile = true;
			return node;
		}

		private AssetTreeNode CreateFileNode(string path, string basePath)
		{
			AssetTreeNode node =  new AssetTreeNode(Path.GetFileName(path), FILE_INDEX, Color.DarkMagenta);
			node.InDiskTree = true;
			node.FileFullPath = path;
			node.RelativePath = path.ToLower().Replace( basePath.ToLower(), "" ).Trim("\\".ToCharArray());
			node.IsAFile = true;
			return node;
		}
		#endregion
		#region Event handlers
		private void DiskTreeView_miCheckAll(object sender, EventArgs args)
		{
			foreach (TreeNode tn in this.Nodes)
			{
				tn.Checked = true;
			}
		}

		private void DiskTreeView_miUncheckAll(object sender, EventArgs args)
		{
			foreach (TreeNode tn in this.Nodes)
			{
				tn.Checked = false;
			}
		}

		private void DiskTreeView_miCheckSelection(object sender, EventArgs args)
		{
			foreach (TreeNode tn in this.SelectedNodes)
			{
				tn.Checked = true;
			}
		}

		private void DiskTreeView_miUncheckSelection(object sender, EventArgs args)
		{
			foreach (TreeNode tn in this.SelectedNodes)
			{
				tn.Checked = false;
			}
		}

		private void DiskTreeView_DragEnter(object sender, System.Windows.Forms.DragEventArgs args)
		{
			if (args.Data.GetDataPresent(DataFormats.FileDrop))
			{
				args.Effect = args.AllowedEffect;
			}
		}

		private void DiskTreeView_DragDrop(object sender, DragEventArgs args)
		{
			if (args.Data.GetDataPresent(DataFormats.FileDrop))
			{
				if (this.assetImportPlacer != null)
				{
					Cursor.Current = Cursors.WaitCursor;

					// Get the specified items
					string[] files = (string[])args.Data.GetData("FileDrop", false);
					// Make sure we only had a single directory
					if (files != null && 
						files.Length == 1 &&
						Directory.Exists(files[0]) == true)
					{
						assetImportPlacer.RaiseEvent_LoadingDirectories();

						this.assetImportPlacer.ResetEverything();

						this.assetImportPlacer.ProjectPath = files[0];

						// add each selected item
						foreach (string draggedPath in files)
						{
							this.AddTreeFromDisk(draggedPath);
						}

						assetImportPlacer.RaiseEvent_DoneLoadingDirectories();
					}
					else
					{
						this.assetImportPlacer.ProjectPath = "Only a single directory can be dragged in!";
					}

					Cursor.Current = Cursors.Default;
				}
			}
		}

		private void DiskTreeView_ItemDrag(object sender, System.Windows.Forms.ItemDragEventArgs args)
		{
			ArrayList nodeList = new ArrayList();
			foreach (AssetTreeNode tn in this.SelectedNodes)
			{
				nodeList.Add(tn);
			}

			AssetTreePlacerDragObject dragObj = new AssetTreePlacerDragObject(AssetTreePlacerDragObject.DragObjectSource.DISKTREEVIEW);
			dragObj.button = args.Button;
			dragObj.nodeList = nodeList;			

			this.DoDragDrop(dragObj, DragDropEffects.Copy);
		}

		private void DiskTreeView_KeyUp(object sender, KeyEventArgs args)
		{
			if (args.KeyData == Keys.Delete)
			{
				foreach (TreeNode tn in this.SelectedNodes)
				{
					if (tn.Checked)
					{
						tn.Checked = false;
					}
				}
			}
		}
		#endregion
	}
}


