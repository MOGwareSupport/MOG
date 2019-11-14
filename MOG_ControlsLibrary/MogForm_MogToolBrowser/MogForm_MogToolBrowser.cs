using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Globalization;
using System.ComponentModel;
using System.Windows.Forms;

using MOG.USER;
using MOG.SYSTEM;
using MOG.PROJECT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.CONTROLLER.CONTROLLERPROJECT;

using EV.Windows.Forms;
using MOG.REPORT;
using MOG.PROMPT;

namespace MOG_ControlsLibrary.Forms
{
	/// <summary>
	/// Summary description for MogForm_MogToolBrowser.
	/// </summary>
	public class MogForm_MogToolBrowser : System.Windows.Forms.Form
	{
		#region System defs
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.TextBox tbFilename;
		private System.Windows.Forms.Label lblFilenameLabel;
		private System.Windows.Forms.ListView lvFileView;
		private System.Windows.Forms.ColumnHeader chName;
		private System.Windows.Forms.ComboBox cbCurFolder;
		private System.Windows.Forms.ToolTip toolTips;
		private System.Windows.Forms.ImageList largeImageList;
		private System.Windows.Forms.ImageList smallimageList;
		private System.Windows.Forms.ToolBar toolBarFolderButtons;
		private System.Windows.Forms.ToolBar toolBarNavButtons;
		private System.Windows.Forms.ToolBarButton tbtnBack;
		private System.Windows.Forms.ToolBarButton tbtnUp;
		private System.Windows.Forms.ToolBarButton tbtnDelete;
		private System.Windows.Forms.ToolBarButton tbtnCreateFolder;
		private System.Windows.Forms.ToolBarButton tbtnSystemTools;
		private System.Windows.Forms.ToolBarButton tbtnProjectTools;
		private System.Windows.Forms.ToolBarButton tbtnUserTools;
		private System.Windows.Forms.ToolBarButton tbtnMyComputer;
		private System.Windows.Forms.ToolBarButton toolBarButton1;
		private System.Windows.Forms.ColumnHeader chSize;
		private System.Windows.Forms.ColumnHeader chType;
		private System.Windows.Forms.ColumnHeader chLastModified;
		private System.Windows.Forms.Label lblLookIn;
		private System.Windows.Forms.Button btnMyComputer;
		private System.Windows.Forms.Button btnSystemTools;
		private System.Windows.Forms.Button btnProjectTools;
		private System.Windows.Forms.Button btnUserTools;
		private ContextMenuStrip ToolsContextMenuStrip;
		private ToolStripMenuItem editToolStripMenuItem;
		private System.ComponentModel.IContainer components;

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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MogForm_MogToolBrowser));
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.tbFilename = new System.Windows.Forms.TextBox();
			this.lblFilenameLabel = new System.Windows.Forms.Label();
			this.lvFileView = new System.Windows.Forms.ListView();
			this.chName = new System.Windows.Forms.ColumnHeader();
			this.chSize = new System.Windows.Forms.ColumnHeader();
			this.chType = new System.Windows.Forms.ColumnHeader();
			this.chLastModified = new System.Windows.Forms.ColumnHeader();
			this.ToolsContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.smallimageList = new System.Windows.Forms.ImageList(this.components);
			this.largeImageList = new System.Windows.Forms.ImageList(this.components);
			this.cbCurFolder = new System.Windows.Forms.ComboBox();
			this.toolTips = new System.Windows.Forms.ToolTip(this.components);
			this.toolBarNavButtons = new System.Windows.Forms.ToolBar();
			this.tbtnBack = new System.Windows.Forms.ToolBarButton();
			this.tbtnUp = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton1 = new System.Windows.Forms.ToolBarButton();
			this.tbtnDelete = new System.Windows.Forms.ToolBarButton();
			this.tbtnCreateFolder = new System.Windows.Forms.ToolBarButton();
			this.toolBarFolderButtons = new System.Windows.Forms.ToolBar();
			this.tbtnSystemTools = new System.Windows.Forms.ToolBarButton();
			this.tbtnProjectTools = new System.Windows.Forms.ToolBarButton();
			this.tbtnUserTools = new System.Windows.Forms.ToolBarButton();
			this.tbtnMyComputer = new System.Windows.Forms.ToolBarButton();
			this.lblLookIn = new System.Windows.Forms.Label();
			this.btnMyComputer = new System.Windows.Forms.Button();
			this.btnSystemTools = new System.Windows.Forms.Button();
			this.btnProjectTools = new System.Windows.Forms.Button();
			this.btnUserTools = new System.Windows.Forms.Button();
			this.ToolsContextMenuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.Enabled = false;
			this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnOK.Location = new System.Drawing.Point(432, 304);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			this.btnOK.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lvFileView_KeyUp);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnCancel.Location = new System.Drawing.Point(512, 304);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			this.btnCancel.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lvFileView_KeyUp);
			// 
			// tbFilename
			// 
			this.tbFilename.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tbFilename.Location = new System.Drawing.Point(168, 304);
			this.tbFilename.Name = "tbFilename";
			this.tbFilename.Size = new System.Drawing.Size(240, 20);
			this.tbFilename.TabIndex = 3;
			this.tbFilename.Visible = false;
			this.tbFilename.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lvFileView_KeyUp);
			// 
			// lblFilenameLabel
			// 
			this.lblFilenameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblFilenameLabel.Location = new System.Drawing.Point(112, 304);
			this.lblFilenameLabel.Name = "lblFilenameLabel";
			this.lblFilenameLabel.Size = new System.Drawing.Size(56, 16);
			this.lblFilenameLabel.TabIndex = 4;
			this.lblFilenameLabel.Text = "Filename:";
			this.lblFilenameLabel.Visible = false;
			// 
			// lvFileView
			// 
			this.lvFileView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lvFileView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chName,
            this.chSize,
            this.chType,
            this.chLastModified});
			this.lvFileView.ContextMenuStrip = this.ToolsContextMenuStrip;
			this.lvFileView.FullRowSelect = true;
			this.lvFileView.HideSelection = false;
			this.lvFileView.Location = new System.Drawing.Point(112, 40);
			this.lvFileView.Name = "lvFileView";
			this.lvFileView.Size = new System.Drawing.Size(472, 256);
			this.lvFileView.SmallImageList = this.smallimageList;
			this.lvFileView.TabIndex = 6;
			this.lvFileView.UseCompatibleStateImageBehavior = false;
			this.lvFileView.View = System.Windows.Forms.View.Details;
			this.lvFileView.DoubleClick += new System.EventHandler(this.lvFileView_DoubleClick);
			this.lvFileView.SelectedIndexChanged += new System.EventHandler(this.lvFileView_SelectedIndexChanged);
			this.lvFileView.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.lvFileView_AfterLabelEdit);
			this.lvFileView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lvFileView_KeyUp);
			// 
			// chName
			// 
			this.chName.Text = "Name";
			this.chName.Width = 227;
			// 
			// chSize
			// 
			this.chSize.Text = "Size";
			this.chSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.chSize.Width = 76;
			// 
			// chType
			// 
			this.chType.Text = "Type";
			this.chType.Width = 64;
			// 
			// chLastModified
			// 
			this.chLastModified.Text = "Last Modified";
			this.chLastModified.Width = 97;
			// 
			// ToolsContextMenuStrip
			// 
			this.ToolsContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editToolStripMenuItem});
			this.ToolsContextMenuStrip.Name = "ToolsContextMenuStrip";
			this.ToolsContextMenuStrip.Size = new System.Drawing.Size(208, 26);
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
			this.editToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
			this.editToolStripMenuItem.Text = "Edit in notepad...";
			this.editToolStripMenuItem.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
			// 
			// smallimageList
			// 
			this.smallimageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("smallimageList.ImageStream")));
			this.smallimageList.TransparentColor = System.Drawing.Color.Red;
			this.smallimageList.Images.SetKeyName(0, "");
			this.smallimageList.Images.SetKeyName(1, "");
			this.smallimageList.Images.SetKeyName(2, "");
			this.smallimageList.Images.SetKeyName(3, "");
			this.smallimageList.Images.SetKeyName(4, "");
			this.smallimageList.Images.SetKeyName(5, "");
			this.smallimageList.Images.SetKeyName(6, "");
			this.smallimageList.Images.SetKeyName(7, "");
			this.smallimageList.Images.SetKeyName(8, "");
			this.smallimageList.Images.SetKeyName(9, "");
			// 
			// largeImageList
			// 
			this.largeImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("largeImageList.ImageStream")));
			this.largeImageList.TransparentColor = System.Drawing.Color.Red;
			this.largeImageList.Images.SetKeyName(0, "");
			this.largeImageList.Images.SetKeyName(1, "");
			this.largeImageList.Images.SetKeyName(2, "");
			this.largeImageList.Images.SetKeyName(3, "");
			this.largeImageList.Images.SetKeyName(4, "");
			this.largeImageList.Images.SetKeyName(5, "");
			this.largeImageList.Images.SetKeyName(6, "");
			this.largeImageList.Images.SetKeyName(7, "");
			this.largeImageList.Images.SetKeyName(8, "");
			this.largeImageList.Images.SetKeyName(9, "");
			// 
			// cbCurFolder
			// 
			this.cbCurFolder.Location = new System.Drawing.Point(112, 8);
			this.cbCurFolder.Name = "cbCurFolder";
			this.cbCurFolder.Size = new System.Drawing.Size(232, 21);
			this.cbCurFolder.TabIndex = 11;
			this.cbCurFolder.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lvFileView_KeyUp);
			// 
			// toolBarNavButtons
			// 
			this.toolBarNavButtons.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.tbtnBack,
            this.tbtnUp,
            this.toolBarButton1,
            this.tbtnDelete,
            this.tbtnCreateFolder});
			this.toolBarNavButtons.ButtonSize = new System.Drawing.Size(24, 23);
			this.toolBarNavButtons.Divider = false;
			this.toolBarNavButtons.Dock = System.Windows.Forms.DockStyle.None;
			this.toolBarNavButtons.DropDownArrows = true;
			this.toolBarNavButtons.ImageList = this.smallimageList;
			this.toolBarNavButtons.Location = new System.Drawing.Point(360, 8);
			this.toolBarNavButtons.Name = "toolBarNavButtons";
			this.toolBarNavButtons.ShowToolTips = true;
			this.toolBarNavButtons.Size = new System.Drawing.Size(120, 26);
			this.toolBarNavButtons.TabIndex = 12;
			this.toolBarNavButtons.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBarNavButtons_ButtonClick);
			this.toolBarNavButtons.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lvFileView_KeyUp);
			// 
			// tbtnBack
			// 
			this.tbtnBack.ImageIndex = 3;
			this.tbtnBack.Name = "tbtnBack";
			// 
			// tbtnUp
			// 
			this.tbtnUp.ImageIndex = 4;
			this.tbtnUp.Name = "tbtnUp";
			// 
			// toolBarButton1
			// 
			this.toolBarButton1.Name = "toolBarButton1";
			this.toolBarButton1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// tbtnDelete
			// 
			this.tbtnDelete.ImageIndex = 5;
			this.tbtnDelete.Name = "tbtnDelete";
			this.tbtnDelete.Visible = false;
			// 
			// tbtnCreateFolder
			// 
			this.tbtnCreateFolder.ImageIndex = 6;
			this.tbtnCreateFolder.Name = "tbtnCreateFolder";
			this.tbtnCreateFolder.Visible = false;
			// 
			// toolBarFolderButtons
			// 
			this.toolBarFolderButtons.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.tbtnSystemTools,
            this.tbtnProjectTools,
            this.tbtnUserTools,
            this.tbtnMyComputer});
			this.toolBarFolderButtons.ButtonSize = new System.Drawing.Size(86, 62);
			this.toolBarFolderButtons.Divider = false;
			this.toolBarFolderButtons.Dock = System.Windows.Forms.DockStyle.None;
			this.toolBarFolderButtons.DropDownArrows = true;
			this.toolBarFolderButtons.ImageList = this.largeImageList;
			this.toolBarFolderButtons.Location = new System.Drawing.Point(16, 40);
			this.toolBarFolderButtons.Name = "toolBarFolderButtons";
			this.toolBarFolderButtons.ShowToolTips = true;
			this.toolBarFolderButtons.Size = new System.Drawing.Size(88, 212);
			this.toolBarFolderButtons.TabIndex = 13;
			this.toolBarFolderButtons.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBarFolderButtons_ButtonClick);
			this.toolBarFolderButtons.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lvFileView_KeyUp);
			// 
			// tbtnSystemTools
			// 
			this.tbtnSystemTools.ImageIndex = 8;
			this.tbtnSystemTools.Name = "tbtnSystemTools";
			this.tbtnSystemTools.Text = "System Tools";
			// 
			// tbtnProjectTools
			// 
			this.tbtnProjectTools.ImageIndex = 8;
			this.tbtnProjectTools.Name = "tbtnProjectTools";
			this.tbtnProjectTools.Text = "Project Tools";
			// 
			// tbtnUserTools
			// 
			this.tbtnUserTools.ImageIndex = 8;
			this.tbtnUserTools.Name = "tbtnUserTools";
			this.tbtnUserTools.Text = "User Tools";
			// 
			// tbtnMyComputer
			// 
			this.tbtnMyComputer.ImageIndex = 9;
			this.tbtnMyComputer.Name = "tbtnMyComputer";
			this.tbtnMyComputer.Text = "My Computer";
			// 
			// lblLookIn
			// 
			this.lblLookIn.Location = new System.Drawing.Point(40, 8);
			this.lblLookIn.Name = "lblLookIn";
			this.lblLookIn.Size = new System.Drawing.Size(56, 16);
			this.lblLookIn.TabIndex = 14;
			this.lblLookIn.Text = "Look in:";
			// 
			// btnMyComputer
			// 
			this.btnMyComputer.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnMyComputer.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
			this.btnMyComputer.ImageIndex = 9;
			this.btnMyComputer.ImageList = this.largeImageList;
			this.btnMyComputer.Location = new System.Drawing.Point(16, 232);
			this.btnMyComputer.Name = "btnMyComputer";
			this.btnMyComputer.Size = new System.Drawing.Size(86, 64);
			this.btnMyComputer.TabIndex = 15;
			this.btnMyComputer.Text = "My Computer";
			this.btnMyComputer.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.btnMyComputer.Click += new System.EventHandler(this.btnMyComputer_Click);
			// 
			// btnSystemTools
			// 
			this.btnSystemTools.BackColor = System.Drawing.SystemColors.Control;
			this.btnSystemTools.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnSystemTools.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
			this.btnSystemTools.ImageIndex = 8;
			this.btnSystemTools.ImageList = this.largeImageList;
			this.btnSystemTools.Location = new System.Drawing.Point(16, 40);
			this.btnSystemTools.Name = "btnSystemTools";
			this.btnSystemTools.Size = new System.Drawing.Size(86, 64);
			this.btnSystemTools.TabIndex = 18;
			this.btnSystemTools.Text = "System Tools";
			this.btnSystemTools.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.btnSystemTools.UseVisualStyleBackColor = false;
			this.btnSystemTools.Click += new System.EventHandler(this.btnSystemTools_Click);
			// 
			// btnProjectTools
			// 
			this.btnProjectTools.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnProjectTools.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
			this.btnProjectTools.ImageIndex = 8;
			this.btnProjectTools.ImageList = this.largeImageList;
			this.btnProjectTools.Location = new System.Drawing.Point(16, 104);
			this.btnProjectTools.Name = "btnProjectTools";
			this.btnProjectTools.Size = new System.Drawing.Size(86, 64);
			this.btnProjectTools.TabIndex = 19;
			this.btnProjectTools.Text = "Project Tools";
			this.btnProjectTools.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.btnProjectTools.Click += new System.EventHandler(this.btnProjectTools_Click);
			// 
			// btnUserTools
			// 
			this.btnUserTools.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnUserTools.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
			this.btnUserTools.ImageIndex = 8;
			this.btnUserTools.ImageList = this.largeImageList;
			this.btnUserTools.Location = new System.Drawing.Point(16, 168);
			this.btnUserTools.Name = "btnUserTools";
			this.btnUserTools.Size = new System.Drawing.Size(86, 64);
			this.btnUserTools.TabIndex = 20;
			this.btnUserTools.Text = "User Tools";
			this.btnUserTools.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.btnUserTools.Click += new System.EventHandler(this.btnUserTools_Click);
			// 
			// MogForm_MogToolBrowser
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(600, 334);
			this.Controls.Add(this.btnUserTools);
			this.Controls.Add(this.btnProjectTools);
			this.Controls.Add(this.btnSystemTools);
			this.Controls.Add(this.btnMyComputer);
			this.Controls.Add(this.lblLookIn);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.toolBarFolderButtons);
			this.Controls.Add(this.toolBarNavButtons);
			this.Controls.Add(this.cbCurFolder);
			this.Controls.Add(this.lvFileView);
			this.Controls.Add(this.lblFilenameLabel);
			this.Controls.Add(this.tbFilename);
			this.Controls.Add(this.btnCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(608, 368);
			this.Name = "MogForm_MogToolBrowser";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Browse MOG Tools";
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lvFileView_KeyUp);
			this.ToolsContextMenuStrip.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion
		#endregion
		#region Member vars
		private string currentPath = "";
		private Stack folderHistory = new Stack();

		private string systemToolsPath = "";
		private string projectToolsPath = "";
		private string userToolsPath = "";

		private ArrayList listViewSortManager = null;

		private string defaultFolder = "My Computer";
		#endregion
		#region Properties
		public string SelectedPath
		{
			get 
			{
				if (this.lvFileView.SelectedItems.Count > 0)
				{
					if (this.lvFileView.SelectedItems[0].Tag is string)
					{
						return (string)this.lvFileView.SelectedItems[0].Tag;
					}
				}

				return "";
			}
		}

		public string DefaultFolder
		{
			get { return this.defaultFolder; }
			set { this.defaultFolder = value; }
		}

		public string SystemToolsPath
		{
			get { return this.systemToolsPath; }
			set { this.systemToolsPath = value; }
		}

		public string ProjectToolsPath
		{
			get { return this.projectToolsPath; }
			set { this.projectToolsPath = value; }
		}

		public string UserToolsPath
		{
			get { return this.userToolsPath; }
			set { this.userToolsPath = value; }
		}
		#endregion
		#region Constants

		public const string text_MyComputer = "My Computer";

		private const int iconindex_FILE			= 0;
		private const int iconindex_FOLDER			= 1;
		private const int iconindex_DRIVE			= 2;
		private const int iconindex_BACK			= 3;
		private const int iconindex_UP				= 4;
		private const int iconindex_DELETE			= 5;
		private const int iconindex_NEWFOLDER		= 6;
		private const int iconindex_DESKTOP			= 7;
		private const int iconindex_SPECIALFOLDER	= 8;

		#endregion
		#region Constructors
		public MogForm_MogToolBrowser()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// Generic init
			init();

