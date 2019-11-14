#include "stdafx.h"
#include "MOG_ProgressDialog.h"
#include "MOG_Progress.h"


MOG_ProgressDialog::MOG_ProgressDialog(void)
{
	InitializeComponent();
	this->bCancelled = false;
	this->mOpeningStackTrace = MOG_Environment_StackTrace;
	mMOGPromptResult = MOGPromptResult::None;

	mTitle = S"Progress Dialog";
	mProgressMessage = S"Please Wait...";
	mFileMessageVisible = false;
	mProgressFileMessage = S"Please Wait...";
	mProgressBarVisible = false;
	mProgressBarPosition = 0;
	mProgressBarPositionMin = 0;
	mProgressBarPositionMax = 100;
	mSecondaryProgressBar = false;
	mProgressBar2Visible = false;
	mProgressBar2Position = 0;
	mProgressBar2PositionMin = 0;
	mProgressBar2PositionMax = 100;
	mProgressPictureVisible = false;
}


// Properties
//***************************************************************
bool MOG_ProgressDialog::get_IsRunning()
{
	if( mDisplayThread )
	{
		return mDisplayThread->IsAlive;
	}
	return false;
}

MOGPromptResult MOG_ProgressDialog::get_MOGPromptResult()
{
	return mMOGPromptResult;
}		



// Setup functions
//***************************************************************
void MOG_ProgressDialog::SetupMessage(String *message)
{
	mProgressMessage = message;
}

void MOG_ProgressDialog::SetupTitle(String *title)
{
	mTitle = title;
}

void MOG_ProgressDialog::SetupProgressGraphic()
{
	// Move over our label
	Point location = ProgressMessageLabel->Location;
	System::Drawing::Size size = ProgressMessageLabel->Size;
	location.X += this->MOGProgressPictureBox->Width;
	size.Width -= this->MOGProgressPictureBox->Width;
	this->ProgressMessageLabel->Location = location;
	this->ProgressMessageLabel->Size = size;

	// Move over our file label
	location = ProgressFileLabel->Location;
	size = ProgressFileLabel->Size;
	location.X += this->MOGProgressPictureBox->Width;
	size.Width -= this->MOGProgressPictureBox->Width;
	this->ProgressFileLabel->Location = location;
	this->ProgressFileLabel->Size = size;

	// Disable progress bars
	mProgressBarVisible = false;
	mProgressBar2Visible = false;

	// Set our frame counter to the beginning
	mGraphicFrame = 0;

	// Finally activate the graphic
	mProgressPictureVisible = true;
}

void MOG_ProgressDialog::SecondaryProgressBarSetup(int minimum, int maximum)
{
	mSecondaryProgressBar = true;
	mProgressBar2PositionMin = minimum;
	mProgressBar2PositionMax = maximum;
}

void MOG_ProgressDialog::SecondaryProgressBarClose()
{
	mSecondaryProgressBar = false;
	mProgressBar2Position = 0;
}

void MOG_ProgressDialog::SetupProgressBar(int minimum, int maximum)
{
	if (mSecondaryProgressBar)
	{
		if (maximum)
		{
			mProgressBar2Visible = true;
		}
		else
		{
			mProgressBar2Visible = false;
		}

		mProgressBar2PositionMin = minimum;
		mProgressBar2PositionMax = maximum;
	}
	else
	{
		if (maximum)
		{
			mProgressBarVisible = true;
		}
		else
		{
			mProgressBarVisible = false;
		}

		mProgressBarPositionMin = minimum;
		mProgressBarPositionMax = maximum;
	}
}	



// Update Functions
//***************************************************************
void MOG_ProgressDialog::UpdateFileMessage(String *message)
{
	if (message->Length != 0)
	{
		mFileMessageVisible = true;

		// KLK - I Had to disable this code because the: ProgressFileLabel->CreateGraphics() call would hang all the time?

		/*Graphics *g = NULL;

		try
		{
			String *formatedDescription = "";

			String* delimStr = S"\n";
			Char delimiter[] = delimStr->ToCharArray();
			String *parts[] = message->Split(delimiter);
			g = this->ProgressFileLabel->CreateGraphics();

			if (parts != NULL && parts->Length > 0)
			{
				for (int p = 0; p < parts->Count; p++)
				{
					String *part = parts[p];
					String *line = part;

					if (this->ProgressFileLabel && this->ProgressFileLabel->Font)
					{
						int strLength = (int)g->MeasureString(part, this->ProgressFileLabel->Font).Width;
												
						if (  strLength > this->ProgressFileLabel->Width && this->ProgressFileLabel->Width > 0)
						{
							int increment = 5;							
							while(strLength > this->ProgressFileLabel->Width)
							{
								line = String::Concat(S"...", part->Substring(increment));
								strLength = (int)g->MeasureString(line, this->ProgressFileLabel->Font).Width;
								increment += 3;
							}							
						}
					}
				
					if (formatedDescription->Length == 0)
					{
						formatedDescription = line;
					}
					else
					{
						formatedDescription = String::Concat(formatedDescription, S"\n", line);
					}
				}
				
			}
			this->ProgressFileLabel->Text = formatedDescription;
		}
		catch(...)
		{*/
			mProgressFileMessage = message;
		/*}
		__finally
		{
			if (g)
			{
				g->Dispose();
			}
		}*/
	}
	else
	{
		mFileMessageVisible = false;
	}
}

