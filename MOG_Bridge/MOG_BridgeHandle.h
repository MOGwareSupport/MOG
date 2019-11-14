// MOG_BridgeHandle.h
//
// NOTE:


#ifndef __MOG_BRIDGEHANDLE_H__
#define __MOG_BRIDGEHANDLE_H__

#pragma once

using namespace System;
using namespace System::Collections;
using namespace System::Collections::Specialized;

using namespace MOG;
using namespace MOG::COMMAND;
using namespace MOG::ASSET_STATUS;
using namespace MOG::FILENAME;
using namespace MOG::PROPERTIES;


namespace MOG
{
	namespace BRIDGE
	{
		public __gc class MOG_Bridge_Object
		{
		public:
			MOG_Bridge_Object();

			MOGBridgeHandle GetHandle()					{ return mHandle; };

		private:
			MOGBridgeHandle mHandle;
		};


		public __gc class MOG_Bridge_AssetSyncFiles : public MOG_Bridge_Object
		{
		public:
			MOG_Bridge_AssetSyncFiles(MOG_Filename *assetFilename, String *platformName);
			ArrayList *GetSyncFileList()				{ return mSyncFileList; };

		private:
			ArrayList *mSyncFileList;
		};


		public __gc class MOG_Bridge_AssetProperties : public MOG_Bridge_Object
		{
		public:
			MOG_Bridge_AssetProperties(MOG_Filename *assetFilename, String *platformName);
			MOG_Properties *GetProperties()				{ return mProperties; };

		private:
			MOG_Properties *mProperties;
		};


		public __gc class MOG_Bridge_HandleTracker
		{
		public:
			static bool Register(MOG_Bridge_Object *object);
			static bool Unregister(MOGBridgeHandle handle);
			static bool Unregister(MOG_Bridge_Object *object);

			static MOG_Bridge_Object *GetObject(MOGBridgeHandle handle);

			static MOGBridgeHandle GetNextHandle()		{ return mHandleCount++; };

		private:
			static HybridDictionary *mHandles = new HybridDictionary();
			static MOGBridgeHandle mHandleCount = 1;

		};

	} // end BRIDGE
} // end MOG

using namespace MOG::BRIDGE;

#endif //__MOG_BRIDGEHANDLE_H__
