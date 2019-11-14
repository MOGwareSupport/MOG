//--------------------------------------------------------------------------------
//	MOG_Project.cpp
//	
//	
//--------------------------------------------------------------------------------

#include "StdAfx.h"

#include "MOG_DosUtils.h"
#include "MOG_Time.h"
#include "MOG_StringUtils.h"
#include "MOG_ControllerRepository.h"
#include "MOG_ControllerAsset.h"
#include "MOG_ControllerProject.h"
#include "MOG_ControllerSystem.h"
#include "MOG_ControllerLibrary.h"
#include "MOG_ControllerInbox.h"
#include "MOG_DBAssetAPI.h"
#include "MOG_DBInboxAPI.h"
#include "MOG_Tokens.h"
#include "MOG_Progress.h"
#include "MOG_DBReports.h"
#include "MOG_CommandFactory.h"
#include "MOG_ControllerReinstance.h"

#include "MOG_Project.h"



//--------------------------------------------------------------------------------
//	MOG_Project
//--------------------------------------------------------------------------------
MOG_Project::MOG_Project(void)
{
	mProjectName = "";
	mProjectKey = "";
	mProjectConfigFilename = "";
	mProjectPath = "";
	mProjectToolsPath = "";
	mProjectUsersPath = "";

	mConfigFile = new MOG_PropertiesIni();

	mAssetsInfo = new ArrayList();
	mPlatforms = new ArrayList();
	mUserDepartments = new ArrayList();
	mUsers = new ArrayList();
	mSuppressProjectRefreshEvent = true;
}

bool MOG_Project::Reload()
{
	// Clear the ArrayLists to make sure we don't retain users/platforms/etc. that have been removed already
	mPlatforms->Clear();
	mUserDepartments->Clear();
	mUsers->Clear();

	return Load();
}

bool MOG_Project::Load()
{
	return Load(mProjectConfigFilename, MOG_ControllerProject::GetBranchName());
}

bool MOG_Project::Load(String *configFilename, String *branchName)
{
	// Check if we are missing a specified branch?
	if (branchName == NULL ||
		branchName->Length == 0)
	{
		// Default to the 'Current' branch
		branchName = (MOG_ControllerProject::GetBranchName()->Length) ? MOG_ControllerProject::GetBranchName() : S"Current";
	}

	if (mConfigFile->Load(configFilename))
	{
		// Project definitions
		mProjectConfigFilename = configFilename;

		mProjectName = mConfigFile->GetString("PROJECT", "Name");
		mProjectKey = mConfigFile->GetString("PROJECT", "Key");
		mProjectPath = MOG_Tokens::GetFormattedString(mConfigFile->GetString("PROJECT", "Path"), MOG_Tokens::GetSystemTokenSeeds());
		mProjectToolsPath = MOG_Tokens::GetFormattedString(mConfigFile->GetString("PROJECT", "Tools"), MOG_Tokens::GetSystemTokenSeeds());
		mProjectUsersPath = MOG_Tokens::GetFormattedString(mConfigFile->GetString("PROJECT", "Users"), MOG_Tokens::GetSystemTokenSeeds());

		// Initialize our Database for this project so we can finish loading
		if (MOG_ControllerSystem::InitializeDatabase(S"", mProjectName, branchName))
		{
			// Verify this project's database tables exist?
			if (MOG_ControllerSystem::GetDB()->ProjectTablesExist(mProjectName))
			{
				// Get valid platforms
				ArrayList *platforms = MOG_DBProjectAPI::GetPlatformsInBranch(branchName);
				if (platforms)
				{
					for (int platform = 0; platform < platforms->Count; platform++)
					{
						// Create platform
						MOG_Platform *newPlatform = new MOG_Platform;
						newPlatform->mPlatformName = __try_cast<String *>(platforms->Item[platform]);
						mPlatforms->Add(newPlatform);
					}

					// Get user departments
					ArrayList *departments = MOG_DBProjectAPI::GetDepartments();
					if (departments)
					{
						for (int department = 0; department < departments->Count; department++)
						{
							// Create category
							MOG_UserDepartment *newDepartment = new MOG_UserDepartment(__try_cast<String *>(departments->Item[department]));
							mUserDepartments->Add(newDepartment);
						}

						// Get users
						mUsers = MOG_DBProjectAPI::GetActiveUsers();
						if (mUsers)
						{
							return true;
						}
					}
				}
			}
		}
	}
	return false;
}


bool MOG_Project::Create(String *systemProjectsPath, String *projectName)
{
	// Populate this project
	mProjectName			= projectName;
	mProjectKey				= projectName->Substring(0, (projectName->Length > 3) ? 3 : projectName->Length);
	mProjectPath			= String::Concat(systemProjectsPath, S"\\", projectName);
	mProjectConfigFilename	= String::Concat(mProjectPath, S"\\tools\\", projectName, S".ini");
	mProjectToolsPath		= String::Concat(mProjectPath, S"\\tools");
	mProjectUsersPath		= String::Concat(mProjectPath, S"\\users");

	// Create valid project directory structure
	DosUtils::DirectoryCreate(mProjectPath);
	DosUtils::DirectoryCreate(mProjectToolsPath);
	DosUtils::DirectoryCreate(mProjectUsersPath);
	DosUtils::DirectoryCreate(String::Concat(mProjectPath, S"\\Archive"));
	DosUtils::DirectoryCreate(String::Concat(mProjectPath, S"\\Assets"));
	DosUtils::DirectoryCreate(String::Concat(mProjectPath, S"\\Tools\\Images"));
	DosUtils::DirectoryCreate(String::Concat(mProjectPath, S"\\Tools\\Viewers"));
	DosUtils::DirectoryCreate(String::Concat(mProjectPath, S"\\Tools\\Utilities"));
	DosUtils::DirectoryCreate(String::Concat(mProjectPath, S"\\Tools\\ClientConfigs"));
	DosUtils::DirectoryCreate(String::Concat(mProjectPath, S"\\Tools\\Property.Menus"));
	DosUtils::DirectoryCreate(String::Concat(mProjectPath, S"\\Tools\\Rippers"));
	DosUtils::DirectoryCreate(String::Concat(mProjectPath, S"\\Tools\\BuildScripts"));
	DosUtils::DirectoryCreate(String::Concat(mProjectPath, S"\\Tools\\PackageScripts"));
	DosUtils::DirectoryCreate(String::Concat(mProjectPath, S"\\Tools\\Configs"));

	// Create valid project ini
	mConfigFile = new MOG_PropertiesIni(mProjectConfigFilename);

	// [PROJECT]
	mConfigFile->PutString("PROJECT", "Name", mProjectName);
	mConfigFile->PutString("PROJECT", "Key", mProjectKey);
	mConfigFile->PutString("PROJECT", "Tools", String::Concat(S"{SystemProjectsPath}\\", projectName, S"\\Tools"));
	mConfigFile->PutString("PROJECT", "Users", String::Concat(S"{SystemProjectsPath}\\", projectName, S"\\Users"));
	mConfigFile->PutString("PROJECT", "Path", String::Concat(S"{SystemProjectsPath}\\", projectName));
	
	mConfigFile->Save();

	return true;
}

