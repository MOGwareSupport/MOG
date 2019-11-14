using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Collections;
using System.IO;

using MOG;
using MOG.FILENAME;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERREPOSITORY;
using MOG.CONTROLLER.CONTROLLERPACKAGE;
using MOG.CONTROLLER.CONTROLLERASSET;
using MOG.PROPERTIES;
using MOG.DOSUTILS;
using MOG.REPORT;
using MOG.PROMPT;

using MOG_ControlsLibrary.Utils;

namespace MOG_ControlsLibrary.Controls
{
	/// <summary>
	/// Implements all the functionality we need to display TreeNodes once they get down to the Asset-level
	/// </summary>
	public abstract class MogControl_AssetTreeView : MogControl_BaseTreeView
	{
		#region Globals
		private Color mArchivedNodeForeColor;
		public Color ArchivedNodeForeColor
		{
			get
			{
				if( mArchivedNodeForeColor.IsEmpty)
				{
					return this.ForeColor;
				}
				return mArchivedNodeForeColor;
			}
			set
			{
				this.mArchivedNodeForeColor = value;
			}
		}
		// Global for deciding whether or not we should expand our assets (default is true)
		private bool bExpandAssets;
		/// <summary>
		/// Allows designer to decide whether or not we should show Assets within a RepositoryTreeView
		/// </summary>
		[Category( "Appearance" ), 
		Description( "If TRUE, allows user to expand into Assets, including revisions, contents, etc..."),]
		public bool ExpandAssets
		{
			get
			{
				return this.bExpandAssets;
			}
			set
			{
				this.bExpandAssets = value;
			}
		}
		
		private bool bNodesDefaultToChecked;
		/// <summary>
		/// If true, all nodes will start with their checked state set to 'checked'
		/// </summary>
		[Category("Appearance"), Description("If true, nodes will default to checked.")]
		public bool NodesDefaultToChecked
		{
			get
			{
				return this.bNodesDefaultToChecked;
			}
			set
			{
				this.bNodesDefaultToChecked = value;
			}
		}

		private string mExclusionList = "";
		/// <summary>
		/// If true, all nodes will start with their checked state set to 'checked'
		/// </summary>
		[Category("Appearance"), Description("List of strings to exclude if a node's text matches any string in the exclusion list.")]
		public string ExclusionList
		{
			get
			{
				return this.mExclusionList;
			}
			set
			{
				this.mExclusionList = value;
			}
		}

		private bool bExpandPackageGroupAssets;
		[Category( "Appearance" ),
		Description( "If TRUE, allows user to see Assets node under each Package Group node that has Assets "
			+ "associated with it." ) ]
		public bool ExpandPackageGroupAssets
		{
			get
			{
				return this.bExpandPackageGroupAssets;
			}
			set
			{
				this.bExpandPackageGroupAssets = value;
			}
		}

		private bool bExpandPackageGroups;
		[Category( "Appearance" ),
		Description( "If TRUE, allows user to see packages and expand into groups.")]
		public bool ExpandPackageGroups
		{
			get
			{
				return this.bExpandPackageGroups;
			}
			set
			{
				this.bExpandPackageGroups = value;
			}
		}

		private bool bShowAssets;
		/// <summary>
		/// If true, Assets will be shown, false if user does not need to see Assets
		/// </summary>
		[Category( "Appearance" ), Description( "If true, Assets will be shown to the user.") ]
		public bool ShowAssets
		{
			get
			{
				return this.bShowAssets;
			}
			set
			{
				this.bShowAssets = value;
			}
		}

		private bool mbShowPlatformSpecific;
		/// <summary>
		/// If true, Assets will be shown, false if user does not need to see Assets
		/// </summary>
		[Category("Appearance"), Description("If true, Platform-specific assets will be shown to the user.")]
		public bool ShowPlatformSpecific
		{
			get
			{
				return mbShowPlatformSpecific;
			}
			set
			{
				mbShowPlatformSpecific = value;
			}
		}
		
		private LeafFocusLevel mFocusForAssetNodes = LeafFocusLevel.RepositoryItems;

		[Category( "Appearance" ), Description( "Enumeration to indicate how this TreeView should expand Asset-level nodes" )]
		public LeafFocusLevel FocusForAssetNodes
		{
			get
			{
				return mFocusForAssetNodes;
			}
			set
			{
				this.mFocusForAssetNodes = value;
			}
		}
		#endregion Globals

		#region SubClass Global Variables
		private PopulatePackageClass mPopulatePackage;
		#endregion SubClass Global Variables

		public MogControl_AssetTreeView() : base()
		{
			this.mPopulatePackage = new PopulatePackageClass(this);
		}

		protected override void ExpandTreeDown()
		{
			this.ExpandTreeDown( null );
		}

		protected override void ExpandTreeDown(TreeNode node)
		{
			// Decide which kind of RepositoryFocusLevel we have and expand accordingly
			switch( ((Mog_BaseTag)node.Tag).LeafFocus )
			{
				case LeafFocusLevel.Version:
					// Show current version and revisions nodes
					PopulateVersionNodes( node );
					break;
				case LeafFocusLevel.RepositoryItems:
					// Show Files and node for Contained Items (either Packages or Contained Assets)
					PopulateRepositoryItemNodes( node );
					break;
				case LeafFocusLevel.ContainedItems:
					// Show the Packages or Contained Assets from above
					PopulateContainedItemNodes( node );
					break;
				case LeafFocusLevel.PackageGroup:
					PopulatePackageGroupNodes( node );
					break;
			}
		}

		public override void Initialize()
		{
			AddExtraIcons();

			bIsInitialized = true;
		}

