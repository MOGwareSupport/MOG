using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Threading;

using MOG;
using MOG.COMMAND.SLAVE;
using MOG_Slave.Slave_Mog_Utilities;
using MOG.REPORT;
using MOG.PROMPT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG_CoreControls;



namespace MOG_Slave
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class FormMainSlave : System.Windows.Forms.Form
	{
		private System.ComponentModel.IContainer components;
		public System.Windows.Forms.NotifyIcon niconSystemTrayIcon;
		private System.Windows.Forms.ContextMenu cmnuSystemTrayMenu;
		private System.Windows.Forms.MenuItem mnuItemQuit;
		private System.Windows.Forms.MenuItem mnuItemSeparator2;
		private System.Windows.Forms.MenuItem mnuItmInformation;
		private System.Windows.Forms.MenuItem mnuItmHolder;
		private System.Windows.Forms.MenuItem mnuItmShowLog;
		private System.Windows.Forms.MenuItem mnuItmHideCommand;

		public EventCallbacks mEventCallbacks;
		public CommandLineLog mLogWindow;
		public System.Windows.Forms.ImageList SlaveIconsImageList;
		private System.Windows.Forms.ToolTip MOGToolTip;
		public Thread mMogProcess;
		public bool mInitialized;

		private void SlaveMessageWride()
		{
		}

		int MOGS_StringCompare(String String1, String String2)
		{
			return String.Compare(String1, String2, true);
		}

		public FormMainSlave(string[] args)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			if (InitSlave(args))
			{
				InitSlaveInformationMenu();

				mEventCallbacks = new EventCallbacks(this);

				// Startup the mog process loop
				if (mMogProcess == null)
				{
					mMogProcess = new Thread(new ThreadStart(this.MogProcess));
					mMogProcess.Start();
					mInitialized = true;
				}
			}
			else
			{
				mInitialized = false;
			}
		}

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

		private void MogProcess()
		{
			// Loop until we shutdown
			while(!MOG_Main.IsShutdown())
			{
				// Only process while we are not shutting down?
				if (!MOG_Main.IsShutdownInProgress())
				{
					try
					{
						MOG_Main.Process();
					}
					catch(Exception ex)
					{
						MOG_Report.ReportSilent("MogProcess", ex.Message, ex.StackTrace, MOG_ALERT_LEVEL.CRITICAL);
					}
				}
			
				Thread.Sleep(1);
			}

			// Inform our parent thread that we have exited
			mMogProcess = null;

			// Gracefully wait a while to see if our parent thread will terminate the application for us?
			Thread.Sleep(500);
			// At this point, let's take the inititive and shut ourselves down
			Application.Exit();
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FormMainSlave));
			this.niconSystemTrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
			this.cmnuSystemTrayMenu = new System.Windows.Forms.ContextMenu();
			this.mnuItmShowLog = new System.Windows.Forms.MenuItem();
			this.mnuItmHideCommand = new System.Windows.Forms.MenuItem();
			this.mnuItmInformation = new System.Windows.Forms.MenuItem();
			this.mnuItmHolder = new System.Windows.Forms.MenuItem();
			this.mnuItemSeparator2 = new System.Windows.Forms.MenuItem();
			this.mnuItemQuit = new System.Windows.Forms.MenuItem();
			this.SlaveIconsImageList = new System.Windows.Forms.ImageList(this.components);
			this.MOGToolTip = new System.Windows.Forms.ToolTip(this.components);
			// 
			// niconSystemTrayIcon
			// 
			this.niconSystemTrayIcon.ContextMenu = this.cmnuSystemTrayMenu;
			this.niconSystemTrayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("niconSystemTrayIcon.Icon")));
			this.niconSystemTrayIcon.Text = "";
			this.niconSystemTrayIcon.Visible = true;
			this.niconSystemTrayIcon.DoubleClick += new System.EventHandler(this.niconSystemTrayIcon_DoubleClick);
			// 
			// cmnuSystemTrayMenu
			// 
			this.cmnuSystemTrayMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																							   this.mnuItmShowLog,
																							   this.mnuItmHideCommand,
																							   this.mnuItmInformation,
																							   this.mnuItemSeparator2,
																							   this.mnuItemQuit});
			this.cmnuSystemTrayMenu.Popup += new System.EventHandler(this.cmnuSystemTrayMenu_Popup);
			// 
			// mnuItmShowLog
			// 
			this.mnuItmShowLog.Index = 0;
			this.mnuItmShowLog.Text = "Show Log Window...";
			this.mnuItmShowLog.Click += new System.EventHandler(this.mnuItmShowLog_Click);
			// 
			// mnuItmHideCommand
			// 
			this.mnuItmHideCommand.Checked = true;
			this.mnuItmHideCommand.Index = 1;
			this.mnuItmHideCommand.Text = "Hide Command Window";
			this.mnuItmHideCommand.Click += new System.EventHandler(this.mnuItmHideCommand_Click);
			// 
			// mnuItmInformation
			// 
			this.mnuItmInformation.Index = 2;
			this.mnuItmInformation.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																							  this.mnuItmHolder});
			this.mnuItmInformation.Text = "Information";
			// 
			// mnuItmHolder
			// 
			this.mnuItmHolder.Index = 0;
			this.mnuItmHolder.Text = "Put info here";
			// 
			// mnuItemSeparator2
			// 
			this.mnuItemSeparator2.Index = 3;
			this.mnuItemSeparator2.Text = "-";
			// 
			// mnuItemQuit
			// 
			this.mnuItemQuit.Index = 4;
			this.mnuItemQuit.Text = "Quit";
			this.mnuItemQuit.Click += new System.EventHandler(this.mnuItemQuit_Click);
			// 
			// SlaveIconsImageList
			// 
			this.SlaveIconsImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
			this.SlaveIconsImageList.ImageSize = new System.Drawing.Size(16, 16);
			this.SlaveIconsImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("SlaveIconsImageList.ImageStream")));
			this.SlaveIconsImageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// FormMainSlave
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(480, 78);
			this.ContextMenu = this.cmnuSystemTrayMenu;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormMainSlave";
			this.Opacity = 0;
			this.ShowInTaskbar = false;
			this.Text = "FormMainSlave";
			this.WindowState = System.Windows.Forms.FormWindowState.Minimized;

		}
		#endregion
        
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args) 
		{
            Control.CheckForIllegalCrossThreadCalls = false;

            FormMainSlave form = new FormMainSlave(args);
			if (form != null && form.mInitialized)
			{
				Application.Run(form);
			}

			// Give the mog server some time to read our socket before we shut down
			Thread.Sleep(500);
		}

		private int getNumArgs(string[] args)
		{
			return args.Length;
		}

		private void InitSlaveInformationMenu()
		{
			// Initialize the Explorer context menu
			mnuItmInformation.MenuItems.Clear();
			
			mnuItmInformation.MenuItems.Add(String.Concat("Server:", MOG_ControllerSystem.GetServerComputerName()));
			mnuItmInformation.MenuItems.Add(String.Concat("ServerIp:", MOG_ControllerSystem.GetServerComputerIP()));
			mnuItmInformation.MenuItems.Add(String.Concat("NetworkID:", MOG_ControllerSystem.GetCommandManager().GetNetwork().GetID()));
            mnuItmInformation.MenuItems.Add(String.Concat("Commands:", Convert.ToString(MOG_ControllerSystem.GetCommandManager().GetCommandsList().Count)));
		}

		private bool InitSlave(string[] args)
		{
			String configFilename;
			bool bInitialized = false;
			int argc = getNumArgs(args);
            
			// Default into SLAVE mode
			configFilename = "";

			// Parse command line options
			if (args.Length > 0)
			{	
				foreach(String arg in args)
				{
					if (arg.IndexOf("mog=") != -1)
					{
						configFilename = arg.Substring(arg.IndexOf("=")+1);
						FileInfo file = new FileInfo(configFilename);
						if (!file.Exists)
						{
							MessageBox.Show(String.Concat("MOG Configutation file (", configFilename, ") does not exist! Exiting..."), "INI Error");
							Shutdown();
							return false;
						}
					}
				}
			}

			// Check command-line argument #2
			if (argc >= 2)
			{
				MessageBox.Show("Command line arguments are not yet supported.");
			}
			else
			{
				// Use the specified config file
				if (argc >= 2)
				{
					configFilename = args[1];
				}

				try
				{
					// Attempt to initialize the Slave?
					if (MOG_Main.Init_Slave(configFilename, "Slave"))
					{
						// Make sure we obtain a valid NetworkID before continuing
						if (MOG_ControllerSystem.RequireNetworkIDInitialization())
						{
							bInitialized = true;
							this.niconSystemTrayIcon.Text = RefreshConnectionToolText();
						}
						else
						{
							// This is a great place to kill the application because there is already another server running
							MOG_Report.ReportMessage("Mog Initialization Error", "Slave was unable to obtain a NetworkID from the server.", "", MOG_ALERT_LEVEL.ALERT);
							MOG_Main.Shutdown();
							return false;
						}
					}
					else
					{
						// The server was shutdown
						Bitmap DisconnectedIcon = new Bitmap(SlaveIconsImageList.Images[1]);			
						niconSystemTrayIcon.Icon = System.Drawing.Icon.FromHandle(DisconnectedIcon.GetHicon());
						niconSystemTrayIcon.Text = "Server is disconnected!";

						// This is a great place to kill the application because there is already another server running
						MOG_Report.ReportMessage("Mog Initialization Error", "Slave was unable to initialize.", "", MOG_ALERT_LEVEL.ALERT);
						MOG_Main.Shutdown();
						return false;
					}
				}
				catch(Exception e)
				{
					MOG_Report.ReportMessage("Mog Initialization Error", e.Message, e.StackTrace, MOG_ALERT_LEVEL.CRITICAL);
					MOG_Main.Shutdown();
					return false;
				}
			}

			return bInitialized;
		}

		internal void Shutdown()
		{
			// Inform MOG we are shutting down
			MOG_Main.Shutdown();

			// Play nice and let the processing thread shutdown on it's own
			int waitTime = 0;
			int sleepTime = 10;
			while (mMogProcess != null)
			{
				// Gracefully sleep while we wait for the process thread to shutdown
				Thread.Sleep(sleepTime);
				waitTime += sleepTime;
				// Check if we have waited long enough?
				if (waitTime >= 1000)
				{
					// Forcible terminate the processing thread
					mMogProcess.Abort();
					mMogProcess = null;
				}
			}

			// Terminate our application
			Application.Exit();
		}

		public string RefreshConnectionToolText()
		{
			// get SQL server and MOG Repository drive mapping info
			string connectionString = MOG_ControllerSystem.GetDB().GetConnectionString();
			string sqlServerName = "NONE";
			if (connectionString.ToLower().IndexOf("data source") != -1)
			{
				// parse out sql server name
				//sqlServerName = connectionString.Substring(connectionString.ToLower().IndexOf("data source=")+13, 
				string[] substrings = connectionString.Split(";".ToCharArray());

				// look for correct part o' the connection string
				foreach (string substring in substrings)
				{
					if (substring.ToLower().StartsWith("data source="))
					{
						sqlServerName = substring.Substring( substring.IndexOf("=")+1 ).ToUpper();
					}
				}
			}

			try
			{
				// get MOG repository drive and mapping info
				string mogDrive = MOG_ControllerSystem.GetSystemRepositoryPath();
				char mogDriveLetter = Path.GetPathRoot(mogDrive)[0];
				string mogDriveTarget = new NetworkDriveMapper().GetDriveMapping(mogDriveLetter);

				// Connected to X-SERVER @IP 192.168.5.5; SQL Server: JBIANCHI; MOG Repository M: mapped to \\GX\MOG
				//ConnectionString=Packet size=4096;integrated security=SSPI;data source=NEMESIS;persist security info=False;initial catalog=mog16;

				return string.Concat(MOG_ControllerSystem.GetServerComputerName(), " SQL SERVER: ", sqlServerName);
			}
			catch(Exception e)
			{
				MOG_Report.ReportMessage("RefreshConnectionToolText", e.Message, e.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
			}

			return "Connected";
		}

		private void mnuItemQuit_Click(object sender, System.EventArgs e)
		{
			Shutdown();
		}

		private void cmnuSystemTrayMenu_Popup(object sender, System.EventArgs e)
		{
			//InitSlaveInformationMenu();
		}

		// Show the command line output
		private void mnuItmShowLog_Click(object sender, System.EventArgs e)
		{
			MOG_CommandSlave manager = (MOG_CommandSlave)MOG_ControllerSystem.GetCommandManager();
			mLogWindow = new CommandLineLog(manager);
			mLogWindow.Show();
		}
		private void niconSystemTrayIcon_DoubleClick(object sender, System.EventArgs e)
		{
			MOG_CommandSlave manager = (MOG_CommandSlave)MOG_ControllerSystem.GetCommandManager();
			mLogWindow = new CommandLineLog(manager);
			mLogWindow.Show();
		}

		private void mnuItmHideCommand_Click(object sender, System.EventArgs e)
		{
			MOG_CommandSlave manager = (MOG_CommandSlave)MOG_ControllerSystem.GetCommandManager();
            MenuItem menu = (MenuItem)sender;
			if (menu.Checked)
			{
				menu.Checked = false;
			}
			else
			{
				menu.Checked = true;
			}
			// Set the commanger to be the same
			manager.SetCommandLineHideWindow(menu.Checked);
		}		
	}
}

