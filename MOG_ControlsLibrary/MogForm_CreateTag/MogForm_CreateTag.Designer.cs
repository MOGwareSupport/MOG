namespace MOG_ControlsLibrary.MogForm_CreateTag
{
	partial class MogForm_CreateTag
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MogForm_CreateTag));
			this.TagNameTextBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.TagOkButton = new System.Windows.Forms.Button();
			this.TagCancelButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// TagNameTextBox
			// 
			this.TagNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.TagNameTextBox.Location = new System.Drawing.Point(12, 28);
			this.TagNameTextBox.Name = "TagNameTextBox";
			this.TagNameTextBox.Size = new System.Drawing.Size(471, 20);
			this.TagNameTextBox.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(134, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Enter the name for this tag:";
			// 
			// TagOkButton
			// 
			this.TagOkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.TagOkButton.Location = new System.Drawing.Point(408, 54);
			this.TagOkButton.Name = "TagOkButton";
			this.TagOkButton.Size = new System.Drawing.Size(75, 23);
			this.TagOkButton.TabIndex = 2;
			this.TagOkButton.Text = "Ok";
			this.TagOkButton.UseVisualStyleBackColor = true;
			this.TagOkButton.Click += new System.EventHandler(this.TagOkButton_Click);
			// 
			// TagCancelButton
			// 
			this.TagCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.TagCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.TagCancelButton.Location = new System.Drawing.Point(327, 54);
			this.TagCancelButton.Name = "TagCancelButton";
			this.TagCancelButton.Size = new System.Drawing.Size(75, 23);
			this.TagCancelButton.TabIndex = 3;
			this.TagCancelButton.Text = "Cancel";
			this.TagCancelButton.UseVisualStyleBackColor = true;
			// 
			// MogForm_CreateTag
			// 
			this.AcceptButton = this.TagOkButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.TagCancelButton;
			this.ClientSize = new System.Drawing.Size(495, 89);
			this.Controls.Add(this.TagCancelButton);
			this.Controls.Add(this.TagOkButton);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.TagNameTextBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.Name = "MogForm_CreateTag";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "CreateTag";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox TagNameTextBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button TagOkButton;
		private System.Windows.Forms.Button TagCancelButton;
	}
}