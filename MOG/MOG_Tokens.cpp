//--------------------------------------------------------------------------------
//	MOG_Tokens.cpp
//	
//	
//--------------------------------------------------------------------------------
#include "StdAfx.h"

#include "MOG_System.h"
#include "MOG_ControllerProject.h"
#include "MOG_ControllerPackage.h"
#include "MOG_ControllerSystem.h"

#include "MOG_Tokens.h"

using namespace System::Text::RegularExpressions;

bool MOG_Tokens::TokenExist(String* format)
{
	if (format && Regex::IsMatch(format, S"\\{.*\\}"))
	{
		return true;
	}

	return false;
}

String* MOG_Tokens::GetOSTokenSeeds()
{
	String* seeds = S"";

	seeds = AddNextTokenSeed(seeds, TOKEN_Windows, Environment::SystemDirectory, false);

	return seeds;
}

String* MOG_Tokens::GetSystemTokenSeeds()
{
	String* seeds = S"";

	seeds = AddNextTokenSeed(seeds, TOKEN_SystemRepositoryPath, MOG_ControllerSystem::GetSystemRepositoryPath(), false);
	seeds = AddNextTokenSeed(seeds, TOKEN_SystemProjectsPath, MOG_ControllerSystem::GetSystemProjectsPath(), false);
	
	return seeds;
}


//
String* MOG_Tokens::GetProjectTokenSeeds(MOG_Project* pProjectInfo)
{
	String* seeds = S"";

	// Make sure we have a valid pProjectInfo?
	if (pProjectInfo)
	{
		// Append all the Project specific token seeds
		seeds = AddNextTokenSeed(seeds, TOKEN_Project, pProjectInfo->GetProjectName(), false);
		seeds = AddNextTokenSeed(seeds, TOKEN_ProjectName, pProjectInfo->GetProjectName(), false);
		seeds = AddNextTokenSeed(seeds, TOKEN_ProjectPath, pProjectInfo->GetProjectPath(), false);
	}

	if (MOG_ControllerProject::IsProject())
	{
		seeds = AddNextTokenSeed(seeds, TOKEN_LoginUserName, MOG_ControllerProject::GetUserName(), false);
		seeds = AddNextTokenSeed(seeds, TOKEN_ProjectBranch, MOG_ControllerProject::GetBranchName(), false);
	}

	return seeds;
}


//
String* MOG_Tokens::GetWorkspaceTokenSeeds(MOG_ControllerSyncData* workspace)
{
	String* seeds = S"";

	if (workspace)
	{
		return MOG_Tokens::GetWorkspaceTokenSeeds(workspace->GetSyncDirectory(), workspace->GetPlatformName());
	}

	return seeds;
}


String* MOG_Tokens::GetWorkspaceTokenSeeds(String* workspaceDirectory, String* platformName)
{
	String* seeds = S"";

	seeds = AddNextTokenSeed(seeds, TOKEN_ProjectPlatform, platformName, false);
	seeds = AddNextTokenSeed(seeds, TOKEN_WorkspaceDirectory, workspaceDirectory, false);

	return seeds;
}


String* MOG_Tokens::GetRipperTokenSeeds(String* sourcePath, String* sourceFilePattern, String* destinationPath)
{
	String* seeds = S"";

	seeds = AddNextTokenSeed(seeds, TOKEN_SourcePath, sourcePath, false);
	seeds = AddNextTokenSeed(seeds, TOKEN_SourceFilePattern, sourceFilePattern, false);
	seeds = AddNextTokenSeed(seeds, TOKEN_DestinationPath, destinationPath, false);

	return seeds;
}


String* MOG_Tokens::GetPackageTokenSeeds(MOG_PackageMerge_PackageFileInfo* packageFileInfo)
{
	String* seeds = S"";

	// Check if there is anything specified?
	if (packageFileInfo)
	{
		// Call our other function to populate the seeds
		seeds = GetPackageTokenSeeds(packageFileInfo->mPackageAssetFilename->GetAssetFullName(), packageFileInfo->mPackageAssetFilename->GetAssetPlatform(), S"", S"", packageFileInfo->mRelativePackageFile);
		seeds = AddNextTokenSeed(seeds, TOKEN_PackageWorkspaceDirectory, packageFileInfo->mPackageWorkspaceDirectory, false);
		seeds = AddNextTokenSeed(seeds, TOKEN_PackageDataDirectory, packageFileInfo->mPackageDataDirectory, false);
	}

	return seeds;
}


String* MOG_Tokens::GetPackageTokenSeeds(MOG_PackageMerge_PackageFileInfo* packageFileInfo, String* assignedPackageStatement)
{
	String* seeds = S"";
	String* packageGroup = S"";
	String* packageObject = S"";

	// Check if there is anything specified?
	if (assignedPackageStatement && assignedPackageStatement->Length)
	{
		packageGroup = MOG_ControllerPackage::GetPackageGroups(assignedPackageStatement);
		packageObject = MOG_ControllerPackage::StripPackageObjectIdentifiers(MOG_ControllerPackage::GetPackageObjects(assignedPackageStatement));
	}

	// Check if there is anything specified?
	if (packageFileInfo)
	{
		// Call our other function to populate the seeds
		seeds = GetPackageTokenSeeds(packageFileInfo->mPackageAssetFilename->GetAssetFullName(), packageFileInfo->mPackageAssetFilename->GetAssetPlatform(), packageGroup, packageObject, packageFileInfo->mRelativePackageFile);
		seeds = AddNextTokenSeed(seeds, TOKEN_PackageWorkspaceDirectory, packageFileInfo->mPackageWorkspaceDirectory, false);
		seeds = AddNextTokenSeed(seeds, TOKEN_PackageDataDirectory, packageFileInfo->mPackageDataDirectory, false);
	}

	return seeds;
}


