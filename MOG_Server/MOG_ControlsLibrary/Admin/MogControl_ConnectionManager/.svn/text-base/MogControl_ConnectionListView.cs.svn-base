using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;

using MOG;
using MOG.COMMAND;
using MOG.FILENAME;
using MOG.CONTROLLER.CONTROLLERSYSTEM;

using MOG_Server.Server_Mog_Utilities;
using MOG_Server.Server_Utilities;
using MOG_Server.MOG_ControlsLibrary.Common;


namespace MOG_Server.MOG_ControlsLibrary.Admin
{
	/// <summary>
	/// Summary description for MogControl_ConnectionListView.
	/// </summary>
	public class MogControl_ConnectionListView : ListViewFilter.ListViewFilter
	{
		private ArrayList slaveList = new ArrayList();
		private ArrayList clientList = new ArrayList();
		private ArrayList editorList = new ArrayList();
		private ArrayList serverList = new ArrayList();

		private Color serverColor = Color.Purple;
		private Color clientColor = Color.Blue;
		private Color editorColor = Color.YellowGreen;
		private Color slaveColor = Color.Red;

		private ContextMenu contextMenu;
		private MenuItem miKillConnection;
		private MenuItem miRefresh;
		private MenuItem miRetask;
		private MenuItem miLaunchSlave;

		private bool allowKillConnection = true;
		private bool allowRefresh = true;
		private bool allowRetask = true;
		private bool allowLaunchSlave = true;

		private bool showKillConnection = true;
		private bool showRefresh = true;
		private bool showRetask = true;
		private bool showLaunchSlave = true;


		private const int INFORMATION_INDEX = 4;
		
		public EventCallbacks EventCallbacks
		{
			set { value.NewConnection += new MOG_NewConnectionHandler(NewConnectionHandler); }
		}

		public bool AllowKillConnection
		{
			get { return this.allowKillConnection; }
			set { this.allowKillConnection = value; RebuildContextMenu(); }
		}
		public bool AllowRefresh
		{
			get { return this.allowRefresh; }
			set { this.allowRefresh = value; RebuildContextMenu(); }
		}
		public bool AllowRetask
		{
			get { return this.allowRetask; }
			set { this.allowRetask = value; RebuildContextMenu(); }
		}
		public bool AllowLaunchSlave
		{
			get { return this.allowLaunchSlave; }
			set { this.allowLaunchSlave = value; RebuildContextMenu(); }
		}

		public bool ShowKillConnection
		{
			get { return this.showKillConnection; }
			set { this.showKillConnection = value; RebuildContextMenu(); }
		}
		public bool ShowRefresh
		{
			get { return this.showRefresh; }
			set { this.showRefresh = value; RebuildContextMenu(); }
		}
		public bool ShowRetask
		{
			get { return this.showRetask; }
			set { this.showRetask = value; RebuildContextMenu(); }
		}
		public bool ShowLaunchSlave
		{
			get { return this.showLaunchSlave; }
			set { this.showLaunchSlave = value; RebuildContextMenu(); }
		}

		public Color ServerColor
		{
			get { return this.serverColor; }
			set { this.serverColor = value; }
		}
		public Color ClientColor
		{
			get { return this.clientColor; }
			set { this.clientColor = value; }
		}
		public Color EditorColor
		{
			get { return this.editorColor; }
			set { this.editorColor = value; }
		}
		public Color SlaveColor
		{
			get { return this.slaveColor; }
			set { this.slaveColor = value; }
		}

		public void AutoSizeColumns()
		{
			foreach (ColumnHeader col in this.Columns)
				col.Width = -2;
		}

		public void NewConnectionHandler(object sender, MOG_NewConnectionEventArgs e)
		{
			//RefreshConnections();
			DisplayUpdateExistingConnections(e.connectionCommand);
		}

		public MogControl_ConnectionListView()
		{
			// events
			this.KeyUp += new KeyEventHandler(MogControl_ConnectionListView_KeyUp);

			// list view options
			this.HideSelection = false;
			this.FullRowSelect = true;

			// context menu
			RebuildContextMenu();

			// list view columns
			this.Columns.Add("Machine Name", 200, HorizontalAlignment.Left);
			this.Columns.Add("IP Address", 200, HorizontalAlignment.Center);
			this.Columns.Add("Network ID", 200, HorizontalAlignment.Center);
			this.Columns.Add("Type", 200, HorizontalAlignment.Left);
			this.Columns.Add("Information", 200, HorizontalAlignment.Left);
		}

