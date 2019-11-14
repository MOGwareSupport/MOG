//--------------------------------------------------------------------------------
//	MOG_PropertyFactory.cpp
//	
//	
//--------------------------------------------------------------------------------

#include "stdafx.h"

#include "MOG_Define.h"
#include "MOG_StringUtils.h"
#include "MOG_DosUtils.h"
#include "MOG_Time.h"
#include "MOG_ControllerProject.h"
#include "MOG_ControllerPackage.h"
#include "MOG_ControllerSyncData.h"
#include "MOG_PropertyFactory.h"
#include "MOG_Property.h"

#include <stdio.h>

//////////////////////////////////////////////////////////
//Start of Relationship Properties 
////////////////////////////////////////////////////////
MOG_Property *MOG_PropertyFactory::MOG_Relationships::New_RelationshipAssignment( String *relationship, String *assetName, String *groups, String *objects )
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Relationships, relationship, MOG_ControllerPackage::CombinePackageAssignment(assetName, groups, objects), S"None");
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Relationships::New_PackageAssignment( String *packageName, String *packageGroups, String *packageObjects )
{
	return New_PackageAssignment( packageName, packageGroups, packageObjects, true );
}

MOG_Property *MOG_PropertyFactory::MOG_Relationships::New_PackageAssignment( String *packageName, String *packageGroups, String *packageObjects, bool bForceNoneValue )
{
	String *value = (bForceNoneValue) ? S"None" : S"";

	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Relationships, PROPTEXT_Relationships_Packages, MOG_ControllerPackage::CombinePackageAssignment(packageName, packageGroups, packageObjects), value);
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Relationships::New_DependencyAssignment( MOG_Filename *assetFilename )
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Relationships, PROPTEXT_Relationships_Dependency, assetFilename->GetAssetFullName(), S"None");
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Relationships::New_AssetLink( String *linkedFilename, String *packageName, String *packageGroups, String *packageObjects )
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Relationships, PROPTEXT_Relationships_AssetLink, MOG_ControllerPackage::CombinePackageAssignment(packageName, packageGroups, packageObjects), linkedFilename);
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Relationships::New_AssetReference( String *referencedFilename, String *packageName, String *packageGroups, String *packageObjects )
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Relationships, PROPTEXT_Relationships_AssetReference, MOG_ControllerPackage::CombinePackageAssignment(packageName, packageGroups, packageObjects), referencedFilename);
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Relationships::New_AssetSourceFile( String *referencedFilename )
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Relationships, PROPTEXT_Relationships_AssetSourceFile, referencedFilename, "");
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Relationships::New_AssetSourceApplication( String *referencedAplication )
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Relationships, PROPTEXT_Relationships_AssetSourceApp, referencedAplication, "");
	return property;
}

