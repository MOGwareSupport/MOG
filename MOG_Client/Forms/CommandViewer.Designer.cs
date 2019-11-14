using MOG_ControlsLibrary.MogControl_MogCommandViewer;
namespace MOG_Client.Forms
{
	partial class CommandViewer
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CommandViewer));
			this.CommandView = new MOG_ControlsLibrary.MogControl_MogCommandViewer.MogControl_MogCommandViewer();
			this.ViewCloseButton = new System.Windows.Forms.Button();
			this.btnRestart = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// CommandView
			// 
			this.CommandView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.CommandView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.CommandView.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.CommandView.Location = new System.Drawing.Point(12, 12);
			this.CommandView.Name = "CommandView";
			this.CommandView.Size = new System.Drawing.Size(636, 179);
			this.CommandView.TabIndex = 0;
			// 
			// ViewCloseButton
			// 
			this.ViewCloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.ViewCloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.ViewCloseButton.Location = new System.Drawing.Point(566, 198);
			this.ViewCloseButton.Name = "ViewCloseButton";
			this.ViewCloseButton.Size = new System.Drawing.Size(82, 23);
			this.ViewCloseButton.TabIndex = 1;
			this.ViewCloseButton.Text = "Close";
			this.ViewCloseButton.UseVisualStyleBackColor = true;
			this.ViewCloseButton.Click += new System.EventHandler(this.ViewCloseButton_Click);
			// 
			// btnRestart
			// 
			this.btnRestart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnRestart.Location = new System.Drawing.Point(12, 198);
			this.btnRestart.Name = "btnRestart";
			this.btnRestart.Size = new System.Drawing.Size(75, 23);
			this.btnRestart.TabIndex = 2;
			this.btnRestart.Text = "Restart";
			this.btnRestart.UseVisualStyleBackColor = true;
			this.btnRestart.Click += new System.EventHandler(this.btnRestart_Click);
			// 
			// CommandViewer
			// 
			this.AcceptButton = this.ViewCloseButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(660, 233);
			this.Controls.Add(this.btnRestart);
			this.Controls.Add(this.ViewCloseButton);
			this.Controls.Add(this.CommandView);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.Name = "CommandViewer";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "CommandViewer";
			this.Load += new System.EventHandler(this.CommandViewer_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private MogControl_MogCommandViewer CommandView;
		private System.Windows.Forms.Button ViewCloseButton;
		private System.Windows.Forms.Button btnRestart;
	}
}