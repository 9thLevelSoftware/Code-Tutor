---
type: "THEORY"
title: "💻 Step 2: Implement Repository"
---



**Key points:**
- All database logic is encapsulated
- `transaction { }` calls are hidden from callers
- Easy to understand: each method does one thing
- Private helper method for mapping

---



```kotlin
// src/main/kotlin/com/example/repositories/BookRepositoryImpl.kt
package com.example.repositories

import com.example.database.tables.Books
import com.example.models.Book
import org.jetbrains.exposed.v1.core.*
import org.jetbrains.exposed.v1.core.SqlExpressionBuilder.eq
import org.jetbrains.exposed.v1.jdbc.transactions.transaction

class BookRepositoryImpl : BookRepository {

    override fun getAll(): List<Book> = transaction {
        Books.selectAll()
            .orderBy(Books.title)
            .map { rowToBook(it) }
    }

    override fun getById(id: Int): Book? = transaction {
        Books.selectAll()
            .where { Books.id eq id }
            .map { rowToBook(it) }
            .singleOrNull()
    }

    override fun insert(book: Book): Int = transaction {
        Books.insert {
            it[title] = book.title
            it[author] = book.author
            it[year] = book.year
            it[isbn] = book.isbn
        }[Books.id]
    }

    override fun update(id: Int, book: Book): Boolean = transaction {
        Books.update({ Books.id eq id }) {
            it[title] = book.title
            it[author] = book.author
            it[year] = book.year
            it[isbn] = book.isbn
        } > 0
    }

    override fun delete(id: Int): Boolean = transaction {
        Books.deleteWhere { Books.id eq id } > 0
    }

    override fun findByAuthor(author: String): List<Book> = transaction {
        Books.selectAll()
            .where { Books.author eq author }
            .map { rowToBook(it) }
    }

    override fun search(query: String): List<Book> = transaction {
        Books.selectAll()
            .where {
                (Books.title like "%$query%") or
                (Books.author like "%$query%")
            }
            .map { rowToBook(it) }
    }

    private fun rowToBook(row: ResultRow): Book {
        return Book(
            id = row[Books.id],
            title = row[Books.title],
            author = row[Books.author],
            year = row[Books.year],
            isbn = row[Books.isbn]
        )
    }
}
```