//////////////////////////////////////////////////////////
//Start of MOG_Asset_StatsProperties
////////////////////////////////////////////////////////
MOG_Property *MOG_PropertyFactory::MOG_Asset_StatsProperties::New_Creator(String * creator)
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_AssetStats, PROPTEXT_AssetStats_Creator, creator);
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Asset_StatsProperties::New_Owner(String *owner)
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_AssetStats, PROPTEXT_AssetStats_Owner, owner);
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Asset_StatsProperties::New_SourceMachine(String *sourceMachine)
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_AssetStats, PROPTEXT_AssetStats_SourceMachine, sourceMachine);
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Asset_StatsProperties::New_SourcePath( String *sourcePath )
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_AssetStats, PROPTEXT_AssetStats_SourcePath, sourcePath);
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Asset_StatsProperties::New_CreatedTime( String *versionTimestamp )
{
	// Check if no specific time was specified?
	if (!versionTimestamp || versionTimestamp->Length == 0)
	{
		// Auto generate a current time stamp
		versionTimestamp = MOG_Time::GetVersionTimestamp();
	}

	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_AssetStats, PROPTEXT_AssetStats_CreatedTime, versionTimestamp);
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Asset_StatsProperties::New_ModifiedTime( String *versionTimestamp )
{
	// Check if no specific time was specified?
	if (!versionTimestamp || versionTimestamp->Length == 0)
	{
		// Auto generate a current time stamp
		versionTimestamp = MOG_Time::GetVersionTimestamp();
	}

	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_AssetStats, PROPTEXT_AssetStats_ModifiedTime, versionTimestamp);
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Asset_StatsProperties::New_RippedTime( String *versionTimestamp )
{
	// Check if no specific time was specified?
	if (!versionTimestamp || versionTimestamp->Length == 0)
	{
		// Auto generate a current time stamp
		versionTimestamp = MOG_Time::GetVersionTimestamp();
	}

	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_AssetStats, PROPTEXT_AssetStats_RippedTime, versionTimestamp);
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Asset_StatsProperties::New_BlessedTime( String *versionTimestamp )
{
	// Check if no specific time was specified?
	if (!versionTimestamp || versionTimestamp->Length == 0)
	{
		// Auto generate a current time stamp
		versionTimestamp = MOG_Time::GetVersionTimestamp();
	}

	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_AssetStats, PROPTEXT_AssetStats_BlessedTime, versionTimestamp);
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Asset_StatsProperties::New_PreviousRevision( String *versionTimestamp )
{
	// Check if no specific time was specified?
	if (!versionTimestamp || versionTimestamp->Length == 0)
	{
		// Auto generate a current time stamp
		versionTimestamp = MOG_Time::GetVersionTimestamp();
	}

	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_AssetStats, PROPTEXT_AssetStats_PreviousRevision, versionTimestamp);
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Asset_StatsProperties::New_Status(MOG_AssetStatusType status)
{
	return New_Status(MOG_AssetStatus::GetInfo(status)->Status);
}

MOG_Property *MOG_PropertyFactory::MOG_Asset_StatsProperties::New_Status(String *status)
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_AssetStats, PROPTEXT_AssetStats_Status, status);
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Asset_StatsProperties::New_Size( int size)
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_AssetStats, PROPTEXT_AssetStats_Size, size.ToString());
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Asset_StatsProperties::New_LastComment( String *comment )
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_AssetStats, PROPTEXT_AssetStats_LastComment, comment);
	return property;
}

//////////////////////////////////////////////////////////
//Start of MOG_Asset_InfoProperties 
////////////////////////////////////////////////////////
MOG_Property *MOG_PropertyFactory::MOG_Asset_InfoProperties::New_Description(String *description)
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_AssetInfo, PROPTEXT_AssetInfo_Description, description);
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Asset_InfoProperties::New_Group(String *group)
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_AssetInfo, PROPTEXT_AssetInfo_Group, group);
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Asset_InfoProperties::New_AssetIcon(String *icon)
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_AssetInfo, PROPTEXT_AssetInfo_AssetIcon, icon);
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Asset_InfoProperties::New_AssetViewer(String *assetViewer)
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_AssetInfo, PROPTEXT_AssetInfo_AssetViewer, assetViewer);
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Asset_InfoProperties::New_PropertyMenu(String *propertyMenu)
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_AssetInfo, PROPTEXT_AssetInfo_PropertyMenu, propertyMenu);
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Asset_InfoProperties::New_IsPackagedAsset( bool bIsPackagedAsset )
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_AssetInfo, PROPTEXT_AssetInfo_IsPackagedAsset, bIsPackagedAsset.ToString());
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Asset_InfoProperties::New_IsPackage( bool bIsPackage )
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_AssetInfo, PROPTEXT_AssetInfo_IsPackage, bIsPackage.ToString());
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Asset_InfoProperties::New_IsBuild(bool bIsBuild)
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_AssetInfo, PROPTEXT_AssetInfo_IsBuild, bIsBuild.ToString());
	return property;
}

///////////////////////////////////////////////////////////////////
//Start of MOG_Asset_OptionsProperties
////////////////////////////////////////////////////////////////////

