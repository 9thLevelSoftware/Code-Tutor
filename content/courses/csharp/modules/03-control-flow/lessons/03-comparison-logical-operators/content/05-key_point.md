---
type: "KEY_POINT"
title: "Comparison and Logical Operators"
---

## Key Takeaways

- **`==` checks equality, `=` assigns** -- this is the most common beginner mistake. `if (age == 18)` checks; `age = 18` overwrites. C# catches most accidental assignments.

- **`&&` (AND) requires both sides true; `||` (OR) requires at least one** -- `&&` is stricter. Both operators short-circuit: if the left side determines the result, the right side is never evaluated.

- **`!` (NOT) flips a boolean** -- `!isRaining` means "it is not raining." Use it to invert conditions. Avoid double negatives like `!isNotReady`.

- **Comparison operators work with numbers, strings, and more** -- You can compare strings alphabetically: `"apple" < "banana"` is true!

- **Short-circuit evaluation prevents crashes** -- `user != null && user.Name == "John"` is safe because if `user` is null, the second check never runs.

- **Use parentheses when mixing `&&` and `||`** -- `(a && b) || c` is clearer than `a && b || c`. Don't rely on operator precedence memorization.

- **Double vs single symbols** -- Always use `&&` and `||` (not `&` and `|`) for logical operations. The single versions don't short-circuit and can cause unexpected behavior.
