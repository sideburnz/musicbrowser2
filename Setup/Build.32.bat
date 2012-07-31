SET BUILD_TYPE=Release
if "%1" == "Debug" set BUILD_TYPE=Debug

REM Determine whether we are on an 32 or 64 bit machine
if "%PROCESSOR_ARCHITECTURE%"=="x86" if "%PROCESSOR_ARCHITEW6432%"=="" goto x86

set ProgramFilesPath=%ProgramFiles(x86)%

goto startInstall

:x86

set ProgramFilesPath=%ProgramFiles%

:startInstall

pushd "%~dp0"

CD G:\Code\MusicBrowser2\setup

SET WIX_BUILD_LOCATION=%ProgramFilesPath%\Windows Installer XML v3.5\bin
SET APP_INTERMEDIATE_PATH=..\MusicBrowser2\obj\%BUILD_TYPE%
SET OUTPUTNAME=..\MusicBrowser2 (32 bit).msi

REM Cleanup leftover intermediate files

del /f /q "%APP_INTERMEDIATE_PATH%\*.wixobj"
del /f /q "%OUTPUTNAME%"

REM Build the MSI for the setup package

"%WIX_BUILD_LOCATION%\candle.exe" .\setup.32.wxs -dBuildType=%BUILD_TYPE% -out "%APP_INTERMEDIATE_PATH%\setup.32.wixobj"
"%WIX_BUILD_LOCATION%\light.exe" "%APP_INTERMEDIATE_PATH%\setup.32.wixobj" -cultures:en-US -ext "%ProgramFilesPath%\Windows Installer XML v3.5\bin\WixUIExtension.dll" -ext "%ProgramFilesPath%\Windows Installer XML v3.5\bin\WixUtilExtension.dll" -loc setup-en-us.wxl -out "%OUTPUTNAME%"

popd


pause