String* MOG_Tokens::GetPackageTokenSeeds(String* assignedPackageStatement)
{
	String* seeds = S"";

	// Check if there is anything specified?
	if (assignedPackageStatement && assignedPackageStatement->Length)
	{
		// Break up the tokens from the specified assignedPackageStatement
		String* packageName = MOG_ControllerPackage::GetPackageName(assignedPackageStatement);
		String* packagePlatform = MOG_ControllerPackage::GetPackagePlatform(assignedPackageStatement);
		String* packageGroup = MOG_ControllerPackage::GetPackageGroups(assignedPackageStatement);
		String* packageObject = MOG_ControllerPackage::StripPackageObjectIdentifiers(MOG_ControllerPackage::GetPackageObjects(assignedPackageStatement));
		String* packageFile = MOG_ControllerPackage::MapPackageAssetNameToFile(new MOG_Filename(packageName));

		// Call our other function to populate the seeds
		seeds = GetPackageTokenSeeds(packageName, packagePlatform, packageGroup, packageObject, packageFile);
	}

	return seeds;
}


String* MOG_Tokens::GetPackageTokenSeeds(String* packageName, String* packagePlatform, String* packageGroup, String* packageObject, String* packageFile)
{
	String* seeds = S"";
	MOG_Filename* packageFilename = new MOG_Filename(packageName);

	seeds = AddNextTokenSeed(seeds, TOKEN_PackageName, packageName, false);
	seeds = AddNextTokenSeed(seeds, TOKEN_PackageClassification, packageFilename->GetAssetClassification(), false);
	seeds = AddNextTokenSeed(seeds, TOKEN_PackageLabel, packageFilename->GetAssetLabel(), false);
	seeds = AddNextTokenSeed(seeds, TOKEN_PackageLabelNoExtension, packageFilename->GetAssetLabelNoExtension(), false);
	seeds = AddNextTokenSeed(seeds, TOKEN_PackageExtension, packageFilename->GetExtension(), false);
	seeds = AddNextTokenSeed(seeds, TOKEN_PackagePlatform, packagePlatform, false);
	seeds = AddNextTokenSeed(seeds, TOKEN_PackageGroup, packageGroup, false);
	seeds = AddNextTokenSeed(seeds, TOKEN_PackageObject, packageObject, false);
	seeds = AddNextTokenSeed(seeds, TOKEN_PackageFile, packageFile, false);

	return seeds;
}


// 
String* MOG_Tokens::GetFilenameTokenSeeds(MOG_Filename* pFilename)
{
	String* seeds = S"";

	// Make sure we have a valid pFilename?
	if (pFilename)
	{
// JohnRen - Disabled because these would result in temporary classifications being automatically created
		{
//			seeds = AddNextTokenSeed(seeds, TOKEN_FullFilename, pFilename->GetEncodedFilename(), true);
//			seeds = AddNextTokenSeed(seeds, TOKEN_AssetPath, pFilename->GetAssetEncodedPath(), false);
// JohnRen - Had to enable these again because they are used in package commands...substituting the more generic original counterparts
			{
				seeds = AddNextTokenSeed(seeds, TOKEN_FullFilename, pFilename->GetOriginalFilename(), true);
				seeds = AddNextTokenSeed(seeds, TOKEN_AssetPath, pFilename->GetPath(), false);
			}
//			seeds = AddNextTokenSeed(seeds, TOKEN_AssetFilesPath, pFilename->GetAssetEncodedFilesPath(), true);
//			seeds = AddNextTokenSeed(seeds, TOKEN_AssetRelativeFile, pFilename->GetAssetEncodedRelativeFile(), true);
		}

		seeds = AddNextTokenSeed(seeds, TOKEN_Drive, pFilename->GetDrive(), true);
		seeds = AddNextTokenSeed(seeds, TOKEN_Path, pFilename->GetPath(), true);
		seeds = AddNextTokenSeed(seeds, TOKEN_Filename, pFilename->GetFilename(), true);
		seeds = AddNextTokenSeed(seeds, TOKEN_Extension, pFilename->GetExtension(), true);
		seeds = AddNextTokenSeed(seeds, TOKEN_ProjectName, pFilename->GetProjectName(), true);
		seeds = AddNextTokenSeed(seeds, TOKEN_ProjectPath, pFilename->GetProjectPath(), true);
		seeds = AddNextTokenSeed(seeds, TOKEN_UserName, pFilename->GetUserName(), true);
		seeds = AddNextTokenSeed(seeds, TOKEN_UserPath, pFilename->GetUserPath(), true);
		seeds = AddNextTokenSeed(seeds, TOKEN_BoxName, pFilename->GetBoxName(), true);
		seeds = AddNextTokenSeed(seeds, TOKEN_BoxPath, pFilename->GetBoxPath(), true);
		seeds = AddNextTokenSeed(seeds, TOKEN_GroupName, pFilename->GetGroupName(), true);
		seeds = AddNextTokenSeed(seeds, TOKEN_GroupTree, pFilename->GetGroupTree(), true);
		seeds = AddNextTokenSeed(seeds, TOKEN_AssetFilesName, pFilename->GetAssetFilesName(), true);
		seeds = AddNextTokenSeed(seeds, TOKEN_AssetFilesScope, pFilename->GetAssetFilesScope(), true);
		seeds = AddNextTokenSeed(seeds, TOKEN_VersionTimeStamp, pFilename->GetVersionTimeStamp(), true);
		seeds = AddNextTokenSeed(seeds, TOKEN_DeletedTimeStamp, pFilename->GetDeletedTimeStamp(), true);
		seeds = AddNextTokenSeed(seeds, TOKEN_RepositoryPath, pFilename->GetRepositoryPath(), true);

		seeds = AddNextTokenSeed(seeds, TOKEN_AssetFullName, pFilename->GetAssetFullName(), false);
		seeds = AddNextTokenSeed(seeds, TOKEN_AssetName, pFilename->GetAssetName(), false);
		seeds = AddNextTokenSeed(seeds, TOKEN_AssetPlatform, pFilename->GetAssetPlatform(), false);
		seeds = AddNextTokenSeed(seeds, TOKEN_AssetLabel, pFilename->GetAssetLabel(), false);
		seeds = AddNextTokenSeed(seeds, TOKEN_AssetLabelNoExtension, DosUtils::PathGetFileNameWithoutExtension(pFilename->GetAssetLabel()), false);

		// Add all classification parts
		seeds = MOG_Tokens::AppendTokenSeeds(seeds, GetClassificationTokenSeeds(pFilename->GetAssetClassification()));
	}

	return seeds;
}


