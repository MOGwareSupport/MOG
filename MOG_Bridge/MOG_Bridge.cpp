// This is the main DLL file.  We can put whatever managed C++ stuff in here that we want

// MOG_Bridge has MOG_BridgeAPI.h in it
#include "MOG_Bridge.h"
#include "MOG_BridgeHandle.h"
#include "MOG_BridgeContainer.h"

#include <cstring>

using namespace System;
using namespace System::IO;
using namespace System::Collections;
using namespace System::Windows::Forms;
using namespace System::Runtime::InteropServices;

using namespace MOG;
using namespace MOG::BRIDGE;
using namespace MOG::CONTROLLER::CONTROLLERASSET;
using namespace MOG::CONTROLLER::CONTROLLERPROJECT;
using namespace MOG::CONTROLLER::CONTROLLERPACKAGE;
using namespace MOG::CONTROLLER::CONTROLLERSYNCDATA;
using namespace MOG::CONTROLLER::CONTROLLERSYSTEM;
using namespace MOG::CONTROLLER::CONTROLLERINBOX;
using namespace MOG::CONTROLLER::CONTROLLERREPOSITORY;
using namespace MOG::DATABASE;
using namespace MOG::FILENAME;
using namespace MOG::REPORT;
using namespace MOG::DOSUTILS;
using namespace MOG::PROMPT;
using namespace MOG::TIME;

using namespace MOG_ControlsLibrary;
using namespace MOG_ControlsLibrary::Forms;
using namespace MOG_ControlsLibrary::Forms::Wizards;


extern "C"
{
	// Export a function our users can call to 
	//  Get a MOG_Bridge*
	MOG_Export MOG_BridgeAPI* GetMOGBridge()
	{
		return new MOG_Bridge();
	} 

} // End extern "C"



MOG_Bridge::MOG_Bridge()
{ 
	mBaseHandle = MOG_BridgeContainer::RegisterContainer( this );
	mIsInitialized = false;
}

// Destroy MOG_Bridge by shutting down 
//  its __gc components in BridgeContainer
MOG_Bridge::~MOG_Bridge()
{
	MOG_BridgeContainer::GetContainer( mBaseHandle)->Shutdown();
}

//
//
// Shutdown the managed components attached
//	to Bridge through BridgeContainer
void MOG_Bridge::Shutdown( void )
{
	//Make sure to log us out or something
	MOG_ControllerProject::Logout();
	
	if(mIsInitialized)
	{
		MOG_BridgeContainer::GetContainer( mBaseHandle)->Shutdown();
	}

	//MOG_BridgeContainer::DestroyBridge( mBaseHandle );
}

//
//
// Returns true if we were able to initialize a client,
//	Else, returns false.
bool MOG_Bridge::InitializeClient( const char* name, bool bUnused )
{
	try
	{
		Control::CheckForIllegalCrossThreadCalls = false;

		if(mIsInitialized)
			return false;

		// Register our Bridge now so we can respond with callbacks during the initialization
		MOG_BridgeContainer::RegisterBridge(this);

		String __gc* pName = new String(name);
		MOG_Main::Init_Client(S"", pName);

		mIsInitialized = true;
		return true;
	}
	catch( Exception* e )
	{
		e->ToString();
		return false;
	}
}

// Overload of InitializeClient 

// Returns true if we were able to initialize a CommandLine,
//  Else, returns false.
bool MOG_Bridge::InitializeCommandLine( const char* name, bool bUnused )
{
	try
	{
		Control::CheckForIllegalCrossThreadCalls = false;
		
		if(mIsInitialized)
			return false;

		// Register our Bridge now so we can respond with callbacks during the initialization
		MOG_BridgeContainer::RegisterBridge(this);

		String __gc* pName = new String(name);
		MOG_Main::Init_CommandLine(S"", pName);

		mIsInitialized = true;
		return true;
	}
	catch( Exception* e)
	{
		e->ToString();
		return false;
	}
}

// Returns true if we were able to initialize an editor,
//  Else, returns false.
bool MOG_Bridge::InitializeEditor( const char* name, bool bUnused )
{
	try
	{
		Control::CheckForIllegalCrossThreadCalls = false;

		if (mIsInitialized)
			return false;

		// Register our Bridge now so we can respond with callbacks during the initialization
		MOG_BridgeContainer::RegisterBridge( this);

		// MOG_Main::Init_Editor(S"") occurs inside this command
		String __gc* pName = new String(name);
		if (MOG_BridgeContainer::GetContainer( mBaseHandle )->InitializeEditor(name))
		{
			mIsInitialized = true;
			return true;
		}
	}
	catch(Exception* e)
	{
		e->ToString();
		return false;
	}
	
	return false;
}

// Returns true if we were able to initialize an editor,
//  Else, returns false.
void MOG_Bridge::SetAssociatedWorkspacePath( const char* path )
{
	// Inform MOG_Main about our alternative associated path
	String __gc* pPath = new String(path);
	MOG_Main::SetAssociatedWorkspacePath(pPath);
}

//
//
// This function must be ticked on each process loop if no thread was created in the Initialize functions
bool MOG_Bridge::ProcessTick( void )
{
	try
	{
		return MOG_BridgeContainer::GetContainer(mBaseHandle)->MogProcess();
	}
	catch(...)
	{
		return false;
	}
}


//
//
// Provided CommandHandler is assigned, we call it, returning it's result.
//	Else, return false
bool MOG_Bridge::HandleCommand( MOGAPI_BaseInfo* pCommandInfo )
{
	// If we have a valid command handler, run it.
	if(mCommandHandler)
	{
		return mCommandHandler( pCommandInfo );
	}
	return false;
}

//  Provided EventHandler is assigned, we call it, 
//	 Else, do nothing
void MOG_Bridge::HandleEvent( MOGAPI_BaseInfo* pEventInfo )
{
	// If we have a valid event handler, run it.
	if(mEventHandler)
	{
		mEventHandler( pEventInfo );
	}
}

//
//
//
// Takes Handler and assigns it to CommandHandler
void MOG_Bridge::RegisterCommandHandler( bool (Handler)( MOGAPI_BaseInfo* pCommandType ) )
{
	mCommandHandler = Handler;
}

// Takes Handler and assigns it to EventHandler
void MOG_Bridge::RegisterEventHandler(void (Handler)( MOGAPI_BaseInfo* pEventType ), unsigned int iEventFlags )
{
	MOG_BridgeContainer::GetContainer( mBaseHandle )->SetEventFlags( iEventFlags );
	mEventHandler = Handler;
}

void MOG_Bridge::SetEventHandlerFlags( unsigned int iFlags )
{
	try
	{
		MOG_BridgeContainer::GetContainer( mBaseHandle )->SetEventFlags( iFlags );
	}
	catch( Exception* e )
	{
		e->ToString();
	}
}

unsigned int MOG_Bridge::GetEventHandlerFlags( void )
{
	try
	{
		return MOG_BridgeContainer::GetContainer( mBaseHandle )->GetEventFlags();
	}
	catch( Exception* e )
	{
		e->ToString();
		return 0;
	}
}

//
//
//
// Returns true if we are able to release the lock
bool MOG_Bridge::PersistentLock_Release(const char* pAssetName, LockInfo* info)
{
	try
	{
		// Make sure LockInfo is valid
		if(!info)
			info = new LockInfo;

		// Convert char* pAssetName into a String*
		String __gc* pAssetNameStr = new String(pAssetName);

		if(pAssetNameStr->Length > 0)
		{
			if(!MOG_ControllerProject::IsProject())
			{
				info->LockStatus = MOG_BridgeAPI::LockInfo::LockInfo_ClientNotLoggedIn;
				return false;
			}

			// Try to do a PersistentLock_Release
			try
			{
				info->LockStatus = MOG_BridgeAPI::LockInfo::LockInfo_Successful;
				return MOG_ControllerProject::PersistentLock_Release(pAssetNameStr);
			}
			catch(System::Exception* e)
			{
				info->LockStatus = MOG_BridgeAPI::LockInfo::LockInfo_FatalError;
				NotifySystemError( "ERROR: Lock Release Fatal Error", e->ToString(), e->StackTrace );
				return false;
			}
		}
		else
		{
			info->LockStatus = MOG_BridgeAPI::LockInfo::LockInfo_AssetDoesNotExist;
			NotifySystemError( "ERROR: Lock Release Asset Does Not Exist", pAssetName, Environment::StackTrace );
			return false;
		}
	}
	catch( Exception* e )
	{
		NotifySystemError( "ERROR: Lock Release Error", e->ToString(), e->StackTrace );
		return false;
	}
}

