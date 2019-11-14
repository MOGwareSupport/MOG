//--------------------------------------------------------------------------------
//	MOG_AssetStatus.h
//	
//	
//--------------------------------------------------------------------------------
#pragma once
#using <mscorlib.dll>
#using <system.dll>

using namespace System;
using namespace System::Drawing;


namespace MOG
{
namespace ASSET_STATUS
{

public __value enum MOG_AssetStatusType {
	None,
	Unknown,

	// Drafts
	Importing,		// The state of an asset while it is being imported
	Imported,		// The state of an asset after it has been imported (Used by an editor to skip the auto update local)
	ImportError,	// Indicates there was an error during the importation of an asset
	Unprocessed,	// Indicates the asset needs to be processed/ripped
	Pending,		// The state of an asset while it is waiting to be scheduled for a rip
	Waiting,		// The state of an asset while it is waiting for an available slave
	Ripping,		// The state of an asset while it is actively being ripped
	Processed,		// Indicates the asset has been ripped and is ready for use
	RipError,		// Indicates an asset was not ripped correctly
	Renamed,		// The immeadiate state of an asset after it has been renamed
	Deleted,		// The state of the asset after it has been deleted and while it is located in a user's trash
	Modifying,		// The state of an asset while it is being modified.
	Modified,		// The immeadiate state of an asset after it has been modified

	// Local Workspace
	Copying,		// Indicates that an asset is in the process of being copied
	Copied,			// Indicates what assets have been copied to the local workspace
	Unpackaged,		// Indicates the recently added packaged assets still needing to be packaged in the local workspace
	Packaging,		// Indicates the asset is in the process of being packaged
	Repackage,		// Indicates what assets the user has requested be repackaged on the next local package merge
	Repackaging,	// Indicates the asset is in the process of being packaged
	Packaged,		// The finished state of an asset after it has been packaged in the local workspace
	Unpackage,		// Indicates when a packaged asset has been scheduled for removal from the local workspace.
	Unpackaging,	// Indicates the asset is in the process of being unpackaged
	PackageError,	// Indicates when a packaged asset caused a subsequent package error during a package merge

	// Inbox
	Sent,			// Indicates the asset was sent or copied to someone else's inbox
	Blessed,		// Indicates the asset was blessed to another user's inbox (i.e. BlessTarget)
	Unblessable,	// Indicates the asset is unblessable (most likely caused if the asset gets copied from another user's inbox)
	Rejected,		// Indicates the asset was rejected by a subequent bless target
	Rebuilt,		// REMOVE - This doesn't belong in AssetStatus because it is only used to refresh the views after they have been rebuilt

	// Repository
	Blessing,		// Indicates the asset is in the process of being blessed and is waiting to be posted
	Posted,			// The final state of the asset after it has been posted
	Created,		// The state of an asset that was created directly in the MOG Repository without being blessed
	CreationError,	// Indicated there was an error during the creation of an asset directly intot he MOG Repository
	Archived,		// The state of an asset that has been removed from the Repository and relocated into the Archive
};


// StatusIcons
public __value enum StateIcon
{
	Blank = -1,
	Unknown,

	Pending,
	Waiting,
	Working,
	Unprocessed,
	Alert,
	Error,

	AddToPackage,
	Repackage,
	RemoveFromPackage,
	Packaging,
	Packaged,
	PackageError,
	// WARNING - We can ONLY have a maximum of 15 state icons
	// New States also need to be added in the StateIconFiles list
};


public __gc class MOG_AssetStatusInfo
{
public:
	MOG_AssetStatusInfo(MOG_AssetStatusType type, String *status, Color color, StateIcon stateIconIndex, String *sound);

	MOG_AssetStatusType Type;
	String *Status;
	Color Color;
	StateIcon StateIconIndex;
	String *Sound;
};

public __gc class MOG_AssetStatus
{
public:
	// State Icons
	static String* GetStateIconFiles()[]						{ return StateIconFiles; };
	static MOG_AssetStatusInfo* GetAssetStatusInfos()[]			{ return AssetStatusInfos; };

	// Get By MOG_AssetStatusType
	static MOG_AssetStatusInfo *GetInfo(MOG_AssetStatusType status);
	static String *GetText(MOG_AssetStatusType status)			{ return GetInfo(status)->Status; };
	static Color GetColor(MOG_AssetStatusType status)			{ return GetInfo(status)->Color; };
	static int GetStateIconIndex(MOG_AssetStatusType status)	{ return GetInfo(status)->StateIconIndex; };
	static String *GetSound(MOG_AssetStatusType status)			{ return GetInfo(status)->Sound; };

	// Get By Status
	static MOG_AssetStatusInfo *GetInfo(String *status);
	static MOG_AssetStatusType GetType(String *status)			{ return GetInfo(status)->Type; };
	static Color GetColor(String *status)						{ return GetInfo(status)->Color; };
	static int GetStateIconIndex(String *status)				{ return GetInfo(status)->StateIconIndex; };
	static String *GetSound(String *status)						{ return GetInfo(status)->Sound; };

	static String *StateIconFiles[] = {
		"Unknown.bmp",						// Unknown

		"Pending.bmp",						// Pending
		"Waiting.bmp",						// Waiting
		"Processing.bmp",					// Working
		"Unprocessed.bmp",					// Unprocessed
		"RipError.bmp",						// Alert
		"RipError.bmp",						// Error

		"Packaged.bmp",						// AddToPackage
		"Repackage.bmp",					// Repackage
		"Unpackaged.bmp",					// RemoveFromPackage
		"Processing.bmp",					// Packaging
		"Package.bmp",						// Packaged
		"PackageErrorInvalidPackage.bmp",	// PackageError
	};


private:
	static MOG_AssetStatusInfo *AssetStatusInfos[] = {
		//						TYPE									TEXT				COLOR				STATE ICON						SOUND
		new MOG_AssetStatusInfo(MOG_AssetStatusType::None,				S"None",			Color::Black,		StateIcon::Blank,				S""),
		new MOG_AssetStatusInfo(MOG_AssetStatusType::Unknown,			S"Unknown",			Color::Black,		StateIcon::Unknown,				S""),

		// Drafts
		new MOG_AssetStatusInfo(MOG_AssetStatusType::Importing,			S"Importing",		Color::DarkRed,		StateIcon::Working,				S""),
		new MOG_AssetStatusInfo(MOG_AssetStatusType::Imported,			S"Imported",		Color::DarkGreen,	StateIcon::Blank,				S""),
		new MOG_AssetStatusInfo(MOG_AssetStatusType::ImportError,		S"ImportError",		Color::Red,			StateIcon::Error,				S""),
		new MOG_AssetStatusInfo(MOG_AssetStatusType::Unprocessed,		S"Unprocessed",		Color::DarkRed,		StateIcon::Unprocessed,			S""),
		new MOG_AssetStatusInfo(MOG_AssetStatusType::Pending,			S"Pending",			Color::Black,		StateIcon::Pending,				S""),
		new MOG_AssetStatusInfo(MOG_AssetStatusType::Waiting,			S"Waiting",			Color::DarkSalmon,	StateIcon::Waiting,				S""),
		new MOG_AssetStatusInfo(MOG_AssetStatusType::Ripping,			S"Ripping",			Color::DarkOrange,	StateIcon::Working,				S""),
		new MOG_AssetStatusInfo(MOG_AssetStatusType::Processed,			S"Processed",		Color::DarkGreen,	StateIcon::Blank,				S""),
		new MOG_AssetStatusInfo(MOG_AssetStatusType::RipError,			S"RipError",		Color::Red,			StateIcon::Error,				S""),
		new MOG_AssetStatusInfo(MOG_AssetStatusType::Deleted,			S"Deleted",			Color::Black,		StateIcon::Blank,				S""),
		new MOG_AssetStatusInfo(MOG_AssetStatusType::Modifying,			S"Modifying",		Color::DarkOrange,	StateIcon::Pending,				S""),
		new MOG_AssetStatusInfo(MOG_AssetStatusType::Modified,			S"Modified",		Color::DarkRed,		StateIcon::Unprocessed,			S""),

		// Local Workspace
		new MOG_AssetStatusInfo(MOG_AssetStatusType::Copying,			S"Copying",			Color::DarkOrange,	StateIcon::Working,				S""),
		new MOG_AssetStatusInfo(MOG_AssetStatusType::Copied,			S"Copied",			Color::DarkBlue,	StateIcon::Blank,				S""),
		new MOG_AssetStatusInfo(MOG_AssetStatusType::Unpackaged,		S"Unpackaged",		Color::Salmon,		StateIcon::AddToPackage,		S""),
		new MOG_AssetStatusInfo(MOG_AssetStatusType::Packaging,			S"Packaging",		Color::DarkOrange,	StateIcon::Packaging,			S""),
		new MOG_AssetStatusInfo(MOG_AssetStatusType::Repackage,			S"Repackage",		Color::Salmon,		StateIcon::Repackage,			S""),
		new MOG_AssetStatusInfo(MOG_AssetStatusType::Repackaging,		S"Repackaging",		Color::DarkOrange,	StateIcon::Packaging,			S""),
		new MOG_AssetStatusInfo(MOG_AssetStatusType::Packaged,			S"Packaged",		Color::DarkBlue,	StateIcon::Packaged,			S""),
		new MOG_AssetStatusInfo(MOG_AssetStatusType::Unpackage,			S"Unpackage",		Color::Salmon,		StateIcon::RemoveFromPackage,	S""),
		new MOG_AssetStatusInfo(MOG_AssetStatusType::Unpackaging,		S"Unpackaging",		Color::DarkOrange,	StateIcon::Packaging,			S""),
		new MOG_AssetStatusInfo(MOG_AssetStatusType::PackageError,		S"PackageError",	Color::Red,			StateIcon::PackageError,		S""),
		// Inbox
		new MOG_AssetStatusInfo(MOG_AssetStatusType::Sent,				S"Sent",			Color::Black,		StateIcon::Blank,				S""),
		new MOG_AssetStatusInfo(MOG_AssetStatusType::Blessed,			S"Blessed",			Color::DarkBlue,	StateIcon::Blank,				S""),
		new MOG_AssetStatusInfo(MOG_AssetStatusType::Unblessable,		S"Unblessable",		Color::Gray,		StateIcon::Blank,				S""),
		new MOG_AssetStatusInfo(MOG_AssetStatusType::Rejected,			S"Rejected",		Color::Red,			StateIcon::Alert,				S""),
		new MOG_AssetStatusInfo(MOG_AssetStatusType::Rebuilt,			S"Rebuilt",			Color::Black,		StateIcon::Blank,				S""),

		// Repository
		new MOG_AssetStatusInfo(MOG_AssetStatusType::Blessing,			S"Blessing",		Color::DarkOrange,	StateIcon::Working,				S""),
		new MOG_AssetStatusInfo(MOG_AssetStatusType::Posted,			S"Posted",			Color::Black,		StateIcon::Blank,				S""),
		new MOG_AssetStatusInfo(MOG_AssetStatusType::Created,			S"Created",			Color::Black,		StateIcon::Blank,				S""),
		new MOG_AssetStatusInfo(MOG_AssetStatusType::CreationError,		S"CreationError",	Color::Red,			StateIcon::Error,				S""),
		new MOG_AssetStatusInfo(MOG_AssetStatusType::Archived,			S"Archived",		Color::Black,		StateIcon::Blank,				S""),
	};
};

}
}

using namespace MOG::ASSET_STATUS;

