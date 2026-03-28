---
type: "THEORY"
title: "If as an Expression (Kotlin Special Feature!)"
---


Here's something unique to Kotlin: `if` is not just a statement, it's an **expression** that can return a value!

**Traditional approach (statement):**

**Kotlin's expression approach:**

Both do the same thing, but the expression form is cleaner and more concise!

### More Expression Examples

**Example 1: Max of two numbers**

**Example 2: Fee calculation**

**Example 3: Multi-line expression blocks**

**Important:** When using if as an expression, you **must** have an else clause (the expression must always produce a value).

---



```kotlin
val result = if (score >= 60) {
    val bonus = 10
    score + bonus  // Last expression is returned
} else {
    score  // Last expression is returned
}
```
