---
type: "KEY_POINT"
title: "Animation Curves"
---


**Curves** control how the animation progresses over time. They add personality to your animations!

**Common Curves:**

| Curve | Effect | Best For |
|-------|--------|----------|
| `Curves.linear` | Constant speed | Progress indicators |
| `Curves.easeIn` | Start slow, end fast | Elements leaving screen |
| `Curves.easeOut` | Start fast, end slow | Elements entering screen |
| `Curves.easeInOut` | Slow-fast-slow | Most UI transitions |
| `Curves.bounceOut` | Bouncy ending | Playful UI, notifications |
| `Curves.elasticOut` | Springy overshoot | Attention-grabbing |

**Pro Tip:** `Curves.easeInOut` is a safe default for most animations. It feels natural because real objects accelerate and decelerate.

