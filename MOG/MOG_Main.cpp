//--------------------------------------------------------------------------------
//	MOG.cpp
//	
//	
//--------------------------------------------------------------------------------

#include "stdafx.h"

#include "MOG_ControllerSystem.h"
#include "MOG_DosUtils.h"
#include "MOG_Tokens.h"

#include "MOG_SystemUtilities.h"
#include "MOG_DosUtils.h"


using namespace MOG_CoreControls;


MOG_Main::MOG_Main(void)
{
	mShutdown = false;
}


MOG_Main::~MOG_Main(void)
{
}


bool MOG_Main::Init_Client(String *configFilename, String *name)
{
	mName = name;
	mProcessMutex_Locked = false;
	mShutdownInProgress = false;
	mShutdown = false;

	// Make sure we have a valid config filename?
	if (!configFilename || !configFilename->Length)
	{
		configFilename = BuildDefaultConfigFile();
	}

	// Check our bootup precautions
	if (CheckInitPrecautions(configFilename))
	{
		// Initialize MOG_REPORT for the server
// MOG is really stable now so I am shutting off the log files so things will run faster
//		MOG_Prompt::SetMode(MOG_PROMPT_MODE_TYPE::MOG_PROMPT_FILE);
		MOG_Report::SetLogFileName(String::Concat(Application::StartupPath, S"\\Logs\\Clients\\", Environment::MachineName, S".log"));

		// Create a new system
		return MOG_ControllerSystem::InitializeClient(configFilename);
	}

	return false;
}


bool MOG_Main::Init_Slave(String *configFilename, String *name)
{
	mName = name;
	mProcessMutex_Locked = false;
	mShutdownInProgress = false;
	mShutdown = false;

	// Make sure we have a valid config filename?
	if (!configFilename || !configFilename->Length)
	{
		configFilename = BuildDefaultConfigFile();
	}

	// Check our bootup precautions
	if (CheckInitPrecautions(configFilename))
	{
		// Setup initial error reporting options
// MOG is really stable now so I am shutting off the log files so things will run faster
//		MOG_Prompt::SetMode(MOG_PROMPT_MODE_TYPE::MOG_PROMPT_FILE);
		MOG_Prompt::SetMode(MOG_PROMPT_MODE_TYPE::MOG_PROMPT_SILENT);
		MOG_Report::SetLogFileName(String::Concat(Application::StartupPath, S"\\Logs\\Slaves\\", Environment::MachineName, S".log"));

		// Create a new system
		return MOG_ControllerSystem::InitializeSlave(configFilename);
	}

	return false;
}


bool MOG_Main::Init_Editor(String *configFilename, String *name)
{
	mName = name;
	mProcessMutex_Locked = false;
	mShutdownInProgress = false;
	mShutdown = false;

	// Make sure we have a valid config filename?
	if (!configFilename || !configFilename->Length)
	{
		configFilename = BuildDefaultConfigFile();
	}

	// Check our bootup precautions
	if (CheckInitPrecautions(configFilename))
	{
		// Setup initial error reporting options
// MOG is really stable now so I am shutting off the log files so things will run faster
//		MOG_Prompt::SetMode(MOG_PROMPT_MODE_TYPE::MOG_PROMPT_FILE);
//		MOG_Prompt::SetMode(MOG_PROMPT_MODE_TYPE::MOG_PROMPT_SILENT);
		MOG_Report::SetLogFileName(String::Concat(Application::StartupPath,S"\\Logs\\Editors\\", Environment::MachineName, S".log"));

		// Create a new system
		if (MOG_ControllerSystem::InitializeEditor(configFilename))
		{
			return true;
		}
	}

	return false;
}


bool MOG_Main::Init_CommandLine(String *configFilename, String *name)
{
	mName = name;
	mProcessMutex_Locked = false;
	mShutdownInProgress = false;
	mShutdown = false;

	// Make sure we have a valid config filename?
	if (!configFilename || !configFilename->Length)
	{
		configFilename = BuildDefaultConfigFile();
	}

	// Check our bootup precautions
	if (CheckInitPrecautions(configFilename))
	{
		// Setup initial error reporting options
// MOG is really stable now so I am shutting off the log files so things will run faster
//		MOG_Prompt::SetMode(MOG_PROMPT_MODE_TYPE::MOG_PROMPT_FILE);
		MOG_Prompt::SetMode(MOG_PROMPT_MODE_TYPE::MOG_PROMPT_CONSOLE);
		MOG_Prompt::SetMode(MOG_PROMPT_MODE_TYPE::MOG_PROMPT_SILENT);
		MOG_Report::SetLogFileName(String::Concat(Application::StartupPath, S"\\Logs\\CommandLines\\", Environment::MachineName, S".log"));

		// Create a new system
		return MOG_ControllerSystem::InitializeCommandLine(configFilename);
	}

	return false;
}


