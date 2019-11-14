using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using MOG.INI;
using MOG.CONTROLLER.CONTROLLERPROJECT;

namespace MOG_ControlsLibrary.MogForm_ConfigureProject
{
	public partial class MOGWebTabManager : UserControl
	{
		struct webtab
		{
			public string title;
			public string url;
			public int position;
		};

		public MOGWebTabManager()
		{
			InitializeComponent();			
		}

		public void InitializeListView()
		{
			WebTabTabsListView.Items.Clear();

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
					ListViewItem item = new ListViewItem(newtab.title);
					item.SubItems.Add(newtab.url);
					item.SubItems.Add(newtab.position.ToString());

					WebTabTabsListView.Items.Add(item);
				}
			}
		}

		private void WebTabAddButton_Click(object sender, EventArgs e)
		{
			if (WebTabTitleTextBox.Text.Length != 0 &&
				WebTabURLTextBox.Text.Length != 0)
			{
				MOG_PropertiesIni project = MOG_ControllerProject.GetProject().GetConfigFile();
				if (project != null)
				{
					project.PutSectionString("WebTabs", WebTabTitleTextBox.Text);
					project.PutString("WebTab_" + WebTabTitleTextBox.Text, "Title", WebTabTitleTextBox.Text);
					project.PutString("WebTab_" + WebTabTitleTextBox.Text, "URL", WebTabURLTextBox.Text);
					project.PutString("WebTab_" + WebTabTitleTextBox.Text, "POSITION", "-1");
					project.Save();

					ListViewItem item = new ListViewItem(WebTabTitleTextBox.Text);
					item.SubItems.Add(WebTabURLTextBox.Text);
					item.SubItems.Add("");

					WebTabTabsListView.Items.Add(item);

					SavePositionSettings(project);
				}
			}
		}

		private void removeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (WebTabTabsListView.SelectedItems != null)
			{
				MOG_PropertiesIni project = MOG_ControllerProject.GetProject().GetConfigFile();
				if (project != null)
				{
					foreach (ListViewItem item in WebTabTabsListView.SelectedItems)
					{
						project.RemoveString("WebTabs", item.Text);
						project.RemoveString("WebTab_" + item.Text, "Title");
						project.RemoveString("WebTab_" + item.Text, "URL");
						project.RemoveString("WebTab_" + item.Text, "Position");

						// Only remove the section if it is now empty
						if (project.CountKeys("WebTab_" + item.Text) == 0)
						{
							project.RemoveSection("WebTab_" + item.Text);
						}

						item.Remove();
					}

					SavePositionSettings(project);					
				}
			}
		}

		private void moveUpToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (WebTabTabsListView.SelectedItems != null)
			{
				WebTabTabsListView.BeginUpdate();
				ListView.SelectedIndexCollection col = WebTabTabsListView.SelectedIndices;
				for (int i = 0; i <= WebTabTabsListView.Items.Count - 1; i++)
				{
					ListViewItem o = WebTabTabsListView.Items[i];

					if ((col.Contains(i)) && (i > 0))
					{
						WebTabTabsListView.Items.RemoveAt(i);
						WebTabTabsListView.Items.Insert(i - 1, o);
						//WebTabTabsListView.SetSelected(i - 1, true);
					}					
				}

				for (int pos = 0; pos < WebTabTabsListView.Items.Count; pos++)
				{
					WebTabTabsListView.Items[pos].SubItems[2].Text = pos.ToString();
				}
				WebTabTabsListView.EndUpdate();

				SavePositionSettings();
			}
		}

		private void moveDownToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (WebTabTabsListView.SelectedItems != null)
			{
				WebTabTabsListView.BeginUpdate();
				ListView.SelectedIndexCollection col = WebTabTabsListView.SelectedIndices;
				for (int i = 0; i <= WebTabTabsListView.Items.Count - 1; i++)
				{
					if ((col.Contains(i)) && (i < WebTabTabsListView.Items.Count -1))
					{
						ListViewItem o = WebTabTabsListView.Items[i];
						WebTabTabsListView.Items.Remove(o);
						WebTabTabsListView.Items.Insert(i+1, o);
						break;						
					}
				}

				for (int pos = 0; pos < WebTabTabsListView.Items.Count; pos++)
				{
					WebTabTabsListView.Items[pos].SubItems[2].Text = pos.ToString();
				}
				WebTabTabsListView.EndUpdate();

				SavePositionSettings();
			}
		}

		private void SavePositionSettings()
		{
			SavePositionSettings(null);
		}

		private void SavePositionSettings(MOG_PropertiesIni project)
		{
			if (project == null)
			{
				project = MOG_ControllerProject.GetProject().GetConfigFile();
			}

			int i = 0;
			if (project != null)
			{
				foreach (ListViewItem item in WebTabTabsListView.Items)
				{
					item.SubItems[2].Text = i.ToString();
					project.PutString("WebTab_" + item.Text, "POSITION", item.SubItems[2].Text);
					i++;
				}

				project.Save();
			}			
		}
	}
}
