using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.PROJECT;
using MOG.PLATFORM;
using MOG;
using MOG.FILENAME;
using MOG_ControlsLibrary.Forms;
using MOG_ControlsLibrary.Controls;
using System.IO;
using MOG_ControlsLibrary.Utils;
using MOG.CONTROLLER.CONTROLLERSYNCDATA;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.CONTROLLER.CONTROLLERREPOSITORY;
using System.Collections;
using MOG.PROPERTIES;
using MOG.PROMPT;
using MOG.TIME;
using MOG.DOSUTILS;
using MOG.CONTROLLER.CONTROLLERPACKAGE;
using System.Threading;

namespace MOG_ControlsLibrary
{
	public partial class PackageCreator : Form
	{
		private MOG_Filename mAssetName = null;
		public MOG_Filename AssetName
		{
			get { return mAssetName; }
		}
		
		public string Platform
		{
			get { return PlatformCombo.Text; }
			set { PlatformCombo.SelectedItem = value; }
		}

		public string PackageName
		{
			get { return PackageNameTextBox.Text; }
			set { PackageNameTextBox.Text = value; }
		}

		public string Classification
		{
			get { return ClassificationTextBox.Text; }
			set	{ ClassificationTextBox.Text = value; }
		}

		public string SyncTarget
		{
			get	{ return (SyncTargetTextBox.Enabled) ? SyncTargetTextBox.Text : ""; }
			set
			{
				if (SyncTargetTextBox.Enabled)
				{
					SyncTargetTextBox.Text = value;
					SyncTargetTreeView.MOGSelectedNode = null;
				}
			}
		}

		public PackageCreator()
		{
			InitializeComponent();

			// Make sure we are not in the designer / DesignMode
			//if (GetService(typeof(System.ComponentModel.Design.IDesignerHost)) == null)
			if (System.ComponentModel.LicenseManager.UsageMode != System.ComponentModel.LicenseUsageMode.Designtime)
			{
				MOG_Project project = MOG_ControllerProject.GetProject();
				if (project != null)
				{
					PlatformCombo.Items.Add("All");
					PlatformCombo.SelectedIndex = 0;

					foreach (MOG_Platform platform in project.GetPlatforms())
					{
						PlatformCombo.Items.Add(platform.mPlatformName);
					}

					PlatformCombo.Items.Add(MOG_ControllerProject.GetAllPlatformsString());

					ClassificationTreeView.Initialize(DrillToClassification);
					SyncTargetTreeView.InitializeVirtual(Platform);

					PlatformCombo.BackColor = Color.PaleGreen;
				}
			}

			IsTextBoxValid(this.PackageNameTextBox);
			IsTextBoxValid(this.SyncTargetTextBox);
			IsTextBoxValid(this.ClassificationTextBox);
		}

		private void DrillToClassification()
		{
			if (!String.IsNullOrEmpty(Classification))
			{
				TreeNode node = ClassificationTreeView.DrillToNodePath(Classification);
				if (node != null)
				{
					ClassificationTreeView.SelectedNode = node;
				}
			}
		}

