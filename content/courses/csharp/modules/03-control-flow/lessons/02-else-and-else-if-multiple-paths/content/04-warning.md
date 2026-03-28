---
type: "WARNING"
title: "Common Pitfalls to Avoid"
---

## ⚠️ Pitfall 1: Wrong Order of Conditions

This is the #1 mistake beginners make! Remember: first match wins.

```csharp
// WRONG - 85 prints "C" instead of "B"
int score = 85;
if (score >= 70)
    Console.WriteLine("C");    // This runs first!
else if (score >= 80)
    Console.WriteLine("B");    // Never reached
else if (score >= 90)
    Console.WriteLine("A");    // Never reached
```

**Fix:** Arrange from highest to lowest (or most specific to least specific).

## ⚠️ Pitfall 2: Using = Instead of ==

```csharp
int x = 5;

// WRONG - assigns 10 to x, always true
if (x = 10)
    Console.WriteLine("x is 10");

// CORRECT - compares x to 10
if (x == 10)
    Console.WriteLine("x is 10");
```

C# actually prevents this in most cases (unlike some languages), but it's still worth knowing!

## ⚠️ Pitfall 3: Forgetting the else

```csharp
// Missing the "unknown" case
if (day == "Saturday" || day == "Sunday")
    Console.WriteLine("Weekend!");
else if (day == "Friday")
    Console.WriteLine("Almost weekend!");
// What about Monday-Thursday? Nothing prints!
```

**Best Practice:** Always consider adding an `else` to catch unexpected cases, even if just for error reporting.

## ⚠️ Pitfall 4: Overlapping Ranges

```csharp
// Confusing: what happens at exactly 18?
int age = 18;
if (age < 18)
    Console.WriteLine("Minor");
else if (age > 18)
    Console.WriteLine("Adult");
// At exactly 18, nothing prints!
```

**Fix:** Use `<=` or `>=` to be explicit about boundaries:
```csharp
if (age < 18)
    Console.WriteLine("Minor");
else
    Console.WriteLine("Adult");  // Catches 18 and up
```

## ⚠️ Pitfall 5: Too Many else ifs

If you have more than 5-6 else if conditions, consider using a `switch` statement instead. It's cleaner and easier to maintain!
