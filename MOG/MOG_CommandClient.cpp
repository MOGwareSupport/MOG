//--------------------------------------------------------------------------------
//	MOG_CommandClient.cpp
//	
//	
//--------------------------------------------------------------------------------

#include "StdAfx.h"

#include "MOG_Define.h"
#include "MOG_StringUtils.h"
#include "MOG_Main.h"
#include "MOG_DosUtils.h"
#include "MOG_Time.h"
#include "MOG_Ini.h"
#include "MOG_ControllerAsset.h"
#include "MOG_EnvironmentVars.h"
#include "MOG_ControllerPackageMergeLocal.h"
#include "MOG_ControllerProject.h"
#include "MOG_ControllerSystem.h"
#include "MOG_ControllerInbox.h"
#include "MOG_ControllerRepository.h"
#include "MOG_ControllerSyncData.h"
#include "MOG_DBSyncedDataAPI.h"
#include "MOG_DBPackageAPI.h"
#include "MOG_Tokens.h"
#include "MOG_SystemUtilities.h"
#include "MOG_Prompt.h"
#include "MOG_Progress.h"
#include "MOG_CommandFactory.h"

#include "MOG_CommandClient.h"




MOG_CommandClient::MOG_CommandClient()
{
	mNetwork = new MOG_NetworkClient;
	mLockTracker = new MOG_LockTracker(this, true);
}

bool MOG_CommandClient::RegisterWithServer(void)
{
	MOG_Command *command = MOG_CommandFactory::Setup_RegisterClient(MOG_Main::GetName(), MOG_Main::GetAssociatedWorkspacePath());
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

	// Inform the server about our active views
	NotifyServerOfActiveViews(	MOG_ControllerProject::GetActiveTabName(),
								MOG_ControllerProject::GetActiveUserName(), 
								MOG_ControllerProject::GetPlatformName(), 
								MOG_ControllerProject::GetWorkspaceDirectory());
	return true;
}


bool MOG_CommandClient::NotifyServerOfActiveViews(String *tabName, String *userName, String *platformName, String *workspaceDirectory)
{
	// Register Computer
	MOG_Command *newCommand = MOG_CommandFactory::Setup_ActiveViews(tabName, userName, platformName, workspaceDirectory);
	MOG_ControllerSystem::GetCommandManager()->SendToServer(newCommand);
	return true;
}


bool MOG_CommandClient::AddDeletePackageCommand(String *packageFilename, String *platform, String *packages[])
{
	MOG_ASSERT_THROW(packageFilename->Length, MOG_Exception::MOG_EXCEPTION_InvalidData, "MOG_ControllerAsset::PackageAssetFiles - Specified 'packageFilename' Invalid");

	bool bPackaged = false;

	// Get the current Project?
	MOG_Project *pProject = MOG_ControllerProject::GetProject();
	if (pProject)
	{
		// Open this platform's package file
		MOG_Ini *packageFile = new MOG_Ini();
		if (packageFile->Open(packageFilename, FileShare::ReadWrite))
		{
			// Always make sure we re-create the platform key in this package file
			packageFile->PutString("PACKAGE", "Platform", platform);

			// Walk through all the listed packages
			for (int p = 0; p < packages->Count; p++)
			{
				String *packageCommand = String::Concat(S"DeletePackage PackageFile(", packages[p], S")");
				packageFile->PutString("Commands", packageCommand, MOG_ControllerProject::GetUser()->GetUserName());
			}

			// Close the file
			packageFile->Close();
			packageFile = NULL;

			bPackaged = true;
		}
	}

	return bPackaged;
}


