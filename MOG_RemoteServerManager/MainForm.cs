using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MOG.REPORT;
using MOG;
using System.Threading;
using MOG.PROMPT;
using MOG_ControlsLibrary.MogUtils_Settings;

namespace MOG_RemoteServerManager
{
	public partial class MainForm : Form
	{
		public Thread mMogProcess;

		public MainForm()
		{
			string PrefsLocation = String.Concat(Environment.CurrentDirectory, "\\MOGPrefs.ini");
			MogUtils_Settings.CreateSettings(PrefsLocation);

			InitializeComponent();

			InitializeMOG();
		}

		private void InitializeMOG()
		{
			// Check to see if we can init the server
			if (!MOG_Main.Init_Client("", "Remote Server Manager"))
			{
				// Inform the user that we failed to connect and will continue in an offline mode
				string message = "Failed to connect to the MOG Server.\n" +
								 "Please be advised that you will most likely experience limited functionality during this session.";
				MOG_Report.ReportMessage("Connection Failed", message, "", MOG_ALERT_LEVEL.ALERT);
			}

			RemoteServerSettings.Initialize();			
		}

		private void MogProcess()
		{
			while (!MOG_Main.IsShutdown())
			{
				try
				{
					MOG_Main.Process();
				}
				catch (Exception ex)
				{
					MOG_Report.ReportMessage("MOG Process", ex.Message, ex.StackTrace, MOG_ALERT_LEVEL.CRITICAL);
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

		private void MainForm_Load(object sender, EventArgs e)
		{
			if (mMogProcess == null)
			{
				mMogProcess = new Thread(new ThreadStart(this.MogProcess));
				mMogProcess.Name = "RemoteServerManager::MogProcess";
				mMogProcess.Start();
			}
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{			
			MOG_Main.Shutdown();
			RemoteServerSettings.SaveSettings();
		}
	}
}