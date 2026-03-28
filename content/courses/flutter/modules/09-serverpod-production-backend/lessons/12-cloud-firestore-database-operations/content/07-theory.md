---
type: "THEORY"
title: "Introduction"
---

### Create Firestore Service




```dart
// lib/services/firestore_service.dart
import 'package:cloud_firestore/cloud_firestore.dart';
import '../models/task.dart';

class FirestoreService {
  final FirebaseFirestore _firestore = FirebaseFirestore.instance;

  // Reference to tasks collection
  CollectionReference get _tasksCollection => _firestore.collection('tasks');

  // ========== CREATE ==========

  // Add a new task
  Future<String> createTask(Task task) async {
    try {
      final docRef = await _tasksCollection.add(task.toMap());
      return docRef.id; // Return the generated document ID
    } catch (e) {
      throw 'Failed to create task: $e';
    }
  }

  // Add task with custom ID
  Future<void> createTaskWithId(String id, Task task) async {
    try {
      await _tasksCollection.doc(id).set(task.toMap());
    } catch (e) {
      throw 'Failed to create task: $e';
    }
  }

  // ========== READ ==========

  // Get single task by ID
  Future<Task?> getTask(String taskId) async {
    try {
      final doc = await _tasksCollection.doc(taskId).get();

      if (doc.exists) {
        return Task.fromFirestore(doc);
      }
      return null;
    } catch (e) {
      throw 'Failed to get task: $e';
    }
  }

  // Get all tasks for a user (returns Future)
  Future<List<Task>> getUserTasks(String userId) async {
    try {
      final querySnapshot = await _tasksCollection
          .where('userId', isEqualTo: userId)
          .orderBy('createdAt', descending: true)
          .get();

      return querySnapshot.docs
          .map((doc) => Task.fromFirestore(doc))
          .toList();
    } catch (e) {
      throw 'Failed to get tasks: $e';
    }
  }

  // Get tasks as a Stream (real-time updates!)
  Stream<List<Task>> getUserTasksStream(String userId) {
    return _tasksCollection
        .where('userId', isEqualTo: userId)
        .orderBy('createdAt', descending: true)
        .snapshots()
        .map((snapshot) {
      return snapshot.docs.map((doc) => Task.fromFirestore(doc)).toList();
    });
  }

  // Get completed tasks only
  Future<List<Task>> getCompletedTasks(String userId) async {
    try {
      final querySnapshot = await _tasksCollection
          .where('userId', isEqualTo: userId)
          .where('isCompleted', isEqualTo: true)
          .orderBy('createdAt', descending: true)
          .get();

      return querySnapshot.docs
          .map((doc) => Task.fromFirestore(doc))
          .toList();
    } catch (e) {
      throw 'Failed to get completed tasks: $e';
    }
  }

  // ========== UPDATE ==========

  // Update entire task
  Future<void> updateTask(String taskId, Task task) async {
    try {
      await _tasksCollection.doc(taskId).update(task.toMap());
    } catch (e) {
      throw 'Failed to update task: $e';
    }
  }

  // Update specific fields only
  Future<void> updateTaskFields(String taskId, Map<String, dynamic> fields) async {
    try {
      await _tasksCollection.doc(taskId).update(fields);
    } catch (e) {
      throw 'Failed to update task fields: $e';
    }
  }

  // Toggle task completion
  Future<void> toggleTaskCompletion(String taskId, bool isCompleted) async {
    try {
      await _tasksCollection.doc(taskId).update({
        'isCompleted': !isCompleted,
      });
    } catch (e) {
      throw 'Failed to toggle task: $e';
    }
  }

  // ========== DELETE ==========

  // Delete a task
  Future<void> deleteTask(String taskId) async {
    try {
      await _tasksCollection.doc(taskId).delete();
    } catch (e) {
      throw 'Failed to delete task: $e';
    }
  }

  // Delete all completed tasks for a user
  Future<void> deleteCompletedTasks(String userId) async {
    try {
      final querySnapshot = await _tasksCollection
          .where('userId', isEqualTo: userId)
          .where('isCompleted', isEqualTo: true)
          .get();

      // Batch delete for efficiency
      final batch = _firestore.batch();
      for (var doc in querySnapshot.docs) {
        batch.delete(doc.reference);
      }
      await batch.commit();
    } catch (e) {
      throw 'Failed to delete completed tasks: $e';
    }
  }

  // ========== ADVANCED QUERIES ==========

  // Search tasks by title
  Future<List<Task>> searchTasks(String userId, String searchTerm) async {
    try {
      final querySnapshot = await _tasksCollection
          .where('userId', isEqualTo: userId)
          .where('title', isGreaterThanOrEqualTo: searchTerm)
          .where('title', isLessThanOrEqualTo: '$searchTerm\uf8ff')
          .get();

      return querySnapshot.docs
          .map((doc) => Task.fromFirestore(doc))
          .toList();
    } catch (e) {
      throw 'Failed to search tasks: $e';
    }
  }

  // Get task count for a user
  Future<int> getTaskCount(String userId) async {
    try {
      final querySnapshot = await _tasksCollection
          .where('userId', isEqualTo: userId)
          .count()
          .get();

      return querySnapshot.count ?? 0;
    } catch (e) {
      throw 'Failed to get task count: $e';
    }
  }
}
```
