//--------------------------------------------------------------------------------
//	MOG_Command.cpp
//	
//	
//--------------------------------------------------------------------------------

#include "StdAfx.h"

#include "MOG_Define.h"
#include "MOG_Time.h"
#include "MOG_ControllerProject.h"
#include "MOG_ControllerSystem.h"
#include "MOG_CommandFactory.h"

#include "MOG_Command.h"


using System::IO::MemoryStream;

using namespace System::IO;



MOG_Command::MOG_Command(void)
{
	Initialize();
}


void MOG_Command::Initialize(void)
{
	mBlocking = false;
	mCompleted = false;
	mRemoveDuplicateCommands = false;
	mExclusiveCommand = false;
	mPersistentLock = false;
	mPreserveCommand = false;
	mLicenseRequired = false;

	mCommandID = 0;
	mParentCommandID = 0;
	mCommandTimeStamp = MOG_Time::GetVersionTimestamp();
	mCommandType = MOG_COMMAND_None;
	mNetworkID = 0;
	mComputerIP = "";
	mComputerName = "";

	mProject = "";
	mBranch = "";
	mJobLabel = "";
	mTab = "";
	mUserName = "";
	mPlatform = "";
	mValidSlaves = "";

	mWorkingDirectory = "";
	mSource = "";
	mDestination = "";
	mTool = "";
	mVariables = "";
	mTitle = "";
	mDescription = "";
	mVersion = "";

	mOptions = "";

	mAssetFilename = new MOG_Filename("");

	mCommand = NULL;

	mAssignedSlaveID = 0;
}


bool MOG_Command::SetSystemDefaultCommandSettings()
{
	// We always need to populate certain information within the command...
	// Check to see what of this information might be missing?
	if (GetNetworkID() == 0)
	{
		SetNetworkID(MOG_ControllerSystem::GetNetworkID());
	}
	if (!GetComputerName()->Length)
	{
		// Use MOG_ControllerProject::GetComputerName() just in case we logged in aas a specific computer
		SetComputerName(MOG_ControllerProject::GetComputerName());
	}
	if (!GetComputerIP()->Length)
	{
		SetComputerIP(MOG_ControllerSystem::GetComputerIP());
	}
	// Make sure we always try to include the LoginProject
	if (!GetProject()->Length)
	{
		if (MOG_ControllerProject::IsProject())
		{
			SetProject(MOG_ControllerProject::GetProjectName());
		}
	}
	// Make sure we always try to include the LoginProject
	if (!GetBranch()->Length)
	{
		if (MOG_ControllerProject::IsBranch())
		{
			SetBranch(MOG_ControllerProject::GetBranchName());
		}
	}
	// Make sure we always try to include the LoginUser
	if (!GetUserName()->Length)
	{
		if (MOG_ControllerProject::IsUser())
		{
			SetUserName(MOG_ControllerProject::GetUserName());
		}
	}

	return true;
}


void MOG_Command::SetCommand(MOG_Command *command)
{
	if (command)
	{
		mCommand = new MOG_Command();
		mCommand->Clone(command);
	}
	else
	{
		mCommand = NULL;
	}
}


String *MOG_Command::ToString()
{
	String *name;
	__box MOG_COMMAND_TYPE* ovar = __box(mCommandType);

	name = ovar->ToString()->ToUpper()->Replace("MOG_COMMAND_", "");

	return name;
}


void MOG_Command::Clone(MOG_Command *sourceCommand)
{
	// Check if we have a bad command?
	if (this == NULL)
	{
		return;
	}

	mBlocking = sourceCommand->mBlocking;
	mCompleted = sourceCommand->mCompleted;
	mRemoveDuplicateCommands = sourceCommand->mRemoveDuplicateCommands;
	mExclusiveCommand = sourceCommand->mExclusiveCommand;
	mPersistentLock = sourceCommand->mPersistentLock;
	mPreserveCommand = sourceCommand->mPreserveCommand;
	mLicenseRequired = sourceCommand->mLicenseRequired;

	mCommandID = sourceCommand->mCommandID;
	mParentCommandID = sourceCommand->mParentCommandID;
	mCommandTimeStamp = sourceCommand->mCommandTimeStamp;
	mCommandType = sourceCommand->mCommandType;
	mNetworkID = sourceCommand->mNetworkID;
	mComputerIP = sourceCommand->mComputerIP;
	mComputerName = sourceCommand->mComputerName;

	mProject = sourceCommand->mProject;
	mBranch = sourceCommand->mBranch;
	mJobLabel = sourceCommand->mJobLabel;
	mTab = sourceCommand->mTab;
	mUserName = sourceCommand->mUserName;
	mPlatform = sourceCommand->mPlatform;
	mValidSlaves = sourceCommand->mValidSlaves;

	mWorkingDirectory = sourceCommand->mWorkingDirectory;
	mSource = sourceCommand->mSource;
	mDestination = sourceCommand->mDestination;
	mTool = sourceCommand->mTool;
	mVariables = sourceCommand->mVariables;
	mTitle = sourceCommand->mTitle;
	mDescription = sourceCommand->mDescription;
	mVersion = sourceCommand->mVersion;

	mOptions = sourceCommand->mOptions;

	mAssetFilename = new MOG_Filename(sourceCommand->mAssetFilename->GetOriginalFilename());

	mAssignedSlaveID = sourceCommand->mAssignedSlaveID;

	// Check if there is a contained command within this command
	if (sourceCommand->mCommand)
	{
		// Make sure we don't contain ourself?
		if (sourceCommand->mCommand == sourceCommand)
		{
			return;
		}
		// Check this contained command also has another contained command?
		if (sourceCommand->mCommand->mCommand)
		{
			// Make sure this contained command doesn't contain ourself?
			if (sourceCommand->mCommand->mCommand == sourceCommand &&
				sourceCommand->mCommand->mCommand == sourceCommand->mCommand)
			{
				return;
			}
		}

		// Clone this contained command
		mCommand = new MOG_Command();
		mCommand->Clone(sourceCommand->mCommand);
	}
	else
	{
		mCommand = NULL;
	}
}


