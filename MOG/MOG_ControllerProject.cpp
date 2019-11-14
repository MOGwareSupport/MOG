//--------------------------------------------------------------------------------
//	MOG_ControllerProject.cpp
//	
//	
//--------------------------------------------------------------------------------

#include "StdAfx.h"

#include "MOG_Define.h"
#include "MOG_DosUtils.h"
#include "MOG_ControllerAsset.h"
#include "MOG_ControllerRepository.h"
#include "MOG_ControllerLibrary.h"
#include "MOG_Tokens.h"
#include "MOG_ControllerSystem.h"
#include "MOG_ControllerInbox.h"
#include "MOG_ControllerPackage.h"
#include "MOG_ControllerPackageMergeNetwork.h"
#include "MOG_DBAssetAPI.h"
#include "MOG_DBInboxAPI.h"
#include "MOG_DBPackageAPI.h"
#include "MOG_DBPostCommandAPI.h"
#include "MOG_DBSyncedDataAPI.h"
#include "MOG_DBPackageCommandAPI.h"
#include "MOG_Progress.h"
#include "MOG_CommandFactory.h"

#include "MOG_ControllerProject.h"

using namespace System::ComponentModel;
using namespace System::Collections::Generic;
using namespace MOG_CoreControls;

bool MOG_ControllerProject::ProjectExists(String* projectName)
{
	ArrayList* projects = MOG_ControllerSystem::GetSystem()->GetProjectNames();

	if (projects)
	{
		for (int i = 0; i < projects->Count; i++)
		{
			String* test = __try_cast<String*>(projects->Item[i]);
			if (String::Compare(projectName, test, true) == 0)
			{
				return true;
			}
		}
	}

	return false;
}

MOG_Project *MOG_ControllerProject::LoginProject(String *projectName, String *branchName)
{
	// Check if we are going to be changing projects?
	if (String::Compare(GetProjectName(), projectName, true) != 0)
	{
		// We shouldn't reset our workspace out from under the Client unless we are really switching projects
		SetCurrentSyncDataController(NULL);
	}

	// Always reload the config file before we login to a project to make sure we have the latest info
	MOG_ControllerSystem::GetSystem()->Load(MOG_ControllerSystem::GetSystem()->GetConfigFilename());

	// Clear the active Project and Branch
	mProject = NULL;
	mBranchName = "";

	// Get the list of project names
	ArrayList *projectNames = MOG_ControllerSystem::GetSystem()->GetProjectNames();
	// Find this name in the list of project names
	for (int n = 0; n < projectNames->Count; n++)
	{
		// Did we find a match?
		if (String::Compare(__try_cast<String *>(projectNames->Item[n]), projectName, true) == 0)
		{
			MOG_Project *newProject = new MOG_Project;

			// Load the Project's configuration file
			// Load the Project's configuration file
			// Make sure we reolve the SystemRepositoryPath token
			String *configFilename = MOG_ControllerSystem::GetSystem()->GetConfigFile()->GetString(projectName, "ConfigFile");
			String *untokenizedConfigFilename = configFilename->ToLower()->Replace(TOKEN_SystemRepositoryPath->ToLower(), MOG_ControllerSystem::GetSystemRepositoryPath());
			// Temp Code to help migrate old untokenized configFilenames
			if (String::Compare(configFilename, untokenizedConfigFilename, true) == 0)
			{
				// Save out a properly tokenized configFilename
				String *tokenizedConfigFilename = configFilename->ToLower()->Replace(MOG_ControllerSystem::GetSystemRepositoryPath()->ToLower(), TOKEN_SystemRepositoryPath);
				MOG_ControllerSystem::GetSystem()->GetConfigFile()->PutString(projectName, "ConfigFile", tokenizedConfigFilename);
				MOG_ControllerSystem::GetSystem()->GetConfigFile()->Save();
			}
			if (newProject->Load(untokenizedConfigFilename, branchName))
			{
				mProject = newProject;

				// Stop looking...We found our specified Project
				break;
			}
		}
	}
	
	// Check to make sure we actually found the specified project?
	if (mProject)
	{
		// Branch...
		// Make sure there was a branchName specified?
		if (!branchName || !branchName->Length)
		{
			branchName = "Current";
		}
		// Assign the specified branch
		mBranchName = branchName;

		// Register this computer's active project with the server
		MOG_Command *newCommand = MOG_CommandFactory::Setup_LoginProject(projectName, branchName);
		MOG_ControllerSystem::GetCommandManager()->CommandProcess(newCommand);

		// Establish the correct database connection for this project
		MOG_ControllerSystem::GetDB()->Connect(projectName, branchName);

		if (MOG_DBProjectAPI::GetBranchIDByName(branchName) == 0)
		{
			//We don't deserve to log in with such a branch name
			mProject = NULL;

			String* branches = "";
			ArrayList* existingBranches = MOG_DBProjectAPI::GetAllBranchNames(projectName);
			if (existingBranches)
			{
				for (int i = 0; i < existingBranches->Count; i++)
				{
					MOG_DBBranchInfo* pBranchInfo = __try_cast<MOG_DBBranchInfo*>(existingBranches->Item[i]);
					if (pBranchInfo)
						branches = String::Concat(branches, S" ", pBranchInfo->mBranchName);
				}
			}
			
			MOG_Report::ReportMessage(	S"Login Error",
										String::Concat(	S"The specified branch does not exist in the project.\n",
														S"PROJECT: ", projectName, S"\n",
														S"BRANCH: ", branchName),
										S"", MOG_ALERT_LEVEL::ERROR);
		}
	}
	else
	{
		MOG_Report::ReportMessage(	S"Login Error", 
									String::Concat(	S"The specified project does not exist.\n",
													S"PROJECT: ", projectName, S"\n",
													S"If this project was removed, there may be some orphaned commands needing to be removed from the MOG Server."),
									S"",
									MOG_ALERT_LEVEL::ERROR);
	}

	// Return the specified Project
	return mProject;
}

MOG_Project *MOG_ControllerProject::LoginProjectIfNecessary(String *projectName, String *branchName)
{
	if (String::Compare(GetProjectName(), projectName, true) != 0 ||
		String::Compare(GetBranchName(), branchName, true) != 0)
	{
		return LoginProject(projectName, branchName);
	}

	return mProject;
}

void MOG_ControllerProject::Logout()
{
	mProject = NULL;
	mBranchName = S"";
	mUser = NULL;
	mPrivileges = NULL;
	mActiveTabName = S"";
	mActiveUser = NULL;
	mCurrentSyncDataController = NULL;
}

void MOG_ControllerProject::RefreshPrivileges( void )
{
	mPrivileges = new MOG_Privileges();
}

bool MOG_ControllerProject::TagCreate(String *sourceTagName, String *newTagName)
{
	return BranchCreate(sourceTagName, newTagName, true);
}

bool MOG_ControllerProject::BranchCreate(String *sourceBranchName, String *newBranchName)
{
	return BranchCreate(sourceBranchName, newBranchName, false);
}

bool MOG_ControllerProject::BranchCreate(String *sourceBranchName, String *newBranchName, bool bIsTag)
{
	// Check to see if we need generate a default Branch name?
	if (!newBranchName->Length)
	{
		// Construct a new time stamp
		MOG_Time *time = new MOG_Time;
		newBranchName = time->FormatString(String::Concat(TOKEN_Year_4, S"_", TOKEN_Month_2, S"_", TOKEN_Day_2, S"_", TOKEN_24Hour_2, S"-", TOKEN_Minute_2));
	}

	// Get user name
	String *creator = MOG_ControllerProject::GetUserName_DefaultAdmin();

	// Make sure we remove any already existing branch
	MOG_DBProjectAPI::PurgeBranch(newBranchName);

	// Create a new branch in the database
	if (MOG_DBProjectAPI::AddBranch(newBranchName, creator, NULL, bIsTag) )
	{
		// Make a copy of the source branch assets and classifications
		if( MOG_DBProjectAPI::CreateNewBranch(sourceBranchName, newBranchName) )
		{
			return true;
		}
	}

	return false;
}


bool MOG_ControllerProject::TagCreate(MOG_ControllerSyncData* workspace, String *newTagName)
{
	return BranchCreate(workspace, newTagName, true);
}

bool MOG_ControllerProject::BranchCreate(MOG_ControllerSyncData* workspace, String *newBranchName)
{
	return BranchCreate(workspace, newBranchName, false);
}

bool MOG_ControllerProject::BranchCreate(MOG_ControllerSyncData* workspace, String *newBranchName, bool bIsTag)
{
	// Make sure we have a valid workspace?
	if (workspace)
	{
		// Get user name from the workspace
		String *creator = workspace->GetUserName();

		// Make sure we remove any already existing branch
		MOG_DBProjectAPI::PurgeBranch(newBranchName);

		// Create a new branch in the database
		if (MOG_DBProjectAPI::AddBranch(newBranchName, creator, NULL, bIsTag) )
		{
			// Make a copy of the source branch assets and classifications
			if( MOG_DBProjectAPI::CreateNewBranch(workspace, newBranchName) )
			{
				return true;
			}
		}
	}

	return false;
}


bool MOG_ControllerProject::BranchRename(String *oldBranchName, String *newBranchName)
{
	// Check to see if we need generate a default Branch name?
	if (oldBranchName->Length &&
		newBranchName->Length)
	{
// TODO - We really should check if there are any outstanding issues with this branch before it is renamed (i.e. PackageCommands, PostCommands, PendingCommands, etc...)
		// Create a new branch in the database
		if (MOG_DBProjectAPI::RenameBranch(oldBranchName, newBranchName))
		{
			return true;
		}
	}

	return false;
}


