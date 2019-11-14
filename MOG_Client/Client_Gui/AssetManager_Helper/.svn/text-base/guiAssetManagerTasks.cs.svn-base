using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.IO;

using MOG_Client.Client_Mog_Utilities;
using MOG.USER;
using MOG.INI;
using MOG.COMMAND;
using MOG.FILENAME;
using MOG.REPORT;
using MOG.TIME;
using MOG.CONTROLLER.CONTROLLERTASK;

using EV.Windows.Forms;

namespace MOG_Client.Client_Gui.AssetManager_Helper
{
	/// <summary>
	/// Summary description for guiAssetManagerTasks.
	/// </summary>
	public class guiAssetManagerTasks
	{
		guiAssetManager mParent;
		int mNewTasksCount;

		public guiAssetManagerTasks(guiAssetManager parent)
		{
			mParent = parent;
			mNewTasksCount = 0;
			InitializeTaskMenu();
			
			// Tasks
			mParent.mListViewSort_Manager.Add(new ListViewSortManager(mParent.mainForm.AssetManagerInboxTasksListViewFilter, new Type[] {
															typeof(ListViewTextCaseInsensitiveSort),
															typeof(ListViewDateSort),
															typeof(ListViewDateSort),
															typeof(ListViewTextCaseInsensitiveSort),
															typeof(ListViewTextCaseInsensitiveSort),
															typeof(ListViewTextCaseInsensitiveSort),
															typeof(ListViewTextCaseInsensitiveSort)
														}));
			mParent.mListViewSort_Manager.Add(new ListViewSortManager(mParent.mainForm.AssetManagerOutboxTasksListViewFilter, new Type[] {
															typeof(ListViewTextCaseInsensitiveSort),
															typeof(ListViewDateSort),
															typeof(ListViewDateSort),
															typeof(ListViewTextCaseInsensitiveSort),
															typeof(ListViewTextCaseInsensitiveSort),
															typeof(ListViewTextCaseInsensitiveSort),
															typeof(ListViewTextCaseInsensitiveSort)
														}));
		}

		public void InitializeTaskMenu()
		{
			if ( mParent.mMog.IsProject() )
			{
				// Add all the users to the AssignTo subMenu
				ArrayList users = mParent.mMog.GetProject().GetUsers();
				for (int u = 0; u < users.Count; u++)
				{
					MOG_User user = (MOG_User)users[u];
					MenuItem Item = new MenuItem(user.GetUserName());
					Item.Click += new System.EventHandler(this.mnuItmSendToTask_Click);
					mParent.mainForm.AssetManagerInboxTasksSendToMenuItem.MenuItems.Add(Item);
				}

			}
		}

		private void mnuItmSendToTask_Click(object sender, System.EventArgs e)
		{
			guiAssetController asset = new guiAssetController(mParent.mainForm);

			string comment = "Temp empty comment";

			// Make sure a node is selected before popup is allowed
			if (mParent.mainForm.AssetManagerInboxTasksListViewFilter.SelectedItems.Count != 0)
			{
				// This code will only work in the inbox :(
				foreach(ListViewItem item in mParent.mainForm.AssetManagerInboxTasksListViewFilter.SelectedItems)
				{
					asset.SendToTask(item.SubItems[(int)guiAssetManager.TaskBoxColumns.FULLNAME].Text, comment);
				}																	   
			}
		}

