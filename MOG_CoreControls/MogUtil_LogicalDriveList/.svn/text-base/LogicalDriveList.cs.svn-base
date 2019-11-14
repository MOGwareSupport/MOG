using System;
using System.Windows.Forms;
using System.Collections;

namespace MOG_CoreControls
{
	/// <summary>
	/// Summary description for NetworkDriveCollection.
	/// </summary>
	public class LogicalDriveList
	{
		protected NetworkDriveMapper driveMapper;
		protected ArrayList list;

		public bool Interactive { get { return this.driveMapper.Interactive; } set { this.driveMapper.Interactive = value; } }
		public bool Prompt { get { return this.driveMapper.Prompt; } set { this.driveMapper.Prompt = value; } }
		public bool Persistent { get { return this.driveMapper.Persistent; } set { this.driveMapper.Persistent = value; } } 
		public bool SaveCredentials { get { return this.driveMapper.SaveCredentials; } set { this.driveMapper.SaveCredentials = value; } } 
		public bool Force { get { return this.driveMapper.Force; } set { this.driveMapper.Force = value; } } 
		public bool UseCommandLine { get { return this.driveMapper.UseCommandLine; } set { this.driveMapper.UseCommandLine = value; } } 

		public ArrayList MappedDrives 
		{
			get 
			{
				ArrayList mList = new ArrayList();
				foreach (LogicalDrive drive in this.list)
				{
					if (drive.Mapped)
						mList.Add(drive);
				}

				return mList;
			}
		}

		public ArrayList FreeDrives 
		{
			get 
			{
				ArrayList fList = new ArrayList();
				foreach (LogicalDrive drive in this.list)
				{
					if (drive.Free)
						fList.Add(drive);
				}

				return fList;			
			}
		}

		public ArrayList LocalDrives 
		{
			get 
			{
				ArrayList lList = new ArrayList();
				foreach (LogicalDrive drive in this.list)
				{
					if (drive.Local)
						lList.Add(drive);
				}

				return lList;			
			}
		}


		public LogicalDriveList()
		{
			this.driveMapper = new NetworkDriveMapper();
			this.driveMapper.Interactive = false;
			this.driveMapper.Persistent = false;
			this.driveMapper.Prompt = false;
			this.driveMapper.SaveCredentials = false;
			this.driveMapper.UseCommandLine = false;
		}
		
		public void PopulateComboBox(System.Windows.Forms.ComboBox cBox)
		{
			PopulateComboBox(cBox, "   ");
		}

		public void PopulateComboBox(System.Windows.Forms.ComboBox cBox, string seperator)
		{
			cBox.Items.Clear();
			foreach (LogicalDrive drive in this.list)
			{
				if (!drive.Local)
					cBox.Items.Add( drive.Drive + seperator + drive.Path );
			}
		}
		public void PopulateComboBoxNoMapping(System.Windows.Forms.ComboBox cBox)
		{
			cBox.Items.Clear();
			foreach (LogicalDrive drive in this.list)
			{
				if (!drive.Local)
					cBox.Items.Add( drive.Drive );
			}
		}

		public void PopulateComboBoxMappedOnlyWithMapping(System.Windows.Forms.ComboBox cBox, string seperator)
		{
			cBox.Items.Clear();
			foreach (LogicalDrive drive in this.MappedDrives)
			{
				if (!drive.Local)
					cBox.Items.Add( drive.Drive + seperator + drive.Path );
			}
		}
		public void PopulateComboBoxMappedOnlyNoMapping(System.Windows.Forms.ComboBox cBox)
		{
			cBox.Items.Clear();
			foreach (LogicalDrive drive in this.MappedDrives)
			{
				if (!drive.Local)
					cBox.Items.Add( drive.Drive );
			}
		}

		public void PopulateComboBoxFreeOnly(System.Windows.Forms.ComboBox cBox)
		{
			cBox.Items.Clear();
			foreach (LogicalDrive drive in this.FreeDrives)
			{
				if (!drive.Local)
					cBox.Items.Add( drive.Drive );
			}
		}



