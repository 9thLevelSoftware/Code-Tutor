---
type: "THEORY"
title: "🧪 Testing Your Complete API"
---


### Test GET All Books


**Expected Response:**

### Test GET Single Book


### Test CREATE New Book


**Expected Response:**

### Test UPDATE Book


### Test DELETE Book


### Test Error Cases


---



```bash
# Invalid ID (not a number)
curl http://localhost:8080/api/books/abc

# Non-existent book
curl http://localhost:8080/api/books/9999

# Empty title (validation error)
curl -X POST http://localhost:8080/api/books \
  -H "Content-Type: application/json" \
  -d '{"title": "", "author": "Unknown", "year": 2024}'
```
