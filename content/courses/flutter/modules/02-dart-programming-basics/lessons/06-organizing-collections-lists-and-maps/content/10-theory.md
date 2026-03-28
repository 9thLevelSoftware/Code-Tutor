---
type: "THEORY"
title: "Typed Lists (Recommended)"
---


Be explicit about what type of items your list holds:




```dart
void main() {
  List<String> fruits = ['Apple', 'Banana'];
  List<int> numbers = [1, 2, 3];
  List<double> prices = [19.99, 24.50];

  // fruits.add(123);  // ❌ Error: can't add int to List<String>
  fruits.add('Orange');  // ✅ Works!
}
```
