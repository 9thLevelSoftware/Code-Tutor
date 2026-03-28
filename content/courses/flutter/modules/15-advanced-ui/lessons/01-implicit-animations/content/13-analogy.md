---
type: "ANALOGY"
title: "The Automatic Sliding Door"
---

Implicit animations in Flutter work like automatic sliding doors at a supermarket. You do not manually push the door open inch by inch. You just walk toward it (change a value), and the door smoothly slides open on its own. You control the destination ("open" or "closed"), and the door handles all the in-between motion automatically.

`AnimatedContainer`, `AnimatedOpacity`, and `AnimatedAlign` all follow this pattern. You change a property -- width, opacity, alignment -- and Flutter smoothly transitions from the old value to the new one. You choose the duration and curve, but you never manually calculate intermediate frames.

**The tradeoff is simplicity for control.** Automatic doors are perfect for most entrances, but if you need a door that opens halfway, pauses, then opens the rest of the way on a second trigger, you need an explicit animation controller. Implicit animations handle the common case beautifully; explicit animations handle the edge cases.
