#ifndef __MOG_DBREPORTS_H__
#define __MOG_DBREPORTS_H__

#using <mscorlib.dll>
#using <system.dll>
#using <System.Data.dll>

#include "stdlib.h"
#include "MOG_Define.h"
#include "MOG_User.h"
#include "MOG_DBAssetAPI.h"
#include "MOG_DBQueryBuilderAPI.h"


using namespace System;
using namespace System::Data;
using namespace System::Data::SqlClient;
using namespace System::Collections;

namespace MOG {
namespace DATABASE {

//This is a structure to contain a classification tied to the value of a given property.
public __gc struct MOG_DBClassificationWithProperyValue
{
	String *mClassification;
	String * mProperyValue;
};

//This is a class which contains an array list of MOG_DBClassificationWithProperyValue values.
//This class populates a list with the classification and a property value.
public __gc class MOG_DBClassificationsWithPropery
{
public:
	//constructor which dosn't actually build the list
	MOG_DBClassificationsWithPropery(void);
	//constructor that also builds the mlistOfClassificationsWithProperyValues
	MOG_DBClassificationsWithPropery(MOG_Property  *propertyForPopulation , String *rootClassification);
	//Attach the property name here so we know what property the list is for.
	String *mProperyName;
	//Attach the section here so we know exactly what property we are talking about.
	String *mSection;
	//This is the member variable to contain an array list of MOG_DBClassificationWithProperyValues of all classifications with an explicitly defined value for the property.
	ArrayList * mlistOfClassificationsWithProperyValues;
	//This function is used to find the most apropriate inherited value for a property within a specific classfication.
	String * GetPropertyValueForClosestClassification(String * classificationToLookFor);
	//This function is used to autmatically populate the mlistOfClassificationsWithProperyValues based on the passeed MOG_Property and rootClassification.
	void PopulateListOfClassificationsWithPropertyValue(MOG_Property *property, String *rootClassification);

	//This is a support function used to get a object directly cast from the mlistOfClassificationsWithProperyValues as a MOG_DBClassificationWithProperyValue
	MOG_DBClassificationWithProperyValue *GetClassificationPropertyValueForIndex(int index);
	private:
};

//This is a class for running all reports.
public __gc class MOG_DBReports
{
public:

//These are functions to call the various reports we need in the report section.
static ArrayList *RunReport_GetAssetsByClassification(String *rootClassification, ArrayList *propertiesToInclude, bool activeOnly);
static ArrayList *RunReport_GetActiveAssetsBySyncTarget(String *rootSyncTargetPath, String *platformName, ArrayList *propertiesToInclude);
static ArrayList *RunReport_GetActivePackagesByClassification(String *rootClassification, ArrayList *propertiesToInclude);
static ArrayList *RunReport_GetAllRevisionsOfAsset(MOG_Filename *assetFilename, ArrayList *propertiesToInclude);
static ArrayList *RunReport_GetArchiveAssetsByClassification(String *rootClassification, ArrayList *propertiesToInclude);
static ArrayList *RunReport_GetPackageAssets(MOG_Filename *packageName, ArrayList *propertiesToInclude, String *platform);
static ArrayList *RunReport_GetActivePackagesByClassificationWithExclusions(String* selectString, ArrayList * properties, String* rootClassification, ArrayList *mogPropertiesToFilterOn, bool excludeValue);
static ArrayList *RunReport_GetPackageGroupAssets(MOG_Filename *packageName, ArrayList *propertiesToInclude, String *platform, String *packageGroup);
//Treeview Reports
static ArrayList *ClassificationForProperty(String * rootClassification, MOG_Property *propertyToFilterOn, bool excludeValue);
//Report for the Re revision properties
static ArrayList *ClassificationForPropertyNoJumpingClassifications(String * rootClassification, MOG_Property *propertyToFilterOn, bool excludeValue);

};
}
}

#endif