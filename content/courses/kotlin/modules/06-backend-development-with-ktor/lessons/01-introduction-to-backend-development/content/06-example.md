---
type: "EXAMPLE"
title: "📝 Practical Example: Library API Design"
---


Let's design an API for a library system on paper:

### Resources
- Books
- Users
- Loans (when someone borrows a book)

### Endpoints


---



```kotlin
Books:
GET    /api/books              - List all books
GET    /api/books/42           - Get specific book
POST   /api/books              - Add new book (admin only)
PUT    /api/books/42           - Update book details
DELETE /api/books/42           - Remove book

Users:
GET    /api/users              - List all users
GET    /api/users/alice        - Get user profile
POST   /api/users              - Register new user
PUT    /api/users/alice        - Update user info

Loans:
GET    /api/loans              - List all current loans
POST   /api/loans              - Check out a book
DELETE /api/loans/5            - Return a book

Search:
GET    /api/books?author=Orwell           - Search by author
GET    /api/books?available=true          - Find available books
GET    /api/books?category=scifi&year=2020 - Multiple filters
```