// Returns char* of command result
bool MOG_Bridge::PersistentLock_Request(const char* pAssetName, const char* pDescription, LockInfo* info)
{
	try
	{
		// Make sure LockInfo is valid
		if(!info) info = new LockInfo;
		if(!pDescription) pDescription = "";

		// If we are not logged into a project, return false
		if(!MOG_ControllerProject::IsProject())
		{
			info->LockStatus = MOG_BridgeAPI::LockInfo::LockInfo_ClientNotLoggedIn;
			return false;
		}

		// Convert char* pAssetName into a String*, then resolve it
		String* pAssetNameStr = new String(pAssetName);

		if(pAssetNameStr->Length > 0)
		{
			// Convert description to a String*
			String* pDescriptionStr = new String(pDescription);

			// Try to do a PersistentLock_Release
			try
			{
				MOG_Command* pCommand = MOG_ControllerProject::PersistentLock_Request(pAssetNameStr, pDescriptionStr);
				MOG_Command* pObstructingCommand = pCommand->GetCommand();
				if( pObstructingCommand )
				{
					PopulateLockInfo( pObstructingCommand, info );
					return false;
				}
				info->LockStatus = LockInfo::LockInfo_Successful;
				return true;
			}
			catch(System::Exception* e)
			{
				info->LockStatus = MOG_BridgeAPI::LockInfo::LockInfo_FatalError;
				NotifySystemError( "ERROR: Lock Request Fatal Error", e->ToString(), e->StackTrace );
				return false;
			}
		}
		else
		{
			// If we did not find that the server was not present...
			if(info->LockStatus != LockInfo::LockInfo_ServerNotPresent)
			{
				// Then the asset does not exist
				info->LockStatus = LockInfo::LockInfo_AssetDoesNotExist;
				NotifySystemError( "ERROR: Lock Request Asset Does Not Exist", pAssetName, Environment::StackTrace );
			}
			return false;
		}
	}
	catch( Exception* e )
	{
		NotifySystemError( "ERROR: Lock Request Error", e->ToString(), e->StackTrace );
		return false;
	}
}

bool MOG_Bridge::PersistentLock_Query(const char* pAssetName, LockInfo* info)
{
	try
	{
		// Make sure LockInfo is valid
		if(!info)
			info = new LockInfo;

		// Set this to a default status
		info->LockStatus = MOG_BridgeAPI::LockInfo::LockInfo_Default;

		// If we are not logged into a project, return false
		if(!MOG_ControllerProject::IsProject())
		{
			info->LockStatus = MOG_BridgeAPI::LockInfo::LockInfo_ClientNotLoggedIn;
			return false;
		}

		// Convert char* pAssetName into a String*
		String* pAssetNameStr = new String(pAssetName);

		// Try to do a PersistentLock_Query
		try
		{
			// Look to see if this is an asset

			// If this is an asset...
			if(pAssetNameStr->Length > 0)
			{
				// Execute command
				MOG_Command* pCommand = MOG_ControllerProject::PersistentLock_Query(pAssetNameStr);

				// Check to see if there is an obstructing lock
				MOG_Command* pObstructingLockCommand = pCommand->GetCommand();
				if(pObstructingLockCommand)
				{
					PopulateLockInfo( pObstructingLockCommand, info );
					return true;
				}

				// If we did not set a status already...
				if(info->LockStatus != MOG_BridgeAPI::LockInfo::LockInfo_MoreThanOneAssociatedAssetFound)
				{
					// Set Successful
					info->LockStatus = MOG_BridgeAPI::LockInfo::LockInfo_Successful;
				}
				// Else, return true
				return false;
			}
			else
			{
				// If we did not find that the server was not present...
				if(info->LockStatus != MOG_BridgeAPI::LockInfo::LockInfo_ServerNotPresent)
					// Then the asset does not exist
					info->LockStatus = MOG_BridgeAPI::LockInfo::LockInfo_AssetDoesNotExist;
				return false;
			}
		}
		catch(System::Exception* e)
		{
			e->ToString();
			return false;
		}
	}
	catch( Exception* e )
	{
		e->ToString();
		return false;
	}
}

void MOG_Bridge::PopulateLockInfo( MOG_Command* pCommand, LockInfo* info )
{
	// Populate locked by name
	CopyManagedStringToNative(info->AlreadyLockedBy, pCommand->GetUserName(), MAX_ALREADY_LOCKED_BY_Name);
	
	// Populate locked by time
	CopyManagedStringToNative(info->AlreadyLockedByTime, pCommand->GetCommandTimeStamp(), MAX_ALREADY_LOCKED_BY_Time);

	// Populate locked by description
	CopyManagedStringToNative(info->AlreadyLockedByDescription, pCommand->GetDescription(), MAX_ALREADY_LOCKED_BY_Description);
	
	info->LockStatus = MOG_BridgeAPI::LockInfo::LockInfo_ObstructingLockFound;
}

//
//Used for warning user of recoverable issues
bool MOG_Bridge::NotifySystemAlert(IN const char* pTitle, IN const char* pMessage, IN const char* pStackTrace)
{
//?	MOG_Bridge::NotifySystemException - This API should be expanded to support the title and stackTrace
	// Make sure to inform the Server about this
	return MOG_Report::ReportMessage(pTitle, pMessage, pStackTrace, MOG_ALERT_LEVEL::ALERT);
}

//
//User errors that alert the server
bool MOG_Bridge::NotifySystemError(IN const char* pTitle, IN const char* pMessage, IN const char* pStackTrace)
{
//?	MOG_Bridge::NotifySystemException - This API should be expanded to support the title and stackTrace
	// Make sure to inform the Server about this
	return MOG_Report::ReportMessage(pTitle, pMessage, pStackTrace, MOG_ALERT_LEVEL::ERROR);
}

//
//System errors, not recoverable
bool MOG_Bridge::NotifySystemException(IN const char* pTitle, IN const char* pMessage, IN const char* pStackTrace)
{
//?	MOG_Bridge::NotifySystemException - This API should be expanded to support the title and stackTrace
	// Make sure to inform the Server about this
	return MOG_Report::ReportMessage(pTitle, pMessage, pStackTrace, MOG_ALERT_LEVEL::CRITICAL);
}

bool MOG_Bridge::NotifySystemAlert(IN String* pTitle, IN String* pMessage, IN String* pStackTrace)
{
	return MOG_Report::ReportMessage(pTitle, pMessage, pStackTrace, MOG_ALERT_LEVEL::ALERT);
}

bool MOG_Bridge::NotifySystemError(IN String* pTitle, IN String* pMessage, IN String* pStackTrace)
{
	return MOG_Report::ReportMessage(pTitle, pMessage, pStackTrace, MOG_ALERT_LEVEL::ERROR);
}

bool MOG_Bridge::NotifySystemException(IN String* pTitle, IN String* pMessage, IN String* pStackTrace)
{
	return MOG_Report::ReportMessage(pTitle, pMessage, pStackTrace, MOG_ALERT_LEVEL::CRITICAL);
}

bool MOG_Bridge::AddLateResolvers( IN const char* PackageName, IN const char* LinkedPackage)
{
	return MOG_ControllerPackageMergeNetwork::AddLateResolvers(PackageName, LinkedPackage);
}


//
//
//
// If we have an active project, returns the name of the user through passed-in variable, *userName.
bool MOG_Bridge::GetUserName(char* userName)
{
	try
	{
		// Make sure we are logged in to a project?
		if (MOG_ControllerProject::IsProject())
		{
			// Make sure we are logged in as a user?
			if (MOG_ControllerProject::IsUser())
			{
				// Obtain the user's name
				CopyManagedStringToNative(userName, MOG_ControllerProject::GetUserName(), MAX_MOG_CharArrayLength);
				return true;
			}
		}
	}
	catch( Exception* e )
	{
		e->ToString();
	}

	return false;
}

// Same as GetUserName(char*), except returns branch name through *branchName
bool MOG_Bridge::GetBranchName(char* branchName)
{
	try
	{
		// Make sure we are logged in to a project?
		if (MOG_ControllerProject::IsProject())
		{
			CopyManagedStringToNative(branchName, MOG_ControllerProject::GetBranchName(), MAX_MOG_CharArrayLength);
			return true;
		}
	}
	catch( Exception* e )
	{
		e->ToString();
	}

	return false;
}

// Same as GetUserName(char*), except returns project name through *projectName
bool MOG_Bridge::GetProjectName(char* projectName)
{
	try
	{
		// Make sure we are logged in to a project?
		if (MOG_ControllerProject::IsProject())
		{
			CopyManagedStringToNative(projectName, MOG_ControllerProject::GetProjectName(), MAX_MOG_CharArrayLength);
			return true;
		}
	}	
	catch( Exception* e )
	{
		e->ToString();
	}

	return false;
}

bool MOG_Bridge::GetLocalWorkspacePlatform(char* localWorkspacePlatform)
{
	try
	{
		// Make sure we are logged in to a project?
		if (MOG_ControllerProject::IsProject())
		{
			// Make sure we have a local workspace?
			MOG_ControllerSyncData* sync = MOG_ControllerProject::GetCurrentSyncDataController();
			if (sync)
			{
				CopyManagedStringToNative(localWorkspacePlatform, sync->GetPlatformName(), MAX_MOG_CharArrayLength);
				return true;
			}
		}
	}	
	catch( Exception* e )
	{
		e->ToString();
	}

	return false;
}

