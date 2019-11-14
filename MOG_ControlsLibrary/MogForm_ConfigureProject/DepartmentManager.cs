using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using MOG.PROJECT;
using MOG.USER;
using MOG.PROMPT;
using System.Collections;

namespace MOG_ControlsLibrary
{
	public partial class DepartmentManager : UserControl
	{
		MOG_Project mProject = null;
		
		public DepartmentManager()
		{
			InitializeComponent();
		}

		public void LoadFromProject(MOG_Project project)
		{
			mProject = project;

			foreach (MOG_UserDepartment department in project.GetUserDepartments())
			{
				AddDepartment(department.mDepartmentName);
			}

			foreach (ColumnHeader column in DepartmentList.Columns)
			{
				column.Width = DepartmentList.Width - 1;
			}
		}

		public void AddDepartment(string departmentName)
		{
			if (mProject != null)
			{
				mProject.UserDepartmentAdd(departmentName);
			}

			if (DepartmentList.FindItemWithText(departmentName) == null)
			{
				DepartmentList.Items.Add(departmentName);
			}
		}

		public void RemoveDepartment(string departmentName)
		{
			if (mProject != null)
			{
				ArrayList usingUsers = mProject.GetDepartmentUsers(departmentName);
				if (usingUsers != null && usingUsers.Count > 0)
				{
					MOG_Prompt.PromptMessage("Problem removing department", "This department cannot be removed because there are users still assigned to it.");
				}
				else
				{
					if (mProject.UserDepartmentRemove(departmentName))
					{
						ListViewItem item = DepartmentList.FindItemWithText(departmentName);
						if (item != null)
						{
							DepartmentList.Items.Remove(item);
						}
					}
				}
			}
		}

		private void RemoveDepartmentButton_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem item in DepartmentList.SelectedItems)
			{
				RemoveDepartment(item.Text);
			}
		}

		private void NewDepartmentButton_Click(object sender, EventArgs e)
		{
			AddDepartmentForm form = new AddDepartmentForm(this);
			form.ShowDialog(this);
		}

		private void DepartmentList_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			RemoveDepartmentButton.Enabled = (DepartmentList.SelectedItems.Count > 0);
		}
	}
}
