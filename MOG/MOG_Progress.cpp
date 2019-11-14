#include "stdafx.h"
#include "MOG_ProgressDialog.h"
#include "MOG_Progress.h"
#include "MOG_ControllerSystem.h"

using namespace MOG::CONTROLLER::CONTROLLERSYSTEM;
//using namespace System::Threading;
//using namespace System::Windows::Forms::ComponentModel;
 
//
// CreateDialog:
// Create and add a new dialog to our dialogs array
//
int MOG_Progress::CreateDialog(String *title, String *message, int progressMin, int progressMax)
{
	if (mDialogs == NULL)
	{
		mDialogs = new ArrayList();
	}

	try
	{
		// Tokenize the message string to obscure repository path information
		message = MOG_ControllerSystem::TokenizeMessageString(message);

		MOG_ProgressDialog *progress = new MOG_ProgressDialog();
		if( ClientForm != NULL )//&& ClientForm->Enabled )
		{
			progress->StartPosition = FormStartPosition::Manual;
			progress->Location = MOG_Prompt::GetCenteredLocation( ClientForm, progress->Size );
		}

		if (MOG_Prompt::IsMode(MOG_PROMPT_CONSOLE))
		{
			Console::WriteLine(String::Concat(S"** ", title, S" **\n   - ", message));
		}
		else if (!MOG_Prompt::IsMode(MOG_PROMPT_SILENT))
		{
			progress->Show( title, message, progressMax );
			MOG_CoreControls::MogUtil_DepthManager::DepthManager::VirtualModal(ClientForm, progress, MOG_CoreControls::MogUtil_DepthManager::FormDepth::TOP);
		}

		mDialogs->Add(progress);

		progress->InitializingInt = mDialogs->Count;
		mVisible++;

		return mDialogs->Count;
	}
	// If we had any error, we need the server to know
	catch(Exception *e)
	{
		MOG_ControllerSystem::NotifySystemException( e->GetType()->ToString(), e->Message, e->StackTrace, true );
	}

	return 0;
}

//
// GetDialog:
// Return a dialog pointer to one of the dialogs in our dialogs array from a dialog id number
//
MOG_ProgressDialog *MOG_Progress::GetDialog(int dialogId, bool explicitMatch)
{
	try
	{
		if (mDialogs != NULL && mDialogs->Count >= dialogId && dialogId >= 0)
		{
			if (dialogId == 0 && mDialogs->Count == 1 && !explicitMatch)
			{
				return __try_cast<MOG_ProgressDialog*>(mDialogs->Item[0]);
			}
			else if (dialogId == 0 && !explicitMatch)
			{
				for (int i = 0; i < mDialogs->Count; i++)
				{
					if (mDialogs->Item[i] != NULL)
					{
						MOG_ProgressDialog *progress = __try_cast<MOG_ProgressDialog*>(mDialogs->Item[i]);
						if (progress->IsRunning)
						{
							return progress;
						}
					}
				}
			}			 
			else
			{
				if (dialogId >= 0 && mDialogs->Item[dialogId - 1] != NULL)
				{
					return __try_cast<MOG_ProgressDialog*>(mDialogs->Item[dialogId - 1]);
				}
			}
		}
	}
	catch(Exception *e)
	{
		MOG_ControllerSystem::NotifySystemException( "MOG_Progress::GetDialog", e->Message, e->StackTrace, true );
		return NULL;
	}

	return NULL;
}
void MOG_Progress::SecondaryProgressBarSetup(int dialogId)
{
	SecondaryProgressBarSetup(dialogId, 0, 100);
}
void MOG_Progress::SecondaryProgressBarSetup(int dialogId, int max)
{
	SecondaryProgressBarSetup(dialogId, 0, max);
}
void MOG_Progress::SecondaryProgressBarSetup(int dialogId, int min, int max)
{
	MOG_ProgressDialog *progress = GetDialog( dialogId );
	if (progress)
	{
		progress->SecondaryProgressBarSetup(min, max);
	}
}

void MOG_Progress::SecondaryProgressBarClose(int dialogId)
{
	MOG_ProgressDialog *progress = GetDialog( dialogId );
	if (progress)
	{
		progress->SecondaryProgressBarClose();
	}
}

void MOG_Progress::TerminateDialog(int dialogId)
{
	try
	{
		if (mDialogs && dialogId > 0 && mDialogs->Count >= dialogId)
		{
			// Get a handle to the desired dialog
			MOG_ProgressDialog *progress = GetDialog(dialogId);
			if (progress)
			{
				// Close the dialog
				progress->AbortThread();

				mDialogs->Item[dialogId - 1] = NULL;
				if (mVisible)
				{
					mVisible --;
				}
			}
		}
	}
	catch(Exception *e)
	{
		MOG_ControllerSystem::NotifySystemException( "MOG_Progress::TerminateDialog", e->Message, e->StackTrace, true );
	}

	return;
}

