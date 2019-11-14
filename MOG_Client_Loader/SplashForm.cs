using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;

using MOG_Client_Loader;
using MOG_CoreControls;

namespace AppLoading
{
	/// <summary>
	/// Summary description for SplashForm.
	/// </summary>
	public class SplashForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.PictureBox pictureBox1;

		private System.Windows.Forms.Timer StartTimer;
		private System.ComponentModel.IContainer components;
		private System.Timers.Timer BarTimer;

		private FormLoader mainForm;
		private MOG_XpProgressBar SplashProgressBar;
		public string[] mArgs;

		public SplashForm(string[] args)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			System.Drawing.Rectangle workingRectangle = Screen.PrimaryScreen.WorkingArea;

			Point bottomRight = new Point(workingRectangle.Width - (this.Width + 2),
										  workingRectangle.Height - (this.Height + 2));
			this.Location = bottomRight;

			mArgs = args;

			Assembly assembly = Assembly.GetCallingAssembly();
			StartTimer.Enabled = true;
			StartTimer.Start();
		}

		private void InitializeMainForm()
		{
			// Display the splash screen			
			Refresh();
			intializeProgressBar(100);
			BarTimer.Enabled = true;
			BarTimer.Start();
			Refresh();

			updateSplash("Checking for versions", 0);
			mainForm = new FormLoader(this);
		}

		public void intializeProgressBar(int max)
		{
			SplashProgressBar.PositionMax = max;
			SplashProgressBar.Position = 0;
		}

		public void updateSplash(string str, int sleepTime)
		{
			if (str.Length > 0) SplashProgressBar.Text = str;
			SplashProgressBar.Position ++;
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
			this.StartTimer = new System.Windows.Forms.Timer(this.components);
			this.BarTimer = new System.Timers.Timer();
			this.SplashProgressBar = new MOG_XpProgressBar();
			((System.ComponentModel.ISupportInitialize)(this.BarTimer)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(0, 0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(230, 19);
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
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
			// SplashProgressBar
			// 
			this.SplashProgressBar.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(14)), ((System.Byte)(114)), ((System.Byte)(214)));
			this.SplashProgressBar.ColorBackGround = System.Drawing.Color.FromArgb(((System.Byte)(9)), ((System.Byte)(71)), ((System.Byte)(134)));
			this.SplashProgressBar.ColorBarBorder = System.Drawing.Color.FromArgb(((System.Byte)(130)), ((System.Byte)(188)), ((System.Byte)(247)));
			this.SplashProgressBar.ColorBarCenter = System.Drawing.Color.FromArgb(((System.Byte)(48)), ((System.Byte)(145)), ((System.Byte)(241)));
			this.SplashProgressBar.ColorText = System.Drawing.Color.WhiteSmoke;
			this.SplashProgressBar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.SplashProgressBar.GradientStyle = MOG_CoreControls.GradientMode.Vertical;
			this.SplashProgressBar.Location = new System.Drawing.Point(46, 3);
			this.SplashProgressBar.Name = "SplashProgressBar";
			this.SplashProgressBar.Position = 0;
			this.SplashProgressBar.PositionMax = 100;
			this.SplashProgressBar.PositionMin = 0;
			this.SplashProgressBar.Size = new System.Drawing.Size(180, 12);
			this.SplashProgressBar.SteepDistance = ((System.Byte)(0));
			this.SplashProgressBar.SteepWidth = ((System.Byte)(1));
			this.SplashProgressBar.TabIndex = 3;
			// 
			// SplashForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.Color.WhiteSmoke;
			this.ClientSize = new System.Drawing.Size(230, 19);
			this.Controls.Add(this.SplashProgressBar);
			this.Controls.Add(this.pictureBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "SplashForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
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
		static void Main(string[] args) 
		{
            Control.CheckForIllegalCrossThreadCalls = false;

            Application.Run(new SplashForm(args));		
		}

		private void StartTimer_Tick(object sender, System.EventArgs e)
		{
			if (mainForm == null)
			{
				StartTimer.Stop();
				InitializeMainForm();
				StartTimer.Start();
			}
			else 
			{
				SplashProgressBar.Position = 100;
				StartTimer.Stop();
				mainForm.Show();				
			}
		}

		private void BarTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (SplashProgressBar.Position != 100 && Visible == true && mainForm.bCopying == false)
			{
				SplashProgressBar.Position++;
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
