---
type: "THEORY"
title: "Understanding Operators Deeply"
---

## Comparison Operators Reference

| Operator | Meaning | Example | Result |
|----------|---------|---------|--------|
| `==` | Equal to | `5 == 5` | `true` |
| `!=` | Not equal to | `5 != 3` | `true` |
| `>` | Greater than | `10 > 5` | `true` |
| `<` | Less than | `3 < 7` | `true` |
| `>=` | Greater than or equal | `5 >= 5` | `true` |
| `<=` | Less than or equal | `4 <= 5` | `true` |

## Logical Operators Explained

### && (AND) - "Both Must Be True"

The `&&` operator returns `true` ONLY if **both** conditions are true:

```
true && true   = true
true && false  = false
false && true  = false
false && false = false
```

**Why use it?** When you need multiple requirements:
```csharp
// To ride alone: must be 16+ AND have a license
if (age >= 16 && hasLicense)
    Console.WriteLine("You can drive alone!");
```

### || (OR) - "At Least One Must Be True"

The `||` operator returns `true` if **either** condition is true:

```
true || true   = true
true || false  = true
false || true  = true
false || false = false
```

**Why use it?** When you accept multiple valid options:
```csharp
// Discount applies if senior OR student
if (age >= 65 || isStudent)
    Console.WriteLine("You get a discount!");
```

### ! (NOT) - "Flip the Value"

The `!` operator inverts a boolean:

```
!true  = false
!false = true
```

**Why use it?** To check the opposite of something:
```csharp
// Continue only if NOT cancelled
if (!isCancelled)
    StartGame();

// Same as: if (isCancelled == false)
```

## Short-Circuit Evaluation

This is a powerful optimization! C# stops evaluating as soon as it knows the answer:

### && Short-Circuits on First False
```csharp
// If user is null, the second check NEVER runs!
if (user != null && user.Age >= 18)
    Console.WriteLine("Valid user");
```
If `user` is `null`, the first part is `false`, so C# skips `user.Age` entirely (preventing a crash!).

### || Short-Circuits on First True
```csharp
// If it's weekend, we don't even check holiday!
if (isWeekend || isHoliday)
    Console.WriteLine("Day off!");
```

## Combining Operators

Use parentheses `()` to control order when mixing `&&` and `||`:

```csharp
// Without parentheses - confusing!
if (age > 18 && hasTicket || isVIP)
    
// With parentheses - clear intent!
if ((age > 18 && hasTicket) || isVIP)
```

**Rule of thumb:** When in doubt, add parentheses! Code clarity beats brevity.