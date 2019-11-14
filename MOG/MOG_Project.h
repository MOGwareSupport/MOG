//--------------------------------------------------------------------------------
//	MOG_PROJECT.h
//	
//	
//--------------------------------------------------------------------------------

#ifndef __MOG_PROJECT_H__
#define __MOG_PROJECT_H__

#include "MOG_Define.h"
#include "MOG_User.h"
#include "MOG_INI.h"
#include "MOG_Properties.h"
#include "MOG_Platform.h"


namespace MOG {
namespace PROJECT {


public __gc class MOG_Project {

public:
	MOG_Project(void);					// For Create new project

	void Save();

	bool Load();
	bool Load(String *configFilename, String *branchName);
	bool Create(String *systemProjectsPath, String *projectName);
	bool Reload();
	
	bool Offline(String *targetPath);
	bool DuplicateProject(String *newProjectName);
	
	MOG_Project *CopyProject(String *systemProjectPath, String *targetProject);
	bool CopyUsers(MOG_Project *sourceProject);
	bool CopyPlatforms(MOG_Project *sourceProject);

	
	bool	UserAdd(MOG_User *user);
	bool	UserUpdate(String *currentUserName, String *newUserName, String *emailAddress, String *blessTarget, String *department);
	bool	UserRemove(String *user);

	bool	PlatformAdd(MOG_Platform *patform);
	bool	PlatformRemove(String *platform);

	bool	UserDepartmentAdd(MOG_UserDepartment *department);
	bool	UserDepartmentAdd(String *department);
	bool	UserDepartmentRemove(String *department);

	// Platforms
	ArrayList *GetPlatforms() { return mPlatforms; };
	String *GetPlatformNames()[];
	MOG_Platform *GetPlatform(String *platformName);
	bool PlatformExists(String *platformName);

	// Users
	ArrayList *GetUsers() { return mUsers; };
	MOG_User *GetUser(String *userName);
	ArrayList *GetUserDepartments() { return mUserDepartments; };
	MOG_UserDepartment *GetUserDepartment(String *userDeptName);
	ArrayList *GetDepartmentUsers(String *department);

	// Classification Properties
	bool ClassificationExists(String* classificationFullName);
	bool ClassificationAdd(String *classificationFullName);
	bool ClassificationRename(String *currentClassificationName, String* newClassificationName);
	bool AssetRename(String *sourceAssetFullName, String * targetAssetFullName);
	bool ModifyBlessedAssetProperties(MOG_Filename *AssetFilename, ArrayList *removeProperties, ArrayList *addProperties);
	bool ClassificationRemove(String *classificationFullName);
	MOG_Properties *GetClassificationProperties(String *classification);
	ArrayList *GetSubClassifications( String *classification, String *branchName );
	ArrayList *GetSubClassifications( String *classification, String *branchName, ArrayList *properties );
	ArrayList *GetArchivedSubClassifications( String *classificationParent );
	
	// Classifications
	ArrayList *GetRootClassificationNames();
	ArrayList *GetSubClassificationNames(String *classificatioName);

	// Access Functions
	String				*GetProjectConfigFilename(void) { return mProjectConfigFilename; };
	MOG_PropertiesIni	*GetConfigFile(void) { return mConfigFile; };

	void	SetProjectName(String *projectName) { mProjectName = projectName;};
	String	*GetProjectName(void) { return mProjectName; };

	void	SetProjectPath(String *projectPath) {mProjectPath = projectPath;} ;
	String	*GetProjectPath(void) { return mProjectPath; };

	void	SetProjectToolsPath(String *projectToolsPath) { mProjectToolsPath = projectToolsPath;};
	String	*GetProjectToolsPath(void) { return mProjectToolsPath; };

	void	SetProjectUsersPath(String *projectUsersPath) {mProjectUsersPath = projectUsersPath;};
	String	*GetProjectUsersPath(void) { return mProjectUsersPath; };

	void	SetProjectCvsName(String *projectCvsName) {mProjectCvsName = projectCvsName;};
	String	*GetProjectCvsName(void) { return mProjectCvsName; };

	void	SetProjectBuildDirectory(String *projectBuildDirectory) {mProjectBuildDirectory = projectBuildDirectory;};
	String	*GetProjectBuildDirectory(void) { return mProjectBuildDirectory; };

	void	SetProjectConnectionString(String *connectionString);
	String	*GetProjectConnectionString(void) {return mProjectConnectionString;};

	void	SuppressProjectRefreshEvent() { mSuppressProjectRefreshEvent = true; };
	void	EnableProjectRefreshEvent() { mSuppressProjectRefreshEvent = false; };

private:
	void PopulatePlatformSyncFiles_Worker(Object* sender, DoWorkEventArgs* e);
	void RemovePlatformSpecificAssets_Worker(Object* sender, DoWorkEventArgs* e);
	void Offline_Worker(Object* sender, DoWorkEventArgs* e);
	void DuplicateProject_Worker(Object* sender, DoWorkEventArgs* e);
	void DuplicateProject_RepairClassificationsProperties(String* newProjectName, BackgroundWorker* worker);
	void DuplicateProject_RepairAssetsProperties(String* newProjectName, BackgroundWorker* worker);
	void DuplicateProject_RepairInboxAssets(String* newProjectName, BackgroundWorker* worker);
	void DuplicateProject_RepairProperties(String* newProjectName, MOG_Properties* properties);

private:
	String *mProjectName;				// Project Name
	String *mProjectKey;				// Project Key
	String *mProjectPath;				// Project's Path
	String *mProjectToolsPath;			// Project's Tools
	String *mProjectUsersPath;			// Project's Users
	String *mProjectCvsName;			// Project's CVS name
	String *mProjectBuildDirectory;		// Projects CVS build directory

	String *mProjectConnectionString;	//We will use this when we go to one database per project.

	String *mProjectConfigFilename;		//Ini location and filename
	
	MOG_PropertiesIni *mConfigFile;		//Project ini

	MOG_Project *mLoginProject;
	MOG_User *mLoginUser;

	ArrayList *mAssetsInfo;
	ArrayList *mPlatforms;
	ArrayList *mUserDepartments;
	ArrayList *mUsers;
	bool	   mSuppressProjectRefreshEvent;
};


}
}

using namespace MOG::PROJECT;

#endif	// __MOG_PROJECT_H__
