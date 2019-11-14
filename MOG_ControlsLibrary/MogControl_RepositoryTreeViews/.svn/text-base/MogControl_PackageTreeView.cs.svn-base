using System;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using System.Text.RegularExpressions;
using System.ComponentModel;

using MOG;
using MOG.FILENAME;
using MOG.DATABASE;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERREPOSITORY;
using MOG.CONTROLLER.CONTROLLERASSET;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.PLATFORM;
using MOG.TIME;
using MOG.PROMPT;
using MOG.REPORT;

using MOG_ControlsLibrary.Forms;
using MOG_ControlsLibrary.Utils;
using MOG.CONTROLLER.CONTROLLERPACKAGE;

namespace MOG_ControlsLibrary.Controls
{
	/// <summary>
	/// Summary description for MogControl_PackageTreeView.
	/// </summary>
	public class MogControl_PackageTreeView : MogControl_PropertyClassificationTreeView
	{
		private BackgroundWorker mWorker;

		private bool mbAllowItemDrag;
		public ArrayList CreatedPackages;
		/// <summary>
		/// Decides whether or not we want to allow the user to drag our nodes
		/// </summary>
		[Category("Behavior"), Description("Decides whether or not we want to allow the user to drag our nodes.")]
		public bool AllowItemDrag
		{
			get { return mbAllowItemDrag; }
			set { mbAllowItemDrag = value; }
		}

		[Category("Behavior"), Description("Occures after a new package is created.")]
		public EventHandler AfterPackageCreate;

		private void InitializeComponent()
		{
			// 
			// MogControl_PackageTreeView
			// 
			FocusForAssetNodes = MOG_ControlsLibrary.Controls.LeafFocusLevel.PackageGroup;
			AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(MogControl_PackageTreeView_AfterLabelEdit);
			ItemDrag += new System.Windows.Forms.ItemDragEventHandler(MogControl_PackageTreeView_ItemDrag);
		}

		public MogControl_PackageTreeView()
		{
			InitializeComponent();

			// Initialize our list of newly created packages
			CreatedPackages = new ArrayList();

			MogPropertyList.Add(MOG.MOG_PropertyFactory.MOG_Asset_InfoProperties.New_IsPackage(true));
		}

		public override void Initialize()
		{
			Initialize(OnInitializeDone);
		}

		public override void Initialize(MethodInvoker OnInitializeDone)
		{
			this.OnInitializeDone = OnInitializeDone;
			
			UseWaitCursor = true;

			Nodes.Clear();

			Nodes.Add("Populating...");

			mWorker = new BackgroundWorker();
			mWorker.DoWork += InitializeClassificationList_Worker;
			mWorker.RunWorkerCompleted += OnWorkerCompleted;

			mWorker.RunWorkerAsync();
		}

		private void InitializeClassificationList_Worker(object sender, DoWorkEventArgs e)
		{
			InitializeClassificationsList();
		}

		private void OnWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			base.Initialize();

			if (OnInitializeDone != null)
			{
				OnInitializeDone();
			}

			UseWaitCursor = false;
		}

		/// <summary>
		/// Prepare a drag object to recieve any driped items from the package tree
		/// </summary>
		public DataObject GetItemDragEventDataObject()
		{
			if (Nodes.Count > 0)
			{
				Mog_BaseTag tag = SelectedNode.Tag as Mog_BaseTag;
				if (tag != null)
				{
					string packageFullPath = tag.PackageFullName;

					ArrayList packages = new ArrayList();
					packages.Add(packageFullPath);

					// Create a new Data object for the send
					return new DataObject("Package", packages);
				}
			}

			return null;
		}

