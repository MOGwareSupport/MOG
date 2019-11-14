using System;
using System.Collections.Generic;
using System.Text;
using MOG.COMMAND;
using System.Collections.Specialized;
using System.Collections;
using MOG.CONTROLLER.CONTROLLERSYSTEM;

namespace MOG_Server
{
	public class MOG_JobInfo
	{
		private bool mSkipJob;

		private MOG_ServerJobManager mServerJobManager;

		// Job Command Tracking
		private MOG_Command mStartJobCommand;
		private HybridDictionary mTaskCommandList;
		private HybridDictionary mPrimaryCommandList;
		private HybridDictionary mPackageCommandList;
		private HybridDictionary mPostCommandList;
		private HybridDictionary mDeferredCommandList;

		// Job state tracking
		private ArrayList mProcessedPackageCommands = new ArrayList();

		public MOG_JobInfo(MOG_ServerJobManager pServerJobManager)
		{
			// Initialize the JobInfo
			mServerJobManager = pServerJobManager;

			mSkipJob = false;

			mStartJobCommand = null;
			mTaskCommandList = new HybridDictionary();
			mPrimaryCommandList = new HybridDictionary();
			mPackageCommandList = new HybridDictionary();
			mPostCommandList = new HybridDictionary();
			mDeferredCommandList = new HybridDictionary();
		}

		internal string GetJobLabel()				
		{
			return (IsJobStarted()) ? mStartJobCommand.GetJobLabel() : ""; 
		}

		internal bool IsJobStarted()
		{
			return (mStartJobCommand != null);
		}

		internal bool IsJobFinished()
		{
			bool bFinished = false;

			// Make sure this Job has been started?
			if (IsJobStarted())
			{
				// Check if this job has finished with its commands?
				if (mTaskCommandList.Count == 0 &&
					mPrimaryCommandList.Count == 0 &&
					mPackageCommandList.Count == 0 &&
					mPostCommandList.Count == 0 &&
					mDeferredCommandList.Count == 0)
				{
					// Make sure no slave is still doing something for this job?
					if (mServerJobManager.GetActiveSlaves(GetJobLabel()).Count == 0)
					{
						bFinished = true;
					}
				}
			}

			return bFinished;
		}

		internal bool IsJobPackaging(MOG_Command pCommand)
		{
			// Check if this job has already begun any packaging commands yet?
			if (mProcessedPackageCommands.Count > 0)
			{
				// Check if this package command is related to any already processed packaging commands within this job?
				foreach (MOG_Command command in mProcessedPackageCommands)
				{
					// Check if the CommandType matches?
					// Check if the AssetFilename matches?
					// Check if the Platforms matches?
					// Check if the Branch matches?
					if (command.GetCommandType() == pCommand.GetCommandType() &&
					String.Compare(command.GetAssetFilename().GetOriginalFilename(), pCommand.GetAssetFilename().GetOriginalFilename(), true) == 0 &&
					String.Compare(command.GetPlatform(), pCommand.GetPlatform(), true) == 0 &&
					String.Compare(command.GetBranch(), pCommand.GetBranch(), true) == 0)
					{
						// Indicate that this job is packaging a similar network package command
						return true;
					}
				}

				// In addition lets check all pending merge commands to make sure we don't have a deadlock situation coming down the road
				IDictionaryEnumerator enumerator = mPackageCommandList.GetEnumerator();
				while (enumerator.MoveNext())
				{
					MOG_Command command = (MOG_Command)(enumerator.Value);

					// Check if the CommandType matches?
					// Check if the AssetFilename matches?
					// Check if the Platforms matches?
					// Check if the Branch matches?
					if (command.GetCommandType() == pCommand.GetCommandType() &&
					String.Compare(command.GetAssetFilename().GetOriginalFilename(), pCommand.GetAssetFilename().GetOriginalFilename(), true) == 0 &&
					String.Compare(command.GetPlatform(), pCommand.GetPlatform(), true) == 0 &&
					String.Compare(command.GetBranch(), pCommand.GetBranch(), true) == 0)
					{
						// Indicate that this job will soon be packaging a similar network package command
						return true;
					}
				}
			}

			return false;
		}


