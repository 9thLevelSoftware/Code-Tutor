@echo off
REM Setup Git Hooks for Code Tutor
REM Run this script once after cloning the repository

echo Setting up Git hooks for Code Tutor...

REM Check if pre-commit is installed
where pre-commit >nul 2>&1
if %ERRORLEVEL% equ 0 (
    echo Installing pre-commit hooks...
    pre-commit install
    pre-commit install --hook-type pre-push
    echo Pre-commit hooks installed successfully!
) else (
    echo pre-commit not found. Installing via pip...
    pip install pre-commit
    if %ERRORLEVEL% equ 0 (
        pre-commit install
        pre-commit install --hook-type pre-push
        echo Pre-commit hooks installed successfully!
    ) else (
        echo Failed to install pre-commit. Please install manually: pip install pre-commit
    )
)

REM Ensure CSharpier tool manifest exists
if not exist ".config" mkdir .config

if not exist ".config\dotnet-tools.json" (
    echo {"version":1,"isRoot":true,"tools":{"csharpier":{"version":"0.29.2","commands":["dotnet-csharpier"]}}} > .config\dotnet-tools.json
    echo Created dotnet-tools.json manifest
)

REM Restore dotnet tools
dotnet tool restore
if %ERRORLEVEL% equ 0 (
    echo Dotnet tools restored successfully!
)

echo.
echo Setup complete! Hooks will run on commit and push.
echo To run all hooks manually: pre-commit run --all-files
pause