bool MOG_Main::CheckInitPrecautions(String *configFilename)
{
	// Check if we are missing a configFilename?
	MOG_ASSERT_THROW(configFilename, MOG_Exception::MOG_EXCEPTION_SystemInitFailed, S"No system config file specified.");
	MOG_ASSERT_THROW(configFilename->Length, MOG_Exception::MOG_EXCEPTION_SystemInitFailed, S"No system config file specified.");
	// Missing configFilename's path?
	MOG_ASSERT_THROW(DosUtils::FileExistFast(configFilename), MOG_Exception::MOG_EXCEPTION_SystemInitFailed, S"Can't locate MOG Repository.  Check network drive mapping.");

	return true;
}


String *MOG_Main::GetExecutablePath()
{
	return Application::StartupPath;
}

String *MOG_Main::GetExecutablePath_StripCurrentDirectory()
{
	String* path = GetExecutablePath();
	String* testString = S"\\Current";

	if (path->ToLower()->EndsWith(testString->ToLower()))
	{
		path = path->Substring(0, path->Length - testString->Length);
	}

	return path;
}


String *MOG_Main::GetAssociatedWorkspacePath()
{
	String *path = "";

	// Check if the user has specified a specific associated workspace path?
	if (mAssociatedWorkspacePath->Length > 0)
	{
		path = mAssociatedWorkspacePath;
	}
	else
	{
		// Assume the path of the executable
		path = Application::StartupPath;
	}

	return path;
}


String *MOG_Main::GetDefaultRepositoryPath(String *configFile)
{
	String *repositoryPath = S"";

	if (!configFile || !configFile->Length)
	{
		configFile = FindLocalConfigFile();
	}

	if (configFile->Length)
	{
		if (DosUtils::FileExistFast(configFile))
		{
			MOG_Ini *defaultSettings = new MOG_Ini(configFile);

			if (defaultSettings->SectionExist(S"MOG") && defaultSettings->KeyExist(S"MOG", S"SystemRepositoryPath"))
			{
				repositoryPath = defaultSettings->GetString(S"MOG", S"SystemRepositoryPath");
				repositoryPath = repositoryPath->TrimEnd(S"\\"->ToCharArray());
			}
		}
	}

	return repositoryPath;
}

String *MOG_Main::GetDefaultSystemConfigFile(String *configFile)
{
	String *systemRepository = "";
	String *systemConfigFile = "";

	if (!configFile || !configFile->Length)
	{
		configFile = FindLocalConfigFile();
	}

	if (configFile->Length)
	{
		if (DosUtils::FileExistFast(configFile))
		{
			MOG_Ini *defaultSettings = new MOG_Ini(configFile);

			if (defaultSettings->SectionExist("MOG") && defaultSettings->KeyExist("MOG", "SystemConfiguration"))
			{
				systemRepository = defaultSettings->GetString("MOG", "SystemRepositoryPath");
				systemConfigFile = defaultSettings->GetString("MOG", "SystemConfiguration");

				systemRepository = systemRepository->TrimEnd(S"\\"->ToCharArray());

				systemConfigFile = systemConfigFile->Replace(S"{SystemRepositoryPath}", systemRepository);
			}
		}
	}

	return systemConfigFile;
}


String *MOG_Main::FindInstalledConfigFile()
{
	String* path = DosUtils::GetSystemEnvironmentVariable(S"MOG_PATH");
	if (path && path->Length)
	{
		return String::Concat(path, S"\\MOG.ini");
	}
	else
	{
// Removed because the ServerLoader is using the MOG.dll and it shouldn't!
// We want this warning, but just can't do it now.
//		MOG_Report::ReportMessage(	S"MOG_PATH not defined",
//									String::Concat(	S"MOG cannot find the config file, 'MOG.ini', because the MOG_PATH environment variable is not defined.\n",
//													S"MOG_PATH should point to the location at which you installed MOG",
//													S"If you have recently installed MOG, this may be resolved with a reboot."),
//									Environment::StackTrace,
//									MOG_ALERT_LEVEL::ERROR);
//
		return S"";
	}
}

