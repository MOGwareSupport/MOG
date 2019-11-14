//--------------------------------------------------------------------------------
//	MOG_Define.h
//	
//	
//--------------------------------------------------------------------------------
#ifndef __MOG_DEFINE_H__
#define __MOG_DEFINE_H__

#include "stdafx.h"

#using <mscorlib.dll>
#using <system.dll>

using namespace System;
using namespace System::Collections;
using namespace System::IO;
using System::Collections::ArrayList;
using System::Net::IPHostEntry;
using System::Net::IPAddress;
using System::Net::Dns;


#include "MOG_Report.h"
#include "MOG_Prompt.h"
#include "MOG_Exception.h"


#ifndef NULL
#define NULL 0;
#endif

namespace MOG {


#define MOG_MAJOR_VERSION	4		// Requires ALL clients to update their version
#define MOG_MINOR_VERSION	0		// Recommends clients should update their version
#define MOG_SYSTEM_CONFIGPATH		S"Tools"
#define MOG_SYSTEM_CONFIGFILENAME	S"MOGConfig.ini"
#define MOG_SYSTEM_RELATIVECONFIG	String::Concat(MOG_SYSTEM_CONFIGPATH, S"\\", MOG_SYSTEM_CONFIGFILENAME)
#define MOG_SYSTEM_CONFIG			String::Concat(S"{SystemRepositoryPath}", S"\\", MOG_SYSTEM_RELATIVECONFIG)

#define INVALID_WINDOWSPATH_CHARACTERS		S"/*?<>|"
#define INVALID_WINDOWSFILENAME_CHARACTERS	S"/\\:*?\"<>|"
#define INVALID_MOG_CHARACTERS				S"/\\:*?\"<>|[]`,$^%#="

// Debugging Defines
//#define MOG_DEBUG_NETWORK	1

// This is used to help track down orphaned MOG objects that get destructed without being closing properly
#if 0
	#define MOG_Environment_StackTrace			Environment::StackTrace
#else
	#define MOG_Environment_StackTrace			S"#define MOG_Environment_StackTrace is turned off in MOG_Define.h"
#endif

#define MOG_STANDARD_TIMESTAMP		String::Concat(TOKEN_Month_2, S"/", TOKEN_Day_2, S"/", TOKEN_Year_4, S" ", TOKEN_Hour_2, S":", TOKEN_Minute_2, S":", TOKEN_Second_2, S" ", TOKEN_AMPM)


#define MOG_ASSERT(condition, description)												\
	if (!(condition))																	\
	{																					\
		MOG_Report::ReportMessage("MOG_Assert", description, Environment::StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR); \
	}

#define MOG_ASSERT_THROW(condition, type, description)									\
	if (!(condition))																	\
	{																					\
		MOG_Exception *exception = new MOG_Exception(type, description);				\
		throw exception;																\
	}

//#define MOG_ASSERT_THROW_EX(condition, type, description, innerException)				\
//	if (!(condition))																	\
//	{																					\
//		MOG_Exception *exception = new MOG_Exception(type, description, innerException);\
//		throw exception;																\
//	}
}

using namespace MOG;

#endif	// __MOG_DEFINE_H__


