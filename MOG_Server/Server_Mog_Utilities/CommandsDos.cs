using System;
using System.Diagnostics;
using System.Windows.Forms;

using MOG.REPORT;
using MOG.PROMPT;

namespace MOG_Server.Server_Mog_Utilities
{
	/// <summary>
	/// Summary description for CommandsDos.
	/// </summary>
	public class CommandsDos
	{
		public CommandsDos()
		{
			
		}

		static public int ShellExecute(string command, string arguments)
		{
			//string output = "";
			int rc = 0;
			
			// Run the command
			Process p = new Process();
			p.StartInfo.FileName = command;
			p.StartInfo.Arguments = arguments;
  			        
			try
			{
				p.Start();
			}
			catch(Exception e)
			{
				MOG_Report.ReportMessage("SpawnDosCommand Error", String.Concat("Could not waitForExit (", e.Message, ")"), e.StackTrace, MOG_ALERT_LEVEL.CRITICAL);
			}

			try
			{
				p.WaitForExit();
			}
			catch(Exception e)
			{
				MOG_Report.ReportMessage("SpawnDosCommand Error", String.Concat("Could not waitForExit (", e.Message, ")"), e.StackTrace, MOG_ALERT_LEVEL.CRITICAL);
			}


			p.Close();

			return rc;
		}
	}
}
