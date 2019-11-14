using System;
using System.ComponentModel;
using System.Windows.Forms;
using MOG;
using System.Collections;
using MOG.DATABASE;
using System.Collections.Generic;
using MOG.FILENAME;
using MOG.CONTROLLER.CONTROLLERPROJECT;

namespace MOG_ControlsLibrary.Controls
{
	/// <summary>
	/// Summary description for MogControl_FullTreeView.
	/// </summary>
	public class MogControl_SyncTreeView : MogControl_PropertyClassificationTreeView
	{
		private BackgroundWorker mWorker;

		public MogControl_SyncTreeView()
		{
		}

		public override void Initialize()
		{
			Initialize(null);
		}

		public override void Initialize(MethodInvoker OnInitializeDone)
		{
			this.OnInitializeDone = OnInitializeDone;

			Cursor = Cursors.WaitCursor;

			Nodes.Clear();

			mWorker = new BackgroundWorker();
			mWorker.DoWork += InitializeClassificationList_Worker;
			mWorker.RunWorkerCompleted += OnWorkerCompleted;

			mWorker.RunWorkerAsync();
		}

		private void InitializeClassificationList_Worker(object sender, DoWorkEventArgs e)
		{		
			lock (mRequiredClassifications)
			{
				ArrayList classifications;
				
				ExclusionList = MOG_Filename.GetProjectLibraryClassificationString();
				classifications = MOG_DBReports.ClassificationForProperty("", MOG_PropertyFactory.MOG_Sync_OptionsProperties.New_SyncFiles(true), false);
				
				mRequiredClassifications.Clear();

				if (classifications != null && classifications.Count > 0)
				{
					foreach (string classificationName in classifications)
					{
						// Do we already have this class?
						if (!mRequiredClassifications.Contains(classificationName))
						{
							bool excluded = false;
							if (ExclusionList.Length > 0)
							{
								excluded = StringUtils.IsFiltered(classificationName, ExclusionList);
							}

							// Is it excluded?
							if (excluded == false)
							{
								//we don't have this classification yet, so add it and give it a container for assets
								List<string> assetList = new List<string>();
								mRequiredClassifications.Add(classificationName, assetList);
							}
						}
					}
				}

				if (ShowAssets)
				{
					// Enable this if we wnat to show assets in this tree
					ArrayList assets = MOG_DBAssetAPI.GetAllAssets(true, MOG_ControllerProject.GetBranchName());

					// Enable this if we wnat to show assets in this tree
					if (assets != null && assets.Count > 0)
					{
						foreach (MOG_Filename asset in assets)
						{
							int index = mRequiredClassifications.IndexOfKey(asset.GetAssetClassification());
							if (index >= 0)
							{
								List<string> assetList = mRequiredClassifications.GetByIndex(index) as List<string>;
								if (assetList != null)
								{
									bool excluded = false;
									if (ExclusionList.Length > 0)
									{
										excluded = StringUtils.IsFiltered(asset.GetAssetFullName(), ExclusionList);
									}

									// Is it excluded?
									if (excluded == false)
									{
										assetList.Add(asset.GetAssetFullName());
									}
								}
							}
						}
					}
				}
			}
		}

		private void OnWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			base.Initialize();

			if (OnInitializeDone != null)
			{
				OnInitializeDone();
			}

			Cursor = Cursors.Default;
		}
	}
}