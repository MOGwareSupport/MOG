using System;
using System.Collections;
using System.Windows.Forms;

using Server_Gui;
using EV.Windows.Forms;

using MOG;
using MOG.COMMAND;
using System.Drawing;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.REPORT;
using System.Collections.Generic;



namespace MOG_Server.Server_Gui
{
	/// <summary>
	/// Summary description for guiPendingCommands.
	/// </summary>
	public class guiPendingCommands
	{
		private FormMainSMOG mainForm;
		public enum CommandsColumns {TYPE, ASSET, MACHINE, IP, ID, COMMANDID, TIME, SLAVE, FULLNAME};
        private ArrayList mListViewSort_Manager;

		public guiPendingCommands(FormMainSMOG main)
		{
			mainForm = main;

			Initialize();

			// Assets
			mListViewSort_Manager = new ArrayList();
			mListViewSort_Manager.Add(new ListViewSortManager(mainForm.CommandspendingListView, new Type[] {
																						   typeof(ListViewTextCaseInsensitiveSort),
																						   typeof(ListViewTextCaseInsensitiveSort),
																						   typeof(ListViewTextCaseInsensitiveSort),
																						   typeof(ListViewTextCaseInsensitiveSort),
																						   typeof(ListViewInt32Sort),
																						   typeof(ListViewInt32Sort),
																							typeof(ListViewDateSort),
																						   typeof(ListViewTextCaseInsensitiveSort)																						   
																					   }));
			

		}

		private void Initialize()
		{

		}

		private ListViewItem LocateItem(MOG_Command command, ListView view)
		{
			// This is a much faster way to check if the command already exists in our list
			if (view.Items.ContainsKey(command.GetCommandID().ToString()))
			{
				return view.Items[mainForm.CommandspendingListView.Items.IndexOfKey(command.GetCommandID().ToString())];
			}

			return null;
		}

		public void KillCommand()
		{
			try
			{
				List<ListViewItem> killed = new List<ListViewItem>();

				foreach (ListViewItem item in mainForm.CommandspendingListView.SelectedItems)
				{
					UInt32 commandID = Convert.ToUInt32(item.SubItems[(int)CommandsColumns.COMMANDID].Text);
					if (MOG_ControllerSystem.KillCommandFromServer(commandID))
					{
						killed.Add(item);
					}
				}

				foreach (ListViewItem item in killed)
				{
					item.Remove();
				}
			}
			catch (Exception e)
			{
				MOG_Report.ReportMessage("Kill Command Error", e.Message, e.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.ERROR);
			}
		}

