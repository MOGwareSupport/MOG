@echo off 
set EchoType=off
set BuildType=Release
set RevisionNumber=0
set TargetRepository=M:
Call :GetBackupVersion
Call :GetMinorVersion
Call :GetVersionInfo

:RESET
set SourceBinaries=%TargetRepository%\Updates\1.8
set MOG_TOOLS=%TargetRepository%\Updates\Uploads
set EvalBuild=%TargetRepository%\Updates\InstallerBuilds\Eval
set FirstBuild=%TargetRepository%\Updates\InstallerBuilds\1.0

:StartMenu
set Selection=
echo ----------------------------------------
echo MOG Version Updater: %BuildType%
echo Target Repository: %TargetRepository%
echo ----------------------------------------
echo I - Internal 1.8 deploys
echo M - MOG 1.0 Installer deploys
echo E - MOG Eval Installer deploys
echo.
echo 0 - Exit
set /P Selection="Enter Selection:"

if /I %Selection%x==Ex Goto PostEval
if /I %Selection%x==Mx Goto Post1.0
if /I %Selection%x==Ix Goto MENU

if /I %Selection%x==0x Goto Done

echo.
echo ERROR - INVALID ENTRY
echo.
Goto StartMenu

:PostEval
set SourceBinaries=%EvalBuild%
set FallThrough=true
echo --------------------------------------------------
echo Posting all to %SourceBinaries% for install build
echo --------------------------------------------------
call :GetLastCodeRevisionNumber
Call :GetNewCodeRevisionNumber

Goto PostServer

:Post1.0
set SourceBinaries=%FirstBuild%
set FallThrough=true
echo --------------------------------------------------
echo Posting all to %SourceBinaries% for install build
echo --------------------------------------------------
call :GetLastCodeRevisionNumber
Call :GetNewCodeRevisionNumber

Goto PostServer


:MENU
@cls
set Selection=
set FallThrough=false
@echo %EchoType%
echo ----------------------------------------
echo MOG 1.8 Downloadable Version Updater: %BuildType%
echo Target Repository: %TargetRepository% : %SourceBinaries%
echo ----------------------------------------
Call :ShowVersionInfo
echo C - Post Client and Deploy
echo S - Post Server and Deploy
echo L - Post Server Loader And Deploy
echo I - Post Client Loader and Deploy
echo A - Post All
echo.
echo T - Change Target Repository
echo 5 - Toggle Echo
echo R - Release
echo D - Debug
echo.
echo 0 - Exit
set /P Selection="Enter Selection:"

if /I %Selection%x==Cx Goto PostClient
if /I %Selection%x==Sx Goto PostServer
if /I %Selection%x==Lx Goto PostServerLoader
if /I %Selection%x==Ix Goto PostClientLoader
if /I %Selection%x==Bx Goto PostMogBridge
if /I %Selection%x==Ax Goto PostAll

if /I %Selection%x==Ex Goto ToggleEcho
if /I %Selection%x==Rx Goto ReleaseBuild
if /I %Selection%x==Dx Goto DebugBuild
if /I %Selection%x==Tx Goto SetTargetRepository

if %Selection%x==5x Goto ToggleEcho

if %Selection%x==0x Goto Done


echo.
echo ERROR - INVALID ENTRY
echo.
Goto MENU

:PostAll
set FallThrough=true

:: --------------------------------------------
:Postserver
:: --------------------------------------------
echo ---------------------------
echo Create Server Version %ArchiveVersion%

md "%SourceBinaries%\Server\%ArchiveVersion%"

echo Posting new server version
:: Post new version
xcopy MOG_Server\bin\%BuildType%\MOG_Server.exe "%SourceBinaries%\Server\%ArchiveVersion%"  /Y >NUL
xcopy MOG_Server\MOG_Server.exe.manifest "%SourceBinaries%\Server\%ArchiveVersion%" /Y  >NUL
xcopy MOG_Server\bin\%BuildType%\*.dll "%SourceBinaries%\Server\%ArchiveVersion%" /Y >NUL
if exist MOG_Server\bin\%BuildType%\*.chm xcopy MOG_Server\bin\%BuildType%\*.chm "%SourceBinaries%\Server\%ArchiveVersion%" /Y >NUL

