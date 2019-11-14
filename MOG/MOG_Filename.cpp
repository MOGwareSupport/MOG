//--------------------------------------------------------------------------------
//	MOG_Filename.cpp
//	
//	
//--------------------------------------------------------------------------------

#include "StdAfx.h"

#include <string.h>
#include <limits>

#include "MOG_Define.h"
#include "MOG_Main.h"
#include "MOG_StringUtils.h"
#include "MOG_Time.h"
#include "MOG_Tokens.h"
#include "MOG_ControllerSystem.h"
#include "MOG_ControllerProject.h"
#include "MOG_ControllerInbox.h"
#include "MOG_DBAssetAPI.h"
#include "MOG_DBInboxAPI.h"

#include "MOG_Filename.h"

using namespace System::Text::RegularExpressions;

MOG_Filename::MOG_Filename(void)
{
	ClearFilename();
}


MOG_Filename::MOG_Filename(MOG_Filename *filename)
{
	SetFilename(filename);
}


MOG_Filename::MOG_Filename(String *filename)
{
	SetFilename(filename);
}


MOG_Filename::MOG_Filename(char *pFilename)
{
	SetFilename(pFilename);
}


MOG_Filename::MOG_Filename(String *userName, String *boxName, String *groupTree, String *assetFullName)
{
	String *temp = String::Concat(MOG_ControllerProject::GetProject()->GetProjectUsersPath(), S"\\");

	if (userName->Length)
	{
		temp = String::Concat(temp, userName, S"\\");
	}
	if (boxName->Length)
	{
		temp = String::Concat(temp, boxName, S"\\");
	}
	if (groupTree->Length)
	{
		temp = String::Concat(temp, groupTree, S"\\");
	}
	if (assetFullName->Length)
	{
		temp = String::Concat(temp, assetFullName);
	}

	SetFilename(temp);
}

MOG_Filename::MOG_Filename(String *assetFullName, String *revisionTimeStamp)
{
	// Make sure to initialize our old variables
	ClearFilename();

	// Parse this as the Asset's name
	if (ParseAssetName(assetFullName))
	{
		// Retain the original string for this asset
		mOriginalFilename = assetFullName;

		// Force the specified revision
		mVersionTimeStamp = revisionTimeStamp;
		if (mVersionTimeStamp->Length)
		{
			// Append mVersionTimeStamp onto the asset in mOriginalFilename
			mOriginalFilename = String::Concat(mOriginalFilename, S"\\R.", mVersionTimeStamp);;
		}

		// There is no path to be parsed in this constructor
		bAlreadyParsedPath = true;
	}
}

void MOG_Filename::SetFilename(String *filename)
{
	// Make sure to initialize our old variables
	ClearFilename();

	// Make sure it is valid
	if (filename != NULL)
	{
		// Retain the original string
		mOriginalFilename = filename;

		// Always Assume this is an unknown type until we know better
		mType = MOG_FILENAME_Unknown;
	}
}


void MOG_Filename::SetFilename(MOG_Filename *pFilename)
{
	SetFilename(pFilename->GetOriginalFilename());
}


// JohnRen - I think we should remove this routine from MOG_Filename because MOG_Time already can do this
String *MOG_Filename::GetVersionTimeStampString(String *format)
{
	// Make sure we have parsed the path
	ParsePath();

	String *versionString = S"";

	// Construct a new time
	MOG_Time *time = new MOG_Time;
	if (time->SetTimeStamp(mVersionTimeStamp))
	{
		// Check if we were not supplied a format?
		if (format->Length == 0)
		{
			// Default to a nice standard format
			format = String::Concat(TOKEN_Month_1, S"/", TOKEN_Day_1, S"/", TOKEN_Year_4, S" ", TOKEN_Hour_1, S":", TOKEN_Minute_2, S" ", TOKEN_AMPM);
		}

		// Construct a new version time stamp and load it with this newVersionName...then beautify the name
		versionString = time->FormatString(format);
	}

	return versionString;
}


void MOG_Filename::ClearFilename(void)
{
	// Clean out all old variables
	bAlreadyParsedPath = false;

	mType = MOG_FILENAME_None;

	mOriginalFilename = S"";

	mDrive = NULL;
	mPath = NULL;
	mFilename = NULL;
	mExtension = NULL;

	mProjectName = S"";
	mProjectPath = S"";
	mRepositoryPath = S"";
	mUserName = S"";
	mUserPath = S"";
	mBoxName = S"";
	mBoxPath = S"";

	mGroupName = S"";
	mGroupTree = S"";
	mIsWithinGroup = false;

	mAssetFilesName = S"";
	mAssetFilesScope = S"";

	mVersionTimeStamp = S"";
	mDeletedTimeStamp = S"";

	mAssetOriginalFullName = S"";
	mAssetOriginalPath = S"";
	mAssetClassification = S"";
	mAssetEncodedClassification = S"";
	mAssetEncodedName = S"";
	mAssetName = S"";
	mAssetPlatform = S"";
	mAssetLabel = S"";

	mFailedPreviousEncodedClassificationQuery = false;
	mFailedPreviousEncodedNameQuery = false;

	mIsWithinRepository = false;
	mIsBlessed = false;
	mIsArchived = false;

	mIsWithinInboxes = false;
	mIsDrafts = false;
	mIsInbox = false;
	mIsOutbox = false;
	mIsTrash = false;
}


String *MOG_Filename::GetAssetOriginalFullName(void)
{
	// Make sure we have parsed the path
	ParsePath();

	return mAssetOriginalFullName;
}


String *MOG_Filename::GetEncodedFilename(void)
{
	// Make sure we have parsed the path
	ParsePath();

	String *path = S"";

	switch(mType)
	{
		case MOG_FILENAME_None:
		case MOG_FILENAME_Unknown:
		case MOG_FILENAME_Group:
		case MOG_FILENAME_Info:
		case MOG_FILENAME_SlaveTask:
		case MOG_FILENAME_Log:
		case MOG_FILENAME_Config:
			// Give them back their original string
			path = mOriginalFilename;
			break;

		case MOG_FILENAME_Asset:
		case MOG_FILENAME_Link:
			// Check if the original string was already encoded?
			if (mAssetEncodedName->Length &&
				mOriginalFilename->ToLower()->Contains(mAssetEncodedName->ToLower()))
			{
				// Give them back their original string because it was already encoded
				path = mOriginalFilename;
			}
			else
			{
				// Proceed to fixup an encoded version of their original string
				// Start with their original string
				path = mOriginalFilename;

				// Build the wrong and right names
				String *wrong = "";
				String *right = "";

				if (IsWithinRepository())
				{
					wrong = String::Concat(GetAssetClassification(), S"\\", GetAssetName());
					right = String::Concat(GetAssetEncodedClassification(), S"\\", GetAssetEncodedName());
				}
				else
				{
					wrong = String::Concat(GetAssetFullName());
					right = String::Concat(GetAssetEncodedFullName());
				}

				// Check if we contain the wrong name?
				int pos = path->IndexOf(wrong, StringComparison::CurrentCultureIgnoreCase);
				if (pos != -1)
				{
					// Replace the wrong name with the right name...Do it this way to preserve the case
					path = String::Concat(path->Substring(0, pos), right, path->Substring(pos + wrong->Length));
				}
			}
			break;
	}

	return path;
}

