//--------------------------------------------------------------------------------
//	MOG_INI.h
//	
//	
//--------------------------------------------------------------------------------

#include "stdafx.h"

#include "MOG_Define.h"
#include "MOG_INI.h"
#include "MOG_StringUtils.h"
#include "MOG_DosUtils.h"
#include "MOG_Time.h"
#include "MOG_ControllerSystem.h"
#include "MOG_Prompt.h"
#include "MOG_Progress.h"

using namespace System::Text;
using namespace System::Threading;
using namespace System::Windows::Forms;

#include <stdio.h>


//***************************************************************
MOG_Ini::MOG_Ini(String *fullFilename)
{
	mSections = new HybridDictionary(true);
	mChanged = false;
	mFullFilename = "";
	mLockedFileStream = NULL;
	mHandle = IniHandle::ClearHandle;

	Load(fullFilename);

	// Used for debugging
	mOpenedStackTrace = MOG_Environment_StackTrace;
}

//***************************************************************
MOG_Ini::MOG_Ini(void)
{
	mSections = new HybridDictionary(true);
	mChanged = false;
	mFullFilename = "";
	mLockedFileStream = NULL;
	mHandle = IniHandle::ClearHandle;

	// Used for debugging
	mOpenedStackTrace = MOG_Environment_StackTrace;
}

//***************************************************************
bool MOG_Ini::Load()
{
	// Make sure mFullFilename has previously been set
	if (mFullFilename->Length)
	{
		// Clear previous member data
		if(mSections != 0)
			mSections->Clear();

		return LoadFile(mFullFilename, FileAccess::Read, FileShare::Read );
	}
	return false;
}

//***************************************************************
bool MOG_Ini::Load(String *fullFilename)
{
	Close();

	// Clear previous member data
	if(mSections != 0)
		mSections->Clear();

	mFullFilename = fullFilename;

	return LoadFile(mFullFilename, FileAccess::Read, FileShare::Read );
}

//***************************************************************
bool MOG_Ini::Open(String *fullFilename, FileShare shareLevel)
{
	Close();

	// Clear previous member data
	if(mSections != 0)
		mSections->Clear();

	mFullFilename = fullFilename;

	FileAccess access;

	// Set our access level
	switch (shareLevel)
	{
		case FileShare::None: 
			access = FileAccess::ReadWrite;
			mHandle = IniHandle::HoldHandle;
			break;
		case FileShare::Read: 
			access = FileAccess::Read;
			mHandle = IniHandle::ClearHandle;
			break;
		case FileShare::ReadWrite: 
			access = FileAccess::ReadWrite;
			mHandle = IniHandle::HoldHandle;
			break;
		case FileShare::Write: 
			access = FileAccess::ReadWrite;
			mHandle = IniHandle::HoldHandle;
			break;
		default:
			access = FileAccess::Read;
			mHandle = IniHandle::ClearHandle;
			break;
	}

	// We clear out the filename if we were unable to load
	if (!LoadFile(fullFilename, access, shareLevel))
	{
		mFullFilename = "";
		return false;
	}

	return true;
}

//***************************************************************
bool MOG_Ini::Save()
{
	if (mFullFilename && mFullFilename->Length)
		return Save(mFullFilename);
	else
		return false;
}

