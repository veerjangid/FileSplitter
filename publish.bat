@echo off
echo Publishing FileSplitter Application...
echo.

rem Clean previous builds
if exist "bin\Release" rmdir /s /q "bin\Release"
if exist "publish" rmdir /s /q "publish"

rem Build and publish the application
dotnet publish -c Release -o publish --self-contained false

echo.
echo Publishing completed!
echo Output location: %cd%\publish
echo.

rem Create shortcuts
echo Creating shortcuts...
echo @echo off > publish\FileSplitter-GUI.bat
echo cd /d "%~dp0" >> publish\FileSplitter-GUI.bat
echo FileSplitter.exe gui >> publish\FileSplitter-GUI.bat

echo @echo off > publish\FileSplitter-Console.bat
echo cd /d "%~dp0" >> publish\FileSplitter-Console.bat
echo FileSplitter.exe console >> publish\FileSplitter-Console.bat

echo Shortcuts created!
echo.
echo Files ready for distribution in 'publish' folder.
pause 