		#region Version level expansion
		/// <summary>
		/// Populate ONLY the first set of nodes under the asset, including Current Version, 
		///		if applicable, and All Revisions.  Places a blank node (for further expansion)
		///		under each one of these nodes with LeafFocusLevel of RepositoryItems
		///		
		///	NOTE:  This seems to be very stable.  Any changes should be carefully made.
		/// </summary>
		/// <param name="tree"></param>
		/// <param name="node">Fully validated repository item node</param>
		/// <returns></returns>
		private void PopulateVersionNodes( TreeNode node )
		{
			// Create a dateFormat just like that used in standard MS Windows USA regional date settings
			string dateFormat = MOG_Tokens.GetMonth_1() + "/" + MOG_Tokens.GetDay_1() + "/" + MOG_Tokens.GetYear_4()
				+ " " + MOG_Tokens.GetHour_1() + ":" + MOG_Tokens.GetMinute_2()	+ " " + MOG_Tokens.GetAMPM();

			// Derive a repositoryFile from node.FullFilename
			MOG_Filename repositoryFile = new MOG_Filename( ((Mog_BaseTag)node.Tag).FullFilename );
			// Get our blessedAssetFile
			MOG_Filename blessedAssetFile =  new MOG_Filename( MOG_ControllerProject.GetAssetCurrentBlessedPath( repositoryFile ) );
			// Get a timeStamp to see when we are looking at the current asset (below)
			string currentTimeStamp = MOG_ControllerProject.GetAssetCurrentVersionTimeStamp( repositoryFile );

			// Get all the revisions of this asset
			ArrayList revisions = MOG_ControllerRepository.GetBlessedRevisions( repositoryFile, true, false );
			// Have a currentVersionNode to attach to the passed-in TreeNode, node
			TreeNode currentVersionNode = null;
			// For Revisions node, have a TreeNode[] to put all those nodes into
			TreeNode[] versionNodes = new TreeNode[revisions.Count];
			
			// Foreach revision, create a versionNode for it and attach it to versionNodes[i]
			for( int i = 0; i < versionNodes.Length; ++i)
			{
				// Pull up the exact versionFile we are using
				MOG_Filename versionFile = new MOG_Filename( blessedAssetFile.GetEncodedFilename() + "\\r." + revisions[i]);
				// Create our TreeNode.Text for this label
				string textLabel = "<" + versionFile.GetVersionTimeStampString(dateFormat) + ">";
				// Create our versionNode to attach to versionNodes[i]
				TreeNode versionNode = new TreeNode( textLabel );

				// Assign the default node checked state
				versionNode.Checked = NodesDefaultToChecked;

				versionNode.ForeColor = node.ForeColor;
				// Attach a Mog_BaseTag that will allow for further expansion down in BaseLeafTreeView
				versionNode.Tag = new Mog_BaseTag( versionNode, versionFile.GetEncodedFilename(), LeafFocusLevel.RepositoryItems, true );
				// Add a blank node for further expansion
				versionNode.Nodes.Add( new TreeNode( Blank_Node_Text ) );
				// Save versionNode out to our TreeNode[]
				versionNodes[i] = versionNode;
				
				// If we have our current repository item...
				if( versionFile.GetVersionTimeStamp() == currentTimeStamp )
				{
					// Do a bitwise (object.Clone()) of our TreeNode
					currentVersionNode = (TreeNode)versionNode.Clone();
					// Label this node as Current + date
					currentVersionNode.Text = Current_Text + " " + versionNode.Text;
					// Attach a tag for further expansion
					currentVersionNode.Tag = new Mog_BaseTag( versionNode, versionFile.GetEncodedFilename(), 
						LeafFocusLevel.RepositoryItems, true );
					// Display this TreeNode.Text as blue
					versionNode.ForeColor = System.Drawing.Color.Blue;
					// Add this directly to the node that was passed in to us
					node.Nodes.Add( currentVersionNode );
				}
			}

			// If we found a current version node, indicate it
			if( currentVersionNode != null )
			{
				// Create a Revision node in which to place all the revisions we found above as children
				TreeNode revisionNode = new TreeNode( Revisions_Text, versionNodes );

				// Assign the default node checked state
				revisionNode.Checked = NodesDefaultToChecked;

				revisionNode.Tag = new Mog_BaseTag( revisionNode, blessedAssetFile.GetEncodedFilename(), 
					LeafFocusLevel.RepositoryItems, true );
				// Add Revision node to node that was passed in
				node.Nodes.Add( revisionNode );
			}
				// Else, we found no version node, so indicate that by showing all versions at root of assetNode
			else
			{
				node.Nodes.AddRange( versionNodes );
			}
		}
		#endregion Version level expansion

		#region RepositoryItem level expansion
		/// <summary>
		/// Populate sub nodes for our Asset-level node
		/// </summary>
		/// <param name="tree"></param>
		/// <param name="node"></param>
		private void PopulateRepositoryItemNodes( TreeNode node )
		{
			// Get a valid MOG_Filename for this node
			MOG_Filename repositoryFile = new MOG_Filename( GetBlessedVersionPath( ((Mog_BaseTag)node.Tag).FullFilename ));

			// Indicates if we will include the ContainedItems (Packages or Contained Assets) with our item
			bool includeContainedItems = true;

			MOG_Properties pProperties = new MOG_Properties( repositoryFile );
			// Fill in our contained items as necessary
			TreeNode containedItemsNode = null;
			// If this is a package, fill in the contained items node
			if( pProperties.IsPackage )
			{
				// Make sure we retain the fact that we now know we are a package
				((Mog_BaseTag)node.Tag).PackageNodeType = PackageNodeTypes.Package;

				// Construct our interior node
				containedItemsNode = new TreeNode( Contained_Assets_Text, new TreeNode[]{new TreeNode(Blank_Node_Text) } );
				// Assign the default node checked state
				containedItemsNode.Checked = NodesDefaultToChecked;
				//PopulateRepository_PopulatePackageGroupNodes( repositoryFile, containedItemsNode );
				containedItemsNode.Tag = new Mog_BaseTag( containedItemsNode, ((Mog_BaseTag)node.Tag).FullFilename,
					LeafFocusLevel.PackageGroup, false );
			}
				// Else, find out if we have a packaged asset...
			else
			{
				includeContainedItems = false;
			}

			PopulateFileNodes( node );

			// Add our nodes
			if( includeContainedItems )
				node.Nodes.Add( containedItemsNode );

			PopulateRelations( node, pProperties );
		}

