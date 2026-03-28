---
type: "WARNING"
title: "Safe Access Pitfalls"
---

### 1. You can't use `?.` for Assignment
You can use optional chaining to **read** data, but not to **write** it.
```javascript
user?.profile?.name = "Alice"; // SYNTAX ERROR
```
If you want to assign a value, you must first ensure the path exists.

### 2. Overusing `?.` (The "Silent Bug")
While `?.` prevents crashes, it can also hide real bugs. If you use it everywhere, you might not realize that a critical piece of data (like a user ID) is missing until much later in your code when something else fails mysteriously.
*   **Rule:** Only use `?.` when you expect that a value might genuinely be missing.

### 3. Mixing `??` with `&&` or `||`
For safety, JavaScript doesn't allow you to mix `??` with `&&` or `||` without using parentheses to show exactly what you mean.
```javascript
let x = a && b ?? c; // SYNTAX ERROR
let x = (a && b) ?? c; // OK
```

### 4. Optional Chaining is Not a Solution for Everything
`?.` only protects against `null` or `undefined`. If a variable exists but is of the wrong type (e.g., you try to call a string as a function `name?()`), your code will still crash.
