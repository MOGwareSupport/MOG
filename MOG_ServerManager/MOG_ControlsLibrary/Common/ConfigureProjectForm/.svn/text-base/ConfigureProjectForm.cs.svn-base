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
using MOG.PROMPT;
using MOG.CONTROLLER.CONTROLLERPROJECT;

using MOG_ServerManager.Utilities;
using MOG_ControlsLibrary;

namespace MOG_ServerManager.MOG_ControlsLibrary
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
		private MOG_ServerManager.MOG_ControlsLibrary.ProjectInfoControl projectInfoControl;
		private MOG_ServerManager.MOG_ControlsLibrary.ProjectInfoControl projectInfoControl1;
		private System.Windows.Forms.Button btnClose;
		private MOG_ServerManager.MOG_ControlsLibrary.AssetClassificationConfigControl assetClassificationConfigControl1;
		private MOGUserManager userManager;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ConfigureProjectForm));
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tpProjectInfo = new System.Windows.Forms.TabPage();
			this.projectInfoControl1 = new MOG_ServerManager.MOG_ControlsLibrary.ProjectInfoControl();
			this.tpUsers = new System.Windows.Forms.TabPage();
			this.userManager = new MOGUserManager();
			this.tpClassifications = new System.Windows.Forms.TabPage();
			this.assetClassificationConfigControl1 = new MOG_ServerManager.MOG_ControlsLibrary.AssetClassificationConfigControl();
			this.tpPlatforms = new System.Windows.Forms.TabPage();
			this.platformEditor = new MOGPlatformEditor();
			this.projectInfoControl = new MOG_ServerManager.MOG_ControlsLibrary.ProjectInfoControl();
			this.btnBrowseProjectPath = new System.Windows.Forms.Button();
			this.btnClose = new System.Windows.Forms.Button();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.tabControl.SuspendLayout();
			this.tpProjectInfo.SuspendLayout();
			this.tpUsers.SuspendLayout();
			this.tpClassifications.SuspendLayout();
			this.tpPlatforms.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl
			// 
			this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl.Controls.Add(this.tpProjectInfo);
			this.tabControl.Controls.Add(this.tpUsers);
			this.tabControl.Controls.Add(this.tpClassifications);
			this.tabControl.Controls.Add(this.tpPlatforms);
			this.tabControl.Location = new System.Drawing.Point(8, 8);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(664, 450);
			this.tabControl.TabIndex = 0;
			this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
			// 
			// tpProjectInfo
			// 
			this.tpProjectInfo.Controls.Add(this.projectInfoControl1);
			this.tpProjectInfo.Location = new System.Drawing.Point(4, 22);
			this.tpProjectInfo.Name = "tpProjectInfo";
			this.tpProjectInfo.Size = new System.Drawing.Size(656, 424);
			this.tpProjectInfo.TabIndex = 0;
			this.tpProjectInfo.Text = "Project Info";
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
			this.projectInfoControl1.Size = new System.Drawing.Size(640, 104);
			this.projectInfoControl1.TabIndex = 0;
			// 
			// tpUsers
			// 
			this.tpUsers.Controls.Add(this.userManager);
			this.tpUsers.Location = new System.Drawing.Point(4, 22);
			this.tpUsers.Name = "tpUsers";
			this.tpUsers.Size = new System.Drawing.Size(656, 424);
			this.tpUsers.TabIndex = 1;
			this.tpUsers.Text = "Users";
			// 
			// userManager
			// 
			this.userManager.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.userManager.Location = new System.Drawing.Point(8, 8);
			this.userManager.Name = "userManager";
			this.userManager.Size = new System.Drawing.Size(640, 408);
			this.userManager.TabIndex = 0;
			// 
			// tpClassifications
			// 
			this.tpClassifications.Controls.Add(this.assetClassificationConfigControl1);
			this.tpClassifications.Location = new System.Drawing.Point(4, 22);
			this.tpClassifications.Name = "tpClassifications";
			this.tpClassifications.Size = new System.Drawing.Size(656, 424);
			this.tpClassifications.TabIndex = 6;
			this.tpClassifications.Text = "Classifications";
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
			this.assetClassificationConfigControl1.Size = new System.Drawing.Size(640, 408);
			this.assetClassificationConfigControl1.TabIndex = 0;
			// 
			// tpPlatforms
			// 
			this.tpPlatforms.Controls.Add(this.platformEditor);
			this.tpPlatforms.Location = new System.Drawing.Point(4, 22);
			this.tpPlatforms.Name = "tpPlatforms";
			this.tpPlatforms.Size = new System.Drawing.Size(656, 424);
			this.tpPlatforms.TabIndex = 2;
			this.tpPlatforms.Text = "Platforms";
			// 
			// platformEditor
			// 
			this.platformEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.platformEditor.IconLocation = null;
			this.platformEditor.Location = new System.Drawing.Point(8, 8);
			this.platformEditor.Name = "platformEditor";
			this.platformEditor.Size = new System.Drawing.Size(640, 408);
			this.platformEditor.TabIndex = 0;
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
			// btnBrowseProjectPath
			// 
			this.btnBrowseProjectPath.Location = new System.Drawing.Point(0, 0);
			this.btnBrowseProjectPath.Name = "btnBrowseProjectPath";
			this.btnBrowseProjectPath.TabIndex = 0;
			// 
			// btnClose
			// 
			this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnClose.Location = new System.Drawing.Point(584, 472);
			this.btnClose.Name = "btnClose";
			this.btnClose.TabIndex = 3;
			this.btnClose.Text = "Close";
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// imageList
			// 
			this.imageList.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// ConfigureProjectForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnClose;
			this.ClientSize = new System.Drawing.Size(682, 512);
			this.ControlBox = false;
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.tabControl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "ConfigureProjectForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Project Configuration";
			this.tabControl.ResumeLayout(false);
			this.tpProjectInfo.ResumeLayout(false);
			this.tpUsers.ResumeLayout(false);
			this.tpClassifications.ResumeLayout(false);
			this.tpPlatforms.ResumeLayout(false);
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

		private const int USERS_WIDTH  = 688;
		private const int USERS_HEIGHT = 544;

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

			this.Text = "Project Configuration - " + project.GetProjectName();

			this.Size = new Size(PROJECT_INFO_WIDTH, PROJECT_INFO_HEIGHT);

			// project info
			this.projectInfoControl1.LoadFromProject(project);

			// platforms
            this.platformEditor.LoadDefaults();
            this.platformEditor.LoadFromProject(project);

			// users
			this.userManager.LoadFromProject(project);

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
					MOG_Prompt.PromptResponse("No Platforms", "You must have at least one platform for the project configuration to be valid", "", MOGPromptButtons.OK, MOG_ALERT_LEVEL.CRITICAL);
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
		private void PackageCommandSelectedEventHandler(object sender, tsbEventArgs e)
		{
			//this.tbAddAssetToPackageCommand.Text += "{" + e.SelectedToken + "}";
		}
		
		private void tabControl_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.tabControl.SelectedTab == this.tpProjectInfo)
			{
				this.Size = new Size(PROJECT_INFO_WIDTH, PROJECT_INFO_HEIGHT);
			}
			else if (this.tabControl.SelectedTab == this.tpPlatforms)
			{
				this.Size = new Size(PLATFORMS_WIDTH, PLATFORMS_HEIGHT);
			}
			else if (this.tabControl.SelectedTab == this.tpUsers)
			{
				this.Size = new Size(USERS_WIDTH, USERS_HEIGHT);
			}
			else if (this.tabControl.SelectedTab == this.tpClassifications)
			{
				this.Size = new Size(CLASSIFICATIONS_WIDTH, CLASSIFICATIONS_HEIGHT);
			}
		}

		private void btnClose_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			Dispose();
		}

	}
		#endregion	
}


/*
#region User definitions
#endregion
#region System definitions
#endregion
#region Constructors
#endregion
#region Member functions
#endregion
#region Event handlers
#endregion
*/



