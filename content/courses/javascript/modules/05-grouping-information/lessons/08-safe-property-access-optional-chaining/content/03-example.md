---
type: "EXAMPLE"
title: "Nullish Coalescing (??) Examples"
---

Nullish coalescing (??) provides a default value only when the left side is null or undefined. Unlike the || operator, it preserves valid falsy values like 0, empty string, or false.

```javascript
// The problem with || for default values
let count = 0;
let displayCount = count || 'No count';  // 'No count' - WRONG!
// We wanted 0, but || treats 0 as falsy

let username = '';
let displayName = username || 'Anonymous';  // 'Anonymous' - maybe WRONG!
// What if empty string was intentional?

// Nullish Coalescing only checks for null/undefined
let count2 = 0;
let displayCount2 = count2 ?? 'No count';  // 0 - CORRECT!
console.log(displayCount2);  // 0

let username2 = '';
let displayName2 = username2 ?? 'Anonymous';  // '' - keeps empty string
console.log(displayName2);  // ''

// Only null and undefined trigger the default
console.log(null ?? 'default');       // 'default'
console.log(undefined ?? 'default');  // 'default'
console.log(0 ?? 'default');          // 0
console.log('' ?? 'default');         // ''
console.log(false ?? 'default');      // false

// Practical example: API response handling
let apiResponse = {
  user: {
    settings: {
      theme: 'dark',
      fontSize: 0,  // Valid setting!
      notifications: null  // Not set
    }
  }
};

let theme = apiResponse?.user?.settings?.theme ?? 'light';
console.log(theme);  // 'dark'

let fontSize = apiResponse?.user?.settings?.fontSize ?? 16;
console.log(fontSize);  // 0 (not 16! because 0 is a valid value)

let notifications = apiResponse?.user?.settings?.notifications ?? true;
console.log(notifications);  // true (null triggers default)
```
