---
type: "THEORY"
title: "Setting Up the Android Emulator"
---


This is like having a virtual phone inside your computer.

### Step 1: Install Android Studio

Even though we're using VS Code, we need Android Studio for the emulator.

1. Download from: `https://developer.android.com/studio`
2. Install it (this will take 5-10 minutes)
3. Open Android Studio
4. Click "More Actions" → "SDK Manager"
5. Make sure these are checked:
   - Android SDK Platform-Tools
   - Android SDK Build-Tools
   - Android SDK Command-line Tools

### Step 2: Create a Virtual Device

1. In Android Studio, click "More Actions" → "Virtual Device Manager"
2. Click "Create Device"
3. Choose a phone model (Pixel 6 is a good default)
4. Click "Next"
5. Download a system image (recommended: latest stable release)
6. Click "Next" → "Finish"

### Step 3: Start the Emulator

1. In the Virtual Device Manager, click the ▶ play button next to your device
2. Wait 1-2 minutes for it to boot up
3. You'll see a virtual phone appear on your screen!

### Step 4: Verify in VS Code

1. Open VS Code
2. Look at the bottom-right corner
3. Click where it shows the device
4. You should see your new emulator listed!

