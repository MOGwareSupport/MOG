using System;
using System.IO;
using System.Collections;
using System.Windows.Forms;

using MOG_ServerManager.Utilities;
using MOG.DOSUTILS;

namespace MOG_ServerManager
{
	/// <summary>
	/// Summary description for UserTrashBin.
	/// </summary>
	public class UserTrashBin
	{
		#region Member vars
		private bool overridesGlobal = false;
		private bool overridesProject = false;
		private string trashPath = "";
		private DateTime lastPurged = new DateTime(1776, 7, 4);
		private string logFilename = "";
		private bool logActions = true;
		#endregion
		#region Properties
		public bool LogActions
		{
			get { return this.logActions; }
			set { this.logActions = value; }
		}

		public string LogFilename
		{
			get { return this.logFilename; }
			set { this.logFilename = value; }
		}

		public string TrashPath
		{
			get { return this.trashPath; }
		}
		
		public bool OverridesGlobal
		{
			get { return this.overridesGlobal; }
			set { this.overridesGlobal = value; }
		}
		public bool OverridesProject
		{
			get { return this.overridesProject; }
			set { this.overridesProject = value; }
		}
		public DateTime LastPurged
		{
			get { return this.lastPurged; }
			set { this.lastPurged = value; }
		}
		#endregion
		#region Constructor
		public UserTrashBin(string trashPath)
		{
			this.trashPath = trashPath;
		}
		#endregion
		#region Public functions
		// ---------------------------------------------------
		// performs purge - if purge scope is overriden, returns false; otherwise performs purge and returns true
		public bool ExecutePurge(PurgeCommand cmd)
		{
			if (!DosUtils.DirectoryExistFast(this.trashPath))
			{
				//MessageBox.Show("Rejected purge of \n\n\t" + this.trashPath + "\n\t(COMMAND: " + cmd.ToString() + ")" + "\n\n because dir does not exist");
				if (this.logActions)
					Utils.AppendLineToFile(this.logFilename, DateTime.Now.ToString() + ":\tUserTrashBin: Rejected purge of '" + this.trashPath + "' from command { " + cmd.ToString() + " } because the specified directory does not exist");
				
				return false;
			}

			if (cmd.Type != PurgeCommandType.FORCE)
			{
				if (cmd.Scope == PurgeCommandScope.GLOBAL  &&  this.overridesGlobal)
				{
					//MessageBox.Show("Rejected purge of \n\n\t" + this.trashPath + "\n\t(COMMAND: " + cmd.ToString() + ")" + "\n\n because it overrides global commands");
					if (this.logActions)
						Utils.AppendLineToFile(this.logFilename, DateTime.Now.ToString() + ":\tUserTrashBin: Rejected purge of '" + this.trashPath + "' from command { " + cmd.ToString() + " } because it overrides global commands");

					return false;		// don't purge if we override global commands
				}

				if (cmd.Scope == PurgeCommandScope.PROJECT  &&  this.overridesProject)
				{
					//MessageBox.Show("Rejected purge of \n\n\t" + this.trashPath + "\n\t(COMMAND: " + cmd.ToString() + ")" + "\n\n because it overrides project commands");
					if (this.logActions)
						Utils.AppendLineToFile(this.logFilename, DateTime.Now.ToString() + ":\tUserTrashBin: Rejected purge of '" + this.trashPath + "' from command { " + cmd.ToString() + " } because it overrides project commands");

					return false;		// don't purge if we override project commands
				}
			}

			
			// execute purge
			if (cmd.Type == PurgeCommandType.FORCE  ||  cmd.Type == PurgeCommandType.INTERVAL)
			{
				try
				{
					Directory.Delete(this.trashPath, true);
					Directory.CreateDirectory(this.trashPath);

					this.lastPurged = DateTime.Now;

					//MessageBox.Show("Purged \n\n\t" + this.trashPath + "\n\t(COMMAND: " + cmd.ToString() + ")" + "\n\nat " + this.lastPurged.ToString(), "Purged");
					if (this.logActions)
						Utils.AppendLineToFile(this.logFilename, DateTime.Now.ToString() + ":\tUserTrashBin: Purged '" + this.trashPath + "' from command { " + cmd.ToString() + " }");
				}
				catch (Exception e)
				{
					//MessageBox.Show("Failed purge of \n\n\t" + this.trashPath + "\n\t(COMMAND: " + cmd.ToString() + ")" + "\n\nat " + this.lastPurged.ToString() + " due to an exception:\n\n" + e.Message + "\n" + e.StackTrace, "Exception");
					if (this.logActions)
						Utils.AppendLineToFile(this.logFilename, DateTime.Now.ToString() + ":\tUserTrashBin: Failed purge of '" + this.trashPath + "' from command { " + cmd.ToString() + " } due to an exception:\n\n" + e.Message + "\n" + e.StackTrace);
				}
			}
			else if (cmd.Type == PurgeCommandType.AGE)
			{
				ArrayList filelist = DeleteFilesByAge(this.trashPath, cmd.DayInterval, cmd.HourInterval, cmd.MinInterval, true);
				//string msg = "";
				//foreach (string file in filelist)
				//	msg += "\t" + file + "\n";
				//MessageBox.Show("Age Purge of " + this.trashPath + " resulted in the following actions:\n\n" + msg);
				if (this.logActions)
				{
					Utils.AppendLineToFile(this.logFilename, DateTime.Now.ToString() + ":\tUserTrashBin: Age-purge of '" + this.trashPath + "' from command { " + cmd.ToString() + " } resulted in the following actions:");
					foreach (string file in filelist)
						Utils.AppendLineToFile(this.logFilename, "\t\t\t\t" + file);
				}

			}
			else if (cmd.Type == PurgeCommandType.MEGS)
			{
				if (DirSizeInMegs( this.trashPath ) > (float)cmd.MegsLimit)
				{
					// purge it baby
					try
					{
						Directory.Delete(this.trashPath, true);
						Directory.CreateDirectory(this.trashPath);

						this.lastPurged = DateTime.Now;

						//MessageBox.Show("Purged (MEGS) \n\n\t" + this.trashPath + "\n\t(COMMAND: " + cmd.ToString() + ")" + "\n\nat " + this.lastPurged.ToString(), "Purged");
						if (this.logActions)
							Utils.AppendLineToFile(this.logFilename, DateTime.Now.ToString() + ":\tUserTrashBin: Megs-purged '" + this.trashPath + "' from command { " + cmd.ToString() + " }");

					}
					catch (Exception e)
					{
						//MessageBox.Show("Failed purge (MEGS) of \n\n\t" + this.trashPath + "\n\t(COMMAND: " + cmd.ToString() + ")" + "\n\nat " + this.lastPurged.ToString() + " due to an exception:\n\n" + e.Message + "\n" + e.StackTrace, "Exception");
						if (this.logActions)
							Utils.AppendLineToFile(this.logFilename, DateTime.Now.ToString() + ":\tUserTrashBin: Failed megs-purge of '" + this.trashPath + "' from command { " + cmd.ToString() + " } due to an exception:\n\n" + e.Message + "\n" + e.StackTrace);
					}
				}
			}

			return true;
		}
		#endregion
		#region Private functions