		public string GetMapping(string driveLetter)
		{
			if (driveLetter == null)
				return null;

			return GetMapping( driveLetter.Trim().ToCharArray()[0] );
		}

		public string GetMapping(char driveLetter)
		{
			LogicalDrive drive = Get(driveLetter);
			if (drive != null)
				return drive.Path;
			else
				return null;
		}

		public void Refresh() 
		{
			this.list = new ArrayList();

			ArrayList driveInfo = this.driveMapper.GetDriveDescriptions();

			foreach (string drive in driveInfo) 
			{
				//c:?local drive
				string[] lines = drive.Split("?".ToCharArray());
				if (lines[1].ToLower().StartsWith("network"))
				{
					char driveLetter = lines[0].ToCharArray()[0];
					string path = this.driveMapper.GetDriveMapping(driveLetter);
					this.list.Add( new LogicalDrive( driveLetter, path, LogicalDriveType.Mapped ) );
				}
				else 
				{
					char driveLetter = lines[0].ToCharArray()[0];
					this.list.Add(new LogicalDrive(driveLetter, driveLetter.ToString() + ":\\", LogicalDriveType.Local));
				}
			}

			for (char c = 'a'; c <= 'z'; c++)
			{
				if ( Get(c) == null )
					this.list.Add( new LogicalDrive( c, "", LogicalDriveType.Free ) );
			}
		}

		private LogicalDrive Get(char driveLetter)
		{
			foreach (LogicalDrive drive in this.list)
			{
				if (drive.DriveLetter.ToString().ToLower().ToCharArray()[0] == driveLetter.ToString().ToLower().ToCharArray()[0])
					return drive;
			}

			return null;
		}

		private LogicalDrive Get(string drive) 
		{
			if ( !Char.IsLetter(drive.ToCharArray()[0]) )
				return null;

			return Get( drive.ToCharArray()[0] );
		}

		public bool MapDrive(char letter, string path)
		{
            LogicalDrive drive = this.Get(letter);
			if (drive == null)
			{
				//MessageBox.Show("Drive is null");
				return false;
			}
			if (drive.Local)
			{
				//MessageBox.Show("Drive is local");
				return false;
			}

			return this.driveMapper.MapDrive(path, drive.Drive);
		}

		public bool MapDrive(string drive, string path) 
		{
			if ( !Char.IsLetter( drive.ToCharArray()[0] ) )
				return false;

			return this.MapDrive( drive.ToCharArray()[0], path );
		}

		public bool UnMapDrive(char letter)
		{
			LogicalDrive drive = this.Get(letter);
			if (drive == null)
				return false;
			if (drive.Local)
				return false;

			return this.driveMapper.UnMapDrive(drive.Drive);
		}

		public bool UnMapDrive(string drive)
		{
			if ( !Char.IsLetter( drive.ToCharArray()[0] ) )
				return false;

			return this.UnMapDrive( drive.ToCharArray()[0] );
		}

		public bool IsLocalDrive(char driveLetter)
		{
			foreach (LogicalDrive drive in this.list)
			{
				if (drive.Local && drive.DriveLetter.ToString().ToLower().ToCharArray()[0] == driveLetter.ToString().ToLower().ToCharArray()[0])
					return true;
			}

			return false;
		}

		public bool IsMappedDrive(char driveLetter)
		{
			foreach (LogicalDrive drive in this.list)
			{
				if (drive.Mapped && drive.DriveLetter.ToString().ToLower().ToCharArray()[0] == driveLetter.ToString().ToLower().ToCharArray()[0])
					return true;
			}

			return false;
		}

		public bool IsFreeDrive(char driveLetter)
		{
			foreach (LogicalDrive drive in this.list)
			{
				if (drive.Free && drive.DriveLetter.ToString().ToLower().ToCharArray()[0] == driveLetter.ToString().ToLower().ToCharArray()[0])
					return true;
			}

			return false;
		}

	}
}


