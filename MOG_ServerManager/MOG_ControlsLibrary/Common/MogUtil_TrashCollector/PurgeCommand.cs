using System;
using System.IO;
using System.Collections;

using MOG.INI;

namespace MOG_ServerManager
{
	/// <summary>
	/// Summary description for PurgeCommand.
	/// </summary>
	public class PurgeCommand
	{
		#region Member vars
		private bool active = true;
		private PurgeCommandType type;
		private PurgeCommandScope scope;
		private ArrayList targetTrashBinList = new ArrayList();
		private DateTime lastExecuted = new DateTime(1776, 7, 4);
		private string configFilename = "";

		// Interval
		private int dayInterval;
		private int hourInterval;
		private int minInterval;

		// Megs
		private int megLimit;

		#endregion
		#region Properties

		public int DayInterval
		{
			get { return this.dayInterval; }
			set { this.dayInterval = value; }
		}
		public int HourInterval
		{
			get { return this.hourInterval; }
			set { this.hourInterval = value; }
		}
		public int MinInterval
		{
			get { return this.minInterval; }
			set { this.minInterval = value; }
		}

		public ArrayList TargetTrashBinList 
		{
			get { return this.targetTrashBinList; }
			set { this.targetTrashBinList = value; }
		}

		public string ConfigFilename
		{
			get { return this.configFilename; }
			set { this.configFilename = value; }
		}

		public PurgeCommandScope Scope
		{
			get { return this.scope; }
			set { this.scope = value; }
		}

		public PurgeCommandType Type
		{
			get { return this.type; }
			set { this.type = value; }
		}

		public DateTime LastExecuted
		{
			get { return this.lastExecuted; }
			set { this.lastExecuted = value; }
		}

		public bool Active
		{
			get { return this.active; }
		}

		public bool PastDue
		{
			get { return _PastDue(); }
		}

		public int MegsLimit
		{
			get { return this.megLimit; }
		}

		#endregion
		#region Constants
		
		// a 'tick' (as far as the DateTime class is concerned) is one hundred nanoseconds (nanosecond = a billionth of a second, 10^-9 seconds)
		public const long TICKS_PER_DAY	= 864000000000;	// 24 hours per day * 60 mins per hr * 60 secs per min * 10000000000 ns per sec / 100 ns per tick;
		public const long TICKS_PER_HOUR	= 36000000000;	// 60 mins per hr * 60 secs per min * 10000000000 ns per sec / 100 ns per tick;
		public const long TICKS_PER_MIN	= 600000000;	// 60 secs per min * 10000000000 ns per sec / 100 ns per tick;

		#endregion
		#region Constructors
		public PurgeCommand()
		{
			this.type = PurgeCommandType.EMPTY;
			this.active = true;
			this.scope = PurgeCommandScope.NONE;
		}

		#region Interval command constructors
		// for interval commands
		public PurgeCommand(PurgeCommandScope scope, PurgeCommandType type, int dayInterval, int hourInterval, int minInterval)
		{
			this.scope = scope;
			this.type = type;
			this.dayInterval = dayInterval;
			this.hourInterval = hourInterval;
			this.minInterval = minInterval;
		}
		public static PurgeCommand ConstructIntervalCommand(PurgeCommandScope scope, ArrayList trashBins, int dayInterval, int hourInterval, int minInterval)
		{
			PurgeCommand cmd = new PurgeCommand(scope, PurgeCommandType.INTERVAL, dayInterval, hourInterval, minInterval);
			
			foreach (UserTrashBin trashBin in trashBins)
				cmd.AddUserTrash(trashBin);

			return cmd;
		}

		
		public static PurgeCommand ConstructIntervalCommand(PurgeCommandScope scope, UserTrashBin trashBin, int dayInterval, int hourInterval, int minInterval)
		{
			PurgeCommand cmd = new PurgeCommand(scope, PurgeCommandType.INTERVAL, dayInterval, hourInterval, minInterval);

			cmd.AddUserTrash(trashBin);

			return cmd;
		}
		#endregion
		#region Megs command constructors
		// for megs commands
		public PurgeCommand(PurgeCommandScope scope, PurgeCommandType type, int megLimit)
		{
			this.scope = scope;
			this.type = type;
			this.megLimit = megLimit;
		}
		public static PurgeCommand ConstructMegsCommand(PurgeCommandScope scope, ArrayList trashBins, int megLimit)
		{
			PurgeCommand cmd = new PurgeCommand(scope, PurgeCommandType.MEGS, megLimit);
			
			foreach (UserTrashBin trashBin in trashBins)
				cmd.AddUserTrash(trashBin);

			return cmd;
		}
		#endregion
		#region Asset Age command constructors
		