// 
String* MOG_Tokens::GetClassificationTokenSeeds(String *classification)
{
	String* seeds = S"";

	// Make sure we have a valid pFilename?
	if (classification &&
		classification->Length)
	{
		// Add all classification parts
		seeds = AddNextTokenSeed(seeds, TOKEN_AssetClassification, MOG_Filename::GetAdamlessClassification(classification), false);
		seeds = AddNextTokenSeed(seeds, TOKEN_AssetClassificationFull, classification, false);
		seeds = AddNextTokenSeed(seeds, TOKEN_AssetClassificationPath, MOG_Filename::GetClassificationPath(MOG_Filename::GetAdamlessClassification(classification)), false);
		seeds = AddNextTokenSeed(seeds, TOKEN_AssetClassificationPathFull, MOG_Filename::GetClassificationPath(classification), false);
		String* parts[] = MOG_Filename::SplitClassificationString(classification);
		for (int i = 0; i < parts->Count; i++)
		{
			String* token = TOKEN_AssetClassificationX->Replace(S"X", Convert::ToString(i));
			seeds = AddNextTokenSeed(seeds, token, parts[i], false);
		}
	}

	return seeds;
}


String* MOG_Tokens::GetTimeTokenSeeds()
{
	String* seeds = S"";
	String* formatString = S"";

	// Generate a string of all the tokens, delimited by a comma
	for(int i = 0; i < mTimeTokens->Length; ++i)
	{
		formatString = String::Concat(formatString, mTimeTokens[i], ",");
	}

	return formatString;
}
//
String* MOG_Tokens::GetTimeTokenSeeds(MOG_Time* pTime)
{
	String* seeds = S"";
	String* formatString = S"";

	// Make sure we need a valid pTime?
	if (pTime)
	{
		// Generate a string of all the tokens, delimited by a comma
		for(int i = 0; i < mTimeTokens->Length; ++i)
		{
			formatString = String::Concat(formatString, mTimeTokens[i], ",");
		}
		// Get a formatted time string from the formatString
		String* timeString = pTime->FormatString(formatString);

		// Create a __wchar_t[] to make String::Split happy
		Char delimiter[] = (S",")->ToCharArray();
		// Split all values out for seeds
		String* values[] = timeString->Split(delimiter);

		// Go through each token:value pair and append to seeds
		for(int i = 0; i < mTimeTokens->Length; ++i)
		{
			// Add each token:value pair to seeds
			seeds = AddNextTokenSeed(seeds, mTimeTokens[i], values[i], false);
		}
	}

	return seeds;
}


//
String* MOG_Tokens::GetPropertyTokenSeeds(MOG_Property* pProperty)
{
	String* seeds = S"";

	// Add the property's key and value...ignoring the property's section
	String* tokenName = String::Concat(S"{", pProperty->mPropertyKey, S"}");
	String* tokenValue = MOG_Tokens::GetFormattedString(pProperty->mPropertyValue);
	seeds = AddNextTokenSeed(seeds, tokenName, tokenValue, false);

	return seeds;
}


//
String* MOG_Tokens::GetPropertyTokenSeeds(ArrayList* pProperties)
{
	String* seeds = S"";

	// Make sure we have something?
	if (pProperties)
	{
		// Loop through all the specified properties
		for (int p = 0; p < pProperties->Count; p++)
		{
			// Add property to seeds
			MOG_Property* pProperty = __try_cast<MOG_Property* >(pProperties->Item[p]);
			seeds = AppendTokenSeeds(seeds, GetPropertyTokenSeeds(pProperty));
		}
	}

	return seeds;
}


//
String* MOG_Tokens::AppendTokenSeeds(String* seeds, String* newSeeds)
{
	// Make sure we have something to add?
	if (newSeeds->Length)
	{
		// Check if we already have seeds?
		if (seeds->Length)
		{
			// Append delimiter
			seeds = String::Concat(seeds, S";");
		}

		// Append token and value to seeds
		seeds = String::Concat(seeds, newSeeds);
	}

	return seeds;
}


String* MOG_Tokens::GetFormattedString(String* format)
{
	if (TokenExist(format))
	{
		String* seeds = S"";

		// Build all the seeds
		seeds = MOG_Tokens::AppendTokenSeeds(seeds, GetSystemTokenSeeds());
		seeds = MOG_Tokens::AppendTokenSeeds(seeds, GetProjectTokenSeeds(MOG_ControllerProject::GetProject()));
//?	MultiWorkspaces - Can we consider removing this from the tokens and only call the other GetFormattedString() when we really need to?
		seeds = MOG_Tokens::AppendTokenSeeds(seeds, GetWorkspaceTokenSeeds(MOG_ControllerProject::GetCurrentSyncDataController()));

		// Call the normal API
		return GetFormattedString(format, seeds);
	}

	return format;
}


