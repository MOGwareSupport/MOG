//
//	NEW CODE - This has not been finished!!!
//	Initial Attempt to revisit the MOG string tokens.
//	Initial problems deal with how to allow this to work within the MOG.dll w/o causing a circular reference?
//


using System;
using System.Collections.Generic;
using System.Text;

using MOG;
using MOG.FILENAME;
using MOG.PROPERTIES;
using MOG.CONTROLLER.CONTROLLERPACKAGE;
using MOG.CONTROLLER.CONTROLLERASSET;
using System.Collections;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERREPOSITORY;


namespace MOG_CoreControls.Utils
{
	class MOG_Tokens
	{
		// Repository Tokens
		public const string TOKEN_Repository_Path						= "{Repository.Path}";
		public const string TOKEN_Repository_ProjectsPath				= "{Repository.ProjectsPath}";
		public const string TOKEN_Repository_ToolsPath					= "{Repository.ToolsPath}";
		public const string TOKEN_Repository_Project_Path				= "{Repository.Project.Path}";
		public const string TOKEN_Repository_Project_ToolsPath			= "{Repository.Project.ToolsPath}";
		public const string TOKEN_Repository_Project_AssetsPath			= "{Repository.Project.AssetsPath}";
		public const string TOKEN_Repository_Project_ArchivePath		= "{Repository.Project.ArchivePath}";
		public const string TOKEN_Repository_Project_UsersPath			= "{Repository.Project.UsersPath}";

		// Project Tokens
		public const string TOKEN_Project_Name							= "{Project.Name}";
		public const string TOKEN_Project_BranchName					= "{Project.BranchName}";
		public const string TOKEN_Project_UserName						= "{Project.UserName}";
		public const string TOKEN_Project_PlatformName					= "{Project.PlatformName}";
		public const string TOKEN_Project_WorkspaceDirectory			= "{Project.WorkspaceDirectory}";

		// Ripper Tokens
		public const string TOKEN_Ripper_SourcePath						= "{Ripper.SourcePath}";
		public const string TOKEN_Ripper_SourceFilePattern				= "{Ripper.SourceFilePattern}";
		public const string TOKEN_Ripper_DestinationPath				= "{Ripper.DestinationPath}";

		// Package Tokens
		public const string TOKEN_Package_WorkspaceDirectory			= "{Package.WorkspaceDirectory}";
		public const string TOKEN_Package_DataDirectory					= "{Package.DataDirectory}";
		public const string TOKEN_Package_PackageFile_FullName			= "{Package.PackageFile.FullName}";
		public const string TOKEN_Package_PackageFile_Classification	= "{Package.PackageFile.Classification}";
		public const string TOKEN_Package_PackageFile_Label				= "{Package.PackageFile.Label}";
		public const string TOKEN_Package_PackageFile_Platform			= "{Package.PackageFile.Platform}";
		public const string TOKEN_Package_PackageFile_Group				= "{Package.PackageFile.PackageGroup}";
		public const string TOKEN_Package_PackageFile_Object			= "{Package.PackageFile.PackageObject}";
		public const string TOKEN_Package_PackageFile_Filename			= "{Package.PackageFile.Filename}";

		// Inbox Tokens
		public const string TOKEN_Inbox_UserName						= "{Inbox.UserName}";
		public const string TOKEN_Inbox_UserPath						= "{Inbox.UserPath}";
		public const string TOKEN_Inbox_BoxName							= "{Inbox.BoxName}";
		public const string TOKEN_Inbox_BoxPath							= "{Inbox.BoxPath}";

		// Asset Tokens
		public const string TOKEN_Asset_AssetName_Path					= "{Asset.AssetName.Path}";
		public const string TOKEN_Asset_AssetName_FullName				= "{Asset.AssetName.FullName}";
		public const string TOKEN_Asset_AssetName_Classification		= "{Asset.AssetName.Classification}";
		public const string TOKEN_Asset_AssetName_Name					= "{Asset.AssetName.Name}";
		public const string TOKEN_Asset_AssetName_PlatformName			= "{Asset.AssetName.PlatformName}";
		public const string TOKEN_Asset_AssetName_Label					= "{Asset.AssetName.Label}";

		public const string TOKEN_Asset_ImportedFile					= "{Asset.ImportedFile}";
		public const string TOKEN_Asset_RippedFile						= "{Asset.RippedFile}";

		public const string TOKEN_Asset_Property						= "{Asset.Property}";
		public const string TOKEN_Asset_ClassificationPath				= "{Asset.ClassificationPath}";
		public const string TOKEN_Asset_VersionTimeStamp				= "{Asset.VersionTimeStamp}";

