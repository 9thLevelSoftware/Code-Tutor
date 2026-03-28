---
type: "THEORY"
title: "🔧 Understanding URLs and Endpoints"
---


### URL Structure


- **Scheme**: `https://` (secure) or `http://` (insecure)
- **Domain**: The server address
- **Port**: Usually 80 (HTTP) or 443 (HTTPS), often hidden
- **Path**: The route to the resource
- **Query Parameters**: Additional filters or options
- **Fragment**: Specific section (rarely used in APIs)

### RESTful API Design Principles

**REST** = Representational State Transfer (don't worry about the name, focus on the pattern)

Good API endpoint design:


**Key Principles**:
1. Use **nouns** for resources (books, users, orders)
2. Use **HTTP methods** for actions (GET, POST, DELETE)
3. Use **plural** names (`/books`, not `/book`)
4. Be **consistent** throughout your API

---



```kotlin
✅ GET    /books           - Get all books
✅ GET    /books/123       - Get book with ID 123
✅ POST   /books           - Create a new book
✅ PUT    /books/123       - Update book 123 (replace entirely)
✅ PATCH  /books/123       - Update book 123 (partial update)
✅ DELETE /books/123       - Delete book 123

❌ GET    /getAllBooks     - Don't use verbs in URLs
❌ POST   /books/delete    - Use DELETE method instead
❌ GET    /book             - Use plural nouns
```