		/// <summary>
		/// Given node with a valid Mog_BaseTag and MOG_Properties created from the Asset we're 
		///  getting Relationships for, populates a Relationships node with all relationships and adds
		///  it to `node`.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="properties"></param>
		private void PopulateRelations( TreeNode node, MOG_Properties properties )
		{
			const string Relationships_Text = "Relationships";

			//Get the array of MOG_Property objects that for our relationships
			ArrayList relationships = properties.GetPropertyList(Relationships_Text);
			if( relationships != null && relationships.Count > 0 )
			{
				// Generate a SortedList of sections that will have values attached in ArrayLists
				SortedList sections = new SortedList();
				foreach( MOG_Property property in relationships )
				{
					if( !sections.ContainsKey( property.mPropertySection ) )
					{
						ArrayList values = new ArrayList();
						values.Add( property.mPropertyKey );
						sections.Add( property.mPropertySection, values );
					}
					else
					{
						ArrayList values = (ArrayList)sections.GetByIndex( sections.IndexOfKey( property.mPropertySection ) );
						values.Add( property.mPropertyKey );
					}
				}

				// Store our fullFilename, add our Relationships node, and add its tag
				string fullFilename = GetBlessedVersionPath( ((Mog_BaseTag)node.Tag).FullFilename );
				TreeNode relationsNode = node.Nodes.Add( Relationships_Text);
				if(node.ForeColor == this.ArchivedNodeForeColor)
				{
					relationsNode.ForeColor = node.ForeColor;
				}
				relationsNode.ImageIndex =  MogUtil_AssetIcons.GetClassIconIndex( Relationship_ImageText );
				relationsNode.SelectedImageIndex =  relationsNode.ImageIndex;
				relationsNode.Tag = new Mog_BaseTag( relationsNode, relationsNode.Text );
				
				// Foreach entry in our relationship sections
				foreach( DictionaryEntry entry in sections )
				{
					string section = (string)entry.Key;
					ArrayList values = (ArrayList)entry.Value;
					
					// Create a node for this Section (e.g. "Packages.PC")
					TreeNode sectionNode = relationsNode.Nodes.Add( section );
					if(node.ForeColor == this.ArchivedNodeForeColor)
					{
						sectionNode.ForeColor = node.ForeColor;
					}
					sectionNode.Tag = new Mog_BaseTag( sectionNode, fullFilename );

					// Foreach value (name) in values, populate
					foreach( string name in values )
					{
						// Get a MOG_Filename and create our node...
						MOG_Filename nodeFilename = new MOG_Filename( name );
						TreeNode valueNode = sectionNode.Nodes.Add( name );
						if(node.ForeColor == this.ArchivedNodeForeColor)
						{
							valueNode.ForeColor = node.ForeColor;
						}

						Mog_BaseTag valueTag = null;
						// If we have an Asset, we will get ready to expand it down...
						if( MOG_FILENAME_TYPE.MOG_FILENAME_Asset == nodeFilename.GetFilenameType())
						{
							// Get currently blessed timestamp and valid filename
							nodeFilename = MOG_ControllerProject.GetAssetCurrentBlessedVersionPath( nodeFilename );

							// Assign our tag, set our image, and add our blank node for further expansion
							valueTag = new Mog_BaseTag( valueNode, nodeFilename.GetEncodedFilename(), this.FocusForAssetNodes, true );
							SetImageIndices( valueNode, GetAssetImageIndex( nodeFilename.GetEncodedFilename() ) );
							// Make it so we can expand below our Package (or whatever relationship we have)
							//valueNode.Nodes.Add( Blank_Node_Text );
						}
						else
						{
							valueTag = new Mog_BaseTag( valueNode, valueNode.Text );
						}

						// Reference our valueTag to valueNode.Tag
						valueNode.Tag = valueTag;
						SetImageIndices( valueNode, GetAssetFileImageIndex( ((Mog_BaseTag)node.Tag).FullFilename ) );
					}
				}

			} // end if
		} // end ()

		/// <summary>
		/// Adds "Imported.Files" and "Processed.Files" nodes to the node passed in.  Assumes that you are passing
		///  in a node.Tag that is a Mog_BaseTag, which should have a valid full filename.
		/// </summary>
		/// <param name="tree"></param>
		/// <param name="node"></param>
		private void PopulateFileNodes( TreeNode node )
		{
			// Get a full Filename
			string fullFilename = GetBlessedVersionPath( ((Mog_BaseTag)node.Tag).FullFilename );
			// Get our baseFolderIndex for all the nodes we're adding in this block
			int baseFolderIndex = MogUtil_AssetIcons.GetClassIconIndex( BaseFolder_ImageText );

			// Create a node for imported files
			TreeNode importedFilesNode = new TreeNode( Imported_Files_Text );
			// Assign the default node checked state
			importedFilesNode.Checked = NodesDefaultToChecked;

			importedFilesNode.Tag = new Mog_BaseTag( importedFilesNode, fullFilename, LeafFocusLevel.RepositoryItems, true );
			if(node.ForeColor == this.ArchivedNodeForeColor)
			{
				importedFilesNode.ForeColor = node.ForeColor;
			}

			// Create a node for our processed files
			TreeNode processedFilesNode = new TreeNode( Processed_Files_Text );

			// Assign the default node checked state
			processedFilesNode.Checked = NodesDefaultToChecked;
			processedFilesNode.Tag = new Mog_BaseTag( processedFilesNode, fullFilename, LeafFocusLevel.RepositoryItems, true );
			if(node.ForeColor == this.ArchivedNodeForeColor)
			{
				processedFilesNode.ForeColor = node.ForeColor;
			}

			// Get the directories for our repository item
			DirectoryInfo[] directories = DosUtils.DirectoryGetList( fullFilename, "");
			// Get our properties, so we can tell if this is a NativeDataType == false
			MOG_Properties assetProperties = new MOG_Properties( new MOG_Filename( fullFilename ) );	

			// If we got directories
			if( directories != null && directories.Length > 0 )
			{
				// Go through each platform only for the asset of this parent node
				foreach(DirectoryInfo directory in directories)
				{
					// If we are looking at the Files.Imported directory...
					if( directory.Name.ToLower().IndexOf("imported") > -1)
					{
						PopulateFiles( directory, importedFilesNode );
					}
					// We are looking at a processed directory, populate if we are not a NativeDataType...
					else if(assetProperties.NativeDataType == false) 
					{
						string platformName = directory.Name.Replace("Files.", "");
						TreeNode platformNode = new TreeNode( platformName );
						platformNode.Tag = new Mog_BaseTag( platformNode, fullFilename, LeafFocusLevel.RepositoryItems, true );
						if(node.ForeColor == this.ArchivedNodeForeColor)
						{
							platformNode.ForeColor = node.ForeColor;
						}
						processedFilesNode.Nodes.Add( platformNode );
						platformNode.ImageIndex = baseFolderIndex;
						platformNode.SelectedImageIndex = platformNode.ImageIndex;
						PopulateFiles( directory, platformNode );
					}
				}
			}

			// Add our files nodes
			node.Nodes.Add( importedFilesNode );
			importedFilesNode.ImageIndex = baseFolderIndex;
			importedFilesNode.SelectedImageIndex = importedFilesNode.ImageIndex;

			// If this is not a native data type, show our processed node...
			if(assetProperties.NativeDataType == false)
			{
				node.Nodes.Add( processedFilesNode );
				processedFilesNode.ImageIndex = baseFolderIndex;
				processedFilesNode.SelectedImageIndex = processedFilesNode.ImageIndex;
			}
		}

