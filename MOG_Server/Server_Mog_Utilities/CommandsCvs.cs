using System;
using System.Windows.Forms;
using System.Diagnostics;

using MOG.DOSUTILS;
using MOG.REPORT;
using MOG.PROMPT;

namespace MOG_Server.Server_Mog_Utilities
{
	/// <summary>
	/// Summary description for CommandsCvs.
	/// </summary>
	public class CommandsCvs
	{
		public CommandsCvs()
		{
			
		}

		static public string Checkout(string project, string directory, string toolsPath)
		{
			string command		= "";
			string target		= "";
			string output		= "";
			string arguments	= "";

			target = string.Concat(directory, "\\", project);

			// Check if the target directory exists
			if (DosUtils.DirectoryExist(target))
			{
				if (MessageBox.Show("Target directory already exists!  Checkout will delete current data!  Do you want to continue?", "Checkout", MessageBoxButtons.YesNo) == DialogResult.No)
				{
					return "";
				}
				else
				{
					DosUtils.DirectoryDelete(target);
				}
			}

			// Create dir
			DosUtils.DirectoryCreate(directory);
			Environment.CurrentDirectory = directory;

			command = string.Concat(toolsPath, "\\cvs.bat");
			arguments = string.Concat(directory, " checkout ", project);

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


			p.Close();
			
			return output;
		}

		static public string Update(string project, string directory, string toolsPath)
		{
			string command		= "";
			string output		= "";
			string arguments	= "";

			// set current dir
			Environment.CurrentDirectory = directory;

			command = string.Concat(toolsPath, "\\cvs.bat");
			arguments = string.Concat(directory, " update -d");

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


			p.Close();
			return output;
		}
	}
}
