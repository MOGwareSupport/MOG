using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Collections;
using MOG.INI;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.COMMAND;
using MOG.PROMPT;
using MOG.REPORT;

namespace MOG_Server
{
	public class MOG_ServerJobManager
	{
		private MOG_CommandServerCS mServerCommandManager;

		// Track the Jobs
		private HybridDictionary mActiveJobs;
		private ArrayList mJobOrder;

		// Slave Management
		private ArrayList mAvailableSlaves;
		private ArrayList mActiveSlaves;
		private int mManagedSlavesMax;
		private ArrayList mRequestedSlaveNames;
		private ArrayList mAutoLaunchedSlaveNames;
		private DateTime mNextNoSlaveReportTime;
		//private DateTime mLastSlaveRequstTime;  // KLK removed durring port because it was never referenced
		private DateTime mNextSlaveRequstTime;
		private DateTime mNextSlaveReleaseTime;

		internal MOG_CommandServerCS GetServerCommandManager()
		{ 
			return mServerCommandManager; 
		}

		internal MOG_ServerJobManager(MOG_CommandServerCS pServerCommandManager)
		{
			mServerCommandManager = pServerCommandManager;

			mAvailableSlaves = new ArrayList();
			mActiveSlaves = new ArrayList();
			mRequestedSlaveNames = new ArrayList();
			mAutoLaunchedSlaveNames = new ArrayList();

			mManagedSlavesMax = 0;

			mNextNoSlaveReportTime = DateTime.Now;
			mNextSlaveRequstTime = DateTime.Now;
			mNextSlaveReleaseTime = DateTime.Now;

			mActiveJobs = new HybridDictionary();
			mJobOrder = new ArrayList();
		}


		bool Initialize()
		{
			// Load the Network settings from the MOG_SYSTEM_CONFIGFILENAME
			MOG_Ini pConfigFile = MOG_ControllerSystem.GetSystem().GetConfigFile();
			// Check if this server has a managed slave setting?
			if (pConfigFile.KeyExist("Slaves", "ManagedSlavesMax"))
			{
				// Initialize the Server's ManagedSlavesMax
				mManagedSlavesMax = pConfigFile.GetValue("Slaves", "ManagedSlavesMax");
			}

			return true;
		}


		internal bool AddCommand(MOG_Command pCommand)
		{
			bool bAdded = false;

			MOG_JobInfo job = (MOG_JobInfo)mActiveJobs[pCommand.GetJobLabel()];
			if (job == null)
			{
				// Create a new job
				job = new MOG_JobInfo(this);
				mJobOrder.Add(job);
				mActiveJobs[pCommand.GetJobLabel()] = job;
			}

			// Make sure we have a valid job?
			if (job != null)
			{
				// Add this command to the identified job
				bAdded = job.AddCommand(pCommand);
			}

			return bAdded;
		}


		internal bool AddSlave(MOG_Command pSlave)
		{
			// Add the new slave to our list of available slaves
			mAvailableSlaves.Add(pSlave);
			return true;
		}


		internal bool RemoveSlave(MOG_Command pSlave)
		{
			// Remove this computer name from our requested slaves list
			RemoveRequestedSlaveName(pSlave.GetComputerName());

			// Remove this slave from our list of automatically launched slaves
			RemoveAutoLaunchedSlaveName(pSlave.GetComputerName());

			// Scan all the available slaves looking for this one
			for (int s = 0; s < mAvailableSlaves.Count; s++)
			{
				MOG_Command availableSlave = (MOG_Command)mAvailableSlaves[s];

				// Check if this is the slave we are looking for?
				if (pSlave.GetNetworkID() == availableSlave.GetNetworkID())
				{
					// Remove it from mAvailableSlaves
					mAvailableSlaves.RemoveAt(s);
					break;
				}
			}

			// Scan all the active slaves looking for this one
			for (int s = 0; s < mActiveSlaves.Count; s++)
			{
				MOG_Command activeSlave = (MOG_Command)mActiveSlaves[s];

				// Check if this is the slave we are looking for?
				if (pSlave.GetNetworkID() == activeSlave.GetNetworkID())
				{
					// Remove the slave from mActiveSlaves
					mActiveSlaves.RemoveAt(s);
					// Recover the command this slave was actively working on
					RecoverActiveSlaveCommand(pSlave);
					break;
				}
			}

			return true;
		}


