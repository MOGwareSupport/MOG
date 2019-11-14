using System;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading;
using System.Drawing;

using MOG;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERREPOSITORY;
using MOG.FILENAME;
using MOG.PROPERTIES;
using MOG.REPORT;
using MOG.DATABASE;

using MOG_ControlsLibrary.Utils;
using System.Collections.Generic;
//using MOG_Client;

namespace MOG_ControlsLibrary.Controls
{
	/// <summary>
	/// MogControl_PropertyClassificationTreeView will expand based on the ArrayList of 
	///  MOG_Property objects placed inside the MogPropertyList property (by using ArrayList::Add() ).
	/// You should place MOG_Property objects inside the ArrayList BEFORE initializing the class.
	///  That means that, for inheriting classes, simply add the MOG_Propertys inside the 
	///  class's constructor.
	/// </summary>
	public class MogControl_PropertyClassificationTreeView : MogControl_AssetTreeView
	{
		#region Global Variables and Properties

		protected ArrayList mPropertyList;

		protected SortedList mRequiredClassifications;
		protected SortedList mExludedClassifications;
		protected SortedList mRequiredAssets;

		/// <summary>
		/// Get the PropertyList in use by this tree. You may only insert MOG_Property objects.
		/// </summary>
		[Browsable(false)]
		public ArrayList MogPropertyList
		{
			get { return mPropertyList; }
		}

		#endregion Global Variables and Properties
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		//private System.ComponentModel.Container components = null;

		public MogControl_PropertyClassificationTreeView()
			: base()
		{
			mRequiredClassifications = new SortedList();
			mExludedClassifications = new SortedList();
			mRequiredAssets = new SortedList();
			mPropertyList = new ArrayList();
		}

