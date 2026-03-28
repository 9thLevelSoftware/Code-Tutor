---
type: "KEY_POINT"
title: "GroupBy, SelectMany, and Join"
---

## Key Takeaways

- **`GroupBy` creates groups with a `.Key` property** -- `products.GroupBy(p => p.Category)` returns groups where each group is an `IGrouping<string, Product>`. Chain `.Select()` to aggregate each group.

- **`SelectMany` flattens nested collections** -- if each order has a list of items, `orders.SelectMany(o => o.Items)` extracts all items into one flat sequence. Essential for one-to-many relationships.

- **`Join` connects two collections by matching keys** -- similar to SQL INNER JOIN. Use `GroupJoin` for LEFT JOIN behavior where you need all items from the first collection even without matches.

## See Also

- **Prerequisites**: Filtering with `.Where()` (M09 L03) and transforming with `.Select()` (M09 L04) - master the basics before advanced operations
- **Foundation**: `IEnumerable<T>` deferred execution (M09 L02) applies to all LINQ methods - queries build plans, execution happens on consumption
- **Pattern**: Database joins (M12 L03 EF Core) use the same LINQ `Join()` syntax for SQL queries
- **Advanced**: Asynchronous streams (M10) extend deferred execution concepts for I/O-bound sequences
- **Real World**: `SelectMany` is commonly used with Entity Framework for navigating relationships without loading entire object graphs
