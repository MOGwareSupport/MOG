//--------------------------------------------------------------------------------
//	MOG_CommandSlave.cpp
//	
//	
//--------------------------------------------------------------------------------

#include "StdAfx.h"

#include "MOG_Define.h"
#include "MOG_ControllerPackageMergeNetwork.h"
#include "MOG_ControllerPackageRebuildNetwork.h"
#include "MOG_ControllerAsset.h"
#include "MOG_ControllerRepository.h"
#include "MOG_ControllerProject.h"
#include "MOG_ControllerSystem.h"
#include "MOG_ControllerInbox.h"
#include "MOG_ControllerRipper.h"
#include "MOG_StringUtils.h"
#include "MOG_DosUtils.h"
#include "MOG_Tokens.h"
#include "MOG_EnvironmentVars.h"
#include "MOG_DBAssetAPI.h"
#include "MOG_DBPackageAPI.h"
#include "MOG_DBPostCommandAPI.h"
#include "MOG_CommandFactory.h"

#include "MOG_CommandSlave.h"

#include <process.h>



using namespace System::Diagnostics;
using namespace System::Windows::Forms;

MOG_CommandSlave::MOG_CommandSlave()
{
	mNetwork = new MOG_NetworkClient;

	// Always re-initialize the thread variables
	InitializeThreadVariables(NULL, true);
}


bool MOG_CommandSlave::Shutdown(void)
{
	// Always make sure we abort any running threads
	AbortThread();

	MOG_CommandManager::Shutdown();

	return true;
}


bool MOG_CommandSlave::RegisterWithServer(void)
{
	MOG_Command *command = MOG_CommandFactory::Setup_RegisterSlave(MOG_Main::GetName(), MOG_Main::GetAssociatedWorkspacePath());
	SendToServer(command);

	// Check if we are logged into a project?
	if (MOG_ControllerProject::IsProject())
	{
		// Inform the server what project we have logged in to
		command = MOG_CommandFactory::Setup_LoginProject(MOG_ControllerProject::GetProjectName(), MOG_ControllerProject::GetBranchName());
		SendToServer(command);
	}
	// Check if we have logged in as a user?
	if (MOG_ControllerProject::IsUser())
	{
		// Inform the server about who we are logged in as
		command = MOG_CommandFactory::Setup_LoginUser(MOG_ControllerProject::GetUser()->GetUserName());
		SendToServer(command);
	}

	return true;
}


bool MOG_CommandSlave::CommandExecute(MOG_Command *pCommand)
{
	bool bProcessed = false;

	switch (pCommand->GetCommandType())
	{
		case MOG_COMMAND_ShutdownSlave:
			if (Command_ShutdownSlave(pCommand))
			{
				bProcessed = true;
			}
			break;
		case MOG_COMMAND_LoginProject:
			if (Command_LoginProject(pCommand))
			{
				bProcessed = true;
			}
			break;
		case MOG_COMMAND_LoginUser:
			if (Command_LoginUser(pCommand))
			{
				bProcessed = true;
			}
			break;
		case MOG_COMMAND_AssetRipRequest:
			if (Command_AssetRipRequest(pCommand))
			{
				bProcessed = true;
			}
			break;
		case MOG_COMMAND_AssetProcessed:
			if (Command_AssetProcessed(pCommand))
			{
				bProcessed = true;
			}
			break;
		case MOG_COMMAND_SlaveTask:
			if (Command_SlaveTask(pCommand))
			{
				bProcessed = true;
			}
			break;
		case MOG_COMMAND_ReinstanceAssetRevision:
			if (Command_ReinstanceAssetRevision(pCommand))
			{
				bProcessed = true;
			}
			break;
		case MOG_COMMAND_Bless:
			if (Command_Bless(pCommand))
			{
				bProcessed = true;
			}
			break;
		case MOG_COMMAND_RemoveAssetFromProject:
			if (Command_RemoveAssetFromProject(pCommand))
			{
				bProcessed = true;
			}
			break;
		case MOG_COMMAND_NetworkPackageMerge:
			if (Command_NetworkPackageMerge(pCommand))
			{
				bProcessed = true;
			}
			break;
		case MOG_COMMAND_NetworkPackageRebuild:
			if (Command_NetworkPackageRebuild(pCommand))
			{
				bProcessed = true;
			}
			break;
		case MOG_COMMAND_Post:
			if (Command_Post(pCommand))
			{
				bProcessed = true;
			}
			break;
		case MOG_COMMAND_Archive:
			if (Command_Archive(pCommand))
			{
				bProcessed = true;
			}
			break;
		case MOG_COMMAND_ScheduleArchive:
			if (Command_ScheduleArchive(pCommand))
			{
				bProcessed = true;
			}
			break;
		case MOG_COMMAND_LockReadRequest:
			if (Command_LockReadRequest(pCommand))
			{
				bProcessed = true;
			}
			break;
		case MOG_COMMAND_LockReadRelease:
			if (Command_LockReadRelease(pCommand))
			{
				bProcessed = true;
			}
			break;
		case MOG_COMMAND_LockWriteRequest:
			if (Command_LockWriteRequest(pCommand))
			{
				bProcessed = true;
			}
			break;
		case MOG_COMMAND_LockWriteRelease:
			if (Command_LockWriteRelease(pCommand))
			{
				bProcessed = true;
			}
			break;
		case MOG_COMMAND_BuildFull:
			if (Command_BuildFull(pCommand))
			{
				bProcessed = true;
			}
			break;
		case MOG_COMMAND_Build:
			if (Command_Build(pCommand))
			{
				bProcessed = true;
			}
			break;

		default:
			// Allow the parent a chance to process this command
			if (MOG_CommandManager::CommandExecute(pCommand))
			{
				bProcessed = true;
			}
			break;
	}

	return bProcessed;
}


String *MOG_CommandSlave::FormatSlaveLogCommandComment(MOG_Command *pCommand, String *comment)
{
    MOG_Time *time = new MOG_Time();

    // Dress up the comment with a nice border before the comment
    // Include the current date/time, current user name as well as the computer name
    String *formatedComment = "\r\n--------------------------------------------------------------------------------\r\n";
	formatedComment = String::Concat(formatedComment, S"COMMAND: ", pCommand->ToString(), "\r\n");
	formatedComment = String::Concat(formatedComment, S"   DATE: ", time->FormatString(String::Concat(TOKEN_Day_2, S":", TOKEN_Month_2, S":", TOKEN_Year_2, S"     ", TOKEN_Hour_2, S":", TOKEN_Minute_2, S":", TOKEN_Second_2, S":", TOKEN_ampm)), S"\r\n");

	// Check if there was a user specified in the command?
	if (pCommand->GetProject()->Length)
	{
		formatedComment = String::Concat(formatedComment, S"PROJECT: ", pCommand->GetProject(), "\r\n");
	}

	// Check if there was a user specified in the command?
	if (pCommand->GetBranch()->Length)
	{
		formatedComment = String::Concat(formatedComment, S" BRANCH: ", pCommand->GetBranch(), "\r\n");
	}

	// Check if there was a user specified in the command?
	if (pCommand->GetUserName()->Length)
	{
		formatedComment = String::Concat(formatedComment, S"   USER: ", pCommand->GetUserName(), "\r\n");
	}

	// Check if there was an asset specified in the command?
	if (pCommand->GetAssetFilename()->GetFullFilename()->Length)
	{
		// Check if this command contains an asset?
		if (pCommand->GetAssetFilename()->GetFilenameType() == MOG_FILENAME_TYPE::MOG_FILENAME_Asset)
		{
			// Dump out the actual asset name
			formatedComment = String::Concat(formatedComment, S"  ASSET: ", pCommand->GetAssetFilename()->GetAssetFullName(), "\r\n");
		}

		// Dump out the full path
		formatedComment = String::Concat(formatedComment, S"  ASSET: ", pCommand->GetAssetFilename()->GetFullFilename(), "\r\n");
	}

	// Check if there is an additional comment?
	if (comment->Length)
	{
		formatedComment = String::Concat(formatedComment, comment, "\r\n");
	}

	return formatedComment;
}


bool MOG_CommandSlave::Command_ShutdownSlave(MOG_Command *pCommand)
{
	// Shut ourselves down
	MOG_Main::Shutdown();

	return true;
}


bool MOG_CommandSlave::Command_ConnectionKill(MOG_Command *pCommand)
{
	// Inform our server about us shutting down
	MOG_Command *pShutdownSlave = MOG_CommandFactory::Setup_ShutdownSlave();
	SendToServer(pShutdownSlave);

	// Shut ourselves down
	MOG_CommandManager::Command_ConnectionKill(pCommand);

	// Always eat this command.
	return true;
}


bool MOG_CommandSlave::Command_LoginProject(MOG_Command *pCommand)
{
	// Always eat this command...We don't care if the server to tracks this
	return true;
}


bool MOG_CommandSlave::Command_LoginUser(MOG_Command *pCommand)
{
	// Always eat this command...We don't care if the server to tracks this
	return true;
}


bool MOG_CommandSlave::Command_AssetRipRequest(MOG_Command *pCommand)
{
	// Check if we are free to start a SlaveTask thread?
	if (!gThread)
	{
		// Always re-initialize the thread variables
		InitializeThreadVariables(pCommand, mCommandLineHideWindow);

		// Start a new thread so we can continue to process incomming commands
		gThread = new Thread(new ThreadStart(0, Thread_Process_AssetRipRequest));
		gThread->Start();

		// Always eat the command when we successfully start a new thread
		return true;
	}

	return false;
}


