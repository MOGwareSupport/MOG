using System;
using System.Collections;
using System.Windows.Forms;

using MOG;
using MOG.COMMAND;
using MOG.INI;
using MOG.CONTROLLER.CONTROLLERSYSTEM;

using Server_Gui;
using MOG_Server.Server_Gui;
using MOG_Server.MOG_ControlsLibrary.Common;


namespace MOG_Server.Server_Mog_Utilities
{
	#region Event-handling structures and delegates
	// new command
	public delegate void MOG_NewCommandHandler(object sender, MOG_NewCommandEventArgs e);
	public class MOG_NewCommandEventArgs : EventArgs
	{
		public MOG_Command command;
	}

	// new connection
	public delegate void MOG_NewConnectionHandler(object sender, MOG_NewConnectionEventArgs e);
	public class MOG_NewConnectionEventArgs : EventArgs
	{
		public MOG_Command connectionCommand;
	}

	// new lock
	public delegate void MOG_NewLockHandler(object sender, MOG_NewLockEventArgs e);
	public class MOG_NewLockEventArgs : EventArgs
	{
		public MOG_Command lockCommand;
	}

	// critical exception
	public delegate void MOG_NewSystemExceptionHandler(object sender, MOG_NewSystemExceptionEventArgs e);
	public class MOG_NewSystemExceptionEventArgs : EventArgs
	{
		public MOG_Command exceptionCommand;
	}
	#endregion

	/// <summary>
	/// Summary description for EventCallbacks.
	/// </summary>
	public class EventCallbacks
	{
		FormMainSMOG mainForm;
		public ArrayList mDialogs = new ArrayList();
		private int windowId;

		#region Events-n-such
		// new command
		public event MOG_NewCommandHandler NewCommand;
		private void OnNewCommand(MOG_Command command)
		{
			if (this.NewCommand != null)
			{
				MOG_NewCommandEventArgs e = new MOG_NewCommandEventArgs();
				e.command = command;
				this.NewCommand(this, e);
			}
		}

		// new connection
		public event MOG_NewConnectionHandler NewConnection;
		private void OnNewConnection(MOG_Command connectionCommand)
		{
			if (this.NewConnection != null)
			{
				MOG_NewConnectionEventArgs e = new MOG_NewConnectionEventArgs();
				e.connectionCommand = connectionCommand;
				this.NewConnection(this, e);
			}
		}

		// new lock
		public event MOG_NewLockHandler NewLock;
		private void OnNewLock(MOG_Command lockCommand)
		{
			if (this.NewLock != null)
			{
				MOG_NewLockEventArgs e = new MOG_NewLockEventArgs();
				e.lockCommand = lockCommand;
				this.NewLock(this, e);
			}
		}

		// new critical exception
		public event MOG_NewSystemExceptionHandler NewSystemException;
		private void OnNewSystemException(MOG_Command exceptionCommand)
		{
			if (this.NewSystemException != null)
			{
				MOG_NewSystemExceptionEventArgs e = new MOG_NewSystemExceptionEventArgs();
				e.exceptionCommand = exceptionCommand;
				this.NewSystemException(this, e);
			}
		}
		#endregion

		public EventCallbacks(FormMainSMOG main)
		{
			mainForm = main;

			if (MOG_ControllerSystem.IsCommandManager())
			{
				MOG_Callbacks callbacks = new MOG_Callbacks();

				callbacks.mPreEventCallback = new MOG_CallbackCommandEvent(this.CommandPreEventCallBack);
				callbacks.mEventCallback = new MOG_CallbackCommandEvent(this.CommandEventCallBack);

				MOG_ControllerSystem.GetCommandManager().SetCallbacks(callbacks);
			}
		}

