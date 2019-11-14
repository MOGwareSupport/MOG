//#define USE_DIRECTSOUND

using System;
using System.Windows.Forms;
using System.IO;
using System.Media;

#if USE_DIRECTSOUND
using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;
#endif

using MOG_Client;
using AppLoading;

using MOG.INI;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.DOSUTILS;
using MOG.REPORT;

namespace MOG_Client.Client_Utilities
{
	/// <summary>
	/// Summary description for Directx_Sound.
	/// </summary>
	public class guiSound
	{
        private SoundPlayer mPlayer;
        private MogMainForm mHandle;
        private MOG_Ini mConfig;
        private string mSoundPath;
        private string mTheme;
        private int mVersion;
        private bool mSoundAvailable;

		public bool Enabled
		{
			get { return mSoundAvailable; }
			set 
			{ 				
				if (value)
				{
					// If we want to enable sound, we need to do through a re-initialization of the sound system
					InitializeSoundEngine();					
				}
				else
				{
					// Disable is as easy as setting to false
					mSoundAvailable = value; 
				}
			}
		}

		private void InitializeSoundEngine()
		{
			try
			{
                // Create an instance of the SoundPlayer class.
                mPlayer = new SoundPlayer();
			}
			catch(Exception e)
			{
				e.ToString();
				mSoundAvailable = false;
				return;
			}

			mSoundAvailable = true;
		}

		public guiSound(MogMainForm handle, SplashForm SplashScreen, string configFilename, string soundClass)
		{
			mTheme = soundClass;
			mHandle = handle;
			mSoundAvailable = true;

			InitializeSoundEngine();			
		
			// Load the sound ini
			string iniPath = Application.StartupPath;
			string configFile = string.Concat(iniPath, "\\", configFilename);
						
			if(DosUtils.FileExist(configFile))
			{
				mConfig = new MOG_Ini(configFile);				
				mSoundPath = string.Concat(iniPath, "\\ClientSounds");

				// Update the sound menu with the scheme choices
				if (mConfig.SectionExist("Themes"))
				{
					// Add a menuItem for each them found in the ini
					for(int i = 0; i < mConfig.CountKeys("Themes"); i++)
					{
						// Assign the click event to each menu item
						ToolStripMenuItem item = new ToolStripMenuItem(mConfig.GetKeyNameByIndexSLOW("Themes", i));
						item.Click += new System.EventHandler(this.SoundMenu_OnClick);
						
						// Set the check mark on the theme that pass passed in to the constructor
						if (string.Compare(item.Text, mTheme, true) == 0)
						{
							item.Checked = true;
						}

						// Add th menu item
						mHandle.themeToolStripMenuItem.DropDownItems.Add(item);						
					}

					// Setup default theme
					if (mTheme == null)
					{
						if (mHandle.themeToolStripMenuItem.DropDownItems.Count > 0)
						{
							mTheme = mHandle.themeToolStripMenuItem.DropDownItems[0].Text;
							ToolStripMenuItem defaultTheme = mHandle.themeToolStripMenuItem.DropDownItems[0] as ToolStripMenuItem;
							defaultTheme.Checked = true;
						}
					}
				}

				// Get our current sounds version
				if (mConfig.SectionExist("Version"))
				{
					mVersion = Convert.ToInt32(mConfig.GetString("SoundsVersion", "Version"));
				}
				else
				{
					mVersion = 0;
				}

				// Get all default sounds
				if (mConfig.SectionExist("SOUNDS"))
				{
					string SourceSoundPath = MOG_ControllerSystem.GetSystemRepositoryPath() + "\\" + mConfig.GetString("Sounds", "Root");
					// Make sure we have a current sound directory
					if (!DosUtils.DirectoryExist(mSoundPath))
					{
						DosUtils.DirectoryCreate(mSoundPath);
					}

					// Make sure we have all the needed sounds
					if (DosUtils.FileExist(string.Concat(SourceSoundPath, "\\version.ini")))
					{
						MOG_Ini soundVersion = new MOG_Ini(string.Concat(SourceSoundPath, "\\version.ini"));

						int sourceVersion = Convert.ToInt32(soundVersion.GetString("SoundsVersion", "Version"));

						if( sourceVersion > mVersion )
						{
							// Update all our sounds
							foreach(FileInfo file in DosUtils.FileGetList(SourceSoundPath, "*.wav"))
							{
								string target = string.Concat(mSoundPath, "\\", file.Name);
								if (DosUtils.FileCopyModified(file.FullName, target))
								{
									SplashScreen.updateSplashNoStep("UPDATING: " + file.Name, 0);
								}
							}

							// Update our version number
							mConfig.PutString("SoundsVersion", "Version", sourceVersion.ToString());
							mConfig.Save();
						}
					}
				}

			}

			// Set the main theme to be the same as this private version
			mHandle.mSoundScheme = mTheme;
		}

		private void SoundMenu_OnClick(object sender, System.EventArgs e)
		{
			// Clear all the checks
			foreach (ToolStripMenuItem Item in mHandle.themeToolStripMenuItem.DropDownItems)
			{
				Item.Checked = false;
			}

			// Assign the name of the theme picked as the current sound theme
			ToolStripMenuItem item = (ToolStripMenuItem)sender;
			mTheme = item.Text;
			item.Checked = true;

			// Set the main theme to be the same as this private version
			mHandle.mSoundScheme = mTheme;
		}

		public void PlaySound(string filename)
		{
            if (!mSoundAvailable) return;

            try
            {
                mPlayer.SoundLocation = filename;
                mPlayer.Load();
				mPlayer.Play();
            }
            catch (Exception e)
            {
                MOG_Report.ReportMessage("Sound Error", e.Message, e.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
                mSoundAvailable = false;
                return;
            }
		}

		public bool Playing()
		{
			if (!mSoundAvailable) return false;

            //return mPlayer.

			return false;
		}
   
		public void PlayStatusSound(string bank, string status)
		{
			if (!mSoundAvailable) return;

			// Create the section name from the theme and back
			string schemeBank = string.Concat(mTheme, ".", bank);

			if(mConfig != null)
			{
				if(mConfig.KeyExist(schemeBank, status))
				{
					// Get the sound fileName and make a full path
					string filename = string.Concat(mSoundPath, "\\", mConfig.GetString(schemeBank, status));
					if(DosUtils.FileExist(filename))
					{
						PlaySound(filename);
					}
				}
			}
		}
	}
}
