//--------------------------------------------------------------------------------
//	MOG_Command.h
//	
//	
//--------------------------------------------------------------------------------

#ifndef __MOG_COMMAND_H__
#define __MOG_COMMAND_H__

#include "MOG_Filename.h"
#include "MOG_NetworkSocket.h"
#include "MOG_AssetStatus.h"

#using <System.Xml.dll>

using namespace System::Xml;
using namespace System::Xml::Serialization;



namespace MOG {
namespace COMMAND {


public __value enum MOG_COMMAND_TYPE
{
	MOG_COMMAND_None,

	MOG_COMMAND_ConnectionNew,				// Used to establish new connections with the Server
	MOG_COMMAND_ConnectionLost,				// Used to indicate that a connection to the Server was lost
	MOG_COMMAND_ConnectionKill,				// Used to Kill a specific connection to the Server

	MOG_COMMAND_RegisterClient,				// Used by Clients to register them with the Server
	MOG_COMMAND_ShutdownClient,				// Used by Clients to shut down
	MOG_COMMAND_RegisterSlave,				// Used by Slaves to register them with the Server
	MOG_COMMAND_ShutdownSlave,				// Used by Slaves to shut down
	MOG_COMMAND_RegisterEditor,				// Used by Editors to register them with the Server
	MOG_COMMAND_ShutdownEditor,				// Used by Editors to shut down
	MOG_COMMAND_RegisterCommandLine,		// Used by CommandLines to register them with the Server
	MOG_COMMAND_ShutdownCommandLine,		// Used by CommandLines to shut down

	MOG_COMMAND_SQLConnection,				// Used to specifiy the new SQL Connection information
	MOG_COMMAND_MOGRepository,				// Used to Specify the MOG Repository

	MOG_COMMAND_License,					// Indicate Mog license information
	MOG_COMMAND_LoginProject,				// Used by Clients to inform the Server when they login to a project
	MOG_COMMAND_LoginUser,					// Used by Clients to inform the Server who they are logged in as

	MOG_COMMAND_ActiveViews,				// Used by the Client to inform the Server about their active views
	MOG_COMMAND_ViewUpdate,					// ?
	MOG_COMMAND_ViewLock,					// ?
	MOG_COMMAND_ViewConnection,				// ?
	MOG_COMMAND_TaskUpdate,					// Information concerning updated tasks

	MOG_COMMAND_RefreshApplication,			// Used to force Clients to restart so they will update their executables
	MOG_COMMAND_RefreshTools,				// Used to force Clients to refresh their tools
	MOG_COMMAND_RefreshProject,				// Used to force Clients to refresh their project

	MOG_COMMAND_AssetRipRequest,			// Used by a CLient to start a Rip process on an Asset
	MOG_COMMAND_AssetProcessed,				// Used to inform a Client that an Asset has been processed
	MOG_COMMAND_SlaveTask,					// Used by Slaves to perform a TaskFile during the Rip process

	MOG_COMMAND_ReinstanceAssetRevision,	// Used to reinstance a blessed asset

	MOG_COMMAND_Bless,						// Used to bless an Asset into the master repository

	MOG_COMMAND_AssetWatch,					// Indicates to the user when a specific asset has been modified in the Mog repository

	MOG_COMMAND_RemoveAssetFromProject,		// Used to schedule an Asset for removal from the project

	MOG_COMMAND_LocalPackageRebuild,		// Used to schedule the rebuilding of local package(s)
	MOG_COMMAND_NetworkPackageMerge,		// Used to schedule a NetworkPackageMerge
	MOG_COMMAND_LocalPackageMerge,			// Used to schedule a LocalPackageMerge
	MOG_COMMAND_EditorPackageMergeTask,		// Used to inform the Editor about a PackageMergeTask
	MOG_COMMAND_NetworkPackageRebuild,		// Used to schedule the rebuilding of package(s)

	MOG_COMMAND_Post,						// Used by a Slave to post new Assets after a bless has completed

	MOG_COMMAND_Archive,					// Used to begin the Archive process
	MOG_COMMAND_CleanTrash,					// Used by a Slave to clean all Trash throughout the System

	MOG_COMMAND_LockCopy,					// Used by a Client to move a file using the appropriate locks
	MOG_COMMAND_LockMove,					// Used by a Client to move a file using the appropriate locks
	MOG_COMMAND_LockReadRequest,			// Used by a Client to request a ReadLock
	MOG_COMMAND_LockReadRelease,			// Used by a Client to release a ReadLock
	MOG_COMMAND_LockWriteRequest,			// Used by a Client to request a WriteLock
	MOG_COMMAND_LockWriteRelease,			// Used by a Client to release a WriteLock
	MOG_COMMAND_LockReadQuery,				// Used by a Client to query wether a particular ReadLock is free
	MOG_COMMAND_LockWriteQuery,				// Used by a Client to query wether a particular WriteLock is free
	MOG_COMMAND_LockRequestRelease,			// Used by a Client to ask any active lock holder if they could release the lock immeadiately
	MOG_COMMAND_LockPersistentNotify,		// Used to notify Clients of all active PersistentLocks
	MOG_COMMAND_LockWatch,					// Used by a Client so they will be informed when a lock becomes available

	MOG_COMMAND_GetEditorInfo,				// Used to obtain Editor information from the server

	MOG_COMMAND_NetworkBroadcast,			// Send a 'Net Send' style message to any connection

	MOG_COMMAND_BuildFull,					// Used to request a full build
	MOG_COMMAND_Build,						// Used to request a build

	MOG_COMMAND_NewBranch,					// Used to trigger the creation of a new branch

	MOG_COMMAND_InstantMessage,				// Used to send an instant message to another logged in user

	MOG_COMMAND_Complete,					// Used to encapsulate any command to indicate that it was completed
	MOG_COMMAND_Postpone,					// Used to encapsulate any command that needs to be postponed for later processing
	MOG_COMMAND_Failed,						// Used to encapsulate any command to indicate that it failed to be processed

	MOG_COMMAND_NotifySystemAlert,			// Notify the Server of a System Alert
	MOG_COMMAND_NotifySystemError,			// Notify the Server of a System Error
	MOG_COMMAND_NotifySystemException,		// Notify the Server of a System Exception

	MOG_COMMAND_RequestActiveCommands,		// Used to request the active commands waiting to be processed from the Server
	MOG_COMMAND_NotifyActiveCommand,		// Used by the Server to send each active command waiting to be processed

	MOG_COMMAND_RequestActiveLocks,			// Used to request the active locks waiting to be processed from the Server
	MOG_COMMAND_NotifyActiveLock,			// Used by the Server to send each active lock on the Server

	MOG_COMMAND_RequestActiveConnections,	// Used to request the active connections waiting to be processed from the Server
	MOG_COMMAND_NotifyActiveConnection,		// Used by the Server to send each active connection to the Server

	MOG_COMMAND_ScheduleArchive,			// Used by a Slave to schedule an asset for Archive

	MOG_COMMAND_AutomatedTesting,			// Used to inform a client to launch a Slave

	MOG_COMMAND_RetaskCommand,				// Used to inform the server to retask a specific command
	MOG_COMMAND_KillCommand,				// Used to inform the server to kill a specific command

	MOG_COMMAND_LaunchSlave,				// Used to inform a client to launch a Slave

	MOG_COMMAND_StartJob,					// Used to start processing job related commands

//==================================================================================
// TODO - Move these commands to a better spot next time we change the MajorRevision

//==================================================================================

	// Always add new commands above this line
	MOG_COMMAND_TOTALTYPES
};


//typedef enum MOG_CPU_TYPE {
//	MOG_CPU_NA,
//	MOG_CPU_P1,
//	MOG_CPU_P2,
//	MOG_CPU_P3,
//	MOG_CPU_DURON,
//	MOG_CPU_P4,
//	MOG_CPU_ATHLON,
//} MOG_CPU_TYPE;


[Serializable]
public __gc class MOG_Command
{
public:
	MOG_Command(void);

	void Initialize(void);
	bool SetSystemDefaultCommandSettings();

	//MOG_Command *Clone(void);
	void Clone(MOG_Command *sourceCommand);

	// Serialize command into a NetworkPacket
	NetworkPacket *Serialize(void);
	int SerializeCommand(NetworkPacket *packet, int offset);
	static MOG_Command *Deserialize(NetworkPacket *packet);
	int DeserializeCommand(NetworkPacket *packet, int offset);

	// Access Functions
	void SetCommandType(MOG_COMMAND_TYPE commandType)				{ mCommandType = commandType; };
	MOG_COMMAND_TYPE GetCommandType(void)							{ return mCommandType; };
	String *ToString(void);

	void SetCommandID(UInt32 commandID)								{ mCommandID = commandID; };
	UInt32 GetCommandID(void)										{ return mCommandID; };

	void SetParentCommandID(UInt32 commandID)						{ mParentCommandID = commandID; };
	UInt32 GetParentCommandID(void)									{ return mParentCommandID; };

	void SetCommandTimeStamp(String *commandTimeStamp)				{ mCommandTimeStamp = commandTimeStamp; };
	String *GetCommandTimeStamp(void)								{ return mCommandTimeStamp; };

	bool IsCompleted(void)											{ return mCompleted; };
	void SetCompleted(bool completed)								{ mCompleted = completed; };

	bool IsBlocking(void)											{ return mBlocking; };
	void SetBlocking(bool blocked)									{ mBlocking = blocked; };

	bool IsRemoveDuplicateCommands(void)							{ return mRemoveDuplicateCommands; };
	void SetRemoveDuplicateCommands(bool removeDuplicateCommands)	{ mRemoveDuplicateCommands = removeDuplicateCommands; };

	bool IsExclusiveCommand(void)									{ return mExclusiveCommand; };
	void SetExclusiveCommand(bool exclusive)						{ mExclusiveCommand = exclusive; };

	bool IsPersistentLock(void)										{ return mPersistentLock; };
	void SetPersistentLock(bool check)								{ mPersistentLock = check; };

	bool IsPreserveCommand(void)									{ return mPreserveCommand; };
	void SetPreserveCommand(bool check)								{ mPreserveCommand = check; };

	bool IsLicenseRequired(void)									{ return mLicenseRequired; };

	void SetNetworkID(int networkID)								{ mNetworkID = networkID; };
	int GetNetworkID(void)											{ return mNetworkID; };

	void SetComputerIP(String *computerIP)							{ mComputerIP = computerIP; };
	String *GetComputerIP(void)										{ return mComputerIP; };

	void SetComputerName(String *computerName)						{ mComputerName = computerName; };
	String *GetComputerName(void)									{ return mComputerName; };

	void SetProject(String *project)								{ mProject = project; };
	String *GetProject(void)										{ return mProject; };

	void SetBranch(String *branch)									{ mBranch = branch; };
	String *GetBranch(void)											{ return mBranch; };

	void SetJobLabel(String *jobLabel)								{ mJobLabel = jobLabel; };
	String *GetJobLabel(void)										{ return mJobLabel; };

	void SetTab(String *tab)										{ mTab = tab; };
	String *GetTab(void)											{ return mTab; };

	void SetUserName(String *userName)								{ mUserName = userName; };
	String *GetUserName(void)										{ return mUserName; };

	void SetPlatform(String *platform)								{ mPlatform = platform; };
	String *GetPlatform(void)										{ return mPlatform; };

	void SetValidSlaves(String *validSlaves)						{ mValidSlaves = validSlaves; };
	String *GetValidSlaves(void)									{ return mValidSlaves; };

	void SetWorkingDirectory(String *workingDirectory)				{ mWorkingDirectory = workingDirectory; };
	String *GetWorkingDirectory(void)								{ return mWorkingDirectory; };

	void SetSource(String *source)									{ mSource = source; } ;
	String *GetSource(void)											{ return mSource; };

	void SetDestination(String *destination)						{ mDestination = destination; };
	String *GetDestination(void)									{ return mDestination; };

	void SetTool(String *tool)										{ mTool = tool; };
	String *GetTool(void)											{ return mTool; };

	void SetVariables(String *variables)							{ mVariables = variables; };
	String *GetVeriables(void)										{ return mVariables; };

	void SetTitle(String *title)									{ mTitle = title; };
	String *GetTitle(void)											{ return mTitle; };

	void SetDescription(String *description)						{ mDescription = description; };
	String *GetDescription(void)									{ return mDescription; };

	void SetVersion(String *version)								{ mVersion = version; };
	String *GetVersion(void)										{ return mVersion; };

	void SetOptions(String *options)								{ mOptions = options; };
	String *GetOptions(void)										{ return mOptions; };

	void SetAssetFilename(String *assetFilename)					{ mAssetFilename->SetFilename(assetFilename); };
	MOG_Filename *GetAssetFilename(void)							{ return mAssetFilename; };

//	void SetCPU(String *cpu)										{ mCPU = cpu; };
//	String *GetCPU(void)											{ return mCPU; };

//	void  SetCPUType(MOG_CPU_TYPE cpuType)							{ mCPUType = cpuType; };
//	MOG_CPU_TYPE GetCPUType(void)									{ return mCPUType; };

//	void SetMhz(long mhz)											{ mMhz = mhz; };
//	long GetMhz(void)												{ return mMhz; };

	void SetCommand(MOG_Command *command);
	MOG_Command *GetCommand(void)									{ return mCommand; };

	void SetAssignedSlaveID(int id)									{ mAssignedSlaveID = id; };
	int GetAssignedSlaveID(void)									{ return mAssignedSlaveID; };

//private:
public:
	MOG_COMMAND_TYPE mCommandType;
	UInt32 mCommandID;
	UInt32 mParentCommandID;
	String *mCommandTimeStamp;
//	String *mDelayedExecutionTime;

	bool mBlocking;					// This command will be sent to the server blocking
	bool mCompleted;				// Successful execution
	bool mRemoveDuplicateCommands;	// Check all pending commands and remove any duplicates
	bool mExclusiveCommand;			// Only allow one of these commands to be processed at a time
	bool mPersistentLock;			// Lock survives a client shutdown and is kept till the lock is released by the locked owner
	bool mPreserveCommand;			// Save this command into the database
	bool mLicenseRequired;			// does command require a license

	int mNetworkID;
	String *mComputerIP;
	String *mComputerName;
	String *mProject;
	String *mBranch;
	String *mJobLabel;
	String *mTab;
	String *mUserName;
	String *mPlatform;
	String *mValidSlaves;

	String *mWorkingDirectory;
	String *mSource;
	String *mDestination;
	String *mTool;
	String *mVariables;
	String *mTitle;
	String *mDescription;
	String *mVersion;

	String *mOptions;

	MOG_Filename *mAssetFilename;

//? MOG_Command - Should add the CPU information included with the register slave command
//	String *mCPU;
//	MOG_CPU_TYPE mCPUType;
//	long mMhz;

	MOG_Command *mCommand;
	int mAssignedSlaveID;
};

}
}

using namespace MOG::COMMAND;

#endif	// __MOG_COMMAND_H__