bool MOG_Bridge::GetLocalWorkspace(char* localWorkspace)
{
	try
	{
		// Make sure we are logged in to a project?
		if (MOG_ControllerProject::IsProject())
		{
			// Make sure we have a local workspace?
			MOG_ControllerSyncData* sync = MOG_ControllerProject::GetCurrentSyncDataController();
			if (sync)
			{
				CopyManagedStringToNative(localWorkspace, sync->GetSyncDirectory(), MAX_MOG_CharArrayLength);
				return true;
			}
		}
	}	
	catch( Exception* e )
	{
		e->ToString();
	}

	return false;
}

//
//
//
// Imports asset using MOG_Main::ImportAsset(...)
bool MOG_Bridge::ImportAsset(const char* pAssetName, 
							 const char* pImportFiles[],
							 int importFilesLen,
							 const char* pLogFiles[],
							 int logFilesLen, 
							 const MOGAPI_Property* pProperties[],
							 int propertiesLen,
							 bool bDeleteImportFilesAfterImport,
							 bool bDeleteLogFilesAfterImport)
{
	try
	{
		MOG_Filename* newFilename;
		ArrayList* importFiles = new ArrayList();
		ArrayList* logFiles = new ArrayList();
		ArrayList* properties = new ArrayList();

		if(!MOG_ControllerProject::IsProject())
		{
			return false;
		}

		String* pFilenameStr = new String(pAssetName);

		// Populate importFiles
		for (int i = 0; i < importFilesLen; ++i)
		{
			importFiles->Add( new String(pImportFiles[i]) );
		}
		// Populate logFiles
		for (int i = 0; i < logFilesLen; ++i)
		{
			logFiles->Add( new String(pLogFiles[i]) );
		}
		// Populate properties
		for (int i = 0; i < propertiesLen; ++i)
		{
			String* Section;
			String* PropertySection;
			String* PropertyKey;
			String* PropertyValue;

			Section = new String( pProperties[i]->mSection );
			PropertySection = new String( pProperties[i]->mPropertySection );
			PropertyKey = new String( pProperties[i]->mPropertyKey );
			PropertyValue = new String( pProperties[i]->mPropertyValue );
			properties->Add( new MOG_Property( Section, PropertySection, PropertyKey, PropertyValue )  );
		}

		// Check if there are any special properties being sent in with this asset?
		if (properties->Count)
		{
			// fix up the specified properties with syntactically correct values
			properties = MOG_ControlsLibrary::Utils::PropertyHelper::RepairPropertyList(properties);
			if (!properties || properties->Count == 0)
			{
				return false;
			}
		}

		// Proceed to create the asset
		newFilename = MOG_ControllerAsset::CreateAsset( pFilenameStr,
														S"",
														importFiles,
														logFiles,
														properties,
                                                        bDeleteImportFilesAfterImport, 
														bDeleteLogFilesAfterImport);

		if(newFilename)
			return true;
		
		return false;
	}
	catch( Exception* e )
	{
		NotifySystemError( "ERROR: Import Asset", e->ToString(), e->StackTrace );
		return false;
	}
} // end ImportAsset(...)


//
//
//
// Adds the asset to the pending imports
bool MOG_Bridge::AddPendingImport(const char* pFilename, 
								  const char* pAssetLabel, 
								  const char* pPackageFile, 
								  const char* pPackageGroupName,
								  const char* pPackageObjectName,
								  const char* pLOD,
								  bool bExportedFromEditor)
{
	// Make sure we transfer the data over to our memory
	String* myFilename = new String(pFilename);
	String* myAssetLabel = new String(pAssetLabel);
	String* myPackageFile = new String(pPackageFile);
	String* myPackageGroupName = new String(pPackageGroupName);
	String* myPackageObjectName = new String(pPackageObjectName);
	String* myLOD = new String(pLOD);
	// Check if we have a specified LOD?
	if (myLOD->Length)
	{
		// Check if myAssetLabel is identical to myPackageObject?
		if (String::Compare(myAssetLabel, myPackageObjectName, true) == 0)
		{
			// Check if this is a real LOD?
			int LOD = Convert::ToInt32(myLOD);
			if (LOD > 0)
			{
				// We need to do something to change this name or else this LOD will collide with base LOD asset
				myAssetLabel = MOG_ControllerProject::GenerateLODName(myAssetLabel, myLOD);
			}
		}
	}

	// Make sure we are logged into a project
	if(MOG_ControllerProject::IsProject())
	{
		// Create a new PendingImport object
		MOG_PendingImport* pendingImport = new MOG_PendingImport();
		// Populate the new pendingImport
		pendingImport->mFilename = myFilename;
		pendingImport->mAssetLabel = myAssetLabel;
		pendingImport->mAssetPlatform = S"All";
		pendingImport->mFinalStatus = MOG_AssetStatusType::Imported;	// Imported denotes something from the editor
		// Check if we have a packageFile?
		if (myPackageFile->Length)
		{
			// Setup our package assignment
			MOG_Property* packageAssignment = MOG_PropertyFactory::MOG_Relationships::New_PackageAssignment(myPackageFile, myPackageGroupName, myPackageObjectName);
			pendingImport->mProperties->Add(packageAssignment);
		}
		// Check if there was an LOD specified?
		if (myLOD->Length)
		{
			// Setup our LOD property
			MOG_Property* lod = new MOG_Property(S"Properties", S"Custom", S"LOD", myLOD);
			pendingImport->mProperties->Add(lod);
		}
		// Check if this is being exported from the editor?
		if (bExportedFromEditor)
		{
			// Setup our asset's state to Imported which will skip the auto update local workspace
			MOG_Property* status = MOG_PropertyFactory::MOG_Asset_StatsProperties::New_Status(MOG_AssetStatusType::Imported);
			pendingImport->mProperties->Add(status);
		}
		// Check if there are any special properties being sent in with this asset?
		if (pendingImport->mProperties->Count)
		{
			// fix up the specified properties with syntactically correct values
			pendingImport->mProperties = MOG_ControlsLibrary::Utils::PropertyHelper::RepairPropertyList(pendingImport->mProperties);
		}

		// Add this pendingImport to our list of pendingImports
		MOG_PendingImport::GetPendingImportList()->Add(pendingImport);
		return true;
	}
	else
	{
		String* message = String::Concat(	S"MOG can only import assets when the application is logged into a project.\n",
											S"FILE: ", myFilename);
		MOG_Prompt::PromptMessage("MOG Import Failed", message);
	}

	return false;
}


