---
type: "THEORY"
title: "✅ Solution & Explanation"
---


Here's the complete solution with all exercises:


### Testing the Solutions


### Key Techniques Used

1. **Query Parameters**: `call.request.queryParameters["key"]`
2. **Filtering**: `filter { predicate }` on lists
3. **Range Check**: `it.year in minYear..maxYear`
4. **Case-Insensitive Search**: `contains(query, ignoreCase = true)`
5. **Inline Data Classes**: Define response structure locally

---



```bash
# Exercise 1: Search
curl "http://localhost:8080/api/books/search?title=brave"

# Exercise 2: Filter by year
curl "http://localhost:8080/api/books/filter?minYear=1930&maxYear=1950"

# Exercise 3: Books by author
curl http://localhost:8080/api/books/author/Orwell

# Exercise 4: Count
curl http://localhost:8080/api/books/count
```
