---
type: "THEORY"
title: "🔍 Understanding Transactions"
---


### What Is a Transaction?

A **transaction** is an "all-or-nothing" unit of work:

**The Bank Transfer Analogy:**

**If anything fails:**
- ❌ Step 1 succeeds, Step 2 fails → **Rollback** (Alice gets money back)
- ✅ Both succeed → **Commit** (changes saved)

**ACID Properties:**
- **Atomicity**: All or nothing
- **Consistency**: Database stays valid
- **Isolation**: Transactions don't interfere
- **Durability**: Committed data is saved permanently

### Using Transactions in Exposed


---



```kotlin
// All queries must be in a transaction
transaction {
    val books = Books.selectAll().toList()
    Books.insert { /* ... */ }
}

// Transactions can return values
val bookId: Int = transaction {
    Books.insert { /* ... */ }[Books.id]
}

// Nested transactions
transaction {
    val id = transaction {
        Books.insert { /* ... */ }[Books.id]
    }
    Users.insert { it[favoriteBookId] = id }
}
```
