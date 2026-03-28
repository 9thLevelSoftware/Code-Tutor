---
type: "EXAMPLE"
title: "Truthiness Narrowing and Optional Chaining"
---

JavaScript's truthiness rules can narrow types. Combined with optional chaining, you get powerful null-safe code:

```typescript
// TRUTHINESS NARROWING
// If a value is truthy, TypeScript knows it's not null/undefined

function greetUser(name: string | null | undefined): string {
  if (name) {
    // TypeScript knows: name is string (truthy check excludes null/undefined)
    return `Hello, ${name.toUpperCase()}!`;
  }
  return 'Hello, Guest!';
}

console.log(greetUser('Alice'));     // 'Hello, ALICE!'
console.log(greetUser(null));        // 'Hello, Guest!'
console.log(greetUser(undefined));   // 'Hello, Guest!'

// WARNING: Truthiness is tricky with 0, '', and false!
function processCount(count: number | null): string {
  // WRONG - 0 is falsy, but it's a valid number!
  if (count) {
    return `Count: ${count}`;
  }
  return 'No count';  // This triggers for count = 0!
}

console.log(processCount(5));    // 'Count: 5'
console.log(processCount(0));    // 'No count' (WRONG! 0 is valid)
console.log(processCount(null)); // 'No count'

// CORRECT - use explicit null check
function processCountFixed(count: number | null): string {
  if (count !== null) {
    // TypeScript knows: count is number
    return `Count: ${count}`;
  }
  return 'No count';
}

console.log(processCountFixed(0));  // 'Count: 0' (Correct!)

// Double-bang (!!) for boolean conversion
function hasContent(text: string | null | undefined): boolean {
  return !!text;  // Converts to boolean: true if truthy, false if falsy
}

console.log(hasContent('hello'));    // true
console.log(hasContent(''));         // false (empty string is falsy)
console.log(hasContent(null));       // false

// OPTIONAL CHAINING (?.) with narrowing
interface Company {
  name: string;
  address?: {
    street: string;
    city: string;
    country?: string;
  };
}

function getCompanyLocation(company: Company): string {
  // Optional chaining - returns undefined if address is missing
  const city = company.address?.city;
  const country = company.address?.country;
  
  if (city && country) {
    // Both are defined (narrowed to string)
    return `${city}, ${country}`;
  } else if (city) {
    // Only city is defined
    return city;
  }
  return 'Location unknown';
}

let company1: Company = { name: 'TechCorp' };
let company2: Company = { name: 'GlobalInc', address: { street: '123 Main', city: 'Boston' } };
let company3: Company = { name: 'WorldWide', address: { street: '456 Oak', city: 'London', country: 'UK' } };

console.log(getCompanyLocation(company1));  // 'Location unknown'
console.log(getCompanyLocation(company2));  // 'Boston'
console.log(getCompanyLocation(company3));  // 'London, UK'

// NULLISH COALESCING (??) with narrowing
// ?? returns right side only for null/undefined (not 0 or '')

function getDisplayName(user: { name?: string; username: string }): string {
  // ?? only uses fallback for null/undefined, not empty string
  const displayName = user.name ?? user.username;
  return displayName.toUpperCase();  // TypeScript knows it's string
}

console.log(getDisplayName({ username: 'alice123' }));
// 'ALICE123'

console.log(getDisplayName({ name: 'Alice', username: 'alice123' }));
// 'ALICE'

console.log(getDisplayName({ name: '', username: 'alice123' }));
// '' (empty string is NOT nullish, so it's used)

// Combined patterns
interface UserProfile {
  id: number;
  name?: string;
  settings?: {
    theme?: 'light' | 'dark';
    notifications?: boolean;
  };
}

function getUserTheme(profile: UserProfile | null): string {
  // Chain optional access with nullish coalescing
  return profile?.settings?.theme ?? 'light';
}

console.log(getUserTheme(null));                                    // 'light'
console.log(getUserTheme({ id: 1 }));                              // 'light'
console.log(getUserTheme({ id: 1, settings: {} }));                // 'light'
console.log(getUserTheme({ id: 1, settings: { theme: 'dark' } })); // 'dark'
```
