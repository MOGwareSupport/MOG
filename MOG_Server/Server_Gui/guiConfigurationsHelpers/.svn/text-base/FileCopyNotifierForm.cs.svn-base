using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MOG_Server
{
	/// <summary>
	/// Summary description for FileCopyNotifierForm.
	/// </summary>
	public class FileCopyNotifierForm : System.Windows.Forms.Form
	{
		#region System definitions

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
			this.label = new System.Windows.Forms.Label();
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.btnAbort = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label
			// 
			this.label.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.label.Location = new System.Drawing.Point(12, 13);
			this.label.Name = "label";
			this.label.Size = new System.Drawing.Size(720, 14);
			this.label.TabIndex = 5;
			this.label.Text = "Copying <source> to <dest>";
			// 
			// progressBar
			// 
			this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.progressBar.Location = new System.Drawing.Point(12, 31);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(718, 32);
			this.progressBar.Step = 1;
			this.progressBar.TabIndex = 4;
			// 
			// btnAbort
			// 
			this.btnAbort.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.btnAbort.Location = new System.Drawing.Point(336, 83);
			this.btnAbort.Name = "btnAbort";
			this.btnAbort.TabIndex = 3;
			this.btnAbort.Text = "Abort";
			// 
			// FileCopyNotifierForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(744, 118);
			this.ControlBox = false;
			this.Controls.Add(this.label);
			this.Controls.Add(this.progressBar);
			this.Controls.Add(this.btnAbort);
			this.MinimumSize = new System.Drawing.Size(248, 152);
			this.Name = "FileCopyNotifierForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "File Copy Progress";
			this.ResumeLayout(false);

		}
		#endregion

		private System.Windows.Forms.Label label;
		#endregion
		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.Button btnAbort;

		#region User definitions
		private bool aborted;

		public int NumFiles { get{return this.progressBar.Maximum;} set{this.progressBar.Maximum = value;} }
		public string LabelText { get{return this.label.Text;} set{this.label.Text = value;} }
		public string TitleText { get{return this.Text;} set{this.Text = value;} }
		public bool Aborted { get{return aborted;} }
		#endregion

		#region Constructor
		public FileCopyNotifierForm(int numFiles)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.progressBar.Maximum = numFiles;
			this.progressBar.Minimum = 0;
			this.progressBar.Step = 1;
			this.progressBar.Value = 0;

			this.aborted = false;
		}
		#endregion

		#region Member functions

		public void step() 
		{
			this.progressBar.PerformStep();
			Application.DoEvents();
		}
		#endregion

		#region Events
		private void btnAbort_Click(object sender, System.EventArgs e)
		{
			this.aborted = true;
		}
		#endregion
	}

	public class FileCopyNotifierFormAbortedByUserException : Exception 
	{
		public FileCopyNotifierFormAbortedByUserException(string msg) : base(msg) {}
	}
}
