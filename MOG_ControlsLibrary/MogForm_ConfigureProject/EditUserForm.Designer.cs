namespace MOG_ControlsLibrary
{
	partial class EditUserForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Username = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.Email = new System.Windows.Forms.TextBox();
			this.Privileges = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.Departments = new System.Windows.Forms.ComboBox();
			this.label12 = new System.Windows.Forms.Label();
			this.BlessTargets = new System.Windows.Forms.ComboBox();
			this.label17 = new System.Windows.Forms.Label();
			this.AddAndCloseButton = new System.Windows.Forms.Button();
			this.AddButton = new System.Windows.Forms.Button();
			this.CloseButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// Username
			// 
			this.Username.Location = new System.Drawing.Point(94, 12);
			this.Username.Name = "Username";
			this.Username.Size = new System.Drawing.Size(160, 20);
			this.Username.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(58, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Username:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 41);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(76, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Email Address:";
			// 
			// Email
			// 
			this.Email.Location = new System.Drawing.Point(94, 38);
			this.Email.Name = "Email";
			this.Email.Size = new System.Drawing.Size(160, 20);
			this.Email.TabIndex = 1;
			// 
			// Privileges
			// 
			this.Privileges.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.Privileges.Location = new System.Drawing.Point(346, 37);
			this.Privileges.Name = "Privileges";
			this.Privileges.Size = new System.Drawing.Size(160, 21);
			this.Privileges.TabIndex = 3;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(268, 40);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(64, 16);
			this.label4.TabIndex = 75;
			this.label4.Text = "Privileges:";
			// 
			// Departments
			// 
			this.Departments.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.Departments.Location = new System.Drawing.Point(346, 10);
			this.Departments.Name = "Departments";
			this.Departments.Size = new System.Drawing.Size(160, 21);
			this.Departments.TabIndex = 2;
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(268, 13);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(72, 16);
			this.label12.TabIndex = 72;
			this.label12.Text = "Department:";
			// 
			// BlessTargets
			// 
			this.BlessTargets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.BlessTargets.Location = new System.Drawing.Point(346, 64);
			this.BlessTargets.Name = "BlessTargets";
			this.BlessTargets.Size = new System.Drawing.Size(160, 21);
			this.BlessTargets.TabIndex = 4;
			// 
			// label17
			// 
			this.label17.Location = new System.Drawing.Point(268, 67);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(72, 16);
			this.label17.TabIndex = 73;
			this.label17.Text = "Bless Target:";
			// 
			// AddAndCloseButton
			// 
			this.AddAndCloseButton.Location = new System.Drawing.Point(325, 91);
			this.AddAndCloseButton.Name = "AddAndCloseButton";
			this.AddAndCloseButton.Size = new System.Drawing.Size(105, 23);
			this.AddAndCloseButton.TabIndex = 6;
			this.AddAndCloseButton.Text = "Add and Close";
			this.AddAndCloseButton.UseVisualStyleBackColor = true;
			this.AddAndCloseButton.Click += new System.EventHandler(this.AddAndCloseButton_Click);
			// 
			// AddButton
			// 
			this.AddButton.Location = new System.Drawing.Point(249, 91);
			this.AddButton.Name = "AddButton";
			this.AddButton.Size = new System.Drawing.Size(70, 23);
			this.AddButton.TabIndex = 5;
			this.AddButton.Text = "Add";
			this.AddButton.UseVisualStyleBackColor = true;
			this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
			// 
			// CloseButton
			// 
			this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CloseButton.Location = new System.Drawing.Point(436, 91);
			this.CloseButton.Name = "CloseButton";
			this.CloseButton.Size = new System.Drawing.Size(70, 23);
			this.CloseButton.TabIndex = 7;
			this.CloseButton.Text = "Close";
			this.CloseButton.UseVisualStyleBackColor = true;
			this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
			// 
			// EditUserForm
			// 
			this.AcceptButton = this.AddButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.CloseButton;
			this.ClientSize = new System.Drawing.Size(519, 122);
			this.Controls.Add(this.CloseButton);
			this.Controls.Add(this.AddButton);
			this.Controls.Add(this.AddAndCloseButton);
			this.Controls.Add(this.Privileges);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.Departments);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.BlessTargets);
			this.Controls.Add(this.label17);
			this.Controls.Add(this.Email);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.Username);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "EditUserForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Edit User";
			this.Load += new System.EventHandler(this.EditUserForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox Username;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox Email;
		private System.Windows.Forms.ComboBox Privileges;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox Departments;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.ComboBox BlessTargets;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.Button AddAndCloseButton;
		private System.Windows.Forms.Button AddButton;
		private System.Windows.Forms.Button CloseButton;
	}
}