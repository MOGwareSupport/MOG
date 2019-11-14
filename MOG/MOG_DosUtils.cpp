//--------------------------------------------------------------------------------
//	MOG_DosUtils.cpp
//	
//	
//--------------------------------------------------------------------------------
#include "StdAfx.h"

#include "MOG_Define.h"
#include "MOG_StringUtils.h"
#include "MOG_Ini.h"
#include "MOG_Prompt.h"

#include "MOG_DosUtils.h"
#include "MOG_Progress.h"

#include <malloc.h>
#include <string.h>

using namespace System::Diagnostics;
using namespace System::Threading;
using namespace System::Text::RegularExpressions;
using namespace MOG_CoreControls::Utilities;

using System::DateTime;


bool DosUtils::FileTouch(String *filename)
{
	FileInfo *file = new FileInfo(filename);
	if (file->Exists)
	{
		try
		{
			file->set_LastWriteTime(DateTime::Now);
		}
		catch(...)
		{
			return false;
		}

		return true;
	}

	return false;
}


bool DosUtils::Initialize(String *s, String *t)
{
	sDrive = S"";
	tDrive = S"";
	sPath = S"";
	tPath = S"";
	sFullName = S"";
	tFullName = S"";
	sName = S"";
	tName = S"";
	sDirectoryName = S"";
	tDirectoryName = S"";
	sFilename = S"";
	tFilename = S"";
	sIsDirectory = false;
	tIsDirectory = false;
	sContainsWildCard = false;
	tContainsWildCard = false;
	DirectoryInfo *sPathInfo = NULL;
	DirectoryInfo *tPathInfo = NULL;
	FileInfo *sFile = NULL;
	FileInfo *tFile = NULL;
	int pos;

	if (s && s->Length)
	{
		// Check for a drive letter?
		pos = s->IndexOf(":\\");
		if (pos != -1)
		{
			// Get the drive letter including the ':'
			sDrive = s->Substring(0, pos + 1);
		}
		// Check for a UNC path?
		else if (s->StartsWith("\\\\"))
		{
			// Get the 3rd index of the '\'
			sDrive = s->Substring(0, s->IndexOf("\\", 3) + 1);
		}

		// Make sure that there is more to the source than just a drive
		if ((s->Length - sDrive->Length) > 1)
		{
			// If the source ends with a '\', we can we assume the source is a directory? 
			if (s->EndsWith("\\"))
			{
				// Strip off the last '\'
				s = s->Substring(0, s->Length - 1);
				// Indicate that we can assume this is a directory
				sIsDirectory = true;
			}

			// Extract sPath
			try{
				int index = s->LastIndexOf("\\");
				if (index >= 0)
					sPath = s->Substring(0, index);
			}
			catch(Exception *e)
			{
				mLastKnownError = e->Message;
				sPath = "";
			}

			if (sPath->Length > 0)
			{
				// Extract sName
				sName = s->Substring(sPath->Length + 1);

				// Complete sFullName
				sFullName = String::Concat(sPath, "\\", sName);
			}
			else
			{
				sName = s;
				sFullName = s;
			}

			// Get the sPathInfo?
			// Check for any wildcards in sPath
			MOG_ASSERT((sPath->IndexOf(S"*") == -1 && sPath->IndexOf(S"?") == -1), S"Wildcards can not be specified in a path");
			if (sPath->Length)
				sPathInfo = new DirectoryInfo(sPath);

			// Check for any wildcards in sName
			if (sName->IndexOf(S"*") != -1 || sName->IndexOf(S"?") != -1)
			{
				sContainsWildCard = true;
				sIsDirectory = false;	// No wild cards on directories
			}
			// Gather more information about the source
			else
			{
				// Check to see if sFullName is a directory
				DirectoryInfo *dirInfo = new DirectoryInfo(sFullName);
				if (dirInfo->Exists)
				{
					sIsDirectory = true;
					sDirInfo = dirInfo;
					sDirectoryName = sName;
				}
				// Looks like it is just a file
				else
				{
					MOG_ASSERT(!sIsDirectory, S"The source is not a directory or does not exist");
					if (!sIsDirectory)
					{
						sFileInfo = new FileInfo(sFullName);
						sFilename = sName;
					}
				}
			}
		}
	}

	if (t && t->Length)
	{
		// Check for a drive letter?
		pos = t->IndexOf(":\\");
		if (pos != -1)
		{
			// Get the drive letter including the ':'
			tDrive = t->Substring(0, pos + 1);
		}
		// Check for a UNC path?
		else if (t->StartsWith("\\\\"))
		{
			// Get the 3rd index of the '\'
			tDrive = t->Substring(0, t->IndexOf("\\", 3) + 1);
		}

		// Make sure that there is more to the target than just a drive
		if ((t->Length - tDrive->Length) > 1)
		{
			// If the target ends with a '\', we need to match the source name and type
			if (t->EndsWith("\\"))
			{
				// Strip off the last '\'
				t = t->Substring(0, t->Length - 1);
				// Match the name and type of the source
				tIsDirectory = true;
			}

			DirectoryInfo *dirInfo = NULL;

			// Check to see if tFullName is a directory
			try
			{
				dirInfo = new DirectoryInfo(t);
				if (dirInfo->Exists)
				{
					//if the source is a filename
					if (!sIsDirectory)	// If the source is a Directory then just leave it alone.
						t = String::Concat(t, S"\\", sName);	// Add on the source file name.
					tPathInfo = dirInfo;
					tIsDirectory = sIsDirectory;
				}
				else
				{
					if (tIsDirectory)
					{
						if (!sIsDirectory)	// If the source is a Directory then just leave it alone.
						{
							t = String::Concat(t, S"\\", sName);	// Add on the source file name.
							tIsDirectory = 0;
						}
					}
				}
			}
			catch(Exception *e)
			{
				// If it can't use this file as a directory then just keep going
				mLastKnownError = e->Message;
			}

			// Extract tPath
			tPath = t->Substring(0, t->LastIndexOf("\\"));
			// Extract tName
			tName = t->Substring(tPath->Length + 1);
			// Complete tFullName
			tFullName = String::Concat(tPath, "\\", tName);

			// Get the tPathInfo?
			// Check for any wildcards in tPath
			MOG_ASSERT((tPath->IndexOf(S"*") == -1 && tPath->IndexOf(S"?") == -1), S"Wildcards can not be specified in a path");
			tPathInfo = Get_tPathInfo();

			// Check for any wildcards in tName
			if (tName->IndexOf(S"*") != -1 || tName->IndexOf(S"?") != -1)
			{
				if (sIsDirectory)
				{
					tName = "";	// No wild cards on the copy side just now.
					tFullName = String::Concat(tPath, "\\", tName);	// Recreate the full name
					// If we are copying a directory, we are always copying it to a directory!
					tIsDirectory = true;
				}
				else
				{
					tName = sName;	// No wild cards on the copy side just now.
					tFullName = String::Concat(tPath, "\\", tName);	// Recreate the full name
					tIsDirectory = false;	// No wild cards on directories
				}

				tContainsWildCard = true;
			}
			// Gather more information about the target
			else
			{
				// If we are copying a directory, we are always copying it to a directory!
				if (sIsDirectory)
					tIsDirectory = true;

				// Check to see if the target path exists
				if (!tPathInfo->Exists)
				{
					// If the target doesn't exist, Match the type of the source
					tIsDirectory = sIsDirectory;
				}

				// Fixup the right name based on the target's type
				if (tIsDirectory)
				{
					tDirInfo = dirInfo;
					tDirectoryName = tName;
				}
				// Looks like it is just a file
				else
				{
					tFileInfo = new FileInfo(tFullName);
					tFilename = tName;
				}
			}
		}
	}
	return true;
}

void DosUtils::Dispose()
{
	sDrive = NULL;
	tDrive = NULL;
	sPath = NULL;
	tPath = NULL;
	sFullName = NULL;
	tFullName = NULL;
	sName = NULL;
	tName = NULL;
	sDirectoryName = NULL;
	tDirectoryName = NULL;
	sFilename = NULL;
	tFilename = NULL;
	sPathInfo = NULL;
	tPathInfo = NULL;
	sDirInfo = NULL;
	tDirInfo = NULL;
	sFileInfo = NULL;
	tFileInfo = NULL;
}

void DosUtils::Shutdown()
{
	try
	{

	// Check if we have an active Process running?
	if (gProcess)
	{
		// Make sure we terminate/kill this active process
		gProcess->Kill();
		gProcess = NULL;
	}

	}
	catch (...)
	{
	}
	__finally
	{
		gProcess = NULL;
	}
}


bool DosUtils::Exist(String *filename)
{
	// Return either one
	return (FileExist(filename) || DirectoryExist(filename));
}


bool DosUtils::ExistFast(String *filename)
{
	// Return either one
	return (FileExistFast(filename) || DirectoryExistFast(filename));
}


bool DosUtils::Copy(String *source, String *target, bool overwrite)
{
	DosUtils *utils = new DosUtils;
	utils->Initialize(source, target);

	// Is this a directory?
	if (utils->sIsDirectory)
	{
		return DirectoryCopy(utils->sFullName, utils->tFullName, overwrite);
	}

	// Otherwise, assume it as a file
	return FileCopy(utils->sFullName, utils->tFullName, overwrite);
}

bool DosUtils::CopyFast(String *source, String *target, bool overwrite)
{
	FileInfo* fileInfo = new FileInfo(source);

	// Is this a directory?
	if (fileInfo->Attributes & FileAttributes::Directory)
	{
		return DirectoryCopyFast(source, target, overwrite);
	}

	// Otherwise, assume it as a file
	return FileCopyFast(source, target, overwrite);
}


bool DosUtils::Move(String *source, String *target, bool overwrite)
{
	DosUtils *utils = new DosUtils;
	utils->Initialize(source, target);

	// Is this a directory?
	if (utils->sIsDirectory)
	{
		return DirectoryMove(utils->sFullName, utils->tFullName, overwrite);
	}

	// Otherwise, assume it as a file
	return FileMove(utils->sFullName, utils->tFullName, overwrite, false);
}

bool DosUtils::MoveFast(String *source, String *target, bool overwrite)
{
	FileInfo* fileInfo = new FileInfo(source);

	// Is this a directory?
	if (fileInfo->Attributes & FileAttributes::Directory)
	{
		return DirectoryMoveFast(source, target, overwrite);
	}

	// Otherwise, it is a file
	return FileMoveFast(source, target, overwrite);
}


bool DosUtils::Rename(String *source, String *target, bool overwrite)
{
	DosUtils *utils = new DosUtils;
	utils->Initialize(source, target);

	// Try it as a directory first
	if (utils->sIsDirectory)
	{
		return DirectoryMove(source, target, overwrite);
	}

	// Otherwise, assume it as a file
	return FileMove(source, target, overwrite, false);
}

bool DosUtils::RenameFast(String *source, String *target, bool overwrite)
{
	return MoveFast(source, target, overwrite);
}


