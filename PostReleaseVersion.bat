:: ---------------------------------------
:: This file takes care of copying all the
:: needed files to out l:_MOGReleases Directory
:: To complete the build, go to the ServerInstaller svn project and
:: run;
:: 	!.bat  - Updates the ServerInstaller directories with the latest bins
::	BuildAll.bat - Creates the zips and installers
:: ---------------------------------------

@echo off
set EchoType=off
set BuildType=Release
set RevisionNumber=0
set TargetRepository=L:\_MOGReleases\Pending\Deployment Drop
set TargetInstallerBinaries=L:\_MOGReleases\InstallerSrc\CurrentBinaries
set logNumber=1
set PostCode=false

:RESET
set SourceBinaries=%TargetRepository%  
set MOG_TOOLS=%TargetRepository%\Updates\Uploads 
set EvalBuild=%TargetRepository%\Updates\InstallerBuilds\Eval
set FirstBuild=%TargetRepository%\Updates\InstallerBuilds\1.0

:Menu
Call :GetGlobalSettings
set Selection=
set CompileBuild=false
echo ----------------------------------------
echo MOG Version Updater: %BuildType%
echo Target Repository: %TargetRepository%
echo ----------------------------------------
echo 1 - MOG 1.0 Release Deployment
echo 2 - MOG 1.0 Release Deployment Compile
echo 3 - MOG Library 1.0 Release Deployment Compile
echo 4 - MOG 1.0 Release Deployment Compile (With code)
echo. 
echo 0 - Exit
set /P Selection="Enter Selection:"

if /I %Selection%x==1x Goto PostRelease
if /I %Selection%x==2x Goto PostReleaseCompile
if /I %Selection%x==3x Goto PostLibraryReleaseCompile
if /I %Selection%x==4x (
  call :SetName PostCode true
  Goto PostReleaseCompile
)
if /I %Selection%x==Ix Goto MENU

if /I %Selection%x==0x Goto Done

echo.
echo ERROR - INVALID ENTRY
echo.
Goto Menu

:PostLibraryReleaseCompile
Call :SetName CompileBuild true
Call :SetName BuildName "Release Library"
Call :SetName BuildType Release Library
Call :SetName TargetInstallerBinaries L:\_MOGReleases\InstallerSrc\CurrentLibraryBinaries
Goto PostRelease

:PostReleaseCompile
Call :SetName CompileBuild true
Call :SetName BuildName "Release"
Call :SetName BuildType Release

:PostRelease

:LogMenu
set Selection=
echo ----------------------------------------
echo MOG Version LOGS
echo ----------------------------------------
echo 1 - Use 1 log
echo 2 - Use 2 logs (Client, Server)
echo.
echo 0 - Exit
set /P Selection="Enter Selection:"

if /I %Selection%x==1x Goto OneLog
if /I %Selection%x==2x Goto TwoLog

if /I %Selection%x==0x Goto Done

echo.
echo ERROR - INVALID ENTRY
echo.
Goto LogMenu

:OneLog
call :SetName logNumber 1
goto :Post

:TwoLog
call :SetName logNumber 2
goto :Post

:Post
set SourceBinaries=%EvalBuild%
set FallThrough=true
echo --------------------------------------------------
echo Posting all to %SourceBinaries% for Release build
echo --------------------------------------------------
call :GetLastCodeRevisionNumber
Call :GetNewCodeRevisionNumber

echo.
echo    --------------------------------------------------
echo    Adding notes...
echo    --------------------------------------------------

:: Save out the ChangeList file
Call :WriteNotes "%TargetInstallerBinaries%\ServerWhatsNew.txt"
if %LogNumber%x==2x Call :WriteNotes "%TargetInstallerBinaries%\ClientWhatsNew.txt"

:: Setup the MSDev environment
Call "c:\Program Files\Microsoft Visual Studio 8\VC\vcvarsall.bat" x86

::@echo on
if %CompileBuild%x==falsex goto PostBins

echo    --------------------------------------------------
echo    Setting version number...
echo    --------------------------------------------------