		/// <summary>
		/// Do all the work for Initialization.
		/// </summary>
		public override void Initialize()
		{
			AddExtraIcons();

			ImageList = MogUtil_AssetIcons.Images;

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
				
				//Go through and expand all the nodes that used to be expanded
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

		protected override void ExpandTreeDown()
		{
			UseWaitCursor = true;

			bool bCreateRootProjectNode = false;

			if (mRequiredClassifications.Count > 0)
			{
				bCreateRootProjectNode = true;
			}
			else
			{
				// Fixes a crash at startup when the project is totally empty
				if (MogPropertyList.Count > 0)
				{
					// Check if the specified property is the Inclusion property
					MOG_Property testProperty = MogPropertyList[0] as MOG_Property;
					if (testProperty != null)
					{
						// Check if this is the 'FilterInclusion' property?
						if (string.Compare("FilterInclusions", testProperty.mPropertyKey, true) == 0)
						{
							// Show the normal tree anytime we had an inclusion filter yet nothing was found
							bCreateRootProjectNode = true;
						}
					}
				}
			}

			// Check if we decided to create the root project node?
			if (bCreateRootProjectNode)
			{
				Enabled = true;
				string adamNodeName = MOG_ControllerProject.GetProjectName();
				TreeNode rootNode = new TreeNode(adamNodeName, new TreeNode[] { new TreeNode(Blank_Node_Text) });
				rootNode.Checked = NodesDefaultToChecked;
				rootNode.ImageIndex = MogUtil_AssetIcons.GetClassIconIndex(adamNodeName);
				rootNode.SelectedImageIndex = rootNode.ImageIndex;
				rootNode.Tag = new Mog_BaseTag(rootNode, rootNode.Text);
				rootNode.Name = rootNode.Text;
				((Mog_BaseTag)rootNode.Tag).PackageNodeType = PackageNodeTypes.Class;
				Nodes.Add(rootNode);

				rootNode.Expand();
			}
			else
			{
				Enabled = false;
				Nodes.Add(NothingReturned_Text);
			}

			UseWaitCursor = false;
		}

		protected override void ExpandTreeDown(TreeNode node)
		{
			Cursor = Cursors.WaitCursor;

			// Check if this is the first time we are expanding this tree node
			if (node != null && node.Nodes.Count == 1 && node.Nodes[0].Text == Blank_Node_Text)
			{
				// Clean out the temp(s)
				node.Nodes.Clear();

				// If we are down to the Asset/Package level and we are allowed to expand Assets...
				if (IsAtAssetLevel(node) && (ExpandAssets || ExpandPackageGroups))
				{
					// Expand our Asset/Package nodes
					base.ExpandTreeDown(node);
				}
				else
				{
					// If we have a valid project (mostly for the designer purposes...)
					if (MOG_ControllerProject.GetProject() != null)
					{
						// Get the children of the node we are expanding
						ExpandPropertyTreeDown(node);
					}
				}
			}

			Cursor = Cursors.Default;
		}

		/// <summary>
		/// Shows Assets based on MOG_Property(s) assigned to PropertyList
		/// </summary>
		private void ExpandPropertyTreeDown(TreeNode node)
		{
			BeginUpdate();

			List<string> classificationsToAdd = GetSubClassifications(node);
			List<string> assetsToAdd = GetAssets(node);

			// Sort our classifications alphabetically
			classificationsToAdd.Sort();

			// Foreach classification, add it
			foreach (string classification in classificationsToAdd)
			{
				TreeNode classificationNode = new TreeNode(classification);

				// Assign the default node checked state			
				classificationNode.Checked = node.Checked;

				classificationNode.Tag = new Mog_BaseTag(classificationNode, classification, RepositoryFocusLevel.Classification, false);
				((Mog_BaseTag)classificationNode.Tag).PackageNodeType = PackageNodeTypes.Class;
				node.Nodes.Add(classificationNode);
				
				classificationNode.Name = classificationNode.FullPath;
				SetImageIndices(classificationNode, GetClassificationImageIndex(classificationNode.FullPath));

				classificationNode.Nodes.Add(new TreeNode(Blank_Node_Text));
			}

			// Use the System.IComparable interface for `string` to sort our list
			assetsToAdd.Sort();

			// Foreach Asset, add it
			foreach (string assetFilename in assetsToAdd)
			{
				MOG_Filename assetFile = new MOG_Filename(assetFilename);
				assetFile = MOG_ControllerProject.GetAssetCurrentBlessedPath(assetFile);
				TreeNode assetNode;
				// If we are expandingAssets or expandingPackageGroups, we need to be able to expand down
				if (ExpandAssets || ExpandPackageGroups)
				{
					assetNode = new TreeNode(assetFile.GetAssetName(), new TreeNode[] { new TreeNode(Blank_Node_Text) });
				}
				// Else, do not expand down.  Showing the Asset is all we need to do
				else
				{
					assetNode = new TreeNode(assetFile.GetAssetName());
				}

				assetNode.Tag = new Mog_BaseTag(assetNode, assetFile.GetEncodedFilename(), FocusForAssetNodes, true);
				((Mog_BaseTag)assetNode.Tag).PackageNodeType = PackageNodeTypes.Asset;
				((Mog_BaseTag)assetNode.Tag).PackageFullName = assetFile.GetAssetFullName();
				node.Nodes.Add(assetNode);
				assetNode.Name = assetFile.GetAssetFullName();

				SetImageIndices(assetNode, GetAssetFileImageIndex(assetFile.GetEncodedFilename()));
			}

			EndUpdate();
		}

		public bool HasSubclassifications(TreeNode node)
		{
			List<string> subclassifications = GetSubClassifications(node);
			if (subclassifications != null && subclassifications.Count > 0)
			{
				return true;
			}

			return false;
		}

		public bool HasAssets(TreeNode node)
		{
			List<string> assets = GetAssets(node);
			if (assets != null && 
				assets.Count > 0)
			{
				return true;
			}

			return false;
		}

		private bool ShowEntireClassificationContents(string classification)
		{
			bool bShowEntireTree = true;

			// Check if we have any properties associated with this tree?
			if (MogPropertyList.Count > 0)
			{
				// Check if we have no mRequiredClassifications?
				if (mRequiredClassifications.Count > 0)
				{
					int bestMatchingRequiredClassificationLength = 0;

					// Check if this classification is a child of a required classification?
					foreach (DictionaryEntry requiredClassification in mRequiredClassifications)
					{
						// Is this classification a child of this required classification?
						if (MOG_Filename.IsParentClassificationString(classification, requiredClassification.Key.ToString()))
						{
							// Check if this requiredClassification is a better match?
							if (requiredClassification.Key.ToString().Length > bestMatchingRequiredClassificationLength)
							{
								// Start tracking this as our best match thus far
								bestMatchingRequiredClassificationLength = requiredClassification.Key.ToString().Length;
							}
						}
					}

					// Check if we think we found a best match?
					if (bestMatchingRequiredClassificationLength != 0)
					{
						// Check if this classification is a child of a an excluded classification?
						foreach (DictionaryEntry excludedClassification in mExludedClassifications)
						{
							// Is this classification a child of this excluded classification?
							if (MOG_Filename.IsParentClassificationString(classification, excludedClassification.Key.ToString()))
							{
								// Check if this trumps our best match?
								if (excludedClassification.Key.ToString().Length > bestMatchingRequiredClassificationLength)
								{
									// Looks like the nays take it
									bestMatchingRequiredClassificationLength = 0;
									break;
								}
							}
						}
					}

					// Check if we still have a best possitive match?
					if (bestMatchingRequiredClassificationLength == 0)
					{
						bShowEntireTree = false;
					}
				}
			}

			return bShowEntireTree;
		}

		public List<string> GetSubClassifications(TreeNode node)
		{
			ArrayList classificationList = new ArrayList();
			string nodeClassification = node.FullPath;

			// Check if this is a classification we want to display everything in?
			if (ShowEntireClassificationContents(nodeClassification))
			{
				// Get all the children of this node's classification
				ArrayList childClassifications = MOG_DBAssetAPI.GetClassificationChildren(nodeClassification);
				if (childClassifications != null)
				{
					classificationList.AddRange(childClassifications);
				}
				// Make sure we exclude any items that have explicitly undefined the property
				classificationList = ExcludeClassifications(classificationList, nodeClassification);
			}
			else
			{
				// Get the classification children based on the required assets and required classifications
				classificationList = GetRequiredClassificationChildren(nodeClassification);
// I was trying to get required classifications to still be excluded if they had a matching exclusion but...
// This cause the required classifications to be excluded even when they has a child classification that required them to be visible.
// Removed it until later...We really need to know what is a real required classification versus a parent the got included due to a required child
//				// Make sure we exclude any classifications even if they are included as a required classification
//				classificationList = ExcludeClassifications(classificationList, nodeClassification);
			}

			// Transfer the classificationList over to a string array
			List<string> returnClassifications = new List<string>();
			foreach (string classification in classificationList)
			{
				returnClassifications.Add(classification);
			}

			return returnClassifications;
		}

		private ArrayList ExcludeClassifications(ArrayList classificationList, string nodeClassification)
		{
			ArrayList excludedClassificationList = new ArrayList();

			// Check if we have any properties associated with this tree?
			if (MogPropertyList.Count > 0)
			{
				// Get the list of excluded classifications for this node classification
				ArrayList excludedClassifications = MOG_DBAssetAPI.GetClassificationChildren(nodeClassification, "", MogPropertyList, true);
				if (excludedClassifications != null)
				{
					excludedClassificationList.AddRange(excludedClassifications);
				}

				// Not sure if this really is the best place for this logic but exclusions are hooked to the hip of inclusions...
				// Check if the specified property is the Inclusion property
				MOG_Property testProperty = MogPropertyList[0] as MOG_Property;
				if (testProperty != null)
				{
					// Check if this is the 'FilterInclusion' property?
					if (string.Compare("FilterInclusions", testProperty.mPropertyKey, true) == 0)
					{
						// Obtain the list of assets this filter is specifically excluded from
						ArrayList exclusionPropertyList = new ArrayList();
						exclusionPropertyList.Add(MOG.MOG_PropertyFactory.MOG_Classification_InfoProperties.New_FilterExclusions(testProperty.mPropertyValue));
						// Get the list of excluded classifications for this node classification
						ArrayList excludedCLassifications = MOG_DBAssetAPI.GetClassificationChildren(nodeClassification, "", exclusionPropertyList);
						if (excludedCLassifications != null)
						{
							excludedClassificationList.AddRange(excludedCLassifications);
						}
					}
				}
			}

			// Check for any explicitly specified exclusions?
			if (ExclusionList.Length > 0)
			{
				foreach (string classification in classificationList)
				{
					string testClassification = MOG_Filename.JoinClassificationString(nodeClassification, classification);
					if (StringUtils.IsFiltered(testClassification, ExclusionList))
					{
						// Remove this asset from the list
						excludedClassificationList.Add(classification);
					}
				}
			}

			// Remove the identified classifications to be excluded
			foreach (string excludedClassification in excludedClassificationList)
			{
				// Check if this excludedclassification is listed in our list?
				foreach (string classification in classificationList)
				{
					if (string.Compare(classification, excludedClassification, true) == 0)
					{
						// Remove this asset from the list
						classificationList.Remove(classification);
						break;
					}
				}
			}

			return classificationList;
		}

		private ArrayList GetRequiredClassificationChildren(string classification)
		{
			ArrayList childClassificationList = new ArrayList();

			// Scan our list of required classifications looking for any immediate children that should be included?
			foreach (DictionaryEntry requiredClassification in mRequiredClassifications)
			{
				// Parse for the immediate child of this classification
				string childClassification = ParseForChildClassification(classification, requiredClassification.Key.ToString());
				if (childClassification.Length > 0)
				{
					// Make sure we don't already have this child?
					if (!childClassificationList.Contains(childClassification))
					{
						// Add this child classification
						childClassificationList.Add(childClassification);
					}
				}
			}

			// Scan our list of required assets looking for any immediate children that should be included?
			foreach (DictionaryEntry requiredAsset in mRequiredAssets)
			{
				// Obtain the classification from the asset
				MOG_Filename requiredAssetFilename = new MOG_Filename(requiredAsset.Key.ToString());
				string requiredClassification = requiredAssetFilename.GetAssetClassification();

				// Parse for the immediate child of this classification
				string childClassification = ParseForChildClassification(classification, requiredClassification);
				if (childClassification.Length > 0)
				{
					// Make sure we don't already have this child?
					if (!childClassificationList.Contains(childClassification))
					{
						// Add this child classification
						childClassificationList.Add(childClassification);
					}
				}
			}

			return childClassificationList;
		}

		private string ParseForChildClassification(string rootClassification, string childClassification)
		{
			string child = "";

			// Check if classification is a parent for requiredClassification?
			if (MOG_Filename.IsParentClassificationString(childClassification, rootClassification))
			{
				// Break up the classifications into their parts
				string[] rootClassificationParts = MOG_Filename.SplitClassificationString(rootClassification);
				string[] childClassificationParts = MOG_Filename.SplitClassificationString(childClassification);

				// Make sure there is an immediate child classification we can extract?
				if (childClassificationParts.Length > rootClassificationParts.Length)
				{
					// Extract the next immediate child classification of childClassificationParts
					child = childClassificationParts[rootClassificationParts.Length];
				}
			}

			return child;
		}
		
		public List<string> GetAssets(TreeNode node)
		{
			ArrayList assetList = new ArrayList();
			string nodeClassification = node.FullPath;

			// Check if we should show asset-level nodes?
			if (ShowAssets || ExpandPackageGroups)
			{
				// Check if this is a classification we want to display everything in?
				if (ShowEntireClassificationContents(nodeClassification))
				{
					// Get the list of assets
					assetList = MOG_DBAssetAPI.GetAllAssetsByClassification(nodeClassification);
				}
				else
				{
					assetList = GetRequiredAssets(nodeClassification);
				}
			}

			// Exclude assets from our list that have undefined the property
			assetList = ExcludeAssets(assetList, nodeClassification);

			// Prepare our list of assets for return
			List<string> returnAssets = new List<string>();
			foreach (MOG_Filename asset in assetList)
			{
				// Check if we want to exclude platform-specific asset?
				if (!ShowPlatformSpecific)
				{
					MOG_Filename filename = new MOG_Filename(asset);
					if (filename.IsPlatformSpecific())
					{
						continue;
					}
				}

				returnAssets.Add(asset.GetAssetFullName());
			}

			return returnAssets;
		}

		private ArrayList ExcludeAssets(ArrayList assetList, string classification)
		{
			// Check if we have any properties associated with this tree?
			if (MogPropertyList.Count > 0)
			{
				// Get the list of excluded assets for this node classification
				ArrayList excludedAssetList = MOG_DBAssetAPI.GetAllAssetsByProperty(classification, (MOG_Property)MogPropertyList[0], true);
				foreach (MOG_Filename excludedAsset in excludedAssetList)
				{
					// Check if this excludedAsset is listed?
					foreach (MOG_Filename asset in assetList)
					{
						if (string.Compare(asset.GetAssetFullName(), excludedAsset.GetAssetFullName(), true) == 0)
						{
							// Remove this asset from the list
							assetList.Remove(asset);
							break;
						}
					}
				}
			}

			return assetList;
		}

		private ArrayList GetRequiredAssets(string classification)
		{
			ArrayList requiredAssetList = new ArrayList();

			// Break up classification into its parts
			string[] classificationParts = MOG_Filename.SplitClassificationString(classification);

			// Scan our list of required assets looking for any immediate children that should be included?
			foreach (DictionaryEntry requiredAsset in mRequiredAssets)
			{
				MOG_Filename requiredAssetFilename = new MOG_Filename(requiredAsset.Key.ToString());
				string requiredClassification = requiredAssetFilename.GetAssetClassification();

				// Check if classification is a parent for requiredClassification?
				if (MOG_Filename.IsParentClassificationString(requiredClassification, classification))
				{
					// Break up classification into its parts
					string[] requiredClassificationParts = MOG_Filename.SplitClassificationString(requiredClassification);
					// Make sure there is an immediate child classification we can extract?
					if (requiredClassificationParts.Length == classificationParts.Length)
					{
						requiredAssetList.Add(requiredAssetFilename);
					}
				}
			}

			return requiredAssetList;
		}

		protected virtual void InitializeClassificationsList()
		{
			InitializeClassificationsList(false);
		}
		
		protected virtual void InitializeClassificationsList(bool importableOnly)
		{
			ArrayList requiredClassificationList = new ArrayList();
//			ArrayList excludedClassificationList = null;
			ArrayList requiredAssetList = null;

			// If we have no MOG_Property(s), populate as a full TreeView
			if (MogPropertyList.Count > 0)
			{
				// Get the list of required classifications
				requiredClassificationList = MOG_DBAssetAPI.GetAllActiveClassifications((MOG_Property)MogPropertyList[0]);

//				// Get the list of excluded classifications for this node classification
//				excludedClassificationList = MOG_DBAssetAPI.GetAllActiveClassifications((MOG_Property)MogPropertyList[0], true);

				// Check if we should show asset-level nodes?
				if (ShowAssets || ExpandPackageGroups)
				{
					// Get the list of required assets
					requiredAssetList = MOG_DBAssetAPI.GetAllAssetsByProperty((MOG_Property)MogPropertyList[0]);
				}
			}
			else
			{
				ArrayList childClassifications = MOG_DBAssetAPI.GetClassificationChildren(MOG_ControllerProject.GetProjectName());
				if (childClassifications != null)
				{
					requiredClassificationList.AddRange(childClassifications);
				}
			}

			// Process our list of required classifications
			mRequiredClassifications.Clear();
			if (requiredClassificationList != null && 
				requiredClassificationList.Count > 0)
			{
				foreach (string classificationName in requiredClassificationList)
				{
					// Do we already have this class?
					if (!mRequiredClassifications.Contains(classificationName))
					{
						bool excluded = false;
						if (ExclusionList.Length > 0)
						{
							excluded = StringUtils.IsFiltered(classificationName, ExclusionList);
						}
						
						// Is it excluded?
						if (excluded == false)
						{
							if (!importableOnly || !MOG_Filename.IsLibraryClassification(classificationName))
							{
								//we don't have this classification yet, so add it and give it a container for assets
								List<string> assetList = new List<string>();
								mRequiredClassifications.Add(classificationName, assetList);
							}
						}
					}
				}
			}

			// Process our list of excluded classifications
			if (false)
			{
				// We need to perform a query and get all the classifications that contain a specific exclusion
				// Then examine each one and add any that are applicable to excluding the file in question to the mExcludedClassifications list
				// This was not implemented because of timeing and the need to obtain the file in question
			}

			// Process our list of required assets
			if (requiredAssetList != null && requiredAssetList.Count > 0)
			{
			    foreach (MOG_Filename requiredAsset in requiredAssetList)
			    {
			        if (!importableOnly ||
						!MOG_Filename.IsLibraryClassification(requiredAsset.GetAssetClassification()))
			        {
						// Check if this asset is excluded?
	                    bool excluded = false;
	                    if (ExclusionList.Length > 0)
	                    {
	                        excluded = StringUtils.IsFiltered(requiredAsset.GetAssetFullName(), ExclusionList);
	                    }

	                    // Make sure we are not excluded?
	                    if (!excluded)
	                    {
							mRequiredAssets[requiredAsset.GetAssetFullName()] = requiredAsset;
	                    }
			        }
			    }
			}
		}

		/// <summary>
		/// Used to get an Asset node with a good node.Tag for this TreeView or any inheriting classes.
		///  Does not use full filename.
		/// </summary>
		protected override TreeNode CreateAssetNode(MOG_Filename asset)
		{
			TreeNode assetNode = base.CreateAssetNode(asset);

			// If we are expandingAssets or expandingPackageGroups, we need to be able to expand down
			if (ExpandAssets || ExpandPackageGroups)
			{
				// Add the dummy node so the usere will be given the opportunity to expand it
				assetNode.Nodes.Add(new TreeNode(Blank_Node_Text));
			}

			// Rebuild the node's tag
			MOG_Filename assetFile = MOG_ControllerProject.GetAssetCurrentBlessedPath(asset);
			Mog_BaseTag tag = new Mog_BaseTag(assetNode, assetFile.GetEncodedFilename(), FocusForAssetNodes, true);
			tag.PackageNodeType = PackageNodeTypes.Asset;
			tag.PackageFullName = assetFile.GetAssetFullName();
			assetNode.Tag = tag;

			return assetNode;
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion

	}
}