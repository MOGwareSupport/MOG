using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using MOG.DATABASE;
using System.Collections;
using MOG;
using System.IO;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.INI;
using MOG.DOSUTILS;
using MOG.PROMPT;
using MOG.FILENAME;

namespace MOG_ControlsLibrary.Controls
{
	public partial class MogControl_SyncTreeFilter : UserControl
	{
		bool bHaltEvents = false;
		string mOriginalSyncComboBoxMessage;

		private SyncFilter mFilter = new SyncFilter();
		public SyncFilter Filter
		{
			get { return mFilter; }
		}
		
		public delegate void TreeViewEvent(object sender, System.Windows.Forms.TreeViewEventArgs e);		
		[Category("Behavior"), Description("Occurs when a nodes checked status has been changed")]
		public event TreeViewEvent CheckedStateChanged;

		[Category("Behavior"), Description("Occurs when the tree has been loaded")]
		public event TreeViewEvent TreeInitialized;

		public delegate void ConfigureFiltersEvent(object sender, bool enabled);
		[Category("Behavior"), Description("Occurs when the user clicks the configure filters button")]
		public event ConfigureFiltersEvent ConfigureFilter;

		public delegate void FilterLoadedEvent(string filterName);
		public event FilterLoadedEvent FilterLoaded;
	
		public MogControl_SyncTreeFilter()
		{
			InitializeComponent();

			SyncShowAssetsCheckBox.Checked = ClassificationTreeView.ShowAssets = MogUtils_Settings.MogUtils_Settings.LoadBoolSetting("SyncLatestForm", "ShowAssets", false);

			mOriginalSyncComboBoxMessage = SyncFilterComboBox.Text;
		}

		private void ClassificationTreeView_AfterCheck(object sender, TreeViewEventArgs e)
		{
			if (!bHaltEvents)
			{
				bHaltEvents = true;


				// Remove the trailing ~ in an asset file name node.
				// Ie. Before: Glest~Test~Textures~{All}Glest.exe
				//     After:  Glest~Test~Textures{All}Glest.exe
				string fullpath = NodePath2Asset(e.Node.FullPath);				

				// Do we have a parent?
				if (e.Node.Parent != null)
				{					
					// Are we different than our parent?
					if (e.Node.Checked != e.Node.Parent.Checked)
					{
						if (e.Node.Checked)
						{
							mFilter.AddInclusion(fullpath);
						}
						else
						{
							mFilter.AddExclusion(fullpath);
						}
					}
					else
					{
						// Remove this filter
						if (!e.Node.Checked)
						{
							mFilter.RemoveInclusion(fullpath);
						}
						else
						{
							mFilter.RemoveExclusion(fullpath);
						}
					}
				}
				else
				{
					if (e.Node.Checked)
					{
						mFilter.AddInclusion(fullpath);
						mFilter.RemoveExclusion(fullpath);
					}
					else
					{
						mFilter.AddExclusion(fullpath);
						mFilter.RemoveInclusion(fullpath);
					}
				}

				PropagateCheckedState(e.Node);

				if (CheckedStateChanged != null)
				{
					object[] args = { sender, e };
					CheckedStateChanged(sender, e);
				}
				bHaltEvents = false;
			}
			else
			{
				//PropagateCheckedState(e.Node);
			}
		}

		private void PropagateCheckedState(TreeNode parent)
		{
			// Take the checked state of this node to all of its children
			foreach (TreeNode node in parent.Nodes)
			{
				if (parent.Checked)
				{
					if (mFilter.IsExcluded(parent.FullPath))
					{
						parent.Checked = false;
						return;
					}
				}
				else
				{
					if (mFilter.IsIncluded(parent.FullPath))
					{
						parent.Checked = true;
						return;
					}
				}

				node.Checked = parent.Checked;
				PropagateCheckedState(node);
			}
		}

		private void SyncSaveButton_Click(object sender, EventArgs e)
		{
			// Get the name for the filter
			string filterName = SyncFilterComboBox.Text;

			string filterFileName = Path.Combine(MOG_ControllerProject.GetUser().GetUserToolsPath(), filterName + ".sync");

			mFilter.Save(filterFileName);
		}

