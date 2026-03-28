---
type: "KEY_POINT"
title: "Filtering with Where"
---

## Key Takeaways

- **`.Where()` keeps items that match a condition** -- `products.Where(p => p.Price > 10)` returns only products over $10. The original collection is not modified.

- **Combine conditions with `&&` and `||`** -- `p => p.Price > 10 && p.InStock` filters on multiple criteria in a single lambda. Keep complex conditions readable.

- **Chain `.Where()` calls for readability** -- `products.Where(p => p.InStock).Where(p => p.Price < 100)` is equivalent to combining conditions but can be easier to read when filters are independent.

## See Also

- **Prerequisites**: Lambda expressions (M06) are essential for writing `x => x > 5` filter conditions
- **Foundation**: `IEnumerable<T>` and deferred execution (M09 L02) explains how `.Where()` builds a query plan that executes later
- **Next Step**: `.Select()` transformation (M09 L04) - combine filtering with projection to shape your results
- **Related**: Entity Framework Core (M12 L03) uses `.Where()` to filter database queries with the same LINQ syntax
- **Pattern**: Method chaining connects to fluent interfaces (M07) - each LINQ method returns a new queryable sequence
