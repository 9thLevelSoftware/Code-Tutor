---
type: "THEORY"
title: "📝 Lesson Checkpoint Quiz"
---


### Question 1
What does the following code do?

A) Updates all books by incrementing their year by 1
B) Updates only books published before 1950, incrementing their year by 1
C) Deletes books from before 1950
D) Selects books from before 1950

---

### Question 2
What's the difference between `innerJoin` and `leftJoin`?

A) There is no difference
B) innerJoin only returns rows where both tables have matches; leftJoin returns all rows from the left table
C) leftJoin is faster
D) innerJoin supports more tables

---

### Question 3
Why use batch operations instead of individual inserts in a loop?

A) They look better in code
B) They're required by Exposed
C) They're much faster and use fewer database connections
D) They provide better error messages

---



```kotlin
Books.update({ Books.year less 1950 }) { it[year] = year + 1 }
```
