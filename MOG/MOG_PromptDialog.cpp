#include "stdafx.h"
#include "MOG_PromptDialog.h"

MOGPromptResult MOG_PromptDialog::get_MOGPromptResult()
{
	return mMOGPromptResult;
}

MOG_PromptDialog::MOG_PromptDialog(void)
{
	InitializeComponent();
	mMOGPromptResult = MOGPromptResult::None;

	if (!DesignMode)
	{
		// Check to see if email is an option
		if (MOG_ControllerProject::IsProject())
		{
			if (MOG_ControllerProject::GetProject()->GetConfigFile()->KeyExist("PROJECT", "EmailSmtp") == false)
			{
				PromptEmailButton->Enabled = false;
				PromptToolTip->SetToolTip(PromptEmailButton, "EmailSmtp property not set in project settings");
			}
		}
	}
}

void MOG_PromptDialog::Dispose(Boolean disposing)
{
	if (disposing && components)
	{
		components->Dispose();
	}
	__super::Dispose(disposing);
}
	
void MOG_PromptDialog::SetupStackTrace(String *stacktrace)
{
	if (stacktrace->Length)
	{
		mStackTrace = stacktrace;
		PomptMoreButton->Visible = true;
	}
	else
	{
		mStackTrace = "";
		PomptMoreButton->Visible = false;
	}
}
void MOG_PromptDialog::SetupMessage(String *message)
{
	this->PromptMessageLabel->Text = message;
}
void MOG_PromptDialog::SetupTitle(String *title)
{
	this->Text = title;
}

System::Void MOG_PromptDialog::PomptMoreButton_Click(System::Object *  sender, System::EventArgs *  e)
{
	MOG_PromptMoreDialog *dialog = new MOG_PromptMoreDialog();
	dialog->SetStackTrace(mStackTrace);
	dialog->SetInfo(Text, this->PromptMessageLabel->Text);

	dialog->Text = this->Text;
	
	dialog->ShowDialog(this);
}

System::Void MOG_PromptDialog::PromptListView_DoubleClick(System::Object *  sender, System::EventArgs *  e)
{
	MOG_PromptMoreDialog *dialog = new MOG_PromptMoreDialog();
	dialog->SetStackTrace(mStackTrace);
	dialog->SetInfo(Text, this->PromptMessageLabel->Text);

	dialog->Text = this->Text;
	
	dialog->ShowDialog(this);
}


System::Void MOG_PromptDialog::PromptOKButton_Click(System::Object *  sender, System::EventArgs *  e)
{
	Button *button = __try_cast<Button*>(sender);
	try
	{
		String *result = __try_cast<String *>(button->Tag);
		this->mMOGPromptResult = StringToMOGPromptResult(result);
	}
	catch(...)
	{
		this->mMOGPromptResult = MOGPromptResult::OK;
	}				
	this->Close();
}

System::Void MOG_PromptDialog::PromptListView_Click(System::Object *  sender, System::EventArgs *  e)
{
	for (int i = 0; i < PromptListView->SelectedItems->Count; i++)
	{
		ListViewItem *selectedItem = PromptListView->SelectedItems->Item[i];
		LoadMessageFromListView(selectedItem);
	}
}

System::Void MOG_PromptDialog::PromptEmailButton_Click(System::Object *  sender, System::EventArgs *  e)
{
	MOG_PromptMoreDialog *dialog = new MOG_PromptMoreDialog();
	dialog->SetStackTrace(mStackTrace);
	dialog->SetInfo(Text, this->PromptMessageLabel->Text);
	dialog->CopyMessageToEmail();
}

System::Void MOG_PromptDialog::PromptCopyButton_Click(System::Object *  sender, System::EventArgs *  e)
{
	MOG_PromptMoreDialog *dialog = new MOG_PromptMoreDialog();
	dialog->SetStackTrace(mStackTrace);
	dialog->SetInfo(Text, this->PromptMessageLabel->Text);
	dialog->CopyMessageToClipboard();
}


System::Void MOG_PromptDialog::MOG_PromptDialog_Closed(System::Object *  sender, System::EventArgs *  e)
{
	MOG_Prompt::DialogVisible(-1);
}


void MOG_PromptDialog::SetupImage(MOG_ALERT_LEVEL alertLevel, bool playSound)
{
	PromptPictureBoxAlert->Visible = false;
	PromptPictureBoxError->Visible = false;
	PromptPictureBoxCritical->Visible = false;

	switch(alertLevel)
	{
		case MOG_ALERT_LEVEL::MESSAGE:
			if(playSound)MessageBeep(MOG::MessageBeepType::Ok);
			PromptCopyButton->Visible = false;
			PromptEmailButton->Visible = false;
			break;
		case MOG_ALERT_LEVEL::ALERT:
			if(playSound)MessageBeep(MOG::MessageBeepType::Default);
			PromptPictureBoxAlert->Visible = true;
			PromptCopyButton->Visible = false;
			PromptEmailButton->Visible = false;
			//LoadAlertIcon(IMAGE_ALERT);
			break;
		case MOG_ALERT_LEVEL::ERROR:
			if(playSound)MessageBeep(MOG::MessageBeepType::Information);
			PromptPictureBoxError->Visible = true;
			PromptCopyButton->Visible = true;
			PromptEmailButton->Visible = true;
			//LoadAlertIcon(IMAGE_ERROR);					
			break;
		case MOG_ALERT_LEVEL::CRITICAL:
			if(playSound)MessageBeep(MOG::MessageBeepType::Error);
			PromptPictureBoxCritical->Visible = true;
			PromptCopyButton->Visible = true;
			PromptEmailButton->Visible = true;
			//LoadAlertIcon(IMAGE_EXCEPTION);
			break;
	}
}