// JohnRen - Removed because it was taking too long and we no longer default here anyway
//			// Default to My Computer
//			DisplayMyComputer();
		}

		public MogForm_MogToolBrowser(string path)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// Generic init
			init();

			OpenPath(path);
		}

		private void init()
		{
			// Setup column sorting
			this.listViewSortManager = new ArrayList();
			this.listViewSortManager.Add(new ListViewSortManager(this.lvFileView, new Type[] {
																									typeof(ListViewTextCaseInsensitiveSort),
																									typeof(ListViewFileSizeSort),
																									typeof(ListViewTextCaseInsensitiveSort),
																									typeof(ListViewDateSort),
			}));


			this.tbtnBack.Enabled = false;

			// Get MOG tools paths info
			UpdateMogPaths();
		}
		#endregion
		#region Static member functions
		public static string ShowBrowser(string path)
		{
			MogForm_MogToolBrowser form = new MogForm_MogToolBrowser();
			form.defaultFolder = form.ProjectToolsPath;
			form.OpenPath(path);
			if (form.ShowDialog() == DialogResult.OK)
			{
				return form.SelectedPath;
			}
			else
			{
				return path;
			}
		}
		#endregion
		#region Member functions
		#region Utils
		private void AutoSizeColumns()
		{
			foreach (ColumnHeader h in this.lvFileView.Columns)
			{
				// Don't resize the name column
				if (h != this.chName)
				{
					h.Width = -2;		// autosize
				}
			}
		}

		private string FormatFileSize(long size)
		{
			size /= 1024;			// convert to KB
			if (size < 1) size = 1;

			// Set up a NumberFormatInfo structure to kill decimal points on the ToString() call below
			NumberFormatInfo nfi = new CultureInfo( "en-US", false ).NumberFormat;
			nfi.NumberDecimalDigits = 0;
			return size.ToString("N", nfi) + " KB";
		}

		private void NavigateBack()
		{
			if (this.folderHistory.Count > 0)
			{
				string path = this.folderHistory.Pop() as string;
			
				// If the history's empty, we have nowhere to go back to
				if (this.folderHistory.Count == 0)
				{
					this.tbtnBack.Enabled = false;
				}

				if (path != null)
				{
					DisplayFolder(path);
				}

				this.tbtnUp.Enabled = !(this.currentPath.ToLower() == text_MyComputer.ToLower());
			}
		}

		private void NavigateUp()
		{
			// Can't go up from My Comptuer
			if (this.currentPath.ToLower() == text_MyComputer.ToLower())
			{
				return;
			}

			// If currentPath is the root of a logical drive, go to my computer
			string parentPath = Path.GetDirectoryName(this.currentPath);
			if (parentPath == null)
			{
				SwitchToPath(text_MyComputer);
			}
			else
			{
				// Switch to its parent
				SwitchToPath(parentPath);
			}
		}

		private void CreateNewFolder()
		{
			// Create a node and edit its label (the AfterLabelEdit handler will create the directory and do error checking)
			ListViewItem newFolderItem = new ListViewItem("New Folder", iconindex_FOLDER);
			this.lvFileView.Items.Add(newFolderItem);
			newFolderItem.EnsureVisible();
			this.lvFileView.LabelEdit = true;
			newFolderItem.BeginEdit();
		}

		private bool PathAccessible(string path)
		{
			// Make sure path exists
			if (!Directory.Exists(path))
			{
				return false;
			}

			// Make sure we can view path
			try
			{
				Directory.GetFiles(path);
				Directory.GetDirectories(path);
			}
			catch
			{
				return false;
			}

			return true;
		}

		private void DeleteSelectedItems()
		{
			if (this.lvFileView.SelectedItems.Count == 1)
			{
                if (MOG_Prompt.PromptResponse("Confirm Delete", "Are you sure you want to delete " + lvFileView.SelectedItems[0].Text + "?", MOGPromptButtons.YesNo) != MOGPromptResult.Yes)
				{
					return;
				}
			}
			else if (this.lvFileView.SelectedItems.Count > 1)
			{
				if (MOG_Prompt.PromptResponse("Confirm Delete", "Are you sure you want to delete these " + lvFileView.SelectedItems.Count.ToString() + " items?", MOGPromptButtons.YesNo) != MOGPromptResult.Yes)
				{
					return;
				}
			}

			// Proceed with delete
			foreach (ListViewItem deleteItem in this.lvFileView.SelectedItems)
			{
				string deletePath = "";
				try
				{
					deletePath = deleteItem.Tag as string;
					if (deletePath != null)
					{
						if (File.Exists(deletePath))
						{
							File.Delete(deletePath);
						}
						else if (Directory.Exists(deletePath))
						{
							Directory.Delete(deletePath, true);
						}
					}
				}
				catch (Exception ex)
				{
					MOG_Report.ReportMessage("Delete Failed", "Couldn't delete " + deleteItem.Text, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.ERROR);
				}
			}

			// Refresh listview
			DisplayFolder(this.currentPath);
		}
		#endregion

		public void UpdateMogPaths()
		{
			// Load the system/project/user tools paths and enable their buttons accordingly

			MOG_System sys = MOG_ControllerSystem.GetSystem();
			if (sys != null)
			{
				this.systemToolsPath = sys.GetSystemToolsPath();
				this.tbtnSystemTools.Enabled = Directory.Exists(this.systemToolsPath);
			}
			else
			{
				this.tbtnSystemTools.Enabled = false;
				this.systemToolsPath = "";
			}

			MOG_Project proj = MOG_ControllerProject.GetProject();
			if (proj != null)
			{
				this.projectToolsPath = proj.GetProjectToolsPath();
				this.tbtnProjectTools.Enabled = Directory.Exists(this.projectToolsPath);
			}
			else
			{
				this.tbtnProjectTools.Enabled = false;
				this.projectToolsPath = "";
			}

			MOG_User usr = MOG_ControllerProject.GetUser();
			if (usr != null)
			{
				this.userToolsPath = usr.GetUserToolsPath();
				this.tbtnUserTools.Enabled = Directory.Exists(this.userToolsPath);
			}
			else
			{
				this.tbtnUserTools.Enabled = false;
				this.userToolsPath = "";
			}
		}

		private void SwitchToPath(string path)
		{
			// We need a copy before the DisplayFolder() call because it will change currentPath
			string curDirCopy = this.currentPath;
			if (DisplayFolder(path))
			{
				// Update folder history
				this.folderHistory.Push(curDirCopy);
				this.tbtnBack.Enabled = true;
			}
		}

		public bool OpenPath(string path)
		{
			if (path == null  ||  path == "")
			{
				return DisplayFolder(this.defaultFolder);
			}

			// If path doesn't exist, try to push it through LocateTool()
			if (!File.Exists(path)  &&  !Directory.Exists(path))
			{
				path = MOG.CONTROLLER.CONTROLLERSYSTEM.MOG_ControllerSystem.LocateTool(path);
			}

			// If all else fails, just load the default directory
			if (!File.Exists(path)  &&  !Directory.Exists(path))
			{
				return DisplayFolder(this.defaultFolder);
			}

			return DisplayFolder(path);
		}

		private bool DisplayMyComputer()
		{
			// Prepare listview for update
			this.lvFileView.Items.Clear();
			this.lvFileView.BeginUpdate();
			Cursor.Current = Cursors.WaitCursor;

			ListViewItem driveItem = null;

			// Load "My Computer"
			foreach (string driveName in Directory.GetLogicalDrives())
			{
				if (Directory.Exists(driveName))
				{
					driveItem = new ListViewItem(driveName.ToUpper(), iconindex_DRIVE);
					driveItem.SubItems.Add("");
					driveItem.SubItems.Add("Logical Drive");
					driveItem.SubItems.Add("");
					driveItem.Tag = driveName;

					// Make sure path is valid and we can read from it and set color accordingly
					if (!PathAccessible(driveName))
					{
						driveItem.ForeColor = Color.Red;
					}

					this.lvFileView.Items.Add(driveItem);
				}
			}

			AutoSizeColumns();

			// We're done
			this.lvFileView.EndUpdate();
			Cursor.Current = Cursors.Default;

			// Set up path vars
			this.cbCurFolder.Text = text_MyComputer;
			this.currentPath = text_MyComputer;
			this.tbFilename.Text = "";

			// Can't delete anything or make folders at the My Computer level
			this.tbtnDelete.Enabled = false;
			this.tbtnCreateFolder.Enabled = false;
			this.tbtnUp.Enabled = false;

			return true;
		}

		private bool DisplayFolder(string path)
		{
			if (path == null  ||  path == "")
			{
				return DisplayFolder(this.defaultFolder);
			}

			if (File.Exists(path))
			{
				path = Path.GetDirectoryName(path);
			}

			// Filter out special paths
			if (path.ToLower() == text_MyComputer.ToLower())
			{
				return DisplayMyComputer();
			}

			// Make sure path is valid and we can read from it
			if (!PathAccessible(path))
			{
				MOG_Report.ReportMessage("Forbidden", "Couldn't open folder " + path, Environment.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.ERROR);
				return false;
			}

			// Prep the listview for update
			this.lvFileView.Items.Clear();
			this.lvFileView.BeginUpdate();
			Cursor.Current = Cursors.WaitCursor;

			// Do folders first
			ListViewItem dirItem = null;
			foreach (DirectoryInfo dInfo in new DirectoryInfo(path).GetDirectories())
			{
				try
				{
					dirItem = new ListViewItem( Path.GetFileName(dInfo.FullName), iconindex_FOLDER );
					dirItem.SubItems.Add("");								// file size
					dirItem.SubItems.Add("File Folder");					// file type
					dirItem.SubItems.Add(dInfo.LastWriteTime.ToString());	// last modified date
					dirItem.Tag = dInfo.FullName;
					this.lvFileView.Items.Add(dirItem);
				}
				catch
				{
					// Set to red if there was any kind of failure
					dirItem.ForeColor = Color.Red;
				}
			}

			// Now the files
			ListViewItem fileItem = null;
			foreach (FileInfo fInfo in new DirectoryInfo(path).GetFiles())
			{
				try
				{
					fileItem = new ListViewItem( Path.GetFileName(fInfo.FullName), iconindex_FILE );
					fileItem.SubItems.Add( FormatFileSize(fInfo.Length) );						// file size
					fileItem.SubItems.Add( Path.GetExtension(fInfo.FullName).ToUpper() );		// file type
					fileItem.SubItems.Add(fInfo.LastWriteTime.ToString());					// last modified date
					fileItem.Tag = fInfo.FullName;
					this.lvFileView.Items.Add(fileItem);
				}
				catch
				{
					// Set to red if there's a problem
					fileItem.ForeColor = Color.Red;
				}
			}

			// Update path vars
			if (path.ToLower().StartsWith( this.systemToolsPath.ToLower() ))
			{
				this.cbCurFolder.Text = "System Tools";
			}
			else if (path.ToLower().StartsWith( this.projectToolsPath.ToLower() ))
			{
				this.cbCurFolder.Text = "Project Tools";
			}
			else if (path.ToLower().StartsWith( this.userToolsPath.ToLower() ))
			{
				this.cbCurFolder.Text = "User Tools";
			}
			else
			{
				this.cbCurFolder.Text = Path.GetFileName(path);
			}
			
			// If this path has no filename (i.e., its a root directory)
			if (this.cbCurFolder.Text == "")
			{
				this.cbCurFolder.Text = path;
			}

			this.currentPath = path;
			this.tbFilename.Text = path;

			AutoSizeColumns();

			// Tell the listview we're done updating it
			this.lvFileView.EndUpdate();
			Cursor.Current = Cursors.Default;

			this.tbtnDelete.Enabled = true;
			this.tbtnCreateFolder.Enabled = true;

			// Disable up button for My Computer and the MOG tools directories
			this.tbtnUp.Enabled = !(path.ToLower() == text_MyComputer.ToLower())  &&
				!(path.ToLower() == this.systemToolsPath.ToLower())  &&
				!(path.ToLower() == this.projectToolsPath.ToLower())  &&
				!(path.ToLower() == this.userToolsPath.ToLower());

			return true;
		}
		#endregion
		#region Event handlers
		private void mogControl_MogToolBrowserTreeView1_DoubleClick(object sender, System.EventArgs e)
		{
			//MessageBox.Show(this.mogControl_MogToolBrowserTreeView1.SelectedPath);
			//Dispose();
		}

