using System;
using System.Collections;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace MOG_Client_Loader
{
	public class MOG_IniKey 
	{
		public string mKey;
		public string mValue;

		public MOG_IniKey()
		{
			mKey = ""; 
			mValue = "";
		}
	}

	public class MOG_IniSection 
	{
		public string mSection;
		public ArrayList mKeys;
		
		public MOG_IniSection()
		{
			mSection = ""; 
			mKeys = new ArrayList();	
		}
	}


	/// <summary>
	/// Summary description for MOG_Ini.
	/// </summary>
	public class MOG_Ini
	{
		enum IniHandle {ClearHandle, HoldHandle};
		enum ParseState
		{
			STATE_PARSE_BEGIN,	// start of line (any can follow)
			STATE_PARSE_SECTION,	// inside a section
			STATE_PARSE_KEY,		// inside a key
			STATE_PARSE_END,	// end of line (no more after this) // after a section name usually
		};

		#region Private data
		private string			mFullFilename;
		private bool			mChanged;
		private ArrayList		mSections;

		private MOG_IniSection	mSectionCache;
		private MOG_IniKey		mKeyCache;

		private FileStream		mLockedFileStream;
		private IniHandle		mHandle;
		#endregion

		public MOG_Ini(string fullFilename)
		{
			mChanged = false;
			mFullFilename = "";
			mSections = new ArrayList();
			mLockedFileStream = null;
			mHandle = IniHandle.ClearHandle;
			Load(fullFilename);
		}

		public MOG_Ini()
		{
			mSections = new ArrayList();
			mFullFilename = "";
			mSectionCache = new MOG_IniSection();
			mKeyCache = new MOG_IniKey();
			mChanged = false;
			mHandle = IniHandle.ClearHandle;
			mLockedFileStream = null;
		}

		public bool Load(string fullFilename)
		{
			Close();

			// Clear previous member data
			if(mSections.Count != 0)
				mSections.Clear();

			mSectionCache = new MOG_IniSection();
			mKeyCache = new MOG_IniKey();

			mFullFilename = fullFilename;

			return LoadFile(mFullFilename, FileAccess.Read, FileShare.Read );
		}

		public bool Open(string fullFilename, FileShare shareLevel)
		{
			Close();

			// Clear previous member data
			if(mSections.Count != 0)
				mSections.Clear();

			mSectionCache = new MOG_IniSection();
			mKeyCache = new MOG_IniKey();

			mFullFilename = fullFilename;

			mHandle = IniHandle.HoldHandle;

			return LoadFile(fullFilename, FileAccess.ReadWrite, shareLevel);
		}

		public bool Save()
		{
			if (mFullFilename != "" && mFullFilename.Length != 0)
				return Save(mFullFilename);
			else 
				return false;
		}

		public bool Save(string fullFilename)
		{
			string outLine = "";
			MOG_IniSection sec;
			MOG_IniKey key;

			if (mChanged || (string.Compare(mFullFilename, fullFilename, true) != 0))
			{
				mFullFilename = fullFilename;

				FileInfo file = new FileInfo(fullFilename);
		
				// Check if target exists
				if (!file.Exists)
				{
					try
					{
						// Check if target has a dir
						if (fullFilename.LastIndexOf("\\") != -1)
						{
							// verify the target dir exists
							DirectoryInfo targetDir = new DirectoryInfo(fullFilename.Substring(0, fullFilename.LastIndexOf("\\")));

							// Check if target dir exists
							if (!targetDir.Exists)
							{
								targetDir.Create();
							}
						}
					}
					catch(Exception e)
					{
						//MessageBox.Show("MOG_Ini.Save Error ", string.Concat( mFullFilename, "\nCould not Create directory (", e.ToString(), ")"), MOGPromptButtons.OK);
						MessageBox.Show("MOG_Ini.Save Error ", string.Concat( mFullFilename, "\nCould not Create directory (", e.ToString(), ")"), MessageBoxButtons.OK);
					}
				}

				//FileStream  fs;
				StreamWriter sw = null;
				int openCounter = 0;

				while(sw == null)
				{
					try
					{
						if (mLockedFileStream == null) 
						{
							mLockedFileStream = new FileStream(fullFilename, FileMode.Create, FileAccess.Write, FileShare.None);
						}
						else
						{
							mLockedFileStream.Seek(0, SeekOrigin.Begin);
						}
						sw = new StreamWriter(mLockedFileStream);
					}
					catch
					{
						// KLK - I am not suer how to get to what actually happened.  For now I will assume that it was a sharing violation.
						// I dont like this :(
						Thread.Sleep(100);

						openCounter++;

						if (openCounter >= 10)
						{
							string title = "File Sharing Violation";
							string message = "Application: " + Path.GetFileName(Application.ExecutablePath) + " - MOG_Ini.Save\n" + 
											 "File: " + fullFilename + "\n" + 
											 "\n" + 
											 "Close any open files that are associated with this asset.";
							DialogResult rc = MessageBox.Show(title, message, MessageBoxButtons.RetryCancel);
							if (rc != DialogResult.Retry)
							{
								return false;
							}
							else
							{
								openCounter = 0;
							}
						}
					}
				}

				// Check for proper create or load
				if(sw != null)
				{
					// Walk through all the Sections in the file
					for (int s = 0; s < mSections.Count; s++)
					{
						sec = (MOG_IniSection)mSections[s];

						outLine = string.Concat("\r\n[", sec.mSection, "]");
						sw.WriteLine(outLine);
				
						// Walk through the Keys in this Section
						for (int t = 0; t < sec.mKeys.Count; t++)
						{
							key = (MOG_IniKey)(sec.mKeys[t]);

							if (key.mValue.Length != 0)
							{
								outLine = string.Concat(key.mKey, "=", key.mValue);
								sw.WriteLine(outLine);
							}
							else
							{
								outLine = string.Concat(key.mKey);
								sw.WriteLine(outLine);
							}
						}
					}
					Int64 pos = mLockedFileStream.Position;
					mLockedFileStream.SetLength(pos);
					mChanged = false;
					mLockedFileStream.Flush();
					sw.Close();
					if ( mHandle == IniHandle.ClearHandle)
					{
						// Close the StreamWriter who will close the FileStream for us.
						mLockedFileStream.Close();
						mLockedFileStream = null;
					}

					return true;
				}

				//MessageBox.Show("MOG_Ini.Save ", string.Concat(mFullFilename, "\nCannot Save File", fullFilename), MOGPromptButtons.OK);	
				return false;
			}

			return true;
		}
	
		private bool LoadFile(string fullFilename, FileAccess openLevel, FileShare shareLevel)
		{
			FileInfo file;
			try
			{
				//FileStream  fs;
				StreamReader read = null;
				int openCounter = 0;

				while (read == null)
				{
					try
					{
						// Make sure file exists
						file = new FileInfo(fullFilename);
						if(!file.Exists)
						{
							return false;
						}

						// Open the file
						if(mLockedFileStream == null)
						{
							mLockedFileStream = new FileStream(fullFilename, FileMode.Open, openLevel, shareLevel);
						}
						read = new StreamReader(mLockedFileStream);
					}
					catch
					{
						// KLK - I am not suer how to get to what actually happened.  For now I will assume that it was a sharing violation.
						// I dont like this :(
						Thread.Sleep(100);
			
						openCounter++;

						if (openCounter >= 10)
						{
							string title = "File Sharing Violation";
							string message = "Application: " + Path.GetFileName(Application.ExecutablePath) + " - MOG_Ini.LoadFile\n" + 
											 "File: " + fullFilename + "\n" + 
											 "\n" + 
											 "Close any open files that are associated with this asset.";
							DialogResult rc = MessageBox.Show(title, message, MessageBoxButtons.RetryCancel);
							if (rc != DialogResult.Retry)
							{
								return false;
							}
							else
							{
								openCounter = 0;
							}
						}
					}
				}

				string RamFile = "";

				RamFile = read.ReadToEnd();

				if (mHandle == IniHandle.ClearHandle)
				{
					mLockedFileStream.Close();
					//mLockedFileStream.Finalize();
					mLockedFileStream = null;
				}

				ParseFile(RamFile);
			}
			catch
			{
				
			}

			return true;
		}
		private bool ParseFile(string fileBuffer)
		{
			ParseState state = ParseState.STATE_PARSE_BEGIN;
			CharEnumerator  thisChar = fileBuffer.GetEnumerator();
			//	char thisChar;
			string thisString = "";
			MOG_IniKey		 key = new MOG_IniKey();
			MOG_IniSection	 section = new MOG_IniSection();
			// FIX FOR [=] parsing incorrectly

			// Scan every character in the file
			while(thisChar.MoveNext())
			{
				// Check for parsing characters
				// End of Keyname?
				if ((thisChar.Current == '=') && (state == ParseState.STATE_PARSE_BEGIN))
				{
					// Extra check to make sure the key we are going to write is actually valid
					if( thisString.Length != 0)
					{
						// Set this Key name
						key.mKey = thisString;
						// Clear thisString
						thisString = "";
						state = ParseState.STATE_PARSE_KEY;
					}
				}
					// carriage return
				else if (thisChar.Current == 13) 
				{
					// just eat this byte of the stream - 
				}
					// End of line?
				else if (thisChar.Current == 10)
				{
					// Only bother to do something on the end of a line if we have buffered up a string
					if (thisString.Length != 0 && key.mKey.Length != 0)
					{
						key.mValue = thisString;
				
						MOG_IniKey  kHolder = new MOG_IniKey();
						kHolder.mKey = key.mKey;
						kHolder.mValue = key.mValue;
						// Push this key into the current section
						section.mKeys.Add(kHolder);
						// Erase both the key & value
						key.mKey = "";
						key.mValue = "";
					}
						// If we have a key without a value then we must save it as a key only.
					else
					{
						if(thisString.Length != 0)
						{
							key.mKey = thisString;
					
							MOG_IniKey  kHolder = new MOG_IniKey();
							kHolder.mKey = key.mKey;
							kHolder.mValue = "";
				
							// Push this key into the current section
							section.mKeys.Add(kHolder);
							// Erase both the key & value
							key.mKey= "";
							key.mValue = "";
						}

					}
					// Clear thisString
					thisString = "";
					state = ParseState.STATE_PARSE_BEGIN;
				}
					// New Section?
				else if ((thisChar.Current == '[') && (state == ParseState.STATE_PARSE_BEGIN))
				{
					// Only push if we previously had a section name
					if (section.mSection.Length != 0)
					{
						MOG_IniSection  sHolder = new MOG_IniSection();
						sHolder.mSection = section.mSection;
						sHolder.mKeys = (ArrayList)(section.mKeys.Clone());

						// Push this section and all of it's keys
						mSections.Add(sHolder);
						// Clean up the section variables
						section.mSection = "";
						section.mKeys.RemoveRange(0, section.mKeys.Count);
						// Clear the SectionKey
						section.mSection = "";
					}
					// Clear thisString
					thisString = "";
					state = ParseState.STATE_PARSE_SECTION;
				}
					// End Section?
				else if ((thisChar.Current == ']') && (state == ParseState.STATE_PARSE_SECTION))
				{
					// Set the section name to the current string
					section.mSection = thisString;
					// Clear thisString
					thisString = "";
					state = ParseState.STATE_PARSE_END;
				}
				else
				{
					thisString = string.Concat(thisString, Convert.ToString(thisChar.Current));
				}
			}

			// Push the final key into the section?
			if (thisString.Length != 0)
			{
				// Key names must get filled in first...then the key values
				if (key.mKey.Length == 0)
					key.mKey = thisString;
				else
					key.mValue = thisString;

				MOG_IniKey  kHolder = new MOG_IniKey();
				kHolder.mKey = key.mKey;
				kHolder.mValue = key.mValue;
		
				// Push this key into the current section
				section.mKeys.Add(kHolder);
				// Push this key into the current section
				//section.mKeys.Add(key.MemberwiseClone());
				// Erase both the key & value
				key.mKey = "";
				key.mValue = "";
			}

			// Push the final section?
			if (section.mSection.Length != 0)
			{
				// Push this section and all of it's keys
				mSections.Add(section);
			}

			return true;
		}
	
		public void SetFilename(string filename, bool setChanged)
		{
//?	MOG_Ini::SetFilename - Decide how to handle a write-locked file when the name is changed
			// Set the new filename
			mFullFilename = filename;

			// Only bother setting mChanged if the file hasn't already been changed...
			if (!mChanged)
			{
				mChanged = setChanged;
			}
		}

		public void CloseNoSave()
		{
			mChanged = false;

			Close();
		}

		public void Close()
		{
			Save();

			// Close the StreamWriter who will close the FileStream for us.
			if (mHandle == IniHandle.HoldHandle)
			{
				mLockedFileStream.Close();
				mHandle = IniHandle.ClearHandle;
			}

			// Clear previous member data
			mSections.Clear();
			mFullFilename = "";
			mChanged = false;
		}

		public void Empty()
		{
			// Clear previous member data
			//	mSections.Clear(mSections, 0, mSections.Count);
			mSections.Clear();
			mChanged = true;
		}

		public void EmptySection(string section)
		{
			MOG_IniSection pSection = SectionFind(section);

			// Make sure the section was found
			if (pSection != null)
			{
				mSections.Remove(pSection);
			}

			mChanged = true;
		}

		public int GetValue(string section, string key)
		{
			int value = 0;

			string str = GetString(section, key);
			if (str.Length != 0)
			{
				value = Convert.ToInt32(str);
			}

			return value;
		}

		public string GetString(string section, string key)
		{
			MOG_IniKey pKey = KeyFind(section, key);

			if (pKey != null)
			{
				return pKey.mValue;
			}

			if (section!=null)
			{
				MessageBox.Show(string.Concat(mFullFilename, "\nSection - ", section, "\nKey - ", key, " not found!"), " GetString ", MessageBoxButtons.OK);
			}
			else
			{
					MessageBox.Show(string.Concat(mFullFilename, "\nSection - ", section, "\nKey - ", key,  "\n\nSection not found!"), " GetString ", MessageBoxButtons.OK);
			}

			return "";
		}

		public int CountKeys(string section)
		{
			MOG_IniSection pSection = SectionFind(section);

			if (pSection != null)
			{
				return pSection.mKeys.Count;
			}

			MessageBox.Show(string.Concat(mFullFilename, "\n", section, "\nSection not found!")," CountKeys ", MessageBoxButtons.OK);

			return 0;
		}

		public bool SectionExist(string section)
		{
			if (SectionFind(section) != null)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public MOG_IniSection SectionFind(string section)
		{
			if (string.Compare(section, mSectionCache.mSection, true) == 0)
			{
				return mSectionCache;
			}

			for (int s = 0; s < mSections.Count; s++)
			{
				MOG_IniSection pSection = (MOG_IniSection)(mSections[s]);

				if (string.Compare(section, pSection.mSection, true) == 0)//MOG_StringCompare(section, pSection.mSection))
				{
					mKeyCache = new MOG_IniKey();
					mSectionCache = pSection;
					return mSectionCache;
				}
			}

			return null;
		}

		public bool KeyExist(string section, string key)
		{
			if (KeyFind(section, key) != null)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public MOG_IniKey KeyFind(string section, string key)
		{
			MOG_IniSection pSection = SectionFind(section);

			if (string.Compare(key, mKeyCache.mKey, true) == 0)
			{
				return mKeyCache;
			}

			// Located the section?
			if (pSection != null)
			{
				for (int k = 0; k < pSection.mKeys.Count; k++)
				{
					MOG_IniKey pKey = (MOG_IniKey)(pSection.mKeys[k]);

					if (string.Compare(key, pKey.mKey, true) == 0)//MOG_StringCompare(key, pKey.mKey))
					{
						mKeyCache = pKey;
						return mKeyCache;
					}
				}
			}

			return null;
		}

		public MOG_IniKey KeyFind(MOG_IniSection pSection, string key)
		{
			if (pSection != null)
			{
				for (int k = 0; k < pSection.mKeys.Count; k++)
				{
					MOG_IniKey pKey = (MOG_IniKey)(pSection.mKeys[k]);

					if (string.Compare(key, pKey.mKey, true) == 0)//MOG_StringCompare(key, pKey.mKey))
					{
						return pKey;
					}
				}
			}

			return null;
		}

		public bool PutFile(string iniFile)
		{
			MOG_Ini temp = new MOG_Ini();
	
			// Load the ini file
			if (temp.Load(iniFile) == false)
			{
				return false;
			}
	
			PutFile(temp);

			temp.Close();

			return true;
		}
		public bool PutFile(MOG_Ini iniFile)
		{
			string section, key, value;
	
			// Loop through all the sections and keys and add them to our ini
			for (int x = 0; x < iniFile.CountSections(); x++)
			{
				section = iniFile.GetSectionByIndex(x);

				if (iniFile.CountKeys(section)!=0)
				{
					for (int y = 0; y < iniFile.CountKeys(section); y++)
					{
						key = iniFile.GetKeyNameByIndex(section, y);
						value = iniFile.GetKeyByIndex(section, y);

						PutString(section, key, value);
					}
				}
				else
				{
					// Add the section with no keys into our ini
					PutSectionString(section, "");
				}
			}

			return true;
		}
		public int CountSections()
		{
			return mSections.Count;
		}

		public string GetSectionByIndex(int sectionIndex)
		{
			//			MOG_ASSERT_RETURN_null(sectionIndex<mSections.Count, "MOG ERROR - section index is bigger than number section in ini");
			//			MOG_ASSERT_RETURN_null(sectionIndex>=0, "MOG ERROR - section index must be 0 or greater in ini");
			return ((MOG_IniSection)(mSections[sectionIndex])).mSection;		
		}
	
		public string GetKeyByIndex(string section, Int32 keyIndex)
		{
			MOG_IniSection pSection = SectionFind(section);

			if (pSection != null)
			{
				if ((keyIndex < pSection.mKeys.Count) && (keyIndex >= 0) )
				{
					return ((MOG_IniKey)(pSection.mKeys[keyIndex])).mValue;
				}
			}

			MessageBox.Show(" GetKeyByIndex ", string.Concat(mFullFilename, "\n", section, ":", Convert.ToString(keyIndex), "\nSection and or Key not found!"), MessageBoxButtons.OK);

			return "";
		}	

		public string GetKeyNameByIndex(string section, Int32 keyIndex)
		{
			MOG_IniSection pSection = SectionFind(section);

			if (pSection != null)
			{
				if( (keyIndex < pSection.mKeys.Count) && (keyIndex >= 0) )
				{
					return ((MOG_IniKey)(pSection.mKeys[keyIndex])).mKey;
				}
			}

			MessageBox.Show(" GetKeyNameByIndex ", string.Concat(mFullFilename, "\n", section, ":", Convert.ToString(keyIndex), "\nSection and or Key not found!\n"), MessageBoxButtons.OK);

			return "";
		}	

		public int PutValue(string section, string key, int value)
		{
			return PutString(section, key, Convert.ToString(value));
		}

		public int PutString(string section, string key, string str)
		{
			MOG_IniKey keyHolder = new MOG_IniKey();
			MOG_IniSection sectionHolder = new MOG_IniSection();

			//MOG_ASSERT_RETURN_null(section.Length, "MOG_INI - PutString requires a valid section");
			//MOG_ASSERT_RETURN_null(key.Length || str.Length, "MOG_INI - PutString requires either a valid key or str");
			if (str == null)
			{
				return 0;
			}
			// Let's be somewhat helpful in identifying this problem and fix it.
			if (str.Length == 0)
			{
				return PutSectionString(section, key);
			}
			else if (key.Length == 0)
			{
				return PutSectionString(section, str);
			}

			MOG_IniSection pSection = SectionFind(section);
			// Section found, now check for key
			if (pSection != null)
			{
				MOG_IniKey pKey = KeyFind(pSection, key);
				// Found key, now replace value
				if (pKey != null)
				{
					pKey.mValue = str;
				}
					// Key not found, add key and value
				else
				{
					keyHolder.mKey = key;
					keyHolder.mValue = str;
					pSection.mKeys.Add(keyHolder);
				}
			}
				// No section, Add new section, key and value
			else
			{
				string upperSection = section.ToUpper();
		
				keyHolder.mKey = key;
				keyHolder.mValue = str;

				sectionHolder.mSection = upperSection;
				sectionHolder.mKeys.Add(keyHolder);
				mSections.Add(sectionHolder);
			}

			mChanged = true;
			return 1;
		}

		public int	PutSectionString(string section, string str)
		{
			MOG_IniKey keyHolder = new MOG_IniKey();
			MOG_IniSection sectionHolder = new MOG_IniSection();

			MOG_IniSection pSection = SectionFind(section);
			// Section found, now check for key
			if (pSection != null)
			{
				MOG_IniKey pKey = KeyFind(section, str);
				// Key not found, add key and value
				if (pKey == null)
				{
					keyHolder.mKey = str;
					pSection.mKeys.Add(keyHolder);
				}
			}
				// No section, Add new section, key and value
			else
			{
				string upperSection = section.ToUpper();
		
				keyHolder.mKey = str;

				sectionHolder.mSection = upperSection;
				sectionHolder.mKeys.Add(keyHolder);
				mSections.Add(sectionHolder);
			}

			mChanged = true;

			return 1;
		}

		public bool RemoveString(string section, string key)
		{
			MOG_IniSection pSection = SectionFind(section);
			// Section found, now check for key
			if(pSection != null)
			{
				MOG_IniKey pKey = KeyFind(section, key);
				// Found key, now replace value
				if(pKey != null)
				{
					pSection.mKeys.Remove(pKey);
				}
			}

			mChanged = true;

			return true;	
		}

		public bool RemoveSection(string section)
		{
			MOG_IniSection pSection = SectionFind(section);
			// Make sure the section was found
			if(pSection != null)
			{
				mSections.Remove(pSection);
			}

			mChanged = true;
	
			return true;	
		}

		public bool RenameSection(string section, string newSection)
		{
			MOG_IniSection pSection = SectionFind(section);
			// Section found, now check for key
			if(pSection != null)
			{
				pSection.mSection = newSection;
				mChanged = true;
				return true;
			}

			return false;
		}

		public bool RenameKey(string section, string key, string newKey)
		{
			MOG_IniKey pKey = KeyFind(section, key);
			// Section found, now check for key
			if(pKey != null)
			{
				pKey.mKey = newKey;
				mChanged = true;
				return true;		
			}

			return false;
		}

	}

}
