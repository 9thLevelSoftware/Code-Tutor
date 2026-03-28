---
type: "EXAMPLE"
title: "Real-World Examples"
---


### User Profile


### Product Inventory


### Shopping Cart




```dart
void main() {
  List<Map<String, dynamic>> cart = [
    {'name': 'Laptop', 'price': 999.99, 'quantity': 1},
    {'name': 'Mouse', 'price': 29.99, 'quantity': 2},
  ];

  var total = 0.0;
  for (var item in cart) {
    total += item['price'] * item['quantity'];
  }

  print('Total: \$$total');  // Output: Total: $1059.97
}
```