		public MogControl_ConnectionListView(EventCallbacks eventCallbacks)
		{
			// events
			this.KeyUp += new KeyEventHandler(MogControl_ConnectionListView_KeyUp);

			this.EventCallbacks = eventCallbacks;

			// list view options
			this.HideSelection = false;
			this.FullRowSelect = true;

			// context menu
			RebuildContextMenu();

			// list view columns
			this.Columns.Add("Machine Name", 200, HorizontalAlignment.Left);
			this.Columns.Add("IP Address", 200, HorizontalAlignment.Center);
			this.Columns.Add("Network ID", 200, HorizontalAlignment.Center);
			this.Columns.Add("Type", 200, HorizontalAlignment.Left);
			this.Columns.Add("Information", 200, HorizontalAlignment.Left);
		}

		private void RebuildContextMenu()
		{
			this.miRefresh = new MenuItem("&Refresh", new EventHandler(miRefresh_Click), Shortcut.F5);
			this.miRefresh.DefaultItem = true;
			this.miKillConnection = new MenuItem("&Kill Connection", new EventHandler(miKillConnection_Click), Shortcut.CtrlK);
			this.miRetask = new MenuItem("Re&task", new EventHandler(miRetask_Click), Shortcut.CtrlR);
			this.miLaunchSlave = new MenuItem("&Launch Slave", new EventHandler(miLaunchSlave_Click), Shortcut.CtrlL);

			if (!this.allowRefresh)
				this.miRefresh.Enabled = false;
			if (!this.allowRetask)
				this.miRetask.Enabled = false;
			if (!this.allowLaunchSlave)
				this.miLaunchSlave.Enabled = false;
			if (!this.allowKillConnection)
				this.miKillConnection.Enabled = false;

			this.contextMenu = new ContextMenu();
			if (this.showRefresh)
			{
				this.contextMenu.MenuItems.Add(this.miRefresh);
			}
			if (this.showRetask)
			{
				this.contextMenu.MenuItems.Add(new MenuItem("-"));
				this.contextMenu.MenuItems.Add(this.miRetask);
			}
			if (this.showLaunchSlave)
				this.contextMenu.MenuItems.Add(this.miLaunchSlave);
			if (this.showKillConnection)
			{
				this.contextMenu.MenuItems.Add(new MenuItem("-"));
				this.contextMenu.MenuItems.Add(this.miKillConnection);
			}

			this.contextMenu.Popup += new EventHandler(contextMenu_Popup);
			this.ContextMenu = this.contextMenu;
		}

		private void KillSelected()
		{
			foreach (ListViewItem item in this.SelectedItems)
			{
				KillConnection(  Int32.Parse(item.SubItems[2].Text)  );
			}
		}

		private bool  KillConnection(int id)
		{
			return MOG_ControllerSystem.ConnectionKill(Convert.ToInt32(id));
		}

		private void LaunchSlaveOnSelected()
		{
			foreach (ListViewItem item in this.SelectedItems)
			{
				LaunchSlave(  Int32.Parse(item.SubItems[2].Text)  );
			}
		}

		private void LaunchSlave(int id)
		{
			MOG_ControllerSystem.LaunchSlave(id);
		}

		private void RetaskSelected()
		{
			foreach (ListViewItem item in this.SelectedItems)
			{
				Retask(  Int32.Parse(item.SubItems[2].Text)  );
			}
		}

		private void Retask(int slaveNetworkID)
		{
			MOG_ControllerSystem.RetaskAssignedSlaveCommand(slaveNetworkID);
		}

		private void contextMenu_Popup(object sender, EventArgs e)
		{
			if (this.SelectedItems.Count <= 0)
			{
				this.miKillConnection.Enabled = false;
				this.miRetask.Enabled = false;
				this.miLaunchSlave.Enabled = false;
			}
			else
			{
				if (this.allowKillConnection)
					this.miKillConnection.Enabled = true;
				if (this.allowRetask)
					this.miRetask.Enabled = true;
				if (this.allowLaunchSlave)
					this.miLaunchSlave.Enabled = true;
			}
		}

