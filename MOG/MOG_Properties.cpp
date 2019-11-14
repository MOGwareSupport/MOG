//--------------------------------------------------------------------------------
//	MOG_Properties.cpp
//	
//	
//--------------------------------------------------------------------------------

#include "StdAfx.h"

#include "MOG_ControllerSystem.h"
#include "MOG_ControllerProject.h"
#include "MOG_ControllerPackage.h"
#include "MOG_ControllerRepository.h"
#include "MOG_DBInboxAPI.h"
#include "MOG_DBReports.h"
#include "MOG_CommandFactory.h"
#include "MOG_ControllerReinstance.h"
#include "MOG_Tokens.h"

#include "MOG_Properties.h"

using namespace System::Collections::Generic;



public __gc class MOG_PropertiesCacheObject
{
public:
	Object *mThread;
	MOG_Properties *mProperties;
	int mReferenceCount;
};



//--------------------------------------------------------------------------------
//	MOG_Properties
//--------------------------------------------------------------------------------
MOG_Properties::MOG_Properties()
{
	// Initialize our properties class
	Initialize(NULL, S"", S"", NULL);
}


MOG_Properties::MOG_Properties(ArrayList *properties)
{	
	// Initialize our properties class
	Initialize(NULL, "", "", NULL);
	// Since this is not related to any asset, we can allow this to be modified
	mCanModify = true;

	// Make sure we have a valid list of properties?
	if (properties)
	{
		// Stuff these properties into our mProperties
		for (int i = 0; i < properties->Count; i++)
		{
			MOG_Property *property = __try_cast<MOG_Property *>(properties->Item[i]);
			mProperties->PutProperty(property);
		}
	}
}


MOG_Properties::MOG_Properties(MOG_Filename *assetFilename)
{
	ArrayList *properties = NULL;

	// Initialize our properties class
	Initialize(assetFilename, assetFilename->GetAssetPlatform(), assetFilename->GetAssetClassification(), NULL);
	PopulateInheritance(mClassification);

	// Determin where this asset is located?
	// Check if this asset is within the inboxes?
	if (assetFilename->IsWithinInboxes() || assetFilename->IsLocal())
	{
		// Retrieve the properties from the Inbox Database
		properties = MOG_DBInboxAPI::GetAllInboxAssetProperties(assetFilename);
		// Strip out the already inherited properties of this inbox asset...
		// Inbox Asset Properties currently contain their inherited properties as well as the non-inheerited properties
		// This was done to help improve the speed of inbox database queries for populating the inbox columns
		// Stuff these properties into our mProperties
		int i = 0;
		while (i < properties->Count)
		{
			MOG_Property *thisProperty = __try_cast<MOG_Property *>(properties->Item[i]);

			// Check if this property is contained within the relationships section?
			if (this->IsPropertyAlreadyInherited(thisProperty))
			{
				bool bIsInheritedProperty = true;

				// Check if this is a relationship?
				if (String::Compare(thisProperty->mSection, PROPTEXT_Relationships, true) == 0)
				{
					bool bOverriddenRelationship = false;

					// We need to track relationships and watch for when they have been overridden.
					// Scan properties looking for related relationships that indicate this relationship is being overridden?
					for (int r = 0; r < properties->Count; r++)
					{
						MOG_Property *nextProperty = __try_cast<MOG_Property *>(properties->Item[r]);

						// Check if nextProperty is related to this thisProperty?
						if (String::Compare(nextProperty->mSection, thisProperty->mSection, true) == 0 &&
							String::Compare(nextProperty->mPropertySection, thisProperty->mPropertySection, true) == 0 &&
							String::Compare(nextProperty->mPropertyKey, thisProperty->mPropertyKey, true) != 0)
						{
							// Check if this property is contained within the relationships section?
							if (!this->IsPropertyAlreadyInherited(nextProperty))
							{
								bOverriddenRelationship = true;
								break;
							}
						}
					}

					// Check if this relationship was never overridden...meaning it can be omitted
					if (bOverriddenRelationship)
					{
						bIsInheritedProperty = false;
					}
				}

				// Check if we determined this is an inherited property and can be ommited?
				if (bIsInheritedProperty)
				{
					// Remove this property from the obtained inbox asset properties becaise it is already inherited
					properties->RemoveAt(i);
					continue;
				}
			}

			// Increment our counter being this property is staying and not being removed
			i++;
		}

		// Process the inheritence
	}
	else
	{
		// Get the revisionTimeStamp for this asset
		String *revisionTimeStamp = assetFilename->GetVersionTimeStamp();
		if (!revisionTimeStamp->Length)
		{
			// Get the current revisionTimeStamp for this asset
			revisionTimeStamp = MOG_DBAssetAPI::GetAssetVersion(assetFilename);
		}

		// Retrieve the asset's properties from the database
		properties = MOG_DBAssetAPI::GetAllAssetVersionProperties(assetFilename, revisionTimeStamp);
	}

	// Push the non-inherited properties
	PushProperties(mProperties, properties);
}


MOG_Properties::MOG_Properties(String *classification)
{
	// Initialize our properties class
	Initialize(NULL, S"", classification, NULL);
	PopulateInheritance(mClassification);

	// Get the properties for this classification
	ArrayList *properties = MOG_DBAssetAPI::GetAllAssetClassificationProperties(classification, S"");
	// Push the non-inherited properties
	PushProperties(mProperties, properties);
}

void MOG_Properties::OnPropertyChanged( PropertyEventArgs *e )
{
	if( PropertyChanged != NULL )
	{
		PropertyChanged( this, e);
	}
}

void MOG_Properties::Initialize(MOG_Filename *assetFilename, String *scopeName, String *classification, MOG_PropertiesIni *propertiesFile)
{
	// Set Defaults
	mScopeExplicitMode = false;
	mCanModify = false;
	mModified = false;
	mDBClassification = false;
	mDBInbox = false;
	mDBRepository = false;
	mScopeName = S"All";
	mImmeadiateMode = false;

	// Check if an assetFilename was specified?
	if (assetFilename)
	{
		// Retain the name of the asset
		mAssetFilename = assetFilename;

		// Determin where this asset is located?
		// Check if this asset is within the MOG Repository?
		if (mAssetFilename->IsWithinRepository())
		{
			// Indicate the AssetProperties Database
			mDBRepository = true;
		}
		// Check if this asset is within the inboxes?
		if (mAssetFilename->IsWithinInboxes() || mAssetFilename->IsLocal())
		{
			// Indicate the Inbox Database
			mDBInbox = true;
		}
	}
	else
	{
		// Make sure we never have an invalid mAssetFilename
		mAssetFilename = new MOG_Filename();

		// Indicate the AssetClassificationProperties Database because there was no filename specified
		mDBClassification = true;
	}

	// Remember our specified classification
	mClassification = classification;

	// Check if there was a valid scopeName specified?
	if (scopeName->Length)
	{
		mScopeName = scopeName;
	}

	// Check if there was a specific properties file specified?
	if (propertiesFile)
	{
		// Use this specified properties file as our properties
		mProperties = propertiesFile;

		// Push the non-inherited properties
		PushProperties(propertiesFile->GetPropertyList());
	}
	else
	{
		// Clear all the member variables
		mProperties = new MOG_PropertiesIni();
	}

	// Clear all our inherited properties
	mClassificationPropertiesHierarchy = NULL;
	mInheritedProperties = new MOG_PropertiesIni();
}


bool MOG_Properties::PopulateInheritance()
{
	return PopulateInheritance(mClassification);
}


bool MOG_Properties::PopulateInheritance(String *classification)
{
	// Initialize the ClassificationPropertiesHierarchy
	if (classification->Length)
	{
		// Check if we are a classification?
		if (mDBClassification)
		{
			// Check if the specified classification is including us?
			if (classification->Length == mClassification->Length)
			{
				// Make sure we don't inherit our own properties...Strip us off the specified classification
				classification = MOG_Filename::GetParentsClassificationString(classification);
			}
		}

		// Check if we are are using our cache?  and
		// Check if this classification is the same as gLastClassification?
		if (gUsePropertiesCache &&
			String::Compare(classification, gLastClassification, true) == 0)
		{
			// Reuse our last classification's properties
			mInheritedProperties = gLastInheritedProperties;
		}
		else
		{
			// Create an empty inheritance properties file
			mInheritedProperties = new MOG_PropertiesIni();
			// Get all the inherited property values of the specified classification
			mClassificationPropertiesHierarchy = MOG_DBAssetAPI::GetAllDerivedAssetClassificationProperties(classification, S"");
			if (mClassificationPropertiesHierarchy != NULL)
			{
				// Walk through all of our inherited classification's properties
				for (int c = 0; c < mClassificationPropertiesHierarchy->Count; c++)
				{
					MOG_ClassificationProperties *childClassification = __try_cast<MOG_ClassificationProperties *>(mClassificationPropertiesHierarchy->Item[c]);

					// Push this classification's properties into our mInheritedProperties file
					PushProperties(mInheritedProperties, childClassification->mProperties);
				}
			}

			// Save this classification for the next round
			gLastClassification = classification;
			gLastInheritedProperties = mInheritedProperties;
		}

		return true;
	}

	return false;
}

bool MOG_Properties::PushProperties(ArrayList *properties)
{
	// Push in a NULL indicating there is no list needing to be updated yet we still want to clear any potential relationship inheritance
	return PushProperties(NULL, properties);
}

bool MOG_Properties::PushProperties(MOG_PropertiesIni *propertiesIniFile, ArrayList *properties)
{
	// Check if we obtained our properties?
	if (properties)
	{
		// Check if a propertiesIniFile was specified?
		if (propertiesIniFile)
		{
			// Handle complex relationship inheritance schema to sever inheritance of relationships being overridden
			// Scan properties for any relationships and remove them because they are getting overridden
			for (int i = 0; i < properties->Count; i++)
			{
				MOG_Property *property = __try_cast<MOG_Property *>(properties->Item[i]);

				// Is this property a relationship?
				if (String::Compare(property->mSection, PROPTEXT_Relationships, true) == 0)
				{
					// Get the previously set associated relationships because they getting overridden
					ArrayList *existingRelationships = propertiesIniFile->GetPropertyList(property->mSection, property->mPropertySection);
					for (int r = 0; r < existingRelationships->Count; r++)
					{
						MOG_Property *newRelationship = __try_cast<MOG_Property*>(existingRelationships->Item[r]);
						// Simply remove all like relationships becuase we are about to override them
						propertiesIniFile->RemovePropertySection(newRelationship->mSection, newRelationship->mPropertySection);
					}
				}
			}

			// Stuff these properties into our mProperties
			for (int i = 0; i < properties->Count; i++)
			{
				MOG_Property *property = __try_cast<MOG_Property *>(properties->Item[i]);

				// Proceed to push the new property
				propertiesIniFile->PutProperty(property);
			}
		}

		return true;
	}

	return false;
}

String *MOG_Properties::GetInheritedPropertyClassification(MOG_Property *inheritedProperty)
{
	String *inheritedPropertyClassification = "";

	// Initialize the ClassificationPropertiesHierarchy
	if (inheritedProperty)
	{
		// Make sure our mClassificationPropertiesHierarchy is valid
		if (mClassificationPropertiesHierarchy != NULL)
		{
			// Walk through all of our inherited classification's properties
			for (int c = 0; c < mClassificationPropertiesHierarchy->Count; c++)
			{
				MOG_ClassificationProperties *childClassification = __try_cast<MOG_ClassificationProperties *>(mClassificationPropertiesHierarchy->Item[c]);

				// Check for a matching property?
				for (int i = 0; i < childClassification->mProperties->Count; i++)
				{
					MOG_Property *thisProperty = __try_cast<MOG_Property *>(childClassification->mProperties->Item[i]);

					// Check if this property matches?
					if (String::Compare(thisProperty->mSection, inheritedProperty->mSection, true) == 0 &&
						String::Compare(thisProperty->mKey, inheritedProperty->mKey, true) == 0 &&
						String::Compare(thisProperty->mValue, inheritedProperty->mValue, true) == 0)
					{
						// return the classification associated with this inherited property
						return childClassification->mClassification;
					}
				}
			}
		}
	}

	return inheritedPropertyClassification;
}


bool MOG_Properties::DoesPropertyExist(MOG_Property *property)
{
	return IsPropertyAlreadyInherited(property->mSection, property->mPropertySection, property->mPropertyKey, property->mPropertyValue);
}


bool MOG_Properties::DoesPropertyExist(String *section, String *propertySection, String *propertyName)
{
	// Check if we have any inheritence information?
	if (mInheritedProperties)
	{
		MOG_Property *inheritedProperty = NULL;

		// Check if this property already exists in our inherited properties?
		if (mInheritedProperties->PropertyExist(section, propertySection, propertyName))
		{
			return true;
		}
	}

	// Make sure we have a valid mProperties?
	if (mProperties)
	{
		// Check if this property already exists?
		if (mProperties->PropertyExist(section, propertySection, propertyName))
		{
			return true;
		}
	}

	return false;
}


bool MOG_Properties::IsPropertyAlreadyInherited(MOG_Property *property)
{
	return IsPropertyAlreadyInherited(property->mSection, property->mPropertySection, property->mPropertyKey, property->mPropertyValue);
}


bool MOG_Properties::IsPropertyAlreadyInherited(String *section, String *propertySection, String *propertyName, String *propertyValue)
{
	bool bAlreadyInherited = false;

	// Only reference if it exists
	if (mInheritedProperties)
	{
		MOG_Property *inheritedProperty = NULL;

		// Check if this property already exists in our inherited properties?
		if (mInheritedProperties->PropertyExist(section, propertySection, propertyName))
		{
			inheritedProperty = mInheritedProperties->GetProperty(section, propertySection, propertyName);
		}
		else
		{
			// Build a tempProperty for easier parsing of the property's scope
			MOG_Property *tempProperty = new MOG_Property(section, propertySection, propertyName, propertyValue);
			// Check if this tempProperty contains a scope?
			if (tempProperty->GetPropertyScope() &&
				tempProperty->GetPropertyScope()->Length)
			{
				// Check if this property will inherit from a scopeless non-inherited property?
				if (mProperties->PropertyExist(section, tempProperty->GetScopelessPropertySection(), propertyName))
				{
					inheritedProperty = mProperties->GetProperty(section, tempProperty->GetScopelessPropertySection(), propertyName);
				}
				// Check if this property will inherit from a scopeless inherited property?
				else if (mInheritedProperties->PropertyExist(section, tempProperty->GetScopelessPropertySection(), propertyName))
				{
					inheritedProperty = mInheritedProperties->GetProperty(section, tempProperty->GetScopelessPropertySection(), propertyName);
				}
			}
		}

		// Check if we actually found a match?
		if (inheritedProperty)
		{
			// Handle special tokenized and formulatic strings
			String* newResolvedValue = "";
			String* inheritedResolvedValue = "";

			// Build the seeds needed for resolving value
			String *seeds = "";
			// Check if this is a classification only?
			if (mClassification->Length)
			{
				// Build seeds for mClassification and Properties
				seeds = MOG_Tokens::AppendTokenSeeds(seeds, MOG_Tokens::GetClassificationTokenSeeds(mClassification));
				seeds = MOG_Tokens::AppendTokenSeeds(seeds, MOG_Tokens::GetPropertyTokenSeeds(GetPropertyList()));
			}
			else
			{
				// Build seeds for AssetFilename and Properties
				seeds = MOG_Tokens::AppendTokenSeeds(seeds, MOG_Tokens::GetFilenameTokenSeeds(GetAssetFilename()));
				seeds = MOG_Tokens::AppendTokenSeeds(seeds, MOG_Tokens::GetPropertyTokenSeeds(GetPropertyList()));
			}

			// Resolve both the new and inherited values
			newResolvedValue = MOG_Tokens::GetFormattedString(propertyValue, seeds);
			inheritedResolvedValue = MOG_Tokens::GetFormattedString(inheritedProperty->mValue, seeds);
			
			// Check if the newResolvedValue is already implied in the existingResolvedValue?
			if (String::Compare(newResolvedValue, inheritedResolvedValue, true) == 0)
			{
				// Indicate that this property is already inherited
				bAlreadyInherited = true;

				// Special check for relationship properties...
				// If the asset ever overrides a relationship property we want to terminate all inheritance for that relationship
				// Check if this property is contained within the relationships section?
				if (String::Compare(section, PROPTEXT_Relationships, true) == 0)
				{
					// Make sure we haven't already cleared this relationship
					if (mProperties->GetPropertyList(section, propertySection)->Count > 0)
					{
						// This propeerty has overridden this relationship so we can completly ignore any inheritance
						bAlreadyInherited = false;
					}
				}
			}
		}
	}

	// Indicate this property ins't already inherited
	return bAlreadyInherited;
}

