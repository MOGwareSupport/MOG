using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Iesi.Collections;

using MOG;
using MOG.INI;
using MOG.USER;
using MOG.PROJECT;

namespace MOG_Server.Server_Gui.guiConfigurationsHelpers
{
	/// <summary>
	/// Summary description for AddUserForm.
	/// </summary>
	public class AddUserForm : System.Windows.Forms.Form
	{
		#region User definitions
		public string UserName { get {return NameTextBox.Text;} }
		public string UserCategory { get {return ClassComboBox.Text.ToString();} }
		public string UserBlessTarget { get {return BlessTargetComboBox.Text.ToString();} }
		public string UserEmail { get {return EmailTextBox.Text;} }

		private BASE mog;
		#endregion
		#region System definitions
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.ComboBox BlessTargetComboBox;
		private System.Windows.Forms.TextBox NameTextBox;
		private System.Windows.Forms.ComboBox ClassComboBox;
		private System.Windows.Forms.Label NameLabel;
		private System.Windows.Forms.Label ClassLabel;
		private System.Windows.Forms.Label BlessTargetLabel;
		private System.Windows.Forms.Label EmailAddress;
		private System.Windows.Forms.TextBox EmailTextBox;
		private System.Windows.Forms.ToolTip AddUserFormToolTips;
		private System.ComponentModel.IContainer components;


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
		#endregion

		#region Constructors
		public AddUserForm(BASE mMog)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			
			this.Text = "Add User";
			this.mog=mMog;
			init();
		}
		public AddUserForm(BASE mMog, MOG_User user)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			
			this.Text = string.Concat("Edit User - ", user.GetUserName());
			this.mog=mMog;
			init();

