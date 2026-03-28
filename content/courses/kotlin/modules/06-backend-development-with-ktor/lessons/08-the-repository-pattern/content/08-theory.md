---
type: "THEORY"
title: "🌐 Step 4: Updated Routes Using Services"
---



**Notice:**
- Routes are thin (no business logic!)
- Just handle HTTP concerns (parameters, status codes, responses)
- Call service methods
- Map service errors to HTTP status codes

---



```kotlin
// src/main/kotlin/com/example/plugins/Routing.kt
package com.example.plugins

import com.example.models.*
import com.example.services.*
import io.ktor.http.*
import io.ktor.server.application.*
import io.ktor.server.request.*
import io.ktor.server.response.*
import io.ktor.server.routing.*

fun Application.configureRouting(
    bookService: BookService
) {
    routing {
        bookRoutes(bookService)
    }
}

fun Route.bookRoutes(bookService: BookService) {
    route("/api/books") {
        // Get all books
        get {
            val books = bookService.getAllBooks()
            call.respond(ApiResponse(success = true, data = books))
        }

        // Get single book
        get("/{id}") {
            val id = call.parameters["id"]?.toIntOrNull()
                ?: return@get call.respond(
                    HttpStatusCode.BadRequest,
                    ApiResponse<Book>(success = false, message = "Invalid ID")
                )

            val book = bookService.getBook(id)
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

                bookService.createBook(request)
                    .onSuccess { book ->
                        call.respond(
                            HttpStatusCode.Created,
                            ApiResponse(success = true, data = book)
                        )
                    }
                    .onFailure { error ->
                        when (error) {
                            is ValidationException -> call.respond(
                                HttpStatusCode.BadRequest,
                                ApiResponse<Book>(success = false, message = error.message)
                            )
                            is DuplicateException -> call.respond(
                                HttpStatusCode.Conflict,
                                ApiResponse<Book>(success = false, message = error.message)
                            )
                            else -> call.respond(
                                HttpStatusCode.InternalServerError,
                                ApiResponse<Book>(success = false, message = "Server error")
                            )
                        }
                    }
            } catch (e: Exception) {
                call.respond(
                    HttpStatusCode.BadRequest,
                    ApiResponse<Book>(success = false, message = "Invalid request")
                )
            }
        }

        // Update book
        put("/{id}") {
            val id = call.parameters["id"]?.toIntOrNull()
                ?: return@put call.respond(HttpStatusCode.BadRequest)

            val request = call.receive<UpdateBookRequest>()

            bookService.updateBook(id, request)
                .onSuccess { book ->
                    call.respond(ApiResponse(success = true, data = book))
                }
                .onFailure { error ->
                    when (error) {
                        is NotFoundException -> call.respond(HttpStatusCode.NotFound)
                        else -> call.respond(HttpStatusCode.InternalServerError)
                    }
                }
        }

        // Delete book
        delete("/{id}") {
            val id = call.parameters["id"]?.toIntOrNull()
                ?: return@delete call.respond(HttpStatusCode.BadRequest)

            bookService.deleteBook(id)
                .onSuccess {
                    call.respond(
                        HttpStatusCode.OK,
                        ApiResponse<Unit>(success = true, message = "Book deleted")
                    )
                }
                .onFailure { error ->
                    when (error) {
                        is NotFoundException -> call.respond(HttpStatusCode.NotFound)
                        else -> call.respond(HttpStatusCode.InternalServerError)
                    }
                }
        }

        // Search
        get("/search") {
            val query = call.request.queryParameters["q"] ?: ""
            val results = bookService.searchBooks(query)
            call.respond(ApiResponse(success = true, data = results))
        }
    }
}
```
