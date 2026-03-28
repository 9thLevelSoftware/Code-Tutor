---
type: "KEY_POINT"
title: "Key Takeaways"
---

**Type Narrowing Mastery:**

1. **Narrowing is control-flow analysis** - TypeScript tracks types through if/else, switch, and early returns

2. **Match the guard to the type:**
   - Primitives: typeof
   - Classes: instanceof
   - Object shapes: 'in' operator
   - Complex validation: custom type guards
   - Tagged objects: discriminated unions

3. **Discriminated unions are the power pattern:**
   - Add a 'type', 'kind', or 'status' property
   - Use literal types for the discriminant
   - Combine with exhaustiveness checking (never)

4. **Custom type guards are reusable safety:**
   - Return type: `param is Type`
   - Actually validate the structure inside
   - Compose guards for nested objects

5. **Watch out for JavaScript quirks:**
   - typeof null === 'object'
   - 0, '', and false are falsy
   - Interfaces don't exist at runtime

6. **Use exhaustiveness checking:**
   - `const _never: never = value` in default case
   - TypeScript errors if you forget a union member

7. **Early returns keep code clean:**
   - Handle invalid cases first and return
   - Remaining code has the narrowed type