---
type: "THEORY"
title: "Common Setup Issues and Solutions"
---


### Issue 1: "Firebase already exists"
**Solution**: Use a different project name or select existing project during `flutterfire configure`

### Issue 2: "Package 'firebase_core' has no versions..."
**Solution**: Run `flutter pub upgrade` and ensure you have stable Flutter channel

### Issue 3: "Build failed on iOS"
**Solution**:

### Issue 4: "Gradle build failed on Android"
**Solution**: Ensure your `android/app/build.gradle` has:

### Issue 5: "Multiple dex files define..."
**Solution**: Add to `android/app/build.gradle`:



```gradle
android {
    // ...
    packagingOptions {
        exclude 'META-INF/DEPENDENCIES'
    }
}
```
