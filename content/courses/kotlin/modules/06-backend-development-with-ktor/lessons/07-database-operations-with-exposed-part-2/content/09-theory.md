---
type: "THEORY"
title: "📦 Batch Operations"
---


### Batch Insert

Inserting many records efficiently:


**Why batch operations?**
- ✅ Much faster for large datasets
- ✅ Single database round-trip
- ✅ Better transaction handling

### Batch Update


---



```kotlin
fun updateBatch(updates: Map<Int, String>): Unit = transaction {
    updates.forEach { (id, newTitle) ->
        Books.update({ Books.id eq id }) {
            it[title] = newTitle
        }
    }
}
```
