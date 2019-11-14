//--------------------------------------------------------------------------------
//	MOG_ControllerAsset.cpp
//	
//	
//--------------------------------------------------------------------------------

#include "StdAfx.h"

#include "MOG_Define.h"
#include "MOG_StringUtils.h"
#include "MOG_DosUtils.h"
#include "MOG_ControllerPackage.h"
#include "MOG_ControllerRepository.h"
#include "MOG_ControllerInbox.h"
#include "MOG_ControllerProject.h"
#include "MOG_ControllerSystem.h"
#include "MOG_Tokens.h"
#include "MOG_DBAssetAPI.h"
#include "MOG_DBInboxAPI.h"
#include "FilePattern.h"

#include "MOG_ControllerAsset.h"



public __gc class MOG_AssetCacheObject
{
public:
	Object *mThread;
	MOG_ControllerAsset *mAssetController;
	int mReferenceCount;
};



MOG_ControllerAsset::MOG_ControllerAsset()
{
	Initialize();
}


MOG_ControllerAsset::~MOG_ControllerAsset(void)
{
	// Check if this asset was left opened?
	if (mOpened)
	{
		// Notify the user of our concerns about not manually closing an asset controller
		String *message = String::Concat(   S"The programmer has failed to close an asset controller properly which should be reported as a serious bug.\n",
											S"ASSET: ", mAssetFilename->GetAssetFullName(), S"\n",
											S"There can be a number of serious problems occur before the system can perform a delayed close of an asset in its destructor.");
		MOG_Report::ReportMessage(S"Copy", message, String::Concat( Environment::StackTrace, S"\r\n\r\n\tOpening StackTrace: \r\n", mOpenedStackTrace), MOG_ALERT_LEVEL::CRITICAL);

		// Make sure we close out asset
		Close();
	}
}


void MOG_ControllerAsset::Initialize(void)
{
	// Initialize the current system
	mOpened = false;
	mAssetFilename = new MOG_Filename();
	mProperties = NULL;
	mSubstituteProperties= NULL;
	mPendingViewUpdate = NULL;
}


// I want to phase this function out and use the other Import instead!!!
MOG_Filename *MOG_ControllerAsset::CreateAsset(String *sourceFullName, String *targetName, bool bMoveFiles)
{
	MOG_Filename *assetFilename = new MOG_Filename(targetName);
	ArrayList *importItems = new ArrayList();
	importItems->Add(sourceFullName);

	return CreateAsset(assetFilename->GetAssetFullName(), S"", importItems, NULL, NULL, bMoveFiles, bMoveFiles, true);
}

MOG_Filename *MOG_ControllerAsset::CreateAsset(MOG_Filename *assetFilename, String *assetRootPath, ArrayList *importItems, ArrayList *logFiles, ArrayList *properties, bool bMoveImportFiles, bool bMoveLogFiles)
{
	bool bAutoDetectMissingImportFiles = (importItems && importItems->Count > 1) ? false : true;
	return CreateAsset(assetFilename->GetEncodedFilename(), assetRootPath, importItems, logFiles, properties, bMoveImportFiles, bMoveLogFiles, bAutoDetectMissingImportFiles);
}

MOG_Filename *MOG_ControllerAsset::CreateAsset(MOG_Filename *assetFilename, String *assetRootPath, ArrayList *importItems, ArrayList *logFiles, ArrayList *properties, bool bMoveImportFiles, bool bMoveLogFiles, bool bAutoDetectMissingImportFiles)
{
	return CreateAsset(assetFilename->GetEncodedFilename(), assetRootPath, importItems, logFiles, properties, bMoveImportFiles, bMoveLogFiles, bAutoDetectMissingImportFiles);
}

MOG_Filename *MOG_ControllerAsset::CreateAsset(String *assetName, String *assetRootPath, ArrayList *importItems, ArrayList *logFiles, ArrayList *properties, bool bMoveImportFiles, bool bMoveLogFiles)
{
	bool bAutoDetectMissingImportFiles = (importItems && importItems->Count > 1) ? false : true;
	return CreateAsset(assetName, assetRootPath, importItems, logFiles, properties, bMoveImportFiles, bMoveLogFiles, bAutoDetectMissingImportFiles);
}