		bool RecoverActiveSlaveCommand(MOG_Command pActiveSlave)
		{
			bool bRecovered = false;

			// Make sure we have a valid slave specified?
			if (pActiveSlave != null)
			{
				// Make sure this slave actually has an active command
				if (pActiveSlave.GetCommand() != null)
				{
					// Get the job of this slave's command
					MOG_JobInfo job = (MOG_JobInfo)mActiveJobs[pActiveSlave.GetCommand().GetJobLabel()];
					if (job != null)
					{
						bRecovered = job.RecoverCommand(pActiveSlave.GetCommand());
					}
				}
			}

			return bRecovered;
		}


		internal ArrayList GetAllPendingCommands()
		{
			ArrayList commands = new ArrayList();

			// Enumerate through all the jobs
			foreach (MOG_JobInfo job in mJobOrder)
			{
				if (job != null)
				{
					commands.AddRange(job.GetAllPendingCommands());
				}
			}

			return commands;
		}


		bool CheckExclusiveCommands(MOG_Command pCommand)
		{
			// Scan mActiveSlaves looking for one that matches this command
			for (int s = 0; s < mActiveSlaves.Count; s++)
			{
				// Make sure that this slave contains a valid command?
				MOG_Command pSlave = (MOG_Command)mActiveSlaves[s];
				if (pSlave.GetCommand() != null)
				{
					// Check if the CommandType matches?
					// Check if the AssetFilename matches?
					// Check if the Platforms matches?
					// Check if the Branch matches?
					// Check if the JobLabel matches?
					if (pSlave.GetCommand().GetCommandType() == pCommand.GetCommandType() &&
						String.Compare(pSlave.GetCommand().GetAssetFilename().GetOriginalFilename(), pCommand.GetAssetFilename().GetOriginalFilename(), true) == 0 &&
						String.Compare(pSlave.GetCommand().GetPlatform(), pCommand.GetPlatform(), true) == 0 &&
						String.Compare(pSlave.GetCommand().GetBranch(), pCommand.GetBranch(), true) == 0)
// JohnRen - Checking the JobLabel breaks exclusivity because 2 users could have initiated the same item and could collide
//						String.Compare(pSlave.GetCommand().GetJobLabel(), pCommand.GetJobLabel(), true) == 0)
					{
						return true;
					}
				}
			}

			// Check if this command is a network packaging command?
			if (pCommand.GetCommandType() == MOG_COMMAND_TYPE.MOG_COMMAND_NetworkPackageMerge)
			{
				// Check if there was an earlier network package merge similar to this command?
				// Enumerate through all the jobs
				foreach (MOG_JobInfo job in mJobOrder)
				{
					// Make sure this is a valid job?  and
					// Make sure this is another job unrelated to this command?
					if (job != null &&
						string.Compare(job.GetJobLabel(), pCommand.GetJobLabel(), true) != 0)
					{
						// Ask this job if it is packaging a similar command?
						if (job.IsJobPackaging(pCommand))
						{
							// We never want to allow a second network package merge to begin until the earlier one has finished posting
							return true;
						}
					}
				}
			}

			return false;
		}


