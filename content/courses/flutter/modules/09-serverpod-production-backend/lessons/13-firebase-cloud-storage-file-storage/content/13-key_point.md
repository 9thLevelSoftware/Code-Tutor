---
type: "KEY_POINT"
title: "Answer Key"
---


### Answer 1: B
**Correct**: To save storage costs and prevent quota issues

Old files consume storage (which costs money) and count toward your quota. For example, if a user updates their profile picture 10 times, you'd be storing 10 images instead of 1. Always delete the old file before uploading a new one.

### Answer 2: C
**Correct**: By user ID (users/{userId}/...)

Organizing by user ID makes it easy to implement security rules (users can only access their own files), manage per-user quotas, and delete all user data when they delete their account. It's the industry standard pattern.

### Answer 3: B
**Correct**: It provides user feedback, especially for large files

Without progress indicators, users might think the app froze when uploading a 10MB video. Progress bars (0%, 25%, 50%, 100%) reassure users that the upload is working and show how long it will take.