			FillFields(user);
			this.NameTextBox.Enabled = false;
		}

		private void init() 
		{
			// initialize the combo boxes
			this.NameTextBox.Text = "";
			this.ClassComboBox.Text = "New class...";
			this.BlessTargetComboBox.Text = "MasterData";
			this.EmailTextBox.Text = "";


			// set up user categories in alphabetical order
			SortedSet categories = new SortedSet();		// use this to nicely sort our user class names

			foreach (MOG_UserCategory userCat in this.mog.GetProject().GetUserCategories())
			{
				categories.Add(userCat.mCategoryName);
			}

			foreach (string name in categories)
			{
				this.ClassComboBox.Items.Add(name);		// add user categories in alphabetical order
			}

			this.ClassComboBox.Items.Add("New class...");	// add new option always goes at the bottom
			

			// set up the bless target combo box
			//  it should have all the usernames and "MasterData"
			this.BlessTargetComboBox.Items.Add("MasterData");
			foreach (MOG_User userHandle in this.mog.GetProject().GetUsers()) 
			{
				this.BlessTargetComboBox.Items.Add(userHandle.GetUserName());
			}
		}
		#endregion

		#region Member functions
		public MOG_User UserInfo() 
		{
			MOG_User mu = new MOG_User();

			mu.SetUserName(this.UserName);
			mu.SetUserCategory(this.UserCategory);
			mu.SetBlessTarget(this.UserBlessTarget);
			mu.SetUserEmail(this.UserEmail);
			mu.SetUserPath( string.Concat(this.mog.GetProject().GetProjectUsersPath(), "\\", mu.GetUserName()) );
			mu.SetUserToolsPath( string.Concat(mu.GetUserPath(), "\\Tools") );

			return mu;
		}

		private void FillFields(MOG_User user) 
		{
			this.NameTextBox.Text = user.GetUserName();
			this.ClassComboBox.SelectedItem = user.GetUserCategory();
			this.BlessTargetComboBox.SelectedItem = user.GetBlessTarget();
			this.EmailTextBox.Text = user.GetUserEmail();
		}

		public static MOG_User EditUser(BASE mog, MOG_User user) 
		{
			AddUserForm auf = new AddUserForm(mog, user);
			if (auf.ShowDialog() == DialogResult.OK) 
			{
				return auf.UserInfo();
			}

			return null;
		}
		#endregion

		#region Events
		private void btnOK_Click(object sender, System.EventArgs e)
		{
			if (NameTextBox.Text == "") 
			{
				MessageBox.Show("Please enter a name for the new user", "Missing data");
				NameTextBox.Focus();
				return;
			}

			if (ClassComboBox.Text == "New class..."  ||  ClassComboBox.Text == "") 
			{
				MessageBox.Show("Please select a user category", "Missing data");
				ClassComboBox.Focus();
				return;
			}

			if (BlessTargetComboBox.Text == "") 
			{
				MessageBox.Show("Please select a bless target", "Missing data");
				BlessTargetComboBox.Focus();
				return;
			}
			
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void ClassComboBox_SelectionChangeCommitted(object sender, System.EventArgs e)
		{
			if (ClassComboBox.Text == "New class...") 
			{
				OneLineInputForm olif = new OneLineInputForm("Add new user category", "New category name", OneLineInputSize.Medium);
				string newClassName = OneLineInputForm.ShowMedium("Add new user category", "New category name");
				if (newClassName != null && newClassName != "") 
				{
					ClassComboBox.Items.Add( newClassName );
					ClassComboBox.SelectedItem = newClassName;

					MOG_UserCategory muc = new MOG_UserCategory();
					muc.mCategoryName = newClassName;
					this.mog.GetProject().UserCategoryAdd(muc);
				}
			}
		}
		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.btnCancel = new System.Windows.Forms.Button();
			this.BlessTargetComboBox = new System.Windows.Forms.ComboBox();
			this.NameTextBox = new System.Windows.Forms.TextBox();
			this.ClassComboBox = new System.Windows.Forms.ComboBox();
			this.NameLabel = new System.Windows.Forms.Label();
			this.ClassLabel = new System.Windows.Forms.Label();
			this.BlessTargetLabel = new System.Windows.Forms.Label();
			this.btnOK = new System.Windows.Forms.Button();
			this.EmailAddress = new System.Windows.Forms.Label();
			this.EmailTextBox = new System.Windows.Forms.TextBox();
			this.AddUserFormToolTips = new System.Windows.Forms.ToolTip(this.components);
			this.SuspendLayout();
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnCancel.Location = new System.Drawing.Point(136, 136);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 5;
			this.btnCancel.Text = "Cancel";
			this.AddUserFormToolTips.SetToolTip(this.btnCancel, "Cancel the creation of this user");
			// 
			// BlessTargetComboBox
			// 
			this.BlessTargetComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.BlessTargetComboBox.Location = new System.Drawing.Point(120, 72);
			this.BlessTargetComboBox.Name = "BlessTargetComboBox";
			this.BlessTargetComboBox.Size = new System.Drawing.Size(176, 21);
			this.BlessTargetComboBox.TabIndex = 2;
			this.BlessTargetComboBox.Text = "MasterData";
			this.AddUserFormToolTips.SetToolTip(this.BlessTargetComboBox, "The machine (i.e., the archive) that this user will bless to");
			// 
			// NameTextBox
			// 
			this.NameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.NameTextBox.Location = new System.Drawing.Point(120, 24);
			this.NameTextBox.Name = "NameTextBox";
			this.NameTextBox.Size = new System.Drawing.Size(176, 20);
			this.NameTextBox.TabIndex = 0;
			this.NameTextBox.Text = "Name";
			this.AddUserFormToolTips.SetToolTip(this.NameTextBox, "The name the user will be known by");
			// 
			// ClassComboBox
			// 
			this.ClassComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.ClassComboBox.Location = new System.Drawing.Point(120, 48);
			this.ClassComboBox.Name = "ClassComboBox";
			this.ClassComboBox.Size = new System.Drawing.Size(176, 21);
			this.ClassComboBox.TabIndex = 1;
			this.ClassComboBox.Text = "Unknown";
			this.AddUserFormToolTips.SetToolTip(this.ClassComboBox, "The category the user will be (i.e., programmer, artist)");
			this.ClassComboBox.SelectionChangeCommitted += new System.EventHandler(this.ClassComboBox_SelectionChangeCommitted);
			// 
			// NameLabel
			// 
			this.NameLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.NameLabel.Location = new System.Drawing.Point(48, 24);
			this.NameLabel.Name = "NameLabel";
			this.NameLabel.Size = new System.Drawing.Size(64, 16);
			this.NameLabel.TabIndex = 10;
			this.NameLabel.Text = "User Name";
			this.NameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// ClassLabel
			// 
			this.ClassLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.ClassLabel.Location = new System.Drawing.Point(32, 48);
			this.ClassLabel.Name = "ClassLabel";
			this.ClassLabel.Size = new System.Drawing.Size(80, 16);
			this.ClassLabel.TabIndex = 11;
			this.ClassLabel.Text = "User Category";
			this.ClassLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// BlessTargetLabel
			// 
			this.BlessTargetLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.BlessTargetLabel.Location = new System.Drawing.Point(16, 72);
			this.BlessTargetLabel.Name = "BlessTargetLabel";
			this.BlessTargetLabel.Size = new System.Drawing.Size(96, 16);
			this.BlessTargetLabel.TabIndex = 12;
			this.BlessTargetLabel.Text = "User Bless Target";
			this.BlessTargetLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// btnOK
			// 
			this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnOK.Location = new System.Drawing.Point(224, 136);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 4;
			this.btnOK.Text = "OK";
			this.AddUserFormToolTips.SetToolTip(this.btnOK, "Accept the specified options and create this user");
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// EmailAddress
			// 
			this.EmailAddress.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.EmailAddress.Location = new System.Drawing.Point(8, 96);
			this.EmailAddress.Name = "EmailAddress";
			this.EmailAddress.Size = new System.Drawing.Size(104, 16);
			this.EmailAddress.TabIndex = 13;
			this.EmailAddress.Text = "User email address";
			this.EmailAddress.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// EmailTextBox
			// 
			this.EmailTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.EmailTextBox.Location = new System.Drawing.Point(120, 96);
			this.EmailTextBox.Name = "EmailTextBox";
			this.EmailTextBox.Size = new System.Drawing.Size(176, 20);
			this.EmailTextBox.TabIndex = 3;
			this.EmailTextBox.Text = "email address";
			this.AddUserFormToolTips.SetToolTip(this.EmailTextBox, "The user\'s email address");
			// 
			// AddUserForm
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(328, 190);
			this.Controls.Add(this.EmailTextBox);
			this.Controls.Add(this.NameTextBox);
			this.Controls.Add(this.EmailAddress);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.BlessTargetLabel);
			this.Controls.Add(this.ClassLabel);
			this.Controls.Add(this.NameLabel);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.BlessTargetComboBox);
			this.Controls.Add(this.ClassComboBox);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(336, 224);
			this.Name = "AddUserForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Add User";
			this.ResumeLayout(false);

		}
		#endregion

	}
}
