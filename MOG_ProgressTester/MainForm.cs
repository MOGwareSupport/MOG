using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using MOG_CoreControls;
using System.Threading;

namespace MOG_ProgressTester
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		private void TestButton_Click(object sender, EventArgs e)
		{
			ProgressDialog progress = new ProgressDialog("Long Process", "Doing stuff...", Test_Worker, null, true);
			progress.ShowDialog(this);
		}

		private void Test_Worker(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;

			for (int i = 0; i < 10 && !worker.CancellationPending; i++)
			{
				worker.ReportProgress(i * 10, null);
				worker.ReportProgress(0, new PushProgressbarCommand());

				for (int j = 0; j < 10 && !worker.CancellationPending; j++)
				{
					worker.ReportProgress(j * 10, null);
					worker.ReportProgress(0, new PushProgressbarCommand());

					for (int k = 0; k < 10 && !worker.CancellationPending; k++)
					{
						worker.ReportProgress(k * 10, null);
						Thread.Sleep(100);
					}

					worker.ReportProgress(0, new PopProgressbarCommand());
				}

				worker.ReportProgress(0, new PopProgressbarCommand());
			}
		}
	}
}