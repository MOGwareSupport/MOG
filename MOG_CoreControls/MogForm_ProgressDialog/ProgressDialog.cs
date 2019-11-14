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
	public partial class ProgressDialog : Form
	{
		private static bool mHidden = false;
		public static bool Hidden
		{
			get { return mHidden; }
			set { mHidden = value; }
		}
	
		protected AutoResetEvent mWorkerFinishedEvent = new AutoResetEvent(false);

		protected BackgroundWorker mWorker;
		protected object mParameter;
		
		protected object mWorkerResult = null;
		public object WorkerResult
		{
			get { return mWorkerResult; }
		}

		Stack<ProgressBar> mProgressStack = new Stack<ProgressBar>();

		public ProgressBar CurrentProgress
		{
			get
			{
				if (mProgressStack.Count > 0)
				{
					return mProgressStack.Peek();
				}

				return null;
			}
		}

		public ProgressDialog(string title, string description, DoWorkEventHandler work, object parameter, bool reportsProgress)
			: this(title, description, work, parameter)
		{

		}
		
		public ProgressDialog(string title, string description, DoWorkEventHandler work, object parameter)
		{
			InitializeComponent();

			PushProgressbar(ProgressBarStyle.Marquee);
			
			Text = title;
			Description.Text = description;
			CurrentProgress.Value = 0;

			mParameter = parameter;

			if (work != null)
			{
				mWorker = new BackgroundWorker();
				mWorker.WorkerSupportsCancellation = true;
				mWorker.WorkerReportsProgress = true;
				mWorker.DoWork += work;
				mWorker.ProgressChanged += OnProgressChanged;
				mWorker.RunWorkerCompleted += OnWorkerCompleted;
			}
		}

		public void PushProgressbar(ProgressBarStyle style)
		{
			ProgressBar progress = new ProgressBar();

			progress.Height = 10;
			progress.Style = style;
			progress.Width = FlowPanel.Width - FlowPanel.Padding.Left - FlowPanel.Padding.Right - FlowPanel.Margin.Left - FlowPanel.Margin.Right;
			progress.Anchor = (AnchorStyles.Left | AnchorStyles.Right);

			bool bIsInControlList = false;

			if (mProgressStack.Count == 0 ||
				progress.Style != ProgressBarStyle.Marquee ||
				CurrentProgress.Style != ProgressBarStyle.Marquee)
			{
				bIsInControlList = true;
			}

			progress.Tag = bIsInControlList;

			if (bIsInControlList)
			{
				FlowPanel.Controls.Add(progress);
			}

			mProgressStack.Push(progress);
		}

		public void PopProgressbar()
		{
			int index = FlowPanel.Controls.Count - 1;
			ProgressBar progress = mProgressStack.Pop();

			bool bIsInControlList = (bool)progress.Tag;
			if (bIsInControlList)
			{
				FlowPanel.Controls.Remove(progress);
			}

			//We have to call this so it will free up the window handle and we won't run out after using a lot of them
			progress.Dispose();
		}

		public new virtual DialogResult ShowDialog(IWin32Window owner)
		{
			if (Hidden)
			{
				//We're not actually showing the form, so just use the logic that's in the no-parameter version
				return this.ShowDialog();
			}
			else
			{
				return base.ShowDialog(owner);
			}
		}
		
		public new virtual DialogResult ShowDialog()
		{
			if (Hidden)
			{
				ProgressDialog_Load(null, null);

				//Wait here until the background worker signals the event that means we can continue
				while (!mWorkerFinishedEvent.WaitOne(0, false))
				{
					//Usually calling DoEvents is not a good idea, but this is a case when we want to
					//block our thread inside this modal dialog, but we still need to get the workercompleted event
					//and events don't preempt threads, they wait for the thread to run the message loop
					Application.DoEvents();
				}

				return DialogResult.OK;
			}
			else
			{
				return base.ShowDialog();
			}
		}

		protected void OnProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			if (e.UserState is ProgressCommand)
			{
				if (e.UserState is PushProgressbarCommand)
				{
					PushProgressbarCommand command = e.UserState as PushProgressbarCommand;
					PushProgressbar(command.Style);
				}
				else if (e.UserState is PopProgressbarCommand)
				{
					PopProgressbar();
				}
			}
			else
			{
				if (e.ProgressPercentage > 0)
				{
					if (CurrentProgress.Style == ProgressBarStyle.Marquee)
					{
						//Up until now this has been in Marquee mode, but now that we actually have some progress to report, let's change its style.
						CurrentProgress.Style = ProgressBarStyle.Blocks;
					}

					//Update the progress value
					if (e.ProgressPercentage <= CurrentProgress.Maximum)
					{
						CurrentProgress.Value = e.ProgressPercentage;
					}
					else
					{
						CurrentProgress.Value = CurrentProgress.Maximum;
					}
				}

				if (Hidden)
				{
					string message = e.UserState as string;
					if (!string.IsNullOrEmpty(message))
					{
						Console.WriteLine(message.Replace("\n", "   "));
					}
				}

				if (e.UserState is string)
				{
					Description.Text = e.UserState as string;
				}
			}
		}

		protected void OnWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			mWorkerResult = e.Result;

			mWorkerFinishedEvent.Set();

			Close();
		}

		private void CancelButton_Click(object sender, EventArgs e)
		{
			if (mWorker != null)
			{
				mWorker.CancelAsync();
			}
			else
			{
				Close();
			}

			Cancel.Text = "Canceling...";
			Cancel.Enabled = false;
		}

		protected void ProgressDialog_Load(object sender, EventArgs e)
		{
			if (mWorker != null && !mWorker.IsBusy)
			{
				mWorker.RunWorkerAsync(mParameter);
			}
		}

		protected void ProgressDialog_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (Cancel.Enabled)
				DialogResult = DialogResult.OK;
			else
				DialogResult = DialogResult.Cancel;
		}
	}

	public class ProgressCommand
	{

	}

	public class PushProgressbarCommand : ProgressCommand
	{
		private ProgressBarStyle mStyle;
		public ProgressBarStyle Style
		{
			get { return mStyle; }
		}
		
		public PushProgressbarCommand()
		{
			mStyle = ProgressBarStyle.Blocks;
		}

		public PushProgressbarCommand(ProgressBarStyle style)
		{
			mStyle = style;
		}
	}

	public class PopProgressbarCommand : ProgressCommand
	{

	}
}