//
String* MOG_Tokens::GetFormattedString(String* format, MOG_Filename* seedAssetFilename)
{ 
	if (TokenExist(format))
	{
		String* seeds = S"";

		// Build all the seeds
		seeds = MOG_Tokens::AppendTokenSeeds(seeds, GetSystemTokenSeeds());
		seeds = MOG_Tokens::AppendTokenSeeds(seeds, GetProjectTokenSeeds(MOG_ControllerProject::GetProject()));
//?	MultiWorkspaces - Can we consider removing this from the tokens and only call the other GetFormattedString() when we really need to?
		seeds = MOG_Tokens::AppendTokenSeeds(seeds, GetWorkspaceTokenSeeds(MOG_ControllerProject::GetCurrentSyncDataController()));
		seeds = MOG_Tokens::AppendTokenSeeds(seeds, GetFilenameTokenSeeds(seedAssetFilename));

		// Call the normal API
		return GetFormattedString(format, seeds);
	}

	return format;
}


//
String* MOG_Tokens::GetFormattedString(String* format, MOG_Filename* seedAssetFilename, ArrayList* properties)
{ 
	if (TokenExist(format))
	{
		String* seeds = S"";

		// Build all the seeds
		seeds = MOG_Tokens::AppendTokenSeeds(seeds, GetSystemTokenSeeds());
		seeds = MOG_Tokens::AppendTokenSeeds(seeds, GetProjectTokenSeeds(MOG_ControllerProject::GetProject()));
//?	MultiWorkspaces - Can we consider removing this from the tokens and only call the other GetFormattedString() when we really need to?
		seeds = MOG_Tokens::AppendTokenSeeds(seeds, GetWorkspaceTokenSeeds(MOG_ControllerProject::GetCurrentSyncDataController()));
		seeds = MOG_Tokens::AppendTokenSeeds(seeds, GetFilenameTokenSeeds(seedAssetFilename));
		seeds = MOG_Tokens::AppendTokenSeeds(seeds, GetPropertyTokenSeeds(properties));

		// Call the normal API
		return GetFormattedString(format, seeds);
	}

	return format;
}


//
String* MOG_Tokens::GetFormattedString(String* format, MOG_Filename* seedAssetFilename, ArrayList* properties, String* sourcePath, String* sourceFilePattern, String* destinationPath)
{
	if (TokenExist(format))
	{
		String* seeds = S"";

		// Build all the seeds
		seeds = MOG_Tokens::AppendTokenSeeds(seeds, GetSystemTokenSeeds());
		seeds = MOG_Tokens::AppendTokenSeeds(seeds, GetProjectTokenSeeds(MOG_ControllerProject::GetProject()));
//?	MultiWorkspaces - Can we consider removing this from the tokens and only call the other GetFormattedString() when we really need to?
		seeds = MOG_Tokens::AppendTokenSeeds(seeds, GetWorkspaceTokenSeeds(MOG_ControllerProject::GetCurrentSyncDataController()));
		seeds = MOG_Tokens::AppendTokenSeeds(seeds, GetFilenameTokenSeeds(seedAssetFilename));
		seeds = MOG_Tokens::AppendTokenSeeds(seeds, GetPropertyTokenSeeds(properties));
		seeds = MOG_Tokens::AppendTokenSeeds(seeds, GetRipperTokenSeeds(sourcePath, sourceFilePattern, destinationPath));

		// Call the normal API
		return GetFormattedString(format, seeds);
	}

	return format;
}


//
String* MOG_Tokens::GetFormattedString(String* format, MOG_Filename* seedAssetFilename, ArrayList* properties, String* assignedPackageStatement)
{
	if (TokenExist(format))
	{
		String* seeds = S"";

		// Build all the seeds
		seeds = MOG_Tokens::AppendTokenSeeds(seeds, GetSystemTokenSeeds());
		seeds = MOG_Tokens::AppendTokenSeeds(seeds, GetProjectTokenSeeds(MOG_ControllerProject::GetProject()));
//?	MultiWorkspaces - Can we consider removing this from the tokens and only call the other GetFormattedString() when we really need to?
		seeds = MOG_Tokens::AppendTokenSeeds(seeds, GetWorkspaceTokenSeeds(MOG_ControllerProject::GetCurrentSyncDataController()));
		seeds = MOG_Tokens::AppendTokenSeeds(seeds, GetFilenameTokenSeeds(seedAssetFilename));
		seeds = MOG_Tokens::AppendTokenSeeds(seeds, GetPropertyTokenSeeds(properties));
		seeds = MOG_Tokens::AppendTokenSeeds(seeds, GetPackageTokenSeeds(assignedPackageStatement));

		// Call the normal API
		return GetFormattedString(format, seeds);
	}

	return format;
}


//
String* MOG_Tokens::GetFormattedString(String* format, MOG_Filename* seedAssetFilename, MOG_PackageMerge_PackageFileInfo* packageFileInfo, String* workspaceDirectory, String* platformName)
{
	if (TokenExist(format))
	{
		String* seeds = S"";

		// Obtain the properties from the specified PackageFileInfo
		ArrayList* properties = NULL;
		if (packageFileInfo)
		{
			properties = packageFileInfo->mPackageProperties->GetApplicableProperties();
		}

		// Build all the seeds
		seeds = MOG_Tokens::AppendTokenSeeds(seeds, GetSystemTokenSeeds());
		seeds = MOG_Tokens::AppendTokenSeeds(seeds, GetProjectTokenSeeds(MOG_ControllerProject::GetProject()));
		seeds = MOG_Tokens::AppendTokenSeeds(seeds, GetWorkspaceTokenSeeds(workspaceDirectory, platformName));
		seeds = MOG_Tokens::AppendTokenSeeds(seeds, GetFilenameTokenSeeds(seedAssetFilename));
		seeds = MOG_Tokens::AppendTokenSeeds(seeds, GetPropertyTokenSeeds(properties));
		seeds = MOG_Tokens::AppendTokenSeeds(seeds, GetPackageTokenSeeds(packageFileInfo));

		// Call the normal API
		return GetFormattedString(format, seeds);
	}

	return format;
}


