---
type: "THEORY"
title: "Firestore Query Operators"
---


### Comparison Operators


### Ordering and Limiting




```dart
// Order by field (ascending)
.orderBy('createdAt')

// Order descending
.orderBy('createdAt', descending: true)

// Multiple orderBy
.orderBy('priority', descending: true)
.orderBy('createdAt')

// Limit results
.limit(10)

// Start after document (pagination)
.startAfterDocument(lastDocument)
```
