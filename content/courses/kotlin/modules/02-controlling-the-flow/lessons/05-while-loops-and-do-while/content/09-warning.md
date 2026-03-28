---
type: "WARNING"
title: "Common Pitfalls and Best Practices"
---


### Pitfall 1: Infinite Loops from Typos

❌ **Dangerous typo:**

✅ **Safe:**

### Pitfall 2: Off-by-One Errors

❌ **Subtle bug:**

✅ **Correct:**

### Pitfall 3: Not Validating Input

❌ **Crash risk:**

✅ **Safe:**

### Best Practice 1: Always Have an Exit

Every loop should have a clear, guaranteed exit condition:


### Best Practice 2: Initialize Before Loop


### Best Practice 3: Choose the Right Loop


---



```kotlin
// Use while when condition-based
var keepGoing = true
while (keepGoing) {
    val choice = readln()
    if (choice == "quit") keepGoing = false
}

// Use for when count-based
for (i in 1..10) {
    println(i)
}

// Use do-while when must execute once
do {
    showMenu()
    choice = readln()
} while (choice != "exit")
```
