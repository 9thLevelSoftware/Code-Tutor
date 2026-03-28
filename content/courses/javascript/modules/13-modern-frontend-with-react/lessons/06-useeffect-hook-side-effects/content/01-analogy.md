---
type: "ANALOGY"
title: "Understanding the Concept"
---

Imagine a security guard at a building entrance:

Without security guard:
- People come and go
- No one tracks entrances/exits
- No one locks doors at night
- No cleanup when building closes

With security guard (useEffect):
- WHEN people arrive (component mounts) → Check them in
- WHEN visitor badge expires (dependency changes) → Issue new badge
- WHEN building closes (component unmounts) → Lock doors, cleanup
- Guard watches specific things (dependencies)
- Guard performs actions automatically

React useEffect is your component's security guard:
- Runs code AFTER render (side effects)
- Runs when component mounts
- Runs when specific values change
- Cleanup when component unmounts
- Perfect for: API calls, timers, subscriptions, document title changes!