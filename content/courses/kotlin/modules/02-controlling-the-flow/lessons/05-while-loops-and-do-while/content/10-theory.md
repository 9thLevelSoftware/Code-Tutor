---
type: "THEORY"
title: "Quick Quiz"
---


**Question 1:** What's the output?

<details>
<summary>Answer</summary>

**Output:** `5 4 3 2 1`

**Explanation:** Starts at 5, prints and decrements until x reaches 0 (loop stops when x is not > 0).
</details>

---

**Question 2:** How many times does this execute?

<details>
<summary>Answer</summary>

**Answer:** 0 times

**Explanation:** The condition `10 < 5` is false from the start, so the loop body never executes.
</details>

---

**Question 3:** What's the difference between these?

<details>
<summary>Answer</summary>

**Answer:**
- **A (while):** Checks condition FIRST. Might not execute at all.
- **B (do-while):** Executes FIRST, then checks. Always executes at least once.

**Example:**
Output: `B`
</details>

---

**Question 4:** What does break do?

<details>
<summary>Answer</summary>

**Answer:** `break` immediately exits the loop, regardless of the condition.

**Example:**
</details>

---



```kotlin
while (true) {
    val input = readln()
    if (input == "quit") {
        break  // Exit the infinite loop
    }
    println("You said: $input")
}
```
