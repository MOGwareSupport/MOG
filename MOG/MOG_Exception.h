//--------------------------------------------------------------------------------
//	MOG_Exception.h
//	
//	
//--------------------------------------------------------------------------------
#ifndef __MOG_EXCEPTION_H__
#define __MOG_EXCEPTION_H__

namespace MOG {
namespace EXCEPTION {

public __gc class MOG_Exception : public Exception
{
public:

__value enum MOG_EXCEPTION_TYPE
{
	MOG_EXCEPTION_Generic,
	MOG_EXCEPTION_SystemInitFailed,
	MOG_EXCEPTION_UserNotLoggedIn,
	MOG_EXCEPTION_ProjectNotLoggedIn,
	MOG_EXCEPTION_BranchNotLoggedIn,
	MOG_EXCEPTION_NotInitialized,
	MOG_EXCEPTION_NotResponsible,
	MOG_EXCEPTION_InvalidUser,
	MOG_EXCEPTION_InvalidPlatform,
	MOG_EXCEPTION_InvalidAssetName,
	MOG_EXCEPTION_InvalidAssetClassification,
	MOG_EXCEPTION_InvalidAssetPlatform,
	MOG_EXCEPTION_InvalidAssetProject,
	MOG_EXCEPTION_InvalidComment,
	MOG_EXCEPTION_InvalidBlessedAsset,
	MOG_EXCEPTION_InvalidPath,
	MOG_EXCEPTION_InvalidBox,
	MOG_EXCEPTION_InvalidData,
	MOG_EXCEPTION_MissingData,
	MOG_EXCEPTION_MissingFile,
	MOG_EXCEPTION_NeedsProcessing,
	MOG_EXCEPTION_OpenAsset,
	MOG_EXCEPTION_IllegalOperation,
	MOG_EXCEPTION_InvalidWindowsName,
	MOG_EXCEPTION_OutstandingDependency,
	MOG_EXCEPTION_UnableToMoveAsset,
};

	MOG_Exception()
		: Exception(S"Generic Exception")
	{
		mType = MOG_EXCEPTION_Generic;
	}

	MOG_Exception(String *description)
		: Exception(description)
	{
		mType = MOG_EXCEPTION_Generic;
	}

	MOG_Exception(MOG_EXCEPTION_TYPE type, String *description)
		: Exception(description)
	{
		mType = type;
	}

	//MOG_Exception(MOG_EXCEPTION_TYPE type, String *description, Exception *ex)
	//	: Exception(description, ex)
	//{
	//	mType = type;
	//}

	MOG_EXCEPTION_TYPE GetType() { return mType; }

private:

	MOG_EXCEPTION_TYPE mType;
};

}
}

using namespace MOG::EXCEPTION;

#endif	// __MOG_EXCEPTION_H__
