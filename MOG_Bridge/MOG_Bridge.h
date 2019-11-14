// MOG_Bridge.h
//  Hybrid header that exposes one method (GetMOGBridge())
//		to unmanaged C/C++, which returns a pointer
//		to allow interaction with methods
//
// NOTE:
// This file should NEVER include <mscorlib.dll> because this class must derive from MOG_BridgeAPI.
// The Source file will include it instead so the code can still talk with unmanaged C/C++.
//
// In other words, the only include file should be MOG_BridgeAPI.h.


#ifndef __MOG_BRIDGEIMPL_H__
#define __MOG_BRIDGEIMPL_H__

#pragma once

#include "MOG_BridgeAPI.h"

using namespace System;
using namespace System::Collections;

using namespace MOG;
using namespace MOG::COMMAND;
using namespace MOG::ASSET_STATUS;


namespace MOG
{
	namespace BRIDGE
	{
		public __gc class MOG_PendingImport
		{
		public:
			// Static list of pending objects (This had to be in a managed class so we just made it static here)
			static ArrayList *gPendingImports = new ArrayList();
			static ArrayList *GetPendingImportList()			{ return gPendingImports; };

			MOG_PendingImport();

			String *mFilename;
			String *mAssetClassification;
			String *mAssetLabel;
			String *mAssetPlatform;
			ArrayList *mProperties;
			MOG_AssetStatusType mFinalStatus;
		};

		class MOG_Bridge : public MOG_BridgeAPI
		{
		private:
			int mBaseHandle;		// Handle by which MOG_Bridge references its instance to MOG_BridgeContainer
			bool mIsInitialized;

			// Function pointers
			bool (*mCommandHandler)( MOGAPI_BaseInfo *pCommandInfo );
			void (*mEventHandler)(MOGAPI_BaseInfo *pEventInfo );

			//Utility functions
			__gc class String *ResolveAssetNameForSyncTargetFile(__gc class String *pAssetName, LockInfo *info);
			void PopulateLockInfo( MOG_Command *pCommand, LockInfo *info );

			virtual bool NotifySystemAlert(IN String *pTitle, IN String *pMessage, IN String *pStackTrace);
			virtual bool NotifySystemError(IN String *pTitle, IN String *pMessage, IN String *pStackTrace);
			virtual bool NotifySystemException(IN String *pTitle, IN String *pMessage, IN String *pStackTrace);

		public:
			MOG_Bridge();
			~MOG_Bridge();
			virtual void Shutdown( void );

			virtual bool CheckAPIVersion(int version);
			virtual bool CheckMajorVersion();
			virtual bool CheckMinorVersion();

			virtual bool InitializeClient(const char *name, bool bUnused);
			virtual bool InitializeCommandLine(const char *name, bool bUnused);
			virtual bool InitializeEditor(const char *name, bool bUnused);

			virtual void SetAssociatedWorkspacePath( const char *path );

			virtual bool ProcessTick( void );


			bool HandleCommand( MOGAPI_BaseInfo *pCommandInfo );
			void HandleEvent( MOGAPI_BaseInfo *pEventInfo );

			virtual void RegisterCommandHandler( bool (Handler)( MOGAPI_BaseInfo *pCommandType ) );
			virtual void RegisterEventHandler( void (Handler)( struct MOGAPI_BaseInfo *pEventType ), unsigned int iEventFlags);

			virtual void SetEventHandlerFlags( unsigned int iFlags );
			virtual unsigned int GetEventHandlerFlags( void );

			virtual bool PersistentLock_Release(const char *assetName, LockInfo *info);
			virtual bool PersistentLock_Request(const char *assetName, const char *description, LockInfo *info);
			virtual bool PersistentLock_Query(const char *pAssetName, LockInfo *info);

			virtual bool ResolveAssetNameForSyncTargetFile(IN const char *pFileName, OUT char *pAssetName, LockInfo *info);

			virtual MOGBridgeHandle AssetSyncFiles_OpenObject(IN const char *pAssetName);
			virtual bool AssetSyncFiles_CloseObject(IN MOGBridgeHandle handle);
			virtual int AssetSyncFiles_GetFileCount(IN MOGBridgeHandle handle);
			virtual bool AssetSyncFiles_GetFilename(IN MOGBridgeHandle handle, IN int index, OUT char *pFilename);

