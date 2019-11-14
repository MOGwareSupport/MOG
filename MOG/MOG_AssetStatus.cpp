//--------------------------------------------------------------------------------
//	MOG_AssetStatus.cpp
//	
//	
//--------------------------------------------------------------------------------

#include "StdAfx.h"


#include "MOG_AssetStatus.h"



MOG_AssetStatusInfo::MOG_AssetStatusInfo(MOG_AssetStatusType type, String *status, System::Drawing::Color color, StateIcon stateIconIndex, String *sound)
{
	Type = type;
	Status = status;
	Color = color;
	StateIconIndex = stateIconIndex;
	Sound = sound;
}


MOG_AssetStatusInfo *MOG_AssetStatus::GetInfo(MOG_AssetStatusType status)
{
	// Find the specified item
	for (int i = 0; i < AssetStatusInfos->Count; i++)
	{
		if (status == AssetStatusInfos[i]->Type)
		{
			return AssetStatusInfos[i];
		}
	}

	// Return our 'Unknown' status
	return AssetStatusInfos[0];
}

MOG_AssetStatusInfo *MOG_AssetStatus::GetInfo(String *status)
{
	// Find the specified item
	for (int i = 0; i < AssetStatusInfos->Count; i++)
	{
		if (String::Compare(status, AssetStatusInfos[i]->Status, true) == 0)
		{
			return AssetStatusInfos[i];
		}
	}

	// Return our 'Unknown' status
	return AssetStatusInfos[0];
}

