---
type: "THEORY"
title: "The Fundamentals: If Statements"
---


### Basic If Statement

The simplest form of conditional logic is the **if statement**:


**Output:**

**How it works:**
1. Program evaluates `temperature > 90` → `95 > 90` → `true`
2. Because the condition is true, the code inside the braces `{ }` executes
3. Program continues to the next line after the if statement

**If the temperature was 85:**

**Output:**

### Anatomy of an If Statement


**Parts:**
- `if` - Keyword that starts the conditional statement
- `(condition)` - A Boolean expression that evaluates to true or false
- `{ }` - Code block containing statements to execute when true
- Indentation - Makes the code readable (best practice: 4 spaces)

### Multiple Independent If Statements

You can have multiple separate if statements:


**Output:**

**Important:** Each if statement is checked independently. If `score = 85`, both the second and third conditions are true, so both messages print.

---



```kotlin
Great job!
Good effort!
```
