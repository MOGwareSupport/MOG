//--------------------------------------------------------------------------------
//	MOG_ControllerDemoProject.cpp
//	
//	
//--------------------------------------------------------------------------------

#include "StdAfx.h"

#include "MOG_Define.h"
#include "MOG_DosUtils.h"
#include "MOG_ControllerSystem.h"
#include "MOG_DBAPI.h"

#include "MOG_ControllerDemoProject.h"
#include "MOG_ControllerRepository.h"



ArrayList *MOG_ControllerDemoProject::GetAllDemoProjectNames()
{
	ArrayList *projectNames = new ArrayList();

	// Get the list of contained directories
	DirectoryInfo *dirs[] = DosUtils::DirectoryGetList(MOG_ControllerSystem::GetSystem()->GetSystemDemoProjectsPath(), S"*.*");
	if (dirs)
	{
		// Add each directory's name
		for (int d = 0; d < dirs->Count; d++)
		{
			projectNames->Add(dirs[d]->Name);
		}
	}

	return projectNames;
}


bool MOG_ControllerDemoProject::ImportDemoProject(String *projectName, BackgroundWorker* worker)
{
	String *projectRepositoryLocation = String::Concat(MOG_ControllerSystem::GetSystemProjectsPath(), S"\\", projectName);
	String *demoProjectLocation = String::Concat(MOG_ControllerSystem::GetSystemDemoProjectsPath(), S"\\", projectName);
	bool bFailed = false;

	if (worker != NULL)
	{
		worker->ReportProgress(0, new PushProgressbarCommand(ProgressBarStyle::Marquee));
	}

	// Make sure this DemoProject exists
	if (DosUtils::DirectoryExistFast(demoProjectLocation))
	{
		// Proceed to create our new project
		if (MOG_ControllerSystem::GetSystem()->ProjectCreate(projectName))
		{
			// Erase all the tables so we know there is no stale data
			MOG_ControllerSystem::GetDB()->DeleteProjectTables(projectName);
			// Create new empty tables so we will have a blank slate
			MOG_ControllerSystem::GetDB()->VerifyTables(projectName);

			// Import the database tables
			if (MOG_Database::ImportProjectTables(projectName, demoProjectLocation))
			{
				// Proceed to copy the DemoProject into the repository
				if (MOG_ControllerSystem::DirectoryCopyEx(demoProjectLocation, projectRepositoryLocation, worker))
				{
				}
				else
				{
					// Inform the user we were unable to locate the decompressing tool
					String *message = String::Concat(	S"Unable to copy the specified DemoProject into the repository.\n\n",
														S"Please check your network permissions to the repository.");
					MOG_Prompt::PromptMessage(S"Import DemoProject Failed", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
					bFailed = true;
				}
			}
			else
			{
				// Inform the user we were unable to locate the decompressing tool
				String *message = String::Concat(	S"Failed to import the DemoProject's database tables.\n\n",
													S"Please check your database permissions.");
				MOG_Prompt::PromptMessage(S"Import DemoProject Failed", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
				bFailed = true;
			}
		}
		else
		{
			bFailed = true;
		}
	}
	else
	{
		// Inform the user we were unable to locate the decompressing tool
		String *message = String::Concat(	S"'", demoProjectLocation, S"' does not exist in the DemoProject tray.\n\n",
											S"DemoProjects must be added to the DemoProject tray before they can be imported.");
		MOG_Prompt::PromptMessage(S"Import DemoProject Failed", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
		bFailed = true;
	}

	if (worker != NULL)
	{
		worker->ReportProgress(0, new PopProgressbarCommand());
	}

	if (!bFailed)
	{
		return true;
	}
	return false;
}


bool MOG_ControllerDemoProject::DemoizeProject(String *projectName, BackgroundWorker* worker)
{
	bool bFailed = false;

	if (worker != NULL)
	{
		worker->ReportProgress(0, new PushProgressbarCommand(ProgressBarStyle::Marquee));
	}

	// Make sure the specified project exists
	String *projectPath = String::Concat(MOG_ControllerSystem::GetSystem()->GetSystemProjectsPath(), S"\\", projectName);
	if (DosUtils::DirectoryExistFast(projectPath))
	{
		// Check if the DemoProject already exists?
		String *demoProjectPath = String::Concat(MOG_ControllerSystem::GetSystem()->GetSystemDemoProjectsPath(), S"\\", projectName);
		if (DosUtils::DirectoryExistFast(demoProjectPath))
		{
			// Ask user to overwrite already existing DemoProject?
			String *message =	String::Concat(	S"There is already a DemoProject named '", projectName, S"'\n\n",
												S"Do you want to overwrite this DemoProject?");
			if (MOG_Prompt::PromptResponse(S"DemoProject Already Exists", message, MOGPromptButtons::YesNo) == MOGPromptResult::Yes)
			{
				// Remove the DemoProject
				RemoveDemoProject(projectName, worker);
			}
		}

		// Make sure there isn't a DemoProject in our way?
		if (!DosUtils::DirectoryExistFast(demoProjectPath))
		{
			// Create the database directory
			if (DosUtils::DirectoryCreate(demoProjectPath, true))
			{
				// Dump the database table out into the new DemoProject directory
				if (MOG_Database::ExportProjectTables(projectName, projectPath))
				{
					// Delete the contents of the archive directory because this is filled all sorts of junk we shouldn't copy
					DosUtils::DirectoryCreate(MOG_ControllerRepository::GetArchivePath(), true);

					// Proceed to copy the repository data
					if (MOG_ControllerSystem::DirectoryCopyEx(projectPath, demoProjectPath, worker))
					{
					}
					else
					{
						// Inform the user we were unable to locate the decompressing tool
						String *message = String::Concat(	S"Unable to copy Project files .\n\n",
															S"Please check your network permissions to the repository.");
						MOG_Prompt::PromptMessage(S"Create DemoProject Failed", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
						bFailed = true;
					}
				}
				else
				{
					// Inform the user we were unable to locate the decompressing tool
					String *message = String::Concat(	S"Unable to dump project's database tables.\n\n",
														S"Please check your SQL connection string.");
					MOG_Prompt::PromptMessage(S"Create DemoProject Failed", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
					bFailed = true;
				}
			}
			else
			{
				// Inform the user we were unable to locate the decompressing tool
				String *message = String::Concat(	S"Unable to create DemoProject directory.\n\n",
													S"Please check your network permissions to the repository.");
				MOG_Prompt::PromptMessage(S"Create DemoProject Failed", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
				bFailed = true;
			}
		}
	}

	if (worker != NULL)
	{
		worker->ReportProgress(0, new PopProgressbarCommand());
	}

	if (!bFailed)
	{
		return true;
	}
	return false;
}


bool MOG_ControllerDemoProject::AddDemoProject(String *importItem, BackgroundWorker* worker)
{
	String *filename = DosUtils::PathGetFileName(importItem);
	String *projectName = DosUtils::PathGetFileNameWithoutExtension(importItem);
	String *demoProjectsPath = MOG_ControllerSystem::GetSystemDemoProjectsPath();
	bool bFailed = false;

	if (worker != NULL)
	{
		worker->ReportProgress(0, new PushProgressbarCommand(ProgressBarStyle::Marquee));
	}

	// Check if this is a valid DemoProject?
	if (IsValidDemoProjectDirectory(importItem))
	{
		// Make sure there isn't a DemoProject in our way?
		if (ResolveRemovalOfAlreadyExistingDemoProject(projectName, worker))
		{
			String *targetFolder = String::Concat(demoProjectsPath, S"\\", projectName);

			// Proceed to copy the DemoProject directory over to the repository
			if (!MOG_ControllerSystem::DirectoryCopyEx(importItem, targetFolder, worker))
			{
				// Inform the user we were unable to locate the decompressing tool
				String *message = String::Concat(	S"Unable to copy the specified DemoProject into the repository.\n\n",
													S"Please check your network permissions to the repository.");
				MOG_Prompt::PromptMessage(S"Add DemoProject Failed", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
				bFailed = true;
			}
		}
	}
	else if (IsValidDemoProjectCompressedFile(importItem))
	{
		// Make sure there isn't a DemoProject in our way?
		if (ResolveRemovalOfAlreadyExistingDemoProject(projectName, worker))
		{
			// Locate the required unzip utility
			String *tool = MOG_ControllerSystem::LocateTool("paext.exe");
			if (tool->Length)
			{
				String *toolDir = DosUtils::PathGetDirectoryPath(tool);
				String *args = String::Concat(S"-d -p\"", demoProjectsPath, S"\" \"", importItem, S"\"");
				ArrayList *environment = new ArrayList();
				String *output = "";

				// Execute the tool
				if (DosUtils::SpawnDosCommand(toolDir, tool, args, environment, &output, false) != 0)
				{
					bFailed = true;
				}
			}
			else
			{
				// Inform the user we were unable to locate the decompressing tool
				String *message = String::Concat(	S"Unable to locate the required decompressing tool named 'paext.exe'.\n\n",
													S"Please check your system tools path.");
				MOG_Prompt::PromptMessage(S"Add DemoProject Failed", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
				bFailed = true;
			}
		}
	}
	else
	{
		// Get the directory Contents
		DirectoryInfo *dirInfo = DosUtils::DirectoryGetInfo(importItem);

		// Check if this is a directory?
		if (dirInfo->Exists)
		{
			// Attempt to process each contained file
			for (int i = 0; i < dirInfo->GetFiles()->Count; i++)
			{
				String *newImportItem = dirInfo->GetFiles()[i]->FullName;

				if (IsValidDemoProjectCompressedFile(newImportItem))
				{
					if (!AddDemoProject(newImportItem, worker))
					{
						bFailed = true;
					}
				}
			}

			// Attempt to process each contained directory
			for (int i = 0; i < dirInfo->GetDirectories()->Count; i++)
			{
				String *newImportItem = dirInfo->GetDirectories()[i]->FullName;

				if (AddDemoProject(newImportItem, worker))
				{
					bFailed = true;
				}
			}
		}
	}

	if (worker != NULL)
	{
		worker->ReportProgress(0, new PopProgressbarCommand());
	}

	if (!bFailed)
	{
		return true;
	}
	return false;
}


bool MOG_ControllerDemoProject::ResolveRemovalOfAlreadyExistingDemoProject(String *projectName, BackgroundWorker* worker)
{
	String *targetDemoProjectPath = String::Concat(MOG_ControllerSystem::GetSystemDemoProjectsPath(), S"\\", projectName);

	// Check if there is already a DemoProject with this name?
	if (DosUtils::DirectoryExistFast(targetDemoProjectPath))
	{
		// Ask user to overwrite already existing DemoProject?
		String *message =	String::Concat(	S"There is already a DemoProject named '", projectName, S"'\n\n",
											S"Do you want to overwrite this DemoProject?");
		if (MOG_Prompt::PromptResponse(S"DemoProject Already Exists", message, MOGPromptButtons::YesNo) == MOGPromptResult::Yes)
		{
			// Remove the DemoProject
			if (RemoveDemoProject(projectName, worker))
			{
				return true;
			}
		}

		return false;
	}

	return true;
}


bool MOG_ControllerDemoProject::ContainsDemoProject(String *demoProjectPath)
{
	// Check if this is a DemoProject?
	if (IsValidDemoProject(demoProjectPath))
	{
		return true;
	}

	// Get the directory's info
	DirectoryInfo *dirInfo = DosUtils::DirectoryGetInfo(demoProjectPath);
	if (dirInfo)
	{
		// Make sure it exists?
		if (dirInfo->Exists)
		{
			// Traverse each contained directory
			for (int d = 0; d < dirInfo->GetDirectories()->Count; d++)
			{
				if (ContainsDemoProject(dirInfo->GetDirectories()[d]->FullName))
				{
					return true;
				}
			}
		}
	}

	return false;
}


bool MOG_ControllerDemoProject::IsValidDemoProject(String *directory)
{
	// Check if this is a one of the valid types?
	if (IsValidDemoProjectDirectory(directory) ||
		IsValidDemoProjectCompressedFile(directory))
	{
		return true;
	}

	return false;
}


bool MOG_ControllerDemoProject::IsValidDemoProjectDirectory(String *directory)
{
	// Get the directory information for the specified directory
	DirectoryInfo *dirInfo = DosUtils::DirectoryGetInfo(directory);

	// Make sure it actually exists?
	if (dirInfo->Exists)
	{
		int weight = 0;

		// Check the directory for the required contents
		for (int d = 0; d < dirInfo->GetDirectories()->Count; d++)
		{
			if (String::Compare(dirInfo->GetDirectories()[d]->Name, S"Database", true) == 0)
			{
				weight++;
			}
			if (String::Compare(dirInfo->GetDirectories()[d]->Name, S"Tools", true) == 0)
			{
				weight++;
			}
			if (String::Compare(dirInfo->GetDirectories()[d]->Name, S"Assets", true) == 0)
			{
				weight++;
			}
			if (String::Compare(dirInfo->GetDirectories()[d]->Name, S"Users", true) == 0)
			{
				weight++;
			}
		}

		// Check if we detected enough within this directory to conjsider it a valid DemoProject?
		if (weight == 4)
		{
			return true;
		}
	}

	return false;
}


bool MOG_ControllerDemoProject::IsValidDemoProjectCompressedFile(String *filename)
{
	String *extension = DosUtils::PathGetExtension(filename);

	// Check the extension
	if (String::Compare(extension, "zip", true) == 0 ||
		String::Compare(extension, "rar", true) == 0)
	{						
		// Make sure it actually exists?
		if (DosUtils::FileExistFast(filename))
		{
			// Indicate we have a DemoProjectCompressedFile
			return true;
		}
	}

	return false;
}

bool MOG_ControllerDemoProject::RemoveDemoProject(String *projectName, BackgroundWorker* worker)
{
	String *target = String::Concat(MOG_ControllerSystem::GetSystem()->GetSystemDemoProjectsPath(), S"\\", projectName);
	bool bRemoved = false;

	if (worker != NULL)
	{
		worker->ReportProgress(0, new PushProgressbarCommand(ProgressBarStyle::Marquee));
	}

	// Make sure the specified project exists
	if (DosUtils::DirectoryExistFast(target))
	{
		// Delete the demo project's directory
		if (DosUtils::DirectoryDeleteFast(target))
		{
			bRemoved = true;
		}
		else
		{
			String *message = String::Concat(	S"Unable to delete the DemoProject named '", projectName, S"'.\n\n",
												S"Please check your networks permissions or opened file handles.");
			MOG_Prompt::PromptMessage(S"Remove DemoProject Failed", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
		}
	}
	else
	{
		String *message = String::Concat(	S"Unable to find a DemoProject named '", projectName, S"'.\n\n",
											S"Please make sure you specified the correct DemoProject.");
		MOG_Prompt::PromptMessage(S"Remove DemoProject Failed", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
	}

	if (worker != NULL)
	{
		worker->ReportProgress(0, new PopProgressbarCommand());
	}

	return bRemoved;
}


