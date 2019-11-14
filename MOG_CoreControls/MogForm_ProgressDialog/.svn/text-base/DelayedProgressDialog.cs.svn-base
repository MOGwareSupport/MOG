using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace MOG_CoreControls
{
	public partial class DelayedProgressDialog : ProgressDialog
	{
		AutoResetEvent mWaitEvent = new AutoResetEvent(false);
		int mDelay = 0;

		public DelayedProgressDialog(string title, string description, DoWorkEventHandler work, object parameter, bool reportsProgress)
			: this(title, description, work, parameter, reportsProgress, 500)
		{

		}
		
		public DelayedProgressDialog(string title, string description, DoWorkEventHandler work, object parameter, bool reportsProgress, int delay)
			: base(title, description, work, parameter, reportsProgress)
		{
			mDelay = delay;

			InitializeComponent();
		}

		public override DialogResult ShowDialog()
		{
			//Start a timer who will determine when it has been long enough for the dialog to wait before showing up
			System.Threading.Timer delayTimer = new System.Threading.Timer(OnDelayTimer_Fired, null, mDelay, Timeout.Infinite);

			//Start the process
			mWorker.RunWorkerCompleted += OnWorkerCompleted;
			mWorker.RunWorkerAsync(mParameter);

			//Wait here until the timer tells us we can show the dialog
			while (!mWaitEvent.WaitOne(0, false))
			{
				//Usually calling DoEvents is not a good idea, but this is a case when we want to
				//block our thread inside this modal dialog, but we still need to get the workercompleted event
				//and events don't preempt threads, they wait for the thread to run the message loop
				Application.DoEvents();
			}

			//I wonder if the worker process is already finished
			if (mWorker.IsBusy)
			{
				//Worker is still running, so I guess it's time to pop up the dialog
				return base.ShowDialog();
			}
			else
			{
				//Oh nice, the worker process finished before the timer fired
				delayTimer.Dispose();

				return DialogResult.OK;
			}
		}

		void OnDelayTimer_Fired(object state)
		{
			mWaitEvent.Set();
		}

		private new void OnWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			mWaitEvent.Set();
			
			base.OnWorkerCompleted(sender, e);
		}
	}
}

