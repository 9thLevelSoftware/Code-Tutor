---
type: "EXAMPLE"
title: "Safe Access and Backups"
---

```javascript
const user = {
    profile: {
        name: 'Alice',
        // address is missing!
    },
    getSettings() {
        return { theme: 'dark' };
    }
};

// 1. Optional Chaining (?.)
// Without ?. this would crash: console.log(user.profile.address.city);
console.log(user.profile.address?.city); // undefined (No crash!)

// Also works for functions/methods
console.log(user.getSettings?.().theme); // 'dark'
console.log(user.logout?.()); // undefined (No crash even though logout doesn't exist)

// 2. Nullish Coalescing (??)
// Providing a default value ONLY for null/undefined
const userCity = user.profile.address?.city ?? 'Unknown City';
console.log(userCity); // 'Unknown City'

// 3. ?? vs ||
// || treats 0 and "" as false and replaces them
// ?? treats 0 and "" as VALID values
const score = 0;
const finalScore1 = score || 10; // 10
const finalScore2 = score ?? 10; // 0 (Correct!)

// 4. Using with Arrays
const list = null;
console.log(list?.[0]); // undefined
```