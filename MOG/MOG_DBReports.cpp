#include "stdafx.h"
#include "MOG_Database.h"
#include "MOG_ControllerProject.h"
#include "MOG_ControllerSystem.h"
#include "MOG_DBReports.h"
#include "MOG_DBAPI.h"
#include "MOG_DBCache.h"


/////////////////////////////////////////////////////////////////////////////////////////////////////////////
//BEGINING OF THE MOG_DBClassificationsWithPropery FUNCTIONS
/////////////////////////////////////////////////////////////////////////////////////////////////////////////

	MOG_DBClassificationsWithPropery::MOG_DBClassificationsWithPropery(void)
	{
		mlistOfClassificationsWithProperyValues= new ArrayList();
	}

	MOG_DBClassificationsWithPropery::MOG_DBClassificationsWithPropery(MOG_Property  *propertyForPopulation , String *rootClassification)
	{
		//set the array list to a non null value
		mlistOfClassificationsWithProperyValues= new ArrayList();
		//Add the property name in the property name.
		mProperyName = propertyForPopulation->mPropertyKey;
		//Add the propery section to the member variable.
		mSection = propertyForPopulation->mPropertySection;
		//Run the function to populate the mlistOfClassificationsWithProperyValues list of classifications with 
		PopulateListOfClassificationsWithPropertyValue(propertyForPopulation, rootClassification );
	}

	//This function finds the best possible classification to inherit it's value from.
	String * MOG_DBClassificationsWithPropery::GetPropertyValueForClosestClassification(String * classificationToLookFor)
	{
		//This variable contains the degree of commonality for inheritance
		int commonChars = 0;
		//This is the property value of the classification which most closely matches the asset's classification.
		String *currentValue = "";
		//try each of the classifications 
		for(int i = 0; i < mlistOfClassificationsWithProperyValues->Count; i++)
		{
				if(classificationToLookFor->Length >= GetClassificationPropertyValueForIndex(i)->mClassification->Length)
				{
					String *compareString1 = classificationToLookFor->Substring(0,GetClassificationPropertyValueForIndex(i)->mClassification->Length);
					String *compareString2= GetClassificationPropertyValueForIndex(i)->mClassification;
					//only make the comparison if the value has a possiblility of having more common chars.		
					if(GetClassificationPropertyValueForIndex(i)->mClassification->Length > commonChars)
					{
						//if the sub string is identical to the classification make it the new current value.
						if(String::Compare(compareString1, compareString2, true) == 0)
						{
							commonChars = GetClassificationPropertyValueForIndex(i)->mClassification->Length;
							currentValue = GetClassificationPropertyValueForIndex(i)->mProperyValue;
						}
					}
				}
			}
		return currentValue;
	}

