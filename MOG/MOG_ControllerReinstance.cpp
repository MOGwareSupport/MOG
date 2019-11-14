//--------------------------------------------------------------------------------
//	MOG_ControllerReinstance.cpp
//	
//	
//--------------------------------------------------------------------------------

#include "StdAfx.h"

#include "MOG_Project.h"
#include "MOG_ControllerPackage.h"
#include "MOG_ControllerRepository.h"
#include "MOG_ControllerSystem.h"
#include "MOG_DBAssetAPI.h"
#include "MOG_DBQueryBuilderAPI.h"
#include "MOG_Properties.h"
#include "MOG_Filename.h"
#include "MOG_CommandFactory.h"
#include "MOG_ControllerProject.h"

#include "MOG_ControllerReinstance.h"


using namespace System::Collections::Generic;


///////////////////////////////////////////////////////////
//BEGIN ReinstanceAsset Class
///////////////////////////////////////////////////////
MOG_AssetReinstanceInfo::MOG_AssetReinstanceInfo(MOG_Filename *oldAsset, MOG_Filename *newAsset, ArrayList *listOfPropertiesToRemove, ArrayList * listOfPropertiesToAdd, bool bRemoveEmptyClassifications)
{
	//Initilize our Reinstance asset object with the passed values
	InitilizeReinstanceAsset(oldAsset, newAsset, listOfPropertiesToRemove, listOfPropertiesToAdd);
	
	// Always assume we want to remove empty classifications
	mRemoveEmptyClassifications = bRemoveEmptyClassifications;
}




void MOG_AssetReinstanceInfo::InitilizeReinstanceAsset(MOG_Filename *oldAsset, MOG_Filename *newAsset, ArrayList *listOfPropertiesToRemove, ArrayList * listOfPropertiesToAdd)
{
	//initilize all variables.

	// Check if there was an old asset specified?
	if(oldAsset)
	{
		mOldAssetFilename = oldAsset;

		// Check if this was an asset?
		if (mOldAssetFilename->GetFilenameType() == MOG_FILENAME_TYPE::MOG_FILENAME_Asset)
		{
		}
	}
	else
	{
		mOldAssetFilename = NULL;
	}

	// Check if there was a new asset specified?
	if(newAsset)
	{
		mNewAssetFilename = newAsset;

		// Check if this was an asset?
		if (mNewAssetFilename->GetFilenameType() == MOG_FILENAME_TYPE::MOG_FILENAME_Asset)
		{
		}
	}
	else
	{
		mOldAssetFilename = NULL;
	}

	// Check if there were properties specified for removal?
	if(listOfPropertiesToRemove)
	{
		mPropertiesToRemove = listOfPropertiesToRemove;
	}
	else
	{
		mPropertiesToRemove = new ArrayList();
	}

	// Check if there were properties specified for addition?
	if(listOfPropertiesToAdd)
	{
		mPropertiesToAdd = listOfPropertiesToAdd;
	}
	else
	{
		mPropertiesToAdd = new ArrayList();
	}
}

////////////////////////////////////////////////////////////////////////////////////////
//End of MOG_AssetReinstanceInfo Class
///////////////////////////////////////////////////////////////////////////////////////





///////////////////////////////////////////////////////////
//BEGIN MOG_ReinstanceAssetList Class
///////////////////////////////////////////////////////

void MOG_ReinstanceAssetList::Add(MOG_AssetReinstanceInfo *reinstanceAsset)
{
	// Make sure we have a valid object?
	if (reinstanceAsset)
	{
		// Scan the list to make sure it isn't already listed?
		for (int i = 0; i < Count; i++)
		{
			MOG_AssetReinstanceInfo *testReinstanceAsset = __try_cast<MOG_AssetReinstanceInfo *>(Item[i]);
			// Check if this matches?
			if (String::Compare(testReinstanceAsset->mNewAssetFilename->GetAssetFullName(), reinstanceAsset->mNewAssetFilename->GetAssetFullName(), true) == 0)
			{
				// Bail out now because this item appears to be already listed
				return;
			}
		}

		// Add this new item
		ArrayList::Add(reinstanceAsset);
	}
}


