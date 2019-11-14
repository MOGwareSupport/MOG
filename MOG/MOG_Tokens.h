//--------------------------------------------------------------------------------
//	MOG_Tokens.h
//	
//	To add an item, please do the following:
//		1)	Add a '#define' under the appropriate (commented) section,
//		2)	Add a getter function for C# under the appropriate section,
//		3)	Add the token defined in step 1 under the appropriate array (nammed accordingly).
//--------------------------------------------------------------------------------
#pragma once
#using <mscorlib.dll>
#using <system.dll>

#include "string.h"
#include "MOG_Time.h"
#include "MOG_Project.h"
#include "MOG_System.h"
#include "MOG_ControllerPackageMerge.h"
#include "MOG_ControllerSyncData.h"

namespace MOG
{
	//Commands
	#define TOKEN_CommandSubString					S"Substring"
	#define TOKEN_CommandReplace					S"Replace"
	#define TOKEN_CommandTrim						S"Trim"
	#define TOKEN_CommandTrimEnd					S"TrimEnd"
	#define TOKEN_CommandTrimStart					S"TrimStart"
	#define TOKEN_CommandLength						S"Length"
	#define TOKEN_CommandPadLeft					S"PadLeft"
	#define TOKEN_CommandPadRight					S"PadRight"
	#define TOKEN_CommandIndexOf					S"IndexOf"
	#define TOKEN_CommandIndexOfAny					S"IndexOfAny"
	#define TOKEN_CommandUpper						S"Upper"
	#define TOKEN_CommandLower						S"Lower"
	#define TOKEN_CommandInsert						S"Insert"
	#define TOKEN_CommandLastIndexOf				S"LastIndexOf"
	#define TOKEN_CommandLastIndexOfAny				S"LastIndexOfAny"
	#define TOKEN_CommandRemove						S"Remove"

	// OS
//	#define TOKEN_System							S"{System}"
	#define TOKEN_Windows							S"{Windows}"

	//System
	#define TOKEN_SystemRepositoryPath				S"{SystemRepositoryPath}"
	#define TOKEN_SystemProjectsPath				S"{SystemProjectsPath}"

	//Project
	#define TOKEN_ProjectPath						S"{ProjectPath}"
	#define TOKEN_ProjectName						S"{ProjectName}"
	#define TOKEN_Project							S"{Project}"
	#define TOKEN_Repository						S"{Repository}"
	#define TOKEN_Archive							S"{Archive}"
	#define TOKEN_Users								S"{Users}"
	#define TOKEN_ProjectPlatform					S"{ProjectPlatform}"
	#define TOKEN_ProjectBranch						S"{ProjectBranch}"
	#define TOKEN_ProjectSystemPath					S"{ProjectSystemPath}"
	#define TOKEN_ProjectSystemToolsPath			S"{ProjectSystemToolsPath}"
	#define TOKEN_LoginUserName						S"{LoginUserName}"
	#define TOKEN_WorkspaceDirectory				S"{WorkspaceDirectory}"

	//Ripper
	#define TOKEN_SourcePath						S"{SourcePath}"
	#define TOKEN_SourceFilePattern					S"{SourceFilePattern}"
	#define TOKEN_DestinationPath					S"{DestinationPath}"

	//Package
	#define TOKEN_PackageName						S"{PackageName}"
	#define TOKEN_PackageClassification				S"{PackageClassification}"
	#define TOKEN_PackageLabel						S"{PackageLabel}"
	#define TOKEN_PackageLabelNoExtension			S"{PackageLabelNoExtension}"
	#define TOKEN_PackageExtension					S"{PackageExtension}"
	#define TOKEN_PackagePlatform					S"{PackagePlatform}"
	#define TOKEN_PackageGroup						S"{PackageGroup}"
	#define TOKEN_PackageObject						S"{PackageObject}"
	#define TOKEN_PackageFile						S"{PackageFile}"
	#define TOKEN_PackageWorkspaceDirectory			S"{PackageWorkspaceDirectory}"
	#define TOKEN_PackageDataDirectory				S"{PackageDataDirectory}"

