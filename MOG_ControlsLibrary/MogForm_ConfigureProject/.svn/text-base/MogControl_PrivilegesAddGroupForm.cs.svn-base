using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MOG_ControlsLibrary.Common.MogControl_PrivilegesChangeForm
{
	/// <summary>
	/// Summary description for AddGroupForm.
	/// </summary>
	public class MogControl_PrivilegesAddGroupForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button OKButton;
		private System.Windows.Forms.Button CancelButton1;
		private System.Windows.Forms.Label label1;
		public System.Windows.Forms.TextBox GroupNameTextBox;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public MogControl_PrivilegesAddGroupForm()
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MogControl_PrivilegesAddGroupForm));
			this.OKButton = new System.Windows.Forms.Button();
			this.CancelButton1 = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.GroupNameTextBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// OKButton
			// 
			this.OKButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.OKButton.Location = new System.Drawing.Point(116, 64);
			this.OKButton.Name = "OKButton";
			this.OKButton.TabIndex = 1;
			this.OKButton.Text = "OK";
			this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
			// 
			// CancelButton1
			// 
			this.CancelButton1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.CancelButton1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelButton1.Location = new System.Drawing.Point(204, 64);
			this.CancelButton1.Name = "CancelButton1";
			this.CancelButton1.TabIndex = 2;
			this.CancelButton1.Text = "Cancel";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(128, 16);
			this.label1.TabIndex = 2;
			this.label1.Text = "Privilege Group Name:";
			// 
			// GroupNameTextBox
			// 
			this.GroupNameTextBox.Location = new System.Drawing.Point(8, 32);
			this.GroupNameTextBox.Name = "GroupNameTextBox";
			this.GroupNameTextBox.Size = new System.Drawing.Size(376, 20);
			this.GroupNameTextBox.TabIndex = 0;
			this.GroupNameTextBox.Text = "";
			// 
			// MogControl_PrivilegesAddGroupForm
			// 
			this.AcceptButton = this.OKButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.CancelButton1;
			this.ClientSize = new System.Drawing.Size(392, 101);
			this.Controls.Add(this.GroupNameTextBox);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.CancelButton1);
			this.Controls.Add(this.OKButton);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MogControl_PrivilegesAddGroupForm";
			this.ShowInTaskbar = false;
			this.Text = "Add a Privilege Group";
			this.ResumeLayout(false);

		}
		#endregion

		private void OKButton_Click(object sender, System.EventArgs e)
		{
			if( this.GroupNameTextBox.Text.Length < 1 )
			{
				MessageBox.Show( this, "Please enter a new Privilege Group name or click 'Cancel' "
					+ "to exit this dialog.", "Error!", MessageBoxButtons.OK, 
					MessageBoxIcon.Exclamation );
				return;
			}
			this.DialogResult = DialogResult.OK;
			this.Close();
		}
	}
}