//
String* MOG_Tokens::GetFormattedString(String* format, MOG_Filename* seedAssetFilename, MOG_PackageMerge_PackageFileInfo* packageFileInfo, String* workspaceDirectory, String* platformName, MOG_Properties* properties, String* assignedPackageStatement)
{
	if (TokenExist(format))
	{
		String* seeds = S"";

		// Obtain the properties from the specified PackageFileInfo
		ArrayList* assetProperties = NULL;
		if (properties)
		{
			properties->SetScope(packageFileInfo->mPackageAssetFilename->GetAssetPlatform());
			assetProperties = properties->GetApplicableProperties();
		}

		// Obtain the properties from the specified PackageFileInfo
		ArrayList* packageProperties = NULL;
		if (packageFileInfo)
		{
			packageProperties = packageFileInfo->mPackageProperties->GetApplicableProperties();
		}

		// Build all the seeds
		seeds = MOG_Tokens::AppendTokenSeeds(seeds, GetSystemTokenSeeds());
		seeds = MOG_Tokens::AppendTokenSeeds(seeds, GetProjectTokenSeeds(MOG_ControllerProject::GetProject()));
		seeds = MOG_Tokens::AppendTokenSeeds(seeds, GetWorkspaceTokenSeeds(workspaceDirectory, platformName));
		seeds = MOG_Tokens::AppendTokenSeeds(seeds, GetFilenameTokenSeeds(seedAssetFilename));
		seeds = MOG_Tokens::AppendTokenSeeds(seeds, GetPropertyTokenSeeds(assetProperties));
		seeds = MOG_Tokens::AppendTokenSeeds(seeds, GetPropertyTokenSeeds(packageProperties));
		seeds = MOG_Tokens::AppendTokenSeeds(seeds, GetPackageTokenSeeds(packageFileInfo, assignedPackageStatement));

		// Call the normal API
		return GetFormattedString(format, seeds);
	}

	return format;
}


//
String* MOG_Tokens::GetFormattedString(String* format, String* seeds)
{
	String* str = "";

	if (TokenExist(format))
	{
		for (int i = 0; i < format->Length; /* i++ */)
		{
			// Check for the beginning of a token
			if (format->Chars[i] == '{')
			{
				// Attempt to extract the token
				String* token = GetFormattedString_ParseToken(format->Substring(i));
				if (token->Length)
				{
					// Skip the token
					i += token->Length;

					// Check for any token commands?
					String* command = GetFormattedString_ParseCommands(token);
					String* baseToken = (command->Length == 0) ? token : GetFormattedString_ParseBaseToken(token);
					// Attempt to extract the value of this token from our seeds
					String* seedValue = GetFormattedString_GetSeedString(baseToken, seeds);
					if (seedValue)
					{
						// Check if we detected a command?
						if (command->Length)
						{
							// Execute the token command on the seedValued
							seedValue = GetFormattedString_ExecuteTokenCommands(seedValue, command, seeds);
						}

						// Add the seedValue onto our string
						str = String::Concat(str, seedValue);
						continue;
					}

					// Since the token was not found in our seeds, we should place the token back onto the string
					str = String::Concat(str, token);
					continue;
				}
			}

			// Add the character to the path
			str = String::Concat(str, format->Substring(i, 1));
			i++;
		}

		return str;
	}

	return format;
}


//
String* MOG_Tokens::GetFormattedString_GetSeedString(String* token, String* seeds)
{
	String* seedValue = NULL;

	// Check if we have any seeds?
	if (seeds->Length)
	{
		// Find the token within the seeds
		String* test = String::Concat(token, S"=");
		int tokenStartPos = seeds->LastIndexOf(test, StringComparison::CurrentCultureIgnoreCase);
		if (tokenStartPos != -1)
		{
			// Find the endPos of this token
			int tokenEqualsPos = tokenStartPos + test->Length;
			int tokenEndPos = seeds->IndexOfAny(S",;"->ToCharArray(), tokenEqualsPos);
			if (tokenEndPos != -1)
			{
				// Extract the seedValue
				seedValue = seeds->Substring(tokenEqualsPos, tokenEndPos - tokenEqualsPos); 
			}
			else
			{
				seedValue = seeds->Substring(tokenEqualsPos);
			}
		}
	}

	return seedValue;
}


String* MOG_Tokens::GetFormattedString_ParseToken(String* text)
{
	String* token = S"";

	// Parse the specified text
	String* tempToken = S"";
	int nestedTokens = 0;
	for (int i = 0; i < text->Length; /* i++ */)
	{
		// Check for the beginning of another token
		if (text->Chars[i] == '{')
		{
			nestedTokens++;
		}
		// Check for the beginning of another token
		if (text->Chars[i] == '}')
		{
			nestedTokens--;
		}

		// Add the character to the path
		tempToken = String::Concat(tempToken, text->Substring(i, 1));
		i++;

		if (nestedTokens == 0)
		{
			token = tempToken;
			break;
		}
	}

	return token;
}


