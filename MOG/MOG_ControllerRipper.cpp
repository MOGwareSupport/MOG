//--------------------------------------------------------------------------------
//	MOG_ControllerRipper.cpp
//	
//	
//--------------------------------------------------------------------------------

#include "StdAfx.h"

#include "MOG_Define.h"
#include "MOG_Tokens.h"
#include "MOG_EnvironmentVars.h"
#include "MOG_ControllerProject.h"
#include "MOG_ControllerSystem.h"
#include "MOG_ControllerInbox.h"
#include "MOG_CommandFactory.h"

#include "MOG_ControllerRipper.h"



MOG_ControllerRipper::MOG_ControllerRipper(MOG_Command *pCommand)
{
	Initialize(	pCommand->GetAssetFilename(),
				NULL,
				Convert::ToInt32(pCommand->GetNetworkID()),
				pCommand->GetUserName(), 
				pCommand->GetJobLabel(),
				pCommand->GetValidSlaves(),
				false);
}


MOG_ControllerRipper::MOG_ControllerRipper(MOG_Filename *assetFilename)
{
	Initialize(assetFilename, NULL, 0, S"", S"", S"", false);
}


MOG_ControllerRipper::MOG_ControllerRipper(MOG_Filename *assetFilename, MOG_Properties *properties, int originatingNetworkID, String *originatingUserName, String *jobLabel, String *validSlaves, bool showRipCommandWindow)
{
	Initialize(assetFilename, properties, originatingNetworkID, originatingUserName, jobLabel, validSlaves, showRipCommandWindow);
}


void MOG_ControllerRipper::Initialize(MOG_Filename *assetFilename, MOG_Properties *properties, int originatingNetworkID, String *originatingUserName, String *jobLabel, String *validSlaves, bool showRipCommandWindow)
{
	mAssetFilename = assetFilename;
	mProperties = properties;
	mOriginatingNetworkID = originatingNetworkID;
	mOriginatingUserName = originatingUserName;
	mJobLabel = jobLabel;
	mValidSlaves = validSlaves;
	mShowRipCommandWindow = showRipCommandWindow;
	mOutputLog = S"";

	// Setup initialization defaults...
	// Check if no originating NetworkID was specified?
	if (mOriginatingNetworkID == 0)
	{
		// Assume us as the originator and use our own NetworkID
		mOriginatingNetworkID = MOG_ControllerSystem::GetNetworkID();
	}
	// Check if no originating UserName was specified?
	if (mOriginatingUserName->Length == 0)
	{
		// Assume us as the originator and use our own UserName
		mOriginatingUserName = MOG_ControllerProject::GetUserName();
	}
	// Check if no jobLabel was specified?
	if (mJobLabel->Length == 0)
	{
		// Obtain a unique jobLabel for this rip
		mJobLabel = String::Concat(S"Rip.", MOG_ControllerSystem::GetComputerName(), S".", MOG_Time::GetVersionTimestamp());
	}
}


ArrayList *MOG_ControllerRipper::GetRipperEnvironment(String *platformName, MOG_Properties *pProperties)
{
	ArrayList *environment = new ArrayList();

	// Build the project variables
	DosUtils::EnvironmentList_AddVariable(environment, MOG_EnvVar_ProjectName, MOG_ControllerProject::GetProjectName());
	DosUtils::EnvironmentList_AddVariable(environment, MOG_EnvVar_BranchName, MOG_ControllerProject::GetBranchName());
	DosUtils::EnvironmentList_AddVariable(environment, MOG_EnvVar_UserName, MOG_ControllerProject::GetUser()->GetUserName());
	DosUtils::EnvironmentList_AddVariable(environment, MOG_EnvVar_PlatformName, platformName);

	// Build the command variables
	DosUtils::EnvironmentList_AddVariable(environment, MOG_EnvVar_CommandLabel, mJobLabel);

	// Add the Ripping variables
	DosUtils::EnvironmentList_AddVariable(environment, MOG_EnvVar_WorkingDirectory, MOG_ControllerAsset::GetAssetImportedDirectory(pProperties));
	DosUtils::EnvironmentList_AddVariable(environment, MOG_EnvVar_AssetFiles, MOG_ControllerAsset::GetAssetImportedDirectory(pProperties));
	DosUtils::EnvironmentList_AddVariable(environment, MOG_EnvVar_RippedFilesDestination, MOG_ControllerAsset::GetAssetFilesDirectory(pProperties, platformName, false));

	// Build the Filename variables
	MOG_Filename *assetFilename = pProperties->GetAssetFilename();
	DosUtils::EnvironmentList_AddVariable(environment, MOG_EnvVar_Asset, assetFilename->GetEncodedFilename());
	DosUtils::EnvironmentList_AddVariable(environment, MOG_EnvVar_AssetPath, assetFilename->GetPath());
	DosUtils::EnvironmentList_AddVariable(environment, MOG_EnvVar_AssetName, assetFilename->GetAssetFullName());
	DosUtils::EnvironmentList_AddVariable(environment, MOG_EnvVar_AssetClassification, assetFilename->GetAssetClassification());
	DosUtils::EnvironmentList_AddVariable(environment, MOG_EnvVar_AssetLabel, assetFilename->GetAssetLabel());
	DosUtils::EnvironmentList_AddVariable(environment, MOG_EnvVar_AssetPlatformName, assetFilename->GetAssetPlatform());
	DosUtils::EnvironmentList_AddVariable(environment, MOG_EnvVar_AssetExtension, assetFilename->GetExtension());

	// Include all of the asset's properties in the environment?
	ArrayList *properties = pProperties->GetApplicableProperties();
	if (properties != NULL)
	{
		// Add each specified property to our environment
		for (int p = 0; p < properties->Count; p++)
		{
			MOG_Property *property = __try_cast<MOG_Property *>(properties->Item[p]);
			if (property->mPropertyKey->Length)
			{
				String *tokenizedValue = MOG_Tokens::GetFormattedString(property->mPropertyValue, NULL, properties);
				DosUtils::EnvironmentList_AddVariable(environment, property->mPropertyKey, tokenizedValue);
			}
		}
	}

	return environment;
}