String *MOG_Filename::GetFullFilename(void)
{
	// Make sure we have parsed the path
	ParsePath();

	String *path = S"";

	switch(mType)
	{
		case MOG_FILENAME_None:
		case MOG_FILENAME_Unknown:
		case MOG_FILENAME_Group:
		case MOG_FILENAME_Info:
		case MOG_FILENAME_SlaveTask:
		case MOG_FILENAME_Log:
		case MOG_FILENAME_Config:
			// Give them back their original string
			path = mOriginalFilename;
			break;

		case MOG_FILENAME_Asset:
		case MOG_FILENAME_Link:
			// Check if we have no encoded asset name?
			if (mAssetEncodedName->Length == 0 ||
				!mOriginalFilename->ToLower()->Contains(mAssetEncodedName->ToLower()))
			{
				// Give them back their original string because it is already unencoded
				path = mOriginalFilename;
			}
			else
			{
				// Proceed to fixup an encoded version of their original string
				// Start with their original string
				path = mOriginalFilename;

				// Build the wrong and right names
				String *wrong = "";
				String *right = "";

				if (IsWithinRepository())
				{
					wrong = String::Concat(GetAssetEncodedClassification(), S"\\", GetAssetEncodedName());
					right = String::Concat(GetAssetClassification(), S"\\", GetAssetName());
				}
				else
				{
					wrong = String::Concat(GetAssetEncodedFullName());
					right = String::Concat(GetAssetFullName());
				}

				// Check if we contain the wrong name?
				int pos = path->IndexOf(wrong, StringComparison::CurrentCultureIgnoreCase);
				if (pos != -1)
				{
					// Replace the wrong name with the right name...Do it this way to preserve the case
					path = String::Concat(path->Substring(0, pos), right, path->Substring(pos + wrong->Length));
				}
			}
			break;
	}

	return path;
}

String *MOG_Filename::GetAssetEncodedFilesPath(void)
{
	// Make sure we have parsed the path
	ParsePath();

	String *path = S"";

	// Make sure this is a valid asset type?
	if (mType == MOG_FILENAME_Asset)
	{
		// Make sure we actually detected a files name?
		if (mAssetFilesName->Length)
		{
			// Obtain the asset's path?
			path = GetAssetEncodedPath();
			if (path)
			{
				// Append the asset's internal files name
				path = String::Concat(path, S"\\", mAssetFilesName);
			}
		}
	}

	return path;
}


String *MOG_Filename::GetAssetEncodedRelativeFile(void)
{
	// Make sure we have parsed the path
	ParsePath();

	String *relativeFile = S"";

	// Make sure we actually detected a files name?
	if (mAssetFilesName->Length)
	{
		// Make sure this is a valid asset type?
		if (mType == MOG_FILENAME_Asset)
		{
			// Obtain the needed asset encoded paths?
			String *encodedFullFilename = GetEncodedFilename();
			String *encodedAssetFilesPath = GetAssetEncodedFilesPath();
			if (encodedFullFilename->Length &&
				encodedAssetFilesPath->Length)
			{
				// Append the asset's internal files name
				relativeFile = encodedFullFilename->Substring(encodedAssetFilesPath->Length)->Trim(S"\\"->ToCharArray());
			}
		}
	}

	return relativeFile;
}


String *MOG_Filename::GetAssetEncodedInboxPath(String *boxName)
{
	// Make sure we have parsed the path
	ParsePath();

	return String::Concat(MOG_ControllerInbox::GetInboxPath(mUserName, boxName), S"\\", GetAssetEncodedFullName());
}


String *MOG_Filename::GetAssetEncodedInboxPath(String *userName, String *boxName)
{
	// Make sure we have parsed the path
	ParsePath();

	return String::Concat(MOG_ControllerInbox::GetInboxPath(userName, boxName), S"\\", GetAssetEncodedFullName());
}


String *MOG_Filename::GetAssetEncodedPath(void)
{
	// Make sure we have parsed the path
	ParsePath();

	String *path = "";

	// Check if this is within the inboxes?
	if (mIsWithinInboxes)
	{
		path = mBoxPath;

		// Check if we are within a group?
		if (mIsWithinGroup)
		{
			// Add the entire group tree
			path = String::Concat(path, S"\\", mGroupTree);
		}

		// Append the asset's folder name
		path = String::Concat(path, S"\\", GetAssetEncodedFullName());

		// Check if this asset is deleted?
		if (mIsTrash)
		{
			// Append the asset's version path
			path = String::Concat(path, S"\\R.", mDeletedTimeStamp);
		}
	}
	// Check if this asset is within the repository?
	else if (mIsWithinRepository)
	{
		path = mRepositoryPath;

		// Append the asset's folder name
		path = String::Concat(path, S"\\", GetAssetEncodedClassification(), S"\\", GetAssetEncodedName());

		// Check if this asset has a version?
		if (mVersionTimeStamp->Length)
		{
			// Append the asset's veresion path
			path = String::Concat(path, S"\\R.", mVersionTimeStamp);
		}
	}
	else
	{
		// Check if we have an original path?
		if (mAssetOriginalPath->Length)
		{
			// Use the Asset's original path
			path = Path::GetDirectoryName(mAssetOriginalPath);
			// Append the asset's folder name
			path = String::Concat(path, S"\\", GetAssetEncodedFullName());
		}
	}

	return path;
}


String *MOG_Filename::GetAssetFullName(void)
{
	// Make sure we have parsed the path
	ParsePath();

	// Build the asset name requesting the classification
	String *fullname = String::Concat(GetAssetClassification(), GetAssetName());

	return fullname;
}



String *MOG_Filename::GetAssetEncodedFullName(void)
{
	// Build the asset name requesting the encoded classification
	return GetAssetEncodedName();
}



String *MOG_Filename::GetFormattedString(String *format, String *seeds)
{
	// Make sure we have parsed the path
	ParsePath();

	seeds = MOG_Tokens::AppendTokenSeeds(seeds, MOG_Tokens::GetFilenameTokenSeeds(this));
	return MOG_Tokens::GetFormattedString(format, seeds);
}

