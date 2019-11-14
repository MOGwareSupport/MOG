//--------------------------------------------------------------------------------
//	MOG_DBEventAPI.h
//	
//	
//--------------------------------------------------------------------------------
#ifndef __MOG_DBEVENTAPI_H__
#define __MOG_DBEVENTAPI_H__


#using <mscorlib.dll>
#using <system.dll>
#using <System.Data.dll>

#include "stdlib.h"
#include "MOG_Define.h"
#include "MOG_DBAPI.h"


using namespace System;
using namespace System::Data;
using namespace System::Data::SqlClient;



namespace MOG {
namespace DATABASE {

public __gc struct MOG_DBEventInfo
{
	int mID;
	String *mType;
	String *mTimeStamp;
	String *mTitle;
	String *mStackTrace;
	String *mDescription;
	String *mEventID;
	String *mUserName;
	String *mComputerName;
	String *mProjectName;
	String *mBranchName;
	int mRepeatCount;

	MOG_DBEventInfo()
	{
		mID = 0;
		mType = "";
		mTimeStamp = "";
		mTitle = "";
		mStackTrace = "";
		mDescription = "";
		mEventID = "";
		mUserName = "";
		mComputerName = "";
		mProjectName = "";
		mBranchName = "";
		mRepeatCount = 0;
	}
};


public __gc class MOG_DBEventAPI
{
public:
	static ArrayList *GetEvents(String *minTimeStamp, String *maxTimeStamp, String *projectName, String *branchName);
	static bool AddEvent(String *type, String *timeStamp, String *title, String *description, String *stackTrace, String *eventID, String *userName, String *computerName, bool allowDuplicates);
	static bool RemoveEvent(MOG_DBEventInfo *eventInfo);
	static bool RemoveEvents(ArrayList *events);

protected:
	static MOG_DBEventInfo *QueryEvent(String *selectString);
	static ArrayList *QueryEvents(String *selectString);
	static MOG_DBEventInfo *SQLCreateEvent(MOG_DBEventInfo *eventInfo, bool allowDuplicates);
	static MOG_DBEventInfo *SQLReadEvent(SqlDataReader *myReader);
	static bool SQLUpdateEvent(MOG_DBEventInfo *eventInfo);
	static bool SQLRemoveEvent(MOG_DBEventInfo *eventInfo);
};


}
}

using namespace MOG;
using namespace MOG::DATABASE;

#endif
