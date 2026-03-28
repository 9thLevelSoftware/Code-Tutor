---
type: "THEORY"
title: "Accessing Map Values"
---


Use square brackets with the key to retrieve its value. Maps provide fast lookup - finding a value by its key is nearly instant, even in large Maps.

**Note:** If the key doesn't exist, you'll get `null`. Use `containsKey()` to check first, or provide a default with `??`.




```dart
void main() {
  var phoneBook = {
    'Alice': '555-1234',
    'Bob': '555-5678',
  };

  print(phoneBook['Alice']);  // Output: 555-1234
  print(phoneBook['Bob']);    // Output: 555-5678
}
```
