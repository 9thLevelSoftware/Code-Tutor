---
type: "EXAMPLE"
title: "🔍 Code Breakdown"
---


Let's analyze the key patterns:

### 1. Route Organization


**Benefits:**
- Clear hierarchy
- Easy to add new routes
- Can move to separate files as project grows

### 2. Receiving Request Bodies


- **call.receive<T>()**: Automatically parses JSON to Kotlin object
- Throws exception if JSON is invalid (we'll handle this in later lessons)

### 3. Responding with Status Codes


**Common patterns:**
- **200 OK**: Successful GET/PUT
- **201 Created**: Successful POST (new resource)
- **204 No Content**: Successful DELETE (no body needed)
- **400 Bad Request**: Invalid input
- **404 Not Found**: Resource doesn't exist

### 4. Parameter Extraction and Validation


**Key techniques:**
- **?.toIntOrNull()**: Safe conversion (null if not a number)
- **Early return**: If validation fails, respond and exit
- **@get/@post/@put/@delete**: Label for return statement

### 5. API Response Wrapper


**Consistent responses:**

---



```json
{
  "success": true,
  "data": { "id": 1, "title": "1984" },
  "message": "Book created successfully"
}
```