bool MOG_Project::Offline(String *targetPath)
{
	ProgressDialog* progress = new ProgressDialog(S"Going Offline", S"Please wait while the system files are cached.", new DoWorkEventHandler(this, &MOG_Project::Offline_Worker), targetPath, true);
	if (progress->ShowDialog() == DialogResult::OK)
	{
		return true;
	}

	return false;
}

void MOG_Project::Offline_Worker(Object* sender, DoWorkEventArgs* e)
{
	BackgroundWorker* worker = dynamic_cast<BackgroundWorker*>(sender);
	String* targetPath = dynamic_cast<String*>(e->Argument);

	// Copy project files
	String *projectPath = String::Concat(targetPath, S"\\MOG\\Projects");
	MOG_Project *project = CopyProject(projectPath, GetProjectName());

	if (worker != NULL)
	{
		worker->ReportProgress(10, S"Caching System Tools directory...");
	}

	// Copy the system tools
	String *systemToolsPath = MOG_ControllerSystem::GetSystem()->GetSystemToolsPath();
	String *drive = DosUtils::PathGetRootPath(systemToolsPath);
	String *targetSystemToolsPath = systemToolsPath->ToUpper()->Replace(drive, targetPath);
	if (!DosUtils::DirectoryCopyModified(systemToolsPath, targetSystemToolsPath))
	{
		MOG_Report::ReportMessage("Offline - System Tools Path", String::Concat(S"The target system path(", systemToolsPath, S") could not be copied!"), Environment::StackTrace, MOG_ALERT_LEVEL::ALERT);
		e->Cancel = true;
		return;
	}

	if (worker != NULL)
	{
		worker->ReportProgress(30, S"Fixing up paths...");
	}

	// Copy my user box
	MOG_User *user = MOG_ControllerProject::GetUser();
	String *userPath = user->GetUserPath();
	String *userTargetPath = userPath->ToUpper()->Replace(drive, targetPath);

	if (worker != NULL)
	{
		worker->ReportProgress(50, S"Caching user directory...");
	}

	if (!DosUtils::DirectoryCopyModified(userPath, userTargetPath))
	{
		MOG_Report::ReportMessage("Offline - Copy User Path", String::Concat(S"The target user(", user->GetUserName(), S") could not be copied!"), Environment::StackTrace, MOG_ALERT_LEVEL::ALERT);
		e->Cancel = true;
		return;
	}

	if (worker != NULL)
	{
		worker->ReportProgress(100, S"Done.");
	}
}

bool MOG_Project::DuplicateProject(String *newProjectName)
{
	ProgressDialog* progress = new ProgressDialog(S"Duplicating Project", S"Please wait while the project is duplicated.", new DoWorkEventHandler(this, &MOG_Project::DuplicateProject_Worker), newProjectName, true);
	if (progress->ShowDialog() == DialogResult::OK)
	{
		return true;
	}

	return false;
}

void MOG_Project::DuplicateProject_Worker(Object* sender, DoWorkEventArgs* e)
{
	BackgroundWorker* worker = dynamic_cast<BackgroundWorker*>(sender);
	String* newProjectName = dynamic_cast<String*>(e->Argument);

	// Copy the existing project's directory to a new directory
	String *sourceProjectDirectory = mProjectPath;
	String *targetProjectDirectory = Path::Combine(MOG_ControllerSystem::GetSystem()->GetSystemProjectsPath(), newProjectName);

	// Proceed to create our new project
	if (MOG_ControllerSystem::GetSystem()->ProjectCreate(newProjectName))
	{
		// Erase all the tables so we know there is no stale data
		MOG_ControllerSystem::GetDB()->DeleteProjectTables(newProjectName);
		// Create new empty tables so we will have a blank slate
		MOG_ControllerSystem::GetDB()->VerifyTables(newProjectName);

		// Export the database tables into the old project's directory so it will be duplicated
		if (worker != NULL)
		{
			worker->ReportProgress(0, S"Exporting existing project's database tables.");
		}
		if (MOG_Database::ExportProjectTables(mProjectName, mProjectPath))
		{
			// Copy the existing project's repository
			if (worker != NULL)
			{
				worker->ReportProgress(0, S"Copying existing project's repository");
			}
			if (MOG_ControllerSystem::DirectoryCopyEx(sourceProjectDirectory, targetProjectDirectory, worker))
			{
				// Fixup all the known filenames that contain a reference to the project's name
				String *newProjectPath			 = Path::Combine(MOG_ControllerSystem::GetSystemProjectsPath(), newProjectName);
				String *newProjectToolsPath		 = Path::Combine(newProjectPath, S"Tools");
				String *newProjectUsersPath		 = Path::Combine(newProjectPath, S"Users");
				String *oldProjectFilenameHeader = String::Concat(mProjectName, S".");
				String *newProjectFilenameHeader = String::Concat(newProjectName, S".");

				// Delete the original project's primary configuration file
				String *oldProjectConfigFilename = String::Concat(newProjectToolsPath, S"\\", mProjectName, S".ini");
				DosUtils::FileDeleteFast(oldProjectConfigFilename);

				// Rename the project's custom tool box controls
				String *newProjectClientConfigsDirectory = Path::Combine(newProjectToolsPath, S"ClientConfigs");
				ArrayList* files = DosUtils::FileGetRecursiveList(newProjectClientConfigsDirectory, "*.*");
				if (files != NULL)
				{
					// Add all the sizes of the files
					for (int f = 0; f < files->Count; f++)
					{
						String *oldFilename = __try_cast<String *>(files->Item[f]);

						// Check if this file contained the name of the project?
						String *oldProjectFilename = Path::GetFileName(oldFilename);
						String *newProjectFilename = oldProjectFilename->Replace(oldProjectFilenameHeader, newProjectFilenameHeader);
						// Check if the filename was changed?
						if (String::Compare(newProjectFilename, oldProjectFilename, true) != 0)
						{
							String *newFilename = Path::Combine(Path::GetDirectoryName(oldFilename), newProjectFilename);
							DosUtils::FileRenameFast(oldFilename, newFilename, true);
						}
					}
				}

				// Rename the project's user-level custom tool box controls
				files = DosUtils::FileGetRecursiveList(newProjectUsersPath, "*.*");
				if (files != NULL)
				{
					// Add all the sizes of the files
					for (int f = 0; f < files->Count; f++)
					{
						String *oldFilename = __try_cast<String *>(files->Item[f]);

						// Check if this file contained the name of the project?
						String *oldProjectFilename = Path::GetFileName(oldFilename);
						String *newProjectFilename = oldProjectFilename->Replace(oldProjectFilenameHeader, newProjectFilenameHeader);
						// Check if the filename was changed?
						if (String::Compare(newProjectFilename, oldProjectFilename, true) != 0)
						{
							String *newFilename = Path::Combine(Path::GetDirectoryName(oldFilename), newProjectFilename);
							DosUtils::FileRenameFast(oldFilename, newFilename, true);
						}
					}
				}

				if (worker != NULL)
				{
					worker->ReportProgress(0, S"Importing new project's database tables.");
				}
				// Import the new Database tables under the new project
				MOG_Database::ImportProjectTables(newProjectName, targetProjectDirectory);
				// We now need to login to this project before we start working on remapping asset properties
				MOG_ControllerProject::LoginProject(newProjectName, "Current");

				if (worker != NULL)
				{
					worker->ReportProgress(0, S"Renaming the new project's classifications.");
				}
				// Rename all the classifications to reflect the new project's name
				MOG_DBAssetAPI::RenameAdamClassificationName(mProjectName, newProjectName);

				if (worker != NULL)
				{
					worker->ReportProgress(0, S"Fixing up relational asset properties.");
				}

				DuplicateProject_RepairClassificationsProperties(newProjectName, worker);
				DuplicateProject_RepairAssetsProperties(newProjectName, worker);
				DuplicateProject_RepairInboxAssets(newProjectName, worker);
			}
		}
	}
}

