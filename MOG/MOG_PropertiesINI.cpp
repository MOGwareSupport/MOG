//--------------------------------------------------------------------------------
//	MOG_PropertiesINI.h
//	
//	
//--------------------------------------------------------------------------------

#include "stdafx.h"

#include "MOG_Define.h"
#include "MOG_PropertiesINI.h"
#include "MOG_StringUtils.h"
#include "MOG_DosUtils.h"
#include "MOG_Time.h"

using namespace System::Threading;
using namespace System::Windows::Forms;

#include <stdio.h>



//***************************************************************
MOG_PropertiesIni::MOG_PropertiesIni(String *fullFilename) : MOG_Ini(fullFilename)
{
}

//***************************************************************
MOG_PropertiesIni::MOG_PropertiesIni(void) : MOG_Ini()
{
}

//***************************************************************
bool MOG_PropertiesIni::PropertyExist(String *section, String *propertySection, String *propertyName)
{
	String *propertyKey = String::Concat(S"{", propertySection, S"}", propertyName);
	return KeyExist(section, propertyKey);
}


//***************************************************************
int MOG_PropertiesIni::CountPropertySections(String *section)
{
	MOG_IniSection *pSection = SectionFind(section);
	if (pSection)
	{
		HybridDictionary *foundSections = new HybridDictionary(true);

		// Iterate through the keys
		IDictionaryEnumerator *keyEnumerator = pSection->mKeys->GetEnumerator();
		while ( keyEnumerator->MoveNext() )
		{
			MOG_IniKey *pKey = __try_cast<MOG_IniKey*>(keyEnumerator->Value);

			if (IsProperty(pKey->mKey))
			{
				String *workingPropertySection = ParsePropertySection(pKey->mKey);
				foundSections->Item[workingPropertySection] = workingPropertySection;
			}		
		}

		return 	foundSections->Count;
	}

	MOG_Report::ReportMessage("MOG_PropertiesIni::CountPropertySections ", String::Concat(mFullFilename, S"\nSECTION: ", section, S"\nSection not found!"), Environment::StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);

	return 0;	
}

//***************************************************************
int MOG_PropertiesIni::CountProperties(String *section, String *propertySection)
{
	String *propertyKey = String::Concat(S"{", propertySection, S"}");

	MOG_IniSection *pSection = SectionFind(section);
	if (pSection)
	{
		HybridDictionary *foundSections = new HybridDictionary(true);

		// Iterate through the keys
		IDictionaryEnumerator *keyEnumerator = pSection->mKeys->GetEnumerator();
		while ( keyEnumerator->MoveNext() )
		{
			MOG_IniKey *pKey = __try_cast<MOG_IniKey*>(keyEnumerator->Value);

			// Is this key of the same propertySection?
			if (String::Compare(ParsePropertySection(pKey->mKey), propertySection, true) == 0)
			{
				foundSections->Item[pKey->mKey] = pKey->mKey;
			}
		}

		return 	foundSections->Count;
	}

	MOG_Report::ReportMessage("MOG_PropertiesIni::CountProperty ", String::Concat(mFullFilename, S"\nSECTION: ", section, S"\nSection not found!"), Environment::StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);

	return 0;	
}

//***************************************************************
MOG_Property *MOG_PropertiesIni::GetProperty(String *section, String *propertySection, String *propertyName)
{
	String *propertyKey = String::Concat(S"{", propertySection, S"}", propertyName);
	MOG_Property *property = new MOG_Property(section, propertySection, propertyName, GetString(section, propertyKey));
	return property;
}

//***************************************************************
String *MOG_PropertiesIni::GetPropertyString(String *section, String *propertySection, String *propertyName)
{
	String *propertyKey = String::Concat(S"{", propertySection, S"}", propertyName);

	return GetString(section, propertyKey);
}

//***************************************************************
bool MOG_PropertiesIni::IsProperty(String *str)
{
	// Get the indexes of our property delimiters
	int leftPropertyDelimiter = str->IndexOf("{");
	int rightPropertyDelimiter = str->IndexOf("}");

	// Do we have both a left and right property delimiter and is the right one left of the '='?
	if (leftPropertyDelimiter != -1 &&
		rightPropertyDelimiter != -1)
	{
		return true;
	}

	return false;
}

//***************************************************************
String *MOG_PropertiesIni::ParsePropertySection(String *str)
{
	String* delimStr = S"{}";
	Char delimiter[] = delimStr->ToCharArray();

	// Split out the string based on the '{}' symbols
	String* parts[] = str->Split(delimiter,3);

	// Make sure we got enough parts in our split
	if (parts->Count >= 2)
	{
		// Return the propertySection name
		return parts[1];
	}
	
	// No propertySection found
	return "";
}