void MOG_Progress::ProgressInitGraphic(int dialogId)
{
	MOG_ProgressDialog *progress = GetDialog(dialogId);
	if (progress)
	{
		progress->SetupProgressGraphic();
	}
}

MOGPromptResult MOG_Progress::ProgressUpdateGraphic(int dialogId, String *fileMessage)
{
	MOG_ProgressDialog *progress = GetDialog(dialogId);
	if (progress)
	{
		Application::DoEvents();
		return progress->UpdateProgressGraphic(fileMessage);
	}

	return MOGPromptResult::None;
}

void MOG_Progress::ProgressSetMinMax(int dialogId, int min, int max)
{
	MOG_ProgressDialog *progress = GetDialog(dialogId);
	if (progress)
	{
		progress->SetupProgressBar(min, max);
	}
}
int MOG_Progress::ProgressSetup(String *title, String *message)
{
	return CreateDialog(title, message, 0, 0);	
}

int MOG_Progress::ProgressSetup(String *title, String *message, int progressMax)
{
	return CreateDialog(title, message, 0, progressMax);	
}

int MOG_Progress::ProgressSetup(String *title, String *message, int progressMin, int progressMax)
{
	return CreateDialog(title, message, progressMin, progressMax);	
}

void MOG_Progress::ProgressClose(int dialogId)
{
	try
	{
		TerminateDialog(dialogId);
	}
	catch( Exception *ex )
	{
		MOG_Report::ReportSilent( S"Possible Error Closing Progress Dialog",
			ex->Message, ex->StackTrace, MOG_ALERT_LEVEL::ERROR);
	}
}

MOGPromptResult MOG_Progress::ProgressStatus( int dialogID )
{
	MOG_ProgressDialog *progress = GetDialog( dialogID );
	if (progress)
	{
		return progress->MOGPromptResult;
	}

	return MOGPromptResult::None;
}

MOGPromptResult MOG_Progress::ProgressUpdate( int dialogID, String *fileMessage )
{
	return ProgressUpdate( dialogID, NULL, fileMessage );
}

MOGPromptResult MOG_Progress::ProgressUpdate(int dialogID, String *message, String *fileMessage)
{
	return ProgressUpdateStep( dialogID, message, fileMessage, 0 );
}

MOGPromptResult MOG_Progress::ProgressUpdatePercent( int dialogID, String *fileMessage, int percent )
{
	return ProgressUpdatePercent( dialogID, NULL, fileMessage, percent );
}

MOGPromptResult MOG_Progress::ProgressUpdatePercent( int dialogID, String *message, String *fileMessage, int percent )
{
	MOG_ProgressDialog *progress = GetDialog(dialogID);
	if (progress)
	{
		// Tokenize the message string to obscure repository path information
		message = MOG_ControllerSystem::TokenizeMessageString(message);
		fileMessage = MOG_ControllerSystem::TokenizeMessageString(fileMessage);

		progress->SetupProgressBar(0,100);
		progress->UpdateProgressValue(message, fileMessage, percent);			
		return progress->MOGPromptResult;
	}

	return MOGPromptResult::None;
}

MOGPromptResult MOG_Progress::ProgressUpdateStep( int dialogID, String *fileMessage )
{
	return ProgressUpdateStep( dialogID, NULL, fileMessage );
}

MOGPromptResult MOG_Progress::ProgressUpdateStep(int dialogID, String *message, String *fileMessage)
{
	return ProgressUpdateStep( dialogID, message, fileMessage, 1 );
}

MOGPromptResult MOG_Progress::ProgressUpdateStep( int dialogID, String *fileMessage, int step )
{
	return ProgressUpdateStep( dialogID, NULL, fileMessage, step );
}

MOGPromptResult MOG_Progress::ProgressUpdateStep(int dialogId, String *message, String *fileMessage, int step)
{
	MOG_ProgressDialog *progress = GetDialog(dialogId);
	if (progress)
	{
		// Tokenize the message string to obscure repository path information
		message = MOG_ControllerSystem::TokenizeMessageString(message);
		fileMessage = MOG_ControllerSystem::TokenizeMessageString(fileMessage);

		progress->UpdateProgressStep( message, fileMessage, step );
		return progress->MOGPromptResult;
	}

	return MOGPromptResult::None;
}
