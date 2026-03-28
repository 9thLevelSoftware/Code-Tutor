---
type: "THEORY"
title: "Running Your App on Different Devices"
---


### Option 1: Using the VS Code GUI

1. Click the device selector (bottom-right)
2. Choose your target device
3. Press F5 or click "Run"

### Option 2: Using the Terminal




```bash
# List available devices
flutter devices

# Run on a specific device
flutter run -d <device-id>

# Run on Chrome
flutter run -d chrome

# Run on all connected devices
flutter run -d all
```
