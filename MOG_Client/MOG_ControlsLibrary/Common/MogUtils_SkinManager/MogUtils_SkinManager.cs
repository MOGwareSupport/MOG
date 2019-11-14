using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;

using MOG.DOSUTILS;
using MOG.INI;
using MOG.PROMPT;

namespace MOG_ControlsLibrary.Common
{
	/// <summary>
	/// Summary description for MogUtil_Skin.
	/// </summary>
	public class MogUtils_SkinManager
	{
		ArrayList mSkins;
		ArrayList mSkinNames;
		ArrayList mSections;
		ArrayList mSectionNames;
		MOG_PropertiesIni mSkinDef;

		public int GetImageIndex(string section, string key)
		{
			for (int index = 0; index < mSectionNames.Count; index ++)
			{
				string sectionName = (string)mSectionNames[index];

				if (string.Compare(sectionName, section, true) == 0)
				{
					ArrayList SkinNames = (ArrayList)mSkinNames[index];

					for (int imageIndex = 0; imageIndex < SkinNames.Count; imageIndex++)
					{
						string skinName = (string)SkinNames[imageIndex];
						if (string.Compare(skinName, key, true) == 0)
						{
							return imageIndex;
						}
					}
				}				
			}

			return -1;
		}

		public void DownloadSkins()
		{
			if (DosUtils.DirectoryExist(MOG.MOG_Main.GetExecutablePath() + "\\Skin") == false)
			{
				DosUtils.DirectoryCreate(MOG.MOG_Main.GetExecutablePath() + "\\Skin");
			}

			string sourcePath = MOG.CONTROLLER.CONTROLLERSYSTEM.MOG_ControllerSystem.GetSystemRepositoryPath() + "\\Updates\\Skins";
			string targetPath = MOG.MOG_Main.GetExecutablePath() + "\\Skin";

			if(DosUtils.DirectoryExist(sourcePath))
			{
				// Copy all the files down
				FileInfo []files = DosUtils.FileGetList(sourcePath, "*.*");
				foreach (FileInfo file in files)
				{
					DosUtils.FileCopyModified(file.FullName, targetPath + "\\" + file.Name);
				}
			}			
		}

		public Image GetImage(string section, string key)
		{
			for (int index = 0; index < mSectionNames.Count; index ++)
			{
				string sectionName = (string)mSectionNames[index];

				if (string.Compare(sectionName, section, true) == 0)
				{
					ArrayList SkinNames = (ArrayList)mSkinNames[index];
					ArrayList Skins = (ArrayList)mSkins[index];

					for (int imageIndex = 0; imageIndex < SkinNames.Count; imageIndex++)
					{
						string skinName = (string)SkinNames[imageIndex];
						if (string.Compare(skinName, key, true) == 0)
						{
							return (Image)Skins[imageIndex];
						}
					}
				}				
			}

			return null;
		}

		public MogUtils_SkinManager(string skinDefinitionFile)
		{
			mSkins = new ArrayList();
			mSkinNames = new ArrayList();
			mSections = new ArrayList();
			mSectionNames = new ArrayList();
		
			if (DosUtils.FileExist(skinDefinitionFile))
			{
				mSkinDef = new MOG_PropertiesIni(skinDefinitionFile);
			}
			else
			{
				ConstructDefaultSkin(skinDefinitionFile);
			}
		}

