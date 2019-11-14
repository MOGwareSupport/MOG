using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using MOG_ControlsLibrary;
using MOG_ControlsLibrary.Utils;
using MOG_ControlsLibrary.Controls;

using MOG;
using MOG.INI;
using MOG.PROJECT;
using MOG.TIME;
using MOG.PROPERTIES;
using MOG.PLATFORM;
using MOG.DOSUTILS;
using MOG.FILENAME;
using MOG.DATABASE;
using MOG.PROMPT;
using MOG.REPORT;
using MOG.CONTROLLER.CONTROLLERPACKAGE;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERASSET;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.CONTROLLER.CONTROLLERREPOSITORY;
using MOG_ControlsLibrary.Forms;

namespace MOG_ControlsLibrary.Controls
{
	/// <summary>
	/// Summary description for MogControl_PackageManagementTreeView.
	/// </summary>
	public class MogControl_PackageManagementTreeView : System.Windows.Forms.UserControl
	{
		public System.Windows.Forms.ContextMenuStrip PackageTreeContextMenu;
		#region properties
		[Category("Appearance"), Description("Governs whether the 'Platform Specific' CheckBox is shown")]
		public bool UsePlatformSpecificCheckBox
		{
			get { return PackagePlatformsCheckBox.Visible; }
			set
			{
				PackagePlatformsCheckBox.Visible = value;
				PackageTreeViewBottomPanel.Visible = value;
				Update();
			}
		}

		[Browsable( false)]
		public MogControl_PackageTreeView TreeView
		{
			get { return ProjectPackagesTreeView; }
		}
		#region Form variables
		[Category("Behavior"), Description("If true, allows tree to expand down to Asset level.")]
		public bool ExpandAssets
		{
			get { return ProjectPackagesTreeView.ExpandAssets; }
			set { ProjectPackagesTreeView.ExpandAssets = value; }
		}
		private System.Windows.Forms.ToolStripMenuItem PackageNewClassificationMenuItem;
		private System.Windows.Forms.ToolStripSeparator PackageSeparator1MenuItem;
		private System.Windows.Forms.ToolStripMenuItem PackageNewPackageMenuItem;
		private System.Windows.Forms.ToolStripMenuItem PackageNewGroupMenuItem;
		private System.Windows.Forms.ToolStripSeparator PackageSeparator2MenuItem;
		private System.Windows.Forms.ToolStripMenuItem PackageRemoveMenuItem;
		private System.Windows.Forms.ToolStripMenuItem PackageNewObjectMenuItem;
		private System.Windows.Forms.ImageList PackagesImageList;
		private System.Windows.Forms.Panel PackageTreeViewBottomPanel;
		private System.Windows.Forms.CheckBox PackagePlatformsCheckBox;
		public MOG_ControlsLibrary.Controls.MogControl_PackageTreeView ProjectPackagesTreeView;
		private System.Windows.Forms.ImageList StateImageList;
		private ToolStripMenuItem PackageNewPackageSubMenu;
		private ToolStripMenuItem allToolStripMenuItem;
		private System.ComponentModel.IContainer components;
		#endregion

		[Category("Behavior"), Description("Occurs when selection has been changed")]
		public event TreeViewEventHandler AfterPackageSelect;

		public string SelectedPackageFullFilename
		{
			get
			{
				if (this.ProjectPackagesTreeView.SelectedNode != null)
				{
					// Find our parent package
					TreeNode package = FindPackage(this.ProjectPackagesTreeView.SelectedNode);
					if (package != null)
					{
						return ((Mog_BaseTag)package.Tag).FullFilename;
					}
				}

				return "";
			}
		}

        public string SelectedPackageFullName
        {
            get
            {
                if (this.ProjectPackagesTreeView.SelectedNode != null)
                {
                    // Find our parent package
                    TreeNode package = FindPackage(this.ProjectPackagesTreeView.SelectedNode);
                    if (package != null)
                    {
                        return ((Mog_BaseTag)package.Tag).PackageFullName;
                    }
                }

                return "";
            }
        }
		#endregion properties
		
		public MogControl_PackageManagementTreeView()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// Make sure to initialize this based on the control
			ProjectPackagesTreeView.ShowPlatformSpecific = PackagePlatformsCheckBox.Checked;

