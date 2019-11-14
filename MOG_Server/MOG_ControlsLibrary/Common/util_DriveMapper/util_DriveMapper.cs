using System;
using System.Management;
using System.Collections;
using System.Windows.Forms;
using System.Runtime.InteropServices;


namespace MOG_Server.MOG_ControlsLibrary.Common
{
	/// <summary>
	/// Summary description for util_DriveMapper.
	/// </summary>
	public class util_DriveMapper
	{
		public util_DriveMapper()
		{
		}

		// import unmanaged functions
		[DllImport("mpr.dll")] private static extern int WNetAddConnection2A(ref NetResource pstNetRes, string psPassword, string psUsername, int piFlags);
		[DllImport("mpr.dll")] private static extern int WNetCancelConnection2A(string psName, int piFlags, int pfForce);
		[DllImport("mpr.dll")] private static extern int WNetConnectionDialog(int phWnd, int piType);
		[DllImport("mpr.dll")] private static extern int WNetDisconnectDialog(int phWnd, int piType);
		
		[DllImport("mpr.dll")] private static extern int WNetCancelConnection2(string lpName, int dwFlags, bool fForce);
		[DllImport("mpr.dll")] private static extern int WNetGetLastError(ref int lpError, char[] lpErrorBuf, int nErrorBufSize, char[] lpNameBuf, int nNameBufSize);
		[DllImport("mpr.dll")] private static extern int WNetAddConnection2(ref NetResource lpNetResource, string lpPassword, string lpUsername, int dwFlags);


		[DllImport("mpr.dll")] private static extern int WNetGetConnection(string localName, string remoteName, ref int remoteNameSize);
		//[DllImport("mpr.dll")] private static extern int WNetGetUniversalName(string localPath, int infoLevel,  ref UniversalNameInfo lpBuffer, ref int bufferSize);

		[DllImport("mpr", CharSet=CharSet.Auto)]
		private static extern int WNetGetUniversalName (string localPath,
			int infoLevel, ref UniversalNameInfo buffer, ref int bufferSize);

		[DllImport("mpr", CharSet=CharSet.Auto)]
		private static extern int WNetGetUniversalName (string localPath,
			int infoLevel, IntPtr buffer, ref int bufferSize);
		
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

		private const int RESOURCE_CONNECTED			= 0x00000001;
		private const int RESOURCEDISPLAYTYPE_SERVER	= 0x00000002;

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

		public static bool MapDrive(string drive, string unc)
		{
			// CHAR szDeviceName[80];
			// DWORD cchBuff = sizeof(szDeviceName);
			// DWORD int rc, rc2;
			string szDeviceName = "01234567890123456789012345678901234567890123456789012345678901234567890123456789";
			int cchBuff = szDeviceName.Length;
			int rc, rc2;

			NetResource resource = new NetResource();

			resource.scope    = RESOURCE_CONNECTED;
			resource.type     = RESOURCETYPE_DISK;
			resource.displayType = RESOURCEDISPLAYTYPE_SERVER;

			// Map the network drive
			resource.comment      = null;
			resource.provider     = null;

			// Check to see if this connected drive is correct
			if(WNetGetConnection( drive, szDeviceName, ref cchBuff) == NO_ERROR)
			{
				if(szDeviceName.ToLower() != unc.ToLower())
				{
					string local = drive;
					resource.localName    = local;
					resource.remoteName   = unc;

					rc = WNetCancelConnection2( resource.localName, CONNECT_UPDATE_PROFILE, true);
					if (rc!=NO_ERROR)
					{			
						char[] buffer = new char[255];
						char[] buffer2 = new char[255];
						rc2 =  WNetGetLastError(
							ref rc,     // error code
							buffer,   // error description buffer
							255, // size of description buffer
							buffer2,    // buffer for provider name
							255   // size of provider name buffer
							);
					}
					else
					{
						rc = WNetAddConnection2(ref resource, null, null, CONNECT_UPDATE_PROFILE);
						if (rc !=NO_ERROR)
						{
							return false;
						}
						else
						{
							return true;
						}
					}
				}
				else
				{
					return true;
				}
			}
				// Try to connect it without disconnection
			else
			{
				resource.localName    = drive;
				resource.remoteName   = unc;

				rc = WNetAddConnection2(ref resource, null, null, CONNECT_UPDATE_PROFILE);
				if (rc != NO_ERROR)
				{
					return false;
				}
				else
				{
					return true;
				}
			}

			return true;
		}



		public static bool DisconnectDrive(string drive)
		{
			string szDeviceName = "01234567890123456789012345678901234567890123456789012345678901234567890123456789";
			int cchBuff = szDeviceName.Length;
			int rc, rc2;
			//bool nothing = true;

			NetResource resource = new NetResource();

			resource.scope    = RESOURCE_CONNECTED;
			resource.type     = RESOURCETYPE_DISK;
			resource.displayType = RESOURCEDISPLAYTYPE_SERVER;

			// Map the network drive
			resource.comment      = null;
			resource.provider     = null;

			// Check to see if this connected drive is correct
			if(WNetGetConnection( drive, szDeviceName, ref cchBuff ) == NO_ERROR)
			{
				string local = drive;
				resource.localName    = local;
				resource.remoteName   = "";//(char*)unc.c_str();

				//nothing = false;

				rc = WNetCancelConnection2( resource.localName, CONNECT_UPDATE_PROFILE, true);
				if(rc != NO_ERROR)
				{			
					char[] buffer = new char[255];
					char[] buffer2 = new char[255];

					rc2 =  WNetGetLastError(
						ref rc,     // error code
						buffer,   // error description buffer
						255, // size of description buffer
						buffer2,    // buffer for provider name
						255   // size of provider name buffer
						);

					return false;
				}
				else
				{
					return true;
				}
			}
			else
			{
				return true;
			}
		}
	}
}