		private void miRefresh_Click(object sender, EventArgs e)
		{
			RefreshConnections();
		}

		private void miKillConnection_Click(object sender, EventArgs e)
		{
			KillSelected();
		}

		private void miRetask_Click(object sender, EventArgs e)
		{
			RetaskSelected();
		}

		private void miLaunchSlave_Click(object sender, EventArgs e)
		{
			LaunchSlaveOnSelected();
		}

		public void KeyHandler(object sender, KeyEventArgs e)
		{
			MogControl_ConnectionListView_KeyUp(sender, e);
		}

		private void MogControl_ConnectionListView_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.F5)
			{
				RefreshConnections();
			}
			else if (e.KeyData == Keys.F4)
			{
                AutoSizeColumns();
			}
		}

		public void RefreshConnections()
		{
			RefreshLists();

			this.Items.Clear();
			
			// servers
			foreach (MOG_Command serverCmd in this.serverList)
			{
				ListViewItem item = new ListViewItem(serverCmd.GetComputerName());
				item.SubItems.Add( serverCmd.GetComputerIP() );
				item.SubItems.Add(serverCmd.GetNetworkID().ToString());
				item.SubItems.Add( "SERVER" );
				item.SubItems.Add( "information" );
				SetForeColor(item, this.serverColor);

				this.Items.Add(item);
			}

			// clients
			foreach (MOG_Command serverCmd in this.clientList)
			{
				ListViewItem item = new ListViewItem(serverCmd.GetComputerName());
				item.SubItems.Add( serverCmd.GetComputerIP() );
				item.SubItems.Add(serverCmd.GetNetworkID().ToString());
				item.SubItems.Add( "CLIENT" );
				item.SubItems.Add( "information" );
				SetForeColor(item, this.clientColor);

				this.Items.Add(item);
			}

			// slaves
			foreach (MOG_Command serverCmd in this.slaveList)
			{
				ListViewItem item = new ListViewItem(serverCmd.GetComputerName());
				item.SubItems.Add( serverCmd.GetComputerIP() );
				item.SubItems.Add(serverCmd.GetNetworkID().ToString());
				item.SubItems.Add( "SLAVE" );
				item.SubItems.Add( "information" );
				SetForeColor(item, this.slaveColor);

				this.Items.Add(item);
			}

			// editors
			foreach (MOG_Command serverCmd in this.editorList)
			{
				ListViewItem item = new ListViewItem(serverCmd.GetComputerName());
				item.SubItems.Add( serverCmd.GetComputerIP() );
				item.SubItems.Add(serverCmd.GetNetworkID().ToString());
				item.SubItems.Add( "SERVER" );
				item.SubItems.Add( "information" );
				SetForeColor(item, this.editorColor);

				this.Items.Add(item);
			}

            AutoSizeColumns();
		}

		private void RefreshLists()
		{
			// get connection lists
			MOG_CommandServerCS commandServer = (MOG_CommandServerCS)MOG_ControllerSystem.GetCommandManager();
			if (commandServer == null)
				return;
			
			this.slaveList = commandServer.GetRegisteredSlaves();
			this.clientList = commandServer.GetRegisteredClients();
			this.editorList = commandServer.GetRegisteredEditors();
			this.serverList = new ArrayList();

			// build server's "command"
			MOG_Command command = new MOG_Command();
			command.SetComputerName( MOG_ControllerSystem.GetComputerName() );
			command.SetComputerIP( MOG_ControllerSystem.GetComputerIP() );
			command.SetNetworkID( 1 );
			command.SetCommandType( 0 );
			command.SetDescription( "" );
			this.serverList.Add( command );
		}

		private void SetForeColor(ListViewItem item, Color color)
		{
			item.ForeColor = color;
			for (int i = 0; i < item.SubItems.Count; i++)
				item.SubItems[i].ForeColor = color;
		}


		#region Information generating functions (from Server_Gui\Monitor.cs)
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
				item = Utils.GetListViewItem(this, command.GetAssignedSlaveID().ToString(), "Network ID");
			}
			else
			{
				// Find the connection by network id
				item = Utils.GetListViewItem(this, command.GetNetworkID().ToString(), "Network ID");
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
						item.SubItems.Add(description);
						item.ForeColor = Color.Black;
						// Add the new item
						this.Items.Add(item);
					}
					break;

				default:
					// Always update the command desription
					if (item != null)
					{
						item.SubItems[INFORMATION_INDEX].Text = description;
					}
					break;
			}
		}

		public string FilterDesiredCommand(MOG_Command command)
		{
			MOG_CommandServerCS commandServer = (MOG_CommandServerCS)MOG_ControllerSystem.GetCommandManager();
			bool match = false;

			// Describe the current command of this connection
			string description = "";

			// Since we haven't found our match yet, keep looking...
			if (!match)
			{
				MOG_Command slave;

				// Check if this command was assigned to a slave?
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
					switch(command.GetCommandType())
					{
						// Only allow the Following commands to build a unique description
						case MOG_COMMAND_TYPE.MOG_COMMAND_AssetRipRequest:
						case MOG_COMMAND_TYPE.MOG_COMMAND_AssetProcessed:
						case MOG_COMMAND_TYPE.MOG_COMMAND_SlaveTask:
						case MOG_COMMAND_TYPE.MOG_COMMAND_ReinstanceAssetRevision:
						case MOG_COMMAND_TYPE.MOG_COMMAND_Bless:
						case MOG_COMMAND_TYPE.MOG_COMMAND_Post:
						case MOG_COMMAND_TYPE.MOG_COMMAND_Archive:
						case MOG_COMMAND_TYPE.MOG_COMMAND_NetworkPackageMerge:
						case MOG_COMMAND_TYPE.MOG_COMMAND_LocalPackageMerge:
						case MOG_COMMAND_TYPE.MOG_COMMAND_NetworkPackageRebuild:
						case MOG_COMMAND_TYPE.MOG_COMMAND_LocalPackageRebuild:
						case MOG_COMMAND_TYPE.MOG_COMMAND_RemoveAssetFromProject:
						case MOG_COMMAND_TYPE.MOG_COMMAND_BuildFull:
						case MOG_COMMAND_TYPE.MOG_COMMAND_Build:
							match = true;
							break;

						case MOG_COMMAND_TYPE.MOG_COMMAND_ConnectionNew:
						case MOG_COMMAND_TYPE.MOG_COMMAND_RegisterSlave:
						case MOG_COMMAND_TYPE.MOG_COMMAND_ViewUpdate:
						case MOG_COMMAND_TYPE.MOG_COMMAND_Complete:
							// Always revert the slave into "Idle"...
							description = "Idle";
							return description;

						
						default:
							// Don't change the existing description...
							description = "Unknown - " + command.ToString();
							return description;
					}
				}
			}

			// Since we haven't found our match yet, keep looking...
			if (!match)
			{
				// Attempt to find the registered slave associated with this command?
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
							match = true;
							break;

						default:
							// Use the RegisteredClient as the command so we get the standard description...
							description = GetDetailedCommandDescription(editor);
							return description;
					}
				}
			}

			// Since we haven't found our match yet, keep looking...
			if (!match)
			{
				// Attempt to find the registered client associated with this command?
				MOG_Command client = commandServer.LocateClientByID(command.GetNetworkID());
				if (client != null)
				{
					switch(command.GetCommandType())
					{
							// Only allow the Following commands to build a unique description
						case MOG_COMMAND_TYPE.MOG_COMMAND_AssetRipRequest:
						case MOG_COMMAND_TYPE.MOG_COMMAND_AssetProcessed:
						case MOG_COMMAND_TYPE.MOG_COMMAND_SlaveTask:
						case MOG_COMMAND_TYPE.MOG_COMMAND_ReinstanceAssetRevision:
						case MOG_COMMAND_TYPE.MOG_COMMAND_Bless:
						case MOG_COMMAND_TYPE.MOG_COMMAND_Post:
						case MOG_COMMAND_TYPE.MOG_COMMAND_Archive:
						case MOG_COMMAND_TYPE.MOG_COMMAND_RemoveAssetFromProject:
						case MOG_COMMAND_TYPE.MOG_COMMAND_NetworkPackageMerge:
						case MOG_COMMAND_TYPE.MOG_COMMAND_LocalPackageMerge:
						case MOG_COMMAND_TYPE.MOG_COMMAND_NetworkPackageRebuild:
						case MOG_COMMAND_TYPE.MOG_COMMAND_LocalPackageRebuild:
						case MOG_COMMAND_TYPE.MOG_COMMAND_BuildFull:
							//case MOG_COMMAND_TYPE.MOG_COMMAND_Build:
							break;

						default:
							// Use the RegisteredClient as the command so we get the standard description...
							description = GetDetailedCommandDescription(client);
							return description;
					}
				}
			}

			// Use the RegisteredClient as the command so we get the standard description...
			description = GetDetailedCommandDescription(command);
			return description;
		}

		public string GetDetailedCommandDescription(MOG_Command command)
		{
			MOG_CommandServerCS commandServer = (MOG_CommandServerCS)MOG_ControllerSystem.GetCommandManager();

			// Describe the current command of this connection
			string description = "";

			// Check if this command is a MOG_COMMAND_RegisterClient?
			// Check if this command is a MOG_COMMAND_RegisterEditor?
			switch (command.GetCommandType())
			{
				case MOG_COMMAND_TYPE.MOG_COMMAND_RegisterClient:
				case MOG_COMMAND_TYPE.MOG_COMMAND_RegisterEditor:
				case MOG_COMMAND_TYPE.MOG_COMMAND_LoginProject:
				case MOG_COMMAND_TYPE.MOG_COMMAND_LoginUser:
				case MOG_COMMAND_TYPE.MOG_COMMAND_ActiveViews:
					// Indicate the LoginProject
					if (command.GetProject() != null && command.GetProject().Length > 0)
					{
						if (description.Length > 0)
						{
							description = String.Concat(description, ", ");
						}
						description = String.Concat(description, command.GetProject());
					}

					// Indicate the LoginUser
					if (command.GetUserName() != null && command.GetUserName().Length > 0)
					{
						if (description.Length > 0)
						{
							description = String.Concat(description, ", ");
						}
						description = String.Concat(description, command.GetUserName());
					}

					// Attempt to locate the current view settings for this client
					MOG_Command viewsCommand = commandServer.LocateActiveViewByID(command.GetNetworkID());
					if (viewsCommand != null)
					{
						// Attempt to show the ActiveTab
						if (viewsCommand.GetTab() != null && viewsCommand.GetTab().Length > 0)
						{
							if (description.Length > 0)
							{
								description = String.Concat(description, ", ");
							}
							description = String.Concat(description, viewsCommand.GetTab());
						}
						// Attempt to show the ActivePlatform
						if (viewsCommand.GetPlatform() != null && viewsCommand.GetPlatform().Length > 0)
						{
							if (description.Length > 0)
							{
								description = String.Concat(description, ", ");
							}
							description = String.Concat(description, viewsCommand.GetPlatform());
						}
						// Attempt to show the ActiveUser
						if (viewsCommand.GetUserName() != null && viewsCommand.GetUserName().Length > 0)
						{
							if (description.Length > 0)
							{
								description = String.Concat(description, ", ");
							}
							description = String.Concat(description, viewsCommand.GetUserName());
						}
					}
					return description;
			}


			description = command.ToString();

			// If there is an Asset associated with the command?, show it as well
			if (command.GetAssetFilename().GetFilename().Length != 0)
			{
				if (command.GetAssetFilename().GetUserName().Length > 0)
				{
					description = String.Concat(description, " for ", command.GetAssetFilename().GetUserName());

					if (command.GetAssetFilename().GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_SlaveTask)
					{
						string temp;
						int start, len;
                            
						temp = command.GetAssetFilename().GetPath();
	
						start = command.GetAssetFilename().GetBoxPath().Length + 1;
						len = temp.LastIndexOf("\\") - start;

						if ( start > 0 && len != -1)
						{
							temp = temp.Substring(start, len);

							description = String.Concat(description, " - ", temp);
						}
					}
					else
					{
						description = String.Concat(description, " - ", command.GetAssetFilename().GetFilename());
					}
				}
			}

			return description;
		}
		#endregion
	}
}
