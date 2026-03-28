---
type: "THEORY"
title: "Storage Structure"
---


Firebase Storage organizes files like a file system:


**Best practices**:
- Organize by user ID or content type
- Use consistent naming conventions
- Avoid spaces in filenames (use hyphens or underscores)



```dart
gs://your-app.appspot.com/
├── users/
│   ├── user123/
│   │   ├── profile.jpg
│   │   └── documents/
│   │       └── resume.pdf
│   └── user456/
│       └── profile.jpg
├── posts/
│   ├── post001.jpg
│   └── post002.mp4
└── public/
    └── app-logo.png
```