bool MOG_CommandClient::CommandExecute(MOG_Command *pCommand)
{
	bool processed = false;

	switch (pCommand->GetCommandType())
	{
		case MOG_COMMAND_ShutdownClient:
			processed = Command_ShutdownClient(pCommand);
			break;
		case MOG_COMMAND_ShutdownSlave:
			processed = Command_ShutdownSlave(pCommand);
			break;
		case MOG_COMMAND_RegisterEditor:
			processed = Command_RegisterEditor(pCommand);
			break;
		case MOG_COMMAND_ShutdownEditor:
			processed = Command_ShutdownEditor(pCommand);
			break;
		case MOG_COMMAND_LoginProject:
			processed = Command_LoginProject(pCommand);
			break;
		case MOG_COMMAND_LoginUser:
			processed = Command_LoginUser(pCommand);
			break;
		case MOG_COMMAND_AssetRipRequest:
			processed = Command_AssetRipRequest(pCommand);
			break;
		case MOG_COMMAND_Bless:
			processed = Command_Bless(pCommand);
			break;
		case MOG_COMMAND_LocalPackageMerge:
			processed = Command_LocalPackageMerge(pCommand);
			break;
		case MOG_COMMAND_LocalPackageRebuild:
			processed = Command_LocalPackageRebuild(pCommand);
			break;
		case MOG_COMMAND_LockReadRequest:
		case MOG_COMMAND_LockReadRelease:
		case MOG_COMMAND_LockWriteRequest:
		case MOG_COMMAND_LockWriteRelease:
		case MOG_COMMAND_LockWriteQuery:
		case MOG_COMMAND_LockReadQuery:
		case MOG_COMMAND_LockPersistentNotify:
			processed = mLockTracker->CommandExecute(pCommand);
			break;
		case MOG_COMMAND_NetworkBroadcast:
			processed = Command_NetworkBroadcast(pCommand);
			break;
		case MOG_COMMAND_InstantMessage:
			processed = Command_InstantMessage(pCommand);
			break;
		case MOG_COMMAND_NotifyActiveCommand:
			processed = Command_NotifyActiveCommand(pCommand);
			break;
		case MOG_COMMAND_NotifyActiveLock:
			processed = Command_NotifyActiveLock(pCommand);
			break;
		case MOG_COMMAND_NotifyActiveConnection:
			processed = Command_NotifyActiveConnection(pCommand);
			break;
		case MOG_COMMAND_AutomatedTesting:
			processed = Command_AutomatedTesting(pCommand);
			break;
		case MOG_COMMAND_Complete:
			processed = Command_Complete(pCommand);
			break;

		default:
			// Allow the parent a chance to process this command
			processed = MOG_CommandManager::CommandExecute(pCommand);
			break;
	}

	return processed;
}


bool MOG_CommandClient::Command_ShutdownClient(MOG_Command *pCommand)
{
	// Shut ourselves down
	MOG_Main::Shutdown();

	return true;
}


bool MOG_CommandClient::Command_ShutdownSlave(MOG_Command *pCommand)
{
	// Always eat this command.
	return true;
}


bool MOG_CommandClient::Command_RegisterEditor(MOG_Command *pCommand)
{
	// Always eat this command.
	return true;
}


bool MOG_CommandClient::Command_ShutdownEditor(MOG_Command *pCommand)
{
	// Always eat this command.
	return true;
}


bool MOG_CommandClient::Command_ConnectionKill(MOG_Command *pCommand)
{
	// Inform our server about us shutting down
	MOG_Command *pShutdownClient = MOG_CommandFactory::Setup_ShutdownClient();
	SendToServer(pShutdownClient);

	// Shut ourselves down
	MOG_CommandManager::Command_ConnectionKill(pCommand);

	// Always eat this command.
	return true;
}


bool MOG_CommandClient::Command_LoginProject(MOG_Command *pCommand)
{
	// Send this command to the server
	return SendToServer(pCommand);
}


bool MOG_CommandClient::Command_LoginUser(MOG_Command *pCommand)
{
	// Send this command to the server
	return SendToServer(pCommand);
}


bool MOG_CommandClient::Command_AssetRipRequest(MOG_Command *pCommand)
{
	// Send this command to the server
	return SendToServer(pCommand);
}


bool MOG_CommandClient::Command_Bless(MOG_Command *pCommand)
{
	// Send this command to the server
	return SendToServer(pCommand);
}