		/// <summary>
		/// TODO:  Finish this comment
		/// </summary>
		/// <param name="directory"></param>
		/// <param name="parentNode"></param>
		/// <param name="fileTag"></param>
		private void PopulateFiles( DirectoryInfo directory, TreeNode parentNode )
		{
			// Get the files in this directory
			FileInfo[] files = DosUtils.FileGetList( directory.FullName, "" );
			// If files is OK...
			if( files != null && files.Length > 0 )
			{
				// Add each file to the Imported Files node
				foreach( FileInfo file in files )
				{
					TreeNode fileNode = new TreeNode( file.Name );

					// Assign the default node checked state			
					fileNode.Checked = NodesDefaultToChecked;
			
					if(parentNode.ForeColor == this.ArchivedNodeForeColor)
					{
						fileNode.ForeColor = parentNode.ForeColor;
					}
					fileNode.Tag = new Mog_BaseTag( fileNode, ((Mog_BaseTag)parentNode.Tag).FullFilename,
						LeafFocusLevel.RepositoryItems, true );
					parentNode.Nodes.Add(fileNode);
					SetImageIndices( fileNode, GetAssetFileImageIndex( directory.FullName + "\\" + file.Name ) );
				}
			}

			DirectoryInfo[] innerDirectories = DosUtils.DirectoryGetList( directory.FullName, "" );
			if( innerDirectories != null && innerDirectories.Length > 0 )
			{
				// For each inner directory, recursively go through and populate
				foreach( DirectoryInfo innerDirectory in innerDirectories )
				{
					TreeNode innerDirectoryNode = new TreeNode( innerDirectory.Name );

					// Assign the default node checked state			
					innerDirectoryNode.Checked = NodesDefaultToChecked;
					innerDirectoryNode.Tag = new Mog_BaseTag( innerDirectoryNode, ((Mog_BaseTag)parentNode.Tag).FullFilename,
						LeafFocusLevel.RepositoryItems, true );
					parentNode.Nodes.Add( innerDirectoryNode );
					SetImageIndices( innerDirectoryNode, GetSpecialImageIndex( BaseFolder_ImageText ) );
					// Recurse to the next level
					PopulateFiles( innerDirectory, 
						innerDirectoryNode );
				}
			}
		}
		#endregion LeafFocusLevel.RepositoryItem expansion

		#region ContainedItems level expansion
		/// <summary>
		/// Populates Assets or Packages (depending on repository item type) within the Contained Items node
		/// </summary>
		/// <param name="tree"></param>
		/// <param name="node"></param>
		private void PopulateContainedItemNodes( TreeNode node )
		{
			// Get a MOG_Filename for the node we are on
			MOG_Filename repositoryFile = new MOG_Filename( ((Mog_BaseTag)node.Tag ).FullFilename );
			MOG_Properties repositoryProp = new MOG_Properties( repositoryFile );

			// Populate repositoryList according to what type of repository item we have
			ArrayList repositoryList = new ArrayList();

			if( repositoryProp.IsPackage )
			{
				// Make sure we retain the fact that we now know we are a package
				((Mog_BaseTag)node.Tag).PackageNodeType = PackageNodeTypes.Package;

				// Get the list of contained assets
				repositoryList = MOG_ControllerPackage.GetAssetsInPackage( repositoryFile );
			}
			else
			{
				ArrayList properties = repositoryProp.GetPackages();
				foreach( MOG_Property property in properties )
				{
					string packageName = MOG_ControllerPackage.GetPackageName( property.mPropertyKey );
					repositoryList.Add( new MOG_Filename( packageName ) );
				}
			}

			PopulateFromMogFilenames( node, repositoryList );
		}
		#endregion ContainedItems level expansion

		#region PackageGroup level expansion
		/// <summary>
		/// Used in PopulateRepositoryItemNodes() to populate Package Groups
		/// </summary>
		/// <param name="node">Node to which we will add Group Nodes (as applicable</param>
		private void PopulatePackageGroupNodes( TreeNode node )
		{
			if( this.mPopulatePackage != null )
			{
				mPopulatePackage.PopulatePackageGroupNodes( node );
			}
		} // end ()

		/// <summary>
		/// Encapsulates everything having to do with a populating a PackageNode,
		///  since Package population is a TreeView population unto itself...
		/// </summary>
		private class PopulatePackageClass
		{
			public MogControl_AssetTreeView mParent;
			public PopulatePackageClass( MogControl_AssetTreeView parentTree )
			{
				this.mParent = parentTree;
			}
			/// <summary>
			/// Used in PopulateRepositoryItemNodes() to populate Package Groups
			/// </summary>
			/// <param name="node">Node to which we will add Group Nodes (as applicable</param>
			public void PopulatePackageGroupNodes( TreeNode node )
			{
				SortedList groupsToAdd = null;

				// If we have no package groupNodes, create them
				if( ((Mog_BaseTag)node.Tag).AttachedPackageGroupNodes == null )
				{
					groupsToAdd = InitializePackageGroups( node );
				}
				else
				{
					groupsToAdd = InitializePackageSubGroups( node );
				}

				CheckForValidSubNodes( node );

				// If we are supposed to, add all our assets for this group
				if( mParent.ExpandPackageGroupAssets )
				{
					AddPackageGroupAssets(node);
				}
			} // end ()

