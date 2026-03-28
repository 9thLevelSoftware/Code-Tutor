---
type: "THEORY"
title: "Understanding Operators Deeply"
---

## Comparison Operators Reference

Comparison operators evaluate the relationship between two values and return a boolean result (`true` or `false`). These are the building blocks of conditional logic.

| Operator | Meaning | Example | Result | Description |
|----------|---------|---------|--------|-------------|
| `==` | Equal to | `5 == 5` | `true` | Values are identical |
| `!=` | Not equal to | `5 != 3` | `true` | Values differ |
| `>` | Greater than | `10 > 5` | `true` | Left exceeds right |
| `<` | Less than | `3 < 7` | `true` | Left is smaller |
| `>=` | Greater than or equal | `5 >= 5` | `true` | Left meets or exceeds |
| `<=` | Less than or equal | `4 <= 5` | `true` | Left is at most right |

### Comparison with Strings

Strings are compared alphabetically using Unicode values:

```csharp
"apple" < "banana"   // true (a comes before b)
"Zebra" > "apple"    // false (uppercase Z < lowercase a)
"cat" == "cat"       // true
```

**Important:** String comparison is case-sensitive by default. `"Apple" != "apple"`.

## Logical Operators Explained

### && (AND) — "Both Must Be True"

The AND operator returns `true` ONLY if **both** conditions evaluate to `true`. If either condition is `false`, the entire expression is `false`.

#### Truth Table for &&

| Left | Right | Result | Explanation |
|------|-------|--------|-------------|
| `true` | `true` | `true` | Both satisfied |
| `true` | `false` | `false` | Right side fails |
| `false` | `true` | `false` | Left side fails |
| `false` | `false` | `false` | Both fail |

**Real-world analogy:** A security door requiring both a keycard AND a fingerprint. Missing either one keeps the door locked.

```csharp
// To access the admin panel: must be admin AND authenticated
if (userRole == "admin" && isAuthenticated)
    Console.WriteLine("Admin access granted");
```

### || (OR) — "At Least One Must Be True"

The OR operator returns `true` if **either** condition evaluates to `true`. It only returns `false` when BOTH conditions are `false`.

#### Truth Table for ||

| Left | Right | Result | Explanation |
|------|-------|--------|-------------|
| `true` | `true` | `true` | Both satisfied |
| `true` | `false` | `true` | Left side passes |
| `false` | `true` | `true` | Right side passes |
| `false` | `false` | `false` | Both fail |

**Real-world analogy:** An elevator accepting either a keycard OR a special passcode. Either credential grants access.

```csharp
// Discount applies if customer is a student OR a senior
if (isStudent || age >= 65)
    Console.WriteLine("Discount applied!");
```

### ! (NOT) — "Flip the Value"

The NOT operator is a unary operator that inverts a boolean value. It turns `true` into `false` and `false` into `true`.

#### Truth Table for !

| Input | Result |
|-------|--------|
| `true` | `false` |
| `false` | `true` |

**Real-world analogy:** A "Do Not Disturb" sign. If you are NOT disturbed, you can work peacefully.

```csharp
// Continue only if transaction is NOT cancelled
if (!isCancelled)
    ProcessTransaction();

// Equivalent but less readable: if (isCancelled == false)
```

## Operator Precedence

When multiple operators appear in a single expression, C# evaluates them in a specific order. Understanding precedence prevents bugs and reduces the need for excessive parentheses.

### Precedence Hierarchy (Highest to Lowest)

| Precedence | Operators | Description |
|------------|-----------|-------------|
| 1 | `()` | Parentheses (grouping) |
| 2 | `!` | Logical NOT |
| 3 | `==`, `!=` | Equality and inequality |
| 4 | `<`, `>`, `<=`, `>=` | Relational comparisons |
| 5 | `&&` | Logical AND |
| 6 | `\|\|` | Logical OR |

### Precedence in Action

```csharp
// Without parentheses - evaluated by precedence
bool result = true || false && false;  // true!

// Evaluation order:
// 1. && has higher precedence: false && false = false
// 2. || evaluates: true || false = true
```

**Best Practice:** When mixing `&&` and `||`, always use parentheses to make intent explicit:

```csharp
// Clear and unambiguous
bool canAccess = (isAdmin || isModerator) && isActive;
// vs the potentially confusing:
bool canAccess = isAdmin || isModerator && isActive;  // Admin bypasses isActive check!
```

### Complex Precedence Example

```csharp
int age = 25;
bool hasLicense = true;
bool isInsured = false;
bool isEmergencyVehicle = true;

// Without parentheses - relies on precedence
if (age >= 18 && hasLicense || isEmergencyVehicle && !isInsured)
{
    // Evaluates as: (age >= 18 && hasLicense) || (isEmergencyVehicle && !isInsured)
    // Result: (true && true) || (true && true) = true || true = true
}

// With parentheses - explicitly shows intent
if ((age >= 18 && hasLicense) || (isEmergencyVehicle && !isInsured))
{
    // Much clearer! Emergency vehicles bypass normal requirements
}
```

## Short-Circuit Evaluation

C# uses **short-circuit evaluation** for logical operators—a performance optimization that stops evaluating as soon as the final result is determined.

### && Short-Circuits on First False

When using `&&`, if the left side evaluates to `false`, the entire expression must be `false`, so C# skips the right side entirely.