bool MOG_Filename::Equals(Object* obj)
{
	//Check for null and compare run-time types.
	if( obj == NULL || GetType() != obj->GetType() )
	{
		return false;
	}

	// check instance equality
	if( this == obj )
	{
		return true;
	}

	// check value equality
	MOG_Filename* mfn = dynamic_cast<MOG_Filename*>(obj);
	return (String::Compare(GetOriginalFilename(), mfn->GetOriginalFilename(), true) == 0);
}


int MOG_Filename::GetHashCode()
{
	return GetOriginalFilename()->GetHashCode();
}


String *MOG_Filename::GetClassificationAdamObjectName(String *classification)
{
	// Split the classification up into all the parts
	String *parts[] = SplitClassificationString(classification);
	if (parts != NULL)
	{
		if (parts->Count)
		{
			// Return the root classification name
            return parts[0];
		}
	}

	return S"";
}


String *MOG_Filename::GetAdamlessClassification(String *classification)
{
	String *adamlessClassification = S"";

	int pos = classification->IndexOf(S"~");
	if (pos != -1)
	{
		adamlessClassification = classification->Substring(pos)->Trim(S"~"->ToCharArray());
	}

	return adamlessClassification;
}


bool MOG_Filename::IsClassificationValidForProject(String *assetClassification, String *projectName)
{
	// Make sure there are valid values specified?
	if (projectName->Length &&
		assetClassification->Length)
	{
		// Create test strings including a trailing '~' for safer check
		String *testClassification1 = String::Concat(assetClassification, S"~");
		String *testProjectName1 = String::Concat(projectName, S"~");
		// Make sure the asset's classification starts with our active project's name?
		if (testClassification1->StartsWith(testProjectName1, StringComparison::CurrentCultureIgnoreCase))
		{
			return true;
		}

		// Create test strings including a trailing '{' in case it is an asset in the project's root
		String *testClassification2 = String::Concat(assetClassification, S"{");
		String *testProjectName2 = String::Concat(projectName, S"{");
		// Make sure the asset's classification starts with our active project's name?
		if (testClassification2->StartsWith(testProjectName2, StringComparison::CurrentCultureIgnoreCase))
		{
			return true;
		}
	}

	return false;
}


String *MOG_Filename::AppendAdamObjectNameOnClassification(String *classification)
{
	// Check if this asset is missing the adam object name?
	if (String::Compare(MOG_Filename::GetClassificationAdamObjectName(classification), MOG_ControllerProject::GetProjectName(), true) != 0)
	{
		// Recreate the classification so that it will include the Adam Object
		classification = String::Concat(MOG_ControllerProject::GetProjectName(), S"~", classification);
	}

	return classification->Trim(S"~"->ToCharArray());
}


MOG_Filename *MOG_Filename::AppendAdamObjectNameOnAssetName(MOG_Filename *assetFilename)
{
	// Make sure this is a valid asset?
	if (assetFilename->GetFilenameType() == MOG_FILENAME_TYPE::MOG_FILENAME_Asset)
	{
		// Check if this classification is missing the adam classification in its name?
		String *oldClassification = assetFilename->GetAssetClassification();
		String *newClassification = AppendAdamObjectNameOnClassification(oldClassification);
		if (String::Compare(oldClassification, newClassification, true) != 0)
		{
			MOG_Filename* newFilename = NULL;

			// Recreate the asset's name so that it will include the Adam Object in its classifications
			String* newAssetName = String::Concat(newClassification, S"{", assetFilename->GetAssetPlatform(), S"}", assetFilename->GetAssetLabel());
			// Check if there is a specified path?
			if (assetFilename->GetPath()->Length)
			{
				newFilename = new MOG_Filename(String::Concat(assetFilename->GetPath(), S"\\", newAssetName));
			}
			else
			{
				newFilename = new MOG_Filename(newAssetName);
			}

			// Return the newly adjusted name
			return newFilename;
		}
	}

	// Just return what they sent in
	return assetFilename;
}


String *MOG_Filename::SplitClassificationString(String *classification)[]
{
	// Establish the delimiters
	String* delimStr = S"~";
	Char delimiter[] = delimStr->ToCharArray();

	// Trim off any leading or trailing delimiters
	classification = classification->Trim(delimiter);

	// Split the classification
	String* parts[] = classification->Split(delimiter);

	return parts;
}


String *MOG_Filename::GetProjectLibraryClassificationString()
{
	// Join the classification
	String* classification = "";

	// Make sure we are logged into a project
	if (MOG_ControllerProject::IsProject())
	{
		classification = MOG_Filename::JoinClassificationString(MOG_ControllerProject::GetProjectName(), S"Library");
	}

	return classification;
}

bool MOG_Filename::IsLibraryClassification(String* classification)
{
	if (classification->StartsWith(GetProjectLibraryClassificationString()))
	{
		return true;
	}

	return false;
}


String *MOG_Filename::JoinClassificationString(String *parts[])
{
	// Join the classification
	String* classification = String::Join("~", parts);

	return classification;
}

String *MOG_Filename::JoinClassificationString(String *part1, String *part2)
{
	// Join the classification
	String* classification = String::Concat(part1, S"~", part2);

	return classification;
}


String *MOG_Filename::AppendOnClassification(String *classification, String *addon)
{
	String *newClassification = classification->Replace(S"\\", S"~");

	// Check if we have something to addon?
	if (addon->Length)
	{
		newClassification = String::Concat(newClassification, S"~", addon->Replace(S"\\", S"~"));
	}

	return newClassification;
}


String *MOG_Filename::GetClassificationPath(String *classification)
{
	// Convert all the classification '~' markers to a '\'
	return classification->Replace(S"~", S"\\");
}


String *MOG_Filename::GetChildClassificationString(String *classification)
{
	// Default us to the specified classification
	String *child = classification;

	// Check if there is a '~' within our classification?
	int pos = child->LastIndexOf("~");
	if (pos > 0)
	{
		// Extract the child portion of specified classification
		child = child->Substring(pos)->Trim(S"~"->ToCharArray());
	}

	return child;
}


bool MOG_Filename::IsParentClassificationString(String *fullClassification, String *parentClassification)
{
	// Add the classification delimiter to the test strings because it ensures the comparison is 100% accurate
	String *testFullClassification = String::Concat(fullClassification, S"~");
	String *testParentClassification = String::Concat(parentClassification, S"~");

	// Check if the full classification actually starts with the parent?
	if (testFullClassification->StartsWith(testParentClassification, StringComparison::CurrentCultureIgnoreCase))
	{
		return true;
	}

	return false;
}