MOG_Property *MOG_PropertyFactory::MOG_Asset_OptionsProperties::New_DefaultAssetNameIncludeExtension(String* defaultAssetNameIncludeExtension)
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_DefaultAssetNameIncludeExtension, defaultAssetNameIncludeExtension);
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Asset_OptionsProperties::New_DefaultAssetNamePlatform(String* defaultAssetNamePlatform)
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_DefaultAssetNamePlatform, defaultAssetNamePlatform);
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Asset_OptionsProperties::New_UnBlessable(bool unblessable)
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_UnBlessable, unblessable.ToString());
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Asset_OptionsProperties::New_UniquePackageAssignmentVerification(bool uniquePackageAssignmentVerification)
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_UniquePackageAssignmentVerification, uniquePackageAssignmentVerification.ToString());
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Asset_OptionsProperties::New_OutofdateVerification(bool outofdateVerification)
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_OutofdateVerification, outofdateVerification.ToString());
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Asset_OptionsProperties::New_LocalVerificationBeforeBless(bool value)
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_LocalVerificationBeforeBless, value.ToString());
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Asset_OptionsProperties::New_ValidSlaves(String *validSlaves)
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_ValidSlaves, validSlaves);
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Asset_OptionsProperties::New_MaintainLock(bool value)
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_MaintainLock, value.ToString());
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Asset_OptionsProperties::New_LockPackageManagement(bool value)
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_LockPackageManagement, value.ToString());
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Asset_OptionsProperties::New_ShowPostLockComment(bool value)
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_ShowPostLockComment, value.ToString());
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Asset_OptionsProperties::New_RequireLockComment(bool value)
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_RequireLockComment, value.ToString());
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Asset_OptionsProperties::New_UnReferencedRevisionHistory( int numRevisions )
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_UnReferencedRevisionHistory, numRevisions.ToString());
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Asset_OptionsProperties::New_BlessTarget( String *blessTarget )
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_BlessTarget, blessTarget);
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Asset_OptionsProperties::New_BlessEmailNotify( String *blessEmailNotify )
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_BlessEmailNotify, blessEmailNotify);
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Asset_OptionsProperties::New_ShowLocalModifiedWarning( bool value)
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_ShowLocalModifiedWarning, value.ToString());
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Asset_OptionsProperties::New_AutoLockOnImport( bool value)
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_AssetOptions, PROPTEXT_AssetOptions_AutoLockOnImport, value.ToString());
	return property;
}


///////////////////////////////////////////////////////////////////////
//Start of MOG_Classification_InfoProperties 
///////////////////////////////////////////////////////////////////////
MOG_Property *MOG_PropertyFactory::MOG_Classification_InfoProperties::New_IsLibrary( bool bIsLibrary )
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_ClassificationInfo, PROPTEXT_ClassificationInfo_IsLibrary, bIsLibrary.ToString());
	return property;
}

MOG_Property  *MOG_PropertyFactory::MOG_Classification_InfoProperties::New_ClassIcon(String *classIcon)
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_ClassificationInfo, PROPTEXT_ClassificationInfo_ClassIcon, classIcon);
	return property;
}

MOG_Property  *MOG_PropertyFactory::MOG_Classification_InfoProperties::New_FilterInclusions(String *inclusions)
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_ClassificationInfo, PROPTEXT_ClassificationInfo_FilterInclusions, inclusions);
	return property;
}

MOG_Property  *MOG_PropertyFactory::MOG_Classification_InfoProperties::New_FilterExclusions(String *exclusions)
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_ClassificationInfo, PROPTEXT_ClassificationInfo_FilterExclusions, exclusions);
	return property;
}

MOG_Property  *MOG_PropertyFactory::MOG_Classification_InfoProperties::New_PromptUserOnFilterViolation(bool bPrompt)
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_ClassificationInfo, PROPTEXT_ClassificationInfo_PromptUserOnFilterViolation, bPrompt.ToString());
	return property;
}

MOG_Property  *MOG_PropertyFactory::MOG_Classification_InfoProperties::New_Classification(String *classification)
{
	MOG_Property  *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_ClassificationInfo, PROPTEXT_ClassificationInfo_Classification, classification);
	return property;
}

