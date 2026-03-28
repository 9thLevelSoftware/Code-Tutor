---
type: "EXAMPLE"
title: "Dart Frog in Action"
---


Here is a typical Dart Frog endpoint for fetching a user:



```dart
// routes/users/[id].dart
import 'package:dart_frog/dart_frog.dart';
import 'package:my_app/database.dart'; // You choose the database library

Future<Response> onRequest(RequestContext context, String id) async {
  final request = context.request;
  
  if (request.method == HttpMethod.get) {
    // You write your own database logic
    final db = context.read<Database>();
    final user = await db.query('SELECT * FROM users WHERE id = ?', [id]);
    
    if (user == null) {
      return Response(statusCode: 404, body: 'User not found');
    }
    
    return Response.json(body: user.toJson());
  }
  
  return Response(statusCode: 405);
}
```
