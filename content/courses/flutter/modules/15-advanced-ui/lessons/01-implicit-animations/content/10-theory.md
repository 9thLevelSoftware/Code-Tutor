---
type: "THEORY"
title: "TweenAnimationBuilder - Custom Implicit Animations"
---


**TweenAnimationBuilder** lets you create custom implicit animations for any value type. It's incredibly flexible!

**Use Cases:**
- Animate values that no built-in widget supports
- Custom progress indicators
- Animated charts and graphs
- Any numeric animation

**How It Works:**
1. Define a `tween` (start -> end values)
2. Flutter interpolates between them over `duration`
3. Your `builder` receives the current animated value
4. Rebuild your widget using that value

