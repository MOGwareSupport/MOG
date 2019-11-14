using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;

using MOG;
using MOG.REPORT;
using MOG.TIME;
using MOG.DATABASE;
using MOG.CONTROLLER.CONTROLLERPROJECT;

//using MOG_ColumnTreeView = GlacialComponents.Controls.GlacialTreeList.GlacialTreeList;
//using MOG_ColumnTreeNode = GlacialComponents.Controls.GlacialTreeList.GTLTreeNode;
using MOG_ColumnTreeView = System.Windows.Forms.TreeView;
using MOG_ColumnTreeNode = System.Windows.Forms.TreeNode;


namespace MOG_Client.Client_Gui.ProjectManager_Helper
{
	public enum TaskType {Project, Department, User, Task};
	public enum DepartmentsCols {id, name};
	public enum UsersCols {id, name, department_id};
	public enum TasksCols {id, name, department_id, assignedTo_id, Comment, PercentComplete, Creator_id, CreationDate, CompletionDate, Status_id, priority_id, Asset, Project_id, Branch_id, TimeStamp, ResponsibleParty_id, Link_id, Parent_id, Child_Id};
	/// <summary>
	/// Summary description for TaskNode.
	/// </summary>
	public class TaskNode
	{

		private TaskType mType;
		private string mName;
		//private object mDepartmentInfo;
		//private object mUserInfo;
		private object mTaskInfo;
		
		public TaskNode(TaskType type, string name)
		{
			mType = type;
			mName = name;
			mTaskInfo = null;
		}

		public TaskType GetTaskType()
		{
			return mType;
		}

		public TaskNode(TaskType type, string name, object data)
		{
			mType = type;
			mName = name;

			switch(mType)
			{
				case TaskType.Task:
					mTaskInfo = data;
					break;
			}
		}

		public string GetName()
		{
			return mName;
		}

		public string GetExtendedName(string currentUser)
		{
			if (GetTaskInfo() != null)
			{
				if (string.Compare(GetTaskInfo().GetAssignedTo(), currentUser, true) != 0)
				{
					if (string.Compare(GetTaskInfo().GetCreator(), currentUser, true) == 0)
					{
						return GetTaskInfo().GetAssignedTo() + " working on - " + GetName() + "(" + GetTaskInfo().GetPercentComplete() + "%)";
					}
					else
					{						
						return GetTaskInfo().GetCreator() + " needs - " + GetName() + "(" + GetTaskInfo().GetPercentComplete() + "%) - from " + GetTaskInfo().GetAssignedTo();
					}
				}

				return GetName() + "(" + GetTaskInfo().GetPercentComplete() + "%)";
			}
			
			return GetName();
		}

		public Font GetNodeFont(TreeView tree)
		{
			if (GetTaskInfo() != null)
			{
				if (GetTaskInfo().GetPercentComplete() >= 100)
				{
					return new System.Drawing.Font(tree.Font.FontFamily, tree.Font.Size, FontStyle.Strikeout, tree.Font.Unit, tree.Font.GdiCharSet, tree.Font.GdiVerticalFont);
				}
			}
			
			return new System.Drawing.Font(tree.Font.FontFamily, tree.Font.Size, FontStyle.Regular, tree.Font.Unit, tree.Font.GdiCharSet, tree.Font.GdiVerticalFont);
		}

//		public Font GetNodeFont(MOG_ColumnTreeView tree)
//		{
//			if (GetTaskInfo() != null)
//			{
//				if (GetTaskInfo().GetPercentComplete() >= 100)
//				{
//					return new System.Drawing.Font(tree.Font.FontFamily, tree.Font.Size, FontStyle.Strikeout, tree.Font.Unit, tree.Font.GdiCharSet, tree.Font.GdiVerticalFont);
//				}
//			}
//			
//			return new System.Drawing.Font(tree.Font.FontFamily, tree.Font.Size, FontStyle.Regular, tree.Font.Unit, tree.Font.GdiCharSet, tree.Font.GdiVerticalFont);
//		}

		public Color GetNodeFontColor(MOG_ColumnTreeView tree)
		{
			MOG_Time current = new MOG_Time();

			if (GetTaskInfo() != null)
			{
				if (current.Compare(new MOG_Time(GetTaskInfo().GetDueDate())) > 0)
				{
					return Color.Red;
				}
			}

			return SystemColors.ControlText;
		}
		
		public System.Windows.Forms.CheckState GetChecked()
		{
			if (GetTaskInfo() != null)
			{
				if (GetTaskInfo().GetPercentComplete() >= 100)
				{
					return CheckState.Checked;
				}
			}

			return CheckState.Unchecked;
		}

		public int GetImage()
		{
			switch(mType)
			{
				case TaskType.Department:
					return 1;
				case TaskType.User:
					return 2;
				case TaskType.Task:
					// Check if this is assigned to this user
					string userId = MOG_ControllerProject.GetUser().GetUserName();
					
					if (mTaskInfo != null)
					{
						if (string.Compare(userId, GetTaskInfo().GetAssignedTo(), true) != 0)
						{
							return 4; // This is assigned to someone else
						}
						else
						{
							// Check to see if this is a child task
							if (GetTaskInfo().GetParent().Length > 0)
							{
								return 6;
							}
							else
							{
								return 3; // This is assigned to me
							}
						}
					}
					else
					{
						return 3;
					}
			}
			return 7;
		}

		public MOG_TaskInfo GetTaskInfo()
		{
			if (mType == TaskType.Task)
			{
				return (MOG_TaskInfo)mTaskInfo;
			}

			return null;
		}

		public string GenerateUpdateCMD()
		{
			return "";
		}
	}
}
