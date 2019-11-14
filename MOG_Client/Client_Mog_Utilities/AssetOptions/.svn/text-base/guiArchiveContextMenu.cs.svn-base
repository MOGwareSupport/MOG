using System;
using System.IO;
using System.Windows.Forms;
using System.Collections;

using MOG;
using MOG.INI;
using MOG.TIME;
using MOG.PROJECT;
using MOG.PROPERTIES;
using MOG.PLATFORM;
using MOG.REPORT;
using MOG.PROMPT;
using MOG.FILENAME;
using MOG.DOSUTILS;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG_Client.Client_Mog_Utilities;
using MOG_ControlsLibrary;
using MOG_ControlsLibrary.Common.MogControl_RepositoryTreeViews;
using MOG.CONTROLLER.CONTROLLERSYSTEM;

namespace MOG_Client.Client_Mog_Utilities.AssetOptions
{
	/// <summary>
	/// Summary description for guiAssetContextMenu.
	/// </summary>
	public class guiArchiveContextMenu
	{		
		public delegate void MogMenuItem_Click(object sender, System.EventArgs e);

		private ListView mView;
		private TreeView mTree;

		public ListView SetView { get{return mView;} set{mView = value; mTree = null;}}
		public TreeView SetTree { get{return mTree;} set{mTree = value; mView = null;}}

		#region Constructors
		public guiArchiveContextMenu()
		{
			mView = null;
			mTree = null;
		}

		public guiArchiveContextMenu(ListView view)
		{
			mView = view;
			mTree = null;
		}

		public guiArchiveContextMenu(TreeView tree)
		{
			mView = null;
			mTree = tree;
		}
		#endregion
	
		public void InitializeItems(MenuItem Item)
		{
			Item.MenuItems.Add(CreateItem("Remove", new MogMenuItem_Click(RemoveMenuItem_Click)));
			Item.MenuItems.Add(CreateItem("Make Current", new MogMenuItem_Click(MakeCurrentMenuItem_Click)));

			MenuItem RebuildItem = CreateItem("Rebuild Package", null);
			
			// Add a menu item for each platform
			ArrayList platforms = MOG_ControllerProject.GetProject().GetPlatforms();
			for (int p = 0; p < platforms.Count; p++)
			{
				MOG_Platform platform = (MOG_Platform)platforms[p];
				RebuildItem.MenuItems.Add(CreateItem(platform.mPlatformName, new MogMenuItem_Click(RebuildPackageMenuItem_Click)));
			}
			RebuildItem.MenuItems.Add(CreateItem("All", new MogMenuItem_Click(RebuildPackageMenuItem_Click)));

			// Add this complex menu
			Item.MenuItems.Add(RebuildItem);
		}

		private MenuItem CreateItem(string name, MogMenuItem_Click method)
		{
			MenuItem Item = new MenuItem(name);
			if (method != null)
			{
				Item.Click += new System.EventHandler(method);
			}
			return Item;
		}


		#region UTILITY FUNCTIONS
		/// <summary>
		/// Locates a string in an array and returns its index
		/// </summary>
		/// <param name="cols"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		private int ColumnNameFind(string []cols, string name)
		{
			for (int x = 0; x < cols.Length; x++)
			{
				if (string.Compare(cols[x], name, true) == 0)
				{
					return x;
				}
			}

			return -1;
		}

		private ArrayList ControlGetSelectedItems()
		{
			ArrayList nodes = new ArrayList();

			// Get the list nodes
			if (mView != null && mTree == null)
			{
				string []columnNames = (string[])mView.Tag;

				foreach (ListViewItem item in mView.SelectedItems)
				{
					string text = item.SubItems[ColumnNameFind(columnNames, "Fullname")].Text;

					guiAssetTreeTag Tag = new guiAssetTreeTag(text, guiAssetTreeTag.TREE_FOCUS.LABEL, true);
					Tag.mItemPtr = item;

					nodes.Add(Tag);
				}
			}
				// Get the tree nodes
			else if (mView == null && mTree != null)
			{
				guiAssetTreeTag Tag = (guiAssetTreeTag)mTree.SelectedNode.Tag;
				Tag.mItemPtr = mTree.SelectedNode;

				nodes.Add(Tag);
			}
			else
			{
				// No valid nodes
				return null;
			}

			return nodes;
		}

