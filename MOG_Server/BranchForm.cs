using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MOG_Server
{
	/// <summary>
	/// Summary description for BranchForm.
	/// </summary>
	public class BranchForm : System.Windows.Forms.Form
	{
		public System.Windows.Forms.TextBox BranchProjectNameTextBox;
		public System.Windows.Forms.TextBox BranchBranchNameTextBox;
		private System.Windows.Forms.Button BranchOkButton;
		private System.Windows.Forms.Button BranchCancelButton;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public BranchForm()
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(BranchForm));
			this.BranchProjectNameTextBox = new System.Windows.Forms.TextBox();
			this.BranchBranchNameTextBox = new System.Windows.Forms.TextBox();
			this.BranchOkButton = new System.Windows.Forms.Button();
			this.BranchCancelButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// BranchProjectNameTextBox
			// 
			this.BranchProjectNameTextBox.Enabled = false;
			this.BranchProjectNameTextBox.Location = new System.Drawing.Point(8, 16);
			this.BranchProjectNameTextBox.Name = "BranchProjectNameTextBox";
			this.BranchProjectNameTextBox.Size = new System.Drawing.Size(104, 20);
			this.BranchProjectNameTextBox.TabIndex = 0;
			this.BranchProjectNameTextBox.Text = "ProjectName";
			// 
			// BranchBranchNameTextBox
			// 
			this.BranchBranchNameTextBox.Location = new System.Drawing.Point(120, 16);
			this.BranchBranchNameTextBox.Name = "BranchBranchNameTextBox";
			this.BranchBranchNameTextBox.Size = new System.Drawing.Size(144, 20);
			this.BranchBranchNameTextBox.TabIndex = 1;
			this.BranchBranchNameTextBox.Text = "BranchName";
			// 
			// BranchOkButton
			// 
			this.BranchOkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.BranchOkButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.BranchOkButton.Location = new System.Drawing.Point(184, 48);
			this.BranchOkButton.Name = "BranchOkButton";
			this.BranchOkButton.TabIndex = 2;
			this.BranchOkButton.Text = "Ok";
			// 
			// BranchCancelButton
			// 
			this.BranchCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.BranchCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.BranchCancelButton.Location = new System.Drawing.Point(104, 48);
			this.BranchCancelButton.Name = "BranchCancelButton";
			this.BranchCancelButton.TabIndex = 3;
			this.BranchCancelButton.Text = "Cancel";
			// 
			// BranchForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(272, 86);
			this.Controls.Add(this.BranchCancelButton);
			this.Controls.Add(this.BranchOkButton);
			this.Controls.Add(this.BranchBranchNameTextBox);
			this.Controls.Add(this.BranchProjectNameTextBox);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "BranchForm";
			this.Text = "New Project Branch";
			this.ResumeLayout(false);

		}
		#endregion
	}
}