bool MOG_Properties::IsPropertyNotInherited(MOG_Property *property)
{
	return IsPropertyNotInherited(property->mSection, property->mPropertySection, property->mPropertyKey, property->mPropertyValue);
}


bool MOG_Properties::IsPropertyNotInherited(String *section, String *propertySection, String *propertyName, String *propertyValue)
{
	// Only reference if it exists
	if (mProperties)
	{
		if (mProperties->PropertyExist(section, propertySection, propertyName))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	// Indicate this property isn't defined here, so it must be inherited
	return false;
}


bool MOG_Properties::RemoveAlreadyInheritedProperties()
{
	// Get all of our non inherited properties
	ArrayList *properties = GetNonInheritedProperties();
	if (properties)
	{
		// Stuff these properties into our mProperties
		for (int i = 0; i < properties->Count; i++)
		{
			MOG_Property *property = __try_cast<MOG_Property *>(properties->Item[i]);

			// Check if this property is already inherited?
			if (IsPropertyAlreadyInherited(property))
			{
				// Remove this property from us because this new one won't alter our derived state
				RemoveProperty(property->mSection, property->mPropertySection, property->mPropertyKey);
			}
		}
	}

	return true;
}


MOG_Properties *MOG_Properties::OpenAssetProperties(MOG_Filename* assetFilename)
{
	MOG_Properties *newProperties = NULL;
	bool bFailed = false;
	bool bWait = false;

	// Make sure this is a valid asset?
	if (assetFilename)
	{
		String *propertiesFilename = MOG_ControllerAsset::GetAssetPropertiesFilename(assetFilename);

		while (!bFailed)
		{
			// Check if we need to wait?
			if (bWait)
			{
				// We need to wait for our turn because we found another thread already using this asset
				// Goto sleep and wait for a bit before we try again
				Thread::Sleep(100);
			}

			// Ensure exclusivity
			Monitor::Enter(S"OpeningProperties");
			__try
			{
	//			// Check if we are using our propertied cache?
	//			if (gUsePropertiesCache)
	//			{
	//				// Check if this asset is already in our cache?
	//				if (gPropertiesCache->Contains(propertiesFilename))
	//				{
	//					MOG_PropertiesCacheObject *cacheObject = dynamic_cast<MOG_PropertiesCacheObject *>(gPropertiesCache->Item[propertiesFilename]);
	//
	////					// Now check to ensure we are in the same thread?
	////					if (cacheObject->mThread == Thread::CurrentThread->CurrentThread)
	////					{
	//						// Increment our reference count
	//						cacheObject->mReferenceCount++;

	//						// Return our cached object
	//						return cacheObject->mProperties;
	////					}
	////					else
	////					{
	////						// Indicate we need to wait
	////						bWait = true;
	////						continue;
	////					}
	//				}
	//			}

				// Load the actual asset's PropertiesFile
				MOG_PropertiesIni *propertiesFile = new MOG_PropertiesIni();
				if (propertiesFile->Open(propertiesFilename, FileShare::Read))
				{
					newProperties = new MOG_Properties();

					// Create our own MOG_Filename so nobody will mess with ours
					MOG_Filename *newFilename = new MOG_Filename(assetFilename);
					// Initialize our properties class
					newProperties->Initialize(newFilename, assetFilename->GetAssetPlatform(), assetFilename->GetAssetClassification(), propertiesFile);
					newProperties->PopulateInheritance(newProperties->mClassification);

					// Indicate that this Property was opened and can be modified
					newProperties->mCanModify = true;

//					// Add the newly opened asset to our cache
//					MOG_PropertiesCacheObject *cacheObject = new MOG_PropertiesCacheObject();
//					cacheObject->mThread = Thread::CurrentThread;
//					cacheObject->mProperties = newProperties;
//					cacheObject->mReferenceCount = 1;
//					gPropertiesCache->Item[propertiesFilename] = cacheObject;
				}
				else
				{
					bFailed = true;
				}

				// Stop looping
				break;
			}
			__finally
			{
				Monitor::Exit(S"OpeningProperties");
			}
		}
	}

	return newProperties;
}


MOG_Properties *MOG_Properties::OpenClassificationProperties(String *classification)
{
	// Make sure this is a valid classification?
	if (classification && classification->Length)
	{
		// Create our new properties?
		MOG_Properties *newProperties = new MOG_Properties();
		if (newProperties)
		{
			// Initialize our properties class
			newProperties->Initialize(NULL, S"", classification, NULL);
			newProperties->PopulateInheritance(newProperties->mClassification);

			// Get the properties for this classification
			ArrayList *properties = MOG_DBAssetAPI::GetAllAssetClassificationProperties(classification, S"");
			// Push the non-inherited properties
			newProperties->PushProperties(newProperties->mProperties, properties);

			// Indicate that this Property was opened and can be modified
			newProperties->mCanModify = true;

			// Indicate that this property is working in the immeadiate mode
			// NOTE: We assume classifications are only opened by a user initiated action so we can operate in the slower immediate mode
//			newProperties->mImmeadiateMode = true;

			return newProperties;
		}
	}

	return NULL;
}


MOG_Properties *MOG_Properties::OpenFileProperties(String *propertiesFilename)
{
	MOG_PropertiesIni *iniFile = new MOG_PropertiesIni();
	if (iniFile->Open(propertiesFilename, FileShare::Read))
	{
		return OpenFileProperties(iniFile);
	}

	return NULL;
}


MOG_Properties *MOG_Properties::OpenFileProperties(MOG_PropertiesIni *propertiesFile)
{
	// Make sure this is a valid asset?
	if (propertiesFile)
	{
		// Create our new properties?
		MOG_Properties *newProperties = new MOG_Properties();
		if (newProperties)
		{
			MOG_Filename *iniFilename = new MOG_Filename(propertiesFile->GetFilename());

			// Always make sure we obtain the asset's location rather than the properties file
			MOG_Filename *assetFilename = new MOG_Filename(iniFilename->GetAssetEncodedPath());

			// Initialize our properties class
			newProperties->Initialize(assetFilename, assetFilename->GetAssetPlatform(), assetFilename->GetAssetClassification(), propertiesFile);
			newProperties->PopulateInheritance(newProperties->mClassification);

			// Indicate that this Property was opened and can be modified
			newProperties->mCanModify = true;

			return newProperties;
		}
	}

	return NULL;
}


bool MOG_Properties::Close(bool bSave)
{
	bool bFailed = false;

	// Ensure exclusivity
	Monitor::Enter(S"ClosingProperties");

	__try
	{
		// Check if this asset is already in our cache?
		String *propertiesFilename = mProperties->GetFilename();
		if (gPropertiesCache->Contains(propertiesFilename))
		{
			MOG_PropertiesCacheObject *cacheObject = dynamic_cast<MOG_PropertiesCacheObject *>(gPropertiesCache->Item[propertiesFilename]);

//			// Now check to ensure we are in the same thread?
//			if (cacheObject->mThread == Thread::CurrentThread)
//			{
				// Decrement our reference count
				cacheObject->mReferenceCount--;

				// Check if we still have references?
				if (cacheObject->mReferenceCount)
				{
					return true;
				}
//			}
//			else
//			{
//				// This means big trouble!!!
//			}
		}

		// Check if we need to save on close?
		if (bSave)
		{
			// Save all of our properties
			if (!Save())
			{
				bFailed = true;
			}

			// Check if we contain a Properties file?
			if (mProperties)
			{
				// Make sure we close our handle
				mProperties->Close();
			}
		}
		else
		{
			// Clear the modified flag
			mModified = false;
			// Check if we contain a Properties file?
			if (mProperties)
			{
				// Inform the Properties file not to save
				mProperties->CloseNoSave();
			}
		}

		// Clean up and destroy all our more significant member variables
		mSection = NULL;
		mAssetFilename = NULL;
		mClassification = NULL;
		mScopeName = NULL;
		mProperties = NULL;
		mInheritedProperties = NULL;
		mClassificationPropertiesHierarchy = NULL;

		// Now remove this asset from our cache
		gPropertiesCache->Remove(propertiesFilename);
	}
	__finally
	{
		Monitor::Exit(S"ClosingProperties");
	}

	if (!bFailed)
	{
		return true;
	}
	return false;
}


bool MOG_Properties::UpdateAssetFilename(MOG_Filename *newAssetFilename, String *newPropertiesFilename)
{
	// Check if we have been tracking the filename before we just assume it needs to be updated?
	if (mAssetFilename != NULL)
	{
		mAssetFilename = newAssetFilename;

		// Determin where this asset is located?
		// Check if this asset is within the MOG Repository?
		if (mAssetFilename->IsWithinRepository())
		{
			// Indicate the AssetProperties Database
			mDBRepository = true;
			mDBInbox = false;
		}
		// Check if this asset is within the inboxes?
		if (mAssetFilename->IsWithinInboxes() || mAssetFilename->IsLocal())
		{
			// Indicate the Inbox Database
			mDBRepository = false;
			mDBInbox = true;
		}
	}

	// Update the propeertiesFilename
	if (mProperties->GetFilename()->Length)
	{
		mProperties->SetFilename(newPropertiesFilename, false);
	}

	return true;
}


bool MOG_Properties::Save()
{
	bool bFailed = false;

	// Only bother to save the items if were not already operating in the immeadiate mode?
	if (!mImmeadiateMode)
	{
		// Check if we ever actually modified the properties?
		if (mProperties && (mModified || mProperties->HasChanged()))
		{
			// Save the actual properties file
			if (mProperties->GetFilename()->Length)
			{
				if (!mProperties->Save())
				{
					bFailed = true;
				}
			}


			// Check if we are a classification?
			if (mDBClassification)
			{
				if( MOG_DBAssetAPI::RemoveAllAssetClassificationProperties( mClassification, MOG_ControllerProject::GetBranchName() ))
				{
					MOG_DBAssetAPI::AddAssetClassificationProperties( mClassification, MOG_ControllerProject::GetBranchName(), GetNonInheritedProperties());
				}

// JohnRen - We may need to do this for classifications someday if they ever stop being immediate.
//	MOG_DBAssetAPI::RemoveAssetClassificationProperties() - Missing
//	MOG_DBAssetAPI::AddAssetClassificationProperties() - Missing
//				// Add property to the inbox properties database
//				if (!MOG_DBAssetAPI::RemoveAssetClassificationProperties(mClassification))
//				{
//					if (!MOG_DBAssetAPI::AddAssetClassificationProperties(mClassification, GetNonInheritedProperties()))
//					{
//						bFailed = true;
//					}
//				}
//				else
//				{
//					bFailed = true;
//				}
			}

			// Check if this property's file exists within the inboxes?
			if (mDBInbox)
			{
				// Add all the properties to the inbox properties database
				if (!MOG_DBInboxAPI::UpdateInboxAssetProperties(mAssetFilename, GetPropertyList()))
				{
					bFailed = true;
				}
			}

			// Clear our modified flag
			mModified = false;
		}
	}

	// Check if we finished without any errors?
	if (!bFailed)
	{
		return true;
	}

	return false;
}

Boolean MOG_Properties::ParseBool(String *boolText)
{
	return ParseBool(boolText, false);
}

Boolean MOG_Properties::ParseBool(String *boolText, bool bDefaultValue)
{
	// parse boolText, avoiding the exceptions that Boolean.Parse() throws if boolText is invalid
	if (boolText &&
		boolText->Length)
	{
		if (boolText->ToLower()->StartsWith(S"True"->ToLower()))
		{
			return true;
		}
		if (boolText->ToLower()->StartsWith(S"False"->ToLower()))
		{
			return false;
		}
	}

	return bDefaultValue;
}


MOG_InheritedBoolean MOG_Properties::ParseInheritedBoolean(String *boolText)
{
	return ParseInheritedBoolean(boolText, false);
}

MOG_InheritedBoolean MOG_Properties::ParseInheritedBoolean(String *boolText, bool bDefaultValue)
{
	// Default us to our 'False' state
	MOG_InheritedBoolean InheritedBoolean = MOG_InheritedBoolean::False;
	if (bDefaultValue == true)
	{
		InheritedBoolean = MOG_InheritedBoolean::True;
	}

	// Check if mScopeExplicitMode is set?
	if (mScopeExplicitMode)
	{
		// Default us to the inherited state in case we fail to find an explicit property below
		InheritedBoolean = MOG_InheritedBoolean::Inherited;
	}

	if (boolText &&
		boolText->Length)
	{
		// Parse the value into a real bool
		bool bValue = ParseBool(boolText, bDefaultValue);
		if (bValue == false)
		{
			InheritedBoolean = MOG_InheritedBoolean::False;
		}
		if (bValue == true)
		{
			InheritedBoolean = MOG_InheritedBoolean::True;
		}
	}

	return InheritedBoolean;
}

MOG_DefaultPrompt MOG_Properties::ParseDefaultPrompt(String *text)
{
	// Default us to never prompt the user
	MOG_DefaultPrompt scope = MOG_DefaultPrompt::No;
	// Check if mScopeExplicitMode is set?
	if (mScopeExplicitMode)
	{
		// Default us to the inherited state in case we fail to find an explicit property below
		scope = MOG_DefaultPrompt::Inherited;
	}

	// Map the text to the correct enum
	if (text &&
		text->Length)
	{
		if (text->StartsWith(S"Yes", StringComparison::CurrentCultureIgnoreCase))
		{
			scope = MOG_DefaultPrompt::Yes;
		}
		else if (text->StartsWith(S"No", StringComparison::CurrentCultureIgnoreCase))
		{
			scope = MOG_DefaultPrompt::No;
		}
		else if (text->StartsWith(S"PromptDefaultNo", StringComparison::CurrentCultureIgnoreCase))
		{
			scope = MOG_DefaultPrompt::PromptDefaultNo;
		}
		else if (text->StartsWith(S"PromptDefaultYes", StringComparison::CurrentCultureIgnoreCase))
		{
			scope = MOG_DefaultPrompt::PromptDefaultYes;
		}
	}

	return scope;
}

MOG_PackageCommandPropagation MOG_Properties::ParsePackageCommandPropagation(String *text)
{
	// Default us to our 'None' state
	MOG_PackageCommandPropagation scope = MOG_PackageCommandPropagation::PerRecursiveFile;
	// Check if mScopeExplicitMode is set?
	if (mScopeExplicitMode)
	{
		// Default us to the inherited state in case we fail to find an explicit property below
		scope = MOG_PackageCommandPropagation::Inherited;
	}

	// Map the text to the correct enum
	if (text &&
		text->Length)
	{
		if (text->StartsWith(S"PerAsset", StringComparison::CurrentCultureIgnoreCase))
		{
			scope = MOG_PackageCommandPropagation::PerAsset;
		}
		else if (text->StartsWith(S"PerRootFile", StringComparison::CurrentCultureIgnoreCase))
		{
			scope = MOG_PackageCommandPropagation::PerRootFile;
		}
		else if (text->StartsWith(S"PerRecursiveFile", StringComparison::CurrentCultureIgnoreCase))
		{
			scope = MOG_PackageCommandPropagation::PerRecursiveFile;
		}
	}

	return scope;
}

MOG_PackageStyle MOG_Properties::ParsePackageStyle(String *styleText)
{
	// Default us to our 'None' state
	MOG_PackageStyle packageStyle = MOG_PackageStyle::Disabled;
	// Check if mScopeExplicitMode is set?
	if (mScopeExplicitMode)
	{
		// Default us to the inherited state in case we fail to find an explicit property below
		packageStyle = MOG_PackageStyle::Inherited;
	}

	// Map the text to the correct enum
	if (styleText &&
		styleText->Length)
	{
		if (styleText->StartsWith(S"Disabled", StringComparison::CurrentCultureIgnoreCase))
		{
			packageStyle = MOG_PackageStyle::Disabled;
		}
		else if (styleText->StartsWith(S"Simple", StringComparison::CurrentCultureIgnoreCase))
		{
			packageStyle = MOG_PackageStyle::Simple;
		}
		else if (styleText->StartsWith(S"TaskFile", StringComparison::CurrentCultureIgnoreCase))
		{
			packageStyle = MOG_PackageStyle::TaskFile;
		}
	}

	return packageStyle;
}

String *MOG_Properties::MapInheritedBoolean(MOG_InheritedBoolean value)
{
	String *propertyValue = S"";

	switch (value)
	{
		case MOG_InheritedBoolean::False:
			propertyValue = S"False";
			break;
		case MOG_InheritedBoolean::True:
			propertyValue = S"True";
			break;
		case MOG_InheritedBoolean::Inherited:
			propertyValue = S"Inherited";
			break;
	}

	return propertyValue;
}

String *MOG_Properties::MapDefaultPrompt(MOG_DefaultPrompt value)
{
	String *propertyValue = S"";

	// Check for each of the possible styles
	if (value == MOG_DefaultPrompt::Yes)
	{
		propertyValue = S"Yes";
	}
	if (value == MOG_DefaultPrompt::No)
	{
		propertyValue = S"No";
	}
	if (value == MOG_DefaultPrompt::PromptDefaultNo)
	{
		propertyValue = S"PromptDefaultNo";
	}
	if (value == MOG_DefaultPrompt::PromptDefaultYes)
	{
		propertyValue = S"PromptDefaultYes";
	}

	return propertyValue;
}

String *MOG_Properties::MapPackageCommandPropagation(MOG_PackageCommandPropagation value)
{
	String *propertyValue = S"";

	// Check for each of the possible styles
	if (value == MOG_PackageCommandPropagation::PerAsset)
	{
		propertyValue = S"PerAsset";
	}
	if (value == MOG_PackageCommandPropagation::PerRootFile)
	{
		propertyValue = S"PerRootFile";
	}
	if (value == MOG_PackageCommandPropagation::PerRecursiveFile)
	{
		propertyValue = S"PerRecursiveFile";
	}

	return propertyValue;
}

String *MOG_Properties::MapPackageStyle(MOG_PackageStyle value)
{
	String *propertyValue = S"";

	// Check for each of the possible styles
	if (value == MOG_PackageStyle::Disabled)
	{
		propertyValue = S"Disabled";
	}
	if (value == MOG_PackageStyle::Simple)
	{
		propertyValue = S"Simple";
	}
	if (value == MOG_PackageStyle::TaskFile)
	{
		propertyValue = S"TaskFile";
	}

	return propertyValue;
}


ArrayList *MOG_Properties::GetNonInheritedProperties()
{
	return mProperties->GetPropertyList();
};


MOG_PropertiesIni *MOG_Properties::GetCombinedPropertiesFile(bool includeNonInherited, bool includeInherited)
{
	MOG_PropertiesIni *tempProperties = new MOG_PropertiesIni();
	if (includeInherited && mInheritedProperties)
	{
		// Get the inherited properties
		tempProperties->PutFile(mInheritedProperties);

		// Handle complex relationship inheritance schema to sever inheritance of relationships being overridden
		// Scan tempProperties and remove relationships being overridden
		ArrayList *nonInheritedRelationships = GetPropertyList(PROPTEXT_Relationships, true, false);
		for (int r = 0; r < nonInheritedRelationships->Count; r++)
		{
			MOG_Property *nonInheritedRelationship = __try_cast<MOG_Property*>(nonInheritedRelationships->Item[r]);
			// Simply remove all like relationships just added from the inherited list
			tempProperties->RemovePropertySection(nonInheritedRelationship->mSection, nonInheritedRelationship->mPropertySection);
		}
	}
	
	if (includeNonInherited)
	{
		// Get the non-inherited properties
		tempProperties->PutFile(mProperties);
	}

	return tempProperties;
}


ArrayList *MOG_Properties::GetPropertyList()
{
	return GetPropertyList(true, true);
}

ArrayList *MOG_Properties::GetPropertyList(bool includeNonInherited, bool includeInherited)
{
	MOG_PropertiesIni *tempProperties = GetCombinedPropertiesFile(includeNonInherited, includeInherited);

	return tempProperties->GetPropertyList();
};


ArrayList *MOG_Properties::GetPropertyList(String *section)
{
	return GetPropertyList(section, true, true);
}

ArrayList *MOG_Properties::GetPropertyList(String *section, bool includeNonInherited, bool includeInherited)
{
	MOG_PropertiesIni *tempProperties = GetCombinedPropertiesFile(includeNonInherited, includeInherited);

	return tempProperties->GetPropertyList(section);
};


ArrayList *MOG_Properties::GetPropertyList(String *section, String *propertySection)
{
	return GetPropertyList(section, propertySection, true, true);
}

ArrayList *MOG_Properties::GetPropertyList(String *section, String *propertySection, bool includeNonInherited, bool includeInherited)
{
	MOG_PropertiesIni *tempProperties = GetCombinedPropertiesFile(includeNonInherited, includeInherited);

	// Get all the properties in the specified section
	ArrayList *verifiedProperties = new ArrayList();
	ArrayList *allProperties = tempProperties->GetPropertyList(section);
	if (allProperties)
	{
		// Verify each property to make sure it matches the property section
		for (int i = 0; i < allProperties->Count; i++)
		{
			MOG_Property *property = __try_cast<MOG_Property *>(allProperties->Item[i]);

			// Check if either the full PropertySection or PropertySectionScopeless matches?
			if (String::Compare(property->mPropertySection, propertySection, true) == 0 ||
				String::Compare(property->mPropertySectionScopeless, propertySection, true) == 0)
			{
				verifiedProperties->Add(property);
			}
		}
	}

	return verifiedProperties;
}


ArrayList *MOG_Properties::GetPropertyList(String *section, String *propertySection, String *scopeName)
{
	String *scopedPropertySection = String::Concat(propertySection, S".", scopeName);
	return GetPropertyList(section, scopedPropertySection);
}


ArrayList *MOG_Properties::GetApplicableProperties()
{
	return GetApplicableProperties(S"", S"");
}


ArrayList *MOG_Properties::GetApplicableProperties(String *section)
{
	return GetApplicableProperties(section, S"");
}


ArrayList *MOG_Properties::GetApplicableProperties(String *section, String *propertySection)
{
	ArrayList *applicableProperties = new ArrayList();

	// Scan allProperties and isolate the applicable ones for this scope
	ArrayList *allProperties = GetPropertyList();
	if (allProperties)
	{
		for (int p = 0; p < allProperties->Count; p++)
		{
			// Get this property
			MOG_Property *property = __try_cast<MOG_Property *>(allProperties->Item[p]);

			// Check if there was a specific section specified?
			if (section && section->Length)
			{
				// Check if this property's section matches?
				if (String::Compare(section, property->mSection, true) == 0)
				{
				}
				else
				{
					// Skip this property
					continue;
				}
			}

			// Check if there was a specific property section specified?
			if (propertySection && propertySection->Length)
			{
				// Check if this property matches either the full property key or it's scopeless version?
				if (String::Compare(propertySection, property->mPropertySection, true) == 0 ||
					String::Compare(propertySection, property->mPropertySectionScopeless, true) == 0)
				{
				}
				else
				{
					// Skip this property
					continue;
				}
			}

			// Use GetProperty() to resolve properties with scope because we only want the scopeless versions
			MOG_Property *scopedProperty = GetProperty(property->mSection, property->mPropertySectionScopeless, property->mPropertyKey);
			if (scopedProperty)
			{
				// Check if this already exists in our applicableProperties?
				bool bFound = false;
				for (int f = 0; f < applicableProperties->Count; f++)
				{
					// Get this property
					MOG_Property *applicableProperty = __try_cast<MOG_Property *>(applicableProperties->Item[f]);

					// Check if this applicableProperty matches the scopedProperty?
					if (String::Compare(applicableProperty->mSection, scopedProperty->mSection, true) == 0 &&
						String::Compare(applicableProperty->mPropertySectionScopeless, scopedProperty->mPropertySectionScopeless, true) == 0 &&
						String::Compare(applicableProperty->mPropertyKey, scopedProperty->mPropertyKey, true) == 0)
					{
						// Indicate we found a match and exit loop
						bFound = true;
						break;
					}
				}

				// Check if we never found a match?
				if (!bFound)
				{
					// Add this property to our applicableProperties
					applicableProperties->Add(scopedProperty);
				}
			}
		}
	}

	// Return our list of applicable properties
	return applicableProperties;
}



MOG_Property *MOG_Properties::GetInheritedPropertyFromClassificationsArray(String *section, String *propertySection, String *propertyName)
{
	// Make sure we have some inheritences?
	if (mClassificationPropertiesHierarchy->Count)
	{
		// Walk through all of our inherited classification's properties (backwards)
		for (int c = mClassificationPropertiesHierarchy->Count - 1; c >= 0; c--)
		{
			MOG_ClassificationProperties *childClassification = __try_cast<MOG_ClassificationProperties *>(mClassificationPropertiesHierarchy->Item[c]);

			// Scan this child classification for this property
			for (int i = 0; i < childClassification->mProperties->Count; i++)
			{
				MOG_Property *property = __try_cast<MOG_Property *>(childClassification->mProperties->Item[i]);
				// Compare this property?
				if (String::Compare(property->mSection, section, true) == 0 &&
					String::Compare(property->mPropertySection, propertySection) == 0 &&
					String::Compare(property->mPropertyKey, propertyName, true) == 0)
				{
					return property;
				}
			}
		}
	}

	return NULL;
}


MOG_Property *MOG_Properties::GetProperty(String *section, String *propertySection, String *propertyName)
{
	// Makse sure we have a valid mProperties?
	if (mProperties)
	{
		// If we are scope specific, append the scope to the property section
		if (mScopeName && mScopeName->Length && String::Compare(mScopeName, "All", true) != 0)
		{
			String *scopedPropertySection = String::Concat(propertySection, S".", mScopeName);
			if (mProperties->PropertyExist(section, scopedPropertySection, propertyName))
			{
				String *value = mProperties->GetPropertyString(section, scopedPropertySection, propertyName);
				MOG_Property *property = new MOG_Property(section, scopedPropertySection, propertyName, value);
				return property;
			}
		
			// Only reference if it exists
			if (mInheritedProperties)
			{
				// Check if this exists in our classifications inheritance properties?
				if (mInheritedProperties->PropertyExist(section, scopedPropertySection, propertyName))
				{
					String *value = mInheritedProperties->GetPropertyString(section, scopedPropertySection, propertyName);
					MOG_Property *property = new MOG_Property(section, scopedPropertySection, propertyName, value);
					return property;
				}
			}

			// Check if mScopeExplicitMode is set?
			if (mScopeExplicitMode)
			{
				// Fail becuase we are explicit
				return NULL;
			}
		}

		// Check for the scope inspecific version
		if (mProperties->PropertyExist(section, propertySection, propertyName))
		{
			String *value = mProperties->GetPropertyString(section, propertySection, propertyName);
			MOG_Property *property = new MOG_Property(section, propertySection, propertyName, value);
			return property;
		}

		// Only reference if it exists
		if (mInheritedProperties)
		{
			// Check if this exists in our classifications inheritance properties?
			if (mInheritedProperties->PropertyExist(section, propertySection, propertyName))
			{
				String *value = mInheritedProperties->GetPropertyString(section, propertySection, propertyName);
				MOG_Property *property = new MOG_Property(section, propertySection, propertyName, value);
				return property;
			}
		}
	}

	return NULL;
}


String *MOG_Properties::GetPropertyString(String *section, String *propertySection, String *propertyName)
{
	// Makse sure we have a valid mProperties?
	if (mProperties)
	{
		// If we are scope specific, append the scope to the property section
		if (mScopeName->Length && String::Compare(mScopeName, "All", true) != 0)
		{
			String *scopedPropertySection = String::Concat(propertySection, S".", mScopeName);
			if (mProperties->PropertyExist(section, scopedPropertySection, propertyName))
			{
				return mProperties->GetPropertyString(section, scopedPropertySection, propertyName);
			}

			// Only reference if it exists
			if (mInheritedProperties)
			{
				// Check if this exists in our classifications inheritance properties?
				if (mInheritedProperties->PropertyExist(section, scopedPropertySection, propertyName))
				{
					return mInheritedProperties->GetPropertyString(section, scopedPropertySection, propertyName);
				}
			}

			// Check if mScopeExplicitMode is set?
			if (mScopeExplicitMode)
			{
				// Fail becuase we are explicit
				return NULL;
			}
		}

		// Return the scope inspecific version
		if (mProperties->PropertyExist(section, propertySection, propertyName))
		{
			return mProperties->GetPropertyString(section, propertySection, propertyName);
		}

		// Only reference if it exists
		if (mInheritedProperties)
		{
			// Check if this exists in our classifications inheritance properties?
			if (mInheritedProperties->PropertyExist(section, propertySection, propertyName))
			{
				return mInheritedProperties->GetPropertyString(section, propertySection, propertyName);
			}
		}
	}

	return "";
}


bool MOG_Properties::SetProperty(String *section, String *propertySection, String *propertyName, String *propertyValue)
{
	//Make sure the property has a valid section and name before changing anything
	if (propertySection->Length != 0 && propertyName->Length != 0)
	{
		// Check if we can modify this instance?
		if (mCanModify)
		{
			// Check if mScopeExplicitMode is set?
			if (mScopeExplicitMode)
			{
				// Make sure this property doesn't already contain a scope?
				if (propertySection->IndexOf(S".") == -1)
				{
					// Ensure property is scoped
					propertySection = String::Concat(propertySection, S".", mScopeName);
				}
			}

			// Check if this property is being set to nothing?
			if (propertyValue->Length == 0)
			{
				// Check if this property has an 'Inherited' value?
				// We have to check this or else it could prevent properties like relationships from being imported correctly (i.e. demo projects).
				if (mInheritedProperties->PropertyExist(section, propertySection, propertyName))
				{
					// Indicate that we actually want this property to inherit
					propertyValue = S"Inherit";
				}
			}
			if (String::Compare(propertyValue, S"Inherit", true) == 0 ||
				String::Compare(propertyValue, S"Inherited", true) == 0)
			{
				// Remove this property from us because we want to inherit our parent's value
				RemoveProperty(section, propertySection, propertyName);
				return true;
			}
			// Check if the property is actually set to 'None'?
			if (String::Compare(propertyValue, S"None", true) == 0 ||
				String::Compare(propertyValue, S"Nothing", true) == 0)
			{
				// Actually set the property to nothing
				propertyValue = S"";
			}

			// Check if this property is already inherited?
			if (IsPropertyAlreadyInherited(section, propertySection, propertyName, propertyValue))
			{
				// Since this property doesn't alter our inherited state,
				// Make sure this property is removed from our own properties
				// First check if this property exists in us?
				if (mProperties->PropertyExist(section, propertySection, propertyName))
				{
					// Remove this property from us because this new one won't alter our derived state
					RemoveProperty(section, propertySection, propertyName);
				}
			}
			else
			{
				// Construct us a property
				MOG_Property *property = new MOG_Property(section, propertySection, propertyName, propertyValue);

				// Put this change into the properties file
				mProperties->PutPropertyString(section, propertySection, propertyName, propertyValue);

				// Indicate that this properties has been modified
				mModified = true;

				// Check if we are in the immeadiate mode?
				if (mImmeadiateMode)
				{
					// Save the actual properties file
					Save();

					// Add Property to the Database
					// Check if we are a classification?
					if (mDBClassification)
					{
						// Add property to the inbox properties database
						MOG_DBAssetAPI::AddAssetClassificationProperty(mClassification, S"", property->mSection, property->mKey, property->mPropertyValue);
					}
					// Check if this property's file exists within the inboxes?
					else if (mDBInbox)
					{
						// Add property to the inbox properties database
						MOG_DBInboxAPI::UpdateInboxAssetProperty(mAssetFilename, property);
					}
					else if (mAssetFilename->GetVersionTimeStamp()->Length)
					{
						// Remove property to the inbox properties database
						MOG_DBAssetAPI::AddAssetVersionProperty(mAssetFilename, mAssetFilename->GetVersionTimeStamp(), property->mSection, property->mKey, property->mPropertyValue);
					}
				}
			}

			if( this->PropertyChanged )
			{
				OnPropertyChanged( new PropertyEventArgs( section, propertySection, propertyName, propertyValue, ""/*scopeName*/));
			}

			return true;
		}

		if( this->PropertyChanged )
		{
			OnPropertyChanged( new PropertyEventArgs( section, propertySection, propertyName, propertyValue, ""/*scopeName*/));
		}
	}

	return false;
}


bool MOG_Properties::SetProperty(String *section, String *propertySection, String *propertyName, String *propertyValue, String *scopeName)
{
	// If we are scope specific, append the scope to the property section
	if (scopeName->Length && String::Compare(scopeName, "All", true) != 0)
	{
		String *scopedPropertySection = String::Concat(propertySection, S".", scopeName);
		return SetProperty(section, scopedPropertySection, propertyName, propertyValue);
	}

	return SetProperty(section, propertySection, propertyName, propertyValue);
}


bool MOG_Properties::SetProperties(ArrayList *properties)
{
	// Make sure we have properties?
	if (properties)
	{
		// Loop through all the specified properties
		for (int p = 0; p < properties->Count; p++)
		{
			MOG_Property *property = __try_cast<MOG_Property *>(properties->Item[p]);
			if (property)
			{
				SetProperty(property->mSection, property->mPropertySection, property->mPropertyKey, property->mPropertyValue);
			}
		}
	}

	return true;
}


bool MOG_Properties::RemoveProperty(MOG_Property *removeProperty)
{
	return RemoveProperty(removeProperty->mSection, removeProperty->mPropertySection, removeProperty->mPropertyKey);
}

bool MOG_Properties::RemoveProperty(String *section, String *propertySection, String *propertyName)
{
	// Check if we can modify this instance?
	if (mCanModify)
	{
		// Construct us a property
		MOG_Property *property = new MOG_Property(section, propertySection, propertyName, S"");

		// Remove this Property from the properties file
		mProperties->RemovePropertyString(section, propertySection, propertyName);

		// Indicate that this properties has been modified
		mModified = true;

		// Check if we are in the immeadiate mode?
		if (mImmeadiateMode)
		{
			// Save the actual properties file
			Save();

			// Remove Property from the Database
			// Check if we are a classification?
			if (mDBClassification)
			{
				// Add property to the inbox properties database
				MOG_DBAssetAPI::RemoveAssetClassificationProperty(mClassification, S"", property->mSection, property->mKey);
			}
			// Check if this property's file exists within the inboxes?
			else if (mDBInbox)
			{
				// Remove property to the inbox properties database
				MOG_DBInboxAPI::RemoveInboxAssetProperty(mAssetFilename, property->mSection, property->mKey);
			}
			else if (mAssetFilename->GetVersionTimeStamp()->Length)
			{
				// Remove property to the inbox properties database
				MOG_DBAssetAPI::RemoveAssetVersionProperty(mAssetFilename, mAssetFilename->GetVersionTimeStamp(), property->mSection, property->mKey);
			}
		}

		return true;
	}

	return false;
}


bool MOG_Properties::RemoveProperty(String *section, String *propertySection, String *propertyName, String *scopeName)
{
	// If we are scope specific, append the scope to the property section
	if (scopeName->Length && String::Compare(scopeName, "All", true) != 0)
	{
		String *scopedPropertySection = String::Concat(propertySection, S".", scopeName);
		return RemoveProperty(section, scopedPropertySection, propertyName);
	}

	return RemoveProperty(section, propertySection, propertyName);
}


bool MOG_Properties::AddRelationship(MOG_Property *relationshipProperty)
{
	// Make sure we have something valid to add?
	if (relationshipProperty && 
		relationshipProperty->mPropertySection && 
		relationshipProperty->mPropertyKey)
	{
		// If we have no PropertyValue...
		if( relationshipProperty->mPropertyValue->Length == 0 )
		{
			relationshipProperty->mPropertyValue = S"None";
		}

		// Set the relationship property
		SetProperty(relationshipProperty->mSection, relationshipProperty->mPropertySection, relationshipProperty->mPropertyKey, relationshipProperty->mPropertyValue);
	}

	return true;
}


bool MOG_Properties::AddRelationships(ArrayList *relationshipProperties)
{
	// Add all the relationshipProperties
	for (int p = 0; p < relationshipProperties->Count; p++)
	{
		MOG_Property *relationshipProperty = __try_cast<MOG_Property*>(relationshipProperties->Item[p]);
		AddRelationship(relationshipProperty);
	}

	return true;
}

bool MOG_Properties::RemoveRelationship(MOG_Property *relationshipProperty)
{
	// Make sure we have something valid to remove?
	if (relationshipProperty && 
		relationshipProperty->mPropertySection->Length &&
		relationshipProperty->mPropertyKey->Length)
	{
		// Remove the relationship property without scope
		RemoveProperty(relationshipProperty->mSection, relationshipProperty->mPropertySection, relationshipProperty->mPropertyKey);
		// Remove the relationship property with scope just in case it was platform specific
		RemoveProperty(relationshipProperty->mSection, relationshipProperty->mPropertySection, relationshipProperty->mPropertyKey, MOG_ControllerPackage::GetPackagePlatform(relationshipProperty->mPropertyKey));
	}

	return true;
}

bool MOG_Properties::RemoveRelationships(ArrayList *relationshipProperties)
{
	// Remove all the relationshipProperties
	for (int p = 0; p < relationshipProperties->Count; p++)
	{
		MOG_Property *relationshipProperty = __try_cast<MOG_Property*>(relationshipProperties->Item[p]);
		RemoveRelationship(relationshipProperty);
	}

	return true;
}

bool MOG_Properties::RemoveRelationships(String *relationshipText)
{
	// Remove all the relationships
	return mProperties->RemovePropertySection(PROPTEXT_Relationships, relationshipText) == 1;
}


ArrayList *MOG_Properties::GetApplicableRelationships(String *assetName)
{
	ArrayList *relationshipAssignmentProperties = GetApplicableProperties();

	// Remove unrelated relationship assignments...Don't auto increment our counter because the remove will collapse the array around us
	for (int i = 0; i < relationshipAssignmentProperties->Count; /*i++*/)
	{
		MOG_Property *relationshipAssignmentProperty = __try_cast<MOG_Property*>(relationshipAssignmentProperties->Item[i]);
		if (!relationshipAssignmentProperty->mPropertyKey->ToLower()->StartsWith(assetName->ToLower()))
		{
			// Remove this unrelated relationship assignment
			relationshipAssignmentProperties->RemoveAt(i);
			// Don't auto increment our counter because the remove will collapse the array around us
			continue;
		}
		// Now we can increament our counter because we didn't remove this relationship assignment
		i++;
	}

	return relationshipAssignmentProperties;
}



// --------------------------------------------------------------------------------
// Property Getters
// --------------------------------------------------------------------------------
String *MOG_Properties::get_Creator()
{
	return GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetStats, PROPTEXT_AssetStats_Creator);
}

