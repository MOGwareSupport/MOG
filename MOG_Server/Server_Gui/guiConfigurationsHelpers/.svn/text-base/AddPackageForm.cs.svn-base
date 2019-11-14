
/*
#region System definitions
#endregion
#region User definitions
#endregion
#region Constructor
#endregion
#region Member functions
#endregion
#region Events
#endregion
*/

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MOG_Server.Server_Gui.guiConfigurationsHelpers
{
	public class AddPackageForm : System.Windows.Forms.Form
	{
		#region System definitions
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.TextBox NewPackageTextBox;
		private System.Windows.Forms.Label GroupsLabel;
		private System.Windows.Forms.Label KeyLabel;
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
			this.NewPackageTextBox = new System.Windows.Forms.TextBox();
			this.GroupsLabel = new System.Windows.Forms.Label();
			this.KeyLabel = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnCancel.Location = new System.Drawing.Point(368, 72);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 9;
			this.btnCancel.Text = "Cancel";
			// 
			// btnOK
			// 
			this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnOK.Location = new System.Drawing.Point(456, 72);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 8;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// NewPackageTextBox
			// 
			this.NewPackageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.NewPackageTextBox.Location = new System.Drawing.Point(200, 24);
			this.NewPackageTextBox.Name = "NewPackageTextBox";
			this.NewPackageTextBox.Size = new System.Drawing.Size(256, 20);
			this.NewPackageTextBox.TabIndex = 7;
			this.NewPackageTextBox.Text = "[New package name]";
			// 
			// GroupsLabel
			// 
			this.GroupsLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.GroupsLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.GroupsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.GroupsLabel.Location = new System.Drawing.Point(472, 24);
			this.GroupsLabel.Name = "GroupsLabel";
			this.GroupsLabel.Size = new System.Drawing.Size(64, 24);
			this.GroupsLabel.TabIndex = 6;
			this.GroupsLabel.Text = "[Groups]";
			this.GroupsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// KeyLabel
			// 
			this.KeyLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.KeyLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.KeyLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.KeyLabel.Location = new System.Drawing.Point(24, 24);
			this.KeyLabel.Name = "KeyLabel";
			this.KeyLabel.Size = new System.Drawing.Size(160, 24);
			this.KeyLabel.TabIndex = 5;
			this.KeyLabel.Text = "Key";
			this.KeyLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label2.Location = new System.Drawing.Point(184, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(16, 32);
			this.label2.TabIndex = 17;
			this.label2.Text = ".";
			// 
			// label1
			// 
			this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.Location = new System.Drawing.Point(456, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(16, 32);
			this.label1.TabIndex = 18;
			this.label1.Text = ".";
			// 
			// AddPackageForm
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(560, 126);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.NewPackageTextBox);
			this.Controls.Add(this.GroupsLabel);
			this.Controls.Add(this.KeyLabel);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(568, 160);
			this.Name = "AddPackageForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Add Package";
			this.ResumeLayout(false);

		}
		#endregion
		#endregion

		#region User definitions
		public string NewPackageName { get{return this.NewPackageTextBox.Text;} }
		#endregion

		#region Constructors
		public AddPackageForm(string keyName)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			
			this.KeyLabel.Text = keyName;
		}
		public AddPackageForm(string keyName, string currentPackageName)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			
			// edit mode
			this.Text = "Edit Package";
			this.KeyLabel.Text = keyName;
			this.NewPackageTextBox.Text = currentPackageName;
			this.NewPackageTextBox.Focus();
		}
		#endregion

		#region Member functions
		public static string AddPackage(string keyName) 
		{
			AddPackageForm apf = new AddPackageForm( keyName );
			if ( apf.ShowDialog() == DialogResult.OK ) 
			{
				return apf.NewPackageName;
			}
			
			return null;
		}

		public static string EditPackage(string keyName, string currentPackageName) 
		{
			AddPackageForm apf = new AddPackageForm( keyName, currentPackageName );
			if ( apf.ShowDialog() == DialogResult.OK ) 
			{
				return apf.NewPackageName;
			}
			
			return null;
		}
		#endregion

		#region Events
		private void btnOK_Click(object sender, System.EventArgs e)
		{
			string replacedSpaces = this.NewPackageTextBox.Text.Replace(" ", "@");
			if ( this.NewPackageTextBox.Text != replacedSpaces ) 
			{
				MessageBox.Show("Spaces are not allowed in package names", "Invalid character");
				return;
			}

			this.DialogResult = DialogResult.OK;
			this.Close();
		}
		#endregion
	}
}