	//Filename
	#define TOKEN_FullFilename						S"{FullFilename}"
	#define TOKEN_Filename							S"{Filename}"
	#define TOKEN_Drive								S"{Drive}"
	#define TOKEN_Path								S"{Path}"
	#define TOKEN_Extension							S"{Extension}"
	#define TOKEN_UserName							S"{UserName}"
	#define TOKEN_UserPath							S"{UserPath}"
	#define TOKEN_BoxName							S"{BoxName}"
	#define TOKEN_BoxPath							S"{BoxPath}"
	#define TOKEN_GroupName							S"{GroupName}"
	#define TOKEN_GroupTree							S"{GroupTree}"
	#define TOKEN_AssetFilesName					S"{AssetFilesName}"
	#define TOKEN_AssetFilesScope					S"{AssetFilesScope}"
	#define TOKEN_AssetFilesPath					S"{AssetFilesPath}"
	#define TOKEN_AssetRelativeFile					S"{AssetRelativeFile}"
	#define TOKEN_VersionTimeStamp					S"{VersionTimeStamp}"
	#define TOKEN_DeletedTimeStamp					S"{DeletedTimeStamp}"
	#define TOKEN_RepositoryPath					S"{RepositoryPath}"
	#define TOKEN_AssetFullName						S"{AssetFullName}"
	#define TOKEN_AssetPath							S"{AssetPath}"
	#define TOKEN_AssetClassification				S"{AssetClassification}"
	#define TOKEN_AssetClassificationFull			S"{AssetClassification.Full}"
	#define TOKEN_AssetClassificationPath			S"{AssetClassificationPath}"
	#define TOKEN_AssetClassificationPathFull		S"{AssetClassificationPath.Full}"
	#define TOKEN_AssetClassificationX				S"{AssetClassification.X}"
	#define TOKEN_AssetName							S"{AssetName}"
	#define TOKEN_AssetPlatform						S"{AssetPlatform}"
	#define TOKEN_AssetLabel						S"{AssetLabel}"
	#define TOKEN_AssetLabelNoExtension				S"{AssetLabelNoExtension}"

	//Time
	#define TOKEN_Year_2							S"{Year.2}"
	#define TOKEN_Year_4							S"{Year.4}"
	#define TOKEN_month								S"{month}"
	#define TOKEN_Month								S"{Month}"
	#define TOKEN_MONTH								S"{MONTH}"
	#define TOKEN_Month_1							S"{Month.1}"
	#define TOKEN_Month_2							S"{Month.2}"
	#define TOKEN_month_3							S"{month.3}"
	#define TOKEN_Month_3							S"{Month.3}"
	#define TOKEN_MONTH_3							S"{MONTH.3}"
	#define TOKEN_day								S"{day}"
	#define TOKEN_Day								S"{Day}"
	#define TOKEN_DAY								S"{DAY}"
	#define TOKEN_Day_1 							S"{Day.1}"
	#define TOKEN_Day_2 							S"{Day.2}"
	#define TOKEN_day_3 							S"{day.3}"
	#define TOKEN_Day_3 							S"{Day.3}"
	#define TOKEN_DAY_3 							S"{DAY.3}"
	#define TOKEN_24Hour_1 							S"{24Hour.1}"
	#define TOKEN_24Hour_2 							S"{24Hour.2}"
	#define TOKEN_Hour_1 							S"{Hour.1}"
	#define TOKEN_Hour_2 							S"{Hour.2}"
	#define TOKEN_Minute_1 							S"{Minute.1}"
	#define TOKEN_Minute_2 							S"{Minute.2}"
	#define TOKEN_Second_1 							S"{Second.1}"
	#define TOKEN_Second_2 							S"{Second.2}"
	#define TOKEN_Millisecond 					 	S"{Millisecond}"
	#define TOKEN_ampm 								S"{ampm}"
	#define TOKEN_AMPM 								S"{AMPM}"
	

// New StatusTypes must also be added to mStatusTypes and the Get functions (below)