			virtual MOGBridgeHandle AssetProperties_OpenObject(IN const char *pAssetName);
			virtual bool AssetProperties_CloseObject(IN MOGBridgeHandle handle);
			virtual bool AssetProperties_GetProperty(IN MOGBridgeHandle handle, IN MOGAPI_Property *property);
			virtual bool AssetProperties_GetProperty(IN MOGBridgeHandle handle, IN const char *section, IN const char *propertySection, IN const char *propertyKey, OUT char *propertyValue);
			virtual bool AssetProperties_GetProperty(IN MOGBridgeHandle handle, IN const char *section, IN int keyIndex, OUT char *key, OUT char *value);
			virtual bool AssetProperties_GetPackageCommand_Add(IN MOGBridgeHandle handle, IN int fileIndex, IN int packageAssignmentIndex, OUT char *value);
			virtual bool AssetProperties_GetPackageCommand_Remove(IN MOGBridgeHandle handle, IN int fileIndex, IN int packageAssignmentIndex, OUT char *value);

			virtual bool GetUserName(char *userName);
			virtual bool GetBranchName(char *branchName);
			virtual bool GetProjectName(char *projectName);
			virtual bool GetLocalWorkspace(char *localWorkspace);
			virtual bool GetLocalWorkspacePlatform(char *localWorkspace);

			virtual bool NotifySystemAlert(IN const char *pTitle, IN const char *pMessage, IN const char *pStackTrace);
			virtual bool NotifySystemError(IN const char *pTitle, IN const char *pMessage, IN const char *pStackTrace);
			virtual bool NotifySystemException(IN const char *pTitle, IN const char *pMessage, IN const char *pStackTrace);

			virtual bool AddLateResolvers( IN const char *PackageName, IN const char *LinkedPackage);

			virtual bool ImportAsset(const char *pAssetName, const char *pImportFiles[], int importFilesLen, const char *pLogFiles[], 
									int logFilesLen, const MOGAPI_Property *pProperties[], int propertiesLen, bool bDeleteImportFilesAfterImport, 
									bool bDeleteLogFilesAfterImport);
			virtual bool AddPendingImport(const char *pFilename, const char *pAssetLabel, const char *pPackageFile, const char *pPackageGroupName, const char *pPackageObjectName, const char *pLOD, bool bExportedFromEditor);
			virtual bool DoPendingImports();

			virtual bool IsLocalFileOutofdate(IN const char *pFilename);

			virtual bool RemovePackageAssignment(IN const char *pAssetLabel, IN const char *pPackageFile, IN const char *pPackageGroupName, IN const char *pPackageObjectName, IN const char *LOD);
			virtual bool RemoveRelationshipAssignment(IN const char *pAssetLabel, IN const char *pRelationship, IN const char *pRelatedAssetName, IN const char *pRelatedGroupName, IN const char *pRelatedObjectName);

			virtual bool GenerateLODName(IN const char *pAssetLabel, IN const char *pLOD, OUT char *pName);
			virtual bool DetectLODTagFromAssetLabel(IN const char *pAssetLabel, OUT char *pLODTag);
			virtual bool StripLODTagFromAssetLabel(IN const char *pAssetLabel, OUT char *pStrippedAssetLabel);
			virtual bool DetectLODFromAssetLabel(IN const char *pAssetLabel, OUT char *pLOD);
			virtual bool DetectLODFromLODTag(IN const char *pLODTag, OUT char *pLOD);

			// NOT IMPLEMENTED 
			virtual char *GetPermissions(void);
			virtual void SetPermissions(const char *pPermissions);

			// DLL Reporting Modes
			virtual void EnableSilentMode(bool enable);
			virtual bool IsSilentMode();
			virtual void EnableConsoleMode(bool enable);
			virtual bool IsConsoleMode();
			virtual void EnableLogFileMode(bool enable);
			virtual bool IsLogFileMode();
			virtual void SetLogFileName(const char* filename);

			// Inbox Controls
			virtual bool InboxFetchFromRepository(IN const char *pBox, IN const char *pAssetName);
			virtual bool InboxDelete(IN const char *pBox, IN const char *pAssetName);
			virtual bool InboxRip(IN const char *pBox, IN const char *pAssetName);
			virtual bool InboxReject(IN const char *pBox, IN const char *pAssetName, IN const char *pComment);
			virtual bool InboxBless(IN const char *pBox, IN const char *pAssetName, IN const char *pComment);
			virtual bool InboxMoveTo(IN const char *pBox, IN const char *pAssetName, IN const char *pComment, IN const char *pTargetUser);
			virtual bool InboxCopyTo(IN const char *pBox, IN const char *pAssetName, IN const char *pComment, IN const char *pTargetUser);

			virtual bool IsConnected( void );

			//Managed/Unmanaged String functions
			static void CopyManagedStringToNative(char* pDst, String *pSrc, size_t length);
			static char* AllocNativeString(String* pSrc);
			static void FreeNativeString(char* pNativeString);

			// Utility Getter for MOG_BridgeContainer::RegisterContainer
			int GetHandle( void );
		};
	} // end BRIDGE
} // end MOG

using namespace MOG::BRIDGE;

#endif //__MOG_BRIDGEIMPL_H__