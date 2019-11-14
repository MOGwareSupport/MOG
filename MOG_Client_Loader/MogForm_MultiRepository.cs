using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MOG_Client_Loader
{
	/// <summary>
	/// Summary description for MogForm_MultiRepository.
	/// </summary>
	public class MogForm_MultiRepository : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button RepCancelButton;
		private System.Windows.Forms.Button RepOKButton;
		public System.Windows.Forms.ComboBox RepositoryComboBox;
		private System.Windows.Forms.Label label1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public MogForm_MultiRepository()
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MogForm_MultiRepository));
			this.RepCancelButton = new System.Windows.Forms.Button();
			this.RepOKButton = new System.Windows.Forms.Button();
			this.RepositoryComboBox = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// RepCancelButton
			// 
			this.RepCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.RepCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.RepCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.RepCancelButton.Location = new System.Drawing.Point(132, 52);
			this.RepCancelButton.Name = "RepCancelButton";
			this.RepCancelButton.TabIndex = 0;
			this.RepCancelButton.Text = "Cancel";
			// 
			// RepOKButton
			// 
			this.RepOKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.RepOKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.RepOKButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.RepOKButton.Location = new System.Drawing.Point(212, 52);
			this.RepOKButton.Name = "RepOKButton";
			this.RepOKButton.TabIndex = 1;
			this.RepOKButton.Text = "OK";
			// 
			// RepositoryComboBox
			// 
			this.RepositoryComboBox.Location = new System.Drawing.Point(16, 24);
			this.RepositoryComboBox.Name = "RepositoryComboBox";
			this.RepositoryComboBox.Size = new System.Drawing.Size(272, 21);
			this.RepositoryComboBox.TabIndex = 2;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(272, 16);
			this.label1.TabIndex = 3;
			this.label1.Text = "Select desired repository:";
			// 
			// MogForm_MultiRepository
			// 
			this.AcceptButton = this.RepOKButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.RepCancelButton;
			this.ClientSize = new System.Drawing.Size(296, 85);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.RepositoryComboBox);
			this.Controls.Add(this.RepOKButton);
			this.Controls.Add(this.RepCancelButton);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.Name = "MogForm_MultiRepository";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Repositories";
			this.ResumeLayout(false);

		}
		#endregion
	}
}
