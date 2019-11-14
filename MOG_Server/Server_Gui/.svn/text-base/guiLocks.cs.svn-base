using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;

using Server_Gui;
using MOG_Server.Server_Gui.guiConfigurationsHelpers;

using MOG;
using MOG.TIME;
using MOG.COMMAND;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using EV.Windows.Forms;



namespace MOG_Server.Server_Gui
{
	/// <summary>
	/// Summary description for guiLocks.
	/// </summary>
	public class guiLocks
	{
		private FormMainSMOG mainForm;
		private bool mResetLockList;

		public enum LOCK_COLUMNS {TYPE, FULLNAME, IP, TIME, MACHINE, USER, ID, DESCRIPTION, FULL_TYPE};

		public ArrayList mListViewSort_Manager;

		public guiLocks(FormMainSMOG main)
		{
			mainForm = main;

			mListViewSort_Manager = new ArrayList();
			// Locks
			mListViewSort_Manager.Add(new ListViewSortManager(mainForm.LocksListView, new Type[] {
																									 typeof(ListViewTextSort),
																									 typeof(ListViewTextSort),
																									 typeof(ListViewTextSort),
																									 typeof(ListViewDateSort),
																									 typeof(ListViewTextSort),
																									 typeof(ListViewInt32Sort),
																									 typeof(ListViewTextSort)
																								 }));
			// RequestLocks
			mListViewSort_Manager.Add(new ListViewSortManager(mainForm.LocksRequestLocksListView, new Type[] {
																									 typeof(ListViewTextSort),
																									 typeof(ListViewTextSort),
																									 typeof(ListViewTextSort),
																									 typeof(ListViewDateSort),
																									 typeof(ListViewTextSort),
																									 typeof(ListViewInt32Sort),
																									 typeof(ListViewTextSort)
																								 }));
			

			Initialize();
		}

		public void SetResetLocks(bool state)
		{
			mResetLockList = state;
		}

		private ListViewItem InitNewRequestItem(MOG_Command command)
		{
			if ( (command.GetCommandType() == MOG_COMMAND_TYPE.MOG_COMMAND_LockWriteRequest) || (command.GetCommandType() == MOG_COMMAND_TYPE.MOG_COMMAND_LockReadRequest))
			{
				ListViewItem item = new ListViewItem();

				MOG_Time time = new MOG_Time();
				time.SetTimeStamp(command.GetCommandTimeStamp());

				item.Text = command.ToString();
				item.SubItems.Add(command.GetAssetFilename().GetOriginalFilename());
				item.SubItems.Add(command.GetComputerIP());
				item.SubItems.Add(time.FormatString(""));
				item.SubItems.Add(command.GetComputerName());
				item.SubItems.Add(command.GetUserName());
				item.SubItems.Add(Convert.ToString(command.GetNetworkID()));
				item.SubItems.Add(command.GetDescription());
				item.SubItems.Add(command.ToString());

				switch(command.GetCommandType())
				{
					case MOG_COMMAND_TYPE.MOG_COMMAND_LockReadRequest:
						item.Text = "Read Lock";
						item.ForeColor = Color.Green;
						item.ImageIndex = 0;
						break;
					case MOG_COMMAND_TYPE.MOG_COMMAND_LockWriteRequest:
						item.Text = "Write Lock";
						item.ForeColor = Color.Red;
						item.ImageIndex = 0;
						break;
				}

				return item;
			}
			return null;
		}

		private ListViewItem InitNewLockItem(MOG_Command command)
		{
			if ( (command.GetCommandType() == MOG_COMMAND_TYPE.MOG_COMMAND_LockWriteRequest) || (command.GetCommandType() == MOG_COMMAND_TYPE.MOG_COMMAND_LockReadRequest))
			{
				ListViewItem item = new ListViewItem();
				
				MOG_Time time = new MOG_Time();
				time.SetTimeStamp(command.GetCommandTimeStamp());

				item.Text = "Lock";
				item.SubItems.Add(command.GetAssetFilename().GetOriginalFilename());
				item.SubItems.Add(command.GetComputerIP());
				item.SubItems.Add(time.FormatString(""));
				item.SubItems.Add(command.GetComputerName());
				item.SubItems.Add(command.GetUserName());
				item.SubItems.Add(Convert.ToString(command.GetNetworkID()));
				item.SubItems.Add(command.GetDescription());
				item.SubItems.Add(command.ToString());

				switch(command.GetCommandType())
				{
					case MOG_COMMAND_TYPE.MOG_COMMAND_LockReadRequest:
						item.Text = "Read Lock";
						item.ForeColor = Color.Green;
						item.ImageIndex = 2;
						break;
					case MOG_COMMAND_TYPE.MOG_COMMAND_LockWriteRequest:
						item.Text = "Write Lock";
						item.ForeColor = Color.Red;
						item.ImageIndex = 1;
						break;
				}

				return item;
			}
			return null;
		}

