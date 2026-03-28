---
type: "KEY_POINT"
title: "params Collections in C# 13"
---

## Key Takeaways

- **C# 13 expands `params` beyond arrays** -- `params` now works with `IEnumerable<T>`, `ReadOnlySpan<T>`, `IReadOnlyList<T>`, and more. Callers can pass individual items, collection expressions, or existing collections. This removes the historical limitation of array-only params parameters.

- **`params ReadOnlySpan<T>` avoids heap allocations** -- for performance-critical code, span-based params keep data on the stack. This is a zero-cost abstraction for small argument lists. Use this for high-throughput methods where allocations matter.

- **`params` must be the last parameter** -- `void Log(string level, params string[] messages)` is valid. The compiler collects all remaining arguments into the params collection. No other parameters can follow a params declaration.

- **Overload resolution prefers specific types** -- when multiple params overloads exist (array, span, IEnumerable), the compiler picks based on argument types. Span is preferred for performance, followed by arrays, then IEnumerable.

- **Collection expressions work with params** -- you can call `Log(["a", "b", "c"])` directly. The collection expression is passed without creating an intermediate array in many cases.

- **Existing code remains compatible** -- methods declared with `params T[]` continue to work exactly as before. Upgrading to newer collection types is opt-in through method signatures.
