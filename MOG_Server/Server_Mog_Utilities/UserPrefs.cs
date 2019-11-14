using System;
using System.Windows.Forms;
using MOG;
using MOG.INI;
using Server_Gui;

namespace MOG_Server.Server_Mog_Utilities
{
	/// <summary>
	/// Summary description for UserPrefs.
	/// </summary>
	public class UserPrefs
	{
		static public MOG_Ini ini;

		public UserPrefs(String PrefsLocation)
		{
			ini = new MOG_Ini(PrefsLocation);
		}

		public bool Save(FormMainSMOG main)
		{
			ini.PutString("Gui", "ActiveTab", Convert.ToString(main.SMOG_Main_TabControl.SelectedIndex));
			ini.PutString("Gui", "Width", Convert.ToString(main.Width));
			ini.PutString("Gui", "Height", Convert.ToString(main.Height));
			ini.PutString("Gui", "top", Convert.ToString(main.Top));
			ini.PutString("Gui", "left", Convert.ToString(main.Left));

			//ini.PutString("Server", "ServerVersion", main.CurrentServerVersion);
			//ini.PutString("Server", "ClientVersion", main.CurrentClientVersion);

			return ini.Save();
		}

		static public int SavePref(string property, string key, string val)
		{
			int rc = ini.PutString(property, key, val);
			UserPrefs.ini.Save();
			return rc;
		}

		static public string LoadPref(string section, string key)
		{
			if (ini.KeyExist(section, key)) 
			{
				return ini.GetString(section, key);
			}

			return "";
		}

		public bool Load(FormMainSMOG main)
		{
			if (ini.KeyExist("Gui", "ActiveTab"))
			{
				int activeTab = Convert.ToInt32(ini.GetString("Gui", "ActiveTab"));
				if (activeTab <= main.SMOG_Main_TabControl.TabCount)
				{
					main.SMOG_Main_TabControl.SelectedIndex = activeTab;
				}
			}

			if (ini.KeyExist("Gui", "Width")) main.Width = Convert.ToInt32(ini.GetString("Gui", "Width"));
			if (ini.KeyExist("Gui", "Height")) main.Height = Convert.ToInt32(ini.GetString("Gui", "Height"));
			if (ini.KeyExist("Gui", "top")) main.Top = Convert.ToInt32(ini.GetString("Gui", "top"));
			if (ini.KeyExist("Gui", "left")) main.Left = Convert.ToInt32(ini.GetString("Gui", "left"));


			// Get our currently deployed versions
			//if (ini.KeyExist("Server", "ServerVersion")) main.CurrentServerVersion = ini.GetString("Server", "ServerVersion");
			//if (ini.KeyExist("Server", "ClientVersion")) main.CurrentClientVersion = ini.GetString("Server", "CLientVersion");

			// Per john's request.  
			{
                //Make sure it's not offscreen to the maximum
                if (main.Left > Screen.PrimaryScreen.Bounds.Size.Width - main.Width)
                    main.Left = Screen.PrimaryScreen.Bounds.Size.Width - main.Width;
                if (main.Top > Screen.PrimaryScreen.Bounds.Size.Height - main.Height)
                    main.Top = Screen.PrimaryScreen.Bounds.Size.Height - main.Height;
                
                // Set minimum requirements
				if (main.Width < 100) main.Width = 100;
				if (main.Height < 50) main.Height = 50;
				if (main.Top < 0) main.Top = 0;
				if (main.Left < 0) main.Left = 0;
			}

			return true;
		}
	}
}