String *MOG_Filename::GetParentsClassificationString(String *classification)
{
	// Default us to nothing just in case we are the adam classification
	String *parent = "";

	// Check if there is a '~' within the specified classification?
	int pos = classification->LastIndexOf("~");
	if (pos > 1)
	{
		// Extract the parent portion of our classification
		parent = classification->Substring(0, pos);
	}

	return parent;
}


String *MOG_Filename::BuildDefaultClassificationFromPath(String *filename)
{
	String *relativePath = "";
	String *classification = "";

	// Make sure we have a valid workspace?
//?	MultiWorkspaces - We need to be careful about this...what assumptions are we making here?
	if (MOG_ControllerProject::GetCurrentSyncDataController())
	{
		// Check if this contain's a full path?
		if (DosUtils::PathIsDriveRooted(filename))
		{
			// Make sure this filename falls within our workspace?
//?	MultiWorkspaces - Making this asumption could be a problem when we have multiple active workspaces
			String *workspaceDirectory = String::Concat(MOG_ControllerProject::GetWorkspaceDirectory(), S"\\");
			if (filename->StartsWith(workspaceDirectory, StringComparison::CurrentCultureIgnoreCase))
			{
				// Build a relative path of the filename within the workspace
				relativePath = DosUtils::PathGetDirectoryPath(filename->Substring(workspaceDirectory->Length));
			}
		}
		else
		{
			relativePath = DosUtils::PathGetDirectoryPath(filename);
		}

		// Check if we obtained a relativePath?
		if (relativePath->Length)
		{
			// Convert the path into a classification
			classification = relativePath->Replace("\\", S"~");
		}
	}

	// Make sure we have the project's adom object name in the classification
	return AppendAdamObjectNameOnClassification(classification);
}


MOG_Filename *MOG_Filename::CreateAssetName(String *classification, String *platformName, String *assetLabel)
{
	// Check if the label might already contain a platformName?
	if (assetLabel->IndexOf(S"{") != -1)
	{
		// Attempt to split the assetLabel
		String *parts[] = assetLabel->Split(S"{}"->ToCharArray(), 3);
		if (parts->Count == 3)
		{
			bool bValidPlatformName = false;

			// Check if this is the '{All}' platform?
			if (String::Compare(parts[1], S"All", true) == 0)
			{
				bValidPlatformName = true;
			}
			else
			{
				// Check if this is a valid platform?
				String *platformNames[] = MOG_ControllerProject::GetProject()->GetPlatformNames();
				for (int p = 0; p < platformNames->Count; p++)
				{
					// Check if this is the '{All}' platform?
					if (String::Compare(parts[1], platformNames[p], true) == 0)
					{
						bValidPlatformName = true;
						break;
					}
				}
			}

			// Check if we determined this to be a valid platformName?
			if (bValidPlatformName)
			{
				// Check if no platform was specified?
				if (platformName->Length == 0)
				{
					platformName = parts[1];
				}

				// Check if we are missing a valid classification?
				if (classification->Length == 0)
				{
					classification = parts[0];
				}

				// Always use the stripped down part as our new assetLabel
				assetLabel = parts[2];
			}
		}
	}

	// Make sure we have valid specifiers
	if (platformName->Length, assetLabel->Length)
	{
		// Combine the AssetFullName
		return new MOG_Filename(String::Concat(classification, S"{", platformName, S"}", assetLabel), S"");
	}

	return NULL;
}


MOG_Filename *MOG_Filename::GetLocalUpdatedTrayFilename(String *localWorkspaceDirectory, MOG_Filename* assetFilename)
{
	MOG_Filename* localAssetFilename = NULL;

	if (assetFilename->GetFilenameType() == MOG_FILENAME_TYPE::MOG_FILENAME_Asset)
	{
		if (!String::IsNullOrEmpty(localWorkspaceDirectory))
		{
			localAssetFilename = new MOG_Filename(String::Concat(localWorkspaceDirectory, S"\\MOG\\UpdatedTray\\", assetFilename->GetAssetEncodedFullName()));
		}
	}

	return localAssetFilename;
}


MOG_Filename *MOG_Filename::GetLocalRemoveTrayFilename(String *localWorkspaceDirectory, MOG_Filename* assetFilename)
{
	MOG_Filename* localAssetFilename = NULL;

	if (assetFilename->GetFilenameType() == MOG_FILENAME_TYPE::MOG_FILENAME_Asset)
	{
		if (!String::IsNullOrEmpty(localWorkspaceDirectory))
		{
			localAssetFilename = new MOG_Filename(String::Concat(localWorkspaceDirectory, S"\\MOG\\RemoveTray\\", assetFilename->GetAssetEncodedFullName()));
		}
	}

	return localAssetFilename;
}


String *MOG_Filename::GetRelativePathWithinInbox(void)
{
	// Make sure we have parsed the path
	ParsePath();

	// Strip off the user's path...if it is defined
	String *path = mOriginalFilename->Substring(mUserPath->Length);

	// Check if we have the encoded name contained within this relative path?
	if (mAssetEncodedName->Length &&
		path->ToLower()->Contains(mAssetEncodedName->ToLower()))
	{
		// Attempt to replace the wrong name with the right name?
		String *wrong = GetAssetEncodedFullName();
		int pos = path->IndexOf(wrong, StringComparison::CurrentCultureIgnoreCase);
		if (pos != -1)
		{
			// Create the replacement string
			String *right = GetAssetFullName();
			// Replace the wrong asset name with the right one...Do it this way to preserve the case
			path = String::Concat(path->Substring(0, pos), right, path->Substring(pos + wrong->Length));
		}
	}

	return path;
}


bool MOG_Filename::IsWithinRepository(void)
{
	// Make sure we have parsed the path
	ParsePath();

	return mIsWithinRepository;
}


bool MOG_Filename::IsBlessed(void)
{
	// Make sure we have parsed the path
	ParsePath();

	return mIsBlessed;
}


bool MOG_Filename::IsArchived(void)
{
	// Make sure we have parsed the path
	ParsePath();

	return mIsArchived;
}


String *MOG_Filename::GetVersionTimeStamp(void)
{
	// Make sure we have parsed the path
	ParsePath();

	return mVersionTimeStamp;
}


String *MOG_Filename::GetDeletedTimeStamp(void)
{
	// Make sure we have parsed the path
	ParsePath();

	return mDeletedTimeStamp;
}


