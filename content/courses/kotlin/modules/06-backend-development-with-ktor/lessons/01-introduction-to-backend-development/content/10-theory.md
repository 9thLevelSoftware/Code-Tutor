---
type: "THEORY"
title: "✅ Solution & Explanation"
---


Here's a well-designed API for the To-Do List application:

### Endpoints


### Example Request/Response Flow

**Creating a Task:**


**Marking Task Complete:**


### Key Design Decisions

1. **Consistent naming**: All endpoints use `/api/tasks` (plural noun)
2. **Proper HTTP methods**: GET for reading, POST for creating, PUT for updating, DELETE for removing
3. **Meaningful status codes**: 201 for creation, 204 for deletion, 404 when not found
4. **Query parameters for filtering**: `?status=completed` instead of `/tasks/completed`
5. **Resource IDs in the path**: `/tasks/{id}` for specific tasks

---



```kotlin
Request:
PUT /api/tasks/42 HTTP/1.1
Content-Type: application/json
Authorization: Bearer user_token_123

{
    "completed": true
}

Response:
HTTP/1.1 200 OK
Content-Type: application/json

{
    "id": 42,
    "title": "Buy groceries",
    "description": "Milk, eggs, bread",
    "dueDate": "2024-12-01",
    "completed": true,
    "completedAt": "2024-11-13T15:45:00Z"
}
```
