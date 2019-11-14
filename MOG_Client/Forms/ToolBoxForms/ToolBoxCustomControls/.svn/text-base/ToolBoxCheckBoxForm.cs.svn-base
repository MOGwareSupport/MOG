using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using MOG;

namespace MOG_Client.Forms
{
	/// <summary>
	/// Summary description for ToolBoxCheckBoxForm.
	/// </summary>
	public class ToolBoxCheckBoxForm : System.Windows.Forms.Form
	{
		private ToolBox mToolBox;

		private System.Windows.Forms.Button CancelButton1;
		private System.Windows.Forms.Button OkButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.CheckBox TagCheckBox;
		public System.Windows.Forms.TextBox TagTextBox;
		private System.Windows.Forms.ToolTip CustomToolTip;
		public System.Windows.Forms.TextBox TrueArgumentTextBox;
		public System.Windows.Forms.TextBox FalseArgumentTextBox;
		private System.Windows.Forms.Button TrueClearButton;
		private System.Windows.Forms.Button TrueArgumentsMenuButton;
		private System.Windows.Forms.Button TrueArgumentsButton;
		private System.Windows.Forms.Button FalseClearButton;
		private System.Windows.Forms.Button FalseArgumentsMenuButton;
		private System.Windows.Forms.Button FalseArgumentsButton;
		public System.Windows.Forms.TextBox ToolTipTextBox;
		private System.Windows.Forms.Label label5;
		public System.Windows.Forms.TextBox ControlNameTextBox;
		private GroupBox groupBox1;
		private System.ComponentModel.IContainer components;

		public ToolBoxCheckBoxForm( ToolBox toolBox )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.mToolBox = toolBox;
		}

