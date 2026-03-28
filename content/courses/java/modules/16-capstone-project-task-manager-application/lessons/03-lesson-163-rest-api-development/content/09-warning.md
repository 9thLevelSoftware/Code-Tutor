---
type: "WARNING"
title: "Common REST API Mistakes"
---

Avoid these common mistakes when building REST APIs:

Mistake 1: Verbs in URLs
Wrong: POST /api/createTask
Right: POST /api/tasks

Mistake 2: Inconsistent naming
Wrong: /api/tasks but /api/getCategories
Right: /api/tasks and /api/categories

Mistake 3: Wrong HTTP methods
Wrong: GET /api/tasks/delete/1
Right: DELETE /api/tasks/1

Mistake 4: Not using proper status codes
Wrong: Return 200 OK with {"error": "Not found"}
Right: Return 404 Not Found with error body

Mistake 5: Exposing entity IDs for all operations
Wrong: GET /api/users/1/password
Right: Do not expose password endpoint at all

Mistake 6: No pagination for list endpoints
Wrong: GET /api/tasks returns all 10,000 tasks
Right: GET /api/tasks?page=0&size=20 with limits

Mistake 7: Returning entities instead of DTOs
Wrong: Return User entity (includes password hash)
Right: Return UserResponse DTO (no password)

Mistake 8: No input validation
Wrong: Accept any input, let database fail
Right: Validate with @Valid and return 400 for bad input

### HTTP Status Codes Quick Reference

| Code | Meaning | When to Use |
|------|---------|-------------|
| 200 | OK | Successful GET, PUT, PATCH |
| 201 | Created | Successful POST (resource created) |
| 204 | No Content | Successful DELETE |
| 400 | Bad Request | Validation errors, malformed JSON |
| 401 | Unauthorized | Missing/invalid authentication |
| 403 | Forbidden | Authenticated but not authorized |
| 404 | Not Found | Resource doesn't exist |
| 409 | Conflict | Duplicate data, concurrent modification |
| 500 | Server Error | Unexpected exceptions |