		internal void ProcessJobs()
		{
			// Keep looping while we have idle slaves
			do
			{
				bool bAssignedSlave = false;

				// Enumerate through all the jobs
				foreach (MOG_JobInfo job in mJobOrder)
				{
					if (job != null)
					{
						// Request this job to process one of its commands
						if (job.Process())
						{
							// Indicate that a slave was assigned
							bAssignedSlave = true;
						}
					}
				}

				// Check if we never assigned a slave during the last pass?
				if (!bAssignedSlave)
				{
					// Stop trying to assign more slaves because nobody used one on the last pass
					break;
				}

				// Check if we have run out of available slaves?
				if (mAvailableSlaves.Count == 0)
				{
					// There is no need to continue when we have run out of slaves
					break;
				}
			} while (mAvailableSlaves.Count != 0);

			// Check for finished jobs
			RetireFinishedJobs();

			// Attempt to shutdown inactive slaves
			ReleaseInactiveSlaves();
		}


		void RetireFinishedJobs()
		{
			ArrayList finishedJobs = new ArrayList();

			// Enumerate through all the jobs
			foreach (MOG_JobInfo job in mJobOrder)
			{
				if (job != null)
				{
					// Check if this Job has finished?
					if (job.IsJobFinished())
					{
						// Keep track of the finished jobs
						finishedJobs.Add(job);
					}
				}
			}

			// Enumerate through all the jobs
			foreach (MOG_JobInfo finishedJob in finishedJobs)
			{
				// Inform everyone that this job has been finished
				if (finishedJob.FinishJob())
				{
					// Remove this job from our lists
					mJobOrder.Remove(finishedJob);
					mActiveJobs.Remove(finishedJob.GetJobLabel());
				}
			}
		}


		bool ReleaseInactiveSlaves()
		{
			// Make sure we didn't just barely request a new slave?
			if (DateTime.Now > mNextSlaveRequstTime)
			{
				// Check if we have any remaining auto-launched slaves that can be shut down?
				if (mAutoLaunchedSlaveNames.Count != 0)
				{
					// Check if enough time has passed since our last shutdown?
					if (DateTime.Now > mNextSlaveReleaseTime)
					{
						// Get the slave off of the bottom of our mAutoLaunchedSlaveNames
						string slaveName = (String)mAutoLaunchedSlaveNames[mAutoLaunchedSlaveNames.Count - 1];
						// Remove this slave from our list
						RemoveAutoLaunchedSlaveName(slaveName);

						// Check if there is already a slave running on this machine?
						if (LocateAvailableSlaveByComputerName(slaveName) != null)
						{
							// Inform this slave to terminate itself
							MOG_ControllerSystem.ShutdownSlave(slaveName);
							mNextSlaveReleaseTime = DateTime.Now.AddSeconds(5);
							return true;
						}
					}
				}
			}

			return false;
		}

		ArrayList GetRequestedSlaveNames()
		{
			return mRequestedSlaveNames;
		}

