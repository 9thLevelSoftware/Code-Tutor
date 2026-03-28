---
type: "THEORY"
title: "Removing Items"
---



**Two ways to remove**:
- **`remove('value')`** - Remove specific item
- **`removeAt(index)`** - Remove by position



```dart
void main() {
  var fruits = ['Apple', 'Banana', 'Orange'];

  fruits.remove('Banana');
  print(fruits);  // Output: [Apple, Orange]

  fruits.removeAt(0);  // Remove by index
  print(fruits);  // Output: [Orange]
}
```
