using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace MOG_ServerManager
{
	/// <summary>
	/// Summary description for WizardHeader.
	/// </summary>
	public class WizardHeader : System.Windows.Forms.UserControl
	{

		[Category("WizardHeader")]
		public string Title
		{
			get { return this.lblTitle.Text; } 
			set { this.lblTitle.Text = value; }
		}

		[Category("WizardHeader")]
		public string Description
		{
			get { return this.lblDescription.Text; }
			set { this.lblDescription.Text = value; }
		}

		[Category("WizardHeader")]
		public Image Image 
		{
			get { return this.picBox.Image; }
			set { this.picBox.Image = value; }
		}



		private System.Windows.Forms.Label lblTitle;
		private System.Windows.Forms.Label lblDescription;
		private System.Windows.Forms.PictureBox picBox;
		private System.Windows.Forms.Label label1;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public WizardHeader()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(WizardHeader));
			this.lblTitle = new System.Windows.Forms.Label();
			this.lblDescription = new System.Windows.Forms.Label();
			this.picBox = new System.Windows.Forms.PictureBox();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lblTitle
			// 
			this.lblTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lblTitle.BackColor = System.Drawing.SystemColors.Window;
			this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblTitle.Location = new System.Drawing.Point(0, 0);
			this.lblTitle.Name = "lblTitle";
			this.lblTitle.Size = new System.Drawing.Size(432, 24);
			this.lblTitle.TabIndex = 3;
			this.lblTitle.Text = "Title";
			this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblDescription
			// 
			this.lblDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lblDescription.BackColor = System.Drawing.SystemColors.Window;
			this.lblDescription.Location = new System.Drawing.Point(8, 24);
			this.lblDescription.Name = "lblDescription";
			this.lblDescription.Size = new System.Drawing.Size(432, 36);
			this.lblDescription.TabIndex = 4;
			this.lblDescription.Text = "Description";
			// 
			// picBox
			// 
			this.picBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.picBox.Image = ((System.Drawing.Image)(resources.GetObject("picBox.Image")));
			this.picBox.Location = new System.Drawing.Point(352, 0);
			this.picBox.Name = "picBox";
			this.picBox.Size = new System.Drawing.Size(64, 60);
			this.picBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.picBox.TabIndex = 5;
			this.picBox.TabStop = false;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.label1.BackColor = System.Drawing.SystemColors.Window;
			this.label1.Location = new System.Drawing.Point(-8, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(312, 48);
			this.label1.TabIndex = 6;
			// 
			// WizardHeader
			// 
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(this.picBox);
			this.Controls.Add(this.lblDescription);
			this.Controls.Add(this.lblTitle);
			this.Controls.Add(this.label1);
			this.Name = "WizardHeader";
			this.Size = new System.Drawing.Size(416, 60);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
