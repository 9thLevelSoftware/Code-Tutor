---
type: "EXAMPLE"
title: "Serverpod in Action"
---


Here is the same user endpoint in Serverpod:



```dart
// First, define your model in lib/src/protocol/user.yaml
// class: User
// table: users
// fields:
//   name: String
//   email: String
//   createdAt: DateTime

// Serverpod generates User class and database methods automatically!

// lib/src/endpoints/user_endpoint.dart
import 'package:serverpod/serverpod.dart';
import '../generated/protocol.dart'; // Auto-generated!

class UserEndpoint extends Endpoint {
  // This method is automatically callable from Flutter!
  Future<User?> getUser(Session session, int id) async {
    // Type-safe ORM query - no SQL strings
    return await User.db.findById(session, id);
  }
  
  Future<User> createUser(Session session, User user) async {
    // Automatic validation, serialization, database insert
    return await User.db.insertRow(session, user);
  }
  
  Future<List<User>> searchUsers(Session session, String query) async {
    return await User.db.find(
      session,
      where: (t) => t.name.ilike('%$query%'),
      orderBy: (t) => t.createdAt,
      orderDescending: true,
      limit: 20,
    );
  }
}
```
