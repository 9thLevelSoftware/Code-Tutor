---
type: "THEORY"
title: "Else If: Multiple Conditions"
---


What if you have more than two possibilities? Use **else if** to chain conditions:


**Output:**

### How Else If Works

The program checks conditions **in order** from top to bottom:

1. Check first condition (`score >= 90`) → `85 >= 90` → **false**, skip
2. Check second condition (`score >= 80`) → `85 >= 80` → **true**, execute and **STOP**
3. Don't check any remaining conditions

**Critical:** Once a condition is true, the rest are ignored. Order matters!

**Example showing order importance:**

❌ **WRONG ORDER:**
**Output:** `Grade: D` (Wrong! Should be A)

✅ **CORRECT ORDER:**
**Output:** `Grade: A` (Correct!)

**Rule:** Put the most specific conditions first, most general conditions last.

---



```kotlin
val score = 95

if (score >= 90) {
    println("Grade: A")  // This executes!
} else if (score >= 60) {
    println("Grade: D")  // Never reached (but that's okay)
}
```
