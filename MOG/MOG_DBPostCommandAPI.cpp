//--------------------------------------------------------------------------------
//	MOG_DBPost.cpp
//	
//	
//--------------------------------------------------------------------------------
#include "stdafx.h"


#include "MOG_ControllerSystem.h"
#include "MOG_DBPostCommandAPI.h"

using namespace System::ComponentModel;
using namespace MOG_CoreControls;


MOG_DBPostInfo *MOG_DBPostCommandAPI::SchedulePost(String *assetName, String *assetVersion, String *jobLabel, String *branchName, String *createdBy, String *blessedBy)
{
	return SQLCreatePost(assetName, assetVersion, jobLabel, branchName, createdBy, blessedBy);
}


ArrayList *MOG_DBPostCommandAPI::GetScheduledPosts(String *branchName)
{
	return GetScheduledPosts(S"", "", branchName);
}


ArrayList *MOG_DBPostCommandAPI::GetScheduledPosts(String *jobLabel, String *branchName)
{
	return GetScheduledPosts(S"", jobLabel, branchName);
}


ArrayList *MOG_DBPostCommandAPI::GetScheduledPosts(String *assetName, String *jobLabel, String *branchName)
{
	String *selectString = String::Concat(S"SELECT * FROM ", MOG_ControllerSystem::GetDB()->GetPostCommandsTable());

	// Build the appropriate where clause
	String *whereClause = "";
	// Was a job label specified?
	if (jobLabel->Length)
	{
		// Check if we need to append an 'AND'?
		if (whereClause->Length)
		{
			whereClause = String::Concat(whereClause, S" AND ");
		}
		whereClause = String::Concat(whereClause, S"Label='", MOG_DBAPI::FixSQLParameterString(jobLabel), S"'");
	}
	// Was a branch specified?
	if (branchName->Length)
	{
		// Check if we need to append an 'AND'?
		if (whereClause->Length)
		{
			whereClause = String::Concat(whereClause, S" AND ");
		}
		whereClause = String::Concat(whereClause, S" BranchName='", MOG_DBAPI::FixSQLParameterString(branchName), S"'");
	}
	// Was an asset specified?
	if (assetName->Length)
	{
		// Check if we need to append an 'AND'?
		if (whereClause->Length)
		{
			whereClause = String::Concat(whereClause, S" AND ");
		}
		whereClause = String::Concat(whereClause, S" AssetName='", MOG_DBAPI::FixSQLParameterString(assetName), S"'");
	}
	// Check if we created a where clause?
	if (whereClause->Length)
	{
		// Add on the optional where clause
		selectString = String::Concat(selectString, S" WHERE ", whereClause);
	}

	// Always order by ID
	selectString = String::Concat(selectString, S" ORDER BY ID");

	return QueryPosts(selectString);
}


bool MOG_DBPostCommandAPI::RemovePost(MOG_DBPostInfo *post)
{
	// Make sure we have a specified post?
	if (post)
	{
		return RemovePost(post->mID);
	}

	return false;
}


bool MOG_DBPostCommandAPI::RemovePost(MOG_Filename *blessedAssetFilename)
{
	return RemovePost(blessedAssetFilename, S"", S"");
}

bool MOG_DBPostCommandAPI::RemovePost(MOG_Filename *blessedAssetFilename, String *branchName)
{
	return RemovePost(blessedAssetFilename, S"", branchName);
}

