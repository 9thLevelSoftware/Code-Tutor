---
type: "ANALOGY"
title: "Understanding the Concept"
---

Think of a traffic light at an intersection:

Static Display (no conditions):
- Always shows green light
- Never changes
- Dangerous! Cars and pedestrians confused

Conditional Display (based on state):
- If state = 'stop' → Show RED light
- If state = 'caution' → Show YELLOW light
- If state = 'go' → Show GREEN light
- Changes based on current condition

React conditional rendering works the same:
- Different UI based on state/props
- Show login button if NOT logged in
- Show profile if logged in
- Show loading spinner while fetching
- Show error message if failed

One component, many possible displays!