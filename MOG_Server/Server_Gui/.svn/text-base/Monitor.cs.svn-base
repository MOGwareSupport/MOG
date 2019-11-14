using Server_Gui;
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;

using EV.Windows.Forms;

using MOG;
using MOG.COMMAND;
using MOG.REPORT;
using MOG.FILENAME;
using MOG.CONTROLLER.CONTROLLERSYSTEM;



namespace MOG_Server.Server_Gui
{
	/// <summary>
	/// Summary description for Project.
	/// </summary>
	/// 
	
	public class guiMonitor
	{
		public enum connectionType
		{
			CONNECTIONTYPE_SERVER,
			CONNECTIONTYPE_SLAVE,
			CONNECTIONTYPE_CLIENT,
		};

		public enum MONITOR_TABS {MACHINE_NAME, IP, ID, TYPE, INFORMATION};

		public FormMainSMOG mainForm;
		private ArrayList mListViewSort_Manager;

		public guiMonitor(FormMainSMOG form)
		{
			mainForm = form;

			mListViewSort_Manager = new ArrayList();
			mListViewSort_Manager.Add(new ListViewSortManager(mainForm.lviewMonitor, new Type[] {
																											   typeof(ListViewTextCaseInsensitiveSort),
																											   typeof(ListViewTextCaseInsensitiveSort),
																											   typeof(ListViewInt32Sort),
																											   typeof(ListViewTextCaseInsensitiveSort),
																											   typeof(ListViewTextCaseInsensitiveSort),
			}));
		}

		public void KillConnection(string id)
		{
			MOG_ControllerSystem.ConnectionKill(Convert.ToInt32(id));
		}

		public void RetaskCommand()
		{
			foreach (ListViewItem item in mainForm.lviewMonitor.SelectedItems)
			{
				int netId = Convert.ToInt32(item.SubItems[(int)guiMonitor.MONITOR_TABS.ID].Text);
				MOG_ControllerSystem.RetaskAssignedSlaveCommand(netId);
			}			
		}

		public void DislayAddConnection(String name, String ip, Int32 Id, MOG_COMMAND_TYPE connectionType, String info)
		{
			ListViewItem item = new ListViewItem();
			Color typeColor = Color.Black;

			// Setup new connection node
			item.Text = name;
			item.SubItems.Add(ip);
			item.SubItems.Add(Convert.ToString(Id));

			string connectionTypeString;
			switch(connectionType)
			{
				case MOG_COMMAND_TYPE.MOG_COMMAND_RegisterClient:
					connectionTypeString = "CLIENT";
					typeColor = Color.DarkBlue;
					break;
				case MOG_COMMAND_TYPE.MOG_COMMAND_RegisterSlave:
					connectionTypeString = "SLAVE";
					typeColor = Color.Purple;
					break;
				case MOG_COMMAND_TYPE.MOG_COMMAND_RegisterEditor:
					connectionTypeString = "EDITOR";
					typeColor = Color.SlateGray;
					break;
				default:
					connectionTypeString = "SERVER";
					break;
			}

			item.SubItems.Add(connectionTypeString);
			item.SubItems.Add(info);
			item.ForeColor = typeColor;

			mainForm.lviewMonitor.Items.Add(item);
			return;
		}

		public void DislayDeleteAllConnections()
		{
			//this needs to walk the connections and add all the connected computers.

			foreach(ListViewItem item in mainForm.lviewMonitor.Items)
			{
				mainForm.lviewMonitor.Items.Remove(item);
			}
		}

		public void DisplayDeleteConnectionsByID(int id)
		{
			ListViewItem foundItem = null;

			foreach(ListViewItem item in mainForm.lviewMonitor.Items)
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
				mainForm.lviewMonitor.Items.Remove(foundItem);
			}
		}

		private ListViewItem LocateListViewItem(ListView view, Int32 id)
		{
			foreach (ListViewItem item in view.Items)
			{
				if (Convert.ToInt32(item.SubItems[(int)MONITOR_TABS.ID].Text) == id)
				{
                    return item;
				}
			}
			return null;
		}

