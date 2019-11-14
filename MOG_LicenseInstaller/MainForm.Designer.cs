namespace MOG_LicenseInstaller
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
			this.LicenseGroupBox = new System.Windows.Forms.GroupBox();
			this.LicenseRichTextBox = new System.Windows.Forms.RichTextBox();
			this.LicenseInstallButton = new System.Windows.Forms.Button();
			this.LicenseOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.LicenseLoadButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.LicenseServerLabel = new System.Windows.Forms.Label();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.searchForServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.LicenseGroupBox.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// LicenseGroupBox
			// 
			this.LicenseGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.LicenseGroupBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(228)))), ((int)(((byte)(239)))));
			this.LicenseGroupBox.Controls.Add(this.LicenseRichTextBox);
			this.LicenseGroupBox.Controls.Add(this.LicenseLoadButton);
			this.LicenseGroupBox.Location = new System.Drawing.Point(12, 61);
			this.LicenseGroupBox.Name = "LicenseGroupBox";
			this.LicenseGroupBox.Size = new System.Drawing.Size(475, 197);
			this.LicenseGroupBox.TabIndex = 1;
			this.LicenseGroupBox.TabStop = false;
			this.LicenseGroupBox.Text = "License File";
			// 
			// LicenseRichTextBox
			// 
			this.LicenseRichTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.LicenseRichTextBox.DetectUrls = false;
			this.LicenseRichTextBox.Enabled = false;
			this.LicenseRichTextBox.Location = new System.Drawing.Point(6, 48);
			this.LicenseRichTextBox.Name = "LicenseRichTextBox";
			this.LicenseRichTextBox.Size = new System.Drawing.Size(463, 143);
			this.LicenseRichTextBox.TabIndex = 1;
			this.LicenseRichTextBox.Text = "";
			// 
			// LicenseInstallButton
			// 
			this.LicenseInstallButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.LicenseInstallButton.Location = new System.Drawing.Point(360, 264);
			this.LicenseInstallButton.Name = "LicenseInstallButton";
			this.LicenseInstallButton.Size = new System.Drawing.Size(127, 23);
			this.LicenseInstallButton.TabIndex = 2;
			this.LicenseInstallButton.Text = "Install License File";
			this.LicenseInstallButton.UseVisualStyleBackColor = true;
			this.LicenseInstallButton.Click += new System.EventHandler(this.LicenseInstallButton_Click);
			// 
			// LicenseOpenFileDialog
			// 
			this.LicenseOpenFileDialog.AddExtension = false;
			this.LicenseOpenFileDialog.FileName = "mog_license";
			this.LicenseOpenFileDialog.Title = "Locate a MOG License";
			// 
			// LicenseLoadButton
			// 
			this.LicenseLoadButton.Location = new System.Drawing.Point(6, 19);
			this.LicenseLoadButton.Name = "LicenseLoadButton";
			this.LicenseLoadButton.Size = new System.Drawing.Size(127, 23);
			this.LicenseLoadButton.TabIndex = 3;
			this.LicenseLoadButton.Text = "Load License File...";
			this.LicenseLoadButton.UseVisualStyleBackColor = true;
			this.LicenseLoadButton.Click += new System.EventHandler(this.LicenseLoadButton_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(15, 37);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(130, 13);
			this.label1.TabIndex = 4;
			this.label1.Text = "Located MOG Server:";
			// 
			// LicenseServerLabel
			// 
			this.LicenseServerLabel.AutoSize = true;
			this.LicenseServerLabel.Location = new System.Drawing.Point(142, 37);
			this.LicenseServerLabel.Name = "LicenseServerLabel";
			this.LicenseServerLabel.Size = new System.Drawing.Size(64, 13);
			this.LicenseServerLabel.TabIndex = 5;
			this.LicenseServerLabel.Text = "Searching...";
			// 
			// menuStrip1
			// 
			this.menuStrip1.BackColor = System.Drawing.Color.LightSteelBlue;
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(499, 24);
			this.menuStrip1.TabIndex = 6;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.quitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// quitToolStripMenuItem
			// 
			this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
			this.quitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.quitToolStripMenuItem.Text = "Quit";
			this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
			// 
			// toolsToolStripMenuItem
			// 
			this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.searchForServerToolStripMenuItem});
			this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
			this.toolsToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.toolsToolStripMenuItem.Text = "Tools";
			// 
			// searchForServerToolStripMenuItem
			// 
			this.searchForServerToolStripMenuItem.Name = "searchForServerToolStripMenuItem";
			this.searchForServerToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
			this.searchForServerToolStripMenuItem.Text = "Search for server...";
			this.searchForServerToolStripMenuItem.Click += new System.EventHandler(this.searchForServerToolStripMenuItem_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(228)))), ((int)(((byte)(239)))));
			this.ClientSize = new System.Drawing.Size(499, 290);
			this.Controls.Add(this.LicenseServerLabel);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.LicenseInstallButton);
			this.Controls.Add(this.LicenseGroupBox);
			this.Controls.Add(this.menuStrip1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "MOG License Installer";
			this.LicenseGroupBox.ResumeLayout(false);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox LicenseGroupBox;
		private System.Windows.Forms.RichTextBox LicenseRichTextBox;
		private System.Windows.Forms.Button LicenseInstallButton;
		private System.Windows.Forms.OpenFileDialog LicenseOpenFileDialog;
		private System.Windows.Forms.Button LicenseLoadButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label LicenseServerLabel;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem searchForServerToolStripMenuItem;
	}
}

