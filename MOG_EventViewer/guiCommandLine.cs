using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;


using MOG;
using MOG.REPORT;
using MOG.PROMPT;
using MOG.DOSUTILS;



namespace MOG_Client.Client_Mog_Utilities
{
	/// <summary>
	/// Summary description for guiCommandLine.
	/// </summary>
	public class guiCommandLine
	{
		static public int ShellExecute(string command)
		{
			string output = "";
            return ShellExecute(command, "", ProcessWindowStyle.Normal, ref output);
		}

		static public int ShellExecute(string command, bool WaitForExit)
		{
			string output = "";
            return Shell(command, "", ProcessWindowStyle.Normal, ref output, WaitForExit);
		}

		static public int ShellSpawn(string command)
		{
			string output = "";
			return Shell(command, "", ProcessWindowStyle.Normal, ref output, false);
		}

		static public int ShellExecute(string command, string arguments, ProcessWindowStyle windowStyle, ref string output)
		{
			return Shell(command, arguments, windowStyle, ref output, true);
		}

		static public int ShellSpawn(string command, string arguments, ProcessWindowStyle windowStyle, ref string output)
		{
			return Shell(command, arguments, windowStyle, ref output, false);
		}

		static public int ShellSpawn(string command, string arguments)
		{
			string output = "";
			return Shell(command, arguments, ProcessWindowStyle.Normal, ref output, false);
		}
				
		static public int Shell(string command, string arguments, ProcessWindowStyle windowStyle, ref string output, bool WaitForExit)
		{
			int rc = -1;
			// Run the command
			Process p = new Process();
			p.StartInfo.FileName = command;
			p.StartInfo.Arguments = arguments;
			p.StartInfo.WorkingDirectory = DosUtils.PathGetDirectoryPath(command);

			p.StartInfo.WindowStyle = windowStyle;

			if (windowStyle == ProcessWindowStyle.Hidden)
			{
				p.StartInfo.CreateNoWindow = true;
				p.StartInfo.RedirectStandardOutput = true;
				p.StartInfo.RedirectStandardError = true;
				p.StartInfo.UseShellExecute = false;
			}

			try
			{
				p.Start();
			}
			catch(Exception e)
			{
				MOG_Prompt.PromptMessage("ShellExecute", "Command:" + p.StartInfo.FileName + "\n" +
															"Args:" + p.StartInfo.Arguments + "\n" +
															e.Message, e.StackTrace, MOG_ALERT_LEVEL.ERROR);
				return -1;
			}

			if (windowStyle == ProcessWindowStyle.Hidden)
			{
				output = string.Concat("StdOut", p.StandardOutput.ReadToEnd());
				output = string.Concat(output, " StdError:", p.StandardError.ReadToEnd());
				rc = p.ExitCode;
			}

			try
			{
				if (WaitForExit)
				{
					p.WaitForExit();

					rc = p.ExitCode;
					
					// If the process exited with an error, let our user know it was not MOG's fault
					if (rc != 0)
					{
						MOG_Report.ReportMessage("Unable to Open " + Path.GetFileNameWithoutExtension(command),
							"Windows was unable to open the viewer: \r\n\r\n" + command, "", MOG_ALERT_LEVEL.ALERT);
					}

					p.Close();
				}
				else
				{
					rc = 0;
				}
			}
			catch
			{
				p.Close();
			}

			return rc;
		}
	}
}
  