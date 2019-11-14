//--------------------------------------------------------------------------------
//	MOG_System.cpp
//	
//	
//--------------------------------------------------------------------------------

#include "StdAfx.h"

#include "MOG_Tokens.h"
#include "MOG_Define.h"
#include "MOG_DosUtils.h"
#include "MOG_StringUtils.h"
#include "MOG_Database.h"
#include "MOG_DBProjectAPI.h"
#include "MOG_ControllerSystem.h"
#include "MOG_ControllerProject.h"
#include "MOG_System.h"



//--------------------------------------------------------------------------------
//	MOG
//--------------------------------------------------------------------------------
MOG_System::MOG_System(void)
{
	// Get the startup info
	mRepositoryPath = "";
	mExternalAssetRepositoryPath = "";
	mExternalAssetArchivePath = "";
	mConfigFile = new MOG_Ini();
	mConfigFilename = "";
	mProjectNames = new ArrayList;
}

bool MOG_System::Load(String *configFilename)
{
	// Retain the ConfigFile in the System's member variables
	// Go ahead and set mConfigFilename just in-case the network drive is unmapped... This way we'll automatically connect
	mConfigFilename = configFilename;

	// Make sure mConfigFilename file exists
	if (DosUtils::FileExistFast(mConfigFilename))
	{
		if (mConfigFile->Load(mConfigFilename))
		{
			int numProjects;

			// Set the System variables
			mRepositoryPath = MOG_Main::GetDefaultRepositoryPath(configFilename);
			// Check if there is an ExternalAssetRepository specified
			if (mConfigFile->KeyExist("MOG", "ExternalAssetRepository"))
			{
				mExternalAssetRepositoryPath = mConfigFile->GetString("MOG", "ExternalAssetRepository");
			}
			else
			{
				mExternalAssetRepositoryPath = "";
			}
			// Check if there is an ExternalAssetArchive specified
			if (mConfigFile->KeyExist("MOG", "ExternalAssetArchive"))
			{
				mExternalAssetRepositoryPath = mConfigFile->GetString("MOG", "ExternalAssetArchive");
			}
			else
			{
				mExternalAssetArchivePath = "";
			}

			// Make sure to erase the old list of Projects
			mProjectNames = new ArrayList;
			if (mConfigFile->SectionExist(S"Projects"))
			{
				// Determine how many projects MOG contains
				numProjects = mConfigFile->CountKeys("Projects");
				for (int proj = 0; proj < numProjects; proj++)
				{
					// Get this project's name
					mProjectNames->Add(mConfigFile->GetKeyNameByIndexSLOW("Projects", proj));
				}

				mProjectNames->Sort();
			}
			return true;
		}
	}
	return false;
}


String *MOG_System::LocateRipper(String *type, MOG_Filename *assetFullFilename)
{
	String *ripperFullFilename;

	// Check for 'type.ProjectKey.AssetKey.Platform' in all tool paths
	// Check for 'type.ProjectKey.AssetKey.All' in all tool paths
	// Check for 'type.ProjectKey.All.Platform' in all tool paths
	// Check for 'type.ProjectKey.All.All' in all tool paths
	// Check for 'type.All.AssetKey.Platform' in all tool paths
	// Check for 'type.All.AssetKey.All' in all tool paths
	// Check for 'type.All.All.All' in all tool paths

	return ripperFullFilename;
}

MOG_Project *MOG_System::GetProject(String *projectName)
{
	// Find project
	for(int p = 0; p < mProjectNames->Count; p++)
	{
		if( !String::Compare(__try_cast<String *>(mProjectNames->Item[p]), projectName, true) )
		{
			MOG_Project *newProject = new MOG_Project;

			// Load the Project's configuration file
			// Make sure we reolve the SystemRepositoryPath token
			String *configFilename = GetConfigFile()->GetString(projectName, "ConfigFile");
			String *untokenizedConfigFilename = configFilename->ToLower()->Replace(TOKEN_SystemRepositoryPath->ToLower(), MOG_ControllerSystem::GetSystemRepositoryPath());
			// Temp Code to help migrate old untokenized configFilenames
			if (String::Compare(configFilename, untokenizedConfigFilename, true) == 0)
			{
				// Save out a properly tokenized configFilename
				String *tokenizedConfigFilename = configFilename->ToLower()->Replace(MOG_ControllerSystem::GetSystemRepositoryPath()->ToLower(), TOKEN_SystemRepositoryPath);
				GetConfigFile()->PutString(projectName, "ConfigFile", tokenizedConfigFilename);
				GetConfigFile()->Save();
			}
			if (newProject->Load(untokenizedConfigFilename, S""))
			{
				return newProject;
			}
		}
	}

	return NULL;
}


