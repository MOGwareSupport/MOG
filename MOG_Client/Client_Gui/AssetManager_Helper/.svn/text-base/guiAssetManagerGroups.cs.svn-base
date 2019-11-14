using System;
using System.Collections;
using System.Windows.Forms;

using MOG;
using MOG.FILENAME;
using System.Collections.Generic;
using MOG_ControlsLibrary.MogUtils_Settings;
using MOG_ControlsLibrary;

namespace MOG_Client.Client_Gui.AssetManager_Helper
{

	/// <summary>
	/// Summary description for guiAssetManagerGroups.
	/// </summary>
	public class guiAssetManagerGroups
	{
		SortedList<string, string> mGroups;
		SortedList<string, ListView> mGroupEnabledListViews;

		private bool mSaveSettings = true;
		public bool SaveSettings
		{
			get { return mSaveSettings; }
			set { mSaveSettings = value; }
		}	

		public guiAssetManagerGroups()
		{
			mGroups = new SortedList<string, string>();
			mGroupEnabledListViews = new SortedList<string, ListView>();
		}

		public void Add(ListView listView, string groupName)
		{
			if (mGroups.ContainsKey(listView.Name))
			{
				mGroups.Remove(listView.Name);				
			}			
			mGroups.Add(listView.Name, groupName);

			if (mGroupEnabledListViews.ContainsKey(listView.Name))
			{
				mGroupEnabledListViews.Remove(listView.Name);
			}
			mGroupEnabledListViews.Add(listView.Name, listView);

			listView.ColumnClick += new ColumnClickEventHandler(view_ColumnClick);			
		}

		void view_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			ListView view = sender as ListView;
			if (view != null)
			{
				string group = view.Columns[e.Column].Text;

				if (mSaveSettings)
				{
					MogUtils_Settings.SaveSetting(view.Name, "SortedGroup", group);
				}
				
				SetGroup(view, group);
				UpdateGroups(view);				
			}
		}

		internal void UpdateGroups(ListView view)
		{
			if (view != null && view.ShowGroups)
			{
				CreateGroups(view, mGroups[view.Name]);
			}
		}

		public void UpdateGroupItem(ListView view, ListViewItem item, string defaultGroup)
		{
			// Load our last group from the ini
			defaultGroup = MogUtils_Settings.LoadSetting_default(view.Name, "SortedGroup", defaultGroup);

			string groupText = item.SubItems[ColumnNameFind(view.Columns, defaultGroup)].Text;
			ListViewGroup group = new ListViewGroup(groupText, groupText);
			if (view.Groups[groupText] != null)
				group = view.Groups[groupText];
			else
				view.Groups.Add(group);

			item.Group = group;			
		}

		private void CreateGroups(ListView view, string columnText)
		{
			view.Groups.Clear();

			foreach (ListViewItem item in view.Items)
			{
				string groupText = GetColumnText(view, item, columnText);
				ListViewGroup group = new ListViewGroup(groupText, groupText);
				if (view.Groups[groupText] != null)
					group = view.Groups[groupText];
				else
					view.Groups.Add(group);

				item.Group = group;
			}
		}

		public string GetColumnText(ListView view, ListViewItem item, string defaultColumn)
		{
			// Load our last group from the ini
			string columnText = MogUtils_Settings.LoadSetting_default(view.Name, "SortedGroup", defaultColumn);

			return item.SubItems[ColumnNameFind(view.Columns, columnText)].Text;			
		}

		internal void SetGroup(ListView view, string groupName)
		{
			if (mGroups.ContainsKey(view.Name))
			{
				mGroups.Remove(view.Name);
			}

			mGroups.Add(view.Name, groupName);
		}

		internal void ToggleShowGroups(bool showGroups)
		{
			foreach (ListView view in mGroupEnabledListViews.Values)
			{
				view.ShowGroups = showGroups;
				if (view.ShowGroups)
				{
					UpdateGroups(view);
				}
			}
		}

		internal string GetGroup(MogControl_ListView view)
		{
			if (mGroups.ContainsKey(view.Name))
			{
				return mGroups[view.Name];
			}

			return "";
		}

		internal bool Enable(bool showGroups)
		{
			ToggleShowGroups(showGroups);
			return showGroups;
		}

		/// <summary>
		/// Locates a string in an array and returns its index or 0, if not found
		/// </summary>
		/// <param name="cols"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public int ColumnNameFind(ListView.ColumnHeaderCollection columns, string name)
		{
			return ColumnNameFind(columns, name, false);
		}

		/// <summary>
		/// USE FOR DEBUGGING PURPOSES ONLY.  If returnNegativeOneIfMissing is `true`, this will return an 
		///  invalid index, which will then cause any non-robust code to throw (thereby allowing you to
		///  find why the first column of a given ListView is being set to the incorrect value).
		/// </summary>
		/// <param name="columns"></param>
		/// <param name="name"></param>
		/// <param name="returnNegativeOneIfMissing">If true, this method will return -1 for a missing name</param>
		/// <returns></returns>
		public int ColumnNameFind(ListView.ColumnHeaderCollection columns, string name, bool returnNegativeOneIfMissing)
		{
			int x = 0;
			foreach (ColumnHeader col in columns)
			{
				if (string.Compare(col.Text, name, true) == 0)
				{
					return x;
				}

				x++;
			}

			if (returnNegativeOneIfMissing)
			{
				return -1;
			}
			return 0;
		}

	}
}
