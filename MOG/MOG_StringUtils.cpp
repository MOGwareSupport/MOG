//--------------------------------------------------------------------------------
//	MOG_StringUtils.cpp
//	
//	
//--------------------------------------------------------------------------------

#include "stdafx.h"

#include "MOG_Define.h"
#include "MOG_StringUtils.h"
#include <malloc.h>
#include <string.h>


//************************************************************************
//************************************************************************
bool MOG_StringCheckEqual(String *large, String *small)
{
	// Make a local copy because this will muck around with the guts of both strings!
	// Convert both strings to lower
	String *llarge = large->ToLower();
	String *lsmall = small->ToLower();

	int bigIndex = 0;
	int bigEndIndex = llarge->Length;
	int littleIndex = 0;
	int littleEndIndex = lsmall->Length;

	bool failed = false;

	while(bigIndex < bigEndIndex && littleIndex < littleEndIndex && !failed)
	{
		__wchar_t littleChar = lsmall->Chars[littleIndex];
		__wchar_t bigChar = llarge->Chars[bigIndex];

		// advance both strings?
		if (bigChar == littleChar || bigChar == '?' || littleChar == '?' || (bigChar == '*' && littleChar == '*'))
		{
			bigIndex++;
			littleIndex++;
		}
		// little string has a wild card?
		else if(littleChar == '*')
		{
			// Does the little string end with a '*'?
			if (littleIndex + 1 == littleEndIndex)
			{
				// Eat the rest of all the characters
				bigIndex = bigEndIndex;
				littleIndex = littleEndIndex;
			}
			// advance only the big string?
			else if (lsmall->Chars[littleIndex + 1] != bigChar)
			{
				bigIndex++;
			}
			// does the next character match?...move past the '*'?
			else if (lsmall->Chars[littleIndex + 1] == bigChar)
			{
				// JohnRen - I don't like this since the following test would fail
				// "abc*bcabc" "abcabcabcabc"
				// Might be better to be more picky about when we drop out of this wild card check
				littleIndex++;
			}
		}
		// big string has a wild card?
		else if (bigChar == '*')
		{
			// Does the big string end with a '*'?
			if (bigIndex + 1 == bigEndIndex)
			{
				// Eat the rest of all the characters
				bigIndex = bigEndIndex;
				littleIndex = littleEndIndex;
			}
			// advance only the little string
			else if (llarge->Chars[bigIndex + 1] != littleChar)
			{
				littleIndex++;
			}
			// does the next character match?...move past the '*'?
			else if (llarge->Chars[bigIndex + 1] == littleChar)
			{
				// JohnRen - I don't like this since the following test would fail
				// "abc*bcabc" "abcabcabcabc"
				// Might be better to be more picky about when we drop out of this wild card check
				bigIndex++;
			}
		}
		// otherwise, the compare fails
		else
		{
			failed = true;
		}
	}

	// JohnRen - This might never need to be called now that I check for '*' as the last character of the strings.
	if (!failed && bigIndex == bigEndIndex && littleIndex != littleEndIndex)
	{
		while(littleIndex < littleEndIndex && !failed)
		{
			if (lsmall->Chars[littleIndex] != '*')
			{
				failed = true;
			}
			else
			{
				littleIndex++;
			}
		}
	}

	// JohnRen - This might never need to be called now that I check for '*' as the last character of the strings.
	if (!failed && littleIndex == littleEndIndex && bigIndex != bigEndIndex)
	{
		while(bigIndex < bigEndIndex && !failed)
		{
			if(llarge->Chars[bigIndex] != '*')
			{
				failed = true;
			}
			else
			{
				bigIndex++;
			}
		}
	}

	return !failed;
}

//************************************************************************
//************************************************************************
bool MOG::MOG_StringCompare(String *string1, String *string2)
{
	if ( string1->Length > string2->Length )
	{
		return(MOG_StringCheckEqual(string1, string2));
	}
	else
	{
		return(MOG_StringCheckEqual(string2, string1));
	}
}


bool StringUtils::IsFiltered(String *assetFilename, String *exclusionList)
{
	return StringUtils::IsFiltered(assetFilename, exclusionList, "");
}


bool StringUtils::IsFiltered(String *assetFilename, String *exclusionList, String *inclusionList)
{
	// Check both the exclusions and inclusions
	String *bestExclusion = StringUtils::GetBestFilter(assetFilename, exclusionList);
	String *bestInclusion = StringUtils::GetBestFilter(assetFilename, inclusionList);

	// Check if the inclusion is more specific?
	if (bestInclusion->Length > bestExclusion->Length)
	{
		return false;
	}
	
	// We are not filtered if both best's are empty
	if (bestInclusion->Length == 0 && bestExclusion->Length == 0)
	{
		return false;
	}

	return true;
}


String *StringUtils::GetBestFilter(String *item, String *filterList)
{
	String *bestFilterItem = "";

	// First check to see if there are any specified exclusions
	if (filterList->Length)
	{
		String* delimStr = S",;";
		Char delimiter[] = delimStr->ToCharArray();
		String* filterListItems[] = filterList->Trim()->Split(delimiter);

		// Check all the listed exlusions
		for (int i = 0; i < filterListItems->Count; i++)
		{
			String *filterItem = filterListItems[i];

			// Check if the item begins with this listItem?
			if (item->StartsWith(filterItem, StringComparison::CurrentCultureIgnoreCase))
			{
				// Check if this list item is more specific than our last?
				if (filterItem->Length > bestFilterItem->Length)
				{
					// Retain this filterItem as our bestFilterItem
					bestFilterItem = filterItem;
				}
			}
		}
	}

	// Return the best matching filter item
	return bestFilterItem;
}


int StringUtils::CompareStartsWithLikeness(String* string1, String* string2)
{
	int pos = 0;

	// Make sure we are not NULL
	if (string1 && 
		string2)
	{
		while (String::Compare(string1->Substring(0, pos), string2->Substring(0,pos), true) == 0)
		{
			pos++;
			if (pos >= string1->Length || pos >= string2->Length)
			{
				break;
			}
		}
	}

	return pos;
}
