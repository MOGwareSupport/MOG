using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace MOG_Server
{
	[Serializable]
	public class MOG_ShutdownInfo
	{
		// Constructor
		public MOG_ShutdownInfo()
		{
			ShutdownTime = DateTime.Now;

			CommandsList = new ArrayList();
		}

		// Record the time when the server was shut down
		public DateTime ShutdownTime;

		// Record the various command/slave lists so they can be restored on boot up
		public ArrayList CommandsList;
	}
}
