---
type: "THEORY"
title: "Understanding Context Parameters"
---


### The Problem

Kotlin has extension functions that operate on a single receiver:

```kotlin
fun String.wordCount(): Int = this.split(" ").size
"Hello World".wordCount()  // String is the receiver
```

But what if a function needs multiple "contexts" to operate?

```kotlin
class Logger { fun info(msg: String) { /* ... */ } }
class Database { fun query(sql: String): List<Row> { /* ... */ } }

// Traditional approach: pass as parameters
fun loadUsers(logger: Logger, db: Database): List<User> {
    logger.info("Loading users")
    return db.query("SELECT * FROM users").map { /* ... */ }
}
```

### The Solution: Context Parameters

Context parameters allow functions to declare named dependencies that the compiler provides implicitly:

```kotlin
context(logger: Logger, db: Database)
fun loadUsers(): List<User> {
    logger.info("Loading users")      // Explicit access via parameter name
    return db.query("SELECT * FROM users").map { /* ... */ }
}
```

> **Feature Status**: Context parameters are Beta since Kotlin 2.2 and experimental in Kotlin 2.3.4. Enable with `-Xcontext-parameters`. They replace the deprecated *context receivers* experiment (`-Xcontext-receivers`) which used unnamed `context(Type)` syntax.

---