//		private void mogControl_MogToolBrowserTreeView1_Event_NodeClicked(object sender, MogToolEventArgs args)
//		{
//			this.tbFilename.Text = args.NodePath;
//		}

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			Hide();
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			Hide();
		}

		private void lvFileView_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyData == Keys.F5)
			{
				DisplayFolder(this.currentPath);
			}
			else if (e.KeyData == Keys.Back)
			{
				if (this.tbtnBack.Enabled)
				{
					NavigateBack();
				}
			}
			else if (e.KeyData == Keys.Enter)
			{
				if (this.btnOK.Enabled)
				{
					this.btnOK.PerformClick();
				}
			}
			else if (e.KeyData == Keys.Escape)
			{
				this.btnCancel.PerformClick();
			}
			//			else if (e.KeyData == Keys.Delete)
			//			{
			//				DeleteSelectedItems();
			//			}
		}

		private void lvFileView_DoubleClick(object sender, System.EventArgs e)
		{
			if (this.lvFileView.SelectedItems.Count > 0)
			{
				if (this.lvFileView.SelectedItems[0].Tag is string)
				{
					string selectedItem = (string)this.lvFileView.SelectedItems[0].Tag;
					if (Directory.Exists(selectedItem))
					{
						SwitchToPath( (string)this.lvFileView.SelectedItems[0].Tag );
					}
					else if (File.Exists(selectedItem))
					{
						this.btnOK.PerformClick();
					}
				}
				else
				{
					MessageBox.Show("Bad Tag");
				}
			}
		}

		private void btnMyComputer_Click(object sender, System.EventArgs e)
		{
			SwitchToPath(text_MyComputer);
		}

		private void btnSystemTools_Click(object sender, System.EventArgs e)
		{
			if (Directory.Exists(this.systemToolsPath))
			{
				SwitchToPath(this.systemToolsPath);
			}
		}

		private void btnProjectTools_Click(object sender, System.EventArgs e)
		{
			if (Directory.Exists(this.projectToolsPath))
			{
				SwitchToPath(this.projectToolsPath);
			}

		}

		private void btnUserTools_Click(object sender, System.EventArgs e)
		{
			if (Directory.Exists(this.userToolsPath))
			{
				SwitchToPath(this.userToolsPath);
			}
		}

		private void lvFileView_AfterLabelEdit(object sender, System.Windows.Forms.LabelEditEventArgs e)
		{
			ListViewItem item = null;
			try
			{
				item = this.lvFileView.Items[e.Item];

				// If we couldn't get an item, trigger the error handling in the catch block below
				if (item == null)
				{
					throw new Exception();
				}

				string label = e.Label;
				if (label == null)
				{
					label = item.Text;
				}

				string path = this.currentPath + "\\" + label;

				if (Directory.Exists(path))
				{
					throw new Exception();
				}

				if (!Directory.CreateDirectory(path).Exists)
				{
					throw new Exception();
				}

				// Set the selected node
				item.Selected = true;
				item.Tag = path;
			}
			catch (Exception ex)
			{
				MOG_Report.ReportMessage("Error", "Error creating folder", ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.ERROR);
				e.CancelEdit = true;
				if (item != null)
				{
					item.Remove();
				}
			}
			finally
			{
				this.lvFileView.LabelEdit = false;
			}
		}

		private void lvFileView_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.lvFileView.SelectedItems.Count > 0)
			{
				this.tbFilename.Text = this.lvFileView.SelectedItems[0].Text;
				this.btnOK.Enabled = true;
				this.tbtnDelete.Enabled = true;
			}
			else
			{
				this.btnOK.Enabled = false;
				this.tbtnDelete.Enabled = false;
			}
		}

		private void toolBarFolderButtons_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if (e.Button == this.tbtnSystemTools)
			{
				if (Directory.Exists(this.systemToolsPath))
				{
					SwitchToPath(this.systemToolsPath);
				}
			}
			else if (e.Button == this.tbtnProjectTools)
			{
				if (Directory.Exists(this.projectToolsPath))
				{
					SwitchToPath(this.projectToolsPath);
				}
			}
			else if (e.Button == this.tbtnUserTools)
			{
				if (Directory.Exists(this.userToolsPath))
				{
					SwitchToPath(this.userToolsPath);
				}
			}
			else if (e.Button == this.tbtnMyComputer)
			{
				SwitchToPath(text_MyComputer);
			}
		}

		private void toolBarNavButtons_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if (e.Button == this.tbtnBack)
			{
				NavigateBack();
			}
			else if (e.Button == this.tbtnUp)
			{
				NavigateUp();
			}
			else if (e.Button == this.tbtnDelete)
			{
				DeleteSelectedItems();
			}
			else if (e.Button == this.tbtnCreateFolder)
			{
				CreateNewFolder();
			}
		}
		#endregion

		private void editToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (lvFileView.SelectedItems.Count > 0)
			{
				foreach (ListViewItem item in lvFileView.SelectedItems)
				{
					string ripper = item.Tag as string;

					// Edit the ripper
					System.Diagnostics.Process.Start("Notepad.exe", ripper);
				}
			}
		}
	}
}
