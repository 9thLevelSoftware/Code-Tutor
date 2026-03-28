---
type: "THEORY"
title: "🗂️ Defining Tables with Exposed"
---


### Creating Your First Table

Create `src/main/kotlin/com/example/database/tables/Books.kt`:


**Breaking this down:**

- **object**: Singleton (only one instance exists)
- **Table()**: Exposed's base class for defining tables

- **integer("id")**: Column named "id" storing integers
- **autoIncrement()**: Database automatically generates IDs (1, 2, 3, ...)

- **varchar**: Variable-length string
- **255**: Maximum length

- **nullable()**: This column can be NULL (optional field)

- Defines `id` as the primary key (unique identifier)

### Column Types Reference


---



```kotlin
// Numbers
val intColumn = integer("int_col")
val longColumn = long("long_col")
val floatColumn = float("float_col")
val doubleColumn = double("double_col")
val decimalColumn = decimal("price", 10, 2)  // 10 digits, 2 decimal places

// Text
val stringColumn = varchar("name", 100)
val textColumn = text("description")  // Unlimited length

// Boolean
val boolColumn = bool("active")

// Date/Time
val dateColumn = datetime("created_at")

// Special
val enumColumn = enumeration<Status>("status")
val blobColumn = blob("image")  // Binary data
```
