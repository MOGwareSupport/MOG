using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using MOG.PROJECT;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.DATABASE;
using MOG.USER;
using System.Collections;
using MOG.CONTROLLER.CONTROLLERSYSTEM;

namespace MOG_ServerManager
{
	public partial class MogControl_RemoteServers : UserControl
	{
		public MogControl_RemoteServers()
		{
			InitializeComponent();

		}

		#region Initialization

		public void Initialize()
		{
			// Load server nodes
			TreeNode root = LoadServerNodes();

			// Load all the projects of that server
			LoadProjects(root);
		}

		private TreeNode LoadServerNodes()
		{
			TreeNode root = CreateNode(Environment.MachineName, "Server", new RemoteSettings());
			RemoteProjectsTreeView.Nodes.Add(root);
			return root;
		}

		private void LoadProjects(TreeNode root)
		{
			// Clear all the projects
			//this.lvProjects.Items.Clear();

			// Get all the projects listed in the mog system
			foreach (string projectName in MOG_ControllerSystem.GetSystem().GetProjectNames())
			{
				MOG_Project pProject = MOG_ControllerSystem.GetSystem().GetProject(projectName);
				if (pProject != null)
				{
					root.Nodes.Add(CreateProjectNode(pProject));
				}
			}
		}

		private TreeNode CreateProjectNode(MOG_Project pProject)
		{
			TreeNode project = CreateNode(pProject.GetProjectName(), "Project", new RemoteSettings());

			// Load all the branches of each project
			ArrayList branches = MOG_DBProjectAPI.GetAllBranchNames(pProject.GetProjectName());
			if (branches != null)
			{
				TreeNode branchNode = CreateNode("Branches", "Branches", new RemoteSettings());
				foreach (MOG_DBBranchInfo branch in branches)
				{
					branchNode.Nodes.Add(CreateNode(branch.mBranchName, "Branch", new RemoteSettings()));					
				}

				project.Nodes.Add(branchNode);
			}

			// Load all the users of the project
			LoadProjectUsers(pProject, project);

			// Load all the tool directories
			// Load misc

			return project;
		}

		private void LoadProjectUsers(MOG_Project pProject, TreeNode project)
		{
			SortedList<string, List<string>> departments = new SortedList<string, List<string>>();

			// Add all the users to the AssignTo subMenu
			ArrayList users = pProject.GetUsers();
			if (users != null)
			{
				TreeNode Users = CreateNode("Users", "Users", new RemoteSettings());

				for (int u = 0; u < users.Count; u++)
				{
					MOG_User user = (MOG_User)users[u];

					// Does this department exist?
					if (departments.ContainsKey(user.GetUserDepartment().ToLower()) == false)
					{
						// No, then add it with the user
						List<string> userList = new List<string>();
						userList.Add(user.GetUserName());

						departments.Add(user.GetUserDepartment().ToLower(), userList);
					}
					else
					{
						// yup, its already there so add this user to the array
						departments[user.GetUserDepartment().ToLower()].Add(user.GetUserName());
					}
				}

				foreach (string department in departments.Keys)
				{
					TreeNode departmentNode = CreateNode(department, "Users", new RemoteSettings());
					foreach (string userName in departments[department])
					{
						departmentNode.Nodes.Add(CreateNode(userName, "User", new RemoteSettings()));
					}
					Users.Nodes.Add(departmentNode);
				}

				project.Nodes.Add(Users);
			}
		}

		private TreeNode CreateNode(string p, string imageKey, RemoteSettings remoteSettings)
		{
			TreeNode node = new TreeNode(p);
			node.Tag = remoteSettings;

			node.ImageKey = imageKey;
			node.SelectedImageKey = imageKey;

			return node;
		}

		#endregion

		#region Control events

		private void RemoteEnableCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			RemoteSettingsGroupBox.Enabled = RemoteEnableCheckBox.Checked;
		}

		private void RemoteProjectsTreeView_AfterSelect(object sender, TreeViewEventArgs e)
		{
			RemotePropertyGrid.SelectedObject = e.Node.Tag;
		}
		#endregion

		public bool EnableRemoteServers
		{
			get { return RemoteEnableCheckBox.Checked; }
			set { RemoteEnableCheckBox.Checked = value; }
		}
	
	}
}
