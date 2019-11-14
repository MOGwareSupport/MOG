using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using EV.Windows.Forms;

using MOG_Client.Forms;
using MOG_ControlsLibrary.Common.MOG_ContextMenu;
using MOG_ControlsLibrary.Controls;

using MOG;
using MOG.TIME;
using MOG.FILENAME;
using MOG.DATABASE;
using MOG.REPORT;
using MOG.PROMPT;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.USER;

using MOG_ColumnTreeView = System.Windows.Forms.TreeView;
using MOG_ColumnTreeNode = System.Windows.Forms.TreeNode;
using MOG_ControlsLibrary.Common.MogControl_TaskEditor;


namespace MOG_Client.Client_Mog_Utilities.Tasks
{
	/// <summary>
	/// Summary description for TaskWindow.
	/// </summary>
	public class TaskWindow : System.Windows.Forms.UserControl
	{
		#region MOG vars
		private enum RunningMode {Project, User};
		private MOG_Database db;
		private bool mImportanceSorted;
		private ArrayList mListViewSort_Manager;
		private RunningMode mMode;
		#endregion
		#region Form related vars

		public MOG_ColumnTreeView TasksUsersTasksTreeView;
		private System.Windows.Forms.TabPage TaskView;
		private System.Windows.Forms.TabPage AssetView;
		private System.Windows.Forms.CheckBox TaskHideCompleteCheckBox;
		public System.Windows.Forms.ImageList MOGTasksImageList;
		private System.Windows.Forms.ContextMenu TodoContextMenu;
		private System.Windows.Forms.MenuItem ToDoNewMenuItem;
		private System.Windows.Forms.MenuItem ToDoSep1MenuItem;
		private System.Windows.Forms.MenuItem ToDoDeleteMenuItem;
		private System.Windows.Forms.TabControl TasksTabControl;
		private System.Windows.Forms.MenuItem ToDoSep2MenuItem;
		private System.Windows.Forms.MenuItem ToDoCreateTabMenuItem;
		private System.Windows.Forms.ContextMenu TaskTabsContextMenu;
		private System.Windows.Forms.MenuItem TaskTabsRemoveMenuItem;
		private System.Windows.Forms.MenuItem ToDoEditMenuItem;
		private System.Windows.Forms.ToolBar TaskToolBar;
		private System.Windows.Forms.ToolBarButton TaskSortToolBarButton;
		private System.Windows.Forms.ImageList ToolsImageList;
		public MogControl_FullTreeView TaskRepositoryTreeView;
		public System.Windows.Forms.ListView ProjectManagerTasksListView;
		private System.Windows.Forms.ColumnHeader ProjectManagerTasksTitleColumnHeader;
		private System.Windows.Forms.ColumnHeader ProjectManagerTasksDepartmentColumnHeader;
		private System.Windows.Forms.ColumnHeader ProjectManagerTasksCreatorColumnHeader;
		private System.Windows.Forms.ColumnHeader ProjectManagerTasksStartDateColumnHeader;
		private System.Windows.Forms.ColumnHeader ProjectManagerTasksDueDateColumnHeader;
		private System.Windows.Forms.ColumnHeader ProjectManagerTasksStatusColumnHeader;
		private System.Windows.Forms.ColumnHeader ProjectManagerTasksPercentageColumnHeader;
		private System.Windows.Forms.ColumnHeader ProjectManagerTasksAssignedToColumnHeader;
		private System.Windows.Forms.ColumnHeader ProjectManagerTasksPriorityColumnHeader;
		private System.Windows.Forms.Panel TasksButtonPanel;
		private System.Windows.Forms.MenuItem ToDoAssignToMenuItem;
		private System.Windows.Forms.MenuItem ToDoAssignToDepartmentMenuItem;
		private System.Windows.Forms.MenuItem ToDoSep3MenuItem;
		private System.Windows.Forms.Splitter TaskEditorSplitter;
		private System.Windows.Forms.Splitter TasksSplitter;
		private System.Windows.Forms.ToolBarButton TaskViewEditorToolBarButton;
		private System.Windows.Forms.ToolBarButton TaskViewListToolBarButton;
		private System.Windows.Forms.ToolBarButton toolBarButton1;
		private MogControl_TaskEditor mogControl_TaskEditor;
		private System.ComponentModel.IContainer components;

		#endregion

		[Category("Appearance"), Description("Show the tasks list view")]
		public bool TaskListViewEnabled 
		{
			get 
			{
				return this.ProjectManagerTasksListView.Visible;
			}
			set
			{
				this.ProjectManagerTasksListView.Visible = value;
				this.TasksSplitter.Visible = value;
				if (ProjectManagerTasksListView.Visible == false)
				{
					this.TasksTabControl.Dock = DockStyle.Fill;
				}
				else
				{
					this.TasksTabControl.Dock = DockStyle.Top;
				}
			}
		}

		[Category("Appearance"), Description("Show the tasks editor view")]
		public bool TaskEditorEnabled 
		{
			get 
			{
				return this.mogControl_TaskEditor.Visible;
			}
			set
			{
				this.mogControl_TaskEditor.Visible = value;
				this.TaskEditorSplitter.Visible = value;				
			}
		}
		
		public TaskWindow()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			mListViewSort_Manager = new ArrayList();

			mListViewSort_Manager.Add(new ListViewSortManager(ProjectManagerTasksListView, new Type[] {
																												   typeof(ListViewTextSort),
																												   typeof(ListViewTextCaseInsensitiveSort),
																												   typeof(ListViewTextCaseInsensitiveSort),
																												   typeof(ListViewDateSort),
																												   typeof(ListViewDateSort),
																												   typeof(ListViewTextCaseInsensitiveSort),
																												   typeof(ListViewTextCaseInsensitiveSort),
																												   typeof(ListViewTextCaseInsensitiveSort),
																												   typeof(ListViewTextCaseInsensitiveSort),
			}));
		}

