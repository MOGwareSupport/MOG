using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MOG.USER;
using MOG.PROJECT;
using MOG.PROMPT;
using MOG;
using MOG.CONTROLLER.CONTROLLERSYSTEM;

namespace MOG_ControlsLibrary
{
	public partial class EditUserForm : Form
	{
		MOG_Project mProject;
		MOGUserManager mUserManager;
		MOG_Privileges mPrivileges;
		MOG_User mUser;

		public EditUserForm(MOGUserManager userManager, MOG_Project project, MOG_Privileges privileges, MOG_User user)
		{
			mProject = project;
			mUserManager = userManager;
			mUser = user;
			mPrivileges = privileges;

			InitializeComponent();
		}

		private void PopulateDepartments()
		{
			if (mProject != null)
			{
				// Add all known user departments
				foreach (MOG_UserDepartment department in mProject.GetUserDepartments())
				{
					if (!Departments.Items.Contains(department.mDepartmentName))
					{
						Departments.Items.Add(department.mDepartmentName);
					}
				}

				Departments.SelectedIndex = 0;
			}
		}

		private void PopulatePrivileges()
		{
			Privileges.Items.Clear();

			if (mPrivileges != null)
			{
				foreach (string group in mPrivileges.GroupsList)
				{
					Privileges.Items.Add(group);
				}

				Privileges.SelectedIndex = 0;
			}
		}
		
		private void PopulateBlessTargets()
		{
			// reset to only MasterData
			BlessTargets.Items.Clear();
			BlessTargets.Items.Add("MasterData");

			if (mProject != null)
			{
				foreach (MOG_User user in mProject.GetUsers())
				{
					BlessTargets.Items.Add(user.GetUserName());
				}
			}

			BlessTargets.SelectedIndex = 0;
		}
		
		private void EditUserForm_Load(object sender, EventArgs e)
		{
			PopulateDepartments();
			PopulatePrivileges();
			PopulateBlessTargets();

			if (mUser != null)
			{
				AddButton.Visible = false;
				AddAndCloseButton.Text = "Update";

				Username.Text = mUser.GetUserName();
				Email.Text = mUser.GetUserEmailAddress();
				Departments.Text = mUser.GetUserDepartment();
				Privileges.Text = mPrivileges.GetUserGroup(mUser.GetUserName());
				BlessTargets.Text = mUser.GetBlessTarget();

				// Check if this is the Admin User?
				if (string.Compare(mUser.GetUserName(), "Admin", true) == 0)
				{
					Departments.Enabled = false;
					Privileges.Enabled = false;
					BlessTargets.Enabled = false;
					Username.Enabled = false;
				}
			}
		}

		public void AddUser(string name, string department, string email, string blessTarget, string privileges)
		{
			MOG_User user = new MOG_User();
			user.SetUserName(name);
			user.SetUserEmailAddress(email);
			user.SetUserDepartment(department);
			user.SetBlessTarget(blessTarget);
			mUserManager.AddUser(user, privileges);

			PopulateBlessTargets();
		}

		public void UpdateUser(string name, string department, string email, string blessTarget, string privileges)
		{
			MOG_User user = new MOG_User();
			user.SetUserName(name);
			user.SetUserEmailAddress(email);
			user.SetUserDepartment(department);
			user.SetBlessTarget(blessTarget);
			mUserManager.UpdateUser(mUser.GetUserName(), user, privileges);

			PopulateBlessTargets();
		}

		private bool VerifyFields()
		{
			// check username
			if (String.IsNullOrEmpty(Username.Text))
			{
				MOG_Prompt.PromptResponse("Missing Data", "Please enter a user name");
				Username.Focus();
				return false;
			}
			if (Email.Text.IndexOf(" ") != -1)
			{
				MOG_Prompt.PromptResponse("Invalid Character", "Email addresses cannot contian spaces");
				Email.Focus();
				return false;
			}

			if (MOG_ControllerSystem.InvalidMOGCharactersCheck(Username.Text, false))
			{
				MOG_Prompt.PromptResponse("Please fix the user name", "Invalid Character in user name.  Fix this and try again.");
				Username.Focus();
				return false;
			}			
			
			if (String.IsNullOrEmpty(Departments.Text))
			{
				MOG_Prompt.PromptResponse("Missing Data", "Please enter a user category");
				Departments.Focus();
				return false;
			}

			if (String.IsNullOrEmpty(Privileges.Text))
			{
				MOG_Prompt.PromptResponse("Missing Data", "Please enter a user privilege group");
				Privileges.Focus();
				return false;
			}

			// fixup hand-entered bless target
			foreach (string blessTarget in BlessTargets.Items)
			{
				if (String.Compare(blessTarget, BlessTargets.Text, true) == 0)
				{
					BlessTargets.SelectedItem = blessTarget;
				}
			}

			if (String.Compare(BlessTargets.Text, Username.Text, true) == 0 &&
				!BlessTargets.Items.Contains(Username.Text))
			{
				BlessTargets.Items.Add(Username.Text);
				BlessTargets.SelectedItem = Username.Text;
			}

			if (String.IsNullOrEmpty(BlessTargets.SelectedItem as string))
			{
				MOG_Prompt.PromptResponse("Missing Data", "Please enter a user bless target");
				BlessTargets.Focus();
				return false;
			}

			return true;
		}

		private void AddButton_Click(object sender, EventArgs e)
		{
			AddUser();
		}

		private bool AddUser()
		{
			if (VerifyFields())
			{
				if (mUser != null)
				{
					UpdateUser(Username.Text, Departments.Text, Email.Text, BlessTargets.Text, Privileges.Text);
				}
				else
				{
					AddUser(Username.Text, Departments.Text, Email.Text, BlessTargets.Text, Privileges.Text);
				}

				return true;
			}

			return false;
		}

		private void CloseButton_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void AddAndCloseButton_Click(object sender, EventArgs e)
		{
			if (AddUser())
			{
				CloseButton.PerformClick();
			}
		}
	}
}