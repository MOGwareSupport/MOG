// This is the main DLL file.  We can put whatever managed C++ stuff in here that we want

// MOG_Bridge has MOG_BridgeAPI.h in it
#include "MOG_Bridge.h"
#include "MOG_BridgeContainer.h"

#include <cstring>

#include "Mog_BridgeHandle.h"

using namespace MOG::CONTROLLER::CONTROLLERPROJECT;



bool MOG_Bridge_HandleTracker::Register(MOG_Bridge_Object *object)
{
	// Make sure we have a valid object?
	if (object &&
		object->GetHandle())
	{
		mHandles->Item[object->GetHandle().ToString()] = object;
		return true;
	}

	return false;
}


bool MOG_Bridge_HandleTracker::Unregister(MOG_Bridge_Object *object)
{
	return Unregister(object->GetHandle());
}


bool MOG_Bridge_HandleTracker::Unregister(MOGBridgeHandle handle)
{
	// Make sure we have a valid handle?
	if (handle)
	{
		mHandles->Remove(handle.ToString());
		return true;
	}

	return false;
}


MOG_Bridge_Object *MOG_Bridge_HandleTracker::GetObject(MOGBridgeHandle handle)
{
	return __try_cast<MOG_Bridge_Object *>(mHandles->Item[handle.ToString()]);
}


MOG_Bridge_Object::MOG_Bridge_Object()
{
	mHandle = MOG_Bridge_HandleTracker::GetNextHandle();
}



MOG_Bridge_AssetSyncFiles::MOG_Bridge_AssetSyncFiles(MOG_Filename *assetFilename, String *platformName)
{
	// Ask the project to resolve what asset this file belongs to?
	mSyncFileList = MOG_ControllerProject::MapAssetNameToFilename(assetFilename, platformName);
}


MOG_Bridge_AssetProperties::MOG_Bridge_AssetProperties(MOG_Filename *assetFilename, String *platformName)
{
	// Ask the project to resolve what asset this file belongs to?
	mProperties = new MOG_Properties(assetFilename);
	// Check if we have a platform specified?
	if (platformName->Length)
	{
		// Set the scope of our Properties to be platform specific
		mProperties->SetScope(platformName);
	}
}


