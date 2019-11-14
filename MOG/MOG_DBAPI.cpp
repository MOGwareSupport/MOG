
//--------------------------------------------------------------------------------
//	MOG_Database.cpp
//	
//	
//--------------------------------------------------------------------------------
#include "stdafx.h"

#include "MOG_Report.h"
#include "MOG_ControllerSystem.h"
#include "MOG_Progress.h"
#include "MOG_DBErrorHandler.h"
#include "MOG_DBQueryBuilderAPI.h"
#include "MOG_DosUtils.h"

#include "MOG_DBAPI.h"


//Function to see if a column Exists in the Mog Database.  Pass it a tablename and a column name, return true if the column exists, false if it does not.
bool MOG_DBAPI::ColumnExists(String *tableName, String *columnName)
{
	bool bExists = false;
	SqlConnection *columnExistsConnection = MOG_DBAPI::GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());

		String *columnExistsCommand = String::Concat(S"EXEC sp_columns @table_name = '", tableName, S"' , @column_name = '", columnName , S"'");

		SqlDataReader *columnExistsReader = MOG_DBAPI::GetNewDataReader(columnExistsCommand, columnExistsConnection);
		//Try to read the data reader.  Will only work if a row is returned by sp_columns.
		try
		{
			if(columnExistsReader->Read())
			{
				bExists = true;
			}
		}
		catch(Exception *e)
		{
			MOG_Prompt::PromptMessage("Could not verify SQL database!", e->Message, e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
		}
		__finally
		{
			columnExistsConnection->Close();
		}
		return bExists;
}
bool MOG_DBAPI::TableExists(String *tableName)
{
	bool bExists = false;
	SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());

	try
	{
		String *command = String::Concat("SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE (TABLE_NAME = '", tableName,"')");

		SqlDataReader *myReader = MOG_DBAPI::GetNewDataReader(command, myConnection); 
		if( myReader->Read() )
		{
			bExists = true;
		}
		if(!bExists)
		{
			myReader->Close();
			String *command = String::Concat("SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE (TABLE_NAME = '", tableName->Replace(S"[",S"")->Replace(S"]",S""),"')");
			SqlDataReader *myReader2 = MOG_DBAPI::GetNewDataReader(command, myConnection); 
			if( myReader2->Read() )
			{
				bExists = true;
			}
		}
		myConnection->Close();
	}
	catch(Exception *e)
	{
		MOG_Prompt::PromptMessage("Could not query SQL database!", e->Message, e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
	}
	__finally
	{
		myConnection->Close();
	}
	return bExists;
}

String *MOG_DBAPI::QueryString(String *selectString, String *columnName)
{
	String *value = "";

	SqlConnection *myConnection = GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());

	SqlDataReader *myReader = GetNewDataReader(selectString, myConnection);;
	try
	{
		while(myReader->Read())
		{
			value = myReader->GetString(myReader->GetOrdinal(columnName))->Trim();
		}
	}
	catch(Exception *e)
	{
		MOG_Prompt::PromptMessage("Could not query SQL database!", e->Message, e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
	}
	__finally
	{
		myConnection->Close();
	}
	return value;
}


ArrayList *MOG_DBAPI::QueryStrings(String *selectString, String *columnName)
{
	ArrayList *values = new ArrayList();

	SqlConnection *myConnection = GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());

	SqlDataReader *myReader = GetNewDataReader(selectString, myConnection);
	try
	{
		while(myReader->Read())
		{
			values->Add(myReader->GetValue(myReader->GetOrdinal(columnName))->ToString()->Trim());
		}
	}
	catch(Exception *e)
	{
		MOG_Prompt::PromptMessage("Could not query SQL database!", e->Message, e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
	}
	__finally
	{
		myConnection->Close();
	}

	return values;
}

// Return an ArrayList of ArrayList that contain the result of String[], ordinals.
ArrayList *MOG_DBAPI::QueryStringsArray(String *selectString, String *columnNames[])
{
	ArrayList *values = new ArrayList();

	SqlConnection *myConnection = GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());

	SqlDataReader *myReader = GetNewDataReader(selectString, myConnection);
	try
	{
		while(myReader->Read())
		{
			String *ordinalValues[] = new String *[ columnNames->Length ];
			for( int i = 0; i < columnNames->Length; ++i )
			{
				ordinalValues[i] = myReader->GetString(myReader->GetOrdinal(columnNames[i]))->Trim();
			}
			values->Add(ordinalValues);
		}
	}
	catch(Exception *e)
	{
		MOG_Prompt::PromptMessage("Could not query SQL database!", e->Message, e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
	}
	__finally
	{
		myConnection->Close();
	}

	return values;
}

