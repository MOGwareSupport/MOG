using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using MOG;
using System.Collections;
using MOG.REPORT;
using MOG_ControlsLibrary.Common.MogControl_PrivilegesChangeForm;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.USER;

namespace MOG_ControlsLibrary.Common.MogControl_PrivilegesForm
{
	public partial class MogControl_Privileges : UserControl
	{
		private const int OptionsPanel_MinWidth = 280;
		private int OldWidth;
		private MOG_Privileges mPrivileges;
		private TreeNode mGroupNode;
		private MessageObject GridMessage;
		private const string User_DataObject = "user";

		TreeNode mLastSelectedNode;

		/// <summary>
		/// Describe what kind of node we have
		/// </summary>
		public enum NodeType
		{
			Group,
			User,
			UserSpecific,
			Other,
		}

		public MogControl_Privileges()
		{
			InitializeComponent();

			// Initialize our OldWidth property for layout...
			this.OldWidth = this.Width;			
		}

		public void Initialize_Control(MOG_Privileges privileges)
		{
			// Set our privileges object
			this.mPrivileges = privileges;

			// Initialize our Groups node with groups and users
			InitializeMainTreeView(mPrivileges);
		}

		private void RefreshMainTreeView()
		{
			if (this.MainTreeView.Nodes.Count > 0)
			{
				MainTreeView.Nodes.Clear();
			}
			InitializeMainTreeView(this.mPrivileges);
			this.MainTreeView.ExpandAll();
		}

		private void InitializeMainTreeView(MOG_Privileges privileges)
		{
			if (this.MainTreeView.Nodes.Count > 0)
			{
				MainTreeView.Nodes.Clear();
			}

			this.GridMessage = new MessageObject("Nothing selected.");

			// Create our groupNode and set our global groupNode
			TreeNode groupNode = this.MainTreeView.Nodes.Add("Groups");
			this.mGroupNode = groupNode;
			groupNode.Tag = NodeType.Other;

			TreeNode userNode = this.MainTreeView.Nodes.Add("Users");
			userNode.Tag = NodeType.Other;

			// Get a list of all valid users for verification with the privileges.ini
			ArrayList projectUsers = MOG_ControllerProject.GetProject().GetUsers();
			SortedList<string, MOG_User> sortedProjectUsers = new SortedList<string, MOG_User>();
			foreach (MOG_User user in projectUsers)
			{
				sortedProjectUsers.Add(user.GetUserName().ToLower(), user);
			}

			SortedList users = privileges.UsersList;
			ArrayList groups = privileges.GroupsList;
			foreach (string groupName in groups)
			{
				TreeNode groupNameNode = groupNode.Nodes.Add(groupName);
				groupNameNode.Tag = NodeType.Group;

				foreach (DictionaryEntry user in users)
				{
					if (sortedProjectUsers.ContainsKey(user.Key.ToString().ToLower()))
					{
						string userGroupName = user.Value.ToString();
						if (userGroupName == groupName)
						{
							TreeNode usersGroupNode = groupNameNode.Nodes.Add(user.Key.ToString());
							usersGroupNode.Tag = NodeType.User;
						}
					}
				}
			}

			groupNode.Expand();

			// Add all our users under the userNode
			foreach (DictionaryEntry user in users)
			{
				string userName = user.Key.ToString();
				if (sortedProjectUsers.ContainsKey(userName.ToLower()))
				{
					TreeNode userNameNode = userNode.Nodes.Add(userName);
					userNameNode.Tag = NodeType.UserSpecific;

					if (mPrivileges.UserHasCustomPrivileges(userName))
					{
						userNameNode.ForeColor = Color.Blue;
					}
				}
				else
				{
					privileges.RemoveUser(userName);
				}
			}

			MainTreeView.ExpandAll();
		}

		#region MainTreeView Events
		private void MainTreeView_ItemDrag(object sender, ItemDragEventArgs e)
		{
			try
			{
				TreeNode node = (TreeNode)e.Item;
				if (node.Tag.ToString() == NodeType.User.ToString())
				{
					DataObject send = new DataObject(User_DataObject, node);

					if (send != null)
					{
						// Fire the DragDrop event
						DragDropEffects dde1 = DoDragDrop(send, DragDropEffects.Copy);
					}
				}
			}
			catch (Exception ex)
			{
				ex.ToString();
			}
		}

		private void MainTreeView_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (this.mLastSelectedNode != null)
			{
				this.mLastSelectedNode.BackColor = MainTreeView.BackColor;
			}
			e.Node.BackColor = SystemColors.Highlight;
			this.mLastSelectedNode = e.Node;
		}

		private void MainTreeView_DragDrop(object sender, DragEventArgs e)
		{
			try
			{
				if (e.Data.GetDataPresent(User_DataObject))
				{
					// Get our array list
					TreeNode node = (TreeNode)e.Data.GetData(User_DataObject);

					// If we do not still have the first node we selected...
					if (node != this.MainTreeView.SelectedNode)
					{
						string groupToAddTo = this.MainTreeView.SelectedNode.Text;
						string userToChange = node.Text;
						ChangeUserGroup(userToChange, groupToAddTo);
					}
				}
			}
			catch (Exception ex)
			{
				ex.ToString();
			}
		}

