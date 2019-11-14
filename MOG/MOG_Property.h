//--------------------------------------------------------------------------------
//	MOG_Property.h
//	
//	
//--------------------------------------------------------------------------------

#ifndef __MOG_Property_H__
#define __MOG_Property_H__

#include "MOG_Define.h"
#include "MOG_Ini.h"

namespace MOG {

public __gc class MOG_Property
{
public:
	// Basic Ini values
	String *mSection;
	String *mKey;
	String *mValue;

	// Enhanced property values
	String *mPropertySection;
	String *mPropertyKey;
	String *mPropertyValue;

	String *mPropertySectionScope;
	String *mPropertySectionScopeless;

public:
	MOG_Property();
	MOG_Property(String *propertyString);
	MOG_Property( String *Section, String *JoinedPropertySectionKey, String *Value );
	MOG_Property( String *Section, String *PropertySection, String *PropertyKey, String *PropertyValue );

	void Clear();
	void Initialize( String *Section, String *PropertySection, String *PropertyKey, String *PropertyValue );
	void Initialize( String *Section, String *JoinedPropertySectionKey, String *Value );

	// Property Scope
	String *GetPropertyScope();
	String *GetScopelessPropertySection();
	String *GetPropertyAsString();

	void ParsePropertyString(String *propertyString);

	// Property Ini Helper
	void SetSection(String *section);
	void SetKey(String *key);
	void SetValue(String *value);

	// Static Functions
	static MOG_Property *SetScope(MOG_Property *property, String *scopeName);

	MOG_Property *Clone( void );
};


}

using namespace MOG;

#endif	// __MOG_Property_H__