if %BuildType%==Debug xcopy MOG_Server\bin\%BuildType%\*.pdb "%SourceBinaries%\Server\%ArchiveVersion%"  /Y>NUL

::
:: Write version info file
::
echo [info] > "%SourceBinaries%\Server\%ArchiveVersion%\VERSION.INI"
echo Name=%ServerName% >> "%SourceBinaries%\Server\%ArchiveVersion%\VERSION.INI"
echo MajorVersion=%SMajorVersion% >> "%SourceBinaries%\Server\%ArchiveVersion%\VERSION.INI"
echo MinorVersion=%MinorVersion% >> "%SourceBinaries%\Server\%ArchiveVersion%\VERSION.INI"
echo Type=Server >> "%SourceBinaries%\Server\%ArchiveVersion%\VERSION.INI"

echo ---------------------------
echo Done!
Call :WriteNotes "%SourceBinaries%\Server\%ArchiveVersion%\WhatsNew.txt"
Call :WriteFileList Server "%SourceBinaries%\Server\%ArchiveVersion%" MOG_Server.exe

:: Deploy it
if exist "%SourceBinaries%\Server\Current" rd "%SourceBinaries%\Server\Current" /S /Q
md "%SourceBinaries%\Server\Current"
xcopy "%SourceBinaries%\Server\%ArchiveVersion%" "%SourceBinaries%\Server\Current" /E /R /H >NUL

:: Pause
if %FallThrough%==false goto MENU


:: --------------------------------------------
:PostserverLoader
:: --------------------------------------------
echo ---------------------------
echo Create Server loader Version %ArchiveVersion%

md "%SourceBinaries%\ServerLoader\%ArchiveVersion%"

echo Posting new server version
:: Post new version
xcopy MOG_Server_Loader\bin\%BuildType%\MOG_Server_Loader.exe "%SourceBinaries%\ServerLoader\%ArchiveVersion%"  /Y >NUL
xcopy MOG_Server_Loader\MOG_Server_Loader.exe.manifest "%SourceBinaries%\ServerLoader\%ArchiveVersion%" /Y  >NUL
xcopy MOG_Server_Loader\Loader.ini "%SourceBinaries%\ServerLoader\%ArchiveVersion%"  /Y >NUL
xcopy MOG_Server_Loader\bin\%BuildType%\*.dll "%SourceBinaries%\ServerLoader\%ArchiveVersion%" /Y >NUL

if %BuildType%==Debug xcopy MOG_Server\bin\%BuildType%\*.pdb "%SourceBinaries%\ServerLoader\%ArchiveVersion%"  /Y>NUL

if exist "%SourceBinaries%\ServerLoader\Current" rd "%SourceBinaries%\ServerLoader\Current" /S /Q

md "%SourceBinaries%\ServerLoader\Current"
xcopy MOG_Server_Loader\bin\%BuildType%\MOG_Server_Loader.exe "%SourceBinaries%\ServerLoader\Current"  /Y >NUL
xcopy MOG_Server_Loader\MOG_Server_Loader.exe.manifest "%SourceBinaries%\ServerLoader\Current" /Y  >NUL
xcopy MOG_Server_Loader\Loader.ini "%SourceBinaries%\ServerLoader\Current"  /Y >NUL
xcopy MOG_Server_Loader\bin\%BuildType%\*.dll "%SourceBinaries%\ServerLoader\Current" /Y >NUL

if %BuildType%==Debug xcopy MOG_Server\bin\%BuildType%\*.pdb "%SourceBinaries%\ServerLoader\Current"  /Y>NUL


echo ---------------------------
echo Done!

:: Pause
if %FallThrough%==false goto MENU

:: --------------------------------------------
:PostClientLoader
:: --------------------------------------------
echo ---------------------------
echo Create Client loader Version %ArchiveVersion%

md "%SourceBinaries%\ClientLoader\%ArchiveVersion%"

