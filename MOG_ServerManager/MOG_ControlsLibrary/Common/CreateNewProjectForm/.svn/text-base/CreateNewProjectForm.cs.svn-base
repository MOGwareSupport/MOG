using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

//using MOG_Server.Server_Gui.Forms;
//using MOG_Server.Server_Gui.Wizards;

using MOG.DOSUTILS;
using MOG.INI;
using MOG.USER;
using MOG.TIME;
using MOG.REPORT;
using MOG.PROMPT;
using MOG.PROGRESS;
using MOG.SYSTEM;
using MOG.PROJECT;
using MOG.PLATFORM;
using MOG.DATABASE;
using MOG.PROPERTIES;
using MOG.CONTROLLER.CONTROLLERASSET;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERDEMOPROJECT;

using MOG_ServerManager.Utilities;
using MOG_CoreControls;
using System.Collections.Generic;

namespace MOG_ServerManager
{
	/// <summary>
	/// Summary description for CreateNewProjectForm.
	/// </summary>
	public class CreateNewProjectForm : System.Windows.Forms.Form
	{
		#region System defs

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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateNewProjectForm));
            this.btnCreate = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblInfo = new System.Windows.Forms.Label();
            this.warningPicBox = new System.Windows.Forms.PictureBox();
            this.rbNewProject = new System.Windows.Forms.RadioButton();
            this.rbDemoProject = new System.Windows.Forms.RadioButton();
            this.btnImport = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbLaunchClient = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lvDemoProjects = new System.Windows.Forms.ListView();
            this.hdrName = new System.Windows.Forms.ColumnHeader();
            this.hdrDescription = new System.Windows.Forms.ColumnHeader();
            this.hdrPath = new System.Windows.Forms.ColumnHeader();
            this.projectInfoControl = new ProjectInfoControl();
            ((System.ComponentModel.ISupportInitialize)(this.warningPicBox)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCreate
            // 
            this.btnCreate.Enabled = false;
            this.btnCreate.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnCreate.Location = new System.Drawing.Point(411, 309);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(75, 23);
            this.btnCreate.TabIndex = 2;
            this.btnCreate.Text = "Create";
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnCancel.Location = new System.Drawing.Point(411, 344);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            // 
            // lblInfo
            // 
            this.lblInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInfo.Location = new System.Drawing.Point(72, 320);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(264, 40);
            this.lblInfo.TabIndex = 5;
            this.lblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // warningPicBox
            // 
            this.warningPicBox.Image = ((System.Drawing.Image)(resources.GetObject("warningPicBox.Image")));
            this.warningPicBox.Location = new System.Drawing.Point(32, 320);
            this.warningPicBox.Name = "warningPicBox";
            this.warningPicBox.Size = new System.Drawing.Size(32, 32);
            this.warningPicBox.TabIndex = 6;
            this.warningPicBox.TabStop = false;
            this.warningPicBox.Visible = false;
            // 
            // rbNewProject
            // 
            this.rbNewProject.Location = new System.Drawing.Point(14, 184);
            this.rbNewProject.Name = "rbNewProject";
            this.rbNewProject.Size = new System.Drawing.Size(130, 17);
            this.rbNewProject.TabIndex = 7;
            this.rbNewProject.Text = "&Create New Project";
            this.rbNewProject.CheckedChanged += new System.EventHandler(this.Radio_CheckedChanged);
            // 
            // rbDemoProject
            // 
            this.rbDemoProject.Checked = true;
            this.rbDemoProject.Location = new System.Drawing.Point(16, 16);
            this.rbDemoProject.Name = "rbDemoProject";
            this.rbDemoProject.Size = new System.Drawing.Size(226, 17);
            this.rbDemoProject.TabIndex = 8;
            this.rbDemoProject.TabStop = true;
            this.rbDemoProject.Text = "&Import Demo Project";
            this.rbDemoProject.CheckedChanged += new System.EventHandler(this.Radio_CheckedChanged);
            // 
            // btnImport
            // 
            this.btnImport.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnImport.Location = new System.Drawing.Point(411, 157);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(75, 23);
            this.btnImport.TabIndex = 10;
            this.btnImport.Text = "Import";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbLaunchClient);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.rbDemoProject);
            this.groupBox1.Controls.Add(this.rbNewProject);
            this.groupBox1.Controls.Add(this.warningPicBox);
            this.groupBox1.Controls.Add(this.lblInfo);
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.btnCreate);
            this.groupBox1.Controls.Add(this.projectInfoControl);
            this.groupBox1.Controls.Add(this.btnImport);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(504, 382);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            // 
            // cbLaunchClient
            // 
            this.cbLaunchClient.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbLaunchClient.Checked = true;
            this.cbLaunchClient.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbLaunchClient.Location = new System.Drawing.Point(34, 355);
            this.cbLaunchClient.Name = "cbLaunchClient";
            this.cbLaunchClient.Size = new System.Drawing.Size(368, 24);
            this.cbLaunchClient.TabIndex = 13;
            this.cbLaunchClient.Text = "Launch MOG Client after successful project creation";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lvDemoProjects);
            this.groupBox2.Location = new System.Drawing.Point(32, 40);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(456, 112);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Demo Projects";
            // 
            // lvDemoProjects
            // 
            this.lvDemoProjects.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvDemoProjects.CheckBoxes = true;
            this.lvDemoProjects.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.hdrName,
            this.hdrDescription,
            this.hdrPath});
            this.lvDemoProjects.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvDemoProjects.Location = new System.Drawing.Point(16, 16);
            this.lvDemoProjects.Name = "lvDemoProjects";
            this.lvDemoProjects.Size = new System.Drawing.Size(424, 81);
            this.lvDemoProjects.TabIndex = 11;
            this.lvDemoProjects.UseCompatibleStateImageBehavior = false;
            this.lvDemoProjects.View = System.Windows.Forms.View.Details;
            // 
            // hdrName
            // 
            this.hdrName.Width = 121;
            // 
            // hdrDescription
            // 
            this.hdrDescription.Width = 0;
            // 
            // hdrPath
            // 
            this.hdrPath.Width = 0;
            // 
            // projectInfoControl
            // 
            this.projectInfoControl.AddAssetToPackageCommand = "";
            this.projectInfoControl.BuildTool = "";
            this.projectInfoControl.DeletePackageCommand = "";
            this.projectInfoControl.Enabled = false;
            this.projectInfoControl.ImmediateMode = false;
            this.projectInfoControl.Location = new System.Drawing.Point(24, 208);
            this.projectInfoControl.Name = "projectInfoControl";
            this.projectInfoControl.PackageTool = "";
            this.projectInfoControl.ProjectKey = "";
            this.projectInfoControl.ProjectName = "";
            this.projectInfoControl.ProjectNameInSourceControl = "";
            this.projectInfoControl.ProjectNameReadOnly = false;
            this.projectInfoControl.ProjectPath = "";
            this.projectInfoControl.ProjectsPath = "";
            this.projectInfoControl.RemoveAssetFromPackageCommand = "";
            this.projectInfoControl.Size = new System.Drawing.Size(472, 105);
            this.projectInfoControl.TabIndex = 0;
            // 
            // CreateNewProjectForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(504, 382);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateNewProjectForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select your project creation option:";
            ((System.ComponentModel.ISupportInitialize)(this.warningPicBox)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion
		#endregion
		#region Member vars
		private bool executeCreate = true;
        private ProjectInfoControl projectInfoControl;
        private Button btnCreate;
        private Button btnCancel;
        private Label lblInfo;
        private PictureBox warningPicBox;
        private RadioButton rbNewProject;
        private RadioButton rbDemoProject;
        private Button btnImport;
        private GroupBox groupBox1;
        private ListView lvDemoProjects;
		private ColumnHeader hdrName;
		private ColumnHeader hdrDescription;
		private ColumnHeader hdrPath;
		private System.Windows.Forms.GroupBox groupBox2;
		public System.Windows.Forms.CheckBox cbLaunchClient;
		private ArrayList existingProjects = new ArrayList();
		#endregion
		#region Properties
		public ArrayList ExistingProjects
		{
			get { return this.existingProjects; }
			set { this.existingProjects = value; }
		}

		public bool ExecuteCreate
		{
			get { return this.executeCreate; }
			set { this.executeCreate = value; }
		}

		public string ProjectName
		{
			get { return this.projectInfoControl.ProjectName; }
		}
		#endregion
		#region Constructor
		public CreateNewProjectForm()
			: this( null )
		{}

		public CreateNewProjectForm(string defaultProjName)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

            RefreshDemoProjectsListView();
            
            this.projectInfoControl.ProjectsPath = MOG_ControllerSystem.GetSystem().GetSystemProjectsPath();
			this.projectInfoControl.ProjectName_TextChanged += new EventHandler(projectInfoControl_ProjectName_TextChanged);

			this.projectInfoControl.ProjectName = defaultProjName;
			if( defaultProjName != null)
			{
				this.projectInfoControl.ProjectNameSelectAll();
			}
		}
		#endregion
		#region Private functions
		private MOG_Project CreateProject(bool showProgressBar)
		{
			ProgressDialog progress = new ProgressDialog("Creating " + this.projectInfoControl.ProjectName, "Creating...", CreateProject_Worker, null, false);
			progress.ShowDialog(this);

			return progress.WorkerResult as MOG_Project;
		}

		private void CreateProject_Worker(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;

			MOG_Project project = new MOG_Project();

			try
			{
				MOG_System sys = MOG_ControllerSystem.GetSystem();
				if (sys != null)
				{
					project = sys.ProjectCreate(this.projectInfoControl.ProjectName);
					if (project != null)
					{
						worker.ReportProgress(0, "Setting project info...");

						// project info
						project.SetProjectPath(this.projectInfoControl.ProjectPath);
						project.SetProjectToolsPath(this.projectInfoControl.ProjectPath + "\\Tools");
						project.SetProjectUsersPath(this.projectInfoControl.ProjectPath + "\\Users");

						worker.ReportProgress(0, "Copying tools...");

						worker.ReportProgress(0, "Saving and logging in...");

						// create project and login
						project.Save();

						MOG_ControllerProject.LoginProject(this.projectInfoControl.ProjectName, "");

						worker.ReportProgress(0, "Creating current branch...");

						// create default branch
						MOG_ControllerProject.BranchCreate("", "Current");

						worker.ReportProgress(0, "Creating classification tree...");

						// platforms
						MOG_Platform pcPlatform = new MOG_Platform();
						pcPlatform.mPlatformName = "PC";
						project.PlatformAdd(pcPlatform);

						// create classifications
						MogUtil_ClassificationLoader classLoader = new MogUtil_ClassificationLoader();
						classLoader.ProjectName = this.projectInfoControl.ProjectName;
						foreach (MOG_Properties props in classLoader.GetClassPropertiesListFromFiles())
						{
							if (props.Classification.ToLower() == project.GetProjectName().ToLower())
							{
								// create only project name root class node
								project.ClassificationAdd(props.Classification);
								MOG_Properties properties = project.GetClassificationProperties(props.Classification);
								properties.SetImmeadiateMode(true);
								properties.SetProperties(props.GetPropertyList());
								break;
							}
						}

						project.Save();
					}
					else
					{
						throw new Exception("Failed to create the project.");
					}
				}
				else
				{
					throw new Exception("System not initialized.");
				}
			}
			catch (Exception ex)
			{
				MOG_Report.ReportMessage("Project Creation Failed", ex.Message, ex.StackTrace, MOG_ALERT_LEVEL.CRITICAL);
			}

			e.Result = project;
		}

		// checks projName against known project names, both those in the repository and in the database
		public static bool DuplicateProjectNameCheck(string projName, bool verbose)
		{
			// make sure we don't have a project called projName locally
			foreach (string existingProjName in MOG_ControllerSystem.GetSystem().GetProjectNames())
			{
				if (existingProjName.ToLower() == projName.ToLower())
				{
					if (verbose)
					{
						Utils.ShowMessageBoxExclamation("Duplicate project name.\nA project called " + projName + " already exists.", "Duplicate Project Name");
					}

					return false;
				}
			}

			// make sure projName doesn't exist in the tables
			if (MOG_ControllerSystem.GetDB().ProjectTablesExist(projName))
			{
				// allow user to overwrite the tables if he wants
				if (Utils.ShowMessageBoxConfirmation("Project information tables for a project named " + projName + " currently exist in the database.\nWould you like to overwrite these tables?", "Overwrite Duplicate Tables?") == MOGPromptResult.No)
				{
					return false;
				}

				// remove the tables
				MOG_ControllerSystem.GetDB().DeleteProjectTables(projName);
			}

			return true;
		}

		private void RefreshDemoProjectsListView()
		{
			// Load demo project listview
			this.lvDemoProjects.Items.Clear();

			// Get the list of DemoProjects
			ArrayList demoProjectNames = MOG_ControllerDemoProject.GetAllDemoProjectNames();
			foreach (string demoProjectName in demoProjectNames)
			{
				// Setup the listviewitem
				ListViewItem projItem = new ListViewItem(demoProjectName);
				projItem.SubItems.Add("MOG DemoProject");
				this.lvDemoProjects.Items.Add(projItem);
			}

			// do we have any installed demo projects?
			if (lvDemoProjects.Items.Count > 0)
			{
				// By default check the first demo in the demo projects window
				lvDemoProjects.Items[0].Checked = true;
			}
			else
			{
				// We have no installed Demo projects, so lets default to new project
				this.rbNewProject.Checked = true;
			}
		}
		#endregion

		private void projectInfoControl_ProjectName_TextChanged(object sender, EventArgs e)
		{
			if (this.projectInfoControl.ProjectName.Length > 12)
			{
				this.lblInfo.Text = "Project names of over 12 characters are not recommended";
				this.warningPicBox.Visible = true;
			}
			else
			{
				this.lblInfo.Text = "";
				this.warningPicBox.Visible = false;
			}
		}

		private void btnCreate_Click(object sender, System.EventArgs e)
		{
			if (!DuplicateProjectNameCheck( this.projectInfoControl.ProjectName, true ))
			{
				this.projectInfoControl.FocusProjectName();
				return;
			}

			if (this.executeCreate)
			{
				MOG_Project proj = CreateProject(true);
				if (proj == null)
				{
					// If project creation failed, return
					return;
				}

				MOG_ControllerProject.LoginProject(proj.GetProjectName(), "");

				try
				{
					Hide();
					ConfigureProjectWizardForm wiz = new ConfigureProjectWizardForm(proj);
					wiz.ShowDialog(this);
				}
				catch(Exception ex)
				{
					MOG_Prompt.PromptResponse("Configure Error", ex.Message, ex.StackTrace, MOGPromptButtons.OK, MOG_ALERT_LEVEL.CRITICAL);					
				}
			}

			this.DialogResult = DialogResult.OK;
			Hide();
		}

		private void Radio_CheckedChanged(object sender, EventArgs e)
		{
			projectInfoControl.Enabled = rbNewProject.Checked;
			btnCreate.Enabled = rbNewProject.Checked;

			btnImport.Enabled = rbDemoProject.Checked;
			lvDemoProjects.Enabled = rbDemoProject.Checked;
		}

		private void btnImport_Click(object sender, EventArgs e)
		{
			if (lvDemoProjects.Items.Count > 0)
			{
				List<string> projectNames = new List<string>();
				
				foreach (ListViewItem projItem in lvDemoProjects.Items)
				{
					if (projItem.Checked)
					{
						projectNames.Add(projItem.Text);
					}
				}

				ProgressDialog progress = new ProgressDialog("Importing Demo Project", "Please wait...", ImportDemoProject_Worker, projectNames, true);
				DialogResult = progress.ShowDialog(this);
			}
			else
			{
				DialogResult = DialogResult.OK;
			}

			Hide();
		}

		private void ImportDemoProject_Worker(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			List<string> projectNames = e.Argument as List<string>;

			for (int i = 0; i < projectNames.Count && !worker.CancellationPending; i++)
			{
				// Project's path is encoded in its third subitem
				MOG_ControllerDemoProject.ImportDemoProject(projectNames[i], worker);

				worker.ReportProgress(i * 100 / projectNames.Count, "Importing " + projectNames[i]);
			}
		}

		#region Event handlers

	}
		#endregion
}