bool MOG_CommandClient::Command_LocalPackageMerge(MOG_Command *pCommand)
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
		// Start a new thread so we can continue to process incomming commands
		gThread = new Thread(new ParameterizedThreadStart(this, &MOG_CommandClient::Thread_Process_LocalPackageMerge));
		gThread->Start(pCommand);

		// Always eat the command when we successfully start a new thread
		return true;
	}

	// We need to wait until the other thread has finished
	return false;
}


void MOG_CommandClient::Thread_Process_LocalPackageMerge(Object *object)
{
	MOG_Command *pCommand = dynamic_cast<MOG_Command *>(object);

	try
	{
		// Perform our local package merge
		MOG_ControllerPackageMergeLocal *localPackageMerge = new MOG_ControllerPackageMergeLocal();
		PackageList *affectedPackages = localPackageMerge->DoPackageEvent(pCommand->GetWorkingDirectory(), pCommand->GetPlatform(), NULL);
		if (affectedPackages->Count)
		{
			return;
		}
	}
	catch(Exception *e)
	{
		MOG_Report::ReportMessage(S"Client/Package Merge", e->Message, e->StackTrace, MOG_ALERT_LEVEL::CRITICAL);
	}
	__finally
	{
		// Send the MOG_COMMAND_PackageComplete command
		MOG_Command *packageComplete = MOG_CommandFactory::Setup_Complete(pCommand, true);
		SendToServer(packageComplete);

		// Abort our thread
		MOG_CommandManager::AbortThread();
	}
}


