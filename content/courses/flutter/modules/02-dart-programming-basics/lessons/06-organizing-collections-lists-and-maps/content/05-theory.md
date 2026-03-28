---
type: "THEORY"
title: "List Length"
---


How many items are in a list?


**Useful pattern**: The last item is always at index `length - 1`:




```dart
void main() {
  var fruits = ['Apple', 'Banana', 'Orange'];

  var lastIndex = fruits.length - 1;
  print('Last fruit: ${fruits[lastIndex]}');  // Output: Orange
}
```
