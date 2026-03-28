---
type: "EXAMPLE"
title: "Your First Endpoint"
---


Let us verify everything works by examining the default endpoint:



```dart
// my_app_server/lib/src/endpoints/example_endpoint.dart

import 'package:serverpod/serverpod.dart';

// Every endpoint extends the Endpoint class
class ExampleEndpoint extends Endpoint {
  // This method can be called from your Flutter app!
  // The 'session' parameter is always first - it contains
  // database connection, auth info, logging, etc.
  Future<String> hello(Session session, String name) async {
    return 'Hello, $name!';
  }
}

// After running 'serverpod generate', you can call this from Flutter:
//
// final client = Client('http://localhost:8080/');
// final greeting = await client.example.hello('World');
// print(greeting); // Prints: Hello, World!
```