		// Commands
		public const string TOKEN_Drive									= "Drive(<string>)";
		public const string TOKEN_Path									= "Path(<string>)";
		public const string TOKEN_Filename								= "Filename(<string>)";
		public const string TOKEN_Extension								= "Extension(<string>)";
		public const string TOKEN_NoExtension							= "NoExtension(<string>)";
		public const string TOKEN_Length								= "Length(<string>)";
		public const string TOKEN_SubString								= "SubString(<pos>, <len>)";
		public const string TOKEN_Replace								= "Replace(<string>, <string>)";
		public const string TOKEN_Trim									= "Trim(<chars>)";
		public const string TOKEN_TrimStart								= "TrimStart(<chars>)";
		public const string TOKEN_TrimEnd								= "TrimEnd(<chars>)";


		// Seeded values
		MOG_Filename mAssetFilename = new MOG_Filename();
		MOG_Properties mProperties = null;
		MOG_ControllerAsset mAsset = null;
		string mPackageAssignment = "";
		MOG_PackageMerge_PackageFileInfo mPackageFileInfo = null;
		string mRipperSourcePath = "";
		string mRipperSourceFilePattern = "";
		string mRipperDestinationPath = "";

		// Constructors
		MOG_Tokens(MOG_Filename assetFilename)
		{
			mAssetFilename = assetFilename;
		}

		MOG_Tokens(MOG_Filename assetFilename, ArrayList properties)
		{
			mAssetFilename = assetFilename;
			mProperties = new MOG_Properties(properties);
		}

		MOG_Tokens(MOG_Filename assetFilename, ArrayList properties, string packageAssignment)
		{
			mAssetFilename = assetFilename;
			mProperties = new MOG_Properties(properties);
			mPackageAssignment = packageAssignment;
		}

		MOG_Tokens(MOG_Filename assetFilename, MOG_PackageMerge_PackageFileInfo packageFileInfo)
		{
			mAssetFilename = assetFilename;
			mPackageFileInfo = packageFileInfo;
		}

		MOG_Tokens(MOG_ControllerAsset asset)
		{
			mAsset = asset;
			mAssetFilename = asset.GetAssetFilename();
			mProperties = asset.GetProperties();
		}

		string FormatString(string tokenizedString)
		{
			// Is tokenizedString token free?
			if (!DoesTokenExist(tokenizedString))
			{
				return tokenizedString;
			}

			string formattedString = "";

			// Parse the tokenizedString
			for (int i = 0; i < tokenizedString.Length;)
			{
				char thisChar = tokenizedString[i];

				// Check if this is the beginning of a token
				if (thisChar == '{')
				{
					// Attempt to extract the token
					string token = tokenizedString.Substring(i);
					int endTokenPos = token.IndexOf("}");
					if (endTokenPos != -1)
					{
						// Get the token including the end brace
						token = token.Substring(0, endTokenPos + 1);

						// Get the value for this token
						string value = ResolveToken(token);
						if (value.Length > 0)
						{
							// Add the seedValue onto our string
							formattedString += value;
						}
						else
						{
							// Add back the unresolved token because we found no valur for this token
							formattedString += token;
						}

						// Skip the token's length
						i += token.Length;
						continue;
					}
				}

				// Add the character back to the formattedString
				formattedString += thisChar;
				i++;
			}

			return formattedString;
		}

		bool DoesTokenExist(string tokenizedString)
		{
			// Check if this string contain both the beginning and ending token identifiers?
			if (tokenizedString.Contains("{") &&
				tokenizedString.Contains("}"))
			{
				return true;
			}

			return false;
		}