		internal bool Process()
		{
			// Check if we haven't officially started the job yet?
			if (mStartJobCommand == null)
			{
				return false;
			}

			// Check if we have already determined this job can be skipped for now?
			if (mSkipJob)
			{
				return false;
			}

			// Process a command from this level
			if (ProcessCommandLevel(mTaskCommandList))
			{
				// Stop processing
				return true;
			}
			// Check if we have completed the commands for this level?
			if (IsCommandLevelFinished(mTaskCommandList))
			{
				if (ProcessCommandLevel(mPrimaryCommandList))
				{
					// Stop processing
					return true;
				}
				// Check if we have completed the commands for this level?
				if (IsCommandLevelFinished(mPrimaryCommandList))
				{
					if (ProcessCommandLevel(mPackageCommandList))
					{
						// Stop processing
						return true;
					}
					// Check if we have completed the commands for this level?
					if (IsCommandLevelFinished(mPackageCommandList))
					{
						if (ProcessCommandLevel(mPostCommandList))
						{
							// Stop processing
							return true;
						}
						// Check if we have completed the commands for this level?
						if (IsCommandLevelFinished(mPostCommandList))
						{
							// We can clear this list of network package commands now that we have finished posting
							mProcessedPackageCommands.Clear();

							if (ProcessCommandLevel(mDeferredCommandList))
							{
								// Stop processing
								return true;
							}
						}
					}
				}
			}

			// Indicate we failed to initiate our next command so start skipping this job
			//	mSkipJob = true;
			return false;
		}

		private bool IsCommandLevelFinished(HybridDictionary commandLevel)
		{
			bool bCommandLevelFinished = false;

			// Never continue until all the tasks commands have finished
			if (commandLevel.Count == 0)
			{
				// Looks like we might just be finished because our queue is empy
				bCommandLevelFinished = true;

				// Doublecheck the active slaves because we shouldn't move on until all slaves have finished their jobs for this last level?
				foreach (MOG_Command slave in mServerJobManager.GetActiveSlaves(GetJobLabel()))
				{
					// Make sure no slave is working on our level
					if (GetJobCommandList(slave.GetCommand()) == commandLevel)
					{
						// Nope, we still have a slave working on one of our commands
						bCommandLevelFinished = false;
						break;
					}
				}
			}

			return bCommandLevelFinished;
		}

		private bool ProcessCommandLevel(HybridDictionary commandLevel)
		{
			// Enumerate through the remaining primary commands
			IDictionaryEnumerator enumerator = commandLevel.GetEnumerator();
			while (enumerator.MoveNext())
			{
				MOG_Command pCommand = (MOG_Command)(enumerator.Value);

				// Attempt to process this command
				if (ProcessCommand(pCommand))
				{
					// Stop processing
					return true;
				}
			}
			
			return false;
		}

		internal bool FinishJob()
		{
			bool bFinished = false;

			// First make sure this Job is Finished
			if (IsJobFinished())
			{
				// Inform everyone that this job has been finished
				MOG_Command complete = MOG_CommandFactory.Setup_Complete(mStartJobCommand, true);
				if (mServerJobManager.GetServerCommandManager().SendToServer(complete))
				{
					bFinished = true;
				}
			}

			return bFinished;
		}


		internal bool ProcessCommand(MOG_Command pCommand)
		{
			bool bProcessed = false;

			// Make sure this command hasn't already been tasked out to a slave?
			if (pCommand.GetAssignedSlaveID() == 0)
			{
				// Attempt to process this command
				if (MOG_ControllerSystem.GetCommandManager().CommandProcess(pCommand))
				{
					// Indicate the command was processed
					bProcessed = true;

					// Check if this command was a network package merge command?
					if (pCommand.GetCommandType() == MOG_COMMAND_TYPE.MOG_COMMAND_NetworkPackageMerge)
					{
						// Keep track of this command so we can watch exclusivity with other packaging commands in other jobs
						mProcessedPackageCommands.Add(pCommand);
					}
				}
			}

			return bProcessed;
		}