NetworkPacket *MOG_Command::Serialize(void)
{
	NetworkPacket *packet = new NetworkPacket;
	// I have to make this bugger HUGE so we never have to worry about an overflow!!!
	// I hate using such a magic number, but we don't know how big this will be until it has been serialized
	// I wish .Net would allow us to have a nice dynamic byte array...Oh well
	packet->buffer = new Byte[1024 * 100];
	int offset = 0;

	// Add a place holder for the final size of the packet...to be added after SerializeCommand
	offset = SerializeInt32(packet, offset, 0);
	// Serialize the MOG_Command
	packet->size = SerializeCommand(packet, offset);
	// Write out the final packet size at the place holder
	SerializeInt32(packet, 0, packet->size);

#ifdef MOG_DEBUG_NETWORK
	// Used for tracking the network packets
	MOG_Report::LogComment(String::Concat(S"NETWORK - Serialized Command:", GetOriginalFilename(),S" ID=", GetCommandID().ToString(), S" Asset=", mAssetFilename->GetAssetFullName()));
#endif

	// .Net sucks because it won't allow me to dynamically size the Byte[] buffer during serialization
	// We now must manually copy the serialized data over to an correctly sized buffer
	Byte correctBuffer[] = new Byte[packet->size];
	for (int i = 0; i < packet->size; i++)
	{
		correctBuffer[i] = packet->buffer[i];
	}
	packet->buffer = correctBuffer;

	return packet;
}

int MOG_Command::SerializeCommand(NetworkPacket *packet, int offset)
{
//?	MOG_Command::SerializeCommand - Change Serialize/Deserialize over to the newer idea of using a dynamic array of strings for better flexibility and future compatibility
	// Serialize all the important member variables of this MOG_Command
	offset = SerializeInt32(packet, offset, Convert::ToInt32(MOG_MAJOR_VERSION));
	offset = SerializeInt32(packet, offset, Convert::ToInt32(mCommandType));
	offset = SerializeUInt32(packet, offset, Convert::ToUInt32(mCommandID));
	offset = SerializeUInt32(packet, offset, Convert::ToUInt32(mParentCommandID));
	offset = SerializeString(packet, offset, mCommandTimeStamp);

	offset = SerializeBool(packet, offset, mBlocking);
	offset = SerializeBool(packet, offset, mCompleted);
	offset = SerializeBool(packet, offset, mRemoveDuplicateCommands);
	offset = SerializeBool(packet, offset, mExclusiveCommand);
	offset = SerializeBool(packet, offset, mPersistentLock);
	offset = SerializeBool(packet, offset, mPreserveCommand);
	offset = SerializeBool(packet, offset, mLicenseRequired);

	offset = SerializeInt32(packet, offset, Convert::ToInt32(mNetworkID));
	offset = SerializeString(packet, offset, mComputerIP);
	offset = SerializeString(packet, offset, mComputerName);
	offset = SerializeString(packet, offset, mProject);
	offset = SerializeString(packet, offset, mBranch);
	offset = SerializeString(packet, offset, mJobLabel);
	offset = SerializeString(packet, offset, mTab);
	offset = SerializeString(packet, offset, mUserName);
	offset = SerializeString(packet, offset, mPlatform);
	offset = SerializeString(packet, offset, mValidSlaves);

	offset = SerializeString(packet, offset, mWorkingDirectory);
	offset = SerializeString(packet, offset, mSource);
	offset = SerializeString(packet, offset, mDestination);
	offset = SerializeString(packet, offset, mTool);
	offset = SerializeString(packet, offset, mVariables);
	offset = SerializeString(packet, offset, mTitle);
	offset = SerializeString(packet, offset, mDescription);
	offset = SerializeString(packet, offset, mVersion);

	offset = SerializeString(packet, offset, mOptions);

	offset = SerializeString(packet, offset, mAssetFilename->GetOriginalFilename());

	// Indicate in the serialized data whether we have a imbeded command
	bool bContainedCommand = (mCommand) ? true : false;
	offset = SerializeBool(packet, offset, bContainedCommand);
	if (bContainedCommand)
	{
		// Imbed this command into the packet
		offset = mCommand->SerializeCommand(packet, offset);
	}

	return offset;
}