//
//
//
// Performs all the pending imports
bool MOG_Bridge::DoPendingImports()
{
	bool bFailed = false;
	bool bImported = false;

	try
	{
		// Make sure we are logged into a project
		if(MOG_ControllerProject::IsProject())
		{
			bool bAskToCancelAll = false;
			bool bAskToCancelAll_NoToAll = false;
			bool bApplyToAll = false;
			String* allAssetClassification = S"";
			String* allAssetPlatform = S"";
			bool allAssetLabelIncludeExtension = false;
			ArrayList* allAssetProperties = NULL;

			for (int i = 0; i < MOG_PendingImport::GetPendingImportList()->Count; i++)
			{
				MOG_PendingImport* pendingImport = __try_cast<MOG_PendingImport*>(MOG_PendingImport::GetPendingImportList()->Item[i]);

				MOG_Filename* assetVerifedName = NULL;

				// Always assume the specified asset label
				String* assetLabel = pendingImport->mAssetLabel;
				// Check if this they have specified to use extensions?
				if (allAssetLabelIncludeExtension)
				{
					// Append on the filename's extension to the specified asset label
					assetLabel = String::Concat(assetLabel, Path::GetExtension(pendingImport->mFilename));
				}

				// Create import source files array
				ArrayList* importFiles = new ArrayList();
				importFiles->Add(pendingImport->mFilename);

				// Attempt to detect this filename name?
				ArrayList* detectedAssetNames = MOG_ControllerProject::MapFilenameToAssetName(pendingImport->mFilename, MOG_ControllerProject::GetPlatformName(), MOG_ControllerProject::GetWorkspaceDirectory());
				if (detectedAssetNames && detectedAssetNames->Count == 1)
				{
					// Use the newly detected name
					assetVerifedName = __try_cast<MOG_Filename*>(detectedAssetNames->Item[0]);
				}
				else
				{
					// Check if the user has already said to apply to all?
					if (bApplyToAll == true)
					{
						assetVerifedName = MOG_Filename::CreateAssetName(allAssetClassification, allAssetPlatform, assetLabel);
						pendingImport->mProperties = allAssetProperties;
					}
					else
					{
						// Looks like we need user intervention...
						ImportAssetWizard* wizard = new ImportAssetWizard();
						// Set the screeen position
						wizard->StartPosition = FormStartPosition::CenterScreen;

						// Set the wizard startup variables
						wizard->ImportSourceFilename = pendingImport->mFilename;
						wizard->ImportPotentialMatches = detectedAssetNames;
						wizard->ImportHasMultiples = (MOG_PendingImport::GetPendingImportList()->Count > 1) ? true : false;

						// Seed the wizard with what we already know
						wizard->Seed_AssetClassification = pendingImport->mAssetClassification;
						wizard->Seed_AssetLabel = assetLabel;
						wizard->Seed_AssetPlatform = pendingImport->mAssetPlatform;
						wizard->Seed_AssetProperties = pendingImport->mProperties;

						// Present the user the wizard
						DialogResult result = wizard->ShowDialog(NULL);
						if (result == DialogResult::Cancel)
						{
							bAskToCancelAll = true;
						}
						else if (result == DialogResult::OK)
						{
							// Transfer over variables from the wizard
							assetVerifedName = new MOG_Filename(wizard->ImportEndMogTextBox->Text);
							pendingImport->mProperties->AddRange(wizard->ImportPropertyArray);		// AddRange so the original properties are preserved

							// Check if the indicated this applied to all?
							if (wizard->ImportMultipleApplyToAll)
							{
								bApplyToAll = true;
								// Get the currrent settings for all subsequent assets
								allAssetClassification = assetVerifedName->GetAssetClassification();
								allAssetPlatform = assetVerifedName->GetAssetPlatform();
								allAssetProperties = pendingImport->mProperties;
								allAssetLabelIncludeExtension = wizard->ImportShowExtension;
							}
						}
						else
						{
							// Do Advanced import dialog
							ImportAssetTreeForm* importForm = new ImportAssetTreeForm(new ImportFile(pendingImport->mFilename));
							// Set the screen position
							importForm->StartPosition = FormStartPosition::CenterScreen;

							// Seed the wizard with what we already know
							importForm->Seed_AssetClassification = pendingImport->mAssetClassification;
							importForm->Seed_AssetLabel = assetLabel;
							importForm->Seed_AssetPlatform = pendingImport->mAssetPlatform;
							importForm->Seed_AssetProperties = pendingImport->mProperties;

							DialogResult rc = importForm->ShowDialog();
							if (rc == DialogResult::Cancel)
							{
								bAskToCancelAll = true;
							}
							else
							{
								// Construct the new name based on the dialog fields
								String* newName = importForm->GetFixedAssetName();

								pendingImport->mProperties->AddRange(importForm->MOGPropertyArray);		// AddRange so the original properties are preserved
								assetVerifedName = new MOG_Filename(newName);

								// Result of Yes means 'OK to All'
								if (rc == DialogResult::Yes)
								{
									bApplyToAll = true;
									// Get the currrent settings for all subsequent assets
									allAssetClassification = assetVerifedName->GetAssetClassification();
									allAssetPlatform = assetVerifedName->GetAssetPlatform();
									allAssetProperties = pendingImport->mProperties;
									allAssetLabelIncludeExtension = importForm->MOGShowExtensions;
								}
							}
						}
					}
				}

				// Check if we got our verified asset name?
				if (assetVerifedName)
				{
					// Import it according to the wizard settings
					try
					{
						// Import this asset with its properties
						if (MOG_ControllerAsset::CreateAsset(assetVerifedName->GetAssetFullName(), S"", importFiles, NULL, pendingImport->mProperties, false, false, false))
						{
							bImported = true;
						}
						else
						{
							String* message = String::Concat(	S"Failed to import the file.\n",
																S"File: ", pendingImport->mFilename);
							MOG_Report::ReportMessage(S"Import Failed", message, S"", MOG_ALERT_LEVEL::ERROR);
							bFailed = true;
							bAskToCancelAll = true;
						}
					}
					catch (Exception* e)
					{
						String* message = String::Concat(	S"There was an unhandled exception when trying to import the file.\n",
															S"File: ", pendingImport->mFilename, S"\n\n",
															S"Exception:\n",
															e->Message);
						MOG_Report::ReportMessage(S"Unhandled Exception", message, e->StackTrace, MOG_ALERT_LEVEL::CRITICAL);
						bFailed = true;
						bAskToCancelAll = true;
					}
				}
				else
				{
					bAskToCancelAll = true;
				}

				// Should we check about canceling the remaing imports?
				if (bAskToCancelAll &&
					bAskToCancelAll_NoToAll == false &&
					MOG_PendingImport::GetPendingImportList()->Count > 1)
				{
					// Ask the user if they want to cancel all remaining imports?
					MOGPromptResult result = MOG_Prompt::PromptResponse("Cancel All", "Do you want to cancel all remaining imports?", MOGPromptButtons::YesNoYesToAllNoToAll);
					if (result == MOGPromptResult::Yes ||
						result == MOGPromptResult::YesToAll)
					{
						break;
					}
					else if (result == MOGPromptResult::NoToAll)
					{
						bAskToCancelAll_NoToAll = true;
					}
				}
			}
		}
	}
	catch( Exception* e )
	{
		String* message = String::Concat(	S"There was an unhandled exception when trying to import the files.\n",
											S"Exception:\n",
											e->Message);
		MOG_Report::ReportMessage(S"Unhandled Exception", message, e->StackTrace, MOG::PROMPT::MOG_ALERT_LEVEL::CRITICAL);
		bFailed = true;
	}

	// Always make sure we clear out our pending imports
	MOG_PendingImport::GetPendingImportList()->Clear();

	// Check if we failed?
	if (!bFailed)
	{
		return true;
	}
	return false;
} // end ImportAssetUsingWizard(...)

//
//
//
// NOT IMPLEMENTED
char* MOG_Bridge::GetPermissions(void)
{
	//glk: This needs to get the BASE and use it.
	return "NotImplemented";
}

// NOT IMPLEMENTED
void MOG_Bridge::SetPermissions(const char* pPermissions)
{
	//DoSomething
}

void MOG_Bridge::EnableSilentMode(bool enable)
{
	if (enable)
	{
		MOG_Prompt::SetMode(MOG_PROMPT_MODE_TYPE::MOG_PROMPT_SILENT);
	}
	else
	{
		MOG_Prompt::ClearMode(MOG_PROMPT_MODE_TYPE::MOG_PROMPT_SILENT);
	}
}

void MOG_Bridge::EnableConsoleMode(bool enable)
{
	if (enable)
	{
		MOG_Prompt::SetMode(MOG_PROMPT_MODE_TYPE::MOG_PROMPT_CONSOLE);
	}
	else
	{
		MOG_Prompt::ClearMode(MOG_PROMPT_MODE_TYPE::MOG_PROMPT_CONSOLE);
	}
}

void MOG_Bridge::EnableLogFileMode(bool enable)
{
	if (enable)
	{
		MOG_Prompt::SetMode(MOG_PROMPT_MODE_TYPE::MOG_PROMPT_FILE);
	}
	else
	{
		MOG_Prompt::ClearMode(MOG_PROMPT_MODE_TYPE::MOG_PROMPT_FILE);
	}
}

bool MOG_Bridge::IsSilentMode()
{
	return MOG_Prompt::IsMode(MOG_PROMPT_MODE_TYPE::MOG_PROMPT_SILENT);
}

bool MOG_Bridge::IsConsoleMode()
{
	return MOG_Prompt::IsMode(MOG_PROMPT_MODE_TYPE::MOG_PROMPT_CONSOLE);
}

bool MOG_Bridge::IsLogFileMode()
{
	return MOG_Prompt::IsMode(MOG_PROMPT_MODE_TYPE::MOG_PROMPT_FILE);
}

void MOG_Bridge::SetLogFileName(const char* filename)
{
	MOG_Report::SetLogFileName(new String(filename));
}

void MOG_Bridge::CopyManagedStringToNative(char* pDst, String* pSrc, size_t length)
{
	if (pDst && pSrc)
	{
		char* unmanagedSrc = (char*)Marshal::StringToHGlobalAnsi(pSrc).ToPointer();

		strncpy(pDst, unmanagedSrc, length );

		Marshal::FreeHGlobal(IntPtr(unmanagedSrc));
	}
}

char* MOG_Bridge::AllocNativeString(String* pSrc)
{
	char* pNativeString = NULL;

	if (pSrc)
	{
		pNativeString = (char*)Marshal::StringToHGlobalAnsi(pSrc).ToPointer();
	}

	return pNativeString;
}

void MOG_Bridge::FreeNativeString(char* pNativeString)
{
	if (pNativeString)
	{
		Marshal::FreeHGlobal(IntPtr(pNativeString));
	}
}