		bool RequestNewSlave(MOG_Command pCommand)
		{
			MOG_Command pClient = null;
			bool bAutoLaunchedSlave = false;
			bool bThisSpecificSlaveBeingRequested = false;

			// Check if we should wait a bit longer before we request a slave?
			if (DateTime.Now < mNextSlaveRequstTime)
			{
				// Ignore this request
				return false;
			}

			// Check for a 'SlaveMachine' section in the System's config file?
			MOG_Ini pConfigFile = MOG_ControllerSystem.GetSystem().GetConfigFile();
			string[] validSlaveMachines = null;
			if (pConfigFile.SectionExist("SlaveMachines"))
			{
				validSlaveMachines = pConfigFile.GetSectionKeys("SlaveMachines");
			}

			// Check if we obtained a list of validSlaveMachines?
			if (validSlaveMachines != null &&
				validSlaveMachines.Length > 0)
			{
				// Scan all the slaves specifically listed in our config file first
				// Walk list of specified SlaveMachines?
				foreach (string machineName in validSlaveMachines)
				{
					// Get the SlaveMachine info
					string machinePriority = pConfigFile.GetString("SlaveMachines", machineName);

					bThisSpecificSlaveBeingRequested = false;

					// Check if we have already requested this slave?
					if (IsAlreadyRequestedSlave(machineName))
					{
						// We can skip this machine because we have already issued a request for a slave
						continue;
					}

					// Check if we had a specific list of validSlaves specified?
					if (pCommand.GetValidSlaves().Length != 0)
					{
						// Check if this machineName isn't listed in the command's validSlaves?
						if (IsSlaveListed(machineName, pCommand.GetValidSlaves()))
						{
							bThisSpecificSlaveBeingRequested = true;
						}
						else
						{
							// We can skip this machine because it isn't listed as a validSlave
							continue;
						}
					}

					// Check if there is already a slave running on this machine?
					if (mServerCommandManager.LocateRegisteredSlaveByComputerName(machineName) != null)
					{
						continue;
					}

					// Make sure there is a client running on this machine?
					pClient = mServerCommandManager.LocateClientByComputerName(machineName);
					if (pClient != null)
					{
						// Check if this Slave should be considered an AutoLaunchedSlave?
						if (String.Compare(machinePriority, "Always", true) != 0 &&
							String.Compare(machinePriority, "Yes", true) != 0 &&
							String.Compare(machinePriority, "True", true) != 0)
						{
							// Indicate this slave should be terminated when it is no longer needed
							bAutoLaunchedSlave = true;
						}
						break;
					}
				}
			}

			// Check if we are still missing a client to launch a slave on?
			if (pClient == null)
			{
				// Scan all the registered clients looking for one that we can send this request to?
				ArrayList registeredClients = this.mServerCommandManager.GetRegisteredClients();
				for (int c = 0; c < registeredClients.Count; c++)
				{
					MOG_Command pRegisteredClient = (MOG_Command)registeredClients[c];
					string machineName = pRegisteredClient.GetComputerName();

					bThisSpecificSlaveBeingRequested = false;

					// Check if we have already requested this slave?
					if (IsAlreadyRequestedSlave(machineName))
					{
						// We can skip this machine because we have already issued a request for a slave
						continue;
					}

					// Check if there is already a slave running on this machine?
					if (mServerCommandManager.LocateRegisteredSlaveByComputerName(machineName) != null)
					{
						continue;
					}
					
					// Check if we had a specific list of validSlaves specified?
					if (pCommand.GetValidSlaves().Length != 0)
					{
						// Check if this machineName isn't listed in the specified validSlaves?
						if (IsSlaveListed(machineName, pCommand.GetValidSlaves()))
						{
							bThisSpecificSlaveBeingRequested = true;
						}
						else
						{
							// We can skip this machine because it isn't listed as a validSlave
							continue;
						}
					}
					else
					{
						// Check if there was a list of validSlaveMachines specified?
						if (validSlaveMachines != null)
						{
							// Check if this computer was listed?
							if (pConfigFile.KeyExist("SlaveMachines", machineName))
							{
								// Make sure this isn't listed as an exclusion?
								string machinePriority = pConfigFile.GetString("SlaveMachines", machineName);
								if (String.Compare(machinePriority, "Never", true) != 0 ||
									String.Compare(machinePriority, "No", true) != 0 ||
									String.Compare(machinePriority, "Exempt", true) != 0 ||
									String.Compare(machinePriority, "Ignore", true) != 0 ||
									String.Compare(machinePriority, "Skip", true) != 0)
								{
									continue;
								}
							}
						}
					}

					// Looks like we can use this client
					pClient = pRegisteredClient;
					// Indicate this slave should be terminated when it is no longer needed
					bAutoLaunchedSlave = true;
					break;
				}
			}

			// Check if this is just a generic slave request?
			if (!bThisSpecificSlaveBeingRequested)
			{
				// Check if we have a maximum number of slaves to auto launch? and
				// Check if we are over our maximum number of slaves?
				if (mManagedSlavesMax != 0 &&
					mManagedSlavesMax < mAutoLaunchedSlaveNames.Count)
				{
					// No need to launch this slave because we have exceeded our max
					return false;
				}
			}

			// Check if we found a client that we can request a new slave from?
			if (pClient != null)
			{
				// Instruct this client to launch a slave
				MOG_ControllerSystem.LaunchSlave(pClient.GetNetworkID());
				// Track when this last request was made
				mNextSlaveRequstTime = DateTime.Now.AddSeconds(5);

				// Add this slave to our list of requested slave names
				AddRequestedSlave(pClient.GetComputerName());
				// Check if we should add this slave as an AutoLaunchedSlave?
				if (bAutoLaunchedSlave)
				{
					AddAutoLaunchedSlaveName(pClient.GetComputerName());
				}

				// Indicate we launched a new slave
				return true;
			}

			// Check if it is time to report a missing slave error?
			if (DateTime.Now > mNextNoSlaveReportTime)
			{
				// Check if there was a specific validslaves listed?
				if (pCommand.GetValidSlaves().Length >0)
				{
					bool bWarnUser = true;

					// Split up the specified ValidSlaves
					String delimStr = ",;";
					Char[] delimiter = delimStr.ToCharArray();
					String[] SlaveNames = pCommand.GetValidSlaves().Trim().Split(delimiter);
					// Check the registered slaves to see if a valid slave is running
					for (int n = 0; n < SlaveNames.Length; n++)
					{
						// Check if we found our matching slave name?
						if (mServerCommandManager.LocateRegisteredSlaveByComputerName(SlaveNames[n]) != null)
						{
							// We found a match...this command will eventually get processed
							bWarnUser = false;
							break;
						}
					}

					// Check if we decided to warn the user?
					if (bWarnUser)
					{
						// Setup the Broadcast message indicating that there are no valid slaves running
						string message = String.Concat("MOG Server is trying to process a command containing a 'ValidSlaves' property and has been unable to launch the needed Slave.\n",
														 "VALIDSLAVES=", pCommand.GetValidSlaves(), "\n\n",
														 "Please check this machine to ensure it is connected to the MOG Server.");
						MOG_ControllerSystem.NetworkBroadcast(pCommand.GetUserName(), message);
						// Record the time when this report was sent
						mNextNoSlaveReportTime = DateTime.Now.AddMinutes(5);
					}
				}
			}

			// Indicate that we failed to find a qualified slave
			return false;
		}