//***************************************************************
bool MOG_Ini::Save(String *fullFilename)
{
	String *outLine = "";
	int x = 0;
	MOG_IniSection *sec;
	MOG_IniKey *key;
	bool bFilenameChanged = false;

	//if ( mChanged && ( mHandle == IniHandle::HoldHandle) && String::Compare(mFullFilename, fullFilename, true)==0) ERROR and exit

	if (String::Compare(mFullFilename, fullFilename, true))
	{
		mFullFilename = fullFilename;
		bFilenameChanged = true;
	}

	if (mChanged || bFilenameChanged)
	{
		FileInfo *file = new FileInfo(fullFilename);

		// Check if target exists
		if (!file->Exists)
		{
			try
			{
				// Check if target has a dir
				if (fullFilename->LastIndexOf("\\") != -1)
				{
					// verify the target dir exists
					DirectoryInfo *targetDir = new DirectoryInfo(fullFilename->Substring(0, fullFilename->LastIndexOf("\\")));

					// Check if target dir exists
					if (!targetDir->Exists)
					{
						targetDir->Create();
					}
				}
			}
			catch(Exception *e)
			{
				MOG_Report::ReportMessage("MOG_Ini::Save Error ", String::Concat( mFullFilename, S"\nCould not Create directory (", e->Message, S")"), e->StackTrace, MOG::PROMPT::MOG_ALERT_LEVEL::ERROR);
			}
		}
		else
		{
		}

		//FileStream *fs;
		StreamWriter *sw;
		int openCounter = 0;

		int windowId = 0;
		bool bCanceled = false;

		while(!sw)
		{
			try
			{
				if (mLockedFileStream == NULL)
				{
					mLockedFileStream = new FileStream(fullFilename, FileMode::Create, FileAccess::Write, FileShare::None);
				}
				else
				{
					mLockedFileStream->Seek(0, SeekOrigin::Begin);
				}

				if (mLockedFileStream)
				{
					sw = new StreamWriter(mLockedFileStream);
				}
			}
			catch(Exception* e)
			{
				// Gracefully wait a bit before we attempt it again
				Thread::Sleep(100);
				// Wait a few times before we notify the user
				if (openCounter++ >= 10)
				{
					// Check if we need to initialize a new dialog?
					if (windowId == 0)
					{
						// Initialize the dialog
//						MOG_Filename *assetFilename = new MOG_Filename(fullFilename);
//						String *message = String::Concat( S"MOG is unable to access a required file due to a potential file sharing violation.\n",
//															S"ASSET: ", assetFilename->GetAssetFullName(), S"\n",
//															S"\n",
//															S"Close any open files that may be associated with this asset.");
						// Show the user the system exception
						String *message = String::Concat(	e->Message, S"\n",
															S"FILE: ", mFullFilename);
						windowId = MOG_Progress::ProgressSetup(S"MOG_Ini::Save Error", message);
					}

					if (windowId != 0)
					{
						// Process the dialog
						bCanceled = MOG_Progress::ProgressStatus(windowId) == MOGPromptResult::Cancel;
						if (bCanceled)
						{
							break;
						}						
					}
				}
			}
		}
		// Check if we ever initialized a dialog?
		if (windowId != 0)
		{
			// Shutdown the dialog
			MOG_Progress::ProgressClose(windowId);
		}

		// Check for proper create or load
		if(sw != 0)
		{
			// Iterate through the sections
			IDictionaryEnumerator *sectionEnumerator = mSections->GetEnumerator();
			while ( sectionEnumerator->MoveNext() )
			{
				sec = __try_cast<MOG_IniSection*>(sectionEnumerator->Value);

				outLine = String::Concat("\r\n[", sec->mSection, "]");
				sw->WriteLine(outLine);
				
				// Iterate through the keys
				IDictionaryEnumerator *keyEnumerator = sec->mKeys->GetEnumerator();
				while ( keyEnumerator->MoveNext() )
				{
					key = __try_cast<MOG_IniKey*>(keyEnumerator->Value);

					if (key->mValue->Length)
					{
						outLine = String::Concat(key->mKey, "=", key->mValue, key->mComment);
						sw->WriteLine(outLine);
					}
					else
					{
						outLine = String::Concat(key->mKey, key->mComment);
						sw->WriteLine(outLine);
					}

					// Check if we have some array elements?
					if (key->mArrayItems)
					{
						for (int i = 0; i < key->mArrayItems->Count; i++)
						{
							String *value = dynamic_cast<String *>(key->mArrayItems->Item[i]);

							if (value->Length)
							{
								outLine = String::Concat(key->mKey, "=", value);
								sw->WriteLine(outLine);
							}
							else
							{
								outLine = key->mKey;
								sw->WriteLine(outLine);
							}
						}
					}
				}
			}
			
			try
			{
				__int64 pos = mLockedFileStream->Position;
				mLockedFileStream->SetLength(pos);
				mChanged = false;
				mLockedFileStream->Flush();
				sw->Flush();
			}
			catch(Exception *e)
			{
				MOG_Report::ReportMessage("MOG_Ini::Save Error ", String::Concat( mFullFilename, S"\nCould flush ini filestream handle(", e->Message, S")"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
			}

			if ( mHandle == IniHandle::ClearHandle)
			{
				// Close the StreamWriter who will close the FileStream for us.
				try
				{
					sw->Close();
					mLockedFileStream->Close();
				}
				catch(Exception *e)
				{
					MOG_Report::ReportMessage("MOG_Ini::Save Error ", String::Concat( mFullFilename, S"\nCould close ini filestream handle(", e->Message, S")"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
				}
				mLockedFileStream = NULL;
			}

			return true;
		}

		MOG_Report::ReportMessage("MOG_Ini::Save ", String::Concat(mFullFilename, S"\nCannot Save File", fullFilename), Environment::StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
		return false;
	}

	return true;
}

//***************************************************************
bool MOG_Ini::LoadFile(String *fullFilename, FileAccess openLevel, FileShare shareLevel)
{
	// Make sure we have a valid filename specified?
	if (!fullFilename || !fullFilename->Length)
	{
		return false;
	}

	StreamReader *read;
	int windowId = 0;
	int openCounter = 0;
	bool bCanceled = false;

	try
	{
		while (!read)
		{
			try
			{
				// Make sure file exists
				FileInfo *file = new FileInfo(fullFilename);
				if(!file->Exists)
				{
					return true;
				}

				// Open the file
				if(mLockedFileStream == NULL)
				{
					mLockedFileStream = new FileStream(fullFilename, FileMode::Open, openLevel, shareLevel);
				}
				read = new StreamReader(mLockedFileStream);
			}
			catch(Exception* e)
			{
				e->ToString();

				// KLK - I am not sure how to get to what actually happened.  For now I will assume that it was a sharing violation.
				// I dont like this :(
				Thread::Sleep(100);

				openCounter++;

				if (openCounter >= 10)
				{
					// Check if we need to initialize a new dialog?
					if (windowId == 0)
					{
						// Initialize the dialog
						MOG_Filename *assetFilename = new MOG_Filename(fullFilename);
						String *message = String::Concat( S"MOG is unable to access a required file due to a potential file sharing\n violation.\n",
															S"FILE: ", fullFilename, S"\n",
															S"Close any open files that may be associated with this asset.");
						windowId = MOG_Progress::ProgressSetup(	S"File Sharing Violation", message);
						MOG_Progress::ProgressInitGraphic(windowId);
					}

					if (windowId != 0)
					{
						// Process the dialog
						bCanceled = MOG_Progress::ProgressUpdateGraphic(windowId, "") == MOGPromptResult::Cancel;
						if (bCanceled)
						{
							break;
						}

						// Gracefully sleep for a bit while we wait for the next attempt
						Thread::Sleep(100);
					}
				}
			}
		}

		String *RamFile = read->ReadToEnd();

		if (mHandle == IniHandle::ClearHandle)
		{
			read->Close();
			mLockedFileStream->Close();
			mLockedFileStream = NULL;
		}

		// Parse the file
		return ParseFile(RamFile);
	}
	catch(Exception* e)
	{
		e->ToString();
		return false;
	}
	__finally
	{
		// Check if we ever initialized a dialog?
		if (windowId != 0)
		{
			// Shutdown the dialog
			MOG_Progress::ProgressClose(windowId);
		}
	}

	return false;
}

__value enum ParseState
{
	NewLine,			// start of line (any can follow)
	ParsingSection,		// inside a section
	ParsingKey,			// inside a key
	ParsingValue,		// inside a key
	ParsingComment,		// inside a comment
};

bool MOG_Ini::ParseFile(String *fileBuffer)
{
	CharEnumerator *thisChar = fileBuffer->GetEnumerator();
	StringBuilder *thisString = new StringBuilder();
	StringBuilder *commentWhitespace = new StringBuilder();
	//String *thisString = "";
	//String *commentWhitespace = "";
	MOG_IniKey		*key = new MOG_IniKey();
	MOG_IniSection	*section = new MOG_IniSection();
	ParseState state = ParseState::NewLine;

	// Add comment string, and initialize to nothing
	key->mComment = "";

	// Scan every character in the file
	while(thisChar->MoveNext())
	{
		// Check for parsing characters
		// End of Keyname?
		if ((thisChar->Current == '=') && (state == ParseState::ParsingKey))
		{
			// Set this Key name
			key->mKey = thisString->ToString();
			// Clear thisString
			thisString = new StringBuilder();
			// Move onto parsing the value
			state = ParseState::ParsingValue;
		}
		// carriage return
		else if (thisChar->Current == 13)
		{
			// just eat this byte of the stream - 
		}
		// End of line?
		else if (thisChar->Current == 10)
		{
			// Check what piece we were parsing?
			switch (state)
			{
				case ParseState::ParsingSection:
					// Set the section name to the current string trimmimg off the ending ']' character
					section->mSection = thisString->ToString()->Substring(0, thisString->ToString()->LastIndexOf(S"]"));	
					break;
				case ParseState::ParsingKey:
					key->mKey = thisString->ToString();
					break;
				case ParseState::ParsingValue:
					key->mValue = thisString->ToString();
					break;
				case ParseState::ParsingComment:
					key->mComment = thisString->ToString();
					break;
			}

			// Check if we have anything to add?
			if (key->mKey->Length > 0 ||
				key->mValue->Length > 0 ||
				key->mComment->Length > 0)
			{
				PutArrayItem(section, key->mKey, key->mValue, key->mComment);
			}

			// Clear all line variables
			key->mKey = "";
			key->mValue = "";
			key->mComment = "";
			thisString = new StringBuilder();
			commentWhitespace = new StringBuilder();

			// Indicate we are starting a new line
			state = ParseState::NewLine;
		}
		// New Section?
		else if ((thisChar->Current == '[') && (state == ParseState::NewLine))
		{
			// Check if we were able to name our section?
			if (section->mSection->Length > 0)
			{
				// Push this section and all of it's keys
				mSections->Item[section->mSection] = section;

				// Clean up the section variables
				section = new MOG_IniSection();
			}

			state = ParseState::ParsingSection;			
		}
		else
		{
			// Check if this is a new line?
			if (thisString->ToString()->Length == 0 && state == ParseState::NewLine)
			{
				if (section->mSection->Length)
				{
					// Make sure to indicate we are no longer at the beginning of the line
					state = ParseState::ParsingKey;
				}
			}

			// Add the single character
			thisString->Append(thisChar->Current);

			// Track any growing whitespace
			if (Char::IsWhiteSpace(thisChar->Current))
			{
				commentWhitespace->Append(thisChar->Current);
			}
		}
	}

	// Push the final key into the section?
	if (thisString->ToString()->Length > 0)
	{
		// Key names must get filled in first...then the key values
		if (!(key->mKey->Length > 0))
			key->mKey = thisString->ToString();
		else
			key->mValue = thisString->ToString();

		// Put this key into our section
		PutArrayItem(section, key->mKey, key->mValue, key->mComment);

		// Erase both the key & value
		key->mKey = "";
		key->mValue = "";
		key->mComment = "";
	}

	// Push the final section?
	if (section->mSection->Length > 0)
	{
		// Push this section and all of it's keys
		mSections->Item[section->mSection] = section;
		//mSections->Add(section);
	}

	// Always make sure we don't think we changed whatever file was just loaded
	mChanged = false;

	return true;
}

//***************************************************************
void MOG_Ini::SetFilename(String *filename, bool setChanged)
{
//? MOG_Ini::Save - Decide how to handle a locked file when the name is changed but we still have the old file open

	// Set the new filename
	mFullFilename = filename;

	// Only bother setting mChanged if the file hasn't already been changed...
	if (!mChanged)
	{
		mChanged = setChanged;
	}
}

//***************************************************************
void MOG_Ini::CloseNoSave()
{
	mChanged = false;

	Close();
}

//***************************************************************
void MOG_Ini::Close()
{
	Save();

	// Close the StreamWriter who will close the FileStream for us.
	if (mHandle == IniHandle::HoldHandle)
	{
		if (mLockedFileStream)
		{
			mLockedFileStream->Close();
			mLockedFileStream = NULL;
		}
		mHandle = IniHandle::ClearHandle;
	}

	// Clear previous member data
	mSections->Clear();
	mFullFilename = "";
	mChanged = false;
}

//***************************************************************
void MOG_Ini::Empty()
{
	// Clear previous member data
//	mSections->Clear(mSections, 0, mSections->Count);
	mSections->Clear();
	mChanged = true;
}

//***************************************************************
void MOG_Ini::EmptySection(String *section)
{
	if (mSections->Contains(section))
	{
		mSections->Remove(section);
		mChanged = true;
	}
}

//***************************************************************
bool MOG_Ini::GetBool(String *section, String *key)
{
	bool value = false;

//?	MOG_Ini::GetBool - This will cause a crash if str contains a ';' character
	String *str = GetString(section, key);
	if (str->Length)
	{
		value = Convert::ToBoolean(str);
	}

	return value;
}

//***************************************************************
int MOG_Ini::GetValue(String *section, String *key)
{
	int value = 0;

//?	MOG_Ini::GetBool - This will cause a crash if str contains a ';' character
	String *str = GetString(section, key);
	if (str->Length)
	{
		value = Convert::ToInt32(str);
	}

	return value;
}

//***************************************************************
// Returns empty string if we did not find key
String *MOG_Ini::GetString(String *section, String *key)
{
	MOG_IniKey *pKey = KeyFind(section, key);
	if (pKey)
	{
		return pKey->mValue;
	}

	MOG_Report::ReportMessage("MOG_Ini::GetString ", String::Concat(mFullFilename, S"\nSECTION: ", section, S"\nKEY : ", key, S"\nSection and or Key not found!"), Environment::StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);

	return S"";
}

//***************************************************************
int MOG_Ini::CountKeys(String *section)
{
	MOG_IniSection *pSection = SectionFind(section);
	if (pSection)
	{
		return pSection->mKeys->Count;
	}

	MOG_Report::ReportMessage("MOG_Ini::CountKeys ", String::Concat(mFullFilename, S"\nSECTION: ", section, S"\nSection not found!"), Environment::StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);

	return 0;
}

//***************************************************************
bool MOG_Ini::SectionExist(String *section)
{
	if (SectionFind(section))
	{
		return true;
	}
	else
	{
		return false;
	}

	return false;
}

//***************************************************************
MOG_IniSection *MOG_Ini::SectionFind(String *section)
{
	if (mSections->Contains(section))
	{
		return __try_cast<MOG_IniSection *>(mSections->Item[section]);
	}

	return NULL;
}

//***************************************************************
bool MOG_Ini::KeyExist(String *section, String *key)
{
	if (KeyFind(section, key))
	{
		return true;
	}
	else
	{
		return false;
	}
}

//***************************************************************
MOG_IniKey *MOG_Ini::KeyFind(String *section, String *key)
{
	return KeyFind(SectionFind(section), key);
}

//***************************************************************
MOG_IniKey *MOG_Ini::KeyFind(MOG_IniSection *pSection, String *key)
{
	// Located the section?
	if (pSection)
	{
		if (pSection->mKeys->Contains(key))
		{
			return __try_cast<MOG_IniKey *>(pSection->mKeys->Item[key]);
		}
	}

	return NULL;
}

//***************************************************************
bool MOG_Ini::PutFile(String *iniFile)
{
	MOG_Ini *temp = new MOG_Ini;
	
	// Load the ini file
	if (temp->Load(iniFile) == false)
	{
		return false;
	}
	
	PutFile(temp);

	temp->Close();

	return true;
}
//***************************************************************
bool MOG_Ini::PutFile(MOG_Ini *iniFile)
{
	// Iterate through the sections
	IDictionaryEnumerator *sectionEnumerator = iniFile->mSections->GetEnumerator();
	while ( sectionEnumerator->MoveNext() )
	{
		MOG_IniSection *section = dynamic_cast<MOG_IniSection *>(sectionEnumerator->Value);
		if (section)
		{
			// Check if we have keys?
			if (section->mKeys->Count)
			{
				// Iterate through the keys
				IDictionaryEnumerator *keyEnumerator = section->mKeys->GetEnumerator();
				while ( keyEnumerator->MoveNext() )
				{
					MOG_IniKey *key = dynamic_cast<MOG_IniKey *>(keyEnumerator->Value);
					if (key)
					{
						PutString(section->mSection, key->mKey, key->mValue);
					}
				}
			}
			else
			{
				// Add the section with no keys into our ini
				PutSection(section->mSection);
			}
		}
	}

	return true;
}

//***************************************************************
bool MOG_Ini::PutFileSection(MOG_Ini *iniFile, String *sourceSection, String *targetSection )
{
	return MergeSections(iniFile->SectionFind(sourceSection), targetSection);
}

//***************************************************************
bool MOG_Ini::MergeSections(String *sourceSection, String *targetSection)
{
	return MergeSections(SectionFind(sourceSection), targetSection);
}

//***************************************************************
bool MOG_Ini::MergeSections(MOG_IniSection *pSection, String *targetSection)
{
	if (pSection)
	{
		// Iterate through the keys
		IDictionaryEnumerator *keyEnumerator = pSection->mKeys->GetEnumerator();
		while ( keyEnumerator->MoveNext() )
		{
			MOG_IniKey *key = __try_cast<MOG_IniKey *>(keyEnumerator->Value);
			PutString(targetSection, key->mKey, key->mValue);
		}

		return true;
	}
	
	return false;
}

//***************************************************************
bool MOG_Ini::RetrieveValue(String *target, String *section, String *key)
{
	if (SectionExist(section) && KeyExist(section, key))
	{
		target = GetString(section, key);
		return true;
	}

	target = "";
	return false;
}

//***************************************************************
bool MOG_Ini::RetrieveValue(bool *target, String *section, String *key)
{
	if (SectionExist(section) && KeyExist(section, key))
	{
		*target = GetBool(section, key);
		return true;
	}

	*target = false;
	return false;
}

//***************************************************************
bool MOG_Ini::RetrieveValue(int *target, String *section, String *key)
{
	if (SectionExist(section) && KeyExist(section, key))
	{
		*target = GetValue(section, key);
		return true;
	}

	*target = 0;
	return false;
}

//***************************************************************
int MOG_Ini::CountSections()
{
	return mSections->Count;
}

//***************************************************************
String *MOG_Ini::GetSectionByIndexSLOW(int sectionIndex)
{
	MOG_ASSERT_THROW(sectionIndex<mSections->Count, MOG_Exception::MOG_EXCEPTION_InvalidData, "MOG ERROR - section index is bigger than number section in ini");
	MOG_ASSERT_THROW(sectionIndex>=0, MOG_Exception::MOG_EXCEPTION_InvalidData, "MOG ERROR - section index must be 0 or greater in ini");

	// Iterate through the sections
	int count = 0;
	IDictionaryEnumerator *sectionEnumerator = mSections->GetEnumerator();
	while ( sectionEnumerator->MoveNext() )
	{
		MOG_IniSection *section = __try_cast<MOG_IniSection *>(sectionEnumerator->Value);
		if (count++ == sectionIndex)
		{
			return section->mSection;
		}
	}

	return NULL;
}
	
//***************************************************************
String *MOG_Ini::GetKeyByIndexSLOW(String *section, Int32 keyIndex)
{
	MOG_IniSection *pSection = SectionFind(section);
	if (pSection)
	{
		// Iterate through the keys
		int count = 0;
		IDictionaryEnumerator *keyEnumerator = pSection->mKeys->GetEnumerator();
		while ( keyEnumerator->MoveNext() )
		{
			MOG_IniKey *key = __try_cast<MOG_IniKey *>(keyEnumerator->Value);
			if (count++ == keyIndex)
			{
				return key->mValue;
			}
		}
	}

	MOG_Report::ReportMessage("MOG_Ini::GetKeyByIndex ", String::Concat(mFullFilename, S"\nSECTION: ", section, S"\nKEY: ", Convert::ToString(keyIndex), S"\nSection and or Key not found!"), Environment::StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);

	return "";
}	

//***************************************************************
String *MOG_Ini::GetKeyNameByIndexSLOW(String *section, Int32 keyIndex)
{
	MOG_IniSection *pSection = SectionFind(section);
	if (pSection)
	{
		// Iterate through the keys
		int count = 0;
		IDictionaryEnumerator *keyEnumerator = pSection->mKeys->GetEnumerator();
		while ( keyEnumerator->MoveNext() )
		{
			MOG_IniKey *key = __try_cast<MOG_IniKey *>(keyEnumerator->Value);
			if (count++ == keyIndex)
			{
				return key->mKey;
			}
		}
	}

	MOG_Report::ReportMessage("MOG_Ini::GetKeyNameByIndex ", String::Concat(mFullFilename, S"\nSECTION: ", section, S"\nKEY: ", Convert::ToString(keyIndex), S"\nSection and or Key not found!\n"), Environment::StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);

	return "";
}

//***************************************************************
String *MOG_Ini::GetSectionKeys(String *section)[]
{
	MOG_IniSection *pSection = SectionFind(section);
	if (pSection)
	{
		String *strings[] = new String *[pSection->mKeys->Count];
		
		// Iterate through the keys
		int count = 0;
		IDictionaryEnumerator *keyEnumerator = pSection->mKeys->GetEnumerator();
		while ( keyEnumerator->MoveNext() )
		{
			MOG_IniKey *key = __try_cast<MOG_IniKey *>(keyEnumerator->Value);
			strings[count++] = key->mKey;
		}

		return strings;
	}

	MOG_Report::ReportMessage("MOG_Ini::CountKeys ", String::Concat(mFullFilename, S"\nSECTION: ", section, S"\nSection not found!"), Environment::StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);

	return NULL;
}

//***************************************************************
// Match a non-regex pattern to our sections
ArrayList *MOG_Ini::GetSections(String *patternToMatch)
{
	ArrayList *sections = new ArrayList();

	// If patternToMatch is NULL, let it be empty, instead...
	if(!patternToMatch)
	{
		patternToMatch = S"";
	}

	// Iterate through the sections
	IDictionaryEnumerator *sectionEnumerator = mSections->GetEnumerator();
	while ( sectionEnumerator->MoveNext() )
	{
		MOG_IniSection *section = __try_cast<MOG_IniSection *>(sectionEnumerator->Value);

		// Check if this sections starts with the specified patternToMatch
		if(section->mSection->StartsWith(patternToMatch, StringComparison::CurrentCultureIgnoreCase))
		{
			sections->Add(section->mSection);
		}
	}

	return sections;
}

//***************************************************************
int MOG_Ini::PutBool(String *section, String *key, const bool value)
{
	return PutString(section, key, Convert::ToString(value));
}


//***************************************************************
int MOG_Ini::PutValue(String *section, String *key, const int value)
{
	return PutString(section, key, Convert::ToString(value));
}

//***************************************************************
int MOG_Ini::PutString(String *section, String *key, String *str)
{
	return PutString(section, key, str, S"");
}

int MOG_Ini::PutString(String *section, String *key, String *str, String *comment)
{
	MOG_ASSERT_THROW(section->Length, MOG_Exception::MOG_EXCEPTION_InvalidData, "MOG_INI - PutString requires a valid section");
	MOG_ASSERT_THROW(key->Length, MOG_Exception::MOG_EXCEPTION_InvalidData, "MOG_INI - PutString requires either a valid key or str");
	if (str == NULL)
	{
		return 0;
	}
	else if (key->Length == 0)
	{
		return PutSectionString(section, str);
	}

	MOG_IniSection *pSection = SectionFind(section);
	// Check if section is missing
	if (pSection == NULL)
	{
		// Create section
		pSection = new MOG_IniSection();
		pSection->mSection = section;
		// Add section
		mSections->Item[pSection->mSection] = pSection;
	}
	// Section found, now check for key
	if (pSection)
	{
		MOG_IniKey *pKey = NULL;

		// Check if this key already exists
		if (pSection->mKeys->Contains(key))
		{
			// Use the existing key
			pKey = __try_cast<MOG_IniKey*>(pSection->mKeys->Item[key]);

			// Check if the key's value is actually being changed
			if (String::Compare(pKey->mValue, str, true) != 0)
			{
				// Assign the new value
				pKey->mValue = str;

				// Check if we have a comment specified?
				if (comment->Length)
				{
					// Assign the comment into this key
					pKey->mComment = comment;
				}

				// Add key
				pSection->mKeys->Item[key] = pKey;
				mChanged = true;
			}
		}
		else
		{
			// Create a new key
			pKey = new MOG_IniKey();
			pKey->mKey = key;
			pKey->mValue = str;

			// Check if we have a comment specified?
			if (comment->Length)
			{
				// Assign the comment into this key
				pKey->mComment = comment;
			}

			// Add key
			pSection->mKeys->Item[key] = pKey;
			mChanged = true;
		}
	}

	return 1;
}

//***************************************************************
int	MOG_Ini::PutSectionString(String *section, String *str)
{
	MOG_IniSection *pSection = SectionFind(section);
	// Check if section is missing
	if (pSection == NULL)
	{
		// Create section
		pSection = new MOG_IniSection();
		pSection->mSection = section;
		// Add section
		mSections->Item[pSection->mSection] = pSection;
	}
	// Section found, now check for key
	if (pSection)
	{
		// Check if this key already exists
		if (!pSection->mKeys->Contains(str))
		{
			// Create key
			MOG_IniKey *pKey = new MOG_IniKey();
			pKey->mKey = str;
			// Add key
			pSection->mKeys->Item[str] = pKey;
			mChanged = true;
		}
	}

	return 1;
}

//***************************************************************
int	MOG_Ini::PutSection(String *section)
{
	// Make sure this doesn't already exists
	MOG_IniSection *pSection = SectionFind(section);
	if (!pSection)
	{
		// Create new section
		pSection = new MOG_IniSection;
		pSection->mSection = section;
		// Add the new section
		mSections->Item[pSection->mSection] = pSection;
		mChanged = true;
	}

	return 1;
}

//***************************************************************
int MOG_Ini::PutArray(String *section, String *key, ArrayList *items)
{
	// Make sure we have a valid array?
	if (items)
	{
		// Loop through our array
		for (int i = 0; i < items->Count; i++)
		{
			// Get this item from the array
			String *item = dynamic_cast<String *>(items->Item[i]);
			if (item)
			{
				// Put this single item
				PutArrayItem(section, key, item, S"");
			}
		}

		return 1;
	}

	return 0;
}

//***************************************************************
int MOG_Ini::PutArrayItem(String *section, String *key, String *value)
{
	return PutArrayItem(section, key, value, S"");
}

int MOG_Ini::PutArrayItem(String *section, String *key, String *value, String *comment)
{
	MOG_ASSERT_THROW(section->Length, MOG_Exception::MOG_EXCEPTION_InvalidData, "MOG_INI - PutString requires a valid section");
	MOG_ASSERT_THROW(key->Length, MOG_Exception::MOG_EXCEPTION_InvalidData, "MOG_INI - PutString requires either a valid key or str");

	// Check if section is missing
	MOG_IniSection *pSection = SectionFind(section);
	if (pSection)
	{
		if (PutArrayItem(pSection, key, value, comment))
		{
			return 1;
		}
	}

	// Add the first item as a normal string
	return PutString(section, key, value, comment);
}

int MOG_Ini::PutArrayItem(MOG_IniSection *pSection, String *key, String *value, String *comment)
{
	MOG_ASSERT_THROW(pSection, MOG_Exception::MOG_EXCEPTION_InvalidData, "MOG_INI - PutString requires a valid section");
	MOG_ASSERT_THROW(key->Length, MOG_Exception::MOG_EXCEPTION_InvalidData, "MOG_INI - PutString requires either a valid key or str");

	// Check if this key already exists
	if (pSection->mKeys->Contains(key))
	{
		// Use the existing key
		MOG_IniKey *pKey = __try_cast<MOG_IniKey*>(pSection->mKeys->Item[key]);
		if (pKey)
		{
			// Check if this is the first element of the array for this key?
			if (pKey->mArrayItems == NULL)
			{
				pKey->mArrayItems = new ArrayList();
			}

			// Add this item to our array
			pKey->mArrayItems->Add(value);
			mChanged = true;

			// Check if we have a comment specified?
			if (comment->Length)
			{
				// Assign the comment into this key
				pKey->mComment = comment;
			}

			return 1;
		}
	}

	// Create a new key
	MOG_IniKey *pKey = new MOG_IniKey();
	pKey->mKey = key;
	pKey->mValue = value;

	// Check if we have a comment specified?
	if (comment->Length)
	{
		// Assign the comment into this key
		pKey->mComment = comment;
	}

	// Add key
	pSection->mKeys->Item[key] = pKey;
	mChanged = true;
	return 1;
}

//***************************************************************
int MOG_Ini::RemoveString(String *section, String *key)
{
	MOG_IniSection *pSection = SectionFind(section);
	if(pSection)
	{
		if (pSection->mKeys->Contains(key))
		{
			pSection->mKeys->Remove(key);
			mChanged = true;
		}
	}

	return true;	
}

//***************************************************************
int MOG_Ini::RemoveSection(String *section)
{
	if (mSections->Contains(section))
	{
		mSections->Remove(section);
		mChanged = true;
	}

	return true;
}

//***************************************************************
bool MOG_Ini::RenameSection(String *section, String *newSection)
{
	MOG_IniSection *pSection = SectionFind(section);
	if (pSection)
	{
		// Remove our old Section
		mSections->Remove(section);

		// Rename the section
		pSection->mSection = newSection;
		// Add back the renamed section
		mSections->Item[newSection] = pSection;
		mChanged = true;

		return true;
	}

	return false;
}

//***************************************************************
bool MOG_Ini::RenameKey(String *section, String *key, String *newKey)
{
	MOG_IniKey *pKey = KeyFind(section, key);
	// Section found, now check for key
	if(pKey)
	{
		// Remove the old Key
		RemoveString(section, key);

		// Put a new key
		PutString(section, newKey, pKey->mValue);
		mChanged = true;

		return true;		
	}

	return false;
}
//***************************************************************

MOG_IniKey::MOG_IniKey()
{
	mKey = ""; 
	mValue = "";
}

MOG_IniSection::MOG_IniSection()
{
	mSection = ""; 
	mKeys = new HybridDictionary(true);	
}
