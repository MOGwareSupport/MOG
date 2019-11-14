using System;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Collections;
using System.Windows.Forms;

using MOG;
using MOG.TIME;
using MOG.COMMAND;
using MOG.PROJECT;
using MOG.FILENAME;
using MOG.PROPERTIES;
using MOG.CONTROLLER.CONTROLLERASSET;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERREPOSITORY;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.INI;
using MOG.PROMPT;
using MOG.PROGRESS;
using MOG.REPORT;
using MOG_CoreControls;
using System.ComponentModel;



namespace MOG_ControlsLibrary
{
	/// <summary>
	/// Summary description for AssetClassificationConfigControl.
	/// </summary>
	public class AssetClassificationConfigControl : MogControl_ClassificationTreeView
	{
		#region Member vars
		private ArrayList assetFilenameNodes = new ArrayList();

		private ArrayList defaultAssetTypes = new ArrayList( new string[] { "Textures", "Animations", "Sounds" } );
		private ArrayList defaultRippers = new ArrayList( new string[] { "texture_ripper.bat", "animation_ripper.bat", "sound_ripper.bat" } );
		private string projectRootPath = "";

		private TreeNodeCollection sourceNodes;

		#endregion
		#region Properties
		public string ProjectRootPath
		{
			get { return this.projectRootPath; }
			set { this.projectRootPath = value; }
		}

		public int NumAssets
		{
			get { return CountAssetNodes(this.sourceNodes); }
		}

		public new int NumClassifications
		{
			get { return CountClassNodes(this.sourceNodes); }
		}

		#endregion
		#region Events
		public static event EventHandler AssetImport_Begin;
		public static event EventHandler AssetImport_Finish;
		#endregion
		#region Constructors
		public AssetClassificationConfigControl()
		{
		
		}
		#endregion
		#region Protected functions
		protected override void AddClassificationNode(classTreeNode parentNode)
		{
			if (parentNode == null)
			{
				if (this.tvClassifications.Nodes.Count > 0)
					parentNode = this.tvClassifications.Nodes[0] as classTreeNode;
				else
					return;
			}

			if (!parentNode.IsAssetFilenameNode)
				base.AddClassificationNode(parentNode);
		}
		#endregion
		#region Public functions

		private int CountAssetsAndClasses(TreeNodeCollection nodes)
		{
			int count = 0;
			foreach (AssetTreeNode atn in nodes)
			{
				++count;

				if (!atn.IsAnAssetFilename)
					count += CountAssetsAndClasses(atn.Nodes);
			}

			return count;
		}

		public void EncodeAll()
		{
			this.tvClassifications.BeginUpdate();
			
			ProgressDialog progress = new ProgressDialog("Creating Asset Properties", "Please wait while the Properties for these assets are generated", EncodeAll_Worker, null, true);
			progress.ShowDialog();

			this.tvClassifications.EndUpdate();
		}
		
		private void EncodeAll_Worker(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;

			int totalItems = CountAssetsAndClasses(this.sourceNodes);
			int populatedItems = 0;

			// encode
			EncodeAll(this.tvClassifications.Nodes, totalItems, populatedItems, worker);
		}

		private int EncodeAll(TreeNodeCollection nodes, int totalItems, int populatedItems, BackgroundWorker worker)
		{
			foreach (classTreeNode ctn in nodes)
			{
				if (!ctn.FilledIn  &&  ctn.assetTreeNode != null)
				{
					// kill dummy node
					ctn.Nodes.Clear();

					// populate properties
					if (ctn.props == null)
					{
						// Get the classification's properties that we can edit
						ctn.props = MOG_Properties.OpenClassificationProperties(ctn.FullPath);
						ctn.props.SetImmeadiateMode(true);

						// build synctargetpath
						if (ctn.assetTreeNode.FileFullPath != "")	// "" indicates don't set sync data path
						{
							string syncDataPath = ctn.assetTreeNode.FileFullPath;
							if (syncDataPath.ToLower().StartsWith(this.projectRootPath.ToLower()))
								syncDataPath = syncDataPath.Substring( this.projectRootPath.Length ).Trim("\\".ToCharArray() );

							ctn.props.SyncTargetPath = syncDataPath;
						}
					}

					// add converted children
					foreach (AssetTreeNode atn in ctn.assetTreeNode.Nodes)
					{
						string message = "Creating:\n" +
										 "     " + ctn.FullPath + "\n" +
										 "     " + atn.FullPath.Substring(ctn.FullPath.Length);
						worker.ReportProgress(populatedItems * 100 / totalItems, message);

						ctn.Nodes.Add( EncodeNode(atn, ctn.props) );
					}

					// mark this node as filled-in
					ctn.FilledIn = true;
					ctn.assetTreeNode = null;
				}

				++populatedItems;

				// recurse
				populatedItems = EncodeAll(ctn.Nodes, totalItems, populatedItems, worker);
			}

			return populatedItems;
		}