		private void btnCreate_Click(object sender, EventArgs e)
		{
			if (IsInformationValid())
			{
				string[] platforms = Platform.Split(",".ToCharArray());
				// Make sure there was something specified before we do anything
				if (platforms.Length > 0)
				{
					bool bPromptUser = true;
					bool bCreatePackage = true;
					bool bRebuildPackage = false;

					foreach (string platform in platforms)
					{
						// Create the new package name
						MOG_Filename assetName = MOG_Filename.CreateAssetName(Classification, platform.Trim(), PackageName);

						// Check if we should prompt the user?
						if (bPromptUser)
						{
							// Don't bother the user again
							bPromptUser = false;

							// Check if this was a platform specific package?
							if (assetName.IsPlatformSpecific())
							{
								// Check if there are ANY assiciated assets with this new platform-specific package?
								if (MOG_ControllerPackage.GetAssociatedAssetsForPackage(assetName).Count > 0)
								{
									// Prompt the user if they wish to automatically populate this new platform-specific packages?
									string message = "Whenever new platform-specific packages are created, they sometimes need to be populated if existing package assignments exist.\n\n" + 
													 "MOG has detected this to be the case and recommends you to automatically populated this package.";
									MOGPromptResult result = MOG_Prompt.PromptResponse("Automatically populate this new platform-specific package?", message, MOGPromptButtons.YesNo);
									switch (result)
									{
										case MOGPromptResult.Yes:
											bCreatePackage = true;
											bRebuildPackage = true;
											break;
										case MOGPromptResult.No:
											bCreatePackage = true;
											bRebuildPackage = false;
											break;
										case MOGPromptResult.Cancel:
											bCreatePackage = false;
											bRebuildPackage = false;
											break;
									}
								}
							}
						}

						// Check if we should create the package?
						if (bCreatePackage)
						{
							MOG_Filename newPackage = MOG_ControllerProject.CreatePackage(assetName, SyncTarget);
							if (newPackage != null)
							{
								// Post the new package into the project
								mAssetName = newPackage;
								string jobLabel = "NewPackageCreated." + MOG_ControllerSystem.GetComputerName() + "." + MOG_Time.GetVersionTimestamp();
								MOG_ControllerProject.PostAssets(MOG_ControllerProject.GetProjectName(), MOG_ControllerProject.GetBranchName(), jobLabel);

								// Check if we should rebuild the package?
								if (bRebuildPackage)
								{
									jobLabel = "PopulateNewPackage." + MOG_ControllerSystem.GetComputerName() + "." + MOG_Time.GetVersionTimestamp();
									// Schedule the rebuild command
									MOG_ControllerPackage.RebuildPackage(assetName, jobLabel);
									// Start the job
									MOG_ControllerProject.StartJob(jobLabel);
								}

								// Well, this is a bit of a hack but was the easiest and safest way to ensure unique JobIDs...
								// JobIDs are only accurate to the microsecond so lets sleep for a very short time.
								Thread.Sleep(10);

								// Setting the dialog's result will automatically close the dialog since we proceeded to create the package
								DialogResult = DialogResult.OK;
							}
						}
					}
				}
			}
		}

		private void ClassificationTreeView_AfterSelect(object sender, TreeViewEventArgs e)
		{
			ClassificationTextBox.Text = e.Node.FullPath;
		}

		private void SyncTargetTreeView_AfterSelect(object sender, TreeViewEventArgs e)
		{
			string relativePath = DosUtils.PathMakeRelativePath(MOG_ControllerProject.GetWorkspaceDirectory(), e.Node.FullPath);
			SyncTargetTextBox.Text = relativePath;
			SyncTargetTextBox.BackColor = Color.PaleGreen;
		}

		private void SyncTargetTreeView_AfterSelect(object sender, NodeLabelEditEventArgs e)
		{
			string relativePath = DosUtils.PathMakeRelativePath(MOG_ControllerProject.GetWorkspaceDirectory(), e.Node.FullPath);
			SyncTargetTextBox.Text = relativePath;
		}
		
