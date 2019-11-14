using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;

using MOG.INI;
using MOG.USER;
using MOG.COMMAND;
using MOG_Client.Client_Mog_Utilities;

using EV.Windows.Forms;

namespace MOG_Client.Client_Gui
{
	/// <summary>
	/// Summary description for guiChatManager.
	/// </summary> 
	public class guiChatManager
	{
		//private MogMainForm mainForm;
		public ChatForm mChatWindow;
		public ArrayList mListViewSort_Manager;

		public guiChatManager(MogMainForm main)
		{
//			mainForm = main;

//			Initialize();

			// Chat
/*			mListViewSort_Manager = new ArrayList();
			mListViewSort_Manager.Add(new ListViewSortManager(mainForm.ChatArchiveListView, new Type[] {
																																			typeof(ListViewDateSort),
																																			typeof(ListViewTextCaseInsensitiveSort),
																																			typeof(ListViewTextCaseInsensitiveSort),
																																			typeof(ListViewTextCaseInsensitiveSort),
																																			typeof(ListViewTextCaseInsensitiveSort)
																																		}));*/
		}
	/*
		private void Initialize()
		{
			// Reset the listView
			mainForm.ChatUsersListView.Items.Clear();

			// Add all the users to the list view
			foreach (MOG_User user in mainForm.gMog.GetProject().GetUsers())
			{
				ListViewItem item = new ListViewItem();
				
				item.Text = user.GetUserName();
				
				item.SubItems.Add(user.GetUserCategory());
				mainForm.ChatUsersListView.Items.Add(item);
			}
		}

		public void UpdateConnections(MOG_Command command)
		{
			foreach (ListViewItem item in mainForm.ChatUsersListView.Items)
			{
				if (command.GetCommand() != null)
				{
					switch(command.GetCommand().GetCommandType())
					{
						case MOG_COMMAND_TYPE.MOG_COMMAND_RegisterClient:
						case MOG_COMMAND_TYPE.MOG_COMMAND_LoginUser:
						case MOG_COMMAND_TYPE.MOG_COMMAND_ShutdownClient:
							if (string.Compare(item.Text, command.GetCommand().GetUser()) == 0)
							{
								item.ForeColor = Color.Green;
							}
							break;
					}
				}
			}
		}

		public void ChatStart(string users)
		{
			ChatStart(users, "");
		}

		public void ChatStart(string users, string handle)
		{
			string friends = "";

			// Make sure a node is selected before popup is allowed
			if (mainForm.ChatUsersListView.SelectedItems.Count != 0)
			{
				foreach (ListViewItem item in mainForm.ChatUsersListView.SelectedItems)
				{
					friends = string.Concat(friends, ",", item.Text);
				}
			}

			// Create our chat window
			mChatWindow = new ChatForm(mainForm, this, friends, handle);
			
			mChatWindow.Show();

			// Play sound
			mainForm.mSoundManager.PlayStatusSound("ClientEvents", "StartChat");
		}

		public void ChatStartRecieve(MOG_Command message)
		{
			// Create our chat window
			mChatWindow = new ChatForm(mainForm, this, message.GetDestination(), message.GetVersion());
			mChatWindow.Show();

			// Play sound
			mainForm.mSoundManager.PlayStatusSound("ClientEvents", "RecieveNewChat");
			
			mChatWindow.RecieveMessage(message);
		}

		public void ChatEnd(string handle)
		{
			if (string.Compare(mChatWindow.ChatGetHandle(), handle) == 0)
			{
				mChatWindow = null;
			}
		}

		public void ChatView()
		{
			// Make sure a node is selected before popup is allowed
			if (mainForm.ChatArchiveListView.SelectedItems.Count != 0)
			{
				foreach (ListViewItem item in mainForm.ChatArchiveListView.SelectedItems)
				{
					guiCommandLine.ShellSpawn("Notepad.exe", item.SubItems[(int)guiAssetManager.ChatArchiveColumns.FULLNAME].Text);
				}
			}
		}

		public void ChatRemove()
		{
			// Make sure a node is selected before popup is allowed
			if (mainForm.ChatArchiveListView.SelectedItems.Count != 0)
			{
				//				foreach (ListViewItem item in mParent.mainForm.AssetManagerInboxChatArchiveListView.SelectedItems)
			{
				// TODO Add chat archive remove here!
			}
			}
		}

		/// <summary>
		/// Re-populates the inbox asset window withthe contents 
		/// of the inbox\contents.info file
		/// </summary>
		public void Refresh()
		{
			// Skip this if we don't have an active project
			if ( !mainForm.gMog.IsProject() )
			{
				return;
			}

			// Clear the desired listBox
			mainForm.ChatArchiveListView.Items.Clear();

			// Get the active users
			mainForm.gMog.RequestActiveConnections();
            
			// Get the tasks for the tasks window			
			UpdateMessageWindow();
		}

		// Update the task window with the contents of the Tasks dir
		public void UpdateMessageWindow()
		{
			MOG_Ini messages = new MOG_Ini(String.Concat(mainForm.gMog.GetProject().GetProjectPath(), "\\InstantMessages\\Contents.info"));

			if (!messages.SectionExist("MESSAGES"))
			{
				return;
			}
			
			// Populate the form window
			for ( int x = 0; x < messages.CountKeys("MESSAGES"); x++ )
			{
				String section = messages.GetKeyNameByIndex("MESSAGES", x);

				ListViewItem item = new ListViewItem();

				item.Text = messages.GetString(section, "Time");
				item.SubItems.Add(messages.GetString(section, "Creator")); 
				item.SubItems.Add(messages.GetString(section, "Invited")); 
				item.SubItems.Add(messages.GetString(section, "ResponsibleParty")); 
				item.SubItems.Add(messages.GetString(section, "Status")); 
				item.SubItems.Add(String.Concat(mainForm.gMog.GetProject().GetProjectPath(), "\\InstantMessages\\", section)); 

				mainForm.ChatArchiveListView.Items.Add(item);
			}

			// Close and release lock on MFAT table
			messages.Close();
		}
		
		*/

	}
}
