---
type: "THEORY"
title: "Summary"
---


Congratulations! You've mastered conditional logic with if statements. Let's recap:

**Key Concepts:**
- **Conditional logic** lets programs make decisions based on conditions
- **If statements** execute code blocks when conditions are true
- **Comparison operators** (`==`, `!=`, `<`, `>`, `<=`, `>=`) create conditions
- **Else** provides an alternative path when the condition is false
- **Else if** chains multiple conditions (checked top to bottom)
- **Nested if** statements check conditions within conditions
- **Kotlin's if expression** can return values (unique feature!)

**Common Patterns:**

**Best Practices:**
- Always use `==` for comparison, not `=`
- Use braces `{ }` even for single statements
- Order else-if conditions from specific to general
- Use descriptive variable names for complex conditions
- Prefer positive conditions over negative when possible

---



```kotlin
// Simple if
if (condition) { /* code */ }

// If-else
if (condition) { /* code */ } else { /* code */ }

// If-else if chain
if (condition1) { /* code */ }
else if (condition2) { /* code */ }
else { /* code */ }

// If as expression
val result = if (condition) value1 else value2
```
