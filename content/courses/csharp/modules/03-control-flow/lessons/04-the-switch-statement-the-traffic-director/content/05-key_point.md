---
type: "KEY_POINT"
title: "Switch Statements and Expressions"
---

# Key Takeaways for Using Switch Effectively

## Choose the Right Tool for the Job

- **Use switch expressions (C# 8+) for value mapping** — `var grade = score switch { >= 90 => "A", >= 80 => "B", _ => "F" };` is concise and returns a value directly. The `_` discard pattern is your default case, catching any values not explicitly handled.

- **Classic switch statements require `break`** — Forgetting `break` causes a compiler error in C# (unlike C/C++ where silent fall-through is allowed). Each `case` must end with `break`, `return`, `throw`, or `goto`.

## Modern Pattern Matching Power

- **Switch expressions support advanced patterns** — Combine relational patterns (`>= 90`), logical patterns (`or`, `and`, `not`), type patterns, and property patterns for expressive, readable branching.

- **The `when` clause extends pattern capabilities** — Add arbitrary conditions like `case int temp when temp > 90 && isWeekend` to create sophisticated matching logic.

## When to Use Switch vs If-Else

**Choose switch when:**
- Checking one variable against many specific values
- Mapping values to results (e.g., day number to day name)
- Handling enum values exhaustively (compiler verifies completeness)
- Code readability matters (switch is clearer than long if-else chains)

**Choose if-else when:**
- Checking ranges (like `score >= 80 && score < 90`)
- Complex conditions involving multiple variables
- Need to check conditions in a specific evaluation order

## Safety and Robustness

- **Always include a `default` or discard (`_`) case** — It handles unexpected values gracefully and makes your code more robust against edge cases and future changes.

- **Remember case sensitivity with strings** — `case "yes"` won't match `"YES"`. Use `.ToLower()` or `.ToUpper()` to normalize input before switching.

- **Order matters in switch expressions** — Place more specific patterns before general ones. The first matching pattern wins, so `>= 90` must come before `>= 60`.

## Type Support and Limitations

- **Switch works with:** `int`, `long`, `string`, `char`, `bool`, `enum` (and nullable versions) — but NOT `double` or `float` due to floating-point precision issues.

- **Use pattern matching for objects** — When working with custom types or `object` references, switch expressions with type patterns provide the flexibility you need.

## C# Version Awareness

| Feature | C# Version |
|---------|------------|
| Classic switch | 1.0 |
| Pattern matching with `when` | 7.0 |
| Switch expressions | 8.0 |
| Property patterns, tuple patterns | 8.0 |
| Relational patterns (`>`, `<`, `>=`) | 9.0 |
| Logical patterns (`and`, `or`, `not`) | 9.0 |
| List patterns | 11.0 |

**Best Practice:** In modern C# (9.0+), prefer switch expressions with pattern matching for cleaner, more maintainable code.
