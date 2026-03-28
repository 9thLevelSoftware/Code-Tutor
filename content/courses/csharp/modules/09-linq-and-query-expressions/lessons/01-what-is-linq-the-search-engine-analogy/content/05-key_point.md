---
type: "KEY_POINT"
title: "LINQ Fundamentals"
---

## Key Takeaways

- **LINQ is lazy by default** -- `var result = list.Where(x => x > 5)` creates a query but does not execute it. Execution happens when you iterate (`foreach`) or materialize (`.ToList()`).

- **Lambda syntax: `x => expression`** -- `x` is the parameter (each item), `=>` means "goes to", and the expression returns a result. Read `x => x > 5` as "each x such that x is greater than 5."

- **`using System.Linq;` is required** -- without this namespace import, LINQ extension methods like `.Where()`, `.Select()`, and `.OrderBy()` are not available. .NET 6+ projects include it via implicit usings.

## See Also

- **Prerequisites**: Methods and lambda expressions (M06) are essential for understanding `x => x` syntax
- **Deep Dive**: `IEnumerable<T>` and deferred execution (M09 L02) explains the "lazy" behavior in detail
- **Related**: Entity Framework Core (M12) uses LINQ to query databases with the same syntax
- **Advanced**: Asynchronous streams (M10) connect to streaming large datasets with LINQ

## See Also

- **Prerequisites**: Methods and lambda expressions (M06) are essential for understanding `x => x` syntax
- **Deep Dive**: `IEnumerable<T>` and deferred execution (M09 L02) explains the "lazy" behavior in detail
- **Related**: Entity Framework Core (M12) uses LINQ to query databases with the same syntax
- **Advanced**: Asynchronous streams (M10) connect to streaming large datasets with LINQ

## See Also

- **Prerequisites**: Methods and lambda expressions (M06) are essential for understanding `x => x` syntax
- **Deep Dive**: `IEnumerable<T>` and deferred execution (M09 L02) explains the "lazy" behavior in detail
- **Related**: Entity Framework Core (M12) uses LINQ to query databases with the same syntax
- **Advanced**: Asynchronous streams (M10) connect to streaming large datasets with LINQ
