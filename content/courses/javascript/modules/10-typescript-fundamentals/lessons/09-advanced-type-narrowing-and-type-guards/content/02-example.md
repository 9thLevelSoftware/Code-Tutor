---
type: "EXAMPLE"
title: "typeof Guards - Narrowing Primitive Types"
---

The typeof operator is your first tool for narrowing primitives. TypeScript understands control flow and narrows types automatically after typeof checks:

```typescript
// TYPEOF RETURNS THESE VALUES:
// 'string', 'number', 'bigint', 'boolean', 'symbol', 'undefined', 'object', 'function'

// Basic typeof narrowing
function processValue(value: string | number | boolean): string {
  // Here, value could be any of the three types
  console.log('Type at start:', typeof value);
  
  if (typeof value === 'string') {
    // TypeScript KNOWS value is string here!
    // All string methods available
    return value.toUpperCase().trim();
  }
  
  if (typeof value === 'number') {
    // TypeScript KNOWS value is number here!
    // All number methods available
    return value.toFixed(2);
  }
  
  // Only boolean remains - TypeScript knows this!
  return value ? 'true' : 'false';
}

console.log(processValue('hello'));  // 'HELLO'
console.log(processValue(3.14159));  // '3.14'
console.log(processValue(true));     // 'true'

// typeof with early returns (preferred pattern)
function formatInput(input: string | number | null | undefined): string {
  // Handle null and undefined first
  if (typeof input === 'undefined') {
    return 'No input provided';
  }
  
  if (input === null) {
    return 'Input was null';
  }
  
  // Now TypeScript knows: input is string | number
  if (typeof input === 'string') {
    return `String: ${input.length} characters`;
  }
  
  // Only number remains
  return `Number: ${input.toFixed(1)}`;
}

console.log(formatInput('hello'));    // 'String: 5 characters'
console.log(formatInput(42));         // 'Number: 42.0'
console.log(formatInput(null));       // 'Input was null'
console.log(formatInput(undefined));  // 'No input provided'

// CRITICAL: typeof null returns 'object' (JavaScript quirk!)
function handleObject(value: object | null): void {
  // WRONG - null passes this check!
  if (typeof value === 'object') {
    // value could STILL be null here because typeof null === 'object'
    console.log('Could be object or null');
  }
  
  // CORRECT - check for null explicitly first
  if (value === null) {
    console.log('Value is null');
    return;
  }
  
  // Now TypeScript knows value is object (not null)
  console.log('Value is definitely an object');
}

handleObject({ name: 'Alice' });  // 'Value is definitely an object'
handleObject(null);                // 'Value is null'
```
