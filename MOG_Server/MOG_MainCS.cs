using System;
using System.Collections.Generic;
using System.Text;
using MOG.TIME;
using MOG.REPORT;
using System.Windows.Forms;
using MOG;

namespace MOG_Server
{
	public class MOG_MainCS
	{
		public static bool Init_Server(string configFilename, string name)
		{
			MOG_Main.SetName(name);
			
			// Make sure we have a valid config filename?
			if (configFilename.Length == 0)
			{
				configFilename = MOG_Main.BuildDefaultConfigFile();
			}

			// Check our bootup precautions
			if (MOG_Main.CheckInitPrecautions(configFilename))
			{
				// Initialize MOG_REPORT for the server
				MOG_Time time = new MOG_Time();
				// MOG is really stable now so I am shutting off the log files so things will run faster
				//		MOG_Prompt.SetMode(MOG_PROMPT_MODE_TYPE.MOG_PROMPT_FILE);
				MOG_Report.SetLogFileName(String.Concat(Application.StartupPath, "\\Logs\\Server\\Server.", time.FormatString(String.Concat("{Day.2}", "-", "{Month.2}", "-", "{Year.2}", " ", "{Hour.2}", ".", "{Minute.2}", "{ampm}")), ".log"));

				// Create a new system
				MOG_CommandServerCS server = new MOG_CommandServerCS();
				return MOG_ControllerSystemCS.InitializeServer(configFilename, server);
			}

			return false;
		}

	}
}
