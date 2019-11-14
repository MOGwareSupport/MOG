using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MOG_Client.Forms
{
	/// <summary>
	/// Summary description for EditFileTypeButton.
	/// </summary>
	public class EditFileTypeButton : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button OkBut;
		private System.Windows.Forms.Button CancelBut;
		public  System.Windows.Forms.TextBox TBox_ButName;
		private System.Windows.Forms.Label label1;
		public  System.Windows.Forms.TextBox TBox_Ext;
		private System.Windows.Forms.Label label2;
		public  System.Windows.Forms.TextBox TBox_Path;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
		private System.Windows.Forms.Label label3;
		public  System.Windows.Forms.CheckBox Check_Hidden;
		private System.Windows.Forms.Button PathBrowseBut;
		private System.Windows.Forms.Label label4;
		public  System.Windows.Forms.TextBox TBox_Command;
		private System.Windows.Forms.Button ToolBrowseBut;
		public  System.Windows.Forms.TextBox FieldName;
		private System.Windows.Forms.Label label5;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public EditFileTypeButton()
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(EditFileTypeButton));
			this.OkBut = new System.Windows.Forms.Button();
			this.CancelBut = new System.Windows.Forms.Button();
			this.TBox_ButName = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.TBox_Ext = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.TBox_Path = new System.Windows.Forms.TextBox();
			this.PathBrowseBut = new System.Windows.Forms.Button();
			this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
			this.label3 = new System.Windows.Forms.Label();
			this.Check_Hidden = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.TBox_Command = new System.Windows.Forms.TextBox();
			this.ToolBrowseBut = new System.Windows.Forms.Button();
			this.FieldName = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// OkBut
			// 
			this.OkBut.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.OkBut.Location = new System.Drawing.Point(64, 208);
			this.OkBut.Name = "OkBut";
			this.OkBut.TabIndex = 0;
			this.OkBut.Text = "Ok";
			this.OkBut.Click += new System.EventHandler(this.OkBut_Click);
			// 
			// CancelBut
			// 
			this.CancelBut.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelBut.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.CancelBut.Location = new System.Drawing.Point(144, 208);
			this.CancelBut.Name = "CancelBut";
			this.CancelBut.TabIndex = 1;
			this.CancelBut.Text = "Cancel";
			// 
			// TBox_ButName
			// 
			this.TBox_ButName.Location = new System.Drawing.Point(24, 32);
			this.TBox_ButName.Name = "TBox_ButName";
			this.TBox_ButName.Size = new System.Drawing.Size(112, 20);
			this.TBox_ButName.TabIndex = 2;
			this.TBox_ButName.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(24, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 16);
			this.label1.TabIndex = 3;
			this.label1.Text = "Button Name";
			// 
			// TBox_Ext
			// 
			this.TBox_Ext.Location = new System.Drawing.Point(24, 176);
			this.TBox_Ext.Name = "TBox_Ext";
			this.TBox_Ext.Size = new System.Drawing.Size(112, 20);
			this.TBox_Ext.TabIndex = 4;
			this.TBox_Ext.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(24, 160);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(136, 16);
			this.label2.TabIndex = 5;
			this.label2.Text = "File Extension (ie: TXT)";
			// 
			// TBox_Path
			// 
			this.TBox_Path.Location = new System.Drawing.Point(24, 128);
			this.TBox_Path.Name = "TBox_Path";
			this.TBox_Path.Size = new System.Drawing.Size(160, 20);
			this.TBox_Path.TabIndex = 6;
			this.TBox_Path.Text = "";
			// 
			// PathBrowseBut
			// 
			this.PathBrowseBut.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PathBrowseBut.Location = new System.Drawing.Point(192, 128);
			this.PathBrowseBut.Name = "PathBrowseBut";
			this.PathBrowseBut.Size = new System.Drawing.Size(80, 23);
			this.PathBrowseBut.TabIndex = 7;
			this.PathBrowseBut.Text = "Browse Path";
			this.PathBrowseBut.Click += new System.EventHandler(this.BrowseBut_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(24, 112);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(168, 16);
			this.label3.TabIndex = 8;
			this.label3.Text = "Directory Path";
			// 
			// Check_Hidden
			// 
			this.Check_Hidden.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.Check_Hidden.Location = new System.Drawing.Point(152, 32);
			this.Check_Hidden.Name = "Check_Hidden";
			this.Check_Hidden.Size = new System.Drawing.Size(128, 24);
			this.Check_Hidden.TabIndex = 9;
			this.Check_Hidden.Text = "Hide Output";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(24, 64);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(100, 16);
			this.label4.TabIndex = 10;
			this.label4.Text = "External Tool Path";
			// 
			// TBox_Command
			// 
			this.TBox_Command.Location = new System.Drawing.Point(24, 80);
			this.TBox_Command.Name = "TBox_Command";
			this.TBox_Command.Size = new System.Drawing.Size(160, 20);
			this.TBox_Command.TabIndex = 11;
			this.TBox_Command.Text = "";
			// 
			// ToolBrowseBut
			// 
			this.ToolBrowseBut.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.ToolBrowseBut.Location = new System.Drawing.Point(192, 80);
			this.ToolBrowseBut.Name = "ToolBrowseBut";
			this.ToolBrowseBut.Size = new System.Drawing.Size(80, 23);
			this.ToolBrowseBut.TabIndex = 12;
			this.ToolBrowseBut.Text = "Browse Tool";
			this.ToolBrowseBut.Click += new System.EventHandler(this.ToolBrowseBut_Click);
			// 
			// FieldName
			// 
			this.FieldName.Location = new System.Drawing.Point(176, 176);
			this.FieldName.Name = "FieldName";
			this.FieldName.TabIndex = 13;
			this.FieldName.Text = "";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(176, 160);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(100, 16);
			this.label5.TabIndex = 14;
			this.label5.Text = "Field Name";
			// 
			// EditFileTypeButton
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(288, 246);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.FieldName);
			this.Controls.Add(this.ToolBrowseBut);
			this.Controls.Add(this.TBox_Command);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.Check_Hidden);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.PathBrowseBut);
			this.Controls.Add(this.TBox_Path);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.TBox_Ext);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.TBox_ButName);
			this.Controls.Add(this.CancelBut);
			this.Controls.Add(this.OkBut);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "EditFileTypeButton";
			this.Text = "EditFileTypeButton";
			this.ResumeLayout(false);

		}
		#endregion

		private void BrowseBut_Click(object sender, System.EventArgs e)
		{
			FolderBrowserDialog fbd = new FolderBrowserDialog();

			if (fbd.ShowDialog() == DialogResult.OK) 
			{
				TBox_Path.Text = fbd.SelectedPath;
			}
		}

		private void OkBut_Click(object sender, System.EventArgs e)
		{
			if(TBox_ButName.Text.Length == 0)
			{
				string caption = "ERROR";
				string message = "Button Name cannot be empty";
				MessageBoxButtons buttons = MessageBoxButtons.OK;

				MessageBox.Show(this, message, caption, buttons);
				return;
			}

			if(TBox_Path.Text.Length == 0)
			{
				string caption = "ERROR";
				string message = "Path cannot be empty";
				MessageBoxButtons buttons = MessageBoxButtons.OK;

				MessageBox.Show(this, message, caption, buttons);
				return;
			}

			if(TBox_Ext.Text.Length == 0)
			{
				string caption = "ERROR";
				string message = "File Extension cannot be empty";
				MessageBoxButtons buttons = MessageBoxButtons.OK;

				MessageBox.Show(this, message, caption, buttons);
				return;
			}

			if(TBox_Command.Text.Length == 0)
			{
				string caption = "ERROR";
				string message = "Tool Path cannot be empty";
				MessageBoxButtons buttons = MessageBoxButtons.OK;

				MessageBox.Show(this, message, caption, buttons);
				return;
			}

			Button b = (Button)sender;
			b.DialogResult = DialogResult.OK;
			this.DialogResult = DialogResult.OK;
			this.Close();
		
		}

		private void ToolBrowseBut_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog odf = new OpenFileDialog();

			if(odf.ShowDialog() == DialogResult.OK)
			{
				TBox_Command.Text = odf.FileName;
			}
		}
	}
}