String *MOG_PropertiesIni::ParsePropertyKey(String *str)
{
	String* delimStr = S"{}";
	Char delimiter[] = delimStr->ToCharArray();

	// Split out the string based on the '{}' symbols
	String* parts[] = str->Split(delimiter,3);

	// Make sure we got enough parts in our split
	if (parts->Count >= 3)
	{
		// Return the propertySection name
		return parts[2];
	}
	
	// No propertySection found
	return "";
}

//***************************************************************
ArrayList *MOG_PropertiesIni::GetPropertyList(String *section, String *propertySection)
{
	ArrayList *properties = new ArrayList();

	MOG_IniSection *pSection = SectionFind(section);
	if (pSection)
	{
		// Iterate through the keys
		IDictionaryEnumerator *keyEnumerator = pSection->mKeys->GetEnumerator();
		while ( keyEnumerator->MoveNext() )
		{
			MOG_IniKey *pKey = __try_cast<MOG_IniKey*>(keyEnumerator->Value);
			MOG_Property *property = new MOG_Property(section, pKey->mKey, pKey->mValue);

			// Is this a property and does it match the propertySection requested?
			if (String::Compare(property->mPropertySection, propertySection, true) == 0)
			{
				// Add to our list
				properties->Add(property);
			}
		}
	}

	// Return all the properties
	return properties;
}

//***************************************************************
ArrayList *MOG_PropertiesIni::GetPropertyList(String *section)
{
	ArrayList *properties = new ArrayList();

	MOG_IniSection *pSection = SectionFind(section);
	if (pSection)
	{
		// Iterate through the keys
		IDictionaryEnumerator *keyEnumerator = pSection->mKeys->GetEnumerator();
		while ( keyEnumerator->MoveNext() )
		{
			MOG_IniKey *pKey = __try_cast<MOG_IniKey*>(keyEnumerator->Value);

			// Add property to our list
			MOG_Property *property = new MOG_Property(section, pKey->mKey, pKey->mValue);
			properties->Add(property);
		}
	}

	// Return all the properties
	return properties;
}

//***************************************************************
ArrayList *MOG_PropertiesIni::GetPropertyList()
{
	ArrayList *properties = new ArrayList();

	// Iterate through the sections
	IEnumerator *sectionEnumerator = mSections->Values->GetEnumerator();
	while ( sectionEnumerator->MoveNext() )
	{
		MOG_IniSection *pSection = __try_cast<MOG_IniSection*>(sectionEnumerator->Current);
		if (pSection)
		{
			properties->AddRange(GetPropertyList(pSection->mSection));
		}
	}

	// Return all the properties
	return properties;
}

//***************************************************************
ArrayList *MOG_PropertiesIni::GetPropertiesByPropertySectionIndex(String *section, Int32 propertySectionIndex)
{
	bool found = false;
	String *propertySection = S"";
	ArrayList *properties = new ArrayList();
	ArrayList *propertyNames = new ArrayList();
	MOG_IniSection *pSection = SectionFind(section);

	if (pSection)
	{
		int propertySectionCount = -1;
		// Iterate through the keys
		IDictionaryEnumerator *keyEnumerator = pSection->mKeys->GetEnumerator();
		while ( keyEnumerator->MoveNext() )
		{
			MOG_IniKey *pKey = __try_cast<MOG_IniKey*>(keyEnumerator->Value);
			String *workingPropertySection = ParsePropertySection(pKey->mKey);

			if (IsProperty(pKey->mKey))
			{
				// Check to see if we have already found this property section by looping though our internal array
				for (int i = 0; i < propertyNames->Count; i++)
				{
					String *key = __try_cast<String*>(propertyNames->Item[i]);
					if (String::Compare(key, workingPropertySection, true) == 0)
					{
						// We already have one
						found = true;
						break;
					}					
				}

				// Only check the index of this property if it is truly a unique one
				if (!found)
				{
					// Add this key for our internal counting 
					propertyNames->Add(workingPropertySection);

					// Increment our count of unique propertySections
					propertySectionCount ++;
				}

				// If our current count od propertSections = the desired one, add this property to the array
				if (propertySectionCount == propertySectionIndex)
				{
					propertySection = workingPropertySection;					
				}

				// Check if this is our desired propertySection?
				if (String::Compare(propertySection, workingPropertySection, true) == 0)
				{
					// Create and load a new property object
					MOG_Property *property	= new MOG_Property(section, pKey->mKey, pKey->mValue);
					// Add it to our properties array
                    properties->Add(property);
				}

				// Reset our found bool
				found = false;
			}		
		}
		return properties;
	}

	MOG_Report::ReportMessage("MOG_PropertiesIni::GetPropertiesByPropertySectionIndex ", String::Concat(mFullFilename, S"\nSECTION: ", section, S"\nPROPERTY SECTION INDEX: ", Convert::ToString(propertySectionIndex), S"\nSection and or Property Section Index not found!\n"), Environment::StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);

	return properties;
}


