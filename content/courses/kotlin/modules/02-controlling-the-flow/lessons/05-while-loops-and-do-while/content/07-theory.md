---
type: "THEORY"
title: "Infinite Loops and Guards"
---


### What is an Infinite Loop?

An infinite loop is a loop that never ends because its condition never becomes false:


**This will:**
- Run indefinitely
- Freeze your program
- Consume CPU and memory
- Require force-stopping

### Intentional Infinite Loops

Sometimes infinite loops are **intentional** and controlled with `break`:


This is safe because we have a guaranteed exit condition.

### Common Infinite Loop Mistakes

❌ **Mistake 1: Forgetting to update the condition**

❌ **Mistake 2: Wrong update direction**

❌ **Mistake 3: Condition that can't change**

### Infinite Loop Guards

Always ask yourself:
1. **Does my condition eventually become false?**
2. **Do I update the variables in the condition?**
3. **Is there a guaranteed exit (break)?**

✅ **Safe pattern:**

---



```kotlin
var attempts = 0
val maxAttempts = 1000  // Safety limit

while (condition && attempts < maxAttempts) {
    // Loop body
    attempts++
}

if (attempts >= maxAttempts) {
    println("Warning: Loop limit reached")
}
```