void MOG_ProgressDialog::UpdateMessage(String *message)
{
	mProgressMessage = message;
}

void MOG_ProgressDialog::UpdateProgressBar()
{
	double val0 = Convert::ToDouble(ProgressBar->Position);
	double val1 = Convert::ToDouble(ProgressBar->PositionMax);
	double val = Convert::ToDouble(val0 / val1);
	val = val * 100;

	int percent = (ProgressBar->Position == 0) ? 0 : (int)val;
	mTitle = String::Concat(percent.ToString() , S"% Complete...");
}

void MOG_ProgressDialog::UpdateProgressBarValue(int value)
{
	mProgressBarPosition = value;
}

MOG::PROMPT::MOGPromptResult MOG_ProgressDialog::UpdateProgressValue( String *message, String *fileMessage, int value )
{
	try
	{
		if (mSecondaryProgressBar)
		{
			mProgressBar2Position = value;					
		}
		else
		{
			// If we have a message to update, update it
			if( message )
			{
				UpdateMessage(message);
			}
			mProgressBarPosition = value;
			UpdateFileMessage(fileMessage);
			UpdateProgressBar();
		}
	}
	catch( Exception *ex )
	{
		ex->ToString();
		MOG_Report::ReportSilent( S"Error with Progress Dialog, UpdateProgressStep()", ex->Message, ex->StackTrace, MOG_ALERT_LEVEL::ERROR );
	}
	return this->MOGPromptResult;
}

MOG::PROMPT::MOGPromptResult MOG_ProgressDialog::UpdateProgress( String *message, String *fileMessage )
{
	return this->UpdateProgressStep( message, fileMessage, 0 );
}

MOG::PROMPT::MOGPromptResult MOG_ProgressDialog::UpdateProgressStep( String *message, String *fileMessage )
{
	return this->UpdateProgressStep( message, fileMessage, 1 );
}

MOG::PROMPT::MOGPromptResult MOG_ProgressDialog::UpdateProgressStep( String *message, String *fileMessage, int step )
{
	try
	{
		if (mSecondaryProgressBar)
		{
			mProgressBar2Position += step;
		}
		else
		{
			// If we have a message to update, update it
			if( message )
			{
				UpdateMessage(message);
			}
			mProgressBarPosition += step;

			UpdateFileMessage(fileMessage);
			UpdateProgressBar();
		}
	}
	catch( Exception *ex )
	{
		ex->ToString();
		MOG_Report::ReportSilent( S"Error with Progress Dialog, UpdateProgressStep()", ex->Message, ex->StackTrace, MOG_ALERT_LEVEL::ERROR );
	}
	return this->MOGPromptResult;
}

MOG::PROMPT::MOGPromptResult MOG_ProgressDialog::UpdateProgressGraphic(String *message)
{
	// If we have a message to update, update it
	if( message )
	{
		UpdateFileMessage(message);
	}
	return this->MOGPromptResult;
}



