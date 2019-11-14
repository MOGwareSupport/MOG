namespace MOG_ProgressTester
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
			this.TestButton = new System.Windows.Forms.Button();
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.moG_XpProgressBar1 = new MOG_CoreControls.MOG_XpProgressBar();
			this.SuspendLayout();
			// 
			// TestButton
			// 
			this.TestButton.Location = new System.Drawing.Point(91, 74);
			this.TestButton.Name = "TestButton";
			this.TestButton.Size = new System.Drawing.Size(75, 23);
			this.TestButton.TabIndex = 0;
			this.TestButton.Text = "Begin Test";
			this.TestButton.UseVisualStyleBackColor = true;
			this.TestButton.Click += new System.EventHandler(this.TestButton_Click);
			// 
			// progressBar1
			// 
			this.progressBar1.Location = new System.Drawing.Point(12, 152);
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(270, 10);
			this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.progressBar1.TabIndex = 2;
			this.progressBar1.Value = 50;
			// 
			// moG_XpProgressBar1
			// 
			this.moG_XpProgressBar1.ColorBackGround = System.Drawing.Color.White;
			this.moG_XpProgressBar1.ColorBarBorder = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(240)))), ((int)(((byte)(170)))));
			this.moG_XpProgressBar1.ColorBarCenter = System.Drawing.Color.White;
			this.moG_XpProgressBar1.ColorText = System.Drawing.Color.Black;
			this.moG_XpProgressBar1.GradientStyle = MOG_CoreControls.GradientMode.Vertical;
			this.moG_XpProgressBar1.Location = new System.Drawing.Point(12, 125);
			this.moG_XpProgressBar1.Name = "moG_XpProgressBar1";
			this.moG_XpProgressBar1.Position = 100;
			this.moG_XpProgressBar1.PositionMax = 100;
			this.moG_XpProgressBar1.PositionMin = 0;
			this.moG_XpProgressBar1.Size = new System.Drawing.Size(270, 12);
			this.moG_XpProgressBar1.SteepDistance = ((byte)(0));
			this.moG_XpProgressBar1.TabIndex = 1;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(290, 264);
			this.Controls.Add(this.progressBar1);
			this.Controls.Add(this.moG_XpProgressBar1);
			this.Controls.Add(this.TestButton);
			this.Name = "MainForm";
			this.Text = "Progress Tester";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button TestButton;
		private MOG_CoreControls.MOG_XpProgressBar moG_XpProgressBar1;
		private System.Windows.Forms.ProgressBar progressBar1;
	}
}

