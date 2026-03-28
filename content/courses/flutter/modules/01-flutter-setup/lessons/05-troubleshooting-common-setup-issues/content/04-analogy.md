---
type: "ANALOGY"
title: "Problem 2: Android License Not Accepted"
---


### Error message:

### Solution:


If this doesn't work:

1. Open Android Studio
2. Go to **Settings** → **Appearance & Behavior** → **System Settings** → **Android SDK**
3. Click **SDK Tools** tab
4. Check **Android SDK Command-line Tools**
5. Click **Apply**

Then run `flutter doctor --android-licenses` again.



```bash
# Accept all Android licenses
flutter doctor --android-licenses

# Type 'y' and press Enter for each license
```
