using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace MOG_ControlsLibrary.Utilities
{
	public class MiscUtilities
	{
		static public void EnsureFormLocatedOnValidMonitors(Form form)
		{
			Rectangle workArea = Screen.GetWorkingArea(form);

			if (form.Top < 0) form.Top = 0;
			if (form.Left < 0) form.Left = 0;
			if (form.Top + form.Height > workArea.Bottom) form.Top = workArea.Bottom - form.Height;
			if (form.Left + form.Width > workArea.Right) form.Left = workArea.Right - form.Width;
		}
	}
}
