---
type: "THEORY"
title: "What's Next?"
---


You now have complete control over program flow—decisions and loops! But how do you store and work with multiple pieces of related data? What if you need to manage a shopping cart with many items, or a class roster with dozens of students?

In **Lesson 2.6: Lists - Storing Multiple Items**, you'll learn:
- Creating and using lists
- Mutable vs immutable lists
- Adding, removing, and accessing elements
- Powerful list operations: filter, map, and more
- Real-world list applications

**Preview:**

---

**Outstanding work! You've completed Lesson 2.5. Lists await you next!** 🎉



```kotlin
val fruits = listOf("Apple", "Banana", "Cherry")
val numbers = mutableListOf(1, 2, 3)
numbers.add(4)

val doubled = numbers.map { it * 2 }
val evens = numbers.filter { it % 2 == 0 }
```
