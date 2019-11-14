using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MOG_ServerManager.MOG_ControlsLibrary
{
	/// <summary>
	/// Summary description for ProgressBarForm.
	/// </summary>
	public class ProgressBarForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.Label lblGlobalDescription;
		private System.Windows.Forms.Label lblDescription;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ProgressBarForm(string title, string globalDescription)
		{
			InitializeComponent();

			this.Text = title;
			this.lblGlobalDescription.Text = globalDescription;
		}

		public void Update(int percent, string description)
		{
			this.progressBar.Value = percent;
			this.lblDescription.Text = description;
		}

		public void Update(string description)
		{
			this.lblDescription.Text = description;
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
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.lblGlobalDescription = new System.Windows.Forms.Label();
			this.lblDescription = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// progressBar
			// 
			this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.progressBar.Location = new System.Drawing.Point(16, 104);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(400, 24);
			this.progressBar.Step = 1;
			this.progressBar.TabIndex = 0;
			// 
			// lblGlobalDescription
			// 
			this.lblGlobalDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lblGlobalDescription.Location = new System.Drawing.Point(16, 8);
			this.lblGlobalDescription.Name = "lblGlobalDescription";
			this.lblGlobalDescription.Size = new System.Drawing.Size(408, 24);
			this.lblGlobalDescription.TabIndex = 1;
			// 
			// lblDescription
			// 
			this.lblDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lblDescription.Location = new System.Drawing.Point(13, 40);
			this.lblDescription.Name = "lblDescription";
			this.lblDescription.Size = new System.Drawing.Size(408, 48);
			this.lblDescription.TabIndex = 2;
			// 
			// ProgressBarForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(434, 144);
			this.ControlBox = false;
			this.Controls.Add(this.lblDescription);
			this.Controls.Add(this.lblGlobalDescription);
			this.Controls.Add(this.progressBar);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "ProgressBarForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = " ";
			this.TopMost = true;
			this.ResumeLayout(false);

		}
		#endregion
	}
}
