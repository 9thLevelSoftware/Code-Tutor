---
type: "THEORY"
title: "🧪 Testing Your Database-Backed API"
---


### Start the Server


You should see SQL logging:

### Test Creating a Book


**Response:**

### Test Getting All Books


### Restart the Server

**Problem with in-memory database:**
- Stop the server (Ctrl+C)
- Start it again
- Query books again: **They're gone!**

**Solution (for next lesson):**
Change to persistent storage:

---



```kotlin
jdbcUrl = "jdbc:h2:file:./data/mydb"
```