String *MOG_Properties::get_SourceMachine()
{
	return GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetStats, PROPTEXT_AssetStats_SourceMachine);
}

String *MOG_Properties::get_SourcePath()
{
	return GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetStats, PROPTEXT_AssetStats_SourcePath);
}

String *MOG_Properties::get_CreatedTime()
{
	return GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetStats, PROPTEXT_AssetStats_CreatedTime);
}

String *MOG_Properties::get_ModifiedTime()
{
	return GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetStats, PROPTEXT_AssetStats_ModifiedTime);
}

String *MOG_Properties::get_RippedTime()
{
	return GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetStats, PROPTEXT_AssetStats_RippedTime);
}

String *MOG_Properties::get_PreviousRevision()
{
	return GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetStats, PROPTEXT_AssetStats_PreviousRevision);
}

String *MOG_Properties::get_BlessedTime()
{
	return GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetStats, PROPTEXT_AssetStats_BlessedTime);
}

String *MOG_Properties::get_Owner()
{
	return GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetStats, PROPTEXT_AssetStats_Owner);
}

String *MOG_Properties::get_Status()
{
	return GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetStats, PROPTEXT_AssetStats_Status);
}

bool MOG_Properties::get_IsUnprocessed()
{
	bool bIsUnprocessed = false;

	// Check the state of the asset?
	switch (MOG_AssetStatus::GetType(Status))
	{
		case MOG_AssetStatusType::None:
		case MOG_AssetStatusType::Unknown:
		case MOG_AssetStatusType::Waiting:
		case MOG_AssetStatusType::Pending:
		case MOG_AssetStatusType::Unprocessed:
		case MOG_AssetStatusType::Ripping:
		case MOG_AssetStatusType::RipError:
		case MOG_AssetStatusType::Deleted:
		case MOG_AssetStatusType::Modifying:
		case MOG_AssetStatusType::Importing:
		case MOG_AssetStatusType::ImportError:
		case MOG_AssetStatusType::CreationError:
			bIsUnprocessed = true;
			break;
	}

	return bIsUnprocessed;
}