void MOG_Project::DuplicateProject_RepairClassificationsProperties(String* newProjectName, BackgroundWorker* worker)
{
	// Get both the archived assets and inbox assets
	ArrayList* classifications = MOG_DBAssetAPI::GetAllClassifications();
	classifications->Sort();

	// Fix each asset
	for (int i = 0; i < classifications->Count; i++)
	{
		String *classification = dynamic_cast<String*>(classifications->Item[i]);

		// Inform user about our progress
		String* message = String::Concat(S"Fixing up relational asset properties.\n",
										 classification );
		if (worker != NULL)
		{
			worker->ReportProgress((i * 100) / classifications->Count, message);
		}

		// Open this classification's properties for verification
		MOG_Properties *classificationProperties = MOG_Properties::OpenClassificationProperties(classification);
		if (classificationProperties)
		{
			// Repair this asset's properties
			DuplicateProject_RepairProperties(newProjectName, classificationProperties);

			// Make sure we close our asset's controller
			classificationProperties->Close();
		}
	}
}

void MOG_Project::DuplicateProject_RepairAssetsProperties(String* newProjectName, BackgroundWorker* worker)
{
	// Get all of the archived assets
	ArrayList* assets = MOG_DBAssetAPI::GetAllArchivedAssets();
	assets->Sort();
	// Fix each asset
	for (int i = 0; i < assets->Count; i++)
	{
		MOG_Filename *assetFilename = dynamic_cast<MOG_Filename*>(assets->Item[i]);

		// Inform user about our progress
		String* message = String::Concat(S"Fixing up relational asset properties.\n",
										 assetFilename->GetAssetClassification(), S"\n",
										 assetFilename->GetAssetLabel());
		if (worker != NULL)
		{
			worker->ReportProgress((i * 100) / assets->Count, message);
		}

		// Obtain the location of this asset in the repository
		MOG_Filename* blessedAssetFilename = MOG_ControllerRepository::GetAssetBlessedVersionPath(assetFilename, assetFilename->GetVersionTimeStamp());
		// Open this asset for verification
		MOG_ControllerAsset *asset = MOG_ControllerAsset::OpenAsset(blessedAssetFilename);
		if (asset)
		{
			// Repair this asset's properties
			DuplicateProject_RepairProperties(newProjectName, asset->GetProperties());

			// Make sure we close our asset's controller
			asset->Close();
		}
	}
}

void MOG_Project::DuplicateProject_RepairInboxAssets(String* newProjectName, BackgroundWorker* worker)
{
	// Get all of the inbox assets
	ArrayList* assets = MOG_DBInboxAPI::InboxGetAssetList();
	assets->Sort();
	// Fix each asset
	for (int i = 0; i < assets->Count; i++)
	{
		MOG_Filename *assetFilename = dynamic_cast<MOG_Filename*>(assets->Item[i]);

		// Inform user about our progress
		String* message = String::Concat(S"Fixing up relational asset properties.\n",
										 assetFilename->GetAssetClassification(), S"\n",
										 assetFilename->GetAssetLabel());
		if (worker != NULL)
		{
			worker->ReportProgress((i * 100) / assets->Count, message);
		}

		// Open this asset for verification
		MOG_ControllerAsset *asset = MOG_ControllerAsset::OpenAsset(assetFilename);
		if (asset)
		{
			// Repair this asset's properties
			DuplicateProject_RepairProperties(newProjectName, asset->GetProperties());

			// Make sure we close our asset's controller
			asset->Close();
		}
	}
}

void MOG_Project::DuplicateProject_RepairProperties(String* newProjectName, MOG_Properties* properties)
{
	String *sourcePath = properties->SourcePath;
	String* oldPath = "";
	String* newPath = "";

	// Detect if our oldPath should be changed to reflect the newPath?
	// Are we still looking for a newPath?
	if (newPath->Length == 0)
	{
		// First check if this contains the default library path?
		String* oldLibraryPath = String::Concat(S":\\MOG_Library\\", mProjectName, S"\\");
		String* newLibraryPath = String::Concat(S":\\MOG_Library\\", newProjectName, S"\\");
		int pos = sourcePath->IndexOf(oldLibraryPath, StringComparison::CurrentCultureIgnoreCase);
		if (pos != -1)
		{
			oldPath = String::Concat(sourcePath->Substring(0, pos), oldLibraryPath);
			newPath = String::Concat(sourcePath->Substring(0, pos), newLibraryPath);
		}
	}
	// Are we still looking for a newPath?
	if (newPath->Length == 0)
	{
		// Check if the project's name is at the root by including the ':'?
		String* oldProjectPath = String::Concat(":\\", mProjectName, "\\");
		String* newProjectPath = String::Concat(":\\", newProjectName, "\\");
		int pos = sourcePath->IndexOf(oldProjectPath, StringComparison::CurrentCultureIgnoreCase);
		if (pos != -1)
		{
			oldPath = String::Concat(sourcePath->Substring(0, pos), oldProjectPath);
			newPath = String::Concat(sourcePath->Substring(0, pos), newProjectPath);
		}
	}

	// Did we determine we need a newPath?
	if (newPath->Length)
	{
		// Check if the '{AssetStats}SourcePath' property contains the old project's name?
		if (sourcePath->StartsWith(oldPath, StringComparison::CurrentCultureIgnoreCase))
		{
			// Fixup all the '{AssetStats}SourcePath' property to be relational to the new project
			properties->SourcePath = String::Concat(newPath, sourcePath->Substring(oldPath->Length));
		}
	}

	// Check if the '{AssetStats}SourcePath' property contains the old project's name?
	ArrayList* relationships = properties->GetRelationships();
	// Scan the relationships in this classifications
	for (int i = 0; i < relationships->Count; i++)
	{
		MOG_Property *oldProperty = __try_cast <MOG_Property *> (relationships->Item[i]);

		// Fixup all the '{Relationships}' properties where specific assets are referenced
		// Check if this SourceFile was within the library directory?
		MOG_Filename* assetFilename = new MOG_Filename(oldProperty->mPropertyKey);
		if (assetFilename->GetFilenameType() == MOG_FILENAME_TYPE::MOG_FILENAME_Asset)
		{
			String* oldAdamObject = String::Concat(mProjectName, S"~");
			String* newAdamObject = String::Concat(newProjectName, S"~");

			// Check if this asset's classification begins with the old project?
			String *assetName = oldProperty->mPropertyKey;
			if (assetName->StartsWith(oldAdamObject, StringComparison::CurrentCultureIgnoreCase))
			{
				// Fixup all the '{Relationships}SourceFile' properties for paths that came from the MOG Library
				properties->RemoveRelationship(oldProperty);
				assetName = String::Concat(newAdamObject, assetName->Substring(oldAdamObject->Length));
				MOG_Property* newProperty = new MOG_Property(oldProperty->mSection, oldProperty->mPropertySection, assetName, oldProperty->mValue);
				properties->AddRelationship(newProperty);
			}
		}
		else
		{
			// Did we determine we need a newPath?
			if (newPath->Length)
			{
				// Check if this was within the library directory?
				String *file = oldProperty->mPropertyKey;
				if (file->StartsWith(oldPath, StringComparison::CurrentCultureIgnoreCase))
				{
					// Fixup the '{Relationships}' property that came from the MOG Library
					properties->RemoveRelationship(oldProperty);
					file = String::Concat(newPath, file->Substring(oldPath->Length));
					MOG_Property* newProperty = new MOG_Property(oldProperty->mSection, oldProperty->mPropertySection, file, oldProperty->mValue);
					properties->AddRelationship(newProperty);
				}
			}
		}
	}
}

