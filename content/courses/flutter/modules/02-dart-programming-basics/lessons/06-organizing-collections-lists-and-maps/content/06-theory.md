---
type: "THEORY"
title: "Adding Items to a List"
---



**`.add()`** adds an item to the **end** of the list.



```dart
void main() {
  var fruits = ['Apple', 'Banana'];

  print(fruits);  // Output: [Apple, Banana]

  fruits.add('Orange');
  print(fruits);  // Output: [Apple, Banana, Orange]

  fruits.add('Mango');
  print(fruits);  // Output: [Apple, Banana, Orange, Mango]
}
```