		public string FilterDesiredCommand(MOG_Command command)
		{
			MOG_CommandServerCS commandServer = (MOG_CommandServerCS)MOG_ControllerSystem.GetCommandManager();

			// Describe the current command of this connection
			string description = "";

			// Check if this command was assigned to a slave?
			MOG_Command slave;
			if (command.GetAssignedSlaveID() != 0)
			{
				// Attempt to find the registered slave associated with this command?
				slave = commandServer.LocateRegisteredSlaveByID(command.GetAssignedSlaveID());
			}
			else
			{
				// Attempt to find the registered slave associated with this command?
				slave = commandServer.LocateRegisteredSlaveByID(command.GetNetworkID());
			}
			// Check if we found a matching slave?
			if (slave != null)
			{
				// Update the status of this slave based on the command info
				switch(command.GetCommandType())
				{
					// Reset the description to Idle?
					case MOG_COMMAND_TYPE.MOG_COMMAND_ConnectionNew:
					case MOG_COMMAND_TYPE.MOG_COMMAND_RegisterSlave:
					case MOG_COMMAND_TYPE.MOG_COMMAND_ViewUpdate:
					case MOG_COMMAND_TYPE.MOG_COMMAND_Complete:
					case MOG_COMMAND_TYPE.MOG_COMMAND_Postpone:
					case MOG_COMMAND_TYPE.MOG_COMMAND_Failed:
						// Always revert the slave into "Idle"...
						description = "Idle";
						break;
					
					default:
						// Build the generic command description
						description = GetGenericCommandDescription(command);
						break;
				}
				// Since we know this was assigned to a slave, return what ever description we generated
				return description;
			}

			// Attempt to find the registered editor by this command's computer name?
			MOG_Command editor = commandServer.LocateAssociatedEditorByClientID(command.GetNetworkID());
			if (editor != null)
			{
				switch(command.GetCommandType())
				{
					// Only allow the Following commands to build a unique description
					case MOG_COMMAND_TYPE.MOG_COMMAND_NetworkPackageMerge:
					case MOG_COMMAND_TYPE.MOG_COMMAND_LocalPackageMerge:
					case MOG_COMMAND_TYPE.MOG_COMMAND_NetworkPackageRebuild:
					case MOG_COMMAND_TYPE.MOG_COMMAND_LocalPackageRebuild:
						// Build the generic command description
						description = GetGenericCommandDescription(command);
						break;

					default:
						// Always revert the editor into "Idle"...
						description = "Idle";
						break;
				}
				// Since we know this was assigned to ab editor, return what ever description we generated
				return description;
			}

			// Attempt to find the registered client associated with this command?
			MOG_Command client = commandServer.LocateClientByID(command.GetNetworkID());
			if (client != null)
			{
				// Use the RegisteredClient as the command so we get the standard description...
				description = GetClientStatusDescription(client);
				return description;
			}

			// Always default everything to "Idle"...
			description = "Idle";
			return description;
		}

		public string GetGenericCommandDescription(MOG_Command command)
		{
			// Describe this command by starting with the basics
			string description = "COMMAND: " + command.ToString();
			// Is there is an AssetName?
			if (command.GetAssetFilename().GetOriginalFilename().Length != 0)
			{
				description += "     ASSET: " + command.GetAssetFilename().GetOriginalFilename();

				// Is there a user within the asset's filename?
				if (command.GetAssetFilename().GetUserName().Length > 0)
				{
					description += "     USER: " + command.GetAssetFilename().GetUserName();
				}

				if (command.GetAssetFilename().GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_SlaveTask)
				{
					description += "     SLAVETASK: " + command.GetAssetFilename().GetFilename();
				}
			}
			return description;
		}

		public string GetClientStatusDescription(MOG_Command command)
		{
			MOG_CommandServerCS commandServer = (MOG_CommandServerCS)MOG_ControllerSystem.GetCommandManager();

			// Describe the current command of this connection
			string description = "";

			// Check if we have a ProjectName specified?
			if (command.GetProject().Length > 0)
			{
				// Add on our ProjectName
				description += "PROJECT: " + command.GetProject();
			}

			// Check if we have a ProjectName specified?
			if (command.GetBranch().Length > 0)
			{
				// Add on our ProjectName
				description += "     BRANCH: " + command.GetBranch();
			}

			// Check if we have a UserName specified?
			if (command.GetUserName().Length > 0)
			{
				// Add on our ProjectName
				description += "     USER: " + command.GetUserName();
			}

			// Attempt to locate the current view settings for this client
			MOG_Command viewsCommand = commandServer.LocateActiveViewByID(command.GetNetworkID());
			if (viewsCommand != null)
			{
				// Check if we have a Tab specified?
				if (viewsCommand.GetTab().Length > 0)
				{
					description += "     TAB: " + viewsCommand.GetTab();
				}
				// Check if we have a Platform specified?
				if (viewsCommand.GetPlatform().Length > 0)
				{
					description += "     PLATFORM: " + viewsCommand.GetPlatform();
				}
				// Check if we have a Tab specified?
				if (viewsCommand.GetUserName().Length > 0)
				{
					description += "     INBOX: " + viewsCommand.GetUserName();
				}
			}

			return description;
		}

