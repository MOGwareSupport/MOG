//--------------------------------------------------------------------------------
//	MOG_DBTaskAPI.h
//	
//	
//--------------------------------------------------------------------------------
#ifndef __MOG_DBTASKAPI_H__
#define __MOG_DBTASKAPI_H__


#using <mscorlib.dll>
#using <system.dll>
#using <System.Data.dll>

#include "stdlib.h"
#include "MOG_Define.h"
#include "MOG_TaskInfo.h"
#include "MOG_DBProjectAPI.h"


using namespace System;
using namespace System::Data;
using namespace System::Data::SqlClient;



namespace MOG {
namespace DATABASE {

public __gc class MOG_DBTaskAPI
{
private:
	static MOG_TaskInfo *QueryTask(String *selectString);
	static ArrayList *QueryTasks(String *selectString);

	static MOG_TaskInfo *SQLReadTask(SqlDataReader *myReader);
	static bool SQLWriteTask(MOG_TaskInfo *taskInfo);
	static MOG_TaskInfo *SQLCreateTask(MOG_TaskInfo *taskInfo);
	static bool SQLSyncTaskIDs(MOG_TaskInfo *taskInfo);

	// Internal MOG Specific Access Functions
	static int GetTaskResponsiblePartyID(String *name);
	static String *GetTaskResponsibleParty(int id);
	static String *GetTaskProject(int id);

	static ArrayList *QueryTaskAssets(String *selectString);

public:
	static ArrayList *GetPriorites();

	static ArrayList *GetDepartmentTasks(String *departmentName);

	// Task Related API
	static ArrayList *GetUserTasks(String *userName);
	static ArrayList *GetAllUserTasks(String *userName);
	static MOG_TaskInfo *GetTask(int task_ID);
	static int GetTaskID(String *name, String *timeStamp);
	static ArrayList *GetChildrenTasks(int task_ID);
	static MOG_TaskInfo *CreateTask(MOG_TaskInfo *taskInfo);
	static bool UpdateTask(MOG_TaskInfo *taskInfo);
	static int SetParentTaskID(MOG_TaskInfo *task, int parentTaskId);

	static bool AddTaskAssetLink(int task_ID, MOG_Filename *assetName);
	static bool RemoveTaskAssetLink(int task_ID, MOG_Filename *assetName);
	static ArrayList *GetTaskAssetLinks(int task_ID);

	static bool RemoveTask(int taskID);
};


}
}

using namespace MOG;
using namespace MOG::DATABASE;

#endif