void MOG_CommandSlave::Thread_Process_AssetRipRequest()
{
	MOG_Command *pCommand = gCommand;

	try
	{
		// Log into the server using the platform and user specified in the command
		MOG_ControllerProject::LoginProject(pCommand->GetProject(), pCommand->GetBranch());
		MOG_ControllerProject::LoginUser(pCommand->GetUserName());
		MOG_ControllerProject::LoginComputer(pCommand->GetComputerName());

		// Record this command in the slave's output log
		String *message = FormatSlaveLogCommandComment(pCommand, S"");
		MOG_ControllerSystem::GetCommandManager()->AppendMessageToOutputLog(message);

		// Create a Ripper to perform all the work detailed in the command
		MOG_ControllerRipper *ripper = new MOG_ControllerRipper(pCommand);
		// Check if we should show the rip window?
		if (gCommandLineHideWindow == false)
		{
			ripper->ShowRipCommandWindow(true);
		}
		// Generate the asset's SlaveTasks
		ArrayList *tasks = ripper->GenerateSlaveTaskCommands();
		if (tasks->Count)
		{
			// Send all of the generated slave tasks to the server for processing
			for (int t = 0; t < tasks->Count; t++)
			{
				MOG_Command *task = __try_cast<MOG_Command*>(tasks->Item[t]);
				MOG_ControllerSystem::GetCommandManager()->SendToServer(task);
			}
		}
		else
		{
			String *message = String::Concat(	S"No SlaveTasks were generated.\n",
												S"ASSET: ", pCommand->GetAssetFilename()->GetAssetFullName(), S"\n\n",
												S"The asset was not ripped correctly.");
			MOG_Report::ReportMessage(S"RipTasker Failed", message, S"", MOG_ALERT_LEVEL::ERROR);
		}
	}
	catch (Exception *e)
	{
		String *message = String::Concat(	S"The Slave ecountered an Exception while working on AssetName:", pCommand->GetAssetFilename()->GetAssetFullName(), S"\n",
											e->Message);
		MOG_Report::ReportMessage(S"Asset Rip", message, e->StackTrace, MOG_ALERT_LEVEL::CRITICAL);
	}
	__finally
	{
		// Send back the AssetProcessed command
		MOG_Command *assetProcessed = MOG_CommandFactory::Setup_AssetProcessed(pCommand->GetNetworkID(), pCommand->GetUserName(), pCommand->GetJobLabel(), pCommand->GetAssetFilename(), pCommand->GetValidSlaves());
		MOG_ControllerSystem::GetCommandManager()->SendToServer(assetProcessed);

		// Send back the RipRequestComplete command
		MOG_Command *complete =  MOG_CommandFactory::Setup_Complete(pCommand, true);
		MOG_ControllerSystem::GetCommandManager()->SendToServer(complete);

		// Clear the project's logged in ComputerName
		MOG_ControllerProject::LoginComputer(NULL);

		// Abort our thread
		MOG_CommandManager::AbortThread();
	}
}


bool MOG_CommandSlave::Command_AssetProcessed(MOG_Command *pCommand)
{
	try
	{
		// Log into the server using the platform and user specified in the command
		MOG_ControllerProject::LoginProject(pCommand->GetProject(), pCommand->GetBranch());
		MOG_ControllerProject::LoginUser(pCommand->GetUserName());
		MOG_ControllerProject::LoginComputer(pCommand->GetComputerName());

		// Record this command in the slave's output log
		String *message = FormatSlaveLogCommandComment(pCommand, S"");
		MOG_ControllerSystem::GetCommandManager()->AppendMessageToOutputLog(message);

		// Create a Ripper to complete the rip
		MOG_ControllerRipper *ripper = new MOG_ControllerRipper(pCommand);
		ripper->CompleteRip();

		// Inform the Server we are finished with this command
		MOG_Command *complete =  MOG_CommandFactory::Setup_Complete(pCommand, true);
		return SendToServer(complete);
	}
	__finally
	{
		// Clear the project's logged in ComputerName
		MOG_ControllerProject::LoginComputer(NULL);
	}
}


bool MOG_CommandSlave::Command_SlaveTask(MOG_Command *pCommand)
{
	// Check if we are free to start a SlaveTask thread?
	if (!gThread)
	{
		// Always re-initialize the thread variables
		InitializeThreadVariables(pCommand, mCommandLineHideWindow);

		// Start a new thread so we can continue to process incomming commands
		gThread = new Thread(new ThreadStart(0, Thread_Process_SlaveTask));
		gThread->Start();

		// Always eat the command when we successfully start a new thread
		return true;
	}

	return false;
}


void MOG_CommandSlave::Thread_Process_SlaveTask()
{
	MOG_Command *pCommand = gCommand;

	try
	{
		// Log into the server using the platform and user specified in the command
		MOG_ControllerProject::LoginProject(pCommand->GetProject(), pCommand->GetBranch());
		MOG_ControllerProject::LoginUser(pCommand->GetUserName());
		MOG_ControllerProject::LoginComputer(pCommand->GetComputerName());

		// Record this command in the slave's output log
		String *message = FormatSlaveLogCommandComment(pCommand, S"");
		MOG_ControllerSystem::GetCommandManager()->AppendMessageToOutputLog(message);

		// Create a Ripper to perform all the work detailed in the command
		MOG_ControllerRipper *ripper = new MOG_ControllerRipper(pCommand);
		// Check if we should show the rip window?
		if (gCommandLineHideWindow == false)
		{
			ripper->ShowRipCommandWindow(true);
		}
		// Process the designated SlaveTask
		if (!ripper->ProcessSlaveTask(pCommand))
		{
			// What should we do?
			// Assume the controller will report the adequest errors/warnings
		}
	}
	catch (Exception *e)
	{
		String *message = String::Concat(	S"The Slave ecountered an Exception while working on AssetName:", pCommand->GetAssetFilename()->GetAssetFullName(), S"\n",
											e->Message);
		MOG_Report::ReportMessage(S"Asset Rip", message, e->StackTrace, MOG_ALERT_LEVEL::CRITICAL);
	}
	__finally
	{
		// Launch a SlaveTaskComplete command
		MOG_Command *slaveTaskComplete = MOG_CommandFactory::Setup_Complete(pCommand, true);
		MOG_ControllerSystem::GetCommandManager()->SendToServer(slaveTaskComplete);

		// Clear the project's logged in ComputerName
		MOG_ControllerProject::LoginComputer(NULL);

		// Abort our thread
		MOG_CommandManager::AbortThread();
	}
}

bool MOG_CommandSlave::Command_ReinstanceAssetRevision(MOG_Command *pCommand)
{
	// Check if we are missing the Database?
	if (!MOG_ControllerSystem::GetDB())
	{
		// Never process this command w/o the database!
		return false;
	}

	// Check if we are free to start a thread?
	if (!gThread)
	{
		// Always re-initialize the thread variables
		InitializeThreadVariables(pCommand, mCommandLineHideWindow);

		// Start a new thread so we can continue to process incomming commands
		gThread = new Thread(new ThreadStart(0, Thread_Process_ReinstanceAssetRevision));
		gThread->Start();

		// Always eat the command when we successfully start a new thread
		return true;
	}

	return false;
}

