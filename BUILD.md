# Code Tutor - Build & Distribution Guide

> **Note:** Code Tutor is a WPF application and only supports Windows.
> Cross-platform support would require porting to a framework like Avalonia or MAUI.

This guide explains how to build Code Tutor installers for distribution.

---

## Prerequisites

1. **.NET 8.0 SDK** - https://dotnet.microsoft.com/download/dotnet/8.0
2. **Inno Setup 6** (optional, for creating installer) - https://jrsoftware.org/isdl.php
3. **PowerShell 5.0+** (included with Windows 10/11)

## Quick Build

**Option 1: Using Batch File (Easiest)**
```batch
# Double-click or run from command line:
build-installer.bat
```

**Option 2: Using PowerShell**
```powershell
# From PowerShell:
.\build-installer.ps1
```

**Option 3: With Custom Version**
```powershell
.\build-installer.ps1 -Version "1.0.1"
```

## What Gets Built

The build script creates:

1. **Self-Contained Executable**
   - Location: `publish/CodeTutor.exe`
   - Size: ~80-120 MB (includes .NET runtime)
   - No installation required (portable)

2. **Windows Installer** (if Inno Setup is installed)
   - Location: `dist/CodeTutor-Setup-1.0.0.exe`
   - Size: ~80-120 MB (compressed)
   - Creates Start Menu shortcuts
   - Creates Desktop shortcut (optional)
   - Detects missing language runtimes

## Build Process Details

**Step 1: Clean Build**
- Removes old publish directory
- Creates fresh build environment

**Step 2: Publish Self-Contained App**
```bash
dotnet publish native-app-wpf/CodeTutor.Wpf.csproj \
  -c Release \
  -r win-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -o publish
```

**Step 3: Copy Content & Documentation**
- Copies `content/` directory (course files)
- Copies `docs/` directory (documentation)
- Copies `README.md`

**Step 4: Create Installer (Inno Setup)**
- Generates `installer.iss` script
- Compiles installer with Inno Setup
- Outputs to `dist/` directory

---

## Build Script Parameters

The `build-installer.ps1` script accepts the following parameters:

| Parameter        | Default     | Description                                     |
|------------------|-------------|-------------------------------------------------|
| `-Configuration` | `"Release"` | Build configuration (`Release` or `Debug`)      |
| `-Version`       | `"1.0.0"`   | Version string embedded in the installer        |
| `-SkipBuild`     | `$false`    | Switch to skip the build step and reuse existing publish directory |

### Examples

```powershell
# Debug build (with symbols)
.\build-installer.ps1 -Configuration Debug

# Skip build (use existing publish directory)
.\build-installer.ps1 -SkipBuild

# Custom version
.\build-installer.ps1 -Version "2.0.0-beta"
```

---

## Manual Build (Without Script)

```bash
# 1. Build
dotnet publish native-app-wpf/CodeTutor.Wpf.csproj -c Release -r win-x64 --self-contained -o publish

# 2. Copy content
xcopy /E /I /Y content publish\Content
xcopy /E /I /Y docs publish\docs
copy README.md publish\

# 3. Create ZIP for distribution
powershell Compress-Archive -Path publish\* -DestinationPath dist\CodeTutor-Portable-1.0.0.zip
```

---

## Distribution Checklist

Before distributing your build:

### Testing
- [ ] Run the installer on a clean Windows VM
- [ ] Verify all shortcuts work
- [ ] Test with Python installed
- [ ] Test with Python NOT installed (should show warning)
- [ ] Test with Node.js installed
- [ ] Test code execution in all available languages
- [ ] Verify content loads correctly
- [ ] Check progress saving/loading
- [ ] Test uninstall process

### Documentation
- [ ] Update version number in `build-installer.ps1`
- [ ] Update version number in `native-app-wpf/CodeTutor.Wpf.csproj`
- [ ] Update CHANGELOG.md with release notes
- [ ] Update README.md if needed

### Distribution
- [ ] Upload installer to GitHub Releases
- [ ] Create release notes
- [ ] Tag the release in git: `git tag v1.0.0`
- [ ] Push tag: `git push origin v1.0.0`

---

## Troubleshooting

### "dotnet not found"
**Solution**: Install .NET 8.0 SDK from https://dotnet.microsoft.com/download

### "Inno Setup not found"
**Solution**: The build will still create the portable executable in `publish/`. To create an installer:
1. Install Inno Setup from https://jrsoftware.org/isdl.php
2. Run the build script again

### Build fails with "Access Denied"
**Solution**: Close any running instances of Code Tutor and try again

### Installer too large
**Solution**: The self-contained build includes the .NET runtime (~60-80 MB). This is normal and ensures the app works without requiring .NET installation.

### Code execution fails after installation
**Checklist**:
- [ ] Check if language runtime is installed (python, node, java, etc.)
- [ ] Verify language is in PATH
- [ ] Check CodeExecutor logs in `%APPDATA%\CodeTutor\logs\`

---

## Platform-Specific Notes

- **Target**: Windows 10 (1809) or later
- **Architecture**: x64 (64-bit)
- **Runtime**: Self-contained (no .NET installation required)
- **Installer**: Inno Setup creates Start Menu + Desktop shortcuts