```csharp
// Short-circuit prevents NullReferenceException!
if (user != null && user.Age >= 18)
{
    Console.WriteLine($"User is {user.Age} years old");
}

// If user is null:
// 1. user != null evaluates to false
// 2. Short-circuit! user.Age is NEVER accessed
// 3. Result is false, no crash occurs
```

**Practical use case:** Null checking before member access, database call prevention when cache hits.

### || Short-Circuits on First True

When using `||`, if the left side evaluates to `true`, the entire expression must be `true`, so C# skips the right side.

```csharp
// Check cache first, only query database if needed
if (TryGetFromCache(key, out var value) || TryGetFromDatabase(key, out value))
{
    Console.WriteLine($"Value: {value}");
}

// If TryGetFromCache returns true:
// 1. Value found in cache
// 2. Short-circuit! Database is never queried
// 3. Saves time and resources
```

**Practical use case:** Fallback chains, default value assignment, expensive operation avoidance.

### Non-Short-Circuit Alternatives

C# also provides `&` and `|` operators that **always evaluate both sides**. These are rarely used for logical operations but are available when needed:

```csharp
// Both sides ALWAYS execute (no short-circuit)
bool result = ExpensiveOperationA() & ExpensiveOperationB();

// Useful when both operations have side effects that must occur
```

**Warning:** Using single `&` or `|` instead of `&&` or `||` is a common source of null reference exceptions and performance issues.

## Combining Conditions Effectively

### Chaining Multiple && Operators

All conditions must be true. Evaluation stops at the first `false`.

```csharp
// Multi-factor authentication check
if (hasValidPassword && hasSecondFactor && !isAccountLocked && isWithinBusinessHours)
{
    GrantAccess();
}
```

**Optimization tip:** Order conditions by likelihood of failure. Put the condition most likely to be `false` first to trigger short-circuit early.

### Chaining Multiple || Operators

Any condition can be true. Evaluation stops at the first `true`.

```csharp
// Multiple ways to qualify for premium support
if (isEnterpriseCustomer || hasSupportContract || hasPriorityFlag || isVIP)
{
    RouteToPriorityQueue();
}
```

**Optimization tip:** Order conditions by likelihood of success. Put the condition most likely to be `true` first to trigger short-circuit early.

### Mixing && and || with Parentheses

Complex logic requires careful grouping. When in doubt, add parentheses.

```csharp
// Complex business rule: eligible for bonus if:
// (senior employee with good performance) OR (new hire during probation)
// AND not on performance improvement plan

bool eligibleForBonus = ((yearsOfService >= 5 && performanceRating >= 4.0) || 
                         (isNewHire && isInProbationPeriod)) 
                         && !isOnPerformancePlan;
```

## Pattern Matching with `is` (Modern C#)

### Type Pattern Matching

The `is` operator tests if an object matches a type, optionally declaring a variable:

```csharp
object data = "Hello, Pattern Matching!";

// Simple type check
if (data is string)
    Console.WriteLine("It's a string");

// Type check with variable declaration (C# 7+)
if (data is string message)
    Console.WriteLine($"Message length: {message.Length}");
```

### Constant Pattern Matching

Check if a value equals a constant:

```csharp
int status = 200;

if (status is 200)
    Console.WriteLine("Success!");

if (status is 404 or 500)
    Console.WriteLine("Error occurred");
```

### Relational Patterns (C# 9+)

Use comparison operators directly in patterns:

```csharp
int temperature = 75;

if (temperature is > 90)
    Console.WriteLine("Hot day!");
else if (temperature is >= 70 and <= 85)
    Console.WriteLine("Pleasant weather");
else if (temperature is < 32)
    Console.WriteLine("Freezing!");
```

### Logical Patterns (C# 9+)

Combine patterns with `and`, `or`, and `not`:

```csharp
// 'and' pattern - both must match
if (age is >= 13 and <= 19)
    Console.WriteLine("Teenager");

// 'or' pattern - either matches
if (day is "Saturday" or "Sunday")
    Console.WriteLine("Weekend!");

// 'not' pattern - negation
if (input is not null)
    ProcessInput(input);
```

### Complete Pattern Matching Example

```csharp
object value = 42;

switch (value)
{
    case int i when i > 0:
        Console.WriteLine($"Positive integer: {i}");
        break;
    case string s when s.Length > 0:
        Console.WriteLine($"Non-empty string: {s}");
        break;
    case null:
        Console.WriteLine("Null value");
        break;
    default:
        Console.WriteLine("Unknown type");
        break;
}
```

## Summary Tables

### Quick Reference: When to Use Each Operator

| Operator | Use When | Example |
|----------|----------|---------|
| `==` | Checking equality | `if (status == "active")` |
| `!=` | Checking difference | `if (error != null)` |
| `>`, `<` | Range boundaries | `if (age > 18)` |
| `>=`, `<=` | Inclusive ranges | `if (score >= 60)` |
| `&&` | All requirements must meet | `if (valid && authenticated)` |
| `\|\|` | Any alternative works | `if (admin \|\| moderator)` |
| `!` | Inverting condition | `if (!cancelled)` |
| `is` | Type checking | `if (obj is string s)` |

### Common Pitfalls to Remember

| Mistake | Correct Form |
|---------|--------------|
| `if (x = 5)` (assignment) | `if (x == 5)` (comparison) |
| `if (a & b)` (bitwise) | `if (a && b)` (logical) |
| `if (x == 5 && y == 10 \|\| z == 15)` | `if ((x == 5 && y == 10) \|\| z == 15)` |
| `if (price == 0.1 + 0.2)` | `if (Math.Abs(price - 0.3) < 0.0001)` |