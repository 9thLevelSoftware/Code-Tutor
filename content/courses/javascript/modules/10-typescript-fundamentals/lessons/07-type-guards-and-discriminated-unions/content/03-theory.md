---
type: "THEORY"
title: "The Logic of Narrowing"
---

Type Narrowing is the process where TypeScript uses your logical checks to "narrow down" a variable from a broader type to a more specific one.

### 1. `typeof` checking
Perfect for primitives like `string`, `number`, and `boolean`. Once you check `typeof x === "string"`, TypeScript treats `x` as a string for the remainder of that code block.

### 2. `instanceof` checking
Perfect for classes and objects. If you have `class Admin` and `class Guest`, using `if (user instanceof Admin)` narrows the type to `Admin`.

### 3. Discriminated Unions
This is the "Gold Standard" for complex data shapes.
*   **The Discriminant:** A common property found in all members of the union, usually named `type`, `kind`, or `status`.
*   **The Power:** When you check the value of the discriminant (e.g., `if (obj.type === 'circle')`), TypeScript can automatically figure out which interface the object belongs to.

### 4. Custom Type Guards (`value is type`)
Sometimes built-in checks aren't enough. You can create a function that returns a boolean, but tell TypeScript: "If this function returns true, it means the variable **is** this specific type." 
`function isBird(pet: any): pet is Bird { ... }`

### 5. Truthiness Narrowing
TypeScript even understands "falsy" checks.
`if (name) { ... }` — If `name` was `string | null`, inside the `if` block it must be a `string` because `null` is falsy.
