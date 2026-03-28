---
type: "THEORY"
title: "Setting Up Firebase Storage"
---


### 1. Enable Storage in Firebase Console

1. Go to https://console.firebase.google.com
2. Select your project
3. Click **"Storage"** in left sidebar
4. Click **"Get started"**
5. Choose security rules:
   - **Test mode**: Anyone can read/write (insecure!)
   - **Production mode**: Requires authentication (recommended)
6. Select location (same as Firestore for consistency)
7. Click **"Done"**

### 2. Add Package to pubspec.yaml


Run:



```bash
flutter pub get
```
