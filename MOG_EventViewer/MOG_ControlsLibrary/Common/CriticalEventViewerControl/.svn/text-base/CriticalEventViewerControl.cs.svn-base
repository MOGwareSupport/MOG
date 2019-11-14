using System;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using MOG.TIME;
using MOG.COMMAND;
using MOG.REPORT;
using MOG.DATABASE;
using MOG.PROGRESS;
using MOG.PROMPT;

using EV.Windows.Forms;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace MOG_EventViewer.MOG_ControlsLibrary
{
	/// <summary>
	/// Summary description for EventViewerControl.
	/// </summary>
	public class CriticalEventViewerControl : System.Windows.Forms.UserControl
	{
		#region System defs

		private System.Windows.Forms.ImageList imageList;
		private System.Windows.Forms.ContextMenuStrip cmEventList;
		private System.Windows.Forms.ToolStripMenuItem miRefresh;
		private System.Windows.Forms.ToolStripMenuItem miPurgeAll;
		private System.Windows.Forms.ToolStripMenuItem miPurgeSelected;
		private System.Windows.Forms.ToolStripSeparator menuItem5;
		private System.Windows.Forms.ToolStripMenuItem miColumns;
		private System.Windows.Forms.ToolStripSeparator menuItem1;
		private System.Windows.Forms.ListView lvfEvents;
		private System.Windows.Forms.StatusBar statusBar;
		private System.Windows.Forms.StatusBarPanel sbpNumEvents;
		private System.Windows.Forms.StatusBarPanel sbpNumSelectedEvents;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.ToolStripMenuItem miProperties;
		private System.Windows.Forms.ToolStripSeparator menuItem4;
		private System.Windows.Forms.ToolStripMenuItem miSelectAll;
		private System.Windows.Forms.ToolStripMenuItem miFilters;
		private System.Windows.Forms.ToolStripMenuItem miTypeFilter;
		private System.Windows.Forms.ToolStripMenuItem miProjectFilter;
		private System.Windows.Forms.ToolStripMenuItem miUserFilter;
		private System.Windows.Forms.ToolStripMenuItem miComputerFilter;
		private System.Windows.Forms.ToolStripMenuItem miBranchFilter;
		private System.Windows.Forms.TreeView tvFilters;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.ToolStripMenuItem miDumpToFile;
		private System.ComponentModel.IContainer components;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CriticalEventViewerControl));
			this.cmEventList = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.miRefresh = new System.Windows.Forms.ToolStripMenuItem();
			this.miSelectAll = new System.Windows.Forms.ToolStripMenuItem();
			this.miDumpToFile = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.miColumns = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItem5 = new System.Windows.Forms.ToolStripSeparator();
			this.miPurgeSelected = new System.Windows.Forms.ToolStripMenuItem();
			this.miPurgeAll = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItem4 = new System.Windows.Forms.ToolStripSeparator();
			this.miFilters = new System.Windows.Forms.ToolStripMenuItem();
			this.miTypeFilter = new System.Windows.Forms.ToolStripMenuItem();
			this.miUserFilter = new System.Windows.Forms.ToolStripMenuItem();
			this.miProjectFilter = new System.Windows.Forms.ToolStripMenuItem();
			this.miComputerFilter = new System.Windows.Forms.ToolStripMenuItem();
			this.miBranchFilter = new System.Windows.Forms.ToolStripMenuItem();
			this.miProperties = new System.Windows.Forms.ToolStripMenuItem();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.statusBar = new System.Windows.Forms.StatusBar();
			this.sbpNumEvents = new System.Windows.Forms.StatusBarPanel();
			this.sbpNumSelectedEvents = new System.Windows.Forms.StatusBarPanel();
			this.lvfEvents = new System.Windows.Forms.ListView();
			this.panel1 = new System.Windows.Forms.Panel();
			this.tvFilters = new System.Windows.Forms.TreeView();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.cmEventList.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.sbpNumEvents)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sbpNumSelectedEvents)).BeginInit();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// cmEventList
			// 
			this.cmEventList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miRefresh,
            this.miSelectAll,
            this.miDumpToFile,
            this.menuItem1,
            this.miColumns,
            this.menuItem5,
            this.miPurgeSelected,
            this.miPurgeAll,
            this.menuItem4,
            this.miFilters,
            this.miProperties});
			this.cmEventList.Name = "cmEventList";
			this.cmEventList.Size = new System.Drawing.Size(196, 198);
			// 
			// miRefresh
			// 
			this.miRefresh.Name = "miRefresh";
			this.miRefresh.ShortcutKeys = System.Windows.Forms.Keys.F5;
			this.miRefresh.Size = new System.Drawing.Size(195, 22);
			this.miRefresh.Text = "Refresh";
			this.miRefresh.Click += new System.EventHandler(this.miRefresh_Click);
			// 
			// miSelectAll
			// 
			this.miSelectAll.Name = "miSelectAll";
			this.miSelectAll.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
			this.miSelectAll.Size = new System.Drawing.Size(195, 22);
			this.miSelectAll.Text = "Select All";
			this.miSelectAll.Click += new System.EventHandler(this.miSelectAll_Click);
			// 
			// miDumpToFile
			// 
			this.miDumpToFile.Name = "miDumpToFile";
			this.miDumpToFile.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
			this.miDumpToFile.Size = new System.Drawing.Size(195, 22);
			this.miDumpToFile.Text = "Dump to File...";
			this.miDumpToFile.Click += new System.EventHandler(this.miDumpToFile_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Name = "menuItem1";
			this.menuItem1.Size = new System.Drawing.Size(192, 6);
			// 
			// miColumns
			// 
			this.miColumns.Name = "miColumns";
			this.miColumns.Size = new System.Drawing.Size(195, 22);
			this.miColumns.Text = "Columns";
			// 
			// menuItem5
			// 
			this.menuItem5.Name = "menuItem5";
			this.menuItem5.Size = new System.Drawing.Size(192, 6);
			// 
			// miPurgeSelected
			// 
			this.miPurgeSelected.Name = "miPurgeSelected";
			this.miPurgeSelected.ShortcutKeys = System.Windows.Forms.Keys.Delete;
			this.miPurgeSelected.Size = new System.Drawing.Size(195, 22);
			this.miPurgeSelected.Text = "Delete";
			this.miPurgeSelected.Click += new System.EventHandler(this.miPurgeSelected_Click);
			// 
			// miPurgeAll
			// 
			this.miPurgeAll.Name = "miPurgeAll";
			this.miPurgeAll.Size = new System.Drawing.Size(195, 22);
			this.miPurgeAll.Text = "Delete All";
			this.miPurgeAll.Click += new System.EventHandler(this.miPurgeAll_Click);
			// 
			// menuItem4
			// 
			this.menuItem4.Name = "menuItem4";
			this.menuItem4.Size = new System.Drawing.Size(192, 6);
			// 
			// miFilters
			// 
			this.miFilters.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miTypeFilter,
            this.miUserFilter,
            this.miProjectFilter,
            this.miComputerFilter,
            this.miBranchFilter});
			this.miFilters.Name = "miFilters";
			this.miFilters.Size = new System.Drawing.Size(195, 22);
			this.miFilters.Text = "Filter";
			// 
			// miTypeFilter
			// 
			this.miTypeFilter.Name = "miTypeFilter";
			this.miTypeFilter.Size = new System.Drawing.Size(132, 22);
			this.miTypeFilter.Text = "Type";
			// 
			// miUserFilter
			// 
			this.miUserFilter.Name = "miUserFilter";
			this.miUserFilter.Size = new System.Drawing.Size(132, 22);
			this.miUserFilter.Text = "User";
			// 
			// miProjectFilter
			// 
			this.miProjectFilter.Name = "miProjectFilter";
			this.miProjectFilter.Size = new System.Drawing.Size(132, 22);
			this.miProjectFilter.Text = "Project";
			// 
			// miComputerFilter
			// 
			this.miComputerFilter.Name = "miComputerFilter";
			this.miComputerFilter.Size = new System.Drawing.Size(132, 22);
			this.miComputerFilter.Text = "Computer";
			// 
			// miBranchFilter
			// 
			this.miBranchFilter.Name = "miBranchFilter";
			this.miBranchFilter.Size = new System.Drawing.Size(132, 22);
			this.miBranchFilter.Text = "Branch";
			// 
			// miProperties
			// 
			this.miProperties.Name = "miProperties";
			this.miProperties.Size = new System.Drawing.Size(195, 22);
			this.miProperties.Text = "Properties";
			this.miProperties.Click += new System.EventHandler(this.miProperties_Click);
			// 
			// imageList
			// 
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList.Images.SetKeyName(0, "");
			this.imageList.Images.SetKeyName(1, "");
			this.imageList.Images.SetKeyName(2, "");
			this.imageList.Images.SetKeyName(3, "");
			this.imageList.Images.SetKeyName(4, "");
			this.imageList.Images.SetKeyName(5, "");
			this.imageList.Images.SetKeyName(6, "");
			this.imageList.Images.SetKeyName(7, "");
			this.imageList.Images.SetKeyName(8, "");
			this.imageList.Images.SetKeyName(9, "");
			this.imageList.Images.SetKeyName(10, "");
			this.imageList.Images.SetKeyName(11, "");
			this.imageList.Images.SetKeyName(12, "");
			// 
			// statusBar
			// 
			this.statusBar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.statusBar.Location = new System.Drawing.Point(0, 426);
			this.statusBar.Name = "statusBar";
			this.statusBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.sbpNumEvents,
            this.sbpNumSelectedEvents});
			this.statusBar.ShowPanels = true;
			this.statusBar.Size = new System.Drawing.Size(703, 22);
			this.statusBar.TabIndex = 11;
			// 
			// sbpNumEvents
			// 
			this.sbpNumEvents.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
			this.sbpNumEvents.Icon = ((System.Drawing.Icon)(resources.GetObject("sbpNumEvents.Icon")));
			this.sbpNumEvents.Name = "sbpNumEvents";
			this.sbpNumEvents.Text = "0 events";
			this.sbpNumEvents.Width = 343;
			// 
			// sbpNumSelectedEvents
			// 
			this.sbpNumSelectedEvents.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
			this.sbpNumSelectedEvents.Icon = ((System.Drawing.Icon)(resources.GetObject("sbpNumSelectedEvents.Icon")));
			this.sbpNumSelectedEvents.Name = "sbpNumSelectedEvents";
			this.sbpNumSelectedEvents.Text = "0 events selected";
			this.sbpNumSelectedEvents.Width = 343;
			// 
			// lvfEvents
			// 
			this.lvfEvents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lvfEvents.ContextMenuStrip = this.cmEventList;
			this.lvfEvents.FullRowSelect = true;
			this.lvfEvents.GridLines = true;
			this.lvfEvents.HideSelection = false;
			this.lvfEvents.LargeImageList = this.imageList;
			this.lvfEvents.Location = new System.Drawing.Point(0, 0);
			this.lvfEvents.Name = "lvfEvents";
			this.lvfEvents.Size = new System.Drawing.Size(698, 424);
			this.lvfEvents.SmallImageList = this.imageList;
			this.lvfEvents.TabIndex = 10;
			this.lvfEvents.UseCompatibleStateImageBehavior = false;
			this.lvfEvents.View = System.Windows.Forms.View.Details;
			this.lvfEvents.VirtualMode = true;
			this.lvfEvents.DoubleClick += new System.EventHandler(this.lvfEvents_DoubleClick);
			this.lvfEvents.SelectedIndexChanged += new System.EventHandler(this.lvfEvents_SelectedIndexChanged);
			this.lvfEvents.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.lvfEvents_RetrieveVirtualItem);
			this.lvfEvents.CacheVirtualItems += new System.Windows.Forms.CacheVirtualItemsEventHandler(this.lvfEvents_CacheVirtualItems);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.statusBar);
			this.panel1.Controls.Add(this.lvfEvents);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(185, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(703, 448);
			this.panel1.TabIndex = 12;
			// 
			// tvFilters
			// 
			this.tvFilters.Dock = System.Windows.Forms.DockStyle.Left;
			this.tvFilters.ImageIndex = 0;
			this.tvFilters.ImageList = this.imageList;
			this.tvFilters.Location = new System.Drawing.Point(0, 0);
			this.tvFilters.Name = "tvFilters";
			this.tvFilters.SelectedImageIndex = 0;
			this.tvFilters.Size = new System.Drawing.Size(185, 448);			
			this.tvFilters.TabIndex = 12;
			this.tvFilters.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvFilters_AfterSelect);
			// 
			// splitter1
			// 
			this.splitter1.Location = new System.Drawing.Point(185, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(3, 448);
			this.splitter1.TabIndex = 13;
			this.splitter1.TabStop = false;
			// 
			// CriticalEventViewerControl
			// 
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.tvFilters);
			this.Name = "CriticalEventViewerControl";
			this.Size = new System.Drawing.Size(888, 448);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lvfEvents_KeyUp);
			this.cmEventList.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.sbpNumEvents)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sbpNumSelectedEvents)).EndInit();
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
		#endregion
		#region Member vars
		private MOG_Database mDatabase = null;
		private ArrayList mRawEventList = new ArrayList();

		private List<string> mAllColumns = new List<string>(new string[] { "Type", "Timestamp", "Description", "Username", "Computer", "Project", "Branch" });
		private List<string> mVisibleColumns = new List<string>();

		private List<ListViewItem> mAllListViewItems = new List<ListViewItem>();
		private List<ListViewItem> mFilteredListViewItems = new List<ListViewItem>();

		private CriticalEventPropertiesForm mEventPropertiesForm = null;

		private Filter mFilter = new Filter();

		#endregion
		#region Delegates
		#endregion
		#region Properties
		public MOG_Database Database
		{
			get { return mDatabase; }
			set { mDatabase = value; }
		}
		#endregion
		#region Constants
		private const int EXCEPTION_INDEX = 0;
		private const int ERROR_INDEX = 1;
		private const int WARNING_INDEX = 2;
		private const int ALERT_INDEX = 3;
		private const int INFO_INDEX = 4;
		private const int FOLDER_INDEX = 5;
		private const int ARROW_INDEX = 6;
		private const int QUESTION_INDEX = 7;
		private const int USER_INDEX = 8;
		private const int COMPUTER_INDEX = 9;
		private const int PROJECT_INDEX = 10;
		private const int BRANCH_INDEX = 11;
		private const int FILDER_INDEX = 12;
		private const int TYPE_INDEX = ARROW_INDEX;
		#endregion

		#region Constructor
		private enum ArrowType { Ascending, Descending }

		private Bitmap GetArrowBitmap(ArrowType type)
		{
			Bitmap bmp = new Bitmap(16, 16);
			Graphics gfx = Graphics.FromImage(bmp);

			Pen lightPen = SystemPens.ControlLightLight;
			Pen shadowPen = SystemPens.ControlDark;

			gfx.FillRectangle(System.Drawing.Brushes.Magenta, 3, 3, 11, 11);

			if (type == ArrowType.Ascending)
			{
				gfx.DrawLine(lightPen, 3, 10, 10, 10);
				gfx.DrawLine(lightPen, 10, 10, 7, 3);
				gfx.DrawLine(shadowPen, 6, 3, 3, 10);
			}
			else if (type == ArrowType.Descending)
			{
				gfx.DrawLine(lightPen, 7, 10, 10, 3);
				gfx.DrawLine(shadowPen, 6, 10, 3, 3);
				gfx.DrawLine(shadowPen, 3, 3, 10, 3);
			}

			gfx.Dispose();

			return bmp;
		}

		public CriticalEventViewerControl()
		{
			InitializeComponent();

			ImageList list = lvfEvents.SmallImageList;
			list.TransparentColor = System.Drawing.Color.Magenta;
			list.Images.Add(GetArrowBitmap(ArrowType.Ascending));
			list.Images.Add(GetArrowBitmap(ArrowType.Descending));

			mVisibleColumns = new List<string>(mAllColumns);

			lvfEvents.Columns.Clear();
			foreach (string colName in mVisibleColumns)
			{
				ColumnHeader header = lvfEvents.Columns.Add(colName, 100, HorizontalAlignment.Left);
			}

			lvfEvents.ColumnClick += Column_Clicked;
		}
		#endregion

		private void Column_Clicked(object sender, ColumnClickEventArgs e)
		{
			for (int i = 0; i < lvfEvents.Columns.Count; i++)
			{
				if (i != e.Column)
				{
					ColumnHeader header = lvfEvents.Columns[i];
					if (header.ImageIndex != -1)
					{
						lvfEvents.Sorting = SortOrder.None;
					}

					header.ImageIndex = -1;
				}
			}
			
			ColumnHeader column = lvfEvents.Columns[e.Column];

			if (lvfEvents.Sorting != SortOrder.Ascending)
			{
				lvfEvents.Sorting = SortOrder.Ascending;
				column.ImageIndex = column.ImageList.Images.Count - 2;
			}
			else
			{
				lvfEvents.Sorting = SortOrder.Descending;
				column.ImageIndex = column.ImageList.Images.Count - 1;
			}
			
			mFilteredListViewItems.Sort(new ColumnSorter(e.Column, lvfEvents.Sorting == SortOrder.Ascending));

			lvfEvents.Refresh();
		}

		private string EventToString(ListViewItem item)
		{
			string s = "";

			if (!(item.Tag is EventData))
				return s;

			MOG_DBEventInfo info = ((EventData)item.Tag).eventInfo;
			foreach (string colName in mAllColumns)
			{
				s += GetCommandData(colName, info) + "\t";
			}

			return s;
		}

		private void ShowPropertiesWindow(int index)
		{
			if (mEventPropertiesForm == null || mEventPropertiesForm.IsDisposed)
			{
				mEventPropertiesForm = new CriticalEventPropertiesForm(lvfEvents);
			}

			mEventPropertiesForm.LoadEvent(index);
			mEventPropertiesForm.ShowDialog(this);
		}

		private void UpdateStatusBar()
		{
			sbpNumEvents.Text = lvfEvents.Items.Count.ToString() + " visible event" + (lvfEvents.Items.Count == 1 ? "" : "s") + " (" + mRawEventList.Count.ToString() + " total)";
			sbpNumSelectedEvents.Text = lvfEvents.SelectedIndices.Count.ToString() + " event" + (lvfEvents.SelectedIndices.Count == 1 ? "" : "s") + " selected";
		}

		private void SaveAll()
		{
			lock (mAllListViewItems)
			{
				foreach (ListViewItem item in mAllListViewItems)
				{
					EventData eventData = item.Tag as EventData;
					if (eventData != null && !eventData.existsInDB)
					{
						SaveEvent(eventData.eventInfo);
					}
				}
			}
		}

		public void SelectAll()
		{
			for (int i = 0; i < lvfEvents.VirtualListSize; i++)
			{
				ListViewItem lvItem = lvfEvents.Items[i];
				lvItem.Selected = true;
			}
		}

		public void RefreshEvents()
		{
			mRawEventList = MOG_DBEventAPI.GetEvents(null, null, null, null);

			if (mRawEventList != null)
			{
				PopulateEventList();
				PopulateFilterNodes();
				FilterEvents();
			}
		}

		private void PopulateEventList()
		{
			lock (mAllListViewItems)
			{
				mAllListViewItems.Clear();
				mFilteredListViewItems.Clear();

				foreach (MOG_DBEventInfo info in mRawEventList)
				{
					ListViewItem item = CreateItemFromDBEventInfo(info, true);
					mAllListViewItems.Add(item);
					mFilteredListViewItems.Add(item);
				}
			}

			lvfEvents.VirtualListSize = mFilteredListViewItems.Count;
		}

		private void lvfEvents_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
		{
			if (e.ItemIndex < mFilteredListViewItems.Count)
			{
				e.Item = mFilteredListViewItems[e.ItemIndex];
			}
		}

		private void lvfEvents_CacheVirtualItems(object sender, CacheVirtualItemsEventArgs e)
		{
			Console.Out.WriteLine("Caching items " + e.StartIndex + " to " + e.EndIndex);
		}

		private void PurgeAll()
		{
			MOG_DBEventAPI.RemoveEvents(mRawEventList);
			RefreshEvents();
		}

		private void PurgeSelected()
		{
			ArrayList eventInfoList = new ArrayList();

			foreach (int index in lvfEvents.SelectedIndices)
			{
				ListViewItem item = lvfEvents.Items[index];
				if (item != null)
				{
					mAllListViewItems.Remove(item);

					EventData eventData = item.Tag as EventData;
					if (eventData != null)
					{
						eventInfoList.Add(eventData.eventInfo);
						mRawEventList.Remove(eventData.eventInfo);
					}
				}
			}

			if (eventInfoList.Count > 0)
			{
				MOG_DBEventAPI.RemoveEvents(eventInfoList);
			}

			for (int i = lvfEvents.SelectedIndices.Count - 1; i >= 0; i--)
			{
				mFilteredListViewItems.RemoveAt(lvfEvents.SelectedIndices[i]);
				lvfEvents.VirtualListSize--;
			}

			lvfEvents.SelectedIndices.Clear();

			lvfEvents.Refresh();
		}

		private void FilterEvents()
		{
			lock (mAllListViewItems)
			{
				lock (mFilteredListViewItems)
				{
					// Set VirtualListSize to zero to prevent it from trying to retrieve items while we operate on it below
					lvfEvents.VirtualListSize = 0;
					lvfEvents.TopItem = null;

					mFilteredListViewItems.Clear();

					foreach (ListViewItem item in mAllListViewItems)
					{
						EventData eventData = item.Tag as EventData;
						if (eventData != null)
						{
							MOG_DBEventInfo info = eventData.eventInfo as MOG_DBEventInfo;
							if (info != null)
							{
								if (mFilter.Contains("Types", info.mType) &&
									mFilter.Contains("Users", info.mUserName) &&
									mFilter.Contains("Computers", info.mComputerName) &&
									mFilter.Contains("Projects", info.mProjectName) &&
									mFilter.Contains("Branches", info.mBranchName))
								{
									mFilteredListViewItems.Add(item);
								}
							}
						}
					}
				}
			}

			// Check if we have anything in mFilteredListViewItems?
			if (mFilteredListViewItems.Count > 0)
			{
				lvfEvents.VirtualListSize = mFilteredListViewItems.Count;
				lvfEvents.TopItem = mFilteredListViewItems[0];
			}

			lvfEvents.Refresh();
			UpdateStatusBar();
		}

		// populate the filter nodes based on what events have been loaded
		private void PopulateFilterNodes()
		{
			tvFilters.Nodes.Clear();
			TreeNode root = new TreeNode("Filters", FILDER_INDEX, FILDER_INDEX);
			root.Nodes.Add(new TreeNode("Types", TYPE_INDEX, TYPE_INDEX));
			root.Nodes.Add(new TreeNode("Users", USER_INDEX, USER_INDEX));
			root.Nodes.Add(new TreeNode("Computers", COMPUTER_INDEX, COMPUTER_INDEX));
			root.Nodes.Add(new TreeNode("Projects", PROJECT_INDEX, PROJECT_INDEX));
			root.Nodes.Add(new TreeNode("Branches", BRANCH_INDEX, BRANCH_INDEX));
			
			tvFilters.Nodes.Add(root);
			root.Expand();

			HybridDictionary nodeDictionary = new HybridDictionary(true);

			foreach (TreeNode node in root.Nodes)
			{
				nodeDictionary.Add(node.Text, node);
			}

			SortedDictionary<string, string>[] dictionaries = new SortedDictionary<string, string>[5] { new SortedDictionary<string, string>(),
																							new SortedDictionary<string, string>(),
																							new SortedDictionary<string, string>(),
																							new SortedDictionary<string, string>(),
																							new SortedDictionary<string, string>()};

			lock (mAllListViewItems)
			{
				foreach (ListViewItem item in mAllListViewItems)
				{
					EventData eventData = item.Tag as EventData;
					if (eventData != null)
					{
						dictionaries[0][eventData.eventInfo.mType] = "Types";
						dictionaries[1][eventData.eventInfo.mUserName] = "Users";
						dictionaries[2][eventData.eventInfo.mProjectName] = "Projects";
						dictionaries[3][eventData.eventInfo.mComputerName] = "Computers";
						dictionaries[4][eventData.eventInfo.mBranchName] = "Branches";
					}
				}
			}

			foreach (SortedDictionary<string, string> dictionary in dictionaries)
			{
				foreach (string text in dictionary.Keys)
				{
					if (text.Length > 0)
					{
						TreeNode node = nodeDictionary[dictionary[text]] as TreeNode;
						if (node != null)
						{
							node.Nodes.Add(new TreeNode(text, node.ImageIndex, node.SelectedImageIndex));
							//miTypeFilter.Items.Add(new ToolStripMenuItem(type, new EventHandler(miFilterSubmenu_Click)));
						}
					}
				}
			}
		}

		private bool SaveEvent(MOG_DBEventInfo eventInfo)
		{
			if (mDatabase != null && eventInfo != null)
			{
				if (eventInfo.mType == "")
					eventInfo.mType = "<type>";
				if (eventInfo.mTimeStamp == "")
					eventInfo.mTimeStamp = "<timestamp>";
				if (eventInfo.mDescription == "")
					eventInfo.mDescription = "<description>";
				if (eventInfo.mEventID == "")
					eventInfo.mEventID = "<eventid>";
				if (eventInfo.mUserName == "")
					eventInfo.mUserName = "<username>";
				if (eventInfo.mComputerName == "")
					eventInfo.mComputerName = "<computername>";
				if (eventInfo.mProjectName == "")
					eventInfo.mProjectName = "<projectname>";
				if (eventInfo.mBranchName == "")
					eventInfo.mBranchName = "<current>";

				MOG_DBEventAPI.AddEvent(eventInfo.mType, eventInfo.mTimeStamp, eventInfo.mTitle, eventInfo.mDescription, eventInfo.mStackTrace, eventInfo.mEventID, eventInfo.mUserName, eventInfo.mComputerName, false);
				return true;
			}

			return false;
		}

		private ListViewItem CreateItemFromDBEventInfo(MOG_DBEventInfo eventInfo, bool fromDB)
		{
			ListViewItem item = ConstructListViewItem(eventInfo);
			((EventData)item.Tag).existsInDB = fromDB;

			switch (eventInfo.mType.ToUpper())
			{
			case "MOG_COMMAND_NOTIFYSYSTEMERROR":
			case "MOG_COMMAND_ERROR":
			case "ERROR":
			case "SYSTEMERROR":
				item.ImageIndex = ERROR_INDEX;
				item.ForeColor = Color.Red;
				break;
			case "MOG_COMMAND_NOTIFYSYSTEMWARNING":
			case "WARNING":
			case "SYSTEMWARNING":
				item.ImageIndex = WARNING_INDEX;
				item.ForeColor = Color.Black;
				break;
			case "MOG_COMMAND_NOTIFYSYSTEMEXCEPTION":
			case "MOG_COMMAND_EXCEPTION":
			case "EXCEPTION":
			case "SYSTEMEXCEPTION":
				item.ImageIndex = EXCEPTION_INDEX;
				item.ForeColor = Color.Black;
				break;
			case "MOG_COMMAND_NOTIFYSYSTEMALERT":
			case "MOG_COMMAND_ALERT":
			case "ALERT":
			case "SYSTEMALERT":
				item.ImageIndex = WARNING_INDEX;
				item.ForeColor = Color.Blue;
				break;
			}

			return item;
		}

		private ListViewItem ConstructListViewItem(MOG_DBEventInfo info)
		{
			ListViewItem item = null;

			if (info != null && mVisibleColumns != null && mVisibleColumns.Count > 0)
			{
				// construct listviewitem
				item = new ListViewItem();

				for (int i = 0; i < mVisibleColumns.Count; i++)
				{
					string text = GetCommandData(mVisibleColumns[i], info);
					if (text.Length >= 260)
					{
						text = text.Substring(0, 259);
					}

					if (i == 0)
					{
						item.Text = text;
					}
					else
					{
						item.SubItems.Add(text);
					}
				}

				EventData eventData = new EventData();
				eventData.eventInfo = info;
				item.Tag = eventData;
			}

			return item;
		}

		public static string GetCommandData(string colName, MOG_DBEventInfo info)
		{
			if (colName != null && info != null)
			{
				switch (colName)
				{
				case "Id":
					return info.mID.ToString();
				case "Type":
					return info.mType.Replace("MOG_COMMAND_", "");
				case "Timestamp":
					return new MOG_Time(info.mTimeStamp).FormatString("");
				case "Description":
					string description = info.mDescription;
					description = description.Replace("\n", " ");
					return description;
				case "Eventid":
					return info.mEventID;
				case "Username":
					return info.mUserName;
				case "Computer":
					return info.mComputerName;
				case "Project":
					return info.mProjectName;
				case "Branch":
					return info.mBranchName;
				default:
					return "Unknown column name: \"" + colName + "\"";
				}
			}

			return "";
		}

		#region Event handlers
		private void miRefresh_Click(object sender, System.EventArgs e)
		{
			RefreshEvents();
		}

		private void miFilterSubmenu_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem item = sender as ToolStripMenuItem;
			if (item != null)
			{
				ToolStripMenuItem parent = item.OwnerItem as ToolStripMenuItem;
				if (parent != null)
				{
					if (mFilter.IsValidCategory(parent.Text))
					{
						// toggle checkmark
						item.Checked = !item.Checked;

						if (item.Checked)
						{
							// user just checked this filter
							mFilter.Add(parent.Text, item.Text);
						}
						else
						{
							// user just unchecked this filter
							mFilter.Remove(parent.Text, item.Text);
						}

						FilterEvents();
					}
				}
			}
		}

		private void miPurgeSelected_Click(object sender, System.EventArgs e)
		{
			string msg;
			if (lvfEvents.SelectedIndices.Count == 1)
			{
				msg = "Are you sure you want to permanently erase this event?";
			}
			else
			{
				msg = "Are you sure you want to permanently erase these " + lvfEvents.SelectedIndices.Count.ToString() + " events?";
			}

			if (Utils.ShowMessageBoxConfirmation(msg, "Confirm Purge") == DialogResult.Yes)
			{
				PurgeSelected();
			}
		}

		private void miPurgeAll_Click(object sender, System.EventArgs e)
		{
			if (Utils.ShowMessageBoxConfirmation("Are you sure you want to permanently erase all events?", "Confirm Purge") == DialogResult.Yes)
			{
				PurgeAll();
			}
		}

		private void lvfEvents_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyData == Keys.Enter && lvfEvents.SelectedIndices.Count > 0)
			{
				ShowPropertiesWindow(lvfEvents.SelectedIndices[0]);
			}
		}

		private void cmEventList_Popup(object sender, System.EventArgs e)
		{
			if (lvfEvents.SelectedIndices.Count > 0)
			{
				miPurgeSelected.Enabled = true;
				miProperties.Enabled = true;
				miDumpToFile.Enabled = true;
			}
			else
			{
				miPurgeSelected.Enabled = false;
				miProperties.Enabled = false;
				miDumpToFile.Enabled = false;
			}
		}

		private void lvfEvents_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			UpdateStatusBar();
		}

		private void miProperties_Click(object sender, System.EventArgs e)
		{
			if (lvfEvents.SelectedIndices.Count > 0)
			{
				ShowPropertiesWindow(lvfEvents.SelectedIndices[0]);
			}
		}

		private void lvfEvents_DoubleClick(object sender, System.EventArgs e)
		{
			if (lvfEvents.SelectedIndices.Count > 0)
			{
				ShowPropertiesWindow(lvfEvents.SelectedIndices[0]);
			}
		}

		private void miSelectAll_Click(object sender, System.EventArgs e)
		{
			SelectAll();
		}

		private void miDumpToFile_Click(object sender, System.EventArgs e)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();

			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				try
				{
					StreamWriter sw = new StreamWriter(saveFileDialog.FileName);
					foreach (string colName in mAllColumns)
					{
						sw.Write(colName + "\t");
					}

					sw.WriteLine();
					sw.WriteLine("======================================================================================================================================================================================================================================================================");
					foreach (int index in lvfEvents.SelectedIndices)
					{
						ListViewItem item = lvfEvents.Items[index];
						sw.WriteLine(EventToString(item));
					}

					sw.Close();

					Utils.ShowMessageBoxInfo("Saved events to " + saveFileDialog.FileName, "Events Saved");
				}
				catch
				{
					Utils.ShowMessageBoxExclamation("Couldn't write events to " + saveFileDialog.FileName, "File I/O Failure");
					return;
				}
			}
		}

		private void tvFilters_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			mFilter.Clear();
			if (e.Node != null && e.Node.Parent != null)
			{
				mFilter.Add(e.Node.Parent.Text, e.Node.Text);
			}

			FilterEvents();
			UpdateStatusBar();
		}
		#endregion
	}

	class EventData
	{
		public MOG_DBEventInfo eventInfo;
		public bool existsInDB = false;
	}

	class Filter
	{
		Dictionary<string, List<string>> mCategories = new Dictionary<string, List<string> >();

		public Filter()
		{
		}
		
		public void Add(string category, string text)
		{
			if (!mCategories.ContainsKey(category))
			{
				mCategories[category] = new List<string>();
			}

			List<string> list = mCategories[category];

			if (!list.Contains(text))
			{
				list.Add(text);
			}
		}

		public void Remove(string category, string text)
		{
			if (mCategories.ContainsKey(category))
			{
				List<string> list = mCategories[category];

				list.Remove(text);
			}
		}

		public bool IsValidCategory(string category)
		{
			return mCategories.ContainsKey(category);
		}
		
		public bool Contains(string category, string text)
		{
			if (mCategories.ContainsKey(category))
			{
				List<string> list = mCategories[category];
				if (list.Count > 0)
				{
					return list.Contains(text);
				}
			}

			//This category isn't in the filter, so just let everything pass
			return true;
		}
		
		public void Clear()
		{
			mCategories.Clear();
		}
	}
}



