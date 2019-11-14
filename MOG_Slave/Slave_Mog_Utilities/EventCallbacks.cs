using System;
using System.Drawing;

using MOG;
using MOG.COMMAND;
using MOG.REPORT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;

namespace MOG_Slave.Slave_Mog_Utilities
{
	/// <summary>
	/// Summary description for EventCallbacks.
	/// </summary>
	public class EventCallbacks
	{
		FormMainSlave mainForm;

		public EventCallbacks(FormMainSlave main)
		{
			mainForm = main;

			MOG_Callbacks callbacks = new MOG_Callbacks();

			callbacks.mPreEventCallback = new MOG_CallbackCommandEvent(this.CommandPreEventCallBack);
			callbacks.mEventCallback = new MOG_CallbackCommandEvent(this.CommandEventCallBack);
			MOG_ControllerSystem.GetCommandManager().SetCallbacks(callbacks);
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
			switch(command.GetCommandType())
			{
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
			switch(command.GetCommandType())
			{
			case MOG_COMMAND_TYPE.MOG_COMMAND_Complete:
				if (mainForm.mLogWindow != null)
				{
					mainForm.mLogWindow.UpdateLog();
				}
				break;
			case MOG_COMMAND_TYPE.MOG_COMMAND_SlaveTask:
				if (mainForm.mLogWindow != null)
				{
					mainForm.mLogWindow.UpdateLog();
				}
				break;
			case MOG_COMMAND_TYPE.MOG_COMMAND_ConnectionLost:
				#region MOG_COMMAND_ConnectionLost
				// The server was shutdown
				Bitmap DisconnectedIcon = new Bitmap(mainForm.SlaveIconsImageList.Images[1]);			
				mainForm.niconSystemTrayIcon.Icon = System.Drawing.Icon.FromHandle(DisconnectedIcon.GetHicon());
				mainForm.niconSystemTrayIcon.Text = "Server is disconnected!";
				#endregion
				break;
			case MOG_COMMAND_TYPE.MOG_COMMAND_ConnectionNew:
				#region MOG_COMMAND_ConnectionNew
				Bitmap ConnectedIcon = new Bitmap(mainForm.SlaveIconsImageList.Images[0]);			
				mainForm.niconSystemTrayIcon.Icon = System.Drawing.Icon.FromHandle(ConnectedIcon.GetHicon());
				mainForm.niconSystemTrayIcon.Text = mainForm.RefreshConnectionToolText();
				#endregion
				break;
			}
			return;
		}
	}
}