		internal bool AddCommand(MOG_Command pCommand)
		{
			bool bSuccess = false;

			// Check if this is the StartJob command?
			if (pCommand.GetCommandType() == MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_StartJob)
			{
				mStartJobCommand = pCommand;
				bSuccess = true;
			}
			else
			{
				// Check if this command has requested to have duplicate commands removed?
				if (pCommand.IsRemoveDuplicateCommands())
				{
					// Scan the Server's command queue looking for any other instance of this same command?
					if (RemoveDuplicateCommands(pCommand))
					{
					}
				}

				// Check if this is a completed command?
				if (pCommand.GetCommandType() == MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_Complete)
				{
					// Immediately process 'Complete' commands
					if (ProcessCommand(pCommand))
					{
						// Remove the original command that is encapsulated within this 'Complete' command
						RemoveCommand(pCommand.GetCommand());
						bSuccess = true;
					}
				}
				else
				{
					// Get the appropriate command list for this command
					HybridDictionary commandList = GetJobCommandList(pCommand);
					if (commandList != null)
					{
						// Make sure we always clear the command's assigned slave or else it will never get processed
						pCommand.SetAssignedSlaveID(0);

						// Add this command to the appropriate list for later processing
						commandList[pCommand.GetCommandID().ToString()] = pCommand;
						bSuccess = true;
					}
				}
			}

			return bSuccess;
		}


		internal bool RemoveCommand(MOG_Command pCommand)
		{
			bool bRemoved = false;

			// Get the appropriate command list for this command
			HybridDictionary commandList = GetJobCommandList(pCommand);
			if (commandList != null)
			{
				commandList.Remove(pCommand.GetCommandID().ToString());
				bRemoved = true;
			}

			return bRemoved;
		}


		internal bool RecoverCommand(MOG_Command pCommand)
		{
			bool bRecovered = false;

			// Get the appropriate command list for this command
			HybridDictionary commandList = GetJobCommandList(pCommand);
			if (commandList != null)
			{
				if (commandList.Contains(pCommand.GetCommandID().ToString()))
				{
					// Clear the assigned slave
					pCommand.SetAssignedSlaveID(0);
					commandList[pCommand.GetCommandID().ToString()] = pCommand;
					bRecovered = true;
				}
			}

			return bRecovered;
		}


		internal bool KillCommand(UInt32 commandID)
		{
			// Simply remove the command from all of our lists
			mTaskCommandList.Remove(commandID.ToString());
			mPrimaryCommandList.Remove(commandID.ToString());
			mPackageCommandList.Remove(commandID.ToString());
			mPostCommandList.Remove(commandID.ToString());
			mDeferredCommandList.Remove(commandID.ToString());

			return true;
		}


		internal ArrayList GetAllPendingCommands()
		{
			ArrayList commands = new ArrayList();

			// Check if we have officially started the job?
			if (mStartJobCommand != null)
			{
				commands.Add(mStartJobCommand);
			}

			// Add all of the command lists
			commands.AddRange(mTaskCommandList.Values);
			commands.AddRange(mPrimaryCommandList.Values);
			commands.AddRange(mPackageCommandList.Values);
			commands.AddRange(mPostCommandList.Values);
			commands.AddRange(mDeferredCommandList.Values);

			return commands;
		}


		HybridDictionary GetJobCommandList(MOG_Command pCommand)
		{
			HybridDictionary commandList = null;

			switch (pCommand.GetCommandType())
			{
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_AssetRipRequest:
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_SlaveTask:
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_ReinstanceAssetRevision:
					commandList = mTaskCommandList;
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_Bless:
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_AssetProcessed:
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_RemoveAssetFromProject:
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_NetworkPackageRebuild:
					commandList = mPrimaryCommandList;
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_NetworkPackageMerge:
					commandList = mPackageCommandList;
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_Post:
					commandList = mPostCommandList;
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_Archive:
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_ScheduleArchive:
					commandList = mDeferredCommandList;
					break;
			}

			return commandList;
		}


		bool RemoveDuplicateCommands(MOG_Command pCommand)
		{
			bool bRemoved = false;

			// Get the appropriate command list for this command
			HybridDictionary commandList = GetJobCommandList(pCommand);
			if (commandList != null)
			{
				// Enumerate through the post commands
				IDictionaryEnumerator enumerator = commandList.GetEnumerator();
				while (enumerator.MoveNext())
				{
					MOG_Command thisCommand = (MOG_Command)(enumerator.Value);

					// Check if this is a duplicate command?
					if (IsDuplicateCommand(thisCommand, pCommand))
					{
						// Remove this command
						bRemoved = RemoveCommand(thisCommand);
						break;
					}
				}
			}

			return bRemoved;
		}