String *MOG_Main::FindLocalConfigFile()
{
	String* configFile = String::Concat(GetExecutablePath(), S"\\", S"MOG.ini");
	if (!DosUtils::FileExistFast(configFile))
	{
		configFile = FindInstalledConfigFile();
	}

	return configFile;
}

String *MOG_Main::BuildDefaultConfigFile()
{
	String* repositoryPath = GetDefaultRepositoryPath(S"");
	String* configFile = GetDefaultSystemConfigFile(S"");

	if (repositoryPath->Length)
	{
		if (configFile->Length)
		{
			configFile = MOG_Tokens::GetFormattedString(configFile, String::Concat(S"{SystemRepositoryPath}=", repositoryPath));
		}
		else
		{
			configFile = String::Concat(repositoryPath, S"\\", MOG_SYSTEM_CONFIGPATH, S"\\", MOG_SYSTEM_CONFIGFILENAME);
		}
	}
	
	return configFile;
}


bool MOG_Main::Process(void)
{
	// We should always do our finally clause...
	bool doFinally = true;

	// Make sure we are mutex safe!
	if (!IsProcessMutexLocked())
	{
		try
		{
			// Force us into a mutex state
			LockProcessMutexFlag(true);

			// Tick the system
			MOG_ControllerSystem::Process();

			// Make sure to recover from our process mutex
			LockProcessMutexFlag(false);
		}
		catch(ThreadAbortException *e)
		{
			// Eat a ThreadAbortException (means we were Shutdown intentionally)
			e->ToString();
			// Do not do our finally clause
			doFinally = false;
			return false;
		}
		catch(Exception *e)
		{
			// Make sure to inform the Server about this exception
			MOG_Report::ReportMessage(S"Process", e->Message, e->StackTrace, MOG_ALERT_LEVEL::CRITICAL);
			return false;
		}
		catch(...)
		{
			// Make sure to inform the Server about this exception
			MOG_Report::ReportMessage(S"Process", S"MogProcess: Exception Error durring Mog.Process - Unspecified", Environment::StackTrace, MOG_ALERT_LEVEL::CRITICAL);
			return false;
		}
		__finally
		{
			if(doFinally)
			{
				// Make sure to recover from our process mutex
				LockProcessMutexFlag(false);
			}
		}
	}

	return true;
}


void MOG_Main::Shutdown()
{
	if (!mShutdown && !mShutdownInProgress)
	{
		mShutdownInProgress = true;

		MOG_ControllerSystem::Shutdown();

		mShutdown = true;
	}
}

void MOG_Main::NotifyUserOfPendingShutdown(String *message)
{
	ProgressDialog* progress = new ProgressDialog(S"MOG is being shutting down", message, new DoWorkEventHandler(NULL, &NotifyUserOfPendingShutdown_Worker), NULL, false);
	progress->ShowDialog();
}

void MOG_Main::NotifyUserOfPendingShutdown_Worker(Object* sender, DoWorkEventArgs* e)
{
	BackgroundWorker* worker = dynamic_cast<BackgroundWorker*>(sender);

	// Wait 60 seconds and record the time we want to end at
	DateTime endTime = DateTime::Now.AddSeconds(60);

	// While we still have a sleepCount, show our ProgressBar
	while (endTime > DateTime::Now && !worker->CancellationPending)
	{
		// Sleep 10th second
		Thread::Sleep(100);
	}
}

bool MOG_Main::IsLicensed()
{
// JohnRen - Allow the downloadable non-licensed version to be full featured
//	String *noLicenseFile = "|NoLicenseFile|";
//
//	// Check if there was no license file?
//	if (mDisabledFeatureList->ToLower()->IndexOf(noLicenseFile->ToLower()) != -1)
//	{
//		return false;
//	}

	return true;
}


bool MOG_Main::IsFeatureDisabled(String *feature)
{
	// Check if everything is disabled?
	if (IsUnlicensed())
	{
		return true;
	}

	// Trim any delimiters from the specified feature string because we will just add them back below
	feature = feature->Trim(S"|"->ToCharArray());

	// Check if this specific feature is listed?
	if (mDisabledFeatureList->ToLower()->IndexOf(String::Concat(S"|", feature, S"|")->ToLower()))
	{
		return true;
	}

	return false;
}