//used to find out of a given asset is in the list of assets to revision
MOG_AssetReinstanceInfo *MOG_ReinstanceAssetList::FindByAssetFilename(MOG_Filename *assetFilename)
{
	//loop though all the assets in the reinstance asset list
	for(int i = 0; i < Count; i++)
	{
		MOG_AssetReinstanceInfo *currentReinstanceAsset = __try_cast <MOG_AssetReinstanceInfo*>	(Item[i]);

		// Check if this asset's name matches?
		if (String::Compare(currentReinstanceAsset->mOldAssetFilename->GetAssetFullName(), assetFilename->GetAssetFullName(), true) == 0)
		{
			return currentReinstanceAsset;
		}
	}

	return NULL;
}

//used to find out of any assets contain the given classification
bool MOG_ReinstanceAssetList::IsClassificationListed(String *classification)
{
	//loop though all the assets in the reinstance asset list
	for(int i = 0; i < Count; i++)
	{
		MOG_AssetReinstanceInfo *currentReinstanceAsset = __try_cast <MOG_AssetReinstanceInfo*>	(Item[i]);

		// Check if this asset's classification is a child of?
		if (MOG_Filename::IsParentClassificationString(currentReinstanceAsset->mOldAssetFilename->GetAssetClassification(), classification))
		{
			return true;
		}
	}

	return false;
}

////////////////////////////////////////////////////////////////////////////////////////
//End of MOG_ReinstanceAssetList Class
///////////////////////////////////////////////////////////////////////////////////////





//This is the initilizer that allows us to reinstance a single asset.
MOG_ControllerReinstance::MOG_ControllerReinstance(MOG_Filename *oldAssetName, MOG_Filename *newAssetName)
{
	List<MOG_Filename*>* args = new List<MOG_Filename*>();
	args->Add(oldAssetName);
	args->Add(newAssetName);

	ProgressDialog* progress = new ProgressDialog(S"Analyzing effected assets", S"Please wait while MOG is analyzing the request.", new DoWorkEventHandler(this, &MOG_ControllerReinstance::Constructor1_Worker), args, false);
	progress->ShowDialog();
}

void MOG_ControllerReinstance::Constructor1_Worker(Object* sender, DoWorkEventArgs* e)
{
	List<MOG_Filename*>* args = dynamic_cast<List<MOG_Filename*>*>(e->Argument);
	MOG_Filename* oldAssetName = args->Item[0];
	MOG_Filename* newAssetName = args->Item[1];

	ArrayList* assetsToReinstance = new ArrayList();

	// Build the generic 'All' asset filename
	MOG_Filename* genericAssetFilename = MOG_Filename::CreateAssetName(oldAssetName->GetAssetClassification(), S"All", oldAssetName->GetAssetLabel());

	// Get all the platforms in the project
	String* platforms[] = MOG_ControllerProject::GetAllApplicablePlaformsForAsset(genericAssetFilename->GetAssetFullName(), false);
	// Loop through all the applicable platforms looking for a matching game data file
	for (int p = 0; p < platforms->Count; p++)
	{
		// Build the platform-specific asset filename fo rthis platform
		MOG_Filename* platformAssetFilename = MOG_Filename::CreateAssetName(oldAssetName->GetAssetClassification(), platforms[p], oldAssetName->GetAssetLabel());
		// Check if this asset exists in the project?
		if (MOG_ControllerProject::DoesAssetExists(platformAssetFilename))
		{
			// Add this asset for reinstancing
			assetsToReinstance->Add(MOG_ControllerProject::GetAssetCurrentBlessedVersionPath(platformAssetFilename));
		}
	}
	// Check if this asset exists in the project?
	if (MOG_ControllerProject::DoesAssetExists(genericAssetFilename))
	{
		// Always add the generic asset last because it's behavhior relies heavily on the existance of platform specific assets
		assetsToReinstance->Add(MOG_ControllerProject::GetAssetCurrentBlessedVersionPath(genericAssetFilename));
	}

	//Initilize the variables to recieve data.
	InitilizeControllerReinstance();

	// Loop through all the identified assets
	for (int a = 0; a < assetsToReinstance->Count; a++)
	{
		MOG_Filename* existingAssetFilename = dynamic_cast<MOG_Filename*>(assetsToReinstance->Item[a]);

		// Create our new asset filename using the newAssetName in conjunction with this asset's platform
		MOG_Filename* newAssetFilename = MOG_Filename::CreateAssetName(newAssetName->GetAssetClassification(), existingAssetFilename->GetAssetPlatform(), newAssetName->GetAssetLabel());;
		newAssetFilename = MOG_ControllerRepository::GetAssetBlessedVersionPath(newAssetFilename, existingAssetFilename->GetVersionTimeStamp());

		//Add these values to the variables initilized above.
		mOldName = existingAssetFilename->GetAssetFullName();
		mNewName = newAssetFilename->GetAssetFullName();

		//create a single reinstance asset and populate it.
		MOG_AssetReinstanceInfo * reinstanceAsset = new MOG_AssetReinstanceInfo(existingAssetFilename, newAssetFilename, NULL, NULL, GetClassificationsToReinstanceCount() > 0);
		//Add the new reinstance asset to the reinstance list.
		mAssetsToReinstanceList->Add(reinstanceAsset);

		// Initialize any dependant relationships
		InitializeDependantClassifications();
		InitializeDependantAssets();

		//if we have dependant assets fix them up.
		FixupAssetRelationshipOfDependantAssets();
	}
}


