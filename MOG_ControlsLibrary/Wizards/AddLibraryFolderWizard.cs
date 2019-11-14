using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using MOG.DOSUTILS;
using MOG.PROMPT;
using MOG.REPORT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.CONTROLLER.CONTROLLERPROJECT;

namespace MOG_ControlsLibrary.Wizards
{
	public partial class AddLibraryFolderWizard : Form
	{
		public AddLibraryFolderWizard()
		{
			InitializeComponent();
			
			FolderNewNameTextBox.SelectedText = "New Folder";

			FolderLibraryWizard.NextEnabled = false;			
		}

		public string AddLibraryFolderName
		{
			get { return FolderNewNameTextBox.Text; }
			set { FolderNewNameTextBox.Text = value; }
		}

		public string AddLibraryFolderIcon
		{
			get { return FolderSpecialFolderListView.SelectedItems[0].Tag as string; }
			set { FolderSpecialFolderListView.SelectedItems[0].Tag = value; }
		}

		public int AddLibraryFolderMaxRevisions
		{
			get { return (int)FolderRevisionsNumericUpDown.Value; }
			set { FolderRevisionsNumericUpDown.Value = value; }
		}

		#region "Support functions"
		private void InitializeSpecialFolderIconsListView()
		{
			ImageList Icons = new ImageList();
			
			FolderSpecialFolderListView.LargeImageList = Icons;
			FolderSpecialFolderListView.SmallImageList = Icons;

			FolderSpecialFolderListView.Items.Clear();
			FolderSpecialFolderListView.ShowGroups = true;

			// Get all the system level icons
			string systemIcons = MOG_ControllerSystem.GetSystemRepositoryPath() + "\\Tools\\Images\\Filetypes";
			LoadFileIcons("system", Icons, systemIcons);

			// Get all the system level icons
			if (MOG_ControllerProject.GetProject() != null)
			{
				string projectIcons = MOG_ControllerProject.GetProjectPath() + "\\Tools\\Images";
				LoadFileIcons("project", Icons, projectIcons);
			}
		}

		private void LoadFileIcons(string type, ImageList Icons, string path)
		{
			if (Directory.Exists(path))
			{
				foreach (string file in Directory.GetFiles(path))
				{
					if (file.Contains("Folder"))
					{
						ListViewItem folderIcon = new ListViewItem(Path.GetFileNameWithoutExtension(file), LoadIcon(file, Icons));
						folderIcon.Tag = file;
						switch(type)
						{
							case "system":
								folderIcon.Group = FolderSpecialFolderListView.Groups[0];
								break;
							case "project":
								folderIcon.Group = FolderSpecialFolderListView.Groups[1];
								break;
						}

						FolderSpecialFolderListView.Items.Add(folderIcon);
					}
				}
			}
		}

		private int LoadIcon(string imageFileName, ImageList imageList)
		{
			try
			{
				if (imageFileName != null && imageFileName.Length > 0)
				{
					// Load the icon specified by the Icon= key
					string iconName = imageFileName;
								
					// Make sure that the icon file exists
					if (DosUtils.FileExist(iconName))
					{
						// Get the image
						Image myImage = new Bitmap(iconName);

						// Add the image and the type to the arrayLists
						imageList.Images.Add(myImage);
						
						return imageList.Images.Count -1;
					}
				}
			}
			catch(Exception e)
			{
				MOG_Report.ReportMessage("Load Icon", e.Message, e.StackTrace, MOG_ALERT_LEVEL.CRITICAL);
			}

			return 0;
		}

		#endregion "Support functions"

		#region "Wizard tab events"
		
		private void FolderSpecialWizardPage_ShowFromNext(object sender, EventArgs e)
		{
			FolderLibraryWizard.NextEnabled = false;

			InitializeSpecialFolderIconsListView();
		}

		#endregion "Wizard tab events"

		
		#region "Control events"
	
		private void FolderNewNameTextBox_TextChanged(object sender, EventArgs e)
		{
			// Only let us move forward if we have a valid new folder name
			if (FolderNewNameTextBox.Text.Length > 0)
			{
				FolderLibraryWizard.NextEnabled = true;
			}
			else
			{
				FolderLibraryWizard.NextEnabled = false;
			}
		}

		private void FolderSpecialFolderListView_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Only let us move forward if we have a valid new folder name
			if (FolderSpecialFolderListView.SelectedItems != null && FolderSpecialFolderListView.SelectedItems.Count == 1)
			{
				FolderLibraryWizard.NextEnabled = true;
			}
			else
			{
				FolderLibraryWizard.NextEnabled = false;
			}
		}
		
		private void ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem item = (ToolStripMenuItem)sender;

			switch(item.Text)
			{
				case "Details":
					FolderSpecialFolderListView.View = View.Details;
					break;
				case "Small Icon":
					FolderSpecialFolderListView.View = View.SmallIcon;
					break;
				case "Large Icon":
					FolderSpecialFolderListView.View = View.LargeIcon;
					break;
				case "Tile":
					FolderSpecialFolderListView.View = View.Tile;
					break;
			}
		}
		
		private void AddLibraryFolderWizard_Activated(object sender, EventArgs e)
		{
//			FolderNewNameTextBox.SelectedText = "New Folder";
			FolderNewNameTextBox.Focus();
		}
		#endregion "Control events"

				
	}
}