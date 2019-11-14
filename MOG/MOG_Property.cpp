//--------------------------------------------------------------------------------
//	MOG_Property.cpp
//	
//	
//--------------------------------------------------------------------------------

#include "stdafx.h"

#include "MOG_Define.h"
#include "MOG_StringUtils.h"
#include "MOG_DosUtils.h"
#include "MOG_Time.h"
#include "MOG_ControllerPackage.h"

#include "MOG_Property.h"


using namespace System::Threading;
using namespace System::Windows::Forms;

#include <stdio.h>



//***************************************************************

MOG_Property::MOG_Property()
{
	Clear();
}

MOG_Property::MOG_Property(String *propertyString)
{
	Clear();
	ParsePropertyString(propertyString);
}


MOG_Property::MOG_Property( String *Section, String *JoinedPropertySectionKey, String *Value )
{
	Clear();
	Initialize( Section, JoinedPropertySectionKey, Value );
}


MOG_Property::MOG_Property( String *Section, String *PropertySection, String *PropertyKey, String *PropertyValue )
{
	Clear();
	Initialize( Section, PropertySection, PropertyKey, PropertyValue );
}


void MOG_Property::Clear()
{
	mSection = S"";
	mKey = S"";
	mValue = S"";
	mPropertySection = S"";
	mPropertyKey = S"";
	mPropertyValue = S"";
	mPropertySectionScope = S"";
	mPropertySectionScopeless = S"";
}

void MOG_Property::Initialize( String *Section, String *PropertySection, String *PropertyKey, String *PropertyValue )
{
	mSection = Section;
	mKey = String::Concat(S"{", PropertySection, S"}", PropertyKey);
	mValue = PropertyValue;
	mPropertySection = PropertySection;
	mPropertyKey = PropertyKey;
	mPropertyValue = PropertyValue;

	mPropertySectionScope = GetPropertyScope();
	mPropertySectionScopeless = GetScopelessPropertySection();
}


void MOG_Property::Initialize( String *Section, String *JoinedPropertySectionKey, String *Value )
{
	mSection = Section ? Section : S"";
	mKey = JoinedPropertySectionKey ? JoinedPropertySectionKey : S"";
	mValue = Value ? Value : S"";
	mPropertySection = S"";
	mPropertyKey = S"";
	mPropertyValue = mValue;

	// Check if we start with a property section identifier?
	if (mKey->StartsWith(S"{"))
	{
		// Attempt to split the property values
		String* delimStr = S"{}";
		Char delimiter[] = delimStr->ToCharArray();
		String* parts[] = mKey->Split(delimiter,3);
		// Make sure we got enough parts in our split
		if (parts->Count >= 3)
		{
			// Split out the string based on the '{}' symbols
			mPropertySection = parts[1];
			mPropertyKey = parts[2];
		}
	}

	mPropertySectionScope = GetPropertyScope();
	mPropertySectionScopeless = GetScopelessPropertySection();
}


MOG_Property *MOG_Property::Clone( void )
{
	return __try_cast<MOG_Property*>(MemberwiseClone());
}


String *MOG_Property::GetPropertyScope()
{
	String* delimStr = S".";
	Char delimiter[] = delimStr->ToCharArray();
	String *parts[] = mPropertySection->Split(delimiter);

	if (parts->Count >= 2)
	{
		String *scope = parts[1];
		return scope;
	}

	return "";
}


String *MOG_Property::GetScopelessPropertySection()
{
	String* delimStr = S".";
	Char delimiter[] = delimStr->ToCharArray();
	String *parts[] = mPropertySection->Split(delimiter);
	
	if (parts->Count >= 2)
	{
		String *propertySection = parts[0];
		return propertySection;
	}

	return mPropertySection;
}


void MOG_Property::SetSection(String *section)
{
	mSection = section;
}


void MOG_Property::SetKey(String *key)
{	
	String* delimStr = S"{}";
	Char delimiter[] = delimStr->ToCharArray();
	String* parts[] = key->Split(delimiter,3);						

	// Make sure we got enough parts in our split
	if (parts->Count >= 3)
	{
		// Split out the string based on the '{}' symbols
		mPropertySection = parts[1];
		mPropertyKey = parts[2];

		mPropertySectionScope = GetPropertyScope();
		mPropertySectionScopeless = GetScopelessPropertySection();
	}

	// Set ini key
	mKey = key;
}


void MOG_Property::SetValue(String *value)
{
	mValue = value;
	mPropertyValue = value;
}


MOG_Property *MOG_Property::SetScope(MOG_Property *property, String *scopeName)
{
	// Make sure we split off any already existing scope
	String *parts[] = property->mPropertySection->Split(S"."->ToCharArray(), 2);
	// Add on our new scope name
	String *scopeSpecificPropertySection = String::Concat(parts[0], S".", scopeName);
	// Create a new scope specific property
	MOG_Property *newProperty = new MOG_Property(property->mSection, scopeSpecificPropertySection, property->mPropertyKey, property->mPropertyValue);
	return newProperty;
}

String *MOG_Property::GetPropertyAsString()
{
	if(mKey->Length >0)
	{
		return String::Concat(S"[", mSection, S"]", mKey, S"=", mValue);
	}
	else
	{
		return String::Concat(S"[", mSection, S"]{",mPropertySection,S"}" , mKey, S"=", mValue);
	}
}

void MOG_Property::ParsePropertyString(String *propertyString)
{
	// Initialize to our already existing values?
	String *section = mSection;
	String *propertySection = mPropertySection;
	String *propertyKey = mPropertyKey;
	String *propertyValue = mPropertyValue;
	String *propertyKeyAndValue = "";

	// Break up the property
	String *parts[] = propertyString->Split(S"[]{}"->ToCharArray(), 5);
	if (parts->Length == 5)
	{
		// Check if there was a Section indicated in the parts?
		if (parts[1]->Length)
		{
			section = parts[1];
		}
		// Check if there was a PropertySection indicated in the parts?
		if (parts[3]->Length)
		{
			propertySection = parts[3];
		}

		// Extract the propertyKeyAndValue
		propertyKeyAndValue = parts[4];
	}
	else if (parts->Length == 3)
	{
		// Check if there was a section?
		if (propertyString->Contains("["))
		{
			// Assume there is a Section and no PropertySection
			section = parts[1];
		}
		// Check if there was a PropertySection?
		else if (propertyString->Contains("{"))
		{
			// Assume there is a Section and no PropertySection
			propertySection = parts[1];
		}

		// Extract the propertyKeyAndValue
		propertyKeyAndValue = parts[2];
	}
	else if (parts->Length == 1)
	{
		// Extract the propertyKeyAndValue
		propertyKeyAndValue = parts[0];
	}

	// Check if we extracted a propertyKeyAndValue?
	if (propertyKeyAndValue->Length)
	{
		// Extract the PropertyValue from the PropertyKey
		String *propertyKeyParts[] = propertyKeyAndValue->Split(S"="->ToCharArray(), 2);
		// Set the PropertyKey
		propertyKey = propertyKeyParts[0];
		// Check if there was a PropertyValue?
		if (propertyKeyParts->Length == 2)
		{
			propertyValue = propertyKeyParts[1];
		}
	}

	Initialize(section, propertySection, propertyKey, propertyValue);
}

