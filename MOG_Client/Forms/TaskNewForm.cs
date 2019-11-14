using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using MOG;
using MOG.USER;
using MOG.FILENAME;
using MOG.CONTROLLER;
using MOG.REPORT;
using MOG.TIME;

namespace MOG_Client
{
	/// <summary>
	/// Summary description for FormTaskNew.
	/// </summary>
	public class TaskNewForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox tbxTitle;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public int originalWidth;
		private System.Windows.Forms.RichTextBox TaskNewDescriptionRichTextBox;
		private System.Windows.Forms.DateTimePicker TaskNewDueDateDateTimePicker;
		private System.Windows.Forms.Button TaskNewCancelButton;
		private System.Windows.Forms.Button TaskNewOkButton;
		private System.Windows.Forms.ListBox TaskNewUsersListBox;
		public System.Windows.Forms.ListBox TaskNewFlowListBox;
		private System.Windows.Forms.ComboBox TaskNewFlowPresetsComboBox;
		private System.Windows.Forms.Button TaskNewUpButton;
		private System.Windows.Forms.Button TaskNewDownButton;
		private System.Windows.Forms.Button TaskNewSaveButton;
		private System.Windows.Forms.ComboBox TaskNewCategoryComboBox;
		private System.Windows.Forms.ComboBox TaskNewPriorityComboBox;
		private System.Windows.Forms.Panel TaskNewAdvancedOptionsPanel;
		private System.Windows.Forms.Button TaskNewMoreButton;
		private System.Windows.Forms.Button TaskNewRemoveButton;
		public System.Windows.Forms.TextBox TaskNewAssetTextBox;
		public BASE lMog;

		public TaskNewForm(BASE Mog)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			originalWidth = this.Width;
			lMog = Mog;
			
			InitializeUsers();
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