String* MOG_Tokens::GetFormattedString_ParseCommands(String* token)
{
	// Strip off the token's markers
	token = token->Trim(S"{}"->ToCharArray());

	// Check if this token contains the required '.' '(' ')' seperators?
	while (token->Contains(S".") &&
		   token->Contains(S"(") &&
		   token->Contains(S")"))
	{
		// Strip off the text upto our '.' seperator
		token = token->Substring(token->IndexOf(S"."));

		// Loop through all of our commands
		for (int i = 0; i < mCommands->Count; i++)
		{
			String* thisCommand = dynamic_cast<String*>(mCommands->Item[i]);

			// Check if this command is referenced within this token?
			String* testCommand = String::Concat(S".", thisCommand, S"(");
			if (token->StartsWith(testCommand, StringComparison::CurrentCultureIgnoreCase))
			{
				// Get all remaining commands
				String* commands = token->Substring(1);
				return commands;
			}
		}

		// Looks like we faile to match a command
		// Skip this '.' seperator and try again
		token = token->Substring(1);
	}

	return "";
}


String* MOG_Tokens::GetFormattedString_ParseBaseToken(String* token)
{
	// Get the command within this token
	String *testCommand = String::Concat(S".", GetFormattedString_ParseCommands(token));
	// Strip out the command and its arguments
	String* baseToken = token->Replace(testCommand, S"");

	return baseToken;
}


String* MOG_Tokens::GetFormattedString_ExecuteTokenCommands(String* text, String* commands, String* seeds)
{
	try
	{
		// Check if we have any commands?
		while (commands->Length)
		{
			int pos = commands->IndexOf(S"(");
			if (pos != -1)
			{
				String* command = commands->Substring(0, pos);
				String* commandArgs = GetFormattedString_ParseNestedString(commands->Substring(pos));

				text = GetFormattedString_ExecuteTokenCommand(text, command, commandArgs, seeds);

				commands = GetFormattedString_ParseCommands(commands->Substring(command->Length + commandArgs->Length));
			}
			else
			{
				break;
			}
		}
	}
	catch (...)
	{
	}

	return text;
}