		/// <summary>
		/// Remove a Group or Package Object from the database
		/// </summary>
		/// <param name="removeCandidate">Full path of Group or Object with 
		/// '/' as delimiter, starting from package name.</param>
		/// <param name="packageAsset">The Asset that is the Package</param>
		/// <param name="platformGeneric">
		/// Whether or not this is a platform Generic operation.  
		/// 
		/// This is where the "Show Platform Specific" checkbox would 
		/// plug in from the PackageManagementTreeView
		/// </param>
		/// <returns>Bool indicating success/failure</returns>
		public bool RemoveGroupFromDatabase(string removeCandidate, MOG_Filename packageAsset)
		{
			try
			{
				bool success = true;

				// Get the current version of this package
				string packageVersion = MOG_DBAssetAPI.GetAssetVersion(packageAsset);

				// Check to see if any assets reference this
				if (MOG_DBPackageAPI.GetAllAssetsInPackageGroup(packageAsset, packageVersion, removeCandidate).Count == 0)
				{
					// If all is ok, remove it from the database
					success &= MOG_DBPackageAPI.RemovePackageGroupName(packageAsset, packageVersion, removeCandidate, MOG_ControllerProject.GetUser().GetUserName());
				}
				else
				{
					throw (new Exception("Cannot remove object or group that is used by active assets!"));
				}

				// Are we platform generic?
				if (String.Compare(packageAsset.GetAssetPlatform(), "All", true) == 0)
				{
					// We are platform generic, loop through all platforms then
					ArrayList platforms = MOG_ControllerProject.GetProject().GetPlatforms();
					for (int p = 0; p < platforms.Count; p++)
					{
						MOG_Platform platform = (MOG_Platform)platforms[p];

						// Set this package to be platform specific for this platform name
						packageAsset = MOG_Filename.CreateAssetName(packageAsset.GetAssetClassification(), platform.mPlatformName, packageAsset.GetAssetLabel());
						packageVersion = MOG_DBAssetAPI.GetAssetVersion(packageAsset);
						if (packageVersion.Length > 0)
						{
							// Check to see if any assets reference this
							if (MOG_DBPackageAPI.GetAllAssetsInPackageGroup(packageAsset, packageVersion, removeCandidate).Count == 0)
							{
								// If all is ok, remove it from the database
								success &= MOG_DBPackageAPI.RemovePackageGroupName(packageAsset, packageVersion, removeCandidate, MOG_ControllerProject.GetUser().GetUserName());
							}
							else
							{
								throw (new Exception("Cannot remove object or group that is used by active assets!"));
							}
						}
					}
				}

				return success;
			}
			catch (Exception e)
			{
				// Get the current version of this package
				string packageVersion = MOG_DBAssetAPI.GetAssetVersion(packageAsset);

				// See if we can report to the user about why this node could not be deleted
				ArrayList assignedAssets = MOG_DBPackageAPI.GetAllAssetsInPackageGroup(packageAsset, packageVersion, removeCandidate);

				if (assignedAssets != null)
				{
					// Walk all associated assets and make a list
					string assets = "";
					foreach (MOG_Filename assetName in assignedAssets)
					{
						if (assets.Length == 0)
						{
							assets = assetName.GetEncodedFilename();
						}
						else
						{
							assets = assets + "\n" + assetName.GetEncodedFilename();
						}
					}

					// Tell the user
					MOG_Prompt.PromptMessage("Remove node", "Cannot remove this node because the following assets are assigned to it:\n\n" + assets);
				}
				else
				{
					// This must have been another problem
					MOG_Prompt.PromptMessage("Remove node", "Cannot remove this node.\nMessage\n" + e.Message);
				}
			}

			return false;
		}

		/// <summary>
		/// Add group or object to the database
		/// </summary>
		private bool AddGroupToDatabase(string addCandidate, MOG_Filename packageAsset)
		{
			// Are we platform generic?
			bool success = true;

			if (packageAsset.GetAssetPlatform() == "All")
			{
				string packageVersion = MOG_DBAssetAPI.GetAssetVersion(packageAsset);
				if (packageVersion.Length > 0)
				{
					success = MOG_DBPackageAPI.AddPackageGroupName(packageAsset, packageVersion, addCandidate, MOG_ControllerProject.GetUser().GetUserName());
				}
			}

			// We are platform generic, loop through all platforms then
			ArrayList platforms = MOG_ControllerProject.GetProject().GetPlatforms();
			for (int p = 0; p < platforms.Count && success; p++)
			{
				MOG_Platform platform = (MOG_Platform)platforms[p];

				// Set this package to be platform specific for this platform name
				packageAsset = MOG_Filename.CreateAssetName(packageAsset.GetAssetClassification(), platform.mPlatformName, packageAsset.GetAssetLabel());
				string packageVersion = MOG_DBAssetAPI.GetAssetVersion(packageAsset);
				if (packageVersion.Length > 0)
				{
					// Add to the database
					success &= MOG_DBPackageAPI.AddPackageGroupName(packageAsset, packageVersion, addCandidate, MOG_ControllerProject.GetUser().GetUserName());
				}
			}

			return success;
		}

