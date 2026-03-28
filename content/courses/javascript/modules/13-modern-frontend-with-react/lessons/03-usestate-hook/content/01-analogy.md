---
type: "ANALOGY"
title: "Understanding the Concept"
---

Think of a light switch in your house:

Without state (broken switch):
- You flip the switch up
- Light turns on for a moment
- Switch immediately resets to off
- Light goes back off
- Can't remember if it was on or off!

With state (working switch):
- You flip switch up → light turns ON
- Switch REMEMBERS it's on
- Stays on until you flip it down
- Switch REMEMBERS it's off
- State = Current position of the switch

React useState is like giving your component a working switch:
- Component can remember values between renders
- When state changes → component re-renders
- UI updates automatically to show new state
- Perfect for: counters, form inputs, toggles, user data!