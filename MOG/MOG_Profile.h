//--------------------------------------------------------------------------------
//	MOG_Profile.h
//	
//	
//--------------------------------------------------------------------------------

#ifndef __MOG_PROFILE_H__
#define __MOG_PROFILE_H__

#include "MOG_Main.h"
#include "MOG_INI.h"
#include "MOG_Filename.h"


namespace MOG {
namespace PROFILE {


public __gc class MOG_Profile
{
public:
	static bool Profile(String *description);

private:
	static ArrayList *mProfiles;
};


} // end ns PROFILE
} // end ns MOG

using namespace MOG::PROFILE;

#endif	// __MOG_PROFILE_H__




