// XboxUtils.cpp : Defines the entry point for the dll application.
//

#include "stdafx.h"
#include "xboxdbg.h"

#include <string>
using namespace std;
/*
BOOL APIENTRY DllMain( HANDLE hModule, 
                       DWORD  ul_reason_for_call, 
                       LPVOID lpReserved
					 )
{
    return TRUE;
}
*/

BOOL __stdcall DllMain(HINSTANCE hInst, DWORD dwReason, LPVOID lpReserved) {
	return TRUE;
}


extern "C" __declspec(dllexport) bool __stdcall XboxFileCopy(char* pSource, char *pTarget, bool silent) {
	bool rc = false;
	rc = SUCCEEDED( DmSendFile( pSource, pTarget ) );
	string message;
	if (!rc && !silent)
	{
		message = "File (";
		message += pSource;
		message += ") could not be copied to (";
		message += pTarget;
		message += ")!";
		MessageBox(NULL, message.c_str(), "Xbox Utilities: FileCopy", MB_OK);
	}
	return rc;
}

extern "C" __declspec(dllexport) bool __stdcall XboxFileGet(char* pSource, char *pTarget, bool silent) {
	bool rc = false;
	rc = SUCCEEDED( DmReceiveFile( pTarget, pSource ) );
	string message;
	if (!rc && !silent)
	{
		message = "File (";
		message += pSource;
		message += ") could not be copied to (";
		message += pTarget;
		message += ")!";
		MessageBox(NULL, message.c_str(), "Xbox Utilities: FileGet", MB_OK);
	}
	return rc;
}
extern "C" __declspec(dllexport) bool __stdcall XboxFileRename(char* pSource, char *pTarget) {
	return SUCCEEDED( DmRenameFile( pSource, pTarget ) );
}

extern "C" __declspec(dllexport) bool __stdcall XboxFileDelete(char *pTarget) {
	bool rc = SUCCEEDED( DmDeleteFile( pTarget, false ) );
	string message;
	if (!rc)
	{
		message = "File (";
		message += pTarget;
		message += ") could not be deleted!";
		MessageBox(NULL, message.c_str(), "Xbox Utilities: FileDelete", MB_OK);
	}
	return rc;
}

// file attributes
typedef struct _MOG_DM_FILE_ATTRIBUTES {
    char* Name;
    FILETIME CreationTime;
    FILETIME ChangeTime;
    DWORD SizeHigh;
    DWORD SizeLow;
    DWORD Attributes;
	int Exist;
} MOG_DM_FILE_ATTRIBUTES, *MOG_PDM_FILE_ATTRIBUTES;


extern "C" __declspec(dllexport) bool __stdcall XboxGetFileAttributes(char *pFilename, MOG_PDM_FILE_ATTRIBUTES pAttributes) {
	DM_FILE_ATTRIBUTES temp;

	temp.Attributes = 0;
	temp.SizeHigh = 0;
	temp.SizeLow = 0;

	//strcpy(pAttributes->Name, temp.Name);
	bool rc = SUCCEEDED( DmGetFileAttributes( pFilename, &temp ) );
	if (rc)
	{
		//pAttributes->Name = (char*)malloc(sizeof(temp.Name)+1);
		//strcpy(temp.Name, pAttributes->Name);
		pAttributes->Attributes = temp.Attributes;
		pAttributes->ChangeTime = temp.ChangeTime;
		pAttributes->CreationTime = temp.CreationTime;
		pAttributes->SizeHigh = temp.SizeHigh;
		pAttributes->SizeLow = temp.SizeLow;
		if ( temp.Attributes == 0 && temp.SizeHigh == 0 && temp.SizeLow == 0)
			pAttributes->Exist = 0;
		else pAttributes->Exist = 1;
		return true;
	}

	pAttributes->Exist = 0;
	return false;
}

extern "C" __declspec(dllexport) unsigned int __stdcall XboxGetFileSize(char *pFilename) {
	DM_FILE_ATTRIBUTES temp;
	unsigned int size = 0;

	string message = "";
	bool rc = SUCCEEDED( DmGetFileAttributes( pFilename, &temp ) );
	if (rc)
	{		
		size = (int)temp.SizeLow + (int)temp.SizeHigh;
	}
	else
	{
//		message = "Could not get size of:";
//		message += pFilename;
//		MessageBox(NULL, message.c_str(), "XboxGetFileSize", MB_OK);
		return 0;
	}
	
	return size;
}

extern "C" __declspec(dllexport) bool __stdcall XboxFileExist(char *pFilename) {
	DM_FILE_ATTRIBUTES temp;

	temp.Attributes = 0;
	temp.SizeHigh = 0;
	temp.SizeLow = 0;
	temp.ChangeTime.dwHighDateTime = 0;
	temp.ChangeTime.dwLowDateTime = 0;
	temp.CreationTime.dwHighDateTime = 0;
	temp.CreationTime.dwLowDateTime = 0;

	bool rc = SUCCEEDED( DmGetFileAttributes( pFilename, &temp ) );
	if (rc)
	{		
		if (temp.ChangeTime.dwHighDateTime != 0 || 
			temp.ChangeTime.dwLowDateTime != 0 || 
			temp.CreationTime.dwHighDateTime != 0 ||
			temp.CreationTime.dwLowDateTime != 0 ||
			temp.SizeHigh > 0 ||
			temp.SizeLow > 0
			)
		{
//			char tt[100];
//			string message = pFilename;
//			message += " exists!\n";
//			message += itoa(temp.ChangeTime.dwHighDateTime, tt, 10);
//			message += "\n";
//			message += itoa(temp.ChangeTime.dwLowDateTime, tt, 10);
//			message += "\n";
//			message += itoa(temp.CreationTime.dwHighDateTime, tt, 10);
//			message += "\n";
//			message += itoa(temp.CreationTime.dwLowDateTime, tt, 10);
//			message += "\n";
//			message += itoa(temp.SizeHigh, tt, 10);
//			message += "\n";
//			message += itoa(temp.SizeLow, tt, 10);
//			MessageBox(NULL, message.c_str(), "XboxFileExist", MB_OK);
			return true;
		}
	}

//	string message2 = pFilename;
//	message2 += " does not exists!\n";
//	MessageBox(NULL, message2.c_str(), "XboxFileExist", MB_OK);
			
	return false;
}

