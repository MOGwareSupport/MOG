using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Diagnostics;
using System.Reflection;

using MOG;
using MOG.PROMPT;
using MOG.PROGRESS;
using AppLoading;
using MOG_ControlsLibrary.MogUtils_Settings;
using System.IO;

namespace MOG_EventViewer
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class MOG_EventViewerMainForm : System.Windows.Forms.Form
	{
		#region System defs
		private MOG_EventViewer.MOG_ControlsLibrary.CriticalEventViewerControl criticalEventViewerControl1;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem MainFileMenuItem;
		private System.Windows.Forms.MenuItem MainExitMenuItem;
		private System.Windows.Forms.MenuItem MainAboutMenuItem;
		private MenuItem MainViewMenuItem;
		private MenuItem RefreshMenuItem;
		private MenuItem menuItem1;
		private IContainer components;


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MOG_EventViewerMainForm));
			this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
			this.MainFileMenuItem = new System.Windows.Forms.MenuItem();
			this.MainExitMenuItem = new System.Windows.Forms.MenuItem();
			this.MainViewMenuItem = new System.Windows.Forms.MenuItem();
			this.RefreshMenuItem = new System.Windows.Forms.MenuItem();
			this.MainAboutMenuItem = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.criticalEventViewerControl1 = new MOG_EventViewer.MOG_ControlsLibrary.CriticalEventViewerControl();
			this.SuspendLayout();
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.MainFileMenuItem,
            this.MainViewMenuItem,
            this.MainAboutMenuItem});
			// 
			// MainFileMenuItem
			// 
			this.MainFileMenuItem.Index = 0;
			this.MainFileMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.MainExitMenuItem});
			this.MainFileMenuItem.Text = "File";
			// 
			// MainExitMenuItem
			// 
			this.MainExitMenuItem.Index = 0;
			this.MainExitMenuItem.Text = "Exit";
			this.MainExitMenuItem.Click += new System.EventHandler(this.MainExitMenuItem_Click);
			// 
			// MainViewMenuItem
			// 
			this.MainViewMenuItem.Index = 1;
			this.MainViewMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.RefreshMenuItem});
			this.MainViewMenuItem.Text = "View";
			// 
			// RefreshMenuItem
			// 
			this.RefreshMenuItem.Index = 0;
			this.RefreshMenuItem.Shortcut = System.Windows.Forms.Shortcut.F5;
			this.RefreshMenuItem.Text = "Refresh";
			this.RefreshMenuItem.Click += new System.EventHandler(this.menuItem1_Click);
			// 
			// MainAboutMenuItem
			// 
			this.MainAboutMenuItem.Index = 2;
			this.MainAboutMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1});
			this.MainAboutMenuItem.Text = "About";
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.Text = "About MOG";
			this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click_1);
			// 
			// criticalEventViewerControl1
			// 
			this.criticalEventViewerControl1.Database = null;
			this.criticalEventViewerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.criticalEventViewerControl1.Location = new System.Drawing.Point(0, 0);
			this.criticalEventViewerControl1.Name = "criticalEventViewerControl1";
			this.criticalEventViewerControl1.Size = new System.Drawing.Size(800, 257);
			this.criticalEventViewerControl1.TabIndex = 0;
			// 
			// MOG_EventViewerMainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(800, 257);
			this.Controls.Add(this.criticalEventViewerControl1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Menu = this.mainMenu1;
			this.Name = "MOG_EventViewerMainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "MOG Event Viewer";
			this.Load += new System.EventHandler(this.MOG_EventViewerMainForm_Load);
			this.Closed += new System.EventHandler(this.MOG_EventViewerMainForm_Closed);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MOG_EventViewerMainForm_KeyUp);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MOG_EventViewerMainForm_FormClosing);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
            //Control.CheckForIllegalCrossThreadCalls = false;

            try
			{
				// Check for multiple instances
				Process mogProcess = RunningInstance();
				if (mogProcess != null)
				{
					// we've got a duplicate process - ask the user if they want to continue anyway
					DialogResult res = MessageBox.Show("An instance of the MOG EventViewer is already running on this machine.\n"
						+ "It is possible that this instance is the result of a crash or other problem,\n"
						+ "in which case you should contact your MOG or network administrator.\n\n"
						+ "Would you like to start another viewer?", 
						"Client Already Running", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

					// if not, shutdown
					if (res == DialogResult.No)
					{
						return;
					}

					// user wants to spawn a new instance
				}

				Application.Run(new MOG_EventViewerMainForm());
			}
			catch (Exception e)
			{
				e.ToString();
			}
		}
		#endregion

		public MOG_EventViewerMainForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();			
		}

		public void InitializeSystem(SplashForm SplashScreen)
		{
			// Initialize the .Net style menus
			SplashScreen.updateSplash("Initializing menu system...", 10);

			// Initialize MOG
			SplashScreen.updateSplash("Connecting MOG SQL\\Server...", 40);

			try
			{
				// Check to see if we can init the server?
				if (!MOG_Main.Init_Client("", "Event Viewer"))
				{
					string message = "Was unable to initialize because there is another viewer already running.";
					throw( new Exception(message));
				}

				criticalEventViewerControl1.RefreshEvents();
			}
			catch(Exception e)
			{
				string message =	e.Message + "\n" + 
					e.StackTrace;
				throw( new Exception(message));
			}

			MOG_Prompt.ClientForm = this;

			SplashScreen.updateSplash("Connected...", 100);

			SplashScreen.updateSplash("Done", 1);
			SplashScreen.mInitializationComplete = true;

			MogUtils_Settings.CreateSettings(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "EventViewerPrefs.ini"));
		}

		private void MOG_EventViewerMainForm_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyData == Keys.F5)
				this.criticalEventViewerControl1.RefreshEvents();
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

		private void Shutdown()
		{
			// Inform MOG we are shutting down
			MOG_Main.Shutdown();
			// Terminate our application
			Application.Exit();
		}

		private void MOG_EventViewerMainForm_Closed(object sender, System.EventArgs e)
		{
			Shutdown();
		}

		private void MainExitMenuItem_Click(object sender, System.EventArgs e)
		{
			Shutdown();
		}

		private void menuItem1_Click(object sender, EventArgs e)
		{
			criticalEventViewerControl1.RefreshEvents();
		}

		private void menuItem1_Click_1(object sender, EventArgs e)
		{
			string WhatsNewFilename = MOG.MOG_Main.GetExecutablePath() + "\\WhatsNew.txt";
			AboutForm about = new AboutForm(WhatsNewFilename);
			about.ShowDialog(this);
		}

		private void MOG_EventViewerMainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (WindowState == FormWindowState.Maximized)
			{
				MogUtils_Settings.Settings.PutPropertyString("GuiLayout", "Gui", "Width", Convert.ToString(Width - 8));
				MogUtils_Settings.Settings.PutPropertyString("GuiLayout", "Gui", "Height", Convert.ToString(Height - 8));
			}
			// Else save normally...
			else
			{
				MogUtils_Settings.SaveSetting("GuiLayout", "Gui", "Width", Convert.ToString(Width));
				MogUtils_Settings.SaveSetting("GuiLayout", "Gui", "Height", Convert.ToString(Height));
			}

			MogUtils_Settings.SaveSetting("GuiLayout", "Gui", "top", Convert.ToString(Top));
			MogUtils_Settings.SaveSetting("GuiLayout", "Gui", "left", Convert.ToString(Left));

			MogUtils_Settings.Save();
		}

		private void MOG_EventViewerMainForm_Load(object sender, EventArgs e)
		{
			// Set the size
			Width = MogUtils_Settings.LoadIntSetting("GuiLayout", "Gui", "Width", 800);
			Height = MogUtils_Settings.LoadIntSetting("GuiLayout", "Gui", "Height", 600);
			// Set the location on screen
			if (MogUtils_Settings.Settings.PropertyExist("GuiLayout", "Gui", "left")) Left = MogUtils_Settings.LoadIntSetting("GuiLayout", "Gui", "left");
			if (MogUtils_Settings.Settings.PropertyExist("GuiLayout", "Gui", "top")) Top = MogUtils_Settings.LoadIntSetting("GuiLayout", "Gui", "top");

			// Check for client being visible
			Rectangle workArea = Screen.GetWorkingArea(this);

			if (Top < 0) Top = 0;
			if (Left < 0) Left = 0;
			if (Top > workArea.Bottom) Top = workArea.Bottom - Height;
			if (Left > workArea.Right) Left = workArea.Left - Width;
		}
	}
}
