---
type: "WARNING"
title: "Modification Mistakes"
---

### 1. The Return Value Trap
Remember that `push` and `unshift` return the **new length**, not the updated array.
```javascript
let list = [1, 2];
let result = list.push(3);
console.log(result); // Result: 3 (the length), NOT [1, 2, 3]
```

### 2. Popping from an Empty Array
If you call `pop()` or `shift()` on an empty array `[]`, JavaScript won't error out. It will simply return `undefined`. Always check if an array has items (`list.length > 0`) if you depend on the value you are removing.

### 3. Confusion between Shift and Unshift
The names are very similar. 
*   **Trick to remember:** `unshift` is the longer word, so it's "adding" something. `shift` is shorter, so it's "removing" something.

### 4. Direct Index Assignment vs. Push
While you *can* add an item by doing `list[list.length] = 'new'`, it is much cleaner and more readable to use `list.push('new')`. Use the methods whenever possible!