//This is the initilizer that allows us to reinstance a single asset.
MOG_ControllerReinstance::MOG_ControllerReinstance(MOG_Filename *existingAssetFilename, ArrayList *removeProperties, ArrayList *addProperties)
{
	// Construct a new timestamp
	String *timestamp = MOG_Time::GetVersionTimestamp();

	List<Object*>* args = new List<Object*>();
	args->Add(existingAssetFilename);
	args->Add(removeProperties);
	args->Add(addProperties);
	args->Add(timestamp);

	ProgressDialog* progress = new ProgressDialog(S"Analyzing effected assets", S"Please wait while MOG is analyzing the request.", new DoWorkEventHandler(this, &MOG_ControllerReinstance::Constructor2_Worker), args, false);
	progress->ShowDialog();
}

//This is the initilizer that allows us to reinstance a single asset.
MOG_ControllerReinstance::MOG_ControllerReinstance(MOG_Filename *existingAssetFilename, ArrayList *removeProperties, ArrayList *addProperties, String* timestamp)
{
	List<Object*>* args = new List<Object*>();
	args->Add(existingAssetFilename);
	args->Add(removeProperties);
	args->Add(addProperties);
	args->Add(timestamp);

	ProgressDialog* progress = new ProgressDialog(S"Analyzing effected assets", S"Please wait while MOG is analyzing the request.", new DoWorkEventHandler(this, &MOG_ControllerReinstance::Constructor2_Worker), args, false);
	progress->ShowDialog();
}

void MOG_ControllerReinstance::Constructor2_Worker(Object* sender, DoWorkEventArgs* e)
{
	List<Object*>* args = dynamic_cast<List<Object*>*>(e->Argument);
	MOG_Filename* existingAssetFilename = dynamic_cast<MOG_Filename*>(args->Item[0]);
	ArrayList* removeProperties = dynamic_cast<ArrayList*>(args->Item[1]);
	ArrayList* addProperties = dynamic_cast<ArrayList*>(args->Item[2]);
	String* timestamp = dynamic_cast<String*>(args->Item[3]);

	//Initilize the variables to recieve data.
	InitilizeControllerReinstance(timestamp);

	// Create a new revision for the specified asset
	MOG_Filename *newAssetFilename = MOG_ControllerRepository::GetAssetBlessedVersionPath(existingAssetFilename, mNewTimestamp);

	//Add these values to the variables initilized above.
	mOldName = existingAssetFilename->GetAssetFullName();
	mNewName = newAssetFilename->GetAssetFullName();

	//create a single reinstance asset and populate it.
	MOG_AssetReinstanceInfo * reinstanceAsset = new MOG_AssetReinstanceInfo(existingAssetFilename, newAssetFilename, removeProperties, addProperties, GetClassificationsToReinstanceCount() > 0);
	//Add the new reinstance asset to the reinstance list.
	mAssetsToReinstanceList->Add(reinstanceAsset);

	// Initialize any dependant relationships
	InitializeDependantClassifications();
	InitializeDependantAssets();

	//if we have dependant assets fix them up.
	FixupAssetRelationshipOfDependantAssets();
}


