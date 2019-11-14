using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;
using PerPixelAlphaBlend;
using System.Diagnostics;
using System.Runtime.InteropServices;

using MOG;
using MOG.REPORT;
using MOG.PROMPT;
using MOG.DOSUTILS;
using MOG.CONTROLLER.CONTROLLERSYSTEM;

using MOG_EventViewer;
using MOG_ControlsLibrary.Controls;
using MOG_CoreControls;

namespace AppLoading
{
	/// <summary>
	/// Summary description for SplashForm.
	/// </summary>
	public class SplashForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.PictureBox pictureBox1;
		private MOG_EventViewerMainForm mainForm;
		private MyPerPixelAlphaForm Splash;
		private Bitmap bitmap;

		public string tempVersion;
		private System.Windows.Forms.Timer StartTimer;
		private System.ComponentModel.IContainer components;
		private string[] mCommandLineArgs;
		private MOG_XpProgressBar SplashProgressBar;
		private System.Windows.Forms.Label SplashLabel;
		private System.Windows.Forms.Label VersionLabel;
		public bool mInitializationComplete;

		#region DLL Imports
		[DllImport("user32.dll")] private static extern 
			bool SetForegroundWindow(IntPtr hWnd);
		[DllImport("user32.dll")] private static extern 
			bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
		[DllImport("user32.dll")] private static extern 
			bool IsIconic(IntPtr hWnd);

		private const int SW_RESTORE = 9;

		#endregion

		public SplashForm(string[] args)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			StartTimer.Start();
			mCommandLineArgs = args;

			mInitializationComplete = false;

			this.Load += new System.EventHandler(this.Form1_Load);

			// Setup our version number
			FileVersionInfo file = FileVersionInfo.GetVersionInfo(Application.ExecutablePath);
			if (file != null)
			{
				tempVersion = file.FileVersion;
				if (tempVersion == "3.0.0.0") tempVersion = "Debug build";
			}
			
