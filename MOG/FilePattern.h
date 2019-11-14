//--------------------------------------------------------------------------------
//	FilePattern.h
//	
//	
//--------------------------------------------------------------------------------

#pragma once
#using <mscorlib.dll>
#using <system.dll>

#include "string.h"

using namespace System;
using namespace System::Collections;


namespace MOG
{
	public __gc class FilePattern
	{
	private:
		ArrayList *mPatterns;

	public:
		FilePattern(String *patterns);

		String *ToString();
		void Add(String *newPattern);
		void Remove(String *removePattern);
		bool IsFilePatternMatch(String *item);
	};
}

using namespace MOG;