		private bool IsInformationValid()
		{
			if (this.PackageNameTextBox.BackColor == Color.Tomato)
			{
				MOG_Prompt.PromptResponse("Package name missing", "Please specify a name for the new package.");
				return false;
			}

			if (MOG_ControllerProject.DoesAssetExists(MOG_Filename.CreateAssetName(Classification, Platform, PackageName)))
			{
				MOG_Prompt.PromptResponse("Package name not unique", "This package already exists, please specify a unique package name and try again.");
				return false;
			}

			if (this.ClassificationTextBox.BackColor == Color.Tomato)
			{
				MOG_Prompt.PromptResponse("Classification missing", "Please select a classification for the new package.");
				return false;
			}

			if (SyncTargetTextBox.Enabled &&
				SyncTargetTextBox.BackColor == Color.Tomato)
			{
				MOG_Prompt.PromptResponse("Sync target missing", "Please select a sync target for the new package.");
				return false;
			}

			// Check if this package is missing an extension?
			if (Path.GetExtension(PackageName).Length == 0)
			{
				// Check if we are also missing a defined DefaultPackageFileExtension?
				MOG_Properties tempProperties = new MOG_Properties(Classification);
				if (tempProperties.DefaultPackageFileExtension.Length == 0)
				{
					if (MOG_Prompt.PromptResponse("Warning - Missing extension", "This package does not have an extension.  Most eninges require extensions on packages.\n\nDo you want to continue without and extenstion?", "", MOG.PROMPT.MOGPromptButtons.YesNo, MOG.PROMPT.MOG_ALERT_LEVEL.ALERT) == MOG.PROMPT.MOGPromptResult.No)
					{
						return false;
					}
				}
			}

			if (this.PlatformCombo.BackColor == Color.Tomato)
			{
				MOG_Prompt.PromptResponse("Invalid Platform", "Please select a valid platform for the new package.");
				return false;
			}

			return true;
		}

		private bool IsTextBoxValid(TextBox textBox)
		{
			// Check if this textBox is enabled?
			if (textBox.Enabled)
			{
				// Check if we have any text?
				if (!String.IsNullOrEmpty(textBox.Text) &&
					!MOG_ControllerSystem.InvalidWindowsPathCharactersCheck(textBox.Text, true))
				{
					// Color us green
					textBox.BackColor = Color.PaleGreen;
					return true;
				}
				else
				{
					// Color us red
					textBox.BackColor = Color.Tomato;
				}
			}
			else
			{
				// Color us red
				textBox.BackColor = Color.LightGray;
			}

			return false;
		}

		private void UpdateSyncTarget()
		{
			MOG_Properties props = new MOG_Properties(ClassificationTextBox.Text);
			string seeds = "";
			seeds = MOG_Tokens.AppendTokenSeeds(seeds, MOG_Tokens.GetClassificationTokenSeeds(Classification));
			seeds = MOG_Tokens.AppendTokenSeeds(seeds, MOG_Tokens.GetPropertyTokenSeeds(props.GetPropertyList()));
			string resolvedSyncTargetPath = MOG_Tokens.GetFormattedString(props.SyncTargetPath, seeds);
			SyncTarget = resolvedSyncTargetPath;
		}

		private void UpdateSyncTargetTree()
		{
			// Attempt to drill to the specified path
			string relativeSyncTargetPath = DosUtils.PathMakeRelativePath(this.SyncTargetTreeView.MOGRootNode.FullPath, SyncTargetTextBox.Text);
			this.SyncTargetTreeView.MOGSelectedNode = MogUtil_ClassificationTrees.FindAndExpandTreeNodeFromFullPath(this.SyncTargetTreeView.MOGRootNode.Nodes, "\\", relativeSyncTargetPath);
		}

		private void EnableSyncTargetTree(bool bEnable)
		{
			if (bEnable)
			{
				// Enable the sync target controls
				SyncTargetTreeView.Enabled = true;
				SyncTargetTreeView.Visible = true;
				SyncTargetTextBox.Text = "";
				SyncTargetTextBox.Enabled = true;
				IsTextBoxValid(SyncTargetTextBox);
			}
			else
			{
				// disable the sync target controls
				SyncTargetTreeView.Enabled = false;
				SyncTargetTreeView.Visible = false;
				SyncTargetTextBox.Enabled = false;
				SyncTargetTextBox.Text = "n/a";
				SyncTargetTextBox.BackColor = Color.LightGray;
			}
		}

		private void SyncTargetTextBox_TextChanged(object sender, EventArgs e)
		{
			TextBox tb = sender as TextBox;
			if (tb != null)
			{
				// Check if we are enabled?
				if (tb.Enabled)
				{
					// Validate this text
					if (IsTextBoxValid(tb))
					{
						UpdateSyncTargetTree();
					}
				}
			}
		}

