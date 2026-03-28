---
type: "THEORY"
title: "🔌 Integrating with Ktor Routes"
---


### Initialize Database on Startup

Update `src/main/kotlin/com/example/Application.kt`:


### Update Routes to Use Database

Update `src/main/kotlin/com/example/plugins/Routing.kt`:


---



```kotlin
package com.example.plugins

import com.example.database.dao.BookDao
import com.example.models.*
import io.ktor.http.*
import io.ktor.server.application.*
import io.ktor.server.request.*
import io.ktor.server.response.*
import io.ktor.server.routing.*

fun Application.configureRouting() {
    routing {
        route("/api/books") {
            // Get all books
            get {
                val books = BookDao.getAll()
                call.respond(ApiResponse(success = true, data = books))
            }

            // Get book by ID
            get("/{id}") {
                val id = call.parameters["id"]?.toIntOrNull()
                    ?: return@get call.respond(
                        HttpStatusCode.BadRequest,
                        ApiResponse<Book>(success = false, message = "Invalid ID")
                    )

                val book = BookDao.getById(id)
                if (book == null) {
                    call.respond(
                        HttpStatusCode.NotFound,
                        ApiResponse<Book>(success = false, message = "Book not found")
                    )
                } else {
                    call.respond(ApiResponse(success = true, data = book))
                }
            }

            // Create book
            post {
                try {
                    val request = call.receive<CreateBookRequest>()

                    // Validate
                    if (request.title.isBlank() || request.author.isBlank()) {
                        call.respond(
                            HttpStatusCode.BadRequest,
                            ApiResponse<Book>(
                                success = false,
                                message = "Title and author required"
                            )
                        )
                        return@post
                    }

                    // Create book object (no ID yet)
                    val book = Book(
                        id = 0,  // Will be assigned by database
                        title = request.title,
                        author = request.author,
                        year = request.year,
                        isbn = request.isbn
                    )

                    // Insert and get generated ID
                    val generatedId = BookDao.insert(book)

                    // Fetch the created book
                    val createdBook = BookDao.getById(generatedId)

                    call.respond(
                        HttpStatusCode.Created,
                        ApiResponse(
                            success = true,
                            data = createdBook,
                            message = "Book created successfully"
                        )
                    )
                } catch (e: Exception) {
                    call.respond(
                        HttpStatusCode.InternalServerError,
                        ApiResponse<Book>(
                            success = false,
                            message = "Error creating book: ${e.message}"
                        )
                    )
                }
            }
        }
    }
}
```
