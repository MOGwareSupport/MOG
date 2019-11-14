using System;
using System.Data;
using System.Drawing;
using System.Net.Mail;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;

using MOG;
using MOG.USER;
using MOG.PROMPT;
using MOG.PROJECT;
using MOG.PROGRESS;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using System.Collections.Generic;
using EV.Windows.Forms;
using MOG_CoreControls;

namespace MOG_ControlsLibrary
{
	/// <summary>
	/// Summary description for MOGUserManager.
	/// </summary>
	public class MOGUserManager : System.Windows.Forms.UserControl
	{
		private MOG_Project mProject = null;
		private MOG_Privileges mPrivileges = new MOG_Privileges();

		#region System definitions
		private System.Windows.Forms.Button btnUserEdit;
		private System.Windows.Forms.Button btnRemoveUser;
		private System.Windows.Forms.ListView lvUsers;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.Button btnAddUser;
		private System.Windows.Forms.ContextMenuStrip usersContextMenu;
		private System.Windows.Forms.ToolStripMenuItem EditUserItem;
		private System.Windows.Forms.ToolStripMenuItem RemoveUserItem;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.ColumnHeader chPriveleges;
		private System.Windows.Forms.TextBox tbEmailSMTP;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button btnTestSMTP;
		private ToolStripMenuItem AddNewUserItem;
		private System.ComponentModel.IContainer components;

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.btnUserEdit = new System.Windows.Forms.Button();
			this.btnRemoveUser = new System.Windows.Forms.Button();
			this.lvUsers = new System.Windows.Forms.ListView();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.chPriveleges = new System.Windows.Forms.ColumnHeader();
			this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
			this.usersContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.AddNewUserItem = new System.Windows.Forms.ToolStripMenuItem();
			this.EditUserItem = new System.Windows.Forms.ToolStripMenuItem();
			this.RemoveUserItem = new System.Windows.Forms.ToolStripMenuItem();
			this.btnAddUser = new System.Windows.Forms.Button();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.tbEmailSMTP = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.btnTestSMTP = new System.Windows.Forms.Button();
			this.usersContextMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnUserEdit
			// 
			this.btnUserEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnUserEdit.Enabled = false;
			this.btnUserEdit.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnUserEdit.Location = new System.Drawing.Point(444, 408);
			this.btnUserEdit.Name = "btnUserEdit";
			this.btnUserEdit.Size = new System.Drawing.Size(75, 23);
			this.btnUserEdit.TabIndex = 4;
			this.btnUserEdit.Text = "&Edit";
			this.btnUserEdit.Click += new System.EventHandler(this.btnUserEdit_Click);
			// 
			// btnRemoveUser
			// 
			this.btnRemoveUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnRemoveUser.Enabled = false;
			this.btnRemoveUser.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnRemoveUser.Location = new System.Drawing.Point(525, 408);
			this.btnRemoveUser.Name = "btnRemoveUser";
			this.btnRemoveUser.Size = new System.Drawing.Size(75, 23);
			this.btnRemoveUser.TabIndex = 5;
			this.btnRemoveUser.Text = "&Remove";
			this.btnRemoveUser.Click += new System.EventHandler(this.btnRemoveUser_Click);
			// 
			// lvUsers
			// 
			this.lvUsers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lvUsers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2,
            this.columnHeader1,
            this.columnHeader3,
            this.chPriveleges,
            this.columnHeader4});
			this.lvUsers.ContextMenuStrip = this.usersContextMenu;
			this.lvUsers.FullRowSelect = true;
			this.lvUsers.HideSelection = false;
			this.lvUsers.Location = new System.Drawing.Point(16, 123);
			this.lvUsers.Name = "lvUsers";
			this.lvUsers.Size = new System.Drawing.Size(584, 279);
			this.lvUsers.TabIndex = 2;
			this.lvUsers.UseCompatibleStateImageBehavior = false;
			this.lvUsers.View = System.Windows.Forms.View.Details;
			this.lvUsers.DoubleClick += new System.EventHandler(this.lvUsers_DoubleClick);
			this.lvUsers.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvUsers_ItemSelectionChanged);
			this.lvUsers.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.lvUsers_KeyPress);
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Username";
			this.columnHeader2.Width = 129;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Department";
			this.columnHeader1.Width = 100;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "E-mail Address";
			this.columnHeader3.Width = 154;
			// 
			// chPriveleges
			// 
			this.chPriveleges.Text = "Priveleges";
			this.chPriveleges.Width = 98;
			// 
			// columnHeader4
			// 
			this.columnHeader4.Text = "Bless Target";
			this.columnHeader4.Width = 100;
			// 
			// usersContextMenu
			// 
			this.usersContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AddNewUserItem,
            this.EditUserItem,
            this.RemoveUserItem});
			this.usersContextMenu.Name = "usersContextMenu";
			this.usersContextMenu.Size = new System.Drawing.Size(147, 70);
			// 
			// AddNewUserItem
			// 
			this.AddNewUserItem.Name = "AddNewUserItem";
			this.AddNewUserItem.Size = new System.Drawing.Size(146, 22);
			this.AddNewUserItem.Text = "Add New...";
			this.AddNewUserItem.Click += new System.EventHandler(this.btnAddUser_Click);
			// 
			// EditUserItem
			// 
			this.EditUserItem.Name = "EditUserItem";
			this.EditUserItem.Size = new System.Drawing.Size(146, 22);
			this.EditUserItem.Text = "Edit...";
			this.EditUserItem.Click += new System.EventHandler(this.btnUserEdit_Click);
			// 
			// RemoveUserItem
			// 
			this.RemoveUserItem.Name = "RemoveUserItem";
			this.RemoveUserItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
			this.RemoveUserItem.Size = new System.Drawing.Size(146, 22);
			this.RemoveUserItem.Text = "Remove";
			this.RemoveUserItem.Click += new System.EventHandler(this.btnRemoveUser_Click);
			// 
			// btnAddUser
			// 
			this.btnAddUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAddUser.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnAddUser.Location = new System.Drawing.Point(357, 408);
			this.btnAddUser.Name = "btnAddUser";
			this.btnAddUser.Size = new System.Drawing.Size(81, 23);
			this.btnAddUser.TabIndex = 3;
			this.btnAddUser.Text = "&Add New";
			this.btnAddUser.Click += new System.EventHandler(this.btnAddUser_Click);
			// 
			// tbEmailSMTP
			// 
			this.tbEmailSMTP.Location = new System.Drawing.Point(152, 40);
			this.tbEmailSMTP.Name = "tbEmailSMTP";
			this.tbEmailSMTP.Size = new System.Drawing.Size(168, 20);
			this.tbEmailSMTP.TabIndex = 0;
			this.tbEmailSMTP.Validated += new System.EventHandler(this.tbEmailSMTP_Validated);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(24, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(120, 16);
			this.label2.TabIndex = 71;
			this.label2.Text = "E-mail SMTP Address";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Location = new System.Drawing.Point(16, 80);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(584, 8);
			this.groupBox1.TabIndex = 72;
			this.groupBox1.TabStop = false;
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(16, 8);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(112, 16);
			this.label3.TabIndex = 73;
			this.label3.Text = "Global User Settings";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(16, 104);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(40, 16);
			this.label4.TabIndex = 74;
			this.label4.Text = "Users";
			this.label4.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// btnTestSMTP
			// 
			this.btnTestSMTP.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnTestSMTP.Location = new System.Drawing.Point(326, 38);
			this.btnTestSMTP.Name = "btnTestSMTP";
			this.btnTestSMTP.Size = new System.Drawing.Size(75, 23);
			this.btnTestSMTP.TabIndex = 1;
			this.btnTestSMTP.Text = "Test";
			this.btnTestSMTP.Click += new System.EventHandler(this.btnTestSMTP_Click);
			// 
			// MOGUserManager
			// 
			this.Controls.Add(this.btnTestSMTP);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.tbEmailSMTP);
			this.Controls.Add(this.btnUserEdit);
			this.Controls.Add(this.btnRemoveUser);
			this.Controls.Add(this.lvUsers);
			this.Controls.Add(this.btnAddUser);
			this.Name = "MOGUserManager";
			this.Size = new System.Drawing.Size(624, 456);
			this.usersContextMenu.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion
		#endregion
		public int UserCount
		{
			get { return lvUsers.Items.Count; }
		}

		public int DepartmentCount
		{
			get { return 0; }
		}

		public MOGUserManager()
		{
			InitializeComponent();

			new ListViewSortManager(lvUsers, new Type[] {	typeof(ListViewTextCaseInsensitiveSort),
															typeof(ListViewTextCaseInsensitiveSort),
															typeof(ListViewTextCaseInsensitiveSort),
															typeof(ListViewTextCaseInsensitiveSort),
															typeof(ListViewTextCaseInsensitiveSort)});
		}

		public void AddUser(MOG_User user, string privileges)
		{
			if (FindUser(user.GetUserName()) == null)
			{
				if (mProject != null)
				{
					mProject.UserAdd(user);
				}

				mPrivileges.SetUserGroup(user.GetUserName(), privileges);
				lvUsers.Items.Add(CreateListViewItem(user, privileges));
			}
		}

		public void UpdateUser(string oldUsername, MOG_User user, string privileges)
		{
			ListViewItem item = FindUser(oldUsername);
			if (item != null)
			{
				if (mProject != null)
				{
					if (mProject.UserUpdate(oldUsername, user.GetUserName(), user.GetUserEmailAddress(), user.GetBlessTarget(), user.GetUserDepartment()))
					{
						mPrivileges.SetUserGroup(user.GetUserName(), privileges);

						//update the listview item with the new information
						item.Text = user.GetUserName();
						item.SubItems[1].Text = user.GetUserDepartment();
						item.SubItems[2].Text = user.GetUserEmailAddress();
						item.SubItems[3].Text = mPrivileges.GetUserGroup(user.GetUserName());
						item.SubItems[4].Text = user.GetBlessTarget();
					}
				}
			}
		}

		public void LoadFromProject(MOG_Project project)
		{
			mProject = project;

			lvUsers.Items.Clear();

			foreach (MOG_User user in project.GetUsers())
			{
				AddUser(user, mPrivileges.GetUserGroup(user.GetUserName()));
			}

			// Load up the SMTP server
			if (project.GetConfigFile().KeyExist("PROJECT", "EmailSmtp"))
			{
				tbEmailSMTP.Text = project.GetConfigFile().GetString("PROJECT", "EmailSmtp");
			}
		}

		public ListViewItem FindUser(string name)
		{
			foreach (ListViewItem item in lvUsers.Items)
			{
				if (String.Compare(item.Text, name, true) == 0)
				{
					return item;
				}
			}

			return null;
		}

		private MOG_User CreateUser(ListViewItem item)
		{
			MOG_User user = new MOG_User();
			user.SetUserName(item.Text);
			user.SetUserDepartment(item.SubItems[1].Text);
			user.SetUserEmailAddress(item.SubItems[2].Text);
			mPrivileges.SetUserGroup(user.GetUserName(), item.SubItems[3].Text);
			user.SetBlessTarget(item.SubItems[4].Text);

			return user;
		}

		private ListViewItem CreateListViewItem(MOG_User user, string privileges)
		{
			ListViewItem item = new ListViewItem();
			item.Text = user.GetUserName();
			item.SubItems.Add(user.GetUserDepartment());
			item.SubItems.Add(user.GetUserEmailAddress());
			item.SubItems.Add(privileges);
			item.SubItems.Add(user.GetBlessTarget());
			
			return item;
		}

		private void btnRemoveUser_Click(object sender, System.EventArgs e)
		{
			if (MOG_Prompt.PromptResponse("User Removal Confirmation", "Are you sure you want to remove the selected user(s)?", MOGPromptButtons.YesNo) == MOGPromptResult.Yes)
			{
				foreach (ListViewItem item in this.lvUsers.SelectedItems)
				{
					if (String.Compare(item.Text, "Admin", true) == 0)
					{
						MOG_Prompt.PromptMessage("User Removal Error", "You cannot remove the Admin user.");
					}
					else
					{
						if (mProject != null)
						{
							if (mProject.UserRemove(item.Text))
							{
								lvUsers.Items.Remove(item);
							}
						}
					}
				}
			}
		}

		private void btnAddUser_Click(object sender, EventArgs e)
		{
			MOG_Privileges privs = MOG_ControllerProject.GetPrivileges();
			if (privs != null)
			{
				if (!privs.GetUserPrivilege(MOG_ControllerProject.GetUserName_DefaultAdmin(), MOG_PRIVILEGE.AddNewUser))
				{
					MOG_Prompt.PromptResponse("Insufficient Privileges", "Your privileges do not allow you to add users.");
					return;
				}
			}

			EditUserForm form = new EditUserForm(this, mProject, mPrivileges, null);
			form.ShowDialog(this);
		}
		
		private void btnUserEdit_Click(object sender, System.EventArgs e)
		{
			if (this.lvUsers.SelectedItems.Count > 0)
			{
				EditUserForm form = new EditUserForm(this, mProject, mPrivileges, CreateUser(lvUsers.SelectedItems[0]));
				form.ShowDialog(this);
			}
		}

		private void lvUsers_DoubleClick(object sender, System.EventArgs e)
		{
			this.btnUserEdit.PerformClick();
		}

		private void btnTestSMTP_Click(object sender, System.EventArgs e)
		{
			if (tbEmailSMTP.Text == "")
			{
				MOG_Prompt.PromptResponse("Missing SMTP Server", "Please enter the name of your SMTP Server", MOGPromptButtons.OK);
				tbEmailSMTP.Focus();
			}
			else
			{
				if (lvUsers.SelectedItems.Count == 0)
				{
					MOG_Prompt.PromptResponse("No User Selected", "Please select a user in the list to send a test message to his/her email address.", MOGPromptButtons.OK);
				}
				else
				{
					MOG_User user = CreateUser(lvUsers.SelectedItems[0]);

					List<object> args = new List<object>();
					args.Add(user);
					args.Add(tbEmailSMTP.Text);

					ProgressDialog progress = new ProgressDialog("Testing SMTP Server", "Sending test message using your SMTP Server", TestSMTP_Worker, args, false);
					progress.ShowDialog(this);
				}
			}
		}

		void TestSMTP_Worker(object sender, DoWorkEventArgs e)
		{
			List<object> args = e.Argument as List<object>;
			MOG_User user = args[0] as MOG_User;
			string servername = args[1] as string;

			// Set up the body of the message
			string bodyMsg = "========================================================\r\n";
			bodyMsg += "------------------ MOG SMTP TEST -----------------------\r\n\n";
			bodyMsg += "========================================================\r\n";
			bodyMsg += " Date:" + DateTime.Now.ToShortDateString() + "\r\n";
			bodyMsg += " Time:" + DateTime.Now.ToLongTimeString() + "\r\n";
			bodyMsg += "========================================================\r\n\r\n";

			// Setup a test email message
			MailMessage message = new MailMessage(user.GetUserEmailAddress(), user.GetUserEmailAddress());
			message.Body = bodyMsg;
			message.Subject = "MOG SMTP Test";

			SmtpClient smtp = new SmtpClient(servername);
			
			try
			{
				smtp.Send(message);

				MOG_Prompt.PromptResponse("Success", "Test message successfully sent", MOGPromptButtons.OK);
				if (mProject != null)
				{
					mProject.GetConfigFile().PutString("PROJECT", "EmailSmtp", tbEmailSMTP.Text);
					mProject.GetConfigFile().Save();
				}
			}
			catch (Exception ex)
			{
				MOG_Prompt.PromptResponse("Test Message Failure", "Test message to " + message.To + " using SMTP server " + smtp.Host + " failed with the following error:\n\n" + ex.Message, "", MOGPromptButtons.OK, MOG.PROMPT.MOG_ALERT_LEVEL.ALERT);
			}
		}

		private void lvUsers_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			if (lvUsers.SelectedItems.Count > 0)
			{
				btnUserEdit.Enabled = true;
				btnRemoveUser.Enabled = true;
			}
			else
			{
				btnUserEdit.Enabled = false;
				btnRemoveUser.Enabled = false;
			}
		}

		private void usersContextMenu_Popup(object sender, CancelEventArgs e)
		{
			foreach (ToolStripItem item in usersContextMenu.Items)
			{
				item.Visible = true;
			}
			
			if (lvUsers.SelectedItems.Count == 0)
			{
				EditUserItem.Visible = false;
				RemoveUserItem.Visible = false;
			}
		}

		private void lvUsers_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)Keys.Return)
			{
				btnUserEdit.PerformClick();
			}
		}

		private void tbEmailSMTP_Validated(object sender, EventArgs e)
		{
			mProject.GetConfigFile().PutString("PROJECT", "EmailSmtp", tbEmailSMTP.Text);
			mProject.GetConfigFile().Save();
		}
	}
}
