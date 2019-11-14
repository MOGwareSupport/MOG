using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MOG_Client.Forms
{
	/// <summary>
	/// Summary description for EditFileBrowserButton.
	/// </summary>
	public class EditFileBrowserButton : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		public  System.Windows.Forms.TextBox FieldName;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		public  System.Windows.Forms.TextBox ButtonName;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public EditFileBrowserButton()
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(EditFileBrowserButton));
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.FieldName = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.ButtonName = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.button1.Location = new System.Drawing.Point(23, 136);
			this.button1.Name = "button1";
			this.button1.TabIndex = 0;
			this.button1.Text = "Ok";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.button2.Location = new System.Drawing.Point(111, 136);
			this.button2.Name = "button2";
			this.button2.TabIndex = 1;
			this.button2.Text = "Cancel";
			// 
			// FieldName
			// 
			this.FieldName.Location = new System.Drawing.Point(36, 96);
			this.FieldName.Name = "FieldName";
			this.FieldName.Size = new System.Drawing.Size(136, 20);
			this.FieldName.TabIndex = 2;
			this.FieldName.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(36, 72);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 16);
			this.label1.TabIndex = 3;
			this.label1.Text = "Field Name";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(36, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 16);
			this.label2.TabIndex = 5;
			this.label2.Text = "Button Name";
			// 
			// ButtonName
			// 
			this.ButtonName.Location = new System.Drawing.Point(36, 40);
			this.ButtonName.Name = "ButtonName";
			this.ButtonName.Size = new System.Drawing.Size(136, 20);
			this.ButtonName.TabIndex = 4;
			this.ButtonName.Text = "";
			// 
			// EditFileBrowserButton
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(208, 174);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.ButtonName);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.FieldName);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "EditFileBrowserButton";
			this.Text = "EditFileBrowserButton";
			this.Load += new System.EventHandler(this.EditFileBrowserButton_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void button1_Click(object sender, System.EventArgs e)
		{
		
		}

		private void EditFileBrowserButton_Load(object sender, System.EventArgs e)
		{
		
		}
	}
}