bool MOG_Project::CopyUsers(MOG_Project *sourceProject)
{
	// Copy over all the Departments
	for (int a = 0; a < sourceProject->mAssetsInfo->Count; a++)
	{
		MOG_UserDepartment *department = __try_cast<MOG_UserDepartment *>(sourceProject->mAssetsInfo->Item[a]);
		UserDepartmentAdd(department);
	}

	// Copy over all the users
	for (int u = 0; u < sourceProject->mUsers->Count; u++)
	{
		MOG_User *user = __try_cast<MOG_User *>(sourceProject->mUsers->Item[u]);
		UserAdd(user);
	}

	return true;
}

bool MOG_Project::CopyPlatforms(MOG_Project *sourceProject)
{
	// Copy over all the platforms
	for (int p = 0; p < sourceProject->mPlatforms->Count; p++)
	{
		MOG_Platform *platform = __try_cast<MOG_Platform *>(sourceProject->mPlatforms->Item[p]);

		PlatformAdd(platform);
	}
	return true;
}

MOG_Project *MOG_Project::CopyProject(String *systemProjectPath, String *targetProject)
{
	MOG_Project *pProject = new MOG_Project();
	pProject->Create(systemProjectPath, targetProject);

	// Copy over the tools directory
	String *sTools = String::Concat(GetProjectPath(), S"\\tools");
	String *tTools = String::Concat(systemProjectPath, S"\\", targetProject, "\\Tools");
	if (!DosUtils::DirectoryCopyFast(sTools, tTools, true))
	{
		MOG_Report::ReportMessage("Tools Copy", "We could not copy the tools directory of the parent project!", Environment::StackTrace, MOG_ALERT_LEVEL::ALERT);
		return NULL;
	}

	return pProject;
}

void MOG_Project::Save()
{
	// [PROJECT]
	mConfigFile->PutString("PROJECT", "Name", mProjectName);
	mConfigFile->PutString("PROJECT", "Key", mProjectKey);
	mConfigFile->PutString("PROJECT", "Tools", String::Concat(S"{SystemProjectsPath}\\", mProjectName, S"\\Tools"));
	mConfigFile->PutString("PROJECT", "Users", String::Concat(S"{SystemProjectsPath}\\", mProjectName, S"\\Users"));
	mConfigFile->PutString("PROJECT", "Path", String::Concat(S"{SystemProjectsPath}\\", mProjectName));

	mConfigFile->Save();
}

bool MOG_Project::PlatformExists(String *platformName)
{
	for (int i = 0; i < mPlatforms->Count; i++)
	{
		MOG_Platform *platform = __try_cast<MOG_Platform *>(mPlatforms->Item[i]);
		if (String::Compare(platform->mPlatformName, platformName, false) == 0)
		{
			return true;
		}
	}

	return false;
}

MOG_Platform *MOG_Project::GetPlatform(String *platformName)
{
	for (int p = 0; p < mPlatforms->Count; p++)
	{
		MOG_Platform *platform = __try_cast<MOG_Platform *>(mPlatforms->Item[p]);

		if (!String::Compare(platform->mPlatformName, platformName, true) || platformName->Length == 0)
		{
			return platform;
		}
	}
	return NULL;
}


String *MOG_Project::GetPlatformNames()[]
{
	ArrayList *platforms = GetPlatforms();
	String *platformNames[] = new String*[platforms->Count];
	// Loop through all the platforms in this project
	for (int platform = 0; platform < platforms->Count; platform++)
	{
		// Get this platform
		MOG_Platform *pPlatform = __try_cast<MOG_Platform *>(platforms->Item[platform]);
		if (pPlatform)
		{
			platformNames[platform] = pPlatform->mPlatformName;
		}
	}

	return platformNames;
}

// Returns an ArrayList of the names of the root classifications
ArrayList *MOG_Project::GetRootClassificationNames()
{
	return MOG_DBAssetAPI::GetClassificationChildren("", NULL);
}

// Returns an ArrayList of the names of the children classifications of the specified classification
ArrayList *MOG_Project::GetSubClassificationNames(String *classificatioName)
{
	return MOG_DBAssetAPI::GetClassificationChildren(classificatioName, NULL);
}

MOG_Properties *MOG_Project::GetClassificationProperties(String *classification)
{
	return MOG_Properties::OpenClassificationProperties(classification);
}

// Returns an array list of the classification(s) under the *classification inside the branch, *branchName
ArrayList *MOG_Project::GetSubClassifications( String *classification, String *branchName )
{
	return GetSubClassifications( classification, branchName, NULL );
}

ArrayList *MOG_Project::GetSubClassifications( String *classification, String *branchName, ArrayList *properties )
{
	// If we do not  have a valid branchName...
	if( !branchName || branchName->Length < 1 )
	{
		branchName = MOG_ControllerProject::GetBranchName();
	}
	if( properties )
	{
		return MOG::DATABASE::MOG_DBAssetAPI::GetClassificationChildren( classification, branchName, properties );
	}
	else
	{
		return MOG::DATABASE::MOG_DBAssetAPI::GetClassificationChildren( classification, branchName );
	}
}