bool MOG_ControllerProject::BranchRemove(String *branchName)
{
	// Check to see if we need generate a default Branch name?
	if (branchName->Length)
	{
		// Create a new branch in the database
		if (MOG_DBProjectAPI::RemoveBranch(branchName, MOG_ControllerProject::GetUserName_DefaultAdmin()))
		{
// TODO - We really should clean up any outstanding issues associated with this branch when it is removed (i.e. PackageCommands, PostCommands, PendingCommands, etc...)
			return true;
		}
	}

	return false;
}


bool MOG_ControllerProject::BranchPurge(String *branchName)
{
	// Check to see if we need generate a default Branch name?
	if (branchName->Length)
	{
		// Create a new branch in the database
		if (MOG_DBProjectAPI::PurgeBranch(branchName))
		{
			return true;
		}
	}

	return false;
}


bool MOG_ControllerProject::IsValidPlatform(String *platformName)
{
	// Check for the 'All' platform?
	if (String::Compare(platformName, S"All", true) == 0)
	{
		return true;
	}

	if (mProject)
	{
		return mProject->PlatformExists(platformName);
	}

	return false;
}

String* MOG_ControllerProject::GetAllPlatformsString()
{
	String* multi = S"All";

	if (mProject)
	{
		ArrayList* platforms = mProject->GetPlatforms();

		for (int i = 0; i < platforms->Count; i++)
		{
			MOG_Platform* platform = dynamic_cast<MOG_Platform*>(platforms->Item[i]);
			if (platform)
			{
				multi = String::Concat(multi, S", ", platform->mPlatformName);
			}
		}
	}

	return multi;
}


bool MOG_ControllerProject::IsValidClassification(String *classificationFullName)
{
	// Make sure we have a valid project?
	if (mProject)
	{
		// Check if this classification is valid for this project?
		if (MOG_Filename::IsClassificationValidForProject(classificationFullName, mProject->GetProjectName()))
		{
			return mProject->ClassificationExists(classificationFullName);
		}
	}

	return false;
}


bool MOG_ControllerProject::IsValidUser(String *userName)
{
	int userID = MOG_DBProjectAPI::GetUserID(userName);
	return ( userID != 0 );
}


MOG_User *MOG_ControllerProject::LoginUser(String *userName)
{
	// Make sure we are logged in to a project?
	if(mProject)
	{
		mUser = mProject->GetUser(userName);
		if (mUser)
		{
			// Always assume we will also be the active user on a login
			mActiveUser = mUser;

			MOG_ASSERT_THROW(mUser, MOG_Exception::MOG_EXCEPTION_InvalidUser, String::Concat(S"MOG_Main::LoginUser - No '", userName, S"' user found in this project"));

			// Create the LoginUser command
			MOG_Command *newCommand = MOG_CommandFactory::Setup_LoginUser(userName);
			MOG_ControllerSystem::GetCommandManager()->CommandProcess(newCommand);

			// Reload the project privileges
			mPrivileges = new MOG_Privileges();
		}
	}

	return mUser;
}


String *MOG_ControllerProject::LoginComputer(String *computerName)
{
	// Check for an invalid computerName?
	if (String::IsNullOrEmpty(computerName))
	{
		// Default us back to a non-specified state
		computerName = NULL;
	}

	// Set the computer we are loggin in as
	mComputerName = computerName;

	return mComputerName;
}


bool MOG_ControllerProject::SetActiveTabName(String *activeTabName)
{
	if (activeTabName && activeTabName->Length)
	{
		mActiveTabName = activeTabName;
		MOG_ControllerSystem::GetCommandManager()->NotifyServerOfActiveViews(mActiveTabName, "", "", "");
		return true;
	}
	return false;
}


bool MOG_ControllerProject::SetActiveUserName(String *activeUserName)
{
	// Make sure we are logged in to a project?
	if(mProject)
	{
		mActiveUser = mProject->GetUser(activeUserName);
		MOG_ControllerSystem::GetCommandManager()->NotifyServerOfActiveViews("", GetActiveUserName(), "", "");
		return true;
	}
	return false;
}


bool MOG_ControllerProject::SetCurrentSyncDataController(MOG_ControllerSyncData *syncData)
{
	String* platformName = S"n/a";
	String* syncDirectory = S"n/a";

	mCurrentSyncDataController = syncData;

	if (syncData)
	{
		platformName = syncData->GetPlatformName();
		syncDirectory = syncData->GetSyncDirectory();
	}
	
	if (MOG_ControllerSystem::GetCommandManager())
	{
		MOG_ControllerSystem::GetCommandManager()->NotifyServerOfActiveViews("", "", platformName, syncDirectory);
	}

	return true;
}


bool MOG_ControllerProject::DoesAssetExists(MOG_Filename *assetFilename)
{
	// Make sure we have a valid assetName?
	if (assetFilename &&
		assetFilename->GetFilenameType() == MOG_FILENAME_TYPE::MOG_FILENAME_Asset)
	{
		// Get the current version for this asset
		String *version = MOG_DBAssetAPI::GetAssetVersion(assetFilename);
		if (version && version->Length)
		{
			return true;
		}
	}

	return false;
}


bool MOG_ControllerProject::DoesPlatformSpecificAssetExists(MOG_Filename *assetFilename, String *platformName)
{
	// Check if we can get a platform specific version of this asset?
	if (GetPlatformSpecificAsset(assetFilename, platformName))
	{
		return true;
	}

	return false;
}


MOG_Filename* MOG_ControllerProject::GetPlatformSpecificAsset(MOG_Filename *assetFilename, String *platformName)
{
	// Make sure we have a valid assetName?
	if (assetFilename &&
		assetFilename->GetFilenameType() == MOG_FILENAME_TYPE::MOG_FILENAME_Asset)
	{
		// Replace the exisitng platform in this asset's name with the specified platformName
		MOG_Filename *platformSpecificAssetFilename = platformSpecificAssetFilename->CreateAssetName(assetFilename->GetAssetClassification(), platformName, assetFilename->GetAssetLabel());

		// Check if this platform specific asset exists?
		if (DoesAssetExists(platformSpecificAssetFilename))
		{
			return platformSpecificAssetFilename;
		}
	}

	return NULL;
}


String *MOG_ControllerProject::GetAllApplicablePlaformsForAsset(String *assetName, bool bExcludeOverriddenPlatforms)[]
{
	MOG_Filename *assetFilename = new MOG_Filename(assetName);
	if (assetFilename->GetFilenameType() == MOG_FILENAME_TYPE::MOG_FILENAME_Asset)
	{
		String *platformNames[] = new String*[0];

		// Check to see if this asset is an 'All' Platform asset?
		if (!assetFilename->IsPlatformSpecific())
		{
			// Get the current Project?
			MOG_Project *pProject = GetProject();
			if (pProject)
			{
				ArrayList *tempPlatforms = new ArrayList();

				// Check if there are any platform specific assets within the project?
				String *projectPlatformNames[] = pProject->GetPlatformNames();
				for (int p = 0; p < projectPlatformNames->Count; p++)
				{
					// Make sure there isn't a platform specific version of this asset?
					if (bExcludeOverriddenPlatforms &&
						DoesPlatformSpecificAssetExists(assetFilename, projectPlatformNames[p]))
					{
						// Skip this platform from the list
						continue;
					}

					// Add this platform as an applicable platform
					tempPlatforms->Add(projectPlatformNames[p]);
				}

				// YUCK!!!  I hate that .Net won't allow me to just create a dynamic String[]!!!
				// Transfer the applicable platforms back into a static String array
				platformNames = new String*[tempPlatforms->Count];
				for (int p = 0; p < tempPlatforms->Count; p++)
				{
					String *platformName = __try_cast<String *>(tempPlatforms->Item[p]);
					platformNames[p] = platformName;
				}
			}
		}
		else
		{
			// Use only the platform of this Asset
			platformNames = new String*[1];
			platformNames[0] = assetFilename->GetAssetPlatform();
		}

		return platformNames;
	}

	return NULL;
}


bool MOG_ControllerProject::IsPlatformValidForAsset(MOG_Filename *assetFilename, String *platformName)
{
	// Make sure there was a valid platform specified?
	if (platformName->Length)
	{
		// Check if the platforms match each other?
		if (String::Compare(assetFilename->GetAssetPlatform(), platformName, true) == 0)
		{
			// They are an exact match
			return true;
		}
		else
		{
			// Get all the applicable platforms for this asset excluding overridden platform specific platforms
			String *platformNames[] = MOG_ControllerProject::GetAllApplicablePlaformsForAsset(assetFilename->GetAssetFullName(), true);
			if (platformNames != NULL)
			{
				for (int p = 0; p < platformNames->Count; p++)
				{
					// Check each applicable platform of this asset
					if (String::Compare(platformNames[p], platformName, true) == 0)
					{
						return true;
					}
				}
			}
		}
	}

	return false;
}


