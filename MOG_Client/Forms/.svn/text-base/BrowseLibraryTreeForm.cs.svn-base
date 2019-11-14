using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using MOG_ControlsLibrary.Common.MogControl_RepositoryTreeViews;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG_ControlsLibrary.Controls;

namespace MOG_Client.Forms
{
	/// <summary>
	/// Summary description for BrowseClassTreeForm.
	/// </summary>
	public class BrowseLibraryTreeForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button BrowseOKButton;
		private System.Windows.Forms.Button BrowseCancelButton;
		private MogControl_LibraryTreeView mBaseLeafTreeView;
		/// <summary>
		/// Get the MogControl_BaseLeafTreeView that BrowseClassTreeForm is using
		/// </summary>
		public MogControl_BaseTreeView BaseLeafTreeView
		{	get	{	return this.mBaseLeafTreeView;	}	}
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public BrowseLibraryTreeForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			mBaseLeafTreeView.Initialize();
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(BrowseLibraryTreeForm));
			this.BrowseOKButton = new System.Windows.Forms.Button();
			this.BrowseCancelButton = new System.Windows.Forms.Button();
			this.mBaseLeafTreeView = new MOG_ControlsLibrary.Common.MogControl_RepositoryTreeViews.MogControl_LibraryTreeView();
			this.SuspendLayout();
			// 
			// BrowseOKButton
			// 
			this.BrowseOKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.BrowseOKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.BrowseOKButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.BrowseOKButton.Location = new System.Drawing.Point(152, 252);
			this.BrowseOKButton.Name = "BrowseOKButton";
			this.BrowseOKButton.TabIndex = 0;
			this.BrowseOKButton.Text = "OK";
			this.BrowseOKButton.Click += new System.EventHandler(this.OKButton_Click);
			// 
			// BrowseCancelButton
			// 
			this.BrowseCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.BrowseCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.BrowseCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.BrowseCancelButton.Location = new System.Drawing.Point(232, 252);
			this.BrowseCancelButton.Name = "BrowseCancelButton";
			this.BrowseCancelButton.TabIndex = 1;
			this.BrowseCancelButton.Text = "Cancel";
			// 
			// mBaseLeafTreeView
			// 
			this.mBaseLeafTreeView.AllowDrop = true;
			this.mBaseLeafTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.mBaseLeafTreeView.ExpandAssets = false;
			this.mBaseLeafTreeView.ExpandPackageGroupAssets = false;
			this.mBaseLeafTreeView.ExpandPackageGroups = false;
			this.mBaseLeafTreeView.FocusForAssetNodes = LeafFocusLevel.RepositoryItems;
			this.mBaseLeafTreeView.HideSelection = false;
			this.mBaseLeafTreeView.HotTracking = true;
			this.mBaseLeafTreeView.Location = new System.Drawing.Point(0, 0);
			this.mBaseLeafTreeView.Name = "mBaseLeafTreeView";
			this.mBaseLeafTreeView.PathSeparator = "~";
			this.mBaseLeafTreeView.ShowAssets = false;
			this.mBaseLeafTreeView.Size = new System.Drawing.Size(320, 248);
			this.mBaseLeafTreeView.TabIndex = 0;
			// 
			// BrowseLibraryTreeForm
			// 
			this.AcceptButton = this.BrowseOKButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.BrowseCancelButton;
			this.ClientSize = new System.Drawing.Size(320, 281);
			this.Controls.Add(this.mBaseLeafTreeView);
			this.Controls.Add(this.BrowseCancelButton);
			this.Controls.Add(this.BrowseOKButton);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.Name = "BrowseLibraryTreeForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Select asset location...";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Attach the appropriate tag to this form so that our calling form can
		///		easily see what the user had selected.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OKButton_Click(object sender, System.EventArgs e)
		{
			string projectPath = MOG_ControllerProject.GetProjectName();
			TreeNode node = this.mBaseLeafTreeView.SelectedNode;
			// If we have a node selected...
			if( node != null )
			{
				string fullPath = node.FullPath;//.Replace( projectPath + node.TreeView.PathSeparator, "" );
				this.Tag = fullPath;
			}
		}
	}// end class
}// end ns