		#endregion
		
		#region CLICK HANDLERS
		/// <summary>
		/// Open an explorer window in the target of the selected asset(s)
		/// </summary>
		/// <param name="sender"> Must be a ListView and have a string[] in its Tag describing the columns</param>
		/// <param name="e"></param>
		private void RemoveMenuItem_Click(object sender, System.EventArgs e)
		{
			ArrayList selectedItems = ControlGetSelectedItems();

			string message = "";
			foreach (guiAssetTreeTag tag in selectedItems)
			{				
				if (tag.Execute)
				{
					message = message + tag.FullFilename + "\n";
				}
			}

			if (MOG_Prompt.PromptResponse("Are you sure you want to remove all of these assets from the project?", message, MOGPromptButtons.OKCancel) == MOGPromptResult.OK)
			{
				// Obtain a unique bless label
				string jobLabel = "RemoveAsset." + MOG_ControllerSystem.GetComputerName() + "." + MOG_Time.GetVersionTimestamp();

				foreach (guiAssetTreeTag tag in selectedItems)
				{				
					if (tag.Execute)
					{
						MOG_Filename filename = new MOG_Filename(tag.FullFilename);
					
						// Make sure we are an asset before showing log
						if (filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
						{
							if (guiAssetController.RemoveBlessed(filename, jobLabel))
							{
								tag.ItemRemove();
							}
						}					
					}
				}

				// Start the job
				MOG_ControllerProject.StartJob(jobLabel);
			}
		}

		/// <summary>
		/// Makes the selected assets the current version in the current branch
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MakeCurrentMenuItem_Click(object sender, System.EventArgs e)
		{
			try
			{
				ArrayList selectedItems = ControlGetSelectedItems();
				ArrayList assetFilenames = new ArrayList();

				// Scan the list and prepare it for delivery to the DLL
				string message = "";
				foreach (guiAssetTreeTag tag in selectedItems)
				{
					if (tag.Execute)
					{
						MOG_Filename filename = new MOG_Filename(tag.FullFilename);
						if (filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
						{
							assetFilenames.Add(filename);
						}

						message = message + filename.GetAssetFullName() + "\n";
					}
				}

				// Check if this request effects more than 1 asset??
				if (selectedItems.Count > 1)
				{
					if (MOG_Prompt.PromptResponse("Are you sure you want to make all of these assets the current versions?", message, MOGPromptButtons.OKCancel) != MOGPromptResult.OK)
					{
						return;
					}
				}

				// Stamp all the specified assets
				if (MOG_ControllerProject.MakeAssetCurrentVersion(assetFilenames, "Made current by " + MOG_ControllerProject.GetUserName_DefaultAdmin()))
				{
					// Check if this request effects more than 1 asset??
					if (selectedItems.Count > 1)
					{
						// Inform the user this may take a while
						MOG_Prompt.PromptResponse(	"Completed",
													"This change requires Slave processing.\n" +
													"The project will not reflect these changes until all slaves have finished processing the generated commands.\n" +
													"The progress of this task can be monitored in the Connections Tab.");
					}
				}
				else
				{
					MOG_Prompt.PromptMessage("Make Current Failed", "The system was unable to fully complete the task!", Environment.StackTrace);
				}
			}
			catch(Exception ex)
			{
				MOG_Report.ReportMessage("MakeCurrent Exception", ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
			}
		}

		/// <summary>
		/// Deletes the servers package for this asset then sends a package rebuild command to the server
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RebuildPackageMenuItem_Click(object sender, System.EventArgs e)
		{
			ArrayList selectedItems = ControlGetSelectedItems();
			MenuItem platformItem = (MenuItem)sender;
			string platform = platformItem.Text;

			string message = "";
			foreach (guiAssetTreeTag tag in selectedItems)
			{
				if (tag.Execute)
				{
					message = message + tag.FullFilename + "\n";
				}
			}

			MOG_Prompt.PromptMessage("Rebuild not currently implemented!", message, Environment.StackTrace);
		}
		#endregion
	}
}
