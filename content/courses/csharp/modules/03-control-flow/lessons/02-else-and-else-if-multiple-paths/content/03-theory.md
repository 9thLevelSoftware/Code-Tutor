---
type: "THEORY"
title: "How Conditional Branching Works"
---

## The Anatomy of if-else if-else

The complete conditional chain has three parts:

1. **`if`** — The first and required condition
2. **`else if`** — Optional additional conditions (you can have 0, 1, or many)
3. **`else`** — The final catch-all (optional but recommended)

## Execution Flow

When C# encounters an if-else if-else chain:

1. It evaluates the `if` condition first
2. If **true** — it executes that block and SKIPS everything else
3. If **false** — it moves to the first `else if`
4. This continues until a condition is true or all conditions are exhausted
5. If no conditions were true and there's an `else`, that block executes

## Key Rule: Order Matters!

Conditions are evaluated **top to bottom**. This means you should write your conditions from **most specific to least specific**:

```csharp
// BAD ORDER (wrong!)
if (score >= 60)      // Catches everything 60+
    Console.WriteLine("D");
else if (score >= 70) // Never reached!
    Console.WriteLine("C");
else if (score >= 80) // Never reached!
    Console.WriteLine("B");

// GOOD ORDER (correct!)
if (score >= 90)      // Most specific first
    Console.WriteLine("A");
else if (score >= 80) // Then less specific
    Console.WriteLine("B");
else if (score >= 70)
    Console.WriteLine("C");
else if (score >= 60)
    Console.WriteLine("D");
else                  // Least specific (catch-all)
    Console.WriteLine("F");
```

## Why Use else if Instead of Multiple ifs?

Compare these two approaches:

```csharp
// Multiple separate if statements (WRONG for mutually exclusive choices)
if (temperature > 90)
    Console.WriteLine("Hot");
if (temperature > 70)
    Console.WriteLine("Warm");  // Also prints!
if (temperature > 50)
    Console.WriteLine("Cool");  // Also prints!

// Using else if (CORRECT - only one message)
if (temperature > 90)
    Console.WriteLine("Hot");
else if (temperature > 70)
    Console.WriteLine("Warm");
else if (temperature > 50)
    Console.WriteLine("Cool");
else
    Console.WriteLine("Cold");
```

Use **else if** when choices are mutually exclusive (only one should happen).
