using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Diagnostics;

using MOG.INI;

namespace MOG_ControlsLibrary.Utils
{
	/// <summary>
	/// Summary description for guiUserToDoManager.
	/// </summary>
	public class MogUtil_ControlHide
	{
		private int mOpenWidth;
		private int mCloseWidth;
		private bool mOpen;

		public delegate void ControlHideOpeningEvent();
		public event ControlHideOpeningEvent mOpening;
		
		public MogUtil_ControlHide(int openWidth, int closeWidth, bool open)
		{
			mOpenWidth = openWidth;
			mCloseWidth = closeWidth;
			Opened = open;
		}

		public int Width
		{
			get 
			{
				if (Opened)
				{
					return mOpenWidth;
				}
				else
				{
					return mCloseWidth;
				}
			} 
		}

		public bool Opened
		{
			get 
			{ 
				return mOpen;
			} 
			set
			{
				mOpen = value;

				if (mOpen)
				{
					// Inform any callbacks
					if (mOpening != null)
					{
						mOpening();
					}
				}
			}
		}

		public int ToggleWidth()
		{
			// Are we open
			if (Opened)
			{
				// Close it
				Opened = false;
				//Debug.Write("Close:" + mCloseWidth.ToString() + "\n", "ToggleWidth");
				return mCloseWidth;
			}
			else
			{
				Opened = true;
				//Debug.Write("Open:" + mOpenWidth.ToString() + "\n", "ToggleWidth");
				return mOpenWidth;
			}		
		}

		public void ChangeWidth(int width)
		{
			// Are we open
			if (Opened)
			{
				// Safeguard making too small and reversing us
				if (width <= mCloseWidth || width < 5)
				{
					Opened = false;
					// If we don't have a valid mOpenWidth, set it to something meaningful
					if(mOpenWidth < 5) 
					{
						mOpenWidth = 160;
					}
				}
				else
				{
					mOpenWidth = width;
				}
			}
			else
			{
				// Safeguard making too small and reversing us
				if (width >= mOpenWidth || width > 50)
				{
					Opened = true;
					mCloseWidth = 0;
				}
				else
				{
					mCloseWidth = width;
				}
			}
		}

		public void Save(MOG_PropertiesIni ini, string section, string property, string key)
		{
			ini.PutPropertyString(section, property, key + "_OpenWidth", mOpenWidth.ToString());
			ini.PutPropertyString(section, property, key + "_CLoseWidth", mCloseWidth.ToString());
			ini.PutPropertyString(section, property, key + "_Open", Opened.ToString());
		}

		public void Load(MOG_PropertiesIni ini, string section, string property, string key)
		{
			if (ini.PropertyExist(section, property, key + "_OpenWidth"))
			{
				mOpenWidth = Convert.ToInt32(ini.GetPropertyString(section, property, key + "_OpenWidth"));
			}

			if (ini.PropertyExist(section, property, key + "_CLoseWidth"))
			{
				mCloseWidth = Convert.ToInt32(ini.GetPropertyString(section, property, key + "_CLoseWidth"));
			}

			if (ini.PropertyExist(section, property, key + "_Open"))
			{
				Opened = Convert.ToBoolean(ini.GetPropertyString(section, property, key + "_Open"));
			}
		}
	}
}