		public void SetControlEnableBasedOnPrivilege(ToolBoxControlLocation location)
		{
			bool enable = true;

			switch (location)
			{
				case ToolBoxControlLocation.Project:
					enable = mToolBox.mPrivileges.GetUserPrivilege(mToolBox.mCurrentUserName, MOG_PRIVILEGE.ConfigureProjectCustomTools);
					break;
				case ToolBoxControlLocation.Department:
					enable = mToolBox.mPrivileges.GetUserPrivilege(mToolBox.mCurrentUserName, MOG_PRIVILEGE.ConfigureDepartmentCustomTools);
					break;
				case ToolBoxControlLocation.User:
					enable = mToolBox.mPrivileges.GetUserPrivilege(mToolBox.mCurrentUserName, MOG_PRIVILEGE.ConfigureUserCustomTools);
					break;
			}

			foreach (Control control in Controls)
			{
				if (control.Name != "OkButton" && control.Name != "CancelButton1")
				{
					control.Enabled = enable;
				}
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ToolBoxCheckBoxForm));
			this.CancelButton1 = new System.Windows.Forms.Button();
			this.OkButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.ControlNameTextBox = new System.Windows.Forms.TextBox();
			this.TrueArgumentTextBox = new System.Windows.Forms.TextBox();
			this.FalseArgumentTextBox = new System.Windows.Forms.TextBox();
			this.TagTextBox = new System.Windows.Forms.TextBox();
			this.TagCheckBox = new System.Windows.Forms.CheckBox();
			this.CustomToolTip = new System.Windows.Forms.ToolTip(this.components);
			this.ToolTipTextBox = new System.Windows.Forms.TextBox();
			this.TrueClearButton = new System.Windows.Forms.Button();
			this.TrueArgumentsButton = new System.Windows.Forms.Button();
			this.FalseClearButton = new System.Windows.Forms.Button();
			this.FalseArgumentsButton = new System.Windows.Forms.Button();
			this.TrueArgumentsMenuButton = new System.Windows.Forms.Button();
			this.FalseArgumentsMenuButton = new System.Windows.Forms.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// CancelButton1
			// 
			this.CancelButton1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.CancelButton1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelButton1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.CancelButton1.Location = new System.Drawing.Point(217, 202);
			this.CancelButton1.Name = "CancelButton1";
			this.CancelButton1.Size = new System.Drawing.Size(75, 23);
			this.CancelButton1.TabIndex = 13;
			this.CancelButton1.Text = "Cancel";
			// 
			// OkButton
			// 
			this.OkButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.OkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.OkButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.OkButton.Location = new System.Drawing.Point(129, 202);
			this.OkButton.Name = "OkButton";
			this.OkButton.Size = new System.Drawing.Size(75, 23);
			this.OkButton.TabIndex = 12;
			this.OkButton.Text = "OK";
			this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(10, 44);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(32, 16);
			this.label1.TabIndex = 15;
			this.label1.Text = "Tag:";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(16, 16);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(88, 16);
			this.label4.TabIndex = 18;
			this.label4.Text = "Display Name:";
			// 
			// ControlNameTextBox
			// 
			this.ControlNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.ControlNameTextBox.Location = new System.Drawing.Point(104, 16);
			this.ControlNameTextBox.Name = "ControlNameTextBox";
			this.ControlNameTextBox.Size = new System.Drawing.Size(275, 20);
			this.ControlNameTextBox.TabIndex = 0;
			this.CustomToolTip.SetToolTip(this.ControlNameTextBox, "Enter the name of this Checkbox as you would like it to display.");
			this.ControlNameTextBox.TextChanged += new System.EventHandler(this.DisplayNameTextBox_TextChanged);
			// 
			// TrueArgumentTextBox
			// 
			this.TrueArgumentTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.TrueArgumentTextBox.Location = new System.Drawing.Point(80, 74);
			this.TrueArgumentTextBox.Name = "TrueArgumentTextBox";
			this.TrueArgumentTextBox.Size = new System.Drawing.Size(176, 20);
			this.TrueArgumentTextBox.TabIndex = 4;
			this.CustomToolTip.SetToolTip(this.TrueArgumentTextBox, "Enter the argument you would like to use if the Checkbox is checked");
			// 
			// FalseArgumentTextBox
			// 
			this.FalseArgumentTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.FalseArgumentTextBox.Location = new System.Drawing.Point(80, 96);
			this.FalseArgumentTextBox.Name = "FalseArgumentTextBox";
			this.FalseArgumentTextBox.Size = new System.Drawing.Size(176, 20);
			this.FalseArgumentTextBox.TabIndex = 8;
			this.CustomToolTip.SetToolTip(this.FalseArgumentTextBox, "Enter the argument you would like to use if the Checkbox is unchecked");
			// 
			// TagTextBox
			// 
			this.TagTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.TagTextBox.Location = new System.Drawing.Point(48, 41);
			this.TagTextBox.Name = "TagTextBox";
			this.TagTextBox.Size = new System.Drawing.Size(305, 20);
			this.TagTextBox.TabIndex = 2;
			this.CustomToolTip.SetToolTip(this.TagTextBox, "Enter the tag which other toolbox controls will see");
			// 
			// TagCheckBox
			// 
			this.TagCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.TagCheckBox.Checked = true;
			this.TagCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.TagCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.TagCheckBox.Location = new System.Drawing.Point(13, 19);
			this.TagCheckBox.Name = "TagCheckBox";
			this.TagCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.TagCheckBox.Size = new System.Drawing.Size(172, 16);
			this.TagCheckBox.TabIndex = 1;
			this.TagCheckBox.Text = "Auto Create Tag from Name:";
			this.TagCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.CustomToolTip.SetToolTip(this.TagCheckBox, "Uncheck this if you would like to enter your own Tag name");
			// 
			// ToolTipTextBox
			// 
			this.ToolTipTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.ToolTipTextBox.Location = new System.Drawing.Point(104, 42);
			this.ToolTipTextBox.Name = "ToolTipTextBox";
			this.ToolTipTextBox.Size = new System.Drawing.Size(275, 20);
			this.ToolTipTextBox.TabIndex = 3;
			this.CustomToolTip.SetToolTip(this.ToolTipTextBox, "Enter the ToolTip you would like for users to see when their mouse hovers over th" +
					"is toolbox control");
			// 
			// TrueClearButton
			// 
			this.TrueClearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.TrueClearButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.TrueClearButton.Location = new System.Drawing.Point(312, 72);
			this.TrueClearButton.Name = "TrueClearButton";
			this.TrueClearButton.Size = new System.Drawing.Size(40, 23);
			this.TrueClearButton.TabIndex = 7;
			this.TrueClearButton.Text = "Clear";
			this.CustomToolTip.SetToolTip(this.TrueClearButton, "Clear all arguments");
			this.TrueClearButton.Click += new System.EventHandler(this.TrueClearButton_Click);
			// 
			// TrueArgumentsButton
			// 
			this.TrueArgumentsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.TrueArgumentsButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.TrueArgumentsButton.Location = new System.Drawing.Point(287, 72);
			this.TrueArgumentsButton.Name = "TrueArgumentsButton";
			this.TrueArgumentsButton.Size = new System.Drawing.Size(24, 23);
			this.TrueArgumentsButton.TabIndex = 6;
			this.TrueArgumentsButton.Text = "...";
			this.CustomToolTip.SetToolTip(this.TrueArgumentsButton, "Select multiple Arguments from a list");
			this.TrueArgumentsButton.Click += new System.EventHandler(this.TrueArgumentsButton_Click);
			// 
			// FalseClearButton
			// 
			this.FalseClearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.FalseClearButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.FalseClearButton.Location = new System.Drawing.Point(312, 94);
			this.FalseClearButton.Name = "FalseClearButton";
			this.FalseClearButton.Size = new System.Drawing.Size(40, 23);
			this.FalseClearButton.TabIndex = 11;
			this.FalseClearButton.Text = "Clear";
			this.CustomToolTip.SetToolTip(this.FalseClearButton, "Clear all arguments");
			this.FalseClearButton.Click += new System.EventHandler(this.FalseClearButton_Click);
			// 
			// FalseArgumentsButton
			// 
			this.FalseArgumentsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.FalseArgumentsButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.FalseArgumentsButton.Location = new System.Drawing.Point(287, 94);
			this.FalseArgumentsButton.Name = "FalseArgumentsButton";
			this.FalseArgumentsButton.Size = new System.Drawing.Size(24, 23);
			this.FalseArgumentsButton.TabIndex = 10;
			this.FalseArgumentsButton.Text = "...";
			this.CustomToolTip.SetToolTip(this.FalseArgumentsButton, "Select multiple Arguments from a list");
			this.FalseArgumentsButton.Click += new System.EventHandler(this.FalseArgumentsButton_Click);
			// 
			// TrueArgumentsMenuButton
			// 
			this.TrueArgumentsMenuButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.TrueArgumentsMenuButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.TrueArgumentsMenuButton.Image = ((System.Drawing.Image)(resources.GetObject("TrueArgumentsMenuButton.Image")));
			this.TrueArgumentsMenuButton.Location = new System.Drawing.Point(262, 72);
			this.TrueArgumentsMenuButton.Name = "TrueArgumentsMenuButton";
			this.TrueArgumentsMenuButton.Size = new System.Drawing.Size(24, 23);
			this.TrueArgumentsMenuButton.TabIndex = 5;
			this.TrueArgumentsMenuButton.Text = ">";
			this.CustomToolTip.SetToolTip(this.TrueArgumentsMenuButton, "Select Arguments from a Menu");
			this.TrueArgumentsMenuButton.Click += new System.EventHandler(this.ArgumentsMenuButton_Click);
			// 
			// FalseArgumentsMenuButton
			// 
			this.FalseArgumentsMenuButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.FalseArgumentsMenuButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.FalseArgumentsMenuButton.Image = ((System.Drawing.Image)(resources.GetObject("FalseArgumentsMenuButton.Image")));
			this.FalseArgumentsMenuButton.Location = new System.Drawing.Point(262, 94);
			this.FalseArgumentsMenuButton.Name = "FalseArgumentsMenuButton";
			this.FalseArgumentsMenuButton.Size = new System.Drawing.Size(24, 23);
			this.FalseArgumentsMenuButton.TabIndex = 9;
			this.FalseArgumentsMenuButton.Text = ">";
			this.CustomToolTip.SetToolTip(this.FalseArgumentsMenuButton, "Select Arguments from a Menu");
			this.FalseArgumentsMenuButton.Click += new System.EventHandler(this.ArgumentsMenuButton_Click);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(16, 42);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(56, 16);
			this.label5.TabIndex = 32;
			this.label5.Text = "ToolTip:";
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.TagCheckBox);
			this.groupBox1.Controls.Add(this.TagTextBox);
			this.groupBox1.Controls.Add(this.FalseArgumentTextBox);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.TrueArgumentTextBox);
			this.groupBox1.Controls.Add(this.TrueArgumentsMenuButton);
			this.groupBox1.Controls.Add(this.TrueArgumentsButton);
			this.groupBox1.Controls.Add(this.TrueClearButton);
			this.groupBox1.Controls.Add(this.FalseClearButton);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.FalseArgumentsMenuButton);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.FalseArgumentsButton);
			this.groupBox1.Location = new System.Drawing.Point(19, 68);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(359, 127);
			this.groupBox1.TabIndex = 33;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Tag Options";
			// 
			// label3
			// 
			this.label3.Image = global::MOG_Client.Properties.Resources.Unchecked;
			this.label3.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.label3.Location = new System.Drawing.Point(10, 96);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(64, 16);
			this.label3.TabIndex = 17;
			this.label3.Text = "If False:";
			// 
			// label2
			// 
			this.label2.Image = global::MOG_Client.Properties.Resources.Checked;
			this.label2.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.label2.Location = new System.Drawing.Point(10, 77);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(64, 16);
			this.label2.TabIndex = 16;
			this.label2.Text = "If True:";
			// 
			// ToolBoxCheckBoxForm
			// 
			this.AcceptButton = this.OkButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.CancelButton1;
			this.ClientSize = new System.Drawing.Size(395, 237);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.ToolTipTextBox);
			this.Controls.Add(this.ControlNameTextBox);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.CancelButton1);
			this.Controls.Add(this.OkButton);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MaximumSize = new System.Drawing.Size(3000, 271);
			this.MinimumSize = new System.Drawing.Size(403, 271);
			this.Name = "ToolBoxCheckBoxForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Create/Edit Checkbox";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// TODO: Validate that input is correct
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OkButton_Click(object sender, System.EventArgs e)
		{
			//Perform input validation here before we get to MOG ToolBox
		}

		/// <summary>
		/// Update TagTextBox according to what is in DisplayNameTextBox
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DisplayNameTextBox_TextChanged(object sender, System.EventArgs e)
		{
			// If TagCheckBox is checked, we will assign sender.Text to TagTextBox.Text
			if( this.TagCheckBox.Checked )
				this.TagTextBox.Text = ((TextBox)sender).Text.Replace( " ", "" );
		}

		private void ArgumentsMenuButton_Click( object sender, System.EventArgs e )
		{
			Button button = (Button)sender;
			ContextMenu cm = null;
			if( sender == this.TrueArgumentsMenuButton )
				cm = this.mToolBox.GenerateTagContextMenu( new EventHandler( this.TrueTagMenuItem_Click ) );
			else
				cm = this.mToolBox.GenerateTagContextMenu( new EventHandler( this.FalseTagMenuItem_Click ) );
			
			cm.Show( button, new Point( button.Width, 0 ) );			
		}

		private void TrueTagMenuItem_Click(object sender, EventArgs e)
		{
			string tag = ((MenuItem)sender).Text;
			this.TrueArgumentTextBox.Text += tag.Substring( tag.IndexOf("<"), tag.IndexOf(">") + (">").Length);
		}

		private void FalseTagMenuItem_Click(object sender, EventArgs e)
		{
			string tag = ((MenuItem)sender).Text;
			this.FalseArgumentTextBox.Text += tag.Substring( tag.IndexOf("<"), tag.IndexOf(">") + (">").Length);
		}

		private void TrueArgumentsButton_Click(object sender, System.EventArgs e)
		{
		
		}

		private void FalseArgumentsButton_Click(object sender, System.EventArgs e)
		{
		
		}

		private void TrueClearButton_Click(object sender, System.EventArgs e)
		{
			this.TrueArgumentTextBox.Text = "";
		}

		private void FalseClearButton_Click(object sender, System.EventArgs e)
		{
			this.FalseArgumentTextBox.Text = "";
		}
	}
}
