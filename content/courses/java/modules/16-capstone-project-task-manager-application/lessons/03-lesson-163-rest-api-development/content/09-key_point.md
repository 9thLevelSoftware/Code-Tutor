---
type: "KEY_POINT"
title: "HTTP Status Code Reference"
---

When building REST APIs, use the appropriate HTTP status codes to communicate results clearly to clients.

## Success Codes (2xx)

| Status Code | Meaning | When to Use |
|-------------|---------|-------------|
| 200 OK | Success | GET requests that return data successfully |
| 201 Created | Resource created | POST requests that create a new resource |
| 204 No Content | Empty success | DELETE requests or successful updates with no body |

## Client Error Codes (4xx)

| Status Code | Meaning | When to Use |
|-------------|---------|-------------|
| 400 Bad Request | Invalid input | Request body fails validation (missing fields, invalid format) |
| 401 Unauthorized | Not authenticated | Missing or invalid JWT token, expired session |
| 403 Forbidden | No permission | Authenticated user lacks access to this resource |
| 404 Not Found | Resource missing | Requested task/user/etc. does not exist |
| 409 Conflict | Duplicate or conflict | Trying to create a resource that already exists |
| 422 Unprocessable | Semantic error | Valid JSON but business rules violated (e.g., past due date) |

## Server Error Codes (5xx)

| Status Code | Meaning | When to Use |
|-------------|---------|-------------|
| 500 Internal Server Error | Unexpected failure | Unhandled exceptions, database unavailable |
| 503 Service Unavailable | System overloaded | Maintenance mode, rate limit exceeded |

## Common Response Patterns

**Successful GET (single item):**
```
HTTP/1.1 200 OK
Content-Type: application/json

{ "id": 1, "title": "Task", ... }
```

**Successful GET (list):**
```
HTTP/1.1 200 OK
Content-Type: application/json

[ { "id": 1, ... }, { "id": 2, ... } ]
```

**Created resource:**
```
HTTP/1.1 201 Created
Location: /api/tasks/123
Content-Type: application/json

{ "id": 123, "title": "New Task", ... }
```

**Validation error:**
```
HTTP/1.1 400 Bad Request
Content-Type: application/json

{
  "status": 400,
  "message": "Validation failed",
  "errors": {
    "title": "Title is required",
    "dueDate": "Due date must be in the future"
  }
}
```

**Not found:**
```
HTTP/1.1 404 Not Found
Content-Type: application/json

{
  "status": 404,
  "message": "Task with id 999 not found"
}
```

**Unauthorized:**
```
HTTP/1.1 401 Unauthorized
Content-Type: application/json

{
  "status": 401,
  "message": "Invalid or expired token"
}
```

> **Remember:** Always include a consistent error response body with status code, message, and timestamp. Never expose internal stack traces or database details in production!
