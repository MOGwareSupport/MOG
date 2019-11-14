//--------------------------------------------------------------------------------
//	MOG_Filename.h
//	
//	
//--------------------------------------------------------------------------------

#ifndef __MOG_Filename_H__
#define __MOG_Filename_H__

#include "MOG_Define.h"


namespace MOG {
namespace FILENAME {

public __value enum MOG_FILENAME_TYPE
{
	MOG_FILENAME_None,

	// Filename Types
	MOG_FILENAME_Asset,
	MOG_FILENAME_Group,
	MOG_FILENAME_Link,

	// File Extensions
	MOG_FILENAME_Info,
	MOG_FILENAME_SlaveTask,
	MOG_FILENAME_Log,
	MOG_FILENAME_Config,
	MOG_FILENAME_Unknown,
};


public __gc class MOG_Filename : public System::IComparable
{
// Had to change this for Gus' change to use MOG_Filename in a hash list or something like that?  Can't remember!
//private:
public:
	MOG_FILENAME_TYPE mType;

	String *mOriginalFilename;

	// Parsing Variables
	bool bAlreadyParsedPath;

	String *mDrive;
	String *mPath;
	String *mFilename;
	String *mExtension;

	String *mProjectName;
	String *mProjectPath;
	String *mRepositoryPath;
	String *mUserName;
	String *mUserPath;
	String *mBoxName;
	String *mBoxPath;

	String *mGroupName;
	String *mGroupTree;
	bool mIsWithinGroup;

	String *mAssetFilesName;
	String *mAssetFilesScope;

	String *mVersionTimeStamp;
	String *mDeletedTimeStamp;

	String *mAssetOriginalFullName;			// '#(452)' or 'Advent~Animations~Humans~Male~Gideon{All}JumpHigh'
	String *mAssetOriginalPath;				// 'm:\MOG\Projects\MyProj\Users\JohnRen\Drafts\' + '#(452)' or 'Advent~Animations~Humans~Male~Gideon{All}JumpHigh'
	String *mAssetClassification;			// 'Advent~Animations~Humans~Male~Gideon'
	String *mAssetEncodedClassification;	// '@(37)'
	String *mAssetEncodedName;				// '#(452)'
	String *mAssetName;						// '{All}JumpHigh'
	String *mAssetPlatform;					// 'All'
	String *mAssetLabel;					// 'JumpHigh'

	bool mFailedPreviousEncodedClassificationQuery;
	bool mFailedPreviousEncodedNameQuery;

	bool mIsWithinRepository;
	bool mIsBlessed;
	bool mIsArchived;

	bool mIsWithinInboxes;
	bool mIsDrafts;
	bool mIsInbox;
	bool mIsOutbox;
	bool mIsTrash;

public:
	// Constructors
	MOG_Filename(void);
	MOG_Filename(MOG_Filename *filename);
	MOG_Filename(String *filename);
	MOG_Filename(char *filename);
	MOG_Filename(String *userName, String *boxName, String *groupTree, String *assetFullName);
	MOG_Filename(String *assetFullName, String *revisionTimeStamp);

	// Initialization
	void ClearFilename(void);
	void SetFilename(String *filename);
	void SetFilename(MOG_Filename *filename);
	void SetVersionTimeStamp(String *timeStamp)		{ mVersionTimeStamp = timeStamp; };
	static MOG_Filename *CreateAssetName(String *classification, String *platformName, String *assetLabel);
	static MOG_Filename *GetLocalUpdatedTrayFilename(String *localWorkspaceDirectory, MOG_Filename* assetFilename);
	static MOG_Filename *GetLocalRemoveTrayFilename(String *localWorkspaceDirectory, MOG_Filename* assetFilename);

	bool ValidateWindowsPath(String *filename);

	// Access Functions
	MOG_FILENAME_TYPE GetFilenameType(void);

	// Parsing
	void ParsePath();
	bool ParseAssetName(String *potentialAssetName);
	// General
	String *GetDrive(void);
	String *GetPath(void);
	String *GetFilename(void);
	String *GetExtension(void);
	bool IsWithinPath(String* path);
	static bool IsWithinPath(String* path, String* filename);

	// Project
	String *GetProjectName(void);
	String *GetProjectPath(void);
	String *GetRepositoryPath(void);
	String *GetUserName(void);
	String *GetUserPath(void);
	// Asset
	String *GetAssetFullName(void);
	String *GetAssetClassification(void);
	String *GetAssetName(void);
	String *GetAssetPlatform(void);
	String *GetAssetLabel(void);
	String *GetAssetLabelNoExtension(void);
	bool IsPlatformSpecific(void)					{ return (String::Compare(GetAssetPlatform(), S"All", true) != 0); };
	String *GetAssetFilesName(void);
	String *GetAssetFilesScope(void);

	// Tokens
	String *GetFormattedString(String *format, String *seeds);

	// Repository Utilities
	bool IsWithinRepository(void);
	bool IsBlessed(void);
	bool IsArchived(void);
	bool IsLibrary(void);
	String *GetVersionTimeStamp(void);
	String *GetVersionTimeStampString(String *format);
	String *GetDeletedTimeStamp(void);

	// Original Filename APIs
	String *GetOriginalFilename(void)				{ return mOriginalFilename; };
	String *GetAssetOriginalFullName(void);
	String *GetAssetOriginalPath(void)				{ return mAssetOriginalPath; };

	// Encoded Filename APIs
	String *GetEncodedFilename(void);
	String *GetNotSureDefaultEncodedFilename(void)	{ return GetEncodedFilename(); };
	String *GetAssetEncodedPath(void);
	String *GetAssetEncodedFullName(void);
	String *GetAssetEncodedClassification(void);
	String *GetAssetEncodedName(void);
	String *GetAssetEncodedFilesPath(void);
	String *GetAssetEncodedRelativeFile(void);
	String *GetAssetEncodedInboxPath(String *boxName);
	String *GetAssetEncodedInboxPath(String *userName, String *boxName);

	// Full Filename APIs
	String *GetFullFilename(void);

	// Inbox Utilities
	String *GetRelativePathWithinInbox(void);
	String *GetBoxName(void);
	String *GetBoxPath(void);
	String *GetGroupName(void);
	String *GetGroupTree(void);
	bool IsWithinGroup(void);
	bool IsWithinInboxes(void);
	bool IsDrafts(void);
	bool IsInbox(void);
	bool IsOutBox(void);
	bool IsTrash(void);
	bool IsLink(void);
	bool IsLocal(void);

	// Classification Utilities
	static String *GetClassificationAdamObjectName(String *classification);
	static String *GetAdamlessClassification(String *classification);
	static bool IsClassificationValidForProject(String *assetClassification, String *projectName);
	static String *AppendAdamObjectNameOnClassification(String *classification);
	static MOG_Filename *AppendAdamObjectNameOnAssetName(MOG_Filename *assetFilename);
	static String *GetProjectLibraryClassificationString();
	static bool IsLibraryClassification(String* classification);
	static String *SplitClassificationString(String *classification)[];
	static String *JoinClassificationString(String *parts[]);
	static String *JoinClassificationString(String *part1, String *part2);
	static String *AppendOnClassification(String *classification, String *addon);
	static String *GetClassificationPath(String *classification);
	static String *GetChildClassificationString(String *classification);
	static bool IsParentClassificationString(String *fullClassification, String *parentClassification);
	static String *GetParentsClassificationString(String *classification);
	static String *BuildDefaultClassificationFromPath(String *filename);

	/// Implementation for IComparable .NET System interface (for ArrayList::Sort() )
	int CompareTo(Object *obj);

	// Hash Helpers
	bool Equals(Object* obj);
	int GetHashCode();
};

}
}

using namespace MOG::FILENAME;

#endif	// __MOG_Filename_H__
