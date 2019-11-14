using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections;
using MOG.DATABASE;
using MOG;
using MOG.FILENAME;
using System.Collections.Generic;

namespace MOG_ControlsLibrary.Controls
{
	/// <summary>
	/// Summary description for MogControl_FullTreeView.
	/// </summary>
    public partial class MogControl_ImportTreeView : MogControl_PropertyClassificationTreeView
    {
		public BackgroundWorker mWorker;
		
		public MogControl_ImportTreeView()
		{
		}

		public override void Initialize()
		{
			bIsInitialized = true;

			Cursor = Cursors.WaitCursor;

			Nodes.Clear();

			mWorker = new BackgroundWorker();
			mWorker.DoWork += InitializeImportableClassificationList_Worker;
			mWorker.RunWorkerCompleted += OnWorkerCompleted;
			mWorker.WorkerReportsProgress = true;

			mWorker.RunWorkerAsync();
		}

		public override void Initialize(MethodInvoker OnInitializeDone)
		{
			this.OnInitializeDone = OnInitializeDone;

			Initialize();
		}

		private void InitializeImportableClassificationList_Worker(object sender, DoWorkEventArgs e)
		{
			InitializeClassificationsList(true);
		}

		protected override void InitializeClassificationsList(bool importableOnly)
		{
			lock (mRequiredClassifications)
			{
				ArrayList assets;
				ArrayList classifications;
				// If we have no MOG_Property(s), populate as a full TreeView
				if (MogPropertyList.Count > 0)
				{
					assets = MOG_DBAssetAPI.GetAllAssetsByClassificationAndProperties("", MogPropertyList, true);
					classifications = MOG_DBAssetAPI.GetAllActiveClassifications(MogPropertyList[0] as MOG_Property);
				}
				else
				{
					assets = MOG_DBAssetAPI.GetAllAssets();
					classifications = MOG_DBAssetAPI.GetAllActiveClassifications();
				}

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
								if (!importableOnly || !MOG_Filename.IsLibraryClassification(classificationName))
								{
									//we don't have this classification yet, so add it and give it a container for assets
									List<string> assetList = new List<string>();
									mRequiredClassifications.Add(classificationName, assetList);
								}
							}
						}
					}
				}

				if (assets != null && assets.Count > 0)
				{
					foreach (MOG_Filename asset in assets)
					{
						if (!importableOnly || !MOG_Filename.IsLibraryClassification(asset.GetAssetClassification()))
						{
							int index = mRequiredClassifications.IndexOfKey(asset.GetAssetClassification());
							if (index == -1)
							{
								List<string> assetList = new List<string>();
								mRequiredClassifications.Add(asset.GetAssetClassification(), assetList);
								index = mRequiredClassifications.Count - 1;
							}
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