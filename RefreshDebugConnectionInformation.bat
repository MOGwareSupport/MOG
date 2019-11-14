@echo off

Set SourceFile="C:\Program Files\MOGware\MOG Client\Current\MOG.ini"

if NOT EXIST %SourceFile% GOTO MissingSourceFile


echo Copying latest installed version of 'MOG.ini' to your development directories

echo.
echo MOG_Client\bin\Debug
copy %SourceFile% MOG_Client\bin\Debug

echo.
echo MOG_Client\bin\Release
copy %SourceFile% MOG_Client\bin\Release

echo.
echo MOG_Slave\bin\Debug
copy %SourceFile% MOG_Slave\bin\Debug

echo.
echo MOG_Slave\bin\Release
copy %SourceFile% MOG_Slave\bin\Release

echo.
echo MOG_Server\bin\Debug
copy %SourceFile% MOG_Server\bin\Debug

echo.
echo MOG_Server\bin\Release
copy %SourceFile% MOG_Server\bin\Release

echo.
echo MOG_CommandLine\bin\Debug
copy %SourceFile% MOG_CommandLine\bin\Debug

echo.
echo MOG_CommandLine\bin\Release
copy %SourceFile% MOG_CommandLine\bin\Release

echo.
echo MOG_EventViewer\bin\Debug
copy %SourceFile% MOG_EventViewer\bin\Debug

echo.
echo MOG_EventViewer\bin\Release
copy %SourceFile% MOG_EventViewer\bin\Release

echo.
echo MOG_ServerManager\bin\Debug
copy %SourceFile% MOG_ServerManager\bin\Debug

echo.
echo MOG_ServerManager\bin\Release
copy %SourceFile% MOG_ServerManager\bin\Release

echo.
echo Completed...
pause
GOTO :EOF


:MissingSourceFile
echo ERROR - Source File Missing
echo %SourceFile%
pause
GOTO :EOF
