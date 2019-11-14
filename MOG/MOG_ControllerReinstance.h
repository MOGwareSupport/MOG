//--------------------------------------------------------------------------------
//	MOG_ControllerReinstance.h
//	
//	
//--------------------------------------------------------------------------------

#ifndef __MOG_CONTROLLERREINSTANCE_H__
#define __MOG_CONTROLLERREINSTANCE_H__

#include "MOG_Main.h"
#include "MOG_INI.h"
#include "MOG_Filename.h"
#include "MOG_System.h"
#include "MOG_Project.h"
#include "MOG_Time.h"
#include "MOG_Privileges.h"
#include "MOG_ControllerAsset.h"
#include "MOG_ControllerSyncData.h"

namespace MOG {
namespace CONTROLLER {
namespace CONTROLLERREINSTANCE {

//This object is just around so we have an easy way of tracking old and new properties.
//It used to be a part of the MOG_AssetReinstanceInfo and I used functionality based on this object but after I changed the MOG_AssetReinstanceInfo
//I didn't really want to rewrite the other code so I left this object in.
public __gc struct MOG_ReinstancePropertyChange
{
	MOG_Property *oldProperty;
	MOG_Property *newProperty;
};

//This is and object that contains all the information necessary to reinstance a single object. 
public __gc class MOG_AssetReinstanceInfo
{
public:
	//Normal constructor that initilizes the values via the constructor.
	MOG_AssetReinstanceInfo(MOG_Filename *oldAsset, MOG_Filename *newAsset, ArrayList *listOfPropertiesToRemove, ArrayList *listOfPropertiesToAdd, bool bRemoveEmptyClassifications);

	//The common way to initilize all the variables in this object.
	void InitilizeReinstanceAsset(MOG_Filename *oldAsset, MOG_Filename *newAsset, ArrayList *listOfPropertiesToRemove, ArrayList *listOfPropertiesToAdd);

	MOG_Filename *mOldAssetFilename;		//This is the name of the old Asset (The one that exists at the start of a reinstance.
	MOG_Filename *mNewAssetFilename;		//This is the new instance (name) for the asset or what it will become after the reinstance is completed.
	ArrayList *mPropertiesToAdd;			//This is a list of any new properties to add to the newly reinstanced asset.  These properties will not exist in the previous asset.
	ArrayList *mPropertiesToRemove;			//This is a list of properties contained in the old asset that need to be removed to make a vaild rinstanced object.
	bool mRemoveEmptyClassifications;		//This indicates if any empty classifications should be removed once reinstance has completed.
};


public __gc class MOG_ReinstanceAssetList : public ArrayList
{
public:
	void Add(MOG_AssetReinstanceInfo* reinstanceAsset);

	MOG_AssetReinstanceInfo *FindByAssetFilename(MOG_Filename *assetFilename);
	bool IsClassificationListed(String *classification);
};


//This class provides all the functionality necessary for reinstancing one or many objects.
public __gc class MOG_ControllerReinstance
{
public:
	//This is the constructor we would use to reinstance a single asset.
	MOG_ControllerReinstance(MOG_Filename *oldAssetName, MOG_Filename *newAssetName);
	//This is the constructor we would use to modify the properties of an already blessed asset
	MOG_ControllerReinstance(MOG_Filename *AssetFilename, ArrayList *removeProperties, ArrayList *addProperties);
	//This is the constructor we would use to modify the properties of an already blessed asset
	MOG_ControllerReinstance(MOG_Filename *AssetFilename, ArrayList *removeProperties, ArrayList *addProperties, String* timestamp);
	//This is the constructor we would use to reinstance an entire classification.
	MOG_ControllerReinstance(String *oldClassification, String *newClassification);

	//This gets the count of objects the reisntac controller is planning on reinstancing.
	int GetClassificationsToReinstanceCount()		{ return mClassificationsToReinstanceList->Count; };
	int GetAssetsToReinstanceCount()				{ return mAssetsToReinstanceList->Count; };
	int GetDependantClassificationsCount()			{ return mDependantClassificationsList->Count; };
	int GetDependantAssetsCount()					{ return mDependantAssetsList->Count; };
	int GetParentAssetsCount()						{ return mParentAssetsList->Count; };

	//Once we have a reinstance object created it dosn't do anything until we call this method.  This allows us to modify the controller before we do the actual work.
	//The parameter passed allows us to decide if we want to remove the previous asset from the project when we reinstance via the controller.
	//There were some situations where I though we may not want to remove the assets via the controller 
	bool ReinstanceAllAssets();

	//This function is a helper function but it can be use in other places to take a asset filename and replace the old classifcation with the new classification within the filename and returns the new name.
	static String *ChangeClassificationInName(String* fullName, String* oldClassification, String* newClassification);


private:
	//This is the initilization function That completely populates the reinstance controller when it is constructed.
	void InitilizeControllerReinstance(void);
	void InitilizeControllerReinstance(String* timestamp);
	//This is strictly a helper function to populate the mClassificationsToReinstance list with all the affected classifications by this reinstance.
	void InitializeReinstanceClassifications(void);
	//This is strictly a helper function to populate the mAsstsToReinstance list with all the assets directly affected by this reinstance.
	void InitializeReinstanceAssets(void);
	//This function initializes mDependantClassificationsList with any classifications with Relationship Dependancies
	bool InitializeDependantClassifications();
	//This function initializes mDependantAssetsList with any assets with Relationship Dependancies
	bool InitializeDependantAssets();

	//This function examines all the relationships of all the dependant assets and fixes them so that they point to the newly reinstanced (or moved) asset location.
	bool FixupAssetRelationshipOfDependantAssets(void);
	// This function examines all the relationships in dependant classifications and fixes them so that they point to the newly reinstanced (or moved) asset location
	bool FixupAssetRelationshipsOfDependantClassifications(void);
	//This is a helper function which "pre creates" the parent packages so the Reinstanced dependant files have somting to point at.
	bool CreateParentAssetPlaceholders(void);
	//This function create the new classification tree based on the mClassificationsToReinstanceList
	bool CreateNewClassificationTree(void);

	void Constructor1_Worker(Object* sender, DoWorkEventArgs* e);
	void Constructor2_Worker(Object* sender, DoWorkEventArgs* e);
	void Constructor3_Worker(Object* sender, DoWorkEventArgs* e);
	void ReinstanceAllAssets_Worker(Object* sender, DoWorkEventArgs* e);

private:
	//This is the complete list of classifications we will need to be moving.
	ArrayList *mClassificationsToReinstanceList;
	//This is the complete list of assets we will be reinstancing in this session.
	MOG_ReinstanceAssetList *mAssetsToReinstanceList;
	//This is the list of classifications that have a dependancy on one or more item in the mAssetsToReinstance list
	ArrayList *mDependantClassificationsList;
	//This is the list of assets that have a dependancy on one or more item in the mAssetsToReinstance list
	ArrayList *mDependantAssetsList;
	//This is the list of Assets that have a prent relationship to an item in the  mDependantAssets
	MOG_ReinstanceAssetList *mParentAssetsList;
	//This is the lable used for the reinstance opperation.
	String *mReinstanceLabel;
	//This is the old classification or asset name of the file we are reinstancing.
	String *mOldName;
	//This is the new classification or asset name for the file we are reinstancing.
	String *mNewName;
	//New Asset timestamp
	String *mNewTimestamp;
};
}
}
}

using namespace MOG::CONTROLLER::CONTROLLERREINSTANCE;

#endif	// __MOG_CONTROLLERREINSTANCE_H__