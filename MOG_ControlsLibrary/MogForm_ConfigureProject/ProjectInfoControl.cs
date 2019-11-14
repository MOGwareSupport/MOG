using System;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using MOG;
using MOG.PROJECT;
using MOG.FILENAME;

namespace MOG_ControlsLibrary
{
	/// <summary>
	/// Summary description for ProjectInfoControl.
	/// </summary>
	public class ProjectInfoControl : System.Windows.Forms.UserControl
	{
		#region System definitions
		private System.Windows.Forms.GroupBox gbPackageInfo;
		private System.Windows.Forms.Label lblDeletePackage;
		private System.Windows.Forms.TextBox tbDeletePackageCommand;
		private System.Windows.Forms.Label lblRemoveAssetFromPackage;
		private System.Windows.Forms.TextBox tbRemoveAssetFromPackageCommand;
		private System.Windows.Forms.Label lblAddAssetToPackage;
		private System.Windows.Forms.Label lblPackageCommands;
		private System.Windows.Forms.TextBox tbAddAssetToPackageCommand;
		private System.Windows.Forms.Button btnBrowsePackageTool;
		private System.Windows.Forms.Label lblPackageTool;
		private System.Windows.Forms.TextBox tbPackageTool;
		private System.Windows.Forms.GroupBox grpBoxProjectInfo;
		private System.Windows.Forms.Label lblProjectPath;
		private System.Windows.Forms.TextBox tbProjectKey;
		private System.Windows.Forms.Label lblProjectNameInSourceControl;
		private System.Windows.Forms.TextBox tbProjectNameInSourceControl;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox tbProjectName;
		private System.Windows.Forms.Label lblProjectKey;
		private System.Windows.Forms.Button btnBrowseBuildTool;
		private System.Windows.Forms.Label lblBuildTool;
		private System.Windows.Forms.TextBox tbBuildTool;
		private System.Windows.Forms.TextBox tbProjectPath;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.gbPackageInfo = new System.Windows.Forms.GroupBox();
			this.lblDeletePackage = new System.Windows.Forms.Label();
			this.tbDeletePackageCommand = new System.Windows.Forms.TextBox();
			this.lblRemoveAssetFromPackage = new System.Windows.Forms.Label();
			this.tbRemoveAssetFromPackageCommand = new System.Windows.Forms.TextBox();
			this.lblAddAssetToPackage = new System.Windows.Forms.Label();
			this.lblPackageCommands = new System.Windows.Forms.Label();
			this.tbAddAssetToPackageCommand = new System.Windows.Forms.TextBox();
			this.btnBrowsePackageTool = new System.Windows.Forms.Button();
			this.lblPackageTool = new System.Windows.Forms.Label();
			this.tbPackageTool = new System.Windows.Forms.TextBox();
			this.grpBoxProjectInfo = new System.Windows.Forms.GroupBox();
			this.tbProjectPath = new System.Windows.Forms.TextBox();
			this.lblProjectPath = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.tbProjectName = new System.Windows.Forms.TextBox();
			this.tbProjectKey = new System.Windows.Forms.TextBox();
			this.lblProjectNameInSourceControl = new System.Windows.Forms.Label();
			this.tbProjectNameInSourceControl = new System.Windows.Forms.TextBox();
			this.lblProjectKey = new System.Windows.Forms.Label();
			this.btnBrowseBuildTool = new System.Windows.Forms.Button();
			this.lblBuildTool = new System.Windows.Forms.Label();
			this.tbBuildTool = new System.Windows.Forms.TextBox();
			this.gbPackageInfo.SuspendLayout();
			this.grpBoxProjectInfo.SuspendLayout();
			this.SuspendLayout();
			// 
			// gbPackageInfo
			// 
			this.gbPackageInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.gbPackageInfo.Controls.Add(this.lblDeletePackage);
			this.gbPackageInfo.Controls.Add(this.tbDeletePackageCommand);
			this.gbPackageInfo.Controls.Add(this.lblRemoveAssetFromPackage);
			this.gbPackageInfo.Controls.Add(this.tbRemoveAssetFromPackageCommand);
			this.gbPackageInfo.Controls.Add(this.lblAddAssetToPackage);
			this.gbPackageInfo.Controls.Add(this.lblPackageCommands);
			this.gbPackageInfo.Controls.Add(this.tbAddAssetToPackageCommand);
			this.gbPackageInfo.Controls.Add(this.btnBrowsePackageTool);
			this.gbPackageInfo.Controls.Add(this.lblPackageTool);
			this.gbPackageInfo.Controls.Add(this.tbPackageTool);
			this.gbPackageInfo.Location = new System.Drawing.Point(8, 104);
			this.gbPackageInfo.Name = "gbPackageInfo";
			this.gbPackageInfo.Size = new System.Drawing.Size(472, 152);
			this.gbPackageInfo.TabIndex = 41;
			this.gbPackageInfo.TabStop = false;
			this.gbPackageInfo.Text = "Package Info";
			this.gbPackageInfo.Visible = false;
			// 
			// lblDeletePackage
			// 
			this.lblDeletePackage.Location = new System.Drawing.Point(16, 96);
			this.lblDeletePackage.Name = "lblDeletePackage";
			this.lblDeletePackage.Size = new System.Drawing.Size(112, 16);
			this.lblDeletePackage.TabIndex = 1000;
			this.lblDeletePackage.Text = "Delete Package";
			this.lblDeletePackage.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// tbDeletePackageCommand
			// 
			this.tbDeletePackageCommand.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tbDeletePackageCommand.HideSelection = false;
			this.tbDeletePackageCommand.Location = new System.Drawing.Point(136, 96);
			this.tbDeletePackageCommand.Name = "tbDeletePackageCommand";
			this.tbDeletePackageCommand.Size = new System.Drawing.Size(272, 20);
			this.tbDeletePackageCommand.TabIndex = 40;
			this.tbDeletePackageCommand.Text = "";
			// 
			// lblRemoveAssetFromPackage
			// 
			this.lblRemoveAssetFromPackage.Location = new System.Drawing.Point(16, 72);
			this.lblRemoveAssetFromPackage.Name = "lblRemoveAssetFromPackage";
			this.lblRemoveAssetFromPackage.Size = new System.Drawing.Size(112, 16);
			this.lblRemoveAssetFromPackage.TabIndex = 1000;
			this.lblRemoveAssetFromPackage.Text = "Remove Asset";
			this.lblRemoveAssetFromPackage.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// tbRemoveAssetFromPackageCommand
			// 
			this.tbRemoveAssetFromPackageCommand.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tbRemoveAssetFromPackageCommand.HideSelection = false;
			this.tbRemoveAssetFromPackageCommand.Location = new System.Drawing.Point(136, 72);
			this.tbRemoveAssetFromPackageCommand.Name = "tbRemoveAssetFromPackageCommand";
			this.tbRemoveAssetFromPackageCommand.Size = new System.Drawing.Size(272, 20);
			this.tbRemoveAssetFromPackageCommand.TabIndex = 30;
			this.tbRemoveAssetFromPackageCommand.Text = "";
			// 
			// lblAddAssetToPackage
			// 
			this.lblAddAssetToPackage.Location = new System.Drawing.Point(16, 48);
			this.lblAddAssetToPackage.Name = "lblAddAssetToPackage";
			this.lblAddAssetToPackage.Size = new System.Drawing.Size(112, 16);
			this.lblAddAssetToPackage.TabIndex = 1000;
			this.lblAddAssetToPackage.Text = "Add Asset";
			this.lblAddAssetToPackage.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// lblPackageCommands
			// 
			this.lblPackageCommands.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblPackageCommands.Location = new System.Drawing.Point(16, 24);
			this.lblPackageCommands.Name = "lblPackageCommands";
			this.lblPackageCommands.Size = new System.Drawing.Size(120, 16);
			this.lblPackageCommands.TabIndex = 1000;
			this.lblPackageCommands.Text = "Package Commands:";
			this.lblPackageCommands.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// tbAddAssetToPackageCommand
			// 
			this.tbAddAssetToPackageCommand.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tbAddAssetToPackageCommand.HideSelection = false;
			this.tbAddAssetToPackageCommand.Location = new System.Drawing.Point(136, 48);
			this.tbAddAssetToPackageCommand.Name = "tbAddAssetToPackageCommand";
			this.tbAddAssetToPackageCommand.Size = new System.Drawing.Size(272, 20);
			this.tbAddAssetToPackageCommand.TabIndex = 20;
			this.tbAddAssetToPackageCommand.Text = "";
			// 
			// btnBrowsePackageTool
			// 
			this.btnBrowsePackageTool.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnBrowsePackageTool.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnBrowsePackageTool.Location = new System.Drawing.Point(416, 120);
			this.btnBrowsePackageTool.Name = "btnBrowsePackageTool";
			this.btnBrowsePackageTool.Size = new System.Drawing.Size(24, 23);
			this.btnBrowsePackageTool.TabIndex = 55;
			this.btnBrowsePackageTool.Text = "...";
			this.btnBrowsePackageTool.Click += new System.EventHandler(this.btnBrowsePackageTool_Click);
			// 
			// lblPackageTool
			// 
			this.lblPackageTool.Location = new System.Drawing.Point(16, 120);
			this.lblPackageTool.Name = "lblPackageTool";
			this.lblPackageTool.Size = new System.Drawing.Size(112, 16);
			this.lblPackageTool.TabIndex = 1000;
			this.lblPackageTool.Text = "Package Tool";
			this.lblPackageTool.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// tbPackageTool
			// 
			this.tbPackageTool.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tbPackageTool.Location = new System.Drawing.Point(136, 120);
			this.tbPackageTool.Name = "tbPackageTool";
			this.tbPackageTool.Size = new System.Drawing.Size(272, 20);
			this.tbPackageTool.TabIndex = 50;
			this.tbPackageTool.Text = "";
			// 
			// grpBoxProjectInfo
			// 
			this.grpBoxProjectInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.grpBoxProjectInfo.Controls.Add(this.tbProjectPath);
			this.grpBoxProjectInfo.Controls.Add(this.lblProjectPath);
			this.grpBoxProjectInfo.Controls.Add(this.label2);
			this.grpBoxProjectInfo.Controls.Add(this.tbProjectName);
			this.grpBoxProjectInfo.Location = new System.Drawing.Point(8, 8);
			this.grpBoxProjectInfo.Name = "grpBoxProjectInfo";
			this.grpBoxProjectInfo.Size = new System.Drawing.Size(472, 88);
			this.grpBoxProjectInfo.TabIndex = 37;
			this.grpBoxProjectInfo.TabStop = false;
			this.grpBoxProjectInfo.Text = "Project Info";
			// 
			// tbProjectPath
			// 
			this.tbProjectPath.Location = new System.Drawing.Point(144, 48);
			this.tbProjectPath.Name = "tbProjectPath";
			this.tbProjectPath.ReadOnly = true;
			this.tbProjectPath.Size = new System.Drawing.Size(248, 20);
			this.tbProjectPath.TabIndex = 1001;
			this.tbProjectPath.Text = "";
			// 
			// lblProjectPath
			// 
			this.lblProjectPath.Location = new System.Drawing.Point(24, 48);
			this.lblProjectPath.Name = "lblProjectPath";
			this.lblProjectPath.Size = new System.Drawing.Size(112, 16);
			this.lblProjectPath.TabIndex = 1000;
			this.lblProjectPath.Text = "Project Path";
			this.lblProjectPath.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(24, 24);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(112, 16);
			this.label2.TabIndex = 1000;
			this.label2.Text = "Project Name";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// tbProjectName
			// 
			this.tbProjectName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tbProjectName.Location = new System.Drawing.Point(144, 24);
			this.tbProjectName.Name = "tbProjectName";
			this.tbProjectName.Size = new System.Drawing.Size(144, 20);
			this.tbProjectName.TabIndex = 0;
			this.tbProjectName.Text = "";
			this.tbProjectName.TextChanged += new System.EventHandler(this.tbProjectName_TextChanged);
			// 
			// tbProjectKey
			// 
			this.tbProjectKey.Location = new System.Drawing.Point(224, 320);
			this.tbProjectKey.Name = "tbProjectKey";
			this.tbProjectKey.Size = new System.Drawing.Size(56, 20);
			this.tbProjectKey.TabIndex = 10;
			this.tbProjectKey.Text = "";
			this.tbProjectKey.Visible = false;
			// 
			// lblProjectNameInSourceControl
			// 
			this.lblProjectNameInSourceControl.Location = new System.Drawing.Point(80, 296);
			this.lblProjectNameInSourceControl.Name = "lblProjectNameInSourceControl";
			this.lblProjectNameInSourceControl.Size = new System.Drawing.Size(128, 16);
			this.lblProjectNameInSourceControl.TabIndex = 1000;
			this.lblProjectNameInSourceControl.Text = "Name in Source Control";
			this.lblProjectNameInSourceControl.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			this.lblProjectNameInSourceControl.Visible = false;
			// 
			// tbProjectNameInSourceControl
			// 
			this.tbProjectNameInSourceControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tbProjectNameInSourceControl.Location = new System.Drawing.Point(224, 296);
			this.tbProjectNameInSourceControl.Name = "tbProjectNameInSourceControl";
			this.tbProjectNameInSourceControl.Size = new System.Drawing.Size(144, 20);
			this.tbProjectNameInSourceControl.TabIndex = 5;
			this.tbProjectNameInSourceControl.Text = "";
			this.tbProjectNameInSourceControl.Visible = false;
			// 
			// lblProjectKey
			// 
			this.lblProjectKey.Location = new System.Drawing.Point(96, 320);
			this.lblProjectKey.Name = "lblProjectKey";
			this.lblProjectKey.Size = new System.Drawing.Size(112, 16);
			this.lblProjectKey.TabIndex = 1000;
			this.lblProjectKey.Text = "Project Key";
			this.lblProjectKey.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			this.lblProjectKey.Visible = false;
			// 
			// btnBrowseBuildTool
			// 
			this.btnBrowseBuildTool.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnBrowseBuildTool.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnBrowseBuildTool.Location = new System.Drawing.Point(424, 264);
			this.btnBrowseBuildTool.Name = "btnBrowseBuildTool";
			this.btnBrowseBuildTool.Size = new System.Drawing.Size(24, 23);
			this.btnBrowseBuildTool.TabIndex = 65;
			this.btnBrowseBuildTool.Text = "...";
			this.btnBrowseBuildTool.Visible = false;
			this.btnBrowseBuildTool.Click += new System.EventHandler(this.btnBrowseBuildTool_Click);
			// 
			// lblBuildTool
			// 
			this.lblBuildTool.Location = new System.Drawing.Point(24, 264);
			this.lblBuildTool.Name = "lblBuildTool";
			this.lblBuildTool.Size = new System.Drawing.Size(112, 16);
			this.lblBuildTool.TabIndex = 1000;
			this.lblBuildTool.Text = "Build Tool";
			this.lblBuildTool.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			this.lblBuildTool.Visible = false;
			// 
			// tbBuildTool
			// 
			this.tbBuildTool.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tbBuildTool.Location = new System.Drawing.Point(144, 264);
			this.tbBuildTool.Name = "tbBuildTool";
			this.tbBuildTool.Size = new System.Drawing.Size(272, 20);
			this.tbBuildTool.TabIndex = 60;
			this.tbBuildTool.Text = "";
			this.tbBuildTool.Visible = false;
			// 
			// ProjectInfoControl
			// 
			this.Controls.Add(this.gbPackageInfo);
			this.Controls.Add(this.grpBoxProjectInfo);
			this.Controls.Add(this.btnBrowseBuildTool);
			this.Controls.Add(this.lblBuildTool);
			this.Controls.Add(this.tbBuildTool);
			this.Controls.Add(this.lblProjectNameInSourceControl);
			this.Controls.Add(this.tbProjectKey);
			this.Controls.Add(this.tbProjectNameInSourceControl);
			this.Controls.Add(this.lblProjectKey);
			this.Name = "ProjectInfoControl";
			this.Size = new System.Drawing.Size(488, 104);
			this.gbPackageInfo.ResumeLayout(false);
			this.grpBoxProjectInfo.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
		#endregion
		#region Member vars
		private string packageToolPath;
		private string buildToolPath;

