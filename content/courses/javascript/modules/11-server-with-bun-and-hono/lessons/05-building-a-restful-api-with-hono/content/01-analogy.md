---
type: "ANALOGY"
title: "Understanding the Concept"
---

Imagine managing a library catalog system:

Library Operations (RESTful API):
- GET /books - View all books (like browsing shelves)
- GET /books/:id - View one specific book (like requesting a book by card number)
- POST /books - Add a new book (like cataloging a new arrival)
- PUT /books/:id - Update book info completely (like replacing a catalog card)
- PATCH /books/:id - Update part of book info (like correcting a typo)
- DELETE /books/:id - Remove a book (like removing from catalog)

Each operation uses a specific method (GET, POST, etc.) and follows a pattern!

REST = Representational State Transfer
- A standard way to design APIs
- Uses HTTP methods correctly
- Predictable URL patterns
- Stateless (each request is independent)

Hono makes building RESTful APIs simple and fast. The same REST principles apply whether you use Express or Hono!