///////////////////////////////////////////////////////////////////
//Start of MOG_Sync_OptionsProperties 
///////////////////////////////////////////////////////////////////
MOG_Property  *MOG_PropertyFactory::MOG_Sync_OptionsProperties::New_SyncLabel(String *SyncLabel)
{
	MOG_Property  *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_SyncOptions, PROPTEXT_SyncOptions_SyncLabel, SyncLabel);
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Sync_OptionsProperties::New_SyncTargetPath( String *relativeSyncTargetPath )
{
	MOG_Property *property = new MOG_Property();

	// Make sure there is no drive letter listed in the specified SyncTargetPath
	if (relativeSyncTargetPath->IndexOf(S":\\") != -1)
	{
		// First attempt to strip off the active local workspace directory
//?	MultiWorkspaces - We need to be careful about this...what assumptions are we making here?
		MOG_ControllerSyncData *pSyncData = MOG_ControllerProject::GetCurrentSyncDataController();
		if (pSyncData)
		{
			String* testSyncTargetPath = String::Concat(relativeSyncTargetPath, S"\\");
			String* testSyncDir = String::Concat(pSyncData->GetSyncDirectory(), S"\\");
			if (testSyncTargetPath->StartsWith(testSyncDir, StringComparison::CurrentCultureIgnoreCase))
			{
				relativeSyncTargetPath = testSyncTargetPath->Substring(testSyncDir->Length);
			}
		}

		// Double check to make sure we don't have a drive letter?
		if (relativeSyncTargetPath->IndexOf(S":\\") != -1)
		{
			// Inform the user they can't have a drive listed because the SyncTargetPath needs to be relative to the project
			MOGPromptResult result = MOG_Prompt::PromptResponse(	S"SyncTarget Property Warning",
																	String::Concat(	S"The SyncTargetPath property can't contain a drive letter because it reflects a relative path within a project.\n",
																					S"The Property was not set."),
																	NULL, 
																	MOGPromptButtons::OK, MOG_ALERT_LEVEL::ALERT);
			// Don't set the property
			return NULL;
		}
	}

    // Make sure we trim off unneeded backslashes and spaces
	relativeSyncTargetPath = relativeSyncTargetPath->Trim(S"\\ "->ToCharArray());

	property->Initialize(PROPTEXT_Properties, PROPTEXT_SyncOptions, PROPTEXT_SyncOptions_SyncTargetPath, relativeSyncTargetPath);
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Sync_OptionsProperties::New_SyncFiles( bool syncFiles )
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_SyncOptions, PROPTEXT_SyncOptions_SyncFiles, syncFiles.ToString());
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Sync_OptionsProperties::New_SyncAsReadOnly( bool syncAsReadOnly )
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_SyncOptions, PROPTEXT_SyncOptions_SyncAsReadOnly, syncAsReadOnly.ToString());
	return property;
}

////////////////////////////////////////////////////////////////////
//Start of MOG_Rip_OptionsProperties 
//////////////////////////////////////////////////////////////////////
MOG_Property *MOG_PropertyFactory::MOG_Rip_OptionsProperties::New_ShowRipCommandWindow(bool showRipCommandWindow)
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_RipOptions, PROPTEXT_RipOptions_ShowRipCommandWindow, showRipCommandWindow.ToString());
	return property;
}

MOG_Property  *MOG_PropertyFactory::MOG_Rip_OptionsProperties::New_AssetRipTasker(String *assetRipTasker)
{
	MOG_Property  *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_RipOptions, PROPTEXT_RipOptions_AssetRipTasker, assetRipTasker);
	return property;
}

MOG_Property  *MOG_PropertyFactory::MOG_Rip_OptionsProperties::New_AssetRipper(String *asssetRipper)
{
	MOG_Property  *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_RipOptions, PROPTEXT_RipOptions_AssetRipper, asssetRipper);
	return property;
}

MOG_Property  *MOG_PropertyFactory::MOG_Rip_OptionsProperties::New_DivergentPlatformDataType(String *devergentPlatformDataType)
{
	MOG_Property  *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_RipOptions, PROPTEXT_RipOptions_DivergentPlatformDataType, devergentPlatformDataType);
	return property;
}

