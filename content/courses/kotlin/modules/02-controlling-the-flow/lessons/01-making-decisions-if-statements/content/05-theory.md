---
type: "THEORY"
title: "Comparison Operators"
---


To create conditions, you need to compare values using **comparison operators**:

| Operator | Meaning | Example | Result |
|----------|---------|---------|--------|
| `==` | Equal to | `5 == 5` | `true` |
| `==` | Equal to | `5 == 3` | `false` |
| `!=` | Not equal to | `5 != 3` | `true` |
| `!=` | Not equal to | `5 != 5` | `false` |
| `<` | Less than | `3 < 5` | `true` |
| `<` | Less than | `5 < 3` | `false` |
| `>` | Greater than | `5 > 3` | `true` |
| `<=` | Less than or equal | `5 <= 5` | `true` |
| `>=` | Greater than or equal | `5 >= 3` | `true` |

### Common Comparison Examples

**Numeric comparisons:**

**String comparisons:**

**Boolean comparisons:**

### Critical Mistake: = vs ==

**The #1 beginner mistake:**

❌ **WRONG:**

✅ **CORRECT:**

**Remember:**
- `=` is for **assignment** (storing a value)
- `==` is for **comparison** (checking equality)

---



```kotlin
if (age == 18) {  // This COMPARES age to 18
    println("You are 18")
}
```
