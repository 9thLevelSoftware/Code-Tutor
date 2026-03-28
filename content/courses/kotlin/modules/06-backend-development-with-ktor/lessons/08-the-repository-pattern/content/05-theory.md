---
type: "THEORY"
title: "📝 Step 1: Define Repository Interfaces"
---


Create interfaces in the domain/service layer:


**Why interfaces?**
- ✅ Defines what operations are available
- ✅ Routes depend on interface, not implementation
- ✅ Easy to create mock implementations for testing
- ✅ Can swap implementations (in-memory, SQL, NoSQL, etc.)

---



```kotlin
// src/main/kotlin/com/example/repositories/BookRepository.kt
package com.example.repositories

import com.example.models.Book

interface BookRepository {
    fun getAll(): List<Book>
    fun getById(id: Int): Book?
    fun insert(book: Book): Int
    fun update(id: Int, book: Book): Boolean
    fun delete(id: Int): Boolean
    fun findByAuthor(author: String): List<Book>
    fun search(query: String): List<Book>
}
```
