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
	/// Summary description for EditCustomButtonForm.
	/// </summary>
	public class SimpleButtonForm : System.Windows.Forms.Form
	{
		private ToolBox mToolBox;

		public System.Windows.Forms.TextBox EditButtonNameTextBox;
		public System.Windows.Forms.Button EditBrowseButton;
		private System.Windows.Forms.Button EditCancelButton;
		private System.Windows.Forms.Button EditOkButton;
		private System.Windows.Forms.OpenFileDialog EditOpenFileDialog;
		public System.Windows.Forms.TextBox EditToolExeTextBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button ArgumentBrowseButton;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.Label label3;
		public System.Windows.Forms.TextBox ArgumentsTextBox;
		private System.Windows.Forms.Label label4;
		public System.Windows.Forms.TextBox ToolTipTextBox;
		private System.Windows.Forms.Button ArgumentsMenuButton;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button CommandArgumentsMenuButton;
		private GroupBox groupBox1;
		private System.ComponentModel.IContainer components;

		protected SimpleButtonForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}
		/// <summary>
		/// Initialize a Button
		/// </summary>
		/// <param name="toolBox"></param>
		public SimpleButtonForm( ToolBox toolBox )
		{
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
				if (control.Name != "EditOkButton" && control.Name != "EditCancelButton")
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SimpleButtonForm));
			this.EditButtonNameTextBox = new System.Windows.Forms.TextBox();
			this.EditToolExeTextBox = new System.Windows.Forms.TextBox();
			this.EditBrowseButton = new System.Windows.Forms.Button();
			this.EditCancelButton = new System.Windows.Forms.Button();
			this.EditOkButton = new System.Windows.Forms.Button();
			this.EditOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.ArgumentBrowseButton = new System.Windows.Forms.Button();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.ArgumentsTextBox = new System.Windows.Forms.TextBox();
			this.ArgumentsMenuButton = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.CommandArgumentsMenuButton = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.ToolTipTextBox = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// EditButtonNameTextBox
			// 
			this.EditButtonNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.EditButtonNameTextBox.Location = new System.Drawing.Point(88, 8);
			this.EditButtonNameTextBox.Name = "EditButtonNameTextBox";
			this.EditButtonNameTextBox.Size = new System.Drawing.Size(411, 20);
			this.EditButtonNameTextBox.TabIndex = 0;
			this.EditButtonNameTextBox.Text = "Button name";
			this.toolTip.SetToolTip(this.EditButtonNameTextBox, "The name by which you would like to refer to this button");
			// 
			// EditToolExeTextBox
			// 
			this.EditToolExeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.EditToolExeTextBox.Location = new System.Drawing.Point(86, 13);
			this.EditToolExeTextBox.Name = "EditToolExeTextBox";
			this.EditToolExeTextBox.Size = new System.Drawing.Size(282, 20);
			this.EditToolExeTextBox.TabIndex = 1;
			this.EditToolExeTextBox.Text = "Tool executable";
			this.toolTip.SetToolTip(this.EditToolExeTextBox, "Fill in the filename of the executable or the tag for a MOG Toolbox item");
			// 
			// EditBrowseButton
			// 
			this.EditBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.EditBrowseButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.EditBrowseButton.Location = new System.Drawing.Point(424, 10);
			this.EditBrowseButton.Name = "EditBrowseButton";
			this.EditBrowseButton.Size = new System.Drawing.Size(56, 23);
			this.EditBrowseButton.TabIndex = 3;
			this.EditBrowseButton.Text = "Browse";
			this.toolTip.SetToolTip(this.EditBrowseButton, "Browse to the file you would like to execute");
			this.EditBrowseButton.Click += new System.EventHandler(this.EditBrowseButton_Click);
			// 
			// EditCancelButton
			// 
			this.EditCancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.EditCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.EditCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.EditCancelButton.Location = new System.Drawing.Point(173, 135);
			this.EditCancelButton.Name = "EditCancelButton";
			this.EditCancelButton.Size = new System.Drawing.Size(75, 23);
			this.EditCancelButton.TabIndex = 9;
			this.EditCancelButton.Text = "Cancel";
			// 
			// EditOkButton
			// 
			this.EditOkButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.EditOkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.EditOkButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.EditOkButton.Location = new System.Drawing.Point(261, 135);
			this.EditOkButton.Name = "EditOkButton";
			this.EditOkButton.Size = new System.Drawing.Size(75, 23);
			this.EditOkButton.TabIndex = 10;
			this.EditOkButton.Text = "OK";
			// 
			// EditOpenFileDialog
			// 
			this.EditOpenFileDialog.DefaultExt = "exe";
			this.EditOpenFileDialog.Filter = "Executables|*.exe|Batch Files|*.bat|All Files|*.*";
			this.EditOpenFileDialog.Title = "Browse for tool";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 16);
			this.label1.TabIndex = 5;
			this.label1.Text = "Button Name:";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(6, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(74, 16);
			this.label2.TabIndex = 6;
			this.label2.Text = "Tool Path:";
			// 
			// ArgumentBrowseButton
			// 
			this.ArgumentBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ArgumentBrowseButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ArgumentBrowseButton.Location = new System.Drawing.Point(399, 37);
			this.ArgumentBrowseButton.Name = "ArgumentBrowseButton";
			this.ArgumentBrowseButton.Size = new System.Drawing.Size(24, 23);
			this.ArgumentBrowseButton.TabIndex = 6;
			this.ArgumentBrowseButton.Text = "...";
			this.toolTip.SetToolTip(this.ArgumentBrowseButton, "Add tags from the MOG Toolbox...");
			this.ArgumentBrowseButton.Click += new System.EventHandler(this.ArgumentBrowseButton_Click);
			// 
			// ArgumentsTextBox
			// 
			this.ArgumentsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.ArgumentsTextBox.Location = new System.Drawing.Point(86, 39);
			this.ArgumentsTextBox.Name = "ArgumentsTextBox";
			this.ArgumentsTextBox.Size = new System.Drawing.Size(282, 20);
			this.ArgumentsTextBox.TabIndex = 4;
			this.toolTip.SetToolTip(this.ArgumentsTextBox, "Fill in the filename of the executable or the tag for a MOG Toolbox item");
			// 
			// ArgumentsMenuButton
			// 
			this.ArgumentsMenuButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ArgumentsMenuButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ArgumentsMenuButton.Image = ((System.Drawing.Image)(resources.GetObject("ArgumentsMenuButton.Image")));
			this.ArgumentsMenuButton.Location = new System.Drawing.Point(374, 37);
			this.ArgumentsMenuButton.Name = "ArgumentsMenuButton";
			this.ArgumentsMenuButton.Size = new System.Drawing.Size(24, 23);
			this.ArgumentsMenuButton.TabIndex = 5;
			this.ArgumentsMenuButton.Text = ">";
			this.toolTip.SetToolTip(this.ArgumentsMenuButton, "Select Toolbox Control arguments from a menu");
			this.ArgumentsMenuButton.Click += new System.EventHandler(this.ArgumentsMenuButton_Click);
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button1.Location = new System.Drawing.Point(424, 37);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(56, 23);
			this.button1.TabIndex = 7;
			this.button1.Text = "Clear";
			this.toolTip.SetToolTip(this.button1, "Clear all arguments...");
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// CommandArgumentsMenuButton
			// 
			this.CommandArgumentsMenuButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.CommandArgumentsMenuButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.CommandArgumentsMenuButton.Image = ((System.Drawing.Image)(resources.GetObject("CommandArgumentsMenuButton.Image")));
			this.CommandArgumentsMenuButton.Location = new System.Drawing.Point(399, 10);
			this.CommandArgumentsMenuButton.Name = "CommandArgumentsMenuButton";
			this.CommandArgumentsMenuButton.Size = new System.Drawing.Size(24, 23);
			this.CommandArgumentsMenuButton.TabIndex = 2;
			this.CommandArgumentsMenuButton.Text = ">";
			this.toolTip.SetToolTip(this.CommandArgumentsMenuButton, "Select Toolbox Control arguments to execute from");
			this.CommandArgumentsMenuButton.Click += new System.EventHandler(this.CommandArgumentsMenuButton_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(6, 42);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(74, 16);
			this.label3.TabIndex = 8;
			this.label3.Text = "Argument(s):";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(8, 37);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(61, 16);
			this.label4.TabIndex = 10;
			this.label4.Text = "Tool Tip:";
			// 
			// ToolTipTextBox
			// 
			this.ToolTipTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.ToolTipTextBox.Location = new System.Drawing.Point(88, 34);
			this.ToolTipTextBox.Name = "ToolTipTextBox";
			this.ToolTipTextBox.Size = new System.Drawing.Size(409, 20);
			this.ToolTipTextBox.TabIndex = 8;
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.CommandArgumentsMenuButton);
			this.groupBox1.Controls.Add(this.EditToolExeTextBox);
			this.groupBox1.Controls.Add(this.button1);
			this.groupBox1.Controls.Add(this.EditBrowseButton);
			this.groupBox1.Controls.Add(this.ArgumentsMenuButton);
			this.groupBox1.Controls.Add(this.ArgumentBrowseButton);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.ArgumentsTextBox);
			this.groupBox1.Location = new System.Drawing.Point(11, 60);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(486, 71);
			this.groupBox1.TabIndex = 11;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Button Options";
			// 
			// SimpleButtonForm
			// 
			this.AcceptButton = this.EditOkButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.EditCancelButton;
			this.ClientSize = new System.Drawing.Size(509, 159);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.ToolTipTextBox);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.EditOkButton);
			this.Controls.Add(this.EditCancelButton);
			this.Controls.Add(this.EditButtonNameTextBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(2000, 191);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(329, 191);
			this.Name = "SimpleButtonForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Create/Edit Button";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private void EditBrowseButton_Click(object sender, System.EventArgs e)
		{
			// If we have Gamedata (which we always should, set our directory
			if( MOG_ControllerProject.GetCurrentSyncDataController() != null )
			{
				EditOpenFileDialog.InitialDirectory = MOG_ControllerProject.GetCurrentSyncDataController().GetSyncDirectory();
			}

			// If user wants to accept the file change, reflect change in our form
			if (EditOpenFileDialog.ShowDialog() == DialogResult.OK)
			{
				EditToolExeTextBox.Text = EditOpenFileDialog.FileName;
			}
		}

		private void ArgumentBrowseButton_Click(object sender, System.EventArgs e)
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

		private void button1_Click(object sender, System.EventArgs e)
		{
			this.ArgumentsTextBox.Text = "";
		}

		private void CommandArgumentsMenuButton_Click(object sender, System.EventArgs e)
		{
			ContextMenu cm = this.mToolBox.GenerateTagContextMenu( new EventHandler( this.commandMenuItem_Click ));
			Button button = (Button)sender;
			cm.Show( button, new Point( button.Width, 0 ) );
		}

		private void commandMenuItem_Click( object sender, EventArgs e )
		{
			string tag = ((MenuItem)sender).Text;
			this.EditToolExeTextBox.Text += tag.Substring( tag.IndexOf("<"), tag.IndexOf(">") + (">").Length);
		}
	}
}