		internal ArrayList GetActiveSlaves()
		{
			return mActiveSlaves;
		}

		internal ArrayList GetActiveSlaves(string jobLabel)
		{
			ArrayList activeSlaves = new ArrayList();

			// Scan mActiveSlaves looking for one that matches this command
			for (int s = 0; s < mActiveSlaves.Count; s++)
			{
				MOG_Command pSlave = (MOG_Command)mActiveSlaves[s];

				// Get the command this slave is working on
				MOG_Command pCommand = pSlave.GetCommand();
				if (pCommand != null)
				{
					// Check if this slave is working on this jobLabel?
					if (String.Compare(pCommand.GetJobLabel(), jobLabel, true) == 0)
					{
						// Include this slave
						activeSlaves.Add(pSlave);
					}
				}
			}

			return activeSlaves;
		}

		ArrayList GetAvailableSlaves()
		{
			return mAvailableSlaves;
		}

		bool IsAlreadyRequestedSlave(string computerName)
		{
			// Scan all the requested slave names looking for this one
			for (int i = 0; i < mRequestedSlaveNames.Count; i++)
			{
				string slaveName = (String)mRequestedSlaveNames[i];
				// Does this slave name match?
				if (String.Compare(slaveName, computerName, true) == 0)
				{
					// Indicate we found the slave name
					return true;
				}
			}

			return false;
		}


