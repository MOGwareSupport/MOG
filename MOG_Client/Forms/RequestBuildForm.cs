using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using MOG;
using MOG.PROJECT;
using MOG.DOSUTILS;
using MOG.INI;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;

using MOG_Client.Client_Utilities;
using MOG_ControlsLibrary.MogUtils_Settings;



namespace MOG_Client.Forms
{
	/// <summary>
	/// Summary description for RequestBuildForm.
	/// </summary>
	public class RequestBuildForm : System.Windows.Forms.Form
	{
		public MOG_Ini mOptions;
		private System.Windows.Forms.Button BuildLaunchButton;
		private System.Windows.Forms.Button BuildCancelButton;
		private System.Windows.Forms.Button BuildAddButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListBox BuildsListBox;
		private System.Windows.Forms.ListBox BuildOptionsListBox;
		public System.Windows.Forms.ListBox BuildRequests;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button BuildRequestRemoveButton;
		private System.Windows.Forms.Button BuildRequestUpButton;
		private System.Windows.Forms.Button BuildRequestDownButton;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public RequestBuildForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			string buildTool = MOG_ControllerSystem.LocateTool("Configs", "Build.Options.Info");
			if (DosUtils.FileExist(buildTool))
			{
				mOptions = new MOG_Ini(buildTool);
				InitFormOptions();
			}

