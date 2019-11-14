using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

using MOG_ControlsLibrary.Common;
using MOG_ControlsLibrary.Common.MOG_ContextMenu;
using MOG;
using MOG.REPORT;
using MOG.FILENAME;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.PROMPT;
using MOG.PROJECT;
using MOG.PROPERTIES;

using MOG_ControlsLibrary.Controls;
using MOG_ControlsLibrary.Utils;
using System.ComponentModel;
using System.Collections.Generic;
using MOG_Client.Client_Utilities;
using MOG.CONTROLLER.CONTROLLERASSET;
using MOG.CONTROLLER.CONTROLLERLIBRARY;
using MOG.COMMAND;
using MOG.DOSUTILS;
using System.Text.RegularExpressions;
using MOG_CoreControls;
using System.IO;

namespace MOG_ControlsLibrary.Common.MogControl_RepositoryTreeViews
{
	/// <summary>
	/// Summary description for MogControl_LibraryTreeView.
	/// </summary>
	public class MogControl_LibraryTreeView : MogControl_PropertyClassificationTreeView
	{
		protected MogControl_AssetContextMenu mAssetContextMenu;
		public MogControl_LibraryExplorer LibraryExplorer = null;
		private BackgroundWorker mWorker;

		public MogControl_LibraryTreeView()
		{
			this.HideSelection = false;

			// Make sure we're populating on MOG_Property Library
			//this.MogPropertyList.Add(MOG.MOG_PropertyFactory.MOG_Classification_InfoProperties.New_IsLibrary( true ));

			// Add our context menu
			mAssetContextMenu = new MogControl_AssetContextMenu("NAME, USER, DATE, STATUS, FULLNAME", this);
			this.ContextMenuStrip = mAssetContextMenu.InitializeContextMenu("{LibraryTreeView}");

			// Go-go gadget drag-drop!
			this.AllowDrop = true;
			this.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(MogControl_LibraryTreeView_ItemDrag);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(MogControl_LibraryTreeView_DragEnter);
			this.DragOver += new DragEventHandler(MogControl_LibraryTreeView_DragOver);
			this.DragLeave += new EventHandler(MogControl_LibraryTreeView_DragLeave);
			this.DragDrop += new System.Windows.Forms.DragEventHandler(MogControl_LibraryTreeView_DragDrop);
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

			//Before we do anything here we need to make sure there is going to be a library classification
			string classification = MOG_Filename.GetProjectLibraryClassificationString();
			if (classification.Length > 0)
			{
				//Before we do anything here we need to make sure there is going to be a library classification
				MOG_Project project = MOG_ControllerProject.GetProject();
				if (project != null)
				{
					if (!project.ClassificationExists(classification))
					{
						//Oh no there's not one!  No big deal, just create it.
						if (project.ClassificationAdd(classification))
						{
							MOG_Properties props = MOG_Properties.OpenClassificationProperties(classification);
							if (props != null)
							{
								// This is a library
								props.IsLibrary = true;

								props.AssetIcon = "Images\\FileTypes\\MOG Library.bmp";
								props.ClassIcon = "Images\\FileTypes\\MOG Library Folder.bmp";

								// And we want to checkout all assets to a dir under the project name
								props.SyncTargetPath = "{AssetClassificationPath.Full}";
								props.Close();
							}
						}
					}
				}
			}
			mWorker = new BackgroundWorker();
			mWorker.DoWork += InitializeLibraryClassificationList_Worker;
			mWorker.RunWorkerCompleted += OnWorkerCompleted;

			mWorker.RunWorkerAsync();
		}

