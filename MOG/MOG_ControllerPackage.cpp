//--------------------------------------------------------------------------------
//	MOG_ControllerPackage.cpp
//	
//	
//--------------------------------------------------------------------------------

#include "StdAfx.h"

#include "MOG_Define.h"
#include "MOG_DosUtils.h"
#include "MOG_ControllerSystem.h"
#include "MOG_ControllerProject.h"
#include "MOG_ControllerRepository.h"
#include "MOG_ControllerInbox.h"
#include "MOG_DBInboxAPI.h"
#include "MOG_DBAssetAPI.h"
#include "MOG_DBPackageAPI.h"
#include "MOG_DBSyncedDataAPI.h"
#include "MOG_Tokens.h"
#include "MOG_DBPackageAPI.h"
#include "MOG_DBPostCommandAPI.h"
#include "MOG_EnvironmentVars.h"
#include "MOG_CommandFactory.h"

#include "MOG_ControllerPackage.h"



bool MOG_ControllerPackage::RebuildPackage(MOG_Filename *assetFilename, String *jobLabel)
{
	if (assetFilename->IsLocal())
	{
		// Make sure we have a valid local workspace?
		MOG_ControllerSyncData *pWorkspace = MOG_ControllerProject::GetCurrentSyncDataController();
		if (pWorkspace)
		{
			MOG_Command *rebuildPackage = MOG_CommandFactory::Setup_LocalPackageRebuild(assetFilename->GetEncodedFilename(), pWorkspace->GetPlatformName(), jobLabel);
			// Send our package merge command to the server
			return MOG_ControllerSystem::GetCommandManager()->SendToServer(rebuildPackage);
		}
		else
		{
			// Inform the user they need a valid workspace
		}
	}
	else
	{
		// Send our Rebuild package command to the server
		MOG_Command *rebuildPackage = MOG_CommandFactory::Setup_NetworkPackageRebuild(assetFilename->GetEncodedFilename(), assetFilename->GetAssetPlatform(), jobLabel);
		// Check if we have specified to only use our slave?
		if (MOG_ControllerSystem::GetCommandManager()->mUseOnlyMySlaves)
		{
			// Specify our machine as the valid slave
			rebuildPackage->mValidSlaves = MOG_ControllerSystem::GetComputerName();
		}
		return MOG_ControllerSystem::GetCommandManager()->SendToServer(rebuildPackage);
	}

	return false;
}


String *MOG_ControllerPackage::GetPackageName(String *AssignedPackage)
{
	// Split the package name up
	String* delimStr = S"/";
	Char delimiter[] = delimStr->ToCharArray();
	String* parts[] = AssignedPackage->Split(delimiter, 2);

	String *packageName = "";

	if (parts != NULL)
	{
		if (parts->Count >=1)
		{
			packageName = parts[0];
		}
	}

	return packageName;
}


String *MOG_ControllerPackage::GetPackagePlatform(String *AssignedPackage)
{
	MOG_Filename *packageFilename = new MOG_Filename(GetPackageName(AssignedPackage));
	return packageFilename->GetAssetPlatform();
}


String *MOG_ControllerPackage::GetPackageGroupObjectPath(String *AssignedPackage)
{
	String *path = S"";

	// Check if there is more to the package assignment than just a package name?
	String *packageName = GetPackageName(AssignedPackage);
	if (AssignedPackage->Length > packageName->Length)
	{
		// Extract the remaining portion of the package assignment as it describes our group/object path
		path = AssignedPackage->Substring(packageName->Length + 1);
	}

	// Return the group/object path
	return path;
}


