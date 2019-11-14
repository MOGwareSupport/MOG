using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using MOG;
using MOG.USER;
using MOG.PROJECT;
using MOG.CONTROLLER.CONTROLLERPROJECT;

using Iesi.Collections;



namespace MOG_Server
{
	/// <summary>
	/// Summary description for UserForm.
	/// </summary>
	public class UserForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ComboBox UserClassComboBox;
		private System.Windows.Forms.TextBox UserNameTextBox;
		private System.Windows.Forms.ComboBox UserBlessTargetComboBox;
		private System.Windows.Forms.Button UserCancelButton;
		private System.Windows.Forms.Button UserOkButton;

		private MOG_User mUser;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public UserForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(UserForm));
			this.UserClassComboBox = new System.Windows.Forms.ComboBox();
			this.UserNameTextBox = new System.Windows.Forms.TextBox();
			this.UserBlessTargetComboBox = new System.Windows.Forms.ComboBox();
			this.UserCancelButton = new System.Windows.Forms.Button();
			this.UserOkButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// UserClassComboBox
			// 
			this.UserClassComboBox.Location = new System.Drawing.Point(8, 56);
			this.UserClassComboBox.Name = "UserClassComboBox";
			this.UserClassComboBox.Size = new System.Drawing.Size(160, 21);
			this.UserClassComboBox.TabIndex = 0;
			this.UserClassComboBox.Text = "Unknown";
			// 
			// UserNameTextBox
			// 
			this.UserNameTextBox.Location = new System.Drawing.Point(8, 32);
			this.UserNameTextBox.Name = "UserNameTextBox";
			this.UserNameTextBox.Size = new System.Drawing.Size(160, 20);
			this.UserNameTextBox.TabIndex = 1;
			this.UserNameTextBox.Text = "Name";
			// 
			// UserBlessTargetComboBox
			// 
			this.UserBlessTargetComboBox.Location = new System.Drawing.Point(8, 80);
			this.UserBlessTargetComboBox.Name = "UserBlessTargetComboBox";
			this.UserBlessTargetComboBox.Size = new System.Drawing.Size(160, 21);
			this.UserBlessTargetComboBox.TabIndex = 2;
			this.UserBlessTargetComboBox.Text = "MasterData";
			// 
			// UserCancelButton
			// 
			this.UserCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.UserCancelButton.Location = new System.Drawing.Point(16, 112);
			this.UserCancelButton.Name = "UserCancelButton";
			this.UserCancelButton.TabIndex = 3;
			this.UserCancelButton.Text = "Cancel";
			this.UserCancelButton.Click += new System.EventHandler(this.UserCancelButton_Click);
			// 
			// UserOkButton
			// 
			this.UserOkButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.UserOkButton.Location = new System.Drawing.Point(93, 112);
			this.UserOkButton.Name = "UserOkButton";
			this.UserOkButton.TabIndex = 4;
			this.UserOkButton.Text = "Ok";
			this.UserOkButton.Click += new System.EventHandler(this.UserOkButton_Click);
			// 
			// UserForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(176, 142);
			this.Controls.Add(this.UserOkButton);
			this.Controls.Add(this.UserCancelButton);
			this.Controls.Add(this.UserBlessTargetComboBox);
			this.Controls.Add(this.UserNameTextBox);
			this.Controls.Add(this.UserClassComboBox);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "UserForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "UserForm";
			this.ResumeLayout(false);

		}
		#endregion

		public void UserPopulate(MOG_User user)
		{
			mUser = user;

			this.UserNameTextBox.Text = user.GetUserName();
			this.UserClassComboBox.Text = user.GetUserCategory();
			this.UserBlessTargetComboBox.Text = user.GetBlessTarget();

			SortedSet categories = new SortedSet();

			foreach (MOG_User userHandle in MOG_ControllerProject.GetProject().GetUsers())
			{
//				categories.Add(userHandle.GetUserCategory());
				this.UserBlessTargetComboBox.Items.Add(userHandle.GetUserName());
			}
			
			foreach (MOG_UserCategory userCat in MOG_ControllerProject.GetProject().GetUserCategories())
			{
				categories.Add(userCat.mCategoryName);
			}

			this.UserBlessTargetComboBox.Items.Add("MasterData");

			foreach (string name in categories)
			{
				this.UserClassComboBox.Items.Add(name);
			}
		}

		public void UserUpdate()
		{
			mUser.SetUserName(this.UserNameTextBox.Text);
			mUser.SetUserCategory(this.UserClassComboBox.Text);
			mUser.SetBlessTarget(this.UserBlessTargetComboBox.Text);

			MOG_ControllerProject.GetProject().UserUpdate(	mUser.GetUserName(),
															mUser.GetUserName(),
															mUser.GetUserEmail(),
															mUser.GetBlessTarget(),
															mUser.GetUserCategory() );
		}

		private void UserOkButton_Click(object sender, System.EventArgs e)
		{
			UserUpdate();

			MOG_ControllerProject.GetProject().Load();
			this.Close();
		}

		private void UserCancelButton_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}
	}
}
