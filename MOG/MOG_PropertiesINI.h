//--------------------------------------------------------------------------------
//	MOG_PropertiesINI.h
//	
//	
//--------------------------------------------------------------------------------

#ifndef __MOG_PropertiesINI_H__
#define __MOG_PropertiesINI_H__

#include "MOG_Define.h"
#include "MOG_Ini.h"
#include "MOG_Property.h"
#include "MOG_Filename.h"


namespace MOG {
namespace INI {

public __gc class MOG_PropertiesIni : public MOG_Ini
{
private:
	
public:
	MOG_PropertiesIni(String *filename);
	MOG_PropertiesIni(void);

	bool IsProperty(String *str);
	String *ParsePropertySection(String *str);
	String *ParsePropertyKey(String *str);

	// Returns the number of unique propertySections
	int			CountPropertySections(String *section);	// Returns the number of unique propertySections
	// Returns the number of properties within a propertySection given a specified section and propertySection
	int			CountProperties(String *section, String *propertySection);	// Returns the number of properties within a propertySection given a specified section and propertySection
	
	bool		PropertyExist(String *section, String *propertySection, String *propertyName);
	
	int			PutProperty(MOG_Property *property);
	int			PutPropertyString(String *section, String *propertySection, String *propertyName, String *propertyValue);
	// Removes a property section, its key and value
	int			RemovePropertyString(String *section, String *propertySection, String *propertyName);
	// Removes all the property sections, keys and values of a specified propertySection
	int			RemovePropertySection(String *section, String *propertySection);

	bool		RenameKeyInPropertyINI(MOG_Property *oldProperty, MOG_Property *newProperty);

	
	// Returns a value of a specific property specified by a section and propertySection
	MOG_Property*GetProperty(String *section, String *propertySection, String *propertyName);
	// Returns a value of a specific property specified by a section and propertySection
	String*		GetPropertyString(String *section, String *propertySection, String *propertyName);
	// Returns an array of MOG_Property objects that correspond to the propertySection index given
	ArrayList*	GetPropertiesByPropertySectionIndex(String *section, Int32 propertySectionIndex);
	// Returns a MOG_Property name that corresponds to the propertySection index given
	String*		GetPropertyNameByPropertySectionIndex(String *section, Int32 propertySectionIndex);
	// Returns a MOG_Property object that corresponds to the section, propertySection and propertyIndex given
	MOG_Property*GetPropertyByIndex(String *section, String *propertySection, Int32 propertyIndex);

	// Return an array of MOG_Property objects the entire ini
	ArrayList	*GetPropertyList();
	// Return an array of MOG_Property objects of all properties found in a specified section
	ArrayList	*GetPropertyList(String *section);
	// Return an array of MOG_Property objects of a specified section and propertySection
	ArrayList	*GetPropertyList(String *section, String *propertySection);
};

}
}

using namespace MOG::INI;

#endif	// __MOG_PropertiesWINI_H__