		private void SyncFilterComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			LoadFilter(SyncFilterComboBox.Text);
			SyncSaveButton.Enabled = true;
			UpdatePromoteButton();
		}

		private void UpdatePromoteButton()
		{
			MOG_Privileges privileges = MOG_ControllerProject.GetPrivileges();
			if (privileges.GetUserPrivilege(MOG_ControllerProject.GetUserName(), MOG_PRIVILEGE.ConfigureUpdateFilterPromotions))
			{
				SyncPromoteButton.Enabled = true;
			}
			else
			{
				SyncPromoteButton.Enabled = false;
			}

			ComboBoxItem filter = SyncFilterComboBox.SelectedItem as ComboBoxItem;
			if (filter != null)
			{
				string filterFileName = filter.FullPath;
				if (filterFileName.Contains(MOG_ControllerProject.GetUserPath()))
				{
					// This is a user tool and can be promoted
					SyncPromoteButton.Image = Properties.Resources.Up;
					SyncToolTips.SetToolTip(SyncPromoteButton, "Promote this filter to the team");
				}
				else
				{
					// This is a project tool and can be demoted
					SyncPromoteButton.Image = Properties.Resources.Down;
					SyncToolTips.SetToolTip(SyncPromoteButton, "Demote this filter");
				}
			}
		}

		private bool ApplyFilter(string filterString, bool checkedState)
		{
			if (filterString.Length > 0)
			{
				// Load and iterate through the exclusions
				string[] parts = filterString.Split(",;".ToCharArray());
				if (parts != null && parts.Length > 0)
				{
					foreach (string filter in parts)
					{
						// Find the node
						TreeNode node = ClassificationTreeView.DrillToNodePath(filter);
						if (node != null)
						{
							// Set the checked state
							node.Checked = checkedState;
							if (node.Parent != null) node.Collapse();
						}
					}
				}

				return true;
			}

			return false;
		}

		private void UpdateFilterDropDown(string selectedText)
		{
			SyncFilterComboBox.Items.Clear();
			//int comboWidth = SyncFilterComboBox.Width;

			// Get the users tools
			if (MOG_ControllerProject.GetUser() != null)
			{
				// Get all the filter files found within the users tools directory
				foreach (string filter in Directory.GetFiles(MOG_ControllerProject.GetUser().GetUserToolsPath(), "*.sync"))
				{
					// Add the names of each one of them to the comboBox
					int index = SyncFilterComboBox.Items.Add(new ComboBoxItem(Path.GetFileNameWithoutExtension(filter), Path.Combine(MOG_ControllerProject.GetUser().GetUserToolsPath(), filter)));
					SyncFilterComboBox.AutoCompleteCustomSource.Add(Path.GetFileNameWithoutExtension(filter));

					// Adjust the width
					//SyncFilterComboBox.Width = SyncFilterComboBox.PreferredSize.Width;

					if (Path.GetFileNameWithoutExtension(filter) == selectedText)
					{
						SyncFilterComboBox.SelectedIndex = index;						
					}
				}
			}

			if (!DesignMode)
			{
				// Get project tools
				// Get all the filter files found within the users tools directory
				string toolsPath = Path.Combine(MOG_ControllerProject.GetProjectPath(), "Tools");
				foreach (string filter in Directory.GetFiles(toolsPath, "*.sync"))
				{
					// Add the names of each one of them to the comboBox
					int index = SyncFilterComboBox.Items.Add(new ComboBoxItem(Path.GetFileNameWithoutExtension(filter), Path.Combine(toolsPath, filter)));

					// Adjust the width
					//SyncFilterComboBox.Width = SyncFilterComboBox.PreferredSize.Width;

					if (Path.GetFileNameWithoutExtension(filter) == selectedText)
					{
						SyncFilterComboBox.SelectedIndex = index;
					}
				}
			}
		}

		public void SaveFilter(string filterName)
		{
			string filename = Path.Combine(MOG_ControllerProject.GetUser().GetUserToolsPath(), filterName + ".sync");
			mFilter.Save(filename);
		}

