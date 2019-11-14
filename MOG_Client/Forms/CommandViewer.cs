using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MOG.COMMAND;
using MOG.CONTROLLER.CONTROLLERPROJECT;

namespace MOG_Client.Forms
{
	public partial class CommandViewer : Form
	{
		MOG_Command mCommand;

		public CommandViewer(MOG_Command command)
		{
			InitializeComponent();

			mCommand = command;
		}

		private void CommandViewer_Load(object sender, EventArgs e)
		{
			if (mCommand != null)
			{
				CommandView.LoadCommand(mCommand);
			}
		}

		private void ViewCloseButton_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void btnRestart_Click(object sender, EventArgs e)
		{
			MOG_ControllerProject.StartJob(mCommand.GetJobLabel());
		}
	}
}