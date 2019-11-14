using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

using MOG_Client.Client_Mog_Utilities;
using MOG.DOSUTILS;
using System.Diagnostics;

namespace MOG_EventViewer
{
	/// <summary>
	/// Summary description for AboutForm.
	/// </summary>
	public class AboutForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button1;
		public System.Windows.Forms.RichTextBox AboutWhatsNewRichTextBox;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.LinkLabel AboutMogWareLinkLabel;
		private System.Windows.Forms.Label AboutVersionLabel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Panel ArtPanel;
		public RichTextBox richTextBox1;
		private TabControl tabControl1;
		private TabPage tabPage1;
		private TabPage tabPage2;
		private TabPage tabPage3;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public AboutForm(string filename)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			FileInfo file = new FileInfo(filename);

			if (file.Exists)
			{
				StreamReader read = file.OpenText();
				AboutWhatsNewRichTextBox.Text = read.ReadToEnd();
				read.Close();				
			}

			// Setup our version number
			string tempVersion = "";
			FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(Application.ExecutablePath);
			if (fileVersionInfo != null)
			{
				tempVersion = fileVersionInfo.FileVersion;
				if (tempVersion == "3.0.0.0") tempVersion = "Debug build";
			}
			
			AboutVersionLabel.Text += " " + tempVersion;
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
			this.AboutVersionLabel = new System.Windows.Forms.Label();
			this.AboutWhatsNewRichTextBox = new System.Windows.Forms.RichTextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.AboutMogWareLinkLabel = new System.Windows.Forms.LinkLabel();
			this.button1 = new System.Windows.Forms.Button();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.label1 = new System.Windows.Forms.Label();
			this.ArtPanel = new System.Windows.Forms.Panel();
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.ArtPanel.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabPage3.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.SuspendLayout();
			// 
			// AboutVersionLabel
			// 
			this.AboutVersionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.AboutVersionLabel.Location = new System.Drawing.Point(3, 49);
			this.AboutVersionLabel.Name = "AboutVersionLabel";
			this.AboutVersionLabel.Size = new System.Drawing.Size(464, 16);
			this.AboutVersionLabel.TabIndex = 0;
			this.AboutVersionLabel.Text = "Current Version Number:";
			// 
			// AboutWhatsNewRichTextBox
			// 
			this.AboutWhatsNewRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.AboutWhatsNewRichTextBox.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.AboutWhatsNewRichTextBox.Location = new System.Drawing.Point(3, 19);
			this.AboutWhatsNewRichTextBox.Name = "AboutWhatsNewRichTextBox";
			this.AboutWhatsNewRichTextBox.ReadOnly = true;
			this.AboutWhatsNewRichTextBox.Size = new System.Drawing.Size(476, 277);
			this.AboutWhatsNewRichTextBox.TabIndex = 1;
			this.AboutWhatsNewRichTextBox.TabStop = false;
			this.AboutWhatsNewRichTextBox.Text = "";
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(3, 104);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(472, 56);
			this.label4.TabIndex = 5;
			this.label4.Text = resources.GetString("label4.Text");
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(3, 27);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(336, 16);
			this.label3.TabIndex = 4;
			this.label3.Text = "Copyright (c) MOGware LLC 2005. All rights reserved";
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(3, 3);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(248, 24);
			this.label2.TabIndex = 3;
			this.label2.Text = "MOGware transMOGrifier";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// AboutMogWareLinkLabel
			// 
			this.AboutMogWareLinkLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.AboutMogWareLinkLabel.Location = new System.Drawing.Point(11, 75);
			this.AboutMogWareLinkLabel.Name = "AboutMogWareLinkLabel";
			this.AboutMogWareLinkLabel.Size = new System.Drawing.Size(128, 16);
			this.AboutMogWareLinkLabel.TabIndex = 2;
			this.AboutMogWareLinkLabel.TabStop = true;
			this.AboutMogWareLinkLabel.Text = "Visit MOGware.com";
			this.AboutMogWareLinkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.AboutMogWareLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.AboutMogWareLinkLabel_LinkClicked);
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.button1.Location = new System.Drawing.Point(475, 331);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 0;
			this.button1.Text = "Close";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(0, 0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(64, 360);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pictureBox1.TabIndex = 1;
			this.pictureBox1.TabStop = false;
			// 
			// label1
			// 
			this.label1.Dock = System.Windows.Forms.DockStyle.Top;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(3, 3);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(476, 16);
			this.label1.TabIndex = 7;
			this.label1.Text = "Whats new in this version:";
			// 
			// ArtPanel
			// 
			this.ArtPanel.BackColor = System.Drawing.Color.White;
			this.ArtPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.ArtPanel.Controls.Add(this.pictureBox1);
			this.ArtPanel.Dock = System.Windows.Forms.DockStyle.Left;
			this.ArtPanel.Location = new System.Drawing.Point(0, 0);
			this.ArtPanel.Name = "ArtPanel";
			this.ArtPanel.Size = new System.Drawing.Size(64, 360);
			this.ArtPanel.TabIndex = 8;
			// 
			// richTextBox1
			// 
			this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.richTextBox1.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.richTextBox1.Location = new System.Drawing.Point(3, 3);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.ReadOnly = true;
			this.richTextBox1.Size = new System.Drawing.Size(476, 293);
			this.richTextBox1.TabIndex = 9;
			this.richTextBox1.TabStop = false;
			this.richTextBox1.Text = resources.GetString("richTextBox1.Text");
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Location = new System.Drawing.Point(64, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(490, 325);
			this.tabControl1.TabIndex = 10;
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this.AboutVersionLabel);
			this.tabPage3.Controls.Add(this.label4);
			this.tabPage3.Controls.Add(this.label3);
			this.tabPage3.Controls.Add(this.label2);
			this.tabPage3.Controls.Add(this.AboutMogWareLinkLabel);
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage3.Size = new System.Drawing.Size(482, 299);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "Info";
			this.tabPage3.UseVisualStyleBackColor = true;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.AboutWhatsNewRichTextBox);
			this.tabPage1.Controls.Add(this.label1);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(482, 299);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "What\'s New";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.richTextBox1);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(482, 299);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Credits";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// AboutForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(554, 360);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.ArtPanel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "AboutForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "About MOG Event Viewer";
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ArtPanel.ResumeLayout(false);
			this.ArtPanel.PerformLayout();
			this.tabControl1.ResumeLayout(false);
			this.tabPage3.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void button1_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void AboutMogWareLinkLabel_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			guiCommandLine.ShellSpawn("www.mogware.com");
		}

		private void AboutMogToolsLinkLabel_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			guiCommandLine.ShellSpawn("www.mogtools.com");
		}
	}
}
