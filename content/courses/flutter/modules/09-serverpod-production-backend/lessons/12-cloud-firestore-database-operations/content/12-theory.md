---
type: "THEORY"
title: "Real-Time Updates with Streams"
---


**Streams automatically update when data changes!**

### Single Document Stream


### Collection Stream


**Use with StreamBuilder** for automatic UI updates!



```dart
Stream<List<Task>> getTasksStream() {
  return _tasksCollection
      .snapshots()
      .map((snapshot) {
        return snapshot.docs
            .map((doc) => Task.fromFirestore(doc))
            .toList();
      });
}
```