String *MOG_Properties::get_Size()
{
	return GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetStats, PROPTEXT_AssetStats_Size);
}

String *MOG_Properties::get_LastComment()
{
	return GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetStats, PROPTEXT_AssetStats_LastComment);
}

String *MOG_Properties::get_AssetIcon()
{
	return GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetInfo, PROPTEXT_AssetInfo_AssetIcon);
}

Boolean MOG_Properties::get_IsLibrary()
{
	return ParseBool(GetPropertyString(PROPTEXT_Properties, PROPTEXT_ClassificationInfo, PROPTEXT_ClassificationInfo_IsLibrary));
}

MOG_InheritedBoolean MOG_Properties::get_IsLibrary_InheritedBoolean()
{
	return ParseInheritedBoolean(GetPropertyString(PROPTEXT_Properties, PROPTEXT_ClassificationInfo, PROPTEXT_ClassificationInfo_IsLibrary));
}

Boolean MOG_Properties::get_MaintainLock()
{
	return ParseBool(GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_MaintainLock));
}

MOG_InheritedBoolean MOG_Properties::get_MaintainLock_InheritedBoolean()
{
	return ParseInheritedBoolean(GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_MaintainLock));
}

Boolean MOG_Properties::get_LockPackageManagement()
{
	return ParseBool(GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_LockPackageManagement));
}

MOG_InheritedBoolean MOG_Properties::get_LockPackageManagement_InheritedBoolean()
{
	return ParseInheritedBoolean(GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_LockPackageManagement));
}

Boolean MOG_Properties::get_ShowPostLockComment()
{
	return ParseBool(GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_ShowPostLockComment), true);
}

MOG_InheritedBoolean MOG_Properties::get_ShowPostLockComment_InheritedBoolean()
{
	return ParseInheritedBoolean(GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_ShowPostLockComment), true);
}

Boolean MOG_Properties::get_RequireLockComment()
{
	return ParseBool(GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_RequireLockComment));
}

MOG_InheritedBoolean MOG_Properties::get_RequireLockComment_InheritedBoolean()
{
	return ParseInheritedBoolean(GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_RequireLockComment));
}

Boolean MOG_Properties::get_ShowLocalModifiedWarning()
{
	return ParseBool(GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_ShowLocalModifiedWarning), true);
}

MOG_InheritedBoolean MOG_Properties::get_ShowLocalModifiedWarning_InheritedBoolean()
{
	return ParseInheritedBoolean(GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_ShowLocalModifiedWarning), true);
}

