---
type: "THEORY"
title: "Common Patterns"
---


### User-Specific Data


### Subcollections


### Array Fields


### Increment/Decrement




```dart
// Increment likes count
.update({
  'likes': FieldValue.increment(1)
});

// Decrement
.update({
  'stock': FieldValue.increment(-1)
});
```
