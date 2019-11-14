//--------------------------------------------------------------------------------
//	MOG_EnvironmentVars.h
//	
//	To add an item, please do the following:
//		1)	Add a '#define' under the appropriate (commented) section,
//		2)	Add a getter function for C# under the appropriate section,
//		3)	Add the token defined in step 1 under the appropriate array (nammed accordingly).
//--------------------------------------------------------------------------------

#ifndef __MOG_ENVIRONMENT_VARS_H__
#define __MOG_ENVIRONMENT_VARS_H__

#pragma once
#using <mscorlib.dll>
#using <system.dll>


#include "MOG_Define.h"



namespace MOG
{
	namespace ENVIRONMENT_VARS
	{
		//Asset Related:
		#define	MOG_EnvVar_Asset						S"MOG_Asset"
		#define	MOG_EnvVar_AssetPath					S"MOG_AssetPath"
		#define	MOG_EnvVar_AssetName					S"MOG_AssetName"
		#define	MOG_EnvVar_AssetClassification			S"MOG_AssetClassification"
		#define	MOG_EnvVar_AssetKey						S"MOG_AssetKey"
		#define	MOG_EnvVar_AssetLabel					S"MOG_AssetLabel"
		#define	MOG_EnvVar_AssetExtension				S"MOG_AssetExtension"
		#define	MOG_EnvVar_AssetPlatformName			S"MOG_AssetPlatformName"
		#define	MOG_EnvVar_AssetProjectKey				S"MOG_AssetProjectKey"
		//Filename Related:
		#define	MOG_EnvVar_BoxName						S"MOG_BoxName"
		#define	MOG_EnvVar_Extension					S"MOG_Extension"
		#define	MOG_EnvVar_FullFilename					S"MOG_FullFilename"
		//Project Related:
		#define	MOG_EnvVar_ProjectName					S"MOG_ProjectName"
		#define	MOG_EnvVar_BranchName					S"MOG_BranchName"
		#define	MOG_EnvVar_PlatformName					S"MOG_PlatformName"
		#define	MOG_EnvVar_UserName						S"MOG_UserName"
		//Command Related:
		#define	MOG_EnvVar_CommandID					S"MOG_CommandID"
		#define	MOG_EnvVar_CommandType					S"MOG_CommandType"
		#define	MOG_EnvVar_CommandTimeStamp				S"MOG_CommandTimeStamp"
		//Computer Related:
		#define	MOG_EnvVar_ComputerIP					S"MOG_ComputerIP"
		#define	MOG_EnvVar_ComputerName					S"MOG_ComputerName"
		#define	MOG_EnvVar_NetworkID					S"MOG_NetworkID"


		//Slave Task Related:
		#define MOG_EnvVar_CommandLabel					S"MOG_CommandLabel"
		#define	MOG_EnvVar_WorkingDirectory				S"MOG_WorkingDirectory"
		#define	MOG_EnvVar_AssetFiles					S"MOG_AssetFiles"
		#define	MOG_EnvVar_RippedFilesDestination		S"MOG_RippedFilesDestination"


		//Package Related:
		#define	MOG_EnvVar_PackageWorkspaceDirectory	S"MOG_PackageWorkspaceDirectory"
		#define	MOG_EnvVar_PackageDataDirectory			S"MOG_PackageDataDirectory"
		#define MOG_EnvVar_PackageMergeType				S"MOG_PackageMergeType"
		#define MOG_EnvVar_PackageInputFile				S"MOG_PackageInputFile"
		#define MOG_EnvVar_PackageOutputFile			S"MOG_PackageOutputFile"


		//Build Related:
		#define	MOG_EnvVar_BuildProjectName				S"MOG_BuildProjectName"
		#define	MOG_EnvVar_BuildProjectBranchName		S"MOG_BuildProjectBranchName"
		#define	MOG_EnvVar_BuildType					S"MOG_BuildType"
		#define	MOG_EnvVar_BuildPlatformName			S"MOG_BuildPlatformName"
		#define	MOG_EnvVar_BuildTool					S"MOG_BuildTool"
		#define	MOG_EnvVar_BuildToolPath				S"MOG_BuildToolPath"
		#define	MOG_EnvVar_BuildRequestedBy				S"MOG_BuildRequestedBy"