echo Posting new MOG_Client_Loader version
:: Post new version
xcopy MOG_Client_Loader\bin\%BuildType%\MOG_Client_Loader.exe "%SourceBinaries%\ClientLoader\%ArchiveVersion%"  /Y >NUL
xcopy MOG_Client_Loader\MOG_Client_Loader.exe.manifest "%SourceBinaries%\ClientLoader\%ArchiveVersion%" /Y  >NUL
xcopy MOG_Client_Loader\Loader.ini "%SourceBinaries%\ClientLoader\%ArchiveVersion%"  /Y >NUL
xcopy MOG_Client_Loader\bin\%BuildType%\*.dll "%SourceBinaries%\ClientLoader\%ArchiveVersion%" /Y >NUL

if exist "%SourceBinaries%\ClientLoader\Current" rd "%SourceBinaries%\ClientLoader\Current" /S /Q

md "%SourceBinaries%\ClientLoader\Current"
xcopy MOG_Client_Loader\bin\%BuildType%\MOG_Client_Loader.exe "%SourceBinaries%\ClientLoader\Current"  /Y >NUL
xcopy MOG_Client_Loader\MOG_Client_Loader.exe.manifest "%SourceBinaries%\ClientLoader\Current" /Y  >NUL
xcopy MOG_Client_Loader\Loader.ini "%SourceBinaries%\ClientLoader\Current"  /Y >NUL
xcopy MOG_Client_Loader\bin\%BuildType%\*.dll "%SourceBinaries%\ClientLoader\Current" /Y >NUL

if %BuildType%==Debug xcopy MOG_Server\bin\%BuildType%\*.pdb "%SourceBinaries%\ClientLoader\Current"  /Y>NUL


echo ---------------------------
echo Done!

:: Pause
if %FallThrough%==false goto MENU



:: --------------------------------------------
:Postclient
:: --------------------------------------------
echo ---------------------------
echo Create client Version %ArchiveVersion%

md "%SourceBinaries%\Client\%ArchiveVersion%"

echo ---------------------------
echo Posting new client version to :"%SourceBinaries%\Client\%ArchiveVersion%"
echo ---------------------------
:: Post new version
xcopy MOG_Client\bin\%BuildType%\MOG_Client.exe "%SourceBinaries%\Client\%ArchiveVersion%"  /Y >NUL
xcopy MOG_Client\MOG_Client.exe.manifest "%SourceBinaries%\Client\%ArchiveVersion%" /Y  >NUL
xcopy MOG_Client\MogClient_Skin.info "%SourceBinaries%\Client\%ArchiveVersion%" /Y  >NUL
xcopy MOG_Client\MOG_Sounds.ini "%SourceBinaries%\Client\%ArchiveVersion%" /Y >NUL
xcopy MOG_Client\bin\%BuildType%\*.dll "%SourceBinaries%\Client\%ArchiveVersion%" /Y >NUL
if exist MOG_Client\bin\%BuildType%\*.chm xcopy MOG_Client\bin\%BuildType%\*.chm "%SourceBinaries%\Client\%ArchiveVersion%" /Y >NUL
if %BuildType%==Debug xcopy MOG_Client\bin\%BuildType%\*.pdb "%SourceBinaries%\Client\%ArchiveVersion%"  /Y>NUL

:: Post Server Manager
xcopy MOG_ServerManager\bin\%BuildType%\MOG_ServerManager.exe "%SourceBinaries%\Client\%ArchiveVersion%"  /Y >NUL
xcopy MOG_ServerManager\MOG_ServerManager.exe.manifest "%SourceBinaries%\Client\%ArchiveVersion%" /Y  >NUL
xcopy MOG_ServerManager\bin\%BuildType%\*.dll "%SourceBinaries%\Client\%ArchiveVersion%" /Y >NUL
xcopy MOG_ServerManager\*.rtf "%SourceBinaries%\Client\%ArchiveVersion%"  /Y >NUL
if exist MOG_ServerManager\bin\%BuildType%\*.chm xcopy MOG_ServerManager\bin\%BuildType%\*.chm "%SourceBinaries%\Client\%ArchiveVersion%" /Y >NUL