Boolean MOG_Properties::get_AutoLockOnImport()
{
	return ParseBool(GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_AutoLockOnImport), false);
}

MOG_InheritedBoolean MOG_Properties::get_AutoLockOnImport_InheritedBoolean()
{
	return ParseInheritedBoolean(GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_AutoLockOnImport), false);
}


String *MOG_Properties::get_ClassIcon()
{
	return GetPropertyString(PROPTEXT_Properties, PROPTEXT_ClassificationInfo, PROPTEXT_ClassificationInfo_ClassIcon);
}

String *MOG_Properties::get_FilterInclusions()
{
	return GetPropertyString(PROPTEXT_Properties, PROPTEXT_ClassificationInfo, PROPTEXT_ClassificationInfo_FilterInclusions);
}

String *MOG_Properties::get_FilterExclusions()
{
	return GetPropertyString(PROPTEXT_Properties, PROPTEXT_ClassificationInfo, PROPTEXT_ClassificationInfo_FilterExclusions);
}

Boolean MOG_Properties::get_PromptUserOnFilterViolation()
{
	return ParseBool(GetPropertyString(PROPTEXT_Properties, PROPTEXT_ClassificationInfo, PROPTEXT_ClassificationInfo_PromptUserOnFilterViolation), true);
}

MOG_InheritedBoolean MOG_Properties::get_PromptUserOnFilterViolation_InheritedBoolean()
{
	return ParseInheritedBoolean(GetPropertyString(PROPTEXT_Properties, PROPTEXT_ClassificationInfo, PROPTEXT_ClassificationInfo_PromptUserOnFilterViolation), true);
}

String *MOG_Properties::get_SyncLabel()
{
	return GetPropertyString(PROPTEXT_Properties, PROPTEXT_SyncOptions, PROPTEXT_SyncOptions_SyncLabel);
}

String *MOG_Properties::get_SyncTargetPath()
{
	String *syncTargetPath = S"";

	syncTargetPath = GetPropertyString(PROPTEXT_Properties, PROPTEXT_SyncOptions, PROPTEXT_SyncOptions_SyncTargetPath);

	return 	syncTargetPath;
}

Boolean MOG_Properties::get_SyncFiles()
{
	return ParseBool(GetPropertyString(PROPTEXT_Properties, PROPTEXT_SyncOptions, PROPTEXT_SyncOptions_SyncFiles), true);
}

MOG_InheritedBoolean MOG_Properties::get_SyncFiles_InheritedBoolean()
{
	return ParseInheritedBoolean(GetPropertyString(PROPTEXT_Properties, PROPTEXT_SyncOptions, PROPTEXT_SyncOptions_SyncFiles), true);
}

Boolean MOG_Properties::get_SyncAsReadOnly()
{
	return ParseBool(GetPropertyString(PROPTEXT_Properties, PROPTEXT_SyncOptions, PROPTEXT_SyncOptions_SyncAsReadOnly), false);
}

MOG_InheritedBoolean MOG_Properties::get_SyncAsReadOnly_InheritedBoolean()
{
	return ParseInheritedBoolean(GetPropertyString(PROPTEXT_Properties, PROPTEXT_SyncOptions, PROPTEXT_SyncOptions_SyncAsReadOnly), false);
}

String *MOG_Properties::get_AssetViewer()
{
	return GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetInfo, PROPTEXT_AssetInfo_AssetViewer);
}

String *MOG_Properties::get_Classification()
{
	return GetPropertyString(PROPTEXT_Properties, PROPTEXT_ClassificationInfo, PROPTEXT_ClassificationInfo_Classification);
}

String *MOG_Properties::get_AssetRipTasker()
{
	return GetPropertyString(PROPTEXT_Properties, PROPTEXT_RipOptions, PROPTEXT_RipOptions_AssetRipTasker);
}

String *MOG_Properties::get_AssetRipper()
{
	return GetPropertyString(PROPTEXT_Properties, PROPTEXT_RipOptions, PROPTEXT_RipOptions_AssetRipper);
}

String *MOG_Properties::get_ValidSlaves()
{
	return GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_ValidSlaves);
}

String *MOG_Properties::get_PropertyMenu()
{
	return GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetInfo, PROPTEXT_AssetInfo_PropertyMenu);
}

String *MOG_Properties::get_Description()
{
	return GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetInfo, PROPTEXT_AssetInfo_Description);
}

String *MOG_Properties::get_Group()
{
	return GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetInfo, PROPTEXT_AssetInfo_Group);
}

Boolean MOG_Properties::get_IsPackagedAsset()
{
	// Check if we are already considered a packaged asset?
	bool value = ParseBool(GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetInfo, PROPTEXT_AssetInfo_IsPackagedAsset));
	if (value != true)
	{
		// Check if we should be because we are have a package assignment?
		ArrayList* list = GetPackages();
		if (list && list->Count > 0)
		{
			return true;
		}
	}

	return value;
}

MOG_InheritedBoolean MOG_Properties::get_IsPackagedAsset_InheritedBoolean()
{
	// Check our current asset?
	MOG_InheritedBoolean value = ParseInheritedBoolean(GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetInfo, PROPTEXT_AssetInfo_IsPackagedAsset));
	if (value != MOG_InheritedBoolean::True)
	{
		// Check if we should be because we are have a package assignment?
		ArrayList* list = GetPackages();
		if (list && list->Count > 0)
		{
			return MOG_InheritedBoolean::True;
		}
	}

	return value;
}

Boolean MOG_Properties::get_NativeDataType()
{
	// Check if this is a Library asset?
	if (IsLibrary)
	{
		//Library assets need to check files out from the Files.Imported directory
		//In order to do that they must always be native
		//This is a quick fix to get the library working until we actually redesign the whole thing - JWW
		return true;
	}

	// Check if this asset is assigned a RipTasker or a Ripper?
	if (AssetRipTasker != NULL && 
		AssetRipTasker->Length != 0)
	{
		return false;
	}

	if (AssetRipper != NULL &&		
		AssetRipper->Length != 0)
	{
		return false;
	}

	return true;
}

Boolean MOG_Properties::get_DivergentPlatformDataType()
{
	return ParseBool(GetPropertyString(PROPTEXT_Properties, PROPTEXT_RipOptions, PROPTEXT_RipOptions_DivergentPlatformDataType));
}

MOG_InheritedBoolean MOG_Properties::get_DivergentPlatformDataType_InheritedBoolean()
{
	return ParseInheritedBoolean(GetPropertyString(PROPTEXT_Properties, PROPTEXT_RipOptions, PROPTEXT_RipOptions_DivergentPlatformDataType));
}

MOG_DefaultPrompt MOG_Properties::get_DefaultAssetNameIncludeExtension()
{
	return ParseDefaultPrompt(GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_DefaultAssetNameIncludeExtension));
}

String* MOG_Properties::get_DefaultAssetNamePlatform()
{
	// Default to 'All' since that is the vast majority of asset
	String* defaultAssetNamePlatform = "All";
	if (DoesPropertyExist(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_DefaultAssetNamePlatform))
	{
		defaultAssetNamePlatform = GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_DefaultAssetNamePlatform);
	}

	return defaultAssetNamePlatform;
}

Boolean MOG_Properties::get_UnBlessable()
{
	return ParseBool(GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_UnBlessable));
}

MOG_InheritedBoolean MOG_Properties::get_UnBlessable_InheritedBoolean()
{
	return ParseInheritedBoolean(GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_UnBlessable));
}

Boolean MOG_Properties::get_UniquePackageAssignmentVerification()
{
	return ParseBool(GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_UniquePackageAssignmentVerification), true);
}

MOG_InheritedBoolean MOG_Properties::get_UniquePackageAssignmentVerification_InheritedBoolean()
{
	return ParseInheritedBoolean(GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_UniquePackageAssignmentVerification), true);
}

Boolean MOG_Properties::get_OutofdateVerification()
{
	return ParseBool(GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_OutofdateVerification), false);
}

MOG_InheritedBoolean MOG_Properties::get_OutofdateVerification_InheritedBoolean()
{
	return ParseInheritedBoolean(GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_OutofdateVerification), false);
}

Boolean MOG_Properties::get_LocalVerificationBeforeBless()
{
	return ParseBool(GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_LocalVerificationBeforeBless));
}

MOG_InheritedBoolean MOG_Properties::get_LocalVerificationBeforeBless_InheritedBoolean()
{
	return ParseInheritedBoolean(GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_LocalVerificationBeforeBless));
}

String *MOG_Properties::get_UnReferencedRevisionHistory()
{
	return GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_UnReferencedRevisionHistory);
}

String *MOG_Properties::get_BlessTarget()
{
	return GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_BlessTarget);
}

String *MOG_Properties::get_BlessEmailNotify()
{
	return GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_BlessEmailNotify);
}

Boolean MOG_Properties::get_ShowRipCommandWindow()
{
	return ParseBool(GetPropertyString(PROPTEXT_Properties, PROPTEXT_RipOptions, PROPTEXT_RipOptions_ShowRipCommandWindow));
}

MOG_InheritedBoolean MOG_Properties::get_ShowRipCommandWindow_InheritedBoolean()
{
	return ParseInheritedBoolean(GetPropertyString(PROPTEXT_Properties, PROPTEXT_RipOptions, PROPTEXT_RipOptions_ShowRipCommandWindow));
}

Boolean MOG_Properties::get_UseTempRipDir()
{
	return ParseBool(GetPropertyString(PROPTEXT_Properties, PROPTEXT_RipOptions, PROPTEXT_RipOptions_UseTempRipDir));
}

MOG_InheritedBoolean MOG_Properties::get_UseTempRipDir_InheritedBoolean()
{
	return ParseInheritedBoolean(GetPropertyString(PROPTEXT_Properties, PROPTEXT_RipOptions, PROPTEXT_RipOptions_UseTempRipDir));
}

Boolean MOG_Properties::get_UseLocalTempRipDir()
{
	return ParseBool(GetPropertyString(PROPTEXT_Properties, PROPTEXT_RipOptions, PROPTEXT_RipOptions_UseLocalTempRipDir));
}

MOG_InheritedBoolean MOG_Properties::get_UseLocalTempRipDir_InheritedBoolean()
{
	return ParseInheritedBoolean(GetPropertyString(PROPTEXT_Properties, PROPTEXT_RipOptions, PROPTEXT_RipOptions_UseLocalTempRipDir));
}

Boolean MOG_Properties::get_CopyFilesIntoTempRipDir()
{
	return ParseBool(GetPropertyString(PROPTEXT_Properties, PROPTEXT_RipOptions, PROPTEXT_RipOptions_CopyFilesIntoTempRipDir));
}

MOG_InheritedBoolean MOG_Properties::get_CopyFilesIntoTempRipDir_InheritedBoolean()
{
	return ParseInheritedBoolean(GetPropertyString(PROPTEXT_Properties, PROPTEXT_RipOptions, PROPTEXT_RipOptions_CopyFilesIntoTempRipDir));
}

Boolean MOG_Properties::get_AutoDetectRippedFiles()
{
	return ParseBool(GetPropertyString(PROPTEXT_Properties, PROPTEXT_RipOptions, PROPTEXT_RipOptions_AutoDetectRippedFiles));
}

MOG_InheritedBoolean MOG_Properties::get_AutoDetectRippedFiles_InheritedBoolean()
{
	return ParseInheritedBoolean(GetPropertyString(PROPTEXT_Properties, PROPTEXT_RipOptions, PROPTEXT_RipOptions_AutoDetectRippedFiles));
}

Boolean MOG_Properties::get_IsBuild()
{
	return ParseBool(GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetInfo, PROPTEXT_AssetInfo_IsBuild));
}

MOG_InheritedBoolean MOG_Properties::get_IsBuild_InheritedBoolean()
{
	return ParseInheritedBoolean(GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetInfo, PROPTEXT_AssetInfo_IsBuild));
}

Boolean MOG_Properties::get_ShowBuildCommandWindow()
{
	return ParseBool(GetPropertyString(PROPTEXT_Properties, PROPTEXT_BuildOptions, PROPTEXT_BuildOptions_ShowBuildCommandWindow));
}

MOG_InheritedBoolean MOG_Properties::get_ShowBuildCommandWindow_InheritedBoolean()
{
	return ParseInheritedBoolean(GetPropertyString(PROPTEXT_Properties, PROPTEXT_BuildOptions, PROPTEXT_BuildOptions_ShowBuildCommandWindow));
}

String *MOG_Properties::get_BuildTool()
{
	return GetPropertyString(PROPTEXT_Properties, PROPTEXT_BuildOptions, PROPTEXT_BuildOptions_BuildTool);
}

String *MOG_Properties::get_BuildWorkingDirectory()
{
	return GetPropertyString(PROPTEXT_Properties, PROPTEXT_BuildOptions, PROPTEXT_BuildOptions_BuildWorkingDirectory);
}

String *MOG_Properties::get_DefaultPackageFileExtension()
{
	return GetPropertyString(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_DefaultPackageFileExtension);
}

Boolean MOG_Properties::get_ShowPackageCommandWindow()
{
	return ParseBool(GetPropertyString(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_ShowPackageCommandWindow));
}

MOG_InheritedBoolean MOG_Properties::get_ShowPackageCommandWindow_InheritedBoolean()
{
	return ParseInheritedBoolean(GetPropertyString(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_ShowPackageCommandWindow));
}

MOG_PackageStyle MOG_Properties::get_PackageStyle()
{
	return ParsePackageStyle(GetPropertyString(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_PackageStyle));
}

Boolean MOG_Properties::get_IsPackage()
{
	return ParseBool(GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetInfo, PROPTEXT_AssetInfo_IsPackage));
}

MOG_InheritedBoolean MOG_Properties::get_IsPackage_InheritedBoolean()
{
	return ParseInheritedBoolean(GetPropertyString(PROPTEXT_Properties, PROPTEXT_AssetInfo, PROPTEXT_AssetInfo_IsPackage));
}

Boolean MOG_Properties::get_AutoPackage()
{
	return ParseBool(GetPropertyString(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_AutoPackage));
}

MOG_InheritedBoolean MOG_Properties::get_AutoPackage_InheritedBoolean()
{
	return ParseInheritedBoolean(GetPropertyString(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_AutoPackage));
}

Boolean MOG_Properties::get_ExecuteNetworkPackageMerge()
{
	return ParseBool(GetPropertyString(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_ExecuteNetworkPackageMerge), true);
}

MOG_InheritedBoolean MOG_Properties::get_ExecuteNetworkPackageMerge_InheritedBoolean()
{
	return ParseInheritedBoolean(GetPropertyString(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_ExecuteNetworkPackageMerge), true);
}

Boolean MOG_Properties::get_ClusterPackaging()
{
	return ParseBool(GetPropertyString(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_ClusterPackaging));
}

MOG_InheritedBoolean MOG_Properties::get_ClusterPackaging_InheritedBoolean()
{
	return ParseInheritedBoolean(GetPropertyString(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_ClusterPackaging));
}

