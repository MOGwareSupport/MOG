using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using MOG;

namespace MOG_Client.Forms.ToolBoxForms
{
	/// <summary>
	/// Summary description for ToolBoxRadioButtonForm.
	/// </summary>
	public class ToolBoxRadioButtonForm : System.Windows.Forms.Form
	{
		private string mArguments;
		public string Arguments
		{
			get
			{
				if( mArguments != null )
					return mArguments;
				else
					return "";
			}
			set
			{
				this.mArguments = value;
			}
		}
		private int mLastSelectedIndex;
		public int LastSelectedIndex
		{
			get
			{
				return mLastSelectedIndex;
			}
			set
			{
				mLastSelectedIndex = value;
			}
		}
		private ToolBox mToolBox;
		private System.Windows.Forms.Label label1;
		public System.Windows.Forms.TextBox NameTextBox;
		private System.Windows.Forms.Label label2;
		public System.Windows.Forms.TextBox TagTextBox;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		public System.Windows.Forms.TextBox RadioArgumentTextBox;
		private System.Windows.Forms.Button RadioAddButton;
		private System.Windows.Forms.Button RadioDeleteButton;
		private System.Windows.Forms.Button RadioClearAllButton;
		private System.Windows.Forms.Button OKButton;
		private System.Windows.Forms.Button CancelButton1;
		private System.Windows.Forms.CheckBox TagCheckBox;
		private MOG_Client.Forms.ToolBoxForms.ToolBoxGroupBox RadioGroupBox;
		private System.Windows.Forms.Button UpdateButton;
		private System.Windows.Forms.Label label5;
		public System.Windows.Forms.TextBox RadioNameTextBox;
		private System.Windows.Forms.Label label6;
		public System.Windows.Forms.TextBox ToolTipTextBox;
		private System.Windows.Forms.Button ArgumentsMenuButton;
		private System.Windows.Forms.Button ArgumentBrowseButton;
		private System.Windows.Forms.Button ArgumentsClearButton;
		private System.Windows.Forms.Button ArgumentsContextMenuButton;
		private System.Windows.Forms.Button ArgumentsListButton;
		private System.Windows.Forms.Button MoveUpButton;
		private System.Windows.Forms.Button MoveDownButton;
		private GroupBox groupBox1;
		private GroupBox groupBox2;
		private System.ComponentModel.IContainer components;

		/// <summary>
		/// Gets SortedList of RadioButtons created in this form.
		/// </summary>
		[Description("Sets the RadioButtons to be displayed by this Form (set this at runtime).")]
		public SortedList RadioButtons
		{
			get
			{
				return this.RadioGroupBox.RadioButtons;
			}
			set
			{
				this.RadioGroupBox.RadioButtons = value;
			}
		}

		/// <summary>
		/// Initialize RadioButton Form without pre-defined RadioButtons.  
		/// 
		/// Use InitializeRadioButtons() to pass in a list of RadioButton values.
		/// </summary>
		public ToolBoxRadioButtonForm( ToolBox parentToolBox )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.mToolBox = parentToolBox;
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
		/// Initialize form with a SortedList of radio buttons.
		/// </summary>
		/// <param name="radioButtons">Radio button values organized with Key being the name
		/// and Value being the argument (both are strings).</param>
		public ToolBoxRadioButtonForm( ToolBox parentToolBox, SortedList radioButtons )
			:this( parentToolBox )
		{
			this.RadioGroupBox.RadioButtons = radioButtons;
		}