			VersionLabel.Text = tempVersion;
		}

		private void Form1_Load(object sender, System.EventArgs e)
		{
			// Splash will contain the per-pixel-alpha dib
			Splash = new MyPerPixelAlphaForm(this);
			Splash.Show();
			Bitmap btm = (Bitmap)this.pictureBox1.Image;
			Splash.SetBitmap(btm,255);		
		}

		private void InitializeMainMogForm(string[] args)
		{
			updateSplash("Initializing main", 0);
			mainForm = new MOG_EventViewerMainForm();
			mainForm.InitializeSystem(this);
		}

		public void updateSplash(string str, int sleepTime)
		{
			SplashLabel.Text = str;
			SplashProgressBar.Position += 1;
			Splash.Refresh();
			Update();
			Thread.Sleep(sleepTime);
		}

		public void updateSplashNoStep(string str, int sleepTime)
		{
			SplashLabel.Text = str;
			Update();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplashForm));
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.StartTimer = new System.Windows.Forms.Timer(this.components);
			this.SplashProgressBar = new MOG_CoreControls.MOG_XpProgressBar();
			this.SplashLabel = new System.Windows.Forms.Label();
			this.VersionLabel = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(0, 0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(608, 272);
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.Visible = false;
			// 
			// StartTimer
			// 
			this.StartTimer.Tick += new System.EventHandler(this.StartTimer_Tick);
			// 
			// SplashProgressBar
			// 
			this.SplashProgressBar.BackColor = System.Drawing.Color.Brown;
			this.SplashProgressBar.ColorBackGround = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(58)))), ((int)(((byte)(123)))));
			this.SplashProgressBar.ColorBarBorder = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(153)))), ((int)(((byte)(238)))));
			this.SplashProgressBar.ColorBarCenter = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(80)))), ((int)(((byte)(168)))));
			this.SplashProgressBar.ColorText = System.Drawing.Color.Gray;
			this.SplashProgressBar.Font = new System.Drawing.Font("Verdana", 7.65F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.SplashProgressBar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
			this.SplashProgressBar.Location = new System.Drawing.Point(233, 234);
			this.SplashProgressBar.Name = "SplashProgressBar";
			this.SplashProgressBar.Position = 0;
			this.SplashProgressBar.PositionMax = 6;
			this.SplashProgressBar.PositionMin = 0;
			this.SplashProgressBar.Size = new System.Drawing.Size(360, 12);
			this.SplashProgressBar.SteepDistance = ((byte)(0));
			this.SplashProgressBar.TabIndex = 4;
			this.SplashProgressBar.TextShadow = false;
			// 
			// SplashLabel
			// 
			this.SplashLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(214)))), ((int)(((byte)(234)))));
			this.SplashLabel.Font = new System.Drawing.Font("Verdana", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.SplashLabel.ForeColor = System.Drawing.Color.Black;
			this.SplashLabel.Location = new System.Drawing.Point(234, 224);
			this.SplashLabel.Name = "SplashLabel";
			this.SplashLabel.Size = new System.Drawing.Size(359, 10);
			this.SplashLabel.TabIndex = 1;
			this.SplashLabel.Text = "Initializing Event Viewer...";
			this.SplashLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// VersionLabel
			// 
			this.VersionLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(246)))), ((int)(((byte)(254)))));
			this.VersionLabel.Font = new System.Drawing.Font("Verdana", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.VersionLabel.ForeColor = System.Drawing.Color.Black;
			this.VersionLabel.Location = new System.Drawing.Point(448, 7);
			this.VersionLabel.Name = "VersionLabel";
			this.VersionLabel.Size = new System.Drawing.Size(144, 11);
			this.VersionLabel.TabIndex = 3;
			this.VersionLabel.Text = "3.0.1.23235";
			this.VersionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// SplashForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(608, 272);
			this.Controls.Add(this.VersionLabel);
			this.Controls.Add(this.SplashLabel);
			this.Controls.Add(this.SplashProgressBar);
			this.Controls.Add(this.pictureBox1);
			this.ForeColor = System.Drawing.Color.White;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.Name = "SplashForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Initializing Event Viewer. . .";
			this.TransparencyKey = System.Drawing.Color.Transparent;
			this.Click += new System.EventHandler(this.SplashForm_Click);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args) 
		{
            //Control.CheckForIllegalCrossThreadCalls = false;

            SplashForm form = null;
			try
			{
				// Check for multiple instances
				Process mogProcess = RunningInstance();
				if (mogProcess != null)
				{
					// we've got a duplicate process - ask the user if they want to continue anyway
					DialogResult res = MessageBox.Show("An instance of the MOG Server Manager is already running on this machine.\nIt is possible that this instance is the result of a crash or other problem,\nin which case you should contact your MOG or network administrator.\n\nWould you like to start the Manager anyway?", 
						"Manager Already Running", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

					// if not, shutdown
					if (res == DialogResult.No)
					{
						return;
					}

					// user wants to spawn a new instance
				}

				form = new SplashForm(args);
				form.Closing += new CancelEventHandler(form_Closing);
				form.Closed += new EventHandler(form_Closed);
				form.HandleDestroyed += new EventHandler(form_HandleDestroyed);
				Application.Run( form );	
			}
			catch(Exception e)
			{
				MOG_Report.ReportMessage("Main", e.Message, e.StackTrace, MOG_ALERT_LEVEL.CRITICAL);
				// Write our problem out to our Output window in VS.NET
				System.Diagnostics.Debug.WriteLine(e.ToString() + "\n" + e.StackTrace);
				// Shutdown our process
				if( form != null )
				{
					form.Close();									
				}
				// Exit our application so we can set a breakpoint here and try again
				MOG_Main.Shutdown();
				Application.Exit();
			}
		}

		public static Process RunningInstance() 
		{ 
			Process current = Process.GetCurrentProcess(); 
			Process[] processes = Process.GetProcessesByName (current.ProcessName); 

			//Loop through the running processes in with the same name 
			foreach (Process process in processes) 
			{ 
				//Ignore the current process 
				if (process.Id != current.Id) 
				{ 
					//Make sure that the process is running from the exe file. 
					if (Assembly.GetExecutingAssembly().Location.
						Replace("/", "\\") == current.MainModule.FileName) 
 
					{  
						//Return the other process instance.  
						return process; 
 					}  
				}  
			} 
			//No other instance was found, return null.  
			return null;  
		}

		private void StartTimer_Tick(object sender, System.EventArgs e)
		{
			try
			{
				if (mainForm == null)
				{
					InitializeMainMogForm(mCommandLineArgs);
				}
				else
				{
					if (mInitializationComplete)
					{
						Visible = false;

						this.Splash.Dispose();
						if (bitmap != null) 
						{
							bitmap.Dispose();
							bitmap = null;
						}

						StartTimer.Stop();
						mainForm.Show();					
					}
				}
			}
			catch( Exception ex )
			{
				MOG_Report.ReportMessage( "Major Error", "MOG cannot start because of the following:\r\n\r\n" + ex.Message, ex.StackTrace, MOG_ALERT_LEVEL.CRITICAL );
				this.Close();
				this.Dispose( true );
			}
		}

		private void SplashForm_Click(object sender, System.EventArgs e)
		{
			BringToFront();
		}

		private static void form_Closing(object sender, CancelEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine( "Form is closing." );
		}

		private static void form_Closed(object sender, EventArgs e)
		{
			System.Diagnostics.Debug.WriteLine( "Form is closed." );
		}

		private static void form_HandleDestroyed(object sender, EventArgs e)
		{
			System.Diagnostics.Debug.WriteLine( "Form handle destroyed." );
		}
	}
}
