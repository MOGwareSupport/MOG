namespace MOG_RemoteServerManager
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.RSMSyncButton = new System.Windows.Forms.Button();
			this.RemoteServerSettings = new MOG_RemoteServerManager.MogControl_RemoteServers();
			this.SuspendLayout();
			// 
			// RSMSyncButton
			// 
			this.RSMSyncButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.RSMSyncButton.Location = new System.Drawing.Point(530, 369);
			this.RSMSyncButton.Name = "RSMSyncButton";
			this.RSMSyncButton.Size = new System.Drawing.Size(75, 23);
			this.RSMSyncButton.TabIndex = 0;
			this.RSMSyncButton.Text = "Sync";
			this.RSMSyncButton.UseVisualStyleBackColor = true;
			// 
			// RemoteServerSettings
			// 
			this.RemoteServerSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.RemoteServerSettings.EnableRemoteServers = false;
			this.RemoteServerSettings.Location = new System.Drawing.Point(12, 12);
			this.RemoteServerSettings.Name = "RemoteServerSettings";
			this.RemoteServerSettings.Size = new System.Drawing.Size(595, 353);
			this.RemoteServerSettings.TabIndex = 1;
			// 
			// MainForm
			// 
			this.ClientSize = new System.Drawing.Size(617, 404);
			this.Controls.Add(this.RemoteServerSettings);
			this.Controls.Add(this.RSMSyncButton);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "MainForm";
			this.Text = "Mog Remote Server Manager";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button RSMSyncButton;
		private MogControl_RemoteServers RemoteServerSettings;

	}
}

