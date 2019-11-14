using System;
using System.Collections.Generic;
using System.Text;
using MOG.COMMAND;
using System.Collections;
using MOG.INI;
using MOG.REPORT;
using MOG.PROMPT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG;
using System.IO;
using System.Xml.Serialization;
using MOG.FILENAME;
using MOG.DOSUTILS;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.DATABASE;
using System.Collections.Specialized;

namespace MOG_Server
{
	public class MOG_CommandServerCS : MOG_CommandManager
	{
		private UInt32 gServerCommandIDCounter = 1;

		public MOG_License mLicense;

		protected MOG_ServerJobManager mServerJobManager;  // !!!!FIX THIS!!!!!

		// Track active connections to the server
		protected ArrayList mRegisteredClients;
		protected ArrayList mRegisteredSlaves;
		protected ArrayList mRegisteredEditors;
		protected ArrayList mRegisteredCommandLines;

		protected ArrayList mActiveViews;

		protected ArrayList mActiveWriteLocks;
		protected ArrayList mActiveReadLocks;

		public MOG_CommandServerCS()
		{
			mNetwork = new MOG_NetworkServer();
			mLicense = new MOG_License();

			mRegisteredClients = new ArrayList();
			mRegisteredSlaves = new ArrayList();
			mRegisteredEditors = new ArrayList();
			mRegisteredCommandLines = new ArrayList();

			mActiveViews = new ArrayList();

			mActiveWriteLocks = new ArrayList();
			mActiveReadLocks = new ArrayList();

			mServerJobManager = new MOG_ServerJobManager(this);
		}

		public override bool Initialize()
		{
			// Load the Network settings from the MOG_SYSTEM_CONFIGFILENAME
			MOG_Ini pConfigFile = MOG_ControllerSystem.GetSystem().GetConfigFile();

			int serverPort = pConfigFile.GetValue("Network", "ServerPort");

			do
			{
				MOG_NetworkServer pServer = (MOG_NetworkServer)(mNetwork);
				if (pServer != null)
				{
					if (!pServer.Initialize(serverPort))
					{
						// Inform the user that there is already a server running on this port
						MOGPromptResult choice = MOG_Report.ReportResponse("Server Initialization Failed",
							String.Concat("There is already a server running on port ", serverPort.ToString()), "", MOGPromptButtons.RetryCancel, MOG_ALERT_LEVEL.ALERT);
						if (choice != MOGPromptResult.Retry)
						{
							return false;
						}
					}
				}
			} while (mNetwork.GetID() == 0);
			return true;
		}

		public override bool Shutdown()
		{
			MOG_NetworkServer pServer = (MOG_NetworkServer)(GetNetwork());
			if (pServer != null)
			{
				pServer.CloseAllConnections();
			}

			DumpServerStateToFile(String.Concat(MOG_Main.GetExecutablePath(), "\\ServerStateDump.info"));

			MOG_Ini mIni = new MOG_Ini(String.Concat(MOG_Main.GetExecutablePath(), "\\mog.ini"));
			mIni.PutBool("SERVERSTATUS", "ServerRunning", false);
			mIni.Save();
			mIni.Close();

			return true;
		}

		public override void Process()
		{
			// Send any pending packets 
			if (mNetwork != null)
			{
				mNetwork.SendPendingPackets();
			}

			// Check if we have any commands for processing?
			if (mCommands.Count > 0)
			{
				// Use a temp array as we attempt to process the commands to avoid thread issues
				ArrayList tempCommandList = mCommands;
				mCommands = new ArrayList();

				// Loop through all our commands...don't always increment our counter
				for (int c = 0; c < tempCommandList.Count; /* c++ */)
				{
					// Attempt to process this command
					MOG_Command pCommand = tempCommandList[c] as MOG_Command;
					if (CommandProcess(pCommand))
					{
						// Remove this processed command from our array
						tempCommandList.RemoveAt(c);
						// Don't increment our counter because the Remove will collapse our array wround us
						continue;
					}

					// Increment over this unprocessed command
					c++;
				}

				// Put all unprocessed commands back in for the next loop
				mCommands.InsertRange(0, tempCommandList);
			}

			// Process all the pending Jobs
			mServerJobManager.ProcessJobs();
		}


	// Server State Dump
		public bool DumpServerStateToFile(string filename)
		{
			if (filename != null && filename.Length > 0)
			{
				try
				{
					// use a MOG_ShutdownInfo struct to encode all the command lists and the shutdown time
					MOG_ShutdownInfo shutdownInfo = new MOG_ShutdownInfo();

					// Indicate the time of this dump file
					shutdownInfo.ShutdownTime = DateTime.Now;

					// First, add the commands in mCommands
					shutdownInfo.CommandsList = mCommands;
					// Second, add all of the pending job commands
					shutdownInfo.CommandsList.AddRange(mServerJobManager.GetAllPendingCommands());

					// Open the file specified for output and serialize the lists
					TextWriter writer = new StreamWriter(filename);
					if (writer != null)
					{
						// Tell the XmlSerializer which types we will be serializing
						Type[] typeArray = new Type[3];
						typeArray[0] = typeof(MOG_Command);
						typeArray[1] = typeof(MOG_Filename);
						typeArray[2] = typeof(MOG_ShutdownInfo);

						// Serialize the data
						XmlSerializer serializer = new XmlSerializer(typeof(MOG_ShutdownInfo), typeArray);
						serializer.Serialize(writer, shutdownInfo);
						writer.Flush();
						writer.Close();

						// Success
						return true;
					}
				}
				catch (Exception ex)
				{
					MOG_Prompt.PromptMessage("Save Server State Error!", ex.Message, ex.StackTrace, MOG_ALERT_LEVEL.CRITICAL);
					//MessageBox.Show(ex.ToString());
					return false;
				}
			}

			return false;
		}

		public bool RestoreServerStateFromFile(string filename)
		{
			if (filename != null && filename.Length > 0 && File.Exists(filename))
			{
				MOG_ShutdownInfo shutdownInfo = null;

				// Tell the XmlSerializer which types we will be deserializing
				Type[] typeArray = new Type[3];
				typeArray[0] = typeof(MOG_Command);
				typeArray[1] = typeof(MOG_Filename);
				typeArray[2] = typeof(MOG_ShutdownInfo);

				StreamReader reader = new StreamReader(filename);
				if (reader != null)
				{
					try
					{
						// Check if we have a valid file to read?
						if (reader.Peek() > -1)
						{
							// Extract the serialized MOG_ShutdownInfo object
							XmlSerializer serializer = new XmlSerializer(typeof(MOG_ShutdownInfo), typeArray);
							shutdownInfo = (MOG_ShutdownInfo)(serializer.Deserialize(reader));
						}
					}
					catch (Exception e)
					{
						// JohnRen - Don't tell the user about this since there is nothing they can do about it
						e.ToString();
					}
					finally
					{
						reader.Close();
					}
				}

				// If we got a valid MOG_ShutdownInfo, extract its contained ArrayLists
				if (shutdownInfo != null)
				{
					// Set last shutdown time
					MOG_ControllerSystem.SetLastServerShutdownTime(shutdownInfo.ShutdownTime);

					// Add all the commands to the server
					for (int c = 0; c < shutdownInfo.CommandsList.Count; c++)
					{
						// Attempt to process this command
						MOG_Command pCommand = shutdownInfo.CommandsList[c] as MOG_Command;
						AddCommand(pCommand);
					}

					// Success
					return true;
				}
			}

			return false;
		}

		public ArrayList GetAllPendingCommands()
		{
			ArrayList commands = new ArrayList();

			// Add the immediate commands first
			commands.AddRange(mCommands);

			// Add the job based commands
			commands.AddRange(mServerJobManager.GetAllPendingCommands());

			return commands;
		}

		public override bool ReadConnections()
		{
			ArrayList packets;

			// Check for new connections?
			MOG_NetworkServer pServer = (MOG_NetworkServer)(mNetwork);
			if (pServer != null)
			{
				int newConnectionID = pServer.CheckNewConnections();
				if (newConnectionID != 0)
				{
					// Send the ConnectionNew command back
					String disabledFeatureList = mLicense.GetDisabledFeatureList();
					MOG_Command newConnectionCommand = MOG_CommandFactory.Setup_ConnectionNew(newConnectionID, MOG_ControllerSystem.GetDB().GetConnectionString(), disabledFeatureList);
					SendToConnection(newConnectionID, newConnectionCommand);
				}

				// Read new unverified connections
				packets = pServer.ReadNewConnections();
				// Cycle through all the new commands
				for (int p = 0; p < packets.Count; p++)
				{
					// Process each packet
					ProcessNetworkPacket((NetworkPacket)(packets[p]));
				}

				// Talk to each of the RegisteredClients
				for (int c = 0; c < mRegisteredClients.Count; c++)
				{
					MOG_Command client = (MOG_Command)(mRegisteredClients[c]);

					// Read the packets from the network
					packets = pServer.ReadPackets(client.GetNetworkID());
					if (packets != null)
					{
						// Cycle through all the returned NetworkPackets
						for (int p = 0; p < packets.Count; p++)
						{
							// Process each packet
							ProcessNetworkPacket((NetworkPacket)(packets[p]));
						}
					}
					else
					{
						// Properly shutdown this connection
						MOG_Command pShutDown = MOG_CommandFactory.Setup_ShutdownClient(client);
						AddCommand(pShutDown);
					}
				}

				// Talk to each of the RegisteredEditors
				for (int c = 0; c < mRegisteredEditors.Count; c++)
				{
					MOG_Command editor = (MOG_Command)(mRegisteredEditors[c]);

					// Read the packets from the network
					packets = pServer.ReadPackets(editor.GetNetworkID());
					if (packets != null)
					{
						// Cycle through all the returned NetworkPackets
						for (int p = 0; p < packets.Count; p++)
						{
							// Process each packet
							ProcessNetworkPacket((NetworkPacket)(packets[p]));
						}
					}
					else
					{
						// Properly shutdown this connection
						MOG_Command pShutDown = MOG_CommandFactory.Setup_ShutdownEditor(editor);
						AddCommand(pShutDown);
					}
				}

				// Talk to each of the RegisteredCommandLines
				for (int c = 0; c < mRegisteredCommandLines.Count; c++)
				{
					MOG_Command commandLine = (MOG_Command)(mRegisteredCommandLines[c]);

					// Read the packets from the network
					packets = pServer.ReadPackets(commandLine.GetNetworkID());
					if (packets != null)
					{
						// Cycle through all the returned NetworkPackets
						for (int p = 0; p < packets.Count; p++)
						{
							// Process each packet
							ProcessNetworkPacket((NetworkPacket)(packets[p]));
						}
					}
					else
					{
						// Properly shutdown this connection
						MOG_Command pShutDown = MOG_CommandFactory.Setup_ShutdownCommandLine(commandLine);
						AddCommand(pShutDown);
					}
				}

				// Talk to each of the RegisteredSlaves
				for (int c = 0; c < mRegisteredSlaves.Count; c++)
				{
					MOG_Command slave = (MOG_Command)(mRegisteredSlaves[c]);

					// Read the packets from the network
					packets = pServer.ReadPackets(slave.GetNetworkID());
					if (packets != null)
					{
						// Cycle through all the returned NetworkPackets
						for (int p = 0; p < packets.Count; p++)
						{
							// Process each packet
							ProcessNetworkPacket((NetworkPacket)(packets[p]));
						}
					}
					else
					{
						// Properly shutdown this connection
						MOG_Command pShutDown = MOG_CommandFactory.Setup_ShutdownSlave(slave);
						AddCommand(pShutDown);
					}
				}
			}

			return true;
		}

