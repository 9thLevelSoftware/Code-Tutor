---
type: "THEORY"
title: "Looping Through Maps"
---


You can iterate through Maps using `entries`, `keys`, or `values`. The `entries` property gives you both the key and value together in each iteration - this is the most common approach.

**Using entries (recommended):** Each entry has `.key` and `.value` properties.

**Output**: `Alice scored 95` and `Bob scored 87`




```dart
void main() {
  var scores = {'Alice': 95, 'Bob': 87};

  for (var entry in scores.entries) {
    print('${entry.key} scored ${entry.value}');
  }
}
```
