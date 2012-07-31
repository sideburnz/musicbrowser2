ECHO.
ECHO.Usage: DevInstall.cmd [/u][/debug]
ECHO.
ECHO.This script requires Administrative privileges to run properly.
ECHO.Start > All Programs > Accessories> Right-Click Command Prompt > Select 'Run As Administrator'
ECHO.
 
CD "D:\Code\MusicBrowser2"
D:

set CompanyName=Joocer
set AssemblyName=MusicBrowser
set DirName=MusicBrowser
set RegistrationName=Registration
set ProgramImage=Music-256x256.png
 
ECHO.Determine whether we are on an 32 or 64 bit machine
if "%PROCESSOR_ARCHITECTURE%"=="x86" if "%PROCESSOR_ARCHITEW6432%"=="" goto x86
set ProgramFilesPath=%ProgramFiles(x86)%
ECHO.
 
goto unregister
 
:x86

    ECHO.On an x86 machine
    set ProgramFilesPath=%ProgramFiles%
    ECHO.

:unregister

    ECHO.*** Unregistering and deleting assemblies ***
    ECHO.

    ECHO.Unregister and delete previously installed files (which may fail if nothing is registered)
    ECHO.

    ECHO.Unregister the application entry points
    %windir%\ehome\RegisterMCEApp.exe /allusers "%ProgramFilesPath%\%CompanyName%\%DirName%\%RegistrationName%.xml" /u
    ECHO.

    ECHO.Remove the DLL from the Global Assembly cache
    "%ProgramFilesPath%\Microsoft SDKs\Windows\v7.0A\bin\gacutil.exe" /u "%AssemblyName%"
    "%ProgramFilesPath%\Microsoft SDKs\Windows\v7.0A\bin\gacutil.exe" /u "ServiceStack.Text"
  
    ECHO.

    ECHO.Delete the folder containing the DLLs and supporting files (silent if successful)
    rd /s /q "%ProgramFilesPath%\%CompanyName%\%DirName%"
    ECHO.

    REM Exit out if the /u uninstall argument is provided, leaving no trace of program files.
    if "%1"=="/u" goto exit
                
:releasetype
 
    if "%1"=="/debug" goto debug
    set ReleaseType=Release
    ECHO.
    goto checkbin
                
:debug
    set ReleaseType=Debug
    ECHO.
                
:checkbin
 
    if exist ".\MusicBrowser2\bin\%ReleaseType%\%AssemblyName%.dll" goto register
    ECHO.Cannot find %ReleaseType% binaries.
    ECHO.Build solution as %ReleaseType% and run script again. 
    goto exit
                
:register

    ECHO.*** Copying and registering assemblies ***
    ECHO.

    ECHO.Create the path for the binaries and supporting files (silent if successful)
    md "%ProgramFilesPath%\%CompanyName%\%DirName%"
    ECHO.
    
    ECHO.Copy the binaries to program files
    copy /y ".\MusicBrowser2\bin\%ReleaseType%\%AssemblyName%.dll" "%ProgramFilesPath%\%CompanyName%\%DirName%\""
    copy /y ".\resource\ServiceStack.Text.dll" "%ProgramFilesPath%\%CompanyName%\%DirName%\
    ECHO.
    
    ECHO.Copy the registration XML to program files
    copy /y ".\Setup\Registration.xml" "%ProgramFilesPath%\%CompanyName%\%DirName%\%RegistrationName%.xml"
    ECHO.
    
    ECHO.Copy the program image to program files
    copy /y ".\resource\Application\%ProgramImage%" "%ProgramFilesPath%\%CompanyName%\%DirName%\application.png"
    ECHO.

    ECHO.Register the DLL with the global assembly cache
    "%ProgramFilesPath%\Microsoft SDKs\Windows\v7.0A\Bin\gacutil.exe" /if "%ProgramFilesPath%\%CompanyName%\%DirName%\%AssemblyName%.dll"
    "%ProgramFilesPath%\Microsoft SDKs\Windows\v7.0A\Bin\gacutil.exe" /if "%ProgramFilesPath%\%CompanyName%\%DirName%\ServiceStack.Text.dll"
    ECHO.

    ECHO.Register the application with Windows Media Center
    %windir%\ehome\RegisterMCEApp.exe /allusers "%ProgramFilesPath%\%CompanyName%\%DirName%\%RegistrationName%.xml"
    ECHO.

pause

%SystemRoot%\ehome\ehshell.exe /entrypoint:{46572B5B-2B75-40B3-B48A-F7A308846CB7}\{49233D7D-3063-4B89-801E-C038ADEE6EF9} /nostartupanimation

:exit
