---
type: "THEORY"
title: "Step 1: Define Your Route Constants"
---

Start by defining all your route names and paths in a central location. This prevents typos and makes refactoring easier.

```dart
class AppRoutes {
  static const String home = '/home';
  static const String search = '/search';
  static const String notifications = '/notifications';
  static const String messages = '/messages';
  static const String profile = '/profile';
  
  static const String postDetail = '/post/:id';
  static const String userDetail = '/user/:id';
}
```