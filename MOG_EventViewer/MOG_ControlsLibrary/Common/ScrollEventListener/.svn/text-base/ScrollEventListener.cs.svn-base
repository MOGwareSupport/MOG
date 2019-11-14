using System;
using System.Windows.Forms;

namespace MOG_EventViewer.MOG_ControlsLibrary
{
	/// <summary>
	/// Summary description for ScrollEventListener.
	/// </summary>
	public class ScrollEventListener : NativeWindow
	{
		public delegate void ScrollEvent(object sender, EventArgs e);

		public event ScrollEvent VScroll;
		public event ScrollEvent HScroll;
		public event ScrollEvent MWheelScroll;

		// from winuser.h
		private const int WM_HSCROLL = 0x114;
		private const int WM_VSCROLL = 0x115;
		private const int WM_MOUSEWHEEL = 0x020A;
	
		public Control ctrl;

		public ScrollEventListener(Control ctrl)
		{
			this.ctrl = ctrl;
			AssignHandle(ctrl.Handle);
		}

		protected override void WndProc(ref Message m)
		{
			// Listen for operating system messages
			if (m.Msg == WM_VSCROLL)
			{
				if (this.VScroll != null)
					this.VScroll(ctrl, new EventArgs());
			}
			else if (m.Msg == WM_HSCROLL)
			{
				if (this.HScroll != null)
					this.HScroll(ctrl, new EventArgs());
			}
			else if (m.Msg == WM_MOUSEWHEEL)
			{
				if (this.MWheelScroll != null)
					this.MWheelScroll(ctrl, new EventArgs());
			}

			base.WndProc (ref m);
		}
	}
}

