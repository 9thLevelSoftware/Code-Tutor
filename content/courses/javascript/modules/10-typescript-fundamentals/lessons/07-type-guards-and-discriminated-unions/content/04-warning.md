---
type: "WARNING"
title: "Narrowing Pitfalls"
---

### 1. The "Type Assertion" Trap
You might be tempted to use the `as` keyword to force a type: `const name = x as string;`.
*   **Danger:** This overrides TypeScript's safety checks. If `x` is actually a number, your app will crash at runtime, and TypeScript won't warn you.
*   **Rule:** Always prefer Type Guards (checking) over Type Assertions (forcing).

### 2. Missing Cases in Unions
If you have a union of 3 types but your `if/else` only checks for 2, you might have a bug. 
*   **Advanced Fix:** Many developers use an `exhaustive check` at the end of their logic to ensure that if a new type is added to the union later, the code will fail to compile until the new case is handled.

### 3. Truthiness and Numbers
Be careful with `if (variable)`. If the variable is a number and its value is `0`, the check will be false! 
*   **Fix:** If you are checking for numbers, be explicit: `if (variable !== undefined)`.

### 4. Discriminant Typos
If you misspell your discriminant (e.g., `status: "sucess"` instead of `"success"`), the type guard will fail silently and your code won't run. TypeScript usually catches this if you define your interfaces strictly.
