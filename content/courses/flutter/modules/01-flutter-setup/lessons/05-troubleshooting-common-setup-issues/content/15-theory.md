---
type: "THEORY"
title: "Common Error Patterns"
---


| If you see... | It usually means... |
|---------------|---------------------|
| `command not found` | PATH not set correctly |
| `licenses not accepted` | Run `flutter doctor --android-licenses` |
| `version solving failed` | Package conflict - run `flutter pub upgrade` |
| `gradle build failed` | Android build issue - clean and rebuild |
| `pod install failed` | iOS/Mac dependency issue - reinstall CocoaPods |
| `waiting for lock` | Previous Flutter command stuck - restart |

