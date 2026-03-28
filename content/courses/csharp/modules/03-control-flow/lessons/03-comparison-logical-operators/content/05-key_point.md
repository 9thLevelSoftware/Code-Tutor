---
type: "KEY_POINT"
title: "Comparison, Logical, and Pattern Matching Operators"
---

## Core Concepts: The Foundation of Decision-Making

### Comparison Operators: Measuring Relationships

| Operator | Meaning | Memory Tip |
|----------|---------|------------|
| `==` | Equal to | "Are they the same?" |
| `!=` | Not equal to | "Are they different?" |
| `>` | Greater than | "Bigger number wins" |
| `<` | Less than | "Smaller number wins" |
| `>=` | Greater than or equal | "At least this much" |
| `<=` | Less than or equal | "At most this much" |

These operators are your measuring tools—they compare values and return `true` or `false`. Use them to check if a user meets age requirements, validate input ranges, or compare string values.

### Logical Operators: Combining Decisions

| Operator | Name | Rule | Real-World Analogy |
|----------|------|------|-------------------|
| `&&` | AND | **Both** must be `true` | Security door needing keycard AND fingerprint |
| `\|\|` | OR | **At least one** must be `true` | Elevator accepting keycard OR passcode |
| `!` | NOT | **Inverts** the value | "Do Not Disturb" sign |

**The Power of Short-Circuiting:**
- `&&` stops at the first `false` — subsequent checks never run
- `||` stops at the first `true` — subsequent checks never run

This isn't just an optimization—it's a safety feature:
```csharp
// Safe null checking: second part only runs if first is true
if (user != null && user.Age >= 18)  // No crash if user is null!
```

## Modern C# Pattern Matching (C# 9+)

The `is` operator has evolved far beyond simple type checking:

### Type Patterns with Declaration
```csharp
if (obj is string message)
    Console.WriteLine($"Length: {message.Length}");
```

### Relational Patterns
```csharp
if (temperature is > 90)
    Console.WriteLine("Heat warning!");

if (age is >= 13 and <= 19)
    Console.WriteLine("Teenager");
```

### Logical Patterns
```csharp
if (day is "Saturday" or "Sunday")
    Console.WriteLine("Weekend!");

if (input is not null)
    Process(input);
```

## Critical Rules to Remember

### 1. Assignment vs. Comparison
- `=` assigns a value: `x = 5` puts 5 into x
- `==` compares values: `x == 5` checks if x equals 5

### 2. Use Double Symbols for Logic
- `&&` and `||` short-circuit (stop early when result is known)
- `&` and `|` always evaluate both sides (bitwise, not logical)

### 3. Parentheses Are Your Friends
When mixing `&&` and `||`, always use parentheses to show intent:
```csharp
// Clear:
if ((isAdmin || isModerator) && isActive)

// Unclear (relies on precedence rules):
if (isAdmin || isModerator && isActive)
```

### 4. Floating-Point Comparisons
Never use `==` with `double` or `float`. Use tolerance-based comparison:
```csharp
// Wrong:
if (price == 0.3)  // Might fail due to precision

// Right:
if (Math.Abs(price - 0.3) < 0.0001)
```

## Quick Decision Flowchart

```
Need to check conditions?
    |
    ├─→ Single comparison? → Use ==, !=, >, <, >=, <=
    |
    ├─→ Multiple conditions ALL required? → Use &&
    |
    ├─→ Multiple conditions ANY acceptable? → Use ||
    |
    ├─→ Need to check type? → Use 'is'
    |
    ├─→ Need to negate? → Use !
    |
    └─→ Complex logic? → Use parentheses!
```

## Practice Checklist

Test your understanding with these scenarios:

- [ ] Write an `if` statement checking if a number is between 1 and 100
- [ ] Create a condition that allows access if user is "admin" OR has `isTrusted` flag
- [ ] Use pattern matching to check if an object is a positive integer
- [ ] Write a null-safe condition using `is not null`
- [ ] Convert a complex `&&`/`||` chain to use clear parentheses

## Common Use Cases

| Scenario | Operator Pattern |
|----------|-----------------|
| Validate age range | `if (age >= 18 && age <= 65)` |
| Check multiple roles | `if (role == "admin" \|\| role == "moderator")` |
| Ensure not null AND valid | `if (input is not null && input.Length > 0)` |
| Invert a permission check | `if (!isBlocked)` |
| Pattern match with range | `if (score is >= 90 and <= 100)` |

**Remember:** Every program is a series of decisions. Master these operators, and you master the art of making your code think.