extern "C" __declspec(dllexport) bool __stdcall XboxDirectoryCreate(char *pTarget, bool silent) {
	bool rc = SUCCEEDED( DmMkdir( pTarget ) );
	string message;
	if (!rc && !silent)
	{
		DWORD dw = GetLastError();
		char temp[100];
		message = "Directory (";
		message += pTarget;
		message += ") could not be created with ErrorCode(";
		message += itoa(dw, temp, 10);
		message += ")";
		MessageBox(NULL, message.c_str(), "Xbox Utilities: DirectoryCreate", MB_OK);
	}
	return rc;
}

extern "C" __declspec(dllexport) bool __stdcall XboxDirectoryDelete(char *pTarget, bool silent) {
	string message;
	bool rc = SUCCEEDED(DmDeleteFile( pTarget, true ));
	if (!rc && !silent)
	{
		DWORD dw = GetLastError();
		char temp[100];
		message = "Directory (";
		message += pTarget;
		message += ") could not be deleted with ErrorCode(";
		message += itoa(dw, temp, 10);
		message += ")";
		MessageBox(NULL, message.c_str(), "Xbox Utilities: DirectoryDelete", MB_OK);
	}
	return rc;
}

//extern "C" __declspec(dllexport) bool __stdcall XboxGetXboxName(char *pName, char *pResponse) {
//	return SUCCEEDED( DmGetXboxName( pName, pResponse ) );
//}

extern "C" __declspec(dllexport) __int64 __stdcall XboxGetDiskFreeSpace(char *pName) {
	string message;
	__int64 lpFreeBytesAvailable = 0;    // bytes available
	__int64 lpTotalNumberOfBytes = 0;    // bytes on disk
	__int64 lpTotalNumberOfFreeBytes = 0;// free bytes on disk

	bool rc = SUCCEEDED(DmGetDiskFreeSpace( pName, (PULARGE_INTEGER)&lpFreeBytesAvailable, (PULARGE_INTEGER)&lpTotalNumberOfBytes, (PULARGE_INTEGER)&lpTotalNumberOfFreeBytes ));
	if (!rc )
	{
		DWORD dw = GetLastError();
		message = "Xbox (";
		message += pName;
		message += ") could not determine size!";
		MessageBox(NULL, message.c_str(), "Xbox Utilities: XboxGetDiskFreeSpace", MB_OK);
	}

	char temp[50];
	message = "BA:";
	message += ltoa((long)lpFreeBytesAvailable, temp, 10);
	message += " ";
	message += ltoa((long)lpTotalNumberOfBytes, temp, 10);
	message += " ";
	message += ltoa((long)lpTotalNumberOfFreeBytes, temp, 10);

	MessageBox(NULL, message.c_str(), "Xbox Utilities: XboxGetDiskFreeSpace", MB_OK);

	return lpTotalNumberOfFreeBytes;
}



extern "C" __declspec(dllexport) bool __stdcall XboxSetXboxName(char *pName, bool silent) {
	string message;
	bool rc = SUCCEEDED(DmSetXboxName( pName ));
	if (!rc && !silent)
	{
		DWORD dw = GetLastError();
		//char temp[100];
		message = "Xbox (";
		message += pName;
		message += ") could not be set as default!";
		MessageBox(NULL, message.c_str(), "Xbox Utilities: SetXboxName", MB_OK);
	}
	return rc;
}

extern "C" __declspec(dllexport) char* __stdcall XboxGetFiles(char *pName) {
	
	//char strFind[] = pName;
	string files ="";
	bool first = true;

	//DMHRAPI error;
	//DMAPI HRESULT __stdcall error;
	long error;
	PDM_WALK_DIR pWalkDir = NULL;
	DM_FILE_ATTRIBUTES fileAttr;

	do
	{
		error = DmWalkDir(&pWalkDir, pName, &fileAttr);

		if (error == XBDM_NOERR)
		{
			if (first)
			{
				files = pName;
				//files += "\\";
				files += fileAttr.Name;
				first = false;
			}
			else
			{
				files += ",";
				files += pName;
				//files += "\\";
				files += fileAttr.Name;
			}
		}

		// Examine the contents of fileAttr here.
	}
	while (error == XBDM_NOERR);

	if (error != XBDM_ENDOFLIST)
	{
		// Handle error.
	}

	DmCloseDir(pWalkDir);

	unsigned int i = 0;
	char *temp = (char*) malloc(files.size() + 1);
	for (i = 0; i < files.size(); i++)
	{
		temp[i] = files.at(i);
	}
	temp[i] = '\0';
	return temp;	
}

