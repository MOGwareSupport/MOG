using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using MOG.PROJECT;
using MOG.PROPERTIES;
using MOG.PROGRESS;
using MOG_ControlsLibrary;
using MOG_CoreControls;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using System.IO;


namespace MOG_ServerManager
{
	/// <summary>
	/// Summary description for FileImportForm.
	/// </summary>
	public class FileImportForm : System.Windows.Forms.Form
	{
		#region System defs

		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private AssetImportPlacer assetImportPlacer;
		private CheckBox cbAssetExtensions;
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileImportForm));
			this.assetImportPlacer = new AssetImportPlacer();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.cbAssetExtensions = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// assetImportPlacer
			// 
			this.assetImportPlacer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.assetImportPlacer.IncludeRootDir = true;
			this.assetImportPlacer.Location = new System.Drawing.Point(8, 8);
			this.assetImportPlacer.Name = "assetImportPlacer";
			this.assetImportPlacer.Platforms = ((System.Collections.ArrayList)(resources.GetObject("assetImportPlacer.Platforms")));
			this.assetImportPlacer.ProjectName = "Project Repository";
			this.assetImportPlacer.ProjectPath = "C:\\My Project Import Folder Here";
			this.assetImportPlacer.Size = new System.Drawing.Size(974, 419);
			this.assetImportPlacer.TabIndex = 0;
			this.assetImportPlacer.UseFileExtensionsInAssetNames = false;
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.Location = new System.Drawing.Point(894, 435);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.Location = new System.Drawing.Point(806, 435);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// cbAssetExtensions
			// 
			this.cbAssetExtensions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cbAssetExtensions.AutoSize = true;
			this.cbAssetExtensions.Location = new System.Drawing.Point(694, 412);
			this.cbAssetExtensions.Name = "cbAssetExtensions";
			this.cbAssetExtensions.Size = new System.Drawing.Size(272, 17);
			this.cbAssetExtensions.TabIndex = 3;
			this.cbAssetExtensions.Text = "Include extensions in asset names when adding files";
			this.cbAssetExtensions.UseVisualStyleBackColor = true;
			this.cbAssetExtensions.CheckedChanged += new System.EventHandler(this.cbAssetExtensions_CheckedChanged);
			// 
			// FileImportForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(990, 465);
			this.Controls.Add(this.cbAssetExtensions);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.assetImportPlacer);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(998, 281);
			this.Name = "FileImportForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Asset Importer";
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion
		#endregion
		#region Member vars
		private MOG_Project project;
		#endregion
		#region Constructor
		public FileImportForm(MOG_Project proj)
		{
			InitializeComponent();

			this.project = proj;
			
			this.assetImportPlacer.ProjectName = proj.GetProjectName();
			this.assetImportPlacer.Platforms = new ArrayList( proj.GetPlatformNames() );
			this.assetImportPlacer.LoadClassesFromProject(proj);

			// Hook up events
			this.assetImportPlacer.Event_LoadingDirectories += new EventHandler(assetImportPlacer_Event_LoadingDirectories);
			this.assetImportPlacer.Event_DoneLoadingDirectories += new EventHandler(assetImportPlacer_Event_DoneLoadingDirectories);
		}

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
			MOG_Project project = MOG_ControllerProject.GetProject();

			AddNewClasses(worker, 0, classificationTotal, project, nodes);
		}

		public int AddNewClasses(BackgroundWorker worker, int classificationProgress, int classificationTotal, MOG_Project project, TreeNodeCollection nodes)
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

					//If all the kids have the same sync target, we're setting the classification sync target to the same one
					string commonSyncTarget = FindCommonSyncTarget(atn);
					if (commonSyncTarget != "")
					{
						// Set the classification's sync target to the common sync target of all the assets
						atn.FileFullPath = commonSyncTarget;
					}

					atn.InDB = true;
				}

				// recurse
				classificationProgress = AddNewClasses(worker, classificationProgress, classificationTotal, project, atn.Nodes);
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

		#endregion
		#region Event handlers
		private void assetImportPlacer_Event_LoadingDirectories(object sender, EventArgs args)
		{
			this.Enabled = false;
		}

		private void assetImportPlacer_Event_DoneLoadingDirectories(object sender, EventArgs args)
		{
			this.Enabled = true;
		}

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			AddNewClasses(this.assetImportPlacer.AssetTreeNodes);

			AssetConfigureForm configForm = new AssetConfigureForm(this.project);
			configForm.ProjectRootPath = this.assetImportPlacer.ProjectPath;
			configForm.LoadAssetTree( this.assetImportPlacer.AssetTreeNodes );
			
			Hide();
			if (configForm.ShowDialog() == DialogResult.Cancel)
			{
				this.DialogResult = DialogResult.Cancel;
				Dispose();
				return;
			}

			this.DialogResult = DialogResult.OK;
			Dispose();
		}



		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			Dispose();
		}

		private void cbAssetExtensions_CheckedChanged(object sender, EventArgs e)
		{
			assetImportPlacer.UseFileExtensionsInAssetNames = cbAssetExtensions.Checked;
		}
		#endregion
	}
}
