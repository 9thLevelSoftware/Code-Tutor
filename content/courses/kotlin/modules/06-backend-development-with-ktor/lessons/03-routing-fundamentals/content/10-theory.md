---
type: "THEORY"
title: "📝 Lesson Checkpoint Quiz"
---


Test your understanding of Ktor routing:

### Question 1
In the route definition `route("/books") { get("/{id}") { } }`, what is the full path that will be matched?

A) `/books`
B) `/books/{id}`
C) `/{id}/books`
D) `/books/id`

---

### Question 2
Which HTTP status code should you return when a client tries to create a book with an empty title?

A) 200 OK
B) 201 Created
C) 400 Bad Request
D) 404 Not Found

---

### Question 3
What does `call.receive<CreateBookRequest>()` do?

A) Sends a CreateBookRequest to the client
B) Converts the JSON request body into a CreateBookRequest object
C) Creates a new book in the database
D) Validates that the request is correctly formatted

---

