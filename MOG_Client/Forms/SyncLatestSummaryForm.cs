using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

// Sort manager
using EV.Windows.Forms;

using MOG.INI;
using MOG.FILENAME;
using MOG.DOSUTILS;
using MOG.DATABASE;
using MOG.REPORT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.CONTROLLER.CONTROLLERSYNCDATA;
using MOG.CONTROLLER.CONTROLLERPROJECT;

using MOG_ControlsLibrary.Utils;

namespace MOG_Client
{
	/// <summary>
	/// Summary description for SyncLatestSummaryForm.
	/// </summary>
	public class SyncLatestSummaryForm : System.Windows.Forms.Form
	{
		private MOG_Ini mSummary;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.ListView SummaryListView;
		private System.Windows.Forms.ColumnHeader Asset;
		private System.Windows.Forms.ColumnHeader Date;
		private System.Windows.Forms.ColumnHeader State;
		private ListViewSortManager mSortManager;
		private System.Windows.Forms.Label TotalCopiedLabel;
		private System.Windows.Forms.Label TotalSkippedLabel;
		private System.Windows.Forms.Label TotalDeletedLabel;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public SyncLatestSummaryForm(MOG_Ini summary)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			mSummary = summary;

			SummaryListView.SmallImageList = MogUtil_AssetIcons.Images;

			int totalCopied = 0;
			int totalSkipped = 0;
			int totalDeleted = 0;

			SummaryListView.BeginUpdate();

			totalSkipped += AppendSummary("Files.Skipped", "Skipped", Color.Purple);

			totalCopied += AppendSummary("Files.New", "New", Color.Green);
			totalCopied += AppendSummary("Files.Copied", "Updated", Color.Green);
			totalCopied += AppendSummary("Files.Restored", "Restored", Color.ForestGreen);
			totalCopied += AppendSummary("Files.Reverted", "Reverted", Color.DarkGreen);
			totalDeleted += AppendSummaryNonExistant("Files.Removed", "Removed", Color.OrangeRed);
			totalCopied += AppendSummary("Files.Up-to-date", "Up-to-date", Color.Black);
			totalCopied += AppendSummary("Files.Restamped", "Restamped", Color.ForestGreen);

			totalCopied += AppendSummary("Assets.Canceled", "Canceled", Color.Firebrick);

			totalCopied += AppendSummary("Files.Missing", "Missing", Color.Firebrick);
			totalCopied += AppendSummary("Assets.Missing", "Missing", Color.Firebrick);
			totalCopied += AppendSummary("Files.RemoveError", "Remove Failed", Color.Firebrick);
			totalCopied += AppendSummary("Files.CopyError", "Copy Failed", Color.Firebrick);

			TotalCopiedLabel.Text = TotalCopiedLabel.Text + " " + totalCopied.ToString();
			TotalSkippedLabel.Text = TotalSkippedLabel.Text + " " + totalSkipped.ToString();
			TotalDeletedLabel.Text = TotalDeletedLabel.Text + " " + totalDeleted.ToString();

			if (SummaryListView.Items.Count == 0)
			{
				// Check if the user canceled?
				if (mSummary.SectionExist("Assets.Canceled"))
				{
					SummaryListView.Items.Add("User Canceled!");
				}
				else
				{
					SummaryListView.Items.Add("All assets current!");
				}
			}

			SummaryListView.EndUpdate();