		bool RemoveRequestedSlaveName(string computerName)
		{
			// Scan all the requested slave names looking for this one
			for (int i = 0; i < mRequestedSlaveNames.Count; i++)
			{
				string slaveName = (String)mRequestedSlaveNames[i];
				// Does this slave name match?
				if (String.Compare(slaveName, computerName, true) == 0)
				{
					mRequestedSlaveNames.RemoveAt(i);
					return true;
				}
			}

			return false;
		}


		bool AddAutoLaunchedSlaveName(string computerName)
		{
			// Scan all the requested slave names looking for this one
			for (int i = 0; i < mAutoLaunchedSlaveNames.Count; i++)
			{
				string slaveName = (String)mAutoLaunchedSlaveNames[i];
				// Does this slave name match?
				if (String.Compare(slaveName, computerName, true) == 0)
				{
					// Indicate the name is has already been added
					return true;
				}
			}

			// Add this new slave name
			mAutoLaunchedSlaveNames.Add(computerName);
			return false;
		}


		bool RemoveAutoLaunchedSlaveName(string computerName)
		{
			// Scan all the requested slave names looking for this one
			for (int i = 0; i < mAutoLaunchedSlaveNames.Count; i++)
			{
				string slaveName = (String)mAutoLaunchedSlaveNames[i];
				// Does this slave name match?
				if (String.Compare(slaveName, computerName, true) == 0)
				{
					mAutoLaunchedSlaveNames.RemoveAt(i);
					return true;
				}
			}

			return false;
		}


		bool IsSlaveListed(string slaveComputerName, string validSlaves)
		{
			String delimStr = ",;";
			Char[] delimiter = delimStr.ToCharArray();
			String[] SlaveNames = validSlaves.Trim().Split(delimiter);

			// Look for a valid slave by scanning mAvailableSlaves
			for (int n = 0; n < SlaveNames.Length; n++)
			{
				// Check if we found our matching slave name?
				if (String.Compare(slaveComputerName, SlaveNames[n], true) == 0)
				{
					return true;
				}
			}

			return false;
		}


		internal bool AssignToSlave(MOG_Command pCommand)
		{
			MOG_Command pSlave = null;

			// Always make sure that the AssignedSlaveID is cleared just in case we fail
			pCommand.SetAssignedSlaveID(0);

			// Check if this is an exclusive command?
			if (pCommand.IsExclusiveCommand())
			{
				// Check if this exclusive command is already actively getting processed?
				if (CheckExclusiveCommands(pCommand))
				{
					// Don't assign this exclusive command because there is already another one being processed
					return false;
				}
			}

			// Identify a valid slave based on the mValidSlaves?
			if (pCommand.GetValidSlaves().Length > 0)
			{
				// Look for a valid slave by scanning mAvailableSlaves
				for (int s = 0; s < mAvailableSlaves.Count; s++)
				{
					// Is this slave's name specified in mValidSlaves?
					pSlave = (MOG_Command)mAvailableSlaves[s];
					if (pSlave != null)
					{
						if (IsSlaveListed(pSlave.GetComputerName(), pCommand.GetValidSlaves()))
						{
							break;
						}
					}

					// Indicate that this slave wasn't listed in mValidSlaves
					pSlave = null;
				}

				// No available valid slave found?
				if (pSlave == null)
				{
					// Request a new slave respecting this command's valid slaves
					RequestNewSlave(pCommand);
					// Bail out since we need to wait for a slave to become available
					return false;
				}
			}
			else
			{
				// Check if we have run out of available slaves?
				if (mAvailableSlaves.Count == 0)
				{
					// Request a new slave?
					RequestNewSlave(pCommand);

					// Bail out here because there were no slaves and it can take some time before any requested slaves will be launched
					return false;
				}

				// Since mValidSlaves was blank, get the first slave in the list
				pSlave = (MOG_Command)mAvailableSlaves[0];
			}

			// Did we actually locate a valid available slave?
			if (pSlave != null)
			{
				// Send the command to the available slave
				if (mServerCommandManager.SendToConnection(pSlave.GetNetworkID(), pCommand))
				{
					// Assign the Slave's NetworkID within the command so we will know who is working on this command
					pCommand.SetAssignedSlaveID(pSlave.GetNetworkID());
					// Stick this command into the RegisteredSlave's command
					pSlave.SetCommand(pCommand);
					// Add the slave to mActiveSlaves
					mActiveSlaves.Add(pSlave);
					// Remove the top slave from mAvailableSlaves
					mAvailableSlaves.Remove(pSlave);

					// Send out needed Notify command
					MOG_Command pNotify = MOG_CommandFactory.Setup_NotifyActiveConnection(pSlave);
					mServerCommandManager.SendToActiveTab("Connections", pNotify);

					pNotify = MOG_CommandFactory.Setup_NotifyActiveCommand(pCommand);
					mServerCommandManager.SendToActiveTab("Connections", pNotify);

					return true;
				}
			}

			return false;
		}