void MOG_CommandSlave::Thread_Process_ReinstanceAssetRevision()
{
	MOG_Command *pCommand = gCommand;
	String *propertiesArray[] = NULL;
	
	try
	{
		// Log into the specified project
		MOG_ControllerProject::LoginProject(pCommand->GetProject(), pCommand->GetBranch());
		MOG_ControllerProject::LoginUser(pCommand->GetUserName());
		MOG_ControllerProject::LoginComputer(pCommand->GetComputerName());
		
		// Record this command in the slave's output log
		String *message = FormatSlaveLogCommandComment(pCommand, S"");
		MOG_ControllerSystem::GetCommandManager()->AppendMessageToOutputLog(message);

		// Split up any specified properties
		if(pCommand->GetVeriables()->Length > 0)
		{
			//first make sure we have all valid properties.
			int lastValidPropertyIndex = 0;
			propertiesArray = pCommand->GetVeriables()->Split(S",;"->ToCharArray());

			//This preprocesses the properties to ensure that we didn't have ',' or ';' contained in the property value
			for(int propertyIndex = 0; propertyIndex < propertiesArray->Count; propertyIndex++)
			{
				//check if this appears to be a valid property string?
				if(propertiesArray[propertyIndex]->IndexOf(S"[") != -1)
				{
					// Indicate this as our last identified property string
					lastValidPropertyIndex = propertyIndex;
				}
				else
				{
					//if we don't find add or remove at the start then this is a dangling portion of the last valid property so append it back to the previous property string.
					String::Concat(propertiesArray[lastValidPropertyIndex], S",", propertiesArray[propertyIndex]);
					propertiesArray[propertyIndex] = S"";
				}
			}
		}

		// Check to make sure this is an asset?
		if (pCommand->GetAssetFilename()->GetFilenameType() == MOG_FILENAME_Asset)
		{
			MOG_Filename *blessedAssetFilename = pCommand->GetAssetFilename();

			//check to see if this is in the mog repository
			if (!blessedAssetFilename->IsWithinRepository())
			{
				if(blessedAssetFilename->GetVersionTimeStamp()->Length)
				{
					//establish a fully qualified mog filename if we don't already have one.
					blessedAssetFilename = MOG_ControllerRepository::GetAssetBlessedVersionPath(blessedAssetFilename, blessedAssetFilename->GetVersionTimeStamp());
				}
			}

			// Make sure this asset exists in the MOG Repository?
			if (blessedAssetFilename->IsWithinRepository())
			{
				bool bPerformBlessCommand = false;

				// Check if the new asset already exists?
				// This could happen if they rename a classification back to an old name where the asset previously existed
				if (DosUtils::DirectoryExistFast(blessedAssetFilename->GetEncodedFilename()))
				{
//?	MOG_CommandSlave::Thread_Process_ReinstanceAssetRevision - The new instance already exists so we really should check the current asset's files with the target asset's files to ensure they really are identical.
					// Use a new timsestamp If there are any modified files between the colliding asset revisions
//TODO				if (!VerifyAssetIntegrity(blessedAssetFilename->GetEncodedFilename(),pCommand->GetSource()))
					if (false)
					{
						// Build a new target revision to avoid overwriting the older revision
						MOG_Filename *newFilename = new MOG_Filename(blessedAssetFilename->GetEncodedFilename(), MOG_Time::GetVersionTimestamp());

						// Inform the user that we detected a mis-matched asset that required a new revision in order to maintain integrity
//TODO

						// Establish the newFilename as our new target
						pCommand->SetAssetFilename(newFilename->GetEncodedFilename());
					}
					else
					{
						// Indicate that we want to perform the bless command
						bPerformBlessCommand = true;
					}
				}

				// Check if the asset needs to be copied over?
				// We are assuming that if it already exists then it should be identical
				if (!DosUtils::DirectoryExistFast(blessedAssetFilename->GetEncodedFilename()))
				{
					// Make sure the source asset's directory exists
					if (DosUtils::DirectoryExistFast(pCommand->GetSource()))
					{
						// Copy the asset to the new location
						if (DosUtils::DirectoryCopyFast(pCommand->GetSource(), blessedAssetFilename->GetEncodedFilename(), true))
						{
						}
						else
						{
							String *message = String::Concat(	S"The System was unable to copy a new instance of '", blessedAssetFilename->GetAssetFullName(), S"'.\n",
																S"The Asset was not reinstanced correctly.");
							MOG_Report::ReportMessage(S"ReinstanceAssetRevision", message, S"", MOG_ALERT_LEVEL::ERROR);
						}
					}
					else
					{
						String *message = String::Concat(	S"The Slave was unable to locate ", pCommand->GetSource(), S" to perform a ReinstanceAssetRevision Command\n",
															S"The Asset was not reinstanced correctly.");
						MOG_Report::ReportMessage(S"ReinstanceAssetRevision", message, S"", MOG_ALERT_LEVEL::ERROR);
					}
				}

				// Open the new asset
				MOG_ControllerAsset *newAsset = MOG_ControllerAsset::OpenAsset(blessedAssetFilename);
				if (newAsset)
				{
					try
					{
						//Fix up relationships if necessary before we fire off this bless
						if (propertiesArray != NULL)
						{
							//next itterate through the valid properties and remove them as appropriate.
							for(int removePropertiesIndex = 0; removePropertiesIndex < propertiesArray->Count; removePropertiesIndex++)
							{
								//See if we are a remove property.
								if(propertiesArray[removePropertiesIndex]->ToLower()->StartsWith(S"REMOVE"->ToLower()))
								{
									MOG_Property *propertyToRemove = new MOG_Property(propertiesArray[removePropertiesIndex]);
									//check to see if we have a valid property
									if(propertyToRemove->mSection->Length > 0)
									{
										newAsset->GetProperties()->RemoveProperty(propertyToRemove->mSection, propertyToRemove->mPropertySection, propertyToRemove->mPropertyKey);
									}
								}
							}

							//Itterate through the loop adding all add properties.
							for(int addPropertiesIndex = 0; addPropertiesIndex < propertiesArray->Count; addPropertiesIndex++)
							{
								//check to see if this is a add property
								if(propertiesArray[addPropertiesIndex]->ToLower()->StartsWith(S"ADD"->ToLower()))
								{
									MOG_Property *propertyToAdd = new MOG_Property(propertiesArray[addPropertiesIndex]);
									//check to see if we have a valid property.
									if(propertyToAdd->mSection->Length > 0)
									{
										newAsset->GetProperties()->SetProperty(propertyToAdd->mSection, propertyToAdd->mPropertySection, propertyToAdd->mPropertyKey, propertyToAdd->mValue);
									}
								}
							}

							newAsset->GetProperties()->Save();
						}

						// Check if this asset's revision already exists in the repository?
						bool bAddedInRepository = false;
						if (MOG_DBAssetAPI::GetAssetVersionID(blessedAssetFilename, blessedAssetFilename->GetVersionTimeStamp()))
						{
							bAddedInRepository = true;
						}
						else
						{
							// Add this asset to the database now that it exists in the MOG Repository
							if (MOG_DBAssetAPI::AddAsset(blessedAssetFilename, newAsset->GetProperties()->Creator, newAsset->GetProperties()->CreatedTime))
							{
								// Add this asset to the repository
								if (MOG_ControllerRepository::AddAssetRevisionInfo(newAsset, false))
								{
									bAddedInRepository = true;
								}
								else
								{
									String *message = String::Concat(	S"The System failed to add new revision of ", blessedAssetFilename->GetAssetFullName(), S" in the MOG Repository.\n",
																		S"The Asset was not reinstanced correctly.");
									MOG_Prompt::PromptMessage(S"ReinstanceAssetRevision", message, S"", MOG_ALERT_LEVEL::ERROR);
								}
							}
							else
							{
								String *message = String::Concat(	S"The System failed to add ", blessedAssetFilename->GetAssetFullName(), S" in the MOG Repository.\n",
																	S"The Asset was not reinstanced correctly.");
								MOG_Prompt::PromptMessage(S"ReinstanceAssetRevision", message, S"", MOG_ALERT_LEVEL::ERROR);
							}
						}

						// Check if this asset was successfully added to the repository?
						if (bAddedInRepository)
						{
							bPerformBlessCommand = true;

							// Open the old asset
							MOG_Filename *oldAssetFilename = new MOG_Filename(pCommand->GetSource());
							MOG_ControllerAsset *oldAsset = MOG_ControllerAsset::OpenAsset(oldAssetFilename);
							if (oldAsset)
							{
								try
								{
									// Check if this IsPackage?
									if (oldAsset->GetProperties()->IsPackage)
									{
										// Populate the new package information from the old package
										MOG_DBPackageAPI::PopulateNewPackageVersion(oldAssetFilename, blessedAssetFilename, NULL);
									}

									// Check if there was a name change?
									if (String::Compare(oldAssetFilename->GetAssetFullName(), blessedAssetFilename->GetAssetFullName(), true) != 0)
									{
										// Post the comment to this asset
										String *message = String::Concat(S"Renamed asset to '", blessedAssetFilename->GetAssetFullName(), S"'");
										oldAsset->PostComment(message, true);

										// Check if this asset is a PackagedAsset?
										if (oldAsset->GetProperties()->IsPackagedAsset)
										{
											// Schedule the old asset to be removed from it's packages
											MOG_ControllerPackageMergeNetwork::ScheduleAssetPackageCommands(oldAsset, MOG_PACKAGECOMMAND_TYPE::MOG_PackageCommand_RemoveAsset, pCommand->GetJobLabel(), pCommand->GetValidSlaves(), false);
										}

										// Check if this asset is a package?
										if (oldAsset->GetProperties()->IsPackage)
										{
//?	MOG_CommandSlave::Thread_Process_ReinstanceAssetRevision - We really should rename/reassign the outstanding late resolvers to the new name instead of just removing them
											//Remove any late resolvers for this package that may exist in the database
											ArrayList* lateResolvers = MOG_DBPackageCommandAPI::GetScheduledLateResolverCommands(oldAssetFilename->GetAssetFullName(), pCommand->GetBranch(), pCommand->GetPlatform());
											MOG_DBPackageCommandAPI::RemovePackageCommands(lateResolvers);
										}

										// Remove the old asset from the system
										MOG_DBAssetAPI::RemoveAssetFromBranch(oldAssetFilename, S"");
//?	MOG_CommandSlave::Thread_Process_ReinstanceAssetRevision - We should send out a MOG_Command informing everyone about this asset being removed from the project

										// Check if we want to clean up empty classifications
										if (pCommand->GetOptions()->ToLower()->IndexOf(S"|RemoveEmptyClassifications|"->ToLower()) != -1)
										{
											// Check if we can collapse the classifications?
											String *oldClassification = oldAssetFilename->GetAssetClassification();
											while (oldClassification->Length)
											{
												// Check if we have reached the end of our scope between the new and old name?
												if (MOG_Filename::IsParentClassificationString(blessedAssetFilename->GetAssetClassification(), oldClassification))
												{
													// There are no more differences between the new and old name
													break;
												}

												// Check if there are any assets still assigned to this classification?
												if (MOG_DBAssetAPI::GetDependantAssetCount(oldClassification))
												{
													// We don't want to remove remove any classifications still containing assets
													break;
												}

												// Remove the old classification
												MOG_DBAssetAPI::RemoveClassification(oldClassification);

												// Repeat the process again for our previous parent classification
												oldClassification = MOG_Filename::GetParentsClassificationString(oldClassification);
											}
										}
									}
								}
								catch (Exception *e)
								{
									String *message = String::Concat(	S"The Slave ecountered an Exception while working on AssetName:", pCommand->GetAssetFilename()->GetAssetFullName(), S"\n",
																		e->Message);
									MOG_Report::ReportMessage(S"ReinstanceAssetRevision", message, e->StackTrace, MOG_ALERT_LEVEL::CRITICAL);
								}
								__finally
								{
									// Close the old asset controller
									oldAsset->Close();
								}
							}
						}
					}
					catch (Exception *e)
					{
						String *message = String::Concat(	S"The Slave ecountered an Exception while working on AssetName:", pCommand->GetAssetFilename()->GetAssetFullName(), S"\n",
															e->Message);
						MOG_Report::ReportMessage(S"ReinstanceAssetRevision", message, e->StackTrace, MOG_ALERT_LEVEL::CRITICAL);
					}
					__finally
					{
						// Make sure we close the asset controller
						newAsset->Close();
					}
				}
				else
				{
					String *message = String::Concat(	S"The System was unable to open the specified asset located in the MOG Repository.\n",
														S"ASSET: ", blessedAssetFilename->GetAssetFullName(), S"\n",
														S"The Asset was not reinstanced correctly.");
					MOG_Report::ReportMessage(S"ReinstanceAssetRevision", message, S"", MOG_ALERT_LEVEL::ERROR);
				}

				// Check if we have determined whether or not to perform the bless?
				if (bPerformBlessCommand)
				{
					// Check if a specific target package was specified in the command?
					MOG_Filename* targetPackageFilename = NULL;
					if (pCommand->GetDestination()->Length)
					{
						// Initialize a PackageFileInfo for use when we schedule the package commands
						targetPackageFilename = new MOG_Filename(pCommand->GetDestination());
					}
					// Fire off a bless command for this already existing revision (Indicate we can skip the archiving)
					MOG_Command *bless = MOG_CommandFactory::Setup_Bless(blessedAssetFilename, pCommand->GetJobLabel(), pCommand->GetValidSlaves(), false, false, targetPackageFilename);
					MOG_ControllerSystem::GetCommandManager()->SendToServer(bless);
				}
			}
			else
			{
				String *message = String::Concat(	S"ReinstanceAssetRevision Commands can only be performed on asset revisions located in the MOG Repository.\n",
													S"ASSET: ", blessedAssetFilename->GetAssetFullName(), S"\n",
													S"The Asset was not reinstanced correctly.");
				MOG_Report::ReportMessage(S"ReinstanceAssetRevision", message, S"", MOG_ALERT_LEVEL::ERROR);
			}
		}
		else
		{
			String *message = String::Concat(	S"The System was unable to recognize ", pCommand->GetAssetFilename()->GetAssetFullName(), S" as a valid asset\n",
												S"The Asset was not reinstaced correctly.");
			MOG_Report::ReportMessage(S"ReinstanceAssetRevision", message, S"", MOG_ALERT_LEVEL::ERROR);
		}
	}
	catch (Exception *e)
	{
		String *message = String::Concat(	S"The Slave ecountered an Exception while working on AssetName:", pCommand->GetAssetFilename()->GetAssetFullName(), S"\n",
											e->Message);
		MOG_Report::ReportMessage(S"ReinstanceAssetRevision", message, e->StackTrace, MOG_ALERT_LEVEL::CRITICAL);
	}
	__finally
	{
		// Send back the ReinstanceAssetRevisionComplete Command
		MOG_Command *complete = MOG_CommandFactory::Setup_Complete(pCommand, true);
		MOG_ControllerSystem::GetCommandManager()->SendToServer(complete);

		// Clear the project's logged in ComputerName
		MOG_ControllerProject::LoginComputer(NULL);

		// Abort our thread
		MOG_CommandManager::AbortThread();
	}
}


