---
type: "ANALOGY"
title: "The PEZ Dispenser and the Grocery Line"
---

Working with arrays isn't just about changing values; it's about managing the flow of data. JavaScript provides four built-in "methods" to add and remove items from the ends of an array.

#### The Stack (Back of the list)
Imagine a stack of dinner plates or a PEZ dispenser.
*   **push():** You put a new plate on the **top** (the end of the array).
*   **pop():** You take the **top** plate off. 
This is fast and efficient because the other items in the stack don't have to move.

#### The Queue (Front of the list)
Imagine a line at a grocery store.
*   **unshift():** Someone "cuts" to the very **front** of the line.
*   **shift():** The person at the **front** is served and leaves the line.
In a real array, these are "expensive" actions because if you add someone to the front, every other person in the line has to step back one spot to update their index number!

#### Why these names?
*   **Push/Pop:** Common computer science terms for managing "Stacks".
*   **Shift/Unshift:** Imagine a sliding scale. When you remove the first item, you "shift" all remaining items down by one.
