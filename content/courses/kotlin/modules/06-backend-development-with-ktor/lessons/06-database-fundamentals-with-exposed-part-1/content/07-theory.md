---
type: "THEORY"
title: "💻 Basic Database Operations"
---


### Inserting Data

Create `src/main/kotlin/com/example/database/dao/BookDao.kt`:


**Understanding the INSERT:**


- **transaction { }**: All database operations must be in a transaction
- **Books.insert { }**: DSL for SQL INSERT
- **it[column] = value**: Set column values
- **[Books.id]**: Extract the auto-generated ID

**Behind the scenes SQL:**

### Querying Data


**Mapping to Kotlin objects:**


---



```kotlin
Books.selectAll().map { row ->
    Book(
        id = row[Books.id],
        title = row[Books.title],
        author = row[Books.author],
        year = row[Books.year],
        isbn = row[Books.isbn]
    )
}
```