bool MOG_Filename::IsLibrary(void)
{
	// Make sure we have parsed the path
	ParsePath();

	// Ask for the classification
	String* classification = GetAssetClassification();
	// Are we missing a classification to compare?
	if (classification->Length == 0)
	{
		// Worst case, lets just use what was originally set
		classification = GetOriginalFilename();
	}

	// I hate making this kind of an assumption! - We can fix this right on the next major release
	// Split mClassification
	String* parts[] = MOG_Filename::SplitClassificationString(classification);
	if (parts && parts->Count >= 2)
	{
		// Check if the 2nd node is the library?
		if (String::Compare(parts[1], S"Library", true) == 0)
		{
			return true;
		}
	}

	return false;
}


bool MOG_Filename::IsLocal()
{
	// Make sure we have parsed the path
	ParsePath();

	// Make sure this has a drive?
	if (GetDrive()->Length &&
		GetPath()->Length)
	{
		// Make sure this asset is not any of the following?
		if (!IsWithinRepository() &&
			!IsBlessed() &&
			!IsArchived() &&
			!IsWithinInboxes() &&
			!IsTrash())
		{
			// Check if this asset is located within the UpdatedTray or RemoveTray?
			if (mOriginalFilename->IndexOf(S"\\MOG\\UpdatedTray\\", StringComparison::CurrentCultureIgnoreCase) ||
				mOriginalFilename->IndexOf(S"\\MOG\\RemoveTray\\", StringComparison::CurrentCultureIgnoreCase))
			{
				return true;
			}
		}
	}

	return false;
}

// Does a compare on MOG_Filename::GetOriginalFilename(), which is the full System filename
int MOG_Filename::CompareTo(Object *obj)
{
	// If this is a MOG_Filename*, compare it as such
	if( obj->GetType() == __typeof(MOG_Filename) ) 
	{
		MOG_Filename *compareToFilename = __try_cast<MOG_Filename*>(obj);
		return this->GetOriginalFilename()->CompareTo( compareToFilename->GetOriginalFilename() );
	}
	// Else, if this is a string we've been given, try the compare
	else if( obj->GetType() == __typeof(String) )
	{
		String *compareToString = __try_cast<String*>(obj);
		return this->GetOriginalFilename()->CompareTo( compareToString );
	}

    // Else throw
	throw new ArgumentException(S"Object being compared is not a MOG_Filename or a String");    
}


String *MOG_Filename::GetAssetClassification(void)
{
	// Make sure we have parsed the path
	ParsePath();

	// Check if we need to obtain mAssetClassification?
	if (mAssetClassification->Length == 0)
	{
		// Check if we have already tried before and failed?
		if (!mFailedPreviousEncodedClassificationQuery)
		{
			// Check if we have a mEncodedAssetClassification?
			if (mAssetEncodedClassification->Length)
			{
				// Split mClassification
				String* parts[] = mAssetEncodedClassification->Split(S"@()"->ToCharArray());
				if (parts &&
					parts->Count == 4 &&
					parts[0]->Length == 0 &&
					parts[1]->Length == 0 &&
					parts[2]->Length > 0 &&
					parts[3]->Length == 0)
				{
					String *classificationID = parts[2];

					// Attempt to obtain the classification
					int id = Convert::ToInt32(classificationID);
					if (id)
					{
						mAssetClassification = MOG_DBAssetAPI::GetClassificationFullTreeNameByID(id);
					}
				}
			}
			else
			{
				// Rely on the encoded name to obtain the classification
				GetAssetName();
			}
		}
	}

	// Check if we need to obtain mAssetClassification?
	if (mAssetClassification->Length == 0)
	{
		// Indicate we failed to resolve the encoded classification
		mFailedPreviousEncodedClassificationQuery = true;

		// Default to the mAssetEncodedClassification
		mAssetClassification = mAssetEncodedClassification;
	}

	return mAssetClassification;
}


String *MOG_Filename::GetAssetName(void)
{
	// Make sure we have parsed the path
	ParsePath();

	// Check if we need to obtain mAssetName?
	if (mAssetName->Length == 0)
	{
		// Check if we have already tried before and failed?
		if (!mFailedPreviousEncodedNameQuery)
		{
			// Check if we have a mEncodedAssetName?
			if (mAssetEncodedName->Length)
			{
				// Split mAssetEncodedName
				String* parts[] = mAssetEncodedName->Split(S"#()"->ToCharArray());
				if (parts &&
					parts->Count == 4 &&
					parts[0]->Length == 0 &&
					parts[1]->Length == 0 &&
					parts[2]->Length > 0 &&
					parts[3]->Length == 0)
				{
					// Attempt to construct a valid encoded name
					String *nameID = parts[2];

					// Attempt to obtain the name
					int id = Convert::ToInt32(nameID);
					if (id)
					{
						// Obtain the name from the database
						MOG_Filename *assetFilename = MOG_DBAssetAPI::GetAssetName(id);
						if (assetFilename)
						{
							mAssetClassification = assetFilename->GetAssetClassification();
							mAssetPlatform = assetFilename->GetAssetPlatform();
							mAssetLabel = assetFilename->GetAssetLabel();
							mAssetName = assetFilename->GetAssetName();
						}
					}
				}
			}
		}
	}

	// Check if we need to obtain mAssetName?
	if (mAssetName->Length == 0)
	{
		// Indicate we failed to resolve the encoded name
		mFailedPreviousEncodedNameQuery = true;

		// Default to the mAssetEncodedName
		mAssetName = mAssetEncodedName;
	}

	return mAssetName;
}


String *MOG_Filename::GetAssetLabel(void)
{
	// Make sure we have retrieved the real name
	GetAssetName();

	return mAssetLabel;
}


String *MOG_Filename::GetAssetPlatform(void)
{
	// Make sure we have retrieved the real name
	GetAssetName();

	return mAssetPlatform;
}


String *MOG_Filename::GetAssetLabelNoExtension(void)
{
	// Make sure we have parsed the path
	ParsePath();

	return DosUtils::PathGetFileNameWithoutExtension(GetAssetLabel());
}


String *MOG_Filename::GetAssetFilesName(void)
{
	// Make sure we have parsed the path
	ParsePath();

	return mAssetFilesName;
}


String *MOG_Filename::GetAssetFilesScope(void)
{
	// Make sure we have parsed the path
	ParsePath();

	return mAssetFilesScope;
}


