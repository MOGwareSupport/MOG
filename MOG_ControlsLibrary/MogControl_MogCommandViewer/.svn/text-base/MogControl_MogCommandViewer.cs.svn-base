using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using MOG.COMMAND;

namespace MOG_ControlsLibrary.MogControl_MogCommandViewer
{
	public partial class MogControl_MogCommandViewer : UserControl
	{
		public MogControl_MogCommandViewer()
		{
			InitializeComponent();
		}

		public void LoadCommand(MOG_Command command)
		{
			tbComputer.Text = command.GetComputerName();
			tbDate.Text = MogUtils_StringVersion.VersionToString(command.GetCommandTimeStamp());
			tbIp.Text = command.GetComputerIP();
			tbID.Text = command.GetCommandID().ToString();
			tbProject.Text = command.GetProject();
			tbType.Text = command.GetCommandType().ToString();
			tbUser.Text = command.GetUserName();
			tbBranch.Text = command.GetBranch();
			tbJobId.Text = command.GetJobLabel();
			tbBlocking.Text = command.IsBlocking().ToString();
			tbSlaveID.Text = command.mAssignedSlaveID.ToString();
			tbCompleted.Text = command.IsCompleted().ToString();
			tbValidSlaves.Text = command.GetValidSlaves();
			if (command.GetAssetFilename() != null)
			{
				tbAssetName.Text = command.GetAssetFilename().GetAssetFullName();
			}
		}
	}
}
