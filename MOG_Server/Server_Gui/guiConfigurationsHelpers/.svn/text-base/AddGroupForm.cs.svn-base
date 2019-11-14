using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MOG_Server.Server_Gui.guiConfigurationsHelpers
{
	public class AddGroupForm : System.Windows.Forms.Form
	{
		#region System definitions
		
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.TextBox NewGroupTextBox;
		private System.Windows.Forms.Label KeyLabel;
		private System.Windows.Forms.Label PackageLabel;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

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
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.NewGroupTextBox = new System.Windows.Forms.TextBox();
			this.KeyLabel = new System.Windows.Forms.Label();
			this.PackageLabel = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(368, 72);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 9;
			this.btnCancel.Text = "Cancel";
			// 
			// btnOK
			// 
			this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.btnOK.Location = new System.Drawing.Point(456, 72);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 8;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// NewGroupTextBox
			// 
			this.NewGroupTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.NewGroupTextBox.Location = new System.Drawing.Point(376, 24);
			this.NewGroupTextBox.Name = "NewGroupTextBox";
			this.NewGroupTextBox.Size = new System.Drawing.Size(168, 20);
			this.NewGroupTextBox.TabIndex = 7;
			this.NewGroupTextBox.Text = "[New group name]";
			// 
			// KeyLabel
			// 
			this.KeyLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.KeyLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.KeyLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.KeyLabel.Location = new System.Drawing.Point(16, 24);
			this.KeyLabel.Name = "KeyLabel";
			this.KeyLabel.Size = new System.Drawing.Size(160, 24);
			this.KeyLabel.TabIndex = 5;
			this.KeyLabel.Text = "Key";
			this.KeyLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// PackageLabel
			// 
			this.PackageLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.PackageLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.PackageLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.PackageLabel.Location = new System.Drawing.Point(192, 24);
			this.PackageLabel.Name = "PackageLabel";
			this.PackageLabel.Size = new System.Drawing.Size(168, 24);
			this.PackageLabel.TabIndex = 10;
			this.PackageLabel.Text = "Package";
			this.PackageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label2.Location = new System.Drawing.Point(176, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(16, 32);
			this.label2.TabIndex = 18;
			this.label2.Text = ".";
			// 
			// label1
			// 
			this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.Location = new System.Drawing.Point(360, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(16, 32);
			this.label1.TabIndex = 19;
			this.label1.Text = ".";
			// 
			// AddGroupForm
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(560, 126);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.PackageLabel);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.NewGroupTextBox);
			this.Controls.Add(this.KeyLabel);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(568, 160);
			this.Name = "AddGroupForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Add Group";
			this.ResumeLayout(false);

		}
		#endregion
		#endregion

		#region User definitions
		public string NewGroupName { get{return this.NewGroupTextBox.Text;} }
		#endregion

		#region Constructors
		public AddGroupForm(string keyName, string packageName)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			
			this.KeyLabel.Text = keyName;
			this.PackageLabel.Text = packageName;
		}
		public AddGroupForm(string keyName, string packageName, string currentGroupName)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			
			// edit mode
			this.Text = "Edit Group";
			this.KeyLabel.Text = keyName;
			this.PackageLabel.Text = packageName;
			this.NewGroupTextBox.Text = currentGroupName;
			this.NewGroupTextBox.Focus();
		}
		#endregion

		#region Member functions
		public static string AddGroup(string keyName, string packageName) 
		{
			AddGroupForm agf = new AddGroupForm(keyName, packageName);
			if ( agf.ShowDialog() == DialogResult.OK ) 
			{
				return agf.NewGroupName;
			}
			
			return null;
		}
		public static string EditGroup(string keyName, string packageName, string currentGroupName) 
		{
			AddGroupForm agf = new AddGroupForm(keyName, packageName, currentGroupName);
			if ( agf.ShowDialog() == DialogResult.OK ) 
			{
				return agf.NewGroupName;
			}
			
			return null;
		}
		#endregion

		#region Events
		private void btnOK_Click(object sender, System.EventArgs e)
		{
			string replacedSpaces = this.NewGroupTextBox.Text.Replace(" ", "@");
			if ( this.NewGroupTextBox.Text != replacedSpaces ) 
			{
				MessageBox.Show("Spaces are not allowed in group names", "Invalid character");
				return;
			}

			this.DialogResult = DialogResult.OK;
			this.Close();
		}
		#endregion
	}
}