		public override bool AddCommand(MOG_Command pCommand)
		{
			bool bAdded = false;

			if (pCommand != null)
			{
				// Check if this command came from someone other than the server?
				if (pCommand.GetNetworkID() > 1)
				{
					// Check if this command requires a license?
					if (pCommand.IsLicenseRequired())
					{
						// Confirm the status of this client's license?
						if (!mLicense.ConfirmLicense(pCommand))
						{
							// Shutdown the connection
							MOG_ControllerSystem.ConnectionKill(pCommand.GetNetworkID());
							return false;
						}
					}
				}

				// Check if this command is part of a job?
				if (pCommand.GetJobLabel().Length > 0)
				{
					// Add this command to the job manager
					bAdded = mServerJobManager.AddCommand(pCommand);
					if (!bAdded)
					{
						// Warn the user that this command failed to get added to a job
						string message = String.Concat("Server Job Error!\n",
															"COMMAND: ", pCommand.ToString(), "\n",
															"ASSET: ", pCommand.GetAssetFilename().GetOriginalFilename(), "\n",
															"This command failed to be added to it's server job.");
						MOG_ControllerSystem.NetworkBroadcast(pCommand.GetUserName(), message);
					}
				}
				else
				{
					// Call the Parent's CommandAdd 
					bAdded = base.AddCommand(pCommand);
				}

				// Check if we added the command?
				if (bAdded)
				{
					// Notify the server
					// If callback exist, call it
					if (mCallbacks != null && mCallbacks.mEventCallback != null)
					{
						mCallbacks.mEventCallback.Invoke(pCommand);
					}

					// Send out needed Notify command to clients
					MOG_Command pNotify = MOG_CommandFactory.Setup_NotifyActiveCommand(pCommand);
					SendToActiveTab("Connections", pNotify);
				}
			}

			return bAdded;
		}

		public override bool CommandProcess(MOG_Command pCommand)
		{
			// Allow the Parent to attempt to process this command?
			if (base.CommandProcess(pCommand))
			{
				// Report informative information concerning this processed command
				ReportProcessedCommandLog(pCommand);

				// Never bother to send the None commands
				if (pCommand.GetCommandType() != MOG_COMMAND_TYPE.MOG_COMMAND_None)
				{
					// Send out needed Notify command
					MOG_Command pNotify = MOG_CommandFactory.Setup_NotifyActiveCommand(pCommand);
					SendToActiveTab("Connections", pNotify);
				}

				return true;
			}

			return false;
		}