			private void CheckForValidSubNodes( TreeNode node )
			{
				// For each of our nodes to add, check if it is a terminal node
				foreach( TreeNode subNode in node.Nodes )
				{
					// Get our groupName (trimming off the '~', if we have one)
					string groupName = GetGroupNameFromTreeNode( subNode );
					if( groupName.Length > 0 && groupName[0] == char.Parse(mParent.PathSeparator) )
					{
						groupName = groupName.Substring(1);
					}

					// Get our assets and groups under this subNode
					ArrayList assets = GetGroupAssets( groupName, ((Mog_BaseTag)subNode.Tag).AttachedPackageGroupNodes );
					SortedList moreGroups = GetSubNodesToAdd( subNode );

					// If we have nothing underneath this node, clear it.
					if( (assets == null || assets.Count < 1) && (moreGroups == null || moreGroups.Count < 1 ))
					{
						subNode.Nodes.Clear();
					}
				}
			}

			/// <summary>
			/// Initialize the SortedList of all the groups and Assets we will later be populating (if the user
			///  expands down into groups)
			/// </summary>
			/// <param name="node"></param>
			private SortedList InitializePackageGroups( TreeNode node )
			{
				MOG_Filename packageFile = new MOG_Filename( ((Mog_BaseTag)node.Tag).FullFilename );
				// Check if this is missing a version timestamp?
				if (packageFile.GetVersionTimeStamp().Length == 0)
				{
					// Obtain the current version timestamp
					packageFile = MOG_ControllerProject.GetAssetCurrentBlessedVersionPath(packageFile);
				}
				// Given packageFile is a Package, get its subGroups
				ArrayList subGroups = MOG.DATABASE.MOG_DBPackageAPI.GetAllActivePackageGroups(packageFile, packageFile.GetVersionTimeStamp());
// JohnRen - Not sure why we did this because this was the same thing we just did...Could we ever get a package assignment here?
//				// If we didn't find any subGroups, we are looking at a subGroup, so lets get our groups
//              if( subGroups.Count < 1 )
//				{
//					string packageName = MOG_ControllerPackage.GetPackageName( packageFile.GetEncodedFilename() );
//					packageFile.SetFilename( packageName );
//					packageFile = MOG_ControllerProject.GetAssetCurrentBlessedVersionPath( packageFile );
//					subGroups = MOG.DATABASE.MOG_DBPackageAPI.GetAllActivePackageGroups(packageFile, packageFile.GetVersionTimeStamp() );
//				}

				// Store a list of our subGroups along with their Assets (if any)
				SortedList subGroupsPlusAssets = new SortedList( subGroups.Count );
				foreach( string group in subGroups )
				{
					string correctedGroup = group.Replace( "/", mParent.PathSeparator );
					subGroupsPlusAssets.Add( correctedGroup, GetSubGroupsAssetArrayList( packageFile, group ) );
				}

				((Mog_BaseTag)node.Tag).AttachedPackageGroupNodes = subGroupsPlusAssets;
				return InitializePackageSubGroups( node );
			} // end ()

			private ArrayList GetSubGroupsAssetArrayList( MOG_Filename packageFile, string group )
			{
				// Find the assets associated with this package group/object and return them
				ArrayList assetFilenames = MOG_ControllerPackage.GetAssetsInPackageGroup( packageFile, 
					packageFile.GetVersionTimeStamp(), group );
				if( assetFilenames != null )
				{
					return assetFilenames;
				}

				// By default, return a blank ArrayList
				return new ArrayList();
			}

			/// <summary>
			/// Right now, this is creating blank nodes and it's still causing problems with not 
			///  showing when a node should not be expandable...
			/// </summary>
			/// <param name="node"></param>
			private SortedList InitializePackageSubGroups( TreeNode node )
			{
				SortedList groupsToAdd = GetSubNodesToAdd( node );

				// Add a node for each of our groups at this level
				foreach( DictionaryEntry entry in groupsToAdd )
				{
					string group = entry.Key.ToString();
					string groupPath = entry.Value.ToString().Replace(mParent.PathSeparator, "/");

					TreeNode nextToExpand = AddPackageGroupNode( node, group, groupPath );
				}

				return groupsToAdd;
			} // end ()

			private SortedList GetSubNodesToAdd(TreeNode node)
			{
				// Store the groups we need to add
				SortedList groupsToAdd = new SortedList();

				if( ((Mog_BaseTag)node.Tag).AttachedPackageGroupNodes != null )
				{
					// Find our package node
					TreeNode packageNode = mParent.FindPackage( node );
					if (packageNode != null)
					{
						// Make sure this packageNode's is our parent
						if (node.FullPath.StartsWith(packageNode.FullPath, StringComparison.CurrentCultureIgnoreCase))
						{
							// Extract off our parent's path to obtain this node's subPath
							string subPath = node.FullPath.Substring(packageNode.FullPath.Length).Trim(mParent.PathSeparator.ToCharArray());

							// Foreach group in our groups list, find ones that match this depth and add them...
							foreach (DictionaryEntry groupEntry in ((Mog_BaseTag)node.Tag).AttachedPackageGroupNodes)
							{
								// Get our stored information into meaninful identifiers
								string group = groupEntry.Key.ToString();

								// If we have a valid group AND this Node IS our PackageNode AND this group has no sub-groups...
								if (group.Length > 0 && node == packageNode && group.IndexOf(mParent.PathSeparator) < 0)
								{
									// Add this group:
									groupsToAdd.Add(group, group);
								}
								// Check if this group is a child of this node?
								else if (group.Length > subPath.Length &&
										 group.StartsWith(subPath, StringComparison.CurrentCultureIgnoreCase))
								{
									// Add the sub-group node:

									// If we are about to duplicate the group we are in, skip this group...
									if (subPath == group)
									{
										continue;
									}

									// Break up our relativeGroups (relativeGroups.Length will give us our new node's depth)
									string[] relativeGroups = subPath.Split(mParent.PathSeparator.ToCharArray());
									// Create the depth of the node we are (maybe) going to add to
									int actualGroupIndex = relativeGroups.Length > 0 ? relativeGroups.Length - 1 : 0;

									// Break up the groups we have in this group
									string[] groups = group.Split(mParent.PathSeparator.ToCharArray());

									// If we have groups.Length greater than our new node's depth,  AND we don't already have this group,
									//  AND our groups.Length is greater than the depth of the node we are adding to, 
									//  AND our group's parent is the same as our actual parent node...
									if (groups.Length > relativeGroups.Length && !groupsToAdd.Contains(groups[relativeGroups.Length])
										&& groups.Length > actualGroupIndex && groups[actualGroupIndex] == relativeGroups[actualGroupIndex])
									{
										// Add our subGroup
										groupsToAdd.Add(groups[relativeGroups.Length], group);
									}
								}
							}
						}
					}
				}

				return groupsToAdd;
			}

