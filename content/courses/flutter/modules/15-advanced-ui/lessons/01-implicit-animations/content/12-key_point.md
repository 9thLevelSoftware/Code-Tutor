---
type: "KEY_POINT"
title: "Choosing the Right Implicit Animation"
---


**Quick Reference:**

| Widget | Animates | Example Use |
|--------|----------|-------------|
| `AnimatedContainer` | Size, color, padding, decoration | Expandable cards |
| `AnimatedOpacity` | Opacity (0.0 to 1.0) | Fade in/out |
| `AnimatedScale` | Scale (size multiplier) | Zoom effects |
| `AnimatedRotation` | Rotation (in turns) | Spinning icons |
| `AnimatedSlide` | Position (as Offset) | Slide in/out |
| `AnimatedAlign` | Alignment within parent | Moving elements |
| `AnimatedPadding` | Padding | Expanding spacing |
| `AnimatedPositioned` | Position in Stack | Moving overlays |
| `AnimatedSwitcher` | Widget replacement | Content changes |
| `TweenAnimationBuilder` | Any value | Custom animations |

**Performance Tips:**
- Implicit animations are efficient - Flutter optimizes them
- Avoid animating during build if possible
- Use `const` widgets for children that don't change
- Keep durations reasonable (200-500ms for most UI)

