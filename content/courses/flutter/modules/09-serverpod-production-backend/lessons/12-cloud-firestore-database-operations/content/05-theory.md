---
type: "THEORY"
title: "Setting Up Firestore"
---


### 1. Enable Firestore in Firebase Console

1. Go to https://console.firebase.google.com
2. Select your project
3. Click **"Firestore Database"** in left sidebar
4. Click **"Create database"**
5. **Select mode**:
   - **Test mode** (for learning): Anyone can read/write (insecure!)
   - **Production mode**: Requires security rules (recommended)
6. Choose location (select closest to your users)
7. Click **"Enable"**

### 2. Verify Package in pubspec.yaml


Run:



```bash
flutter pub get
```