//This function populates the mlistOfClassificationsWithProperyValues with values from the database.
//The MOG_Property passed is the property values to retrieve.
//The rootClassfication.  Is the higest level classification we are worried about.  It is also the default value added if no other values are found.
void MOG_DBClassificationsWithPropery::PopulateListOfClassificationsWithPropertyValue(MOG_Property *property, String *rootClassification)
{
	//Creates a MOG_Properties for the rootClassification since we are not going to query anything above this level.
	MOG_Properties *rootClassProps = MOG_ControllerProject::GetClassificationProperties(rootClassification);
	//Build the query to get all the classifications which have this property explicity set for them.
	String *selectString = String::Concat(S"SELECT PropClassifications.FullTreeName, PropClassifications.Prop FROM( SELECT AC.*, ACP.[Value] AS Prop FROM ",
																			MOG_ControllerSystem::GetDB()->GetAssetClassificationsTable(), 
																			S" AS AC INNER JOIN ",
																			MOG_ControllerSystem::GetDB()->GetAssetClassificationsBranchLinksTable(),
																			S"AS ACBL ON	(ACBL.ClassificationID = AC.ID) INNER JOIN ",
																			MOG_ControllerSystem::GetDB()->GetAssetClassificationsPropertiesTable(),
																			S" AS ACP ON (ACP.AssetClassificationBranchLinkID = ACBL.ID) WHERE ACP.Section = '", MOG_DBAPI::FixSQLParameterString(property->mSection), S"' AND [Property] = '", MOG_DBAPI::FixSQLParameterString(property->mKey) , S"') as PropClassifications WHERE  (PropClassifications.FullTreeName LIKE '", MOG_DBAPI::FixSQLParameterString(rootClassification), S"%')" );

		//Open a connection string 
		SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());
		//Make sure the mlistOfClassificationsWithProperyValues has been initilized.
		mlistOfClassificationsWithProperyValues = new ArrayList();

		//First add the root classification to the list.
		//Create a new MOG_DBClassificationWithProperyValue object so it can be added to mlistOfClassificationsWithProperyValues
		if(rootClassification && rootClassification->Length > 0)
		{
			MOG_DBClassificationWithProperyValue *root = new MOG_DBClassificationWithProperyValue();
			root->mClassification = rootClassification;
			root->mProperyValue = rootClassProps->GetProperty(property->mSection, property->mPropertySection, property->mPropertyKey)->mValue;
			mlistOfClassificationsWithProperyValues->Add(root);
		}
		
		//Build a reader to read in all the other classifications with this property attached.
		SqlDataReader *myReader = NULL;
		try
		{
			myReader = MOG_DBAPI::GetNewDataReader(selectString, myConnection);
			//Read in all the classifications with the associated value.
			while(myReader->Read())
			{
				//create a MOG_DBClassificationWithProperyValue
				MOG_DBClassificationWithProperyValue *temp = new MOG_DBClassificationWithProperyValue();
				//populate the values.
				temp->mClassification = myReader->GetString(myReader->GetOrdinal("FullTreeName"));
				temp->mProperyValue = myReader->GetString(myReader->GetOrdinal("Prop"));
				//verify that this is not the same as the root classification.
				if(String::Compare(temp->mClassification, rootClassification, true) != 0)
				{
					//if it is not the root add to to the list.
					mlistOfClassificationsWithProperyValues->Add(temp);
				}
			}
		}
		catch(Exception *e)
		{
			String *message = String::Concat(	S"Could not get records from SQL database!\n",
												S"MOG SERVER: ", MOG_ControllerSystem::GetServerComputerName());
			MOG_Report::ReportMessage(message, e->Message, e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
		}
		__finally
		{
			if( myReader != NULL )
			{
				myReader->Close();
			}
			myConnection->Close();
		}
}

//This function just automatically casts a value in the mlistOfClassificationsWithProperyValues to a MOG_DBClassificationWithProperyValue
MOG_DBClassificationWithProperyValue *MOG_DBClassificationsWithPropery::GetClassificationPropertyValueForIndex(int index)
{
	//verify the list is initilized and the index passed exists in the array list.
	if (mlistOfClassificationsWithProperyValues && index < mlistOfClassificationsWithProperyValues->Count)
	{
		MOG_DBClassificationWithProperyValue *retValue;
		retValue = __try_cast <MOG_DBClassificationWithProperyValue *> (mlistOfClassificationsWithProperyValues->Item[index]);
		return retValue;
	}
	return NULL;
}

///////////////////////////////////////////////////////////////////////////////////////////////////
//End of the MOG_DBClassificationsWithPropery Object
////////////////////////////////////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////////////////////////////////////////////////
//BEGINNING OF THE RunReport functions
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//This is the function used to run a GetAssetsByClassification report
ArrayList *MOG_DBReports::RunReport_GetAssetsByClassification(String *rootClassification, ArrayList *propertiesToInclude, bool activeOnly)
{
	String *reportQuery = "";

	reportQuery = MOG_DBQueryBuilderAPI::MOG_ReportQueries::GenerateReport_GetAssetsByClassification(rootClassification, propertiesToInclude,activeOnly);
	return MOG_DBAssetAPI::GetAssetsWithProperties(reportQuery,propertiesToInclude);
}