String *MOG_Filename::GetAssetEncodedClassification(void)
{
	// Make sure we have parsed the path
	ParsePath();

	// Check if we need to obtain mAssetEncodedClassification?
	if (mAssetEncodedClassification->Length == 0)
	{
		// Check if we have already tried before and failed?
		if (!mFailedPreviousEncodedClassificationQuery)
		{
			// Check if we are missing the classification?
			if (mAssetClassification->Length)
			{
				// We need to obtain the asset's classification using the encoded name first
				GetAssetClassification();
			}

			// Check if we have a mAssetClassification?
			if (mAssetClassification->Length)
			{
				// Attempt to construct a valid encoded classification
				// Get the classification's database id
				int classificationID = MOG_DBAssetAPI::GetClassificationID(mAssetClassification);
				if (classificationID == 0)
				{
					// Create a new classificationID
					if (MOG_DBAssetAPI::CreateClassification(mAssetClassification, MOG_ControllerProject::GetUserName_DefaultAdmin(), MOG_Time::GetVersionTimestamp(), false))
					{
						// Now that it has been created, get the new calssificationID
						classificationID = MOG_DBAssetAPI::GetClassificationID(mAssetClassification);
					}
				}

				// Make sure we obtained a classificationID
				if (classificationID)
				{
					// Return our constructed encoded classification
					mAssetEncodedClassification = String::Concat(S"@(", classificationID.ToString(), S")");
				}
			}
		}
	}

	// Check if we still need to obtain mAssetEncodedClassification?
	if (mAssetEncodedClassification->Length == 0)
	{
		// Indicate we failed to resolve the encoded classification
		mFailedPreviousEncodedClassificationQuery = true;

		// Default to the mAssetClassification
		mAssetEncodedClassification = mAssetClassification;
	}

	// At the very least, give them back what we had
	return mAssetEncodedClassification;
}


String *MOG_Filename::GetAssetEncodedName(void)
{
	// Make sure we have parsed the path
	ParsePath();

	// Check if we need to obtain mAssetEncodedName?
	if (mAssetEncodedName->Length == 0)
	{
		// Check if we have already tried before and failed?
		if (!mFailedPreviousEncodedNameQuery)
		{
			// Check if we have a mAssetClassification?
			// Check if we have a mAssetPlatform?
			// Check if we have a mAssetLabel?
			if (mAssetClassification->Length &&
				mAssetPlatform->Length &&
				mAssetLabel->Length)
			{
				// Get the asset's database id
				int nameID = MOG_DBAssetAPI::GetAssetNameID(this);
				if (nameID == 0)
				{
					// Create a new nameID
					nameID = MOG_DBAssetAPI::CreateAssetName(this);
				}

				if (nameID)
				{
					// Return our constructed encoded name
					mAssetEncodedName = String::Concat(S"#(", nameID.ToString(), S")");
				}
			}
		}
	}

	// Check if we still need to obtain mAssetEncodedName?
	if (mAssetEncodedName->Length == 0)
	{
		// Indicate we failed to resolve the encoded name
		mFailedPreviousEncodedNameQuery = true;

		// Default to the mAssetName
		mAssetEncodedName = mAssetName;
	}

	// At the very least, give them back what we had
	return mAssetEncodedName;
}


bool MOG_Filename::ValidateWindowsPath(String *filename)
{
	// Make sure we have parsed the path
	ParsePath();

	// Check if they neglected to specify a filename?
	if (!filename || !filename->Length)
	{
		// At the very least, we should assume the AssetLabel
		filename = GetAssetLabel();
	}

	// Build a mock version of a blessed asset name
	String *encodedClassification = (mAssetEncodedClassification->Length) ? mAssetEncodedClassification : S"@(0000)";
	String *encodedName = (mAssetEncodedName->Length) ? mAssetEncodedName : S"#(000000)";
	String *blessedFilename = String::Concat(MOG_ControllerProject::GetProjectPath(), S"\\Archive\\", encodedClassification, S"\\", encodedName, S"\\R.000000000000000\\Files.PlatformName\\", DosUtils::PathGetFileName(filename));
	// Check the length of the mock name?
	int blessedDiff = blessedFilename->Length - 260;
	if (blessedDiff > 0)
	{
		// Compose an informative message back to the user
		String *message = String::Concat(S"The blessed repository path for this asset will exceed the Windows filename limitation of 260 characters.\n\n",
										 S"The filename has exceeded this limitation by ", blessedDiff.ToString(), S" characters.\n");
		MOG_ASSERT_THROW(!(blessedDiff > 0), MOG_Exception::MOG_EXCEPTION_InvalidWindowsName, message);
	}

	// Build a mock version of a deleted trash name
	String *encodedTrashPath = String::Concat(GetAssetEncodedInboxPath(S"Trash"), S"\\R.000000000000000\\Files.PlatformName\\", DosUtils::PathGetFileName(filename));
	// Check the length of the mock name?
	int trashDiff = encodedTrashPath->Length - 260;
	if (trashDiff > 0)
	{
		// Compose an informative message back to the user
		String *message = String::Concat(S"The blessed repository path for this asset will exceed the Windows filename limitation of 260 characters.\n\n",
										 S"The filename has exceeded this limitation by ", trashDiff.ToString(), S" characters.\n");
		MOG_ASSERT_THROW(!(trashDiff > 0), MOG_Exception::MOG_EXCEPTION_InvalidWindowsName, message);
	}

	return true;
}


MOG_FILENAME_TYPE MOG_Filename::GetFilenameType(void)
{
	// Make sure we have parsed the path
	ParsePath();

	return mType;
}


String *MOG_Filename::GetDrive(void)
{
	// Check if we need to initialize this variable?
	if (mDrive == NULL)
	{
		// Get the drive?
		mDrive = DosUtils::PathGetRootPath(mOriginalFilename);
	}

	return mDrive;
}


String *MOG_Filename::GetPath(void)
{
	// Check if we need to initialize this variable?
	if (mPath == NULL)
	{
		// Get the path?
		mPath = DosUtils::PathGetDirectoryPath(mOriginalFilename);
	}

	return mPath;
}


String *MOG_Filename::GetFilename(void)
{
	// Check if we need to initialize this variable?
	if (mFilename == NULL)
	{
		// Get the filename?
		mFilename = DosUtils::PathGetFileName(mOriginalFilename);

		// Check if this is a group?
		String *groupString = S"Group.";
		if (mFilename->StartsWith(groupString, StringComparison::CurrentCultureIgnoreCase))
		{
			mType = MOG_FILENAME_Group;
		}
	}

	return mFilename;
}


String *MOG_Filename::GetExtension(void)
{
	// Check if we need to initialize this variable?
	if (mExtension == NULL)
	{
		// Get the Extension
		mExtension = DosUtils::PathGetExtension(GetFilename());

		// Check if this is a slave task?
		if (String::Compare(mExtension, S"SlaveTask", true) == 0)
		{
			mType = MOG_FILENAME_SlaveTask;
		}
	}

	return mExtension;
}


bool MOG_Filename::IsWithinPath(String* path)
{
	return IsWithinPath(path, GetEncodedFilename());
}