String *MOG_ControllerPackage::GetPackageGroups(String *AssignedPackage)
{
	// Split the package name up
	String* delimStr = S"/";
	Char delimiter[] = delimStr->ToCharArray();
	String* parts[] = AssignedPackage->Split(delimiter, 2);

	String *packageName = "";
	String *packageGroups = "";
	String *packageObjects = "";

	if (parts != NULL)
	{
		if (parts->Count >=1)
		{
			packageName = parts[0];
		}

		if (parts->Count >=2)
		{
			// Make sure this group isn't an object
			if (!parts[1]->StartsWith(S"("))
			{
				packageGroups = parts[1];

				// Check if there are any package specifiers within this packageGroups?
				int index = packageGroups->IndexOf(S"/(");
				if (index != -1)
				{
					// Split the packageObjects from the packageGroups
					packageObjects = packageGroups->Substring(index + 1)->Trim(delimiter);
					packageGroups = packageGroups->Substring(0, packageGroups->Length - packageObjects->Length)->Trim(delimiter);
				}
			}
			else
			{
				packageObjects = parts[1];
			}
		}
	}

	return packageGroups;
}


String *MOG_ControllerPackage::GetPackageGroupParent(String *groups)
{
	String *groupParent = S"";

	// Check if we can get the parent?
	int pos = groups->LastIndexOf("/");
	if (pos != -1)
	{
		groupParent = groups->Substring(0, pos);
	}

	return groupParent;
}


String *MOG_ControllerPackage::GetPackageGroupLevels(String *AssignedPackage)[]
{
	String* parts[] = new String*[0];
	
	// Split the packageGroups up
	String* delimStr = S"/";
	Char delimiter[] = delimStr->ToCharArray();
		
	String* packageGroups = GetPackageGroups(AssignedPackage);
	if (!String::IsNullOrEmpty(packageGroups))
	{
		parts = packageGroups->Split(delimiter);
	}
	
	return parts;
}


String *MOG_ControllerPackage::GetPackageObjects(String *AssignedPackage)
{
	// Split the package name up
	String* delimStr = S"/";
	Char delimiter[] = delimStr->ToCharArray();
	String* parts[] = AssignedPackage->Split(delimiter, 2);

	String *packageName = "";
	String *packageGroups = "";
	String *packageObjects = "";

	if (parts != NULL)
	{
		if (parts->Count >=1)
		{
			packageName = parts[0];
		}

		if (parts->Count >=2)
		{
			// Make sure this group isn't an object
			if (!parts[1]->StartsWith(S"("))
			{
				packageGroups = parts[1];

				// Check if there are any package specifiers within this packageGroups?
				int index = packageGroups->IndexOf(S"/(");
				if (index != -1)
				{
					// Split the packageObjects from the packageGroups
					packageObjects = packageGroups->Substring(index + 1)->Trim(delimiter);
					packageGroups = packageGroups->Substring(0, packageGroups->Length - packageObjects->Length)->Trim(delimiter);
				}
			}
			else
			{
				packageObjects = parts[1];
			}
		}
	}

	return packageObjects;
}


String *MOG_ControllerPackage::GetPackageObjectLevels(String *AssignedPackage)[]
{
	String* parts[] = new String*[0];
	
	// Split the packageObjects up
	String* delimStr = S"/";
	Char delimiter[] = delimStr->ToCharArray();
		
	String* packageObjects = GetPackageObjects(AssignedPackage);
	if (!String::IsNullOrEmpty(packageObjects))
	{
		parts = packageObjects->Split(delimiter);
	}
	
	return parts;
}


String *MOG_ControllerPackage::AddPackageObjectIdentifiers(String *packageObjects)
{
	// First strip us down
	packageObjects = StripPackageObjectIdentifiers(packageObjects);

	// Split the packageObjects
	String* delimStr = S"/";
	Char delimiter[] = delimStr->ToCharArray();
	String *parts[] = packageObjects->Split(delimiter);

	// Put packageObjects all back together with the package object identifiers
	packageObjects = S"";
	for (int i = 0; i < parts->Count; i++)
	{
		// Check if we already have something in packageObjects?
		if (packageObjects->Length)
		{
			packageObjects = String::Concat(packageObjects, S"/");
		}

		// Add the packageObjects with the package object specifiers
		packageObjects = String::Concat(packageObjects, S"(", parts[i], S")");
	}

	return packageObjects;
}