//initilizer that allows us to reinstance based on classification (for classification rename.
MOG_ControllerReinstance::MOG_ControllerReinstance(String *oldClassification, String *newClassification)
{
	List<String*>* args = new List<String*>();
	args->Add(oldClassification);
	args->Add(newClassification);

	ProgressDialog* progress = new ProgressDialog(S"Analyzing effected assets", S"Please wait while MOG is analyzing the request.", new DoWorkEventHandler(this, &MOG_ControllerReinstance::Constructor3_Worker), args, false);
	progress->ShowDialog();
}

void MOG_ControllerReinstance::Constructor3_Worker(Object* sender, DoWorkEventArgs* e)
{
	List<String*>* args = dynamic_cast<List<String*>*>(e->Argument);
	String* oldClassification = args->Item[0];
	String* newClassification = args->Item[1];

	//Initilize the Reinstance Controller member variables and set them.
	InitilizeControllerReinstance();
	mOldName = oldClassification;
	mNewName = newClassification;

	// Check the special case for nothing more than a case change
	// Check if the old and new classifications are the same?
	if (String::Compare(oldClassification, newClassification, true) == 0)
	{
		// Since MOG is case insensitive, we really don't need to worry about fixing up the world
		// We can assume this is nothing more then a cosmetic change
		// Simply update the classification in the database and call it good
		MOG_DBAssetAPI::RenameClassificationName(oldClassification, newClassification);
		return;
	}

	// Initialize the initial list of effected assets and classifications
	InitializeReinstanceClassifications();
	InitializeReinstanceAssets();

	// Initialize any dependant relationships
	InitializeDependantClassifications();
	InitializeDependantAssets();

	//if we have dependant assets fix them up.
	FixupAssetRelationshipOfDependantAssets();
}


//This must be called any time we create a Reinstance Controller to actually reinstance the assets.
bool MOG_ControllerReinstance::FixupAssetRelationshipsOfDependantClassifications()
{
	bool bFailed = false;

	// Scan all the dependant classifications
	for( int c = 0; c < mDependantClassificationsList->Count; c++ )
	{
		String *classification = __try_cast <String *> (mDependantClassificationsList->Item[c]);

		// Open the classification's properties
		MOG_Properties *classificationProperties = MOG_Properties::OpenClassificationProperties(classification);
		if (classificationProperties)
		{
			// Get the list of relationships we need to examine to see if they need to be changed
			ArrayList *dependantRelationships = classificationProperties->GetRelationships();
			// Scan the relationships in this classifications
			for (int i = 0; i < dependantRelationships->Count; i++)
			{
				MOG_Property *oldProperty = __try_cast <MOG_Property *> (dependantRelationships->Item[i]);

				// Was this relationship affected?
				if (oldProperty->mPropertyKey->ToLower()->StartsWith(mOldName->ToLower()))
				{
					// We need to keep track of all the parent assets of any relationships
					String *packageName = MOG_ControllerPackage::GetPackageName(oldProperty->mPropertyKey);
					MOG_Filename *newParentAssetFilename = new MOG_Filename(packageName);
					MOG_AssetReinstanceInfo *newParentAssetReinstance = mAssetsToReinstanceList->FindByAssetFilename(newParentAssetFilename);
					mParentAssetsList->Add(newParentAssetReinstance);

					// Setup a new property
					String *newPropertyKey = ChangeClassificationInName(oldProperty->mPropertyKey, mOldName, mNewName);
					MOG_Property *newProperty = new MOG_Property(oldProperty->mSection, oldProperty->mPropertySection, newPropertyKey, oldProperty->mPropertyValue);

					// Remove the old and add the new
					classificationProperties->RemoveProperty(oldProperty->mSection, oldProperty->mPropertySection, oldProperty->mPropertyKey);
					classificationProperties->SetProperty(newProperty->mSection, newProperty->mPropertySection, newProperty->mPropertyKey, newProperty->mPropertyValue);
				}
			}

			// Make sure to close the classification's property when we have finished
			classificationProperties->Close();
		}
	}

	if (!bFailed)
	{
		return true;
	}
	return false;
}


