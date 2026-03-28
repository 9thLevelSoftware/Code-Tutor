---
type: "THEORY"
title: "Step 4: Deep Link Configuration"
---

To support deep linking, ensure your app is configured to handle the specific URL scheme or domain in your platform files (AndroidManifest.xml and Info.plist).

```dart
// In main.dart, initialize deep link listener
void main() {
  final router = _createRouter();
  runApp(MaterialApp.router(routerConfig: router));
}
```

Now, when a user clicks `myapp://post/123`, GoRouter automatically extracts `123` and navigates to the `PostDetailScreen`!