		public void RefreshBox(MOG_Filename add, MOG_Filename del, MOG_Command command)
		{
			Color textColorAdd = Color.Black;
			Color textColorDel = Color.Black;
	
			// Dont add if we dont have a valid box
			if (add.GetBoxName().Length == 0 && del.GetBoxName().Length == 0)
			{
				return;
			}

			ListView currentViewAdd = mParent.IsolateListView(add.GetBoxName(), add.GetType(), add.GetUserName(), ref textColorAdd);
			ListView currentViewDel = mParent.IsolateListView(del.GetBoxName(), del.GetType(), del.GetUserName(), ref textColorDel);

			if (currentViewAdd == currentViewDel)
			{
				// Check to see if this item already exists?
				int index = mParent.ListViewItemFindItem(add.GetFilename(), currentViewAdd);
				if (index != -1)
				{
					currentViewAdd.Items[index].SubItems[(int)guiAssetManager.TaskBoxColumns.STATUS].Text = command.GetDescription();
					currentViewAdd.Items[index].SubItems[(int)guiAssetManager.TaskBoxColumns.FULLNAME].Text = add.GetFullFilename();
					currentViewAdd.Items[index].ForeColor = textColorAdd;
					for (int x = 0; x < currentViewAdd.Items[index].SubItems.Count; x++)
					{
						currentViewAdd.Items[index].SubItems[x].ForeColor = textColorAdd;
					}
				}					
			}
			else
			{
				if (currentViewDel != null)
				{
					int index = mParent.ListViewItemFindItem(del.GetFilename(), currentViewDel);
					if (index != -1)
					{
						currentViewDel.Items[index].Remove();						
					}					
				}
					
				if (currentViewAdd != null)
				{
					// Check to see if this item already exists?
					int index = mParent.ListViewItemFindItem(add.GetFilename(), currentViewAdd);
					if (index != -1)
					{
						currentViewAdd.Items[index].SubItems[(int)guiAssetManager.TaskBoxColumns.STATUS].Text = command.GetDescription();
						currentViewAdd.Items[index].SubItems[(int)guiAssetManager.TaskBoxColumns.FULLNAME].Text = add.GetFullFilename();
						currentViewAdd.Items[index].ForeColor = textColorAdd;
						for (int x = 0; x < currentViewAdd.Items[index].SubItems.Count; x++)
						{
							currentViewAdd.Items[index].SubItems[x].ForeColor = textColorAdd;
						}
					}
					else
					{
						MOG_ControllerTask assetController = new MOG_ControllerTask(mParent.mMog);
						if (!assetController.Open(add.GetFullFilename()))
						{
							return;
						}

						ListViewItem item = new ListViewItem();

						item.Text = assetController.GetTitle();

						MOG_Time t = new MOG_Time();
						t.SetTimeStamp(assetController.GetDueDate());
						item.SubItems.Add(t.ToDateTime().ToString());

						item.SubItems.Add(assetController.GetFileInfo().LastWriteTime.ToString());
						item.SubItems.Add(assetController.GetCreator());
						item.SubItems.Add(assetController.GetPriority());
						item.SubItems.Add(assetController.GetStatus());
						item.SubItems.Add(assetController.GetAsset());
						item.SubItems.Add(assetController.GetAssetFilename().GetFullFilename());
						item.SubItems.Add(assetController.GetAssetFilename().GetBoxName());
						item.ForeColor = Color.Black;
						for (int x = 0; x < item.SubItems.Count; x++)
						{
							item.SubItems[x].ForeColor = textColorAdd;
						}

						item.ImageIndex = 0;//SetAssetIcon(String.Concat(mParent.mMog.GetActiveUser().GetUserPath(), "\\", box, "\\", assetName));
						currentViewAdd.Items.Add(item);

						assetController.Close();
					}
				}
			}

            // Update the tab
			RefreshTab(command);					
		}
		/// <summary>
		/// Re-populates the inbox asset window withthe contents 
		/// of the inbox\contents.info file
		/// </summary>
		public void RefreshInbox()
		{
			// Skip this if we don't have an active project
			if ( !mParent.mMog.IsProject() )
			{
				return;
			}

			// Clear the desired listBox
			mParent.mainForm.AssetManagerInboxTasksListViewFilter.Items.Clear();

			// Get the tasks for the tasks window			
			GetTasksFromBox("Inbox", "Tasks", Color.Black, mParent.mainForm.AssetManagerInboxTasksListViewFilter);
		}

		public void RefreshTab(MOG_Command task)
		{
			if (string.Compare(task.GetDescription(), "New", true) == 0)
			{
				// Update our new message count
				mNewTasksCount++;
			}

			// Determine which boxes we are looking at, Inbox or outBox
			string boxName = "Unknown";
			TabPage pagePointer = null;
			if (string.Compare(mParent.mainForm.AssetManagerBoxesTabControl.SelectedTab.Name,"AssetManagerInboxTabPage") == 0)
			{
				boxName = "Inbox";
				pagePointer = mParent.mainForm.AssetManagerInboxTabControl.TabPages[(int)guiAssetManager.InboxTabOrder.TASKS];
			}
			else if (string.Compare(mParent.mainForm.AssetManagerBoxesTabControl.SelectedTab.Name,"AssetManagerOutboxTabPage") == 0)
			{
				boxName = "Outbox";
				pagePointer = mParent.mainForm.AssetManagerOutboxTabControl.TabPages[(int)guiAssetManager.OutboxTabOrder.TASKS];
			}

			string tabName;
			if (mNewTasksCount > 0)
			{
				tabName = string.Concat("Tasks ", boxName, "(", mNewTasksCount.ToString(), ") New");
			}
			else
			{
				tabName = string.Concat("Tasks ", boxName);
			}

			if (pagePointer != null) pagePointer.Text = tabName;
		}

