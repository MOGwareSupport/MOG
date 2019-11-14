using System;
using System.IO;
using System.Collections;
using System.Windows.Forms;

using Server_Gui;
using MOG_Server.Server_Gui.guiConfigurationsHelpers;

using MOG;
using MOG.INI;
using MOG.SYSTEM;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;



namespace MOG_Server.Server_Gui
{
	/// <summary>
	/// Summary description for Configuration.
	/// </summary>
	public class Configuration
	{
		public FormMainSMOG mainForm;
		public guiProjectOptions mProjectOptions;

		public Configuration(FormMainSMOG main)
		{
			mainForm = main;
			mProjectOptions = new guiProjectOptions(this);

			InitializeConfigurations();
		}

		public void InitializeConfigurations()
		{
			// Clear all the projects
			mainForm.AutoBuildOptionsProjectComboBox.Items.Clear();
			mainForm.ConfigProjectsListView.Items.Clear();

			// Get all the projects listed in the mog system
			foreach (string projectName in MOG_ControllerSystem.GetSystem().GetProjectNames())
			{
				ListViewItem project = new ListViewItem();

				project.Text = projectName;

				mainForm.ConfigProjectsListView.Items.Add(project);

				// Populate the AutoBuild project comboBox
                mainForm.AutoBuildOptionsProjectComboBox.Items.Add(projectName);

				//project.SubItems.Add(lMog.GetSystem().);
			}
		}

		public void ProjectCreate()
		{
			NewProjectForm npf = new NewProjectForm();

			if (npf.ShowDialog() == DialogResult.OK && MessageBox.Show("Are you sure you want to add project '" + npf.NewProjectNameTextBox.Text + "'?", "Confirm project add", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				string projectName = npf.NewProjectNameTextBox.Text;
				MOG_ControllerSystem.GetSystem().ProjectCreate(projectName);

				// Refresh the MOG system
				MOG_ControllerSystem.GetSystem().GetConfigFile().Load();

				InitializeConfigurations();
			}
		}

		public void ProjectRemove() 
		{
			// TODO JKB move some of this into the dll?


			if (mainForm.ConfigProjectsListView.SelectedItems.Count<=0) 
			{
				return;
			}

			string projectName=mainForm.ConfigProjectsListView.SelectedItems[0].Text;

			// remove project called 'projectName' from MOG
			if ( MessageBox.Show( string.Concat("Are you sure you want to remove project '",projectName,"'?"),"Confirm delete",MessageBoxButtons.YesNo ) == DialogResult.No )
			{
				return;
			}

//			MOG_ControllerSystem.GetSystem().ProjectRemove(projectName);

			// TODO: Put this in MOG_System.cpp
			// mark it as deleted in the INI file
			MOG_Ini ini = MOG_ControllerSystem.GetSystem().GetConfigFile();
			ini.RemoveString("projects", projectName);
			ini.PutString("projects.deleted", projectName, "");
			string configFile = ini.GetString(projectName, "ConfigFile");
			string projPath = MOG_ControllerProject.GetProject().GetProjectPath();
			configFile = configFile.Replace(projPath, string.Concat(projPath, ".deleted"));
			ini.RemoveSection(projectName);
			ini.PutString( string.Concat(projectName, ".deleted"), "ConfigFile", configFile );
			MOG_ControllerSystem.GetSystem().GetProjectNames().Remove(projectName);

			ini.Save();
			
			// rename directory
			string dirName = MOG_ControllerProject.GetProject().GetProjectPath();
			if (Directory.Exists(dirName) && !Directory.Exists(string.Concat(dirName, ".deleted")))
			{
				Directory.Move(dirName, string.Concat(dirName, ".deleted"));
			}

			// Refresh the MOG system
			MOG_ControllerSystem.GetSystem().GetConfigFile().Load();
			InitializeConfigurations();
		}

	}
}



