using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;

using Iesi.Collections;

using MOG;
using MOG.INI;
using MOG.PROPERTIES;
using MOG.FILENAME;
using MOG.CONTROLLER.CONTROLLERASSET;
using MOG.CONTROLLER.CONTROLLERREPOSITORY;
using MOG.CONTROLLER.CONTROLLERPACKAGE;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.REPORT;
using MOG.PROMPT;
using MOG.PROGRESS;
using MOG.PLATFORM;

using MOG_ControlsLibrary;
using MOG_ControlsLibrary.Controls;
using MOG_ControlsLibrary.Utils;
using MOG_ControlsLibrary.Common.MogControl_RepositoryTreeViews;
using MOG_Client.Client_Utilities;
using MOG_Client.Client_Mog_Utilities;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using MOG.ASSET_STATUS;
using MOG.CONTROLLER.CONTROLLERINBOX;

namespace MOG_Client.Forms
{
	/// <summary>
	/// Summary description for PackageManagementForm.
	/// </summary>
	public class PackageManagementForm : Form
	{
		#region Main variables
		private bool mbHasBlessedAsset;
		private bool mbInitializing = false;

		private bool mDirty = false;
		public bool Dirty
		{
			get { return mDirty; }			
		}
	
		List<MOG_Filename> mAssetFilenames = new List<MOG_Filename>();

		private string mSelectedClassification;
		[Browsable(false)]
		public string SelectedClassification
		{
			get { return (mSelectedClassification != null) ? mSelectedClassification : ""; }
			set { mSelectedClassification = value; }
		}

		private const string blessedAssetMessage = "Unable to add to Asset(s) because one or more Asset (in the far left pane) is Blessed.\r\n\r\n"
			+ "To add a package link to Asset(s) on the left, simply copy the Asset(s) to your Inbox, make changes, then re-Bless the Asset(s).";
		private const string blessedAssetCaption = "Cannot Add/Delete From Blessed Asset(s)!";
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ContextMenu PackageSelectedPackagesContextMenu;
		private System.Windows.Forms.MenuItem SelectedPackagesRemoveMenuItem;
		private System.Windows.Forms.ToolTip FormToolTip;
		private System.Windows.Forms.Button CloseButton;
		public ListView AssetList;
		private bool ModifyingAssets;
		private ColumnHeader AssetNameColumnHeader;
		private ColumnHeader ClassificationColumnHeader;
		public ListView AssignmentList;
		private ColumnHeader PackageNameColumnHeader;
		private MogControl_PackageManagementTreeView PackageTree;
		private SplitContainer MainSplitContainer;
		private SplitContainer AssignmentSplitContainer;
		private Label PackageManagementLockedLabel;
		private System.ComponentModel.IContainer components;
		#endregion

		/// <summary>
		/// Construct using multiple items in an ArrayList of MOG_Filenames
		/// </summary>
		public PackageManagementForm(ArrayList SelectedItems, bool hasBlessedAsset)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			mbHasBlessedAsset = hasBlessedAsset;
			
			foreach (MOG_Filename filename in SelectedItems)
			{
				mAssetFilenames.Add(filename);
			}

			// Initialize our TreeView and make sure we have our CheckBoxes
			PackageTree.TreeView.Initialize(Initialize);
			PackageTree.TreeView.CheckBoxes = true;

			// Add our MenuItems for Refreshing the Tree...
			PackageTree.PackageTreeContextMenu.Items.Add(new ToolStripSeparator());
			PackageTree.PackageTreeContextMenu.Items.Add(new ToolStripMenuItem("Refresh Tree", null, new EventHandler(RefreshPackageTreeViewMenuItem_Click), Keys.F5));

			PackageTree.TreeView.BeforeCheck += PackageTree_BeforeCheck;
			PackageTree.TreeView.AfterCheck += PackageTree_AfterCheck;
			PackageTree.TreeView.AfterSelect += PackageTree_AfterSelect;
		}
		public PackageManagementForm(ArrayList SelectedItems)
			: this(SelectedItems, false)
		{
		}