Call :SetAssemblyVersion "%CD%\MOG\AssemblyInfo.cpp"
Call :SetAssemblyVersion "%CD%\MOG_Client\AssemblyInfo.cs"
Call :SetAssemblyVersion "%CD%\MOG_ControlsLibrary\AssemblyInfo.cs"
Call :SetAssemblyVersion "%CD%\MOG_CoreControls\Properties\AssemblyInfo.cs"
Call :SetAssemblyVersion "%CD%\MOG_ServerManager\AssemblyInfo.cs"
Call :SetAssemblyVersion "%CD%\MOG_Server\AssemblyInfo.cs"
Call :SetAssemblyVersion "%CD%\MOG_EventViewer\AssemblyInfo.cs"
Call :SetAssemblyVersion "%CD%\MOG_CommandLine\AssemblyInfo.cs"

echo.
echo    --------------------------------------------------
echo    Performing a full %BuildName% compile...
echo    --------------------------------------------------


:: Build the Client
devenv MOG.sln /Rebuild %BuildName%
if ERRORLEVEL 1 goto Error

Call :RestoreAssemblyVersion "%CD%\MOG\AssemblyInfo.cpp"
Call :RestoreAssemblyVersion "%CD%\MOG_Client\AssemblyInfo.cs"
Call :RestoreAssemblyVersion "%CD%\MOG_ControlsLibrary\AssemblyInfo.cs"
Call :RestoreAssemblyVersion "%CD%\MOG_CoreControls\Properties\AssemblyInfo.cs"
Call :RestoreAssemblyVersion "%CD%\MOG_ServerManager\AssemblyInfo.cs"
Call :RestoreAssemblyVersion "%CD%\MOG_Server\AssemblyInfo.cs"
Call :RestoreAssemblyVersion "%CD%\MOG_EventViewer\AssemblyInfo.cs"
Call :RestoreAssemblyVersion "%CD%\MOG_CommandLine\AssemblyInfo.cs"

:PostBins
echo    --------------------------------------------------
echo    Updating binaries for 64bit compatibility...
echo    --------------------------------------------------

Call :Set32BitCore "MOG_Server\bin\%BuildType%\MOG_Server.exe"
Call :Set32BitCore "MOG_Client\bin\%BuildType%\MOG_Client.exe"
Call :Set32BitCore "MOG_Client_Loader\bin\%BuildType%\MOG_Client_Loader.exe"
Call :Set32BitCore "MOG_Slave\bin\%BuildType%\MOG_Slave.exe"
Call :Set32BitCore "MOG_Server_Loader\bin\%BuildType%\MOG_Server_Loader.exe"
Call :Set32BitCore "MOG_ServerManager\bin\%BuildType%\MOG_ServerManager.exe"
Call :Set32BitCore "MOG_CommandLine\bin\%BuildType%\MOG_CommandLine.exe"
Call :Set32BitCore "MOG_EventViewer\bin\%BuildType%\MOG_EventViewer.exe"

Call :GetVersionInfo

:: --------------------------------------------
:Postserver
:: --------------------------------------------
echo ---------------------------
echo Create Server Version 

if exist "%TargetInstallerBinaries%\Server" rd "%TargetInstallerBinaries%\Server" /S /Q
md "%TargetInstallerBinaries%\Server"

echo Posting new server version
:: Post new version
xcopy "MOG_Server\bin\%BuildType%\MOG_Server.exe" "%TargetInstallerBinaries%\Server" /Y >NUL
xcopy "MOG_Server\MOG_Server.exe.manifest" "%TargetInstallerBinaries%\Server" /Y  >NUL
xcopy "MOG_Server\bin\%BuildType%\*.dll" "%TargetInstallerBinaries%\Server" /Y >NUL
if exist "MOG_Server\bin\%BuildType%\*.chm" xcopy "MOG_Server\bin\%BuildType%\*.chm" "%TargetInstallerBinaries%\Server" /Y >NUL

::
:: Write version info file
::
echo [info] > "%TargetInstallerBinaries%\Server\VERSION.INI"
echo Name=%ServerName% >> "%TargetInstallerBinaries%\Server\VERSION.INI"
echo MajorVersion=%SMajorVersion% >> "%TargetInstallerBinaries%\Server\VERSION.INI"
echo MinorVersion=%MinorVersion% >> "%TargetInstallerBinaries%\Server\VERSION.INI"
echo Type=Server >> "%TargetInstallerBinaries%\Server\VERSION.INI"

echo ---------------------------
echo Done!

xcopy _ServerVersion.txt "%TargetInstallerBinaries%"\ServerVersion.txt /Y >NUL

