using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Threading;

using System.Text.RegularExpressions;
using System.IO;

using MOG;
using MOG.FILENAME;
using MOG.PROPERTIES;
using MOG.PROJECT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERSYNCDATA;
using MOG.CONTROLLER.CONTROLLERASSET;
using MOG.CONTROLLER.CONTROLLERREPOSITORY;
using MOG.CONTROLLER.CONTROLLERPACKAGE;
using MOG.TIME;
using MOG.DOSUTILS;
using MOG.REPORT;
using MOG.PROMPT;
using MOG.PLATFORM;
using MOG.DATABASE;

using MOG_ControlsLibrary.Utils;

namespace MOG_ControlsLibrary.Controls
{
	/// <summary>
	/// Summary description for MogControl_RepositoryTreeView.
	/// </summary>
	public class MogControl_ArchivalTreeView : MogControl_PropertyClassificationTreeView
	{
		public static readonly Color Archive_Color = Color.Red;
		private SortedList mArchivalList;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// By default, this initializes a RepositoryTreeView as a Classification_View
		/// </summary>
		public MogControl_ArchivalTreeView()
			: base()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			ArchivedNodeForeColor = Archive_Color;
		}

		public override void Initialize()
		{
			AddExtraIcons();

			ImageList = MogUtil_AssetIcons.Images;
			HotTracking = true;

			mMogPath = MOG_ControllerSystem.GetSystemRepositoryPath();

			Nodes.Clear();
			
			// Add and Expand the first node in the tree
			ExpandTreeDown();

			// Get our immediate Project Name
			string projectName = MOG_ControllerProject.GetProjectName();

			// If we have no global Project Name AND it is the same as our immediate AND we
			//	have a LastNodePath global...
			if (mProjectName != null && mProjectName == projectName && LastNodePath != null)
			{
				string lastSelectedNodePath = LastNodePath;
				
				if (mCurrentlyExpandedNodes != null)
				{
					foreach (string fullPath in mCurrentlyExpandedNodes)
					{
						TreeNode lastNode = DrillToNodePath(fullPath);
						if (lastNode != null)
						{
							lastNode.Expand();
						}
					}
				}

				// Select the last selected node (if we can find it)
				SelectedNode = DrillToNodePath(lastSelectedNodePath);

				// Set our scroll position back to what it was when the user clicked on their last node
				SetHScrollPosition(this, mSavedScrollPosition.X);
				SetVScrollPosition(this, mSavedScrollPosition.Y);
			}
			else
			{
				// Initialize our projectName, since it's either null or a different value
				mProjectName = projectName;
			}

			bIsInitialized = true;
		}

