using System;
using System.Data;
using System.Collections;
using System.Data.SqlClient;

using System.Windows.Forms;
using System.Drawing;

using EV.Windows.Forms;

using MOG_Client.Forms;
using MOG_Client.Client_Utilities;

using MOG;
using MOG.TIME;
using MOG.DATABASE;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;



namespace MOG_Client.Client_Gui.ProjectManager_Helper
{
	/// <summary>
	/// Summary description for guiProjectManagerTasks.
	/// </summary>
	public class guiProjectManagerTasks
	{
		private MogMainForm mainForm;
		//private MOG_Database db;
		public ArrayList mListViewSort_Manager;

		public enum NodeTypes {Department, user, task, subTask};

		public guiProjectManagerTasks(MogMainForm main)
		{
			mainForm = main;
			mListViewSort_Manager = new ArrayList();

//			InitializeDataBase();
//			LoadDepartments();
//
//			InitializeAssignUsers();
//			InitializeAssignDepartments();
		}

		public void Refresh()
		{
//			InitializeDataBase();
//			LoadDepartments();
		}

//		public void FullExpand()
//		{
//			mainForm.ProjectManagerTasksTreeView.ExpandAll();
//		}
//
//		private void InitializeDataBase()
//		{
//			mainForm.ProjectManagerTasksTreeView.Nodes.Clear();
//			db = MOG_ControllerSystem.GetDB();
//		}
//
//		#region Task utility functions
//		/// <summary>
//		/// Get percentage complete of this node and all its children
//		/// </summary>
//		/// <param name="taskParent"></param>
//		/// <param name="totalPercentage"></param>
//		/// <param name="numberOfChildren"></param>
//		/// <returns></returns>
//		private int ChildrenCalulatePercent(MOG_TaskInfo taskParent, ref int totalPercentage, ref int numberOfChildren)
//		{
//							
//			ArrayList tasks = MOG_DBTaskAPI.GetChildrenTasks(taskParent.GetTaskID());
//
//			if (tasks != null)
//			{
//				foreach (MOG_TaskInfo taskHandle in tasks)
//				{
//					numberOfChildren++;
//					ChildrenCalulatePercent(taskHandle, ref totalPercentage, ref numberOfChildren);
//				}
//			}
//			
//			totalPercentage += (taskParent.GetPercentComplete() / numberOfChildren);
//
//			return totalPercentage;
//		}
//
//		/// <summary>
//		/// Create a tree node based on a MOG_TaskInfo structure
//		/// </summary>
//		/// <param name="task"></param>
//		/// <returns></returns>
//		private TreeNode TaskCreateTaskNode(TaskNode taskHandle)
//		{
//			TreeNode cnode = new TreeNode();
//
//			// Create gui task container
//			//TaskNode taskHandle = new TaskNode(TaskType.Task, task.GetName(), task);
//			cnode.Tag = taskHandle;
//
//			switch (taskHandle.GetTaskType())
//			{
//				case TaskType.Task:
//					// Set the name
//					int percent = 0;
//					int num = 1;
//					cnode.Text = taskHandle.GetName() + "(" + ChildrenCalulatePercent(taskHandle.GetTaskInfo(), ref percent, ref num) + "%)";
//			
//					// Add the subItems
//					//			cnode.SubItems.Add(taskHandle.GetTaskInfo().GetPercentComplete().ToString());
//					//			cnode.SubItems.Add(taskHandle.GetTaskInfo().GetCompletionDate().ToString());
//			
//					// Set custom font
//					//			cnode.Font = taskHandle.GetNodeFont(TasksUsersTasksTreeView);
//					cnode.NodeFont = taskHandle.GetNodeFont(mainForm.ProjectManagerTasksTreeView);
//					cnode.ForeColor = taskHandle.GetNodeFontColor(mainForm.ProjectManagerTasksTreeView);
//			
//					// Set the checkbox
//					//			cnode.SubItems[0].Checked = taskHandle.GetChecked();
//					cnode.Checked = (taskHandle.GetChecked() == CheckState.Checked)? true : false;
//					break;
//				default:
//					cnode.Text = taskHandle.GetName();
//					break;
//			}
//
//			// Set the task image
//			cnode.ImageIndex = taskHandle.GetImage();
//			
//			return cnode;
//		}
//
//		/// <summary>
//		/// Create a new TreeNode based on the taskNode passed in and add it to the parent node provided
//		/// </summary>
//		/// <param name="taskNode"></param>
//		/// <param name="parent"></param>
//		private void CreateTreeNode(TaskNode taskNode, TreeNode parent)
//		{
//			TreeNode task = new TreeNode(taskNode.GetTaskInfo().GetName());
//
//			// Save a copy of the task node in the tree node tag
//			task.Tag = taskNode;
//			task.ImageIndex = taskNode.GetImage();
//
//			// Add the tree node
//			parent.Nodes.Add(task);
//		
//		}
//		#endregion
//
//		#region TreeView Population functions
//
//		private void LoadDepartments()
//		{
//			TreeNode root = new TreeNode("Project");
//			root.Tag = new TaskNode(TaskType.Project, "Project");
//				
//			ArrayList departments = MOG_DBProjectAPI.GetDepartments();
//
//			if (departments != null)
//			{
//				foreach (string department in departments)
//				{
//					TaskNode taskHandle = new TaskNode(TaskType.Department, department);
//				
//					TreeNode node = TaskCreateTaskNode(taskHandle);
//					//TreeNode node = new TreeNode(department);
//				
//					//node.Tag = taskHandle;
//					//node.ImageIndex = taskHandle.GetImage();
//
//					// Check for children
//					if (MOG_DBProjectAPI.GetDepartmentUsers(department).Count > 0)
//					{
//						node.Nodes.Add("BLANK");
//					}
//					root.Nodes.Add(node);		
//				}
//			}
//
//			mainForm.ProjectManagerTasksTreeView.Nodes.Add(root);
//			root.Expand();
//		}
//
//		/// <summary>
//		/// Load all the users assigned to a department
//		/// </summary>
//		/// <param name="node"></param>
//		/// <param name="departmentId"></param>
//		/// <returns></returns>
//		private bool LoadUsers(TreeNode node, string departmentId)
//		{
//			ArrayList users = MOG_DBProjectAPI.GetDepartmentUsers(departmentId);
//
//			// Cleanup the tree
//			if ((node.Nodes.Count == 1) && (node.Nodes[0].Text == "BLANK"))
//			{
//				node.Nodes.Clear();
//			}
//
//			foreach (string user in users)
//			{
//				TaskNode taskHandle = new TaskNode(TaskType.User, user);
//				
//				//TreeNode cnode = new TreeNode(user);
//				TreeNode cnode = TaskCreateTaskNode(taskHandle);
//				//cnode.Tag = taskHandle;
//				//cnode.ImageIndex = taskHandle.GetImage();
//
//				// Check for tasks assigned to this user
//				ArrayList test = MOG_DBTaskAPI.GetUserTasks(user);
//				if (test != null && test.Count != 0)
//				{
//					// If we find some, add a dummy blank
//					cnode.Nodes.Add("BLANK");
//				}
//				node.Nodes.Add(cnode);
//			}
//
//			return (node.Nodes.Count > 0);
//		}
//
//		/// <summary>
//		/// Load all the tasks assigned to this user
//		/// </summary>
//		/// <param name="node"></param>
//		/// <param name="userId"></param>
//		/// <returns></returns>
//		private bool LoadTasks(TreeNode node, string userId)
//		{
//			ArrayList tasks = MOG_DBTaskAPI.GetUserTasks(userId);
//
//			// Cleanup the tree
//			node.Nodes.Clear();
//
//			if (tasks != null)
//			{
//				foreach (MOG_TaskInfo task in tasks)
//				{
//					string taskName = task.GetName() + "(" + task.GetPercentComplete() + "%)";
//					
//					TaskNode taskHandle = new TaskNode(TaskType.Task, task.GetName(), task);
//					
//					TreeNode cnode = TaskCreateTaskNode(taskHandle);
//					//TreeNode cnode = new TreeNode(taskName);
//					//cnode.Tag = taskHandle;
//					//cnode.ImageIndex = taskHandle.GetImage();
//
//					// Check if this is assigned to this user
//					if (string.Compare(userId, taskHandle.GetTaskInfo().GetAssignedTo(), true) != 0)
//					{
//						cnode.ImageIndex = cnode.ImageIndex + 2;
//					}
//
//					// Check for any children tasks
//					if (MOG_DBTaskAPI.GetChildrenTasks(taskHandle.GetTaskInfo().GetTaskID()).Count > 0)
//					{
//						// If we find some, add a dummy blank
//						cnode.Nodes.Add("BLANK");
//					}
//					node.Nodes.Add(cnode);
//				}
//			}
//
//			return (node.Nodes.Count > 0);
//		}
//
//		/// <summary>
//		/// Load the tasks not assigned to a user but to a department
//		/// </summary>
//		/// <param name="node"></param>
//		/// <param name="departmentId"></param>
//		/// <returns></returns>
//		private bool LoadFloatingTasks(TreeNode node, string departmentId)
//		{
//			ArrayList tasks = MOG_DBTaskAPI.GetDepartmentTasks(departmentId);;
//
//			// Cleanup the tree
//			node.Nodes.Clear();
//
//			if (tasks != null)
//			{
//				foreach (MOG_TaskInfo task in tasks)
//				{
//					string taskName = task.GetName() + "(" + task.GetPercentComplete() + "%)";
//					
//					TaskNode taskHandle = new TaskNode(TaskType.Task, task.GetName(), task);
//					
//					TreeNode cnode = TaskCreateTaskNode(taskHandle);
//					//TreeNode cnode = new TreeNode(taskName);
//					//cnode.Tag = taskHandle;
//					//cnode.ImageIndex = taskHandle.GetImage();
//
//					
//					// Check for children
//					if (MOG_DBTaskAPI.GetChildrenTasks(taskHandle.GetTaskInfo().GetTaskID()).Count > 0)
//					{
//						// If we find some, add a dummy blank
//						cnode.Nodes.Add("BLANK");
//					}
//					node.Nodes.Add(cnode);
//				}
//			}
//
//			return (node.Nodes.Count > 0);
//		}
//
//		/// <summary>
//		/// Load all the tasks that are sub-tasks of this task
//		/// </summary>
//		/// <param name="node"></param>
//		/// <param name="taskId"></param>
//		/// <returns></returns>
//		private bool LoadChildren(TreeNode node, int taskId)
//		{
//			ArrayList tasks = MOG_DBTaskAPI.GetChildrenTasks(taskId);
//
//			// Cleanup the tree
//			node.Nodes.Clear();
//
//			if (tasks != null)
//			{
//				foreach (MOG_TaskInfo task in tasks)
//				{
//					//string taskName = task.GetName() + "(" + task.GetPercentComplete() + "%)";
//					
//					TaskNode taskHandle = new TaskNode(TaskType.Task, task.GetName(), task);
//					TreeNode cnode = TaskCreateTaskNode(taskHandle);
//					//TreeNode cnode = new TreeNode(taskName);
//					//cnode.Tag = taskHandle;
//					//cnode.ImageIndex = taskHandle.GetImage();
//				
//					
//					// Check if this is assigned to this user
//					string userId = "";
//					TreeNode p = node.Parent;
//					TreeNode c = null;
//					while (userId.Length == 0)
//					{
//						if (p.Parent != null)
//						{
//							c = p;
//							p = p.Parent;
//						}
//						else
//						{
//							userId = c.Text;
//						}
//					}
//
////					if (string.Compare(userId, taskHandle.GetTaskInfo().GetAssignedTo(), true) != 0)
////					{
////						cnode.ImageIndex = cnode.ImageIndex + 2;
////					}
//
//					// Check for children
//					if (MOG_DBTaskAPI.GetChildrenTasks(taskHandle.GetTaskInfo().GetTaskID()).Count > 0)
//					{
//						// If we find some, add a dummy blank
//						cnode.Nodes.Add("BLANK");
//					}
//					node.Nodes.Add(cnode);
//				}
//			}
//
//			return (node.Nodes.Count > 0);
//		}
//
//		/// <summary>
//		/// Load all the tasks that are children of this parent task
//		/// </summary>
//		/// <param name="parent"></param>
//		public void TaskExpandTree(TreeNode parent)
//		{
//			if (parent.Tag != null)
//			{
//				TaskNode task = (TaskNode)parent.Tag;
//				switch(task.GetTaskType())
//				{
//					case TaskType.Department:
//						if (parent.Nodes.Count <= 1)
//						{
//							// Load tasks that have no users
//							LoadFloatingTasks(parent, parent.Text);
//							
//							LoadUsers(parent, parent.Text);
//							//{
//							//	parent.Nodes.Clear();
//							//}
//						}
//						break;
//					case TaskType.User:
//						if (parent.Nodes.Count <= 1)
//						{
//							if (!LoadTasks(parent, parent.Text))
//							{
//								parent.Nodes.Clear();
//							}
//						}
//						break;
//					case TaskType.Task:
//						if (parent.Nodes.Count <= 1)
//						{							
//							if (!LoadChildren(parent, task.GetTaskInfo().GetTaskID()))
//							{
//								parent.Nodes.Clear();
//							}
//						}
//						break;
//				}
//			}
//		}
//		#endregion
//
//		#region Context Menu functions
//		// ******************************************************
//		private void InitializeAssignUsers()
//		{
//			// Only add users to empty menu
//			if (mainForm.ProjectManagerAssignTaskUserMenuItem.MenuItems.Count == 0)
//			{
//				ArrayList departments = MOG_DBProjectAPI.GetDepartments();
//
//				if (departments != null)
//				{
//					foreach (string departmentId in departments)
//					{
//						ArrayList users = MOG_DBProjectAPI.GetDepartmentUsers(departmentId);
//								
//						if (users != null)
//						{
//							foreach (string user in users)
//							{
//								MenuItem Item = new MenuItem(user);
//								Item.Click += new System.EventHandler(this.ProjectManagerTaskAssignUserMenuItem_Click);
//								mainForm.ProjectManagerAssignTaskUserMenuItem.MenuItems.Add(Item);
//							}
//						}
//					}
//				}
//			}
//		}
//
//		// ******************************************************
//		private void InitializeAssignDepartments()
//		{
//			// Only add users to empty menu
//			if (mainForm.ProjectManagerAssignTaskDepartmentMenuItem.MenuItems.Count == 0)
//			{
//				// Add all the users to the Assign subMenu
//				ArrayList departments = MOG_DBProjectAPI.GetDepartments();
//				
//				if (departments != null)
//				{
//					foreach (string department in departments)
//					{
//						MenuItem Item = new MenuItem(department);
//						Item.Click += new System.EventHandler(this.ProjectManagerTaskAssignDepartmentMenuItem_Click);
//						mainForm.ProjectManagerAssignTaskDepartmentMenuItem.MenuItems.Add(Item);
//					}
//				}
//			}			
//		}
//        
//		private void ProjectManagerTaskAssignDepartmentMenuItem_Click(object sender, System.EventArgs e)
//		{
//			TreeNode node = mainForm.ProjectManagerTasksTreeView.SelectedNode;
//			TaskNode taskHandle = (TaskNode)node.Tag;
//
//			if (node != null && taskHandle.GetTaskType() == TaskType.Task)
//			{
//				// Get the specified user
//				MenuItem DepartmentName = (System.Windows.Forms.MenuItem) sender;
//
//				// Ipdate the database
//				taskHandle.GetTaskInfo().SetAssignedTo("");
//				taskHandle.GetTaskInfo().SetDepartment(DepartmentName.Text);
//
//				MOG_DBTaskAPI.UpdateTask(taskHandle.GetTaskInfo());
//
//				// Update the tree
//				node.Remove();
//				AddTaskToTree(mainForm.ProjectManagerTasksTreeView.Nodes, taskHandle);
//			}
//		}
//
//		private void ProjectManagerTaskAssignUserMenuItem_Click(object sender, System.EventArgs e)
//		{
//			TreeNode node = mainForm.ProjectManagerTasksTreeView.SelectedNode;
//			TaskNode taskHandle = (TaskNode)node.Tag;
//
//			if (node != null && taskHandle.GetTaskType() == TaskType.Task)
//			{
//				// Get the specified user
//				MenuItem userName = (System.Windows.Forms.MenuItem) sender;
//
//				// Ipdate the database
//				taskHandle.GetTaskInfo().SetAssignedTo(userName.Text);
//				//taskHandle.GetTaskInfo().SetDepartment(MOG_DBTaskAPI.GetDepartmentName(userName.Text));
//
//				MOG_DBTaskAPI.UpdateTask(taskHandle.GetTaskInfo());
//
//				// Update the tree
//				node.Remove();
//				AddTaskToTree(mainForm.ProjectManagerTasksTreeView.Nodes, taskHandle);
//			}
//		}
//		#endregion
//
//		#region Task Create / Edit / Delete functions
//		/// <summary>
//		/// Add the newly changed or edited task into the tree at the correct spot
//		/// </summary>
//		/// <param name="nodes"></param>
//		/// <param name="newTask"></param>
//		/// <returns></returns>
//		private bool AddTaskToTree(TreeNodeCollection nodes, TaskNode newTask)
//		{
//			// Loop through all the visible nodes of the task tree 
//			foreach(TreeNode node in nodes)
//			{
//				if (node.Tag != null)
//				{
//					TaskNode handle = (TaskNode)node.Tag;
//				
//					switch(handle.GetTaskType())
//					{
//						case TaskType.Project:
//							if (AddTaskToTree(node.Nodes, newTask))
//							{
//								return true;
//							}
//							break;
//							// Locate the correct department first
//						case TaskType.Department:
//							if (string.Compare(node.Text, newTask.GetTaskInfo().GetDepartment(), true) == 0)
//							{
//								// Now determine if this is a task assigned to a department only or to a user within a department
//								if (newTask.GetTaskInfo().GetAssignedTo().Length != 0)
//								{
//									// Ok, search for this user in the current department
//									if (AddTaskToTree(node.Nodes, newTask))
//									{
//										return true;
//									}
//								}
//								else
//								{
//									// Add the department task
//									CreateTreeNode(newTask, node);
//									return true;
//								}
//							}
//							break;
//
//						case TaskType.User:
//							if (string.Compare(node.Text, newTask.GetTaskInfo().GetAssignedTo(), true) == 0)
//							{
//								CreateTreeNode(newTask, node);
//								return true;
//							}
//							break;
//
//						case TaskType.Task:
//							CreateTreeNode(newTask, node);
//							return true;
//					}
//				}
//			}
//
//			return false;
//		}
//
//		public void TaskCreateNew(TreeNode parent)
//		{
//			// Create my task data structures
//			MOG_TaskInfo taskHandle = new MOG_TaskInfo();
//
//			// Open the new task form
//			NewTaskForm taskForm = new NewTaskForm(taskHandle);
//			if (taskForm.ShowDialog() == DialogResult.OK)
//			{
//				TreeNode node = new TreeNode();
//				
//				// Add to the DB
//				ArrayList table = new ArrayList();
//				
//				// Get the user and or department
//				string depId = "Unknown";
//				string userId = "Admin";
//				string parentId = "";
//				TaskNode parentTask;
//				if (parent.Tag != null)
//				{
//					parentTask = (TaskNode)parent.Tag;
//				
//					switch(parentTask.GetTaskType())
//					{
//						case TaskType.Department:
//							depId = parent.Text;
//							break;
//						case TaskType.User:
//							userId = parent.Text;
//							depId = parent.Parent.Text;
//							break;
//						case TaskType.Task:
//							userId = parentTask.GetTaskInfo().GetAssignedTo();
//							depId = parentTask.GetTaskInfo().GetDepartment();
//							parentId = parentTask.GetTaskInfo().GetTaskID().ToString();
//							break;
//					}
//				}
//
//				MOG_Time timestamp = new MOG_Time();
//				MOG_TaskInfo task = new MOG_TaskInfo();
//
//				//task.SetName(taskForm.getName());
//				taskHandle.SetBranch(MOG_ControllerProject.GetBranchName());
//				taskHandle.SetDepartment(depId);
////				taskHandle.SetPriority(taskForm.TaskPriorityComboBox.Text);
//				taskHandle.SetCreator(userId);
//				taskHandle.SetAssignedTo(userId);
//				taskHandle.SetStatus("Assigned");
//				//task.SetPercentComplete(taskForm.getPercentage());
//				taskHandle.SetCreationDate(timestamp.GetTimeStamp());
//				taskHandle.SetDueDate(timestamp.GetTimeStamp());
//				taskHandle.SetCompletionDate(timestamp.GetTimeStamp());
//				taskHandle.SetAsset("None");
//				taskHandle.SetComment("None");
//
//				MOG_TaskInfo newTask;
//				newTask = MOG_DBTaskAPI.CreateTask(taskHandle);
//				
//				if (newTask != null)
//				{
//					if (parentId.Length != 0)
//					{
//						// Ipdate the LINKS database
//						MOG_DBTaskAPI.SetParentTaskID(newTask, Convert.ToInt32(parentId));
//					}
//
//					// Get the task name
//					node.Text = newTask.GetName() + "(" + newTask.GetPercentComplete() + "%)";
//					node.Tag = new TaskNode(TaskType.Task, newTask.GetName(), newTask);
//
//					// Add the node
//					parent.Nodes.Add(node);
//					parent.Expand();
//				}
//			}
//		}
//
//		public void TaskDelete(TreeNode parent)
//		{
//			if (parent.Tag != null)
//			{
//				if (parent.Nodes.Count > 0)
//				{
//					foreach (TreeNode node in parent.Nodes)
//					{
//						// Find the task
//						TaskNode task = (TaskNode)node.Tag;
//
//						// Delete from datebase
//						if (MOG_DBTaskAPI.RemoveTask(task.GetTaskInfo().GetTaskID()))
//						{
//							// Delete from tree
//							TaskDelete(node);
//						}
//					}
//				}
//				else
//				{
//					// Find the task
//					TaskNode task = (TaskNode)parent.Tag;
//
//					// Delete from datebase
//					if (MOG_DBTaskAPI.RemoveTask(task.GetTaskInfo().GetTaskID()))
//					{
//						// Delete from tree
//						parent.TreeView.Nodes.Remove(parent);
//					}
//				}
//			}
//		}
//
//		public void TaskTreeEdit(TreeNode node)
//		{
////			if (node != null)
////			{
////				TaskNode task = (TaskNode)node.Tag;
////				if (task.GetTaskInfo() != null)
////				{
////					NewTaskForm taskForm = new NewTaskForm(task.GetTaskInfo());
////				
////					// If OK, update the database
////					if (taskForm.ShowDialog(mainForm) == DialogResult.OK)
////					{
////						bool complete = false;
////
////						// Ipdate the database
////						complete = MOG_DBTaskAPI.UpdateTask(taskForm.GetTaskInfo());
////
////						if (complete)
////						{
////							TaskNode handle = new TaskNode(TaskType.Task, taskForm.GetTaskInfo().GetName(), taskForm.GetTaskInfo());
////							node.Tag = handle;
////							node.Text = handle.GetExtendedName(handle.GetTaskInfo().GetAssignedTo());
////						}
////					}
////				}
////			}
//		}
//
//		#endregion

//		#region ListView Population functions
//		/// <summary>
//		/// Update the listView of all tasks within the range specified by the tree view
//		/// </summary>
//		/// <param name="parent"></param>
//		/// <param name="level"></param>
//		private void UpdateTaskListWalkChildren(TreeNode parent, int level)
//		{
//			// Find the task
//			TaskNode task = (TaskNode)parent.Tag;
//			ArrayList tasks = null;
//
//			if (task != null)
//			{
//				switch(task.GetTaskType())
//				{
//					case TaskType.Project:
//						ArrayList departments = MOG_DBProjectAPI.GetDepartments();
//
//						tasks = new ArrayList();
//						if (departments != null)
//						{
//							foreach (string department in departments)
//							{
//								ArrayList departmentTasks = MOG_DBTaskAPI.GetDepartmentTasks(department);
//								if (departmentTasks != null)
//								{
//									foreach (MOG_TaskInfo departmentTask in departmentTasks)
//									{
//										tasks.Add(departmentTask);
//									}
//								}
//
//								foreach (string user in MOG_DBProjectAPI.GetDepartmentUsers(department))
//								{
//									ArrayList userTasks = MOG_DBTaskAPI.GetAllUserTasks(user);
//									if (userTasks != null)
//									{
//										foreach (MOG_TaskInfo userTask in userTasks)
//										{
//											tasks.Add(userTask);
//										}
//									}
//								}								
//							}
//						}
//						break;
//					case TaskType.Department:
//						tasks = MOG_DBTaskAPI.GetDepartmentTasks(task.GetName());
//						if (tasks != null)
//						{
//							foreach (string user in MOG_DBProjectAPI.GetDepartmentUsers(task.GetName()))
//							{
//								ArrayList userTasks = MOG_DBTaskAPI.GetAllUserTasks(user);
//								if (userTasks != null)
//								{
//									foreach (MOG_TaskInfo userTask in userTasks)
//									{
//										tasks.Add(userTask);
//									}
//								}
//							}
//						}
//						break;
//					case TaskType.User:
//						tasks = MOG_DBTaskAPI.GetAllUserTasks(task.GetName());
//						break;
//					case TaskType.Task:
//						tasks = MOG_DBTaskAPI.GetChildrenTasks(task.GetTaskInfo().GetTaskID());
//						break;
//				}
//
//				if (tasks != null)
//				{
//					foreach (MOG_TaskInfo taskInfo in tasks)
//					{
//						mainForm.ProjectManagerTasksListView.Items.Add(TaskCreateListViewItem(taskInfo));				
//					}
//				}
//			}
//		}
//	
//		/// <summary>
//		/// Create a new listView item based on a TaskInfo
//		/// </summary>
//		/// <param name="taskInfo"></param>
//		/// <returns></returns>
//		private ListViewItem TaskCreateListViewItem(MOG_TaskInfo taskInfo)
//		{
//			MOG_Time createTime = new MOG_Time(taskInfo.GetCreationDate());
//			MOG_Time completeTime = new MOG_Time(taskInfo.GetCompletionDate());
//
//			ListViewItem item = new ListViewItem();
//
//			item.Text = taskInfo.GetName();
//			item.SubItems.Add(taskInfo.GetDepartment());
//			item.SubItems.Add(taskInfo.GetCreator());
//			item.SubItems.Add(createTime.FormatString(""));
//			item.SubItems.Add(completeTime.FormatString(""));
//			item.SubItems.Add(taskInfo.GetStatus());
//			item.SubItems.Add(taskInfo.GetPercentComplete().ToString() + "%");
//			item.SubItems.Add(taskInfo.GetAssignedTo());
//			item.SubItems.Add(taskInfo.GetPriority());
//			
//			TaskNode taskHandle = new TaskNode(TaskType.Task,taskInfo.GetName());
//			item.ImageIndex = taskHandle.GetImage();
//
//			// Record the task id for easy future lookup
//			item.Tag = taskInfo.GetTaskID();
//
//			switch(taskInfo.GetPriority())
//			{
//				case "Low":
//					item.ForeColor = Color.Blue;
//					break;
//				case "Medium":
//					item.ForeColor = Color.BlueViolet;
//					break;
//				case "High":
//					item.ForeColor = Color.Purple;
//					break;
//				case "Urgent":
//					item.ForeColor = Color.Red;
//					break;
//			}
//
//			return item;
//		}
//
//		/// <summary>
//		/// Open task from the ListView
//		/// </summary>
//		public void TaskListEdit(ListView.SelectedListViewItemCollection SelectedItems)
//		{
//			foreach (ListViewItem item in SelectedItems)
//			{
//				NewTaskForm taskForm = new NewTaskForm(MOG_DBTaskAPI.GetTask((int)item.Tag));
//				
//				// If OK, update the database
//				if (taskForm.ShowDialog(mainForm) == DialogResult.OK)
//				{
//					bool complete = false;
//
//					// Ipdate the database
//					complete = MOG_DBTaskAPI.UpdateTask(taskForm.MOGTaskInfo);
//
//					if (complete)
//					{
//						item.Remove();
//						mainForm.ProjectManagerTasksListView.Items.Add(TaskCreateListViewItem(taskForm.MOGTaskInfo));
//					}
//				}
//			}
//		}
//
//		/// <summary>
//		/// Main update function for the task ListView
//		/// </summary>
//		/// <param name="parent"></param>
//		public void TaskUpdateList(TreeNode parent)
//		{
//			mainForm.ProjectManagerTasksListView.Items.Clear();
//			UpdateTaskListWalkChildren(parent, 0);
//		}
//		#endregion		
	}
}