:: Post Event viewer
xcopy MOG_EventViewer\bin\%BuildType%\MOG_EventViewer.exe "%SourceBinaries%\Client\%ArchiveVersion%"  /Y >NUL
xcopy MOG_EventViewer\MOG_EventViewer.exe.manifest "%SourceBinaries%\Client\%ArchiveVersion%" /Y  >NUL
xcopy MOG_EventViewer\bin\%BuildType%\*.dll "%SourceBinaries%\Client\%ArchiveVersion%" /Y >NUL
if exist MOG_EventViewer\bin\%BuildType%\*.chm xcopy MOG_EventViewer\bin\%BuildType%\*.chm "%SourceBinaries%\Client\%ArchiveVersion%" /Y >NUL

echo ---------------------------
echo Posting new slaves
echo ---------------------------
:: Post new slave
xcopy MOG_Slave\bin\%BuildType%\MOG_Slave.exe "%SourceBinaries%\Client\%ArchiveVersion%" /Y >NUL
xcopy MOG_slave\bin\%BuildType%\*.dll "%SourceBinaries%\Client\%ArchiveVersion%"  /Y >NUL
xcopy MOG_Slave\MOG_Slave.exe.manifest "%SourceBinaries%\Client\%ArchiveVersion%"  /Y >NUL

if %BuildType%==Debug xcopy MOG_Slave\bin\%BuildType%\*.pdb "%SourceBinaries%\Client\%ArchiveVersion%"  /Y >NUL


echo ---------------------------
echo Posting new Command Line MOG
echo ---------------------------
:: Post new CommandLine
xcopy MOG_CommandLine\bin\%BuildType%\MOG_CommandLine.exe "%SourceBinaries%\Client\%ArchiveVersion%"  /Y >NUL
xcopy MOG_CommandLine\bin\%BuildType%\*.dll "%SourceBinaries%\Client\%ArchiveVersion%"  /Y >NUL

if %BuildType%==Debug xcopy MOG_CommandLine\bin\%BuildType%\*.pdb "%SourceBinaries%\Client\%ArchiveVersion%"  /Y >NUL

::
:: Write version info file
::
echo [info] > "%SourceBinaries%\Client\%ArchiveVersion%\VERSION.INI"
echo Name=%ClientName% >> "%SourceBinaries%\Client\%ArchiveVersion%\VERSION.INI"
echo MajorVersion=%CMajorVersion% >> "%SourceBinaries%\Client\%ArchiveVersion%\VERSION.INI"
echo MinorVersion=%MinorVersion% >> "%SourceBinaries%\Client\%ArchiveVersion%\VERSION.INI"
echo Type=Client >> "%SourceBinaries%\Client\%ArchiveVersion%\VERSION.INI"

echo. >> "%SourceBinaries%\Client\%ArchiveVersion%\VERSION.INI"

echo [SERVERCOMPATABILITY] >> "%SourceBinaries%\Client\%ArchiveVersion%\VERSION.INI"
echo ServerMajorVersion=%SMajorVersion% >> "%SourceBinaries%\Client\%ArchiveVersion%\VERSION.INI"
echo ServerMinorversion=%MinorVersion% >> "%SourceBinaries%\Client\%ArchiveVersion%\VERSION.INI"

echo ---------------------------
echo Done!
Call :WriteNotes "%SourceBinaries%\Client\%ArchiveVersion%\WhatsNew.txt"
Call :WriteFileList Client "%SourceBinaries%\Client\%ArchiveVersion%" MOG_Client.exe

:: Deploy it
if exist "%SourceBinaries%\Client\Current" rd "%SourceBinaries%\Client\Current" /S /Q
md "%SourceBinaries%\Client\Current"
xcopy "%SourceBinaries%\Client\%ArchiveVersion%" "%SourceBinaries%\Client\Current" /E /R /H >NUL


:: Pause
if %FallThrough%==false goto MENU

Goto MENU

:: --------------------------------------------
:ImportClient
:: --------------------------------------------
echo ---------------------------
echo Import client

echo ---------------------------
echo Importing new client
echo ---------------------------
:: Import new version
"%MOG_PATH%"\MOG_CommandLine Import EInstall~Repository~MOG~Updates~1.7~Client~Current{All}MOG_Client.exe MOG_Client\bin\%BuildType%\MOG_Client.exe 
"%MOG_PATH%"\MOG_CommandLine Import EInstall~Repository~MOG~Updates~1.7~Client~Current{All}MOG_Client.exe.manifest MOG_Client\MOG_Client.exe.manifest 
"%MOG_PATH%"\MOG_CommandLine Import EInstall~Repository~MOG~Updates~1.7~Client~Current{All}MOG_Sounds.ini MOG_Client\MOG_Sounds.ini

