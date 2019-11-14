//--------------------------------------------------------------------------------
//	MOG_REPORT.cpp
//--------------------------------------------------------------------------------

#include "stdafx.h"

#include "MOG_Define.h"
#include "MOG_Report.h"
#include "MOG_Time.h"
#include "MOG_DosUtils.h"
#include "MOG_Tokens.h"
#include "MOG_Prompt.h"
#include "MOG_ControllerSystem.h"

using System::IO::FileAccess;
using System::IO::FileMode;
using System::Environment;
using namespace System::Threading;
using namespace System;


// ****************** Error Dialogs ***********************
bool MOG_Report::ReportMessage( String *title, String *message, String* stackTrace, MOG_ALERT_LEVEL alertLevel)
{
	bool rc = false;

	// Tokenize the message string to obscure repository path information
	message = MOG_ControllerSystem::TokenizeMessageString(message);

	System::Diagnostics::Debug::WriteLine( String::Concat( message, S"\n", stackTrace ) );

	bool bNotifyUser = true;

	// Make sure we aren't running silent?
	if (MOG_Prompt::IsMode(MOG_PROMPT_CONSOLE))
	{
		Console::WriteLine(String::Concat(S"** ", title, S" **\n   - ", message));
	}
	else if (!MOG_Prompt::IsMode(MOG_PROMPT_SILENT))
	{
		bool bPrompted = MOG_Prompt::PromptMessage(title, message, stackTrace, alertLevel);
		bNotifyUser = (bPrompted) ? false : true;
	}

	if (MOG_ControllerSystem::GetDB())
	{
		switch(alertLevel)
		{
			case MOG_ALERT_LEVEL::ALERT:
				rc = MOG_ControllerSystem::NotifySystemAlert(title, message, stackTrace, bNotifyUser);
				break;
			case MOG_ALERT_LEVEL::ERROR:
				rc = MOG_ControllerSystem::NotifySystemError(title, message, stackTrace, bNotifyUser);
				break;
			case MOG_ALERT_LEVEL::CRITICAL:
				rc = MOG_ControllerSystem::NotifySystemException(title, message, stackTrace, bNotifyUser);
				break;
		}
	}

	return rc;
}

MOGPromptResult MOG_Report::ReportResponse( String *title, String *message, String* stackTrace, MOGPromptButtons buttons, MOG_ALERT_LEVEL alertLevel)
{
	if (!stackTrace)
	{
		stackTrace = S"";
	}
	
	// Tokenize the message string to obscure repository path information
	message = MOG_ControllerSystem::TokenizeMessageString(message);

	System::Diagnostics::Debug::WriteLine( String::Concat( message, S"\n", stackTrace ) );

	switch(alertLevel)
	{
		case MOG_ALERT_LEVEL::ALERT:
			MOG_ControllerSystem::NotifySystemAlert(title, message, stackTrace, true);
			break;
		case MOG_ALERT_LEVEL::ERROR:
			MOG_ControllerSystem::NotifySystemError(title, message, stackTrace, true);
			break;
		case MOG_ALERT_LEVEL::CRITICAL:
			MOG_ControllerSystem::NotifySystemException(title, message, stackTrace, true);
			break;
	}

	// Make sure we aren't running silent?
	if (!MOG_Prompt::IsMode(MOG_PROMPT_SILENT))
	{
		return MOG_Prompt::PromptResponse(title, message, stackTrace, buttons, alertLevel);
	}

	return MOGPromptResult::OK;
}


// ****************** Error (SILENT) Dialogs ***********************
bool MOG_Report::ReportSilent(String *title, String *message, String *stackTrace)
{
	return ReportSilent(title, message, stackTrace, MOG_ALERT_LEVEL::ERROR);
}

bool MOG_Report::ReportSilent(String *title, String *message, String *stackTrace, MOG_ALERT_LEVEL alertLevel)
{
	if (!stackTrace)
	{
		stackTrace = S"";
	}
	
	// Tokenize the message string to obscure repository path information
	message = MOG_ControllerSystem::TokenizeMessageString(message);

	System::Diagnostics::Debug::WriteLine( String::Concat( message, S"\n", stackTrace ) );

	if (MOG_Prompt::IsMode(MOG_PROMPT_FILE))
	{
		DosUtils::AppendTextToFile(mLogFilename, String::Concat(title, S" - ", message, Environment::NewLine));
	}

	switch(alertLevel)
	{
		case MOG_ALERT_LEVEL::ALERT:
			return MOG_ControllerSystem::NotifySystemAlert(title, message, stackTrace, false);
		case MOG_ALERT_LEVEL::ERROR:
			return MOG_ControllerSystem::NotifySystemError(title, message, stackTrace, false);
		case MOG_ALERT_LEVEL::CRITICAL:
			return MOG_ControllerSystem::NotifySystemException(title, message, stackTrace, false);			
	}

	return false;
}


// ****************** Misc methods ***********************
void MOG_Report::SetLogFileName(String *string)
{
	mLogFilename = string;

	// Check if we are dumping to a log file?
	if (MOG_Prompt::IsMode(MOG_PROMPT_FILE))
	{
		// Make sure the directory exists
		DosUtils::DirectoryCreate(DosUtils::PathGetDirectoryPath(mLogFilename), false);
	}
}

bool MOG_Report::TestOnCPlusSide()
{
	ReportMessage( S"MC++ Foo", S"MC++ test message", S"MC++ stacktrace for test", MOG_ALERT_LEVEL::MESSAGE );
	return true;
}