//
//
//
// Public method to determine whether an asset (filename) passed in can be locked.
bool MOG_Bridge::ResolveAssetNameForSyncTargetFile(IN const char* pFileName, OUT char* pAssetName, LockInfo* info)
{
	try
	{
		// Make sure LockInfo is valid
		if(!info)
			info = new LockInfo;

		String* pAssetNameStr = new String( pFileName );
		String* resultStr = ResolveAssetNameForSyncTargetFile( pAssetNameStr, info );

		// Assign our result back into pAssetName
		CopyManagedStringToNative(pAssetName, resultStr, MAX_MOG_CharArrayLength);

		if(resultStr->Length > 0)
			return true;
		else
			return false;
	}
	catch( Exception* e )
	{
		NotifySystemError( strcat("ERROR: Unable to Resolve Asset Name :", pFileName) , e->ToString(), e->StackTrace );
		return false;
	}
}

//
//
//
// Private method to determine whether an asset (filename) passed in can be locked.
String* MOG_Bridge::ResolveAssetNameForSyncTargetFile(String* pFileName, LockInfo* info)
{
	// If we have no server...
	if (String::Compare(MOG_ControllerSystem::GetServerComputerIP(), S"", true) == 0)
	{
		// Set our lock status to LockInfo_MoreThanOneAssetAttached
		info->LockStatus = MOG_BridgeAPI::LockInfo::LockInfo_ServerNotPresent;
		return S"";
	}

	// Get our active platform?
	String* platformName = MOG_ControllerProject::GetPlatformName();

	// Ask the project to resolve what asset this file belongs to?
	ArrayList* foundAsset = MOG_ControllerProject::MapFilenameToAssetName(pFileName, platformName, MOG_ControllerProject::GetWorkspaceDirectory());
	// If we have more than one asset attached, indicate so in info->LockStatus
	if(foundAsset->Count > 1)
	{
		info->LockStatus = MOG_BridgeAPI::LockInfo::LockInfo_MoreThanOneAssociatedAssetFound;
	}
	// Else if we have no assetsAttached, pFileName is not valid
	else if(foundAsset->Count < 1)
	{
		return S"";
	}

	// Return the first item in the assetsAttached ArrayList*
	return __try_cast<MOG_Filename*>( foundAsset->get_Item(0) )->GetAssetFullName();
}

//
//
//
// Public method to obtain the synced file for the specified asset.
MOGBridgeHandle MOG_Bridge::AssetSyncFiles_OpenObject(IN const char* pAssetName)
{
	MOGBridgeHandle handle = 0;

	try
	{
		// Make sure we have a valid AssetName?
		if (pAssetName)
		{
			MOG_Filename* assetFilename = new MOG_Filename(pAssetName);
			// Check if this asset is missing a path?
			if (assetFilename->GetAssetOriginalPath()->Length == 0)
			{
				// Attempt to locate the best matching asset
				MOG_Filename* tempAssetFilename = MOG_ControllerInbox::LocateBestMatchingAsset(assetFilename);
				if (!tempAssetFilename)
				{
					// Request this from the MOG Repository
					tempAssetFilename = MOG_ControllerProject::GetAssetCurrentBlessedVersionPath(assetFilename);
				}
				assetFilename = tempAssetFilename;
			}

			// Make sure we obtained a valid assetFilename?
			if (assetFilename)
			{
				MOG_Bridge_AssetSyncFiles* syncFileList = new MOG_Bridge_AssetSyncFiles(assetFilename, MOG_ControllerProject::GetPlatformName());
				MOG_Bridge_HandleTracker::Register(syncFileList);
				handle = syncFileList->GetHandle();
			}
		}
	}
	catch( Exception* e )
	{
		NotifySystemError( strcat("ERROR: Unable to obtain sync files for pAssetName :", pAssetName) , e->ToString(), e->StackTrace );
	}

	return handle;
}

bool MOG_Bridge::AssetSyncFiles_CloseObject(IN MOGBridgeHandle handle)
{
	MOG_Bridge_HandleTracker::Unregister(handle);
	return true;
}

int MOG_Bridge::AssetSyncFiles_GetFileCount(IN MOGBridgeHandle handle)
{
	int count = 0;

	// Make sure we have a valid object?
	if (handle)
	{
		// Make sure we have a valid handle the right type of object?
		MOG_Bridge_AssetSyncFiles* object = dynamic_cast<MOG_Bridge_AssetSyncFiles*>(MOG_Bridge_HandleTracker::GetObject(handle));
		if (object)
		{
			count = object->GetSyncFileList()->Count;
		}
	}

	return count;
}

bool MOG_Bridge::AssetSyncFiles_GetFilename(IN MOGBridgeHandle handle, IN int index, OUT char* pFilename)
{
	// Make sure we have a valid object?
	if (handle)
	{
		// Make sure we have a valid handle the right type of object?
		MOG_Bridge_AssetSyncFiles* object = dynamic_cast<MOG_Bridge_AssetSyncFiles*>(MOG_Bridge_HandleTracker::GetObject(handle));
		if (object)
		{
			ArrayList* syncFileList = object->GetSyncFileList();

			// Make sure the specified index is within our bounds?
			if (syncFileList->Count > index)
			{
				// Assign our result back into pFileName
				CopyManagedStringToNative(pFilename, dynamic_cast<String*>( syncFileList->get_Item(index)), MAX_MOG_CharArrayLength);

				return true;
			}
		}
	}

	return false;
}

MOGBridgeHandle MOG_Bridge::AssetProperties_OpenObject(IN const char* pAssetName)
{
	MOGBridgeHandle handle = 0;

	try
	{
		// Make sure we have a valid AssetName?
		if (pAssetName)
		{
			MOG_Filename* assetFilename = new MOG_Filename(pAssetName);
			// Check if this asset is missing a path?
			if (assetFilename->GetAssetOriginalPath()->Length == 0)
			{
				// Attempt to locate the best matching asset
				MOG_Filename* tempAssetFilename = MOG_ControllerInbox::LocateBestMatchingAsset(assetFilename);
				if (!tempAssetFilename)
				{
					// Request this from the MOG Repository
					tempAssetFilename = MOG_ControllerProject::GetAssetCurrentBlessedVersionPath(assetFilename);
				}
				assetFilename = tempAssetFilename;
			}

			// Make sure we obtained a valid assetFilename?
			if (assetFilename)
			{
				MOG_Bridge_AssetProperties* properties = new MOG_Bridge_AssetProperties(assetFilename, MOG_ControllerProject::GetPlatformName());
				MOG_Bridge_HandleTracker::Register(properties);
				handle = properties->GetHandle();
			}
		}
	}
	catch( Exception* e )
	{
		NotifySystemError( strcat("ERROR: Unable to obtain properties for pAssetName :", pAssetName) , e->ToString(), e->StackTrace );
	}

	return handle;
}

bool MOG_Bridge::AssetProperties_CloseObject(IN MOGBridgeHandle handle)
{
	MOG_Bridge_HandleTracker::Unregister(handle);
	return true;
}

bool MOG_Bridge::AssetProperties_GetProperty(IN MOGBridgeHandle handle, IN MOGAPI_Property* prop)
{
	return AssetProperties_GetProperty(handle, prop->mSection, prop->mPropertySection, prop->mPropertyKey, prop->mPropertyValue);
}

bool MOG_Bridge::AssetProperties_GetProperty(IN MOGBridgeHandle handle, IN const char* section, IN const char* propertySection, IN const char* propertyKey, OUT char* propertyValue)
{
	// Make sure we have a valid object?
	if (handle)
	{
		// Make sure we have a valid handle the right type of object?
		MOG_Bridge_AssetProperties* object = dynamic_cast<MOG_Bridge_AssetProperties*>(MOG_Bridge_HandleTracker::GetObject(handle));
		if (object)
		{
			// Get the property value
			MOG_Property* prop = object->GetProperties()->GetProperty(section, propertySection, propertyKey);
			if (prop)
			{
				// Format the property's return value
				String* value = MOG_Tokens::GetFormattedString(prop->mPropertyValue, object->GetProperties()->GetAssetFilename(), object->GetProperties()->GetPropertyList());

				// Assign our result back into propertyValue
				CopyManagedStringToNative(propertyValue, value, MAX_MOG_CharArrayLength);

				return true;
			}
		}
	}

	return false;
}

bool MOG_Bridge::AssetProperties_GetProperty(IN MOGBridgeHandle handle, IN const char* section, IN int keyIndex, OUT char* key, OUT char* value)
{
	// Make sure we have a valid object?
	if (handle)
	{
		// Make sure we have a valid handle the right type of object?
		MOG_Bridge_AssetProperties* object = dynamic_cast<MOG_Bridge_AssetProperties*>(MOG_Bridge_HandleTracker::GetObject(handle));
		if (object)
		{
			// Get the properties in this section
			ArrayList* properties = object->GetProperties()->GetPropertyList(section);
			// Make sure this keyIndex is valid?
			if (keyIndex < properties->Count)
			{
				// Get the property for the given keyIndex
				MOG_Property* prop = dynamic_cast<MOG_Property*>(properties->Item[keyIndex]);

				// Format the property's return value
				String* temp = MOG_Tokens::GetFormattedString(prop->mKey, object->GetProperties()->GetAssetFilename(), object->GetProperties()->GetPropertyList());
				// Assign our result back into key
				CopyManagedStringToNative(key, temp, MAX_MOG_CharArrayLength);

				// Format the property's return value
				temp = MOG_Tokens::GetFormattedString(prop->mPropertyValue, object->GetProperties()->GetAssetFilename(), object->GetProperties()->GetPropertyList());
				// Assign our result back into value
				CopyManagedStringToNative(value, temp, MAX_MOG_CharArrayLength);

				return true;
			}
		}
	}

	return false;
}

