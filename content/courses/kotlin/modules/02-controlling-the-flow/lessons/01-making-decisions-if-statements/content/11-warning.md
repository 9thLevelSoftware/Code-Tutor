---
type: "WARNING"
title: "Common Pitfalls and Best Practices"
---


### Pitfall 1: Missing Braces

While braces are optional for single statements, **always use them** for clarity:

⚠️ **Risky (works but confusing):**

✅ **Better (clear and safe):**

### Pitfall 2: Semicolons After Conditions

❌ **WRONG:**

This creates an empty if statement, and the code block always executes!

✅ **CORRECT:**

### Pitfall 3: Comparing Floating-Point Numbers with ==

Floating-point arithmetic can be imprecise:

❌ **Risky:**

✅ **Better:**

### Best Practice 1: Readable Conditions

Use descriptive variable names and comments for complex conditions:

❌ **Unclear:**

✅ **Clear:**

### Best Practice 2: Positive Conditions

When possible, write conditions in positive form:

⚠️ **Harder to read:**

✅ **Easier to read:**

---



```kotlin
if (isValid) {
    // Do something
}
```
