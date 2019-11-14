using System;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Collections;
using System.Windows.Forms;

using MOG_ServerManager.Utilities;

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
using MOG.PROGRESS;
using MOG.PROMPT;
using MOG.REPORT;
using MOG_ControlsLibrary;
using MOG_CoreControls;
using System.ComponentModel;


namespace MOG_ServerManager
{
	/// <summary>
	/// Summary description for AssetClassificationConfigControl.
	/// </summary>
	public class AssetClassificationConfigControl : MogControl_ClassificationTreeView
	{
		#region System defs
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
		#endregion
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

		public void CreateAssetConfigs()
		{
			RaiseAssetImport_Begin();

			try
			{
				MOG_Project proj = MOG_ControllerProject.GetProject();
				if (proj != null)
				{
					ProgressDialog progress = new ProgressDialog("Importing Assets", "Please wait while the Assets are imported into the Project's Repository.", CreateAssetConfigs_Worker, null, true);
					progress.ShowDialog();

					// Post the projects new assets skipping the PostScanProcess
					MOG_ControllerProject.PostAssets(MOG_ControllerProject.GetProjectName(), MOG_ControllerProject.GetBranchName(), MOG_ControllerProject.GetProjectName(), true);
				}
			}
			catch (Exception e)
			{
				MOG_Report.ReportMessage("Unable to import asset", e.Message, e.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
			}
			finally
			{
				RaiseAssetImport_Finish();
			}
		}

		private void CreateAssetConfigs_Worker(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;

			MOG_ControllerProject.LoginUser("Admin");

			// Construct a new common timestamp for all of these assets
			string timestamp = MOG_Time.GetVersionTimestamp();

			// Activate the properties cache to help save time during the importation process
			MOG_Properties.ActivatePropertiesCache(true);

			for (int nodeIndex = 0; nodeIndex < assetFilenameNodes.Count; nodeIndex++)
			{
				classTreeNode tn = assetFilenameNodes[nodeIndex] as classTreeNode;

				string fullAssetName = tn.FullPath;//tn.Parent.FullPath + tn.Text;
				string fileList = Utils.ArrayListToString(tn.importFiles, "");

				// Check if this is a library asset?
				bool bIsInLibrary = false;
				if (tn.TreeView != null)
				{
					string fullPath = tn.FullPath + tn.TreeView.PathSeparator;
					string testPath = tn.TreeView.PathSeparator + "Library" + tn.TreeView.PathSeparator;
					if (fullPath.IndexOf(testPath, 0, StringComparison.CurrentCultureIgnoreCase) != -1)
					{
						bIsInLibrary = true;
					}
				}

				MOG_Filename repositoryName = null;
				if (bIsInLibrary && tn.importFiles.Count > 0)
				{
					// Use the timestamp of the file (Needed for out-of-date checks with library assets)
					String libraryTimestamp = "";
					FileInfo file = new FileInfo(tn.importFiles[0] as string);
					if (file != null && file.Exists)
					{
						libraryTimestamp = MOG_Time.GetVersionTimestamp(file.LastWriteTime);
					}
					repositoryName = MOG_ControllerRepository.GetAssetBlessedVersionPath(new MOG_Filename(fullAssetName), libraryTimestamp);
				}
				else
				{
					// Use the common timestamp for all the assets
					repositoryName = MOG_ControllerRepository.GetAssetBlessedVersionPath(new MOG_Filename(fullAssetName), timestamp);
				}

				MOG_Filename createdAssetFilename = null;

				string message = "Importing:\n" +
								 "     " + repositoryName.GetAssetClassification() + "\n" +
								 "     " + repositoryName.GetAssetName();
				worker.ReportProgress(nodeIndex * 100 / assetFilenameNodes.Count, message);

				if (worker.CancellationPending)
				{
					if (Utils.ShowMessageBoxConfirmation("Are you sure you want to cancel asset importation?", "Cancel Asset Importation?") == MOGPromptResult.Yes)
					{
						return;
					}
				}

				try
				{
					string dirScope = MOG_ControllerAsset.GetCommonDirectoryPath(this.projectRootPath, tn.importFiles);

					// Construct our list non-inherited asset assuming none
					ArrayList props = null;
					if (tn.props != null)
					{
						// Ask the tn.props for the list of non-inherited properties
						props = tn.props.GetNonInheritedProperties();
					}
					else
					{
						props = new ArrayList();

						// Setup SyncTargetPath Property
						string assetDirectoryScope = MOG_ControllerAsset.GetCommonDirectoryPath(this.projectRootPath, tn.importFiles);
						if (assetDirectoryScope.Length > this.projectRootPath.Length)
						{
							string syncTargetPath = assetDirectoryScope.Substring(this.projectRootPath.Length + 1);
							props.Add(MOG.MOG_PropertyFactory.MOG_Sync_OptionsProperties.New_SyncTargetPath(syncTargetPath));
						}
					}

					// Proceed to import the asset
					createdAssetFilename = MOG_ControllerAsset.CreateAsset(repositoryName, dirScope, tn.importFiles, null, props, false, false);
					if (createdAssetFilename == null)
					{
						// it's probably a network problem (TODO: Check for sure)

						// build a list of files for error message
						string files = "\n\nFiles contained in " + tn.Text + "\n";
						foreach (string fname in tn.importFiles)
							files += "\t" + fname + "\n";

						MOGPromptResult r = MOG_Prompt.PromptResponse("Import Error", "Importation of " + tn.FullPath + " failed.  Please ensure that the file is accessible and click Retry." + files, MOGPromptButtons.AbortRetryIgnore);
						if (r == MOGPromptResult.Retry)
						{
							--nodeIndex;		// stay on the same node (continue auto-increments)
							continue;
						}
						else if (r == MOGPromptResult.Abort)
						{
							RaiseAssetImport_Finish();
							MOG_Prompt.PromptResponse("Cancelled", "Importation Cancelled", Environment.StackTrace, MOGPromptButtons.OK, MOG_ALERT_LEVEL.MESSAGE);
							return;
						}
						else if (r == MOGPromptResult.Ignore)
						{
							continue;
						}
					}

					// Schedule this asset for posting under this project name
					MOG_ControllerProject.AddAssetForPosting(createdAssetFilename, MOG_ControllerProject.GetProjectName());
				}
				catch (Exception ex)
				{
					MOG_Report.ReportMessage("Create Asset", "Could not correctly create asset.\nMessage=" + ex.Message, ex.StackTrace, MOG_ALERT_LEVEL.CRITICAL);
					continue;
				}
			}

			// Shut off the properties cache
			MOG_Properties.ActivatePropertiesCache(false);
		}

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
			// encode
			this.tvClassifications.BeginUpdate();

			// Well, what an UGLY HACK!!!  We can thanks Bianchi here for having the data structure be a GUI object
			// Use our own temp TreeNode so we don't have to worry about the progress worker being in a different thread
			TreeNode tempNode = new TreeNode();
			// Copy over the GUI's nodes to our tempNode used in the encoding thread
			foreach (classTreeNode ctn in tvClassifications.Nodes)
			{
				ctn.Remove();
				tempNode.Nodes.Add(ctn);
			}
			// Ferform the encoding
			ProgressDialog progress = new ProgressDialog("Creating Asset Properties", "Please wait while the Properties for these assets are generated", EncodeAll_Worker, tempNode.Nodes, false);
			progress.ShowDialog();
		}

