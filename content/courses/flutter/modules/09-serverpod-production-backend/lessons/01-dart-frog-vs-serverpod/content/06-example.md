---
type: "EXAMPLE"
title: "Calling Serverpod from Flutter"
---


Here is the magic - calling your endpoint from Flutter:



```dart
// In your Flutter app - fully type-safe, no manual API calls!
import 'package:my_app_client/my_app_client.dart';

class UserService {
  final Client client;
  
  UserService(this.client);
  
  Future<User?> fetchUser(int id) async {
    // Direct method call - Serverpod generated this client code!
    return await client.user.getUser(id);
  }
  
  Future<User> createUser(String name, String email) async {
    final user = User(
      name: name,
      email: email,
      createdAt: DateTime.now(),
    );
    // Full type safety - User class is shared between server and client
    return await client.user.createUser(user);
  }
  
  Future<List<User>> search(String query) async {
    return await client.user.searchUsers(query);
  }
}

// No JSON parsing, no http package, no URL strings, no error-prone manual work!
```
