using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.IO;

using MOG.COMMAND;
using MOG_Client;
using MOG.INI;
using MOG.REPORT;
using MOG.FILENAME;
using MOG.CONTROLLER;
using MOG.CONTROLLER.CONTROLLERMESSAGE;

using EV.Windows.Forms;

namespace MOG_Client.Client_Gui.AssetManager_Helper
{
	/// <summary>
	/// Summary description for Messages.
	/// </summary>
	public class guiAssetManagerMessages
	{
		guiAssetManager mParent;
		int mNewMessageCount;

		public guiAssetManagerMessages(guiAssetManager Parent)
		{
			mParent = Parent;
			mNewMessageCount = 0;

			mParent.mListViewSort_Manager.Add(new ListViewSortManager(mParent.mainForm.AssetManagerInboxMessagesListViewFilter, new Type[] {
													typeof(ListViewTextCaseInsensitiveSort),
													typeof(ListViewTextCaseInsensitiveSort),
													typeof(ListViewDateSort),
													typeof(ListViewTextCaseInsensitiveSort),
													typeof(ListViewTextCaseInsensitiveSort),
													typeof(ListViewTextCaseInsensitiveSort)
												}));
			mParent.mListViewSort_Manager.Add(new ListViewSortManager(mParent.mainForm.AssetManagerOutboxMessagesListViewFilter, new Type[] {
													typeof(ListViewTextCaseInsensitiveSort),
													typeof(ListViewTextCaseInsensitiveSort),
													typeof(ListViewDateSort),
													typeof(ListViewTextCaseInsensitiveSort),
													typeof(ListViewTextCaseInsensitiveSort),
													typeof(ListViewTextCaseInsensitiveSort)
												}));

		}

		public void MessageCreate()
		{
			MessageForm newMessage = new MessageForm(mParent.mainForm);
			
			newMessage.Create();

			newMessage.Show();
		}

		public void MessageEdit()
		{
			MessageForm newMessage = new MessageForm(mParent.mainForm);

			ListView box;

			// Get the correct box
			if (String.Compare(mParent.mainForm.AssetManagerBoxesTabControl.SelectedTab.Name, "AssetManagerInboxTabPage", true) == 0)
			{
				box = mParent.mainForm.AssetManagerInboxMessagesListViewFilter;
			}

			else if (String.Compare(mParent.mainForm.AssetManagerBoxesTabControl.SelectedTab.Name, "AssetManagerOutboxTabPage", true) == 0)
			{
				box = mParent.mainForm.AssetManagerOutboxMessagesListViewFilter;
			}
			else
			{
				box = null;
				return;
			}
			
			// We can only edit one message no more or less
			if (box.SelectedItems.Count == 1)
			{
				foreach(ListViewItem item in box.SelectedItems)
				{
					newMessage.Open(item);
					newMessage.Show();
					return;
				}
			}
			else
			{
				MOG_REPORT.ShowMessageBox("Edit message", "You can only edit one message at a time.", MessageBoxButtons.OK);
				return;
			}
		}

		public void MessageRemove()
		{
			// We can only accept one selected item on a double click
			if (mParent.mainForm.AssetManagerInboxMessagesListViewFilter.SelectedItems.Count != 0)
			{
				// TODO (kier) This needs to select the asset associated with the task in the Explorer tree
				foreach(ListViewItem item in mParent.mainForm.AssetManagerInboxMessagesListViewFilter.SelectedItems)
				{
					MOG_ControllerMessage message = new MOG_ControllerMessage(mParent.mMog);
					
					if (message.Open(item.SubItems[(int)guiAssetManager.MessageBoxColumns.FULLNAME].Text))
					{
						message.Delete();
						message.Close();
					}
				}
			}

		}

		public int ListViewItemMessagesFindItem(string name, ListView list)
		{
			foreach (ListViewItem item in list.Items)
			{
				if(string.Compare(name, item.SubItems[(int)guiAssetManager.AssetBoxColumns.FULLNAME].Text, true) == 0)
				{
					return item.Index;
				}
			}

			return -1;
		}