// Returns ArrayList of non-branch specific classifications
ArrayList *MOG_Project::GetArchivedSubClassifications( String *classificationParent )
{
	return MOG::DATABASE::MOG_DBAssetAPI::GetArchivedClassificationChildren( classificationParent );
}

bool MOG_Project::ClassificationExists(String* classificationFullName)
{
	int classificationID = MOG_DBAssetAPI::GetClassificationIDForBranch(classificationFullName);
	return ( classificationID != 0 );
}

bool MOG_Project::ClassificationAdd(String *classificationFullName)
{
	// Check for invalid characters
	String *invalidChars = MOG_ControllerSystem::GetInvalidMOGCharacters();
	if (MOG_ControllerSystem::InvalidCharactersCheck(classificationFullName, invalidChars, false))
	{
		String *message = String::Concat(	S"The classification contains invalid characters.\n",
											S"INVALID CHARACTERS: ", invalidChars, S"\n\n",
											S"The classification was not added.");
		MOGPromptResult result = MOG_Prompt::PromptResponse(S"Invalid Characters Found", message, NULL, MOGPromptButtons::OK, MOG_ALERT_LEVEL::ALERT);
		return false;
	}

	// Attempt to create the classification
	if (MOG_DBAssetAPI::CreateClassification(classificationFullName, MOG_ControllerProject::GetUserName_DefaultAdmin(), MOG_Time::GetVersionTimestamp()))
	{
		return true;
	}
	return false;
}

 bool MOG_Project::ClassificationRename(String *currentClassificationName, String* newClassificationName)
{
	// Check for invalid characters
	String *invalidChars = MOG_ControllerSystem::GetInvalidMOGCharacters();
	if (MOG_ControllerSystem::InvalidCharactersCheck(newClassificationName, invalidChars, false))
	{
		String *message = String::Concat(	S"The new classification contains invalid characters.\n",
											S"INVALID CHARACTERS: ", invalidChars, S"\n\n",
											S"The classification was not renamed.");
		MOGPromptResult result = MOG_Prompt::PromptResponse(S"Invalid Characters Found", message, NULL, MOGPromptButtons::OK, MOG_ALERT_LEVEL::ALERT);
		return false;
	}

	// Make sure we have valid arguments
	if (!currentClassificationName || !currentClassificationName->Length ||
		!newClassificationName || !newClassificationName->Length)
	{
		return false;
	}
	if(String::Compare(currentClassificationName, newClassificationName, false) == 0)
	{
		return false;
	}
	if(newClassificationName->ToLower()->StartsWith(String::Concat(currentClassificationName->ToLower(), S"~")))
	{
		return false;
	}
	if(newClassificationName->Trim()->EndsWith(S"~"))
	{
		return false;
	}

	// Make sure there are not any contained locks?
//?	MOG_Command *MOG_ControllerProject::PersistentLock_QueryClassification - Simply adding '*' is flawed because this will match simular classifications that start with the specified classification rather than exact matches
	String *test = String::Concat(currentClassificationName, S"*");
	MOG_Command *pLockQuery = MOG_ControllerProject::PersistentLock_Query(test);
	if (pLockQuery->IsCompleted() &&
		pLockQuery->GetCommand())
	{
		String *message = String::Concat(	S"Cannot rename the classification because it contains locked assets.\n",
											S"\n\n",
											S"Please free these locks and try again.");
		MOGPromptResult result = MOG_Prompt::PromptResponse(S"Asset Rename Failed", message, NULL, MOGPromptButtons::OK, MOG_ALERT_LEVEL::ALERT);
		return false;
	}

	//Build a new MOG_ControllerReinstance
	MOG_ControllerReinstance *reinstanceClassification = new MOG_ControllerReinstance(currentClassificationName, newClassificationName);

	// Construct an informative message
	int classificationCount = reinstanceClassification->GetClassificationsToReinstanceCount();
	int reinstanceCount = reinstanceClassification->GetAssetsToReinstanceCount();
	int dependantCount = reinstanceClassification->GetDependantAssetsCount();
	String *message = String::Concat(	S"This change will effect the following:\n",
										S"          CLASSIFICATIONS: ", Convert::ToString(classificationCount), S"\n",
										S"          CONTAINED ASSETS: ", Convert::ToString(reinstanceCount - dependantCount), S"\n",
										S"          DEPENDANT ASSETS: ", Convert::ToString(dependantCount), S"\n",
										S"\n",
										S"Do you wish to continue?");
	// Prompt the user for confirmation
	if (MOG_Prompt::PromptResponse(	S"Change Classification" , 
									message,
									MOG::PROMPT::MOGPromptButtons::YesNo) == MOG::PROMPT::MOGPromptResult::Yes)
	{
		try
		{
			// Reinstance all the assets
			if (reinstanceClassification->ReinstanceAllAssets())
			{
				return true;
			}
		}
		__finally
		{
			if (reinstanceClassification->GetAssetsToReinstanceCount() > 0)
			{
				// Inform the user this may take a while
				MOG_Prompt::PromptResponse(	S"Asset Processing", 
													String::Concat(	S"This change requires Slave processing.\n",
																	S"The project will not reflect these changes until all slaves have finished processing the generated commands.\n",
																	S"The progress of this task can be monitored in the Connections Tab."));
			}
		}
	}

	return false;
}

bool MOG_Project::AssetRename(String *sourceAssetFullName, String *targetAssetFullName)
{
	// Check for invalid characters
	String *invalidChars = MOG_ControllerSystem::GetInvalidMOGCharacters();
	if (MOG_ControllerSystem::InvalidCharactersCheck(targetAssetFullName, invalidChars, false))
	{
		String *message = String::Concat(	S"The new asset name contains invalid characters.\n",
											S"INVALID CHARACTERS: ", invalidChars, S"\n\n",
											S"The asset was not renamed.");
		MOGPromptResult result = MOG_Prompt::PromptResponse(S"Invalid Characters Found", message, NULL, MOGPromptButtons::OK, MOG_ALERT_LEVEL::ALERT);
		return false;
	}

	if(!sourceAssetFullName || !sourceAssetFullName->Length || 
		!targetAssetFullName || !targetAssetFullName->Length)
	{
		return false;
	}
	if(String::Compare(sourceAssetFullName , targetAssetFullName, false) == 0 )
	{
		return false;
	}

	// Make sure this asset isn't locked?
	//See if we already have it checked out
	MOG_Command *pLockQuery = MOG_ControllerProject::PersistentLock_Query(sourceAssetFullName);
	if (pLockQuery->IsCompleted() &&
		pLockQuery->GetCommand())
	{
		String *message = String::Concat(	S"Cannot rename the asset because it is currently locked by ", pLockQuery->GetCommand()->GetUserName(), S".\n",
											S"ASSET: ", sourceAssetFullName, S"\n\n",
											S"Please free this lock and try again.");
		MOGPromptResult result = MOG_Prompt::PromptResponse(S"Asset Rename Failed", message, NULL, MOGPromptButtons::OK, MOG_ALERT_LEVEL::ALERT);
		return false;
	}

	MOG_Filename *oldAssetFilename = new MOG_Filename(sourceAssetFullName);
	String *oldAssetRevision = MOG_ControllerProject::GetAssetCurrentVersionTimeStamp(oldAssetFilename);
	MOG_Filename *oldBlessedAssetFilename = MOG_ControllerRepository::GetAssetBlessedVersionPath(oldAssetFilename, oldAssetRevision);
	MOG_Filename *newAssetFilename = new MOG_Filename(targetAssetFullName);
	MOG_Filename *newBlessedAssetFilename = MOG_ControllerRepository::GetAssetBlessedVersionPath(newAssetFilename, oldAssetRevision);
	if(newBlessedAssetFilename->GetAssetLabel()->Trim()->Length == 0)
	{
		return false;
	}
	MOG_ControllerReinstance *reinstanceAsset = new MOG_ControllerReinstance(oldBlessedAssetFilename, newBlessedAssetFilename);
	//Reinstance the asset based on the new classifications.
	if (reinstanceAsset->ReinstanceAllAssets())
	{
		return true;
	}

	return false;
}

