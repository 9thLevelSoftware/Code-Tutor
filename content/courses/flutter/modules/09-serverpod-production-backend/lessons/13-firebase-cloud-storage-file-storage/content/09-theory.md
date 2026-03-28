---
type: "THEORY"
title: "Firebase Storage Security Rules"
---


### Default Rules (Test Mode - Insecure!)


### Production Rules (Secure)


### Update Rules in Firebase Console

1. Go to Firebase Console → Storage
2. Click "Rules" tab
3. Paste your security rules
4. Click "Publish"



```dart
rules_version = '2';
service firebase.storage {
  match /b/{bucket}/o {
    // User-specific files
    match /users/{userId}/{allPaths=**} {
      // Only the user can read/write their own files
      allow read, write: if request.auth != null && request.auth.uid == userId;
    }

    // Public files (anyone can read)
    match /public/{allPaths=**} {
      allow read: if true;
      allow write: if request.auth != null;  // Only authenticated users can write
    }

    // Posts (owner can write, anyone can read)
    match /posts/{postId} {
      allow read: if true;
      allow write: if request.auth != null;
    }

    // Validate file size (max 5MB for images)
    match /users/{userId}/profile/{fileName} {
      allow write: if request.auth != null
                   && request.auth.uid == userId
                   && request.resource.size < 5 * 1024 * 1024;  // 5MB
    }

    // Validate file type (only images)
    match /users/{userId}/images/{fileName} {
      allow write: if request.auth != null
                   && request.auth.uid == userId
                   && request.resource.contentType.matches('image/.*');
    }
  }
}
```