String *MOG_Properties::get_PackagePreMergeEvent()
{
	return GetPropertyString(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_PackagePreMergeEvent);
}

String *MOG_Properties::get_PackagePostMergeEvent()
{
	return GetPropertyString(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_PackagePostMergeEvent);
}

String *MOG_Properties::get_TaskFileTool()
{
	return GetPropertyString(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_TaskFileTool);
}

String *MOG_Properties::get_InputPackageTaskFile()
{
	return GetPropertyString(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_InputPackageTaskFile);
}

String *MOG_Properties::get_OutputPackageTaskFile()
{
	return GetPropertyString(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_OutputPackageTaskFile);
}

String *MOG_Properties::get_PackageWorkspaceDirectory()
{
	return GetPropertyString(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_PackageWorkspaceDirectory);
}

String *MOG_Properties::get_PackageDataDirectory()
{
	return GetPropertyString(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_PackageDataDirectory);
}

Boolean MOG_Properties::get_SyncPackageWorkspaceDirectory()
{
	return ParseBool(GetPropertyString(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_SyncPackageWorkspaceDirectory));
}

MOG_InheritedBoolean MOG_Properties::get_SyncPackageWorkspaceDirectory_InheritedBoolean()
{
	return ParseInheritedBoolean(GetPropertyString(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_SyncPackageWorkspaceDirectory));
}

Boolean MOG_Properties::get_CleanupPackageWorkspaceDirectory()
{
	return ParseBool(GetPropertyString(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_CleanupPackageWorkspaceDirectory));
}

MOG_InheritedBoolean MOG_Properties::get_CleanupPackageWorkspaceDirectory_InheritedBoolean()
{
	return ParseInheritedBoolean(GetPropertyString(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_CleanupPackageWorkspaceDirectory));
}

MOG_PackageCommandPropagation MOG_Properties::get_PackageCommand_Propagation()
{
	return ParsePackageCommandPropagation(GetPropertyString(PROPTEXT_Properties, PROPTEXT_PackageCommands, PROPTEXT_PackageCommands_PackageCommand_Propagation));
}

String *MOG_Properties::get_PackageCommand_Add()
{
	return GetPropertyString(PROPTEXT_Properties, PROPTEXT_PackageCommands, PROPTEXT_PackageCommands_PackageCommand_Add);
}

String *MOG_Properties::get_PackageCommand_Remove()
{
	return GetPropertyString(PROPTEXT_Properties, PROPTEXT_PackageCommands, PROPTEXT_PackageCommands_PackageCommand_Remove);
}

String *MOG_Properties::get_PackageCommand_DeletePackageFile()
{
	return GetPropertyString(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_PackageCommand_DeletePackageFile);
}

String *MOG_Properties::get_PackageCommand_ResolveLateResolvers()
{
	return GetPropertyString(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_PackageCommand_ResolveLateResolvers);
}



// --------------------------------------------------------------------------------
// Property Setters
// --------------------------------------------------------------------------------
void MOG_Properties::set_Creator(String *value)
{
	// Make sure this is a valid user?
	if (!MOG_ControllerProject::IsValidUser(value))
	{
		// Inform the user they entered an invalid user
		MOGPromptResult result = MOG_Prompt::PromptResponse(	S"Set Property",
																String::Concat(	S"'", value, S"' is not a valid user in the project.\n",
																				S"The Property was not set."),
																NULL, 
																MOGPromptButtons::OK, MOG_ALERT_LEVEL::ALERT);
		// Don't set the property
		return;
	}

	SetProperty(PROPTEXT_Properties, PROPTEXT_AssetStats, PROPTEXT_AssetStats_Creator, value); 
}

void MOG_Properties::set_Owner(String *value)
{
	// Make sure this is a valid user?
	if (!MOG_ControllerProject::IsValidUser(value))
	{
		// Inform the user they entered an invalid user
		MOGPromptResult result = MOG_Prompt::PromptResponse(	S"Set Property",
																String::Concat(	S"'", value, S"' is not a valid user in the project.\n",
																				S"The Property was not set."),
																NULL, 
																MOGPromptButtons::OK, MOG_ALERT_LEVEL::ALERT);
		// Don't set the property
		return;
	}

	SetProperty(PROPTEXT_Properties, PROPTEXT_AssetStats, PROPTEXT_AssetStats_Owner, value);
}

void MOG_Properties::set_SourceMachine(String *value)
{
	// Check for invalid characters
	if (MOG_ControllerSystem::InvalidWindowsFilenameCharactersCheck(value, true))
	{
		return;
	}

	SetProperty(PROPTEXT_Properties, PROPTEXT_AssetStats, PROPTEXT_AssetStats_SourceMachine, value); 
}

void MOG_Properties::set_SourcePath(String *value)
{
	// Check for invalid characters
	if (MOG_ControllerSystem::InvalidWindowsPathCharactersCheck(value, true))
	{
		return;
	}

	SetProperty(PROPTEXT_Properties, PROPTEXT_AssetStats, PROPTEXT_AssetStats_SourcePath, value); 
}

void MOG_Properties::set_CreatedTime(String *value)
{
	SetTimestamp(PROPTEXT_Properties, PROPTEXT_AssetStats, PROPTEXT_AssetStats_CreatedTime, value);
}

void MOG_Properties::set_ModifiedTime(String *value)
{
	SetTimestamp(PROPTEXT_Properties, PROPTEXT_AssetStats, PROPTEXT_AssetStats_ModifiedTime, value);
}

void MOG_Properties::set_RippedTime(String *value)
{
	SetTimestamp(PROPTEXT_Properties, PROPTEXT_AssetStats, PROPTEXT_AssetStats_RippedTime, value);
}

void MOG_Properties::set_BlessedTime(String *value)
{
	SetTimestamp(PROPTEXT_Properties, PROPTEXT_AssetStats, PROPTEXT_AssetStats_BlessedTime, value);
}

void MOG_Properties::set_PreviousRevision(String *value)
{
	SetTimestamp(PROPTEXT_Properties, PROPTEXT_AssetStats, PROPTEXT_AssetStats_PreviousRevision, value);
}

void MOG_Properties::SetTimestamp(String *section, String *propertySection, String *propertyName, String *timestamp)
{
	try
	{
		// Empty strings are fine too?
		if (timestamp->Length == 0)
		{
			SetProperty(section, propertySection, propertyName, timestamp);
			return;
		}
		// Make sure it is a valid length?
		if (timestamp->Length == 15)
		{
			Int64 testInt = Int64::Parse( timestamp );
			SetProperty(section, propertySection, propertyName, timestamp);
			return;
		}
	}
	catch(...)
	{
	}

	// Inform the user this was an invalid timestamp
	MOGPromptResult result = MOG_Prompt::PromptResponse(	S"Set Property",
															String::Concat(	S"The specified value was not a valid 15 digit timestamp.\n",
																			S"The Property was not set."),
															NULL, 
															MOGPromptButtons::OK, MOG_ALERT_LEVEL::ALERT);
}

void MOG_Properties::set_Status(String *value)
{
	// Make sure this is a valid status?
	if (MOG_AssetStatus::GetType(value) == MOG_AssetStatusType::None)
	{
		// Inform the user they entered an invalid user
		MOGPromptResult result = MOG_Prompt::PromptResponse(	S"Set Property",
																String::Concat(	S"'", value, S"' is not a valid AssetStatus.\n",
																				S"The Property was not set."),
																NULL, 
																MOGPromptButtons::OK, MOG_ALERT_LEVEL::ALERT);
		// Don't set the property
		return;
	}

	SetProperty(PROPTEXT_Properties, PROPTEXT_AssetStats, PROPTEXT_AssetStats_Status, value);
}

void MOG_Properties::set_Size(String *value)
{
	try
	{
		Int64 testInt = Int64::Parse( value );
		SetProperty(PROPTEXT_Properties, PROPTEXT_AssetStats, PROPTEXT_AssetStats_Size, value);
	}
	catch(...)
	{
	}
}

void MOG_Properties::set_LastComment(String *value)
{	// Strip out the line breaks and leading/trailing spaces
	value = value->Replace("\n", "   ")->Trim();
	if (value->Length)
	{
		// Don't ever set this if it is blank so it will preserve last user to comment their bless in case it is bless targeted.
		SetProperty(PROPTEXT_Properties, PROPTEXT_AssetStats, PROPTEXT_AssetStats_LastComment, value);
	}
}

void MOG_Properties::set_ClassIcon(String *value)
{
	// Always make sure we bust this down to the relative path within the tools dir
	String* newtoolPath = MOG_ControllerSystem::InternalizeTool(value, S"Images");
	SetProperty(PROPTEXT_Properties, PROPTEXT_ClassificationInfo, PROPTEXT_ClassificationInfo_ClassIcon, newtoolPath);
}

void MOG_Properties::set_FilterInclusions(String *value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_ClassificationInfo, PROPTEXT_ClassificationInfo_FilterInclusions, value);
}

void MOG_Properties::set_FilterExclusions(String *value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_ClassificationInfo, PROPTEXT_ClassificationInfo_FilterExclusions, value);
}

void MOG_Properties::set_PromptUserOnFilterViolation(bool value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_ClassificationInfo, PROPTEXT_ClassificationInfo_PromptUserOnFilterViolation, value.ToString());
}

void MOG_Properties::set_PromptUserOnFilterViolation_InheritedBoolean(MOG_InheritedBoolean value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_ClassificationInfo, PROPTEXT_ClassificationInfo_PromptUserOnFilterViolation, MapInheritedBoolean(value));
}

void MOG_Properties::set_SyncLabel(String *value)
{
	// Check for invalid characters
	if (MOG_ControllerSystem::InvalidMOGCharactersCheck(value, true))
	{
		return;
	}

	SetProperty(PROPTEXT_Properties, PROPTEXT_SyncOptions, PROPTEXT_SyncOptions_SyncLabel, value);
}

void MOG_Properties::set_SyncTargetPath(String *value)
{
	// Resolve any contained tokens
	String* newResolvedValue = "";

	// Check if this is a classification only?
	if (mClassification->Length)
	{
		// Resolve for mClassification and Properties
		String *seeds = "";
		seeds = MOG_Tokens::AppendTokenSeeds(seeds, MOG_Tokens::GetClassificationTokenSeeds(mClassification));
		seeds = MOG_Tokens::AppendTokenSeeds(seeds, MOG_Tokens::GetPropertyTokenSeeds(GetPropertyList()));
		newResolvedValue = MOG_Tokens::GetFormattedString(value, seeds);
	}
	else
	{
		// Resolve for AssetFilename and Properties
		String *seeds = "";
		seeds = MOG_Tokens::AppendTokenSeeds(seeds, MOG_Tokens::GetFilenameTokenSeeds(GetAssetFilename()));
		seeds = MOG_Tokens::AppendTokenSeeds(seeds, MOG_Tokens::GetPropertyTokenSeeds(GetPropertyList()));
		newResolvedValue = MOG_Tokens::GetFormattedString(value, seeds);
	}
	
	// Check for invalid characters
	String *invalidChars = MOG_ControllerSystem::GetInvalidWindowsPathCharacters();
	// Include ':' because SyncTargetPath must be a relative path
	invalidChars = String::Concat(invalidChars, S":");
	if (MOG_ControllerSystem::InvalidCharactersCheck(newResolvedValue, invalidChars, true))
	{
		return;
	}

	// Make sure there is no drive letter listed in the specified SyncTargetPath
	if (newResolvedValue->IndexOf(S":\\") != -1)
	{
		// First attempt to strip off the active local workspace directory
//?	MultiWorkspaces - We need to be careful about this...what assumptions are we making here?
		MOG_ControllerSyncData *pSyncData = MOG_ControllerProject::GetCurrentSyncDataController();
		if (pSyncData)
		{
			String* testSyncTargetPath = String::Concat(value, S"\\");
			String* testSyncDir = String::Concat(pSyncData->GetSyncDirectory(), S"\\");
			if (testSyncTargetPath->StartsWith(testSyncDir, StringComparison::CurrentCultureIgnoreCase))
			{
				value = testSyncTargetPath->Substring(testSyncDir->Length);
			}
		}

		// Double check to make sure we don't have a drive letter?
		if (value->IndexOf(S":\\") != -1)
		{
			// Inform the user they can't have a drive listed because the SyncTargetPath needs to be relative to the project
			MOGPromptResult result = MOG_Prompt::PromptResponse(	S"Set Property",
																	String::Concat(	S"The SyncTargetPath property can't contain a drive letter because it reflects a relative path within a project.\n",
																					S"The Property was not set."),
																	NULL, 
																	MOGPromptButtons::OK, MOG_ALERT_LEVEL::ALERT);
			// Don't set the property
			return;
		}
	}

    // Make sure we trim off unneeded backslashes and spaces
	value = value->Trim(S"\\ "->ToCharArray());

	// Get the existing property before it is changed so we can compare it below
	MOG_Property* existingProperty = GetProperty(PROPTEXT_Properties, PROPTEXT_SyncOptions, PROPTEXT_SyncOptions_SyncTargetPath);

	// Finally set our prepared property value
	SetProperty(PROPTEXT_Properties, PROPTEXT_SyncOptions, PROPTEXT_SyncOptions_SyncTargetPath, value);

	// Check to see if this is a classification
	if(mDBClassification)
	{
		// Check if the property is actually changing?
		if (existingProperty != NULL &&
			String::Compare(SyncTargetPath, existingProperty->mValue, true) != 0)
		{
			// Fix up this property in all the contained assets
			if (MOG_Properties::FixupInheritedPropertiesForAffectedAssets(mClassification, MOG_PropertyFactory::MOG_Sync_OptionsProperties::New_SyncTargetPath(value)))
			{
				// Automatically save the properties since the user restamped assets
				// This will prevent the user from being prompted to apply their changes when they close the dialog
				Save();
			}
		}
	}
}

void MOG_Properties::set_SyncFiles(Boolean value)
{
	MOG_Property* existingProperty = GetProperty(PROPTEXT_Properties, PROPTEXT_SyncOptions, PROPTEXT_SyncOptions_SyncFiles);

	SetProperty(PROPTEXT_Properties, PROPTEXT_SyncOptions, PROPTEXT_SyncOptions_SyncFiles, value.ToString()); 

	// Check to see if this is a classification
	if(mDBClassification)
	{
		// Check if the property is actually changing?
		if (existingProperty != NULL &&
			String::Compare(SyncFiles.ToString(), existingProperty->mValue, true) != 0)
		{
			// Fix up this property in all the contained assets
			if (MOG_Properties::FixupInheritedPropertiesForAffectedAssets(mClassification, MOG_PropertyFactory::MOG_Sync_OptionsProperties::New_SyncFiles(value)))
			{
				// Automatically save the properties since the user restamped assets
				// This will prevent the user from being prompted to apply their changes when they close the dialog
				Save();
			}
		}
	}
}

void MOG_Properties::set_SyncFiles_InheritedBoolean(MOG_InheritedBoolean value)
{
	SyncFiles = Convert::ToBoolean(MapInheritedBoolean(value));
}

