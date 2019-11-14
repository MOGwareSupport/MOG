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
using MOG_ControlsLibrary.MogUtils_Settings;
using System.IO;
using System.Xml.Serialization;

namespace MOG_RemoteServerManager
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
					
			RemoteEnableCheckBox.Checked = MogUtils_Settings.LoadBoolSetting("RemoteServer", "RemoteEnabled", "Enabled", false);

			LoadSettings();
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
				MOG_Project pProject = MOG_ControllerProject.LoginProject(projectName, "");
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
			MogUtils_Settings.SaveSetting("RemoteServer", "RemoteEnabled", "Enabled", RemoteEnableCheckBox.Checked.ToString());
		}

		private void RemoteProjectsTreeView_AfterSelect(object sender, TreeViewEventArgs e)
		{
			RemotePropertyGrid.SelectedObject = e.Node.Tag;
		}
		#endregion

		public struct NodeData
		{
			public string fullpath;
			public RemoteSettings data;
		}

		public bool SaveSettings()
		{
			try
			{
				// Open the file specified for output and serialize the lists
				TextWriter writer = new StreamWriter("test.txt");
				if (writer != null)
				{
					// Tell the XmlSerializer which types we will be serializing
					Type[] typeArray = new Type[1];
					typeArray[0] = typeof(SaveInfo);

					// Serialize the data
					XmlSerializer serializer = new XmlSerializer(typeof(SaveInfo), typeArray);

					SaveInfo info = new SaveInfo();

					SaveNodeSettings(info.data, RemoteProjectsTreeView.Nodes);

					serializer.Serialize(writer, info.data);

					writer.Flush();
					writer.Close();

					// Success
					return true;
				}
			}
			catch (Exception e)
			{
				e.ToString();
			}
			finally
			{
			}

			return false;
		}

		private void SaveNodeSettings(ArrayList data, TreeNodeCollection nodes)
		{
			foreach (TreeNode node in nodes)
			{
				if (node.Tag is RemoteSettings && ((RemoteSettings)node.Tag).SyncSources.Count != 0)
				{
					NodeData remoteData = new NodeData();
					remoteData.fullpath = node.FullPath;
					remoteData.data = node.Tag as RemoteSettings;
					data.Add(remoteData);
				}
				
				if (node.Nodes.Count != 0)
				{
					SaveNodeSettings(data, node.Nodes);
				}
			}
		}

		private void LoadNodeSettings(NodeData remoteData, TreeNodeCollection nodes)
		{
			foreach (TreeNode node in nodes)
			{
				if (string.Compare(remoteData.fullpath, node.FullPath) == 0)
				{
					node.Tag = remoteData.data;
				}
				else
				{
					if (node.Nodes.Count != 0)
					{
						LoadNodeSettings(remoteData, node.Nodes);
					}
				}
			}
		}

		public void LoadSettings()
		{
			FileInfo settings = new FileInfo("test.txt");
			if (settings != null && settings.Exists && settings.Length != 0)
			{
				SaveInfo info = new SaveInfo();
				
				// Tell the XmlSerializer which types we will be deserializing
				Type[] typeArray = new Type[1];
				typeArray[0] = typeof(SaveInfo);

				StreamReader reader = new StreamReader("test.txt");
				if (reader != null)
				{
					try
					{
						// Check if we have a valid file to read?
						if (reader.Peek() > -1)
						{
							// Extract the serialized MOG_ShutdownInfo object
							XmlSerializer serializer = new XmlSerializer(typeof(SaveInfo), typeArray);
							info = serializer.Deserialize(reader) as SaveInfo;
						}

						foreach (NodeData remoteData in info.data)
						{
							LoadNodeSettings(remoteData, RemoteProjectsTreeView.Nodes);
						}

					}
					catch (Exception e)
					{
						// JohnRen - Don't tell the user about this since there is nothing they can do about it
						e.ToString();
					}
					finally
					{
						reader.Close();
					}
				}
			}
		}

		public bool EnableRemoteServers
		{
			get { return RemoteEnableCheckBox.Checked; }
			set { RemoteEnableCheckBox.Checked = value; }
		}
	
	}

	public class SaveInfo
	{
		public ArrayList data;

		public SaveInfo()
		{
			data = new ArrayList();
		}
	}
}
