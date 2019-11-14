using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CP.Windows.Forms
{
	/// <summary>
	/// Flags that control display and behaviour of folder browse dialog
	/// </summary>
	[Flags]
	public enum BrowseFlags : int
	{
		/// <summary>
		/// Same as BIF_RETURNONLYFSDIRS 
		/// </summary>
		ReturnOnlyFSDirs   = 0x0001,
		/// <summary>
		/// Same as BIF_DONTGOBELOWDOMAIN 
		/// </summary>
		DontGoBelowDomain  = 0x0002,
		/// <summary>
		/// Same as BIF_STATUSTEXT 
		/// </summary>
		ShowStatusText     = 0x0004,
		/// <summary>
		/// Same as BIF_RETURNFSANCESTORS 
		/// </summary>
		ReturnFSancestors  = 0x0008,
		/// <summary>
		/// Same as BIF_EDITBOX 
		/// </summary>
		EditBox            = 0x0010,
		/// <summary>
		/// Same as BIF_VALIDATE 
		/// </summary>
		Validate           = 0x0020,
		/// <summary>
		/// Same as BIF_NEWDIALOGSTYLE
		/// </summary>
		NewDialogStyle     = 0x0040,
		/// <summary>
		/// Same as BIF_BROWSEINCLUDEURLS 
		/// </summary>
		BrowseIncludeURLs  = 0x0080,
		/// <summary>
		/// Same as BIF_UAHINT
		/// </summary>
		AddUsageHint       = 0x0100,
		/// <summary>
		/// Same as BIF_NONEWFOLDERBUTTON 
		/// </summary>
		NoNewFolderButton  = 0x0200,
		/// <summary>
		/// Same as BIF_BROWSEFORCOMPUTER
		/// </summary>
		BrowseForComputer  = 0x1000,
		/// <summary>
		/// Same as BIF_BROWSEFORPRINTER 
		/// </summary>
		BrowseForPrinter   = 0x2000,
		/// <summary>
		/// Same as BIF_BROWSEINCLUDEFILES 
		/// </summary>
		IncludeFiles	   = 0x4000,
		/// <summary>
		/// Same as BIF_SHAREABLE 
		/// </summary>
		ShowShareable	   = 0x8000,
	}
	
	#region Delegate and Event Arg Decalarations
	
	/// <summary>
	/// Provides data for folder selection changed event
	/// </summary>
	public class FolderSelChangedEventArgs : EventArgs, IDisposable
	{
		private IntPtr pidlNewSelect;

		internal FolderSelChangedEventArgs(IntPtr pidlNewSelect)
		{
			this.pidlNewSelect = pidlNewSelect;
		}
		
		/// <summary>
		/// Return ITEMIDLIST for the currently selected folder
		/// </summary>
		public IntPtr CurSelFolderPidl
		{
			get
			{
				return pidlNewSelect;
			}
		}

		/// <summary>
		/// Gets the path of the folder which is currently selected
		/// </summary>
		public string CurSelFolderPath
		{
			get
			{
				StringBuilder path = new StringBuilder(260);
				UnManagedMethods.SHGetPathFromIDList(pidlNewSelect, path);

				return path.ToString();
			}
		}

		public void Dispose()
		{
			UnManagedMethods.SHMemFree(pidlNewSelect);
		}
	};

	public delegate void FolderSelChangedEventHandler(object sender, FolderSelChangedEventArgs e);
	
	/// <summary>
	/// Provides data for the IUnknownObtainedEvent.
	/// </summary>
	public class IUnknownObtainedEventArgs : EventArgs
	{
		private object siteUnknown;

		internal IUnknownObtainedEventArgs(object siteUnknown)
		{
			this.siteUnknown = siteUnknown;
		}
		
		/// <summary>
		/// Object that corrensponds to the IUnknown obtained
		/// </summary>
		public object SiteUnknown
		{
			get
			{
				return siteUnknown;
			}
		}
	}
	
	public delegate void IUnknownObtainedEventHandler(object sender, IUnknownObtainedEventArgs args);
	
	/// <summary>
	/// Provides data for validation failed event.
	/// </summary>
	public class ValidateFailedEventArgs
	{
		private string invalidText;
		private bool dismissDialog = false;

		internal ValidateFailedEventArgs(string invalidText)
		{
			this.invalidText = invalidText;
		}
		
		/// <summary>
		/// The text which called validation to fail
		/// </summary>
		public string InvalidText
		{
			get
			{
				return invalidText;
			}
		}
		
		/// <summary>
		/// Sets whether the dialog needs to be dismissed or not
		/// </summary>
		public bool DismissDialog
		{
			get
			{
				return dismissDialog;
			}
			set
			{
				dismissDialog = value;
			}
		}
	}
	
	public delegate void ValidateFailedEventHandler(object sender, ValidateFailedEventArgs args);
	
	#endregion

	/// <summary>
	/// Encapsulates the shell folder browse dialog shown by SHBrowseForFolder
	/// </summary>
	public class ShellFolderBrowser : System.ComponentModel.Component
	{
		private string title;
		private IntPtr pidlReturned = IntPtr.Zero;
		private IntPtr handle;
		private string displayName;
		private BrowseFlags flags;
		
		/// <summary>
		/// 
		/// </summary>
		public ShellFolderBrowser()
		{
		}
		
		#region Component properties

		/// <summary>
		/// String that is displayed above the tree view control in the dialog box. 
		/// This string can be used to specify instructions to the user. 
		/// Can only be modified if the dalog is not currently displayed.
		/// </summary>
		[Description("String that is displayed above the tree view control in the dialog box. This string can be used to specify instructions to the user.")] 
		public string Title
		{
			get
			{
				return title;
			}
			set
			{
				if (handle != IntPtr.Zero)
					throw new InvalidOperationException();
				
				title = value;
			}
		}
		
		/// <summary>
		/// The display name of the folder selected by the user
		/// </summary>
		[Description("The display name of the folder selected by the user")]
		public string FolderDisplayName
		{
			get
			{
				return displayName;
			}
		}
		
		/// <summary>
		/// The folder path that was selected
		/// </summary>
		public string FolderPath
		{
			get
			{
				if (pidlReturned == IntPtr.Zero)
					return string.Empty;
				
				StringBuilder pathReturned = new StringBuilder(260);

				UnManagedMethods.SHGetPathFromIDList(pidlReturned, pathReturned);
				return pathReturned.ToString();
			}
		}

		/// <summary>
		/// Sets the flags that control the behaviour of the dialog
		/// </summary>
		public BrowseFlags BrowseFlags
		{
			get
			{
				return flags;
			}
			set
			{
				flags = value;
			}
		}
		
		#endregion

		#region ShowDialog and related methods
		
		private bool ShowDialogInternal(ref BrowseInfo bi)
		{
			bi.title = title;
			bi.displayname = new string('\0', 260);
			bi.callback = new BrowseCallBackProc(this.CallBack);
			bi.flags = (int)flags;
			
			//Free any old pidls
			if (pidlReturned != IntPtr.Zero)
				UnManagedMethods.SHMemFree(pidlReturned);

			bool ret = (pidlReturned = UnManagedMethods.SHBrowseForFolder(ref bi)) != IntPtr.Zero;

			if (ret)
			{
				displayName = bi.displayname;
			}
			
			//Reset the handle
			handle = IntPtr.Zero;

			return ret;
		}
	
		/// <summary>
		/// Shows the dialog
		/// </summary>
		/// <param name="owner">The window to use as the owner</param>
		/// <returns></returns>
		public bool ShowDialog(System.Windows.Forms.IWin32Window owner)
		{
			if (handle != IntPtr.Zero)
				throw new InvalidOperationException();

			BrowseInfo bi = new BrowseInfo();
			
			if (owner != null)
				bi.hwndOwner = owner.Handle;

			return ShowDialogInternal(ref bi);
		}

		/// <summary>
		/// Shows the dialog using active window as the owner
		/// </summary>
		public bool ShowDialog()
		{
			return ShowDialog(Form.ActiveForm);
		}
		#endregion

		#region Functions that send messages to the dialog
		private const int WM_USER				  = 0x0400;

		private const int BFFM_SETSTATUSTEXTA     = (WM_USER + 100);
		private const int BFFM_SETSTATUSTEXTW     = (WM_USER + 104);
		
		/// <summary>
		/// Sets the text of the staus area of the folder dialog
		/// </summary>
		/// <param name="text">Text to set</param>
		public void SetStatusText(string text)
		{
			if (handle == IntPtr.Zero)
				throw new InvalidOperationException();
			
			int msg = (Environment.OSVersion.Platform == PlatformID.Win32NT) ? BFFM_SETSTATUSTEXTW : BFFM_SETSTATUSTEXTA;
			IntPtr strptr = Marshal.StringToHGlobalAuto(text);

			UnManagedMethods.SendMessage(handle, msg, IntPtr.Zero, strptr);
			
			Marshal.FreeHGlobal(strptr);
		}	

		private const int BFFM_ENABLEOK           = (WM_USER + 101);
		
		/// <summary>
		/// Enables or disables the ok button
		/// </summary>
		/// <param name="bEnable">true to enable false to diasble the OK button</param>
		public void EnableOkButton(bool bEnable)
		{
			if (handle == IntPtr.Zero)
				throw new InvalidOperationException();

			IntPtr lp = bEnable ? new IntPtr(1) : IntPtr.Zero;
			
			UnManagedMethods.SendMessage(handle, BFFM_ENABLEOK, IntPtr.Zero, lp);
		}

		private const int BFFM_SETSELECTIONA      = (WM_USER + 102);
		private const int BFFM_SETSELECTIONW      = (WM_USER + 103);
		
		/// <summary>
		/// Sets the selection the text specified
		/// </summary>
		/// <param name="newsel">The path of the folder which is to be selected</param>
		public void SetSelection(string newsel)
		{
			if (handle == IntPtr.Zero)
				throw new InvalidOperationException();
			
			int msg = (Environment.OSVersion.Platform == PlatformID.Win32NT) ? BFFM_SETSELECTIONA : BFFM_SETSELECTIONW;
			
			IntPtr strptr = Marshal.StringToHGlobalAuto(newsel);

			UnManagedMethods.SendMessage(handle, msg, new IntPtr(1), strptr);
			
			Marshal.FreeHGlobal(strptr);
		}
		
		private const int BFFM_SETOKTEXT          = (WM_USER + 105);
		
		/// <summary>
		/// Sets the text of the OK button in the dialog
		/// </summary>
		/// <param name="text">New text of the OK button</param>
		public void SetOkButtonText(string text)
		{
			if (handle == IntPtr.Zero)
				throw new InvalidOperationException();
			
			IntPtr strptr = Marshal.StringToHGlobalUni(text);

			UnManagedMethods.SendMessage(handle, BFFM_SETOKTEXT, new IntPtr(1), strptr);
			
			Marshal.FreeHGlobal(strptr);
		}
		
		private const int BFFM_SETEXPANDED        = (WM_USER + 106);
		
		/// <summary>
		/// Expand a path in the folder
		/// </summary>
		/// <param name="path">The path to expand</param>
		public void SetExpanded(string path)
		{
			IntPtr strptr = Marshal.StringToHGlobalUni(path);

			UnManagedMethods.SendMessage(handle, BFFM_SETEXPANDED, new IntPtr(1), strptr);
			
			Marshal.FreeHGlobal(strptr);
		}
		#endregion
		
		#region Callback Handling and Event Propogation
		
		/// <summary>
		/// Fired when the dialog is initialized
		/// </summary>
		public event EventHandler Initialized;
		/// <summary>
		/// Fired when selection changes
		/// </summary>
		public event FolderSelChangedEventHandler SelChanged;
		/// <summary>
		/// Shell provides an IUnknown through this event. For details see documentation of SHBrowseForFolder
		/// </summary>
		public event IUnknownObtainedEventHandler IUnknownObtained;
		/// <summary>
		/// Fired when validation of text typed by user fails
		/// </summary>
		public event ValidateFailedEventHandler ValidateFailed;
		
		private const int BFFM_INITIALIZED        = 1;
		private const int BFFM_SELCHANGED         = 2;
		private const int BFFM_VALIDATEFAILEDA    = 3;
		private const int BFFM_VALIDATEFAILEDW    = 4;
		private const int BFFM_IUNKNOWN           = 5;

		private int CallBack(IntPtr hwnd, int msg, IntPtr lp, IntPtr lpData)
		{
			int ret = 0;

			switch(msg)
			{
				case BFFM_INITIALIZED:
					handle = hwnd;
					if (Initialized != null)
					{
						Initialized(this, null);
					}
					break;
				case BFFM_IUNKNOWN:
					if (IUnknownObtained != null)
					{
						IUnknownObtained(this, new IUnknownObtainedEventArgs(Marshal.GetObjectForIUnknown(lp)));
					}
					break;
				case BFFM_SELCHANGED:
					if (SelChanged != null)
					{
						FolderSelChangedEventArgs e = new FolderSelChangedEventArgs(lp);
						SelChanged(this, e);
					}
					break;
				case BFFM_VALIDATEFAILEDA:
					if (ValidateFailed != null)
					{
						ValidateFailedEventArgs e = new ValidateFailedEventArgs(Marshal.PtrToStringAnsi(lpData));
						ValidateFailed(this, e);
						
						ret = (e.DismissDialog) ? 0 : 1;
					}
					break;
				case BFFM_VALIDATEFAILEDW:
					if (ValidateFailed != null)
					{
						ValidateFailedEventArgs e = new ValidateFailedEventArgs(Marshal.PtrToStringUni(lpData));
						ValidateFailed(this, e);
						
						ret = (e.DismissDialog) ? 0 : 1;
					}
					break;
			}

			return ret;
		}

		#endregion

		protected override void Dispose(bool disposing)
		{
			if (pidlReturned != IntPtr.Zero)
			{
				UnManagedMethods.SHMemFree(pidlReturned);
				pidlReturned = IntPtr.Zero;
			}
		}
	}
}