//This is a function for running a report in this case GetActiveAssetsBySyncTarget
ArrayList *MOG_DBReports::RunReport_GetActiveAssetsBySyncTarget(String *rootSyncTargetPath, String *platformName, ArrayList *propertiesToInclude)
{
	String *reportQuery = "";

	reportQuery = MOG_DBQueryBuilderAPI::MOG_ReportQueries::GenerateReport_GetActiveAssetsBySyncTarget(rootSyncTargetPath, platformName, propertiesToInclude);
	return MOG_DBAssetAPI::GetAssetsWithProperties(reportQuery,propertiesToInclude);
}

//This function is for running the report in this case GetActivePackagesByClassification
ArrayList *MOG_DBReports::RunReport_GetActivePackagesByClassification(String *rootClassification, ArrayList *propertiesToInclude)
{
	String *reportQuery = "";

	reportQuery = MOG_DBQueryBuilderAPI::MOG_ReportQueries::GenerateReport_GetActivePackagesByClassification(rootClassification,propertiesToInclude);
	return MOG_DBAssetAPI::GetAssetsWithProperties(reportQuery,propertiesToInclude);
}

//This is a function used to build a report in this case GetAllRevisionsOfAsset
ArrayList *MOG_DBReports::RunReport_GetAllRevisionsOfAsset(MOG_Filename *assetFilename, ArrayList *propertiesToInclude)
{
	String *reportQuery = "";

	reportQuery = MOG_DBQueryBuilderAPI::MOG_ReportQueries::GenerateReport_GetAllRevisionsOfAsset(assetFilename, propertiesToInclude);
	return MOG_DBAssetAPI::GetAssetsWithProperties(reportQuery,propertiesToInclude);
}

//This is a function used to build a report in this case GetActivePackagesByClassifcationWithExclusions
ArrayList *MOG_DBReports::RunReport_GetActivePackagesByClassificationWithExclusions(String* selectString, ArrayList * properties, String* rootClassification, ArrayList *mogPropertiesToFilterOn, bool excludeValue)
{
	String *reportQuery = "";
	ArrayList *listOfExclusionProperties = new ArrayList();
	MOG_Property *tempPropertyExclude = new MOG_Property;
	reportQuery = MOG_DBQueryBuilderAPI::MOG_ReportQueries::GenerateReport_GetActivePackagesByClassification(rootClassification, properties);
	listOfExclusionProperties->Add(MOG_PropertyFactory::MOG_Asset_InfoProperties::New_IsPackage(false));
	return MOG_DBAssetAPI::GetAssetsWithPropertiesFilterByPropertyValue(reportQuery, properties, rootClassification,listOfExclusionProperties ,true);
}

//This is a function used to build a report in this case (GetArchiveAssetsByClassification
ArrayList *MOG_DBReports::RunReport_GetArchiveAssetsByClassification(String *rootClassification, ArrayList *propertiesToInclude)
{
	String *reportQuery = "";
// JohnRen - Changed to use the report that includes all asset revisions rather than just the current revision of each asset
//	reportQuery = MOG_DBQueryBuilderAPI::MOG_ReportQueries::GenerateReport_GetArchiveAssetsByClassification(rootClassification, propertiesToInclude);
	reportQuery = MOG_DBQueryBuilderAPI::MOG_ReportQueries::GenerateReport_GetAllRevisionsOfAssets(rootClassification, propertiesToInclude);
	return MOG_DBAssetAPI::GetAssetsWithProperties(reportQuery,propertiesToInclude);
}

//This is a function used to build a report in this case GetPackageAssets
ArrayList *MOG_DBReports::RunReport_GetPackageAssets(MOG_Filename *packageName, ArrayList *properties, String *platform)
{
	String *reportQuery = "";
	reportQuery = MOG_DBQueryBuilderAPI::MOG_ReportQueries::GenerateReport_PackageAssets(packageName, properties, platform);
	return MOG_DBAssetAPI::GetAssetsWithProperties(reportQuery,properties);
}

