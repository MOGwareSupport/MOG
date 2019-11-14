using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;

using MOG_Server_Loader;

namespace AppLoading
{
	/// <summary>
	/// Summary description for SplashForm.
	/// </summary>
	public class SplashForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.ProgressBar SplashProgressBar;

		private System.Windows.Forms.Timer StartTimer;
		private System.ComponentModel.IContainer components;
		private System.Timers.Timer BarTimer;

		private FormLoader mainForm;

		public SplashForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			Assembly assembly = Assembly.GetCallingAssembly();
			StartTimer.Enabled = true;
			StartTimer.Start();
		}

		private bool InitializeMainForm()
		{
			// Display the splash screen			
			Refresh();
			intializeProgressBar(100);
			BarTimer.Enabled = true;
			BarTimer.Start();
			Refresh();

			updateSplash("Checking for versions", 0);
			mainForm = new FormLoader(this);
			return mainForm.Initialize();
		}

		public void intializeProgressBar(int max)
		{
			SplashProgressBar.Maximum = max;
			SplashProgressBar.Step = 1;
		}

		public void updateSplash(string str, int sleepTime)
		{
			//SplashLabel.Text = str;
			SplashProgressBar.PerformStep();
			Thread.Sleep(sleepTime);
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
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SplashForm));
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.SplashProgressBar = new System.Windows.Forms.ProgressBar();
			this.StartTimer = new System.Windows.Forms.Timer(this.components);
			this.BarTimer = new System.Timers.Timer();
			((System.ComponentModel.ISupportInitialize)(this.BarTimer)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(0, 0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(224, 176);
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			// 
			// SplashProgressBar
			// 
			this.SplashProgressBar.Location = new System.Drawing.Point(16, 128);
			this.SplashProgressBar.Name = "SplashProgressBar";
			this.SplashProgressBar.Size = new System.Drawing.Size(184, 8);
			this.SplashProgressBar.TabIndex = 2;
			// 
			// StartTimer
			// 
			this.StartTimer.Tick += new System.EventHandler(this.StartTimer_Tick);
			// 
			// BarTimer
			// 
			this.BarTimer.Interval = 50;
			this.BarTimer.SynchronizingObject = this;
			this.BarTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.BarTimer_Elapsed);
			// 
			// SplashForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.Color.WhiteSmoke;
			this.ClientSize = new System.Drawing.Size(224, 176);
			this.Controls.Add(this.SplashProgressBar);
			this.Controls.Add(this.pictureBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "SplashForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "AppLoading - Loader";
			this.TransparencyKey = System.Drawing.Color.WhiteSmoke;
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SplashForm_KeyDown);
			((System.ComponentModel.ISupportInitialize)(this.BarTimer)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
            Control.CheckForIllegalCrossThreadCalls = false;

            Application.Run(new SplashForm());		
		}

		private void StartTimer_Tick(object sender, System.EventArgs e)
		{
			if (mainForm == null)
			{
				StartTimer.Stop();
				if (InitializeMainForm())
                    StartTimer.Start();
				else
					Application.Exit();
			}
			else 
			{
				SplashProgressBar.Value = 100;
				StartTimer.Stop();
				mainForm.Show();				
			}
		}

		private void BarTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (SplashProgressBar.Value != 100 && Visible == true)
			{
				SplashProgressBar.PerformStep();
			}
		}

		private void SplashForm_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			mainForm.UserAbort = true;
			mainForm.Opacity = 1;
			this.Visible = false;
		}
	}
}