String* MOG_Tokens::GetFormattedString_ExecuteTokenCommand(String *text, String* command, String* commandArgs, String* seeds)
{
	try
	{
		if (String::Compare(command, TOKEN_CommandSubString, true) == 0)
		{
			int numArgs = GetFormattedString_CountCommandArgs(commandArgs);
			if (numArgs == 1)
			{
				int startIndex = GetFormattedString_GetCommandArgAsInt(commandArgs, 0, seeds);
				if (startIndex > text->Length)
				{
					startIndex = text->Length;
				}

				text = text->Substring(startIndex);
			}
			else if (numArgs == 2)
			{
				int startIndex = GetFormattedString_GetCommandArgAsInt(commandArgs, 0, seeds);
				if (startIndex > text->Length)
				{
					startIndex = text->Length;
				}
				int count = GetFormattedString_GetCommandArgAsInt(commandArgs, 1, seeds);
				if (startIndex + count > text->Length)
				{
					count = text->Length - startIndex;
				}

				text = text->Substring(startIndex, count);
			}
		}
		else if (String::Compare(command, TOKEN_CommandReplace, true) == 0)
		{
			int numArgs = GetFormattedString_CountCommandArgs(commandArgs);
			if (numArgs == 2)
			{
				String* oldValue = GetFormattedString_GetCommandArg(commandArgs, 0, seeds);
				String* newValue = GetFormattedString_GetCommandArg(commandArgs, 1, seeds);

				text = text->Replace(oldValue, newValue);
			}
		}
		else if (String::Compare(command, TOKEN_CommandInsert, true) == 0)
		{
			int numArgs = GetFormattedString_CountCommandArgs(commandArgs);
			if (numArgs == 2)
			{
				int startIndex = GetFormattedString_GetCommandArgAsInt(commandArgs, 0, seeds);
				if (startIndex > text->Length)
				{
					startIndex = text->Length;
				}

				String* value = GetFormattedString_GetCommandArg(commandArgs, 1, seeds);

				text = text->Insert(startIndex, value);
			}
		}
		else if (String::Compare(command, TOKEN_CommandRemove, true) == 0)
		{
			int numArgs = GetFormattedString_CountCommandArgs(commandArgs);
			if (numArgs == 1)
			{
				int startIndex = GetFormattedString_GetCommandArgAsInt(commandArgs, 0, seeds);
				if (startIndex > text->Length)
				{
					startIndex = text->Length;
				}

				text = text->Remove(startIndex);
			}
			if (numArgs == 2)
			{
				int startIndex = GetFormattedString_GetCommandArgAsInt(commandArgs, 0, seeds);
				if (startIndex > text->Length)
				{
					startIndex = text->Length;
				}
				int count = GetFormattedString_GetCommandArgAsInt(commandArgs, 1, seeds);
				if (startIndex + count > text->Length)
				{
					count = text->Length - startIndex;
				}

				text = text->Remove(startIndex, count);
			}
		}
		else if (String::Compare(command, TOKEN_CommandTrim, true) == 0)
		{
			int numArgs = GetFormattedString_CountCommandArgs(commandArgs);
			if (numArgs == 0)
			{
				text = text->Trim();
			}
			else if (numArgs == 1)
			{
				String* trimChars = GetFormattedString_GetCommandArg(commandArgs, 0, seeds);
				text = text->Trim(trimChars->ToCharArray());
			}
		}
		else if (String::Compare(command, TOKEN_CommandTrimEnd, true) == 0)
		{
			int numArgs = GetFormattedString_CountCommandArgs(commandArgs);
			if (numArgs == 1)
			{
				String* trimChars = GetFormattedString_GetCommandArg(commandArgs, 0, seeds);
				text = text->TrimEnd(trimChars->ToCharArray());
			}
		}
		else if (String::Compare(command, TOKEN_CommandTrimStart, true) == 0)
		{
			int numArgs = GetFormattedString_CountCommandArgs(commandArgs);
			if (numArgs == 1)
			{
				String* trimChars = GetFormattedString_GetCommandArg(commandArgs, 0, seeds);
				text = text->TrimStart(trimChars->ToCharArray());
			}
		}
		else if (String::Compare(command, TOKEN_CommandLength, true) == 0)
		{
			int numArgs = GetFormattedString_CountCommandArgs(commandArgs);
			if (numArgs == 0)
			{
				text = text->Length.ToString();
			}
		}
		else if (String::Compare(command, TOKEN_CommandLower, true) == 0)
		{
			int numArgs = GetFormattedString_CountCommandArgs(commandArgs);
			if (numArgs == 0)
			{
				text = text->ToLower();
			}
		}
		else if (String::Compare(command, TOKEN_CommandUpper, true) == 0)
		{
			int numArgs = GetFormattedString_CountCommandArgs(commandArgs);
			if (numArgs == 0)
			{
				text = text->ToUpper();
			}
		}
		else if (String::Compare(command, TOKEN_CommandPadLeft, true) == 0)
		{
			int numArgs = GetFormattedString_CountCommandArgs(commandArgs);
			if (numArgs == 1)
			{
				int totalWidth = GetFormattedString_GetCommandArgAsInt(commandArgs, 0, seeds);
				text = text->PadLeft(totalWidth);
			}
			else if (numArgs == 2)
			{
				int totalWidth = GetFormattedString_GetCommandArgAsInt(commandArgs, 0, seeds);
				String* paddingChar = GetFormattedString_GetCommandArg(commandArgs, 1, seeds);
				text = text->PadLeft(totalWidth, paddingChar->Chars[0]);
			}
		}
		else if (String::Compare(command, TOKEN_CommandPadRight, true) == 0)
		{
			int numArgs = GetFormattedString_CountCommandArgs(commandArgs);
			if (numArgs == 1)
			{
				int totalWidth = GetFormattedString_GetCommandArgAsInt(commandArgs, 0, seeds);
				text = text->PadRight(totalWidth);
			}
			else if (numArgs == 2)
			{
				int totalWidth = GetFormattedString_GetCommandArgAsInt(commandArgs, 0, seeds);
				String* paddingChar = GetFormattedString_GetCommandArg(commandArgs, 1, seeds);
				text = text->PadRight(totalWidth, paddingChar->Chars[0]);
			}
		}
		else if (String::Compare(command, TOKEN_CommandIndexOf, true) == 0)
		{
			int numArgs = GetFormattedString_CountCommandArgs(commandArgs);
			if (numArgs == 1)
			{
				String* value = GetFormattedString_GetCommandArg(commandArgs, 0, seeds);
				text = text->IndexOf(value).ToString();
			}
			else if (numArgs == 2)
			{
				String* value = GetFormattedString_GetCommandArg(commandArgs, 0, seeds);
				int startIndex = GetFormattedString_GetCommandArgAsInt(commandArgs, 1, seeds);
				text = text->IndexOf(value, startIndex).ToString();
			}
			else if (numArgs == 3)
			{
				String* value = GetFormattedString_GetCommandArg(commandArgs, 0, seeds);
				int startIndex = GetFormattedString_GetCommandArgAsInt(commandArgs, 1, seeds);
				int count = GetFormattedString_GetCommandArgAsInt(commandArgs, 2, seeds);
				text = text->IndexOf(value, startIndex, count).ToString();
			}
		}
		else if (String::Compare(command, TOKEN_CommandIndexOfAny, true) == 0)
		{
			int numArgs = GetFormattedString_CountCommandArgs(commandArgs);
			if (numArgs == 1)
			{
				String* anyOf = GetFormattedString_GetCommandArg(commandArgs, 0, seeds);
				text = text->IndexOfAny(anyOf->ToCharArray()).ToString();
			}
			else if (numArgs == 2)
			{
				String* anyOf = GetFormattedString_GetCommandArg(commandArgs, 0, seeds);
				int startIndex = GetFormattedString_GetCommandArgAsInt(commandArgs, 1, seeds);
				text = text->IndexOfAny(anyOf->ToCharArray(), startIndex).ToString();
			}
			else if (numArgs == 3)
			{
				String* anyOf = GetFormattedString_GetCommandArg(commandArgs, 0, seeds);
				int startIndex = GetFormattedString_GetCommandArgAsInt(commandArgs, 1, seeds);
				int count = GetFormattedString_GetCommandArgAsInt(commandArgs, 2, seeds);
				text = text->IndexOfAny(anyOf->ToCharArray(), startIndex, count).ToString();
			}
		}
		else if (String::Compare(command, TOKEN_CommandLastIndexOf, true) == 0)
		{
			int numArgs = GetFormattedString_CountCommandArgs(commandArgs);
			if (numArgs == 1)
			{
				String* value = GetFormattedString_GetCommandArg(commandArgs, 0, seeds);
				text = text->LastIndexOf(value).ToString();
			}
			else if (numArgs == 2)
			{
				String* value = GetFormattedString_GetCommandArg(commandArgs, 0, seeds);
				int startIndex = GetFormattedString_GetCommandArgAsInt(commandArgs, 1, seeds);
				text = text->LastIndexOf(value, startIndex).ToString();
			}
			else if (numArgs == 3)
			{
				String* value = GetFormattedString_GetCommandArg(commandArgs, 0, seeds);
				int startIndex = GetFormattedString_GetCommandArgAsInt(commandArgs, 1, seeds);
				int count = GetFormattedString_GetCommandArgAsInt(commandArgs, 2, seeds);
				text = text->LastIndexOf(value, startIndex, count).ToString();
			}
		}
		else if (String::Compare(command, TOKEN_CommandLastIndexOfAny, true) == 0)
		{
			int numArgs = GetFormattedString_CountCommandArgs(commandArgs);
			if (numArgs == 1)
			{
				String* anyOf = GetFormattedString_GetCommandArg(commandArgs, 0, seeds);
				text = text->LastIndexOfAny(anyOf->ToCharArray()).ToString();
			}
			else if (numArgs == 2)
			{
				String* anyOf = GetFormattedString_GetCommandArg(commandArgs, 0, seeds);
				int startIndex = GetFormattedString_GetCommandArgAsInt(commandArgs, 1, seeds);
				text = text->LastIndexOfAny(anyOf->ToCharArray(), startIndex).ToString();
			}
			else if (numArgs == 3)
			{
				String* anyOf = GetFormattedString_GetCommandArg(commandArgs, 0, seeds);
				int startIndex = GetFormattedString_GetCommandArgAsInt(commandArgs, 1, seeds);
				int count = GetFormattedString_GetCommandArgAsInt(commandArgs, 2, seeds);
				text = text->LastIndexOfAny(anyOf->ToCharArray(), startIndex, count).ToString();
			}
		}
	}
	catch (...)
	{
	}

	return text;
}
				
