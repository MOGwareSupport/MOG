using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MOG_Server_Loader
{
	/// <summary>
	/// Summary description for NotesForm.
	/// </summary>
	public class NotesForm : System.Windows.Forms.Form
	{
		public System.Windows.Forms.RichTextBox NotesRichTextBox;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button CloseButton;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public NotesForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			string WhatsNewFilename = "M:\\Mog\\Updates\\1.5\\Client\\WhatsNew.txt";

			FileInfo file = new FileInfo(WhatsNewFilename);

			if (file.Exists)
			{
				StreamReader read = file.OpenText();
				NotesRichTextBox.Text = read.ReadToEnd();
				read.Close();				
			}
			
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(NotesForm));
			this.NotesRichTextBox = new System.Windows.Forms.RichTextBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.CloseButton = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// NotesRichTextBox
			// 
			this.NotesRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.NotesRichTextBox.Location = new System.Drawing.Point(0, 0);
			this.NotesRichTextBox.Name = "NotesRichTextBox";
			this.NotesRichTextBox.Size = new System.Drawing.Size(304, 238);
			this.NotesRichTextBox.TabIndex = 0;
			this.NotesRichTextBox.Text = "";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.CloseButton);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 238);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(304, 40);
			this.panel1.TabIndex = 1;
			// 
			// CloseButton
			// 
			this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.CloseButton.Location = new System.Drawing.Point(224, 8);
			this.CloseButton.Name = "CloseButton";
			this.CloseButton.TabIndex = 0;
			this.CloseButton.Text = "Close";
			// 
			// NotesForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(304, 278);
			this.Controls.Add(this.NotesRichTextBox);
			this.Controls.Add(this.panel1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "NotesForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "NotesForm";
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
