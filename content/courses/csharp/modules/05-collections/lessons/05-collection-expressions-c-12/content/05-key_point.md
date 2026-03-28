---
type: "KEY_POINT"
title: "Collection Expressions Mastery"
---

## Key Takeaways

- **`[1, 2, 3]` replaces verbose initialization** -- collection expressions (C# 12) work for arrays, lists, spans, and more. The target type determines what gets created. This single syntax eliminates the need to remember different initialization patterns for each collection type.

- **The spread element `..` combines collections** -- `[..existing, newItem]` expands `existing` inline. Use it to concatenate or prepend without manual loops or temporary variables. This operator is particularly powerful for merging configurations or building dynamic datasets.

- **Empty collections use `[]`** -- cleaner than `new List<int>()` or `Array.Empty<int>()`. Assign to the appropriate type: `List<int> items = [];`. The compiler generates optimal code for empty collections.

- **Target-typed expressions reduce verbosity** -- write `int[] numbers = [1, 2, 3];` instead of `new int[] { 1, 2, 3 }`. The compiler infers the collection type from the variable declaration, making code more readable while maintaining type safety.

- **Performance optimizations are automatic** -- collection expressions often generate better IL than traditional initializers. For immutable collections, the compiler can share instances when possible. Span-based scenarios avoid heap allocations entirely.

- **Compatibility requires C# 12 / .NET 9** -- collection expressions are a modern language feature. Ensure your project targets .NET 9 or higher to use this syntax. The compiler will provide clear error messages if you're targeting an older framework.
