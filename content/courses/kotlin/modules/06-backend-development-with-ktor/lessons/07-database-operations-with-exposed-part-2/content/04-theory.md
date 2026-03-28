---
type: "THEORY"
title: "🔄 UPDATE Operations"
---


### Basic Update


**Understanding the syntax:**


- **update({ condition })**: WHERE clause
- **it[column] = value**: SET clause
- Returns the number of rows updated

**Behind the scenes SQL:**

### Conditional Updates


### Partial Updates (Only Changed Fields)


**This is powerful for PATCH endpoints** where clients only send changed fields!

---



```kotlin
data class UpdateBookRequest(
    val title: String? = null,
    val author: String? = null,
    val year: Int? = null,
    val isbn: String? = null
)

fun partialUpdate(id: Int, request: UpdateBookRequest): Boolean = transaction {
    // Build update dynamically based on what's provided
    val updateCount = Books.update({ Books.id eq id }) {
        request.title?.let { newTitle -> it[Books.title] = newTitle }
        request.author?.let { newAuthor -> it[Books.author] = newAuthor }
        request.year?.let { newYear -> it[Books.year] = newYear }
        request.isbn?.let { newIsbn -> it[Books.isbn] = newIsbn }
    }
    updateCount > 0
}
```
