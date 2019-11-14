using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using MOG.PROJECT;
using MOG.CONTROLLER.CONTROLLERPROJECT;

namespace MOG_ControlsLibrary.Forms
{
	/// <summary>
	/// Summary description for ClassificationCreateForm.
	/// </summary>
	public class ClassificationCreateForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox CurrentClassificationTextBox;
		private System.Windows.Forms.TextBox NewClassificationTextBox;
		private System.Windows.Forms.TextBox FullClassificationNameTextBox;
		private System.Windows.Forms.Button OKButton;
		private System.Windows.Forms.Button CancelButton1;

		public string FullClassificationName
		{
			get { return FullClassificationNameTextBox.Text; }
		}
	
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ClassificationCreateForm(string classification)
		{
			InitializeComponent();

			StartPosition = FormStartPosition.CenterParent;

			CurrentClassificationTextBox.Text = classification;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ClassificationCreateForm));
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.CurrentClassificationTextBox = new System.Windows.Forms.TextBox();
			this.NewClassificationTextBox = new System.Windows.Forms.TextBox();
			this.FullClassificationNameTextBox = new System.Windows.Forms.TextBox();
			this.OKButton = new System.Windows.Forms.Button();
			this.CancelButton1 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(56, 40);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(120, 16);
			this.label1.TabIndex = 2;
			this.label1.Text = "Parent Classification:";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(40, 8);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(128, 16);
			this.label2.TabIndex = 3;
			this.label2.Text = "Full Classification name:";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(16, 72);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(160, 16);
			this.label3.TabIndex = 4;
			this.label3.Text = "Enter new Classification here:";
			// 
			// CurrentClassificationTextBox
			// 
			this.CurrentClassificationTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.CurrentClassificationTextBox.Location = new System.Drawing.Point(176, 40);
			this.CurrentClassificationTextBox.Name = "CurrentClassificationTextBox";
			this.CurrentClassificationTextBox.ReadOnly = true;
			this.CurrentClassificationTextBox.Size = new System.Drawing.Size(368, 20);
			this.CurrentClassificationTextBox.TabIndex = 5;
			this.CurrentClassificationTextBox.Text = "";
			this.CurrentClassificationTextBox.TextChanged += new System.EventHandler(this.CurrentClassificationTextBox_TextChanged);
			// 
			// NewClassificationTextBox
			// 
			this.NewClassificationTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.NewClassificationTextBox.Location = new System.Drawing.Point(176, 72);
			this.NewClassificationTextBox.Name = "NewClassificationTextBox";
			this.NewClassificationTextBox.Size = new System.Drawing.Size(368, 20);
			this.NewClassificationTextBox.TabIndex = 0;
			this.NewClassificationTextBox.Text = "";
			this.NewClassificationTextBox.TextChanged += new System.EventHandler(this.NewClassificationTextBox_TextChanged);
			// 
			// FullClassificationNameTextBox
			// 
			this.FullClassificationNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.FullClassificationNameTextBox.Location = new System.Drawing.Point(176, 8);
			this.FullClassificationNameTextBox.Name = "FullClassificationNameTextBox";
			this.FullClassificationNameTextBox.ReadOnly = true;
			this.FullClassificationNameTextBox.Size = new System.Drawing.Size(368, 20);
			this.FullClassificationNameTextBox.TabIndex = 6;
			this.FullClassificationNameTextBox.Text = "";
			// 
			// OKButton
			// 
			this.OKButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.OKButton.Location = new System.Drawing.Point(196, 104);
			this.OKButton.Name = "OKButton";
			this.OKButton.TabIndex = 9;
			this.OKButton.Text = "OK";
			this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
			// 
			// CancelButton1
			// 
			this.CancelButton1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.CancelButton1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelButton1.Location = new System.Drawing.Point(284, 104);
			this.CancelButton1.Name = "CancelButton1";
			this.CancelButton1.TabIndex = 8;
			this.CancelButton1.Text = "Cancel";
			// 
			// ClassificationCreateForm
			// 
			this.AcceptButton = this.OKButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.CancelButton1;
			this.ClientSize = new System.Drawing.Size(552, 132);
			this.Controls.Add(this.OKButton);
			this.Controls.Add(this.CancelButton1);
			this.Controls.Add(this.NewClassificationTextBox);
			this.Controls.Add(this.FullClassificationNameTextBox);
			this.Controls.Add(this.CurrentClassificationTextBox);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MaximumSize = new System.Drawing.Size(2000, 160);
			this.MinimumSize = new System.Drawing.Size(416, 160);
			this.Name = "ClassificationCreateForm";
			this.Text = "Add a classification";
			this.ResumeLayout(false);

		}
		#endregion

		private void CurrentClassificationTextBox_TextChanged(object sender, System.EventArgs e)
		{
			this.FullClassificationNameTextBox.Text = CurrentClassificationTextBox.Text;
		}

		private void NewClassificationTextBox_TextChanged(object sender, System.EventArgs e)
		{
			this.FullClassificationNameTextBox.Text = CurrentClassificationTextBox.Text + "~" + ((TextBox)sender).Text;
		}

		private void OKButton_Click(object sender, System.EventArgs e)
		{
			if( this.NewClassificationTextBox.Text == null || this.NewClassificationTextBox.Text.Length < 1 )
			{
				MessageBox.Show( this, "Please enter a valid classification name, or click 'Cancel' " 
					+ "to close the dialog.", "No classification name found!",
					MessageBoxButtons.OK);
				return;
			}

			if( this.FullClassificationNameTextBox.Text.IndexOf("{") > -1 ||
				this.FullClassificationNameTextBox.Text.IndexOf("}") > -1 )
			{
				MessageBox.Show( this, "Invalid character (for Classification), '{' and/or '}', detected.\r\n\r\n"
					+"Classification name:\r\n" + this.FullClassificationNameTextBox.Text,
					"Invalid Character(s) for Classification Name Found!", MessageBoxButtons.OK, MessageBoxIcon.Warning );
				this.CurrentClassificationTextBox.Text = this.CurrentClassificationTextBox.Text.Replace("{", "" );
				this.CurrentClassificationTextBox.Text = this.CurrentClassificationTextBox.Text.Replace("}", "" );
				this.FullClassificationNameTextBox.Text = this.FullClassificationNameTextBox.Text.Replace("{", "" );
				this.FullClassificationNameTextBox.Text = this.FullClassificationNameTextBox.Text.Replace("}", "" );
				return;
			}

			MOG_Project project = MOG_ControllerProject.GetProject();
			project.ClassificationAdd(FullClassificationNameTextBox.Text);
			
			this.DialogResult = DialogResult.OK;
			this.Close();
		}
	}
}
