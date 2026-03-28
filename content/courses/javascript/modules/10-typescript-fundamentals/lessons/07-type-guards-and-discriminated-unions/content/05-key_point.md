---
type: "KEY_POINT"
title: "Key Takeaways"
---

Key points about type guards and discriminated unions:

1. **Type guards narrow types** - They tell TypeScript which specific type a value is within a code block.

2. **Use the right guard for the job**:
   - `typeof` for primitives
   - `Array.isArray()` for arrays
   - `in` operator for property existence
   - Custom guards for complex logic
   - Discriminated unions for related types

3. **Discriminated unions are powerful** - Add a `kind` property with literal types for type-safe switches.

4. **Custom type guards use `is`** - Return type `param is Type` enables TypeScript narrowing.

5. **Exhaustiveness checking** - Use `never` in default case to catch missing union members at compile time.

6. **Narrowing is block-scoped** - Type is narrowed only within the if/else/switch block.

7. **Combine with utility types** - Works great with `Exclude`, `Extract`, and other utility types.