bool MOG_CommandClient::Command_LocalPackageRebuild(MOG_Command *pCommand)
{
/*	try
	{
		MOG_Filename *matchedAssetFilename = NULL;

		// Initialize the threaded dialog
		gDialogHandle = MOG_Progress::ProgressSetup("Updating Packages", "Please wait while the Packages are being updated.");
		MOG_Progress::ProgressInitGraphic(gDialogHandle);

		// Check if this ia already a valid asset?
		if (pCommand->GetAssetFilename()->GetFilenameType() == MOG_FILENAME_TYPE::MOG_FILENAME_Asset)
		{
			// Use the asset original filename
			matchedAssetFilename = pCommand->GetAssetFilename();
		}
		// Attempt ot match what was specified to an asset name?
		else
		{
			// Check if the specified assetFile includes the working directory?
			String *assetFile = pCommand->GetAssetFilename()->GetFullFilename();
			String *testWorkingDirectory = String::Concat(pCommand->GetWorkingDirectory(), S"\\");
			if (assetFile->ToLower()->StartsWith(testWorkingDirectory->ToLower()))
			{
				// Strip off the preceeding working directory
				assetFile = assetFile->Substring(testWorkingDirectory->Length);
			}
			// Attempt to match this assetFile to a valid asset name?
			ArrayList *assets = MOG_ControllerProject::MapFilenameToAssetName(assetFile, pCommand->GetPlatform());
			if (assets->Count)
			{
				// Always use the first asset name
				matchedAssetFilename = __try_cast<MOG_Filename*>(assets->Item[0]);
				MOG_ControllerProject::MapFilenameToAssetName_WarnAboutAmbiguousMatches(assetFile, assets)
			}
		}

		// Check if we now have a valid asset name?
		if (matchedAssetFilename)
		{
			// Get the appropriate path for this asset given our workspace?
			MOG_ControllerSyncData *syncData = new MOG_ControllerSyncData(pCommand->GetWorkingDirectory(), pCommand->GetProject(), pCommand->GetBranch(), pCommand->GetPlatform(), pCommand->GetUserName());
			MOG_Filename *assetFilename = syncData->LocateAsset_LocalWorkspaceAndRepository(matchedAssetFilename);
			if (assetFilename)
			{
				// Get the properties for this package
				MOG_Properties *pProperties = new MOG_Properties(assetFilename);
				pProperties->SetScope(pCommand->GetPlatform());

				// Get the list of packages that need to be rebuilt
				ArrayList *packageFiles = MOG_ControllerPackage::GetAssociatedPackageList(assetFilename, pProperties, jobLabel);
				if (packageFiles->Count)
				{
					// Loop through all the specified packages
					for (int p = 0; p < packageFiles->Count; p++)
					{
						MOG_PackageMerge_PackageFileInfo *packageFileInfo = __try_cast<MOG_PackageMerge_PackageFileInfo*>(packageFiles->Item[p]);

						// Get the currentVersion for this package given the specified local workspace?
						String *localRevision = MOG_DBSyncedDataAPI::GetLocalAssetVersion(	pCommand->GetComputerName(),
																							pCommand->GetProject(),
																							pCommand->GetPlatform(),
																							pCommand->GetWorkingDirectory(),
																							pCommand->GetUserName(),
																							packageFileInfo->mPackageAssetFilename);
						if (localRevision->Length)
						{
//?	MOG_CommandClient::Command_LocalPackageRebuild - What about the local/unblessed assets when rebuilding local packages?  We should be smart about the list of contained assets to account for the user's local workspace.
							// Get all contained assets in this package
							ArrayList *containedAssets = MOG_DBPackageAPI::GetAllAssetsInPackage(packageFileInfo->mPackageAssetFilename, localRevision);
							if (containedAssets)
							{
								ArrayList *packageMergeTasks = new ArrayList();

								// Schedule all the specified packages to be deleted
								MOG_ControllerPackage::ExecutePackageCommands_DeletePackageFile(packageFileInfo);

								// Scan the contained assets
								for (int a = 0; a < containedAssets->Count; a++)
								{
									MOG_Filename *assetFilename = __try_cast<MOG_Filename*>(containedAssets->Item[a]);

									// Open this asset
									MOG_Filename *blessedAssetFilename = MOG_ControllerRepository::GetAssetBlessedVersionPath(assetFilename, assetFilename->GetVersionTimeStamp());
									MOG_ControllerAsset *asset = MOG_ControllerAsset::OpenAsset(blessedAssetFilename);
									if (asset)
									{
										// Track any potential package merge tasks
										packageMergeTasks = MOG_ControllerPackage::AddPackageTask(packageMergeTasks, packageFileInfo);

										MOG_ControllerPackage::ExecutePackageCommands_AddAsset(asset, pCommand->GetPlatform(), packageFileInfo);
										// Close the asset controller as soon as we are finished with it
										asset->Close();
									}
									else
									{
										// Make sure to inform the Server about this
										String *message = String::Concat(	S"The System was unable to open ", blessedAssetFilename->GetAssetFullName(), S"\n",
																			S"Confirm this asset exists in the MOG Repository.  The rebuilt package will be missing this asset.");
										MOG_Report::ReportMessage(S"RebuildPackage", message, S"", MOG_ALERT_LEVEL::ERROR);
									}
								}

								// Check if we had any package merge tasks?
								if (packageMergeTasks->Count)
								{
									for (int t = 0; t < packageMergeTasks->Count; t++)
									{
										// Send each package merge task to the server for processing
										MOG_Command *packageMergeTask = __try_cast<MOG_Command*>(packageMergeTasks->Item[t]);
										SendToServer(packageMergeTask);
									}
								}
							}
							else
							{
								String *message = String::Concat(	S"There are no assets contained within this package.  There was nothing to rebuild.\n",
																	S"PACKAGEFILE: ", packageFileInfo->mPackageAssetFilename->GetAssetFullName(), S"\n",
																	S"PLATFORM: ", mPlatformName, S"\n");
								MOG_Prompt::PromptMessage(S"RebuildPackage", message, S"", MOG_ALERT_LEVEL::ALERT);
							}
						}
						else
						{
							String *message = String::Concat(	S"System was unable to locate a current revision for package.\n",
																S"PACKAGEFILE: ", packageFileInfo->mPackageAssetFilename->GetAssetFullName());
							MOG_Prompt::PromptMessage(S"RebuildPackage", message, S"", MOG_ALERT_LEVEL::ALERT);
						}
					}
				}
			}
			else
			{
				// Inform the user their local branch needs to be updated
				String *message = String::Concat(	S"Your local workspace has no revision information for '", matchedAssetFilename->GetAssetFullName(), S"'\n",
													S"You may need to update your local workspace.");
				MOG_Prompt::PromptMessage(S"RebuildPackage", message, S"", MOG_ALERT_LEVEL::ALERT);
			}
		}
		else
		{
			// Warn the user we found no matching asset for the specified file
			String *message = String::Concat(	S"MOG was unable to locate a matching asset for the specified item.\n",
												S"ITEM: ", pCommand->GetAssetFilename()->GetAssetFullName(), S"\n",
												S"The desired asset may have been removed from the project.\n");
			MOG_Prompt::PromptMessage(S"RebuildPackage", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
		}
	}
	catch(Exception *e)
	{
		MOG_Report::ReportMessage(S"Client/Package Merge", e->Message, e->StackTrace, MOG_ALERT_LEVEL::CRITICAL);
	}
	__finally
	{
		// Send the MOG_COMMAND_PackageComplete command
		MOG_Command *packageComplete = new MOG_Command();
		packageComplete->Clone(pCommand);
		packageComplete->SetCommandType(MOG_COMMAND_PackageComplete);
		SendToServer(packageComplete);
	}
*/
	// Always eat this command
	return true;
}


