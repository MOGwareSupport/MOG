using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.IO;
using System.ComponentModel;

using MOG;
using MOG.INI;
using MOG.USER;
using MOG.DOSUTILS;
using MOG.FILENAME;
using MOG.COMMAND;
using MOG.PROPERTIES;
using MOG.REPORT;
using MOG.CONTROLLER;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.ASSET_STATUS;
using MOG.DATABASE;
using MOG_Client.Client_Mog_Utilities;
using MOG_Client.Client_Mog_Utilities.AssetOptions;
using MOG_Client.Client_Gui;

using MOG_ControlsLibrary;
using MOG_ControlsLibrary.Common;
using MOG_ControlsLibrary.Utils;

namespace MOG_Client.Client_Gui.AssetManager_Helper
{
	/// <summary>
	/// Summary description for guiAssetManagerTrash.
	/// </summary>
	public class guiAssetManagerTrash
	{
		private guiAssetManager mAssetManager;
		BackgroundWorker mRefreshWorker;
		private long mSize;
		public guiAssetManagerTrash(guiAssetManager assetManager)
		{
			mAssetManager = assetManager;
			mSize = 0;

			mAssetManager.mainForm.AssetManagerTrashListView.SmallImageList = MogUtil_AssetIcons.Images;
		}

		public void Refresh()
		{
			if (mRefreshWorker == null)
			{
				mRefreshWorker = new BackgroundWorker();
				mRefreshWorker.WorkerSupportsCancellation = false;
				mRefreshWorker.WorkerReportsProgress = true;
				mRefreshWorker.DoWork += RefreshWorker_DoWork;
				mRefreshWorker.ProgressChanged += RefreshWorker_ProgressUpdate;
				mRefreshWorker.RunWorkerCompleted += RefreshWorker_Completed;
			}

			if (!mRefreshWorker.IsBusy)
			{
				mAssetManager.mainForm.Cursor = Cursors.WaitCursor;
				mAssetManager.mainForm.AssetManagerTrashListView.BeginUpdate();
				mAssetManager.mainForm.AssetManagerTrashListView.Update();
				mAssetManager.mainForm.AssetManagerTrashListView.Items.Clear();

				mRefreshWorker.RunWorkerAsync();
			}
		}

		private void RefreshWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			string trashDirectory = MOG_ControllerProject.GetActiveUser().GetUserPath() + "\\Trash";
			mSize = 0;

			ArrayList assets = MOG_DBInboxAPI.InboxGetAssetListWithProperties("Trash", MOG_ControllerProject.GetActiveUser().GetUserName(), "", "");
			int i = 0;
			foreach (MOG_DBInboxProperties asset in assets)
			{
				// Create the properties for this asset
				MOG_Properties pProperties = new MOG_Properties(asset.mProperties);

				worker.ReportProgress(i, asset.mAsset.GetAssetName());
				
				// Create the ListViewItem
				ListViewItem item = guiAssetManager.NewListViewItem(asset.mAsset, pProperties.Status, pProperties);

				if (!string.IsNullOrEmpty(pProperties.Size))
				{
					mSize += long.Parse(pProperties.Size);
				}

				worker.ReportProgress(i, item);
				i++;
			}
		}

		private void RefreshWorker_ProgressUpdate(object sender, ProgressChangedEventArgs e)
		{
			if (e.UserState is string)
			{
				guiStartup.StatusBarUpdate(mAssetManager.mainForm, "Building", e.UserState as string);
			}
			else if (e.UserState is ListViewItem)
			{
				mAssetManager.mainForm.AssetManagerTrashListView.Items.Add(e.UserState as ListViewItem);

				mAssetManager.mainForm.AssetManagerTrashTotalSizeLabel.Text = "Total Size:" + FormatSize(mSize);
			}
		}
		
		private void RefreshWorker_Completed(object sender, RunWorkerCompletedEventArgs e)
		{
			mAssetManager.mainForm.AssetManagerTrashListView.EndUpdate();
			guiStartup.StatusBarClear(mAssetManager.mainForm);

			// Update global size
			mAssetManager.mainForm.AssetManagerTrashTotalSizeLabel.Text = "Total Size:" + FormatSize(mSize);
			mAssetManager.mainForm.Cursor = Cursors.Default;

			mAssetManager.mGroups.UpdateGroups(mAssetManager.mainForm.AssetManagerTrashListView);

			mAssetManager.UpdateAssetManagerTrashTabText(true);
		}

		public void RefreshRemove(MOG_Command command)
		{
			// Find the asset
			int index = ListViewItemFindFullItem(command.GetDestination(), mAssetManager.mainForm.AssetManagerTrashListView);
			if (index != -1)
			{
				mAssetManager.mainForm.AssetManagerTrashListView.Items.RemoveAt(index);
			}

			// Update global size
			mAssetManager.mainForm.AssetManagerTrashTotalSizeLabel.Text = "Total Size:" + FormatSize(mSize);
		}

		private string CalculateFileSizes(DirectoryInfo source)
		{
			// Get the size
			long size = CalcSize(source);
			mSize += size;

			// Now format
			return FormatSize(size);
		}

		private string FormatSize(long size)
		{
			string length;
			if (size > 1024000)
			{
				// Do MegaByte conversion
				length = Convert.ToString((size/10240) + 1);
				length = string.Concat(length.Substring(0, length.Length - 2), ".", length.Substring(length.Length - 2), " MB");
			}
			else if (size > 0)
			{
				// Do KiloByte conversion
				length = string.Concat(Convert.ToString((size / 1024) + 1), " KB");
			}
			else
			{
				length = "0 KB";
			}
			return length;
		}

		private long CalcSize(DirectoryInfo source)
		{
			long size = 0;

			if (source.GetDirectories().Length > 0)
			{
				DirectoryInfo[] directories = source.GetDirectories();
				foreach (DirectoryInfo dir in directories)
				{
					size += CalcSize(dir);
				}
			}
			else
			{
				FileInfo[] files = source.GetFiles();
				foreach (FileInfo file in files)
				{
					size += file.Length;
				}
			}

			return size;
		}

		private int ListViewItemFindFullItem(string name, ListView list)
		{
			foreach (ListViewItem item in list.Items)
			{
				if(string.Compare(name, item.SubItems[(int)guiAssetManager.TrashBoxColumns.FULLNAME].Text, true) == 0)
				{
					return item.Index;
				}
			}

			return -1;
		}
	}
}