bool MOG_Bridge::AssetProperties_GetPackageCommand_Add(IN MOGBridgeHandle handle, IN int fileIndex, IN int packageAssignmentIndex, OUT char* value)
{
	// Make sure we have a valid object?
	if (handle)
	{
		// Make sure we have a valid handle the right type of object?
		MOG_Bridge_AssetProperties* object = dynamic_cast<MOG_Bridge_AssetProperties*>(MOG_Bridge_HandleTracker::GetObject(handle));
		if (object)
		{
			MOG_Properties* properties = object->GetProperties();
			String* commandValue = properties->PackageCommand_Add;
			String* fileValue = S"";
			String* packageAssignmentValue = S"";

			// Get the fileValue
			String* filesSection = MOG_ControllerAsset::GetAssetPlatformFilesSection(properties, MOG_ControllerProject::GetPlatformName(), true);
			ArrayList* filesProperties = properties->GetPropertyList(filesSection);
			// Make sure this keyIndex is valid?
			if (fileIndex < filesProperties->Count)
			{
				// Get the property for the given keyIndex
				MOG_Property* prop = dynamic_cast<MOG_Property*>(filesProperties->Item[fileIndex]);
				fileValue = prop->mKey;
			}

			// Get the packageAssignmentValue
			ArrayList* packageAssignmentProperties = properties->GetPropertyList(S"Relationships", S"Packages");
			// Make sure this keyIndex is valid?
			if (packageAssignmentIndex < packageAssignmentProperties->Count)
			{
				// Get the property for the given keyIndex
				MOG_Property* prop = dynamic_cast<MOG_Property*>(packageAssignmentProperties->Item[packageAssignmentIndex]);
				packageAssignmentValue = prop->mPropertyKey;
			}

			// Format the property's return value
			String* workspaceDirectory = MOG_ControllerProject::GetWorkspaceDirectory();
			String* platformName = MOG_ControllerProject::GetPlatformName();
			String* label = S"";
			MOG_PackageMerge_PackageFileInfo* packageFileInfo = new MOG_PackageMerge_PackageFileInfo(MOG_ControllerPackage::GetPackageName(packageAssignmentValue), workspaceDirectory, platformName, label);
			MOG_Filename* filename = new MOG_Filename(String::Concat(properties->GetAssetFilename()->GetEncodedFilename(), S"\\", filesSection, S"\\", fileValue));
			String* packageCommand = MOG_Tokens::GetFormattedString(commandValue, filename, packageFileInfo, workspaceDirectory, platformName, properties, packageAssignmentValue);
			// Assign our result back into value
			CopyManagedStringToNative(value, packageCommand, MAX_MOG_CharArrayLength);
			return true;
		}
	}

	return false;
}

bool MOG_Bridge::AssetProperties_GetPackageCommand_Remove(IN MOGBridgeHandle handle, IN int fileIndex, IN int packageAssignmentIndex, OUT char* value)
{
	// Make sure we have a valid object?
	if (handle)
	{
		// Make sure we have a valid handle the right type of object?
		MOG_Bridge_AssetProperties* object = dynamic_cast<MOG_Bridge_AssetProperties*>(MOG_Bridge_HandleTracker::GetObject(handle));
		if (object)
		{
			MOG_Properties* properties = object->GetProperties();
			String* commandValue = properties->PackageCommand_Add;
			String* fileValue = S"";
			String* packageAssignmentValue = S"";

			// Get the fileValue
			String* filesSection = MOG_ControllerAsset::GetAssetPlatformFilesSection(properties, MOG_ControllerProject::GetPlatformName(), true);
			ArrayList* filesProperties = properties->GetPropertyList(filesSection);
			// Make sure this keyIndex is valid?
			if (fileIndex < filesProperties->Count)
			{
				// Get the property for the given keyIndex
				MOG_Property* prop = dynamic_cast<MOG_Property*>(filesProperties->Item[fileIndex]);
				fileValue = prop->mKey;
			}

			// Get the packageAssignmentValue
			ArrayList* packageAssignmentProperties = properties->GetPropertyList(S"Relationships", S"Packages");
			// Make sure this keyIndex is valid?
			if (packageAssignmentIndex < packageAssignmentProperties->Count)
			{
				// Get the property for the given keyIndex
				MOG_Property* prop = dynamic_cast<MOG_Property*>(packageAssignmentProperties->Item[packageAssignmentIndex]);
				packageAssignmentValue = prop->mPropertyKey;
			}

			// Format the property's return value
			String* workspaceDirectory = MOG_ControllerProject::GetWorkspaceDirectory();
			String* platformName = MOG_ControllerProject::GetPlatformName();
			String* label = S"";
			MOG_PackageMerge_PackageFileInfo* packageFileInfo = new MOG_PackageMerge_PackageFileInfo(MOG_ControllerPackage::GetPackageName(packageAssignmentValue), workspaceDirectory, platformName, label);
			MOG_Filename* filename = new MOG_Filename(String::Concat(properties->GetAssetFilename()->GetEncodedFilename(), S"\\", filesSection, S"\\", fileValue));
			String* packageCommand = MOG_Tokens::GetFormattedString(commandValue, filename, packageFileInfo, workspaceDirectory, platformName, properties, packageAssignmentValue);
			// Assign our result back into value
			CopyManagedStringToNative(value, packageCommand, MAX_MOG_CharArrayLength);
			return true;
		}
	}

	return false;
}


bool MOG_Bridge::IsLocalFileOutofdate(IN const char* pFilename)
{
	if(MOG_ControllerProject::IsProject())
	{
		// Check if this is not already an asset name?
		String* myFilename = new String(pFilename);
		MOG_Filename* assetFilename = new MOG_Filename(myFilename);
		if (assetFilename->GetFilenameType() != MOG_FILENAME_TYPE::MOG_FILENAME_Asset)
		{
			// Check if this is an absolute path?
			String* relativeFilename = myFilename;
			String* localWorkspaceDirectory = MOG_ControllerProject::GetWorkspaceDirectory();
			if (localWorkspaceDirectory->StartsWith(relativeFilename))
			{
				// Convert this into a relative path
				relativeFilename->Substring(localWorkspaceDirectory->Length + 1);
			}

			// Map this relative filename to its asset name
			ArrayList* assets = MOG_ControllerProject::MapFilenameToAssetName(relativeFilename, MOG_ControllerProject::GetPlatformName(), MOG_ControllerProject::GetWorkspaceDirectory());
			if (assets->Count > 0)
			{
				assetFilename = dynamic_cast<MOG_Filename*>(assets->Item[0]);
			}
		}

		// Locate the best matching version of this asset just in case the user has it sitting in their mailbox
		MOG_Filename* tempAssetFilename = MOG_ControllerInbox::LocateBestMatchingAsset(assetFilename);
		if (!tempAssetFilename)
		{
			// Request this from the MOG Repository
			tempAssetFilename = MOG_ControllerProject::GetAssetCurrentBlessedVersionPath(assetFilename);
		}
		assetFilename = tempAssetFilename;

		// Check if asset is out-of-date?
		return MOG_ControllerAsset::ValidateAsset_IsAssetOutofdate(assetFilename);
	}

	return false;
}


