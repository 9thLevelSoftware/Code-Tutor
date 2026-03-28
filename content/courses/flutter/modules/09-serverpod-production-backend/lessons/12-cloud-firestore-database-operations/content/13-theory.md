---
type: "THEORY"
title: "Batch Operations (Multiple Writes)"
---


For performance, batch multiple writes:


**Benefits**:
- ✅ Atomic (all succeed or all fail)
- ✅ More efficient (single network call)
- ✅ Up to 500 operations per batch



```dart
Future<void> batchUpdateTasks(List<Task> tasks) async {
  final batch = _firestore.batch();

  for (var task in tasks) {
    final docRef = _tasksCollection.doc(task.id);
    batch.update(docRef, task.toMap());
  }

  await batch.commit(); // Execute all updates at once
}
```