		public static PurgeCommand ConstructAgeCommand(PurgeCommandScope scope, ArrayList trashBins, int dayInterval, int hourInterval, int minInterval)
		{
			PurgeCommand cmd = new PurgeCommand(scope, PurgeCommandType.AGE, dayInterval, hourInterval, minInterval);
			
			foreach (UserTrashBin trashBin in trashBins)
				cmd.AddUserTrash(trashBin);

			return cmd;
		}
		#endregion

		#endregion
		#region Public functions

		public void Reset()
		{
			this.active = true;
		}

		public void Save()
		{
			if (File.Exists(this.configFilename))
			{
				MOG_Ini ini = new MOG_Ini(this.configFilename);
				if (ini != null)
				{
					if (this.type == PurgeCommandType.INTERVAL)
					{
						ini.PutString("INTERVAL", "LastExecuted", this.lastExecuted.ToString());
						ini.PutString("INTERVAL", "DayInterval", this.dayInterval.ToString());
						ini.PutString("INTERVAL", "HourInterval", this.hourInterval.ToString());
						ini.PutString("INTERVAL", "MinInterval", this.minInterval.ToString());

						if (ini.KeyExist("TRASH", "Scope"))
						{
							if (this.scope.ToString().ToLower() != ini.GetString("TRASH", "Scope").ToLower())
								ini.PutString("INTERVAL", "Scope", this.scope.ToString());
						}

						ini.Save();
						ini.Close();
					}
				}
			}
		}

		public bool Execute()
		{
			if (!this.active)
				return false;

			bool retVal = true;

			foreach (UserTrashBin trashBin in this.targetTrashBinList)
			{
				if (!trashBin.ExecutePurge(this))
					retVal = false;
			}

			this.active = false;
			this.lastExecuted = DateTime.Now;

			return retVal;
		}

		public void AddUserTrash(UserTrashBin bin)
		{
			if (this.targetTrashBinList == null)
				targetTrashBinList = new ArrayList();

			this.targetTrashBinList.Add(bin);
		}
		#endregion
		#region Private functions
		public bool _PastDue()
		{
			switch (this.type)
			{
				case PurgeCommandType.INTERVAL:
					long actualTickInterval = DateTime.Now.Ticks - this.lastExecuted.Ticks;
					long neededTickInterval = (this.dayInterval*TICKS_PER_DAY) + (this.hourInterval*TICKS_PER_HOUR) + (this.minInterval*TICKS_PER_MIN);
					return (actualTickInterval >= neededTickInterval);
				case PurgeCommandType.AGE:
					return true;
				case PurgeCommandType.MEGS:
                    return true;
			}

			return false;
		}
		#endregion
		#region Overriden functions
		public override string ToString()
		{
			string trailer = (this.active) ? "Active" : "Not active";
			trailer += ", " + this.scope.ToString();
			trailer += ", " + this.targetTrashBinList.Count.ToString() + " targets";
			trailer += ", last executed " + this.lastExecuted.ToString();
			trailer += ", config file = " + this.configFilename;

			if (this.type == PurgeCommandType.INTERVAL)
			{
				return "Purge every " + this.dayInterval.ToString() + " days, " + this.hourInterval.ToString() + " hours, " + this.minInterval.ToString() + " minutes;  " + trailer;
			}
			else if (this.type == PurgeCommandType.MEGS)
			{
				return "Purge after trash bin contains " + this.megLimit.ToString() + " megabytes;   " + trailer;
			}
			else if (this.type == PurgeCommandType.AGE)
			{
				return "Purge individual assets/files when they are " + " days, " + this.hourInterval.ToString() + " hours, " + this.minInterval.ToString() + " minutes old;  " + trailer;
			}
			else
			{
				return "Empty command, " + trailer;
			}
		}

		#endregion
	}

	#region Public enums
	public enum PurgeCommandScope { NONE, GLOBAL, PROJECT, USER };
	public enum PurgeCommandType { EMPTY, FORCE, INTERVAL, AGE, MEGS };
	#endregion
}

	