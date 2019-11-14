using Server_Gui;
using System;
using MOG;
using System.Windows.Forms;
using System.Collections;
using MOG.COMMAND.SERVER;
using MOG.COMMAND;
using MOG.REPORT;

namespace MOG_Server.Server_Mog_Utilities
{
	/// <summary>
	/// Summary description for Project.
	/// </summary>
	/// 
	
	public class Monitor
	{
		public enum connectionType
		{
			CONNECTIONTYPE_SERVER,
			CONNECTIONTYPE_SLAVE,
			CONNECTIONTYPE_CLIENT,
		};

		public FormMainSMOG mForm;
//J		static private Int32 IDCount = 0;

		public Monitor(FormMainSMOG form)
		{
			mForm = form;
		}

		public void DislayAddConnection(String name, String ip, Int32 Id, MOG_COMMAND_TYPE connectionType)
		{
			ListViewItem item = new ListViewItem();

			// Setup new connection node
			item.Text = name;
			item.SubItems.Add(ip);
			item.SubItems.Add(Convert.ToString(Id));

			string connectionTypeString;
			switch(connectionType)
			{
				case MOG_COMMAND_TYPE.MOG_COMMAND_RegisterClient:
					connectionTypeString = "CLIENT";
					break;
				case MOG_COMMAND_TYPE.MOG_COMMAND_RegisterSlave:
					connectionTypeString = "SLAVE";
					break;
				default:
					connectionTypeString = "SERVER";
					break;
			}

			item.SubItems.Add(connectionTypeString);

			mForm.lviewMonitor.Items.Add(item);
			return;
		}

		public void DislayDeleteAllConnections()
		{
			//this needs to walk the connections and add all the connected computers.

			foreach(ListViewItem item in mForm.lviewMonitor.Items)
			{
				mForm.lviewMonitor.Items.Remove(item);
			}
		}

		public void DisplayDeleteConnectionsByID(int id)
		{
			ListViewItem foundItem = null;

			foreach(ListViewItem item in mForm.lviewMonitor.Items)
			{
				Int32 ID = Convert.ToInt32(item.SubItems[2].Text);

				if (id == ID)
				{
					foundItem = item;
					break;
				}
			}

			if (foundItem != null)
			{
				mForm.lviewMonitor.Items.Remove(foundItem);
			}

			MOG_REPORT.ReportComment("deleted a computer");
		}

		public void DisplayUpdateExistingConnections()
		{
			//first let's kill the existing ones.
			foreach(ListViewItem item in mForm.lviewMonitor.Items)
			{
				mForm.lviewMonitor.Items.Remove(item);
			}

			//this needs to walk the connections and add all the connected computers.
			MOG_CommandServer commandServer = (MOG_CommandServer)mForm.gMog.GetCommandManager();

			ArrayList slavesArray = commandServer.GetRegisteredSlaves();
			ArrayList clientsArray = commandServer.GetRegisteredClients();

			//show the server first.
			DislayAddConnection(mForm.gMog.GetComputerName(), mForm.gMog.GetComputerIP(), 1, 0);

			foreach(Object item in clientsArray)
			{
				MOG_Command connection = (MOG_Command)item;

				DislayAddConnection(connection.GetComputerName(), connection.GetComputerIP(), connection.GetNetworkID(), MOG_COMMAND_TYPE.MOG_COMMAND_RegisterClient);
			}

			foreach(Object item in slavesArray)
			{
				MOG_Command connection = (MOG_Command)item;

				DislayAddConnection(connection.GetComputerName(), connection.GetComputerIP(), connection.GetNetworkID(), MOG_COMMAND_TYPE.MOG_COMMAND_RegisterSlave);
			}
		}
	}
}
