---
type: "THEORY"
title: "The Logic of Safety"
---

Modern JavaScript (ES2020) introduced these two operators to handle the common "Cannot read property of undefined" errors that haunted developers for decades.

### 1. Optional Chaining (`?.`)
The `?.` operator stops the evaluation if the value before it is `null` or `undefined`.
*   **Deep Access:** `a?.b?.c?.d` — If any part is missing, the whole thing returns `undefined`.
*   **Function Calls:** `obj.method?.()` — Only runs the method if it actually exists.
*   **Array Access:** `arr?.[0]` — Safely checks the array before accessing an index.

### 2. Nullish Coalescing (`??`)
The `??` operator is a logical operator that returns its right-hand side operand when its left-hand side operand is `null` or `undefined`.

**Why not just use `||`?**
JavaScript has a concept of "Falsy" values: `false`, `0`, `""`, `null`, `undefined`, and `NaN`.
*   The `||` operator returns the backup for **ANY** of those falsy values.
*   The `??` operator **ONLY** returns the backup for `null` and `undefined`.
This makes `??` the perfect choice for setting default values when `0` or `""` are actually valid inputs.

### 3. Short-Circuiting
Like other logical operators, `?.` and `??` short-circuit. If the condition is met (or failed), the rest of the line is never even looked at by the computer.