String *MOG_ControllerPackage::StripPackageObjectIdentifiers(String *packageObjects)
{
	String* stripped = S"";
	
	// Trim the outer edges of packageObjects
	packageObjects = packageObjects->Trim(S"/"->ToCharArray());

	String* parts[] = packageObjects->Split(S"/"->ToCharArray());
	for (int i = 0; i < parts->Length; i++)
	{
		if (parts[i]->StartsWith(S"(") && parts[i]->EndsWith(S")"))
		{
			parts[i] = parts[i]->Substring(1, parts[i]->Length - 2);
		}

		if (i > 0)
		{
			stripped = String::Concat(stripped, S"/");
		}

		stripped = String::Concat(stripped, parts[i]);
	}
	
	return stripped;
}


String* MOG_ControllerPackage::CombinePackageInternalPath(String* groups, String* objects)
{
	String* path = groups;

	if (objects->Length)
	{
		objects = AddPackageObjectIdentifiers(objects);

		if (path->Length)
		{
			path = String::Concat(path, S"/", objects);
		}
		else
		{
			path = objects;
		}
	}

	return path;
}


String* MOG_ControllerPackage::CombinePackageGroups(String* group1, String* group2)
{
	if (String::IsNullOrEmpty(group2))
	{
		return group1;
	}
	else if (String::IsNullOrEmpty(group1))
	{
		return group2;
	}
	else
	{
		return String::Concat(group1, S"/", group2);
	}
}

String* MOG_ControllerPackage::CombinePackageObjects(String* object1, String* object2)
{
	if (String::IsNullOrEmpty(object2))
	{
		return object1;
	}
	else if (String::IsNullOrEmpty(object1))
	{
		return object2;
	}
	else
	{
		return String::Concat(object1, S"/", object2);
	}
}

String *MOG_ControllerPackage::CombinePackageAssignment(String *packageName, String *packageGroups, String *packageObjects)
{
	// First add the packageName
	String *assignedPackage = packageName;

	// Check if there was a packageGroups specified?
	if (!String::IsNullOrEmpty(packageGroups))
	{
		// Check if we already have something in our mPropertyKey?
		if (assignedPackage->Length)
		{
			// Add a delimiter
			assignedPackage = String::Concat(assignedPackage, S"/");
		}
		// Add on the specified packageGroups
		assignedPackage = String::Concat(assignedPackage, packageGroups);
	}

	// Check if there was a packageObjects specified?
	if (!String::IsNullOrEmpty(packageObjects))
	{
		// Make sure to add the PackageObject identifiers
		packageObjects = MOG_ControllerPackage::AddPackageObjectIdentifiers(packageObjects);

		// Check if we already have something in our mPropertyKey?
		if (assignedPackage->Length)
		{
			// Add a delimiter
			assignedPackage = String::Concat(assignedPackage, S"/");
		}
		// Add on the specified packageObjects
		assignedPackage = String::Concat(assignedPackage, packageObjects);
	}

	return assignedPackage;
}


