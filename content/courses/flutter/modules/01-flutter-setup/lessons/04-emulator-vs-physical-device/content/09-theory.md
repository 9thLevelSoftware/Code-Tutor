---
type: "THEORY"
title: "Common Issues and Fixes"
---


### "No devices found"

**Solution**: Make sure at least one is running:
- Start Chrome
- Start an emulator
- Connect a physical device

### Emulator is very slow

**Solutions**:
- Enable hardware acceleration in BIOS (Intel VT-x or AMD-V)
- Increase RAM allocated to emulator (in Android Studio)
- Use a physical device instead

### "Waiting for another flutter command to release the startup lock"

**Solution**:

```bash
# Delete the lock file manually
# Mac/Linux:
rm -f ~/development/flutter/bin/cache/lockfile

# Windows PowerShell:
Remove-Item -Force C:\src\flutter\bin\cache\lockfile

# Or simply restart your computer (easiest fix)
```

### iOS Simulator not showing

**Mac Only Solution**:



```bash
sudo xcode-select --switch /Applications/Xcode.app/Contents/Developer
```
