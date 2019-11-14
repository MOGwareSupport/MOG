using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MOG_ControlsLibrary
{
	/// <summary>
	/// Summary description for CreateBranchForm.
	/// </summary>
	public class CreateBranchForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button BranchCreateButton;
		private System.Windows.Forms.Button BranchCancelButton;
		private System.Windows.Forms.Label label1;
		public System.Windows.Forms.TextBox BranchNameTextBox;
		private System.Windows.Forms.Label label2;
		public System.Windows.Forms.TextBox BranchSourceTextBox;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public CreateBranchForm()
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(CreateBranchForm));
			this.BranchCreateButton = new System.Windows.Forms.Button();
			this.BranchCancelButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.BranchNameTextBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.BranchSourceTextBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// BranchCreateButton
			// 
			this.BranchCreateButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.BranchCreateButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.BranchCreateButton.Location = new System.Drawing.Point(128, 88);
			this.BranchCreateButton.Name = "BranchCreateButton";
			this.BranchCreateButton.TabIndex = 1;
			this.BranchCreateButton.Text = "Create";
			// 
			// BranchCancelButton
			// 
			this.BranchCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.BranchCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.BranchCancelButton.Location = new System.Drawing.Point(208, 88);
			this.BranchCancelButton.Name = "BranchCancelButton";
			this.BranchCancelButton.TabIndex = 2;
			this.BranchCancelButton.Text = "Cancel";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 40);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(264, 16);
			this.label1.TabIndex = 2;
			this.label1.Text = "Project Branch Name:";
			// 
			// BranchNameTextBox
			// 
			this.BranchNameTextBox.Location = new System.Drawing.Point(24, 56);
			this.BranchNameTextBox.Name = "BranchNameTextBox";
			this.BranchNameTextBox.Size = new System.Drawing.Size(256, 20);
			this.BranchNameTextBox.TabIndex = 0;
			this.BranchNameTextBox.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 16);
			this.label2.TabIndex = 4;
			this.label2.Text = "Branching from:";
			// 
			// BranchSourceTextBox
			// 
			this.BranchSourceTextBox.Location = new System.Drawing.Point(24, 16);
			this.BranchSourceTextBox.Name = "BranchSourceTextBox";
			this.BranchSourceTextBox.ReadOnly = true;
			this.BranchSourceTextBox.Size = new System.Drawing.Size(256, 20);
			this.BranchSourceTextBox.TabIndex = 5;
			this.BranchSourceTextBox.Text = "";
			// 
			// CreateBranchForm
			// 
			this.AcceptButton = this.BranchCreateButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.BranchCancelButton;
			this.ClientSize = new System.Drawing.Size(292, 114);
			this.Controls.Add(this.BranchSourceTextBox);
			this.Controls.Add(this.BranchNameTextBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.BranchCancelButton);
			this.Controls.Add(this.BranchCreateButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "CreateBranchForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Create new project branch";
			this.ResumeLayout(false);

		}
		#endregion
	}
}