bool MOG_DBAPI::QueryExists(String *selectString)
{
	bool bExists = false;
	
	SqlConnection *myConnection = GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());
	try
	{
		SqlCommand *myCommand = new SqlCommand(selectString, myConnection);
		SqlDataReader *myReader = GetNewDataReader(selectString, myConnection);
		if( myReader->Read() )
		{
			bExists = true;
		}
	}
	catch(Exception *e)
	{
		MOG_Report::ReportMessage("Could not query SQL database!", e->Message, e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
	}
	__finally
	{
		myConnection->Close();
	}
	return bExists;
}

String *MOG_DBAPI::SafeStringRead(SqlDataReader *myReader, String *columnName)
{
	int ordinal = myReader->GetOrdinal(columnName);
	if( !myReader->IsDBNull(ordinal) )
	{
		return myReader->GetString(ordinal);
	}

	return "";
}

int MOG_DBAPI::SafeIntRead(SqlDataReader *myReader, String *columnName)
{
	int ordinal = myReader->GetOrdinal(columnName);
	if( !myReader->IsDBNull(ordinal) )
	{
		return myReader->GetInt32(ordinal);
	}

	return 0;
}

int MOG_DBAPI::QueryInt(String *selectString, String *columnName)
{
	int value = 0;

	// Query our int array
	ArrayList *values = QueryIntArray(selectString, columnName);
	if (values->Count > 0)
	{
		// Always return the last value as that one will be the most recently added item
		value = *dynamic_cast<__box int*>(values->Item[values->Count - 1]);
	}

	return value;
}

ArrayList *MOG_DBAPI::QueryIntArray(String *selectString, String *columnName)
{
	ArrayList *values = new ArrayList();

	SqlConnection *myConnection = GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());
	SqlDataReader *myReader = GetNewDataReader(selectString, myConnection);
	try
	{
		while(myReader->Read())
		{
			values->Add(__box(myReader->GetInt32(myReader->GetOrdinal(columnName))));
		}
	}
	catch(Exception *e)
	{
		String *message = String::Concat(	S"Could not get records from SQL database!\n",
											S"MOG SERVER: ", MOG_ControllerSystem::GetServerComputerName());
		MOG_Report::ReportMessage(message, e->Message, e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
	}
	__finally
	{
		myReader->Close();
		myConnection->Close();
	}

	return values;
}

String *MOG_DBAPI::FixSQLParameterString(String *theString)
{
	String *newString = theString;
	if (newString)
	{
		// Make sure the string hasn't already been fixed by checking for a double single quote?
		if (newString->IndexOf(S"''") == -1)
		{
			// Double up all the single quotes so the database can handle them
			newString = newString->Replace(S"'", S"''");
		}
	}

	return newString;
}


String *MOG_DBAPI::TruncateIfNecessary(String *testString, int length, bool truncateFront)
{
	if(testString)
	{
		if(testString->Length > length)
		{
			if(truncateFront)
			{
				int startChar = testString->Length - length;
				return testString->Substring(startChar);
			}
			else
			{
				return testString->Substring(0,length);
			}
		}
	}
	return testString;
}

bool MOG_DBAPI::ExecuteTransaction(ArrayList *queryList)
{
	return ExecuteTransaction(queryList, 100);
}

bool MOG_DBAPI::ExecuteTransaction(ArrayList *queryList, int groupSize)
{
	StringBuilder *transactionGroup = NULL;
	ArrayList *transactionGroups = new ArrayList();
	int thisGroupCount = 0;
	bool bFailed = false;

	// Make sure we have a valid queryList
	if(queryList)
	{
		// Process each transaction
		for(int i = 0; i < queryList->Count; i++)
		{
			// Check if this is the beginning of a new group
			if (thisGroupCount == 0)
			{
				// Create a new transactionString
				transactionGroup = new StringBuilder("");
				// Immediately add this transactionString to our transactionGroups list
				transactionGroups->Add(transactionGroup);
			}

			// Append this query to this group's transaction string
			transactionGroup->Append(String::Concat(__try_cast<String*> (queryList->Item[i]), S" \n"));
			// Count the queries in this group
			thisGroupCount++;
			// Check if we have exceeded our desired groupSize
			if (thisGroupCount > groupSize)
			{
				thisGroupCount = 0;
			}
		}


		if (!RunNonQuery(transactionGroups, MOG_ControllerSystem::GetDB()->GetConnectionString()))
		{
			bFailed = true;
		}
	}


	// Check if we failed
	if (bFailed)
	{
		return false;
	}
	return true;
}

