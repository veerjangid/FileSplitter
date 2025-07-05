@echo off
echo FileSplitter Installation Script
echo ================================
echo.

rem Check if .NET 6 is installed
echo Checking for .NET 6.0...
dotnet --version > nul 2>&1
if errorlevel 1 (
    echo .NET 6.0 is not installed. Please install .NET 6.0 SDK first.
    echo Download from: https://dotnet.microsoft.com/download/dotnet/6.0
    pause
    exit /b 1
)

echo .NET 6.0 is installed.
echo.

rem Restore dependencies
echo Restoring dependencies...
dotnet restore
if errorlevel 1 (
    echo Failed to restore dependencies.
    pause
    exit /b 1
)

echo Dependencies restored successfully.
echo.

rem Build the project
echo Building the project...
dotnet build -c Release
if errorlevel 1 (
    echo Build failed.
    pause
    exit /b 1
)

echo Build completed successfully.
echo.

rem Create test output directory
if not exist "test_output" mkdir test_output
echo Created test_output directory.
echo.

echo Installation completed successfully!
echo.
echo You can now run the application using:
echo   - dotnet run (for version selector)
echo   - dotnet run gui (for GUI version)
echo   - dotnet run console (for console version)
echo   - Or use the batch files: run_gui.bat, run_console.bat
echo.
echo For development, open the solution in Visual Studio:
echo   - Open FileSplitter.sln
echo.
pause 