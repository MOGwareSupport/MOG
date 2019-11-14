using System;
using System.Drawing;
using System.Windows.Forms;

namespace MOG_ControlsLibrary
{
	/// <summary>
	/// Summary description for IconMenuItem.
	/// </summary>
	public class IconMenuItem : MenuItem
	{
		#region Member vars
		private Image img;
		private int imageTextBorderPadding = 5;
		private Font font;
		private Color textColor;
		private int imgWidth = 16;
		private int imgHeight = 16;
		private int heightPadding = 3;
		#endregion
		#region Properties
		public Image Image
		{
			get { return this.img; }
		}
		#endregion
		#region Constructors
		public IconMenuItem(string text, Image img, Font font, Color textColor, int imageTextBorderPadding) :base(text)
		{
			this.OwnerDraw = true;
			this.MeasureItem += new MeasureItemEventHandler(IconMenuItem_MeasureItem);
			this.DrawItem += new DrawItemEventHandler(IconMenuItem_DrawItem);

			this.font = font;
			//this.imgHeight = font.Height;
			//this.imgWidth = font.Height;
			this.textColor = textColor;
			this.imageTextBorderPadding = imageTextBorderPadding;
			this.img = img;

			SetToDefaultsOnNull();
		}

		public IconMenuItem(string text, Image img, Font font, Color textColor) :base(text)
		{
			this.OwnerDraw = true;
			this.MeasureItem += new MeasureItemEventHandler(IconMenuItem_MeasureItem);
			this.DrawItem += new DrawItemEventHandler(IconMenuItem_DrawItem);

			this.font = font;
			//this.imgHeight = font.Height;
			//this.imgWidth = font.Height;
			this.textColor = textColor;
			this.imageTextBorderPadding = 5;
			this.img = img;

			SetToDefaultsOnNull();
		}

		public IconMenuItem(string text, Image img, Font font) :base(text)
		{
			this.OwnerDraw = true;
			this.MeasureItem += new MeasureItemEventHandler(IconMenuItem_MeasureItem);
			this.DrawItem += new DrawItemEventHandler(IconMenuItem_DrawItem);

			this.font = font;
			//this.imgHeight = font.Height;
			//this.imgWidth = font.Height;
			this.textColor = SystemColors.MenuText;
			this.imageTextBorderPadding = 5;
			this.img = img;

			SetToDefaultsOnNull();
		}

		public IconMenuItem(string text, Image img) :base(text)
		{
			this.OwnerDraw = true;
			this.MeasureItem += new MeasureItemEventHandler(IconMenuItem_MeasureItem);
			this.DrawItem += new DrawItemEventHandler(IconMenuItem_DrawItem);

			this.font = SystemInformation.MenuFont;
			//this.imgHeight = font.Height;
			//this.imgWidth = font.Height;
			this.textColor = SystemColors.MenuText;
			this.imageTextBorderPadding = 5;
			this.img = img;

			SetToDefaultsOnNull();
		}

		public IconMenuItem(string text, Image img, System.EventHandler onClick) :base(text)
		{
			this.OwnerDraw = true;
			this.MeasureItem += new MeasureItemEventHandler(IconMenuItem_MeasureItem);
			this.DrawItem += new DrawItemEventHandler(IconMenuItem_DrawItem);
			this.Click += onClick;

			this.font = SystemInformation.MenuFont;
			//this.imgHeight = font.Height;
			//this.imgWidth = font.Height;
			this.textColor = SystemColors.MenuText;
			this.imageTextBorderPadding = 5;
			this.img = img;

			SetToDefaultsOnNull();
		}

		public IconMenuItem(string text) :base(text)
		{
			this.OwnerDraw = true;
			this.MeasureItem += new MeasureItemEventHandler(IconMenuItem_MeasureItem);
			this.DrawItem += new DrawItemEventHandler(IconMenuItem_DrawItem);

			this.font = SystemInformation.MenuFont;
			//this.imgHeight = font.Height;
			//this.imgWidth = font.Height;
			this.textColor = SystemColors.MenuText;
			this.imageTextBorderPadding = 5;
			this.img = null;
		}
		#endregion
		#region Private functions
		private void SetToDefaultsOnNull()
		{
			if (this.font == null)
				this.font = SystemInformation.MenuFont;

			if (this.imageTextBorderPadding < 0)
				this.imageTextBorderPadding = 5;
		}
		#endregion
		#region Overriden event handlers
		private void IconMenuItem_MeasureItem(object sender, System.Windows.Forms.MeasureItemEventArgs e)
		{
			int imageWidth = this.imgWidth + this.imageTextBorderPadding;
			int imageHeight = 0;
			if (this.img != null)
			{
				imageWidth = this.imgWidth + this.imageTextBorderPadding;
				imageHeight = this.imgHeight;
			}

			// calculate height
			if (imageHeight > this.font.Height)
				e.ItemHeight = imageHeight;
			else
				e.ItemHeight = this.font.Height + this.heightPadding;

			// calculate width
			e.ItemWidth = imageWidth + (int)e.Graphics.MeasureString(this.Text, this.font).Width;
		}

		private void IconMenuItem_DrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
		{
			//e.Graphics.DrawIcon(this.icon, new Rectangle(0,0, icon.Width, icon.Height));

			int imageWidth  = this.imgWidth + this.imageTextBorderPadding;
			int imageHeight = 0;
			if (this.img != null)
			{
				imageWidth = this.imgWidth + this.imageTextBorderPadding;
				imageHeight = this.imgHeight;
			}

			if (this.Enabled)
				e.DrawBackground();
			
			// draw image
			if (this.img != null)
				e.Graphics.DrawImage(img, new Rectangle(0, this.Index*(this.font.Height+this.heightPadding), this.imgWidth, this.imgHeight));//e.Graphics.DrawImage(img, new Rectangle(0, this.Index*(this.font.Height+this.heightPadding), img.Width, img.Height));


			// draw text
			PointF textPoint = new PointF( imageWidth, this.Index*(this.font.Height+this.heightPadding) );
			// setup text color, depending on state
			Color drawColor = this.textColor;
			if (!this.Enabled)
				drawColor = SystemColors.GrayText;
			else if ( (e.State & DrawItemState.Selected) > 0 )
				drawColor = SystemColors.HighlightText;
			
			e.Graphics.DrawString(this.Text, this.font, new SolidBrush(drawColor), textPoint);

		}
		#endregion
	}
}