move "%TargetInstallerBinaries%\ServerWhatsNew.txt" "%TargetInstallerBinaries%\Server\WhatsNew.txt"
Call :WriteFileList Server "%TargetInstallerBinaries%\Server" MOG_Server.exe


:: --------------------------------------------
:PostserverLoader
:: --------------------------------------------
echo ---------------------------
echo Create Server loader Version

if exist "%TargetInstallerBinaries%\ServerLoader" rd "%TargetInstallerBinaries%\ServerLoader" /S /Q
md "%TargetInstallerBinaries%\ServerLoader"

echo Posting new server version
:: Post new version
xcopy "MOG_Server_Loader\bin\%BuildType%\MOG_Server_Loader.exe" "%TargetInstallerBinaries%\ServerLoader"  /Y >NUL
xcopy MOG_Server_Loader\MOG_Server_Loader.exe.manifest "%TargetInstallerBinaries%\ServerLoader" /Y  >NUL
xcopy MOG_Server_Loader\Loader.ini "%TargetInstallerBinaries%\ServerLoader"  /Y >NUL
xcopy "MOG_Server_Loader\bin\%BuildType%\*.dll" "%TargetInstallerBinaries%\ServerLoader" /Y >NUL


echo ---------------------------
echo Done!

:: --------------------------------------------
:PostClientLoader
:: --------------------------------------------
echo ---------------------------
echo Create Client loader Version

if exist "%TargetInstallerBinaries%\ClientLoader" rd "%TargetInstallerBinaries%\ClientLoader" /S /Q
md "%TargetInstallerBinaries%\ClientLoader"

echo Posting new server version
:: Post new version
xcopy "MOG_Client_Loader\bin\%BuildType%\MOG_Client_Loader.exe" "%TargetInstallerBinaries%\ClientLoader"  /Y >NUL
xcopy MOG_Client_Loader\MOG_Client_Loader.exe.manifest "%TargetInstallerBinaries%\ClientLoader" /Y  >NUL
xcopy MOG_Client_Loader\Loader.ini "%TargetInstallerBinaries%\ClientLoader"  /Y >NUL
xcopy "MOG_Client_Loader\bin\%BuildType%\*.dll" "%TargetInstallerBinaries%\ClientLoader" /Y >NUL


echo ---------------------------
echo Done!


:: --------------------------------------------
:Postclient
:: --------------------------------------------
echo ---------------------------
echo Create client Version

if exist "%TargetInstallerBinaries%\Client" rd "%TargetInstallerBinaries%\Client" /S /Q
md "%TargetInstallerBinaries%\Client"

echo Posting new server version
:: Post new version
xcopy "MOG_Client\bin\%BuildType%\MOG_Client.exe" "%TargetInstallerBinaries%\Client"  /Y >NUL
xcopy MOG_Client\MOG_Client.exe.manifest "%TargetInstallerBinaries%\Client" /Y  >NUL
xcopy "MOG_Client\bin\%BuildType%\*.dll" "%TargetInstallerBinaries%\Client" /Y >NUL
xcopy MOG_Client\MogClient_Skin.info "%TargetInstallerBinaries%\Client" /Y  >NUL
xcopy MOG_Client\MOG_Sounds.ini "%TargetInstallerBinaries%\Client" /Y >NUL

if exist "MOG_Client\bin\%BuildType%\*.chm" xcopy "MOG_Client\bin\%BuildType%\*.chm" "%TargetInstallerBinaries%\Client" /Y >NUL

:: Post Server Manager
xcopy "MOG_ServerManager\bin\%BuildType%\MOG_ServerManager.exe" "%TargetInstallerBinaries%\Client"  /Y >NUL
xcopy MOG_ServerManager\MOG_ServerManager.exe.manifest "%TargetInstallerBinaries%\Client" /Y  >NUL
xcopy "MOG_ServerManager\bin\%BuildType%\*.dll" "%TargetInstallerBinaries%\Client" /Y >NUL
xcopy MOG_ServerManager\*.rtf "%TargetInstallerBinaries%\Client"  /Y >NUL
if exist "MOG_ServerManager\bin\%BuildType%\*.chm" xcopy "MOG_ServerManager\bin\%BuildType%\*.chm" "%TargetInstallerBinaries%\Client" /Y >NUL