SqlConnection *MOG_DBAPI::GetOpenSqlConnection(String *connectionString)
{
	MOG_SqlErrorHandler *SqlEH = NULL;
	//Create a SqlConnection.
	SqlConnection *retConnection = new SqlConnection(connectionString);
	//While the connection is not open attempt to open it.
	while( retConnection->State != 1 )
	{
		try
		{
			retConnection->Open();
		}
		catch(SqlException *e)
		{
			if(SqlEH)
			{
				SqlEH->HandleException(e);
			}
			else
			{
				SqlEH = new MOG_SqlErrorHandler(e);
			}
			if (SqlEH->IsRecoverable() == UNRECOVERABLE)
			{
				return NULL;
			}
		}
	}
	if(SqlEH)
	{
		SqlEH->CleanUpSqlErrorHandler();
	}
	//return an open Sql Connection.  This should be closed in the calling function.
	return retConnection;
}
SqlDataReader *MOG_DBAPI::GetNewDataReader ( String *commandString, SqlConnection *myConnection )
{
	MOG_SqlErrorHandler *SqlEH = NULL;
	//create a new reader.
	SqlDataReader *myReader = NULL;
	while( !myReader )
	{
		try
		{
			//verify that the connection we wer passed is open.
			while( myConnection->State != 1 )
			{
				//if it is not opened open it.
				myConnection->Open();
			}
			//create a new command
			SqlCommand *getReaderCommand = new SqlCommand( commandString, myConnection );
			//Attempt to build a reader.
			myReader = getReaderCommand->ExecuteReader();
		}
		catch(SqlException *e)
		{
			if(SqlEH)
			{
				SqlEH->HandleException(e);
			}
			else
			{
				SqlEH = new MOG_SqlErrorHandler(e);
			}
			if (SqlEH->IsRecoverable() == UNRECOVERABLE)
			{
				return NULL;
			}
		}
	}
	if(SqlEH)
	{
		SqlEH->CleanUpSqlErrorHandler();
	}
	return myReader;
}
String *MOG_DBAPI::GetNewSecularValueAsString( String *commandString, String *connectionString )
{
	MOG_SqlErrorHandler *SqlEH = NULL;
	//create a variable to manage the message box.
	//open a new connection.
	SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(connectionString);
	//new SqlConnection(connectionString);
	String *retString = NULL;
	try
	{
		while( !retString )
		{
			try
			{
				//verify that we have an open connection.
			while( myConnection->State != 1 )
			{
				//if not attempt to reopen the connection.
				myConnection->Open();
			}
			//create a secular command.
			SqlCommand *getSecularCommand = new SqlCommand( commandString, myConnection );
			//attempt to convert the return value to a string.
			retString = Convert::ToString(getSecularCommand->ExecuteScalar());
			}
			catch(SqlException *e)
			{
				if(SqlEH)
				{
					SqlEH->HandleException(e);
				}
				else
				{
					SqlEH = new MOG_SqlErrorHandler(e);
				}
				if (SqlEH->IsRecoverable() == UNRECOVERABLE)
				{
					return NULL;
				}
			}
		}
		myConnection->Close();
		if(SqlEH)
		{
			SqlEH->CleanUpSqlErrorHandler();
		}
	}
	catch (...)
	{
	}
	__finally
	{
		//close the connection.
		myConnection->Close();

		if(SqlEH)
		{
			SqlEH->CleanUpSqlErrorHandler();
		}
	}
	return retString;
}
int MOG_DBAPI::GetNewSecularValueAsInt( String *commandString , String *connectionString )
{
	MOG_SqlErrorHandler *SqlEH = NULL;
	//create a open SqlConnection.
	SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(connectionString);
	//SqlConnection *myConnection = new SqlConnection(connectionString);
	String *retString = NULL;
	//If we don't have a return string yet.
	try
	{
	while( !retString )
	{
		try
		{
		//Verify that our connection is still open.
		while( myConnection->State != 1 )
		{
			myConnection->Open();
		}
		//create a command to return a secular value.
		SqlCommand *getSecularCommand = new SqlCommand( commandString, myConnection );
		//attempt to convert the secular value in to a string and return the value.		
		retString = Convert::ToString(getSecularCommand->ExecuteScalar());
		}
		catch(SqlException *e)
		{
			if(SqlEH)
			{
				SqlEH->HandleException(e);
			}
			else
			{
				SqlEH = new MOG_SqlErrorHandler(e);
			}
			if (SqlEH->IsRecoverable() == UNRECOVERABLE)
			{
				return 0;
			}
		}
	}
	myConnection->Close();
	if(SqlEH)
	{
		SqlEH->CleanUpSqlErrorHandler();
	}
	try
	{
		return Convert::ToInt32(retString);
	}
	catch(...)
	{
		return 0;
	}
	}
	catch (...)
	{
	}
	__finally
	{
		//close the connection.
		myConnection->Close();

		if(SqlEH)
		{
			SqlEH->CleanUpSqlErrorHandler();
		}
	}
	return 0;
}
bool MOG_DBAPI::RunNonQuery( String *commandString, String *connectionString )
{
	MOG_SqlErrorHandler *SqlEH = NULL;
	//create the connection String.
	SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(connectionString);
	//SqlConnection *myConnection = new SqlConnection(connectionString);

	try
	{
		//While we have a default value here keep trying.
		while (true)
		{
			try
			{
				//Attempt to connect to the Sql Server
				while( myConnection->State != 1 )
				{
					myConnection->Open();
					//If we have hit a error dialog before reset the error 
				}
				//Create a SqlCommand based on the passed Query.
				SqlCommand *nonQueryCommand = new SqlCommand( commandString, myConnection );
				//Attempt to run the query.
				int affectedRows = nonQueryCommand->ExecuteNonQuery();
				if (affectedRows != -1)
				{
					return true;
				}
				break;
			}
			catch(SqlException *e)
			{
				if(SqlEH)
				{
					SqlEH->HandleException(e);
				}
				else
				{
					SqlEH = new MOG_SqlErrorHandler(e);
				}

				if (SqlEH->IsRecoverable() == UNRECOVERABLE)
				{
					return false;
				}
				if(SqlEH->IsRecoverable() == IGNORABLE)
				{
					return true;
				}
			}
		}
	}
	catch (...)
	{
	}
	__finally
	{
		//close the connection.
		myConnection->Close();

		if(SqlEH)
		{
			SqlEH->CleanUpSqlErrorHandler();
		}
	}

	return true;
}
bool MOG_DBAPI::RunNonQuery( ArrayList *transactionGroups, String *connectionString )
{
	MOG_SqlErrorHandler *SqlEH = NULL;
	//create the connection String.
	SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(connectionString);
	//SqlConnection *myConnection = new SqlConnection(connectionString);

	try
	{
		//While we have a default value here keep trying.
		while (true)
		{
			try
			{
				//Attempt to connect to the Sql Server
				while( myConnection->State != 1 )
				{
					myConnection->Open();
					//If we have hit a error dialog before reset the error 
				}

				// Begin the transaction
				SqlCommand *nonQueryCommand = new SqlCommand( S"BEGIN TRANSACTION", myConnection );
				nonQueryCommand->ExecuteNonQuery();

				// Process each transaction group
				for (int i = 0; i < transactionGroups->Count; i++)
				{
					StringBuilder *transactionGroup = __try_cast<StringBuilder *>(transactionGroups->Item[i]);

					//Create a SqlCommand based on this transactionGroup
					nonQueryCommand = new SqlCommand( transactionGroup->ToString(), myConnection );
					nonQueryCommand->ExecuteNonQuery();
				}

				// End the transaction
				nonQueryCommand = new SqlCommand( S"COMMIT TRANSACTION", myConnection );
				nonQueryCommand->ExecuteNonQuery();

				break;
			}
			catch(SqlException *e)
			{
				if(SqlEH)
				{
					SqlEH->HandleException(e);
				}
				else
				{
					SqlEH = new MOG_SqlErrorHandler(e);
				}

				if (SqlEH->IsRecoverable() == UNRECOVERABLE)
				{
					return false;
				}
				if(SqlEH->IsRecoverable() == IGNORABLE)
				{
					return true;
				}
			}
		}
	}
	catch (...)
	{
	}
	__finally
	{
		//close the connection.
		myConnection->Close();

		if(SqlEH)
		{
			SqlEH->CleanUpSqlErrorHandler();
		}
	}

	return true;
}

