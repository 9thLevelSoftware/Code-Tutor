---
type: "THEORY"
title: "Advanced Bonus: When to Use If vs When"
---


While you'll learn about `when` expressions in the next lesson, here's a preview of when to use each:

**Use if/else for:**
- Binary decisions (two outcomes)
- Range comparisons
- Simple conditions

**Use when (covered next lesson) for:**
- Multiple specific values
- Complex condition patterns
- More than 3-4 options

**Example - if is fine here:**

**Example - when is better (preview):**

---



```kotlin
when (dayOfWeek) {
    1 -> "Monday"
    2 -> "Tuesday"
    3 -> "Wednesday"
    // ... cleaner than many else ifs
}
```
