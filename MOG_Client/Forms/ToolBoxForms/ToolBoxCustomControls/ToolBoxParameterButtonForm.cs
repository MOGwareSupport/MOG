using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using MOG_Client.Forms.ToolBoxForms;

using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG;

namespace MOG_Client.Forms
{
	/// <summary>
	/// Summary description for CustomButtonEdit.
	/// </summary>
	public class EditParameterButtonForm : System.Windows.Forms.Form
	{
		private ToolBox mToolBox;

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button BrowseButton;

		public System.Windows.Forms.TextBox CommandTextBox;
		public System.Windows.Forms.TextBox NameTextBox;
		public System.Windows.Forms.TextBox ArgumentsTextBox;
		private System.Windows.Forms.Button OkButton;
		private System.Windows.Forms.Button CancelButton1;
		public System.Windows.Forms.CheckBox HideOutputCheckBox;
		public System.Windows.Forms.TextBox TagTextBox;
		private System.Windows.Forms.Label label3;
		public System.Windows.Forms.CheckBox TagCheckBox;
		private System.Windows.Forms.Button ArgumentsButton;
		private System.Windows.Forms.Button ArgumentsMenuButton;
		private System.Windows.Forms.Label label5;
		public System.Windows.Forms.TextBox ToolTipTextBox;
		private System.Windows.Forms.Button ClearButton;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.Button CommandArgumentsButton;
		private GroupBox groupBox1;
		private GroupBox groupBox2;
		private System.ComponentModel.IContainer components;

