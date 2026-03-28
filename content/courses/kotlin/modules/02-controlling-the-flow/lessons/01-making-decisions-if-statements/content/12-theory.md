---
type: "THEORY"
title: "Quick Quiz"
---


Test your understanding:

**Question 1:** What will this code print?

<details>
<summary>Answer</summary>

**Output:** `Keep trying!`

**Explanation:** `75 >= 80` is false, so the else block executes.
</details>

---

**Question 2:** What's wrong with this code?

<details>
<summary>Answer</summary>

**Error:** Using `=` instead of `==`

`=` is assignment, `==` is comparison. Should be:
</details>

---

**Question 3:** What will this print if temperature = 85?

<details>
<summary>Answer</summary>

**Output:**

**Explanation:** These are three separate if statements (not else if). Both `85 > 80` and `85 > 70` are true, so both B and C print.
</details>

---

**Question 4:** Is this valid Kotlin code?

<details>
<summary>Answer</summary>

**Yes!** This is valid. In Kotlin, `if` is an expression and can return a value. The result will be "Positive" if x > 0, otherwise "Non-positive".
</details>

---



```kotlin
val result = if (x > 0) "Positive" else "Non-positive"
```