		#region Progress bar window callback functions
		int DialogInitCallback(string title, string message, string button)
		{
			CallbackDialogForm dialog = new CallbackDialogForm();

			dialog.Text = title;

			dialog.CallbackDialogFilenameLabel.Visible = false;
			dialog.CallbackDialogFilesProgressBar.Visible = false;

			dialog.CallbackDialoglMessageLabel.Text = message;

			// Check if the button string needs to be parsed
			if (button.IndexOf("/") != -1)
			{
				string []buttons = button.Split(new Char[] {'/'});
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
			else if (button == "")
			{
				dialog.CallbackDialogCancelButton.Visible = false;
			}
			else
			{
				dialog.CallbackDialogCancelButton.Visible = true;
				dialog.CallbackDialogCancelButton.Text = button;
			}

			dialog.Show();
            
			// Record the window tag for this form
			dialog.Tag = windowId++;
			mDialogs.Add(dialog);

			Application.DoEvents();

			return (int)dialog.Tag;
		}

		void DialogUpdateCallback(int dialogId, int percent, string description)
		{
			Application.DoEvents();
			CallbackDialogForm dialog = null;

			foreach (CallbackDialogForm temp in mDialogs)
			{
				if ((int)temp.Tag == dialogId)
					dialog = temp;
			}

			if (dialog == null) return;

			if (description.Length != 0)
			{
				dialog.CallbackDialogFilenameLabel.Visible = true;
				dialog.CallbackDialogFilenameLabel.Text = description;

				dialog.CallbackDialogFilesProgressBar.Visible = true;

				// Check for the special '-1' specifier?
				if (percent == -1)
				{
					// Just increment the bar by the increment value
					dialog.CallbackDialogFilesProgressBar.Increment(1);
					// Check if we just exceeded our maximum?
					if (dialog.CallbackDialogFilesProgressBar.Value == dialog.CallbackDialogFilesProgressBar.Maximum)
					{
						dialog.CallbackDialogFilesProgressBar.Value = dialog.CallbackDialogFilesProgressBar.Minimum;
					}
				}
				else
				{
					dialog.CallbackDialogFilesProgressBar.Value = percent;
				}
			}
		}

		bool DialogProcessCallback(int dialogId)
		{
			Application.DoEvents();
			foreach (CallbackDialogForm dialog in mDialogs)
			{
				if ((int)dialog.Tag == dialogId)
					return dialog.cancel;
			}
			return false;
			//return mDialog.result;
		}


		void DialogKillCallback(int dialogId)
		{
			Application.DoEvents();
			CallbackDialogForm dialog = null;

			foreach (CallbackDialogForm temp in mDialogs)
			{
				if ((int)temp.Tag == dialogId)
					dialog = temp;
			}

			if (dialog == null) return;

			mDialogs.Remove(dialog);

			dialog.Dispose();
		}
		#endregion

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
			switch(command.GetCommandType())
			{
				case MOG_COMMAND_TYPE.MOG_COMMAND_AssetRipRequest:
				case MOG_COMMAND_TYPE.MOG_COMMAND_AssetProcessed:
				case MOG_COMMAND_TYPE.MOG_COMMAND_SlaveTask:
				case MOG_COMMAND_TYPE.MOG_COMMAND_ReinstanceAssetRevision:
				case MOG_COMMAND_TYPE.MOG_COMMAND_Bless:
				case MOG_COMMAND_TYPE.MOG_COMMAND_Post:
				case MOG_COMMAND_TYPE.MOG_COMMAND_RemoveAssetFromProject:
				case MOG_COMMAND_TYPE.MOG_COMMAND_NetworkPackageMerge:
				case MOG_COMMAND_TYPE.MOG_COMMAND_LocalPackageMerge:
				case MOG_COMMAND_TYPE.MOG_COMMAND_NetworkPackageRebuild:
				case MOG_COMMAND_TYPE.MOG_COMMAND_LocalPackageRebuild:
				case MOG_COMMAND_TYPE.MOG_COMMAND_Archive:
				case MOG_COMMAND_TYPE.MOG_COMMAND_ScheduleArchive:
					if (mainForm.SMOG_Main_TabControl.SelectedIndex == (int)FormMainSMOG.MAIN_PAGES.SMOG_COMMANDS)
					{
						mainForm.mPendingCommands.RefreshWindow(command);
					}
					break;
				default:
					break;
			}
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
			if (mainForm.WindowState == FormWindowState.Minimized)
			{
				return;
			}

			switch (command.GetCommandType())
			{
				case MOG_COMMAND_TYPE.MOG_COMMAND_NotifySystemAlert:
				case MOG_COMMAND_TYPE.MOG_COMMAND_NotifySystemError:
				case MOG_COMMAND_TYPE.MOG_COMMAND_NotifySystemException:
					// notify anyone who cares
					OnNewSystemException(command);
					break;

				case MOG_COMMAND_TYPE.MOG_COMMAND_LockReadRelease:
				case MOG_COMMAND_TYPE.MOG_COMMAND_LockWriteRelease:
				case MOG_COMMAND_TYPE.MOG_COMMAND_LockWriteRequest:
				case MOG_COMMAND_TYPE.MOG_COMMAND_LockReadRequest:
					if (mainForm.mLocks != null)
					{
						mainForm.mLocks.RefreshLockWindows(command);
					}

					// notify anyone who cares
					OnNewLock(command);
					break;
				case MOG_COMMAND_TYPE.MOG_COMMAND_ConnectionKill:
				case MOG_COMMAND_TYPE.MOG_COMMAND_ShutdownSlave:
				case MOG_COMMAND_TYPE.MOG_COMMAND_ShutdownClient:
				case MOG_COMMAND_TYPE.MOG_COMMAND_ShutdownEditor:
				case MOG_COMMAND_TYPE.MOG_COMMAND_ShutdownCommandLine:
				case MOG_COMMAND_TYPE.MOG_COMMAND_RegisterSlave:
				case MOG_COMMAND_TYPE.MOG_COMMAND_RegisterClient:
				case MOG_COMMAND_TYPE.MOG_COMMAND_RegisterEditor:
				case MOG_COMMAND_TYPE.MOG_COMMAND_RegisterCommandLine:
				case MOG_COMMAND_TYPE.MOG_COMMAND_AssetRipRequest:
				case MOG_COMMAND_TYPE.MOG_COMMAND_AssetProcessed:
				case MOG_COMMAND_TYPE.MOG_COMMAND_SlaveTask:
				case MOG_COMMAND_TYPE.MOG_COMMAND_ReinstanceAssetRevision:
				case MOG_COMMAND_TYPE.MOG_COMMAND_Bless:
				case MOG_COMMAND_TYPE.MOG_COMMAND_Post:
				case MOG_COMMAND_TYPE.MOG_COMMAND_RemoveAssetFromProject:
				case MOG_COMMAND_TYPE.MOG_COMMAND_NetworkPackageMerge:
				case MOG_COMMAND_TYPE.MOG_COMMAND_LocalPackageMerge:
				case MOG_COMMAND_TYPE.MOG_COMMAND_NetworkPackageRebuild:
				case MOG_COMMAND_TYPE.MOG_COMMAND_LocalPackageRebuild:
				case MOG_COMMAND_TYPE.MOG_COMMAND_Archive:
				case MOG_COMMAND_TYPE.MOG_COMMAND_ScheduleArchive:
				case MOG_COMMAND_TYPE.MOG_COMMAND_ActiveViews:
					OnNewConnection(command);

					if (mainForm.SMOG_Main_TabControl.SelectedIndex == (int)FormMainSMOG.MAIN_PAGES.SMOG_CONNECTIONS)
					{
						mainForm.mMonitor.DisplayUpdateExistingConnections(command);
					}
					if (mainForm.SMOG_Main_TabControl.SelectedIndex == (int)FormMainSMOG.MAIN_PAGES.SMOG_COMMANDS)
					{
						mainForm.mPendingCommands.RefreshWindow(command);
					}
					break;
			}
			return;
		}
	}
}
