---
type: "THEORY"
title: "How Switch Works"
---

# Understanding Switch Statements and Expressions

## Classic Switch Statement Syntax

The traditional switch statement evaluates a single expression and matches it against multiple constant values:

```csharp
switch (expression)
{
    case constant1:
        // Code executes when expression == constant1
        break;
    case constant2:
    case constant3:
        // Both constant2 and constant3 execute this code
        break;
    default:
        // Code executes when no cases match
        break;
}
```

## Key Components Explained

### The Switch Expression
The expression inside `switch()` must be of a type that supports equality comparison:

**Fully supported types:**
- Integral types: `int`, `long`, `byte`, `short`, `char`
- String: `string` (case-sensitive by default)
- Boolean: `bool`
- Enumeration: `enum` types
- Nullable versions of all above

**Not supported (classic switch):**
- Floating-point: `double`, `float` (precision issues)
- Custom objects (without pattern matching)

### Case Labels
Each `case value:` defines a potential match with these requirements:
- Values must be **compile-time constants** (known when code compiles)
- Each value must be **unique** within the switch
- Multiple case labels can share the same code block

### Exit Statements
Every case must end with an exit statement. In C#, **implicit fall-through is NOT allowed**:

- `break` — exits the switch statement (most common)
- `return` — exits the entire method
- `throw` — throws an exception
- `goto case X` — explicitly jumps to another case

## Switch Expression Syntax (C# 8.0+)

Switch expressions provide a concise, functional syntax for value mapping:

```csharp
var result = value switch
{
    pattern1 => expression1,
    pattern2 => expression2,
    _ => defaultExpression
};
```

### Benefits of Switch Expressions

1. **Concise syntax** — No `case`, `break`, or curly braces required
2. **Returns a value** — Perfect for variable assignments and expressions
3. **Pattern matching** — Supports relational and type patterns
4. **Exhaustiveness checking** — Compiler warns about missing cases

## Pattern Matching Capabilities

### Relational Patterns (C# 9.0+)

Compare values using relational operators directly in patterns:

```csharp
string grade = score switch
{
    >= 90 => "A",
    >= 80 => "B",
    >= 70 => "C",
    _ => "F"
};
```

### Logical Patterns (C# 9.0+)

Combine patterns using `and`, `or`, and `not`:

```csharp
string description = number switch
{
    < 0 => "Negative",
    0 => "Zero",
    > 0 and < 100 => "Small positive",
    >= 100 and < 1000 => "Medium",
    >= 1000 => "Large"
};
```

### Type Patterns (C# 8.0+)

Match based on runtime type:

```csharp
string result = obj switch
{
    int i => $"Integer: {i}",
    string s => $"String of length {s.Length}",
    List<int> list => $"List with {list.Count} items",
    null => "Null reference",
    _ => "Unknown type"
};
```

### Property Patterns (C# 8.0+)

Match based on object properties:

```csharp
string category = customer switch
{
    { IsPremium: true, YearsActive: > 5 } => "VIP Customer",
    { IsPremium: true } => "Premium Customer",
    { YearsActive: > 2 } => "Regular Customer",
    _ => "New Customer"
};
```

### When Clauses (C# 7.0+)

Add arbitrary conditions to patterns:

```csharp
case int temp when temp > 100 && isHoliday:
    Console.WriteLine("Extreme heat during holiday!");
    break;
```

## Exhaustiveness Checking

The C# compiler can verify that switch expressions handle all possible values:

### Enum Exhaustiveness

```csharp
enum Color { Red, Green, Blue }

// Compiler error if any enum value is missing
string hex = color switch
{
    Color.Red => "#FF0000",
    Color.Green => "#00FF00",
    Color.Blue => "#0000FF"
};
```

### Boolean Exhaustiveness

```csharp
string text = booleanValue switch
{
    true => "Yes",
    false => "No"
};
```

To make a switch non-exhaustive (intentionally leaving cases unhandled), add a discard pattern `_` as a catch-all.

## Performance Considerations

### Compiler Optimizations

The C# compiler optimizes switch statements based on the switch type:

**Jump Table (O(1)):**
- Used for dense integer ranges
- Direct array lookup by value
- Constant time regardless of case count

**Hash Table (O(1) average):**
- Used for strings and sparse integers
- Hash-based lookup
- Nearly constant time

**Sequential Search (O(n)):**
- Fallback for complex patterns
- Checks each case in order
- Time grows linearly with case count

### Best Practices for Performance

1. **Prefer switch over if-else chains** when checking one variable against many values
2. **Use integers or enums** for best performance (jump table optimization)
3. **Group string cases** with the same value together (compiler may optimize)
4. **Consider switch expressions** — they enable additional compiler optimizations

## When to Use What

| Use Classic Switch When... | Use Switch Expression When... |
|---------------------------|------------------------------|
| Multiple lines of code per case | Simple value-to-value mapping |
| Side effects (console output) | Assigning a result to a variable |
| Complex control flow needed | Pattern matching with ranges |
| Working with C# 7.0 or earlier | Modern C# 8+ codebase |
| Need goto case statements | Exhaustiveness checking desired |

## C# Version Feature Summary

| Feature | Version | Description |
|---------|---------|-------------|
| Classic switch | 1.0 | Basic value matching |
| Pattern matching with `when` | 7.0 | Conditional case guards |
| Switch expressions | 8.0 | Concise functional syntax |
| Property patterns | 8.0 | Match object properties |
| Tuple patterns | 8.0 | Switch on multiple values |
| Relational patterns | 9.0 | `>`, `<`, `>=`, `<=` in patterns |
| Logical patterns | 9.0 | `and`, `or`, `not` combinators |
| List patterns | 11.0 | Match list/array elements |