		private void MainTreeView_DragEnter(object sender, DragEventArgs e)
		{
			// If our data is the right format for dragDrop, allow dragDrop
			if (e.Data.GetDataPresent(User_DataObject))
			{
				e.Effect = DragDropEffects.Copy;
			}
		}

		private void MainTreeView_DragOver(object sender, DragEventArgs e)
		{
			Point clientPoint = this.PointToClient(new Point(e.X, e.Y));
			TreeNode node = this.MainTreeView.GetNodeAt(clientPoint);
			if (node != null && node != this.MainTreeView.SelectedNode && node.Tag.ToString() == NodeType.Group.ToString())
			{
				this.MainTreeView.SelectedNode = node;
			}
		}

		private void MainTreeView_MouseDown(object sender, MouseEventArgs e)
		{
			TreeView tv = (TreeView)sender;
			try
			{
				tv.SelectedNode = (TreeNode)tv.GetNodeAt(e.X, e.Y);
				if (tv.SelectedNode != null)
				{
					UpdateMainPropertyGrid(tv.SelectedNode);
				}
			}
			// If we get exceptions, we eat them
			catch (Exception ex)
			{
				ex.ToString();
			}
		}
		#endregion

		private void UpdateMainPropertyGrid(TreeNode node)
		{
			// If this is one of our Group nodes
			if (this.mGroupNode.Nodes.Contains(node))
			{
				this.mPrivileges.SetCurrentGroupForProperties(node.Text);
				this.MainPropertyGrid.SelectedObject = this.mPrivileges;
				SelectedGroupLabel.Text = "GROUP: " + node.Text;
			}
			else
			{
				// By default, select our GridMessage object
				this.MainPropertyGrid.SelectedObject = GridMessage;
				GridMessage.mMessage = "Drag a user to change his/her group";

				if (node.Tag.ToString() == NodeType.User.ToString())
				{
					SelectedGroupLabel.Text = "USER: " + node.Text;
				}
				else if (node.Tag.ToString() == NodeType.UserSpecific.ToString())
				{
					this.mPrivileges.SetCurrentGroupForProperties(node.Text);
					this.MainPropertyGrid.SelectedObject = this.mPrivileges;
					SelectedGroupLabel.Text = "User-Specific Privileges for " + node.Text;
				}
				else
				{
					SelectedGroupLabel.Text = "No Valid Node Selected";
				}
			}
		}

		/// <summary>
		/// Change the user, userName, to the groupName passed in.
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="groupName"></param>
		private void ChangeUserGroup(string userName, string groupName)
		{
			// If we have the Admin User, display an error message
			if (userName == MOG_Privileges.MainSections.Admin.ToString() &&
				groupName != MOG_Privileges.MainSections.Admin.ToString())
			{
				MessageBox.Show(this, "User 'Admin' cannot change groups.");
				return;
			}

			// Change the user's group
			this.mPrivileges.RemoveUser(userName);
			this.mPrivileges.AddUser(userName, groupName);

			string nodeFullPath = MainTreeView.SelectedNode.FullPath;
			// Re-initialize our tree
			this.InitializeMainTreeView(mPrivileges);

			// Expand back down to where we were
			string[] pathSplit = nodeFullPath.Split(MainTreeView.PathSeparator.ToCharArray());
			TreeNodeCollection nodes = this.MainTreeView.Nodes;
			foreach (string nodePath in pathSplit)
			{
				foreach (TreeNode nodeToExpand in nodes)
				{
					if (nodeToExpand.Text == nodePath)
					{
						nodeToExpand.Expand();
						nodes = nodeToExpand.Nodes;
						MainTreeView.SelectedNode = nodeToExpand;
						break;
					}
				}
			}
			MainTreeView.ExpandAll();
		}

		private void ResetToGroupDefaults(TreeNode node, string userName, string groupName)
		{
			// Change the user's group
			this.mPrivileges.UserRemoveCustomPrivileges(userName);
			node.ForeColor = SystemColors.ControlText;
			// This triggers the property grid to refresh
			this.MainPropertyGrid.SelectedObject = this.mPrivileges;
		}