		public void LoadAssetTree(TreeNodeCollection nodes)
		{
			this.sourceNodes = nodes;

			this.tvClassifications.Nodes.Clear();

			this.tvClassifications.BeforeExpand += new TreeViewCancelEventHandler(tvClassifications_BeforeExpand);
			this.tvClassifications.MouseDown += new MouseEventHandler(tvClassifications_MouseDown);
			this.tvClassifications.AfterSelect += new TreeViewEventHandler(tvClassifications_AfterSelect);

			// populate first level
			foreach (AssetTreeNode atn in nodes)
			{
				classTreeNode classNode = EncodeNode(atn, null);
				this.tvClassifications.Nodes.Add( classNode );

				classNode.Expand();
			}
		}
		#endregion
		#region Private functions

//		private int CountClassNodes(TreeNodeCollection nodes)
//		{
//			int count = 0;
//			foreach (AssetTreeNode atn in nodes)
//			{
//				if (atn.IsAClassification)
//				{
//					++count;
//					count += CountClassNodes(atn.Nodes);
//				}
//			}
//
//			return count;
//		}

		private int CountAssetNodes(TreeNodeCollection nodes)
		{
			int count = 0;
			foreach (AssetTreeNode atn in nodes)
			{
				if (atn.IsAnAssetFilename)
					++count;
				else if (atn.IsAClassification)
					count += CountAssetNodes(atn.Nodes);
			}

			return count;
		}

		private void RaiseAssetImport_Begin()
		{
			if (AssetImport_Begin != null)
				AssetImport_Begin(this, new EventArgs());
		}

		private void RaiseAssetImport_Finish()
		{
			if (AssetImport_Finish != null)
				AssetImport_Finish(this, new EventArgs());
		}


		public static int CountClassNodes(TreeNodeCollection nodes)
		{
			int count = 0;
			
			foreach (AssetTreeNode tn in nodes)
			{
				if (tn.IsAClassification)
				{
					++count;
					count += CountClassNodes(tn.Nodes);
				}
			}

			return count;
		}

		private int SameIndex(string strOne, string strTwo)
		{
			char[] one = strOne.ToLower().ToCharArray();
			char[] two = strTwo.ToLower().ToCharArray();

			int i = 0;
			while (i < one.Length  &&  i < two.Length)
			{
				if (one[i] != two[i])
					break;

				++i;
			}
			
			return i;
		}

		private classTreeNode EncodeNode(AssetTreeNode atn, MOG_Properties props)
		{
			classTreeNode classNode = new classTreeNode(atn.Text);
			classNode.ImageIndex = BLUEDOT_INDEX;
			classNode.SelectedImageIndex = BLUEDOT_INDEX;
			classNode.props = null;

			// if this is an asset filename node?
			if (atn.IsAnAssetFilename)
			{
				classNode.Text = classNode.Text;
				classNode.ForeColor = Color.Blue;
				classNode.ImageIndex = ARROW_INDEX;
				classNode.SelectedImageIndex = ARROW_INDEX;
				classNode.IsAssetFilenameNode = true;

				// Make list o' children files
				foreach (AssetTreeNode fileNode in atn.fileNodes)
				{
					if (fileNode.IsAFile)
					{
						classNode.importFiles.Add( fileNode.FileFullPath );
					}
				}

				// Check if we have a props?
				if (classNode.props != null)
				{
					// Setup SyncTargetPath Property
					string relativePath = "";
					string assetDirectoryPath = MOG.CONTROLLER.CONTROLLERASSET.MOG_ControllerAsset.GetCommonDirectoryPath(this.projectRootPath, classNode.importFiles);
					if (assetDirectoryPath.Length > this.projectRootPath.Length)
					{
						relativePath = assetDirectoryPath.Substring(this.projectRootPath.Length + 1);
					}

					// Set our SyncTargetPath
					classNode.props.SyncTargetPath = relativePath;
				}

				// add it to the list of asset filename nodes for later processing
				this.assetFilenameNodes.Add(classNode);

				return classNode;		// don't do children of an asset filename node
			}

			classNode.FilledIn = false;
			classNode.assetTreeNode = atn;

			// add a dummy child node if necessary
			if (atn.Nodes.Count > 0)
				classNode.Nodes.Add( new classTreeNode("DUMMY_NODE") );

			return classNode;
		}

