using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MOG_Client.Forms
{
	/// <summary>
	/// Summary description for EditCustomButtonForm.
	/// </summary>
	public class EditCustomButtonForm : System.Windows.Forms.Form
	{
		public System.Windows.Forms.TextBox EditButtonNameTextBox;
		public System.Windows.Forms.Button EditBrowseButton;
		private System.Windows.Forms.Button EditCancelButton;
		private System.Windows.Forms.Button EditOkButton;
		private System.Windows.Forms.OpenFileDialog EditOpenFileDialog;
		public System.Windows.Forms.TextBox EditToolExeTextBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public EditCustomButtonForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(EditCustomButtonForm));
			this.EditButtonNameTextBox = new System.Windows.Forms.TextBox();
			this.EditToolExeTextBox = new System.Windows.Forms.TextBox();
			this.EditBrowseButton = new System.Windows.Forms.Button();
			this.EditCancelButton = new System.Windows.Forms.Button();
			this.EditOkButton = new System.Windows.Forms.Button();
			this.EditOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// EditButtonNameTextBox
			// 
			this.EditButtonNameTextBox.Location = new System.Drawing.Point(8, 24);
			this.EditButtonNameTextBox.Name = "EditButtonNameTextBox";
			this.EditButtonNameTextBox.Size = new System.Drawing.Size(296, 20);
			this.EditButtonNameTextBox.TabIndex = 0;
			this.EditButtonNameTextBox.Text = "Button name";
			// 
			// EditToolExeTextBox
			// 
			this.EditToolExeTextBox.Location = new System.Drawing.Point(8, 64);
			this.EditToolExeTextBox.Name = "EditToolExeTextBox";
			this.EditToolExeTextBox.Size = new System.Drawing.Size(296, 20);
			this.EditToolExeTextBox.TabIndex = 1;
			this.EditToolExeTextBox.Text = "Tool executable";
			// 
			// EditBrowseButton
			// 
			this.EditBrowseButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.EditBrowseButton.Location = new System.Drawing.Point(312, 64);
			this.EditBrowseButton.Name = "EditBrowseButton";
			this.EditBrowseButton.Size = new System.Drawing.Size(24, 23);
			this.EditBrowseButton.TabIndex = 2;
			this.EditBrowseButton.Text = "...";
			this.EditBrowseButton.Click += new System.EventHandler(this.EditBrowseButton_Click);
			// 
			// EditCancelButton
			// 
			this.EditCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.EditCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.EditCancelButton.Location = new System.Drawing.Point(184, 96);
			this.EditCancelButton.Name = "EditCancelButton";
			this.EditCancelButton.TabIndex = 3;
			this.EditCancelButton.Text = "Cancel";
			// 
			// EditOkButton
			// 
			this.EditOkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.EditOkButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.EditOkButton.Location = new System.Drawing.Point(264, 96);
			this.EditOkButton.Name = "EditOkButton";
			this.EditOkButton.TabIndex = 4;
			this.EditOkButton.Text = "Ok";
			// 
			// EditOpenFileDialog
			// 
			this.EditOpenFileDialog.DefaultExt = "exe";
			this.EditOpenFileDialog.Filter = "Executables|*.exe|Batch Files|*.bat|All Files|*.*";
			this.EditOpenFileDialog.Title = "Browse for tool";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(144, 16);
			this.label1.TabIndex = 5;
			this.label1.Text = "Button Label";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(168, 16);
			this.label2.TabIndex = 6;
			this.label2.Text = "Tool Executable";
			// 
			// EditCustomButtonForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(346, 128);
			this.ControlBox = false;
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.EditOkButton);
			this.Controls.Add(this.EditCancelButton);
			this.Controls.Add(this.EditBrowseButton);
			this.Controls.Add(this.EditToolExeTextBox);
			this.Controls.Add(this.EditButtonNameTextBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "EditCustomButtonForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Edit Button";
			this.ResumeLayout(false);

		}
		#endregion

		private void EditBrowseButton_Click(object sender, System.EventArgs e)
		{
			if (EditOpenFileDialog.ShowDialog() == DialogResult.OK)
			{
				EditToolExeTextBox.Text = EditOpenFileDialog.FileName;
			}
		}
	}
}
