using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using MOG.CONTROLLER.CONTROLLERPROJECT;

using MOG_Client.Client_Gui;
using System.Collections.Generic;

namespace MOG_Client.Forms
{
	/// <summary>
	/// Summary description for LoginForm.
	/// </summary>
	public class LoginForm : System.Windows.Forms.Form
	{
		private string mDesiredProject;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label3;
		public System.Windows.Forms.ComboBox LoginUsersComboBox;
		private System.Windows.Forms.Button LoginButton;
		private System.Windows.Forms.Button LoginCancelButton;
		private System.Windows.Forms.Label label2;
		public System.Windows.Forms.ComboBox LoginBranchesComboBox;
		public System.Windows.Forms.ComboBox LoginProjectsComboBox;
		private System.Windows.Forms.TextBox textBox1;
		private Label label4;
		public ComboBox LoginDepartmentComboBox;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Constuctor for the login form
		/// </summary>
		/// <param name="desiredProject"></param>
		public LoginForm(string desiredProject)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			mDesiredProject = desiredProject;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
			this.LoginUsersComboBox = new System.Windows.Forms.ComboBox();
			this.LoginButton = new System.Windows.Forms.Button();
			this.LoginCancelButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.LoginBranchesComboBox = new System.Windows.Forms.ComboBox();
			this.LoginProjectsComboBox = new System.Windows.Forms.ComboBox();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.LoginDepartmentComboBox = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// LoginUsersComboBox
			// 
			this.LoginUsersComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.LoginUsersComboBox.Location = new System.Drawing.Point(104, 86);
			this.LoginUsersComboBox.Name = "LoginUsersComboBox";
			this.LoginUsersComboBox.Size = new System.Drawing.Size(216, 21);
			this.LoginUsersComboBox.Sorted = true;
			this.LoginUsersComboBox.TabIndex = 2;
			// 
			// LoginButton
			// 
			this.LoginButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.LoginButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.LoginButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.LoginButton.Location = new System.Drawing.Point(158, 140);
			this.LoginButton.Name = "LoginButton";
			this.LoginButton.Size = new System.Drawing.Size(75, 23);
			this.LoginButton.TabIndex = 4;
			this.LoginButton.Text = "Login";
			// 
			// LoginCancelButton
			// 
			this.LoginCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.LoginCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.LoginCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.LoginCancelButton.Location = new System.Drawing.Point(238, 140);
			this.LoginCancelButton.Name = "LoginCancelButton";
			this.LoginCancelButton.Size = new System.Drawing.Size(75, 23);
			this.LoginCancelButton.TabIndex = 5;
			this.LoginCancelButton.Text = "Cancel";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(88, 23);
			this.label1.TabIndex = 3;
			this.label1.Text = "Login Project:";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 86);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(88, 23);
			this.label3.TabIndex = 5;
			this.label3.Text = "Login User:";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 32);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(88, 23);
			this.label2.TabIndex = 7;
			this.label2.Text = "Project Branch:";
			this.label2.Visible = false;
			// 
			// LoginBranchesComboBox
			// 
			this.LoginBranchesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.LoginBranchesComboBox.Location = new System.Drawing.Point(104, 32);
			this.LoginBranchesComboBox.Name = "LoginBranchesComboBox";
			this.LoginBranchesComboBox.Size = new System.Drawing.Size(216, 21);
			this.LoginBranchesComboBox.Sorted = true;
			this.LoginBranchesComboBox.TabIndex = 1;
			this.LoginBranchesComboBox.Text = "Current";
			this.LoginBranchesComboBox.Visible = false;
			// 
			// LoginProjectsComboBox
			// 
			this.LoginProjectsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.LoginProjectsComboBox.Location = new System.Drawing.Point(104, 8);
			this.LoginProjectsComboBox.Name = "LoginProjectsComboBox";
			this.LoginProjectsComboBox.Size = new System.Drawing.Size(216, 21);
			this.LoginProjectsComboBox.Sorted = true;
			this.LoginProjectsComboBox.TabIndex = 0;
			this.LoginProjectsComboBox.SelectedIndexChanged += new System.EventHandler(this.LoginProjectsComboBox_SelectedIndexChanged);
			// 
			// textBox1
			// 
			this.textBox1.Enabled = false;
			this.textBox1.Location = new System.Drawing.Point(104, 113);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(216, 20);
			this.textBox1.TabIndex = 3;
			this.textBox1.Text = "Password";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(8, 59);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(88, 23);
			this.label4.TabIndex = 9;
			this.label4.Text = "Department:";
			// 
			// LoginDepartmentComboBox
			// 
			this.LoginDepartmentComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.LoginDepartmentComboBox.Location = new System.Drawing.Point(104, 59);
			this.LoginDepartmentComboBox.Name = "LoginDepartmentComboBox";
			this.LoginDepartmentComboBox.Size = new System.Drawing.Size(216, 21);
			this.LoginDepartmentComboBox.Sorted = true;
			this.LoginDepartmentComboBox.TabIndex = 8;
			this.LoginDepartmentComboBox.SelectedIndexChanged += new System.EventHandler(this.LoginDepartmentComboBox_SelectedIndexChanged);
			// 
			// LoginForm
			// 
			this.AcceptButton = this.LoginButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.LoginCancelButton;
			this.ClientSize = new System.Drawing.Size(326, 169);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.LoginDepartmentComboBox);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.LoginProjectsComboBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.LoginBranchesComboBox);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.LoginCancelButton);
			this.Controls.Add(this.LoginButton);
			this.Controls.Add(this.LoginUsersComboBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "LoginForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Login to project";
			this.Load += new System.EventHandler(this.LoginForm_Activated);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private void LoginForm_Activated(object sender, EventArgs e)
		{
			//Fill the combo box up with a list of available projects
			List<string> projects = guiStartup.GetProjects();
			LoginProjectsComboBox.Items.Clear();
			LoginProjectsComboBox.Items.AddRange(projects.ToArray());
			if (projects.Contains(mDesiredProject))
			{
				LoginProjectsComboBox.SelectedItem = mDesiredProject;
			}
		}

		private void LoginProjectsComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			// Fill in our ComboBoxes through guiStartup
			List<string> branches = guiStartup.GetBranches(LoginProjectsComboBox.Text);
			LoginBranchesComboBox.Items.Clear();
			LoginBranchesComboBox.Items.AddRange(branches.ToArray());
			if (LoginBranchesComboBox.Items.Contains(MOG_ControllerProject.GetBranchName()))
			{
				LoginBranchesComboBox.SelectedItem = MOG_ControllerProject.GetBranchName();
			}

			List<string> departments = guiStartup.GetDepartments(LoginProjectsComboBox.Text);
			LoginDepartmentComboBox.Items.Clear();
			LoginDepartmentComboBox.Items.AddRange(departments.ToArray());
			if (LoginDepartmentComboBox.Items.Contains(MOG_ControllerProject.GetDepartment()))
			{
				LoginDepartmentComboBox.SelectedItem = MOG_ControllerProject.GetDepartment();
			}
		}

		private void LoginDepartmentComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			List<string> users = guiStartup.GetUsers(LoginProjectsComboBox.Text, LoginDepartmentComboBox.Text);
			LoginUsersComboBox.Items.Clear();
			LoginUsersComboBox.Items.AddRange(users.ToArray());
			if (LoginUsersComboBox.Items.Contains(MOG_ControllerProject.GetUserName_DefaultAdmin()))
			{
				LoginUsersComboBox.SelectedItem = MOG_ControllerProject.GetUserName_DefaultAdmin();
			}
		}
	}
	
}