MOG_Project *MOG_System::ProjectCreate(String *projectName)
{
	// Check for invalid characters
	String *invalidChars = MOG_ControllerSystem::GetInvalidMOGCharacters();
	// Don't allow '(' or ')' because it colides with the new encoded classifications
	invalidChars = String::Concat(invalidChars, S"()");
	if (MOG_ControllerSystem::InvalidCharactersCheck(projectName, invalidChars, false))
	{
		String *message = String::Concat(	S"The specified project name contains invalid characters.\n",
											S"INVALID CHARACTERS: ", invalidChars, S"\n\n",
											S"The project was not created.");
		throw new Exception(message);
		return NULL;
	}

	// establish connection to database
	if (MOG_ControllerSystem::GetDB()->Connect(projectName, "Current"))
	{
		if(MOG_ControllerSystem::GetDB()->ProjectTablesExist(projectName))
		{
			if(MOG_Prompt::PromptResponse(S"Project Already Exists",  String::Concat(S"It appears you are attempting to install a project over a previously created MOG project.",
																					       S"\nYou are either attempting to...",
																					       S"\n  1.  Install over an orphan project",
																					       S"\n  2.  Install over an existing project.",
																					       S"\nNote: Be careful if your company uses more than one MOG server as this could be an active project on another server.",
																					       S"\nThe safest way to avoid this issue is to select 'No' and create a project with a different name.",
																					       S"\nDo you want to overwrite this project?"), MOGPromptButtons::YesNo)== MOGPromptResult::Yes)
			{
				MOG_ControllerSystem::GetDB()->DeleteProjectTables(projectName);
			}
			else
			{
				return NULL;
			}
		}
		if (MOG_ControllerSystem::GetDB()->VerifyTables(projectName))
		{
			MOG_Project *project = new MOG_Project;

			if (project->Create(GetSystemProjectsPath(), projectName))
			{
				// Add project and config file to main ini
				mConfigFile->PutSectionString("PROJECTS", projectName);
				String *tokenizedConfigFilename = project->GetProjectConfigFilename()->ToLower()->Replace(MOG_ControllerSystem::GetSystemRepositoryPath()->ToLower(), TOKEN_SystemRepositoryPath);
				mConfigFile->PutString(projectName, "ConfigFile", tokenizedConfigFilename);
				mConfigFile->Save(mConfigFilename);

				mProjectNames->Add(projectName);
				mProjectNames->Sort();

				MOG_DBProjectAPI::AddBranch(S"Current", S"Admin", S"");
				
				MOG_ControllerProject::LoginProject(projectName, S"");
				
				// create default user and current branch
				MOG_UserDepartment *admin = new MOG_UserDepartment("Admin");
				project->UserDepartmentAdd(admin);

				MOG_User *adminUser = new MOG_User();
				adminUser->SetUserDepartment("Admin");
				adminUser->SetUserName("Admin");
				adminUser->SetBlessTarget("MasterData");
				project->UserAdd(adminUser);

				return project;
			}
		}
	}

	return NULL;
}


bool MOG_System::ProjectRemove(String *projectName)
{
	// Make sure we have a valid name?
	if (projectName->Length)
	{
		// Remove node
		for(int p = 0; p < mProjectNames->Count; p++)
		{
			if( !String::Compare(__try_cast<String *>(mProjectNames->Item[p]), projectName, true) )
			{
				// Remove Directory structure
				DosUtils::DirectoryDeleteFast(String::Concat(GetSystemProjectsPath(), S"\\", projectName));

				mProjectNames->RemoveAt(p);

				break;
			}
		}

		// Remove ini node
		mConfigFile->RemoveString("Projects", projectName);
		mConfigFile->RemoveSection(projectName);
		mConfigFile->Save(mConfigFilename);

		return true;
	}

	return false;
}

bool MOG_System::LoadServerInformation()
{
	// Make sure that the network drive is still valid?
	if (DosUtils::FileExistFast(GetConfigFilename()))
	{
		// Always load/reload the MOG_SYSTEM_CONFIGFILENAME file incase the server's info has changed
		MOG_Ini *pConfigFile = GetConfigFile();
		if (pConfigFile->Load())
		{
			// Get the server's info
			ServerName = pConfigFile->GetString("Network", "ServerName");
			ServerIP = pConfigFile->GetString("Network", "ServerIP");
			ServerMacAddress = pConfigFile->GetString("Network", "ServerMacAddress");
			ServerPort = pConfigFile->GetValue("Network", "ServerPort");
			ServerMajorVersion = pConfigFile->GetValue(S"Network", S"MajorVersion");
			ServerMinorVersion = pConfigFile->GetValue(S"Network", S"MinorVersion");

			return true;
		}
	}

	return false;
}