		private void EncodeAll_Worker(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			TreeNodeCollection nodes = e.Argument as TreeNodeCollection;

			// Well, what an UGLY HACK!!!  We can thanks Bianchi here for having the data structure be a GUI object
			// Establish our worker completed event so we can grab the encoded nodes
			worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(EncodeAll_WorkerCompleted);
			// Copy over the nodes to our tempTreeView so we can have the Node's FullPath work
			TreeView tempTreeView = new TreeView();
			foreach (classTreeNode ctn in nodes)
			{
				ctn.Remove();
				tempTreeView.Nodes.Add(ctn);
			}
			// Perform the encoding
			EncodeAll(tempTreeView.Nodes, worker);

			// Return the encoded nodes
			e.Result = tempTreeView.Nodes;
		}

		void EncodeAll_WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			TreeNodeCollection returnNodes = e.Result as TreeNodeCollection;

			// Well, what an UGLY HACK!!!  We can thanks Bianchi here for having the data structure be a GUI object
			// Push the modified returnNodes into the dialog within the worker completed so we are thread safe
			foreach (classTreeNode ctn in returnNodes)
			{
				ctn.Remove();
				tvClassifications.Nodes.Add(ctn);
			}

			this.tvClassifications.EndUpdate();
		}
		
		private bool EncodeAll(TreeNodeCollection nodes, BackgroundWorker worker)
		{
			foreach (classTreeNode ctn in nodes)
			{
				if (worker != null)
				{
					// Check for user initiated cancel
					if (worker.CancellationPending)
					{
						return false;
					}

					// Report progress to the user
					worker.ReportProgress(0, ctn.FullPath);
				}

				if (!ctn.FilledIn)
				{
					if (ctn.assetTreeNode != null)
					{
						// kill dummy node
						ctn.Nodes.Clear();

						// populate properties
						if (ctn.props == null)
						{
							// Get the classification's properties that we can edit
							ctn.props = MOG_Properties.OpenClassificationProperties(ctn.FullPath.Replace("\\", "~"));
							ctn.props.SetImmeadiateMode(true);
						}

						// add converted children
						foreach (AssetTreeNode atn in ctn.assetTreeNode.Nodes)
						{
							ctn.Nodes.Add(EncodeNode(atn, ctn.props));
						}

						// Setup SyncTargetPath Property of this classification
						// First, we need to build a complete list of all the files being imported in this classification
						ArrayList allImportFiles = new ArrayList();
						foreach (classTreeNode ttn in ctn.Nodes)
						{
							allImportFiles.AddRange(ttn.importFiles);
						}
						// Check if we have any contained items?
						if (allImportFiles.Count > 0)
						{
							// Scan for the highest common path amoung these files
							string assetDirectoryPath = MOG_ControllerAsset.GetCommonDirectoryPath(this.projectRootPath, allImportFiles);
							// Check if this command path is longer than the projectRootPath?
							if (assetDirectoryPath.Length > this.projectRootPath.Length)
							{
								// Trim off the projectRootPath to obtain our desiredSyncTargetPath
								string desiredSyncTargetPath = assetDirectoryPath.Substring(this.projectRootPath.Length + 1);
								// Add the detected SyncTargetPath to this classification
								ctn.props.SyncTargetPath = desiredSyncTargetPath;
							}
						}
						else
						{
							// build synctargetpath
							if (ctn.assetTreeNode.FileFullPath != "" &&			// "" indicates don't set sync data path
								ctn.assetTreeNode.FileFullPath != "<empty>")	// "<empty>" indicates don't set sync data path
							{
								string syncDataPath = ctn.assetTreeNode.FileFullPath;
								if (syncDataPath.ToLower().StartsWith(this.projectRootPath.ToLower()))
									syncDataPath = syncDataPath.Substring( this.projectRootPath.Length ).Trim("\\".ToCharArray() );

								// Check if the syncTargetPath was resolved to nothing?
								if (syncDataPath == "")
								{
									// Force the syncTargetPath to 'Nothing' so it will resemble the resolved syncDataPath and not simply inherit
									syncDataPath = "Nothing";
								}

								ctn.props.SyncTargetPath = syncDataPath;
							}
						}

						// mark this node as filled-in
						ctn.FilledIn = true;
						ctn.assetTreeNode = null;
					}
				}

				// recurse
				if (!EncodeAll(ctn.Nodes, worker))
				{
					return false;
				}
			}

			return true;
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
						MOG_Filename assetFilename = new MOG_Filename(ctn.FullPath);
						ctn.props = MOG_Properties.OpenAssetProperties(assetFilename);

						// Setup SyncTargetPath Property
						string assetDirectoryPath = MOG.CONTROLLER.CONTROLLERASSET.MOG_ControllerAsset.GetCommonDirectoryPath(this.projectRootPath, ctn.importFiles);
						if (assetDirectoryPath.Length > this.projectRootPath.Length)
						{
							string desiredSyncTargetPath = assetDirectoryPath.Substring(this.projectRootPath.Length + 1);
							ctn.props.SyncTargetPath = desiredSyncTargetPath;
						}

						// Make sure we trigger the propertyGrid to redisplay this new props
						this.propertyGrid.SelectedObject = ctn.props;
					}
					else
					{
						// Get the classification's properties that we can edit
						ctn.props = MOG_Properties.OpenClassificationProperties(ctn.FullPath);
						ctn.props.SetImmeadiateMode(true);

						// build synctargetpath
						if (ctn.assetTreeNode != null && 
							ctn.assetTreeNode.FileFullPath != "" &&			// "" indicates don't set sync data path
							ctn.assetTreeNode.FileFullPath != "<empty>")	// "<empty>" indicates don't set sync data path
						{
							string syncDataPath = ctn.assetTreeNode.FileFullPath;
							if (syncDataPath.ToLower().StartsWith(this.projectRootPath.ToLower()))
								syncDataPath = syncDataPath.Substring( this.projectRootPath.Length ).Trim("\\".ToCharArray() );

							// Check if the syncTargetPath was resolved to nothing?
							if (syncDataPath == "")
							{
								// Force the syncTargetPath to 'Nothing' so it will resemble the resolved syncDataPath and not simply inherit
								syncDataPath = "Nothing";
							}

							ctn.props.SyncTargetPath = syncDataPath;
						}

						// Make sure we trigger the propertyGrid to redisplay this new props
						this.propertyGrid.SelectedObject = ctn.props;
					}
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

				// Make list o' children
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

		private classTreeNode EncodeTree(AssetTreeNode tn, MOG_Properties props, int dialogId)
		{
			classTreeNode classNode = new classTreeNode(tn.Text);
			classNode.ImageIndex = BLUEDOT_INDEX;
			classNode.SelectedImageIndex = BLUEDOT_INDEX;

			// Get a local handle for our progress dialog
			MOG_Progress.ProgressUpdate(dialogId, tn.FullPath);
			
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
				classNode.Nodes.Add( EncodeTree(subNode, props, dialogId) );
			}

			return classNode;
		}
		#endregion
		#region Event handlers
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
		#endregion
	}
}


