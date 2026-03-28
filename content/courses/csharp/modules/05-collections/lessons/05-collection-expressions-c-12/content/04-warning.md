---
type: "WARNING"
title: "Common Pitfalls"
---

## Watch Out For These!

**var doesn't work directly**: Collection expressions require an explicit target type. `var x = [1, 2, 3];` won't compile! Always specify the type: `int[] x = [1, 2, 3];` or `List<int> x = [1, 2, 3];`

**Don't confuse [ ] with { }**: Collection expressions use SQUARE brackets [ ]. The old initializer syntax uses curly braces { }. They're different! [1, 2, 3] is C# 12+, {1, 2, 3} is older syntax.

**Spread element requires two dots**: To spread a collection, use two dots: [..array]. A single dot or no dots treats it as a single element, not a spread.

**Performance consideration**: Collection expressions are highly optimized, especially for immutable collections like ImmutableArray<T>. They often generate better code than traditional initializers!

**Requires C# 12 / .NET 9 or later**: Collection expressions are a C# 12 language feature. This course uses .NET 9, which fully supports them. If working on older projects targeting .NET 7 or earlier, you'll need to upgrade to use this syntax.