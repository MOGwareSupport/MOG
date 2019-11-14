using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace MOG_CoreControls.Utilities
{
	public class Win32_Shell
	{
		private const uint ERROR_SUCCESS = 0x0000;
		
		private enum FileOperation : uint
		{
			Move = 0x0001,
			Copy = 0x0002,
			Delete = 0x0003,
			Rename = 0x0004,
		};

		[Flags]
		private enum FileOperationFlag : ushort
		{
			AllowUndo = 0x0040,
			ConfirmMouse = 0x0002,
			FilesOnly = 0x0080,
			MultiDestFiles = 0x0001,
			NoConfirmation = 0x0010,
			NoConfirmMkDir = 0x0200,
			RenameOnCollision = 0x0008,
			Silent = 0x0004,
			SimpleProgress = 0x0100,
			WantMappingHandle = 0x0020,
		}

		private struct SHFILEOPSTRUCT
		{
			public IntPtr hwnd;
			public FileOperation wFunc;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string pFrom;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string pTo;
			public ushort fFlags;
			public bool fAnyOperationsAborted;
			public IntPtr hNameMappings;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string lpszProgressTitle;
		}

		[DllImport("shell32.dll", CharSet = CharSet.Unicode)]
		extern static int SHFileOperation([In] ref SHFILEOPSTRUCT lpFileOp);

		public static bool RecycleFile(string filename)
		{
			SHFILEOPSTRUCT param = new SHFILEOPSTRUCT();

			param.hwnd = IntPtr.Zero;
			param.wFunc = FileOperation.Delete;
			param.fFlags = (ushort)(FileOperationFlag.AllowUndo | FileOperationFlag.NoConfirmation);
			param.pFrom = filename + '\0';
			param.pTo = null;
			param.fAnyOperationsAborted = false;
			param.hNameMappings = IntPtr.Zero;
			param.lpszProgressTitle = null;

			return (SHFileOperation(ref param) == ERROR_SUCCESS);
		}
	}
}
