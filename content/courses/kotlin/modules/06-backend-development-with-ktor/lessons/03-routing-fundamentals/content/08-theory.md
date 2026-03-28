---
type: "THEORY"
title: "🎯 Exercise: Extend the API"
---


Add these features to your Books API:

### Exercise 1: Search by Title

Add a route that searches books by title (case-insensitive).

**Endpoint**: `GET /api/books/search?title=brave`

**Expected**: Return all books whose title contains "brave" (case-insensitive)

**Hints:**

### Exercise 2: Filter by Year

Add a route to get books published in a specific year range.

**Endpoint**: `GET /api/books/filter?minYear=1930&maxYear=1950`

**Expected**: Return books published between 1930 and 1950 (inclusive)

### Exercise 3: Get Books by Author

Add a route to get all books by a specific author.

**Endpoint**: `GET /api/books/author/{authorName}`

**Expected**: Return all books by that author (case-insensitive match)

### Exercise 4: Count Endpoint

Add a route that returns the total number of books.

**Endpoint**: `GET /api/books/count`

**Expected Response**:

---



```json
{
  "success": true,
  "data": { "count": 5 },
  "message": "Total books counted"
}
```
