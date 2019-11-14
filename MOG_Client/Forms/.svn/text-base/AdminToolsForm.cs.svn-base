using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using MOG;
using MOG.INI;
using MOG.TIME;
using MOG.PROJECT;
using MOG.REPORT;
using MOG.FILENAME;
using MOG.DOSUTILS;
using MOG.CONTROLLER.CONTROLLERREPOSITORY;

namespace MOG_Client.Forms
{
	/// <summary>
	/// Summary description for AdminToolsForm.
	/// </summary>
	public class AdminToolsForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button AdminToolsLaunchButton;
		public System.Windows.Forms.ListView AdminToolsOptionsListView;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public AdminToolsForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
			System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Check for current version exists in assets tree");
			System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Check for newer versions");
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(AdminToolsForm));
			this.AdminToolsLaunchButton = new System.Windows.Forms.Button();
			this.AdminToolsOptionsListView = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.SuspendLayout();
			// 
			// AdminToolsLaunchButton
			// 
			this.AdminToolsLaunchButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.AdminToolsLaunchButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.AdminToolsLaunchButton.Location = new System.Drawing.Point(226, 112);
			this.AdminToolsLaunchButton.Name = "AdminToolsLaunchButton";
			this.AdminToolsLaunchButton.Size = new System.Drawing.Size(88, 23);
			this.AdminToolsLaunchButton.TabIndex = 0;
			this.AdminToolsLaunchButton.Text = "Launch Report";
			this.AdminToolsLaunchButton.Click += new System.EventHandler(this.AdminToolsLaunchButton_Click);
			// 
			// AdminToolsOptionsListView
			// 
			this.AdminToolsOptionsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.AdminToolsOptionsListView.CheckBoxes = true;
			this.AdminToolsOptionsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																										this.columnHeader1});
			this.AdminToolsOptionsListView.FullRowSelect = true;
			this.AdminToolsOptionsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			listViewItem1.StateImageIndex = 0;
			listViewItem1.Tag = "TOOL_EXISTS";
			listViewItem2.StateImageIndex = 0;
			listViewItem2.Tag = "TOOL_CHECKNEWER";
			this.AdminToolsOptionsListView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
																									  listViewItem1,
																									  listViewItem2});
			this.AdminToolsOptionsListView.Location = new System.Drawing.Point(8, 8);
			this.AdminToolsOptionsListView.MultiSelect = false;
			this.AdminToolsOptionsListView.Name = "AdminToolsOptionsListView";
			this.AdminToolsOptionsListView.Size = new System.Drawing.Size(299, 97);
			this.AdminToolsOptionsListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.AdminToolsOptionsListView.TabIndex = 1;
			this.AdminToolsOptionsListView.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Tool";
			this.columnHeader1.Width = 295;
			// 
			// AdminToolsForm
			// 
			this.AcceptButton = this.AdminToolsLaunchButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(314, 141);
			this.Controls.Add(this.AdminToolsOptionsListView);
			this.Controls.Add(this.AdminToolsLaunchButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "AdminToolsForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Administrator Tools";
			this.ResumeLayout(false);

		}
		#endregion

		public bool ToolNewerCheck(MOG_Filename mogAsset, string version, MOG_Project pProject, ref string failedString)
		{
			MOG_Time assetTime = new MOG_Time(version);
			MOG_Time correctAssetTime = new MOG_Time();

			DirectoryInfo []dirs = DosUtils.DirectoryGetList(MOG_ControllerRepository.GetAssetBlessedPath(mogAsset).GetEncodedFilename(), "*.*");
						
			if (dirs != null)
			{
				foreach (DirectoryInfo dir in dirs)
				{
					string checkVersion = dir.Name.Substring(dir.Name.LastIndexOf(".")+1);
					MOG_Time dirTime = new MOG_Time(checkVersion);

					// Is this asset equal or newer than this dir version?
					if (assetTime.Compare(dirTime) < 0)
					{
						failedString = "Out of date," + failedString;
						return false;
					}
				}
			}

			return true;
		}

		public bool ToolExistsCheck(MOG_Filename mogAsset, string version, MOG_Project pProject, ref string failedString)
		{
			if (!Directory.Exists(MOG_ControllerRepository.GetAssetBlessedVersionPath(mogAsset, version).GetEncodedFilename()))
			{
				failedString = "Doesn't exist," + failedString;
				return false;				
			}

			return true;
		}

		private void AdminToolsLaunchButton_Click(object sender, System.EventArgs e)
		{
			if (AdminToolsOptionsListView.CheckedItems == null || AdminToolsOptionsListView.CheckedItems.Count == 0)
			{
				MessageBox.Show("At least one tool must be checked inorder to run tool!", "Launch Report");
			}
			else
			{
				this.DialogResult = DialogResult.OK;
			}
		}
	}
}
