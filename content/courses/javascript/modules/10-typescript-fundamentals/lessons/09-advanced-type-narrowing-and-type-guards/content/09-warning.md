---
type: "WARNING"
title: "Common Narrowing Mistakes"
---

**1. typeof null === 'object' (JavaScript quirk)**:
```typescript
function process(val: object | null) {
  if (typeof val === 'object') {
    // val could STILL be null! typeof null === 'object'
  }
  
  // Fix: check null explicitly first
  if (val !== null && typeof val === 'object') {
    // Now val is definitely object
  }
}
```

**2. Truthiness issues with 0 and ''**:
```typescript
function count(n: number | null) {
  if (n) {  // WRONG: 0 is falsy but valid!
    return n * 2;
  }
}

// Fix: use explicit null check
function countFixed(n: number | null) {
  if (n !== null) {  // 0 passes this correctly
    return n * 2;
  }
}
```

**3. Type predicate that doesn't actually validate**:
```typescript
// DANGEROUS: Always returns true, doesn't validate!
function isUser(x: unknown): x is User {
  return true;  // Lies to TypeScript!
}

// Fix: Actually validate the structure
function isUserSafe(x: unknown): x is User {
  return typeof x === 'object' && x !== null
    && 'name' in x && typeof (x as any).name === 'string';
}
```

**4. Forgetting exhaustiveness checks**:
```typescript
type Status = 'pending' | 'active' | 'done';

function handle(s: Status) {
  if (s === 'pending') return;
  if (s === 'active') return;
  // Forgot 'done'! No error by default
}

// Fix: add exhaustiveness check
function handleFixed(s: Status) {
  switch (s) {
    case 'pending': return;
    case 'active': return;
    case 'done': return;
    default:
      const _never: never = s;  // Error if case missing!
  }
}
```

**5. instanceof doesn't work with interfaces**:
```typescript
interface User { name: string; }
const user: User = { name: 'Alice' };

// ERROR: User only exists at compile-time!
if (user instanceof User) { }  // 'User' only refers to a type

// Fix: use 'in' or custom type guard
if ('name' in user) { }
```

**6. Narrowing doesn't persist after function calls**:
```typescript
let value: string | null = 'hello';

if (value !== null) {
  console.log(value.length);  // OK: value is string
  someFunction();  // TypeScript might assume this could change value
  // value is still string | null here in some cases
}
```