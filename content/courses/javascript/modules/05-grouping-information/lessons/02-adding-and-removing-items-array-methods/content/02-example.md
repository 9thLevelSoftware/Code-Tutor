---
type: "EXAMPLE"
title: "Modifying Arrays"
---

```javascript
let cart = ['Apple', 'Banana'];

// 1. Adding to the END (push)
cart.push('Cherry');
console.log(cart); // ['Apple', 'Banana', 'Cherry']

// 2. Removing from the END (pop)
// pop() actually RETURNS the item it removed!
let lastItem = cart.pop(); 
console.log(`Removed: ${lastItem}`);
console.log(cart); // ['Apple', 'Banana']

// 3. Adding to the FRONT (unshift)
cart.unshift('Apricot');
console.log(cart); // ['Apricot', 'Apple', 'Banana']

// 4. Removing from the FRONT (shift)
// Like pop(), shift() returns the removed item
let firstItem = cart.shift();
console.log(`Removed: ${firstItem}`);
console.log(cart); // ['Apple', 'Banana']

// 5. Multiple items
cart.push('Date', 'Elderberry');
console.log(cart); // ['Apple', 'Banana', 'Date', 'Elderberry']
```