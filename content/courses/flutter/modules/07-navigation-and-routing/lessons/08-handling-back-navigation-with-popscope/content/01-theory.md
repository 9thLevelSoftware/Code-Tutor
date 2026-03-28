---
type: "THEORY"
title: "Why PopScope?"
---


`WillPopScope` was deprecated in Flutter 3.12. The new `PopScope` widget provides better control over back navigation with a cleaner API.

**Key differences:**
- `WillPopScope.onWillPop` returned `Future<bool>` (confusing)
- `PopScope.canPop` is a simple boolean
- `PopScope.onPopInvokedWithResult` gives you the pop result