		public EditParameterButtonForm( ToolBox toolBox )
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditParameterButtonForm));
			this.CommandTextBox = new System.Windows.Forms.TextBox();
			this.OkButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.NameTextBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.HideOutputCheckBox = new System.Windows.Forms.CheckBox();
			this.BrowseButton = new System.Windows.Forms.Button();
			this.TagTextBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.ArgumentsTextBox = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.CancelButton1 = new System.Windows.Forms.Button();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.ArgumentsButton = new System.Windows.Forms.Button();
			this.TagCheckBox = new System.Windows.Forms.CheckBox();
			this.ArgumentsMenuButton = new System.Windows.Forms.Button();
			this.ToolTipTextBox = new System.Windows.Forms.TextBox();
			this.ClearButton = new System.Windows.Forms.Button();
			this.CommandArgumentsButton = new System.Windows.Forms.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// CommandTextBox
			// 
			this.CommandTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.CommandTextBox.Location = new System.Drawing.Point(116, 37);
			this.CommandTextBox.Name = "CommandTextBox";
			this.CommandTextBox.Size = new System.Drawing.Size(191, 20);
			this.CommandTextBox.TabIndex = 2;
			// 
			// OkButton
			// 
			this.OkButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.OkButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.OkButton.Location = new System.Drawing.Point(240, 164);
			this.OkButton.Name = "OkButton";
			this.OkButton.Size = new System.Drawing.Size(75, 23);
			this.OkButton.TabIndex = 12;
			this.OkButton.Text = "OK";
			this.OkButton.Click += new System.EventHandler(this.Ok_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(3, 40);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(122, 16);
			this.label1.TabIndex = 3;
			this.label1.Text = "Path to External Tool:";
			// 
			// NameTextBox
			// 
			this.NameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.NameTextBox.Location = new System.Drawing.Point(90, 8);
			this.NameTextBox.Name = "NameTextBox";
			this.NameTextBox.Size = new System.Drawing.Size(546, 20);
			this.NameTextBox.TabIndex = 0;
			this.NameTextBox.TextChanged += new System.EventHandler(this.NameTextBox_TextChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 8);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(80, 16);
			this.label2.TabIndex = 5;
			this.label2.Text = "Button Name:";
			// 
			// HideOutputCheckBox
			// 
			this.HideOutputCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.HideOutputCheckBox.Location = new System.Drawing.Point(6, 13);
			this.HideOutputCheckBox.Name = "HideOutputCheckBox";
			this.HideOutputCheckBox.Size = new System.Drawing.Size(104, 24);
			this.HideOutputCheckBox.TabIndex = 1;
			this.HideOutputCheckBox.Text = "Hide Output";
			this.toolTip.SetToolTip(this.HideOutputCheckBox, "Place a check in this CheckBox if you would like to hide the output window");
			// 
			// BrowseButton
			// 
			this.BrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.BrowseButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.BrowseButton.Location = new System.Drawing.Point(338, 35);
			this.BrowseButton.Name = "BrowseButton";
			this.BrowseButton.Size = new System.Drawing.Size(74, 23);
			this.BrowseButton.TabIndex = 4;
			this.BrowseButton.Text = "Browse";
			this.toolTip.SetToolTip(this.BrowseButton, "Browse for the file or script you would like to run on pressing this Custom Butto" +
					"n");
			this.BrowseButton.Click += new System.EventHandler(this.BrowseButton_Click);
			// 
			// TagTextBox
			// 
			this.TagTextBox.Location = new System.Drawing.Point(43, 37);
			this.TagTextBox.Name = "TagTextBox";
			this.TagTextBox.Size = new System.Drawing.Size(151, 20);
			this.TagTextBox.TabIndex = 9;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(6, 40);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(40, 16);
			this.label3.TabIndex = 9;
			this.label3.Text = "Tag:";
			// 
			// ArgumentsTextBox
			// 
			this.ArgumentsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.ArgumentsTextBox.Location = new System.Drawing.Point(116, 63);
			this.ArgumentsTextBox.Name = "ArgumentsTextBox";
			this.ArgumentsTextBox.Size = new System.Drawing.Size(191, 20);
			this.ArgumentsTextBox.TabIndex = 5;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(3, 66);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(72, 16);
			this.label4.TabIndex = 11;
			this.label4.Text = "Arguments:";
			// 
			// CancelButton1
			// 
			this.CancelButton1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.CancelButton1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelButton1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.CancelButton1.Location = new System.Drawing.Point(328, 164);
			this.CancelButton1.Name = "CancelButton1";
			this.CancelButton1.Size = new System.Drawing.Size(75, 23);
			this.CancelButton1.TabIndex = 13;
			this.CancelButton1.Text = "Cancel";
			// 
			// ArgumentsButton
			// 
			this.ArgumentsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ArgumentsButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ArgumentsButton.Location = new System.Drawing.Point(338, 63);
			this.ArgumentsButton.Name = "ArgumentsButton";
			this.ArgumentsButton.Size = new System.Drawing.Size(24, 23);
			this.ArgumentsButton.TabIndex = 7;
			this.ArgumentsButton.Text = "...";
			this.toolTip.SetToolTip(this.ArgumentsButton, "Click here to select more than one argument from Custom Controls at a time");
			this.ArgumentsButton.Click += new System.EventHandler(this.ArgumentsButton_Click);
			// 
			// TagCheckBox
			// 
			this.TagCheckBox.Checked = true;
			this.TagCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.TagCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.TagCheckBox.Location = new System.Drawing.Point(6, 14);
			this.TagCheckBox.Name = "TagCheckBox";
			this.TagCheckBox.Size = new System.Drawing.Size(141, 23);
			this.TagCheckBox.TabIndex = 10;
			this.TagCheckBox.Text = "Generate Tag from Name";
			this.TagCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.toolTip.SetToolTip(this.TagCheckBox, "Place a check here to automatically generate the Tag from the Button Name");
			this.TagCheckBox.CheckedChanged += new System.EventHandler(this.TagCheckBox_CheckedChanged);
			// 
			// ArgumentsMenuButton
			// 
			this.ArgumentsMenuButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ArgumentsMenuButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ArgumentsMenuButton.Location = new System.Drawing.Point(313, 63);
			this.ArgumentsMenuButton.Name = "ArgumentsMenuButton";
			this.ArgumentsMenuButton.Size = new System.Drawing.Size(24, 23);
			this.ArgumentsMenuButton.TabIndex = 6;
			this.ArgumentsMenuButton.Text = ">";
			this.toolTip.SetToolTip(this.ArgumentsMenuButton, "Select an argument from a Custom Control");
			this.ArgumentsMenuButton.Click += new System.EventHandler(this.ArgumentsMenuButton_Click);
			// 
			// ToolTipTextBox
			// 
			this.ToolTipTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.ToolTipTextBox.Location = new System.Drawing.Point(90, 32);
			this.ToolTipTextBox.Name = "ToolTipTextBox";
			this.ToolTipTextBox.Size = new System.Drawing.Size(546, 20);
			this.ToolTipTextBox.TabIndex = 11;
			this.toolTip.SetToolTip(this.ToolTipTextBox, "Add ToolTip for other MOG users to see");
			// 
			// ClearButton
			// 
			this.ClearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ClearButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ClearButton.Location = new System.Drawing.Point(363, 63);
			this.ClearButton.Name = "ClearButton";
			this.ClearButton.Size = new System.Drawing.Size(49, 23);
			this.ClearButton.TabIndex = 8;
			this.ClearButton.Text = "Clear";
			this.toolTip.SetToolTip(this.ClearButton, "Clear all arguments...");
			this.ClearButton.Click += new System.EventHandler(this.ClearButton_Click);
			// 
			// CommandArgumentsButton
			// 
			this.CommandArgumentsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.CommandArgumentsButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.CommandArgumentsButton.Location = new System.Drawing.Point(313, 35);
			this.CommandArgumentsButton.Name = "CommandArgumentsButton";
			this.CommandArgumentsButton.Size = new System.Drawing.Size(24, 23);
			this.CommandArgumentsButton.TabIndex = 3;
			this.CommandArgumentsButton.Text = ">";
			this.toolTip.SetToolTip(this.CommandArgumentsButton, "Select an argument from a Custom Control");
			this.CommandArgumentsButton.Click += new System.EventHandler(this.CommandArgumentsButton_Click);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(28, 35);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(56, 16);
			this.label5.TabIndex = 16;
			this.label5.Text = "ToolTip:";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.TagCheckBox);
			this.groupBox1.Controls.Add(this.TagTextBox);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Location = new System.Drawing.Point(12, 58);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(200, 100);
			this.groupBox1.TabIndex = 17;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Tag Options";
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.CommandTextBox);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.ClearButton);
			this.groupBox2.Controls.Add(this.CommandArgumentsButton);
			this.groupBox2.Controls.Add(this.BrowseButton);
			this.groupBox2.Controls.Add(this.ArgumentsTextBox);
			this.groupBox2.Controls.Add(this.HideOutputCheckBox);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.ArgumentsMenuButton);
			this.groupBox2.Controls.Add(this.ArgumentsButton);
			this.groupBox2.Location = new System.Drawing.Point(218, 58);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(418, 100);
			this.groupBox2.TabIndex = 18;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Button Options";
			// 
			// EditParameterButtonForm
			// 
			this.AcceptButton = this.OkButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.CancelButton1;
			this.ClientSize = new System.Drawing.Size(642, 194);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.ToolTipTextBox);
			this.Controls.Add(this.NameTextBox);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.CancelButton1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.OkButton);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MaximumSize = new System.Drawing.Size(4000, 320);
			this.MinimumSize = new System.Drawing.Size(650, 228);
			this.Name = "EditParameterButtonForm";
			this.Text = "Create/Edit Parameter Button";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private void Ok_Click(object sender, System.EventArgs e)
		{
			string caption = "Missing Information";
			string message = "";
			bool displayMessage = false;
			// Make sure we have a Name
			if(NameTextBox.Text.Length == 0)
			{
				message += "Button Name cannot be empty\n";
				displayMessage |= true;
			}

			// Make sure we have a Command
			if(CommandTextBox.Text.Length == 0)
			{
				message += "Tool Path cannot be empty\n";
				displayMessage |= true;
			}

			// Make sure we have a Tag
			if( TagTextBox.Text.Length == 0 )
			{
				message += "Tag cannot be empty\n";
				displayMessage |= true;
			}

			// If anything is wrong...
			if( displayMessage )
			{
				MessageBox.Show( this, message, caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
				return;
			}

			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void NameTextBox_TextChanged(object sender, System.EventArgs e)
		{
			// If we have a check to generate the Tag name, we will generate it
			if( this.TagCheckBox.Checked )
				// Get rid of spaces and asign text appropriately
				this.TagTextBox.Text = ((TextBox)sender).Text.Replace(" ", "");
		}

		private void ArgumentsButton_Click(object sender, System.EventArgs e)
		{
			ToolBoxArgumentsBrowserForm browser = new ToolBoxArgumentsBrowserForm( this.mToolBox );
			if( browser.ShowDialog( this ) == DialogResult.OK )
			{
				// If we have nothing for Text, set an empty string
				if( this.ArgumentsTextBox.Text == null )
					this.ArgumentsTextBox.Text = "";

				//Figure out what the user clicked here.
				foreach( ListViewItem item in browser.TagsListView.SelectedItems )
				{
					this.ArgumentsTextBox.Text += "<" + item.Text + ">";
				}
			}
		}

		/// <summary>
		/// Open the Arguments Menu
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ArgumentsMenuButton_Click(object sender, System.EventArgs e)
		{
			ContextMenu cm = this.mToolBox.GenerateTagContextMenu( new EventHandler( this.tagMenuItem_Click ) );
			Button button = (Button)sender;
			cm.Show( button, new Point( button.Width, 0 ) );
		}

		/// <summary>
		/// Adds the tag to our Arguments TextBox
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tagMenuItem_Click(object sender, EventArgs e)
		{
			string tag = ((MenuItem)sender).Text;
			this.ArgumentsTextBox.Text += tag.Substring( tag.IndexOf("<"), tag.IndexOf(">") + (">").Length);
		}

		private void BrowseButton_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			if( MOG_ControllerProject.GetCurrentSyncDataController() != null )
				ofd.InitialDirectory = MOG_ControllerProject.GetCurrentSyncDataController().GetSyncDirectory();

			if(ofd.ShowDialog() == DialogResult.OK)
			{
				CommandTextBox.Text = ofd.FileName;
			}
		}

		private void ClearButton_Click(object sender, System.EventArgs e)
		{
			this.ArgumentsTextBox.Text = "";
		}

		private void CommandArgumentsButton_Click(object sender, System.EventArgs e)
		{
			ContextMenu cm = this.mToolBox.GenerateTagContextMenu( new EventHandler( this.commandMenuItem_Click ) );
			Button button = (Button)sender;
			cm.Show( button, new Point( button.Width, 0 ) );
		}

		private void commandMenuItem_Click( object sender, EventArgs e )
		{
			string tag = ((MenuItem)sender).Text;
			this.CommandTextBox.Text += tag.Substring( tag.IndexOf("<"), tag.IndexOf(">") + (">").Length);
		}

		private void TagCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			TagTextBox.ReadOnly = TagCheckBox.Checked;
		}
	}
}
