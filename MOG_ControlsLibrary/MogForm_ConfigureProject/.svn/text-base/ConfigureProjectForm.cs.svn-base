using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using MOG;
using MOG.PROJECT;
using MOG.PLATFORM;
using MOG.PROPERTIES;
using MOG.USER;
using MOG.FILENAME;

using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG_ControlsLibrary.MogForm_ConfigureProject;
using MOG_ControlsLibrary.Common.MogControl_PrivilegesForm;

namespace MOG_ControlsLibrary
{
	/// <summary>
	/// Summary description for ConfigureProjectForm.
	/// </summary>
	public class ConfigureProjectForm : System.Windows.Forms.Form
	{
		#region System definitions

		private System.Windows.Forms.TabPage tpProjectInfo;
		private System.Windows.Forms.TabPage tpUsers;
		private System.Windows.Forms.TabPage tpPlatforms;
		private System.Windows.Forms.Button btnBrowseProjectPath;
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.ImageList imageList;
		private System.Windows.Forms.TabPage tpClassifications;
		private MOGPlatformEditor platformEditor;
		private ProjectInfoControl projectInfoControl;
		private ProjectInfoControl projectInfoControl1;
		private System.Windows.Forms.Button btnClose;
		private AssetClassificationConfigControl assetClassificationConfigControl1;
		private MOGUserManager userManager;
		private TabPage tpDepartments;
		private DepartmentManager departmentManager;
		private TabPage tpWebTabs;
		private MOGWebTabManager webTabManager;
		private TabPage tpBranches;
		private MOGBranchManager mogBranchManager;
		private MOGWebTabManager mogWebTabManager1;
		private TabPage tpPrivileges;
		private MogControl_Privileges MogControl_Privileges;
		private System.ComponentModel.IContainer components;

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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigureProjectForm));
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tpProjectInfo = new System.Windows.Forms.TabPage();
			this.projectInfoControl1 = new MOG_ControlsLibrary.ProjectInfoControl();
			this.tpDepartments = new System.Windows.Forms.TabPage();
			this.departmentManager = new MOG_ControlsLibrary.DepartmentManager();
			this.tpUsers = new System.Windows.Forms.TabPage();
			this.userManager = new MOG_ControlsLibrary.MOGUserManager();
			this.tpPrivileges = new System.Windows.Forms.TabPage();
			this.MogControl_Privileges = new MOG_ControlsLibrary.Common.MogControl_PrivilegesForm.MogControl_Privileges();
			this.tpClassifications = new System.Windows.Forms.TabPage();
			this.assetClassificationConfigControl1 = new MOG_ControlsLibrary.AssetClassificationConfigControl();
			this.tpPlatforms = new System.Windows.Forms.TabPage();
			this.platformEditor = new MOG_ControlsLibrary.MOGPlatformEditor();
			this.tpWebTabs = new System.Windows.Forms.TabPage();
			this.webTabManager = new MOG_ControlsLibrary.MogForm_ConfigureProject.MOGWebTabManager();
			this.tpBranches = new System.Windows.Forms.TabPage();
			this.mogBranchManager = new MOG_ControlsLibrary.MogForm_ConfigureProject.MOGBranchManager();
			this.btnBrowseProjectPath = new System.Windows.Forms.Button();
			this.btnClose = new System.Windows.Forms.Button();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.projectInfoControl = new MOG_ControlsLibrary.ProjectInfoControl();
			this.mogWebTabManager1 = new MOG_ControlsLibrary.MogForm_ConfigureProject.MOGWebTabManager();
			this.tabControl.SuspendLayout();
			this.tpProjectInfo.SuspendLayout();
			this.tpDepartments.SuspendLayout();
			this.tpUsers.SuspendLayout();
			this.tpPrivileges.SuspendLayout();
			this.tpClassifications.SuspendLayout();
			this.tpPlatforms.SuspendLayout();
			this.tpWebTabs.SuspendLayout();
			this.tpBranches.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl
			// 
			this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl.Controls.Add(this.tpProjectInfo);
			this.tabControl.Controls.Add(this.tpDepartments);
			this.tabControl.Controls.Add(this.tpUsers);
			this.tabControl.Controls.Add(this.tpPrivileges);
			this.tabControl.Controls.Add(this.tpClassifications);
			this.tabControl.Controls.Add(this.tpPlatforms);
			this.tabControl.Controls.Add(this.tpWebTabs);
			this.tabControl.Controls.Add(this.tpBranches);
			this.tabControl.Location = new System.Drawing.Point(8, 8);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(744, 450);
			this.tabControl.TabIndex = 0;
			// 
			// tpProjectInfo
			// 
			this.tpProjectInfo.Controls.Add(this.projectInfoControl1);
			this.tpProjectInfo.Location = new System.Drawing.Point(4, 22);
			this.tpProjectInfo.Name = "tpProjectInfo";
			this.tpProjectInfo.Size = new System.Drawing.Size(736, 424);
			this.tpProjectInfo.TabIndex = 0;
			this.tpProjectInfo.Text = "Project Info";
			this.tpProjectInfo.UseVisualStyleBackColor = true;
			// 
			// projectInfoControl1
			// 
			this.projectInfoControl1.AddAssetToPackageCommand = "";
			this.projectInfoControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.projectInfoControl1.BuildTool = "";
			this.projectInfoControl1.DeletePackageCommand = "";
			this.projectInfoControl1.ImmediateMode = true;
			this.projectInfoControl1.Location = new System.Drawing.Point(8, 136);
			this.projectInfoControl1.Name = "projectInfoControl1";
			this.projectInfoControl1.PackageTool = "";
			this.projectInfoControl1.ProjectKey = "";
			this.projectInfoControl1.ProjectName = "";
			this.projectInfoControl1.ProjectNameInSourceControl = "";
			this.projectInfoControl1.ProjectNameReadOnly = true;
			this.projectInfoControl1.ProjectPath = "";
			this.projectInfoControl1.ProjectsPath = "";
			this.projectInfoControl1.RemoveAssetFromPackageCommand = "";
			this.projectInfoControl1.Size = new System.Drawing.Size(720, 104);
			this.projectInfoControl1.TabIndex = 0;
			// 
			// tpDepartments
			// 
			this.tpDepartments.Controls.Add(this.departmentManager);
			this.tpDepartments.Location = new System.Drawing.Point(4, 22);
			this.tpDepartments.Name = "tpDepartments";
			this.tpDepartments.Padding = new System.Windows.Forms.Padding(3);
			this.tpDepartments.Size = new System.Drawing.Size(736, 424);
			this.tpDepartments.TabIndex = 7;
			this.tpDepartments.Text = "Departments";
			this.tpDepartments.UseVisualStyleBackColor = true;
			// 
			// departmentManager
			// 
			this.departmentManager.Location = new System.Drawing.Point(6, 6);
			this.departmentManager.Name = "departmentManager";
			this.departmentManager.Size = new System.Drawing.Size(727, 412);
			this.departmentManager.TabIndex = 0;
			// 
			// tpUsers
			// 
			this.tpUsers.Controls.Add(this.userManager);
			this.tpUsers.Location = new System.Drawing.Point(4, 22);
			this.tpUsers.Name = "tpUsers";
			this.tpUsers.Size = new System.Drawing.Size(736, 424);
			this.tpUsers.TabIndex = 1;
			this.tpUsers.Text = "Users";
			this.tpUsers.UseVisualStyleBackColor = true;
			// 
			// userManager
			// 
			this.userManager.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.userManager.Location = new System.Drawing.Point(8, 8);
			this.userManager.Name = "userManager";
			this.userManager.Size = new System.Drawing.Size(720, 408);
			this.userManager.TabIndex = 0;
			// 
			// tpPrivileges
			// 
			this.tpPrivileges.Controls.Add(this.MogControl_Privileges);
			this.tpPrivileges.Location = new System.Drawing.Point(4, 22);
			this.tpPrivileges.Name = "tpPrivileges";
			this.tpPrivileges.Size = new System.Drawing.Size(736, 424);
			this.tpPrivileges.TabIndex = 10;
			this.tpPrivileges.Text = "Privileges";
			this.tpPrivileges.UseVisualStyleBackColor = true;
			// 
			// MogControl_Privileges
			// 
			this.MogControl_Privileges.Dock = System.Windows.Forms.DockStyle.Fill;
			this.MogControl_Privileges.Location = new System.Drawing.Point(0, 0);
			this.MogControl_Privileges.Name = "MogControl_Privileges";
			this.MogControl_Privileges.Size = new System.Drawing.Size(736, 424);
			this.MogControl_Privileges.TabIndex = 0;
			// 
			// tpClassifications
			// 
			this.tpClassifications.Controls.Add(this.assetClassificationConfigControl1);
			this.tpClassifications.Location = new System.Drawing.Point(4, 22);
			this.tpClassifications.Name = "tpClassifications";
			this.tpClassifications.Size = new System.Drawing.Size(736, 424);
			this.tpClassifications.TabIndex = 6;
			this.tpClassifications.Text = "Classifications";
			this.tpClassifications.UseVisualStyleBackColor = true;
			// 
			// assetClassificationConfigControl1
			// 
			this.assetClassificationConfigControl1.AddClassificationMenuItemVisible = true;
			this.assetClassificationConfigControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.assetClassificationConfigControl1.ConfigurationVisible = true;
			this.assetClassificationConfigControl1.ImmediateMode = true;
			this.assetClassificationConfigControl1.Location = new System.Drawing.Point(8, 8);
			this.assetClassificationConfigControl1.Name = "assetClassificationConfigControl1";
			this.assetClassificationConfigControl1.ProjectRootPath = "";
			this.assetClassificationConfigControl1.RemoveClassificationMenuItemVisible = true;
			this.assetClassificationConfigControl1.ShowConfigurationMenuItemVisible = true;
			this.assetClassificationConfigControl1.Size = new System.Drawing.Size(720, 408);
			this.assetClassificationConfigControl1.TabIndex = 0;
			// 
			// tpPlatforms
			// 
			this.tpPlatforms.Controls.Add(this.platformEditor);
			this.tpPlatforms.Location = new System.Drawing.Point(4, 22);
			this.tpPlatforms.Name = "tpPlatforms";
			this.tpPlatforms.Size = new System.Drawing.Size(736, 424);
			this.tpPlatforms.TabIndex = 2;
			this.tpPlatforms.Text = "Platforms";
			this.tpPlatforms.UseVisualStyleBackColor = true;
			// 
			// platformEditor
			// 
			this.platformEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.platformEditor.IconLocation = null;
			this.platformEditor.Location = new System.Drawing.Point(8, 8);
			this.platformEditor.Name = "platformEditor";
			this.platformEditor.Size = new System.Drawing.Size(720, 408);
			this.platformEditor.TabIndex = 0;
			// 
			// tpWebTabs
			// 
			this.tpWebTabs.Controls.Add(this.webTabManager);
			this.tpWebTabs.Location = new System.Drawing.Point(4, 22);
			this.tpWebTabs.Name = "tpWebTabs";
			this.tpWebTabs.Size = new System.Drawing.Size(736, 424);
			this.tpWebTabs.TabIndex = 8;
			this.tpWebTabs.Text = "Web Tabs";
			this.tpWebTabs.UseVisualStyleBackColor = true;
			// 
			// webTabManager
			// 
			this.webTabManager.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.webTabManager.Location = new System.Drawing.Point(8, 8);
			this.webTabManager.Name = "webTabManager";
			this.webTabManager.Size = new System.Drawing.Size(720, 408);
			this.webTabManager.TabIndex = 1;
			// 
			// tpBranches
			// 
			this.tpBranches.Controls.Add(this.mogBranchManager);
			this.tpBranches.Location = new System.Drawing.Point(4, 22);
			this.tpBranches.Name = "tpBranches";
			this.tpBranches.Size = new System.Drawing.Size(736, 424);
			this.tpBranches.TabIndex = 9;
			this.tpBranches.Text = "Branches";
			this.tpBranches.UseVisualStyleBackColor = true;
			// 
			// mogBranchManager
			// 
			this.mogBranchManager.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.mogBranchManager.Location = new System.Drawing.Point(8, 8);
			this.mogBranchManager.Name = "mogBranchManager";
			this.mogBranchManager.Size = new System.Drawing.Size(720, 408);
			this.mogBranchManager.TabIndex = 2;
			// 
			// btnBrowseProjectPath
			// 
			this.btnBrowseProjectPath.Location = new System.Drawing.Point(0, 0);
			this.btnBrowseProjectPath.Name = "btnBrowseProjectPath";
			this.btnBrowseProjectPath.Size = new System.Drawing.Size(75, 23);
			this.btnBrowseProjectPath.TabIndex = 0;
			// 
			// btnClose
			// 
			this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnClose.Location = new System.Drawing.Point(664, 472);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(75, 23);
			this.btnClose.TabIndex = 3;
			this.btnClose.Text = "Close";
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// imageList
			// 
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList.Images.SetKeyName(0, "");
			this.imageList.Images.SetKeyName(1, "");
			this.imageList.Images.SetKeyName(2, "");
			this.imageList.Images.SetKeyName(3, "");
			// 
			// projectInfoControl
			// 
			this.projectInfoControl.AddAssetToPackageCommand = "";
			this.projectInfoControl.BuildTool = "";
			this.projectInfoControl.DeletePackageCommand = "";
			this.projectInfoControl.ImmediateMode = false;
			this.projectInfoControl.Location = new System.Drawing.Point(0, 0);
			this.projectInfoControl.Name = "projectInfoControl";
			this.projectInfoControl.PackageTool = "";
			this.projectInfoControl.ProjectKey = "";
			this.projectInfoControl.ProjectName = "";
			this.projectInfoControl.ProjectNameInSourceControl = "";
			this.projectInfoControl.ProjectNameReadOnly = false;
			this.projectInfoControl.ProjectPath = "";
			this.projectInfoControl.ProjectsPath = "";
			this.projectInfoControl.RemoveAssetFromPackageCommand = "";
			this.projectInfoControl.Size = new System.Drawing.Size(488, 104);
			this.projectInfoControl.TabIndex = 0;
			// 
			// mogWebTabManager1
			// 
			this.mogWebTabManager1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.mogWebTabManager1.Location = new System.Drawing.Point(8, 8);
			this.mogWebTabManager1.Name = "mogWebTabManager1";
			this.mogWebTabManager1.Size = new System.Drawing.Size(720, 408);
			this.mogWebTabManager1.TabIndex = 1;
			// 
			// ConfigureProjectForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(762, 512);
			this.ControlBox = false;
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.tabControl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "ConfigureProjectForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Project Configuration";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConfigureProjectForm_FormClosing);
			this.Load += new System.EventHandler(this.ConfigureProjectForm_Load);
			this.tabControl.ResumeLayout(false);
			this.tpProjectInfo.ResumeLayout(false);
			this.tpDepartments.ResumeLayout(false);
			this.tpUsers.ResumeLayout(false);
			this.tpPrivileges.ResumeLayout(false);
			this.tpClassifications.ResumeLayout(false);
			this.tpPlatforms.ResumeLayout(false);
			this.tpWebTabs.ResumeLayout(false);
			this.tpBranches.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
		#endregion
		#region Member vars
		private MOG_Project project;
		#endregion
		#region Constants
		private const int PROJECT_INFO_WIDTH = 520;
		private const int PROJECT_INFO_HEIGHT  = 296;
		
		private const int PLATFORMS_WIDTH = 512;
		private const int PLATFORMS_HEIGHT  = 424;

		private const int USERS_WIDTH  = 608;
		private const int USERS_HEIGHT = 504;

		private const int CLASSIFICATIONS_WIDTH  = 736;
		private const int CLASSIFICATIONS_HEIGHT = 544;

		private const int SLAVES_WIDTH  = 656;
		private const int SLAVES_HEIGHT = 416;

		private const int PACKAGES_WIDTH  = 608;
		private const int PACKAGES_HEIGHT = 296;

		private const int BUILDS_WIDTH  = 496;
		private const int BUILDS_HEIGHT = 304;
		#endregion
		#region Constructors
		public ConfigureProjectForm()
		{
			InitializeComponent();
		}

		public ConfigureProjectForm( MOG_Project project )
		{
			InitializeComponent();

			this.project = project;

			this.Text = "Project Configuration - Project: " + project.GetProjectName() + "     ( Branch: " + MOG_ControllerProject.GetBranchName() + " )";

			// project info
			this.projectInfoControl1.LoadFromProject(project);

			// platforms
			this.platformEditor.LoadDefaults();
			this.platformEditor.LoadFromProject(project);

			//departments
			departmentManager.LoadFromProject(project);
			
			// users
			this.userManager.LoadFromProject(project);

			// Privileges
			MOG_ControllerProject.RefreshPrivileges();
			this.MogControl_Privileges.Initialize_Control(MOG_ControllerProject.GetPrivileges());
			
			// asset classes
			this.assetClassificationConfigControl1.LoadProjectClassifications2(project);
			//this.assetClassificationConfigControl1.LoadProjectClassifications(project);
		}

		#endregion
		#region Member functions
		private bool ConfigsValid(bool showDialogs, bool refocus)
		{
			if (!this.platformEditor.ConfigurationValid())
			{
				if (showDialogs)
					ProjectConfigUtils.ShowMessageBoxExclamation("You must have at least one platform for the project configuration to be valid", "No Platforms");
				if (refocus)
					this.tabControl.SelectedTab = this.tpPlatforms;
				
				return false;
			}

			return true;
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
		
		private void btnClose_Click(object sender, System.EventArgs e)
		{
			Dispose();
		}

		private void ConfigureProjectForm_Load(object sender, EventArgs e)
		{
			webTabManager.InitializeListView();
			mogBranchManager.InitializeListView();

			try
			{
				tabControl.SelectedIndex = MogUtils_Settings.MogUtils_Settings.LoadIntSetting("ConfigureProject", "SelectedTab", 0);
			}
			catch
			{				
			}
		}

		private void ConfigureProjectForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			MogUtils_Settings.MogUtils_Settings.SaveSetting("ConfigureProject", "SelectedTab", tabControl.SelectedIndex.ToString());			
		}

	}
		#endregion	
}
