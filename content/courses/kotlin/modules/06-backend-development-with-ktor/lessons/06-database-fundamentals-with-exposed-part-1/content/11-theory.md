---
type: "THEORY"
title: "🎯 Exercise: Add Users Table"
---


Create a Users table and connect it to books (authors).

### Requirements

1. Create a **Users** table with:
   - id (auto-increment primary key)
   - username (unique, not null)
   - email (unique, not null)
   - createdAt (timestamp)

2. Create **UserDao** with methods:
   - `insert(user)`
   - `getAll()`
   - `getById(id)`
   - `getByUsername(username)`

3. Add routes:
   - `POST /api/users` - Create user
   - `GET /api/users` - Get all users
   - `GET /api/users/{id}` - Get user by ID

### Starter Code


---



```kotlin
// Define the table
object Users : Table() {
    // TODO: Add columns
}

// Define the model
@Serializable
data class User(
    val id: Int,
    val username: String,
    val email: String,
    val createdAt: String
)

// TODO: Create UserDao
// TODO: Create routes
```
