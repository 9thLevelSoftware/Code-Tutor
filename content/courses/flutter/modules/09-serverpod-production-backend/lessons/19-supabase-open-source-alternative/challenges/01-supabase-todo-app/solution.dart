import 'package:flutter/material.dart';
import 'package:supabase_flutter/supabase_flutter.dart';

final supabase = Supabase.instance.client;

class TodoService {
  Future<List<Map<String, dynamic>>> getTodos() async {
    return await supabase
        .from('todos')
        .select()
        .eq('user_id', supabase.auth.currentUser!.id)
        .order('created_at');
  }
  
  Future<void> addTodo(String title) async {
    await supabase.from('todos').insert({
      'title': title,
      'user_id': supabase.auth.currentUser!.id,
    });
  }
  
  Future<void> toggleTodo(int id, bool completed) async {
    await supabase
        .from('todos')
        .update({'completed': completed})
        .eq('id', id);
  }
  
  Future<void> deleteTodo(int id) async {
    await supabase.from('todos').delete().eq('id', id);
  }
  
  RealtimeChannel subscribeToChanges(VoidCallback onUpdate) {
    return supabase
        .channel('todos')
        .onPostgresChanges(
          event: PostgresChangeEvent.all,
          schema: 'public',
          table: 'todos',
          callback: (_) => onUpdate(),
        )
        .subscribe();
  }
}