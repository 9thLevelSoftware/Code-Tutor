---
type: "THEORY"
title: "🌐 HTTP: The Language of the Web"
---


### What Is HTTP?

**HTTP** stands for **Hypertext Transfer Protocol**. It's the standard way computers communicate on the web.

Think of HTTP as the "language rules" for how a customer (client) and a waiter (server) communicate:

- **Customer**: "I'd like a coffee, please." (GET request)
- **Waiter**: "Here's your coffee." (200 OK response)

### HTTP Request Structure

When a client makes a request, it includes:


**Components**:
1. **Method**: What action to perform (GET, POST, PUT, DELETE)
2. **Path**: Which resource you want (`/api/books`)
3. **Headers**: Metadata about the request
4. **Body**: Data sent with the request (optional)

### HTTP Methods: The "Verbs" of the Web

| Method   | Purpose           | Restaurant Analogy          | Example              |
|----------|-------------------|-----------------------------|----------------------|
| **GET**  | Retrieve data     | "What's on the menu?"       | Get list of books    |
| **POST** | Create new data   | "I'd like to order this"    | Create a new book    |
| **PUT**  | Update/replace    | "Change my entire order"    | Update book details  |
| **DELETE** | Remove data     | "Cancel my order"           | Delete a book        |

### HTTP Status Codes: The "Results" of Requests

Status codes tell you what happened with your request:

#### **2xx Success** ✅
- **200 OK**: Request succeeded, here's your data
- **201 Created**: New resource created successfully
- **204 No Content**: Success, but no data to return

#### **4xx Client Errors** ❌ (You made a mistake)
- **400 Bad Request**: Your request doesn't make sense
- **401 Unauthorized**: You need to log in first
- **403 Forbidden**: You're logged in, but not allowed to do this
- **404 Not Found**: This resource doesn't exist

#### **5xx Server Errors** 💥 (Server made a mistake)
- **500 Internal Server Error**: Something broke on the server
- **503 Service Unavailable**: Server is temporarily down

### Real-World Example

When you visit `https://example.com/books`:


---



```kotlin
1. Your browser sends:
   GET /books HTTP/1.1
   Host: example.com

2. Server processes the request

3. Server responds:
   HTTP/1.1 200 OK
   Content-Type: application/json

   [
     {"id": 1, "title": "1984"},
     {"id": 2, "title": "Brave New World"}
   ]
```
