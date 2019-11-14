
using System;
using System.Management;
using System.Collections;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace MOG_CoreControls
{
	/// <summary>
	/// Summary description for NetworkDriveMapper.
	/// </summary>
	public class NetworkDriveMapper
	{
		private bool interactive;
		private bool prompt;
		private bool persistent;
		private bool saveCredentials;
		private bool force;
		private bool useCommandLine;

		public bool Interactive { get { return this.interactive; } set { this.interactive = value; } }
		public bool Prompt { get { return this.prompt; } set { this.prompt = value; } }
		public bool Persistent { get { return this.persistent; } set { this.persistent = value; } } 
		public bool SaveCredentials { get { return this.saveCredentials; } set { this.saveCredentials = value; } } 
		public bool Force { get { return this.force; } set { this.force = value; } } 
		public bool UseCommandLine { get { return this.useCommandLine; } set { this.useCommandLine = value; } } 


		// import unmanaged functions
		[DllImport("mpr.dll")] private static extern int WNetAddConnection2A(ref NetResource pstNetRes, string psPassword, string psUsername, int piFlags);
		[DllImport("mpr.dll")] private static extern int WNetCancelConnection2A(string psName, int piFlags, int pfForce);
		[DllImport("mpr.dll")] private static extern int WNetConnectionDialog(int phWnd, int piType);
		[DllImport("mpr.dll")] private static extern int WNetDisconnectDialog(int phWnd, int piType);
		
		[DllImport("mpr.dll")] private static extern int WNetGetConnection(string localName, string remoteName, ref int remoteNameSize);
		//[DllImport("mpr.dll")] private static extern int WNetGetUniversalName(string localPath, int infoLevel,  ref UniversalNameInfo lpBuffer, ref int bufferSize);

		[DllImport("mpr", CharSet=CharSet.Auto)]
		private static extern int WNetGetUniversalName (string localPath,
			int infoLevel, ref UniversalNameInfo buffer, ref int bufferSize);

		[DllImport("mpr", CharSet=CharSet.Auto)]
		private static extern int WNetGetUniversalName (string localPath,
			int infoLevel, IntPtr buffer, ref int bufferSize);
		
		//		BOOL CopyFileEx(
		//			LPCTSTR lpExistingFileName,
		//			LPCTSTR lpNewFileName,
		//			LPPROGRESS_ROUTINE lpProgressRoutine,
		//			LPVOID lpData,
		//			LPBOOL pbCancel,
		//			DWORD dwCopyFlags
		//			);
		
		// constants for flag-setting
		private const int RESOURCETYPE_DISK = 0x1;
		//Standard	
		private const int CONNECT_INTERACTIVE = 0x00000008;
		private const int CONNECT_PROMPT = 0x00000010;
		private const int CONNECT_UPDATE_PROFILE = 0x00000001;
		//IE4+
		private const int CONNECT_REDIRECT = 0x00000080;
		//NT5 only
		private const int CONNECT_COMMANDLINE = 0x00000800;
		private const int CONNECT_CMD_SAVECRED = 0x00001000;
		

		// error codes
		private const int NO_ERROR					= 0;
		private const int ERROR_BAD_DEVICE			= 1200;
		private const int ERROR_NOT_CONNECTED		= 2250;
		private const int ERROR_MORE_DATA			= 234;
		private const int ERROR_CONNECTION_UNAVAIL	= 1201;
		private const int ERROR_NO_NETWORK			= 1222;
		//private const int ERROR_EXTENDED_ERROR		= 
		//private const int ERROR_NO_NET_OR_BAD_PATH	= 

		private const int UNIVERSAL_NAME_INFO_LEVEL	= 0x00000001;

		// for use with WNetAddConnection2A()
		[StructLayout(LayoutKind.Sequential)]
			private struct NetResource
		{
			public int scope;
			public int type;
			public int displayType;
			public int usage;
			public string localName;
			public string remoteName;
			public string comment;
			public string provider;
		}

		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
			struct UniversalNameInfo//UNIVERSAL_NAME_INFO
		{
			[MarshalAs(UnmanagedType.LPTStr)]
			public string universalName;
		}


		public NetworkDriveMapper()
		{
			this.interactive = false;
			this.prompt = false;
			this.persistent = false;
			this.saveCredentials = false;
			this.force = false;
			this.useCommandLine = false;
		}

		public bool MapDrive(string remoteName, string localName, string username, string password) 
		{
			//CopyFileEx("foo", "foobar", new System.IntPtr(2), ref ss,  3);

			if (remoteName == null || remoteName == "")
				return false;

			if (localName == null || localName == "")
				return false;

			NetResource nr = new NetResource();
			nr.type = RESOURCETYPE_DISK;
			nr.localName = localName;
			nr.remoteName = remoteName;
			nr.provider = null;

			// unused by WNetAddConnection2A()
			nr.scope = 0;
			nr.displayType = 0;
			nr.usage = 0;
			nr.comment = "";

			// set the flags
			int flags = 0;
			if (this.interactive)
				flags |= CONNECT_INTERACTIVE;
			if (this.prompt)
				flags |= CONNECT_PROMPT;
			if (this.persistent)
				flags |= CONNECT_UPDATE_PROFILE;
			if (this.saveCredentials)
				flags |= CONNECT_CMD_SAVECRED;
			if (this.force)
				flags |= CONNECT_REDIRECT;
			if (this.useCommandLine)
				flags |= CONNECT_COMMANDLINE;

			if ( WNetAddConnection2A(ref nr, password, username, flags) > 0 )
			{
				//MessageBox.Show("WNetAddConnection2A() failed");
				//throw new Exception("Couldn't map network drive");
				return false;
			}

			return true;
		}

		public bool MapDrive(string remoteName, string localName)
		{
			return MapDrive(remoteName, localName, null, null);
		}

		public bool UnMapDrive(string localName)
		{
			if (WNetCancelConnection2A(localName, CONNECT_UPDATE_PROFILE, Convert.ToInt32(false)) > 1)
			{
				//MessageBox.Show("Couldn't disconnect network drive");
				return false;
			}

			return true;
		}

		public void ConnectionDialog(Form parentForm)
		{
			WNetConnectionDialog( parentForm.Handle.ToInt32(), RESOURCETYPE_DISK);
		}

		public void DisconnectionDialog(Form parentForm)
		{
			WNetDisconnectDialog( parentForm.Handle.ToInt32(), RESOURCETYPE_DISK );
		}

		public string GetDriveMapping(char driveLetter)
		{
			string localPath = driveLetter.ToString() + ":";
			UniversalNameInfo info = new UniversalNameInfo();
			info.universalName = "";
			int size = Marshal.SizeOf(info);

			try 
			{
				int retVal = WNetGetUniversalName(localPath, UNIVERSAL_NAME_INFO_LEVEL, ref info, ref size);
				
				if (retVal == 0)
				{
					return info.universalName;
				}
				else if (retVal == ERROR_MORE_DATA)
				{
					IntPtr pBuffer = Marshal.AllocHGlobal(size);
					try 
					{
						retVal = WNetGetUniversalName(
							localPath, UNIVERSAL_NAME_INFO_LEVEL, 
							pBuffer, ref size);

						if (retVal == NO_ERROR)
						{
							info = (UniversalNameInfo)Marshal.PtrToStructure(pBuffer,
								typeof(UniversalNameInfo));
						}
					}
					finally 
					{
						Marshal.FreeHGlobal(pBuffer);
					}
				}
				else
				{
				}
			}
			catch
			{
				//MessageBox.Show(e.Message + "\n\n" + e.StackTrace, "Exception in GetDriveInfo()", MOGPromptButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
				return "";
			}

			return info.universalName;
		}

		public string GetMappingInfo() 
		{
			ArrayList mapInfo = new ArrayList();
			string info = "";

			for (char c = 'a'; c <= 'z'; c++)
			{
				info += c.ToString().ToUpper() + ":\t - " + GetDriveMapping(c) + "\n";
			}

			return info;
		}


		public ArrayList GetDriveDescriptions() 
		{
			ArrayList driveDescriptions = new ArrayList();

			ManagementObjectSearcher DiskSearch = new ManagementObjectSearcher(new SelectQuery("Select * from Win32_LogicalDisk"));
			ManagementObjectCollection moDiskCollection = DiskSearch.Get();

//			MessageBox.Show("Finished getting - " + moDiskCollection.Count.ToString());
			
			foreach(ManagementObject mo in  moDiskCollection)
			{
				driveDescriptions.Add(mo["Name"] + "?" + mo["Description"]);
				mo.Dispose();
			}

//			info += "\n\n\n";

//			ManagementObjectSearcher ShareDiskSearch = new ManagementObjectSearcher(new SelectQuery("Select * from Win32_MappedLogicalDisk"));
//			ManagementObjectCollection moSharedDiskCollection = ShareDiskSearch.Get();
//			foreach(ManagementObject mo in  moSharedDiskCollection)
//			{
//				info += mo["Name"] + "\t - " + mo["Description"] + "\n";
//				mo.Dispose();
//			}

			return driveDescriptions;
		}
	}
}