		MOG_Command LocateActiveSlaveByID(int networkID)
		{
			// Scan mActiveSlaves looking for one that matches this command
			for (int s = 0; s < mActiveSlaves.Count; s++)
			{
				MOG_Command pSlave = (MOG_Command)mActiveSlaves[s];

				// Check if this is the slave we are looking for?
				if (pSlave.GetNetworkID() == networkID)
				{
					return pSlave;
				}
			}

			return null;
		}


		MOG_Command LocateActiveSlaveByAssignedCommandNetworkID(int activeCommandNetworkID)
		{
			// Scan mActiveSlaves looking for one that matches this command
			for (int s = 0; s < mActiveSlaves.Count; s++)
			{
				MOG_Command pSlave = (MOG_Command)mActiveSlaves[s];

				// Make sure that this slave contains a valid command
				if (pSlave.GetCommand() != null)
				{
					// Check if this slave's contained command's NetworkID matches this NetworkID?
					if (pSlave.GetCommand().GetNetworkID() == activeCommandNetworkID)
					{
						return pSlave;
					}
				}
			}

			return null;
		}


		internal bool ReleaseSlave(MOG_Command pCommand)
		{
			// Scan mActiveSlaves looking for one that matches this command
			// This way is safer just in case NetworkIDs get reset from a reconnect event
			for (int s = 0; s < mActiveSlaves.Count; s++)
			{
				MOG_Command pSlave = (MOG_Command)mActiveSlaves[s];

				// Check if this slave is working on something?
				if (pSlave.GetCommand() != null)
				{
					// Check if the CommandID matches?
					if (pSlave.GetCommand().GetCommandID() == pCommand.GetCommandID())
					{
						// Clear this slave's command
						pSlave.SetCommand(null);
						// Refresh the command's SetAssignedSlaveID
						pCommand.SetAssignedSlaveID(pSlave.GetNetworkID());

						// Send out needed Notify command
						MOG_Command pNotify = MOG_CommandFactory.Setup_NotifyActiveConnection(pSlave);
						mServerCommandManager.SendToActiveTab("Connections", pNotify);

						pNotify = MOG_CommandFactory.Setup_NotifyActiveCommand(pCommand);
						mServerCommandManager.SendToActiveTab("Connections", pNotify);

						// Insert the slave at the top of mAvailableSlaves so it will be immeadiately reused
						mAvailableSlaves.Insert(0, pSlave);
						// Erase the slave from mActiveSlaves
						mActiveSlaves.RemoveAt(s);

						// Return the command that the slave was processing for reference
						return true;
					}
				}
			}

			string message = String.Concat("Server could not release slave because it failed to locate the slave in mActiveSlaves\n",
												"Command:", pCommand.ToString(), "     NetworkID:", pCommand.GetNetworkID(), "     Asset:", pCommand.GetAssetFilename().GetOriginalFilename());
			MOG_Report.ReportMessage("Server", message, Environment.StackTrace, MOG_ALERT_LEVEL.ERROR);
			return false;
		}


