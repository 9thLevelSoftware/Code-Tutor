---
type: "THEORY"
title: "🗑️ DELETE Operations"
---


### Basic Delete


**SQL equivalent:**

### Conditional Deletes


### Delete All (Dangerous!)


⚠️ **Warning**: Always use WHERE clauses unless you really want to delete everything!

---



```kotlin
// Delete all records (use with caution!)
fun deleteAll(): Int = transaction {
    Books.deleteAll()
}
```
