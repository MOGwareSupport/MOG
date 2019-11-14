using System;
using System.Runtime.InteropServices;

namespace MOG_Client.Client_Utilities
{
//	[StructLayout(LayoutKind.Explicit, Size=28, CharSet=CharSet.Auto)]
//	public class DM_FILE_ATTRIBUTES 
//	{
//		[FieldOffset(0)]public uint SizeHigh;
//		[FieldOffset(4)]public uint SizeLow; 
//		[FieldOffset(8)]public uint Attributes; 
//		[FieldOffset(12)]public FILETIME CreationTime; 
//		[FieldOffset(20)]public FILETIME ChangeTime; 
//		[FieldOffset(28)]public bool Exist; 
//	};

	/// <summary>
	/// Summary description for XboxUtils.
	/// </summary>
	public class XboxUtils
	{
		//[StructLayout(LayoutKind.Explicit)]
		public struct DM_FILE_ATTRIBUTES 
		{
			public string Name;
            public System.Runtime.InteropServices.ComTypes.FILETIME CreationTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME ChangeTime;
			public long SizeHigh;
			public long SizeLow;
			public long Attributes;
			public int Exist;
		};

		// XBOX Utils functions
		[DllImport("XboxUtils.dll", SetLastError=true)]
		static extern bool XboxFileCopy(string source, string target, bool silent);
		[DllImport("XboxUtils.dll", SetLastError=true)]
		static extern bool XboxFileGet(string source, string target, bool silent);
		[DllImport("XboxUtils.dll", SetLastError=true)]
		static extern bool XboxFileRename(string source, string target);
		[DllImport("XboxUtils.dll", SetLastError=true)]
		static extern bool XboxFileDelete(string source);
		[DllImport("XboxUtils.dll", SetLastError=true)]
		static extern bool XboxFileExist(string source);
		[DllImport("XboxUtils.dll", SetLastError=true)]
		static extern bool XboxDirectoryCreate(string target, bool silent);
		[DllImport("XboxUtils.dll", SetLastError=true)]
		static extern bool XboxDirectoryDelete(string source, bool silent);
		[DllImport("XboxUtils.dll", SetLastError=true)]
		static extern bool XboxSetXboxName(string name, bool silent);
		[DllImport("XboxUtils.dll", SetLastError=true)]
		static extern bool XboxGetFileAttributes(string name, ref DM_FILE_ATTRIBUTES pAttributes);
		[DllImport("XboxUtils.dll", SetLastError=true)]
		static extern UInt64 XboxGetDiskFreeSpace(string name);
		[DllImport("XboxUtils.dll", SetLastError=true)]
		static extern string XboxGetFiles(string name);
		[DllImport("XboxUtils.dll", SetLastError=true)]
		static extern uint XboxGetFileSize(string name);
		
		public XboxUtils()
		{
		}

		static public bool FileExist(string filename)
		{
			DM_FILE_ATTRIBUTES att = new DM_FILE_ATTRIBUTES();
			XboxGetFileAttributes(filename, ref att);
			if (att.ChangeTime.dwHighDateTime != 0 || 
				att.ChangeTime.dwLowDateTime != 0 || 
				att.CreationTime.dwHighDateTime != 0 ||
				att.CreationTime.dwLowDateTime != 0 ||
				att.SizeHigh > 0 ||
				att.SizeLow > 0
				)
			{
				return true;
			}


			return false;
			//return (att.SizeHigh != 0 && att.SizeLow != 0);
		
			//bool exist = XboxFileExist(filename);
			//return exist;
		}

		static public bool FileCopyVerify(string source, string target, bool silent)
		{
				XboxFileCopy(source, target, silent);
				return FileExist(target);
		}

		static public bool FileCopy(string source, string target, bool silent)
		{
			XboxFileCopy(source, target, silent);
			return (Marshal.GetLastWin32Error() == 0);
		}

		static public bool FileGet(string source, string target, bool silent)
		{
			XboxFileGet(source, target, silent);
			return (Marshal.GetLastWin32Error() == 0);
		}

		static public bool FileRename(string source, string target)
		{
			XboxFileRename(source, target);
			return (Marshal.GetLastWin32Error() != 0);
		}

		static public bool FileDelete(string source)
		{
			XboxFileDelete(source);
			return (Marshal.GetLastWin32Error() == 0);
		}

		static public bool DirectoryCreateVerify(string target, bool silent)
		{
			XboxDirectoryCreate(target, silent);
			return FileExist(target);
		}
		
		static public bool DirectoryCreate(string target, bool silent)
		{
			XboxDirectoryCreate(target, silent);
			return (Marshal.GetLastWin32Error() != 0);
		}

		static public bool DirectoryDelete(string source, bool silent)
		{
			XboxDirectoryDelete(source, silent);
			return (Marshal.GetLastWin32Error() != 0);
		}

		static public bool SetXboxName(string name, bool silent)
		{
			XboxSetXboxName(name, silent);
			return (Marshal.GetLastWin32Error() != 0);
		}

		static public bool GetFileAttributes(string name, ref DM_FILE_ATTRIBUTES pAttributes)
		{
			XboxGetFileAttributes(name, ref pAttributes);
			return (Marshal.GetLastWin32Error() != 0);
		}

		static public uint GetFileSize(string name)
		{
			return XboxGetFileSize(name);
		}

		static public UInt64 GetDiskFreeSpace(string name)
		{			
			return XboxGetDiskFreeSpace(name);
		}

		static public string GetFiles(string directory)
		{
			return XboxGetFiles(directory);
		}
	}
}