:: Post Event viewer
xcopy "MOG_EventViewer\bin\%BuildType%\MOG_EventViewer.exe" "%TargetInstallerBinaries%\Client"  /Y >NUL
xcopy MOG_EventViewer\MOG_EventViewer.exe.manifest "%TargetInstallerBinaries%\Client" /Y  >NUL
xcopy "MOG_EventViewer\bin\%BuildType%\*.dll" "%TargetInstallerBinaries%\Client" /Y >NUL
if exist "MOG_EventViewer\bin\%BuildType%\*.chm" xcopy "MOG_EventViewer\bin\%BuildType%\*.chm" "%TargetInstallerBinaries%\Client" /Y >NUL

echo ---------------------------
echo Posting new slaves
echo ---------------------------
:: Post new slave
xcopy "MOG_Slave\bin\%BuildType%\MOG_Slave.exe" "%TargetInstallerBinaries%\Client" /Y >NUL
xcopy "MOG_slave\bin\%BuildType%\*.dll" "%TargetInstallerBinaries%\Client"  /Y >NUL
xcopy MOG_Slave\MOG_Slave.exe.manifest "%TargetInstallerBinaries%\Client"  /Y >NUL

if "%BuildType%"=="Debug" xcopy "MOG_Slave\bin\%BuildType%\*.pdb" "%TargetInstallerBinaries%\Client"  /Y >NUL


echo ---------------------------
echo Posting new Command Line MOG
echo ---------------------------
:: Post new CommandLine
xcopy "MOG_CommandLine\bin\%BuildType%\MOG_CommandLine.exe" "%TargetInstallerBinaries%\Client"  /Y >NUL
xcopy "MOG_CommandLine\bin\%BuildType%\*.dll" "%TargetInstallerBinaries%\Client"  /Y >NUL

echo ---------------------------
echo Posting new MOG Bridge
echo ---------------------------
:: Post new Bridge
if exist "%TargetInstallerBinaries%\Bridge" rd "%TargetInstallerBinaries%\Bridge" /S /Q
md "%TargetInstallerBinaries%\Bridge"

xcopy "Win32\%BuildType%\*.dll" "%TargetInstallerBinaries%\Bridge"  /Y >NUL
md "%TargetInstallerBinaries%\Bridge\Integration Samples"
xcopy MOG_Bridge\"Integration Samples" "%TargetInstallerBinaries%\Bridge\Integration Samples" /E
xcopy MOG_Bridge\MOG_BridgeAPI.h "%TargetInstallerBinaries%\Bridge"  /Y >NUL

::
:: Write version info file
::
echo [info] > "%TargetInstallerBinaries%\Client\VERSION.INI"
echo Name=%ClientName% >> "%TargetInstallerBinaries%\Client\VERSION.INI"
echo MajorVersion=%CMajorVersion% >> "%TargetInstallerBinaries%\Client\VERSION.INI"
echo MinorVersion=%MinorVersion% >> "%TargetInstallerBinaries%\Client\VERSION.INI"
echo Type=Client >> "%TargetInstallerBinaries%\Client\VERSION.INI"

echo. >> "%TargetInstallerBinaries%\Client\VERSION.INI"

echo [SERVERCOMPATABILITY] >> "%TargetInstallerBinaries%\Client\VERSION.INI"
echo ServerMajorVersion=%SMajorVersion% >> "%TargetInstallerBinaries%\Client\VERSION.INI"
echo ServerMinorversion=%MinorVersion% >> "%TargetInstallerBinaries%\Client\VERSION.INI"

echo ---------------------------
echo Done!

xcopy _ClientVersion.txt "%TargetInstallerBinaries%"\ClientVersion.txt /Y >NUL

if %LogNumber%x==1x copy "%TargetInstallerBinaries%\Server\WhatsNew.txt" "%TargetInstallerBinaries%\Client\WhatsNew.txt"
if %LogNumber%x==2x move "%TargetInstallerBinaries%\ClientWhatsNew.txt" "%TargetInstallerBinaries%\Client\WhatsNew.txt"
Call :WriteFileList Client "%TargetInstallerBinaries%\Client" MOG_Client.exe

if %PostCode%==true (
  echo ---------------------------
  echo Posting MOG Source
  echo ---------------------------
  call PostSource.bat
)


