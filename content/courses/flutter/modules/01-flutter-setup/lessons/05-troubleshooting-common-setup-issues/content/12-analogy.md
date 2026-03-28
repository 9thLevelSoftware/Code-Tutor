---
type: "ANALOGY"
title: "Problem 10: \"Version Solving Failed\""
---


### Error message:
`Because package_a requires package_b ^1.0.0 which doesn't exist, version solving failed.`

This happens when your packages have conflicting version requirements - Package A needs version 1.x of something, but Package B needs version 2.x.

### Solution:
Update packages to compatible versions or reset your dependencies:




```bash
# Update all packages
flutter pub upgrade

# If that doesn't work, delete lock file
rm pubspec.lock

# Get fresh dependencies
flutter pub get
```
