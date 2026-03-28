---
type: "KEY_POINT"
title: "Foreach and Collection Selection"
---

## Key Takeaways

- **`foreach` is the cleanest way to iterate** — `foreach (var item in collection)` works with arrays, lists, dictionaries, and any `IEnumerable<T>`. No index management needed. The compiler generates efficient enumeration code behind the scenes, often more optimal than hand-written loops.

- **Do not modify a collection while iterating it** — adding or removing items during `foreach` throws `InvalidOperationException`. The enumeration becomes invalid when the underlying collection changes. Collect changes in a separate list, then apply them after the loop completes. For scenarios requiring modification during iteration, use a traditional `for` loop iterating backwards.

- **Choose the right collection for the job** — `List<T>` for ordered items requiring index access, `Dictionary<TKey, TValue>` for O(1) key-based lookups, `HashSet<T>` for unique value collections and fast membership testing, arrays for fixed-size performance-critical scenarios, and `Queue<T>`/`Stack<T>` for FIFO/LIFO patterns.

- **Prefer `var` in foreach declarations** — `foreach (var item in collection)` reduces verbosity and handles anonymous types gracefully. The compiler infers the correct type from the collection's generic argument. Explicit type declarations are only necessary when you need a specific interface or base class.

- **Empty collections are safe** — iterating over an empty collection simply executes zero iterations with no exception. This eliminates defensive null checks and empty checks in many scenarios. Initialize collections to empty rather than null to leverage this safety.

- **`break` and `continue` work in foreach** — use `break` to exit early when a condition is met (like finding an item), `continue` to skip processing for specific items. These control flow statements behave identically to their behavior in `for` and `while` loops.
