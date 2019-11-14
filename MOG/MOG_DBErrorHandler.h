#ifndef __MOG_DBERRORHANDLER_H__
#define __MOG_DBERRORHANDLER_H__

#using <mscorlib.dll>
#using <system.dll>
#using <System.Data.dll>

#include "stdlib.h"
#include "MOG_Define.h"
#include "MOG_DBCache.h"


using namespace System;
using namespace System::Data;
using namespace System::Data::SqlClient;
using namespace System::Collections;



namespace MOG {
namespace DATABASE {

public __value enum SqlErrorRecoveryModel
{
	UNRECOVERABLE,
	RECOVERABLE,
	IGNORABLE
};

//This is an object used to handle Sql Exceptions for All of Mog.  It has all the necessary functionality to manage Sql Exceptions.
public __gc class MOG_SqlErrorHandler
{
public:
	MOG_SqlErrorHandler();
	MOG_SqlErrorHandler(SqlException *exception);
	SqlException *GetCurrentError(){return mCurrentError;};
	SqlException *GetPreviousError(){return mPreviousError;};
	SqlErrorRecoveryModel IsRecoverable(){return mRecoveryModel;};
	SqlErrorRecoveryModel HandleException(SqlException *exception);
	bool CleanUpSqlErrorHandler(void);

private:
	void HandleException_Worker(Object* sender, DoWorkEventArgs* e);

private:
	SqlException *mCurrentError;
	SqlException *mPreviousError;
	SqlErrorRecoveryModel mRecoveryModel;
	int mRetryNumber;
	int mRecoverWaitMS;
	int mNumberOfUpdates;
	String *mMessage;
};
}
}


using namespace MOG::DATABASE;

#endif