		public void RefreshBox(MOG_Filename add, MOG_Filename del, MOG_Command command)
		{
			Color textColorAdd = Color.Black;
			Color textColorDel = Color.Black;
			ListView currentViewAdd = mParent.IsolateListView(add.GetBoxName(), add.GetType(), add.GetUserName(), ref textColorAdd);
			ListView currentViewDel = mParent.IsolateListView(del.GetBoxName(), del.GetType(), del.GetUserName(), ref textColorDel);

			if (currentViewAdd == currentViewDel)
			{
				// Check to see if this item already exists?
				int index = ListViewItemMessagesFindItem(add.GetFullFilename(), currentViewAdd);
				if (index != -1)
				{
					currentViewAdd.Items[index].SubItems[(int)guiAssetManager.MessageBoxColumns.STATUS].Text = command.GetDescription();
					currentViewAdd.Items[index].SubItems[(int)guiAssetManager.MessageBoxColumns.BOX].Text = add.GetBoxName();
					currentViewAdd.Items[index].SubItems[(int)guiAssetManager.MessageBoxColumns.FULLNAME].Text = add.GetFullFilename();
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
					int index = ListViewItemMessagesFindItem(del.GetFullFilename(), currentViewDel);
					if (index != -1)
					{
						currentViewDel.Items[index].Remove();						
					}					
				}
					
				if (currentViewAdd != null)
				{
					// Check to see if this item already exists?
					int index = ListViewItemMessagesFindItem(add.GetFullFilename(), currentViewAdd);
					if (index != -1)
					{
						currentViewAdd.Items[index].SubItems[(int)guiAssetManager.MessageBoxColumns.STATUS].Text = command.GetDescription();
						currentViewAdd.Items[index].SubItems[(int)guiAssetManager.MessageBoxColumns.FULLNAME].Text = add.GetFullFilename();
						currentViewAdd.Items[index].ForeColor = textColorAdd;
						for (int x = 0; x < currentViewAdd.Items[index].SubItems.Count; x++)
						{
							currentViewAdd.Items[index].SubItems[x].ForeColor = textColorAdd;
						}
					}
					else
					{
						MOG_ControllerMessage assetController = new MOG_ControllerMessage(mParent.mMog);
						if (!assetController.Open(add.GetFullFilename()))
						{
							return;
						}

						// Create the newly added node
						ListViewItem  item = mParent.CreateListViewNode(assetController.GetSubject(),
							assetController.GetFrom(),
							assetController.GetFileInfo().LastWriteTime.ToString(),
							assetController.GetTo(),
							assetController.GetStatus(),
							"",
							assetController.GetAssetFilename().GetFullFilename(),
							assetController.GetAssetFilename().GetBoxName(),
							Color.Black );

						currentViewAdd.Items.Add(item);

						assetController.Close();
					}
				}
			}

			// Update the Tab
			RefreshTab(command);								
		}
		
		public void RefreshInbox()
		{
			// Skip this if we don't have an active project
			if ( !mParent.mMog.IsProject() )
			{
				return;
			}

			RefreshTabClear();

			// Clear the desired listBox
			mParent.mainForm.AssetManagerInboxMessagesListViewFilter.Items.Clear();

			// Get the tasks for the tasks window			
			GetTasksFromBox("Inbox", "Messages", Color.Black, mParent.mainForm.AssetManagerInboxMessagesListViewFilter);
		}

		public void RefreshInbox(MOG_Command asset, MOG_Filename source, MOG_Filename destination)
		{
			// Skip this if we don't have an active project
			if ( !mParent.mMog.IsProject() )
			{
				return;
			}

			// Check for remove
			if (destination.GetFullFilename().Length == 0)
			{
				// We are removing
				int index = mParent.ListViewItemFindItem(source.GetFilename(), mParent.mainForm.AssetManagerInboxMessagesListViewFilter);
				if (index != -1)
				{
					// Remove it from our list
					mParent.mainForm.AssetManagerInboxAssetListView.Items[index].Remove();
				}
			}// Check for new message
			else if (source.GetFullFilename().Length == 0)
			{
				// We are importing
				MOG_Controller assetController = MOG_Controller.CreateController(mParent.mMog, destination.GetFullFilename());
				if (!assetController.Open(destination.GetFullFilename()))
				{
					return;
				}

				// Check if this message is to go to the outbox or inbox
				if (string.Compare(destination.GetBoxName(), "Outbox", true) == 0)
				{
					// Create the newly added node
					ListViewItem  item = mParent.CreateListViewNode(assetController.GetPropertiesFile().GetString("MESSAGE", "SUBJECT"),
						assetController.GetPropertiesFile().GetString("MESSAGE", "FROM"),
						assetController.GetPropertiesFile().GetString("ASSET", "TIME"),
						"ATTACHMENTS",
						assetController.GetPropertiesFile().GetString("MESSAGE", "CC"),
						"",
						assetController.GetAssetFilename().GetFullFilename(),
						assetController.GetAssetFilename().GetBoxName(),
						Color.Black );
				
					// Add it to our listView
					mParent.mainForm.AssetManagerOutboxMessagesListViewFilter.Items.Add(item);
				}
				else
				{
					// Create the newly added node
					ListViewItem  item = mParent.CreateListViewNode(assetController.GetPropertiesFile().GetString("MESSAGE", "SUBJECT"),
						assetController.GetPropertiesFile().GetString("MESSAGE", "TO"),
						assetController.GetPropertiesFile().GetString("ASSET", "TIME"),
						"ATTACHMENTS",
						assetController.GetPropertiesFile().GetString("MESSAGE", "CC"),
						"",
						assetController.GetAssetFilename().GetFullFilename(),
						assetController.GetAssetFilename().GetBoxName(),
						Color.Blue );
				
					// Add it to our listView
					mParent.mainForm.AssetManagerInboxMessagesListViewFilter.Items.Add(item);
				}

				assetController.Close();
			}
		}

