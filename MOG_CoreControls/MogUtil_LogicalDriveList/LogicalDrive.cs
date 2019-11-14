using System;

namespace MOG_CoreControls
{
	/// <summary>
	/// Summary description for LogicalDrive.
	/// </summary>
	public class LogicalDrive
	{
		private char driveLetter;
		private string remotePath;
		private LogicalDriveType type;

		public string Drive { get { return this.driveLetter.ToString() + ":"; } }
		public char DriveLetter { get { return this.driveLetter; } set { this.driveLetter = value.ToString().ToLower().ToCharArray()[0]; } }
		public string Path { get { return this.remotePath; } set { this.remotePath = value; } }
		
		public bool Free
		{
			get { return (this.type == LogicalDriveType.Free); }
			set { this.type = LogicalDriveType.Free; }
		}

		public bool Local
		{
			get { return (this.type == LogicalDriveType.Local); }
			set { this.type = LogicalDriveType.Local; }
		}

		public bool Mapped
		{
			get { return (this.type == LogicalDriveType.Mapped); }
			set { this.type = LogicalDriveType.Mapped; }
		}

		public LogicalDrive()
		{
			this.DriveLetter = 'z';
			this.remotePath = "nopath";
		}

		public LogicalDrive(char driveLetter, string path, LogicalDriveType type) 
		{
			this.DriveLetter = driveLetter;
			this.remotePath = path;
			this.type = type;
		}

		public override string ToString()
		{
			if (this.Mapped)
				return this.Drive + "   " + this.remotePath;
			else
				return this.Drive;
		}

	}

	public enum LogicalDriveType { Free, Mapped, Local };
}