		public void LoadFilter(string filterName)
		{
			// Is this an already fully qualified name?
			if (DosUtils.ExistFast(filterName) == false)
			{
				// No, then lets assume it came from the user's tools directory
				filterName = MOG_ControllerSystem.LocateTool(filterName + ".sync");
			}
			
			SyncFilterComboBox.Text = Path.GetFileNameWithoutExtension(filterName);
			SyncFilterComboBox.Tag = filterName;
			
			if (DosUtils.ExistFast(filterName))
			{
				//if (ClassificationTreeView.Visible)
				//{
					// Reset the tree
					ClassificationTreeView.Initialize(OnWorkerCompleteLoadFilter);
				//}
				//else
				//{
				//    OnWorkerCompleteLoadFilter();
				//}
			}
		}

		private void OnWorkerCompleteLoadFilter()
		{
			if (mFilter.Load(SyncFilterComboBox.Tag as string))
			{
				// Fire the call back
				if (FilterLoaded != null)
				{
					FilterLoaded(SyncFilterComboBox.Tag as string);
				}

				// If we did NOT apply an exclusion filter AND we did NOT apply an inclusion filter?
				string exclusions = ConvertToTreePaths(mFilter.GetExclusionString());
				string inclusions = ConvertToTreePaths(mFilter.GetInclusionString());

				bool appliedExlusionFilter = ApplyFilter(exclusions, false);
				bool appliedInclusionFilter = ApplyFilter(inclusions, true);

				if (appliedExlusionFilter == false && 
					appliedInclusionFilter == false)
				{
					// If the tree is not initialized?
					if (TreeInitialized != null)
					{
						// Initialize it
						TreeInitialized(this, new TreeViewEventArgs(ClassificationTreeView.TopNode));
					}
				}
			}
			Cursor = Cursors.Default;
		}

		// Remove the trailing ~ in an asset file name node.
		// Ie. Before: Glest~Test~Textures~{All}Glest.exe
		//     After:  Glest~Test~Textures{All}Glest.exe
		private string NodePath2Asset(string fullpath)
		{
			string asset = fullpath;
			if (fullpath.Contains("{") && fullpath.Contains("}"))
			{
				asset = fullpath.Remove(fullpath.IndexOf("{") - 1, 1);
			}
			return asset;
		}

		// Remove the trailing ~ in an asset file name node.
		// Ie. Before: Glest~Test~Textures~{All}Glest.exe
		//     After:  Glest~Test~Textures{All}Glest.exe
		private string Asset2NodePath(string fullpath)
		{
			string nodePath = fullpath;
			if (fullpath.Contains("{") && fullpath.Contains("}"))
			{
				nodePath = fullpath.Insert(fullpath.IndexOf("{"), "~");
			}
			return nodePath;
		}

		private string ConvertToTreePaths(string filter)
		{
			StringBuilder output = new StringBuilder();

			string[] parts = filter.Split(",;".ToCharArray());
			foreach (string item in parts)
			{
				if (output.Length > 0)
				{
					output.Append(",");
				}

				output.Append(Asset2NodePath(item));
			}

			return output.ToString();			
		}

		private void SyncDelButton_Click(object sender, EventArgs e)
		{
			if (SyncFilterComboBox.SelectedItem != null)
			{
				ComboBoxItem item = SyncFilterComboBox.SelectedItem as ComboBoxItem;
				if (item != null)
				{
					string filterFileName = item.FullPath;

					if (MOG_Prompt.PromptResponse("Delete filter?", "Are you sure you want to delete this filter?\n" + item.FullPath, MOGPromptButtons.YesNo) == MOGPromptResult.Yes)
					{
						if (DosUtils.ExistFast(filterFileName) && DosUtils.DeleteFast(filterFileName))
						{
							UpdateFilterDropDown("");

							mFilter.Clear();
							//if (ClassificationTreeView.Visible)
							//{
							ClassificationTreeView.Initialize();
							//}

							SyncSaveButton.Enabled = false;
							SyncFilterComboBox.Text = mOriginalSyncComboBoxMessage;
						}
					}
				}
			}
		}

		private void SyncAddButton_Click(object sender, EventArgs e)
		{
			// Get the name for the filter
			string filterName = Microsoft.VisualBasic.Interaction.InputBox("Enter the name for this sync filter", "Sync Filter Name", "MySyncFilter", -1, -1);

			if (filterName != null && filterName.Length > 0)
			{
				string filterFileName = Path.Combine(MOG_ControllerProject.GetUser().GetUserToolsPath(), filterName + ".sync");

				mFilter.Save(filterFileName);

				UpdateFilterDropDown(filterName);

				// Set the comboBox text to this newly loaded filter
				//SyncFilterComboBox.Text = filterName;
			}
		}