bool MOG_Filename::IsWithinPath(String* path, String* filename)
{
	// Add on a temp '\' to both string before comparison to ensure we don't match a path similar
	String* testPath = String::Concat(path, S"\\");
	String* testFilename = String::Concat(filename, S"\\");
	if (testFilename->StartsWith(testPath, StringComparison::CurrentCultureIgnoreCase))
	{
		return true;
	}

	return false;
}


String *MOG_Filename::GetProjectName(void)
{
	// Make sure we have parsed the path
	ParsePath();

	return mProjectName;
}


String *MOG_Filename::GetProjectPath(void)
{
	// Make sure we have parsed the path
	ParsePath();

	return mProjectPath;
}


String *MOG_Filename::GetRepositoryPath(void)
{
	// Make sure we have parsed the path
	ParsePath();

	return mRepositoryPath;
}


String *MOG_Filename::GetUserName(void)
{
	// Make sure we have parsed the path
	ParsePath();

	return mUserName;
}


String *MOG_Filename::GetUserPath(void)
{
	// Make sure we have parsed the path
	ParsePath();

	return mUserPath;
}


String *MOG_Filename::GetBoxName(void)
{
	// Make sure we have parsed the path
	ParsePath();

	return mBoxName;
}


String *MOG_Filename::GetBoxPath(void)
{
	// Make sure we have parsed the path
	ParsePath();

	return mBoxPath;
}


String *MOG_Filename::GetGroupName(void)
{
	// Make sure we have parsed the path
	ParsePath();

	return mGroupName;
}


String *MOG_Filename::GetGroupTree(void)
{
	// Make sure we have parsed the path
	ParsePath();

	return mGroupTree;
}


bool MOG_Filename::IsWithinGroup(void)
{
	// Make sure we have parsed the path
	ParsePath();

	return mIsWithinGroup;
}


bool MOG_Filename::IsWithinInboxes(void)
{
	// Make sure we have parsed the path
	ParsePath();

	return mIsWithinInboxes;
}


bool MOG_Filename::IsDrafts(void)
{
	// Make sure we have parsed the path
	ParsePath();

	return mIsDrafts;
}


bool MOG_Filename::IsInbox(void)
{
	// Make sure we have parsed the path
	ParsePath();

	return mIsInbox;
}


bool MOG_Filename::IsOutBox(void)
{
	// Make sure we have parsed the path
	ParsePath();

	return mIsOutbox;
}


bool MOG_Filename::IsTrash(void)
{
	// Make sure we have parsed the path
	ParsePath();

	return mIsTrash;
}


bool MOG_Filename::IsLink(void)
{
	// Make sure we have parsed the asset
	ParsePath();

	return (mType == MOG_FILENAME_Link);
}

	
void MOG_Filename::ParsePath()
{
	// Check if we have already parsed the path
	if (bAlreadyParsedPath == true)
	{
		return;
	}

	// Proceed to parse the path
	bAlreadyParsedPath = true;

	// Split the path up to try and get as much out of it as possible
	String* delimStr = S"\\";
	Char delimiter[] = delimStr->ToCharArray();
	String* parts[] = mOriginalFilename->Trim()->Split(delimiter);
	int partOffset = 0;

	// These are used to indicate when we found certain parts while parsing the filename
	bool bJustStarted = false;
	bool bJustDetectedProjectPath = false;
	bool bJustDetectedProjectName = false;
	bool bJustDetectedUserPath = false;
	bool bJustDetectedUserName = false;
	bool bJustDetectedUserBox = false;
	bool bJustDetectedAssetName = false;

	// Walk through the directory names looking for identifiers
	// Track our pos in the string as we examine each part
	for (int pos = 0, i = 0; i < parts->Count; pos += parts[i]->Length + 1, i++)
	{
		// Check if it starts with the repository path?
		if (mOriginalFilename->StartsWith(MOG_ControllerSystem::GetSystemRepositoryPath(), StringComparison::CurrentCultureIgnoreCase))
		{
			// Skip ahead until we have stated parsing from the inside fo the path
			if (pos < MOG_ControllerSystem::GetSystemRepositoryPath()->Length)
			{
				// Skip past this one because it isn't within the repository
				continue;
			}
		}

		// Check if we still haven't started detecting anything yet?
		if (mProjectPath->Length == 0 &&
			mProjectName->Length == 0 &&
			mUserPath->Length == 0 &&
			mUserName->Length == 0 &&
			mBoxPath->Length == 0 &&
			mBoxName->Length == 0 &&
			mAssetName->Length == 0)
		{
			bJustStarted = true;
		}
		else
		{
			bJustStarted = false;
		}
		if (bJustStarted)
		{
			// Check for the 'Projects' identifier?
			if (String::Compare(parts[i], S"Projects", true) == 0)
			{
				bJustDetectedProjectPath = true;
				continue;
			}
		}
		// Check if we just detected the ProjectPath?
		if (bJustDetectedProjectPath)
		{
			bJustDetectedProjectPath = false;
			// Retrieve the ProjectName
			mProjectName = parts[i];
			mProjectPath = mOriginalFilename->Substring(0, pos + mProjectName->Length);
			// Indicate we just detected the ProjectName
			bJustDetectedProjectName = true;
			continue;
		}
		// Check if we just detected the ProjectName?
		if (bJustStarted || bJustDetectedProjectName)
		{
			bJustDetectedProjectName = false;

			// Check for the 'Assets' identifier?
			String *assetsString = S"Assets";
			if (String::Compare(parts[i], assetsString, true) == 0)
			{
				mIsWithinRepository = true;
				mIsBlessed = true;
				mRepositoryPath = mOriginalFilename->Substring(0, pos + assetsString->Length);
				continue;
			}
			// Check for the 'Archive' identifier?
			String *archiveString = S"Archive";
			if (String::Compare(parts[i], archiveString, true) == 0)
			{
				mIsWithinRepository = true;
				mIsArchived = true;
				mRepositoryPath = mOriginalFilename->Substring(0, pos + archiveString->Length);
				continue;
			}
			// Check for the 'Users' identifier?
			if (!String::Compare(parts[i], S"Users", true))
			{
				bJustDetectedUserPath = true;
				continue;
			}
		}
		// Check if we just detected the UserPath?
		if (bJustDetectedUserPath)
		{
			bJustDetectedUserPath = false;
			// Retrieve the UserName
			mUserName = parts[i];
			mUserPath = mOriginalFilename->Substring(0, pos + mUserName->Length);
			// The User's BoxName should follow the UserName
			bJustDetectedUserName = true;
			continue;
		}
		// Check if we just detected the UserName?
		if (bJustStarted || bJustDetectedUserName)
		{
			bJustDetectedUserName = false;

			// Check for the 'Drafts' identifier?
			if (String::Compare(parts[i], S"Drafts", true) == 0)
			{
				mIsDrafts = true;
				bJustDetectedUserBox = true;
			}
			// Check for the 'Inbox' identifier?
			else if (String::Compare(parts[i], S"Inbox", true) == 0)
			{
				mIsInbox = true;
				bJustDetectedUserBox = true;
			}
			// Check for the 'Outbox' identifier?
			else if (String::Compare(parts[i], S"Outbox", true) == 0)
			{
				mIsOutbox = true;
				bJustDetectedUserBox = true;
			}
			// Check for the 'Trash' identifier?
			else if (String::Compare(parts[i], S"Trash", true) == 0)
			{
				mIsTrash = true;
				bJustDetectedUserBox = true;
			}

			// Check if we matched any of the above boxes?
			if (bJustDetectedUserBox)
			{
				// Retrieve the BoxName
				mBoxName = parts[i];
				mBoxPath = mOriginalFilename->Substring(0, pos + mBoxName->Length);
				mIsWithinInboxes = true;
				continue;
			}
		}
		// Check if we just detected the UserBox?
		if (bJustStarted || bJustDetectedUserBox)
		{
			bJustDetectedUserBox = false;

			// Check for the 'Group.' identifier?
			String *groupString = S"Group.";
			if (parts[i]->StartsWith(groupString, StringComparison::CurrentCultureIgnoreCase))
			{
				// Extract the GroupName
				mGroupName = parts[i];
				// Only set the AssetName as this group if this is the final piece of the filename
				if (i == parts->Count - 1)
				{
					mAssetName = mGroupName;
				}

				// Check if we have already started tracking a GroupTree?
				if (mGroupTree->Length)
				{
					// Prepare to append the new GroupName
					mGroupTree = String::Concat(mGroupTree, S"\\");
				}
				// Append the new group name on to the GroupTree
				mGroupTree = String::Concat(mGroupTree, mGroupName);
				mIsWithinGroup = true;
				continue;
			}
		}
		// Check if we have found the AssetName?
		if (bJustDetectedAssetName)
		{
			bJustDetectedAssetName = false;

			// Check for the 'R.' identifier?
			String *revisionToken = S"R.";
			if (parts[i]->StartsWith(revisionToken, StringComparison::CurrentCultureIgnoreCase))
			{
				// Check if this is within the trash?
				if (mIsTrash)
				{
					mDeletedTimeStamp = parts[i]->Substring(revisionToken->Length);
				}
				else
				{
					mVersionTimeStamp = parts[i]->Substring(revisionToken->Length);
				}

				// Keep drilling within the asset as if the name was just detected
				bJustDetectedAssetName = true;
				continue;
			}

			// Check for 'Files.' identifier?
			String *filesString = S"Files.";
			if (parts[i]->StartsWith(filesString, StringComparison::CurrentCultureIgnoreCase))
			{
				// Extract the Files info
				mAssetFilesName = parts[i];
				mAssetFilesScope = mAssetFilesName->Substring(mAssetFilesName->IndexOf(S".") + 1);
				continue;
			}
		}

		// Check if we still haven't found an AssetName in the path?
		if (!mAssetName->Length)
		{
			// Parse the AssetName
			if (ParseAssetName(parts[i]))
			{
				// Obtain the AssetPath of this asset
				mAssetOriginalPath = mOriginalFilename->Substring(0, pos + parts[i]->Length);
				bJustDetectedAssetName = true;
			}
		}
	}
}


