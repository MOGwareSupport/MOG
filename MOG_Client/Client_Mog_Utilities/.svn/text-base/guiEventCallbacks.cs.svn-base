using System;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Threading;

using MOG;
using MOG.COMMAND;
using MOG.DOSUTILS;
using MOG.FILENAME;
using MOG.INI;
using MOG.REPORT;
using MOG.PROMPT;
using MOG.PROPERTIES;
using MOG.CONTROLLER.CONTROLLERASSET;
using MOG.CONTROLLER.CONTROLLERINBOX;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.CONTROLLER.CONTROLLERSYNCDATA;
using MOG.ASSET_STATUS;

using MOG_Client.Forms;
using MOG_Client.Client_Gui;
using MOG_Client.Client_Mog_Utilities.AssetOptions;
using MOG_ControlsLibrary.Common.MogControl_RepositoryTreeViews;
using MOG_ControlsLibrary.Controls;
using System.IO;
using MOG_Client.Client_Gui.AssetManager_Helper;

namespace MOG_Client.Client_Mog_Utilities
{

	/// <summary>
	/// Summary description for EventCallbacks.
	/// </summary>
	public class guiEventCallbacks
	{
		public MogMainForm mainForm;
		public CallbackDialogForm mDialog;
		public ArrayList mDialogs;
		private int windowId;

		public guiEventCallbacks(MogMainForm main)
		{
			mainForm = main;

			// Initialize arraylist
			mDialogs = new ArrayList();
			windowId = 1;

			MOG_Callbacks callbacks = new MOG_Callbacks();

			callbacks.mPreEventCallback = new MOG_CallbackCommandEvent(this.CommandPreEventCallBack);
			callbacks.mEventCallback = new MOG_CallbackCommandEvent(this.CommandEventCallBack);
			callbacks.mCommandCallback = new MOG_CallbackCommandProcess(this.CommandProcessCallBack);

			if (!MOG_ControllerSystem.GetOffline() && MOG_ControllerSystem.IsCommandManager())
			{
				MOG_ControllerSystem.GetCommandManager().SetCallbacks(callbacks);
			}
		}

		bool DialogLocateSQLServer(int dialogId)
		{
			return MainMenuFileClass.MOGGlobalSQLLogin();
		}

		#region Dialog Callback Methods
		int DialogInitCallback(string title, string message, string button)
		{
			mainForm.ProcessTimer.Stop();
			CallbackDialogForm dialog = new CallbackDialogForm();

			dialog.Text = title;

			dialog.CallbackDialogFilenameLabel.Visible = false;
			dialog.CallbackDialogFilesProgressBar.Visible = false;

			dialog.CallbackDialoglMessageLabel.Text = message;

			// Check if the button string needs to be parsed
			if (button.IndexOf("/") != -1)
			{
				string[] buttons = button.Split(new Char[] { '/' });
				dialog.buttons = buttons;

				for (int i = 0; i < buttons.Length; i++)
				{
					switch (i)
					{
					case 0:
						dialog.CallbackDialogOptional1Button.Text = buttons[i];
						dialog.CallbackDialogOptional1Button.Visible = true;
						break;
					case 1:
						dialog.CallbackDialogOptional2Button.Text = buttons[i];
						dialog.CallbackDialogOptional2Button.Visible = true;
						break;
					case 2:
						dialog.CallbackDialogOptional3Button.Text = buttons[i];
						dialog.CallbackDialogOptional3Button.Visible = true;
						break;
					case 3:
						dialog.CallbackDialogOptional4Button.Text = buttons[i];
						dialog.CallbackDialogOptional4Button.Visible = true;
						break;
					default:
						// Error
						break;
					}
				}
			}
			else
			{
				dialog.CallbackDialogCancelButton.Visible = true;
				dialog.CallbackDialogCancelButton.Text = button;
			}

			mainForm.Activate();
			// This dialog should be pushed to topmost when it doesn't have a parent or else it can ger lost behind other apps.
			// You would think this is simple but for some reason MOG has really struggled with these dialogs being kept on top...
			// We have tried it all and finally ended up with this...toggling the TopMost mode seems to be working 100% of the time.
			dialog.TopMost = true;
			dialog.TopMost = false;
			dialog.TopMost = true;
			dialog.Show();

			// Record the window tag for this form
			dialog.Tag = windowId++;
			mDialogs.Add(dialog);

			mainForm.ProcessTimer.Start();

			dialog.Refresh();

			return (int)dialog.Tag;
		}