bool MOG_DBAPI::DatabaseExists(String *connectionString, String *databaseName)
{
	SqlConnection *myConn = GetOpenSqlConnection(connectionString);
	String *dbListQuery = "exec sp_databases";
	SqlDataReader *myReader = GetNewDataReader(dbListQuery,myConn);
	try
	{
		while(myReader->Read())
		{
			if(String::Compare(myReader->GetString(myReader->GetOrdinal(S"DATABASE_NAME")), databaseName, true) == 0)
			{
				return true;
			}
		}
	}
	catch(...)
	{
	
	}
	__finally
	{
		if(myConn && myConn->State == 1)
		{
			myConn->Close();
		}
	}
	return false;
}
bool MOG_DBAPI::CreateDatabase(String *connectionString, String *databaseName)
{
	String *createDBCmd = MOG_DBQueryBuilderAPI::MOG_DatabaseManagementQueries::Query_CreateMogDatabase(databaseName, GetModelDatabaseFilePath(connectionString), GetModelDatabaseFilePath(connectionString));
	RunNonQuery(createDBCmd, connectionString);
	return true;
}


bool MOG_DBAPI::TestServerConnection(String * connectionString)
{
	try
	{
		//open a connection and close it if we fail return false
		SqlConnection *testConnection = new SqlConnection(connectionString);
		testConnection->Open();
		testConnection->Close();
	}
	catch(...)
	{
		return false;
	}
	return true;
}