			/// <summary>
			/// Add a node for a PackageGroup
			/// </summary>
			/// <param name="node"></param>
			/// <param name="newNodeName"></param>
			/// <param name="subGroups"></param>
			/// <param name="groupPath"></param>
			/// <returns></returns>
			private TreeNode AddPackageGroupNode(TreeNode node, string newNodeName, string groupPath )
			{
				SortedList subGroups = ((Mog_BaseTag)node.Tag).AttachedPackageGroupNodes;
				MOG_Filename packageFile = new MOG_Filename( ((Mog_BaseTag)node.Tag).FullFilename );

				// Add our groupNode
				TreeNode groupNode = new TreeNode( newNodeName );
				// Assign the default node checked state			
				//groupNode.Checked = NodesDefaultToChecked;

				groupNode.Tag = new Mog_BaseTag( groupNode, packageFile.GetEncodedFilename(), 
					LeafFocusLevel.PackageGroup, true );

				Mog_BaseTag groupNodeTag = (Mog_BaseTag)groupNode.Tag;
				groupNodeTag.PackageFullName = packageFile.GetAssetFullName() + "/" + groupPath;
				groupNodeTag.PackageFullPath = groupPath;
				groupNodeTag.AttachedPackageGroupNodes = subGroups;

				// Check the node name for group specifiers
				if( newNodeName.IndexOf( "(" ) > -1 && newNodeName.IndexOf( ")" ) > -1 )
				{
					groupNodeTag.PackageNodeType = PackageNodeTypes.Object;
					groupNode.ImageIndex = MogUtil_AssetIcons.GetClassIconIndex( PackageObject_ImageText );
				}
				else
				{
					groupNodeTag.PackageNodeType = PackageNodeTypes.Group;
					groupNode.ImageIndex = MogUtil_AssetIcons.GetClassIconIndex( PackageGroup_ImageText );
				}
				groupNode.SelectedImageIndex = groupNode.ImageIndex;

				groupNode.Nodes.Add( Blank_Node_Text );
				node.Nodes.Add( groupNode );

				return groupNode;
			} // end ()

			private string GetGroupNameFromTreeNode( TreeNode node )
			{
				string groupName = null;

			
				MOG_Filename packageFile = new MOG_Filename( ((Mog_BaseTag)node.Tag).FullFilename );
				string pattern = mParent.PathSeparator + Contained_Assets_Text;

				// If we have this pattern, go ahead and get rid of it for our groupName...
				if( node.FullPath.IndexOf(pattern) > -1 )
				{
					groupName = node.FullPath.Substring( node.FullPath.IndexOf(pattern)+pattern.Length );
				}
					// Else, if we have the pacakage's Asset name in our Fullpath...
				else if( node.FullPath.IndexOf( packageFile.GetAssetName()) > -1 )
				{
					// Get rid of our package name to get our groupName
					groupName = node.FullPath.Substring( node.FullPath.IndexOf( packageFile.GetAssetName())+packageFile.GetAssetName().Length );
				}
				// Else, we'll leave groupName as null, which will make it throw (below)

				return groupName;
			}

			/// <summary>
			/// Add the Assets node for the PackageGroup associated with the node being passed in
			/// </summary>
			/// <param name="node"></param>
			private void AddPackageGroupAssets(TreeNode node)
			{
				try
				{
					// Get a groupName to get our packaged assets from
					string groupName = GetGroupNameFromTreeNode( node );			

					// If we have a starting ~ that is not a solitary ~, remove it
					if( groupName.IndexOf(mParent.PathSeparator) == 0 && groupName.Length > 1)
					{
						groupName = groupName.Substring(1);
					}

					// Get our grouped assets
					SortedList groupsPlusAssets = ((Mog_BaseTag)node.Tag).AttachedPackageGroupNodes;
					ArrayList groupAssets = GetGroupAssets( groupName, groupsPlusAssets);

					// Populate if we have any grouped assets...
					if( groupAssets != null && groupAssets.Count > 0 ) 
					{	//GLK:  This is the place where I should start determining which version of an asset was attached to this asset.
						TreeNode assetsNode = new TreeNode( Assets_Text );
						// Assign the default node checked state			
						//assetsNode.Checked = NodesDefaultToChecked;

						assetsNode.Tag = new Mog_BaseTag( assetsNode, ((Mog_BaseTag)node.Tag).FullFilename );
						node.Nodes.Add( assetsNode );
						// Set the icon
						assetsNode.ImageIndex = MogUtil_AssetIcons.GetClassIconIndex( BaseFolder_ImageText );
						assetsNode.SelectedImageIndex = assetsNode.ImageIndex;

						mParent.PopulateFromMogFilenames( assetsNode, groupAssets );
					}
				}
				catch( NullReferenceException ex )
				{
					MOG_Report.ReportMessage( "Error Adding Assets to Package Node!", ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.ERROR );
					//MOG_Report.ReportSilent( ex.Message, ex.StackTrace );
				}
			} // end ()

			private ArrayList GetGroupAssets( string groupName, SortedList groupsPlusAssets )
			{
				ArrayList groupAssets = null;
				if( groupsPlusAssets.IndexOfKey( groupName ) > -1 )
				{
					groupAssets = (ArrayList)groupsPlusAssets.GetByIndex( groupsPlusAssets.IndexOfKey( groupName ));
				}
				return groupAssets;
			}
		} // end class
 		#endregion LeafFocusLevel.PackageGroup expansion

		#region Utilities
		private string GetBlessedVersionPath( string fullFilename )
		{
			MOG_Filename filename = new MOG_Filename( fullFilename );
			if( filename.GetVersionTimeStamp().Length < 1 )
			{
				filename = MOG_ControllerProject.GetAssetCurrentBlessedVersionPath( filename );
			}
			return filename.GetEncodedFilename();
		}
		/// <summary>
		/// Populates a node from an ArrayList of MOG_Filenames
		/// </summary>
		/// <param name="node">Node to populate</param>
		/// <param name="repositoryList">ArrayList of MOG_Filenames to populate from</param>
		private void PopulateFromMogFilenames( TreeNode node, ArrayList repositoryList )
		{
			// Foreach MOG_Filename we get back from our Controller call...
			foreach( MOG_Filename repositoryFilename in repositoryList )
			{
				// Add a new repositoryNode that can be expanded down.
				string repositoryItem = repositoryFilename.GetAssetName();
				TreeNode repositoryNode = GetAssetNode( repositoryFilename, node.ForeColor, true );
				node.Nodes.Add( repositoryNode );
				SetImageIndices( repositoryNode, GetAssetFileImageIndex( repositoryFilename.GetEncodedFilename() ) );
				// If we didn't get a valid image index, lets get something MOG-generic for an Asset
				if( repositoryNode.ImageIndex == 0 )
				{
					repositoryNode.ImageIndex = MogUtil_AssetIcons.GetClassIconIndex( Asset_ImageText );
					repositoryNode.SelectedImageIndex = repositoryNode.ImageIndex;
				}
			}
		}