		private void SyncPromoteButton_Click(object sender, EventArgs e)
		{
			string toolsPath = Path.Combine(MOG_ControllerProject.GetProjectPath(), "Tools");

			ComboBoxItem filter = SyncFilterComboBox.SelectedItem as ComboBoxItem;
			if (filter != null)
			{
				string targetFilterName = "";
				string filterFileName = filter.FullPath;
				if (filterFileName.Contains(MOG_ControllerProject.GetUserPath()))
				{
					targetFilterName = filterFileName.Replace(MOG_ControllerProject.GetUser().GetUserToolsPath(), toolsPath);					
				}
				else
				{
					// This is a project tool and should be demoted
					targetFilterName = filterFileName.Replace(toolsPath, MOG_ControllerProject.GetUser().GetUserToolsPath());									
				}

				// This is a user tool and should be promoted
				if (DosUtils.FileCopyFast(filterFileName, targetFilterName, true))
				{
					if (DosUtils.FileDeleteFast(filterFileName))
					{
						UpdateFilterDropDown(Path.GetFileNameWithoutExtension(targetFilterName));
						UpdatePromoteButton();
					}
				}	
			}
		}

		private void SyncConfigureButton_Click(object sender, EventArgs e)
		{
			MOG_Privileges privileges = MOG_ControllerProject.GetPrivileges();
			if (privileges.GetUserPrivilege(MOG_ControllerProject.GetUserName(), MOG_PRIVILEGE.ConfigureUpdateFilterPromotions))
			{
				if (SyncConfigureButton.Visible)
				{
					SyncFilterLabel.Visible = true;
					SyncAddButton.Visible = true;
					SyncDelButton.Visible = true;
					SyncSaveButton.Visible = true;
					SyncPromoteButton.Visible = true;
					SyncConfigureButton.Visible = false;
					ClassificationTreeView.Visible = true;
					SyncShowAssetsCheckBox.Visible = true;
					SyncFilterComboBox.Enabled = true;
					CloseButton.Visible = true;
					SyncFilterComboBox.Width -= (SyncAddButton.Width * 3) + 6;

					ClassificationTreeView.Initialize();

					if (ConfigureFilter != null)
					{
						ConfigureFilter(sender, true);
					}
				}
			}
			else
			{
				MOG_Prompt.PromptResponse("Insufficient Privileges", "Your privileges do not allow you to configure sync filters.");
			}
		}

		private void MogControl_SyncTreeFilter_Load(object sender, EventArgs e)
		{
			UpdateFilterDropDown("");
		}

		private void SyncShowAssetsCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			ClassificationTreeView.ShowAssets = SyncShowAssetsCheckBox.Checked;
			//if (ClassificationTreeView.Visible)
			//{
				ClassificationTreeView.Initialize();
			//}
		}

		private void CloseButton_Click(object sender, EventArgs e)
		{
			SyncFilterLabel.Visible = false;
			SyncAddButton.Visible = false;
			SyncDelButton.Visible = false;
			SyncSaveButton.Visible = false;
			SyncPromoteButton.Visible = false;
			SyncConfigureButton.Visible = true;
			ClassificationTreeView.Visible = false;
			SyncShowAssetsCheckBox.Visible = false;
			SyncFilterComboBox.Enabled = false;
			CloseButton.Visible = false;
			SyncFilterComboBox.Width += (SyncAddButton.Width * 3) + 6;

			UpdateFilterDropDown("");

			if (ConfigureFilter != null)
			{
				ConfigureFilter(sender, false);
			}
		}

		private void SyncShowAssetsCheckBox_Validated(object sender, EventArgs e)
		{
			MogUtils_Settings.MogUtils_Settings.SaveSetting("SyncLatestForm", "ShowAssets", SyncShowAssetsCheckBox.Checked.ToString());
		}		
	}

	class ComboBoxItem
	{
		public string Name;
		public string FullPath;

		public ComboBoxItem(string name, string fullpath)
		{
			Name = name; 
			FullPath = fullpath;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
