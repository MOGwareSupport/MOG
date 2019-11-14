using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using MOG_ControlsLibrary.Controls;

namespace MOG_Client.Forms
{
	/// <summary>
	/// Summary description for CreateNewPackageForm.
	/// </summary>
	public class CreateNewPackageForm : System.Windows.Forms.Form
	{
		private MogControl_PackageManagementTreeView mogControl_PackageManagementTreeView1;
		private System.Windows.Forms.Button NewPackageOKButton;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public CreateNewPackageForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//mogControl_PackageManagementTreeView1.InitializePackageList();
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(CreateNewPackageForm));
			this.mogControl_PackageManagementTreeView1 = new MogControl_PackageManagementTreeView();
			this.NewPackageOKButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// mogControl_PackageManagementTreeView1
			// 
			this.mogControl_PackageManagementTreeView1.Location = new System.Drawing.Point(0, 0);
			this.mogControl_PackageManagementTreeView1.Name = "mogControl_PackageManagementTreeView1";
			this.mogControl_PackageManagementTreeView1.Size = new System.Drawing.Size(296, 248);
			this.mogControl_PackageManagementTreeView1.TabIndex = 0;
			// 
			// NewPackageOKButton
			// 
			this.NewPackageOKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.NewPackageOKButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.NewPackageOKButton.Location = new System.Drawing.Point(216, 248);
			this.NewPackageOKButton.Name = "NewPackageOKButton";
			this.NewPackageOKButton.TabIndex = 2;
			this.NewPackageOKButton.Text = "Close";
			// 
			// CreateNewPackageForm
			// 
			this.AcceptButton = this.NewPackageOKButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Controls.Add(this.NewPackageOKButton);
			this.Controls.Add(this.mogControl_PackageManagementTreeView1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "CreateNewPackageForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "New Package";
			this.ResumeLayout(false);

		}
		#endregion
	}
}
