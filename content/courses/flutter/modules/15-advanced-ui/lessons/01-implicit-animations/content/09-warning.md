---
type: "WARNING"
title: "AnimatedSwitcher Key Requirement"
---


**Common Mistake:** Forgetting to add a `key` to the child widget!

```dart
// WRONG - Won't animate!
AnimatedSwitcher(
  duration: const Duration(milliseconds: 300),
  child: Text('$_count'),
)

// CORRECT - Will animate
AnimatedSwitcher(
  duration: const Duration(milliseconds: 300),
  child: Text(
    '$_count',
    key: ValueKey<int>(_count),
  ),
)
```

**Why?** Flutter uses keys to determine widget identity. Without a key, Flutter sees the same `Text` widget type and just updates its data - no transition needed. With a key, Flutter sees a completely new widget and performs the animation.

