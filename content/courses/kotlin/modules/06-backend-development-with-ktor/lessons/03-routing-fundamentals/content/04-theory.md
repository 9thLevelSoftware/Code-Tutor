---
type: "THEORY"
title: "🗂️ Organizing Routes"
---


As your API grows, putting all routes in one function becomes messy. Let's learn to organize them properly.

### Route Organization Patterns

**Pattern 1: Flat (What We Did Before)**

**Pattern 2: Grouped by Resource (Better!)**

**Pattern 3: Separate Files by Resource (Best for Large Projects)**

For this lesson, we'll use **Pattern 2** (grouped routes).

---



```kotlin
// BookRoutes.kt
fun Route.bookRoutes() {
    route("/books") {
        get { }
        post { }
        // etc.
    }
}

// Application.kt
routing {
    bookRoutes()
    userRoutes()
    orderRoutes()
}
```