		private void ConstructDefaultSkin(string filename)
		{
			mSkinDef = new MOG_PropertiesIni(filename);
			mSkinDef.PutString("Workspace","{Inbox}Button.Rest", "Skin\\InboxButton(Rest).png");
			mSkinDef.PutString("Workspace","{Inbox}Button.Hover", "Skin\\InboxButton(Hover).png");
			mSkinDef.PutString("Workspace","{Inbox}Button.Pressed", "Skin\\InboxButton(Pressed).png");
			mSkinDef.PutString("Workspace","{Inbox}BackgroundBarMiddle", "Skin\\WideBar_Middle.png");
			mSkinDef.PutString("Workspace","{Inbox}BackgroundBarEnd", "Skin\\WideBar_End.png");
			mSkinDef.PutString("Workspace","{MyTasks}Button.Rest", "Skin\\TasksButton(Rest).png");
			mSkinDef.PutString("Workspace","{MyTasks}Button.Hover", "None");
			mSkinDef.PutString("Workspace","{MyTasks}Button.Pressed", "None");
			mSkinDef.PutString("Workspace","{MyTasks}BackgroundBarMiddle", "Skin\\ThinBar_Middle.png");
			mSkinDef.PutString("Workspace","{MyTasks}BackgroundBarEnd", "Skin\\ThinBar_End.png");
			mSkinDef.PutString("Workspace","{LocalExplorer}Button.Rest", "Skin\\LocalExplorerButton(Rest).png");
			mSkinDef.PutString("Workspace","{LocalExplorer}Button.Hover", "None");
			mSkinDef.PutString("Workspace","{LocalExplorer}Button.Pressed", "None");
			mSkinDef.PutString("Workspace","{LocalExplorer}BackgroundBarMiddle", "Skin\\ThinBar_Middle.png");
			mSkinDef.PutString("Workspace","{LocalExplorer}BackgroundBarEnd", "Skin\\ThinBar_End.png");
			mSkinDef.PutString("Workspace","{MyWorkspace}Button.Rest", "Skin\\WorkspaceButton(Rest).png");
			mSkinDef.PutString("Workspace","{MyWorkspace}Button.Hover", "Skin\\WorkspaceButton(Hover).png");
			mSkinDef.PutString("Workspace","{MyWorkspace}Button.Pressed", "Skin\\WorkspaceButton(Pressed).png");
			mSkinDef.PutString("Workspace","{MyWorkspace}BackgroundBarMiddle", "Skin\\ThinBar_Middle.png");
			mSkinDef.PutString("Workspace","{MyWorkspace}BackgroundBarEnd", "Skin\\ThinBar_End.png");
			mSkinDef.PutString("Workspace","{MyTools}Button.Rest", "Skin\\ToolBoxButton(Rest).png");
			mSkinDef.PutString("Workspace","{MyTools}Button.Hover", "Skin\\ToolBoxButton(Hover).png");
			mSkinDef.PutString("Workspace","{MyTools}Button.Pressed", "Skin\\ToolBoxButton(Pressed).png");
			mSkinDef.PutString("Workspace","{MyTools}BackgroundBarMiddle", "Skin\\ThinBar_Middle.png");
			mSkinDef.PutString("Workspace","{MyTools}BackgroundBarEnd", "Skin\\ThinBar_End.png");
			mSkinDef.Save();
		}

		public void Load(string section)
		{
			if (mSkinDef != null)
			{
				if (mSkinDef.SectionExist(section))
				{
					ArrayList SkinNames = new ArrayList();		
					ArrayList Skins = new ArrayList();
		
					for (int i = 0; i < mSkinDef.CountKeys(section); i++)
					{
						string label = mSkinDef.GetKeyNameByIndexSLOW(section, i);
						string file = mSkinDef.GetKeyByIndexSLOW(section, i);
						string fullFilename = "";

						if (Path.IsPathRooted(file))
						{
							fullFilename = file;
						}
						else
						{
							fullFilename = MOG.MOG_Main.GetExecutablePath() + "\\" + file;
						}

					LoadImage:

						if (string.Compare(file, "none", true) != 0)
						{
							if (DosUtils.FileExist(fullFilename))
							{
								// Get the group image
								Image myImage = new Bitmap(fullFilename);

								// Add the image and the type to the arrayLists
								Skins.Add(myImage);
								SkinNames.Add(label);
							}
							else
							{
								switch(MOG_Prompt.PromptResponse("Custom skin", "Skin label:\n" + label + "\nSkin bitmap:\n" + fullFilename + "+\nCould not be found or is missing! This image will be nullified.", MOGPromptButtons.RetryCancel))
								{
									case MOGPromptResult.Retry:
										goto LoadImage;
								}
							}
						}
					}

					mSections.Add(SkinNames);
					mSectionNames.Add(section);
					mSkins.Add(Skins);
					mSkinNames.Add(SkinNames);
				}
			}
		}
	}
}