//***************************************************************
String* MOG_PropertiesIni::GetPropertyNameByPropertySectionIndex(String *section, Int32 propertySectionIndex)
{
	bool found = false;
	String *propertySection = S"";
	ArrayList *properties = new ArrayList();
	ArrayList *propertyNames = new ArrayList();

	MOG_IniSection *pSection = SectionFind(section);
	if (pSection)
	{
		int propertySectionCount = -1;
		// Iterate through the keys
		IDictionaryEnumerator *keyEnumerator = pSection->mKeys->GetEnumerator();
		while ( keyEnumerator->MoveNext() )
		{
			MOG_IniKey *pKey = __try_cast<MOG_IniKey*>(keyEnumerator->Value);
			String *workingPropertySection = ParsePropertySection(pKey->mKey);

			if (IsProperty(pKey->mKey))
			{
				// Check to see if we have already found this property section by looping though our internal array
				for (int i = 0; i < propertyNames->Count; i++)
				{
					String *key = __try_cast<String*>(propertyNames->Item[i]);
					if (String::Compare(key, workingPropertySection, true) == 0)
					{
						// We already have one
						found = true;
						break;
					}					
				}

				// Only check the index of this property if it is truly a unique one
				if (!found)
				{
					// Add this key for our internal counting 
					propertyNames->Add(workingPropertySection);

					// Increment our count of unique propertySections
					propertySectionCount ++;
				}

				// If our current count od propertSections = the desired one, add this property to the array
				if (propertySectionCount == propertySectionIndex)
				{
					return workingPropertySection;
				}

				// Reset our found bool
				found = false;
			}		
		}
	}

	MOG_Report::ReportMessage("MOG_PropertiesIni::GetPropertyNameByPropertySectionIndex ", String::Concat(mFullFilename, S"\nSECTION: ", section, S"\nPROPERTY SECTION INDEX: ", Convert::ToString(propertySectionIndex), S"\nSection and or Property Section Index not found!\n"), Environment::StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);

	return "";
}


//***************************************************************
MOG_Property* MOG_PropertiesIni::GetPropertyByIndex(String *section, String *propertySection, Int32 propertyIndex)
{
	String *propertyKey = String::Concat(S"{", propertySection, S"}");
    MOG_IniSection *pSection = SectionFind(section);

	if (pSection)
	{
		int count = 0;
		// Iterate through the keys
		IDictionaryEnumerator *keyEnumerator = pSection->mKeys->GetEnumerator();
		while ( keyEnumerator->MoveNext() )
		{
			MOG_IniKey *pKey = __try_cast<MOG_IniKey*>(keyEnumerator->Value);
			String *sourcePropertySection = ParsePropertySection(pKey->mKey);

			// Check for matching propertySections
			if (String::Compare(sourcePropertySection, propertySection, true) == 0)
			{
				// If this is the property that we are looking for, return it
				if (count == propertyIndex)
				{
					// Create and load a new property object
					MOG_Property *property	= new MOG_Property(section, pKey->mKey, pKey->mValue);
					return property;
				}
				else
				//if (pKey->mKey->ToLower()->IndexOf(propertyKey->ToLower()) != -1)
				{
					// Else, lets keep counting
					count ++;
				}
			}

			//if (count == propertyIndex)
			//{
			//	// Create and load a new property object
			//	MOG_Property *property	= new MOG_Property(section, pKey->mKey, pKey->mValue);
			//	return property;
			//}
		}
	}

	return NULL;
}

//***************************************************************
int MOG_PropertiesIni::PutProperty(MOG_Property *property)
{
	MOG_ASSERT_THROW(property, MOG_Exception::MOG_EXCEPTION_InvalidData, "MOG_INI - PutPropertyString requires a valid property");
	MOG_ASSERT_THROW(property->mSection->Length, MOG_Exception::MOG_EXCEPTION_InvalidData, "MOG_INI - PutPropertyString requires a valid section");
	MOG_ASSERT_THROW(property->mKey->Length, MOG_Exception::MOG_EXCEPTION_InvalidData, "MOG_INI - PutPropertyString requires a valid key");
	if (property->mPropertyValue == NULL)
	{
		return 0;
	}

	// Now use the normal API of MOG_Ini
	return PutString(property->mSection, property->mKey, property->mPropertyValue);
}

