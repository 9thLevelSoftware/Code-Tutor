---
type: "EXAMPLE"
title: "Database Operations (CRUD)"
---


### Create a Table (in Supabase Dashboard)

1. Go to **Table Editor** > **New Table**
2. Name: `todos`
3. Columns:
   - `id` (int8, primary key, auto-increment)
   - `user_id` (uuid, foreign key to auth.users)
   - `title` (text)
   - `completed` (bool, default: false)
   - `created_at` (timestamptz, default: now())

### Insert (Create)

```dart
Future<void> createTodo(String title) async {
  await supabase.from('todos').insert({
    'title': title,
    'user_id': supabase.auth.currentUser!.id,
  });
}
```

### Select (Read)

```dart
Future<List<Map<String, dynamic>>> getTodos() async {
  final response = await supabase
      .from('todos')
      .select()
      .eq('user_id', supabase.auth.currentUser!.id)
      .order('created_at', ascending: false);
  
  return response;
}
```

### Update

```dart
Future<void> toggleTodo(int id, bool completed) async {
  await supabase
      .from('todos')
      .update({'completed': completed})
      .eq('id', id);
}
```

### Delete

```dart
Future<void> deleteTodo(int id) async {
  await supabase.from('todos').delete().eq('id', id);
}
```



```dart
// Compare to Firestore:
// Firebase:  FirebaseFirestore.instance.collection('todos').add(data)
// Supabase:  supabase.from('todos').insert(data)

// Firebase:  .where('userId', isEqualTo: uid).get()
// Supabase:  .select().eq('user_id', uid)
```
