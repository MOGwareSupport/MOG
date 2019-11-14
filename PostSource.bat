@echo off

"C:\Program Files\SlikSvn\bin"\svn checkout file:///\Nemesis/SVN/MOG.3.0 MOGSource
if NOT exist MOGSource Goto :EOF

cd MOGSource 

call :RecurseDir
rmdir MOG_Server /Q /S
rmdir MOG_ProgressTester /Q /S
rmdir MOG_LicenseGenerator /Q /S
rmdir MOG_LicenseInstaller /Q /S
rmdir MOG_Post /Q /S
rmdir MOG_RemoteServerManager /Q /S
rmdir XboxUtils /Q /S

cd..
"C:\Program Files\WinRAR\Rar.exe" -r a L:\_MOGReleases\Pending\MOGSource.rar MOGSource\*.*
rmdir MOGSource /Q /S

goto END

:RecurseDir
:: echo Recursing == %CD% ==
:: dir /B /AD
FOR /F "tokens=*" %%i in ('dir /B /AD') DO (
  if /I %%i==.svn ( 
      rmdir %%i /S /Q
      echo    Removed %CD%\%%i      
    ) else (
      pushd "%%i"
      call :RecurseDir
    )   
)

popd
Goto :EOF



:END
