---
type: "EXAMPLE"
title: "Real-Time Subscriptions"
---


### Listen to Changes

```dart
class TodosProvider extends ChangeNotifier {
  List<Map<String, dynamic>> _todos = [];
  RealtimeChannel? _subscription;
  
  List<Map<String, dynamic>> get todos => _todos;
  
  void subscribeTodos() {
    // Initial fetch
    _fetchTodos();
    
    // Real-time subscription
    _subscription = supabase
        .channel('todos_changes')
        .onPostgresChanges(
          event: PostgresChangeEvent.all,
          schema: 'public',
          table: 'todos',
          callback: (payload) {
            _fetchTodos(); // Refresh on any change
          },
        )
        .subscribe();
  }
  
  Future<void> _fetchTodos() async {
    final response = await supabase
        .from('todos')
        .select()
        .order('created_at');
    
    _todos = List<Map<String, dynamic>>.from(response);
    notifyListeners();
  }
  
  @override
  void dispose() {
    _subscription?.unsubscribe();
    super.dispose();
  }
}
```



```dart
// Real-time is automatic when you subscribe!
// Any INSERT, UPDATE, or DELETE triggers the callback
```
