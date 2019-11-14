#include "stdafx.h"

#include "MOG_PromptMoreDialog.h"
#include "MOG_ControllerSystem.h"


#include "MOG_Prompt.h"
#include "MOG_Progress.h"

using namespace System::Net;
using namespace System::Net::Mail;

MOG_PromptMoreDialog::MOG_PromptMoreDialog(void)
{
	InitializeComponent();
}

void MOG_PromptMoreDialog::Dispose(Boolean disposing)
{
	if (disposing && components)
	{
		components->Dispose();
	}

	__super::Dispose(disposing);
}

System::Void MOG_PromptMoreDialog::PromptMoreTreeView_AfterSelect(System::Object *  sender, System::Windows::Forms::TreeViewEventArgs *  e)
{
	SetMessageText(__try_cast<String*>(e->Node->Tag));
}

System::Void MOG_PromptMoreDialog::PromptMoreMessageLabel_LinkClicked(System::Object *  sender, System::Windows::Forms::LinkClickedEventArgs *  e)
{
	System::Diagnostics::Process::Start(e->LinkText);
}
System::Void MOG_PromptMoreDialog::PromptCopyButton_Click(System::Object *  sender, System::EventArgs *  e)
{
	CopyMessageToClipboard();
}

System::Void MOG_PromptMoreDialog::PromptEmailButton_Click(System::Object *  sender, System::EventArgs *  e)
{
	CopyMessageToEmail();
}

System::Void MOG_PromptMoreDialog::PromptMoreOKButton_Click(System::Object *  sender, System::EventArgs *  e)
{
	Close();
}

String *MOG_PromptMoreDialog::FormatErrorMessage()
{
	String *systemMode = S"System Mode Unavailable";
	String *date = S"Date Unavailable";
	String *time = S"Time Unavailable";

	try
	{
		systemMode = MOG_ControllerSystem::GetSystemMode();
		date = DateTime::Now.ToShortDateString();
		time = DateTime::Now.ToLongTimeString();
	}
	catch(Exception *ex)
	{
		// We're already inside our message showing to the user, if we are not running silent, show our error...
		if(!MOG_Prompt::IsMode(MOG_PROMPT_MODE_TYPE::MOG_PROMPT_SILENT))
		{
			MessageBox::Show(this, ex->ToString());
		}
		// No matter what, report our error to our Server
		MOG_Report::ReportSilent(S"Crash on Copying To Clipboard", ex->Message, ex->StackTrace, MOG_ALERT_LEVEL::ERROR);
	}

	return String::Concat(
		S"========================================================\r\n",
		S"---------------- MOG ERROR REPORT ----------------------\r\n",
		S"========================================================\r\n",
		S" MOG COMPONENT: ", systemMode, S"\r\n",
		S" DATE: ", date, S"\r\n",
		S" TIME: ", time, S"\r\n",
		S"========================================================\r\n\r\n",
		S" TITLE:\r\n",
		mFullTitle, S"\r\n\r\n",
		S" MESSAGE:\r\n",
		mFullMessage, S"\r\n\r\n",
		S" STACKTRACE:\r\n",
		mFullStackTrace,
		S"\r\n\r\n COMMENTS:\r\n"
		);
}

void MOG_PromptMoreDialog::CopyMessageToClipboard()
{
	try
	{
		// Takes the selected text from a text box and puts it on the clipboard.
		Clipboard::SetDataObject(FormatErrorMessage());
	}
	catch(Exception *ex)
	{
		// We're already inside our message showing to the user, if we are not running silent, show our error...
		if(!MOG_Prompt::IsMode(MOG_PROMPT_MODE_TYPE::MOG_PROMPT_SILENT))
		{
			MessageBox::Show(this, ex->ToString());
		}
		// No matter what, report our error to our Server
		MOG_Report::ReportSilent(S"Crash on Copying To Clipboard", ex->Message, ex->StackTrace, MOG_ALERT_LEVEL::ERROR);
	}
}

void MOG_PromptMoreDialog::CopyMessageToEmail()
{
	ProgressDialog* progress = new ProgressDialog(S"Reporting Error", S"Sending error report to support@mogware.com", new DoWorkEventHandler(this, &MOG_PromptMoreDialog::CopyMessageToEmail_Worker), NULL, false);
	progress->ShowDialog();
}

