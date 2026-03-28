---
type: "ANALOGY"
title: "The Concept: Condition-Based Repetition"
---


### Real-World While Loops

You use condition-based repetition constantly:

**Making coffee:**

**Waiting in line:**

**Learning to ride a bike:**

The key difference from for loops: **You don't know beforehand how many times you'll repeat**. You repeat until a condition changes.

### For vs While: The Fundamental Difference

**Use FOR when:**
- You know the number of iterations upfront
- You're iterating through a collection
- You're counting within a specific range


**Use WHILE when:**
- You repeat until a condition changes
- The number of iterations is unknown
- You're waiting for user input or external event


---



```kotlin
// I don't know when user will enter "quit"
var input = ""
while (input != "quit") {
    input = readln()
}
```