bool MOG_CommandClient::Command_NetworkBroadcast(MOG_Command *pCommand)
{
	// Display the broadcasted message
	return MOG_Prompt::PromptMessage(S"Network Broadcast Message", pCommand->GetDescription(), "");
}


bool MOG_CommandClient::Command_InstantMessage(MOG_Command *pCommand)
{
	MOG_ASSERT_THROW(pCommand->GetVersion()->Length, MOG_Exception::MOG_EXCEPTION_InvalidData, "MOG_CommandClient::Command_InstantMessage - This InstantMessage is missing it's handle identifier");

	// If callback exist, call it
	if (mCallbacks && mCallbacks->mCommandCallback)
	{
		// Process the command
		mCallbacks->mCommandCallback->Invoke(pCommand);
	}
	// Always eat this command
	return true;
}


bool MOG_CommandClient::Command_NotifyActiveCommand(MOG_Command *pCommand)
{
	// Always eat this command
	return true;
}


bool MOG_CommandClient::Command_NotifyActiveLock(MOG_Command *pCommand)
{
	// Always eat this command
	return true;
}


bool MOG_CommandClient::Command_NotifyActiveConnection(MOG_Command *pCommand)
{
	// Always eat this command
	return true;
}


bool MOG_CommandClient::Command_AutomatedTesting(MOG_Command *pCommand)
{
	// Check if we should copy the file local?
	bool bCopyFileLocal = (pCommand->GetOptions()->ToLower()->IndexOf(S"CopyFileLocal"->ToLower()) != -1);

	// Get the specified startingExtensionIndex
	int startingExtensionIndex = 0;
	if (pCommand->GetVersion()->Length != 0)
	{
		startingExtensionIndex = Convert::ToInt32(pCommand->GetVeriables());
	}

	// Get the specified duration
	int duration = 60;
	if (pCommand->GetVersion()->Length != 0)
	{
		duration = Convert::ToInt32(pCommand->GetVersion());
	}

	// Initiate the Automated Testing
	MOG_SystemUtilities::AutomatedTesting_Drone(pCommand->GetTitle(), duration, pCommand->GetProject(), pCommand->GetSource(), bCopyFileLocal, startingExtensionIndex);

	// Always eat this command
	return true;
}


bool MOG_CommandClient::Command_Complete(MOG_Command *pCommand)
{
	// Make sure to call our base class
	MOG_CommandManager::Command_Complete(pCommand);
									 
	// Make sure this contains an encapsulated command?
	if (pCommand->GetCommand())
	{
//		// Determin the type of encapsulated command
//		switch(pCommand->GetCommand()->GetCommandType())
//		{
//			default:
//				break;
//		}
	}

	// Indicate that we received and processed the command
	return true;
}

