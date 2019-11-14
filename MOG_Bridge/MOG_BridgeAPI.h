// MOG_BridgeAPI.h
/*
	Definition for abstract class MOG_BridgeAPI, from which MOG_Bridge is derived.

	NOTE:  IF a function is declared here as non-abstract, it will cause a linker
	problem of unresolved external symbol in client app.
 */

#ifndef __MOG_BRIDGEAPI_H__
#define __MOG_BRIDGEAPI_H__

#include <cstring>

#define MOG_Export __declspec(dllexport)
extern "C"
{
	// Indicates that a parameter will simply be passed in to MOG_BridgeAPI
	#define IN
	// Indicates that a parameter will be written to by MOG_BridgeAPI
	#define OUT

	// Used for dll/code version validation
	// ================================================================================
	//	  NOTE:	Please look for any functions located at the bottom of the BridgeAPI
	//			anytime the MOG_API_VERSION is changed!  In order to maintain backward 
	//			compatibility, new functions are placed at the bottom...
	//			When MOG_API_VERSION changes, we can rearrange functions!!!
	// ================================================================================
	#define MOG_API_VERSION ((1<<16)| 0<<8 | 6)

	// Callback flags for unsigned int, please see documentation for further details.
	#define EVENT_CALLBACK_FLAG_ConnectionEvents				0x00000001
	#define EVENT_CALLBACK_FLAG_RegistrationEvents				0x00000002
	#define EVENT_CALLBACK_FLAG_LoginEvents						0x00000004
	#define EVENT_CALLBACK_FLAG_ViewEvents						0x00000008
	#define EVENT_CALLBACK_FLAG_RefreshEvents					0x00000010
	#define EVENT_CALLBACK_FLAG_RipEvents						0x00000020
	#define EVENT_CALLBACK_FLAG_BlessEvents						0x00000040
	#define EVENT_CALLBACK_FLAG_RemoveEvents					0x00000080
	#define EVENT_CALLBACK_FLAG_PackageEvents					0x00000100
	#define EVENT_CALLBACK_FLAG_PostEvents						0x00000200
	#define EVENT_CALLBACK_FLAG_CleanupEvents					0x00000400
	#define EVENT_CALLBACK_FLAG_LockEvents						0x00000800
	#define EVENT_CALLBACK_FLAG_MergeEvents						0x00001000
	#define EVENT_CALLBACK_FLAG_MessageEvents					0x00002000
	#define EVENT_CALLBACK_FLAG_BuildEvents						0x00004000
	#define EVENT_CALLBACK_FLAG_CommandContainersEvents			0x00008000
	#define EVENT_CALLBACK_FLAG_CriticalEventEvents				0x00010000
	#define EVENT_CALLBACK_FLAG_RequestsEvents					0x00020000
	#define EVENT_CALLBACK_FLAG_All								EVENT_CALLBACK_FLAG_ConnectionEvents | EVENT_CALLBACK_FLAG_RegistrationEvents | EVENT_CALLBACK_FLAG_LoginEvents	| EVENT_CALLBACK_FLAG_ViewEvents | EVENT_CALLBACK_FLAG_RefreshEvents | EVENT_CALLBACK_FLAG_RipEvents | EVENT_CALLBACK_FLAG_BlessEvents | EVENT_CALLBACK_FLAG_RemoveEvents | EVENT_CALLBACK_FLAG_PackageEvents | EVENT_CALLBACK_FLAG_PostEvents | EVENT_CALLBACK_FLAG_CleanupEvents | EVENT_CALLBACK_FLAG_LockEvents | EVENT_CALLBACK_FLAG_MergeEvents | EVENT_CALLBACK_FLAG_MessageEvents	| EVENT_CALLBACK_FLAG_BuildEvents | EVENT_CALLBACK_FLAG_CommandContainersEvents | EVENT_CALLBACK_FLAG_CriticalEventEvents | EVENT_CALLBACK_FLAG_RequestsEvents

	// Used with complex objects that require a handle
	typedef int MOGBridgeHandle;

	// Abstract class containing API for MOG
	class MOG_Export MOG_BridgeAPI
	{
	public:
		// Use this define to size any char arrays passed into MOG
		#define MAX_MOG_CharArrayLength							265

		virtual bool CheckAPIVersion(int version)=0;
		virtual bool CheckMajorVersion()=0;
		virtual bool CheckMinorVersion()=0;

		// Each returns true if initialization successful.
		virtual bool InitializeClient( const char *name, bool bUnused )=0;
		virtual bool InitializeCommandLine( const char *name, bool bUnused )=0;
		virtual bool InitializeEditor( const char *name, bool bUnused )=0;

		// 
		// Allows user to override the path where this MOG instance should be associated
		// This is only needed when the MOG instance is being executed outside of the Client's normal workspace directory and should be called prior to initializing the component
		virtual void SetAssociatedWorkspacePath( const char *path )=0;

		// 
		// Returns true if ProcessTick successfully executed.
		virtual bool ProcessTick( void )=0;

		//
		// Returns true if client is connected 
		virtual bool IsConnected( void )=0;

		//
		// Shutdown the bridge.  MOG_BridgeAPI will work without using this, but it is recommended that 
		//	you shutdown any bridge you instantiate.  This allows the MOG Server to recognize you
		//  have disconnected.
		virtual void Shutdown( void )=0;


		//
		//
		// Assigns your CommandHandler of format `bool MyHandler(char*)` to MOG's bridge.  
		//	It is recommended you return false if you were unable to complete the command.
		virtual void RegisterCommandHandler( bool (Handler)( struct MOGAPI_BaseInfo *pCommandType ) )=0;

		//
		// Assigns your EventHandler of format `void MyHandler(char*)` to MOG's bridge
		//  By default, user gets all Event Callbacks
		virtual void RegisterEventHandler( void (Handler)( struct MOGAPI_BaseInfo *pEventType ), 
					unsigned int iEventFlags=EVENT_CALLBACK_FLAG_All )=0;

		//
		// OR the event callback flags defined above into an unsigned int, then pass that int 
		//	into this function to receive callbacks for only those events.
		virtual void SetEventHandlerFlags( unsigned int iFlags )=0;
		virtual unsigned int GetEventHandlerFlags( void )=0;

		//
		// Utility Functions:
		//
		// Arguments: char pProjectName[MAX_MOG_CharArrayLength]
		virtual bool GetProjectName(OUT char *pProjectName)=0;

		//
		// Arguments: char pBranchName[MAX_MOG_CharArrayLength]
		virtual bool GetBranchName(OUT char *pBranchName)=0;

		//
		// Arguments: char pUserName[MAX_MOG_CharArrayLength]
		virtual bool GetUserName(OUT char *pUserName)=0;

		//
		// Arguments: char pLocalWorkspace[MAX_MOG_CharArrayLength]
		virtual bool GetLocalWorkspace(OUT char *pLocalWorkspace)=0;

		// These functions notify the server of alerts, errors, and exceptions
		// Arguments: char *pMessage 
		virtual bool NotifySystemAlert(IN const char *pTitle, IN const char *pMessage, IN const char *pStackTrace)=0;
		virtual bool NotifySystemError(IN const char *pTitle, IN const char *pMessage, IN const char *pStackTrace)=0;
		virtual bool NotifySystemException(IN const char *pTitle, IN const char *pMessage, IN const char *pStackTrace)=0;

		virtual bool AddLateResolvers( IN const char *PackageName, IN const char *LinkedPackage)=0;

		//
		// Arguments:...
		//virtual bool ResolveAssetNameFromGamedataFile(char *inSyncTargetFilename, char *outAssetName)=0;



		//
		// Asset Functions
		//
		// ImportAsset()
		//	Arguments:	char userName[MAX_MOG_CharArrayLength]
		//				char *pAssetName					; Intended name of the new asset
		//				char *pImportFiles[]				; Array of files to be imported as the new asset
		//				int importFilesLen					; Length of the pImportFiles array
		//				char *pLogFiles[]					; Array of log file to be copied into the new asset
		//				int logFilesLen						; Length of the pLogFiles array
		//				char *pProperties[]					; Array of properties to set in the new asset
		//				int propertiesLen					; Length of the pProperies array
		//				bool bDeleteImportFilesAfterImport	; Delete original import files after importation
		//				bool bDeleteLogFilesAfterImport		; Delete original log files after importation
		virtual bool ImportAsset(IN const char *pAssetName, IN const char *pImportFiles[], IN int importFilesLen, 
					IN const char *pLogFiles[], IN int logFilesLen, IN const struct MOGAPI_Property *pProperties[], 
					IN int propertiesLen, IN bool bDeleteImportFilesAfterImport, IN bool bDeleteLogFilesAfterImport)=0;

		// AddPendingImport()
		//  Used to inform MOG about objects needing to be imported into the Client
		//	Arguments:	char *pFilename						; Full path of the file being imported
		//				char *pAssetLabel					; Intended name of the new asset
		//				char *pPackageFile					; The package file where this asset is being imported
		//				char *pPackageGroupName				; The group within the package
		//				char *pPackageObjectName			; The containing object within the package
		//				char *lod							; The designated LOD for this object
		virtual bool AddPendingImport(IN const char *pFilename, IN const char *pAssetLabel, IN const char *pPackageFile, IN const char *pPackageGroupName, const char *pPackageObjectName, const char *pLOD, bool bExportedFromEditor)=0;

		// PerformPendingImports()
		//  Perform all the previously added imports
		virtual bool DoPendingImports()=0;



		//
		// Passed into PersistentLock... methods to be filled in by MOG
		struct LockInfo
		{
			#define MAX_ALREADY_LOCKED_BY_Name				50
			#define MAX_ALREADY_LOCKED_BY_Time				15
			#define MAX_ALREADY_LOCKED_BY_Description		1024

			enum LockInfo_Status
			{
				LockInfo_Default,		// This status should not be used.
				LockInfo_Successful,
				LockInfo_ClientNotLoggedIn,
				LockInfo_ObstructingLockFound,
				LockInfo_AssetDoesNotExist,
				LockInfo_ServerNotPresent,
				// Returned if MOG has more than one asset associated with assetName* passed in (below)
				LockInfo_MoreThanOneAssociatedAssetFound,
				// Returned if a MOG-internal exception was found
				LockInfo_FatalError,
			};

			// Member variables
			LockInfo_Status LockStatus;
			char AlreadyLockedBy[MAX_ALREADY_LOCKED_BY_Name];
			char AlreadyLockedByTime[MAX_ALREADY_LOCKED_BY_Time];
			char AlreadyLockedByDescription[MAX_ALREADY_LOCKED_BY_Description];
		};


		//
		// ResolveAssetNameForSyncTargetFile(...)
		//				char *pFileName						; File to look for
		//				char *pAssetName					; Character array [MAX_MOG_CharArrayLength]
		//													;  that will return the asset name
		//				LockInfo *info						; Struct containing information from lock, 
		//													;	you may pass in NULL.
		virtual bool ResolveAssetNameForSyncTargetFile(IN const char *pFileName, OUT char *pAssetName, LockInfo *info = NULL)=0;
		//
		// MOGBridgeHandle AssetSyncFiles_OpenObject(...)
		//				char *pAssetName					; AssetName to use
		//
		//	This function is used to obtain the various SyncFiles for a given asset.
		//	This uses an Open/Close API because complex asset could contain multiple files per asset and an API was needed for accessing the list of sync files.
		virtual MOGBridgeHandle AssetSyncFiles_OpenObject(IN const char *pAssetName)=0;
		virtual bool AssetSyncFiles_CloseObject(IN MOGBridgeHandle handle)=0;
		virtual int AssetSyncFiles_GetFileCount(IN MOGBridgeHandle handle)=0;
		virtual bool AssetSyncFiles_GetFilename(IN MOGBridgeHandle handle, IN int index, OUT char *pFilename)=0;
		//
		// NOTE: It is the responsibility of the calling party to use ResolveAssetNameForSyncTargetFile(...)
		//	to assure the correct filename.
		//
		// PersistentLock_Release(...)
		//				char *pAssetName					; Intended name of the new asset
		//				LockInfo *info						; Struct containing information from lock, 
		//													;	you may pass in NULL.
		virtual bool PersistentLock_Release(IN const char *pAssetName, OUT LockInfo *info = NULL)=0;
		//
		// PersistentLock_Request(...)
		//				char *pAssetName					; Intended name of the new asset
		//				char *pDescription					; Description of asset (arbitrary)
		//				LockInfo *info						; Struct containing information from lock, 
		//													;	you may pass in NULL.
		// 
		// Returns true if we can get the lock.  False if asset is already locked.
		virtual bool PersistentLock_Request(IN const char *pAssetName, IN const char *pDescription = NULL, OUT LockInfo *pInfo = NULL)=0;
		//
		// PersistentLock_Query(...) 
		//				char *pAssetName					; Intended name of the new asset
		//				LockInfo *info						; Struct containing information from lock, 
		//													;	you may pass in NULL.
		//
		// NOTE: Same functionality as PersistentLock_Request(...), except does not lock the asset
		virtual bool PersistentLock_Query(IN const char *pAssetName, OUT LockInfo *pInfo = NULL)=0;

		// Check if the the local file is out-of-date with the repository?
		virtual bool IsLocalFileOutofdate(IN const char *pFilename)=0;

		//
		// MOGBridgeHandle AssetSyncFiles_OpenObject(...)
		//				char *pAssetName					; AssetName to use
		//
		//	This function is used to obtain the various SyncFiles for a given asset.
		//	This uses an Open/Close API because complex asset could contain multiple files per asset and an API was needed for accessing the list of sync files.
		virtual MOGBridgeHandle AssetProperties_OpenObject(IN const char *pAssetName)=0;
		virtual bool AssetProperties_CloseObject(IN MOGBridgeHandle handle)=0;
		virtual bool AssetProperties_GetProperty(IN MOGBridgeHandle handle, IN MOGAPI_Property *property)=0;
		virtual bool AssetProperties_GetProperty(IN MOGBridgeHandle handle, IN const char *section, IN const char *propertySection, IN const char *propertyKey, OUT char *propertyValue)=0;
		virtual bool AssetProperties_GetProperty(IN MOGBridgeHandle handle, IN const char *section, IN int keyIndex, OUT char *key, OUT char *value)=0;
		virtual bool AssetProperties_GetPackageCommand_Add(IN MOGBridgeHandle handle, IN int fileIndex, IN int packageAssignmentIndex, OUT char *value)=0;
		virtual bool AssetProperties_GetPackageCommand_Remove(IN MOGBridgeHandle handle, IN int fileIndex, IN int packageAssignmentIndex, OUT char *value)=0;

		// Used to remove a relationship or package assignment of an asset when it is subsequently deleted from within an Editor
		virtual bool RemovePackageAssignment(IN const char *pAssetLabel, IN const char *pPackageFile, IN const char *pPackageGroupName, IN const char *pPackageObjectName, IN const char *LOD)=0;
		virtual bool RemoveRelationshipAssignment(IN const char *pAssetLabel, IN const char *pRelationship, IN const char *pRelatedAssetName, IN const char *pRelatedGroupName, IN const char *pRelatedObjectName)=0;

		// Used for the Generating and parsing of LOD identifiers within an asset's label
		virtual bool GenerateLODName(IN const char *pAssetLabel, IN const char *pLOD, OUT char *pName)=0;
		virtual bool DetectLODTagFromAssetLabel(IN const char *pAssetLabel, OUT char *pLODTag)=0;
		virtual bool StripLODTagFromAssetLabel(IN const char *pAssetLabel, OUT char *pStrippedAssetLabel)=0;
		virtual bool DetectLODFromAssetLabel(IN const char *pAssetLabel, OUT char *pLOD)=0;
		virtual bool DetectLODFromLODTag(IN const char *pLODTag, OUT char *pLOD)=0;

		// NOT IMPLEMENTED 
		virtual char *GetPermissions(void)=0;
		virtual void SetPermissions(const char *pPermissions)=0;

		// DLL Reporting Modes
		virtual void EnableSilentMode(bool enable)=0;
		virtual bool IsSilentMode()=0;
		virtual void EnableConsoleMode(bool enable)=0;
		virtual bool IsConsoleMode()=0;
		virtual void EnableLogFileMode(bool enable)=0;
		virtual bool IsLogFileMode()=0;
		virtual void SetLogFileName(const char* filename)=0;

		// Inbox Controls
		virtual bool InboxFetchFromRepository(IN const char *pBox, IN const char *pAssetName)=0;
		virtual bool InboxDelete(IN const char *pBox, IN const char *pAssetName)=0;
		virtual bool InboxRip(IN const char *pBox, IN const char *pAssetName)=0;
		virtual bool InboxReject(IN const char *pBox, IN const char *pAssetName, IN const char *pComment)=0;
		virtual bool InboxBless(IN const char *pBox, IN const char *pAssetName, IN const char *pComment)=0;
		virtual bool InboxMoveTo(IN const char *pBox, IN const char *pAssetName, IN const char *pComment, IN const char *pTargetUser)=0;
		virtual bool InboxCopyTo(IN const char *pBox, IN const char *pAssetName, IN const char *pComment, IN const char *pTargetUser)=0;


		// ================================================================================
		//	  NOTE:	Add new APIs here so we don't break backward compatibility!!!
		//			Eventually, MOG_API_VERSION will be incremented and we can 
		//			rearrange these APIs within the BridgeAPI.
		// ================================================================================

		// Arguments: char pLocalWorkspacePlatform[MAX_MOG_CharArrayLength]
		virtual bool GetLocalWorkspacePlatform(OUT char *pLocalWorkspacePlatform)=0;

	};

	//
	// Structure passed into MOG_BridgeAPI::ImportAsset()
	struct MOGAPI_Property
	{
		//TODO:  Add comments
		char *mSection, *mPropertySection, *mPropertyKey, *mPropertyValue;

		MOGAPI_Property()
		{
			mSection = NULL;
			mPropertySection = NULL;
			mPropertyKey = NULL;
			mPropertyValue = NULL;
		}

		MOGAPI_Property( const char *section, const char *propertySection, const char *propertyKey, const char *propertyValue )
		{
			mSection = DuplicateString(section);
			mPropertySection = DuplicateString(propertySection);
			mPropertyKey = DuplicateString(propertyKey);
			mPropertyValue = DuplicateString(propertyValue);
		}

		~MOGAPI_Property()
		{
			if( mSection )
			{
				delete[] mSection;
				mSection = NULL;
			}
			if( mPropertySection )
			{
				delete[] mPropertySection;
				mPropertySection = NULL;
			}
			if( mPropertyKey )
			{
				delete[] mPropertyKey;
				mPropertyKey = NULL;
			}
			if( mPropertyValue )
			{
				delete[] mPropertyValue;
				mPropertyValue = NULL;
			}
		}

		// Overloaded the safer APIs so they can be more easily modified for older integrations w/o these being available
#define MOG_USE_SAFE_STRING_API 0
		char* MOG_strcpy_s(char* strDestination, size_t len, const char* strSource)
		{
#if MOG_USE_SAFE_STRING_API
			strcpy_s(strDestination, len, strSource);
			return strDestination;
#else
			return strcpy(strDestination, strSource);
#endif
		}
		char* MOG_strcat_s(char* strDestination, size_t len, const char* strSource)
		{
#if MOG_USE_SAFE_STRING_API
			strcat_s(strDestination, len, strSource);
			return strDestination;
#else
			return strcat(strDestination, strSource);
#endif
		}
		char* MOG_strtok_s(char* strToken, const char* strDelimit, char** context)
		{
#if MOG_USE_SAFE_STRING_API
			return strtok_s(strToken, strDelimit, context);
#else
			return strtok(strToken, strDelimit);
#endif
		}

		char *DuplicateString(const char* str)
		{
			size_t len = strlen(str)+1;
			char *newStr = new char[len];
			MOG_strcpy_s(newStr, len, str);
			return newStr;
		}

		char *DuplicateString(char* str1, char* str2)
		{
			return DuplicateString(str1, str2, "/");
		}

		char *DuplicateString(const char* str1, const char* str2, const char *delimiter)
		{
			size_t len = strlen(str1)+strlen(delimiter)+strlen(str2)+1;
			char *newStr = new char[len];
			MOG_strcpy_s(newStr, len, str1);
			if( strlen(str2) > 0 )
			{
				MOG_strcat_s(newStr, len, delimiter);
				MOG_strcat_s(newStr, len, str2);
			}
			return newStr;
		}

		char *CombineAssignedPackageString(const char* packageName, const char* packageGroup, const char *packageObject)
		{
			const char *delim = "/";
			// input:	packageName = COWGame\Packages\Humans.upk
			//			packageGroup = Group/SubGroup
			//			packageGroup = PackageObj1/PackageObj2
			// output:	COWGame\Packages\Humans.upk/Group/SubGroup/(PackageObj1)/(PackageObj2)
			size_t packageNameLen = packageName != NULL ? strlen(packageName) : 0;
			size_t packageGroupLen = packageGroup != NULL ? strlen(packageGroup) : 0;
			size_t packageObjectLen = packageObject != NULL ? strlen(packageObject) : 0;
			
			// get a big buffer
			size_t size = 2 * (packageNameLen + packageGroupLen + packageObjectLen + 2);
			char *temp = new char[size];
			temp[0] = 0;
			if( packageNameLen > 0 )
			{
				MOG_strcat_s(temp, size, packageName);
			}

			if( packageGroupLen > 0 )
			{
				MOG_strcat_s(temp, size, "/");
				MOG_strcat_s(temp, size, packageGroup);
			}

			if( packageObjectLen > 0 )
			{
				size_t len = packageObjectLen + 1;
				char *local = new char[len];
				MOG_strcpy_s(local, len, packageObject);
				char* pszSecureContext;
				char *token = MOG_strtok_s(local, delim, &pszSecureContext);
				while( token != NULL )
				{
					MOG_strcat_s(temp, size, "/(");
					MOG_strcat_s(temp, size, token);
					MOG_strcat_s(temp, size, ")");

					token = MOG_strtok_s(NULL, delim, &pszSecureContext);
				}
				delete[] local;
			}

			size = strlen(temp) + 1;
			char *newStr = new char[size];
			MOG_strcpy_s(newStr, size, temp);

			delete[] temp;

			return newStr;
		}

		void Setup_AssetLink( const char *linkedFilename, const char *packageName, const char *packageGroup, const char *packageObject  )
		{
			mSection = DuplicateString("Relationships");
			mPropertySection = DuplicateString("AssetLink");
			mPropertyKey = CombineAssignedPackageString(packageName, packageGroup, packageObject);
			mPropertyValue = DuplicateString(linkedFilename);
		}
		void Setup_AssetReference( const char *referencedFilename, const char *packageName, const char *packageGroup, const char *packageObject  )
		{
			mSection = DuplicateString("Relationships");
			mPropertySection = DuplicateString("AssetReference");
			mPropertyKey = CombineAssignedPackageString(packageName, packageGroup, packageObject);
			mPropertyValue = DuplicateString(referencedFilename);
		}
		void Setup_PackageAssignment( const char *packageName, const char *packageGroup, const char *packageObject )
		{
			mSection = DuplicateString("Relationships");
			mPropertySection = DuplicateString("Packages");
			mPropertyKey = CombineAssignedPackageString(packageName, packageGroup, packageObject);
			mPropertyValue = DuplicateString("");
		}
		void Setup_AssetStatus( const char *status )
		{
			mSection = DuplicateString("Properties");
			mPropertySection = DuplicateString("Asset Stats");
			mPropertyKey = DuplicateString("Status");
			mPropertyValue = DuplicateString(status);
		}
	};

	//
	// Enumeration for MOGAPI_BaseInfo to tell user which class to 
	//	cast a MOGAPI_BaseInfo* into
	enum MOGAPI_CALLBACK_TYPE
	{
		MOGAPI_BaseType,
		MOGAPI_PackageMergeType,
		MOGAPI_LoginProjectType,
		MOGAPI_LoginUserType,
		MOGAPI_ViewType,
	};

	//
	// Contains basic information returned by all MOG callbacks
	struct MOGAPI_BaseInfo
	{
		// Type to cast into
		MOGAPI_CALLBACK_TYPE CallbackType;
		char *ProjectName;
		char *BranchName;
		char *CallbackDescription;
	};

	//
	// Information for PackageMerge callbacks
	struct MOGAPI_PackageMergeInfo : public MOGAPI_BaseInfo
	{
		// Full filename of source and destination
		char *SourceFile;
		char *DestinationFile;
	};

	//
	// Information for LoginUser callbacks
	struct MOGAPI_LoginUserInfo : public MOGAPI_BaseInfo
	{
		char *UserName;
	};

	//
	// Information for LoginProject callbacks
	struct MOGAPI_LoginProjectInfo : public MOGAPI_BaseInfo
	{
		//char *BranchName;
		//char *ProjectName;
	};

	//
	// Information for View callbacks
	struct MOGAPI_ViewInfo : public MOGAPI_BaseInfo
	{
		char *Tab;
		char *UserName;
		char *Platform;
		char *WorkingDirectory;
		char *Source;
		char *Destination;
		char *Description;
		char *SourceFile;
		char *DestinationFile;
	};

	//
	// The only way to get a MOG_BridgeAPI*
	typedef MOG_BridgeAPI* (*MogBridgeReturner)();
	MOG_Export MogBridgeReturner GetMogBridge();
}

#endif //__MOG_BRIDGEAPI_H__
