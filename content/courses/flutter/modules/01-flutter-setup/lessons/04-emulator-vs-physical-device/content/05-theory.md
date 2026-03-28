---
type: "THEORY"
title: "Setting Up a Physical Android Device"
---


### Step 1: Enable Developer Mode

On your Android phone:

1. Go to **Settings** → **About Phone**
2. Find "Build Number"
3. Tap it **7 times** (yes, really!)
4. You'll see "You are now a developer!"

### Step 2: Enable USB Debugging

1. Go to **Settings** → **Developer Options**
2. Turn on **USB Debugging**
3. Connect your phone to your computer with a USB cable

### Step 3: Trust Your Computer

When you connect:
- Your phone will show "Allow USB debugging?"
- Check "Always allow from this computer"
- Tap "OK"

### Step 4: Verify Connection

In your terminal/PowerShell:


You should see your phone listed!



```bash
flutter devices
```