			if (!DesignMode)
			{
				MOG_Project project = MOG_ControllerProject.GetProject();

				if (project != null)
				{
					foreach (string platformName in project.GetPlatformNames())
					{
						PackageNewPackageSubMenu.DropDownItems.Add(platformName, null, PackageNewPackageMenuItem_Click);
					}

					PackageNewPackageSubMenu.DropDownItems.Add(MOG_ControllerProject.GetAllPlatformsString(), null, PackageNewPackageMenuItem_Click);
				}
			}
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MogControl_PackageManagementTreeView));
			this.PackageTreeContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.PackageNewClassificationMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.PackageSeparator1MenuItem = new System.Windows.Forms.ToolStripSeparator();
			this.PackageNewPackageSubMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.allToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.PackageNewPackageMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.PackageNewGroupMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.PackageNewObjectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.PackageSeparator2MenuItem = new System.Windows.Forms.ToolStripSeparator();
			this.PackageRemoveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.PackagesImageList = new System.Windows.Forms.ImageList(this.components);
			this.PackageTreeViewBottomPanel = new System.Windows.Forms.Panel();
			this.PackagePlatformsCheckBox = new System.Windows.Forms.CheckBox();
			this.ProjectPackagesTreeView = new MOG_ControlsLibrary.Controls.MogControl_PackageTreeView();
			this.StateImageList = new System.Windows.Forms.ImageList(this.components);
			this.PackageTreeContextMenu.SuspendLayout();
			this.PackageTreeViewBottomPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// PackageTreeContextMenu
			// 
			this.PackageTreeContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.PackageNewClassificationMenuItem,
            this.PackageSeparator1MenuItem,
            this.PackageNewPackageSubMenu,
            this.PackageNewPackageMenuItem,
            this.PackageNewGroupMenuItem,
            this.PackageNewObjectMenuItem,
            this.PackageSeparator2MenuItem,
            this.PackageRemoveMenuItem});
			this.PackageTreeContextMenu.Name = "PackageTreeContextMenu";
			this.PackageTreeContextMenu.Size = new System.Drawing.Size(218, 148);
			this.PackageTreeContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.PackageTreeContextMenu_Popup);
			// 
			// PackageNewClassificationMenuItem
			// 
			this.PackageNewClassificationMenuItem.Name = "PackageNewClassificationMenuItem";
			this.PackageNewClassificationMenuItem.Size = new System.Drawing.Size(217, 22);
			this.PackageNewClassificationMenuItem.Text = "Create new classification";
			this.PackageNewClassificationMenuItem.Click += new System.EventHandler(this.PackageNewClassificationMenuItem_Click);
			// 
			// PackageSeparator1MenuItem
			// 
			this.PackageSeparator1MenuItem.Name = "PackageSeparator1MenuItem";
			this.PackageSeparator1MenuItem.Size = new System.Drawing.Size(214, 6);
			// 
			// PackageNewPackageSubMenu
			// 
			this.PackageNewPackageSubMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allToolStripMenuItem});
			this.PackageNewPackageSubMenu.Name = "PackageNewPackageSubMenu";
			this.PackageNewPackageSubMenu.Size = new System.Drawing.Size(217, 22);
			this.PackageNewPackageSubMenu.Text = "Create new package";
			// 
			// allToolStripMenuItem
			// 
			this.allToolStripMenuItem.Name = "allToolStripMenuItem";
			this.allToolStripMenuItem.Size = new System.Drawing.Size(96, 22);
			this.allToolStripMenuItem.Text = "All";
			this.allToolStripMenuItem.Click += new System.EventHandler(this.PackageNewPackageMenuItem_Click);
			// 
			// PackageNewPackageMenuItem
			// 
			this.PackageNewPackageMenuItem.Name = "PackageNewPackageMenuItem";
			this.PackageNewPackageMenuItem.Size = new System.Drawing.Size(217, 22);
			this.PackageNewPackageMenuItem.Text = "Create new package";
			this.PackageNewPackageMenuItem.Click += new System.EventHandler(this.PackageNewPackageMenuItem_Click);
			// 
			// PackageNewGroupMenuItem
			// 
			this.PackageNewGroupMenuItem.Name = "PackageNewGroupMenuItem";
			this.PackageNewGroupMenuItem.Size = new System.Drawing.Size(217, 22);
			this.PackageNewGroupMenuItem.Text = "Create new group";
			this.PackageNewGroupMenuItem.Click += new System.EventHandler(this.PackageNewGroupMenuItem_Click);
			// 
			// PackageNewObjectMenuItem
			// 
			this.PackageNewObjectMenuItem.Name = "PackageNewObjectMenuItem";
			this.PackageNewObjectMenuItem.Size = new System.Drawing.Size(217, 22);
			this.PackageNewObjectMenuItem.Text = "Create new package object";
			this.PackageNewObjectMenuItem.Click += new System.EventHandler(this.PackageNewObjectMenuItem_Click);
			// 
			// PackageSeparator2MenuItem
			// 
			this.PackageSeparator2MenuItem.Name = "PackageSeparator2MenuItem";
			this.PackageSeparator2MenuItem.Size = new System.Drawing.Size(214, 6);
			// 
			// PackageRemoveMenuItem
			// 
			this.PackageRemoveMenuItem.Name = "PackageRemoveMenuItem";
			this.PackageRemoveMenuItem.Size = new System.Drawing.Size(217, 22);
			this.PackageRemoveMenuItem.Text = "Remove";
			this.PackageRemoveMenuItem.Click += new System.EventHandler(this.PackageRemoveMenuItem_Click);
			// 
			// PackagesImageList
			// 
			this.PackagesImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("PackagesImageList.ImageStream")));
			this.PackagesImageList.TransparentColor = System.Drawing.Color.Transparent;
			this.PackagesImageList.Images.SetKeyName(0, "");
			this.PackagesImageList.Images.SetKeyName(1, "");
			this.PackagesImageList.Images.SetKeyName(2, "");
			this.PackagesImageList.Images.SetKeyName(3, "");
			// 
			// PackageTreeViewBottomPanel
			// 
			this.PackageTreeViewBottomPanel.Controls.Add(this.PackagePlatformsCheckBox);
			this.PackageTreeViewBottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.PackageTreeViewBottomPanel.Location = new System.Drawing.Point(0, 152);
			this.PackageTreeViewBottomPanel.Name = "PackageTreeViewBottomPanel";
			this.PackageTreeViewBottomPanel.Size = new System.Drawing.Size(192, 24);
			this.PackageTreeViewBottomPanel.TabIndex = 9;
			// 
			// PackagePlatformsCheckBox
			// 
			this.PackagePlatformsCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.PackagePlatformsCheckBox.BackColor = System.Drawing.SystemColors.Control;
			this.PackagePlatformsCheckBox.Checked = true;
			this.PackagePlatformsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.PackagePlatformsCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PackagePlatformsCheckBox.Location = new System.Drawing.Point(8, 0);
			this.PackagePlatformsCheckBox.Name = "PackagePlatformsCheckBox";
			this.PackagePlatformsCheckBox.Size = new System.Drawing.Size(144, 24);
			this.PackagePlatformsCheckBox.TabIndex = 8;
			this.PackagePlatformsCheckBox.Text = "Show platform-specific";
			this.PackagePlatformsCheckBox.UseVisualStyleBackColor = false;
			this.PackagePlatformsCheckBox.CheckedChanged += new System.EventHandler(this.PackagePlatformsCheckBox_CheckedChanged);
			// 
			// ProjectPackagesTreeView
			// 
			this.ProjectPackagesTreeView.AllowItemDrag = true;
			this.ProjectPackagesTreeView.ArchivedNodeForeColor = System.Drawing.SystemColors.WindowText;
			this.ProjectPackagesTreeView.ContextMenuStrip = this.PackageTreeContextMenu;
			this.ProjectPackagesTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ProjectPackagesTreeView.ExclusionList = "";
			this.ProjectPackagesTreeView.ExpandAssets = false;
			this.ProjectPackagesTreeView.ExpandPackageGroupAssets = false;
			this.ProjectPackagesTreeView.ExpandPackageGroups = true;
			this.ProjectPackagesTreeView.FocusForAssetNodes = MOG_ControlsLibrary.Controls.LeafFocusLevel.PackageGroup;
			this.ProjectPackagesTreeView.HideSelection = false;
			this.ProjectPackagesTreeView.HotTracking = true;
			this.ProjectPackagesTreeView.ImageIndex = 0;
			this.ProjectPackagesTreeView.LastNodePath = null;
			this.ProjectPackagesTreeView.Location = new System.Drawing.Point(0, 0);
			this.ProjectPackagesTreeView.Name = "ProjectPackagesTreeView";
			this.ProjectPackagesTreeView.NodesDefaultToChecked = false;
			this.ProjectPackagesTreeView.PathSeparator = "~";
			this.ProjectPackagesTreeView.PersistantHighlightSelectedNode = false;
			this.ProjectPackagesTreeView.SelectedImageIndex = 0;
			this.ProjectPackagesTreeView.ShowAssets = true;
			this.ProjectPackagesTreeView.ShowDescription = false;
			this.ProjectPackagesTreeView.ShowPlatformSpecific = false;
			this.ProjectPackagesTreeView.ShowToolTips = false;
			this.ProjectPackagesTreeView.Size = new System.Drawing.Size(192, 152);
			this.ProjectPackagesTreeView.TabIndex = 10;
			this.ProjectPackagesTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ProjectPackagesTreeView_AfterSelect);
			// 
			// StateImageList
			// 
			this.StateImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("StateImageList.ImageStream")));
			this.StateImageList.TransparentColor = System.Drawing.Color.Transparent;
			this.StateImageList.Images.SetKeyName(0, "");
			this.StateImageList.Images.SetKeyName(1, "");
			this.StateImageList.Images.SetKeyName(2, "");
			this.StateImageList.Images.SetKeyName(3, "");
			// 
			// MogControl_PackageManagementTreeView
			// 
			this.Controls.Add(this.ProjectPackagesTreeView);
			this.Controls.Add(this.PackageTreeViewBottomPanel);
			this.Name = "MogControl_PackageManagementTreeView";
			this.Size = new System.Drawing.Size(192, 176);
			this.Load += new System.EventHandler(this.MogControl_PackageManagementTreeView_Load);
			this.PackageTreeContextMenu.ResumeLayout(false);
			this.PackageTreeViewBottomPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region Utility functions
		/// <summary>
		/// Find a tree node going down the tree that matches the passed in string
		/// </summary>
		private TreeNode FindNode(string text, TreeNode parent)
		{
			// Check all the children of this parent
			foreach (TreeNode node in parent.Nodes)
			{
				if (string.Compare(node.Text, text, true) == 0)
				{
					return node;
				}				
			}

			return null;
		}

		/// <summary>
		/// Walk up the parents of a node looking for a parent with a valid MOG_Filename
		/// in the tag as well as the isPackage() bool set to true
		/// </summary>
		private TreeNode FindPackage(TreeNode node)
		{
			try
			{
				MOG_Filename assetName = new MOG_Filename(((MOG_ControlsLibrary.Controls.Mog_BaseTag)node.Tag).FullFilename);
				MOG_Properties props = new MOG_Properties(assetName);
				if (assetName != null && props.IsPackage)
				{
					return node;
				}
				else
				{
					if (node.Parent != null)
					{
						return FindPackage(node.Parent);
					}
					else
					{
						return null;
					}
				}
			}
			catch
			{
				if (node.Parent != null)
				{
					return FindPackage(node.Parent);
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Walk up the parent chain putting together our class
		/// </summary>
		private string FindClassification(TreeNode node)
		{
			try
			{
				Mog_BaseTag tag = (Mog_BaseTag)node.Tag;

				if (tag.PackageNodeType == PackageNodeTypes.Class)
				{
					if (node.Parent != null)
					{
						return FindClassification(node.Parent) + "~" + node.Text;
					}
					else
					{
						return node.Text;
					}
				}
				else
				{
					if (node.Parent != null)
					{
						return FindClassification(node.Parent);
					}
					else
					{
						return "";
					}
				}
			}
			catch
			{
				if (node != null)
				{
					if (node.Parent != null)
					{
						return FindClassification(node.Parent);
					}
					else
					{
						return "";
					}
				}
			}

			return "";
		}

		/// <summary>
		/// Isolate the package path from the full package string using the classification of this node
		/// </summary>
		private string IsolatePackagePath(string packageString, string classification)
		{
			// Convert the classification to have the same delimeters as the packageString
			string packageDelimClassification = classification.Replace("~", "/").ToLower();

			// Check to see if the classification exists in the packageString
			if (packageString.ToLower().IndexOf(packageDelimClassification) != -1)
			{
				string packagePath = packageString.ToLower().Replace(packageDelimClassification, "");

				// Now make sure our packagePath does not start with a delimeter
				packagePath = packagePath.TrimStart("/".ToCharArray());

				return packagePath;
			}
			
			return "";
		}
		
		/// <summary>
		/// Return the image index for this node based on NodeType enum
		/// </summary>
		private int GetClassIconIndex(MOG_ControlsLibrary.Controls.PackageNodeTypes type)
		{
			switch(type)
			{
				case PackageNodeTypes.Class:		return 0;
				case PackageNodeTypes.Asset:		return 1;
				case PackageNodeTypes.Package:		return 1;
				case PackageNodeTypes.Group:		return 2;
				case PackageNodeTypes.Object:		return 3;
			}
			
			return 0;
		}
		#endregion

		#region Package Tree methods
		private void PackageNewClassificationMenuItem_Click(object sender, EventArgs e)
		{
			MOG_Privileges privs = MOG_ControllerProject.GetPrivileges();
			if (privs.GetUserPrivilege(MOG_ControllerProject.GetUserName(), MOG_PRIVILEGE.AddClassification))
			{
				ToolStripMenuItem item = sender as ToolStripMenuItem;
				if (item != null)
				{
					ContextMenuStrip contextMenu = item.Owner as ContextMenuStrip;
					if (contextMenu != null)
					{
						MogControl_BaseTreeView treeview = contextMenu.SourceControl as MogControl_BaseTreeView;
						if (treeview != null)
						{
							TreeNode node = treeview.SelectedNode;
							if (node != null)
							{
								ClassificationCreateForm form = new ClassificationCreateForm(node.FullPath);
								if (form.ShowDialog(treeview.TopLevelControl) == DialogResult.OK)
								{
									treeview.DeInitialize();
									treeview.LastNodePath = form.FullClassificationName;
									treeview.Initialize();
								}
							}
						}
					}
				}
			}
			else
			{
				MOG_Prompt.PromptResponse("Insufficient Privileges", "Your privileges do not allow you to add classifications to the project.");
			}
		}
		
		/// <summary>
		/// Handle the 'New package' menu item click
		/// </summary>
		private void PackageNewPackageMenuItem_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem menuItem = sender as ToolStripMenuItem;

			if (ProjectPackagesTreeView.SelectedNode != null)
			{
				Mog_BaseTag tag = ProjectPackagesTreeView.SelectedNode.Tag as Mog_BaseTag;
				if (tag != null && tag.PackageNodeType == PackageNodeTypes.Class)
				{
					PackageCreator creator = new PackageCreator();
					creator.Classification = ProjectPackagesTreeView.SelectedNode.FullPath;

					if (menuItem != null)
					{
						if (MOG_ControllerProject.IsValidPlatform(menuItem.Text) || 
							String.Compare(menuItem.Text, MOG_ControllerProject.GetAllPlatformsString(), true) == 0)
						{
							creator.Platform = menuItem.Text;
						}
					}

					if (creator.ShowDialog(this) == DialogResult.OK)
					{
						if (creator.AssetName != null)
						{
							// Re-create the tree then drill down to the newly created package
							ProjectPackagesTreeView.DeInitialize();
							ProjectPackagesTreeView.LastNodePath = creator.AssetName.GetAssetClassification() + ProjectPackagesTreeView.PathSeparator + creator.AssetName.GetAssetName();
							ProjectPackagesTreeView.Initialize();
						}
					}
				}
			}
		}

		/// <summary>
		/// Handle the 'New Group' menn item click
		/// </summary>
		private void PackageNewGroupMenuItem_Click(object sender, System.EventArgs e)
		{
			if (ProjectPackagesTreeView.SelectedNode != null)
			{
				// Create a new editable group node
				TreeNode group = new TreeNode("NewGroup");
				
				// Set our icon
				group.ImageIndex = GetClassIconIndex(PackageNodeTypes.Group);
				
				ProjectPackagesTreeView.SelectedNode.Expand();
				ProjectPackagesTreeView.SelectedNode.Nodes.Add(group);
				ProjectPackagesTreeView.SelectedNode.Expand();			
				ProjectPackagesTreeView.LabelEdit = true;
				group.BeginEdit();
			}
		}

		/// <summary>
		/// Handle the '(NewPackageObject)' menu item click
		/// </summary>
		private void PackageNewObjectMenuItem_Click(object sender, System.EventArgs e)
		{
			if (ProjectPackagesTreeView.SelectedNode != null)
			{
				// Create a new editable group node
				TreeNode packageObject = new TreeNode("(NewPackageObject)");
				// Set our icon
				packageObject.ImageIndex = GetClassIconIndex(PackageNodeTypes.Object);
				
				ProjectPackagesTreeView.SelectedNode.Expand();
				ProjectPackagesTreeView.SelectedNode.Nodes.Add(packageObject);
				ProjectPackagesTreeView.SelectedNode.Expand();
				ProjectPackagesTreeView.LabelEdit = true;
				packageObject.BeginEdit();
			}
		}


		private string GetLineage(TreeNode node, bool withPackageLineage)
		{
			if (withPackageLineage)
			{
				// Try and get our lineage
				return ((Mog_BaseTag)node.Tag).PackageFullPath;
			}
			else
			{
				if (((Mog_BaseTag)node.Tag).PackageNodeType != PackageNodeTypes.Asset &&
					((Mog_BaseTag)node.Tag).PackageNodeType != PackageNodeTypes.Package)
				{
					return ((Mog_BaseTag)node.Tag).PackageFullPath;
				}					
			}

			return "";
		}


		/// <summary>
		///  Prepare to drag a package into the assigned packages window
		/// </summary>
		private void ProjectPackagesTreeView_ItemDrag(object sender, System.Windows.Forms.ItemDragEventArgs e)
		{
			DataObject send = PrepareDragObject(ProjectPackagesTreeView, ProjectPackagesTreeView.SelectedNode);

			if (send != null)
			{
				// Fire the DragDrop event
				DragDropEffects dde1=DoDragDrop(send, DragDropEffects.Copy);
			}
		}

		/// <summary>
		/// Prepare a drag object to recieve any driped items from the package tree
		/// </summary>
		private DataObject PrepareDragObject(TreeView tree, TreeNode node)
		{
			// failsafe for unpopulated package lists
			if (tree.Nodes.Count <= 0)
			{
				return null;
			}

			// Create our list holders
			ArrayList packages = new ArrayList();
			MOG_Filename package = new MOG_Filename(node.Text);
			
			// Get the package names
			if (package.GetAssetPlatform().Length == 0)
			{
				// We need to make a new MOG_Filename with a valid platform
				// To do this we need to get to the class, label and desired platform
				string classification = FindClassification(node);
				string packgePath = IsolatePackagePath(node.FullPath, classification);

				if (packgePath.Length > 0)
				{
					package = MOG_Filename.CreateAssetName(classification, "All", packgePath);

					// Add the platforms
					foreach (MOG_Platform platform in MOG_ControllerProject.GetProject().GetPlatforms())
					{
						package = MOG_Filename.CreateAssetName(classification, platform.mPlatformName, packgePath);
						packages.Add(package.GetAssetFullName());
					}
				}
				else
				{
					MOG_Prompt.PromptMessage("Create package name", "Could not locate pachage path in package(" + node.FullPath + ")");
					return null;
				}
			}
			else
			{
				packages.Add(package.GetAssetFullName());					
			}

			if (packages.Count > 0)
			{
				// Create a new Data object for the send
				return new DataObject("Package", packages);
			}

			return null;
		}		
		#endregion		

		/// <summary>
		/// We need to make sure the users does not create a new group under an object
		/// </summary>
		private void PackageTreeContextMenu_Popup(object sender, CancelEventArgs e)
		{
			ContextMenuStrip contextMenu = sender as ContextMenuStrip;

			// Get our tree, then get our selected node, so we can then get our NodeType (which is defaulted to Class)
			MogControl_PackageTreeView tree = contextMenu.SourceControl as MogControl_PackageTreeView;
			if (tree != null)
			{
				TreeNode node = tree.SelectedNode;
				// If we actually have a node selected...
				if (node != null && node.Tag != null)
				{
					PackageNodeTypes nodeType = ((Mog_BaseTag)node.Tag).PackageNodeType;

					// Enable all our MenuItems
					foreach (ToolStripItem item in contextMenu.Items)
					{
						item.Enabled = true;
					}

					// Depending on our NodeType, decide which MenuItems will be available to the user
					switch (nodeType)
					{
					// For package and group, we should not be able to create a sub-package
					case PackageNodeTypes.Asset:
					case PackageNodeTypes.Package:
						this.PackageNewClassificationMenuItem.Enabled = false;
						this.PackageNewPackageMenuItem.Enabled = false;
						this.PackageNewPackageSubMenu.Enabled = false;
						this.PackageRemoveMenuItem.Enabled = true;
						break;
					case PackageNodeTypes.Group:
						this.PackageNewClassificationMenuItem.Enabled = false;
						this.PackageNewPackageMenuItem.Enabled = false;
						this.PackageNewPackageSubMenu.Enabled = false;
						this.PackageRemoveMenuItem.Enabled = true;
						break;
					// For object, we should not be able to create a package or a sub-group
					case PackageNodeTypes.Object:
						this.PackageNewClassificationMenuItem.Enabled = false;
						this.PackageNewPackageMenuItem.Enabled = false;
						this.PackageNewPackageSubMenu.Enabled = false;
						this.PackageNewGroupMenuItem.Enabled = false;
						this.PackageRemoveMenuItem.Enabled = true;
						break;
					// For class, we should only be able to add a package or remove a package
					default: // PackageNodeTypes.Class:
						this.PackageNewClassificationMenuItem.Enabled = true;
						this.PackageNewPackageMenuItem.Enabled = true;
						this.PackageNewPackageSubMenu.Enabled = true;
						this.PackageNewGroupMenuItem.Enabled = false;
						this.PackageNewObjectMenuItem.Enabled = false;
						this.PackageRemoveMenuItem.Enabled = false;
						break;
					}

					// Disable menu items we don't have privileges for
					MOG_Privileges privileges = MOG_ControllerProject.GetPrivileges();
					bool bCanAddClassification = privileges.GetUserPrivilege(MOG_ControllerProject.GetUserName(), MOG_PRIVILEGE.AddClassification);
					if (!bCanAddClassification)
					{
						PackageNewClassificationMenuItem.Enabled = false;
					}
					bool bCanCreatePackage = privileges.GetUserPrivilege(MOG_ControllerProject.GetUserName(), MOG_PRIVILEGE.CreatePackage);
					if (!bCanCreatePackage)
					{
						PackageNewPackageMenuItem.Enabled = false;
						PackageNewPackageSubMenu.Enabled = false;
						PackageRemoveMenuItem.Enabled = false;
					}
					bool bCanCreatePackageGroup = privileges.GetUserPrivilege(MOG_ControllerProject.GetUserName(), MOG_PRIVILEGE.AddPackageGroup);
					if (!bCanCreatePackageGroup)
					{
						PackageNewObjectMenuItem.Enabled = false;
						PackageNewGroupMenuItem.Enabled = false;
						PackageRemoveMenuItem.Enabled = false;
					}

					// Substitute the create package submenu in the place of the create package option depending on the ShowPlatformSpecific option
					PackageNewPackageMenuItem.Visible = !tree.ShowPlatformSpecific;
					PackageNewPackageSubMenu.Visible = tree.ShowPlatformSpecific;
				}
				else
				{
					foreach (ToolStripItem item in contextMenu.Items)
					{
						item.Enabled = false;
					}
				}
				// Now the ContextMenu will display properly
			}
		}


		/// <summary>
		/// Find out if we have a classification that has the IsPackage property set true
		/// </summary>
		private bool NodeClassificationIsPackage( TreeNode node )
		{
			MOG_Properties classificationProps = MOG_Properties.OpenClassificationProperties( node.FullPath );
			if( classificationProps != null && classificationProps.IsPackage )
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Pass up the AfterSelect event to our userControl's AfterPackageSelect event
		/// </summary>
		private void ProjectPackagesTreeView_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			if (AfterPackageSelect != null)
			{
				object []args = {sender, e};
				this.Invoke(AfterPackageSelect, args);
			}
		}

		#region New Item Events
		/// <summary>
		/// Through PackageManagementRepositoryTreeView, generate a new package for each platform
		/// </summary>
		private void newPackageMenuItem_Click(object sender, EventArgs e)
		{
			if (ProjectPackagesTreeView.SelectedNode != null && ((Mog_BaseTag)ProjectPackagesTreeView.SelectedNode.Tag).PackageNodeType == PackageNodeTypes.Class )
			{
				string menuItemText = ((ToolStripMenuItem)sender).Text;
				// Create a new editable group node
				TreeNode package = new TreeNode(menuItemText + "NewPackage");

				// Expand our node before we try and let anyone edit it (this should get rid of any <BLANK> nodes we have)
				ProjectPackagesTreeView.SelectedNode.Expand();
				ProjectPackagesTreeView.SelectedNode.Nodes.Add(package);
				ProjectPackagesTreeView.SelectedNode.Expand();
				ProjectPackagesTreeView.LabelEdit = true;
				package.Tag = package.Text;
				package.BeginEdit();
			}
		}

		private void PackageRemoveMenuItem_Click(object sender, System.EventArgs e)
		{
			try
			{
				TreeNode selectedNode = ProjectPackagesTreeView.SelectedNode;
				// If we have not expanded this node, go ahead and do so...
				if( selectedNode != null && !selectedNode.IsExpanded )
				{
					// Get rid of any Blank node
					selectedNode.Expand();
				}

				// Make sure we have a selected node and that that node does not have sub nodes other than the blank node
				if (selectedNode != null &&	selectedNode.Nodes.Count == 0 ) 
				{
					Mog_BaseTag packageTag = (Mog_BaseTag)ProjectPackagesTreeView.SelectedNode.Tag;
					// Are we a package?
					if( packageTag.PackageNodeType == PackageNodeTypes.Asset ||
						packageTag.PackageNodeType == PackageNodeTypes.Package )
					{
						RemovePackageFromProject( packageTag );
						return;
					}
						// Are we a classification?
					else if( packageTag.PackageNodeType == PackageNodeTypes.Class )
					{
						MessageBox.Show(this, "Cannot remove a Classification node.  Please go to Project Tab | Project Trees "
							+ "to be able to do this.");
						return;
					}

					string removeCandidate = ProjectPackagesTreeView.SelectedNode.Text;

					// Find our parent package
					TreeNode package = ProjectPackagesTreeView.FindPackage(ProjectPackagesTreeView.SelectedNode);

					if (package != null)
					{
						MOG_Filename packageAsset = new MOG_Filename( ((Mog_BaseTag)package.Tag).FullFilename );

						// Remove the classification from our fullPath
						string objectPath = ProjectPackagesTreeView.SelectedNode.FullPath.Replace(packageAsset.GetAssetClassification() + "/", "");
						// First, get the index of our package name
						int assetNameIndex = objectPath.IndexOf( packageAsset.GetAssetName() );
						// If we have a valid index for the package's name...
						if( assetNameIndex > -1 )
						{
							// Remove everything before our package name
							objectPath = objectPath.Substring( assetNameIndex );
						}
						// Now remove our package name
						objectPath = objectPath.Replace( packageAsset.GetAssetName(), "" );
						// Add back in our Group/Object separator
						objectPath = objectPath.Replace( "~", "/" );

						// If we have an initial forward slash...
						if( objectPath.IndexOf( "/" ) == 0 )
						{
							// Get rid of it
							objectPath = objectPath.Substring(1);
						}

						// If we can remove it from the databse, remove the treenode?
						if (ProjectPackagesTreeView.RemoveGroupFromDatabase(objectPath, packageAsset))
						{
							// Remove the node
							ProjectPackagesTreeView.SelectedNode.Remove();
						}
					}
				}
				else
				{
					MOG_Prompt.PromptMessage("Remove node", "Could not remove this node because:\n\tNode must not contain any sub-nodes before removal.");
				}
			}
			catch(Exception ex)
			{
				MOG_Report.ReportMessage("Remove node", "MOG encountered an unexpected problem.  Aborting node remove.\nSystem Message:" + ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
			}	
		}
		#endregion

		/// <summary>
		/// Remove a Package from the PackageManagement Tree. 
		///  Adapted from MogControl_AssetContextMenu.cs::MenuItemRemoveFromProject_Click()
		/// </summary>
		private void RemovePackageFromProject( Mog_BaseTag packageTag )
		{
			try
			{
				string message = "Are you sure you want to remove this package from the game?\r\n" + packageTag.PackageFullName;

				// If user OKs our removal...
				if (MOG_Prompt.PromptResponse("Remove Asset From Project", message, MOGPromptButtons.OKCancel) == MOGPromptResult.OK)
				{
					if (packageTag.Execute)
					{
						MOG_Filename filename = new MOG_Filename(packageTag.FullFilename);
				
						// Make sure we are an asset before showing log
						if (filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
						{
							// Proceed to remove this package from the project; skiping the unpackage merge event
                            if (MOG_ControllerProject.RemoveAssetFromProject(filename, "No longer needed", false))
                            {
                                // Go ahead and actually remove the node
                                packageTag.ItemRemove();
                            }
                            else
                            {
								MOG_Prompt.PromptMessage("Remove Package From Project Failed",
															"The package could not be removed from the project.\n" +
															"PACKAGE: " + filename.GetAssetFullName() + "\n\n" +
															"We are now aborting the remove process.\n", Environment.StackTrace);
								return;
							}						
						}
					}
				}
			}
			catch(Exception ex)
			{
				MOG_Report.ReportMessage("Remove From Project", ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
			}
		}

		private void PackagePlatformsCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			ProjectPackagesTreeView.DeInitialize();
			ProjectPackagesTreeView.ShowPlatformSpecific = PackagePlatformsCheckBox.Checked;
			ProjectPackagesTreeView.Initialize();

			MogUtils_Settings.MogUtils_Settings.SaveSetting("PackageManagementForm", "ShowPlatformSpecific", PackagePlatformsCheckBox.Checked.ToString());
		}

		private void MogControl_PackageManagementTreeView_Load(object sender, EventArgs e)
		{
			PackagePlatformsCheckBox.Checked = MogUtils_Settings.MogUtils_Settings.LoadBoolSetting("PackageManagementForm", "ShowPlatformSpecific", true);
		}
	}
}
