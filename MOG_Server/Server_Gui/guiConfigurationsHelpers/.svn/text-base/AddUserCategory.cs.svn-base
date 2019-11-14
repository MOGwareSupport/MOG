using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MOG_Server.Server_Gui.guiConfigurationsHelpers
{
	/// <summary>
	/// Summary description for AddUserCategory.
	/// </summary>
	public class AddUserCategoryForm : System.Windows.Forms.Form
	{
		#region User definitions
		public string NewCategoryName { get{return nameTxtBox.Text;} }
		#endregion
		#region System definitions
		private System.Windows.Forms.TextBox nameTxtBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;

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
		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.nameTxtBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// nameTxtBox
			// 
			this.nameTxtBox.Location = new System.Drawing.Point(128, 24);
			this.nameTxtBox.Name = "nameTxtBox";
			this.nameTxtBox.Size = new System.Drawing.Size(376, 20);
			this.nameTxtBox.TabIndex = 0;
			this.nameTxtBox.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(112, 16);
			this.label1.TabIndex = 1;
			this.label1.Text = "New category name";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(424, 64);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(336, 64);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "Cancel";
			// 
			// AddUserCategoryForm
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(528, 102);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.nameTxtBox);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AddUserCategoryForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "New User Category";
			this.ResumeLayout(false);

		}
		#endregion

		#region Constructor
		public AddUserCategoryForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}
		#endregion
		
		#region Events
		private void btnOK_Click(object sender, System.EventArgs e)
		{
			if (nameTxtBox.Text=="") 
			{
				// new category name is invalid
				MessageBox.Show("Invalid category name", "Invalid data");
				return;
			}

			this.DialogResult = DialogResult.OK;
			this.Close();
		}
		#endregion
	}
}
