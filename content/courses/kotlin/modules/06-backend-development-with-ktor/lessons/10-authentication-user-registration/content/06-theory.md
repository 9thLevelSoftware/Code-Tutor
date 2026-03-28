---
type: "THEORY"
title: "Testing User Registration"
---


### Test 1: Successful Registration


Response (201 Created):

### Test 2: Weak Password


Response (400 Bad Request):

### Test 3: Duplicate Email


Response (409 Conflict):

### Test 4: Invalid Email Format


Response (400 Bad Request):

---



```json
{
  "success": false,
  "message": "Validation failed",
  "errors": {
    "email": ["email must be a valid email address"]
  },
  "timestamp": "2025-01-15T10:33:45.012"
}
```