bool MOG_DBAPI::TestDatabaseConnection(String *connectionString, String *databaseName)
{
	try
	{
		String *ourConnectionString = "";
		//if we dont have a database name in our connection string add it.
		if(connectionString->Length && databaseName->Length)
		{
			if((connectionString->ToUpper()->IndexOf(S"INITIAL CATALOG") != -1) || 
			(connectionString->ToUpper()->IndexOf(S"DATABASE") != -1)) 
			{
				//if we have a database in the connection string use the one we have
				ourConnectionString = connectionString;
			}
			else 
			{
				ourConnectionString = String::Concat(connectionString, S"INITIAL CATALOG = ", databaseName);				
			}
		}
		//create a sql connection
		SqlConnection *testConnection = new SqlConnection(ourConnectionString);
		//open it
		testConnection->Open();
		//run a command to make sure we have perms in this database
		String *commandToCheckPerms = S"CREATE TABLE MOG_DELETEMEIDONOTBELONGHERE (ID int IDENTITY (1,1) NOT NULL);";
		SqlCommand * mycmd= new SqlCommand(commandToCheckPerms, testConnection);
		int rc = mycmd->ExecuteNonQuery();
		String *commandToCheckPermsTwo = S"DROP TABLE MOG_DELETEMEIDONOTBELONGHERE";
		SqlCommand * mycmd2= new SqlCommand(commandToCheckPermsTwo, testConnection);
		rc = mycmd2->ExecuteNonQuery();
		
		if(testConnection->State == ConnectionState::Open)
		{
			testConnection->Close();	
			return true;
		}
	}
	catch(Exception *e)
	{
		e->ToString();
		return false;
	}
	return true;
}

String *MOG_DBAPI::GetModelDatabaseFilePath(String *connectionString)
{
	String *commandToGetFilePath = MOG_DBQueryBuilderAPI::MOG_DatabaseManagementQueries::Query_GetDefaultDataFilePath(S"model");
	String *modelDatabaseFile = MOG_DBAPI::GetNewSecularValueAsString(commandToGetFilePath, connectionString);
	return DosUtils::PathGetDirectoryPath(modelDatabaseFile);
}

String *MOG_DBAPI::GetModelLogFilePath(String *connectionString)
{
	String *commandToGetFilePath = MOG_DBQueryBuilderAPI::MOG_DatabaseManagementQueries::Query_GetDefaultLogFilePath(S"model");
	String *modelLogFile = MOG_DBAPI::GetNewSecularValueAsString(commandToGetFilePath, connectionString);
	return DosUtils::PathGetDirectoryPath(modelLogFile);
}