//This is a function used to build a report in the case GetPackageGroupAssets
ArrayList *MOG_DBReports::RunReport_GetPackageGroupAssets(MOG_Filename *packageName, ArrayList *propertiesToInclude, String *platform, String *packageGroup)
{

	//JDTODO create a query that actual returns the right results.  This is just a place holder report for now.
	return RunReport_GetPackageAssets(packageName, propertiesToInclude, platform);
}

ArrayList *MOG_DBReports::ClassificationForProperty(String * rootClassification, MOG_Property *propertyToFilterOn, bool excludeValue)
{
	//initilize a list to return which will be a list of classifications as String Values  where Library is true.
	ArrayList *listOfClassificationsWithPropertyValue = new ArrayList();
	ArrayList *listOfClassificationsToReturn = new ArrayList();
	//Get a list of all classifications where the library property is set.
	MOG_DBClassificationsWithPropery *classificationsWithPropertyValueSet = new MOG_DBClassificationsWithPropery(propertyToFilterOn, rootClassification);
	//iterate through this list to find all the ones that have The apropriate property value.
	for(int i = 0; i < classificationsWithPropertyValueSet->mlistOfClassificationsWithProperyValues->Count; i++)
	{
		//We we are excluding the value then get everyone that dosn't have this value.
		if(excludeValue)
		{
			if(String::Compare(classificationsWithPropertyValueSet->GetClassificationPropertyValueForIndex(i)->mProperyValue, propertyToFilterOn->mValue, true) != 0)
			{
				listOfClassificationsWithPropertyValue->Add(classificationsWithPropertyValueSet->GetClassificationPropertyValueForIndex(i)->mClassification);
			}
		}
		//otherwise we are including the asset and we only want to return classifications where the value is actually set.
		else
		{
			if(String::Compare(classificationsWithPropertyValueSet->GetClassificationPropertyValueForIndex(i)->mProperyValue, propertyToFilterOn->mValue, true) == 0)
			{
				listOfClassificationsWithPropertyValue->Add(classificationsWithPropertyValueSet->GetClassificationPropertyValueForIndex(i)->mClassification);
			}
		}
	}

	//for each classification where the property is set  get the sub classifications where the value is not explicitly set to an in appropriate value.
	for(int x = 0; x < listOfClassificationsWithPropertyValue->Count; x++)
	{
		//Add the root classifications to the list to return.
		listOfClassificationsToReturn->Add(__try_cast <String*>(listOfClassificationsWithPropertyValue->Item[x]));
		//Create a select string which will return all the values not explicity set to the appropriate value.
		String *selectString = MOG_DBQueryBuilderAPI::MOG_TreeviewQueries::QueryToGetClassificationsUnderRootClassification(__try_cast <String*>(listOfClassificationsWithPropertyValue->Item[x]),propertyToFilterOn);
		//build the data reader.
		SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());
		SqlDataReader *myReader = MOG_DBAPI::GetNewDataReader(selectString, myConnection);
		try
		{
			while(myReader->Read())
			{
				//Get the classification to use.
				String *tempClassification = myReader->GetString(myReader->GetOrdinal("FullTreeName"));
				//If we are excluding on the the passed property
				if(excludeValue)
				{
					//Check to see if we have an explicitly named appropriate value.
					if(String::Compare(MOG_DBAPI::SafeStringRead(myReader, propertyToFilterOn->mPropertyKey), propertyToFilterOn->mValue, true) != 0)
					{
						if(!listOfClassificationsToReturn->Contains(tempClassification))
						{
							//If so add it to the list.
							listOfClassificationsToReturn->Add(tempClassification);
						}
					}
					//If we don't have an explicitly named value 
					else
					{
						//See if the inherited value falls in to the appropriate values.
						if(String::Compare(classificationsWithPropertyValueSet->GetPropertyValueForClosestClassification(tempClassification), propertyToFilterOn->mValue, true) != 0)
						{
							if(!listOfClassificationsToReturn->Contains(tempClassification))
							{
								//If so add it to the return list.
								listOfClassificationsToReturn->Add(tempClassification);
							}
						}
					}
				}
				//If we are not excluding on the value we are only including the given value.
				else
				{
					//Check to see if we have an explicity defined appropriate value.
					if(String::Compare(MOG_DBAPI::SafeStringRead(myReader, propertyToFilterOn->mPropertyKey), propertyToFilterOn->mValue, true) == 0)
					{
						if(!listOfClassificationsToReturn->Contains(tempClassification))
						{
							//If we do add it.
							listOfClassificationsToReturn->Add(tempClassification);
						}
					}
					//if we don't have an explicitly defined value
					else
					{
						//get the inherited value
						if(String::Compare(classificationsWithPropertyValueSet->GetPropertyValueForClosestClassification(tempClassification), propertyToFilterOn->mValue, true) == 0)
						{
							if(!listOfClassificationsToReturn->Contains(tempClassification))
							{
								//if the inherited value is an appropriate value add it.
								listOfClassificationsToReturn->Add(tempClassification);
							}
						}
					}
				}
			}
		}
		catch(Exception *e)
		{
			String *message = String::Concat(	S"Could not get records from SQL database!\n",
												S"MOG SERVER: ", MOG_ControllerSystem::GetServerComputerName());
			MOG_Report::ReportMessage(message, e->Message, e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
		}
		__finally
		{
			if(myReader)
			{
				myReader->Close();
			}
			myConnection->Close();
		}
	}
	//return the processed list.
	return listOfClassificationsToReturn;
}


