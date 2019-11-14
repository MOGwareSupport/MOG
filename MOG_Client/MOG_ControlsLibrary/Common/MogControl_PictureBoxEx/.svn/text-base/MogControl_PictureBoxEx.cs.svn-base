using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MOG_ControlsLibrary.Common.MogControl_PictureBoxEx
{
	public class MogControl_PictureBoxEx : System.Windows.Forms.PictureBox
	{
		#region System defs
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion
		#region Member vars
		private int rightBorderCopyWidth = 2;
		#endregion
		#region Properties
		public int RightBorderCopyWidth
		{
			get { return this.rightBorderCopyWidth; }
			set 
			{
				this.rightBorderCopyWidth = value;

				// Value must be at least 2
				if (value <= 1)
				{
					this.rightBorderCopyWidth = 2;
				}
			}
		}
		#endregion
		#region Constructor
		public MogControl_PictureBoxEx()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();
		}
		#endregion
		#region OnPaintBackground()
		protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs pevent)
		{
			// First, draw background as normal
			base.OnPaintBackground(pevent);

			// If the PictureBox has grown larger than the Image it contains
			if (this.Width > this.Image.Width)
			{
				int sizeDifference = this.Width - this.Image.Width;
				Graphics gfx = pevent.Graphics;

				int srcWidth = this.rightBorderCopyWidth-1;
				
				// Has to be at least one pixel to work
				if (srcWidth <= 0)
				{
					srcWidth = 1;
				}

				gfx.DrawImage(this.Image, new Rectangle(this.Image.Width, 0, sizeDifference, this.Image.Height), this.Image.Width-this.rightBorderCopyWidth, 0, srcWidth, this.Image.Height, GraphicsUnit.Pixel);
			}
		}	
		#endregion
	}
}