MOG_Command *MOG_Command::Deserialize(NetworkPacket *packet)
{
	int offset = 0;
	Int32 size;

	// Initialize the command to 'Command_None' just in case the deserialization fails
	MOG_Command *newCmd = MOG_CommandFactory::Setup_None();

	// Get the overall length of the packet
	offset = DeserializeInt32(packet, offset, &size);
	// Deserialize the packet
	offset = newCmd->DeserializeCommand(packet, offset);

#ifdef MOG_DEBUG_NETWORK
	// Used for tracking the network packets
	MOG_Report::LogComment(String::Concat(S"NETWORK - Deserialized Command:", newCmd->ToString(),S" ID=", newCmd->GetCommandID().ToString(), S" Asset=", newCmd->mAssetFilename->GetAssetFullName()));
#endif

	return newCmd;
}


int MOG_Command::DeserializeCommand(NetworkPacket *packet, int offset)
{
	Int32 majorVersion;
	Int32 commandType;
	UInt32 commandID;
	UInt32 parentCommandID;
	Int32 networkID;
	String *assetName = "";
	bool bContainedCommand;

	// Attempt to get the major version of this command
	offset = DeserializeInt32(packet, offset, &majorVersion);
	if (majorVersion != MOG_MAJOR_VERSION)
	{
		// Skip to the end of this packet because it appears to be bad
		offset = packet->size;
		return offset;
	}

	// Continue to retreive the command
	offset = DeserializeInt32(packet, offset, &commandType);
	offset = DeserializeUInt32(packet, offset, &commandID);
	offset = DeserializeUInt32(packet, offset, &parentCommandID);
	offset = DeserializeString(packet, offset, &mCommandTimeStamp);

	offset = DeserializeBool(packet, offset, &mBlocking);
	offset = DeserializeBool(packet, offset, &mCompleted);
	offset = DeserializeBool(packet, offset, &mRemoveDuplicateCommands);
	offset = DeserializeBool(packet, offset, &mExclusiveCommand);
	offset = DeserializeBool(packet, offset, &mPersistentLock);
	offset = DeserializeBool(packet, offset, &mPreserveCommand);
	offset = DeserializeBool(packet, offset, &mLicenseRequired);

	offset = DeserializeInt32(packet, offset, &networkID);
	offset = DeserializeString(packet, offset, &mComputerIP);
	offset = DeserializeString(packet, offset, &mComputerName);
	offset = DeserializeString(packet, offset, &mProject);
	offset = DeserializeString(packet, offset, &mBranch);
	offset = DeserializeString(packet, offset, &mJobLabel);
	offset = DeserializeString(packet, offset, &mTab);
	offset = DeserializeString(packet, offset, &mUserName);
	offset = DeserializeString(packet, offset, &mPlatform);
	offset = DeserializeString(packet, offset, &mValidSlaves);

	offset = DeserializeString(packet, offset, &mWorkingDirectory);
	offset = DeserializeString(packet, offset, &mSource);
	offset = DeserializeString(packet, offset, &mDestination);
	offset = DeserializeString(packet, offset, &mTool);
	offset = DeserializeString(packet, offset, &mVariables);
	offset = DeserializeString(packet, offset, &mTitle);
	offset = DeserializeString(packet, offset, &mDescription);
	offset = DeserializeString(packet, offset, &mVersion);

	offset = DeserializeString(packet, offset, &mOptions);

	offset = DeserializeString(packet, offset, &assetName);

	// Check if we have a contained command?
	offset = DeserializeBool(packet, offset, &bContainedCommand);
	if (bContainedCommand)
	{
		mCommand = new MOG_Command;
		offset = mCommand->DeserializeCommand(packet, offset);
	}
	else
	{
		mCommand = NULL;
	}

	// Set the MOG_Filename along with the other special variables
	mAssetFilename = new MOG_Filename(assetName);
	SetCommandType((MOG_COMMAND_TYPE)commandType);
	SetCommandID(commandID);
	SetParentCommandID(parentCommandID);
	SetNetworkID(networkID);

	return offset;
}

