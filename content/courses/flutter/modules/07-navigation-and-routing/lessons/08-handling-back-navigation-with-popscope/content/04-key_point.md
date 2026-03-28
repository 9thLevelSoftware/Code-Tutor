---
type: "KEY_POINT"
title: "Migration from WillPopScope"
---


| Old (Deprecated) | New (Flutter 3.12+) |
|------------------|---------------------|
| `WillPopScope` | `PopScope` |
| `onWillPop: () async => false` | `canPop: false` |
| `onWillPop: () async => true` | `canPop: true` |
| Return value controlled pop | `onPopInvokedWithResult` callback |

