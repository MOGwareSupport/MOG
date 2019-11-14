#include "stdafx.h"

#include "MOG_Main.h"
#include "MOG_Report.h"
#include "MOG_Time.h"
#include "MOG_DBInboxAPI.h"
#include "MOG_DosUtils.h"
#include "MOG_Ini.h"

#include "MOG_Database.h"
#include "MOG_ControllerSystem.h"
#include "MOG_DBErrorHandler.h"
#include "MOG_Progress.h"

using namespace MOG_CoreControls;


MOG_SqlErrorHandler::MOG_SqlErrorHandler()
{
	mCurrentError = NULL;
	mPreviousError = NULL;
	mRecoveryModel = UNRECOVERABLE;
	mRetryNumber = 0;
}

MOG_SqlErrorHandler::MOG_SqlErrorHandler(SqlException *exception)
{
	mCurrentError = NULL;
	mPreviousError = NULL;
	mRecoveryModel = UNRECOVERABLE;
	mRetryNumber = 0;
	HandleException(exception);
}

SqlErrorRecoveryModel MOG_SqlErrorHandler::HandleException(SqlException *exception)
{
	mPreviousError = mCurrentError;
	mCurrentError = exception;
	bool mPermissionError = false;

	//Check to see if this is a continuation of a current error or if this is a new error
	if(!mPreviousError || mPreviousError->Number != mCurrentError->Number)
	{
		CleanUpSqlErrorHandler();
		mPreviousError = mCurrentError;
		mCurrentError = exception;
		//We have a new error determine how to handle it.
		switch(mCurrentError->Number)
		{
		case 11:
			//This is the error we get when the sql server is unreachable while trying to read from the database
			mMessage = String::Concat(S"Unable to connect to SQL Database.",
										S"\nPlease check your network connection and verify that your SQL Server is up and running.",
										S"\nThis is a recoverable SQL error.",
										S"\nMOG will attempt to reestablish the connection to the SQL Server and complete the query until you press cancel.");
			mRecoveryModel = RECOVERABLE;
			mNumberOfUpdates = 10;
			mRecoverWaitMS = 10000;
			break;
		case 17:
			//This is the error where we are unable to connect to the SqlServer
			mRecoveryModel = RECOVERABLE;
			mMessage = S"MOG is unable to connect to the specificed SQL Server";
			mNumberOfUpdates = 10;
			mRecoverWaitMS = 10000;
			break;
		case 102:
			//incorrect Sql Syntax
			mMessage = S"There is somthing wrong with the SQL query you are attempting to run";
			mRecoveryModel = UNRECOVERABLE;
			break;
		case 105:
			//Unclosed quotation mark
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			break;
		case 218:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 219:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 229:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 230:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 262:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 297:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 300:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 929:
			//Unable to close the database because it is already closed.
			mRecoveryModel = IGNORABLE;
			break;
		case 1088:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 1802:
			//case where we can't create the data files necessary to create a database
			mMessage = String::Concat(S"Mog was unable to create the database you specified.\n",
										S"This issue is usually caused by the fact that the default data file names for this database are already in use.\n",
										S"If this is the problem it can be resolved one of three ways.\n",
										S"	1.  Choose a diffrent name for your database.\n",
										S"	2.  Have the Database administrator create a new database with this name on your SQL Server.\n",
										S"	3.  Have your database administrator determine if the default files for this database are not in use and clean them up.\n",
										S"CAUTION: Cleaning up data files which are in use by other databases may result in complete data loss for that database.");
			mRecoveryModel = IGNORABLE;
			mPermissionError = false;
			break;
		case 2104:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 2557:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 2571:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 2627:
			mMessage = mCurrentError->Message;
			mRecoveryModel = IGNORABLE;
			break;
		case 2760:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 2793:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 3110:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 3505:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 3701:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 4602:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 4604:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 4610:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 4613:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 4618:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 4619:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 4625:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 4701:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 4902:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 5011:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 5828:
			//Out of connections 
			mMessage = S"The SQL server is out of available connections.  Waiting for connections to become available.";
			mRecoveryModel = RECOVERABLE;
			mNumberOfUpdates = 10;
			mRecoverWaitMS = 10000;
			break;
		case 6004:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 6005:
			//Sql Server is shutting down.
			mMessage =  mCurrentError->Message;
			mRecoveryModel = RECOVERABLE;
		case 6102:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 6347:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 6348:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 6515:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 7641:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 7658:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 7666:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 7809:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 7821:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 7983:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 8189:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 8401:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 8494:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 9010:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 9205:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 9939:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 9967:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 10024:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 11010:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 11045:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 11211:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 11215:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 14080:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 14126:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 14260:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 14631:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 15007:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 15036:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 15096:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 15148:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 15151:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 15165:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 15240:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 15247:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 15282:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 15388:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 15403:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 15469:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 15470:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 15472:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 15517:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 15622:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		case 18452:
			//User not  associated with trusted Sql Server Connection.
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
		case 18471:
			//permissions case
			mMessage = mCurrentError->Message;
			mRecoveryModel = UNRECOVERABLE;
			mPermissionError = true;
			break;
		default:
			mMessage = String::Concat(S"ERROR NUMBER: ", Convert::ToString(mCurrentError->Number), S"\nMessage: ", mCurrentError->Message);
			//assume unrecoverable error if we have not explicitly determined it is recoverable.
			mRecoveryModel = UNRECOVERABLE;
			break;
		}
		switch(mRecoveryModel)
		{
		case UNRECOVERABLE:
			if(mMessage)
			{
				if(mPermissionError)
				{
					MOG_Prompt::PromptResponse(S"Unrecoverable SQL Permission Error", String::Concat(mMessage, S"\nThis is an unrecoverable SQL Permission error.\nPlease contact your MOG administrator to ensure you have appropriate permissions"),MOG::PROMPT::MOGPromptButtons::OK);
				}
				else
				{
					MOG_Prompt::PromptResponse(S"Unrecoverable SQL Error", String::Concat(mMessage, S"\nThis is an unrecoverable SQL error your MOG session will be terminated\nPlease reopen MOG and try your previous action again."),MOG::PROMPT::MOGPromptButtons::OK);
				}
			}
			break;
		case RECOVERABLE:
			break;
		case IGNORABLE:
			if(mMessage)
			{
				if(mMessage)
				{
//					if(MOG_Prompt::PromptResponse(S"Ignorable SQL Error", String::Concat(mMessage,S"\nPress OK to Continue or Cancel to Terminate your MOG Session"), MOG::PROMPT::MOGPromptButtons::OKCancel) == MOG::PROMPT::MOGPromptResult::Cancel)
//					{
//						mRecoveryModel = UNRECOVERABLE;
//					}
					String *title = S"Ignorable SQL Error";
					String *message = exception->Message;
					MOG_Report::ReportMessage(title, message, Environment::StackTrace, MOG_ALERT_LEVEL::ERROR);
				}
			}
			break;
		default:
			break;
		}
	}
	
	if (mRecoveryModel == RECOVERABLE)
	{
		ProgressDialog* progress = new ProgressDialog(S"Recoverable SQL Error", String::Concat( mMessage, S"\nTo terminate recovery  press Cancel which will close MOG"), new DoWorkEventHandler(this, &MOG_SqlErrorHandler::HandleException_Worker), NULL, false);
		if (progress->ShowDialog() == DialogResult::Cancel)
		{
			mRecoveryModel = UNRECOVERABLE;
		}
	}

	if(mRecoveryModel == UNRECOVERABLE)
	{
		//MOG_Main::Shutdown(true);
		throw(new Exception("UNRECOVERABLE SQL ERROR!", this->mCurrentError));
	}

	return mRecoveryModel;
}

void MOG_SqlErrorHandler::HandleException_Worker(Object* sender, DoWorkEventArgs* e)
{
	BackgroundWorker* worker = dynamic_cast<BackgroundWorker*>(sender);
	
	mRetryNumber++;
	for (int i = 0; i < mNumberOfUpdates && !worker->CancellationPending; i++)
	{
		Thread::Sleep(mRecoverWaitMS / mNumberOfUpdates);
	}
}

bool MOG_SqlErrorHandler::CleanUpSqlErrorHandler(void)
{
	mMessage = NULL;
	mCurrentError = NULL;
	mPreviousError = NULL;
	mRecoveryModel = UNRECOVERABLE;
	mRetryNumber = 0;
	return true;
}