bool MOG_Project::ModifyBlessedAssetProperties(MOG_Filename *AssetFilename, ArrayList *removeProperties, ArrayList *addProperties)
{
	// Make sure we have a valid asset?
	if (AssetFilename)
	{
		// Make sure the specified name contains a version?
		if (!AssetFilename->GetVersionTimeStamp()->Length)
		{
			// Get the repository's path of the current version for this asset
			AssetFilename = MOG_ControllerProject::GetAssetCurrentBlessedVersionPath(AssetFilename);
		}

		// Make sure this asset is located within the repository?
		if (AssetFilename->IsWithinRepository())
		{
			// Make sure we have something to do?
			if ((removeProperties && removeProperties->Count) ||
				(addProperties && addProperties->Count))
			{
				// Reinstance the asset and apply the property changes
				MOG_ControllerReinstance *reinstanceAsset = new MOG_ControllerReinstance(AssetFilename, removeProperties, addProperties);
				if (reinstanceAsset->ReinstanceAllAssets())
				{
					return true;
				}
			}
		}
	}

	return false;
}

bool MOG_Project::ClassificationRemove(String *classificationFullName)
{
	// Attempt to remove the classification
	if (MOG_DBAssetAPI::RemoveClassification(classificationFullName))
	{
		return true;
	}
	return false;
}


MOG_User *MOG_Project::GetUser(String *userName)
{
	for (int u = 0; u < mUsers->Count; u++)
	{
		MOG_User *user = __try_cast<MOG_User *>(mUsers->Item[u]);

		if (!String::Compare(user->GetUserName(), userName, true) )
		{
			return user;
		}
	}

//?	MOG_Project::GetUser - We really should reload the users anytime a user is requested that we don't have
	//Try to Reload the users
	//LoadUser(userName);

	return NULL;
}

MOG_UserDepartment *MOG_Project::GetUserDepartment(String *userDeptName)
{
	for (int i = 0; i < mUserDepartments->Count; i++)
	{
		MOG_UserDepartment *userDept = __try_cast<MOG_UserDepartment *>(mUserDepartments->Item[i]);

		if (!String::Compare(userDept->mDepartmentName, userDeptName, true) )
		{
			return userDept;
		}
	}
	return NULL;
}

bool MOG_Project::UserUpdate(String *currentUserName, String *newUserName, String *emailAddress, String *blessTarget, String *department)
{
	// Check for invalid characters
	String *invalidChars = MOG_ControllerSystem::GetInvalidMOGCharacters();
	if (MOG_ControllerSystem::InvalidCharactersCheck(newUserName, invalidChars, false))
	{
		String *message = String::Concat(	S"The new user name contains invalid characters.\n",
											S"INVALID CHARACTERS: ", invalidChars, S"\n\n",
											S"The new user was not added.");
		MOGPromptResult result = MOG_Prompt::PromptResponse(S"Invalid Characters Found", message, NULL, MOGPromptButtons::OK, MOG_ALERT_LEVEL::ALERT);
		return false;
	}

	if (MOG_ControllerSystem::InvalidCharactersCheck(department, invalidChars, false))
	{
		String *message = String::Concat(	S"The department contains invalid characters.\n",
											S"INVALID CHARACTERS: ", invalidChars, S"\n\n",
											S"The user was not updated.");
		MOGPromptResult result = MOG_Prompt::PromptResponse(S"Invalid Characters Found", message, NULL, MOGPromptButtons::OK, MOG_ALERT_LEVEL::ALERT);
		return false;
	}

	// Get this user?
	MOG_User *user = GetUser(currentUserName);
	if (user)
	{
		// Update the user's information
		user->SetUserName(newUserName);
		user->SetUserEmailAddress(emailAddress);
		user->SetBlessTarget(blessTarget);
		user->SetUserDepartment(department);

		// Establish connection to database
		if (MOG_ControllerSystem::GetDB()->Connect(mProjectName, MOG_ControllerProject::GetBranchName()))
		{
			// Check if we are renaming the user?
			if (String::Compare(currentUserName, newUserName, true) != 0)
			{
				// Update the user in the database
				if (!MOG_DBProjectAPI::RenameUser(currentUserName, newUserName))
				{
					return false;
				}
			}

			// Make sure the department exists
			UserDepartmentAdd(user->GetUserDepartment());

			// Update the user's info in the database
			if (!MOG_DBProjectAPI::UpdateUser(user->GetUserName(), user->GetUserEmailAddress(), user->GetBlessTarget(), user->GetUserDepartment()))
			{
				return false;
			}

			// Force a RefreshProject
			if (mSuppressProjectRefreshEvent == false)
			{
				MOG_ControllerSystem::RefreshProject(mProjectName);
			}
		}
	}

	return true;
}