//
//
//
// Public method to remove a package assignment for an asset.
bool MOG_Bridge::RemovePackageAssignment(IN const char* pAssetLabel, 
										 IN const char* pPackageFile, 
										 IN const char* pPackageGroupName, 
										 IN const char* pPackageObjectName,
										 IN const char* LOD)
{
	bool bRemoved = false;

	// Make sure we transfer the data over to our memory
	String* myAssetLabel = new String(pAssetLabel);
	String* myPackageFile = new String(pPackageFile);
	String* myPackageGroupName = new String(pPackageGroupName);
	String* myPackageObjectName = new String(pPackageObjectName);

	// Setup our package assignment
	MOG_Property* relationshipProperty = MOG_PropertyFactory::MOG_Relationships::New_PackageAssignment(myPackageFile, myPackageGroupName, myPackageObjectName);

	try
	{
		// Make sure we are logged into a project
		if (MOG_ControllerProject::IsProject())
		{
			bool bPerformDeletePrompt = true;
			bool bDelete = false;
			bool bAssetMovedToInbox = false;

			// Fix up the specified properties with syntactically correct values
			MOG_Property* repairedProperty = MOG_ControlsLibrary::Utils::PropertyHelper::RepairProperty(relationshipProperty);

			// Extract the elements of the repaired relationship
			String* packageName = MOG_ControllerPackage::GetPackageName(repairedProperty->mPropertyKey);
			String* packageGroupName = MOG_ControllerPackage::GetPackageGroups(repairedProperty->mPropertyKey);
			MOG_Filename* packageFilename = new MOG_Filename(packageName);

			ArrayList* foundAssets = new ArrayList();
			ArrayList* containedAssets = new ArrayList();

			// Check if the specific myAssetLabel is already a full asset name?
			MOG_Filename* myAssetLabelFilename = new MOG_Filename(myAssetLabel);
			if (myAssetLabelFilename->GetFilenameType() == MOG_FILENAME_TYPE::MOG_FILENAME_Asset)
			{
				// Attempt to locate this asset in the user's inbox
				MOG_Filename* tempAssetFilename = MOG_ControllerInbox::LocateBestMatchingAsset(myAssetLabelFilename);
				if (tempAssetFilename)
				{
					// Use the inbox asset
					myAssetLabelFilename = tempAssetFilename;
				}

				// Check if this asset is located within the inbox already?
				foundAssets->Add(myAssetLabelFilename);
			}
			else
			{
				// Map this package object to its asset names
				foundAssets = MOG_ControllerProject::MapPackageObjectToAssetNames(myAssetLabel, repairedProperty);
				foundAssets->AddRange(MOG_ControllerProject::MapPackageObjectToAssetNamesInUserInbox(myAssetLabel, repairedProperty));

				// Check for the absence of a package object being specified in the package assignment?
				containedAssets = new ArrayList();
				if (MOG_ControllerPackage::GetPackageObjects(relationshipProperty->mPropertyKey)->Length == 0)
				{
					// Check for contained assets when no packageGroup is specified
					containedAssets = MOG_ControllerProject::MapPackageObjectToContainedAssetNames(myAssetLabel, repairedProperty, LOD);
					containedAssets->AddRange(MOG_ControllerProject::MapPackageObjectToContainedAssetNamesInUserInbox(myAssetLabel, repairedProperty, LOD));
					// Combine the two lists for the loop below
					foundAssets->AddRange(containedAssets);
				}
			}

			// Check if we found a matching asset?
			if (foundAssets->Count)
			{
				// Enumerate through the post commands
				for(int i = 0; i < foundAssets->Count; i++)
				{
					MOG_Filename* assetFilename = __try_cast<MOG_Filename*>(foundAssets->Item[i]);

					// Check if this is a blessed asset?
					if (MOG_ControllerProject::DoesAssetExists(assetFilename))
					{
						// Check how many package assignments this asset has?
						MOG_Properties* properties = new MOG_Properties(assetFilename);

						// How many package assignments does this asset have?
						int currentPackageAssignments = properties->GetPackages()->Count;
						if (currentPackageAssignments > 1)
						{
							// Check if this asset is not in the inbox?
							if (!assetFilename->IsWithinInboxes())
							{
								// Obtain the current revision of this blessed asset in the MOG Repository
								MOG_Filename* blessedAssetFilename = MOG_ControllerProject::GetAssetCurrentBlessedVersionPath(assetFilename);
								if (blessedAssetFilename)
								{
									// Copy this blessed asset to the user's drafts
									if (MOG_ControllerInbox::CopyToBox(blessedAssetFilename, S"Drafts", MOG_AssetStatusType::Modifying, NULL))
									{
										assetFilename = new MOG_Filename(assetFilename->GetAssetEncodedInboxPath(MOG_ControllerProject::GetUserName_DefaultAdmin(), "Drafts"));
										bAssetMovedToInbox = true;
									}
								}
							}
						}
						else if (currentPackageAssignments == 1)
						{
							// Check if we should prompt the user about this delete
							if (bPerformDeletePrompt)
							{
								// Prompt the user about removing this asset from the project?
								// Check if this asset contained any other assets as their packageObject?
								String *message = "";
								if (containedAssets->Count)
								{
									message = String::Concat(message, S"This asset contains other assets that may also be removed.\n");
								}
								message = String::Concat(S"Do you want to remove this asset from the MOG Project?\n",
														 S"OBJECT: ", myAssetLabel, S"\n",
														 S"CLASSIFICATION: ", assetFilename->GetAssetClassification(), S"\n",
														 S"ASSET NAME: ", assetFilename->GetAssetName(), S"\n",
														 S"\n",
														 S"Note: Removed assets can be recovered in the project's archive tree.");
								switch (MOG_Prompt::PromptResponse(S"Remove From Project", message, MOGPromptButtons::YesNoCancel))
								{
									case MOGPromptResult::Yes:
										// Delete all of the assets and don't prompt again
										bDelete = true;
										bPerformDeletePrompt = false;
										break;
									case MOGPromptResult::Cancel:
										// Simply return false informing the action was not completed
										return false;
										break;
									case MOGPromptResult::No:
										// Continue w/o removing the asset from the project
										bPerformDeletePrompt = false;
										break;
								}
							}

							// Check if we decided to delete the asset?
							if (bDelete)
							{
								String* comment = S"Removed from project when it was removed from the package.";
								MOG_ControllerProject::RemoveAssetFromProject(assetFilename, comment, false);

								// Check if this asset exists in the user's local updated tray?
								MOG_Filename *updatedAssetFilename = MOG_Filename::GetLocalUpdatedTrayFilename(MOG_ControllerProject::GetWorkspaceDirectory(), assetFilename);
								if (DosUtils::DirectoryExistFast(updatedAssetFilename->GetEncodedFilename()))
								{
									// Proceed to remove this asset from the user's local updated tray
									MOG_ControllerInbox::Delete(updatedAssetFilename);
								}
							}

							// Indicate the package assignment was removed from the asset
							bRemoved = true;
						}
					}

					// Check if this is an inbox asset?
					if (assetFilename->IsWithinInboxes())
					{
						// Check if this asset was part of the containedAssets by comparing our index against the combined list
						if (i >= foundAssets->Count - containedAssets->Count)
						{
							// Proceed to remove the the relationship property from this inbox asset using the repairedPropertyWithObjectName
							MOG_Property* repairedPropertyWithObjectName = MOG_PropertyFactory::MOG_Relationships::New_RelationshipAssignment(repairedProperty->mPropertySection, packageName, packageGroupName, myAssetLabel);
							if (MOG_ControllerInbox::RemoveRelationshipProperty(assetFilename, repairedPropertyWithObjectName))
							{
								bRemoved = true;
							}
						}
						else
						{
							// Proceed to remove the the relationship property from this inbox asset using the original repairedProperty
							if (MOG_ControllerInbox::RemoveRelationshipProperty(assetFilename, repairedProperty))
							{
								bRemoved = true;
							}
						}
					}
				}

				// Check if we moved an asset to the user's inbox?
				if (bAssetMovedToInbox)
				{
					// Inform the user that the package assignment has been removed and the updated asset is sitting in their inbox needing to be blessed.
					MOG_Filename* assetFilename = new MOG_Filename(packageName);
					String* message = String::Concat(S"This package assignment has been removed from the asset(s).\n",
													 S"The modified asset(s) are in your drafts and waiting to be blessed.\n",
													 S"OBJECT: ", myAssetLabel, S"\n",
													 S"CLASSIFICATION: ", assetFilename->GetAssetClassification(), S"\n",
													 S"ASSET NAME: ", assetFilename->GetAssetName(), S"\n");
					MOG_Prompt::PromptMessage(S"Remove Package Assignment", message);
				}
			}
			else
			{
				// No matching assets were found so indicate it has been removed
				bRemoved = true;
			}
		}
		else
		{
			String* message = String::Concat(	S"MOG can only remove package assignments when the application is logged into a project.\n");
			MOG_Prompt::PromptMessage("MOG Import Failed", message);
		}
	}
	catch( Exception* e )
	{
		NotifySystemError( "ERROR: Application threw while trying to remove the package assignment:", e->ToString(), e->StackTrace );
	}

	return bRemoved;
}

//
//
//
// Public method to remove a relationship for an asset.
bool MOG_Bridge::RemoveRelationshipAssignment(IN const char* pAssetLabel, 
											  IN const char* pRelationship, 
											  IN const char* pRelatedAssetName, 
											  IN const char* pRelatedGroupName, 
											  IN const char* pRelatedObjectName)
{
	// Make sure we transfer the data over to our memory
	String* myAssetLabel = new String(pAssetLabel);
	String* myRelationship = new String(pRelationship);
	String* myRelatedAssetName = new String(pRelatedAssetName);
	String* myRelatedGroupName = new String(pRelatedGroupName);
	String* myRelatedObjectName = new String(pRelatedObjectName);

	// Remove the specified relationship from the asset
	return false;
}

bool MOG_Bridge::GenerateLODName(IN const char *pAssetLabel, IN const char *pLOD, OUT char *pName)
{
	String* myAssetLabel = new String(pAssetLabel);
	String* myLOD = new String(pLOD);

	String* result = MOG_ControllerProject::GenerateLODName(myAssetLabel, myLOD);

	// Assign our result back into pAssetName
	CopyManagedStringToNative(pName, result, MAX_MOG_CharArrayLength);
	return true;
}