for %%I in (MOG_Client\bin\%BuildType%\*.dll) do (
"%MOG_PATH%"\MOG_CommandLine Import EInstall~Repository~MOG~Updates~1.7~Client~Current{All}%%~nxI %%I
)
"%MOG_PATH%"\MOG_CommandLine Import EInstall~Repository~MOG~Updates~1.7~Client~Current{All}moghelp.chm MOG_Client\bin\%BuildType%\*.chm

if %BuildType%==Debug (
	for %%I in (MOG_Client\bin\%BuildType%\*.pdb) do (
	    "%MOG_PATH%"\MOG_CommandLine Import EInstall~Repository~MOG~Updates~1.7~Client~Current{All}%%~nxI %%I
	)
)


Goto MENU

:: --------------------------------------------
:PostForMogTools
:: --------------------------------------------
"%programfiles%\winrar\rar.exe" A "%MOG_TOOLS%\Client_%ArchiveVersion%".rar "%SourceBinaries%\Client\%ArchiveVersion%\*.*" -ep
"%programfiles%\winrar\rar.exe" A "%MOG_TOOLS%\Server_%ArchiveVersion%".rar "%SourceBinaries%\Server\%ArchiveVersion%\*.*" -ep
Goto MENU

:: --------------------------------------------
:PostCurrentForMogTools
:: --------------------------------------------
:MOGToolsMENU
::cls
echo ----------------------------------------
echo MOG 1.6 Downloadable Version Updater: %BuildType%
echo Target Repository: %TargetRepository% : %SourceBinaries%
echo ----------------------------------------
set ToolsSelection=
set ToolsFallThrough=false
set Compress=true
echo C - Upload Client
echo S - Upload Server
echo A - Upload All(Client, Server)
echo Z - Zip All
echo.
echo 0 - Back
set /P ToolsSelection="Enter Selection:"

if /I %ToolsSelection%x==Cx Goto UploadClient
if /I %ToolsSelection%x==Sx Goto UploadServer
if /I %ToolsSelection%x==Ax Goto UploadAll
if /I %ToolsSelection%x==Zx Goto ZipAll

if %ToolsSelection%x==0x Goto Menu

echo.
echo ERROR - INVALID ENTRY
echo.
Goto MOGToolsMENU

:: ------------------
:UploadAll
:: ------------------
set ToolsFallThrough=true

:: ------------------
:UploadClient
:: ------------------
cd /D %MOG_TOOLS%
davcopy.exe "Client_%ArchiveVersion%".zip "http://mog.sharepointspace.com/moguser/Build Archive/Latest Interm Binaries" /User:public3\kier

if %ToolsFallThrough%==false goto MOGToolsMENU

:: ------------------
:UploadServer
:: ------------------
cd /D %MOG_TOOLS%
davcopy.exe "Server_%ArchiveVersion%".zip "http://mog.sharepointspace.com/moguser/Build Archive/Latest Interm Binaries" /User:public3\kier

Goto MOGToolsMENU

:: ------------------
:ZipAll
:: ------------------
if %Compress%==true pacomp -a "%MOG_TOOLS%\Server_%ArchiveVersion%.zip" "%SourceBinaries%\Server\Current\*.*"
if %Compress%==true pacomp -a "%MOG_TOOLS%\Client_%ArchiveVersion%".zip "%SourceBinaries%\Client\Current\*.*"

Goto MOGToolsMENU

:: --------------------------------------------
:ImportServer
:: --------------------------------------------
echo ---------------------------
echo Import client

echo ---------------------------
echo Importing new client
echo ---------------------------
:: Import new version
"%MOG_PATH%"\MOG_CommandLine Import EInstall~Repository~MOG~Updates~1.7~Client~Current{All}MOG_Client.exe MOG_Client\bin\%BuildType%\MOG_Client.exe 
"%MOG_PATH%"\MOG_CommandLine Import EInstall~Repository~MOG~Updates~1.7~Client~Current{All}MOG_Client.exe.manifest MOG_Client\MOG_Client.exe.manifest 
"%MOG_PATH%"\MOG_CommandLine Import EInstall~Repository~MOG~Updates~1.7~Client~Current{All}MOG_Sounds.ini MOG_Client\MOG_Sounds.ini