//This must be called any time we create a Reinstance Controller to actually reinstance the assets.
bool MOG_ControllerReinstance::ReinstanceAllAssets()
{
	bool bFailed = false;

	try
	{
//		// Obtain the lock encompassing the transaction
//		String* description = "Automatically locked while assets are reinstanced.";
//		MOG_ControllerProject::PersistentLock_Request(mOldName, description);
//		MOG_ControllerProject::PersistentLock_Request(mNewName, description);

		// Fixup all the dependant classifications
		if (FixupAssetRelationshipsOfDependantClassifications())
		{
			// Create the new classification tree
			if (CreateNewClassificationTree())
			{
				// Ensure that any parent assets have been created
				if (CreateParentAssetPlaceholders())
				{
					ProgressDialog* progress = new ProgressDialog(S"Reinstancing Assets", S"Please wait while MOG reinstances the effected assets.", new DoWorkEventHandler(this, &MOG_ControllerReinstance::ReinstanceAllAssets_Worker), NULL, true);
					progress->ShowDialog();
				}
				else
				{
					bFailed = true;
				}
			}
			else
			{
				bFailed = true;
			}
		}
		else
		{
			bFailed = true;
		}
	}
	catch(Exception *e)
	{
		if(e)
		{
			return false;
		}
	}

	return true;
}

void MOG_ControllerReinstance::ReinstanceAllAssets_Worker(Object* sender, DoWorkEventArgs* e)
{
	BackgroundWorker* worker = dynamic_cast<BackgroundWorker*>(sender);

	//Reinstance all the assets which need to be reinstanced.
	for(int reinstanceAssetIndex = 0; reinstanceAssetIndex < mAssetsToReinstanceList->Count; reinstanceAssetIndex++)
	{
		//get the current asset to reinstance.
		MOG_AssetReinstanceInfo *currentReinstanceAsset = dynamic_cast <MOG_AssetReinstanceInfo*> (mAssetsToReinstanceList->Item[reinstanceAssetIndex]);

		if (worker != NULL)
		{
			worker->ReportProgress(reinstanceAssetIndex * 100 / mAssetsToReinstanceList->Count);
		}

		// Check if we want to attempt to remove any empty classifications when finished?
		String *options = S"";
		if (currentReinstanceAsset->mRemoveEmptyClassifications)
		{
			options = String::Concat(options, S"|RemoveEmptyClassifications|");
		}

		//Build the command to reinstance the asset
		MOG_Command *reinstanceAsset = MOG_CommandFactory::Setup_ReinstanceAssetRevision(currentReinstanceAsset->mNewAssetFilename, currentReinstanceAsset->mOldAssetFilename, mReinstanceLabel, currentReinstanceAsset->mPropertiesToRemove, currentReinstanceAsset->mPropertiesToAdd, options);
		MOG_ControllerSystem::GetCommandManager()->SendToServer(reinstanceAsset);
	}

	// Start the job
	MOG_ControllerProject::StartJob(mReinstanceLabel);
}


