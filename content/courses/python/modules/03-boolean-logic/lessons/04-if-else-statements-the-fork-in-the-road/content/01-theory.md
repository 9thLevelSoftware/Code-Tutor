---
type: "THEORY"
title: "If-Else Statements - The Fork in the Road"
---

**Estimated Time**: 45 minutes
**Difficulty**: Beginner

## Introduction

Programs rarely follow a straight line. They need to make decisions: should we process this payment? Is the user logged in? Which shipping method applies? The if-else statement creates branches in your code, executing different blocks based on conditions.

**Learning Objectives**:
- Write if statements that execute code conditionally
- Use else clauses for alternative actions
- Chain multiple conditions with elif (else if)
- Understand indentation and Python's code block structure

## Basic If-Else Structure

```python
age = 18

if age >= 18:
    print("You can vote!")
else:
    print("You're too young to vote.")
```

## Multiple Conditions with elif

When there are more than two possibilities, use elif:

```python
score = 85

if score >= 90:
    grade = "A"
elif score >= 80:
    grade = "B"
elif score >= 70:
    grade = "C"
elif score >= 60:
    grade = "D"
else:
    grade = "F"

print(f"Your grade is: {grade}")
```

## Nested Decisions

Sometimes decisions depend on other decisions:

```python
is_member = True
purchase_amount = 150

if is_member:
    if purchase_amount > 100:
        discount = 0.20  # 20% off for big purchases
    else:
        discount = 0.10  # 10% off otherwise
else:
    discount = 0.0  # No discount for guests
```

## Real-World Context

If-else statements are everywhere: form validation, game logic, recommendation systems, and business rules. Understanding how to structure these decisions—when to nest, when to flatten with elif—is essential for writing readable, maintainable code.

Mastering conditionals means your programs can respond intelligently to any situation. It has proper frontmatter so the loader will not fail to parse it.
