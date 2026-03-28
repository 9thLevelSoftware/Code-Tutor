---
type: "KEY_POINT"
title: "🔗 Table Relationships: Foreign Keys"
---


### One-to-Many Relationship Example

Let's model books and reviews (one book can have many reviews):


**Key concept:**
- Creates a **foreign key** linking `Reviews.bookId` to `Books.id`
- Ensures referential integrity (can't review a non-existent book)

### Creating Tables with Relationships


**Important**: Create parent table (Books) before child table (Reviews).

---



```kotlin
// In DatabaseFactory.init()
transaction(database) {
    SchemaUtils.create(Books, Reviews)  // Order matters!
}
```
