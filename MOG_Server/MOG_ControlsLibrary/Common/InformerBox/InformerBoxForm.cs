using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MOG_Server.MOG_ControlsLibrary.Common
{
	/// <summary>
	/// Shows a small box with a label in it to communicate info to the user
	/// </summary>
	public class InformerBoxForm : System.Windows.Forms.Form
	{
		#region System defs
		private System.Windows.Forms.Label lblMessage;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(InformerBoxForm));
			this.lblMessage = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lblMessage
			// 
			this.lblMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lblMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblMessage.Location = new System.Drawing.Point(8, 16);
			this.lblMessage.Name = "lblMessage";
			this.lblMessage.Size = new System.Drawing.Size(256, 24);
			this.lblMessage.TabIndex = 0;
			this.lblMessage.Text = "Creating MOG Repository...";
			this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// InformerBoxForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(272, 54);
			this.ControlBox = false;
			this.Controls.Add(this.lblMessage);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "InformerBoxForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Info";
			this.TopMost = true;
			this.ResumeLayout(false);

		}
		#endregion
		#endregion
		#region Constructor
		public InformerBoxForm()
		{
			InitializeComponent();
		}

		public InformerBoxForm(string msg)
		{
			InitializeComponent();

			this.lblMessage.Text = msg;
		}

		public InformerBoxForm(string msg, string title)
		{
			InitializeComponent();

			this.lblMessage.Text = msg;
			this.Text = title;
		}
		#endregion
		#region Properties
		public string Message
		{
			get { return this.lblMessage.Text; }
			set { this.lblMessage.Text = value; }
		}

		public string Title
		{
			get { return this.Text; }
			set { this.Text = value; }
		}
		#endregion
	}
}
