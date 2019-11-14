using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.PROJECT;

namespace MOG_ControlsLibrary
{
	public partial class AddDepartmentForm : Form
	{
		private DepartmentManager mDepartmentManager;

		public AddDepartmentForm(DepartmentManager deptManager)
		{
			mDepartmentManager = deptManager;

			InitializeComponent();
		}

		private void AddButton_Click(object sender, EventArgs e)
		{
			mDepartmentManager.AddDepartment(DepartmentName.Text);
		}

		private void CloseButton_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void AddAndCloseButton_Click(object sender, EventArgs e)
		{
			AddButton.PerformClick();
			CloseButton.PerformClick();
		}
	}
}