bool MOG_ControllerPackage::RemoveGroup(MOG_Filename *assetFilename, String *packageGroup)
{
	bool success = false;

	// Always obtain the latest revision of the specified asset in the MOG Repository
	String *currentRevision = MOG_ControllerProject::GetAssetCurrentVersionTimeStamp(assetFilename);
	MOG_Filename *blessedAssetFilename = MOG_ControllerRepository::GetAssetBlessedVersionPath(assetFilename, currentRevision);
	// Make sure that this is a blessed asset
	if (blessedAssetFilename->IsBlessed())
	{
		// Open the asset...use a try/catch/finally so we can prematurely return and still close the asset controller properly
		MOG_ControllerAsset *asset = NULL;
		try
		{
			asset = MOG_ControllerAsset::OpenAsset(blessedAssetFilename);
			
			//Don't remove package groups that have assets assigned to them
			if (asset && asset->GetProperties()->IsPackage)
			{
				ArrayList* assetsInGroup = MOG_ControllerPackage::GetAssetsInPackageGroup(blessedAssetFilename, currentRevision, packageGroup);
				if (assetsInGroup && assetsInGroup->Count)
				{
					//We can't very well remove a group that still has assets assigned to it
					MOG_Prompt::PromptMessage(	S"Unable to Remove Package Group", 
												String::Concat(	S"There are assets currently assigned to this package group.\n",
																S"Only empty groups can be removed from a package."),
												Environment::StackTrace, MOG_ALERT_LEVEL::ALERT);
					success = false;
				}
				else
				{
					//Remove this group from the dang old package
					String* user = MOG_ControllerProject::GetUserName();
					success = MOG_DBPackageAPI::RemovePackageGroupName(blessedAssetFilename, currentRevision, packageGroup, user);
				}
			}
		}
		__finally
		{
			if (asset)
				asset->Close();
		}
	}

	return success;
}