		private ListViewItem LocateListViewItem(ListView view, string fullName)
		{
			foreach (ListViewItem item in view.Items)
			{
				if (string.Compare(fullName, item.SubItems[(int)LOCK_COLUMNS.FULLNAME].Text, true) == 0)
				{
					return item;
				}
			}
			return null;
		}

		private ListViewItem LocateListViewItem(ListView view, string fullName, string machine)
		{
			foreach (ListViewItem item in view.Items)
			{
				if ((string.Compare(fullName, item.SubItems[(int)LOCK_COLUMNS.FULLNAME].Text, true) == 0) && (string.Compare(machine, item.SubItems[(int)LOCK_COLUMNS.MACHINE].Text, true) == 0))
				{
					return item;
				}
			}
			return null;
		}

		public void Initialize()
		{
			MOG_CommandServerCS server = (MOG_CommandServerCS)MOG_ControllerSystem.GetCommandManager();

			mainForm.LocksListView.Items.Clear();

			ArrayList activeWriteLocks = new ArrayList(server.GetActiveWriteLocks());
			foreach (MOG_Command myLock in activeWriteLocks)
			{	
				ListViewItem item = InitNewLockItem(myLock);

				if (item != null)
				{
					mainForm.LocksListView.Items.Add(item);
				}
			}

			ArrayList activeReadLocks = new ArrayList(server.GetActiveReadLocks());
			foreach (MOG_Command myLock in activeReadLocks)
			{				
				ListViewItem item = InitNewLockItem(myLock);

				if (item != null)
				{
					mainForm.LocksListView.Items.Add(item);
				}
			}

			mainForm.LocksRequestLocksListView.Items.Clear();
		}

		public void KillLock(string name, string type, string user, string cpu)
		{
			MOG_ControllerSystem.LockKill(name, type, user, cpu);
		}

		public void RefreshLockWindows(MOG_Command command)
		{
			if (string.Compare(mainForm.SMOG_Main_TabControl.SelectedTab.Text, "Locks", true) == 0)
			{
				// If the command completed then it is a successful lock, if not then it is a request
				if (command.IsCompleted())
				{
					switch(command.GetCommandType())
					{
						case MOG_COMMAND_TYPE.MOG_COMMAND_LockReadRequest:
						case MOG_COMMAND_TYPE.MOG_COMMAND_LockWriteRequest:

							// Check if this is a lock that was waiting and now has succedded
							ListViewItem oldReleaseLock = LocateListViewItem(mainForm.LocksRequestLocksListView, command.GetAssetFilename().GetOriginalFilename(), command.GetComputerName());
							if (oldReleaseLock != null)
							{
								oldReleaseLock.Remove();
							}
					
							// Create the new lock item
							ListViewItem gotLock = InitNewLockItem(command);

							if (gotLock != null)
							{
								mainForm.LocksListView.Items.Add(gotLock);
							}
							break;
						case MOG_COMMAND_TYPE.MOG_COMMAND_LockWriteRelease:
						case MOG_COMMAND_TYPE.MOG_COMMAND_LockReadRelease:
							ListViewItem releaseLock = LocateListViewItem(mainForm.LocksListView, command.GetAssetFilename().GetOriginalFilename(), command.GetComputerName());
							if (releaseLock != null)
							{
								releaseLock.Remove();
							}
							break;
					}
				}
				else
				{
					if (LocateListViewItem(mainForm.LocksRequestLocksListView, command.GetAssetFilename().GetOriginalFilename(), command.GetComputerName()) == null)
					{
						ListViewItem item = InitNewRequestItem(command);

						if (item != null)
						{
							mainForm.LocksRequestLocksListView.Items.Add(item);
						}
					}
				}
			}
		}
	}
}
