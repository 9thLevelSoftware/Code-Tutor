---
type: "THEORY"
title: "Hands-On Practice"
---


### Exercise 1: Temperature Advisor

**Challenge:** Write a program that:
1. Takes a temperature value
2. Prints different advice based on the temperature:
   - If temp >= 100: "Extreme heat warning! Stay indoors."
   - If temp >= 80: "It's hot! Stay hydrated."
   - If temp >= 60: "Nice weather!"
   - If temp < 60: "It's chilly! Bring a jacket."

<details>
<summary>Click to see solution</summary>


**Output:**

**Key concepts:**
- Multiple conditions with else if
- Ordered from most specific to least specific
- Each temperature falls into exactly one category
</details>

---

### Exercise 2: Even or Odd Checker

**Challenge:** Write a program that:
1. Takes a number
2. Checks if it's even or odd
3. Prints the result

**Hint:** Use the modulo operator `%`. A number is even if `number % 2 == 0`.

<details>
<summary>Click to see solution</summary>


**Output:**

**How it works:**
- `%` (modulo) gives the remainder after division
- `17 % 2` = 1 (remainder when dividing 17 by 2)
- `1 == 0` is false, so else block executes

**Even number example:**
- `18 % 2` = 0
- `0 == 0` is true, so if block executes
</details>

---

### Exercise 3: Login System

**Challenge:** Create a simple login system that:
1. Stores a correct username and password
2. Takes user input for username and password
3. Checks if both match
4. Prints "Login successful" or "Login failed"

<details>
<summary>Click to see solution</summary>


**Sample run:**

**Note:** We're using `&&` (AND operator) which you'll learn more about in the next lesson. For now, understand that both conditions must be true for the if block to execute.
</details>

---

### Exercise 4: Discount Calculator

**Challenge:** Write a program that:
1. Takes a purchase amount
2. Applies discounts based on the amount:
   - $100+: 20% discount
   - $50-$99: 10% discount
   - Under $50: No discount
3. Prints the final price

<details>
<summary>Click to see solution</summary>


**Output:**

**Key concepts:**
- Using if as an expression to calculate the discount
- Storing the result in a variable
- Performing calculations with the result
</details>

---



```kotlin
Original price: $75.0
Discount: 10.0%
Final price: $67.5
```