		private string projectsPath;

		private bool immediateMode = false;
		#endregion
		#region Properties
		public bool ImmediateMode
		{
			get { return this.immediateMode; }
			set { this.immediateMode = value; }
		}

		public bool ProjectNameReadOnly
		{
			get { return this.tbProjectName.ReadOnly; }
			set { this.tbProjectName.ReadOnly = value; }
		}

		public string ProjectName
		{
			get { return this.tbProjectName.Text; }
			set { this.tbProjectName.Text = value; }
		}
		public string ProjectNameInSourceControl
		{
			get { return this.tbProjectNameInSourceControl.Text; }
			set { this.tbProjectNameInSourceControl.Text = value; }
		}
		public string ProjectKey
		{
			get { return this.tbProjectKey.Text; }
			set { this.tbProjectKey.Text = value; }
		}
		public string ProjectPath
		{
			get { return this.tbProjectPath.Text; }
			set { this.tbProjectPath.Text = value; }
		}

		public string ProjectsPath
		{
			get { return this.projectsPath; }
			set { this.projectsPath = value; }
		}

		public string AddAssetToPackageCommand
		{
			get { return this.tbAddAssetToPackageCommand.Text; }
			set { this.tbAddAssetToPackageCommand.Text = value; }
		}
		public string RemoveAssetFromPackageCommand
		{
			get { return this.tbRemoveAssetFromPackageCommand.Text; }
			set { this.tbRemoveAssetFromPackageCommand.Text = value; }
		}
		public string DeletePackageCommand
		{
			get { return this.tbDeletePackageCommand.Text; }
			set { this.tbDeletePackageCommand.Text = value; }
		}
		public string PackageTool
		{
			get { return this.tbPackageTool.Text; }
			set { this.tbPackageTool.Text = value; }
		}
		public string BuildTool
		{
			get { return this.tbBuildTool.Text; }
			set { this.tbBuildTool.Text = value; }
		}
		#endregion
		#region Events
		public event EventHandler ProjectName_TextChanged;
		#endregion
		#region Constructors
		public ProjectInfoControl()
		{
			InitializeComponent();
			this.projectsPath = "";
		}