		private classTreeNode EncodeTree(AssetTreeNode tn, MOG_Properties props, int handle)
		{
			classTreeNode classNode = new classTreeNode(tn.Text);
			classNode.ImageIndex = BLUEDOT_INDEX;
			classNode.SelectedImageIndex = BLUEDOT_INDEX;

			// Get a local handle for our progress dialog
			MOG_Progress.ProgressUpdate(handle, tn.FullPath);
			
			// if this is an asset filename node?
			if (tn.IsAnAssetFilename)
			{
				classNode.Text = classNode.Text;
				classNode.ForeColor = Color.Blue;
				classNode.ImageIndex = ARROW_INDEX;
				classNode.SelectedImageIndex = ARROW_INDEX;
				classNode.IsAssetFilenameNode = true;

				// Inherit Properties from parent
				// 'props' needs to be cloned or else all of these children assets will share the same Properties!!!
				classNode.props = props;

				// Make list o' children
				foreach (AssetTreeNode fileNode in tn.fileNodes)
				{
					if (fileNode.IsAFile)
					{
						classNode.importFiles.Add( fileNode.FileFullPath );
					}
				}

				// setup gamedatapath
				string assetDirectoryPath = MOG.CONTROLLER.CONTROLLERASSET.MOG_ControllerAsset.GetCommonDirectoryPath(this.projectRootPath, classNode.importFiles);
				string relativePath = "";
				if (assetDirectoryPath.Length > this.projectRootPath.Length)
				{
					relativePath = assetDirectoryPath.Substring(this.projectRootPath.Length + 1);
				}
				classNode.props.SyncTargetPath = relativePath;

				// add it to the list of asset filename nodes for later processing
				this.assetFilenameNodes.Add(classNode);

				return classNode;		// don't do children of an asset filename node
			}

			// Use our properties to pass along to our children
			props = MOG_ControllerProject.GetClassificationProperties(tn.FullPath.Replace(tn.TreeView.PathSeparator, "~"));
			props.SetImmeadiateMode(true);
			classNode.props = props;

			// Loop through all of our children nodes sending them our Properties
			foreach (AssetTreeNode subNode in tn.Nodes)
			{
				classNode.Nodes.Add( EncodeTree(subNode, props, handle) );
			}

			return classNode;
		}
		#endregion

		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(AssetClassificationConfigControl));
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// tvClassifications
			// 
			this.tvClassifications.Name = "tvClassifications";
			this.tvClassifications.Size = new System.Drawing.Size(501, 456);
			// 
			// propertyGrid
			// 
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.Size = new System.Drawing.Size(320, 440);
			// 
			// panel2
			// 
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(501, 456);
			// 
			// imageList
			// 
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			// 
			// AssetClassificationConfigControl
			// 
			this.Name = "AssetClassificationConfigControl";
			this.Size = new System.Drawing.Size(840, 456);
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		private void tvClassifications_AfterSelect(object sender, TreeViewEventArgs e)
		{
			classTreeNode ctn = e.Node as classTreeNode;
			PopulateNode(ctn);
		}

		private void tvClassifications_MouseDown(object sender, MouseEventArgs e)
		{
			TreeNode tn = this.tvClassifications.GetNodeAt(e.X, e.Y);
			this.tvClassifications.SelectedNodes.Clear();
			this.tvClassifications.SelectedNodes.Add(tn);
			classTreeNode ctn = tn as classTreeNode;
			PopulateNode(ctn);
		}

		private void PopulateNode(classTreeNode ctn)
		{
			if (ctn != null)
			{
				if (ctn.props == null)
				{
					// need to get a brand new props
					if (ctn.IsAssetFilenameNode)
					{
						// Get a new properties that we can edit
						ctn.props = MOG_Properties.OpenFileProperties(new MOG_PropertiesIni());
						// Load its inherited properties
						ctn.props.PopulateInheritance(new MOG_Filename(ctn.FullPath).GetAssetClassification());
						// Make sure we trigger the propertyGrid to redisplay this new props
						this.propertyGrid.SelectedObject = ctn.props;
					}
					else
					{
						// Get the classification's properties that we can edit
						ctn.props = MOG_Properties.OpenClassificationProperties(ctn.FullPath);
						ctn.props.SetImmeadiateMode(true);

						// build synctargetpath
						if (ctn.assetTreeNode != null  &&  ctn.assetTreeNode.FileFullPath != "")	// "" indicates don't set sync data path
						{
							string syncDataPath = ctn.assetTreeNode.FileFullPath;
							if (syncDataPath.ToLower().StartsWith(this.projectRootPath.ToLower()))
							{
								syncDataPath = syncDataPath.Substring( this.projectRootPath.Length ).Trim("\\".ToCharArray() );
							}
                            
							ctn.props.SyncTargetPath = syncDataPath;
						}

						// Make sure we trigger the propertyGrid to redisplay this new props
						this.propertyGrid.SelectedObject = ctn.props;

					}

					//					// mark node as populated
					//					ctn.FilledIn = true;
				}
				else
				{
					// need to update the existing props
					if (ctn.props.PopulateInheritance())
					{
						// Make sure we trigger the propertyGrid to redisplay this updated props
						this.propertyGrid.SelectedObject = ctn.props;
					}
				}
			}
		}

		private void tvClassifications_BeforeExpand(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		{
			classTreeNode ctn = e.Node as classTreeNode;
			if (ctn != null  &&  !ctn.FilledIn)
			{
				Cursor.Current = Cursors.WaitCursor;

				// kill the dummy node
				ctn.Nodes.Clear();

				// if we have an associated assettreenode
				if (ctn.assetTreeNode != null)
				{
					// populate the next level based on the asset tree node
					foreach (AssetTreeNode subAssetNode in ctn.assetTreeNode.Nodes)
						ctn.Nodes.Add( EncodeNode(subAssetNode, ctn.props) );

					// make sure we don't reload this level again
					ctn.FilledIn = true;
					ctn.assetTreeNode = null;
				}

				Cursor.Current = Cursors.Default;
			}
		}
	}
}