		string ResolveToken(string token)
		{
			string value = "";

			// Make sure this token starts with the '{'?
			if (token.StartsWith("{"))
			{
				// Get the name of this token
				string[] parts = token.Split("{}.".ToCharArray(), 3);
				// Make sure this resembled a real token?
				if (parts.Length == 3)
				{
					// Check for any contained commands?
					string testToken = "{" + parts[1] + "}";
					// Determine which token we have?
					switch(testToken)
					{
						// Repository Tokens
						case TOKEN_Repository_Path:
							value = MOG_ControllerSystem.GetSystemRepositoryPath();
							break;
						case TOKEN_Repository_ProjectsPath:
							value = MOG_ControllerSystem.GetSystemProjectsPath();
							break;
						case TOKEN_Repository_ToolsPath:
							value = MOG_ControllerSystem.GetSystemRepositoryPath() + "\\Tools";
							break;
						case TOKEN_Repository_Project_Path:
							value = MOG_ControllerProject.GetProjectPath();
							break;
						case TOKEN_Repository_Project_ToolsPath:
							value = MOG_ControllerProject.GetProjectPath() + "\\Tools";
							break;
						case TOKEN_Repository_Project_AssetsPath:
							value = MOG_ControllerRepository.GetRepositoryPath();
							break;
						case TOKEN_Repository_Project_ArchivePath:
							value = MOG_ControllerRepository.GetArchivePath();
							break;
						case TOKEN_Repository_Project_UsersPath:
							value = MOG_ControllerProject.GetProjectPath() + "\\Users";
							break;

						// Project Tokens
						case TOKEN_Project_Name:
							value = MOG_ControllerProject.GetProjectName();
							break;
						case TOKEN_Project_BranchName:
							value = MOG_ControllerProject.GetBranchName();
							break;
						case TOKEN_Project_UserName:
							value = MOG_ControllerProject.GetUserName_DefaultAdmin();
							break;
						case TOKEN_Project_PlatformName:
							value = MOG_ControllerProject.GetPlatformName();
							break;
						case TOKEN_Project_WorkspaceDirectory:
							value = MOG_ControllerProject.GetWorkspaceDirectory();
							break;

						// Ripper Tokens
						case TOKEN_Ripper_SourcePath:
							value = mRipperSourcePath;
							break;
						case TOKEN_Ripper_SourceFilePattern:
							value = mRipperSourceFilePattern;
							break;
						case TOKEN_Ripper_DestinationPath:
							value = mRipperDestinationPath;
							break;

						// Package Tokens
						case TOKEN_Package_WorkspaceDirectory:
							if (mPackageFileInfo != null)
							{
								value = mPackageFileInfo.mPackageWorkspaceDirectory;
							}
							break;
						case TOKEN_Package_DataDirectory:
							if (mPackageFileInfo != null)
							{
								value = mPackageFileInfo.mPackageDataDirectory;
							}
							break;
						case TOKEN_Package_PackageFile_Filename:
							if (mPackageFileInfo != null)
							{
								value = mPackageFileInfo.mPackageFile;
							}
							break;
						case TOKEN_Package_PackageFile_FullName:
							{
								MOG_Filename packageFilename = (mPackageFileInfo != null) ? mPackageFileInfo.mPackageAssetFilename : new MOG_Filename(MOG_ControllerPackage.GetPackageName(mPackageAssignment));
								value = packageFilename.GetAssetFullName();
							}
							break;
						case TOKEN_Package_PackageFile_Classification:
							{
								MOG_Filename packageFilename = (mPackageFileInfo != null) ? mPackageFileInfo.mPackageAssetFilename : new MOG_Filename(MOG_ControllerPackage.GetPackageName(mPackageAssignment));
								value = packageFilename.GetAssetClassification();
							}
							break;
						case TOKEN_Package_PackageFile_Label:
							{
								MOG_Filename packageFilename = (mPackageFileInfo != null) ? mPackageFileInfo.mPackageAssetFilename : new MOG_Filename(MOG_ControllerPackage.GetPackageName(mPackageAssignment));
								value = packageFilename.GetAssetLabel();
							}
							break;
						case TOKEN_Package_PackageFile_Platform:
							{
								MOG_Filename packageFilename = (mPackageFileInfo != null) ? mPackageFileInfo.mPackageAssetFilename : new MOG_Filename(MOG_ControllerPackage.GetPackageName(mPackageAssignment));
								value = packageFilename.GetAssetPlatform();
							}
							break;
						case TOKEN_Package_PackageFile_Group:
							value = MOG_ControllerPackage.GetPackageGroups(mPackageAssignment);
							break;
						case TOKEN_Package_PackageFile_Object:
							value = MOG_ControllerPackage.GetPackageObjects(mPackageAssignment);
							break;

						// Inbox Tokens
						case TOKEN_Inbox_UserName:
							value = mAssetFilename.GetUserName();
							break;
						case TOKEN_Inbox_UserPath:
							value = mAssetFilename.GetUserPath();
							break;
						case TOKEN_Inbox_BoxName:
							value = mAssetFilename.GetBoxName();
							break;
						case TOKEN_Inbox_BoxPath:
							value = mAssetFilename.GetBoxPath();
							break;

						// Asset Tokens
						case TOKEN_Asset_AssetName_Path:
							value = mAssetFilename.GetPath();
							break;
						case TOKEN_Asset_AssetName_FullName:
							value = mAssetFilename.GetAssetFullName();
							break;
						case TOKEN_Asset_AssetName_Classification:
							value = mAssetFilename.GetAssetClassification();
							break;
						case TOKEN_Asset_AssetName_Name:
							value = mAssetFilename.GetAssetName();
							break;
						case TOKEN_Asset_AssetName_PlatformName:
							value = mAssetFilename.GetAssetPlatform();
							break;
						case TOKEN_Asset_AssetName_Label:
							value = mAssetFilename.GetAssetLabel();
							break;

						case TOKEN_Asset_ImportedFile:
						case TOKEN_Asset_RippedFile:
							value = ResolveToken_AssetFile(token);
							break;

						case TOKEN_Asset_Property:
							value = ResolveToken_Property(token);
							break;
						case TOKEN_Asset_ClassificationPath:
							value = MOG_Filename.GetClassificationPath(mAssetFilename.GetAssetClassification());
							break;
						case TOKEN_Asset_VersionTimeStamp:
							value = mAssetFilename.GetVersionTimeStamp();
							break;
					}

					// Check if we have a command?
					if (parts[2] != ".")
					{
					}
				}
			}

			return value;
		}

		string ResolveToken_AssetFile(string token)
		{
			string value = "";

			return value;
		}

		string ResolveToken_Property(string token)
		{
			string value = "";

			return value;
		}
	}
}
