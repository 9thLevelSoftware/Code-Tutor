---
type: "THEORY"
title: "Complete Type Narrowing Reference"
---

**All Narrowing Techniques in One Place:**

**1. typeof Guards (primitives):**
```typescript
if (typeof x === 'string') { /* x is string */ }
if (typeof x === 'number') { /* x is number */ }
if (typeof x === 'boolean') { /* x is boolean */ }
if (typeof x === 'function') { /* x is callable */ }
if (typeof x === 'undefined') { /* x is undefined */ }
```

**2. instanceof Guards (classes):**
```typescript
if (x instanceof Date) { /* x is Date */ }
if (x instanceof Error) { /* x is Error */ }
if (x instanceof MyClass) { /* x is MyClass */ }
```

**3. in Operator (property check):**
```typescript
if ('property' in obj) { /* obj has property */ }
if ('bark' in animal) { /* animal is Dog-like */ }
```

**4. Custom Type Guards (is keyword):**
```typescript
function isUser(x: unknown): x is User {
  return typeof x === 'object' && x !== null
    && 'name' in x && 'email' in x;
}
```

**5. Equality Narrowing:**
```typescript
if (x === 'specific') { /* x is 'specific' literal */ }
if (x === null) { /* x is null */ }
if (x !== undefined) { /* x is not undefined */ }
```

**6. Truthiness Narrowing:**
```typescript
if (x) { /* x is truthy (not null, undefined, 0, '', false) */ }
if (!x) { /* x is falsy */ }
```

**7. Discriminated Unions:**
```typescript
type Shape = { kind: 'circle'; r: number } | { kind: 'rect'; w: number; h: number };
if (shape.kind === 'circle') { /* shape is circle */ }
```

**8. Array Checks:**
```typescript
if (Array.isArray(x)) { /* x is an array */ }
```

**9. Assertion Functions (TypeScript 3.7+):**
```typescript
function assertIsString(x: unknown): asserts x is string {
  if (typeof x !== 'string') throw new Error('Not a string');
}
```

**Narrowing Scope Rules:**
- Narrowing applies within the block where the check happens
- After the block, type reverts to original
- Use early returns to keep narrowed type for rest of function
- Reassignment resets the narrowed type