void MOG_PromptMoreDialog::CopyMessageToEmail_Worker(Object* sender, DoWorkEventArgs* e)
{
	try
	{
		String *userEmail = (MOG_ControllerProject::GetUser()->GetUserEmailAddress()->Length > 0 && MOG_ControllerProject::GetUser()->GetUserEmailAddress()->IndexOf("@") != -1) ? MOG_ControllerProject::GetUser()->GetUserEmailAddress() : "Support@MOGware.com";
		MailMessage *message = new MailMessage(userEmail, S"support@mogware.com");
		message->Body = FormatErrorMessage();
		message->Subject = S"MOG ERROR REPORT";

		SmtpClient* client = new SmtpClient(S"mail.mogware.com");
		client->Credentials = new NetworkCredential(S"support@mogware.com", S"J3RKK");
		client->DeliveryMethod = SmtpDeliveryMethod::Network;

		client->Send(message);
	}
	catch(Exception *e)
	{
		MessageBox::Show(e->Message, S"Send Email");
	}
}

void MOG_PromptMoreDialog::SetInfo(String *title, String *message)
{
	mFullTitle = title; 
	mFullMessage = message;
	
	this->PromptMoreErrorMessageLabel->Text = message;

	if (mFullStackTrace->Length)
	{
		PromptMoreToolTip->SetToolTip(PromptEmailButton, FormatErrorMessage());
	}
}

void MOG_PromptMoreDialog::SetMessageText(String *text)
{
	String *sourceFile = "";

	// Check if this line has a source file ascociated
	if (text->IndexOf(":\\") != -1 && text->IndexOf(":line") != -1)
	{
		// Now try and issolate the source file from the line
		sourceFile = text->Substring(text->IndexOf(":\\")-1, text->IndexOf(":line") - (text->IndexOf(":\\")-1));
	}

	this->PromptMoreMessageLabel->Text = String::Concat(text, S"\n\nFile:/", sourceFile);
}

void MOG_PromptMoreDialog::SetStackTrace(String *stackTrace)
{
	// Save full stackTrace
	mFullStackTrace = stackTrace;

	// Setup a token to help us break the stack trace into modules
	String *formated = stackTrace->Replace(" at", "$");

	// Remove the carriage returns from our stacktrace
	formated = formated->Replace("\r\n", "\n");

	// Split the stack trace into each module
	String* delimStr = S"$";
	Char delimiter[] = delimStr->ToCharArray();
	String* parts[] = formated->Split(delimiter);
	
	// Create a root node in the tree that has the entire stack trace 
	TreeNode *rootNode = new TreeNode("All Messages", NODE_IMAGES::ALLCOMPUTER, NODE_IMAGES::ALLCOMPUTER);
	rootNode->Tag = stackTrace;
	this->PromptMoreTreeView->Nodes->Add(rootNode);

	// Create a root node in the tree that has the entire stack trace 
	TreeNode *sourceNode = new TreeNode("Source Messages", NODE_IMAGES::ALLCOMPUTER, NODE_IMAGES::ALLCOMPUTER);
	sourceNode->Tag = stackTrace;
	this->PromptMoreTreeView->Nodes->Add(sourceNode);

	// Create a root node in the tree that has the entire stack trace 
	TreeNode *winNode = new TreeNode("Windows Messages", NODE_IMAGES::ALLCOMPUTER, NODE_IMAGES::ALLCOMPUTER);
	winNode->Tag = stackTrace;
	this->PromptMoreTreeView->Nodes->Add(winNode);

	PromptMoreMessageLabel->Text = "";
	
	// Now loop through all the modules found in the stacktrace and make a series of tree nodes for them
	if (parts != 0)
	{
		for (int i = 0; i < parts->Count; i++)
		{
			String *nodeText = parts[i]->Trim();
			int imageIndex = NODE_IMAGES::WINDOWS;

			// Determine if this module has source code or if it is a system call
			if (parts[i]->IndexOf("\\") != -1)
			{
				// It has a back slash, so it must have source code
				nodeText = parts[i]->Substring(parts[i]->LastIndexOf("\\"))->Trim();
				imageIndex = NODE_IMAGES::SOURCE;
			}

			// If we have a valid line, add it to the tree
			if (parts[i]->Trim()->Length > 0)
			{
				TreeNode *node = new TreeNode(nodeText, imageIndex, imageIndex);
				
				node->Tag = parts[i];

				rootNode->Nodes->Add(node);

				// Add a copy of this node to either the source tree or the windows tree
				if (node->ImageIndex == NODE_IMAGES::SOURCE)
				{
					// Set the default text for this message
					if (PromptMoreMessageLabel->Text->Length == 0)
					{
						this->SetMessageText(parts[i]);
					}
					sourceNode->Nodes->Add(__try_cast<TreeNode*>(node->Clone()));
				}
				else
				{
					winNode->Nodes->Add(__try_cast<TreeNode*>(node->Clone()));
				}
			}
		}

		sourceNode->Expand();
	}
}
