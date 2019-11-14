#include "stdafx.h"
#include "MOG_PromptDialog.h"
#include "MOG_Prompt.h"
#include "MOG_ControllerSystem.h"

using namespace MOG::CONTROLLER::CONTROLLERSYSTEM;
using namespace System::Threading;
using namespace System::Windows::Forms::ComponentModel;

// Overload of PromptMessage(String *title, String *message, String *stackTrace, MOG_ALERT_LEVEL alertLevel)
bool MOG_Prompt::PromptMessage(String *title, String *message)
{
	return PromptMessage( title, message, "", MOG_ALERT_LEVEL::MESSAGE); 
}
// Overload of PromptMessage(String *title, String *message, String *stackTrace, MOG_ALERT_LEVEL alertLevel)
bool MOG_Prompt::PromptMessage(String *title, String *message, String *stackTrace)
{
	return PromptMessage( title, message, stackTrace, MOG_ALERT_LEVEL::ALERT );
}

// glk: All methods overload this one static method so we can have only one place to look for error(s)
bool MOG_Prompt::PromptMessage(String *title, String *message, String *stackTrace, MOG_ALERT_LEVEL alertLevel)
{
	if (IsMode(MOG_PROMPT_CONSOLE))
	{
		Console::WriteLine(String::Concat(S"\n**** ", title, S" ****\n** ", message, S"\n"));
		return true;
	}
	else if (!IsMode(MOG_PROMPT_SILENT))
	{
		if (ClientForm && ClientForm->IsHandleCreated)
		{
			try
			{
				// If we do not already have a Prompt up...
				if ( CheckPromptMessageDialog() )
				{
					StartPromptMessageDialogThread( title, message, stackTrace, alertLevel );

					PromptMessageFormStoreAppend(title, message, stackTrace, alertLevel);
				}
				// Else, we do have a prompt, get it and use it
				else
				{
					MOG_PromptDialog *prompt = PromptMessageDialog;

					PromptMessageFormStoreAppend(title, message, stackTrace, alertLevel);

					// If we have an invalid prompt, close it and start a new one...
					if( prompt->IsDisposed)
					{
						prompt->Close();
						StartPromptMessageDialogThread( title, message, stackTrace, alertLevel );
					}
				}

				return true;
			}
			// If we had any error, we need the server to know
			catch(Exception *e)
			{
				MOG_ControllerSystem::NotifySystemException( e->GetType()->ToString(), e->Message, e->StackTrace, true );
			}
		}
		else
		{
			// Looks like things haven't started up yet but we still need to display this message to the user!
			MessageBox::Show(message, title);
			return true;
		}
	}

	return false;
}

// PROMPT MESSAGE THREADING METHODS
// Outside of these methods, there should be no other method where we access variables of MOG_PromptDialog directly (EVER!!).

// Start up our MOG_PromptDialog and generate a new Thread for it to update within
void MOG_Prompt::StartPromptMessageDialogThread( String *title, String *message, String *stackTrace, MOG_ALERT_LEVEL alertLevel )
{
	// Global variable to keep track of which Message (in our ListView) we are appending
	mCurrentListViewIndex = 0;
	try
	{
		if( PromptMessageDialog )
		{
			PromptMessageDialog->Close();
		}

		// Tokenize the message string to obscure repository path information
		message = MOG_ControllerSystem::TokenizeMessageString(message);

		MOG_PromptDialog *prompt = new MOG_PromptDialog();
		if( ClientForm != NULL )//&& ClientForm->Enabled )
		{
			prompt->StartPosition = FormStartPosition::Manual;
			prompt->Location = MOG_Prompt::GetCenteredLocation( ClientForm, prompt->Size );
		}

		prompt->SetupTitle(title);
		prompt->SetupMessage(message);
		prompt->SetupStackTrace(stackTrace);
		prompt->SetupImage(alertLevel, true);

		PromptMessageDialog = prompt;

		Thread *PromptMessageThread = new Thread( new ThreadStart( 0, &MOG_Prompt::PromptMessageThreadMethod ) );
		PromptMessageThread->ApartmentState = ApartmentState::STA;  //Make sure we are an STA Thread
		PromptMessageThread->Name = String::Concat(S"PromptMessageDialogThread", __box(threadNumber) );
		PromptMessageThread->Start();
		++threadNumber;
		// Allow our Prompt Message to start displaying
		Thread::Sleep( 10 );
	}
	catch( Exception *ex )
	{
		System::Diagnostics::Debug::WriteLine( ex->ToString() );
	}
}

// Let our MOG_PromptDialog know it needs to close next time it runs its Show loop
void MOG_Prompt::CancelPromptMessageDialog( Object *sender, System::ComponentModel::CancelEventArgs *e )
{
	bCancelled = true;
}