::echo on
:: Run installer batch??!!!!  
if %BuildName%x=="Release"x ( 
  if exist C:\Projects\MOGServerInstaller\RunFullRealeaseBuild.bat (
    pushd C:\Projects\MOGServerInstaller
    Call C:\Projects\MOGServerInstaller\RunFullRealeaseBuild.bat
    popd  
    Goto MENU
  )
) 

if %BuildName%x=="Release Library"x ( 
  if exist C:\Projects\MOGLibraryInstaller\RunFullRealeaseBuild.bat (
    pushd C:\Projects\MOGLibraryInstaller
    Call C:\Projects\MOGLibraryInstaller\RunFullRealeaseBuild.bat
    popd  
  )
)

Goto MENU

:: --------------------------------------------
:ToggleBuild
:: --------------------------------------------

if %BuildType%==Debug (
	set BuildType=Release
) else (
	set BuildType=Debug
)

cls
:: Pause
goto MENU

:: --------------------------------------------
:ReleaseBuild
:: --------------------------------------------

set BuildType=Release

cls
:: Pause
goto MENU

:: --------------------------------------------
:DebugBuild
:: --------------------------------------------

set BuildType=Debug

cls
:: Pause
goto MENU

:: --------------------------------------------
:ToggleEcho
:: --------------------------------------------

if %EchoType%==off (
	set EchoType=on
) else (
	set EchoType=off
)

cls
:: Pause
goto MENU


:: --------------------------------------------
:SetTargetRepository
:: --------------------------------------------
set /P TargetRepository="Enter a new target repository: "
cls
:: Pause
goto RESET

:: --------------------------------------------
:GetNewCodeRevisionNumber
:: --------------------------------------------
if not exist %TargetInstallerBinaries%\Logs md %TargetInstallerBinaries%\Logs
if not exist %TargetInstallerBinaries%\Logs\CurrentRevision md %TargetInstallerBinaries%\Logs\CurrentRevision

set /P RevisionNumberHolder="--- Enter latest SVN revision that will be in this build(%RevisionNumber%):"
if NOT %RevisionNumberHolder%x==x Call :SetName RevisionNumber %RevisionNumberHolder%

echo @%RevisionNumber%>%TargetInstallerBinaries%\Logs\CurrentRevision\Revision.txt

goto :EOF

:: --------------------------------------------
:GetLastCodeRevisionNumber
:: --------------------------------------------
if exist "%TargetInstallerBinaries%\Logs\CurrentRevision\Revision.txt" (
	FOR /F "delims=@" %%i in (%TargetInstallerBinaries%\Logs\CurrentRevision\Revision.txt) do (
		call :SetRevisionNumber %%i
	)
) 

::echo --- Last logs latest revision number (%RevisionNumber%)

goto :EOF

FOR /F "delims=@" %i in ("L:\_MOGReleases\InstallerSrc\CurrentBinaries\Logs\CurrentRevision\Revision.txt") do (echo %i )


:: --------------------------------------------
:SetRevisionNumber
:: --------------------------------------------
set RevisionNumber=%1

goto :EOF

:: --------------------------------------------
:ShowVersionInfo
:: --------------------------------------------
Call :GetVersionInfo
echo ----------------------------------------
echo 	ClientName: %ClientName%
echo 	MajorVerion %CMajorVersion%
echo 	MinorVerion %MinorVersion%
echo.
echo 	ServerName: %ServerName%
echo 	MajorVerion %SMajorVersion%
echo 	MinorVerion %MinorVersion%
echo ----------------------------------------

goto :EOF

:: --------------------------------------------
:GetVersionInfo
:: --------------------------------------------
echo Updating Version infos....

:: Get the version timestamps
GetVersion "%CD%\MOG_Client\bin\%BuildType%\MOG_Client.exe" > _ClientVersion.txt
GetVersion "%CD%\MOG_Server\bin\%BuildType%\MOG_Server.exe" > _ServerVersion.txt

:: Make a new file that strips out all the '.'
find_and_replace _ClientVersion.txt _MinorVersion.txt "." "" 0 suppress_banner

FOR /F "eol=; tokens=1,2 delims=:" %%i in (_ClientVersion.txt) do (
	Call :SetName ClientName "Client %%i"
	Call :SetName ArchiveVersion %%i
)