//Buids or updates a MOG_AssetReinstanceInfo object for every dependant Asset file and adds it to the list of assets to be revisioned as necessary.
bool MOG_ControllerReinstance::FixupAssetRelationshipOfDependantAssets(void)
{
	// Only bother to fixup the dependant assets in the event the asset's name is actually changing
	if (String::Compare(mOldName, mNewName, true) != 0)
	{
		for(int dependantAssetIndex = 0; dependantAssetIndex < mDependantAssetsList->Count; dependantAssetIndex++)
		{
			//get a temporary variable to hold the current asset
			MOG_Filename *currentDependantAsset = __try_cast <MOG_Filename*>(mDependantAssetsList->Item[dependantAssetIndex]);
			currentDependantAsset = MOG_ControllerRepository::GetAssetBlessedVersionPath(currentDependantAsset, currentDependantAsset->GetVersionTimeStamp());

			//Build a mog properties object so we can get the relationships.
			MOG_Properties *dependantAssetProperties = new MOG_Properties(currentDependantAsset);
			//Get a list of the relationships we need to examine to see if they need to be changed.
			ArrayList *dependantAssetRelationships = dependantAssetProperties->GetPropertyList(PROPTEXT_Relationships);

			//loop through the relationships and see if they need to be changed.
			for(int dependantRelationshipIndex = 0 ; dependantRelationshipIndex < dependantAssetRelationships->Count ; dependantRelationshipIndex++)
			{
				//cast the current relationship in to a temporary variable.
				MOG_Property *currentRelationship = __try_cast <MOG_Property*>(dependantAssetRelationships->Item[dependantRelationshipIndex]);

				//loop through all the current assets that need to be renamed and check to see if we have relationship to it.		
				for(int currentRinstanceAssetIndex = 0; currentRinstanceAssetIndex < mAssetsToReinstanceList->Count; currentRinstanceAssetIndex++)
				{
					//Get the current Renamed asset in a temporary variable so we can compare it.
					MOG_AssetReinstanceInfo *currentReinstanceAsset = __try_cast <MOG_AssetReinstanceInfo*> (mAssetsToReinstanceList->Item[currentRinstanceAssetIndex]);
					
					//Get all the information we need to make a new relationship.
					String *packageName = MOG_ControllerPackage::GetPackageName(currentRelationship->mPropertyKey);
					String *packageGroups = MOG_ControllerPackage::GetPackageGroups(currentRelationship->mPropertyKey);
					String *packageObjects = MOG_ControllerPackage::GetPackageObjects(currentRelationship->mPropertyKey);

					//check to see if we need to create a new relationship for this particular asset
					if(String::Compare(packageName, currentReinstanceAsset->mOldAssetFilename->GetAssetFullName(), true) == 0)
					{
						// We need to keep track of all the parent assets
						mParentAssetsList->Add(currentReinstanceAsset);

						//make the new Package Name
						String *newPackageName = String::Concat(mNewName, packageName->Substring(mOldName->Length ));

						//build a new relationship property with the new package name.
						MOG_Property *newRelationship = MOG_PropertyFactory::MOG_Relationships::New_PackageAssignment(newPackageName, packageGroups, packageObjects);
						MOG_ReinstancePropertyChange *propertyChange = new MOG_ReinstancePropertyChange();
						propertyChange->oldProperty = currentRelationship;
						propertyChange->newProperty = newRelationship;

						// Attempt to get this asset's reinstance info from mAssetsToReinstanceList
						MOG_AssetReinstanceInfo *assetRevisionInfo = mAssetsToReinstanceList->FindByAssetFilename(currentDependantAsset);
						if (!assetRevisionInfo)
						{
							// Create a new MOG_AssetReinstanceInfo
							MOG_Filename *newDependantAssetFilename = MOG_ControllerRepository::GetAssetBlessedVersionPath(currentDependantAsset, mNewTimestamp);
							assetRevisionInfo = new MOG_AssetReinstanceInfo(currentDependantAsset, newDependantAssetFilename, NULL, NULL, GetClassificationsToReinstanceCount() > 0);
							// Add this new asset's reinstance info to the mAssetsToReinstanceList
							mAssetsToReinstanceList->Add(assetRevisionInfo);
						}

						// Record the property change within this asset's reinstance info
						assetRevisionInfo->mPropertiesToAdd->Add(propertyChange->newProperty);
						assetRevisionInfo->mPropertiesToRemove->Add(propertyChange->oldProperty);
					}
				}
			}
		}
	}
	return true;
}


//This is the basic initilzation
void MOG_ControllerReinstance::InitilizeControllerReinstance(void)
{
	InitilizeControllerReinstance(MOG_Time::GetVersionTimestamp());
}

//This is the basic initilzation
void MOG_ControllerReinstance::InitilizeControllerReinstance(String* timestamp)
{
	//initilize member variables to be populated.
	mClassificationsToReinstanceList = new ArrayList();
	mAssetsToReinstanceList = new MOG_ReinstanceAssetList();
	mDependantClassificationsList = new ArrayList();
	mDependantAssetsList = new ArrayList();
	mParentAssetsList = new MOG_ReinstanceAssetList();
	mReinstanceLabel = S"";
	mNewTimestamp = timestamp;
	mReinstanceLabel = String::Concat(S"Reinstance.", MOG_ControllerSystem::GetComputerName(), S".", mNewTimestamp);
}