MOG_Filename *MOG_ControllerAsset::CreateAsset(String *assetName, String *assetRootPath, ArrayList *importItems, ArrayList *logFiles, ArrayList *properties, bool bMoveImportFiles, bool bMoveLogFiles, bool bAutoDetectMissingImportFiles)
{
	MOG_ASSERT_THROW(assetName && assetName->Length, MOG_Exception::MOG_EXCEPTION_InvalidAssetName, "MOG_ControllerAsset::Create - Requires a valid AssetName");

	// Construct an AssetFilename using the specified assetName and making sure it contains the adam classification in its name?
	MOG_Filename *assetFilename = MOG_Filename::AppendAdamObjectNameOnAssetName(new MOG_Filename(assetName));
	MOG_ASSERT_THROW(assetFilename->GetAssetFullName()->Length > 0, MOG_Exception::MOG_EXCEPTION_InvalidAssetName, "MOG_ControllerAsset::Create - assetFilename->GetAssetFullName()->Length <= 0 - AssetName is not recognized as a valid asset name");
	MOG_ASSERT_THROW(assetFilename->GetFilenameType() == MOG_FILENAME_Asset, MOG_Exception::MOG_EXCEPTION_InvalidAssetName, "MOG_ControllerAsset::Create - AssetName is not recognized as a valid asset name");
	MOG_ASSERT_THROW(MOG_ControllerProject::IsValidPlatform(assetFilename->GetAssetPlatform()), MOG_Exception::MOG_EXCEPTION_InvalidAssetPlatform, "MOG_ControllerAsset::Create - AssetName contains an invalid platform");

	MOG_ControllerAsset *asset = NULL;
	MOG_Filename *finishedAssetFilename = NULL;

	try
	{
		bool failed = false;

		// Create the newAssetFilename from this generated path of the specified assetName
		String *assetPath = CreateAsset_BuildAssetPath(assetFilename);
		MOG_Filename *newAssetFilename = new MOG_Filename(assetPath);

		// Clear the way for this new asset
		if (!CreateAsset_ResolveAlreadyExistingAsset(newAssetFilename))
		{
			// Bail because somthing bad happened while clearing the way for this asset
			return NULL;
		}

		// Create a new MOG_ControllerAsset
		asset = new MOG_ControllerAsset();
		// Set the new asset's name within the user's inbox
		asset->GetAssetFilename()->SetFilename(newAssetFilename);
		MOG_ASSERT_THROW(asset->GetAssetFilename()->GetFilenameType() == MOG_FILENAME_TYPE::MOG_FILENAME_Asset, MOG_Exception::MOG_EXCEPTION_InvalidAssetName, String::Concat(S"MOG_ControllerAsset::Create - '", assetPath, S"' is not recognized as a valid asset name"));

		// Fetch the properties for this asset
		if (asset->FetchFromPreviousAsset(importItems))
		{
			// Validate this asset's root path for the list of importItems
			if (!CreateAsset_IsAssetRootPathValid(assetRootPath, importItems))
			{
				// Bail because there was a problem with the list of importItems
				return NULL;
			}

			// Push in the assetRootPath for this asset now because it could be used/modified in routines below
			asset->GetProperties()->SourcePath = assetRootPath;

			// Resolve the specified importItems into importFiles
			ArrayList *importFiles = CreateAsset_ResolveImportItems(asset, importItems);
			if (!importFiles)
			{
				// Bail because there was a problem with the list of importItems
				return NULL;
			}

			// Perform the Filter verification on this Asset
			if (!CreateAsset_VerifyClassificationFilterCheck(asset, importFiles))
			{
				// Bail because the filters were violated
				return NULL;
			}

			// Set the important properties associated with importing an asset
			CreateAsset_SetImportProperties(asset);

			// Check if we should attempt to auto detect any missing import files?
			if (bAutoDetectMissingImportFiles)
			{
				// Process any specified import files
				if (!CreateAsset_DetectMissingImportFiles(asset, importFiles))
				{
					failed = true;
				}
			}

			// Validate the windows path for all the importFiles
			for (int f = 0; f < importFiles->Count; f++)
			{
				String *importFile = __try_cast<String *>(importFiles->Item[f]);

				// Make sure we validate the windows path for this file?
				assetFilename->ValidateWindowsPath(importFile);
			}

			// Process any specified import files
			if (!CreateAsset_ProcessImportFiles(asset, importFiles, bMoveImportFiles))
			{
				failed = true;
			}

			// Process any specified log files
			if (!CreateAsset_ProcessLogFiles(asset, logFiles, bMoveLogFiles))
			{
				failed = true;
			}

			// Check if no sourcePath was specified thus far?
			if (asset->GetProperties()->SourcePath->Length == 0)
			{
				// Make sure we build one from the list of specified items
				asset->GetProperties()->SourcePath = GetCommonDirectoryPath("", importFiles);
			}

			// Apply any properties that were specified in the API
			asset->GetProperties()->SetProperties(properties);

			// Check if this asset is getting created out in the repository or in a local workspace?
			if (asset->GetAssetFilename()->IsWithinRepository() ||
				asset->GetAssetFilename()->IsLocal())
			{
				// Perform the auto population of an asset's platform specific directories
				CreateAsset_AutoPopulatePlaformSpecificFiles(asset, importFiles);
			}
		}
		else
		{
			// Inform the user we failed to obtain the properties of the asset
			String *message = String::Concat(	S"MOG aborted the creation of the asset because it failed to obtain valid properties for the asset.\n",
												S"ASSET: ", asset->GetAssetFilename()->GetAssetFullName());
			MOG_Prompt::PromptMessage(S"CreateAsset", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
			failed = true;
		}

		// Set the asset sizes for this asset
		if (!MOG_ControllerAsset::SetAssetSizes(asset))
		{
			// Inform user of the importation error
			String *message = String::Concat(	S"MOG failed to set the size of this asset.\n",
												S"ASSET: ", asset->GetAssetFilename()->GetAssetFullName());
			MOG_Prompt::PromptMessage(S"CreateAsset", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
		}

		// Set the final status for this asset
		CreateAsset_SetFinalStatus(failed, asset);

		// Set the asset's default group
		SetDefaultGroup(asset);

		// Check if we failed?
		if (failed)
		{
			return NULL;
		}

		// Check if this asset is being imported directly into the repository?
		if (asset->GetAssetFilename()->IsWithinRepository())
		{
			// Stamp this asset directly into the repository
			CreateAsset_StampRepositoryAsset(asset);
		}

		// Clone a copy of our new asset's name so we can close our asset before we return
		finishedAssetFilename = new MOG_Filename(asset->GetAssetFilename());
	}
	catch (MOG_Exception *e)
	{
		MOG_Report::ReportMessage("Create Asset Failed", e->Message, "", MOG::PROMPT::MOG_ALERT_LEVEL::ERROR);
	}
	catch (Exception *e)
	{
		MOG_Report::ReportMessage("Create Asset Failed", e->Message, e->StackTrace, MOG::PROMPT::MOG_ALERT_LEVEL::CRITICAL);
	}
	__finally
	{
		// Check if we actually created an asset?
		if (asset)
		{
			// Always close the asset when we have finished its creation
			asset->Close();
		}
	}

	// Return the new name of the asset we just created
	return finishedAssetFilename;
} // End CreateAsset()


bool MOG_ControllerAsset::CreateAsset_IsAssetRootPathValid(String *assetRootPath, ArrayList *importItems)
{
	// Check if there is a user specified assetRootPath?
	if (assetRootPath->Length > 0)
	{
		// Loop throught the importItems and validate them with the specified assetRootPath
		for(int i = 0; i < importItems->Count; i++)
		{
			String *importItem = dynamic_cast<String *>(importItems->Item[i]);

			if (importItem->StartsWith(assetRootPath, StringComparison::CurrentCultureIgnoreCase) ||
				assetRootPath->StartsWith(importItem, StringComparison::CurrentCultureIgnoreCase))
			{
				// All is well with this importItem
			}
			else
			{
				return false;
			}
		}
	}

	return true;
}

ArrayList *MOG_ControllerAsset::CreateAsset_ResolveImportItems(MOG_ControllerAsset *asset, ArrayList *importItems)
{
	ArrayList *importFiles = new ArrayList();

	// Check if there were any import items specified?
	if (importItems)
	{
		// Make sure all the specified files exist?
		for (int f = 0; f < importItems->Count; f++)
		{
			String *importItem = __try_cast<String *>(importItems->Item[f]);

			// Make sure the file exists
			if (DosUtils::FileExistFast(importItem))
			{
				// Add the file to our list of importFiles
				importFiles->Add(importItem);
			}
			// Check if this item is a directory?
			else if (DosUtils::DirectoryExistFast(importItem))
			{
				// Check if we are still looking to resolve our SourcePath? or
				if (asset->GetProperties()->SourcePath->Length == 0)
				{
					// Take the path of this directory
					asset->GetProperties()->SourcePath = DosUtils::PathGetDirectoryPath(importItem);
				}

				// Check if the asset's SourcePath is more specific than this importItem?
				if (asset->GetProperties()->SourcePath->Length > importItem->Length)
				{
					// Ignore the original importItem and respect the more specific SourcePath so that files outside of this directory will be ignored
					importFiles->AddRange(DosUtils::FileGetRecursiveList(asset->GetProperties()->SourcePath, S"*.*"));
				}
				else
				{
					// Add all of the contained files to our list of importItem
					importFiles->AddRange(DosUtils::FileGetRecursiveList(importItem, S"*.*"));
				}
			}
			else
			{
				// Indicate we failed to create a new asset from the specified files
				String *message = String::Concat(	S"MOG failed to import the new asset because one of the import items is missing.\n",
													S"MISSING: ", importItem);
				MOG_Prompt::PromptMessage(S"CreateAsset", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
				return NULL;
			}
		}
	}

	return importFiles;
}


String *MOG_ControllerAsset::CreateAsset_BuildAssetPath(MOG_Filename *assetFilename)
{
	String *assetPath = "";

	// Check if this asset is getting created out in the repository or in a local workspace?
	if (assetFilename->IsWithinRepository() ||
		assetFilename->IsLocal())
	{
		// Make sure the asset id is created in the database
		MOG_DBAssetAPI::CreateAssetName(assetFilename, MOG_ControllerProject::GetUserName_DefaultAdmin());

		// Use the path specified in assetFilename
		assetPath = assetFilename->GetEncodedFilename();
	}
	else
	{
		// Try to use the user's path specified in assetFilename?
		String *userPath = assetFilename->GetUserPath();
		if (userPath->Length == 0)
		{
			// Looks like we are missing this information...Try to populate it from the system's information
			userPath = MOG_ControllerProject::GetUser()->GetUserPath();
		}

		// Make sure we established a valid userPath?
		if (userPath->Length)
		{
			// Establish the path to the user's Drafts
			assetPath = assetFilename->GetAssetEncodedInboxPath(S"Drafts");
		}
		else
		{
			MOG_ASSERT_THROW(assetFilename->GetFilenameType() == MOG_FILENAME_Asset, MOG_Exception::MOG_EXCEPTION_InvalidAssetName, "MOG_ControllerAsset::Create - System was unable to determin where the asset should be created");
		}
	}

	return assetPath;
}


bool MOG_ControllerAsset::CreateAsset_ResolveAlreadyExistingAsset(MOG_Filename *assetFilename)
{
	// Check if this asset is getting created out in the repository or in a local workspace?
	if (assetFilename->IsWithinRepository() ||
		assetFilename->IsLocal())
	{
		// Nothing we can do about this situation
	}
	else
	{
		// Check if there is already an asset in our way?
		if (DosUtils::DirectoryExistFast(assetFilename->GetEncodedFilename()))
		{
			MOG_Filename *existingAssetFilename = new MOG_Filename(assetFilename->GetEncodedFilename());
			String *propertiesFilename = MOG_ControllerAsset::GetAssetPropertiesFilename(existingAssetFilename);

			// Open this asset's properties as the existing property set
			MOG_Ini *existingProperties = new MOG_Ini(propertiesFilename);

			// Move the old asset out of our way and into the trash
			if (MOG_ControllerInbox::MoveToTrash(existingAssetFilename))
			{
				// Make sure we leave a copy of this asset's properties file behind for the new asset being imported
				existingProperties->SetFilename(propertiesFilename, true);
				existingProperties->Save();
			}
			else
			{
				return false;
			}
		}
	}

	return true;
}


bool MOG_ControllerAsset::CreateAsset_VerifyClassificationFilterCheck(MOG_ControllerAsset *asset, ArrayList *importFiles)
{
	bool bAssetPassesCheck = true;
	MOG_Filename *assetFilename = asset->GetAssetFilename();

	// Check the classification's inclusion filter.
	if (asset->GetProperties()->FilterInclusions->Length)
	{
		FilePattern *inclusions = new FilePattern(asset->GetProperties()->FilterInclusions);
		if (inclusions->IsFilePatternMatch(assetFilename->GetAssetLabel()) == false)
		{
			// Check if this asset already exists?
			if (MOG_ControllerProject::DoesAssetExists(assetFilename))
			{
				// Continue with the import because it already exists in the project
			}
			else
			{				
				bAssetPassesCheck = false;
			}
		}

		// Are we still not passing?
		if (!bAssetPassesCheck)
		{
			for (int i = 0; i < importFiles->Count; i++)
			{
				String *importFile = __try_cast<String *>(importFiles->Item[i]);

				if (inclusions->IsFilePatternMatch(importFile) == false)
				{
					// Check if we should inform the user of the filter violation?
					if (asset->GetProperties()->PromptUserOnFilterViolation)
					{
						// Prompt user for approval on what should be done on this creation
						String *message = String::Concat(S"This asset violates the classification filter.\n",
														 S"Are you sure you want to Proceed?\n\n",
														 S"FILTER INCLUSION: ", asset->GetProperties()->FilterInclusions, S"\n",
														 S"ASSET: ", importFile, S"\n",
														 S"CLASSIFICATION: ", assetFilename->GetAssetClassification(), S"\n");
						if (MOG_Prompt::PromptResponse("Filter Violation", message, MOGPromptButtons::YesNo) == MOGPromptResult::No)
						{
							return false;
						}
					}
					else
					{
						return false;
					}
				}
				else
				{
					bAssetPassesCheck = true;
				}
			}	
		}
	}

	// Check the classification's exclusion filter.
	if (asset->GetProperties()->FilterExclusions->Length)
	{
		FilePattern *exclusions = new FilePattern(asset->GetProperties()->FilterExclusions);
		if (exclusions->IsFilePatternMatch(assetFilename->GetAssetLabel()) == true)
		{
			// Check if this asset already exists?
			if (MOG_ControllerProject::DoesAssetExists(assetFilename))
			{
				// Continue with the import because it already exists in the project
			}
			else
			{
				bAssetPassesCheck = false;				
			}
		}

		// Are we still not passing?
		if (!bAssetPassesCheck)
		{		
			for (int i = 0; i < importFiles->Count; i++)
			{
				String *importFile = __try_cast<String *>(importFiles->Item[i]);

				if (exclusions->IsFilePatternMatch(importFile) == true)
				{
					// Check if we should inform the user of the filter violation?
					if (asset->GetProperties()->PromptUserOnFilterViolation)
					{
						// Prompt user for approval on what should be done on this creation
						String *message = String::Concat(S"This asset violates the classification filter.\n",
														 S"Are you sure you want to Proceed?\n\n",
														 S"FILTER EXCLUSION: ", asset->GetProperties()->FilterExclusions, S"\n",
														 S"ASSET: ", importFile, S"\n",
														 S"CLASSIFICATION: ", assetFilename->GetAssetClassification(), S"\n");
						if (MOG_Prompt::PromptResponse("Filter Violation", message, MOGPromptButtons::YesNo) == MOGPromptResult::No)
						{
							return false;
						}
					}
					else
					{
						return false;
					}
				}
			}
		}
	}	

	return true;
}


bool MOG_ControllerAsset::CreateAsset_SetImportProperties(MOG_ControllerAsset *asset)
{
	MOG_Properties *pProperties = asset->GetProperties();
	MOG_Filename *assetFilename = asset->GetAssetFilename();

	// Set the apropriate asset's times
	String *timestamp = MOG_Time::GetVersionTimestamp();
	pProperties->CreatedTime = timestamp;
	pProperties->ModifiedTime = timestamp;
	// Check if this asset has a BlessedTime?
	if (pProperties->BlessedTime->Length)
	{
		// Graduate the BlessedTime to this asset's PreviousRevision
		pProperties->PreviousRevision = pProperties->BlessedTime;
		// Clear the asset's BlessedTime...No inbox asset should ever have a BlessedTime
		pProperties->BlessedTime = "";
	}

	// Set Owner/Creator
	pProperties->Creator = MOG_ControllerProject::GetUserName_DefaultAdmin();
	pProperties->Owner = MOG_ControllerProject::GetUserName_DefaultAdmin();

	// Set a default yet somewhat informative LastComment
	pProperties->LastComment = String::Concat(MOG_ControllerProject::GetUserName_DefaultAdmin(), S" imported on ", MOG_Time::FormatTimestamp(timestamp, S""));

	// Make sure to record the SourceMachine for this Import
	pProperties->SourceMachine = MOG_ControllerSystem::GetComputerName();

	// Update the asset's status
	pProperties->Status = MOG_AssetStatus::GetText(MOG_AssetStatusType::Importing);
	// Update the inbox views
	MOG_ControllerInbox::UpdateInboxView(MOG_AssetStatusType::Importing, NULL, assetFilename, pProperties);

	// Check if this asset automatically wants the lock?
	if (pProperties->AutoLockOnImport)
	{
		// Obtain a lock query on this asset
		MOG_Command *pLockQuery = MOG_ControllerProject::PersistentLock_Query(assetFilename->GetAssetFullName());
		if (pLockQuery->IsCompleted() &&
			pLockQuery->GetCommand())
		{
			// Check if someone else already has this asset locked?
			if (String::Compare(pLockQuery->GetCommand()->GetUserName(), MOG_ControllerProject::GetUser()->GetUserName(), true) != 0)
			{
				// Indicate that this asset is already locked
				String* message = String::Concat(	S"Asset is already locked by '", pLockQuery->GetCommand()->GetUserName(), S"'.\n",
													S"ASSET: ", assetFilename->GetAssetFullName(), S"\n\n",
													S"The asset was not automatically locked.\n");
				MOG_Prompt::PromptMessage(S"Auto Lock Failed", message);
			}
		}
		else
		{
			// Attempt to obtain the lock
			String *comment = "Automatically locked when imported.";
			MOG_Command *lockRequest = MOG_ControllerProject::PersistentLock_Request(assetFilename->GetAssetFullName(), comment);
			if (lockRequest->IsCompleted() == false)
			{
				// Indicate that this asset is already locked
				String* message = String::Concat(	S"Unable to obtain the lock.\n",
													S"ASSET: ", assetFilename->GetAssetFullName(), S"\n\n",
													S"The asset could not be automatically locked.\n");
				MOG_Prompt::PromptMessage(S"Auto Lock Failed", message);
			}
		}
	}

	return true;
}

bool MOG_ControllerAsset::ValidateAsset_IsAssetOutofdate(MOG_Filename *assetFilename)
{
	return ValidateAsset_IsAssetOutofdate(assetFilename, new MOG_Properties(assetFilename));
}

bool MOG_ControllerAsset::ValidateAsset_IsAssetOutofdate(MOG_Filename *assetFilename, MOG_Properties *properties)
{
	// Check if this is within the user's mailbox?
	if (assetFilename->IsWithinInboxes())
	{
		// Check if we should perform the out-of-date verification?
		if (properties->OutofdateVerification)
		{
			// Check if we have a PreviousRevision to compare?
			if (properties->PreviousRevision->Length)
			{
			    // Get the current revision of this asset in the repository
			    String *currentRepositoryRevision = MOG_ControllerProject::GetAssetCurrentVersionTimeStamp(assetFilename);

				// Make sure this asset's PreviousRevision is not older than the current revision for this asset
				if (String::Compare(properties->PreviousRevision, currentRepositoryRevision, true) < 0)
				{
				    return true;
			    }
		    }
	    }
	}

	return false;
}

bool MOG_ControllerAsset::CreateAsset_ProcessLogFiles(MOG_ControllerAsset *asset, ArrayList *logFiles, bool bMoveLogFiles)
{
	bool bFailed = false;
	MOG_Filename *assetFilename = asset->GetAssetFilename();

	// Check if there were any log files specified?
	if (logFiles)
	{
		// Loop through all the specified logFiles
		for (int f = 0; f < logFiles->Count; f++)
		{
			String *logFile = __try_cast<String *>(logFiles->Item[f]);

			// Make sure we validate the filename?
			assetFilename->ValidateWindowsPath(DosUtils::PathGetFileName(logFile));

			// Check to make sure the file exist?
			if (DosUtils::FileExistFast(logFile))
			{
				// Establish the desired target for this file
				String *targetFile = String::Concat(assetFilename->GetEncodedFilename(), S"\\", DosUtils::PathGetFileName(logFile));

				// Check if we need to move the files?
				if (bMoveLogFiles)
				{
					// Move the log file into this blessed asset's directory
					if (!DosUtils::FileMoveFast(logFile, targetFile, true))
					{
						// Indicate we failed to import the specified file
						String *message = String::Concat(	S"MOG failed to import the following file due to a potential file sharing violation.\n",
															S"FILE: ", logFile);
						MOG_Prompt::PromptMessage(S"CreateAsset", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
						bFailed = true;
					}
				}
				else
				{
					// Copy the log file into this blessed asset's directory
					if (!DosUtils::FileCopyFast(logFile, targetFile, true))
					{
						// Indicate we failed to import the specified file
						String *message = String::Concat(	S"MOG failed to import the following file due to a potential file sharing violation.\n",
															S"FILE: ", logFile);
						MOG_Prompt::PromptMessage(S"CreateAsset", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
						bFailed = true;
					}
				}
			}
		}
	}

	// Check if we failed?
	if (bFailed)
	{
		return false;
	}
	return true;
}


bool MOG_ControllerAsset::CreateAsset_DetectMissingImportFiles(MOG_ControllerAsset *asset, ArrayList *importFiles)
{
	bool bFailed = false;
	MOG_Filename *assetFilename = asset->GetAssetFilename();

	// Check if there were any import files specified?
	if (importFiles && importFiles->Count)
	{
		// Check if we have previously been imported?
		if (asset->GetProperties()->GetNonInheritedPropertiesIni()->SectionExist(S"Files.Imported"))
		{
			// Get the list of imported files before they get cleared (These are relative paths from the previous SourcePath)
			String *pastImportFiles[] = asset->GetProperties()->GetNonInheritedPropertiesIni()->GetSectionKeys(S"Files.Imported");
			if (pastImportFiles && pastImportFiles->Length > 1)
			{
				// Scan all of the past imported files
				for (int f = 0; f < pastImportFiles->Count; f++)
				{
					String *pastImportFile = pastImportFiles[f];

					// Scan the new list of importFiles looking for a match
					for (int i = 0; i < importFiles->Count; i++)
					{
						String *importFile = __try_cast<String *>(importFiles->Item[i]);

						// Check if this importFile ends with the pastImportFile?
						String *testFile = String::Concat(S"\\", pastImportFile);
						if (importFile->EndsWith(testFile, StringComparison::CurrentCultureIgnoreCase))
						{
							// Obtain the beginning portion of this importFile's path
							String *tempPath = importFile->Remove(importFile->Length - testFile->Length);
							// Check if this is the first match?  or
							// Check if this new tempPath is more accurate than our last match?
							if (asset->GetProperties()->SourcePath->Length == 0 ||
								tempPath->Length < asset->GetProperties()->SourcePath->Length)
							{
								// Establish this SourcePath based on the realive location of this pastImportFile
								asset->GetProperties()->SourcePath = tempPath;
							}
						}
					}
				}

				// Scan all of the past imported files
				for (int f = 0; f < pastImportFiles->Count; f++)
				{
					String *pastImportFile = pastImportFiles[f];
					String *pastImportFileLocation = String::Concat(asset->GetProperties()->SourcePath, S"\\", pastImportFile);

					// Make sure this file exists where is is supposed to?
					if (DosUtils::FileExistFast(pastImportFileLocation))
					{
						bool bAlreadyExists = false;

						// Check if this file is already listed in the importFiles?
						for (int i = 0; i < importFiles->Count; i++)
						{
							String *importFile = __try_cast<String *>(importFiles->Item[i]);
							if (String::Compare(importFile, pastImportFileLocation, true) == 0)
							{
								bAlreadyExists = true;
								break;
							}
						}

						// Check if it should be added?
						if (!bAlreadyExists)
						{
							importFiles->Add(pastImportFileLocation);
						}
					}
					else
					{
						// Notify user about missing file
						String *message = String::Concat(	S"Import failed to locate the associated file.\n",
															S"MISSING FILE: ", pastImportFile, S"\n",
															S"ASSET: ", assetFilename->GetAssetFullName(), S"\n",
															S"The integrity of this asset may be compromised.");
						MOG_Prompt::PromptMessage(S"CreateAsset Warning", message, Environment::StackTrace, MOG_ALERT_LEVEL::ALERT);
					}
				}
			}
		}
	}

	return true;
}

bool MOG_ControllerAsset::CreateAsset_ProcessImportFiles(MOG_ControllerAsset *asset, ArrayList *importFiles, bool bMoveImportFiles)
{
	bool bFailed = false;
	MOG_Filename *assetFilename = asset->GetAssetFilename();

	// Always clean our old list of 'ImportFiles' in preparation of a new list
	if (asset->GetProperties()->GetNonInheritedPropertiesIni()->SectionExist(S"Files.Imported"))
	{
		asset->GetProperties()->GetNonInheritedPropertiesIni()->EmptySection(S"Files.Imported");
	}

	asset->GetProperties()->RemoveRelationships("SourceFile");

	// Check if there were any import files specified?
	if (importFiles && importFiles->Count)
	{
		// Loop through all the specified importFiles
		for (int f = 0; f < importFiles->Count; f++)
		{
			String *importFile = __try_cast<String *>(importFiles->Item[f]);

			// Make sure we validate the filename?
			assetFilename->ValidateWindowsPath(importFile->Substring(asset->GetProperties()->SourcePath->Length));

			// Establish the desired target for this import file
			String *targetPath = String::Concat(assetFilename->GetEncodedFilename(), S"\\Files.Imported");
			String *targetFile = String::Concat(targetPath, S"\\", DosUtils::PathGetFileName(importFile));
			// Check if there was an SourcePath specified?
			// Make sure SourcePath is greater than the importFile?
			// Make sure that SourcePath matches the directory scop of the importFile?
			if (asset->GetProperties()->SourcePath->Length &&
				asset->GetProperties()->SourcePath->Length < importFile->Length &&
				String::Compare(asset->GetProperties()->SourcePath, importFile->Substring(0, asset->GetProperties()->SourcePath->Length), true) == 0)
			{
				// Change targetFile to include any subdirectories specified within the import file's path that follows the SourcePath
				targetFile = String::Concat(targetPath, S"\\", importFile->Substring(asset->GetProperties()->SourcePath->Length + 1));
			}

			// Check if we need to move the files?
			if (bMoveImportFiles)
			{
				// Move the file into this blessed asset's directory
				if (!DosUtils::FileMoveFast(importFile, targetFile, true))
				{
					// Indicate we failed to import the specified file
					String *message = String::Concat(	S"MOG failed to import the following file due to a potential file sharing violation.\n",
														S"FILE: ", importFile);
					MOG_Prompt::PromptMessage(S"CreateAsset", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
					bFailed = true;
				}
			}
			else
			{
				// Copy the file into this blessed asset's directory
				if (!DosUtils::FileCopyFast(importFile, targetFile, true))
				{
					// Indicate we failed to import the specified file
					String *message = String::Concat(	S"MOG failed to import the following file due to a potential file sharing violation.\n",
														S"FILE: ", importFile);
					MOG_Prompt::PromptMessage(S"CreateAsset", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
					bFailed = true;
				}
			}

			// Force all imported files to become read only
			DosUtils::SetAttributes(targetFile, FileAttributes::ReadOnly);
			// Save the sourceFilename string in the properties
			asset->GetProperties()->GetNonInheritedPropertiesIni()->PutSectionString("Files.Imported", targetFile->Substring(targetPath->Length + 1));
			// Add the SourceFile relationship for each importFile
			asset->GetProperties()->AddRelationship(MOG_PropertyFactory::MOG_Relationships::New_AssetSourceFile(importFile));
		}
	}

	// Check if we failed?
	if (bFailed)
	{
		return false;
	}
	return true;
}


bool MOG_ControllerAsset::CreateAsset_AutoPopulatePlaformSpecificFiles(MOG_ControllerAsset *asset, ArrayList *importFiles)
{
	// Loop through all the platforms for this project and build the platform specific files directories
	if (MOG_ControllerProject::GetProject())
	{
		String *applicablePlatforms[] = MOG_ControllerProject::GetProject()->GetPlatformNames();
		for (int p = 0; p < applicablePlatforms->Count; p++)
		{
			// Set the platform specifier in the asset's properties
			asset->GetProperties()->SetScope(applicablePlatforms[p]);
			// Check if this asset is a native data type?
			if (!asset->GetProperties()->NativeDataType)
			{
				// Check if this files directory is missing for this platform?
				String *filesDataDirectory = GetAssetFilesDirectory(asset->GetProperties(), applicablePlatforms[p], false);
				// Only bother to copy the files if the directory is missing?
				if (!DosUtils::DirectoryExistFast(filesDataDirectory))
				{
					// Detect where we can pull the platformSpecificFiles from?
					// By default we can always assume the importFiles
					ArrayList *platformFiles = importFiles;
					// Only bother if we actually have any files
					if (platformFiles)
					{
						// Check if our importFiles are comming out of a legitimate asset directory?
						String *importFile = __try_cast<String *>(importFiles->Item[0]);
						String *test = String::Concat(S"\\", DosUtils::PathGetFileName(filesDataDirectory), S"\\");
						int pos = importFile->ToLower()->IndexOf(test->ToLower());
						if (pos != -1)
						{
							// Check if we have a neighboring 'Files.' directory for this platform?
							String *platformFilesDirectory = String::Concat(importFile->Substring(0, pos), S"\\", DosUtils::PathGetFileName(filesDataDirectory));
							if (DosUtils::DirectoryExistFast(platformFilesDirectory))
							{
								// Substitute the default importFiles with the real platform specific files
								platformFiles = DosUtils::FileGetRecursiveList(platformFilesDirectory, S"*.*");
							}
						}

						// Loop through all the specified platformFiles and place them into the asset's files directories for each applicable platform
						for (int f = 0; f < platformFiles->Count; f++)
						{
							String *assetFile = __try_cast<String *>(platformFiles->Item[f]);

							// Establish the desired targets for this file
							String *sourceFile = GetAssetImportedDirectory(asset->GetProperties());
							// Establish the desired target for this import file
							String *targetFile = filesDataDirectory;
							// Check if there was an SourcePath specified?
							// Make sure SourcePath is greater than the assetFile?
							// Make sure that SourcePath matches the directory scope of the assetFile?
							if (asset->GetProperties()->SourcePath->Length &&
								asset->GetProperties()->SourcePath->Length < assetFile->Length &&
								String::Compare(asset->GetProperties()->SourcePath, assetFile->Substring(0, asset->GetProperties()->SourcePath->Length), true) == 0)
							{
								// Include any subdirectories specified within the import file's path that follows the SourcePath
								sourceFile = String::Concat(sourceFile, S"\\", assetFile->Substring(asset->GetProperties()->SourcePath->Length + 1));
								targetFile = String::Concat(targetFile, S"\\", assetFile->Substring(asset->GetProperties()->SourcePath->Length + 1));
							}
							else
							{
								// Append the name of the import file onto the targetFile
								sourceFile = String::Concat(sourceFile, S"\\", DosUtils::PathGetFileName(assetFile));
								targetFile = String::Concat(targetFile, S"\\", DosUtils::PathGetFileName(assetFile));
							}

							// Check to make sure the file exist?
							if (DosUtils::FileExistFast(sourceFile))
							{
								// Copy the log file into this blessed asset's directory
								DosUtils::CopyFast(sourceFile, targetFile, true);
							}

							// Save the sourceFilename string in the properties file
							asset->GetProperties()->GetNonInheritedPropertiesIni()->PutSectionString(DosUtils::PathGetFileName(filesDataDirectory), DosUtils::PathGetFileName(targetFile));
						}
					}
				}
			}
		}
	}
	else
	{
		MOG_Report::ReportMessage(S"Create Asset Error", S"There is no current project.  Unable to create asset.", Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
	}

	// Restore us back to a default scope
	asset->GetProperties()->SetScope(S"");

	return true;
}


String *MOG_ControllerAsset::CreateAsset_SetFinalStatus(bool bFailed, MOG_ControllerAsset *asset)
{
	String *finalStatus = MOG_AssetStatus::GetText(MOG_AssetStatusType::None);
	MOG_Filename *assetFilename = asset->GetAssetFilename();
	MOG_Properties *properties = asset->GetProperties();

	// Check if the import failed?
	if (bFailed)
	{
		// Check if this asset is getting created in the inboxes?
		if (assetFilename->IsWithinInboxes())
		{
			// Set the status to 'ImportError'
			finalStatus = MOG_AssetStatus::GetText(MOG_AssetStatusType::ImportError);
		}
		// Check if this asset is getting created out in the repository or in a local workspace?
		if (assetFilename->IsWithinRepository() ||
			assetFilename->IsLocal())
		{
			// Set the status to 'CreationError'
			finalStatus = MOG_AssetStatus::GetText(MOG_AssetStatusType::CreationError);
		}
	}
	// Check if this asset is getting created in the inboxes?
	else if (assetFilename)
	{
		// Check if this asset still thinks it is being imported?
		if (String::Compare(properties->Status, MOG_AssetStatus::GetText(MOG_AssetStatusType::Importing), true) == 0)
		{
			// Check if this is a native asset?
			if (properties->NativeDataType)
			{
				// Set the final status to Processed
				finalStatus = MOG_AssetStatus::GetText(MOG_AssetStatusType::Processed);
			}
			else
			{
				// Set the final status to Unprocessed
				finalStatus = MOG_AssetStatus::GetText(MOG_AssetStatusType::Unprocessed);
			}
		}
		else
		{
			// This means the asset status was changed by the properties passed into CreateAsset()
			// Therefore, get the final status from the asset
			finalStatus = properties->Status;
		}
	}
	// Check if this asset is out in the repository?
	else if (assetFilename)
	{
		// Set the final status to Created
		finalStatus = MOG_AssetStatus::GetText(MOG_AssetStatusType::Created);
	}
	// Check if this asset is in our local workspace?
	else if (assetFilename)
	{
		// Set the final status to the predetermined assetFinalStatus
		finalStatus = MOG_AssetStatus::GetText(MOG_AssetStatusType::Created);
	}

	// Set the final status of the asset
	MOG_ControllerInbox::UpdateInboxView(asset, finalStatus);

	return finalStatus;
}


bool MOG_ControllerAsset::ReimportAssetUsingLocalFiles(MOG_Filename *inboxAssetFilename, String* localWorkspaceDirectory)
{
	ArrayList* localFiles = new ArrayList();

	// Make sure we have a valid set of properties?
	MOG_Properties* properties = new MOG_Properties(inboxAssetFilename);
	if (properties)
	{
		// Make sure this is a native asset
		if (properties->NativeDataType)
		{
			// Check if this asset has it's files synced?
			if (properties->SyncFiles)
			{
				// Obtain the list of files from the asset
				String *assetFiles[] = GetAssetPlatformFiles(properties, "", false);
				if (assetFiles)
				{
					// Obtain the list contained files in this asset
					String *relativePath = MOG_Tokens::GetFormattedString(properties->SyncTargetPath, inboxAssetFilename, properties->GetApplicableProperties())->Trim(S"\\"->ToCharArray());

					// Build list of local files
					for(int i = 0; i < assetFiles->Count; i++)
					{
						String* assetFile = assetFiles[i];
						String* relativeFile = Path::Combine(relativePath, assetFile);
						String* localFile = Path::Combine(localWorkspaceDirectory, relativeFile);

						localFiles->Add(localFile);
					}

					// Reimport the asset using the local files
					if (MOG_ControllerAsset::CreateAsset(inboxAssetFilename, "", localFiles, NULL, NULL, false, false))
					{
						return true;
					}
				}
			}
			else
			{
				// Inform the user we can't reimport assets that don't sync their files
			}
		}
		else
		{
			// Warn the user we can't reimport non-native assets
		}
	}

	return false;
}


String *MOG_ControllerAsset::SetDefaultGroup(MOG_ControllerAsset *asset)
{
	String *defaultGroup = "";

	if (asset)
	{
		MOG_Properties *pProperties = asset->GetProperties();

		// Check if this is a packaged asset?
		if (pProperties->IsPackagedAsset)
		{
			// Obtain the package of this asset
			ArrayList *packages = pProperties->GetPackages();
			if (packages && packages->Count)
			{
				// Walk through all the listed packages
				for (int packageIndex = 0; packageIndex < packages->Count; packageIndex++)
				{
					MOG_Property *packageAssignmentProperty = __try_cast<MOG_Property*>(packages->Item[packageIndex]);

					// Get the packageName in this package assignment
					String *packageName = MOG_ControllerPackage::GetPackageName(packageAssignmentProperty->mPropertyKey);
					MOG_Filename *PackageAssetFilename = new MOG_Filename(packageName);
					String *PackageLabel = PackageAssetFilename->GetAssetLabel();
					// Check if this is going to be apended?
					if (defaultGroup->Length)
					{
						defaultGroup = String::Concat(defaultGroup, S", ");
					}
					// Append the package name
					defaultGroup = String::Concat(defaultGroup, PackageLabel);
				}
			}
		}
		else if (pProperties->SyncFiles)
		{
			defaultGroup = MOG_Tokens::GetFormattedString(pProperties->SyncTargetPath, asset->GetAssetFilename(), pProperties->GetApplicableProperties());
		}

		// Assign the default group
		pProperties->Group = defaultGroup;
	}

	return defaultGroup;
}


bool MOG_ControllerAsset::CreateAsset_StampRepositoryAsset(MOG_ControllerAsset *asset)
{
	// Make sure we populate this asset's BlessedTime
	asset->GetProperties()->BlessedTime = asset->GetAssetFilename()->GetVersionTimeStamp();

	// Add this asset to the database now that it exists in the MOG Repository
	if (MOG_DBAssetAPI::AddAsset(asset->GetAssetFilename(), asset->GetProperties()->Creator, S""))
	{
		// Add this asset to the repository
		if (MOG_ControllerRepository::AddAssetRevisionInfo(asset, true))
		{
			return true;
		}
		else
		{
			String *message = String::Concat(	S"The System failed to add new revision of ", asset->GetAssetFilename()->GetAssetFullName(), S" in the MOG Repository.\n",
												S"The Asset was not created correctly.");
			MOG_Prompt::PromptMessage(S"CreateAsset", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
		}
	}
	else
	{
		String *message = String::Concat(	S"The System failed to add ", asset->GetAssetFilename()->GetAssetFullName(), S" in the MOG Repository.\n",
											S"The Asset was not created correctly.");
		MOG_Prompt::PromptMessage(S"CreateAsset", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
	}

	return false;
}


MOG_ControllerAsset *MOG_ControllerAsset::OpenAsset(MOG_Filename *assetFilename, MOG_Properties *pSubstituteProperties)
{
	// Open the asset
	MOG_ControllerAsset *asset = MOG_ControllerAsset::OpenAsset(assetFilename);
	if (asset)
	{
		// Check if there was a substitute properties specified?
		if (pSubstituteProperties)
		{
			// Force the asset to use this substitute properties
			asset->SetSubtituteProperties(pSubstituteProperties);
		}
	}

	return asset;
}

MOG_ControllerAsset *MOG_ControllerAsset::OpenAsset(String *assetFullFilename)
{
	bool bFailed = false;
	bool bWait = false;

	while (!bFailed)
	{
		// Check if we need to wait?
		if (bWait)
		{
			// We need to wait for our turn because we found another thread already using this asset
			// Goto sleep and wait for a bit before we try again
			Thread::Sleep(100);
		}

		// Ensure exclusivity
		Monitor::Enter(S"OpeningAsset");
		__try
		{
			// Check if this asset is already in our cache?
			if (gAssetCache->Contains(assetFullFilename))
			{
				MOG_AssetCacheObject *cacheObject = dynamic_cast<MOG_AssetCacheObject *>(gAssetCache->Item[assetFullFilename]);

				// Make sure this cached asset looks valid?
				if (cacheObject->mAssetController->GetAssetFilename()->GetFilenameType() == MOG_FILENAME_TYPE::MOG_FILENAME_Asset)
				{
					// Now check to ensure we are in the same thread?
					if (cacheObject->mThread == Thread::CurrentThread)
					{
						// Increment our reference count
						cacheObject->mReferenceCount++;

						// Return our cached object
						return cacheObject->mAssetController;
					}
					else
					{
						// Indicate we need to wait
						bWait = true;
						continue;
					}
				}
				else
				{
					// Remove this invalid asset from our cache
					gAssetCache->Remove(assetFullFilename);
				}
			}

			// Create a new MOG_ControllerAsset
			MOG_ControllerAsset *asset = new MOG_ControllerAsset();
			// Initialize mAssetFilename
			if (asset->Open(assetFullFilename))
			{
				// Add the newly opened asset to our cache
				MOG_AssetCacheObject *cacheObject = new MOG_AssetCacheObject();
				cacheObject->mThread = Thread::CurrentThread;
				cacheObject->mAssetController = asset;
				cacheObject->mReferenceCount = 1;
				gAssetCache->Add(assetFullFilename, cacheObject);

				// Return the asset
				return asset;
			}
			else
			{
				bFailed = true;
			}
		}
		__finally
		{
			Monitor::Exit(S"OpeningAsset");
		}
	}

//	// WOW, This thing is just plain missing???
//	MOG_ASSERT_THROW(false, MOG_Exception::MOG_EXCEPTION_MissingData, "MOG_ControllerAsset::OpenAsset - Can't locate the specified asset");
	return NULL;
}

bool MOG_ControllerAsset::Open(String *assetFullFilename)
{
	// Initialize mAssetFilename
	mAssetFilename->SetFilename(assetFullFilename);

	// Make sure this is an asset?
	if (mAssetFilename->GetFilenameType() == MOG_FILENAME_TYPE::MOG_FILENAME_Asset)
	{
		// Does the asset exist?
		if (DosUtils::DirectoryExistFast(mAssetFilename->GetEncodedFilename()))
		{
			// Have the Asset load its properties
			if (GetProperties())
			{
				// Remember who originally opened this asset for debugging purposes
				mOpenedStackTrace = MOG_Environment_StackTrace;
				// Indicate that we opened the asset
				mOpened = true;

				return true;
			}
		}
	}

	return false;
}


bool MOG_ControllerAsset::FreeAssetLock(void)
{
//	// Release the lock on the mAssetPath
//	MOG_Command *command = new MOG_Command();
//	command->Setup_LockWriteRelease(mAssetFilename->GetFullFilename());
//	MOG_ControllerSystem::GetCommandManager()->CommandProcess(command);

	return true;
}

bool MOG_ControllerAsset::Close(void)
{
	// Ensure exclusivity
	Monitor::Enter(S"ClosingAsset");

	__try
	{
		// Check if we were really open?
		if (mOpened)
		{
			// Make sure to release our lock when the asset is closed
			FreeAssetLock();
		}

		// Check if this asset is already in our cache?
		String *assetFullFilename = mAssetFilename->GetOriginalFilename();
		if (gAssetCache->Contains(assetFullFilename))
		{
			MOG_AssetCacheObject *cacheObject = dynamic_cast<MOG_AssetCacheObject *>(gAssetCache->Item[assetFullFilename]);

			// Now check to ensure we are in the same thread?
			if (cacheObject->mThread == Thread::CurrentThread)
			{
				// Decrement our reference count
				cacheObject->mReferenceCount--;

				// Check if we still have references?
				if (cacheObject->mReferenceCount)
				{
					return true;
				}
			}
			else
			{
				// This means big trouble!!!
			}
		}

		// Check if we have already opened our properties?
		if (mProperties)
		{
			// Close our Properties
			mProperties->Close();
			mProperties = NULL;
		}

		// Clear any substitute properties
		mSubstituteProperties = NULL;

		// Check if we have a mPendingViewUpdate?
		if (mPendingViewUpdate)
		{
			// Send out the ViewUpdate command to the server now
			MOG_ControllerSystem::GetCommandManager()->SendToServer(mPendingViewUpdate);
			mPendingViewUpdate = NULL;
		}

		// Erase mAssetFilename
		mAssetFilename->SetFilename("");

		// Indicate the state of the asset is closed
		mOpened = false;

		// Now remove this asset from our cache
		gAssetCache->Remove(assetFullFilename);
	}
	__finally
	{
		Monitor::Exit(S"ClosingAsset");
	}

	return true;
}


bool MOG_ControllerAsset::FetchFromPreviousAsset(ArrayList *importItems)
{
	String *assetPropertiesFilename = GetAssetPropertiesFilename();
	MOG_Filename *foundAssetFilename = NULL;

	// Check if this asset is missing a valid properties file?
	if (!DosUtils::FileExistFast(assetPropertiesFilename))
	{
		// Looks like we will need to go find one
		String *targetPropertiesFile = assetPropertiesFilename;
		String *sourcePropertiesFile = "";

		// Check if this new asset is headed for the inbox?
		if (mAssetFilename->IsWithinInboxes())
		{
			// Locate the best matching possibility in our inbox
			MOG_Filename* possibleAssetFilename = MOG_ControllerInbox::LocateBestMatchingAsset(mAssetFilename);
			if (possibleAssetFilename)
			{
				// Make sure this properties file actually exists?
				String* possiblePropertiesFile = MOG_ControllerAsset::GetAssetPropertiesFilename(possibleAssetFilename);
				if (DosUtils::FileExistFast(possiblePropertiesFile))
				{
					sourcePropertiesFile = possiblePropertiesFile;
					foundAssetFilename = possibleAssetFilename;
				}
			}
		}
		// Check if we need to keep looking further?
		if (sourcePropertiesFile->Length == 0)
		{
			// Time to check if there is an already existing asset we can pull it from?
			String *blessedRevision = MOG_DBAssetAPI::GetAssetVersion(mAssetFilename);
			if (blessedRevision->Length)
			{
				// Establish the blessedAssetFilename
				foundAssetFilename = new MOG_Filename(MOG_ControllerRepository::GetAssetBlessedVersionPath(mAssetFilename, blessedRevision));
				sourcePropertiesFile = MOG_ControllerAsset::GetAssetPropertiesFilename(foundAssetFilename);
			}
		}

		// Did we find a possible sourceFile?
		if (sourcePropertiesFile->Length)
		{
			// Copy our found properties file into the asset
			if (DosUtils::FileCopyFast(sourcePropertiesFile, targetPropertiesFile, true))
			{
				// Fetch any tools located within this location
				String *ToolsSourceDir = String::Concat(DosUtils::PathGetDirectoryPath(sourcePropertiesFile), S"\\Tools");
				String *ToolsTargetDir = DosUtils::PathGetDirectoryPath(targetPropertiesFile);
				if (DosUtils::DirectoryExistFast(ToolsSourceDir))
				{
					// Copy over any asset's tools as well
					if (!DosUtils::DirectoryCopyFast(ToolsSourceDir, ToolsTargetDir, true))
					{
						// Warn user we failed to get all of the asset's tools
					}
				}
			}
		}
	}

	// Proceed to open the properties file
	mProperties = MOG_Properties::OpenAssetProperties(this->GetAssetFilename());
	if (mProperties)
	{
		// Check if we had to find an allready existing asset to grab this properties file from?
		if (foundAssetFilename)
		{
			// Check if this found asset's properties has a BlessedTime?
			if (mProperties->BlessedTime->Length)
			{
				// Fixup the properties based on where it came from

				// By default, use the BlessedTime of this asset as our PreviousRevision
				String* previousRevision = mProperties->BlessedTime;

				// Check if this new asset is headed for the inbox?
				if (mAssetFilename->IsWithinInboxes())
				{
					// Check if any import items were specified?
					if (importItems && importItems->Count)
					{
						// Check if this asset is being imported from a workspace?
						String* importItem = dynamic_cast<String*>(importItems->Item[0]);
						String* workspaceDirectory = MOG_ControllerSyncData::DetectWorkspaceRoot(importItem);
						if (workspaceDirectory->Length)
						{
							// Check if this foundAssetFilename came from the repository?
							if (foundAssetFilename->IsWithinRepository())
							{
								// Get the workspaceID for this local workspaceDirectory
								int workspaceID = MOG_DBSyncedDataAPI::GetSyncedLocationID(workspaceDirectory);
								if (workspaceID)
								{
									// Use the locally synced revision for this asset since this file being imported is based from that previously synced file and off the repository's revision
									previousRevision = MOG_DBSyncedDataAPI::GetLocalAssetVersion(workspaceID, foundAssetFilename);

									// Check if this file also matches the SyncTargetPath of the asset?
									String *syncTargetPath = MOG_Tokens::GetFormattedString(mProperties->SyncTargetPath, mAssetFilename, mProperties->GetPropertyList())->Trim(S"\\"->ToCharArray());;
									String *relativeFile = DosUtils::PathMakeRelativePath(workspaceDirectory, importItem);
									if (DosUtils::PathIsWithinPath(syncTargetPath, relativeFile))
									{
										// Set the Imported status property so we don't trigger the auto update local and waste time copying it back down over the top of itself
										mProperties->Status = MOG_AssetStatus::GetText(MOG_AssetStatusType::Imported);
									}
								}
							}
						}
					}
				}

				// Set the PreviousRevision for this new inbox asset
				mProperties->PreviousRevision = previousRevision;
				// Clear the asset's BlessedTime because no inbox should ever be imported with a BlessedTime
				mProperties->BlessedTime = "";
			}
		}

		// Check for possible out-of-date asset revision
		if (ValidateAsset_IsAssetOutofdate(mAssetFilename, mProperties))
		{
			// Warn the user they are working with a potentially out-of-date asset
			String *message = String::Concat(	S"This asset is out-of-date with the repository.\n",
												S"Asset: ", mAssetFilename->GetAssetFullName(), S"\n\n",
												S"It is recommended you GetLatest before continuing to work with this asset.\n");
			MOG_Prompt::PromptMessage("Out-of-date Asset", message);
		}
	}

	// Check if this new asset is headed for the inbox?
	if (mAssetFilename->IsWithinInboxes())
	{
		// Check for any inbox conflicts for this asset?
		ArrayList* conflicts = MOG_ControllerInbox::GetInboxConflictsForAsset(mAssetFilename, MOG_ControllerProject::GetUserName());
		if (conflicts->Count)
		{
			String* conflictsString = "";
			for (int i = 0; i < conflicts->Count; i++)
			{
				String *conflict = dynamic_cast<String*>(conflicts->Item[i]);
				conflictsString = String::Concat(conflictsString, conflict, S"\n");
			}
			// Warn the user about multtiple people working on this asset
			String *message = String::Concat(	S"It may not be safe to work on this asset until this other instance is blessed.\n",
												conflictsString, S"\n",
												S"It is recommended you resolve this before working on this asset.\n");
			MOG_Prompt::PromptMessage("Another inbox instance of this asset has been detected!", message);
		}
	}

	return true;
}


bool MOG_ControllerAsset::PostComment(String *comment, bool bUseRevisionHeader)
{
	// Retain this as our last comment
	GetProperties()->LastComment = comment;

	// Make sure to pretty up the comment before posting
	if (bUseRevisionHeader)
	{
		comment = FormatRevisionComment(comment);
	}
	else
	{
		comment = FormatUserComment(comment);
	}

	// Open the existing comments log
	String *comments = "";
	if (DosUtils::FileExistFast(GetAssetCommentsFilename()))
	{
		comments = DosUtils::FileRead(GetAssetCommentsFilename());
	}
	// Update the Comments log
	comments = String::Concat(comment, comments);
	DosUtils::FileWrite(GetAssetCommentsFilename(), comments);

	return true;
}


String *MOG_ControllerAsset::FormatUserComment(String *comment)
{
	String* formatedComment = "";
	DateTime time = DateTime::Now;
	String* timestamp = String::Concat(time.ToLongDateString(), S"   ", time.ToLongTimeString());

	// Clean up comment of any empty lines
	comment = comment->Trim(Environment::NewLine->ToCharArray());

    // Dress up the comment for read-ability
	formatedComment = String::Concat(formatedComment, S"          ", timestamp, Environment::NewLine);
	formatedComment = String::Concat(formatedComment, S"          ", MOG_ControllerProject::GetUserName(), Environment::NewLine);
	formatedComment = String::Concat(formatedComment, S"          ", comment, Environment::NewLine);
    formatedComment = String::Concat(formatedComment, S"          ", S"----------------------------------------------------------------------", Environment::NewLine);

	return formatedComment;
}


String *MOG_ControllerAsset::FormatRevisionComment(String *comment)
{
	String* formatedComment = "";
	DateTime time = MOG_Time::GetDateTimeFromTimeStamp(GetAssetFilename()->GetVersionTimeStamp());
	String* revisionTimestamp = String::Concat(time.ToShortDateString(), S"   ", time.ToLongTimeString());

	// Clean up comment of any empty lines
	comment = comment->Trim(Environment::NewLine->ToCharArray());

	// Dress up the comment for read-ability
    formatedComment = String::Concat(formatedComment, S"================================================================================", Environment::NewLine);
	formatedComment = String::Concat(formatedComment, S"REVISION: ", revisionTimestamp, Environment::NewLine);
	formatedComment = String::Concat(formatedComment, MOG_ControllerProject::GetUserName(), Environment::NewLine);
	formatedComment = String::Concat(formatedComment, comment, Environment::NewLine);
    formatedComment = String::Concat(formatedComment, S"--------------------------------------------------------------------------------", Environment::NewLine);

	return formatedComment;
}


MOG_Properties *MOG_ControllerAsset::GetProperties(void)
{
	// Check if we haven't already loaded our properties?
	if (mProperties == NULL)
	{
		// Make sure this asset contains it's properties file?
		if (DosUtils::FileExistFast(GetAssetPropertiesFilename()))
		{
			// Proceed to open the properties file
			mProperties = MOG_Properties::OpenAssetProperties(this->GetAssetFilename());
		}
	}

	// Check if we have a substitute properties?
	if (mSubstituteProperties)
	{
		return mSubstituteProperties;
	}

	return mProperties;
}


MOG_Properties *MOG_ControllerAsset::GetProperties(String *platformName)
{
	// Get the asset's properties
	MOG_Properties *properties = GetProperties();
	if (properties)
	{
		// Set the scope of our properties for the specified platform
		properties->SetScope(platformName);
	}

	return properties;
}


bool MOG_ControllerAsset::RecordAllAssetFiles(MOG_ControllerAsset *asset)
{
	bool recorded = true;
	bool bImportedFilesFound = false;

	// Get a list of our "Files." sections
	ArrayList *filesSections = asset->GetProperties()->GetNonInheritedPropertiesIni()->GetSections(S"Files.");

	// Go through each Files section found in filesSections
	for( int i = 0; i < filesSections->Count; ++i)
	{
		String *dirName = __try_cast<String*>(filesSections->Item[i]);
		String *dir = String::Concat(asset->GetAssetFilename()->GetEncodedFilename(), S"\\", dirName);

		// Check for required directories?
		if (String::Compare(dirName, S"Files.Imported", true) == 0)
		{
			bImportedFilesFound = true;
		}

		// Get the list of contained files
		ArrayList *files = DosUtils::FileGetRecursiveRelativeList(dir, S"*.*");
		if (files->Count)
		{
			// Clear the Section that we are about to re-populate
			asset->GetProperties()->GetNonInheritedPropertiesIni()->EmptySection(dirName);

			// Go through our files and add them to this section
			for (int f = 0; f < files->Count; f++)
			{
				String *relativeFilename = __try_cast<String *>(files->Item[f]);
				asset->GetProperties()->GetNonInheritedPropertiesIni()->PutSectionString(dirName, relativeFilename);
			}
		}
		else
		{
			// Empty 'Files.*' directories are bad!
			recorded = false;
		}
	}

	// Check if we failed to located our imported files?
	if (!bImportedFilesFound)
	{
		// Fail because no 'Files.Imported' existed...
		recorded = false;
	}

	return recorded;
}


bool MOG_ControllerAsset::RecordAllAssetFiles(MOG_Filename *assetFilename)
{
	bool recorded = true;

	// Open this asset
	MOG_ControllerAsset *asset = MOG_ControllerAsset::OpenAsset(assetFilename);
	if (asset)
	{
		// Call our other counterpart that takes an asset controller
		recorded = RecordAllAssetFiles(asset);

		// Make sure to close down our controller
		asset->Close();
	}

	return recorded;
}


bool MOG_ControllerAsset::SetAssetSizes(MOG_ControllerAsset *asset)
{
	long size = 0;
	long platformSize = 0;

	// Calculate the size of all the files within the imported files directory
	// Get the location of the asset's import files
	String *importSpecificFilesPath = GetAssetImportedDirectory(asset->GetProperties());
	ArrayList *files = DosUtils::FileGetRecursiveList(importSpecificFilesPath, S"*.*");
	if (files != NULL)
	{
		// Add all the sizes of the files
		for (int f = 0; f < files->Count; f++)
		{
			String *filename = __try_cast<String *>(files->Item[f]);
			FileInfo *fileInfo = DosUtils::FileGetInfo(filename);
			if (fileInfo)
			{
				size += (long)fileInfo->Length;
			}
		}
	}
	// Save the calculated size of all the files for the import files
	asset->GetProperties()->Size = size.ToString();

	// Calculate the size of all the files within the platform specific files directories
	// Get all the platforms in the project
	String *platformNames[] = MOG_ControllerProject::GetProject()->GetPlatformNames();
	for (int p = 0; p < platformNames->Count; p++)
	{
		// Get the platformName
		String *platformName = platformNames[p];
		// Only bother to recalculate the file sizes if this asset is not NativeDataType?
		if (!asset->GetProperties(platformName)->NativeDataType)
		{
			// Check if we need to calculate the platformSize?
			// Check if this asset is divergent?
			if (platformSize == 0 ||
				asset->GetProperties(platformName)->DivergentPlatformDataType)
			{
				platformSize = 0;
				// Get the location of the asset's platform specific files
				String *platformSpecificFilesPath = GetAssetProcessedDirectory(asset->GetProperties(), platformName);
				ArrayList *files = DosUtils::FileGetRecursiveList(platformSpecificFilesPath, S"*.*");
				if (files != NULL)
				{
					// Add all the sizes of the files
					for (int f = 0; f < files->Count; f++)
					{
						String *filename = __try_cast<String *>(files->Item[f]);
						FileInfo *fileInfo = DosUtils::FileGetInfo(filename);
						if (fileInfo)
						{
							platformSize += (long)fileInfo->Length;
						}
					}
				}

				// Use the calculated size of the platform files
				size = platformSize;
			}
		}

		// Save the calculated size of all the files for this platform using the scope explicit mode
		asset->GetProperties()->SetScopeExplicitMode(true);
		asset->GetProperties()->Size = size.ToString();
		asset->GetProperties()->SetScopeExplicitMode(false);
	}

	// Restore us back to a default scope
	asset->GetProperties()->SetScope(S"");
	asset->GetProperties()->SetScopeExplicitMode(false);

	return true;
}


bool MOG_ControllerAsset::SetAssetSizes(MOG_Filename *assetFilename)
{
	bool bCompleted = false;

	// Make sure this is an actual asset?
	if (assetFilename->GetFilenameType() == MOG_FILENAME_TYPE::MOG_FILENAME_Asset)
	{
		// Open this asset
		MOG_ControllerAsset *asset = MOG_ControllerAsset::OpenAsset(assetFilename);
		if (asset)
		{
			// Call our other counterpart that takes an asset controller
			bCompleted = SetAssetSizes(asset);

			// Make sure to close down our controller
			asset->Close();
			return bCompleted;
		}
	}

	return bCompleted;
}

bool MOG_ControllerAsset::CleanAsset()
{
	bool bDeleteLogFiles = false;
	bool bFailed = false;

	// Remove any pre-existing processed files and logs
	// Loop through all the sections...Can't auto increment because the remove will collapse the array around us
	for(int d = 0; d < GetProperties()->GetNonInheritedPropertiesIni()->CountSections(); /* d++ */)
	{
		// Loop through all the sections
		String *section = GetProperties()->GetNonInheritedPropertiesIni()->GetSectionByIndexSLOW(d);
		if (section->ToLower()->StartsWith(S"Files."->ToLower()))
		{
			String *dirName = section;

			// Make sure this isn't 'Files.Imported'? and
			if (String::Compare(dirName, S"Files.Imported", true) != 0)
			{
				// Remove this section
				GetProperties()->GetNonInheritedPropertiesIni()->RemoveSection(dirName);

				// Remove the directory
				String *dir = String::Concat(GetAssetFilename()->GetEncodedFilename(), S"\\", dirName);
				if (!DosUtils::DirectoryDeleteFast(dir))
				{
					bFailed = true;
				}

				bDeleteLogFiles = true;

				// Never increment because the RemoveSection collapsed the array around us
				continue;
			}
		}

		// Only increment when we didn't remove the section
		d++;
	}

	// Remove log files?
	if (bDeleteLogFiles)
	{
		// Get all the contained log files
		FileInfo *files[] = DosUtils::FileGetList(GetAssetFilename()->GetEncodedFilename(), S"*.Log");
		if (files != NULL)
		{
			// Delete each log file
			for (int f = 0; f < files->Count; f++)
			{
				if (!DosUtils::FileDelete(files[f]->FullName))
				{
					bFailed = true;
				}
			}
		}
	}

	if (!bFailed)
	{
		return true;
	}

	return false;
}


bool MOG_ControllerAsset::IsProcessed()
{
	// Check the asset's state to make sure it isn't unprocessed
	if (GetProperties()->IsUnprocessed)
	{
		return false;
	}

	return true;
}

String *MOG_ControllerAsset::GetAssetPlatformFilesSection(MOG_Properties *pProperties, String *platformName, bool bValidateSection)
{
	String *sectionName = S"";
	String *files[] = NULL;

	// Check if there is a platform specified?
	if (platformName->Length)
	{
		// Have the properties set the platform scope
		pProperties->SetScope(platformName);

		// Check if this asset is not a NativeDataType?
		if (!pProperties->NativeDataType)
		{
			// Check if this asset is a DivergentPlatformDataType?
			if (pProperties->DivergentPlatformDataType)
			{
				sectionName = String::Concat(S"Files.", platformName);

				// Check if we want to attempt to validate files?
				if (bValidateSection)
				{
					// Validate this sectionName by checking for listed files in the asset's properties
					if (pProperties->GetNonInheritedPropertiesIni()->SectionExist(sectionName))
					{
						files = pProperties->GetNonInheritedPropertiesIni()->GetSectionKeys(sectionName);
						if (files && files->Count)
						{
							// Return this sectionName as it has been validated with contained files
							return sectionName;
						}
					}
				}
				else
				{
					return sectionName;
				}
			}

			// We need to use the asset's platform just in case a platform specific asset is also non divergent
			sectionName = String::Concat(S"Files.", pProperties->GetAssetFilename()->GetAssetPlatform());

			// Check if we want to attempt to validate files?
			if (bValidateSection)
			{
				// Validate this sectionName by checking for listed files in the asset's properties
				if (pProperties->GetNonInheritedPropertiesIni()->SectionExist(sectionName))
				{
					files = pProperties->GetNonInheritedPropertiesIni()->GetSectionKeys(sectionName);
					if (files && files->Count)
					{
						// Return this sectionName as it has been validated with contained files
						return sectionName;
					}
				}
			}
			else
			{
				return sectionName;
			}
		}
	}

	// Let's just use the imported files and there is nothing to validate
	sectionName = S"Files.Imported";
	return sectionName;
}

String *MOG_ControllerAsset::GetAssetPlatformFiles(MOG_Properties *pProperties, String *platformName, bool bValidateSection)[]
{
	String *files[] = NULL;

	// Get the correct sectionName
	String *sectionName = GetAssetPlatformFilesSection(pProperties, platformName, bValidateSection);
	if (sectionName->Length)
	{
		// Make sure the sectionName exists
		if (pProperties->GetNonInheritedPropertiesIni()->SectionExist(sectionName))
		{
			// Obtain the list fo files out of the sectionName
			files = pProperties->GetNonInheritedPropertiesIni()->GetSectionKeys(sectionName);
		}
	}

	return files;
}

String *MOG_ControllerAsset::GetAssetFilesDirectory(MOG_Properties *pProperties, String *platformName, bool bValidateSection)
{
	// Get the correct sectionName
	String *sectionName = GetAssetPlatformFilesSection(pProperties, platformName, bValidateSection);
	// Construct the asset's files directory
	String *directory = String::Concat(pProperties->GetAssetFilename()->GetEncodedFilename(), S"\\", sectionName);
	return directory;
}

String* MOG_ControllerAsset::GetFileList(String* platformName, bool bAddSyncTargetPath)[]
{
	String *sectionName = S"";
	String *files[] = NULL;

	// is the specified platform is valid for this asset?
	if (String::Compare(mAssetFilename->GetAssetPlatform(), S"All", true) == 0 ||
		String::Compare(mAssetFilename->GetAssetPlatform(), platformName, true) == 0)
	{
		// Have the Asset load its properties
		GetProperties(platformName);

		// only go here if the asset is not a NativeDataType?
		if (!GetProperties()->NativeDataType)
		{
			// Check if this asset is a DivergentPlatformDataType?
			if (GetProperties()->DivergentPlatformDataType)
			{
				// Build the sectionName for the specified platform
				sectionName = String::Concat(S"Files.", platformName);
				if (GetProperties(platformName)->GetNonInheritedPropertiesIni()->SectionExist(sectionName))
				{
					files = GetProperties()->GetNonInheritedPropertiesIni()->GetSectionKeys(sectionName);
				}
			}

			// Check if we failed to find a platform specific list of files
			if (files == NULL)
			{
				// Build the sectionName for the asset's platform
				sectionName = "Files.All";
				if (GetProperties(platformName)->GetNonInheritedPropertiesIni()->SectionExist(sectionName))
				{
					files = GetProperties()->GetNonInheritedPropertiesIni()->GetSectionKeys(sectionName);
				}
			}
		}

		// Check if we failed to find a processed list of files
		if (files == NULL)
		{
			// Build the sectionName for the asset's imported files
			sectionName = "Files.Imported";
			if (GetProperties(platformName)->GetNonInheritedPropertiesIni()->SectionExist(sectionName))
			{
				files = GetProperties()->GetNonInheritedPropertiesIni()->GetSectionKeys(sectionName);
			}
		}

		if (bAddSyncTargetPath)
		{
			// fixup the sync target path
			String* syncTargetPath = MOG_Tokens::GetFormattedString(GetProperties()->SyncTargetPath);
			for (int i = 0; i < files->Count; i++)
			{
				files[i] = Path::Combine(syncTargetPath, files[i]);
			}
		}
	}

	return files;
}

String* MOG_ControllerAsset::GetFileList(String* platformName, String* workspace)[]
{
	String* files[] = GetFileList(platformName, true);
	if (files)
	{
		for (int i = 0; i < files->Count; i++)
		{
			files[i] = Path::Combine(workspace, files[i]);
		}
	}
	
	return files;
}


String *MOG_ControllerAsset::GetAssetPropertiesFilename()
{
	return String::Concat(mAssetFilename->GetEncodedFilename(), S"\\Properties.Info");
}

String *MOG_ControllerAsset::GetAssetPropertiesFilename(MOG_Filename *assetFilename)
{
	return String::Concat(assetFilename->GetEncodedFilename(), S"\\Properties.Info");
}


String *MOG_ControllerAsset::GetAssetCommentsFilename()
{
	return String::Concat(mAssetFilename->GetEncodedFilename(), S"\\Comments.Log");
}


String *MOG_ControllerAsset::GetAssetCommentsFilename(MOG_Filename *assetFilename)
{
	return String::Concat(assetFilename->GetEncodedFilename(), S"\\Comments.Log");
}


ArrayList *MOG_ControllerAsset::GetAssetsByClassification( String *classification )
{
	return MOG::DATABASE::MOG_DBAssetAPI::GetAllAssetsByClassification( classification);
}

ArrayList *MOG_ControllerAsset::GetAssetsByClassification( String *classification, MOG_Property *propertyToFilterBy )
{
	return MOG::DATABASE::MOG_DBAssetAPI::GetAllAssetsByClassification( classification, propertyToFilterBy );
}


ArrayList *MOG_ControllerAsset::GetArchivedAssetsByClassification( String *classification )
{
	return MOG::DATABASE::MOG_DBAssetAPI::GetAllArchivedAssetsByClassification( classification );
}


// subtracts rootDirectoryPath from each member of files and then finds the longest common string in all the
//  elements of files
String *MOG_ControllerAsset::GetCommonDirectoryPath(String *rootDirectoryPath, ArrayList *files)
{
	if (files == NULL || files->Count == 0)
	{
		return "";
	}

	if (rootDirectoryPath == NULL)
	{
		// assume nothing
		rootDirectoryPath = "";
	}

	// loop through each filename in files and build relDirScope
	String *relDirScope = __try_cast<String *>(files->Item[0]);
	if (relDirScope != NULL)
	{
		// remove the filename
		relDirScope = relDirScope->Substring(0, relDirScope->LastIndexOf("\\"));

		for (int i = 1; i < files->Count; i++)
		{
			//GetCommonDirectoryPath_IdenticalStartingCharacterCount
			String *curDirScope = __try_cast<String *>(files->Item[i]);
			if (curDirScope != NULL) 
			{
				// count identical initial chars
				relDirScope = GetCommonDirectoryPath_IdenticalStartingCharacterCount(relDirScope, curDirScope);
			}
		}

		// Make sure we didn't scan back beyond the specified rootDirectoryPath?
		if (rootDirectoryPath->Length > relDirScope->Length)
		{
			// Force us back to respect the rootDirectoryPath
			relDirScope = rootDirectoryPath;
		}

		// Only trim the front if it isn't a UNC path?
		if (relDirScope->StartsWith(S"\\\\"))
		{
			// Trim any backslashes that remain on the end
			relDirScope = relDirScope->TrimEnd((new String("\\"))->ToCharArray());
		}
		else
		{
			// trim any backslashes that remain
			relDirScope = relDirScope->Trim((new String("\\"))->ToCharArray());
		}

		// we're done
		return relDirScope;
	}
	else
	{
		return "";
	}
}

// helper function of GetCommonDirectoryPath() that counts the number of identical initial characters
//  of two strings (not case-sensitive)
String *MOG_ControllerAsset::GetCommonDirectoryPath_IdenticalStartingCharacterCount(String *strOne, String *strTwo)
{
	String *str = S"";

	// Split the path up
	String *strOneParts[] = strOne->Split(S"\\"->ToCharArray());
	String *strTwoParts[] = strTwo->Split(S"\\"->ToCharArray());
	if (strOneParts != NULL &&		
		strTwoParts != NULL)
	{
		int index = 0;

		while (index < strOneParts->Count  &&  index < strTwoParts->Count)
		{
			if (String::Compare(strOneParts[index], strTwoParts[index], true) != 0)
			{
				break;
			}

			// Build back up our string of identical parts...Be careful about adding the '\'
			if (str->Length == 0)
			{
				str = strOneParts[index];
			}
			else
			{
				str = String::Concat(str, S"\\", strOneParts[index]);
			}

			index++;
		}
	}

	return str;
}


bool MOG_ControllerAsset::ValidateAsset_IsUniquePackageAssignment(MOG_Properties* pProperties)
{
	if (ValidateAsset_GetCollidingPackageAssignments(pProperties)->Count)
	{
		return false;
	}

	return true;
}


bool MOG_ControllerAsset::ValidateAsset_IsUniqueSyncTarget(MOG_Properties* pProperties)
{
	if (ValidateAsset_GetCollidingSyncTargets(pProperties)->Count)
	{
		return false;
	}

	return true;
}


ArrayList* MOG_ControllerAsset::ValidateAsset_GetCollidingSyncTargets(MOG_Properties *pProperties)
{
	HybridDictionary* collidingAssets = new HybridDictionary();

	// Get all the applicable platforms for this asset excluding overridden platform specific platforms
	String *applicablePlatforms[] = MOG_ControllerProject::GetAllApplicablePlaformsForAsset(pProperties->GetAssetFilename()->GetAssetFullName(), true);
	if (applicablePlatforms != NULL)
	{
		// Scan each applicable platform
		for (int p = 0; p < applicablePlatforms->Count; p++)
		{
			// Get this platform's name
			String *platformName = applicablePlatforms[p];
			pProperties->SetScope(platformName);

			// Get the SyncTargetPath for this specific platform
			String *syncTargetPath = MOG_Tokens::GetFormattedString(pProperties->SyncTargetPath, pProperties->GetAssetFilename(), pProperties->GetPropertyList())->Trim(S"\\"->ToCharArray());

			// Get all the files for this platform listed in the Properties
			String *sectionName = DosUtils::PathGetFileName(GetAssetProcessedDirectory(pProperties, platformName));
			String *files[] = NULL;
			if (pProperties->GetNonInheritedPropertiesIni()->SectionExist(sectionName))
			{
				files = pProperties->GetNonInheritedPropertiesIni()->GetSectionKeys(sectionName);
			}
			if (files != NULL)
			{
				// Check each contained file
				for (int f = 0; f < files->Count; f++)
				{
					String *platformspecificFile = files[f];
					String *relativeSyncFile = Path::Combine(syncTargetPath, platformspecificFile);

					// Get all the assets that share this as their sync target?
//					ArrayList *assets = MOG_DBAssetAPI::GetAllAssetsBySyncLocation(relativeSyncFile, platformName);
					ArrayList *assets = MOG_DBAssetAPI::GetSyncTargetFileAssetLinks(relativeSyncFile, platformName);
					if (assets != NULL)
					{
						// Check each asset
						for (int a = 0; a < assets->Count; a++)
						{
							MOG_Filename *assetFilename = __try_cast<MOG_Filename *>(assets->Item[a]);
							// Check if this assets doesn't match us?
							// Only check the Classification and Label because this could be a platform specific asset and still be valid
							if (String::Compare(assetFilename->GetAssetClassification(), pProperties->GetAssetFilename()->GetAssetClassification(), true) != 0 ||
								String::Compare(assetFilename->GetAssetLabel(), pProperties->GetAssetFilename()->GetAssetLabel(), true) != 0)
							{
								// Add this colliding asset to our list of colliding assets.
								collidingAssets->Item[assetFilename->GetAssetFullName()] = assetFilename;
							}
						}
					}
				}
			}
		}

		// Restore the asset's properties scope
		pProperties->SetScope("All");
	}

	// Return all of our colliding assets
	return new ArrayList(collidingAssets->Values);
}


ArrayList* MOG_ControllerAsset::ValidateAsset_GetCollidingPackageAssignments(MOG_Properties *pProperties)
{
	HybridDictionary* collidingAssets = new HybridDictionary();

	// Check if this verification should be performed?
	if (pProperties->UniquePackageAssignmentVerification)
	{
		String *myAssetLabel = pProperties->GetAssetFilename()->GetAssetLabelNoExtension();

		// Get all the applicable platforms for this asset excluding overridden platform specific platforms
		ArrayList* packageAssignments = pProperties->GetPackages();
		if (packageAssignments != NULL)
		{
			// Scan each applicable platform 
			for (int package = 0; package < packageAssignments->Count; package++)
			{
				// Get this platform's name
				MOG_Property* property = dynamic_cast<MOG_Property*>(packageAssignments->Item[package]);
				String* packageAssignment = property->GetPropertyAsString();
				String* packageName = MOG_ControllerPackage::GetPackageName(packageAssignment);
				String* packageGroup = MOG_ControllerPackage::GetPackageGroups(packageAssignment);
				String* packageObject = MOG_ControllerPackage::StripPackageObjectIdentifiers(MOG_ControllerPackage::GetPackageObjects(packageAssignment));

				// Get all assets with the same package assignment
				// Map this package object to its asset names
				ArrayList* foundAssets = MOG_ControllerProject::MapPackageObjectToAssetNames(myAssetLabel, property);
				foundAssets->AddRange(MOG_ControllerProject::MapPackageObjectToAssetNamesInUserInbox(myAssetLabel, property));

//				// Check for the absence of a package object being specified in the package assignment?
//				ArrayList* containedAssets = new ArrayList();
//				if (MOG_ControllerPackage::GetPackageObjects(property->mPropertyKey)->Length == 0)
//				{
//					// Check for contained assets when no packageGroup is specified
//					containedAssets = MOG_ControllerProject::MapPackageObjectToContainedAssetNames(myAssetLabel, property, LOD);
//					containedAssets->AddRange(MOG_ControllerProject::MapPackageObjectToContainedAssetNamesInUserInbox(myAssetLabel, property, LOD));
//					// Combine the two lists for the loop below
//					foundAssets->AddRange(containedAssets);
//				}

				// Check if we found a matching asset?
				if (foundAssets->Count)
				{
					// Enumerate through the post commands
					for(int i = 0; i < foundAssets->Count; i++)
					{
						MOG_Filename* assetFilename = __try_cast<MOG_Filename*>(foundAssets->Item[i]);

						// Check if this is another asset besides ourself?
						if (String::Compare(assetFilename->GetAssetFullName(), pProperties->GetAssetFilename()->GetAssetFullName(), true) != 0)
						{
							// Track this colliding asset
							collidingAssets->Item[assetFilename->GetAssetFullName()] = assetFilename;
						}
					}
				}

//				// Get all the files for this platform listed in the Properties
//				String *sectionName = DosUtils::PathGetFileName(GetAssetProcessedDirectory(pProperties, platformName));
//				String *files[] = NULL;
//				if (pProperties->GetNonInheritedPropertiesIni()->SectionExist(sectionName))
//				{
//					files = pProperties->GetNonInheritedPropertiesIni()->GetSectionKeys(sectionName);
//				}
//				if (files != NULL)
//				{
//					// Check each contained file
//					for (int f = 0; f < files->Count; f++)
//					{
//						String *platformspecificFile = files[f];
//					}
//				}
			}
		}
	}

	// Return all of our colliding assets
	return new ArrayList(collidingAssets->Values);
}


bool MOG_ControllerAsset::ValidateAsset_HasPackageAssignment(MOG_Properties *pProperties)
{
	// Check the package assignments
	ArrayList *packages = pProperties->GetPackages();
	if (packages && packages->Count)
	{
		// Yup, we have a package assignment
		return true;
	}

	// Nope, no package assignments
	return false;
}


bool MOG_ControllerAsset::ValidateAsset_AllAssignedPackagesExist(MOG_Properties* pProperties)
{
	if (ValidateAsset_GetMissingPackages(pProperties)->Count)
	{
		return false;
	}
	return true;
}


ArrayList* MOG_ControllerAsset::ValidateAsset_GetMissingPackages(MOG_Properties* pProperties)
{
	ArrayList* missingPackages = new ArrayList();

	// Check if this is a package asset?
	if (pProperties->IsPackagedAsset)
	{
		// Check each package assignment to make sure it is valid
		ArrayList *packages = pProperties->GetPackages();
		if (packages && packages->Count)
		{
			// Walk through all the listed packages
			for (int packageIndex = 0; packageIndex < packages->Count; packageIndex++)
			{
				MOG_Property *packageAssignmentProperty = __try_cast<MOG_Property*>(packages->Item[packageIndex]);

				// Get the packageName in this package assignment
				String *packageName = MOG_ControllerPackage::GetPackageName(packageAssignmentProperty->mPropertyKey);
				// Make sure this package exists in the project?
				MOG_Filename *packageFilename = new MOG_Filename(packageName);
				if (!MOG_ControllerProject::DoesAssetExists(packageFilename))
				{
					missingPackages->Add(packageFilename);
				}
			}
		}
	}

	return missingPackages;
}


bool MOG_ControllerAsset::ValidateAsset_PlatformApplicable(MOG_Properties* pProperties, String* platformName)
{
	bool bIsApplicable = false;

	// Check if this is a package asset?
	if (pProperties->IsPackagedAsset)
	{
		// Check each package assignment to make sure it is valid
		ArrayList *packages = pProperties->GetPackages();
		if (packages && packages->Count)
		{
			// Walk through all the listed packages
			for (int packageIndex = 0; packageIndex < packages->Count; packageIndex++)
			{
				MOG_Property *packageAssignmentProperty = __try_cast<MOG_Property*>(packages->Item[packageIndex]);

				// Get the packageName in this package assignment
				String *packageName = MOG_ControllerPackage::GetPackageName(packageAssignmentProperty->mPropertyKey);
				MOG_Filename *packageFilename = new MOG_Filename(packageName);

				// Check if this package assignment is for the '{All}' package?
				if (String::Compare(packageFilename->GetAssetPlatform(), S"All", true) == 0)
				{
					bIsApplicable = true;
				}
				// Check if this package assignment matches the specified platformName?
				else if (String::Compare(packageFilename->GetAssetPlatform(), platformName, true) == 0)
				{
					bIsApplicable = true;
				}
			}
		}
	}
	else
	{
		// Ask the project if this platform is applicable for this asset?
		bIsApplicable = MOG_ControllerProject::IsPlatformValidForAsset(pProperties->GetAssetFilename(), platformName);
	}

	return bIsApplicable;
}