void MOG_Properties::set_SyncAsReadOnly(Boolean value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_SyncOptions, PROPTEXT_SyncOptions_SyncAsReadOnly, value.ToString()); 
}

void MOG_Properties::set_SyncAsReadOnly_InheritedBoolean(MOG_InheritedBoolean value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_SyncOptions, PROPTEXT_SyncOptions_SyncAsReadOnly, MapInheritedBoolean(value));
}

void MOG_Properties::set_AssetIcon(String *value)
{
	// Check for invalid characters
	if (MOG_ControllerSystem::InvalidWindowsPathCharactersCheck(value, true))
	{
		return;
	}

	// Always make sure we bust this down to the relative path within the tools dir
	String* newtoolPath = MOG_ControllerSystem::InternalizeTool(value, S"Images");
	SetProperty(PROPTEXT_Properties, PROPTEXT_AssetInfo, PROPTEXT_AssetInfo_AssetIcon, newtoolPath);
}

void MOG_Properties::set_IsLibrary(Boolean value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_ClassificationInfo, PROPTEXT_ClassificationInfo_IsLibrary, value.ToString()); 
}

void MOG_Properties::set_IsLibrary_InheritedBoolean(MOG_InheritedBoolean value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_ClassificationInfo, PROPTEXT_ClassificationInfo_IsLibrary, MapInheritedBoolean(value));
}

void MOG_Properties::set_MaintainLock(Boolean value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_MaintainLock, value.ToString());
}

void MOG_Properties::set_MaintainLock_InheritedBoolean(MOG_InheritedBoolean value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_MaintainLock, MapInheritedBoolean(value));
}

void MOG_Properties::set_LockPackageManagement(Boolean value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_LockPackageManagement, value.ToString());
}

void MOG_Properties::set_LockPackageManagement_InheritedBoolean(MOG_InheritedBoolean value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_LockPackageManagement, MapInheritedBoolean(value));
}

void MOG_Properties::set_RequireLockComment(Boolean value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_RequireLockComment, value.ToString());
}

void MOG_Properties::set_RequireLockComment_InheritedBoolean(MOG_InheritedBoolean value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_RequireLockComment, MapInheritedBoolean(value));
}

void MOG_Properties::set_ShowPostLockComment(Boolean value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_ShowPostLockComment, value.ToString());
}

void MOG_Properties::set_ShowPostLockComment_InheritedBoolean(MOG_InheritedBoolean value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_ShowPostLockComment, MapInheritedBoolean(value));
}

void MOG_Properties::set_ShowLocalModifiedWarning(Boolean value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_ShowLocalModifiedWarning, value.ToString());
}

void MOG_Properties::set_ShowLocalModifiedWarning_InheritedBoolean(MOG_InheritedBoolean value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_ShowLocalModifiedWarning, MapInheritedBoolean(value));
}

void MOG_Properties::set_AutoLockOnImport(Boolean value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_AutoLockOnImport, value.ToString());
}

void MOG_Properties::set_AutoLockOnImport_InheritedBoolean(MOG_InheritedBoolean value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_AutoLockOnImport, MapInheritedBoolean(value));
}


void MOG_Properties::set_AssetViewer(String *value)
{
	// Check for invalid characters
	if (MOG_ControllerSystem::InvalidWindowsPathCharactersCheck(value, true))
	{
		return;
	}

	// Always make sure we bust this down to the relative path within the tools dir
	String* newtoolPath = MOG_ControllerSystem::InternalizeTool(value, S"Viewers");
	SetProperty(PROPTEXT_Properties, PROPTEXT_AssetInfo, PROPTEXT_AssetInfo_AssetViewer, newtoolPath);
}

void MOG_Properties::set_Classification(String *value)
{
	// Check for invalid characters
	if (MOG_ControllerSystem::InvalidMOGCharactersCheck(value, true))
	{
		return;
	}

	SetProperty(PROPTEXT_Properties, PROPTEXT_ClassificationInfo, PROPTEXT_ClassificationInfo_Classification, value);
}

void MOG_Properties::set_AssetRipTasker(String *value)
{
// We can't perform this check because RipTaskers can include arguments that can handle invalid chars
//	// Check for invalid characters
//	if (MOG_ControllerSystem::InvalidMOGCharactersCheck(value, true))
//	{
//		return;
//	}

	// Check if this is a Library asset?
	if (IsLibrary)
	{
		// Check if they are actually attempting to set it to something?
		if (value->Length)
		{
			//Library assets need to check files out from the Files.Imported directory
			//In order to do that they must always be native
			//This is a quick fix to get the library working until we actually redesign the whole thing - JWW
			// Inform the user they entered an invalid user
			MOGPromptResult result = MOG_Prompt::PromptResponse(	S"Set Property Failed",
																	String::Concat(	S"No support is available for assigning rippers to Library Assets.\n\n",
																					S"Rippers can only be assigned to non-Library Assets."),
																	NULL, 
																	MOGPromptButtons::OK, MOG_ALERT_LEVEL::ALERT);
			value = S"";
		}
	}

	// Always make sure we bust this down to the relative path within the tools dir
	String* newtoolPath = MOG_ControllerSystem::InternalizeTool(value, S"Rippers");
	SetProperty(PROPTEXT_Properties, PROPTEXT_RipOptions, PROPTEXT_RipOptions_AssetRipTasker, newtoolPath);
}

void MOG_Properties::set_AssetRipper(String *value)	
{
// We can't perform this check because Rippers can include arguments that can handle invalid chars
//	// Check for invalid characters
//	if (MOG_ControllerSystem::InvalidMOGCharactersCheck(value, true))
//	{
//		return;
//	}

	// Check if this is a Library asset?
	if (IsLibrary)
	{
		// Check if they are actually attempting to set it to something?
		if (value->Length)
		{
			//Library assets need to check files out from the Files.Imported directory
			//In order to do that they must always be native
			//This is a quick fix to get the library working until we actually redesign the whole thing - JWW
			// Inform the user they entered an invalid user
			MOGPromptResult result = MOG_Prompt::PromptResponse(	S"Set Property Failed",
																	String::Concat(	S"No support is available for assigning rippers to Library Assets.\n\n",
																					S"Rippers can only be assigned to non-Library Assets."),
																	NULL, 
																	MOGPromptButtons::OK, MOG_ALERT_LEVEL::ALERT);
			value = S"";
		}
	}

	// Always make sure we bust this down to the relative path within the tools dir
	String* newtoolPath = MOG_ControllerSystem::InternalizeTool(value, S"Rippers");
	SetProperty(PROPTEXT_Properties, PROPTEXT_RipOptions, PROPTEXT_RipOptions_AssetRipper, newtoolPath);
}

void MOG_Properties::set_ValidSlaves(String *value)
{
	// Check for invalid characters
	if (MOG_ControllerSystem::InvalidWindowsFilenameCharactersCheck(value, true))
	{
		return;
	}

	SetProperty(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_ValidSlaves, value);
}

void MOG_Properties::set_Description(String *value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_AssetInfo, PROPTEXT_AssetInfo_Description, value);
}

void MOG_Properties::set_Group(String *value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_AssetInfo, PROPTEXT_AssetInfo_Group, value);
}

void MOG_Properties::set_IsPackagedAsset(Boolean value)
{
	// Check if they want to say it isn't a PackagedAsset?
	if (value == false)
	{
		// Check if this asset has a package assignment?
		ArrayList* list = GetPackages();
		if (list && list->Count > 0)
		{
			// Inform them that it can't be changed because the asset is assigned to a package.
			MOGPromptResult result = MOG_Prompt::PromptResponse(	S"Set Property",
																	String::Concat(	S"This property can't be turned off because the asset has an active Package Assignment.\n",
																					S"You must first remove all package assignments before this property can be shut off?\n",
																					S"ASSETNAME: ", mAssetFilename->GetAssetFullName()),
																	NULL, 
																	MOGPromptButtons::OK, MOG_ALERT_LEVEL::ALERT);
			return;
		}
	}

	SetProperty(PROPTEXT_Properties, PROPTEXT_AssetInfo, PROPTEXT_AssetInfo_IsPackagedAsset, value.ToString());
}

void MOG_Properties::set_IsPackagedAsset_InheritedBoolean(MOG_InheritedBoolean value)
{
	// Check if they want to say it isn't a PackagedAsset?
	if (value == false)
	{
		// Check if this asset has a package assignment?
		ArrayList* list = GetPackages();
		if (list && list->Count > 0)
		{
			// Inform them that it can't be changed because the asset is assigned to a package.
			MOGPromptResult result = MOG_Prompt::PromptResponse(	S"Set Property",
																	String::Concat(	S"This property can't be turned off because the asset has an active Package Assignment.\n",
																					S"You must first remove all package assignments before this property can be shut off?\n",
																					S"ASSETNAME: ", mAssetFilename->GetAssetFullName()),
																	NULL, 
																	MOGPromptButtons::OK, MOG_ALERT_LEVEL::ALERT);
			return;
		}
	}

	SetProperty(PROPTEXT_Properties, PROPTEXT_AssetInfo, PROPTEXT_AssetInfo_IsPackagedAsset, MapInheritedBoolean(value));
}

void MOG_Properties::set_DivergentPlatformDataType(Boolean value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_RipOptions, PROPTEXT_RipOptions_DivergentPlatformDataType, value.ToString());
}

void MOG_Properties::set_DivergentPlatformDataType_InheritedBoolean(MOG_InheritedBoolean value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_RipOptions, PROPTEXT_RipOptions_DivergentPlatformDataType, MapInheritedBoolean(value));
}

void MOG_Properties::set_DefaultAssetNameIncludeExtension(MOG_DefaultPrompt value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_DefaultAssetNameIncludeExtension, MapDefaultPrompt(value));
}

void MOG_Properties::set_DefaultAssetNamePlatform(String* value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_DefaultAssetNamePlatform, value);
}

void MOG_Properties::set_UnBlessable(Boolean value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_UnBlessable, value.ToString());
}

void MOG_Properties::set_UnBlessable_InheritedBoolean(MOG_InheritedBoolean value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_UnBlessable, MapInheritedBoolean(value));
}

void MOG_Properties::set_UniquePackageAssignmentVerification(Boolean value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_UniquePackageAssignmentVerification, value.ToString());
}

void MOG_Properties::set_UniquePackageAssignmentVerification_InheritedBoolean(MOG_InheritedBoolean value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_UniquePackageAssignmentVerification, MapInheritedBoolean(value));
}

void MOG_Properties::set_OutofdateVerification(Boolean value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_OutofdateVerification, value.ToString());
}

void MOG_Properties::set_OutofdateVerification_InheritedBoolean(MOG_InheritedBoolean value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_OutofdateVerification, MapInheritedBoolean(value));
}

void MOG_Properties::set_LocalVerificationBeforeBless(Boolean value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_LocalVerificationBeforeBless, value.ToString());
}

void MOG_Properties::set_LocalVerificationBeforeBless_InheritedBoolean(MOG_InheritedBoolean value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_LocalVerificationBeforeBless, MapInheritedBoolean(value));
}

void MOG_Properties::set_UnReferencedRevisionHistory(String *value)	
{
	try
	{
		Int64 testInt = Int64::Parse( value );
		SetProperty(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_UnReferencedRevisionHistory, value); 
	}
	catch(...)
	{
	}
}

void MOG_Properties::set_BlessTarget(String *value)	
{
	// Check if they specified anything?
	if (value->Length)
	{
		// Make sure this is a valid user?
		if (!MOG_ControllerProject::IsValidUser(value))
		{
			// Inform the user they entered an invalid user
			MOGPromptResult result = MOG_Prompt::PromptResponse(	S"Set Property",
																	String::Concat(	S"'", value, S"' is not a valid user in the project.\n",
																					S"The Property was not set."),
																	NULL, 
																	MOGPromptButtons::OK, MOG_ALERT_LEVEL::ALERT);
			// Don't set the property
			return;
		}
	}

	SetProperty(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_BlessTarget, value); 
}

void MOG_Properties::set_BlessEmailNotify(String *value)			
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_BlessEmailNotify, value);
}

void MOG_Properties::set_ShowRipCommandWindow(Boolean value)			
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_RipOptions, PROPTEXT_RipOptions_ShowRipCommandWindow, value.ToString());
}

void MOG_Properties::set_ShowRipCommandWindow_InheritedBoolean(MOG_InheritedBoolean value)			
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_RipOptions, PROPTEXT_RipOptions_ShowRipCommandWindow, MapInheritedBoolean(value));
}

void MOG_Properties::set_UseTempRipDir(Boolean value)			
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_RipOptions, PROPTEXT_RipOptions_UseTempRipDir, value.ToString());
}

void MOG_Properties::set_UseTempRipDir_InheritedBoolean(MOG_InheritedBoolean value)			
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_RipOptions, PROPTEXT_RipOptions_UseTempRipDir, MapInheritedBoolean(value));
}

void MOG_Properties::set_UseLocalTempRipDir(Boolean value)			
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_RipOptions, PROPTEXT_RipOptions_UseLocalTempRipDir, value.ToString());
}

void MOG_Properties::set_UseLocalTempRipDir_InheritedBoolean(MOG_InheritedBoolean value)			
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_RipOptions, PROPTEXT_RipOptions_UseLocalTempRipDir, MapInheritedBoolean(value));
}

void MOG_Properties::set_CopyFilesIntoTempRipDir(Boolean value)			
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_RipOptions, PROPTEXT_RipOptions_CopyFilesIntoTempRipDir, value.ToString());
}

void MOG_Properties::set_CopyFilesIntoTempRipDir_InheritedBoolean(MOG_InheritedBoolean value)			
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_RipOptions, PROPTEXT_RipOptions_CopyFilesIntoTempRipDir, MapInheritedBoolean(value));
}

void MOG_Properties::set_AutoDetectRippedFiles(Boolean value)			
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_RipOptions, PROPTEXT_RipOptions_AutoDetectRippedFiles, value.ToString());
}

void MOG_Properties::set_AutoDetectRippedFiles_InheritedBoolean(MOG_InheritedBoolean value)			
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_RipOptions, PROPTEXT_RipOptions_AutoDetectRippedFiles, MapInheritedBoolean(value));
}

void MOG_Properties::set_IsBuild(Boolean value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_AssetInfo, PROPTEXT_AssetInfo_IsBuild, value.ToString());
}

void MOG_Properties::set_IsBuild_InheritedBoolean(MOG_InheritedBoolean value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_AssetInfo, PROPTEXT_AssetInfo_IsBuild, MapInheritedBoolean(value));
}

void MOG_Properties::set_ShowBuildCommandWindow(Boolean value)			
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_BuildOptions, PROPTEXT_BuildOptions_ShowBuildCommandWindow, value.ToString());
}

void MOG_Properties::set_ShowBuildCommandWindow_InheritedBoolean(MOG_InheritedBoolean value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_BuildOptions, PROPTEXT_BuildOptions_ShowBuildCommandWindow, MapInheritedBoolean(value));
}

