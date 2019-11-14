using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace MOG_CoreControls
{
	class TrimmedLabel : Label
	{
		protected override void OnPaint(PaintEventArgs e)
		{
			StringFormat format = new StringFormat();
			format.Trimming = StringTrimming.EllipsisPath;
			e.Graphics.DrawString(Text, Font, Brushes.Black, e.ClipRectangle, format);
		}
	}
}