void MOG_ControllerReinstance::InitializeReinstanceClassifications()
{
	//Get all the childern classifications under currentClassificationName with non inherited properties.
	mClassificationsToReinstanceList = MOG_DBAssetAPI::GetAllActiveClassificationsByRootClassification(mOldName);
}


//This takes the classification and gets a list of all the base affected assets and creates a new asset for reinstance for each of them.
void MOG_ControllerReinstance::InitializeReinstanceAssets()
{
	ArrayList *listOfOldClassificationAssets = MOG_DBAssetAPI::GetAllAssetsInClassificationTree(mOldName);
	for(int oldAssetIndex = 0; oldAssetIndex < listOfOldClassificationAssets->Count; oldAssetIndex++)
	{
		MOG_Filename *oldAssetFilename = __try_cast <MOG_Filename*> (listOfOldClassificationAssets->Item[oldAssetIndex]);
		MOG_Filename *newAssetFilename = new MOG_Filename(ChangeClassificationInName(oldAssetFilename->GetAssetFullName(), mOldName, mNewName));

		//create a new reinstance asset
		MOG_AssetReinstanceInfo *reinstanceAssetSet = new MOG_AssetReinstanceInfo(	MOG_ControllerRepository::GetAssetBlessedVersionPath(oldAssetFilename, oldAssetFilename->GetVersionTimeStamp()),
																					MOG_ControllerRepository::GetAssetBlessedVersionPath(newAssetFilename, mNewTimestamp),
																					NULL,
																					NULL,
																					GetClassificationsToReinstanceCount() > 0);
		//Add this asset to the list of items to reinstance.
		mAssetsToReinstanceList->Add(reinstanceAssetSet);
	}
}


//This function initializes mDependantClassificationsList with any classifications with Relationship Dependancies
bool MOG_ControllerReinstance::InitializeDependantClassifications(void)
{
	// Build a relationship property to send in to the query so we get dependant classifications.
	MOG_Property *relationshipProperty = MOG_PropertyFactory::MOG_Relationships::New_RelationshipAssignment(S"", mOldName, S"", S"");
	// Fixup relationshipProperty to use a wildcard and omit the property's value in the upcomming database query
	relationshipProperty->mKey = String::Concat(mOldName, S"*");
	relationshipProperty->mValue = S"";
	relationshipProperty->mPropertyValue = S"";

	// Scan all classifications that contain a related relationship assignment
	mDependantClassificationsList = MOG_DBAssetAPI::GetAllActiveClassifications( relationshipProperty);

	return true;
}


//This function initializes mDependantAssetsList with any assets with Relationship Dependancies
bool MOG_ControllerReinstance::InitializeDependantAssets(void)
{
	//get a new property to send in to the query so we get the right affected assets.
	MOG_Property *relationshipProperty = new MOG_Property();
	relationshipProperty->mSection = S"Relationships";
	relationshipProperty->mPropertyKey = mOldName;
	//this function gets all the assets which have a relationship with one or more of the assets we are reinstanceing.
	mDependantAssetsList = MOG_DBAssetAPI::QueryAssets(MOG_DBQueryBuilderAPI::MOG_AssetQueries::Query_AllCurrentAssetsWithPropertySet(relationshipProperty));
	return true;

//	//get a new property to send in to the query so we get the right affected assets.
//	MOG_Property *relationshipProperty = MOG_PropertyFactory::MOG_Relationships::New_RelationshipAssignment(S"", mOldName, S"", S"");
//	// Fixup the Property for compatibility with the upcomming 'LIKE' database query
//	relationshipProperty->mKey = mOldName;
//	relationshipProperty->mValue = S"";
//	relationshipProperty->mPropertyValue = S"";
//
//	ArrayList *properties = new ArrayList();
//	properties->Add(relationshipProperty);
//
//	// Build our list of dependant assets
//	mDependantAssetsList = MOG_DBAssetAPI::GetAllAssetsByClassificationAndProperties(S"", properties, true, true);
//	return true;
}