		#region ToolStripItem Events
		private void addGroupToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MogControl_PrivilegesAddGroupForm agf = new MogControl_PrivilegesAddGroupForm();
			if (agf.ShowDialog(this) == DialogResult.OK)
			{
				string newGroupName = agf.GroupNameTextBox.Text;
				this.mPrivileges.AddGroup(newGroupName);
				this.RefreshMainTreeView();
			}
		}

		private void deleteGroupToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TreeNode selectedNode = this.MainTreeView.SelectedNode;
			// If we have a node selected AND it is at the groupNode-level of the treeView...
			if (selectedNode != null && this.mGroupNode.Nodes.Contains(selectedNode))
			{
				// If this group still has users in it...
				if (this.mPrivileges.GroupHasUsers(selectedNode.Text))
				{
					// Warn user that we still have users in this group
					DialogResult result = MessageBox.Show(this, "This group still has users!\r\n\r\nWould you like MOG to move"
						+ " these users into the DefaultUser group?", "Users Found!",
						MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);

					// If user would like not to delete this group now, cancel deletion...
					if (result == DialogResult.No || result == DialogResult.Cancel)
					{
						return;
					}
				}

				// Remove the group, if we have it
				if (this.mPrivileges.RemoveGroup(selectedNode.Text))
				{
					this.RefreshMainTreeView();
				}
				// Else, report that we had an error
				else
				{
					MessageBox.Show(this, "Unable to remove group.\r\n");
				}
			}
			else
			{
				string nodeText = "<No node selected>";
				if (selectedNode != null)
				{
					nodeText = "<" + selectedNode.Text + ">";
				}
				MessageBox.Show(this, "The node, " + nodeText + ", is not valid.  Please select a Group Node.");
			}
		}

		private void assignToGroupToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
				if (menuItem != null)
				{
					TreeNode node = MainTreeView.SelectedNode;
					// Create identifiers for our userName and the groupName we will be changing to
					string userName = node.Text;
					string groupName = menuItem.Text;
					this.ChangeUserGroup(userName, groupName);
				}
			}
			// Tell the user if there was an error
			catch (Exception ex)
			{
				MOG_Report.ReportMessage("Error Assigning to Group " + ((MenuItem)sender).Text,
					ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.ERROR);
			}
		}

		private void resetToGroupDefaultsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
				if (menuItem != null)
				{
					TreeNode node = MainTreeView.SelectedNode;
					// Create identifiers for our userName and the groupName we will be changing to
					string userName = node.Text;
					string groupName = menuItem.Text;
					ResetToGroupDefaults(node, userName, groupName);
				}
			}
			// Tell the user if there was an error
			catch (Exception ex)
			{
				MOG_Report.ReportMessage("Error Assigning to Group " + ((MenuItem)sender).Text,
					ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.ERROR);
			}
		}		

		private void MainTreeContextMenuStrip_Opening(object sender, CancelEventArgs e)
		{
			if (this.MainTreeView.SelectedNode.Tag.ToString() == NodeType.User.ToString())
			{
				this.addGroupToolStripMenuItem.Enabled = false;
				this.deleteGroupToolStripMenuItem.Enabled = false;
				this.resetToGroupDefaultsToolStripMenuItem.Enabled = false;
				this.seperatorToolStripMenuItem.Enabled = false;
				this.assignToGroupToolStripMenuItem.Enabled = false;

				// Get the currently selected user
				TreeNode node = MainTreeView.SelectedNode;
				string userName = node.Text;
				// Make sure we are not Admin?
				if (userName != MOG_Privileges.MainSections.Admin.ToString())
				{
					ArrayList groups = this.mPrivileges.GroupsList;

					// First, clear our sub-MenuItems, if we already have them...
					if (assignToGroupToolStripMenuItem.DropDownItems.Count > 0)
					{
						assignToGroupToolStripMenuItem.DropDownItems.Clear();
					}

					foreach (string groupName in groups)
					{
						ToolStripMenuItem groupMenuItem = new ToolStripMenuItem(groupName);
						groupMenuItem.Click += new EventHandler(assignToGroupToolStripMenuItem_Click);
						assignToGroupToolStripMenuItem.DropDownItems.Add(groupMenuItem);
					}
					assignToGroupToolStripMenuItem.Enabled = true;
				}
			}
			else if (this.MainTreeView.SelectedNode.Tag.ToString() == NodeType.UserSpecific.ToString())
			{
				foreach (ToolStripItem item in this.MainTreeContextMenuStrip.Items)
				{
					item.Enabled = false;
				}

				// Get the currently selected user
				TreeNode node = MainTreeView.SelectedNode;
				string userName = node.Text;
				// Make sure we are not Admin?
				if (userName != MOG_Privileges.MainSections.Admin.ToString())
				{
					if (mPrivileges.UserHasCustomPrivileges(userName))
					{
						resetToGroupDefaultsToolStripMenuItem.Enabled = true;
					}
				}
			}
			else
			{
				this.addGroupToolStripMenuItem.Enabled = true;
				this.deleteGroupToolStripMenuItem.Enabled = true;
				this.assignToGroupToolStripMenuItem.Enabled = false;
				this.seperatorToolStripMenuItem.Enabled = false;
				this.resetToGroupDefaultsToolStripMenuItem.Enabled = false;
			}
		}
		#endregion		
	}

	/// <summary>
	/// PrivilegeForm-specific internal-only class for displaying message in PropertyGrid
	/// </summary>
	class MessageObject
	{
		[Category("Information"),
		Description("Please select a Group to change its privileges.  You may also drag users to different groups.")]
		public string Message
		{
			get
			{
				return this.mMessage;
			}
		}

		protected internal string mMessage;

		public MessageObject(string message)
		{
			this.mMessage = message;
		}
	} // end sub-class
}