bool MOG_CommandSlave::Command_Bless(MOG_Command *pCommand)
{
	// Check if we are missing the Database?
	if (!MOG_ControllerSystem::GetDB())
	{
		// Never process this command w/o the database!
		return false;
	}

	// Check if we are free to start a SlaveTask thread?
	if (!gThread)
	{
		// Always re-initialize the thread variables
		InitializeThreadVariables(pCommand, mCommandLineHideWindow);

		// Start a new thread so we can continue to process incomming commands
		gThread = new Thread(new ThreadStart(0, Thread_Process_Bless));
		gThread->Start();

		// Always eat the command when we successfully start a new thread
		return true;
	}

	return false;
}

void MOG_CommandSlave::Thread_Process_Bless()
{
	MOG_Command *pCommand = gCommand;
	MOG_Filename* assetFilename = pCommand->GetAssetFilename();

	try
	{
		// Log into the specified project
		MOG_ControllerProject::LoginProject(pCommand->GetProject(), pCommand->GetBranch());
		MOG_ControllerProject::LoginUser(pCommand->GetUserName());
		MOG_ControllerProject::LoginComputer(pCommand->GetComputerName());

		// Record this command in the slave's output log
		String *message = FormatSlaveLogCommandComment(pCommand, S"");
		MOG_ControllerSystem::GetCommandManager()->AppendMessageToOutputLog(message);

		// Make sure the asset's directory still exists
		if (DosUtils::DirectoryExistFast(assetFilename->GetEncodedFilename()))
		{
			if (assetFilename->GetFilenameType() == MOG_FILENAME_Asset)
			{
				// Make sure this asset exists in the MOG Repository?
				if (assetFilename->IsWithinRepository())
				{
					// Open the new asset
					MOG_ControllerAsset *newAsset = MOG_ControllerAsset::OpenAsset(assetFilename);
					if (newAsset)
					{
						bool bSkipAssetArchiving = false;

						// Check if this bless has requested we skip the AssetArchiving?
						if (pCommand->GetOptions()->ToLower()->IndexOf(S"|SkipArchiving|"->ToLower()) != -1)
						{
							bSkipAssetArchiving = true;
						}

						// Check if this bless has not requested we skip this packaing events?
						if (pCommand->GetOptions()->ToLower()->IndexOf(S"|SkipPackaging|"->ToLower()) == -1)
						{
							// Check if there was a specific target package specified in the command?
							MOG_PackageMerge_PackageFileInfo* targetPackageFileInfo = NULL;
							if (pCommand->GetDestination()->Length)
							{
								// Initialize a PackageFileInfo for use when we schedule the package commands
								MOG_Filename* targetPackageFilename = new MOG_Filename(pCommand->GetDestination());
								targetPackageFileInfo = new MOG_PackageMerge_PackageFileInfo(targetPackageFilename, "", targetPackageFilename->GetAssetPlatform(), pCommand->GetJobLabel());
							}

							// Check if we have an old revision of this asset?
							String *oldTimestamp = MOG_DBAssetAPI::GetAssetVersion(assetFilename);
							if (oldTimestamp->Length)
							{
								// Only open the oldAsset if there was a valid version previously blessed
								MOG_ControllerAsset *oldAsset = MOG_ControllerAsset::OpenAsset(MOG_ControllerRepository::GetAssetBlessedVersionPath(assetFilename, oldTimestamp));
								if (oldAsset)
								{
									// Always attempt to schedule this asset to be removed from it's packages
									ArrayList *pAffectedPackages = MOG_ControllerPackageMergeNetwork::ScheduleAssetPackageCommands(oldAsset, MOG_PACKAGECOMMAND_TYPE::MOG_PackageCommand_RemoveAsset, pCommand->GetJobLabel(), pCommand->GetValidSlaves(), targetPackageFileInfo, true);
									if (pAffectedPackages->Count)
									{
									}
									// Make sure we close the asset controller
									oldAsset->Close();
								}
							}
							else
							{
								// Check if this new asset is platform specific?
								if (assetFilename->IsPlatformSpecific())
								{
									// Remove the already existing generic asset in preparation of this incoming platform-specific one
									MOG_ControllerPackageMergeNetwork::SchedulePlatformSpecificAssetAddition(newAsset, pCommand->GetJobLabel(), pCommand->GetValidSlaves());
								}
							}

							// Always schedule this asset to be added to it's packages
							ArrayList *pAffectedPackages = MOG_ControllerPackageMergeNetwork::ScheduleAssetPackageCommands(newAsset, MOG_PACKAGECOMMAND_TYPE::MOG_PackageCommand_AddAsset, pCommand->GetJobLabel(), pCommand->GetValidSlaves(), targetPackageFileInfo, true);
							if (pAffectedPackages->Count)
							{
								// Change our status to Packaging
								MOG_ControllerInbox::UpdateInboxView(newAsset, MOG_AssetStatusType::Packaging);
								// Update any links to reflect our new status
								MOG_ControllerInbox::UpdateAssetLinkFiles(newAsset);
							}
						}

						// Schedule the asset for posting
						MOG_ControllerProject::AddAssetForPosting(assetFilename, pCommand->GetJobLabel(), newAsset->GetProperties()->Owner, newAsset->GetProperties()->Creator);
						// Fire off the post command
						MOG_Command *post =  MOG_CommandFactory::Setup_Post(pCommand->GetJobLabel(), bSkipAssetArchiving);
						post->SetNetworkID(1);								// Indicate that this was an automatically generated command
						post->SetValidSlaves(pCommand->GetValidSlaves());	// Always pass along the specified ValidSlaves from the original command
						MOG_ControllerSystem::GetCommandManager()->SendToServer(post);

						// Make sure we close the asset controller
						newAsset->Close();
					}
					else
					{
						String *message = String::Concat(	S"The System was unable to open the specified asset located in the MOG Repository.\n",
															S"ASSET: ", assetFilename->GetAssetFullName(), S"\n",
															S"The Asset was not blessed correctly.");
						MOG_Report::ReportMessage(S"Bless", message, S"", MOG_ALERT_LEVEL::ERROR);
					}
				}
				else
				{
					String *message = String::Concat(	S"Bless Commands can only be performed on asset revisions located in the MOG Repository.\n",
														S"ASSET: ", assetFilename->GetAssetFullName(), S"\n",
														S"The Asset was not blessed correctly.");
					MOG_Report::ReportMessage(S"Bless", message, S"", MOG_ALERT_LEVEL::ERROR);
				}
			}
			else
			{
				String *message = String::Concat(	S"The System was unable to recognize ", assetFilename->GetAssetFullName(), S" as a valid asset\n",
													S"The Asset was not blessed correctly.");
				MOG_Report::ReportMessage(S"Bless", message, S"", MOG_ALERT_LEVEL::ERROR);
			}
		}
		else
		{
			String *message = String::Concat(	S"The Slave was unable to locate ", assetFilename->GetAssetFullName(), S" to perform its Bless Command\n",
												S"The Asset was not blessed correctly.");
			MOG_Report::ReportMessage(S"Bless", message, S"", MOG_ALERT_LEVEL::ERROR);
		}

	}
	catch (Exception *e)
	{
		String *message = String::Concat(	S"The Slave ecountered an Exception while working on AssetName:", assetFilename->GetAssetFullName(), S"\n",
											e->Message);
		MOG_Report::ReportMessage(S"Bless", message, e->StackTrace, MOG_ALERT_LEVEL::CRITICAL);
	}
	__finally
	{
		// Send our complete command back to the server
		MOG_Command *complete =  MOG_CommandFactory::Setup_Complete(pCommand, true);
		MOG_ControllerSystem::GetCommandManager()->SendToServer(complete);

		// Clear the project's logged in ComputerName
		MOG_ControllerProject::LoginComputer(NULL);

		// Abort our thread
		MOG_CommandManager::AbortThread();
	}
}


