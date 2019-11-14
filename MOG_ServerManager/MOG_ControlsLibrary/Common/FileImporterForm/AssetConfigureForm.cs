using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using MOG.PROJECT;

namespace MOG_ServerManager
{
	/// <summary>
	/// Summary description for AssetConfigureForm.
	/// </summary>
	public class AssetConfigureForm : System.Windows.Forms.Form
	{
		#region System defs

		private AssetClassificationConfigControl assetClassificationConfigControl;
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(AssetConfigureForm));
			this.assetClassificationConfigControl = new AssetClassificationConfigControl();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// assetClassificationConfigControl
			// 
			this.assetClassificationConfigControl.AddClassificationMenuItemVisible = true;
			this.assetClassificationConfigControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.assetClassificationConfigControl.ConfigurationVisible = true;
			this.assetClassificationConfigControl.ImmediateMode = false;
			this.assetClassificationConfigControl.Location = new System.Drawing.Point(8, 8);
			this.assetClassificationConfigControl.Name = "assetClassificationConfigControl";
			this.assetClassificationConfigControl.ProjectRootPath = "";
			this.assetClassificationConfigControl.RemoveClassificationMenuItemVisible = true;
			this.assetClassificationConfigControl.ShowConfigurationMenuItemVisible = true;
			this.assetClassificationConfigControl.Size = new System.Drawing.Size(936, 528);
			this.assetClassificationConfigControl.TabIndex = 0;
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.Location = new System.Drawing.Point(848, 544);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.Location = new System.Drawing.Point(768, 544);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// AssetConfigureForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(952, 582);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.assetClassificationConfigControl);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "AssetConfigureForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "AssetConfigureForm";
			this.ResumeLayout(false);

		}
		#endregion
		#endregion
		#region Member vars
		private MOG_Project project = null;
		#endregion
		#region Properties
		public string ProjectRootPath
		{
			get { return this.assetClassificationConfigControl.ProjectRootPath; }
			set { this.assetClassificationConfigControl.ProjectRootPath = value; }
		}
		#endregion
		#region Constructor
		public AssetConfigureForm(MOG_Project proj)
		{
			InitializeComponent();

			this.project = proj;
			this.project.SetProjectName( this.project.GetProjectName() );
		}

		#endregion
		#region Public functions
		public void LoadAssetTree( TreeNodeCollection nodes )
		{
			this.assetClassificationConfigControl.LoadAssetTree(nodes);
		}
		#endregion
		#region Event handlers
		private void btnOK_Click(object sender, System.EventArgs e)
		{
			this.assetClassificationConfigControl.EncodeAll();

			Hide();
			// write imported assets to INIs/DB
			this.assetClassificationConfigControl.CreateAssetConfigs();

			this.DialogResult = DialogResult.OK;
			Dispose();
		}
		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			Dispose();
		}
		#endregion
	}
}





