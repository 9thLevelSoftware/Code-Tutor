---
type: "THEORY"
title: "AnimatedOpacity and AnimatedScale"
---


**AnimatedOpacity** - Fade elements in and out:

```dart
AnimatedOpacity(
  duration: const Duration(milliseconds: 500),
  opacity: _isVisible ? 1.0 : 0.0,
  child: const Text('I fade in and out!'),
)
```

**AnimatedScale** - Grow and shrink elements:

```dart
AnimatedScale(
  duration: const Duration(milliseconds: 300),
  scale: _isLarge ? 1.5 : 1.0,
  child: const Icon(Icons.favorite, size: 48),
)
```

**Combining Animations:**
You can nest these widgets to combine effects:

```dart
AnimatedOpacity(
  duration: const Duration(milliseconds: 300),
  opacity: _isVisible ? 1.0 : 0.0,
  child: AnimatedScale(
    duration: const Duration(milliseconds: 300),
    scale: _isVisible ? 1.0 : 0.5,
    child: const MyWidget(),
  ),
)
```

This creates a "pop in" effect where the widget fades in while scaling up.