// Manage our MOG_PromptDialog while it is running until we get bCancelled == true
void MOG_Prompt::PromptMessageThreadMethod()
{
	try
	{
		if( PromptMessageDialog )
		{
			bCancelled = false;
			PromptMessageDialog->add_Closing( new CancelEventHandler( NULL, &MOG_Prompt::CancelPromptMessageDialog ) );
			if( ClientForm != NULL )
			{
				PromptMessageDialog->StartPosition = FormStartPosition::Manual;
				PromptMessageDialog->Location = MOG_Prompt::GetCenteredLocation( ClientForm, PromptMessageDialog->Size );
			}
			//PromptMessageDialog->Show();
			MOG_CoreControls::MogUtil_DepthManager::DepthManager::ShowVirtualModal(ClientForm, PromptMessageDialog, MOG_CoreControls::MogUtil_DepthManager::FormDepth::TOP);

			mVisible++;
			// Until user closes form, leave it up
			while( !PromptMessageDialog->IsDisposed && !bCancelled )
			{
				// Update our variables
				PromptMessageFormUpdate();
				// Update our form
				Application::DoEvents();
				// If we are Shutdown, break out of this...
				if(MOG_Main::IsShutdown())
				{
					// Turn of the comment below to make it so MOG_Prompt will honor our 
					//  Shutdown (for now, it will display to the user after an attempted Shutdown)
//					break;
				}

				// Sleep to allow our other Thread(s) to process
				Thread::Sleep( 100 );
			}
		}
	}
	catch( ThreadAbortException *ex )
	{
		ex->ToString();
	}
}

// This is ONLY EVER to be called from within PromptMessageThreadMethod.  It updates our MOG_PromptDialog
void MOG_Prompt::PromptMessageFormUpdate()
{
	// Set our mutex to true -- keeps PromptMessageFormStoreAppend from stepping on our toes
	mListViewMutex->WaitOne();

	// Foreach item in our ArrayList, add, then clear our arrayList cache
	for(int i = 0; i < mStoredListViewItems->Count; ++i)
	{
		PromptMessageDialog->PromptListView->Items->Add(__try_cast<ListViewItem*>(mStoredListViewItems->Item[i]));
	}

	// Clear our StoredListViewItems
	mStoredListViewItems->Clear();

	// Release our mutex
	mListViewMutex->ReleaseMutex();

	// Show the list view if it is not already visible, once we have 2 or more items to show
	if(PromptMessageDialog->PromptListView->Visible == false && PromptMessageDialog->PromptListView->Items->Count >= 2)
	{
		PromptMessageDialog->PromptListView->Visible = true;
		PromptMessageDialog->MaximumSize = System::Drawing::Size(0, 0);
		PromptMessageDialog->Height += 128;
	}

	PromptMessageDialog->CheckIfShowing();
}

void MOG_Prompt::PromptMessageFormStoreAppend(String *title, String *message, String *stackTrace, MOG_ALERT_LEVEL alertLevel)
{
	String *count = Convert::ToString(++mCurrentListViewIndex);

	// Create a holder for this message data
	ListViewMessage *holder = new ListViewMessage;
	holder->mTitle = title;
	holder->mMessage = message;
	holder->mStackTrace = stackTrace;
	holder->mLevel = alertLevel;

	// Create our list view item
	ListViewItem *item = new ListViewItem(count);
	item->ImageIndex = AlertToImageIndex(alertLevel);
	item->SubItems->Add(AlertToString(alertLevel));
    item->SubItems->Add(title);
	item->SubItems->Add(message);
	item->Tag = holder;

	// Make sure PromptMessageFormUpdate does not access the ArrayList while we're adding to it
	mListViewMutex->WaitOne();
	mStoredListViewItems->Add(item);
	mListViewMutex->ReleaseMutex();
}

// PROMPT RESPONSE METHODS

MOGPromptResult MOG_Prompt::PromptResponse(String *title, String *message)
{
	return PromptResponse(title, message, "", MOGPromptButtons::OK, MOG_ALERT_LEVEL::MESSAGE);
}

// Overload of MOGPromptResult MOG_Prompt::PromptResponse(String *title, String *message, String *stackTrace, MOGPromptButtons buttons, MOG_ALERT_LEVEL alertLevel)
MOGPromptResult MOG_Prompt::PromptResponse(String *title, String *message, MOGPromptButtons buttons)
{
	return PromptResponse(title, message, "", buttons, MOG_ALERT_LEVEL::MESSAGE );
}