void MOG_PromptDialog::LoadAlertIcon(int index)
{
	try
	{
		PromptPictureBox->Image = this->PromptIconsImageList->Images->Item[index];
	}
	catch(...)
	{
	}
}

void MOG_PromptDialog::LoadMessageFromListView(ListViewItem *selectedItem)
{
	if (selectedItem->Tag)
	{
		ListViewMessage *holder = __try_cast<ListViewMessage*>(selectedItem->Tag);
		
		this->SetupTitle(holder->mTitle);
		this->SetupMessage(holder->mMessage);
		this->SetupStackTrace(holder->mStackTrace);
		this->SetupImage(holder->mLevel, false);
	}
}

void MOG_PromptDialog::SetupButtons(MOGPromptButtons buttons)
{
	String *buttonString = "";
	switch(buttons)
	{
		case MOGPromptButtons::AbortRetryIgnore:
			buttonString = "Abort/Retry/Ignore";
			break;
		case MOGPromptButtons::OKCancel:
			buttonString = "Ok/Cancel";
			break;
		case MOGPromptButtons::RetryCancel:
			buttonString = "Retry/Cancel";
			break;
		case MOGPromptButtons::YesNo:
			buttonString = "Yes/No";
			break;
		case MOGPromptButtons::YesNoCancel:
			buttonString = "Yes/No/Cancel";
			break;
		case MOGPromptButtons::YesNoYesToAllNoToAll:
			buttonString = "Yes To All/No To All/Yes/No";
			break;
		default:
			return;
	}
	
	CreateButtons(buttonString);
}

void MOG_PromptDialog::CreateButtons(String *buttons)
{
	String* delimStr = S"/";
	Char delimiter[] = delimStr->ToCharArray();
	String* parts[] = buttons->Split(delimiter);
	// Reverse the order of our parts, so they display in the order they were typed...
	parts->Reverse(parts);

	if(parts != 0)
	{
		for(int i = 0; i < parts->Count; i++)
		{
			switch(i)
			{
			case 0:
				this->PromptOKButton->Text = parts[i];
				this->PromptOKButton->Visible = true;
				this->PromptOKButton->Tag = parts[i];
				this->PromptOKButton->TabIndex = 0;
				break;
			case 1:
				this->PromptButton1->Text = parts[i];
				this->PromptButton1->Visible = true;
				this->PromptButton1->Tag = parts[i];
				this->PromptButton1->TabIndex = 1;
				break;
			case 2:
				this->PromptButton2->Text = parts[i];
				this->PromptButton2->Visible = true;
				this->PromptButton2->Tag = parts[i];
				this->PromptButton2->TabIndex = 2;
				break;
			case 3:
				this->PromptButton3->Text = parts[i];
				this->PromptButton3->Visible = true;
				this->PromptButton3->Tag = parts[i];
				this->PromptButton3->TabIndex = 3;
				break;
			}
		}
	}
}

MOGPromptResult MOG_PromptDialog::StringToMOGPromptResult(String *dialogResult)
{
	if (dialogResult != NULL)
	{
		if (String::Compare(dialogResult->ToUpper(), "ABORT") == 0)
		{
			return MOGPromptResult::Abort;
		}
		else if (String::Compare(dialogResult->ToUpper(), "RETRY") == 0)
		{
			return MOGPromptResult::Retry;
		}
		else if (String::Compare(dialogResult->ToUpper(), "IGNORE") == 0)
		{
			return MOGPromptResult::Ignore;
		}
		else if (String::Compare(dialogResult->ToUpper(), "OK") == 0)
		{
			return MOGPromptResult::OK;
		}
		else if (String::Compare(dialogResult->ToUpper(), "CANCEL") == 0)
		{
			return MOGPromptResult::Cancel;
		}
		else if (String::Compare(dialogResult->ToUpper(), "YES") == 0)
		{
			return MOGPromptResult::Yes;
		}
		else if (String::Compare(dialogResult->ToUpper(), "NO") == 0)
		{
			return MOGPromptResult::No;
		}
		else if (String::Compare(dialogResult->ToUpper(), "YES TO ALL") == 0)
		{
			return MOGPromptResult::YesToAll;
		}
		else if (String::Compare(dialogResult->ToUpper(), "NO TO ALL") == 0)
		{
			return MOGPromptResult::NoToAll;
		}
	}

	return MOGPromptResult::None;
}