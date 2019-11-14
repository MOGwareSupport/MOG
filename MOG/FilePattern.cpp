//--------------------------------------------------------------------------------
//	FilePattern.cpp
//	
//	
//--------------------------------------------------------------------------------

#include "StdAfx.h"

#include "FilePattern.h"


using namespace System;
using namespace System::Collections::Generic;
using namespace System::Text;
using namespace System::IO;



FilePattern::FilePattern(String *patterns)
{
	mPatterns = new ArrayList();

	// Make sure there is something specified in the patterns?
	if (patterns->Length > 0)
	{
		String *parts[] = patterns->Split(S","->ToCharArray());
		if (parts != NULL &&
			parts->Length > 0)
		{
			// Check each of our patterns
			for (int i = 0; i < parts->Count; i++)
			{
				String *pattern = parts[i];

				// Make sure this isn't empty?
				if (pattern->Length > 0)
				{
					// Check if this pattern contains a wildcard?
					if (pattern->Contains(S"*"))
					{
						// Check if this is already covered by an earlier pattern?
						if (!IsFilePatternMatch(pattern->Trim()))
						{
							// Make sure there are no illegal characters in this pattern
							Path::GetDirectoryName(pattern);
							Path::GetFileName(pattern);

							// Proceed to add this pattern
							mPatterns->Add(pattern->Trim());
						}
					}
				}
			}
			// Check each of our patterns
			for (int i = 0; i < parts->Count; i++)
			{
				String *pattern = parts[i];

				// Make sure this isn't empty?
				if (pattern->Length > 0)
				{
					// Check if this pattern contains a wildcard?
					if (!pattern->Contains(S"*"))
					{
						// Make sure this isn't already covered by an earlier pattern with a wildcard?
						if (!IsFilePatternMatch(pattern->Trim()))
						{
							mPatterns->Add(pattern->Trim());
						}
					}
				}
			}
		}
	}
}


String *FilePattern::ToString()
{
	// Combine the patterns into a single comma delimited string
//	return String::Join(S",", mPatterns->ToArray());
	return "";
}


void FilePattern::Add(String *newPattern)
{
	String *strippedPattern = Path::GetFileName(newPattern);

	// Check each of our patterns
	for (int i = 0; i < mPatterns->Count; i++)
	{
		String *thisPattern = __try_cast<String *>(mPatterns->Item[i]);

		// Check if this pattern matches?
		if (String::Compare(thisPattern, strippedPattern, true) == 0)
		{
			// Bail out now because it already exists
			return;
		}
	}

	// Proceed to add the pattern
	mPatterns->Add(strippedPattern);
}


void FilePattern::Remove(String *removePattern)
{
	String *strippedPattern = Path::GetFileName(removePattern);

	// Remove the specified pattern
	mPatterns->Remove(strippedPattern);
}


bool FilePattern::IsFilePatternMatch(String *item)
{
	// Check each of our patterns
	for (int i = 0; i < mPatterns->Count; i++)
	{
		String *pattern = __try_cast<String *>(mPatterns->Item[i]);

		// Check if it is an exact match?
		if (String::Compare(pattern, item, true) == 0)
		{
			return true;
		}
		// Check if this is the special no extension pattern?
		else if (String::Compare(pattern, S"*.", true) == 0)
		{
			// Check if this file is extension-less?
			if (Path::GetExtension(item)->Length == 0)
			{
				// Matched
				return true;
			}
		}
		// Check if there is a '*' at the beginning and end of this pattern?
		else if (pattern->StartsWith(S"*") && pattern->EndsWith(S"*"))
		{
			// Check if this pattern is listed anywhere within the specified fullFilename?
			if (item->ToLower()->Contains(pattern->ToLower()->Trim(S"*"->ToCharArray())))
			{
				// Matched
				return true;
			}
		}
		// Check if there is a '*' at the beginning?
		else if (pattern->StartsWith(S"*"))
		{
			// Check if this pattern is listed anywhere within the specified fullFilename?
			if (item->ToLower()->EndsWith(pattern->ToLower()->Trim(S"*"->ToCharArray())))
			{
				// Matched
				return true;
			}
		}
		// Check if there is a '*' at the beginning and end of this pattern?
		else if (pattern->EndsWith(S"*"))
		{
			// Check if this pattern is listed anywhere within the specified fullFilename?
			if (item->ToLower()->StartsWith(pattern->ToLower()->Trim(S"*"->ToCharArray())))
			{
				// Matched
				return true;
			}
		}
	}

	return false;
}

