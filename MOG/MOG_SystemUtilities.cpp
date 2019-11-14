//--------------------------------------------------------------------------------
//	MOG_SystemUtilities.cpp
//	
//	
//--------------------------------------------------------------------------------

#include "StdAfx.h"

#include "MOG_Define.h"
#include "MOG_Main.h"
#include "MOG_Project.h"
#include "MOG_Ini.h"
#include "MOG_DosUtils.h"
#include "MOG_ControllerAsset.h"
#include "MOG_ControllerRepository.h"
#include "MOG_ControllerInbox.h"
#include "MOG_ControllerLibrary.h"
#include "MOG_ControllerProject.h"
#include "MOG_ControllerSystem.h"
#include "MOG_ControllerPackage.h"
#include "MOG_Tokens.h"
#include "MOG_DBAssetAPI.h"
#include "MOG_DBInboxAPI.h"
#include "MOG_DBProjectAPI.h"
#include "MOG_DBSyncedDataAPI.h"
#include "MOG_Progress.h"
#include "MOG_CommandFactory.h"

#include "MOG_SystemUtilities.h"



#using <mscorlib.dll>
using namespace System;
using namespace System::IO;
using namespace System::Threading;
using namespace System::Collections::Generic;
using namespace MOG_CoreControls;


ArrayList *MOG_SystemUtilities::GetAutomatedTestNames(void)
{
	ArrayList *testNames = new ArrayList();

	testNames->Add(S"ImportProcessBless");
	testNames->Add(S"CopyNetworkFiles");
	testNames->Add(S"LibraryCheckOutIn");

	return testNames;
}