		public void RefreshTab()
		{
			int newItems = 0;
			MOG_Ini contents = new MOG_Ini();

			// Determine which boxes we are looking at, Inbox or outBox
			string boxName = "Unknown";
			TabPage pagePointer = null;
			if (string.Compare(mParent.mainForm.AssetManagerBoxesTabControl.SelectedTab.Name,"AssetManagerInboxTabPage") == 0)
			{
				boxName = "Inbox";
				pagePointer = mParent.mainForm.AssetManagerInboxTabControl.TabPages[(int)guiAssetManager.InboxTabOrder.TASKS];
			}
			else if (string.Compare(mParent.mainForm.AssetManagerBoxesTabControl.SelectedTab.Name,"AssetManagerOutboxTabPage") == 0)
			{
				boxName = "Outbox";
				pagePointer = mParent.mainForm.AssetManagerOutboxTabControl.TabPages[(int)guiAssetManager.OutboxTabOrder.TASKS];
			}
			else
			{
				MOG_REPORT.ShowMessageBox("ERROR", "No valid box selected in mAssetManager.Tasks.RefreshTab", MessageBoxButtons.OK);
				return;
			}

			// Check all tasks for the new status
			FileInfo file = new FileInfo(String.Concat(mParent.mMog.GetActiveUser().GetUserPath(), "\\", boxName, "\\Contents.info"));
		
			// If the .info file exists, open it
			if (file.Exists)
			{
				// Load the file
				contents.Load(file.FullName);

				// Find the items in the TASKS section
				if ( contents.SectionExist("Tasks") )
				{

					for ( int i = 0; i < contents.CountKeys("Tasks"); i++ )
					{
						string taskName = contents.GetKeyNameByIndex("Tasks", i);
						string taskStatus = contents.GetString(taskName, "Status");
						if (string.Compare( taskStatus, "New", true) == 0)
						{
							newItems++;
						}
					}
				}
			}

			// If we have new items, update the tab
			string tabName;
			if (newItems > 0)
			{
				tabName =  string.Concat("Tasks ", boxName, "(", newItems.ToString(), ") New");				
			}
			else
			{
				tabName =  string.Concat("Tasks ", boxName);
			}

			// Store count
			mNewTasksCount = newItems;

			// Set the tab name
			if (pagePointer != null) pagePointer.Text = tabName;
		}

		/// <summary>
		/// Re-populates the inbox asset window withthe contents 
		/// of the inbox\contents.info file
		/// </summary>
		public void RefreshOutbox()
		{
			// Skip this if we don't have an active project
			if ( !mParent.mMog.IsProject() )
			{
				return;
			}

			// Clear the desired listBox
			mParent.mainForm.AssetManagerOutboxTasksListViewFilter.Items.Clear();

			// Get the tasks for the tasks window			
			GetTasksFromBox("Outbox", "Tasks", Color.Black, mParent.mainForm.AssetManagerOutboxTasksListViewFilter);
		}

