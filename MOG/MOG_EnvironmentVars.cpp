//--------------------------------------------------------------------------------
//	MOG_EnvironmentVars.cpp
//	
//	
//--------------------------------------------------------------------------------
#include "StdAfx.h"

#include "MOG_ControllerProject.h"
#include "MOG_ControllerSystem.h"
#include "MOG_DosUtils.h"

#include "MOG_EnvironmentVars.h"



String *MOG_EnvironmentVars::CreateToolsPathEnvironmentVariableString(String *relativeToolsPath)
{
	return CreateToolsPathEnvironmentVariableString(relativeToolsPath, NULL);
}

String *MOG_EnvironmentVars::CreateToolsPathEnvironmentVariableString(String *relativeToolsPath, MOG_Filename *assetDirectory)
{
	// Build the path environment variable
	String *toolsDirs = "";

	// Check if there is a relative dir on this tool?
	if (relativeToolsPath->Length)
	{
		// Add the relative dir of 'Tools' for the System, Project, User and Asset Directory scope first
		if (assetDirectory)
		{
			toolsDirs = String::Concat(toolsDirs, S";", assetDirectory->GetEncodedFilename(), S"\\Tools\\", relativeToolsPath);
		}
		if (MOG_ControllerProject::IsUser())
		{
			toolsDirs = String::Concat(toolsDirs, S";", MOG_ControllerProject::GetUser()->GetUserPath(), S"\\Tools\\", relativeToolsPath);
		}
		if (MOG_ControllerProject::IsProject())
		{
			toolsDirs = String::Concat(toolsDirs, S";", MOG_ControllerProject::GetProject()->GetProjectToolsPath(), S"\\", relativeToolsPath);
		}
		toolsDirs = String::Concat(toolsDirs, S";", MOG_ControllerSystem::GetSystem()->GetSystemToolsPath(), S"\\", relativeToolsPath);
	}

	// Add the root of 'Tools' for the System, Project, User and Asset Directory
	if (assetDirectory)
	{
		toolsDirs = String::Concat(toolsDirs, S";", assetDirectory->GetEncodedFilename(), S"\\Tools");
	}
	if (MOG_ControllerProject::IsUser())
	{
		toolsDirs = String::Concat(toolsDirs, S";", MOG_ControllerProject::GetUser()->GetUserPath(), S"\\Tools");
	}
	if (MOG_ControllerProject::IsProject())
	{
		toolsDirs = String::Concat(toolsDirs, S";", MOG_ControllerProject::GetProject()->GetProjectToolsPath());
	}
	toolsDirs = String::Concat(toolsDirs, S";", MOG_ControllerSystem::GetSystem()->GetSystemToolsPath());

	// Build the path environment variable including the orininal path of this systemthis slave...
	String *newPath = String::Concat(S"Path=", toolsDirs, S";", DosUtils::GetSystemEnvironmentVariable("Path"));

	return newPath;
}