bool MOG_CommandSlave::Command_RemoveAssetFromProject(MOG_Command *pCommand)
{
	// Check if we are missing the Database?
	if (!MOG_ControllerSystem::GetDB())
	{
		// Never process this command w/o the database!
		return false;
	}

	// Check if we are free to start a SlaveTask thread?
	if (!gThread)
	{
		// Always re-initialize the thread variables
		InitializeThreadVariables(pCommand, mCommandLineHideWindow);

		// Start a new thread so we can continue to process incomming commands
		gThread = new Thread(new ThreadStart(0, Thread_Process_RemoveAssetFromProject));
		gThread->Start();

		// Always eat the command when we successfully start a new thread
		return true;
	}

	return false;
}

void MOG_CommandSlave::Thread_Process_RemoveAssetFromProject()
{
//?	MOG_CommandSlave::RemoveAssetFromProject - We need an easier/safer way to recover from a large multi-asset removal in case of a user error (Pete's Mistake)
	MOG_Command *pCommand = gCommand;
	bool bFailed = false;

	try
	{
		// Log into the specified project
		MOG_ControllerProject::LoginProject(pCommand->GetProject(), pCommand->GetBranch());
		MOG_ControllerProject::LoginUser(pCommand->GetUserName());
		MOG_ControllerProject::LoginComputer(pCommand->GetComputerName());

		// Record this command in the slave's output log
		String *message = FormatSlaveLogCommandComment(pCommand, S"");
		MOG_ControllerSystem::GetCommandManager()->AppendMessageToOutputLog(message);

		bool bUnpackageAsset = false;
		bool bRemoveSingleRevision = false;
		bool bRemoveAllRevisions = false;

		// Get the current revision of this asset in our active branch
		String *currentAssetRevision = MOG_ControllerProject::GetAssetCurrentVersionTimeStamp(pCommand->GetAssetFilename());
		// Check if this asset is in our active branch?
		if (currentAssetRevision->Length > 0)
		{
			// Check if there was a revision specified in the remove command?
			if (pCommand->GetAssetFilename()->GetVersionTimeStamp()->Length > 0)
			{
				// Check if the asset's revision is active in our branch?
				if (String::Compare(pCommand->GetAssetFilename()->GetVersionTimeStamp(), currentAssetRevision, true) == 0)
				{
					bUnpackageAsset = true;
				}
				else
				{
					bRemoveSingleRevision = true;
				}
			}
			else
			{
				bUnpackageAsset = true;
			}
		}
		else
		{
			// Check if there was a revision specified in the remove command?
			if (pCommand->GetAssetFilename()->GetVersionTimeStamp()->Length > 0)
			{
				bRemoveSingleRevision = true;
			}
			else
			{
				bRemoveAllRevisions = true;
			}
		}

		// Check if we think this asset should be unpackaged?
		if (bUnpackageAsset)
		{
			// Open the asset...use a try/catch/finally so we can prematurely return and still close the asset controller properly
			MOG_ControllerAsset *asset = NULL;
			try
			{
				// Open the asset
				asset = MOG_ControllerAsset::OpenAsset(pCommand->GetAssetFilename());
				if (asset)
				{
					// Check if this asset was a packaged asset?
					if (asset->GetProperties()->IsPackagedAsset)
					{
						// Check if we should skip the unpackaging event?
						if (pCommand->GetOptions()->IndexOf(S"|SkipUnpackaging|", StringComparison::CurrentCultureIgnoreCase) == -1)
						{
						    // Schedule this asset to be removed from it's packages
						    MOG_ControllerPackageMergeNetwork::ScheduleAssetPackageCommands(asset, MOG_PACKAGECOMMAND_TYPE::MOG_PackageCommand_RemoveAsset, pCommand->GetJobLabel(), pCommand->GetValidSlaves(), false);
							// Check for any package collisions that might need to be repaired with this out-going asset
							RemoveAssetFromProject_RestampAnyCollidingAssets(asset, pCommand->GetJobLabel(), pCommand->GetValidSlaves());
					    }
					}

					// Check if this asset is a package?
					if (asset->GetProperties()->IsPackage)
					{
//?	MOG_CommandSlave::Thread_Process_RemoveAssetFromProject - We should really iterate through the applicable platforms of the package and only remove the related LateResolvers because this approach could purge beyond the scope of this package
						//Remove any late resolvers for this package that may exist in the database
						ArrayList* lateResolvers = MOG_DBPackageCommandAPI::GetScheduledLateResolverCommands(asset->GetAssetFilename()->GetAssetFullName(), MOG_ControllerProject::GetBranchName());
						MOG_DBPackageCommandAPI::RemovePackageCommands(lateResolvers);
					}

					// Post the command's comment to this asset
					asset->PostComment(pCommand->GetDescription(), true);
				}
			}
			__finally
			{
				if (asset)
				{
					asset->Close();
				}
			}

			// Attempt to remove all the revisions of this asset
			if (RemoveAssetFromProject_RemoveAssetFromBranch(pCommand->GetAssetFilename(), pCommand->GetJobLabel()))
			{
				// Check if we should skip the unpackaging event?
				if (pCommand->GetOptions()->IndexOf(S"|SkipUnpackaging|", StringComparison::CurrentCultureIgnoreCase) == -1)
				{
					// Check if this was a platform specific asset?
					if (pCommand->GetAssetFilename()->IsPlatformSpecific())
					{
						// Open the asset
						MOG_ControllerAsset *asset = MOG_ControllerAsset::OpenAsset(pCommand->GetAssetFilename());
						if (asset)
						{
							try
							{
								// Indicate this platform-specific asset is being removed
								MOG_ControllerPackageMergeNetwork::SchedulePlatformSpecificAssetRemoval(asset, pCommand->GetJobLabel(), pCommand->GetValidSlaves());
							}
							__finally
							{
								asset->Close();
							}
						}
					}
				}
			}
			else
			{
				// Indicate we failed
				bFailed = true;
			}
		}

		// Check if we should try to remove all revisions?
		if (bRemoveAllRevisions)
		{
			// Attempt to remove all the revisions of this asset
			if (RemoveAssetFromProject_RemoveAllAssetRevisions(pCommand->GetAssetFilename()))
			{
				// Remove the name of this asset since all of its revisions are now gone
				if (!RemoveAssetFromProject_RemoveAssetName(pCommand->GetAssetFilename()))
				{
					// Indicate we failed
					bFailed = true;
				}
			}
		}
		// Check if we should only remove a single revision?
		else if (bRemoveSingleRevision)
		{
			// Attempt to remove all the revisions of this asset
			if (RemoveAssetFromProject_RemoveAssetRevision(pCommand->GetAssetFilename(), pCommand->GetAssetFilename()->GetVersionTimeStamp(), true))
			{
				// Attempt to remove the name of this asset just in case that was the last revision
				RemoveAssetFromProject_RemoveAssetName(pCommand->GetAssetFilename());
			}
			else
			{
				bFailed = true;
			}
		}
	}
	catch (Exception *e)
	{
		String *message = String::Concat(	S"The Slave ecountered an Exception while working on AssetName:", pCommand->GetAssetFilename()->GetAssetFullName(), S"\n",
											e->Message);
		MOG_Report::ReportMessage(S"RemoveAssetFromProject", message, e->StackTrace, MOG_ALERT_LEVEL::CRITICAL);
	}
	__finally
	{
		// Send our complete command back to the server
		MOG_Command *complete =  MOG_CommandFactory::Setup_Complete(pCommand, !bFailed);
		MOG_ControllerSystem::GetCommandManager()->SendToServer(complete);

		// Clear the project's logged in ComputerName
		MOG_ControllerProject::LoginComputer(NULL);

		// Abort our thread
		MOG_CommandManager::AbortThread();
	}
}