		public override bool CommandExecute(MOG_Command pCommand)
		{
			bool processed = false;

			switch (pCommand.GetCommandType())
			{
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_ConnectionKill:
					processed = Command_ConnectionKill(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_RegisterClient:
					processed = Command_RegisterClient(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_ShutdownClient:
					processed = Command_ShutdownClient(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_RegisterSlave:
					processed = Command_RegisterSlave(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_ShutdownSlave:
					processed = Command_ShutdownSlave(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_RegisterEditor:
					processed = Command_RegisterEditor(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_ShutdownEditor:
					processed = Command_ShutdownEditor(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_RegisterCommandLine:
					processed = Command_RegisterCommandLine(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_ShutdownCommandLine:
					processed = Command_ShutdownCommandLine(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_SQLConnection:
					processed = Command_SQLConnection(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_MOGRepository:
					processed = Command_MOGRepository(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_LoginProject:
					processed = Command_LoginProject(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_LoginUser:
					processed = Command_LoginUser(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_ActiveViews:
					processed = Command_ActiveViews(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_ViewUpdate:
					processed = Command_ViewUpdate(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_RefreshApplication:
					processed = Command_RefreshApplication(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_RefreshTools:
					processed = Command_RefreshTools(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_RefreshProject:
					processed = Command_RefreshProject(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_AssetRipRequest:
					processed = Command_AssetRipRequest(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_AssetProcessed:
					processed = Command_AssetProcessed(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_SlaveTask:
					processed = Command_SlaveTask(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_ReinstanceAssetRevision:
					processed = Command_ReinstanceAssetRevision(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_Bless:
					processed = Command_Bless(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_RemoveAssetFromProject:
					processed = Command_RemoveAssetFromProject(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_NetworkPackageMerge:
					processed = Command_NetworkPackageMerge(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_LocalPackageMerge:
					processed = Command_LocalPackageMerge(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_NetworkPackageRebuild:
					processed = Command_PackageRebuild(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_Post:
					processed = Command_Post(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_Archive:
					processed = Command_Archive(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_ScheduleArchive:
					processed = Command_ScheduleArchive(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_LockReadRequest:
					processed = Command_LockReadRequest(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_LockReadRelease:
					processed = Command_LockReadRelease(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_LockReadQuery:
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_LockWriteQuery:
					processed = Command_LockQuery(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_LockWriteRequest:
					processed = Command_LockWriteRequest(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_LockWriteRelease:
					processed = Command_LockWriteRelease(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_GetEditorInfo:
					processed = Command_GetEditorInfo(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_NetworkBroadcast:
					processed = Command_NetworkBroadcast(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_BuildFull:
					processed = Command_BuildFull(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_Build:
					processed = Command_Build(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_InstantMessage:
					processed = Command_InstantMessage(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_NotifySystemAlert:
					processed = Command_NotifySystemAlert(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_NotifySystemError:
					processed = Command_NotifySystemError(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_NotifySystemException:
					processed = Command_NotifySystemException(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_RequestActiveCommands:
					processed = Command_RequestActiveCommands(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_RequestActiveLocks:
					processed = Command_RequestActiveLocks(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_RequestActiveConnections:
					processed = Command_RequestActiveConnections(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_RetaskCommand:
					processed = Command_RetaskCommand(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_KillCommand:
					processed = Command_KillCommand(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_LaunchSlave:
					processed = Command_LaunchSlave(pCommand);
					break;
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_StartJob:
					processed = Command_StartJob(pCommand);
					break;

				default:
					// Allow the parent a chance to process this command
					processed = base.CommandExecute(pCommand);
					break;
			}

			return processed;
		}

		public override bool SendToServer(MOG_Command pCommand)
		{
			// We always need to populate certain information within the command...
			pCommand.SetSystemDefaultCommandSettings();
			// Make sure we set a unique CommandID for this command
			SetUniqueCommandID(pCommand);

			// Just call CommandAdd() because we are already the server
			return AddCommand(pCommand);
		}

		public override bool SendToServerBlocking(MOG_Command pCommand)
		{
			return CommandProcess(pCommand);
		}

		public ArrayList GetRegisteredClients()
		{
			return mRegisteredClients;
		}

		public ArrayList GetRegisteredSlaves()
		{
			return mRegisteredSlaves;
		}

		public ArrayList GetRegisteredEditors()
		{
			return mRegisteredEditors;
		}

		public ArrayList GetRegisteredCommandLines()
		{
			return mRegisteredCommandLines;
		}

		public ArrayList GetActiveViews()
		{
			return mActiveViews;
		}

		public ArrayList GetActiveWriteLocks()
		{
			return mActiveWriteLocks;
		}

		public ArrayList GetActiveReadLocks()
		{
			return mActiveReadLocks;
		}

		// Auxilary Routines.
		public bool SendPackageCommandToProperComponent(MOG_Command pCommand)
		{
			bool bFoundEditor = false;
			bool bSent = false;

			switch (pCommand.GetCommandType())
			{
				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_LocalPackageMerge:
					// Scan all of the editors looking to see if one of them can process this command?
					// Scan all the registered clients looking for this one
					foreach (MOG_Command editor in mRegisteredEditors)
					{
						// Check if this Editor is running on the same machine?  and
						if (string.Compare(editor.GetComputerName(), pCommand.GetComputerName(), true) == 0)
						{
							// Check if this Editor is logged into the same project?  and
							// Check if this Editor is logged in as the same user?
							if (string.Compare(editor.GetProject(), pCommand.GetProject(), true) == 0 &&
								string.Compare(editor.GetUserName(), pCommand.GetUserName(), true) == 0)
							{
								// Check if this Editor is associated with this command's specified workspace directory?
								string testWorkspace1 = editor.GetWorkingDirectory() + "\\";
								string testWorkspace2 = pCommand.GetWorkingDirectory() + "\\";
								if (testWorkspace1.StartsWith(testWorkspace2, StringComparison.CurrentCultureIgnoreCase) ||
									testWorkspace2.StartsWith(testWorkspace1, StringComparison.CurrentCultureIgnoreCase))
								{
									bFoundEditor = true;

									// Send the command to the Editor (Allow the Editor to perform this to avoid sharing violations)
									if (SendToConnection(editor.GetNetworkID(), pCommand))
									{
										bSent = true;
									}
								}
							}
						}
					}
					// Check if we never found an associated editor?
					if (!bFoundEditor)
					{
						// Make sure this command was generated from a client?
						MOG_Command client = LocateClientByID(pCommand.GetNetworkID());
						if (client != null)
						{
							// Being that there is no editor open, send it back to the client to execute the command
							if (SendToConnection(client.GetNetworkID(), pCommand))
							{
								bSent = true;
							}
						}
					}
					break;

				case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_Complete:
					// Make sure it contains an encapsulated command
					if (pCommand.GetCommand() != null)
					{
						// Determin what the encapsulated command is
						switch (pCommand.GetCommand().GetCommandType())
						{
							case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_NetworkPackageMerge:
							case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_NetworkPackageRebuild:
							case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_LocalPackageMerge:
							case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_EditorPackageMergeTask:
								// Always send it back to the originator which is what is encoded within the command
								bSent = SendToConnection(pCommand.GetCommand().GetNetworkID(), pCommand);
								break;
						}
					}
					break;

				default:
					break;
			}

			// Indicate wether we were able to send this command to the editor
			return bSent;
		}

		public MOG_Command LocateClientByComputerName(string computerName)
		{
			// Scan all the registered clients looking for this one
			for (int c = 0; c < mRegisteredClients.Count; c++)
			{
				MOG_Command pClient = (MOG_Command)(mRegisteredClients[c]);

				// Make sure this is a MOG Client?
				if (string.Compare(pClient.GetDescription(), "Client", true) == 0)
				{
					// Does this client match?
					if (string.Compare(pClient.GetComputerName(), computerName, true) == 0)
					{
						// return this available slave
						return pClient;
					}
				}
			}

			return null;
		}

		public MOG_Command LocateClientByUserName(string userName)
		{
			// Scan all the registered clients looking for this one
			for (int c = 0; c < mRegisteredClients.Count; c++)
			{
				MOG_Command pClient = (MOG_Command)(mRegisteredClients[c]);

				// Make sure this is a MOG Client?
				if (string.Compare(pClient.GetDescription(), "Client", true) == 0)
				{
					// Does this client match?
					//?	MOG_CommandServer.LocateClientByUserName - We might need to include the project when trying to detect the proper client
					if (string.Compare(pClient.GetUserName(), userName, true) == 0)
					{
						// return this client
						return pClient;
					}
				}
			}

			return null;
		}

		public MOG_Command LocateClientByID(int id)
		{
			// Scan all the registered clients looking for this one
			for (int c = 0; c < mRegisteredClients.Count; c++)
			{
				MOG_Command pClient = (MOG_Command)(mRegisteredClients[c]);

				// Does this client match?
				if (pClient.GetNetworkID() == id)
				{
					// return this client
					return pClient;
				}
			}

			return null;
		}

		public MOG_Command LocateEditorByID(int id)
		{
			// Scan all the registered clients looking for this one
			for (int c = 0; c < mRegisteredEditors.Count; c++)
			{
				MOG_Command pEditor = (MOG_Command)(mRegisteredEditors[c]);

				// Does this client match?
				if (pEditor.GetNetworkID() == id)
				{
					// return this client
					return pEditor;
				}
			}

			return null;
		}

		public MOG_Command LocateAssociatedEditorByClientID(int clientID)
		{
			// Check if this command originated from a Client?
			MOG_Command pClient = LocateClientByID(clientID);
			if (pClient != null)
			{
				// Obtain the view associated with this Client
				MOG_Command pViews = LocateActiveViewByID(clientID);
				if (pViews != null)
				{
					// Match an associated Editor to this Client
					for (int c = 0; c < mRegisteredEditors.Count; c++)
					{
						MOG_Command pEditor = (MOG_Command)(mRegisteredEditors[c]);
						// Is this Editor running on this commands computer?
						if (string.Compare(pClient.GetComputerName(), pEditor.GetComputerName(), true) == 0)
						{
							// Check if this Editor is associated with this Clients workspace?
							if (pEditor.GetWorkingDirectory().ToLower().StartsWith(pViews.GetWorkingDirectory().ToLower()) ||
								pViews.GetWorkingDirectory().ToLower().StartsWith(pEditor.GetWorkingDirectory().ToLower()))
							{
								return pEditor;
							}
						}
					}
				}
			}

			return null;
		}

		public MOG_Command LocateAssociatedEditorByWorkspace(String computerName, String workspaceDirectory)
		{
			// Match an associated Editor to this Client
			for (int c = 0; c < mRegisteredEditors.Count; c++)
			{
				MOG_Command pEditor = (MOG_Command)(mRegisteredEditors[c]);

				// Is this Editor running on this commands computer?
				if (string.Compare(computerName, pEditor.GetComputerName(), true) == 0)
				{
					// Add on the ending '\' to ensure proper comparisons and avoid false hits with like directories
					string editorWorkspace = pEditor.GetWorkingDirectory() + "\\";
					string clientWorkspace = workspaceDirectory + "\\";
					// Check if this Editor is associated with this Clients workspace?
					if (editorWorkspace.StartsWith(clientWorkspace, StringComparison.CurrentCultureIgnoreCase) ||
						clientWorkspace.StartsWith(editorWorkspace, StringComparison.CurrentCultureIgnoreCase))
					{
						return pEditor;
					}
				}
			}

			return null;
		}

		public MOG_Command LocateAssociatedClientByEditorID(int editorID)
		{
			// Check if this command originated from a Client?
			MOG_Command pEditor = LocateEditorByID(editorID);
			if (pEditor != null)
			{
				// Match an associated Client to this Editor
				for (int c = 0; c < mRegisteredClients.Count; c++)
				{
					MOG_Command pClient = (MOG_Command)(mRegisteredClients[c]);
					// Is this Client running on this Editor's computer?
					if (string.Compare(pClient.GetComputerName(), pEditor.GetComputerName(), true) == 0)
					{
						// Obtain the view associated with this Client
						MOG_Command pViews = LocateActiveViewByID(pClient.GetNetworkID());
						if (pViews != null)
						{
							// Check if this Client is associated with this Editor's workspace?
							if (pEditor.GetWorkingDirectory().ToLower().StartsWith(pViews.GetWorkingDirectory().ToLower()) ||
								pViews.GetWorkingDirectory().ToLower().StartsWith(pEditor.GetWorkingDirectory().ToLower()))
							{
								return pClient;
							}
						}
					}
				}
			}

			return null;
		}

		public MOG_Command LocateRegisteredSlaveByComputerName(string computerName)
		{
			// Scan all the registered slaves looking for this one
			for (int c = 0; c < mRegisteredSlaves.Count; c++)
			{
				MOG_Command pSlave = (MOG_Command)(mRegisteredSlaves[c]);

				// Does this slave match?
				if (string.Compare(pSlave.GetComputerName(), computerName, true) == 0)
				{
					// return this slave
					return pSlave;
				}
			}

			return null;
		}

		public MOG_Command LocateCommandLineByComputerName(string computerName)
		{
			// Scan all the registered CommandLines looking for this one
			for (int c = 0; c < mRegisteredCommandLines.Count; c++)
			{
				MOG_Command commandLine = (MOG_Command)(mRegisteredCommandLines[c]);

				// Is this CommandLine running on this commands computer?
				if (string.Compare(commandLine.GetComputerName(), computerName, true) == 0)
				{
					// return this CommandLine
					return commandLine;
				}
			}

			return null;
		}

		public MOG_Command LocateRegisteredSlaveByID(int id)
		{
			// Scan all the registered slaves looking for this one
			for (int s = 0; s < mRegisteredSlaves.Count; s++)
			{
				MOG_Command slave = (MOG_Command)(mRegisteredSlaves[s]);

				// Is this slave running on this commands computer?
				if (slave.GetNetworkID() == id)
				{
					// return this available slave
					return slave;
				}
			}

			return null;
		}

		public MOG_Command LocateActiveViewByID(int networkID)
		{
			// Scan mActiveViews looking for this networkID
			for (int v = 0; v < mActiveViews.Count; v++)
			{
				MOG_Command view = (MOG_Command)(mActiveViews[v]);

				// Does this NetworkID match?
				if (view.GetNetworkID() == networkID)
				{
					return view;
				}
			}

			return null;
		}

		public bool SendToConnection(int networkID, MOG_Command pCommand)
		{
			// Never send a command to our own connection!!!
			if (networkID == 1)
			{
				// Just eat the command
				return true;
			}

			MOG_NetworkServer pServer = (MOG_NetworkServer)(mNetwork);
			if (pServer != null)
			{
				return pServer.SendPacketToConnection(pCommand.Serialize(), networkID);
			}

			return false;
		}

		public bool SendToAllConnections(MOG_Command pCommand)
		{
			MOG_NetworkServer pServer = (MOG_NetworkServer)(mNetwork);
			if (pServer != null)
			{
				return pServer.SendPacketToAll(pCommand.Serialize());
			}

			return false;
		}

		public bool SendToActiveTab(string tab, MOG_Command pCommand)
		{
			// Scan mActiveViews looking for this tab
			for (int c = 0; c < mActiveViews.Count; c++)
			{
				MOG_Command view = (MOG_Command)(mActiveViews[c]);

				// Does this mActiveViews tab match the specified tab?
				if (string.Compare(view.GetTab(), tab, true) == 0)
				{
					// Send the command to this view
					SendToConnection(view.GetNetworkID(), pCommand);
				}
			}

			// Always eat the command
			return true;
		}
		public bool SendToUsers(string users, MOG_Command pCommand)
		{
			bool sent = false;

			// Process each listed user
			while (users.Length > 0)
			{
				string user;

				// Check to see if there are any more users listed
				int pos = users.IndexOf(",");
				if (pos != -1)
				{
					// Strip out the current user
					user = users.Substring(0, pos);
					// Remove the current user from the completion flow string
					users = users.Substring(pos + 1);
				}
				else
				{
					// Looks like there is only one user listed
					user = users;
					users = "";
				}

				// Clean up the user string...remove any spaces
				user = user.Replace(" ", "");

				// Make sure we found a userName
				if (user.Length > 0)
				{
					// Scan mRegisteredClients
					for (int c = 0; c < mRegisteredClients.Count; c++)
					{
						MOG_Command pClient = (MOG_Command)(mRegisteredClients[c]);

						// Does this client's project match the specified command's project?
						// Does this client's user match the specified user?
						if (string.Compare(pClient.GetProject(), pCommand.GetProject(), true) == 0 &&
							string.Compare(pClient.GetUserName(), user, true) == 0)
						{
							// Send the command to this client
							SendToConnection(pClient.GetNetworkID(), pCommand);
							sent = true;
						}
					}
				}
			}

			// Always eat the command
			return sent;
		}

		public bool SendToProject(string projectName, MOG_Command pCommand)
		{
			bool sent = false;

			// Scan mRegisteredClients
			for (int c = 0; c < mRegisteredClients.Count; c++)
			{
				MOG_Command pClient = (MOG_Command)(mRegisteredClients[c]);

				// Does this client's project match the specified projectName?
				if (string.Compare(pClient.GetProject(), projectName, true) == 0)
				{
					// Send the command to this client
					SendToConnection(pClient.GetNetworkID(), pCommand);
					sent = true;
				}
			}

			// Always eat the command
			return sent;
		}

		public bool SendToComputerName(string computerName, MOG_Command pCommand)
		{
			bool sent = false;

			// Scan mRegisteredClients
			for (int c = 0; c < mRegisteredClients.Count; c++)
			{
				MOG_Command pClient = (MOG_Command)(mRegisteredClients[c]);

				// Does this client's computer name match the specified computerName?
				if (string.Compare(pClient.GetComputerName(), computerName, true) == 0)
				{
					// Send the command to this client
					SendToConnection(pClient.GetNetworkID(), pCommand);
					sent = true;
				}
			}

			// Always eat the command
			return sent;
		}

		public bool SendPersistentLockToAllClients(MOG_Command pLock)
		{
			// Scan mRegisteredClients
			for (int c = 0; c < mRegisteredClients.Count; c++)
			{
				MOG_Command client = (MOG_Command)(mRegisteredClients[c]);

				// Check if this connection has a LockTracker?
				if (client.GetOptions().Contains("{MOG_LockTracker}"))
				{
					// Send the command to this client
					SendToConnection(client.GetNetworkID(), pLock);
				}
			}

			// Always eat the command
			return true;
		}

		public bool SendPersistentLockToAllEditors(MOG_Command pLock)
		{
			// Scan mRegisteredEditors
			for (int c = 0; c < mRegisteredEditors.Count; c++)
			{
				MOG_Command editor = (MOG_Command)(mRegisteredEditors[c]);

				// Check if this connection has a LockTracker?
				if (editor.GetOptions().Contains("{MOG_LockTracker}"))
				{
					// Send the command to this editor
					SendToConnection(editor.GetNetworkID(), pLock);
				}
			}

			// Always eat the command
			return true;
		}

		public bool SendPersistentLocksToConnection(int networkID)
		{
			ArrayList locks;
			int l;

			locks = GetActiveWriteLocks();
			for (l = 0; l < locks.Count; l++)
			{
				MOG_Command lockPersistant = (MOG_Command)(locks[l]);

				// check if this is a persistent lock?
				if (lockPersistant.IsPersistentLock())
				{
					// Inform the connection about this lock
					MOG_Command pNotify = MOG_CommandFactory.Setup_LockPersistentNotify(lockPersistant, "{Notify}");
					SendToConnection(networkID, pNotify);
				}
			}

			locks = GetActiveReadLocks();
			for (l = 0; l < locks.Count; l++)
			{
				MOG_Command lockPersistant = (MOG_Command)(locks[l]);

				// check if this is a persistent lock?
				if (lockPersistant.IsPersistentLock())
				{
					// Inform the connection about this lock
					MOG_Command pNotify = MOG_CommandFactory.Setup_LockPersistentNotify(lockPersistant, "{Notify}");
					SendToConnection(networkID, pNotify);
				}
			}

			return true;
		}


		// Utility Routines
		protected bool LockRequest(MOG_Command pCommand)
		{
			int l;
			MOG_Command containingCommand = pCommand;
			bool foundLock = false;

			// Both read and write locks need to check for existing write locks
			for (l = 0; l < mActiveWriteLocks.Count; l++)
			{
				MOG_Command lockRequest = (MOG_Command)(mActiveWriteLocks[l]);

				// By adding the "\\" we can check all of the following at once
				// Make sure there isn't an exact match?
				// Make sure there are no higher-level locks?
				// Make sure there are no lower-level locks?
				if (StringUtils.StringCompare(string.Concat(lockRequest.GetAssetFilename().GetOriginalFilename(), "\\"), string.Concat(pCommand.GetAssetFilename().GetOriginalFilename(), "\\")))
				{
					// Imbed the colliding lock
					containingCommand.SetCommand(lockRequest);
					foundLock = true;
				}
			}

			// Only write locks need to check for any existing read locks
			if (pCommand.GetCommandType() == MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_LockWriteRequest ||
				pCommand.GetCommandType() == MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_LockWriteQuery)
			{
				// Check for existing read locks
				for (l = 0; l < mActiveReadLocks.Count; l++)
				{
					MOG_Command lockRequest = (MOG_Command)(mActiveReadLocks[l]);

					// By adding the "\\" we can check all of the following at once
					// Make sure there isn't an exact match?
					// Make sure there are no higher-level locks?
					// Make sure there are no lower-level locks?
					if (StringUtils.StringCompare(string.Concat(lockRequest.GetAssetFilename().GetOriginalFilename(), "\\"), string.Concat(pCommand.GetAssetFilename().GetOriginalFilename(), "\\")))
					{
						// Imbed the colliding lock
						containingCommand.SetCommand(lockRequest);
						foundLock = true;
					}
				}
			}

			// Check to see if we found any collining locks
			if (pCommand.GetCommand() != null && foundLock)
			{
				return false;
			}

			return true;
		}

		protected bool IsLockDuplicated(MOG_Command pCommand)
		{
			int l;

			// Both read and write locks need to check for existing write locks
			for (l = 0; l < mActiveWriteLocks.Count; l++)
			{
				MOG_Command lockActive = (MOG_Command)(mActiveWriteLocks[l]);

				// By adding the "\\" we can check all of the following at once
				// Make sure there isn't an exact match?
				// Make sure there are no higher-level locks?
				// Make sure there are no lower-level locks?
				if (StringUtils.StringCompare(string.Concat(lockActive.GetAssetFilename().GetOriginalFilename(), "\\"), string.Concat(pCommand.GetAssetFilename().GetOriginalFilename(), "\\")))
				{
					// Just in case we just happen to receive the same command twice?
					if (lockActive.GetCommandID() == pCommand.GetCommandID())
					{
						return true;
					}
				}
			}

			// Only write locks need to check for any existing read locks
			if (pCommand.GetCommandType() == MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_LockWriteRequest ||
				pCommand.GetCommandType() == MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_LockWriteQuery)
			{
				// Check for existing read locks
				for (l = 0; l < mActiveReadLocks.Count; l++)
				{
					MOG_Command lockActive = (MOG_Command)(mActiveReadLocks[l]);

					// By adding the "\\" we can check all of the following at once
					// Make sure there isn't an exact match?
					// Make sure there are no higher-level locks?
					// Make sure there are no lower-level locks?
					if (StringUtils.StringCompare(string.Concat(lockActive.GetAssetFilename().GetOriginalFilename(), "\\"), string.Concat(pCommand.GetAssetFilename().GetOriginalFilename(), "\\")))
					{
						// Just in case we just happen to receive the same command twice?
						if (lockActive.GetCommandID() == pCommand.GetCommandID())
						{
							return true;
						}
					}
				}
			}

			return false;
		}

		protected bool ProcessNetworkPacket(NetworkPacket packet)
		{
			// Deserialize the packet
			MOG_Command command = MOG_Command.Deserialize((NetworkPacket)(packet));

			// Set this command's unique CommandID
			SetUniqueCommandID(command);

			// We must imeadiately respond to any blocking command
			if (command.IsBlocking())
			{
				// Try to process this command imeadiately
				command.SetCompleted(CommandProcess(command));
				// Send our reply back
				return SendToConnection(command.GetNetworkID(), command);
			}

			// Add the non-blocking commands to the CommandManager for processing
			AddCommand(command);
			return true;
		}

		protected bool ShutdownAllSlavesRunningOnComputer(string computerName)
		{
			// Scan all the registered slaves looking for this one
			for (int s = 0; s < mRegisteredSlaves.Count; s++)
			{
				MOG_Command slave = (MOG_Command)(mRegisteredSlaves[s]);

				// Check if this slave is running on the same computer?
				if (string.Compare(slave.GetComputerName(), computerName, true) == 0)
				{
					// Properly shutdown this connection
					MOG_ControllerSystem.ShutdownSlave(slave.GetNetworkID());
				}
			}

			return true;
		}

		protected bool ReportProcessedCommandLog(MOG_Command pCommand)
		{
			// 	switch(pCommand.GetCommandType())
			//	{
			//		case MOG_COMMAND_ConnectionNew:
			//		case MOG_COMMAND_ConnectionLost:
			//		case MOG_COMMAND_ConnectionKill:
			//		case MOG_COMMAND_RegisterClient:
			//		case MOG_COMMAND_ShutdownClient:
			//		case MOG_COMMAND_RegisterSlave:
			//		case MOG_COMMAND_ShutdownSlave:
			//		case MOG_COMMAND_RegisterEditor:
			//		case MOG_COMMAND_ShutdownEditor:
			//		case MOG_COMMAND_RegisterCommandLine:
			//		case MOG_COMMAND_ShutdownCommandLine:
			//			MOG_Report.LogComment(string.Concat(S"Processed:", pCommand.Tostring(), S"     User:", pCommand.GetUserName(), S"(", Convert.Tostring(pCommand.GetNetworkID()), S")     Computer:", pCommand.GetComputerName(), S"     ", pCommand.GetAssetFilename().GetOriginalFilename()));
			return true;
			//			break;
			//	}
			//	return false;
		}

		protected bool DeleteLocks(int networkID, string computerName)
		{
			// Scan all write locks looking for a matching networkID
			// Manually increment 'l' because the array will collapse around us as we remove locks
			for (int l = 0; l < mActiveWriteLocks.Count; )
			{
				MOG_Command lockObj = (MOG_Command)(mActiveWriteLocks[l]);

				// Do we have a networkID to compare and does it match this lock's NetworkID?
				if (networkID != 0 && networkID == lockObj.GetNetworkID())
				{
					// Do not kill persistent locks, they are set by users working on assets
					if (!lockObj.IsPersistentLock())
					{
						MOG_Command removeLock = new MOG_Command();

						// Manually construct removeCommand from the lock information
						removeLock.SetCommandType(MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_LockWriteRelease);
						removeLock.SetAssetFilename(lockObj.GetAssetFilename().GetOriginalFilename());
						removeLock.SetUserName(lockObj.GetUserName());
						removeLock.SetComputerName(lockObj.GetComputerName());
						removeLock.SetNetworkID(lockObj.GetNetworkID());
						removeLock.SetComputerIP(lockObj.GetComputerIP());
						MOG_ControllerSystem.GetCommandManager().CommandProcess(removeLock);
						continue;
					}
				}

				// Manually increment 'l' because the array will collapse around us as we remove locks
				l++;
			}

			// Scan all read locks looking for a matching networkID
			// Manually increment 'l' because the array will collapse around us as we remove locks
			for (int l = 0; l < mActiveReadLocks.Count; )
			{
				MOG_Command lockObj = (MOG_Command)(mActiveReadLocks[l]);

				// Do we have a networkID to compare and does it match this lock's NetworkID?
				if (networkID != 0 && networkID == lockObj.GetNetworkID())
				{
					// Do not kill persistent locks, they are set by users working on assets
					if (!lockObj.IsPersistentLock())
					{
						MOG_Command removeLock = new MOG_Command();

						// Manually construct removeCommand from the lock information
						removeLock.SetCommandType(MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_LockReadRelease);
						removeLock.SetAssetFilename(lockObj.GetAssetFilename().GetOriginalFilename());
						removeLock.SetUserName(lockObj.GetUserName());
						removeLock.SetComputerName(lockObj.GetComputerName());
						removeLock.SetNetworkID(lockObj.GetNetworkID());
						removeLock.SetComputerIP(lockObj.GetComputerIP());
						MOG_ControllerSystem.GetCommandManager().CommandProcess(removeLock);
						continue;
					}
				}

				// Manually increment 'l' because the array will collapse around us as we remove locks
				l++;
			}

			// Eat the command
			return true;
		}

	// Command Routines
		protected override bool Command_ConnectionKill(MOG_Command pCommand)
		{
			// Try to be civilized and inform the connection they are getting killed
			SendToConnection(pCommand.GetNetworkID(), pCommand);

			//	// However, we can't rely on them to sever the connection because they might be in a hung state
			//	// Remove this connection from our side
			//	MOG_NetworkServer pServer = (MOG_NetworkServer)(mNetwork);
			//	if (pServer)
			//	{
			//		// Close our connection to them
			//		pServer.CloseConnection(pCommand.GetNetworkID());
			//	}

			// Always eat the command
			return true;
		}

		protected virtual bool Command_RegisterClient(MOG_Command pCommand)
		{
			// Add it to mRegisteredComputers
			MOG_Command command = new MOG_Command();
			command.Clone(pCommand);
			mRegisteredClients.Add(command);

			// Add it to mActiveViews
			MOG_Command pView = new MOG_Command();
			pView.Clone(pCommand);
			pView.SetCommandType(MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_ActiveViews);
			mActiveViews.Add(pView);

			// This is one of the commands that the JobManager wants to know about
			mServerJobManager.Command_RegisterClient(pCommand);

			// Check if this connection has a LockTracker?
			if (pCommand.GetOptions().Contains("{MOG_LockTracker}"))
			{
				// Send out all the persistentLocks
				SendPersistentLocksToConnection(pCommand.GetNetworkID());
			}

			// Send out needed Notify command
			MOG_Command pNotify = MOG_CommandFactory.Setup_NotifyActiveConnection(pCommand);
			SendToActiveTab("Connections", pNotify);

			// Eat the command
			return true;
		}

		protected virtual bool Command_ShutdownClient(MOG_Command pCommand)
		{
			// Remove from mActiveViews
			for (int v = 0; v < mActiveViews.Count; v++)
			{
				// Does this NetworkID match?
				MOG_Command view = (MOG_Command)(mActiveViews[v]);
				if (view.GetNetworkID() == pCommand.GetNetworkID())
				{
					// Remove this view
					mActiveViews.RemoveAt(v);
					break;
				}
			}

			// Scan mRegisteredClients looking for this one
			for (int c = 0; c < mRegisteredClients.Count; c++)
			{
				// Does this match the computer's NetworkID?
				MOG_Command client = (MOG_Command)(mRegisteredClients[c]);
				if (pCommand.GetNetworkID() == client.GetNetworkID())
				{
					// Cleanup any locks being left behind
					DeleteLocks(client.GetNetworkID(), client.GetComputerName());

					// Send the shutdown command to the client
					SendToConnection(pCommand.GetNetworkID(), pCommand);

					// Shutdown all the slaves running on this machine
					ShutdownAllSlavesRunningOnComputer(pCommand.GetComputerName());

					// This is one of the commands that the JobManager wants to know about
					mServerJobManager.Command_ShutdownClient(pCommand);

					// Remove it from mRegisteredClients
					mRegisteredClients.RemoveAt(c);

					// Send out needed Notify command
					MOG_Command pNotify = MOG_CommandFactory.Setup_NotifyActiveConnection(pCommand);
					SendToActiveTab("Connections", pNotify);

					break;
				}
			}

			// Always eat the command
			return true;
		}
	
		protected virtual bool Command_RegisterEditor(MOG_Command pCommand)
		{
			// Add it to mRegisteredEditors
			MOG_Command command = new MOG_Command();
			command.Clone(pCommand);
			mRegisteredEditors.Add(command);

			// Notify everyone on this computer that a new Editor just launched
			SendToComputerName(pCommand.GetComputerName(), pCommand);

			// Check if this connection has a LockTracker?
			if (pCommand.GetOptions().Contains("{MOG_LockTracker}"))
			{
				// Send out all the persistentLocks
				SendPersistentLocksToConnection(pCommand.GetNetworkID());
			}

			// Send out needed Notify command
			MOG_Command pNotify = MOG_CommandFactory.Setup_NotifyActiveConnection(pCommand);
			SendToActiveTab("Connections", pNotify);

			// Eat the command
			return true;
		}

		protected virtual bool Command_ShutdownEditor(MOG_Command pCommand)
		{
			// Check if this connection was an Editor
			// Scan mRegisteredEditors looking for this one
			for (int c = 0; c < mRegisteredEditors.Count; c++)
			{
				// Does this match the computer's NetworkID?
				MOG_Command editor = (MOG_Command)(mRegisteredEditors[c]);
				if (pCommand.GetNetworkID() == editor.GetNetworkID())
				{
					// Propagate the editor's WorkingDirectory into the shutdown command so clients can determine which editor is shutting down
					pCommand.SetWorkingDirectory(editor.GetWorkingDirectory());

					// Notify everyone on this computer that a new Editor just shutdown
					SendToComputerName(pCommand.GetComputerName(), pCommand);

					// Cleanup any locks being left behind
					DeleteLocks(editor.GetNetworkID(), editor.GetComputerName());

					// Send the shutdown command to the Editor
					SendToConnection(editor.GetNetworkID(), pCommand);

					// Remove it from mRegisteredEditors
					mRegisteredEditors.RemoveAt(c);

					// Locate the client on this machine
					//? MOG_CommandServer.Command_ShutdownEditor - We really need to match editors that are running from within the active local data directory in case there are multiple editors and clients running on the machine.
					MOG_Command pClient = LocateClientByComputerName(pCommand.GetComputerName());
					if (pClient != null)
					{
						// Send this ShutdownEditor command to the Client
						SendToConnection(pClient.GetNetworkID(), pCommand);
					}

					// Send out needed Notify command
					MOG_Command pNotify = MOG_CommandFactory.Setup_NotifyActiveConnection(pCommand);
					SendToActiveTab("Connections", pNotify);

					break;
				}
			}

			// Eat the command
			return true;
		}

		protected virtual bool Command_RegisterCommandLine(MOG_Command pCommand)
		{
			// Add it to mRegisteredCommandLines
			MOG_Command command = new MOG_Command();
			command.Clone(pCommand);
			mRegisteredCommandLines.Add(command);

			// Locate the client on this machine
			MOG_Command pClient = LocateClientByComputerName(pCommand.GetComputerName());
			if (pClient != null)
			{
				// Send this RegisterClient command to the CommandLine
				SendToConnection(pCommand.GetNetworkID(), pClient);

				// Locate the client's active views on this machine
				MOG_Command pView = LocateActiveViewByID(pClient.GetNetworkID());
				if (pView != null)
				{
					// Send this RegisterClient command to the CommandLine
					SendToConnection(pCommand.GetNetworkID(), pView);
				}
			}

			// Send out needed Notify command
			MOG_Command pNotify = MOG_CommandFactory.Setup_NotifyActiveConnection(pCommand);
			SendToActiveTab("Connections", pNotify);

			// Eat the command
			return true;
		}

		protected virtual bool Command_ShutdownCommandLine(MOG_Command pCommand)
		{
			// Check if this connection was a CommandLine
			// Scan mRegisteredCommandLines looking for this one
			for (int c = 0; c < mRegisteredCommandLines.Count; c++)
			{
				// Does this match the computer's NetworkID?
				MOG_Command commandLine = (MOG_Command)(mRegisteredCommandLines[c]);
				if (commandLine != null)
				{
					if (pCommand.GetNetworkID() == commandLine.GetNetworkID())
					{
						// Cleanup any locks being left behind
						DeleteLocks(commandLine.GetNetworkID(), commandLine.GetComputerName());

						// Send the shutdown command to the CommandLine
						SendToConnection(pCommand.GetNetworkID(), pCommand);

						// Remove it from mRegisteredCommandLines
						mRegisteredCommandLines.RemoveAt(c);

						break;
					}
				}
			}

			// Send out needed Notify command
			MOG_Command pNotify = MOG_CommandFactory.Setup_NotifyActiveConnection(pCommand);
			SendToActiveTab("Connections", pNotify);

			// Eat the command
			return true;
		}

		protected virtual bool Command_RegisterSlave(MOG_Command pCommand)
		{
			// Add it to mRegisteredSlaves...
			// Don't clone the command so that we can have both lists get updated automatically when assigning slave tasks
			mRegisteredSlaves.Add(pCommand);
			// Add this slave in our JobManager
			mServerJobManager.AddSlave(pCommand);

			// This is one of the commands that the JobManager wants to know about
			mServerJobManager.Command_RegisterSlave(pCommand);

			// Send out needed Notify command
			MOG_Command pNotify = MOG_CommandFactory.Setup_NotifyActiveConnection(pCommand);
			SendToActiveTab("Connections", pNotify);

			// Eat the command
			return true;
		}

		protected virtual bool Command_ShutdownSlave(MOG_Command pCommand)
		{
			MOG_Command pSlave = null;
			MOG_Command pClient = null;

			// Check if this was a client that sent us this shutdown slave command?
			pClient = LocateClientByID(pCommand.GetNetworkID());
			if (pClient != null)
			{
				// Check if we have a slave running on this machine?
				pSlave = LocateRegisteredSlaveByComputerName(pClient.GetComputerName());
				if (pSlave != null)
				{
					// Make sure to update the command to reflect the proper NetworkID since this originated from a Client
					pCommand.SetNetworkID(pSlave.GetNetworkID());
				}
			}
			else
			{
				// Check if this networkID matches with a registered slave?
				pSlave = LocateRegisteredSlaveByID(pCommand.GetNetworkID());
				if (pSlave != null)
				{
					// Attempt to locate a client running on this slave's machine?
					pClient = LocateClientByComputerName(pSlave.GetComputerName());
				}
			}

			// Check if we were able to locate a slave to shutdown?
			if (pSlave != null)
			{
				// Check if we have a client that we should tell about this slave termination?
				if (pClient != null)
				{
					// Inform the client that we're killing this slave.
					SendToConnection(pClient.GetNetworkID(), pCommand);
				}

				// Cleanup any locks being left behind
				DeleteLocks(pSlave.GetNetworkID(), "");

				// Send the shutdown command to the connection
				SendToConnection(pSlave.GetNetworkID(), pCommand);

				// Remove this slave from our registered slaves
				mRegisteredSlaves.Remove(pSlave);
				// Remove this slave from our JobManager
				mServerJobManager.RemoveSlave(pSlave);

				// Send out needed Notify command
				MOG_Command pNotify = MOG_CommandFactory.Setup_NotifyActiveConnection(pCommand);
				SendToActiveTab("Connections", pNotify);
			}

			// Always eat the command
			return true;
		}

		protected override bool Command_SQLConnection(MOG_Command pCommand)
		{
			// Save new Repository info to the Server's local ini
			string localConfigFilename = string.Concat(MOG_Main.GetExecutablePath(), "\\MOG.ini");
			MOG_Ini localConfigFile = new MOG_Ini();
			if (localConfigFile.Open(localConfigFilename, FileShare.ReadWrite))
			{
				localConfigFile.PutString("SQL", "Connectionstring", pCommand.GetDescription());
				localConfigFile.Close();

				// Initialize our Database
				MOG_ControllerSystem.InitializeDatabase(pCommand.GetDescription(), "", "");

				// Inform all connections about the change
				SendToAllConnections(pCommand);
			}
			else
			{
				// Inform user we failed
			}

			// Always eat the command
			return true;
		}

		protected override bool Command_MOGRepository(MOG_Command pCommand)
		{
			string localConfigFilename = string.Concat(MOG_Main.GetExecutablePath(), "\\MOG.ini");
			string networkConfigFilename = string.Concat(pCommand.GetWorkingDirectory(), "\\", MOG_ControllerSystem.GetMogSystemRelativeConfig());

			// First validate the specified repository...
			if (DosUtils.FileExistFast(networkConfigFilename))
			{
				// Fixup the SystemRepositoryPath in this ini
				MOG_Ini localConfigFile = new MOG_Ini();
				if (localConfigFile.Open(localConfigFilename, FileShare.ReadWrite))
				{
					localConfigFile.PutString("MOG", "SystemRepositoryPath", pCommand.GetWorkingDirectory());
					localConfigFile.Close();

					// Fixup our loader's ini as well...if it exists
					string loaderFilename = string.Concat(pCommand.GetWorkingDirectory(), "\\..\\Loader.ini");
					if (DosUtils.FileExistFast(loaderFilename))
					{
						// Fixup the SystemRepositoryPath in this ini
						MOG_Ini loaderFile = new MOG_Ini();
						if (loaderFile.Open(loaderFilename, FileShare.ReadWrite))
						{
							loaderFile.PutString("MOG", "SystemRepositoryPath", pCommand.GetWorkingDirectory());
							loaderFile.Close();
						}
					}

					try
					{
						// Attempt to fixup the MOGRepository files
						string rootRepositoryFilename = string.Concat(pCommand.GetWorkingDirectory(), "\\MOGRepository.ini");
						string rootDriveFilename = string.Concat(DosUtils.PathGetRootPath(pCommand.GetWorkingDirectory()), "MOGRepository.ini");
						if (DosUtils.FileExistFast(rootRepositoryFilename))
						{
							// Open the MOGRepositories file in the root of our repository
							MOG_Ini repositoryFile = new MOG_Ini();
							if (repositoryFile.Open(rootRepositoryFilename, FileShare.ReadWrite))
							{
								string repositoryName = "";

								// Make sure we have a repository name?
								if (repositoryFile.CountKeys("MOG_REPOSITORIES") == 1)
								{
									// Get the name of the repository
									repositoryName = repositoryFile.GetKeyNameByIndexSLOW("MOG_REPOSITORIES", 0);
									// Update the SystemRepositoryPath for this repository
									repositoryFile.PutString(repositoryName, "SystemRepositoryPath", pCommand.GetWorkingDirectory());
								}
								repositoryFile.Close();

								// Check if we were able to obtain our repository name?
								if (repositoryName.Length > 0)
								{
									// Open the MOGRepositories file in the root of our drive
									repositoryFile = new MOG_Ini();
									if (repositoryFile.Open(rootDriveFilename, FileShare.ReadWrite))
									{
										// Update the SystemRepositoryPath for our repository
										repositoryFile.PutString(repositoryName, "SystemRepositoryPath", pCommand.GetWorkingDirectory());
										repositoryFile.Close();
									}
								}
							}
						}
					}
					catch
					{
						// Catch because we might throw if the user does not have full access to the root of the drive
					}

					// Fixup the SystemRepositoryPath in this ini
					MOG_Ini networkConfigFile = new MOG_Ini();
					if (networkConfigFile.Open(networkConfigFilename, FileShare.ReadWrite))
					{
						networkConfigFile.PutString("MOG", "SystemRepositoryPath", pCommand.GetWorkingDirectory());
						networkConfigFile.Close();

						// Inform all connections about the change
						SendToAllConnections(pCommand);

						// Reload the Server's config information
						MOG_ControllerSystem.GetSystem().Load(MOG_ControllerSystem.GetSystem().GetConfigFilename());
						// Refresh our system and project
						string projectName = MOG_ControllerProject.GetProjectName();
						string branchName = MOG_ControllerProject.GetBranchName();
						string userName = MOG_ControllerProject.GetUserName();
						if (projectName.Length > 0)
						{
							// Simply login in again and it will refresh everything
							MOG_ControllerProject.LoginProject(projectName, branchName);
							MOG_ControllerProject.LoginUser(userName);
						}

						// Always eat the command
						return true;
					}
				}

				// Inform user we failed
			}
			else
			{
				// Inform the user this appears to be a bogus repository
			}

			// Always eat the command
			return true;
		}

		protected virtual bool Command_LoginProject(MOG_Command pCommand)
		{
			// Scan mRegisteredClients looking for this registered computer
			for (int c = 0; c < mRegisteredClients.Count; c++)
			{
				MOG_Command client = (MOG_Command)(mRegisteredClients[c]);

				// Does this NetworkID match?
				if (client.GetNetworkID() == pCommand.GetNetworkID())
				{
					// Set the login project in this mRegisteredClients command
					client.SetProject(pCommand.GetProject());
					// Set the login project in this mRegisteredClients command
					client.SetBranch(pCommand.GetBranch());

					// Send out needed Notify command
					MOG_Command pNotify = MOG_CommandFactory.Setup_NotifyActiveConnection(pCommand);
					SendToActiveTab("Connections", pNotify);

					break;
				}
			}

			// Eat the command
			return true;
		}

		protected virtual bool Command_LoginUser(MOG_Command pCommand)
		{
			// Scan mRegisteredClients looking for this registered computer
			for (int c = 0; c < mRegisteredClients.Count; c++)
			{
				MOG_Command client = (MOG_Command)(mRegisteredClients[c]);

				// Does this NetworkID match?
				if (client.GetNetworkID() == pCommand.GetNetworkID())
				{
					// Set the login user in this mRegisteredClients command
					client.SetUserName(pCommand.GetUserName());

					// Send out needed Notify command
					MOG_Command pNotify = MOG_CommandFactory.Setup_NotifyActiveConnection(pCommand);
					SendToActiveTab("Connections", pNotify);

					break;
				}
			}

			// Eat the command
			return true;
		}

		protected override bool Command_ActiveViews(MOG_Command pCommand)
		{
			// Locate the associated View
			MOG_Command view = LocateActiveViewByID(pCommand.GetNetworkID());
			if (view != null)
			{
				// Set the specified views in this mActiveViews command
				if (pCommand.GetProject().Length > 0)
				{
					view.SetProject(pCommand.GetProject());
				}
				if (pCommand.GetTab().Length > 0)
				{
					view.SetTab(pCommand.GetTab());
				}
				if (pCommand.GetUserName().Length > 0)
				{
					view.SetUserName(pCommand.GetUserName());
				}
				if (pCommand.GetPlatform().Length > 0)
				{
					view.SetPlatform(pCommand.GetPlatform());
				}
				if (pCommand.GetWorkingDirectory().Length > 0)
				{
					view.SetWorkingDirectory(pCommand.GetWorkingDirectory());
				}

				// Send out needed Notify command
				MOG_Command pNotify = MOG_CommandFactory.Setup_NotifyActiveConnection(view);
				SendToActiveTab("Connections", pNotify);
			}

			return true;
		}

		protected override bool Command_ViewUpdate(MOG_Command pCommand)
		{
			HybridDictionary connections = new HybridDictionary();
			MOG_Filename sourceFilename = new MOG_Filename(pCommand.GetSource());
			MOG_Filename targetFilename = new MOG_Filename(pCommand.GetDestination());
			string user = pCommand.GetUserName();

			// Check if this view command originated from a client?
			MOG_Command pOriginatingClient = LocateClientByID(pCommand.GetNetworkID());
			if (pOriginatingClient != null)
			{
				// Always send it back to the client who sent it
				connections[pOriginatingClient.GetNetworkID()] = pOriginatingClient;
			}

			// Scan the editors
			foreach(MOG_Command editor in mRegisteredEditors)
			{
				// Check if this Editor is running on the same machine?  and
				if (string.Compare(editor.GetComputerName(), pCommand.GetComputerName(), true) == 0)
				{
					// Check if this Editor is logged into the same project?  and
					// Check if this Editor is logged in as the same user?
					if (string.Compare(editor.GetProject(), pCommand.GetProject(), true) == 0 &&
						string.Compare(editor.GetUserName(), pCommand.GetUserName(), true) == 0)
					{
						// Check if this Editor is associated with this source or target filename?
						string testWorkspace = editor.GetWorkingDirectory() + "\\";
						string testSource = sourceFilename.GetOriginalFilename() + "\\";
						string testTarget = targetFilename.GetOriginalFilename() + "\\";
						if (testSource.StartsWith(testWorkspace, StringComparison.CurrentCultureIgnoreCase) ||
							testTarget.StartsWith(testWorkspace, StringComparison.CurrentCultureIgnoreCase))
						{
							// Add this editor so it will be notified
							connections[editor.GetNetworkID()] = editor;
						}
					}
				}
			}

			// Walk through all the known views to see who should be notified?
			foreach(MOG_Command view in mActiveViews)
			{
				// Make sure the project matches?
				if (string.Compare(view.GetProject(), pCommand.GetProject(), true) == 0)
				{
					// Check if this view is sitting on the workspace tab?
					if (string.Compare(view.GetTab(), "Workspace", true) == 0)
					{
						// Check if this view has anything to do with this user?
						if (string.Compare(view.GetUserName(), sourceFilename.GetUserName(), true) == 0 ||
							string.Compare(view.GetUserName(), targetFilename.GetUserName(), true) == 0)
						{
							// Add this connection so it will be notified
							connections[view.GetNetworkID()] = view;
						}
					}
					// Check if this view is sitting on the project or library tab?
					else if (string.Compare(view.GetTab(), "Project", true) == 0 ||
							 string.Compare(view.GetTab(), "Library", true) == 0)
					{
						// Check if this effected the Repository?
						if (sourceFilename.IsWithinRepository() ||
							targetFilename.IsWithinRepository())
						{
							// Add this connection so it will be notified
							connections[view.GetNetworkID()] = view;
						}
					}

					// Locate the client associated with this view
					MOG_Command viewClient = LocateClientByID(view.GetNetworkID());
					if (viewClient != null)
					{
						// Check if this client has anything to do with this user?
						if (string.Compare(viewClient.GetUserName(), pCommand.GetUserName(), true) == 0 ||
							string.Compare(viewClient.GetUserName(), sourceFilename.GetUserName(), true) == 0 ||
							string.Compare(viewClient.GetUserName(), targetFilename.GetUserName(), true) == 0)
						{
							// Add this client so it will be notified
							connections[viewClient.GetNetworkID()] = viewClient;
						}
					}
				}
			}

			// Scan all identified connections
			foreach (MOG_Command connection in connections.Values)
			{
				// Send the command down to each connection
				SendToConnection(connection.GetNetworkID(), pCommand);
			}

			// Always eat this command
			return true;
		}

		protected override bool Command_RefreshApplication(MOG_Command pCommand)
		{
			// Inform all connections
			SendToAllConnections(pCommand);

			// Always eat this command
			return true;
		}

		protected override bool Command_RefreshTools(MOG_Command pCommand)
		{
			// Inform all connections
			SendToAllConnections(pCommand);

			// Always eat this command
			return true;
		}

		protected override bool Command_RefreshProject(MOG_Command pCommand)
		{
			// Inform all connections
			SendToProject(pCommand.GetProject(), pCommand);

			// Always eat this command
			return true;
		}
				
		protected bool Command_AssetRipRequest(MOG_Command pCommand)
		{
			// Send this command to a slave
			return mServerJobManager.AssignToSlave(pCommand);
		}

		protected bool Command_AssetProcessed(MOG_Command pCommand)
		{
			// Send this command to a slave
			return mServerJobManager.AssignToSlave(pCommand);
		}

		protected bool Command_SlaveTask(MOG_Command pCommand)
		{
			// Send this command to a slave
			return mServerJobManager.AssignToSlave(pCommand);
		}

		protected bool Command_ReinstanceAssetRevision(MOG_Command pCommand)
		{
			// Send this command to a slave
			return mServerJobManager.AssignToSlave(pCommand);
		}

		protected bool Command_Bless(MOG_Command pCommand)
		{
			// Send this command to a slave
			return mServerJobManager.AssignToSlave(pCommand);
		}

		protected bool Command_RemoveAssetFromProject(MOG_Command pCommand)
		{
			// Send this command to a slave
			return mServerJobManager.AssignToSlave(pCommand);
		}

		protected bool Command_NetworkPackageMerge(MOG_Command pCommand)
		{
			// Attempt to assign it to an available slave
			return mServerJobManager.AssignToSlave(pCommand);
		}

		protected bool Command_LocalPackageMerge(MOG_Command pCommand)
		{
			// Send this package command to the proper component for processing?
			return SendPackageCommandToProperComponent(pCommand);
		}

		protected bool Command_PackageRebuild(MOG_Command pCommand)
		{
			// Attempt to assign it to an available slave
			return mServerJobManager.AssignToSlave(pCommand);
		}

		protected bool Command_Post(MOG_Command pCommand)
		{
			// Send this command to a slave
			return mServerJobManager.AssignToSlave(pCommand);
		}

		protected bool Command_Archive(MOG_Command pCommand)
		{
			// Send this command to a slave
			return mServerJobManager.AssignToSlave(pCommand);
		}

		protected bool Command_ScheduleArchive(MOG_Command pCommand)
		{
			// Send this command to a slave
			return mServerJobManager.AssignToSlave(pCommand);
		}

		protected override bool Command_LockReadRequest(MOG_Command pCommand)
		{
			bool bDuplicatedLock = IsLockDuplicated(pCommand);

			// Request the lock
			if (bDuplicatedLock || LockRequest(pCommand))
			{
				// Make sure to indicate this was processed by setting the completed member variable
				pCommand.SetCompleted(true);

				// Never duplicate any locks on the server's side
				if (!bDuplicatedLock)
				{
					// Add the lock to our array
					mActiveReadLocks.Add(pCommand);

					// Make sure we can log into the DataBase
					if (MOG_ControllerSystem.GetDB() != null)
					{
						// Add the lock to the database
						if (MOG_DBCommandAPI.AddLock(pCommand))
						{
						}
					}

					// Do not kill persistent locks, they are set by users working on assets
					if (pCommand.IsPersistentLock())
					{
						// Inform all Clients about this lock
						MOG_Command pNotify = MOG_CommandFactory.Setup_LockPersistentNotify(pCommand, "{Processed}");
						SendPersistentLockToAllClients(pNotify);
						SendPersistentLockToAllEditors(pNotify);
					}
				}
			}

			// Send out needed Notify command
			MOG_Command pNotifyLock = MOG_CommandFactory.Setup_NotifyActiveLock(pCommand);
			SendToActiveTab("Locks", pNotifyLock);

			// Indicate whether this lock was completed
			return pCommand.IsCompleted();
		}

		protected override bool Command_LockReadRelease(MOG_Command pCommand)
		{
			// Scan all read locks
			for (int l = 0; l < mActiveReadLocks.Count; l++)
			{
				MOG_Command lockObj = (MOG_Command)(mActiveReadLocks[l]);

				// Does this match the ComputerName, UserName and AssetName?  (We need to track ComputerName and UserName because there can be mutiple Read locks active at the same time)
				if (string.Compare(pCommand.GetComputerName(), lockObj.GetComputerName(), true) == 0 &&
					string.Compare(pCommand.GetUserName(), lockObj.GetUserName(), true) == 0 &&
					string.Compare(pCommand.GetAssetFilename().GetOriginalFilename(), lockObj.GetAssetFilename().GetOriginalFilename(), true) == 0)
				{
					// Make sure to indicate this was processed by setting the completed member variable
					pCommand.SetCompleted(true);
					// Remove it from mRegisteredComputers
					mActiveReadLocks.RemoveAt(l);

					// Make sure we can log into the DataBase
					if (MOG_ControllerSystem.GetDB() != null)
					{
						// Remove the lock from the database
						if (MOG_DBCommandAPI.RemoveLock(lockObj))
						{
						}
					}

					// Encapsulate the original lock command within the lock release command
					pCommand.SetCommand(lockObj);

					// Do not kill persistent locks, they are set by users working on assets
					if (lockObj.IsPersistentLock())
					{
						// Inform all Clients about this lock
						MOG_Command pNotify = MOG_CommandFactory.Setup_LockPersistentNotify(pCommand, "{Processed}");
						SendPersistentLockToAllClients(pNotify);
						SendPersistentLockToAllEditors(pNotify);
					}
					break;
				}
			}

			// Send out needed Notify command
			MOG_Command pNotifyLock = MOG_CommandFactory.Setup_NotifyActiveLock(pCommand);
			SendToActiveTab("Locks", pNotifyLock);

			// Always return true or else the lock release could be sitting here forever
			pCommand.SetCompleted(true);
			return pCommand.IsCompleted();
		}

		protected override bool Command_LockWriteRequest(MOG_Command pCommand)
		{
			bool bDuplicatedLock = IsLockDuplicated(pCommand);

			// Request the lock
			if (bDuplicatedLock || LockRequest(pCommand))
			{
				// Make sure to indicate this was processed by setting the completed member variable
				pCommand.SetCompleted(true);

				// Never duplicate any locks on the server's side
				if (!bDuplicatedLock)
				{
					// Add the lock to our array
					mActiveWriteLocks.Add(pCommand);

					// Make sure we can log into the DataBase
					if (MOG_ControllerSystem.GetDB() != null)
					{
						// Add the lock from the database
						if (MOG_DBCommandAPI.AddLock(pCommand))
						{
						}
					}

					// Do not kill persistent locks, they are set by users working on assets
					if (pCommand.IsPersistentLock())
					{
						// Inform all Clients about this lock
						MOG_Command pNotify = MOG_CommandFactory.Setup_LockPersistentNotify(pCommand, "{Processed}");
						SendPersistentLockToAllClients(pNotify);
						SendPersistentLockToAllEditors(pNotify);
					}
				}
			}

			// Send out needed Notify command
			MOG_Command pNotifyLock = MOG_CommandFactory.Setup_NotifyActiveLock(pCommand);
			SendToActiveTab("Locks", pNotifyLock);

			// Indicate whether this lock was completed
			return pCommand.IsCompleted();
		}

		protected override bool Command_LockWriteRelease(MOG_Command pCommand)
		{
			// Scan all write locks
			for (int l = 0; l < mActiveWriteLocks.Count; l++)
			{
				MOG_Command lockObj = (MOG_Command)(mActiveWriteLocks[l]);

// This is overkill now that we are only using locks as persistent locks...Just allow the lock to be released if the AssetName matches
//				// Does this match the ComputerName, UserName and AssetName?
//				if (string.Compare(pCommand.GetComputerName(), lockObj.GetComputerName(), true) == 0 &&
//					string.Compare(pCommand.GetUserName(), lockObj.GetUserName(), true) == 0 &&
//					string.Compare(pCommand.GetAssetFilename().GetOriginalFilename(), lockObj.GetAssetFilename().GetOriginalFilename(), true) == 0)
				// Does this match the AssetName?
				if (string.Compare(pCommand.GetAssetFilename().GetOriginalFilename(), lockObj.GetAssetFilename().GetOriginalFilename(), true) == 0)
				{
					// Make sure to indicate this was processed by setting the completed member variable
					pCommand.SetCompleted(true);
					// Remove it from mRegisteredComputers
					mActiveWriteLocks.RemoveAt(l);

					// Make sure we can log into the DataBase
					if (MOG_ControllerSystem.GetDB() != null)
					{
						// Remove the lock from the database
						if (MOG_DBCommandAPI.RemoveLock(lockObj))
						{
						}
					}

					// Encapsulate the original lock command within the lock release command
					pCommand.SetCommand(lockObj);

					// Do not kill persistent locks, they are set by users working on assets
					if (lockObj.IsPersistentLock())
					{
						// Inform all Clients about this lock
						MOG_Command pNotify = MOG_CommandFactory.Setup_LockPersistentNotify(pCommand, "{Processed}");
						SendPersistentLockToAllClients(pNotify);
						SendPersistentLockToAllEditors(pNotify);
					}
					break;
				}
			}

			// Send out needed Notify command
			MOG_Command pNotifyLock = MOG_CommandFactory.Setup_NotifyActiveLock(pCommand);
			SendToActiveTab("Locks", pNotifyLock);

			// Always return true or else the lock release could be sitting here forever
			pCommand.SetCompleted(true);
			return pCommand.IsCompleted();
		}

		protected override bool Command_LockQuery(MOG_Command pCommand)
		{
			// Don't bother to request a duplicate lock
			if (!IsLockDuplicated(pCommand))
			{
				LockRequest(pCommand);
			}

			// Indicate whether this lock query was completed
			return true;
		}

		protected bool Command_GetEditorInfo(MOG_Command pCommand)
		{
			// Check if there is an Editor running?
			MOG_Command pEditor = LocateAssociatedEditorByWorkspace(pCommand.GetComputerName(), pCommand.GetWorkingDirectory());
			if (pEditor != null)
			{
				// Return the Registered Editor Info
				pCommand.SetCommand(pEditor);

				return true;
			}

			return false;
		}

		protected bool Command_NetworkBroadcast(MOG_Command pCommand)
		{
			// Check if there were users specified?
			if (pCommand.GetOptions().Length > 0)
			{
				// Send the message to this registered client
				SendToUsers(pCommand.GetOptions(), pCommand);
			}
			else
			{
				// Send the message to the entire project
				SendToProject(pCommand.GetProject(), pCommand);
			}

			// Indicate that we received and processed the command
			return true;
		}

		protected bool Command_BuildFull(MOG_Command pCommand)
		{
			// Check if this isn't a forced build?   Description will be "Force"
			if (pCommand.GetDescription().Length == 0)
			{
				// Check if a build is already running
				if (false)
				{
					// Report a message back to the user indicating a build is already in progress

					// Bail out on this command
					// return false;
				}
			}
			// Send this command to a slave
			return mServerJobManager.AssignToSlave(pCommand);
		}

		protected bool Command_Build(MOG_Command pCommand)
		{
			// Check if this isn't a forced build?   Description will be "Force"
			if (pCommand.GetDescription().Length == 0)
			{
				// Check if a build is already running
				if (false)
				{
					// Report a message back to the user indicating a build is already in progress

					// Bail out on this command
					// return false;
				}
			}
			// Send this command to a slave

			return mServerJobManager.AssignToSlave(pCommand);
		}

		protected bool Command_InstantMessage(MOG_Command pCommand)
		{
			string invitedUsers;
			string users = "";

			// Get the list of invited users
			invitedUsers = pCommand.GetSource();
			// Process each listed user
			while (invitedUsers.Length > 0)
			{
				string user;

				// Check to see if there are any more users listed
				int pos = invitedUsers.IndexOf(",");
				if (pos != -1)
				{
					// Strip out the current user
					user = invitedUsers.Substring(0, pos);
					// Remove the current user from the completion flow string
					invitedUsers = invitedUsers.Substring(pos + 1);
				}
				else
				{
					// Looks like there is only one user listed
					user = invitedUsers;
					invitedUsers = "";
				}

				// Clean up the user string...remove any spaces
				user = user.Replace(" ", "");

				// Make sure we founbd a userName?
				if (user.Length > 0)
				{
					// Build the users list
					if (users.Length > 0)
					{
						// Append this user onto the end of other users
						users = string.Concat(users, ", ", user);
					}
					else
					{
						// This is the first user
						users = user;
					}
				}
			}

			// Specify who in the InvitedUser list will be served this post
			pCommand.SetDestination(users);
			// Send instant message onto all registered Clients of this user
			SendToUsers(users, pCommand);

			// Always eat this command
			return true;
		}

		protected override bool Command_Complete(MOG_Command pCommand)
		{
			// Make sure this contains an encapsulated command?
			if (pCommand.GetCommand() != null)
			{
				// Determin the type of encapsulated command
				switch (pCommand.GetCommand().GetCommandType())
				{
					case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_Post:
						// Check if this is the final Post w/o any assetFilename being specified?
						if (pCommand.GetCommand().GetAssetFilename().GetOriginalFilename().Length == 0)
						{
							// Release this slave
							mServerJobManager.ReleaseSlave(pCommand.GetCommand());
						}
						// Notify all users of this Post being completed
						SendToProject(pCommand.GetProject(), pCommand);
						break;

					case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_RemoveAssetFromProject:
						// Release this slave
						mServerJobManager.ReleaseSlave(pCommand.GetCommand());
						// Notify all users of this RemoveAssetFromProject being completed
						SendToProject(pCommand.GetProject(), pCommand);
						break;

					case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_NetworkPackageMerge:
					case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_NetworkPackageRebuild:
						// Release this slave
						mServerJobManager.ReleaseSlave(pCommand.GetCommand());
						// Send this package command to the proper component for processing?
						SendPackageCommandToProperComponent(pCommand);
						break;
					case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_LocalPackageMerge:
					case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_EditorPackageMergeTask:
						// Send this package command to the proper component for processing?
						SendPackageCommandToProperComponent(pCommand);
						break;

					case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_SlaveTask:
					case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_AssetRipRequest:
					case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_AssetProcessed:
					case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_ReinstanceAssetRevision:
					case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_Bless:
					case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_Archive:
					case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_ScheduleArchive:
					case MOG.COMMAND.MOG_COMMAND_TYPE.MOG_COMMAND_Build:
						// Release this slave
						mServerJobManager.ReleaseSlave(pCommand.GetCommand());
						// Send the command back to the originating client
						SendToConnection(pCommand.GetCommand().GetNetworkID(), pCommand);
						break;

					default:
						break;
				}
			}

			// Always eat this command
			return true;
		}

		protected override bool Command_NotifySystemAlert(MOG_Command pCommand)
		{
			// Check if this command should be sent back to the user by checking the Options?
			if (string.Compare(pCommand.GetOptions(), "NotifyUser=True", true) == 0)
			{
				// Make sure we have a usere name specified?
				if (pCommand.GetUserName().Length > 0)
				{
					// Send this back to the originating user
					if (SendToUsers(pCommand.GetUserName(), pCommand))
					{
						// Only eat this command if were able to send the message
						return true;
					}

					// Save the message for when they do log back into the system
					return false;
				}
				else
				{
					// Send the message to the entire project
					SendToProject(pCommand.GetProject(), pCommand);
				}
			}

			// Always eat this command
			return true;
		}

		protected override bool Command_NotifySystemError(MOG_Command pCommand)
		{
			// Check if this command should be sent back to the user by checking the Options?
			if (string.Compare(pCommand.GetOptions(), "NotifyUser=True", true) == 0)
			{
				// Make sure we have a usere name specified?
				if (pCommand.GetUserName().Length > 0)
				{
					// Send this back to the originating user
					if (SendToUsers(pCommand.GetUserName(), pCommand))
					{
						// Only eat this command if were able to send the message
						return true;
					}

					// Save the message for when they do log back into the system
					return false;
				}
				else
				{
					// Send the message to the entire project
					SendToProject(pCommand.GetProject(), pCommand);
				}
			}

			// Always eat this command

			return true;
		}

		protected override bool Command_NotifySystemException(MOG_Command pCommand)
		{
			// Cleanup any potential locks left behind as a result of this exception
			DeleteLocks(pCommand.GetNetworkID(), pCommand.GetComputerName());

			// Check if this command should be sent back to the user by checking the Options?
			if (string.Compare(pCommand.GetOptions(), "NotifyUser=True", true) == 0)
			{
				// Make sure we have a usere name specified?
				if (pCommand.GetUserName().Length > 0)
				{
					// Send this back to the originating user
					if (SendToUsers(pCommand.GetUserName(), pCommand))
					{
						// Only eat this command if were able to send the message
						return true;
					}

					// Save the message for when they do log back into the system
					return false;
				}
			}

			// Always eat this command
			return true;
		}

		protected bool Command_RequestActiveCommands(MOG_Command pCommand)
		{
			bool bFailed = false;

			if (!mServerJobManager.Command_RequestActiveCommands(pCommand))
			{
				bFailed = true;
			}

			// Send off the Complete command
			MOG_Command pComplete = MOG_CommandFactory.Setup_Complete(pCommand, true);
			if (!SendToConnection(pCommand.GetNetworkID(), pComplete))
			{
				bFailed = true;
			}

			// Always eat this command
			if (!bFailed)
			{
				return true;
			}

			return false;
		}

		protected bool Command_RequestActiveLocks(MOG_Command pCommand)
		{
			int c;

			// Loop through mActiveWriteLocks
			for (c = 0; c < mActiveWriteLocks.Count; c++)
			{
				// Send off the Notify command for each one
				MOG_Command pNotify = MOG_CommandFactory.Setup_NotifyActiveLock((MOG_Command)(mActiveWriteLocks[c]));
				SendToConnection(pCommand.GetNetworkID(), pNotify);
			}

			// Loop through mActiveReadLocks
			for (c = 0; c < mActiveReadLocks.Count; c++)
			{
				// Send off the Notify command for each one
				MOG_Command pNotify = MOG_CommandFactory.Setup_NotifyActiveLock((MOG_Command)(mActiveReadLocks[c]));
				SendToConnection(pCommand.GetNetworkID(), pNotify);
			}

			// Send off the Complete command
			MOG_Command pComplete = MOG_CommandFactory.Setup_Complete(pCommand, true);
			SendToConnection(pCommand.GetNetworkID(), pComplete);


			return true;
		}

		protected bool Command_RequestActiveConnections(MOG_Command pCommand)
		{
			int c;

			// Loop through mRegisteredClients...
			for (c = 0; c < mRegisteredClients.Count; c++)
			{
				// Send off the Notify command for each one
				MOG_Command pNotify = MOG_CommandFactory.Setup_NotifyActiveConnection((MOG_Command)(mRegisteredClients[c]));
				SendToConnection(pCommand.GetNetworkID(), pNotify);
			}

			// Loop through mActiveViews...This is the same as mRegisteredClients except it shows what they are doing
			for (c = 0; c < mActiveViews.Count; c++)
			{
				// Send off the Notify command for each one
				MOG_Command pNotify = MOG_CommandFactory.Setup_NotifyActiveConnection((MOG_Command)(mActiveViews[c]));
				SendToConnection(pCommand.GetNetworkID(), pNotify);
			}

			// Loop through mRegisteredEditors
			for (c = 0; c < mRegisteredEditors.Count; c++)
			{
				// Send off the Notify command for each one
				MOG_Command pNotify = MOG_CommandFactory.Setup_NotifyActiveConnection((MOG_Command)(mRegisteredEditors[c]));
				SendToConnection(pCommand.GetNetworkID(), pNotify);
			}

			// Loop through mRegisteredSlaves...
			for (c = 0; c < mRegisteredSlaves.Count; c++)
			{
				// Send off the Notify command for each one
				MOG_Command pNotify = MOG_CommandFactory.Setup_NotifyActiveConnection((MOG_Command)(mRegisteredSlaves[c]));
				SendToConnection(pCommand.GetNetworkID(), pNotify);
			}

			// Loop through mRegisteredCommandLines
			for (c = 0; c < mRegisteredCommandLines.Count; c++)
			{
				// Send off the Notify command for each one
				MOG_Command pNotify = MOG_CommandFactory.Setup_NotifyActiveConnection((MOG_Command)(mRegisteredCommandLines[c]));
				SendToConnection(pCommand.GetNetworkID(), pNotify);
			}

			// Send off the Complete command
			MOG_Command pComplete = MOG_CommandFactory.Setup_Complete(pCommand, true);
			SendToConnection(pCommand.GetNetworkID(), pComplete);

			return true;
		}

		protected bool Command_RetaskCommand(MOG_Command pCommand)
		{
			// Send this request through to the JobManager
			this.mServerJobManager.Command_RetaskCommand(pCommand);

			// Always eat this command
			return true;
		}

		protected bool Command_KillCommand(MOG_Command pCommand)
		{
			// Kill this command from the ServerJobManager
			mServerJobManager.Command_KillCommand(pCommand);

			// Always eat this command
			return true;
		}

		protected override bool Command_LaunchSlave(MOG_Command pCommand)
		{
			SendToConnection(pCommand.GetNetworkID(), pCommand);

			// Always eat this command
			return true;
		}

		protected bool Command_StartJob(MOG_Command pCommand)
		{

			// TODO!!!
			return false;
		}

		public bool InitiateAutomatedTesting(int clientNetworkID, string testName, string projectName, string importFile, int duration, bool bCopyFileLocal, int startingExtensionIndex)
		{
			bool bFailed = false;

			// Make sure we have a valid ClientNetworkID?
			// Make sure we have a valid ProjectName?
			// Make sure we have a valid ImportFile?
			if (clientNetworkID > 1 &&
				projectName.Length > 0 &&
				importFile.Length > 0)
			{
				// Construct the appropriate command
				MOG_Command command = MOG_CommandFactory.Setup_AutomatedTesting(testName, projectName, importFile, duration, bCopyFileLocal, startingExtensionIndex);

				// Send this command to the specified networkID
				if (!SendToConnection(clientNetworkID, command))
				{
					bFailed = true;
				}
			}
			else
			{
				bFailed = true;
			}

			// Check if we failed?
			if (!bFailed)
			{
				return true;
			}
			return false;
		}

		public override bool GetDatabaseLocks()
		{
			// Make sure we have access to the Database?
			if (MOG_ControllerSystem.GetDB() != null)
			{
				// Get all the lock from the Database
				ArrayList locks = MOG_DBCommandAPI.GetAllLocks();
				if (locks != null)
				{
					for (int i = 0; i < locks.Count; i++)
					{
						MOG_Command command = (MOG_Command)(locks[i]);
						switch (command.GetCommandType())
						{
							case MOG_COMMAND_TYPE.MOG_COMMAND_LockReadRequest:
								mActiveReadLocks.Add(command);
								break;
							case MOG_COMMAND_TYPE.MOG_COMMAND_LockWriteRequest:
								mActiveWriteLocks.Add(command);
								break;
						}
					}
					return true;
				}
				return false;
			}
			return false;
		}

	private void SetUniqueCommandID(MOG_Command pCommand)
	{
		// Make sure we have avalid command?
		if (pCommand != null)
		{
			// Check if we need to set the CommandID?
			if (pCommand.GetCommandID() == 0)
			{
				// Set this command's unique CommandID and the increment gServerCommandIDCounter
				pCommand.SetCommandID(gServerCommandIDCounter++);
			}
		}
	}

	}
}
