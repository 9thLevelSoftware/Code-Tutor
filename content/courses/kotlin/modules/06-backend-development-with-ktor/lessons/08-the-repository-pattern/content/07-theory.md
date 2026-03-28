---
type: "THEORY"
title: "🎯 Step 3: Service Layer (Business Logic)"
---


Create a service that uses repositories:


**Service layer responsibilities:**
- ✅ Business logic and validation
- ✅ Orchestrating multiple repositories
- ✅ Error handling
- ✅ Use cases (what the app does)

---



```kotlin
// src/main/kotlin/com/example/services/BookService.kt
package com.example.services

import com.example.models.*
import com.example.repositories.BookRepository

class BookService(
    private val bookRepository: BookRepository
) {

    fun getAllBooks(): List<Book> {
        return bookRepository.getAll()
    }

    fun getBook(id: Int): Book? {
        return bookRepository.getById(id)
    }

    fun createBook(request: CreateBookRequest): Result<Book> {
        // Validation
        if (request.title.isBlank()) {
            return Result.failure(ValidationException("Title is required"))
        }

        if (request.author.isBlank()) {
            return Result.failure(ValidationException("Author is required"))
        }

        // Check for duplicates
        val existing = bookRepository.findByAuthor(request.author)
            .find { it.title.equals(request.title, ignoreCase = true) }

        if (existing != null) {
            return Result.failure(DuplicateException("Book already exists"))
        }

        // Create book
        val book = Book(
            id = 0,  // Will be assigned by database
            title = request.title,
            author = request.author,
            year = request.year,
            isbn = request.isbn
        )

        val id = bookRepository.insert(book)
        val created = bookRepository.getById(id)
            ?: return Result.failure(Exception("Failed to retrieve created book"))

        return Result.success(created)
    }

    fun updateBook(id: Int, request: UpdateBookRequest): Result<Book> {
        // Check if exists
        val existing = bookRepository.getById(id)
            ?: return Result.failure(NotFoundException("Book not found"))

        // Build updated book
        val updated = existing.copy(
            title = request.title ?: existing.title,
            author = request.author ?: existing.author,
            year = request.year ?: existing.year,
            isbn = request.isbn ?: existing.isbn
        )

        // Update in database
        val success = bookRepository.update(id, updated)

        return if (success) {
            Result.success(updated)
        } else {
            Result.failure(Exception("Failed to update book"))
        }
    }

    fun deleteBook(id: Int): Result<Unit> {
        val exists = bookRepository.getById(id) != null
        if (!exists) {
            return Result.failure(NotFoundException("Book not found"))
        }

        val deleted = bookRepository.delete(id)

        return if (deleted) {
            Result.success(Unit)
        } else {
            Result.failure(Exception("Failed to delete book"))
        }
    }

    fun searchBooks(query: String): List<Book> {
        if (query.isBlank()) {
            return emptyList()
        }
        return bookRepository.search(query)
    }
}

// Custom exceptions
class ValidationException(message: String) : Exception(message)
class NotFoundException(message: String) : Exception(message)
class DuplicateException(message: String) : Exception(message)
```
