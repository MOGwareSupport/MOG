//--------------------------------------------------------------------------------
//	MOG_PropertyFactory.h
//	
//	
//--------------------------------------------------------------------------------

#ifndef __MOG_PropertyFactory_H__
#define __MOG_PropertyFactory_H__

#include "MOG_Define.h"
#include "MOG_Ini.h"
#include "MOG_Property.h"


namespace MOG 
{

#define PROPTEXT_Properties														S"Properties"

#define PROPTEXT_AssetStats														S"Asset Stats"
#define PROPTEXT_AssetStats_Creator												S"Creator"
#define PROPTEXT_AssetStats_Owner												S"Owner"
#define PROPTEXT_AssetStats_SourceMachine										S"SourceMachine"
#define PROPTEXT_AssetStats_SourcePath											S"SourcePath"
#define PROPTEXT_AssetStats_CreatedTime											S"CreatedTime"
#define PROPTEXT_AssetStats_ModifiedTime										S"ModifiedTime"
#define PROPTEXT_AssetStats_RippedTime											S"RippedTime"
#define PROPTEXT_AssetStats_BlessedTime											S"BlessedTime"
#define PROPTEXT_AssetStats_PreviousRevision									S"PreviousRevision"
#define PROPTEXT_AssetStats_Status												S"Status"
#define PROPTEXT_AssetStats_Size												S"Size"
#define PROPTEXT_AssetStats_LastComment											S"LastComment"

#define PROPTEXT_AssetInfo														S"Asset Info"
#define PROPTEXT_AssetInfo_Description											S"Description"
#define PROPTEXT_AssetInfo_Group												S"Group"
#define PROPTEXT_AssetInfo_AssetIcon											S"AssetIcon"
#define PROPTEXT_AssetInfo_AssetViewer											S"AssetViewer"
#define PROPTEXT_AssetInfo_PropertyMenu											S"PropertyMenu"
#define PROPTEXT_AssetInfo_IsPackagedAsset										S"IsPackagedAsset"
#define PROPTEXT_AssetInfo_IsPackage											S"IsPackage"
#define PROPTEXT_AssetInfo_IsBuild												S"IsBuild"

#define PROPTEXT_AssetOptions													S"Asset Options"
#define PROPTEXT_AssetOptions_DefaultAssetNameIncludeExtension					S"DefaultAssetNameIncludeExtension"
#define PROPTEXT_AssetOptions_DefaultAssetNamePlatform							S"DefaultAssetNamePlatform"
#define PROPTEXT_AssetOptions_UnBlessable										S"UnBlessable"
#define PROPTEXT_AssetOptions_UniquePackageAssignmentVerification				S"UniquePackageAssignmentVerification"
#define PROPTEXT_AssetOptions_OutofdateVerification								S"OutofdateVerification"
#define PROPTEXT_AssetOptions_LocalVerificationBeforeBless						S"LocalVerificationBeforeBless"
#define PROPTEXT_AssetOptions_ValidSlaves										S"ValidSlaves"
#define PROPTEXT_AssetOptions_MaintainLock										S"MaintainLock"
#define PROPTEXT_AssetOptions_LockPackageManagement								S"LockPackageManagement"
#define PROPTEXT_AssetOptions_RequireLockComment								S"RequireLockComment"
#define PROPTEXT_AssetOptions_ShowPostLockComment								S"ShowPostLockComment"
#define PROPTEXT_AssetOptions_UnReferencedRevisionHistory						S"UnReferencedRevisionHistory"
#define PROPTEXT_AssetOptions_BlessTarget										S"BlessTarget"
#define PROPTEXT_AssetOptions_BlessEmailNotify									S"BlessEmailNotify"
#define PROPTEXT_AssetOptions_ShowLocalModifiedWarning							S"ShowLocalModifiedWarning"
#define PROPTEXT_AssetOptions_AutoLockOnImport									S"AutoLockOnImport"
	

#define PROPTEXT_ClassificationInfo												S"Classification Info"
#define PROPTEXT_ClassificationInfo_IsLibrary									S"IsLibrary"
#define PROPTEXT_ClassificationInfo_ClassIcon									S"ClassIcon"
#define PROPTEXT_ClassificationInfo_FilterInclusions							S"FilterInclusions"
#define PROPTEXT_ClassificationInfo_FilterExclusions							S"FilterExclusions"
#define PROPTEXT_ClassificationInfo_PromptUserOnFilterViolation					S"PromptUserOnFilterViolation"
#define PROPTEXT_ClassificationInfo_Classification								S"Classification"

#define PROPTEXT_SyncOptions													S"Sync Options"
#define PROPTEXT_SyncOptions_SyncLabel											S"SyncLabel"
#define PROPTEXT_SyncOptions_SyncTargetPath										S"SyncTargetPath"
#define PROPTEXT_SyncOptions_SyncFiles											S"SyncFiles"
#define PROPTEXT_SyncOptions_SyncAsReadOnly										S"SyncAsReadOnly"

#define PROPTEXT_RipOptions														S"Rip Options"
#define PROPTEXT_RipOptions_ShowRipCommandWindow								S"ShowRipCommandWindow"
#define PROPTEXT_RipOptions_AssetRipTasker										S"AssetRipTasker"
#define PROPTEXT_RipOptions_AssetRipper											S"AssetRipper"
#define PROPTEXT_RipOptions_DivergentPlatformDataType							S"DivergentPlatformDataType"
#define PROPTEXT_RipOptions_UseTempRipDir										S"UseTempRipDir"
#define PROPTEXT_RipOptions_UseLocalTempRipDir									S"UseLocalTempRipDir"
#define PROPTEXT_RipOptions_CopyFilesIntoTempRipDir								S"CopyFilesIntoTempRipDir"
#define PROPTEXT_RipOptions_AutoDetectRippedFiles								S"AutoDetectRippedFiles"

#define PROPTEXT_PackageOptions													S"Package Options"
#define PROPTEXT_PackageOptions_DefaultPackageFileExtension						S"DefaultPackageFileExtension"
#define PROPTEXT_PackageOptions_ShowPackageCommandWindow						S"ShowPackageCommandWindow"
#define PROPTEXT_PackageOptions_PackageStyle									S"PackageStyle"
#define PROPTEXT_PackageOptions_PackageWorkspaceDirectory						S"PackageWorkspaceDirectory"
#define PROPTEXT_PackageOptions_PackageDataDirectory							S"PackageDataDirectory"
#define PROPTEXT_PackageOptions_SyncPackageWorkspaceDirectory					S"SyncPackageWorkspaceDirectory"
#define PROPTEXT_PackageOptions_CleanupPackageWorkspaceDirectory				S"CleanupPackageWorkspaceDirectory"
#define PROPTEXT_PackageOptions_AutoPackage										S"AutoPackage"
#define PROPTEXT_PackageOptions_ExecuteNetworkPackageMerge						S"ExecuteNetworkPackageMerge"
#define PROPTEXT_PackageOptions_ClusterPackaging								S"ClusterPackaging"
#define PROPTEXT_PackageOptions_PackagePreMergeEvent							S"PackagePreMergeEvent"
#define PROPTEXT_PackageOptions_PackagePostMergeEvent							S"PackagePostMergeEvent"
#define PROPTEXT_PackageOptions_InputPackageTaskFile							S"InputPackageTaskFile"
#define PROPTEXT_PackageOptions_OutputPackageTaskFile							S"OutputPackageTaskFile"
#define PROPTEXT_PackageOptions_TaskFileTool									S"TaskFileTool"
#define PROPTEXT_PackageOptions_PackageCommand_DeletePackageFile				S"PackageCommand_DeletePackageFile"
#define PROPTEXT_PackageOptions_PackageCommand_ResolveLateResolvers				S"PackageCommand_ResolveLateResolvers"

#define PROPTEXT_PackageCommands												S"Packaging Commands"
#define PROPTEXT_PackageCommands_PackageCommand_Propagation						S"PackageCommand_Propagation"
#define PROPTEXT_PackageCommands_PackageCommand_Add								S"PackageCommand_AddFile"		// TODO - Rename this to remove 'File' but beware of backward compatibility issues!!!
#define PROPTEXT_PackageCommands_PackageCommand_Remove							S"PackageCommand_RemoveFile"	// TODO - Rename this to remove 'File' but beware of backward compatibility issues!!!

#define PROPTEXT_BuildOptions													S"Build Options"
#define PROPTEXT_BuildOptions_ShowBuildCommandWindow							S"ShowBildCommandWindow"
#define PROPTEXT_BuildOptions_BuildTool											S"BuildTool"
#define PROPTEXT_BuildOptions_BuildWorkingDirectory								S"BuildWorkingDirectory"

#define PROPTEXT_Relationships													S"Relationships"
#define PROPTEXT_Relationships_Packages											S"Packages"
#define PROPTEXT_Relationships_Dependency										S"Dependency"
#define PROPTEXT_Relationships_AssetLink										S"AssetLink"
#define PROPTEXT_Relationships_AssetReference									S"AssetReference"
#define PROPTEXT_Relationships_AssetSourceFile									S"SourceFile"
#define PROPTEXT_Relationships_AssetSourceApp									S"SourceApp"


public __gc class MOG_PropertyFactory
{
public:
	__gc class MOG_Relationships
	{
	public:
		static MOG_Property *New_RelationshipAssignment( String *relationship, String *assetName, String *groups, String *objects );
		static MOG_Property *New_PackageAssignment( String *packageName, String *packageGroups, String *packageObjects );
		static MOG_Property *New_PackageAssignment( String *packageName, String *packageGroups, String *packageObjects, bool bForceNoneValue );
		static MOG_Property *MOG_Relationships::New_DependencyAssignment( MOG_Filename *assetFilename );
		static MOG_Property *New_AssetLink( String *linkedFilename, String *packageName, String *packageGroups, String *packageObjects );
		static MOG_Property *New_AssetReference( String *referencedFilename, String *packageName, String *packageGroups, String *packageObjects );
		static MOG_Property *New_AssetSourceFile( String *referencedFilename );
		static MOG_Property *New_AssetSourceApplication( String *referencedApplication );

	};
	__gc class MOG_Asset_StatsProperties
	{
	public:
		static MOG_Property *New_Creator(String * creator);
		static MOG_Property *New_Owner(String *owner);
		static MOG_Property *New_SourceMachine(String *sourceMachine);
		static MOG_Property *New_SourcePath( String *sourcePath );
		static MOG_Property *New_CreatedTime( String *versionTimestamp );
		static MOG_Property *New_ModifiedTime( String *versionTimestamp );
		static MOG_Property *New_RippedTime( String *versionTimestamp );
		static MOG_Property *New_BlessedTime( String *versionTimestamp );
		static MOG_Property *New_PreviousRevision( String *versionTimestamp );
		static MOG_Property *New_Status(MOG_AssetStatusType status);
		static MOG_Property *New_Status(String *status);
		static MOG_Property *New_Size( int size);
		static MOG_Property *New_LastComment( String *comment );
	};
	__gc class MOG_Asset_InfoProperties
	{
	public:
		static MOG_Property *New_Description(String *description);
		static MOG_Property *New_Group(String *group);
		static MOG_Property *New_AssetIcon(String *icon);
		static MOG_Property *New_AssetViewer(String *assetViewer);
		static MOG_Property *New_PropertyMenu(String *propertyMenu);
                static MOG_Property *New_IsPackagedAsset( bool bIsPackagedAsset );
		static MOG_Property *New_IsPackage( bool bIsPackage );
		static MOG_Property *New_IsBuild(bool bIsBuild);
	};
	__gc class MOG_Asset_OptionsProperties
	{
	public:
		static MOG_Property *New_DefaultAssetNameIncludeExtension(String* value);
		static MOG_Property *New_DefaultAssetNamePlatform(String* value);
		static MOG_Property *New_UnBlessable(bool value);
		static MOG_Property *New_UniquePackageAssignmentVerification(bool value);
		static MOG_Property *New_OutofdateVerification(bool value);
		static MOG_Property *New_LocalVerificationBeforeBless(bool value);
		static MOG_Property *New_ValidSlaves(String *validSlaves);
		static MOG_Property *New_MaintainLock(bool value);
		static MOG_Property *New_LockPackageManagement(bool value);
		static MOG_Property *New_RequireLockComment(bool value);
		static MOG_Property *New_ShowPostLockComment(bool value);		
		static MOG_Property *New_UnReferencedRevisionHistory( int numRevisions );
		static MOG_Property *New_BlessTarget(String *blessTarget);
		static MOG_Property *New_BlessEmailNotify(String *blessEmailNotify);
		static MOG_Property *New_ShowLocalModifiedWarning(bool value);
		static MOG_Property *New_AutoLockOnImport(bool value);
	};
	__gc class MOG_Classification_InfoProperties
	{
	public:
		static MOG_Property *New_IsLibrary( bool bIsLibrary );
		static MOG_Property *New_ClassIcon(String *classIcon);
		static MOG_Property *New_FilterInclusions(String *inclusions);
		static MOG_Property *New_FilterExclusions(String *exclusions);
		static MOG_Property *New_PromptUserOnFilterViolation(bool bPrompt);
		static MOG_Property *New_Classification(String *classification);
	};
	__gc class MOG_Sync_OptionsProperties
	{
	public:
		static MOG_Property *New_SyncLabel(String *SyncLabel);
		static MOG_Property *New_SyncTargetPath( String *relativeSyncTargetPath );
		static MOG_Property *New_SyncFiles( bool syncFiles );
		static MOG_Property *New_SyncAsReadOnly( bool syncAsReadOnly );
	};
	__gc class MOG_Rip_OptionsProperties
	{
	public:
		static MOG_Property *New_ShowRipCommandWindow(bool showRipCommandWindow);
		static MOG_Property * New_AssetRipTasker(String *assetRipTasker);
		static MOG_Property *New_AssetRipper(String *asssetRipper);
		static MOG_Property *New_DivergentPlatformDataType(String *devergentPlatformDataType);
		static MOG_Property *New_UseTempRipDir(bool useTempRipDir);
		static MOG_Property *New_UseLocalTempRipDir(bool useLocalTempRipDir);
		static MOG_Property *New_CopyFilesIntoTempRipDir(bool copyFilesIntoTempRipDir);
		static MOG_Property *New_AutoDetectRippedFiles(bool autoDetectRippedFiles);
	};
	__gc class MOG_Package_OptionsProperties
	{
	public:
		static MOG_Property *New_ShowPackageCommandWindow(bool showPackageCommandWindow);
		static MOG_Property *New_DefaultPackageFileExtension(String *defaultPackageFileExtension);
		static MOG_Property *New_PackageStyle(String *packageStyle);
		static MOG_Property *New_PackageWorkspaceDirectory(String *packageWorkspaceDirectory);
		static MOG_Property *New_PackageDataDirectory(String *packageDataDirectory);
		static MOG_Property *New_SyncPackageWorkspaceDirectory(bool syncPackageWorkspaceDirectory);
		static MOG_Property *New_CleanupPackageWorkspaceDirectory(bool cleanupPackageWorkspaceDirectory);
		static MOG_Property *New_AutoPackage( bool bAutoPackage );
		static MOG_Property *New_ExecuteNetworkPackageMerge( bool bExecuteNetworkPackageMerge );
		static MOG_Property *New_ClusterPackaging(bool clusterPackaging);
		static MOG_Property *New_PackagePreMergeEvent(String *packagePreMergeEvent);
		static MOG_Property *New_PackagePostMergeEvent(String *packagePostMergeEvent);
		static MOG_Property *New_InputPackageTaskFile(String *inputPackageTaskFile);
		static MOG_Property *New_OutputPackageTaskFile(String *outputPackageTaskFile);
		static MOG_Property *New_TaskFileTool(String *taskFileTool);
		static MOG_Property *New_PackageCommand_DeletePackageFile(String *deletePackageFileCommand);
		static MOG_Property *New_PackageCommand_ResolveLateResolvers(String *resolveLateResolverCommand);
	};
	__gc class MOG_Package_CommandsProperties
	{
	public:
		static MOG_Property *New_PackageCommand_Propagation(String *scope);
		static MOG_Property *New_PackageCommand_Add(String *addCommand);
		static MOG_Property *New_PackageCommand_Remove(String *removeCommand);
	};
	__gc class MOG_Build_OptionsProperties
	{
	public:
		static MOG_Property *New_ShowBuildCommandWindow(bool showBuildCommandWindow);
		static MOG_Property *New_BuildTool(String *buildTool);
		static MOG_Property *New_BuildWorkingDirectory(String *buildWorkingDirectory);
	};
};
}

using namespace MOG;

#endif	// __MOG_PropertyFactory_H__