bool MOG_Project::UserAdd(MOG_User *user)
{
	// Check for invalid characters
	String *invalidChars = MOG_ControllerSystem::GetInvalidMOGCharacters();
	if (MOG_ControllerSystem::InvalidCharactersCheck(user->GetUserName(), invalidChars, false))
	{
		String *message = String::Concat(	S"The specified user name contains invalid characters.\n",
											S"INVALID CHARACTERS: ", invalidChars, S"\n\n",
											S"The user was not added.");
		MOGPromptResult result = MOG_Prompt::PromptResponse(S"Invalid Characters Found", message, NULL, MOGPromptButtons::OK, MOG_ALERT_LEVEL::ALERT);
		return false;
	}
	if (MOG_ControllerSystem::InvalidCharactersCheck(user->GetUserDepartment(), invalidChars, false))
	{
		String *message = String::Concat(	S"The specified department contains invalid characters.\n",
											S"INVALID CHARACTERS: ", invalidChars, S"\n\n",
											S"The user was not added.");
		MOGPromptResult result = MOG_Prompt::PromptResponse(S"Invalid Characters Found", message, NULL, MOGPromptButtons::OK, MOG_ALERT_LEVEL::ALERT);
		return false;
	}

	// establish connection to database
	if (MOG_ControllerSystem::GetDB()->Connect(mProjectName, MOG_ControllerProject::GetBranchName()))
	{
		// Make sure the department exists
		UserDepartmentAdd(user->GetUserDepartment());

		if (MOG_DBProjectAPI::AddUser(user->GetUserName(), user->GetUserEmailAddress(), user->GetBlessTarget(), user->GetUserDepartment(), S"Admin", MOG_Time::GetVersionTimestamp()))
		{
			// Define user directory
			// Create User tree
			String *userPath = String::Concat(mProjectUsersPath, S"\\", user->GetUserName());

			DosUtils::DirectoryCreate(userPath);
			DosUtils::DirectoryCreate(String::Concat(userPath, S"\\Drafts"));
			DosUtils::DirectoryCreate(String::Concat(userPath, S"\\Inbox"));
			DosUtils::DirectoryCreate(String::Concat(userPath, S"\\Outbox"));
			DosUtils::DirectoryCreate(String::Concat(userPath, S"\\Tools"));
			DosUtils::DirectoryCreate(String::Concat(userPath, S"\\Trash"));

			// Add this user to our privileges file
			MOG_Privileges *privileges = MOG_ControllerProject::GetPrivileges();
			if( privileges )
			{
				privileges->AddUser( user->GetUserName(), user->GetUserDepartment() );
			}

			// Append to users
			mUsers->Add(user);

			// Force a RefreshProject
			if (mSuppressProjectRefreshEvent == false)
			{
				MOG_ControllerSystem::RefreshProject(mProjectName);
			}

			return true;
		}
	}
	
	return false;
}


bool MOG_Project::UserRemove(String *userName)
{
	if (String::Compare(userName, S"Admin", true) == 0)
	{
		MOG_Prompt::PromptMessage("Unable to remove Admin", "The Admin account cannot be removed from the project", Environment::StackTrace, MOG_ALERT_LEVEL::ALERT);
		return false;
	}

	if (MOG_ControllerSystem::GetDB()->Connect(mProjectName, MOG_ControllerProject::GetBranchName()))
	{
		if (MOG_DBProjectAPI::RemoveUser(userName, MOG_ControllerProject::GetUserName_DefaultAdmin()))
		{
			// Find the user
			for (int u = 0; u < mUsers->Count; u++)
			{
				MOG_User *user = __try_cast<MOG_User *>(mUsers->Item[u]);

				if (!String::Compare(user->GetUserName(), userName, true))
				{
					// Remove the user directories
					DosUtils::DirectoryDeleteFast(String::Concat(mProjectUsersPath, S"\\", userName));

//? MOG_Project::UserRemove - Scan Inbox for any tasks assigned to this user
//? MOG_Project::UserRemove - Scan all tasks looking this user in the completion flow
//? MOG_Project::UserRemove - Scan Inbox for any assets assigned to this user
//? MOG_Project::UserRemove - Scan Outbox for any links that list this user as the creator
//? MOG_Project::UserRemove - Scan all users looking for this user being their bless target
//? MOG_Project::UserRemove - Remove links contained in the Outbox from their asset's Properties.Info file

					// If we have user privileges, remove this user from that file as well
					MOG_Privileges *privileges = MOG_ControllerProject::GetPrivileges();
					if( privileges )
					{
						privileges->RemoveUser( user->GetUserName() );
					}

					// Remove the user node
					mUsers->RemoveAt(u);

					// Force a RefreshProject
					if (mSuppressProjectRefreshEvent == false)
					{
						MOG_ControllerSystem::RefreshProject(mProjectName);
					}

					break;
				}
			}

			return true;
		}
	}

	return false;
}



ArrayList *MOG_Project::GetDepartmentUsers(String *department)
{
	ArrayList *users = new ArrayList();
	for (int x = 0; x < mUsers->Count; x++)
	{
		MOG_User *user = __try_cast<MOG_User*>(mUsers->Item[x]);

		if (!String::Compare(user->GetUserDepartment(), department, true))
		{
			users->Add(user);
		}
	}

	return users;
}


