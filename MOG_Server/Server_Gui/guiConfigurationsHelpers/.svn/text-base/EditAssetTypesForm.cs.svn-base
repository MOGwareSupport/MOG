/*
#region System definitions
#endregion
#region User definitions
#endregion
#region Constructor
#endregion
#region Member functions
#endregion
#region Events
#endregion
*/

using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using MOG;
using MOG.INI;
using MOG.PROJECT;

namespace MOG_Server.Server_Gui.guiConfigurationsHelpers
{
	/// <summary>
	/// Summary description for EditAssetTypesForm.
	/// </summary>
	public class EditAssetTypesForm : System.Windows.Forms.Form
	{
		#region User definitions
		private BASE mog;
		private assetType assetTree;
		private ArrayList initialAssets;
		private bool changed;
		private AssetTreeNode currentNode;
		private assetType currentAsset;
		#endregion

		#region System definitions

		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Button btnRemove;
		private System.Windows.Forms.TreeView AssetTypesTreeView;
		private System.Windows.Forms.ContextMenu ListViewContextMenu;
		private System.Windows.Forms.MenuItem AddMenuItem;
		private System.Windows.Forms.MenuItem RemoveMenuItem;
		private System.Windows.Forms.ComboBox KeyComboBox;
		private System.Windows.Forms.ComboBox PackageComboBox;
		private System.Windows.Forms.ComboBox GroupComboBox;
		private System.Windows.Forms.Label ProjectLabel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label FullAssetNameLabel;
		private System.Windows.Forms.ToolTip EditAssetTypesFormToolTips;
		private System.Windows.Forms.Button btnApply;
		private System.Windows.Forms.MenuItem EditMenuItem;
		private System.Windows.Forms.Button btnEdit;
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnAdd = new System.Windows.Forms.Button();
			this.btnRemove = new System.Windows.Forms.Button();
			this.AssetTypesTreeView = new System.Windows.Forms.TreeView();
			this.ListViewContextMenu = new System.Windows.Forms.ContextMenu();
			this.AddMenuItem = new System.Windows.Forms.MenuItem();
			this.EditMenuItem = new System.Windows.Forms.MenuItem();
			this.RemoveMenuItem = new System.Windows.Forms.MenuItem();
			this.KeyComboBox = new System.Windows.Forms.ComboBox();
			this.PackageComboBox = new System.Windows.Forms.ComboBox();
			this.GroupComboBox = new System.Windows.Forms.ComboBox();
			this.ProjectLabel = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.FullAssetNameLabel = new System.Windows.Forms.Label();
			this.EditAssetTypesFormToolTips = new System.Windows.Forms.ToolTip(this.components);
			this.btnEdit = new System.Windows.Forms.Button();
			this.btnApply = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.Location = new System.Drawing.Point(376, 512);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "OK";
			this.EditAssetTypesFormToolTips.SetToolTip(this.btnOK, "Accept the changes made to the asset tree and close this window");
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(552, 512);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "Cancel";
			this.EditAssetTypesFormToolTips.SetToolTip(this.btnCancel, "Close this window and cancel any changes made to the asset type tree");
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnAdd
			// 
			this.btnAdd.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnAdd.Location = new System.Drawing.Point(88, 512);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.TabIndex = 4;
			this.btnAdd.Text = "&Add";
			this.EditAssetTypesFormToolTips.SetToolTip(this.btnAdd, "Add a package or group");
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// btnRemove
			// 
			this.btnRemove.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnRemove.Location = new System.Drawing.Point(264, 512);
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.TabIndex = 5;
			this.btnRemove.Text = "&Remove";
			this.EditAssetTypesFormToolTips.SetToolTip(this.btnRemove, "Remove the currently selected package or group");
			this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
			// 
			// AssetTypesTreeView
			// 
			this.AssetTypesTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.AssetTypesTreeView.ContextMenu = this.ListViewContextMenu;
			this.AssetTypesTreeView.HideSelection = false;
			this.AssetTypesTreeView.ImageIndex = -1;
			this.AssetTypesTreeView.Location = new System.Drawing.Point(16, 40);
			this.AssetTypesTreeView.Name = "AssetTypesTreeView";
			this.AssetTypesTreeView.SelectedImageIndex = -1;
			this.AssetTypesTreeView.Size = new System.Drawing.Size(616, 416);
			this.AssetTypesTreeView.TabIndex = 6;
			this.EditAssetTypesFormToolTips.SetToolTip(this.AssetTypesTreeView, "Graphical representation of the asset type tree");
			this.AssetTypesTreeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.AssetTypesTreeView_MouseDown);
			this.AssetTypesTreeView.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.AssetTypesTreeView_AfterExpand);
			this.AssetTypesTreeView.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.AssetTypesTreeView_AfterCollapse);
			this.AssetTypesTreeView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.AssetTypesTreeView_KeyUp);
			this.AssetTypesTreeView.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.AssetTypesTreeView_AfterLabelEdit);
			// 
			// ListViewContextMenu
			// 
			this.ListViewContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																								this.AddMenuItem,
																								this.EditMenuItem,
																								this.RemoveMenuItem});
			// 
			// AddMenuItem
			// 
			this.AddMenuItem.Index = 0;
			this.AddMenuItem.Text = "Insert";
			this.AddMenuItem.Click += new System.EventHandler(this.AddMenuItem_Click);
			// 
			// EditMenuItem
			// 
			this.EditMenuItem.Index = 1;
			this.EditMenuItem.Text = "Edit";
			this.EditMenuItem.Click += new System.EventHandler(this.EditMenuItem_Click);
			// 
			// RemoveMenuItem
			// 
			this.RemoveMenuItem.Index = 2;
			this.RemoveMenuItem.Text = "Remove";
			this.RemoveMenuItem.Click += new System.EventHandler(this.RemoveMenuItem_Click);
			// 
			// KeyComboBox
			// 
			this.KeyComboBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.KeyComboBox.Location = new System.Drawing.Point(168, 472);
			this.KeyComboBox.Name = "KeyComboBox";
			this.KeyComboBox.Size = new System.Drawing.Size(120, 21);
			this.KeyComboBox.TabIndex = 8;
			this.KeyComboBox.Text = "[Key]";
			this.EditAssetTypesFormToolTips.SetToolTip(this.KeyComboBox, "Select from all the keys in the current project");
			this.KeyComboBox.SelectionChangeCommitted += new System.EventHandler(this.KeyComboBox_SelectionChangeCommitted);
			// 
			// PackageComboBox
			// 
			this.PackageComboBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.PackageComboBox.Location = new System.Drawing.Point(304, 472);
			this.PackageComboBox.Name = "PackageComboBox";
			this.PackageComboBox.Size = new System.Drawing.Size(120, 21);
			this.PackageComboBox.TabIndex = 9;
			this.PackageComboBox.Text = "[Package]";
			this.EditAssetTypesFormToolTips.SetToolTip(this.PackageComboBox, "Select from all the packages in the current key");
			this.PackageComboBox.SelectionChangeCommitted += new System.EventHandler(this.PackageComboBox_SelectionChangeCommitted);
			// 
			// GroupComboBox
			// 
			this.GroupComboBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.GroupComboBox.Location = new System.Drawing.Point(440, 472);
			this.GroupComboBox.Name = "GroupComboBox";
			this.GroupComboBox.Size = new System.Drawing.Size(128, 21);
			this.GroupComboBox.TabIndex = 10;
			this.GroupComboBox.Text = "[Group]";
			this.EditAssetTypesFormToolTips.SetToolTip(this.GroupComboBox, "Select from all the groups in the current package");
			this.GroupComboBox.SelectionChangeCommitted += new System.EventHandler(this.GroupComboBox_SelectionChangeCommitted);
			// 
			// ProjectLabel
			// 
			this.ProjectLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.ProjectLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.ProjectLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.ProjectLabel.Location = new System.Drawing.Point(88, 472);
			this.ProjectLabel.Name = "ProjectLabel";
			this.ProjectLabel.Size = new System.Drawing.Size(64, 24);
			this.ProjectLabel.TabIndex = 14;
			this.ProjectLabel.Text = "Project";
			this.ProjectLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.EditAssetTypesFormToolTips.SetToolTip(this.ProjectLabel, "The three-letter abreviation of the current project");
			// 
			// label1
			// 
			this.label1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.Location = new System.Drawing.Point(288, 464);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(16, 32);
			this.label1.TabIndex = 15;
			this.label1.Text = ".";
			// 
			// label2
			// 
			this.label2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label2.Location = new System.Drawing.Point(152, 464);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(16, 32);
			this.label2.TabIndex = 16;
			this.label2.Text = ".";
			// 
			// label3
			// 
			this.label3.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label3.Location = new System.Drawing.Point(424, 464);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(16, 32);
			this.label3.TabIndex = 17;
			this.label3.Text = ".";
			// 
			// FullAssetNameLabel
			// 
			this.FullAssetNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FullAssetNameLabel.Location = new System.Drawing.Point(16, 24);
			this.FullAssetNameLabel.Name = "FullAssetNameLabel";
			this.FullAssetNameLabel.Size = new System.Drawing.Size(592, 16);
			this.FullAssetNameLabel.TabIndex = 18;
			this.FullAssetNameLabel.Text = "Asset name";
			this.EditAssetTypesFormToolTips.SetToolTip(this.FullAssetNameLabel, "The fully qualified name of this asset type");
			// 
			// btnEdit
			// 
			this.btnEdit.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnEdit.Location = new System.Drawing.Point(176, 512);
			this.btnEdit.Name = "btnEdit";
			this.btnEdit.TabIndex = 20;
			this.btnEdit.Text = "&Edit";
			this.EditAssetTypesFormToolTips.SetToolTip(this.btnEdit, "Edit the currently selected package or group");
			this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
			// 
			// btnApply
			// 
			this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnApply.Enabled = false;
			this.btnApply.Location = new System.Drawing.Point(464, 512);
			this.btnApply.Name = "btnApply";
			this.btnApply.TabIndex = 19;
			this.btnApply.Text = "Apply";
			this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
			// 
			// EditAssetTypesForm
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(648, 558);
			this.Controls.Add(this.btnEdit);
			this.Controls.Add(this.btnApply);
			this.Controls.Add(this.FullAssetNameLabel);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.ProjectLabel);
			this.Controls.Add(this.GroupComboBox);
			this.Controls.Add(this.PackageComboBox);
			this.Controls.Add(this.KeyComboBox);
			this.Controls.Add(this.AssetTypesTreeView);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.btnRemove);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(656, 256);
			this.Name = "EditAssetTypesForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Edit Asset Types";
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.EditAssetTypesForm_KeyUp);
			this.ResumeLayout(false);

		}
		#endregion

		#endregion

		#region Constructors
		public EditAssetTypesForm(BASE mog)
		{
			init(mog, null);
		}
		public EditAssetTypesForm(BASE mog, ArrayList assets)
		{
			init(mog, assets);
		}

		private void init(BASE mog, ArrayList assets)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.mog = mog;
			changed = false;
			currentNode = null;
			currentAsset = null;

			if (assets == null) 
			{
				// if 'assets' is null, then load all the assets from the INI file
				this.assetTree = loadAssetTreeFromINI( this.mog.GetProject().GetConfigFile() );
				this.initialAssets = this.mog.GetProject().GetAssetTypes();
			}
			else 
			{
				// if 'assets' isn't null, load only the assets it specifies
				this.initialAssets = assets;
				this.assetTree = loadAssetTreeFromINI( this.mog.GetProject().GetConfigFile(), assets );
			}
			
			PopulateTreeView(assetTree);
			PopulateComboBoxes(assetTree);
			
			if (AssetTypesTreeView.Nodes.Count > 0) 
			{
				SelectNode( this.AssetTypesTreeView.Nodes[0] );
			}
		}
		#endregion

		#region Member functions

		private string contructFullAssetName(string projectName, string keyName, string packageName, string groupName)
		{
			string fullAssetName = projectName;
			if (keyName.StartsWith("[")) 
			{
				return fullAssetName;
			}
			fullAssetName += "." + keyName;

			if (packageName.StartsWith("[")) 
			{
				return fullAssetName;
			}
			fullAssetName += "." + packageName;

			if (groupName.StartsWith("[")) 
			{
				return fullAssetName;
			}
			fullAssetName += "." + groupName;

			return fullAssetName;
		}

		private assetType loadAssetTreeFromINI( MOG_Ini ini ) 
		{
			// load asset types
			assetType tree = new assetType("ROOT_ASSET_TREE_NODE");
			int numAssetKeys = ini.CountKeys("assets");
			for (int i=0; i<numAssetKeys; i++) 
			{
				tree.SubTypes.Add( new assetType(ini.GetKeyNameByIndex("assets", i)) );
			}
			string typesIniFilename = string.Concat(this.mog.GetProject().GetProjectToolsPath(), "\\", ini.GetString("project", "TypesConfigFile"));
			ini = new MOG_Ini(typesIniFilename);

			// populate all the subnodes of assets
			PopulateAssetTypes(tree, ini);
			
			return tree;
		}
		
		private assetType loadAssetTreeFromINI( MOG_Ini ini, ArrayList assets ) 
		{
			// load asset types
			assetType tree = new assetType("ROOT_ASSET_TREE_NODE");
			int numAssetKeys = ini.CountKeys("assets");
			foreach (string curAssetName in assets) 
			{
				if (ini.KeyExist("assets", curAssetName)) 
				{
					tree.SubTypes.Add( new assetType(curAssetName) );
				}
			}
			string typesIniFilename = string.Concat(this.mog.GetProject().GetProjectPath(), "\\Tools\\", ini.GetString("project", "TypesConfigFile"));
			ini = new MOG_Ini(typesIniFilename);

			// populate all the subnodes of assets
			PopulateAssetTypes(tree, ini);
			
			return tree;
		}

		private assetType getAsset( assetType tree, string name ) 
		{
			if (tree == null) 
			{
				return null;
			}
			
			foreach (assetType asset in tree.SubTypes) 
			{
				if (asset.Name.ToUpper() == name.ToUpper()) 
				{
					return asset;
				}
			}

			return null;
		}

		private AssetTreeNode getAssetNode( TreeNodeCollection nodes, string name ) 
		{
			if (nodes == null) 
			{
				return null;
			}
			
			foreach (AssetTreeNode node in nodes) 
			{
				if (node.Text.ToUpper() == name.ToUpper()) 
				{
					return node;
				}
			}

			return null;
		}

		private void PopulateComboBoxesFromTreeNode( AssetTreeNode node ) 
		{
			assetType key = null;
			assetType package = null;
			assetType group = null;

			if (node == null) 
			{
				return;
			}

			switch (node.Asset.Level) 
			{
				case 0:
					// node is a key
					key = node.Asset;
					
					// set up key box
					PopulateComboBox(this.KeyComboBox, this.assetTree.SubTypes);
					KeyComboBox.SelectedItem = key.Name;
					// set up package box
					PopulateComboBox(this.PackageComboBox, key.SubTypes);
					this.PackageComboBox.Text = "[Select package]";
					// set up group box
					this.GroupComboBox.Items.Clear();
					this.GroupComboBox.Text = "[Select group]";

					break;
				case 1:
					// node is a package
					package = node.Asset;
					key = ((AssetTreeNode)node.Parent).Asset;
					
					// set up key box
					PopulateComboBox(this.KeyComboBox, this.assetTree.SubTypes);
					this.KeyComboBox.SelectedItem = key.Name;
					// set up package box
					PopulateComboBox(this.PackageComboBox, key.SubTypes);
					this.PackageComboBox.SelectedItem = package.Name;
					// set up group box
					PopulateComboBox(this.GroupComboBox, package.SubTypes);
					this.GroupComboBox.Text = "[Select group]";
				
					break;
				case 2:
					// node is a group
					group = node.Asset;
					package = ((AssetTreeNode)node.Parent).Asset;
					key = ((AssetTreeNode)node.Parent.Parent).Asset;

					// set up key box
					PopulateComboBox(this.KeyComboBox, this.assetTree.SubTypes);
					this.KeyComboBox.SelectedItem = key.Name;
					// set up package box
					PopulateComboBox(this.PackageComboBox, key.SubTypes);
					this.PackageComboBox.SelectedItem = package.Name;
					// set up group box
					PopulateComboBox(this.GroupComboBox, package.SubTypes);
					this.GroupComboBox.SelectedItem = group.Name;

					break;
				default:
					this.KeyComboBox.Items.Clear();
					this.PackageComboBox.Items.Clear();
					this.GroupComboBox.Items.Clear();
					break;
			}
		}
		
		private void PopulateComboBox( ComboBox cBox, ArrayList assets ) 
		{
			if (cBox == null || assets == null || assets.Count <= 0) 
			{
				return;
			}

			cBox.Items.Clear();

			foreach (assetType asset in assets) 
			{
				cBox.Items.Add( asset.Name );
			}
		}

		private void PopulateComboBoxes( assetType tree ) 
		{
			this.ProjectLabel.Text = this.mog.GetProject().GetProjectKey();
			
			PopulateComboBox( this.KeyComboBox, tree.SubTypes );
			this.KeyComboBox.Text = "[Select key]";

			this.PackageComboBox.Items.Clear();
			this.PackageComboBox.Text = "[Select package]";

			this.GroupComboBox.Items.Clear();
			this.GroupComboBox.Text = "[Select group]";
			return;
		}

		private void WriteAssetTreeToINI( assetType tree, MOG_Ini ini ) 
		{
			foreach (assetType asset in tree.SubTypes) 
			{
				foreach (assetType subAsset in asset.SubTypes) 
				{
					ini.PutSectionString( string.Concat(asset.QualifiedName,".Types"), subAsset.Name );
					WriteAssetTreeToINI(asset, ini);
				}
			}
		}

		private void PopulateAssetTypes(assetType asset, MOG_Ini ini) 
		{
			// search the ini for subtree info
			string assetSectionName = string.Concat( asset.QualifiedName, ".Types" );
			if (ini.SectionExist( assetSectionName ) || asset.Name == "ROOT_ASSET_TREE_NODE") 
			{
				// get all children out of it
				int numSubTypes = ini.CountKeys( assetSectionName );

				for (int i=0; i<numSubTypes; i++) 
				{
					asset.SubTypes.Add( new assetType( ini.GetKeyNameByIndex(assetSectionName,i), asset ) );
				}

				// recurse on kids
				foreach (assetType at in asset.SubTypes) 
				{
					PopulateAssetTypes(at, ini);
				}
			}
		}
		
		private void PopulateTreeView(assetType tree) 
		{
			// construct the TreeView
			EncodeExpandState( AssetTypesTreeView.Nodes );
			AssetTypesTreeView.BeginUpdate();
			AssetTypesTreeView.Nodes.Clear();
			foreach (assetType asset in tree.SubTypes) 
			{
				PopulateTreeView(asset, AssetTypesTreeView.Nodes);
			}
			AssetTypesTreeView.EndUpdate();
		}
		private void PopulateTreeView(assetType asset, TreeNodeCollection nodes) 
		{
//			if (asset.Name == "ROOT_ASSET_TREE_NODE") 
//			{
//				foreach (assetType at in asset.SubTypes) 
//				{
//					PopulateTreeView(at, nodes);
//				}
//			}
//			else 
//			{
				AssetTreeNode tn = new AssetTreeNode(asset.Name, asset);
				nodes.Add( tn );
				
				foreach (assetType at in asset.SubTypes) 
				{
					PopulateTreeView(at, tn.Nodes);
				}

				if (asset.Expanded) 
				{
					tn.ExpandMe();
				}
//			}
			
		}

		private void EncodeExpandState(TreeNodeCollection nodes) 
		{
			foreach (AssetTreeNode atn in nodes) 
			{
				if (atn.Asset != null) 
				{
					atn.Asset.Expanded = atn.ShouldBeExpanded;
					EncodeExpandState(atn.Nodes);
				}
			}
		}
		
		private bool isValidTree( assetType tree ) 
		{
			if (tree == null) 
			{
				return false;
			}

			ArrayList emptyAssets = new ArrayList();
			
			// make sure each key has at least one package
			foreach (assetType keyAsset in tree.SubTypes) 
			{
				if (keyAsset.SubTypes.Count <= 0) 
				{
					emptyAssets.Add( keyAsset );
				}
				else 
				{
					// make sure each package has at least one group
					foreach (assetType packageAsset in keyAsset.SubTypes) 
					{
						if (packageAsset.SubTypes.Count <= 0) 
						{
							emptyAssets.Add( packageAsset );
						}
					}
				}
			}
			
			string keyMsg = "";
			string packageMsg = "";
			if (emptyAssets.Count > 0) 
			{
				foreach (assetType asset in emptyAssets) 
				{
					if (asset.Level == 0) 
					{
						keyMsg = string.Concat(keyMsg, "\t", asset.Name, "\n");
					}
					else if (asset.Level == 1) 
					{
						packageMsg = string.Concat(packageMsg, "\t", asset.QualifiedName, "\n");
					}
				}

				if (keyMsg != "") 
				{
					keyMsg = string.Concat("The following keys have no corresponding package:\n", keyMsg);
				}
				if (packageMsg != "") 
				{
					packageMsg = string.Concat("\nThe following packages have no corresponding group:\n", packageMsg);
				}

				packageMsg = string.Concat(packageMsg, "\nAll keys must have at least one package and all packages must have at least one key");
				MessageBox.Show(keyMsg + packageMsg, "Missing keys/packages");
			}
			
			return (emptyAssets.Count == 0);
		}

		// returns true if changes were applied, false otherwise
		private bool ApplyChanges(bool confirm) 
		{
			if (!isValidTree( this.assetTree ))
			{
				return false;		// return false, tree is invalid
			}

			if (this.changed) 
			{
				if ( confirm && MessageBox.Show("Are you sure you want to permanently apply these asset changes?", "Confirm changes", MessageBoxButtons.YesNo) == DialogResult.No ) 
				{
					return false;	// return false, we DID NOT apply the changes
				}

				//
				// rebuild the .types file
				//

				MOG_Ini ini = this.mog.GetProject().GetConfigFile();

				assetType fullAssetTree = loadAssetTreeFromINI( ini );

				// build the full .types filename
				string typeFilename = string.Concat(mog.GetProject().GetProjectToolsPath(), "\\", ini.GetString("project", "TypesConfigFile"));
				
				// delete it and recreate it from scratch
				if (File.Exists(typeFilename)) 
				{
					File.Delete(typeFilename);
				}
				ini = new MOG_Ini(typeFilename);

				// merge the asset types we've been working with with the rest of the asset types
				foreach (assetType asset in this.assetTree.SubTypes) 
				{
					if (fullAssetTree.ContainsSubType( asset.OriginalName )) 
					{
						fullAssetTree.ReplaceSubType( asset );
					}
				}

				// create the file
				this.WriteAssetTreeToINI( fullAssetTree, ini );
				ini.Save();
				ini.Close();

				this.changed = false;
				this.btnApply.Enabled = false;
				
				return true;		// return true, we DID apply the changes
			}

			return false;			// return false, there were no changes to apply
		}

		private void SelectNode( TreeNode node ) 
		{
			AssetTypesTreeView.SelectedNode = node;
			AssetTreeNode atn = (AssetTreeNode)node;
			this.currentNode = atn;
			if (atn != null) 
			{
				this.currentAsset = atn.Asset;
			}
			else 
			{
				this.currentAsset = null;
			}

			if (atn != null) 
			{
				this.FullAssetNameLabel.Text = string.Concat(this.ProjectLabel.Text, ".", atn.Asset.QualifiedName);
				PopulateComboBoxesFromTreeNode( atn );

				switch (atn.Asset.Level) 
				{
					case 0:
						// key
						this.btnAdd.Enabled = true;
						this.btnEdit.Enabled = false;
						this.btnRemove.Enabled = false;

						this.AddMenuItem.Enabled = true;
						this.AddMenuItem.Text = string.Concat("Add new package to '", atn.Asset.Name, "'");
						this.EditMenuItem.Enabled = false;
						this.EditMenuItem.Text = "Edit";
						this.RemoveMenuItem.Enabled = false;
						this.RemoveMenuItem.Text = "Remove";
						break;
					case 1:
						// package
						this.btnAdd.Enabled = true;
						this.btnEdit.Enabled = true;
						this.btnRemove.Enabled = true;

						this.AddMenuItem.Enabled = true;
						this.AddMenuItem.Text = string.Concat("Add new group to '", atn.Asset.Name, "'");
						this.EditMenuItem.Enabled = true;
						this.EditMenuItem.Text = "Edit";
						this.RemoveMenuItem.Enabled = true;
						this.RemoveMenuItem.Text = string.Concat("Remove");
						break;
					case 2:
						// group
						this.btnAdd.Enabled = false;
						this.btnEdit.Enabled = true;
						this.btnRemove.Enabled = true;

						this.AddMenuItem.Enabled = false;
						this.AddMenuItem.Text = "Insert";
						this.EditMenuItem.Enabled = true;
						this.EditMenuItem.Text = "Edit";
						this.RemoveMenuItem.Enabled = true;
						this.RemoveMenuItem.Text = string.Concat("Remove");
						break;
					default:
						this.btnAdd.Enabled = false;
						this.btnEdit.Enabled = false;
						this.btnRemove.Enabled = false;

						this.AddMenuItem.Enabled = false;
						this.AddMenuItem.Text = "Insert";
						this.EditMenuItem.Enabled = false;
						this.EditMenuItem.Text = "Edit";
						this.RemoveMenuItem.Enabled = false;
						this.RemoveMenuItem.Text = "Remove";
						break;
				}
			}
			else 
			{
				this.AddMenuItem.Text = "Insert";
				this.EditMenuItem.Text = "Edit";
				this.RemoveMenuItem.Text = "Remove";

				this.AddMenuItem.Enabled = false;
				this.EditMenuItem.Enabled = false;
				this.RemoveMenuItem.Enabled = false;

				this.btnAdd.Enabled = false;
				this.btnEdit.Enabled = false;
				this.btnRemove.Enabled = false;
			}
		}
		#endregion

		#region Events


		private void KeyComboBox_SelectionChangeCommitted(object sender, System.EventArgs e)
		{
			assetType keyAsset = this.getAsset( this.assetTree, (string)KeyComboBox.SelectedItem );
			if (keyAsset == null) 
			{
				this.PackageComboBox.Items.Clear();
				this.GroupComboBox.Items.Clear();
				OutputBox.writeln("\t--> KeyComboBox_SelectionChangeCommitted(): couldn't lookup selected key");
				return;
			}

			// repopulate the other combo boxes appropriately
			PopulateComboBox(this.PackageComboBox, keyAsset.SubTypes);
			this.PackageComboBox.Text = "[Select package]";

			this.GroupComboBox.Items.Clear();
			this.GroupComboBox.Text = "[Select group]";

			// update the full asset name label
			this.FullAssetNameLabel.Text = string.Concat(this.ProjectLabel.Text, ".", keyAsset.QualifiedName);

			// update add/remove buttons
			this.btnAdd.Enabled = true;
			this.btnEdit.Enabled = false;
			this.btnRemove.Enabled = false;

			// expand the tree view above
			AssetTreeNode keyNode = getAssetNode(AssetTypesTreeView.Nodes, keyAsset.Name);
			if (keyNode == null) 
			{
				OutputBox.writeln("\t--> KeyComboBox_SelectionChangeCommitted(): couldn't lookup selected key's node");
				return;
			}
			keyNode.ExpandMe();

			AssetTypesTreeView.SelectedNode = keyNode;

			OutputBox.writeln( this.ProjectLabel.Text + "." + (string)this.KeyComboBox.SelectedItem + " selected from combo boxes" );
		}



		private void PackageComboBox_SelectionChangeCommitted(object sender, System.EventArgs e)
		{
			assetType keyAsset = this.getAsset( this.assetTree, (string)KeyComboBox.SelectedItem );
			assetType packageAsset = this.getAsset( keyAsset, (string)PackageComboBox.SelectedItem );
			if (keyAsset == null || packageAsset == null) 
			{
				this.GroupComboBox.Items.Clear();
				OutputBox.writeln("\t--> PackageComboBox_SelectionChangeCommitted(): couldn't lookup selected key/package");
				return;
			}
			
			// repopulate the other combo boxes appropriately
			PopulateComboBox(this.GroupComboBox, packageAsset.SubTypes);
			this.GroupComboBox.Text = "[Select group]";

			// update the full asset name label
			this.FullAssetNameLabel.Text = string.Concat(this.ProjectLabel.Text, ".", packageAsset.QualifiedName);

			// update add/remove buttons
			this.btnAdd.Enabled = true;
			this.btnEdit.Enabled = true;
			this.btnRemove.Enabled = true;

			//
			// expand the tree view above
			AssetTreeNode keyNode = getAssetNode(AssetTypesTreeView.Nodes, keyAsset.Name);
			AssetTreeNode packageNode = getAssetNode(keyNode.Nodes, packageAsset.Name);

			if (keyNode == null || packageNode == null) 
			{
				OutputBox.writeln("\t--> PackageComboBox_SelectionChangeCommitted(): couldn't lookup node for selected key/package");
				return;
			}

			keyNode.ExpandMe();
			packageNode.ExpandMe();

			AssetTypesTreeView.SelectedNode = packageNode;		

			OutputBox.writeln( this.ProjectLabel.Text + "." + (string)this.KeyComboBox.SelectedItem + "." + (string)this.PackageComboBox.SelectedItem + " selected from combo boxes" );
		}


		private void GroupComboBox_SelectionChangeCommitted(object sender, System.EventArgs e)
		{
			assetType keyAsset = this.getAsset( this.assetTree, (string)KeyComboBox.SelectedItem );
			assetType packageAsset = this.getAsset( keyAsset, (string)PackageComboBox.SelectedItem );
			assetType groupAsset = this.getAsset( packageAsset, (string)GroupComboBox.SelectedItem );
			if (keyAsset == null || packageAsset == null || groupAsset == null) 
			{
				OutputBox.writeln("\t--> GroupComboBox_SelectionChangeCommitted(): couldn't lookup selected key/package/group");
				return;
			}

			// update the full asset name label
			this.FullAssetNameLabel.Text = string.Concat(this.ProjectLabel.Text, ".", groupAsset.QualifiedName);

			// update add/remove buttons
			this.btnAdd.Enabled = false;
			this.btnEdit.Enabled = true;
			this.btnRemove.Enabled = true;

			//
			// expand the tree view above
			AssetTreeNode keyNode = getAssetNode(AssetTypesTreeView.Nodes, keyAsset.Name);
			AssetTreeNode packageNode = getAssetNode(keyNode.Nodes, packageAsset.Name);
			AssetTreeNode groupNode = getAssetNode(packageNode.Nodes, groupAsset.Name);
			if (keyNode == null || packageNode == null || groupNode == null) 
			{
				OutputBox.writeln("\t--> GroupComboBox_SelectionChangeCommitted(): couldn't lookup node for selected key/package/group");
				return;
			}

			keyNode.ExpandMe();
			packageNode.ExpandMe();
			groupNode.ExpandMe();

			AssetTypesTreeView.SelectedNode = groupNode;

			OutputBox.writeln( this.ProjectLabel.Text + "." + (string)this.KeyComboBox.SelectedItem + "." + (string)this.PackageComboBox.SelectedItem + "." + (string)this.GroupComboBox.SelectedItem + " selected from combo boxes" );
		}

		private void btnEdit_Click(object sender, System.EventArgs e)
		{
			if (AssetTypesTreeView.SelectedNode != null) 
			{
				AssetTreeNode atn = (AssetTreeNode)AssetTypesTreeView.SelectedNode;
				assetType asset = atn.Asset;
				
				string newName = "";
				
				switch (asset.Level)
				{
					case 0:
						// a key - this should never happen
						MessageBox.Show("Editing a key..!");
						break;
					case 1:
						// a package
						newName = AddPackageForm.EditPackage( this.KeyComboBox.Text, this.PackageComboBox.Text );
						break;
					case 2:
						// a group
						newName = AddGroupForm.EditGroup( this.KeyComboBox.Text, this.PackageComboBox.Text, this.GroupComboBox.Text );
						break;
					default:
						break;
				}
				
				if (newName != null && newName != "") 
				{
					asset.ChangeName(newName);
					atn.Text = newName;
					SelectNode(atn);
					changed = true;
					this.btnApply.Enabled = true;
				}
			}		
		}

		private void btnApply_Click(object sender, System.EventArgs e)
		{
			if (ApplyChanges(true)) 
			{
				this.btnApply.Enabled = false;
			}
		}

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			if (this.changed && !ApplyChanges(true))
			{
				return;
			}
			if (!isValidTree(this.assetTree)) 
			{
				return;
			}

			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void btnAdd_Click(object sender, System.EventArgs e)
		{
			if (AssetTypesTreeView.SelectedNode != null) 
			{
				AssetTreeNode atn = (AssetTreeNode)AssetTypesTreeView.SelectedNode;
				assetType asset = atn.Asset;
				string newAssetName = "";
				
				switch (asset.Level)
				{
					case 0:
						// a key
						newAssetName = AddPackageForm.AddPackage( this.KeyComboBox.Text );
						break;
					case 1:
						// a package
						newAssetName = AddGroupForm.AddGroup( this.KeyComboBox.Text, this.PackageComboBox.Text );
						break;
					case 2:
						// a group
						MessageBox.Show("Groups cannot have children", "Error");
						return;
					default:
						break;
				}
				
				if (newAssetName != null && newAssetName != "") 
				{
					assetType newAsset = new assetType( newAssetName, asset );
					asset.SubTypes.Add( newAsset );
					atn.ExpandMe();
					PopulateTreeView(assetTree);
					changed = true;
					this.btnApply.Enabled = true;
				}
			}
		}
		
		private void btnRemove_Click(object sender, System.EventArgs e)
		{
			if (AssetTypesTreeView.SelectedNode == null) 
			{
				return;
			}

			AssetTreeNode atn = (AssetTreeNode)AssetTypesTreeView.SelectedNode;
			assetType asset = atn.Asset;

			if (asset.Level < 1) 
			{
				return;
			}

			string msg = string.Concat("Are you sure you want to remove '", this.ProjectLabel.Text, ".", asset.QualifiedName, "' and all its components?");
			if ( MessageBox.Show(msg, "Confirm delete", MessageBoxButtons.YesNo) == DialogResult.Yes ) 
			{
				AssetTreeNode newSelectedNode = null;

				switch (asset.Level) 
				{
					case 0:
						// a key (shouldn't ever happen)
						this.assetTree.SubTypes.Remove( asset );
						AssetTypesTreeView.Nodes.Remove( atn );
						break;
					case 1:
						// a package
						newSelectedNode = (AssetTreeNode)atn.Parent;
						asset.Parent.SubTypes.Remove( asset );
						atn.Parent.Nodes.Remove( atn );
						SelectNode( newSelectedNode );
						break;
					case 2:
						// a group
						newSelectedNode = (AssetTreeNode)atn.Parent;
						asset.Parent.SubTypes.Remove( asset );
						atn.Parent.Nodes.Remove( atn );
						SelectNode( newSelectedNode );
						break;
					default:
						break;
				}

//				PopulateTreeView(assetTree);
				changed = true;
				this.btnApply.Enabled = true;
			}
		}

		private void AssetTypesTreeView_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			EditAssetTypesForm_KeyUp(sender, e);
		}
		
		private void EditAssetTypesForm_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			switch (e.KeyData) 
			{
				case Keys.Up:
				case Keys.Down:
				case Keys.Left:
				case Keys.Right:
					if (this.AssetTypesTreeView.SelectedNode != null) 
					{
						SelectNode(this.AssetTypesTreeView.SelectedNode);
					}
					break;
				case Keys.Insert:
				case Keys.N:
				case Keys.A:
					btnAdd.PerformClick();
					break;
				case Keys.E:
					this.btnEdit.PerformClick();
					break;
				case Keys.Delete:
				case Keys.D:
				case Keys.R:
					btnRemove.PerformClick();
					break;
				case Keys.F2:
					if (this.currentNode != null && !this.currentNode.IsEditing && this.currentAsset.Level > 0) 
					{
						AssetTypesTreeView.LabelEdit = true;
						AssetTypesTreeView.SelectedNode.BeginEdit();
					}
					break;
			}
		}

		private void AssetTypesTreeView_AfterLabelEdit(object sender, System.Windows.Forms.NodeLabelEditEventArgs e)
		{
			
			if (e.Label != null)
			{
				if (e.Label.Length > 0)
				{
					if (e.Label.IndexOfAny(new char[]{' '}) == -1)
					{
						// success
						e.Node.EndEdit(false);
						this.currentAsset.ChangeName(e.Label);
						this.currentNode.Text = e.Label;
						SelectNode(this.currentNode);
						changed = true;
						this.btnApply.Enabled = true;
					}
					else
					{
						// Cancel the label edit action, inform the user, and place the node in edit mode again.
						e.CancelEdit = true;
						MessageBox.Show("Asset names cannot include spaces", "Invalid asset name");
						e.Node.BeginEdit();
					}
				}
				else
				{
					e.CancelEdit = true;
				}
			}
			AssetTypesTreeView.LabelEdit = false;
		}

		private void AddMenuItem_Click(object sender, System.EventArgs e)
		{
			btnAdd.PerformClick();
		}
		private void RemoveMenuItem_Click(object sender, System.EventArgs e)
		{
			btnRemove.PerformClick();
		}
		private void EditMenuItem_Click(object sender, System.EventArgs e)
		{
			btnEdit.PerformClick();
		}

		private void AssetTypesTreeView_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			SelectNode( AssetTypesTreeView.GetNodeAt( e.X, e.Y ) );
			AssetTreeNode atn = (AssetTreeNode)AssetTypesTreeView.SelectedNode;

			if (atn == null) 
			{
				if (e.Button == MouseButtons.Right) 
				{
					OutputBox.writeln("\tmouseDown (right-click) event in tree view selected nothing");
				}
				else 
				{
					OutputBox.writeln("\tmouseDown event in tree view selected nothing");
				}
			}
			else 
			{
				if (e.Button == MouseButtons.Right) 
				{
					OutputBox.writeln("\tmouseDown event in tree view selected (right-click) " + atn.Asset.QualifiedName);
				}
				else 
				{
					OutputBox.writeln("\tmouseDown event in tree view selected " + atn.Asset.QualifiedName);
				}
			}
		}

		private void AssetTypesTreeView_AfterCollapse(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			AssetTreeNode atn = (AssetTreeNode)this.AssetTypesTreeView.SelectedNode;
			if (atn == null) 
			{
				return;
			}

			OutputBox.writeln("AssetTypesTreeView_AfterCollapse()");
//			atn.CollapseMe();
			atn.ShouldBeExpanded = false;
		}

		private void AssetTypesTreeView_AfterExpand(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			AssetTreeNode atn = (AssetTreeNode)this.AssetTypesTreeView.SelectedNode;
			if (atn == null) 
			{
				return;
			}

			OutputBox.writeln("AssetTypesTreeView_AfterExpand()");
//			atn.ExpandMe();
			atn.ShouldBeExpanded = true;
		}
		
		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			/*	
			this.DialogResult = DialogResult.Cancel;
			if (this.changed && MessageBox.Show("Would you like to save the changes made to the asset tree?", "Save changes?", MessageBoxButtons.YesNo) == DialogResult.Yes) 
			{
				if (ApplyChanges(true)) 
				{
					this.DialogResult = DialogResult.OK;
				}
			}
			this.Dispose();
			*/	

			this.DialogResult = DialogResult.Cancel;
			this.Dispose();
		}

		#endregion
	}

	#region Supporting classes

	// a treenode wrapper that contains a pointer to assetType
	class AssetTreeNode : TreeNode 
	{
		private assetType asset;
		private bool shouldBeExpanded;
		public assetType Asset { get{return asset;} }
		public bool ShouldBeExpanded { get{return this.shouldBeExpanded;} set{this.shouldBeExpanded = value;} }

		public AssetTreeNode(string name, assetType asset) : base(name)
		{
			this.asset = asset;
			this.shouldBeExpanded = false;
		}

		public void ExpandMe()
		{
			Expand();
			this.shouldBeExpanded = true;
		}

		public void CollapseMe()
		{
			Collapse();
			this.shouldBeExpanded = false;
		}
	}
	

	class assetType 
	{
		private string name;
		private string originalName;
		private string prefix;
		private string qualifiedName;
		private assetType parent;
		private int level;
		private ArrayList subTypes;
		private bool expanded;

		public string Name { get{return name;} }
		public string OriginalName { get{return originalName;} }
		public string Prefix { get{return prefix;} }
		public string QualifiedName { get{return qualifiedName;} }
		public int Level { get{return level;} }
		public ArrayList SubTypes { get{return subTypes;} }
		public assetType Parent { get{return parent;} }
		public bool Expanded { get{return expanded;} set{expanded = value;} }

		public void ChangeName(string newName) 
		{
			name = newName;
			qualifiedName = string.Concat(this.prefix, ".", newName);
		}

		public bool ContainsSubType( string subtypeName ) 
		{
			foreach (assetType asset in this.SubTypes) 
			{
				if (asset.Name.ToUpper() == subtypeName.ToUpper()) 
				{
					return true;
				}
			}

			return false;
		}

		public void RemoveSubType( string name ) 
		{
			assetType assetToRemove = null;

			foreach (assetType asset in this.SubTypes) 
			{
				if (asset.Name.ToUpper() == name.ToUpper()) 
				{
					assetToRemove = asset;
					break;
				}
			}

			if (assetToRemove != null) 
			{
				this.SubTypes.Remove( assetToRemove);
			}
		}

		public void ReplaceSubType( assetType currentAsset, assetType newAsset ) 
		{
			for (int i = 0; i < this.SubTypes.Count; i++) 
			{
				if ( ((assetType)this.SubTypes[i]).Name.ToUpper() == currentAsset.Name.ToUpper() ) 
				{
					this.SubTypes[i] = newAsset;
					return;
				}
			}
		}

		public void ReplaceSubType( assetType newAsset ) 
		{
			for (int i = 0; i < this.SubTypes.Count; i++) 
			{
				if ( ((assetType)this.SubTypes[i]).Name.ToUpper() == newAsset.Name.ToUpper() ) 
				{
					this.SubTypes[i] = newAsset;
					return;
				}
			}
		}

		public override bool Equals(object obj) 
		{
			assetType at = (assetType)obj;
			if (this.Name.ToUpper() != at.Name.ToUpper())
				return false;
			if (this.Prefix.ToUpper() != at.Prefix.ToUpper())
				return false;
			if (this.QualifiedName.ToUpper() != at.QualifiedName.ToUpper())
				return false;
			//			if (this.Parent != at.Parent)
			//				return false;
			//			if (this.Level != at.Level)
			//				return false;
			//			if (this.SubTypes.Count != at.SubTypes.Count)
			//				return false;
			return true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public assetType(string n) 
		{
			name = n;
			originalName = n;
			prefix = "";
			qualifiedName = n;
			level = 0;
			subTypes = new ArrayList();
			expanded = false;
		}
		public assetType(string n, assetType parent)
		{
			name = n;
			originalName = n;
			this.parent = parent;
			subTypes=new ArrayList();
			expanded = false;
			if (parent != (assetType)null) 
			{
				prefix = parent.QualifiedName;
				qualifiedName = string.Concat(parent.QualifiedName, ".", n);
				level = parent.Level+1;
			}
			else 
			{
				prefix = "";
				qualifiedName = n;
				level = 0;
			}
		}
	}
	#endregion

	public class OutputBox : System.Windows.Forms.Form
	{
		#region System definitions
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.RichTextBox txtBox;
		private System.Windows.Forms.Button btnLoad;
		private System.Windows.Forms.Button btnClear;
		private System.Windows.Forms.CheckBox AlwaysOnTopCheckBox;

		private System.ComponentModel.Container components = null;

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
		private void InitializeComponent()
		{
			this.btnSave = new System.Windows.Forms.Button();
			this.btnClose = new System.Windows.Forms.Button();
			this.txtBox = new System.Windows.Forms.RichTextBox();
			this.btnLoad = new System.Windows.Forms.Button();
			this.btnClear = new System.Windows.Forms.Button();
			this.AlwaysOnTopCheckBox = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// btnSave
			// 
			this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnSave.Location = new System.Drawing.Point(232, 296);
			this.btnSave.Name = "btnSave";
			this.btnSave.TabIndex = 6;
			this.btnSave.Text = "Save";
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// btnClose
			// 
			this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnClose.Location = new System.Drawing.Point(328, 296);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(72, 24);
			this.btnClose.TabIndex = 5;
			this.btnClose.Text = "Close";
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// txtBox
			// 
			this.txtBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.txtBox.Location = new System.Drawing.Point(16, 48);
			this.txtBox.Name = "txtBox";
			this.txtBox.Size = new System.Drawing.Size(408, 232);
			this.txtBox.TabIndex = 4;
			this.txtBox.Text = "";
			// 
			// btnLoad
			// 
			this.btnLoad.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnLoad.Location = new System.Drawing.Point(136, 296);
			this.btnLoad.Name = "btnLoad";
			this.btnLoad.TabIndex = 8;
			this.btnLoad.Text = "Load";
			this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
			// 
			// btnClear
			// 
			this.btnClear.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnClear.Location = new System.Drawing.Point(40, 296);
			this.btnClear.Name = "btnClear";
			this.btnClear.TabIndex = 9;
			this.btnClear.Text = "Clear";
			this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
			// 
			// AlwaysOnTopCheckBox
			// 
			this.AlwaysOnTopCheckBox.Location = new System.Drawing.Point(24, 16);
			this.AlwaysOnTopCheckBox.Name = "AlwaysOnTopCheckBox";
			this.AlwaysOnTopCheckBox.TabIndex = 10;
			this.AlwaysOnTopCheckBox.Text = "Always on top";
			this.AlwaysOnTopCheckBox.CheckedChanged += new System.EventHandler(this.AlwaysOnTopCheckBox_CheckedChanged);
			// 
			// OutputBox
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnClose;
			this.ClientSize = new System.Drawing.Size(440, 334);
			this.Controls.Add(this.AlwaysOnTopCheckBox);
			this.Controls.Add(this.btnClear);
			this.Controls.Add(this.btnLoad);
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.txtBox);
			this.MinimumSize = new System.Drawing.Size(448, 288);
			this.Name = "OutputBox";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Output Box";
			this.Closed += new System.EventHandler(this.OutputBox_Closed);
			this.ResumeLayout(false);

		}
		#endregion
		#endregion

		#region User definitions
		private static OutputBox ob;
		public static bool Running { get{return ob != null;} }
		private static bool AllowOutputBox = false;
		#endregion

		#region Constructor
		public OutputBox()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

		public void init() 
		{
			this.Location = new Point( 0,0 );
			this.Show();
		}
		#endregion

		#region Member functions

		public static void newBox() 
		{
			ob = new OutputBox();
			ob.init();
		}

		public static void newBoxIfNecessary(Form parent) 
		{
			if (!AllowOutputBox) 
			{
				return;
			}

			if (ob == null) 
			{
				newBox();
			}
		}

		public static void writeln(string line) 
		{
			if (!AllowOutputBox) 
			{
				return;
			}

			if (ob == null) 
			{
				ob = new OutputBox();
				ob.init();
			}

			ob.WriteLine(line);
		}

		public void WriteLine(string line) 
		{
			if (!AllowOutputBox) 
			{
				return;
			}

			this.txtBox.AppendText(line + "\n");
		}

		public static void Kill() 
		{
			if (!AllowOutputBox) 
			{
				return;
			}

			if (ob != null) 
			{
				ob = null;
			}
		}
		#endregion

		#region Events
		private void btnSave_Click(object sender, System.EventArgs e)
		{
			SaveFileDialog sfd = new SaveFileDialog();
			if (sfd.ShowDialog() == DialogResult.OK) 
			{
				StreamWriter sr = new StreamWriter(sfd.FileName);
				foreach (string line in this.txtBox.Lines)
				{
					sr.WriteLine(line);
				}
				sr.Close();
				MessageBox.Show(sfd.FileName + " successfully saved", "Log saved");
			}
		}

		private void btnLoad_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			if (ofd.ShowDialog() == DialogResult.OK && File.Exists( ofd.FileName )) 
			{
				this.txtBox.Clear();
				StreamReader sr = new StreamReader( ofd.FileName );
				string line = sr.ReadLine();
				while (line != null) 
				{
					this.txtBox.AppendText(line + "\n");
					line = sr.ReadLine();
				}
			}
		}

		private void btnClose_Click(object sender, System.EventArgs e)
		{
			Dispose();
			Kill();
		}

		private void btnClear_Click(object sender, System.EventArgs e)
		{
			this.txtBox.Clear();
		}

		private void OutputBox_Closed(object sender, System.EventArgs e)
		{
			ob = null;
		}

		private void AlwaysOnTopCheckBox_CheckedChanged(object sender, System.EventArgs e)
		{
			if (AlwaysOnTopCheckBox.Checked) 
			{
				this.TopMost = true;
			}
			else 
			{
				this.TopMost = false;
			}
		}
		#endregion
	}

}










