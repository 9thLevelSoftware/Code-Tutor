---
type: "WARNING"
title: "Common Pitfalls to Avoid"
---

## ⚠️ Pitfall 1: Single & or | Instead of && or ||

In C#, use **DOUBLE** symbols (`&&` and `||`) for logical operators:

```csharp
// WRONG (single symbols don't short-circuit)
if (obj != null & obj.Value > 0)  // & is bitwise, not logical

// CORRECT
if (obj != null && obj.Value > 0)  // && is proper logical AND
```

**Why it matters:** `&&` and `||` short-circuit (stop early), but `&` and `|` don't. Single versions evaluate BOTH sides, which can cause crashes or extra work.

## ⚠️ Pitfall 2: Forgetting == vs = 

```csharp
int x = 5;

// WRONG (this is assignment, not comparison)
if (x = 10)  // Sets x to 10, always true!
    Console.WriteLine("x is 10");

// CORRECT
if (x == 10)  // Compares x to 10
    Console.WriteLine("x is 10");
```

C# actually prevents this in most cases (unlike JavaScript), but be careful with the distinction!

## ⚠️ Pitfall 3: Relying on Operator Precedence

```csharp
// CONFUSING - relies on precedence rules
if (age > 18 && hasTicket || isVIP)  // Which comes first?

// CLEAR - use parentheses!
if ((age > 18 && hasTicket) || isVIP)
```

**Operator precedence:** `&&` has higher precedence than `||`, but don't rely on memorizing this. Use parentheses!

## ⚠️ Pitfall 4: Checking Equality on Floating Point

```csharp
double price = 0.1 + 0.2;  // Actually 0.30000000000000004!

// WRONG (might be false due to precision)
if (price == 0.3)

// CORRECT (check within tolerance)
if (Math.Abs(price - 0.3) < 0.0001)
```

## ⚠️ Pitfall 5: NOT Overuse (!!)

```csharp
// CONFUSING - double negative
if (!isNotReady)
    StartTask();

// BETTER - rename the variable
bool isReady = true;
if (isReady)
    StartTask();
```

Avoid double negatives! They're hard to reason about. Rename variables to be positive instead.

## ⚠️ Pitfall 6: Chaining || with && Without Parentheses

```csharp
// AMBIGUOUS - what was the intent?
if (isAdmin || isModerator && isActive)

// Does it mean:
// (isAdmin || isModerator) && isActive  ?
// or:
// isAdmin || (isModerator && isActive)  ?
```

**Always use parentheses** when mixing `&&` and `||`!