		private void InitializeUsers()
		{
			for (int x = 0; x < lMog.GetProject().GetUsers().Count; x++)
			{
				MOG_User user = (MOG_User)lMog.GetProject().GetUsers()[x];

				this.TaskNewUsersListBox.Items.Add(user.GetUserName());
			}
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(TaskNewForm));
			this.tbxTitle = new System.Windows.Forms.TextBox();
			this.TaskNewDescriptionRichTextBox = new System.Windows.Forms.RichTextBox();
			this.TaskNewDueDateDateTimePicker = new System.Windows.Forms.DateTimePicker();
			this.TaskNewCancelButton = new System.Windows.Forms.Button();
			this.TaskNewOkButton = new System.Windows.Forms.Button();
			this.TaskNewUsersListBox = new System.Windows.Forms.ListBox();
			this.TaskNewFlowListBox = new System.Windows.Forms.ListBox();
			this.TaskNewFlowPresetsComboBox = new System.Windows.Forms.ComboBox();
			this.TaskNewUpButton = new System.Windows.Forms.Button();
			this.TaskNewDownButton = new System.Windows.Forms.Button();
			this.TaskNewSaveButton = new System.Windows.Forms.Button();
			this.TaskNewCategoryComboBox = new System.Windows.Forms.ComboBox();
			this.TaskNewAssetTextBox = new System.Windows.Forms.TextBox();
			this.TaskNewPriorityComboBox = new System.Windows.Forms.ComboBox();
			this.TaskNewAdvancedOptionsPanel = new System.Windows.Forms.Panel();
			this.TaskNewMoreButton = new System.Windows.Forms.Button();
			this.TaskNewRemoveButton = new System.Windows.Forms.Button();
			this.TaskNewAdvancedOptionsPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// tbxTitle
			// 
			this.tbxTitle.Location = new System.Drawing.Point(8, 9);
			this.tbxTitle.Name = "tbxTitle";
			this.tbxTitle.Size = new System.Drawing.Size(352, 20);
			this.tbxTitle.TabIndex = 2;
			this.tbxTitle.Text = "Default Title";
			// 
			// TaskNewDescriptionRichTextBox
			// 
			this.TaskNewDescriptionRichTextBox.Location = new System.Drawing.Point(8, 57);
			this.TaskNewDescriptionRichTextBox.Name = "TaskNewDescriptionRichTextBox";
			this.TaskNewDescriptionRichTextBox.Size = new System.Drawing.Size(352, 88);
			this.TaskNewDescriptionRichTextBox.TabIndex = 3;
			this.TaskNewDescriptionRichTextBox.Text = "Description";
			// 
			// TaskNewDueDateDateTimePicker
			// 
			this.TaskNewDueDateDateTimePicker.CustomFormat = "MMM dd, yy - ddd - hh:MM t";
			this.TaskNewDueDateDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.TaskNewDueDateDateTimePicker.Location = new System.Drawing.Point(8, 33);
			this.TaskNewDueDateDateTimePicker.Name = "TaskNewDueDateDateTimePicker";
			this.TaskNewDueDateDateTimePicker.Size = new System.Drawing.Size(160, 20);
			this.TaskNewDueDateDateTimePicker.TabIndex = 4;
			// 
			// TaskNewCancelButton
			// 
			this.TaskNewCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.TaskNewCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.TaskNewCancelButton.Location = new System.Drawing.Point(400, 184);
			this.TaskNewCancelButton.Name = "TaskNewCancelButton";
			this.TaskNewCancelButton.Size = new System.Drawing.Size(72, 23);
			this.TaskNewCancelButton.TabIndex = 6;
			this.TaskNewCancelButton.Text = "Cancel";
			// 
			// TaskNewOkButton
			// 
			this.TaskNewOkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.TaskNewOkButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.TaskNewOkButton.Location = new System.Drawing.Point(320, 184);
			this.TaskNewOkButton.Name = "TaskNewOkButton";
			this.TaskNewOkButton.Size = new System.Drawing.Size(72, 23);
			this.TaskNewOkButton.TabIndex = 7;
			this.TaskNewOkButton.Text = "Ok";
			// 
			// TaskNewUsersListBox
			// 
			this.TaskNewUsersListBox.Location = new System.Drawing.Point(160, 16);
			this.TaskNewUsersListBox.Name = "TaskNewUsersListBox";
			this.TaskNewUsersListBox.Size = new System.Drawing.Size(88, 134);
			this.TaskNewUsersListBox.Sorted = true;
			this.TaskNewUsersListBox.TabIndex = 8;
			this.TaskNewUsersListBox.DoubleClick += new System.EventHandler(this.FormTaskNew_DoubleClick);
			// 
			// TaskNewFlowListBox
			// 
			this.TaskNewFlowListBox.Location = new System.Drawing.Point(8, 40);
			this.TaskNewFlowListBox.Name = "TaskNewFlowListBox";
			this.TaskNewFlowListBox.Size = new System.Drawing.Size(88, 108);
			this.TaskNewFlowListBox.TabIndex = 9;
			// 
			// TaskNewFlowPresetsComboBox
			// 
			this.TaskNewFlowPresetsComboBox.Location = new System.Drawing.Point(8, 8);
			this.TaskNewFlowPresetsComboBox.Name = "TaskNewFlowPresetsComboBox";
			this.TaskNewFlowPresetsComboBox.Size = new System.Drawing.Size(88, 21);
			this.TaskNewFlowPresetsComboBox.TabIndex = 10;
			this.TaskNewFlowPresetsComboBox.Text = "Default";
			// 
			// TaskNewUpButton
			// 
			this.TaskNewUpButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.TaskNewUpButton.Location = new System.Drawing.Point(120, 24);
			this.TaskNewUpButton.Name = "TaskNewUpButton";
			this.TaskNewUpButton.Size = new System.Drawing.Size(32, 23);
			this.TaskNewUpButton.TabIndex = 11;
			this.TaskNewUpButton.Text = "Up";
			this.TaskNewUpButton.Click += new System.EventHandler(this.btnUp_Click);
			// 
			// TaskNewDownButton
			// 
			this.TaskNewDownButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.TaskNewDownButton.Location = new System.Drawing.Point(120, 48);
			this.TaskNewDownButton.Name = "TaskNewDownButton";
			this.TaskNewDownButton.Size = new System.Drawing.Size(32, 23);
			this.TaskNewDownButton.TabIndex = 12;
			this.TaskNewDownButton.Text = "Dn";
			this.TaskNewDownButton.Click += new System.EventHandler(this.btnDown_Click);
			// 
			// TaskNewSaveButton
			// 
			this.TaskNewSaveButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.TaskNewSaveButton.Location = new System.Drawing.Point(120, 120);
			this.TaskNewSaveButton.Name = "TaskNewSaveButton";
			this.TaskNewSaveButton.Size = new System.Drawing.Size(32, 23);
			this.TaskNewSaveButton.TabIndex = 13;
			this.TaskNewSaveButton.Text = "save";
			// 
			// TaskNewCategoryComboBox
			// 
			this.TaskNewCategoryComboBox.Location = new System.Drawing.Point(176, 33);
			this.TaskNewCategoryComboBox.Name = "TaskNewCategoryComboBox";
			this.TaskNewCategoryComboBox.Size = new System.Drawing.Size(88, 21);
			this.TaskNewCategoryComboBox.TabIndex = 16;
			this.TaskNewCategoryComboBox.Text = "None";
			// 
			// TaskNewAssetTextBox
			// 
			this.TaskNewAssetTextBox.Location = new System.Drawing.Point(8, 154);
			this.TaskNewAssetTextBox.Name = "TaskNewAssetTextBox";
			this.TaskNewAssetTextBox.Size = new System.Drawing.Size(352, 20);
			this.TaskNewAssetTextBox.TabIndex = 15;
			this.TaskNewAssetTextBox.Text = "None";
			// 
			// TaskNewPriorityComboBox
			// 
			this.TaskNewPriorityComboBox.Items.AddRange(new object[] {
																		 "Priority_None",
																		 "Priority_Low",
																		 "Priority_Medium",
																		 "Priority_High",
																		 "Priority_Urgent"});
			this.TaskNewPriorityComboBox.Location = new System.Drawing.Point(272, 33);
			this.TaskNewPriorityComboBox.Name = "TaskNewPriorityComboBox";
			this.TaskNewPriorityComboBox.Size = new System.Drawing.Size(88, 21);
			this.TaskNewPriorityComboBox.TabIndex = 14;
			this.TaskNewPriorityComboBox.Text = "Priority_None";
			// 
			// TaskNewAdvancedOptionsPanel
			// 
			this.TaskNewAdvancedOptionsPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.TaskNewAdvancedOptionsPanel.Controls.AddRange(new System.Windows.Forms.Control[] {
																									  this.TaskNewMoreButton,
																									  this.TaskNewRemoveButton,
																									  this.TaskNewFlowListBox,
																									  this.TaskNewFlowPresetsComboBox,
																									  this.TaskNewUsersListBox,
																									  this.TaskNewDownButton,
																									  this.TaskNewUpButton,
																									  this.TaskNewSaveButton});
			this.TaskNewAdvancedOptionsPanel.Location = new System.Drawing.Point(368, 8);
			this.TaskNewAdvancedOptionsPanel.Name = "TaskNewAdvancedOptionsPanel";
			this.TaskNewAdvancedOptionsPanel.Size = new System.Drawing.Size(256, 168);
			this.TaskNewAdvancedOptionsPanel.TabIndex = 17;
			// 
			// TaskNewMoreButton
			// 
			this.TaskNewMoreButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.TaskNewMoreButton.Location = new System.Drawing.Point(99, 89);
			this.TaskNewMoreButton.Name = "TaskNewMoreButton";
			this.TaskNewMoreButton.Size = new System.Drawing.Size(10, 23);
			this.TaskNewMoreButton.TabIndex = 15;
			this.TaskNewMoreButton.Text = ">";
			this.TaskNewMoreButton.Click += new System.EventHandler(this.button1_Click);
			// 
			// TaskNewRemoveButton
			// 
			this.TaskNewRemoveButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.TaskNewRemoveButton.Location = new System.Drawing.Point(120, 80);
			this.TaskNewRemoveButton.Name = "TaskNewRemoveButton";
			this.TaskNewRemoveButton.Size = new System.Drawing.Size(32, 23);
			this.TaskNewRemoveButton.TabIndex = 14;
			this.TaskNewRemoveButton.Text = "Del";
			this.TaskNewRemoveButton.Click += new System.EventHandler(this.btnRemove_Click);
			// 
			// TaskNewForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(480, 216);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.TaskNewAdvancedOptionsPanel,
																		  this.TaskNewCategoryComboBox,
																		  this.TaskNewAssetTextBox,
																		  this.TaskNewPriorityComboBox,
																		  this.TaskNewOkButton,
																		  this.TaskNewCancelButton,
																		  this.TaskNewDueDateDateTimePicker,
																		  this.TaskNewDescriptionRichTextBox,
																		  this.tbxTitle});
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "TaskNewForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "FormTaskNew";
			this.DoubleClick += new System.EventHandler(this.FormTaskNew_DoubleClick);
			this.TaskNewAdvancedOptionsPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void FormTaskNew_DoubleClick(object sender, System.EventArgs e)
		{
			TaskNewFlowListBox.Items.Add(((ListBox)sender).SelectedItem.ToString());
		}

		private void btnRemove_Click(object sender, System.EventArgs e)
		{
			if (TaskNewFlowListBox.SelectedIndex != -1)
			{
				TaskNewFlowListBox.Items.Remove(TaskNewFlowListBox.SelectedItem);
			}
		}

		private void btnUp_Click(object sender, System.EventArgs e)
		{
			if (TaskNewFlowListBox.SelectedIndex != -1)
			{
				// Get the current item
				Object item = TaskNewFlowListBox.SelectedItem;
				int index = TaskNewFlowListBox.SelectedIndex;

				// Make sure we arn't already at the top of the list
				if (index > 0)
				{
					// Copy us up one position
					TaskNewFlowListBox.Items.Insert(index - 1, item);

					// Kill our old position
					TaskNewFlowListBox.Items.RemoveAt(index + 1);

					// Set new position as the active node
					TaskNewFlowListBox.SelectedIndex = index -1;
				}
			}
		}

		private void btnDown_Click(object sender, System.EventArgs e)
		{
			if (TaskNewFlowListBox.SelectedIndex != -1)
			{
				// Get the current item
				Object item = TaskNewFlowListBox.SelectedItem;
				int index = TaskNewFlowListBox.SelectedIndex;

				// Make sure we are not already at the bottom
				if (index < TaskNewFlowListBox.Items.Count - 1)
				{
					// Move us down two
					TaskNewFlowListBox.Items.Insert(index + 2, item);
					// Remove our old spot
					TaskNewFlowListBox.Items.RemoveAt(index);

					// Make the moved item the selected one
					TaskNewFlowListBox.SelectedIndex = index+1;
				}
			}
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			if (this.Width < (this.TaskNewAdvancedOptionsPanel.Left + TaskNewAdvancedOptionsPanel.Width + 3))
				this.Width = (this.TaskNewAdvancedOptionsPanel.Left + TaskNewAdvancedOptionsPanel.Width + 3);
			else
				this.Width = originalWidth;

		}
	}
}
