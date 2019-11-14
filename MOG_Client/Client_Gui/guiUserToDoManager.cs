using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;

namespace MOG_Client.Client_Gui
{
	/// <summary>
	/// Summary description for guiUserToDoManager.
	/// </summary>
	public class guiUserToDoManager
	{
		private int mTodoWidth;

		public guiUserToDoManager()
		{
			mTodoWidth = 168;
		}

		public int Width
		{
			get {	return this.mTodoWidth; }
			set {	this.mTodoWidth = value; }
		}
	}
}
