---
type: "THEORY"
title: "Introduction to Implicit Animations"
---


**What Are Implicit Animations?**

Implicit animations are Flutter's easiest way to add motion to your app. They automatically animate between old and new values whenever a property changes.

**Why "Implicit"?**
You don't explicitly control the animation - you just change a value, and Flutter smoothly transitions to the new state. The animation happens implicitly.

**When to Use Implicit Animations:**
- Simple property changes (size, color, position)
- UI state transitions (expanded/collapsed, enabled/disabled)
- Visual feedback on user actions
- When you want animations with minimal code

**Implicit vs Explicit Animations:**

| Implicit | Explicit |
|----------|----------|
| Change value, animation happens | Full control over animation |
| Less code, simpler | More code, more power |
| AnimatedContainer, AnimatedOpacity | AnimationController, Tween |
| Good for simple transitions | Good for complex sequences |

**The Pattern:**
Every implicit animation widget follows the same pattern:
1. Wrap your content in an Animated* widget
2. Specify a `duration`
3. Optionally add a `curve` for easing
4. Change a property via setState()
5. Flutter animates the change automatically!

