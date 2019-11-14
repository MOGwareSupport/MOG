using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using MOG;
using MOG.PROPERTIES;
using MOG_ControlsLibrary.Controls;

namespace MOG_ControlsLibrary.Forms
{
	/// <summary>
	/// Summary description for GameDataPathForm.
	/// </summary>
	public class GameDataPathForm : System.Windows.Forms.Form
	{
		private ArrayList mMOGPropertyArray;
		private MOG_ControlsLibrary.Controls.MogControl_GameDataDestinationTreeView GameDataMogControl_GameDataDestinationTreeView;
		private System.Windows.Forms.TextBox GameDataPathTextBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button GameDataCloseButton;
		private bool mEditing = false;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public GameDataPathForm(string platform)
			:this( platform, null )
		{
		}

		public GameDataPathForm( string platform, string assetName )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			mMOGPropertyArray = new ArrayList();
			this.GameDataMogControl_GameDataDestinationTreeView.InitializeVirtual(platform);	
			if( assetName == null )
			{
				this.Text += "this package";
			}
			else
			{
				this.Text += assetName;
			}
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(GameDataPathForm));
			this.GameDataMogControl_GameDataDestinationTreeView = new MogControl_GameDataDestinationTreeView();
			this.GameDataPathTextBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.GameDataCloseButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// GameDataMogControl_GameDataDestinationTreeView
			// 
			this.GameDataMogControl_GameDataDestinationTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.GameDataMogControl_GameDataDestinationTreeView.Location = new System.Drawing.Point(8, 8);
			this.GameDataMogControl_GameDataDestinationTreeView.MOGSelectedNode = null;
			this.GameDataMogControl_GameDataDestinationTreeView.MOGShowFiles = false;
			this.GameDataMogControl_GameDataDestinationTreeView.Name = "GameDataMogControl_GameDataDestinationTreeView";
			this.GameDataMogControl_GameDataDestinationTreeView.Size = new System.Drawing.Size(288, 295);
			this.GameDataMogControl_GameDataDestinationTreeView.TabIndex = 0;
			this.GameDataMogControl_GameDataDestinationTreeView.AfterTargetSelect += new MogControl_GameDataDestinationTreeView.TreeViewEvent(this.GameDataMogControl_GameDataDestinationTreeView_AfterTargetSelect);
			// 
			// GameDataPathTextBox
			// 
			this.GameDataPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.GameDataPathTextBox.Location = new System.Drawing.Point(8, 321);
			this.GameDataPathTextBox.Name = "GameDataPathTextBox";
			this.GameDataPathTextBox.Size = new System.Drawing.Size(288, 20);
			this.GameDataPathTextBox.TabIndex = 1;
			this.GameDataPathTextBox.Text = "";
			this.GameDataPathTextBox.TextChanged += new System.EventHandler(this.GameDataPathTextBox_TextChanged);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.Location = new System.Drawing.Point(8, 305);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(216, 16);
			this.label1.TabIndex = 2;
			this.label1.Text = "Game data destination:";
			// 
			// GameDataCloseButton
			// 
			this.GameDataCloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.GameDataCloseButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.GameDataCloseButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.GameDataCloseButton.Location = new System.Drawing.Point(224, 345);
			this.GameDataCloseButton.Name = "GameDataCloseButton";
			this.GameDataCloseButton.TabIndex = 3;
			this.GameDataCloseButton.Text = "Close";
			// 
			// GameDataPathForm
			// 
			this.AcceptButton = this.GameDataCloseButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(304, 374);
			this.Controls.Add(this.GameDataCloseButton);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.GameDataPathTextBox);
			this.Controls.Add(this.GameDataMogControl_GameDataDestinationTreeView);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.Name = "GameDataPathForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Select the target path for ";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Get the MOG_Properties for this asset
		/// </summary>
		public ArrayList MOGPropertyArray
		{
			get { return mMOGPropertyArray; }
		}

		private void GameDataMogControl_GameDataDestinationTreeView_AfterTargetSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			// Clear any previous packages
			mMOGPropertyArray.Clear();

			// Make sure to strip off the root node test since it represents the workspace directory
			string relativeSyncTargetPath = this.GameDataMogControl_GameDataDestinationTreeView.MOGGameDataTarget.Substring(this.GameDataMogControl_GameDataDestinationTreeView.MOGRootNode.Text.Length);

			mEditing = true;
			this.GameDataPathTextBox.Text = this.GameDataMogControl_GameDataDestinationTreeView.MOGGameDataTarget;
			mEditing = false;
			
			// Add our new package to our properies
            mMOGPropertyArray.Add(MOG.MOG_PropertyFactory.MOG_Sync_OptionsProperties.New_SyncTargetPath(relativeSyncTargetPath));
		}

		private void GameDataPathTextBox_TextChanged(object sender, System.EventArgs e)
		{
			if (!mEditing)
			{
				// Clear any previous packages
				mMOGPropertyArray.Clear();

				// Make sure to strip off the root node test since it represents the workspace directory
				string relativeSyncTargetPath = this.GameDataPathTextBox.Text.Substring(this.GameDataMogControl_GameDataDestinationTreeView.MOGRootNode.Text.Length);

				// Add our new package to our properies
                mMOGPropertyArray.Add(MOG.MOG_PropertyFactory.MOG_Sync_OptionsProperties.New_SyncTargetPath(relativeSyncTargetPath));
			}
		}
	}
}