//***************************************************************
int MOG_PropertiesIni::PutPropertyString(String *section, String *propertySection, String *propertyName, String *propertyValue)
{
	MOG_ASSERT_THROW(section->Length, MOG_Exception::MOG_EXCEPTION_InvalidData, "MOG_INI - PutPropertyString requires a valid section");
	MOG_ASSERT_THROW(propertySection->Length, MOG_Exception::MOG_EXCEPTION_InvalidData, "MOG_INI - PutPropertyString requires a valid property");
	MOG_ASSERT_THROW(propertyName->Length, MOG_Exception::MOG_EXCEPTION_InvalidData, "MOG_INI - PutPropertyString requires either a valid key or str");
	if (propertyValue == NULL)
	{
		return 0;
	}

	// Create our property string to look like the following: {property}Key=str
	String *propertyKey = String::Concat(S"{", propertySection, S"}", propertyName);

	// Now use the normal API of MOG_Ini
	return PutString(section, propertyKey, propertyValue);
}

//***************************************************************
int MOG_PropertiesIni::RemovePropertyString(String *section, String *propertySection, String *propertyName)
{
	MOG_ASSERT_THROW(section->Length, MOG_Exception::MOG_EXCEPTION_InvalidData, "MOG_INI - PutPropertyString requires a valid section");
	MOG_ASSERT_THROW(propertySection->Length, MOG_Exception::MOG_EXCEPTION_InvalidData, "MOG_INI - PutPropertyString requires a valid property");
	MOG_ASSERT_THROW(propertyName->Length, MOG_Exception::MOG_EXCEPTION_InvalidData, "MOG_INI - PutPropertyString requires either a valid key or str");

	// Create our property string to look like the following: {property}Key=str
	String *PropertyKey = String::Concat(S"{", propertySection, S"}", propertyName);

	// Now use the normal API of MOG_Ini
	return RemoveString(section, PropertyKey);
}

//***************************************************************
int MOG_PropertiesIni::RemovePropertySection(String *section, String *propertySection)
{
	MOG_ASSERT_THROW(section->Length, MOG_Exception::MOG_EXCEPTION_InvalidData, "MOG_INI - PutPropertyString requires a valid section");
	MOG_ASSERT_THROW(propertySection->Length, MOG_Exception::MOG_EXCEPTION_InvalidData, "MOG_INI - PutPropertyString requires a valid property");
	
	// Find the section
	MOG_IniSection *pSection = SectionFind(section);
	if (pSection)
	{
		ArrayList *items = new ArrayList();

		// Iterate through the keys
		IDictionaryEnumerator *keyEnumerator = pSection->mKeys->GetEnumerator();
		while ( keyEnumerator->MoveNext() )
		{
			MOG_IniKey *pKey = __try_cast<MOG_IniKey*>(keyEnumerator->Value);

			// Does this propertySection match our desired propertySection?
			if (String::Compare(ParsePropertySection(pKey->mKey), propertySection, true) == 0)
			{
				// Schedule this item for removal
				items->Add(pKey->mKey);
			}
		}

		// Remove all the items schedule for removal
		for (int i = 0; i < items->Count; i++)
		{
			String* key = dynamic_cast<String*>(items->Item[i]);

			// Remove it
			MOG_ASSERT_THROW(RemoveString(section, key), MOG_Exception::MOG_EXCEPTION_InvalidData, String::Concat(S"MOG_INI - RemovePropertySection was unable to remove property(", key, S") of section(", section, S")"));
			mChanged = true;
		}
	}
	
	// Now use the normal API of MOG_Ini
	return true;
}

//a function that allows us to rename a property in the Property.ini for an asset  it takes the old property and removes it then adds it to the 
bool MOG_PropertiesIni::RenameKeyInPropertyINI(MOG_Property *oldProperty, MOG_Property *newProperty)
{
	if(PropertyExist(oldProperty->mSection, oldProperty->mPropertySection, oldProperty->mPropertyKey))
	{
		RemovePropertyString(oldProperty->mSection, oldProperty->mPropertySection, oldProperty->mPropertyKey);
		PutProperty(newProperty);
	}
	return true;
}