		/// <summary>
		/// Walk up the parents of a node looking for a parent with a valid MOG_Filename
		/// in the tag as well as the isPackage() bool set to true
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public TreeNode FindPackage(TreeNode node)
		{
			if (node.Tag is Mog_BaseTag)
			{
				Mog_BaseTag tag = node.Tag as Mog_BaseTag;
				MOG_Filename assetName = new MOG_Filename( tag.FullFilename );
				MOG_Properties props = new MOG_Properties(assetName);
				if (assetName != null && props.IsPackage && 
					(node.Text == assetName.GetAssetName() || node.Text == assetName.GetAssetLabel()
					|| node.Text == Contained_Assets_Text ) )
				{
					// Make sure we retain the fact that we now know we are a package
					((Mog_BaseTag)node.Tag).PackageNodeType = PackageNodeTypes.Package;

					return node;
				}
			}
			
			if (node.Parent != null)
			{
				return FindPackage(node.Parent);
			}
			
			return null;
		} // end ()

		/// <summary>
		/// Decides if we need to go into the BaseLeafTreeView
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		protected virtual bool IsAtAssetLevel( TreeNode node )
		{
			// If we have a valid node
			if( node != null && node.Tag is Mog_BaseTag )
			{
				bool isRepositoryFocus = ((Mog_BaseTag)node.Tag).RepositoryFocus == RepositoryFocusLevel.Repository;
				MOG_Filename assetName = new MOG_Filename( ((Mog_BaseTag)node.Tag).FullFilename );

				// We will return true, (allowing this to expand as a Leaf node
				if( isRepositoryFocus && 
                    ( ((Mog_BaseTag)node.Tag).Execute 
                        || (assetName.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset) ) )
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Finds the file associated with the Asset by looking for something that 
		///  matches the assetLabel in the assetDirectory
		/// </summary>
		/// <param name="assetLabel"></param>
		/// <param name="assetDirectory"></param>
		/// <returns>Full Windows path for a filename matching the asset label</returns>
		protected string FindAssetsFile( string assetLabel, string assetDirectory )
		{
			string fullFilename = "";
			if( DosUtils.DirectoryExist( assetDirectory ) )
			{
				DirectoryInfo[] directories = DosUtils.DirectoryGetList( assetDirectory, "");
				FileInfo[] files = DosUtils.FileGetList( assetDirectory, "" );
				string guessedFilename = "";

				// See if we find our exact assetLabel in files
				foreach( FileInfo file in files )
				{
					if( file.Name.ToLower() == assetLabel.ToLower() )
					{
						return file.FullName;
					}
					else if( file.Name.ToLower().IndexOf( assetLabel.ToLower() ) > -1 )
					{
						guessedFilename = file.FullName;
					}
				}

				// If we get here, we didn't find the file, so look further...
				foreach( DirectoryInfo directory in directories )
				{
					string possibleFilename = FindAssetsFile( assetLabel, directory.FullName );
					if( possibleFilename.Length > 0 )
					{
						return possibleFilename;
					}
				}

				// If, after we've got everything done, we still haven't found anything...
				if( fullFilename.Length < 1 )
				{
					// Put our guessedFilename into fullFilename so at least we can get a file icon
					fullFilename = guessedFilename;
				}
			}
			return fullFilename;
		}

		/// <summary>
		/// Used to get an Asset node with a good node.Tag for this TreeView (at sub-Asset level)
		/// </summary>
		private TreeNode GetAssetNode( MOG_Filename asset, System.Drawing.Color foreColor, bool useFullFilename )
		{
			string assetName = asset.GetAssetFullName();
			MOG_Filename assetFullFilename = MOG_ControllerProject.GetAssetCurrentBlessedPath( asset );

			TreeNode assetNode = new TreeNode( asset.GetAssetName() );

			// Assign the default node checked state			
			assetNode.Checked = NodesDefaultToChecked;

			if( useFullFilename )
			{
				assetNode.Text = asset.GetAssetFullName();
			}
            
			assetNode.Tag = new Mog_BaseTag( assetNode, assetFullFilename.GetEncodedFilename(), LeafFocusLevel.RepositoryItems, true );

			assetNode.Nodes.Add( new TreeNode( Blank_Node_Text ) );	
			return assetNode;		
		}
		#endregion Utilities

		public TreeNode AddNode_PackageAssignment(string packageAssignment)
		{
			TreeNode node = null;

			// Make sure we have a valid package assignment?
			if (packageAssignment.Length > 0)
			{
				// Get the peices of the specified package assignment
				string packageName = MOG_ControllerPackage.GetPackageName(packageAssignment);
				MOG_Filename packageAssetFilename = new MOG_Filename(packageName);

				if (ShowPlatformSpecific || !packageAssetFilename.IsPlatformSpecific())
				{
					// Add the classification first
					node = AddNode_Classification(null, packageAssetFilename.GetAssetClassification());
					if (node != null)
					{
						// Add the package
						node = AddNode_AssetName(node, packageAssetFilename);
						if (node != null)
						{
							// Add the package groups
							TreeNode lastGroupNode = AddNode_PackageGroups(node, packageAssignment);
							if (lastGroupNode != null)
							{
								node = lastGroupNode;
							}
							if (node != null)
							{
								// Add the package objects
								TreeNode lastObjectNode = AddNode_PackageObjects(node, packageAssignment);
								if (lastObjectNode != null)
								{
									node = lastObjectNode;
								}
							}
						}
					}
				}
			}

			return node;
		}

		public TreeNode AddNode_Classification(TreeNode parent, string classification)
		{
			TreeNodeCollection collection = (parent != null) ? parent.Nodes : Nodes;
			TreeNode node = null;

			// Make sure there was a classification contained in the specified package assignment
			if (!String.IsNullOrEmpty(classification))
			{
				// Insert the classification parts
				string[] parts = classification.Split("~".ToCharArray());
				foreach (string part in parts)
				{
					// Check if this classification node already exists
					node = FindLocalNode(collection, part);
					if (node == null)
					{
						// Check if this is within a node previously marked with a 'Blank_Node_Text'
						if (collection.Count == 1 && collection[0].Text == Blank_Node_Text)
						{
							collection.Clear();
						}
						// Create a new classification node
						node = collection.Add(part);
						node.ForeColor = Color.DarkSalmon;
						Mog_BaseTag tag = new Mog_BaseTag(node, node.FullPath);
						tag.PackageNodeType = PackageNodeTypes.Class;
						node.Tag = tag;
					}

					// Update our collection off this node
					collection = node.Nodes;
				}
			}

			return node;
		}

		public TreeNode AddNode_AssetName(TreeNode parent, MOG_Filename assetName)
		{
			TreeNodeCollection collection = parent.Nodes;
			TreeNode node = null;

			// Make sure there was an asset name specified in the package assignment
			if (!String.IsNullOrEmpty(assetName.GetAssetName()))
			{
				// Check if this asset node already exists
				node = FindLocalNode(collection, assetName.GetAssetName());
				if (node == null)
				{
					// Check if this is within a node previously marked with a 'Blank_Node_Text'
					if (collection.Count == 1 && collection[0].Text == Blank_Node_Text)
					{
						collection.Clear();
					}
					// Create a new asset node
					node = collection.Add(assetName.GetAssetName());
					node.ForeColor = Color.DarkSalmon;
					Mog_BaseTag tag = new Mog_BaseTag(node, assetName.GetAssetFullName());
					tag.PackageFullName = assetName.GetAssetFullName();
					tag.PackageNodeType = PackageNodeTypes.Asset;
					node.Tag = tag;

					SetImageIndices(node, GetAssetImageIndex(tag.PackageFullName));
				}

				// Update our collection off this node
				collection = node.Nodes;
			}

			return node;
		}

		public TreeNode AddNode_PackageGroups(TreeNode parent, string packageAssignment)
		{
			TreeNodeCollection collection = parent.Nodes;
			TreeNode node = null;

			// Check if the package group node already exists
			if (!String.IsNullOrEmpty(packageAssignment))
			{
				string packageName = MOG_ControllerPackage.GetPackageName(packageAssignment);
				string packageGroups = MOG_ControllerPackage.GetPackageGroups(packageAssignment);
				string packageObjects = MOG_ControllerPackage.GetPackageObjects(packageAssignment);
				string[] groups = MOG_ControllerPackage.GetPackageGroupLevels(packageAssignment);
				string groupPath = "";

				foreach (string group in groups)
				{
					groupPath = MOG_ControllerPackage.CombinePackageGroups(groupPath, group);

					// Check if this group node already exists
					node = FindLocalNode(collection, group);
					if (node == null)
					{
						// Check if this is within a node previously marked with a 'Blank_Node_Text'
						if (collection.Count == 1 && collection[0].Text == Blank_Node_Text)
						{
							collection.Clear();
						}
						// Create a new node
						node = collection.Add(group);
						node.ForeColor = Color.DarkSalmon;

						string fullname = MOG_ControllerPackage.CombinePackageAssignment(packageName, groupPath, null);
						Mog_BaseTag tag = new Mog_BaseTag(node, fullname);
						tag.PackageFullName = fullname;
						tag.PackageNodeType = PackageNodeTypes.Group;
						node.Tag = tag;
					}

					// Update our collection off this node
					collection = node.Nodes;
				}
			}

			return node;
		}

		public TreeNode AddNode_PackageObjects(TreeNode parent, string packageAssignment)
		{
			TreeNodeCollection collection = parent.Nodes;
			TreeNode node = null;

			// Check if there was a package object specified in the package assignment
			if (!String.IsNullOrEmpty(packageAssignment))
			{
				string packageName = MOG_ControllerPackage.GetPackageName(packageAssignment);
				string packageGroups = MOG_ControllerPackage.GetPackageGroups(packageAssignment);
				string packageObjects = MOG_ControllerPackage.GetPackageObjects(packageAssignment);
				string[] objects = MOG_ControllerPackage.GetPackageObjectLevels(packageAssignment);
				string objectPath = "";

				foreach (string obj in objects)
				{
					objectPath = MOG_ControllerPackage.CombinePackageGroups(objectPath, obj);

					// Check if this object node already exists
					node = FindLocalNode(collection, obj);
					if (node == null)
					{
						// Check if this is within a node previously marked with a 'Blank_Node_Text'
						if (collection.Count == 1 && collection[0].Text == Blank_Node_Text)
						{
							collection.Clear();
						}
						// Create a new node
						node = collection.Add(obj);
						node.ForeColor = Color.DarkSalmon;

						string fullname = MOG_ControllerPackage.CombinePackageAssignment(packageName, packageGroups, objectPath);
						Mog_BaseTag tag = new Mog_BaseTag(node, fullname);
						tag.PackageFullName = fullname;
						tag.PackageNodeType = PackageNodeTypes.Object;
						node.Tag = tag;
					}

					// Update our collection off this node
					collection = node.Nodes;
				}
			}

			return node;
		}

		public override void MakeAssetCurrent(MOG_Filename assetFilename)
		{
			// Let our parent do its magic first
			base.MakeAssetCurrent(assetFilename);

			// Make sure this assetFilename has the info we want
			if (assetFilename != null &&
				assetFilename.GetVersionTimeStamp().Length > 0)
			{
				// Attempt to locate this asset
				TreeNode foundNode = FindNode(assetFilename.GetAssetFullName());
				if (foundNode != null)
				{
					// Check if this is a package node?
					if (ExpandPackageGroupAssets)
					{
						// Check if the user has this node expanded?
						if (foundNode.IsExpanded)
						{
							// Check if this node has package groups attached?
							if (((Mog_BaseTag)foundNode.Tag).PackageNodeType == PackageNodeTypes.Package)
							{
								// Refresh this node becasue the package contents were just changed
								// This is a bit of a brute force action here but we want the contents of the package to be refreshed
								Initialize();
							}
						}
					}
				}
			}
		}

	}
}