bool MOG_CommandSlave::RemoveAssetFromProject_RestampAnyCollidingAssets(MOG_ControllerAsset* asset, String* jobLabel, String* validSlaves)
{
	bool bRestamped = false;
	ArrayList* restampAssets = new ArrayList();

	// Make sure we have a valid asset?
	if (asset)
	{
		// Get the asset's properties
		MOG_Properties* assetProperties = asset->GetProperties();
		// Does this asset need to have it's files packaged?
		if (assetProperties->IsPackagedAsset)
		{
			// Get info from the asset
			String* assetName = asset->GetAssetFilename()->GetAssetFullName();
			String* assetVersion = asset->GetAssetFilename()->GetVersionTimeStamp();
			String* assetPlatform = asset->GetAssetFilename()->GetAssetPlatform();

			// Make sure this asset's platform is still valid in the project?
			if (MOG_ControllerProject::IsValidPlatform(assetPlatform))
			{
				ArrayList* packageAssignments = asset->GetProperties()->GetPackages();

				// Walk through all the listed packages
				for (int packageIndex = 0; packageIndex < packageAssignments->Count; packageIndex++)
				{
					MOG_Property* packageAssignmentProperty = __try_cast<MOG_Property*>(packageAssignments->Item[packageIndex]);

					// Build the package related tokens
					MOG_Filename* packageName = new MOG_Filename(MOG_ControllerPackage::GetPackageName(packageAssignmentProperty->mPropertyKey));
					String* packageGroups = MOG_ControllerPackage::GetPackageGroups(packageAssignmentProperty->mPropertyKey);
					String* packageObjects = MOG_ControllerPackage::GetPackageObjects(packageAssignmentProperty->mPropertyKey);

					// Add this packageName to our list of packageNames
					ArrayList* packageNames = new ArrayList();
					// Check if packagePlatform is the 'All'?
					if (String::Compare(packageName->GetAssetPlatform(), S"All", true) == 0)
					{
						bool bMissingPlatformSpecificPackage = false;

						// Check if any platform specific packages exist within the project?
						String* projectPlatformNames[] = MOG_ControllerProject::GetProject()->GetPlatformNames();
						for (int p = 0; p < projectPlatformNames->Count; p++)
						{
							// Get the platform specific asset for this platform?
							MOG_Filename* platformSpecificAssetFilename = MOG_ControllerProject::GetPlatformSpecificAsset(packageName, projectPlatformNames[p]);
							if (platformSpecificAssetFilename)
							{
								// Add this platform specific package to our list of packageNames
								packageNames->Add(platformSpecificAssetFilename);
							}
							else
							{
								// Flag that we were missing a platform specific package...meaning we should still package the all
								bMissingPlatformSpecificPackage = true;
							}
						}

						// Check if we were missing a platform specific package?
						if (bMissingPlatformSpecificPackage)
						{
							// Being that there is a missing platform-specific package...looks like we need to package this 'All'
							packageNames->Add(packageName);
						}
					}
					else
					{
						// Always add the package if it isn't an all
						packageNames->Add(packageName);
					}

					// Process each packageFilename
					for (int p = 0; p < packageNames->Count; p++)
					{
						packageName = dynamic_cast<MOG_Filename*>(packageNames->Item[p]);

						// Make sure this package's platform is valid for this asset?
						if (MOG_ControllerProject::IsPlatformValidForAsset(asset->GetAssetFilename(), packageName->GetAssetPlatform()))
						{
							// Make sure this Package exists in our project?
							if (MOG_ControllerProject::DoesAssetExists(packageName))
							{
								// Get all the possible assets that might map to this package object (including any assets mapped to a specific package object)?
								String* packageObject = asset->GetAssetFilename()->GetAssetLabel();
								MOG_Property* relationshipProperty = MOG_PropertyFactory::MOG_Relationships::New_PackageAssignment(packageName->GetAssetFullName(), packageGroups, packageObjects);
								ArrayList* mappedAssets = MOG_ControllerProject::MapPackageObjectToAssetNames(packageObject, relationshipProperty);
								// Check if this package has multiple assets mapping to the same package object (including any assets mapped to a specific package object)?
								if (mappedAssets->Count > 1)
								{
									// Check if this package should rebuild its package objects whenever one is removed
									if (true)
									{
										// Restamp all the shared package objects
										for (int a = 0; a < mappedAssets->Count; a++)
										{
											MOG_Filename* mappedAsset = dynamic_cast<MOG_Filename*>(mappedAssets->Item[a]);

											// Ignore the asset we just scheduled for removal
											if (String::Compare(mappedAsset->GetAssetFullName(), asset->GetAssetFilename()->GetAssetFullName(), true) == 0)
											{
												continue;
											}

											// Obtain the correct revision number to use for this mapped asset
											String* mappedAssetVersion = mappedAsset->GetVersionTimeStamp();
											if (mappedAssetVersion->Length == 0)
											{
												// Use the current timestamp for this asset
												mappedAssetVersion = MOG_ControllerProject::GetAssetCurrentVersionTimeStamp(mappedAsset);
											}

											// Add this asset for restamping...if not already added
											if (!restampAssets->Contains(mappedAsset))
											{
												restampAssets->Add(mappedAsset);
											}
										}
									}
								}
							}
						}
					}
				}

				// Restamp each asset we determined needs to be restamped
				for (int i = 0; i < restampAssets->Count; i++)
				{
					MOG_Filename* restampAssetFilename = dynamic_cast<MOG_Filename*>(restampAssets->Item[i]);
					MOG_Filename* blessedAssetFilename = MOG_ControllerRepository::GetAssetBlessedVersionPath(restampAssetFilename, restampAssetFilename->GetVersionTimeStamp());

					// Open the asset...use a try/catch/finally so we can prematurely return and still close the asset controller properly
					MOG_ControllerAsset *restampAsset = NULL;
					try
					{
						// Open the asset
						restampAsset = MOG_ControllerAsset::OpenAsset(blessedAssetFilename);
						if (restampAsset)
						{
							// Schedule this mappedAsset to be restamped in it's packages
							MOG_ControllerPackageMergeNetwork::ScheduleAssetPackageCommands(restampAsset, MOG_PACKAGECOMMAND_TYPE::MOG_PackageCommand_AddAsset, jobLabel, validSlaves, false);
							bRestamped = true;
						}
					}
					__finally
					{
						if (restampAsset)
						{
							restampAsset->Close();
						}
					}
				}
			}
		}
	}

	// Indicate whether anything was restamped
	return bRestamped;
}

bool MOG_CommandSlave::Command_NetworkPackageMerge(MOG_Command *pCommand)
{
	// Check if we are missing the Database?
	if (!MOG_ControllerSystem::GetDB())
	{
		// Never process this command w/o the database!
		return false;
	}

	// Check if we are free to start a SlaveTask thread?
	if (!gThread)
	{
		// Always re-initialize the thread variables
		InitializeThreadVariables(pCommand, mCommandLineHideWindow);

		// Start a new thread so we can continue to process incomming commands
		gThread = new Thread(new ThreadStart(0, Thread_Process_NetworkPackageMerge));
		gThread->Start();

		// Always eat the command when we successfully start a new thread
		return true;
	}

	return false;
}


void MOG_CommandSlave::Thread_Process_NetworkPackageMerge()
{
	MOG_Command *pCommand = gCommand;

	try
	{
		// Log into the server using the platform and user specified in the command
		MOG_ControllerProject::LoginProject(pCommand->GetProject(), pCommand->GetBranch());
		MOG_ControllerProject::LoginUser(pCommand->GetUserName());
		MOG_ControllerProject::LoginComputer(pCommand->GetComputerName());

		// Record this command in the slave's output log
		String *message = FormatSlaveLogCommandComment(pCommand, S"");
		MOG_ControllerSystem::GetCommandManager()->AppendMessageToOutputLog(message);

		// Initialize our temporary WorkspaceDirectory for the network package merge
		String *workspaceDirectory = String::Concat(S"C:\\TEMP\\MOG\\", pCommand->GetProject(), S"\\", pCommand->GetBranch(), S"\\", pCommand->GetPlatform());
		MOG_ControllerSyncData *syncData = new MOG_ControllerSyncData(workspaceDirectory, pCommand->GetProject(), pCommand->GetBranch(), pCommand->GetPlatform(), S"Admin");
		MOG_ControllerProject::SetCurrentSyncDataController(syncData);

		// Perform a NetworkPackageMerge and get the affected packages
		bool bRejectAssets = (pCommand->GetOptions()->IndexOf(S"|RejectAssets|", StringComparison::CurrentCultureIgnoreCase) != -1) ? true : false;
		MOG_ControllerPackageMergeNetwork *networkPackageMerge = new MOG_ControllerPackageMergeNetwork(bRejectAssets);
		networkPackageMerge->DoPackageEvent(pCommand->GetAssetFilename(), pCommand->GetJobLabel(), pCommand->GetBranch(), workspaceDirectory, pCommand->GetPlatform(), pCommand->GetValidSlaves(), NULL);
	}
	catch (Exception *e)
	{
		String *message = String::Concat(	S"The Slave ecountered an Exception while working on AssetName:", pCommand->GetAssetFilename()->GetAssetFullName(), S"\n",
											e->Message);
		MOG_Report::ReportMessage(S"Bless/Package", message, e->StackTrace, MOG_ALERT_LEVEL::CRITICAL);
	}
	__finally
	{
		// Send back the TaskComplete Command
		MOG_Command *complete =  MOG_CommandFactory::Setup_Complete(pCommand, true);
		MOG_ControllerSystem::GetCommandManager()->SendToServer(complete);

		// Clear the project's logged in ComputerName
		MOG_ControllerProject::LoginComputer(NULL);

		// Abort our thread
		MOG_CommandManager::AbortThread();
	}
}


bool MOG_CommandSlave::Command_NetworkPackageRebuild(MOG_Command *pCommand)
{
	// Check if we are missing the Database?
	if (!MOG_ControllerSystem::GetDB())
	{
		// Never process this command w/o the database!
		return false;
	}

	// Check if we are free to start a SlaveTask thread?
	if (!gThread)
	{
		// Always re-initialize the thread variables
		InitializeThreadVariables(pCommand, mCommandLineHideWindow);

		// Start a new thread so we can continue to process incomming commands
		gThread = new Thread(new ThreadStart(0, Thread_Process_PackageRebuild));
		gThread->Start();

		// Always eat the command when we successfully start a new thread
		return true;
	}

	return false;
}


void MOG_CommandSlave::Thread_Process_PackageRebuild()
{
	MOG_Command *pCommand = gCommand;

	try
	{
		// Log into the server using the platform and user specified in the command
		MOG_ControllerProject::LoginProject(pCommand->GetProject(), pCommand->GetBranch());
		MOG_ControllerProject::LoginUser(pCommand->GetUserName());
		MOG_ControllerProject::LoginComputer(pCommand->GetComputerName());

		// Record this command in the slave's output log
		String *message = FormatSlaveLogCommandComment(pCommand, S"");
		MOG_ControllerSystem::GetCommandManager()->AppendMessageToOutputLog(message);

		// Initialize our temporary WorkspaceDirectory for the network package merge
		String *workspaceDirectory = String::Concat(S"C:\\TEMP\\MOG\\", pCommand->GetProject(), S"\\", pCommand->GetBranch(), S"\\", pCommand->GetPlatform());
		MOG_ControllerSyncData *syncData = new MOG_ControllerSyncData(workspaceDirectory, pCommand->GetProject(), pCommand->GetBranch(), pCommand->GetPlatform(), S"Admin");
		MOG_ControllerProject::SetCurrentSyncDataController(syncData);

		// Perform a NetworkPackageMerge and get the affected packages
		MOG_ControllerPackageRebuildNetwork *networkPackageMerge = new MOG_ControllerPackageRebuildNetwork();
		networkPackageMerge->DoPackageEvent(pCommand->GetAssetFilename(), pCommand->GetJobLabel(), pCommand->GetBranch(), workspaceDirectory, pCommand->GetPlatform(), pCommand->GetValidSlaves(), NULL);
	}
	catch (Exception *e)
	{
		String *message = String::Concat(	S"The Slave ecountered an Exception while working on AssetName:", pCommand->GetAssetFilename()->GetAssetFullName(), S"\n",
											e->Message);
		MOG_Report::ReportMessage(S"Bless/Package", message, e->StackTrace, MOG_ALERT_LEVEL::CRITICAL);
	}
	__finally
	{
		// Send back the TaskComplete Command
		MOG_Command *complete =  MOG_CommandFactory::Setup_Complete(pCommand, true);
		MOG_ControllerSystem::GetCommandManager()->SendToServer(complete);

		// Clear the project's logged in ComputerName
		MOG_ControllerProject::LoginComputer(NULL);

		// Abort our thread
		MOG_CommandManager::AbortThread();
	}
}