		private ArrayList DeleteFilesByAge(string path, int days, int hours, int minutes, bool recurse)
		{
			return DeleteFilesByAge(path, 
				((long)days*PurgeCommand.TICKS_PER_DAY) +
				((long)hours*PurgeCommand.TICKS_PER_HOUR) +
				((long)minutes*PurgeCommand.TICKS_PER_MIN),
				recurse);
		}
		private ArrayList DeleteFilesByAge(string path, long interval, bool recurse)
		{
			ArrayList filelist = new ArrayList();

			DirectoryInfo dInfo = new DirectoryInfo(path);

			foreach (FileInfo fInfo in dInfo.GetFiles())
			{
				long difference = DateTime.Now.Ticks - fInfo.LastWriteTime.Ticks;
				if (difference >= interval)
				{
					try 
					{
						if (DosUtils.FileDeleteFast(fInfo.FullName))
						{
							filelist.Add("Deleted " + fInfo.FullName);
							DosUtils.DirectoryDeleteEmptyParentsFast(fInfo.DirectoryName, true);
						}
						else
						{
							filelist.Add("Couldn't delete " + fInfo.FullName);
						}
					}
					catch 
					{
						filelist.Add("Couldn't delete " + fInfo.FullName);
						continue;
					}
					
				}
			}

			if (recurse)
			{
				foreach (string dirname in Directory.GetDirectories(path))
					filelist.AddRange( DeleteFilesByAge(dirname, interval, true) );
			}

			return filelist;
		}


		private float DirSizeInMegs(string path)
		{
			return (float)DirSizeInBytes(path) / (1024*1024);		// meg is 1024 kilobytes (KB = 1024 bytes)
		}

		private long DirSizeInBytes(string path)
		{
			if (!Directory.Exists(path))
				return 0;

			long size = 0;

			DirectoryInfo dInfo = new DirectoryInfo(path);
			foreach (FileInfo fInfo in dInfo.GetFiles())
				size += fInfo.Length;

			foreach (string dirname in Directory.GetDirectories(path))
				size += DirSizeInBytes(dirname);

			return size;
		}
		#endregion
		#region Overriden functions
		public override string ToString()
		{
			return "UserTrashBin: " + this.trashPath + ";  last purged on " + this.lastPurged.ToString();
		}

		#endregion
	}
}



