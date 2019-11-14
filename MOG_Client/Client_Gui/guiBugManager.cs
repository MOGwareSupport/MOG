using System;
using MOG.CONTROLLER.CONTROLLERPROJECT;


namespace MOG_Client.Client_Gui
{
	/// <summary>
	/// Summary description for BugManager.
	/// </summary>
	public class guiBugManager
	{
		MogMainForm mainForm;
		public string mBugDatabaseURL;

		public guiBugManager(MogMainForm main)
		{
			mainForm = main;

			InitializeStartPage();
		}

		private void InitializeStartPage()
		{
			System.Object nullObject = 0;
			string str = "";
			System.Object nullObjStr = str;
			if (MOG_ControllerProject.GetProject().GetConfigFile().KeyExist("Project", "BugDatabase"))
			{
				mBugDatabaseURL = MOG_ControllerProject.GetProject().GetConfigFile().GetString("Project", "BugDatabase");
			}
			else
			{
				mBugDatabaseURL = "http://192.168.0.120";
			}
//			mainForm.BugManagerAxWebBrowser.Navigate(mBugDatabaseURL, ref nullObject, ref nullObjStr, ref nullObjStr, ref nullObjStr);
		}
	}
}
