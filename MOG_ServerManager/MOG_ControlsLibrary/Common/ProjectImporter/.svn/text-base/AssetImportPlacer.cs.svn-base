using System;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using MOG.PROJECT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.PROMPT;
using MOG.PROGRESS;

using MOG_ServerManager.Utilities;
using MOG_ControlsLibrary;

namespace MOG_ServerManager
{
	/// <summary>
	/// Summary description for AssetTreePlacer.
	/// </summary>
	public class AssetImportPlacer : System.Windows.Forms.UserControl
	{
		#region System defs

		private System.Windows.Forms.TextBox tbPath;
		private System.Windows.Forms.Button btnBrowse;
		private System.Windows.Forms.ImageList imageList;
		private DiskTreeView tvDisk;
		private SelectedTreeView tvSelected;
		private AssetTreeView tvAssets;
		private System.Windows.Forms.Panel panel;
		private System.Windows.Forms.Panel pnlDiskTreeView;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Panel pnlSelectedTreeView;
		private System.Windows.Forms.Splitter splitter2;
		private System.Windows.Forms.Panel pnlAssetTreeView;
		private System.Windows.Forms.ComboBox cbClassTemplates;
		private System.Windows.Forms.Label lblTemplatesLabel;
		private System.ComponentModel.IContainer components;

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
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(AssetImportPlacer));
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.tbPath = new System.Windows.Forms.TextBox();
			this.btnBrowse = new System.Windows.Forms.Button();
			this.tvDisk = new DiskTreeView();
			this.tvSelected = new SelectedTreeView();
			this.tvAssets = new AssetTreeView();
			this.panel = new System.Windows.Forms.Panel();
			this.pnlAssetTreeView = new System.Windows.Forms.Panel();
			this.lblTemplatesLabel = new System.Windows.Forms.Label();
			this.cbClassTemplates = new System.Windows.Forms.ComboBox();
			this.splitter2 = new System.Windows.Forms.Splitter();
			this.pnlSelectedTreeView = new System.Windows.Forms.Panel();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.pnlDiskTreeView = new System.Windows.Forms.Panel();
			this.panel.SuspendLayout();
			this.pnlAssetTreeView.SuspendLayout();
			this.pnlSelectedTreeView.SuspendLayout();
			this.pnlDiskTreeView.SuspendLayout();
			this.SuspendLayout();
			// 
			// imageList
			// 
			this.imageList.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// tbPath
			// 
			this.tbPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tbPath.Location = new System.Drawing.Point(16, 8);
			this.tbPath.Name = "tbPath";
			this.tbPath.Size = new System.Drawing.Size(280, 20);
			this.tbPath.TabIndex = 3;
			this.tbPath.Text = "";
			this.tbPath.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbPath_KeyUp);
			// 
			// btnBrowse
			// 
			this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnBrowse.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.btnBrowse.Location = new System.Drawing.Point(301, 7);
			this.btnBrowse.Name = "btnBrowse";
			this.btnBrowse.Size = new System.Drawing.Size(24, 22);
			this.btnBrowse.TabIndex = 4;
			this.btnBrowse.Text = "...";
			this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
			// 
			// tvDisk
			// 
			this.tvDisk.AllowDrop = true;
			this.tvDisk.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tvDisk.AssetImportPlacer = null;
			this.tvDisk.AssetTreeView = null;
			this.tvDisk.CheckBoxes = true;
			this.tvDisk.CheckStyle = CheckBoxTreeView.cbtvCheckStyle.Recursive;
			this.tvDisk.HideSelection = false;
			this.tvDisk.ImageList = this.imageList;
			this.tvDisk.IncludeRootDir = false;
			this.tvDisk.Location = new System.Drawing.Point(8, 40);
			this.tvDisk.Name = "tvDisk";
			this.tvDisk.SelectedTreeView = null;
			this.tvDisk.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			this.tvDisk.SelectionMode = CodersLab.Windows.Controls.TreeViewSelectionMode.MultiSelect;
			this.tvDisk.ShowLines = false;
			this.tvDisk.ShowProgressBar = true;
			this.tvDisk.Size = new System.Drawing.Size(320, 320);
			this.tvDisk.TabIndex = 5;
			this.tvDisk.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tvDisk_AfterCheck);
			this.tvDisk.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.tvDisk_AfterExpand);
			this.tvDisk.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.tvDisk_AfterCollapse);
			this.tvDisk.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tvDisk_MouseUp);
			// 
			// tvSelected
			// 
			this.tvSelected.AllowDrop = true;
			this.tvSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tvSelected.AssetTreeView = null;
			this.tvSelected.DiskTreeView = null;
			this.tvSelected.ImageList = this.imageList;
			this.tvSelected.Location = new System.Drawing.Point(0, 40);
			this.tvSelected.Name = "tvSelected";
			this.tvSelected.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			this.tvSelected.SelectionMode = CodersLab.Windows.Controls.TreeViewSelectionMode.MultiSelect;
			this.tvSelected.Size = new System.Drawing.Size(320, 320);
			this.tvSelected.TabIndex = 6;
			this.tvSelected.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.tvSelected_AfterExpand);
			this.tvSelected.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tvSelected_MouseUp);
			this.tvSelected.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.tvSelected_AfterCollapse);
			// 
			// tvAssets
			// 
			this.tvAssets.AllowDrop = true;
			this.tvAssets.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tvAssets.DiskTreeView = null;
			this.tvAssets.ImageList = this.imageList;
			this.tvAssets.LabelEdit = true;
			this.tvAssets.Location = new System.Drawing.Point(0, 40);
			this.tvAssets.Name = "tvAssets";
			this.tvAssets.RootName = "Project Repository";
			this.tvAssets.SelectedTreeView = null;
			this.tvAssets.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			this.tvAssets.SelectionMode = CodersLab.Windows.Controls.TreeViewSelectionMode.MultiSelect;
			this.tvAssets.Size = new System.Drawing.Size(344, 320);
			this.tvAssets.TabIndex = 7;
			// 
			// panel
			// 
			this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.panel.Controls.Add(this.pnlAssetTreeView);
			this.panel.Controls.Add(this.splitter2);
			this.panel.Controls.Add(this.pnlSelectedTreeView);
			this.panel.Controls.Add(this.splitter1);
			this.panel.Controls.Add(this.pnlDiskTreeView);
			this.panel.Location = new System.Drawing.Point(8, 8);
			this.panel.Name = "panel";
			this.panel.Size = new System.Drawing.Size(1000, 368);
			this.panel.TabIndex = 8;
			// 
			// pnlAssetTreeView
			// 
			this.pnlAssetTreeView.Controls.Add(this.lblTemplatesLabel);
			this.pnlAssetTreeView.Controls.Add(this.cbClassTemplates);
			this.pnlAssetTreeView.Controls.Add(this.tvAssets);
			this.pnlAssetTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlAssetTreeView.Location = new System.Drawing.Point(651, 0);
			this.pnlAssetTreeView.Name = "pnlAssetTreeView";
			this.pnlAssetTreeView.Size = new System.Drawing.Size(349, 368);
			this.pnlAssetTreeView.TabIndex = 9;
			// 
			// lblTemplatesLabel
			// 
			this.lblTemplatesLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblTemplatesLabel.Location = new System.Drawing.Point(8, 8);
			this.lblTemplatesLabel.Name = "lblTemplatesLabel";
			this.lblTemplatesLabel.Size = new System.Drawing.Size(64, 16);
			this.lblTemplatesLabel.TabIndex = 9;
			this.lblTemplatesLabel.Text = "Templates";
			this.lblTemplatesLabel.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// cbClassTemplates
			// 
			this.cbClassTemplates.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.cbClassTemplates.Location = new System.Drawing.Point(80, 8);
			this.cbClassTemplates.Name = "cbClassTemplates";
			this.cbClassTemplates.Size = new System.Drawing.Size(264, 21);
			this.cbClassTemplates.TabIndex = 8;
			this.cbClassTemplates.SelectedIndexChanged += new System.EventHandler(this.cbClassTemplates_SelectedIndexChanged);
			// 
			// splitter2
			// 
			this.splitter2.Location = new System.Drawing.Point(648, 0);
			this.splitter2.Name = "splitter2";
			this.splitter2.Size = new System.Drawing.Size(3, 368);
			this.splitter2.TabIndex = 8;
			this.splitter2.TabStop = false;
			// 
			// pnlSelectedTreeView
			// 
			this.pnlSelectedTreeView.Controls.Add(this.tvSelected);
			this.pnlSelectedTreeView.Dock = System.Windows.Forms.DockStyle.Left;
			this.pnlSelectedTreeView.Location = new System.Drawing.Point(331, 0);
			this.pnlSelectedTreeView.Name = "pnlSelectedTreeView";
			this.pnlSelectedTreeView.Size = new System.Drawing.Size(317, 368);
			this.pnlSelectedTreeView.TabIndex = 7;
			// 
			// splitter1
			// 
			this.splitter1.Location = new System.Drawing.Point(328, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(3, 368);
			this.splitter1.TabIndex = 1;
			this.splitter1.TabStop = false;
			// 
			// pnlDiskTreeView
			// 
			this.pnlDiskTreeView.Controls.Add(this.btnBrowse);
			this.pnlDiskTreeView.Controls.Add(this.tbPath);
			this.pnlDiskTreeView.Controls.Add(this.tvDisk);
			this.pnlDiskTreeView.Dock = System.Windows.Forms.DockStyle.Left;
			this.pnlDiskTreeView.Location = new System.Drawing.Point(0, 0);
			this.pnlDiskTreeView.Name = "pnlDiskTreeView";
			this.pnlDiskTreeView.Size = new System.Drawing.Size(328, 368);
			this.pnlDiskTreeView.TabIndex = 0;
			// 
			// AssetImportPlacer
			// 
			this.Controls.Add(this.panel);
			this.Name = "AssetImportPlacer";
			this.Size = new System.Drawing.Size(1016, 384);
			this.panel.ResumeLayout(false);
			this.pnlAssetTreeView.ResumeLayout(false);
			this.pnlSelectedTreeView.ResumeLayout(false);
			this.pnlDiskTreeView.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
		#endregion
		#region Member vars
		private FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
		private bool diskSelectedMirroringEnabled = true;

		private ClassificationTemplateInfo currentTemplate = null;
		private bool stifleTemplateChangeHandler = false;
		
		private CLASS_LOAD_TYPE classLoadType = CLASS_LOAD_TYPE.FILES;
		public bool IsImporterUsingTemplates()
		{
			return (classLoadType == CLASS_LOAD_TYPE.FILES);
		}

		private bool loadedDirAlready = false;
		#endregion
		#region Enums
		private enum CLASS_LOAD_TYPE { NONESPECIFIED, FILES, PROJECT }
		#endregion
		#region Properties
		public string CurrentTemplatePath
		{
			get 
			{
				if (this.currentTemplate != null)
					return this.currentTemplate.path;
				else
					return "";
			}
		}

		public ArrayList Platforms
		{
			get { return this.tvAssets.Platforms; }
			set { this.tvAssets.Platforms = value; }
		}

		public string ProjectName
		{
			get { return this.tvAssets.RootName; }
			set { this.tvAssets.RootName = value; }
		}

		public string ProjectPath
		{
			get { return this.tbPath.Text; }
			set { this.tbPath.Text = value; }
		}

		public TreeNodeCollection AssetTreeNodes
		{
			get { return this.tvAssets.Nodes; }
		}

		public bool IncludeRootDir
		{
			get { return this.tvDisk.IncludeRootDir; }
			set { this.tvDisk.IncludeRootDir = value; }
		}

		public bool LoadedDirAlready
		{
			get { return this.loadedDirAlready; }
		}

		public bool UseFileExtensionsInAssetNames
		{
			get { return this.tvAssets.UseFileExtensionsInAssetNames; }
			set { this.tvAssets.UseFileExtensionsInAssetNames = value; }
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
		#region Events
		// Events for notifying people that we are loading a lot of directory info, and they might want to disable controls that might screw us up
		public event EventHandler Event_LoadingDirectories;
		public event EventHandler Event_DoneLoadingDirectories;
		#endregion
		#region Constructor
		public AssetImportPlacer()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
			this.folderBrowserDialog.SelectedPath = "";
			this.tbPath.Text = folderBrowserDialog.SelectedPath;

			// hook up events
			this.tvAssets.DroppedNodes += new EventHandler(tvAssets_DroppedNodes);

			// teach the trees about each other
			this.tvDisk.AssetTreeView = this.tvAssets;
			this.tvDisk.SelectedTreeView = this.tvSelected;
			this.tvSelected.DiskTreeView = this.tvDisk;
			this.tvSelected.AssetTreeView = this.tvAssets;
			this.tvAssets.DiskTreeView = this.tvDisk;
			this.tvAssets.SelectedTreeView = this.tvSelected;

			this.tvDisk.AssetImportPlacer = this;
		}
		#endregion
		#region Public functions
		// Raise the loading directories event, in case the form that contians this control wants to disable its buttons and stuff
		public void RaiseEvent_LoadingDirectories()
		{
			if (this.Event_LoadingDirectories != null)
			{
				this.Event_LoadingDirectories(this, new EventArgs());
			}
		}

		// Raise the done loading directories event
		public void RaiseEvent_DoneLoadingDirectories()
		{
			if (this.Event_DoneLoadingDirectories != null)
			{
				this.Event_DoneLoadingDirectories(this, new EventArgs());
			}
		}

		
		public void LoadClassesFromProject(MOG_Project proj)
		{
			this.classLoadType = CLASS_LOAD_TYPE.PROJECT;
			this.lblTemplatesLabel.Enabled = false;
			this.lblTemplatesLabel.Hide();
			this.cbClassTemplates.Items.Clear();
			this.cbClassTemplates.Text = "";
			this.cbClassTemplates.Enabled = false;
			this.cbClassTemplates.Hide();
			this.tvAssets.LoadClassesFromProject(proj);
		}

		public bool LoadDefaultConfigurationsFromFiles(string path)
		{
			this.classLoadType = CLASS_LOAD_TYPE.FILES;
			this.lblTemplatesLabel.Show();
			this.lblTemplatesLabel.Enabled = true;
			this.cbClassTemplates.Show();
			this.cbClassTemplates.Enabled = true;
			LoadTemplates();
			return this.tvAssets.LoadDefaultConfigurationsFromFiles(path);
		}
		#endregion
		#region Private functions

		private void DisableEverything()
		{
			this.Enabled = false;
		}

		private void EnableEverything()
		{
			this.Enabled = true;
		}

		private void LoadTemplates()
		{
			// Blank out our list
			this.cbClassTemplates.Items.Clear();
			this.cbClassTemplates.SelectedItem = null;

			// Scan the known templates
			foreach (ClassificationTemplateInfo tem in ClassificationTemplateLoader.GetTemplates())
			{
				this.cbClassTemplates.Items.Add(tem);

				// Special code to help determine the correct default templates to use
				if (tem.name.ToLower() == "default")
				{
					// Always set default to be the default because it is the desired default
					this.stifleTemplateChangeHandler = true;
					this.cbClassTemplates.SelectedItem = tem;
					this.currentTemplate = tem;
					this.stifleTemplateChangeHandler = false;
				}
				// Special code to use the library template as a default
				if (tem.name.ToLower() == "library")
				{
					// Check if we are still missing a default?
					if (this.currentTemplate == null)
					{
						// Only set the library as default if we haven't already found 'default'
						this.stifleTemplateChangeHandler = true;
						this.cbClassTemplates.SelectedItem = tem;
						this.currentTemplate = tem;
						this.stifleTemplateChangeHandler = false;
					}
				}
			}
		}

		private void ResetClasses(string path)
		{
			switch (this.classLoadType)
			{
				case CLASS_LOAD_TYPE.FILES:
					this.tvAssets.LoadDefaultConfigurationsFromFiles(path);
					break;
				case CLASS_LOAD_TYPE.PROJECT:
					this.tvAssets.LoadClassesFromProject(MOG.CONTROLLER.CONTROLLERPROJECT.MOG_ControllerProject.GetProject());
					break;
			}
		}

		public void ResetEverything()
		{
			this.tvDisk.ResetEverything();
			this.tvSelected.ResetEverything();
			this.tvAssets.ResetEverything();

			// Reset the default template classes
			string defaultTemplatePath = MOG_ControllerSystem.LocateInstallItem("ProjectTemplates\\Default");
			if (defaultTemplatePath.Length == 0)
			{
				// Fallback to using the library template
				defaultTemplatePath = MOG_ControllerSystem.LocateInstallItem("ProjectTemplates\\Library");
			}
			ResetClasses(defaultTemplatePath);

			FillAssetTreeWithTemplate(false);
		}

		private void ConformSelectedRootOrderToDisk()
		{
		}
		
		// for each selected node in the selected tree, highlight those nodes in the asset tree
		private void MatchSelectedNodes_SelectedToAsset()
		{
			// first clear selected nodes from asset tree
			this.tvAssets.SelectedNodes.Clear();

			foreach (AssetTreeNode selNode in this.tvSelected.SelectedNodes)
			{
				if (selNode.AssetNode != null  &&  selNode.AssetNode.TreeView == this.tvAssets)
				{
					if (selNode.AssetNode.IsAFile  &&  selNode.AssetNode.AssetFilenameNode != null)
						this.tvAssets.SelectedNodes.Add(selNode.AssetNode.AssetFilenameNode);
					else
						this.tvAssets.SelectedNodes.Add(selNode.AssetNode);
				}
			}
		}

		private void tvAssets_DroppedNodes(object sender, EventArgs args)
		{
			MatchSelectedNodes_SelectedToAsset();
		}

		private AssetTreeNode EncodeDirectoryTree(string path)
		{
			if (Directory.Exists(path))
			{
				AssetTreeNode tn = new AssetTreeNode( Path.GetFileName(path) );
				tn.IsAFolder = true;
				
				foreach (string subdirPath in Directory.GetDirectories(path))
					tn.Nodes.Add( EncodeDirectoryTree(subdirPath) );

				foreach (string filePath in Directory.GetFiles(path))
				{
					AssetTreeNode fileNode = new AssetTreeNode( Path.GetFileName(filePath) );
					fileNode.IsAFile = true;
					tn.Nodes.Add( fileNode );
				}

				return tn;
			}
			
			return null;
		}
		#endregion
		#region Event handlers
		private void btnBrowse_Click(object sender, System.EventArgs e)
		{
			// If the directory in the text box exists, make it the folder browser's default location
			if (Directory.Exists(this.tbPath.Text))
			{
				this.folderBrowserDialog.SelectedPath = this.tbPath.Text;
			}

			if (this.folderBrowserDialog.ShowDialog() == DialogResult.OK)
			{
				if (this.loadedDirAlready)
				{
					if (Utils.ShowMessageBoxConfirmation("Loading a new directory will reset all the assets and classifications you've placed.\nAre you sure you want to continue?", "WARNING") != MOGPromptResult.Yes)
						return;
				}

				// Make sure no one can mess around with the control while we're loading
				DisableEverything();
				// Tell any listeners that we're loading a potentially huge directory tree
				RaiseEvent_LoadingDirectories();

				Cursor.Current = Cursors.WaitCursor;

				// Clear the TreeViews and such
				ResetEverything();

				// Load up
				this.tbPath.Text = this.folderBrowserDialog.SelectedPath;
				this.tvDisk.LoadTreeFromDisk(this.folderBrowserDialog.SelectedPath);
				this.loadedDirAlready = true;
				
				// Re-eneable the interface
				EnableEverything();
				// Tell everyone we're done loading directories
				RaiseEvent_DoneLoadingDirectories();

				Cursor.Current = Cursors.Default;
			}
		}



		private void tvDisk_AfterCheck(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			this.diskSelectedMirroringEnabled = false;

			if (e.Node.Checked)
			{
				this.tvSelected.NewNode(e.Node as AssetTreeNode);				
			}
			else
			{
				this.tvSelected.RemoveNode(e.Node as AssetTreeNode);
			}

			ConformSelectedRootOrderToDisk();

			// Force an update of the selected tree's red status to catch any empty directories trees without any files (those will always be green)
//			this.tvSelected.RefreshRedStatus();
			this.tvSelected.RefreshColorStatusAboveNode(e.Node as AssetTreeNode);
            
			this.diskSelectedMirroringEnabled = true;
		}

		#region Disk-Selected mirroring functions
		private void tvSelected_AfterCollapse(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			if (!this.diskSelectedMirroringEnabled)
				return;

			AssetTreeNode tn = e.Node as AssetTreeNode;
			if (tn != null  &&  tn.DiskNode != null)
			{
				tn.DiskNode.EnsureVisible();
				tn.DiskNode.Collapse();
			}
		}

		private void tvSelected_AfterExpand(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			if (!this.diskSelectedMirroringEnabled)
				return;

			AssetTreeNode tn = e.Node as AssetTreeNode;
			if (tn != null  &&  tn.DiskNode != null)
			{
				tn.DiskNode.EnsureVisible();
				tn.DiskNode.Expand();
			}
		}

		private void tvSelected_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (!this.diskSelectedMirroringEnabled)
				return;

			this.tvDisk.SelectedNodes.Clear();
			foreach (AssetTreeNode tn in this.tvSelected.SelectedNodes)
			{
				if (tn.DiskNode != null)//  &&  tn.TreeView == this.tvDisk)
					this.tvDisk.SelectedNodes.Add(tn.DiskNode);
			}	
		}
		
		private void tvDisk_AfterCollapse(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			if (!this.diskSelectedMirroringEnabled)
				return;

			AssetTreeNode tn = e.Node as AssetTreeNode;
			if (tn != null  &&  tn.SelectedNode != null)
			{
				tn.SelectedNode.EnsureVisible();
				tn.SelectedNode.Collapse();
			}
		}

		private void tvDisk_AfterExpand(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			if (!this.diskSelectedMirroringEnabled)
				return;

			AssetTreeNode tn = e.Node as AssetTreeNode;
			if (tn != null  &&  tn.SelectedNode != null)
			{
// JohnRen - Removed because this was seriously slowing down the importation of larger 100k+ file projects
//				this.tvDisk.BeginUpdate();
//				this.tvSelected.BeginUpdate();
				Cursor.Current = Cursors.WaitCursor;
				Application.DoEvents();

				tn.SelectedNode.EnsureVisible();
				tn.SelectedNode.Expand();

				// Check if we are checked?
// JohnRen - Removed because this was seriously slowing down the importation of larger projects
//				if (tn.Checked)
//				{
//					// Propagate the check down to this node's children
//					foreach (AssetTreeNode subNode in tn.Nodes)
//					{
//						subNode.Checked = tn.Checked;
//					}
//				}

				Cursor.Current = Cursors.Default;
// JohnRen - Removed because this was seriously slowing down the importation of larger 100k+ file projects
//				this.tvDisk.EndUpdate();
//				this.tvSelected.EndUpdate();
			}
		}

		private void tvDisk_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (!this.diskSelectedMirroringEnabled)
			{
				return;
			}
			this.tvSelected.SelectedNodes.Clear();
			foreach (AssetTreeNode tn in this.tvDisk.SelectedNodes)
			{
				if (tn.SelectedNode != null)
					this.tvSelected.SelectedNodes.Add(tn.SelectedNode);
			}	
		}

		#endregion

		private void cbClassTemplates_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.stifleTemplateChangeHandler)
				return;

			FillAssetTreeWithTemplate(true);
		}

		private void FillAssetTreeWithTemplate(bool warn)
		{
			ClassificationTemplateInfo template = this.cbClassTemplates.SelectedItem as ClassificationTemplateInfo;
			if (template != null)
			{
				if (warn && Utils.ShowMessageBoxConfirmation("Are you sure you want to change the template?\nIf you proceed, all placed assets will need to be re-added.", "Confirm Template Change") != MOGPromptResult.Yes)
				{
					this.stifleTemplateChangeHandler = true;
					this.cbClassTemplates.SelectedItem = this.currentTemplate;
					this.stifleTemplateChangeHandler = false;
					return;
				}

				this.tvAssets.DeleteAllAssets();
				this.tvAssets.LoadDefaultConfigurationsFromFiles(template.path);
				this.currentTemplate = template;
			}
		}

		private void tbPath_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			// Load the directory in the textbox on Enter
			if (e.KeyData == Keys.Enter)
			{
				// Make sure the directory they specified exists
				if (!Directory.Exists(this.tbPath.Text))
				{
					Utils.ShowMessageBoxExclamation(this.tbPath.Text + " does not exist", "Path Not Found");
					return;
				}

				// Throw up a warning window if they've already loaded a directory
				if (this.loadedDirAlready)
				{
					if (Utils.ShowMessageBoxConfirmation("Loading a new directory will reset all the assets and classifications you've placed.\nAre you sure you want to continue?", "WARNING") != MOGPromptResult.Yes)
					{
						return;
					}
				}

				// Tell the world that we're loading a directory
				RaiseEvent_LoadingDirectories();

				// Reset all the panes
				ResetEverything();

				// Load the directory and flag that we've loaded already
				this.tvDisk.LoadTreeFromDisk( this.tbPath.Text );
				this.loadedDirAlready = true;

				// Tell the world we're done
				RaiseEvent_DoneLoadingDirectories();
			}
		}
	}
		#endregion

	#region Helpers
	class ClassificationTemplateLoader
	{
		public static ArrayList GetTemplates()
		{
			ArrayList templates = new ArrayList();

			foreach (string templateDir in Directory.GetDirectories( MOG_ControllerSystem.LocateInstallItem("ProjectTemplates")))
				templates.Add(new ClassificationTemplateInfo( Path.GetFileName(templateDir), templateDir ));

			return templates;
		}
	}

	class ClassificationTemplateInfo
	{
		public string name = "";
		public string path = "";

		public ClassificationTemplateInfo(string name, string path)
		{
			this.name = name;
			this.path = path;
		}

		public override string ToString()
		{
			return this.name;
		}

	}
	#endregion

}