bool MOG_Project::PlatformAdd(MOG_Platform *platform)
{
	// Check for invalid characters
	String *invalidChars = MOG_ControllerSystem::GetInvalidMOGCharacters();
	if (MOG_ControllerSystem::InvalidCharactersCheck(platform->mPlatformName, invalidChars, false))
	{
		String *message = String::Concat(	S"The specified platform name contains invalid characters.\n",
											S"INVALID CHARACTERS: ", invalidChars, S"\n\n",
											S"The platform was not added.");
		MOGPromptResult result = MOG_Prompt::PromptResponse(S"Invalid Characters Found", message, NULL, MOGPromptButtons::OK, MOG_ALERT_LEVEL::ALERT);
		return false;
	}

	// Make sure the PlatformName is valid?
	if (platform->mPlatformName->Length <= 10)
	{
		if (MOG_ControllerSystem::GetDB()->Connect(mProjectName, MOG_ControllerProject::GetBranchName()))
		{
			// Only bother to add this platform if it doesn't already exist
			if (!MOG_ControllerProject::IsValidPlatform(platform->mPlatformName))
			{
				// Add platform to the database truncating PlatformName for the PlatformKey
				if (MOG_DBProjectAPI::AddPlatformToBranch(platform->mPlatformName, MOG_ControllerProject::GetBranchName()))
				{
					// Add the platform
					mPlatforms->Add(platform);

					ProgressDialog* progress = new ProgressDialog(S"Populating Platform", S"Please wait while the system populates the new platform.", new DoWorkEventHandler(this, &MOG_Project::PopulatePlatformSyncFiles_Worker), platform->mPlatformName, true);
					if (progress->ShowDialog() == DialogResult::OK)
					{
						// Force a RefreshProject
						if (mSuppressProjectRefreshEvent == false)
						{
							MOG_ControllerSystem::RefreshProject(mProjectName);
						}
					}
				}

				return true;
			}
		}
	}
	else
	{
		// Inform the user it is too long
		String *message = String::Concat(	S"Platform names can not exceed 10 characters in length.\n\n",
											S"Platform was not added to the project.");
		MOG_Prompt::PromptMessage(S"Add Platform Failed", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
	}

	return false;
}


void MOG_Project::PopulatePlatformSyncFiles_Worker(Object* sender, DoWorkEventArgs* e)
{
	BackgroundWorker* worker = dynamic_cast<BackgroundWorker*>(sender);
	String* platformName = dynamic_cast<String*>(e->Argument);

	// Make sure all generic 'All' assets have been stamped in the SyncTargetFileMap table for this platform
	ArrayList *assets = MOG_DBAssetAPI::GetAllArchivedAssetsByPlatform(S"All");
	for (int i = 0; i < assets->Count; i++)
	{
		MOG_Filename* assetFilename = dynamic_cast<MOG_Filename*>(assets->Item[i]);

		int percent = i * 100 / assets->Count;
		worker->ReportProgress(percent, assetFilename->GetAssetFullName());

		// Check if this asset revision needs to be added for this platform?
		if (!MOG_DBAssetAPI::DoesSyncTargetFileLinkExistForAsset(assetFilename, platformName))
		{
			// Get the fully quantified path to this asset revision in the repository
			MOG_Filename* blessedAssetFilename = MOG_ControllerRepository::GetAssetBlessedVersionPath(assetFilename, assetFilename->GetVersionTimeStamp());
			// Get the propereties of this blessed asset revision
			MOG_Properties* assetProperties = new MOG_Properties(blessedAssetFilename);

			// Make sure the files of this asset revision have been added for this platform
			if (!MOG_DBAssetAPI::AddAssetRelatedFilesForPlatform(blessedAssetFilename, assetProperties, platformName))
			{
//				bFailed = true;
			}
		}

		// Check for user cancel?
		if (worker->CancellationPending)
		{
			break;
		}
	}
}

bool MOG_Project::PlatformRemove(String *platform)
{
	// Assume the platform has already been removed until we verify it actually exists in the project
	bool bPlatformRemoved = true;

	if (MOG_ControllerSystem::GetDB()->Connect(mProjectName, MOG_ControllerProject::GetBranchName()))
	{
		// Platform does exist in the project...indicate it hasn't yet been removed
		bPlatformRemoved = false;
		bool bRemovePlatform = true;

		// Check if the project contained any platform specific assets?
		bool bRemoveAssets = false;
		ArrayList *assets = MOG_DBAssetAPI::GetAllAssetsByPlatform(platform);
		if (assets->Count)
		{
			// Prompt the user about removing these assets
			String* message = String::Concat(S"MOG has detected this project contains platform-specific assets.\n\n",
											 S"It is recommended these assets be removed from the project anytime their associated platform is removed.\n\n", 
											 S"PLATFORM: ", platform);
			MOGPromptResult result = MOG_Prompt::PromptResponse("Remove all platform-specific assets from the project?", message, MOGPromptButtons::YesNoCancel);
			switch (result)
			{
				case MOGPromptResult::Yes:
					bRemovePlatform = true;
					bRemoveAssets = true;
					break;
				case MOGPromptResult::No:
					bRemovePlatform = true;
					bRemoveAssets = false;
					break;
				case MOGPromptResult::Cancel:
					bRemovePlatform = false;
					bRemoveAssets = false;
					break;
			}
		}

		// Check if we should proceed to remove the platform?
		if (bRemovePlatform)
		{
			// Remove the actual platform from the project's branch
			if (MOG_DBProjectAPI::RemovePlatformFromBranch(platform, MOG_ControllerProject::GetBranchName()))
			{
				// Indicate we have removed the platform from the project
				bPlatformRemoved = true;

				// Find the platform
				for (int p = 0; p < mPlatforms->Count; p++)
				{
					if (!String::Compare(__try_cast<MOG_Platform *>(mPlatforms->Item[p])->mPlatformName, platform, true))
					{
						// Remove the node
						mPlatforms->RemoveAt(p);
					}
				}

				// Check if we should also remove the platform-specific assets from the project?
				if (bRemoveAssets)
				{
					ProgressDialog* progress = new ProgressDialog(S"Removing platform-specific assets", S"Please wait while the system removes the platform-specific assets from the project.", new DoWorkEventHandler(this, &MOG_Project::RemovePlatformSpecificAssets_Worker), assets, true);
					progress->ShowDialog();
				}

				// Force a RefreshProject
				if (mSuppressProjectRefreshEvent == false)
				{
					MOG_ControllerSystem::RefreshProject(mProjectName);
				}
			}
		}
	}

	return bPlatformRemoved;
}

void MOG_Project::RemovePlatformSpecificAssets_Worker(Object* sender, DoWorkEventArgs* e)
{
	BackgroundWorker* worker = dynamic_cast<BackgroundWorker*>(sender);
	ArrayList* assets = dynamic_cast<ArrayList*>(e->Argument);

	// Create a nice default jobLabel
	String *jobLabel = String::Concat(S"RemovePlatform.", MOG_ControllerSystem::GetComputerName(), S".", MOG_Time::GetVersionTimestamp());

	// Remove all the platform-specific assets from the project
	for (int i = 0; i < assets->Count; i++)
	{
		MOG_Filename* assetFilename = dynamic_cast<MOG_Filename*>(assets->Item[i]);

		int percent = i * 100 / assets->Count;
		worker->ReportProgress(percent, assetFilename->GetAssetFullName());

		// Check if this asset revision needs to be added for this platform?
		MOG_ControllerProject::RemoveAssetFromProject(assetFilename, "Platform removed from project.", jobLabel, true);

		// Check for user cancel?
		if (worker->CancellationPending)
		{
			break;
		}
	}

	// Start this job now that we have schedule all the assets for removal
	MOG_ControllerProject::StartJob(jobLabel);
}

bool MOG_Project::UserDepartmentAdd(String* department)
{
	return UserDepartmentAdd(new MOG_UserDepartment(department));
}

bool MOG_Project::UserDepartmentAdd(MOG_UserDepartment *department)
{
	if (MOG_ControllerSystem::GetDB()->Connect(mProjectName, MOG_ControllerProject::GetBranchName()))
	{
		if (MOG_DBProjectAPI::AddDepartment(department->mDepartmentName))
		{
			// First make sure that we don't already have one
			for (int c = 0;  c < mUserDepartments->Count; c++)
			{
				if (!String::Compare(__try_cast<MOG_UserDepartment *>(mUserDepartments->Item[c])->mDepartmentName, department->mDepartmentName, true))
				{
					return false;
				}
			}

			mUserDepartments->Add(department);

			// Force a RefreshProject
			if (mSuppressProjectRefreshEvent == false)
			{
				MOG_ControllerSystem::RefreshProject(mProjectName);
			}

			return true;
		}
	}

	return false;
}


bool MOG_Project::UserDepartmentRemove(String *userDepartment)
{
	if (MOG_ControllerSystem::GetDB()->Connect(mProjectName, MOG_ControllerProject::GetBranchName()))
	{
		if (MOG_DBProjectAPI::RemoveDepartment(userDepartment))
		{
			// Find the department
			for (int c = 0; c < mUserDepartments->Count; c++)
			{
				MOG_UserDepartment *department = __try_cast<MOG_UserDepartment *>(mUserDepartments->Item[c]);

				if (!String::Compare(department->mDepartmentName, userDepartment, true))
				{
//? MOG_Project::CategoryRemove - Scan all users and reassign them to a valid category

					// Force a RefreshProject
					if (mSuppressProjectRefreshEvent == false)
					{
						MOG_ControllerSystem::RefreshProject(mProjectName);
					}

					// Remove the node
					mUserDepartments->RemoveAt(c);
				}
			}
		}

		return true;
	}

	return false;
}

void MOG_Project::SetProjectConnectionString(String * connectionString)
{
	if(connectionString->Length > 0)
	{
		mProjectConnectionString = connectionString;
	}
	else
	{
		//Put in code to build 
	}
}


