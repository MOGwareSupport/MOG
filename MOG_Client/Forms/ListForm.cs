using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

using EV.Windows.Forms;

using MOG_Client.Client_Mog_Utilities.AssetOptions;
using MOG_Client.Client_Mog_Utilities;
using MOG_ControlsLibrary.Utils;
using MOG_ControlsLibrary.Common.MOG_ContextMenu;
using MOG_ControlsLibrary.Common.MogControl_RepositoryTreeViews;
using MOG_ControlsLibrary.Controls;
using MOG_CoreControls;

using MOG;
using MOG.INI;
using MOG.TIME;
using MOG.REPORT;
using MOG.PROMPT;
using MOG.PROGRESS;
using MOG.PROJECT;
using MOG.PROPERTIES;
using MOG.FILENAME;
using MOG.DOSUTILS;
using MOG.CONTROLLER;
using MOG.DATABASE;
using MOG.CONTROLLER.CONTROLLERASSET;
using MOG.CONTROLLER.CONTROLLERREPOSITORY;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.CONTROLLER.CONTROLLERPACKAGE;
using System.Collections.Generic;
using MOG_ControlsLibrary.MogUtils_Settings;



namespace MOG_Client.Forms
{
	/// <summary>
	/// Summary description for ListForm.
	/// This form is used for the 'Generate Report' command in mog.
	/// </summary>
	public class ListForm : System.Windows.Forms.Form
	{
		private enum ASSET_LIST {NAME, CLASSIFICATION, PLATFORM, VERSION, SIZE, PACKAGE, CREATOR, OWNER, ASSOC_ASSETS, LAST_COMMENT, GAMEPATH, FAILED_STRING, FULLNAME};
		private enum EXPORTS {EXCEL, TEXT};
		private ArrayList mListViewSort_Manager;

		#region Form variables
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		public System.Windows.Forms.ListView ListListView;
		private OOGroup.Windows.Forms.ImageButton ListExcelButton;
		private OOGroup.Windows.Forms.ImageButton ListNotpadButton;
		private OOGroup.Windows.Forms.ImageButton ListSaveButton;
		private System.Windows.Forms.Button ListOkButton;
		private System.Windows.Forms.SaveFileDialog ListSaveFileDialog;
		private System.Windows.Forms.ImageList ListImageList;
		private System.Windows.Forms.ToolTip ToolTip;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label AssetListTotalLabel;
		private System.Windows.Forms.Label AssetListSelectedTotalLabel;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ColumnHeader ReportPackageColumnHeader;
		private System.Windows.Forms.ColumnHeader ReportNameColumnHeader;
		private System.Windows.Forms.ColumnHeader ReportPlatformColumnHeader;
		private System.Windows.Forms.ColumnHeader ReportVersionColumnHeader;
		private System.Windows.Forms.ColumnHeader ReportSizeColumnHeader;
		private System.Windows.Forms.ColumnHeader ReportExtensionColumnHeader;
		private System.Windows.Forms.ColumnHeader ReportCreatorColumnHeader;
		private System.Windows.Forms.ColumnHeader ReportLastBlessColumnHeader;
		private System.Windows.Forms.ColumnHeader ReportAssetsColumnHeader;
		private System.Windows.Forms.ColumnHeader ReportFailedCheckColumnHeader;
		private System.Windows.Forms.ColumnHeader ReportClassColumnHeader;
		private System.Windows.Forms.ColumnHeader ReportLastCommentColumnHeader;
		private MOG_XpProgressBar ListProgressBar;
		private ColumnHeader ReportComputerColumnHeader;
		private ColumnHeader ReportFullNameColumnHeader;
		private System.ComponentModel.IContainer components;
		#endregion

		public ListForm(string title)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.ListListView.SmallImageList = MogUtil_AssetIcons.Images;

			// Assets
			mListViewSort_Manager = new ArrayList();
			ListViewSortManager sorter = new ListViewSortManager(ListListView, new Type[] {
																							  typeof(ListViewTextCaseInsensitiveSort),
																							  typeof(ListViewTextCaseInsensitiveSort),
																							  typeof(ListViewTextCaseInsensitiveSort),
																							  typeof(ListViewDateSort),
																							  typeof(ListViewStringSizeSort),
																							  typeof(ListViewTextCaseInsensitiveSort),
																							  typeof(ListViewTextCaseInsensitiveSort),
																							  typeof(ListViewTextCaseInsensitiveSort),
																							  typeof(ListViewTextCaseInsensitiveSort),
																							  typeof(ListViewTextCaseInsensitiveSort),
																								typeof(ListViewTextCaseInsensitiveSort),
																								typeof(ListViewTextCaseInsensitiveSort),
																							  typeof(ListViewTextCaseInsensitiveSort)
																						  });

