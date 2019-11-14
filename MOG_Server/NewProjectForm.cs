using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MOG_Server
{
	/// <summary>
	/// Summary description for NewProjectForm.
	/// </summary>
	public class NewProjectForm : System.Windows.Forms.Form
	{
		public System.Windows.Forms.TextBox NewProjectNameTextBox;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public NewProjectForm()
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(NewProjectForm));
			this.NewProjectNameTextBox = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// NewProjectNameTextBox
			// 
			this.NewProjectNameTextBox.Location = new System.Drawing.Point(0, 8);
			this.NewProjectNameTextBox.Name = "NewProjectNameTextBox";
			this.NewProjectNameTextBox.Size = new System.Drawing.Size(288, 20);
			this.NewProjectNameTextBox.TabIndex = 0;
			this.NewProjectNameTextBox.Text = "Project Name";
			// 
			// button1
			// 
			this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.button1.Location = new System.Drawing.Point(131, 40);
			this.button1.Name = "button1";
			this.button1.TabIndex = 1;
			this.button1.Text = "Cancel";
			// 
			// button2
			// 
			this.button2.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.button2.Location = new System.Drawing.Point(208, 40);
			this.button2.Name = "button2";
			this.button2.TabIndex = 2;
			this.button2.Text = "Ok";
			// 
			// NewProjectForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(288, 70);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.NewProjectNameTextBox);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "NewProjectForm";
			this.Text = "New Project";
			this.ResumeLayout(false);

		}
		#endregion
	}
}