		public void RefreshWindow(MOG_Command command)
		{
			if (command == null) 
			{
				GetCommandSnapShot();
			}
			else
			{
				ListViewItem item = null;

				switch (command.GetCommandType())
				{
    				// Eat these commands, we don't need to show them in this window
					case MOG_COMMAND_TYPE.MOG_COMMAND_None:
					case MOG_COMMAND_TYPE.MOG_COMMAND_ConnectionKill:
					case MOG_COMMAND_TYPE.MOG_COMMAND_ConnectionLost:
					case MOG_COMMAND_TYPE.MOG_COMMAND_ConnectionNew:
					case MOG_COMMAND_TYPE.MOG_COMMAND_InstantMessage:
					case MOG_COMMAND_TYPE.MOG_COMMAND_LockCopy:
					case MOG_COMMAND_TYPE.MOG_COMMAND_LockMove:
					case MOG_COMMAND_TYPE.MOG_COMMAND_LockReadRelease:
					case MOG_COMMAND_TYPE.MOG_COMMAND_LockReadRequest:
					case MOG_COMMAND_TYPE.MOG_COMMAND_LockWriteRelease:
					case MOG_COMMAND_TYPE.MOG_COMMAND_LockWriteRequest:
					case MOG_COMMAND_TYPE.MOG_COMMAND_LoginProject:
					case MOG_COMMAND_TYPE.MOG_COMMAND_LoginUser:
					case MOG_COMMAND_TYPE.MOG_COMMAND_NetworkBroadcast:
					case MOG_COMMAND_TYPE.MOG_COMMAND_NewBranch:
					case MOG_COMMAND_TYPE.MOG_COMMAND_NotifyActiveCommand:
					case MOG_COMMAND_TYPE.MOG_COMMAND_NotifyActiveConnection:
					case MOG_COMMAND_TYPE.MOG_COMMAND_NotifyActiveLock:
					case MOG_COMMAND_TYPE.MOG_COMMAND_RegisterClient:
					case MOG_COMMAND_TYPE.MOG_COMMAND_RegisterCommandLine:
					case MOG_COMMAND_TYPE.MOG_COMMAND_RegisterEditor:
					case MOG_COMMAND_TYPE.MOG_COMMAND_RegisterSlave:
					case MOG_COMMAND_TYPE.MOG_COMMAND_RemoveAssetFromProject:
					case MOG_COMMAND_TYPE.MOG_COMMAND_RequestActiveCommands:
					case MOG_COMMAND_TYPE.MOG_COMMAND_RequestActiveConnections:
					case MOG_COMMAND_TYPE.MOG_COMMAND_ShutdownClient:
					case MOG_COMMAND_TYPE.MOG_COMMAND_ShutdownCommandLine:
					case MOG_COMMAND_TYPE.MOG_COMMAND_ShutdownEditor:
					case MOG_COMMAND_TYPE.MOG_COMMAND_ShutdownSlave:
					case MOG_COMMAND_TYPE.MOG_COMMAND_RefreshApplication:
					case MOG_COMMAND_TYPE.MOG_COMMAND_RefreshTools:
					case MOG_COMMAND_TYPE.MOG_COMMAND_RefreshProject:
					case MOG_COMMAND_TYPE.MOG_COMMAND_ViewConnection:
					case MOG_COMMAND_TYPE.MOG_COMMAND_ViewLock:
					case MOG_COMMAND_TYPE.MOG_COMMAND_ActiveViews:
					case MOG_COMMAND_TYPE.MOG_COMMAND_ViewUpdate:
					case MOG_COMMAND_TYPE.MOG_COMMAND_AssetProcessed:
						break;
					default:
						string commandString = command.ToString();

						if (command.IsCompleted())
						{
							item = LocateItem(command, mainForm.CommandspendingListView);							
							while(item!=null)
							{
								item.Remove();
								item = LocateItem(command, mainForm.CommandspendingListView);
							}
						}
						else
						{
							// See if it already exists
							if (LocateItem(command, mainForm.CommandspendingListView) == null)
							{
								item = new ListViewItem();

								item.Text = command.ToString();
								item.SubItems.Add(command.GetAssetFilename().GetOriginalFilename());
								item.SubItems.Add(command.GetComputerName().ToString());
								item.SubItems.Add(command.GetComputerIP().ToString());
								item.SubItems.Add(command.GetNetworkID().ToString());
								item.SubItems.Add(command.GetCommandID().ToString());
								item.SubItems.Add(command.GetCommandTimeStamp());
								item.SubItems.Add(command.GetComputerName());
								item.SubItems.Add(command.GetAssetFilename().GetOriginalFilename());
								item.ImageIndex = GetImageIndex(command.GetCommandType());

								// Assign the Key for this listViewItem
								item.Name = command.GetCommandID().ToString();

								mainForm.CommandspendingListView.Items.Add(item);
							}
						}

						break;
				}
			}
		}

		private int GetImageIndex(MOG_COMMAND_TYPE type)
		{
			switch (type)
			{
					// Eat these commands, we don't need to show them in this window
				case MOG_COMMAND_TYPE.MOG_COMMAND_Archive:
					return 1;
				case MOG_COMMAND_TYPE.MOG_COMMAND_AssetProcessed:
				case MOG_COMMAND_TYPE.MOG_COMMAND_AssetRipRequest:
				case MOG_COMMAND_TYPE.MOG_COMMAND_SlaveTask:
					return 0;
				case MOG_COMMAND_TYPE.MOG_COMMAND_ReinstanceAssetRevision:
				case MOG_COMMAND_TYPE.MOG_COMMAND_Bless:
					return 6;
                case MOG_COMMAND_TYPE.MOG_COMMAND_Build:
                case MOG_COMMAND_TYPE.MOG_COMMAND_BuildFull:
					return 2;
				default:
					return 3;
			}
		}

		public void GetCommandSnapShot()
		{
            // Clear our existing list
			mainForm.CommandspendingListView.Items.Clear();

            // Obtain the server
			MOG_CommandServerCS server = (MOG_CommandServerCS)(MOG_ControllerSystem.GetCommandManager());
            if (server != null)
            {
                // Get all the pending commands
			    ArrayList commands = server.GetAllPendingCommands();
			    foreach (MOG_Command command in commands)
			    {
				    ListViewItem item = new ListViewItem();

                    item.Text = command.ToString();
				    item.SubItems.Add(command.GetAssetFilename().GetOriginalFilename());
				    item.SubItems.Add(command.GetComputerName());
				    item.SubItems.Add(command.GetComputerIP());
				    item.SubItems.Add(command.GetNetworkID().ToString());
				    item.SubItems.Add(command.GetCommandID().ToString());
				    item.SubItems.Add(command.GetCommandTimeStamp());
				    item.SubItems.Add(command.GetComputerName());
				    item.SubItems.Add(command.GetAssetFilename().GetOriginalFilename());
				    item.ForeColor = Color.Black;
				    item.ImageIndex = GetImageIndex(command.GetCommandType());
				    item.Name = command.GetCommandID().ToString();
                    // Add the new item
				    mainForm.CommandspendingListView.Items.Add(item);
			    }
            }
		}
	}
}