// Overload of MOGPromptResult MOG_Prompt::PromptResponse(String *title, String *message, String *stackTrace, MOGPromptButtons buttons, MOG_ALERT_LEVEL alertLevel)
MOGPromptResult MOG_Prompt::PromptResponse(String *title, String *message, String *stackTrace, MOGPromptButtons buttons)
{
	return PromptResponse(title, message, stackTrace, buttons, MOG_ALERT_LEVEL::MESSAGE );
}

MOGPromptResult MOG_Prompt::PromptResponse(String *title, String *message, String *stackTrace, MOGPromptButtons buttons, MOG_ALERT_LEVEL alertLevel)
{
	if (!stackTrace)
	{
		stackTrace = S"";
	}
	
	if (!IsMode(MOG_PROMPT_SILENT))
	{
		try
		{
			// Tokenize the message string to obscure repository path information
			message = MOG_ControllerSystem::TokenizeMessageString(message);

			MOG_PromptDialog *prompt = new MOG_PromptDialog();
			prompt->SetupTitle(title);
			prompt->SetupMessage(message);
			prompt->SetupStackTrace(stackTrace);
			prompt->SetupImage(alertLevel, true);
			prompt->SetupButtons(buttons);
			mVisible++;
		
			prompt->StartPosition = FormStartPosition::CenterScreen;

			// This dialog should be pushed to topmost when it doesn't have a parent or else it can ger lost behind other apps.
			// You would think this is simple but for some reason MOG has really struggled with these dialogs being kept on top...
			// We have tried it all and finally ended up with this...toggling the TopMost mode seems to be working 100% of the time.
			prompt->TopMost = true;
			prompt->TopMost = false;
			prompt->TopMost = true;

			prompt->ShowDialog();
			
			if (mVisible)
			{
				mVisible--;
			}
			return prompt->MOGPromptResult;
		}
		// If we had any error, we need the server to know
		catch (Exception *e)
		{
			MOG_ControllerSystem::NotifySystemException( e->GetType()->ToString(), e->Message, e->StackTrace, true );
		}
	}

	return MOGPromptResult::OK;
}

// UTILITY METHODS
Point MOG_Prompt::GetCenteredLocation( Form *mainForm, Size promptSize )
{
	try
	{
		Point mainLocation = mainForm->PointToScreen(mainForm->Location);
		Size mainSize = mainForm->Size;
		int mainLowerX = mainLocation.X + mainSize.Width;
		int mainLowerY = mainLocation.Y + mainSize.Height;
		Point midPoint = Point( mainLowerX/2, mainLowerY/2 );
		int midPromptX = promptSize.Width/2;
		int midPromptY = promptSize.Height/2;
		Point promptCenterLocation = Point( midPoint.X - midPromptX, midPoint.Y - midPromptY );
		return promptCenterLocation;
	}
	catch(Exception *ex)
	{
		// Return the Point at the top-left corner of our screen
		ex->ToString();
		return Point(0,0);
	}
}

// Returns true if we need to restart our PromptMessageDialog
bool MOG_Prompt::CheckPromptMessageDialog()
{
	// Validate our form:

	// If we have a PromptMessageDialog*, double-check it's state...
	if( PromptMessageDialog != NULL)
	{
		MOG_PromptDialog *prompt = PromptMessageDialog;
		// If our prompt is showing as Disposed, we need to make sure the system closed it so 
		//  we can open a new one (otherwise, we'll get 'ghost' windows that do not respond)
		if( prompt->IsDisposed )
		{
			// Release Environment resources
			prompt->Close();
			return true;
		}
	}
	// Else, if we don't have a PromptMessageDialog object* at all...
	else if( PromptMessageDialog == NULL )
	{
		return true;
	}

	// Else, we already have valid a MOG_PromptDialog*, so use it
	return false;
}

int MOG_Prompt::AlertToImageIndex(MOG_ALERT_LEVEL alertLevel)
{
	switch(alertLevel)
	{
	case MOG_ALERT_LEVEL::MESSAGE:
		return MessageIcons::IMAGE_ALERT;
	case MOG_ALERT_LEVEL::ALERT:
		return MessageIcons::IMAGE_ALERT;
	case MOG_ALERT_LEVEL::ERROR:
		return MessageIcons::IMAGE_ERROR;
	case MOG_ALERT_LEVEL::CRITICAL:
		return MessageIcons::IMAGE_EXCEPTION;
	}
	return 0;
}

String *MOG_Prompt::AlertToString(MOG_ALERT_LEVEL alertLevel)
{
	switch(alertLevel)
	{
	case MOG_ALERT_LEVEL::MESSAGE:
		return "Message";
	case MOG_ALERT_LEVEL::ALERT:
		return "Alert";
	case MOG_ALERT_LEVEL::ERROR:
		return "Error";
	case MOG_ALERT_LEVEL::CRITICAL:
		return "Critical";
	}

	return "";
}