//This is a helper function to build the correct new classification name
String* MOG_ControllerReinstance::ChangeClassificationInName(String* fullName, String* oldClassification, String* newClassification)
{
	//make sure we have both an old and a new classification.
	if(fullName && oldClassification && newClassification && fullName->Length >= oldClassification->Length && newClassification->Length > 0)
	{
		//insert the new classificaiton
		return  String::Concat(newClassification,fullName->Substring(oldClassification->Length));
	}

	return NULL;
}


//This is a helper function which "pre creates" the parent packages so the Reinstanced dependant files have someting to point at.
bool MOG_ControllerReinstance::CreateParentAssetPlaceholders(void)
{
	bool bFailed = false;

	// Itterate through all the parent assets and create a new package asset for them.
	for(int currentParentIndex = 0; currentParentIndex < mParentAssetsList->Count; currentParentIndex++)
	{
		MOG_AssetReinstanceInfo *currentReinstanceAsset = __try_cast <MOG_AssetReinstanceInfo*> (mParentAssetsList->Item[currentParentIndex]);
		MOG_Properties *currentAssetProperties = new MOG_Properties(currentReinstanceAsset->mOldAssetFilename);
		if (!MOG_ControllerProject::CreatePackage(currentReinstanceAsset->mNewAssetFilename, currentAssetProperties->get_SyncTargetPath()))
		{
			bFailed = true;
		}
	}

	if (!bFailed)
	{
		return true;
	}
	return false;
}


//This is a helper function which "creates" the new classification tree and transfers all the assigned properties
bool MOG_ControllerReinstance::CreateNewClassificationTree(void)
{
	bool bFailed = false;

	//Create a set of new classifications to be created by copying the children classifications and replacing the current Root classification with the new classification.
	// Build the new tree of classifications from the old tree
	for(int i = 0; i < mClassificationsToReinstanceList->Count; i++)
	{
		String *currentOldClassificationName = __try_cast <String*>(mClassificationsToReinstanceList->Item[i]);

		//Get the current classifications's non inherited properties
		MOG_Properties *currentClassificationProperties = new MOG_Properties(currentOldClassificationName);
		ArrayList *nonInheritedProperties = currentClassificationProperties->GetNonInheritedProperties();

		//Create the new Classification
		String* addClassificationName = S"";
		if(String::Compare(currentOldClassificationName, mOldName, true) == 0)
		{
			addClassificationName = mNewName;
		}
		else
		{
			addClassificationName = MOG_ControllerReinstance::ChangeClassificationInName( currentOldClassificationName, mOldName, mNewName);
		}

		// Create a set of new classifications based on the renamed classifications.
		// Attempt to create the classification
		if (MOG_DBAssetAPI::CreateClassification(addClassificationName, MOG_ControllerProject::GetUserName_DefaultAdmin(), MOG_Time::GetVersionTimestamp()))
		{
			// Assign non-inherited properties to the new classifications
			MOG_Properties *propertyObjectToAddProperties = MOG_Properties::OpenClassificationProperties(addClassificationName);
			for(int indexPropertySet = 0;indexPropertySet <  nonInheritedProperties->Count;indexPropertySet++ )
			{
				MOG_Property *currentProperty = __try_cast<MOG_Property*>(nonInheritedProperties->Item[indexPropertySet]);
				propertyObjectToAddProperties->SetProperty(currentProperty->mSection,currentProperty->mPropertySection,currentProperty->mPropertyKey,currentProperty->mPropertyValue);
			}
			// Always make sure we close the controller
			propertyObjectToAddProperties->Close();

			// Check to see if this was an empty classification?
			if (!mAssetsToReinstanceList->IsClassificationListed(currentOldClassificationName))
			{
				// Remove this classification now because there are no assets scheduled to be reinstanced
				MOG_DBAssetAPI::RemoveClassification(currentOldClassificationName);
			}
		}
		else
		{
			bFailed = true;
		}
	}

	if (!bFailed)
	{
		return true;
	}
	return false;
}

