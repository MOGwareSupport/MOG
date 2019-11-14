//--------------------------------------------------------------------------------
//	MOG_DBAPI.h
//	
//	
//--------------------------------------------------------------------------------
#ifndef __MOG_DBAPI_H__
#define __MOG_DBAPI_H__


#using <mscorlib.dll>
#using <system.dll>
#using <System.Data.dll>

#include "stdlib.h"
#include "MOG_Define.h"
#include "MOG_DBErrorHandler.h"


using namespace System;
using namespace System::Data;
using namespace System::Data::SqlClient;
using namespace System::Collections;
using namespace System::Threading;

namespace MOG {
namespace DATABASE {

public __gc class MOG_DBAPI
{
// JohnRen - Managed C doesn't support friends!
// We still need these functions exposed to the other Database classes...
//protected:
public:
	// Generic SQL Database Access Functions
	static int QueryInt(String *selectString, String *columnName);
	static ArrayList *QueryIntArray(String *selectString, String *columnName);
	static String *QueryString(String *selectString, String *columnName);
	static ArrayList *QueryStrings(String *selectString, String *columnName);
	static ArrayList *QueryStringsArray(String *selectString, String *columnNames[]);
	static bool QueryExists(String *selectString);

	static String *SafeStringRead(SqlDataReader *myReader, String *columnName);
	static int SafeIntRead(SqlDataReader *myReader, String *columnName);

	// used to fix things like having a ' inside a string parameter
	static String *FixSQLParameterString(String *theString);
	static String *TruncateIfNecessary(String *testString, int length, bool truncateFront);
	static bool ExecuteTransaction(ArrayList *queryList);
	static bool ExecuteTransaction(ArrayList *queryList, int groupSize);

	static SqlConnection *GetOpenSqlConnection(String *connectionString);
	static SqlDataReader *GetNewDataReader( String *commandString, SqlConnection *myConnection );
	static String *GetNewSecularValueAsString( String *commandString, String *connectionString );
	static int GetNewSecularValueAsInt( String *commandString, String *connectionString );
	static bool RunNonQuery( String *commandString, String *connectionString );
	static bool RunNonQuery( ArrayList *commandStrings, String *connectionString );

	static bool TableExists(String *tableName);
	static bool ColumnExists(String *tableName, String *columnName);

	static bool DatabaseExists(String *connectionString, String *databaseName);
	static bool CreateDatabase(String *connectionString, String *databaseName);

	static bool TestServerConnection(String * connectionString);
	static bool TestDatabaseConnection(String *connectionString, String *databaseName);

	static String *GetModelDatabaseFilePath(String *connectionString);
	static String *GetModelLogFilePath(String * connectionString);
};
}
}

using namespace MOG;
using namespace MOG::DATABASE;

#endif


