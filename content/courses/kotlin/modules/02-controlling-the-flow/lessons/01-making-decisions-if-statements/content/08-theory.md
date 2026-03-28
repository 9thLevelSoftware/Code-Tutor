---
type: "THEORY"
title: "Nested If Statements"
---


You can put if statements inside other if statements:


**Output:**

**How it works:**
1. Check outer condition (`age >= 16`) → true, enter outer block
2. Print "You are old enough to drive"
3. Check inner condition (`hasLicense`) → true, execute
4. Print "You can drive legally!"

**Nested if statement pattern:**

**Alternative:** In the next lesson, you'll learn about **logical operators** (`&&`, `||`) which often eliminate the need for nesting.

---



```kotlin
if (outerCondition) {
    // Outer block
    if (innerCondition) {
        // Inner block (only reached if BOTH conditions are true)
    }
}
```
