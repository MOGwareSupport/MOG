//--------------------------------------------------------------------------------
//	MOG_DBPost.cpp
//	
//	
//--------------------------------------------------------------------------------
#include "stdafx.h"

#include "MOG_DBPostAPI.h"



MOG_DBPostInfo *MOG_DBPostAPI::SchedulePost(String *assetName, String *version, String *groupID, String *branchName, String *createdBy, String *blessedBy)
{
	return SQLCreatePost(assetName, version, groupID, branchName, createdBy, blessedBy);
}


ArrayList *MOG_DBPostAPI::GetScheduledPosts(String *groupID, String *branchName)
{
	String *selectString = String::Concat(S"SELECT * from Post WHERE GroupID='", groupID, S"' and BranchName='", branchName, S"'");

	return QueryPosts(selectString);
}


bool MOG_DBPostAPI::RemovePost(MOG_DBPostInfo *post)
{
	// Early out if the post is not valid
	if (!post)
	{
		return false;
	}

	String *deleteCmd = String::Concat(S"DELETE from Post WHERE id='", post->mID.ToString(), S"'");
	
	SqlConnection *myConnection = new SqlConnection(mConnectionString);
	SqlCommand *myCommand = new SqlCommand(deleteCmd, myConnection);

	myCommand->Connection->Open();

	try
	{
		myCommand->ExecuteNonQuery();
	}
	catch (SqlException *e) 
	{
		MOG_REPORT::ShowMessageBox("Could not create record in SQL database!", e->ToString(), MessageBoxButtons::OK);
		return false;
	}

	myCommand->Connection->Close();

	return true;
}


bool MOG_DBPostAPI::RemovePosts(ArrayList *posts)
{
	// Early out if the post is not valid
	if (!posts || !posts->Count)
	{
		return false;
	}

	// Walk through the specified posts
	for (int p = 0; p < posts->Count; p++)
	{
		MOG_DBPostInfo *post = __try_cast<MOG_DBPostInfo *>(posts->Item[p]);
		RemovePost(post);
	}

	return true;
}


MOG_DBPostInfo *MOG_DBPostAPI::SQLCreatePost(String *assetName, String *version, String *groupID, String *branchName, String *createdBy, String *blessedBy)
{
	// Build a SQL Insert statement string for all the input-form
	String *insertCmd = "insert into Post values (@AssetName,@VersionTimeStamp,@GroupID,@BranchName,@CreatedBy,@BlessedBy)";

	// Initialize the SqlCommand with the new SQL string 
	// and the connection information.
	SqlConnection *myConnection = new SqlConnection(mConnectionString);
	SqlCommand *myCommand = new SqlCommand(insertCmd, myConnection);

	// Create new parameters for the SqlCommand object and initialize them to the specified TaskInfo
	myCommand->Parameters->Add(new SqlParameter("@AssetName",		SqlDbType::VarChar, 250))->Value	= assetName;
	myCommand->Parameters->Add(new SqlParameter("@VersionTimeStamp",SqlDbType::VarChar, 12))->Value		= version;
	myCommand->Parameters->Add(new SqlParameter("@GroupID",			SqlDbType::VarChar, 12))->Value		= groupID;
	myCommand->Parameters->Add(new SqlParameter("@BranchName",		SqlDbType::VarChar, 50))->Value		= branchName;
	myCommand->Parameters->Add(new SqlParameter("@CreatedBy",		SqlDbType::VarChar, 30))->Value		= createdBy;
	myCommand->Parameters->Add(new SqlParameter("@BlessedBy",		SqlDbType::VarChar, 30))->Value		= blessedBy;
	myCommand->Connection->Open();

	try
	{
		myCommand->ExecuteNonQuery();
	}
	catch (SqlException *e) 
	{
		MOG_REPORT::ShowMessageBox("Could not create record in SQL database!", e->ToString(), MessageBoxButtons::OK);
		return NULL;
	}
	// Close the connection
	myCommand->Connection->Close();

	// Create a MOG_DBPostInfo to return back
	MOG_DBPostInfo *postInfo = new MOG_DBPostInfo;
	postInfo->mID = 0;
	postInfo->mAssetName = assetName;
	postInfo->mVersion = version;
	postInfo->mGroupID = groupID;
	postInfo->mBranchName = branchName;
	postInfo->mCreatedBy = createdBy;
	postInfo->mBlessedBy = blessedBy;
	return postInfo;
}


