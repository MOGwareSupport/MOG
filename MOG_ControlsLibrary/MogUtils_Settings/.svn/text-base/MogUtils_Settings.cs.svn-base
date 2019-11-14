using System;
using System.Collections.Generic;
using System.Text;
using MOG.INI;
using System.Windows.Forms;
using System.Diagnostics;

namespace MOG_ControlsLibrary.MogUtils_Settings
{
	public class MogUtils_Settings
	{
		static public MOG_PropertiesIni Settings;

		static public void CreateSettings(string settingsLocation)
		{
			Settings = new MOG_PropertiesIni(settingsLocation);
		}

		#region Utilities
		static private string GetColumnOrderString(ListView listView)
		{
			string order = "";

			foreach (ColumnHeader header in listView.Columns)
			{
				if (order.Length == 0)
				{
					order = header.Text;
				}
				else
				{
					order = order + "," + header.Text;
				}
			}

			return order;
		}

		static private void PutColumnOrderString(ListView listView, string order)
		{
			string[] parts = order.Split(",".ToCharArray());
		}
		#endregion
		#region Loaders
		static private SortOrder ConvertStringToSortOrder(string order)
		{
			switch (order.ToLower())
			{
			case "ascending": return SortOrder.Ascending;
			case "descending": return SortOrder.Descending;
			default: return SortOrder.None;
			}
		}

		static private bool LoadDynamic_LayoutListView(ListView view, string section, string key)
		{
			try
			{
				if (Settings != null)
				{
					foreach (ColumnHeader column in view.Columns)
					{
						if (Settings.PropertyExist(section, key, column.Text + "_width")) column.Width = Convert.ToInt32(Settings.GetPropertyString(section, key, column.Text + "_width"));
					}
				}
			}
			catch
			{
			}

			return true;
		}

		static public string LoadSetting_default(string property, string key, string defaultSetting)
		{
			if (Settings != null)
			{
				if (property != null && property.Length > 0 &&
					key != null && key.Length > 0)
				{
					if (Settings.PropertyExist("MOGOptions", property, key))
					{
						return Settings.GetPropertyString("MOGOptions", property, key);
					}
				}
			}
			return defaultSetting;
		}

		static public string LoadSetting(string section, string key)
		{
			if (Settings != null)
			{
				if (section != null && section.Length > 0 &&
					key != null && key.Length > 0)
				{
					if (Settings.PropertyExist("MOGOptions", section, key))
					{
						return Settings.GetPropertyString("MOGOptions", section, key);
					}
				}
			}
			return "";
		}
		static public string LoadSetting(string section, string property, string key)
		{
			if (Settings != null)
			{
				if (section != null && section.Length > 0 &&
					property != null && property.Length > 0 &&
					key != null && key.Length > 0)
				{
					if (Settings.PropertyExist(section, property, key))
					{
						return Settings.GetPropertyString(section, property, key);
					}
				}
			}
			return "";
		}

		public static void LoadListView(string section, ListView listView)
		{
			if (SettingExist(section))
			{
				// Load the sorted flag			
				if (SettingExist(section, "Columns", "Sort")) PutColumnOrderString(listView, LoadSetting(section, "Columns", "Sort"));

				// Load all the columns
				LoadDynamic_LayoutListView(listView, section, "columns");
			}			
		}

		public static int LoadIntSetting(string section, string property, string key)
		{
			return Convert.ToInt32(LoadSetting(section, property, key));
		}

		public static int LoadIntSetting(string section, string property, string key, int defaultSetting)
		{
			if (LoadSetting(section, property, key).Length != 0)
				return Convert.ToInt32(LoadSetting(section, property, key));
			else return defaultSetting;
		}

		public static int LoadIntSetting(string property, string key, int defaultSetting)
		{
			if (LoadSetting("MOGOptions", property, key).Length != 0)
				return Convert.ToInt32(LoadSetting("MOGOptions", property, key));
			else return defaultSetting;
		}

		public static bool LoadBoolSetting(string property, string key, bool defaultSetting)
		{
			return LoadBoolSetting("MOGOptions", property, key, defaultSetting);
		}

		public static bool LoadBoolSetting(string section, string property, string key, bool defaultSetting)
		{
			if (LoadSetting(section, property, key).Length != 0)
			return Convert.ToBoolean(LoadSetting(section, property, key));
			else return defaultSetting;
		}

		public static SortOrder LoadSortOrderSetting(string section, string property, string key)
		{
			return ConvertStringToSortOrder(LoadSetting(section, property, key));
		}

		public static DateTime LoadDateTimeSetting(string section, string property, string key)
		{
			return Convert.ToDateTime(LoadSetting(section, property, key));
		}

		#endregion
		#region Savers
		static private bool SaveDynamic_LayoutListView(ListView view, string section, string key)
		{
			if (Settings != null)
			{
				foreach (ColumnHeader column in view.Columns)
				{
					Settings.PutPropertyString(section, key, column.Text + "_width", column.Width.ToString());
				}
			}

			return true;
		}

		static public int SaveSetting(string property, string key, string val)
		{
			int rc = 0;

			if (Settings != null)
			{
				if (property != null && property.Length > 0 &&
					key != null && key.Length > 0)
				{
					rc = Settings.PutPropertyString("MOGOptions", property, key, val);
					Settings.Save();
				}
			}

			return rc;
		}

		static public int SaveSetting(string section, string property, string key, string val)
		{
			int rc = 0;

			if (Settings != null)
			{
				if (section != null && section.Length > 0 &&
					property != null && property.Length > 0 &&
					key != null && key.Length > 0)
				{
					rc = Settings.PutPropertyString(section, property, key, val);
					Settings.Save();
				}
			}

			return rc;
		}

		public static int SaveIntSetting(string property, string key, int val)
		{
			return SaveSetting(property, key, val.ToString());
		}

		public static void SaveListView(string sectionName, ListView listView)
		{
			if (Settings != null)
			{
				// Save the sorted flag		
				Settings.PutPropertyString(sectionName, "Columns", "Sort", GetColumnOrderString(listView));

				// Save all the columns
				SaveDynamic_LayoutListView(listView, sectionName, "columns");
			}
		}
		#endregion

		public static bool SettingExist(string p, string p_2, string p_3)
		{
			bool exist = false;

			if (Settings != null)
			{
				exist = Settings.PropertyExist(p, p_2, p_3);

				if (!exist)
				{
					if (Settings.SectionExist(p) == false)
					{
						Debug.WriteLine("Section(" + p + ") does not exist while looking up property(" + p_2 + ")", p_3);
					}
					else
					{
						Debug.WriteLine("Property(" + p_2 + ") does not exist within section(" + p + ")", p_3);
					}
				}
			}

			return exist;
		}

		public static bool SettingExist(string p, string p_2)
		{
			bool exist = false;

			if (Settings != null)
			{
				exist = Settings.PropertyExist("MOGOptions", p, p_2);

				if (!exist)
				{
					if (Settings.SectionExist(p) == false)
					{
						Debug.WriteLine("Section(MOGOptions) does not exist", p_2);
					}
					else
					{
						Debug.WriteLine("Property(" + p + ") does not exist within section(" + "MOGOptions" + ")", p_2);
					}
				}
			}

			return exist;			
		}

		public static bool SettingExist(string p)
		{
			bool exist = false;

			if (Settings != null)
			{
				exist = Settings.SectionExist(p);

				if (!exist)
				{
					Debug.WriteLine("Section does not exist", "MOGOptions");
				}
			}

			return exist;
		}

		public static bool Save()
		{
			if (Settings != null)
			{
				return Settings.Save();
			}

			return false;
		}		
	}
}
