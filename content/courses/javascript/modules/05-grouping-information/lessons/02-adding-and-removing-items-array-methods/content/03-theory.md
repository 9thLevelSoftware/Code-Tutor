---
type: "THEORY"
title: "Mutating the Array"
---

These four methods are **mutators**, meaning they change the original array directly.

### 1. Handling the End: `push` and `pop`
*   **`push(...items)`:** Adds one or more elements to the end and returns the **new length** of the array.
*   **`pop()`:** Removes the last element and returns **that element**. If the array is empty, it returns `undefined`.

### 2. Handling the Start: `unshift` and `shift`
*   **`unshift(...items)`:** Adds one or more elements to the beginning and returns the **new length**.
*   **`shift()`:** Removes the first element and returns **that element**.

### 3. Performance Reality
In high-performance applications, `push` and `pop` are significantly faster than `shift` and `unshift`. 
*   When you `pop`, you just throw away the last item. 
*   When you `shift`, JavaScript has to re-calculate the index of **every other item** in the array (0 becomes 1, 1 becomes 2, etc.). For a small array, this doesn't matter, but for an array of 100,000 items, it can slow down your app!

### 4. Method Chaining (Preview)
Because these methods return values (either the new length or the removed item), you can sometimes use them inside other expressions, though it's usually cleaner to keep them on their own lines.