bool MOG_DBPostCommandAPI::RemovePost(MOG_Filename *blessedAssetFilename, String *jobLabel, String *branchName)
{
	String *deleteCmd = String::Concat(	S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetPostCommandsTable(), 
										S" WHERE ");
	String *whereCmd = "";
	if (blessedAssetFilename)
	{
		if (blessedAssetFilename->GetAssetFullName()->Length)
		{
			if (whereCmd->Length) whereCmd = String::Concat(whereCmd, S" AND ");
			whereCmd = String::Concat(whereCmd, S" AssetName='", MOG_DBAPI::FixSQLParameterString(blessedAssetFilename->GetAssetFullName()), S"'");
		}
		if (blessedAssetFilename->GetVersionTimeStamp()->Length)
		{
			if (whereCmd->Length) whereCmd = String::Concat(whereCmd, S" AND ");
			whereCmd = String::Concat(whereCmd, S" AssetVersionTimestamp='", blessedAssetFilename->GetVersionTimeStamp(), S"'");
		}
	}
	if (jobLabel->Length)
	{
		if (whereCmd->Length) whereCmd = String::Concat(whereCmd, S" AND ");
		whereCmd = String::Concat(whereCmd, S" Label='", MOG_DBAPI::FixSQLParameterString(jobLabel), S"'");
	}
	if (branchName->Length)
	{
		if (whereCmd->Length) whereCmd = String::Concat(whereCmd, S" AND ");
		whereCmd = String::Concat(whereCmd, S" BranchName='", MOG_DBAPI::FixSQLParameterString(branchName), S"'");
	}
	deleteCmd = String::Concat(deleteCmd, whereCmd);
		
	return MOG_DBAPI::RunNonQuery(deleteCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
}


bool MOG_DBPostCommandAPI::RemovePost(UInt32 commandId)
{
	String *deleteCmd = String::Concat(S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetPostCommandsTable(), S" WHERE id='", commandId.ToString(), S"'");
		
	return MOG_DBAPI::RunNonQuery(deleteCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());
}


bool MOG_DBPostCommandAPI::RemovePosts(ArrayList *posts)
{
	bool bSuccess = false;

	// Make sure we have something to do?
	if (posts && posts->Count)
	{
		// Remove the processed post commands
		String* message = String::Concat(S"Please wait while the old post commands are removed.\n", S"This may take a few minutes.");
		ProgressDialog* progress = new ProgressDialog("Removing pending post commands", message, new DoWorkEventHandler(NULL, RemovePosts_Worker), posts, true);
		if (progress->ShowDialog() == DialogResult::OK)
		{
			bSuccess = *dynamic_cast<__box bool*>(progress->WorkerResult);
		}
	}

	return bSuccess;
}

void MOG_DBPostCommandAPI::RemovePosts_Worker(Object* sender, DoWorkEventArgs* e)
{
	BackgroundWorker* worker = dynamic_cast<BackgroundWorker*>(sender);
	ArrayList* posts = dynamic_cast<ArrayList*>(e->Argument);

	e->Result = __box(true);
	
	ArrayList *transactionItems = new ArrayList();

	// Walk through the specified posts
	for (int p = 0; p < posts->Count; p++)
	{
		MOG_DBPostInfo *post = __try_cast<MOG_DBPostInfo *>(posts->Item[p]);

		if (worker != NULL)
		{
			worker->ReportProgress(p * 100 / posts->Count);
		}

		// Create the delete command for this post
		String *deleteCmd = String::Concat(S"DELETE FROM ", MOG_ControllerSystem::GetDB()->GetPostCommandsTable(), S" WHERE id='", post->mID.ToString(), S"'");
		// Add this to our transaction list
		transactionItems->Add( deleteCmd );
	}

	// Execute the entire transaction
	if (!MOG_DBAPI::ExecuteTransaction(transactionItems))
	{
		e->Result = __box(false);
	}
}


bool MOG_DBPostCommandAPI::RemoveStalePostCommands(ArrayList *recentlyRemovedPostCommands, String *branchName)
{
	bool bFailed = false;
	ArrayList *stalePostCommands = new ArrayList();

	// Make sure we have something to do?
	if (recentlyRemovedPostCommands && recentlyRemovedPostCommands->Count)
	{
		// Get all the remaining post commands for this branch
		ArrayList *remainingPostCommands = MOG_DBPostCommandAPI::GetScheduledPosts(branchName);
		if (remainingPostCommands && remainingPostCommands->Count)
		{
			// Walk through the specified posts
			for (int p = 0; p < recentlyRemovedPostCommands->Count; p++)
			{
				MOG_DBPostInfo *recentlyRemovedPostCommand = __try_cast<MOG_DBPostInfo *>(recentlyRemovedPostCommands->Item[p]);

				// Walk through the remaining posts
				for (int r = 0; r < remainingPostCommands->Count; r++)
				{
					MOG_DBPostInfo *remainingPostCommand = __try_cast<MOG_DBPostInfo *>(remainingPostCommands->Item[r]);

					// Check if this remaining post matches? and
					// Check if it is older?
					if (String::Compare(remainingPostCommand->mAssetName, recentlyRemovedPostCommand->mAssetName, true) == 0 &&
						remainingPostCommand->mID < recentlyRemovedPostCommand->mID)
					{
						stalePostCommands->Add(remainingPostCommand);
					}
				}
			}
		}
	}

	// Check if we have stalePostCommands pending removal?
	if (stalePostCommands->Count > 0)
	{
		// Remove all the stale post commands
		if (!RemovePosts(stalePostCommands))
		{
			bFailed = true;
		}
	}

	// Check if we ever failed?
	if (!bFailed)
	{
		return true;
	}

	return false;
}


MOG_DBPostInfo *MOG_DBPostCommandAPI::SQLCreatePost(String *assetName, String *assetVersion, String *jobLabel, String *branchName, String *createdBy, String *blessedBy)
{
	// Build a SQL Insert statement string for all the input-form
	String *insertCmd = String::Concat(S"INSERT INTO ", MOG_ControllerSystem::GetDB()->GetPostCommandsTable(),
		S" ( AssetName, AssetVersionTimestamp, Label, BranchName, CreatedBy, BlessedBy) VALUES",
		S" ('",MOG_DBAPI::FixSQLParameterString(assetName) , S"', '",MOG_DBAPI::FixSQLParameterString(assetVersion) , S"', '",MOG_DBAPI::FixSQLParameterString(jobLabel) , S"', '", MOG_DBAPI::FixSQLParameterString(branchName), S"', '", MOG_DBAPI::FixSQLParameterString(createdBy), S"', '", MOG_DBAPI::FixSQLParameterString(blessedBy), S"')");

	MOG_DBAPI::RunNonQuery(insertCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());

	// Create a MOG_DBPostInfo to return back
	MOG_DBPostInfo *postInfo = new MOG_DBPostInfo;
	postInfo->mID = 0;
	postInfo->mAssetName = assetName;
	postInfo->mAssetVersion = assetVersion;
	postInfo->mLabel = jobLabel;
	postInfo->mBranchName = branchName;
	postInfo->mCreatedBy = createdBy;
	postInfo->mBlessedBy = blessedBy;
	return postInfo;
}


MOG_DBPostInfo *MOG_DBPostCommandAPI::QueryPost(String *selectString)
{
	SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());

	MOG_DBPostInfo *postInfo = NULL;

	try
	{
		SqlDataReader *myReader = MOG_DBAPI::GetNewDataReader(selectString ,myConnection);
		while(myReader->Read())
		{
			postInfo = SQLReadPost(myReader);
			break;
		}
		myReader->Close();
		
	}
	catch(Exception *e)
	{
		String *message = String::Concat(	S"Could not get records from SQL database!\n",
											S"MOG SERVER: ", MOG_ControllerSystem::GetServerComputerName());
		MOG_Report::ReportMessage(message, e->Message, e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
		return NULL;
	}
	__finally
	{
		myConnection->Close();
	}

	return postInfo;
}


ArrayList *MOG_DBPostCommandAPI::QueryPosts(String *selectString)
{
	SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());
	ArrayList *posts = NULL;

	try
	{
		posts = new ArrayList();

		SqlDataReader *myReader = MOG_DBAPI::GetNewDataReader(selectString, myConnection);
		while(myReader->Read())
		{
			posts->Add(SQLReadPost(myReader));
		}
		myReader->Close();
	}
	catch(Exception *e)
	{
		String *message = String::Concat(	S"Could not get records from SQL database!\n",
											S"MOG SERVER: ", MOG_ControllerSystem::GetServerComputerName());
		MOG_Report::ReportMessage(message, e->Message, e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
		return NULL;
	}
	__finally
	{
		myConnection->Close();
	}

	return posts;
}


MOG_DBPostInfo *MOG_DBPostCommandAPI::SQLReadPost(SqlDataReader *myReader)
{
	MOG_DBPostInfo *postInfo = new MOG_DBPostInfo;

	postInfo->mID = myReader->GetInt32(myReader->GetOrdinal(S"ID"));
	postInfo->mAssetName = myReader->GetString(myReader->GetOrdinal(S"AssetName"))->Trim();
	postInfo->mAssetVersion = myReader->GetString(myReader->GetOrdinal(S"AssetVersionTimestamp"))->Trim();
	postInfo->mLabel = myReader->GetString(myReader->GetOrdinal(S"Label"))->Trim();
	postInfo->mBranchName = myReader->GetString(myReader->GetOrdinal(S"BranchName"))->Trim();
	postInfo->mCreatedBy = myReader->GetString(myReader->GetOrdinal(S"CreatedBy"))->Trim();
	postInfo->mBlessedBy = myReader->GetString(myReader->GetOrdinal(S"BlessedBy"))->Trim();

	return postInfo;
}


bool MOG_DBPostCommandAPI::SQLWritePost(String *assetName, String *assetVersion, String *jobLabel, String *branchName, String *createdBy, String *blessedBy)
{
	String *updateCmd = String::Concat(S"UPDATE ", MOG_ControllerSystem::GetDB()->GetPostCommandsTable(), S" SET");

	// Variables
	updateCmd = String::Concat(updateCmd, S" AssetName='", MOG_DBAPI::FixSQLParameterString(assetName), S"',");
	updateCmd = String::Concat(updateCmd, S" AssetVersionTimestamp='", MOG_DBAPI::FixSQLParameterString(assetVersion), S"',");
	updateCmd = String::Concat(updateCmd, S" Label='", MOG_DBAPI::FixSQLParameterString(jobLabel), S"',");
	updateCmd = String::Concat(updateCmd, S" BranchName='", MOG_DBAPI::FixSQLParameterString(branchName), S"',");
	updateCmd = String::Concat(updateCmd, S" CreatedBy='", MOG_DBAPI::FixSQLParameterString(createdBy), S"',");
	updateCmd = String::Concat(updateCmd, S" BlessedBy='", MOG_DBAPI::FixSQLParameterString(blessedBy), S"',");

	// Where clause
	updateCmd = String::Concat(updateCmd, S" WHERE AssetName='", MOG_DBAPI::FixSQLParameterString(assetName), S"', BranchName='", MOG_DBAPI::FixSQLParameterString(branchName), S"'");

	return MOG_DBAPI::RunNonQuery(updateCmd, MOG_ControllerSystem::GetDB()->GetConnectionString());

}