		private void PackageNameTextBox_TextChanged(object sender, EventArgs e)
		{
			TextBox tb = sender as TextBox;
			if (tb != null)
			{
				// Validate this text
				if (IsTextBoxValid(tb))
				{
				}
			}
		}

		private void UpdateClassificationTree()
		{
			// Attempt to drill to the specified path
			string classification = ClassificationTextBox.Text;

			// Check how this classifciation will impact the SyncTargetTree?
			MOG_Properties props = new MOG_Properties(Classification);
			if (props.IsPackagedAsset)
			{
				EnableSyncTargetTree(false);
			}
			else if (props.SyncFiles)
			{
				EnableSyncTargetTree(true);
			}
			else
			{
				EnableSyncTargetTree(false);
			}

			// Check if we have nothing selected?  or
			// Check if we have the wrong thing selected?
			if (this.ClassificationTreeView.SelectedNode == null ||
				string.Compare(this.ClassificationTreeView.SelectedNode.FullPath, classification, true) != 0)
			{
				// Locate and select the appropriate node in the classification tree
				this.ClassificationTreeView.SelectedNode = MogUtil_ClassificationTrees.FindAndExpandTreeNodeFromFullPath(this.ClassificationTreeView.Nodes, "~", classification);
			}
		}

		private void ClassificationTextBox_TextChanged(object sender, EventArgs e)
		{
			TextBox tb = sender as TextBox;
			if (tb != null)
			{
				// Validate this text
				if (IsTextBoxValid(tb))
				{
					// Make sure to update our associated tree
					UpdateClassificationTree();
					UpdateSyncTarget();
				}
			}
		}

		private void AddClassification_Click(object sender, EventArgs e)
		{
			MOG_Privileges privs = MOG_ControllerProject.GetPrivileges();
			if (privs.GetUserPrivilege(MOG_ControllerProject.GetUserName(), MOG_PRIVILEGE.AddClassification))
			{
				ToolStripItem item = sender as ToolStripItem;
				if (item != null)
				{
					ContextMenuStrip strip = item.Owner as ContextMenuStrip;
					if (strip != null)
					{
						MogControl_FullTreeView treeview = strip.SourceControl as MogControl_FullTreeView;
						if (treeview != null)
						{
							TreeNode node = treeview.SelectedNode;
							if (node != null)
							{
								ClassificationCreateForm form = new ClassificationCreateForm(node.FullPath);
								if (form.ShowDialog(treeview.TopLevelControl) == DialogResult.OK)
								{
									Classification = form.FullClassificationName;
									treeview.Initialize(DrillToClassification);
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

		private void AddFolder_Click(object sender, EventArgs e)
		{
			ToolStripItem item = sender as ToolStripItem;
			if (item != null)
			{
				ContextMenuStrip strip = item.Owner as ContextMenuStrip;
				if (strip != null)
				{
					TreeView treeview = strip.SourceControl as TreeView;
					if (treeview != null)
					{
						TreeNode node = treeview.SelectedNode;
						if (node != null)
						{
							node.Expand();
							TreeNode newNode = node.Nodes.Add("New Folder");
							newNode.ImageIndex = MogUtil_AssetIcons.GetClassIconIndex("folder");
							newNode.SelectedImageIndex = MogUtil_AssetIcons.GetClassIconIndex("folderactive");
							
							node.Expand();
							treeview.LabelEdit = true;
							newNode.BeginEdit();
						}
					}
				}
			}
		}

		private void SyncTargetTreeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
		{
			e.Node.TreeView.LabelEdit = false;
			e.Node.TreeView.SelectedNode = e.Node;

			SyncTargetTextBox.Text = e.Node.Parent.FullPath.Replace("~","\\") + "\\" + e.Label;
		}

		private void PlatformCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			SyncTargetTreeView.ReinitializeVirtual(Platform);
		}
	}
}