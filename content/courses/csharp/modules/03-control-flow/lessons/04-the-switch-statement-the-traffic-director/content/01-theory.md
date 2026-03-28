---
type: "THEORY"
title: "The Switch Statement - The Traffic Director"
---

**Estimated Time**: 50 minutes
**Difficulty**: Beginner

## Introduction

The switch statement provides a clean, readable way to handle multiple discrete values. Unlike cascading if-else chains, switch clearly signals "pick one path based on this value"—making code easier to read and often more performant.

In this lesson, you'll learn:

1. **Basic Switch Syntax**: Matching values against cases
2. **Pattern Matching (C# 7+)**: Type patterns, property patterns, and when clauses
3. **Switch Expressions (C# 8+)**: Concise syntax for value-returning switches
4. **Best Practices**: When to prefer switch over if-else

## Classic Switch vs. Modern Alternatives

**Traditional switch statement:**
```csharp
switch (dayOfWeek)
{
    case DayOfWeek.Monday:
        Console.WriteLine("Start of work week");
        break;
    case DayOfWeek.Friday:
        Console.WriteLine("Almost weekend!");
        break;
    default:
        Console.WriteLine("Regular day");
        break;
}
```

**Modern switch expression:**
```csharp
var message = dayOfWeek switch
{
    DayOfWeek.Monday => "Start of work week",
    DayOfWeek.Friday => "Almost weekend!",
    _ => "Regular day"
};
```

## Pattern Matching Power

C# switch statements have evolved far beyond simple value matching:

```csharp
public decimal CalculateShipping(Address address, decimal weight) => address switch
{
    { Country: "USA", State: "AK" or "HI" } => weight * 2.5m,  // Alaska/Hawaii premium
    { Country: "USA" } => weight * 1.0m,
    { Country: var c } when IsEUCountry(c) => weight * 1.5m,
    _ => weight * 3.0m  // International
};
```

## Real-World Context

Switch statements appear everywhere in production code:
- **State machines**: Game logic, workflow engines, UI states
- **Parsing**: Interpreters, compilers, protocol handlers
- **Routing**: HTTP request routing, message dispatch
- **Business rules**: Pricing calculators, tax engines, approval workflows

The .NET compiler optimizes switch statements on integers and enums into jump tables—often faster than equivalent if-else chains. Modern pattern matching makes switch the right choice for complex decision trees.

## When to Use Switch

✅ **Use switch when:**
- Comparing one variable against many constant values
- Enum-based state machines
- Type-based dispatch (pattern matching)
- Return value mapping

❌ **Use if-else when:**
- Complex boolean conditions (ranges, compound logic)
- Checking multiple unrelated variables
- Conditions requiring side effects in evaluation

Switch isn't just a control structure—it's self-documenting code that tells readers exactly what to expect. It has proper frontmatter so the loader will not fail to parse it.
