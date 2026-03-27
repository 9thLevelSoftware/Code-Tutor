# Setup Git Hooks for Code Tutor
# Run this script once after cloning the repository

Write-Host "Setting up Git hooks for Code Tutor..." -ForegroundColor Cyan

# Check if pre-commit is installed
$preCommitInstalled = Get-Command pre-commit -ErrorAction SilentlyContinue

if ($preCommitInstalled) {
    Write-Host "Installing pre-commit hooks..." -ForegroundColor Green
    pre-commit install
    pre-commit install --hook-type pre-push
    Write-Host "Pre-commit hooks installed successfully!" -ForegroundColor Green
} else {
    Write-Host "pre-commit not found. Installing via pip..." -ForegroundColor Yellow
    pip install pre-commit
    if ($LASTEXITCODE -eq 0) {
        pre-commit install
        pre-commit install --hook-type pre-push
        Write-Host "Pre-commit hooks installed successfully!" -ForegroundColor Green
    } else {
        Write-Host "Failed to install pre-commit. Please install manually: pip install pre-commit" -ForegroundColor Red
    }
}

# Ensure CSharpier is installed as a local tool
Write-Host "Ensuring CSharpier is available..." -ForegroundColor Cyan

# Check if there's a dotnet-tools.json, if not create one
$toolsManifest = Join-Path $PSScriptRoot ".config/dotnet-tools.json"
if (-not (Test-Path $toolsManifest)) {
    $configDir = Join-Path $PSScriptRoot ".config"
    if (-not (Test-Path $configDir)) {
        New-Item -ItemType Directory -Path $configDir -Force | Out-Null
    }

    $toolsContent = @{
        version = 1
        isRoot = $true
        tools = @{
            "csharpier" = @{
                version = "0.29.2"
                commands = @("dotnet-csharpier")
            }
        }
    } | ConvertTo-Json -Depth 10

    Set-Content -Path $toolsManifest -Value $toolsContent
    Write-Host "Created dotnet-tools.json manifest" -ForegroundColor Green
}

# Restore dotnet tools
dotnet tool restore
if ($LASTEXITCODE -eq 0) {
    Write-Host "Dotnet tools restored successfully!" -ForegroundColor Green
} else {
    Write-Host "Note: dotnet tool restore may require manual intervention" -ForegroundColor Yellow
}

Write-Host "`nSetup complete! Hooks will run on commit and push." -ForegroundColor Cyan
Write-Host "To run all hooks manually: pre-commit run --all-files" -ForegroundColor White