bool MOG_Filename::ParseAssetName(String *potentialAssetName)
{
	// Check for a '(Link)' tag at the beginning of the part?
	String *linkString = S"(Link)";
	if (potentialAssetName->StartsWith(linkString, StringComparison::CurrentCultureIgnoreCase))
	{
		// Indicate that this is really a Link to an Asset instead of an actual Asset
		mType = MOG_FILENAME_Link;
		// Strip off the '(Link)' tag from the beginning of the classification
		potentialAssetName = potentialAssetName->Substring(linkString->Length);
	}

	// Check for an encoded name?
	if (potentialAssetName->StartsWith(S"#("))
	{
		// Indicate this is an Asset if it ins't already a link?
		if (mType != MOG_FILENAME_Link)
		{
			// Assume this is an asset because it really looks like one
			mType = MOG_FILENAME_Asset;
		}

		// Retain the original
		mAssetOriginalFullName = potentialAssetName;

		// Get the encoded name
		mAssetEncodedName = potentialAssetName;

		// Check if we are in the repository?
		if (mIsWithinRepository)
		{
			// Attempt to obtain the encoded classification from the repository path
			String *temp = mOriginalFilename->Substring(mRepositoryPath->Length)->Trim(S"\\"->ToCharArray());
			int pos = temp->IndexOf(S"\\");
			if (pos != -1)
			{
				temp = temp->Substring(0, pos);
			}
			mAssetEncodedClassification = temp;
		}

		return true;
	}
	else
	{
		// Attempt to split the asset name in order to validate it?
		String* delimStr = S"{}";
		Char delimiter[] = delimStr->ToCharArray();
		String* tokens[] = potentialAssetName->Split(delimiter, 3);
		// An asset name should always be 3 tokens
		if (tokens->Count == 3)
		{
			// Indicate this is an Asset if it ins't already a link?
			if (mType != MOG_FILENAME_Link)
			{
// JohnRen - Wow, this would be better but there have been some assumptions made with things like '{All}Assetname' that now fail!
// The trash was severely broken with this change and we had complaints come in about MOG_CommandLine forcing them to include the projectname.
//				// Check if we are logged into a project?
//				if (MOG_ControllerProject::IsProject())
//				{
//					// Make sure this asset's classification starts with the project's name?
//					if (IsClassificationValidForProject(tokens[0], MOG_ControllerProject::GetProjectName()))
//					{
//						// This looks like a real asset
//						mType = MOG_FILENAME_Asset;
//					}
//				}
//				else
//				{
					// It would have been nice to verify the classification with the project but, assume this is an asset because it really looks like one
					mType = MOG_FILENAME_Asset;
//				}
			}

			// Retain the original
			mAssetOriginalFullName = potentialAssetName;

			// Trim off any leading or trailing delimiters from the asset's classification
			mAssetClassification = tokens[0]->Trim(S"~"->ToCharArray());
			mAssetPlatform = tokens[1];
			mAssetLabel = tokens[2];
			mAssetName = String::Concat(S"{", mAssetPlatform, S"}", mAssetLabel);

			// Base the extension off of asset label just in case there is a '.' elsewhere in the asset name
			mExtension = DosUtils::PathGetExtension(mAssetLabel);

			return true;
		}
	}

	return false;
}
