---
type: "THEORY"
title: "AnimatedSwitcher - Widget Transitions"
---


**AnimatedSwitcher** animates between different widgets, not just property changes. It's perfect for:

- Loading states (spinner -> content)
- Tab content transitions
- Swapping icons or text
- Any widget replacement

**Key Concept:** AnimatedSwitcher uses the widget's `key` to detect changes. When the key changes, it animates out the old widget and animates in the new one.