			// Initialize sorting columns
			mSortManager = new ListViewSortManager(SummaryListView, new Type[] {   typeof(ListViewTextCaseInsensitiveSort),
																				   typeof(ListViewDateSort),
																				   typeof(ListViewTextCaseInsensitiveSort) });
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SyncLatestSummaryForm));
			this.buttonOk = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.TotalDeletedLabel = new System.Windows.Forms.Label();
			this.TotalSkippedLabel = new System.Windows.Forms.Label();
			this.TotalCopiedLabel = new System.Windows.Forms.Label();
			this.SummaryListView = new System.Windows.Forms.ListView();
			this.Asset = new System.Windows.Forms.ColumnHeader();
			this.Date = new System.Windows.Forms.ColumnHeader();
			this.State = new System.Windows.Forms.ColumnHeader();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonOk
			// 
			this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonOk.Location = new System.Drawing.Point(214, 64);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(80, 23);
			this.buttonOk.TabIndex = 0;
			this.buttonOk.Text = "Ok";
			this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.TotalDeletedLabel);
			this.panel1.Controls.Add(this.TotalSkippedLabel);
			this.panel1.Controls.Add(this.TotalCopiedLabel);
			this.panel1.Controls.Add(this.buttonOk);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 259);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(508, 96);
			this.panel1.TabIndex = 2;
			// 
			// TotalDeletedLabel
			// 
			this.TotalDeletedLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.TotalDeletedLabel.Location = new System.Drawing.Point(16, 43);
			this.TotalDeletedLabel.Name = "TotalDeletedLabel";
			this.TotalDeletedLabel.Size = new System.Drawing.Size(480, 16);
			this.TotalDeletedLabel.TabIndex = 3;
			this.TotalDeletedLabel.Text = "Total Files Deleted:";
			// 
			// TotalSkippedLabel
			// 
			this.TotalSkippedLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.TotalSkippedLabel.Location = new System.Drawing.Point(16, 23);
			this.TotalSkippedLabel.Name = "TotalSkippedLabel";
			this.TotalSkippedLabel.Size = new System.Drawing.Size(480, 16);
			this.TotalSkippedLabel.TabIndex = 2;
			this.TotalSkippedLabel.Text = "Total Files Skipped:";
			// 
			// TotalCopiedLabel
			// 
			this.TotalCopiedLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.TotalCopiedLabel.Location = new System.Drawing.Point(16, 4);
			this.TotalCopiedLabel.Name = "TotalCopiedLabel";
			this.TotalCopiedLabel.Size = new System.Drawing.Size(480, 16);
			this.TotalCopiedLabel.TabIndex = 1;
			this.TotalCopiedLabel.Text = "Total Files Copied:";
			// 
			// SummaryListView
			// 
			this.SummaryListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																							  this.Asset,
																							  this.Date,
																							  this.State});
			this.SummaryListView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.SummaryListView.FullRowSelect = true;
			this.SummaryListView.Location = new System.Drawing.Point(0, 0);
			this.SummaryListView.Name = "SummaryListView";
			this.SummaryListView.Size = new System.Drawing.Size(508, 259);
			this.SummaryListView.TabIndex = 3;
			this.SummaryListView.View = System.Windows.Forms.View.Details;
			// 
			// Asset
			// 
			this.Asset.Text = "Asset";
			this.Asset.Width = 286;
			// 
			// Date
			// 
			this.Date.Text = "Date";
			this.Date.Width = 133;
			// 
			// State
			// 
			this.State.Text = "State";
			this.State.Width = 88;
			// 
			// SyncLatestSummaryForm
			// 
			this.AcceptButton = this.buttonOk;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(508, 355);
			this.Controls.Add(this.SummaryListView);
			this.Controls.Add(this.panel1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.Name = "SyncLatestSummaryForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "GetLatest Latest Summary";
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private int AppendSummary(string section, string comment, Color nodeColor)
		{
			// Get all the file copies
			if (mSummary.SectionExist(section))
			{
				string[] keys = mSummary.GetSectionKeys(section);
				foreach(string key in keys)
				{
					// Get the asset/file information from the summary file
					string assetName = mSummary.GetString(section, key);
					string fileName = key;

					// Trim any starting '\'
					if (fileName != null && fileName.Length > 0)
					{
						fileName = fileName.TrimStart("\\".ToCharArray());
					}

					try
					{
						string fullfilename = MOG_ControllerProject.GetCurrentSyncDataController().GetSyncDirectory() + "\\" + fileName;

						ListViewItem item = new ListViewItem();
						item.Text = Path.GetFileName(fileName);
						item.ForeColor = nodeColor;

						FileInfo file = new FileInfo(fullfilename);
						item.SubItems.Add(file.LastWriteTime.ToShortDateString() + " " + file.LastWriteTime.ToShortTimeString());
						item.SubItems.Add(comment);

						item.ImageIndex = MogUtil_AssetIcons.GetFileIconIndex(fullfilename);

						SummaryListView.Items.Add(item);
					}
					catch(Exception e)
					{
						MOG_Report.ReportMessage("Update Summary", e.Message, e.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.ERROR);
					}
				}

				return mSummary.CountKeys(section);
			}

			return 0;
		}

		private int AppendSummaryNonExistant(string section, string comment, Color nodeColor)
		{
			// Get all the file copies
			if (mSummary.SectionExist(section))
			{
				for (int i = 0; i < mSummary.CountKeys(section); i++)
				{
					ListViewItem item = new ListViewItem();

					// Get classification name
					string assetName = mSummary.GetKeyByIndexSLOW(section, i);

					// Get file name
					string fileName = mSummary.GetKeyNameByIndexSLOW(section, i);

					// Trim any starting '\'
					if (fileName != null && fileName.Length > 0)
					{
						fileName = fileName.TrimStart("\\".ToCharArray());
					}

					item.Text = fileName;
					item.ForeColor = nodeColor;

					item.SubItems.Add("");
					item.SubItems.Add(comment);
																
					SummaryListView.Items.Add(item);
				}

				return mSummary.CountKeys(section);
			}

			return 0;
		}

		private void buttonOk_Click(object sender, System.EventArgs e)
		{
			Close();
		}
	}
}