		public ProjectInfoControl(MOG_Project project)
		{
			InitializeComponent();

			LoadFromProject(project);
		}
		#endregion
		#region Public functions
		public void LoadFromProject(MOG_Project project)
		{
			this.tbProjectName.Text = project.GetProjectName();
			this.tbProjectNameInSourceControl.Text = project.GetProjectName();
			this.tbProjectKey.Text = project.GetProjectName();
			this.tbProjectPath.Text = project.GetProjectPath();
			this.tbBuildTool.Text = "UNKNOWN";

			this.projectsPath = Path.GetDirectoryName( project.GetProjectPath() );
		}

		public bool IsValid(bool showMessages)
		{
			if (this.tbProjectName.Text == "") 
			{
				if (showMessages)
				{
					ProjectConfigUtils.ShowMessageBoxExclamation("Please enter a project name", "Missing data");
					this.tbProjectName.Focus();
				}
				return false;
			}
			else if (this.tbProjectKey.Text == "") 
			{
				if (showMessages)
				{
					ProjectConfigUtils.ShowMessageBoxExclamation("Please enter a project key", "Missing data");
					this.tbProjectKey.Focus();
				}
				return false;
			}
			else if (this.tbProjectPath.Text == "") 
			{
				if (showMessages)
				{
					ProjectConfigUtils.ShowMessageBoxExclamation("Please enter a project path", "Missing data");
					this.tbProjectPath.Focus();
				}
				return false;
			}

			return true;
		}