bool MOG_ControllerRipper::IsPlatformRelevent(String *platformName1, String *platformName2)
{
	bool bApplicable = false;

	// Check for platform relevence...
	// Do they match?
	if (String::Compare(platformName1, platformName2, true) == 0)
	{
		bApplicable = true;
	}
	// Is either one of them 'All'?
	else if (String::Compare(platformName1, S"All", true) == 0 ||
			 String::Compare(platformName2, S"All", true) == 0)
	{
		bApplicable = true;
	}

	return bApplicable;
}

ArrayList *MOG_ControllerRipper::GenerateSlaveTaskCommands(String *platformName)
{
	ArrayList *generatedSlaveTaskCommands = new ArrayList();

	// Check if a specific platform was specified?
	if (platformName->Length == 0)
	{
		platformName = S"All";
	}

	try
	{
		// Make sure we have a valid assetFilename specified?
		if (mAssetFilename &&
			mAssetFilename->GetFilenameType() == MOG_FILENAME_TYPE::MOG_FILENAME_Asset)
		{
			MOG_Project *pProject = MOG_ControllerProject::GetProject();
			if (pProject)
			{
				// Open the asset?
				MOG_ControllerAsset *asset = MOG_ControllerAsset::OpenAsset(mAssetFilename, mProperties);
				if (asset)
				{
					// Get the asset's properties
					MOG_Properties *pProperties = asset->GetProperties();

					// Set the proper platform scope
					pProperties->SetScope(S"All");

					// Check if there was no slave specified?
					if (mValidSlaves->Length == 0)
					{
						// Respect the setting with the properties of this asset
						mValidSlaves = pProperties->ValidSlaves;
					}

					// Check if we are hiding the rip window?
					if (mShowRipCommandWindow != true)
					{
						// Respect the setting with the properties of this asset
						mShowRipCommandWindow = pProperties->ShowRipCommandWindow;
					}

					// Set the asset's appropriate processing state
					MOG_ControllerInbox::UpdateInboxView(asset, MOG_AssetStatusType::Ripping);

					// Get the Platforms that this asset should be ripped for
					String *platformNames[] = NULL;
					if (pProperties->DivergentPlatformDataType)
					{
						// Loop through all the applicable platforms for this asset
						platformNames = MOG_ControllerProject::GetAllApplicablePlaformsForAsset(mAssetFilename->GetAssetFullName(), false);
					}
					else
					{
						// Use the generic '.All'
						platformNames = new String*[1];
						platformNames[0] = S"All";
					}

					// Build the slave task for each identified platform
					for (int p = 0; p < platformNames->Count; p++)
					{
						// Check for platform relevence
						if (IsPlatformRelevent(platformNames[p], platformName))
						{
							String *platformName = platformNames[p];

							// Get the Asset's Properties associated with this platform?
							pProperties->SetScope(platformNames[p]);

							// Build the needed information for launching the RipTasker
							String *workingDirectory = asset->GetAssetFilename()->GetEncodedFilename();
							String *sourcePath = MOG_ControllerAsset::GetAssetImportedDirectory(pProperties);
							String *sourceMask =  S"*.*";
							String *sourceFiles = String::Concat(sourcePath, S"\\", sourceMask);
							String *destination = MOG_ControllerAsset::GetAssetFilesDirectory(pProperties, platformNames[p], false);
							String *description = String::Concat(S"Default MOG Slave Task for '", platformNames[p], S"'");
							ArrayList *environment = GetRipperEnvironment(platformNames[p], pProperties);

							// Make sure the destination is different than the source
							if (String::Compare(sourcePath, destination, true) != 0)
							{
								// Create the destination in preparation of receiving ripped files from the SlaveTasks
								if (!DosUtils::DirectoryCreate(destination, true))
								{
									String *message = String::Concat(	S"Failed to create directory in the MOG Repository.  The network may be down.\n",
																		S"'", mAssetFilename->GetAssetFullName(), S"' was not ripped correctly.");
									MOG_Report::ReportMessage(S"RipTasker Failed", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
								}
							}

							// Create the destination in preparation of receiving ripped files from the SlaveTasks
							if (DosUtils::DirectoryExist(destination))
							{
								// Create a corresponding setion in the properties file so we can clean this directory up later
								pProperties->GetNonInheritedPropertiesIni()->PutSection(DosUtils::PathGetFileName(destination));

								// Check if no RipTasker has been specified for this asset?
								// resolve any tokens that might be in the ripper string
								String *ripTasker = MOG_Tokens::GetFormattedString(pProperties->AssetRipTasker, mAssetFilename, pProperties->GetPropertyList(), sourcePath, sourceFiles, destination);

								if (ripTasker->Length == 0 || 
									String::Compare(ripTasker, S"None", true) == 0 ||
									String::Compare(ripTasker, S"Default", true) == 0 ||
									String::Compare(ripTasker, S"Copy", true) == 0 )
								{
									//resolve any tokens that might be in the ripper string
									String* ripper = MOG_Tokens::GetFormattedString(pProperties->AssetRipper, mAssetFilename, pProperties->GetPropertyList(), sourcePath, sourceFiles, destination);
									
									// Send our new default slave task command to the server
									MOG_Command *newSlaveTask = MOG_CommandFactory::Setup_SlaveTask(mOriginatingNetworkID,
																									mOriginatingUserName,
																									mJobLabel,
																									mAssetFilename,
																									platformNames[p], 
																									workingDirectory,
																									ripper,
																									environment,
																									(mShowRipCommandWindow) ? String::Concat(S"ShowRipWindow=", mShowRipCommandWindow.ToString()) : S"",
																									sourceFiles,
																									destination,
																									description,
																									mValidSlaves);
									generatedSlaveTaskCommands->Add(newSlaveTask);

									// Indicate the use of the built-in default RipTasker
									String *message = String::Concat(	S"RipTasker: Default\n",
																		S"PLATFORM: ", platformNames[p], S"\n",
																		S"Asset used the built-in default MOG RipTasker.");
									// Save a copy of the rip log into the asset directory
									String *logFilename = String::Concat(mAssetFilename->GetEncodedFilename(), S"\\RipTasker.", platformNames[p], S".log");
									DosUtils::FileWrite(logFilename, message);

									// Append this message to our output log
									mOutputLog = String::Concat(mOutputLog, 
																S"----------------------------------------------------------------------\n",
																message);
								}
								// Use the specified RipTasker to process this asset
								else
								{
									// Locate this ripper in the Tools directories
									String *relativeRipTasker = DosUtils::FileStripArguments(ripTasker);
									String *relativeRipTaskerPath = DosUtils::PathGetDirectoryPath(relativeRipTasker);
									String *relativeRipTaskerFilename = DosUtils::PathGetFileName(relativeRipTasker);
									String *locatedRipTasker = MOG_ControllerSystem::LocateTool(relativeRipTaskerPath, relativeRipTaskerFilename, mAssetFilename);
									if (locatedRipTasker->Length)
									{
										bool bWarnUserAboutModifiedFiles = true;
										String *output = S"";

										ArrayList* oldFiles = DosUtils::FileGetRecursiveRelativeList(sourcePath, sourceMask);

										//Mark the time before we start ripping
										DateTime preRipTime = DateTime::Now;

										// Extend the Path environment variable to encompass all potential tools directories
										DosUtils::EnvironmentList_AddVariable(environment, MOG_EnvironmentVars::CreateToolsPathEnvironmentVariableString(relativeRipTaskerPath, mAssetFilename));

										// Launch the slave process in a seperate window
										int errorCode = DosUtils::SpawnDosCommand(sourcePath, locatedRipTasker, DosUtils::FileGetArguments(ripTasker), environment, &output, !mShowRipCommandWindow);
										if (errorCode != 0)
										{
											String *message = String::Concat(	S"Ripping tool '", ripTasker, S"' returned ErrorCode ", errorCode.ToString(), S"\n",
																				S"Click the details button to view the Ripper log.\n\n",
																				S"Confirm this seriousness of this error with your MOG Administrator.");
											MOG_Report::ReportMessage(S"Asset RipTasker", message, output, MOG_ALERT_LEVEL::ALERT);
										}

										ArrayList* newFiles = DosUtils::FileGetRecursiveRelativeList(sourcePath, sourceMask);

										ArrayList* createdFiles = DetectCreatedFiles(oldFiles, newFiles);
										ArrayList* modifiedFiles = CheckForModifiedOriginalFiles(sourcePath, oldFiles, sourcePath, newFiles, preRipTime);
										if (bWarnUserAboutModifiedFiles)
										{
											WarnModifiedFiles(modifiedFiles);
										}

										if (createdFiles && createdFiles->Count > 0)
										{
											CleanUpCreatedFiles(createdFiles, sourcePath);
										}

										// Indicate the output of our RipTasker
										String *message = String::Concat(	S"RIPTASKER: ", ripTasker, S"\n",
																			S"PLATFORM: ", platformNames[p], S"\n",
																			output);
										// Save a copy of the RipTasker log into the asset directory
										String *logFilename = String::Concat(mAssetFilename->GetEncodedFilename(), S"\\RipTasker.", platformNames[p], S".log");
										DosUtils::FileWrite(logFilename, message);

										// Append this message to our output log
										mOutputLog = String::Concat(mOutputLog, 
																	S"----------------------------------------------------------------------\n",
																	message);
									}
									else
									{
										String *message = String::Concat(	S"Unable to locate '", ripTasker, S"' in any of the System's Tools Directories.\n",
																			S"The Asset cannot be ripped correctly until this this tool exists.\n",
																			S"ASSET: ", mAssetFilename->GetAssetFullName());
										MOG_Report::ReportMessage(S"RipTasker Failed", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
										break;
									}
								}
							}
						}
					}

					// Always close the asset as soon as we have finished with it
					asset->Close();
				}
				else
				{
					String *message = String::Concat(	S"Unable to open specified asset.\n",
														S"ASSET: ", mAssetFilename->GetFullFilename());
					MOG_Report::ReportMessage(S"RipTasker Failed", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
				}
			}
			else
			{
				String *message = S"ControllerRipper cannot work properly without being logged into a project.";
				MOG_Report::ReportMessage(S"RipTasker Failed", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
			}
		}
	}
	catch (Exception *e)
	{
		String *message = String::Concat(	S"ASSETNAME: ", mAssetFilename->GetAssetFullName(), S"\n",
											e->Message);
		MOG_Report::ReportMessage(S"RipTasker Failed", message, e->StackTrace, MOG_ALERT_LEVEL::CRITICAL);
	}

	return generatedSlaveTaskCommands;
}


bool MOG_ControllerRipper::ProcessSlaveTasks(ArrayList *tasks)
{
	bool bFailed = false;

	if (tasks->Count)
	{
		// Process each command listed in the array
		for (int t = 0; t < tasks->Count; t++)
		{
			MOG_Command *task = __try_cast<MOG_Command*>(tasks->Item[t]);
			if (!ProcessSlaveTask(task))
			{
				bFailed = true;
			}
		}
	}

	return bFailed;
}

bool MOG_ControllerRipper::ProcessSlaveTask(MOG_Command *pCommand)
{
	try
	{
		MOG_Project *pProject = MOG_ControllerProject::GetProject();
		if (pProject)
		{
			String *commandLog = S"";
			String *platformName = pCommand->GetPlatform();

			// Check if no Ripper has been specified for this asset?
			String *ripper = pCommand->GetTool();
			if (ripper->Length == 0 || 
				String::Compare(ripper, S"None", true) == 0 ||
				String::Compare(ripper, S"Default", true) == 0 ||
				String::Compare(ripper, S"Copy", true) == 0 )
			{
				// Gather the needed information for our default slave task
				String *sourcePath = DosUtils::PathGetDirectoryPath(pCommand->GetSource());
				String *targetPath = pCommand->GetDestination();
				ArrayList *files = DosUtils::FileGetRecursiveRelativeList(sourcePath, DosUtils::PathGetFileName(pCommand->GetSource()));

				// Copy the contained files
				for (int f = 0; f < files->Count; f++)
				{
					String *relativeFile = __try_cast<String *>(files->Item[f]);
					String *sourceFile = String::Concat(sourcePath, S"\\", relativeFile);
					String *targetFile = String::Concat(targetPath, S"\\", relativeFile);
					if (DosUtils::FileCopyFast(sourceFile, targetFile, true))
					{
						DosUtils::FileTouch(targetFile);
					}
				}

				// Indicate to the commandLog that we used the built in DefaultRip command
				commandLog = String::Concat(	S"RIPPER: Default\n",
												S"PLATFORM: ", platformName, S"\n",
												S"Asset used the built-in default MOG CopyRipper.");

				// Save a copy of the rip log into the asset directory
				String *logFilename = String::Concat(pCommand->GetAssetFilename()->GetEncodedFilename(), S"\\Rip.", platformName, S".log");
				DosUtils::FileWrite(logFilename, commandLog);
			}
			// Use the specified Ripper to process this asset
			else
			{
				// Locate this ripper in the Tools directories
				String *relativeRipper = DosUtils::FileStripArguments(ripper);
				String *relativeRipperPath = DosUtils::PathGetDirectoryPath(relativeRipper);
				String *relativeRipperFilename = DosUtils::PathGetFileName(relativeRipper);
				String *locatedRipper = MOG_ControllerSystem::LocateTool(relativeRipperPath, relativeRipperFilename, pCommand->GetAssetFilename());
				if (locatedRipper->Length)
				{
					// Build the needed information for launching the Ripper
					ArrayList *environment = new ArrayList();
					DosUtils::EnvironmentList_AddVariables(environment, pCommand->GetVeriables()->Split(S","->ToCharArray()));

					// Extend the Path environment variable to encompass all potential tools directories
					DosUtils::EnvironmentList_AddVariable(environment, MOG_EnvironmentVars::CreateToolsPathEnvironmentVariableString(relativeRipperPath, pCommand->GetAssetFilename()));

					bool bShowRipCommandWindow = (mShowRipCommandWindow) ? true : (pCommand->GetOptions()->ToLower()->IndexOf(S"ShowRipWindow"->ToLower()) != -1) ? true : false;
					bool bUseTempRipDir				= String::Compare(DosUtils::EnvironmentList_GetVariable(environment, PROPTEXT_RipOptions_UseTempRipDir), S"true", true) == 0;
					bool bUseLocalTempRipDir		= String::Compare(DosUtils::EnvironmentList_GetVariable(environment, PROPTEXT_RipOptions_UseLocalTempRipDir), S"true", true) == 0;
					bool bCopyFilesIntoTempRipDir	= String::Compare(DosUtils::EnvironmentList_GetVariable(environment, PROPTEXT_RipOptions_CopyFilesIntoTempRipDir), S"true", true) == 0;
					bool bAutoDetectRippedFiles		= String::Compare(DosUtils::EnvironmentList_GetVariable(environment, PROPTEXT_RipOptions_AutoDetectRippedFiles), S"true", true) == 0;
					bool bWarnUserAboutModifiedFiles = false;

					// Get a list of all the files in the imported directory so we can compare it to the list we get after we rip
					String *SourceDirectory = DosUtils::PathGetDirectoryPath(pCommand->GetSource());
					String *SourceFilePattern = DosUtils::PathGetFileName(pCommand->GetSource());
					ArrayList* oldFiles = DosUtils::FileGetRecursiveRelativeList(SourceDirectory, SourceFilePattern);

					// Establish our working directory
					String* workingDirectory = SourceDirectory;
					if (bUseTempRipDir)
					{
						String* tempDirectory = CreateTempRipDir(bUseLocalTempRipDir, pCommand->GetAssetFilename()->GetEncodedFilename());

						if (bCopyFilesIntoTempRipDir)
						{
							CopyFilesIntoTempRipDir(pCommand->GetSource(), tempDirectory);
						}

						workingDirectory = tempDirectory;

						// Update the environment to reflect our new working directory
						DosUtils::EnvironmentList_AddVariable(environment, MOG_EnvVar_WorkingDirectory, workingDirectory);
					}

					// Mark the time before we start ripping
					DateTime preRipTime = DateTime::Now;

					// Launch the slave process in a seperate window
					int errorCode = DosUtils::SpawnDosCommand(workingDirectory, locatedRipper, DosUtils::FileGetArguments(ripper), environment, &commandLog, !bShowRipCommandWindow);
					if (errorCode != 0)
					{
						String *message = String::Concat(	S"Ripping tool '", ripper, S"' returned ErrorCode ", errorCode.ToString(), S"\n",
															S"Click the details button to view the Ripper log.\n\n",
															S"Confirm this seriousness of this error with your MOG Administrator.");
						MOG_Report::ReportMessage(S"Asset Ripper", message, commandLog, MOG_ALERT_LEVEL::ALERT);
					}

					//Get a list of files in the working directory after the ripper has completed
					ArrayList* newFiles = DosUtils::FileGetRecursiveRelativeList(workingDirectory, S"*");

					//Find out which of the current set of files were created or modified by the ripper
					ArrayList* createdFiles = DetectCreatedFiles(oldFiles, newFiles);
					ArrayList* modifiedFiles = CheckForModifiedOriginalFiles(SourceDirectory, oldFiles, workingDirectory, newFiles, preRipTime);
					if (!bUseTempRipDir)
					{
						if (bWarnUserAboutModifiedFiles)
						{
							WarnModifiedFiles(modifiedFiles);
						}
					}

					if (bAutoDetectRippedFiles)
					{
						String* destination = DosUtils::EnvironmentList_GetVariable(environment, MOG_EnvVar_RippedFilesDestination);
						MoveModifiedFiles(modifiedFiles, workingDirectory, destination, bUseTempRipDir);
						MoveCreatedFiles(createdFiles, workingDirectory, destination);
					}
					else
					{
						//We are not in a temp rip dir, but let's at least clean up the place before we leave
						CleanUpCreatedFiles(createdFiles, workingDirectory);
					}

					if (bUseTempRipDir)
					{
						//Delete the temp rip dir
						if (DosUtils::DirectoryExistFast(workingDirectory))
							DosUtils::DirectoryDeleteFast(workingDirectory);
					}

					// Save a copy of the rip log into the asset directory
					String *logFilename = String::Concat(pCommand->GetAssetFilename()->GetEncodedFilename(), S"\\Rip.", platformName, S".log");
					DosUtils::FileWrite(logFilename, commandLog);

					// Update our own log as well
					MOG_ControllerSystem::GetCommandManager()->AppendMessageToOutputLog(commandLog);
				}
				else
				{
					String *message = String::Concat(	S"The System was unable to locate '", ripper, S"' in any of the System's Tools Directories\n",
														S"The Asset will not be ripped correctly until this this tool exists.");
					MOG_Report::ReportMessage(S"Asset Rip", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
				}
			}
		}
		else
		{
			String *message = String::Concat( S"The Slave's default RipTasker was unable to login to the specified Project: ", pCommand->GetProject());
			MOG_Report::ReportMessage(S"Asset Rip", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
		}
	}
	catch (Exception *e)
	{
		String *message = String::Concat(	S"ASSETNAME: ", mAssetFilename->GetAssetFullName(), S"\n",
											e->Message);
		MOG_Report::ReportMessage(S"SlaveTask Failed", message, e->StackTrace, MOG_ALERT_LEVEL::CRITICAL);
		return false;
	}

	return true;
}


bool MOG_ControllerRipper::CompleteRip()
{
	bool bFailed = false;

	// Make sure the asset's directory still exists
	MOG_ControllerAsset *asset = MOG_ControllerAsset::OpenAsset(mAssetFilename, mProperties);
	if (asset)
	{
		// Set the asset sizes for each platform of this asset
		MOG_ControllerAsset::SetAssetSizes(asset);

		// Record all the files and return if any assets failed
		bool recorded = MOG_ControllerAsset::RecordAllAssetFiles(asset);
		if (recorded)
		{
			// Indicate it was processed
			MOG_ControllerInbox::UpdateInboxView(asset, MOG_AssetStatusType::Processed);
		}
		else
		{
			// Indicate a RipError
			MOG_ControllerInbox::UpdateInboxView(asset, MOG_AssetStatusType::RipError);

			String *message = String::Concat(	S"Ripper failed to create all the necessary files for the asset.\n",
												S"ASSET: ", mAssetFilename->GetAssetFullName(), S"\n\n",
												S"The Asset was not ripped correctly.");
			MOG_Report::ReportMessage(S"Asset Rip", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
			bFailed = true;
		}

		// Always make sure we close the asset's controller
		asset->Close();
	}
	else
	{
		String *message = String::Concat(	S"The System was unable to open the specified asset.\n",
											S"ASSET: ", mAssetFilename->GetFullFilename(), S"\n\n",
											S"The Asset was not ripped correctly.");
		MOG_Report::ReportMessage(S"Asset Rip", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
		bFailed = true;
	}

	return bFailed;
}


bool MOG_ControllerRipper::LocalRipper(MOG_Properties *properties, String *platformName)
{
	bool bFailed = false;

	// Check if a specific MOG_Properties was specified?
	if (properties)
	{
		mProperties = properties;
	}

	// Check if a specific platform was specified?
	if (platformName->Length == 0)
	{
		platformName = S"All";
	}

	// Update all of the asset's dependencies
//	UpdateAssetDependencies();

	// Generate the slave tasks
	ArrayList *tasks = GenerateSlaveTaskCommands(platformName);
	if (tasks)
	{
		// Process each command listed in the array
		for (int t = 0; t < tasks->Count; t++)
		{
			MOG_Command *task = __try_cast<MOG_Command*>(tasks->Item[t]);

			// Check for platform relevence
			if (IsPlatformRelevent(task->GetPlatform(), platformName))
			{
				if (!ProcessSlaveTask(task))
				{
					bFailed = true;
				}
			}
		}
	}

	// Complete the rip
	if (!CompleteRip())
	{
		bFailed = true;
	}

	return bFailed;
}


String* MOG_ControllerRipper::CreateTempRipDir(bool bLocal, String* assetName)
{
	String* tempDirectory = S"";

	if (bLocal)
	{
		tempDirectory = Path::Combine(Environment::GetFolderPath(Environment::SpecialFolder::ApplicationData), S"Mogware");
	}
	else
	{
		tempDirectory = String::Concat(assetName);
	}

	tempDirectory = String::Concat(tempDirectory, S"\\TempRip.", MOG_ControllerSystem::GetComputerName(), S".", Convert::ToString(MOG_ControllerSystem::GetNetworkID()), S".", MOG_Time::GetVersionTimestamp());
	
	//If the directory already exists, delete it so we don't have anything left over from before
	if (DosUtils::DirectoryExistFast(tempDirectory))
		DosUtils::DirectoryDelete(tempDirectory);

	//There should not be a working directory now, so let's create a new one
	DosUtils::DirectoryCreate(tempDirectory, true);

	return tempDirectory;
}

void MOG_ControllerRipper::CopyFilesIntoTempRipDir(String* source, String* tempDirectory)
{
	String* srcFileDir = DosUtils::PathGetDirectoryPath(source);
	String* srcFileMask = DosUtils::PathGetFileName(source);

	// Copy all the files into the temp rip dir
	ArrayList* files = DosUtils::FileGetRecursiveRelativeList(srcFileDir, srcFileMask);
	if (files != NULL)
	{
		for (int i = 0; i < files->Count; i++)
		{
			String* filename = __try_cast<String*>(files->Item[i]);
			if (filename)
			{
				String* srcFilename = String::Concat(srcFileDir, S"\\", filename);
				String* dstFilename = String::Concat(tempDirectory, S"\\", filename);
				DosUtils::FileCopyFast(srcFilename, dstFilename, true);
			}
		}
	}

	// Change all of the copied files attribute's to normal so the ripper has complete access
	DosUtils::SetAttributesRecursive(tempDirectory, S"*.*", FileAttributes::Normal);
}

ArrayList* MOG_ControllerRipper::CheckForModifiedOriginalFiles(String* originalDirectory, ArrayList* originalFiles, String* workingDirectory, ArrayList* newFiles, DateTime preRipTime)
{
	ArrayList* modifiedFiles = new ArrayList;
	
	// Find the files (new files + files with newer timestamp)
	if (originalFiles != NULL && newFiles != NULL)
	{
		// Go through the list of new files
		for (int i = 0; i < newFiles->Count; i++)
		{
			String* newFile = __try_cast<String*>(newFiles->Item[i]);
			bool modified = false;

			// Now go through the list of old files and see if there is a match on any of them
			for (int j = 0; j < originalFiles->Count; j++)
			{
				String* originalFile = __try_cast<String*>(originalFiles->Item[j]);

				if (String::Compare(newFile, originalFile, true) == 0)
				{
					// Let's check the timestamp of the newFile to see if it changed during our rip?
					String* newFilename = String::Concat(workingDirectory, S"\\", newFile);
					FileInfo* newFileInfo = DosUtils::FileGetInfo(newFilename);
					if (newFileInfo->LastWriteTime >= preRipTime)
					{
						//The timestamp has changed
						modified = true;
					}
					else
					{
						// Check if the newFile and originalFile are different files?
				        String* originalFilename = String::Concat(originalDirectory, S"\\", originalFile);
						if (String::Compare(newFilename, originalFilename, true) != 0)
						{
						    // Check the newFile's time against the original file's time to see if its been modified since the copy
					        FileInfo* originalFileInfo = DosUtils::FileGetInfo(originalFilename);
					        // Did the user modify this time in ANY way?
					        if (originalFileInfo->LastWriteTime != newFileInfo->LastWriteTime)
					        {
						        // The timestamp has changed
						        modified = true;
					        }
						}
					}

					break;
				}
			}

			if (modified)
			{
				modifiedFiles->Add(newFile);
			}
		}
	}

	return modifiedFiles;
}

ArrayList* MOG_ControllerRipper::DetectCreatedFiles(ArrayList* oldFiles, ArrayList* newFiles)
{
	ArrayList* createdFiles = new ArrayList;

	//find the files (new files + files with newer timestamp)
	if (oldFiles != NULL && newFiles != NULL)
	{
		//Go through the list of new files
		for (int i = 0; i < newFiles->Count; i++)
		{
			String* newFile = __try_cast<String*>(newFiles->Item[i]);
			bool created = true;

			//now go through the list of old files and see if there is a match on any of them
			for (int j = 0; j < oldFiles->Count; j++)
			{
				String* oldFile = __try_cast<String*>(oldFiles->Item[j]);

				if (String::Compare(newFile, oldFile, true) == 0)
				{
					//Well, the new filename matched an old filename, so it isn't a totally new file
					created = false;
					break;
				}
			}

			if (created)
			{
				createdFiles->Add(newFile);
			}
		}
	}

	return createdFiles;
}

void MOG_ControllerRipper::MoveModifiedFiles(ArrayList* files, String* sourcePath, String* destinationPath, bool bUseTempRipDir)
{
	if (files != NULL)
	{
		for (int i = 0; i < files->Count; i++)
		{
			String* filename = __try_cast<String*>(files->Item[i]);
			String* srcFilename = String::Concat(sourcePath, S"\\", filename);
			String* dstFilename = String::Concat(destinationPath, S"\\", filename);

			if (bUseTempRipDir)
			{
				//We're in a temp dir so this file is going to get deleted anyway
				//Might as well jsut move it for a possible speed increase
				DosUtils::FileMoveFast(srcFilename, dstFilename, true);
			}
			else
			{
				//We are in the source directory so we gotta leave that original file alone
				DosUtils::FileCopyFast(srcFilename, dstFilename, true);
			}
		}
	}
}

void MOG_ControllerRipper::MoveCreatedFiles(ArrayList* files, String* sourcePath, String* destinationPath)
{
	if (files != NULL)
	{
		for (int i = 0; i < files->Count; i++)
		{
			String* filename = __try_cast<String*>(files->Item[i]);
			
			String* srcFilename = String::Concat(sourcePath, S"\\", filename);
			String* dstFilename = String::Concat(destinationPath, S"\\", filename);
			DosUtils::FileMoveFast(srcFilename, dstFilename, true);
		}
	}
}

void MOG_ControllerRipper::WarnModifiedFiles(ArrayList* files)
{
	if (files != NULL)
	{
		for (int i = 0; i < files->Count; i++)
		{
			String* filename = __try_cast<String*>(files->Item[i]);

			MOG_Report::ReportMessage(	S"Rip Warning",
										String::Concat(	S"The ripper has modified the source file: '", filename, S"'\n",
														S"It is not recommended to modify source files when ripping.\n",
														S"Please use a TempRipDir to ensure the integrity of the source file."),
										Environment::StackTrace, MOG_ALERT_LEVEL::ALERT);
		}
	}
}

void MOG_ControllerRipper::CleanUpCreatedFiles(ArrayList* files, String* workingDirectory)
{
	if (files != NULL)
	{
		for (int i = 0; i < files->Count; i++)
		{
			String* filename = __try_cast<String*>(files->Item[i]);
			String* srcFilename = String::Concat(workingDirectory, S"\\", filename);
			
			MOG_Report::ReportMessage(	S"Rip Warning",
										String::Concat(	S"The ripper has created a new file in the source directory: '", filename, S"'\n",
														S"Adding a file to the repository with a ripper is not permitted and it will be deleted.\n",
														S"Please use TempRipDir or delete the file yourself to avoid this warning in the future."),
										Environment::StackTrace, MOG_ALERT_LEVEL::ALERT);

			DosUtils::FileDeleteFast(srcFilename);
		}
	}
}


