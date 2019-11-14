namespace MOG_ControlsLibrary
{
	partial class AddDepartmentForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.DepartmentName = new System.Windows.Forms.TextBox();
			this.AddButton = new System.Windows.Forms.Button();
			this.CloseButton = new System.Windows.Forms.Button();
			this.AddAndCloseButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(65, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Department:";
			// 
			// DepartmentName
			// 
			this.DepartmentName.Location = new System.Drawing.Point(95, 12);
			this.DepartmentName.Name = "DepartmentName";
			this.DepartmentName.Size = new System.Drawing.Size(226, 20);
			this.DepartmentName.TabIndex = 0;
			// 
			// AddButton
			// 
			this.AddButton.Location = new System.Drawing.Point(109, 45);
			this.AddButton.Name = "AddButton";
			this.AddButton.Size = new System.Drawing.Size(53, 23);
			this.AddButton.TabIndex = 1;
			this.AddButton.Text = "Add";
			this.AddButton.UseVisualStyleBackColor = true;
			this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
			// 
			// CloseButton
			// 
			this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CloseButton.Location = new System.Drawing.Point(260, 45);
			this.CloseButton.Name = "CloseButton";
			this.CloseButton.Size = new System.Drawing.Size(61, 23);
			this.CloseButton.TabIndex = 3;
			this.CloseButton.Text = "Close";
			this.CloseButton.UseVisualStyleBackColor = true;
			this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
			// 
			// AddAndCloseButton
			// 
			this.AddAndCloseButton.Location = new System.Drawing.Point(168, 45);
			this.AddAndCloseButton.Name = "AddAndCloseButton";
			this.AddAndCloseButton.Size = new System.Drawing.Size(86, 23);
			this.AddAndCloseButton.TabIndex = 2;
			this.AddAndCloseButton.Text = "Add and Close";
			this.AddAndCloseButton.UseVisualStyleBackColor = true;
			this.AddAndCloseButton.Click += new System.EventHandler(this.AddAndCloseButton_Click);
			// 
			// AddDepartmentForm
			// 
			this.AcceptButton = this.AddAndCloseButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.CloseButton;
			this.ClientSize = new System.Drawing.Size(333, 80);
			this.Controls.Add(this.AddAndCloseButton);
			this.Controls.Add(this.CloseButton);
			this.Controls.Add(this.AddButton);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.DepartmentName);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "AddDepartmentForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Add Department";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox DepartmentName;
		private System.Windows.Forms.Button AddButton;
		private System.Windows.Forms.Button CloseButton;
		private System.Windows.Forms.Button AddAndCloseButton;
	}
}