		//
		//
		public __gc class MOG_EnvironmentVars
		{
		private:
			const static String *mSystemRelated[] = 
			{
				MOG_EnvVar_Asset,
				MOG_EnvVar_AssetPath,
				MOG_EnvVar_AssetName,
				MOG_EnvVar_AssetClassification,
				MOG_EnvVar_AssetKey,
				MOG_EnvVar_AssetLabel,
				MOG_EnvVar_AssetExtension,
				MOG_EnvVar_AssetPlatformName,
				MOG_EnvVar_AssetProjectKey,

				MOG_EnvVar_BoxName,
				MOG_EnvVar_Extension,
				MOG_EnvVar_FullFilename,

				MOG_EnvVar_ProjectName,
				MOG_EnvVar_BranchName,
				MOG_EnvVar_PlatformName,
				MOG_EnvVar_UserName,

				MOG_EnvVar_CommandID,
				MOG_EnvVar_CommandType,
				MOG_EnvVar_CommandTimeStamp,

				MOG_EnvVar_ComputerIP,
				MOG_EnvVar_ComputerName,
				MOG_EnvVar_NetworkID,
			}; // end assetRelated[]

			const static String *mTaskRelated[] =
			{
				MOG_EnvVar_CommandLabel,
				MOG_EnvVar_WorkingDirectory,
				MOG_EnvVar_AssetFiles,
				MOG_EnvVar_RippedFilesDestination,
			}; // end taskRelated[]

			const static String *mPackageRelated[] =
			{
				MOG_EnvVar_PackageMergeType,
				MOG_EnvVar_PackageInputFile,
				MOG_EnvVar_PackageOutputFile,
			}; // end mPackageRelated[]

			const static String *mBuildRelated[] =
			{
				MOG_EnvVar_BuildProjectName,
				MOG_EnvVar_BuildProjectBranchName,
				MOG_EnvVar_BuildType,
				MOG_EnvVar_BuildPlatformName,
				MOG_EnvVar_BuildTool,
				MOG_EnvVar_BuildToolPath,
				MOG_EnvVar_BuildRequestedBy,
			}; // end buildRelated[]

		public:
			// Path Utility Functions
			static String *CreateToolsPathEnvironmentVariableString(String *relativeToolsPath);
			static String *CreateToolsPathEnvironmentVariableString(String *relativeToolsPath, MOG_Filename *assetDirectory);

			// Decoder for Server GUI
//			static String *DecodeEnvironmentVar(Object variables, blah, blah){};

			// Get functions for each of the arrays of tokens
			static const String *GetAllSystemRelated()[]	{	return mSystemRelated;	};
			static const String *GetAllTaskRelated()[]		{	return mTaskRelated;	};
			static const String *GetAllBuildRelated()[]		{	return mBuildRelated;	};

			// Get functions for C# use :: Asset Related
			static String *GetAsset(void)					{	return MOG_EnvVar_Asset;	};
			static String *GetAssetPath(void)				{	return MOG_EnvVar_AssetPath;	};
			static String *GetAssetName(void)				{	return MOG_EnvVar_AssetName;	};
			static String *GetAssetClassification(void)		{	return MOG_EnvVar_AssetClassification;	};
			static String *GetAssetKey(void)				{	return MOG_EnvVar_AssetKey;	};
			static String *GetAssetLabel(void)				{	return MOG_EnvVar_AssetLabel;	};
			static String *GetAssetExtension(void)			{	return MOG_EnvVar_AssetExtension;	};
			static String *GetAssetPlatformName(void)		{	return MOG_EnvVar_AssetPlatformName;	};

			// Get functions for C# use :: Filename Related
			static String *GetBoxName(void)					{	return MOG_EnvVar_BoxName;	};
			static String *GetExtension(void)				{	return MOG_EnvVar_Extension;	};
			static String *GetFullFilename(void)			{	return MOG_EnvVar_FullFilename;	};

			// Get functions for C# use :: Project Related
			static String *GetProjectName(void)				{	return MOG_EnvVar_ProjectName;	};
			static String *GetBranchName(void)				{	return MOG_EnvVar_BranchName;	};
			static String *GetPlatformName(void)			{	return MOG_EnvVar_PlatformName;	};
			static String *GetUserName(void)				{	return MOG_EnvVar_UserName;	};

			// Get functions for C# use :: Command Related
			static String *GetCommandID(void)				{	return MOG_EnvVar_CommandID;	};
			static String *GetCommandType(void)				{	return MOG_EnvVar_CommandType;	};
			static String *GetCommandTimeStamp(void)		{	return MOG_EnvVar_CommandTimeStamp;	};

			// Get functions for C# use :: Computer Related
			static String *GetComputerIP(void)				{	return MOG_EnvVar_ComputerIP;	};
			static String *GetComputerName(void)			{	return MOG_EnvVar_ComputerName;	};
			static String *GetNetworkID(void)				{	return MOG_EnvVar_NetworkID;	};


			// Get functions for C# use :: Slave Task Related
			static String *GetCommandLabel(void)			{	return MOG_EnvVar_CommandLabel;	};
			static String *GetWorkingDirectory(void)		{	return MOG_EnvVar_WorkingDirectory;	};
			static String *GetAssetFiles(void)				{	return MOG_EnvVar_AssetFiles;	};
			static String *GetRippedFilesDestination(void)	{	return MOG_EnvVar_RippedFilesDestination;	};


			// Get functions for C# use :: Package Related
			static String *GetPackageMergeType(void)		{	return MOG_EnvVar_PackageMergeType;	};
			static String *GetPackageInputFile(void)		{	return MOG_EnvVar_PackageInputFile;	};
			static String *GetPackageOutputFile(void)		{	return MOG_EnvVar_PackageOutputFile;	};


			// Get functions for C# use :: Build Related
			static String *GetBuildProjectName(void)		{	return MOG_EnvVar_BuildProjectName;	};
			static String *GetBuildProjectBranchName(void)	{	return MOG_EnvVar_BuildProjectBranchName;	};
			static String *GetBuildType(void)				{	return MOG_EnvVar_BuildType;	};
			static String *GetBuildPlatformName(void)		{	return MOG_EnvVar_BuildPlatformName;	};
			static String *GetBuildTool(void)				{	return MOG_EnvVar_BuildTool;	};
			static String *GetBuildToolPath(void)			{	return MOG_EnvVar_BuildToolPath;	};
			static String *GetBuildRequestedBy(void)		{	return MOG_EnvVar_BuildRequestedBy;	};
		}; // End class
	} // End namespace SYSTEM_VARS
} // End namespace MOG

using namespace MOG::ENVIRONMENT_VARS;

#endif