		private void InitializeArchivalList()
		{
			try
			{
				if (mArchivalList == null)
				{
					mArchivalList = new SortedList();
				}
				mArchivalList.Clear();

				// Get all our Assets
				ArrayList assets = MOG.DATABASE.MOG_DBAssetAPI.GetAllArchivedAssets();

				// Get all classifications (to start with, that is)
				ArrayList classifications = MOG_ControllerProject.GetProject().GetArchivedSubClassifications(mProjectName);

				// If we found classifications...
				if (classifications != null && classifications.Count > 0)
				{
					foreach (string classificationName in classifications)
					{
						// If we don't already have this classification, add it.
						if (!mArchivalList.Contains(classificationName))
						{
							ArrayList assetsList = new ArrayList();
							mArchivalList.Add(classificationName, assetsList);
						}
					}
				}

				// If we got assets
				if (assets != null && assets.Count > 0)
				{
					foreach (MOG_Filename package in assets)
					{
						// If we do not already have this classification...
						if (!mArchivalList.Contains(package.GetAssetClassification()))
						{
							// Add classification
							ArrayList assetsList = new ArrayList();
							assetsList.Add(package.GetEncodedFilename());
							mArchivalList.Add(package.GetAssetClassification(), assetsList);
						}
						// Else, we have this classification, so add this asset to it...
						else
						{
							ArrayList assetsList = (ArrayList)mArchivalList.GetByIndex(mArchivalList.IndexOfKey(package.GetAssetClassification()));
							assetsList.Add(package.GetEncodedFilename());
						}
					}
				}
			}
			catch (Exception e)
			{
				e.ToString();
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

		protected override void ExpandTreeDown()
		{
			ExpandTreeDown(null);
		}

		/// <summary>
		/// Expand this tree down as a type of our Classification TreeView
		/// </summary>
		/// <param name="parentNode"></param>
		protected override void ExpandTreeDown(TreeNode node)
		{
			try
			{
				// Change to mouse to hourglass
				Cursor = Cursors.WaitCursor;

				if (node != null)
				{
					if (node.Nodes.Count == 1 && node.Nodes[0].Text == Blank_Node_Text)
					{
						// this is the first time we are expanding this tree node
						// If we are down to the Asset/Package level...
						if (IsAtAssetLevel(node) && ExpandAssets)
						{
							// Expand our Asset/Package nodes
							base.ExpandTreeDown(node);
						}
						// Else expand according to RootTreeType
						else
						{
							// Clean out the temp(s)
							node.Nodes.Clear();
							ExpandArchivalTreeDown(node);
						}
					}
				}
				else
				{
					// We need to add a root node
					ArrayList classifications = MOG_ControllerProject.GetProject().GetSubClassifications("", MOG_ControllerProject.GetBranchName());

					// This will cause a Windows exception, if index is out of range...
					TreeNode rootNode;
					if (classifications != null && classifications.Count > 0)
					{
						foreach (string classification in classifications)
						{
							rootNode = new TreeNode(classification, new TreeNode[] { new TreeNode(Blank_Node_Text) });
							rootNode.Tag = new Mog_BaseTag(rootNode, rootNode.Text);
							((Mog_BaseTag)rootNode.Tag).PackageNodeType = PackageNodeTypes.Class;
							Nodes.Add(rootNode);
							rootNode.Name = rootNode.FullPath;
						}

						// Since we do have our first node now, go ahead and expand it
						foreach (TreeNode root in Nodes)
						{
							root.Expand();
						}
					}
					else
					{
						Enabled = false;
						Nodes.Add(NothingReturned_Text);
					}
				}
			}
			catch (Exception ex)
			{
				MOG_Report.ReportMessage("Error expanding Archive Tree", ex.Message, ex.StackTrace, MOG_ALERT_LEVEL.ERROR);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		public override void MakeAssetCurrent(MOG_Filename assetFilename)
		{
			// Call our parent's MakeAssetCurrent
			base.MakeAssetCurrent(assetFilename);

			// Make sure this assetFilename has the info we want
			if (assetFilename != null &&
				assetFilename.GetVersionTimeStamp().Length > 0)
			{
				TreeNode foundNode = FindNode(assetFilename.GetAssetFullName());
				if (foundNode != null)
				{
					// Check if this node was previously marked as a deleted version?
					if (foundNode.ForeColor == Archive_Color)
					{
						// Collapse this baby and let it get rebuilt the next time the user expands it because it needs to change it internal structure
						foundNode.Collapse();
						foundNode.Nodes.Clear();
						foundNode.Nodes.Add(Blank_Node_Text);
					}

					// Reset the color
					foundNode.ForeColor = Color.Black;

					// Update this parent node with the new information concerning this asset
					Mog_BaseTag assetTag = foundNode.Tag as Mog_BaseTag;
					if (assetTag != null)
					{
						// Create a dateFormat just like that used in standard MS Windows USA regional date settings
						string dateFormat = MOG_Tokens.GetMonth_1() + "/" + MOG_Tokens.GetDay_1() + "/" + MOG_Tokens.GetYear_4()
							+ " " + MOG_Tokens.GetHour_1() + ":" + MOG_Tokens.GetMinute_2() + " " + MOG_Tokens.GetAMPM();

						// Scan the children nodes looking for other places needing to be fixed up
						foreach (TreeNode node in foundNode.Nodes)
						{
							// Make sure this is a valid node?
							if (node != null)
							{
								// Checkif this is the 'All Revisions'?
								if (node.Text == Revisions_Text)
								{
									bool bFoundCurrentRevisionNode = false;

									// Fixup this list of revisions
									foreach (TreeNode revisionNode in node.Nodes)
									{
										Mog_BaseTag baseTag = revisionNode.Tag as Mog_BaseTag;
										if (baseTag != null)
										{
											MOG_Filename revisionFilename = new MOG_Filename(baseTag.FullFilename);
											if (revisionFilename.GetVersionTimeStamp() == assetFilename.GetVersionTimeStamp())
											{
												revisionNode.ForeColor = CurrentVersion_Color;
												bFoundCurrentRevisionNode = true;
											}
											else
											{
												revisionNode.ForeColor = Color.Black;
											}
										}
									}

									// Check if we need to add our new revision node?
									if (!bFoundCurrentRevisionNode)
									{
										// Looks like this is a new revision and needs to be added
										// Hey Whipple - What do I do here?
										// It seems like this is already added by an earlier event so I suspect we will never hit this.
									}
								}
								else
								{
									// Check if this is the 'Current <' node
									if (node.Text.StartsWith(Current_Text + " <"))
									{
										node.Text = Current_Text + " <" + assetFilename.GetVersionTimeStampString(dateFormat) + ">";
									}

									// Update it's tag
									Mog_BaseTag currentTag = node.Tag as Mog_BaseTag;
									if (currentTag != null)
									{
										assetTag.FullFilename = assetFilename.GetOriginalFilename();
									}

									// Finally collapse this baby and let it get rebuilt the next time the user expands it
									node.Collapse();
									node.Nodes.Clear();
									node.Nodes.Add(Blank_Node_Text);
								}
							}
						}
					}
				}
			}
		}

		private void ExpandArchivalTreeDown(TreeNode node)
		{
			// Create names for our lookup into the DB
			string parentClassification = node.FullPath;
			string branchName = MOG_ControllerProject.GetBranchName();

			// Make our node.Tag easy to use
			Mog_BaseTag parentTag = (Mog_BaseTag)node.Tag;

			// Get a list of the classifications for our regular views
			ArrayList classifications;
			// If we have attached classifications, use them
			if (parentTag.AttachedClassifications != null && parentTag.AttachedClassifications.Count > 0)
			{
				classifications = parentTag.AttachedClassifications;
			}
			else
			{
				classifications = MOG_ControllerProject.GetProject().GetSubClassifications(parentClassification, branchName);
			}
			// Get a list of archived classifications to compare our regular classifications to
			ArrayList archiveClassifications = MOG_ControllerProject.GetProject().GetArchivedSubClassifications(parentClassification);

			// If we have null classifcations, quit our function
			if (classifications == null || archiveClassifications == null)
			{
				return;
			}

			FillInClassifications(node, classifications, archiveClassifications);

			//Populate assets for this classification
			// If classificationNode is in a treeView (so we can use the FullPath property) AND we want Assets...
			if (node.TreeView != null && ExpandAssets)
			{
				FillInAssets(node);
			}
		}

		private void FillInAssets(TreeNode node)
		{
			// Store assets under this classifcation
			ArrayList assets = MOG_ControllerAsset.GetAssetsByClassification(node.FullPath);
			assets.Sort();

			bool assetsValid = (assets != null && assets.Count > 0);
			// If we actually have assets to list, and our archivedAssets do not differ from our branch-related assets...
			if (assetsValid)
			{
				// Foreach asset in this classification...
				foreach (MOG_Filename asset in assets)
				{
					TreeNode assetNode = CreateAssetNode(asset, node.ForeColor);
					node.Nodes.Add(assetNode);
				}
			}

			// Also, get our archived assets, if we are in archive view
			ArrayList archivedAssets = MOG_ControllerAsset.GetArchivedAssetsByClassification(node.FullPath);
			archivedAssets.Sort();

			// If we have valid archivedAssets, AND 
			//  (we EITHER have more archivedAssets than assets
			//  OR we have no valid assets)
			if (archivedAssets != null && archivedAssets.Count > 0
				&& ((assetsValid && assets.Count < archivedAssets.Count) || !assetsValid))
			{
				//TODO:  Possibly add a "Removed Assets" node here...
				// If we do have branch-related assets, check for them by name...
				List<string> branchAssetsList = new List<string>();
				if (assets != null && assets.Count > 0)
				{
					// Create a list of our Current assets
					foreach (MOG_Filename asset in assets)
					{
						branchAssetsList.Add(asset.GetAssetFullName().ToLower());
					}
				}

				// Fill in any Assets that are not current
				foreach (MOG_Filename archiveAsset in archivedAssets)
				{
					TreeNode assetNode = null;
					// If there is nothing in our branchAssetsList OR 
					//  branchAssetsList does not contain the current asset we're looking at...
					if (branchAssetsList.Count == 0 || !branchAssetsList.Contains(archiveAsset.GetAssetFullName().ToLower()))
					{
						assetNode = CreateAssetNode(archiveAsset, Archive_Color);
						node.Nodes.Add(assetNode);

						//Add this bad boy to the list so we don't add it more than once
						branchAssetsList.Add(archiveAsset.GetAssetFullName().ToLower());
					}
				}
			}
		}

		private void FillInClassifications(TreeNode node, ArrayList classifications, ArrayList archiveClassifications)
		{
			string parentClassification = node.FullPath;
			string branchName = MOG_ControllerProject.GetBranchName();
			Mog_BaseTag parentTag = (Mog_BaseTag)node.Tag;

			// Fill in our classifications, comparing to archive classifications
			foreach (string childTempClassification in archiveClassifications)
			{
				// Hold the childClassification name in a mutable variable
				string childClassification = parentClassification + PathSeparator + childTempClassification;
				TreeNode classificationNode = new TreeNode();

				// Add our classification node
				classificationNode.Text = childTempClassification;
				classificationNode.Tag = new Mog_BaseTag(classificationNode, childClassification);
				((Mog_BaseTag)classificationNode.Tag).PackageNodeType = PackageNodeTypes.Class;

				// If we are in the Archive View and this classification is not in our classifications,
				//	change our color to Archive_Color
				if (!classifications.Contains(childTempClassification))
				{
					classificationNode.ForeColor = Archive_Color;
				}

				node.Nodes.Add(classificationNode);

				classificationNode.Name = classificationNode.FullPath;
				SetImageIndices(classificationNode, GetClassificationImageIndex(classificationNode.FullPath));

				// JohnRen - Speed up trees (This is a lot of extra work just so we can know if there is anything inside)
				if (true)
				{
					// Add our node
					classificationNode.Nodes.Add(new TreeNode(Blank_Node_Text));
				}
			}
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// MogControl_RepositoryTreeView
			// 
		}
		#endregion

		#region Utility methods
		/// <summary>
		/// Used to get an Asset node with a good node.Tag for this TreeView or any inheriting classes.
		///  Does not use full filename.
		/// </summary>
		protected override TreeNode CreateAssetNode(MOG_Filename asset)
		{
			return CreateAssetNode(asset, Color.Black, false);
		}

		private TreeNode CreateAssetNode(MOG_Filename asset, System.Drawing.Color foreColor)
		{
			return CreateAssetNode(asset, foreColor, false);
		}

		/// <summary>
		/// Used to get an Asset node with a good node.Tag for this TreeView or any inheriting classes.
		/// </summary>
		private TreeNode CreateAssetNode(MOG_Filename asset, System.Drawing.Color foreColor, bool useFullFilename)
		{
			string assetName = asset.GetAssetFullName();
			MOG_Filename assetFullFilename;

			// If we found revisions, go ahead and select the first one as the full filename for our tag...
			assetFullFilename = MOG_ControllerRepository.GetAssetBlessedPath(asset);

			TreeNode assetNode = new TreeNode(asset.GetAssetName());

			if (useFullFilename)
			{
				assetNode.Text = asset.GetAssetFullName();
			}

			assetNode.Tag = new Mog_BaseTag(assetNode, assetFullFilename.GetEncodedFilename(), RepositoryFocusLevel.Repository, true);

			// If we are Archive_Color, remain displayed as such
			if (foreColor == Archive_Color)
				assetNode.ForeColor = Archive_Color;

			assetNode.Nodes.Add(new TreeNode(Blank_Node_Text));
			assetNode.Name = asset.GetAssetFullName();

			SetImageIndices(assetNode, GetAssetImageIndex(((Mog_BaseTag)assetNode.Tag).FullFilename));

			return assetNode;
		}
		#endregion Utility methods
	}
}