void MOG_Properties::set_BuildTool(String *value)
{
	// Check for invalid characters
	if (MOG_ControllerSystem::InvalidWindowsPathCharactersCheck(value, true))
	{
		return;
	}

	// Always make sure we bust this down to the relative path within the tools dir
	String* newtoolPath = MOG_ControllerSystem::InternalizeTool(value, S"BuildScripts");
	SetProperty(PROPTEXT_Properties, PROPTEXT_BuildOptions, PROPTEXT_BuildOptions_BuildTool, newtoolPath);
}

void MOG_Properties::set_BuildWorkingDirectory(String *value)
{
	// Check for invalid characters
	if (MOG_ControllerSystem::InvalidWindowsPathCharactersCheck(value, true))
	{
		return;
	}

	SetProperty(PROPTEXT_Properties, PROPTEXT_BuildOptions, PROPTEXT_BuildOptions_BuildWorkingDirectory, value);
}

void MOG_Properties::set_DefaultPackageFileExtension(String *value)			
{
	// Trim off any '.' that the user may have included
	String *extension = value->Trim(S"."->ToCharArray());
	SetProperty(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_DefaultPackageFileExtension, extension);
}

void MOG_Properties::set_ShowPackageCommandWindow(Boolean value)			
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_ShowPackageCommandWindow, value.ToString());
}

void MOG_Properties::set_ShowPackageCommandWindow_InheritedBoolean(MOG_InheritedBoolean value)			
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_ShowPackageCommandWindow, MapInheritedBoolean(value));
}

void MOG_Properties::set_PackageStyle(MOG_PackageStyle value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_PackageStyle, MapPackageStyle(value));
}

void MOG_Properties::set_IsPackage(Boolean value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_AssetInfo, PROPTEXT_AssetInfo_IsPackage, value.ToString());
}

void MOG_Properties::set_IsPackage_InheritedBoolean(MOG_InheritedBoolean value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_AssetInfo, PROPTEXT_AssetInfo_IsPackage, MapInheritedBoolean(value));
}

void MOG_Properties::set_AutoPackage(Boolean value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_AutoPackage, value.ToString());
}

void MOG_Properties::set_AutoPackage_InheritedBoolean(MOG_InheritedBoolean value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_AutoPackage, MapInheritedBoolean(value));
}

void MOG_Properties::set_ExecuteNetworkPackageMerge(Boolean value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_ExecuteNetworkPackageMerge, value.ToString());
}

void MOG_Properties::set_ExecuteNetworkPackageMerge_InheritedBoolean(MOG_InheritedBoolean value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_ExecuteNetworkPackageMerge, MapInheritedBoolean(value));
}

void MOG_Properties::set_ClusterPackaging(Boolean value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_ClusterPackaging, value.ToString());
}

void MOG_Properties::set_ClusterPackaging_InheritedBoolean(MOG_InheritedBoolean value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_ClusterPackaging, MapInheritedBoolean(value));
}

void MOG_Properties::set_PackagePreMergeEvent(String *value)
{
	// Always make sure we bust this down to the relative path within the tools dir
	String* newtoolPath = MOG_ControllerSystem::InternalizeTool(value, S"PackageScripts");
	SetProperty(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_PackagePreMergeEvent, newtoolPath);
}

void MOG_Properties::set_PackagePostMergeEvent(String *value)
{
	// Always make sure we bust this down to the relative path within the tools dir
	String* newtoolPath = MOG_ControllerSystem::InternalizeTool(value, S"PackageScripts");
	SetProperty(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_PackagePostMergeEvent, newtoolPath);
}

void MOG_Properties::set_TaskFileTool(String *value)
{
	// Check for invalid characters
	if (MOG_ControllerSystem::InvalidWindowsPathCharactersCheck(value, true))
	{
		return;
	}

	// Always make sure we bust this down to the relative path within the tools dir
	String* newtoolPath = MOG_ControllerSystem::InternalizeTool(value, S"PackageScripts");
	SetProperty(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_TaskFileTool, newtoolPath);
}

void MOG_Properties::set_InputPackageTaskFile(String *value)
{
	// Check for invalid characters
	if (MOG_ControllerSystem::InvalidWindowsPathCharactersCheck(value, true))
	{
		return;
	}

	SetProperty(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_InputPackageTaskFile, value);
}

void MOG_Properties::set_OutputPackageTaskFile(String *value)
{
	// Check for invalid characters
	if (MOG_ControllerSystem::InvalidWindowsPathCharactersCheck(value, true))
	{
		return;
	}

	SetProperty(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_OutputPackageTaskFile, value);
}

void MOG_Properties::set_PackageWorkspaceDirectory(String *value)
{
	// Check for invalid characters
	if (MOG_ControllerSystem::InvalidWindowsPathCharactersCheck(value, true))
	{
		return;
	}

	SetProperty(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_PackageWorkspaceDirectory, value);
}

void MOG_Properties::set_PackageDataDirectory(String *value)
{
	// Check for invalid characters
	if (MOG_ControllerSystem::InvalidWindowsPathCharactersCheck(value, true))
	{
		return;
	}

	SetProperty(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_PackageDataDirectory, value);
}

void MOG_Properties::set_SyncPackageWorkspaceDirectory(Boolean value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_SyncPackageWorkspaceDirectory, value.ToString());
}

void MOG_Properties::set_SyncPackageWorkspaceDirectory_InheritedBoolean(MOG_InheritedBoolean value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_SyncPackageWorkspaceDirectory, MapInheritedBoolean(value));
}

void MOG_Properties::set_CleanupPackageWorkspaceDirectory(Boolean value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_CleanupPackageWorkspaceDirectory, value.ToString());
}

void MOG_Properties::set_CleanupPackageWorkspaceDirectory_InheritedBoolean(MOG_InheritedBoolean value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_CleanupPackageWorkspaceDirectory, MapInheritedBoolean(value));
}

void MOG_Properties::set_PackageCommand_Propagation(MOG_PackageCommandPropagation value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_PackageCommands, PROPTEXT_PackageCommands_PackageCommand_Propagation, MapPackageCommandPropagation(value));
}

void MOG_Properties::set_PackageCommand_Add(String *value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_PackageCommands, PROPTEXT_PackageCommands_PackageCommand_Add, value);
}

void MOG_Properties::set_PackageCommand_Remove(String *value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_PackageCommands, PROPTEXT_PackageCommands_PackageCommand_Remove, value);
}

void MOG_Properties::set_PackageCommand_DeletePackageFile(String *value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_PackageCommand_DeletePackageFile, value);
}

void MOG_Properties::set_PackageCommand_ResolveLateResolvers(String *value)
{
	SetProperty(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_PackageCommand_ResolveLateResolvers, value);
}

void MOG_Properties::set_PropertyMenu(String *value)
{
	// Check for invalid characters
	if (MOG_ControllerSystem::InvalidWindowsPathCharactersCheck(value, true))
	{
		return;
	}

	// Always make sure we bust this down to the relative path within the tools dir
	String* newtoolPath = MOG_ControllerSystem::ResolveToolRelativePath(value);
	if (Path::IsPathRooted(newtoolPath))
	{
		value = String::Concat(S"\"", value, S"\"");
		newtoolPath = MOG_ControllerSystem::InternalizeTool(value, "Property.Menus");
	}
	SetProperty(PROPTEXT_Properties, PROPTEXT_AssetInfo, PROPTEXT_AssetInfo_PropertyMenu, newtoolPath); 
}

// Return only PropertyDescriptors that reflect what Attributes the programmer wants
PropertyDescriptorCollection *MOG_Properties::GetProperties(Attribute *attributes[])
{
	// Get the collection of properties
	PropertyDescriptorCollection *baseProps = NULL;
	if (attributes)
	{
		baseProps = TypeDescriptor::GetProperties(this, attributes, true);
	}
	else
	{
		baseProps = TypeDescriptor::GetProperties(this, true);
	}

	// Get an empty base collection to add all our MOG_PropertyDescriptors to
	PropertyDescriptorCollection *mogProps = new PropertyDescriptorCollection( NULL );

	// For each property use a property descriptor of our own that is able to be globalized
	for( int i = 0; i < baseProps->Count; ++i )
	{
		PropertyDescriptor* desc = baseProps->Item[i];

		bool bInherited = true;
		
		String* displayName = desc->DisplayName->Replace(S"_InheritedBoolean", S"");

		if (desc->GetValue(this))
		{
			if (IsPropertyNotInherited(PROPTEXT_Properties, desc->Category, displayName, desc->GetValue(this)->ToString()))
			{
				bInherited = false;
			}
		}
		else
		{
			bInherited = false;
		}

		mogProps->Add(new MOG_PropertyDescriptor(desc, bInherited, !mCanModify));
	}

	return mogProps;
}

//
PropertyDescriptorCollection *MOG_Properties::GetProperties( void )
{
	return GetProperties(NULL);
}


bool MOG_Properties::FixupInheritedPropertiesForAffectedAssets(String *classification, MOG_Property *propertyToUseWithNewValue)
{
	ArrayList *listOfAssetsToRevision = new ArrayList();
	MOG_Properties *rootProperties = new MOG_Properties(classification);
	MOG_Property *currentProperty = rootProperties->GetProperty(propertyToUseWithNewValue->mSection, propertyToUseWithNewValue->mPropertySection, propertyToUseWithNewValue->mPropertyKey);
	if (currentProperty)
	{
		// Get the contained assets affected by this change
		listOfAssetsToRevision = GetListOfAffectedAssets(classification, currentProperty);
		if (listOfAssetsToRevision->Count)
		{
			// Warning user about the scope of this change
			if (MOG_Prompt::PromptResponse(	String::Concat(S"Change ", propertyToUseWithNewValue->mPropertyKey, S"?"),
											String::Concat(propertyToUseWithNewValue->mPropertyKey ,S"=", propertyToUseWithNewValue->mPropertyValue, S"\n",
															S"There are (", Convert::ToString(listOfAssetsToRevision->Count), S") contained assets effected by this change.\n",
															S"Would you like to restamp these assets now (Strongly Recommended)?"), MOG::PROMPT::MOGPromptButtons::YesNo) == MOGPromptResult::Yes)
			{
				// Establish the new timestamp that all these reinstances will share
				String *newTimeStamp = MOG_Time::GetVersionTimestamp();
				String *jobLabel = String::Concat(S"Reinstance.", MOG_ControllerSystem::GetComputerName(), S".", newTimeStamp);

				// Set the new property
				MOG_Properties *props = MOG_Properties::OpenClassificationProperties(classification);
				props->SetProperty(propertyToUseWithNewValue->mSection,propertyToUseWithNewValue->mPropertySection,propertyToUseWithNewValue->mPropertyKey,propertyToUseWithNewValue->mPropertyValue);
				props->Close();

				// Iterate through all the effected assets performing the reinstance command
				List<Object*>* args = new List<Object*>();
				args->Add(listOfAssetsToRevision);
				args->Add(newTimeStamp);
				ArrayList* addProperties = new ArrayList();
				addProperties->Add(propertyToUseWithNewValue);
				args->Add(addProperties);

				ProgressDialog* progress = new ProgressDialog(S"Processing Assets", NULL, new DoWorkEventHandler(this, &MOG_Properties::FixupInheritedPropertiesForAffectedAssets_Worker), args, true);
				progress->ShowDialog();

				MOG_Prompt::PromptResponse(	String::Concat(S"Change ", propertyToUseWithNewValue->mPropertyKey),
											String::Concat(	S"This change requires Slave processing.\n",
															S"The project will not reflect these changes until all slaves have finished processing the generated commands.\n",
															S"The progress of this task can be monitored in the Connections Tab."));
				return true;
			}
		}
	}

	return false;
}

void MOG_Properties::FixupInheritedPropertiesForAffectedAssets_Worker(Object* sender, DoWorkEventArgs* e)
{
	BackgroundWorker* worker = dynamic_cast<BackgroundWorker*>(sender);
	List<Object*>* args = dynamic_cast<List<Object*>*>(e->Argument);
	ArrayList* listOfAssetsToRevision = dynamic_cast<ArrayList*>(args->Item[0]);
	String* newTimeStamp = dynamic_cast<String*>(args->Item[1]);
	ArrayList* addProperties = dynamic_cast<ArrayList*>(args->Item[2]);
	
	for (int i = 0 ; i < listOfAssetsToRevision->Count ; i++)
	{
		String *message = String::Concat(S"Processing Asset ", Convert::ToString(i), S" of ", Convert::ToString(listOfAssetsToRevision->Count));

		if (worker != NULL)
		{
			worker->ReportProgress(i * 100 / listOfAssetsToRevision->Count, message);
		}

		MOG_DBAssetProperties *currentAsset =  __try_cast <MOG_DBAssetProperties*> (listOfAssetsToRevision->Item[i]);
		MOG_Filename *oldBlessedAssetFilename = MOG_ControllerRepository::GetAssetBlessedVersionPath(currentAsset->mAsset, currentAsset->mAsset->GetVersionTimeStamp());
		MOG_ControllerReinstance *rController = new MOG_ControllerReinstance(oldBlessedAssetFilename, new ArrayList(), addProperties, newTimeStamp);
		rController->ReinstanceAllAssets();
	}
}

ArrayList* MOG_Properties::GetListOfAffectedAssets( String *classification, MOG_Property *currentProperty )
{
	// Create an empty property that will use a wild card selection to return all assets that have this particular property, disregarding the value.
	MOG_Property * emptyProperty = new MOG_Property(currentProperty->mSection, currentProperty->mPropertySection, currentProperty->mPropertyKey, "");
	emptyProperty->mValue = "*";
	ArrayList *listOfClassificationsWithInheritedProperty = MOG_DBReports::ClassificationForPropertyNoJumpingClassifications(classification, currentProperty, false);
	ArrayList * listOfAssetsWithInheritedProperty = MOG_DBAssetAPI::GetAllAssetsByParentClassificationWithProperties_ThatInheritProperty(classification,emptyProperty);
	ArrayList *listOfAssetsToRevision = new ArrayList();
	for(int i = 0 ; i < listOfAssetsWithInheritedProperty->Count ; i++)
	{
		bool revisionAsset = false;
		MOG_DBAssetProperties *currentAsset =  __try_cast <MOG_DBAssetProperties*> (listOfAssetsWithInheritedProperty->Item[i]);
		for(int ii = 0; ii < listOfClassificationsWithInheritedProperty->Count; ii++)
		{
			if(String::Compare(currentAsset->mAsset->GetAssetClassification(),  __try_cast <String*> (listOfClassificationsWithInheritedProperty->Item[ii]),true) == 0)
			{
				revisionAsset = true;
				break;
			}
		}
		if(revisionAsset)
		{
			listOfAssetsToRevision->Add(listOfAssetsWithInheritedProperty->Item[i]);
		}
	}
	return listOfAssetsToRevision;
}

void MOG_Properties::ActivatePropertiesCache(bool value)
{
	gUsePropertiesCache = value;

	// Check if we are deactivating the cache?
	if (!gUsePropertiesCache)
	{
		// Flush the properties cache
		gLastClassification = S"";
		gLastInheritedProperties = NULL;
		gPropertiesCache = new HybridDictionary();
	}
}
