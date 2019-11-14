using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using MOG.COMMAND.SLAVE;

namespace MOG_Slave
{
	/// <summary>
	/// Summary description for CommandLineLog.
	/// </summary>
	public class CommandLineLog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.RichTextBox richTextBoxOutputLog;
		private System.Windows.Forms.Button btnDone;
		private System.Windows.Forms.Button btnSaveLog;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.TextBox textBoxMaxSize;
		private System.Windows.Forms.Label label1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Button btnClear;
		private System.Windows.Forms.Button btnRefresh;
		private System.Windows.Forms.SaveFileDialog saveLogFileDialog;

		public MOG_CommandSlave mSlave;

		public CommandLineLog(MOG_CommandSlave slave)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			mSlave = slave;

			// Load the output log into the window
			// Set the Multiline property to true.
			richTextBoxOutputLog.Multiline = true;
			// Set WordWrap to true to allow text to wrap to the next line.
			richTextBoxOutputLog.WordWrap = true;
			// Add vertical scroll bars to the TextBox control.
		    richTextBoxOutputLog.ScrollBars = RichTextBoxScrollBars.ForcedBoth;

			// Get the max text length
			textBoxMaxSize.Text = Convert.ToString(mSlave.GetMaxLogLength());

			UpdateLog();
		}

		public void UpdateLog()
		{
			// Set the default text of the control.
			richTextBoxOutputLog.Text = mSlave.GetOutputLog();
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(CommandLineLog));
			this.richTextBoxOutputLog = new System.Windows.Forms.RichTextBox();
			this.btnDone = new System.Windows.Forms.Button();
			this.btnSaveLog = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.btnRefresh = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxMaxSize = new System.Windows.Forms.TextBox();
			this.btnClear = new System.Windows.Forms.Button();
			this.saveLogFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// richTextBoxOutputLog
			// 
			this.richTextBoxOutputLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.richTextBoxOutputLog.AutoSize = true;
			this.richTextBoxOutputLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.richTextBoxOutputLog.Location = new System.Drawing.Point(0, 0);
			this.richTextBoxOutputLog.Name = "richTextBoxOutputLog";
			this.richTextBoxOutputLog.ShowSelectionMargin = true;
			this.richTextBoxOutputLog.Size = new System.Drawing.Size(488, 248);
			this.richTextBoxOutputLog.TabIndex = 0;
			this.richTextBoxOutputLog.Text = "";
			this.richTextBoxOutputLog.WordWrap = false;
			// 
			// btnDone
			// 
			this.btnDone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDone.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnDone.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnDone.Location = new System.Drawing.Point(8, 88);
			this.btnDone.Name = "btnDone";
			this.btnDone.TabIndex = 1;
			this.btnDone.Text = "Done";
			this.btnDone.Click += new System.EventHandler(this.btnDone_Click);
			// 
			// btnSaveLog
			// 
			this.btnSaveLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSaveLog.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnSaveLog.Location = new System.Drawing.Point(8, 64);
			this.btnSaveLog.Name = "btnSaveLog";
			this.btnSaveLog.TabIndex = 2;
			this.btnSaveLog.Text = "Save Log";
			this.btnSaveLog.Click += new System.EventHandler(this.btnSaveLog_Click);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.richTextBoxOutputLog);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(576, 249);
			this.panel1.TabIndex = 5;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.btnRefresh);
			this.panel2.Controls.Add(this.label1);
			this.panel2.Controls.Add(this.textBoxMaxSize);
			this.panel2.Controls.Add(this.btnDone);
			this.panel2.Controls.Add(this.btnClear);
			this.panel2.Controls.Add(this.btnSaveLog);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel2.Location = new System.Drawing.Point(488, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(88, 249);
			this.panel2.TabIndex = 6;
			// 
			// btnRefresh
			// 
			this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnRefresh.Location = new System.Drawing.Point(8, 16);
			this.btnRefresh.Name = "btnRefresh";
			this.btnRefresh.TabIndex = 6;
			this.btnRefresh.Text = "Refresh";
			this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(8, 112);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 16);
			this.label1.TabIndex = 5;
			this.label1.Text = "Max Size";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// textBoxMaxSize
			// 
			this.textBoxMaxSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxMaxSize.Location = new System.Drawing.Point(8, 128);
			this.textBoxMaxSize.Name = "textBoxMaxSize";
			this.textBoxMaxSize.Size = new System.Drawing.Size(72, 20);
			this.textBoxMaxSize.TabIndex = 4;
			this.textBoxMaxSize.Text = "";
			this.textBoxMaxSize.TextChanged += new System.EventHandler(this.textBoxMaxSize_TextChanged);
			// 
			// btnClear
			// 
			this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnClear.Location = new System.Drawing.Point(8, 40);
			this.btnClear.Name = "btnClear";
			this.btnClear.TabIndex = 3;
			this.btnClear.Text = "Clear Log";
			this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
			// 
			// saveLogFileDialog
			// 
			this.saveLogFileDialog.DefaultExt = "log";
			this.saveLogFileDialog.FileName = "SlaveLog";
			this.saveLogFileDialog.Filter = "Log Files (*.log)|*.log";
			this.saveLogFileDialog.Title = "Save Slave Log";
			// 
			// CommandLineLog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(576, 249);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "CommandLineLog";
			this.Text = "CommandLineLog";
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void btnDone_Click(object sender, System.EventArgs e)
		{
			this.Hide();
		}

		private void textBoxMaxSize_TextChanged(object sender, System.EventArgs e)
		{
			mSlave.SetMaxLogLength(Convert.ToInt32(textBoxMaxSize.Text));
		}

		private void btnClear_Click(object sender, System.EventArgs e)
		{
			mSlave.SetOutputLog("");
			richTextBoxOutputLog.Text = mSlave.GetOutputLog();
		}

		private void btnRefresh_Click(object sender, System.EventArgs e)
		{
			UpdateLog();
		}

		private void btnSaveLog_Click(object sender, System.EventArgs e)
		{
			if (this.saveLogFileDialog.ShowDialog() == DialogResult.OK)
			{
				MOG.DOSUTILS.DosUtils.AppendTextToFile(this.saveLogFileDialog.FileName, mSlave.GetOutputLog());
			}
		}
	}
}
