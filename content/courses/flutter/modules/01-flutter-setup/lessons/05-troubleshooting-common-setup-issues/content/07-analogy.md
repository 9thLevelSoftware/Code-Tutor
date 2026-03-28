---
type: "ANALOGY"
title: "Problem 5: App Builds But Crashes Immediately"
---


### Check 1: Clean and Rebuild

```bash
# Stop the app if running (press 'q' in terminal)

# Clean the project
flutter clean

# Get fresh dependencies
flutter pub get

# Run again
flutter run
```

### Check 2: Check for Errors in Code

Look at the terminal output. Common errors:

↳ Missing semicolon

↳ Missing import: `import 'package:flutter/material.dart';`

↳ Type mismatch - check your variables



```dart
Error: The argument type 'int' can't be assigned to 'String'
```