int MOG_Tokens::GetFormattedString_CountCommandArgs(String* commandArgs)
{
	try
	{
		// Check if this contains any arguments?
		if (commandArgs->Length &&
			String::Compare(commandArgs, S"()", true) != 0)
		{
			// Count how many args we have by spliting on the commas
			String* args[] = commandArgs->Split(S","->ToCharArray());
			return args->Length;
		}
	}
	catch (...)
	{
	}

	return 0;
}


String* MOG_Tokens::GetFormattedString_GetCommandArg(String* commandArgs, int index, String* seeds)
{
	String* argText = "";

	try
	{
		// Count how many args we have by spliting on the commas
		commandArgs = commandArgs->Trim(S"()"->ToCharArray());
		String* args[] = commandArgs->Split(S","->ToCharArray());

		// Make sure we resolve any contained tokens being used as arguments
		argText = GetFormattedString(args[index], seeds);
	}
	catch (...)
	{
	}

	return argText;
}


int MOG_Tokens::GetFormattedString_GetCommandArgAsInt(String* commandArgs, int index, String* seeds)
{
	String* text = GetFormattedString_GetCommandArg(commandArgs, index, seeds);
	int value = 0;
	try
	{
		value = GetFormattedString_ResolvedMath(text);
	}
	catch (...)
	{
	}

	return value;
}


String* MOG_Tokens::GetFormattedString_ParseNestedString(String* text)
{
	int endPos = 0;
	int nested = 0;
	while (endPos < text->Length)
	{
		if (text->Chars[endPos] == '{' ||
			text->Chars[endPos] == '(')
		{
			nested++;
		}
		if (text->Chars[endPos] == '}' ||
			text->Chars[endPos] == ')')
		{
			nested--;
		}
		endPos++;
		if (!nested)
		{
			break;
		}
	}

	String* subText = text->Substring(0, endPos);
	return subText;
}


int MOG_Tokens::GetFormattedString_ResolvedMath(String* text)
{
	int value = 0;

	// Find the startPos for any contained '('?
	int startPos = text->IndexOf(S"(");
	if (startPos != -1)
	{
		String* nestedString = GetFormattedString_ParseNestedString(text->Substring(startPos));
		String* subText = nestedString->Substring(1, nestedString->Length - 2);

		// Rcursively resolve ourself
		value = GetFormattedString_ResolvedMath(subText);
		// Rebuild our text string with resolved value
		text = String::Concat(text->Substring(0, startPos), value.ToString(), text->Substring(startPos + nestedString->Length));
	}

	String* parts[] = text->Split(S"*"->ToCharArray(), 2);
	if (parts->Length > 1)
	{
		// Rcursively resolve ourself
		value = GetFormattedString_ResolvedMath(parts[0]);
		value *= GetFormattedString_ResolvedMath(parts[1]);
	}
	else
	{
		parts = text->Split(S"/"->ToCharArray(), 2);
		if (parts->Length > 1)
		{
			// Rcursively resolve ourself
			value = GetFormattedString_ResolvedMath(parts[0]);
			value /= GetFormattedString_ResolvedMath(parts[1]);
		}
		else
		{
			parts = text->Split(S"+"->ToCharArray(), 2);
			if (parts->Length > 1)
			{
				// Rcursively resolve ourself
				value = GetFormattedString_ResolvedMath(parts[0]);
				value += GetFormattedString_ResolvedMath(parts[1]);
			}
			else
			{
				parts = text->Split(S"-"->ToCharArray(), 2);
				if (parts->Length > 1)
				{
					// Rcursively resolve ourself
					value = GetFormattedString_ResolvedMath(parts[0]);
					value -= GetFormattedString_ResolvedMath(parts[1]);
				}
				else
				{
					value = Convert::ToInt32(text);
				}
			}
		}
	}

	return value;
}


// Private method for expediting token addition
String* MOG_Tokens::AddNextTokenSeed(String* seeds, String* token, String* value, bool bSkipBlankTokenValue)
{
	// Make sure we have something to add?
	if (token->Length)
	{
		// Make sure there was a valid value specified?
		if (value)
		{
			// Check if we should skip blank token values?
			if (bSkipBlankTokenValue &&
				value->Length == 0)
			{
				return seeds;
			}

			// Check if we already have seeds?
			if (seeds->Length)
			{
				// Append delimiter
				seeds = String::Concat(seeds, S";");
			}

			// Append token and value to seeds
			seeds = String::Concat(seeds, token, S"=", value);
		}
	}

	return seeds;
}

