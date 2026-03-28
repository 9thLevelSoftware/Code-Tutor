---
type: "EXAMPLE"
title: "💻 Code: Building a Complete Books API"
---


Let's build a complete CRUD API step by step.

### Step 1: Define the Data Model

Create `src/main/kotlin/com/example/models/Book.kt`:


**Understanding the annotations:**

- **@Serializable**: Tells kotlinx.serialization this class can be converted to/from JSON
- **data class**: Automatically generates `equals()`, `hashCode()`, `toString()`, `copy()`
- **String?**: The `?` makes `isbn` nullable (optional)

### Step 2: Create an In-Memory Data Store

Create `src/main/kotlin/com/example/data/BookStorage.kt`:


**Key concepts:**

- **object BookStorage**: Singleton pattern (only one instance)
- **AtomicInteger**: Thread-safe counter for generating IDs
- **init { }**: Code that runs when the object is first accessed
- **find { }**: Returns first matching item or `null`
- **indexOfFirst { }**: Returns index of first match or `-1`
- **removeIf { }**: Removes all items matching the predicate

### Step 3: Define Request/Response Models

Create `src/main/kotlin/com/example/models/BookRequest.kt`:


**Why separate request models?**

1. **Security**: Clients shouldn't send IDs when creating (server assigns them)
2. **Flexibility**: Updates can be partial (only changed fields)
3. **Clarity**: Clear what data is expected

### Step 4: Build the Routes

Now for the main event! Update `src/main/kotlin/com/example/plugins/Routing.kt`:


---



```kotlin
package com.example.plugins

import com.example.data.BookStorage
import com.example.models.*
import io.ktor.http.*
import io.ktor.server.application.*
import io.ktor.server.request.*
import io.ktor.server.response.*
import io.ktor.server.routing.*

fun Application.configureRouting() {
    routing {
        // Root endpoint
        get("/") {
            call.respondText("Books API is running! Visit /api/books")
        }

        // API routes
        route("/api") {
            bookRoutes()
        }
    }
}

// Book routes grouped together
fun Route.bookRoutes() {
    route("/books") {
        // GET /api/books - List all books
        get {
            val books = BookStorage.getAll()
            call.respond(
                HttpStatusCode.OK,
                ApiResponse(success = true, data = books)
            )
        }

        // GET /api/books/{id} - Get specific book
        get("/{id}") {
            val id = call.parameters["id"]?.toIntOrNull()

            if (id == null) {
                call.respond(
                    HttpStatusCode.BadRequest,
                    ApiResponse<Book>(
                        success = false,
                        message = "Invalid book ID"
                    )
                )
                return@get
            }

            val book = BookStorage.getById(id)

            if (book == null) {
                call.respond(
                    HttpStatusCode.NotFound,
                    ApiResponse<Book>(
                        success = false,
                        message = "Book not found"
                    )
                )
            } else {
                call.respond(
                    HttpStatusCode.OK,
                    ApiResponse(success = true, data = book)
                )
            }
        }

        // POST /api/books - Create new book
        post {
            val request = call.receive<CreateBookRequest>()

            // Simple validation
            if (request.title.isBlank() || request.author.isBlank()) {
                call.respond(
                    HttpStatusCode.BadRequest,
                    ApiResponse<Book>(
                        success = false,
                        message = "Title and author are required"
                    )
                )
                return@post
            }

            val newBook = Book(
                id = 0, // Will be replaced by storage
                title = request.title,
                author = request.author,
                year = request.year,
                isbn = request.isbn
            )

            val created = BookStorage.add(newBook)

            call.respond(
                HttpStatusCode.Created,
                ApiResponse(
                    success = true,
                    data = created,
                    message = "Book created successfully"
                )
            )
        }

        // PUT /api/books/{id} - Update book
        put("/{id}") {
            val id = call.parameters["id"]?.toIntOrNull()

            if (id == null) {
                call.respond(
                    HttpStatusCode.BadRequest,
                    ApiResponse<Book>(
                        success = false,
                        message = "Invalid book ID"
                    )
                )
                return@put
            }

            val request = call.receive<CreateBookRequest>()

            val updatedBook = Book(
                id = id,
                title = request.title,
                author = request.author,
                year = request.year,
                isbn = request.isbn
            )

            val success = BookStorage.update(id, updatedBook)

            if (success) {
                call.respond(
                    HttpStatusCode.OK,
                    ApiResponse(
                        success = true,
                        data = updatedBook,
                        message = "Book updated successfully"
                    )
                )
            } else {
                call.respond(
                    HttpStatusCode.NotFound,
                    ApiResponse<Book>(
                        success = false,
                        message = "Book not found"
                    )
                )
            }
        }

        // DELETE /api/books/{id} - Delete book
        delete("/{id}") {
            val id = call.parameters["id"]?.toIntOrNull()

            if (id == null) {
                call.respond(
                    HttpStatusCode.BadRequest,
                    ApiResponse<Unit>(
                        success = false,
                        message = "Invalid book ID"
                    )
                )
                return@delete
            }

            val success = BookStorage.delete(id)

            if (success) {
                call.respond(
                    HttpStatusCode.OK,
                    ApiResponse<Unit>(
                        success = true,
                        message = "Book deleted successfully"
                    )
                )
            } else {
                call.respond(
                    HttpStatusCode.NotFound,
                    ApiResponse<Unit>(
                        success = false,
                        message = "Book not found"
                    )
                )
            }
        }
    }
}
```
