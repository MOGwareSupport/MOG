//--------------------------------------------------------------------------------
//	MOG_ControllerRipper.h
//	
//	
//---------------------------------------------------------------------------------

#ifndef __MOG_CONTROLLERRIPPER_H__
#define __MOG_CONTROLLERRIPPER_H__

#include "MOG_Main.h"
#include "MOG_ControllerAsset.h"



namespace MOG {
namespace CONTROLLER {
namespace CONTROLLERRIPPER {


public __gc class MOG_ControllerRipper
{
public:
	// Constructors
	MOG_ControllerRipper(MOG_Command *pCommand);
	MOG_ControllerRipper(MOG_Filename *assetFilename);
	MOG_ControllerRipper(MOG_Filename *assetFilename, MOG_Properties *properties, int originatingNetworkID, String *originatingUserName, String *jobLabel, String *validSlaves, bool showRipCommandWindow);

	// Process
	// Step 1
	ArrayList *GenerateSlaveTaskCommands()		{ return GenerateSlaveTaskCommands(S"All"); };
	ArrayList *GenerateSlaveTaskCommands(String *platformName);
	// Step 2
	bool ProcessSlaveTasks(ArrayList *pCommands);
	bool ProcessSlaveTask(MOG_Command *pCommand);
	// Step 3
	bool MOG_ControllerRipper::CompleteRip();

	// Test Rip
	bool LocalRipper()							{ return LocalRipper(NULL, S"All"); };
	bool LocalRipper(MOG_Properties *properties, String *platformName);

	// Utilities
	void ShowRipCommandWindow(bool value)		{ mShowRipCommandWindow = value; };
	String *GetOutputLog(void)					{ return mOutputLog; };

private:
	MOG_Filename *mAssetFilename;
	MOG_Properties *mProperties;
	int mOriginatingNetworkID;
	String *mOriginatingUserName;
	String *mJobLabel;
	String *mValidSlaves;
	bool mShowRipCommandWindow;
	String *mOutputLog;

	void Initialize(MOG_Filename *assetFilename, MOG_Properties *properties, int originatingNetworkID, String *originatingUserName, String *jobLabel, String *validSlaves, bool showRipCommandWindow);

	ArrayList *GetRipperEnvironment(String *platformName, MOG_Properties *pProperties);
	bool IsPlatformRelevent(String *platformName1, String *platformName2);

	String* CreateTempRipDir(bool bLocal, String* assetName);
	void CopyFilesIntoTempRipDir(String* source, String* tempDirectory);
	ArrayList* CheckForModifiedOriginalFiles(String* originalDirectory, ArrayList* originalFiles, String* workingDirectory, ArrayList* newFiles, DateTime preRipTime);
	ArrayList* DetectCreatedFiles(ArrayList* oldFiles, ArrayList* newFiles);
	void MoveModifiedFiles(ArrayList* files, String* sourcePath, String* destinationPath, bool bUseTempRipDir);
	void MoveCreatedFiles(ArrayList* files, String* sourcePath, String* destinationPath);
	void WarnModifiedFiles(ArrayList* files);
	void CleanUpCreatedFiles(ArrayList* files, String* workingDirectory);
};


}
}
}

using namespace MOG::CONTROLLER::CONTROLLERRIPPER;

#endif	// __MOG_CONTROLLERRIPPER_H__
