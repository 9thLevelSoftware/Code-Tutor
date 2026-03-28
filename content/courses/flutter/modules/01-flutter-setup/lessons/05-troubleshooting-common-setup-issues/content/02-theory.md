---
type: "THEORY"
title: "The #1 Troubleshooting Tool: flutter doctor"
---


This command checks your entire setup and tells you what's wrong.


**What it checks**:
- ✅ Is Flutter installed?
- ✅ Is Dart available?
- ✅ Are Android tools installed?
- ✅ Is Xcode available? (Mac)
- ✅ Are there any missing dependencies?

**How to read the output**:


- **[✓]**: Working perfectly
- **[!]**: Working but with warnings
- **[✗]**: Not working, needs fixing



```dart
[✓] Flutter (Channel stable, 3.38.6)
[✗] Android toolchain - develop for Android devices
    ✗ Android SDK not found
[!] Xcode - develop for iOS and macOS (Xcode 15.0)
    ✗ CocoaPods not installed
[✓] Chrome - develop for the web
[✓] VS Code (version 1.85.0)
```