FOR /F "eol=; tokens=1,2 delims=:" %%i in (_ServerVersion.txt) do (
	Call :SetName ServerName "Server %%i"
)

FOR /F "eol=  tokens=1" %%i in (_MinorVersion.txt) do (
	Call :SetName MinorVersion %%i
	Call :SetName SMinorVersion %%i
	Call :SetName CMinorVersion %%i
)

goto :EOF

:: --------------------------------------------
:GetGlobalSettings
:: --------------------------------------------
FOR /F "eol=; tokens=1,2 delims=:" %%i in (postDownloadableVersion.info) do (
	if /I %%i EQU CMajorVersion Call :SetName CMajorVersion %%j	
	if /I %%i EQU BuildPrefix Call :SetName BuildPrefix %%j
	if /I %%i EQU SMajorVersion Call :SetName SMajorVersion %%j
)

goto :EOF

:: --------------------------------------------
:SetName
:: --------------------------------------------
if %3x==x (
  set %1=%2
) else (
  set %1=%2 %3
)

::echo %1=%2
goto :EOF

:: --------------------------------------------
:WriteFileList
:: --------------------------------------------
set Label=%1
set sourceDir=%2
set exe=%3

echo. > %sourceDir%\FileList.ini
echo [%Label%] >> %sourceDir%\FileList.ini
echo exe=%Exe% >> %sourceDir%\FileList.ini

echo. >> %sourceDir%\FileList.ini

echo [FILES] >> %sourceDir%\FileList.ini

FOR /F "tokens=1*" %%i in ('dir %sourceDir% /b') do (
      echo %%i >> %sourceDir%\FileList.ini
)
goto :EOF

:: --------------------------------------------
:SetAssemblyVersion
:: --------------------------------------------
ren %1 "%~n1.bak"
find_and_replace.exe "%~dpn1.bak" %1 "3.0.0.0" "%BuildPrefix%.%RevisionNumber%" 0 suppress_banner

Goto :EOF

:: --------------------------------------------
:RestoreAssemblyVersion
:: --------------------------------------------
del %1
echo ren "%~dpn1.bak" "%~nx1"
ren "%~dpn1.bak" "%~nx1"

Goto :EOF

:: --------------------------------------------
:Set32BitCore
:: --------------------------------------------
if EXIST %1 (
  corflags.exe %1 /32BIT+ 
) else (
   echo   Set32BitCore - %1 Not Found
) 
Goto :EOF

:: --------------------------------------------
:WriteNotes
:: --------------------------------------------
echo ---------------------------------------------------- > %1
echo Version %BuildPrefix%.%RevisionNumber% : %DATE% - %TIME% >> %1
echo ---------------------------------------------------- >> %1
echo Fixed:					>> %1
echo.>> %1
echo Added:					>> %1
echo.>> %1
echo Removed:					>> %1
echo. >>%1
echo. >>%1

if exist "%TargetInstallerBinaries%\Logs\CurrentRevision\changelog.txt" type "%TargetInstallerBinaries%\Logs\CurrentRevision\changelog.txt" >> %1

notepad %1

if exist "%TargetInstallerBinaries%\Logs\CurrentRevision\changelog.txt" del "%TargetInstallerBinaries%\Logs\CurrentRevision\changelog.txt"
copy %1 "%TargetInstallerBinaries%\Logs\CurrentRevision\changelog.txt"

goto :EOF

:Error
Call :RestoreAssemblyVersion "%CD%\MOG\AssemblyInfo.cpp"
Call :RestoreAssemblyVersion "%CD%\MOG_Client\AssemblyInfo.cs"
Call :RestoreAssemblyVersion "%CD%\MOG_ControlsLibrary\AssemblyInfo.cs"
Call :RestoreAssemblyVersion "%CD%\MOG_CoreControls\Properties\AssemblyInfo.cs"
Call :RestoreAssemblyVersion "%CD%\MOG_ServerManager\AssemblyInfo.cs"
Call :RestoreAssemblyVersion "%CD%\MOG_Server\AssemblyInfo.cs"
Call :RestoreAssemblyVersion "%CD%\MOG_EventViewer\AssemblyInfo.cs"
Call :RestoreAssemblyVersion "%CD%\MOG_CommandLine\AssemblyInfo.cs"

echo !!!Last build ended in an error!!!
pause
goto Menu

:DONE
echo Done!
:: Pause