		MOG_Command LocateAvailableSlaveByComputerName(string computerName)
		{
			// Scan all the available slaves looking for this one
			for (int s = 0; s < mAvailableSlaves.Count; s++)
			{
				MOG_Command slave = (MOG_Command)mAvailableSlaves[s];

				// Is this slave running on this commands computer?
				if (String.Compare(slave.GetComputerName(), computerName, true) == 0)
				{
					// return this available slave
					return slave;
				}
			}

			return null;
		}


		bool AddRequestedSlave(string computerName)
		{
			// Scan all the requested slave names looking for this one
			for (int i = 0; i < mRequestedSlaveNames.Count; i++)
			{
				string slaveName = (String)mRequestedSlaveNames[i];
				// Does this slave name match?
				if (String.Compare(slaveName, computerName, true) == 0)
				{
					// Indicate this name is already listed
					return true;
				}
			}

			// Add this new slave name
			mRequestedSlaveNames.Add(computerName);
			return true;
		}


		internal bool Command_RequestActiveCommands(MOG_Command pCommand)
		{
			bool bFailed = false;

			// Loop through mActiveSlaves
			for (int c = 0; c < mActiveSlaves.Count; c++)
			{
				// Send off the Notify command for each one
				MOG_Command pNotify = MOG_CommandFactory.Setup_NotifyActiveCommand((MOG_Command)mActiveSlaves[c]);
				if (!mServerCommandManager.SendToConnection(pCommand.GetNetworkID(), pNotify))
				{
					bFailed = true;
				}
			}

			// Enumerate through all the jobs
			foreach (MOG_JobInfo job in mJobOrder)
			{
				if (job != null)
				{
					if (!job.Command_RequestActiveCommands(pCommand))
					{
						bFailed = true;
					}
				}
			}

			// Check if we failed?
			if (!bFailed)
			{
				return true;
			}
			return false;
		}


		internal bool Command_KillCommand(MOG_Command pCommand)
		{
			// Enumerate through all the jobs
			foreach (MOG_JobInfo job in mJobOrder)
			{
				if (job != null)
				{
					// Attempt to remove the command from this job
					job.KillCommand(pCommand.GetCommandID());
				}
			}

			// Check the active slaves to see if it is being processed?
			// Just in case the command we want to kill has already been tasked out to a slave
			for (int c = 0; c < mActiveSlaves.Count; c++)
			{
				// Check this slave's assigned command?
				MOG_Command activeSlave = (MOG_Command)mActiveSlaves[c];
				MOG_Command pActiveCommand = activeSlave.GetCommand();
				if (pActiveCommand != null)
				{
					// Checkif the CommandID matches?
					if (pActiveCommand.GetCommandID() == pCommand.GetCommandID())
					{
						// Clear the command this slave is working on
						activeSlave.SetCommand(null);

						// Always kill the connection to this slave just in case it was hung by the command
						MOG_ControllerSystem.ShutdownSlave(activeSlave.GetNetworkID());
						break;
					}
				}
			}

			// Always eat this command
			return true;
		}


		internal bool Command_RetaskCommand(MOG_Command pCommand)
		{
			// Shutdown this slave and its command will be automatically recycled
			return MOG_ControllerSystem.ShutdownSlave(pCommand.GetNetworkID());
		}


		internal bool Command_RegisterClient(MOG_Command pCommand)
		{
			// Remove this computer name from our requested slaves list
			RemoveRequestedSlaveName(pCommand.GetComputerName());

			return true;
		}


		internal bool Command_ShutdownClient(MOG_Command pCommand)
		{
			// Remove this computer name from our requested slaves list
			RemoveRequestedSlaveName(pCommand.GetComputerName());

			return true;
		}


		internal bool Command_RegisterSlave(MOG_Command pCommand)
		{
			// Remove this computer name from our requested slaves list
			RemoveRequestedSlaveName(pCommand.GetComputerName());

			return true;
		}
	}
}