bool MOG_Bridge::DetectLODTagFromAssetLabel(IN const char *pAssetLabel, OUT char *pLODTag)
{
	String* myAssetLabel = new String(pAssetLabel);

	String* result = MOG_ControllerProject::DetectLODTagFromAssetLabel(myAssetLabel);

	// Assign our result back into pAssetName
	CopyManagedStringToNative(pLODTag, result, MAX_MOG_CharArrayLength);
	return true;
}

bool MOG_Bridge::StripLODTagFromAssetLabel(IN const char *pAssetLabel, OUT char *pStrippedAssetLabel)
{
	String* myAssetLabel = new String(pAssetLabel);

	String* result = MOG_ControllerProject::StripLODTagFromAssetLabel(myAssetLabel);

	// Assign our result back into pAssetName
	CopyManagedStringToNative(pStrippedAssetLabel, result, MAX_MOG_CharArrayLength);
	return true;
}

bool MOG_Bridge::DetectLODFromAssetLabel(IN const char *pAssetLabel, OUT char *pLOD)
{
	String* myAssetLabel = new String(pAssetLabel);

	String* result = MOG_ControllerProject::DetectLODFromAssetLabel(myAssetLabel);

	// Assign our result back into pAssetName
	CopyManagedStringToNative(pLOD, result, MAX_MOG_CharArrayLength);
	return true;
}

bool MOG_Bridge::DetectLODFromLODTag(IN const char *pLODTag, OUT char *pLOD)
{
	String* myLODTag = new String(pLODTag);

	String* result = MOG_ControllerProject::DetectLODFromLODTag(myLODTag);

	// Assign our result back into pAssetName
	CopyManagedStringToNative(pLOD, result, MAX_MOG_CharArrayLength);
	return true;
}

bool MOG_Bridge::InboxFetchFromRepository(IN const char *pBox, IN const char *pAssetName)
{
	// Get the specified box defaulting to 'Drafts'
	String* myBox = S"Drafts";
	if (pBox)
	{
		myBox = new String(pBox);
	}

	// Make sure we have a specified asset?
	if (pAssetName)
	{
		String* myAssetName = new String(pAssetName);

		MOG_Filename *assetFilename = new MOG_Filename(myAssetName);
		MOG_Filename *blessedAssetFilename = MOG_ControllerProject::GetAssetCurrentBlessedVersionPath(assetFilename);
		return MOG_ControllerInbox::CopyToBox(blessedAssetFilename, myBox, MOG_AssetStatusType::Sent, NULL);
	}

	return false;
}

bool MOG_Bridge::InboxDelete(IN const char *pBox, IN const char *pAssetName)
{
	// Get the specified box defaulting to 'Drafts'
	String* myBox = S"Drafts";
	if (pBox)
	{
		myBox = new String(pBox);
	}

	// Make sure we have a specified asset?
	if (pAssetName)
	{
		String* myAssetName = new String(pAssetName);

		MOG_Filename *inboxAssetFilename = new MOG_Filename(MOG_ControllerProject::GetUserName_DefaultAdmin(), myBox, "", myAssetName);
		return MOG_ControllerInbox::Delete(inboxAssetFilename);
	}

	return false;
}

bool MOG_Bridge::InboxRip(IN const char *pBox, IN const char *pAssetName)
{
	// Get the specified box defaulting to 'Drafts'
	String* myBox = S"Drafts";
	if (pBox)
	{
		myBox = new String(pBox);
	}

	// Make sure we have a specified asset?
	if (pAssetName)
	{
		String* myAssetName = new String(pAssetName);

		MOG_Filename *inboxAssetFilename = new MOG_Filename(MOG_ControllerProject::GetUserName_DefaultAdmin(), myBox, "", myAssetName);
		return MOG_ControllerInbox::RipAsset(inboxAssetFilename);
	}

	return false;
}

bool MOG_Bridge::InboxReject(IN const char *pBox, IN const char *pAssetName, IN const char *pComment)
{
	// Get the specified box defaulting to 'Drafts'
	String* myBox = S"Drafts";
	if (pBox)
	{
		myBox = new String(pBox);
	}

	// Get the specified comment
	String* myComment = S"No comment specified.";
	if (pComment)
	{
		myComment = new String(pComment);
	}

	// Make sure we have a specified asset?
	if (pAssetName)
	{
		String* myAssetName = new String(pAssetName);

		MOG_Filename *inboxAssetFilename = new MOG_Filename(MOG_ControllerProject::GetUserName_DefaultAdmin(), myBox, "", myAssetName);
		return MOG_ControllerInbox::Reject(inboxAssetFilename, myComment, NULL);
	}

	return false;
}

bool MOG_Bridge::InboxBless(IN const char *pBox, IN const char *pAssetName, IN const char *pComment)
{
	// Get the specified box defaulting to 'Drafts'
	String* myBox = S"Drafts";
	if (pBox)
	{
		myBox = new String(pBox);
	}

	// Get the specified comment
	String* myComment = S"No comment specified.";
	if (pComment)
	{
		myComment = new String(pComment);
	}

	// Make sure we have a specified asset?
	if (pAssetName)
	{
		String* myAssetName = new String(pAssetName);

		MOG_Filename *inboxAssetFilename = new MOG_Filename(MOG_ControllerProject::GetUserName_DefaultAdmin(), myBox, "", myAssetName);
		return MOG_ControllerInbox::BlessAsset(inboxAssetFilename, myComment, false, NULL);
	}

	return false;
}

bool MOG_Bridge::InboxMoveTo(IN const char *pBox, IN const char *pAssetName, IN const char *pComment, IN const char *pTargetUser)
{
	// Get the specified box defaulting to 'Drafts'
	String* myBox = S"Drafts";
	if (pBox)
	{
		myBox = new String(pBox);
	}

	// Get the specified comment
	String* myComment = S"No comment specified.";
	if (pComment)
	{
		myComment = new String(pComment);
	}

	// Make sure we have a specified asset?
	if (pAssetName)
	{
		String* myAssetName = new String(pAssetName);

		// Make sure we have a valid target user
		if (pTargetUser)
		{
			String* myTargetUser = new String(pTargetUser);

			MOG_Filename *inboxAssetFilename = new MOG_Filename(MOG_ControllerProject::GetUserName_DefaultAdmin(), myBox, "", myAssetName);
			return MOG_ControllerInbox::MoveAssetToUserInbox(inboxAssetFilename, myTargetUser, myComment, MOG_AssetStatusType::Sent, NULL);
		}
	}

	return false;
}

bool MOG_Bridge::InboxCopyTo(IN const char *pBox, IN const char *pAssetName, IN const char *pComment, IN const char *pTargetUser)
{
	// Get the specified box defaulting to 'Drafts'
	String* myBox = S"Drafts";
	if (pBox)
	{
		myBox = new String(pBox);
	}

	// Get the specified comment
	String* myComment = S"No comment specified.";
	if (pComment)
	{
		myComment = new String(pComment);
	}

	// Make sure we have a specified asset?
	if (pAssetName)
	{
		String* myAssetName = new String(pAssetName);

		// Make sure we have a valid target user
		if (pTargetUser)
		{
			String* myTargetUser = new String(pTargetUser);

			MOG_Filename *inboxAssetFilename = new MOG_Filename(MOG_ControllerProject::GetUserName_DefaultAdmin(), myBox, "", myAssetName);
			return MOG_ControllerInbox::CopyAssetToUserInbox(inboxAssetFilename, myTargetUser, myComment, MOG_AssetStatusType::Sent, NULL);
		}
	}

	return false;
}

bool MOG_Bridge::IsConnected( void )
{
	return MOG_BridgeContainer::GetContainer( mBaseHandle )->IsConnected();
}

int MOG_Bridge::GetHandle( void )
{
	return mBaseHandle;
}

bool MOG_Bridge::CheckAPIVersion( int version )
{
	return version == MOG_API_VERSION;
}

bool MOG_Bridge::CheckMajorVersion()
{
	//Check the system's server information to make sure it matches the hard-coded define(us)
	if (MOG_ControllerSystem::GetSystem()->ServerMajorVersion == MOG_ControllerSystem::MogMajorVersion)
	{
		return true;
	}
	else
	{
		return false;
	}
}

bool MOG_Bridge::CheckMinorVersion()
{
	//Check the system's server information to make sure it matches the hard-coded define(us)
	if (MOG_ControllerSystem::GetSystem()->ServerMinorVersion == MOG_ControllerSystem::MogMinorVersion)
	{
		return true;
	}
	else
	{
		return false;
	}
}

MOG_PendingImport::MOG_PendingImport()
{
	mFilename = S"";
	mAssetClassification = S"";
	mAssetLabel = S"";
	mAssetPlatform = S"";
	mProperties = new ArrayList();
}