MOG_Property  *MOG_PropertyFactory::MOG_Rip_OptionsProperties::New_UseTempRipDir(bool useTempRipDir)
{
	MOG_Property  *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_RipOptions, PROPTEXT_RipOptions_UseTempRipDir, useTempRipDir.ToString());
	return property;
}

MOG_Property  *MOG_PropertyFactory::MOG_Rip_OptionsProperties::New_UseLocalTempRipDir(bool useLocalTempRipDir)
{
	MOG_Property  *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_RipOptions, PROPTEXT_RipOptions_UseLocalTempRipDir, useLocalTempRipDir.ToString());
	return property;
}

MOG_Property  *MOG_PropertyFactory::MOG_Rip_OptionsProperties::New_CopyFilesIntoTempRipDir(bool copyFilesIntoTempRipDir)
{
	MOG_Property  *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_RipOptions, PROPTEXT_RipOptions_CopyFilesIntoTempRipDir, copyFilesIntoTempRipDir.ToString());
	return property;
}

MOG_Property  *MOG_PropertyFactory::MOG_Rip_OptionsProperties::New_AutoDetectRippedFiles(bool autoDetectRippedFiles)
{
	MOG_Property  *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_RipOptions, PROPTEXT_RipOptions_AutoDetectRippedFiles, autoDetectRippedFiles.ToString());
	return property;
}

//////////////////////////////////////////////////////////////////////////
//Start of MOG_Package_OptionsProperties
//////////////////////////////////////////////////////////////////////////
MOG_Property *MOG_PropertyFactory::MOG_Package_OptionsProperties::New_DefaultPackageFileExtension(String *defaultPackageFileExtension)
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_DefaultPackageFileExtension, defaultPackageFileExtension);
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Package_OptionsProperties::New_ShowPackageCommandWindow(bool showPackageCommandWindow)
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_ShowPackageCommandWindow, showPackageCommandWindow.ToString());
	return property;
}

MOG_Property  *MOG_PropertyFactory::MOG_Package_OptionsProperties::New_PackageStyle(String *packageStyle)
{
	MOG_Property  *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_PackageStyle, packageStyle);
	return property;
}

MOG_Property  *MOG_PropertyFactory::MOG_Package_OptionsProperties::New_PackageWorkspaceDirectory(String *packageWorkspaceDirectory)
{
	MOG_Property  *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_PackageWorkspaceDirectory, packageWorkspaceDirectory);
	return property;
}

MOG_Property  *MOG_PropertyFactory::MOG_Package_OptionsProperties::New_PackageDataDirectory(String *packageDataDirectory)
{
	MOG_Property  *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_PackageDataDirectory, packageDataDirectory);
	return property;
}

MOG_Property  *MOG_PropertyFactory::MOG_Package_OptionsProperties::New_SyncPackageWorkspaceDirectory(bool syncPackageWorkspaceDirectory)
{
	MOG_Property  *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_SyncPackageWorkspaceDirectory, syncPackageWorkspaceDirectory.ToString());
	return property;
}

MOG_Property  *MOG_PropertyFactory::MOG_Package_OptionsProperties::New_CleanupPackageWorkspaceDirectory(bool cleanupPackageWorkspaceDirectory)
{
	MOG_Property  *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_CleanupPackageWorkspaceDirectory, cleanupPackageWorkspaceDirectory.ToString());
	return property;
}

MOG_Property  *MOG_PropertyFactory::MOG_Package_OptionsProperties::New_AutoPackage( bool bAutoPackage )
{
	MOG_Property  *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_AutoPackage, bAutoPackage.ToString());
	return property;
}

MOG_Property  *MOG_PropertyFactory::MOG_Package_OptionsProperties::New_ExecuteNetworkPackageMerge( bool bExecuteNetworkPackageMerge )
{
	MOG_Property  *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_ExecuteNetworkPackageMerge, bExecuteNetworkPackageMerge.ToString());
	return property;
}

