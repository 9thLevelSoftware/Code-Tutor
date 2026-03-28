---
type: "KEY_POINT"
title: "From-End Indexing Mastery"
---

## Key Takeaways

- **`^1` is the last element, `^2` is second-to-last** -- the `^` operator counts from the end. It is equivalent to `array[array.Length - n]` but much more readable. This syntax eliminates mental arithmetic when accessing elements from the end of collections.

- **C# 13 allows `^` in object initializers** -- you can now write `new int[3] { [^1] = 99 }` to set the last element during initialization. Previously this was a compiler error. This enables flexible initialization patterns when working with arrays of unknown or dynamic size.

- **The `Index` type is reusable** -- `Index last = ^1;` stores a from-end index in a variable. Use it with any collection that has a `Length` or `Count` property. This type integrates with `System.Range` for slicing operations.

- **Zero-indexing rules still apply** -- remember that `^0` is equivalent to `array.Length`, which is one past the last valid element. Accessing `array[^0]` will throw `IndexOutOfRangeException` just like `array[array.Length]` does.

- **Works with any indexable collection** -- arrays, `List<T>`, spans, and custom collections with indexers all support the `^` operator. This consistency makes the syntax universally applicable across your codebase.

- **Improves code clarity for boundary operations** -- when your logic naturally concerns the end of a collection (last N items, recent entries, trailing elements), `^` syntax makes the intent immediately clear to readers.