		public void DisplayUpdateExistingConnections(MOG_Command command)
		{
			ListViewItem item;

			// Construct the command description
			string description = FilterDesiredCommand(command);
			if (description == null)
				return;

			// Check if this command was assigned to a slave?
			if (command.GetAssignedSlaveID() != 0)
			{
				// Find the connection by the assigned slave's network id
				item = LocateListViewItem(mainForm.lviewMonitor, command.GetAssignedSlaveID());
			}
			else
			{
				// Find the connection by network id
				item = LocateListViewItem(mainForm.lviewMonitor, command.GetNetworkID());
			}

			switch(command.GetCommandType())
			{
				// Delete connection?
				case MOG_COMMAND_TYPE.MOG_COMMAND_ConnectionKill:
				case MOG_COMMAND_TYPE.MOG_COMMAND_ShutdownClient:
				case MOG_COMMAND_TYPE.MOG_COMMAND_ShutdownSlave:
				case MOG_COMMAND_TYPE.MOG_COMMAND_ShutdownEditor:
				case MOG_COMMAND_TYPE.MOG_COMMAND_ShutdownCommandLine:
					if (item != null)
					{
						item.Remove();
					}
					break;

				// Add connection?
				case MOG_COMMAND_TYPE.MOG_COMMAND_RegisterClient:
				case MOG_COMMAND_TYPE.MOG_COMMAND_RegisterSlave:
				case MOG_COMMAND_TYPE.MOG_COMMAND_RegisterEditor:
				case MOG_COMMAND_TYPE.MOG_COMMAND_RegisterCommandLine:
					if (item == null)
					{
						// This is a new connection, add it
						item = new ListViewItem();

						// Construct the new item
						item.Text = command.GetComputerName();
						item.SubItems.Add(command.GetComputerIP());
						item.SubItems.Add(command.GetNetworkID().ToString());
						item.SubItems.Add(command.GetDescription());
						item.SubItems.Add(command.ToString());
						item.SubItems[(int)MONITOR_TABS.INFORMATION].Text = description;
						item.ForeColor = Color.Black;
						// Add the new item
						mainForm.lviewMonitor.Items.Add(item);
					}
					break;

				default:
					// Always update the command desription
					if (item != null && item.SubItems.Count>=(int)MONITOR_TABS.INFORMATION)
					{
						item.SubItems[(int)MONITOR_TABS.INFORMATION].Text = description;
					}
					break;
			}
		}

		public void DisplayRefreshSlaveStatus()
		{
			//this needs to walk the connections and add all the connected computers.
			MOG_CommandServerCS commandServer = (MOG_CommandServerCS)MOG_ControllerSystem.GetCommandManager();
			if (commandServer == null)
			{
				return;
			}
			
			ArrayList slavesArray = commandServer.GetRegisteredSlaves();
			foreach(Object item in slavesArray)
			{
				MOG_Command connection = (MOG_Command)item;

				if (connection.GetCommandType() == MOG_COMMAND_TYPE.MOG_COMMAND_None)
				{
					// Find item
					ListViewItem lItem = LocateListViewItem(mainForm.lviewMonitor, connection.GetNetworkID());

					if (lItem!=null)
					{
						if (string.Compare(lItem.SubItems[(int)MONITOR_TABS.INFORMATION].Text, "Idle") != 0)
						{
							lItem.SubItems[(int)MONITOR_TABS.INFORMATION].Text = "Idle";
						}
					}
				}								
			}
		}

		public void DisplayUpdateExistingConnections()
		{
			//first let's kill the existing ones.
			foreach(ListViewItem item in mainForm.lviewMonitor.Items)
			{
				mainForm.lviewMonitor.Items.Remove(item);
			}

			//this needs to walk the connections and add all the connected computers.
			MOG_CommandServerCS commandServer = (MOG_CommandServerCS)MOG_ControllerSystem.GetCommandManager();
			if (commandServer == null)
			{
				return;
			}
			
			ArrayList slavesArray = new ArrayList(commandServer.GetRegisteredSlaves());
			ArrayList clientsArray = new ArrayList(commandServer.GetRegisteredClients());
			ArrayList editorsArray = new ArrayList(commandServer.GetRegisteredEditors());

			//show the server first.
			DislayAddConnection(MOG_ControllerSystem.GetComputerName(), MOG_ControllerSystem.GetComputerIP(), 1, 0, "");

			foreach(Object item in clientsArray)
			{
				MOG_Command connection = (MOG_Command)item;

				// Describe the current command of this connection
				string description = FilterDesiredCommand(connection);
				if (description != null)
				{
					DislayAddConnection(connection.GetComputerName(), connection.GetComputerIP(), connection.GetNetworkID(), MOG_COMMAND_TYPE.MOG_COMMAND_RegisterClient, description);
				}
			}

			foreach(Object item in editorsArray)
			{
				MOG_Command connection = (MOG_Command)item;

				// Describe the current command of this connection
				string description = FilterDesiredCommand(connection);
				if (description != null)
				{
					DislayAddConnection(connection.GetComputerName(), connection.GetComputerIP(), connection.GetNetworkID(), MOG_COMMAND_TYPE.MOG_COMMAND_RegisterEditor, description);
				}
			}

			foreach(Object item in slavesArray)
			{
				MOG_Command connection = (MOG_Command)item;
				string description;

				// Check if this Slave is actively assigned anything?
				MOG_Command workingCommand = connection.GetCommand();
				if (workingCommand == null)
				{
					// Describe the current command of this connection
					description = FilterDesiredCommand(connection);
				}
				else
				{
					// Describe the current command of this connection's command
					description = FilterDesiredCommand(workingCommand);
				}
				
				if (description != null)
				{
					DislayAddConnection(connection.GetComputerName(), connection.GetComputerIP(), connection.GetNetworkID(), MOG_COMMAND_TYPE.MOG_COMMAND_RegisterSlave, description);
				}
			}
		}
	}
}