			mListViewSort_Manager.Add(sorter);

			// Create hash tokens for column names
			foreach (ColumnHeader col in ListListView.Columns)
			{
				col.Name = col.Text;
			}

			Text = title;

			// Initialize the archive context menu
			MogControl_AssetContextMenu mAssetContextMenu = new MogControl_AssetContextMenu(ListListView.Columns ,ListListView);
			ListListView.ContextMenuStrip = mAssetContextMenu.InitializeContextMenu("{report}");
			
			// Initialize special graphic buttons
			Bitmap home = new Bitmap(ListImageList.Images[0]);
			ListExcelButton.SetImage(home);
			home = new Bitmap(ListImageList.Images[1]);
			ListNotpadButton.SetImage(home);
			home = new Bitmap(ListImageList.Images[3]);
			ListSaveButton.SetImage(home);

			ListListView.ContextMenuStrip.Opening += new CancelEventHandler(ContextMenuStrip_Opening);
		}

		/// <summary>
		/// Disables the context menus of the report form if we are in light version
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void ContextMenuStrip_Opening(object sender, CancelEventArgs e)
		{
			MogUtil_VersionInfo.SetLightVersionControl(ListListView.ContextMenuStrip); 
		}
				
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ListForm));
			this.panel1 = new System.Windows.Forms.Panel();
			this.ListProgressBar = new MOG_CoreControls.MOG_XpProgressBar();
			this.AssetListSelectedTotalLabel = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.AssetListTotalLabel = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.ListOkButton = new System.Windows.Forms.Button();
			this.panel2 = new System.Windows.Forms.Panel();
			this.ListListView = new System.Windows.Forms.ListView();
			this.ReportNameColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.ReportClassColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.ReportPlatformColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.ReportVersionColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.ReportSizeColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.ReportPackageColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.ReportCreatorColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.ReportLastBlessColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.ReportAssetsColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.ReportLastCommentColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.ReportExtensionColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.ReportFailedCheckColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.ListSaveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.ListImageList = new System.Windows.Forms.ImageList(this.components);
			this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
			this.ReportComputerColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.ReportFullNameColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.ListSaveButton = new OOGroup.Windows.Forms.ImageButton();
			this.ListNotpadButton = new OOGroup.Windows.Forms.ImageButton();
			this.ListExcelButton = new OOGroup.Windows.Forms.ImageButton();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.ListProgressBar);
			this.panel1.Controls.Add(this.AssetListSelectedTotalLabel);
			this.panel1.Controls.Add(this.label3);
			this.panel1.Controls.Add(this.AssetListTotalLabel);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.ListOkButton);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 379);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(1287, 48);
			this.panel1.TabIndex = 0;
			// 
			// ListProgressBar
			// 
			this.ListProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.ListProgressBar.ColorBackGround = System.Drawing.SystemColors.Control;
			this.ListProgressBar.ColorBarBorder = System.Drawing.Color.LightSkyBlue;
			this.ListProgressBar.ColorBarCenter = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(151)))), ((int)(((byte)(245)))));
			this.ListProgressBar.ColorText = System.Drawing.Color.Black;
			this.ListProgressBar.Location = new System.Drawing.Point(8, 1);
			this.ListProgressBar.Name = "ListProgressBar";
			this.ListProgressBar.Position = 0;
			this.ListProgressBar.PositionMax = 100;
			this.ListProgressBar.PositionMin = 0;
			this.ListProgressBar.Size = new System.Drawing.Size(1200, 12);
			this.ListProgressBar.SteepDistance = ((byte)(0));
			this.ListProgressBar.TabIndex = 8;
			// 
			// AssetListSelectedTotalLabel
			// 
			this.AssetListSelectedTotalLabel.Location = new System.Drawing.Point(192, 24);
			this.AssetListSelectedTotalLabel.Name = "AssetListSelectedTotalLabel";
			this.AssetListSelectedTotalLabel.Size = new System.Drawing.Size(72, 23);
			this.AssetListSelectedTotalLabel.TabIndex = 7;
			this.AssetListSelectedTotalLabel.Text = "0";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(112, 24);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(80, 23);
			this.label3.TabIndex = 6;
			this.label3.Text = "Selected Total:";
			// 
			// AssetListTotalLabel
			// 
			this.AssetListTotalLabel.Location = new System.Drawing.Point(48, 24);
			this.AssetListTotalLabel.Name = "AssetListTotalLabel";
			this.AssetListTotalLabel.Size = new System.Drawing.Size(56, 23);
			this.AssetListTotalLabel.TabIndex = 5;
			this.AssetListTotalLabel.Text = "0";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(40, 23);
			this.label1.TabIndex = 4;
			this.label1.Text = "Totals:";
			// 
			// ListOkButton
			// 
			this.ListOkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.ListOkButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.ListOkButton.Location = new System.Drawing.Point(1204, 21);
			this.ListOkButton.Name = "ListOkButton";
			this.ListOkButton.Size = new System.Drawing.Size(75, 23);
			this.ListOkButton.TabIndex = 2;
			this.ListOkButton.Text = "Close";
			this.ListOkButton.Click += new System.EventHandler(this.ListOkButton_Click);
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.ListSaveButton);
			this.panel2.Controls.Add(this.ListNotpadButton);
			this.panel2.Controls.Add(this.ListExcelButton);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel2.Location = new System.Drawing.Point(1247, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(40, 379);
			this.panel2.TabIndex = 1;
			// 
			// ListListView
			// 
			this.ListListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ReportNameColumnHeader,
            this.ReportClassColumnHeader,
            this.ReportPlatformColumnHeader,
            this.ReportVersionColumnHeader,
            this.ReportSizeColumnHeader,
            this.ReportPackageColumnHeader,
            this.ReportCreatorColumnHeader,
            this.ReportLastBlessColumnHeader,
            this.ReportComputerColumnHeader,
            this.ReportLastCommentColumnHeader,
            this.ReportAssetsColumnHeader,
            this.ReportExtensionColumnHeader,
            this.ReportFailedCheckColumnHeader,
            this.ReportFullNameColumnHeader});
			this.ListListView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ListListView.FullRowSelect = true;
			this.ListListView.Location = new System.Drawing.Point(0, 0);
			this.ListListView.Name = "ListListView";
			this.ListListView.Size = new System.Drawing.Size(1247, 379);
			this.ListListView.TabIndex = 2;
			this.ListListView.UseCompatibleStateImageBehavior = false;
			this.ListListView.View = System.Windows.Forms.View.Details;
			this.ListListView.SelectedIndexChanged += new System.EventHandler(this.ListListView_SelectedIndexChanged);
			this.ListListView.DoubleClick += new System.EventHandler(this.ListListView_DoubleClick);
			// 
			// ReportNameColumnHeader
			// 
			this.ReportNameColumnHeader.Text = "Name";
			this.ReportNameColumnHeader.Width = 103;
			// 
			// ReportClassColumnHeader
			// 
			this.ReportClassColumnHeader.Text = "MOG Classification";
			this.ReportClassColumnHeader.Width = 154;
			// 
			// ReportPlatformColumnHeader
			// 
			this.ReportPlatformColumnHeader.Text = "Platform";
			this.ReportPlatformColumnHeader.Width = 54;
			// 
			// ReportVersionColumnHeader
			// 
			this.ReportVersionColumnHeader.Text = "Version";
			this.ReportVersionColumnHeader.Width = 113;
			// 
			// ReportSizeColumnHeader
			// 
			this.ReportSizeColumnHeader.Text = "Size";
			this.ReportSizeColumnHeader.Width = 48;
			// 
			// ReportPackageColumnHeader
			// 
			this.ReportPackageColumnHeader.Text = "Package";
			this.ReportPackageColumnHeader.Width = 100;
			// 
			// ReportCreatorColumnHeader
			// 
			this.ReportCreatorColumnHeader.Text = "Creator";
			this.ReportCreatorColumnHeader.Width = 58;
			// 
			// ReportLastBlessColumnHeader
			// 
			this.ReportLastBlessColumnHeader.Text = "Last Bless";
			this.ReportLastBlessColumnHeader.Width = 68;
			// 
			// ReportAssetsColumnHeader
			// 
			this.ReportAssetsColumnHeader.Text = "Assoc Assets";
			this.ReportAssetsColumnHeader.Width = 100;
			// 
			// ReportLastCommentColumnHeader
			// 
			this.ReportLastCommentColumnHeader.Text = "Last Comment";
			this.ReportLastCommentColumnHeader.Width = 200;
			// 
			// ReportExtensionColumnHeader
			// 
			this.ReportExtensionColumnHeader.Text = "GamePath";
			this.ReportExtensionColumnHeader.Width = 207;
			// 
			// ReportFailedCheckColumnHeader
			// 
			this.ReportFailedCheckColumnHeader.Text = "Failed Check";
			this.ReportFailedCheckColumnHeader.Width = 80;
			// 
			// ListSaveFileDialog
			// 
			this.ListSaveFileDialog.DefaultExt = "rep";
			this.ListSaveFileDialog.Filter = "Mog Report | *.rep";
			this.ListSaveFileDialog.Title = "Save List Report";
			// 
			// ListImageList
			// 
			this.ListImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ListImageList.ImageStream")));
			this.ListImageList.TransparentColor = System.Drawing.Color.Transparent;
			this.ListImageList.Images.SetKeyName(0, "");
			this.ListImageList.Images.SetKeyName(1, "");
			this.ListImageList.Images.SetKeyName(2, "");
			this.ListImageList.Images.SetKeyName(3, "");
			// 
			// ReportComputerColumnHeader
			// 
			this.ReportComputerColumnHeader.Text = "Computer";
			// 
			// ReportFullNameColumnHeader
			// 
			this.ReportFullNameColumnHeader.Text = "Fullname";
			this.ReportFullNameColumnHeader.Width = 0;
			// 
			// ListSaveButton
			// 
			this.ListSaveButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.ListSaveButton.Location = new System.Drawing.Point(8, 64);
			this.ListSaveButton.Name = "ListSaveButton";
			this.ListSaveButton.Size = new System.Drawing.Size(24, 23);
			this.ListSaveButton.TabIndex = 3;
			this.ToolTip.SetToolTip(this.ListSaveButton, "Save this list for future use");
			this.ListSaveButton.Click += new System.EventHandler(this.ListSaveButton_Click);
			// 
			// ListNotpadButton
			// 
			this.ListNotpadButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.ListNotpadButton.Location = new System.Drawing.Point(8, 40);
			this.ListNotpadButton.Name = "ListNotpadButton";
			this.ListNotpadButton.Size = new System.Drawing.Size(24, 23);
			this.ListNotpadButton.TabIndex = 1;
			this.ToolTip.SetToolTip(this.ListNotpadButton, "Export selected items to a text file");
			this.ListNotpadButton.Click += new System.EventHandler(this.ListNotpadButton_Click);
			// 
			// ListExcelButton
			// 
			this.ListExcelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.ListExcelButton.Location = new System.Drawing.Point(8, 16);
			this.ListExcelButton.Name = "ListExcelButton";
			this.ListExcelButton.Size = new System.Drawing.Size(24, 23);
			this.ListExcelButton.TabIndex = 0;
			this.ToolTip.SetToolTip(this.ListExcelButton, "Export selected items to Excel");
			this.ListExcelButton.Click += new System.EventHandler(this.ListExcelButton_Click);
			// 
			// ListForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(1287, 427);
			this.Controls.Add(this.ListListView);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.Name = "ListForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Custom Asset List";
			this.Load += new System.EventHandler(this.ListForm_Load);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ListForm_KeyUp);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ListForm_FormClosing);
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
		
		#region Report Exporting
		/// <summary>
		/// Get a unique auto generated filename for this reportFile
		/// </summary>
		/// <param name="path"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		private string CreateUniqueFilename(string path, string name)
		{
			MOG_Time time = new MOG_Time();
			return path + "\\" + name.Substring(0, name.LastIndexOf(".")) + "   " + time.FormatString("{Month.2}-{Day.2}-{Year.2} {Hour.2} {Minute.2} {Second.2} {AMPM}") + name.Substring(name.LastIndexOf("."));
		}
		
		/// <summary>
		/// Core export function that handles all the actual exporting
		/// </summary>
		private void ExportTo(EXPORTS format, ArrayList items)
		{
			List<object> args = new List<object>();
			args.Add(items);
			args.Add(format);

			// Init dialog window
			ProgressDialog progress = new ProgressDialog("Exporting Asset List to Excel", "Please wait while MOG exports the report...", ExportTo_Worker, args, true);
			if (progress.ShowDialog() == DialogResult.OK)
			{
				string filename = progress.WorkerResult as string;

				if (DosUtils.FileExist(filename))
				{
					guiCommandLine.ShellSpawn(filename);
				}
			}
		}

		void ExportTo_Worker(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			List<object> args = e.Argument as List<object>;
			ArrayList items = args[0] as ArrayList;
			EXPORTS format = (EXPORTS)args[1];

			string tempListFilename;

			switch (format)
			{
			case EXPORTS.EXCEL:
				tempListFilename = CreateUniqueFilename(MOG_ControllerProject.GetUser().GetUserToolsPath(), "AssetList.slk");
				break;
			case EXPORTS.TEXT:
				tempListFilename = CreateUniqueFilename(MOG_ControllerProject.GetUser().GetUserToolsPath(), "AssetList.txt");
				break;
			default:
				tempListFilename = "";
				break;
			}

			// Clear out any previous temp file
			if (DosUtils.FileExist(tempListFilename))
			{
				DosUtils.FileDelete(tempListFilename);
			}
			
			for (int i = 0; i < items.Count; i++)
			{
				ListViewItem item = items[i] as ListViewItem;

				string message = "Adding:\n" +
								 "     " + item.SubItems[(int)ASSET_LIST.NAME].Text;
				worker.ReportProgress(i * 100 / items.Count, message);

				string text = item.SubItems[(int)ASSET_LIST.NAME].Text + "\t" +
					item.SubItems[(int)ASSET_LIST.CLASSIFICATION].Text + "\t" +
					item.SubItems[(int)ASSET_LIST.PLATFORM].Text + "\t" +
					item.SubItems[(int)ASSET_LIST.VERSION].Text + "\t" +
					item.SubItems[(int)ASSET_LIST.SIZE].Text + "\t" +
					item.SubItems[(int)ASSET_LIST.OWNER].Text;

				switch (format)
				{
				case EXPORTS.EXCEL:
					DosUtils.AppendTextToSlkFile(tempListFilename, text);
					break;
				case EXPORTS.TEXT:
					DosUtils.AppendTextToFile(tempListFilename, text + "\r\n");
					break;
				default:
					break;
				}
			}

			if (format == EXPORTS.EXCEL)
			{
				DosUtils.FileCloseSlk(tempListFilename);
			}

			e.Result = tempListFilename;
		}

		/// <summary>
		/// Export out to Excel format
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ListExcelButton_Click(object sender, System.EventArgs e)
		{
			if (ListListView.SelectedItems.Count == 0)
			{
				MOG_Prompt.PromptMessage("No assets selected", "There are no assets selected to export to Excel.", Environment.StackTrace);
				return;
			}

			ArrayList items = new ArrayList();
			foreach (ListViewItem item in ListListView.SelectedItems)
			{
				items.Add(item);
			}
			ExportTo(EXPORTS.EXCEL, items);
		}

		/// <summary>
		/// Export to .txt file
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ListNotpadButton_Click(object sender, System.EventArgs e)
		{
			if (ListListView.SelectedItems.Count == 0)
			{
				MOG_Prompt.PromptMessage("No assets selected", "There are no assets selected to export to text.", Environment.StackTrace);
				return;
			}

			ArrayList items = new ArrayList();
			foreach (ListViewItem item in ListListView.SelectedItems)
			{
				items.Add(item);
			}
			ExportTo(EXPORTS.TEXT, items);
		}
		#endregion

		#region Load / Save reports
		/// <summary>
		/// Load a report form from a file and populate it
		/// </summary>
		/// <param name="filename"></param>
		public void LoadReportList(string filename)
		{
			MOG_Ini report = new MOG_Ini(filename);
			
			// Set the form title
			Text = Path.GetFileName(filename);
			
			if (report.SectionExist("ASSETS"))
			{
				ListListView.Items.Clear();

				ListListView.BeginUpdate();

				ProgressMax(report.CountKeys("ASSETS"));

				for (int x = 0; x < report.CountKeys("ASSETS"); x++)
				{
					MOG_Filename mogAsset = new MOG_Filename(report.GetKeyNameByIndexSLOW("ASSETS", x));
					string extraInfo = report.GetKeyByIndexSLOW("ASSETS", x);
					
					MOG_Properties pProperties = new MOG_Properties(mogAsset);
					
					string version = mogAsset.GetVersionTimeStamp();
					string currentVersion = MOG_DBAssetAPI.GetAssetVersion(mogAsset);//mCurrentInfo.GetString("ASSETS", mogAsset.GetAssetName());

					MOG_Time assetTime = new MOG_Time(version);
					MOG_Time currentAssetTime = new MOG_Time(currentVersion);
					
					ListViewItem item = new ListViewItem();						

					// We have support for the old lists as well as the new ones that have extra information stored.
					if (string.Compare(extraInfo, "ReportList", true) !=0)
					{
						string []extraItems = extraInfo.Split(",".ToCharArray());						
						foreach (string extra in extraItems)
						{
							if (item.Text.Length == 0)
							{
								item.Text = extra;
							}
							else
							{
								item.SubItems.Add(extra);
							}
						}
						
						// Update the version
						if (assetTime.Compare(currentAssetTime) != 0 )
						{
							item.SubItems[FindColumn("Version")].Text = currentAssetTime.FormatString("");
							item.SubItems[FindColumn("Version")].ForeColor = Color.Red;
						}						
					}
					else
					{
						item = AddItemToListView(mogAsset, pProperties, MOG_ControllerRepository.GetAssetBlessedVersionPath(mogAsset, version).GetEncodedFilename());
						
						// Get version
						if (assetTime.Compare(currentAssetTime) != 0 )									// Version
						{
							item.SubItems[FindColumn("Version")].Text = currentAssetTime.FormatString("");
							item.SubItems[FindColumn("Version")].ForeColor = Color.Red;
							version = currentVersion;
						}
						else
						{
							item.SubItems[FindColumn("Version")].Text = assetTime.FormatString("");
						}						
					}

					// Icon
					item.ImageIndex = MogUtil_AssetIcons.GetAssetIconIndex(mogAsset.GetAssetFullName());
					
					ListListView.Items.Add(item);

					ProgressStep();
				}

				UpdateAssetTotals();
				ListListView.EndUpdate();
				ProgressReset();
			}
		}

		private ListViewItem AddItemToListView(MOG_Filename mogAsset, MOG_Properties pProperties, string fullPath)
		{
			ListViewItem item = new ListViewItem();
			foreach (ColumnHeader column in ListListView.Columns)
			{
				item.SubItems.Add("");
			}

			SetSubColumnText(item, "Name", mogAsset.GetAssetLabel());
			SetSubColumnText(item, "MOG Classification", mogAsset.GetAssetClassification());
			SetSubColumnText(item, "Platform", mogAsset.GetAssetPlatform());
			SetSubColumnText(item, "Version", mogAsset.GetVersionTimeStampString(""));

			if (pProperties != null)
			{
				SetSubColumnText(item, "Size", guiAssetController.FormatSize(pProperties.Size));
				SetSubColumnText(item, "Package", GetPackages(pProperties));
				SetSubColumnText(item, "Creator", pProperties.Creator);
				SetSubColumnText(item, "Last Bless", pProperties.Owner);
				SetSubColumnText(item, "Computer", pProperties.SourceMachine);
				SetSubColumnText(item, "GamePath", pProperties.SyncTargetPath);
				SetSubColumnText(item, "Last Comment", pProperties.LastComment);
			}

			SetSubColumnText(item, "Fullname", fullPath);

			// Icon
			item.ImageIndex = MogUtil_AssetIcons.GetAssetIconIndex(mogAsset.GetOriginalFilename(), pProperties, false);
			return ListListView.Items.Add(item);
		}

		private void SetSubColumnText(ListViewItem item, string columnName, string value)
		{
			int colIndex = FindColumn(columnName);
			if (colIndex != -1) item.SubItems[colIndex].Text = value;
		}

		public int FindColumn(string name)
		{
			if (ListListView.Columns.ContainsKey(name))
				return ListListView.Columns.IndexOfKey(name);

			int x = 0;
			foreach (ColumnHeader column in ListListView.Columns)
			{
				if (string.Compare(column.Text, name, true) == 0)
				{
					return x;
				}

				x++;
			}

			return -1;
		}

		/// <summary>
		/// Save out this report
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ListSaveButton_Click(object sender, System.EventArgs e)
		{
			string ReportDir = MOG_ControllerProject.GetUser().GetUserPath() + "\\Reports";
			string FileName = CreateUniqueFilename(ReportDir, "Untitled.rep");

			if (!DosUtils.DirectoryExist(ReportDir))
			{
				DosUtils.DirectoryCreate(ReportDir);
			}
			
			ListSaveFileDialog.InitialDirectory = ReportDir;
			ListSaveFileDialog.FileName = FileName;

			if (ListSaveFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				SaveReportList(ListSaveFileDialog.FileName);
			}
		}

		/// <summary>
		/// Save our current listView items out to the target ini file
		/// </summary>
		/// <param name="filename"></param>
		private void SaveReportList(string filename)
		{
			ListOkButton.Enabled = false;

			MOG_Ini report = new MOG_Ini(filename);
			report.Empty();

			try
			{
				ProgressMax(ListListView.Items.Count);

				foreach (ListViewItem item in ListListView.Items)
				{
					string extraInfo = "";

					for (int i = 0; i < item.SubItems.Count; i++)
					{
						ProgressStep();

						if (item.SubItems[i].Text.Length == 0)
						{
							extraInfo = extraInfo + " " + ",";
						}
						else
						{
							extraInfo = extraInfo + item.SubItems[i].Text + ",";
						}
					}
					report.PutString("ASSETS", item.SubItems[FindColumn("Fullname")].Text, extraInfo);
				}
			}
			catch
			{
			}
			finally
			{
				report.Save();
				report.Close();
				ProgressReset();
				ListOkButton.Enabled = true;
			}
		}
		#endregion

		#region Node Adding functions
		/// <summary>
		/// Add a simple item that has no MOG_Properties
		/// </summary>
		/// <param name="mogAsset"></param>
		/// <param name="failedString"></param>
		/// <returns></returns>
		public ListViewItem ItemAdd(string mogAsset, string failedString)
		{
			ListViewItem item = new ListViewItem();
			item.Text = mogAsset;
			item.SubItems.Add(failedString);
			
			return ListListView.Items.Add(item);
		}

		/// <summary>
		/// Add an item that is a sub-list of items we already have
		/// </summary>
		/// <param name="tag"></param>
		/// <returns></returns>
		public ListViewItem ItemAdd( Mog_BaseTag tag )
		{
			ListViewItem item = tag.Owner as ListViewItem;
			if( item != null )
			{
				return ListListView.Items.Add( (ListViewItem)item.Clone() );
			}
			else
			{
				return new ListViewItem();
			}
		}

		/// <summary>
		/// Get the assigned packages for this asset
		/// </summary>
		/// <param name="pProperties"></param>
		/// <returns></returns>
		private string GetPackages(MOG_Properties pProperties)
		{
			ArrayList packages = pProperties.GetPackages();

			// Do we have any assiged packages?
			if (packages.Count > 0)
			{
				string packageString = "";

				// String them all together in one comma delimited string
				foreach (MOG_Property package in packages)
				{
					packageString = packageString + "," + MOG_ControllerPackage.GetPackageName(package.mPropertyKey);
				}

				// Return that string
				return packageString;
			}
			else
			{
				// Nope, no packages
				return "";
			}
		}

		/// <summary>
		/// Determine and return the full filename for this asset
		/// </summary>
		/// <param name="mogAsset"></param>
		/// <param name="pProperties"></param>
		/// <param name="version"></param>
		/// <returns></returns>
		private MOG_Filename ToString(MOG_Filename mogAsset, MOG_Properties pProperties, string version)
		{
			// Is this asset already in the repository?
			if (mogAsset.IsBlessed())
			{
				return mogAsset;
			}
				// Create a path to the blessed version of this asset
			else if (pProperties != null && version.Length > 0)
			{
				return MOG_ControllerRepository.GetAssetBlessedVersionPath(mogAsset, version);
			}
				// OK, just return thr fullFilename of this asset
			else
			{
				return mogAsset;
			}			
		}

		/// <summary>
		///  Add a new mog asset to the listView items array
		/// </summary>
		/// <param name="mogAsset"></param>
		/// <param name="version"></param>
		/// <param name="pProperties"></param>
		/// <param name="failedString"></param>
		/// <returns></returns>
		public ListViewItem ItemAdd(MOG_Filename mogAsset, MOG_Properties pProperties, string failedString)
		{
			//MOG_Time assetTime = new MOG_Time(version);

			ListViewItem item = AddItemToListView(mogAsset, pProperties, ToString(mogAsset, pProperties, mogAsset.GetVersionTimeStamp()).GetEncodedFilename());
			//item.SubItems[FindColumn("Failed Check")].Text = failedString;												// Failed string

			return item;
		}

		#endregion

		public void ProgressStep()
		{
			if (this.ListProgressBar.Position + 1 < this.ListProgressBar.PositionMax)
			{
				this.ListProgressBar.Position++;
				Application.DoEvents();
			}
		}

		public void ProgressMax(int max)
		{
			ProgressReset();
			this.ListProgressBar.PositionMax = max;
			Application.DoEvents();
		}

		public void ProgressReset()
		{
			this.ListProgressBar.Position = 0;
			Application.DoEvents();
		}
	
		/// <summary>
		/// Close the form
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ListOkButton_Click(object sender, System.EventArgs e)
		{
			Close();
		}
		
		/// <summary>
		/// Open the properties window on the asset selected
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ListListView_DoubleClick(object sender, System.EventArgs e)
		{
			Control sourceControl = sender as Control;
			ArrayList assetNames = new ArrayList();

			foreach (ListViewItem item in ListListView.SelectedItems)
			{
				// Find the 'Fullname' column index
				int columnIndex = FindColumn("Fullname");
				if (columnIndex == -1)
				{
					// Do it the old way that could crash
					columnIndex = (int)ASSET_LIST.FULLNAME;
					// Make sure we don't exceed our sub items
					if (columnIndex >= item.SubItems.Count)
					{
						columnIndex = -1;
					}
				}

				// Check if we found a valid columnIndex?
				if (columnIndex != -1)
				{
					assetNames.Add(item.SubItems[columnIndex].Text);
				}
			}

			AssetPropertiesForm properties = new AssetPropertiesForm();
			properties.Initialize(assetNames);
			properties.ShowDialog(sourceControl.TopLevelControl);
		}

		/// <summary>
		/// Update the totals numbers on the bottom of the form
		/// </summary>
		public void UpdateAssetTotals()
		{
			AssetListTotalLabel.Text = ListListView.Items.Count.ToString();
			AssetListSelectedTotalLabel.Text = ListListView.SelectedItems.Count.ToString();
		}
		
		/// <summary>
		/// Support the Ctrl-A for select all
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ListForm_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode == Keys.A && e.Modifiers == Keys.Control)
			{
				ListListView.Cursor = Cursors.WaitCursor;
				ListListView.BeginUpdate();

				foreach (ListViewItem item in ListListView.Items)
				{
					item.Selected = true;
				}

				ListListView.EndUpdate();
				ListListView.Cursor = Cursors.Default;
			}
		}

		/// <summary>
		/// Update our totals
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ListListView_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			AssetListSelectedTotalLabel.Text = ListListView.SelectedItems.Count.ToString();
		}

		private void ListForm_Load(object sender, EventArgs e)
		{
			MogUtils_Settings.LoadListView("ReportWindow", ListListView);
			Top = MogUtils_Settings.LoadIntSetting("ReportWindow", "Top", Top);
			Left = MogUtils_Settings.LoadIntSetting("ReportWindow", "Left", Left);
			Width = MogUtils_Settings.LoadIntSetting("ReportWindow", "Width", Width);
			Height = MogUtils_Settings.LoadIntSetting("ReportWindow", "Height", Height);
		}

		private void ListForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			MogUtils_Settings.SaveListView("ReportWindow", ListListView);
			MogUtils_Settings.SaveIntSetting("ReportWindow", "Top", Top);
			MogUtils_Settings.SaveIntSetting("ReportWindow", "Left", Left);
			MogUtils_Settings.SaveIntSetting("ReportWindow", "Width", Width);
			MogUtils_Settings.SaveIntSetting("ReportWindow", "Height", Height);
		}
	}
}
