---
type: "ANALOGY"
title: "The Universal Adapter"
---

Imagine traveling to a foreign country where every electrical outlet is different. In one room, you need a three-prong adapter. In another, a two-prong round plug. In the bathroom, something else entirely. You end up carrying a bag full of different adapters, constantly fumbling to find the right one for each situation. This was the old way of initializing collections in C# – each collection type had its own syntax, its own rules, and its own quirks.

Arrays used curly braces with a verbose `new int[] { }` syntax. Lists required `new List<T> { }`. Spans demanded `stackalloc`. Immutable arrays, queues, stacks – each had its own special initialization ceremony. Developers had to remember which syntax worked where, and code reviews often caught mismatched initialization styles. It was mental overhead that added no value, like memorizing which adapter goes in which outlet.

C# 12 introduces collection expressions – the universal adapter for all collections. Just as a universal travel adapter lets you plug into any outlet with one simple device, collection expressions use a single, clean syntax that works everywhere: square brackets `[ ]`.

With collection expressions, you write `[1, 2, 3]` and let the context determine the collection type. Assign it to an `int[]`? You get an array. Assign it to a `List<int>`? You get a list. Assign it to a `Span<int>`? You get a span. The compiler handles the adapter selection automatically based on your declared type.

The magic doesn't stop there. The spread operator `..` lets you combine collections effortlessly. Need to merge two lists? `[..firstList, ..secondList]`. Prepending a single item? `[newItem, ..existingList]`. Appending? `[..existingList, newItem]`. This replaces cumbersome loops and temporary variables with clean, declarative syntax.

Empty collections are equally elegant: `int[] empty = [];` beats `Array.Empty<int>()` or `new List<int>()`. The syntax scales from simple to complex scenarios without changing – whether you're creating a three-item array or a thousand-item list with spread operations, the syntax remains `[ ]`.

This is more than convenience; it's cognitive load reduction. One syntax to learn. One pattern to remember. One universal adapter that works in every collection context. That's the power of C# 12 collection expressions.