---
type: "KEY_POINT"
title: "💾 LIGHTWEIGHT ALTERNATIVE: Skip Android Studio (Save 5GB+)"
---


**Want to save disk space?** You can install ONLY the Command Line Tools instead of the full Android Studio IDE (which is 5-10GB). This is for advanced users comfortable with the terminal.

### Option B: Command Line Tools Only

**Step 1: Download Command Line Tools**

Go to: `https://developer.android.com/studio#command-tools`

Download "Command line tools only" for your OS (~150MB instead of 1GB+).

**Step 2: Set Up Directory Structure**

```bash
# Create Android SDK directory
mkdir -p ~/Android/Sdk/cmdline-tools

# Extract downloaded zip to:
~/Android/Sdk/cmdline-tools/latest/
```

**Step 3: Set Environment Variables**

```bash
# Add to ~/.bashrc, ~/.zshrc, or PowerShell profile
export ANDROID_SDK_ROOT=$HOME/Android/Sdk
export PATH=$PATH:$ANDROID_SDK_ROOT/cmdline-tools/latest/bin
export PATH=$PATH:$ANDROID_SDK_ROOT/platform-tools
export PATH=$PATH:$ANDROID_SDK_ROOT/emulator
```

**Step 4: Install Required Components**

```bash
# Accept licenses
sdkmanager --licenses

# Install essential components
sdkmanager "platform-tools"
sdkmanager "emulator"
sdkmanager "platforms;android-34"
sdkmanager "system-images;android-34;google_apis;x86_64"
sdkmanager "build-tools;34.0.0"
```

**Step 5: Create and Run Emulator**

```bash
# Create virtual device
avdmanager create avd -n pixel6 -k "system-images;android-34;google_apis;x86_64"

# Start emulator
emulator -avd pixel6
```

**Step 6: Verify Flutter Sees It**

```bash
flutter doctor
flutter devices
```

### Which Should You Choose?

| Option | Disk Space | Ease of Use | Recommended For |
|--------|------------|-------------|------------------|
| Android Studio (Full) | ~10GB | ⭐⭐⭐⭐⭐ Easy | Beginners |
| Command Line Tools | ~2GB | ⭐⭐ Advanced | Experienced devs |

**Our Recommendation**: If you're learning Flutter, install the full Android Studio - it's easier and has helpful visual tools. The command line approach is great if you're experienced or have limited disk space.

---

