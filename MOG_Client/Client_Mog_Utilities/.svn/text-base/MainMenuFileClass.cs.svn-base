using System;
using System.IO;
using System.Windows.Forms;

using MOG_Client;
using MOG_Client.Forms;

using MOG;
using MOG.FILENAME;
using MOG.INI;
using MOG.CONTROLLER.CONTROLLERLIBRARY;
using MOG.CONTROLLER.CONTROLLERSYSTEM;

using MOG_ControlsLibrary.Common;
using MOG_ControlsLibrary;
using MOG_ControlsLibrary.Utils;
using MOG_Client.Client_Utilities;
using MOG_Client.Client_Gui;

namespace MOG_Client.Client_Mog_Utilities
{
	/// <summary>
	/// Summary description for MainMenuFileClass.
	/// </summary>
	public class MainMenuFileClass
	{
		public MainMenuFileClass()
		{
		}

		static public bool MOGGlobalSQLLogin()
		{
			// Attempt to get our SQL connection string
			string mConnectionString = "";

			MOG_Ini config = new MOG_Ini(MOG_Main.GetExecutablePath() + "\\MOG.ini");

			SQLConnectForm sql = new SQLConnectForm();
			if (sql.ShowDialog() == DialogResult.OK)
			{
				mConnectionString = sql.mConnectString;
				config.PutString("SQL", "ConnectionString", mConnectionString);
				config.Save();
				config = null;

				return MOG_ControllerSystem.InitializeDatabase(mConnectionString, "", "");
			}
			
			return false;
		}

		static public void MOGGlobalOffline(MogMainForm main, MenuItem item)
		{
			bool offline;
			if (item.Checked)
			{
				offline = false;
				item.Checked = false;
			}
			else
			{
				offline = true;
				item.Checked = true;
			}
			
			guiStartup.SetOffline(main, offline);
		}

		static public void MOGGlobalQuit(MogMainForm main)
		{
			// Save user prefs
			main.mUserPrefs.SaveStaticForm_LayoutPrefs();
			guiUserPrefs.SaveDynamic_LayoutPrefs("AssetManager", main);
			guiUserPrefs.SaveStatic_ProjectPrefs();
			main.mUserPrefs.Save();

			main.Shutdown();
		}

		static public bool MOGGlobalRepositoryLogin(MogMainForm main, bool forceRestart)
		{
			SelectValidRepository:

			MogForm_RepositoryBrowser_Client browser = new MogForm_RepositoryBrowser_Client(forceRestart);
			DialogResult rc = DialogResult.Cancel;

			if(!MOG_Main.IsShutdown())
			{
				rc = browser.ShowDialog();
			}

			if (rc == DialogResult.OK)
			{
				if (forceRestart)
				{
					main.Close();
				}
				return true;
			}
			else if ( rc == DialogResult.Retry)
			{
				goto SelectValidRepository;
			}

			return false;
		}
	}
}
