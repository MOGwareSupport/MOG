
//--------------------------------------------------------------------------------
//	MOG_CommandFactory.cpp
//	
//	
//--------------------------------------------------------------------------------

#include "StdAfx.h"

#include "MOG_Define.h"
#include "MOG_Time.h"
#include "MOG_ControllerProject.h"
#include "MOG_ControllerSystem.h"

#include "MOG_Command.h"
#include "MOG_CommandFactory.h"


using System::IO::MemoryStream;

using namespace System::IO;

MOG_Command * MOG_CommandFactory::Setup_None(void)
{
	MOG_Command *setupCommand = new MOG_Command();
	
	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_None;
	return setupCommand;
}

MOG_Command * MOG_CommandFactory::Setup_ConnectionNew(int networkID, String *connectionString, String *disabledFeatureList)
{
	MOG_Command *setupCommand = new MOG_Command();
	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_ConnectionNew;
	setupCommand->mDescription = connectionString;
	setupCommand->mVersion = disabledFeatureList;
	setupCommand->mNetworkID = networkID;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_ConnectionLost(void)
{
	MOG_Command *setupCommand = new MOG_Command();
	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_ConnectionLost;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_ConnectionKill(int networkID)
{
	MOG_Command *setupCommand = new MOG_Command();
	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_ConnectionKill;
	setupCommand->mNetworkID = networkID;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_RegisterClient(String *name, String *executablePath)
{
	MOG_Command *setupCommand = new MOG_Command();
	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_RegisterClient;
	setupCommand->mDescription = name;
	setupCommand->mWorkingDirectory = executablePath;
	setupCommand->mLicenseRequired = true;
	setupCommand->mOptions = "{MOG_LockTracker}";
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_ShutdownClient()
{
	MOG_Command *setupCommand = new MOG_Command();
	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_ShutdownClient;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_ShutdownClient(MOG_Command *pRegisteredCommand)
{
	MOG_Command *setupCommand = new MOG_Command();
	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_ShutdownClient;
	setupCommand->mNetworkID = pRegisteredCommand->GetNetworkID();
	setupCommand->mComputerName = pRegisteredCommand->GetComputerName();
	setupCommand->mComputerIP = pRegisteredCommand->GetComputerIP();
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_RegisterEditor(String *name, String *executablePath)
{
	MOG_Command *setupCommand = new MOG_Command();
	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_RegisterEditor;
	setupCommand->mDescription = name;
	setupCommand->mWorkingDirectory = executablePath;
	setupCommand->mOptions = "{MOG_LockTracker}";
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_ShutdownEditor()
{
	MOG_Command *setupCommand = new MOG_Command();
	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_ShutdownEditor;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_ShutdownEditor(MOG_Command *pRegisteredCommand)
{
	MOG_Command *setupCommand = new MOG_Command();
	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_ShutdownEditor;
	setupCommand->mNetworkID = pRegisteredCommand->GetNetworkID();
	setupCommand->mComputerName = pRegisteredCommand->GetComputerName();
	setupCommand->mComputerIP = pRegisteredCommand->GetComputerIP();
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_RegisterCommandLine(String *name, String *executablePath)
{
	MOG_Command *setupCommand = new MOG_Command();
	
	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_RegisterCommandLine;
	setupCommand->mDescription = name;
	setupCommand->mWorkingDirectory = executablePath;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_ShutdownCommandLine()
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_ShutdownCommandLine;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_ShutdownCommandLine(MOG_Command *pRegisteredCommand)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_ShutdownCommandLine;
	setupCommand->mNetworkID = pRegisteredCommand->GetNetworkID();
	setupCommand->mComputerName = pRegisteredCommand->GetComputerName();
	setupCommand->mComputerIP = pRegisteredCommand->GetComputerIP();
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_RegisterSlave(String *name, String *executablePath)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_RegisterSlave;
	setupCommand->mDescription = name;
	setupCommand->mWorkingDirectory = executablePath;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_ShutdownSlave()
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_ShutdownSlave;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_ShutdownSlave(MOG_Command *pRegisteredCommand)
{
	return Setup_ShutdownSlave(pRegisteredCommand->GetNetworkID());
}


MOG_Command * MOG_CommandFactory::Setup_ShutdownSlave(int networkID)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_ShutdownSlave;
	setupCommand->mNetworkID = networkID;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_ShutdownSlave(String *computerName)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_ShutdownSlave;
	setupCommand->mComputerName = computerName;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_SQLConnection(String *connectionString)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_SQLConnection;
	setupCommand->mDescription = connectionString;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_MOGRepository(String *reposiotryPath)
{
	MOG_Command *setupCommand = new MOG_Command()
;
	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_MOGRepository;
	setupCommand->mWorkingDirectory = reposiotryPath;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_License()
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_License;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_LoginProject(String *loginProject, String *loginBranch)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_LoginProject;
	setupCommand->mProject = loginProject;
	setupCommand->mBranch = loginBranch;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_LoginUser(String *loginUser)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_LoginUser;
	setupCommand->mUserName = loginUser;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_ActiveViews(String *tabName, String *userName, String *platformName, String *workspaceDirectory)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_ActiveViews;
	setupCommand->mTab = tabName;
	setupCommand->mUserName = userName;
	setupCommand->mPlatform = platformName;
	setupCommand->mWorkingDirectory = workspaceDirectory;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_ViewUpdate(String *source, String *target, MOG_AssetStatusType status)
{
	return Setup_ViewUpdate(source, target, MOG_AssetStatus::GetText(status), NULL);
}

MOG_Command * MOG_CommandFactory::Setup_ViewUpdate(String *source, String *target, String *status, MOG_Properties *properties)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_ViewUpdate;
	setupCommand->mSource = source;
	setupCommand->mDestination = target;
	setupCommand->mDescription = status;

	// Check if we have a properties specified?
	if (properties)
	{
		// Get all of the asset's properties
		ArrayList *tempPropList = properties->GetPropertyList();

		// Build a string list of properties
		StringBuilder *tempPropString = new StringBuilder();
		for (int i = 0; i < tempPropList->Count; i++)
		{
			MOG_Property *tempProp = dynamic_cast<MOG_Property *>(tempPropList->Item[i]);

			// Skip all Files. properties
			if (tempProp->mSection->StartsWith(S"Files.", StringComparison::CurrentCultureIgnoreCase))
			{
				continue;
			}
			// Skip all relationships except package assignments because those need to be shown in the inbox columns
			if (tempProp->mSection->StartsWith(S"Relationships", StringComparison::CurrentCultureIgnoreCase) &&
				!tempProp->mPropertySection->StartsWith(S"Packages", StringComparison::CurrentCultureIgnoreCase))
			{
				continue;
			}

			// At some point, we just need to say enough is enough...10k of properies is way more than they deserve to receive for a view update notification
			if (tempPropString->Length < (10 * 1024))
			{
				// Proceed to add this property to our list to transmit
				tempPropString->Append(S"$");
				tempPropString->Append(tempProp->GetPropertyAsString());
			}
		}

		// Add the generated list of properties
		setupCommand->mVariables = tempPropString->ToString();
	}

	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_RefreshApplication()
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_RefreshApplication;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_RefreshTools()
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_RefreshTools;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_RefreshProject(String *projectName)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_RefreshProject;
	setupCommand->mProject = projectName;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_AssetRipRequest(String *jobLabel, MOG_Filename *assetFilename, String *validSlaves)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_AssetRipRequest;
	setupCommand->mJobLabel = jobLabel;
	setupCommand->mAssetFilename->SetFilename(assetFilename->GetEncodedFilename());
	setupCommand->mValidSlaves = validSlaves;
	setupCommand->mPreserveCommand = true;
	setupCommand->mLicenseRequired = true;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_AssetProcessed(int originatingNetworkID, String *originatingUserName, String *jobLabel, MOG_Filename *assetFilename, String *validSlaves)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_AssetProcessed;
	setupCommand->mNetworkID = originatingNetworkID;
	setupCommand->mUserName = originatingUserName;
	setupCommand->mJobLabel = jobLabel;
	setupCommand->mAssetFilename->SetFilename(assetFilename->GetEncodedFilename());
	setupCommand->mValidSlaves = validSlaves;
	setupCommand->mPreserveCommand = true;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_SlaveTask(int originatingNetworkID, String *originatingUserName, String *jobLabel, MOG_Filename *assetFilename, String *platformName, String *workingDirectory, String *ripper, ArrayList *environment, String *showWindow, String *source, String *destination, String *description, String *validSlaves)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Build a single string environment so we can contain it in the command
	String *flatEnvironmentString = S"";
	if (environment)
	{
		for (int i = 0; i < environment->Count; i++)
		{
			String *variable = __try_cast<String *>(environment->Item[i]);
			if (flatEnvironmentString->Length)
			{
				flatEnvironmentString = String::Concat(flatEnvironmentString, S",");
			}
			flatEnvironmentString = String::Concat(flatEnvironmentString, variable);
		}
	}

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_SlaveTask;
	setupCommand->mNetworkID = originatingNetworkID;
	setupCommand->mUserName = originatingUserName;
	setupCommand->mJobLabel = jobLabel;
	setupCommand->mAssetFilename->SetFilename(assetFilename);
	setupCommand->mPlatform = platformName;
	setupCommand->mWorkingDirectory = workingDirectory;
	setupCommand->mTool = ripper;
	setupCommand->mVariables = flatEnvironmentString;
	setupCommand->mOptions = showWindow;
	setupCommand->mSource = source;
	setupCommand->mDestination = destination;
	setupCommand->mDescription = description;
	setupCommand->mValidSlaves = validSlaves;
	setupCommand->mPreserveCommand = true;
	return setupCommand;
}


MOG_Command* MOG_CommandFactory::Setup_ReinstanceAssetRevision(MOG_Filename* newAssetRevisionFilename, MOG_Filename* oldAssetRevisionFilename, String* jobLabel, ArrayList* propertiesToRemove, ArrayList* propertiesToAdd, String* options)
{
	return Setup_ReinstanceAssetRevision(newAssetRevisionFilename, oldAssetRevisionFilename, jobLabel, propertiesToRemove, propertiesToAdd, options, NULL);
}

MOG_Command* MOG_CommandFactory::Setup_ReinstanceAssetRevision(MOG_Filename* newAssetRevisionFilename, MOG_Filename* oldAssetRevisionFilename, String* jobLabel, ArrayList* propertiesToRemove, ArrayList* propertiesToAdd, String* options, MOG_Filename* targetPackageFilename)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_ReinstanceAssetRevision;
	setupCommand->mAssetFilename->SetFilename(newAssetRevisionFilename->GetEncodedFilename());
	setupCommand->mJobLabel = jobLabel;
	setupCommand->mSource = oldAssetRevisionFilename->GetEncodedFilename();
	setupCommand->mPreserveCommand = true;
	setupCommand->mLicenseRequired = true;
	setupCommand->mOptions = options;
	// Check if there was a specific targetPackageFilename specified
	if (targetPackageFilename)
	{
		setupCommand->mDestination = targetPackageFilename->GetAssetFullName();
	}

	//use mVariables to send over the Properties To Remove/Add
	if(propertiesToRemove)
	{
		for(int i = 0; i < propertiesToRemove->Count; i++)
		{
			MOG_Property *propertyToAppend = __try_cast <MOG_Property*> (propertiesToRemove->Item[i]);
			if(setupCommand->mVariables->Length > 0)
			{
				setupCommand->mVariables = String::Concat(setupCommand->mVariables, S",");
			}
			setupCommand->mVariables = String::Concat(setupCommand->mVariables, S"REMOVE");	
			setupCommand->mVariables = String::Concat(setupCommand->mVariables, propertyToAppend->GetPropertyAsString());
		}
	}
	
	if(propertiesToAdd)
	{
		for(int i = 0; i < propertiesToAdd->Count; i++)
		{
			MOG_Property *propertyToAppend = __try_cast <MOG_Property*> (propertiesToAdd->Item[i]);
			if(setupCommand->mVariables->Length > 0)
			{
				setupCommand->mVariables = String::Concat(setupCommand->mVariables, S",");
			}
			setupCommand->mVariables = String::Concat(setupCommand->mVariables, S"ADD");
			setupCommand->mVariables = String::Concat(setupCommand->mVariables, propertyToAppend->GetPropertyAsString());
		}
	}
	return setupCommand;
}


MOG_Command* MOG_CommandFactory::Setup_Bless(MOG_Filename* assetFilename, String* jobLabel, String* validSlaves)
{
	return Setup_Bless(assetFilename, jobLabel, validSlaves, false, false);
}

MOG_Command* MOG_CommandFactory::Setup_Bless(MOG_Filename* assetFilename, String* jobLabel, String* validSlaves, bool skipPackaging, bool skipArchiving)
{
	return Setup_Bless(assetFilename, jobLabel, validSlaves, skipPackaging, skipArchiving, NULL);
}

MOG_Command* MOG_CommandFactory::Setup_Bless(MOG_Filename* assetFilename, String* jobLabel, String* validSlaves, bool skipPackaging, bool skipArchiving, MOG_Filename* targetPackageFilename)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_Bless;
	setupCommand->mAssetFilename->SetFilename(assetFilename->GetEncodedFilename());
	setupCommand->mJobLabel = jobLabel;
	setupCommand->mValidSlaves = validSlaves;
	setupCommand->mRemoveDuplicateCommands = true;
	setupCommand->mExclusiveCommand = true;
	setupCommand->mPreserveCommand = true;
	if(skipPackaging)
	{
		setupCommand->mOptions = String::Concat(setupCommand->mOptions, "|SkipPackaging|");
	}
	if(skipArchiving)
	{
		setupCommand->mOptions = String::Concat(setupCommand->mOptions, "|SkipArchiving|");
	}
	// Check if there was a specific targetPackageFilename specified
	if (targetPackageFilename)
	{
		setupCommand->mDestination = targetPackageFilename->GetAssetFullName();
	}

	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_RemoveAssetFromProject(MOG_Filename *assetFilename, String *comment, String *jobLabel, bool skipUnpackaging)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_RemoveAssetFromProject;
	setupCommand->mAssetFilename->SetFilename(assetFilename->GetEncodedFilename());
	setupCommand->mPreserveCommand = true;
	setupCommand->mDescription = comment;
	setupCommand->mJobLabel = jobLabel;
	if(skipUnpackaging)
	{
		setupCommand->mOptions = String::Concat(setupCommand->mOptions, "|SkipUnpackaging|");
	}
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_NetworkPackageMerge(String *platform, String *jobLabel, String *validSlaves, bool rejectAssets)
{
	return Setup_NetworkPackageMerge(NULL, platform, jobLabel, validSlaves, rejectAssets);
}

MOG_Command * MOG_CommandFactory::Setup_NetworkPackageMerge(String *packageName, String *platform, String *jobLabel, String *validSlaves, bool rejectAssets)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_NetworkPackageMerge;
	setupCommand->mAssetFilename->SetFilename(packageName);
	setupCommand->mPlatform = platform;
	setupCommand->mJobLabel = jobLabel;
	setupCommand->mValidSlaves = validSlaves;
	setupCommand->mRemoveDuplicateCommands = true;
	setupCommand->mExclusiveCommand = true;
	setupCommand->mPreserveCommand = true;
	if (rejectAssets)
	{
		setupCommand->mOptions = String::Concat(setupCommand->mOptions, "|RejectAssets|");
	}
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_LocalPackageMerge(String *workingDirectory, String *platformName)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_LocalPackageMerge;
	setupCommand->mWorkingDirectory = workingDirectory;
	setupCommand->mPlatform = platformName;
	setupCommand->mRemoveDuplicateCommands = true;
	setupCommand->mExclusiveCommand = true;
	setupCommand->mPreserveCommand = true;
	setupCommand->mLicenseRequired = true;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_EditorPackageMergeTask(String *inputFile, String *outputFile)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_EditorPackageMergeTask;
	setupCommand->mSource = inputFile;
	setupCommand->mDestination = outputFile;
	setupCommand->mRemoveDuplicateCommands = true;
	setupCommand->mExclusiveCommand = true;
	setupCommand->mPreserveCommand = true;
	setupCommand->mLicenseRequired = true;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_NetworkPackageRebuild(String *packageName, String *platform, String *jobLabel)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_NetworkPackageRebuild;
	setupCommand->mAssetFilename->SetFilename(packageName);
	setupCommand->mPlatform = platform;
	setupCommand->mJobLabel = jobLabel;
	setupCommand->mRemoveDuplicateCommands = true;
	setupCommand->mExclusiveCommand = true;
	setupCommand->mPreserveCommand = true;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_LocalPackageRebuild(String *packageName, String *platform, String *jobLabel)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_LocalPackageRebuild;
	setupCommand->mAssetFilename->SetFilename(packageName);
	setupCommand->mPlatform = platform;
	setupCommand->mJobLabel = jobLabel;
	setupCommand->mRemoveDuplicateCommands = true;
	setupCommand->mExclusiveCommand = true;
	setupCommand->mPreserveCommand = true;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_Post(String *jobLabel)
{
	return Setup_Post(jobLabel, false);
}

MOG_Command *MOG_CommandFactory::Setup_Post(MOG_Filename *assetFilename, String *jobLabel)
{
	MOG_Command *post = Setup_Post(jobLabel);
	post->mAssetFilename = assetFilename;
	return post;
}

MOG_Command * MOG_CommandFactory::Setup_Post(String *jobLabel, bool skipArchiving)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_Post;
	setupCommand->mJobLabel = jobLabel;
	setupCommand->mRemoveDuplicateCommands = true;
	setupCommand->mExclusiveCommand = true;
	setupCommand->mPreserveCommand = true;
	if(skipArchiving)
	{
		setupCommand->mOptions = String::Concat(setupCommand->mOptions, "|SkipArchiving|");
	}
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_Archive(String *path)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_Archive;
	setupCommand->mAssetFilename->SetFilename(path);
	setupCommand->mRemoveDuplicateCommands = true;
	setupCommand->mExclusiveCommand = true;
	setupCommand->mPreserveCommand = true;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_ScheduleArchive(MOG_Filename *assetName, String *jobLabel)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_ScheduleArchive;
	setupCommand->mAssetFilename->SetFilename(assetName->GetEncodedFilename());
	setupCommand->mRemoveDuplicateCommands = true;
	setupCommand->mExclusiveCommand = true;
	setupCommand->mPreserveCommand = true;
	setupCommand->mJobLabel = jobLabel;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_CleanTrash(String *userName, String *timeStamp)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_CleanTrash;
	setupCommand->mPreserveCommand = true;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_LockCopy(String *source, String *destination, String *description)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_LockCopy;
	setupCommand->mSource = source;
	setupCommand->mDestination = destination;
	setupCommand->mDescription = description;
	setupCommand->mPreserveCommand = true;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_LockMove(String *source, String *destination, String *description)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_LockCopy;
	setupCommand->mSource = source;
	setupCommand->mDestination = destination;
	setupCommand->mDescription = description;
	setupCommand->mPreserveCommand = true;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_LockReadRequest(String *lockName, String *description)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_LockReadRequest;
	setupCommand->mAssetFilename->SetFilename(lockName);
	setupCommand->mDescription = description;
	setupCommand->mPreserveCommand = true;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_LockReadRelease(String *lockName)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_LockReadRelease;
	setupCommand->mAssetFilename->SetFilename(lockName);
	setupCommand->mPreserveCommand = true;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_LockWriteRequest(String *lockName, String *description)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_LockWriteRequest;
	setupCommand->mAssetFilename->SetFilename(lockName);
	setupCommand->mDescription = description;
	setupCommand->mPreserveCommand = true;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_LockWriteRelease(String *lockName)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_LockWriteRelease;
	setupCommand->mAssetFilename->SetFilename(lockName);
	setupCommand->mPreserveCommand = true;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_LockReadQuery(String *lockName)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_LockReadQuery;
	setupCommand->mAssetFilename->SetFilename(lockName);
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_LockWriteQuery(String *lockName)
{
	MOG_Command *setupCommand = new MOG_Command();
	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_LockWriteQuery;
	setupCommand->mAssetFilename->SetFilename(lockName);
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_LockPersistentNotify(MOG_Command *lock, String* options)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_LockPersistentNotify;
	setupCommand->SetCommand(lock);
	setupCommand->mOptions = options;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_LockWatch(MOG_Command *lock)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_LockWatch;
	setupCommand->SetCommand(lock);
	setupCommand->mLicenseRequired = true;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_GetEditorInfo(String* workspaceDirectory)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_GetEditorInfo;
	setupCommand->mWorkingDirectory = workspaceDirectory;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_NetworkBroadcast(String *users, String *message)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_NetworkBroadcast;
	setupCommand->mOptions = users;
	setupCommand->mDescription = message;
	setupCommand->mPreserveCommand = true;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_NetworkBroadcast_EntireProject(String *project, String *message)
{
	MOG_Command *setupCommand = Setup_NetworkBroadcast("", message);

	// Populate the command information
	setupCommand->mProject = project;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_BuildFull(String *assetBuildName, String *source, String *target, String *validSlaves, bool force)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_BuildFull;
	setupCommand->mAssetFilename->SetFilename(assetBuildName);
	setupCommand->mSource = source;
	setupCommand->mDestination = target;
	setupCommand->mValidSlaves = validSlaves;
	if (force == true)
	{
		setupCommand->mDescription = "Force";
	}
	setupCommand->mPreserveCommand = true;
	setupCommand->mLicenseRequired = true;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_Build(String *executable, String *validSlaves, String *user, String *EnvVariables)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_Build;
	setupCommand->mAssetFilename->SetFilename(executable);
	setupCommand->mValidSlaves = validSlaves;
	setupCommand->mUserName = user;
	setupCommand->mDescription = EnvVariables;
	setupCommand->mPreserveCommand = true;
	setupCommand->mLicenseRequired = true;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_NewBranch(String *newBranchName, String *baseBranchName)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_NewBranch;
	setupCommand->mDestination = newBranchName;
	setupCommand->mSource = baseBranchName;
	setupCommand->mPreserveCommand = true;
	setupCommand->mLicenseRequired = true;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_InstantMessage(String *handle, String *senderUserName, String *projectName, String *invitedUsers, String *message)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_InstantMessage;
	setupCommand->mVersion = handle;
	setupCommand->mUserName = senderUserName;
	setupCommand->mProject = projectName;
	setupCommand->mSource = invitedUsers;
	setupCommand->mDescription = message;
	setupCommand->mPreserveCommand = true;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_RequestActiveCommands(void)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_RequestActiveCommands;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_NotifyActiveCommand(MOG_Command *pCommand)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_NotifyActiveCommand;
	setupCommand->mCommand = pCommand;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_RequestActiveLocks(void)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_RequestActiveLocks;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_NotifyActiveLock(MOG_Command *pCommand)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_NotifyActiveLock;
	setupCommand->mCommand = pCommand;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_RequestActiveConnections(void)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_RequestActiveConnections;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_NotifyActiveConnection(MOG_Command *pCommand)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_NotifyActiveConnection;
	setupCommand->mCommand = pCommand;
	return setupCommand;
}

MOG_Command * MOG_CommandFactory::Setup_Complete(MOG_Command *pCommand, bool bWasCommandCompleted)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_Complete;
	setupCommand->mPreserveCommand = true;

	// Propagate the complete state of the included command
	pCommand->SetCompleted(bWasCommandCompleted);
	// Save the passed-in within this completed command
	setupCommand->mCommand = pCommand;

	// Match the important elements of the encapsulated command
	setupCommand->mJobLabel = pCommand->GetJobLabel();
	setupCommand->mProject = pCommand->GetProject();
	setupCommand->mBranch = pCommand->GetBranch();
	setupCommand->mUserName = pCommand->GetUserName();
	setupCommand->mPlatform = pCommand->GetPlatform();
	setupCommand->mValidSlaves = pCommand->GetValidSlaves();

	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_Postpone(MOG_Command *pCommand, String *description)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_Postpone;
	setupCommand->mCommand = pCommand;
	setupCommand->mDescription = description;
	setupCommand->mPreserveCommand = true;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_Failed(MOG_Command *pCommand, String *description)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_Failed;
	setupCommand->mCommand = pCommand;
	setupCommand->mDescription = description;
	setupCommand->mPreserveCommand = true;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_NotifySystemAlert(String *title, String *message, String *stackTrace, bool bNotifyUser)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_NotifySystemAlert;
	setupCommand->mTitle = title;
	setupCommand->mDescription = message;
	setupCommand->mSource = stackTrace;
	setupCommand->mOptions = (bNotifyUser) ? S"NotifyUser=True" : S"";
	setupCommand->mPreserveCommand = true;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_NotifySystemError(String *title, String *message, String *stackTrace, bool bNotifyUser)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_NotifySystemError;
	setupCommand->mTitle = title;
	setupCommand->mDescription = message;
	setupCommand->mSource = stackTrace;
	setupCommand->mOptions = (bNotifyUser) ? S"NotifyUser=True" : S"";
	setupCommand->mPreserveCommand = true;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_NotifySystemException(String *title, String *message, String *stackTrace, bool bNotifyUser)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_NotifySystemException;
	setupCommand->mTitle = title;
	setupCommand->mDescription = message;
	setupCommand->mSource = stackTrace;
	setupCommand->mOptions = (bNotifyUser) ? S"NotifyUser=True" : S"";
	setupCommand->mPreserveCommand = true;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_AutomatedTesting(String *testName, String *projectName, String *testImportFile, int testDuration_Seconds, bool bCopyFileLocal, int startingExtensionIndex)
{
	MOG_Command *setupCommand = new MOG_Command();;

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_AutomatedTesting;
	setupCommand->mTitle = testName;
	setupCommand->mProject = projectName;
	setupCommand->mSource = testImportFile;
	setupCommand->mVersion = testDuration_Seconds.ToString();
	setupCommand->mVariables = startingExtensionIndex.ToString();
	if (bCopyFileLocal)
	{
		setupCommand->mOptions= String::Concat(setupCommand->mOptions, S"|CopyFileLocal|");
	}
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_RetaskCommand(int slaveNetworkID)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_RetaskCommand;
	setupCommand->mNetworkID = slaveNetworkID;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_KillCommand(UInt32 commandID)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_KillCommand;
	setupCommand->mCommandID = commandID;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_LaunchSlave(int clientNetworkID)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_LaunchSlave;
	setupCommand->mNetworkID = clientNetworkID;
	return setupCommand;
}


MOG_Command * MOG_CommandFactory::Setup_StartJob(String *jobLabel)
{
	MOG_Command *setupCommand = new MOG_Command();

	// Populate the command information
	setupCommand->mCommandType = MOG_COMMAND_StartJob;
	setupCommand->mJobLabel = jobLabel;
	return setupCommand;
}