bool DosUtils::Delete(String *filename)
{
	DosUtils *utils = new DosUtils;
	utils->Initialize(filename, "");

	if (utils->sContainsWildCard)
	{
		// Get list of files that match the file pattern
		FileInfo *files[] = utils->Get_sPathInfo()->GetFiles(utils->sName);

		// check each file seperately
		for (int f = 0; f < files->Count; f++)
		{
			// Make sure the each directory copies or else we should quit
			if (!FileDelete(String::Concat(utils->sPath, S"\\", files[f]->Name)))
			{
				return false;	//if we fail on a delete then stop.
			}
		}

		return true;
	}

	// Get source DirectoryInfo
	if (utils->sIsDirectory)
	{
		try
		{
			// Make sure we always clear the read Only flag!
			SetAttributesRecursive(filename, S"*.*", FileAttributes::Normal);
			utils->Get_sDirInfo()->Delete(true);
		}
		catch(Exception *e)
		{
			mLastKnownError = e->Message;
			String *message = String::Concat(	S"Could not delete: Access is denied.",
												S"   ", filename, S"\n\n",
												S"Make sure the disk is not full or write-protected and that the file is not currently in use.");
			MOG_Report::ReportMessage(S"Delete Error", message, e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
			return false;
		}

		return true;
	}

	// Otherwise, assume it as a file
	return FileDelete(filename);
}

bool DosUtils::DeleteFast(String *filename)
{
	FileInfo* fileInfo = new FileInfo(filename);

	if (fileInfo->Attributes & FileAttributes::Directory)
	{
		return DirectoryDeleteFast(filename);
	}

	return FileDeleteFast(filename);
}

bool DosUtils::Recycle(String *path)
{
	return Win32_Shell::RecycleFile(path);
}

int DosUtils::DirectoryDepth(String *directory)
{
	try
	{
		if (directory->Length)
		{
			int count = 0;
			String *temp = directory;

			while(temp->IndexOf("\\") != -1)
			{
				temp = temp->Remove(temp->IndexOf("\\"), 1);
				count ++;
			}

			return count;
		}
	}
	catch(...)
	{
	}

	return 0;
}

String *DosUtils::DirectoryGetAtDepth(String *directory, int depth)
{
	try
	{
		String* delimStr = S"\\";
		Char delimiter[] = delimStr->ToCharArray();
		String* directories[] = directory->Split(delimiter);

		if (directories != NULL && directories->Count > 0 && directories->Count >= depth)
		{
			return directories[depth];
		}
	}
	catch(...)
	{
	}

	return "";
}


bool DosUtils::FileExist(String *filename)
{
	try
	{
		// Get the common file information
		DosUtils *utils = new DosUtils;
		utils->Initialize(filename, "");

		// Does this source contain wild cards?
		if (utils->sContainsWildCard)
		{
			// Get list of files that match the file pattern
			FileInfo *files[] = utils->Get_sPathInfo()->GetFiles(utils->sName);

			// check each file seperately
			for (int f = 0; f < files->Count; f++)
			{
				// Make sure the each directory copies or else we should quit
				if (FileExist(String::Concat(utils->sPath, S"\\", files[f]->Name)))
				{
					return true;	// One matches return true.
				}
			}

			return false;	// Nothing matched.
		}

		// Make sure we have a filename to check
		if (utils->sFullName->Length)
		{
			FileInfo *file = new FileInfo(utils->sFullName);
			return (file->Exists);
		}
	}
	catch(Exception *e)
	{
		throw new Exception(String::Concat(e->Message, S"\n", filename), e);
	}

	return false;
}


bool DosUtils::FileExistFast(String *filename)
{
	try
	{
		FileInfo *file = new FileInfo(filename);
		return file->Exists;
	}
	catch(Exception *e)
	{
		mLastKnownError = e->Message;
		MOG_Report::ReportMessage(S"FileExistFast Error", String::Concat(S"Could not get FileInfo for '", filename, S"' (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
	}

	return false;
}


bool DosUtils::FileMissingModifiedFast(String *sourceFilename, String *targetFilename)
{
	try
	{
		// Obtain the sourceInfo
		FileInfo *sourceInfo = new FileInfo(sourceFilename);
		if (sourceInfo->Exists)
		{
			// Obtain the targetInfo
			FileInfo *targetInfo = new FileInfo(targetFilename);
			if (targetInfo->Exists)
			{
				// Compare the timestamps  (Must Use ToFileTime() because sometimes LastWriteTime comparisons can fail)
				if (sourceInfo->LastWriteTime.ToFileTime() == targetInfo->LastWriteTime.ToFileTime())
				{
					// Indicate the files are identical
					return false;
				}
			}

			// Indicate the target file is missing or modified
			return true;
		}
	}
	catch(Exception *e)
	{
		mLastKnownError = e->Message;
		MOG_Report::ReportMessage(S"FileCompareFast Error", String::Concat(S"Could not get FileInfo for '", sourceFilename, S"' (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
	}

	return false;
}


String* DosUtils::FileStripArguments(String *filename)
{
	String* drive = S"([a-zA-Z]:\\\\)?";
	String* ext = S"\\.\\w\\w\\w";

	String* word = String::Concat(drive, S"([^ <>|\"])+");
	String* wordExt = String::Concat(word, ext);

	//If you're inside quotes, you can have a space in the word
	String* spaceWord = String::Concat(drive, S"([^<>|\"])+");
	String* spaceWordExt = String::Concat(spaceWord, ext);
	String* quotedWord = String::Concat(S"\"", spaceWord, S"\"");
	String* quotedWordExt = String::Concat(S"\"", spaceWordExt, S"\"");

	String* command = String::Concat(S"^", S"((", word, S")|(", quotedWord, S"))");
	String* commandExt = String::Concat(S"^", S"((", wordExt, S")|(", quotedWordExt, S"))");
	String* commandSpaceExt = String::Concat(S"^", spaceWordExt);

	String* tests[] = 
	{
		String::Concat(commandExt, S"\\s"),
		String::Concat(commandExt, S"$"),
		String::Concat(commandSpaceExt, S"\\s"),
		String::Concat(commandSpaceExt, S"$"),
		String::Concat(command, S"\\s"),
		String::Concat(command, S"$"),
	};
	
	for (int i = 0; i < tests->Length; i++)
	{
		Match* match = Regex::Match(filename, tests[i], RegexOptions::IgnoreCase);
		if (match->Success)
		{
			return match->Value->Trim();
		}
	}
	
	return filename;
}

String* DosUtils::FileGetArguments(String *filename)
{
	String* command = FileStripArguments(filename);
	
	if (command)
	{
		return filename->Remove(0, command->Length)->Trim();
	}
	else
	{
		return "";
	}
}

String* DosUtils::SplitArguments(String *arguments)[]
{
	ArrayList *argumentArray = new ArrayList();
	String *argument = S"";
	bool bQuoted = false;

	// Parse the string looking for spaces
	for (int c = 0; c < arguments->Length; c++)
	{
		// Get the next character
		String *character = arguments->Substring(c, 1);

		// Check if we just found a space?
		if (String::Compare(character, S"\"", true) == 0)
		{
			// Reverse our state
			bQuoted = !bQuoted;
			continue;
		}

		// Make sure we are not within a quote?
		if (!bQuoted)
		{
			// Check if we just found a space?
			if (String::Compare(character, S" ", true) == 0)
			{
				// Check if we have an argument to add?
				if (argument->Length)
				{
					// Add this argument
					argumentArray->Add(argument);
					// Reset argument
					argument = S"";
				}
				continue;
			}
		}

		// Append this character onto the argument
		argument = String::Concat(argument, character);
	}

	// Check if we need to add the last argument?
	if (argument->Length)
	{
		// Add this argument
		argumentArray->Add(argument);
		// Reset argument
		argument = S"";
	}

	// Convert the argumentArray into a String[]
	String *argumentList[] = new String*[argumentArray->Count];
	for (int i = 0; i < argumentArray->Count; i++)
	{
		argumentList[i] = __try_cast<String*>(argumentArray->Item[i]);
	}
	// Return our list of arguments
	return argumentList;
}

long long DosUtils::FileGetSize(String *filename)
{
	long long size = 0;

	FileInfo* file = NULL;
	
	try
	{
		file = new FileInfo(filename);

		size = file->Length;
	}
	catch (Exception* e)
	{
		mLastKnownError = e->Message;
		MOG_Report::ReportMessage(S"FileGetSize Error", String::Concat(S"Could not get size for", filename, S" (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
	}

	return size;
}

bool DosUtils::FileCopy(String *source, String *target, bool overwrite)
{
	// Get the common file information
	DosUtils *utils = new DosUtils;
	utils->Initialize(source, target);

	// Does this source contain wild cards?
	if (utils->sContainsWildCard)
	{
		// Get list of files that match the file pattern
		FileInfo *files[] = utils->Get_sPathInfo()->GetFiles(utils->sName);
		// Copy each file seperately
		for (int f = 0; f < files->Count; f++)
		{
			// Make sure the each file copies or else we should quit
			if (!FileCopy(files[f]->FullName, String::Concat(utils->tPath, S"\\", files[f]->Name), overwrite))
			{
				return false;
			}
		}
		utils->Dispose();
		return true;
	}

	// Get FileInfo on both the source and destination
	utils->Get_sFileInfo();
	utils->Get_tFileInfo();

	// Check to make sure the source file exists
	MOG_ASSERT_THROW(utils->sFileInfo->Exists, MOG_Exception::MOG_EXCEPTION_MissingData, String::Concat(S"Attempting to copy non existing file(", utils->sFileInfo->FullName, S")"));

	// If the source and destination are the same name...just say we did it
	// Do we have to use a wildcard compatible compare?
	if (source->IndexOf("*") != -1 || target->IndexOf("*") != -1)
	{
		if (MOG_StringCompare(source, target))
		{
			utils->Dispose();
			return true;
		}
	}
	else
	{
		if (String::Compare(source, target, true) == 0)
		{
			utils->Dispose();
			return true;
		}
	}

	// Check to make sure we aren't overwiting an existing file
	if (utils->tFileInfo->Exists)
	{
		// Overwrite this file?
		if (!overwrite)
		{
			utils->Dispose();
			return false;
		}
		
		//trying to delete the existing file the first time, and keep retrying as long as the user clicks retry
		while (true)
		{
			try
			{
				// Make sure we always clear the read Only flag!
				utils->tFileInfo->Attributes = FileAttributes::Normal;
				utils->tFileInfo->Delete();
				break; //out of the retry loop
			}
			catch(Exception *e)
			{
				mLastKnownError = e->Message;
				
				//inform the user of the error and ask them if they want to fix something and retry
				String *message = String::Concat(	S"Could not delete: Access is denied.",
													S"   ", target, S"\n\n",
													S"Make sure the disk is not full or write-protected and that the file is not currently in use.");
				if (MOG_Prompt::PromptResponse(S"FileCopy Error", message, e->StackTrace, MOGPromptButtons::RetryCancel) == MOGPromptResult::Cancel)
				{
					//the user has chosen to cancel the file copy, so clean up and go home
					utils->Dispose();
					return false;
				}
			}
		}
	}

	// Make sure that the target directory exists before we copy
	if (!utils->tFileInfo->Directory->Exists)
	{
		while (true)
		{
			try
			{
				utils->tFileInfo->Directory->Create();

	//			// Set the file to be Normal & Archived
	//			File::SetAttributes(utils->tFileInfo->Directory->FullName, FileAttributes::Normal);
	//			File::SetAttributes(utils->tFileInfo->Directory->FullName, static_cast<FileAttributes>(File::GetAttributes(utils->tFileInfo->Directory->FullName) | FileAttributes::Archive));

				break; //out of the retry loop
			}
			catch(Exception *e)
			{
				mLastKnownError = e->Message;

				//inform the user of the error and ask them if they want to fix something and retry
				if (MOG_Prompt::PromptResponse(S"FileCopy Error", String::Concat(S"Could not create parent directory for ", target), e->StackTrace, MOGPromptButtons::RetryCancel) == MOGPromptResult::Cancel)
				{
					//the user has chosen to cancel the file copy, so clean up and go home
					utils->Dispose();
					return false;
				}
			}
		}
	}

	while (true)
	{
		try
		{
			utils->sFileInfo->CopyTo(utils->tFullName);
			utils->tFileInfo->Refresh();

	//		// Set the file to be Normal & Archived
	//		File::SetAttributes(utils->tFullName, FileAttributes::Normal);
	//		File::SetAttributes(utils->tFullName, static_cast<FileAttributes>(File::GetAttributes(utils->tFullName) | FileAttributes::Archive));

			break; //out of the retry loop
		}
		catch(Exception *e)
		{
			mLastKnownError = e->Message;

			//inform the user of the error and ask them if they want to fix something and retry
			if (MOG_Prompt::PromptResponse(S"FileCopy Error", String::Concat(S"Could not copy ", source, S" to ", target), e->StackTrace, MOGPromptButtons::RetryCancel) == MOGPromptResult::Cancel)
			{
				//the user has chosen to cancel the file copy, so clean up and go home
				utils->Dispose();
				return false;
			}
		}
	}

	utils->Dispose();
	utils = NULL;

	return true;
}


bool DosUtils::FileCopyFast(String *source, String *target, bool overwrite)
{
	FileInfo* srcFileInfo = NULL;
	FileInfo* dstFileInfo = NULL;
	
	// Get FileInfo on both the source and destination
	try
	{
        srcFileInfo = new FileInfo(source);
		dstFileInfo = new FileInfo(target);
	}
	catch (Exception* e)
	{
		mLastKnownError = e->Message;
	}

	// Check to make sure the source file exists
	MOG_ASSERT_THROW(srcFileInfo->Exists, MOG_Exception::MOG_EXCEPTION_MissingData, String::Concat(S"Attempting to copy non existing file(", source, S")"));

	// If the source and destination are the same name...just say we did it
	if (String::Compare(source, target, true) == 0)
	{
		return true;
	}

	// Check to make sure we aren't overwiting an existing file
	if (dstFileInfo->Exists)
	{
		// Overwrite this file?
		if (!overwrite)
		{
			return false;
		}
		
		//trying to delete the existing file the first time, and keep retrying as long as the user clicks retry
		while (true)
		{
			try
			{
				// Make sure we always clear the read Only flag!
				dstFileInfo->Attributes = FileAttributes::Normal;
				dstFileInfo->Delete();
				break; //out of the retry loop
			}
			catch(Exception *e)
			{
				mLastKnownError = e->Message;
				
				//inform the user of the error and ask them if they want to fix something and retry
				String *message = String::Concat(	S"Could not delete: Access is denied.",
													S"   ", target, S"\n\n",
													S"Make sure the disk is not full or write-protected and that the file is not currently in use.");
				if (MOG_Prompt::PromptResponse(S"FileCopy Error", message, e->StackTrace, MOGPromptButtons::RetryCancel) == MOGPromptResult::Cancel)
				{
					//the user has chosen to cancel the file copy, so clean up and go home
					return false;
				}
			}
		}
	}

	// Make sure that the target directory exists before we copy
	if (!dstFileInfo->Directory->Exists)
	{
		while (true)
		{
			try
			{
				dstFileInfo->Directory->Create();
				break; //out of the retry loop
			}
			catch(Exception *e)
			{
				mLastKnownError = e->Message;

				//inform the user of the error and ask them if they want to fix something and retry
				if (MOG_Prompt::PromptResponse(S"FileCopy Error", String::Concat(S"Could not create parent directory for ", target), e->StackTrace, MOGPromptButtons::RetryCancel) == MOGPromptResult::Cancel)
				{
					//the user has chosen to cancel the file copy, so clean up and go home
					return false;
				}
			}
		}
	}

	int dialogId = 0;
	int violationStage = 0;
	bool retval = true;
	DateTime warnTime = DateTime::Now.AddSeconds(5);

	//Do the actual copy
	while (true)
	{
		try
		{
			srcFileInfo->CopyTo(target, overwrite);
			dstFileInfo->Refresh();
			break; //out of the retry loop
		}
		catch(IO::IOException* e)
		{
			if (dialogId == 0)
			{
				dialogId = MOG_Progress::ProgressSetup(S"File Sharing Violation", String::Concat(	S"Please wait while MOG attempts to access the file.\n",
																									S"FILE: ", target, S"\n\n",
																									e->Message));
			}

			if (MOG_Prompt::IsMode(MOG_PROMPT_SILENT))
			{
				violationStage = SilentSharingViolationUpdate(target, violationStage, warnTime, e->Message, e->StackTrace);
			}
			else
			{
				if (MOG_Progress::ProgressStatus(dialogId) == MOGPromptResult::Cancel ||
					DateTime::Now > warnTime)
				{
					mLastKnownError = e->Message;
					MOG_Report::ReportMessage(S"FileCopyFast Error", String::Concat(S"MOG timed out while waiting for access to the file: ", target, S" (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
					retval = false;
					break;
				}
			}

			//We want to wait patiently and not pound the file server constantly
			Thread::Sleep(100);
		}
		catch(Exception *e)
		{
			mLastKnownError = e->Message;

			//inform the user of the error and ask them if they want to fix something and retry
			if (MOG_Prompt::PromptResponse(S"FileCopy Error", String::Concat(S"Could not copy ", source, S" to ", target), e->StackTrace, MOGPromptButtons::RetryCancel) == MOGPromptResult::Cancel)
			{
				//the user has chosen to cancel the file copy, so clean up and go home
				retval = false;
			}
		}
	}

	MOG_Progress::ProgressClose(dialogId);
	return retval;
}


bool DosUtils::FileCopyModified(String *source, String *target)
{
	// Get the common file information
	DosUtils *utils = new DosUtils;
	utils->Initialize(source, target);

	// Does this source contain wild cards?
	if (utils->sContainsWildCard)
	{
		// Get list of files that match the file pattern
		FileInfo *files[] = utils->Get_sPathInfo()->GetFiles(utils->sName);
		// Copy each file seperately
		for (int f = 0; f < files->Count; f++)
		{
			// Make sure the each file copies or else we should quit
			if (!FileCopyModified(files[f]->FullName, String::Concat(utils->tPath, S"\\", files[f]->Name)))
			{
				return false;
			}
		}
		utils->Dispose();
		return true;
	}

	// Get FileInfo on both the source and destination
	utils->Get_sFileInfo();
	utils->Get_tFileInfo();

	// Check to make sure the source file exists
	MOG_ASSERT_THROW(utils->sFileInfo->Exists, MOG_Exception::MOG_EXCEPTION_MissingData, String::Concat(S"Attempting to copy non existing file(", utils->sFileInfo->FullName, S")"));

	// If the source and destination are the same name...just say we did it
	// Do we have to use a wildcard compatible compare?
	if (source->IndexOf("*") != -1 || target->IndexOf("*") != -1)
	{
		if (MOG_StringCompare(source, target))
		{
			utils->Dispose();
			return true;
		}
	}
	else
	{
		if (String::Compare(source, target, true) == 0)
		{
			utils->Dispose();
			return true;
		}
	}

	// Check to see that the target files needs updating  (Must Use ToFileTime() because sometimes LastWriteTime comparisons can fail)
	if (utils->sFileInfo->LastWriteTime.ToFileTime() != utils->tFileInfo->LastWriteTime.ToFileTime())
	{
		// Check to make sure we aren't overwiting an existing file
		if (utils->tFileInfo->Exists)
		{
			try
			{
				// Make sure we always clear the read Only flag!
				utils->tFileInfo->Attributes = FileAttributes::Normal;
				utils->tFileInfo->Delete();
			}
			catch(Exception *e)
			{
				mLastKnownError = e->Message;
				String *message = String::Concat(	S"Could not delete: Access is denied.",
													S"   ", target, S"\n\n",
													S"Make sure the disk is not full or write-protected and that the file is not currently in use.");
				MOG_Report::ReportMessage(S"FileCopyModified Error", message, e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
				utils->Dispose();
				return false;
			}
		}

		// Make sure that the target directory exists before we copy
		if (!utils->tFileInfo->Directory->Exists)
		{
			try
			{
				utils->tFileInfo->Directory->Create();

//				// Set the file to be Normal & Archived
//				File::SetAttributes(utils->tFileInfo->Directory->FullName, FileAttributes::Normal);
//				File::SetAttributes(utils->tFileInfo->Directory->FullName, static_cast<FileAttributes>(File::GetAttributes(utils->tFileInfo->Directory->FullName) | FileAttributes::Archive));
			}
			catch(Exception *e)
			{
				mLastKnownError = e->Message;
				MOG_Report::ReportMessage(S"FileCopyModified Error", String::Concat(S"Could not create parent directory for ", target, S"(", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
				utils->Dispose();
				return false;
			}
		}

		try
		{
			utils->sFileInfo->CopyTo(utils->tFullName);
			utils->tFileInfo->Refresh();
			
//			// Set the file to be Normal & Archived
//			File::SetAttributes(utils->tFullName, FileAttributes::Normal);
//			File::SetAttributes(utils->tFullName, static_cast<FileAttributes>(File::GetAttributes(utils->tFullName) | FileAttributes::Archive));
		}
		catch(Exception *e)
		{
			mLastKnownError = e->Message;
			MOG_Report::ReportMessage(S"FileCopyModified Error", String::Concat(S"Could not copy ", source, S" to ", target, S" (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
			utils->Dispose();
			return false;
		}
	}
	else
	{
		utils->Dispose();
		utils = NULL;
		return false;
	}

	utils->Dispose();
	utils = NULL;

	return true;
}

bool DosUtils::FileMove(String *source, String *target, bool overwrite, bool includeDirectoriesWithWildCards)
{
	// Get the common file information
	DosUtils *utils = new DosUtils;
	utils->Initialize(source, target);

	// Does this source contain wild cards?
	if (utils->sContainsWildCard)
	{
		// Get list of files that match the file pattern
		FileInfo *files[] = utils->Get_sPathInfo()->GetFiles(utils->sName);

		// Copy each file seperately
		for (int f = 0; f < files->Count; f++)
		{
			// Make sure the each file copies or else we should quit
			if (!FileMove(files[f]->FullName, String::Concat(utils->tPath, S"\\", files[f]->Name), overwrite, false))
			{
				return false;
			}
		}

		if (includeDirectoriesWithWildCards)
		{
			// Get list of files that match the file pattern
			DirectoryInfo *directories[] = utils->Get_sPathInfo()->GetDirectories(utils->sName);

			// Copy each file seperately
			for (int f = 0; f < directories->Count; f++)
			{
				// Make sure the each directory copies or else we should quit
				if (!DirectoryMove(directories[f]->FullName, String::Concat(utils->tPath, S"\\", directories[f]->Name), overwrite))
				{
					return false;
				}
			}
		}

		utils->Dispose();
		return true;
	}

	// Check to make sure the source file exists
	if (!utils->sFileInfo->Exists)
	{
		utils->Dispose();
		return false;
	}
	// If the source and destination are the same name...just say we did it
	// Do we have to use a wilcard capable compare?
	if (source->IndexOf("*") != -1 || target->IndexOf("*") != -1)
	{	
		if (MOG_StringCompare(source, target))
		{
			utils->Dispose();
			return true;
		}
	}
	else
	{
		if (String::Compare(source, target, true) == 0)
		{
			utils->Dispose();
			return true;
		}
	}

	// Check to make sure we aren't overwiting an existing file
	if (utils->tFileInfo->Exists)
	{
		// Overwrite this file?
		if (!overwrite)
		{
			utils->Dispose();
			return false;
		}

		try
		{
			// Make sure we always clear the read Only flag!
			utils->tFileInfo->Attributes = FileAttributes::Normal;
			utils->tFileInfo->Delete();
		}
		catch(Exception *e)
		{
			mLastKnownError = e->Message;
				String *message = String::Concat(	S"Could not delete: Access is denied.",
													S"   ", target, S"\n\n",
													S"Make sure the disk is not full or write-protected and that the file is not currently in use.");
			MOG_Report::ReportMessage(S"FileMove Error", message, e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
			utils->Dispose();
			return false;
		}
	}

	// Make sure that the target directory exists before we move
	if (!utils->tFileInfo->Directory->Exists)
	{
		try
		{
			utils->tFileInfo->Directory->Create();

//			// Set the file to be Normal & Archived
//			File::SetAttributes(utils->tFileInfo->Directory->FullName, FileAttributes::Normal);
//			File::SetAttributes(utils->tFileInfo->Directory->FullName, static_cast<FileAttributes>(File::GetAttributes(utils->tFileInfo->Directory->FullName) | FileAttributes::Archive));
		}
		catch(Exception *e)
		{
			mLastKnownError = e->Message;
			MOG_Report::ReportMessage(S"FileMove Error", String::Concat(S"Could not create parent directory for ", target, S" (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
			utils->Dispose();
			return false;
		}
	}

	try
	{
		utils->sFileInfo->MoveTo(utils->tFullName);
		utils->tFileInfo->Refresh();
		
//		// Set the file to be Normal & Archived
//		File::SetAttributes(utils->tFullName, FileAttributes::Normal);
//		File::SetAttributes(utils->tFullName, static_cast<FileAttributes>(File::GetAttributes(utils->tFullName) | FileAttributes::Archive));
	}
	catch(Exception *e)
	{
		mLastKnownError = e->Message;
		MOG_Report::ReportMessage(S"FileMove Error", String::Concat(S"Could not move ", source, S" to ", target, S" (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
		utils->Dispose();
		return false;
	}

	utils->Dispose();
	return true;
}


bool DosUtils::FileMoveFast(String *source, String *target, bool overwrite)
{
	FileInfo* srcFileInfo = NULL;
	FileInfo* dstFileInfo = NULL;

	try
	{
		srcFileInfo = new FileInfo(source);
		dstFileInfo = new FileInfo(target);
	}
	catch (Exception* e)
	{
		mLastKnownError = e->Message;
		MOG_Report::ReportMessage(S"FileMoveFast Error", String::Concat(S"Could not get FileInfo for", source, S" or ", target, S" (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
		return false;
	}
	
	// Check to make sure the source file exists
	if (!srcFileInfo->Exists)
	{
		return false;
	}

	// If the source and destination are the same name...just say we did it
	if (String::Compare(source, target, true) == 0)
	{
		return true;
	}

	// Check to make sure we aren't overwiting an existing file
	if (dstFileInfo->Exists)
	{
		// Overwrite this file?
		if (!overwrite)
		{
			return false;
		}

		try
		{
			// Make sure we always clear the read Only flag!
			dstFileInfo->Attributes = FileAttributes::Normal;
			dstFileInfo->Delete();
		}
		catch(Exception *e)
		{
			mLastKnownError = e->Message;
			String *message = String::Concat(	S"Could not delete: Access is denied.",
												S"   ", target, S"\n\n",
												S"Make sure the disk is not full or write-protected and that the file is not currently in use.");
			MOG_Report::ReportMessage(S"FileMoveFast Error", message, e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
			return false;
		}
	}

	// Make sure that the target directory exists before we move
	if (!dstFileInfo->Directory->Exists)
	{
		try
		{
			dstFileInfo->Directory->Create();
		}
		catch(Exception *e)
		{
			mLastKnownError = e->Message;
			MOG_Report::ReportMessage(S"FileMoveFast Error", String::Concat(S"Could not create parent directory for ", target, S" (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
			return false;
		}
	}

	int dialogId = 0;
	int violationStage = 0;
	bool retval = true;
	DateTime warnTime = DateTime::Now.AddSeconds(5);

	while (true)
	{
		try
		{
			srcFileInfo->MoveTo(target);
			dstFileInfo->Refresh();
			break;
		}
		catch(IO::IOException* e)
		{
			if (dialogId == 0)
			{
				dialogId = MOG_Progress::ProgressSetup(S"File Sharing Violation", String::Concat(	S"Please wait while MOG attempts to access the file.\n",
																									S"FILE: ", source, S"\n\n",
																									e->Message));
			}
			
			if (MOG_Prompt::IsMode(MOG_PROMPT_SILENT))
			{
				violationStage = SilentSharingViolationUpdate(source, violationStage, warnTime, e->Message, e->StackTrace);
			}
			else
			{
				if (MOG_Progress::ProgressStatus(dialogId) == MOGPromptResult::Cancel ||
					DateTime::Now > warnTime)
				{
					mLastKnownError = e->Message;
					MOG_Report::ReportMessage(S"FileMoveFast Error", String::Concat(S"MOG timed out while waiting for access to the file: ", source, S" (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
					retval = false;
					break;
				}
			}

			Thread::Sleep(100);
		}
		catch(Exception *e)
		{
			mLastKnownError = e->Message;
			MOG_Report::ReportMessage(S"FileMoveFast Error", String::Concat(S"Could not move ", source, S" to ", target, S" (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
			retval = false;
			break;
		}
	}

	MOG_Progress::ProgressClose(dialogId);
	return retval;
}


bool DosUtils::FileRename(String *source, String *target, bool overwrite)
{
	return FileMove(source, target, overwrite, false);
}

bool DosUtils::FileRenameFast(String *source, String *target, bool overwrite)
{
	return FileMoveFast(source, target, overwrite);
}

bool DosUtils::FileDelete(String *filename)
{
	return FileDeleteFast(filename);
}

bool DosUtils::FileDeleteFast(String *filename)
{
	FileInfo *file = NULL;
	
	try
	{
		file = new FileInfo(filename);

		if (file && file->Exists)
		{
			// Make sure we always clear the read Only flag!
			file->Attributes = FileAttributes::Normal;
			file->Delete();
		}
	}
	catch(Exception *e)
	{
		mLastKnownError = e->Message;
		String *message = String::Concat(	S"Could not delete: Access is denied.",
											S"   ", filename, S"\n\n",
											S"Make sure the disk is not full or write-protected and that the file is not currently in use.");
		MOG_Report::ReportMessage(S"FileDelete Error", message, e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
		return false;
	}

	return true;
}

bool DosUtils::FileCloseSlk(String *filename)
{
	return AppendTextToFile(filename, "E");
}

bool DosUtils::AppendTextToSlkFile(String *filename, String *text)
{
	static int Y = 1;

	if (!FileExist(filename))
	{
		// Create the header
		AppendTextToFile(filename, String::Concat(S"ID;PWXL;N;E\r\n",
								S"P;PGeneral\r\n",
								S"P;P0\r\n",
								S"P;P0.00\r\n",
								S"P;P#,##0\r\n",
								S"P;P#,##0.00\r\n",
								S"P;P#,##0_);;\\(#,##0\\)\r\n",
								S"P;P#,##0_);;[Red]\\(#,##0\\)\r\n",
								S"P;P#,##0.00_);;\\(#,##0.00\\)\r\n",
								S"P;P#,##0.00_);;[Red]\\(#,##0.00\\)\r\n",
								S"P;P\"$\"#,##0_);;\\(\"$\"#,##0\\)\r\n",
								S"P;P\"$\"#,##0_);;[Red]\\(\"$\"#,##0\\)\r\n",
								S"P;P\"$\"#,##0.00_);;\\(\"$\"#,##0.00\\)\r\n",
								S"P;P\"$\"#,##0.00_);;[Red]\\(\"$\"#,##0.00\\)\r\n",
								S"P;P0%\r\n",
								S"P;P0.00%\r\n",
								S"P;P0.00E+00\r\n",
								S"P;P##0.0E+0\r\n",
								S"P;P#\\ ?/?\r\n",
								S"P;P#\\ ??/??\r\n",
								S"P;Pm/d/yyyy\r\n",
								S"P;Pd\\-mmm\\-yy\r\n",
								S"P;Pd\\-mmm\r\n",
								S"P;Pmmm\\-yy\r\n",
								S"P;Ph:mm\\ AM/PM\r\n",
								S"P;Ph:mm:ss\\ AM/PM\r\n",
								S"P;Ph:mm\r\n",
								S"P;Ph:mm:ss\r\n",
								S"P;Pm/d/yyyy\\ h:mm\r\n",
								S"P;Pmm:ss\r\n",
								S"P;Pmm:ss.0\r\n",
								S"P;P@\r\n",
								S"P;P[h]:mm:ss\r\n",
								S"P;P_(\"$\"* #,##0_);;_(\"$\"* \\(#,##0\\);;_(\"$\"* \"-\"_);;_(@_)\r\n",
								S"P;P_(* #,##0_);;_(* \\(#,##0\\);;_(* \"-\"_);;_(@_)\r\n",
								S"P;P_(\"$\"* #,##0.00_);;_(\"$\"* \\(#,##0.00\\);;_(\"$\"* \"-\"??_);;_(@_)\r\n",
								S"P;P_(* #,##0.00_);;_(* \\(#,##0.00\\);;_(* \"-\"??_);;_(@_)\r\n",
								S"P;FArial;M200\r\n",
								S"P;FArial;M200\r\n",
								S"P;FArial;M200\r\n",
								S"P;FArial;M200\r\n",
								S"F;P0;DG0G8;M255\r\n",
								S"O;L;D;V0;K47;G100 0.001\r\n"));
		Y = 1;

		return AppendTextToSlkFile(filename, text);
	}
	else
	{
		// Split out the string into sub strings
		String* delimStr = S"\t";
		Char delimiter[] = delimStr->ToCharArray();
		String* cells[] = text->Split(delimiter);

		for (int i = 1; i < cells->Count+1; i++)
		{
			if (!AppendTextToFile(filename, String::Concat(S"C;Y", Convert::ToString(Y), S";X", Convert::ToString(i), S";K\"", cells[i-1]->Trim(), S"\"\r\n")))
			{
				MOG_Report::ReportMessage(S"AppendTextToSlkFile Error", String::Concat(S"Could not append to file (", mLastKnownError, S")\n"), Environment::StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
				return false;
			}
		}

		Y++;
	}

	return true;
}

bool DosUtils::AppendTextToFile(String *filename, String *text)
{
	bool done = false;
	FileStream *fs;

	try
	{
		fs = new FileStream(filename, FileMode::Append, FileAccess::Write);
	}
	catch(Exception *e)
	{
		mLastKnownError = e->Message;
		MOG_Report::ReportMessage(S"AppendTextToFile Error", String::Concat(S"Could not write file (", filename, S") because of error:"), e->Message, MOG_ALERT_LEVEL::ALERT);
		return false;
	}

	if (fs)
	{
		StreamWriter *w = new StreamWriter(fs);
		if (w)
		{
			try
			{
				w->Write(text);
				w->Close();
			}
			catch(Exception *e)
			{
				mLastKnownError = e->Message;
				MOG_Report::ReportMessage(S"AppendTextToFile Error", String::Concat(S"Could not write file (", filename, S") because of error:"), e->Message, MOG_ALERT_LEVEL::ALERT);
				return false;
			}


			done = true;
		}
		fs->Close();
	}

	return done;

#if BINARY_FILE_FORMAT
	FileInfo *file = new FileInfo(filename);
	StreamWriter *stream = file->AppendText();

	stream->Write(text);
	stream->Flush();
	stream->Close();

	return true;
#endif
}

bool DosUtils::FileWrite(String *filename, String *text)
{
	bool done = false;

	FileInfo *file = new FileInfo(filename);

	if (file->Exists)
	{
		// Make sure we always clear the read Only flag!
		file->Attributes = FileAttributes::Normal;
		file->Delete();
	}

	FileStream *fs;

	try
	{
		fs = new FileStream(filename, FileMode::CreateNew, FileAccess::Write);		
	}
	catch(Exception *e)
	{
		mLastKnownError = e->Message;
		MOG_Report::ReportMessage(S"FileWrite Error", String::Concat(S"Could not write file (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
		return false;
	}
	
	if (fs)
	{
		StreamWriter *w = new StreamWriter(fs);
		if (w)
		{
			try
			{
				w->NewLine = "\n";
				w->Write(text);
				w->Close();
			}
			catch(Exception *e)
			{
				mLastKnownError = e->Message;
				MOG_Report::ReportMessage(S"AppendTextToFile Error", String::Concat(S"Could not write file (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
				return false;
			}


			done = true;
		}
		fs->Close();
	}

	return done;
}

String *DosUtils::PathGetDrive(String *filename)
{
	String* drive = S"";

	if (filename)
	{
		try
		{
			if (filename->IndexOf(":") != -1)
			{
				drive = filename->Substring(0, filename->IndexOf(":"));			
			}
		}
		catch(Exception *ex)
		{
			MOG_Report::ReportMessage(S"Path/Filename Error", ex->Message, ex->StackTrace, MOG_ALERT_LEVEL::ERROR);
		}
	}

	return drive;
}

bool DosUtils::PathIsDriveRooted(String *filename)
{
	if (filename)
	{
		try
		{
			if (filename->IndexOf(":") != -1)
			{
				return true;
			}
		}
		catch(Exception *ex)
		{
			MOG_Report::ReportMessage(S"Path/Filename Error", ex->Message, ex->StackTrace, MOG_ALERT_LEVEL::ERROR);
		}
	}

	return false;
}

bool DosUtils::PathIsWithinPath(String* parentPath, String* childPath)
{
	if (parentPath &&
		childPath)
	{
		try
		{
			// Append on a trailing '\' to both strings to resolve the possibility of near matches
			String *testParentPath = String::Concat(parentPath, S"\\");
			String *testChildPath = String::Concat(childPath, S"\\");
			if (testChildPath->StartsWith(testParentPath, StringComparison::CurrentCultureIgnoreCase))
			{
				// Indicate this path is within the parentPath
				return true;
			}

		}
		catch(Exception *ex)
		{
			MOG_Report::ReportMessage(S"Path/Filename Error", ex->Message, ex->StackTrace, MOG_ALERT_LEVEL::ERROR);
		}
	}

	return false;
}

String *DosUtils::PathMakeRelativePath(String* rootPath, String* fullPath)
{
	String* relativePath = fullPath;

	if (rootPath && 
		fullPath)
	{
		try
		{
			if (rootPath->Length)
			{
				// Append on a trailing '\' to both strings to resolve the possibility of near matches
				String *testRootPath = String::Concat(rootPath, S"\\");
				String *testFullPath = String::Concat(fullPath, S"\\");
				if (testFullPath->StartsWith(testRootPath, StringComparison::CurrentCultureIgnoreCase))
				{
					// Always strip off the workspaceDirectory if it exists so we become relative
					relativePath = testFullPath->Substring(testRootPath->Length)->Trim(S"\\"->ToCharArray());
				}
			}
		}
		catch(Exception *ex)
		{
			MOG_Report::ReportMessage(S"Path/Filename Error", ex->Message, ex->StackTrace, MOG_ALERT_LEVEL::ERROR);
		}
	}

	return relativePath;
}

String *DosUtils::PathGetFileName(String *filename)
{
	if (filename)
	{
		try
		{
// Removed because we now support virual names that can exceed 260
//			filename = Path::GetFileName(CheckForInvalidCharactersInPath(filename));
			// Get the last index of the path seperator
			int pos = filename->LastIndexOf("\\");
			if (pos != -1)
			{
				filename = filename->Substring((pos + 1), filename->Length - (pos + 1));
			}

			// Manually strip out all invalid characters
			filename = CheckForInvalidCharactersInPath(filename);
		}
		catch(Exception *ex)
		{
			MOG_Report::ReportMessage(S"Path/Filename Error", ex->Message, ex->StackTrace, MOG_ALERT_LEVEL::ERROR);
		}
	}

	return filename;
}

String *DosUtils::PathGetRootPath(String *filename)
{
	String *root = S"";

	if (filename)
	{
		try
		{
// Removed because we now support virual names that can exceed 260
//			filename = Path::GetPathRoot(CheckForInvalidCharactersInPath(filename));
			// Get the index of the :\ for drive letter
			int pos = filename->IndexOf(":");
			if (pos != -1)
			{
				// Check to see if this root ends has a backslash
				int backSlashPos = filename->IndexOf("\\", pos);
				if (backSlashPos != -1)
				{
					// If so, return the root with the backslash
					root = filename->Substring(0, backSlashPos + 1);
				}
				else
				{
					// Else return it without
					root = filename;
				}
			}
			else
			{
				// Check to see if this is a machine name root
				if (filename->StartsWith("\\\\"))
				{
					// Make sure we have a string length long enough to handle this assumption
					if (filename->Length >= 2)
					{
						// Check to see if this root has a backslash after the initial 2 backslashes
						int pos = filename->IndexOf("\\", 2);
						if (pos != -1 && filename->Length > pos)
						{
							// If so, return the root with the backslash
							root = filename->Substring(0, pos + 1);
						}					
					}


					// If we don't have a root, that means that this filename is only the root
					if (!root->Length)
					{
						// Else return it without
						root = filename;
					}
				}
			}

			// Manually strip out all invalid characters
			root = CheckForInvalidCharactersInPath(root);
		}
		catch(Exception *ex)
		{
			MOG_Report::ReportMessage(S"Path/Filename Error", ex->Message, ex->StackTrace, MOG_ALERT_LEVEL::ERROR);
		}
	}

	return root;
}

String *DosUtils::PathGetDirectoryPath(String *filename)
{
	// Strip out all invalid characters
	String *directory = S"";

	if (filename)
	{
		try
		{
// Removed because we now support virual names that can exceed 260
//			filename = Path::GetDirectoryName(CheckForInvalidCharactersInPath(filename));
			// Get the last index of the path seperator
			int pos = filename->LastIndexOf("\\");
			if (pos != -1)
			{
				// Extract the path and strip out all invalid characters
				directory = CheckForInvalidCharactersInPath(filename->Substring(0, pos));
			}
		}
		catch(Exception *ex)
		{
			MOG_Report::ReportMessage(S"Path/Filename Error", ex->Message, ex->StackTrace, MOG_ALERT_LEVEL::ERROR);
		}
	}

	return directory;
}

String *DosUtils::PathEnsureFullPath(String *path, String *defaultRootPath)
{
	// Check if we are missing a root?
	if (PathGetRootPath(path)->Length == 0)
	{
		// Check if the path dosn't already start with the desired root?
		if (!path->ToLower()->StartsWith(defaultRootPath->ToLower()))
		{
			// Append the two strings together
			path = String::Concat(defaultRootPath->TrimEnd(S"\\"->ToCharArray()), S"\\", path->TrimStart(S"\\"->ToCharArray()));
		}
	}
	return path;
}

String *DosUtils::PathGetExtension(String *filename)
{
	String *extension = S"";

	if (filename)
	{
		try
		{
// Removed because we now support virual names that can exceed 260
//			filename = Path::GetExtension(CheckForInvalidCharactersInPath(filename));
//			if (filename->Length)
//			{
//				//Strip off the preceding period
//				filename = filename->Substring(1);
//			}
			// Get only the filename
			filename = PathGetFileName(filename);
			// Get the last index of the path seperator
			int pos = filename->LastIndexOf(".");
			if (pos != -1)
			{
				extension = filename->Substring((pos + 1), filename->Length - (pos + 1));
			}

			// Manually strip out all invalid characters
			extension = CheckForInvalidCharactersInPath(extension);
		}
		catch(Exception *ex)
		{
			MOG_Report::ReportMessage(S"Path/Filename Error", ex->Message, ex->StackTrace, MOG_ALERT_LEVEL::ERROR);
		}
	}

	return extension;
}

String *DosUtils::PathGetFileNameWithoutExtension(String *filename)
{
	if (filename)
	{
		try
		{
// Removed because we now support virual names that can exceed 260
//			filename = Path::GetFileNameWithoutExtension(CheckForInvalidCharactersInPath(filename));
			// Obtain the filename using our other API
			filename = PathGetFileName(filename);
			// Get the last index of the path seperator
			int pos = filename->LastIndexOf(".");
			if (pos != -1)
			{
				filename = filename->Substring(0, pos);
			}
		}
		catch(Exception *ex)
		{
			MOG_Report::ReportMessage(S"Path/Filename Error", ex->Message, ex->StackTrace, MOG_ALERT_LEVEL::ERROR);
		}
	}

	return filename;
}

String *DosUtils::CheckForInvalidCharactersInPath(String *filename)
{
	if (filename)
	{
		// Do we have any invalid characters?
		if(filename->IndexOfAny(Path::InvalidPathChars) >= 0)
		{
			// Go through our Path::InvalidPathChars to get rid of them...
			for(int i = 0; i < Path::InvalidPathChars->Length; ++i)
			{
				filename = filename->Replace(Path::InvalidPathChars[i].ToString(), S"");
			}

			// Warn the user
			MOG_Prompt::PromptMessage(	S"Invalid Path/Filename", 
										String::Concat(	S"MOG has detected and removed invalid characters contained in the specified path.\r\n",
														S"INVALID CHARACTERS: ", new String(Path::InvalidPathChars)));
		}
	}

	return filename;
}

String* DosUtils::FileRead(String *filename)
{
	String* contents = S"";
	FileInfo *file = new FileInfo(filename);
	FileStream *fs;

	try
	{
		fs = new FileStream(filename, FileMode::Open, FileAccess::Read);
	}
	catch(Exception *e)
	{
		mLastKnownError = e->Message;
		MOG_Report::ReportMessage(S"FileOpen error", String::Concat(S"Could not open file for reading(", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
		return false;
	}
	
	if (fs)
	{
		StreamReader *r = new StreamReader(fs);
		if (r)
		{
			try
			{
				contents = r->ReadToEnd();
				r->Close();
			}
			catch(Exception *e)
			{
				mLastKnownError = e->Message;
				MOG_Report::ReportMessage(S"FileRead Error", String::Concat(S"Could not read file (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
				return false;
			}
		}
		fs->Close();
	}

	return contents;
}

bool DosUtils::DirectoryExist(String *directory)
{
	// Get the common file information
	DosUtils *utils = new DosUtils;
	utils->Initialize(directory, "");

	// Does this source contain wild cards?
	if (utils->sContainsWildCard)
	{
		// Get list of files that match the file pattern
		DirectoryInfo *directories[] = utils->Get_sPathInfo()->GetDirectories(utils->sName);

		// check each file seperately
		for (int f = 0; f < directories->Count; f++)
		{
			// Make sure the each directory copies or else we should quit
			if (DirectoryExist(String::Concat(utils->sPath, S"\\", directories[f]->Name)))
			{
				return true;	// One matches return true.
			}
		}

		return false;	// Nothing matched.
	}

	// Make sure we have a directory name to check
	if (utils->sFullName->Length)
	{
		DirectoryInfo *dir = new DirectoryInfo(utils->sFullName);
		return dir->Exists;
	}

	return false;
}


bool DosUtils::DirectoryExistFast(String *directory)
{
	// Make sure we have a directory name to check
	if (directory && directory->Length)
	{
		try
		{
			DirectoryInfo *dir = new DirectoryInfo(directory);
			return dir->Exists;
		}
		catch (Exception* e)
		{
			e->Message;
			MOG_Report::ReportMessage(S"DirectoryExistFast Error", String::Concat(S"Could not get DirectoryInfo for ", directory, S" (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
			return false;
		}
	}

	return false;
}


long long DosUtils::DirectoryGetSize(String *directory)
{
	long long size = 0;
	int i;

	try
	{
		DirectoryInfo* dir = new DirectoryInfo(directory);
		DirectoryInfo* subDirs[] = dir->GetDirectories();
		FileInfo* files[] = dir->GetFiles();

		//Add up the sizes of all the files in this directory
		for (i = 0; i < files->Length; i++)
		{
			size += files[i]->Length;
		}

		//Recurse through all the subdirectories and get their sizes too
		for (i = 0; i < subDirs->Length; i++)
		{
			size += DirectoryGetSize(subDirs[i]->FullName);
		}
	}
	catch (Exception* e)
	{
		mLastKnownError = e->Message;
		MOG_Report::ReportMessage(S"DirectoryGetSize Error", String::Concat(S"Could not get size of ", directory, S" (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
	}

	return size;
}

bool DosUtils::DirectoryCopyModified(String *source, String *target)
{
	// Get the common file information
	DosUtils *utils = new DosUtils;
	utils->Initialize(source, target);

	// Does this source contain wild cards?
	// JKN hopefully this will not break anything!!
	if (utils->sContainsWildCard)
	{
		// Get list of files that match the file pattern
		DirectoryInfo *directories[] = utils->Get_sPathInfo()->GetDirectories(utils->sName);
		// Copy each file seperately
		for (int f = 0; f < directories->Count; f++)
		{
			// Make sure the each file copies or else we should quit
			if (!DirectoryCopyModified(directories[f]->FullName, String::Concat(utils->tPath, S"\\", directories[f]->Name)))
			{
				return false;
			}
		}
		utils->Dispose();
		return true;
	}

	// Check to make sure the source file exists
	MOG_ASSERT_THROW(utils->sDirInfo->Exists, MOG_Exception::MOG_EXCEPTION_MissingData, "attempting to copy non existing directory.");

	// If the source and destination are the same name...just say we did it
	// Do we have to use a wilcard capable compare?
	if (source->IndexOf("*") != -1 || target->IndexOf("*") != -1)
	{
		if (MOG_StringCompare(source, target))
		{
			utils->Dispose();
			return true;
		}
	}
	else
	{
		if (String::Compare(source, target, true) == 0)
		{
			utils->Dispose();
			return true;
		}
	}
	
	// Make sure that the target's parent directory exists before we copy anything
	if (!utils->tDirInfo->Parent->Exists)
	{
		try
		{
			utils->tDirInfo->Parent->Create();

//			// Set the file to be Normal & Archived
//			File::SetAttributes(utils->tDirInfo->Parent->FullName, FileAttributes::Normal);
//			File::SetAttributes(utils->tDirInfo->Parent->FullName, static_cast<FileAttributes>(File::GetAttributes(utils->tDirInfo->Parent->FullName) | FileAttributes::Archive));
		}
		catch(Exception *e)
		{
			mLastKnownError = e->Message;
			MOG_Report::ReportMessage(S"DirectoryCopyModified Error", String::Concat(S"Could not create parent directory for ", target, S" (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
			utils->Dispose();
			return false;
		}

	}

	// Copy the directories one at a time
	DirectoryInfo *directories[] = utils->sDirInfo->GetDirectories();
	for(int d = 0; d < directories->Count; d++)
	{
		// Recursively call ourselves
		DirectoryCopyModified(directories[d]->FullName, String::Concat(utils->tDirInfo->FullName, "\\", directories[d]->Name));
	}

	// Make sure that the target directory exists before copy files
	if (!utils->tDirInfo->Exists)
	{
		try
		{
			utils->tDirInfo->Create();

//			// Set the file to be Normal & Archived
//			File::SetAttributes(utils->tDirInfo->FullName, FileAttributes::Normal);
//			File::SetAttributes(utils->tDirInfo->FullName, static_cast<FileAttributes>(File::GetAttributes(utils->tDirInfo->FullName) | FileAttributes::Archive));
		}
		catch(Exception *e)
		{
			mLastKnownError = e->Message;
			MOG_Report::ReportMessage(S"DirectoryCopyModified Error", String::Concat(S"Could not create target directory ", target, S" (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
			utils->Dispose();
			return false;
		}
	}
	// Copy each file one at a time
	FileInfo *files[] = utils->sDirInfo->GetFiles();
	for(int f = 0; f < files->Count; f++)
	{
		try
		{
			String *targetFilename = String::Concat(utils->tDirInfo->FullName, "\\", files[f]->Name);
			FileInfo *targetInfo = new FileInfo(targetFilename);
			
			// Determine if we need to update this file  (Must Use ToFileTime() because sometimes LastWriteTime comparisons can fail)
			if (files[f]->LastWriteTime.ToFileTime() != targetInfo->LastWriteTime.ToFileTime())
			{
				// Make sure the destination file doesn't exist
				if (targetInfo->Exists)
				{
					// Make sure we always clear the read Only flag!
					targetInfo->Attributes = FileAttributes::Normal;
					targetInfo->Delete();
				}
				// Copy the file
				files[f]->CopyTo(targetFilename);

//				// Set the file to be Normal & Archived
//				File::SetAttributes(targetFilename, FileAttributes::Normal);
//				File::SetAttributes(targetFilename, static_cast<FileAttributes>(File::GetAttributes(targetFilename) | FileAttributes::Archive));
			}
		}
		catch(Exception *e)
		{
			mLastKnownError = e->Message;
			MOG_Report::ReportMessage(S"DirectoryCopyModified Error", String::Concat(S"Could not copy ", source, S" to ", target, S" (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
			utils->Dispose();
			return false;
		}
	}

	utils->Dispose();
	return true;
}


bool DosUtils::DirectoryCopy(String *source, String *target, bool overwrite)
{
	// Get the common file information
	DosUtils *utils = new DosUtils;
	utils->Initialize(source, target);

	// Does this source contain wild cards?
	// JKN hopefully this will not break anything!!
	if (utils->sContainsWildCard)
	{
		// Get list of files that match the file pattern
		DirectoryInfo *directories[] = utils->Get_sPathInfo()->GetDirectories(utils->sName);
		// Copy each file seperately
		for (int f = 0; f < directories->Count; f++)
		{
			// Make sure the each file copies or else we should quit
			if (!DirectoryCopy(directories[f]->FullName, String::Concat(utils->tPath, S"\\", directories[f]->Name), overwrite))
			{
				return false;
			}
		}
		utils->Dispose();
		return true;
	}

	// Check to make sure the source file exists
	MOG_ASSERT_THROW(utils->sDirInfo->Exists, MOG_Exception::MOG_EXCEPTION_MissingData, "attempting to copy non existing directory.");

	// If the source and destination are the same name...just say we did it
	// Do we have to use a wilcard capable compare?
	if (source->IndexOf("*") != -1 || target->IndexOf("*") != -1)
	{
		if (MOG_StringCompare(source, target))
		{
			utils->Dispose();
			return true;
		}
	}
	else
	{
		if (String::Compare(source, target, true) == 0)
		{
			utils->Dispose();
			return true;
		}
	}

	// Check to make sure we aren't overwiting an existing directory
	if (utils->tDirInfo->Exists)
	{
		// Overwrite this directory?
		if (!overwrite)
		{
			utils->Dispose();
			return false;
		}
	}

	// Make sure that the target's parent directory exists before we copy anything
	if (!utils->tDirInfo->Parent->Exists)
	{
		try
		{
			utils->tDirInfo->Parent->Create();

//			// Set the file to be Normal & Archived
//			File::SetAttributes(utils->tDirInfo->Parent->FullName, FileAttributes::Normal);
//			File::SetAttributes(utils->tDirInfo->Parent->FullName, static_cast<FileAttributes>(File::GetAttributes(utils->tDirInfo->Parent->FullName) | FileAttributes::Archive));
		}
		catch(Exception *e)
		{
			mLastKnownError = e->Message;
			MOG_Report::ReportMessage(S"DirectoryCopy Error", String::Concat(S"Could not create parent directory for ", target, S" (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
			utils->Dispose();
			return false;
		}

	}

	// Copy the directories one at a time
	DirectoryInfo *directories[] = utils->sDirInfo->GetDirectories();
	for(int d = 0; d < directories->Count; d++)
	{
		// Recursively call ourselves
		DirectoryCopy(directories[d]->FullName, String::Concat(utils->tDirInfo->FullName, "\\", directories[d]->Name));
	}

	// Make sure that the target directory exists before copy files
	if (!utils->tDirInfo->Exists)
	{
		try
		{
			utils->tDirInfo->Create();

//			// Set the file to be Normal & Archived
//			File::SetAttributes(utils->tDirInfo->FullName, FileAttributes::Normal);
//			File::SetAttributes(utils->tDirInfo->FullName, static_cast<FileAttributes>(File::GetAttributes(utils->tDirInfo->FullName) | FileAttributes::Archive));
		}
		catch(Exception *e)
		{
			mLastKnownError = e->Message;
			MOG_Report::ReportMessage(S"DirectoryCopy Error", String::Concat(S"Could not create target directory ", target, S" (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
			utils->Dispose();
			return false;
		}
	}
	// Copy each file one at a time
	FileInfo *files[] = utils->sDirInfo->GetFiles();
	for(int f = 0; f < files->Count; f++)
	{
		try
		{
			String *targetFilename = String::Concat(utils->tDirInfo->FullName, "\\", files[f]->Name);
			// Make sure the destination file doesn't exist
			FileInfo *targetInfo = new FileInfo(targetFilename);
			if (targetInfo->Exists)
			{
				// Make sure we always clear the read Only flag!
				targetInfo->Attributes = FileAttributes::Normal;
				targetInfo->Delete();
			}
			// Copy the file
			files[f]->CopyTo(targetFilename);
			
//			// Set the file to be Normal & Archived
//			File::SetAttributes(targetFilename, FileAttributes::Normal);
//			File::SetAttributes(targetFilename, static_cast<FileAttributes>(File::GetAttributes(targetFilename) | FileAttributes::Archive));
		}
		catch(Exception *e)
		{
			mLastKnownError = e->Message;
			MOG_Report::ReportMessage(S"DirectoryCopy Error", String::Concat(S"Could not copy ", source, S" to ", target, S" (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
			utils->Dispose();
			return false;
		}
	}

	utils->Dispose();
	return true;
}


bool DosUtils::DirectoryCopyFast(String *source, String *target, bool overwrite)
{
	DirectoryInfo* srcDirInfo = NULL;
	DirectoryInfo* dstDirInfo = NULL;

	try
	{
		srcDirInfo = new DirectoryInfo(source);
		dstDirInfo = new DirectoryInfo(target);
	}
	catch (Exception* e)
	{
		mLastKnownError = e->Message;
		MOG_Report::ReportMessage(S"DirectoryCopyFast Error", String::Concat(S"Could not get DirectoryInfo for ", source, S" or ", target, S" (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
		return false;
	}

	// If the source and destination are the same name...just say we did it
	if (String::Compare(source, target, true) == 0)
	{
		return true;
	}

	// Check to make sure we aren't overwiting an existing directory
	if (dstDirInfo->Exists)
	{
		// Overwrite this directory?
		if (!overwrite)
		{
			return false;
		}
	}

	// Make sure that the target's parent directory exists before we copy anything
	if (!dstDirInfo->Parent->Exists)
	{
		try
		{
			dstDirInfo->Parent->Create();
		}
		catch(Exception *e)
		{
			mLastKnownError = e->Message;
			MOG_Report::ReportMessage(S"DirectoryCopyFast Error", String::Concat(S"Could not create parent directory for ", target, S" (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
			return false;
		}
	}

	// Copy the directories one at a time
	DirectoryInfo *directories[] = srcDirInfo->GetDirectories();
	for(int d = 0; d < directories->Count; d++)
	{
		// Recursively call ourselves
		DirectoryCopyFast(directories[d]->FullName, String::Concat(target, "\\", directories[d]->Name), overwrite);
	}

	// Make sure that the target directory exists before copy files
	if (!dstDirInfo->Exists)
	{
		try
		{
			dstDirInfo->Create();
		}
		catch(Exception *e)
		{
			mLastKnownError = e->Message;
			MOG_Report::ReportMessage(S"DirectoryCopyFast Error", String::Concat(S"Could not create target directory ", target, S" (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
			return false;
		}
	}

	bool retval = true;
	
	// Copy each file one at a time
	FileInfo *files[] = srcDirInfo->GetFiles();
	for(int f = 0; f < files->Count; f++)
	{
		int dialogId = 0;
		int violationStage = 0;
		DateTime warnTime = DateTime::Now.AddSeconds(5);

		while (true)
		{
			try
			{
				String *targetFilename = String::Concat(target, "\\", files[f]->Name);
				// Make sure the destination file doesn't exist
				FileInfo *targetInfo = new FileInfo(targetFilename);
				if (targetInfo->Exists)
				{
					// Make sure we always clear the read Only flag!
					targetInfo->Attributes = FileAttributes::Normal;
					targetInfo->Delete();
				}
				// Copy the file
				files[f]->CopyTo(targetFilename);
				break;
			}
			catch(IO::IOException* e)
			{
				if (dialogId == 0)
				{
					dialogId = MOG_Progress::ProgressSetup(S"File Sharing Violation", String::Concat(	S"Please wait while MOG attempts to access the file.\n",
																										S"FILE: ", target, S"\n\n",
																										e->Message));
				}
				
				if (MOG_Prompt::IsMode(MOG_PROMPT_SILENT))
				{
					violationStage = SilentSharingViolationUpdate(target, violationStage, warnTime, e->Message, e->StackTrace);
				}
				else
				{
					if (MOG_Progress::ProgressStatus(dialogId) == MOGPromptResult::Cancel ||
						DateTime::Now > warnTime)
					{
						mLastKnownError = e->Message;
						MOG_Report::ReportMessage(S"DirectoryCopyFast Error", String::Concat(S"MOG timed out while waiting for access to the file: ", target, S" (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
						retval = false;
						break;
					}
				}

				Thread::Sleep(100);
			}
			catch(Exception *e)
			{
				mLastKnownError = e->Message;
				MOG_Report::ReportMessage(S"DirectoryCopyFast Error", String::Concat(S"Could not copy ", source, S" to ", target, S" (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
				retval = false;
				break;
			}
		}

		MOG_Progress::ProgressClose(dialogId);
	}

	return retval;
}


bool DosUtils::DirectoryMove(String *source, String *target, bool overwrite)
{
	// Get the common file information
	DosUtils *utils = new DosUtils;
	utils->Initialize(source, target);

	// Check to make sure the source directory exists
	if (!utils || !utils->sDirInfo || !utils->sDirInfo->Exists)
	{
		utils->Dispose();
		return false;
	}
	// If the source and destination are the same name...just say we did it
	// Do we have to use a wilcard capable compare?
	if (source->IndexOf("*") != -1 || target->IndexOf("*") != -1)
	{
		if (MOG_StringCompare(source, target))
		{
			utils->Dispose();
			return true;
		}
	}
	else
	{
		if (String::Compare(source, target, true) == 0)
		{
			utils->Dispose();
			return true;
		}
	}

	// Check to make sure we aren't overwiting an existing directory
	if (utils->tDirInfo->Exists)
	{
		// Overwrite this directory?
		if (!overwrite)
		{
			utils->Dispose();
			return false;
		}

		bool retVal = FileMove(String::Concat(source, "\\*.*"), target, 1, true);	//move all files and directories
		
		// If it worked delete the directory we just moved everything from.  It should be empty.
		if (retVal)
		{
			// Make sure we always clear the read Only flag!
			SetAttributesRecursive(source, S"*.*", FileAttributes::Normal);
			utils->sDirInfo->Delete(true);
		}
		utils->Dispose();
		return retVal;
	}

	// Check to make sure that the drives are the same
	if (!String::Compare(utils->sDrive, utils->tDrive, true))
	{
		// Make sure that the target's parent directory exists before we move
		if (!utils->tDirInfo->Parent->Exists)
		{
			try
			{
				utils->tDirInfo->Parent->Create();

//				// Set the file to be Normal & Archived
//				File::SetAttributes(utils->tDirInfo->Parent->FullName, FileAttributes::Normal);
//				File::SetAttributes(utils->tDirInfo->Parent->FullName, static_cast<FileAttributes>(File::GetAttributes(utils->tDirInfo->FullName) | FileAttributes::Archive));
			}
			catch(Exception *e)
			{
				mLastKnownError = e->Message;
				MOG_Report::ReportMessage(S"DirectoryMove Error", String::Concat(S"Could not create parent directory for ", target, S" (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
				utils->Dispose();
				return false;
			}

		}

		try
		{
			utils->sDirInfo->MoveTo(utils->tFullName);
		}
		catch(Exception *e)
		{
			mLastKnownError = e->Message;

			// TODO - This should throw!

			//MOG_Report::ReportMessage(S"DirectoryMove Error", String::Concat(S"Could not move directory (", e->Message, S")\n", e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
			utils->Dispose();
			return false;
		}

		try
		{
			utils->tDirInfo->Refresh();
			
//			// Set the file to be Normal & Archived
//			File::SetAttributes(utils->tDirInfo->FullName, FileAttributes::Normal);
//			File::SetAttributes(utils->tDirInfo->FullName, static_cast<FileAttributes>(File::GetAttributes(utils->tDirInfo->FullName) | FileAttributes::Archive));
		}
		catch(Exception *e)
		{
			mLastKnownError = e->Message;
			MOG_Report::ReportMessage(S"DirectoryMove Error", String::Concat(S"Could not Refresh directory (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
			utils->Dispose();
			return false;
		}

		utils->Dispose();
		return true;
	}
	// Guess we can't just move it...we need to do it the hard way
	else
	{
		// Copy it then delete it
		if (DirectoryCopy(utils->sFullName, utils->tFullName))
		{
			bool retVal = DirectoryDelete(utils->sFullName);
			utils->Dispose();
			return retVal;
		}
	}

	utils->Dispose();
	return false;
}


bool DosUtils::DirectoryMoveFast(String *source, String *target, bool overwrite)
{
	DirectoryInfo* srcDirInfo = NULL;
	DirectoryInfo* dstDirInfo = NULL;

	// Check to make sure the source directory exists
	try
	{
		srcDirInfo = new DirectoryInfo(source);
		if (!srcDirInfo->Exists)
			return false;

		dstDirInfo = new DirectoryInfo(target);
	}
	catch (Exception* e)
	{
		mLastKnownError = e->Message;
		MOG_Report::ReportMessage(S"DirectoryMoveFast Error", String::Concat(S"Could not get DirectoryInfo for ", source, S" or ", target, S" (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
		return false;
	}

	// If the source and destination are the same name...just say we did it
	if (String::Compare(source, target) == 0)
	{
		return true;
	}

	// If the target dir already exists or the dirs are on different drives, we need to get fancy
	if (dstDirInfo->Exists || String::Compare(PathGetRootPath(source), PathGetRootPath(target), true) != 0)
	{
		// There are extenuating circumstances preventing us from simply moving the dir
		// Copy it then delete it
		if (DirectoryCopyFast(source, target, overwrite))
		{
			return DirectoryDeleteFast(source);
		}
	}
	else
	{
		// Make sure that the target's parent directory exists before we move
		if (!dstDirInfo->Parent->Exists)
		{
			try
			{
				dstDirInfo->Parent->Create();
			}
			catch(Exception *e)
			{
				mLastKnownError = e->Message;
				MOG_Report::ReportMessage(S"DirectoryMoveFast Error", String::Concat(S"Could not create parent directory for ", target, S" (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
				return false;
			}
		}

		int dialogId = 0;
		int violationStage = 0;
		bool retval = true;
		DateTime warnTime = DateTime::Now.AddSeconds(5);
		
		//Move the directory
		while (true)
		{
			try
			{
				srcDirInfo->MoveTo(target);
				break;
			}
			catch(IO::IOException* e)
			{
				if (dialogId == 0)
				{
					dialogId = MOG_Progress::ProgressSetup(S"File Sharing Violation", String::Concat(	S"Please wait while MOG attempts to access the file.\n",
																										S"FILE: ", source, S"\n\n",
																										e->Message));
				}
				
				if (MOG_Prompt::IsMode(MOG_PROMPT_SILENT))
				{
					violationStage = SilentSharingViolationUpdate(source, violationStage, warnTime, e->Message, e->StackTrace);
				}
				else
				{
					if (MOG_Progress::ProgressStatus(dialogId) == MOGPromptResult::Cancel ||
						DateTime::Now > warnTime)
					{
						mLastKnownError = e->Message;
						MOG_Report::ReportMessage(S"DirectoryMoveFast Error", String::Concat(S"MOG timed out while waiting for access to the file: ", source, S" (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
						retval = false;
						break;
					}
				}

				Thread::Sleep(100);
			}
			catch(Exception *e)
			{
				mLastKnownError = e->Message;
				MOG_Report::ReportMessage(S"DirectoryMoveFast Error", String::Concat(S"Could not get move directory ", source, S" to ", target, S" (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
				retval = false;
			}
		}

		MOG_Progress::ProgressClose(dialogId);

		//Check to make sure it actually moved
		try
		{
			dstDirInfo->Refresh();
		}
		catch(Exception *e)
		{
			mLastKnownError = e->Message;
			MOG_Report::ReportMessage(S"DirectoryMoveFast Error", String::Concat(S"Could not Refresh directory (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::CRITICAL);
			retval = false;
		}

		return retval;
	}
	
	return false;
}


bool DosUtils::DirectoryRename(String *source, String *target, bool overwrite)
{
	return DirectoryMove(source, target, overwrite);
}

bool DosUtils::DirectoryRenameFast(String *source, String *target, bool overwrite)
{
	return DirectoryMoveFast(source, target, overwrite);
}

bool DosUtils::DirectoryCreate(String *directory, bool overwrite)
{
	DirectoryInfo *dir;
	try
	{
		dir = new DirectoryInfo(directory);
	}
	catch(Exception *e)
	{
		mLastKnownError = e->Message;
		MOG_ASSERT(false, "error on new directory info");
	}

	if (dir)
	{
		if (dir->Exists && overwrite)
		{
			try
			{
				// Make sure we always clear the read Only flag!
				SetAttributesRecursive(directory, S"*.*", FileAttributes::Normal);
				dir->Delete(true);
			}
			catch(Exception *e)
			{
				mLastKnownError = e->Message;
				String *message = String::Concat(	S"Could not delete: Access is denied.",
													S"   ", directory, S"\n\n",
													S"Make sure the disk is not full or write-protected and that the file is not currently in use.");
				MOG_Report::ReportMessage(S"DirectoryCreate Error", message, e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
				dir = NULL;
				return false;
			}
		}

		try
		{
			dir->Create();

//			// Set the file to be Normal & Archived
//			File::SetAttributes(dir->FullName, FileAttributes::Normal);
//			File::SetAttributes(dir->FullName, static_cast<FileAttributes>(File::GetAttributes(dir->FullName) | FileAttributes::Archive));
		}
		catch(Exception *e)
		{
			mLastKnownError = e->Message;
			MOG_Report::ReportMessage(S"DirectoryCreate Error", String::Concat(S"Could not create ", directory, S" (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
			dir = NULL;
			return false;
		}

		dir = NULL;

		return true;
	}

	return false;
}


bool DosUtils::DirectoryDelete(String *directory)
{
	DosUtils *utils = new DosUtils;
	utils->Initialize(directory, "");

	if (utils->sContainsWildCard)
	{
		// Get list of files that match the file pattern
		DirectoryInfo *directories[] = utils->Get_sPathInfo()->GetDirectories(utils->sName);

		// check each file seperately
		for (int f = 0; f < directories->Count; f++)
		{
			// Make sure the each directory copies or else we should quit
			if (!DirectoryDelete(String::Concat(utils->sPath, S"\\", directories[f]->Name)))
			{
				return false;	//if we fail on a delete then stop.
			}
		}

		utils->Dispose();
		return true;
	}

	DirectoryInfo *dir = new DirectoryInfo(utils->sFullName);

	if (dir->Exists)
	{
		try
		{
			// Make sure we always clear the read Only flag!
			SetAttributesRecursive(directory, S"*.*", FileAttributes::Normal);
			dir->Delete(true);
		}
		catch(Exception *e)
		{
			mLastKnownError = e->Message;
			String *message = String::Concat(	S"Could not delete: Access is denied.",
												S"   ", directory, S"\n\n",
												S"Make sure the disk is not full or write-protected and that the file is not currently in use.");
			MOG_Report::ReportMessage(S"DirectoryDelete Error", message, e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
			utils->Dispose();
			return false;
		}
	}

	utils->Dispose();
	return true; 
}


bool DosUtils::DirectoryDeleteFast(String *directory)
{
	DirectoryInfo *dir = NULL;

	try
	{
		dir = new DirectoryInfo(directory);
		if (dir && dir->Exists)
		{
			// Make sure we always clear the read Only flag!
			SetAttributesRecursive(directory, S"*.*", FileAttributes::Normal);
			dir->Delete(true);
		}
	}
	catch(Exception *e)
	{
		mLastKnownError = e->Message;
		String *message = String::Concat(	S"Could not delete: Access is denied.",
											S"   ", directory, S"\n\n",
											S"Make sure the disk is not full or write-protected and that the file is not currently in use.");
		MOG_Report::ReportMessage(S"DirectoryDeleteFast Error", message, e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
		return false;
	}

	return true; 
}

//Use this when you want to get the read only files too
bool DosUtils::DirectoryDeleteFast(String* directory, bool force)
{
	if (force)
	{
		//Go through all the files and make them all normal, clearing any attributes (read-only, etc.)
		SetAttributesRecursive(directory, S"*.*", FileAttributes::Normal);
	}
	
	return DirectoryDeleteFast(directory);
}


bool DosUtils::DirectoryDeleteEmptyParents(String *path, String *fileExclusions)
{
	// Make sure this path is a valid Directory?
	if (DirectoryExist(path))
	{
		// Get list of files that match the file pattern
		DirectoryInfo *directories[] = DirectoryGetList(path, "");
		// Make sure that there were no directories in the specified path?
		if (directories->Count == 0)
		{
			// Get the list of files contained within this directory
			FileInfo *files[] = FileGetList(path, "");

			// Split out the string into sub strings
			String* delimStr = S",";
			Char delimiter[] = delimStr->ToCharArray();
			String* exclusions[] = fileExclusions->Split(delimiter, 5);

			// Scan the found files and check them against the fileExclusions
			for (int i = 0; i < files->Count; i++)
			{
				// Check all the exclusions
				for (int n = 0; n < exclusions->Count; n++)
				{
					// Check if this file matches the exclusions?
					if (!StringUtils::StringCompare(files[i]->Name, exclusions[n]))
					{
						// Abort since we found a file that was not included in the fileExclusions
						return false;
					}
				}

				// Delete the file
				FileDelete(files[i]->FullName);
			}

			// Get the DirectoryInfo
			DirectoryInfo *dir = new DirectoryInfo(path);
			// Double check to make sure we have no files or directories remaining
			if (dir->GetFiles()->Count != 0 ||
				dir->GetDirectories()->Count != 0)
			{
				return false;
			}

			// Delete this directory
			DirectoryDelete(path);

			// Check if we have a Parent Directory?
			if (dir->Parent)
			{
				// Perform recursion on the parent directory
				return DirectoryDeleteEmptyParents(dir->Parent->FullName, fileExclusions);
			}
		}
	}

	return false;
}

bool DosUtils::DirectoryDeleteEmptyParentsFast(String *path, bool recurse)
{
	try
	{
		DirectoryInfo* dir = new DirectoryInfo(path);
	
		// Make sure this path is a valid Directory?
		if (dir->Exists)
		{
			// Make sure that there were no subdirectories or files in the specified path?
			DirectoryInfo *directories[] = dir->GetDirectories();
			FileInfo *files[] = dir->GetFiles();
			if (directories->Count == 0 && files->Count == 0)
			{
				//This directory is empty, DELETE IT!!!
				dir->Delete();

				// Check if we have a Parent Directory?
				if (recurse && dir->Parent)
				{
					// Perform recursion on the parent directory
					DirectoryDeleteEmptyParentsFast(dir->Parent->FullName, recurse);
				}

				//we deleted something, so return true
				return true;
			}
		}
	}
	catch(Exception* e)
	{
		mLastKnownError = e->Message;
		MOG_Report::ReportMessage(S"DirectoryDeleteEmptyParentsFast Error", e->Message, e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
	}

	//We never deleted anything
	return false;
}


String *DosUtils::CurrentDirectory()
{
	return Environment::get_CurrentDirectory();
}




FileInfo* DosUtils::FileGetList(String *directoryFullName, String *filePattern)[]
{
	if (!filePattern || !filePattern->Length)
	{
		filePattern = "*.*";
	}

	DirectoryInfo *dir = NULL;

	try
	{
		dir = new DirectoryInfo(directoryFullName);

		if (dir->Exists)
		{
			// get a reference to each file in that directory
			return dir->GetFiles(filePattern);
		}
	}
	catch(Exception *e)
	{
		mLastKnownError = e->Message;
		MOG_Report::ReportMessage(S"FileGetList Error", String::Concat(S"Could not GetFiles from ", directoryFullName, S" (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
	}

	return NULL;
}

ArrayList *DosUtils::FileGetRecursiveList(String *directoryFullName, String *filePattern)
{
	ArrayList *fileList = new ArrayList();

	if (!filePattern->Length)
	{
		filePattern = "*.*";
	}

	DirectoryInfo *dir = new DirectoryInfo(directoryFullName);

	if (dir->Exists)
	{
		try
		{
			// get a reference to each file in that directory
			FileInfo* files[] = dir->GetFiles(filePattern);
			for( int i = 0; i < files->Count; i++ )
			{
				fileList->Add(files[i]->FullName);
			}

			DirectoryInfo *directories[] = dir->GetDirectories();
			for( int i = 0; i < directories->Count; i++ )
			{
				fileList->AddRange(FileGetRecursiveList(directories[i]->FullName, filePattern));
			}
		}
		catch(Exception *e)
		{
			mLastKnownError = e->Message;
			MOG_Report::ReportMessage(S"FileGetList Error", String::Concat(S"Could not GetFiles from ", directoryFullName, S" (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
			return fileList;
		}
	}

	return fileList;
}

ArrayList *DosUtils::FileGetRecursiveRelativeList(String *directoryFullName, String *filePattern)
{
	ArrayList *fileList = FileGetRecursiveList(directoryFullName, filePattern);

	for( int i = 0; i < fileList->Count; i++ )
	{
		String *file = __try_cast<String*>(fileList->Item[i]);
		
		file = file->Substring(directoryFullName->Length + 1);
		
		fileList->Item[i] = file;
	}

	return fileList;
}

DirectoryInfo *DosUtils::DirectoryGetList(String *directoryFullName, String *filePattern)[]
{
	if (!filePattern->Length)
	{
		filePattern = "*.*";
	}

	DirectoryInfo *dir = NULL;
	
	try
	{
		dir = new DirectoryInfo(directoryFullName);

		if (dir->Exists)
		{
			return dir->GetDirectories(filePattern);
		}
	}
	catch(Exception *e)
	{
		mLastKnownError = e->Message;
		return NULL;
	}
	catch(...)
	{
		return NULL;
	}

	return NULL;
}

bool DosUtils::SetAttributes(String *filename, FileAttributes attributes)
{
	try
	{
		// Make sure the file exists?
		FileInfo *file = new FileInfo(filename);
		if (file->Exists)
		{
			// Set the file to whatever attribute they passed in
			File::SetAttributes(file->FullName, attributes);
		}
	}
	catch(Exception *e)
	{
		mLastKnownError = e->Message;
		MOG_Report::ReportMessage(S"DosUtils::SetAttributes Error", String::Concat(S"Could not SetAttribute for ", filename, S" (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
		return false;
	}

	return true;
}

bool DosUtils::SetAttributesRecursive(String *directoryFullName, String *filePattern, FileAttributes attributes)
{
	bool bFailed = false;

	// Make sure the directory exists?
	DirectoryInfo *dir = new DirectoryInfo(directoryFullName);
	if (dir->Exists)
	{
		try
		{
			// Set each file in the directory
			FileInfo* files[] = dir->GetFiles(filePattern);
			for( int i = 0; i < files->Count; i++ )
			{
				// Set the file to be Normal & Archived
				File::SetAttributes(files[i]->FullName, attributes);
			}

			// Recurse each contained directory
			DirectoryInfo *directories[] = dir->GetDirectories();
			for( int i = 0; i < directories->Count; i++ )
			{
				// Set the directory to be Normal & Archived
				File::SetAttributes(directories[i]->FullName, attributes);

				if (!SetAttributesRecursive(directories[i]->FullName, filePattern, attributes))
				{
					bFailed = true;
				}
			}
		}
		catch(Exception *e)
		{
			mLastKnownError = e->Message;
			MOG_Report::ReportMessage(S"FileGetList Error", String::Concat(S"Could not GetFiles from ", directoryFullName, S" (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
			bFailed = true;
		}
	}

	if (!bFailed)
	{
		return true;
	}

	return false;
}


FileInfo *DosUtils::FileGetInfo(String *filename)
{
	FileInfo *file;

	try
	{
		file = new FileInfo(filename);
	}
	catch(Exception *e)
	{
		mLastKnownError = e->Message;
		MOG_Report::ReportMessage(S"DosUtils::FileGetInfo Error", String::Concat(S"Could not Get FileInfo of ", filename, S" (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
		return NULL;
	}

	return file;
}


DirectoryInfo *DosUtils::DirectoryGetInfo(String *filename)
{
	DirectoryInfo *directory = new DirectoryInfo(filename);

	return directory;
}




//	*****************************************
//	*****************************************
//		COMMAND LINE ROUTINES
//	*****************************************
//	*****************************************
String* DosUtils::GetSystemEnvironmentVariable(String *EnvVarName)
{
	return Environment::GetEnvironmentVariable(EnvVarName);
}

using System::Collections::DictionaryEntry;
using System::Collections::IDictionary;

ArrayList * DosUtils::GetSystemEnvironmentVariables()
{
	ArrayList *variables = new ArrayList();

	IDictionary* environmentVariables = Environment::GetEnvironmentVariables();
	IEnumerator* myEnum = environmentVariables->GetEnumerator();
	while (myEnum->MoveNext())
	{
		DictionaryEntry* de = __try_cast<DictionaryEntry*>(myEnum->Current);
 
		String *variable = String::Concat(de->Key, S"=", de->Value);
		variables->Add(variable);
	}

	return variables;
}



// *********************************************************
// Ie - MOG_SetEnvironmentVar("LIB", "c:\\mylib");
// Will return 0 if successful, or -1 in the case of an error 
// *********************************************************
//String *SetEnvironmentVar(String *var, String *value)
//{	
//	return Environment::ExpandEnvironmentVariables(
//}

// *********************************************************
// *********************************************************
void DosUtils::EnvironmentList_AddVariable(ArrayList *environment, String *variableStatement)
{
	environment->Add(variableStatement);
}


void DosUtils::EnvironmentList_AddVariable(ArrayList *environment, String *variable, String *value)
{
	environment->Add(String::Concat(variable, S"=", value));
}


void DosUtils::EnvironmentList_AddVariables(ArrayList *environment, String *variableStatements[])
{
	// Make sure we have a valid array?
	if (variableStatements != NULL)
	{
		// Add each index as a seperate variable
		for (int i = 0; i < variableStatements->Count; i++)
		{
			environment->Add(variableStatements[i]);
		}
	}
}


void DosUtils::EnvironmentList_AddFile(ArrayList *environment, String *infoFile)
{
	// Setup the environment from the SlaveTask file
	MOG_Ini *file = new MOG_Ini(infoFile);
	String *envVar;

	// Loop through all the sections
	for(int j = 0; j < file->CountSections(); j++)
	{
		// Loop through all the keys
		for(int i = 0; i < file->CountKeys(file->GetSectionByIndexSLOW(j)); i++)
		{
			// Only build the environment from the ENVIRONMENT section
// JohnRen - Removed for now...We might always want to take all the variables not just the environment!
//			if (!String::Compare(assetTask.GetSectionByIndex(j), "ENVIRONMENT", true))
			{
				String *section =  file->GetSectionByIndexSLOW(j);
				String *variable = file->GetKeyNameByIndexSLOW(section, i);
				String *value =    file->GetKeyByIndexSLOW(section, i);

				// Only add valid variables and values
				if (variable->Length && value->Length)
				{
					// Get the key name first, next get the value
					envVar = envVar->Concat(variable, S"=", value);
					// Allocate memory for this string
					environment->Add(envVar);
				}
			}
		}
	}
}


String* DosUtils::EnvironmentList_GetVariable(ArrayList* environment, String* variable)
{
	String* value = S"";
	
	// Make sure we have a valid array?
	if (environment)
	{
		// Add each index as a seperate variable
		for (int i = 0; i < environment->Count; i++)
		{
			String* item = __try_cast<String*>(environment->Item[i]);
			String* parts[] = item->Split(S"="->ToCharArray(), 2);
			if (parts->Count == 2)
			{
				if (String::Compare(variable, parts[0], true) == 0)
				{
					value = parts[1];
					//don't break, let's keep looking so we always find the most recently added variable
				}
			}
		}
	}

	return value;
}

void DosUtils::OnOutputDataReceived(Object* sender, DataReceivedEventArgs* e)
{
	if (!String::IsNullOrEmpty(e->Data))
	{
		mProcessOutputLog->AppendLine(e->Data);
	}
}

void DosUtils::OnErrorDataReceived(Object* sender, DataReceivedEventArgs* e)
{
	if (!String::IsNullOrEmpty(e->Data))
	{
		mProcessOutputLog->AppendLine(e->Data);
	}
}

int DosUtils::SpawnDosCommand(String *directory, String *command, String *args, ArrayList *environment, String **OutputLog, bool hideWindow)
{
	int exitCode = 0;

	// Setup a new Process
	ProcessStartInfo* si = new ProcessStartInfo();

	// Setup the environment
	for (int i = 0; i < environment->Count; i++)
	{
		// Break up the variable into the name and value
		String *variable = __try_cast<String *>(environment->Item[i]);
		if (variable->Length)
		{
			int pos = variable->IndexOf("=");
			if (pos != -1)
			{
				String *name = variable->Substring(0, pos);
				String *value = variable->Substring(pos + 1);
				// Add the variable to the environment
				try
				{
					if (si->EnvironmentVariables->ContainsKey(name))
					{
						si->EnvironmentVariables->Remove(name);
						si->EnvironmentVariables->Add(name, value);
					}
					else
					{
						si->EnvironmentVariables->Add(name, value);
					}
				}
				catch(Exception *e)
				{
					mLastKnownError = e->Message;
					MOG_Report::ReportMessage(S"SpawnDosCommand Error", String::Concat(S" (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
				}
			}
		}
	}

	// Specify the directory and filename
	si->FileName = command;
	si->Arguments = args;
	si->WorkingDirectory = directory;
	si->UseShellExecute = false;	//This has to be false in order to use environment variables and redirects

	if (hideWindow)
	{
		si->WindowStyle = ProcessWindowStyle::Hidden;
		si->CreateNoWindow = true;
		si->RedirectStandardOutput = true;
		si->RedirectStandardError = true;
		si->RedirectStandardInput = true;
	}
	else
	{
		si->WindowStyle = ProcessWindowStyle::Normal;
	}

	Process* p = NULL;
	try
	{
		mProcessOutputLog = new StringBuilder("");
		
		p = new Process();
		p->StartInfo = si;

		p->EnableRaisingEvents = true;
		p->OutputDataReceived += new DataReceivedEventHandler(OnOutputDataReceived);
		p->ErrorDataReceived += new DataReceivedEventHandler(OnErrorDataReceived);

		p->Start();

		if (si->RedirectStandardOutput)
		{
			p->BeginOutputReadLine();
		}
		if (si->RedirectStandardError)
		{
			p->BeginErrorReadLine();
		}
	}
	catch(Exception *e)
	{
		mLastKnownError = e->Message;
		MOG_Report::ReportMessage(S"SpawnDosCommand Error", String::Concat(S"Could not start (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
		return -1;
	}

	if (p)
	{
		try
		{
			// Make sure we keep a global handle to this process so we can shutdown if needed
			gProcess = p;
			p->WaitForExit();
		}
		catch(Exception *e)
		{
			mLastKnownError = e->Message;
			MOG_Report::ReportMessage(S"SpawnDosCommand Error", String::Concat(S"Could not waitForExit (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
			return -1;
		}
		__finally
		{
			gProcess = NULL;
		}

		exitCode = p->ExitCode;
		p->Close();
		p = NULL;
	}

	*OutputLog = String::Copy(mProcessOutputLog->ToString());

	return exitCode;
}

bool DosUtils::SpawnCommand(String *command, String *args)
{
	// Run the command
	Process *p = new Process();
	p->StartInfo->FileName = command;
	p->StartInfo->Arguments = args;
	p->StartInfo->WorkingDirectory = PathGetDirectoryPath(command);

	p->StartInfo->UseShellExecute = false;
	try
	{
		p->Start();
	}
	catch(Exception *e)
	{
		mLastKnownError = e->Message;
		MOG_Report::ReportMessage(S"SpawnCommand Error", String::Concat(S"Could not start (", e->Message, S")\n"), e->StackTrace, PROMPT::MOG_ALERT_LEVEL::ERROR);
		return false;
	}

	p->Close();
	p = NULL;

	return true;
}

int DosUtils::SilentSharingViolationUpdate(String* filename, int violationStage, DateTime startTime, String* message, String* stackTrace)
{
	// Get the elapsed time
	TimeSpan diff = DateTime::Now - startTime;

	switch (violationStage)
	{
	case 0:
		if (diff.Seconds > 15)
		{
			mLastKnownError = message;
			MOG_Report::ReportMessage(	S"One of your tasks has been delayed",
										String::Concat(	S"A slave is being delayed due to a file sharing violation.\n",
														S"   FILE: '", filename, S"\n",
														S"\n",
														S"Please Make sure you don't have this folder/file open on your machine.\n"),
										S"",
										PROMPT::MOG_ALERT_LEVEL::ALERT);
			violationStage++;
		}
		break;
	case 1:
		if (diff.Minutes > 1)
		{
			mLastKnownError = message;
			MOG_Report::ReportMessage(	S"One of your tasks is still being delayed",
										String::Concat(	S"A slave is experiencing an extended delay due to a file sharing violation.\n",
														S"   FILE: '", filename, S"\n",
														S"\n",
														S"Please Make sure you don't have this folder/file open on your machine.\n"),
										S"", 
										PROMPT::MOG_ALERT_LEVEL::ALERT);
			violationStage++;
		}
		break;
	case 2:
	default:
		int multiplier = violationStage - 1;
		if (diff.Minutes > (5 * multiplier))
		{
			mLastKnownError = message;
			MOG_Report::ReportMessage(	S"One of your tasks appears to be hung",
										String::Concat(	S"A slave is hung due to a file sharing violation.\n",
														S"   FILE: '", filename, S"\n",
														S"\n",
														S"Please Contact your MOG Administrator for assistance.\n"),
										S"",
										PROMPT::MOG_ALERT_LEVEL::ERROR);
			violationStage++;
		}
		break;
	}

	return violationStage;
}