MOG_DBPostInfo *MOG_DBPostAPI::QueryPost(String *selectString)
{
	// We cannot continue if the server is absent
	if (!mIsConnected) return NULL;

	SqlConnection *myConnection = new SqlConnection(mConnectionString);
	SqlCommand	*myCommand = new SqlCommand(selectString, myConnection);
	MOG_DBPostInfo *postInfo = new MOG_DBPostInfo;

	try
	{
		myConnection->Open();

		SqlDataReader *myReader = myCommand->ExecuteReader();
		while(myReader->Read())
		{
			postInfo = SQLReadPost(myReader);
			break;
		}
		myReader->Close();
		myConnection->Close();
	}
	catch(Exception *e)
	{
		MOG_REPORT::ShowMessageBox("Could not get records from SQL database!", e->ToString(), MessageBoxButtons::OK);
		return NULL;
	}

	return postInfo;
}


ArrayList *MOG_DBPostAPI::QueryPosts(String *selectString)
{
	// We cannot continue if the server is absent
	if (!mIsConnected) return NULL;

	SqlConnection *myConnection = new SqlConnection(mConnectionString);
	SqlCommand	*myCommand = new SqlCommand(selectString, myConnection);
	ArrayList *posts = new ArrayList();

	try
	{
		myConnection->Open();

		SqlDataReader *myReader = myCommand->ExecuteReader();
		while(myReader->Read())
		{
			posts->Add(SQLReadPost(myReader));
		}
		myReader->Close();
		myConnection->Close();
	}
	catch(Exception *e)
	{
		MOG_REPORT::ShowMessageBox("Could not get records from SQL database!", e->ToString(), MessageBoxButtons::OK);
		return NULL;
	}

	return posts;
}


MOG_DBPostInfo *MOG_DBPostAPI::SQLReadPost(SqlDataReader *myReader)
{
	MOG_DBPostInfo *postInfo = new MOG_DBPostInfo;

	postInfo->mID = myReader->GetInt32(myReader->GetOrdinal(S"ID"));
	postInfo->mAssetName = myReader->GetString(myReader->GetOrdinal(S"AssetName"))->Trim();
	postInfo->mVersion = myReader->GetString(myReader->GetOrdinal(S"VersionTimeStamp"))->Trim();
	postInfo->mGroupID = myReader->GetString(myReader->GetOrdinal(S"GroupID"))->Trim();
	postInfo->mBranchName = myReader->GetString(myReader->GetOrdinal(S"BranchName"))->Trim();
	postInfo->mCreatedBy = myReader->GetString(myReader->GetOrdinal(S"CreatedBy"))->Trim();
	postInfo->mBlessedBy = myReader->GetString(myReader->GetOrdinal(S"BlessedBy"))->Trim();

	return postInfo;
}


bool MOG_DBPostAPI::SQLWritePost(String *assetName, String *version, String *groupID, String *branchName, String *createdBy, String *blessedBy)
{
	String *updateCmd = "UPDATE Assets SET";

	// Variables
	updateCmd = String::Concat(updateCmd, S" AssetName='", assetName, S"',");
	updateCmd = String::Concat(updateCmd, S" VersionTimeStamp='", version, S"',");
	updateCmd = String::Concat(updateCmd, S" GroupID='", groupID, S"',");
	updateCmd = String::Concat(updateCmd, S" BranchName='", branchName, S"',");
	updateCmd = String::Concat(updateCmd, S" CreatedBy='", createdBy, S"',");
	updateCmd = String::Concat(updateCmd, S" BlessedBy='", blessedBy, S"',");

	// Where clause
	updateCmd = String::Concat(updateCmd, S" WHERE AssetName='", assetName, S"', BranchName='", branchName, S"'");

	SqlConnection *myConnection = new SqlConnection(mConnectionString);
	SqlCommand *myCommand = new SqlCommand(updateCmd, myConnection);

	// Connect to the database and update the information.
	myCommand->Connection->Open();
	
	// Test  whether the data was updated and  display the 
	//appropriate message to the user.
	try 
	{
		myCommand->ExecuteNonQuery();
	}
	catch (SqlException *e) 
	{
		MOG_REPORT::ShowMessageBox("Could not update record to SQL database!", e->ToString(), MessageBoxButtons::OK);				
		return false;
	}
	// Close the connection.
	myCommand->Connection->Close();

	return true;
}


