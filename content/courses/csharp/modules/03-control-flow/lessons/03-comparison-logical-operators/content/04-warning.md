---
type: "WARNING"
title: "Common Pitfalls to Avoid"
---

## ⚠️ Pitfall 1: Confusing Assignment (=) with Comparison (==)

The single equals sign `=` assigns a value; the double equals `==` compares values. This is the #1 source of bugs in conditional statements.

### The Mistake

```csharp
int x = 5;

// WRONG: This assigns 10 to x, then evaluates x (which is now 10, truthy)
if (x = 10)  
    Console.WriteLine("x is 10");  // Always runs, and x is now 10!

// CORRECT: Compare x to 10
if (x == 10)  
    Console.WriteLine("x equals 10");
```

### Why C# Helps (But You Still Need to Be Careful)

C# is smarter than some languages—it won't let you use assignment in an `if` condition unless the result is explicitly boolean:

```csharp
bool isReady = true;

// This won't compile in C# (compiler error)
if (isReady = false)  // Error: Cannot implicitly convert type 'bool' to...
    Console.WriteLine("Ready!");
```

However, this protection doesn't help when the assigned value is already a boolean or in other contexts.

### Prevention Strategy

**Yoda Conditions** (putting the constant first) make accidental assignment a compiler error:

```csharp
// If you accidentally use = here, it won't compile
if (10 == x)   // Can't assign to 10!
    Console.WriteLine("x is 10");

// vs
if (x = 10)    // Would compile (but fail logic)
    Console.WriteLine("x is 10");
```

While Yoda conditions work, modern C# compiler warnings catch most cases anyway. Focus on code reviews and tests.

## ⚠️ Pitfall 2: Using Single & or | Instead of && or ||

The single symbols `&` and `|` are **bitwise** operators, not logical operators. They don't short-circuit and can cause unexpected behavior.

### The Mistake

```csharp
// WRONG: Single & doesn't short-circuit - both sides always execute
if (user != null & user.Age > 18)  // CRASH if user is null!
{
    Console.WriteLine("Adult user");
}

// CORRECT: && short-circuits - safe null checking
if (user != null && user.Age > 18)  // Safe! Second check skipped if null
{
    Console.WriteLine("Adult user");
}
```

### When Single Symbols Are Actually Needed

Very rarely, you need both sides to execute (side effects):

```csharp
// Both methods must run for logging purposes
if (ValidateFormat(input) & ValidateContent(input))  // Both execute
{
    Console.WriteLine("All validations passed");
}
```

**Rule:** Use `&&` and `||` for logical operations unless you have a specific reason not to.

## ⚠️ Pitfall 3: Relying on Operator Precedence (Without Parentheses)

Mixing `&&` and `||` without parentheses creates code that's hard to read and prone to bugs.

### The Mistake

```csharp
// CONFUSING: What was the intent?
if (isAdmin || isModerator && isActive)
    Console.WriteLine("Access granted");

// Does it mean:
// (isAdmin || isModerator) && isActive  ?  -- Admin requires isActive too?
// isAdmin || (isModerator && isActive)  ?  -- Admin bypasses isActive check?
```

**Reality:** `&&` has higher precedence than `||`, so it evaluates as:
```csharp
if (isAdmin || (isModerator && isActive))
```

### The Fix

Always use parentheses when mixing operators:

```csharp
// CLEAR: Explicit intent, no guessing
if ((isAdmin || isModerator) && isActive)
    Console.WriteLine("Access granted (requires active)");

// Or if you want admin to bypass:
if (isAdmin || (isModerator && isActive))
    Console.WriteLine("Access granted (admin bypass)");
```

**Golden Rule:** Code is read 10x more than it's written. Clarity beats brevity.

## ⚠️ Pitfall 4: Comparing Floating-Point Numbers with ==

Floating-point arithmetic involves tiny rounding errors. Direct equality checks often fail unexpectedly.

### The Mistake

```csharp
double price = 0.1 + 0.2;
Console.WriteLine(price);  // 0.30000000000000004 (not exactly 0.3!)

// WRONG: May be false due to precision
if (price == 0.3)
    Console.WriteLine("Price is exactly 30 cents");
else
    Console.WriteLine("Price mismatch!");  // This runs!
```

### The Solution

Compare floating-point numbers within a small tolerance (epsilon):

```csharp
// CORRECT: Check if values are "close enough"
double epsilon = 0.0001;
if (Math.Abs(price - 0.3) < epsilon)
    Console.WriteLine("Price is approximately 30 cents");

// Or use a more generous tolerance for currency
decimal moneyValue = 0.1m + 0.2m;  // Use decimal for money!
if (moneyValue == 0.3m)  // Decimal is precise for base-10 numbers
    Console.WriteLine("Exact 30 cents");
```

### Best Practice for Financial Calculations

Use `decimal` for money—it's base-10 precise:

```csharp
decimal accountBalance = 100.00m;
decimal purchase = 49.99m;
decimal remaining = accountBalance - purchase;

if (remaining == 50.01m)  // True with decimal!
    Console.WriteLine("Balance updated correctly");
```

## ⚠️ Pitfall 5: Double Negatives (!!) and Confusing NOT Usage

Multiple negations or inverted variable names make code hard to reason about.

### The Mistake

```csharp
// CONFUSING: Double negative
bool isNotDisabled = true;
if (!isNotDisabled)
    EnableFeature();  // Wait... so if it's NOT not disabled?

// Also confusing: Negative variable names
bool isNotAvailable = false;
if (!isNotAvailable)
    ProcessRequest();  // Available? Not unavailable? Ugh.
```

### The Solution

Use positive variable names and single negations:

```csharp
// CLEAR: Positive naming
bool isEnabled = false;
if (!isEnabled)  // Clearly checking "if not enabled"
    ShowDisabledMessage();

// Even better: Rename variables to be positive
bool isAvailable = true;
if (isAvailable)
    ProcessRequest();
```

### Avoid NOT Overuse Pattern

```csharp
// AVOID: Triple negation nightmare
if (!(!user.IsInactive && !user.IsBlocked))
    AllowLogin();  // What does this even mean?

// BETTER: Decompose and use positive logic
bool canLogin = user.IsActive && !user.IsBlocked;
if (canLogin)
    AllowLogin();
```

## ⚠️ Pitfall 6: Null Comparison Confusion

Comparing nullable types and checking for null requires care.

### The Mistake

```csharp
string? name = null;

// Dangerous: may compile but cause issues
if (name == null)  // OK for reference types
    Console.WriteLine("No name");

// But with nullable value types:
int? age = null;
if (age == null)  // Works, but...
    Console.WriteLine("Age unknown");
```

### Modern C# Solution (C# 9+)

Use `is` and `is not` patterns for clearer null checking:

```csharp
string? input = null;

// Modern pattern - clear and expressive
if (input is null)
    Console.WriteLine("Input is null");

if (input is not null)
    Console.WriteLine($"Input: {input}");

// Works with nullable value types too
int? score = null;
if (score is null)
    Console.WriteLine("No score available");
```

## ⚠️ Pitfall 7: String Comparison Surprises

String comparison has several gotchas that can cause bugs.

### The Mistake

```csharp
string name1 = "John";
string name2 = "john";

// Case-sensitive comparison (likely not what you want)
if (name1 == name2)
    Console.WriteLine("Names match!");  // Never runs!

// Also surprising: culture-specific comparisons
string turkishI = "Istanbul";
string lowerI = "istanbul";
// In Turkish culture, 'I'.ToLower() is 'ı' (dotless i)
```

### The Solution

```csharp
// Case-insensitive comparison
if (name1.Equals(name2, StringComparison.OrdinalIgnoreCase))
    Console.WriteLine("Names match (case ignored)");

// Or use string.Compare with ignore case
if (string.Compare(name1, name2, StringComparison.OrdinalIgnoreCase) == 0)
    Console.WriteLine("Names match");

// For Turkish or culture-specific scenarios, specify culture
if (name1.Equals(name2, StringComparison.CurrentCultureIgnoreCase))
    Console.WriteLine("Culture-aware match");
```

## ⚠️ Pitfall 8: Chaining || with && Without Clear Grouping

Complex conditions without parentheses create ambiguity about intent.

### The Mistake

```csharp
// AMBIGUOUS: Which conditions are grouped?
if (isAdmin || isModerator && isActive || isSystemAccount)
    GrantAccess();

// Is it:
// (isAdmin || (isModerator && isActive)) || isSystemAccount ?
// ((isAdmin || isModerator) && isActive) || isSystemAccount ?
// isAdmin || (isModerator && (isActive || isSystemAccount)) ?
```

### The Solution

Break complex conditions into meaningful variables or use explicit grouping:

```csharp
// APPROACH 1: Break into logical steps
bool hasPrivilegedRole = isAdmin || isModerator;
bool hasValidAccess = hasPrivilegedRole && isActive;
bool canBypassChecks = isSystemAccount;

if (hasValidAccess || canBypassChecks)
    GrantAccess();

// APPROACH 2: Explicit parentheses with comments
if ((isAdmin || isModerator) && isActive || isSystemAccount)
//     ↑ privileged roles must be active  ↑ or system always
    GrantAccess();
```

## Summary Checklist

Before committing code with comparisons:

- [ ] Used `==` for comparison, not `=` for assignment
- [ ] Used `&&` and `||` (double symbols) for logic, not `&` and `|`
- [ ] Added parentheses when mixing `&&` and `||`
- [ ] Used epsilon comparison for floating-point numbers
- [ ] Avoided double negatives and confusing NOT chains
- [ ] Used `is null` / `is not null` for modern null checking
- [ ] Specified `StringComparison` for case-insensitive string checks
- [ ] Broke complex conditions into readable steps or comments