		private void CheckValidClassificationExists()
		{
			// If, after we initialize, our TreeView is not Enabled, that means we have nothing to display...
			if (!PackageTree.TreeView.Enabled)
			{
				MOGPromptResult result = MOG_Prompt.PromptResponse("Missing Package Classification",
					"Package Management requires a package classification.\r\n\r\n"
					+ "MOG can automatically set the '1stPackage' property for you on a specific classification where packages can be added.\r\n\r\n"
					+ "Do you want to enable Package Management in your project?"
					, MOGPromptButtons.YesNo);
				if (result != MOGPromptResult.Yes)
				{
					this.Close();
				}
				else
				{
					// Allow the user to choose the classification he/she would like to use as the first IsPackage Classification
					BrowseClassTreeForm bctForm = new BrowseClassTreeForm();
					bctForm.StartPosition = FormStartPosition.CenterParent;
					bctForm.Text = "Choose Classification To Add Packages To";

					if (bctForm.ShowDialog(this) == DialogResult.OK)
					{
						string classificationName = bctForm.Tag as string;
						MOG_Properties props = MOG_Properties.OpenClassificationProperties(classificationName);
						props.IsPackage = true;
						props.Close();
						PackageTree.TreeView.Enabled = true;
						PackageTree.TreeView.DeInitialize();
					}
					else
					{
						this.Close();
					}
				}
			}
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PackageManagementForm));
			this.PackageSelectedPackagesContextMenu = new System.Windows.Forms.ContextMenu();
			this.SelectedPackagesRemoveMenuItem = new System.Windows.Forms.MenuItem();
			this.CloseButton = new System.Windows.Forms.Button();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.FormToolTip = new System.Windows.Forms.ToolTip(this.components);
			this.AssetList = new System.Windows.Forms.ListView();
			this.AssetNameColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.ClassificationColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.AssignmentList = new System.Windows.Forms.ListView();
			this.PackageNameColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.PackageTree = new MOG_ControlsLibrary.Controls.MogControl_PackageManagementTreeView();
			this.MainSplitContainer = new System.Windows.Forms.SplitContainer();
			this.AssignmentSplitContainer = new System.Windows.Forms.SplitContainer();
			this.PackageManagementLockedLabel = new System.Windows.Forms.Label();
			this.MainSplitContainer.Panel1.SuspendLayout();
			this.MainSplitContainer.Panel2.SuspendLayout();
			this.MainSplitContainer.SuspendLayout();
			this.AssignmentSplitContainer.Panel1.SuspendLayout();
			this.AssignmentSplitContainer.Panel2.SuspendLayout();
			this.AssignmentSplitContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// PackageSelectedPackagesContextMenu
			// 
			this.PackageSelectedPackagesContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.SelectedPackagesRemoveMenuItem});
			this.PackageSelectedPackagesContextMenu.Popup += new System.EventHandler(this.PackageSelectedPackagesContextMenu_Popup);
			// 
			// SelectedPackagesRemoveMenuItem
			// 
			this.SelectedPackagesRemoveMenuItem.Index = 0;
			this.SelectedPackagesRemoveMenuItem.Shortcut = System.Windows.Forms.Shortcut.Del;
			this.SelectedPackagesRemoveMenuItem.Text = "Remove";
			this.SelectedPackagesRemoveMenuItem.Click += new System.EventHandler(this.SelectedPackagesRemoveMenuItem_Click);
			// 
			// CloseButton
			// 
			this.CloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.CloseButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.CloseButton.Location = new System.Drawing.Point(525, 475);
			this.CloseButton.Name = "CloseButton";
			this.CloseButton.Size = new System.Drawing.Size(75, 23);
			this.CloseButton.TabIndex = 3;
			this.CloseButton.Text = "Done";
			this.FormToolTip.SetToolTip(this.CloseButton, "Apply the changes listed above");
			this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Group Name";
			this.columnHeader2.Width = 171;
			// 
			// AssetList
			// 
			this.AssetList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.AssetNameColumnHeader,
            this.ClassificationColumnHeader});
			this.AssetList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.AssetList.FullRowSelect = true;
			this.AssetList.Location = new System.Drawing.Point(0, 0);
			this.AssetList.Name = "AssetList";
			this.AssetList.Size = new System.Drawing.Size(593, 111);
			this.AssetList.TabIndex = 1;
			this.FormToolTip.SetToolTip(this.AssetList, "Assets that are being modified");
			this.AssetList.UseCompatibleStateImageBehavior = false;
			this.AssetList.View = System.Windows.Forms.View.Details;
			// 
			// AssetNameColumnHeader
			// 
			this.AssetNameColumnHeader.Text = "Asset Name";
			this.AssetNameColumnHeader.Width = 253;
			// 
			// ClassificationColumnHeader
			// 
			this.ClassificationColumnHeader.Text = "Classification";
			this.ClassificationColumnHeader.Width = 335;
			// 
			// AssignmentList
			// 
			this.AssignmentList.AllowDrop = true;
			this.AssignmentList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.PackageNameColumnHeader});
			this.AssignmentList.ContextMenu = this.PackageSelectedPackagesContextMenu;
			this.AssignmentList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.AssignmentList.FullRowSelect = true;
			this.AssignmentList.Location = new System.Drawing.Point(0, 0);
			this.AssignmentList.Name = "AssignmentList";
			this.AssignmentList.Size = new System.Drawing.Size(593, 123);
			this.AssignmentList.TabIndex = 2;
			this.FormToolTip.SetToolTip(this.AssignmentList, "Packages assigned to the Assets on the left pane.");
			this.AssignmentList.UseCompatibleStateImageBehavior = false;
			this.AssignmentList.View = System.Windows.Forms.View.Details;
			// 
			// PackageNameColumnHeader
			// 
			this.PackageNameColumnHeader.Text = "Assigned Packages";
			this.PackageNameColumnHeader.Width = 578;
			// 
			// PackageTree
			// 
			this.PackageTree.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PackageTree.ExpandAssets = false;
			this.PackageTree.Location = new System.Drawing.Point(0, 0);
			this.PackageTree.Name = "PackageTree";
			this.PackageTree.Size = new System.Drawing.Size(593, 206);
			this.PackageTree.TabIndex = 16;
			this.PackageTree.UsePlatformSpecificCheckBox = true;
			// 
			// MainSplitContainer
			// 
			this.MainSplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.MainSplitContainer.Location = new System.Drawing.Point(7, 12);
			this.MainSplitContainer.Name = "MainSplitContainer";
			this.MainSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// MainSplitContainer.Panel1
			// 
			this.MainSplitContainer.Panel1.Controls.Add(this.AssetList);
			// 
			// MainSplitContainer.Panel2
			// 
			this.MainSplitContainer.Panel2.Controls.Add(this.AssignmentSplitContainer);
			this.MainSplitContainer.Size = new System.Drawing.Size(593, 448);
			this.MainSplitContainer.SplitterDistance = 111;
			this.MainSplitContainer.TabIndex = 17;
			// 
			// AssignmentSplitContainer
			// 
			this.AssignmentSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.AssignmentSplitContainer.Location = new System.Drawing.Point(0, 0);
			this.AssignmentSplitContainer.Name = "AssignmentSplitContainer";
			this.AssignmentSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// AssignmentSplitContainer.Panel1
			// 
			this.AssignmentSplitContainer.Panel1.Controls.Add(this.PackageTree);
			// 
			// AssignmentSplitContainer.Panel2
			// 
			this.AssignmentSplitContainer.Panel2.Controls.Add(this.AssignmentList);
			this.AssignmentSplitContainer.Size = new System.Drawing.Size(593, 333);
			this.AssignmentSplitContainer.SplitterDistance = 206;
			this.AssignmentSplitContainer.TabIndex = 0;
			// 
			// PackageManagementLockedLabel
			// 
			this.PackageManagementLockedLabel.AutoSize = true;
			this.PackageManagementLockedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.PackageManagementLockedLabel.ForeColor = System.Drawing.Color.Red;
			this.PackageManagementLockedLabel.Location = new System.Drawing.Point(302, 480);
			this.PackageManagementLockedLabel.Name = "PackageManagementLockedLabel";
			this.PackageManagementLockedLabel.Size = new System.Drawing.Size(208, 13);
			this.PackageManagementLockedLabel.TabIndex = 18;
			this.PackageManagementLockedLabel.Text = "PACKAGE MANAGEMENT LOCKED";
			this.PackageManagementLockedLabel.Visible = false;
			// 
			// PackageManagementForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(612, 506);
			this.Controls.Add(this.PackageManagementLockedLabel);
			this.Controls.Add(this.MainSplitContainer);
			this.Controls.Add(this.CloseButton);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(368, 344);
			this.Name = "PackageManagementForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Package Management";
			this.Load += new System.EventHandler(this.PackageManagementForm_Load);
			this.Closing += new System.ComponentModel.CancelEventHandler(this.PackageManagementForm_Closing);
			this.MainSplitContainer.Panel1.ResumeLayout(false);
			this.MainSplitContainer.Panel2.ResumeLayout(false);
			this.MainSplitContainer.ResumeLayout(false);
			this.AssignmentSplitContainer.Panel1.ResumeLayout(false);
			this.AssignmentSplitContainer.Panel2.ResumeLayout(false);
			this.AssignmentSplitContainer.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		#region Asset methods
		/// <summary>
		/// Build our assets list from an ArrayList of MOG_Filenames.  This is to only be used on construction/initialization.
		/// </summary>
		private void Initialize()
		{
			mbInitializing = true;

			// Clear all the inherited package assignments in preparation of repoulating everything
			ClearInheritedPackageAssignments();

			// Store all the Packages we find as we go through our SelectedItems
			HybridDictionary allPackages_NonInherited = new HybridDictionary(true);
			HybridDictionary allPackages_Inherited = new HybridDictionary(true);

			AssetList.Items.Clear();
			AssignmentList.Items.Clear();

			// Add all the valid assets to the assets Listview
			foreach (MOG_Filename filename in mAssetFilenames)
			{
				string classificationToDisplay = null;
				string nameToDisplay = null;
				MOG_Properties propertiesInfo = null;

				if (filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
				{
					//this is an asset
					nameToDisplay = filename.GetAssetName();
					classificationToDisplay = filename.GetAssetClassification();
					// Add the package name
					propertiesInfo = new MOG_Properties(filename);
				}
				else if (filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Unknown)
				{
					//this is a classification
					nameToDisplay = "";
					classificationToDisplay = filename.GetOriginalFilename();
					propertiesInfo = new MOG_Properties(filename.GetOriginalFilename());
				}
				else
				{
					continue;
				}

				List<string> packageNames_NonInherited = GetPackageNames(propertiesInfo.GetNonInheritedPackages());
				AddPackageNamesToList(allPackages_NonInherited, packageNames_NonInherited);
				if (packageNames_NonInherited.Count == 0)
				{
					List<string> packageNames_Inherited = GetPackageNames(propertiesInfo.GetInheritedPackages());
					AddPackageNamesToList(allPackages_Inherited, packageNames_Inherited);
				}

				// Add an Asset to our list of Assets
				AddAssetListViewItem(nameToDisplay, classificationToDisplay, filename);

				// Check if this asset's package management has been locked?
				if (propertiesInfo.LockPackageManagement == true)
				{
					// Show the user that package management has been locked
					PackageManagementLockedLabel.Visible = true;
					// Disable the dialog
					AssignmentList.Enabled = false;
//					PackageTree.Enabled = false;
					PackageTree.TreeView.Enabled = false;
				}
			}

			foreach (ColumnHeader column in AssetList.Columns)
			{
				//-1 means set the width to the longest item in the column
				//-2 does the same thing but won't cut off the column header either
				column.Width = -2;
			}

			CheckPackages(allPackages_NonInherited, false);
			CheckPackages(allPackages_Inherited, true);

			mbInitializing = false;
		}

		private List<string> GetPackageNames(ArrayList packages)
		{
			List<string> packageNames = new List<string>();

			// Add our packages to our mogPackagesSet and expand the tree to that node
			foreach (MOG_Property package in packages)
			{
				packageNames.Add(package.mPropertyKey);
			}

			return packageNames;
		}

		private void AddPackageNamesToList(HybridDictionary list, List<string> packageNames)
		{
			foreach (string package in packageNames)
			{
				if (list.Contains(package))
				{
					list[package] = (int)list[package] + 1;
				}
				else
				{
					list[package] = 1;
				}
			}
		}

		private void CheckPackages(HybridDictionary packages, bool markAsInherited)
		{
			// Add our packages to our mogPackagesSet and expand the tree to that node
			foreach (DictionaryEntry entry in packages)
			{
				string package = entry.Key as string;
				int count = (int)entry.Value;

				// Generate a MOG_Filename and concat our path from it
				MOG_Filename packageFile = new MOG_Filename(package);
				string assetName = packageFile.GetAssetName();
				string assetFullName = packageFile.GetAssetClassification() + assetName;

				// Make sure we have this Package selected (if possible)
				TreeNode node = FindPackageNode(assetFullName);
				if (node == null)
				{
					node = AddPackageNode(assetFullName);
				}
				if (node != null)
				{
					if (count < AssetList.Items.Count)
					{
						node.ForeColor = Color.Gray;
					}

					if (markAsInherited)
					{
						node.NodeFont = new Font(node.TreeView.Font, FontStyle.Italic);
					}

					node.Checked = true;
				}
			}
		}

		private void ClearInheritedPackageAssignments()
		{
			foreach (ListViewItem item in AssignmentList.Items)
			{
				// Generate a MOG_Filename and concat our path from it
				MOG_Filename packageFile = new MOG_Filename(item.Text);
				string assetName = packageFile.GetAssetName();
				string assetFullName = packageFile.GetAssetClassification() + assetName;

				// Find this package's node
				TreeNode node = FindPackageNode(assetFullName);
				if (node != null)
				{
					// Check if this was inherited?
					if (node.NodeFont != null && node.NodeFont.Italic)
					{
						// Clear the check and reset its font
						node.Checked = false;
						node.NodeFont = new Font(AssignmentList.Font, FontStyle.Regular);
					}
				}
			}
		}

		private void AddAssetListViewItem(string name, string classification, MOG_Filename filename)
		{
			if (name != null && classification != null)
			{
				// Create a new item to add to the Assets ListView
				ListViewItem item = new ListViewItem(name);

				// Make our MOG_Filename our tag (for reference)
				item.Tag = filename;

				// Add the asset to the assets list
				item.SubItems.Add(classification);
				AssetList.Items.Add(item);
			}
		}

		private bool IsSelectedAsset(string assetName)
		{
			foreach (MOG_Filename filename in mAssetFilenames)
			{
				if (String.Compare(filename.GetAssetFullName(), assetName, true) == 0)
				{
					return true;
				}
			}

			return false;
		}
		
		private TreeNode FindPackageNode(string packageAssignment)
		{
			return PackageTree.TreeView.DrillToPackage(packageAssignment);
		}

		private TreeNode AddPackageNode(string packageAssignment)
		{
			TreeNode node = PackageTree.TreeView.AddNode_PackageAssignment(packageAssignment);
			if (node != null)
			{
				PackageTree.TreeView.DrillToPackage(packageAssignment);
			}

			return node;
		}

		/// <summary>
		/// Function to determine if the passed in package is already within our packageListView
		/// </summary>
		/// <returns>True if the package was found, false if not</returns>
		private ListViewItem FindAssignmentListItem(string packageName)
		{
			foreach (ListViewItem item in AssignmentList.Items)
			{
				if (String.Compare(item.Text, packageName, true) == 0)
				{
					return item;
				}
			}

			return null;
		}

		private bool AddPackageLinkToAssets(string packageName, Color color, bool markAsInherited, bool setPackageProperties)
		{
			bool success = true;

			ListViewItem item = FindAssignmentListItem(packageName);
			if (item == null)
			{
				item = new ListViewItem(packageName);
				item.ForeColor = color;
				AssignmentList.Items.Add(item);

				if (setPackageProperties)
				{
					success = false;

					ArrayList PackageAssignmentProps = new ArrayList();
					PackageAssignmentProps.Add(CreatePackageAssignmentProperty(packageName));

					// Go through our listview items again to Add or Remove assets we found in the previous foreach loop
					foreach (ListViewItem assetItem in AssetList.Items)
					{
						MOG_Filename assetFilename = assetItem.Tag as MOG_Filename;
						if (assetFilename != null &&
							assetFilename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
						{
							// If this asset is not a blessed Asset, and it's not a classification...
							if (!assetFilename.IsBlessed())
							{
								MOG_ControllerAsset asset = MOG_ControllerAsset.OpenAsset(assetFilename);
								if (asset != null)
								{
									if (asset.GetProperties().AddPackages(PackageAssignmentProps))
									{
										success = true;

										// Make sure we keep the group in sync with our new assignment
										MOG_ControllerAsset.SetDefaultGroup(asset);

										// Change the state of the asset to indicate it has been modified
										MOG_ControllerInbox.UpdateInboxView(asset, MOG_AssetStatusType.Modifying);
										ModifyingAssets = true;
									}

									asset.Close();
								}
							}
							else
							{
								MOG_Prompt.PromptMessage("Cannot add PackageAssignments to Blessed Assets",
														 "Please copy the Asset to your Inbox and change its package assignments from there so it can be properly processed and re-Blessed.",
														 "",
														 MOG.PROMPT.MOG_ALERT_LEVEL.ALERT);
							}
						}
						else if (assetFilename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Unknown)
						{
							// We should be looking at a classification, so treat it as such...
							MOG_Properties properties = MOG_Properties.OpenClassificationProperties(assetFilename.GetOriginalFilename());
							if (properties != null)
							{
								if (properties.AddPackages(PackageAssignmentProps))
								{
									success = true;
								}
								properties.Close();
							}
						}
					}
				}
			}

			if (item != null && markAsInherited)
			{
				item.Font = new Font(AssignmentList.Font, FontStyle.Italic);
			}
			
			if (!success)
			{
				MOG_Report.ReportMessage("Error Adding Package Assignment", "Failed to add package assignment!", Environment.StackTrace, MOG_ALERT_LEVEL.ERROR);
			}

			return success;
		}

		private MOG_Property CreatePackageAssignmentProperty(string package)
		{
			// Initialize our packages ArrayLists for addition and removal
			string packageName = MOG_ControllerPackage.GetPackageName(package);
			string packageGroups = MOG_ControllerPackage.GetPackageGroups(package);
			string packageObjects = MOG_ControllerPackage.GetPackageObjects(package);
			return MOG_PropertyFactory.MOG_Relationships.New_PackageAssignment(packageName, packageGroups, packageObjects);
		}

		/// <summary>
		/// Remove a package from the assigned packages list
		/// </summary>
		private void SelectedPackagesRemoveMenuItem_Click(object sender, System.EventArgs e)
		{
			foreach (ListViewItem item in AssignmentList.SelectedItems)
			{
				TreeNode node = FindPackageNode(item.Text);
				if (node != null)
				{
					node.Checked = false;
				}
			}
		}

		/// <summary>
		/// Remove packages from Assets currently selected.  Use CheckForBlessedAssetMessage() before calling.
		/// </summary>
		/// <param name="packagesToRemove">ArrayList of MOG_Property objects for packages to be removed</param>
		private void RemovePackageLinkFromAssets(ArrayList packagesToRemove)
		{
			foreach (ListViewItem item in AssetList.Items)
			{
				MOG_Filename assetFilename = item.Tag as MOG_Filename;
				if (assetFilename != null)
				{
					// If this asset is not a blessed Asset, and it's not a classification...
					if (!assetFilename.IsBlessed() && assetFilename.GetFilenameType() != MOG_FILENAME_TYPE.MOG_FILENAME_Unknown)
					{
						// Open up this asset so we can remove the package relationships
						MOG_ControllerAsset asset = MOG_ControllerAsset.OpenAsset(assetFilename);
						if (asset != null)
						{
							// Remove the packages
							asset.GetProperties().RemovePackages(packagesToRemove);
							// Depending on how the user clicked, it is possible that an inherited package assignment can be left behind as a non-inherited property
							// The following code ensures all non-inherited package assignments are flushed by restamping any non-inherited assignemnts.
							asset.GetProperties().SetProperties(asset.GetProperties().GetNonInheritedPackages());

							// Make sure we keep the group in sync with our new assignment
							MOG_ControllerAsset.SetDefaultGroup(asset);

							// Change the state of the asset to indicate it has been modified
							MOG_ControllerInbox.UpdateInboxView(asset, MOG_AssetStatusType.Modifying);
							ModifyingAssets = true;

							asset.Close();
						}
					}
					else
					{
						// We should be looking at a classification, so treat it as such...
						MOG_Properties properties = MOG_Properties.OpenClassificationProperties(assetFilename.GetOriginalFilename());
						if (properties != null)
						{
							properties.RemovePackages(packagesToRemove);
							properties.Close();
						}
					}
				}
			}
		}

		/// <summary>
		/// Return true and display a message to user if we have a blessed asset
		/// </summary>
		private bool WarnAboutBlessedAsset()
		{
			if (mbHasBlessedAsset)
			{
				MessageBox.Show(this, blessedAssetMessage, blessedAssetCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return true;
			}
			return false;
		}
		#endregion

		private void RefreshPackageTreeViewMenuItem_Click(object sender, System.EventArgs e)
		{
			RefreshPackageTreeView();
		}

		private void RefreshPackageTreeView()
		{
			PackageTree.TreeView.DeInitialize();
			PackageTree.TreeView.Initialize();
		}

		private void PackageSelectedPackagesContextMenu_Popup(object sender, System.EventArgs e)
		{
			SelectedPackagesRemoveMenuItem.Enabled = false;
			
			ContextMenu cm = sender as ContextMenu;
			if (cm != null)
			{
				ListView listview = cm.SourceControl as ListView;
				if (listview != null && listview.SelectedItems.Count > 0)
				{
					SelectedPackagesRemoveMenuItem.Enabled = true;
				}
			}
		}

		private void CloseButton_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void PackageTree_BeforeCheck(object sender, TreeViewCancelEventArgs e)
		{
			if (!mbInitializing)
			{
				Mog_BaseTag basetag = e.Node.Tag as Mog_BaseTag;
				if (basetag != null)
				{
					if (basetag.PackageNodeType == PackageNodeTypes.Class)
					{
						MessageBox.Show(this, "Cannot select classification nodes.");
						e.Cancel = true;
					}
					else if (e.Node.NodeFont != null && e.Node.NodeFont.Italic)
					{
						MessageBox.Show(this, "This package assignment is being inherited from the classification and can't be unassigned.\n" + 
											  "You may override this inherited package assignemnt by selecting a different package.",
											  "Can't remove this package assignment");
						e.Cancel = true;
					}
					else if (IsSelectedAsset(basetag.PackageFullName))
					{
						MessageBox.Show(this, "You cannot assign an asset to be packaged into itself.");
						e.Cancel = true;
					}
				}
			}
		}

		/// <summary>
		/// Adds an asset to our ListView and to our Asset's properties
		/// </summary>
		private void PackageTree_AfterCheck(object sender, TreeViewEventArgs e)
		{
			ArrayList packageToChange = new ArrayList();
			Mog_BaseTag basetag = e.Node.Tag as Mog_BaseTag;
			if (basetag != null)
			{
				string packageFullname = basetag.PackageFullName;

				if (e.Node.Checked)
				{
					bool markAsInherited = e.Node.NodeFont != null && e.Node.NodeFont.Italic;
					bool setProperty = e.Action != TreeViewAction.Unknown;

					//This node just got checked, so lets add the links to all the assets
					AddPackageLinkToAssets(packageFullname, e.Node.ForeColor, markAsInherited, setProperty);
				}
				else
				{
					if (e.Node.ForeColor == Color.Gray)
					{
						e.Node.ForeColor = e.Node.TreeView.ForeColor;
					}

					//node got unchecked, take the packages assignments off the assets
					packageToChange.Add(CreatePackageAssignmentProperty(packageFullname));

					ListViewItem item = FindAssignmentListItem(packageFullname);
					if (item != null)
					{
						item.Remove();
					}

					RemovePackageLinkFromAssets(packageToChange);
				}

				if (!mbInitializing)
				{
					Initialize();
					mDirty = true;
				}
			}
		}

		private void PackageManagementForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			guiUserPrefs.SaveDynamic_LayoutPrefs(guiUserPrefs.PackageManagementForm_Text, this);

			// Check if we were modyifying assets?
			if (ModifyingAssets)
			{
			    // Itterate through all the assets and mark them as being modified
			    foreach (ListViewItem item in AssetList.Items)
			    {
			        MOG_Filename assetFilename = item.Tag as MOG_Filename;
			        if (assetFilename != null)
			        {
			            // Open up this asset so we can remove the package relationships
			            MOG_ControllerAsset asset = MOG_ControllerAsset.OpenAsset(assetFilename);
			            if (asset != null)
			            {
							if (asset.GetProperties().NativeDataType)
							{
								MOG_ControllerInbox.UpdateInboxView(asset, MOG_AssetStatusType.Processed);
							}
							else
							{
								MOG_ControllerInbox.UpdateInboxView(asset, MOG_AssetStatusType.Modified);
							}
							asset.Close();
			            }
			        }
			    }
			}
		}

		private void PackageTree_AfterSelect(object sender, TreeViewEventArgs e)
		{
			//we're just keeping track of what the classification was on the most recently selected node
			Match match = Regex.Match(e.Node.FullPath, "^([^{}]*)(?:{.+}.+)?$");
			if (match.Groups.Count > 1)
			{
				SelectedClassification = match.Groups[1].Value;
			}
		}

		private void PackageManagementForm_Load(object sender, System.EventArgs e)
		{
			CheckValidClassificationExists();

			guiUserPrefs.LoadDynamic_LayoutPrefs(guiUserPrefs.PackageManagementForm_Text, this);
			PackageTree.TreeView.DrillToAssetName(SelectedClassification);
		}
	}
}
