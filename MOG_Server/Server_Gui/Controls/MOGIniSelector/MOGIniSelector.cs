using System;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Runtime.InteropServices;


using MOG_Server.Server_Utilities;
using MOG_Server.MOG_ControlsLibrary.Common;

using MOG;
using MOG.INI;

namespace MOG_Server.Server_Gui
{
	/// <summary>
	/// Summary description for MOGIniSelector.
	/// </summary>
	public class MOGIniSelector : System.Windows.Forms.UserControl
	{
		#region System definitions
		private System.Windows.Forms.ListView lvRepositories;
		private System.Windows.Forms.ListView lvIniFile;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnLocate;
		private System.Windows.Forms.Button btnRemove;
		private System.Windows.Forms.ContextMenu cmRepositories;
		private System.Windows.Forms.MenuItem miLocateNew;
		private System.Windows.Forms.MenuItem miRemove;
		private System.Windows.Forms.ContextMenu cmIniFiles;
		private System.Windows.Forms.MenuItem miView;
		private System.Windows.Forms.MenuItem miBrowseToRepository;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem miRename;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem miMoveToTop;
		private System.Windows.Forms.MenuItem miMoveToBottom;
		private System.Windows.Forms.MenuItem menuItem2;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

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
			this.lvRepositories = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.cmRepositories = new System.Windows.Forms.ContextMenu();
			this.miLocateNew = new System.Windows.Forms.MenuItem();
			this.miBrowseToRepository = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.miRename = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.miMoveToTop = new System.Windows.Forms.MenuItem();
			this.miMoveToBottom = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.miRemove = new System.Windows.Forms.MenuItem();
			this.lvIniFile = new System.Windows.Forms.ListView();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.cmIniFiles = new System.Windows.Forms.ContextMenu();
			this.miView = new System.Windows.Forms.MenuItem();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnLocate = new System.Windows.Forms.Button();
			this.btnRemove = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lvRepositories
			// 
			this.lvRepositories.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lvRepositories.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																							 this.columnHeader1,
																							 this.columnHeader2});
			this.lvRepositories.ContextMenu = this.cmRepositories;
			this.lvRepositories.FullRowSelect = true;
			this.lvRepositories.HideSelection = false;
			this.lvRepositories.LabelEdit = true;
			this.lvRepositories.Location = new System.Drawing.Point(8, 8);
			this.lvRepositories.MultiSelect = false;
			this.lvRepositories.Name = "lvRepositories";
			this.lvRepositories.Size = new System.Drawing.Size(264, 144);
			this.lvRepositories.TabIndex = 0;
			this.lvRepositories.View = System.Windows.Forms.View.Details;
			this.lvRepositories.DoubleClick += new System.EventHandler(this.lvRepositories_DoubleClick);
			this.lvRepositories.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lvRepositories_MouseUp);
			this.lvRepositories.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lvRepositories_KeyUp);
			this.lvRepositories.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.lvRepositories_AfterLabelEdit);
			this.lvRepositories.SelectedIndexChanged += new System.EventHandler(this.lvRepositories_SelectedIndexChanged);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Name";
			this.columnHeader1.Width = 92;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Path";
			this.columnHeader2.Width = 166;
			// 
			// cmRepositories
			// 
			this.cmRepositories.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						   this.miLocateNew,
																						   this.miBrowseToRepository,
																						   this.menuItem1,
																						   this.miRename,
																						   this.menuItem3,
																						   this.miMoveToTop,
																						   this.miMoveToBottom,
																						   this.menuItem2,
																						   this.miRemove});
			this.cmRepositories.Popup += new System.EventHandler(this.cmRepositories_Popup);
			// 
			// miLocateNew
			// 
			this.miLocateNew.Index = 0;
			this.miLocateNew.Text = "Locate new...";
			this.miLocateNew.Click += new System.EventHandler(this.miLocateNew_Click);
			// 
			// miBrowseToRepository
			// 
			this.miBrowseToRepository.Index = 1;
			this.miBrowseToRepository.Text = "Browse to repository...";
			this.miBrowseToRepository.Click += new System.EventHandler(this.miBrowseToRepository_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 2;
			this.menuItem1.Text = "-";
			// 
			// miRename
			// 
			this.miRename.Index = 3;
			this.miRename.Text = "Rename";
			this.miRename.Click += new System.EventHandler(this.miRename_Click);
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 4;
			this.menuItem3.Text = "-";
			// 
			// miMoveToTop
			// 
			this.miMoveToTop.Index = 5;
			this.miMoveToTop.Text = "Move to top";
			this.miMoveToTop.Click += new System.EventHandler(this.miMoveToTop_Click);
			// 
			// miMoveToBottom
			// 
			this.miMoveToBottom.Index = 6;
			this.miMoveToBottom.Text = "Move to bottom";
			this.miMoveToBottom.Click += new System.EventHandler(this.miMoveToBottom_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 7;
			this.menuItem2.Text = "-";
			// 
			// miRemove
			// 
			this.miRemove.Index = 8;
			this.miRemove.Text = "Remove";
			this.miRemove.Click += new System.EventHandler(this.miRemove_Click);
			// 
			// lvIniFile
			// 
			this.lvIniFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lvIniFile.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						this.columnHeader3});
			this.lvIniFile.ContextMenu = this.cmIniFiles;
			this.lvIniFile.FullRowSelect = true;
			this.lvIniFile.HideSelection = false;
			this.lvIniFile.Location = new System.Drawing.Point(280, 8);
			this.lvIniFile.Name = "lvIniFile";
			this.lvIniFile.Size = new System.Drawing.Size(160, 144);
			this.lvIniFile.TabIndex = 1;
			this.lvIniFile.View = System.Windows.Forms.View.Details;
			this.lvIniFile.DoubleClick += new System.EventHandler(this.lvIniFile_DoubleClick);
			this.lvIniFile.SelectedIndexChanged += new System.EventHandler(this.lvIniFile_SelectedIndexChanged);
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "INI File";
			this.columnHeader3.Width = 149;
			// 
			// cmIniFiles
			// 
			this.cmIniFiles.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					   this.miView});
			this.cmIniFiles.Popup += new System.EventHandler(this.cmIniFiles_Popup);
			// 
			// miView
			// 
			this.miView.Index = 0;
			this.miView.Text = "View";
			this.miView.Click += new System.EventHandler(this.miView_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnCancel.Location = new System.Drawing.Point(360, 160);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.Enabled = false;
			this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnOK.Location = new System.Drawing.Point(280, 160);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 3;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnLocate
			// 
			this.btnLocate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnLocate.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnLocate.Location = new System.Drawing.Point(16, 160);
			this.btnLocate.Name = "btnLocate";
			this.btnLocate.Size = new System.Drawing.Size(88, 23);
			this.btnLocate.TabIndex = 4;
			this.btnLocate.Text = "Locate new...";
			this.btnLocate.Click += new System.EventHandler(this.btnLocate_Click);
			// 
			// btnRemove
			// 
			this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnRemove.Location = new System.Drawing.Point(112, 160);
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.Size = new System.Drawing.Size(72, 23);
			this.btnRemove.TabIndex = 5;
			this.btnRemove.Text = "Remove";
			this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
			// 
			// MOGIniSelector
			// 
			this.Controls.Add(this.btnRemove);
			this.Controls.Add(this.btnLocate);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.lvIniFile);
			this.Controls.Add(this.lvRepositories);
			this.Name = "MOGIniSelector";
			this.Size = new System.Drawing.Size(448, 192);
			this.ResumeLayout(false);

		}
		#endregion
		#endregion
		#region Member vars
		private string lastSelectedPath;
		#endregion
		#region Properties
		public ArrayList RepositoryNames
		{
			get 
			{
				ArrayList names = new ArrayList();
				foreach (ListViewItem item in this.lvRepositories.Items)
					names.Add( item.Text );
				
				return names;
			}
		}
		public ArrayList RepositoryPaths
		{
			get 
			{
				ArrayList paths = new ArrayList();
				foreach (ListViewItem item in this.lvRepositories.Items)
					paths.Add( item.SubItems[1].Text );

				return paths;
			}
		}

		public bool OKButtonVisible
		{
			get { return this.btnOK.Visible; }
			set { this.btnOK.Visible = value; }
		}
		public bool CancelButtonVisible
		{
			get { return this.btnCancel.Visible; }
			set { this.btnCancel.Visible = value; }
		}
		public bool LocateButtonVisible
		{
			get { return this.btnLocate.Visible; }
			set { this.btnLocate.Visible = true; }
		}
		public string LocateButtonText
		{
			get { return this.btnLocate.Text; }
			set { this.btnLocate.Text = value; }
		}
		public bool RemoveButtonVisible
		{
			get { return this.btnRemove.Visible; }
			set { this.btnRemove.Visible = value; }
		}
		public string SelectedINIPath
		{
			get 
			{
				if (this.lvRepositories.SelectedItems.Count <= 0)
					return "";
				if (this.lvIniFile.SelectedItems.Count <= 0)
					return "";

				return this.lvRepositories.SelectedItems[0].SubItems[1].Text + "\\Tools\\" + this.lvIniFile.SelectedItems[0].Text;
			}
		}

		public string SelectedINIFilename
		{
			get 
			{
				if (this.lvIniFile.SelectedItems.Count <= 0)
					return MOG_Main.GetDefaultSystemConfigFilenameDefine();

				return this.lvIniFile.SelectedItems[0].Text;
			}
		}
		public string SelectedRepositoryPath
		{
			get 
			{
				if (this.lvRepositories.SelectedItems.Count <= 0)
					return "";

				return this.lvRepositories.SelectedItems[0].SubItems[1].Text;
			}
		}
		#endregion
		#region Events
		public event EventHandler OKClicked;
		public event EventHandler CancelClicked;
		public event MOGIniSelectorEventHandler SelectedRepositoryChanged;
		#endregion
		#region Constructors
		public MOGIniSelector()
		{
			InitializeComponent();
		}

		public MOGIniSelector( ArrayList names, ArrayList paths )
		{
			InitializeComponent();

			if (names.Count == paths.Count)
			{
				for (int i=0; i<names.Count; i++)
					AddRepository((string)names[i], (string)paths[i]);
			}
		}
		#endregion
		#region Public member functions

		private const int DRIVE_TYPE_CD			= 5;
		private const int DRIVE_TYPE_HD			= 3;
		private const int DRIVE_TYPE_RAM		= 6;
		private const int DRIVE_TYPE_NETWORK	= 4;
		private const int DRIVE_TYPE_FLOPPY		= 2;

		[DllImport("kernel32.dll")]
			//TYPE:		
			//5-A CD-ROM drive. 
			//3-A hard drive. 
			//6-A RAM disk. 
			//4-A network drive or a drive located on a network server. 
			//2-A floppy drive or some other removable-disk drive. 
		public static extern long GetDriveType(string driveLetter);

		public void LoadShallowRepositories()
		{
			// get rid of old drives
			this.lvRepositories.Items.Clear();

			AddShallowRepositories();
		}

		public void AddShallowRepositories()
		{
			this.lvIniFile.Items.Clear();

			foreach (string drive in Directory.GetLogicalDrives())
			{
				int type = (int)GetDriveType(drive);
				if (type == DRIVE_TYPE_HD  ||  type == DRIVE_TYPE_NETWORK)
				{
					if (Directory.Exists(drive))
					{
						// look for a repository marker
						if (File.Exists(drive + "MogRepository.ini"))
						{
							// repository exists, open it up
							MOG_Ini ini = new MOG_Ini(drive + "MogRepository.ini");
							if (!ini.SectionExist("MOG_REPOSITORIES"))
								continue;

							for (int sectionIndex = 0; sectionIndex < ini.CountKeys("MOG_REPOSITORIES"); sectionIndex++)
							{
								string sectionName = ini.GetKeyNameByIndexSLOW("MOG_REPOSITORIES", sectionIndex);
								if (ini.SectionExist(sectionName)  &&  ini.KeyExist(sectionName, "SystemRepositoryPath"))
								{
									ListViewItem item = new ListViewItem(sectionName + " on " + drive.Trim("\\".ToCharArray()));
									item.SubItems.Add(ini.GetString(sectionName, "SystemRepositoryPath"));
									this.lvRepositories.Items.Add(item);
								}
							}

							ini.Close();
						}
					}
				}
			}
		}

		public ListViewItem AddRepository(string name, string path)
		{
			if ( !Directory.Exists(path + "\\Tools") )
				return null;

			foreach (ListViewItem item in this.lvRepositories.Items)
			{
				if (item.Text.ToLower() == name.ToLower()  &&  item.SubItems[1].Text.ToLower() == path.ToLower())
					return null;
			}

			ListViewItem newItem = new ListViewItem(name);
			newItem.SubItems.Add(path);
			this.lvRepositories.Items.Add(newItem);

			Utils.AutoSizeListViewColumns(this.lvRepositories);
			return newItem;
		}

		public bool LoadRepositories(string iniFilename)
		{
			if (!Directory.Exists( Path.GetDirectoryName(iniFilename) ))
				return false;

			MOG_Ini ini = new MOG_Ini(iniFilename);
			if (ini == null)
				return false;

			if (ini.SectionExist("REPOSITORIES"))
			{
				for (int i=0; i<ini.CountKeys("REPOSITORIES"); i++)
				{
					string key = ini.GetKeyNameByIndexSLOW("REPOSITORIES", i);
					if (key.ToLower() == "default")
						continue;

					if (!ini.SectionExist("REPOSITORIES." + key))
						continue;

					string name = ini.GetString("REPOSITORIES."+key, "name");
					string path = ini.GetString("REPOSITORIES."+key, "path");
					AddRepository(name, path);
				}
			}

			ini.Close();
			return true;
		}

		public bool SaveRepositories(string iniFilename)
		{
			if (!Directory.Exists( Path.GetDirectoryName(iniFilename) ))
				return false;

			MOG_Ini ini = new MOG_Ini(iniFilename);
			if (ini == null)
				return false;

			// remove all old repository data
			int i;
			if (ini.SectionExist("REPOSITORIES"))
			{
				for (i=0; i<ini.CountKeys("REPOSITORIES"); i++)
				{
					// skip default
					if (ini.GetKeyNameByIndexSLOW("REPOSITORIES", i).ToLower() == "default")
						continue;

					ini.RemoveSection("REPOSITORIES." + ini.GetKeyNameByIndexSLOW("REPOSITORIES", i));
				}
				ini.RemoveSection("REPOSITORIES");
			}

			// and save the new data
			i=0;
			foreach (ListViewItem item in this.lvRepositories.Items)
			{
				string mrString = "mr" + i.ToString();
				ini.PutString("REPOSITORIES", mrString, "");
				ini.PutString("REPOSITORIES." + mrString, "name", item.Text);
				ini.PutString("REPOSITORIES." + mrString, "path", item.SubItems[1].Text);

				if (item == this.lvRepositories.SelectedItems[0])
				{
					// this one's the default
					ini.PutString("REPOSITORIES", "default", mrString);

					// is there an INI selected in lvIniFiles?
					if (this.lvIniFile.SelectedItems.Count > 0)
						ini.PutString("REPOSITORIES." + mrString, "ini", item.SubItems[1].Text + "\\Tools\\" + this.lvIniFile.SelectedItems[0].Text);
					else
						ini.PutString("REPOSITORIES." + mrString, "ini", item.SubItems[1].Text + "\\" + MOG_Main.GetDefaultSystemRelativeConfigFileDefine());
				}
				else
					ini.PutString("REPOSITORIES." + "mr" + i.ToString(), "ini", item.SubItems[1].Text + "\\" + MOG_Main.GetDefaultSystemRelativeConfigFileDefine());

				++i;
			}

			ini.Save();
			ini.Close();
			return true;
		}
		#endregion
		#region Private member functions
		private void OnOKClicked()
		{
			if (this.OKClicked != null)
				this.OKClicked(this, new EventArgs());
		}

		private void OnCancelClicked()
		{
			if (this.CancelClicked != null)
				this.CancelClicked(this, new EventArgs());
		}

		private void OnSelectedRepositoryChanged(string path, string name, string ini)
		{
			if (this.SelectedRepositoryChanged != null)
			{
				MOGIniSelectorEventArgs args = new MOGIniSelectorEventArgs();
				args.selectedPath = path;
				args.selectedName = name;
				args.selectedIni = ini;
				this.SelectedRepositoryChanged(this, args);
			}
		}
		#endregion
		#region Event handlers
		private void btnOK_Click(object sender, System.EventArgs e)
		{
			OnOKClicked();
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			OnCancelClicked();
		}

		private void lvRepositories_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			LoadINIs();
		}

		private void lvRepositories_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			LoadINIs();
		}

		private void LoadINIs()
		{
			if (this.lvRepositories.SelectedItems.Count > 0)
			{
				this.btnOK.Enabled = false;
				this.lvIniFile.Enabled = true;
				this.lvIniFile.Items.Clear();
				this.lvIniFile.Focus();
				string selectedToolsPath = this.lvRepositories.SelectedItems[0].SubItems[1].Text + "\\Tools";
				if (Directory.Exists(selectedToolsPath))
				{
					foreach (string iniName in Directory.GetFiles(selectedToolsPath, "*.ini"))
						this.lvIniFile.Items.Add( Path.GetFileName(iniName) );
				}
				
				if (this.lvIniFile.Items.Count > 0)
					this.lvIniFile.Items[0].Selected = true;
			}
			else
			{
				this.btnOK.Enabled = false;
				this.lvIniFile.Items.Clear();
				this.lvIniFile.Enabled = false;
			}
		}

		private void btnLocate_Click(object sender, System.EventArgs e)
		{
			FolderBrowserDialog fbd = new FolderBrowserDialog();
			fbd.SelectedPath = this.lastSelectedPath;

			if (fbd.ShowDialog() == DialogResult.OK)
			{
				if (!Directory.Exists(fbd.SelectedPath))
					return;
				if (!Directory.Exists(fbd.SelectedPath + "\\Tools"))
				{
					Utils.ShowMessageBoxExclamation(fbd.SelectedPath + " is not a valid MOG Repository", "Invalid Repository");
					return;
				}

				ListViewItem item = AddRepository("New repository", fbd.SelectedPath);
				item.Selected = true;
				this.lastSelectedPath = fbd.SelectedPath;
			}
		}

		private void btnRemove_Click(object sender, System.EventArgs e)
		{
			foreach (ListViewItem item in this.lvRepositories.SelectedItems)
				item.Remove();
		}

		private void cmRepositories_Popup(object sender, System.EventArgs e)
		{
			if (this.lvRepositories.SelectedItems.Count > 0)
			{
				this.miLocateNew.Enabled = false;
				this.miRemove.Enabled = true;
				this.miBrowseToRepository.Enabled = true;
				this.miRename.Enabled = true;
			}
			else
			{
				this.miLocateNew.Enabled = true;
				this.miRemove.Enabled = false;
				this.miBrowseToRepository.Enabled = false;
				this.miRename.Enabled = false;
			}
		}

		private void cmIniFiles_Popup(object sender, System.EventArgs e)
		{
			if (this.lvIniFile.SelectedItems.Count > 0)
				this.miView.Enabled = true;
			else
				this.miView.Enabled = false;
		}

		private void miLocateNew_Click(object sender, System.EventArgs e)
		{
			this.btnLocate.PerformClick();
		}

		private void miRemove_Click(object sender, System.EventArgs e)
		{
			this.btnRemove.PerformClick();
		}

		private void miView_Click(object sender, System.EventArgs e)
		{
			if (this.lvRepositories.SelectedItems.Count > 0)
			{
				foreach (ListViewItem item in this.lvIniFile.SelectedItems)
					Utils.RunCommand("notepad.exe", this.lvRepositories.SelectedItems[0].SubItems[1].Text + "\\Tools\\" + item.Text);
			}
		}

		private void lvIniFile_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.lvIniFile.SelectedItems.Count > 0)
			{
				this.btnOK.Enabled = true;
				OnSelectedRepositoryChanged(this.lvRepositories.SelectedItems[0].SubItems[1].Text, this.lvRepositories.SelectedItems[0].SubItems[0].Text, this.lvIniFile.SelectedItems[0].Text);
			}
			else
			{
				this.btnOK.Enabled = false;
				OnSelectedRepositoryChanged(this.lvRepositories.SelectedItems[0].SubItems[1].Text, this.lvRepositories.SelectedItems[0].SubItems[0].Text, MOG_Main.GetDefaultSystemConfigFilenameDefine());
			}
		}

		private void lvIniFile_DoubleClick(object sender, System.EventArgs e)
		{
			this.btnOK.PerformClick();
		}

		private void lvRepositories_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyData == Keys.F2  &&  this.lvRepositories.SelectedItems.Count > 0)
				this.lvRepositories.SelectedItems[0].BeginEdit();
		}

		private void lvRepositories_AfterLabelEdit(object sender, System.Windows.Forms.LabelEditEventArgs e)
		{
			if (e.Label == "")
			{
				Utils.ShowMessageBoxExclamation("You must specify a name for this repository", "Missing Name");
				e.CancelEdit = true;
			}
		}

		private void miBrowseToRepository_Click(object sender, System.EventArgs e)
		{
			if (this.lvRepositories.SelectedItems.Count > 0)
				Utils.RunCommand("explorer.exe", this.lvRepositories.SelectedItems[0].SubItems[1].Text);
		}

		private void lvRepositories_DoubleClick(object sender, System.EventArgs e)
		{
			this.btnOK.PerformClick();
		}

		private void miRename_Click(object sender, System.EventArgs e)
		{
			if (this.lvRepositories.SelectedItems.Count > 0)
				this.lvRepositories.SelectedItems[0].BeginEdit();
		}

		private void lvRepositories_ItemDrag(object sender, System.Windows.Forms.ItemDragEventArgs e)
		{
			if (this.lvRepositories.SelectedItems.Count > 0)
			{
				ArrayList selected = new ArrayList();
				foreach (ListViewItem item in this.lvRepositories.SelectedItems)
					selected.Add(item);
				this.lvRepositories.DoDragDrop(selected, DragDropEffects.Move);
			}
		}

		private void lvRepositories_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
		{
			if (e.Data.GetDataPresent("System.Collections.ArrayList"))
				e.Effect = e.AllowedEffect;
		}

		private void lvRepositories_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{
			ArrayList items = (ArrayList)e.Data.GetData("System.Collections.ArrayList");
			if (items != null)
			{
				Point p = this.lvRepositories.PointToClient( new Point(e.X, e.Y) );
				ListViewItem targetItem = this.lvRepositories.GetItemAt( p.X, p.Y );

				foreach (ListViewItem dropItem in items)
				{
					dropItem.Remove();
					int index = targetItem.Index+1;
					if (index >= this.lvRepositories.Items.Count)
						index = this.lvRepositories.Items.Count-1;

					this.lvRepositories.Items.Insert(index, dropItem);
				}
			}
		}

		private void miMoveToTop_Click(object sender, System.EventArgs e)
		{
			if (this.lvRepositories.SelectedItems.Count > 0)
			{
				ListViewItem item = this.lvRepositories.SelectedItems[0];
				item.Remove();
				this.lvRepositories.Items.Insert(0, item);
			}
		}

		private void miMoveToBottom_Click(object sender, System.EventArgs e)
		{
			if (this.lvRepositories.SelectedItems.Count > 0)
			{
				ListViewItem item = this.lvRepositories.SelectedItems[0];
				item.Remove();
				this.lvRepositories.Items.Insert( this.lvRepositories.Items.Count, item );
			}		
		}



	}
		#endregion
	
	public delegate void MOGIniSelectorEventHandler(object sender, MOGIniSelectorEventArgs e);
	public class MOGIniSelectorEventArgs : EventArgs
	{
		public string selectedPath;
		public string selectedName;
		public string selectedIni;
	}
}


/*
#region System definitions
#endregion
#region Member vars
#endregion
#region Properties
#endregion
#region Constructors
#endregion
#region Member functions
#endregion
#region Event handlers
#endregion
*/ 