		public void RefreshTab(MOG_Command message)
		{
			if (string.Compare(message.GetDescription(), "New", true) == 0)
			{
				// Update our new message count
				mNewMessageCount++;
			}

			// Determine which boxes we are looking at, Inbox or outBox
			string boxName;
			if (string.Compare(mParent.mainForm.AssetManagerBoxesTabControl.SelectedTab.Name,"AssetManagerInboxTabPage") == 0)
			{
				boxName = "Inbox";
			}
			else if (string.Compare(mParent.mainForm.AssetManagerBoxesTabControl.SelectedTab.Name,"AssetManagerOutboxTabPage") == 0 )
			{
				boxName = "Outbox";
			}
			else
			{
				boxName = "Unknown";
			}

			string tabName;
			if (mNewMessageCount > 0)
			{
				tabName = string.Concat("Messages ", boxName, "(", mNewMessageCount.ToString(), ") New");
			}
			else
			{
				tabName = string.Concat("Messages ", boxName);
			}

			mParent.mainForm.AssetManagerBoxesTabControl.SelectedTab.Text = tabName;
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
				pagePointer = mParent.mainForm.AssetManagerInboxTabControl.TabPages[(int)guiAssetManager.InboxTabOrder.MESSAGES];
			}
			else if (string.Compare(mParent.mainForm.AssetManagerBoxesTabControl.SelectedTab.Name,"AssetManagerOutboxTabPage") == 0)
			{
				boxName = "Outbox";
				pagePointer = mParent.mainForm.AssetManagerOutboxTabControl.TabPages[(int)guiAssetManager.OutboxTabOrder.MESSAGES];
			}
			else
			{
				MOG_REPORT.ShowMessageBox("ERROR", "No valid box selected in mAssetManager.Messages.RefreshTab", MessageBoxButtons.OK);
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
				if ( contents.SectionExist("Messages") )
				{

					for ( int i = 0; i < contents.CountKeys("Messages"); i++ )
					{
						string messageName = contents.GetKeyNameByIndex("Messages", i);
						string messageStatus = contents.GetString(messageName, "Status");
						if (string.Compare( messageStatus, "New", true) == 0)
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
				tabName =  string.Concat("Messages ", boxName, "(", newItems.ToString(), ") New");				
			}
			else
			{
				tabName =  string.Concat("Messages ", boxName);
			}

			// Store this count
			mNewMessageCount = newItems;

			// Set the tab name
			if (pagePointer != null) pagePointer.Text = tabName;
		}

		public void RefreshTabClear()
		{
			mNewMessageCount = 0;

			// Determine which boxes we are looking at, Inbox or outBox
			string boxName = "Unknown";
			TabPage pagePointer = null;
			if (string.Compare(mParent.mainForm.AssetManagerBoxesTabControl.SelectedTab.Name,"AssetManagerInboxTabPage") == 0)
			{
				boxName = "Inbox";
				pagePointer = mParent.mainForm.AssetManagerInboxTabControl.TabPages[(int)guiAssetManager.InboxTabOrder.MESSAGES];
			}
			else if (string.Compare(mParent.mainForm.AssetManagerBoxesTabControl.SelectedTab.Name,"AssetManagerOutboxTabPage") == 0)
			{
				boxName = "Outbox";
				pagePointer = mParent.mainForm.AssetManagerOutboxTabControl.TabPages[(int)guiAssetManager.OutboxTabOrder.MESSAGES];
			}

			string tabName = string.Concat("Messages ", boxName);
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
			mParent.mainForm.AssetManagerOutboxMessagesListViewFilter.Items.Clear();

			// Get the tasks for the tasks window			
			GetTasksFromBox("Outbox", "Messages", Color.Black, mParent.mainForm.AssetManagerOutboxMessagesListViewFilter);
		}

		public bool GetTasksFromBox(string box, string section, Color textColor, ListView listViewToFill)
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

						node.Text = (contents.GetString(assetName, "SUBJECT"));			// Name
						node.SubItems.Add(contents.GetString(assetName, "TO"));
						node.SubItems.Add(contents.GetString(assetName, "TIME"));
						node.SubItems.Add(contents.GetString(assetName, "FROM"));
						node.SubItems.Add(contents.GetString(assetName, "STATUS"));
						node.SubItems.Add("");
						node.SubItems.Add(String.Concat(file.DirectoryName, "\\", assetName));	// Fullname
						node.SubItems.Add(box);											// Current box
						node.ForeColor = textColor;

						if (string.Compare(contents.GetString(assetName, "STATUS"), "New", true) == 0)
						{
							node.Font = new Font(node.Font, FontStyle.Bold);
						}

						node.ImageIndex = 0;//SetAssetIcon(String.Concat(mParent.mMog.GetActiveUser().GetUserPath(), "\\", box, "\\", assetName));
						listViewToFill.Items.Add(node);						
					}
				}
				contents.Close();
			}

			return true;
		}

	}
}
