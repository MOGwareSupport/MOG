using System;
using System.Collections.Generic;
using System.Text;
using MOG.SYSTEM;
using MOG.PROMPT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;

namespace MOG_Server
{
	public class MOG_ControllerSystemCS
	{
		public static bool InitializeServer(string configFilename, MOG_CommandServerCS server)
		{
			MOG_ControllerSystem.InitializeSystem(configFilename, server);
			MOG_System system = MOG_ControllerSystem.GetSystem();
			if (system != null)
			{
				// Initialize the server mode?
				MOG_ControllerSystem.SetSystemMode(MOG_SystemMode.MOG_SystemMode_Server);
				
				// Recover saved locks from database
				if (!server.GetDatabaseLocks())
				{
					// Display the broadcasted message
					MOG_Prompt.PromptMessage("Database Connection Failed", "Unable to recover persistant locks from the database.", Environment.StackTrace);
				}

				return true;
			}

			return false;
		}			
	}
}