		public bool GetTasksFromBox(string box, string section, Color textColor, System.Windows.Forms.ListView listViewToFill)
		{
			MOG_Ini contents = new MOG_Ini();

			// Get a handle to the inbox\contents.info
			FileInfo file = new FileInfo(String.Concat(mParent.mMog.GetActiveUser().GetUserPath(), "\\", box, "\\Contents.info"));

			// If the .info file exists, open it
			if (file.Exists)
			{
				// Load the file
				contents.Load(file.FullName );

				// Find the items in the INBOX section
				if ( contents.SectionExist(section) )
				{

					for ( int i = 0; i < contents.CountKeys(section); i++ )
					{
						ListViewItem node = new ListViewItem();

						String assetName = contents.GetKeyNameByIndex(section, i);

						// Set the due date
						MOG_Time t = new MOG_Time();
						t.SetTimeStamp(contents.GetString(assetName, "DUEDATE"));
						DateTime dueDate = t.ToDateTime();

						node.Text = (contents.GetString(assetName, "TITLE"));			// Name
						node.SubItems.Add(dueDate.ToString());
						node.SubItems.Add(contents.GetString(assetName, "TIME"));
						node.SubItems.Add(contents.GetString(assetName, "CREATOR"));
						node.SubItems.Add(contents.GetString(assetName, "PRIORITY"));
						node.SubItems.Add(contents.GetString(assetName, "STATUS"));
						node.SubItems.Add(contents.GetString(assetName, "ASSET"));
						node.SubItems.Add(String.Concat(file.DirectoryName, "\\", assetName));	// Fullname
						node.SubItems.Add(box);											// Current box
						node.ForeColor = textColor;

						node.ImageIndex = 0;//SetAssetIcon(String.Concat(mParent.mMog.GetActiveUser().GetUserPath(), "\\", box, "\\", assetName));
						listViewToFill.Items.Add(node);						
					}
				}

				contents.Close();
			}

			return true;
		}

		public void TaskCreateNew()
		{
			guiAssetController tasks = new guiAssetController(mParent.mainForm);
			tasks.CreateTask();
		}

		public void TaskBless()
		{
			guiAssetController asset = new guiAssetController(mParent.mainForm);

			string comment = "Temp empty comment";

			// Make sure a node is selected before popup is allowed
			if (mParent.mainForm.AssetManagerInboxTasksListViewFilter.SelectedItems.Count != 0)
			{
				// This code will only work in the inbox :(
				foreach(ListViewItem item in mParent.mainForm.AssetManagerInboxTasksListViewFilter.SelectedItems)
				{
					asset.BlessTask(item.SubItems[(int)guiAssetManager.TaskBoxColumns.FULLNAME].Text, comment);
				}																	   
			}
		}

		public void TaskEdit()
		{
			// Make sure a node is selected before popup is allowed
			if (mParent.mainForm.AssetManagerInboxTasksListViewFilter.SelectedItems.Count != 0)
			{
				// This code will only work in the inbox :(
				foreach(ListViewItem item in mParent.mainForm.AssetManagerInboxTasksListViewFilter.SelectedItems)
				{
					guiAssetController tasks = new guiAssetController(mParent.mainForm);
					tasks.EditTask(item.SubItems[(int)guiAssetManager.TaskBoxColumns.FULLNAME].Text);
				}
			}
		}

		public void TaskReject()
		{
			guiAssetController asset = new guiAssetController(mParent.mainForm);

			string comment = "Temp empty comment";

			// Make sure a node is selected before popup is allowed
			if (mParent.mainForm.AssetManagerInboxTasksListViewFilter.SelectedItems.Count != 0)
			{
				// This code will only work in the inbox :(
				foreach(ListViewItem item in mParent.mainForm.AssetManagerInboxTasksListViewFilter.SelectedItems)
				{
					asset.RejectTask(item.SubItems[(int)guiAssetManager.TaskBoxColumns.FULLNAME].Text, comment);
				}																	   
			}
		}

		public void TaskOpen()
		{
			// We can only accept one selected item on a double click
			if (mParent.mainForm.AssetManagerInboxTasksListViewFilter.SelectedItems.Count != 0)
			{
				// TODO (kier) This needs to select the asset associated with the task in the Explorer tree
				foreach(ListViewItem item in mParent.mainForm.AssetManagerInboxTasksListViewFilter.SelectedItems)
				{
					string fullPath = item.SubItems[(int)guiAssetManager.TaskBoxColumns.ASSET].Text;

					if (string.Compare(fullPath, "None", true) != 0)
					{
						guiCommandLine.ShellExecute(fullPath);
					}
				}
			}
		}

		public void TaskRemove()
		{
			guiAssetController asset = new guiAssetController(mParent.mainForm);

			// We can only accept one selected item on a double click
			if (mParent.mainForm.AssetManagerInboxTasksListViewFilter.SelectedItems.Count != 0)
			{
				// TODO (kier) This needs to select the asset associated with the task in the Explorer tree
				foreach(ListViewItem item in mParent.mainForm.AssetManagerInboxTasksListViewFilter.SelectedItems)
				{
					asset.RemoveTask(item.SubItems[(int)guiAssetManager.TaskBoxColumns.FULLNAME].Text);
				}
			}
		}

	}
}
