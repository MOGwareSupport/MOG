//--------------------------------------------------------------------------------
//	MOG_StringUtils.h
//	
//	
//--------------------------------------------------------------------------------

#ifndef __MOG_STRINGUTILS_H__
#define __MOG_STRINGUTILS_H__

namespace MOG {

bool MOG_StringCompare(String *string1, String *string2);

public __gc class StringUtils
{
public:
	static bool StringCompare(String *string1, String *string2) { return MOG_StringCompare(string1, string2);}
	static bool IsFiltered(String *assetFilename, String *exclusionList);
	static bool IsFiltered(String *assetFilename, String *exclusionList, String *inclusionList);
	static String *GetBestFilter(String *item, String *filterList);
	static int CompareStartsWithLikeness(String* string1, String* string2);
};

}

using namespace MOG;

#endif //__MOG_STRINGUTILS_H__
