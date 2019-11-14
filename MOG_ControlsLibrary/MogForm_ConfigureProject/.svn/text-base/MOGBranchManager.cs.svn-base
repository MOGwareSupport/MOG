using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using MOG.DATABASE;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.PROMPT;
using MOG_ControlsLibrary.Utils;
using MOG;

namespace MOG_ControlsLibrary.MogForm_ConfigureProject
{
	public partial class MOGBranchManager : UserControl
	{
		public MOGBranchManager()
		{
			InitializeComponent();
		}

		public void InitializeListView()
		{
			InitializeBranches();
			InitializeTags();

			// Set light version control
			MogUtil_VersionInfo.SetLightVersionControl(BranchAddButton);
		}

		private void InitializeBranches()
		{
			BranchesListView.Items.Clear();

			ArrayList Branches = MOG_DBProjectAPI.GetAllBranchNames();

			if (Branches != null)
			{
				foreach (MOG_DBBranchInfo branch in Branches)
				{
					if (!branch.mTag)
					{
						AddBranchListViewItem(branch);
					}
				}
			}
		}

		private void InitializeTags()
		{
			TagsListView.Items.Clear();

			ArrayList Branches = MOG_DBProjectAPI.GetAllBranchNames();

			if (Branches != null)
			{
				string activeTag = "";
				if (MOG_ControllerProject.GetProject().GetConfigFile().KeyExist("Project", "ActiveTag"))
				{
					activeTag = MOG_ControllerProject.GetProject().GetConfigFile().GetString("Project", "ActiveTag");
				}

				foreach (MOG_DBBranchInfo branch in Branches)
				{
					// Is this a tag?  or
					// Is this 'Current'?
					if (branch.mTag ||
						string.Compare(branch.mBranchName, "Current", true) == 0)
					{
						AddTagListViewItem(branch, (string.Compare(branch.mBranchName, activeTag, true) == 0));
					}
				}
			}
		}

		private void AddBranchListViewItem(MOG_DBBranchInfo branch)
		{
			ListViewItem item = new ListViewItem();

			item.Text = branch.mBranchName;
			item.SubItems.Add(MogUtils_StringVersion.VersionToString(branch.mCreatedDate));
			item.SubItems.Add(branch.mCreatedBy);
			item.SubItems.Add(MogUtils_StringVersion.VersionToString(branch.mRemovedDate));
			item.SubItems.Add(branch.mRemovedBy);

			// This is a removed branch
			if (branch.mRemovedBy.Length != 0)
				item.BackColor = System.Drawing.Color.Red;

			BranchesListView.Items.Add(item);
		}

		private void AddTagListViewItem(MOG_DBBranchInfo branch, bool isActiveTag)
		{
			ListViewItem item = new ListViewItem();

			item.Text = branch.mBranchName;
			item.SubItems.Add(MogUtils_StringVersion.VersionToString(branch.mCreatedDate));
			item.SubItems.Add(branch.mCreatedBy);
			if (isActiveTag)
			{
				item.ImageKey = "check";
			}
			
			TagsListView.Items.Add(item);
		}

		private void SetActiveTagItem(ListViewItem activeItem)
		{
			// Clear the 'ActiveTag' status on all tags
			foreach (ListViewItem item in TagsListView.Items)
			{
				item.ImageKey = "";
			}

			// Make sure we have a new tag specified?
			if (activeItem != null)
			{
				// Mark this as the new 'ActiveTag'
				activeItem.ImageKey = "check";
			}
		}

		private void RemoveSelectedBranches(ListView activeListview)
		{
			if (activeListview != null && activeListview.SelectedItems != null)
			{
				foreach (ListViewItem item in activeListview.SelectedItems)
				{
					// Remove CURRENT!  Are you sure!
					if (string.Compare(item.Text, "Current", true) == 0)
					{
						if (MOG_Prompt.PromptMessage("Remove branch", "'Current' is a system defined branch that cannot be removed!"))
						{
							continue;
						}
					}

					string message = "Are you sure you want to remove this branch or tag?\n" + 
									 "BRANCH: " + item.Text;
					if (MOG_Prompt.PromptResponse("Remove branch/tag?", message, MOGPromptButtons.YesNo) == MOGPromptResult.Yes)
					{
						// Remove the branch
						if (MOG_ControllerProject.BranchRemove(item.Text))
						{
//TODO - We really shouldn't purge the branch right now but rather show the removed branches in red and allow the user to purge them later
							// Also purge this branch now
							if (MOG_ControllerProject.BranchPurge(item.Text))
							{
								item.Remove();
							}
						}
						else
						{
							MOG_Prompt.PromptMessage("Error in remove branch/tag", "Branch/Tag ( " + item.Text + " ) was not able to be removed!");
						}
					}
				}
			}
		}

		private void AddBranch()
		{
			MOG_Privileges privs = MOG_ControllerProject.GetPrivileges();

			if (privs.GetUserPrivilege(MOG_ControllerProject.GetUserName(), MOG_PRIVILEGE.CreateBranch))
			{
				CreateBranchForm newBranch = new CreateBranchForm();
				newBranch.BranchSourceTextBox.Text = MOG_ControllerProject.GetBranchName();

				if (newBranch.ShowDialog() == DialogResult.OK)
				{
					// Create the branch
					if (MOG_ControllerProject.BranchCreate(MOG_ControllerProject.GetBranchName(), newBranch.BranchNameTextBox.Text))
					{
						MOG_DBBranchInfo branch = MOG_DBProjectAPI.GetBranch(newBranch.BranchNameTextBox.Text);

						AddBranchListViewItem(branch);

						MOG_Prompt.PromptMessage("Create Branch", "New branch successfully created.\n" +
												 "BRANCH: " + newBranch.BranchNameTextBox.Text);
					}
				}
			}
			else
			{
				MOG_Prompt.PromptResponse("Insufficient Privileges", "Your privileges do not allow you to create branches.");
			}
		}

		private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
		{
			InitializeListView();
		}

		private void BranchRemoveButton_Click(object sender, EventArgs e)
		{
			RemoveSelectedBranches(BranchesListView);
		}		
		
		private void removeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			RemoveSelectedBranches(BranchesListView);
		}

		private void addToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AddBranch();
		}

		private void BranchAddButton_Click(object sender, EventArgs e)
		{
			AddBranch();
		}

		private void TagRefreshToolStripMenuItem_Click(object sender, EventArgs e)
		{
			InitializeListView();
		}

		private void TagRemoveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			RemoveSelectedBranches(TagsListView);
		}

		private void setAsActiveTagToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (TagsListView != null && TagsListView.SelectedItems != null)
			{
				if (TagsListView.SelectedItems.Count > 1)
				{
					string message = "You can only select one tag as active!";
					MOG_Prompt.PromptMessage("Set active tag", message);
				}
				else
				{
					ListViewItem item = TagsListView.SelectedItems[0];

					MOG_ControllerProject.GetProject().GetConfigFile().PutString("Project", "ActiveTag", item.Text);
					MOG_ControllerProject.GetProject().GetConfigFile().Save();

					SetActiveTagItem(item);
				}
			}			
		}

		private void clearActiveTagToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (TagsListView != null && TagsListView.SelectedItems != null)
			{
				MOG_ControllerProject.GetProject().GetConfigFile().RemoveString("Project", "ActiveTag");
				MOG_ControllerProject.GetProject().GetConfigFile().Save();

				SetActiveTagItem(null);
				TagsListView.Refresh();
			}
		}		
	}
}
