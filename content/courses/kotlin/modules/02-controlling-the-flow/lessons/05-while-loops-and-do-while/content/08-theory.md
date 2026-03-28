---
type: "THEORY"
title: "Hands-On Exercises"
---


### Exercise 1: Number Guessing Game

**Challenge:** Create a number guessing game where:
1. Computer picks a random number 1-100
2. User keeps guessing until correct
3. Provide "higher" or "lower" hints
4. Count the number of guesses

<details>
<summary>Click to see solution</summary>


**Sample Run:**

**Key concepts:**
- Do-while ensures at least one guess
- Using random numbers
- Tracking attempts with a counter
</details>

---

### Exercise 2: Sum Until Zero

**Challenge:** Keep asking user for numbers and sum them. Stop when user enters 0.

<details>
<summary>Click to see solution</summary>


**Sample Run:**
</details>

---

### Exercise 3: Fibonacci Sequence

**Challenge:** Print Fibonacci numbers while they're less than 1000.

Fibonacci: Each number is the sum of the previous two (1, 1, 2, 3, 5, 8, 13...)

<details>
<summary>Click to see solution</summary>


**Output:**

**Key concepts:**
- While(true) with break for complex conditions
- Updating multiple variables
- Fibonacci algorithm
</details>

---

### Exercise 4: Print Even Numbers

**Challenge:** Print even numbers from 1 to 20 using a while loop and continue.

<details>
<summary>Click to see solution</summary>


**Output:**

**Alternative without continue:**
</details>

---



```kotlin
fun main() {
    var number = 0

    println("Even numbers from 1 to 20:")

    while (number < 20) {
        number++

        if (number % 2 == 0) {
            print("$number ")
        }
    }
}
```