		public void ProjectNameSelectAll()
		{
			this.tbProjectName.Focus();
			this.tbProjectName.SelectAll();
		}
		public void FocusProjectName()
		{
			this.tbProjectName.Focus();
		}
		public void FocusProjectKey()
		{
			this.tbProjectKey.Focus();
		}
		public void FocusProjectPath()
		{
			this.tbProjectPath.Focus();
		}
		#endregion
		#region Private functions
		private void RaiseProjectName_TextChanged()
		{
			if (this.ProjectName_TextChanged != null)
				this.ProjectName_TextChanged(this, new EventArgs());
		}

		private string[] RemoveTokenBraces(string[] bracedTokenNames)
		{
			string[] debracedTokenNames = new string[ bracedTokenNames.Length ];

			for (int i=0; i<bracedTokenNames.Length; i++)
				debracedTokenNames[i] = RemoveTokenBraces(bracedTokenNames[i]);

			return debracedTokenNames;
		}

		private string RemoveTokenBraces(string bracedTokenName)
		{
			if (bracedTokenName.StartsWith("{")  &&  bracedTokenName.EndsWith("}"))
				return bracedTokenName.Substring(1, bracedTokenName.Length-2);
			else
				return bracedTokenName;
		}
		#endregion
		#region Event handlers
		private void btnBrowsePackageTool_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				FileInfo fInfo = new FileInfo(ofd.FileName);
				if (fInfo.Exists)
				{
					this.packageToolPath = ofd.FileName;
					this.tbPackageTool.Text = fInfo.Name;// + "   [paramaters]";
				}
			}		
		}

		private void btnBrowseBuildTool_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				FileInfo fInfo = new FileInfo(ofd.FileName);
				if (fInfo.Exists)
				{
					this.buildToolPath = ofd.FileName;
					this.tbBuildTool.Text = fInfo.Name + "   [parameters]";
					string parameters = this.tbBuildTool.Text.Replace(fInfo.Name, "").Trim();
					parameters = this.tbBuildTool.Text.Trim().Substring(fInfo.Name.Length).Trim();
					MessageBox.Show("\""+parameters+"\"");
				}
			}
		}

		private void tbProjectName_TextChanged(object sender, System.EventArgs e)
		{
			this.tbProjectNameInSourceControl.Text = this.tbProjectName.Text;
			this.tbProjectPath.Text = this.projectsPath + "\\" + this.tbProjectName.Text;
			this.tbProjectPath.Text = this.tbProjectPath.Text.Replace("\\\\", "\\");
			
			int keyLen = 3;
			if (this.tbProjectName.Text.Length < 3)
				keyLen = this.tbProjectName.Text.Length;
			this.tbProjectKey.Text = this.tbProjectName.Text.Substring(0, keyLen);

			RaiseProjectName_TextChanged();
		}
	}
		#endregion
	
}


/*
#region Member vars
#endregion
#region System definitions
#endregion
#region Constructors
#endregion
#region Public functions
#endregion
#region Private functions
#endregion
#region Event handlers
#endregion
*/



