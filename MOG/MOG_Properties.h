//--------------------------------------------------------------------------------
//	MOG_Properties.h
//	
//	
//--------------------------------------------------------------------------------

#ifndef __MOG_PROPERTIES_H__
#define __MOG_PROPERTIES_H__

#include "MOG_DosUtils.h"
#include "MOG_Define.h"
#include "MOG_Database.h"
#include "MOG_Filename.h"
#include "MOG_PropertiesIni.h"
#include "MOG_Progress.h"
#include "MOG_UITypeConverters.h"
#include "MOG_Property.h"
#include "MOG_PropertyFactory.h"


using namespace System::ComponentModel;
using namespace MOG::DOSUTILS;
using namespace MOG;
using namespace System::Collections;


// Forward declarations
//#include "MOG_ControllerAsset.h"
namespace MOG {

namespace CONTROLLER {
namespace CONTROLLERASSET {
public __gc class MOG_ControllerAsset;
}
}
}
namespace MOG {
	public __gc class MOG_Tokens;
}


namespace MOG {
namespace PROPERTIES {

public __value enum MOG_InheritedBoolean
{
	False,
	True,
	Inherited,
};

public __value enum MOG_DefaultPrompt {
	No,
	Yes,
	PromptDefaultNo,
	PromptDefaultYes,
	Inherited,
};

public __value enum MOG_PackageStyle {
	Disabled,		// Can't use 'None' because that is a MOG_Properties reserved word
	Simple,
	TaskFile,
	Inherited,
};

public __value enum MOG_PackageCommandPropagation {
	PerRecursiveFile,
	PerRootFile,
	PerAsset,
	Inherited,
};

public __gc class PropertyEventArgs : public EventArgs
{
public:
	String *section, *propertySection, *propertyName, *propertyValue, *scopeName;

	// Constructor for new PropertyEventArgs object
	PropertyEventArgs( String *section, String *propertySection, String *propertyName, String *propertyValue, String *scopeName)
	{
		this->section = section;
		this->propertySection = propertySection;
		this->propertyName = propertyName;
		this->propertyValue = propertyValue;
		this->scopeName = scopeName;
	}
};

public __delegate void PropertyChangedHandler(Object *sender, PropertyEventArgs *e);