bool MOG_CommandSlave::Command_Post(MOG_Command *pCommand)
{
	// Check if we are missing the Database?
	if (!MOG_ControllerSystem::GetDB())
	{
		// Never process this command w/o the database!
		return false;
	}

	// Check if we are free to start a SlaveTask thread?
	if (!gThread)
	{
		// Always re-initialize the thread variables
		InitializeThreadVariables(pCommand, mCommandLineHideWindow);

		// Start a new thread so we can continue to process incomming commands
		gThread = new Thread(new ThreadStart(0, Thread_Process_Post));
		gThread->Start();

		// Always eat the command when we successfully start a new thread
		return true;
	}

	return false;
}


void MOG_CommandSlave::Thread_Process_Post()
{
	MOG_Command *pCommand = gCommand;

	try
	{

	bool skipArchiving = false;

	// Check if this bless has requested we skip the AssetArchiving?
	if (pCommand->GetOptions()->IndexOf(S"|SkipArchiving|", StringComparison::CurrentCultureIgnoreCase) != -1)
	{
		skipArchiving = true;
	}

	// Perform the post?
	if (!MOG_ControllerProject::PostAssets(pCommand->GetProject(), pCommand->GetBranch(), pCommand->GetJobLabel(), skipArchiving))
	{
		String *message = String::Concat(	S"Slave was unable to perform the post command\n",
											S"The Database could be down.");
		MOG_Report::ReportMessage(S"Bless/Post", message, S"", MOG_ALERT_LEVEL::ERROR);
	}

	}
	catch (Exception *e)
	{
		String *message = String::Concat(	S"The Slave ecountered an Exception while working on AssetName:", pCommand->GetAssetFilename()->GetAssetFullName(), S"\n",
											e->Message);
		MOG_Report::ReportMessage(S"Bless/Post", message, e->StackTrace, MOG_ALERT_LEVEL::CRITICAL);
	}
	__finally
	{
		// Send back the PostComplete Command to free our slave of this task
		MOG_Command *postComplete = MOG_CommandFactory::Setup_Complete(pCommand, true);
		MOG_ControllerSystem::GetCommandManager()->SendToServer(postComplete);

		// Abort our thread
		MOG_CommandManager::AbortThread();
	}
}


bool MOG_CommandSlave::Command_ScheduleArchive(MOG_Command *pCommand)
{
	// Check if we are missing the Database?
	if (!MOG_ControllerSystem::GetDB())
	{
		// Never process this command w/o the database!
		return false;
	}

	try
	{
		// Log into the server using the project/user specified in the command
		MOG_ControllerProject::LoginProject(pCommand->GetProject(), pCommand->GetBranch());
		MOG_ControllerProject::LoginUser(pCommand->GetUserName());
		MOG_ControllerProject::LoginComputer(pCommand->GetComputerName());

		// Record this command in the slave's output log
		String *message = FormatSlaveLogCommandComment(pCommand, S"");
		MOG_ControllerSystem::GetCommandManager()->AppendMessageToOutputLog(message);

		// Request from the Database all the unused revisions of this asset
		ArrayList *unusedRevisions = MOG_DBAssetAPI::GetAllUnusedAssetRevisions(pCommand->GetAssetFilename());
		if (unusedRevisions != NULL)
		{
			// Get the asset's properties
			MOG_Properties *pProperties = new MOG_Properties(pCommand->GetAssetFilename());

			// Get the desired number of unused revisions for this asset
			String *numRevisions = pProperties->UnReferencedRevisionHistory;
			int num = 0;
			try
			{
				num = Convert::ToInt32(numRevisions);
			}
			catch (...)
			{
			}

			// Check if it is set to zero...meaning infinite
			if (num > 0)
			{
				// Check if we have exceeded the desired number of unreferenced revisions
				if (unusedRevisions->Count > num)
				{
					// Loop through the remaining unused revisions
					for (int i = num; i < unusedRevisions->Count; i++)
					{
						String *unusedRevision = __try_cast<String *>(unusedRevisions->Item[i]);

						// Make sure this revision is really unused?
						if (MOG_DBAssetAPI::GetAssetRevisionReferences(pCommand->GetAssetFilename(), unusedRevision, false)->Count == 0)
						{
							// Move this asset over to the archive repository
							MOG_Filename *blessedAssetFilename = MOG_ControllerRepository::GetAssetBlessedVersionPath(pCommand->GetAssetFilename(), unusedRevision);
							MOG_Filename *archiveAssetFilename = MOG_ControllerRepository::GetAssetArchivedVersionPath(pCommand->GetAssetFilename(), unusedRevision);
							if (DosUtils::DirectoryMoveFast(blessedAssetFilename->GetEncodedFilename(), archiveAssetFilename->GetEncodedFilename(), true))
							{
								// Remove the revision from the database
								if (MOG_DBAssetAPI::RemoveAssetVersion(pCommand->GetAssetFilename(), unusedRevision))
								{
								}
								else
								{
									String *message = String::Concat(	S"Slave was unable to remove the ", unusedRevision, S" revision of ", pCommand->GetAssetFilename()->GetAssetFullName(), S"\n",
																		S"The Database could be down.");
									MOG_Report::ReportMessage(S"Bless/Archive", message, "", MOG_ALERT_LEVEL::ERROR);
								}
							}
						}
					}
				}
			}
		}
		else
		{
			String *message = String::Concat(	S"Slave was unable to receive the unsued revisions of ", pCommand->GetAssetFilename()->GetAssetFullName(), S" from the database.\n",
												S"The Database could be down.");
			MOG_Report::ReportMessage(S"Bless/Archive", message, S"", MOG_ALERT_LEVEL::ERROR);
		}
	}
	catch (Exception *e)
	{
		String *message = String::Concat(	S"The Slave ecountered an Exception while attempting to archive '", pCommand->GetAssetFilename()->GetAssetFullName(), S"'.\n",
											e->Message);
		MOG_Report::ReportMessage(S"Bless/Post", message, e->StackTrace, MOG_ALERT_LEVEL::CRITICAL);
	}
	__finally
	{
		// Indicate to the server we have finished processing this command
		MOG_Command *scheduleArchiveComplete =  MOG_CommandFactory::Setup_Complete(pCommand, true);
		SendToServer(scheduleArchiveComplete);

		// Clear the project's logged in ComputerName
		MOG_ControllerProject::LoginComputer(NULL);
	}

	return true;
}

bool MOG_CommandSlave::Command_Archive(MOG_Command *pCommand)
{
	// Send back the ArchiveComplete Command
	MOG_Command *archiveComplete = MOG_CommandFactory::Setup_Complete(pCommand, true);
	return SendToServer(archiveComplete);
}


bool MOG_CommandSlave::Command_LockReadRequest(MOG_Command *pCommand)
{
	bool locked = false;

	// Keep trying until we actually process the lock
	while (!locked)
	{
		// Send the command to the server
		locked = SendToServerBlocking(pCommand);
		if (!locked)
		{
			Thread *currentThread = Thread::CurrentThread;
			currentThread->Sleep(500);
		}
	}

	return locked;
}


bool MOG_CommandSlave::Command_LockReadRelease(MOG_Command *pCommand)
{
	bool unlocked = false;

//	// Send the command to the server
//	return SendToServer(pCommand);
	// Keep trying until we actually process the unlock
	while (!unlocked)
	{
		// Send the command to the server
		unlocked = SendToServerBlocking(pCommand);
		if (!unlocked)
		{
			Thread *currentThread = Thread::CurrentThread;
			currentThread->Sleep(500);
		}
	}

	return unlocked;
}


bool MOG_CommandSlave::Command_LockWriteRequest(MOG_Command *pCommand)
{
	bool locked = false;

	// Keep trying until we actually process the lock
	while (!locked)
	{
		// Send the command to the server
		locked = SendToServerBlocking(pCommand);
		if (!locked)
		{
			Thread *currentThread = Thread::CurrentThread;
			currentThread->Sleep(500);
		}
	}

	return locked;
}


bool MOG_CommandSlave::Command_LockWriteRelease(MOG_Command *pCommand)
{
	bool unlocked = false;

//	// Send the command to the server
//	return SendToServer(pCommand);
	// Keep trying until we actually process the unlock
	while (!unlocked)
	{
		// Send the command to the server
		unlocked = SendToServerBlocking(pCommand);
		if (!unlocked)
		{
			Thread *currentThread = Thread::CurrentThread;
			currentThread->Sleep(500);
		}
	}

	return unlocked;
}


bool MOG_CommandSlave::Command_BuildFull(MOG_Command *pCommand)
{
	// Always eat this command
	return true;
}


bool MOG_CommandSlave::Command_Build(MOG_Command *pCommand)
{
	// Check if we are free to start a new thread?
	if (!gThread)
	{
		// Always re-initialize the thread variables
		InitializeThreadVariables(pCommand, mCommandLineHideWindow);

		// Start a new thread so we can continue to process incomming commands
		gThread = new Thread(new ThreadStart(0, Thread_Process_Build));
		gThread->Start();

		// Always eat the command when we successfully start a new thread
		return true;
	}

	return false;
}


