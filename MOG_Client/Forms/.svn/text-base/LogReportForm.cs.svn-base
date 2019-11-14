using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using MOG.DOSUTILS;
using MOG_Client.Client_Utilities;

using MOG_Client;

namespace MOG_Client.Forms
{
	/// <summary>
	/// Summary description for LogReportForm.
	/// </summary>
	public class Report : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel panel1;
		public System.Windows.Forms.Button LogOkButton;
		public System.Windows.Forms.RichTextBox LogRichTextBox;
		private System.Windows.Forms.Button LogSaveButton;
		private System.Windows.Forms.SaveFileDialog LogSaveFileDialog;
		private MogMainForm mainForm;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Report(MogMainForm main)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			mainForm = main;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Report));
			this.panel1 = new System.Windows.Forms.Panel();
			this.LogSaveButton = new System.Windows.Forms.Button();
			this.LogOkButton = new System.Windows.Forms.Button();
			this.LogRichTextBox = new System.Windows.Forms.RichTextBox();
			this.LogSaveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.LogSaveButton);
			this.panel1.Controls.Add(this.LogOkButton);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 233);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(292, 40);
			this.panel1.TabIndex = 0;
			// 
			// LogSaveButton
			// 
			this.LogSaveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.LogSaveButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.LogSaveButton.Location = new System.Drawing.Point(139, 8);
			this.LogSaveButton.Name = "LogSaveButton";
			this.LogSaveButton.TabIndex = 1;
			this.LogSaveButton.Text = "Save Report";
			this.LogSaveButton.Click += new System.EventHandler(this.LogSaveButton_Click);
			// 
			// LogOkButton
			// 
			this.LogOkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.LogOkButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.LogOkButton.Location = new System.Drawing.Point(216, 8);
			this.LogOkButton.Name = "LogOkButton";
			this.LogOkButton.TabIndex = 0;
			this.LogOkButton.Text = "Close";
			this.LogOkButton.Click += new System.EventHandler(this.LogOkButton_Click);
			// 
			// LogRichTextBox
			// 
			this.LogRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.LogRichTextBox.Location = new System.Drawing.Point(0, 0);
			this.LogRichTextBox.Name = "LogRichTextBox";
			this.LogRichTextBox.Size = new System.Drawing.Size(292, 233);
			this.LogRichTextBox.TabIndex = 1;
			this.LogRichTextBox.Text = "";
			// 
			// LogSaveFileDialog
			// 
			this.LogSaveFileDialog.DefaultExt = "txt";
			this.LogSaveFileDialog.Filter = "Text Files | *.txt";
			this.LogSaveFileDialog.Title = "Save to text file";
			// 
			// Report
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Controls.Add(this.LogRichTextBox);
			this.Controls.Add(this.panel1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "Report";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "LogReportForm";
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void LogOkButton_Click(object sender, System.EventArgs e)
		{
			// Save our forms dimensions
			//mainForm.mUserPrefs.Save("ReportForm", this);
			guiUserPrefs.SaveDynamic_LayoutPrefs("ReportForm", this);
			Close();
		}

		private void LogSaveButton_Click(object sender, System.EventArgs e)
		{
			if (LogSaveFileDialog.ShowDialog() == DialogResult.OK)
			{
				LogRichTextBox.SaveFile(LogSaveFileDialog.FileName);
			}
		}
	}
}
