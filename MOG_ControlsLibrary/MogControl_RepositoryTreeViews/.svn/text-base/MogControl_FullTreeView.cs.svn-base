using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace MOG_ControlsLibrary.Controls
{
	/// <summary>
	/// Summary description for MogControl_FullTreeView.
	/// </summary>
	public class MogControl_FullTreeView : MogControl_PropertyClassificationTreeView
	{
		private BackgroundWorker mWorker;
		
		public MogControl_FullTreeView()
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
			InitializeClassificationsList();
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