bool MOG_SystemUtilities::AutomatedTesting_Drone(String *testName, int testDuration_Seconds, String *projectName, String *testImportFile, bool bCopyFileLocal, int startingExtensionIndex)
{
	bool bThreadAlreadyRunning = false;

	// Check if there is already a thread running?
	if (mAutomatedTesting_Duration)
	{
		bThreadAlreadyRunning = true;
	}

	// Update the our variables to reflect the specified values
	mAutomatedTesting_Duration = testDuration_Seconds;

	// Attempt to copy the specified file to our local hard drive
	String *importFilename = DosUtils::PathGetFileName(testImportFile);
	String *localImportFile = String::Concat(S"C:\\TestAssets\\", importFilename);
	if (bCopyFileLocal && 
		DosUtils::FileCopyFast(testImportFile, localImportFile, true))
	{
		// Use the new local copy of this file for our testing
		mAutomatedTesting_TestImportFile = localImportFile;
	}
	else
	{
		// Since we failed to copy the file let's simply use the specified file
		mAutomatedTesting_TestImportFile = testImportFile;
	}

	// Check if we are counting the assets?
	mAutomatedTesting_AssetIndexNumber = startingExtensionIndex;

	// Make sure there isn't already a thread running?
	// Make sure we actually have a valid duration before bothering to start a new thread
	if (!bThreadAlreadyRunning &&
		mAutomatedTesting_Duration)
	{
		// Log us into the specified project
		MOG_Project *pProject = MOG_ControllerProject::LoginProject(projectName, S"");
		if (pProject)
		{
			String *userName = String::Concat(S"Drone.", MOG_ControllerSystem::GetComputerName());

			// Always attempt to create our Drone User
			MOG_User *pUser = new MOG_User();
			pUser->SetUserName(userName);
			pUser->SetUserDepartment(S"Drone");
			pUser->SetBlessTarget(S"MasterData");
			pProject->UserAdd(pUser);

			// Log in as our Drone User
			pUser = MOG_ControllerProject::LoginUser(userName);
			if (pUser)
			{
				// Make sure we have a duration to even bother starting a thread?
				if (testDuration_Seconds)
				{
					ThreadStart *threadDelegate = NULL;

					// Start the new thread
					if (testName->ToLower()->IndexOf(S"ImportProcessBless"->ToLower()) != -1)
					{
						threadDelegate = new ThreadStart(0, &MOG_SystemUtilities::AutomatedTesting_Drone_ImportProcessBless_Thread);
					}
					else if (testName->ToLower()->IndexOf(S"CopyNetworkFiles"->ToLower()) != -1)
					{
						threadDelegate = new ThreadStart(0, &MOG_SystemUtilities::AutomatedTesting_Drone_CopyNetworkFiles_Thread);
					}
					else if (testName->ToLower()->IndexOf(S"LibraryCheckOutIn"->ToLower()) != -1)
					{
						threadDelegate = new ThreadStart(0, &MOG_SystemUtilities::AutomatedTesting_Drone_LibraryCheckOutIn_Thread);
					}
					else
					{
						String *message = S"The Drone failed to match the specified test name.";
						MOG_Report::ReportMessage(S"Automatted Testing", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
					}

					// Check if we were able to match the test name?
					if (threadDelegate != NULL)
					{
						Thread *newThread = new Thread(threadDelegate);
						newThread->Start();
						return true;
					}
				}
			}
			else
			{
				String *message = S"The Drone failed to log in as its Drone User.";
				MOG_Report::ReportMessage(S"Automatted Testing", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
			}
		}
		else
		{
			String *message = S"The Drone failed to log into the specified project.";
			MOG_Report::ReportMessage(S"Automatted Testing", message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
		}
	}

	return false;
}


void MOG_SystemUtilities::AutomatedTesting_Drone_ImportProcessBless_Thread()
{
	List<Object*>* args = new List<Object*>();
	args->Add(mAutomatedTesting_TestImportFile);
	args->Add(__box(mAutomatedTesting_Duration));

	// Initialize our progress dialog
	ProgressDialog* progress = new ProgressDialog("Automated Testing", "Please wait while the Client performs the test.", new DoWorkEventHandler(NULL, &AutomatedTesting_Drone_ImportProcessBless_Worker), args, false);
	progress->ShowDialog();
}

void MOG_SystemUtilities::AutomatedTesting_Drone_ImportProcessBless_Worker(Object* sender, DoWorkEventArgs* e)
{
	BackgroundWorker* worker = dynamic_cast<BackgroundWorker*>(sender);
	List<Object*>* args = dynamic_cast<List<Object*>*>(e->Argument);
	String* importPath = dynamic_cast<String*>(args->Item[0]);
	int duration = *dynamic_cast<__box int*>(args->Item[1]);	

	String *importFilename = DosUtils::PathGetFileName(importPath);

	DateTime testStartTime = DateTime::Now;
	DateTime testCompleteTime = DateTime::Now.AddSeconds(duration);
	TimeSpan testDuration = testCompleteTime - testStartTime;
	
	try
	{
		// Repeat loop for the specified iterations
		while (!worker->CancellationPending)
		{
			// Check if we have exceeded our specified time span?
			TimeSpan testTimeRemaining = testCompleteTime - DateTime::Now;
			int testPercent = 100 - Convert::ToInt32((testTimeRemaining.TotalSeconds * 100) / testDuration.TotalSeconds);
			if (testPercent > 100)
			{
				// Time to  end our automated testing loop
				break;
			}

			// Build our asset's name
			String *testAssetLabel = S"TestFile";
			String *testAssetclassification = String::Concat(S"AutomatedTesting~", importFilename, S"~", MOG_ControllerSystem::GetComputerName(), S"~TestFile.", Convert::ToString(mAutomatedTesting_AssetIndexNumber));
			// Check if the index is something other than zero?
			if (mAutomatedTesting_AssetIndexNumber != 0)
			{
				// Increment the index
				mAutomatedTesting_AssetIndexNumber++;
			}
			ArrayList *importFiles = new ArrayList();
			importFiles->Add(mAutomatedTesting_TestImportFile);

			// Create the new asset in the MOG Repository
			MOG_Filename *assetFilename = MOG_ControllerAsset::CreateAsset(MOG_Filename::CreateAssetName(testAssetclassification, S"All", testAssetLabel), S"", importFiles, NULL, NULL, false, false);
			if (assetFilename)
			{
// Removed because the "Processed" directory is no more...We need another way to test when an assets has finished ripping
//				// Process the asset
//				if (MOG_ControllerInbox::RipAsset(assetFilename))
//				{
//					// Wait until the asset is processed
//					while (!bCanceled)
//					{
//						// Check to see if the asset has been finished
//						ArrayList *assets = MOG_DBInboxAPI::InboxGetAssetList(new MOG_Filename(assetFilename->GetAssetEncodedInboxPath("\\Processed\\*")));
//						if (assets->Count == 1)
//						{
//							// Make sure we retain the newly processed assetFilename
//							assetFilename = __try_cast<MOG_Filename *>(assets->Item[0]);
//							break;
//						}
//					}
//				}

				// Bless the asset
				String *jobLabel = String::Concat(S"AutomatedTesting.", MOG_ControllerSystem::GetComputerName(), S".", MOG_Time::GetVersionTimestamp());
				if (!MOG_ControllerInbox::BlessAsset(assetFilename, S"AutomatedTesting", false, jobLabel, worker))
				{
					String *message = S"The Drone failed to 'Bless' the asset.";
					MOG_Report::ReportMessage(S"Automatted Testing", message, Environment::StackTrace, MOG_ALERT_LEVEL::ALERT);
				}
			}
			else
			{
				String *message = S"The Drone failed to 'Import' a new asset from the specified file.";
				MOG_Report::ReportMessage(S"Automatted Testing", message, Environment::StackTrace, MOG_ALERT_LEVEL::ALERT);
			}
		}

		// Log in as 'Admin' to indicate we have finished the Automated Testing
		MOG_ControllerProject::LoginUser(S"Admin");
	}
	catch (...)
	{
	}
	__finally
	{
		// Clear our member variable so we will allow another test run to begin
		mAutomatedTesting_Duration = 0;
		mAutomatedTesting_TestBlessPercent = 0;
		mAutomatedTesting_TestProcessPercent = 0;
	}
}


void MOG_SystemUtilities::AutomatedTesting_Drone_LibraryCheckOutIn_Thread()
{
	List<Object*>* args = new List<Object*>();
	args->Add(mAutomatedTesting_TestImportFile);
	args->Add(__box(mAutomatedTesting_Duration));

	// Initialize our progress dialog
	ProgressDialog* progress = new ProgressDialog("Automated Testing", "Please wait while the Client performs the test.", new DoWorkEventHandler(NULL, &AutomatedTesting_Drone_LibraryCheckOutIn_Worker), args, false);
	progress->ShowDialog();
}

void MOG_SystemUtilities::AutomatedTesting_Drone_LibraryCheckOutIn_Worker(Object* sender, DoWorkEventArgs* e)
{
	BackgroundWorker* worker = dynamic_cast<BackgroundWorker*>(sender);
	List<Object*>* args = dynamic_cast<List<Object*>*>(e->Argument);
	String* importPath = dynamic_cast<String*>(args->Item[0]);
	int duration = *dynamic_cast<__box int*>(args->Item[1]);	

	String *importFilename = DosUtils::PathGetFileName(importPath);

	DateTime testStartTime = DateTime::Now;
	DateTime testCompleteTime = DateTime::Now.AddSeconds(duration);
	TimeSpan testDuration = testCompleteTime - testStartTime;

	try
	{		
		MOG_Filename *assetFilename = new MOG_Filename(importFilename);
		assetFilename = MOG_ControllerProject::GetAssetCurrentBlessedPath(assetFilename);
		
		// Repeat loop for the specified iterations
		while (!worker->CancellationPending)
		{
			// Check if we have exceeded our specified time span?
			TimeSpan testTimeRemaining = testCompleteTime - DateTime::Now;
			int testPercent = 100 - Convert::ToInt32((testTimeRemaining.TotalSeconds * 100) / testDuration.TotalSeconds);
			if (testPercent > 100)
			{
				// Time to  end our automated testing loop
				break;
			}

			if (assetFilename)
			{
				if (MOG_ControllerLibrary::CheckOut(assetFilename, S"AutomatedTesting", worker))
				{
					// Verify the checked out asset is the same as the latest version
					String *blessed, *local;
					CheckAssets_VerifyBlessedFile(assetFilename, &blessed, &local);

					// Modify the asset
					FileInfo *localFile = new FileInfo(local);
					if (localFile)
					{
						// Check if this is a txt file?
						if (local->EndsWith(".txt", StringComparison::CurrentCultureIgnoreCase))
						{
							// Append the MachineName and time to the text file
							String *modification = String::Concat(DateTime::Now.ToLongTimeString(), S" - Modified by ", MOG_ControllerSystem::GetComputerName());
							StreamWriter *sw = localFile->AppendText();
							sw->WriteLine(modification);
							sw->Close();
						}
						else
						{
							try
							{
								// Touch the last write time
								localFile->CreationTime = DateTime::Now;
								localFile->LastWriteTime = DateTime::Now;
								localFile->LastAccessTime = DateTime::Now;
							}
							catch(...)
							{
							}
						}
					}

					// Checkin the asset
					String *jobLabel = String::Concat(S"AutomatedTesting.", MOG_ControllerSystem::GetComputerName(), S".", MOG_Time::GetVersionTimestamp());
					if (!MOG_ControllerLibrary::CheckIn(assetFilename, S"AutomatedTesting", false, jobLabel))
					{
						String *message = S"The Drone failed to 'CheckOut' the asset.";
						MOG_Report::ReportMessage(S"Automatted Testing", message, Environment::StackTrace, MOG_ALERT_LEVEL::ALERT);
					}
				}
			}
			else
			{
				String *message = S"Invalid library asset name. Required full asset name (Project~Library~Class~{All}AssetName.txt)";
				MOG_Report::ReportMessage(S"Automatted Testing", message, Environment::StackTrace, MOG_ALERT_LEVEL::ALERT);
			}
		}

		// Log in as 'Admin' to indicate we have finished the Automated Testing
		MOG_ControllerProject::LoginUser(S"Admin");

	}
	catch (...)
	{
	}
	__finally
	{		
		// Clear our member variable so we will allow another test run to begin
		mAutomatedTesting_Duration = 0;
		mAutomatedTesting_TestBlessPercent = 0;
		mAutomatedTesting_TestProcessPercent = 0;
	}
}

void MOG_SystemUtilities::CheckAssets_VerifyBlessedFile(MOG_Filename *assetFilename, String **sourceCopyFile, String **targetCopyFile)
{
	// Open the blessed revision of this asset
	String *revision = MOG_ControllerProject::GetAssetCurrentVersionTimeStamp(assetFilename);
	MOG_Filename *blessedAsset = new MOG_Filename(MOG_ControllerRepository::GetAssetBlessedVersionPath(assetFilename, revision));
	MOG_Properties *assetProperties = new MOG_Properties(assetFilename);

	// Establish the source and target paths
	String *sourcePath = MOG_ControllerAsset::GetAssetProcessedDirectory(assetProperties, "All");
	String *targetPath = MOG_ControllerLibrary::GetWorkingDirectory();
	// Tokenize our local path
	String *localPath = MOG_Tokens::GetFormattedString(assetProperties->SyncTargetPath, blessedAsset, assetProperties->GetApplicableProperties());
	if (localPath->Length > 0)
	{
		targetPath = String::Concat(targetPath, S"\\", localPath);
	}

	// no platform specified, get the source files
	String *filesDirctory = MOG_ControllerAsset::GetAssetProcessedDirectory(assetProperties, "All");
	ArrayList *copyFiles = DosUtils::FileGetRecursiveRelativeList(filesDirctory, S"");
	if (copyFiles && copyFiles->Count)
	{
		for (int j = 0; j < copyFiles->Count; j++)
		{
			String *relativeFile = __try_cast<String*>(copyFiles->Item[j]);

			// Build our source and target file 
			*sourceCopyFile = Path::Combine(sourcePath, relativeFile);
			*targetCopyFile = Path::Combine(targetPath, relativeFile);
			relativeFile = Path::Combine(localPath, relativeFile);

			FileInfo *localFile = NULL;			
			FileInfo *blessedFile = NULL;

			localFile = new FileInfo(*targetCopyFile);			
			blessedFile = new FileInfo(*sourceCopyFile);

			if (localFile->LastWriteTime.ToFileTime() != blessedFile->LastWriteTime.ToFileTime())
			{
				String *message = S"Latest Bessed version does not match local just checked out version!";
				MOG_Report::ReportMessage(S"Automatted Testing", message, Environment::StackTrace, MOG_ALERT_LEVEL::ALERT);
			}
		}
	}
}


void MOG_SystemUtilities::AutomatedTesting_Drone_CopyNetworkFiles_Thread()
{
	List<Object*>* args = new List<Object*>();
	args->Add(__box(mAutomatedTesting_Duration));

	// Initialize our progress dialog
	ProgressDialog* progress = new ProgressDialog("Automated Testing", "Please wait while the Client performs the test.", new DoWorkEventHandler(NULL, &AutomatedTesting_Drone_ImportProcessBless_Worker), args, false);
	progress->ShowDialog();
}

void MOG_SystemUtilities::AutomatedTesting_Drone_CopyNetworkFiles_Worker(Object* sender, DoWorkEventArgs* e)
{
	BackgroundWorker* worker = dynamic_cast<BackgroundWorker*>(sender);
	List<Object*>* args = dynamic_cast<List<Object*>*>(e->Argument);
	int duration = *dynamic_cast<__box int*>(args->Item[0]);	

	DateTime testStartTime = DateTime::Now;
	DateTime testCompleteTime = DateTime::Now.AddSeconds(duration);
	TimeSpan testDuration = testCompleteTime - testStartTime;
	
	try
	{
		// Repeat loop for the specified iterations
		int count = 0;
		while (!worker->CancellationPending)
		{
			// Check if we have exceeded out specified time span?
			TimeSpan testTimeRemaining = testCompleteTime - DateTime::Now;
			int testPercent = 100 - Convert::ToInt32((testTimeRemaining.TotalSeconds * 100) / testDuration.TotalSeconds);
			if (testPercent > 100)
			{
				// Time to  end our automated testing loop
				break;
			}

			// Setup the new target file
			String *sourcePath = DosUtils::PathGetDirectoryPath(mAutomatedTesting_TestImportFile);
			String *sourceFilename = DosUtils::PathGetDirectoryPath(mAutomatedTesting_TestImportFile);
			String *targetPath = String::Concat(sourcePath, S"\\", MOG_ControllerSystem::GetComputerName());
			String *targetFilename = Convert::ToString(count++);
			String *target = String::Concat(targetPath, S"\\", targetFilename);

			if (DosUtils::FileCopyFast(mAutomatedTesting_TestImportFile, target, true))
			{
			}
		}

		// Log in as 'Admin' to indicate we have finished the Automated Testing
		MOG_ControllerProject::LoginUser(S"Admin");
	}
	catch (...)
	{
	}
	__finally
	{
		// Clear our member variable so we will allow another test run to begin
		mAutomatedTesting_Duration = 0;
		mAutomatedTesting_TestBlessPercent = 0;
		mAutomatedTesting_TestProcessPercent = 0;
	}
}


bool MOG_SystemUtilities::AutomatedTesting_UserInbox(int testDuration_Seconds, int testBlessPercent, int testProcessPercent)
{
	// Make sure it is safe to begin our automated test thread?
	if (mAutomatedTesting_Duration == 0)
	{
		// Setup our variables for this new automated test run
		mAutomatedTesting_Duration = testDuration_Seconds;
		mAutomatedTesting_TestBlessPercent = testBlessPercent;
		mAutomatedTesting_TestProcessPercent = testProcessPercent;

		// Start the new thread
		ThreadStart *threadDelegate = new ThreadStart(0, &MOG_SystemUtilities::AutomatedTesting_UserInbox_Thread);
		Thread *newThread = new Thread(threadDelegate);
		newThread->Start();
		return true;
	}

	return false;
}


void MOG_SystemUtilities::AutomatedTesting_UserInbox_Thread()
{
	List<Object*>* args = new List<Object*>();
	args->Add(MOG_ControllerProject::GetUser()->GetUserName());
	args->Add(MOG_ControllerProject::GetUser()->GetUserPath());
	args->Add(__box(mAutomatedTesting_Duration));

	// Initialize our progress dialog
	ProgressDialog* progress = new ProgressDialog("Automated Testing", "Please wait while the Client performs the test.", new DoWorkEventHandler(NULL, &AutomatedTesting_Drone_LibraryCheckOutIn_Worker), args, false);
	progress->ShowDialog();
}

void MOG_SystemUtilities::AutomatedTesting_UserInbox_Worker(Object* sender, DoWorkEventArgs* e)
{
	BackgroundWorker* worker = dynamic_cast<BackgroundWorker*>(sender);
	List<Object*>* args = dynamic_cast<List<Object*>*>(e->Argument);
	String *currentUser = dynamic_cast<String*>(args->Item[0]);
	String *currentUserPath = dynamic_cast<String*>(args->Item[1]);
	int duration = *dynamic_cast<__box int*>(args->Item[2]);
	
	DateTime testStartTime = DateTime::Now;
	DateTime testCompleteTime = DateTime::Now.AddSeconds(duration);
	TimeSpan testDuration = testCompleteTime - testStartTime;

	// Repeat loop for the specified iterations
	while (!worker->CancellationPending)
	{
		MOG_Filename *assetFilename = NULL;

		// Check if we have exceeded out specified time span?
		TimeSpan testTimeRemaining = testCompleteTime - DateTime::Now;
		int testPercent = 100 - Convert::ToInt32((testTimeRemaining.TotalSeconds * 100) / testDuration.TotalSeconds);
		if (testPercent > 100)
		{
			// Time to end our automated testing loop
			break;
		}

		// Bless an asset from the user's processed?
		if ((rand() % 100) < mAutomatedTesting_TestBlessPercent)
		{
			// Get all of our processed assets
			ArrayList *assets = new ArrayList();
// JohnRen - Can't look in our Inbox because we need to make a copy of the asset in our Inbox before we bless it.
//			assets->AddRange(MOG_DBInboxAPI::InboxGetAssetList(new MOG_Filename(String::Concat(currentUserPath, S"\\InBox\\*"))));
			assets->AddRange(MOG_DBInboxAPI::InboxGetAssetList(new MOG_Filename(String::Concat(currentUserPath, S"\\Drafts\\*"))));
			if (assets && assets->Count)
			{
				// Pick a random asset
				assetFilename = __try_cast<MOG_Filename *>(assets->Item[rand() % assets->Count]);

				// Send this asset back to ourselves so that it will be preserved in our inbox
				if (MOG_ControllerInbox::CopyAssetToUserInbox(assetFilename, currentUser, S"Blessed by the 'AutoTest' feature.", MOG_AssetStatusType::Sent, worker))
				{
					// Bless the asset
					String *jobLabel = String::Concat(S"AutomatedTesting.", MOG_ControllerSystem::GetComputerName(), S".", MOG_Time::GetVersionTimestamp());
					if (MOG_ControllerInbox::BlessAsset(assetFilename, S"AutomatedTesting", false, jobLabel, worker))
					{
					}
				}
			}
		}

		// Process an asset?
		if ((rand() % 100) < mAutomatedTesting_TestProcessPercent)
		{
			// Get all the assets available for testing
			ArrayList *assets = new ArrayList();
			assets->AddRange(MOG_DBInboxAPI::InboxGetAssetList(new MOG_Filename(String::Concat(currentUserPath, S"\\Inbox\\*"))));
			assets->AddRange(MOG_DBInboxAPI::InboxGetAssetList(new MOG_Filename(String::Concat(currentUserPath, S"\\Drafts\\*"))));
			if (assets && assets->Count)
			{
				// Pick a random asset
				assetFilename = __try_cast<MOG_Filename *>(assets->Item[rand() % assets->Count]);

				// Process the asset
				if (MOG_ControllerInbox::RipAsset(assetFilename))
				{
				}
			}
		}
	}

	// Clear our member variable so we will allow another test run to begin
	mAutomatedTesting_Duration = 0;
	mAutomatedTesting_TestBlessPercent = 0;
	mAutomatedTesting_TestProcessPercent = 0;
}


bool MOG_SystemUtilities::FixGameDataTable_MissingSlashes()
{
	return true;
}


ArrayList* MOG_SystemUtilities::AdminToolsReportUnreferencedRevisions(String *classification)
{
	// Initialize our progress dialog
	ProgressDialog* progress = new ProgressDialog(S"Scanning for unreferenced revisions in project...", S"Please wait while the project is scanned for unreferenced revisions.", new DoWorkEventHandler(NULL, &AdminToolsReportUnreferencedRevsions_Worker), classification, false);
	progress->ShowDialog();

	ArrayList* results = dynamic_cast<ArrayList*>(progress->WorkerResult);

	return results;
}

void MOG_SystemUtilities::AdminToolsReportUnreferencedRevsions_Worker(Object* sender, DoWorkEventArgs* e)
{
	BackgroundWorker* worker = dynamic_cast<BackgroundWorker*>(sender);
	String* classification = dynamic_cast<String*>(e->Argument);
	ArrayList* unreferencedRevisions = new ArrayList();
	String *comment = "Archived revision using Admin Tools.";

	// First get a complete list of assets and their revisions contained within the specified classification
	ArrayList* revisions = MOG_DBAssetAPI::GetAllArchivedAssetsByParentClassification(classification);
	// Make sure both lists are sorted because of some following optimizations
	revisions->Sort();

	// Walk the list of revisions
	for (int r = 0; r < revisions->Count; r++)
	{
		MOG_Filename* revisionFilename = dynamic_cast<MOG_Filename*>(revisions->Item[r]);
		if (revisionFilename)
		{
			String* message = String::Concat(revisionFilename->GetAssetClassification(), S"\n",
											 revisionFilename->GetAssetLabel());

			if (worker != NULL)
			{
				worker->ReportProgress((r * 100) / revisions->Count, message);
			}

			// Check if this revision is in use?
			if (MOG_DBAssetAPI::GetAssetRevisionReferences(revisionFilename, revisionFilename->GetVersionTimeStamp(), false)->Count == 0)
			{
				// Track this unreferenced revision
				unreferencedRevisions->Add(revisionFilename);
			}
		}

		if (worker != NULL)
		{
			// Check for user cancel
			if (worker->CancellationPending)
			{
				break;
			}
		}
	}

	e->Result = unreferencedRevisions;
}

bool MOG_SystemUtilities::CleanInvalidItems()
{
	// Initialize our progress dialog
	ProgressDialog* progress = new ProgressDialog(S"Cleaning Project...", S"Please wait while the project is scanned and invalid items are removed.", new DoWorkEventHandler(NULL, &CleanInvalidItems_Worker), NULL, false);
	progress->ShowDialog();

	return true;
}

void MOG_SystemUtilities::CleanInvalidItems_Worker(Object* sender, DoWorkEventArgs* e)
{
	HybridDictionary *invalidClassifications = new HybridDictionary();
	HybridDictionary *invalidAssets = new HybridDictionary();

	// Get all of the project's classifications
	ArrayList *classifications = MOG_DBAssetAPI::GetAllClassifications();
	// Loop throught all of the project's classifications
	for (int c = 0; c < classifications->Count; c++)
	{
		String *classification = dynamic_cast<String *>(classifications->Item[c]);

		// Check if this classification doesn't begin with the project's name?
		if (!classification->StartsWith(MOG_ControllerProject::GetProjectName(), StringComparison::CurrentCultureIgnoreCase))
		{
			// Add this classification for removal
			invalidClassifications->Item[classification] = classification;
		}
		// Check if this classification ends with a '~'?
		else if (classification->EndsWith("~", StringComparison::CurrentCultureIgnoreCase))
		{
			// Add this classification for removal
			invalidClassifications->Item[classification] = classification;
		}
	}

	// Get all of the assets within the entire project
	ArrayList *assets = MOG_DBAssetAPI::GetAllAssets();
	// Loop through all of the assets looking for one matching our invalidClassifications
	for (int a = 0; a < assets->Count; a++)
	{
		MOG_Filename *assetFilename = dynamic_cast<MOG_Filename *>(assets->Item[a]);

		// Check if this asset's classification is contained in our list of invalidClassifications
		if (invalidClassifications->Contains(assetFilename->GetAssetClassification()))
		{
			// Add this asset for removal
			invalidAssets->Item[assetFilename->GetOriginalFilename()] = assetFilename;
		}
		// Check if this assetFilename contains a bogus revision?
		else if (!assetFilename->GetVersionTimeStamp()->Length)
		{
			// Add this asset for removal
			invalidAssets->Item[assetFilename->GetOriginalFilename()] = assetFilename;
		}
	}


	// Remove all of the bogus assets
	IDictionaryEnumerator *eAsset = invalidAssets->GetEnumerator();
	while(eAsset->MoveNext())
	{
		MOG_Filename *assetFilename = dynamic_cast<MOG_Filename *>(eAsset->Value);
		if (assetFilename)
		{
			String *removeRevision = assetFilename->GetVersionTimeStamp();
			if (!removeRevision->Length)
			{
				removeRevision = MOG_ControllerProject::GetAssetCurrentVersionTimeStamp(assetFilename);
			}
			// Check if we were able to obtain a specific revision?
			if (removeRevision->Length)
			{
				// Remove this revision of this asset
				MOG_Filename *tempBlessedAssetFilename = MOG_ControllerRepository::GetAssetBlessedVersionPath(assetFilename, removeRevision);
				MOG_Filename *tempArchiveAssetFilename = MOG_ControllerRepository::GetAssetArchivedVersionPath(assetFilename, removeRevision);
				// Remove the revision from the database
				if (MOG_DBAssetAPI::RemoveAssetVersion(tempArchiveAssetFilename, tempArchiveAssetFilename->GetVersionTimeStamp()))
				{
					// Move the directory over to the archive
					if (DosUtils::DirectoryMoveFast(tempBlessedAssetFilename->GetEncodedFilename(), tempArchiveAssetFilename->GetEncodedFilename(), true))
					{
					}
				}
			}
			// Remove the name of this asset from our database
			MOG_DBAssetAPI::RemoveAssetName(assetFilename);
		}
	}

	// Remove all of the bogus classifications
	IDictionaryEnumerator *eClassification = invalidClassifications->GetEnumerator();
	while(eClassification->MoveNext())
	{
		String *invalidClassification = dynamic_cast<String *>(eClassification->Value);

		// Remove this bogus classification from our project
		if (MOG_DBAssetAPI::RemoveClassification(invalidClassification))
		{
			// Move any remnents of this classification in the project's repository to the archive
			int invalidClassificationID = MOG_DBAssetAPI::GetClassificationID(invalidClassification);
			if (invalidClassificationID)
			{
				String *source = String::Concat(MOG_ControllerRepository::GetRepositoryPath(), S"\\@(", invalidClassificationID.ToString(), S")");
				String *target = String::Concat(MOG_ControllerRepository::GetArchivePath(), S"\\@(", invalidClassificationID.ToString(), S")");
				DosUtils::DirectoryMoveFast(source, target, true);
			}
		}
	}
	
	// Check for duplicate AssetNameIDs
	HybridDictionary *duplicateAssetNames = new HybridDictionary();
	// Get all of the assets within the entire project
	ArrayList *assetNames = MOG_DBAssetAPI::GetAssetNames();
	// Loop through the assets checking for duplicate names
	for (int a = 0; a < assetNames->Count; a++)
	{
		MOG_Filename *assetFilename = dynamic_cast<MOG_Filename *>(assetNames->Item[a]);

		// Check if this asset hasn't been added yet?
		if (!duplicateAssetNames->Contains(assetFilename->GetAssetFullName()))
		{
			// Add this asset to our duplicateAssets list
			duplicateAssetNames->Item[assetFilename->GetAssetFullName()] = assetFilename;
		}
		else
		{
			// Resolved for this duplicated AssetNameID
			ArrayList *IDs = MOG_DBAssetAPI::GetAssetNameIDs(assetFilename);

			// We should have more than one
			if (IDs->Count > 1)
			{
				int lastID = *dynamic_cast<__box int*>(IDs->Item[IDs->Count - 1]);

				// Remove all of the duplicates...Leaving the last one
				for (int i = 0; i < IDs->Count - 1; i++)
				{
					int thisID = *dynamic_cast<__box int*>(IDs->Item[i]);

					// Update any asset versions pointing to this duplicate ID
					MOG_DBAssetAPI::UpdateRevisionsWithReplacementAssetNameID(thisID, lastID);
					// Remove this AssetNameID
					MOG_DBAssetAPI::RemoveAssetName(thisID);
				}
			}
		}
	}

	// Check for duplicate BranchLinks
	HybridDictionary *duplicateAssets = new HybridDictionary();
	// Get all of the assets within the entire project
	assets = MOG_DBAssetAPI::GetAllAssets();
	// Loop through the assets checking for duplicate names
	for (int a = 0; a < assets->Count; a++)
	{
		MOG_Filename *thisAssetFilename = dynamic_cast<MOG_Filename *>(assets->Item[a]);

		// Check if this asset hasn't been added yet?
		if (!duplicateAssets->Contains(thisAssetFilename->GetAssetFullName()))
		{
			// Add this asset to our duplicateAssets list
			duplicateAssets->Item[thisAssetFilename->GetAssetFullName()] = thisAssetFilename;
		}
		else
		{
			// Get the previousAssetFilename that was already encountered
			MOG_Filename *previousAssetFilename = dynamic_cast<MOG_Filename *>(duplicateAssets->Item[thisAssetFilename->GetAssetFullName()]);

			// Check if thisAssetFilename is newer than previousAssetFilename?
			if (String::Compare(previousAssetFilename->GetVersionTimeStamp(), thisAssetFilename->GetVersionTimeStamp(), true) < 0)
			{
				// Remove this asset from the branch
				MOG_DBAssetAPI::RemoveAssetFromBranch(previousAssetFilename, MOG_ControllerProject::GetBranchName());
				// Replace previousAssetFilename currently in the duplicateAssets list
				duplicateAssets->Item[previousAssetFilename->GetAssetFullName()] = thisAssetFilename;
			}
			else
			{
				// Remove this asset from the branch
				MOG_DBAssetAPI::RemoveAssetFromBranch(thisAssetFilename, MOG_ControllerProject::GetBranchName());
			}
		}
	}

	// Clean out any orphaned branch links from previously purged branches
	String *whereCmd = S"";
	ArrayList *branches = MOG_DBProjectAPI::GetAllBranchNames();
	for (int i = 0; i < branches->Count; i++)
	{
		MOG_DBBranchInfo *branchInfo = dynamic_cast<MOG_DBBranchInfo*>(branches->Item[i]);
		if (whereCmd->Length)
		{
			whereCmd = String::Concat(whereCmd, S" AND ");
		}
		whereCmd = String::Concat(whereCmd, S" (BranchID <> ", Convert::ToString(branchInfo->mID), S") ");
	}
	ArrayList *transactionCommands = new ArrayList();
	transactionCommands->Add(String::Concat(S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetBranchLinksTable(), S" ",
											S"WHERE ", whereCmd));
	MOG_DBAPI::ExecuteTransaction(transactionCommands);

	// Repair all package assignments in the project that contain the extra '~' after the classification
	String *searchString = String::Concat(MOG_ControllerProject::GetProjectName(), S"*~{*");
	ArrayList *properties = new ArrayList();
	properties->Add(MOG_PropertyFactory::MOG_Relationships::New_PackageAssignment(searchString, "", "", false));
	ArrayList *associatedAssets = MOG_DBAssetAPI::GetAllArchivedAssetsByClassificationAndProperties(MOG_ControllerProject::GetProjectName(), properties, true);
	for (int i = 0; i < associatedAssets->Count; i++)
	{
		MOG_Filename *assetFilename = dynamic_cast<MOG_Filename*>(associatedAssets->Item[i]);
		MOG_Filename *blessedFilename = MOG_ControllerRepository::GetAssetBlessedVersionPath(assetFilename, assetFilename->GetVersionTimeStamp());

		MOG_ControllerAsset *asset = MOG_ControllerAsset::OpenAsset(blessedFilename);
		if (asset)
		{
			ArrayList *packages = asset->GetProperties()->GetPackages();
			if (packages && packages->Count)
			{
				// Clear out all the package assignments
				asset->GetProperties()->RemovePackages();

				// Repair and readd all of these package assignments
				for (int packageIndex = 0; packageIndex < packages->Count; packageIndex++)
				{
					// Repair this package assignment
					MOG_Property *packageAssignment = __try_cast<MOG_Property*>(packages->Item[packageIndex]);
					String *packageName = MOG_ControllerPackage::GetPackageName(packageAssignment->mPropertyKey);
					String *packageGroups = MOG_ControllerPackage::GetPackageGroups(packageAssignment->mPropertyKey);
					String *packageObjects = MOG_ControllerPackage::GetPackageObjects(packageAssignment->mPropertyKey);
					MOG_Filename *packageFilename = new MOG_Filename(packageName);
					MOG_Property *repairedAssignment = MOG_PropertyFactory::MOG_Relationships::New_PackageAssignment(packageFilename->GetAssetFullName(), packageGroups, packageObjects, false);
					// Readd this repaired package assignment
					asset->GetProperties()->AddPackage(repairedAssignment);
				}
			}

			// Restamp this asset's properties
			MOG_DBAssetAPI::RemoveAllAssetVersionProperties(blessedFilename, blessedFilename->GetVersionTimeStamp());
			MOG_DBAssetAPI::AddAssetVersionProperties(blessedFilename, blessedFilename->GetVersionTimeStamp(), asset->GetProperties()->GetNonInheritedProperties());

			// Make sure we close our asset's controller
			asset->Close();
		}
	}
}


ArrayList* MOG_SystemUtilities::RepairRevisionMetadata(String* classification)
{
	// Initialize our progress dialog
	ProgressDialog* progress = new ProgressDialog(S"Scanning Revisions...", S"Please wait while the project is scanned.", new DoWorkEventHandler(NULL, &RepairRevisionMetadata_Worker), classification, true);
	progress->ShowDialog();

	ArrayList* results = dynamic_cast<ArrayList*>(progress->WorkerResult);

	return results;
}

void MOG_SystemUtilities::RepairRevisionMetadata_Worker(Object* sender, DoWorkEventArgs* e)
{
	BackgroundWorker* worker = dynamic_cast<BackgroundWorker*>(sender);
	String* classification = dynamic_cast<String*>(e->Argument);

	ArrayList* repairedRevisions = new ArrayList();

	// First get a complete list of assets and their revisions contained within the specified classification
	ArrayList *propertiesToInclude = new ArrayList();
	propertiesToInclude->Add(MOG_PropertyFactory::MOG_Asset_StatsProperties::New_Creator(""));
	propertiesToInclude->Add(MOG_PropertyFactory::MOG_Asset_StatsProperties::New_CreatedTime(""));
	propertiesToInclude->Add(MOG_PropertyFactory::MOG_Asset_StatsProperties::New_Status(""));
	propertiesToInclude->Add(MOG_PropertyFactory::MOG_Asset_StatsProperties::New_SourceMachine(""));
	propertiesToInclude->Add(MOG_PropertyFactory::MOG_Asset_StatsProperties::New_SourcePath(""));
	ArrayList* revisions = MOG_DBAssetAPI::GetAllArchivedAssetsByParentClassificationWithProperties(classification, propertiesToInclude);

	// Walk the list of revisions
	for (int r = 0; r < revisions->Count; r++)
	{
		MOG_DBAssetProperties* dbAssetProperties = dynamic_cast<MOG_DBAssetProperties*>(revisions->Item[r]);
		if (dbAssetProperties)
		{
			MOG_Filename* revisionFilename = dbAssetProperties->mAsset;

			String* message = String::Concat(revisionFilename->GetAssetClassification(), S"\n",
											 revisionFilename->GetAssetLabel());

			if (worker != NULL)
			{
				worker->ReportProgress((r * 100) / revisions->Count, message);
			}

			// Check if this revision has no properties?
			// Every asset should always have some uninheritable properties
			if (dbAssetProperties->mProperties->Count == 0)
			{
				// Open this blessed asset
				MOG_Filename *blessedFilename = MOG_ControllerRepository::GetAssetBlessedVersionPath(revisionFilename, revisionFilename->GetVersionTimeStamp());

				// Track this asset as one being repaired
				repairedRevisions->Add(blessedFilename);

				// Open this blessed revision
				MOG_ControllerAsset* asset = MOG_ControllerAsset::OpenAsset(blessedFilename);
				if (asset)
				{
					// Add this asset to the database now that it exists in the MOG Repository
					if (MOG_DBAssetAPI::AddAsset(asset->GetAssetFilename(), asset->GetProperties()->Creator, asset->GetAssetFilename()->GetVersionTimeStamp()))
					{
						// Add the asset's revision information
						if (MOG_ControllerRepository::AddAssetRevisionInfo(asset, true))
						{
						}
					}
					asset->Close();
				}
			}
		}

		if (worker != NULL)
		{
			// Check for user cancel
			if (worker->CancellationPending)
			{
				break;
			}
		}
	}

	e->Result = repairedRevisions;
}


ArrayList* MOG_SystemUtilities::FindNewerUnpostedRevisions(String* classification)
{
	// Initialize our progress dialog
	ProgressDialog* progress = new ProgressDialog(S"Scanning Repository...", S"Please wait while the project is scanned.", new DoWorkEventHandler(NULL, &FindNewerUnpostedRevisions_Worker), classification, true);
	progress->ShowDialog();

	ArrayList* results = dynamic_cast<ArrayList*>(progress->WorkerResult);

	return results;
}

void MOG_SystemUtilities::FindNewerUnpostedRevisions_Worker(Object* sender, DoWorkEventArgs* e)
{
	BackgroundWorker* worker = dynamic_cast<BackgroundWorker*>(sender);
	String* classification = dynamic_cast<String*>(e->Argument);

	ArrayList* newerRevisions = new ArrayList();
	// First get a complete list of assets and their revisions contained within the specified classification
	ArrayList* revisions = MOG_DBAssetAPI::GetAllArchivedAssetsByParentClassification(classification);
	ArrayList* assets = MOG_DBAssetAPI::GetAllCurrentAssetsByClassification(classification, false);

	// Make sure both lists are sorted because of some following optimizations
	revisions->Sort();
	assets->Sort();

	// Walk the list of current assets
	int revisionOffset = 0;
	for (int a = 0; a < assets->Count; a++)
	{
		MOG_Filename* assetFilename = dynamic_cast<MOG_Filename*>(assets->Item[a]);
		if (assetFilename)
		{
			String* message = String::Concat(assetFilename->GetAssetClassification(), S"\n",
											 assetFilename->GetAssetLabel());

			if (worker != NULL)
			{
				worker->ReportProgress((a * 100) / assets->Count, message);
			}

			// Scan the list of revisions looking for a newer revision
			for (int r = revisionOffset; r < revisions->Count; r++)
			{
				MOG_Filename* revisionFilename = dynamic_cast<MOG_Filename*>(revisions->Item[r]);
				if (revisionFilename)
				{
					// Check if this revision matches the asset?
					int value = String::Compare(revisionFilename->GetAssetFullName(), assetFilename->GetAssetFullName(), true);
					if (value == 0)
					{
						// Check if this revision is newer than our asset's revision?
						if (String::Compare(revisionFilename->GetVersionTimeStamp(), assetFilename->GetVersionTimeStamp(), true) > 0)
						{
							// Add this newer revision
							MOG_Filename *blessedFilename = MOG_ControllerRepository::GetAssetBlessedVersionPath(revisionFilename, revisionFilename->GetVersionTimeStamp());
							newerRevisions->Add(blessedFilename);
						}

						// Move onto the next revision
						revisionOffset++;
					}
					else if (value < 0)
					{
						// Move onto the next revision
						revisionOffset++;
					}
					else if (value > 0)
					{
						break;
					}
				}
			}
		}

		if (worker != NULL)
		{
			// Check for user cancel
			if (worker->CancellationPending)
			{
				break;
			}
		}
	}

	e->Result = newerRevisions;
}

ArrayList* MOG_SystemUtilities::FindAssetWithMultiplePackageAssignments(String* classification)
{
	// Initialize our progress dialog
	ProgressDialog* progress = new ProgressDialog(S"Scanning Repository...", S"Please wait while the project is scanned.", new DoWorkEventHandler(NULL, &FindAssetWithMultiplePackageAssignments_Worker), classification, true);
	progress->ShowDialog();

	ArrayList* results = dynamic_cast<ArrayList*>(progress->WorkerResult);

	return results;
}

void MOG_SystemUtilities::FindAssetWithMultiplePackageAssignments_Worker(Object* sender, DoWorkEventArgs* e)
{
	BackgroundWorker* worker = dynamic_cast<BackgroundWorker*>(sender);
	String* classification = dynamic_cast<String*>(e->Argument);

	ArrayList* found = new ArrayList();
	ArrayList* assets = MOG_DBAssetAPI::GetAllCurrentAssetsByClassification(classification, false);
	// Make sure the lists is sorted because it looks better
	assets->Sort();

	// Walk the list of current assets
	int revisionOffset = 0;
	for (int a = 0; a < assets->Count; a++)
	{
		MOG_Filename* assetFilename = dynamic_cast<MOG_Filename*>(assets->Item[a]);
		if (assetFilename)
		{
			String* message = String::Concat(assetFilename->GetAssetClassification(), S"\n",
											 assetFilename->GetAssetLabel());

			if (worker != NULL)
			{
				worker->ReportProgress((a * 100) / assets->Count, message);
			}

			// Get this asset's propeerties
			MOG_Properties *properties = new MOG_Properties(assetFilename);
			if (properties != NULL)
			{
				// Get the package assignments of this asset
				ArrayList *packages = properties->GetPackages();
				if (packages != NULL &&
					packages->Count > 1)
				{
					MOG_Filename* blessedFilename = MOG_ControllerRepository::GetAssetBlessedVersionPath(assetFilename, assetFilename->GetVersionTimeStamp());
					found->Add(blessedFilename);
				}
			}
		}

		if (worker != NULL)
		{
			// Check for user cancel
			if (worker->CancellationPending)
			{
				break;
			}
		}
	}

	e->Result = found;
}

ArrayList* MOG_SystemUtilities::FindCollidingPackageAssignments(String* classification)
{
	// Initialize our progress dialog
	ProgressDialog* progress = new ProgressDialog(S"Scanning Repository...", S"Please wait while the project is scanned.", new DoWorkEventHandler(NULL, &FindCollidingPackageAssignments_Worker), classification, true);
	progress->ShowDialog();

	ArrayList* results = dynamic_cast<ArrayList*>(progress->WorkerResult);

	return results;
}

void MOG_SystemUtilities::FindCollidingPackageAssignments_Worker(Object* sender, DoWorkEventArgs* e)
{
	BackgroundWorker* worker = dynamic_cast<BackgroundWorker*>(sender);
	String* classification = dynamic_cast<String*>(e->Argument);

	ArrayList* found = new ArrayList();
	HybridDictionary* packageAssignments = new HybridDictionary(true);
	ArrayList* assets = MOG_DBAssetAPI::GetAllCurrentAssetsByClassification(classification, false);
	// Make sure the lists is sorted because it looks better
	assets->Sort();

	// Walk the list of current assets
	int revisionOffset = 0;
	for (int a = 0; a < assets->Count; a++)
	{
		MOG_Filename* assetFilename = dynamic_cast<MOG_Filename*>(assets->Item[a]);
		if (assetFilename)
		{
			String* message = String::Concat(assetFilename->GetAssetClassification(), S"\n",
											 assetFilename->GetAssetLabel());

			if (worker != NULL)
			{
				worker->ReportProgress((a * 100) / assets->Count, message);
			}

			// Get this asset's properties
			MOG_Properties *properties = new MOG_Properties(assetFilename);
			if (properties != NULL)
			{
				// Check if this asset should veerify unique package assignments?
				if (properties->UniquePackageAssignmentVerification)
				{
					// Get the package assignments of this asset
					ArrayList *packages = properties->GetPackages();
					if (packages != NULL)
					{
						for (int p = 0; p < packages->Count; p++)
						{
							MOG_Property *packageAssignmentProperty = __try_cast<MOG_Property*>(packages->Item[p]);

							// Make sure this doesn't already exist?
							String* propertyString = packageAssignmentProperty->GetPropertyAsString();
							// Get the package assignment parts
							String *packageName = MOG_ControllerPackage::GetPackageName(propertyString);
							String *packageGroup = MOG_ControllerPackage::GetPackageGroups(propertyString);
							String *packageObject = MOG_ControllerPackage::GetPackageObjects(propertyString);
							// Check if this package object is missing?
							if (packageObject->Length == 0)
							{
								// Use the asset's label as the default packageGroup
								packageObject = assetFilename->GetAssetLabel();
								propertyString = MOG_ControllerPackage::CombinePackageAssignment(packageName, packageGroup, packageObject);
							}

// JohnRen - This logic is flawed!!!  It works alright in the simple scenerios but not in the more complicated ones...
// Each project can structure their package commands dynamically.
// They might use the asset label, ripped asset filenames, package objects or even a hard coded target in the package command.
// Also, what about platform-specific packages either being explicitly specified or implied by the 'All' package assignment?
// I'm not sure how we can compare the package assignments to know when something actually collides because I would need to interpret their package command as if was their packager.

							if (packageAssignments->Contains(propertyString))
							{
								MOG_Filename* collidingAssetA = dynamic_cast<MOG_Filename*>(packageAssignments->Item[propertyString]);
								MOG_Filename* blessedFilenameA = MOG_ControllerRepository::GetAssetBlessedVersionPath(collidingAssetA, collidingAssetA->GetVersionTimeStamp());
								found->Add(blessedFilenameA);

								MOG_Filename* blessedFilenameB = MOG_ControllerRepository::GetAssetBlessedVersionPath(assetFilename, assetFilename->GetVersionTimeStamp());
								found->Add(blessedFilenameB);
							}

							// Add this asset
							packageAssignments->Item[propertyString] = assetFilename;
						}
					}
				}
			}
		}

		if (worker != NULL)
		{
			// Check for user cancel
			if (worker->CancellationPending)
			{
				break;
			}
		}
	}

	e->Result = found;
}

ArrayList* MOG_SystemUtilities::FindInvalidPackageAssignments(String* classification)
{
	// Initialize our progress dialog
	ProgressDialog* progress = new ProgressDialog(S"Scanning Repository...", S"Please wait while the project is scanned.", new DoWorkEventHandler(NULL, &FindInvalidPackageAssignments_Worker), classification, true);
	progress->ShowDialog();

	ArrayList* results = dynamic_cast<ArrayList*>(progress->WorkerResult);

	return results;
}

void MOG_SystemUtilities::FindInvalidPackageAssignments_Worker(Object* sender, DoWorkEventArgs* e)
{
	BackgroundWorker* worker = dynamic_cast<BackgroundWorker*>(sender);
	String* classification = dynamic_cast<String*>(e->Argument);

	ArrayList* found = new ArrayList();
	HybridDictionary* packageAssignments = new HybridDictionary(true);
	ArrayList* assets = MOG_DBAssetAPI::GetAllCurrentAssetsByClassification(classification, false);
	// Make sure the lists is sorted because it looks better
	assets->Sort();

	// Walk the list of current assets
	int revisionOffset = 0;
	for (int a = 0; a < assets->Count; a++)
	{
		MOG_Filename* assetFilename = dynamic_cast<MOG_Filename*>(assets->Item[a]);
		if (assetFilename)
		{
			String* message = String::Concat(assetFilename->GetAssetClassification(), S"\n",
											 assetFilename->GetAssetLabel());

			if (worker != NULL)
			{
				worker->ReportProgress((a * 100) / assets->Count, message);
			}

			// Get this asset's properties
			MOG_Properties *properties = new MOG_Properties(assetFilename);
			if (properties != NULL)
			{
				// Check if this asset is a packaged asset?
				if (properties->IsPackagedAsset)
				{
					bool bInvalidPackageAssignment = false;

					// Get the package assignments of this asset
					ArrayList *packages = properties->GetPackages();
					if (packages != NULL)
					{
						// We should always have at least one package assignment!
						if (packages->Count > 0)
						{
							for (int p = 0; p < packages->Count; p++)
							{
								MOG_Property *packageAssignmentProperty = __try_cast<MOG_Property*>(packages->Item[p]);
								String *packageName = MOG_ControllerPackage::GetPackageName(packageAssignmentProperty->mPropertyKey);
								MOG_Filename* packageFilename = new MOG_Filename(packageName);

								// Check if this package doesn't exists in the project
								if (!MOG_ControllerProject::DoesAssetExists(packageFilename))
								{
									bInvalidPackageAssignment = true;
									break;
								}
								// Check if this platform isn'y valid for this asset
								else if (!MOG_ControllerProject::IsPlatformValidForAsset(assetFilename, packageFilename->GetAssetPlatform()))
								{
									bInvalidPackageAssignment = true;
									break;
								}
							}
						}
						else
						{
							bInvalidPackageAssignment = true;
						}
					}
					else
					{
						bInvalidPackageAssignment = true;
					}

					// CHeck if this asest was found to have an invalid package assignment?
					if (bInvalidPackageAssignment)
					{
						// Add this to our list of found assets
						MOG_Filename* invalidFilename = MOG_ControllerRepository::GetAssetBlessedVersionPath(assetFilename, assetFilename->GetVersionTimeStamp());
						found->Add(invalidFilename);
					}
				}
			}
		}

		if (worker != NULL)
		{
			// Check for user cancel
			if (worker->CancellationPending)
			{
				break;
			}
		}
	}

	e->Result = found;
}

bool MOG_SystemUtilities::UpdateProjectFor304XDrop()
{
	// Initialize our progress dialog
	ProgressDialog* progress = new ProgressDialog(S"Updating Project For 3.0.4.X Drop...", S"Please wait while the project is scanned.", new DoWorkEventHandler(NULL, &UpdateProjectFor304XDrop_Worker), NULL, true);
	progress->ShowDialog();

	return true;
}

void MOG_SystemUtilities::UpdateProjectFor304XDrop_Worker(Object* sender, DoWorkEventArgs* e)
{
	BackgroundWorker* worker = dynamic_cast<BackgroundWorker*>(sender);

	String* classification = MOG_ControllerProject::GetProjectName();
	// First get a complete list of assets and their revisions contained within the specified classification
	ArrayList* revisions = MOG_DBAssetAPI::GetAllArchivedAssetsByParentClassification(classification);
	revisions->Sort();

	// Scan the list of revisions looking for a newer revision
	for (int r = 0; r < revisions->Count; r++)
	{
		MOG_Filename* revisionFilename = dynamic_cast<MOG_Filename*>(revisions->Item[r]);
		if (revisionFilename)
		{
			String* message = String::Concat(S"Scanning Repository's Previous Revisions:\n",
											 S"     ", revisionFilename->GetAssetClassification(), S"\n",
											 S"     ", revisionFilename->GetAssetLabel());

			if (worker != NULL)
			{
				worker->ReportProgress((r * 100) / revisions->Count, message);
			}

			// Obtain a fully quantified assetFilename to the blessed asset
			MOG_Filename* blessedFilename = MOG_ControllerRepository::GetAssetBlessedVersionPath(revisionFilename, revisionFilename->GetVersionTimeStamp());

			// Open this inbox asset
			MOG_Properties* props = MOG_Properties::OpenAssetProperties(blessedFilename);
			if (props)
			{
				// Set us into immediate mode because it is faster and safer when we are only setting a single property in a blessed revision
				props->SetImmeadiateMode(true);
				// Set the BlessedTime of this revision to match it's revision
				props->BlessedTime = blessedFilename->GetVersionTimeStamp();
				// Make sure we close this asset
				props->Close();
			}
		}

		if (worker != NULL)
		{
			// Check for user cancel
			if (worker->CancellationPending)
			{
				break;
			}
		}
	}

	// Scan the inboxes for assets that need to be updated
	ArrayList* inboxAssets = MOG_DBInboxAPI::InboxGetAssetList(S"", S"", S"", S"");

	for (int r = 0; r < inboxAssets->Count; r++)
	{
		MOG_Filename* inboxFilename = dynamic_cast<MOG_Filename*>(inboxAssets->Item[r]);
		if (inboxFilename)
		{
			String* message = String::Concat(S"Scanning User Inboxes:\n",
											 S"     ", inboxFilename->GetRelativePathWithinInbox(), S"\n",
											 S"     ", inboxFilename->GetAssetClassification(), S"\n",
											 S"     ", inboxFilename->GetAssetLabel());

			if (worker != NULL)
			{
				worker->ReportProgress((r * 100) / inboxAssets->Count, message);
			}
		}

		// Only bother to fixup assets in the user's drafts and inbox
		if (inboxFilename->IsDrafts() ||
			inboxFilename->IsInbox())
		{
			// Open this inbox asset
			MOG_ControllerAsset* asset = MOG_ControllerAsset::OpenAsset(inboxFilename);
			if (asset)
			{
				// Update this asset's properties for the new Out-of-date check
				if (asset->GetProperties()->PreviousRevision->Length == 0)
				{
					// Update this inbox asset to the current version of this asset what ever it may be
					asset->GetProperties()->PreviousRevision = MOG_ControllerProject::GetAssetCurrentVersionTimeStamp(inboxFilename);
				}

				// Make sure we close this asset
				asset->Close();
			}
		}

		if (worker != NULL)
		{
			// Check for user cancel
			if (worker->CancellationPending)
			{
				break;
			}
		}
	}
}







//--------------------------------------------------------------------------------
// Old System Utilities
//--------------------------------------------------------------------------------
ArrayList *MOG_SystemUtilities::CheckBlessedAssets_VerifyPackageAssetIntegrity(String *packageName)
{
	ArrayList *badAssets = new ArrayList();

	return badAssets;
}

ArrayList *MOG_SystemUtilities::CheckBlessedAssets_VerifyPackageIntegrity(String *packageName)
{
	ArrayList *badPackages = new ArrayList();

	return badPackages;
}


bool MOG_SystemUtilities::CheckUserOutBox_FindStaleAssets(String *projectPath, String *userName, String *assetName)
{
	return false;
}


ArrayList *MOG_SystemUtilities::BuildBlessedAssetsList(String *blessedPath)
{
	ArrayList *assets = new ArrayList();

	return assets;
}


ArrayList *MOG_SystemUtilities::RepairAssetDirectories(ArrayList *assets, bool bAutoRepair)
{
	ArrayList *reportStrings = new ArrayList();

	return reportStrings;
}


bool MOG_SystemUtilities::CheckCurrentBlessedAssets_FindMissingVersions(String *projectPath, String *assetName)
{
	return false;
}


bool MOG_SystemUtilities::SyncProjectFrom(String *sourceMogPath, String *sourceProjectPath, String *branchName, String *assetPattern)
{
	return false;
}


bool MOG_SystemUtilities::RebuildPackageDatabaseLists(String *projectName, String *branchName)
{
	return false;
}


ArrayList *MOG_SystemUtilities::ScanDataBaseForOutOfSyncAssets(String *branchName)
{
	ArrayList *badAssets = new ArrayList();

	return badAssets;
}

ArrayList *MOG_SystemUtilities::CheckAssets_FindCollidingAssets(ArrayList *assets)
{
	MOG_ASSERT_THROW(assets, MOG_Exception::MOG_EXCEPTION_InvalidData, "MOG_SystemUtilities::CheckAssets_FindCollidingAssets - Specified 'assets' Invalid");

	ArrayList *badAssets = new ArrayList();

	return badAssets;
}


ArrayList *MOG_SystemUtilities::CheckAssets_BuildPlatformSyncTargetFileList(ArrayList *assets, String *platform)
{
	MOG_ASSERT_THROW(assets, MOG_Exception::MOG_EXCEPTION_InvalidData, "MOG_SystemUtilities::CheckAssets_FindCollidingAssets - Specified 'assets' Invalid");

	ArrayList *platformSyncTargetFiles = new ArrayList();

	return platformSyncTargetFiles;
}


bool MOG_SystemUtilities::TestLocks(String *lockName)
{
	// Obtain numerous iterations of this lock
	for (int l = 0; l < 10; l++)
	{
		// Build a newLockName using this iteration
		String *newLockName = String::Concat(lockName, S".", l.ToString());
		// Get the top level lock
		MOG_Command *command = MOG_CommandFactory::Setup_LockWriteRequest(newLockName, "TestLock");
		if (MOG_ControllerSystem::GetCommandManager()->CommandProcess(command))
		{
			// Make sure we haven't exceeded our string length too far?
			if (newLockName->Length < 50)
			{
				TestLocks(newLockName);
			}

			// Release the lock
			command = MOG_CommandFactory::Setup_LockWriteRelease(newLockName);
			MOG_ControllerSystem::GetCommandManager()->CommandProcess(command);
		}
	}

	return true;
}


bool MOG_SystemUtilities::PopulateDataBaseTables(String *projectName, String *branchName)
{
	bool failed = false;

	return failed;
}

bool MOG_SystemUtilities::DatabaseVersion6_UpdatePropertiesIniFile(MOG_Filename *assetToUpdate)
{
	//builid holder variables for the given property to update.
	MOG_Property *oldProperty;
	MOG_Property *newProperty;

	//get the iniFile 
	MOG_PropertiesIni *iniFile = new MOG_PropertiesIni(String::Concat(assetToUpdate->GetEncodedFilename(), S"\\Properties.Info"));

	///////////////////////////////////////////////////////////
	//Rename {AssetInfo} to  {Asset Stats} 
	///////////////////////////////////////////////////////////
	
	//if this property is set
	if(iniFile->PropertyExist(S"PROPERTIES",S"AssetInfo", S"Creator") )
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"AssetInfo", S"Creator");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Asset Stats", S"Creator", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}

	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"AssetInfo", S"Owner") )
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"AssetInfo", S"Owner");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Asset Stats", S"Owner", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}

	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"AssetInfo", S"SourceMachine") )
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"AssetInfo", S"SourceMachine");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Asset Stats", S"SourceMachine", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}

	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"AssetInfo", S"SourcePath") )
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"AssetInfo", S"SourcePath");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Asset Stats", S"SourcePath", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}


	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"AssetInfo", S"CreatedTime") )
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"AssetInfo", S"CreatedTime");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Asset Stats", S"CreatedTime", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}


	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"AssetInfo", S"ModifiedTime") )
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"AssetInfo", S"ModifiedTime");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Asset Stats", S"ModifiedTime", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}


	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"AssetInfo", S"Status") )
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"AssetInfo", S"Status");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Asset Stats", S"Status", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}


	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"AssetInfo", S"Size") )
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"AssetInfo", S"Size");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Asset Stats", S"Size", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}

	////////////////////////////////////////////////////////////////////
	//Rename {MOGProperty} to {Asset Info}
	/////////////////////////////////////////////////////////////////////


	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"MOGProperty", S"Description"))
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"MOGProperty", S"Description");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Asset Info", S"Description", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}

	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"MOGProperty", S"AssetIcon") )
	{
		
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"MOGProperty", S"AssetIcon");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Asset Info", S"AssetIcon", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}

	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"MOGProperty", S"AssetViewer") )
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"MOGProperty", S"AssetViewer");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Asset Info", S"AssetViewer", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}


	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"MOGProperty", S"PropertyMenu") )
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"MOGProperty", S"PropertyMenu");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Asset Info", S"PropertyMenu", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}


	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"MOGProperty", S"PackagedAsset") )
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"MOGProperty", S"PackagedAsset");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Asset Info", S"IsPackagedAsset", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}

	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"MOGProperty", S"Package") )
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"MOGProperty", S"Package");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Asset Info", S"IsPackage", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}

	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"MOGProperty", S"Build") )
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"MOGProperty", S"Build");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Asset Info", S"IsBuild", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}

	///////////////////////////////////////////////////////////////////
	// Rename {MOGProperty} to {Asset Options}
	///////////////////////////////////////////////////////////////////

	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"MOGProperty", S"SyncTargetPath") )
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"MOGProperty", S"SyncTargetPath");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Asset Options", S"SyncTargetPath", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}


	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"MOGProperty", S"ValidSlaves") )
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"MOGProperty", S"ValidSlaves");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Asset Options", S"ValidSlaves", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}


	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"MOGProperty", S"ShowRipCommandWindow") )
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"MOGProperty", S"ShowRipCommandWindow");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Asset Options", S"ShowRipCommandWindow", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}


	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"MOGProperty", S"MaintainLock") )
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"MOGProperty", S"MaintainLock");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Asset Options", S"MaintainLock", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}


	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"MOGProperty", S"UnReferencedRevisionHistory") )
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"MOGProperty", S"UnReferencedRevisionHistory");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Asset Options", S"UnReferencedRevisionHistory", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}

	///////////////////////////////////////////////////////////////////////////
	//Changed {MOGProperty} to {Classificaion Info}
	////////////////////////////////////////////////////////////////////////////


	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"MOGProperty", S"Library") )
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"MOGProperty", S"Library");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Classification Info", S"IsLibrary", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}
	

	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"MOGProperty", S"ClassIcon") )
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"MOGProperty", S"ClassIcon");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Classification Info", S"ClassIcon", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}
	

	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"MOGProperty", S"Classification") )
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"MOGProperty", S"Classification");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Classification Info", S"Classification", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}

	////////////////////////////////////////////////////////////////////////////
	//Change {MOGProperty} to {Sync Options}
	////////////////////////////////////////////////////////////////////////////

	//	//get the property from the ini if possible
	//oldProperty = iniFile->GetProperty(S"Properties",S"MOGProperty", S"SyncRootNodeLabel");
	////if this property is set
	//if(oldProperty->mValue && oldProperty->mValue->Length)
	//{
	//	//create the property to add
	//	newProperty = new MOG_Property(S"Properties", S"Sync Options", S"SyncRootNodeLabel", oldProperty->mValue);
	//	//update the value as appropriate
	//	iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	//}
	

	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"MOGProperty", S"SyncRootNode"))
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"MOGProperty", S"SyncRootNode");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Sync Options", S"SyncLabel", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}

	//////////////////////////////////////////////////////////////////////////////////
	//Changed {MOGProperty} to {Rip Options}
	/////////////////////////////////////////////////////////////////////////////////


	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"MOGProperty", S"AssetRipTasker") )
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"MOGProperty", S"AssetRipTasker");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Rip Options", S"AssetRipTasker", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}


	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"MOGProperty", S"AssetRipper") )
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"MOGProperty", S"AssetRipper");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Rip Options", S"AssetRipper", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}


	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"MOGProperty", S"DivergentPlatformDataType") )
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"MOGProperty", S"DivergentPlatformDataType");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Rip Options", S"DivergentPlatformDataType", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}

	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"MOGProperty", S"UseTempRipDir") )
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"MOGProperty", S"UseTempRipDir");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Rip Options", S"UseTempRipDir", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}


	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"MOGProperty", S"UseLocalTempRipDir"))
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"MOGProperty", S"UseLocalTempRipDir");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Rip Options", S"UseLocalTempRipDir", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}

	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"MOGProperty", S"CopyFilesIntoTempRipDir") )
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"MOGProperty", S"CopyFilesIntoTempRipDir");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Rip Options", S"CopyFilesIntoTempRipDir", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}

	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"MOGProperty", S"AutoDetectRippedFiles") )
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"MOGProperty", S"AutoDetectRippedFiles");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Rip Options", S"AutoDetectRippedFiles", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}

	///////////////////////////////////////////////////////////////////////////
	//Rename {MOGProperty} to {Package Options}
	//////////////////////////////////////////////////////////////////////////


	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"MOGProperty", S"PackageStyle") )
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"MOGProperty", S"PackageStyle");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Package Options", S"PackageStyle", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}


	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"MOGProperty", S"PackageWorkingDirectory") )
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"MOGProperty", S"PackageWorkingDirectory");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Package Options", S"PackageWorkingDirectory", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}


	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"MOGProperty", S"CleanupPackageWorkingDirectory"))
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"MOGProperty", S"CleanupPackageWorkingDirectory");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Package Options", S"CleanupPackageWorkingDirectory", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}

	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"MOGProperty", S"AutoPackage") )
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"MOGProperty", S"AutoPackage");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Package Options", S"AutoPackage", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}

	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"MOGProperty", S"ClusterPackaging") )
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"MOGProperty", S"ClusterPackaging");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Package Options", S"ClusterPackaging", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}

	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"MOGProperty", S"PackagePreMergeEvent") )
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"MOGProperty", S"PackagePreMergeEvent");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Package Options", S"PackagePreMergeEvent", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}


	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"MOGProperty", S"PackagePostMergeEvent") )
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"MOGProperty", S"PackagePostMergeEvent");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Package Options", S"PackagePostMergeEvent", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}

	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"MOGProperty", S"InputPackageTaskFile"))
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"MOGProperty", S"InputPackageTaskFile");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Package Options", S"InputPackageTaskFile", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}

	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"MOGProperty", S"OutputPackageTaskFile") )
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"MOGProperty", S"OutputPackageTaskFile");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Package Options", S"OutputPackageTaskFile", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}

	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"MOGProperty", S"PackageTool") )
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"MOGProperty", S"PackageTool");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Package Options", S"TaskFileTool", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}


	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"MOGProperty", S"PackageCommand_DeletePackage"))
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"MOGProperty", S"PackageCommand_DeletePackage");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Package Options", S"PackageCommand_DeletePackageFile", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}

	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"MOGProperty", S"PackageCommand_ResolveLateResolvers") )
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"MOGProperty", S"PackageCommand_ResolveLateResolvers");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Package Options", S"PackageCommand_ResolveLateResolvers", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}
	
	///////////////////////////////////////////////////////////////////////////////////////
	//Changed {MOGProperty} to {Packaging Commands}
	//////////////////////////////////////////////////////////////////////////////////////


	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"MOGProperty", S"PackageCommand_AddAsset") )
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"MOGProperty", S"PackageCommand_AddAsset");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Packaging Commands", S"PackageCommand_AddFile", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}

	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"MOGProperty", S"PackageCommand_RemoveAsset") )
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"MOGProperty", S"PackageCommand_RemoveAsset");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Packaging Commands", S"PackageCommand_RemoveFile", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}

	////////////////////////////////////////////////////////////////////////////////////////////
	//Change {MOGProperty} to {Build Options}
	///////////////////////////////////////////////////////////////////////////////////////////


	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"MOGProperty", S"BuildTool"))
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"MOGProperty", S"BuildTool");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Build Options", S"BuildTool", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}

	//if this property is set
	if(iniFile->PropertyExist(S"Properties",S"MOGProperty", S"BuildWorkingDirectory") )
	{
		//get the property from the ini if possible
		oldProperty = iniFile->GetProperty(S"Properties",S"MOGProperty", S"BuildWorkingDirectory");
		//create the property to add
		newProperty = new MOG_Property(S"Properties", S"Build Options", S"BuildWorkingDirectory", oldProperty->mValue);
		//update the value as appropriate
		iniFile->RenameKeyInPropertyINI(oldProperty, newProperty);
	}

	//////////////////////////////////////////////////////////
	//Remove Depricated properties
	////////////////////////////////////////////////////////

	//Remove the GameDataAsset property
	if(iniFile->PropertyExist(S"Properties", S"MOGProperty", S"GameDataAsset"))
	{
		iniFile->RemovePropertyString(S"Properties",S"MOGProperty", S"GameDataAsset");
	}

	//Remove the NativeDataType property
	if(iniFile->PropertyExist(S"Properties", S"MOGProperty", S"NativeDataType"))
	{
		iniFile->RemovePropertyString(S"Properties",S"MOGProperty", S"NativeDataType");
	}

	iniFile->Close();

	return true;
}


