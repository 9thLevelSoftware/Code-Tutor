---
type: "THEORY"
title: "Step 8: Security Rules (CRITICAL!)"
---


### Firestore Security Rules


### Storage Security Rules




```javascript
rules_version = '2';
service firebase.storage {
  match /b/{bucket}/o {
    function isSignedIn() {
      return request.auth != null;
    }

    function isOwner(userId) {
      return isSignedIn() && request.auth.uid == userId;
    }

    // User profile pictures
    match /users/{userId}/profile/{fileName} {
      allow read: if true;
      allow write: if isOwner(userId)
                   && request.resource.contentType.matches('image/.*')
                   && request.resource.size < 5 * 1024 * 1024;  // 5MB max
    }

    // Post images
    match /posts/{userId}/{fileName} {
      allow read: if true;
      allow write: if isOwner(userId)
                   && request.resource.contentType.matches('image/.*')
                   && request.resource.size < 10 * 1024 * 1024;  // 10MB max
    }
  }
}
```
