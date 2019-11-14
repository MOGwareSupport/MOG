using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using MOG;
using MOG.USER;
using MOG.COMMAND;
using MOG.PROJECT;
using MOG.DATABASE;
using MOG.FILENAME;
using MOG.PLATFORM;
using MOG.PROPERTIES;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.PROMPT;
using MOG.PROGRESS;

using MOG_ServerManager.Utilities;
using MOG_ControlsLibrary;
using MOG_CoreControls;

namespace MOG_ServerManager
{
	/// <summary>
	/// Summary description for NewProjectWizardForm1.
	/// </summary>
	public class ConfigureProjectWizardForm : System.Windows.Forms.Form
	{
		#region System definitions
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnNext;
		private MOGUserManager userManager;
		private MOGPlatformEditor platformEditor;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.PictureBox pictureBox4;
		private System.Windows.Forms.Button btnRemoveAsset2;
		private System.Windows.Forms.ColumnHeader columnHeader6;
		private System.Windows.Forms.ColumnHeader columnHeader5;
		private System.Windows.Forms.ListView lvUnconfiguredAssets;
		private System.Windows.Forms.TabPage tpPlatforms;
		private System.Windows.Forms.TabPage tpUsers;
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.TabPage tpAssetPlacer;
		private System.Windows.Forms.Label lblProjectType;
		private System.Windows.Forms.Label lblStep1;
		private System.Windows.Forms.Label lblStep2;
		private System.Windows.Forms.Label lblStep3;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btnBack;
		private WizardHeader platformsHeader;
		private WizardHeader usersHeader;
		private WizardHeader asserPlacerHeader;
		private WizardHeader finishHeader;
		private System.Windows.Forms.Label lblAssetPlacerStep2;
		private System.Windows.Forms.Label lblAssetPlacerStep1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TabPage tpFinish;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label lblNumUsers;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label lblNumPlatforms;
		private System.Windows.Forms.Label lblNumClassifications;
		private System.Windows.Forms.Label lblNumAssets;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label lblNumDepartments;
		private System.Windows.Forms.Label label11;
		private AssetClassificationConfigControl assetClassificationConfigControl;
		private System.Windows.Forms.Label lblImportTime;
		private System.Windows.Forms.Label lblImportTimeLabel;
		private AssetImportPlacer assetImportPlacer;
		private System.Windows.Forms.ToolTip toolTip1;
		private CheckBox cbAssetExtensions;
		private TabPage tpDepartments;
		private WizardHeader wizardHeader1;
		private DepartmentManager departmentManager;
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigureProjectWizardForm));
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tpPlatforms = new System.Windows.Forms.TabPage();
			this.platformEditor = new MOG_ControlsLibrary.MOGPlatformEditor();
			this.platformsHeader = new MOG_ServerManager.WizardHeader();
			this.tpDepartments = new System.Windows.Forms.TabPage();
			this.departmentManager = new MOG_ControlsLibrary.DepartmentManager();
			this.wizardHeader1 = new MOG_ServerManager.WizardHeader();
			this.tpUsers = new System.Windows.Forms.TabPage();
			this.userManager = new MOG_ControlsLibrary.MOGUserManager();
			this.usersHeader = new MOG_ServerManager.WizardHeader();
			this.tpAssetPlacer = new System.Windows.Forms.TabPage();
			this.cbAssetExtensions = new System.Windows.Forms.CheckBox();
			this.assetImportPlacer = new MOG_ServerManager.AssetImportPlacer();
			this.label1 = new System.Windows.Forms.Label();
			this.lblAssetPlacerStep2 = new System.Windows.Forms.Label();
			this.lblAssetPlacerStep1 = new System.Windows.Forms.Label();
			this.lblStep3 = new System.Windows.Forms.Label();
			this.lblStep2 = new System.Windows.Forms.Label();
			this.lblStep1 = new System.Windows.Forms.Label();
			this.asserPlacerHeader = new MOG_ServerManager.WizardHeader();
			this.tpFinish = new System.Windows.Forms.TabPage();
			this.lblImportTime = new System.Windows.Forms.Label();
			this.lblImportTimeLabel = new System.Windows.Forms.Label();
			this.lblNumDepartments = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.lblNumAssets = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.lblNumClassifications = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.lblNumPlatforms = new System.Windows.Forms.Label();
			this.lblNumUsers = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.finishHeader = new MOG_ServerManager.WizardHeader();
			this.assetClassificationConfigControl = new MOG_ServerManager.AssetClassificationConfigControl();
			this.lblProjectType = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnNext = new System.Windows.Forms.Button();
			this.label6 = new System.Windows.Forms.Label();
			this.btnRemoveAsset2 = new System.Windows.Forms.Button();
			this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
			this.lvUnconfiguredAssets = new System.Windows.Forms.ListView();
			this.btnBack = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.pictureBox4 = new System.Windows.Forms.PictureBox();
			this.tabControl.SuspendLayout();
			this.tpPlatforms.SuspendLayout();
			this.tpDepartments.SuspendLayout();
			this.tpUsers.SuspendLayout();
			this.tpAssetPlacer.SuspendLayout();
			this.tpFinish.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
			this.SuspendLayout();
			// 
			// tabControl
			// 
			this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl.Controls.Add(this.tpPlatforms);
			this.tabControl.Controls.Add(this.tpDepartments);
			this.tabControl.Controls.Add(this.tpUsers);
			this.tabControl.Controls.Add(this.tpAssetPlacer);
			this.tabControl.Controls.Add(this.tpFinish);
			this.tabControl.Location = new System.Drawing.Point(0, -21);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(1031, 525);
			this.tabControl.TabIndex = 0;
			this.tabControl.TabStop = false;
			this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
			// 
			// tpPlatforms
			// 
			this.tpPlatforms.Controls.Add(this.platformEditor);
			this.tpPlatforms.Controls.Add(this.platformsHeader);
			this.tpPlatforms.Location = new System.Drawing.Point(4, 22);
			this.tpPlatforms.Name = "tpPlatforms";
			this.tpPlatforms.Size = new System.Drawing.Size(1023, 499);
			this.tpPlatforms.TabIndex = 1;
			this.tpPlatforms.Text = "Platforms";
			this.tpPlatforms.UseVisualStyleBackColor = true;
			// 
			// platformEditor
			// 
			this.platformEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.platformEditor.IconLocation = null;
			this.platformEditor.Location = new System.Drawing.Point(8, 72);
			this.platformEditor.Name = "platformEditor";
			this.platformEditor.Size = new System.Drawing.Size(1007, 421);
			this.platformEditor.TabIndex = 11;
			// 
			// platformsHeader
			// 
			this.platformsHeader.BackColor = System.Drawing.SystemColors.Window;
			this.platformsHeader.Description = "Please specify the desired platforms for the new project";
			this.platformsHeader.Dock = System.Windows.Forms.DockStyle.Top;
			this.platformsHeader.Image = global::MOG_ServerManager.Properties.Resources.MOG_Platforms;
			this.platformsHeader.Location = new System.Drawing.Point(0, 0);
			this.platformsHeader.Name = "platformsHeader";
			this.platformsHeader.Size = new System.Drawing.Size(1023, 60);
			this.platformsHeader.TabIndex = 6;
			this.platformsHeader.Title = "Platforms (1 of 4)";
			// 
			// tpDepartments
			// 
			this.tpDepartments.Controls.Add(this.departmentManager);
			this.tpDepartments.Controls.Add(this.wizardHeader1);
			this.tpDepartments.Location = new System.Drawing.Point(4, 22);
			this.tpDepartments.Name = "tpDepartments";
			this.tpDepartments.Size = new System.Drawing.Size(1023, 466);
			this.tpDepartments.TabIndex = 10;
			this.tpDepartments.Text = "Departments";
			this.tpDepartments.UseVisualStyleBackColor = true;
			// 
			// departmentManager
			// 
			this.departmentManager.Dock = System.Windows.Forms.DockStyle.Fill;
			this.departmentManager.Location = new System.Drawing.Point(0, 60);
			this.departmentManager.Name = "departmentManager";
			this.departmentManager.Size = new System.Drawing.Size(1023, 406);
			this.departmentManager.TabIndex = 8;
			// 
			// wizardHeader1
			// 
			this.wizardHeader1.BackColor = System.Drawing.SystemColors.Window;
			this.wizardHeader1.Description = "Please specify the different departments of users that will be working on this pr" +
				"oject.";
			this.wizardHeader1.Dock = System.Windows.Forms.DockStyle.Top;
			this.wizardHeader1.Image = global::MOG_ServerManager.Properties.Resources.MOG_Platforms;
			this.wizardHeader1.Location = new System.Drawing.Point(0, 0);
			this.wizardHeader1.Name = "wizardHeader1";
			this.wizardHeader1.Size = new System.Drawing.Size(1023, 60);
			this.wizardHeader1.TabIndex = 7;
			this.wizardHeader1.Title = "Departments (2 of 4)";
			// 
			// tpUsers
			// 
			this.tpUsers.Controls.Add(this.userManager);
			this.tpUsers.Controls.Add(this.usersHeader);
			this.tpUsers.Location = new System.Drawing.Point(4, 22);
			this.tpUsers.Name = "tpUsers";
			this.tpUsers.Size = new System.Drawing.Size(1023, 466);
			this.tpUsers.TabIndex = 2;
			this.tpUsers.Text = "Users";
			this.tpUsers.UseVisualStyleBackColor = true;
			// 
			// userManager
			// 
			this.userManager.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.userManager.Location = new System.Drawing.Point(16, 72);
			this.userManager.Name = "userManager";
			this.userManager.Size = new System.Drawing.Size(999, 388);
			this.userManager.TabIndex = 1;
			// 
			// usersHeader
			// 
			this.usersHeader.BackColor = System.Drawing.SystemColors.Window;
			this.usersHeader.Description = "Enter team member data below.";
			this.usersHeader.Dock = System.Windows.Forms.DockStyle.Top;
			this.usersHeader.Image = global::MOG_ServerManager.Properties.Resources.MOG_Mail;
			this.usersHeader.Location = new System.Drawing.Point(0, 0);
			this.usersHeader.Name = "usersHeader";
			this.usersHeader.Size = new System.Drawing.Size(1023, 60);
			this.usersHeader.TabIndex = 6;
			this.usersHeader.Title = "Users (3 of 4)";
			// 
			// tpAssetPlacer
			// 
			this.tpAssetPlacer.Controls.Add(this.cbAssetExtensions);
			this.tpAssetPlacer.Controls.Add(this.assetImportPlacer);
			this.tpAssetPlacer.Controls.Add(this.label1);
			this.tpAssetPlacer.Controls.Add(this.lblAssetPlacerStep2);
			this.tpAssetPlacer.Controls.Add(this.lblAssetPlacerStep1);
			this.tpAssetPlacer.Controls.Add(this.lblStep3);
			this.tpAssetPlacer.Controls.Add(this.lblStep2);
			this.tpAssetPlacer.Controls.Add(this.lblStep1);
			this.tpAssetPlacer.Controls.Add(this.asserPlacerHeader);
			this.tpAssetPlacer.Location = new System.Drawing.Point(4, 22);
			this.tpAssetPlacer.Name = "tpAssetPlacer";
			this.tpAssetPlacer.Size = new System.Drawing.Size(1023, 466);
			this.tpAssetPlacer.TabIndex = 6;
			this.tpAssetPlacer.Text = "AssetPlacer";
			this.tpAssetPlacer.UseVisualStyleBackColor = true;
			// 
			// cbAssetExtensions
			// 
			this.cbAssetExtensions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cbAssetExtensions.AutoSize = true;
			this.cbAssetExtensions.Location = new System.Drawing.Point(727, 446);
			this.cbAssetExtensions.Name = "cbAssetExtensions";
			this.cbAssetExtensions.Size = new System.Drawing.Size(272, 17);
			this.cbAssetExtensions.TabIndex = 37;
			this.cbAssetExtensions.Text = "Include extensions in asset names when adding files";
			this.cbAssetExtensions.UseVisualStyleBackColor = true;
			this.cbAssetExtensions.CheckedChanged += new System.EventHandler(this.cbAssetExtensions_CheckedChanged);
			// 
			// assetImportPlacer
			// 
			this.assetImportPlacer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.assetImportPlacer.IncludeRootDir = false;
			this.assetImportPlacer.Location = new System.Drawing.Point(8, 104);
			this.assetImportPlacer.Name = "assetImportPlacer";
			this.assetImportPlacer.Platforms = ((System.Collections.ArrayList)(resources.GetObject("assetImportPlacer.Platforms")));
			this.assetImportPlacer.ProjectName = "Project Repository";
			this.assetImportPlacer.ProjectPath = "C:\\My Project Import Folder Here";
			this.assetImportPlacer.Size = new System.Drawing.Size(1007, 356);
			this.assetImportPlacer.TabIndex = 36;
			this.assetImportPlacer.UseFileExtensionsInAssetNames = false;
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.SystemColors.Window;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(680, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(48, 16);
			this.label1.TabIndex = 35;
			this.label1.Text = "Step 3:";
			// 
			// lblAssetPlacerStep2
			// 
			this.lblAssetPlacerStep2.BackColor = System.Drawing.SystemColors.Window;
			this.lblAssetPlacerStep2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblAssetPlacerStep2.Location = new System.Drawing.Point(352, 24);
			this.lblAssetPlacerStep2.Name = "lblAssetPlacerStep2";
			this.lblAssetPlacerStep2.Size = new System.Drawing.Size(48, 16);
			this.lblAssetPlacerStep2.TabIndex = 34;
			this.lblAssetPlacerStep2.Text = "Step 2:";
			// 
			// lblAssetPlacerStep1
			// 
			this.lblAssetPlacerStep1.BackColor = System.Drawing.SystemColors.Window;
			this.lblAssetPlacerStep1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblAssetPlacerStep1.Location = new System.Drawing.Point(24, 24);
			this.lblAssetPlacerStep1.Name = "lblAssetPlacerStep1";
			this.lblAssetPlacerStep1.Size = new System.Drawing.Size(48, 16);
			this.lblAssetPlacerStep1.TabIndex = 6;
			this.lblAssetPlacerStep1.Text = "Step 1:";
			// 
			// lblStep3
			// 
			this.lblStep3.BackColor = System.Drawing.SystemColors.Window;
			this.lblStep3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblStep3.Location = new System.Drawing.Point(728, 24);
			this.lblStep3.Name = "lblStep3";
			this.lblStep3.Size = new System.Drawing.Size(232, 40);
			this.lblStep3.TabIndex = 33;
			this.lblStep3.Text = "Review the Project Repository, adding, removing, or moving Classifications, Asset" +
				"s and files as necessary";
			// 
			// lblStep2
			// 
			this.lblStep2.BackColor = System.Drawing.SystemColors.Window;
			this.lblStep2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblStep2.Location = new System.Drawing.Point(400, 24);
			this.lblStep2.Name = "lblStep2";
			this.lblStep2.Size = new System.Drawing.Size(232, 40);
			this.lblStep2.TabIndex = 32;
			this.lblStep2.Text = "Organize your project files below by dragging and dropping the files marked red i" +
				"nto the Project Repository";
			// 
			// lblStep1
			// 
			this.lblStep1.BackColor = System.Drawing.SystemColors.Window;
			this.lblStep1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblStep1.Location = new System.Drawing.Point(72, 24);
			this.lblStep1.Name = "lblStep1";
			this.lblStep1.Size = new System.Drawing.Size(232, 40);
			this.lblStep1.TabIndex = 31;
			this.lblStep1.Text = "Specify the location of  the project you would like to import and check the files" +
				" and folders that should be imported as assets";
			// 
			// asserPlacerHeader
			// 
			this.asserPlacerHeader.BackColor = System.Drawing.SystemColors.Window;
			this.asserPlacerHeader.Description = "";
			this.asserPlacerHeader.Dock = System.Windows.Forms.DockStyle.Top;
			this.asserPlacerHeader.Image = global::MOG_ServerManager.Properties.Resources.MOG_Import;
			this.asserPlacerHeader.Location = new System.Drawing.Point(0, 0);
			this.asserPlacerHeader.Name = "asserPlacerHeader";
			this.asserPlacerHeader.Size = new System.Drawing.Size(1023, 88);
			this.asserPlacerHeader.TabIndex = 6;
			this.asserPlacerHeader.Title = "Import Project Assets (4 of 4)";
			// 
			// tpFinish
			// 
			this.tpFinish.Controls.Add(this.lblImportTime);
			this.tpFinish.Controls.Add(this.lblImportTimeLabel);
			this.tpFinish.Controls.Add(this.lblNumDepartments);
			this.tpFinish.Controls.Add(this.label11);
			this.tpFinish.Controls.Add(this.lblNumAssets);
			this.tpFinish.Controls.Add(this.label10);
			this.tpFinish.Controls.Add(this.lblNumClassifications);
			this.tpFinish.Controls.Add(this.label8);
			this.tpFinish.Controls.Add(this.lblNumPlatforms);
			this.tpFinish.Controls.Add(this.lblNumUsers);
			this.tpFinish.Controls.Add(this.label5);
			this.tpFinish.Controls.Add(this.label4);
			this.tpFinish.Controls.Add(this.label2);
			this.tpFinish.Controls.Add(this.finishHeader);
			this.tpFinish.Location = new System.Drawing.Point(4, 22);
			this.tpFinish.Name = "tpFinish";
			this.tpFinish.Size = new System.Drawing.Size(1023, 466);
			this.tpFinish.TabIndex = 9;
			this.tpFinish.Text = "FinishPage";
			this.tpFinish.UseVisualStyleBackColor = true;
			// 
			// lblImportTime
			// 
			this.lblImportTime.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblImportTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblImportTime.Location = new System.Drawing.Point(447, 302);
			this.lblImportTime.Name = "lblImportTime";
			this.lblImportTime.Size = new System.Drawing.Size(296, 16);
			this.lblImportTime.TabIndex = 21;
			// 
			// lblImportTimeLabel
			// 
			this.lblImportTimeLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblImportTimeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblImportTimeLabel.Location = new System.Drawing.Point(287, 302);
			this.lblImportTimeLabel.Name = "lblImportTimeLabel";
			this.lblImportTimeLabel.Size = new System.Drawing.Size(144, 16);
			this.lblImportTimeLabel.TabIndex = 20;
			this.lblImportTimeLabel.Text = "Estimated Import Time:";
			this.lblImportTimeLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// lblNumDepartments
			// 
			this.lblNumDepartments.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblNumDepartments.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblNumDepartments.Location = new System.Drawing.Point(447, 254);
			this.lblNumDepartments.Name = "lblNumDepartments";
			this.lblNumDepartments.Size = new System.Drawing.Size(296, 16);
			this.lblNumDepartments.TabIndex = 19;
			// 
			// label11
			// 
			this.label11.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label11.Location = new System.Drawing.Point(287, 254);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(144, 16);
			this.label11.TabIndex = 18;
			this.label11.Text = "Number of Departments:";
			this.label11.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// lblNumAssets
			// 
			this.lblNumAssets.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblNumAssets.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblNumAssets.Location = new System.Drawing.Point(447, 286);
			this.lblNumAssets.Name = "lblNumAssets";
			this.lblNumAssets.Size = new System.Drawing.Size(296, 16);
			this.lblNumAssets.TabIndex = 17;
			// 
			// label10
			// 
			this.label10.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label10.Location = new System.Drawing.Point(287, 286);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(144, 16);
			this.label10.TabIndex = 16;
			this.label10.Text = "Number of Assets:";
			this.label10.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// lblNumClassifications
			// 
			this.lblNumClassifications.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblNumClassifications.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblNumClassifications.Location = new System.Drawing.Point(447, 270);
			this.lblNumClassifications.Name = "lblNumClassifications";
			this.lblNumClassifications.Size = new System.Drawing.Size(296, 16);
			this.lblNumClassifications.TabIndex = 15;
			// 
			// label8
			// 
			this.label8.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label8.Location = new System.Drawing.Point(287, 270);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(144, 16);
			this.label8.TabIndex = 14;
			this.label8.Text = "Number of Classifications:";
			this.label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// lblNumPlatforms
			// 
			this.lblNumPlatforms.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblNumPlatforms.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblNumPlatforms.Location = new System.Drawing.Point(447, 222);
			this.lblNumPlatforms.Name = "lblNumPlatforms";
			this.lblNumPlatforms.Size = new System.Drawing.Size(296, 16);
			this.lblNumPlatforms.TabIndex = 13;
			// 
			// lblNumUsers
			// 
			this.lblNumUsers.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblNumUsers.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblNumUsers.Location = new System.Drawing.Point(447, 238);
			this.lblNumUsers.Name = "lblNumUsers";
			this.lblNumUsers.Size = new System.Drawing.Size(296, 16);
			this.lblNumUsers.TabIndex = 12;
			// 
			// label5
			// 
			this.label5.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.Location = new System.Drawing.Point(287, 238);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(144, 16);
			this.label5.TabIndex = 10;
			this.label5.Text = "Number of Users:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label4
			// 
			this.label4.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(287, 222);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(144, 16);
			this.label4.TabIndex = 9;
			this.label4.Text = "Platforms:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label2
			// 
			this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(431, 190);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(96, 16);
			this.label2.TabIndex = 8;
			this.label2.Text = "Project Stats";
			// 
			// finishHeader
			// 
			this.finishHeader.BackColor = System.Drawing.SystemColors.Window;
			this.finishHeader.Description = "";
			this.finishHeader.Dock = System.Windows.Forms.DockStyle.Top;
			this.finishHeader.Image = global::MOG_ServerManager.Properties.Resources.MOG_Completed_;
			this.finishHeader.Location = new System.Drawing.Point(0, 0);
			this.finishHeader.Name = "finishHeader";
			this.finishHeader.Size = new System.Drawing.Size(1023, 60);
			this.finishHeader.TabIndex = 7;
			this.finishHeader.Title = "Finished!";
			// 
			// assetClassificationConfigControl
			// 
			this.assetClassificationConfigControl.AddClassificationMenuItemVisible = true;
			this.assetClassificationConfigControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.assetClassificationConfigControl.ConfigurationVisible = true;
			this.assetClassificationConfigControl.ImmediateMode = false;
			this.assetClassificationConfigControl.Location = new System.Drawing.Point(16, 72);
			this.assetClassificationConfigControl.Name = "assetClassificationConfigControl";
			this.assetClassificationConfigControl.ProjectRootPath = "";
			this.assetClassificationConfigControl.RemoveClassificationMenuItemVisible = true;
			this.assetClassificationConfigControl.ShowConfigurationMenuItemVisible = true;
			this.assetClassificationConfigControl.Size = new System.Drawing.Size(991, 406);
			this.assetClassificationConfigControl.TabIndex = 7;
			// 
			// lblProjectType
			// 
			this.lblProjectType.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblProjectType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblProjectType.Location = new System.Drawing.Point(423, 243);
			this.lblProjectType.Name = "lblProjectType";
			this.lblProjectType.Size = new System.Drawing.Size(160, 16);
			this.lblProjectType.TabIndex = 19;
			this.lblProjectType.Text = "What would you like to do?";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(100, 100);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 23);
			this.label3.TabIndex = 0;
			this.label3.Text = "LABEL3";
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnCancel.Location = new System.Drawing.Point(943, 510);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 5;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnNext
			// 
			this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnNext.Location = new System.Drawing.Point(863, 510);
			this.btnNext.Name = "btnNext";
			this.btnNext.Size = new System.Drawing.Size(75, 23);
			this.btnNext.TabIndex = 4;
			this.btnNext.Text = "Next >";
			this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
			// 
			// label6
			// 
			this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label6.Location = new System.Drawing.Point(24, 40);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(400, 24);
			this.label6.TabIndex = 1;
			this.label6.Text = "LABEL6";
			// 
			// btnRemoveAsset2
			// 
			this.btnRemoveAsset2.Location = new System.Drawing.Point(328, 96);
			this.btnRemoveAsset2.Name = "btnRemoveAsset2";
			this.btnRemoveAsset2.Size = new System.Drawing.Size(80, 23);
			this.btnRemoveAsset2.TabIndex = 19;
			this.btnRemoveAsset2.Text = "Remove";
			// 
			// columnHeader6
			// 
			this.columnHeader6.Text = "Configured?";
			this.columnHeader6.Width = 75;
			// 
			// columnHeader5
			// 
			this.columnHeader5.Text = "Asset Name";
			this.columnHeader5.Width = 187;
			// 
			// lvUnconfiguredAssets
			// 
			this.lvUnconfiguredAssets.FullRowSelect = true;
			this.lvUnconfiguredAssets.Location = new System.Drawing.Point(32, 40);
			this.lvUnconfiguredAssets.MultiSelect = false;
			this.lvUnconfiguredAssets.Name = "lvUnconfiguredAssets";
			this.lvUnconfiguredAssets.Size = new System.Drawing.Size(280, 232);
			this.lvUnconfiguredAssets.TabIndex = 17;
			this.lvUnconfiguredAssets.UseCompatibleStateImageBehavior = false;
			this.lvUnconfiguredAssets.View = System.Windows.Forms.View.Details;
			// 
			// btnBack
			// 
			this.btnBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnBack.Location = new System.Drawing.Point(783, 510);
			this.btnBack.Name = "btnBack";
			this.btnBack.Size = new System.Drawing.Size(75, 23);
			this.btnBack.TabIndex = 0;
			this.btnBack.Text = "< Back";
			this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
			// 
			// pictureBox4
			// 
			this.pictureBox4.Location = new System.Drawing.Point(584, 8);
			this.pictureBox4.Name = "pictureBox4";
			this.pictureBox4.Size = new System.Drawing.Size(80, 80);
			this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox4.TabIndex = 0;
			this.pictureBox4.TabStop = false;
			// 
			// ConfigureProjectWizardForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(1031, 574);
			this.ControlBox = false;
			this.Controls.Add(this.btnBack);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnNext);
			this.Controls.Add(this.tabControl);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MinimumSize = new System.Drawing.Size(1039, 582);
			this.Name = "ConfigureProjectWizardForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Configure New Project Wizard";
			this.Resize += new System.EventHandler(this.ConfigureProjectWizardForm_Resize);
			this.tabControl.ResumeLayout(false);
			this.tpPlatforms.ResumeLayout(false);
			this.tpDepartments.ResumeLayout(false);
			this.tpUsers.ResumeLayout(false);
			this.tpAssetPlacer.ResumeLayout(false);
			this.tpAssetPlacer.PerformLayout();
			this.tpFinish.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion
		#endregion

		private MOG_Project loadedProject;

		#region Constructors
		public ConfigureProjectWizardForm(MOG_Project proj)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.loadedProject = proj;

			// load up the platform editor with some default platforms
			platformEditor.LoadDefaults();
			platformEditor.LoadFromProject(loadedProject);
		
			this.tabControl.SelectedTab = this.tpPlatforms;
			PlatformsPageInit();
			this.platformEditor.FocusNewPlatformTextBox();

			this.assetClassificationConfigControl.ShowConfigurationMenuItemVisible = false;
			this.assetClassificationConfigControl.AddClassificationMenuItemVisible = false;
			this.assetClassificationConfigControl.RemoveClassificationMenuItemVisible = false;

			this.assetImportPlacer.ProjectName = this.loadedProject.GetProjectName();

			// Load the default template
			string defaultTemplatePath = MOG_ControllerSystem.LocateInstallItem("ProjectTemplates\\Default");
			if (defaultTemplatePath.Length == 0)
			{
				// Fallback to using the library template
				defaultTemplatePath = MOG_ControllerSystem.LocateInstallItem("ProjectTemplates\\Library");
			}
			this.assetImportPlacer.LoadDefaultConfigurationsFromFiles(defaultTemplatePath);
		}
		#endregion
		#region Public functions
		#endregion
		#region Private functions

		private int CountClassifications_Recursive(TreeNodeCollection nodes)
		{
			int count = 0;

			foreach (AssetTreeNode atn in nodes)
			{
				if (atn.IsAClassification)
				{
					++count;
					count += CountClassifications_Recursive(atn.Nodes);
				}
			}

			return count;
		}

		private void AddNewClasses(TreeNodeCollection nodes)
		{
			ProgressDialog progress = new ProgressDialog("Creating Classification Tree", "Please wait while the Classification Tree is created", AddNewClasses_Worker, nodes, true);
			progress.ShowDialog(this);
		}

		private void AddNewClasses_Worker(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			TreeNodeCollection nodes = e.Argument as TreeNodeCollection;
			int classificationTotal = CountClassifications_Recursive(nodes);

			MogUtil_ClassificationLoader classLoader = new MogUtil_ClassificationLoader();
			MOG_Project project = MOG_ControllerProject.GetProject();
			classLoader.ProjectName = project.GetProjectName();
			ArrayList templateProperties = classLoader.GetClassPropertiesListFromFiles(assetImportPlacer.CurrentTemplatePath);

			AddNewClasses(worker, 0, classificationTotal, project, nodes, templateProperties);
		}

		public int AddNewClasses(BackgroundWorker worker, int classificationProgress, int classificationTotal, MOG_Project project, TreeNodeCollection nodes, ArrayList templateProperties)
		{
			foreach (AssetTreeNode atn in nodes)
			{
				if (atn.IsAClassification && !atn.InDB)
				{
					string classFullName = atn.FullPath.Replace(atn.TreeView.PathSeparator, "~");

					worker.ReportProgress(classificationProgress++ * 100 / classificationTotal, "Creating " + classFullName);

					project.ClassificationAdd(classFullName);
					MOG_Properties properties = project.GetClassificationProperties(classFullName);
					properties.SetImmeadiateMode(true);

					foreach (MOG_Properties templateProps in templateProperties)
					{
						if (String.Compare(templateProps.Classification, classFullName, true) == 0)
						{
							properties.SetProperties(templateProps.GetPropertyList());
							break;
						}
					}

					//If all the kids have the same sync target, we're setting the classification sync target to the same one
					string commonSyncTarget = FindCommonSyncTarget(atn);
					if (commonSyncTarget != "")
					{
						atn.FileFullPath = commonSyncTarget;
					}

					atn.InDB = true;
				}

				// recurse
				classificationProgress = AddNewClasses(worker, classificationProgress, classificationTotal, project, atn.Nodes, templateProperties);
			}

			return classificationProgress;
		}

		public string FindCommonSyncTarget(AssetTreeNode classification)
		{
			bool bAssetFilenameExists = false;
			bool bCommonSyncTargetExists = false;
			string commonSyncTarget = "";

			//Find out if we have at least one asset filename node in this tree
			foreach (AssetTreeNode assetnode in classification.Nodes)
			{
				if (assetnode.IsAnAssetFilename)
				{
					bAssetFilenameExists = true;
					break;
				}
			}

			if (bAssetFilenameExists)
			{
				//There is at least one asset filename node, so let's see if they all share a sync target
				commonSyncTarget = null;
				bCommonSyncTargetExists = true;

				foreach (AssetTreeNode assetnode in classification.Nodes)
				{
					if (!bCommonSyncTargetExists)
					{
						//There is no hope of findong a common sync target
						break;
					}
					
					if (assetnode.IsAnAssetFilename)
					{
						//Check all the file nodes in the asset node
						foreach (AssetTreeNode filenode in assetnode.fileNodes)
						{
							try
							{
								string testSyncTarget = Path.GetDirectoryName(filenode.FileFullPath);

								if (commonSyncTarget != null)
								{
									if (String.Compare(testSyncTarget, commonSyncTarget, true) != 0)
									{
										//The sync target is different, no commonality, no fun, let's go
										bCommonSyncTargetExists = false;
										break;
									}
								}

								commonSyncTarget = testSyncTarget;
							}
							catch
							{
								bCommonSyncTargetExists = false;
								break;
							}
						}
					}
				}
			}

			if (bCommonSyncTargetExists)
			{
				//we got a common sync target!
				return commonSyncTarget;
			}
			else
			{
				//"" means there is nothing common
				return "";
			}
		}

		private bool Finish()
		{
			loadedProject.SuppressProjectRefreshEvent();
			
			//
			// platforms
			foreach (string pName in this.loadedProject.GetPlatformNames())
			{
				if ( !this.platformEditor.ContainsPlatform(pName) )
					this.loadedProject.PlatformRemove(pName);
			}

			foreach (MOG_Platform p in this.platformEditor.MOG_Platforms)
			{
				if (this.loadedProject.GetPlatform(p.mPlatformName) == null)
					this.loadedProject.PlatformAdd( p );
			}

			//
			// classifications and assets

			// munge class icons to some local defaults before we copy
			foreach (MOG_Properties props in this.assetClassificationConfigControl.RootMOG_Properties)
			{
				if (props.Classification.ToLower().EndsWith("textures"))
					props.ClassIcon = MOG_ControllerSystem.LocateInstallItem("SystemImages\\AssetTypes\\texture_file.bmp");
				else if (props.Classification.ToLower().EndsWith("animations"))
					props.ClassIcon = MOG_ControllerSystem.LocateInstallItem("SystemImages\\AssetTypes\\anim_file.bmp");
				else if (props.Classification.ToLower().EndsWith("models"))
					props.ClassIcon = MOG_ControllerSystem.LocateInstallItem("SystemImages\\AssetTypes\\model_file.bmp");
				else if (props.Classification.ToLower().EndsWith("packages"))
					props.ClassIcon = MOG_ControllerSystem.LocateInstallItem("SystemImages\\AssetTypes\\package_file.bmp");
				else if (props.Classification.ToLower().EndsWith("tools"))
					props.ClassIcon = MOG_ControllerSystem.LocateInstallItem("SystemImages\\AssetTypes\\tools_file.bmp");
				else if (props.Classification.ToLower().EndsWith("sounds"))
					props.ClassIcon = MOG_ControllerSystem.LocateInstallItem("SystemImages\\AssetTypes\\audio_file.bmp");
				else if (props.Classification.ToLower().EndsWith("music"))
					props.ClassIcon = MOG_ControllerSystem.LocateInstallItem("SystemImages\\AssetTypes\\music_file.bmp");
			}

			// copy images to project dir
			if (!Directory.Exists(loadedProject.GetProjectToolsPath() + "\\Images"))
				Directory.CreateDirectory(loadedProject.GetProjectToolsPath() + "\\Images");
			this.assetClassificationConfigControl.CopyIcons(loadedProject.GetProjectToolsPath() + "\\Images");

			// Set the active project
			MOG_ControllerProject.LoginProject(loadedProject.GetProjectName(), "");

			// fill in missing asset configs
			Hide();
			this.assetClassificationConfigControl.EncodeAll();

			// write imported assets to INIs/DB
			this.assetClassificationConfigControl.CreateAssetConfigs();

			this.loadedProject.Save();

			loadedProject.EnableProjectRefreshEvent();
			
			return true;
		}

		#endregion
		#region Wizard tab page init functions
		private void PlatformsPageInit()
		{
			this.platformEditor.FocusNewPlatformTextBox();
		}
		private void DepartmentsPageInit()
		{
			this.departmentManager.LoadFromProject(loadedProject);
		}
		private void UsersPageInit()
		{
			this.userManager.LoadFromProject(loadedProject);
		}
		private void AssetPlacerPageInit()
		{
			// set the platforms correctly
			this.assetImportPlacer.Platforms = this.platformEditor.PlatformNames;
		}
		private void AssetConfigurerInit()
		{
			AddNewClasses(this.assetImportPlacer.AssetTreeNodes);
			this.assetClassificationConfigControl.ProjectRootPath = this.assetImportPlacer.ProjectPath;
			this.assetClassificationConfigControl.LoadAssetTree(this.assetImportPlacer.AssetTreeNodes);
		}
		private void FinishPageInit()
		{
			this.lblNumUsers.Text = this.userManager.UserCount.ToString();
			this.lblNumDepartments.Text = this.userManager.DepartmentCount.ToString();

			this.lblNumPlatforms.Text = "";
			foreach (string platName in this.platformEditor.PlatformNames)
				this.lblNumPlatforms.Text += platName + ", ";
			this.lblNumPlatforms.Text = this.lblNumPlatforms.Text.Substring(0, this.lblNumPlatforms.Text.Length - 2);

			this.lblNumClassifications.Text = this.assetClassificationConfigControl.NumClassifications.ToString();
			this.lblNumAssets.Text = this.assetClassificationConfigControl.NumAssets.ToString();

			// calculate import time
			string importTimeText = "";
			int assets = this.assetClassificationConfigControl.NumAssets;
			// Assuming we can import approximately 5 asset/sec
			int seconds = assets / 5;
			int minutes = 0;
			if (seconds >= 60)
			{
				minutes = seconds / 60;
				seconds %= 60;
			}
			int hours = 0;
			if (minutes >= 60)
			{
				hours = minutes / 60;
				minutes %= 60;
			}

			if (hours > 0)
				importTimeText = hours.ToString() + " hours ";
			if (minutes > 0)
				importTimeText += minutes.ToString() + " minutes ";
			if (seconds > 0)
				importTimeText += seconds.ToString() + " seconds";

			this.lblImportTime.Text = importTimeText;
		}
		#endregion
		#region Event handlers

		private void btnNext_Click(object sender, System.EventArgs e)
		{
			if (tabControl.SelectedIndex >= tabControl.TabCount-1)
			{
				if (Finish())
				{
					Close();
				}
			}
			else
			{
				tabControl.SelectedIndex++;
			}
		}

		private void btnBack_Click(object sender, EventArgs e)
		{
			if (tabControl.SelectedIndex > 0)
			{
				tabControl.SelectedIndex--;
			}
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void tabControl_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			btnBack.Enabled = tabControl.SelectedIndex != 0;
			btnNext.Text = (tabControl.SelectedTab == tpFinish) ? "Finish" : "Next >";

			if (tabControl.SelectedTab == tpPlatforms)
			{
				PlatformsPageInit();
			}
			else if (tabControl.SelectedTab == tpDepartments)
			{
				DepartmentsPageInit();
			}
			else if (tabControl.SelectedTab == tpUsers)
			{
				UsersPageInit();
			}
			else if (tabControl.SelectedTab == tpAssetPlacer)
			{
				AssetPlacerPageInit();
			}
			else if (tabControl.SelectedTab == tpFinish)
			{
				AssetConfigurerInit();
				FinishPageInit();
			}
		}

		private void ConfigureProjectWizardForm_Resize(object sender, System.EventArgs e)
		{
			if( this.Owner != null )
			{
				this.Location = MOG_Prompt.GetCenteredLocation( this.Owner, this.Size );
			}
		}

		private void cbAssetExtensions_CheckedChanged(object sender, EventArgs e)
		{
			assetImportPlacer.UseFileExtensionsInAssetNames = cbAssetExtensions.Checked;
		}
	}

	#endregion

	#region Supporting classes and enums

	class SlaveMachineListViewItem : ListViewItem
	{
		public bool willBuild;
		public bool willPackage;

		public BoolArrayList assetSettings;

		public SlaveMachineListViewItem(string name, bool willBuild, bool willPackage) : base(name)
		{
			this.willBuild = willBuild;
			this.willPackage = willPackage;
			this.assetSettings = new BoolArrayList();
		}

		public void AddBoolValue(string name, bool b) 
		{
			this.assetSettings.AddBool( new BoolValue(name, b) );
		}

		public bool GetBool(string name) 
		{
			return this.assetSettings.GetBool(name);
		}

		public void SetBool(string name, bool b)
		{
			this.assetSettings.Get(name).b = b;
		}
	}

	class BoolArrayList : ArrayList 
	{
		public void AddBool(BoolValue b) 
		{
			this.Add(b);
		}

		public BoolValue Get(string name)
		{
			foreach (BoolValue bVal in this)
			{
				if (name.ToLower() == bVal.name.ToLower())
					return bVal;
			}

			return new BoolValue("ERROR!!", false);
		}

		public bool GetBool(string name)
		{
			return this.Get(name).b;
		}
	}

	class BoolValue 
	{
		public string name;
		public bool b;

		public BoolValue(string name, bool b)
		{
			this.name = name;
			this.b = b;
		}
	}

	class UserGroupArrayList : ArrayList 
	{
		public UserGroup Get(string name) 
		{
			foreach (UserGroup group in this) 
			{
				if (group.name.ToLower().Equals(name.ToLower()))
					return group;
			}
			
			return null;
		}
	}

	class UserGroup 
	{
		public string name;
		public ArrayList users;

		public UserGroup(string name)
		{
			this.name = name;
			this.users = new ArrayList();
		}
	}

	class User
	{
		public string name;
		public string emailAddress;
		public string blessTarget;

		public User(string name)
		{
			this.name = name;
			this.emailAddress = "";
			this.blessTarget = "MasterData";
		}

		public User(string name, string emailAddress, string blessTarget)
		{
			this.name = name;
			this.emailAddress = emailAddress;
			this.blessTarget = blessTarget;
		}
	}


	class AssetKeyArrayList : ArrayList 
	{
		public AssetKey Get(string name) 
		{
			foreach (AssetKey key in this)
				if ( key.name.ToLower().Equals( name.ToLower() ) )
					return key;
			return null;
		}
	}

	class AssetPackageArrayList : ArrayList 
	{
		public AssetPackage Get(string name) 
		{
			foreach (AssetPackage package in this)
				if ( package.name.ToLower().Equals( name.ToLower() ) )
					return package;
			return null;
		}
	}
	
	class AssetGroupArrayList : ArrayList 
	{
		public AssetGroup Get(string name) 
		{
			foreach (AssetGroup group in this)
				if ( group.name.ToLower().Equals( name.ToLower() ) )
					return group;
			return null;
		}
	}

	class AssetKey 
	{
		public string name;
		public AssetPackageArrayList packages;
		public bool configured;

		public AssetKey(string name)
		{
			this.name = name;
			this.packages = new AssetPackageArrayList();
			this.configured = false;
		}

		public override bool Equals(object obj)
		{
			if ( ((AssetKey)obj).name.ToLower().Equals( this.name.ToLower() ) )
				return true;

			return false;
		}

		public override int GetHashCode() {	return base.GetHashCode ();	}
	}

	class AssetPackage
	{
		public string name;
		public AssetGroupArrayList groups;

		public AssetPackage(string name)
		{
			this.name = name;
			this.groups = new AssetGroupArrayList();
		}
		
		public override bool Equals(object obj)
		{
			if ( ((AssetPackage)obj).name.ToLower().Equals( this.name.ToLower() ) )
				return true;

			return false;
		}

		public override int GetHashCode() {	return base.GetHashCode ();	}
	}
	
	class AssetGroup
	{
		public string name;

		public AssetGroup(string name)
		{
			this.name = name;
		}

		public override bool Equals(object obj)
		{
			if ( ((AssetGroup)obj).name.ToLower().Equals( this.name.ToLower() ) )
				return true;

			return false;
		}

		public override int GetHashCode() {	return base.GetHashCode ();	}

	}
	#endregion
}