for %%I in (MOG_Client\bin\%BuildType%\*.dll) do (
"%MOG_PATH%"\MOG_CommandLine Import EInstall~Repository~MOG~Updates~1.7~Client~Current{All}%%~nxI %%I
)
"%MOG_PATH%"\MOG_CommandLine Import EInstall~Repository~MOG~Updates~1.7~Client~Current{All}moghelp.chm MOG_Client\bin\%BuildType%\*.chm

if %BuildType%==Debug (
	for %%I in (MOG_Client\bin\%BuildType%\*.pdb) do (
	    "%MOG_PATH%"\MOG_CommandLine Import EInstall~Repository~MOG~Updates~1.7~Client~Current{All}%%~nxI %%I
	)
)

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
:GetBackupVersion
:: --------------------------------------------

if %TIME:~0,2% LSS 10 (
	set ArchiveVersion=%DATE:~4,2%-%DATE:~7,2%-%DATE:~12,2%_%TIME:~1,1%.%TIME:~3,2%.%TIME:~6,2%
) else (
	set ArchiveVersion=%DATE:~4,2%-%DATE:~7,2%-%DATE:~12,2%_%TIME:~0,2%.%TIME:~3,2%.%TIME:~6,2%
)
goto :EOF

:: --------------------------------------------
:GetNewCodeRevisionNumber
:: --------------------------------------------
if not exist %SourceBinaries%\Logs md %SourceBinaries%\Logs
if not exist %SourceBinaries%\Logs\CurrentRevision md %SourceBinaries%\Logs\CurrentRevision

set /P RevisionNumber="--- Enter latest SVN revision that will be in this build:"

echo @%RevisionNumber%>%SourceBinaries%\Logs\CurrentRevision\Revision.txt

goto :EOF

:: --------------------------------------------
:GetLastCodeRevisionNumber
:: --------------------------------------------
if exist %SourceBinaries%\Logs\CurrentRevision\Revision.txt (
	FOR /F "delims=@" %%i in (%SourceBinaries%\Logs\CurrentRevision\Revision.txt) do (
		call :SetRevisionNumber %%i
	)
) 

echo --- Last logs latest revision number (%RevisionNumber%)

goto :EOF

:: --------------------------------------------
:SetRevisionNumber
:: --------------------------------------------
set RevisionNumber=%1

goto :EOF

:: --------------------------------------------
:GetMinorVersion
:: --------------------------------------------

if %TIME:~0,2% LSS 10 (
	set MinorVersion=%DATE:~12,2%%DATE:~4,2%%DATE:~7,2%0%TIME:~1,1%%TIME:~3,2%%TIME:~6,2%
) else (
	set MinorVersion=%DATE:~12,2%%DATE:~4,2%%DATE:~7,2%%TIME:~0,2%%TIME:~3,2%%TIME:~6,2%
)
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
FOR /F "eol=; tokens=1,2 delims=:" %%i in (postDownloadableVersion.info) do (
	if /I %%i EQU ClientName Call :SetName ClientName "%%j %ArchiveVersion%"
	if /I %%i EQU CMajorVersion Call :SetName CMajorVersion %%j
	if /I %%i EQU CMinorVersion Call :SetName CMinorVersion %%j

	if /I %%i EQU ServerName Call :SetName ServerName "%%j %ArchiveVersion%"
	if /I %%i EQU SMajorVersion Call :SetName SMajorVersion %%j
	if /I %%i EQU SMinorVersion Call :SetName SMinorVersion %%j
)

goto :EOF

:: --------------------------------------------
:SetName
:: --------------------------------------------
set %1=%2

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
:WriteNotes
:: --------------------------------------------
echo ------------------------------------------ > %1
echo %DATE% - %TIME% >> %1
echo ------------------------------------------ >> %1
echo Fixed:					>> %1
echo.>> %1
echo Added:					>> %1
echo.>> %1
echo Removed:					>> %1
notepad %1

goto :EOF

:DONE
echo Done!
:: Pause
