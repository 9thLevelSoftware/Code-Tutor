---
type: "THEORY"
title: "🔍 Complex WHERE Clauses"
---


### Comparison Operators


### Logical Operators


### String Operations


### IN Operator


### NULL Checks


---



```kotlin
// IS NULL
Books.selectAll().where {
    Books.isbn.isNull()
}

// IS NOT NULL
Books.selectAll().where {
    Books.isbn.isNotNull()
}
```
