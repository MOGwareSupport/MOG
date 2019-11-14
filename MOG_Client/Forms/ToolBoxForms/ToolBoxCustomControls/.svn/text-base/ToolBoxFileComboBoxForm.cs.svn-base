using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG;

namespace MOG_Client.Forms.ToolBoxForms
{
	/// <summary>
	/// Summary description for ToolBoxFolderComboBoxForm.
	/// </summary>
	public class ToolBoxFileComboBoxForm : System.Windows.Forms.Form
	{
		private const string LeaveBlank_Text = "Leave this blank, if you would like to add files, later on...";
		private string mSelectedPath;
		public System.Windows.Forms.TextBox ToolTipTextBox;
		private ComboBox mDefComboBox;
		public ComboBox DefComboBox
		{
			get
			{
				if( mDefComboBox != null )
					return mDefComboBox;
				else
					return new ComboBox();
			}
			set
			{
				this.mDefComboBox = value;
			}
		}
		public int ComboBoxDepth;
	
		public string SelectedPath
		{
			get
			{
				return this.mSelectedPath;
			}
			set
			{
				this.mSelectedPath = value;
			}
		}

		public bool TagFullPaths
		{
			get { return FullFilenamesRadioButton.Checked; }
			set { FullFilenamesRadioButton.Checked = value; }
		}
		
		public bool TagWorkspaceRelativePaths
		{
			get { return WorkspaceRelativeFilenamesRadioButton.Checked; }
			set { WorkspaceRelativeFilenamesRadioButton.Checked = value; }
		}
		
		public bool TagFolderRelativePaths
		{
			get { return FolderRelativeFilenamesRadioButton.Checked; }
			set { FolderRelativeFilenamesRadioButton.Checked = value; }
		}
		
		public bool TagBasePaths
		{
			get { return BaseFilenamesRadioButton.Checked; }
			set { BaseFilenamesRadioButton.Checked = value; }
		}

		public bool ShowFullPaths
		{
			get { return ShowFullFilenamesRadioButton.Checked; }
			set { ShowFullFilenamesRadioButton.Checked = value; }
		}

		public bool ShowWorkspaceRelativePaths
		{
			get { return ShowWorkspaceRelativeFilenamesRadioButton.Checked; }
			set { ShowWorkspaceRelativeFilenamesRadioButton.Checked = value; }
		}

		public bool ShowFolderRelativePaths
		{
			get { return ShowFolderRelativeFilenamesRadioButton.Checked; }
			set { ShowFolderRelativeFilenamesRadioButton.Checked = value; }
		}

		public bool ShowBasePaths
		{
			get { return ShowBaseFilenamesRadioButton.Checked; }
			set { ShowBaseFilenamesRadioButton.Checked = value; }
		}

		public bool RecurseFolder
		{
			get { return RecurseFoldersCheckBox.Checked; }
			set { RecurseFoldersCheckBox.Checked = value; }
		}
	
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.CheckBox TagCheckBox;
		public System.Windows.Forms.TextBox NameTextBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button ToolBoxIniComboCancelButton;
		private System.Windows.Forms.Button ToolBoxIniComboOKButton;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button PathBrowseButton;
		public System.Windows.Forms.TextBox PathTextBox;
		public System.Windows.Forms.TextBox TagTextBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label5;
		public System.Windows.Forms.TextBox DepthTextBox;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.Label label6;
		public System.Windows.Forms.TextBox PatternTextBox;
		private System.ComponentModel.IContainer components;
		private GroupBox groupBox1;
		private GroupBox groupBox2;
		private GroupBox groupBox3;
		private RadioButton BaseFilenamesRadioButton;
		private RadioButton WorkspaceRelativeFilenamesRadioButton;
		private RadioButton FolderRelativeFilenamesRadioButton;
		private RadioButton FullFilenamesRadioButton;
		private CheckBox RecurseFoldersCheckBox;
		private GroupBox groupBox4;
		private RadioButton ShowFullFilenamesRadioButton;
		private RadioButton ShowBaseFilenamesRadioButton;
		private RadioButton ShowWorkspaceRelativeFilenamesRadioButton;
		private RadioButton ShowFolderRelativeFilenamesRadioButton;