MOG_Property  *MOG_PropertyFactory::MOG_Package_OptionsProperties::New_ClusterPackaging(bool clusterPackaging)
{
	MOG_Property  *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_ClusterPackaging, clusterPackaging.ToString());
	return property;
}

MOG_Property  *MOG_PropertyFactory::MOG_Package_OptionsProperties::New_PackagePreMergeEvent(String *packagePreMergeEvent)
{
	MOG_Property  *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_PackagePreMergeEvent, packagePreMergeEvent);
	return property;
}

MOG_Property  *MOG_PropertyFactory::MOG_Package_OptionsProperties::New_PackagePostMergeEvent(String *packagePostMergeEvent)
{
	MOG_Property  *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_PackagePostMergeEvent, packagePostMergeEvent);
	return property;
}

MOG_Property  *MOG_PropertyFactory::MOG_Package_OptionsProperties::New_InputPackageTaskFile(String *inputPackageTaskFile)
{
	MOG_Property  *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_InputPackageTaskFile, inputPackageTaskFile);
	return property;
}

MOG_Property  *MOG_PropertyFactory::MOG_Package_OptionsProperties::New_OutputPackageTaskFile(String *outputPackageTaskFile)
{
	MOG_Property  *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_OutputPackageTaskFile, outputPackageTaskFile);
	return property;
}

MOG_Property  *MOG_PropertyFactory::MOG_Package_OptionsProperties::New_TaskFileTool(String *taskFileTool)
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_TaskFileTool, taskFileTool);
	return property;
}

MOG_Property  *MOG_PropertyFactory::MOG_Package_OptionsProperties::New_PackageCommand_DeletePackageFile(String *deletePackageFileCommand)
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_PackageCommand_DeletePackageFile, deletePackageFileCommand);
	return property;
}

MOG_Property  *MOG_PropertyFactory::MOG_Package_OptionsProperties::New_PackageCommand_ResolveLateResolvers(String *resolveLateResolverCommand)
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_PackageOptions, PROPTEXT_PackageOptions_PackageCommand_ResolveLateResolvers, resolveLateResolverCommand);
	return property;
}

//////////////////////////////////////////////////////////////////////////
//Start of MOG_Package_CommandsProperties
//////////////////////////////////////////////////////////////////////////
MOG_Property  *MOG_PropertyFactory::MOG_Package_CommandsProperties::New_PackageCommand_Propagation(String *scope)
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_PackageCommands, PROPTEXT_PackageCommands_PackageCommand_Propagation, scope);
	return property;
}

MOG_Property  *MOG_PropertyFactory::MOG_Package_CommandsProperties::New_PackageCommand_Add(String *addCommand)
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_PackageCommands, PROPTEXT_PackageCommands_PackageCommand_Add, addCommand);
	return property;
}

MOG_Property *MOG_PropertyFactory::MOG_Package_CommandsProperties::New_PackageCommand_Remove(String *removeCommand)
{
	MOG_Property  *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_PackageCommands, PROPTEXT_PackageCommands_PackageCommand_Remove, removeCommand);
	return property;
}

//////////////////////////////////////////////////////////////////
//Start of MOG_Build_OptionsProperties
//////////////////////////////////////////////////////////////////

MOG_Property *MOG_PropertyFactory::MOG_Build_OptionsProperties::New_ShowBuildCommandWindow(bool showBuildCommandWindow)
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_BuildOptions, PROPTEXT_BuildOptions_ShowBuildCommandWindow, showBuildCommandWindow.ToString());
	return property;
}

MOG_Property  *MOG_PropertyFactory::MOG_Build_OptionsProperties::New_BuildTool(String *buildTool)
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_BuildOptions, PROPTEXT_BuildOptions_BuildTool, buildTool);
	return property;
}

MOG_Property  *MOG_PropertyFactory::MOG_Build_OptionsProperties::New_BuildWorkingDirectory(String *buildWorkingDirectory)
{
	MOG_Property *property = new MOG_Property();
	property->Initialize(PROPTEXT_Properties, PROPTEXT_BuildOptions, PROPTEXT_BuildOptions_BuildWorkingDirectory, buildWorkingDirectory);
	return property;
}