void MOG_CommandSlave::Thread_Process_Build()
{
	MOG_Command *pCommand = gCommand;

	try
	{
		// Log into the server using the platform and user specified in the command
		MOG_ControllerProject::LoginProject(pCommand->GetProject(), pCommand->GetBranch());
		MOG_ControllerProject::LoginUser(pCommand->GetUserName());
		MOG_ControllerProject::LoginComputer(pCommand->GetComputerName());

		// Record this command in the slave's output log
		String *message = FormatSlaveLogCommandComment(pCommand, S"");
		MOG_ControllerSystem::GetCommandManager()->AppendMessageToOutputLog(message);

		// Initialize the environment variables from the command
		ArrayList *environment = new ArrayList;
		if (pCommand->GetDescription()->Length > 0)
		{
			String* delimStr = S",";
			Char delimiter[] = delimStr->ToCharArray();
			String* vars[] = pCommand->GetDescription()->Trim()->Split(delimiter);
			for (int i = 0; i < vars->Length; i++)
			{
				DosUtils::EnvironmentList_AddVariable(environment, vars[i]);
			}
		}

		// Make sure we have a valid BuildTool?
		if (pCommand->GetAssetFilename()->GetEncodedFilename()->Length)
		{
			// Initialize the output log
			String *output = "";
			// Get the command line executable
			String *buildPath = pCommand->GetAssetFilename()->GetPath();
			String *buildTool = pCommand->GetAssetFilename()->GetEncodedFilename();

			// Run the buildTool never hiding it's window
			int errorCode = DosUtils::SpawnDosCommand(buildPath, buildTool, "", environment, &output, false);
			if (errorCode != 0)
			{
				String *message = String::Concat(	S"The ", DosUtils::PathGetFileName(buildTool), S" BuildTool returned ErrorCode ", errorCode.ToString(), S"\n",
													S"Click the details button to view the BuildTool log.\n",
													S"Confirm this seriousness of this error with your MOG Administrator.");
				MOG_Report::ReportMessage(S"Build", message, output, MOG_ALERT_LEVEL::ERROR);
			}
		}
		else
		{
			String *message = String::Concat(	S"Invalid BuildTool specified.\n",
												S"The Build did not launch.");
			MOG_Report::ReportMessage(S"Build", message, S"", MOG_ALERT_LEVEL::ERROR);
		}
	}
	catch (Exception *e)
	{
		String *message = String::Concat(	S"The Slave ecountered an Exception while working on AssetName:", pCommand->GetAssetFilename()->GetAssetFullName(), S"\n",
											e->Message);
		MOG_Report::ReportMessage(S"Build", message, e->StackTrace, MOG_ALERT_LEVEL::CRITICAL);
	}
	__finally
	{
		// Send back the BuildComplete command
		MOG_Command *buildComplete = MOG_CommandFactory::Setup_Complete(pCommand, true);
		MOG_ControllerSystem::GetCommandManager()->SendToServer(buildComplete);

		// Clear the project's logged in ComputerName
		MOG_ControllerProject::LoginComputer(NULL);

		// Abort our thread
		MOG_CommandManager::AbortThread();
	}
}


bool MOG_CommandSlave::RemoveAssetFromProject_RemoveAssetFromBranch(MOG_Filename *assetFilename, String *jobLabel)
{
	bool bFailed = false;

	// Remove this asset from the database
	if (MOG_DBAssetAPI::RemoveAssetFromBranch(assetFilename, S""))
	{
		// Schedule this asset for archive
		MOG_Command *command = MOG_CommandFactory::Setup_ScheduleArchive(assetFilename, jobLabel);
		MOG_ControllerSystem::GetCommandManager()->SendToServer(command);
	}
	else
	{
		// Inform user of this error
		String *message = String::Concat(	S"Unable to remove the asset from the branch.\n",
											S"ASSET: ", assetFilename->GetAssetFullName(), S"\n",
											S"The Database could be down.");
		MOG_Report::ReportMessage(S"Remove Asset From Project Failed", message, S"", MOG_ALERT_LEVEL::ERROR);
		// Indicate we failed
		bFailed = true;
	}

	if (!bFailed)
	{
		return true;
	}
	return false;
}


bool MOG_CommandSlave::RemoveAssetFromProject_RemoveAssetRevision(MOG_Filename *assetFilename, String *removeRevision, bool bReportErrors)
{
	bool bFailed = false;

	// Make sure this revision still exists?
	int assetVersionID = MOG_DBAssetAPI::GetAssetVersionID(assetFilename, removeRevision);
	if (assetVersionID)
	{
		// Move this asset over to the archive repository
		MOG_Filename *tempBlessedAssetFilename = MOG_ControllerRepository::GetAssetBlessedVersionPath(assetFilename, removeRevision);
		MOG_Filename *tempArchiveAssetFilename = MOG_ControllerRepository::GetAssetArchivedVersionPath(assetFilename, removeRevision);

		// Attempt to remove this asset's associated packages...
		ArrayList *packages = MOG_DBAssetAPI::GetPackageReferencesForAsset(assetVersionID);
		for( int i = 0; i < packages->Count; i++ )
		{
			MOG_Filename *packageName = __try_cast<MOG_Filename*>(packages->Item[i]);

			// Attempt to remove each package
			RemoveAssetFromProject_RemoveAssetRevision(packageName, packageName->GetVersionTimeStamp(), false);
		}

		// Make sure this asset's revision is not in use?
// JohnRen - Changed because all we really want to check is branches and Packages
//		ArrayList *references = MOG_DBAssetAPI::GetAssetRevisionReferences(tempArchiveAssetFilename, tempArchiveAssetFilename->GetVersionTimeStamp(), true);
		ArrayList *references = new ArrayList();
		references->AddRange(MOG_DBAssetAPI::GetAssetRevisionReferencesInBranches(tempArchiveAssetFilename, tempArchiveAssetFilename->GetVersionTimeStamp()));
		references->AddRange(MOG_DBAssetAPI::GetAssetRevisionReferencesInPackages(tempArchiveAssetFilename, tempArchiveAssetFilename->GetVersionTimeStamp()));
		if (references->Count == 0)
		{
			// Remove the asset revision from PendingPackageCommands
			MOG_DBPackageCommandAPI::RemovePackageCommand(tempArchiveAssetFilename);
			// Remove the asset revision from PendingPostCommands
			MOG_DBPostCommandAPI::RemovePost(tempArchiveAssetFilename);

			// Remove the revision from the database
			if (MOG_DBAssetAPI::RemoveAssetVersion(tempArchiveAssetFilename, tempArchiveAssetFilename->GetVersionTimeStamp()))
			{
				// Move the directory over to the archive
				if (DosUtils::DirectoryMoveFast(tempBlessedAssetFilename->GetEncodedFilename(), tempArchiveAssetFilename->GetEncodedFilename(), true))
				{
				}
				else
				{
					// Not sure what to do here since it was already removed from the database...
				}
			}
			else
			{
				if (bReportErrors)
				{
					// Inform user of this error
					String *message = String::Concat(	S"Unable to remove the '", removeRevision, S"' revision of '", tempBlessedAssetFilename->GetAssetFullName(), S"'.\n",
														S"The Database could be down.");
					MOG_Report::ReportMessage(S"Remove Asset From Project Failed", message, S"", MOG_ALERT_LEVEL::ERROR);
				}
				// Indicate we failed
				bFailed = true;
			}
		}
		else
		{
			if (bReportErrors)
			{
				// Inform user of this error
				String *message = String::Concat(	S"Unable to archive this asset revision because it is still being referenced.\n",
													S"ASSET: ", tempArchiveAssetFilename->GetAssetFullName(), S"\n",
													S"REVISION: ", tempArchiveAssetFilename->GetVersionTimeStamp(), S"\n",
													S"REFERENCES: \n");
				// Show the user what references kept us from being able to remove this revision
				for (int i = 0; i < references->Count; i++)
				{
					String *reference = __try_cast<String *>(references->Item[i]);
					message = String::Concat(message, S"     ", reference, S"\n");
				}

				MOG_Report::ReportMessage(S"Remove Asset From Project Failed", message, S"", MOG_ALERT_LEVEL::ALERT);
			}
			// Indicate we failed
			bFailed = true;
		}
	}

	if (!bFailed)
	{
		return true;
	}
	return false;
}


bool MOG_CommandSlave::RemoveAssetFromProject_RemoveAllAssetRevisions(MOG_Filename *assetFilename)
{
	bool bFailed = false;
	ArrayList *removeList = new ArrayList();

	// Attempt to remove each revisions of this asset
	ArrayList *existingRevisions = MOG_DBAssetAPI::GetAllAssetRevisions(assetFilename);
	for (int i = 0; i < existingRevisions->Count; i++)
	{
		String *removeRevision = __try_cast<String *>(existingRevisions->Item[i]);

		// Remove this specific revision
		if (!RemoveAssetFromProject_RemoveAssetRevision(assetFilename, removeRevision, true))
		{
			bFailed = true;
		}
	}

	if (!bFailed)
	{
		return true;
	}
	return false;
}


bool MOG_CommandSlave::RemoveAssetFromProject_RemoveAssetName(MOG_Filename *assetFilename)
{
	bool bFailed = false;

	// Check if there are no more remaining revisions of this asset?
	ArrayList *remainingRevisions = MOG_DBAssetAPI::GetAllAssetRevisions(assetFilename);
	if (remainingRevisions->Count == 0)
	{
		// Remove the asset's name
		if (MOG_DBAssetAPI::RemoveAssetName(assetFilename))
		{
			// Make sure to cleanup the asset's old directory in the repository
			String *blessedPath = MOG_ControllerRepository::GetAssetBlessedPath(assetFilename)->GetEncodedFilename();
			DosUtils::DirectoryDeleteFast(blessedPath);
		}
		else
		{
			// Inform user of this error
			String *message = String::Concat(	S"Unable to remove '", assetFilename->GetAssetFullName(), S"'.\n",
												S"The Database could be down.");
			MOG_Report::ReportMessage(S"Remove Asset From Project Failed", message, S"", MOG_ALERT_LEVEL::ERROR);
			// Indicate we failed
			bFailed = true;
		}
	}

	if (!bFailed)
	{
		return true;
	}
	return false;
}