	public __gc class MOG_Properties : public ICustomTypeDescriptor
	{
	public:
		
		void OnPropertyChanged( PropertyEventArgs *e );

		// Constructors
		MOG_Properties();
		MOG_Properties(ArrayList *properties);
		MOG_Properties(MOG_Filename *assetFilename);
		MOG_Properties(String *classification);

		static void ActivatePropertiesCache(bool value);

		// Modify Properties
		static MOG_Properties *OpenAssetProperties(MOG_Filename* assetFilename);
		static MOG_Properties *OpenClassificationProperties(String *classification);
		static MOG_Properties *OpenFileProperties(String *propertiesFilename);
		static MOG_Properties *OpenFileProperties(MOG_PropertiesIni *propertiesFile);
		void DisableModification()							{ mCanModify = false; }
		void SetImmeadiateMode(bool value)					{ mImmeadiateMode = value; };
		bool IsModified()									{ return mModified; };
		bool UpdateAssetFilename(MOG_Filename *newAssetFilename, String *newPropertiesFilename);
		MOG_Filename *GetAssetFilename()					{ return mAssetFilename; };
		String *GetClassificationName()						{ return mClassification; };

		bool Save(void);
		// Close must be called if the properties are ever opened
		bool Close(void)									{ return Close(true); };
		bool Close(bool bSave);

		// Inheritance
		bool PopulateInheritance();
		bool PopulateInheritance(String *classification);
		String *GetInheritedPropertyClassification(MOG_Property *inheritedProperty);
		bool DoesPropertyExist(MOG_Property *property);
		bool DoesPropertyExist(String *section, String *propertySection, String *propertyName);
		bool IsPropertyAlreadyInherited(MOG_Property *property);
		bool IsPropertyAlreadyInherited(String *section, String *propertySection, String *propertyName, String *propertyValue);
		bool IsPropertyNotInherited(MOG_Property *property);
		bool IsPropertyNotInherited(String *section, String *propertySection, String *propertyName, String *propertyValue);
		bool RemoveAlreadyInheritedProperties();

		// Scope
		void SetScopeExplicitMode(bool bValue)				{	mScopeExplicitMode = bValue; };
		void SetScope(String *scopeName)					{	mScopeName = scopeName;	};

		// Get Property
		MOG_Property *GetInheritedPropertyFromClassificationsArray(String *section, String *propertySection, String *propertyName);
		MOG_Property *GetProperty(String *section, String *propertySection, String *propertyName);
		String *GetPropertyString(String *section, String *propertySection, String *propertyName);

		// Get Multiple Properties
		ArrayList *GetNonInheritedProperties();
		MOG_PropertiesIni *GetCombinedPropertiesFile(bool includeNonInherited, bool includeInherited);
		ArrayList *GetPropertyList();
		ArrayList *GetPropertyList(bool includeNonInherited, bool includeInherited);
		ArrayList *GetPropertyList(String *section);
		ArrayList *GetPropertyList(String *section, bool includeNonInherited, bool includeInherited);
		ArrayList *GetPropertyList(String *section, String *propertySection);
		ArrayList *GetPropertyList(String *section, String *propertySection, bool includeNonInherited, bool includeInherited);
		ArrayList *GetPropertyList(String *section, String *propertySection, String *scopeName);
		ArrayList *GetApplicableProperties();
		ArrayList *GetApplicableProperties(String *section);
		ArrayList *GetApplicableProperties(String *section, String *propertySection);

		// Remove Property
		bool RemoveProperty(MOG_Property *removeProperty);
		bool RemoveProperty(String *section, String *propertySection, String *propertyName);
		bool RemoveProperty(String *section, String *propertySection, String *propertyName, String *scopeName);

		// Set Property
		bool SetProperty(String *section, String *propertySecion, String *propertyName, String *value);
		bool SetProperty(String *section, String *propertySection, String *propertyName, String *propertyValue, String *scopeName);
		bool SetProperties(ArrayList *properties);

		// Relationships
		bool AddRelationship(MOG_Property *relationshipProperty);
		bool AddRelationships(ArrayList *relationshipProperties);
		bool RemoveRelationship(MOG_Property *relationshipProperty);
		bool RemoveRelationships(ArrayList *relationshipProperties);
		bool RemoveRelationships(String *relationshipText);
		ArrayList *GetApplicableRelationships(String *assetName);

		// Package Relationships
		bool AddPackage(MOG_Property *packageAssignment)		{ return AddRelationship(packageAssignment); };
		bool AddPackages(ArrayList *packageAssignments)			{ return AddRelationships(packageAssignments); };
		bool RemovePackage(MOG_Property *packageAssignment)		{ return RemoveRelationship(packageAssignment); };
		bool RemovePackages(ArrayList *packageAssignments)		{ return RemoveRelationships(packageAssignments); };
		bool RemovePackages()									{ return RemoveRelationships(PROPTEXT_Relationships_Packages); };
		ArrayList *GetPackages(void)							{ return GetPropertyList(PROPTEXT_Relationships, PROPTEXT_Relationships_Packages); };
		ArrayList *GetNonInheritedPackages(void)				{ return GetPropertyList(PROPTEXT_Relationships, PROPTEXT_Relationships_Packages, true, false); };
		ArrayList *GetInheritedPackages(void)					{ return GetPropertyList(PROPTEXT_Relationships, PROPTEXT_Relationships_Packages, false, true); };
		ArrayList *GetApplicablePackages(void)					{ return GetApplicableProperties(PROPTEXT_Relationships, PROPTEXT_Relationships_Packages); };
		ArrayList *GetApplicablePackages(String *assetName)		{ return GetApplicableRelationships(assetName); };
		ArrayList *GetRelationships(void)						{ return GetPropertyList(PROPTEXT_Relationships); };
		ArrayList *GetApplicableRelationships(void)				{ return GetApplicableProperties(PROPTEXT_Relationships); };

		MOG_PropertiesIni *GetNonInheritedPropertiesIni(void)	{ return mProperties; }
		MOG_PropertiesIni *GetInheritedPropertiesIni(void)		{ return mInheritedProperties; }

		// Used to force a refresh of the calling property grid
		static System::Windows::Forms::PropertyGrid *PropertyGrid = NULL;

		// --------------------------------------------------------------------------------
		// Property Getters
		// --------------------------------------------------------------------------------
		// Asset Stats

		[CategoryAttribute(PROPTEXT_AssetStats), 
		DescriptionAttribute("MOG user who last imported the Asset.")]
		__property String *get_Creator();

		[CategoryAttribute(PROPTEXT_AssetStats),
		DescriptionAttribute("MOG user who last blessed this Asset.")]
		__property String *get_Owner();

		[CategoryAttribute(PROPTEXT_AssetStats),
		DescriptionAttribute("Name of the machine where this Asset was last imported from.")]
		__property String *get_SourceMachine();

		[CategoryAttribute(PROPTEXT_AssetStats),
		DescriptionAttribute("Path where this Asset was last imported from.")]
		__property String *get_SourcePath();

		[CategoryAttribute(PROPTEXT_AssetStats),
		DescriptionAttribute("When this Asset was created or imported to a user's mailbox.")]
		__property String *get_CreatedTime();

		[CategoryAttribute(PROPTEXT_AssetStats),
		DescriptionAttribute("When this Asset was last modified or imported to a user's mailbox.")]
		__property String *get_ModifiedTime();

		[CategoryAttribute(PROPTEXT_AssetStats),
		DescriptionAttribute("When this Asset was ripped.")]
		__property String *get_RippedTime();

		[CategoryAttribute(PROPTEXT_AssetStats),
		DescriptionAttribute("When this Asset was blessed or the blessed repository revision of this asset.")]
		__property String *get_BlessedTime();

		[CategoryAttribute(PROPTEXT_AssetStats),
		DescriptionAttribute("The previous revision of this asset.  This property helps determine when a mailbox asset has become out-of-date with the repository.")]
		__property String *get_PreviousRevision();

		[CategoryAttribute(PROPTEXT_AssetStats),
		DescriptionAttribute("Current status of this Asset.")]
		__property String *get_Status();

		[CategoryAttribute(PROPTEXT_AssetStats),
		DescriptionAttribute("Indicates if the asset needs processing.")]
		[BrowsableAttribute( false )]
		__property bool get_IsUnprocessed();

		[CategoryAttribute(PROPTEXT_AssetStats),
		DescriptionAttribute("Size (on disk) of this Asset for the specified platform.")]
		__property String *get_Size();

		[CategoryAttribute(PROPTEXT_AssetStats),
		DescriptionAttribute("Last comment made on this asset.")]
		__property String *get_LastComment();


		// Asset Info
		[CategoryAttribute(PROPTEXT_AssetInfo),
		DescriptionAttribute("A text description of this Asset or Classification.")]
		__property String *get_Description();

		[CategoryAttribute(PROPTEXT_AssetInfo), 
		DescriptionAttribute("A group that can used to better organize your assets.")]
		__property String *get_Group();

		[CategoryAttribute(PROPTEXT_AssetInfo), 
		DescriptionAttribute("The icon file to associate with this Asset.  (This property supports tokenized MOG strings)"),
		Editor(__typeof(MOG::UITYPESEDITORS::MOGToolTypeEditor), __typeof(System::Drawing::Design::UITypeEditor))]
		__property String *get_AssetIcon();

		[CategoryAttribute(PROPTEXT_AssetInfo),
		DescriptionAttribute("The viewer program associated with this Asset or Classification.  (This property supports tokenized MOG strings)"),
		Editor(__typeof(MOG::UITYPESEDITORS::MOGToolTypeEditor), __typeof(System::Drawing::Design::UITypeEditor))]
		__property String *get_AssetViewer();

		[CategoryAttribute(PROPTEXT_AssetInfo), 
		DescriptionAttribute("Indicates which 'Tools\\Property.Menus' file you would like to use for your custom Change Properties Menu."),
		Editor(__typeof(MOG::UITYPESEDITORS::MOGToolTypeEditor), __typeof(System::Drawing::Design::UITypeEditor))]
		__property String *get_PropertyMenu();

		[CategoryAttribute(PROPTEXT_AssetInfo), 
		DescriptionAttribute("Indicates whether or not this Asset is packaged.")]
		__property MOG_InheritedBoolean get_IsPackagedAsset_InheritedBoolean();
		[BrowsableAttribute( false )]
		__property Boolean get_IsPackagedAsset();

		[CategoryAttribute(PROPTEXT_AssetInfo), 
		DescriptionAttribute("Describes whether or not this is a Package Asset.")]
		__property MOG_InheritedBoolean get_IsPackage_InheritedBoolean();
		[BrowsableAttribute( false )]
		__property Boolean get_IsPackage();

		[CategoryAttribute(PROPTEXT_AssetInfo), 
		DescriptionAttribute("Describes whether or not this is a Build Asset.")]
		__property MOG_InheritedBoolean get_IsBuild_InheritedBoolean();
		[BrowsableAttribute( false )]
		__property Boolean get_IsBuild();


		// Asset Options
		[CategoryAttribute(PROPTEXT_AssetOptions), 
		DescriptionAttribute("Specifies whether the default asset name should include the imported file's extension.  This option makes most sense for native (non-ripping) assets.  Assets that require ripping often change their extensions so including the imported file's extension can become confusing later in development.")]
		__property MOG_DefaultPrompt get_DefaultAssetNameIncludeExtension();

		[CategoryAttribute(PROPTEXT_AssetOptions), 
		DescriptionAttribute("Specifies the default asset name platform so the user doesn't have to specify this for every new asset created.  This is only needed for multi-platform projects that wish to have divergent platform-specific assets.")]
		__property String* get_DefaultAssetNamePlatform();

		[CategoryAttribute(PROPTEXT_AssetOptions), 
		DescriptionAttribute("Indicates whether this asset should never be blessed into the project.")]
		__property MOG_InheritedBoolean get_UnBlessable_InheritedBoolean();
		[BrowsableAttribute( false )]
		__property Boolean get_UnBlessable();

		[CategoryAttribute(PROPTEXT_AssetOptions), 
		DescriptionAttribute("Indicates whether to perform a unique package assignment verification on inbox assets before it is blessed.  This helps prevent two assets from getting packaged over the top of each other.")]
		__property MOG_InheritedBoolean get_UniquePackageAssignmentVerification_InheritedBoolean();
		[BrowsableAttribute( false )]
		__property Boolean get_UniquePackageAssignmentVerification();

		[CategoryAttribute(PROPTEXT_AssetOptions), 
		DescriptionAttribute("Indicates whether to perform an out-of-date verification on inbox assets.  This helps prevent accidental overwrites when multiple users need to work with binary files.")]
		__property MOG_InheritedBoolean get_OutofdateVerification_InheritedBoolean();
		[BrowsableAttribute( false )]
		__property Boolean get_OutofdateVerification();

		[CategoryAttribute(PROPTEXT_AssetOptions), 
		DescriptionAttribute("Indicates whether to check the local workspace for the asset before it is blessed.  The resulting warning helps encourage users to test their assets locally before blessing them.")]
		__property MOG_InheritedBoolean get_LocalVerificationBeforeBless_InheritedBoolean();
		[BrowsableAttribute( false )]
		__property Boolean get_LocalVerificationBeforeBless();

		[CategoryAttribute(PROPTEXT_AssetOptions), 
		DescriptionAttribute("Comma delimited list specifying what slave machines are authorized to rip this Asset.  Blank means all slaves are valid.")]
		__property String *get_ValidSlaves();

		[CategoryAttribute(PROPTEXT_AssetOptions), 
		DescriptionAttribute("Maintain lock after CheckIn or Bless.")]
		__property MOG_InheritedBoolean get_MaintainLock_InheritedBoolean();
		[BrowsableAttribute( false )]
		__property Boolean get_MaintainLock();
		
		[CategoryAttribute(PROPTEXT_AssetOptions), 
		DescriptionAttribute("Maintain lock after CheckIn or Bless.")]
		__property MOG_InheritedBoolean get_LockPackageManagement_InheritedBoolean();
		[BrowsableAttribute( false )]
		__property Boolean get_LockPackageManagement();
		
		[CategoryAttribute(PROPTEXT_AssetOptions), 
		DescriptionAttribute("Show the comment form when a lock is requested.")]
		__property MOG_InheritedBoolean get_RequireLockComment_InheritedBoolean();
		[BrowsableAttribute( false )]
		__property Boolean get_RequireLockComment();
		
		[CategoryAttribute(PROPTEXT_AssetOptions), BrowsableAttribute( false ),
		DescriptionAttribute("Show the comment form when locks are released.")]
		__property MOG_InheritedBoolean get_ShowPostLockComment_InheritedBoolean();
		[BrowsableAttribute( false )]
		__property Boolean get_ShowPostLockComment();		

		[CategoryAttribute(PROPTEXT_AssetOptions),
		DescriptionAttribute("Indicates how many unused/unreferenced revisions the system will leave in the MOG Repository before archiving them.")]
		__property String *get_UnReferencedRevisionHistory();

		[CategoryAttribute(PROPTEXT_AssetOptions),
		DescriptionAttribute("Indicates a specific bless target for this asset.")]
		__property String *get_BlessTarget();

		[CategoryAttribute(PROPTEXT_AssetOptions),
		DescriptionAttribute("Indicates an email address or addresses to send bless notifies to.")]
		__property String *get_BlessEmailNotify();

		[CategoryAttribute(PROPTEXT_AssetOptions), 
		DescriptionAttribute("Show a warning message when updating a file that has been modified locally.")]
		__property MOG_InheritedBoolean get_ShowLocalModifiedWarning_InheritedBoolean();
		[BrowsableAttribute( false )]
		__property Boolean get_ShowLocalModifiedWarning();

		[CategoryAttribute(PROPTEXT_AssetOptions), 
		DescriptionAttribute("Automaticaly obtain an asset lock when it is imported into the user's drafts.")]
		__property MOG_InheritedBoolean get_AutoLockOnImport_InheritedBoolean();
		[BrowsableAttribute( false )]
		__property Boolean get_AutoLockOnImport();


		// Classification Info
		[CategoryAttribute(PROPTEXT_ClassificationInfo),
		DescriptionAttribute("Is this asset classified as a Library asset."),
		BrowsableAttribute( false )]
		__property MOG_InheritedBoolean get_IsLibrary_InheritedBoolean();
		[BrowsableAttribute( false )]
		__property Boolean get_IsLibrary();

		[CategoryAttribute(PROPTEXT_ClassificationInfo), 
		DescriptionAttribute("The icon file to associate with this Classification.  (This property supports tokenized MOG strings)"),
		Editor(__typeof(MOG::UITYPESEDITORS::MOGToolTypeEditor), __typeof(System::Drawing::Design::UITypeEditor))]
		__property String *get_ClassIcon();

		[CategoryAttribute(PROPTEXT_ClassificationInfo), 
			DescriptionAttribute("A series of comma delimited patterns denoting what can be included as an asset within this classification.")]
		__property String *get_FilterInclusions();

		[CategoryAttribute(PROPTEXT_ClassificationInfo), 
		DescriptionAttribute("A series of comma delimited patterns denoting what cannot be included as an asset within this classification.")]
		__property String *get_FilterExclusions();

		[CategoryAttribute(PROPTEXT_ClassificationInfo), 
		DescriptionAttribute("Indicates whether or not to prompt the user in the event of a filter violation.")]
		__property MOG_InheritedBoolean get_PromptUserOnFilterViolation_InheritedBoolean();
		[BrowsableAttribute( false )]
		__property Boolean get_PromptUserOnFilterViolation();

//		[CategoryAttribute(PROPTEXT_ClassificationInfo),
//		DescriptionAttribute("Specifies the Classification of this Asset.")]
		[BrowsableAttribute( false )]
		__property String *get_Classification();


		// Sync Options
		[CategoryAttribute(PROPTEXT_SyncOptions), 
			DescriptionAttribute("Specifies the name this tree should be known as when syncing data.")]
		__property String *get_SyncLabel();

		[CategoryAttribute(PROPTEXT_SyncOptions),
		DescriptionAttribute("Where this Asset's files should be synced to when the users updates their local project directory.  (This property supports tokenized MOG strings)")]
		__property String *get_SyncTargetPath();

		[CategoryAttribute(PROPTEXT_SyncOptions), 
		DescriptionAttribute("Indicates whether or not Asset's files should be synced when getting latest data in a local workspace.")]
		__property MOG_InheritedBoolean get_SyncFiles_InheritedBoolean();
		[BrowsableAttribute( false )]
		__property Boolean get_SyncFiles();

		[CategoryAttribute(PROPTEXT_SyncOptions), 
		DescriptionAttribute("Indicates whether or not files should be synced as ReadOnly.")]
		__property MOG_InheritedBoolean get_SyncAsReadOnly_InheritedBoolean();
		[BrowsableAttribute( false )]
		__property Boolean get_SyncAsReadOnly();


		// Rip Options
		[CategoryAttribute(PROPTEXT_RipOptions), 
		DescriptionAttribute("Indicates whether to show the command window when ripping assets.")]
		__property MOG_InheritedBoolean get_ShowRipCommandWindow_InheritedBoolean();
		[BrowsableAttribute( false )]
		__property Boolean get_ShowRipCommandWindow();

		[CategoryAttribute(PROPTEXT_RipOptions),
		DescriptionAttribute("Specifies what command should be executed when breaking this Asset up into multiple rip slave tasks.  (This property supports tokenized MOG strings)"),
		Editor(__typeof(MOG::UITYPESEDITORS::MOGToolTypeEditor), __typeof(System::Drawing::Design::UITypeEditor))]
		__property String *get_AssetRipTasker();

		[CategoryAttribute(PROPTEXT_RipOptions),
		DescriptionAttribute("Specifies what command should be executed when ripping this Asset.  (This property supports tokenized MOG strings)"), 
		Editor(__typeof(MOG::UITYPESEDITORS::MOGToolTypeEditor), __typeof(System::Drawing::Design::UITypeEditor))]
		__property String *get_AssetRipper();

		[BrowsableAttribute( false )]
		__property Boolean get_NativeDataType();

		[CategoryAttribute(PROPTEXT_RipOptions), 
		DescriptionAttribute("Indicates whether or not this Asset requires unique ripping for each platform.  'True' means there will be multiple slave tasks generated so unique data can be prepared for each platform.")]
		__property MOG_InheritedBoolean get_DivergentPlatformDataType_InheritedBoolean();
		[BrowsableAttribute( false )]
		__property Boolean get_DivergentPlatformDataType();

		[CategoryAttribute(PROPTEXT_RipOptions), 
		DescriptionAttribute("Run the ripper from a temporary directory other than the asset source files directory")]
		__property MOG_InheritedBoolean get_UseTempRipDir_InheritedBoolean();
		[BrowsableAttribute( false )]
		__property Boolean get_UseTempRipDir();

		[CategoryAttribute(PROPTEXT_RipOptions), 
		DescriptionAttribute("The temporary ripping directory will be created on the local hard drive")]
		__property MOG_InheritedBoolean get_UseLocalTempRipDir_InheritedBoolean();
		[BrowsableAttribute( false )]
		__property Boolean get_UseLocalTempRipDir();

		[CategoryAttribute(PROPTEXT_RipOptions), 
		DescriptionAttribute("Copy the asset source files into the temporary directory before ripping to preserve source file integrity")]
		__property MOG_InheritedBoolean get_CopyFilesIntoTempRipDir_InheritedBoolean();
		[BrowsableAttribute( false )]
		__property Boolean get_CopyFilesIntoTempRipDir();

		[CategoryAttribute(PROPTEXT_RipOptions), 
		DescriptionAttribute("Automatically detect any files that were created or touched by the ripping process, and identify them as ripped files")]
		__property MOG_InheritedBoolean get_AutoDetectRippedFiles_InheritedBoolean();
		[BrowsableAttribute( false )]
		__property Boolean get_AutoDetectRippedFiles();


		// Package Options
		[CategoryAttribute(PROPTEXT_PackageOptions),
		DescriptionAttribute("Specifies the default extension of package files.")]
		__property String *get_DefaultPackageFileExtension();

		[CategoryAttribute(PROPTEXT_PackageOptions), 
		DescriptionAttribute("Indicates whether to show the command window when packaging assets.")]
		__property MOG_InheritedBoolean get_ShowPackageCommandWindow_InheritedBoolean();
		[BrowsableAttribute( false )]
		__property Boolean get_ShowPackageCommandWindow();

		[CategoryAttribute(PROPTEXT_PackageOptions), 
		DescriptionAttribute("Describes the type of packaging to be used on the asset.")]
		__property MOG_PackageStyle get_PackageStyle();

		[CategoryAttribute(PROPTEXT_PackageOptions),
		DescriptionAttribute("Specifies the workspace directory that the Package Tool will be executed in when packaging this Package.  (This property supports tokenized MOG strings)"),
		Editor(__typeof(MOG::UITYPESEDITORS::PathTypeEditor), __typeof(System::Drawing::Design::UITypeEditor))]
		__property String *get_PackageWorkspaceDirectory();

		[CategoryAttribute(PROPTEXT_PackageOptions),
		DescriptionAttribute("Specifies where the package data can be copied in preparation of a package merge.  (This property supports tokenized MOG strings)"),
		Editor(__typeof(MOG::UITYPESEDITORS::PathTypeEditor), __typeof(System::Drawing::Design::UITypeEditor))]
		__property String *get_PackageDataDirectory();

		[CategoryAttribute(PROPTEXT_PackageOptions), 
		DescriptionAttribute("Indicates whether the 'PackageWorkspaceDirectory' should be synced to the latest project data before each package merge.")]
		__property MOG_InheritedBoolean get_SyncPackageWorkspaceDirectory_InheritedBoolean();
		[BrowsableAttribute( false )]
		__property Boolean get_SyncPackageWorkspaceDirectory();

		[CategoryAttribute(PROPTEXT_PackageOptions), 
		DescriptionAttribute("Indicates whether the 'PackageWorkspaceDirectory' should be cleaned up after each package merge.")]
		__property MOG_InheritedBoolean get_CleanupPackageWorkspaceDirectory_InheritedBoolean();
		[BrowsableAttribute( false )]
		__property Boolean get_CleanupPackageWorkspaceDirectory();

		[CategoryAttribute(PROPTEXT_PackageOptions), 
		DescriptionAttribute("Indicates whether this Package should be automatically scheduled for repackaging when any of its contained Assets are blessed.")]
		__property MOG_InheritedBoolean get_AutoPackage_InheritedBoolean();
		[BrowsableAttribute( false )]
		__property Boolean get_AutoPackage();

		[CategoryAttribute(PROPTEXT_PackageOptions), 
		DescriptionAttribute("Indicates whether to execute network package merges.")]
		__property MOG_InheritedBoolean get_ExecuteNetworkPackageMerge_InheritedBoolean();
		[BrowsableAttribute( false )]
		__property Boolean get_ExecuteNetworkPackageMerge();

		[CategoryAttribute(PROPTEXT_PackageOptions), 
		DescriptionAttribute("Indicates whether multiple packages can be packaged together as a single package event on the same slave.  (This property supports tokenized MOG strings)")]
		__property MOG_InheritedBoolean get_ClusterPackaging_InheritedBoolean();
		[BrowsableAttribute( false )]
		__property Boolean get_ClusterPackaging();

		[CategoryAttribute(PROPTEXT_PackageOptions),
		DescriptionAttribute("Specifies the command to be used in preparation of packaging.  (This property supports tokenized MOG strings)"),
		Editor(__typeof(MOG::UITYPESEDITORS::MOGToolTypeEditor), __typeof(System::Drawing::Design::UITypeEditor))]
		__property String *get_PackagePreMergeEvent();

		[CategoryAttribute(PROPTEXT_PackageOptions),
		DescriptionAttribute("Specifies the command to be used after packaging is completed.  (This property supports tokenized MOG strings)"),
		Editor(__typeof(MOG::UITYPESEDITORS::MOGToolTypeEditor), __typeof(System::Drawing::Design::UITypeEditor))]
		__property String *get_PackagePostMergeEvent();

		[CategoryAttribute(PROPTEXT_PackageOptions),
			DescriptionAttribute("Specifies the input package TaskFile used by the 'TaskFileTool' when packaging.")]
		__property String *get_InputPackageTaskFile();

		[CategoryAttribute(PROPTEXT_PackageOptions),
		DescriptionAttribute("Specifies the output TaskFile created by the 'TaskFileTool' when packaging.")]
		__property String *get_OutputPackageTaskFile();

		[CategoryAttribute(PROPTEXT_PackageOptions),
		DescriptionAttribute("Specifies the command to be used when packaging this Asset's files.  (This property supports tokenized MOG strings)"),
		Editor(__typeof(MOG::UITYPESEDITORS::MOGToolTypeEditor), __typeof(System::Drawing::Design::UITypeEditor))]
		__property String *get_TaskFileTool();

		[CategoryAttribute(PROPTEXT_PackageOptions),
		DescriptionAttribute("Specifies the command to use when deleting the entire Package.  This command is only used when rebuilding a package from scratch.  These commands will be formatted and placed inside the '\\MOG\\PackageCommands.Info' file located in the working directory.  (This property supports tokenized MOG strings)")] 
		__property String *get_PackageCommand_DeletePackageFile();

		[CategoryAttribute(PROPTEXT_PackageOptions),
		DescriptionAttribute("Specifies the command to use when resolving Package Late Resolvers.  This command is used to fix broken links caused by assets missing from packages.  These commands will be formatted and placed inside the '\\MOG\\PackageCommands.Info' file located in the working directory.  (This property supports tokenized MOG strings)")] 
		__property String *get_PackageCommand_ResolveLateResolvers();


		// Packaging Commands
		[CategoryAttribute(PROPTEXT_PackageCommands),
		DescriptionAttribute("Specifies how many times the package commands should be constructed and for what item; Each Asset, Each RootFile or Each RecursiveFile.  If using SimplePackaging, the commands are immediately executed; If using TaskFile Packaging, the commands they will be added to the task file for later processing by the designated packaging tool.")]
		__property MOG_PackageCommandPropagation get_PackageCommand_Propagation();

		[CategoryAttribute(PROPTEXT_PackageCommands),
		DescriptionAttribute("Specifies the command to use when adding an Asset's files to a Package.  These commands will be formatted and placed inside the '\\MOG\\PackageCommands.Info' file located in the working directory.  (This property supports tokenized MOG strings)")]
		__property String *get_PackageCommand_Add();

		[CategoryAttribute(PROPTEXT_PackageCommands),
		DescriptionAttribute("Specifies the command to use when removing an Asset's files from a Package.  These commands will be formatted and placed inside the '\\MOG\\PackageCommands.Info' file located in the working directory.  (This property supports tokenized MOG strings)")] 
		__property String *get_PackageCommand_Remove();


		// Build Options
		[CategoryAttribute(PROPTEXT_BuildOptions), 
		DescriptionAttribute("Indicates whether to show the command window when running builds.")]
		__property MOG_InheritedBoolean get_ShowBuildCommandWindow_InheritedBoolean();
		[BrowsableAttribute( false )]
		__property Boolean get_ShowBuildCommandWindow();

		[CategoryAttribute(PROPTEXT_BuildOptions),
		DescriptionAttribute("Specifies the Build Tool that will be executed when building this Asset.  (This property supports tokenized MOG strings)"), 
		Editor(__typeof(MOG::UITYPESEDITORS::MOGToolTypeEditor), __typeof(System::Drawing::Design::UITypeEditor))]
		__property String *get_BuildTool();

		[CategoryAttribute(PROPTEXT_BuildOptions),
		DescriptionAttribute("Specifies the working directory when building this Asset.  (This property supports tokenized MOG strings)"), 
		Editor(__typeof(MOG::UITYPESEDITORS::PathTypeEditor), __typeof(System::Drawing::Design::UITypeEditor))]
		__property String *get_BuildWorkingDirectory();





		// --------------------------------------------------------------------------------
		// Property Setters
		// --------------------------------------------------------------------------------
		// Asset Stats
		__property void set_Creator(String *value);
		__property void set_Owner(String *value);
		__property void set_SourceMachine(String *value);
		__property void set_SourcePath(String *value);
		__property void set_CreatedTime(String *value);
		__property void set_ModifiedTime(String *value);
		__property void set_RippedTime(String *value);
		__property void set_BlessedTime(String *value);
		__property void set_PreviousRevision(String *value);
		__property void set_Status(String *value);
		__property void set_Size(String *value);
		__property void set_LastComment(String *value);

		// Asset Info
		__property void set_Description(String *value);
		__property void set_Group(String *value);
		__property void set_AssetIcon(String *value);
		__property void set_AssetViewer(String *value);
		__property void set_PropertyMenu(String *value);
		__property void set_IsPackagedAsset(Boolean value);
		__property void set_IsPackagedAsset_InheritedBoolean(MOG_InheritedBoolean value);
		__property void set_IsPackage(Boolean value);
		__property void set_IsPackage_InheritedBoolean(MOG_InheritedBoolean value);
		__property void set_IsBuild(Boolean value);
		__property void set_IsBuild_InheritedBoolean(MOG_InheritedBoolean value);

		// Asset Options
		__property void set_DefaultAssetNameIncludeExtension(MOG_DefaultPrompt value);
		__property void set_DefaultAssetNamePlatform(String* value);
		__property void set_UnBlessable(Boolean value);
		__property void set_UnBlessable_InheritedBoolean(MOG_InheritedBoolean value);
		__property void set_UniquePackageAssignmentVerification(Boolean value);
		__property void set_UniquePackageAssignmentVerification_InheritedBoolean(MOG_InheritedBoolean value);
		__property void set_OutofdateVerification(Boolean value);
		__property void set_OutofdateVerification_InheritedBoolean(MOG_InheritedBoolean value);
		__property void set_LocalVerificationBeforeBless(Boolean value);
		__property void set_LocalVerificationBeforeBless_InheritedBoolean(MOG_InheritedBoolean value);
		__property void set_ValidSlaves(String *value);
		__property void set_MaintainLock(Boolean value);
		__property void set_MaintainLock_InheritedBoolean(MOG_InheritedBoolean value);
		__property void set_LockPackageManagement(Boolean value);
		__property void set_LockPackageManagement_InheritedBoolean(MOG_InheritedBoolean value);
		__property void set_RequireLockComment(Boolean value);
		__property void set_RequireLockComment_InheritedBoolean(MOG_InheritedBoolean value);
		__property void set_ShowPostLockComment(Boolean value);
		__property void set_ShowPostLockComment_InheritedBoolean(MOG_InheritedBoolean value);
		__property void set_UnReferencedRevisionHistory(String *value);
		__property void set_BlessTarget(String *value);
		__property void set_BlessEmailNotify(String *value);
		__property void set_ShowLocalModifiedWarning(Boolean value);
		__property void set_ShowLocalModifiedWarning_InheritedBoolean(MOG_InheritedBoolean value);
		__property void set_AutoLockOnImport(Boolean value);
		__property void set_AutoLockOnImport_InheritedBoolean(MOG_InheritedBoolean value);
		

		// Classification Info
		__property void set_IsLibrary(Boolean value);
		__property void set_IsLibrary_InheritedBoolean(MOG_InheritedBoolean value);
		__property void set_ClassIcon(String *value);
		__property void set_FilterInclusions(String *value);
		__property void set_FilterExclusions(String *value);
		__property void set_PromptUserOnFilterViolation(bool value);
		__property void set_PromptUserOnFilterViolation_InheritedBoolean(MOG_InheritedBoolean value);
		__property void set_Classification(String *value);

		// Sync Options
		__property void set_SyncLabel(String *value);
		__property void set_SyncTargetPath(String *value);
		__property void set_SyncFiles(Boolean value);
		__property void set_SyncFiles_InheritedBoolean(MOG_InheritedBoolean value);
		__property void set_SyncAsReadOnly(Boolean value);
		__property void set_SyncAsReadOnly_InheritedBoolean(MOG_InheritedBoolean value);

		// Rip Options
		__property void set_ShowRipCommandWindow(Boolean value);
		__property void set_ShowRipCommandWindow_InheritedBoolean(MOG_InheritedBoolean value);
		__property void set_AssetRipTasker(String *value);
		__property void set_AssetRipper(String *value);
		__property void set_DivergentPlatformDataType(Boolean value);
		__property void set_DivergentPlatformDataType_InheritedBoolean(MOG_InheritedBoolean value);
		__property void set_UseTempRipDir(Boolean value);
		__property void set_UseTempRipDir_InheritedBoolean(MOG_InheritedBoolean value);
		__property void set_UseLocalTempRipDir(Boolean value);
		__property void set_UseLocalTempRipDir_InheritedBoolean(MOG_InheritedBoolean value);
		__property void set_CopyFilesIntoTempRipDir(Boolean value);
		__property void set_CopyFilesIntoTempRipDir_InheritedBoolean(MOG_InheritedBoolean value);
		__property void set_AutoDetectRippedFiles(Boolean value);
		__property void set_AutoDetectRippedFiles_InheritedBoolean(MOG_InheritedBoolean value);

		// Package Options
		__property void set_DefaultPackageFileExtension(String *value);
		__property void set_ShowPackageCommandWindow(Boolean value);
		__property void set_ShowPackageCommandWindow_InheritedBoolean(MOG_InheritedBoolean value);
		__property void set_PackageStyle(MOG_PackageStyle value);
		__property void set_PackageWorkspaceDirectory(String *value);
		__property void set_PackageDataDirectory(String *value);
		__property void set_SyncPackageWorkspaceDirectory(Boolean value);
		__property void set_SyncPackageWorkspaceDirectory_InheritedBoolean(MOG_InheritedBoolean value);
		__property void set_CleanupPackageWorkspaceDirectory(Boolean value);
		__property void set_CleanupPackageWorkspaceDirectory_InheritedBoolean(MOG_InheritedBoolean value);
		__property void set_AutoPackage(Boolean value);
		__property void set_AutoPackage_InheritedBoolean(MOG_InheritedBoolean value);
		__property void set_ExecuteNetworkPackageMerge(Boolean value);
		__property void set_ExecuteNetworkPackageMerge_InheritedBoolean(MOG_InheritedBoolean value);
		__property void set_ClusterPackaging(Boolean value);
		__property void set_ClusterPackaging_InheritedBoolean(MOG_InheritedBoolean value);
		__property void set_PackagePreMergeEvent(String *value);
		__property void set_PackagePostMergeEvent(String *value);
		__property void set_InputPackageTaskFile(String *value);
		__property void set_OutputPackageTaskFile(String *value);
		__property void set_TaskFileTool(String *value);
		__property void set_PackageCommand_DeletePackageFile(String *value);
		__property void set_PackageCommand_ResolveLateResolvers(String *value);

		// Packaging Commands
		__property void set_PackageCommand_Propagation(MOG_PackageCommandPropagation value);
		__property void set_PackageCommand_Add(String *value);
		__property void set_PackageCommand_Remove(String *value);

		// Build Options
		__property void set_ShowBuildCommandWindow(Boolean value);
		__property void set_ShowBuildCommandWindow_InheritedBoolean(MOG_InheritedBoolean value);
		__property void set_BuildTool(String *value);
		__property void set_BuildWorkingDirectory(String *value);
		


	private:
		String *mSection;
		MOG_Filename *mAssetFilename;
		String *mClassification;
		String *mScopeName;
		bool mScopeExplicitMode;
		bool mCanModify;
		bool mModified;
		bool mImmeadiateMode;
		bool mDBClassification;
		bool mDBInbox;
		bool mDBRepository;

		// Cache related
		static bool gUsePropertiesCache;
		static String *gLastClassification = S"";
		static MOG_PropertiesIni *gLastInheritedProperties = NULL;
		static HybridDictionary *gPropertiesCache = new HybridDictionary();

		MOG_PropertiesIni *mProperties;
		MOG_PropertiesIni *mInheritedProperties;
		ArrayList *mClassificationPropertiesHierarchy;

		void Initialize(MOG_Filename *assetFilename, String *scopeName, String *classification, MOG_PropertiesIni *propertiesFile);

		Boolean ParseBool(String *boolText);
		Boolean ParseBool(String *boolText, bool bDefaultValue);
		MOG_InheritedBoolean ParseInheritedBoolean(String *boolText);
		MOG_InheritedBoolean ParseInheritedBoolean(String *boolText, bool bDefaultValue);
		MOG_DefaultPrompt ParseDefaultPrompt(String *text);
		MOG_PackageCommandPropagation ParsePackageCommandPropagation(String *text);
		MOG_PackageStyle ParsePackageStyle(String *styleText);
		String *MapInheritedBoolean(MOG_InheritedBoolean value);
		String *MapDefaultPrompt(MOG_DefaultPrompt value);
		String *MapPackageCommandPropagation(MOG_PackageCommandPropagation value);
		String *MapPackageStyle(MOG_PackageStyle value);
		void SetTimestamp(String *section, String *propertySection, String *propertyName, String *timestamp);

		bool PushProperties(ArrayList *properties);
		bool PushProperties(MOG_PropertiesIni *propertiesIniFile, ArrayList *properties);

		bool  FixupInheritedPropertiesForAffectedAssets(String *classification, MOG_Property *propertyToUseWithNewValue);
		ArrayList * GetListOfAffectedAssets(String *classification, MOG_Property *currentProperty);
	
	private:
		void FixupInheritedPropertiesForAffectedAssets_Worker(Object* sender, DoWorkEventArgs* e);
		

		// NEW STUFF ************************************************************************************

	public:
		TypeConverter *GetConverter()
		{
			return TypeDescriptor::GetConverter( this, true );
		}

		EventDescriptorCollection *GetEvents(Attribute *attributes[])
		{
			return TypeDescriptor::GetEvents( this, attributes, true );
		}

		EventDescriptorCollection *GetEvents()
		{
			return TypeDescriptor::GetEvents( this, true );
		}

		String *GetComponentName()
		{
			return TypeDescriptor::GetComponentName( this, true );
		}

		Object *GetPropertyOwner(PropertyDescriptor *pd)
		{
			return this;
		}

		AttributeCollection *GetAttributes()
		{
			return TypeDescriptor::GetAttributes( this, true );
		}

		PropertyDescriptorCollection *GetProperties(Attribute *attributes[]);
		PropertyDescriptorCollection *GetProperties( void );	

		Object *GetEditor(Type *editorBaseType)
		{
			return TypeDescriptor::GetEditor( this, editorBaseType, true );
		}

		PropertyDescriptor *GetDefaultProperty()
		{
			return TypeDescriptor::GetDefaultProperty( this, true );
		}

		EventDescriptor *GetDefaultEvent()
		{
			return TypeDescriptor::GetDefaultEvent( this, true );
		}

		String *GetClassName()
		{
			return TypeDescriptor::GetClassName( this, true );
		}
		
		//had to move this down here because it was jacking up intelisense
		__event PropertyChangedHandler *PropertyChanged;
	};

}
}

using namespace MOG::PROPERTIES;
using namespace MOG::UITYPESEDITORS;

#endif	// __MOG_PROPERTIES_H__
