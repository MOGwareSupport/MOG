using System;

using MOG;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.COMMAND;
using MOG_Client;



namespace MOG_Client.Client_Gui
{
	/// <summary>
	/// Summary description for SlaveManager.
	/// </summary>
	public class guiSlaveManager
	{
		private MogMainForm mainForm;

		public guiSlaveManager(MogMainForm main)
		{
			mainForm = main;
		}

		public void SlaveAdd()
		{
			MOG_CommandManager manager = MOG_ControllerSystem.GetCommandManager();
			if (manager != null)
			{
				MOG_Network network = manager.GetNetwork();
				if (network != null)
				{
					// Add a slave
					MOG_ControllerSystem.LaunchSlave(network.GetID());
				}
			}
		}

		public void SlaveDel()
		{
			// Remove a slave
			MOG_ControllerSystem.ShutdownSlave();
		}
	}
}
