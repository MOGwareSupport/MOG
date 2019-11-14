using System;
using System.IO;
using System.Collections;
using System.Windows.Forms;
using System.Diagnostics;

using MOG;
using MOG.PROJECT;
using MOG.PROPERTIES;
using MOG.PLATFORM;
using MOG.USER;
using MOG.REPORT;
using MOG.TIME;
using MOG.INI;
using MOG.DOSUTILS;
using MOG.FILENAME;
using MOG_Server.Server_Mog_Utilities;
using MOG_Server.Server_Utilities;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;



namespace MOG_Server.Server_Gui.guiConfigurationsHelpers
{
	/// <summary>
	/// Summary description for guiProjectOptions.
	/// </summary>
	public class guiProjectOptions
	{
		private Configuration mParent;

		//private bool DailyBuildStarted;

		public guiProjectOptions(Configuration Parent)
		{
			mParent = Parent;
			//DailyBuildStarted = false;
		}
	}
}