	public __gc class MOG_Tokens
	{
	public:
		static bool TokenExist(String* format);
		static String* GetOSTokenSeeds();
		static String* GetSystemTokenSeeds();
		static String* GetProjectTokenSeeds(MOG_Project* pProject);
		static String* GetWorkspaceTokenSeeds(MOG_ControllerSyncData* workspace);
		static String* GetWorkspaceTokenSeeds(String* workspaceDirectory, String* platformName);
		static String* GetRipperTokenSeeds(String* sourcePath, String* sourceFilePattern, String* destinationPath);
		static String* GetPackageTokenSeeds(String* assignedPackageStatement);
		static String* GetPackageTokenSeeds(MOG_PackageMerge_PackageFileInfo* packageFileInfo);
		static String* GetPackageTokenSeeds(MOG_PackageMerge_PackageFileInfo* packageFileInfo, String* assignedPackageStatement);
		static String* GetPackageTokenSeeds(String* packageName, String* packagePlatform, String* packageGroups, String* packageObjects, String* packageFile);
		static String* GetFilenameTokenSeeds(MOG_Filename* pFilename);
		static String* GetClassificationTokenSeeds(String *classification);
		static String* GetTimeTokenSeeds(MOG_Time* pTime);
		static String* GetTimeTokenSeeds();
		static String* GetPropertyTokenSeeds(MOG_Property* pProperty);
		static String* GetPropertyTokenSeeds(ArrayList* pProperties);

		static String* AppendTokenSeeds(String* seeds, String* newSeeds);

		static String* GetFormattedString(String* format);
		static String* GetFormattedString(String* format, MOG_Filename* seedAssetFilename);
		static String* GetFormattedString(String* format, MOG_Filename* seedAssetFilename, ArrayList* properties);
		static String* GetFormattedString(String* format, MOG_Filename* seedAssetFilename, ArrayList* properties, String* sourcePath, String* sourceFilePattern, String* destinationPath);
		static String* GetFormattedString(String* format, MOG_Filename* seedAssetFilename, ArrayList* properties, String* assignedPackageStatement);
		static String* GetFormattedString(String* format, MOG_Filename* seedAssetFilename, MOG_PackageMerge_PackageFileInfo* packageFileInfo, String* workspaceDirectory, String* platformName);
		static String* GetFormattedString(String* format, MOG_Filename* seedAssetFilename, MOG_PackageMerge_PackageFileInfo* packageFileInfo, String* workspaceDirectory, String* platformName, MOG_Properties* properties, String* assignedPackageStatement);
		static String* GetFormattedString(String* format, String* seeds);
		static String* GetFormattedString_GetSeedString(String* token, String* seeds);
		static String* GetFormattedString_ParseToken(String* text);
		static String* GetFormattedString_ParseCommands(String* token);
		static String* GetFormattedString_ParseBaseToken(String* token);
		static String* GetFormattedString_ExecuteTokenCommands(String* text, String* command, String* seeds);
		static String* GetFormattedString_ExecuteTokenCommand(String* text, String* commands, String* args, String* seeds);
		static int GetFormattedString_CountCommandArgs(String* commandArgs);
		static String* GetFormattedString_GetCommandArg(String* commandArgs, int index, String* seeds);
		static int GetFormattedString_GetCommandArgAsInt(String* commandArgs, int index, String* seeds);
		static String* GetFormattedString_ParseNestedString(String* text);
		static int GetFormattedString_ResolvedMath(String* text);

		// Get functions for each of the arrays of tokens
		static const String* GetAllSystemTokens(void)[]		{	return mSystemTokens;	};
		static const String* GetAllProjectTokens(void)[]	{	return mProjectTokens;	};
		static const String* GetAllFilenameTokens(void)[]	{	return mFilenameTokens;	};
		static const String* GetAllRipperTokens(void)[]		{	return mRipperTokens;	};
		static const String* GetAllPackageTokens(void)[]	{	return mPackageTokens;	};
		static String* GetAllTimeTokens(void)[]				{	return mTimeTokens;		};

		// Get functions for C# use :: OS
		static String* GetWindows(void)						{	return TOKEN_Windows;	};

		// Get functions for C# use :: SYSTEM
		static String* GetSystemRepositoryPath(void)		{	return TOKEN_SystemRepositoryPath;	};

		// Get functions for C# use :: PROJECT
		static String* GetProjectPath(void)					{	return TOKEN_ProjectPath;	};
		static String* GetProjectName(void)					{	return TOKEN_ProjectName;	};
		static String* GetProject(void)						{	return TOKEN_Project;		};
		static String* GetRepository(void)					{	return TOKEN_Repository;	};
		static String* GetArchive(void)						{	return TOKEN_Archive;		};
		static String* GetUsers(void)						{	return TOKEN_Users;			};
		static String* GetProjectPlatform(void)				{	return TOKEN_ProjectPlatform;		};
		static String* GetProjectBranch(void)				{	return TOKEN_ProjectBranch;		};
		static String* GetProjectSystemPath(void)			{	return TOKEN_ProjectSystemPath;			};
		static String* GetProjectSystemToolsPath(void)		{	return TOKEN_ProjectSystemToolsPath;	};
		static String* GetLoginUserName(void)				{	return TOKEN_LoginUserName;	};
		static String* GetWorkspaceDirectory(void)			{	return TOKEN_WorkspaceDirectory; };

		// Get functions for C# use :: PACKAGE
		static String* GetPackageName (void)				{	return TOKEN_PackageName;	};
		static String* GetPackageClassification (void)		{	return TOKEN_PackageClassification;	};
		static String* GetPackageLabel (void)				{	return TOKEN_PackageLabel;	};
		static String* GetPackageLabelNoExtension (void)	{	return TOKEN_PackageLabelNoExtension;	};
		static String* GetPackageExtension (void)			{	return TOKEN_PackageExtension;	};
		static String* GetPackagePlatform (void)			{	return TOKEN_PackagePlatform;	};
		static String* GetPackageGroup (void)				{	return TOKEN_PackageGroup;	};
		static String* GetPackageObject (void)				{	return TOKEN_PackageObject;};
		static String* GetPackageFile (void)				{	return TOKEN_PackageFile;	};
		static String* GetPackageWorkspaceDirectory (void)	{	return TOKEN_PackageWorkspaceDirectory;	};
		static String* GetPackageDataDirectory (void)		{	return TOKEN_PackageDataDirectory;	};

		// Get functions for C# use :: FILENAME
		static String* GetFullFilename (void)				{	return TOKEN_FullFilename;	};
		static String* GetFilename (void)					{	return TOKEN_Filename;	};
		static String* GetDrive (void)						{	return TOKEN_Drive;		};
		static String* GetPath (void)						{	return TOKEN_Path;		};
		static String* GetExtension (void)					{	return TOKEN_Extension;	};
		static String* GetUserName (void)					{	return TOKEN_UserName;	};
		static String* GetUserPath (void)					{	return TOKEN_UserPath;	};
		static String* GetBoxName (void)					{	return TOKEN_BoxName;	};
		static String* GetBoxPath (void)					{	return TOKEN_BoxPath;	};
		static String* GetGroupName (void)					{	return TOKEN_GroupName;	};
		static String* GetGroupTree (void)					{	return TOKEN_GroupTree;	};
		static String* GetAssetFilesName (void)				{	return TOKEN_AssetFilesName;	};
		static String* GetAssetFilesScope (void)			{	return TOKEN_AssetFilesScope;	};
		static String* GetAssetFilesPath (void)				{	return TOKEN_AssetFilesPath;	};
		static String* GetAssetRelativeFile (void)			{	return TOKEN_AssetRelativeFile;	};
		static String* GetVersionTimeStamp (void)			{	return TOKEN_VersionTimeStamp;	};
		static String* GetDeletedTimeStamp (void)			{	return TOKEN_DeletedTimeStamp; };
		static String* GetRepositoryPath (void)				{	return TOKEN_RepositoryPath;		};
		static String* GetAssetFullName (void)				{	return TOKEN_AssetFullName;		};
		static String* GetAssetPath (void)					{	return TOKEN_AssetPath;		};
		static String* GetAssetClassification (void)		{	return TOKEN_AssetClassification;	};
		static String* GetAssetClassificationFull (void)	{	return TOKEN_AssetClassificationFull;	};
		static String* GetAssetClassificationPath (void)	{	return TOKEN_AssetClassificationPath;	};
		static String* GetAssetClassificationPathFull (void){	return TOKEN_AssetClassificationPathFull;	};
		static String* GetAssetClassificationX (void)		{	return TOKEN_AssetClassificationX;	};
		static String* GetAssetName (void)					{	return TOKEN_AssetName;		};
		static String* GetAssetPlatform (void)				{	return TOKEN_AssetPlatform;	};
		static String* GetAssetLabel (void)					{	return TOKEN_AssetLabel;	};
		static String* GetAssetLabelNoExtension (void)		{	return TOKEN_AssetLabelNoExtension;	};

		// Get functions for C# use :: TIME
		static String* GetYear_2 (void)			{	return TOKEN_Year_2;		};
		static String* GetYear_4 (void)			{	return TOKEN_Year_4;		};
		static String* Getmonth (void)			{	return TOKEN_month;		};
		static String* GetMonth (void)			{	return TOKEN_Month;		};
		static String* GetMONTH (void)			{	return TOKEN_MONTH;		};
		static String* GetMonth_1 (void)		{	return TOKEN_Month_1;		};
		static String* GetMonth_2 (void)		{	return TOKEN_Month_2;		};
		static String* Getmonth_3 (void)		{	return TOKEN_month_3;		};
		static String* GetMonth_3 (void)		{	return TOKEN_Month_3;		};
		static String* GetMONTH_3 (void)		{	return TOKEN_MONTH_3;		};
		static String* Getday (void)			{	return TOKEN_day;		};
		static String* GetDay (void)			{	return TOKEN_Day;		};
		static String* GetDAY (void)			{	return TOKEN_DAY;		};
		static String* GetDay_1 (void)			{	return TOKEN_Day_1;		};
		static String* GetDay_2 (void)			{	return TOKEN_Day_2;		};
		static String* Getday_3 (void)			{	return TOKEN_day_3;		};
		static String* GetDay_3 (void)			{	return TOKEN_Day_3;		};
		static String* GetDAY_3 (void)			{	return TOKEN_DAY_3;		};
		static String* Get24Hour_1 (void)		{	return TOKEN_24Hour_1;		};
		static String* Get24Hour_2 (void)		{	return TOKEN_24Hour_2;		};
		static String* GetHour_1 (void)			{	return TOKEN_Hour_1;		};
		static String* GetHour_2 (void)			{	return TOKEN_Hour_2;		};
		static String* GetMinute_1 (void)		{	return TOKEN_Minute_1;		};
		static String* GetMinute_2 (void)		{	return TOKEN_Minute_2;		};
		static String* GetSecond_1 (void)		{	return TOKEN_Second_1;		};
		static String* GetSecond_2 (void)		{	return TOKEN_Second_2;		};
		static String* GetMillisecond (void)	{	return TOKEN_Millisecond; 	};
		static String* Getampm (void)			{	return TOKEN_ampm ;		};
		static String* GetAMPM (void)			{	return TOKEN_AMPM ;		};

	private:
		static String* AddNextTokenSeed(String* currentSeeds, String* token, String* value, bool bSkipBlankTokenValue);

		static const String* mCommands[] = 
		{
			TOKEN_CommandSubString,
			TOKEN_CommandReplace,
			TOKEN_CommandTrim,
			TOKEN_CommandTrimEnd,
			TOKEN_CommandTrimStart,
			TOKEN_CommandLength,
			TOKEN_CommandPadLeft,
			TOKEN_CommandPadRight,
			TOKEN_CommandIndexOf,
			TOKEN_CommandIndexOfAny,
			TOKEN_CommandUpper,
			TOKEN_CommandLower,
			TOKEN_CommandInsert,
			TOKEN_CommandLastIndexOf,
			TOKEN_CommandLastIndexOfAny,
			TOKEN_CommandRemove,
		};

		static const String* mOSTokens[] = 
		{
			TOKEN_Windows,
		};
		static const String* mSystemTokens[] = 
		{
			TOKEN_SystemRepositoryPath,
			TOKEN_SystemProjectsPath,
		};
		static const String* mProjectTokens[] = 
		{
			TOKEN_Project,
			TOKEN_ProjectName,
			TOKEN_ProjectPath,
			TOKEN_Repository,
			TOKEN_Archive,
			TOKEN_Users,
			TOKEN_ProjectPlatform,
			TOKEN_ProjectBranch,
			TOKEN_ProjectSystemPath,
			TOKEN_ProjectSystemToolsPath,
			TOKEN_LoginUserName,
			TOKEN_WorkspaceDirectory,
		};
		static const String* mPackageTokens[] = 
		{
			TOKEN_PackageName,
			TOKEN_PackageClassification,
			TOKEN_PackageLabel,
			TOKEN_PackageLabelNoExtension,
			TOKEN_PackageExtension,
			TOKEN_PackagePlatform,
			TOKEN_PackageGroup,
			TOKEN_PackageObject,
			TOKEN_PackageFile,
			TOKEN_PackageWorkspaceDirectory,
			TOKEN_PackageDataDirectory,
		};
		static const String* mRipperTokens[] = 
		{
			TOKEN_SourcePath,
			TOKEN_SourceFilePattern,
			TOKEN_DestinationPath,
		};
		static const String* mFilenameTokens[] = 
		{
			TOKEN_AssetFullName,
			TOKEN_AssetPath,
			TOKEN_AssetClassification,
			TOKEN_AssetClassificationFull,
			TOKEN_AssetClassificationPath,
			TOKEN_AssetClassificationPathFull,
			TOKEN_AssetClassificationX,
			TOKEN_AssetName,
			TOKEN_AssetPlatform,
			TOKEN_AssetLabel,
			TOKEN_AssetLabelNoExtension,

			TOKEN_FullFilename,
			TOKEN_Filename,
			TOKEN_Drive,
			TOKEN_Path,
			TOKEN_Extension,
			TOKEN_UserName,
			TOKEN_UserPath,
			TOKEN_BoxName,
			TOKEN_BoxPath,
			TOKEN_GroupName,
			TOKEN_GroupTree,
			TOKEN_AssetFilesName,
			TOKEN_AssetFilesScope,
			TOKEN_AssetFilesPath,
			TOKEN_AssetRelativeFile,
			TOKEN_VersionTimeStamp,
			TOKEN_DeletedTimeStamp,
			TOKEN_RepositoryPath,
		};
		static String* mTimeTokens[] = 
		{
			TOKEN_Year_2,
	 		TOKEN_Year_4,
	 		TOKEN_month,
	 		TOKEN_Month,
	 		TOKEN_MONTH,
	 		TOKEN_Month_1,
	 		TOKEN_Month_2,
	 		TOKEN_month_3,
	 		TOKEN_Month_3,
	 		TOKEN_MONTH_3,
	 		TOKEN_day,
	 		TOKEN_Day,
	 		TOKEN_DAY,
	 		TOKEN_Day_1,
	 		TOKEN_Day_2,
	 		TOKEN_day_3,
	 		TOKEN_Day_3,
	 		TOKEN_DAY_3,
	 		TOKEN_24Hour_1,
	 		TOKEN_24Hour_2,
	 		TOKEN_Hour_1,
	 		TOKEN_Hour_2,
	 		TOKEN_Minute_1,
	 		TOKEN_Minute_2,
	 		TOKEN_Second_1,
	 		TOKEN_Second_2,
	 		TOKEN_Millisecond,
	 		TOKEN_ampm,
	 		TOKEN_AMPM,
		};
	};
}

using namespace MOG;