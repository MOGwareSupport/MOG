//--------------------------------------------------------------------------------
//	MOG_Profile.cpp
//	
//	
//--------------------------------------------------------------------------------

#include "StdAfx.h"

#include "MOG_Time.h"

#include "MOG_Profile.h"



bool MOG_Profile::Profile(String *description)
{
	// Check if we need to create our mProfiles
	if (!mProfiles)
	{
		mProfiles = new ArrayList();
	}

	// Make sure we have a valid profile list?
	if (mProfiles)
	{
		// Create the time for this profile
		String *profile = String::Concat(S"(", MOG_Time::GetVersionTimestamp()->Substring(6), S")   ",  description);

		// Add this profile to our list
		mProfiles->Add(profile);

		return true;
	}

	return false;
}