		bool DialogProcessCallback(int dialogId)
		{
			//Application.DoEvents();
			foreach (CallbackDialogForm dialog in mDialogs)
			{
				if ((int)dialog.Tag == dialogId)
					return dialog.cancel;
			}
			return false;
			//return mDialog.result;
		}

		int ModalDialogProcessCallback(int dialogId)
		{
			//Application.DoEvents();
			foreach (CallbackDialogForm dialog in mDialogs)
			{
				if ((int)dialog.Tag == dialogId)
				{
					//dialog.Activate();
					return dialog.result;
				}
			}
			return -1;
		}

		void DialogUpdateCallback(int dialogId, int percent, string description)
		{
			mainForm.ProcessTimer.Stop();
			//Application.DoEvents();
			CallbackDialogForm dialog = null;

			foreach (CallbackDialogForm temp in mDialogs)
			{
				if ((int)temp.Tag == dialogId)
					dialog = temp;
			}

			if (dialog == null) return;

			dialog.TopMost = false;

			if (description.Length != 0)
			{
				dialog.CallbackDialogFilenameLabel.Visible = true;

				string formatedDescription = "";
				string[] parts = description.Split("\n".ToCharArray());
				if (parts != null && parts.Length > 0)
				{
					foreach (string part in parts)
					{
						int strLength = (int)dialog.CallbackDialogFilenameLabel.CreateGraphics().MeasureString(part, dialog.CallbackDialogFilenameLabel.Font).Width;

						string line = part;
						if (strLength > dialog.CallbackDialogFilenameLabel.Width)
						{
							int increment = 5;
							while (strLength > dialog.CallbackDialogFilenameLabel.Width)
							{
								line = "..." + part.Substring(increment);
								strLength = (int)dialog.CallbackDialogFilenameLabel.CreateGraphics().MeasureString(line, dialog.CallbackDialogFilenameLabel.Font).Width;
								increment += 3;
							}
						}

						if (formatedDescription.Length == 0)
						{
							formatedDescription = line;
						}
						else
						{
							formatedDescription = formatedDescription + "\n" + line;
						}
					}

				}
				dialog.CallbackDialogFilenameLabel.Text = formatedDescription;

				dialog.CallbackDialogFilesProgressBar.Visible = true;
				dialog.CallbackDialogFilesProgressBar.Value = percent;
				dialog.Text = "OLD!!!   " + percent + "% Complete...";
			}
			mainForm.ProcessTimer.Start();
		}

		void DialogKillCallback(int dialogId)
		{
			mainForm.ProcessTimer.Stop();
			//Application.DoEvents();
			CallbackDialogForm dialog = null;

			foreach (CallbackDialogForm temp in mDialogs)
			{
				if ((int)temp.Tag == dialogId)
					dialog = temp;
			}

			if (dialog == null) return;

			mDialogs.Remove(dialog);

			dialog.Dispose();

			mainForm.ProcessTimer.Start();
		}
		#endregion

		bool CommandProcessCallBack(MOG_Command command)
		{
			return true;
		}

		//--------------------------------------------------------------------------------
		void CommandPreEventCallBack(MOG_Command command)
		{
			if (mainForm.IsHandleCreated)
			{
				mainForm.BeginInvoke(new MOG_CallbackCommandEvent(CommandPreEventCallBack_Invoked), new object[] { command });
			}
		}

		void CommandPreEventCallBack_Invoked(MOG_Command command)
		{
//			switch (command.GetCommandType())
//			{
//			case MOG_COMMAND_TYPE.MOG_COMMAND_LocalPackageMerge:
//				mainForm.mAssetManager.UpdatePackageButton();
//				break;
//			}
		}