			// Load prefs
			PreloadLastSettings();
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(RequestBuildForm));
			this.BuildLaunchButton = new System.Windows.Forms.Button();
			this.BuildCancelButton = new System.Windows.Forms.Button();
			this.BuildsListBox = new System.Windows.Forms.ListBox();
			this.BuildOptionsListBox = new System.Windows.Forms.ListBox();
			this.BuildRequests = new System.Windows.Forms.ListBox();
			this.BuildAddButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.BuildRequestRemoveButton = new System.Windows.Forms.Button();
			this.BuildRequestUpButton = new System.Windows.Forms.Button();
			this.BuildRequestDownButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// BuildLaunchButton
			// 
			this.BuildLaunchButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.BuildLaunchButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.BuildLaunchButton.Location = new System.Drawing.Point(274, 218);
			this.BuildLaunchButton.Name = "BuildLaunchButton";
			this.BuildLaunchButton.TabIndex = 0;
			this.BuildLaunchButton.Text = "Start Request";
			this.BuildLaunchButton.Click += new System.EventHandler(this.BuildLaunchButton_Click);
			// 
			// BuildCancelButton
			// 
			this.BuildCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.BuildCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.BuildCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.BuildCancelButton.Location = new System.Drawing.Point(194, 218);
			this.BuildCancelButton.Name = "BuildCancelButton";
			this.BuildCancelButton.TabIndex = 1;
			this.BuildCancelButton.Text = "Cancel";
			// 
			// BuildsListBox
			// 
			this.BuildsListBox.Location = new System.Drawing.Point(8, 24);
			this.BuildsListBox.Name = "BuildsListBox";
			this.BuildsListBox.Size = new System.Drawing.Size(80, 186);
			this.BuildsListBox.TabIndex = 2;
			this.BuildsListBox.DoubleClick += new System.EventHandler(this.BuildsListBox_DoubleClick);
			this.BuildsListBox.SelectedIndexChanged += new System.EventHandler(this.BuildsListBox_SelectedIndexChanged);
			// 
			// BuildOptionsListBox
			// 
			this.BuildOptionsListBox.Location = new System.Drawing.Point(96, 24);
			this.BuildOptionsListBox.Name = "BuildOptionsListBox";
			this.BuildOptionsListBox.Size = new System.Drawing.Size(72, 186);
			this.BuildOptionsListBox.TabIndex = 3;
			this.BuildOptionsListBox.DoubleClick += new System.EventHandler(this.BuildOptionsListBox_DoubleClick);
			// 
			// BuildRequests
			// 
			this.BuildRequests.Location = new System.Drawing.Point(208, 24);
			this.BuildRequests.Name = "BuildRequests";
			this.BuildRequests.Size = new System.Drawing.Size(136, 186);
			this.BuildRequests.TabIndex = 8;
			this.BuildRequests.SelectedIndexChanged += new System.EventHandler(this.BuildRequests_SelectedIndexChanged);
			// 
			// BuildAddButton
			// 
			this.BuildAddButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.BuildAddButton.Location = new System.Drawing.Point(176, 48);
			this.BuildAddButton.Name = "BuildAddButton";
			this.BuildAddButton.Size = new System.Drawing.Size(24, 16);
			this.BuildAddButton.TabIndex = 4;
			this.BuildAddButton.Text = ">>";
			this.BuildAddButton.Click += new System.EventHandler(this.BuildAddButton_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(40, 16);
			this.label1.TabIndex = 7;
			this.label1.Text = "Builds";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(93, 8);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(48, 16);
			this.label2.TabIndex = 8;
			this.label2.Text = "Options";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(208, 8);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(80, 16);
			this.label3.TabIndex = 9;
			this.label3.Text = "Build Requests";
			// 
			// BuildRequestRemoveButton
			// 
			this.BuildRequestRemoveButton.Enabled = false;
			this.BuildRequestRemoveButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.BuildRequestRemoveButton.Location = new System.Drawing.Point(176, 72);
			this.BuildRequestRemoveButton.Name = "BuildRequestRemoveButton";
			this.BuildRequestRemoveButton.Size = new System.Drawing.Size(24, 16);
			this.BuildRequestRemoveButton.TabIndex = 5;
			this.BuildRequestRemoveButton.Text = "<<";
			this.BuildRequestRemoveButton.Click += new System.EventHandler(this.BuildRequestRemoveButton_Click);
			// 
			// BuildRequestUpButton
			// 
			this.BuildRequestUpButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.BuildRequestUpButton.Location = new System.Drawing.Point(176, 96);
			this.BuildRequestUpButton.Name = "BuildRequestUpButton";
			this.BuildRequestUpButton.Size = new System.Drawing.Size(24, 16);
			this.BuildRequestUpButton.TabIndex = 6;
			this.BuildRequestUpButton.Text = "Up";
			this.BuildRequestUpButton.Click += new System.EventHandler(this.BuildRequestUpButton_Click);
			// 
			// BuildRequestDownButton
			// 
			this.BuildRequestDownButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.BuildRequestDownButton.Location = new System.Drawing.Point(176, 120);
			this.BuildRequestDownButton.Name = "BuildRequestDownButton";
			this.BuildRequestDownButton.Size = new System.Drawing.Size(24, 16);
			this.BuildRequestDownButton.TabIndex = 7;
			this.BuildRequestDownButton.Text = "Dn";
			this.BuildRequestDownButton.Click += new System.EventHandler(this.BuildRequestDownButton_Click);
			// 
			// RequestBuildForm
			// 
			this.AcceptButton = this.BuildLaunchButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.BuildCancelButton;
			this.ClientSize = new System.Drawing.Size(354, 247);
			this.Controls.Add(this.BuildRequestDownButton);
			this.Controls.Add(this.BuildRequestUpButton);
			this.Controls.Add(this.BuildRequestRemoveButton);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.BuildAddButton);
			this.Controls.Add(this.BuildRequests);
			this.Controls.Add(this.BuildOptionsListBox);
			this.Controls.Add(this.BuildsListBox);
			this.Controls.Add(this.BuildCancelButton);
			this.Controls.Add(this.BuildLaunchButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.Name = "RequestBuildForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Request builds";
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.RequestBuildForm_KeyUp);
			this.ResumeLayout(false);

		}
		#endregion

		private void PreloadLastSettings()
		{
			string buildString = guiUserPrefs.LoadPref("RequestBuild", "Builds");
			if (buildString.Length > 0)
			{
				string []builds = buildString.Trim().Split(",".ToCharArray());
				foreach (string build in builds)
				{
					BuildRequests.Items.Add(build);
				}
			}
		}

		private void SaveLastSettings()
		{
			string builds = "";
			foreach (string build in BuildRequests.Items)
			{
				if (builds.Length > 0)
				{
					builds = builds + "," + build;
				}
				else
				{
					builds = build;
				}
			}

			MogUtils_Settings.SaveSetting("RequestBuild", "Builds", builds);
		}

		private void InitFormOptions()
		{
			if (mOptions.SectionExist("Builds"))
			{
				for (int i = 0; i < mOptions.CountKeys("Builds"); i++)
				{
					string command = mOptions.GetKeyNameByIndexSLOW("Builds", i);
					BuildsListBox.Items.Add(command);
				}
			}
		}
		
		private void BuildAddButton_Click(object sender, System.EventArgs e)
		{
			string buildOptions = "";
			string buildType = "";

			// Add the Build type with the options and put in in the Request builds list
			if (BuildOptionsListBox.SelectedItem != null) 
			{
				buildOptions = (string)BuildOptionsListBox.SelectedItem;
			}

			if (BuildsListBox.SelectedItem != null) 
			{
				buildType = (string)BuildsListBox.SelectedItem;
			}

			BuildRequests.Items.Add(buildType + " " + buildOptions);
		}

		private void BuildOptionsListBox_DoubleClick(object sender, System.EventArgs e)
		{
			string buildOptions = "";
			string buildType = "";

			// Add the Build type with the options and put in in the Request builds list
			if (BuildOptionsListBox.SelectedItem != null) 
			{
				buildOptions = (string)BuildOptionsListBox.SelectedItem;
			}

			if (BuildsListBox.SelectedItem != null) 
			{
				buildType = (string)BuildsListBox.SelectedItem;
			}

			BuildRequests.Items.Add(buildType + " " + buildOptions);
		}

		private void BuildsListBox_DoubleClick(object sender, System.EventArgs e)
		{
			if (BuildsListBox.SelectedItem != null)
			{
				string buildOptions = "Builds." + (string)BuildsListBox.SelectedItem;

				if (mOptions.CountKeys(buildOptions) == 0)
				{
					BuildRequests.Items.Add((string)BuildsListBox.SelectedItem);
				}
			}
		}

		private void BuildsListBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			BuildOptionsListBox.Items.Clear();

			if (BuildsListBox.SelectedItem != null)
			{
				string buildOptions = "Builds." + (string)BuildsListBox.SelectedItem;

				for (int j = 0; j < mOptions.CountKeys(buildOptions); j++)
				{
					string subCommand = mOptions.GetKeyNameByIndexSLOW(buildOptions, j);
					BuildOptionsListBox.Items.Add(subCommand);
				}
			}
		}

		private void BuildRequestRemoveButton_Click(object sender, System.EventArgs e)
		{
			if (BuildRequests.SelectedItem != null)
			{
				BuildRequests.Items.RemoveAt(BuildRequests.SelectedIndex);
			}
		}

		private void BuildRequests_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (BuildRequests.SelectedItem != null)
			{
				BuildRequestRemoveButton.Enabled = true;
			}
			else
			{
				BuildRequestRemoveButton.Enabled = false;
			}
		}

		private void BuildLaunchButton_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.OK;
			SaveLastSettings();
			Close();
		}

		private void BuildRequestUpButton_Click(object sender, System.EventArgs e)
		{
			if (BuildRequests.SelectedItem != null)
			{
				int index = BuildRequests.SelectedIndex;
				string item = (string)BuildRequests.SelectedItem;
				if (BuildRequests.SelectedIndex != 0)
				{
					BuildRequests.Items.RemoveAt(index);
					BuildRequests.Items.Insert(index -1, item);

					BuildRequests.SelectedIndex = index -1;
				}
			}
		}

		private void BuildRequestDownButton_Click(object sender, System.EventArgs e)
		{
			if (BuildRequests.SelectedItem != null)
			{
				int index = BuildRequests.SelectedIndex;
				string item = (string)BuildRequests.SelectedItem;
				if (BuildRequests.SelectedIndex != BuildRequests.Items.Count-1)
				{
					BuildRequests.Items.RemoveAt(index);
					BuildRequests.Items.Insert(index +1, item);

					BuildRequests.SelectedIndex = index +1;
				}
			}
		}

		private void RequestBuildForm_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete)
			{
				if (BuildRequests.SelectedItem != null)
				{
					BuildRequests.Items.RemoveAt(BuildRequests.SelectedIndex);
				}
			}
		}	
	}
}