ArrayList *MOG_DBReports::ClassificationForPropertyNoJumpingClassifications(String * rootClassification, MOG_Property *propertyToFilterOn, bool excludeValue)
{
	//initilize a list to return which will be a list of classifications as String Values  where Library is true.
	ArrayList *listOfClassificationsWithPropertyValue = new ArrayList();
	ArrayList *listOfClassificationsToReturn = new ArrayList();
	ArrayList *listOfSubClassificationsWithNonTargetProperty = new ArrayList();
	//Get a list of all classifications where the library property is set.
	MOG_DBClassificationsWithPropery *classificationsWithPropertyValueSet = new MOG_DBClassificationsWithPropery(propertyToFilterOn, rootClassification);
	//iterate through this list to find all the ones that have The apropriate property value.
	for(int i = 0; i < classificationsWithPropertyValueSet->mlistOfClassificationsWithProperyValues->Count; i++)
	{
		//We we are excluding the value then get everyone that dosn't have this value.
		if(excludeValue)
		{
			if(String::Compare(classificationsWithPropertyValueSet->GetClassificationPropertyValueForIndex(i)->mProperyValue, propertyToFilterOn->mValue, true) != 0)
			{
				listOfClassificationsWithPropertyValue->Add(classificationsWithPropertyValueSet->GetClassificationPropertyValueForIndex(i)->mClassification);
			}
		}
		//otherwise we are including the asset and we only want to return classifications where the value is actually set.
		else
		{
			if(String::Compare(classificationsWithPropertyValueSet->GetClassificationPropertyValueForIndex(i)->mProperyValue, propertyToFilterOn->mValue, true) == 0)
			{
				listOfClassificationsWithPropertyValue->Add(classificationsWithPropertyValueSet->GetClassificationPropertyValueForIndex(i)->mClassification);
			}
		}
	}
	listOfSubClassificationsWithNonTargetProperty = ClassificationForProperty(rootClassification, propertyToFilterOn, !excludeValue);
	//remove classifications that are subclasses of any classification in the listOfSubClassificationsWithNonTargetProperty list
	for(int iClassesToCheck = 0; iClassesToCheck < listOfClassificationsWithPropertyValue->Count;iClassesToCheck++)
	{
		String *classificationToCheck = __try_cast <String*>(listOfClassificationsWithPropertyValue->Item[iClassesToCheck]);
		for(int iRemoveIt = 0; iRemoveIt < listOfSubClassificationsWithNonTargetProperty->Count;iRemoveIt++)
		{
			String *stringToLookFor = __try_cast <String*>(listOfSubClassificationsWithNonTargetProperty->Item[iRemoveIt]);
			if(stringToLookFor->Length <= classificationToCheck->Length)
			{
				if(classificationToCheck->IndexOf(stringToLookFor, 0 ,stringToLookFor->Length) > -1)
				{
					listOfClassificationsWithPropertyValue->Remove(classificationToCheck);
				}
			}
		}
	}
	//for each classification where the property is set  get the sub classifications where the value is not explicitly set to an in appropriate value.
	for(int x = 0; x < listOfClassificationsWithPropertyValue->Count; x++)
	{
		//Add the root classifications to the list to return.
		listOfClassificationsToReturn->Add(__try_cast <String*>(listOfClassificationsWithPropertyValue->Item[x]));
		//Create a select string which will return all the values not explicity set to the appropriate value.
		String *selectString = MOG_DBQueryBuilderAPI::MOG_TreeviewQueries::QueryToGetClassificationsUnderRootClassification(__try_cast <String*>(listOfClassificationsWithPropertyValue->Item[x]),propertyToFilterOn);
		//Get the list of classifications to exclude on
		
		//build the data reader.
		SqlConnection *myConnection = MOG_DBAPI::GetOpenSqlConnection(MOG_ControllerSystem::GetDB()->GetConnectionString());
		SqlDataReader *myReader = MOG_DBAPI::GetNewDataReader(selectString, myConnection);
		try
		{
			while(myReader->Read())
			{
				//Get the classification to use.
				String *tempClassification = myReader->GetString(myReader->GetOrdinal("FullTreeName"));
				//If we are excluding on the the passed property
				if(excludeValue)
				{
					//Check to see if we have an explicitly named appropriate value.
					if(String::Compare(MOG_DBAPI::SafeStringRead(myReader, propertyToFilterOn->mPropertyKey), propertyToFilterOn->mValue, true) != 0)
					{
						//If so add it to the list.
						listOfClassificationsToReturn->Add(tempClassification);
					}
					//If we don't have an explicitly named value 
					else
					{
						//See if the inherited value falls in to the appropriate values.
						if(String::Compare(classificationsWithPropertyValueSet->GetPropertyValueForClosestClassification(tempClassification), propertyToFilterOn->mValue, true) != 0)
						{
							//If so add it to the return list.
							listOfClassificationsToReturn->Add(tempClassification);
						}
					}
				}
				//If we are not excluding on the value we are only including the given value.
				else
				{
					//Check to see if we have an explicity defined appropriate value.
					if(String::Compare(MOG_DBAPI::SafeStringRead(myReader, propertyToFilterOn->mPropertyKey), propertyToFilterOn->mValue, true) == 0)
					{
						//If we do add it.
						listOfClassificationsToReturn->Add(tempClassification);
					}
					//if we don't have an explicitly defined value
					else
					{
						//get the inherited value
						if(String::Compare(classificationsWithPropertyValueSet->GetPropertyValueForClosestClassification(tempClassification), propertyToFilterOn->mValue, true) == 0)
						{
							//if the inherited value is an appropriate value add it.
							listOfClassificationsToReturn->Add(tempClassification);
						}
					}
				}
			}
		}
		catch(Exception *e)
		{
			String *message = String::Concat(	S"Could not get records from SQL database!\n",
												S"MOG SERVER: ", MOG_ControllerSystem::GetServerComputerName());
			MOG_Report::ReportMessage(message, e->Message, e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
		}
		__finally
		{
			if(myReader)
			{
				myReader->Close();
			}
			myConnection->Close();
		}
	}
	//return the processed list.
	return listOfClassificationsToReturn;
}











