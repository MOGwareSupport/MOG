//--------------------------------------------------------------------------------
//	MOG_PLATFORM.h
//	
//	
//--------------------------------------------------------------------------------

#ifndef __MOG_PLATFORM_H__
#define __MOG_PLATFORM_H__

#include "MOG_Define.h"


namespace MOG {
namespace PLATFORM {


public __gc class MOG_Platform
{
public:
	String *mPlatformName;				// Platform name

	MOG_Platform()				 : mPlatformName("")	{}
	MOG_Platform(String* name)	 : mPlatformName(name)	{}
};


}
}

using namespace MOG::PLATFORM;

#endif	// __MOG_PLATFORM_H__