		private bool CheckLabelEdit(NodeLabelEditEventArgs e)
		{
			// Make sure that the node was actually edited.  If not, make them do it again...
			if (e.Label == null)
			{
				e.CancelEdit = true;
				// Show our MessageBox and allow user to decide whether they want to continue editting or not
				MOGPromptResult result = MOG_Prompt.PromptResponse("Create new node", "It seems you have entered an invalid name, try again?", MOGPromptButtons.YesNo);

				// If user is telling us they want to cancel the edit...
				if (result == MOGPromptResult.No)
				{
					e.Node.Remove();
				}
				// Else, try again
				else
				{
					e.Node.BeginEdit();
				}

				return false;
			}
			return true;
		}

		/// <summary>
		/// Utility method to make sure we are not duplicating an existing node
		/// </summary>
		private bool ValidateNewNodeName(NodeLabelEditEventArgs e, MOG_Filename packageName)
		{
			// Foreach node in our Parent's nodes, see if we already have the AssetName we propose to add
			foreach (TreeNode node in e.Node.Parent.Nodes)
			{
				// If we find the AssetName we propose to add, warn the user and quit...
				if (node.Text.ToLower() == packageName.GetAssetName().ToLower())
				{
					MessageBox.Show(this, "Package name, " + packageName.GetAssetName() + ", already exists!", "Package Already Exists!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					e.Node.EndEdit(true);
					e.Node.Remove();
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Create Package for each platform in this Project
		/// </summary>
		private MOG_Filename CreatePackageForPlatform(NodeLabelEditEventArgs e, MOG_Filename packageName, TreeNode parent)
		{
			// Initialize our ArrayList for MOG_Property objects
			ArrayList targetSyncPath = null;

			// If our node name already exists, return...
			if (!ValidateNewNodeName(e, packageName))
			{
				return null;
			}

			// If our targetSyncPath is null, even after we try to get a valid one, return...
			if (targetSyncPath == null &&
				(targetSyncPath = GetValidTargetSyncPath(e, packageName)) == null)
			{
				return null;
			}

            MOG_Property syncTargetPathProperty = targetSyncPath[0] as MOG_Property;

            // Create the new package
            MOG_Filename createdPackageFilename = MOG_ControllerProject.CreatePackage(packageName, syncTargetPathProperty.mValue);
			if (createdPackageFilename == null)
			{
				return null;
			}

            RenameNode(e, createdPackageFilename);

			// Post the projects new assets
			string jobLabel = "NewPackageCreated." + MOG_ControllerSystem.GetComputerName() + "." + MOG_Time.GetVersionTimestamp();
			MOG_ControllerProject.PostAssets(MOG_ControllerProject.GetProjectName(), MOG_ControllerProject.GetBranchName(), jobLabel);

			// Return our created package
			return createdPackageFilename;
		}

		private void RenameNode(NodeLabelEditEventArgs e, MOG_Filename repositoryName)
		{
			TreeNode parent = e.Node.Parent;

			e.Node.Text = repositoryName.GetAssetName();

			e.Node.Tag = new Mog_BaseTag(e.Node, repositoryName.GetEncodedFilename(), RepositoryFocusLevel.Repository, true);
			((Mog_BaseTag)e.Node.Tag).PackageNodeType = PackageNodeTypes.Asset;
			((Mog_BaseTag)e.Node.Tag).PackageFullName = repositoryName.GetAssetFullName();

			e.CancelEdit = true;
		}

		/// <summary>
		/// Utility method for the Package portion of AfterLabelEdit() (above)
		/// </summary>
		private ArrayList GetValidTargetSyncPath(NodeLabelEditEventArgs e, MOG_Filename packageName)
		{
			// Get the gameDataPath for this new package
			GameDataPathForm getPath = new GameDataPathForm(packageName.GetAssetPlatform(), packageName.GetAssetName());
			ArrayList targetSyncPath = null;

			// This dialog should be pushed to topmost when it doesn't have a parent or else it can ger lost behind other apps.
			// You would think this is simple but for some reason MOG has really struggled with these dialogs being kept on top...
			// We have tried it all and finally ended up with this...toggling the TopMost mode seems to be working 100% of the time.
			getPath.TopMost = true;
			getPath.TopMost = false;
			getPath.TopMost = true;
			// Show the dialog
			if (DialogResult.Cancel != getPath.ShowDialog())
			{

				targetSyncPath = getPath.MOGPropertyArray;

				// Get a value for our relative gamedata path
				string relativePath = null;
				if (targetSyncPath.Count > 0)
				{
					relativePath = ((MOG_Property)targetSyncPath[0]).mPropertyValue;
				}
				// If we found our relativePath...
				if (relativePath != null)
				{
					// Go ahead and add an initial backslash, if necessary...
					if (relativePath.Length > 0 && relativePath[0] != '\\')
					{
						relativePath = "\\" + relativePath;
					}

					// Find all our assets with this gamedata path
					ArrayList assets = MOG_DBAssetAPI.GetAllAssetsBySyncLocation(relativePath, packageName.GetAssetPlatform());
					// If we found assets...
					if (assets != null && assets.Count > 0)
					{
						// Check and see if any of the assets we found match the package we want to add...
						foreach (MOG_Filename asset in assets)
						{
							// If we already have the package name we want to add, warn user and quit...
							if (asset.GetAssetName().ToLower() == packageName.GetAssetName().ToLower())
							{
								MessageBox.Show(this, "Package name, " + packageName.GetAssetName() + ", already exists in location, '" + relativePath.Substring(1) + "'!", "Package Already Exists!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
								e.Node.EndEdit(true);
								e.Node.Remove();
								return null;
							}
						}
					}
				}
			}

			// Finished our method, so return true
			return targetSyncPath;
		}

		private ArrayList GetPackageLabelEditPlatforms(NodeLabelEditEventArgs e, ref string nodeText)
		{
			ArrayList packagePlatforms = new ArrayList();

			if (e.Node.Text.IndexOf("}") > -1)
			{
				// Search for one `{` + zero-or-more word characters + `}`
				string searchString = @"\{\w*\}";
				// Store our platforms
				MatchCollection matches = Regex.Matches(e.Node.Text, searchString);
				foreach (Match match in matches)
				{
					// Add our platforms without the brackets
					packagePlatforms.Add(Regex.Replace(match.Value, @"(?:\{|\})", ""));
				}

				// Search for and replace any comma-space combinations that come before a '{'
				nodeText = Regex.Replace(e.Node.Text, @"\, (?=\{)", "");
				// Get rid of ellipsis added by Kier
				nodeText = nodeText.Replace("...", "");

				// Get rid of our platforms
				nodeText = Regex.Replace(nodeText, searchString, "");
			}

			return packagePlatforms;
		}

		private string GetQualifiedGroupName(NodeLabelEditEventArgs e, TreeNode packageNode)
		{
			e.Node.Tag = new Mog_BaseTag(e.Label, ((Mog_BaseTag)packageNode.Tag).FullFilename, LeafFocusLevel.PackageGroup, true);

			// If this is a Package Object...
			if (e.Node.Text.IndexOf("(") > -1 && e.Node.Text.IndexOf(")") > -1)
			{
				e.Node.Text = "(" + e.Label + ")";
				((Mog_BaseTag)e.Node.Tag).PackageNodeType = PackageNodeTypes.Object;
			}
			// Else this is a Group Node...
			else
			{
				e.Node.Text = e.Label;
				((Mog_BaseTag)e.Node.Tag).PackageNodeType = PackageNodeTypes.Group;
			}

			string groupName = e.Node.FullPath.Substring(packageNode.FullPath.Length).Trim(PathSeparator.ToCharArray());
			groupName = groupName.Replace(PathSeparator, "/");

			// Assign the package path
			((Mog_BaseTag)e.Node.Tag).PackageFullPath = groupName;

			// Assign the full package name
			((Mog_BaseTag)e.Node.Tag).PackageFullName = packageNode.FullPath + "/" + groupName;

			return groupName;
		}

		private void MogControl_PackageTreeView_AfterLabelEdit(object sender, System.Windows.Forms.NodeLabelEditEventArgs e)
		{
			// Encapsulate all of this in a try-catch, since we don't want to crash MOG
			try
			{
				if (!CheckLabelEdit(e))
				{
					return;
				}

				// Disable further label editing
				e.Node.TreeView.LabelEdit = false;

				// Find our parent package
				TreeNode package = FindPackage(e.Node);

				TreeNode parent = e.Node.Parent;

				string nodeText = e.Node.Text.ToLower();
				ArrayList packagePlatforms = GetPackageLabelEditPlatforms(e, ref nodeText);

				// Switch base on what our nodeText was before the user editted it
				switch (nodeText.ToLower())
				{
				case "newgroup":
					#region new-group
					if (package != null)
					{
						// Get the new edited label name
						//string groupName = e.Label;

						MOG_Filename packageAsset = new MOG_Filename(((Mog_BaseTag)package.Tag).FullFilename);

						string groupName = GetQualifiedGroupName(e, package);

						// If we have a duplicate, do not add it
						if (ValidateNodeIsNotDuplicate(e.Node.Parent, groupName) == true)
						{
							MOG_Prompt.PromptResponse("Duplicate Entry!", "The group, " + groupName + ", "
								+ "already exists!  Group not added.");
							e.Node.Remove();
							return;
						}

						// Add group to Database
						if (!AddGroupToDatabase(groupName, packageAsset))
						{
							// We need to clean up the unsuccessfull add to the database
							e.Node.Remove();

							// Show error
							MOG_Prompt.PromptMessage("Create group", "We were unable to add this group to the database. Aborting");
						}

						AttachValidatedTagToNewObjectOrGroup(package, e.Node, PackageNodeTypes.Group);
						// Go ahead and select our node
						e.Node.TreeView.SelectedNode = e.Node;
					}
					#endregion newgroup
					break;
				case "(newpackageobject)":
					#region new-package-object
					if (package != null)
					{
						// Get the new edited label name
						string objectName = e.Label;
						e.Node.Text = "(" + objectName + ")";

						// If we have a duplicate, do not add it
						if (ValidateNodeIsNotDuplicate(e.Node.Parent, e.Node.Text) == true)
						{
							MOG_Prompt.PromptResponse("Duplicate Entry!", "The object, (" + objectName + "), "
								+ "already exists!  Package object not added.");
							e.Node.Remove();
							return;
						}

						MOG_Filename packageAsset = new MOG_Filename(((Mog_BaseTag)package.Tag).FullFilename);

						string groupPath = GetQualifiedGroupName(e, package);

						// Add group to Database
						if (!AddGroupToDatabase(groupPath, packageAsset))
						{
							// We need to clean up the unsuccessfull add to the database
							e.Node.Remove();

							// Show error
							MOG_Prompt.PromptMessage("Create object", "We were unable to add this object to the database. Aborting");
						}
						else
						{
							e.CancelEdit = true;
							AttachValidatedTagToNewObjectOrGroup(package, e.Node, PackageNodeTypes.Object);
							// Go ahead and select our node
							e.Node.TreeView.SelectedNode = e.Node;
						}
					}
					#endregion new-package-object
					break;
				case "newpackage":
					#region new-package
					// Get the new package name
					string nodeFullname = MOG_Filename.JoinClassificationString(e.Node.FullPath, e.Label);

					// Construct a valid filename
					MOG_Filename[] packages = new MOG_Filename[packagePlatforms.Count];
					bool duplicateExists = false;
					string duplicates = "";
					for (int i = 0; i < packages.Length && i < packagePlatforms.Count; ++i)
					{
						MOG_Filename packageName = MOG_Filename.CreateAssetName(e.Node.Parent.FullPath, (string)packagePlatforms[i], e.Label);
						packages[i] = packageName;
						// If we have a duplicate package, store 
						if ((duplicateExists |= ValidateNodeIsNotDuplicate(e.Node.Parent, packageName.GetAssetName())) == true)
						{
							duplicates += packageName + "\r\n";
						}
					}

					// If we had duplicates, warn user and exit
					if (duplicateExists)
					{
						MOG_Prompt.PromptResponse("Duplicate Entry(ies) Detected!", "The following were already exist:\r\n\r\n"
							+ duplicates);
						e.Node.Remove();
						return;
					}

					// If we did not get a ValidPackageName (or the user decided to abort...)
					foreach (MOG_Filename packageName in packages)
					{
						if (!ValidatePackageExtension(e, packageName))
						{
							return;
						}
					}

					bool problemAdding = false;
					string packageErrorPrompt = "Did not complete package add for the following: \r\n";

					// Go backwards through our packages (so we can get the right platforms.
					for (int i = packages.Length - 1; i > -1; --i)
					{
						MOG_Filename packageName = packages[i];
						// Did we get a valid package name?
						if (packageName != null)
						{
							// Create the package
							MOG_Filename createdPackage = CreatePackageForPlatform(e, packageName, parent);

							// If the user cancelled some part of the process or we had an error...
							if (createdPackage == null)
							{
								// Remove the node
								packageErrorPrompt += "\t" + packageName + ".";
								problemAdding = true;
								break;
							}


							TreeNode newPackageNode;

							// For our first platform, we've already got a node, use it...
							if (i == 0)
							{
								e.Node.Text = createdPackage.GetAssetName();
								e.Label.ToString();
								newPackageNode = e.Node;
							}
							// For subsequent platforms, add a new node...
							else
							{
								newPackageNode = e.Node.Parent.Nodes.Add(createdPackage.GetAssetName());
							}

							// Now that we've got our initial information, add our tag
							newPackageNode.Tag = new Mog_BaseTag(newPackageNode, createdPackage.GetEncodedFilename(), this.FocusForAssetNodes, true);
							((Mog_BaseTag)newPackageNode.Tag).PackageNodeType = PackageNodeTypes.Package;
							((Mog_BaseTag)newPackageNode.Tag).PackageFullName = createdPackage.GetAssetFullName();
							SetImageIndices(newPackageNode, GetAssetFileImageIndex(createdPackage.GetEncodedFilename()));
						}
					}

					// If we had a problem, let the user know and remove our package
					if (problemAdding)
					{
						MOG_Prompt.PromptMessage("Unable to Add Package(s)",
							packageErrorPrompt);
						e.Node.Remove();
						foreach (MOG_Filename packageName in packages)
						{
							RemovePackage(packageName);
						}
					}
					else // Else, we were OK, so go ahead and select this node...
					{
						e.Node.TreeView.SelectedNode = e.Node;

						foreach (MOG_Filename packageName in packages)
						{
							// Add this package to the list of newly added packages
							this.CreatedPackages.Add(packageName);
						}

						// Fire the after package create event
						if (AfterPackageCreate != null)
						{
							AfterPackageCreate(this, EventArgs.Empty);
						}
					}
					#endregion new-package
					break;
				}
			}
			catch (Exception ex)
			{
				MOG_Report.ReportMessage("TreeNode Error!", "Error committing change to package, group, or package object label:\n" + ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.ERROR);
				if (e.Node != null)
				{
					e.Node.Remove();
				}
			}
		}

		/// <summary>
		/// Checks node.Parent.Nodes to see if the node we have added is a duplicate of an existing node
		/// </summary>
		/// <param name="node"></param>
		/// <returns>`true` if node is duplicate</returns>
		private bool ValidateNodeIsNotDuplicate(TreeNode parentNode, string testString)
		{
			if (parentNode != null)
			{
				int count = 0;
				// Foreach node in Parent.Nodes, see if we have a match
				foreach (TreeNode node in parentNode.Nodes)
				{
					// If we have a match, increment count
					if (testString.ToLower() == node.Text.ToLower())
					{
						++count;
					}
					// If we have counted more than once, we have a dup...
					if (count > 1)
					{
						return true;
					}
				}
			}
			// We made it through everything, we have no dup
			return false;
		}

		/// <summary>
		/// Remove a Package from the PackageManagement Tree. 
		///  Adapted from MogControl_AssetContextMenu.cs::MenuItemRemoveFromProject_Click()
		/// </summary>
		private void RemovePackage(MOG_Filename packageName)
		{
			try
			{
				if (!MOG_ControllerProject.RemoveAssetFromProject(packageName, "No longer needed", false))
				{
					return;
				}
			}
			catch (Exception ex)
			{
				MOG_Report.ReportMessage("Remove From Project", ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
			}
		}

		private void AttachValidatedTagToNewObjectOrGroup(TreeNode package, TreeNode newNode, PackageNodeTypes nodeType)
		{
			// If this thing is a Group or Object, we need to do this algorithm
			string fullFilename = null;
			MOG_Filename assetFile = null;

			int imageIndex = 0;

			switch (nodeType)
			{
			case PackageNodeTypes.Group:
				imageIndex = MogUtil_AssetIcons.GetClassIconIndex(PackageGroup_ImageText);
				break;
			case PackageNodeTypes.Object:
				imageIndex = MogUtil_AssetIcons.GetClassIconIndex(PackageObject_ImageText);
				break;
			default:
				MOG_Report.ReportSilent("Got Unexpected PackageNodeTypes",
					"Unexpected PackageNodeTypes enum given for MOG_ControlsLibrary.Controls",
					Environment.StackTrace);
				break;
			}

			string groupPath = newNode.FullPath.Substring(package.FullPath.Length).Trim(PathSeparator.ToCharArray()).Replace(PathSeparator, "/");
			fullFilename = package.FullPath + "/" + groupPath;
			assetFile = new MOG_Filename(((Mog_BaseTag)package.Tag).FullFilename);

			// Now that we've got our initial information, add our tag
			newNode.Tag = new Mog_BaseTag(newNode, assetFile.GetEncodedFilename(), this.FocusForAssetNodes, true);
			((Mog_BaseTag)newNode.Tag).PackageNodeType = nodeType;
			((Mog_BaseTag)newNode.Tag).PackageFullName = fullFilename;
			SetImageIndices(newNode, imageIndex);
		}

		/// <summary>
		/// Utility method to make sure we have a valid Package name (which should have an extension)
		/// </summary>
		private bool ValidatePackageExtension(NodeLabelEditEventArgs e, MOG_Filename packageName)
		{
			// Make sure packages have extensions, if not make strong warning
			if (packageName.GetExtension().Length == 0)
			{
				string message = "This package does not have an extension!\r\n"
					+ "Most engines require extensions on packages.\r\n\r\n"
					+ "(Click 'Ignore' to continue without adding an extension)";
				switch (MOG_Prompt.PromptResponse("Create new package", message, MOGPromptButtons.AbortRetryIgnore))
				{
				case MOGPromptResult.Retry:
					e.CancelEdit = true;
					e.Node.TreeView.LabelEdit = true;
					e.Node.BeginEdit();
					return false;
				case MOGPromptResult.Abort:
					e.Node.Remove();
					e.CancelEdit = true;
					return false;
				}
			}

			// We finished the block.  Return true.
			return true;
		}

		private void MogControl_PackageTreeView_ItemDrag(object sender, System.Windows.Forms.ItemDragEventArgs e)
		{
			DataObject send = GetItemDragEventDataObject();

			if (send != null)
			{
				// Fire the DragDrop event
				DragDropEffects dde1 = DoDragDrop(send, DragDropEffects.Copy);
			}
		}

	}
}
