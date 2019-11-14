using System;
using System.Collections.Generic;
using System.Text;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.INI;
using System.Windows.Forms;

namespace MOG_Client.Client_Gui
{
	public class guiWebTabManager
	{
		MogMainForm mainForm;

		struct webtab
		{
			public string title;
			public string url;
			public int position;
		};

		public guiWebTabManager(MogMainForm main)
		{
			mainForm = main;		
		}

		public void LoadTabs()
		{
			// Clear out any previous web tabs
			ClearWebTabs();

			MOG_PropertiesIni project = MOG_ControllerProject.GetProject().GetConfigFile();
			if (project.SectionExist("WebTabs"))
			{
				SortedList<int, webtab> list = new SortedList<int, webtab>();
				for (int i = 0; i < project.CountKeys("WebTabs"); i++)
				{
					string section = "WebTab_" + project.GetKeyNameByIndexSLOW("WebTabs", i);

					// Upgrade old tabs to new format
					if (project.SectionExist(section) == false &&
						project.SectionExist(project.GetKeyNameByIndexSLOW("WebTabs", i)) == true &&
						string.Compare(project.GetKeyNameByIndexSLOW("WebTabs", i), "Project", true) != 0)
					{
						project.RenameSection(project.GetKeyNameByIndexSLOW("WebTabs", i), section);
					}

					// Load Tab information
					if (project.SectionExist(section))
					{
						webtab tab = new webtab();
						tab.title = "WebTab";
						tab.url = "http://www.mogware.com";
						tab.position = 0;

						// Get custom title
						if (project.KeyExist(section, "TITLE"))
						{
							tab.title = project.GetString(section, "TITLE");
						}

						// Get custom url
						if (project.KeyExist(section, "URL"))
						{
							tab.url = project.GetString(section, "URL");
						}

						// Get position
						if (project.KeyExist(section, "POSITION"))
						{
							tab.position = Convert.ToInt32(project.GetString(section, "POSITION"));
						}

						list.Add(tab.position, tab);
					}
				}

				foreach (webtab newtab in list.Values)
				{
					// Create the tab
					TabPage tab = CreateWebTab(newtab.title, newtab.url);

					// Add it to our main tab control
					mainForm.MOGTabControl.TabPages.Add(tab);
				}
			}
		}

		private TabPage CreateWebTab(string title, string URL)
		{
			// Create our web tab
			TabPage tab = new TabPage(title);
			tab.Tag = "WEBTAB";

			// Create the web browser
			WebBrowser webPage = new WebBrowser();
			webPage.Dock = DockStyle.Fill;

			// Login to the selected url
			webPage.Navigate(URL);

			// Add the web control
			tab.Controls.Add(webPage);
			return tab;
		}

		private void ClearWebTabs()
		{
			// Walk thru all the tabs
			foreach (TabPage tab in mainForm.MOGTabControl.TabPages)
			{
				// Check if this is a web tab
				if (tab.Tag as string == "WEBTAB")
				{
					// Remove it
					mainForm.MOGTabControl.TabPages.Remove(tab);
				}
			}
		}
	}
}