		#region System methods
		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Node1");
			System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Node0", new System.Windows.Forms.TreeNode[] {
            treeNode1});
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TaskWindow));
			this.TasksUsersTasksTreeView = new System.Windows.Forms.TreeView();
			this.TodoContextMenu = new System.Windows.Forms.ContextMenu();
			this.ToDoNewMenuItem = new System.Windows.Forms.MenuItem();
			this.ToDoSep1MenuItem = new System.Windows.Forms.MenuItem();
			this.ToDoAssignToMenuItem = new System.Windows.Forms.MenuItem();
			this.ToDoAssignToDepartmentMenuItem = new System.Windows.Forms.MenuItem();
			this.ToDoSep3MenuItem = new System.Windows.Forms.MenuItem();
			this.ToDoDeleteMenuItem = new System.Windows.Forms.MenuItem();
			this.ToDoSep2MenuItem = new System.Windows.Forms.MenuItem();
			this.ToDoCreateTabMenuItem = new System.Windows.Forms.MenuItem();
			this.ToDoEditMenuItem = new System.Windows.Forms.MenuItem();
			this.MOGTasksImageList = new System.Windows.Forms.ImageList(this.components);
			this.TasksTabControl = new System.Windows.Forms.TabControl();
			this.TaskTabsContextMenu = new System.Windows.Forms.ContextMenu();
			this.TaskTabsRemoveMenuItem = new System.Windows.Forms.MenuItem();
			this.TaskView = new System.Windows.Forms.TabPage();
			this.AssetView = new System.Windows.Forms.TabPage();
			this.TaskRepositoryTreeView = new MOG_ControlsLibrary.Controls.MogControl_FullTreeView();
			this.ProjectManagerTasksListView = new System.Windows.Forms.ListView();
			this.ProjectManagerTasksTitleColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.ProjectManagerTasksDepartmentColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.ProjectManagerTasksCreatorColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.ProjectManagerTasksStartDateColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.ProjectManagerTasksDueDateColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.ProjectManagerTasksStatusColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.ProjectManagerTasksPercentageColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.ProjectManagerTasksAssignedToColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.ProjectManagerTasksPriorityColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.TaskHideCompleteCheckBox = new System.Windows.Forms.CheckBox();
			this.TaskToolBar = new System.Windows.Forms.ToolBar();
			this.TaskSortToolBarButton = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton1 = new System.Windows.Forms.ToolBarButton();
			this.TaskViewEditorToolBarButton = new System.Windows.Forms.ToolBarButton();
			this.TaskViewListToolBarButton = new System.Windows.Forms.ToolBarButton();
			this.ToolsImageList = new System.Windows.Forms.ImageList(this.components);
			this.TasksButtonPanel = new System.Windows.Forms.Panel();
			this.TaskEditorSplitter = new System.Windows.Forms.Splitter();
			this.TasksSplitter = new System.Windows.Forms.Splitter();
			this.mogControl_TaskEditor = new MOG_ControlsLibrary.Common.MogControl_TaskEditor.MogControl_TaskEditor();
			this.TasksTabControl.SuspendLayout();
			this.TaskView.SuspendLayout();
			this.AssetView.SuspendLayout();
			this.TasksButtonPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// TasksUsersTasksTreeView
			// 
			this.TasksUsersTasksTreeView.AllowDrop = true;
			this.TasksUsersTasksTreeView.CheckBoxes = true;
			this.TasksUsersTasksTreeView.ContextMenu = this.TodoContextMenu;
			this.TasksUsersTasksTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TasksUsersTasksTreeView.FullRowSelect = true;
			this.TasksUsersTasksTreeView.HotTracking = true;
			this.TasksUsersTasksTreeView.ImageIndex = 8;
			this.TasksUsersTasksTreeView.ImageList = this.MOGTasksImageList;
			this.TasksUsersTasksTreeView.Indent = 1;
			this.TasksUsersTasksTreeView.ItemHeight = 15;
			this.TasksUsersTasksTreeView.Location = new System.Drawing.Point(0, 0);
			this.TasksUsersTasksTreeView.Name = "TasksUsersTasksTreeView";
			treeNode1.Name = "";
			treeNode1.Text = "Node1";
			treeNode2.Name = "";
			treeNode2.Text = "Node0";
			this.TasksUsersTasksTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode2});
			this.TasksUsersTasksTreeView.SelectedImageIndex = 0;
			this.TasksUsersTasksTreeView.Size = new System.Drawing.Size(232, 210);
			this.TasksUsersTasksTreeView.TabIndex = 4;
			this.TasksUsersTasksTreeView.DragDrop += new System.Windows.Forms.DragEventHandler(this.TasksUsersTasksTreeView_DragDrop);
			this.TasksUsersTasksTreeView.BeforeCheck += new System.Windows.Forms.TreeViewCancelEventHandler(this.TasksUsersTasksTreeView_BeforeCheck);
			this.TasksUsersTasksTreeView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.TasksUsersTasksTreeView_BeforeExpand);
			this.TasksUsersTasksTreeView.DragOver += new System.Windows.Forms.DragEventHandler(this.TasksUsersTasksTreeView_DragOver);
			this.TasksUsersTasksTreeView.DoubleClick += new System.EventHandler(this.TasksUsersTasksTreeView_DoubleClick);
			this.TasksUsersTasksTreeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TasksUsersTasksTreeView_MouseDown);
			this.TasksUsersTasksTreeView.Click += new System.EventHandler(this.TasksUsersTasksTreeView_Click);
			// 
			// TodoContextMenu
			// 
			this.TodoContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.ToDoNewMenuItem,
            this.ToDoSep1MenuItem,
            this.ToDoAssignToMenuItem,
            this.ToDoAssignToDepartmentMenuItem,
            this.ToDoSep3MenuItem,
            this.ToDoDeleteMenuItem,
            this.ToDoSep2MenuItem,
            this.ToDoCreateTabMenuItem,
            this.ToDoEditMenuItem});
			// 
			// ToDoNewMenuItem
			// 
			this.ToDoNewMenuItem.Index = 0;
			this.ToDoNewMenuItem.Shortcut = System.Windows.Forms.Shortcut.Ins;
			this.ToDoNewMenuItem.Text = "New";
			this.ToDoNewMenuItem.Click += new System.EventHandler(this.ToDoNewMenuItem_Click);
			// 
			// ToDoSep1MenuItem
			// 
			this.ToDoSep1MenuItem.Index = 1;
			this.ToDoSep1MenuItem.Text = "-";
			// 
			// ToDoAssignToMenuItem
			// 
			this.ToDoAssignToMenuItem.Index = 2;
			this.ToDoAssignToMenuItem.Text = "Assign to";
			// 
			// ToDoAssignToDepartmentMenuItem
			// 
			this.ToDoAssignToDepartmentMenuItem.Index = 3;
			this.ToDoAssignToDepartmentMenuItem.Text = "Assign to department";
			// 
			// ToDoSep3MenuItem
			// 
			this.ToDoSep3MenuItem.Index = 4;
			this.ToDoSep3MenuItem.Text = "-";
			// 
			// ToDoDeleteMenuItem
			// 
			this.ToDoDeleteMenuItem.Index = 5;
			this.ToDoDeleteMenuItem.Shortcut = System.Windows.Forms.Shortcut.Del;
			this.ToDoDeleteMenuItem.Text = "Delete";
			this.ToDoDeleteMenuItem.Click += new System.EventHandler(this.ToDoDeleteMenuItem_Click);
			// 
			// ToDoSep2MenuItem
			// 
			this.ToDoSep2MenuItem.Index = 6;
			this.ToDoSep2MenuItem.Text = "-";
			// 
			// ToDoCreateTabMenuItem
			// 
			this.ToDoCreateTabMenuItem.Index = 7;
			this.ToDoCreateTabMenuItem.Text = "Create Tab";
			this.ToDoCreateTabMenuItem.Click += new System.EventHandler(this.ToDoCreateTabMenuItem_Click);
			// 
			// ToDoEditMenuItem
			// 
			this.ToDoEditMenuItem.Index = 8;
			this.ToDoEditMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlP;
			this.ToDoEditMenuItem.Text = "Properties";
			this.ToDoEditMenuItem.Click += new System.EventHandler(this.ToDoEditMenuItem_Click);
			// 
			// MOGTasksImageList
			// 
			this.MOGTasksImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("MOGTasksImageList.ImageStream")));
			this.MOGTasksImageList.TransparentColor = System.Drawing.Color.Transparent;
			this.MOGTasksImageList.Images.SetKeyName(0, "");
			this.MOGTasksImageList.Images.SetKeyName(1, "");
			this.MOGTasksImageList.Images.SetKeyName(2, "");
			this.MOGTasksImageList.Images.SetKeyName(3, "");
			this.MOGTasksImageList.Images.SetKeyName(4, "");
			this.MOGTasksImageList.Images.SetKeyName(5, "");
			this.MOGTasksImageList.Images.SetKeyName(6, "");
			this.MOGTasksImageList.Images.SetKeyName(7, "");
			this.MOGTasksImageList.Images.SetKeyName(8, "");
			// 
			// TasksTabControl
			// 
			this.TasksTabControl.ContextMenu = this.TaskTabsContextMenu;
			this.TasksTabControl.Controls.Add(this.TaskView);
			this.TasksTabControl.Controls.Add(this.AssetView);
			this.TasksTabControl.Dock = System.Windows.Forms.DockStyle.Top;
			this.TasksTabControl.Location = new System.Drawing.Point(0, 28);
			this.TasksTabControl.Name = "TasksTabControl";
			this.TasksTabControl.SelectedIndex = 0;
			this.TasksTabControl.Size = new System.Drawing.Size(240, 236);
			this.TasksTabControl.TabIndex = 5;
			this.TasksTabControl.SelectedIndexChanged += new System.EventHandler(this.TasksTabControl_SelectedIndexChanged);
			// 
			// TaskTabsContextMenu
			// 
			this.TaskTabsContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.TaskTabsRemoveMenuItem});
			// 
			// TaskTabsRemoveMenuItem
			// 
			this.TaskTabsRemoveMenuItem.Index = 0;
			this.TaskTabsRemoveMenuItem.Shortcut = System.Windows.Forms.Shortcut.Del;
			this.TaskTabsRemoveMenuItem.Text = "Remove";
			this.TaskTabsRemoveMenuItem.Visible = false;
			this.TaskTabsRemoveMenuItem.Click += new System.EventHandler(this.TaskTabsRemoveMenuItem_Click);
			// 
			// TaskView
			// 
			this.TaskView.ContextMenu = this.TaskTabsContextMenu;
			this.TaskView.Controls.Add(this.TasksUsersTasksTreeView);
			this.TaskView.Location = new System.Drawing.Point(4, 22);
			this.TaskView.Name = "TaskView";
			this.TaskView.Size = new System.Drawing.Size(232, 210);
			this.TaskView.TabIndex = 0;
			this.TaskView.Text = "Tasks";
			// 
			// AssetView
			// 
			this.AssetView.Controls.Add(this.TaskRepositoryTreeView);
			this.AssetView.Location = new System.Drawing.Point(4, 22);
			this.AssetView.Name = "AssetView";
			this.AssetView.Size = new System.Drawing.Size(232, 210);
			this.AssetView.TabIndex = 1;
			this.AssetView.Text = "Assets";
			// 
			// TaskRepositoryTreeView
			// 
			this.TaskRepositoryTreeView.ArchivedNodeForeColor = System.Drawing.SystemColors.WindowText;
			this.TaskRepositoryTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TaskRepositoryTreeView.ExpandAssets = false;
			this.TaskRepositoryTreeView.ExpandPackageGroupAssets = false;
			this.TaskRepositoryTreeView.ExpandPackageGroups = false;
			this.TaskRepositoryTreeView.FocusForAssetNodes = MOG_ControlsLibrary.Controls.LeafFocusLevel.RepositoryItems;
			this.TaskRepositoryTreeView.HotTracking = true;
			this.TaskRepositoryTreeView.ImageIndex = 0;
			this.TaskRepositoryTreeView.Location = new System.Drawing.Point(0, 0);
			this.TaskRepositoryTreeView.Name = "TaskRepositoryTreeView";
			this.TaskRepositoryTreeView.PathSeparator = "~";
			this.TaskRepositoryTreeView.SelectedImageIndex = 0;
			this.TaskRepositoryTreeView.ShowAssets = false;
			this.TaskRepositoryTreeView.Size = new System.Drawing.Size(232, 210);
			this.TaskRepositoryTreeView.TabIndex = 0;
			// 
			// ProjectManagerTasksListView
			// 
			this.ProjectManagerTasksListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ProjectManagerTasksTitleColumnHeader,
            this.ProjectManagerTasksDepartmentColumnHeader,
            this.ProjectManagerTasksCreatorColumnHeader,
            this.ProjectManagerTasksStartDateColumnHeader,
            this.ProjectManagerTasksDueDateColumnHeader,
            this.ProjectManagerTasksStatusColumnHeader,
            this.ProjectManagerTasksPercentageColumnHeader,
            this.ProjectManagerTasksAssignedToColumnHeader,
            this.ProjectManagerTasksPriorityColumnHeader});
			this.ProjectManagerTasksListView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ProjectManagerTasksListView.Enabled = false;
			this.ProjectManagerTasksListView.FullRowSelect = true;
			this.ProjectManagerTasksListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.ProjectManagerTasksListView.Location = new System.Drawing.Point(0, 272);
			this.ProjectManagerTasksListView.MultiSelect = false;
			this.ProjectManagerTasksListView.Name = "ProjectManagerTasksListView";
			this.ProjectManagerTasksListView.Size = new System.Drawing.Size(240, 96);
			this.ProjectManagerTasksListView.SmallImageList = this.MOGTasksImageList;
			this.ProjectManagerTasksListView.TabIndex = 8;
			this.ProjectManagerTasksListView.UseCompatibleStateImageBehavior = false;
			this.ProjectManagerTasksListView.View = System.Windows.Forms.View.Details;
			this.ProjectManagerTasksListView.SelectedIndexChanged += new System.EventHandler(this.ProjectManagerTasksListView_SelectedIndexChanged);
			// 
			// ProjectManagerTasksTitleColumnHeader
			// 
			this.ProjectManagerTasksTitleColumnHeader.Text = "Title";
			this.ProjectManagerTasksTitleColumnHeader.Width = 0;
			// 
			// ProjectManagerTasksDepartmentColumnHeader
			// 
			this.ProjectManagerTasksDepartmentColumnHeader.Text = "Department";
			this.ProjectManagerTasksDepartmentColumnHeader.Width = 79;
			// 
			// ProjectManagerTasksCreatorColumnHeader
			// 
			this.ProjectManagerTasksCreatorColumnHeader.Text = "Creator";
			// 
			// ProjectManagerTasksStartDateColumnHeader
			// 
			this.ProjectManagerTasksStartDateColumnHeader.Text = "Start Date";
			// 
			// ProjectManagerTasksDueDateColumnHeader
			// 
			this.ProjectManagerTasksDueDateColumnHeader.Text = "DueDate";
			// 
			// ProjectManagerTasksStatusColumnHeader
			// 
			this.ProjectManagerTasksStatusColumnHeader.Text = "Status";
			// 
			// ProjectManagerTasksPercentageColumnHeader
			// 
			this.ProjectManagerTasksPercentageColumnHeader.Text = "Percent Complete";
			// 
			// ProjectManagerTasksAssignedToColumnHeader
			// 
			this.ProjectManagerTasksAssignedToColumnHeader.Text = "AssignedTo";
			// 
			// ProjectManagerTasksPriorityColumnHeader
			// 
			this.ProjectManagerTasksPriorityColumnHeader.Text = "Priority";
			// 
			// TaskHideCompleteCheckBox
			// 
			this.TaskHideCompleteCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.TaskHideCompleteCheckBox.Checked = true;
			this.TaskHideCompleteCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.TaskHideCompleteCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.TaskHideCompleteCheckBox.Location = new System.Drawing.Point(3, 5);
			this.TaskHideCompleteCheckBox.Name = "TaskHideCompleteCheckBox";
			this.TaskHideCompleteCheckBox.Size = new System.Drawing.Size(104, 24);
			this.TaskHideCompleteCheckBox.TabIndex = 6;
			this.TaskHideCompleteCheckBox.Text = "Hide complete";
			// 
			// TaskToolBar
			// 
			this.TaskToolBar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.TaskSortToolBarButton,
            this.toolBarButton1,
            this.TaskViewEditorToolBarButton,
            this.TaskViewListToolBarButton});
			this.TaskToolBar.ButtonSize = new System.Drawing.Size(16, 16);
			this.TaskToolBar.DropDownArrows = true;
			this.TaskToolBar.ImageList = this.ToolsImageList;
			this.TaskToolBar.Location = new System.Drawing.Point(0, 0);
			this.TaskToolBar.Name = "TaskToolBar";
			this.TaskToolBar.ShowToolTips = true;
			this.TaskToolBar.Size = new System.Drawing.Size(240, 28);
			this.TaskToolBar.TabIndex = 7;
			this.TaskToolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.TaskToolBar_ButtonClick);
			// 
			// TaskSortToolBarButton
			// 
			this.TaskSortToolBarButton.ImageIndex = 0;
			this.TaskSortToolBarButton.Name = "TaskSortToolBarButton";
			this.TaskSortToolBarButton.Pushed = true;
			this.TaskSortToolBarButton.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.TaskSortToolBarButton.Tag = "toggleSort";
			this.TaskSortToolBarButton.ToolTipText = "Toggle tree sort {Sorted}";
			// 
			// toolBarButton1
			// 
			this.toolBarButton1.Name = "toolBarButton1";
			this.toolBarButton1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// TaskViewEditorToolBarButton
			// 
			this.TaskViewEditorToolBarButton.ImageIndex = 1;
			this.TaskViewEditorToolBarButton.Name = "TaskViewEditorToolBarButton";
			this.TaskViewEditorToolBarButton.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.TaskViewEditorToolBarButton.Tag = "toggleeditor";
			this.TaskViewEditorToolBarButton.ToolTipText = "Toggle the inline task viewer window on and off";
			// 
			// TaskViewListToolBarButton
			// 
			this.TaskViewListToolBarButton.ImageIndex = 2;
			this.TaskViewListToolBarButton.Name = "TaskViewListToolBarButton";
			this.TaskViewListToolBarButton.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.TaskViewListToolBarButton.Tag = "togglelist";
			this.TaskViewListToolBarButton.ToolTipText = "Toggle the inline tasks list window on and off";
			// 
			// ToolsImageList
			// 
			this.ToolsImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ToolsImageList.ImageStream")));
			this.ToolsImageList.TransparentColor = System.Drawing.Color.Transparent;
			this.ToolsImageList.Images.SetKeyName(0, "");
			this.ToolsImageList.Images.SetKeyName(1, "");
			this.ToolsImageList.Images.SetKeyName(2, "");
			// 
			// TasksButtonPanel
			// 
			this.TasksButtonPanel.Controls.Add(this.TaskHideCompleteCheckBox);
			this.TasksButtonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.TasksButtonPanel.Location = new System.Drawing.Point(0, 552);
			this.TasksButtonPanel.Name = "TasksButtonPanel";
			this.TasksButtonPanel.Size = new System.Drawing.Size(240, 32);
			this.TasksButtonPanel.TabIndex = 9;
			// 
			// TaskEditorSplitter
			// 
			this.TaskEditorSplitter.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.TaskEditorSplitter.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.TaskEditorSplitter.Location = new System.Drawing.Point(0, 368);
			this.TaskEditorSplitter.Name = "TaskEditorSplitter";
			this.TaskEditorSplitter.Size = new System.Drawing.Size(240, 8);
			this.TaskEditorSplitter.TabIndex = 10;
			this.TaskEditorSplitter.TabStop = false;
			// 
			// TasksSplitter
			// 
			this.TasksSplitter.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.TasksSplitter.Dock = System.Windows.Forms.DockStyle.Top;
			this.TasksSplitter.Location = new System.Drawing.Point(0, 264);
			this.TasksSplitter.Name = "TasksSplitter";
			this.TasksSplitter.Size = new System.Drawing.Size(240, 8);
			this.TasksSplitter.TabIndex = 11;
			this.TasksSplitter.TabStop = false;
			// 
			// mogControl_TaskEditor
			// 
			this.mogControl_TaskEditor.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.mogControl_TaskEditor.EditableMode = false;
			this.mogControl_TaskEditor.Enabled = false;
			this.mogControl_TaskEditor.Location = new System.Drawing.Point(0, 376);
			this.mogControl_TaskEditor.Name = "mogControl_TaskEditor";
			this.mogControl_TaskEditor.Size = new System.Drawing.Size(240, 176);
			this.mogControl_TaskEditor.TabIndex = 11;
			// 
			// TaskWindow
			// 
			this.Controls.Add(this.ProjectManagerTasksListView);
			this.Controls.Add(this.TaskEditorSplitter);
			this.Controls.Add(this.mogControl_TaskEditor);
			this.Controls.Add(this.TasksSplitter);
			this.Controls.Add(this.TasksTabControl);
			this.Controls.Add(this.TaskToolBar);
			this.Controls.Add(this.TasksButtonPanel);
			this.Name = "TaskWindow";
			this.Size = new System.Drawing.Size(240, 584);
			this.DoubleClick += new System.EventHandler(this.TasksUsersTasksTreeView_DoubleClick);
			this.Load += new System.EventHandler(this.mogControl_TaskEditor_Load);
			this.TasksTabControl.ResumeLayout(false);
			this.TaskView.ResumeLayout(false);
			this.AssetView.ResumeLayout(false);
			this.TasksButtonPanel.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		#endregion

		#region Task related methods
		#region Initialization methods
		public void InitializeForUser()
		{
			db = MOG_ControllerSystem.GetDB();

			// Set our mode
			mMode = RunningMode.User;

			mImportanceSorted = true;

			InitializeToDoTree(this.TasksUsersTasksTreeView);
			TaskRepositoryTreeView.Initialize();

			// Currently we don't want users to be assigning tasks out by context menu?
			this.ToDoAssignToMenuItem.Enabled = false;
			this.ToDoAssignToDepartmentMenuItem.Enabled = false;
			//glk: Not tested, not sure if it's even necessary...
//			this.mAssetContextMenu = new MogControl_AssetContextMenu( "Inbox", this.TaskRepositoryTreeView );
//			this.TaskRepositoryTreeView.ContextMenu = this.mAssetContextMenu.InitializeContextMenu("{project}");
		}

		public void InitializeForProject()
		{
			db = MOG_ControllerSystem.GetDB();

			// Set our mode
			mMode = RunningMode.Project;

			mImportanceSorted = true;

			// Initialize the project tree view
			InitializeProjectTree(this.TasksUsersTasksTreeView);

			// Update our task list view
			TaskUpdateList(TasksUsersTasksTreeView.SelectedNode);

			// Init the context menu
			InitializeAssignUsers();
			InitializeAssignDepartments();
		}

		/// <summary>
		/// Inital ToDo Tree initializer. Passing it a tree, this function will populate with the current users tasks
		/// </summary>
		/// <param name="tree"></param>
		private void InitializeToDoTree(MOG_ColumnTreeView tree)
		{
			tree.Nodes.Clear();

			// Load the root task headers
			MOG_ColumnTreeNode task = new MOG_ColumnTreeNode();
			task.Text = MOG_ControllerProject.GetUser().GetUserName() + "'s Tasks";
			task.Tag = new TaskNode(TaskType.User, MOG_ControllerProject.GetUser().GetUserName());

			// Load current users tasks
			LoadUserTasks(task, MOG_ControllerProject.GetUser().GetUserName());

			// Add to the tree root
			tree.Nodes.Add(task);

			// Default open of first generation of tasks
			task.Expand();
		}

		/// <summary>
		/// Inital project tasks Tree initializer. Passing it a tree, this function will populate with the current project tasks
		/// </summary>
		/// <param name="tree"></param>
		public void InitializeProjectTree(MOG_ColumnTreeView tree)
		{
			tree.Nodes.Clear();

			// Load the root task headers
			MOG_ColumnTreeNode project = new MOG_ColumnTreeNode();
			project.Text = MOG_ControllerProject.GetProjectName();
			project.Tag = new TaskNode(TaskType.Project, MOG_ControllerProject.GetProjectName());

			// Load the current departments
			LoadDepartments(tree, project);

			// Default open of first generation of tasks
			project.Expand();
		}
		
		/// <summary>
		/// Populate a tree from a reference task node
		/// </summary>
		/// <param name="tree"></param>
		/// <param name="TopTask"></param>
		public void InitializeTaskTree(MOG_ColumnTreeView tree, TaskNode TopTask)
		{
			tree.Nodes.Clear();

			// Load the root task headers
			MOG_ColumnTreeNode topNode = new MOG_ColumnTreeNode();
			topNode.Text = TopTask.GetName();
			topNode.Tag = TopTask;
			topNode.ImageIndex = TopTask.GetImage();

			switch(TopTask.GetTaskType())
			{
				case TaskType.User:
					InitializeToDoTree(tree);
					return;
				case TaskType.Task:
					// Load current users tasks
					LoadTaskChildren(topNode, TopTask.GetTaskInfo().GetTaskID());
					break;
			}
			

			// Add to the tree root
			tree.Nodes.Add(topNode);

			// Default open of first generation of tasks
			topNode.Expand();
		}
		#endregion

		#region Task Utility methods
		/// <summary>
		/// Polls database for all children of a specified task to determine if they are all complete
		/// </summary>
		/// <param name="taskNode"></param>
		/// <returns></returns>
		private bool ChildrenTaskAreComplete(MOG_ColumnTreeNode taskNode)
		{
			if (taskNode.Tag != null &&
				((TaskNode)taskNode.Tag).GetTaskInfo() != null)
			{				
				ArrayList tasks = MOG_DBTaskAPI.GetChildrenTasks(((TaskNode)taskNode.Tag).GetTaskInfo().GetTaskID());

				if (tasks != null)
				{
					foreach (MOG_TaskInfo taskHandle in tasks)
					{
						if (taskHandle.GetPercentComplete() < 100)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		/// <summary>
		/// Calculate the importance of this task by taking into account its children, priority, and due dates
		/// </summary>
		/// <param name="taskParent"></param>
		private int CalculateImportanceRating(MOG_TaskInfo taskParent)
		{
			ArrayList tasks = MOG_DBTaskAPI.GetChildrenTasks(taskParent.GetTaskID());

			// Check for children
			if (tasks != null)
			{
				foreach (MOG_TaskInfo taskHandle in tasks)
				{
					// Bump rating dur to dependent tasks
					if (taskHandle.GetPercentComplete() != 100)
					{
						taskParent.IncrementImportanceRating(1);					
						taskParent.IncrementImportanceRating(CalculateImportanceRating(taskHandle));
					}
				}
			}
			
			if (taskParent.GetPercentComplete() != 100)
			{
				// Add in priority
				switch(taskParent.GetPriority())
				{
					case "Critical":
						taskParent.IncrementImportanceRating(3);
						break;
					case "High":
						taskParent.IncrementImportanceRating(2);
						break;
					case "Medium":
						taskParent.IncrementImportanceRating(1);
						break;
					case "Low":
						taskParent.IncrementImportanceRating(-1);
						break;
				}

				// Add in due date
				MOG_Time current = new MOG_Time();
				if (current.Compare(new MOG_Time(taskParent.GetDueDate())) > 0)
				{
					taskParent.IncrementImportanceRating(5);
				}
			}

			return taskParent.GetImportanceRating();
		}

		private int ChildrenCalulatePercent(MOG_TaskInfo taskParent, ref int totalPercentage, ref int numberOfChildren)
		{
							
			ArrayList tasks = MOG_DBTaskAPI.GetChildrenTasks(taskParent.GetTaskID());

			if (tasks != null)
			{
				foreach (MOG_TaskInfo taskHandle in tasks)
				{
					numberOfChildren++;
					ChildrenCalulatePercent(taskHandle, ref totalPercentage, ref numberOfChildren);
				}
			}
			
			totalPercentage += (taskParent.GetPercentComplete() / numberOfChildren);

			return totalPercentage;
		}

		private void ChildrenTaskInheritParentUpdate(MOG_ColumnTreeNode parent)
		{
			if (parent.Tag != null &&
				((TaskNode)parent.Tag).GetTaskInfo() != null)
			{
				TaskNode parentTaskInfo = (TaskNode)parent.Tag;
				ArrayList tasks = MOG_DBTaskAPI.GetChildrenTasks(parentTaskInfo.GetTaskInfo().GetTaskID());

				if (tasks != null)
				{
					foreach (MOG_TaskInfo taskHandle in tasks)
					{
						MOG_TaskInfo original = taskHandle.Clone();
						taskHandle.SetPriority(parentTaskInfo.GetTaskInfo().GetPriority());
						taskHandle.SetComment(taskHandle.GetComment() + "\n" + taskHandle.GetChangesAsComment(original));

						MOG_DBTaskAPI.UpdateTask(taskHandle);
					}
				}
			}
		}
		#endregion

		#region Task population methods
		/// <summary>
		/// Create a generic tree node based on a MOG_TaskInfo structure
		/// </summary>
		/// <param name="task"></param>
		/// <returns></returns>
		private MOG_ColumnTreeNode TaskCreateNode(string name, TaskType type)
		{
			MOG_ColumnTreeNode cnode = new MOG_ColumnTreeNode(name);

			// Create gui task container
			TaskNode taskHandle = new TaskNode(type, name, null);
			cnode.Tag = taskHandle;

			// Add the subItems
			//			cnode.SubItems.Add(taskHandle.GetTaskInfo().GetPercentComplete().ToString());
			//			cnode.SubItems.Add(taskHandle.GetTaskInfo().GetCompletionDate().ToString());
			
			// Set custom font
			//			cnode.Font = taskHandle.GetNodeFont(TasksUsersTasksTreeView);
			cnode.NodeFont = taskHandle.GetNodeFont(TasksUsersTasksTreeView);
			
			// Set the checkbox
			cnode.Checked = (taskHandle.GetChecked() == CheckState.Checked)? true : false;

			// Set the task image
			cnode.ImageIndex = taskHandle.GetImage();
			
			return cnode;
		}

		/// <summary>
		/// Create a tree node based on a MOG_TaskInfo structure
		/// </summary>
		/// <param name="task"></param>
		/// <returns></returns>
		private MOG_ColumnTreeNode TaskCreateTaskNode(MOG_TaskInfo task)
		{
			MOG_ColumnTreeNode cnode = new MOG_ColumnTreeNode();

			// Create gui task container
			TaskNode taskHandle = new TaskNode(TaskType.Task, task.GetName(), task);
			cnode.Tag = taskHandle;

			// Set the name
			int percent = 0;
			int num = 1;
			cnode.Text = taskHandle.GetName() + "(" + ChildrenCalulatePercent(taskHandle.GetTaskInfo(), ref percent, ref num) + "%)";
			
			// Add the subItems
			//			cnode.SubItems.Add(taskHandle.GetTaskInfo().GetPercentComplete().ToString());
			//			cnode.SubItems.Add(taskHandle.GetTaskInfo().GetCompletionDate().ToString());
			
			// Set custom font
//			cnode.Font = taskHandle.GetNodeFont(TasksUsersTasksTreeView);
			cnode.NodeFont = taskHandle.GetNodeFont(TasksUsersTasksTreeView);
			cnode.ForeColor = taskHandle.GetNodeFontColor(TasksUsersTasksTreeView);
			
			// Set the checkbox
//			cnode.SubItems[0].Checked = taskHandle.GetChecked();
			cnode.Checked = (taskHandle.GetChecked() == CheckState.Checked)? true : false;

			// Set the task image
			cnode.ImageIndex = taskHandle.GetImage();
			
			return cnode;
		}

		/// <summary>
		/// Add the newly changed or edited task into the tree at the correct spot
		/// </summary>
		/// <param name="nodes"></param>
		/// <param name="newTask"></param>
		/// <returns></returns>
		private bool AddTaskToTree(TreeNodeCollection nodes, TaskNode newTask)
		{
			// Loop through all the visible nodes of the task tree 
			foreach(MOG_ColumnTreeNode node in nodes)
			{
				if (node.Tag != null)
				{
					TaskNode handle = (TaskNode)node.Tag;
						
					switch(handle.GetTaskType())
					{
						case TaskType.Project:
							if (AddTaskToTree(node.Nodes, newTask))
							{
								return true;
							}
							break;
							// Locate the correct department first
						case TaskType.Department:
							if (string.Compare(node.Text, newTask.GetTaskInfo().GetDepartment(), true) == 0)
							{
								// Now determine if this is a task assigned to a department only or to a user within a department
								if (newTask.GetTaskInfo().GetAssignedTo().Length != 0)
								{
									// Ok, search for this user in the current department
									if (AddTaskToTree(node.Nodes, newTask))
									{
										return true;
									}
								}
								else
								{
									// Add the department task
									node.Nodes.Add(TaskCreateTaskNode(newTask.GetTaskInfo()));
									//CreateTreeNode(newTask, node);
									return true;
								}
							}
							break;
		
						case TaskType.User:
							if (string.Compare(node.Text, newTask.GetTaskInfo().GetAssignedTo(), true) == 0)
							{
								node.Nodes.Add(TaskCreateTaskNode(newTask.GetTaskInfo()));
								//CreateTreeNode(newTask, node);
								return true;
							}
							break;
		
						case TaskType.Task:
							node.Nodes.Add(TaskCreateTaskNode(newTask.GetTaskInfo()));
							//CreateTreeNode(newTask, node);
							return true;
					}
				}
			}
		
			return false;
		}

		/// <summary>
		/// Load all the departments into the tree
		/// </summary>
		/// <param name="tree"></param>
		/// <param name="project"></param>
		private void LoadDepartments(MOG_ColumnTreeView tree, MOG_ColumnTreeNode project)
		{
			ArrayList departments = MOG_DBProjectAPI.GetDepartments();

			if (departments != null)
			{
				foreach (string department in departments)
				{
					MOG_ColumnTreeNode node = TaskCreateNode(department, TaskType.Department);
					
					// Check for children
					if (MOG_DBProjectAPI.GetDepartmentUsers(department).Count > 0)
					{
						node.Nodes.Add("BLANK");
					}
					project.Nodes.Add(node);		
				}
			}

			tree.Nodes.Add(project);
//			project.Expand();
		}

		/// <summary>
		/// Load all the users assigned to a department
		/// </summary>
		/// <param name="node"></param>
		/// <param name="departmentId"></param>
		/// <returns></returns>
		private bool LoadUsers(MOG_ColumnTreeNode node, string departmentId)
		{
			ArrayList users = MOG_DBProjectAPI.GetDepartmentUsers(departmentId);

			// Cleanup the tree
			if ((node.Nodes.Count == 1) && (node.Nodes[0].Text == "BLANK"))
			{
				node.Nodes.Clear();
			}

			foreach (string user in users)
			{
				TreeNode cnode = TaskCreateNode(user, TaskType.User);
				
				// Check for tasks assigned to this user
				ArrayList test = MOG_DBTaskAPI.GetUserTasks(user);
				if (test != null && test.Count != 0)
				{
					// If we find some, add a dummy blank
					cnode.Nodes.Add("BLANK");
				}
				node.Nodes.Add(cnode);
			}

			return (node.Nodes.Count > 0);
		}

		/// <summary>
		/// Load all the tasks assigned to this user
		/// </summary>
		/// <returns></returns>
		private bool LoadUserTasks(MOG_ColumnTreeNode parent, string userName)
		{
			// Make sure we have a valid user logged in
			if (MOG_ControllerProject.GetUser() == null ||
				MOG_ControllerProject.GetUser().GetUserName().Length == 0)
			{
				return false;
			}

			// Fetch the tasks from the database
			ArrayList tasks = MOG_DBTaskAPI.GetUserTasks(userName);

			// Cleanup the tree
			parent.Nodes.Clear();

			// Create the task nodes for this user
			if (tasks != null)
			{
				foreach (MOG_TaskInfo taskHandle in tasks)
				{	
					// Only create the node if it is either less than 100% or we are not set to hide completed tasks					
					if (taskHandle.GetPercentComplete() != 100 ||
						!TaskHideCompleteCheckBox.Checked)
					{
						// Create the tree node
						MOG_ColumnTreeNode taskNode = TaskCreateTaskNode(taskHandle);

						// Check for any children tasks
						if (MOG_DBTaskAPI.GetChildrenTasks(taskHandle.GetTaskID()).Count > 0)
						{
							// If we find some, add a dummy blank
							taskNode.Nodes.Add("BLANK");
							taskNode.Collapse();
						}

						// Calculate the order of task insertion
						if (mImportanceSorted) 
						{
							AddToTreeSorted(parent, taskNode);
						}
						else
						{
							parent.Nodes.Add(taskNode);
						}
					}
				}
			}

			return (parent.Nodes.Count > 0);
		}

		/// <summary>
		/// Load the tasks not assigned to a user but to a department
		/// </summary>
		/// <param name="node"></param>
		/// <param name="departmentId"></param>
		/// <returns></returns>
		private bool LoadFloatingTasks(MOG_ColumnTreeNode node, string departmentId)
		{
			ArrayList tasks = MOG_DBTaskAPI.GetDepartmentTasks(departmentId);;

			// Cleanup the tree
			node.Nodes.Clear();

			if (tasks != null)
			{
				foreach (MOG_TaskInfo task in tasks)
				{
					TreeNode cnode = TaskCreateTaskNode(task);
					
					// Check for children
					if (MOG_DBTaskAPI.GetChildrenTasks(task.GetTaskID()).Count > 0)
					{
						// If we find some, add a dummy blank
						cnode.Nodes.Add("BLANK");
					}
					node.Nodes.Add(cnode);
				}
			}

			return (node.Nodes.Count > 0);
		}

		/// <summary>
		/// Load all the tasks that are sub-tasks of this task
		/// </summary>
		/// <param name="node"></param>
		/// <param name="taskId"></param>
		/// <returns></returns>
		private bool LoadTaskChildren(MOG_ColumnTreeNode node, int taskId)
		{
			ArrayList tasks = MOG_DBTaskAPI.GetChildrenTasks(taskId);

			// Cleanup the tree
			node.Nodes.Clear();

			if (tasks != null)
			{
				foreach (MOG_TaskInfo taskHandle in tasks)
				{
					// Only create the node if it is either less than 100% or we are not set to hide completed tasks
					if (taskHandle.GetPercentComplete() != 100 ||
						!TaskHideCompleteCheckBox.Checked)
					{
						// Create the tree node
						MOG_ColumnTreeNode taskNode = TaskCreateTaskNode(taskHandle);

						// Check for children
						if (MOG_DBTaskAPI.GetChildrenTasks(taskHandle.GetTaskID()).Count > 0)
						{
							// If we find some, add a dummy blank
							taskNode.Nodes.Add("BLANK");
							taskNode.Collapse();
						}

						// Add to the tree either sorted or unsorted
						if (mImportanceSorted) 
						{
							AddToTreeSorted(node, taskNode);
						}
						else
						{
							node.Nodes.Add(taskNode);
						}
					}
				}
			}

			return (node.Nodes.Count > 0);
		}

		/// <summary>
		/// Add a child node to the parent's node according to its importance level
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="child"></param>
		/// <returns></returns>
		private bool AddToTreeSorted(MOG_ColumnTreeNode parent, MOG_ColumnTreeNode child)
		{
			try
			{
				MOG_TaskInfo ChildTaskHandle = ((TaskNode)child.Tag).GetTaskInfo();

				// Calculate the order of task insertion
				CalculateImportanceRating(ChildTaskHandle);

				if (parent.Nodes.Count > 0)
				{
					foreach (MOG_ColumnTreeNode node in parent.Nodes)
					{
						MOG_TaskInfo siblingTaskHandle = ((TaskNode)node.Tag).GetTaskInfo();
						if (ChildTaskHandle.GetImportanceRating() >= siblingTaskHandle.GetImportanceRating())
						{
							parent.Nodes.Insert(node.Index, child);
							return true;
						}						
					}
				}
				else
				{
					parent.Nodes.Add(child);
					return true;
				}
			}
			catch{}
			
			parent.Nodes.Add(child);
			return true;
		}

		/// <summary>
		/// Prepopulate the task branch before expanding
		/// </summary>
		/// <param name="parent"></param>
		public void TaskExpandTree(MOG_ColumnTreeNode parent)
		{
			if (parent.Tag != null)
			{
				TaskNode taskHandle = (TaskNode)parent.Tag;
				switch(taskHandle.GetTaskType())
				{
					case TaskType.Project:
						break;
					case TaskType.Department:
						if (parent.Nodes.Count <= 1)
						{
							// Load tasks that have no users
							LoadFloatingTasks(parent, parent.Text);
							
							LoadUsers(parent, parent.Text);
						}
						break;
					case TaskType.User:
						if (parent.Nodes.Count <= 1)
						{
							if (!LoadUserTasks(parent, taskHandle.GetName()))
							{
								parent.Nodes.Clear();
							}
						}
						break;
					case TaskType.Task:
						if (parent.Nodes.Count <= 1)
						{							
							if (!LoadTaskChildren(parent, taskHandle.GetTaskInfo().GetTaskID()))
							{
								parent.Nodes.Clear();
							}
						}
						break;
				}
			}
		}
		#endregion

		#region Task - Edit / Create / Update / Delete
		/// <summary>
		/// Show the create dialog and create a new task node and database entry
		/// </summary>
		/// <param name="parent"></param>
		public void TaskCreateNew(MOG_ColumnTreeNode parent, string userName)
		{
			// Create my task data structures
			MOG_TaskInfo taskHandle = new MOG_TaskInfo();

			// Get parent task inheritance if this is a child task
			if (parent.Tag != null &&
				((TaskNode)parent.Tag).GetTaskType() == TaskType.Task)
			{
				taskHandle = new MOG_TaskInfo(((TaskNode)parent.Tag).GetTaskInfo());
			}
			
			// Set some defaults for this task before opening the dialog
			taskHandle.SetAssignedTo(userName);
			taskHandle.SetCreator(MOG_ControllerProject.GetUser().GetUserName());

			// Set default name
			taskHandle.SetName("New task");

			// Open the new task dialog
			NewTaskForm taskForm = new NewTaskForm(taskHandle);
			if (taskForm.ShowDialog() == DialogResult.OK)
			{
				// Get all the properties of the newly setup task info from our create task form
				taskHandle.SetBranch(MOG_ControllerProject.GetBranchName());
				taskHandle.SetDepartment(taskForm.MOGTaskInfo.GetDepartment());
				taskHandle.SetPriority(taskForm.MOGTaskInfo.GetPriority());
				taskHandle.SetAssignedTo(taskForm.MOGTaskInfo.GetAssignedTo());
				taskHandle.SetDueDate(taskForm.MOGTaskInfo.GetDueDate());
				taskHandle.SetCompletionDate(taskForm.MOGTaskInfo.GetCompletionDate());
				taskHandle.SetComment(taskForm.MOGTaskInfo.GetComment());
//				taskHandle.SetAsset(taskForm.MOGTaskInfo.GetAsset());
				taskHandle.SetStatus(taskForm.MOGTaskInfo.GetStatus());
				
				taskHandle.SetCreationDate(MOG_Time.GetVersionTimestamp());
				taskHandle.SetCreator(taskForm.MOGTaskInfo.GetCreator());
				
				// Add the task to the database
				MOG_TaskInfo newTask = MOG_DBTaskAPI.CreateTask(taskHandle);
				
				// If the add was successfull
				if (newTask != null)
				{
					// Check to see if we need to add a parent child link in the database
					if (parent.Tag != null &&
						((TaskNode)parent.Tag).GetTaskType() == TaskType.Task)
					{
						// Ipdate the LINKS database
						MOG_DBTaskAPI.SetParentTaskID(newTask, Convert.ToInt32(((TaskNode)parent.Tag).GetTaskInfo().GetTaskID()));
					}

					// Create the tree node
					MOG_ColumnTreeNode newTaskTreeNode = TaskCreateTaskNode(newTask);
					
					// Add the node
					parent.Nodes.Add(newTaskTreeNode);
					parent.Expand();
				}
			}
		}

		/// <summary>
		/// Open the task in the edit task dialog and allow for changes to be made
		/// </summary>
		public void TaskEdit(MOG_ColumnTreeView tree)
		{
			//foreach (MOG_ColumnTreeNode node in tree.SelectedNodes)
			if (tree != null && tree.SelectedNode != null)
			{
				MOG_ColumnTreeNode node = tree.SelectedNode;
				if (node != null)
				{
					TaskNode guiTaskHandle = (TaskNode)node.Tag;
					if (guiTaskHandle.GetTaskInfo() != null)
					{						
						CalculateImportanceRating(guiTaskHandle.GetTaskInfo());
						NewTaskForm taskForm = new NewTaskForm(guiTaskHandle.GetTaskInfo());

						// Is this our own task? Or are we allowed to edit another users task?
						if (string.Compare(MOG_ControllerProject.GetUser().GetUserName(), guiTaskHandle.GetTaskInfo().GetCreator(), true) == 0 ||
							MOG_ControllerProject.GetPrivileges().GetUserPrivilege( MOG_ControllerProject.GetUser().GetUserName(), MOG_PRIVILEGE.EditOtherUsersTask))
						{
							taskForm.EditableMode = true;
						}
						else
						{
							taskForm.EditableMode = false;
						}
							
						// Open the task in the dialog
						if (taskForm.ShowDialog() == DialogResult.OK)
						{
							bool complete = false;

							// Update the database
							complete = MOG_DBTaskAPI.UpdateTask(taskForm.MOGTaskInfo);

							// If the database accepted our change, update the tree node								
							if (complete)
							{
								// Flush the old and create a new guiTaskhandle
								guiTaskHandle = new TaskNode(TaskType.Task, taskForm.MOGTaskInfo.GetName(), taskForm.MOGTaskInfo);
								node.Tag = guiTaskHandle;

								// Update the nodes visuals
								node.Text = guiTaskHandle.GetExtendedName(guiTaskHandle.GetTaskInfo().GetAssignedTo());

								// Update children tasks
								ChildrenTaskInheritParentUpdate(node);
							}
						}
					}					
				}
			}
		}

		/// <summary>
		/// This method allows us to set a task as complete to a set percentage without editing
		/// </summary>
		/// <param name="task"></param>
		/// <param name="percentComplete"></param>
		/// <returns></returns>
		public bool TaskUpdatePercentComplete(MOG_ColumnTreeNode taskNode, int percentComplete)
		{
			// Only allow the user to delete tasks with no children
			if (taskNode.Nodes.Count == 0 ||
				(!TaskHideCompleteCheckBox.Checked) && ChildrenTaskAreComplete(taskNode) ||
				percentComplete < 100)
			{
				if (taskNode.Tag != null)
				{
					TaskNode guiHandle = (TaskNode)taskNode.Tag;

					if (guiHandle.GetTaskInfo() != null)
					{
						MOG_TaskInfo handle = guiHandle.GetTaskInfo();
						MOG_TaskInfo original = handle.Clone();
						if (handle != null)
						{
							// Make sure we are the user assigned to this task before editing
							if (string.Compare(handle.GetAssignedTo(), MOG_ControllerProject.GetUser().GetUserName(), true) == 0)
							{
								handle.SetPercentComplete(percentComplete);
								handle.SetComment(handle.GetComment() + "\n" + handle.GetChangesAsComment(original));
								if (MOG_DBTaskAPI.UpdateTask(handle))
								{
									// Update the title
									taskNode.Text = guiHandle.GetExtendedName(MOG_ControllerProject.GetUser().GetUserName());
									taskNode.NodeFont = guiHandle.GetNodeFont(taskNode.TreeView);
						
									// Remove the node if we have checked 'Hide completed'
									if (handle.GetPercentComplete() >= 100 &&
										TaskHideCompleteCheckBox.Checked)
									{
										taskNode.Remove();
									}
									return true;
								}
							}
							else
							{
								MOG_Prompt.PromptMessage("Update Task", "You cannot edit another users ToDo", Environment.StackTrace);
								return false;
							}
						}
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Remove a task from the tree and also from the database
		/// </summary>
		/// <param name="task"></param>
		public void TaskDelete(MOG_ColumnTreeNode taskNode)
		{
			// Only allow the user to delete tasks with no children
			if (taskNode.Nodes.Count == 0)
			{
				// Make sure we have a valid database object
				if (taskNode.Tag != null)
				{
					TaskNode guiHandle = (TaskNode)taskNode.Tag;
					MOG_TaskInfo handle = guiHandle.GetTaskInfo();
					if (handle != null)
					{
						// Only allow us to delete tasks that are both assigned to us and that we have created
						if (string.Compare(handle.GetCreator(), MOG_ControllerProject.GetUser().GetUserName(), true) == 0 || 
							MOG_ControllerProject.GetPrivileges().GetUserPrivilege( MOG_ControllerProject.GetUser().GetUserName(), MOG_PRIVILEGE.DeleteOtherUsersTask))
						{
							// Attempt to remove the task from the database task table
							if (MOG_DBTaskAPI.RemoveTask(handle.GetTaskID()))
							{
								// We should now attempt to remove any parent child links

								// Now remove the tree node
								taskNode.Remove();
							}
						}
						else
						{
							MOG_Prompt.PromptMessage("Delete Task", "You cannot delete another users ToDo", Environment.StackTrace);
						}
					}
				}
			}
			else
			{
				MOG_Prompt.PromptMessage("Delete ToDo", "Cannot delete a parent ToDo. Try deleting all the children of this ToDo first", Environment.StackTrace);
			}
		}
		#endregion

		#endregion

		#region Context menus
				// ******************************************************
		private void InitializeAssignUsers()
		{
			this.ToDoAssignToMenuItem.MenuItems.Clear();

			ArrayList departments = MOG_DBProjectAPI.GetDepartments();
		
			if (departments != null)
			{
				foreach (string departmentId in departments)
				{
					ArrayList users = MOG_DBProjectAPI.GetDepartmentUsers(departmentId);
										
					if (users != null)
					{
						foreach (string user in users)
						{
							MenuItem Item = new MenuItem(user);
							Item.Click += new System.EventHandler(this.ToDoAssignUserMenuItem_Click);
							this.ToDoAssignToMenuItem.MenuItems.Add(Item);
						}
					}
				}
			}			
		}
		
		// ******************************************************
		private void InitializeAssignDepartments()
		{
			this.ToDoAssignToDepartmentMenuItem.MenuItems.Clear();

			// Add all the users to the Assign subMenu
			ArrayList departments = MOG_DBProjectAPI.GetDepartments();
						
			if (departments != null)
			{
				foreach (string department in departments)
				{
					MenuItem Item = new MenuItem(department);
					Item.Click += new System.EventHandler(this.TodoAssignDepartmentMenuItem_Click);
					this.ToDoAssignToDepartmentMenuItem.MenuItems.Add(Item);
				}
			}						
		}
		        				
		#region ToDo Context menu click handle's
		private void ToDoNewMenuItem_Click(object sender, System.EventArgs e)
		{
			try
			{
				Point pt = PointToClient(Cursor.Position);
				MOG_ColumnTreeView targetTree = (MOG_ColumnTreeView)TasksTabControl.SelectedTab.GetChildAtPoint(pt);

				//foreach (MOG_ColumnTreeNode node in TasksUsersTasksTreeView.SelectedNodes)
				if (targetTree != null && targetTree.SelectedNode != null)
				{
					MOG_ColumnTreeNode node = targetTree.SelectedNode;

					switch (mMode)
					{
						case RunningMode.User:
							TaskCreateNew(node, MOG_ControllerProject.GetUser().GetUserName());
							break;
						case RunningMode.Project:
							try
							{
								TaskNode taskNode = (TaskNode)node.Tag;
								switch(taskNode.GetTaskType())
								{
									case TaskType.Department:
										TaskCreateNew(node, taskNode.GetName());
										break;
									case TaskType.User:
										TaskCreateNew(node, taskNode.GetName());
										break;
									case TaskType.Task:
										TaskCreateNew(node, taskNode.GetTaskInfo().GetAssignedTo());
										break;
								}
							}
							catch
							{
							}
							break;
					}
				}			
			}
			catch{}
		}

		private void TodoAssignDepartmentMenuItem_Click(object sender, System.EventArgs e)
		{
			// Get our target TreeView
			Point pt = PointToClient(Cursor.Position);
			MOG_ColumnTreeView targetTree = (MOG_ColumnTreeView)TasksTabControl.SelectedTab.GetChildAtPoint(pt);

			if (targetTree != null && targetTree.SelectedNode != null)
			{
				// Get the selected node
				MOG_ColumnTreeNode node = targetTree.SelectedNode;
				TaskNode taskHandle = (TaskNode)node.Tag;
                MOG_TaskInfo original = taskHandle.GetTaskInfo().Clone();
		
				// Make sure we have a valid node and valid task
				if (node != null && taskHandle.GetTaskType() == TaskType.Task)
				{
					// Get the specified user
					MenuItem DepartmentName = (System.Windows.Forms.MenuItem) sender;
		
					// Update the database
					taskHandle.GetTaskInfo().SetAssignedTo("");
					taskHandle.GetTaskInfo().SetDepartment(DepartmentName.Text);
					taskHandle.GetTaskInfo().SetComment(taskHandle.GetTaskInfo().GetComment() + "\n" + taskHandle.GetTaskInfo().GetChangesAsComment(original));
		
					MOG_DBTaskAPI.UpdateTask(taskHandle.GetTaskInfo());
		
					// Update the tree
					node.Remove();
					AddTaskToTree(targetTree.Nodes, taskHandle);
				}
			}
		}
		
		private void ToDoAssignUserMenuItem_Click(object sender, System.EventArgs e)
		{
			try
			{
				// Get our target TreeView
				Point pt = PointToClient(Cursor.Position);
				MOG_ColumnTreeView targetTree = (MOG_ColumnTreeView)TasksTabControl.SelectedTab.GetChildAtPoint(pt);

				if (targetTree != null)
				{
					// Get the selected node
					MOG_ColumnTreeNode node = targetTree.SelectedNode;
					TaskNode taskHandle = (TaskNode)node.Tag;
					MOG_TaskInfo original = taskHandle.GetTaskInfo().Clone();
		
					// Make sure the node is valid along with its task
					if (node != null && taskHandle.GetTaskType() == TaskType.Task)
					{
						// Get the specified user
						MenuItem userName = (System.Windows.Forms.MenuItem) sender;
						MOG_User user = MOG_DBProjectAPI.GetUser(userName.Text);
						if (user != null)
						{
							// Ipdate the database
							taskHandle.GetTaskInfo().SetAssignedTo(user.GetUserName());
							taskHandle.GetTaskInfo().SetDepartment(user.GetUserDepartment());
							taskHandle.GetTaskInfo().SetComment(taskHandle.GetTaskInfo().GetComment() + "\n" + taskHandle.GetTaskInfo().GetChangesAsComment(original));
		
							MOG_DBTaskAPI.UpdateTask(taskHandle.GetTaskInfo());
		
							// Update the tree
							node.Remove();
							AddTaskToTree(targetTree.Nodes, taskHandle);
						}
					}
				}
			}
			catch (Exception ex)
			{
				MOG_Report.ReportMessage("Assign to user", ex.Message, ex.StackTrace, MOG_ALERT_LEVEL.ERROR);
			}
		}

		private void ToDoEditMenuItem_Click(object sender, System.EventArgs e)
		{
			try
			{
				Point pt = PointToClient(Cursor.Position);
				MOG_ColumnTreeView targetTree = (MOG_ColumnTreeView)TasksTabControl.SelectedTab.GetChildAtPoint(pt);
				TaskEdit(targetTree);				
			}
			catch{}
		}

		private void ToDoDeleteMenuItem_Click(object sender, System.EventArgs e)
		{
			try
			{
				Point pt = PointToClient(Cursor.Position);
				MOG_ColumnTreeView targetTree = (MOG_ColumnTreeView)TasksTabControl.SelectedTab.GetChildAtPoint(pt);
				
				//foreach (MOG_ColumnTreeNode node in TasksUsersTasksTreeView.SelectedNodes)
				if (targetTree != null && TasksUsersTasksTreeView.SelectedNode != null)
				{
					MOG_ColumnTreeNode node = targetTree.SelectedNode;

					TaskDelete(node);
				}
			}
			catch{}
		}

		/// <summary>
		/// Create a custom tab for the selected task node
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ToDoCreateTabMenuItem_Click(object sender, System.EventArgs e)
		{
			try
			{
				Point pt = PointToClient(Cursor.Position);
				MOG_ColumnTreeView targetTree = (MOG_ColumnTreeView)TasksTabControl.SelectedTab.GetChildAtPoint(pt);
				
				if (targetTree != null && targetTree.SelectedNode != null)
				{
					MOG_ColumnTreeNode node = targetTree.SelectedNode;
					TaskNode taskHandle = (TaskNode)node.Tag;

					if (taskHandle.GetTaskType() == TaskType.Task)
					{
						// Create the tab
						TabPage customTab = new TabPage(taskHandle.GetName());
						customTab.ContextMenu = this.TaskTabsContextMenu;

						// Create the tree within the tab
						MOG_ColumnTreeView taskTree = new MOG_ColumnTreeView();
				
						#region Set default tree values
						taskTree.AllowDrop = true;
						taskTree.CheckBoxes = true;
						taskTree.ContextMenu = this.TodoContextMenu;
						taskTree.Dock = System.Windows.Forms.DockStyle.Fill;
						taskTree.FullRowSelect = true;
						taskTree.HotTracking = true;
						taskTree.ImageList = this.MOGTasksImageList;
						taskTree.Indent = 1;
						taskTree.ItemHeight = 15;
						taskTree.Location = new System.Drawing.Point(0, 0);
						taskTree.Size = new System.Drawing.Size(240, 254);
						taskTree.TabIndex = 4;
						taskTree.Tag = taskHandle;
						taskTree.DragOver += new System.Windows.Forms.DragEventHandler(this.TasksUsersTasksTreeView_DragOver);
						taskTree.DoubleClick += new System.EventHandler(this.TasksUsersTasksTreeView_DoubleClick);
						taskTree.BeforeCheck += new System.Windows.Forms.TreeViewCancelEventHandler(this.TasksUsersTasksTreeView_BeforeCheck);
						taskTree.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.TasksUsersTasksTreeView_BeforeExpand);
						taskTree.DragDrop += new System.Windows.Forms.DragEventHandler(this.TasksUsersTasksTreeView_DragDrop);
						#endregion

						// Hook up all the controls
						customTab.Controls.Add(taskTree);
						TasksTabControl.Controls.Add(customTab);
					}
				}
			}
			catch{}
		}
		#endregion
		#endregion

		#region ToDo tree click events
		private void TasksUsersTasksTreeView_DoubleClick(object sender, System.EventArgs e)
		{
			TaskEdit(TasksUsersTasksTreeView);
		}

		private void TasksUsersTasksTreeView_BeforeCheck(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		{
			if (e.Node.Checked != true)
			{				
				if (!TaskUpdatePercentComplete(e.Node, 100))
				{
					e.Cancel = true;
				}
				else
				{
					e.Cancel = false;
				}				
			}
			else
			{				
				if (!TaskUpdatePercentComplete(e.Node, 0))
				{
					e.Cancel = true;
				}
				else
				{
					e.Cancel = false;
				}				
			}		
		}

		private void TasksUsersTasksTreeView_BeforeExpand(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		{
			TaskExpandTree(e.Node);	
		}
		#endregion

		#region ListView Population functions
		/// <summary>
		/// Update the listView of all tasks within the range specified by the tree view
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="level"></param>
		private void UpdateTaskListWalkChildren(MOG_ColumnTreeNode parent, int level)
		{
			if (TaskListViewEnabled && parent != null)
			{
				// Find the task
				TaskNode task = (TaskNode)parent.Tag;
				ArrayList tasks = null;

				if (task != null)
				{
					switch(task.GetTaskType())
					{
						case TaskType.Project:
							ArrayList departments = MOG_DBProjectAPI.GetDepartments();

							tasks = new ArrayList();
							if (departments != null)
							{
								foreach (string department in departments)
								{
									ArrayList departmentTasks = MOG_DBTaskAPI.GetDepartmentTasks(department);
									if (departmentTasks != null)
									{
										foreach (MOG_TaskInfo departmentTask in departmentTasks)
										{
											tasks.Add(departmentTask);
										}
									}

									foreach (string user in MOG_DBProjectAPI.GetDepartmentUsers(department))
									{
										ArrayList userTasks = MOG_DBTaskAPI.GetAllUserTasks(user);
										if (userTasks != null)
										{
											foreach (MOG_TaskInfo userTask in userTasks)
											{
												tasks.Add(userTask);
											}
										}
									}								
								}
							}
							break;
						case TaskType.Department:
							tasks = MOG_DBTaskAPI.GetDepartmentTasks(task.GetName());
							if (tasks != null)
							{
								foreach (string user in MOG_DBProjectAPI.GetDepartmentUsers(task.GetName()))
								{
									ArrayList userTasks = MOG_DBTaskAPI.GetAllUserTasks(user);
									if (userTasks != null)
									{
										foreach (MOG_TaskInfo userTask in userTasks)
										{
											tasks.Add(userTask);
										}
									}
								}
							}
							break;
						case TaskType.User:
							tasks = MOG_DBTaskAPI.GetAllUserTasks(task.GetName());
							break;
						case TaskType.Task:
							tasks = MOG_DBTaskAPI.GetChildrenTasks(task.GetTaskInfo().GetTaskID());
							break;
					}

					if (tasks != null)
					{
						foreach (MOG_TaskInfo taskInfo in tasks)
						{
							this.ProjectManagerTasksListView.Items.Add(TaskCreateListViewItem(taskInfo));				
						}
					}
				}
			}
		}
	
		/// <summary>
		/// Create a new listView item based on a TaskInfo
		/// </summary>
		/// <param name="taskInfo"></param>
		/// <returns></returns>
		private ListViewItem TaskCreateListViewItem(MOG_TaskInfo taskInfo)
		{
			MOG_Time createTime = new MOG_Time(taskInfo.GetCreationDate());
			MOG_Time completeTime = new MOG_Time(taskInfo.GetCompletionDate());

			ListViewItem item = new ListViewItem();

			item.Text = taskInfo.GetName();
			item.SubItems.Add(taskInfo.GetDepartment());
			item.SubItems.Add(taskInfo.GetCreator());
			item.SubItems.Add(createTime.FormatString(""));
			item.SubItems.Add(completeTime.FormatString(""));
			item.SubItems.Add(taskInfo.GetStatus());
			item.SubItems.Add(taskInfo.GetPercentComplete().ToString() + "%");
			item.SubItems.Add(taskInfo.GetAssignedTo());
			item.SubItems.Add(taskInfo.GetPriority());
			
			TaskNode taskHandle = new TaskNode(TaskType.Task,taskInfo.GetName());
			item.ImageIndex = taskHandle.GetImage();

			// Record the task id for easy future lookup
			item.Tag = taskInfo;

			switch(taskInfo.GetPriority())
			{
				case "Low":
					item.ForeColor = Color.Blue;
					break;
				case "Medium":
					item.ForeColor = Color.BlueViolet;
					break;
				case "High":
					item.ForeColor = Color.Purple;
					break;
				case "Urgent":
					item.ForeColor = Color.Red;
					break;
			}

			return item;
		}

		/// <summary>
		/// Open task from the ListView
		/// </summary>
		public void TaskListEdit(ListView.SelectedListViewItemCollection SelectedItems)
		{
			foreach (ListViewItem item in SelectedItems)
			{
				NewTaskForm taskForm = new NewTaskForm(MOG_DBTaskAPI.GetTask((int)item.Tag));
				
				// If OK, update the database
				if (taskForm.ShowDialog() == DialogResult.OK)
				{
					bool complete = false;

					// Ipdate the database
					complete = MOG_DBTaskAPI.UpdateTask(taskForm.MOGTaskInfo);

					if (complete)
					{
						item.Remove();
						this.ProjectManagerTasksListView.Items.Add(TaskCreateListViewItem(taskForm.MOGTaskInfo));
					}
				}
			}
		}

		/// <summary>
		/// Main update function for the task ListView
		/// </summary>
		/// <param name="parent"></param>
		public void TaskUpdateList(TreeNode parent)
		{
			ProjectManagerTasksListView.Items.Clear();
			UpdateTaskListWalkChildren(parent, 0);
		}
		#endregion

		private void TaskUsersHideCheckBox_Click(object sender, System.EventArgs e)
		{
			InitializeToDoTree(TasksUsersTasksTreeView);
		}

		#region Tab Events

		/// <summary>
		/// Repopulate or create the TaskTree's within eash of the selected tabs
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TasksTabControl_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			switch(TasksTabControl.SelectedTab.Name)
			{
				case "TaskView":
					this.RefreshTaskTree();
					break;
				case "AssetView":
					// Leave it up to the Mog AssetTree to display everything in this tab page
					this.AssetView.ResumeLayout();
					break;
				case "":	// All custom tabs come in with blank names
					MOG_ColumnTreeView customTree = new MOG_ColumnTreeView();

					try
					{
						customTree = (MOG_ColumnTreeView)TasksTabControl.SelectedTab.Controls[0];

						TaskNode TopTask = (TaskNode)customTree.Tag;

						InitializeTaskTree(customTree, TopTask);
					}
					catch
					{
					}				
					break;
			}		
		}
		#endregion

		#region Task Tree methods
		private void RefreshTaskTree()
		{
			try
			{
				switch(mMode)
				{
					case RunningMode.User:
						InitializeForUser();
						break;
					case RunningMode.Project:
						InitializeForProject();
						break;
				}
			}
			catch{}
		}

		private void TasksUsersTasksTreeView_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
		{
			if(e.Data.GetDataPresent("ArrayListAssetManager"))
			{ 
				MOG_ColumnTreeView targetTree = (MOG_ColumnTreeView)sender;
				e.Effect = DragDropEffects.All;

				// Select the currently pointed at node
				Point pt = targetTree.PointToClient(new Point(e.X, e.Y));
				targetTree.SelectedNode = targetTree.GetNodeAt(pt);

				targetTree.SelectedNode.Expand();
				return;
			}
			
			e.Effect = DragDropEffects.None; 
		}

		private void TasksUsersTasksTreeView_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{
			// We will only accept a ArrayListAssetManager type object
			if(e.Data.GetDataPresent("ArrayListAssetManager"))
			{ 
				MOG_ColumnTreeView targetTree = (MOG_ColumnTreeView)sender;
				
				// Get our array list
				ArrayList items = (ArrayList)e.Data.GetData("ArrayListAssetManager");


				Point pt = targetTree.PointToClient(new Point(e.X, e.Y));
				targetTree.SelectedNode = targetTree.GetNodeAt(pt);
					
				TaskNode taskNode = (TaskNode)targetTree.SelectedNode.Tag;
				MOG_TaskInfo task = taskNode.GetTaskInfo();
				MOG_TaskInfo original = task.Clone();

				foreach(string item in items)
				{
					if (targetTree.SelectedNode != null)
					{
						MOG_Filename assetName = new MOG_Filename(item);

						// Add the 
						if (!task.AddAsset(assetName))
						{
							//Error!
						}
					}
				}

				string changesComment = task.GetChangesAsComment(original);
				if (changesComment.Length > 0)
				{
					task.SetComment(task.GetComment() + "\n" + changesComment);
				}
				else
				{
					task.SetComment(task.GetComment() + "\n" + "Assets added");
				}
				MOG_DBTaskAPI.UpdateTask(task);
			}
		}
		
		private void TasksUsersTasksTreeView_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			MOG_ColumnTreeView targetTree = (MOG_ColumnTreeView)sender;
			//Point pt = targetTree.PointToClient();
			targetTree.SelectedNode = targetTree.GetNodeAt(new Point(e.X, e.Y));
		}
		#endregion

		#region Task TabPage context menu
		private void TaskTabsRemoveMenuItem_Click(object sender, System.EventArgs e)
		{
            //int i = 1;		
		}
		#endregion

		#region Tool bar events
		private void TaskToolBar_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			switch (((string)e.Button.Tag).ToLower())
			{
				case "togglesort":
					mImportanceSorted = e.Button.Pushed;
					e.Button.ToolTipText = "Toggle sort {" + mImportanceSorted.ToString() + "}";
					RefreshTaskTree();
					break;
				case "toggleeditor":
					TaskEditorEnabled = e.Button.Pushed;
					break;
				case "togglelist":
					TaskListViewEnabled = e.Button.Pushed;
					break;					
			}
		}
		#endregion

		private void mogControl_TaskEditor_Load(object sender, System.EventArgs e)
		{
			// Adjust for not enabled sub components
			if (!TaskListViewEnabled)
			{
				mogControl_TaskEditor.Height = 0;
			}
		}

		/// <summary>
		/// Update our task view window with the latest task
		/// </summary>
		/// <param name="task"></param>
		private void TaskUpdateEditorList(MOG_TaskInfo task)
		{
			if (TaskEditorEnabled && task != null)
			{
				mogControl_TaskEditor.Initialize(task);
			}			
		}

		/// <summary>
		/// Clear our our task viewer window
		/// </summary>
		private void TaskUpdateEditorListClear()
		{
			mogControl_TaskEditor.Initialize(null);
		}

		/// <summary>
		/// If the user clicks an item in the list view, update the viewer
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ProjectManagerTasksListView_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			// Make sure we have a valid selected item
			if (this.ProjectManagerTasksListView.SelectedItems != null &&
				this.ProjectManagerTasksListView.SelectedItems.Count == 1)
			{
				try
				{
					// Try to get the MOG_TaskInfo
					MOG_TaskInfo Task = (MOG_TaskInfo)this.ProjectManagerTasksListView.SelectedItems[0].Tag;

					// Update the task viewer
					TaskUpdateEditorList(Task);
				}
				catch
				{
				}
			}
		}

		/// <summary>
		/// Update the viewer and list view when the user clicks in the tree view
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TasksUsersTasksTreeView_Click(object sender, System.EventArgs e)
		{
			// Update our task listView
			TaskUpdateList(this.TasksUsersTasksTreeView.SelectedNode);

			try
			{
				// Update our task viewer
				TaskNode task = (TaskNode)this.TasksUsersTasksTreeView.SelectedNode.Tag;
				TaskUpdateEditorList(task.GetTaskInfo());
			}
			catch
			{
				TaskUpdateEditorListClear();
			}
		}
	}

	#region Task Node
	public enum TaskType {Project, Department, User, Task};
	public enum DepartmentsCols {id, name};
	public enum UsersCols {id, name, department_id};
	public enum TasksCols {id, name, department_id, assignedTo_id, Comment, PercentComplete, Creator_id, CreationDate, CompletionDate, Status_id, priority_id, Asset, Project_id, Branch_id, TimeStamp, ResponsibleParty_id, Link_id, Parent_id, Child_Id};
	/// <summary>
	/// Summary description for TaskNode.
	/// </summary>
	public class TaskNode
	{

		private TaskType mType;
		private string mName;
		//private object mDepartmentInfo;
		//private object mUserInfo;
		private object mTaskInfo;
		
		public TaskNode(TaskType type, string name)
		{
			mType = type;
			mName = name;
			mTaskInfo = null;
		}

		public TaskType GetTaskType()
		{
			return mType;
		}

		public TaskNode(TaskType type, string name, object data)
		{
			mType = type;
			mName = name;

			switch(mType)
			{
				case TaskType.Task:
					mTaskInfo = data;
					break;
			}
		}

		public string GetName()
		{
			return mName;
		}

		public string GetExtendedName(string currentUser)
		{
			if (GetTaskInfo() != null)
			{
				if (string.Compare(GetTaskInfo().GetAssignedTo(), currentUser, true) != 0)
				{
					if (string.Compare(GetTaskInfo().GetCreator(), currentUser, true) == 0)
					{
						return GetTaskInfo().GetAssignedTo() + " working on - " + GetName() + "(" + GetTaskInfo().GetPercentComplete() + "%)";
					}
					else
					{						
						return GetTaskInfo().GetCreator() + " needs - " + GetName() + "(" + GetTaskInfo().GetPercentComplete() + "%) - from " + GetTaskInfo().GetAssignedTo();
					}
				}

				return GetName() + "(" + GetTaskInfo().GetPercentComplete() + "%)";
			}
			
			return GetName();
		}

//		public Font GetNodeFont(TreeView tree)
//		{
//			if (GetTaskInfo() != null)
//			{
//				if (GetTaskInfo().GetPercentComplete() >= 100)
//				{
//					return new System.Drawing.Font(tree.Font.FontFamily, tree.Font.Size, FontStyle.Strikeout, tree.Font.Unit, tree.Font.GdiCharSet, tree.Font.GdiVerticalFont);
//				}
//			}
//			
//			return new System.Drawing.Font(tree.Font.FontFamily, tree.Font.Size, FontStyle.Regular, tree.Font.Unit, tree.Font.GdiCharSet, tree.Font.GdiVerticalFont);
//		}

		public Font GetNodeFont(MOG_ColumnTreeView tree)
		{
			MOG_Time current = new MOG_Time();

			if (GetTaskInfo() != null)
			{
				if (GetTaskInfo().GetPercentComplete() >= 100)
				{
					return new System.Drawing.Font(tree.Font.FontFamily, tree.Font.Size, FontStyle.Strikeout, tree.Font.Unit, tree.Font.GdiCharSet, tree.Font.GdiVerticalFont);
				}				
			}
			
			return new System.Drawing.Font(tree.Font.FontFamily, tree.Font.Size, FontStyle.Regular, tree.Font.Unit, tree.Font.GdiCharSet, tree.Font.GdiVerticalFont);
		}

		public Color GetNodeFontColor(MOG_ColumnTreeView tree)
		{
			MOG_Time current = new MOG_Time();

			if (GetTaskInfo() != null)
			{
				if (current.Compare(new MOG_Time(GetTaskInfo().GetDueDate())) > 0)
				{
					return Color.Red;
				}
			}

			return SystemColors.ControlText;
		}

		
		public System.Windows.Forms.CheckState GetChecked()
		{
			if (GetTaskInfo() != null)
			{
				if (GetTaskInfo().GetPercentComplete() >= 100)
				{
					return CheckState.Checked;
				}
			}

			return CheckState.Unchecked;
		}

		public int GetImage()
		{
			switch(mType)
			{
				case TaskType.Department:
					return 1;
				case TaskType.User:
					return 2;
				case TaskType.Task:
					// Check if this is assigned to this user
					string userId = MOG_ControllerProject.GetUser().GetUserName();
					
					if (mTaskInfo != null)
					{
						if (string.Compare(userId, GetTaskInfo().GetAssignedTo(), true) != 0)
						{
							return 4; // This is assigned to someone else
						}
						else
						{
							// Check to see if this is a child task
							if (GetTaskInfo().GetParent().Length > 0)
							{
								return 6;
							}
							else
							{
								return 3; // This is assigned to me
							}
						}
					}
					else
					{
						return 3;
					}
			}
			return 7;
		}

		public MOG_TaskInfo GetTaskInfo()
		{
			if (mType == TaskType.Task)
			{
				return (MOG_TaskInfo)mTaskInfo;
			}

			return null;
		}

		public string GenerateUpdateCMD()
		{
			return "";
		}
	}
	#endregion
}