String *MOG_ControllerPackage::MapPackageAssetNameToFile(MOG_Filename *packageName)
{
	String *packageFile = "";

// JohnRen - Well, this would be nicer but we should do it the slow way just in case there is a package in the inbox that should take precedence
//	// Check if we contain a revision timestamp?
//	if (packageName->GetVersionTimeStamp()->Length)
//	{
//		// Get the properties of this package
//		MOG_Properties *packageProperties = new MOG_Properties(packageName);
//		String *files[] = MOG_ControllerAsset::GetAssetImportedFiles(packageProperties);
//		if (files && files->Count > 0)
//		{
//			packageFile = files[0];
//		}
//	}
//
//	// Check if we are still looking?
//	if (packageFile->Length == 0)
//	{
		// Get the file for this packageAssetName
		ArrayList *packageFiles = MOG_ControllerProject::MapAssetNameToFilename(packageName, packageName->GetAssetPlatform());
		if (packageFiles->Count)
		{
			// Always assume the first one is the correct match
			packageFile = __try_cast<String *>(packageFiles->Item[0]);
			if (packageFiles->Count > 1)
			{
				// Make sure to inform the Server about this error
				String *message = String::Concat(	S"MapPackageAssetNameToFile() returned more than one file.  The system cannot currently support Package Assets that contain more than a single Package File.\n",
													S"PACKAGE: ", packageName->GetAssetFullName(), S"\n",
													S"Please reBless this asset with only 1 contained file.");
				MOG_Report::ReportMessage(S"MapPackageAssetNameToFile", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
			}
		}
//	}

	// Cleanup any extra backslashes
	packageFile = packageFile->Trim(S"\\"->ToCharArray());

	// Check if we failed to obtain a valid PackageFile?
	if (packageFile->Length == 0)
	{
		// Make sure to inform the Server about this error
		String *message = String::Concat(	S"The asset contains a package assignment to a missing package.\n",
											S"Package: ", packageName->GetAssetFullName(), S"\n\n",
											S"It is recommended that this package assignment be removed.");
		MOG_Report::ReportMessage(S"MapPackageAssetNameToFile", message, Environment::StackTrace, MOG_ALERT_LEVEL::ALERT);
	}

	return packageFile;
}

ArrayList* MOG_ControllerPackage::GetAssociatedAssetsForPackage(MOG_Filename* packageFilename)
{
	ArrayList* associatedAssets = new ArrayList();

	// Check for any current assets in the project that contain a package relationship...
	// This is the better way to get the contained assets in the event this platform-specific package was created much later
	// Get any assets with a relationship to this package

	// Scan for any assets with a package assigned to the root of this package?
	ArrayList* properties = new ArrayList();
	properties->Add(MOG_PropertyFactory::MOG_Relationships::New_PackageAssignment(packageFilename->GetAssetFullName(), "", "", false));
	associatedAssets->AddRange(MOG_DBAssetAPI::GetAllAssetsByClassificationAndProperties(MOG_ControllerProject::GetProjectName(), properties, true));

	// Scan for any assets with a package assignment to a subgroup within the package?
	properties->Clear();
	properties->Add(MOG_PropertyFactory::MOG_Relationships::New_PackageAssignment(packageFilename->GetAssetFullName(), "*", "", false));
	associatedAssets->AddRange(MOG_DBAssetAPI::GetAllAssetsByClassificationAndProperties(MOG_ControllerProject::GetProjectName(), properties, true));
	// Check if this package is platform specific?
	if (packageFilename->IsPlatformSpecific())
	{
		MOG_Filename* genericPackageFilename = MOG_Filename::CreateAssetName(packageFilename->GetAssetClassification(), S"All", packageFilename->GetAssetLabel());

		// Scan for any assets with a package assigned to the root of this package?
		properties->Clear();
		properties->Add(MOG_PropertyFactory::MOG_Relationships::New_PackageAssignment(genericPackageFilename->GetAssetFullName(), "", "", false));
		associatedAssets->AddRange(MOG_DBAssetAPI::GetAllAssetsByClassificationAndProperties(MOG_ControllerProject::GetProjectName(), properties, true));

		// Scan for any assets with a package assignment to a subgroup within the platform-generic package?
		properties->Clear();
		properties->Add(MOG_PropertyFactory::MOG_Relationships::New_PackageAssignment(genericPackageFilename->GetAssetFullName(), "*", "", false));
		associatedAssets->AddRange(MOG_DBAssetAPI::GetAllAssetsByClassificationAndProperties(MOG_ControllerProject::GetProjectName(), properties, true));
	}

	return associatedAssets;
}

ArrayList *MOG_ControllerPackage::GetAssetsInPackage( MOG_Filename *blessedPackageFilename )
{
	String *packageVersion = blessedPackageFilename->GetVersionTimeStamp();
	return MOG::DATABASE::MOG_DBPackageAPI::GetAllAssetsInPackage( blessedPackageFilename, packageVersion );
}


ArrayList *MOG_ControllerPackage::GetPackageSubGroups( MOG_Filename *packageName, String *packageVersion )
{
	return MOG::DATABASE::MOG_DBPackageAPI::GetAllPackageGroups( packageName, packageVersion );
}


ArrayList *MOG_ControllerPackage::GetAssetsInPackageGroup( MOG_Filename *packageName, String *packageVersion, String *packageGroupName)
{
	// If user provides empty or NULL string, populate our packageVersion
	if( !packageVersion || packageVersion->Length < 1 )
		packageVersion = packageName->GetVersionTimeStamp();

	return MOG::DATABASE::MOG_DBPackageAPI::GetAllAssetsInPackageGroup( packageName, packageVersion, packageGroupName);
}


String *MOG_ControllerPackage::GetLatestPackageRevision_IncludingUnpostedRevisions(MOG_Filename *packageAssetFilename, String *jobLabel)
{
	// Start with the current version of the asset
	String *revision = MOG_ControllerProject::GetAssetCurrentVersionTimeStamp(packageAssetFilename);

	// Check the pending post commands for an unposted revision of this package before assuming the current revision from the project.
	ArrayList *posts = MOG_DBPostCommandAPI::GetScheduledPosts(packageAssetFilename->GetAssetFullName(), S"", MOG_ControllerProject::GetBranchName());
	if (posts && posts->Count)
	{
		// Scan all the posts for our package?
		for (int i = 0; i < posts->Count; i++)
		{
			// Check if this post matches our packageName?
			MOG_DBPostInfo *post = __try_cast<MOG_DBPostInfo *>(posts->Item[i]);

			// Check if this post's version timestamp is newer?
			if (String::Compare(post->mAssetVersion, revision, true) > 0)
			{
				// Use this newer timestamp
				revision = post->mAssetVersion;
			}
		}
	}

	// Return the newest timestamp
	return revision;
}



