---
type: "EXAMPLE"
title: "Custom Type Guards with 'is' Keyword"
---

When built-in checks aren't enough, create custom type guard functions. The magic is in the return type: `param is Type`. This tells TypeScript that when the function returns true, the parameter IS that type:

```typescript
// Basic custom type guard
function isString(value: unknown): value is string {
  return typeof value === 'string';
}

function isNumber(value: unknown): value is number {
  return typeof value === 'number' && !isNaN(value);
}

// Using the custom guards
function processUnknown(value: unknown): string {
  if (isString(value)) {
    // TypeScript knows: value is string
    return value.toUpperCase();
  }
  
  if (isNumber(value)) {
    // TypeScript knows: value is number
    return value.toFixed(2);
  }
  
  return 'Unknown type';
}

console.log(processUnknown('hello'));  // 'HELLO'
console.log(processUnknown(42.567));   // '42.57'
console.log(processUnknown(null));     // 'Unknown type'

// Type guard for interface validation
interface User {
  id: number;
  name: string;
  email: string;
}

// REUSABLE type guard - validates unknown data is User
function isUser(value: unknown): value is User {
  // Check if it's an object
  if (typeof value !== 'object' || value === null) {
    return false;
  }
  
  // Cast to access properties (we're validating them)
  const obj = value as Record<string, unknown>;
  
  // Check required properties exist and have correct types
  return (
    typeof obj.id === 'number' &&
    typeof obj.name === 'string' &&
    typeof obj.email === 'string'
  );
}

// Safe API response handling
function handleApiData(data: unknown): void {
  if (isUser(data)) {
    // TypeScript knows: data is User - full autocomplete!
    console.log(`User: ${data.name} (${data.email})`);
    console.log(`ID: ${data.id}`);
  } else {
    console.log('Invalid user data received');
  }
}

handleApiData({ id: 1, name: 'Alice', email: 'alice@test.com' });
// User: Alice (alice@test.com)
// ID: 1

handleApiData({ name: 'Bob' });  // Missing id and email
// Invalid user data received

handleApiData('not an object');
// Invalid user data received

// Type guard for arrays
function isStringArray(value: unknown): value is string[] {
  return Array.isArray(value) && value.every(item => typeof item === 'string');
}

function processData(data: unknown): void {
  if (isStringArray(data)) {
    // TypeScript knows: data is string[]
    console.log('String array:', data.join(', '));
    console.log('Total length:', data.reduce((sum, s) => sum + s.length, 0));
  }
}

processData(['apple', 'banana', 'cherry']);
// String array: apple, banana, cherry
// Total length: 17

// Advanced: Type guard for nested objects
interface Address {
  street: string;
  city: string;
  zip: string;
}

interface Person {
  name: string;
  age: number;
  address: Address;
}

function isAddress(value: unknown): value is Address {
  if (typeof value !== 'object' || value === null) return false;
  const obj = value as Record<string, unknown>;
  return (
    typeof obj.street === 'string' &&
    typeof obj.city === 'string' &&
    typeof obj.zip === 'string'
  );
}

function isPerson(value: unknown): value is Person {
  if (typeof value !== 'object' || value === null) return false;
  const obj = value as Record<string, unknown>;
  return (
    typeof obj.name === 'string' &&
    typeof obj.age === 'number' &&
    isAddress(obj.address)  // Reuse the address guard!
  );
}

let personData = {
  name: 'Alice',
  age: 30,
  address: { street: '123 Main St', city: 'Boston', zip: '02101' }
};

if (isPerson(personData)) {
  console.log(`${personData.name} lives in ${personData.address.city}`);
}
// Alice lives in Boston
```
