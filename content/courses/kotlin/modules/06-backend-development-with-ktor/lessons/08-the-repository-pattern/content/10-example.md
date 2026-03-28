---
type: "EXAMPLE"
title: "🧪 Making Code Testable"
---


### Why This Architecture Enables Testing


**Benefits of testable architecture:**
- ✅ No database needed for tests
- ✅ Fast execution (milliseconds)
- ✅ Reliable (no network/disk issues)
- ✅ Easy to simulate edge cases

---



```kotlin
// src/test/kotlin/com/example/services/BookServiceTest.kt
package com.example.services

import com.example.models.*
import com.example.repositories.BookRepository
import kotlin.test.*

class BookServiceTest {

    // Mock repository (no real database!)
    class MockBookRepository : BookRepository {
        private val books = mutableMapOf<Int, Book>()
        private var nextId = 1

        override fun getAll() = books.values.toList()
        override fun getById(id: Int) = books[id]

        override fun insert(book: Book): Int {
            val id = nextId++
            books[id] = book.copy(id = id)
            return id
        }

        override fun update(id: Int, book: Book): Boolean {
            if (id !in books) return false
            books[id] = book.copy(id = id)
            return true
        }

        override fun delete(id: Int) = books.remove(id) != null

        override fun findByAuthor(author: String) =
            books.values.filter { it.author == author }

        override fun search(query: String) =
            books.values.filter {
                it.title.contains(query, ignoreCase = true) ||
                it.author.contains(query, ignoreCase = true)
            }
    }

    @Test
    fun `create book with valid data should succeed`() {
        val repository = MockBookRepository()
        val service = BookService(repository)

        val request = CreateBookRequest(
            title = "Test Book",
            author = "Test Author",
            year = 2024
        )

        val result = service.createBook(request)

        assertTrue(result.isSuccess)
        assertEquals("Test Book", result.getOrNull()?.title)
    }

    @Test
    fun `create book with blank title should fail`() {
        val repository = MockBookRepository()
        val service = BookService(repository)

        val request = CreateBookRequest(
            title = "",
            author = "Test Author",
            year = 2024
        )

        val result = service.createBook(request)

        assertTrue(result.isFailure)
        assertTrue(result.exceptionOrNull() is ValidationException)
    }

    @Test
    fun `delete non-existent book should fail`() {
        val repository = MockBookRepository()
        val service = BookService(repository)

        val result = service.deleteBook(999)

        assertTrue(result.isFailure)
        assertTrue(result.exceptionOrNull() is NotFoundException)
    }
}
```
