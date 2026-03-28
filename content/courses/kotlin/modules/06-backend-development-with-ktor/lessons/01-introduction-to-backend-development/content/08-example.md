---
type: "EXAMPLE"
title: "🔍 Code Breakdown"
---


Let's analyze the key concepts:

### 1. Request Structure

- **method**: Tells the server what you want to do
- **path**: Identifies which resource you're targeting
- **headers**: Additional information (authentication, content type, etc.)
- **body**: The actual data (for POST/PUT requests)

### 2. Response Structure

- **statusCode**: Numerical code (200 = success, 404 = not found)
- **statusMessage**: Description of the status
- **body**: The data you requested (or error information)

### 3. Request Handling Logic


This pattern will be the foundation of every backend you build.

---



```kotlin
when {
    request.method != "GET" -> // Wrong HTTP method
    id == null -> // Invalid input
    id !in books -> // Resource doesn't exist
    else -> // Success!
}
```