		void CommandEventCallBack(MOG_Command command)
		{
			if (mainForm.IsHandleCreated)
			{
				mainForm.BeginInvoke(new MOG_CallbackCommandEvent(CommandEventCallBack_Invoked), new object[] { command });
			}
		}

		void CommandEventCallBack_Invoked(MOG_Command command)
		{
			switch (command.GetCommandType())
			{
			case MOG_COMMAND_TYPE.MOG_COMMAND_NotifySystemAlert:
			case MOG_COMMAND_TYPE.MOG_COMMAND_NotifySystemError:
			case MOG_COMMAND_TYPE.MOG_COMMAND_NotifySystemException:
				#region MOG_COMMAND_NotifySystemException
				MOG_ALERT_LEVEL level = MOG_ALERT_LEVEL.ALERT;
				switch (command.GetCommandType())
				{
				case MOG_COMMAND_TYPE.MOG_COMMAND_NotifySystemAlert:
					level = MOG_ALERT_LEVEL.ALERT;
					break;
				case MOG_COMMAND_TYPE.MOG_COMMAND_NotifySystemError:
					level = MOG_ALERT_LEVEL.ERROR;
					break;
				case MOG_COMMAND_TYPE.MOG_COMMAND_NotifySystemException:
					level = MOG_ALERT_LEVEL.CRITICAL;
					break;
				}
				MOG_Prompt.PromptMessage(command.GetTitle(), command.GetDescription(), command.GetSource(), level);
				#endregion
				break;
			case MOG_COMMAND_TYPE.MOG_COMMAND_LockPersistentNotify:
				#region MOG_COMMAND_LockPersistentNotify
				// Check if this is the same project?
				MOG_Command lockInfo = command.GetCommand();
				if (lockInfo != null)
				{
					// Check if we are on the locks tab?
					if (string.Compare(MOG_ControllerProject.GetActiveTabName(), "Locks", true) == 0)
					{
						if (mainForm.mLibraryManager != null)
						{
							mainForm.mLibraryManager.RefreshItem(command);
						}
					}

					// Check if this lock is related to our project?
					if (MOG_Filename.IsClassificationValidForProject(lockInfo.GetAssetFilename().GetOriginalFilename(), MOG_ControllerProject.GetProjectName()))
					{
						// Check if we are on the workspace tab?
						if (string.Compare(MOG_ControllerProject.GetActiveTabName(), "Workspace", true) == 0)
						{
							if (mainForm.mAssetManager != null)
							{
								mainForm.mAssetManager.RefreshLockStatus(command);
// JohnRen - Removed because this is just too slow when the server sends us a ton of locks at startup
//								mainForm.LocalBranchMogControl_GameDataDestinationTreeView.RefreshFileLockStatus(command);
								mainForm.mAssetManager.mLocal.RefreshLockStatus(command);
							}
						}
						// Check if we are on the project tab?
						else if (string.Compare(MOG_ControllerProject.GetActiveTabName(), "Project", true) == 0)
						{
							if (mainForm.mProjectManager != null)
							{
								mainForm.mProjectManager.UpdateAsset(command);
							}
						}
						// Check if we are on the library tab?
						else if (string.Compare(MOG_ControllerProject.GetActiveTabName(), "Library", true) == 0)
						{
							if (mainForm.mLibraryManager != null)
							{
								mainForm.mLibraryManager.RefreshItem(command);
							}
						}

						// Check if this notify just got processed?
						if (command.GetOptions().Contains("{Processed}"))
						{
							// Check if we have a local workspace defined?
							MOG_ControllerSyncData workspace = MOG_ControllerProject.GetCurrentSyncDataController();
							if (workspace != null)
							{
								// Time to check if this was our lock that just got released?
								if (string.Compare(lockInfo.GetUserName(), MOG_ControllerProject.GetUserName(), true) == 0 &&
									string.Compare(lockInfo.GetComputerName(), MOG_ControllerSystem.GetComputerName(), true) == 0)
								{
									// Is this lockInfo for an asset?
									if (lockInfo.GetAssetFilename().GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
									{
										// Check the user's privilege
										MOG_Privileges privileges = MOG_ControllerProject.GetPrivileges();
										if (!privileges.GetUserPrivilege(MOG_ControllerProject.GetUserName(), MOG_PRIVILEGE.IgnoreSyncAsReadOnly))
										{
											MOG_Filename bestAssetFilename = lockInfo.GetAssetFilename();

											// Check if there is a better inbox asset that can be substituted?
											MOG_Filename inboxAssetFilename = MOG_ControllerInbox.LocateBestMatchingAsset(bestAssetFilename);
											if (inboxAssetFilename != null)
											{
												bestAssetFilename = inboxAssetFilename;
											}

											// Check if this asset has it's files synced as read only?
											MOG_Properties properties = new MOG_Properties(bestAssetFilename);
											if (properties.SyncAsReadOnly)
											{
												switch (lockInfo.GetCommandType())
												{
													case MOG_COMMAND_TYPE.MOG_COMMAND_LockWriteRequest:
														workspace.SetLocalFileAttributes(lockInfo.GetAssetFilename(), FileAttributes.Normal);
														break;
													case MOG_COMMAND_TYPE.MOG_COMMAND_LockWriteRelease:
														workspace.SetLocalFileAttributes(lockInfo.GetAssetFilename(), FileAttributes.ReadOnly);
														break;
												}
											}
										}
									}
								}
							}
						}
					}
				}
				#endregion
				break;
			case MOG_COMMAND_TYPE.MOG_COMMAND_ViewUpdate:
				#region MOG_COMMAND_ViewUpdate

				// Make sure this is relevant to our active project?
				if (string.Compare(command.GetProject(), MOG_ControllerProject.GetProjectName(), true) != 0)
				{
					break;
				}

				if (string.Compare(MOG_ControllerProject.GetActiveTabName(), "Project", true) == 0)
				{
					// Process the auto process and package commands
					switch (MOG_AssetStatus.GetType(command.GetDescription()))
					{
					case MOG_AssetStatusType.Modified:
					case MOG_AssetStatusType.Unprocessed:
						// Check to see if this command comes from us
						if (string.Compare(command.GetComputerName(), MOG_ControllerSystem.GetComputerName(), true) != 0)
						{
							break;
						}

						// Check for Auto Process
						if (mainForm.AssetManagerAutoProcessCheckBox.Checked &&
							string.Compare(command.GetUserName(), MOG_ControllerProject.GetUser().GetUserName(), true) == 0)
						{
							guiAssetController.Process(command.GetDestination());
						}
						break;
					//KLK - This may be overkill because we are on the Project tab and why do we want to change their local package button
					case MOG_AssetStatusType.Packaged:
						break;
					case MOG_AssetStatusType.Unpackaged:
					case MOG_AssetStatusType.Repackage:
					case MOG_AssetStatusType.PackageError:
					case MOG_AssetStatusType.Unpackage:
						// We just got a new asset.  Make sure we inform the gui that a package is now required
						if (mainForm.mAssetManager != null)
						{
							mainForm.mAssetManager.RefreshWindowsBoth(command);
						}
						break;
					}
				}

				//if (string.Compare(MOG_ControllerProject.GetActiveTabName(), "Workspace", true) == 0)
				if (true)
				{
					if (string.Compare(command.GetDescription(), "UnGrouped", true) == 0)
					{
						command.SetDestination("");
					}

					try
					{
						// Filter out updates to other computers that share our username and project
						if ((string.Compare(command.GetComputerName(), MOG_ControllerSystem.GetComputerName(), true) != 0) &&
							(string.Compare(command.GetDescription(), MOG_AssetStatus.GetText(MOG_AssetStatusType.Copied), true) == 0 ||
							string.Compare(command.GetDescription(), MOG_AssetStatus.GetText(MOG_AssetStatusType.Unpackaged), true) == 0 ||
							string.Compare(command.GetDescription(), MOG_AssetStatus.GetText(MOG_AssetStatusType.Repackage), true) == 0 ||
							string.Compare(command.GetDescription(), MOG_AssetStatus.GetText(MOG_AssetStatusType.Packaged), true) == 0 ||
							string.Compare(command.GetDescription(), MOG_AssetStatus.GetText(MOG_AssetStatusType.PackageError), true) == 0 ||
							string.Compare(command.GetDescription(), MOG_AssetStatus.GetText(MOG_AssetStatusType.Unpackage), true) == 0))
						{
							return;
						}
						else
						{
							mainForm.mAssetManager.RefreshWindowsBoth(command);
						}
					}
					catch (Exception e)
					{
						Debug.Write(e.ToString());
					}

					// JohnRen - Changed...
					// The ViewUpdate command contains the computer name of who generated the view command...not the computer name of the originator of the rip
					// Because of this, AutoUpdate local would not work anytime somebody else's slave performed the rip.
					// Changed the AutoUpdate events to respect the login user instead of the computer name because this will be 100% correct...
					// However, multiple users will experience the event if they are logged in as the same user.
					//					// Process the auto process and package commands
					//					// only do this code if the command came from us
					//					if (string.Compare(command.GetComputerName(), MOG_ControllerSystem.GetComputerName(), true) == 0)
					MOG_Filename destinationFilename = new MOG_Filename(command.GetDestination());
					if (string.Compare(destinationFilename.GetUserName(), MOG_ControllerProject.GetUserName(), true) == 0)
					{
						switch (MOG_AssetStatus.GetType(command.GetDescription()))
						{
						case MOG_AssetStatusType.Modified:
						case MOG_AssetStatusType.Unprocessed:
							// Check for Auto Process
							if (mainForm.AssetManagerAutoProcessCheckBox.Checked)
							{
								// Only auto-process this if it is in the drafts folder
								if (destinationFilename.IsDrafts())
								{
									// Make sure we are the ones that requested it
									if (string.Compare(command.GetUserName(), MOG_ControllerProject.GetUser().GetUserName(), true) == 0 &&
										(string.Compare(command.GetComputerName(), MOG_ControllerSystem.GetComputerName(), true) == 0))
									{
										guiAssetController.Process(command.GetDestination());
									}
								}
							}
							break;
						case MOG_AssetStatusType.Imported:
							// Only auto-process this if it is in the drafts folder
							if (destinationFilename.IsDrafts())
							{
								// Make sure we are the ones that requested it
								if (string.Compare(command.GetUserName(), MOG_ControllerProject.GetUser().GetUserName(), true) == 0 &&
									(string.Compare(command.GetComputerName(), MOG_ControllerSystem.GetComputerName(), true) == 0))
								{
									guiAssetController.UpdateLocal(command.GetDestination(), false);
								}
							}
							break;
						case MOG_AssetStatusType.Processed:
						case MOG_AssetStatusType.Sent:
							// Check if the MainForm's AutoUpdate button is checked?
							if (mainForm.AssetManagerAutoUpdateLocalCheckBox.Checked)
							{
								// Only auto-process this if it is in the drafts folder
								if (destinationFilename.IsDrafts())
								{
									// Make sure we are the ones that requested it
									if (string.Compare(command.GetUserName(), MOG_ControllerProject.GetUser().GetUserName(), true) == 0 &&
										(string.Compare(command.GetComputerName(), MOG_ControllerSystem.GetComputerName(), true) == 0))
									{
										// Check if we have a current workspace?
										if (MOG_ControllerProject.GetCurrentSyncDataController() != null)
										{
											guiAssetController.UpdateLocal(command.GetDestination(), false);
										}
									}
								}
							}
							break;
						case MOG_AssetStatusType.Rebuilt:
							if (mainForm.mAssetManager != null)
							{
								mainForm.mAssetManager.RefreshActiveWindow();
							}
							break;
						case MOG_AssetStatusType.Deleted:
							MOG_Filename assetName = new MOG_Filename(command.GetSource());
							if (assetName.IsTrash() && mainForm.mAssetManager != null && mainForm.mAssetManager.mTrash != null && mainForm.AssetManagerInboxTabControl.SelectedTab.Name == "AssetManagerTrashTabPage")
							{
								mainForm.mAssetManager.mTrash.RefreshRemove(command);
							}
							else if (assetName.IsLocal() && mainForm.mAssetManager != null)
							{
								MOG_Filename tempAssetName = new MOG_Filename(command.GetSource());
							}
							break;
						}
					}
				}

				#endregion
				break;
			case MOG_COMMAND_TYPE.MOG_COMMAND_RegisterEditor:
				#region MOG_COMMAND_RegisterEditor
				// We just lost an Editor
				WorkspaceManager.RegisterEditor(command.GetWorkingDirectory());
				#endregion
				break;
			case MOG_COMMAND_TYPE.MOG_COMMAND_ShutdownEditor:
				#region MOG_COMMAND_ShutdownEditor
				// We just lost an Editor
				WorkspaceManager.ShutdownEditor(command.GetWorkingDirectory());
				#endregion
				break;
			case MOG_COMMAND_TYPE.MOG_COMMAND_ConnectionLost:
				#region MOG_COMMAND_ConnectionLost
				// The server was shutdown
				Bitmap DisconnectedIcon = new Bitmap(mainForm.StatusBarImageList.Images[1]);
				mainForm.MOGStatusBarConnectionStatusBarPanel.Icon = System.Drawing.Icon.FromHandle(DisconnectedIcon.GetHicon());
				mainForm.MOGStatusBarConnectionStatusBarPanel.Text = "Disconnected";
				mainForm.MOGStatusBarConnectionStatusBarPanel.ToolTipText = "Server is disconnected!";
				#endregion
				break;
			case MOG_COMMAND_TYPE.MOG_COMMAND_ConnectionNew:
				#region MOG_COMMAND_ConnectionNew
				Bitmap ConnectedIcon = new Bitmap(mainForm.StatusBarImageList.Images[0]);
				mainForm.MOGStatusBarConnectionStatusBarPanel.Icon = System.Drawing.Icon.FromHandle(ConnectedIcon.GetHicon());
				mainForm.MOGStatusBarConnectionStatusBarPanel.Text = "Connected";
				mainForm.MOGStatusBarConnectionStatusBarPanel.ToolTipText = mainForm.RefreshConnectionToolText();
				#endregion
				break;
            case MOG_COMMAND_TYPE.MOG_COMMAND_Complete:
                #region MOG_COMMAND_Complete
                // Make sure this contains an encapsulated command?
                if (command.GetCommand() != null)
                {
                    // Determin the type of encapsulated command
                    switch (command.GetCommand().GetCommandType())
                    {
                    case MOG_COMMAND_TYPE.MOG_COMMAND_Post:
                        #region MOG_COMMAND_PostComplete
						if (mainForm != null)
                        {
							// Check if we are on a tab that cares about this event?
							if (string.Compare(MOG_ControllerProject.GetActiveTabName(), "Connections", true) == 0)
							{
								if (mainForm.mConnectionManager != null)
								{
									// Only listen for the final Post w/o any assetFilename listed or else we would cause too many full refreshes
									if (command.GetCommand().GetAssetFilename().GetOriginalFilename().Length == 0)
									{
										// Refresh the pending package listview
										mainForm.mConnectionManager.RefreshMerging();
										// Refresh the pending post listview
										mainForm.mConnectionManager.RefreshPosting();
									}
								}
							}

							// Check if this remove was successful?
							if (command.GetCommand().IsCompleted())
							{
// Someday this would be nice, but the library tab does refresh when it gets reselected so we need to process this event even though we are on another tab
//								// Check if we are on a tab that cares about this event?
//								if (string.Compare(MOG_ControllerProject.GetActiveTabName(), "Library", true) == 0)
//								{
									if (command.GetCommand().GetAssetFilename().IsLibrary())
									{
										if (mainForm.mLibraryManager != null)
										{
											mainForm.mLibraryManager.RefreshItem(command);
										}
									}
//								}
								// Check if we are on a tab that cares about this event?
								if (string.Compare(MOG_ControllerProject.GetActiveTabName(), "Project", true) == 0)
								{
									if (mainForm.mProjectManager != null)
									{
										mainForm.mProjectManager.MakeAssetCurrent(command.GetCommand().GetAssetFilename());
									}
								}
							}
                        }
                        #endregion
                        break;
					case MOG_COMMAND_TYPE.MOG_COMMAND_ReinstanceAssetRevision:
						#region MOG_COMMAND_ReinstanceAssetRevision
						// Check if this remove was successful?
						if (command.GetCommand().IsCompleted())
						{
							if (mainForm.mProjectManager != null)
							{
								mainForm.mProjectManager.RemoveAssetFromProject(new MOG_Filename(command.GetCommand().GetSource()));
								mainForm.mProjectManager.MakeAssetCurrent(command.GetCommand().GetAssetFilename());
							}
							if (mainForm.LibraryExplorer != null)
							{
								mainForm.LibraryExplorer.LibraryListView.RefreshItem(command);
							}
						}
						#endregion
						break;
					case MOG_COMMAND_TYPE.MOG_COMMAND_RemoveAssetFromProject:
						#region MOG_COMMAND_RemoveAssetFromProject
						// Check if this remove was successful?
						if (command.GetCommand().IsCompleted())
						{
							if (mainForm.mProjectManager != null)
							{
								mainForm.mProjectManager.RemoveAssetFromProject(command.GetCommand().GetAssetFilename());
							}
							if (mainForm.LibraryExplorer != null)
							{
								mainForm.LibraryExplorer.LibraryListView.RefreshItem(command);
							}
						}
						#endregion
						break;
					case MOG_COMMAND_TYPE.MOG_COMMAND_NetworkPackageMerge:
					case MOG_COMMAND_TYPE.MOG_COMMAND_LocalPackageMerge:
					case MOG_COMMAND_TYPE.MOG_COMMAND_EditorPackageMergeTask:
					case MOG_COMMAND_TYPE.MOG_COMMAND_NetworkPackageRebuild:
						WorkspaceManager.EndPackaging(command.GetCommand().GetWorkingDirectory());
						break;
                    }
                }
                #endregion
                break;
			case MOG_COMMAND_TYPE.MOG_COMMAND_NotifyActiveConnection:
				#region MOG_COMMAND_NotifyActiveConnection
				if (string.Compare(MOG_ControllerProject.GetActiveTabName(), "Connections", true) == 0)
				{
					if (mainForm.mConnectionManager != null)
					{
						mainForm.mConnectionManager.UpdateConnections(command);
					}
				}
				#endregion
				break;
			// These are the all around locks requested commands from the server
			case MOG_COMMAND_TYPE.MOG_COMMAND_NotifyActiveLock:
				#region MOG_COMMAND_NotifyActiveLock
				switch (command.GetCommand().GetCommandType())
				{
				case MOG_COMMAND_TYPE.MOG_COMMAND_LockReadRelease:
				case MOG_COMMAND_TYPE.MOG_COMMAND_LockReadRequest:
				case MOG_COMMAND_TYPE.MOG_COMMAND_LockWriteRelease:
				case MOG_COMMAND_TYPE.MOG_COMMAND_LockWriteRequest:
					if (string.Compare(MOG_ControllerProject.GetActiveTabName(), "Locks", true) == 0)
					{
						if (mainForm.mLockManager != null)
						{
							mainForm.mLockManager.RefreshLockWindows(command);
						}
					}
					break;
				}
				#endregion
				break;
			case MOG_COMMAND_TYPE.MOG_COMMAND_RefreshProject:
				#region MOG_COMMAND_RefreshProject
				MainMenuProjectsClass.MOGGlobalRefreshProject(mainForm);
				#endregion
				break;
			case MOG_COMMAND_TYPE.MOG_COMMAND_RefreshApplication:
				break;
			case MOG_COMMAND_TYPE.MOG_COMMAND_RefreshTools:
				break;
			// These are the all around general commands from the server
			case MOG_COMMAND_TYPE.MOG_COMMAND_NotifyActiveCommand:
				#region MOG_COMMAND_NotifyActiveCommand
				// The command manager needs to know about all commands
				if (string.Compare(MOG_ControllerProject.GetActiveTabName(), "Connections", true) == 0)
				{
					if (mainForm.mConnectionManager != null)
					{
						mainForm.mConnectionManager.UpdateCommands(command);
					}
				}
				#endregion
				break;
			}
		}
	} // end class
} // end ns
