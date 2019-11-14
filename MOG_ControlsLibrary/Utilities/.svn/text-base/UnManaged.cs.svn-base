using System;
using System.Runtime.InteropServices;

namespace CP.Windows.Forms
{
	internal delegate int BrowseCallBackProc(IntPtr hwnd, int msg, IntPtr lp, IntPtr wp);

	[StructLayout(LayoutKind.Sequential)]
	internal struct BrowseInfo
	{
		public IntPtr hwndOwner;
		public IntPtr pidlRoot;
		[MarshalAs(UnmanagedType.LPTStr)]
		public string displayname;
		[MarshalAs(UnmanagedType.LPTStr)]
		public string title;
		public int flags;
		[MarshalAs(UnmanagedType.FunctionPtr)]
		public BrowseCallBackProc callback;
		public IntPtr lparam;
	}
	
	[ComImport]
	[Guid("00000002-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IMalloc
	{
		[PreserveSig]
		IntPtr Alloc(IntPtr cb);

		[PreserveSig]
		IntPtr Realloc(IntPtr pv, IntPtr cb);

		[PreserveSig]
		void Free(IntPtr pv);

		[PreserveSig]
		IntPtr GetSize(IntPtr pv);

		[PreserveSig]
		int DidAlloc(IntPtr pv);

		[PreserveSig]
		void HeapMinimize();
	}

	/// <summary>
	/// A class that defines all the unmanaged methods used in the assembly
	/// </summary>
	internal class UnManagedMethods
	{
		[DllImport("Shell32.dll", CharSet=CharSet.Auto)]
		internal extern static System.IntPtr SHBrowseForFolder(ref BrowseInfo bi);
		
		[DllImport("Shell32.dll", CharSet=CharSet.Auto)]
		[return : MarshalAs(UnmanagedType.Bool)]
		internal extern static bool SHGetPathFromIDList(IntPtr pidl, [MarshalAs(UnmanagedType.LPTStr)] System.Text.StringBuilder pszPath);
		
		[DllImport("User32.Dll")]
		[return : MarshalAs(UnmanagedType.Bool)]
		internal extern static bool SendMessage(IntPtr hwnd, int msg, IntPtr wp, IntPtr lp);
	
		[DllImport("Shell32.dll")]
		internal extern static int SHGetMalloc([MarshalAs(UnmanagedType.IUnknown)]out object shmalloc);
	
		//Helper routine to free memory allocated using shells malloc object
		internal static void SHMemFree(IntPtr ptr)
		{
			object shmalloc = null;

			if (SHGetMalloc(out shmalloc) == 0)
			{
				IMalloc malloc = (IMalloc)shmalloc;

				(malloc).Free(ptr);
			}
		}
	}
}