		bool IsDuplicateCommand(MOG_Command pCommand1, MOG_Command pCommand2)
		{
			// Check if the CommandType matches?
			// Check if the AssetFilename matches?
			// Check if the Platforms matches?
			// Check if the Branch matches?
			// Check if the JobLabel matches?
			// Check if the ValidSlaves matches?
			if (pCommand1.GetCommandType() == pCommand2.GetCommandType() &&
				String.Compare(pCommand1.GetAssetFilename().GetOriginalFilename(), pCommand2.GetAssetFilename().GetOriginalFilename(), true) == 0 &&
				String.Compare(pCommand1.GetPlatform(), pCommand2.GetPlatform(), true) == 0 &&
				String.Compare(pCommand1.GetBranch(), pCommand2.GetBranch(), true) == 0 &&
				String.Compare(pCommand1.GetJobLabel(), pCommand2.GetJobLabel(), true) == 0 &&
				String.Compare(pCommand1.GetValidSlaves(), pCommand2.GetValidSlaves(), true) == 0)
			{
				return true;
			}

			return false;
		}


		internal bool Command_RequestActiveCommands(MOG_Command pCommand)
		{
			bool bFailed = false;

			// Send off the Notify commands for this commands list
			IDictionaryEnumerator enumerator = mTaskCommandList.GetEnumerator();
			while (enumerator.MoveNext())
			{
				MOG_Command pActiveCommand = (MOG_Command)(enumerator.Value);
				MOG_Command pNotify = MOG_CommandFactory.Setup_NotifyActiveCommand(pActiveCommand);
				if (!mServerJobManager.GetServerCommandManager().SendToConnection(pCommand.GetNetworkID(), pNotify))
				{
					bFailed = true;
				}
			}

			// Send off the Notify commands for this commands list
			enumerator = mPrimaryCommandList.GetEnumerator();
			while (enumerator.MoveNext())
			{
				MOG_Command pActiveCommand = (MOG_Command)(enumerator.Value);
				MOG_Command pNotify = MOG_CommandFactory.Setup_NotifyActiveCommand(pActiveCommand);
				if (!mServerJobManager.GetServerCommandManager().SendToConnection(pCommand.GetNetworkID(), pNotify))
				{
					bFailed = true;
				}
			}

			// Send off the Notify commands for this commands list
			enumerator = mPackageCommandList.GetEnumerator();
			while (enumerator.MoveNext())
			{
				MOG_Command pActiveCommand = (MOG_Command)(enumerator.Value);
				MOG_Command pNotify = MOG_CommandFactory.Setup_NotifyActiveCommand(pActiveCommand);
				if (!mServerJobManager.GetServerCommandManager().SendToConnection(pCommand.GetNetworkID(), pNotify))
				{
					bFailed = true;
				}
			}

			// Send off the Notify commands for this commands list
			enumerator = mPostCommandList.GetEnumerator();
			while (enumerator.MoveNext())
			{
				MOG_Command pActiveCommand = (MOG_Command)(enumerator.Value);
				MOG_Command pNotify = MOG_CommandFactory.Setup_NotifyActiveCommand(pActiveCommand);
				if (!mServerJobManager.GetServerCommandManager().SendToConnection(pCommand.GetNetworkID(), pNotify))
				{
					bFailed = true;
				}
			}

			// Send off the Notify commands for this commands list
			enumerator = mDeferredCommandList.GetEnumerator();
			while (enumerator.MoveNext())
			{
				MOG_Command pActiveCommand = (MOG_Command)(enumerator.Value);
				MOG_Command pNotify = MOG_CommandFactory.Setup_NotifyActiveCommand(pActiveCommand);
				if (!mServerJobManager.GetServerCommandManager().SendToConnection(pCommand.GetNetworkID(), pNotify))
				{
					bFailed = true;
				}
			}

			// Check if we failed?
			if (!bFailed)
			{
				return true;
			}
			return false;
		}
	}
}