bool MOG_ControllerProject::AddAssetForPosting(MOG_Filename *pendingAsset, String *jobLabel, String *owner, String *creator)
{
	// Make sure we have a valid Database?
	if (MOG_ControllerSystem::GetDB())
	{
		// Check if no Creator was specified?
		if (creator->Length == 0)
		{
			// Determin the Creator of the asset by who's logged in
			creator = MOG_ControllerProject::GetUserName_DefaultAdmin();
		}

		// Check if no Owner was specified?
		if (owner->Length == 0)
		{
			// Determin the Owner of the asset by who's Inbox the asset is in
			owner =	pendingAsset->GetUserName();
			// Check if we are still missing a valid Owner?
			if (owner->Length == 0)
			{
				// Default Owner to the Creator
				owner = creator;
			}
		}

		// Add the asset's PostCommand
		MOG_DBPostCommandAPI::SchedulePost( pendingAsset->GetAssetFullName(),
											pendingAsset->GetVersionTimeStamp(),
											jobLabel,
											GetBranchName(),
											creator,
											owner);
		return true;
	}

	return false;
}


bool MOG_ControllerProject::PostAssets(String *projectName, String *branchName, String *jobLabel)
{
	return PostAssets(projectName, branchName, jobLabel, false);
}


bool MOG_ControllerProject::PostAssets(String *projectName, String *branchName, String *jobLabel, bool skipArchiving)
{
	bool bFailed = false;
	String *newBranchName = "";
	MOG_Project *pProject;
	bool canceled = false;

	// Check if we are missing the Database?
	if (!MOG_ControllerSystem::GetDB())
	{
		// Never process this command w/o the database!
		return false;
	}

	try
	{	
		// Make sure we are logged into the specified project and branch
		MOG_ControllerProject::LoginProjectIfNecessary(projectName, branchName);
		pProject = MOG_ControllerProject::GetProject();
		if (pProject)
		{
			// Get the number of PostCommands
			ArrayList *posts = MOG_DBPostCommandAPI::GetScheduledPosts(jobLabel, MOG_ControllerProject::GetBranchName());
			if (posts)
			{
				// Make sure we have post commands to process?
				if (posts->Count)
				{
					// Stamp the versions into the branch
					MOG_DBAssetAPI::AddAssetVersionsToBranch(posts);

					// Remove the completed posts from the database
					if (MOG_DBPostCommandAPI::RemovePosts(posts))
					{
						// Cleanup any old stale post commands related to this list of assets
						MOG_DBPostCommandAPI::RemoveStalePostCommands(posts, MOG_ControllerProject::GetBranchName());

						// Prepare to launch our worker
						List<Object*>* args = new List<Object*>();
						args->Add(posts);
						args->Add(jobLabel);
						args->Add(__box(skipArchiving));
						String* message = String::Concat(S"Please wait while assets are posted to the '", branchName, S"' branch.\n", S"This may take a few minutes.");
						ProgressDialog* progress = new ProgressDialog("Posting Assets", message, new DoWorkEventHandler(NULL, &PostAssets_Worker), args, true);
						// Set Hidden based on the Prompt mode to prevent slaves from showing this progress dialog
						progress->Hidden = MOG_Prompt::IsMode(MOG_PROMPT_SILENT);

						if (progress->ShowDialog() != DialogResult::OK)
						{
							bFailed = true;
						}
					}
					else
					{
						// Make sure to inform the Server about this
						String *message = String::Concat(	S"System was unable to remove post commands from the Database\n",
															S"The Database could be down.");
						MOG_Report::ReportMessage(S"PostAssets", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
						bFailed = true;
					}
				}
			}
			else
			{
				// Make sure to inform the Server about this
				String *message = String::Concat(	S"System was unable to receive post commands from the Database\n",
													S"The Database could be down.");
				MOG_Report::ReportMessage(S"PostAssets", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
				bFailed = true;
			}
		}
		else
		{
			bFailed = true;
		}
	}
	catch (Exception *e)
	{
		// Make sure to inform the Server about this
		MOG_Report::ReportMessage(S"PostAssets", e->Message, e->StackTrace, MOG_ALERT_LEVEL::CRITICAL);
		bFailed = true;
	}

	// Check if we failed?
	if (!bFailed)
	{
		return true;
	}
	return false;
}

void MOG_ControllerProject::PostAssets_Worker(Object* sender, DoWorkEventArgs* e)
{
	BackgroundWorker* worker = dynamic_cast<BackgroundWorker*>(sender);
	List<Object*>* args = dynamic_cast<List<Object*>*>(e->Argument);
	ArrayList* posts = dynamic_cast<ArrayList*>(args->Item[0]);
	String* jobLabel = dynamic_cast<String*>(args->Item[1]);
	bool skipArchiving = *dynamic_cast<__box bool*>(args->Item[2]);
	
	bool bSentMoreCommands = false;

	// Scan each posted asset
	int numPosts = posts->Count;
	for (int a = 0; a < numPosts && !worker->CancellationPending; a++)
	{
		try
		{
			// Get the asset's repository location
			MOG_DBPostInfo *post = dynamic_cast<MOG_DBPostInfo *>(posts->Item[a]);
			MOG_Filename *assetFilename = new MOG_Filename(post->mAssetName);
			assetFilename->SetFilename(MOG_ControllerRepository::GetAssetBlessedVersionPath(assetFilename, post->mAssetVersion));

			worker->ReportProgress(a * 100 / numPosts);

			// Check if this is an initial post to the project?
			if (!skipArchiving)
			{
				// Open the asset
				MOG_ControllerAsset *pAssetController = MOG_ControllerAsset::OpenAsset(assetFilename);
				if (pAssetController)
				{
					try
					{
						// Remove Inbox Links for this asset
						MOG_ControllerInbox::RemoveAllAssetLinkFiles(pAssetController);

						// Change our status to Posted
						MOG_ControllerInbox::UpdateInboxView(pAssetController, MOG_AssetStatusType::Posted);

						// Check if this asset already had revisions?
						ArrayList *numRevisions = MOG_DBAssetAPI::GetAllAssetRevisions(assetFilename);
						if (numRevisions->Count > 1)
						{
							// Send Archive Command for this asset for further evaluation
							MOG_Command *archive = MOG_CommandFactory::Setup_ScheduleArchive(assetFilename, jobLabel);
							if (MOG_ControllerSystem::GetCommandManager()->SendToServer(archive))
							{
								bSentMoreCommands = true;
							}
						}

						// Check if we should release the persistent lock?
						if (!pAssetController->GetProperties()->MaintainLock)
						{
							// Release the PersistentLock?
							if (!PersistentLock_Release(assetFilename->GetAssetFullName()))
							{
								// Make sure to inform the Server about this
								String *message = String::Concat(	S"MOG was unable to automatically release the associated lock on the asset during the Post.\n",
																	S"ASSET: ", assetFilename->GetAssetFullName(), S"\n");
								MOG_Report::ReportMessage(S"PostAssets", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
							}
						}

						// Check if MaintainLock is not inherited?
						MOG_Property* propMaintainLock = MOG_PropertyFactory::MOG_Asset_OptionsProperties::New_MaintainLock(true);
						if (pAssetController->GetProperties()->IsPropertyNotInherited(propMaintainLock))
						{
							// Because this asset has already been posted into the branch we need to be surgical about how we alter its database properties
							// Switching the properties into immediate mode will immediately update the database with each little property change rather than restamping the whole asset at the end.
							// This will be safer because we don't want to have an open window where the asset has no meta data.
							pAssetController->GetProperties()->SetImmeadiateMode(true);
							// Remove property this way because it plays nice with inherited MaintainLock properties
							pAssetController->GetProperties()->RemoveProperty(propMaintainLock);
							pAssetController->GetProperties()->SetImmeadiateMode(false);
						}
					}
					__finally
					{
						// Make sure to always free our controller as soon as we are finished with it
						pAssetController->Close();
					}
				}
				else
				{
					// Make sure to inform the Server about this
					String *message = String::Concat(	S"System was unable to open ", assetFilename->GetAssetFullName(), S"\n",
														S"Confirm this asset exists in the MOG Repository.");
					MOG_Report::ReportMessage(S"PostAssets", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
				}
			}

			// Send the Complete Command for this asset being blessed
			MOG_Command *pPost = MOG_CommandFactory::Setup_Post(assetFilename, jobLabel);
			pPost->SetSystemDefaultCommandSettings();
			MOG_Command *pComplete = MOG_CommandFactory::Setup_Complete(pPost, true);
			MOG_ControllerSystem::GetCommandManager()->SendToServer(pComplete);
		}
		catch (Exception *e)
		{
			// Make sure to inform the Server about this
			MOG_Report::ReportMessage(S"PostAssets", e->Message, e->StackTrace, MOG_ALERT_LEVEL::CRITICAL);
		}
	}

	// Check if we sent more commands to the server?
	if (bSentMoreCommands)
	{
		// Make sure we restart the job now that we have finished sending any subsequent commands during this post
		MOG_ControllerProject::StartJob(jobLabel);
	}
}

MOG_Filename *MOG_ControllerProject::GetAssetCurrentBlessedPath( MOG_Filename *assetFilename )
{
	return MOG_ControllerRepository::GetAssetBlessedPath( assetFilename );
}

MOG_Filename *MOG_ControllerProject::GetAssetCurrentBlessedVersionPath( MOG_Filename *assetFilename )
{
	return MOG_ControllerRepository::GetAssetBlessedVersionPath( assetFilename, GetAssetCurrentVersionTimeStamp( assetFilename ) );
}

String *MOG_ControllerProject::GetAssetCurrentVersionTimeStamp( MOG_Filename *assetFilename )
{
	return MOG::DATABASE::MOG_DBAssetAPI::GetAssetVersion( assetFilename );
}

ArrayList *MOG_ControllerProject::GetPackagesForAsset( MOG_Filename *assetFilename )
{
	ArrayList *packageFilenames = new ArrayList();
	ArrayList *dbPackageInfos = MOG::DATABASE::MOG_DBPackageAPI::GetPackageGroupsForAsset( assetFilename );
	for( int i = 0; i < dbPackageInfos->Count; ++i )
	{
		MOG_DBPackageInfo *info = __try_cast<MOG_DBPackageInfo*>( dbPackageInfos->Item[i] );
		packageFilenames->Add( info->mPackageFilename );
	}
	return packageFilenames;
}


bool MOG_ControllerProject::PersistentLock_Release(String *assetName)
{
	MOG_Command *command = MOG_CommandFactory::Setup_LockWriteRelease(assetName);
	command->SetNetworkID(MOG_ControllerSystem::GetNetworkID());
	return MOG_ControllerSystem::GetCommandManager()->CommandProcess(command);
}


MOG_Command * MOG_ControllerProject::PersistentLock_Request(String *assetName, String *description)
{
	MOG_Command *command = MOG_CommandFactory::Setup_LockWriteRequest(assetName, description);
	command->SetPersistentLock(true);
	command->SetNetworkID(MOG_ControllerSystem::GetNetworkID());
	MOG_ControllerSystem::GetCommandManager()->CommandProcess(command);
	return command;
}


MOG_Command * MOG_ControllerProject::PersistentLock_Request(String *assetName, String *description, String *lockOwner)
{
	MOG_Command *command = MOG_CommandFactory::Setup_LockWriteRequest(assetName, description);
	command->SetPersistentLock(true);
	command->SetNetworkID(MOG_ControllerSystem::GetNetworkID());
	command->SetUserName(lockOwner);
	MOG_ControllerSystem::GetCommandManager()->CommandProcess(command);
	return command;
}


String* MOG_ControllerProject::GetLockOwner(String *assetName)
{
	String* lockOwner = S"";

	// Make sure we have something specified?
	if (assetName->Length)
	{
		// Attempt to obtain Lock information
		MOG_Command *lockInfo = MOG_ControllerProject::PersistentLock_Query(assetName);
		if (lockInfo)
		{
			// Now check if someone had a lock on this asset
			if (lockInfo->IsCompleted() &&
				lockInfo->GetCommand())
			{
				lockOwner = lockInfo->GetCommand()->GetUserName();
			}
		}
	}

	return lockOwner;
}


bool MOG_ControllerProject::IsLocked(String *assetName)
{
	// Make sure we have something specified?
	if (assetName->Length)
	{
		// Attempt to obtain Lock information
		MOG_Command *lockInfo = MOG_ControllerProject::PersistentLock_Query(assetName);
		if (lockInfo)
		{
			// Now check if someone had a lock on this asset
			if (lockInfo->IsCompleted() &&
				lockInfo->GetCommand())
			{
				return true;
			}
		}
	}

	return false;
}


bool MOG_ControllerProject::IsLockedByMe(String *assetName)
{
	// Make sure we have something specified?
	if (assetName->Length)
	{
		// Attempt to obtain Lock information
		MOG_Command *lockInfo = MOG_ControllerProject::PersistentLock_Query(assetName);
		if (lockInfo)
		{
			// Now check if someone had a lock on this asset
			if (lockInfo->IsCompleted() &&
				lockInfo->GetCommand())
			{
				// Check if this lock belongs to us?
				if (String::Compare(lockInfo->GetCommand()->GetUserName(), MOG_ControllerProject::GetUserName_DefaultAdmin(), true) == 0)
				{
					return true;
				}
			}
		}
	}

	return false;
}


MOG_Command *MOG_ControllerProject::PersistentLock_Query(String *assetName)
{
	MOG_Command *command = MOG_CommandFactory::Setup_LockWriteQuery(assetName);
	command->SetPersistentLock(true);
	command->SetNetworkID(MOG_ControllerSystem::GetNetworkID());

	command->SetCompleted(MOG_ControllerSystem::GetCommandManager()->CommandProcess(command));
	return command;
}


bool MOG_ControllerProject::QueryWriteLock(String *lockName)
{
	MOG_Command *command = MOG_CommandFactory::Setup_LockWriteQuery(lockName);
	return MOG_ControllerSystem::GetCommandManager()->CommandProcess(command);
}


bool MOG_ControllerProject::QueryReadLock(String *lockName)
{
	MOG_Command *command = MOG_CommandFactory::Setup_LockReadQuery(lockName);
	return MOG_ControllerSystem::GetCommandManager()->CommandProcess(command);
}


bool MOG_ControllerProject::BuildFull(String *assetBuildName, String *source, String *target, String *validSlaves, bool force)
{
	// Send the BuildFull command to the server.
	MOG_Command *command = MOG_CommandFactory::Setup_BuildFull(assetBuildName, source, target, validSlaves, force);
	return MOG_ControllerSystem::GetCommandManager()->SendToServer(command);
}


bool MOG_ControllerProject::Build(String *executable, String *validSlaves, String *user, String *EnvVariables)
{
	// Send the BuildFull command to the server.
	MOG_Command *command = MOG_CommandFactory::Setup_Build(executable, validSlaves, user, EnvVariables);
	return MOG_ControllerSystem::GetCommandManager()->SendToServer(command);
}


bool MOG_ControllerProject::Archive()
{
	return false;
}


MOG_Properties *MOG_ControllerProject::GetClassificationProperties(String *classification)
{
	// Open the classification properties for editing
	return MOG_Properties::OpenClassificationProperties(classification);
}


bool MOG_ControllerProject::ValidateAssetFilename(MOG_Filename *assetFilename)
{
	// Validate the asset's platform
	bool bFoundPlatformName = false;
	String *platformNames[] = GetProject()->GetPlatformNames();
	// scan each platformName
	for (int p = 0; p < platformNames->Count; p++)
	{
		// Check if we found a match for the asset's platform?
		if (String::Compare(assetFilename->GetAssetPlatform(), platformNames[p], true))
		{
			bFoundPlatformName = true;
			break;
		}
	}
	// Check if we failed?
	if (!bFoundPlatformName)
	{
		return false;
	}


	// Validate the asset's classification?
	if (!IsValidClassification(assetFilename->GetAssetClassification()))
	{
		return false;
	}

	return true;
}

bool MOG_ControllerProject::MakeAssetCurrentVersion(ArrayList *blessedAssetFilenames, String *comment)
{
	bool bFailed = false;

	// Obtain a unique jobLabel for this post
	String *jobLabel = String::Concat(S"CheckIn.", MOG_ControllerSystem::GetComputerName(), S".", MOG_Time::GetVersionTimestamp());
	// Loop through all the specified assets
	for (int a = 0; a < blessedAssetFilenames->Count; a++)
	{
		MOG_Filename *blessedAssetFilename = __try_cast<MOG_Filename *>(blessedAssetFilenames->Item[a]);
		if (!MakeAssetCurrentVersion(blessedAssetFilename, comment, jobLabel))
		{
			bFailed = true;
		}
	}

	// Start the job
	MOG_ControllerProject::StartJob(jobLabel);

	return !bFailed;
}

bool MOG_ControllerProject::MakeAssetCurrentVersion(MOG_Filename *blessedAssetFilename, String *comment, String *jobLabel)
{
	bool bAutoStartJob = false;

	// Check if we need to autogenerate a jobLabel?
	if (jobLabel->Length == 0)
	{
		// Obtain a unique jobLabel for this post
		jobLabel = String::Concat(S"CheckIn.", MOG_ControllerSystem::GetComputerName(), S".", MOG_Time::GetVersionTimestamp());
		// If we have to create a default jobLabel then we need to start it
		bAutoStartJob = true;
	}

	// Make sure that this is a blessed asset?
	if (blessedAssetFilename->IsBlessed())
	{
		// If we have no version, we need to go get the latest blessed version and assign that version to this asset?
		if (blessedAssetFilename->GetVersionTimeStamp()->Length == 0)
		{
			blessedAssetFilename->SetVersionTimeStamp(MOG_DBAssetAPI::GetAssetVersion(blessedAssetFilename));
		}

		// Make sure the asset's platform is still valid?
		if (MOG_ControllerProject::IsValidPlatform(blessedAssetFilename->GetAssetPlatform()))
		{
			// Open the asset
			MOG_ControllerAsset *asset = MOG_ControllerAsset::OpenAsset(blessedAssetFilename);
			if (asset)
			{
				// Post the comment to this asset
				asset->PostComment(comment, true);
				// Always make sure to close the asset when we are done
				asset->Close();

				// Check if the user specified mUseOnlyMySlaves?
				String *validSlaves = S"";
				if (MOG_ControllerSystem::GetCommandManager()->mUseOnlyMySlaves)
				{
					// Specify us as the only ValidSlaves
					validSlaves = MOG_ControllerSystem::GetComputerName();
				}

				// Check if this asset's classification has been previously removed from the project?
				if (!MOG_ControllerProject::IsValidClassification(blessedAssetFilename->GetAssetClassification()))
				{
					// Make sure this asset's classification is active in the branch (needed for when an asset is made current from within a previosuly removed classification)
					mProject->ClassificationAdd(blessedAssetFilename->GetAssetClassification());
				}

				// Setup the bless command for this asset to be sent to the server (indicate we can skip the asset archiving)
				MOG_Command *command = MOG_CommandFactory::Setup_Bless(blessedAssetFilename, jobLabel, validSlaves, false, true);
				MOG_ControllerSystem::GetCommandManager()->SendToServer(command);

				// Check if we need to start the job?
				if (bAutoStartJob)
				{
					// Start the job
					MOG_ControllerProject::StartJob(jobLabel);
				}

				return true;
			}
		}
		else
		{
			// Inform the user the asset's platform is no longer valid
			String *message = String::Concat(S"Asset platform is not longer valid for this project and cannot be made current.\n",
											 S"ASSET: ", blessedAssetFilename->GetAssetFullName());
			MOG_Prompt::PromptMessage(S"Make Current Failed", message, S"", MOG_ALERT_LEVEL::ERROR);
		}
	}

	return false;
}


bool MOG_ControllerProject::RemoveAssetFromProject(MOG_Filename *assetFilename, String *comment, bool skipUnpackaging)
{
	bool bRemoved = false;

	// Create a nice default jobLabel
	String *jobLabel = String::Concat(S"RemoveAsset.", MOG_ControllerSystem::GetComputerName(), S".", MOG_Time::GetVersionTimestamp());

	// Remove the asset using our generated jobLabel
	if (RemoveAssetFromProject(assetFilename, comment, jobLabel, skipUnpackaging))
	{
		// Make sure we start the job since we had to name it
		if (MOG_ControllerProject::StartJob(jobLabel))
		{
			bRemoved = true;
		}
	}

	return bRemoved;
}


bool MOG_ControllerProject::RemoveAssetFromProject(MOG_Filename *assetFilename, String *comment, String *jobLabel, bool skipUnpackaging)
{
	// Make sure this asset is not currently locked by anyone?
	//Don't remove assets that are currently locked by other users
	MOG_Command* command = PersistentLock_Query(assetFilename->GetAssetFullName());
	MOG_Command* lock = command->GetCommand();
	if (lock && lock->IsCompleted())
	{
		// Check if it is locked by someone other than ourself?
		if (lock->GetUserName() != MOG_ControllerProject::GetActiveUserName() &&
			lock->GetComputerName() != MOG_ControllerSystem::GetComputerName())
		{
			String *message = String::Concat(	S"This asset is currently locked by another user.\n",
												S"ASSET: ", assetFilename->GetAssetFullName(),
												S"USER: ", lock->GetUserName(), S"     COMPUTER: ", lock->GetComputerName());
			MOG_Prompt::PromptMessage(	S"Unable to Remove Asset", message, S"", MOG_ALERT_LEVEL::ALERT);
			return false;
		}
	}

	// First check to see if there is a revision already specified in the assetFilename?
	String *assetRevision = assetFilename->GetVersionTimeStamp();
	// Obtain the latest revision of the specified asset in the MOG Repository
	String *currentAssetRevision = MOG_ControllerProject::GetAssetCurrentVersionTimeStamp(assetFilename);
	// Check if no revision was included in the remove request?
	if (assetRevision->Length == 0)
	{
		// Use the current version of this package
		assetRevision = currentAssetRevision;
	}
	// Check if we actually obtained a revision?
	if (assetRevision->Length)
	{
		// Obtain the fully quantified path to this blessed asset in the repository
		assetFilename = MOG_ControllerRepository::GetAssetBlessedVersionPath(assetFilename, assetRevision);
	}

	// Check if this asset is a package?
	MOG_Properties *pProperties = new MOG_Properties(assetFilename);
	if (pProperties != NULL)
	{
		// Check if this is a package?
		if (pProperties->IsPackage)
		{
			// Check if this remove will result in the package is being remove from the branch?
			// Also make sure this package's platform is still valid in the project?
			if (String::Compare(assetRevision, currentAssetRevision, true) == 0 &&
				MOG_ControllerProject::IsValidPlatform(assetFilename->GetAssetPlatform()))
			{
				// Check if this package has any assigned assets?
				ArrayList* assetsInPackage = MOG_ControllerPackage::GetAssetsInPackage(assetFilename);
				if (assetsInPackage && assetsInPackage->Count)
				{
					String *message = String::Concat(	S"There are assets currently assigned to this package.  These assinged assets will be reinstanced as a result of having this package assignment removed.\n",
														S"Are you sure you want to remove this package from the project?");
					// Ask the user what they want to do
					if (MOG_Prompt::PromptResponse(	S"Package Not Empty!  Continue?", message, S"", MOGPromptButtons::YesNo, MOG_ALERT_LEVEL::ALERT) == MOGPromptResult::No)
					{
						return false;
					}

					MOG_Filename *targetPackageFilename = NULL;
					// Check if this asset being removed is a platform specific package?
					if (assetFilename->IsPlatformSpecific())
					{
						// Limit the reinstance of this asset to the just the generic '{All}' package to save time.  Otherwise, this asset may 
						// end up being repackaged in all of the other platform-specific packages if it uses the '{All}' package assignment
						targetPackageFilename = MOG_Filename::CreateAssetName(assetFilename->GetAssetClassification(), S"All", assetFilename->GetAssetLabel());
					}

					// Reinstance all the assets in the package
					bool bYesToAll = false;
					bool bNoToAll = false;
					for (int i = 0; i < assetsInPackage->Count; i ++)
					{
						MOG_Filename *assetInPackageFilename = __try_cast<MOG_Filename *>(assetsInPackage->Item[i]);

						// Establish our source and new target instance for this packaged asset
						MOG_Filename *sourceAssetFilename = MOG_ControllerRepository::GetAssetBlessedVersionPath(assetInPackageFilename, assetInPackageFilename->GetVersionTimeStamp());
						String *newTimestamp = MOG_Time::GetVersionTimestamp();
						MOG_Filename *targetAssetFilename = MOG_ControllerRepository::GetAssetBlessedVersionPath(assetInPackageFilename, newTimestamp);

						// Open the asset's properties
						MOG_Properties *assetProperties = new MOG_Properties(sourceAssetFilename);
						if (assetProperties)
						{
							// Make sure this is really a PackagedAsset?
							if (assetProperties->IsPackagedAsset)
							{
								ArrayList *propertiesToRemove = new ArrayList();
								ArrayList *propertiesToAdd = new ArrayList();

								// Get all of the packages for this asset
								ArrayList *relationships = assetProperties->GetPackages();
								bool bHasOtherPackageAssignments = false;
								// Check if this asset has any other package assignments?
								for (int r = 0; r < relationships->Count; r++)
								{
									MOG_Property *pProperty = __try_cast<MOG_Property *>(relationships->Item[r]);

									// Check if this relationship is related to the package being removed?
									if (pProperty->mPropertyKey->StartsWith(assetFilename->GetAssetFullName(), StringComparison::CurrentCultureIgnoreCase))
									{
										// Check if this is an inherited property?
										if (!assetProperties->IsPropertyAlreadyInherited(pProperty))
										{
											// Only schedule this property for removal if it isn't an inherited assignment
											propertiesToRemove->Add(pProperty);
										}
										else
										{
											// Get the classification where we inherited this property
											String *inheritedClassification = assetProperties->GetInheritedPropertyClassification(pProperty);
											if (inheritedClassification->Length > 0)
											{
												// Open the inherited classification's properties
												MOG_Properties *inheritedClassificationProperties = MOG_Properties::OpenClassificationProperties(inheritedClassification);
												if (inheritedClassificationProperties)
												{
													// Remove this property from our inherited classification
													inheritedClassificationProperties->RemoveProperty(pProperty);
													inheritedClassificationProperties->Close();
												}
											}
										}
									}
									else
									{
										// Indicate that the asset has other valid package assignments
										bHasOtherPackageAssignments = true;
									}
								}

								// Check if this asset is no longer being contained within a package?
								bool bDeleteAsset = false;
								if (!bHasOtherPackageAssignments)
								{
									// Check if the user has already specified?
									if (bYesToAll)
									{
										bDeleteAsset = true;
									}
									else if(bNoToAll)
									{
										bDeleteAsset = false;
									}
									else
									{
										// Ask the user what MOG should do here?
										String* message = String::Concat(	S"This asset will no longer be assigned to any packages.\n",
																			S"ASSET: ", assetInPackageFilename->GetAssetFullName(), S"\n\n",
																			S"Would you like to removed this asset from the project as well?\n");
										switch(MOG_Prompt::PromptResponse("Removing package from project", message, MOGPromptButtons::YesNoYesToAllNoToAll))
										{
											case MOGPromptResult::YesToAll:
												bYesToAll = true;
												/* Intentially Fall through */
											case MOGPromptResult::Yes:
												bDeleteAsset = true;
												break;
											case MOGPromptResult::NoToAll:
												bNoToAll = true;
												/* Intentially Fall through */
											case MOGPromptResult::No:
												bDeleteAsset = false;
										}
									}
								}

								// Check if we should delete this asset?
								if (bDeleteAsset == true)
								{
									// Recursively call ourselves for each packaged asset the can also be removed; always skipping the unpackaging event
									RemoveAssetFromProject(assetInPackageFilename, "Removed from the project becuase when its package was removed.", jobLabel, false);
								}
								else
								{
									//Build the command to reinstance the asset
									String *options = "";
									MOG_Command *reinstanceAsset = MOG_CommandFactory::Setup_ReinstanceAssetRevision(targetAssetFilename, sourceAssetFilename, jobLabel, propertiesToRemove, propertiesToAdd, options, targetPackageFilename);
									//Reinstance the asset
									MOG_ControllerSystem::GetCommandManager()->SendToServer(reinstanceAsset);
								}
							}
						}
					}
				}
			}
		}
	}

	// Send the command to the server for processing
	MOG_Command *removeAssetCommand = MOG_CommandFactory::Setup_RemoveAssetFromProject(assetFilename, comment, jobLabel, skipUnpackaging);
	return MOG_ControllerSystem::GetCommandManager()->SendToServer(removeAssetCommand);
}


ArrayList *MOG_ControllerProject::MapAssetNameToFilename(MOG_Filename *assetFilename, String *platformName)
{
	String *applicablePlatforms[] = NULL;

	// Make sure we have a valid assetFilename?
	if (assetFilename &&
		assetFilename->GetOriginalFilename()->Length)
	{
		// Make sure we have a valid AssetName?
		if (assetFilename->GetFilenameType() != MOG_FILENAME_TYPE::MOG_FILENAME_Unknown)
		{
			// Check if this is an empty platform? or
			// Check if this is an 'All' platform?
			if (platformName == NULL ||
				platformName->Length == 0 ||
				String::Compare(platformName, S"All", true) == 0)
			{
				// Get all the applicable platforms for this asset excluding overridden platform specific platforms
				applicablePlatforms = MOG_ControllerProject::GetAllApplicablePlaformsForAsset(assetFilename->GetAssetFullName(), false);
			}
			else
			{
				// Use the specified platformName
				applicablePlatforms = new String*[1];
				applicablePlatforms[0] = platformName;
			}

			// Loop through all the applicable platforms looking for a matching game data file
			for (int p = 0; p < applicablePlatforms->Count; p++)
			{
				// Ask the database about the game data file associated with this package?
				ArrayList *packageSyncTargetFiles = MOG_DBAssetAPI::GetSyncTargetFileLinks(assetFilename, applicablePlatforms[p]);
				if (packageSyncTargetFiles && packageSyncTargetFiles->Count)
				{
					// Return the associated file list
					return packageSyncTargetFiles;
				}
			}

			//We still haven't found a list of files, but we have a hold of an asset, so let's just ask it via it's properties
			MOG_Properties *pProperties = new MOG_Properties(assetFilename);
			String *files[] = MOG_ControllerAsset::GetAssetPlatformFiles(pProperties, platformName, false);
			if (files && files->Count)
			{
				return new ArrayList(files);
			}
		}
	}

	return new ArrayList();
}

ArrayList *MOG_ControllerProject::MapFilenameToAssetName_FilterLibraryAssets(ArrayList *assetNames)
{
	// Scan the specified assets...Don't auto increment because the remove will collapse the array around us
	for (int a = 0; a < assetNames->Count; /* a++ */)
	{
		// Make sure this is a valid assetFilename
		MOG_Filename *assetFilename = __try_cast<MOG_Filename *>(assetNames->Item[a]);
		if (assetFilename &&
			assetFilename->GetOriginalFilename()->Length)
		{
			if (assetFilename->IsLibrary())
			{
				// Remove this asset from our list
				assetNames->RemoveAt(a);
				// Don't increment because the remove just collapsed the array around us
				continue;
			}
		}

		// Only now auto increment after we know we are not removing it from our list
		a++;
	}

	return assetNames;
}

ArrayList *MOG_ControllerProject::MapFilenameToAssetName_FilterAssetsWithExtensions(ArrayList *assetNames)
{
	// Scan the specified assets...Don't auto increment because the remove will collapse the array around us
	for (int a = 0; a < assetNames->Count; /* a++ */)
	{
		// Make sure this is a valid assetFilename
		MOG_Filename *assetFilename = __try_cast<MOG_Filename *>(assetNames->Item[a]);
		if (assetFilename &&
			assetFilename->GetOriginalFilename()->Length)
		{
			// Check if this asset contains an extension by comparing the label's length with and w/o an extension
			if (assetFilename->GetAssetLabel()->Length != assetFilename->GetAssetLabelNoExtension()->Length)
			{
				// Remove this asset from our list
				assetNames->RemoveAt(a);
				// Don't increment because the remove just collapsed the array around us
				continue;
			}
		}

		// Only now auto increment after we know we are not removing it from our list
		a++;
	}

	return assetNames;
}

MOG_Filename *MOG_ControllerProject::MapFilenameToLibraryAssetName(String *filename, String *platformName)
{
	return MOG_ControllerLibrary::ConstructAssetNameFromLibraryFile(filename);
}

ArrayList *MOG_ControllerProject::MapFilenameToAssetName(String *filename, String *platformName, String* workspaceDirectory)
{
	String* relativeFilename = S"";
	ArrayList *assetNames = new ArrayList();

	// Check if this is a rooted path?
	if (DosUtils::PathIsDriveRooted(filename))
	{
		// Check if no specific workspace directory was specified?
		if (workspaceDirectory->Length == 0)
		{
			// Check if this filename begins with the current workspace?
			if (DosUtils::PathIsWithinPath(MOG_ControllerProject::GetWorkspaceDirectory(), filename))
			{
				// Use this as our workspaceDirectory
				workspaceDirectory = MOG_ControllerProject::GetWorkspaceDirectory();
				// Respect the platform of the workspace we wre within
				platformName = MOG_ControllerProject::GetPlatformName();
			}
			else
			{
				// Looks like we need to auto detect the workspace by drilling...YUCK
				workspaceDirectory = MOG_ControllerSyncData::DetectWorkspaceRoot(DosUtils::PathGetDirectoryPath(filename));
//				// Respect the platform of the workspace we wre within
//				platformName = {Need the new MultiWorkspace code so we can get the platform from the workspace root};
			}
		}

// JohnRen - Making this filename relative breaks the ability for auto detection when importing assets from within the workspace!
		// Strip off the workspace directory
		relativeFilename = DosUtils::PathMakeRelativePath(workspaceDirectory, filename);
	}
	else
	{
		// Since this isn't rooted, we can assume this is already relative
		relativeFilename = filename;
	}

	// Check if we were able to verify this as a relative
	if (relativeFilename->Length)
	{
		assetNames = MapFilenameToAssetName(relativeFilename, platformName);
	}

	return assetNames;
}

ArrayList *MOG_ControllerProject::MapFilenameToAssetName(String *filename, String *platformName)
{
	ArrayList *assetNames = new ArrayList();

	// Create an initial AssetFilename and verify it isn't a library asset?
	MOG_Filename *assetName = new MOG_Filename(filename);
	if (!assetName->IsLibrary())
	{
		// Make sure we have access to the database
		if (MOG_ControllerSystem::GetDB())
		{
			// First off let's just check if this asset is complete and already in the database?
			if (assetName->GetFilenameType() == MOG_FILENAME_TYPE::MOG_FILENAME_Asset &&
				MOG_DBAssetAPI::GetAssetNameID(assetName) != 0)
			{
				// It's in the database, let's just use this as is!
				assetNames->Add(assetName);
			}
			else
			{
				// Is this relative file listed anywhere in the sync targets for this platform?
				assetNames = MOG_DBAssetAPI::GetSyncTargetFileAssetLinks(filename, platformName);
				assetNames = MapFilenameToAssetName_FilterLibraryAssets(assetNames);
				// Check if we are still missing a match?
				if (assetNames->Count == 0)
				{
					// Get any assets with a relationship to this file
					assetNames = MapFilenameToAssetName_InsertAssetsByProperty(MOG_PropertyFactory::MOG_Relationships::New_AssetSourceFile(filename));
					if (assetNames->Count == 0)
					{
						// Get any assets with relationship to a filename like this
						assetNames = MapFilenameToAssetName_InsertAssetsByProperty(MOG_PropertyFactory::MOG_Relationships::New_AssetSourceFile(String::Concat(S"*\\", DosUtils::PathGetFileName(filename))));

						// Check if any assets have imported files that match this?
						if (assetNames->Count == 0)
						{
							// Get any assets that map to a sync target like this file
							assetNames = MOG_DBAssetAPI::GetSyncTargetFileAssetLinksFileNameOnly(DosUtils::PathGetFileName(filename), S"");
							assetNames = MapFilenameToAssetName_FilterLibraryAssets(assetNames);
						}

						// Check if we only got one back?
						if (assetNames->Count == 1)
						{
							// Add an empty item at the bottom because these last few checks were way to generic for us to use w/o asking the user
							// Adding a blank will cause there to be more than 1 item thus triggering the GUI to prompt the user
							assetNames->Add(new MOG_Filename());
						}
					}
				}
			}
		}
	}

	return assetNames;
}


ArrayList *MOG_ControllerProject::MapFilenamesToAssetNames(ArrayList *filenames, String *platformName, BackgroundWorker* worker)
{
	HybridDictionary *assetNames = new HybridDictionary();

	// Make sure we have a valid list?
	if (filenames)
	{
		// Scan the specified filenames
		for (int p = 0; p < filenames->Count && (worker ? !worker->CancellationPending : true); p++)
		{
			// Get the details of the updated package
			String *filename = __try_cast<String *>(filenames->Item[p]);

			if (worker)
			{
				String *message = String::Concat(	S"Mapping:\n",
													S"     ", Path::GetDirectoryName(filename), S"\n",
													S"     ", Path::GetFileName(filename));
				worker->ReportProgress(p * 100 / filenames->Count, message);
			}

			// Map the packageFile to its MOG's AssetName
			ArrayList *detectedAssets = MOG_ControllerProject::MapFilenameToAssetName(filename, platformName, "");
			if (detectedAssets->Count)
			{
				// Walk through the detectedAssets
				for (int p = 0; p < detectedAssets->Count; p++)
				{
					// Get the details of the updated package
					MOG_Filename *filename = __try_cast<MOG_Filename *>(detectedAssets->Item[p]);

					assetNames->Item[filename->GetAssetFullName()] = filename;
				}
			}
		}
	}

	return MOG_LocalSyncInfo::ConvertFromHybridDictionaryToArrayList(assetNames);
}


ArrayList *MOG_ControllerProject::MapFilenameToAssetName_InsertAssetsByProperty(MOG_Property* property)
{
	HybridDictionary *assetNames = new HybridDictionary();

	// Make sure we have a valid property specified?
	if (property)
	{
		// Get any blessed assets that contain this property
		ArrayList* blessedAssetNames = MOG_DBAssetAPI::GetAllAssetsByProperty(property);
		for (int i = 0; i < blessedAssetNames->Count; i++)
		{
			MOG_Filename* blessedAssetName = dynamic_cast<MOG_Filename*>(blessedAssetNames->Item[i]);
			assetNames->Item[blessedAssetName->GetAssetFullName()] = blessedAssetName;
		}

		// Get any inbox assets that contain this property
		ArrayList* inboxAssetNames = MOG_DBInboxAPI::InboxGetAssetListFromProperty("Drafts", "", property);
		for (int i = 0; i < inboxAssetNames->Count; i++)
		{
			String* inboxAssetName = dynamic_cast<String*>(inboxAssetNames->Item[i]);

			MOG_Filename* tempAssetFilename = new MOG_Filename(inboxAssetName);
			MOG_Filename* inboxAssetFilename = new MOG_Filename(tempAssetFilename->GetAssetEncodedInboxPath(MOG_ControllerProject::GetUserName_DefaultAdmin(), S"Drafts"));

			assetNames->Item[inboxAssetFilename->GetAssetFullName()] = inboxAssetFilename;
		}
	}

	// Make sure we filter out any library assets
	return MapFilenameToAssetName_FilterLibraryAssets(MOG_LocalSyncInfo::ConvertFromHybridDictionaryToArrayList(assetNames));
}


bool MOG_ControllerProject::MapFilenameToAssetName_WarnAboutAmbiguousMatches(String *filename, ArrayList *assets)
{
	bool bWarned = false;

	// Make sure we have a valid list?
	if (assets)
	{
		// Count the number of fake matches in the list
		int fakeMatches = 0;
		for(int a = 0; a < assets->Count; a++ )
		{
			MOG_Filename *assetFilename = __try_cast<MOG_Filename*>(assets->Item[a]);

			// Check if this is null or empty?
			if (assetFilename == NULL ||
				assetFilename->GetOriginalFilename()->Length == 0)
			{
				// Count this as a fake entry
				fakeMatches++;
			}
		}

		// Check if we need to warn the user...Accounting for fakeMatches
		if ((assets->Count - fakeMatches) > 1)
		{
			// Warn the user about multiple matches
			String *message = String::Concat(	S"This file matches more than one asset in the project.  It is not recommended to have multiple assets colliding with a common file.\n",
												S"FILE: ", filename, S"\n",
												S"ASSETS: \n");
			// Display each of the colliding assets
			for(int a = 0; a < assets->Count; a++ )
			{
				MOG_Filename *assetFilename = __try_cast<MOG_Filename*>(assets->Item[a]);
				message = String::Concat(message, S"	", assetFilename->GetAssetFullName(), S"\n");
			}

			MOG_Report::ReportMessage(S"Package Assignment", message, Environment::StackTrace, MOG_ALERT_LEVEL::ALERT);

			// Indicate our return value
			bWarned = true;
		}
	}

	return bWarned;
}


ArrayList *MOG_ControllerProject::MapPackageObjectToAssetNames(String* assetLabel, MOG_Property* relationshipProperty)
{
	HybridDictionary* foundAssets = new HybridDictionary();

	// Extract the elements of this relationship
	String* packageName = MOG_ControllerPackage::GetPackageName(relationshipProperty->mPropertyKey);
	String* packageGroupName = MOG_ControllerPackage::GetPackageGroups(relationshipProperty->mPropertyKey);
	MOG_Filename* packageFilename = new MOG_Filename(packageName);

	// Attempt to locate a blessed asset in the repository with this relationship
	String* packageRevision = MOG_ControllerProject::GetAssetCurrentVersionTimeStamp(packageFilename);
	ArrayList* assets = MOG_DBPackageAPI::GetAllAssetsInPackageGroup(packageFilename, packageRevision, packageGroupName);
	for (int a = 0; a < assets->Count; a++)
	{
		MOG_Filename* assetFilename = __try_cast<MOG_Filename*>(assets->Item[a]);

		// Check if this asset matches the specified assetLabel?
		if (String::Compare(assetLabel, assetFilename->GetAssetLabel(), true) == 0 ||
			String::Compare(assetLabel, assetFilename->GetAssetLabelNoExtension(), true) == 0)
		{
			foundAssets->Item[assetFilename->GetAssetFullName()] = assetFilename;
		}
	}

	return new ArrayList(foundAssets->Values);
}

ArrayList *MOG_ControllerProject::MapPackageObjectToAssetNamesInUserInbox(String* assetLabel, MOG_Property* relationshipProperty)
{
	HybridDictionary* foundAssets = new HybridDictionary();

	// Also check the user's inbox for any assets that might not have been blessed yet
	ArrayList* assets = MOG_DBInboxAPI::InboxGetAssetListFromProperty(S"Drafts", assetLabel, relationshipProperty);
	if (assets->Count > 0)
	{
		// Add each detected inbox asset 
		for (int i = 0; i < assets->Count; i++)
		{
			String* assetFullName = dynamic_cast<String*>(assets->Item[i]);

			MOG_Filename* tempAssetFilename = new MOG_Filename(assetFullName);
			MOG_Filename* assetFilename = new MOG_Filename(tempAssetFilename->GetAssetEncodedInboxPath(MOG_ControllerProject::GetUserName_DefaultAdmin(), S"Drafts"));

			// Check if this asset matches the specified assetLabel?
			if (String::Compare(assetLabel, assetFilename->GetAssetLabel(), true) == 0 ||
				String::Compare(assetLabel, assetFilename->GetAssetLabelNoExtension(), true) == 0)
			{
				foundAssets->Item[assetFilename->GetAssetFullName()] = assetFilename;
			}
		}
	}

	return new ArrayList(foundAssets->Values);
}


ArrayList *MOG_ControllerProject::MapPackageObjectToContainedAssetNames(String* assetLabel, MOG_Property* relationshipProperty, String* LOD)
{
	HybridDictionary* containedAssets = new HybridDictionary();

	// Extract the elements of this relationship
	String* packageName = MOG_ControllerPackage::GetPackageName(relationshipProperty->mPropertyKey);
	String* packageGroupName = MOG_ControllerPackage::GetPackageGroups(relationshipProperty->mPropertyKey);
	MOG_Filename* packageFilename = new MOG_Filename(packageName);

	// Obtain the proper LOD Check if an LOD is specified?
	int iLOD = 0;
	if (LOD->Length)
	{
		try
		{
			iLOD = Convert::ToUInt32(LOD);
		}
		catch(...)
		{
		}
	}

	// Now check for any assets using this this assetLabel as their packageGroup
	String* tempPackageGroupName = MOG_ControllerPackage::CombinePackageInternalPath(packageGroupName, assetLabel);
	String* packageRevision = MOG_ControllerProject::GetAssetCurrentVersionTimeStamp(packageFilename);
	ArrayList* assets = MOG_DBPackageAPI::GetAllAssetsInPackageGroup(packageFilename, packageRevision, tempPackageGroupName);
	for (int a = 0; a < assets->Count; a++)
	{
		MOG_Filename* assetFilename = __try_cast<MOG_Filename*>(assets->Item[a]);

		// Check if an LOD is specified?
		if (iLOD > 0)
		{
			// Check if this LOD doesn't match this asset?
			if (String::Compare(LOD, MOG_ControllerProject::DetectLODFromAssetLabel(assetFilename->GetAssetLabel()), true) != 0)
			{
				continue;
			}
		}

		containedAssets->Item[assetFilename->GetAssetFullName()] = assetFilename;
	}

	return new ArrayList(containedAssets->Values);
}

ArrayList *MOG_ControllerProject::MapPackageObjectToContainedAssetNamesInUserInbox(String* assetLabel, MOG_Property* relationshipProperty, String* LOD)
{
	HybridDictionary* containedAssets = new HybridDictionary();

	// Extract the elements of this relationship
	String* packageName = MOG_ControllerPackage::GetPackageName(relationshipProperty->mPropertyKey);
	String* packageGroupName = MOG_ControllerPackage::GetPackageGroups(relationshipProperty->mPropertyKey);
	MOG_Filename* packageFilename = new MOG_Filename(packageName);

	// Create another relationship property to be used for checking packageObjects
	MOG_Property* relationshipPropertyWithObjectName = MOG_PropertyFactory::MOG_Relationships::New_RelationshipAssignment(relationshipProperty->mPropertySection, packageName, packageGroupName, assetLabel);

	// Obtain the proper LOD Check if an LOD is specified?
	int iLOD = 0;
	if (LOD->Length)
	{
		try
		{
			iLOD = Convert::ToUInt32(LOD);
		}
		catch(...)
		{
		}
	}

	// Check the user's inbox for any assets that might use this asset as their package object
	ArrayList* assets = MOG_DBInboxAPI::InboxGetAssetListFromProperty(S"Drafts", S"", relationshipPropertyWithObjectName);
	if (assets->Count > 0)
	{
		// Add each contained asset 
		for (int i = 0; i < assets->Count; i++)
		{
			String* assetFullName = dynamic_cast<String*>(assets->Item[i]);
			MOG_Filename* tempAssetFilename = new MOG_Filename(assetFullName);
			MOG_Filename* assetFilename = new MOG_Filename(tempAssetFilename->GetAssetEncodedInboxPath(MOG_ControllerProject::GetUserName_DefaultAdmin(), S"Drafts"));

			// Check if an LOD is specified?
			if (iLOD > 0)
			{
				// Check if this LOD doesn't match this asset?
				if (String::Compare(LOD, MOG_ControllerProject::DetectLODFromAssetLabel(tempAssetFilename->GetAssetLabel()), true) != 0)
				{
					continue;
				}
			}

			containedAssets->Item[assetFilename->GetAssetFullName()] = assetFilename;
		}
	}

	return new ArrayList(containedAssets->Values);
}


String* MOG_ControllerProject::GenerateLODName(String* AssetLabel, String* LOD)
{
	// Check if we have something to join in the specified LOD?
	if (LOD->Length)
	{
		// Simply force '_LODX' on the end of the specified AssetLabel
		return String::Concat(AssetLabel, S"_LOD", LOD);
	}

	return AssetLabel;
}


String* MOG_ControllerProject::DetectLODTagFromAssetLabel(String* AssetLabel)
{
	String* LODText = "LOD";
	String* LODStartingDelimiters = "_-.({[";
	String* LODEndingDelimiters = "_-.)}]";

	String* LODTag = "";

	// Find the last index of LODText?
	int pos = AssetLabel->LastIndexOf(LODText);
	if (pos != -1)
	{
		// Strip the off the LODTag
		String* tempLabel = AssetLabel->Substring(0, pos);
		tempLabel = tempLabel->TrimEnd(LODStartingDelimiters->ToCharArray());
		// Obtain the tempLODTag using the tempLabel
		String* tempLODTag = AssetLabel->Substring(tempLabel->Length);
		// Make sure the tempLODTag begins with a valid LODStartingDelimiter?
		if (tempLODTag->IndexOfAny(LODStartingDelimiters->ToCharArray()) == 0)
		{
			// This appears to be a valid LODTag
			LODTag = tempLODTag;
		}
	}

	return LODTag;
}


String* MOG_ControllerProject::StripLODTagFromAssetLabel(String* AssetLabel)
{
	// Obtain the LODTag from the AssetLabel
	String* LODTag = DetectLODTagFromAssetLabel(AssetLabel);
	if (LODTag->Length)
	{
		return AssetLabel->Remove(AssetLabel->Length - LODTag->Length);
	}

	return AssetLabel;
}


String* MOG_ControllerProject::DetectLODFromLODTag(String* LODTag)
{
	String* LODText = "LOD";
	String* LODStartingDelimiters = "_-.({[";
	String* LODEndingDelimiters = "_-.)}]";

	String* LOD = "";

	// Find the last index of LODText?
	int pos = LODTag->LastIndexOf(LODText);
	if (pos != -1)
	{
		// Strip off the LODText
		String* tempLOD = LODTag->Substring(pos + LODText->Length);
		tempLOD = tempLOD->TrimStart(LODStartingDelimiters->ToCharArray());
		tempLOD = tempLOD->TrimEnd(LODEndingDelimiters->ToCharArray());
		try
		{
			Convert::ToUInt32(tempLOD);
			LOD = tempLOD;
		}
		catch(...)
		{
		}
	}

	return LOD;
}


String* MOG_ControllerProject::DetectLODFromAssetLabel(String* AssetLabel)
{
	String* LODTag = DetectLODTagFromAssetLabel(AssetLabel);
	return DetectLODFromLODTag(LODTag);
}


ArrayList *MOG_ControllerProject::GetAllAssetsBySyncLocation(String *syncLocation, String *platform)
{
	return MOG_DBAssetAPI::GetAllAssetsBySyncLocation(syncLocation, platform);
}


ArrayList *MOG_ControllerProject::ResolveAssetNameListForPlatformSpecificAssets(ArrayList *assets, String* platformName)
{
	// Make sure we have a platform specified
	if (platformName->Length)
	{
		// Scan all the assets in the list...Can't automatically increment counter because of the potential remove
		for(int a1 = 0; a1 < assets->Count - 1; /* a1++ */ )
		{
			MOG_Filename *assetFilename1 = __try_cast<MOG_Filename*>(assets->Item[a1]);

			// Check if this asset's platform is not the '{All}' platform?  and 
			// Check if this asset's platform is not the specified platform?
			if (String::Compare(assetFilename1->GetAssetPlatform(), S"All", true) != 0 &&
				String::Compare(assetFilename1->GetAssetPlatform(), platformName, true) != 0)
			{
				// Remove this asset because it is completely out of scope for the specified platform
				assets->RemoveAt(a1);
				// Don't change our counter bacause the array collapsed around us on the remove
				continue;
			}

			// Scan for duplicate asset names starting at the next item...Can't automatically increment counter because of the potential remove
			for(int a2 = a1 + 1; a2 < assets->Count; /* a2++ */ )
			{
				MOG_Filename *assetFilename2 = __try_cast<MOG_Filename*>(assets->Item[a2]);

				// Check for matching classifications and labels?
				if (String::Compare(assetFilename1->GetAssetClassification(), assetFilename2->GetAssetClassification(), true) == 0 &&
					String::Compare(assetFilename1->GetAssetLabel(), assetFilename2->GetAssetLabel(), true) == 0)
				{
					// Check if assetFilename1's platform matches the specified platform?
					if (String::Compare(assetFilename1->GetAssetPlatform(), platformName, true) == 0)
					{
						// Remove the other asset in favor of the matching platform specific asset
						assets->RemoveAt(a2);
						// Don't change our counter bacause the array collapsed around us on the remove
						continue;
					}
					// Check if assetFilename2's platform matches the specified platform?
					if (String::Compare(assetFilename2->GetAssetPlatform(), platformName, true) == 0)
					{
						// Remove the other asset in favor of the matching platform specific asset
						// Post decrement a1 so the parent loop will recheck the same index because the remove just collapsed the array around us
						assets->RemoveAt(a1--);
						// Stop scanning the second loop because we just removed the parent that initiated the second scan
						break;
					}
				}

				// Increment counter
				a2++;
			}

			// Increment counter
			a1++;
		}
	}

	// Return the resolved list of assets
	return assets;
}


MOG_Filename *MOG_ControllerProject::CreatePackage(MOG_Filename *packageFilename, String *syncTargetPath)
{
	String* jobLabel = String::Concat(S"NewPackageCreated.", MOG_ControllerSystem::GetComputerName(), S".", MOG_Time::GetVersionTimestamp());

	MOG_Filename* newPackageFilename = CreatePackage(packageFilename, syncTargetPath, jobLabel);
	// Since we created the job, we always want to start the job
	StartJob(jobLabel);

	return newPackageFilename;
}

MOG_Filename *MOG_ControllerProject::CreatePackage(MOG_Filename *packageFilename, String *syncTargetPath, String* jobLabel)
{
	if (packageFilename != NULL)
	{
		MOG_Project* pProject = GetProject();
		if (pProject)
		{
			//TODO: Validate extension here

			// Attempt to use any timestamp specified in the packageFilename first
			String *timestamp = packageFilename->GetVersionTimeStamp();
			if (timestamp->Length == 0)
			{
				// Construct a new timestamp
				MOG_Time::GetVersionTimestamp();
			}
			// Get a path to our repository
			MOG_Filename* repositoryName = MOG_ControllerRepository::GetAssetBlessedVersionPath(packageFilename, timestamp);

			// Seed the IsPackage property
			ArrayList* properties = new ArrayList;
			properties->Add(MOG_PropertyFactory::MOG_Asset_InfoProperties::New_IsPackage(true));
			properties->Add(MOG_PropertyFactory::MOG_Sync_OptionsProperties::New_SyncTargetPath(syncTargetPath));

			// Import this asset with its properties
			MOG_Filename *newPackageFilename = MOG_ControllerAsset::CreateAsset(repositoryName, "", NULL, NULL, properties, false, false);

			// Schedule this asset for posting under this project name
			if (AddAssetForPosting(newPackageFilename, jobLabel))
			{
				// Post the projects new assets
				PostAssets(pProject->GetProjectName(), GetBranchName(), jobLabel, true);
			}
		}
	}

	return NULL;
}


bool MOG_ControllerProject::StartJob(String *jobLabel)
{
	// Start the job
	MOG_Command *startJob = MOG_CommandFactory::Setup_StartJob(jobLabel);
	return MOG_ControllerSystem::GetCommandManager()->SendToServer(startJob);
}


bool MOG_ControllerProject::RestartJob(MOG_Filename *assetFilename, String *revisionTimestamp, String *jobLabel)
{
	bool bRestarted = false;

	String *comment = String::Concat(MOG_ControllerProject::GetUserName_DefaultAdmin(), S" restarted a stalled network job.");
	MOG_Filename *blessedAssetFilename = MOG_ControllerRepository::GetAssetBlessedVersionPath(assetFilename, revisionTimestamp);
	if (MOG_ControllerProject::MakeAssetCurrentVersion(blessedAssetFilename, comment, jobLabel))
	{
		// We also need to start the job
		if (StartJob(jobLabel))
		{
			bRestarted = true;
		}
	}

	return bRestarted;
}