		private ToolBox mToolBox;

		public ToolBoxFileComboBoxForm(ToolBox toolBox)
		{
			this.mToolBox = toolBox;
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.DepthTextBox.Text = ControlDefinition.Default_ComboBox_Depth.ToString();
			this.PathTextBox.Text = LeaveBlank_Text;
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
				if (control.Name != "ToolBoxIniComboOKButton" && control.Name != "ToolBoxIniComboCancelButton")
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ToolBoxFileComboBoxForm));
			this.ToolTipTextBox = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.TagCheckBox = new System.Windows.Forms.CheckBox();
			this.NameTextBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.ToolBoxIniComboCancelButton = new System.Windows.Forms.Button();
			this.ToolBoxIniComboOKButton = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.PathBrowseButton = new System.Windows.Forms.Button();
			this.PathTextBox = new System.Windows.Forms.TextBox();
			this.TagTextBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.DepthTextBox = new System.Windows.Forms.TextBox();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.WorkspaceRelativeFilenamesRadioButton = new System.Windows.Forms.RadioButton();
			this.FolderRelativeFilenamesRadioButton = new System.Windows.Forms.RadioButton();
			this.BaseFilenamesRadioButton = new System.Windows.Forms.RadioButton();
			this.FullFilenamesRadioButton = new System.Windows.Forms.RadioButton();
			this.ShowFullFilenamesRadioButton = new System.Windows.Forms.RadioButton();
			this.ShowBaseFilenamesRadioButton = new System.Windows.Forms.RadioButton();
			this.ShowWorkspaceRelativeFilenamesRadioButton = new System.Windows.Forms.RadioButton();
			this.ShowFolderRelativeFilenamesRadioButton = new System.Windows.Forms.RadioButton();
			this.label6 = new System.Windows.Forms.Label();
			this.PatternTextBox = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.RecurseFoldersCheckBox = new System.Windows.Forms.CheckBox();
			this.groupBox1.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.SuspendLayout();
			// 
			// ToolTipTextBox
			// 
			this.ToolTipTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.ToolTipTextBox.Location = new System.Drawing.Point(64, 80);
			this.ToolTipTextBox.Name = "ToolTipTextBox";
			this.ToolTipTextBox.Size = new System.Drawing.Size(368, 20);
			this.ToolTipTextBox.TabIndex = 5;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(8, 83);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(56, 16);
			this.label4.TabIndex = 33;
			this.label4.Text = "ToolTip:";
			// 
			// TagCheckBox
			// 
			this.TagCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.TagCheckBox.Checked = true;
			this.TagCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.TagCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.TagCheckBox.Location = new System.Drawing.Point(6, 19);
			this.TagCheckBox.Name = "TagCheckBox";
			this.TagCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.TagCheckBox.Size = new System.Drawing.Size(169, 16);
			this.TagCheckBox.TabIndex = 3;
			this.TagCheckBox.Text = "Auto Create Tag from Name:";
			this.TagCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.TagCheckBox.CheckedChanged += new System.EventHandler(this.TagCheckBox_CheckedChanged);
			// 
			// NameTextBox
			// 
			this.NameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.NameTextBox.Location = new System.Drawing.Point(64, 54);
			this.NameTextBox.Name = "NameTextBox";
			this.NameTextBox.Size = new System.Drawing.Size(368, 20);
			this.NameTextBox.TabIndex = 2;
			this.toolTip.SetToolTip(this.NameTextBox, "The name of the Combo Box (to be displayed");
			this.NameTextBox.TextChanged += new System.EventHandler(this.NameTextBox_TextChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 57);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(48, 16);
			this.label1.TabIndex = 29;
			this.label1.Text = "Name:";
			// 
			// ToolBoxIniComboCancelButton
			// 
			this.ToolBoxIniComboCancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.ToolBoxIniComboCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.ToolBoxIniComboCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.ToolBoxIniComboCancelButton.Location = new System.Drawing.Point(231, 295);
			this.ToolBoxIniComboCancelButton.Name = "ToolBoxIniComboCancelButton";
			this.ToolBoxIniComboCancelButton.Size = new System.Drawing.Size(83, 23);
			this.ToolBoxIniComboCancelButton.TabIndex = 9;
			this.ToolBoxIniComboCancelButton.Text = "Cancel";
			this.ToolBoxIniComboCancelButton.Click += new System.EventHandler(this.ToolBoxIniComboCancelButton_Click);
			// 
			// ToolBoxIniComboOKButton
			// 
			this.ToolBoxIniComboOKButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.ToolBoxIniComboOKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.ToolBoxIniComboOKButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.ToolBoxIniComboOKButton.Location = new System.Drawing.Point(135, 295);
			this.ToolBoxIniComboOKButton.Name = "ToolBoxIniComboOKButton";
			this.ToolBoxIniComboOKButton.Size = new System.Drawing.Size(83, 23);
			this.ToolBoxIniComboOKButton.TabIndex = 8;
			this.ToolBoxIniComboOKButton.Text = "OK";
			this.ToolBoxIniComboOKButton.Click += new System.EventHandler(this.ToolBoxIniComboOKButton_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 8);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(168, 16);
			this.label3.TabIndex = 23;
			this.label3.Text = "Use Folder Path Only (Optional):";
			// 
			// PathBrowseButton
			// 
			this.PathBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.PathBrowseButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PathBrowseButton.Location = new System.Drawing.Point(352, 26);
			this.PathBrowseButton.Name = "PathBrowseButton";
			this.PathBrowseButton.Size = new System.Drawing.Size(80, 23);
			this.PathBrowseButton.TabIndex = 1;
			this.PathBrowseButton.Text = "Browse Folder";
			this.toolTip.SetToolTip(this.PathBrowseButton, "If you select a Folder for this File ComboBox, you will be unable to add other fi" +
					"les later...");
			this.PathBrowseButton.Click += new System.EventHandler(this.PathBrowseButton_Click);
			// 
			// PathTextBox
			// 
			this.PathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.PathTextBox.Location = new System.Drawing.Point(8, 28);
			this.PathTextBox.Name = "PathTextBox";
			this.PathTextBox.Size = new System.Drawing.Size(336, 20);
			this.PathTextBox.TabIndex = 0;
			this.PathTextBox.Text = "Leave this blank, if you would like to add files, later on...";
			this.toolTip.SetToolTip(this.PathTextBox, "Leave BLANK if you would like to add files later");
			// 
			// TagTextBox
			// 
			this.TagTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.TagTextBox.Location = new System.Drawing.Point(50, 41);
			this.TagTextBox.Name = "TagTextBox";
			this.TagTextBox.Size = new System.Drawing.Size(146, 20);
			this.TagTextBox.TabIndex = 4;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(2, 45);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(48, 16);
			this.label2.TabIndex = 28;
			this.label2.Text = "Tag:";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(6, 64);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(172, 17);
			this.label5.TabIndex = 35;
			this.label5.Text = "Depth to Remember (1 or more):";
			// 
			// DepthTextBox
			// 
			this.DepthTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.DepthTextBox.Location = new System.Drawing.Point(176, 61);
			this.DepthTextBox.Name = "DepthTextBox";
			this.DepthTextBox.Size = new System.Drawing.Size(30, 20);
			this.DepthTextBox.TabIndex = 6;
			this.toolTip.SetToolTip(this.DepthTextBox, "The number of files you would like to keep in this Combo Box");
			// 
			// WorkspaceRelativeFilenamesRadioButton
			// 
			this.WorkspaceRelativeFilenamesRadioButton.AutoSize = true;
			this.WorkspaceRelativeFilenamesRadioButton.Location = new System.Drawing.Point(6, 31);
			this.WorkspaceRelativeFilenamesRadioButton.Name = "WorkspaceRelativeFilenamesRadioButton";
			this.WorkspaceRelativeFilenamesRadioButton.Size = new System.Drawing.Size(169, 17);
			this.WorkspaceRelativeFilenamesRadioButton.TabIndex = 0;
			this.WorkspaceRelativeFilenamesRadioButton.Text = "Workspace Relative filenames";
			this.toolTip.SetToolTip(this.WorkspaceRelativeFilenamesRadioButton, "ProjectSubFolder\\MyFolder\\MySubFolder\\MyFile.ext");
			this.WorkspaceRelativeFilenamesRadioButton.UseVisualStyleBackColor = true;
			// 
			// FolderRelativeFilenamesRadioButton
			// 
			this.FolderRelativeFilenamesRadioButton.AutoSize = true;
			this.FolderRelativeFilenamesRadioButton.Location = new System.Drawing.Point(6, 48);
			this.FolderRelativeFilenamesRadioButton.Name = "FolderRelativeFilenamesRadioButton";
			this.FolderRelativeFilenamesRadioButton.Size = new System.Drawing.Size(143, 17);
			this.FolderRelativeFilenamesRadioButton.TabIndex = 0;
			this.FolderRelativeFilenamesRadioButton.Text = "Folder Relative filenames";
			this.toolTip.SetToolTip(this.FolderRelativeFilenamesRadioButton, "MySubFolder\\MyFile.ext");
			this.FolderRelativeFilenamesRadioButton.UseVisualStyleBackColor = true;
			// 
			// BaseFilenamesRadioButton
			// 
			this.BaseFilenamesRadioButton.AutoSize = true;
			this.BaseFilenamesRadioButton.Location = new System.Drawing.Point(6, 65);
			this.BaseFilenamesRadioButton.Name = "BaseFilenamesRadioButton";
			this.BaseFilenamesRadioButton.Size = new System.Drawing.Size(96, 17);
			this.BaseFilenamesRadioButton.TabIndex = 1;
			this.BaseFilenamesRadioButton.Text = "Base filenames";
			this.toolTip.SetToolTip(this.BaseFilenamesRadioButton, "MyFile.ext");
			this.BaseFilenamesRadioButton.UseVisualStyleBackColor = true;
			// 
			// FullFilenamesRadioButton
			// 
			this.FullFilenamesRadioButton.AutoSize = true;
			this.FullFilenamesRadioButton.Checked = true;
			this.FullFilenamesRadioButton.Location = new System.Drawing.Point(6, 14);
			this.FullFilenamesRadioButton.Name = "FullFilenamesRadioButton";
			this.FullFilenamesRadioButton.Size = new System.Drawing.Size(88, 17);
			this.FullFilenamesRadioButton.TabIndex = 2;
			this.FullFilenamesRadioButton.TabStop = true;
			this.FullFilenamesRadioButton.Text = "Full filenames";
			this.toolTip.SetToolTip(this.FullFilenamesRadioButton, "C:\\MyProject\\ProjectSubFolder\\MyFolder\\MySubFolder\\MyFile.ext");
			this.FullFilenamesRadioButton.UseVisualStyleBackColor = true;
			// 
			// ShowFullFilenamesRadioButton
			// 
			this.ShowFullFilenamesRadioButton.AutoSize = true;
			this.ShowFullFilenamesRadioButton.Checked = true;
			this.ShowFullFilenamesRadioButton.Location = new System.Drawing.Point(6, 14);
			this.ShowFullFilenamesRadioButton.Name = "ShowFullFilenamesRadioButton";
			this.ShowFullFilenamesRadioButton.Size = new System.Drawing.Size(88, 17);
			this.ShowFullFilenamesRadioButton.TabIndex = 2;
			this.ShowFullFilenamesRadioButton.TabStop = true;
			this.ShowFullFilenamesRadioButton.Text = "Full filenames";
			this.toolTip.SetToolTip(this.ShowFullFilenamesRadioButton, "C:\\MyProject\\ProjectSubFolder\\MyFolder\\MySubFolder\\MyFile.ext");
			this.ShowFullFilenamesRadioButton.UseVisualStyleBackColor = true;
			// 
			// ShowBaseFilenamesRadioButton
			// 
			this.ShowBaseFilenamesRadioButton.AutoSize = true;
			this.ShowBaseFilenamesRadioButton.Location = new System.Drawing.Point(6, 65);
			this.ShowBaseFilenamesRadioButton.Name = "ShowBaseFilenamesRadioButton";
			this.ShowBaseFilenamesRadioButton.Size = new System.Drawing.Size(96, 17);
			this.ShowBaseFilenamesRadioButton.TabIndex = 1;
			this.ShowBaseFilenamesRadioButton.Text = "Base filenames";
			this.toolTip.SetToolTip(this.ShowBaseFilenamesRadioButton, "MyFile.ext");
			this.ShowBaseFilenamesRadioButton.UseVisualStyleBackColor = true;
			// 
			// ShowWorkspaceRelativeFilenamesRadioButton
			// 
			this.ShowWorkspaceRelativeFilenamesRadioButton.AutoSize = true;
			this.ShowWorkspaceRelativeFilenamesRadioButton.Location = new System.Drawing.Point(6, 31);
			this.ShowWorkspaceRelativeFilenamesRadioButton.Name = "ShowWorkspaceRelativeFilenamesRadioButton";
			this.ShowWorkspaceRelativeFilenamesRadioButton.Size = new System.Drawing.Size(169, 17);
			this.ShowWorkspaceRelativeFilenamesRadioButton.TabIndex = 0;
			this.ShowWorkspaceRelativeFilenamesRadioButton.Text = "Workspace Relative filenames";
			this.toolTip.SetToolTip(this.ShowWorkspaceRelativeFilenamesRadioButton, "ProjectSubFolder\\MyFolder\\MySubFolder\\MyFile.ext");
			this.ShowWorkspaceRelativeFilenamesRadioButton.UseVisualStyleBackColor = true;
			// 
			// ShowFolderRelativeFilenamesRadioButton
			// 
			this.ShowFolderRelativeFilenamesRadioButton.AutoSize = true;
			this.ShowFolderRelativeFilenamesRadioButton.Location = new System.Drawing.Point(6, 48);
			this.ShowFolderRelativeFilenamesRadioButton.Name = "ShowFolderRelativeFilenamesRadioButton";
			this.ShowFolderRelativeFilenamesRadioButton.Size = new System.Drawing.Size(143, 17);
			this.ShowFolderRelativeFilenamesRadioButton.TabIndex = 0;
			this.ShowFolderRelativeFilenamesRadioButton.Text = "Folder Relative filenames";
			this.toolTip.SetToolTip(this.ShowFolderRelativeFilenamesRadioButton, "MySubFolder\\MyFile.ext");
			this.ShowFolderRelativeFilenamesRadioButton.UseVisualStyleBackColor = true;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(6, 44);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(78, 20);
			this.label6.TabIndex = 37;
			this.label6.Text = "File Pattern:";
			// 
			// PatternTextBox
			// 
			this.PatternTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.PatternTextBox.Location = new System.Drawing.Point(72, 41);
			this.PatternTextBox.Name = "PatternTextBox";
			this.PatternTextBox.Size = new System.Drawing.Size(134, 20);
			this.PatternTextBox.TabIndex = 7;
			this.PatternTextBox.Text = "*.*";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.groupBox3);
			this.groupBox1.Controls.Add(this.TagCheckBox);
			this.groupBox1.Controls.Add(this.TagTextBox);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Location = new System.Drawing.Point(12, 106);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(202, 181);
			this.groupBox1.TabIndex = 39;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Tag Options";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.FullFilenamesRadioButton);
			this.groupBox3.Controls.Add(this.WorkspaceRelativeFilenamesRadioButton);
			this.groupBox3.Controls.Add(this.FolderRelativeFilenamesRadioButton);
			this.groupBox3.Controls.Add(this.BaseFilenamesRadioButton);
			this.groupBox3.Location = new System.Drawing.Point(5, 87);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(189, 85);
			this.groupBox3.TabIndex = 39;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Tag return options";
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.groupBox4);
			this.groupBox2.Controls.Add(this.RecurseFoldersCheckBox);
			this.groupBox2.Controls.Add(this.PatternTextBox);
			this.groupBox2.Controls.Add(this.DepthTextBox);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Location = new System.Drawing.Point(220, 106);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(213, 181);
			this.groupBox2.TabIndex = 40;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Options";
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.ShowFullFilenamesRadioButton);
			this.groupBox4.Controls.Add(this.ShowWorkspaceRelativeFilenamesRadioButton);
			this.groupBox4.Controls.Add(this.ShowFolderRelativeFilenamesRadioButton);
			this.groupBox4.Controls.Add(this.ShowBaseFilenamesRadioButton);
			this.groupBox4.Location = new System.Drawing.Point(9, 87);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(189, 85);
			this.groupBox4.TabIndex = 40;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Filename show options";
			// 
			// RecurseFoldersCheckBox
			// 
			this.RecurseFoldersCheckBox.AutoSize = true;
			this.RecurseFoldersCheckBox.Location = new System.Drawing.Point(6, 18);
			this.RecurseFoldersCheckBox.Name = "RecurseFoldersCheckBox";
			this.RecurseFoldersCheckBox.Size = new System.Drawing.Size(100, 17);
			this.RecurseFoldersCheckBox.TabIndex = 39;
			this.RecurseFoldersCheckBox.Text = "Recurse folders";
			this.RecurseFoldersCheckBox.UseVisualStyleBackColor = true;
			// 
			// ToolBoxFileComboBoxForm
			// 
			this.AcceptButton = this.ToolBoxIniComboOKButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.ToolBoxIniComboCancelButton;
			this.ClientSize = new System.Drawing.Size(440, 324);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.ToolTipTextBox);
			this.Controls.Add(this.NameTextBox);
			this.Controls.Add(this.PathTextBox);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.ToolBoxIniComboCancelButton);
			this.Controls.Add(this.ToolBoxIniComboOKButton);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.PathBrowseButton);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(1000, 1000);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(437, 330);
			this.Name = "ToolBoxFileComboBoxForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Create/Edit File Drop Down";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private void PathBrowseButton_Click(object sender, System.EventArgs e)
		{
			FolderBrowserDialog fbd = new FolderBrowserDialog();
			fbd.Description = "Please select the desired Folder...";
			
			if( MOG_ControllerProject.GetCurrentSyncDataController() != null )
				fbd.SelectedPath = MOG_ControllerProject.GetCurrentSyncDataController().GetSyncDirectory();

			if (fbd.ShowDialog( this ) == DialogResult.OK) 
			{
				PathTextBox.Text = fbd.SelectedPath;
				this.mSelectedPath = PathTextBox.Text;
				if( this.NameTextBox.Text != null && this.NameTextBox.Text.Length < 1 )
				{
					this.NameTextBox.Text = fbd.SelectedPath.Substring( fbd.SelectedPath.LastIndexOf("\\")+("\\").Length );
				}
				if( this.mDefComboBox != null )
				{
					this.mDefComboBox.Items.Add( new MogTaggedString(fbd.SelectedPath, fbd.SelectedPath, MOG_Client.MogTaggedString.FilenameStyleTypes.FullFilename, SelectedPath) );
				}
			}
		}

		private void NameTextBox_TextChanged(object sender, System.EventArgs e)
		{
			if( this.TagCheckBox.Checked )
				this.TagTextBox.Text = ((TextBox)sender).Text;
		}

		private void ToolBoxIniComboOKButton_Click(object sender, System.EventArgs e)
		{
			// If our user didn't select a path, make sure we do not pretend they did...
			if( this.PathTextBox.Text == LeaveBlank_Text || this.mSelectedPath == LeaveBlank_Text )
			{
				this.PathTextBox.Text = "";
				this.mSelectedPath = "";
			}

			// Try parsing in our depth, otherwise, use default
			try
			{
				this.ComboBoxDepth = int.Parse( this.DepthTextBox.Text );
			}
			catch( Exception )
			{
				this.ComboBoxDepth = ControlDefinition.Default_ComboBox_Depth;
			}

			// TODO: Check other parameters
		}

		private void TagCheckBox_CheckedChanged(object sender, System.EventArgs e)
		{
			if( this.TagCheckBox.Checked )
				this.TagTextBox.Text = this.NameTextBox.Text;
		}

		private void ToolBoxIniComboCancelButton_Click(object sender, System.EventArgs e)
		{			
			this.Close();
		}
	}
}
