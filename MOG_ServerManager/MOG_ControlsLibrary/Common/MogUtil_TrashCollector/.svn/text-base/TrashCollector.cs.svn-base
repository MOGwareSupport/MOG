using System;
using System.IO;
using System.Collections;
using System.Windows.Forms;

using MOG_ServerManager.Utilities;

namespace MOG_ServerManager
{
	/// <summary>
	/// Summary description for TrashCollector.
	/// </summary>
	public class TrashCollector
	{
		#region Member vars
		private ArrayList commandList = new ArrayList();
		private ArrayList trashBinList = new ArrayList();
		private bool autoSave = true;
		private string logFilename = "";
		private bool logActions = true;
		#endregion
		#region Properties
		public string LogFilename
		{
			get { return this.logFilename; }
			set { this.logFilename = value; }
		}
		public bool LogActions
		{
			get { return this.logActions; }
			set { this.logActions = value; }
		}
		public bool AutoSave
		{
			get { return this.autoSave; }
			set { this.autoSave = value; }
		}
		#endregion
		#region Constructor
		public TrashCollector()
		{
		}
		#endregion
		#region Public functions

		// --------------------------------------
		//  GetTrashBin() - Returns the UserTrashBin in this.trashBinList with TrashPath property set to passed-in path
		//		If none exists, one is created, added to the list, and returned
		//		Is not case sensitive
		public UserTrashBin GetTrashBin(string path)
		{
			// Trim the path to the correct trash dir
			if (path.ToLower().Contains("tools"))
			{
				path = path.ToLower().Replace("tools\\", "");
			}

			foreach (UserTrashBin utb in this.trashBinList)
			{
				if (path.Trim().ToLower() == utb.TrashPath.Trim().ToLower())
					return utb;
			}

			// if we're here, it wasn't in the list, so lets add it
			UserTrashBin bin = new UserTrashBin(path);
			if (this.LogActions)
			{
				bin.LogFilename = this.logFilename;
				bin.LogActions = true;
			}
			
			this.trashBinList.Add(bin);
			return bin;
		}

		public void AddCommand(PurgeCommand cmd)
		{
			if (cmd != null)
			{
				if (this.commandList == null)
					this.commandList = new ArrayList();

				this.commandList.Add(cmd);
			}
		}

		public void ProcessCommands()
		{
			foreach (PurgeCommand cmd in this.commandList)
			{
				if (cmd.Active  &&  cmd.PastDue)
				{
					if (this.logActions)
						Utils.AppendLineToFile(this.logFilename, DateTime.Now.ToString() + ":\tTrashCollector: sending execute signal to command { " + cmd.ToString() + " }");

					cmd.Execute();
				}
			}
		}

		public void ClearCommands()
		{
			this.commandList.Clear();
		}

		public void ResetCommands()
		{
			foreach (PurgeCommand cmd in this.commandList)
				cmd.Reset();
		}

		public void DisplayCommands()
		{
			string msg = "";
			foreach (PurgeCommand cmd in this.commandList)
				msg += cmd.ToString() + "\n";
			MessageBox.Show(msg, "Command Summary");
		}

		public void SaveCommands()
		{
			foreach (PurgeCommand cmd in this.commandList)
				cmd.Save();
		}
		#endregion
	}
}