		/// <summary>
		/// If default constructor is used, use this method to initialize radio buttons from
		/// a SortedList
		/// </summary>
		/// <param name="radioButtons">SortedList of RadioButtons where Key is the button
		/// name and Value is the argument (both are strings)</param>
		public void InitializeRadioButtons( SortedList radioButtons )
		{
			this.RadioGroupBox.RadioButtons = radioButtons;
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ToolBoxRadioButtonForm));
			this.label1 = new System.Windows.Forms.Label();
			this.NameTextBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.TagCheckBox = new System.Windows.Forms.CheckBox();
			this.TagTextBox = new System.Windows.Forms.TextBox();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.RadioNameTextBox = new System.Windows.Forms.TextBox();
			this.RadioArgumentTextBox = new System.Windows.Forms.TextBox();
			this.RadioAddButton = new System.Windows.Forms.Button();
			this.RadioDeleteButton = new System.Windows.Forms.Button();
			this.RadioClearAllButton = new System.Windows.Forms.Button();
			this.UpdateButton = new System.Windows.Forms.Button();
			this.label6 = new System.Windows.Forms.Label();
			this.ArgumentsMenuButton = new System.Windows.Forms.Button();
			this.ArgumentBrowseButton = new System.Windows.Forms.Button();
			this.ArgumentsClearButton = new System.Windows.Forms.Button();
			this.ArgumentsContextMenuButton = new System.Windows.Forms.Button();
			this.ArgumentsListButton = new System.Windows.Forms.Button();
			this.MoveUpButton = new System.Windows.Forms.Button();
			this.MoveDownButton = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.OKButton = new System.Windows.Forms.Button();
			this.CancelButton1 = new System.Windows.Forms.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.ToolTipTextBox = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.RadioGroupBox = new MOG_Client.Forms.ToolBoxForms.ToolBoxGroupBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(48, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Name:";
			// 
			// NameTextBox
			// 
			this.NameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.NameTextBox.Location = new System.Drawing.Point(56, 8);
			this.NameTextBox.Name = "NameTextBox";
			this.NameTextBox.Size = new System.Drawing.Size(674, 20);
			this.NameTextBox.TabIndex = 0;
			this.toolTip.SetToolTip(this.NameTextBox, "Enter the name of this Radio Button Group here");
			this.NameTextBox.TextChanged += new System.EventHandler(this.NameTextBox_TextChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(3, 45);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(35, 16);
			this.label2.TabIndex = 2;
			this.label2.Text = "Tag:";
			// 
			// TagCheckBox
			// 
			this.TagCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.TagCheckBox.Checked = true;
			this.TagCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.TagCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.TagCheckBox.Location = new System.Drawing.Point(6, 19);
			this.TagCheckBox.Name = "TagCheckBox";
			this.TagCheckBox.Size = new System.Drawing.Size(176, 24);
			this.TagCheckBox.TabIndex = 1;
			this.TagCheckBox.Text = "Auto Create Tag from Name:";
			this.TagCheckBox.CheckedChanged += new System.EventHandler(this.TagCheckBox_CheckedChanged);
			// 
			// TagTextBox
			// 
			this.TagTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.TagTextBox.Location = new System.Drawing.Point(36, 42);
			this.TagTextBox.Name = "TagTextBox";
			this.TagTextBox.Size = new System.Drawing.Size(154, 20);
			this.TagTextBox.TabIndex = 2;
			this.toolTip.SetToolTip(this.TagTextBox, "This is the tag that can be used between arrow brackets (<>) to point to this Gro" +
					"up\'s currently selected Radio Button argument");
			// 
			// RadioNameTextBox
			// 
			this.RadioNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.RadioNameTextBox.Location = new System.Drawing.Point(142, 19);
			this.RadioNameTextBox.Name = "RadioNameTextBox";
			this.RadioNameTextBox.Size = new System.Drawing.Size(348, 20);
			this.RadioNameTextBox.TabIndex = 0;
			this.toolTip.SetToolTip(this.RadioNameTextBox, "Enter the Display Name for the Radio Button");
			// 
			// RadioArgumentTextBox
			// 
			this.RadioArgumentTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.RadioArgumentTextBox.Location = new System.Drawing.Point(142, 42);
			this.RadioArgumentTextBox.Name = "RadioArgumentTextBox";
			this.RadioArgumentTextBox.Size = new System.Drawing.Size(273, 20);
			this.RadioArgumentTextBox.TabIndex = 1;
			this.toolTip.SetToolTip(this.RadioArgumentTextBox, "Optional: Add arguments for your Radio Button here (e.g. \"editor -r -u -watching=" +
					"true\")");
			// 
			// RadioAddButton
			// 
			this.RadioAddButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.RadioAddButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.RadioAddButton.Location = new System.Drawing.Point(434, 71);
			this.RadioAddButton.Name = "RadioAddButton";
			this.RadioAddButton.Size = new System.Drawing.Size(75, 23);
			this.RadioAddButton.TabIndex = 9;
			this.RadioAddButton.Text = "Add";
			this.toolTip.SetToolTip(this.RadioAddButton, "Click here to Add the Radio Button you have entered above (Tags must be unique)");
			this.RadioAddButton.Click += new System.EventHandler(this.RadioAddButton_Click);
			// 
			// RadioDeleteButton
			// 
			this.RadioDeleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.RadioDeleteButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.RadioDeleteButton.Location = new System.Drawing.Point(282, 71);
			this.RadioDeleteButton.Name = "RadioDeleteButton";
			this.RadioDeleteButton.Size = new System.Drawing.Size(75, 23);
			this.RadioDeleteButton.TabIndex = 7;
			this.RadioDeleteButton.Text = "Delete";
			this.toolTip.SetToolTip(this.RadioDeleteButton, "Click here to Delete an existing Radio Button");
			this.RadioDeleteButton.Click += new System.EventHandler(this.RadioDeleteButton_Click);
			// 
			// RadioClearAllButton
			// 
			this.RadioClearAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.RadioClearAllButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.RadioClearAllButton.Location = new System.Drawing.Point(206, 71);
			this.RadioClearAllButton.Name = "RadioClearAllButton";
			this.RadioClearAllButton.Size = new System.Drawing.Size(75, 23);
			this.RadioClearAllButton.TabIndex = 6;
			this.RadioClearAllButton.Text = "Clear All";
			this.toolTip.SetToolTip(this.RadioClearAllButton, "Click here to delete all Controls");
			this.RadioClearAllButton.Click += new System.EventHandler(this.RadioClearAllButton_Click);
			// 
			// UpdateButton
			// 
			this.UpdateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.UpdateButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.UpdateButton.Location = new System.Drawing.Point(358, 71);
			this.UpdateButton.Name = "UpdateButton";
			this.UpdateButton.Size = new System.Drawing.Size(75, 23);
			this.UpdateButton.TabIndex = 8;
			this.UpdateButton.Text = "Update";
			this.toolTip.SetToolTip(this.UpdateButton, "Click here to update an existing Radio Button");
			this.UpdateButton.Click += new System.EventHandler(this.UpdateButton_Click);
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(8, 34);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(48, 16);
			this.label6.TabIndex = 17;
			this.label6.Text = "ToolTip:";
			this.toolTip.SetToolTip(this.label6, "Tool");
			// 
			// ArgumentsMenuButton
			// 
			this.ArgumentsMenuButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ArgumentsMenuButton.Image = ((System.Drawing.Image)(resources.GetObject("ArgumentsMenuButton.Image")));
			this.ArgumentsMenuButton.Location = new System.Drawing.Point(0, 0);
			this.ArgumentsMenuButton.Name = "ArgumentsMenuButton";
			this.ArgumentsMenuButton.Size = new System.Drawing.Size(24, 23);
			this.ArgumentsMenuButton.TabIndex = 16;
			this.toolTip.SetToolTip(this.ArgumentsMenuButton, "Select Toolbox Control arguments from a menu");
			// 
			// ArgumentBrowseButton
			// 
			this.ArgumentBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ArgumentBrowseButton.Location = new System.Drawing.Point(32, 0);
			this.ArgumentBrowseButton.Name = "ArgumentBrowseButton";
			this.ArgumentBrowseButton.Size = new System.Drawing.Size(24, 23);
			this.ArgumentBrowseButton.TabIndex = 7;
			this.ArgumentBrowseButton.Text = "...";
			this.toolTip.SetToolTip(this.ArgumentBrowseButton, "Add tags from the MOG Toolbox...");
			// 
			// ArgumentsClearButton
			// 
			this.ArgumentsClearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ArgumentsClearButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ArgumentsClearButton.Location = new System.Drawing.Point(470, 40);
			this.ArgumentsClearButton.Name = "ArgumentsClearButton";
			this.ArgumentsClearButton.Size = new System.Drawing.Size(39, 23);
			this.ArgumentsClearButton.TabIndex = 4;
			this.ArgumentsClearButton.Text = "Clear";
			this.toolTip.SetToolTip(this.ArgumentsClearButton, "Clear all arguments...");
			this.ArgumentsClearButton.Click += new System.EventHandler(this.ArgumentsClearButton_Click);
			// 
			// ArgumentsContextMenuButton
			// 
			this.ArgumentsContextMenuButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ArgumentsContextMenuButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ArgumentsContextMenuButton.Image = ((System.Drawing.Image)(resources.GetObject("ArgumentsContextMenuButton.Image")));
			this.ArgumentsContextMenuButton.Location = new System.Drawing.Point(420, 40);
			this.ArgumentsContextMenuButton.Name = "ArgumentsContextMenuButton";
			this.ArgumentsContextMenuButton.Size = new System.Drawing.Size(24, 23);
			this.ArgumentsContextMenuButton.TabIndex = 2;
			this.ArgumentsContextMenuButton.Text = ">";
			this.toolTip.SetToolTip(this.ArgumentsContextMenuButton, "Select tags from a menu...");
			this.ArgumentsContextMenuButton.Click += new System.EventHandler(this.ArgumentsContextMenuButton_Click);
			// 
			// ArgumentsListButton
			// 
			this.ArgumentsListButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ArgumentsListButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ArgumentsListButton.Location = new System.Drawing.Point(445, 40);
			this.ArgumentsListButton.Name = "ArgumentsListButton";
			this.ArgumentsListButton.Size = new System.Drawing.Size(24, 23);
			this.ArgumentsListButton.TabIndex = 3;
			this.ArgumentsListButton.Text = "...";
			this.toolTip.SetToolTip(this.ArgumentsListButton, "Add multiple tags from the MOG Toolbox...");
			this.ArgumentsListButton.Click += new System.EventHandler(this.ArgumentsListButton_Click);
			// 
			// MoveUpButton
			// 
			this.MoveUpButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.MoveUpButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.MoveUpButton.Location = new System.Drawing.Point(706, 68);
			this.MoveUpButton.Name = "MoveUpButton";
			this.MoveUpButton.Size = new System.Drawing.Size(24, 24);
			this.MoveUpButton.TabIndex = 18;
			this.MoveUpButton.Text = "+";
			this.toolTip.SetToolTip(this.MoveUpButton, "Move selected Radio Button up/left");
			this.MoveUpButton.Click += new System.EventHandler(this.MoveUpButton_Click);
			// 
			// MoveDownButton
			// 
			this.MoveDownButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.MoveDownButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.MoveDownButton.Location = new System.Drawing.Point(706, 98);
			this.MoveDownButton.Name = "MoveDownButton";
			this.MoveDownButton.Size = new System.Drawing.Size(24, 24);
			this.MoveDownButton.TabIndex = 19;
			this.MoveDownButton.Text = "-";
			this.toolTip.SetToolTip(this.MoveDownButton, "Move selected Radio Button down/right");
			this.MoveDownButton.Click += new System.EventHandler(this.MoveDownButton_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(9, 23);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(128, 16);
			this.label3.TabIndex = 6;
			this.label3.Text = "Radio Button Name:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(6, 42);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(132, 16);
			this.label4.TabIndex = 7;
			this.label4.Text = "Radio Button Argument:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// OKButton
			// 
			this.OKButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.OKButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.OKButton.Location = new System.Drawing.Point(288, 257);
			this.OKButton.Name = "OKButton";
			this.OKButton.Size = new System.Drawing.Size(75, 23);
			this.OKButton.TabIndex = 10;
			this.OKButton.Text = "OK";
			this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
			// 
			// CancelButton1
			// 
			this.CancelButton1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.CancelButton1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelButton1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.CancelButton1.Location = new System.Drawing.Point(376, 257);
			this.CancelButton1.Name = "CancelButton1";
			this.CancelButton1.Size = new System.Drawing.Size(75, 23);
			this.CancelButton1.TabIndex = 11;
			this.CancelButton1.Text = "Cancel";
			this.CancelButton1.Click += new System.EventHandler(this.CancelButton_Click);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(18, 22);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(100, 16);
			this.label5.TabIndex = 6;
			this.label5.Text = "Radio Button Tag:";
			// 
			// ToolTipTextBox
			// 
			this.ToolTipTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.ToolTipTextBox.Location = new System.Drawing.Point(56, 34);
			this.ToolTipTextBox.Name = "ToolTipTextBox";
			this.ToolTipTextBox.Size = new System.Drawing.Size(674, 20);
			this.ToolTipTextBox.TabIndex = 3;
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox1.Controls.Add(this.TagCheckBox);
			this.groupBox1.Controls.Add(this.TagTextBox);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Location = new System.Drawing.Point(11, 149);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(200, 100);
			this.groupBox1.TabIndex = 20;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Tag Options";
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.RadioNameTextBox);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.RadioArgumentTextBox);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.UpdateButton);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.RadioClearAllButton);
			this.groupBox2.Controls.Add(this.RadioAddButton);
			this.groupBox2.Controls.Add(this.RadioDeleteButton);
			this.groupBox2.Controls.Add(this.ArgumentsClearButton);
			this.groupBox2.Controls.Add(this.ArgumentsListButton);
			this.groupBox2.Controls.Add(this.ArgumentsContextMenuButton);
			this.groupBox2.Location = new System.Drawing.Point(219, 149);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(515, 100);
			this.groupBox2.TabIndex = 21;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Create Radio Button";
			// 
			// RadioGroupBox
			// 
			this.RadioGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.RadioGroupBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.RadioGroupBox.Location = new System.Drawing.Point(11, 60);
			this.RadioGroupBox.Name = "RadioGroupBox";
			this.RadioGroupBox.RadioButtons = ((System.Collections.SortedList)(resources.GetObject("RadioGroupBox.RadioButtons")));
			this.RadioGroupBox.SelectedButton = null;
			this.RadioGroupBox.Size = new System.Drawing.Size(692, 81);
			this.RadioGroupBox.TabIndex = 4;
			this.RadioGroupBox.TabStop = false;
			this.RadioGroupBox.Text = "Radio Button Group";
			this.toolTip.SetToolTip(this.RadioGroupBox, "Use the fields below to add Radio Buttons.  Click on any Radio Button to edit it " +
					"below.");
			this.RadioGroupBox.SelectionChanged += new MOG_Client.Forms.ToolBoxForms.ToolBoxGroupBox.SelectionChangedHandler(this.RadioGroupBox_SelectionChanged);
			// 
			// ToolBoxRadioButtonForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(738, 286);
			this.Controls.Add(this.MoveUpButton);
			this.Controls.Add(this.MoveDownButton);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.ToolTipTextBox);
			this.Controls.Add(this.RadioGroupBox);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.CancelButton1);
			this.Controls.Add(this.OKButton);
			this.Controls.Add(this.NameTextBox);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(746, 320);
			this.Name = "ToolBoxRadioButtonForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Create/Edit Radio Button Group";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		#region Events
		/// <summary>
		/// If TagCheckBox.Checked, updates TagTextBox.Text to value of NameTextBox.Text
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void NameTextBox_TextChanged(object sender, System.EventArgs e)
		{
			if( TagCheckBox.Checked )
				TagTextBox.Text = this.NameTextBox.Text;
		}

		/// <summary>
		/// Cancel this dialog
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CancelButton_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		/// <summary>
		/// Clears all RadioButtons in ToolBoxGroupBox
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RadioClearAllButton_Click(object sender, System.EventArgs e)
		{
			this.RadioGroupBox.Controls.Clear();
		}

		/// <summary>
		/// Add a RadioButton, if it has a valid name.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RadioAddButton_Click(object sender, System.EventArgs e)
		{
			string name = this.RadioNameTextBox.Text;
			string arg = this.RadioArgumentTextBox.Text;
			if( name.Length < 1 )
				MessageBox.Show( this, "Please fill in the 'Radio Button Name' field.", "Name Missing!",
					MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
			else
			{
				RadioButton newButton = new RadioButton();
				newButton.Text = name;
				newButton.Tag = arg;
				this.RadioGroupBox.Controls.Add( newButton );
				newButton.Checked = true;
			}
		}

		/// <summary>
		/// Updates the TagTextBox for this GroupBox dynamically, if TagCheckBox.Checked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TagCheckBox_CheckedChanged(object sender, System.EventArgs e)
		{
			if( this.TagCheckBox.Checked )
				TagTextBox.Text = this.NameTextBox.Text;
		}

		/// <summary>
		/// Catches event fired off by ToolBoxGroupBox whenever the RadioButton selected changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RadioGroupBox_SelectionChanged(object sender, System.EventArgs e)
		{
			this.RadioNameTextBox.Text = this.RadioGroupBox.SelectedButton.Text;
			this.RadioArgumentTextBox.Text = this.RadioGroupBox.SelectedButton.Tag.ToString();
			this.mLastSelectedIndex = this.RadioGroupBox.Controls.GetChildIndex( this.RadioGroupBox.SelectedButton, false );
		}

		/// <summary>
		/// Update the selected RadioButton in ToolBoxGroupBox
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void UpdateButton_Click(object sender, System.EventArgs e)
		{
			if( this.RadioNameTextBox != null && this.RadioArgumentTextBox != null
				&& this.RadioNameTextBox.Text != null && this.RadioArgumentTextBox.Text != null
				&& this.RadioGroupBox.SelectedButton != null )
			{
				this.RadioGroupBox.SelectedButton.Text = this.RadioNameTextBox.Text;
				this.RadioGroupBox.SelectedButton.Tag = this.RadioArgumentTextBox.Text;
			}
		}

		/// <summary>
		/// Delete the selected RadioButton in ToolBoxGroupBox
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RadioDeleteButton_Click(object sender, System.EventArgs e)
		{		
			this.RadioGroupBox.Controls.Remove( this.RadioGroupBox.SelectedButton );
		}

		/// <summary>
		/// Check our input before we exit this Dialog
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OKButton_Click(object sender, System.EventArgs e)
		{
			string message = "";
			bool error = false;
			// If Name is blank...
			if( this.NameTextBox.Text.Length < 1 )
			{
				message += "Please enter a Name for this Radio Group Box.\r\n";
				error |= true;
			}
			// If Tag is blank...
			if( this .TagTextBox.Text.Length < 1 )
			{
				message += "Please enter a Tag for this Radio Group Box.\r\n";
				error |= true;
			}
			// If we have no RadioButtons
			if( this.RadioGroupBox.Controls.Count < 1 )
			{
				message += "Please add at least one Radio Button.\r\n";
				error |= true;
			}

			// If we had an error...
			if( error )
			{
				MessageBox.Show( this, message, "Missing Information", 
					MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
				return;
			}

			// Set our arguments
			this.mArguments = this.RadioGroupBox.SelectedButton.Tag.ToString();
			// Close the form
			this.DialogResult = DialogResult.OK;
			this.Close();
		}
		#endregion Events

		private void ArgumentsClearButton_Click(object sender, System.EventArgs e)
		{
			this.RadioArgumentTextBox.Text = "";
		}

		private void ArgumentsContextMenuButton_Click(object sender, System.EventArgs e)
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
			this.RadioArgumentTextBox.Text += tag.Substring( tag.IndexOf("<"), tag.IndexOf(">") + (">").Length);
		}

		private void ArgumentsListButton_Click(object sender, System.EventArgs e)
		{
			ToolBoxArgumentsBrowserForm browser = new ToolBoxArgumentsBrowserForm( this.mToolBox );
			if( browser.ShowDialog( this ) == DialogResult.OK )
			{
				// If we have nothing for Text, set an empty string
				if( this.RadioArgumentTextBox.Text == null )
					this.RadioArgumentTextBox.Text = "";

				//Figure out what the user clicked here.
				foreach( ListViewItem item in browser.TagsListView.SelectedItems )
				{
					this.RadioArgumentTextBox.Text += "<" + item.Text + ">";
				}
			}
		}

		private void MoveUpButton_Click(object sender, System.EventArgs e)
		{
			// If we actually have a selected button
			if( this.RadioGroupBox.SelectedButton != null )
			{
				// True: We want to move radio button up
				MoveRadioButton( true );
			}
		}

		private void MoveDownButton_Click(object sender, System.EventArgs e)
		{	
			// If we actually have a selected button
			if( this.RadioGroupBox.SelectedButton != null )
			{
				// False: We want to move radio button down
				MoveRadioButton( false );
			}
		}

		private void MoveRadioButton( bool moveUp )
		{
			// Get our current index
			int currentIndex = 
				this.RadioGroupBox.Controls.GetChildIndex( this.RadioGroupBox.SelectedButton, false );

			// Initialize and set our new index
			int newIndex = 0;
			if( moveUp )
			{
				// If we are moving up, do not go below 0
				newIndex = ( (newIndex-1) > 0 ) ? (newIndex-1) : newIndex;
			}
			else
			{
				// If we are moving down, do not go above Count
				newIndex = ( (newIndex+1) < this.RadioGroupBox.Controls.Count) ? (newIndex+1) : newIndex;
			}

			// Move our child by setting its index
			this.RadioGroupBox.Controls.SetChildIndex( this.RadioGroupBox.SelectedButton, newIndex );
		}

	}
}