		private void InitializeLibraryClassificationList_Worker(object sender, DoWorkEventArgs e)
		{
			InitializeLibraryClassificationsList();
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
		
		protected void InitializeLibraryClassificationsList()
		{
			lock (mRequiredClassifications)
			{
				// Create a new list of required classifications and make sure the project's library is listed
				mRequiredClassifications = new SortedList();
				string libraryClassification = MOG_Filename.GetProjectLibraryClassificationString();
				if (libraryClassification.Length > 0)
				{
					mRequiredClassifications.Add(libraryClassification, new List<string>());
				}
			}
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
					//expand according to RootTreeType

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

			string thisClassification = node.FullPath;
			string thisClassificationPath = MOG_ControllerLibrary.ConstructPathFromLibraryClassification(thisClassification);

			// Check for any local directories
			if (thisClassificationPath.Length > 0)
			{
				DirectoryInfo[] directories = DosUtils.DirectoryGetList(thisClassificationPath, "*.*");
				if (directories != null && directories.Length > 0)
				{
					foreach (DirectoryInfo directory in directories)
					{
						// If we don't already have this classification, add it.
						if (!classificationsToAdd.Contains(directory.Name))
						{
							classificationsToAdd.Add(directory.Name);
						}
					}
				}
			}

			// Sort our classifications alphabetically
			classificationsToAdd.Sort();

			// Foreach classification, add it
			foreach (string classification in classificationsToAdd)
			{
				string fullClassification = MOG_Filename.JoinClassificationString(node.FullPath, classification);

				// Only add library classifications
				if (MOG_Filename.IsParentClassificationString(fullClassification, MOG_Filename.GetProjectLibraryClassificationString()))
				{
					TreeNode classificationNode = new TreeNode(classification);

					// Is this a non-MOG folder?
					if (!MOG_ControllerProject.IsValidClassification(fullClassification))
					{
						classificationNode.ForeColor = Color.LightGray;
					}				

					// Assign the default node checked state			
					classificationNode.Checked = node.Checked;

					classificationNode.Tag = new Mog_BaseTag(classificationNode, classification, RepositoryFocusLevel.Classification, false);
					((Mog_BaseTag)classificationNode.Tag).PackageNodeType = PackageNodeTypes.Class;
					node.Nodes.Add(classificationNode);

					classificationNode.Name = classificationNode.FullPath;
					SetImageIndices(classificationNode, GetClassificationImageIndex(classificationNode.FullPath));

					classificationNode.Nodes.Add(new TreeNode(Blank_Node_Text));
				}
			}

			EndUpdate();
		}

// JohnRen - Untested and need to finish before enabling
		//public void RefreshItem(MOG_Command command)
		//{
		//    // No reason to bother if they have no library working directory
		//    if (MOG_ControllerLibrary.GetWorkingDirectory().Length == 0)
		//    {
		//        return;
		//    }
		//
		//    // Make sure this contains an encapsulated command?
		//    MOG_Command encapsulatedCommand = command.GetCommand();
		//    if (encapsulatedCommand != null)
		//    {
		//        // No reason to bother if they are in a different project
		//        if (string.Compare(MOG_ControllerProject.GetProjectName(), encapsulatedCommand.GetProject(), true) != 0)
		//        {
		//            return;
		//        }
		//
		//        // Check if this encapsulatedCommand contains a valid assetFilename?
		//        if (encapsulatedCommand.GetAssetFilename().GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
		//        {
		//            // Check if this was a post command?
		//            if (encapsulatedCommand.GetCommandType() == MOG_COMMAND_TYPE.MOG_COMMAND_Post)
		//            {
		//                // Build our fullPath
		//                string thisFullPath = encapsulatedCommand.GetAssetFilename().GetAssetClassification();
		//                // Attempt to find this node in our tree
		//                TreeNode foundNode = FindNode(thisFullPath);
		//                if (foundNode != null)
		//                {
		//                    // Check if we made it all the way to our desire node?
		//                    if (string.Compare(foundNode.FullPath, thisFullPath, true) == 0)
		//                    {
		//                    }
		//                    // check if this node is expanded?
		//                    else if (foundNode.IsExpanded)
		//                    {
		//                        // Reinitialize the tree so this expanded tree node will be properly updated
		//                        Initialize();
		//                    }
		//                }
		//            }
		//        }
		//    }
		//}
	
		#region Drag drop functionality
		// Track which node we are dragging over currently so we can highlight it properly
		private TreeNode dragOverNode = null;

		private void MogControl_LibraryTreeView_ItemDrag(object sender, ItemDragEventArgs e)
		{
			if (SelectedNode != null)
			{
				DataObject data = new DataObject("LibraryTreeNode", SelectedNode.FullPath);
				DoDragDrop(data, DragDropEffects.All);
			}
		}
		
		public void MogControl_LibraryTreeView_DragEnter(object sender, DragEventArgs args)
		{
			// Accept only Windows FileDrops
			if (args.Data.GetDataPresent(DataFormats.FileDrop) ||
				args.Data.GetDataPresent("LibraryListItems") ||
				args.Data.GetDataPresent("LibraryTreeNode"))
			{
				args.Effect = args.AllowedEffect;
			}
			else
			{
				args.Effect = DragDropEffects.None;
			}
		}

		public void MogControl_LibraryTreeView_DragOver(object sender, DragEventArgs args)
		{
			if (args.Effect != DragDropEffects.None)
			{
				// Do highlighting as we drag over nodes

				// Get the node we're dragging over currently
				TreeNode tn = this.GetNodeAt(this.PointToClient(new Point(args.X, args.Y)));

				// Update colors if needs be
				if (tn != this.dragOverNode)
				{
					if (this.dragOverNode != null)
					{
						// Restore old node's original colors
						this.dragOverNode.BackColor = SystemColors.Window;
						this.dragOverNode.ForeColor = SystemColors.ControlText;
					}

					// save new node
					this.dragOverNode = tn;

					//highlight the new node
					if (dragOverNode != null)
					{
						this.dragOverNode.BackColor = SystemColors.Highlight;
						this.dragOverNode.ForeColor = SystemColors.HighlightText;
					}
				}
			}
		}
		
		public void MogControl_LibraryTreeView_DragLeave(object sender, EventArgs args)
		{
			// Make sure we kill our highlighting when the cursor goes out of our area
			if (this.dragOverNode != null)
			{
				// Restore old node's original colors
				this.dragOverNode.BackColor = SystemColors.Window;
				this.dragOverNode.ForeColor = SystemColors.ControlText;
			}
		}

		public void MogControl_LibraryTreeView_DragDrop(object sender, DragEventArgs args)
		{
			if (this.dragOverNode != null)
			{
				// Restore node's original colors
				this.dragOverNode.BackColor = SystemColors.Window;
				this.dragOverNode.ForeColor = SystemColors.ControlText;
			}

			// Get node we want to drop at
			TreeNode targetNode = this.GetNodeAt(this.PointToClient(new Point(args.X, args.Y)));
			// and select it so it'll show up in the ListView
			this.SelectedNode = targetNode;

			if (args.Data.GetDataPresent("FileDrop"))
			{
				// Extract the filenames and import
				string[] filenames = (string[])args.Data.GetData("FileDrop", false);
				if (filenames != null && filenames.Length > 0)
				{
					bool bCopyFiles = true;
					bool bAutoAddFiles = false;
					bool bPromptUser = false;
					bool bCancel = false;

					// Check if thes files are coming from the same spot?
					string classification = targetNode.FullPath;
					string classificationPath = MOG_ControllerLibrary.ConstructPathFromLibraryClassification(classification);
					// Get the common directory scope of the items
					ArrayList items = new ArrayList(filenames);
					string rootPath = MOG_ControllerAsset.GetCommonDirectoryPath("", items);
					if (rootPath.StartsWith(classificationPath))
					{
						bCopyFiles = false;
					}

					// Check if auto import is checked?
					if (this.LibraryExplorer.IsAutoImportChecked())
					{
						// Automatically add the file on the server
						bAutoAddFiles = true;
						bPromptUser = true;

						// Check if these files are already within the library?
						if (MOG_ControllerLibrary.IsPathWithinLibrary(rootPath))
						{
							// Ignore what the user specified and rely on the classification generated from the filenames
							classification = "";
							bPromptUser = false;
							bCopyFiles = false;
						}
					}

					// Promt the user for confirmation before we import these files
					if (bPromptUser)
					{
						// Prompt the user and allow them to cancel
						if (LibraryFileImporter.PromptUserForConfirmation(filenames, classification) == false)
						{
							bCancel = true;
						}
					}

					// Make sure we haven't canceled
					if (!bCancel)
					{
						if (bCopyFiles)
						{
							// Import the files
							List<object> arguments = new List<object>();
							arguments.Add(filenames);
							arguments.Add(classification);
							ProgressDialog progress = new ProgressDialog("Copying Files", "Please wait while the files are copied", LibraryFileImporter.CopyFiles, arguments, true);
							progress.ShowDialog();
						}
					}

					// Make sure we haven't canceled
					if (!bCancel)
					{
						if (bAutoAddFiles)
						{
							// Import the files
							List<object> arguments = new List<object>();
							arguments.Add(filenames);
							arguments.Add(classification);
							ProgressDialog progress = new ProgressDialog("Copying Files", "Please wait while the files are copied", LibraryFileImporter.ImportFiles, arguments, true);
							progress.ShowDialog();
						}
					}

					// Refresh view
					DeInitialize();
					Initialize();
				}
			}
			else if (args.Data.GetDataPresent("LibraryListItems"))
			{
				ArrayList items = args.Data.GetData("LibraryListItems") as ArrayList;

				foreach (string item in items)
				{
					// Move library asset here
					MOG_Filename assetName = new MOG_Filename(item);
					// Check if this was an asset?
					if (assetName.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
					{
						bool success = MOG_ControllerProject.GetProject().AssetRename(assetName.GetAssetFullName(), SelectedNode.FullPath + assetName.GetAssetName());
						// Make sure we unsync this asset just in case it had already been synced
						MOG_ControllerLibrary.Unsync(assetName);
					}
					// Check if this was a file?
					else if (DosUtils.FileExistFast(item))
					{
						string dstPath = MOG_ControllerLibrary.ConstructPathFromLibraryClassification(SelectedNode.FullPath);
						string dstTarget = Path.Combine(dstPath, Path.GetFileName(item));
						DosUtils.FileMoveFast(item, dstTarget, true);
					}
				}
			}
			else if (args.Data.GetDataPresent("LibraryTreeNode"))
			{
				string classification = args.Data.GetData("LibraryTreeNode") as string;

				if (classification != null && classification.Length > 0)
				{
					//Move classification here
					string[] parts = classification.Split("~".ToCharArray());
					if (parts.Length > 0)
					{
						string lastPart = parts[parts.Length - 1];

						bool success = MOG_ControllerProject.GetProject().ClassificationRename(classification, SelectedNode.FullPath + "~" + lastPart);
					}
					else
					{
						MOG_Prompt.PromptResponse("Cannot move classification", "MOG was unable to move the classification", Environment.StackTrace, MOGPromptButtons.OK);
					}
				}
			}
		}

		#endregion
	}
}
