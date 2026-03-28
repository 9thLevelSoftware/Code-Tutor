---
type: "THEORY"
title: "If Statements - Your First Decisions"
---

**Estimated Time**: 40 minutes
**Difficulty**: Beginner

## Introduction

Every program needs to make choices. Should we save this file? Is the password correct? Is the game over? The if statement is Python's primary decision-making tool—it executes code only when a condition is true.

**Learning Objectives**:
- Write if statements with boolean conditions
- Understand truthiness in Python (what counts as "true")
- Create simple decision trees with if-else
- Use comparison operators within conditions

## Basic If Statement

```python
temperature = 85

if temperature > 80:
    print("It's hot outside!")
    print("Consider turning on the AC.")

print("This always prints (not indented)")
```

## Truthiness in Python

These values are considered "falsy" (treated as False):
- `None`
- `0`, `0.0` (zero of any numeric type)
- `""` (empty string)
- `[]`, `{}`, `()` (empty collections)

Everything else is "truthy":

```python
name = input("Enter your name: ")

if name:  # True if name is not empty
    print(f"Hello, {name}!")
else:
    print("You didn't enter a name.")
```

## Comparison Operators

| Operator | Meaning | Example |
|----------|---------|---------|
| `==` | Equal to | `5 == 5` → `True` |
| `!=` | Not equal to | `5 != 3` → `True` |
| `<` | Less than | `3 < 5` → `True` |
| `>` | Greater than | `7 > 5` → `True` |
| `<=` | Less than or equal | `5 <= 5` → `True` |
| `>=` | Greater than or equal | `7 >= 5` → `True` |

## Real-World Context

If statements are the foundation of interactive software. Login systems check credentials, games check win conditions, e-commerce sites check inventory, and smart homes check sensor values. Mastering the if statement is your first step toward writing programs that respond to real-world conditions.

Make your programs smart with if statements! It has proper frontmatter so the loader will not fail to parse it.
