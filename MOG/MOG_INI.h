//--------------------------------------------------------------------------------
//	MOG_INI.h
//	
//	
//--------------------------------------------------------------------------------

#ifndef __MOG_WINI_H__
#define __MOG_WINI_H__

#include "MOG_Define.h"


using namespace System::Collections::Specialized;


namespace MOG {
namespace INI {


//********************************
public __gc class MOG_IniKey {
public:
	String *mKey;
	String *mValue;
	String *mComment;

	ArrayList *mArrayItems;

	MOG_IniKey();
};


//********************************
public __gc class MOG_IniSection {
public:
	String *mSection;
	HybridDictionary *mKeys;
	
	MOG_IniSection();
};


//********************************
public __gc class MOG_Ini {

public:
	__value enum IniHandle {ClearHandle, HoldHandle};

protected:
	String	*mFullFilename;
	bool	mChanged;
	String	*mOpenedStackTrace;

	HybridDictionary *mSections;

	bool	ParseFile(String *fileBuffer);

	FileStream *mLockedFileStream;
	IniHandle mHandle;
	String *mFileTimeStamp;

public:
	MOG_Ini(String *filename);
	MOG_Ini(void);
	
	bool	HasChanged()					{ return mChanged; };
	bool	HasChanged(bool bChanged)		{ return mChanged = bChanged; };
	bool	Save(String *fullFilename);
	bool	Save();
	bool	Load();
	bool	Load(String *fullFilename);
	bool	LoadFile(String *fullFilename, FileAccess openLevel, FileShare shareLevel);
	bool	Open(String *fullFilename, FileShare shareLevel);
	void	SetFilename(String *filename, bool setChanged);
	String *GetFilename()					{ return mFullFilename; };
	void	Close();
	void	CloseNoSave();

	void	Empty();
	void	EmptySection(String *section);

	// Reaload the ini
	void	Refresh()	{ Load(); };

	// Put an entire ini file into the current ini
	bool	PutFile(MOG_Ini *iniFile);
	bool	PutFile(String *iniFile);
	bool	PutFileSection(MOG_Ini *iniFile, String *sourceSection, String *targetSection);

	// Merge two section together
	bool	MergeSections(String *sourceSection, String *targetSection);
	bool	MergeSections(MOG_IniSection *pSection, String *targetSection);
	bool	RetrieveValue(String *target, String *section, String *key);
	bool	RetrieveValue(bool *target, String *section, String *key);
	bool	RetrieveValue(int *target, String *section, String *key);
	
	MOG_IniSection *SectionFind(String *section);
	bool	SectionExist(String *section);
	MOG_IniKey *KeyFind(String *section, String *key);
	MOG_IniKey *KeyFind(MOG_IniSection *section, String *key);
	bool	KeyExist(String *section, String *key);

	String *GetSectionKeys(String *section)[];
	ArrayList *GetSections(String *pattern);
	String	*GetString(String *section, String *key);
	bool	GetBool(String *section, String *key);
	int		GetValue(String *section, String *key);
	int		CountKeys(String *section);
	int		CountSections();
	
	String	*GetKeyByIndexSLOW(String *section, Int32 keyIndex);
	String	*GetKeyNameByIndexSLOW(String *section, Int32 keyIndex);
	String	*GetSectionByIndexSLOW(int sectionIndex);

	int		PutString(String *section, String *key, String *str);
	int		PutString(String *section, String *key, String *str, String *comment);
	int		PutBool(String *section, String *key, const bool value);
	int		PutValue(String *section, String *key, const int value);
	int		PutSectionString(String *section, String *str);
	int		PutSection(String *section);
	int		PutArray(String *section, String *key, ArrayList *items);
	int		PutArrayItem(String *section, String *key, String *value);
	int		PutArrayItem(String *section, String *key, String *value, String *comment);
	int		PutArrayItem(MOG_IniSection *section, String *key, String *value, String *comment);

	int		RemoveString(String *section, String *key);
	int		RemoveSection(String *section);

	bool	RenameSection(String *section, String *newSection);
	bool	RenameKey(String *section, String *key, String *newKey);

	virtual String *ToString()				{ return mFullFilename; }
};

}
}

using namespace MOG::INI;

#endif	// __MOG_WINI_H__