// Show functions
//***************************************************************
void MOG_ProgressDialog::ShowMethod()
{
	// Display our Form
	__super::Show();

	try
	{
		// While we are not cancelled...
		while( !bCancelled )
		{
			// Update our form's variables within the thread
			this->Text = mTitle;
			this->ProgressMessageLabel->Text = mProgressMessage;
			this->ProgressFileLabel->Visible = mFileMessageVisible;
			this->ProgressFileLabel->Text = mProgressFileMessage;
			this->ProgressBar->Visible = mProgressBarVisible;
			this->ProgressBar->Position = mProgressBarPosition;
			this->ProgressBar->PositionMin = mProgressBarPositionMin;
			this->ProgressBar->PositionMax = mProgressBarPositionMax;
			this->ProgressBar2->Visible = mProgressBar2Visible;
			this->ProgressBar2->Position = mProgressBar2Position;
			this->ProgressBar2->PositionMin = mProgressBar2PositionMin;
			this->ProgressBar2->PositionMax = mProgressBar2PositionMax;
			//this->MOGProgressPictureBox->Visible = mProgressPictureVisible;

			// Check if the graphic is visable?
			if (mProgressPictureVisible)
			{
				// Increment the mGraphicFrame counter
				mGraphicFrame += 1;
				if (mGraphicFrame >= TOTAL_FRAME_COUNT)
				{
					// Loop back to the beginning
					mGraphicFrame = 0;
				}

				switch(mGraphicFrame)
				{
				case 0:
					this->MOGProgressPictureBox->Visible = true;
					break;
				case 1:
					this->MOGProgressPictureBox->Visible = false;
					this->pictureBox1->Visible = true;
					break;
				case 2:
					this->pictureBox1->Visible = false;
					this->pictureBox2->Visible = true;
					break;
				case 3:
					this->pictureBox2->Visible = false;
					this->pictureBox3->Visible = true;
					break;
				case 4:
					this->pictureBox3->Visible = false;
					this->pictureBox4->Visible = true;
					break;
				case 5:
					this->pictureBox4->Visible = false;
					this->pictureBox5->Visible = true;
					break;
				case 6:
					this->pictureBox5->Visible = false;
					this->pictureBox6->Visible = true;
					break;
				case 7:
					this->pictureBox6->Visible = false;
					this->pictureBox7->Visible = true;
					break;
				case 8:
					this->pictureBox7->Visible = false;
					this->pictureBox8->Visible = true;
					break;
				case 9:
					this->pictureBox8->Visible = false;
					this->pictureBox9->Visible = true;
					break;
				}

				// Load the next frame
				//this->MOGProgressPictureBox->Image = this->MOGProgressImageList->Images->Item[mGraphicFrame];
			}

			// Perform all events
			Application::DoEvents();

			// Gracefully sleep
			Thread::Sleep(100);
		}

		// Allow our Thread to close this form by itself
		this->Close();
	}
	// Eat any exceptions
	catch( Exception *ex )
	{
		ex->ToString();
	}
}

void MOG_ProgressDialog::Show( String *title, String *message )
{
	this->Show( title, message, 0, 100 );
}

void MOG_ProgressDialog::Show( String *title, String *message, int progressMax )
{
	this->Show( title, message, 0, progressMax );
}

void MOG_ProgressDialog::Show( String *title, String *message, int progressMin, int progressMax)
{
	try
	{
		// If we already have a DisplayThread, we need to throw
		if( this->mDisplayThread )
		{
			throw new Exception( String::Concat( S"MOG_ProgressDialog is already being shown!!\r\n\r\n",
				S"Opening Stack Trace:\r\n\r\n", this->mOpeningStackTrace, S"\r\n\r\nInternal Stack Trace:\r\n\r\n", Environment::StackTrace ));
		}

		SetupTitle(title);
		SetupMessage(message);
		ProgressFileLabel->Text = "";
		SetupProgressBar(progressMin, progressMax);

		this->mDisplayThread = new Thread( new ThreadStart( this, &MOG_ProgressDialog::ShowMethod ) );
		this->mDisplayThread->Priority = ThreadPriority::Normal;
		this->mDisplayThread->Name = String::Concat( S"MOG_ProgressDialog", __box(++count));
		this->mDisplayThread->Start();
	}
	catch( Exception *ex )
	{
		ex->ToString();
		MOG_Report::ReportSilent( S"Error with Progress Dialog, Show()", ex->Message, ex->StackTrace, MOG_ALERT_LEVEL::ERROR );
	}
}


// Close functions
//***************************************************************
void MOG_ProgressDialog::AbortThread()
{
	this->bCancelled = true;
}

System::Void MOG_ProgressDialog::MOG_ProgressDialog_Closing(System::Object *  sender, System::ComponentModel::CancelEventArgs *  e)
{
	this->mMOGPromptResult = MOGPromptResult::Cancel;
	this->bCancelled = true;
}

// Click event functions
//***************************************************************
System::Void MOG_ProgressDialog::ProgressCancelButton_Click(System::Object *  sender, System::EventArgs *  e)
{
	this->mMOGPromptResult = MOGPromptResult::Cancel;
	this->Close();
}
