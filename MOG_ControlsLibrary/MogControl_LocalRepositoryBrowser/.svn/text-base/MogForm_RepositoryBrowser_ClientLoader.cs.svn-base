using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MOG_ControlsLibrary
{
	/// <summary>
	/// Summary description for MogForm_RepositoryBrowser_ClientLoader.
	/// </summary>
	public class MogForm_RepositoryBrowser_ClientLoader : System.Windows.Forms.Form
	{
		private MogControl_LocalRepositoryTreeView_ClientLoader mogControl_LocalRepositoryTreeView;
		private System.Windows.Forms.Button CloseButton;
		private System.Windows.Forms.Button RefreshButton;
		private System.Windows.Forms.Button ExitButton;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public MogForm_RepositoryBrowser_ClientLoader()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			mogControl_LocalRepositoryTreeView.InitializeNetworkDrivesOnly();
		}

		public string MOGSelectedRepository
		{
			get { return this.mogControl_LocalRepositoryTreeView.MOGGameDataTarget; }
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MogForm_RepositoryBrowser_ClientLoader));
			this.mogControl_LocalRepositoryTreeView = new MogControl_LocalRepositoryTreeView_ClientLoader();
			this.CloseButton = new System.Windows.Forms.Button();
			this.RefreshButton = new System.Windows.Forms.Button();
			this.ExitButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// mogControl_LocalRepositoryTreeView
			// 
			this.mogControl_LocalRepositoryTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.mogControl_LocalRepositoryTreeView.Location = new System.Drawing.Point(4, 8);
			this.mogControl_LocalRepositoryTreeView.MOGAllowDirectoryOpperations = true;
			this.mogControl_LocalRepositoryTreeView.MOGShowFiles = false;
			this.mogControl_LocalRepositoryTreeView.Name = "mogControl_LocalRepositoryTreeView";
			this.mogControl_LocalRepositoryTreeView.Size = new System.Drawing.Size(255, 160);
			this.mogControl_LocalRepositoryTreeView.TabIndex = 0;
			// 
			// CloseButton
			// 
			this.CloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.CloseButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.CloseButton.Location = new System.Drawing.Point(200, 168);
			this.CloseButton.Name = "CloseButton";
			this.CloseButton.Size = new System.Drawing.Size(56, 23);
			this.CloseButton.TabIndex = 1;
			this.CloseButton.Text = "OK";
			// 
			// RefreshButton
			// 
			this.RefreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.RefreshButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.RefreshButton.Location = new System.Drawing.Point(72, 168);
			this.RefreshButton.Name = "RefreshButton";
			this.RefreshButton.Size = new System.Drawing.Size(56, 23);
			this.RefreshButton.TabIndex = 2;
			this.RefreshButton.Text = "Refresh";
			this.RefreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
			// 
			// ExitButton
			// 
			this.ExitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.ExitButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.ExitButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.ExitButton.Location = new System.Drawing.Point(8, 168);
			this.ExitButton.Name = "ExitButton";
			this.ExitButton.Size = new System.Drawing.Size(56, 23);
			this.ExitButton.TabIndex = 3;
			this.ExitButton.Text = "Exit";
			this.ExitButton.Click += new System.EventHandler(this.ExitButton_Click);
			// 
			// MogForm_RepositoryBrowser_ClientLoader
			// 
			this.AcceptButton = this.CloseButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.ExitButton;
			this.ClientSize = new System.Drawing.Size(264, 197);
			this.Controls.Add(this.ExitButton);
			this.Controls.Add(this.RefreshButton);
			this.Controls.Add(this.CloseButton);
			this.Controls.Add(this.mogControl_LocalRepositoryTreeView);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.Name = "MogForm_RepositoryBrowser_ClientLoader";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Locate a valid MOG repository...";
			this.ResumeLayout(false);

		}
		#endregion

		private void ExitButton_Click(object sender, System.EventArgs e)
		{
			Application.Exit();
		}

		private void RefreshButton_Click(object sender, System.EventArgs e)
		{
			this.mogControl_LocalRepositoryTreeView.InitializeNetworkDrivesOnly();
		}
	}
}
